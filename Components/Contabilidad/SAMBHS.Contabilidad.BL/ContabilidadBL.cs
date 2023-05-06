using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using System.Linq.Dynamic;
using System.Transactions;
using SAMBHS.Tesoreria.BL;
using SAMBHS.CommonWIN.BL;
using System.Diagnostics;
using SAMBHS.Almacen.BL;
using SAMBHS.Common.BE.Custom;
using System.Data;
using System.Linq.Expressions;
using System.Data.Objects;
using Dapper;
using SAMBHS.Venta.BL;

namespace SAMBHS.Contabilidad.BL
{
    public class ContabilidadBL
    {
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        AsientosContablesBL _objAsientoContableBL = new AsientosContablesBL();
        public int CodigoError = 1;
        private static string periodo = Globals.ClientSession != null ? (Globals.ClientSession.i_Periodo ?? DateTime.Now.Year).ToString() : DateTime.Now.Year.ToString();
        #region Reportes
        public List<ReporteLibroMayor> ReporteLibroMayor(ref OperationResult objOperationResult, DateTime FechaInicio, DateTime FechaFin, string pstrCuentaMayor, string CuentaDesde, string CuentaHasta, int pIntMoneda, int pIntForma, string pstrOrdenar, string pstrMesSaldoAnterior, string AnioSaldoAnterior, bool CalcularSaldoAnterior, int FormatoEstructura = 1, List<int> ListaDocumentoResumir = null)
        {
            try
            {
                objOperationResult.Success = 1;
                List<ReporteLibroMayor> ListaFiltradoFormas = new List<ReporteLibroMayor>();
                List<saldoscontablesDto> AcumuladoAnterior = new List<saldoscontablesDto>();
                AsientosContablesBL _objAsientoContableBL = new AsientosContablesBL();
                if (CalcularSaldoAnterior)
                {
                    string pstrMes = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? ((int)Mes.Diciembre).ToString("00") : (int.Parse(pstrMesSaldoAnterior) - 1).ToString("00");
                    string pstrAnio = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? (int.Parse(AnioSaldoAnterior) - 1).ToString("00") : AnioSaldoAnterior;
                    DateTime FechaInicioAnterior = DateTime.Parse("01/" + pstrMes + "/" + pstrAnio);
                    DateTime FechaFinAnterior = DateTime.Parse(DateTime.DaysInMonth(int.Parse(pstrAnio), int.Parse(pstrMes)) + "/" + pstrMes + "/" + pstrAnio);
                    if (pstrMesSaldoAnterior != ((int)Mes.Enero).ToString("00"))
                    {

                        var saldoscontables = _objAsientoContableBL.ObtenerSaldosContablesPorMes(ref  objOperationResult, int.Parse(pstrMes), int.Parse(pstrAnio));
                        AcumuladoAnterior = (from n in saldoscontables
                                             select n)
                        .OrderBy(x => x.v_NroCuenta).ToList()
                        .GroupBy(x => new { x.v_NroCuenta })
                        .Select(x => new saldoscontablesDto
                        {
                            d_ImporteDolaresD = x.Sum(y => y.d_ImporteDolaresD),
                            d_ImporteDolaresH = x.Sum(y => y.d_ImporteDolaresH),
                            d_ImporteSolesD = x.Sum(y => y.d_ImporteSolesD),
                            d_ImporteSolesH = x.Sum(y => y.d_ImporteSolesH),
                            v_NroCuenta = x.FirstOrDefault().v_NroCuenta,

                        }).ToList();
                    }
                    else
                    {
                        using (var dbContext = new SAMBHSEntitiesModelWin())
                        {
                            AcumuladoAnterior = (from a in dbContext.diario
                                                 join b in dbContext.diariodetalle on new { IdDiario = a.v_IdDiario, eliminado = 0 } equals new { IdDiario = b.v_IdDiario, eliminado = b.i_Eliminado.Value } into b_join
                                                 from b in b_join.DefaultIfEmpty()
                                                 where a.i_Eliminado == 0 && a.i_IdTipoComprobante == 1 //Asientos de Apertura
                                                  && a.v_Periodo == AnioSaldoAnterior

                                                 select new saldoscontablesDto()
                                                 {

                                                     v_NroCuenta = b.v_NroCuenta.Trim(),
                                                     d_ImporteDolaresD = b.v_Naturaleza == "D" ? a.i_IdMoneda == (int)Currency.Dolares ? b.d_Importe.Value : a.i_IdMoneda == (int)Currency.Soles ? b.d_Cambio : 0 : 0,
                                                     d_ImporteSolesD = b.v_Naturaleza == "D" ? a.i_IdMoneda == (int)Currency.Soles ? b.d_Importe.Value : a.i_IdMoneda == (int)Currency.Dolares ? b.d_Cambio : 0 : 0,
                                                     d_ImporteDolaresH = b.v_Naturaleza == "H" ? a.i_IdMoneda == (int)Currency.Dolares ? b.d_Importe.Value : a.i_IdMoneda == (int)Currency.Soles ? b.d_Cambio : 0 : 0,
                                                     d_ImporteSolesH = b.v_Naturaleza == "H" ? a.i_IdMoneda == (int)Currency.Soles ? b.d_Importe.Value : a.i_IdMoneda == (int)Currency.Dolares ? b.d_Cambio : 0 : 0,

                                                 }).OrderBy(x => x.v_NroCuenta).ToList().GroupBy(o => o.v_NroCuenta).Select(o => new saldoscontablesDto
                                                 {
                                                     d_ImporteDolaresD = o.Sum(y => y.d_ImporteDolaresD),
                                                     d_ImporteDolaresH = o.Sum(y => y.d_ImporteDolaresH),
                                                     d_ImporteSolesD = o.Sum(y => y.d_ImporteSolesD),
                                                     d_ImporteSolesH = o.Sum(y => y.d_ImporteSolesH),
                                                     v_NroCuenta = o.FirstOrDefault().v_NroCuenta,


                                                 }).ToList();

                        }

                    }
                }


                var rangoCtas = new List<string>();
                if (!string.IsNullOrEmpty(CuentaDesde) && !string.IsNullOrEmpty(CuentaHasta))
                {
                    rangoCtas = Utils.Windows.RangoDeCuentas(CuentaDesde, CuentaHasta);
                }
                ListaFiltradoFormas = ListaSegunMesReporteLibroMayorII(ref objOperationResult, FechaInicio, FechaFin, pstrCuentaMayor, pIntMoneda, pIntForma, pstrOrdenar, AcumuladoAnterior, pstrMesSaldoAnterior, rangoCtas, FormatoEstructura, ListaDocumentoResumir);
                return ListaFiltradoFormas.AsQueryable().OrderBy(pstrOrdenar).ToList();

            }
            catch (Exception ex)
            {
                
                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "ContabilidadBL.ReporteLibroMayor()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }
        }

        public List<ReporteLibroMayor> ListaSegunMesReporteLibroMayorII(ref OperationResult objOperationResult, DateTime FechaInicio, DateTime FechaFin, string pstrCuentaMayor, int pIntMoneda, int pIntForma, string pstrOrdenar, List<saldoscontablesDto> AcumuladoAnterior, string pstrMesSaldoAnterior, List<string> rangoCtas,int FormatoEstructura, List<int> ListaResumir)
        {

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    List<asientocontable> ListaAsientoContable = dbContext.asientocontable.ToList();
                    var objContabilidadDAO = new ContabilidadDao();
                    List<ReporteLibroMayor> ReporteFinal = new List<ReporteLibroMayor>();
                    List<ReporteLibroMayor> ListaFiltradoFormas = new List<ReporteLibroMayor>();
                    var ValoresTipodocTrabajadores = dbContext.datahierarchy.Where(c => c.i_GroupId == 132 && c.i_IsDeleted == 0).ToList();
                    objOperationResult.Success = 1;

                    var QueryUnion = objContabilidadDAO.ObtenerDataLibroMayor(ref objOperationResult, FechaInicio, FechaFin, pstrCuentaMayor, rangoCtas,
                        pIntMoneda, AcumuladoAnterior, pstrMesSaldoAnterior, FormatoEstructura, ListaResumir);
                    var QueryFaltantes = QueryUnion;
                    var CuentasFaltantes = AcumuladoAnterior.Select(l => l.v_NroCuenta).Except(QueryFaltantes.Select(l => l.numeroCuenta)).Distinct().ToList();
                    List<ReporteLibroMayor> ListaFaltantes = new List<ReporteLibroMayor>();

                   
                    
                    foreach (var item in CuentasFaltantes)
                    {
                        var objFaltante = new ReporteLibroMayor();
                        objFaltante.cuentaMayor = "CUENTA : " + item.Substring(0, 2) + " - " + NombreCuenta(item.Substring(0, 2), ListaAsientoContable);

                        objFaltante.cuenta = item.Trim() + " - " + NombreCuenta(item.Trim(), ListaAsientoContable);
                        objFaltante.numeroCuenta = item;
                        objFaltante.nroComprobante = "**SIN OPERACIONES**";
                        var AcumuladosAnteriores = AcumuladoAnterior.Where(y => y.v_NroCuenta.Trim() == item).FirstOrDefault();
                        objFaltante.acumuladoAnteriorDebeSoles = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? 0 : AcumuladosAnteriores == null ? 0 : AcumuladosAnteriores.d_ImporteSolesD.Value;
                        objFaltante.acumuladoAnteriorDebeDolares = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? 0 : AcumuladosAnteriores == null ? 0 : AcumuladosAnteriores.d_ImporteDolaresD.Value;
                        objFaltante.acumuladoAnteriorHaberSoles = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? 0 : AcumuladosAnteriores == null ? 0 : AcumuladosAnteriores.d_ImporteSolesH.Value;
                        objFaltante.acumuladoAnteriorHaberDolares = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? 0 : AcumuladosAnteriores == null ? 0 : AcumuladosAnteriores.d_ImporteDolaresH.Value;
                        objFaltante.saldoAnteriorSoles = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? 0 : AcumuladosAnteriores == null ? 0 : AcumuladosAnteriores != null ? AcumuladosAnteriores.d_ImporteSolesD.Value - AcumuladosAnteriores.d_ImporteSolesH.Value : AcumuladosAnteriores == null ? -AcumuladosAnteriores.d_ImporteSolesH.Value : AcumuladosAnteriores.d_ImporteSolesD.Value; // AcumuladoAnterior.Where(y => y.v_NroCuenta.Trim() == x.numeroCuenta.Trim()).FirstOrDefault().d_ImporteSolesD.Value - AcumuladoAnterior.Where(y => y.v_NroCuenta.Trim() == x.numeroCuenta.Trim()).FirstOrDefault().d_ImporteSolesH.Value ,
                        objFaltante.saldoAnteriorDolares = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? 0 : AcumuladosAnteriores == null ? 0 : AcumuladosAnteriores != null ? AcumuladosAnteriores.d_ImporteDolaresD.Value - AcumuladosAnteriores.d_ImporteDolaresH.Value : AcumuladosAnteriores == null ? -AcumuladosAnteriores.d_ImporteDolaresH.Value : AcumuladosAnteriores.d_ImporteDolaresD.Value;  //AcumuladoAnterior.Where(y => y.v_NroCuenta.Trim() == x.numeroCuenta.Trim()).FirstOrDefault().d_ImporteDolaresD.Value - AcumuladoAnterior.Where(y => y.v_NroCuenta.Trim() == x.numeroCuenta.Trim()).FirstOrDefault().d_ImporteDolaresH.Value, i_CodigoDocumento
                        ListaFaltantes.Add(objFaltante);
                    }
                    if (!string.IsNullOrEmpty(pstrOrdenar))
                    {
                        QueryUnion = QueryUnion.AsQueryable().OrderBy(pstrOrdenar).ToList(); //Concat (ListaFaltantes).OrderBy(pstrOrdenar);
                    }

                    if (pIntForma != -1)
                    {
                        ListaFiltradoFormas = pIntForma == 1 ? QueryUnion.Where(x => x.TipoComprobante != 5).ToList() : pIntForma == 2 ? QueryUnion.ToList() : pIntForma == 3 ? QueryUnion.Where(x => x.TipoComprobante == 5).ToList() : null;
                    }
                    else
                    {
                        ListaFiltradoFormas = QueryUnion.ToList();
                    }
                    return ListaFiltradoFormas;
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "ContabilidadBL.ListaSegunMesReporteLibroMayorII()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;

            }

        }

        public string NombreCuenta(string pstrNumeroCuenta, List<asientocontable> asientocobtanle)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var NombreCuentaMayor = (from n in asientocobtanle  // dbContext.asientocontable
                                         where n.i_Eliminado == 0 && n.v_NroCuenta == pstrNumeroCuenta && n.v_Periodo == periodo
                                         select n.v_NombreCuenta.ToUpper()).FirstOrDefault();
                return NombreCuentaMayor == null ? "*No Existe*" : NombreCuentaMayor.Trim();
            }
        }

        public List<CuentaImputable> NombreCuentaImputable(string pstrCuenta)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                List<CuentaImputable> Lista = new List<CuentaImputable>();

                var Cuenta = (from n in dbContext.asientocontable
                              where n.i_Eliminado == 0 && n.v_NroCuenta == pstrCuenta && n.v_Periodo == periodo
                              select new CuentaImputable
                              {
                                  cuenta = n.v_NombreCuenta,
                                  imputable = n.i_Imputable.Value,
                                  Naturaleza = n.i_Naturaleza == 1 ? "D" : "H",

                              }).ToList();

                return Cuenta;
            }
        }
        public List<CuentaImputable> NombreCuentaImputable(string pstrCuenta, List<asientocontable> ListaAsiento)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                List<CuentaImputable> Lista = new List<CuentaImputable>();

                var Cuenta = (from n in ListaAsiento
                              where n.i_Eliminado == 0 && n.v_NroCuenta == pstrCuenta && n.v_Periodo == periodo
                              select new CuentaImputable
                              {
                                  cuenta = n.v_NombreCuenta,
                                  imputable = n.i_Imputable.Value,
                                  Naturaleza = n.i_Naturaleza == 1 ? "D" : "H",

                              }).ToList();

                return Cuenta;
            }
        }

        public List<ReporteLibroCajayBancos> ReporteLibroCajayBancos(ref OperationResult objOperationResult, DateTime FechaInicio, DateTime FechaFin, int pIntMoneda, string pstrOrdenar, string pstrMesSaldoAnterior, string AnioSaldoAnteior, int Formato)
        {
            try
            {
                ////
                ///  Recopila todas las cuentas de tesoreria 
                /// Formato 1.1 -->Todas las cuentas  101
                ///Formato 1.2 -->> Todas las cuentas diferenes 101
                ///En reporte se debe mostrar las solo las cuentas asociadas a cada una de estas cuentas con naturaleza contraria
                //
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    pstrOrdenar = "OrdenAdicional," + pstrOrdenar;
                    List<asientocontable> ListaAsientoContable = dbContext.asientocontable.ToList();
                    List<ReporteLibroCajayBancos> ReporteFinal = new List<ReporteLibroCajayBancos>();
                    List<ReporteLibroCajayBancos> ListaFiltradoFormas = new List<ReporteLibroCajayBancos>();
                    List<saldoscontablesDto> AcumuladoAnterior = new List<saldoscontablesDto>();
                    List<ReporteLibroCajayBancos> queryFinal = new List<ReporteLibroCajayBancos>();
                    ContabilidadBL _objContabilidadBL = new ContabilidadBL();

                    string pstrMes = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? ((int)Mes.Diciembre).ToString("00") : (int.Parse(pstrMesSaldoAnterior) - 1).ToString("00");
                    string pstrAnio = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? (int.Parse(AnioSaldoAnteior) - 1).ToString() : AnioSaldoAnteior;


                    if (pstrMesSaldoAnterior != ((int)Mes.Enero).ToString("00"))
                    {

                        var saldoscontables = _objAsientoContableBL.ObtenerSaldosContablesPorMes(ref  objOperationResult, int.Parse(pstrMes), int.Parse(pstrAnio), 1, false, false, "10");
                        AcumuladoAnterior = (from n in saldoscontables
                                             select n)
                        .OrderBy(x => x.v_NroCuenta).ToList()
                        .GroupBy(x => new { x.v_NroCuenta })
                        .Select(x => new saldoscontablesDto
                        {
                            d_ImporteDolaresD = x.Sum(y => y.d_ImporteDolaresD),
                            d_ImporteDolaresH = x.Sum(y => y.d_ImporteDolaresH),
                            d_ImporteSolesD = x.Sum(y => y.d_ImporteSolesD),
                            d_ImporteSolesH = x.Sum(y => y.d_ImporteSolesH),
                            v_NroCuenta = x.FirstOrDefault().v_NroCuenta,

                        }).ToList();
                    }
                    else
                    {

                        AcumuladoAnterior = (from a in dbContext.diario
                                             join b in dbContext.diariodetalle on new { IdDiario = a.v_IdDiario, eliminado = 0 } equals new { IdDiario = b.v_IdDiario, eliminado = b.i_Eliminado.Value } into b_join
                                             from b in b_join.DefaultIfEmpty()
                                             where a.i_Eliminado == 0 && a.i_IdTipoComprobante == 1 //Asientos de Apertura
                                              && a.v_Periodo == AnioSaldoAnteior

                                             select new saldoscontablesDto()
                                             {

                                                 v_NroCuenta = b.v_NroCuenta.Trim(),
                                                 d_ImporteDolaresD = b.v_Naturaleza == "D" ? a.i_IdMoneda == (int)Currency.Dolares ? b.d_Importe.Value : a.i_IdMoneda == (int)Currency.Soles ? b.d_Cambio : 0 : 0,
                                                 d_ImporteSolesD = b.v_Naturaleza == "D" ? a.i_IdMoneda == (int)Currency.Soles ? b.d_Importe.Value : a.i_IdMoneda == (int)Currency.Dolares ? b.d_Cambio : 0 : 0,
                                                 d_ImporteDolaresH = b.v_Naturaleza == "H" ? a.i_IdMoneda == (int)Currency.Dolares ? b.d_Importe.Value : a.i_IdMoneda == (int)Currency.Soles ? b.d_Cambio : 0 : 0,
                                                 d_ImporteSolesH = b.v_Naturaleza == "H" ? a.i_IdMoneda == (int)Currency.Soles ? b.d_Importe.Value : a.i_IdMoneda == (int)Currency.Dolares ? b.d_Cambio : 0 : 0,

                                             }).OrderBy(x => x.v_NroCuenta).ToList();


                    }
                    List<ReporteLibroCajayBancos> LisT1 = new List<ReporteLibroCajayBancos>();
                    List<ReporteLibroCajayBancos> LisT2 = new List<ReporteLibroCajayBancos>();
                    List<ReporteLibroCajayBancos> LisD1 = new List<ReporteLibroCajayBancos>();
                    List<ReporteLibroCajayBancos> LisD2 = new List<ReporteLibroCajayBancos>();
                    ReporteLibroCajayBancos objRep = new ReporteLibroCajayBancos();
                    if (Formato == 1)
                    {
                        #region Formato 1
                        //Recopila informacion solamente de la cuenta 101

                        var queryTesoreria = (from a in dbContext.tesoreria

                                              join b in dbContext.tesoreriadetalle on new { Tesoreria = a.v_IdTesoreria, eliminado = 0 }
                                                                                   equals new { Tesoreria = b.v_IdTesoreria, eliminado = b.i_Eliminado.Value } into b_join

                                              from b in b_join.DefaultIfEmpty()

                                              join c in dbContext.documento on new { tipodoc = a.i_IdTipoDocumento.Value, eliminado = 0 }
                                                                           equals new { tipodoc = c.i_CodigoDocumento, eliminado = c.i_Eliminado.Value } into c_join
                                              from c in c_join.DefaultIfEmpty()

                                              join d in dbContext.documento on new { DocRef = b.i_IdTipoDocumento.Value, eliminado = 0 }
                                                                           equals new { DocRef = d.i_CodigoDocumento, eliminado = d.i_Eliminado.Value } into d_join
                                              from d in d_join.DefaultIfEmpty()

                                              join e in dbContext.asientocontable on new { nroCuenta = b.v_NroCuenta, eliminado = 0, p = periodo }
                                                                                  equals new { nroCuenta = e.v_NroCuenta, eliminado = e.i_Eliminado.Value, p = e.v_Periodo } into e_join

                                              from e in e_join.DefaultIfEmpty()

                                              join f in dbContext.datahierarchy on new { medioPago = a.i_IdMedioPago.Value, eliminado = 0, grupo = 44 }
                                                                                equals new { medioPago = f.i_ItemId, eliminado = f.i_IsDeleted.Value, grupo = f.i_GroupId } into f_join
                                              from f in f_join.DefaultIfEmpty()
                                              join g in dbContext.datahierarchy on new { Grupo = 111, item = e.i_EntFinanciera.Value, eliminado = 0 }
                                                                          equals new { Grupo = g.i_GroupId, item = g.i_ItemId, eliminado = g.i_IsDeleted.Value } into g_join
                                              from g in g_join.DefaultIfEmpty()

                                              where a.i_Eliminado == 0 && a.t_FechaRegistro >= FechaInicio && a.t_FechaRegistro <= FechaFin
                                              && (b.i_EsDestino == "0" || b.i_EsDestino == null)

                                              && a.i_IdEstado == 1

                                              // && a.v_IdTesoreria == "N001-XE000002842"
                                              orderby b.v_NroCuenta
                                              select new ReporteLibroCajayBancos
                                              {

                                                  fecha = a.t_FechaRegistro.Value,
                                                  nroComprobante = c == null ? a.v_Mes.Trim() + "-" + a.v_Correlativo.Trim() : c.v_Siglas.Trim() + " " + a.v_Mes.Trim() + "-" + a.v_Correlativo.Trim(),
                                                  docReferencia = d == null ? b.v_NroDocumento : d.v_Siglas.Trim() + " " + b.v_NroDocumento.Trim(),
                                                  nombre = a.v_Nombre,
                                                  descripcionOperacion = string .IsNullOrEmpty ( b.v_Analisis ) ? string.IsNullOrEmpty ( a.v_Glosa )? "" : a.v_Glosa : b.v_Analisis,
                                                  haberSoles = b.v_Naturaleza == "D" && a.i_IdMoneda.Value == (int)Currency.Soles && pIntMoneda == (int)Currency.Soles ? b.d_Importe.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Dolares && a.i_IdMoneda == (int)Currency.Soles ? b.d_Importe.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Soles && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Cambio.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Dolares && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Cambio.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Todas && a.i_IdMoneda == (int)Currency.Soles ? b.d_Importe.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Todas && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Cambio.Value : 0,
                                                  haberDolares = b.v_Naturaleza == "D" && a.i_IdMoneda.Value == (int)Currency.Dolares && pIntMoneda == (int)Currency.Dolares ? b.d_Importe.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Dolares && a.i_IdMoneda == (int)Currency.Soles ? b.d_Cambio.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Soles && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Importe.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Soles && a.i_IdMoneda == (int)Currency.Soles ? b.d_Cambio.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Todas && a.i_IdMoneda == (int)Currency.Soles ? b.d_Cambio.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Todas && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Importe.Value : 0,
                                                  debeSoles = b.v_Naturaleza == "H" && a.i_IdMoneda.Value == (int)Currency.Soles && pIntMoneda == (int)Currency.Soles ? b.d_Importe.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Dolares && a.i_IdMoneda == (int)Currency.Soles ? b.d_Importe.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Soles && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Cambio.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Dolares && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Cambio.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Todas && a.i_IdMoneda == (int)Currency.Soles ? b.d_Importe.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Todas && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Cambio.Value : 0,
                                                  debeDolares = b.v_Naturaleza == "H" && a.i_IdMoneda.Value == (int)Currency.Dolares && pIntMoneda == (int)Currency.Dolares ? b.d_Importe.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Dolares && a.i_IdMoneda == (int)Currency.Soles ? b.d_Cambio.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Soles && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Importe.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Soles && a.i_IdMoneda == (int)Currency.Soles ? b.d_Cambio.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Todas && a.i_IdMoneda == (int)Currency.Soles ? b.d_Cambio.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Todas && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Importe.Value : 0,
                                                  cuentaMayor = "",
                                                  cuenta = e == null ? "*NO EXISTE CUENTA*" : e.v_NombreCuenta.Trim(),
                                                  tipoComprobante = 0,
                                                  numeroCuenta = b.v_NroCuenta.Trim(),
                                                  monedaTransaccion = a.i_IdMoneda.Value == 1 ? "S/." : "US$.",
                                                  medioPago = f.v_Value2,
                                                  entidadFinanciera = g == null ? "" : g.v_Value1,
                                                  IdTesoreria = a.v_IdTesoreria,
                                                  v_IdTesoreriaDetalle = b.v_IdTesoreriaDetalle,
                                                  cuentaGrupo = b.v_NroCuenta,
                                                  OrdenAdicional = "03",
                                              }).ToList();

                        ////Tesorerias que contiene cuentas 101  y  cuentas diferentes a la 101 (42,60)
                        /// 1011101 D
                        /// 4211101 H
                        /// 6211101H
                        var TodasTesorerias = queryTesoreria.GroupBy(o => o.IdTesoreria).ToList();
                        var SoloTesoreria101 = TodasTesorerias.Where(o => o.Any(x => x.numeroCuenta.StartsWith("101"))).ToList();
                        var Tesoreriassin101 = SoloTesoreria101.Select(g => g.Where(o => !o.numeroCuenta.StartsWith("101"))).ToList();
                        List<ReporteLibroCajayBancos> ListaTesoreriasSin101 = Tesoreriassin101.SelectMany(o => o).ToList();
                        var LisT1111 = ListaTesoreriasSin101.Select(x =>
                         {
                             var Detalles = queryTesoreria.Where(o => o.IdTesoreria == x.IdTesoreria && o.numeroCuenta.StartsWith("101")).ToList();
                             var solocuenta10 = Detalles.Where(o => o.IdTesoreria == x.IdTesoreria && o.numeroCuenta.StartsWith("101")).ToList().Select(o => o.numeroCuenta).Distinct().ToList();
                             foreach (var detallecuenta10 in solocuenta10)
                             {
                                 objRep = (ReporteLibroCajayBancos)x.Clone();
                                 objRep.Grupo = detallecuenta10 != null ? "CUENTA : " + detallecuenta10 + " " + _objContabilidadBL.NombreCuenta(detallecuenta10, ListaAsientoContable) : "";
                                 objRep.cuentaGrupo = detallecuenta10 != null ? detallecuenta10 : "";
                                 LisT1.Add(objRep);
                                 var DetallesoOtros = Detalles.Where(o => o.numeroCuenta != detallecuenta10).ToList();
                                 foreach (var detallitosOtros in DetallesoOtros)
                                 {
                                     if (!LisT1.Select(p => p.v_IdTesoreriaDetalle + p.cuentaGrupo).Contains(detallitosOtros.v_IdTesoreriaDetalle + detallecuenta10))
                                     {
                                         objRep = (ReporteLibroCajayBancos)detallitosOtros.Clone();
                                         objRep.Grupo = detallecuenta10 != null ? "CUENTA : " + detallecuenta10 + " " + _objContabilidadBL.NombreCuenta(detallecuenta10, ListaAsientoContable) : "";
                                         objRep.cuentaGrupo = detallecuenta10 != null ? detallecuenta10 : "";
                                         LisT1.Add(objRep);
                                     }
                                 }

                             }
                             return LisT1;
                         }).ToList();

                        ////Tesorerias que contiene solo cuentas 101  ,porque no estarían incluidas en la consulta anterior
                        /// 1011101 D
                        /// 1011102 H
                        var TesoreriaSolo10 = TodasTesorerias.Where(o => o.All(x => x.numeroCuenta.StartsWith("101"))).ToList();
                        List<ReporteLibroCajayBancos> ListaTesoreriasSolo10 = TesoreriaSolo10.SelectMany(o => o).ToList();
                        var ListaFinal2 = ListaTesoreriasSolo10.Select(x =>
                        {
                            var Detalles = queryTesoreria.Where(o => o.IdTesoreria == x.IdTesoreria && o.numeroCuenta.StartsWith("101") && o.numeroCuenta != x.numeroCuenta).ToList();
                            var solocuenta10 = Detalles.Where(o => o.IdTesoreria == x.IdTesoreria && o.numeroCuenta.StartsWith("101")).ToList().Select(o => o.numeroCuenta).Distinct().ToList();
                            foreach (var detallecuenta10 in solocuenta10)
                            {

                                objRep = new ReporteLibroCajayBancos();
                                objRep = (ReporteLibroCajayBancos)x.Clone();
                                objRep.Grupo = detallecuenta10 != null ? "CUENTA : " + detallecuenta10 + " " + _objContabilidadBL.NombreCuenta(detallecuenta10, ListaAsientoContable) : "";
                                objRep.cuentaGrupo = detallecuenta10 != null ? detallecuenta10 : "";
                                if (!LisT2.Select(p => p.v_IdTesoreriaDetalle + p.cuentaGrupo).Contains(objRep.v_IdTesoreriaDetalle + detallecuenta10))
                                {
                                    LisT2.Add(objRep);
                                }
                                var DetallesoOtros = Detalles.Where(o => o.numeroCuenta != detallecuenta10).ToList();
                                foreach (var detallitosOtros in DetallesoOtros)
                                {
                                    if (!LisT2.Select(p => p.v_IdTesoreriaDetalle + p.cuentaGrupo).Contains(detallitosOtros.v_IdTesoreriaDetalle + detallecuenta10))
                                    {
                                        objRep = (ReporteLibroCajayBancos)detallitosOtros.Clone();
                                        objRep.Grupo = detallecuenta10 != null ? "CUENTA : " + detallecuenta10 + " " + _objContabilidadBL.NombreCuenta(detallecuenta10, ListaAsientoContable) : "";
                                        objRep.cuentaGrupo = detallecuenta10 != null ? detallecuenta10 : "";
                                        LisT2.Add(objRep);
                                    }
                                }
                            }
                            return LisT2;
                        }).ToList();


                        var queryDiario = (from a in dbContext.diario

                                           join b in dbContext.diariodetalle on new { dd = a.v_IdDiario, eliminado = 0 }
                                                                              equals new { dd = b.v_IdDiario, eliminado = b.i_Eliminado.Value } into b_join

                                           from b in b_join.DefaultIfEmpty()

                                           join c in dbContext.documento on new { documento = a.i_IdTipoDocumento.Value, eliminado = 0 }
                                                                          equals new { documento = c.i_CodigoDocumento, eliminado = c.i_Eliminado.Value } into c_join
                                           from c in c_join.DefaultIfEmpty()

                                           join d in dbContext.documento on new { documento = b.i_IdTipoDocumento.Value, eliminado = 0 }
                                                                       equals new { documento = d.i_CodigoDocumento, eliminado = d.i_Eliminado.Value } into d_join
                                           from d in d_join.DefaultIfEmpty()

                                           join e in dbContext.asientocontable on new { nroCuenta = b.v_NroCuenta, eliminado = 0, p = periodo }
                                                                       equals new { nroCuenta = e.v_NroCuenta, eliminado = e.i_Eliminado.Value, p = e.v_Periodo } into e_join

                                           from e in e_join.DefaultIfEmpty()
                                           join g in dbContext.datahierarchy on new { Grupo = 111, item = e.i_EntFinanciera.Value, eliminado = 0 }
                                                                           equals new { Grupo = g.i_GroupId, item = g.i_ItemId, eliminado = g.i_IsDeleted.Value } into g_join
                                           from g in g_join.DefaultIfEmpty()

                                           where a.i_Eliminado == 0 && a.t_Fecha >= FechaInicio && a.t_Fecha <= FechaFin


                                           && (b.i_EsDestino == null || b.i_EsDestino == "0")


                                           orderby b.v_NroCuenta

                                           select new ReporteLibroCajayBancos
                                           {

                                               fecha = a.t_Fecha.Value,
                                               nroComprobante = c == null ? a.v_Mes + "-" + a.v_Correlativo.Trim() : c.v_Siglas.Trim() + " " + a.v_Mes + "-" + a.v_Correlativo.Trim(),
                                               docReferencia = d == null ? b.v_NroDocumento : d.v_Siglas.Trim() + " " + b.v_NroDocumento.Trim(),
                                               nombre = a.v_Nombre,
                                               descripcionOperacion = string.IsNullOrEmpty ( b.v_Analisis ) ? string.IsNullOrEmpty ( a.v_Glosa)? "" : a.v_Glosa : b.v_Analisis,
                                               haberSoles = b.v_Naturaleza == "D" && a.i_IdMoneda.Value == (int)Currency.Soles && pIntMoneda == (int)Currency.Soles ? b.d_Importe.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Dolares && a.i_IdMoneda == (int)Currency.Soles ? b.d_Importe.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Soles && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Cambio.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Dolares && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Cambio.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Todas && a.i_IdMoneda == (int)Currency.Soles ? b.d_Importe.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Todas && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Cambio.Value : 0,
                                               haberDolares = b.v_Naturaleza == "D" && a.i_IdMoneda.Value == (int)Currency.Dolares && pIntMoneda == (int)Currency.Dolares ? b.d_Importe.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Dolares && a.i_IdMoneda == (int)Currency.Soles ? b.d_Cambio.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Soles && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Importe.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Soles && a.i_IdMoneda == (int)Currency.Soles ? b.d_Cambio.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Todas && a.i_IdMoneda == (int)Currency.Soles ? b.d_Cambio.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Todas && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Importe.Value : 0,
                                               debeSoles = b.v_Naturaleza == "H" && a.i_IdMoneda.Value == (int)Currency.Soles && pIntMoneda == (int)Currency.Soles ? b.d_Importe.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Dolares && a.i_IdMoneda == (int)Currency.Soles ? b.d_Importe.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Soles && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Cambio.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Dolares && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Cambio.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Todas && a.i_IdMoneda == (int)Currency.Soles ? b.d_Importe.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Todas && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Cambio.Value : 0,
                                               debeDolares = b.v_Naturaleza == "H" && a.i_IdMoneda.Value == (int)Currency.Dolares && pIntMoneda == (int)Currency.Dolares ? b.d_Importe.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Dolares && a.i_IdMoneda == (int)Currency.Soles ? b.d_Cambio.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Soles && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Importe.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Soles && a.i_IdMoneda == (int)Currency.Soles ? b.d_Cambio.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Todas && a.i_IdMoneda == (int)Currency.Soles ? b.d_Cambio.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Todas && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Importe.Value : 0,
                                               cuentaMayor = "",
                                               cuenta = e == null ? "*NO EXISTE CUENTA*" : e.v_NombreCuenta.Trim(),
                                               tipoComprobante = a.i_IdTipoComprobante.Value,
                                               numeroCuenta = b.v_NroCuenta.Trim(),
                                               monedaTransaccion = a.i_IdMoneda == 1 ? "S/." : "US$.",
                                               medioPago = "",
                                               entidadFinanciera = g == null ? "" : g.v_Value1,
                                               IdTesoreria = a.v_IdDiario,
                                               Apertura = a.i_IdTipoComprobante ?? -1,
                                               OrdenAdicional = a.i_IdTipoComprobante == 1 ? "01" : "02",
                                           }).ToList();

                        var SoloApertura = queryDiario.Where(o => o.Apertura == 1 && o.numeroCuenta.StartsWith("101")).Select(o =>
                            {
                                var ap = o;
                                ap.Grupo = "CUENTA : " + o.numeroCuenta + " " + _objContabilidadBL.NombreCuenta(o.numeroCuenta, ListaAsientoContable);
                                ap.cuentaGrupo = o.numeroCuenta;
                                return ap;

                            }).ToList();

                        var TodosDiarios = queryDiario.Where(o => o.Apertura != 1).GroupBy(o => o.IdTesoreria).ToList();
                        var SoloDiario101 = TodosDiarios.Where(o => o.Any(x => x.numeroCuenta.StartsWith("101"))).ToList();
                        var Diariossin101 = SoloDiario101.Select(g => g.Where(o => !o.numeroCuenta.StartsWith("101"))).ToList();

                        List<ReporteLibroCajayBancos> ListaDiariosSin101 = Diariossin101.SelectMany(o => o).ToList();
                        ListaDiariosSin101 = ListaDiariosSin101.Select(x =>
                        {

                            var solocuenta10 = queryDiario.Where(o => o.IdTesoreria == x.IdTesoreria && o.numeroCuenta.StartsWith("101")).FirstOrDefault();
                            var g = x;
                            g.Grupo = solocuenta10 != null ? "CUENTA : " + solocuenta10.numeroCuenta + " " + _objContabilidadBL.NombreCuenta(solocuenta10.numeroCuenta, ListaAsientoContable) : "";
                            g.cuentaGrupo = solocuenta10 != null ? solocuenta10.numeroCuenta : "";
                            return g;
                        }).ToList();

                        ////Diarios que contiene solo cuentas 101  ,porque no estarían incluidas en la consulta anterior
                        /// 1011101 D
                        /// 1011102 H
                        var DiariosSolo10 = TodosDiarios.Where(o => o.All(x => x.numeroCuenta.StartsWith("101"))).ToList();
                        List<ReporteLibroCajayBancos> ListaDiariosSolo10 = DiariosSolo10.SelectMany(o => o).ToList();
                        var ListaFinalDiarios = ListaDiariosSolo10.Select(x =>
                        {

                            var Detalles = queryDiario.Where(o => o.IdTesoreria == x.IdTesoreria && o.numeroCuenta.StartsWith("101") && o.numeroCuenta != x.numeroCuenta).ToList();
                            var solocuenta10 = Detalles.Where(o => o.IdTesoreria == x.IdTesoreria && o.numeroCuenta.StartsWith("101")).ToList().Select(o => o.numeroCuenta).Distinct().ToList();
                            foreach (var detallecuenta10 in solocuenta10)
                            {

                                objRep = new ReporteLibroCajayBancos();
                                objRep = (ReporteLibroCajayBancos)x.Clone();
                                objRep.Grupo = detallecuenta10 != null ? "CUENTA : " + detallecuenta10 + " " + _objContabilidadBL.NombreCuenta(detallecuenta10, ListaAsientoContable) : "";
                                objRep.cuentaGrupo = detallecuenta10 != null ? detallecuenta10 : "";

                                LisD2.Add(objRep);
                                var DetallesoOtros = Detalles.Where(o => o.numeroCuenta != detallecuenta10).ToList();
                                foreach (var detallitosOtros in DetallesoOtros)
                                {
                                    if (!LisD2.Select(p => p.v_IdTesoreriaDetalle + p.cuentaGrupo).Contains(detallitosOtros.v_IdTesoreriaDetalle + detallecuenta10))
                                    {
                                        objRep = (ReporteLibroCajayBancos)detallitosOtros.Clone();
                                        objRep.Grupo = detallecuenta10 != null ? "CUENTA : " + detallecuenta10 + " " + _objContabilidadBL.NombreCuenta(detallecuenta10, ListaAsientoContable) : "";
                                        objRep.cuentaGrupo = detallecuenta10 != null ? detallecuenta10 : "";
                                        LisD2.Add(objRep);
                                    }
                                }
                            }
                            return LisD2;
                        }).ToList();

                        // LisD = .ToList();
                        var QueryUnion = LisT1.Concat(LisT2).Concat(SoloApertura).Concat(ListaDiariosSin101).Concat(LisD2).AsQueryable();

                        if (!string.IsNullOrEmpty(pstrOrdenar))
                        {
                            QueryUnion = QueryUnion.OrderBy(pstrOrdenar);
                        }
                        ListaFiltradoFormas = QueryUnion.ToList();

                        string max = ListaFiltradoFormas.Count != 0 ? ListaFiltradoFormas.Max(p => int.Parse(p.numeroCuenta)).ToString() : string.Empty;

                        queryFinal = (from A in ListaFiltradoFormas
                                      select new ReporteLibroCajayBancos
                                      {
                                          fecha = A.fecha,
                                          nroComprobante = A.nroComprobante,
                                          docReferencia = A.docReferencia,
                                          nombre = A.nombre,
                                          descripcionOperacion = A.descripcionOperacion,
                                          debeSoles = A.Apertura == 1 ? A.haberSoles : A.debeSoles,
                                          debeDolares = A.Apertura == 1 ? A.haberDolares : A.debeDolares,
                                          haberSoles = A.Apertura == 1 ? A.debeSoles : A.haberSoles,
                                          haberDolares = A.Apertura == 1 ? A.debeDolares : A.haberDolares,
                                          cuentaMayor = A.numeroCuenta.Substring(0, 2) + "      CUENTA  MAYOR: " + NombreCuenta(A.numeroCuenta.Substring(0, 2), ListaAsientoContable),
                                          cuenta = A.numeroCuenta + "       " + NombreCuenta(A.numeroCuenta, ListaAsientoContable),
                                          acumuladoAnteriorDebeSoles = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? 0 : AcumuladoAnterior.Where(y => y.v_NroCuenta == A.cuentaGrupo).Sum(x => x.d_ImporteSolesD.Value),
                                          acumuladoAnteriorDebeDolares = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? 0 : AcumuladoAnterior.Where(y => y.v_NroCuenta == A.cuentaGrupo).Sum(x => x.d_ImporteDolaresD.Value),
                                          acumuladoAnteriorHaberSoles = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? 0 : AcumuladoAnterior.Where(y => y.v_NroCuenta == A.cuentaGrupo).Sum(x => x.d_ImporteSolesH.Value),
                                          acumuladoAnteriorHaberDolares = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? 0 : AcumuladoAnterior.Where(y => y.v_NroCuenta == A.cuentaGrupo).Sum(x => x.d_ImporteDolaresH.Value),
                                          monedaTransaccion = A.monedaTransaccion,
                                          medioPago = A.medioPago,
                                          MaxNumeroCuenta = max,
                                          numeroCuenta = A.numeroCuenta,
                                          entidadFinanciera = "ENTIDAD FINANCIERA : " + A.entidadFinanciera,
                                          Grupo = A.Grupo,
                                          FechaS = A.fecha.Date.Day.ToString("00") + "/" + A.fecha.Date.Month.ToString("00") + "/" + A.fecha.Date.Year.ToString(),

                                      }).ToList();

                        if (!queryFinal.Any())
                        {

                            AcumuladoAnterior = AcumuladoAnterior.Where(o => o.v_NroCuenta.StartsWith("101")).ToList();
                            foreach (var item in AcumuladoAnterior.Select(x => x.v_NroCuenta).Distinct().ToList())
                            {
                                ReporteLibroCajayBancos obj = new ReporteLibroCajayBancos();
                                obj.Grupo = "CUENTA : " + item + " " + _objContabilidadBL.NombreCuenta(item, ListaAsientoContable);
                                obj.descripcionOperacion = "** SIN OPERACIONES **";
                                obj.acumuladoAnteriorDebeSoles = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? 0 : AcumuladoAnterior.Where(y => y.v_NroCuenta == item).Sum(x => x.d_ImporteSolesD.Value);
                                obj.acumuladoAnteriorDebeDolares = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? 0 : AcumuladoAnterior.Where(y => y.v_NroCuenta == item).Sum(x => x.d_ImporteDolaresD.Value);
                                obj.acumuladoAnteriorHaberSoles = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? 0 : AcumuladoAnterior.Where(y => y.v_NroCuenta == item).Sum(x => x.d_ImporteSolesH.Value);
                                obj.acumuladoAnteriorHaberDolares = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? 0 : AcumuladoAnterior.Where(y => y.v_NroCuenta == item).Sum(x => x.d_ImporteDolaresH.Value);
                                queryFinal.Add(obj);
                            }

                        }
                        objOperationResult.Success = 1;
                        return queryFinal.ToList();

                        #endregion
                    }
                    else
                    {
                        //las cuentas deben ir con la naturaleza contraria a como se encuentran en la tesoria o el diario 
                        #region Formato2
                        var queryTesoreria = (from a in dbContext.tesoreria

                                              join b in dbContext.tesoreriadetalle on new { Tesoreria = a.v_IdTesoreria, eliminado = 0 }
                                                                                   equals new { Tesoreria = b.v_IdTesoreria, eliminado = b.i_Eliminado.Value } into b_join

                                              from b in b_join.DefaultIfEmpty()

                                              join c in dbContext.documento on new { tipodoc = a.i_IdTipoDocumento.Value, eliminado = 0 }
                                                                           equals new { tipodoc = c.i_CodigoDocumento, eliminado = c.i_Eliminado.Value } into c_join
                                              from c in c_join.DefaultIfEmpty()

                                              join d in dbContext.documento on new { DocRef = b.i_IdTipoDocumento.Value, eliminado = 0 }
                                                                           equals new { DocRef = d.i_CodigoDocumento, eliminado = d.i_Eliminado.Value } into d_join
                                              from d in d_join.DefaultIfEmpty()

                                              join e in dbContext.asientocontable on new { nroCuenta = b.v_NroCuenta, eliminado = 0, p = periodo }
                                                                                  equals new { nroCuenta = e.v_NroCuenta, eliminado = e.i_Eliminado.Value, p = e.v_Periodo } into e_join

                                              from e in e_join.DefaultIfEmpty()

                                              join f in dbContext.datahierarchy on new { medioPago = a.i_IdMedioPago.Value, eliminado = 0, grupo = 44 }
                                                                                equals new { medioPago = f.i_ItemId, eliminado = f.i_IsDeleted.Value, grupo = f.i_GroupId } into f_join
                                              from f in f_join.DefaultIfEmpty()

                                              join g in dbContext.datahierarchy on new { Grupo = 111, item = e.i_EntFinanciera.Value, eliminado = 0 }
                                                                            equals new { Grupo = g.i_GroupId, item = g.i_ItemId, eliminado = g.i_IsDeleted.Value } into g_join
                                              from g in g_join.DefaultIfEmpty()
                                              where a.i_Eliminado == 0 && a.t_FechaRegistro >= FechaInicio && a.t_FechaRegistro <= FechaFin

                                             && (b.i_EsDestino == null || b.i_EsDestino == "0") && a.i_IdEstado == 1
                                              orderby b.v_NroCuenta
                                              select new ReporteLibroCajayBancos
                                              {

                                                  fecha = a.t_FechaRegistro.Value,
                                                  nroComprobante = c == null ? a.v_Mes.Trim() + "-" + a.v_Correlativo.Trim() : c.v_Siglas + " " + a.v_Mes.Trim() + "-" + a.v_Correlativo.Trim(),
                                                  // nroComprobante = a.v_Mes.Trim() + "-" + a.v_Correlativo.Trim(),
                                                  docReferencia = d == null ? b.v_NroDocumento : d.v_Siglas.Trim() + " " + b.v_NroDocumento.Trim(),
                                                  nombre = a.v_Nombre,
                                                  descripcionOperacion = b.v_Analisis == null ? a.v_Glosa == null ? "" : a.v_Glosa : b.v_Analisis,
                                                  haberSoles = b.v_Naturaleza == "D" && a.i_IdMoneda.Value == (int)Currency.Soles && pIntMoneda == (int)Currency.Soles ? b.d_Importe.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Dolares && a.i_IdMoneda == (int)Currency.Soles ? b.d_Importe.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Soles && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Cambio.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Dolares && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Cambio.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Todas && a.i_IdMoneda == (int)Currency.Soles ? b.d_Importe.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Todas && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Cambio.Value : 0,
                                                  haberDolares = b.v_Naturaleza == "D" && a.i_IdMoneda.Value == (int)Currency.Dolares && pIntMoneda == (int)Currency.Dolares ? b.d_Importe.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Dolares && a.i_IdMoneda == (int)Currency.Soles ? b.d_Cambio.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Soles && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Importe.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Soles && a.i_IdMoneda == (int)Currency.Soles ? b.d_Cambio.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Todas && a.i_IdMoneda == (int)Currency.Soles ? b.d_Cambio.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Todas && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Importe.Value : 0,
                                                  debeSoles = b.v_Naturaleza == "H" && a.i_IdMoneda.Value == (int)Currency.Soles && pIntMoneda == (int)Currency.Soles ? b.d_Importe.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Dolares && a.i_IdMoneda == (int)Currency.Soles ? b.d_Importe.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Soles && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Cambio.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Dolares && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Cambio.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Todas && a.i_IdMoneda == (int)Currency.Soles ? b.d_Importe.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Todas && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Cambio.Value : 0,
                                                  debeDolares = b.v_Naturaleza == "H" && a.i_IdMoneda.Value == (int)Currency.Dolares && pIntMoneda == (int)Currency.Dolares ? b.d_Importe.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Dolares && a.i_IdMoneda == (int)Currency.Soles ? b.d_Cambio.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Soles && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Importe.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Soles && a.i_IdMoneda == (int)Currency.Soles ? b.d_Cambio.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Todas && a.i_IdMoneda == (int)Currency.Soles ? b.d_Cambio.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Todas && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Importe.Value : 0,
                                                  cuentaMayor = "",
                                                  cuenta = e == null ? "*NO EXISTE CUENTA*" : e.v_NombreCuenta.Trim(),
                                                  tipoComprobante = 0,

                                                  numeroCuenta = b.v_NroCuenta.Trim(),
                                                  monedaTransaccion = a.i_IdMoneda.Value == 1 ? "S/." : "US$.",
                                                  medioPago = f.v_Value2,
                                                  entidadFinanciera = g == null ? "" : g.v_Value1,
                                                  IdTesoreria = a.v_IdTesoreria,
                                                  v_IdTesoreriaDetalle = b.v_IdTesoreriaDetalle,
                                                  cuentaGrupo = b.v_NroCuenta,
                                                  OrdenAdicional = "03",

                                              }).ToList();




                        ///Recolecto a todas las tesorerias que solo tienen cuentas 10 (pero diferente de 101) y otras cuentas (42,60) en la tesorerias 
                        ////10411101
                        ///4211101
                        ///4211102
                        var TodasTesorerias = queryTesoreria.GroupBy(o => o.IdTesoreria).ToList();
                        var SoloTesoreria104 = TodasTesorerias.Where(o => o.Any(x => x.numeroCuenta.StartsWith("10") && !x.numeroCuenta.StartsWith("101"))).ToList();
                        var Tesoreriassin104 = SoloTesoreria104.Select(g => g.Where(o => !o.numeroCuenta.StartsWith("10"))).ToList();
                        List<ReporteLibroCajayBancos> ListaTesoreriasSin104 = Tesoreriassin104.SelectMany(o => o).ToList();
                        var LisTFinal = ListaTesoreriasSin104.Select(x =>
                         {

                             var TodosDetalles = queryTesoreria.Where(o => o.IdTesoreria == x.IdTesoreria && o.numeroCuenta.StartsWith("10")).ToList();
                             var Detalles = TodosDetalles.Where(o => !o.numeroCuenta.StartsWith("101")).ToList();
                             var solocuenta10 = Detalles.Where(o => o.IdTesoreria == x.IdTesoreria && o.numeroCuenta.StartsWith("10")).Select(o => o.numeroCuenta).Distinct().ToList();

                             foreach (var detallecuenta10 in solocuenta10)
                             {

                                 objRep = new ReporteLibroCajayBancos();
                                 objRep = (ReporteLibroCajayBancos)x.Clone();
                                 objRep.Grupo = detallecuenta10 != null ? "CUENTA : " + detallecuenta10 + " " + _objContabilidadBL.NombreCuenta(detallecuenta10, ListaAsientoContable) : "";
                                 objRep.cuentaGrupo = detallecuenta10 != null ? detallecuenta10 : "";
                                 LisT1.Add(objRep);
                                 var DetallesoOtros = TodosDetalles.Where(o => o.numeroCuenta != detallecuenta10).ToList();
                                 foreach (var detallitosOtros in DetallesoOtros)
                                 {
                                     if (!LisT1.Select(p => p.v_IdTesoreriaDetalle + p.cuentaGrupo).Contains(detallitosOtros.v_IdTesoreriaDetalle + detallecuenta10))
                                     {
                                         objRep = (ReporteLibroCajayBancos)detallitosOtros.Clone();
                                         objRep.Grupo = detallecuenta10 != null ? "CUENTA : " + detallecuenta10 + " " + _objContabilidadBL.NombreCuenta(detallecuenta10, ListaAsientoContable) : "";
                                         objRep.cuentaGrupo = detallecuenta10 != null ? detallecuenta10 : "";
                                         LisT1.Add(objRep);
                                     }
                                 }
                             }
                             return LisT1;
                         }).ToList();

                        ///Recolecto a todas las tesorerias que solo tienen cuentas 10 (diferntes de 101) en la tesorerias  y no son tomadas en cuenta en la consulta anterior
                        /// 10411101 D
                        /// 10411102 H
                        var TesoreriaSolo10 = TodasTesorerias.Where(o => o.All(x => x.numeroCuenta.StartsWith("10"))).ToList();
                        List<ReporteLibroCajayBancos> ListaTesoreriasSolo10 = TesoreriaSolo10.SelectMany(o => o).ToList();
                        ListaTesoreriasSolo10 = ListaTesoreriasSolo10.Where(o => !o.numeroCuenta.StartsWith("101")).ToList();
                        var ListaFinal2 = ListaTesoreriasSolo10.Select(x =>
                        {

                            var TodosDetalles = queryTesoreria.Where(o => o.IdTesoreria == x.IdTesoreria && o.numeroCuenta.StartsWith("10") && o.numeroCuenta != x.numeroCuenta).ToList();
                            foreach (var item in TodosDetalles)
                            {
                                objRep = new ReporteLibroCajayBancos();
                                objRep = (ReporteLibroCajayBancos)item.Clone();
                                objRep.Grupo = x.numeroCuenta != null ? "CUENTA : " + x.numeroCuenta + " " + _objContabilidadBL.NombreCuenta(x.numeroCuenta, ListaAsientoContable) : "";
                                objRep.cuentaGrupo = x.numeroCuenta != null ? x.numeroCuenta : "";
                                if (!LisT2.Select(p => p.v_IdTesoreriaDetalle + p.cuentaGrupo).Contains(item.v_IdTesoreriaDetalle + x.numeroCuenta))
                                {
                                    LisT2.Add(objRep);
                                }
                            }
                            return LisT2;
                        }).ToList();


                        var queryDiario = (from a in dbContext.diario

                                           join b in dbContext.diariodetalle on new { dd = a.v_IdDiario, eliminado = 0 }
                                                                              equals new { dd = b.v_IdDiario, eliminado = b.i_Eliminado.Value } into b_join

                                           from b in b_join.DefaultIfEmpty()

                                           join c in dbContext.documento on new { documento = a.i_IdTipoDocumento.Value, eliminado = 0 }
                                                                          equals new { documento = c.i_CodigoDocumento, eliminado = c.i_Eliminado.Value } into c_join
                                           from c in c_join.DefaultIfEmpty()

                                           join d in dbContext.documento on new { documento = b.i_IdTipoDocumento.Value, eliminado = 0 }
                                                                       equals new { documento = d.i_CodigoDocumento, eliminado = d.i_Eliminado.Value } into d_join
                                           from d in d_join.DefaultIfEmpty()

                                           join e in dbContext.asientocontable on new { nroCuenta = b.v_NroCuenta, eliminado = 0, p = periodo }
                                                                       equals new { nroCuenta = e.v_NroCuenta, eliminado = e.i_Eliminado.Value, p = e.v_Periodo } into e_join

                                           from e in e_join.DefaultIfEmpty()
                                           join g in dbContext.datahierarchy on new { Grupo = 111, item = e.i_EntFinanciera.Value, eliminado = 0 }
                                                                           equals new { Grupo = g.i_GroupId, item = g.i_ItemId, eliminado = g.i_IsDeleted.Value } into g_join
                                           from g in g_join.DefaultIfEmpty()

                                           where a.i_Eliminado == 0 && a.t_Fecha >= FechaInicio && a.t_Fecha <= FechaFin

                                           && (b.i_EsDestino == null || b.i_EsDestino == "0")

                                           orderby b.v_NroCuenta
                                           select new ReporteLibroCajayBancos
                                           {

                                               fecha = a.t_Fecha.Value,
                                               nroComprobante = c == null ? a.v_Mes + "-" + a.v_Correlativo.Trim() : c.v_Siglas + " " + a.v_Mes + "-" + a.v_Correlativo.Trim(),
                                               //nroComprobante = a.v_Mes + "-" + a.v_Correlativo.Trim(),
                                               docReferencia = d == null ? b.v_NroDocumento : d.v_Siglas.Trim() + " " + b.v_NroDocumento.Trim(),
                                               nombre = a.v_Nombre,
                                               descripcionOperacion = b.v_Analisis == null ? a.v_Glosa == null ? "" : a.v_Glosa : b.v_Analisis,

                                               haberSoles = b.v_Naturaleza == "D" && a.i_IdMoneda.Value == (int)Currency.Soles && pIntMoneda == (int)Currency.Soles ? b.d_Importe.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Dolares && a.i_IdMoneda == (int)Currency.Soles ? b.d_Importe.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Soles && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Cambio.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Dolares && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Cambio.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Todas && a.i_IdMoneda == (int)Currency.Soles ? b.d_Importe.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Todas && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Cambio.Value : 0,
                                               haberDolares = b.v_Naturaleza == "D" && a.i_IdMoneda.Value == (int)Currency.Dolares && pIntMoneda == (int)Currency.Dolares ? b.d_Importe.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Dolares && a.i_IdMoneda == (int)Currency.Soles ? b.d_Cambio.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Soles && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Importe.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Soles && a.i_IdMoneda == (int)Currency.Soles ? b.d_Cambio.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Todas && a.i_IdMoneda == (int)Currency.Soles ? b.d_Cambio.Value : b.v_Naturaleza == "D" && pIntMoneda == (int)Currency.Todas && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Importe.Value : 0,
                                               debeSoles = b.v_Naturaleza == "H" && a.i_IdMoneda.Value == (int)Currency.Soles && pIntMoneda == (int)Currency.Soles ? b.d_Importe.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Dolares && a.i_IdMoneda == (int)Currency.Soles ? b.d_Importe.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Soles && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Cambio.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Dolares && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Cambio.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Todas && a.i_IdMoneda == (int)Currency.Soles ? b.d_Importe.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Todas && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Cambio.Value : 0,
                                               debeDolares = b.v_Naturaleza == "H" && a.i_IdMoneda.Value == (int)Currency.Dolares && pIntMoneda == (int)Currency.Dolares ? b.d_Importe.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Dolares && a.i_IdMoneda == (int)Currency.Soles ? b.d_Cambio.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Soles && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Importe.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Soles && a.i_IdMoneda == (int)Currency.Soles ? b.d_Cambio.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Todas && a.i_IdMoneda == (int)Currency.Soles ? b.d_Cambio.Value : b.v_Naturaleza == "H" && pIntMoneda == (int)Currency.Todas && a.i_IdMoneda == (int)Currency.Dolares ? b.d_Importe.Value : 0,
                                               cuentaMayor = "",
                                               cuenta = e == null ? "*NO EXISTE CUENTA*" : e.v_NombreCuenta.Trim(),
                                               tipoComprobante = a.i_IdTipoComprobante.Value,
                                               numeroCuenta = b.v_NroCuenta.Trim(),
                                               monedaTransaccion = a.i_IdMoneda == 1 ? "S/." : "US$.",
                                               medioPago = "",

                                               entidadFinanciera = g == null ? "" : g.v_Value1,
                                               IdTesoreria = a.v_IdDiario,
                                               Apertura = a.i_IdTipoComprobante ?? -1,
                                               v_IdTesoreriaDetalle = b.v_IdDiarioDetalle,
                                               cuentaGrupo = b.v_NroCuenta,
                                               OrdenAdicional = a.i_IdTipoComprobante == 1 ? "01" : a.i_IdTipoComprobante == 6 ? "05" : "02",


                                           }).ToList();





                        //Diarios Apertura
                        var SoloApertura = queryDiario.Where(o => o.Apertura == 1 && o.numeroCuenta.StartsWith("10") && !o.numeroCuenta.StartsWith("101")).Select(o =>
                        {
                            var ap = o;
                            ap.Grupo = "CUENTA : " + o.numeroCuenta + " " + _objContabilidadBL.NombreCuenta(o.numeroCuenta, ListaAsientoContable);
                            ap.cuentaGrupo = o.numeroCuenta;
                            return ap;

                        }).ToList();


                        //Diarios que contienen cuentas que empiezan con 10 pero difente 101 y otras cuentas asociadas
                        var TodosDiarios = queryDiario.Where(o => o.Apertura != 1).GroupBy(o => o.IdTesoreria).ToList();
                        var SoloDiario104 = TodosDiarios.Where(o => o.Any(x => x.numeroCuenta.StartsWith("10") && !x.numeroCuenta.StartsWith("101"))).ToList();
                        var Diariossin104 = SoloDiario104.Select(g => g.Where(o => !o.numeroCuenta.StartsWith("10"))).ToList();

                        List<ReporteLibroCajayBancos> ListaDiariosSin104 = Diariossin104.SelectMany(o => o).ToList();
                        var ListaDiarios1 = ListaDiariosSin104.Select(x =>
                         {
                             var TodosDetalles = queryDiario.Where(o => o.IdTesoreria == x.IdTesoreria && o.numeroCuenta.StartsWith("10") && o.numeroCuenta != x.numeroCuenta).ToList();
                             var Detalles = TodosDetalles.Where(o => !o.numeroCuenta.StartsWith("101")).ToList();
                             var solocuenta10 = Detalles.Where(o => o.IdTesoreria == x.IdTesoreria && o.numeroCuenta.StartsWith("10")).Select(o => o.numeroCuenta).Distinct().ToList();
                             foreach (var detallecuenta10 in solocuenta10)
                             {

                                 objRep = new ReporteLibroCajayBancos();
                                 objRep = (ReporteLibroCajayBancos)x.Clone();
                                 objRep.Grupo = detallecuenta10 != null ? "CUENTA : " + detallecuenta10 + " " + _objContabilidadBL.NombreCuenta(detallecuenta10, ListaAsientoContable) : "";
                                 objRep.cuentaGrupo = detallecuenta10 != null ? detallecuenta10 : "";
                                 LisD1.Add(objRep);
                                 var DetallesoOtros = TodosDetalles.Where(o => o.numeroCuenta != detallecuenta10).ToList();
                                 foreach (var detallitosOtros in DetallesoOtros)
                                 {

                                     if (!LisD1.Select(p => p.v_IdTesoreriaDetalle + p.cuentaGrupo).Contains(detallitosOtros.v_IdTesoreriaDetalle + detallecuenta10))
                                     {
                                         objRep = (ReporteLibroCajayBancos)detallitosOtros.Clone();
                                         objRep.Grupo = detallecuenta10 != null ? "CUENTA : " + detallecuenta10 + " " + _objContabilidadBL.NombreCuenta(detallecuenta10, ListaAsientoContable) : "";
                                         objRep.cuentaGrupo = detallecuenta10 != null ? detallecuenta10 : "";
                                         LisD1.Add(objRep);
                                     }
                                 }
                             }
                             return LisD1;


                         }).ToList();


                        //Diarios que solo contienen cuentas que empiezan con 10  pero diferente de 101 y ninguna otra cuenta asociada diferente y no son consideradas en la consulta anterior
                        var DiariosSolo10 = TodosDiarios.Where(o => o.All(x => x.numeroCuenta.StartsWith("10"))).ToList();
                        List<ReporteLibroCajayBancos> ListaDiariosSolo10 = DiariosSolo10.SelectMany(o => o).ToList();
                        ListaDiariosSolo10 = ListaDiariosSolo10.Where(o => !o.numeroCuenta.StartsWith("101")).ToList();
                        var ListaDiarios2 = ListaDiariosSolo10.Select(x =>
                        {

                            var TodosDetalles = queryDiario.Where(o => o.IdTesoreria == x.IdTesoreria && o.numeroCuenta.StartsWith("10") && o.numeroCuenta != x.numeroCuenta).ToList();
                            foreach (var item in TodosDetalles)
                            {
                                objRep = new ReporteLibroCajayBancos();
                                objRep = (ReporteLibroCajayBancos)item.Clone();
                                objRep.Grupo = x.numeroCuenta != null ? "CUENTA : " + x.numeroCuenta + " " + _objContabilidadBL.NombreCuenta(x.numeroCuenta, ListaAsientoContable) : "";
                                objRep.cuentaGrupo = x.numeroCuenta != null ? x.numeroCuenta : "";
                                if (!LisD2.Select(p => p.v_IdTesoreriaDetalle + p.cuentaGrupo).Contains(item.v_IdTesoreriaDetalle + x.numeroCuenta))
                                {
                                    LisD2.Add(objRep);
                                }
                            }

                            return LisD2;
                        }).ToList();

                        var QueryUnion = LisT1.Concat(LisT2).Concat(SoloApertura).Concat(LisD1).Concat(LisD2).ToList().AsQueryable();

                        if (!string.IsNullOrEmpty(pstrOrdenar))
                        {
                            QueryUnion = QueryUnion.OrderBy(pstrOrdenar);
                        }
                        ListaFiltradoFormas = QueryUnion.ToList();
                        string max = ListaFiltradoFormas.Count != 0 ? ListaFiltradoFormas.Max(p => int.Parse(p.numeroCuenta)).ToString() : string.Empty;
                        queryFinal = (from A in ListaFiltradoFormas
                                      select new ReporteLibroCajayBancos
                                      {
                                          fecha = A.fecha,
                                          nroComprobante = A.nroComprobante,
                                          docReferencia = A.docReferencia,
                                          nombre = A.nombre,
                                          descripcionOperacion = A.descripcionOperacion,
                                          debeSoles = A.Apertura == 1 ? A.haberSoles : A.debeSoles,
                                          debeDolares = A.Apertura == 1 ? A.haberDolares : A.debeDolares,
                                          haberSoles = A.Apertura == 1 ? A.debeSoles : A.haberSoles,
                                          haberDolares = A.Apertura == 1 ? A.debeDolares : A.haberDolares,
                                          cuentaMayor = A.numeroCuenta.Substring(0, 2) + "      CUENTA  MAYOR: " + NombreCuenta(A.numeroCuenta.Substring(0, 2), ListaAsientoContable),
                                          cuenta = A.numeroCuenta + "    " + NombreCuenta(A.numeroCuenta, ListaAsientoContable),
                                          acumuladoAnteriorDebeSoles = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? 0 : AcumuladoAnterior.Where(y => y.v_NroCuenta == A.cuentaGrupo).Sum(x => x.d_ImporteSolesD.Value),
                                          acumuladoAnteriorDebeDolares = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? 0 : AcumuladoAnterior.Where(y => y.v_NroCuenta == A.cuentaGrupo).Sum(x => x.d_ImporteDolaresD.Value),
                                          acumuladoAnteriorHaberSoles = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? 0 : AcumuladoAnterior.Where(y => y.v_NroCuenta == A.cuentaGrupo).Sum(x => x.d_ImporteSolesH.Value),
                                          acumuladoAnteriorHaberDolares = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? 0 : AcumuladoAnterior.Where(y => y.v_NroCuenta == A.cuentaGrupo).Sum(x => x.d_ImporteDolaresH.Value),
                                          monedaTransaccion = A.monedaTransaccion,
                                          medioPago = A.medioPago,
                                          MaxNumeroCuenta = max,
                                          numeroCuenta = A.numeroCuenta,
                                          entidadFinanciera = "ENTIDAD FINANCIERA : " + A.entidadFinanciera,
                                          Grupo = A.Grupo,
                                          FechaS = A.fecha.Date.Day.ToString("00") + "/" + A.fecha.Date.Month.ToString("00") + "/" + A.fecha.Date.Year.ToString(),
                                          cuentaGrupo = A.cuentaGrupo,
                                      }).ToList();



                        if (Formato == 1)
                        {

                            var CuentasFaltantesFormato1 = AcumuladoAnterior.Where(o => o.v_NroCuenta.StartsWith("101"));
                            var q10 = queryFinal.Where(o => o.cuentaGrupo.StartsWith("10")).ToList();
                            var CuentasFaltantes = CuentasFaltantesFormato1.Select(o => o.v_NroCuenta).Except(q10.Select(o => o.cuentaGrupo)).ToList();
                            foreach (var item in CuentasFaltantes)
                            {
                                ReporteLibroCajayBancos obj = new ReporteLibroCajayBancos();
                                obj.Grupo = "CUENTA : " + item + " " + _objContabilidadBL.NombreCuenta(item, ListaAsientoContable);
                                obj.descripcionOperacion = "** SIN OPERACIONES **";
                                obj.acumuladoAnteriorDebeSoles = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? 0 : AcumuladoAnterior.Where(y => y.v_NroCuenta == item).Sum(x => x.d_ImporteSolesD.Value);
                                obj.acumuladoAnteriorDebeDolares = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? 0 : AcumuladoAnterior.Where(y => y.v_NroCuenta == item).Sum(x => x.d_ImporteDolaresD.Value);
                                obj.acumuladoAnteriorHaberSoles = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? 0 : AcumuladoAnterior.Where(y => y.v_NroCuenta == item).Sum(x => x.d_ImporteSolesH.Value);
                                obj.acumuladoAnteriorHaberDolares = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? 0 : AcumuladoAnterior.Where(y => y.v_NroCuenta == item).Sum(x => x.d_ImporteDolaresH.Value);
                                if (obj.acumuladoAnteriorDebeSoles - obj.acumuladoAnteriorHaberSoles != 0)
                                {
                                    queryFinal.Add(obj);
                                }
                            }
                        }
                        else
                        {

                            var CuentasFaltantesFormato2 = AcumuladoAnterior.Where(o => !o.v_NroCuenta.StartsWith("101"));
                            var q10 = queryFinal.Where(o => o.cuentaGrupo.StartsWith("10")).ToList();
                            var CuentasFaltantes = CuentasFaltantesFormato2.Select(o => o.v_NroCuenta).Except(q10.Select(o => o.cuentaGrupo)).ToList();
                            foreach (var item in CuentasFaltantes)
                            {
                                ReporteLibroCajayBancos obj = new ReporteLibroCajayBancos();
                                obj.Grupo = "CUENTA : " + item + " " + _objContabilidadBL.NombreCuenta(item, ListaAsientoContable);
                                obj.descripcionOperacion = "** SIN OPERACIONES **";
                                obj.acumuladoAnteriorDebeSoles = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? 0 : AcumuladoAnterior.Where(y => y.v_NroCuenta == item).Sum(x => x.d_ImporteSolesD.Value);
                                obj.acumuladoAnteriorDebeDolares = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? 0 : AcumuladoAnterior.Where(y => y.v_NroCuenta == item).Sum(x => x.d_ImporteDolaresD.Value);
                                obj.acumuladoAnteriorHaberSoles = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? 0 : AcumuladoAnterior.Where(y => y.v_NroCuenta == item).Sum(x => x.d_ImporteSolesH.Value);
                                obj.acumuladoAnteriorHaberDolares = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? 0 : AcumuladoAnterior.Where(y => y.v_NroCuenta == item).Sum(x => x.d_ImporteDolaresH.Value);
                                if (obj.acumuladoAnteriorDebeSoles - obj.acumuladoAnteriorHaberSoles != 0)
                                {
                                    queryFinal.Add(obj);
                                }
                            }
                        }
                        objOperationResult.Success = 1;
                        return queryFinal.ToList();

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "ContabilidadBL.ReporteLibroCajayBancos()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }

        }




        public List<ReporteLibroDiario> ReporteLibroDiario(ref  OperationResult objOperationResult, DateTime FechaInicio, DateTime FechaFin, int Filtro, int FormatoEstructura = 1, List<int> ListaDocumentoResumir = null, string Orden = null)
        {

            try
            {
                objOperationResult.Success = 1;
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    List<ReporteLibroDiario> ReporteLibroDiario = new List<ReporteLibroDiario>();
                    string PrimerElemento = string.Empty;
                    var ValoresTipodocTrabajadores = dbContext.datahierarchy.Where(c => c.i_GroupId == 132 && c.i_IsDeleted == 0).ToList();
                    List<ReporteLibroDiario> queryTesoreria = (from a in dbContext.tesoreriadetalle

                                                               join b in dbContext.tesoreria on new { Tesoreria = a.v_IdTesoreria, eliminado = 0 }
                                                                                                     equals new { Tesoreria = b.v_IdTesoreria, eliminado = b.i_Eliminado.Value } into b_join

                                                               from b in b_join.DefaultIfEmpty()

                                                               join c in dbContext.documento on new { tipodoc = b.i_IdTipoDocumento.Value, eliminado = 0 }
                                                                                            equals new { tipodoc = c.i_CodigoDocumento, eliminado = c.i_Eliminado.Value } into c_join
                                                               from c in c_join.DefaultIfEmpty()

                                                               join d in dbContext.documento on new { DocRef = a.i_IdTipoDocumento.Value, eliminado = 0 }
                                                                     equals new { DocRef = d.i_CodigoDocumento, eliminado = d.i_Eliminado.Value } into d_join
                                                               from d in d_join.DefaultIfEmpty()


                                                               join e in dbContext.asientocontable on new { nroCuenta = a.v_NroCuenta, eliminado = 0, p = periodo }
                                                                                                   equals new { nroCuenta = e.v_NroCuenta, eliminado = e.i_Eliminado.Value, p = e.v_Periodo } into e_join

                                                               from e in e_join.DefaultIfEmpty()

                                                               join f in dbContext.datahierarchy on new { Grupo = 18, eliminado = 0, Item = b.i_IdMoneda.Value } equals new { Grupo = f.i_GroupId, eliminado = f.i_IsDeleted.Value, Item = f.i_ItemId } into f_join

                                                               from f in f_join.DefaultIfEmpty()

                                                               join g in dbContext.cliente on new { c = a.v_IdCliente } equals new { c = g.v_IdCliente } into g_join
                                                               from g in g_join.DefaultIfEmpty()

                                                               where a.i_Eliminado == 0 && b.t_FechaRegistro >= FechaInicio && b.t_FechaRegistro <= FechaFin

                                                               && e_join.Any(p => p.v_NroCuenta == a.v_NroCuenta)

                                                               && b.i_IdEstado == 1
                                                               select new
                                                               {


                                                                   monedaTransaccion = b.i_IdMoneda == 1 ? "S/" : "D",
                                                                   // codigoUnicoOperacion = c == null ? b.v_Mes.Trim() + "-" + b.v_Correlativo.Trim() : c.v_Siglas.Trim() + " " + b.v_Mes.Trim() + "-" + b.v_Correlativo.Trim(),
                                                                   //codigoUnicoOperacion = c == null ? b.v_Mes.Trim() + "-" + b.v_Correlativo.Trim() : FormatoEstructura == (int)FormatoEstructuraReporteLibroDiario.TodosAsientosResumidos ? c.v_Siglas + " " + "00000000" : c.v_Siglas.Trim() + " " + b.v_Mes.Trim() + "-" + b.v_Correlativo.Trim(),
                                                                   codigoUnicoOperacion = c == null ? b.v_Mes.Trim() + "-" + b.v_Correlativo.Trim() : FormatoEstructura == (int)FormatoEstructuraReporteLibroDiario.Completa ? c.v_Siglas.Trim() + " " + b.v_Mes.Trim() + "-" + b.v_Correlativo.Trim() : ListaDocumentoResumir.Contains(-1) ? c.v_Siglas + " " + "00000000" : c.v_Siglas.Trim() + " " + b.v_Mes.Trim() + "-" + b.v_Correlativo.Trim(),
                                                                   fecha = b.t_FechaRegistro.Value,
                                                                   // descripcionOperacion = FormatoEstructura == (int)FormatoEstructuraReporteLibroDiario.TodosAsientosResumidos ? c.v_Nombre + " " + "DEL MES" : a.v_Analisis == null || a.v_Analisis.Trim() == string.Empty ? b.v_Glosa : a.v_Analisis,
                                                                   descripcionOperacion = FormatoEstructura == (int)FormatoEstructuraReporteLibroDiario.Resumido ? ListaDocumentoResumir.Contains(-1) ? c.v_Nombre + " " + "DEL MES" : a.v_Analisis == null || a.v_Analisis.Trim() == string.Empty ? b.v_Glosa : a.v_Analisis : a.v_Analisis == null || a.v_Analisis.Trim() == string.Empty ? b.v_Glosa : a.v_Analisis,
                                                                   cuenta = a.v_NroCuenta,
                                                                   denominacion = e == null ? "*Cuenta no Existe*" : e.v_NombreCuenta,
                                                                   debeSoles = a.v_Naturaleza == "D" && b.i_IdMoneda.Value == 1 ? a.d_Importe.Value : a.v_Naturaleza == "D" && b.i_IdMoneda.Value == 2 ? a.d_Cambio.Value : 0,
                                                                   haberSoles = a.v_Naturaleza == "H" && b.i_IdMoneda.Value == 1 ? a.d_Importe.Value : a.v_Naturaleza == "H" && b.i_IdMoneda.Value == 2 ? a.d_Cambio.Value : 0,
                                                                   tipoComprobante = 0,
                                                                   i_CodigoDocumento = c.i_CodigoDocumento,
                                                                   NroRegistro = b.v_Mes.Trim() + b.v_Correlativo.Trim(),
                                                                   SiglasMoneda = b.i_IdMoneda != -1 ? f.v_Field.Trim() : "",
                                                                   TipoDocumentoEmisor = b.i_IdEstado == 1 ? g == null ? 0 : g.v_IdCliente == "N002-CL000000000" ? 1 : g.i_IdTipoIdentificacion.Value : 0,
                                                                   NroDocumentoEmisor = b.i_IdEstado == 1 ? g == null ? "0" : g.v_IdCliente == "N002-CL000000000" ? "00000000" : g.v_NroDocIdentificacion : "0",
                                                                   TipoComprobanteDetalle = d == null ? -1 : d.i_CodigoDocumento,
                                                                   SerieCorrelativoDetalle = a.v_NroDocumento,
                                                                   Glosa = string.IsNullOrEmpty(a.v_Analisis) ? string.IsNullOrEmpty(b.v_Glosa) ? "" : b.v_Glosa : a.v_Analisis.Trim(),   // b.v_Glosa == null ? null : b.v_Glosa.Trim(),
                                                                   SiglasDoc = c.v_Siglas,
                                                                   TipoCliente = g.v_FlagPantalla,
                                                                   IdDiario = b.v_IdTesoreria,
                                                                   Pivot = a.v_NroCuenta.Substring(0, 2),
                                                                   IdDiarioDetalle = a.v_IdTesoreriaDetalle,
                                                                   Naturaleza = a.v_Naturaleza,
                                                                   TipoDocumento = b.i_IdTipoDocumento ?? -1,


                                                               }).ToList().Select(x =>
                                                                   {
                                                                       var TipoCliente = x.TipoCliente == "T" ? ValoresTipodocTrabajadores.Where(y => y.i_ItemId == x.TipoDocumentoEmisor && y.i_IsDeleted == 0).FirstOrDefault() != null ? int.Parse(ValoresTipodocTrabajadores.Where(y => y.i_ItemId == x.TipoDocumentoEmisor && y.i_IsDeleted == 0).Select(a => a.v_Value2).FirstOrDefault()) : x.TipoDocumentoEmisor : x.TipoDocumentoEmisor;
                                                                       return new ReporteLibroDiario
                                              {

                                                  monedaTransaccion = x.monedaTransaccion,
                                                  codigoUnicoOperacion = "2" + x.codigoUnicoOperacion,
                                                  codigoUnico = x.codigoUnicoOperacion,
                                                  fecha = x.fecha,
                                                  descripcionOperacion = x.descripcionOperacion,
                                                  cuenta = x.cuenta,
                                                  denominacion = x.denominacion,
                                                  debeSoles = x.debeSoles,
                                                  haberSoles = x.haberSoles,
                                                  tipoComprobante = x.tipoComprobante,
                                                  NroRegistro = x.i_CodigoDocumento.ToString("000") + x.NroRegistro,
                                                  SiglasMoneda = x.SiglasMoneda,
                                                  TipoDocEmisor = x.TipoCliente == "T" ? TipoCliente == null ? 0 : TipoCliente : x.TipoDocumentoEmisor,
                                                  NroDocEmisor = x.NroDocumentoEmisor,
                                                  TipoComprobanteDetalle = x.TipoComprobanteDetalle,
                                                  SerieCorrelativoDetalle = x.SerieCorrelativoDetalle,
                                                  Glosa = x.Glosa,
                                                  NroRegistroSiglas = x.SiglasDoc + " " + x.NroRegistro,
                                                  IdDiaro = x.IdDiario,
                                                  Pivot = x.Pivot,
                                                  IdDiarioDetalle = x.IdDiarioDetalle,
                                                  Naturaleza = x.Naturaleza,
                                                  TipoDocumento = x.TipoDocumento,
                                                  TipoDocumentoNroRegistro = double.Parse("2" + x.i_CodigoDocumento.ToString("000") + x.NroRegistro)

                                              };

                                                                   }).ToList();



                    List<ReporteLibroDiario> queryDiario = (from a in dbContext.diariodetalle

                                                            join b in dbContext.diario on new { dd = a.v_IdDiario, eliminado = 0 }
                                                                                               equals new { dd = b.v_IdDiario, eliminado = b.i_Eliminado.Value } into b_join

                                                            from b in b_join.DefaultIfEmpty()

                                                            join c in dbContext.documento on new { documento = b.i_IdTipoDocumento.Value, eliminado = 0 }
                                                                                           equals new { documento = c.i_CodigoDocumento, eliminado = c.i_Eliminado.Value } into c_join
                                                            from c in c_join.DefaultIfEmpty()

                                                            join d in dbContext.documento on new { documento = a.i_IdTipoDocumento.Value, eliminado = 0 }
                                                                                        equals new { documento = d.i_CodigoDocumento, eliminado = d.i_Eliminado.Value } into d_join
                                                            from d in d_join.DefaultIfEmpty()

                                                            join e in dbContext.asientocontable on new { nroCuenta = a.v_NroCuenta, eliminado = 0, p = periodo }
                                                                                        equals new { nroCuenta = e.v_NroCuenta, eliminado = e.i_Eliminado.Value, p = e.v_Periodo } into e_join

                                                            from e in e_join.DefaultIfEmpty()

                                                            join f in dbContext.datahierarchy on new { Grupo = 18, eliminado = 0, Item = b.i_IdMoneda.Value } equals new { Grupo = f.i_GroupId, eliminado = f.i_IsDeleted.Value, Item = f.i_ItemId } into f_join

                                                            from f in f_join.DefaultIfEmpty()

                                                            join g in dbContext.cliente on new { c = a.v_IdCliente } equals new { c = g.v_IdCliente } into g_join
                                                            from g in g_join.DefaultIfEmpty()


                                                            where a.i_Eliminado == 0 && b.t_Fecha >= FechaInicio && b.t_Fecha <= FechaFin

                                                             && e_join.Any(p => p.v_NroCuenta == a.v_NroCuenta)

                                                            select new
                                                            {

                                                                monedaTransaccion = b.i_IdMoneda == 1 ? "S/" : "D",
                                                                codigoUnicoOperacion = c == null ? b.v_Mes.Trim() + "-" + b.v_Correlativo.Trim() : FormatoEstructura == (int)FormatoEstructuraReporteLibroDiario.Completa ? c.v_Siglas.Trim() + " " + b.v_Mes.Trim() + "-" + b.v_Correlativo.Trim() : ListaDocumentoResumir.Contains(b.i_IdTipoDocumento ?? -1) ? c.v_Siglas + " " + "00000000" : c.v_Siglas.Trim() + " " + b.v_Mes.Trim() + "-" + b.v_Correlativo.Trim(),
                                                                // codigoUnicoOperacion = c == null ? b.v_Mes.Trim() + "-" + b.v_Correlativo.Trim() : FormatoEstructura == (int)FormatoEstructuraReporteLibroDiario.AsientosVentaCompraResumidos ? b.i_IdTipoDocumento == (int)LibroDiarios.Ventas || b.i_IdTipoDocumento == (int)LibroDiarios.Compra ? c.v_Siglas + " " + "00000000" : c.v_Siglas.Trim() + " " + b.v_Mes.Trim() + "-" + b.v_Correlativo.Trim() : FormatoEstructura == (int)FormatoEstructuraReporteLibroDiario.TodosAsientosResumidos ? c.v_Siglas + " " + "00000000" : c.v_Siglas.Trim() + " " + b.v_Mes.Trim() + "-" + b.v_Correlativo.Trim(),
                                                                fecha = b.t_Fecha.Value,
                                                                descripcionOperacion = FormatoEstructura == (int)FormatoEstructuraReporteLibroDiario.Resumido ? ListaDocumentoResumir.Contains(b.i_IdTipoDocumento ?? -1) ? c.v_Nombre + " " + "DEL MES" : a.v_Analisis == null || a.v_Analisis.Trim() == string.Empty ? b.v_Glosa : a.v_Analisis : a.v_Analisis == null || a.v_Analisis.Trim() == string.Empty ? b.v_Glosa : a.v_Analisis,
                                                                cuenta = a.v_NroCuenta,
                                                                denominacion = e == null ? "*Cuenta no Existe*" : e.v_NombreCuenta,
                                                                debeSoles = a.v_Naturaleza == "D" && b.i_IdMoneda.Value == 1 ? a.d_Importe.Value : a.v_Naturaleza == "D" && b.i_IdMoneda.Value == 2 ? a.d_Cambio.Value : 0,
                                                                haberSoles = a.v_Naturaleza == "H" && b.i_IdMoneda.Value == 1 ? a.d_Importe.Value : a.v_Naturaleza == "H" && b.i_IdMoneda.Value == 2 ? a.d_Cambio.Value : 0,
                                                                tipoComprobante = b.i_IdTipoComprobante.Value,
                                                                i_CodigoDocumento = c.i_CodigoDocumento,
                                                                NroRegistro = b.v_Mes.Trim() + b.v_Correlativo.Trim(),
                                                                SiglasMoneda = b.i_IdMoneda != -1 ? f.v_Field.Trim() : "",
                                                                TipoDocumentoEmisor = g == null ? 0 : g.v_IdCliente == "N002-CL000000000" ? 1 : g.i_IdTipoIdentificacion.Value,
                                                                NroDocumentoEmisor = g == null ? "0" : g.v_IdCliente == "N002-CL000000000" ? "00000000" : g.v_NroDocIdentificacion,
                                                                TipoComprobanteDetalle = d == null ? -1 : d.i_CodigoDocumento,
                                                                SerieCorrelativoDetalle = a.v_NroDocumento,
                                                                Glosa = string.IsNullOrEmpty(a.v_Analisis) ? string.IsNullOrEmpty(b.v_Glosa) ? "" : b.v_Glosa : a.v_Analisis.Trim(),
                                                                SiglasDoc = c.v_Siglas,
                                                                TipoCliente = g.v_FlagPantalla,
                                                                EsApertura = b.i_IdTipoComprobante == 1 ? true : false,
                                                                IdDiario = b.v_IdDiario,
                                                                Pivot = a.v_NroCuenta.Substring(0, 2),
                                                                IdDiarioDetalle = a.v_IdDiarioDetalle,
                                                                Naturaleza = a.v_Naturaleza,
                                                                TipoDocumento = b.i_IdTipoDocumento ?? -1,

                                                            }).ToList().Select(x =>
                                                                {

                                                                    var TipoCliente = x.TipoCliente == "T" ? ValoresTipodocTrabajadores.Where(y => y.i_ItemId == x.TipoDocumentoEmisor && y.i_IsDeleted == 0).FirstOrDefault() != null ? int.Parse(ValoresTipodocTrabajadores.Where(y => y.i_ItemId == x.TipoDocumentoEmisor && y.i_IsDeleted == 0).Select(a => a.v_Value2).FirstOrDefault()) : x.TipoDocumentoEmisor : x.TipoDocumentoEmisor;
                                                                    return new ReporteLibroDiario


                                                                  {

                                                                      monedaTransaccion = x.monedaTransaccion,
                                                                      codigoUnicoOperacion = x.EsApertura ? "1" + x.codigoUnicoOperacion : "2" + x.codigoUnicoOperacion,
                                                                      codigoUnico = x.codigoUnicoOperacion,
                                                                      fecha = x.fecha,
                                                                      descripcionOperacion = x.descripcionOperacion,
                                                                      cuenta = x.cuenta,
                                                                      denominacion = x.denominacion,
                                                                      debeSoles = x.debeSoles,
                                                                      haberSoles = x.haberSoles,
                                                                      tipoComprobante = x.tipoComprobante,
                                                                      NroRegistro = x.i_CodigoDocumento.ToString("000") + x.NroRegistro,
                                                                      SiglasMoneda = x.SiglasMoneda,
                                                                      TipoDocEmisor = x.TipoCliente == "T" ? TipoCliente == null ? 0 : TipoCliente : x.TipoDocumentoEmisor,
                                                                      NroDocEmisor = x.NroDocumentoEmisor,
                                                                      TipoComprobanteDetalle = x.TipoComprobanteDetalle,
                                                                      SerieCorrelativoDetalle = x.SerieCorrelativoDetalle,
                                                                      Glosa = x.Glosa,
                                                                      NroRegistroSiglas = x.SiglasDoc + " " + x.NroRegistro,
                                                                      IdDiaro = x.IdDiario,
                                                                      Pivot = x.Pivot,
                                                                      IdDiarioDetalle = x.IdDiarioDetalle,
                                                                      Naturaleza = x.Naturaleza,
                                                                      TipoDocumento = x.TipoDocumento,
                                                                      TipoDocumentoNroRegistro = x.EsApertura ? double.Parse("1" + x.i_CodigoDocumento.ToString("000") + x.NroRegistro) : double.Parse("2" + x.i_CodigoDocumento.ToString("000") + x.NroRegistro),


                                                                  };

                                                                }).ToList();

                    List<ReporteLibroDiario> QueryUnion = queryTesoreria.Concat(queryDiario).ToList();
                    if (Filtro != -1)
                    {
                        ReporteLibroDiario = Filtro == 1 ? QueryUnion.Where(x => x.tipoComprobante != 5).ToList() : Filtro == 2 ? QueryUnion.ToList() : Filtro == 3 ? QueryUnion.Where(x => x.tipoComprobante == 5).ToList() : Filtro == 4 ? QueryUnion.Where(x => x.tipoComprobante == 1).ToList() : null;
                    }
                    else
                    {
                        ReporteLibroDiario = QueryUnion;
                    }


                    if (FormatoEstructura == (int)FormatoEstructuraReporteLibroDiario.Resumido)
                    {
                        if (ListaDocumentoResumir.Contains(-1))
                        {

                            ListaDocumentoResumir = ListaDocumentoResumir.Select(o => o).Where(o => o != -1).ToList();
                            var Tesorerias = queryTesoreria.GroupBy(o => new { o.codigoUnicoOperacion, o.cuenta }).Select(d =>
                            {
                                var k = d.FirstOrDefault();
                                k.debeSoles = d.Sum(h => h.debeSoles);
                                k.haberSoles = d.Sum(h => h.haberSoles);
                                return k;
                            }).ToList();

                            var DiariosContenidasLista = queryDiario.Where(o => ListaDocumentoResumir.Contains(o.TipoDocumento)).ToList();
                            var DiariosNoContenidasLista = queryDiario.Except(DiariosContenidasLista).ToList();

                            var Diarios = DiariosContenidasLista.GroupBy(o => new { o.codigoUnicoOperacion, o.cuenta }).Select(d =>
                         {

                             var k = d.FirstOrDefault();
                             k.debeSoles = d.Sum(h => h.debeSoles);
                             k.haberSoles = d.Sum(h => h.haberSoles);
                             return k;
                         }).ToList();




                            ReporteLibroDiario = Tesorerias.Concat(Diarios).ToList().Concat(DiariosNoContenidasLista).ToList();

                        }
                        else
                        {
                            var ComprasVentasResumidas = ReporteLibroDiario.Where(o => ListaDocumentoResumir.Contains(o.TipoDocumento)).GroupBy(o => new { o.codigoUnicoOperacion, o.cuenta }).Select(d =>
                                 {

                                     var k = d.FirstOrDefault();
                                     k.debeSoles = d.Sum(h => h.debeSoles);
                                     k.haberSoles = d.Sum(h => h.haberSoles);
                                     return k;
                                 }).ToList();
                            var OtrosDocumentos = ReporteLibroDiario.Where(o => !ListaDocumentoResumir.Contains(o.TipoDocumento)).ToList();
                            ReporteLibroDiario = ComprasVentasResumidas.Concat(OtrosDocumentos).ToList();
                        }
                    }

                    if (!string.IsNullOrEmpty(Orden))
                    {
                        ReporteLibroDiario = ReporteLibroDiario != null ? ReporteLibroDiario.AsQueryable().OrderBy(Orden).ToList() : null;
                    }

                    //ReporteLibroDiario = (ReporteLibroDiario != null) ? ReporteLibroDiario.OrderBy(x => x.codigoUnicoOperacion).ToList() : null;
                    int i = 1;
                    List<ReporteLibroDiario> ListaIndexReporteLibroDiario = new List<ReporteLibroDiario>();
                    var contador = 1;
                    ReporteLibroDiario.AsParallel().ToList().ForEach(p =>
                    {

                        p.Index = contador;
                        contador++;
                    });

                    return ReporteLibroDiario;
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "ContabilidadBL.ReporteLibroDiario()";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }
        }






        public List<ReporteLibroDiarioSimplificado> ReporteLibroDiarioSimplificado(ref  OperationResult objOperationResult, DateTime FechaInicio, DateTime FechaFin, int Filtro)
        {
            var LibroDiario = ReporteLibroDiario(ref objOperationResult, FechaInicio, FechaFin, Filtro);
            var DiariosAgrupados = LibroDiario.GroupBy(o => o.IdDiaro).ToList();
            var Cuentas = LibroDiario.Select(o => o.Pivot).Distinct().ToList();
            List<ReporteLibroDiarioSimplificado> ListaSimplificado = new List<ReporteLibroDiarioSimplificado>();

            foreach (var ld in DiariosAgrupados)
            {
                foreach (var agrup in ld)
                {


                    //ReporteLibroDiario obj = new ReporteLibroDiario();
                    //obj = (ReporteLibroDiario)agrup.Clone();
                    // ListaSimplificado.Add(obj);
                    foreach (var cuenta in Cuentas)
                    {
                        ReporteLibroDiarioSimplificado obj = new ReporteLibroDiarioSimplificado();
                        obj.NroRegistro = agrup.NroRegistro; //(ReporteLibroDiarioSimplificado)agrup.Clone();
                        obj.Pivot = agrup.Pivot;
                        //obj.Pivot = item;
                        //obj.debeSoles = agrup.cuenta.Substring(0, 2) == item ? agrup.debeSoles : 0;
                        //obj.haberSoles = agrup.cuenta.Substring(0, 2) == item ? agrup.haberSoles : 0;

                        var Importe = agrup.Pivot == cuenta; //  aptitudeCertificate.Where(o => o.IdDiarioDetalle == grdData.Rows[i].Cells["IdDiarioDetalle"].Value.ToString() && o.Pivot == cuenta).FirstOrDefault();
                        if (Importe)
                        {

                            string Celda = agrup.Naturaleza == "D" ? cuenta + "Debe" : cuenta + "Haber"; //Importe != null ? Importe.Naturaleza == "D" ? cuenta + "Debe" : cuenta + "Haber" : "";

                            if (!string.IsNullOrEmpty(Celda))
                            {
                                string Celda1 = Celda.Contains("Debe") ? cuenta + "Haber" : cuenta + "Debe";
                                obj.Celda1 = Celda1;
                                obj.CeldaValor1 = agrup.Naturaleza == "D" ? agrup.debeSoles.ToString() : "0";


                                string Celda2 = Celda1.Contains("Debe") ? cuenta + "Haber" : cuenta + "Debe";
                                obj.Celda2 = Celda2;
                                obj.CeldaValor2 = "0";
                                //obj.CeldaDebe =agrup.Naturaleza =="D"?
                                //grdData.Rows[i].Cells[Celda].Value = Importe.Naturaleza == "D" ? Importe.debeSoles : 0;
                                //grdData.Rows[i].Cells[Celda1].Value = Importe.Naturaleza == "H" ? Importe.haberSoles : 0;

                            }
                        }
                        else
                        {
                            string CeldaDebe = cuenta + "Debe";
                            string CeldaHaber = cuenta + "Haber";

                            obj.Celda1 = CeldaDebe;
                            obj.CeldaValor1 = "0";
                            obj.Celda2 = CeldaHaber;
                            obj.CeldaValor2 = "0";
                            //grdData.Rows[i].Cells[CeldaDebe].Value = 0;
                            //grdData.Rows[i].Cells[CeldaHaber].Value = 0;
                        }













                        ListaSimplificado.Add(obj);
                    }
                }
            }


            return ListaSimplificado;





            //            var Items = new[] {
            //    new { TypeCode = 1, UserName = "Don Smith"},
            //    new { TypeCode = 1, UserName = "Mike Jones"},
            //    new { TypeCode = 1, UserName = "James Ray"},
            //    new { TypeCode = 2, UserName = "Tom Rizzo"},
            //    new { TypeCode = 2, UserName = "Alex Homes"},
            //    new { TypeCode = 3, UserName = "Andy Bates"}
            //};
            //            var Columns = from i in Items
            //                          group i.UserName by i.TypeCode;
            //            Dictionary<int, List<string>> Rows = new Dictionary<int, List<string>>();
            //            int RowCount = Columns.Max(g => g.Count());
            //            for (int i = 0; i <= RowCount; i++) // Row 0 is the header row.
            //            {
            //                Rows.Add(i, new List<string>());
            //            }
            //            int RowIndex;
            //            foreach (IGrouping<int, string> c in Columns)
            //            {
            //                Rows[0].Add(c.Key.ToString());
            //                RowIndex = 1;
            //                foreach (string user in c)
            //                {
            //                    Rows[RowIndex].Add(user);
            //                    RowIndex++;
            //                }
            //                for (int r = RowIndex; r <= Columns.Count(); r++)
            //                {
            //                    Rows[r].Add(string.Empty);
            //                }
            //            }

            //            List<string> ListaFinal = new List<string>();
            //            foreach (List<string> row in Rows.Values)
            //            {
            //                ListaFinal.Add(row.Aggregate((current, next) => current + " | " + next));
            //               // Console.WriteLine();
            //            }
            // Console.ReadLine();




            // return ListaFinal;





        }

        public List<KardexList> ReporteLibroInventarioValorizado(ref OperationResult objOperationResult, DateTime FechaInicio, DateTime FechaFin, int Establecimiento)
        {
            try
            {
                objOperationResult.Success = 1;

                var Kardex = new AlmacenBL().ReporteKardexValorizadoSunat(ref objOperationResult, DateTime.Parse("01/01/" + Globals.ClientSession.i_Periodo.ToString()), FechaFin, null, (int)Currency.Soles, "", "", "SOLES", Establecimiento, 0, "", "", 0, 0, 0, (int)FormatoCantidad.Unidades, "-1", false, 0, 0, "-1", FechaInicio, false, "", "","");
                return Kardex.OrderBy(l => l.v_NombreProductoInventarioSunat).ToList();
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "ContabilidadBL.ReporteLibroInventarioValorizado()";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;

            }


        }




        public List<BalanceInicial> ReporteBalanceInicial(ref OperationResult objOperationResult, string pstrPeriodo, int pIntMoneda, int TipoComprobante, int TipoReporte)
        {
            try
            {
                objOperationResult.Success = 1;
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    var PlanCuentas = dbContext.asientocontable.Where(o => o.v_Periodo == pstrPeriodo && o.i_Eliminado == 0).ToList();
                    var AsientoApertura = (from a in dbContext.diario

                                           join b in dbContext.diariodetalle on new { IdDiario = a.v_IdDiario, eliminado = 0 }
                                                                            equals new { IdDiario = b.v_IdDiario, eliminado = b.i_Eliminado.Value } into b_join
                                           from b in b_join.DefaultIfEmpty()

                                           join c in dbContext.asientocontable on new { asiento = b.v_NroCuenta, eliminado = 0, p = periodo }
                                                                            equals new { asiento = c.v_NroCuenta, eliminado = c.i_Eliminado.Value, p = c.v_Periodo } into c_join

                                           from c in c_join.DefaultIfEmpty()

                                           join d in dbContext.documento on new { dia = b.i_IdTipoDocumento.Value, eliminado = 0 } equals new { dia = d.i_CodigoDocumento, eliminado = d.i_Eliminado.Value } into d_join
                                           from d in d_join.DefaultIfEmpty()

                                           where a.v_Periodo == pstrPeriodo && a.i_IdTipoComprobante == TipoComprobante && a.i_Eliminado == 0 && (b.v_NroCuenta != null || b.v_NroCuenta != "") //&& a.v_Mes == "01"

                                           orderby b.v_NroCuenta

                                           select new
                                           {

                                               cuenta = b.v_NroCuenta.Trim(),
                                               denominacion = c == null ? "*No Existe*" : c.v_NombreCuenta.Trim(),
                                               v_mes = a.v_Mes,
                                               imputable = c == null ? -1 : c.i_Imputable.Value,
                                               Importe = a.i_IdMoneda == pIntMoneda ? b.d_Importe ?? 0 : b.d_Cambio ?? 0,
                                               Naturaleza = c.i_Naturaleza == 2 ? "H" : "D",
                                               NroDiario = d.v_Siglas + " " + b.v_NroDocumento,
                                               Grupo1 = b.v_NroCuenta.Substring(0, 2),
                                               Fecha = a.t_Fecha.Value,
                                               Analisis = b.v_Analisis == null ? a.v_Glosa : b.v_Analisis,
                                               NaturalezaAsiento = b.v_Naturaleza,

                                           }).ToList().Select(o =>
                                               {
                                                   OperationResult objOperationResult1 = new OperationResult();
                                                   var jerarquia = PlanCuentas.Where(z => z.i_Imputable == 0 && z.v_NroCuenta == o.cuenta.Substring(0, 4)).ToList();
                                                   var jerarmax = jerarquia.Any() && jerarquia != null ? jerarquia.Max(z => z.i_LongitudJerarquica.Value) : 0;
                                                   var newCuenta = o.cuenta.Substring(0, jerarmax);
                                                   var cuentajerar = PlanCuentas.Where(p => p.v_NroCuenta == newCuenta).FirstOrDefault();
                                                   var Cuenta1 = o.cuenta.Length >= 2 ? o.cuenta.Substring(0, 2) : "";
                                                   var NombreCuenta1 = NombreCuentaImputable(Cuenta1, PlanCuentas);

                                                   var NombreCuenta2 = cuentajerar != null && cuentajerar.v_NroCuenta != null ? NombreCuentaImputable(cuentajerar.v_NroCuenta, PlanCuentas) : null;
                                                   var GrupoInicial = GrupoLibroInicial(ref objOperationResult1, o.cuenta);
                                                   var Titulo = GrupoInicial.Contains(" ") ? GrupoInicial.Split(' ') : null;
                                                   return new BalanceInicial
                                                 {
                                                     cuenta = o.cuenta,
                                                     denominacion = o.denominacion,
                                                     v_mes = o.v_mes,
                                                     imputable = o.imputable,
                                                     //Importe = o.Naturaleza == o.NaturalezaAsiento ? o.Importe.Value : o.Importe.Value * -1,
                                                     Importe = GrupoInicial == "II. PASIVO" || GrupoInicial == "III. PATRIMONIO" ? o.NaturalezaAsiento == "H" ? o.Importe < 0 ? o.Importe * -1 : o.Importe : o.Importe > 0 ? o.Importe * -1 : o.Importe
                                                                                                                                    : o.NaturalezaAsiento == "D" ? o.Importe > 0 ? o.Importe : o.Importe * -1 : o.Importe > 0 ? o.Importe * -1 : o.Importe,
                                                     NaturalezaCuenta = o.Naturaleza,
                                                     Grupo1 = Cuenta1,
                                                     DenominacionGrupo1 = NombreCuenta1 != null && NombreCuenta1.FirstOrDefault() != null ? NombreCuenta1.FirstOrDefault().cuenta : "",
                                                     Grupo2 = cuentajerar != null ? cuentajerar.v_NroCuenta : "",
                                                     DenominacionGrupo2 = cuentajerar != null && cuentajerar.v_NroCuenta != null && NombreCuenta2 != null && NombreCuenta2.FirstOrDefault() != null ? NombreCuenta2.FirstOrDefault().cuenta : "",
                                                     NroDiario = o.NroDiario,
                                                     Grupo3 = o.cuenta,
                                                     DenominacionGrupo3 = o.denominacion,
                                                     Fecha = o.Fecha.ToShortDateString(),
                                                     Analisis = o.Analisis,
                                                     GrupoPrincipal = GrupoInicial,
                                                     TotalGrupoPrincipal = Titulo.Count() == 2 ? "TOTAL  " + Titulo[1] + " :" : "",

                                                 };

                                               }).ToList();

                    if (TipoReporte == (int)TipoReporteBalancesInicial.Inventario)
                    {

                        return AsientoApertura;
                    }
                    else
                    {

                        AsientoApertura = AsientoApertura.GroupBy(l => new { l.GrupoPrincipal, l.Grupo1 }).Select(d =>
                        {

                            var k = d.FirstOrDefault();
                            k.Importe = d.Sum(o => o.Importe);
                            return k;
                        }).ToList();

                        return AsientoApertura;
                    }
                }

            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "ContabilidadBL.ReporteBalanceInicial()";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }


        }

        public List<ReporteHojaTrabajo> ReporteHojaTrabajo(ref OperationResult objOperationResult, string pstrPeriodo, string pstrMes, int pIntNumeroDigitos, int pIntMoneda, int pstrCuentasImputables, bool balancesunat = false, bool LibroInicial = false)
        {
            try
            {
                //TODO:Revisar
                AsientosContablesBL _objAsientoContableBL = new AsientosContablesBL();
                ReporteHojaTrabajo objReporte = new ReporteHojaTrabajo();
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                List<ReporteHojaTrabajo> ListaHojaTrabajo = new List<ReporteHojaTrabajo>();

                List<asientocontable> ListaAsientoContable = dbContext.asientocontable.ToList();

                List<ReporteHojaTrabajo> AsientoApertura = new List<ReporteHojaTrabajo>();
                List<saldoscontablesDto> saldoscontables = new List<saldoscontablesDto>();
                if (balancesunat || LibroInicial)
                {
                    #region Recopila Información de AsientoApertura
                    AsientoApertura = (from a in dbContext.diario

                                       join b in dbContext.diariodetalle on new { IdDiario = a.v_IdDiario, eliminado = 0 }
                                                                        equals new { IdDiario = b.v_IdDiario, eliminado = b.i_Eliminado.Value } into b_join
                                       from b in b_join.DefaultIfEmpty()

                                       join c in dbContext.asientocontable on new { asiento = b.v_NroCuenta, eliminado = 0, p = periodo }
                                                                        equals new { asiento = c.v_NroCuenta, eliminado = c.i_Eliminado.Value, p = c.v_Periodo } into c_join

                                       from c in c_join.DefaultIfEmpty()

                                       where a.v_Periodo == pstrPeriodo && a.i_IdTipoComprobante == 1 && a.i_Eliminado == 0 && (b.v_NroCuenta != null || b.v_NroCuenta != "") //&& a.v_Mes == "01"

                                       orderby b.v_NroCuenta
                                       select new ReporteHojaTrabajo
                                       {

                                           cuenta = b.v_NroCuenta.Trim(),
                                           denominacion = c == null ? "*No Existe*" : c.v_NombreCuenta.Trim(),
                                           v_mes = a.v_Mes,
                                           imputable = c == null ? -1 : c.i_Imputable.Value,
                                           d_ImporteSolesD = a.i_IdMoneda == 1 && b.v_Naturaleza == "D" ? b.d_Importe.Value : a.i_IdMoneda == 2 && b.v_Naturaleza == "D" ? b.d_Cambio.Value : 0,
                                           d_ImporteDolaresD = a.i_IdMoneda == 2 && b.v_Naturaleza == "D" ? b.d_Importe.Value : a.i_IdMoneda == 1 && b.v_Naturaleza == "D" ? b.d_Cambio.Value : 0,
                                           d_ImporteSolesH = a.i_IdMoneda == 1 && b.v_Naturaleza == "H" ? b.d_Importe.Value : a.i_IdMoneda == 2 && b.v_Naturaleza == "H" ? b.d_Cambio.Value : 0,
                                           d_ImporteDolaresH = a.i_IdMoneda == 2 && b.v_Naturaleza == "H" ? b.d_Importe.Value : a.i_IdMoneda == 1 && b.v_Naturaleza == "H" ? b.d_Cambio.Value : 0,
                                           Naturaleza = c.i_Naturaleza == 2 ? "H" : "D",
                                       }).ToList();


                    if (!LibroInicial)   //se agrego para libro Inicial
                        saldoscontables = _objAsientoContableBL.ObtenerSaldosContablesPorMes(ref  objOperationResult, int.Parse(pstrMes), int.Parse(pstrPeriodo), 1, false, false, null, false, null, null, true);

                    #endregion
                }
                else
                {
                    saldoscontables = _objAsientoContableBL.ObtenerSaldosContablesPorMes(ref  objOperationResult, int.Parse(pstrMes), int.Parse(pstrPeriodo));
                }

                #region RecopilaSaldosContables-CuentasRespectivas



                var ReporteTotal = (from a in saldoscontables
                                    join b in dbContext.asientocontable on new { cuenta = a.v_NroCuenta, eliminado = 0, p = periodo }
                                                                              equals new { cuenta = b.v_NroCuenta, eliminado = b.i_Eliminado.Value, p = b.v_Periodo } into b_join
                                    from b in b_join.DefaultIfEmpty()

                                    where (a.v_NroCuenta != null && a.v_NroCuenta != "" && a.v_NroCuenta != "*Cta Eliminada*") // &&   a.v_Mes == pstrMes
                                    orderby a.v_NroCuenta
                                    select new ReporteHojaTrabajo
                                    {
                                        cuenta = a.v_NroCuenta == "*Cta Eliminada*" ? "" : a.v_NroCuenta.Trim(),
                                        denominacion = a.v_NroCuenta == "*Cta Eliminada*" ? "*No Existe*" : b == null ? "*No Existe*" : b.v_NombreCuenta.Trim(),
                                        d_ImporteDolaresD = a.d_ImporteDolaresD.Value,
                                        d_ImporteDolaresH = a.d_ImporteDolaresH.Value,
                                        d_ImporteSolesH = a.d_ImporteSolesH.Value,
                                        d_ImporteSolesD = a.d_ImporteSolesD.Value,
                                        v_mes = a.v_Mes,
                                        Naturaleza = a.Naturaleza,

                                    }).ToList().OrderBy(l => l.cuenta).ToList();

                #endregion
                #region RecopilaCuentasMayoresdeSaldosContables-CuentasRespectivas

                List<ReporteHojaTrabajo> ReporteCuentaMayorFinal = new List<ReporteHojaTrabajo>();
                if (balancesunat || LibroInicial) // LibroInicial se agrego para BalanceInicial
                {

                    var saldosasientosapertura = (from a in AsientoApertura

                                                  select new saldoscontablesDto
                                                  {

                                                      v_NroCuenta = a.cuenta,
                                                      v_Mes = a.v_mes,
                                                      Naturaleza = a.Naturaleza,

                                                  }).ToList();


                    saldoscontables = saldoscontables.Concat(saldosasientosapertura).ToList();
                    ReporteCuentaMayorFinal = (from a in saldoscontables
                                               join b in dbContext.asientocontable on new { cuenta = a.v_NroCuenta, eliminado = 0, p = periodo }
                                                                                          equals new { cuenta = b.v_NroCuenta, eliminado = b.i_Eliminado.Value, p = b.v_Periodo } into b_join
                                               from b in b_join.DefaultIfEmpty()

                                               where (a.v_NroCuenta != null && a.v_NroCuenta != "" && a.v_NroCuenta != "*Cta Eliminada*") // && a.v_Mes == pstrMes

                                               select new
                                               {
                                                   cuenta = a.v_NroCuenta.Trim().Substring(0, 2),
                                                   denominacion = b == null ? "" : "N",
                                                   v_mes = a.v_Mes,
                                                   Naturaleza = a.Naturaleza,

                                               }).ToList().OrderBy(x => x.cuenta).GroupBy(x => x.cuenta).Select(x => x.FirstOrDefault()).ToList()
                                             .Select(l => new ReporteHojaTrabajo
                                             {
                                                 cuenta = l.cuenta,
                                                 denominacion = l.denominacion == "" ? "*No Existe*" : NombreCuenta(l.cuenta, ListaAsientoContable),
                                                 imputable = l.denominacion != "" ? NombreCuentaImputable(l.cuenta.Trim().Substring(0, 2)).FirstOrDefault().imputable : -1,
                                                 Naturaleza = l.Naturaleza,
                                             }).ToList();
                }
                else
                {

                    ReporteCuentaMayorFinal = (from a in saldoscontables
                                               join b in dbContext.asientocontable on new { cuenta = a.v_NroCuenta, eliminado = 0, p = periodo }
                                                                                          equals new { cuenta = b.v_NroCuenta, eliminado = b.i_Eliminado.Value, p = b.v_Periodo } into b_join
                                               from b in b_join.DefaultIfEmpty()

                                               where (a.v_NroCuenta != null && a.v_NroCuenta != "" && a.v_NroCuenta != "*Cta Eliminada*") // && a.v_Mes == pstrMes

                                               select new
                                               {
                                                   cuenta = a.v_NroCuenta.Trim().Substring(0, 2),
                                                   denominacion = b == null ? "" : "N",
                                                   v_mes = a.v_Mes,
                                                   Naturaleza = a.Naturaleza,

                                               }).ToList().OrderBy(x => x.cuenta).GroupBy(x => x.cuenta).Select(x => x.FirstOrDefault()).ToList()
                                                  .Select(l => new ReporteHojaTrabajo
                                                  {
                                                      cuenta = l.cuenta,
                                                      denominacion = l.denominacion == "" ? "*No Existe*" : NombreCuenta(l.cuenta, ListaAsientoContable),
                                                      imputable = l.denominacion != "" ? NombreCuentaImputable(l.cuenta.Trim().Substring(0, 2)).FirstOrDefault().imputable : -1,
                                                      Naturaleza = l.Naturaleza,
                                                  }).ToList();
                }

                #endregion

                foreach (var item in ReporteCuentaMayorFinal.AsParallel())
                {
                    int pIntDigitos = 2;
                    objReporte = new ReporteHojaTrabajo();
                    List<ReporteHojaTrabajo> ListaIncluyeApertura = new List<ReporteHojaTrabajo>();
                    if (balancesunat || LibroInicial)
                    {
                        ListaIncluyeApertura = ReporteTotal.Concat(AsientoApertura).ToList();
                        if (LibroInicial)// se agrego para LibroInicial
                        {
                            ReporteTotal = AsientoApertura;
                        }

                    }
                    else
                    {
                        ListaIncluyeApertura = ReporteTotal;
                    }
                    //var ListaEmpiezaCuenta = ReporteTotal.Where(x => x.cuenta.StartsWith(item.cuenta)).ToList();
                    var ListaEmpiezaCuenta = ListaIncluyeApertura.Where(x => x.cuenta.StartsWith(item.cuenta)).ToList();
                    if (pIntDigitos == pIntNumeroDigitos)
                    {
                        objReporte.cuenta = item.cuenta;
                        objReporte.denominacion = item.denominacion;
                        objReporte.Naturaleza = item.Naturaleza;
                        if (balancesunat)
                        {

                            objReporte.debeApertura = pIntMoneda == (int)Currency.Soles ? AsientoApertura.Where(x => x.cuenta.StartsWith(objReporte.cuenta)).Sum(x => x.d_ImporteSolesD) : AsientoApertura.Where(x => x.cuenta.StartsWith(objReporte.cuenta)).Sum(x => x.d_ImporteDolaresD);
                            objReporte.haberApertura = pIntMoneda == (int)Currency.Soles ? AsientoApertura.Where(x => x.cuenta.StartsWith(objReporte.cuenta)).Sum(x => x.d_ImporteSolesH) : AsientoApertura.Where(x => x.cuenta.StartsWith(objReporte.cuenta)).Sum(x => x.d_ImporteDolaresH);

                        }

                        objReporte.debeSumaAcumulada = SumaCuentasReporteHojaTrabajo(ReporteTotal, objReporte.cuenta.Trim(), "D", pIntMoneda, pstrCuentasImputables == 1 ? true : false);
                        objReporte.haberSumaAcumulada = SumaCuentasReporteHojaTrabajo(ReporteTotal, objReporte.cuenta.Trim(), "H", pIntMoneda, pstrCuentasImputables == 1 ? true : false);
                        List<decimal> Saldos = new List<decimal>();
                        if (LibroInicial)
                        {
                            Saldos = SaldosReporteHojaTrabajo(ReporteTotal, objReporte.cuenta, pIntMoneda, pstrCuentasImputables == 1 ? true : false);
                            objReporte.GrupoLibroInicial = GrupoLibroInicial(ref objOperationResult, objReporte.cuenta);
                            if (objOperationResult.Success == 0) return null;
                            var Titulo = objReporte.GrupoLibroInicial.Split(' ');
                            objReporte.LetrasTotalGrupoInicial = "TOTAL  " + Titulo[1] + " :";
                        }
                        else
                        {
                            Saldos = SaldosReporteHojaTrabajo(ReporteTotal, objReporte.cuenta, pIntMoneda, pstrCuentasImputables == 1 ? true : false, AsientoApertura);
                        }
                        objReporte.saldoDeudor = Saldos[0] > Saldos[1] ? Saldos[0] - Saldos[1] : 0;
                        objReporte.saldoAcreedor = Saldos[1] > Saldos[0] ? Saldos[1] - Saldos[0] : 0;
                        objReporte.debeAjustes = objReporte.cuenta != "*No Existe*" ? CuentaAjuste(objReporte.cuenta) ? objReporte.cuenta.StartsWith("61") ? SumaCuentaAjuste61(objReporte.cuenta, ReporteTotal, pIntMoneda, "D") : objReporte.saldoAcreedor : 0 : 0;
                        objReporte.haberAjustes = objReporte.cuenta != "*No Existe*" ? CuentaAjuste(objReporte.cuenta) ? objReporte.cuenta.StartsWith("61") ? SumaCuentaAjuste61(objReporte.cuenta, ReporteTotal, pIntMoneda, "H") : objReporte.saldoDeudor : 0 : 0;
                        objReporte.activoInventario = objReporte.cuenta != "*No Existe*" ? int.Parse(objReporte.cuenta.Substring(0, 2)) >= 10 && int.Parse(objReporte.cuenta.Substring(0, 2)) <= 59 ? objReporte.saldoDeudor : 0 : 0;
                        objReporte.pasivoInventario = objReporte.cuenta != "*No Existe*" ? int.Parse(objReporte.cuenta.Substring(0, 2)) >= 10 && int.Parse(objReporte.cuenta.Substring(0, 2)) <= 59 ? objReporte.saldoAcreedor : 0 : 0;
                        if (LibroInicial)
                        {
                            objReporte.activoInventario = objReporte.activoInventario != 0 ? objReporte.Naturaleza == "D" ? objReporte.activoInventario : -1 * objReporte.activoInventario : 0;
                            objReporte.pasivoInventario = objReporte.pasivoInventario != 0 ? objReporte.Naturaleza == "H" ? objReporte.pasivoInventario : -1 * objReporte.pasivoInventario : 0;
                        }

                        //De la 60-69 -- TODA la 70 menos 79 (Naturaleza)
                        //la cuenta 69, toda la 7  (menos 79 ni 71 ), toda la 9 (Función)
                        objReporte.perdidaNaturaleza = objReporte.cuenta != "*No Existe*" ? CuentaPerdidaNaturaleza(objReporte.cuenta) ? objReporte.saldoDeudor : 0 : 0;
                        objReporte.gananciaNaturaleza = objReporte.cuenta != "*No Existe*" ? CuentaPerdidaNaturaleza(objReporte.cuenta) ? objReporte.saldoAcreedor : 0 : 0;
                        objReporte.perdidaFuncion = objReporte.cuenta != "*No Existe*" ? CuentaFuncion(objReporte.cuenta) ? objReporte.saldoDeudor : 0 : 0;
                        objReporte.gananciaFuncion = objReporte.cuenta != "*No Existe*" ? CuentaFuncion(objReporte.cuenta) ? objReporte.saldoAcreedor : 0 : 0;
                        objReporte.imputable = item.imputable;
                        pIntDigitos = pIntDigitos + 1;
                        ListaHojaTrabajo.Add(objReporte);

                    }

                    while (pIntDigitos <= pIntNumeroDigitos)
                    {
                        objReporte = new ReporteHojaTrabajo();
                        string a = ListaIncluyeApertura.Where(x => x.cuenta.StartsWith(item.cuenta)).Count() != 0 ? ListaIncluyeApertura.Where(x => x.cuenta.StartsWith(item.cuenta)).FirstOrDefault().cuenta.Length >= pIntDigitos ? ListaIncluyeApertura.Where(x => x.cuenta.StartsWith(item.cuenta)).FirstOrDefault().cuenta.Substring(0, pIntDigitos) : "*No Existe*" : "*No Existe*";
                        if (a != "*No Existe*")
                        {
                            objReporte.cuenta = a;
                            var CuentaImputable = NombreCuentaImputable(a);
                            string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
                            int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
                            objReporte.denominacion = a != "*No Existe*" ? denominacion : "*No Existe*";
                            objReporte.Naturaleza = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().Naturaleza : "";

                            if (balancesunat)
                            {

                                objReporte.debeApertura = pIntMoneda == (int)Currency.Soles ? AsientoApertura.Where(x => x.cuenta.StartsWith(objReporte.cuenta)).Sum(x => x.d_ImporteSolesD) : AsientoApertura.Where(x => x.cuenta.StartsWith(objReporte.cuenta)).Sum(x => x.d_ImporteDolaresD);
                                objReporte.haberApertura = pIntMoneda == (int)Currency.Soles ? AsientoApertura.Where(x => x.cuenta.StartsWith(objReporte.cuenta)).Sum(x => x.d_ImporteSolesH) : AsientoApertura.Where(x => x.cuenta.StartsWith(objReporte.cuenta)).Sum(x => x.d_ImporteDolaresH);

                            }

                            objReporte.debeSumaAcumulada = objReporte.cuenta != "*No Existe*" ? SumaCuentasReporteHojaTrabajo(ReporteTotal, objReporte.cuenta.Trim(), "D", pIntMoneda, pstrCuentasImputables == 1 ? true : false) : 0;
                            objReporte.haberSumaAcumulada = objReporte.cuenta != "*No Existe*" ? SumaCuentasReporteHojaTrabajo(ReporteTotal, objReporte.cuenta.Trim(), "H", pIntMoneda, pstrCuentasImputables == 1 ? true : false) : 0;

                            List<decimal> Saldos = new List<decimal>();
                            if (LibroInicial)
                            {
                                Saldos = SaldosReporteHojaTrabajo(ReporteTotal, objReporte.cuenta, pIntMoneda, pstrCuentasImputables == 1 ? true : false);
                                objReporte.GrupoLibroInicial = GrupoLibroInicial(ref objOperationResult, objReporte.cuenta);
                                if (objOperationResult.Success == 0) return null;
                                var Titulo = objReporte.GrupoLibroInicial.Split(' ');
                                objReporte.LetrasTotalGrupoInicial = "TOTAL  " + Titulo[1] + " :";
                            }
                            else
                            {
                                Saldos = SaldosReporteHojaTrabajo(ReporteTotal, objReporte.cuenta, pIntMoneda, pstrCuentasImputables == 1 ? true : false, AsientoApertura);
                            }
                            objReporte.saldoDeudor = Saldos[0] > Saldos[1] ? Saldos[0] - Saldos[1] : 0;
                            objReporte.saldoAcreedor = Saldos[1] > Saldos[0] ? Saldos[1] - Saldos[0] : 0;
                            objReporte.debeAjustes = objReporte.cuenta != "*No Existe*" ? CuentaAjuste(objReporte.cuenta) ? objReporte.cuenta.StartsWith("61") ? SumaCuentaAjuste61(objReporte.cuenta, ReporteTotal, pIntMoneda, "D") : objReporte.saldoAcreedor : 0 : 0;
                            objReporte.haberAjustes = objReporte.cuenta != "*No Existe*" ? CuentaAjuste(objReporte.cuenta) ? objReporte.cuenta.StartsWith("61") ? SumaCuentaAjuste61(objReporte.cuenta, ReporteTotal, pIntMoneda, "H") : objReporte.saldoDeudor : 0 : 0;
                            objReporte.activoInventario = objReporte.cuenta != "*No Existe*" ? int.Parse(objReporte.cuenta.Substring(0, 2)) >= 10 && int.Parse(objReporte.cuenta.Substring(0, 2)) <= 59 ? objReporte.saldoDeudor : 0 : 0;
                            objReporte.pasivoInventario = objReporte.cuenta != "*No Existe*" ? int.Parse(objReporte.cuenta.Substring(0, 2)) >= 10 && int.Parse(objReporte.cuenta.Substring(0, 2)) <= 59 ? objReporte.saldoAcreedor : 0 : 0;

                            if (LibroInicial)
                            {
                                objReporte.activoInventario = objReporte.activoInventario != 0 ? objReporte.Naturaleza == "D" ? objReporte.activoInventario : -1 * objReporte.activoInventario : 0;
                                objReporte.pasivoInventario = objReporte.pasivoInventario != 0 ? objReporte.Naturaleza == "H" ? objReporte.pasivoInventario : -1 * objReporte.pasivoInventario : 0;
                            }

                            objReporte.perdidaNaturaleza = objReporte.cuenta != "*No Existe*" ? CuentaPerdidaNaturaleza(objReporte.cuenta) ? objReporte.saldoDeudor : 0 : 0;
                            objReporte.gananciaNaturaleza = objReporte.cuenta != "*No Existe*" ? CuentaPerdidaNaturaleza(objReporte.cuenta) ? objReporte.saldoAcreedor : 0 : 0;
                            objReporte.perdidaFuncion = objReporte.cuenta != "*No Existe*" ? CuentaFuncion(objReporte.cuenta) ? objReporte.saldoDeudor : 0 : 0;
                            objReporte.gananciaFuncion = objReporte.cuenta != "*No Existe*" ? CuentaFuncion(objReporte.cuenta) ? objReporte.saldoAcreedor : 0 : 0;
                            objReporte.imputable = a != "*No Existe*" ? imputable : -1;

                            ListaHojaTrabajo.Add(objReporte);
                        }
                        pIntDigitos = pIntDigitos + 1;
                    }

                    int Contador = 1;
                    pIntDigitos = 2;

                    if (Contador < ListaEmpiezaCuenta.Count())
                    {
                        foreach (var subCuenta in ListaEmpiezaCuenta.AsParallel())
                        {


                            while (pIntDigitos <= pIntNumeroDigitos)
                            {
                                objReporte = new ReporteHojaTrabajo();
                                string a = subCuenta.cuenta.StartsWith(item.cuenta) && subCuenta.cuenta.Length >= pIntDigitos ? subCuenta.cuenta.Substring(0, pIntDigitos) : "*No Existe*";
                                objReporte.cuenta = a;

                                if (a != "*No Existe*" && ListaHojaTrabajo.Where(x => x.cuenta == objReporte.cuenta).Count() == 0)
                                {
                                    var CuentaImputable = NombreCuentaImputable(a);
                                    string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
                                    int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
                                    objReporte.denominacion = a != "*No Existe*" ? denominacion : "*No Existe*";
                                    objReporte.Naturaleza = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().Naturaleza : "";
                                    if (balancesunat)
                                    {

                                        objReporte.debeApertura = pIntMoneda == (int)Currency.Soles ? AsientoApertura.Where(x => x.cuenta.StartsWith(objReporte.cuenta)).Sum(x => x.d_ImporteSolesD) : AsientoApertura.Where(x => x.cuenta.StartsWith(objReporte.cuenta)).Sum(x => x.d_ImporteDolaresD);
                                        objReporte.haberApertura = pIntMoneda == (int)Currency.Soles ? AsientoApertura.Where(x => x.cuenta.StartsWith(objReporte.cuenta)).Sum(x => x.d_ImporteSolesH) : AsientoApertura.Where(x => x.cuenta.StartsWith(objReporte.cuenta)).Sum(x => x.d_ImporteDolaresH);

                                    }

                                    objReporte.debeSumaAcumulada = objReporte.cuenta != "*No Existe*" ? SumaCuentasReporteHojaTrabajo(ReporteTotal, objReporte.cuenta.Trim(), "D", pIntMoneda, pstrCuentasImputables == 1 ? true : false) : 0;
                                    objReporte.haberSumaAcumulada = objReporte.cuenta != "*No Existe*" ? SumaCuentasReporteHojaTrabajo(ReporteTotal, objReporte.cuenta.Trim(), "H", pIntMoneda, pstrCuentasImputables == 1 ? true : false) : 0;
                                    List<decimal> Saldos = new List<decimal>();
                                    if (LibroInicial)
                                    {
                                        Saldos = SaldosReporteHojaTrabajo(ReporteTotal, objReporte.cuenta, pIntMoneda, pstrCuentasImputables == 1 ? true : false);
                                        objReporte.GrupoLibroInicial = GrupoLibroInicial(ref objOperationResult, objReporte.cuenta);
                                        if (objOperationResult.Success == 0) return null;
                                        var Titulo = objReporte.GrupoLibroInicial.Split(' ');
                                        objReporte.LetrasTotalGrupoInicial = "TOTAL  " + Titulo[1] + " :";
                                    }
                                    else
                                    {
                                        Saldos = SaldosReporteHojaTrabajo(ReporteTotal, objReporte.cuenta, pIntMoneda, pstrCuentasImputables == 1 ? true : false, AsientoApertura);
                                    }

                                    objReporte.saldoDeudor = Saldos[0] > Saldos[1] ? Saldos[0] - Saldos[1] : 0;
                                    objReporte.saldoAcreedor = Saldos[1] > Saldos[0] ? Saldos[1] - Saldos[0] : 0;



                                    objReporte.debeAjustes = objReporte.cuenta != "*No Existe*" ? CuentaAjuste(objReporte.cuenta) ? objReporte.cuenta.StartsWith("61") ? SumaCuentaAjuste61(objReporte.cuenta, ReporteTotal, pIntMoneda, "D") : objReporte.saldoAcreedor : 0 : 0;
                                    objReporte.haberAjustes = objReporte.cuenta != "*No Existe*" ? CuentaAjuste(objReporte.cuenta) ? objReporte.cuenta.StartsWith("61") ? SumaCuentaAjuste61(objReporte.cuenta, ReporteTotal, pIntMoneda, "H") : objReporte.saldoDeudor : 0 : 0;
                                    objReporte.activoInventario = objReporte.cuenta != "*No Existe*" ? int.Parse(objReporte.cuenta.Substring(0, 2)) >= 10 && int.Parse(objReporte.cuenta.Substring(0, 2)) <= 59 ? objReporte.saldoDeudor : 0 : 0;
                                    objReporte.pasivoInventario = objReporte.cuenta != "*No Existe*" ? int.Parse(objReporte.cuenta.Substring(0, 2)) >= 10 && int.Parse(objReporte.cuenta.Substring(0, 2)) <= 59 ? objReporte.saldoAcreedor : 0 : 0;

                                    if (LibroInicial)
                                    {
                                        objReporte.activoInventario = objReporte.activoInventario != 0 ? objReporte.Naturaleza == "D" ? objReporte.activoInventario : -1 * objReporte.activoInventario : 0;
                                        objReporte.pasivoInventario = objReporte.pasivoInventario != 0 ? objReporte.Naturaleza == "H" ? objReporte.pasivoInventario : -1 * objReporte.pasivoInventario : 0;
                                    }
                                    objReporte.perdidaNaturaleza = objReporte.cuenta != "*No Existe*" ? CuentaPerdidaNaturaleza(objReporte.cuenta) ? objReporte.saldoDeudor : 0 : 0;
                                    objReporte.gananciaNaturaleza = objReporte.cuenta != "*No Existe*" ? CuentaPerdidaNaturaleza(objReporte.cuenta) ? objReporte.saldoAcreedor : 0 : 0;
                                    objReporte.perdidaFuncion = objReporte.cuenta != "*No Existe*" ? CuentaFuncion(objReporte.cuenta) ? objReporte.saldoDeudor : 0 : 0;
                                    objReporte.gananciaFuncion = objReporte.cuenta != "*No Existe*" ? CuentaFuncion(objReporte.cuenta) ? objReporte.saldoAcreedor : 0 : 0;
                                    objReporte.imputable = a != "*No Existe*" ? imputable : -1;
                                    ListaHojaTrabajo.Add(objReporte);

                                }
                                pIntDigitos = pIntDigitos + 1;
                            }
                            pIntDigitos = 2;

                        }
                    }
                }
                objOperationResult.Success = 1;
                if (balancesunat)
                {

                    if (pstrCuentasImputables == 1)
                    {


                        return ListaHojaTrabajo.ToList().Where(y => y.imputable == 1 || (y.imputable == 0 && y.cuenta.Length == 2)).OrderBy(x => x.cuenta).ToList().Where(x => x.debeApertura != 0 || x.haberApertura != 0 || x.debeSumaAcumulada != 0 || x.haberSumaAcumulada != 0 || x.saldoDeudor != 0 || x.saldoAcreedor != 0).ToList();
                    }
                    else
                    {

                        return ListaHojaTrabajo.OrderBy(x => x.cuenta).ToList().Where(x => x.debeApertura != 0 || x.haberApertura != 0 || x.debeSumaAcumulada != 0 || x.haberSumaAcumulada != 0 || x.saldoDeudor != 0 || x.saldoAcreedor != 0).ToList();
                    }

                }
                else
                {
                    if (pstrCuentasImputables == 1)
                    {
                        //return ListaHojaTrabajo.ToList().Where(y => y.imputable == 1  ).OrderBy(x => x.cuenta).ToList().Where(x => x.debeSumaAcumulada != 0 || x.haberSumaAcumulada != 0 || x.debeAjustes != 0 || x.haberAjustes != 0 || x.activoInventario != 0 || x.pasivoInventario != 0 || x.perdidaFuncion != 0 || x.gananciaFuncion != 0 || x.perdidaNaturaleza != 0 || x.gananciaNaturaleza != 0).ToList();
                        return ListaHojaTrabajo.ToList().Where(y => y.imputable == 1 || (y.cuenta.Length == 2 && y.imputable == 0)).OrderBy(x => x.cuenta).ToList().Where(x => x.debeSumaAcumulada != 0 || x.haberSumaAcumulada != 0 || x.debeAjustes != 0 || x.haberAjustes != 0 || x.activoInventario != 0 || x.pasivoInventario != 0 || x.perdidaFuncion != 0 || x.gananciaFuncion != 0 || x.perdidaNaturaleza != 0 || x.gananciaNaturaleza != 0).ToList(); //Se agregó 9 Mayo
                    }
                    else
                    {
                        return ListaHojaTrabajo.ToList().OrderBy(x => x.cuenta).ToList().Where(x => x.debeSumaAcumulada != 0 || x.haberSumaAcumulada != 0 || x.debeAjustes != 0 || x.haberAjustes != 0 || x.activoInventario != 0 || x.pasivoInventario != 0 || x.perdidaFuncion != 0 || x.gananciaFuncion != 0 || x.perdidaNaturaleza != 0 || x.gananciaNaturaleza != 0).ToList(); ;
                    }
                }


            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "ContabilidadBL.ReporteHojaTrabajo()";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;


            }

        }

        public bool EsCuentaCapital(string NroCuenta)
        {
            return (NroCuenta.StartsWith("50")) ? true : false;

        }


        public List<ReportePatrimonioNeto> ReporteInventarioBalancesPatrimonio(ref OperationResult objOperationResult, string pstrPeriodo, string pstrMes, int pIntNumeroDigitos, int pIntMoneda, int pstrCuentasImputables, bool balancesunat = false, bool LibroInicial = false)
        {
            try
            {
                //TODO:Revisar
                AsientosContablesBL _objAsientoContableBL = new AsientosContablesBL();
                ReportePatrimonioNeto objReporte = new ReportePatrimonioNeto();
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                List<ReportePatrimonioNeto> ListaHojaTrabajo = new List<ReportePatrimonioNeto>();

                List<asientocontable> ListaAsientoContable = dbContext.asientocontable.ToList();

                List<ReportePatrimonioNeto> AsientoApertura = new List<ReportePatrimonioNeto>();
                List<saldoscontablesDto> saldoscontables = new List<saldoscontablesDto>();
                //if (balancesunat || LibroInicial)
                //{
                #region Recopila Información de AsientoApertura
                AsientoApertura = (from a in dbContext.diario

                                   join b in dbContext.diariodetalle on new { IdDiario = a.v_IdDiario, eliminado = 0 }
                                                                    equals new { IdDiario = b.v_IdDiario, eliminado = b.i_Eliminado.Value } into b_join
                                   from b in b_join.DefaultIfEmpty()

                                   join c in dbContext.asientocontable on new { asiento = b.v_NroCuenta, eliminado = 0, p = periodo }
                                                                    equals new { asiento = c.v_NroCuenta, eliminado = c.i_Eliminado.Value, p = c.v_Periodo } into c_join

                                   from c in c_join.DefaultIfEmpty()

                                   where a.v_Periodo == pstrPeriodo && a.i_IdTipoComprobante == 1 && a.i_Eliminado == 0 && (b.v_NroCuenta != null || b.v_NroCuenta != "") //&& a.v_Mes == "01"

                                   orderby b.v_NroCuenta
                                   select new
                                   {

                                       cuenta = b.v_NroCuenta.Trim(),
                                       //denominacion = c == null ? "*No Existe*" : c.v_NombreCuenta.Trim(),
                                       // v_mes = a.v_Mes,
                                       //imputable = c == null ? -1 : c.i_Imputable.Value,
                                       d_ImporteSolesD = a.i_IdMoneda == 1 && b.v_Naturaleza == "D" ? b.d_Importe.Value : a.i_IdMoneda == 2 && b.v_Naturaleza == "D" ? b.d_Cambio.Value : 0,
                                       d_ImporteDolaresD = a.i_IdMoneda == 2 && b.v_Naturaleza == "D" ? b.d_Importe.Value : a.i_IdMoneda == 1 && b.v_Naturaleza == "D" ? b.d_Cambio.Value : 0,
                                       d_ImporteSolesH = a.i_IdMoneda == 1 && b.v_Naturaleza == "H" ? b.d_Importe.Value : a.i_IdMoneda == 2 && b.v_Naturaleza == "H" ? b.d_Cambio.Value : 0,
                                       d_ImporteDolaresH = a.i_IdMoneda == 2 && b.v_Naturaleza == "H" ? b.d_Importe.Value : a.i_IdMoneda == 1 && b.v_Naturaleza == "H" ? b.d_Cambio.Value : 0,
                                       i_IdPatrimonioNeto = b.i_IdPatrimonioNeto,
                                       //Naturaleza = c.i_Naturaleza == 2 ? "H" : "D",
                                   }).ToList().Select(o => new ReportePatrimonioNeto
                                   {
                                       cuenta = o.cuenta,
                                       d_ImporteDolaresD = o.d_ImporteDolaresD,
                                       d_ImporteDolaresH = o.d_ImporteDolaresH,
                                       d_ImporteSolesD = o.d_ImporteSolesD,
                                       d_ImporteSolesH = o.d_ImporteSolesH,
                                       i_IdPatrimonioNeto = o.i_IdPatrimonioNeto == null ? 0 : int.Parse(o.i_IdPatrimonioNeto),
                                   }).ToList();

                #endregion


                //    if (!LibroInicial)   //se agrego para libro Inicial
                //        saldoscontables = _objAsientoContableBL.ObtenerSaldosContablesPorMes(ref  objOperationResult, int.Parse(pstrMes), int.Parse(pstrPeriodo), 1, false, false, null, false, null, null, true);

                //    #endregion
                //}
                //else
                //{
                saldoscontables = _objAsientoContableBL.ObtenerSaldosContablesPorMes(ref  objOperationResult, int.Parse(pstrMes), int.Parse(pstrPeriodo), 1, false, false, null, false, null, null, false, true);
                //}

                #region RecopilaSaldosContables-CuentasRespectivas
                var ReporteTotal = (from a in saldoscontables
                                    join b in dbContext.asientocontable on new { cuenta = a.v_NroCuenta, eliminado = 0, p = periodo }
                                                                              equals new { cuenta = b.v_NroCuenta, eliminado = b.i_Eliminado.Value, p = b.v_Periodo } into b_join
                                    from b in b_join.DefaultIfEmpty()

                                    where (a.v_NroCuenta != null && a.v_NroCuenta != "" && a.v_NroCuenta != "*Cta Eliminada*") // &&   a.v_Mes == pstrMes
                                    orderby a.v_NroCuenta
                                    select new ReportePatrimonioNeto
                                    {
                                        cuenta = a.v_NroCuenta == "*Cta Eliminada*" ? "" : a.v_NroCuenta.Trim(),
                                        // denominacion = a.v_NroCuenta == "*Cta Eliminada*" ? "*No Existe*" : b == null ? "*No Existe*" : b.v_NombreCuenta.Trim(),
                                        d_ImporteDolaresD = a.d_ImporteDolaresD.Value,
                                        d_ImporteDolaresH = a.d_ImporteDolaresH.Value,
                                        d_ImporteSolesH = a.d_ImporteSolesH.Value,
                                        d_ImporteSolesD = a.d_ImporteSolesD.Value,
                                        //v_mes = a.v_Mes,
                                        Naturaleza = a.Naturaleza,
                                        i_IdPatrimonioNeto = a.i_IdPatrimonioNeto,

                                    }).ToList().OrderBy(l => l.cuenta).ToList();

                #endregion
                #region RecopilaCuentasMayoresdeSaldosContables-CuentasRespectivas

                List<ReportePatrimonioNeto> ReporteCuentaMayorFinal = new List<ReportePatrimonioNeto>();
                //if (balancesunat || LibroInicial) // LibroInicial se agrego para BalanceInicial
                //{

                //    var saldosasientosapertura = (from a in AsientoApertura

                //                                  select new saldoscontablesDto
                //                                  {

                //                                      v_NroCuenta = a.cuenta,
                //                                      v_Mes = a.v_mes,
                //                                      Naturaleza = a.Naturaleza,

                //                                  }).ToList();


                //    saldoscontables = saldoscontables.Concat(saldosasientosapertura).ToList();
                //    ReporteCuentaMayorFinal = (from a in saldoscontables
                //                               join b in dbContext.asientocontable on new { cuenta = a.v_NroCuenta, eliminado = 0, p = periodo }
                //                                                                          equals new { cuenta = b.v_NroCuenta, eliminado = b.i_Eliminado.Value, p = b.v_Periodo } into b_join
                //                               from b in b_join.DefaultIfEmpty()

                //                               where (a.v_NroCuenta != null && a.v_NroCuenta != "" && a.v_NroCuenta != "*Cta Eliminada*") // && a.v_Mes == pstrMes

                //                               select new
                //                               {
                //                                   cuenta = a.v_NroCuenta.Trim().Substring(0, 2),
                //                                   denominacion = b == null ? "" : "N",
                //                                   v_mes = a.v_Mes,
                //                                   Naturaleza = a.Naturaleza,

                //                               }).ToList().OrderBy(x => x.cuenta).GroupBy(x => x.cuenta).Select(x => x.FirstOrDefault()).ToList()
                //                             .Select(l => new ReporteHojaTrabajo
                //                             {
                //                                 cuenta = l.cuenta,
                //                                 denominacion = l.denominacion == "" ? "*No Existe*" : NombreCuenta(l.cuenta, ListaAsientoContable),
                //                                 imputable = l.denominacion != "" ? NombreCuentaImputable(l.cuenta.Trim().Substring(0, 2)).FirstOrDefault().imputable : -1,
                //                                 Naturaleza = l.Naturaleza,
                //                             }).ToList();
                //}
                //else
                //{

                //ReporteCuentaMayorFinal = (from a in saldoscontables
                //                           join b in dbContext.asientocontable on new { cuenta = a.v_NroCuenta, eliminado = 0, p = periodo }
                //                                                                      equals new { cuenta = b.v_NroCuenta, eliminado = b.i_Eliminado.Value, p = b.v_Periodo } into b_join
                //                           from b in b_join.DefaultIfEmpty()

                //                           where (a.v_NroCuenta != null && a.v_NroCuenta != "" && a.v_NroCuenta != "*Cta Eliminada*") // && a.v_Mes == pstrMes

                //                           select new
                //                           {
                //                               cuenta = a.v_NroCuenta.Trim().Substring(0, 2),
                //                               denominacion = b == null ? "" : "N",
                //                               v_mes = a.v_Mes,
                //                               Naturaleza = a.Naturaleza,

                //                           }).ToList().OrderBy(x => x.cuenta).GroupBy(x => x.cuenta).Select(x => x.FirstOrDefault()).ToList()
                //                              .Select(l => new ReporteHojaTrabajo
                //                              {
                //                                  cuenta = l.cuenta,
                //                                  denominacion = l.denominacion == "" ? "*No Existe*" : NombreCuenta(l.cuenta, ListaAsientoContable),
                //                                  imputable = l.denominacion != "" ? NombreCuentaImputable(l.cuenta.Trim().Substring(0, 2)).FirstOrDefault().imputable : -1,
                //                                  Naturaleza = l.Naturaleza,
                //                              }).ToList();
                //}

                List<NotasPatrimonioNeto> ListaPatrimonioNeto = (from a in dbContext.datahierarchy
                                                                 where a.i_IsDeleted == 0 && a.i_GroupId == 171

                                                                 select new NotasPatrimonioNeto
                                                                      {
                                                                          NombreNota = a.v_Value1,
                                                                          i_ItemId = a.i_ItemId,
                                                                          Tipo = "D",

                                                                      }).ToList();

                NotasPatrimonioNeto objNotaSaldoInicial = new NotasPatrimonioNeto();
                objNotaSaldoInicial.i_ItemId = 0;
                objNotaSaldoInicial.NombreNota = "SALDO INICIAL";
                objNotaSaldoInicial.Tipo = "A";
                ListaPatrimonioNeto.Add(objNotaSaldoInicial);

                NotasPatrimonioNeto objNotaSaldoFinal = new NotasPatrimonioNeto();
                objNotaSaldoFinal.i_ItemId = 0;
                objNotaSaldoFinal.NombreNota = "SALDO FINAL";
                objNotaSaldoFinal.Tipo = "F";
                ListaPatrimonioNeto.Add(objNotaSaldoFinal);

                ListaPatrimonioNeto = ListaPatrimonioNeto.OrderBy(o => o.Tipo).ToList();

                #endregion
                List<ReportePatrimonioNeto> ListaIncluyeApertura = new List<ReportePatrimonioNeto>();
                foreach (var NotaPatrimonio in ListaPatrimonioNeto.AsParallel())
                {
                    objReporte = new ReportePatrimonioNeto();
                    var Capital = NotaPatrimonio.Tipo == "A" ? AsientoApertura.Where(o => o.cuenta.StartsWith("50")).ToList() : ReporteTotal.Where(o => o.cuenta.StartsWith("50") && o.i_IdPatrimonioNeto == NotaPatrimonio.i_ItemId).ToList();
                    objReporte.Capital = NotaPatrimonio.Tipo == "F" ? ListaIncluyeApertura.Sum(x => x.Capital) : pIntMoneda == (int)Currency.Soles ? Capital.Sum(o => o.d_ImporteSolesH) - Capital.Sum(o => o.d_ImporteSolesD) : Capital.Sum(o => o.d_ImporteDolaresH) - Capital.Sum(o => o.d_ImporteDolaresD);
                    //objReporte.Capital = objReporte.Capital < 0 ? objReporte.Capital * -1 : objReporte.Capital;

                    var CapitalAdicional = NotaPatrimonio.Tipo == "A" ? AsientoApertura.Where(o => o.cuenta.StartsWith("52")).ToList() : ReporteTotal.Where(o => o.cuenta.StartsWith("52") && o.i_IdPatrimonioNeto == NotaPatrimonio.i_ItemId).ToList();
                    objReporte.CapitalAdicional = NotaPatrimonio.Tipo == "F" ? ListaIncluyeApertura.Sum(o => o.CapitalAdicional) : pIntMoneda == (int)Currency.Soles ? CapitalAdicional.Sum(o => o.d_ImporteSolesH) - CapitalAdicional.Sum(o => o.d_ImporteSolesH) : CapitalAdicional.Sum(o => o.d_ImporteDolaresH) - CapitalAdicional.Sum(o => o.d_ImporteDolaresD);
                    //objReporte.CapitalAdicional = objReporte.CapitalAdicional < 0 ? objReporte.CapitalAdicional * -1 : objReporte.CapitalAdicional;

                    var AccionesInversion = NotaPatrimonio.Tipo == "A" ? AsientoApertura.Where(o => o.cuenta.StartsWith("51")).ToList() : ReporteTotal.Where(o => o.cuenta.StartsWith("51") && o.i_IdPatrimonioNeto == NotaPatrimonio.i_ItemId).ToList();
                    objReporte.AccionesInversion = NotaPatrimonio.Tipo == "F" ? ListaIncluyeApertura.Sum(o => o.AccionesInversion) : pIntMoneda == (int)Currency.Soles ? AccionesInversion.Sum(o => o.d_ImporteSolesH) - AccionesInversion.Sum(o => o.d_ImporteSolesD) : AccionesInversion.Sum(o => o.d_ImporteDolaresH) - AccionesInversion.Sum(o => o.d_ImporteDolaresD);

                    //objReporte.AccionesInversion = objReporte.AccionesInversion < 0 ? objReporte.AccionesInversion * -1 : objReporte.AccionesInversion;

                    var ExcedenteRevaluacion = NotaPatrimonio.Tipo == "A" ? AsientoApertura.Where(o => o.cuenta.StartsWith("57")).ToList() : ReporteTotal.Where(o => o.cuenta.StartsWith("57") && o.i_IdPatrimonioNeto == NotaPatrimonio.i_ItemId).ToList();
                    objReporte.ExcedenteRevaluacion = NotaPatrimonio.Tipo == "F" ? ListaIncluyeApertura.Sum(o => o.ExcedenteRevaluacion) : pIntMoneda == (int)Currency.Soles ? ExcedenteRevaluacion.Sum(o => o.d_ImporteSolesH) - ExcedenteRevaluacion.Sum(o => o.d_ImporteSolesD) : ExcedenteRevaluacion.Sum(o => o.d_ImporteDolaresH) - ExcedenteRevaluacion.Sum(o => o.d_ImporteDolaresD);
                    //objReporte.ExcedenteRevaluacion = objReporte.ExcedenteRevaluacion < 0 ? objReporte.ExcedenteRevaluacion * -1 : objReporte.ExcedenteRevaluacion;

                    var ReservaLegal = NotaPatrimonio.Tipo == "A" ? AsientoApertura.Where(o => o.cuenta.StartsWith("58")).ToList() : ReporteTotal.Where(o => o.cuenta.StartsWith("58") && o.i_IdPatrimonioNeto == NotaPatrimonio.i_ItemId).ToList();
                    objReporte.ReservaLegal = NotaPatrimonio.Tipo == "F" ? ListaIncluyeApertura.Sum(o => o.ReservaLegal) : pIntMoneda == (int)Currency.Soles ? ReservaLegal.Sum(o => o.d_ImporteSolesH) - ReservaLegal.Sum(o => o.d_ImporteSolesD) : ReservaLegal.Sum(o => o.d_ImporteDolaresH) - ReservaLegal.Sum(o => o.d_ImporteDolaresD);
                    //objReporte.ReservaLegal =  objReporte.ReservaLegal <0 ? objReporte.ReservaLegal *-1 :objReporte.ReservaLegal ;

                    var OtrasReservas = NotaPatrimonio.Tipo == "A" ? AsientoApertura.Where(o => o.cuenta.StartsWith("58")).ToList() : ReporteTotal.Where(o => o.cuenta.StartsWith("58") && o.i_IdPatrimonioNeto == NotaPatrimonio.i_ItemId).ToList();
                    objReporte.OtrasReservas = NotaPatrimonio.Tipo == "F" ? ListaIncluyeApertura.Sum(o => o.OtrasReservas) : pIntMoneda == (int)Currency.Soles ? OtrasReservas.Sum(o => o.d_ImporteSolesH) - OtrasReservas.Sum(o => o.d_ImporteSolesD) : OtrasReservas.Sum(o => o.d_ImporteDolaresH) - OtrasReservas.Sum(o => o.d_ImporteDolaresD);
                    /// objReporte.OtrasReservas = objReporte.OtrasReservas < 0 ? objReporte.OtrasReservas * -1 : objReporte.OtrasReservas;

                    var ResultadosAcumulados = NotaPatrimonio.Tipo == "A" ? AsientoApertura.Where(o => o.cuenta.StartsWith("59")).ToList() : ReporteTotal.Where(o => o.cuenta.StartsWith("59") && o.i_IdPatrimonioNeto == NotaPatrimonio.i_ItemId).ToList();
                    objReporte.ResultadosAcumulados = NotaPatrimonio.Tipo == "F" ? ListaIncluyeApertura.Sum(o => o.ResultadosAcumulados) : pIntMoneda == (int)Currency.Soles ? ResultadosAcumulados.Sum(o => o.d_ImporteSolesH) - ResultadosAcumulados.Sum(o => o.d_ImporteSolesD) : ResultadosAcumulados.Sum(o => o.d_ImporteDolaresH) - ResultadosAcumulados.Sum(o => o.d_ImporteDolaresD);
                    //objReporte.ResultadosAcumulados = objReporte.ResultadosAcumulados < 0 ? objReporte.ResultadosAcumulados * -1 : objReporte.ResultadosAcumulados;


                    objReporte.Nota = NotaPatrimonio.NombreNota;

                    objReporte.Total = objReporte.Capital + objReporte.CapitalAdicional + objReporte.AccionesInversion + objReporte.ExcedenteRevaluacion + objReporte.ReservaLegal + objReporte.OtrasReservas + objReporte.ResultadosAcumulados;
                    ListaIncluyeApertura.Add(objReporte);
                    //}
                }

                return ListaIncluyeApertura;
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "ContabilidadBL.ReporteHojaTrabajo()";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;


            }

        }




        public string GrupoLibroInicial(ref OperationResult objOperationResult, string Cuenta)
        {
            try
            {
                objOperationResult.Success = 1;
                if (Cuenta.Length >= 2)
                {

                    if (int.Parse(Cuenta.Substring(0, 2)) >= 10 && int.Parse(Cuenta.Substring(0, 2)) < 40)
                        return "I. ACTIVO";
                    else
                        if (int.Parse(Cuenta.Substring(0, 2)) >= 40 && int.Parse(Cuenta.Substring(0, 2)) <= 49)
                            return "II. PASIVO";
                        else
                            if (int.Parse(Cuenta.Substring(0, 2)) >= 50 && int.Parse(Cuenta.Substring(0, 2)) <= 60)
                                return "III. PATRIMONIO";
                }

                return "";
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;

                objOperationResult.AdditionalInformation = "ContabilidadBL.GrupoLibroInicial()";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }
        }



        public List<ReporteHojaTrabajo> ReporteHojaTrabajoSBS(ref OperationResult objOperationResult, string pstrPeriodo, string pstrMes, int pIntNumeroDigitos, int pIntMoneda, int pstrCuentasImputables, string TipoReporte = "SBS")
        {
            try
            {
                //TODO:Revisar
                AsientosContablesBL _objAsientoContableBL = new AsientosContablesBL();
                ReporteHojaTrabajo objReporte = new ReporteHojaTrabajo();
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                List<ReporteHojaTrabajo> ListaHojaTrabajo = new List<ReporteHojaTrabajo>();

                List<asientocontable> ListaAsientoContable = dbContext.asientocontable.ToList();
                #region RecopilaSaldosContables-CuentasRespectivas
                var saldoscontables = _objAsientoContableBL.ObtenerSaldosContablesPorMes(ref  objOperationResult, int.Parse(pstrMes), int.Parse(pstrPeriodo));
                if (TipoReporte == "SBS")
                {
                    saldoscontables = saldoscontables.Where(o => o.v_NroCuenta.StartsWith("1") || o.v_NroCuenta.StartsWith("2") || o.v_NroCuenta.StartsWith("3") || o.v_NroCuenta.StartsWith("4") || o.v_NroCuenta.StartsWith("5") || o.v_NroCuenta.StartsWith("6")).ToList();
                }

                var ReporteTotal = (from a in saldoscontables
                                    join b in dbContext.asientocontable on new { cuenta = a.v_NroCuenta, eliminado = 0, p = periodo }
                                                                              equals new { cuenta = b.v_NroCuenta, eliminado = b.i_Eliminado.Value, p = b.v_Periodo } into b_join
                                    from b in b_join.DefaultIfEmpty()

                                    where (a.v_NroCuenta != null && a.v_NroCuenta != "" && a.v_NroCuenta != "*Cta Eliminada*")

                                    orderby a.v_NroCuenta
                                    select new ReporteHojaTrabajo
                                    {
                                        cuenta = a.v_NroCuenta == "*Cta Eliminada*" ? "" : a.v_NroCuenta.Trim(),
                                        denominacion = a.v_NroCuenta == "*Cta Eliminada*" ? "*No Existe*" : b == null ? "*No Existe*" : b.v_NombreCuenta.Trim(),
                                        d_ImporteDolaresD = a.d_ImporteDolaresD.Value,
                                        d_ImporteDolaresH = a.d_ImporteDolaresH.Value,
                                        d_ImporteSolesH = a.d_ImporteSolesH.Value,
                                        d_ImporteSolesD = a.d_ImporteSolesD.Value,
                                        v_mes = a.v_Mes,


                                    }).ToList().OrderBy(l => l.cuenta).ToList();

                #endregion
                #region RecopilaCuentasMayoresdeSaldosContables-CuentasRespectivas



                var ReporteCuentaMayorFinal = (from a in saldoscontables
                                               join b in dbContext.asientocontable on new { cuenta = a.v_NroCuenta, eliminado = 0, p = periodo }
                                                                                          equals new { cuenta = b.v_NroCuenta, eliminado = b.i_Eliminado.Value, p = b.v_Periodo } into b_join
                                               from b in b_join.DefaultIfEmpty()

                                               where (a.v_NroCuenta != null && a.v_NroCuenta != "" && a.v_NroCuenta != "*Cta Eliminada*") // && a.v_Mes == pstrMes

                                               select new
                                               {
                                                   cuenta = a.v_NroCuenta.Trim().Substring(0, 2),
                                                   denominacion = b == null ? "" : "N",
                                                   v_mes = a.v_Mes,

                                               }).ToList().OrderBy(x => x.cuenta).GroupBy(x => x.cuenta).Select(x => x.FirstOrDefault()).ToList()
                                              .Select(l => new ReporteHojaTrabajo
                                              {
                                                  cuenta = l.cuenta,
                                                  denominacion = l.denominacion == "" ? "*No Existe*" : NombreCuenta(l.cuenta, ListaAsientoContable),
                                                  imputable = l.denominacion != "" ? NombreCuentaImputable(l.cuenta.Trim().Substring(0, 2)).FirstOrDefault().imputable : -1,

                                              }).ToList();

                #endregion
                foreach (var item in ReporteCuentaMayorFinal.AsParallel())
                {
                    int pIntDigitos = 2;
                    objReporte = new ReporteHojaTrabajo();
                    var ListaEmpiezaCuenta = ReporteTotal.Where(x => x.cuenta.StartsWith(item.cuenta)).ToList();
                    if (pIntDigitos == pIntNumeroDigitos)
                    {
                        objReporte.cuenta = item.cuenta;
                        objReporte.denominacion = item.denominacion;
                        objReporte.debeSumaAcumulada = SumaCuentasReporteHojaTrabajo(ReporteTotal, objReporte.cuenta.Trim(), "D", pIntMoneda, pstrCuentasImputables == 1 ? true : false);
                        objReporte.haberSumaAcumulada = SumaCuentasReporteHojaTrabajo(ReporteTotal, objReporte.cuenta.Trim(), "H", pIntMoneda, pstrCuentasImputables == 1 ? true : false);
                        var Saldos = SaldosReporteHojaTrabajo(ReporteTotal, objReporte.cuenta, pIntMoneda, pstrCuentasImputables == 1 ? true : false, null);

                        objReporte.saldoDeudor = Saldos[0] > Saldos[1] ? Saldos[0] - Saldos[1] : 0;
                        objReporte.saldoAcreedor = Saldos[1] > Saldos[0] ? Saldos[1] - Saldos[0] : 0;

                        //objReporte.debeAjustes = objReporte.cuenta != "*No Existe*" ? CuentaAjuste(objReporte.cuenta,"S") ? objReporte.cuenta.StartsWith("61") ? SumaCuentaAjuste61(objReporte.cuenta, ReporteTotal, pIntMoneda, "D") : objReporte.saldoAcreedor : 0 : 0;
                        //objReporte.haberAjustes = objReporte.cuenta != "*No Existe*" ? CuentaAjuste(objReporte.cuenta,"S") ? objReporte.cuenta.StartsWith("61") ? SumaCuentaAjuste61(objReporte.cuenta, ReporteTotal, pIntMoneda, "H") : objReporte.saldoDeudor : 0 : 0;
                        //Cuando la empresa no es prico se toma en cuenta que la suma de la 61 = 69
                        //En la empresa prico ya no se toma en cuenta esa logica


                        objReporte.debeAjustes = objReporte.cuenta != "*No Existe*" ? CuentaAjuste(objReporte.cuenta, "S") ? objReporte.saldoAcreedor : 0 : 0;
                        objReporte.haberAjustes = objReporte.cuenta != "*No Existe*" ? CuentaAjuste(objReporte.cuenta, "S") ? objReporte.saldoDeudor : 0 : 0;

                        objReporte.activoInventario = objReporte.cuenta != "*No Existe*" ? int.Parse(objReporte.cuenta.Substring(0, 2)) <= 39 ? objReporte.saldoDeudor : 0 : 0;
                        objReporte.pasivoInventario = objReporte.cuenta != "*No Existe*" ? int.Parse(objReporte.cuenta.Substring(0, 2)) <= 39 ? objReporte.saldoAcreedor : 0 : 0;
                        objReporte.perdidaNaturaleza = objReporte.cuenta != "*No Existe*" ? CuentaPerdidaNaturaleza(objReporte.cuenta, "S") ? objReporte.saldoDeudor : 0 : 0;
                        objReporte.gananciaNaturaleza = objReporte.cuenta != "*No Existe*" ? CuentaPerdidaNaturaleza(objReporte.cuenta, "S") ? objReporte.saldoAcreedor : 0 : 0;
                        objReporte.perdidaFuncion = objReporte.cuenta != "*No Existe*" ? CuentaFuncion(objReporte.cuenta, "S") ? objReporte.saldoDeudor : 0 : 0;
                        objReporte.gananciaFuncion = objReporte.cuenta != "*No Existe*" ? CuentaFuncion(objReporte.cuenta, "S") ? objReporte.saldoAcreedor : 0 : 0;
                        objReporte.imputable = item.imputable;
                        pIntDigitos = pIntDigitos + 1;
                        ListaHojaTrabajo.Add(objReporte);

                    }

                    while (pIntDigitos <= pIntNumeroDigitos)
                    {
                        objReporte = new ReporteHojaTrabajo();
                        string a = ReporteTotal.Where(x => x.cuenta.StartsWith(item.cuenta)).Count() != 0 ? ReporteTotal.Where(x => x.cuenta.StartsWith(item.cuenta)).FirstOrDefault().cuenta.Length >= pIntDigitos ? ReporteTotal.Where(x => x.cuenta.StartsWith(item.cuenta)).FirstOrDefault().cuenta.Substring(0, pIntDigitos) : "*No Existe*" : "*No Existe*";
                        if (a != "*No Existe*")
                        {
                            objReporte.cuenta = a;
                            var CuentaImputable = NombreCuentaImputable(a);
                            string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
                            int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
                            objReporte.denominacion = a != "*No Existe*" ? denominacion : "*No Existe*";
                            objReporte.debeSumaAcumulada = objReporte.cuenta != "*No Existe*" ? SumaCuentasReporteHojaTrabajo(ReporteTotal, objReporte.cuenta.Trim(), "D", pIntMoneda, pstrCuentasImputables == 1 ? true : false) : 0;
                            objReporte.haberSumaAcumulada = objReporte.cuenta != "*No Existe*" ? SumaCuentasReporteHojaTrabajo(ReporteTotal, objReporte.cuenta.Trim(), "H", pIntMoneda, pstrCuentasImputables == 1 ? true : false) : 0;
                            var Saldos = SaldosReporteHojaTrabajo(ReporteTotal, objReporte.cuenta, pIntMoneda, pstrCuentasImputables == 1 ? true : false, null);
                            objReporte.saldoDeudor = Saldos[0] > Saldos[1] ? Saldos[0] - Saldos[1] : 0;
                            objReporte.saldoAcreedor = Saldos[1] > Saldos[0] ? Saldos[1] - Saldos[0] : 0;
                            //objReporte.saldoDeudor = Saldos[0]; // se cambio nuevamente al anterior  6 de mayo
                            //objReporte.saldoAcreedor = Saldos[1];
                            objReporte.debeAjustes = objReporte.cuenta != "*No Existe*" ? CuentaAjuste(objReporte.cuenta, "S") ? objReporte.saldoAcreedor : 0 : 0;
                            objReporte.haberAjustes = objReporte.cuenta != "*No Existe*" ? CuentaAjuste(objReporte.cuenta, "S") ? objReporte.saldoDeudor : 0 : 0;

                            objReporte.activoInventario = objReporte.cuenta != "*No Existe*" ? int.Parse(objReporte.cuenta.Substring(0, 2)) <= 39 ? objReporte.saldoDeudor : 0 : 0;
                            objReporte.pasivoInventario = objReporte.cuenta != "*No Existe*" ? int.Parse(objReporte.cuenta.Substring(0, 2)) <= 39 ? objReporte.saldoAcreedor : 0 : 0;
                            objReporte.perdidaNaturaleza = objReporte.cuenta != "*No Existe*" ? CuentaPerdidaNaturaleza(objReporte.cuenta, "S") ? objReporte.saldoDeudor : 0 : 0;
                            objReporte.gananciaNaturaleza = objReporte.cuenta != "*No Existe*" ? CuentaPerdidaNaturaleza(objReporte.cuenta, "S") ? objReporte.saldoAcreedor : 0 : 0;
                            objReporte.perdidaFuncion = objReporte.cuenta != "*No Existe*" ? CuentaFuncion(objReporte.cuenta, "S") ? objReporte.saldoDeudor : 0 : 0;
                            objReporte.gananciaFuncion = objReporte.cuenta != "*No Existe*" ? CuentaFuncion(objReporte.cuenta, "S") ? objReporte.saldoAcreedor : 0 : 0;
                            objReporte.imputable = a != "*No Existe*" ? imputable : -1;

                            ListaHojaTrabajo.Add(objReporte);
                        }
                        pIntDigitos = pIntDigitos + 1;
                    }

                    int Contador = 1;
                    pIntDigitos = 2;

                    if (Contador < ListaEmpiezaCuenta.Count())
                    {
                        foreach (var subCuenta in ListaEmpiezaCuenta.AsParallel())
                        {


                            while (pIntDigitos <= pIntNumeroDigitos)
                            {
                                objReporte = new ReporteHojaTrabajo();
                                string a = subCuenta.cuenta.StartsWith(item.cuenta) && subCuenta.cuenta.Length >= pIntDigitos ? subCuenta.cuenta.Substring(0, pIntDigitos) : "*No Existe*";
                                objReporte.cuenta = a;
                                if (a != "*No Existe*" && ListaHojaTrabajo.Where(x => x.cuenta == objReporte.cuenta).Count() == 0)
                                {
                                    var CuentaImputable = NombreCuentaImputable(a);
                                    string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
                                    int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
                                    objReporte.denominacion = a != "*No Existe*" ? denominacion : "*No Existe*";
                                    objReporte.debeSumaAcumulada = objReporte.cuenta != "*No Existe*" ? SumaCuentasReporteHojaTrabajo(ReporteTotal, objReporte.cuenta.Trim(), "D", pIntMoneda, pstrCuentasImputables == 1 ? true : false) : 0;
                                    objReporte.haberSumaAcumulada = objReporte.cuenta != "*No Existe*" ? SumaCuentasReporteHojaTrabajo(ReporteTotal, objReporte.cuenta.Trim(), "H", pIntMoneda, pstrCuentasImputables == 1 ? true : false) : 0;
                                    var Saldos = SaldosReporteHojaTrabajo(ReporteTotal, objReporte.cuenta, pIntMoneda, pstrCuentasImputables == 1 ? true : false, null);

                                    objReporte.saldoDeudor = Saldos[0] > Saldos[1] ? Saldos[0] - Saldos[1] : 0;
                                    objReporte.saldoAcreedor = Saldos[1] > Saldos[0] ? Saldos[1] - Saldos[0] : 0;


                                    objReporte.debeAjustes = objReporte.cuenta != "*No Existe*" ? CuentaAjuste(objReporte.cuenta) ? objReporte.saldoAcreedor : 0 : 0;
                                    objReporte.haberAjustes = objReporte.cuenta != "*No Existe*" ? CuentaAjuste(objReporte.cuenta) ? objReporte.saldoDeudor : 0 : 0;
                                    objReporte.activoInventario = objReporte.cuenta != "*No Existe*" ? int.Parse(objReporte.cuenta.Substring(0, 2)) <= 39 ? objReporte.saldoDeudor : 0 : 0;
                                    objReporte.pasivoInventario = objReporte.cuenta != "*No Existe*" ? int.Parse(objReporte.cuenta.Substring(0, 2)) <= 39 ? objReporte.saldoAcreedor : 0 : 0;
                                    objReporte.perdidaNaturaleza = objReporte.cuenta != "*No Existe*" ? CuentaPerdidaNaturaleza(objReporte.cuenta, "S") ? objReporte.saldoDeudor : 0 : 0;
                                    objReporte.gananciaNaturaleza = objReporte.cuenta != "*No Existe*" ? CuentaPerdidaNaturaleza(objReporte.cuenta, "S") ? objReporte.saldoAcreedor : 0 : 0;
                                    objReporte.perdidaFuncion = objReporte.cuenta != "*No Existe*" ? CuentaFuncion(objReporte.cuenta, "S") ? objReporte.saldoDeudor : 0 : 0;
                                    objReporte.gananciaFuncion = objReporte.cuenta != "*No Existe*" ? CuentaFuncion(objReporte.cuenta, "S") ? objReporte.saldoAcreedor : 0 : 0;
                                    objReporte.imputable = a != "*No Existe*" ? imputable : -1;


                                    ListaHojaTrabajo.Add(objReporte);

                                }
                                pIntDigitos = pIntDigitos + 1;
                            }
                            pIntDigitos = 2;

                        }
                    }
                }
                objOperationResult.Success = 1;
                if (pstrCuentasImputables == 1)
                {
                    //return ListaHojaTrabajo.ToList().Where(y => y.imputable == 1  ).OrderBy(x => x.cuenta).ToList().Where(x => x.debeSumaAcumulada != 0 || x.haberSumaAcumulada != 0 || x.debeAjustes != 0 || x.haberAjustes != 0 || x.activoInventario != 0 || x.pasivoInventario != 0 || x.perdidaFuncion != 0 || x.gananciaFuncion != 0 || x.perdidaNaturaleza != 0 || x.gananciaNaturaleza != 0).ToList();


                    return ListaHojaTrabajo.ToList().Where(y => y.imputable == 1 || (y.cuenta.Length == 2 && y.imputable == 0)).OrderBy(x => x.cuenta).ToList().Where(x => x.debeSumaAcumulada != 0 || x.haberSumaAcumulada != 0 || x.debeAjustes != 0 || x.haberAjustes != 0 || x.activoInventario != 0 || x.pasivoInventario != 0 || x.perdidaFuncion != 0 || x.gananciaFuncion != 0 || x.perdidaNaturaleza != 0 || x.gananciaNaturaleza != 0).ToList(); //Se agregó 9 Mayo
                }
                else
                {

                    return ListaHojaTrabajo.ToList().OrderBy(x => x.cuenta).ToList().Where(x => x.debeSumaAcumulada != 0 || x.haberSumaAcumulada != 0 || x.debeAjustes != 0 || x.haberAjustes != 0 || x.activoInventario != 0 || x.pasivoInventario != 0 || x.perdidaFuncion != 0 || x.gananciaFuncion != 0 || x.perdidaNaturaleza != 0 || x.gananciaNaturaleza != 0).ToList(); ;
                }

            }
            catch (Exception e)
            {
                objOperationResult.Success = 0;

                return null;


            }

        }



        public decimal SumaCuentasReporteHojaTrabajo(List<ReporteHojaTrabajo> ListaHojaTrabajo, string NroCuenta, string pstrNaturaleza, int pIntMoneda, bool ReporteSoloImputables)
        {
            try
            {
                decimal Valor;
                //var Lista = ReporteSoloImputables ? ListaHojaTrabajo.FindAll(x => x.cuenta == NroCuenta).ToList() : ListaHojaTrabajo.FindAll(x => x.cuenta.StartsWith(NroCuenta)).ToList(); //Antes del 9 de Mayo
                var Lista = ListaHojaTrabajo.FindAll(x => x.cuenta.StartsWith(NroCuenta)).ToList();
                if (pstrNaturaleza == "D")
                {
                    return Valor = Lista.Count() != 0 ? pIntMoneda == 1 ? Lista.Sum(x => x.d_ImporteSolesD) : Lista.Sum(x => x.d_ImporteDolaresD) : 0;
                }
                else
                {
                    return Valor = Lista.Count() != 0 ? pIntMoneda == 1 ? Lista.Sum(x => x.d_ImporteSolesH) : Lista.Sum(x => x.d_ImporteDolaresH) : 0;
                }
            }
            catch (Exception e)
            {
                return 0;
            }


        }


        public bool CuentaPerdidaNaturaleza(string pstrCuenta, string TipoReporte = "N")
        {
            //De la 60-69 -- TODA la 70 menos 79
            try
            {

                if (TipoReporte == "N")
                {
                    int Longitud = pstrCuenta.Length;

                    if (pstrCuenta.StartsWith("6") || pstrCuenta.StartsWith("7"))
                    {
                        if (Longitud > 2)
                        {
                            if (pstrCuenta.StartsWith("79"))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (pstrCuenta.StartsWith("79"))
                            {
                                return false;
                            }

                        }

                        return true;
                    }
                    else if (pstrCuenta.StartsWith("8"))   // se agrego porque se debe considerar la cuenta 88 o 89
                    {
                        return pstrCuenta.StartsWith("89") || pstrCuenta.StartsWith("88") ? true : false;
                    }

                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (pstrCuenta.StartsWith("5") || pstrCuenta.StartsWith("4"))
                        return true;
                    else return false;


                }

            }
            catch (Exception e)
            {
                throw e;

            }

        }

        public List<decimal> SaldosReporteHojaTrabajo(List<ReporteHojaTrabajo> ListaTotal, string pstrCuenta, int pIntMoneda, bool ReporteImputables, List<ReporteHojaTrabajo> apertura = null)
        {
            try
            {
                List<decimal> ListaFinal = new List<decimal>();
                List<string> ListaCuentasImputables = new List<string>();
                List<ReporteHojaTrabajo> ListaImputables = new List<ReporteHojaTrabajo>();

                if (apertura != null)
                {
                    ListaImputables = ListaTotal.Concat(apertura).ToList().FindAll(x => x.cuenta.StartsWith(pstrCuenta)).ToList();
                }
                else
                {
                    ListaImputables = ListaTotal.FindAll(x => x.cuenta.StartsWith(pstrCuenta)).ToList(); //Se modificó  el 9 mayo
                }

                foreach (var item in ListaImputables)
                {
                    if (!ListaCuentasImputables.Contains(item.cuenta))
                    {
                        ListaCuentasImputables.Add(item.cuenta);
                    }
                }

                decimal acumuladaDebe = 0, acumuladaHaber = 0, saldoDeudor = 0, saldoAcreedor = 0, DeudorFinal = 0, AcreedorFnal = 0;

                foreach (var item in ListaCuentasImputables.AsParallel())
                {
                    if (pIntMoneda == 1)
                    {
                        if (apertura != null && apertura.Any())
                        {
                            acumuladaDebe = ListaTotal.FindAll(x => x.cuenta == item).Sum(x => x.d_ImporteSolesD) + apertura.FindAll(x => x.cuenta == item).Sum(x => x.d_ImporteSolesD); //cambio 6 de abril
                            acumuladaHaber = ListaTotal.FindAll(x => x.cuenta == item).Sum(x => x.d_ImporteSolesH) + apertura.FindAll(x => x.cuenta == item).Sum(x => x.d_ImporteSolesH);
                        }
                        else
                        {

                            acumuladaDebe = ListaTotal.FindAll(x => x.cuenta == item).Sum(x => x.d_ImporteSolesD); //cambio 6 de abril
                            acumuladaHaber = ListaTotal.FindAll(x => x.cuenta == item).Sum(x => x.d_ImporteSolesH);

                        }

                    }
                    else
                    {
                        if (apertura != null && apertura.Any())
                        {
                            acumuladaDebe = ListaTotal.FindAll(x => x.cuenta == item).Sum(x => x.d_ImporteDolaresD) + apertura.FindAll(x => x.cuenta == item).Sum(x => x.d_ImporteDolaresD);
                            acumuladaHaber = ListaTotal.FindAll(x => x.cuenta == item).Sum(x => x.d_ImporteDolaresH) + apertura.FindAll(x => x.cuenta == item).Sum(x => x.d_ImporteDolaresD);
                        }
                        else
                        {
                            acumuladaDebe = ListaTotal.FindAll(x => x.cuenta == item).Sum(x => x.d_ImporteDolaresD);
                            acumuladaHaber = ListaTotal.FindAll(x => x.cuenta == item).Sum(x => x.d_ImporteDolaresH);
                        }
                    }
                    saldoDeudor = acumuladaDebe > acumuladaHaber ? acumuladaDebe - acumuladaHaber : 0;
                    saldoAcreedor = acumuladaDebe < acumuladaHaber ? (acumuladaDebe - acumuladaHaber) * -1 : 0;
                    DeudorFinal = DeudorFinal + saldoDeudor;
                    AcreedorFnal = AcreedorFnal + saldoAcreedor;

                }

                ListaFinal.Add(DeudorFinal);
                ListaFinal.Add(AcreedorFnal);
                return ListaFinal;
            }
            catch (Exception e)
            {

                throw e;
            }


        }
        public decimal CuentaPerdidaNaturaleza61(string pstrCuenta, List<ReporteHojaTrabajo> ListaHojaT, int pIntMoneda, string pstrNaturaleza)
        {
            try
            {
                decimal SumaDebe, SumaHaber, saldoDeudor, saldoAcreedor, perdida, ganancia;
                if (pstrCuenta.Length > 2)
                {
                    string pstrComplemento = pstrCuenta.Substring(2, (pstrCuenta.Length - 2));
                    string pstrcuentaNueva = "69" + pstrComplemento;
                    SumaDebe = pIntMoneda == 1 ? ListaHojaT.FindAll(y => y.cuenta.Length >= pstrCuenta.Length).FindAll(x => x.cuenta.Substring(0, pstrCuenta.Length) == pstrcuentaNueva).ToList().Sum(y => y.d_ImporteSolesD) : ListaHojaT.FindAll(x => x.cuenta.Substring(0, pstrCuenta.Length) == pstrcuentaNueva).ToList().Sum(y => y.d_ImporteDolaresD);
                    SumaHaber = pIntMoneda == 1 ? ListaHojaT.FindAll(y => y.cuenta.Length >= pstrCuenta.Length).FindAll(x => x.cuenta.Substring(0, pstrCuenta.Length) == pstrcuentaNueva).ToList().Sum(y => y.d_ImporteSolesH) : ListaHojaT.FindAll(x => x.cuenta.Substring(0, pstrCuenta.Length) == pstrcuentaNueva).ToList().Sum(y => y.d_ImporteDolaresH);
                    saldoDeudor = SumaDebe > SumaHaber ? SumaDebe - SumaHaber : 0;
                    saldoAcreedor = SumaDebe < SumaHaber ? (SumaDebe - SumaHaber) * -1 : 0;
                    perdida = saldoDeudor;
                    ganancia = saldoAcreedor;
                }

                else
                {
                    SumaDebe = pIntMoneda == 1 ? ListaHojaT.FindAll(x => x.cuenta.Substring(0, 2) == "69").ToList().Sum(x => x.d_ImporteSolesD) : ListaHojaT.FindAll(x => x.cuenta.Substring(0, 2) == "69").ToList().Sum(x => x.d_ImporteDolaresD);
                    SumaHaber = pIntMoneda == 1 ? ListaHojaT.FindAll(x => x.cuenta.Substring(0, 2) == "69").ToList().Sum(x => x.d_ImporteSolesH) : ListaHojaT.FindAll(x => x.cuenta.Substring(0, 2) == "69").ToList().Sum(x => x.d_ImporteDolaresH);
                    saldoDeudor = SumaDebe > SumaHaber ? SumaDebe - SumaHaber : 0;
                    saldoAcreedor = SumaDebe < SumaHaber ? (SumaDebe - SumaHaber) * -1 : 0;
                    perdida = saldoDeudor;
                    ganancia = saldoAcreedor;

                }
                if (pstrNaturaleza == "P")
                {
                    return perdida;
                }
                else
                {
                    return ganancia;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public bool CuentaAjuste(string pstrCuenta, string TipoReporte = "N")
        {
            try
            {

                if (TipoReporte == "N")
                {
                    if (pstrCuenta.StartsWith("79") || pstrCuenta.StartsWith("9") || pstrCuenta.StartsWith("69") || pstrCuenta.StartsWith("61"))
                    {
                        return true;
                    }
                }
                else
                {
                    if (pstrCuenta.StartsWith("7") || pstrCuenta.StartsWith("9"))
                    {
                        return true;
                    }
                }
                return false;

            }
            catch (Exception e)
            {

                throw e;
            }




        }

        public decimal SumaCuentaAjuste61(string pstrCuenta, List<ReporteHojaTrabajo> ListaHojaTrabajo, int pIntMoneda, string Naturaleza)
        {
            try
            {
                decimal debeSuma = 0, haberSuma = 0, saldoDeudor = 0, saldoAcreedor = 0;
                if (pstrCuenta.Length > 2)
                {
                    string pstrcuentaNueva = "";
                    //if (pstrCuenta == "6111102")
                    //{
                    //    pstrcuentaNueva = "69" + "25101";
                    //}
                    //else
                    //{
                    string pstrComplemento = pstrCuenta.Substring(2, (pstrCuenta.Length - 2));
                    pstrcuentaNueva = "69" + pstrComplemento;
                    //}

                    debeSuma = pIntMoneda == 1 ? ListaHojaTrabajo.FindAll(y => y.cuenta.Length >= pstrCuenta.Length).FindAll(x => x.cuenta.Substring(0, pstrCuenta.Length) == pstrcuentaNueva).ToList().Sum(y => y.d_ImporteSolesD) : ListaHojaTrabajo.FindAll(y => y.cuenta.Length >= pstrCuenta.Length).FindAll(x => x.cuenta.Substring(0, pstrCuenta.Length) == pstrcuentaNueva).ToList().Sum(y => y.d_ImporteDolaresD);  // ListaHojaTrabajo.FindAll(x => x.cuenta.Substring(0, pstrCuenta.Length) == pstrcuentaNueva).ToList().Sum(y => y.d_ImporteDolaresD);
                    haberSuma = pIntMoneda == 1 ? ListaHojaTrabajo.FindAll(y => y.cuenta.Length >= pstrCuenta.Length).FindAll(x => x.cuenta.Substring(0, pstrCuenta.Length) == pstrcuentaNueva).ToList().Sum(y => y.d_ImporteSolesH) : ListaHojaTrabajo.FindAll(y => y.cuenta.Length >= pstrCuenta.Length).FindAll(x => x.cuenta.Substring(0, pstrCuenta.Length) == pstrcuentaNueva).ToList().Sum(y => y.d_ImporteDolaresH);                       // ListaHojaTrabajo.FindAll(x => x.cuenta.Substring(0, pstrCuenta.Length) == pstrcuentaNueva).ToList().Sum(y => y.d_ImporteDolaresH);
                    saldoDeudor = debeSuma > haberSuma ? debeSuma - haberSuma : 0;
                    saldoAcreedor = debeSuma < haberSuma ? (debeSuma - haberSuma) * -1 : 0;

                }

                else
                {

                    debeSuma = pIntMoneda == 1 ? ListaHojaTrabajo.FindAll(x => x.cuenta.Substring(0, 2) == ("69")).ToList().Sum(y => y.d_ImporteSolesD) : ListaHojaTrabajo.FindAll(x => x.cuenta.Substring(0, 2) == ("69")).ToList().Sum(y => y.d_ImporteDolaresD);
                    haberSuma = pIntMoneda == 1 ? ListaHojaTrabajo.FindAll(x => x.cuenta.Substring(0, 2) == ("69")).ToList().Sum(y => y.d_ImporteSolesH) : ListaHojaTrabajo.FindAll(x => x.cuenta.Substring(0, 2) == ("69")).ToList().Sum(y => y.d_ImporteDolaresH);
                    saldoDeudor = debeSuma > haberSuma ? debeSuma - haberSuma : 0; //D
                    saldoAcreedor = debeSuma < haberSuma ? (debeSuma - haberSuma) * -1 : 0; //H
                }

                if (saldoDeudor != 0) //Pasara como Haber la C. 69
                {
                    if (Naturaleza == "D")
                    {

                        return saldoDeudor;

                    }

                }

                if (saldoAcreedor != 0)// Pasara como Debe la C. 69
                {
                    if (Naturaleza == "H")
                    {
                        return saldoAcreedor;
                    }
                }

                return 0;
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public bool CuentaFuncion(string pstrCuenta, string TipoReport = "N")
        {

            //la cuenta 69 , la cuenta 88 ,89, toda la 7  (menos 79 ni 71 ), toda la 9
            if (TipoReport == "N")
            {
                if (pstrCuenta.StartsWith("69") || pstrCuenta.StartsWith("7") || pstrCuenta.StartsWith("9") || pstrCuenta.StartsWith("88") || pstrCuenta.StartsWith("89"))
                {
                    if (pstrCuenta.StartsWith("79") || pstrCuenta.StartsWith("71")) // se agregò la 71|| pstrCuenta.StartsWith("72") || pstrCuenta.StartsWith("78"))
                    {
                        return false;
                    }
                    else
                    {

                        return true;
                    }

                }

                return false;
            }
            else
            {
                if (pstrCuenta.StartsWith("9") || pstrCuenta.StartsWith("5"))
                    return true;
                else return false;
            }


        }

        //public List<ReporteBalanceMensual> ReporteBalanceMensual(ref OperationResult objOperationResult, string pstrPeriodo, string pstrMes, int pIntNumeroDigitos, int pIntMoneda, int pIntCuentasImputables, int pIntCuentasGastos79)
        //{

        //    try
        //    {

        //        SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
        //        List<ReporteBalanceMensual> ListaFinal = new List<ReporteBalanceMensual>();
        //        string MesAnterior = pstrMes != "01" ? (int.Parse(pstrMes) - 1).ToString("00") : pstrMes;

        //        #region RecopilaCuentasExistentesMesAnterior
        //        // Si es Enero se trae las cuentas Mayores de Asientos Apertura 
        //        // De lo contrario se trae las Cuentas Mayores de la Tabla Saldo Contables 
        //        //Cuentas que aparecen en Reporte son del Mes Anterior + Mes Requerido
        //        List<ReporteBalanceMensual> AsientoApertura = new List<ReporteBalanceMensual>();
        //        List<ReporteBalanceMensual> CuentasAnteriores = new List<ReporteBalanceMensual>();
        //        List<ReporteBalanceMensual> CuentasAnterioresMes = new List<ReporteBalanceMensual>();
        //        ReporteBalanceMensual objReporte = new ReporteBalanceMensual();
        //        List<string> CuentasMayores = new List<string>();

        //        if (int.Parse(pstrMes) == (int)Mes.Enero)
        //        {
        //            AsientoApertura = (from a in dbContext.diario

        //                               join b in dbContext.diariodetalle on new { IdDiario = a.v_IdDiario, eliminado = 0 }
        //                                                                equals new { IdDiario = b.v_IdDiario, eliminado = b.i_Eliminado.Value } into b_join
        //                               from b in b_join.DefaultIfEmpty()

        //                               join c in dbContext.asientocontable on new { asiento = b.v_NroCuenta, eliminado = 0, p = periodo }
        //                                                                equals new { asiento = c.v_NroCuenta, eliminado = c.i_Eliminado.Value, p = c.v_Periodo } into c_join

        //                               from c in c_join.DefaultIfEmpty()

        //                               where a.v_Periodo == pstrPeriodo && a.i_IdTipoComprobante == 1 && a.i_Eliminado == 0 && a.v_Mes == pstrMes && (b.v_NroCuenta != null || b.v_NroCuenta != "")

        //                               orderby b.v_NroCuenta
        //                               select new ReporteBalanceMensual
        //                               {

        //                                   pstrCuenta = b.v_NroCuenta.Trim(),
        //                                   pstrDenominacion = c == null ? "*No Existe*" : c.v_NombreCuenta.Trim(),
        //                                   mes = a.v_Mes,
        //                                   imputable = c == null ? -1 : c.i_Imputable.Value,
        //                                   ImporteDebeSoles = a.i_IdMoneda == 1 && b.v_Naturaleza == "D" ? b.d_Importe.Value : a.i_IdMoneda == 2 && b.v_Naturaleza == "D" ? b.d_Cambio.Value : 0,
        //                                   ImporteDebeDolares = a.i_IdMoneda == 2 && b.v_Naturaleza == "D" ? b.d_Importe.Value : a.i_IdMoneda == 1 && b.v_Naturaleza == "D" ? b.d_Cambio.Value : 0,
        //                                   ImporteHaberSoles = a.i_IdMoneda == 1 && b.v_Naturaleza == "H" ? b.d_Importe.Value : a.i_IdMoneda == 2 && b.v_Naturaleza == "H" ? b.d_Cambio.Value : 0,
        //                                   ImporteHaberDolares = a.i_IdMoneda == 2 && b.v_Naturaleza == "H" ? b.d_Importe.Value : a.i_IdMoneda == 1 && b.v_Naturaleza == "H" ? b.d_Cambio.Value : 0,

        //                               }).ToList();

        //            CuentasAnteriores = AsientoApertura.ToList();

        //        }
        //        else
        //        {
        //            CuentasAnterioresMes = (from a in dbContext.saldoscontables

        //                                    join b in dbContext.asientocontable on new { cuenta = a.v_NroCuenta, eliminado = 0, p = periodo }
        //                                                                        equals new { cuenta = b.v_NroCuenta, eliminado = b.i_Eliminado.Value, p = b.v_Periodo } into b_join
        //                                    from b in b_join.DefaultIfEmpty()

        //                                    where a.v_Anio == pstrPeriodo && (a.v_NroCuenta != null || a.v_NroCuenta != "")

        //                                    orderby a.v_NroCuenta

        //                                    select new ReporteBalanceMensual
        //                                    {
        //                                        pstrCuenta = a.v_NroCuenta.Trim(),
        //                                        pstrDenominacion = b == null ? "*No Existe*" : b.v_NombreCuenta.Trim(),
        //                                        mes = a.v_Mes,
        //                                        imputable = b == null ? -1 : b.i_Imputable.Value,
        //                                        ImporteDebeSoles = a.d_ImporteSolesD.Value,
        //                                        ImporteDebeDolares = a.d_ImporteDolaresD.Value,
        //                                        ImporteHaberSoles = a.d_ImporteSolesH.Value,
        //                                        ImporteHaberDolares = a.d_ImporteDolaresH.Value,

        //                                    }).ToList();




        //            CuentasAnteriores = (from a in CuentasAnterioresMes

        //                                 where int.Parse(a.mes) <= int.Parse(MesAnterior)
        //                                 select new ReporteBalanceMensual
        //                                 {
        //                                     pstrCuenta = a.pstrCuenta,
        //                                     pstrDenominacion = a.pstrDenominacion,
        //                                     mes = a.mes,
        //                                     imputable = a.imputable,
        //                                     ImporteDebeSoles = a.ImporteDebeSoles,
        //                                     ImporteDebeDolares = a.ImporteDebeDolares,
        //                                     ImporteHaberSoles = a.ImporteHaberSoles,
        //                                     ImporteHaberDolares = a.ImporteHaberDolares,

        //                                 }).ToList();


        //        }
        //        #endregion

        //        #region RecopilaCuentasExistentesMesRequerido

        //        var CuentasMesRequerido = (from a in dbContext.saldoscontables

        //                                   join b in dbContext.asientocontable on new { cuenta = a.v_NroCuenta, eliminado = 0, p = periodo }
        //                                                                         equals new { cuenta = b.v_NroCuenta, eliminado = 0, p = b.v_Periodo } into b_join
        //                                   from b in b_join.DefaultIfEmpty()

        //                                   where a.v_Mes == pstrMes && a.v_Anio == pstrPeriodo

        //                                   orderby b.v_NroCuenta
        //                                   select new ReporteBalanceMensual
        //                                   {
        //                                       pstrCuenta = a.v_NroCuenta.Trim(),
        //                                       pstrDenominacion = b == null ? "*No Existe*" : b.v_NombreCuenta.Trim(),
        //                                       mes = a.v_Mes,
        //                                       imputable = b == null ? -1 : b.i_Imputable.Value,
        //                                       ImporteDebeSoles = a.d_ImporteSolesD.Value,
        //                                       ImporteDebeDolares = a.d_ImporteDolaresD.Value,
        //                                       ImporteHaberSoles = a.d_ImporteSolesH.Value,
        //                                       ImporteHaberDolares = a.d_ImporteDolaresH.Value,
        //                                   }).ToList();


        //        #endregion
        //        // var CuentasTotales = CuentasMesRequerido.Union(CuentasAnteriores).ToList();

        //        var CuentasTotales = CuentasMesRequerido.Concat(CuentasAnteriores).ToList();
        //        foreach (var item in CuentasTotales)
        //        {
        //            string NumCuenta = item.pstrCuenta.Substring(0, 2);
        //            if (!CuentasMayores.Contains(NumCuenta))
        //            {
        //                CuentasMayores.Add(NumCuenta);
        //            }
        //        }
        //        foreach (var item in CuentasMayores.AsParallel())
        //        {
        //            objReporte = new ReporteBalanceMensual();

        //            int pIntDigitos = 2;
        //            var ListaEmpiezaCuenta = CuentasTotales.Where(x => x.pstrCuenta.StartsWith(item.Substring(0, 2))).Select(x => x.pstrCuenta);
        //            while (pIntDigitos <= pIntNumeroDigitos)
        //            {

        //                objReporte = new ReporteBalanceMensual();
        //                objReporte.pstrCuenta = CuentasTotales.Where(x => x.pstrCuenta.StartsWith(item)).Count() != 0 ? CuentasTotales.Where(x => x.pstrCuenta.StartsWith(item)).FirstOrDefault().pstrCuenta.Length >= pIntDigitos ? CuentasTotales.Where(x => x.pstrCuenta.StartsWith(item)).FirstOrDefault().pstrCuenta.Substring(0, pIntDigitos) : "*No Existe*" : "*No Existe*";

        //                if (objReporte.pstrCuenta != "*No Existe*")
        //                {
        //                    var CuentaImputable = NombreCuentaImputable(objReporte.pstrCuenta);
        //                    string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
        //                    int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
        //                    objReporte.pstrDenominacion = objReporte.pstrCuenta != "No Existe*" ? denominacion : "*No Existe*";
        //                    objReporte.debeSaldoAnterior = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasAnteriores, objReporte.pstrCuenta, "D", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;
        //                    objReporte.haberSaldoAnterior = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasAnteriores, objReporte.pstrCuenta, "H", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;
        //                    if (int.Parse(pstrMes) == (int)Mes.Enero)
        //                    {
        //                        objReporte.debeMovimientoMes = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasMesRequerido, objReporte.pstrCuenta, "D", pIntMoneda, pIntCuentasImputables == 1 ? true : false) - SumasDiversasCuentas(CuentasAnteriores, objReporte.pstrCuenta, "D", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;
        //                        objReporte.haberMovimientoMes = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasMesRequerido, objReporte.pstrCuenta, "H", pIntMoneda, pIntCuentasImputables == 1 ? true : false) - SumasDiversasCuentas(CuentasAnteriores, objReporte.pstrCuenta, "H", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;
        //                    }
        //                    else
        //                    {
        //                        objReporte.debeMovimientoMes = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasMesRequerido, objReporte.pstrCuenta, "D", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;
        //                        objReporte.haberMovimientoMes = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasMesRequerido, objReporte.pstrCuenta, "H", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;
        //                    }
        //                    objReporte.debeSumaAcumulada = objReporte.debeSaldoAnterior + objReporte.debeMovimientoMes;
        //                    objReporte.haberSumaAcumulada = objReporte.haberSaldoAnterior + objReporte.haberMovimientoMes;
        //                    var Saldos = CalcularSaldosActuales(CuentasAnteriores, CuentasMesRequerido, objReporte.pstrCuenta, pIntMoneda, CuentasTotales, pIntCuentasImputables == 1 ? true : false);


        //                    objReporte.deudorSaldosActuales = Saldos[0] > Saldos[1] ? Saldos[0] - Saldos[1] : 0;// Antes del 20 de marzo
        //                    objReporte.acreedorSaldosActuales = Saldos[1] > Saldos[0] ? Saldos[1] - Saldos[0] : 0;

        //                    //objReporte.deudorSaldosActuales = Saldos[0];
        //                    //objReporte.acreedorSaldosActuales = Saldos[1];


        //                    objReporte.imputable = objReporte.pstrCuenta != "*No Existe*" ? imputable : -1;
        //                    if (pIntCuentasGastos79 == 1)
        //                    {
        //                        objReporte.Grupo2 = objReporte.pstrCuenta.StartsWith("9") || objReporte.pstrCuenta.StartsWith("79") ? "Grupo2" : "";
        //                        objReporte.Grupo1 = !objReporte.pstrCuenta.StartsWith("9") && !objReporte.pstrCuenta.StartsWith("79") ? "Grupo1" : "";
        //                    }
        //                    else
        //                    {
        //                        objReporte.Grupo2 = objReporte.pstrCuenta.StartsWith("9") ? "Grupo2" : "";
        //                        objReporte.Grupo1 = !objReporte.pstrCuenta.StartsWith("9") ? "Grupo1" : "";
        //                    }



        //                    ListaFinal.Add(objReporte);
        //                }
        //                pIntDigitos = pIntDigitos + 1;
        //            }

        //            int Contador = 1;
        //            pIntDigitos = 2;
        //            if (Contador < ListaEmpiezaCuenta.Count())
        //            {
        //                foreach (var subCuenta in ListaEmpiezaCuenta.AsParallel())
        //                {
        //                    while (pIntDigitos <= pIntNumeroDigitos)
        //                    {
        //                        objReporte = new ReporteBalanceMensual();
        //                        objReporte.pstrCuenta = subCuenta.StartsWith(item) && subCuenta.Length >= pIntDigitos ? subCuenta.Substring(0, pIntDigitos) : "*No Existe*";
        //                        if (objReporte.pstrCuenta != "*No Existe*" && ListaFinal.Where(x => x.pstrCuenta == objReporte.pstrCuenta).Count() == 0)
        //                        {
        //                            var CuentaImputable = NombreCuentaImputable(objReporte.pstrCuenta);
        //                            string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
        //                            int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
        //                            objReporte.pstrDenominacion = objReporte.pstrCuenta != "No Existe*" ? denominacion : "*No Existe*";
        //                            objReporte.debeSaldoAnterior = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasAnteriores, objReporte.pstrCuenta, "D", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;
        //                            objReporte.haberSaldoAnterior = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasAnteriores, objReporte.pstrCuenta, "H", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;
        //                            if (int.Parse(pstrMes) == (int)Mes.Enero)
        //                            {
        //                                objReporte.debeMovimientoMes = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasMesRequerido, objReporte.pstrCuenta, "D", pIntMoneda, pIntCuentasImputables == 1 ? true : false) - SumasDiversasCuentas(CuentasAnteriores, objReporte.pstrCuenta, "D", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;
        //                                objReporte.haberMovimientoMes = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasMesRequerido, objReporte.pstrCuenta, "H", pIntMoneda, pIntCuentasImputables == 1 ? true : false) - SumasDiversasCuentas(CuentasAnteriores, objReporte.pstrCuenta, "H", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;
        //                            }
        //                            else
        //                            {
        //                                objReporte.debeMovimientoMes = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasMesRequerido, objReporte.pstrCuenta, "D", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;
        //                                objReporte.haberMovimientoMes = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasMesRequerido, objReporte.pstrCuenta, "H", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;
        //                            }
        //                            objReporte.debeSumaAcumulada = objReporte.debeSaldoAnterior + objReporte.debeMovimientoMes;
        //                            objReporte.haberSumaAcumulada = objReporte.haberSaldoAnterior + objReporte.haberMovimientoMes;

        //                            var Saldos = CalcularSaldosActuales(CuentasAnteriores, CuentasMesRequerido, objReporte.pstrCuenta, pIntMoneda, CuentasTotales, pIntCuentasImputables == 1 ? true : false);
        //                            objReporte.deudorSaldosActuales = Saldos[0] > Saldos[1] ? Saldos[0] - Saldos[1] : 0;// Antes del 20 de marzo
        //                            objReporte.acreedorSaldosActuales = Saldos[1] > Saldos[0] ? Saldos[1] - Saldos[0] : 0;

        //                            //objReporte.deudorSaldosActuales = Saldos[0];
        //                            //objReporte.acreedorSaldosActuales = Saldos[1];


        //                            objReporte.imputable = objReporte.pstrCuenta != "*No Existe*" ? imputable : -1;
        //                            if (pIntCuentasGastos79 == 1)
        //                            {
        //                                objReporte.Grupo2 = objReporte.pstrCuenta.StartsWith("9") || objReporte.pstrCuenta.StartsWith("79") ? "Grupo2" : "";
        //                                objReporte.Grupo1 = !objReporte.pstrCuenta.StartsWith("9") && !objReporte.pstrCuenta.StartsWith("79") ? "Grupo1" : "";
        //                            }
        //                            else
        //                            {
        //                                objReporte.Grupo2 = objReporte.pstrCuenta.StartsWith("9") ? "Grupo2" : "";
        //                                objReporte.Grupo1 = !objReporte.pstrCuenta.StartsWith("9") ? "Grupo1" : "";

        //                            }

        //                            ListaFinal.Add(objReporte);

        //                        }
        //                        pIntDigitos = pIntDigitos + 1;
        //                    }
        //                    pIntDigitos = 2;
        //                }
        //            }
        //        }

        //        objOperationResult.Success = 1;
        //        if (pIntCuentasImputables == 1)
        //        {

        //            // return ListaFinal.ToList().Where(y => y.imputable == 1).OrderBy(x => x.pstrCuenta).ToList().Where(x => x.debeSaldoAnterior != 0 || x.haberSaldoAnterior != 0 || x.debeMovimientoMes != 0 || x.haberMovimientoMes != 0 || x.debeSumaAcumulada != 0 || x.haberSumaAcumulada != 0 || x.deudorSaldosActuales != 0 || x.acreedorSaldosActuales != 0).ToList();//Antes del 9 Mayo

        //            return ListaFinal.ToList().Where(y => y.imputable == 1 || (y.imputable == 0 && y.pstrCuenta.Length == 2)).OrderBy(x => x.pstrCuenta).ToList().Where(x => x.debeSaldoAnterior != 0 || x.haberSaldoAnterior != 0 || x.debeMovimientoMes != 0 || x.haberMovimientoMes != 0 || x.debeSumaAcumulada != 0 || x.haberSumaAcumulada != 0 || x.deudorSaldosActuales != 0 || x.acreedorSaldosActuales != 0).ToList();
        //        }
        //        else
        //        {


        //            return ListaFinal.OrderBy(x => x.pstrCuenta).ToList().Where(x => x.debeSaldoAnterior != 0 || x.haberSaldoAnterior != 0 || x.debeMovimientoMes != 0 || x.haberMovimientoMes != 0 || x.debeSumaAcumulada != 0 || x.haberSumaAcumulada != 0 || x.deudorSaldosActuales != 0 || x.acreedorSaldosActuales != 0).ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        objOperationResult.Success = 0;

        //        return null;
        //    }

        //}

        public List<ReporteBalanceMensual> ReporteBalanceMensualNuevo(ref OperationResult objOperationResult, string pstrPeriodo, string pstrMes, int pIntNumeroDigitos, int pIntMoneda, int pIntCuentasImputables, int pIntCuentasGastos79)
        {

            try
            {

                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                List<ReporteBalanceMensual> ListaFinal = new List<ReporteBalanceMensual>();
                string MesAnterior = pstrMes != "01" ? (int.Parse(pstrMes) - 1).ToString("00") : pstrMes;

                #region RecopilaCuentasExistentesMesAnterior
                // Si es Enero se trae las cuentas Mayores de Asientos Apertura 
                // De lo contrario se trae las Cuentas Mayores de la Tabla Saldo Contables 
                //Cuentas que aparecen en Reporte son del Mes Anterior + Mes Requerido
                List<ReporteBalanceMensual> AsientoApertura = new List<ReporteBalanceMensual>();
                List<ReporteBalanceMensual> CuentasAnteriores = new List<ReporteBalanceMensual>();
                List<ReporteBalanceMensual> CuentasAnterioresMes = new List<ReporteBalanceMensual>();
                ReporteBalanceMensual objReporte = new ReporteBalanceMensual();
                List<string> CuentasMayores = new List<string>();
                AsientosContablesBL _objAsientoContableBL = new AsientosContablesBL();

                if (int.Parse(pstrMes) == (int)Mes.Enero)
                {
                    AsientoApertura = (from a in dbContext.diario

                                       join b in dbContext.diariodetalle on new { IdDiario = a.v_IdDiario, eliminado = 0 }
                                                                        equals new { IdDiario = b.v_IdDiario, eliminado = b.i_Eliminado.Value } into b_join
                                       from b in b_join.DefaultIfEmpty()

                                       join c in dbContext.asientocontable on new { asiento = b.v_NroCuenta, eliminado = 0, p = periodo }
                                                                        equals new { asiento = c.v_NroCuenta, eliminado = c.i_Eliminado.Value, p = c.v_Periodo } into c_join

                                       from c in c_join.DefaultIfEmpty()

                                       where a.v_Periodo == pstrPeriodo && a.i_IdTipoComprobante == 1 && a.i_Eliminado == 0 && a.v_Mes == pstrMes && (b.v_NroCuenta != null && b.v_NroCuenta != "")

                                       orderby b.v_NroCuenta
                                       select new ReporteBalanceMensual
                                       {

                                           pstrCuenta = b.v_NroCuenta.Trim(),
                                           pstrDenominacion = c == null ? "*No Existe*" : c.v_NombreCuenta.Trim(),
                                           mes = a.v_Mes,
                                           imputable = c == null ? -1 : c.i_Imputable.Value,
                                           ImporteDebeSoles = a.i_IdMoneda == 1 && b.v_Naturaleza == "D" ? b.d_Importe.Value : a.i_IdMoneda == 2 && b.v_Naturaleza == "D" ? b.d_Cambio.Value : 0,
                                           ImporteDebeDolares = a.i_IdMoneda == 2 && b.v_Naturaleza == "D" ? b.d_Importe.Value : a.i_IdMoneda == 1 && b.v_Naturaleza == "D" ? b.d_Cambio.Value : 0,
                                           ImporteHaberSoles = a.i_IdMoneda == 1 && b.v_Naturaleza == "H" ? b.d_Importe.Value : a.i_IdMoneda == 2 && b.v_Naturaleza == "H" ? b.d_Cambio.Value : 0,
                                           ImporteHaberDolares = a.i_IdMoneda == 2 && b.v_Naturaleza == "H" ? b.d_Importe.Value : a.i_IdMoneda == 1 && b.v_Naturaleza == "H" ? b.d_Cambio.Value : 0,

                                       }).ToList();

                    CuentasAnteriores = AsientoApertura.ToList();

                }
                else
                {
                    var saldoscontablesAnteriores = _objAsientoContableBL.ObtenerSaldosContablesPorMes(ref  objOperationResult, int.Parse(MesAnterior), int.Parse(pstrPeriodo));

                    CuentasAnteriores = (from a in saldoscontablesAnteriores

                                         join b in dbContext.asientocontable on new { cuenta = a.v_NroCuenta, eliminado = 0, p = periodo }
                                                                             equals new { cuenta = b.v_NroCuenta, eliminado = b.i_Eliminado.Value, p = b.v_Periodo } into b_join
                                         from b in b_join.DefaultIfEmpty()

                                         where (a.v_NroCuenta != null && a.v_NroCuenta != "" && a.v_NroCuenta != "*Cta Eliminada*")

                                         orderby a.v_NroCuenta

                                         select new ReporteBalanceMensual
                                         {
                                             pstrCuenta = a.v_NroCuenta.Trim(),
                                             pstrDenominacion = b == null ? "*No Existe*" : b.v_NombreCuenta.Trim(),
                                             mes = a.v_Mes,
                                             imputable = b == null ? -1 : b.i_Imputable.Value,
                                             ImporteDebeSoles = a.d_ImporteSolesD.Value,
                                             ImporteDebeDolares = a.d_ImporteDolaresD.Value,
                                             ImporteHaberSoles = a.d_ImporteSolesH.Value,
                                             ImporteHaberDolares = a.d_ImporteDolaresH.Value,

                                         }).ToList();







                }
                #endregion

                #region RecopilaCuentasExistentesMesRequerido

                var saldoscontables = _objAsientoContableBL.ObtenerSaldosContablesPorMes(ref  objOperationResult, int.Parse(pstrMes), int.Parse(pstrPeriodo), int.Parse(pstrMes));

                var CuentasMesRequerido = (from a in saldoscontables

                                           join b in dbContext.asientocontable on new { cuenta = a.v_NroCuenta, eliminado = 0, p = periodo }
                                                                                 equals new { cuenta = b.v_NroCuenta, eliminado = b.i_Eliminado.Value, p = b.v_Periodo } into b_join
                                           from b in b_join.DefaultIfEmpty()

                                           where (a.v_NroCuenta != null && a.v_NroCuenta != "" && a.v_NroCuenta != "*Cta Eliminada*")
                                           orderby b.v_NroCuenta
                                           select new ReporteBalanceMensual
                                           {
                                               pstrCuenta = a.v_NroCuenta.Trim(),
                                               pstrDenominacion = b == null ? "*No Existe*" : b.v_NombreCuenta.Trim(),
                                               mes = a.v_Mes,
                                               imputable = b == null ? -1 : b.i_Imputable.Value,
                                               ImporteDebeSoles = a.d_ImporteSolesD.Value,
                                               ImporteDebeDolares = a.d_ImporteDolaresD.Value,
                                               ImporteHaberSoles = a.d_ImporteSolesH.Value,
                                               ImporteHaberDolares = a.d_ImporteDolaresH.Value,
                                           }).ToList();
                var g = CuentasAnteriores.Where(o => o.pstrCuenta.StartsWith("38")).ToList();

                var gg = CuentasMesRequerido.Where(l => l.pstrCuenta.StartsWith("38")).ToList();

                #endregion
                // var CuentasTotales = CuentasMesRequerido.Union(CuentasAnteriores).ToList();

                var CuentasTotales = CuentasMesRequerido.Concat(CuentasAnteriores).ToList();
                foreach (var item in CuentasTotales)
                {
                    string NumCuenta = item.pstrCuenta.Substring(0, 2);
                    if (NumCuenta == "38")
                    {
                        string j = "";
                    }
                    if (!CuentasMayores.Contains(NumCuenta))
                    {
                        CuentasMayores.Add(NumCuenta);
                    }
                }
                foreach (var item in CuentasMayores.AsParallel())
                {
                    objReporte = new ReporteBalanceMensual();

                    int pIntDigitos = 2;
                    var ListaEmpiezaCuenta = CuentasTotales.Where(x => x.pstrCuenta.StartsWith(item.Substring(0, 2))).Select(x => x.pstrCuenta);
                    while (pIntDigitos <= pIntNumeroDigitos)
                    {

                        objReporte = new ReporteBalanceMensual();
                        objReporte.pstrCuenta = CuentasTotales.Where(x => x.pstrCuenta.StartsWith(item)).Count() != 0 ? CuentasTotales.Where(x => x.pstrCuenta.StartsWith(item)).FirstOrDefault().pstrCuenta.Length >= pIntDigitos ? CuentasTotales.Where(x => x.pstrCuenta.StartsWith(item)).FirstOrDefault().pstrCuenta.Substring(0, pIntDigitos) : "*No Existe*" : "*No Existe*";

                        if (objReporte.pstrCuenta != "*No Existe*")
                        {
                            var CuentaImputable = NombreCuentaImputable(objReporte.pstrCuenta);
                            string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
                            int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
                            objReporte.pstrDenominacion = objReporte.pstrCuenta != "No Existe*" ? denominacion : "*No Existe*";
                            var DebeAnterior = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasAnteriores, objReporte.pstrCuenta, "D", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;
                            var HaberAnterior = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasAnteriores, objReporte.pstrCuenta, "H", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;
                            objReporte.debeSaldoAnterior = DebeAnterior > HaberAnterior ? DebeAnterior - HaberAnterior : 0;
                            objReporte.haberSaldoAnterior = HaberAnterior > DebeAnterior ? HaberAnterior - DebeAnterior : 0;


                            if (int.Parse(pstrMes) == (int)Mes.Enero)
                            {
                                objReporte.debeMovimientoMes = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasMesRequerido, objReporte.pstrCuenta, "D", pIntMoneda, pIntCuentasImputables == 1 ? true : false) - SumasDiversasCuentas(CuentasAnteriores, objReporte.pstrCuenta, "D", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;
                                objReporte.haberMovimientoMes = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasMesRequerido, objReporte.pstrCuenta, "H", pIntMoneda, pIntCuentasImputables == 1 ? true : false) - SumasDiversasCuentas(CuentasAnteriores, objReporte.pstrCuenta, "H", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;
                            }
                            else
                            {
                                objReporte.debeMovimientoMes = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasMesRequerido, objReporte.pstrCuenta, "D", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;
                                objReporte.haberMovimientoMes = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasMesRequerido, objReporte.pstrCuenta, "H", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;
                            }
                            objReporte.debeSumaAcumulada = objReporte.debeSaldoAnterior + objReporte.debeMovimientoMes;
                            objReporte.haberSumaAcumulada = objReporte.haberSaldoAnterior + objReporte.haberMovimientoMes;
                            //var Saldos = CalcularSaldosActuales(CuentasAnteriores, CuentasMesRequerido, objReporte.pstrCuenta, pIntMoneda, CuentasTotales, pIntCuentasImputables == 1 ? true : false);


                            objReporte.deudorSaldosActuales = objReporte.debeSumaAcumulada > objReporte.haberSumaAcumulada ? objReporte.debeSumaAcumulada - objReporte.haberSumaAcumulada : 0;  //   Saldos[0] > Saldos[1] ? Saldos[0] - Saldos[1] : 0;// Antes del 20 de marzo
                            objReporte.acreedorSaldosActuales = objReporte.haberSumaAcumulada > objReporte.debeSumaAcumulada ? objReporte.haberSumaAcumulada - objReporte.debeSumaAcumulada : 0;                          //Saldos[1] > Saldos[0] ? Saldos[1] - Saldos[0] : 0;

                            //objReporte.deudorSaldosActuales = Saldos[0];
                            //objReporte.acreedorSaldosActuales = Saldos[1];


                            objReporte.imputable = objReporte.pstrCuenta != "*No Existe*" ? imputable : -1;
                            if (pIntCuentasGastos79 == 1)
                            {
                                objReporte.Grupo2 = objReporte.pstrCuenta.StartsWith("9") || objReporte.pstrCuenta.StartsWith("79") ? "Grupo2" : "";
                                objReporte.Grupo1 = !objReporte.pstrCuenta.StartsWith("9") && !objReporte.pstrCuenta.StartsWith("79") ? "Grupo1" : "";
                            }
                            else
                            {
                                objReporte.Grupo2 = objReporte.pstrCuenta.StartsWith("9") ? "Grupo2" : "";
                                objReporte.Grupo1 = !objReporte.pstrCuenta.StartsWith("9") ? "Grupo1" : "";
                            }



                            ListaFinal.Add(objReporte);
                        }
                        pIntDigitos = pIntDigitos + 1;
                    }

                    int Contador = 1;
                    pIntDigitos = 2;
                    if (Contador < ListaEmpiezaCuenta.Count())
                    {
                        foreach (var subCuenta in ListaEmpiezaCuenta.AsParallel())
                        {
                            while (pIntDigitos <= pIntNumeroDigitos)
                            {
                                objReporte = new ReporteBalanceMensual();
                                objReporte.pstrCuenta = subCuenta.StartsWith(item) && subCuenta.Length >= pIntDigitos ? subCuenta.Substring(0, pIntDigitos) : "*No Existe*";
                                if (objReporte.pstrCuenta != "*No Existe*" && ListaFinal.Where(x => x.pstrCuenta == objReporte.pstrCuenta).Count() == 0)
                                {
                                    var CuentaImputable = NombreCuentaImputable(objReporte.pstrCuenta);
                                    string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
                                    int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
                                    objReporte.pstrDenominacion = objReporte.pstrCuenta != "No Existe*" ? denominacion : "*No Existe*";

                                    var DebeAnterior = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasAnteriores, objReporte.pstrCuenta, "D", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;
                                    var HaberAnterior = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasAnteriores, objReporte.pstrCuenta, "H", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;

                                    objReporte.debeSaldoAnterior = DebeAnterior > HaberAnterior ? DebeAnterior - HaberAnterior : 0;
                                    objReporte.haberSaldoAnterior = HaberAnterior > DebeAnterior ? HaberAnterior - DebeAnterior : 0;




                                    if (int.Parse(pstrMes) == (int)Mes.Enero)
                                    {
                                        objReporte.debeMovimientoMes = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasMesRequerido, objReporte.pstrCuenta, "D", pIntMoneda, pIntCuentasImputables == 1 ? true : false) - SumasDiversasCuentas(CuentasAnteriores, objReporte.pstrCuenta, "D", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;
                                        objReporte.haberMovimientoMes = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasMesRequerido, objReporte.pstrCuenta, "H", pIntMoneda, pIntCuentasImputables == 1 ? true : false) - SumasDiversasCuentas(CuentasAnteriores, objReporte.pstrCuenta, "H", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;
                                    }
                                    else
                                    {
                                        objReporte.debeMovimientoMes = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasMesRequerido, objReporte.pstrCuenta, "D", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;
                                        objReporte.haberMovimientoMes = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasMesRequerido, objReporte.pstrCuenta, "H", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;
                                    }
                                    objReporte.debeSumaAcumulada = objReporte.debeSaldoAnterior + objReporte.debeMovimientoMes;
                                    objReporte.haberSumaAcumulada = objReporte.haberSaldoAnterior + objReporte.haberMovimientoMes;
                                    objReporte.deudorSaldosActuales = objReporte.debeSumaAcumulada > objReporte.haberSumaAcumulada ? objReporte.debeSumaAcumulada - objReporte.haberSumaAcumulada : 0;  //   Saldos[0] > Saldos[1] ? Saldos[0] - Saldos[1] : 0;// Antes del 20 de marzo
                                    objReporte.acreedorSaldosActuales = objReporte.haberSumaAcumulada > objReporte.debeSumaAcumulada ? objReporte.haberSumaAcumulada - objReporte.debeSumaAcumulada : 0;                          //Saldos[1] > Saldos[0] ? Saldos[1] - Saldos[0] : 0;

                                    objReporte.imputable = objReporte.pstrCuenta != "*No Existe*" ? imputable : -1;
                                    if (pIntCuentasGastos79 == 1)
                                    {
                                        objReporte.Grupo2 = objReporte.pstrCuenta.StartsWith("9") || objReporte.pstrCuenta.StartsWith("79") ? "Grupo2" : "";
                                        objReporte.Grupo1 = !objReporte.pstrCuenta.StartsWith("9") && !objReporte.pstrCuenta.StartsWith("79") ? "Grupo1" : "";
                                    }
                                    else
                                    {
                                        objReporte.Grupo2 = objReporte.pstrCuenta.StartsWith("9") ? "Grupo2" : "";
                                        objReporte.Grupo1 = !objReporte.pstrCuenta.StartsWith("9") ? "Grupo1" : "";

                                    }

                                    ListaFinal.Add(objReporte);

                                }
                                pIntDigitos = pIntDigitos + 1;
                            }
                            pIntDigitos = 2;
                        }
                    }
                }

                objOperationResult.Success = 1;
                if (pIntCuentasImputables == 1)
                {
                    return ListaFinal.ToList().Where(y => y.imputable == 1 || (y.imputable == 0 && y.pstrCuenta.Length == 2)).OrderBy(x => x.pstrCuenta).ToList().Where(x => x.debeSaldoAnterior != 0 || x.haberSaldoAnterior != 0 || x.debeMovimientoMes != 0 || x.haberMovimientoMes != 0 || x.debeSumaAcumulada != 0 || x.haberSumaAcumulada != 0 || x.deudorSaldosActuales != 0 || x.acreedorSaldosActuales != 0).ToList();
                }
                else
                {


                    return ListaFinal.OrderBy(x => x.pstrCuenta).ToList().Where(x => x.debeSaldoAnterior != 0 || x.haberSaldoAnterior != 0 || x.debeMovimientoMes != 0 || x.haberMovimientoMes != 0 || x.debeSumaAcumulada != 0 || x.haberSumaAcumulada != 0 || x.deudorSaldosActuales != 0 || x.acreedorSaldosActuales != 0).ToList();
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "ContabilidadBL.ReporteBalanceMensualNuevo()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }

        }


        public List<ReporteBalanceMensual> ReporteBalanceMensualSBS(ref OperationResult objOperationResult, string pstrPeriodo, string pstrMes, int pIntNumeroDigitos, int pIntMoneda, int pIntCuentasImputables, int pIntCuentasGastos79)
        {

            try
            {

                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                List<ReporteBalanceMensual> ListaFinal = new List<ReporteBalanceMensual>();
                string MesAnterior = pstrMes != "01" ? (int.Parse(pstrMes) - 1).ToString("00") : pstrMes;

                #region RecopilaCuentasExistentesMesAnterior
                // Si es Enero se trae las cuentas Mayores de Asientos Apertura 
                // De lo contrario se trae las Cuentas Mayores de la Tabla Saldo Contables 
                //Cuentas que aparecen en Reporte son del Mes Anterior + Mes Requerido
                List<ReporteBalanceMensual> AsientoApertura = new List<ReporteBalanceMensual>();
                List<ReporteBalanceMensual> CuentasAnteriores = new List<ReporteBalanceMensual>();
                List<ReporteBalanceMensual> CuentasAnterioresMes = new List<ReporteBalanceMensual>();
                ReporteBalanceMensual objReporte = new ReporteBalanceMensual();
                List<string> CuentasMayores = new List<string>();
                AsientosContablesBL _objAsientoContableBL = new AsientosContablesBL();

                if (int.Parse(pstrMes) == (int)Mes.Enero)
                {
                    AsientoApertura = (from a in dbContext.diario

                                       join b in dbContext.diariodetalle on new { IdDiario = a.v_IdDiario, eliminado = 0 }
                                                                        equals new { IdDiario = b.v_IdDiario, eliminado = b.i_Eliminado.Value } into b_join
                                       from b in b_join.DefaultIfEmpty()

                                       join c in dbContext.asientocontable on new { asiento = b.v_NroCuenta, eliminado = 0, p = periodo }
                                                                        equals new { asiento = c.v_NroCuenta, eliminado = c.i_Eliminado.Value, p = c.v_Periodo } into c_join

                                       from c in c_join.DefaultIfEmpty()

                                       where a.v_Periodo == pstrPeriodo && a.i_IdTipoComprobante == 1 && a.i_Eliminado == 0 && a.v_Mes == pstrMes && (b.v_NroCuenta != null && b.v_NroCuenta != "")

                                       orderby b.v_NroCuenta
                                       select new ReporteBalanceMensual
                                       {

                                           pstrCuenta = b.v_NroCuenta.Trim(),
                                           pstrDenominacion = c == null ? "*No Existe*" : c.v_NombreCuenta.Trim(),
                                           mes = a.v_Mes,
                                           imputable = c == null ? -1 : c.i_Imputable.Value,
                                           ImporteDebeSoles = a.i_IdMoneda == 1 && b.v_Naturaleza == "D" ? b.d_Importe.Value : a.i_IdMoneda == 2 && b.v_Naturaleza == "D" ? b.d_Cambio.Value : 0,
                                           ImporteDebeDolares = a.i_IdMoneda == 2 && b.v_Naturaleza == "D" ? b.d_Importe.Value : a.i_IdMoneda == 1 && b.v_Naturaleza == "D" ? b.d_Cambio.Value : 0,
                                           ImporteHaberSoles = a.i_IdMoneda == 1 && b.v_Naturaleza == "H" ? b.d_Importe.Value : a.i_IdMoneda == 2 && b.v_Naturaleza == "H" ? b.d_Cambio.Value : 0,
                                           ImporteHaberDolares = a.i_IdMoneda == 2 && b.v_Naturaleza == "H" ? b.d_Importe.Value : a.i_IdMoneda == 1 && b.v_Naturaleza == "H" ? b.d_Cambio.Value : 0,

                                       }).ToList();

                    CuentasAnteriores = AsientoApertura.ToList();

                }
                else
                {
                    var saldoscontablesAnteriores = _objAsientoContableBL.ObtenerSaldosContablesPorMes(ref  objOperationResult, int.Parse(MesAnterior), int.Parse(pstrPeriodo));

                    CuentasAnteriores = (from a in saldoscontablesAnteriores

                                         join b in dbContext.asientocontable on new { cuenta = a.v_NroCuenta, eliminado = 0, p = periodo }
                                                                             equals new { cuenta = b.v_NroCuenta, eliminado = b.i_Eliminado.Value, p = b.v_Periodo } into b_join
                                         from b in b_join.DefaultIfEmpty()

                                         where (a.v_NroCuenta != null && a.v_NroCuenta != "" && a.v_NroCuenta != "*Cta Eliminada*")

                                         orderby a.v_NroCuenta

                                         select new ReporteBalanceMensual
                                         {
                                             pstrCuenta = a.v_NroCuenta.Trim(),
                                             pstrDenominacion = b == null ? "*No Existe*" : b.v_NombreCuenta.Trim(),
                                             mes = a.v_Mes,
                                             imputable = b == null ? -1 : b.i_Imputable.Value,
                                             ImporteDebeSoles = a.d_ImporteSolesD.Value,
                                             ImporteDebeDolares = a.d_ImporteDolaresD.Value,
                                             ImporteHaberSoles = a.d_ImporteSolesH.Value,
                                             ImporteHaberDolares = a.d_ImporteDolaresH.Value,

                                         }).ToList();







                }
                #endregion

                #region RecopilaCuentasExistentesMesRequerido

                var saldoscontables = _objAsientoContableBL.ObtenerSaldosContablesPorMes(ref  objOperationResult, int.Parse(pstrMes), int.Parse(pstrPeriodo), int.Parse(pstrMes));

                var CuentasMesRequerido = (from a in saldoscontables

                                           join b in dbContext.asientocontable on new { cuenta = a.v_NroCuenta, eliminado = 0, p = periodo }
                                                                                 equals new { cuenta = b.v_NroCuenta, eliminado = b.i_Eliminado.Value, p = b.v_Periodo } into b_join
                                           from b in b_join.DefaultIfEmpty()

                                           where (a.v_NroCuenta != null && a.v_NroCuenta != "" && a.v_NroCuenta != "*Cta Eliminada*")
                                           orderby b.v_NroCuenta
                                           select new ReporteBalanceMensual
                                           {
                                               pstrCuenta = a.v_NroCuenta.Trim(),
                                               pstrDenominacion = b == null ? "*No Existe*" : b.v_NombreCuenta.Trim(),
                                               mes = a.v_Mes,
                                               imputable = b == null ? -1 : b.i_Imputable.Value,
                                               ImporteDebeSoles = a.d_ImporteSolesD.Value,
                                               ImporteDebeDolares = a.d_ImporteDolaresD.Value,
                                               ImporteHaberSoles = a.d_ImporteSolesH.Value,
                                               ImporteHaberDolares = a.d_ImporteDolaresH.Value,
                                           }).ToList();
                var g = CuentasAnteriores.Where(o => o.pstrCuenta.StartsWith("38")).ToList();

                var gg = CuentasMesRequerido.Where(l => l.pstrCuenta.StartsWith("38")).ToList();

                #endregion
                // var CuentasTotales = CuentasMesRequerido.Union(CuentasAnteriores).ToList();

                var CuentasTotales = CuentasMesRequerido.Concat(CuentasAnteriores).ToList();
                foreach (var item in CuentasTotales)
                {
                    string NumCuenta = item.pstrCuenta.Substring(0, 2);
                    if (NumCuenta == "38")
                    {
                        string j = "";
                    }
                    if (!CuentasMayores.Contains(NumCuenta))
                    {
                        CuentasMayores.Add(NumCuenta);
                    }
                }
                foreach (var item in CuentasMayores.AsParallel())
                {
                    objReporte = new ReporteBalanceMensual();

                    int pIntDigitos = 2;
                    var ListaEmpiezaCuenta = CuentasTotales.Where(x => x.pstrCuenta.StartsWith(item.Substring(0, 2))).Select(x => x.pstrCuenta);
                    while (pIntDigitos <= pIntNumeroDigitos)
                    {

                        objReporte = new ReporteBalanceMensual();
                        objReporte.pstrCuenta = CuentasTotales.Where(x => x.pstrCuenta.StartsWith(item)).Count() != 0 ? CuentasTotales.Where(x => x.pstrCuenta.StartsWith(item)).FirstOrDefault().pstrCuenta.Length >= pIntDigitos ? CuentasTotales.Where(x => x.pstrCuenta.StartsWith(item)).FirstOrDefault().pstrCuenta.Substring(0, pIntDigitos) : "*No Existe*" : "*No Existe*";

                        if (objReporte.pstrCuenta != "*No Existe*")
                        {
                            var CuentaImputable = NombreCuentaImputable(objReporte.pstrCuenta);
                            string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
                            int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
                            objReporte.pstrDenominacion = objReporte.pstrCuenta != "No Existe*" ? denominacion : "*No Existe*";
                            var DebeAnterior = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasAnteriores, objReporte.pstrCuenta, "D", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;
                            var HaberAnterior = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasAnteriores, objReporte.pstrCuenta, "H", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;
                            objReporte.debeSaldoAnterior = DebeAnterior > HaberAnterior ? DebeAnterior - HaberAnterior : 0;
                            objReporte.haberSaldoAnterior = HaberAnterior > DebeAnterior ? HaberAnterior - DebeAnterior : 0;
                            if (int.Parse(pstrMes) == (int)Mes.Enero)
                            {
                                objReporte.debeMovimientoMes = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasMesRequerido, objReporte.pstrCuenta, "D", pIntMoneda, pIntCuentasImputables == 1 ? true : false) - SumasDiversasCuentas(CuentasAnteriores, objReporte.pstrCuenta, "D", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;
                                objReporte.haberMovimientoMes = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasMesRequerido, objReporte.pstrCuenta, "H", pIntMoneda, pIntCuentasImputables == 1 ? true : false) - SumasDiversasCuentas(CuentasAnteriores, objReporte.pstrCuenta, "H", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;
                            }
                            else
                            {
                                objReporte.debeMovimientoMes = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasMesRequerido, objReporte.pstrCuenta, "D", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;
                                objReporte.haberMovimientoMes = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasMesRequerido, objReporte.pstrCuenta, "H", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;
                            }
                            objReporte.debeSumaAcumulada = objReporte.debeSaldoAnterior + objReporte.debeMovimientoMes;
                            objReporte.haberSumaAcumulada = objReporte.haberSaldoAnterior + objReporte.haberMovimientoMes;
                            objReporte.deudorSaldosActuales = objReporte.debeSumaAcumulada > objReporte.haberSumaAcumulada ? objReporte.debeSumaAcumulada - objReporte.haberSumaAcumulada : 0;  //   Saldos[0] > Saldos[1] ? Saldos[0] - Saldos[1] : 0;// Antes del 20 de marzo
                            objReporte.acreedorSaldosActuales = objReporte.haberSumaAcumulada > objReporte.debeSumaAcumulada ? objReporte.haberSumaAcumulada - objReporte.debeSumaAcumulada : 0;                          //Saldos[1] > Saldos[0] ? Saldos[1] - Saldos[0] : 0;
                            objReporte.imputable = objReporte.pstrCuenta != "*No Existe*" ? imputable : -1;
                            objReporte.Grupo2 = "";
                            objReporte.Grupo1 = "Grupo1";

                            ListaFinal.Add(objReporte);
                        }
                        pIntDigitos = pIntDigitos + 1;
                    }

                    int Contador = 1;
                    pIntDigitos = 2;
                    if (Contador < ListaEmpiezaCuenta.Count())
                    {
                        foreach (var subCuenta in ListaEmpiezaCuenta.AsParallel())
                        {
                            while (pIntDigitos <= pIntNumeroDigitos)
                            {
                                objReporte = new ReporteBalanceMensual();
                                objReporte.pstrCuenta = subCuenta.StartsWith(item) && subCuenta.Length >= pIntDigitos ? subCuenta.Substring(0, pIntDigitos) : "*No Existe*";
                                if (objReporte.pstrCuenta != "*No Existe*" && ListaFinal.Where(x => x.pstrCuenta == objReporte.pstrCuenta).Count() == 0)
                                {
                                    var CuentaImputable = NombreCuentaImputable(objReporte.pstrCuenta);
                                    string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
                                    int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
                                    objReporte.pstrDenominacion = objReporte.pstrCuenta != "No Existe*" ? denominacion : "*No Existe*";

                                    var DebeAnterior = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasAnteriores, objReporte.pstrCuenta, "D", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;
                                    var HaberAnterior = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasAnteriores, objReporte.pstrCuenta, "H", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;

                                    objReporte.debeSaldoAnterior = DebeAnterior > HaberAnterior ? DebeAnterior - HaberAnterior : 0;
                                    objReporte.haberSaldoAnterior = HaberAnterior > DebeAnterior ? HaberAnterior - DebeAnterior : 0;




                                    if (int.Parse(pstrMes) == (int)Mes.Enero)
                                    {
                                        objReporte.debeMovimientoMes = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasMesRequerido, objReporte.pstrCuenta, "D", pIntMoneda, pIntCuentasImputables == 1 ? true : false) - SumasDiversasCuentas(CuentasAnteriores, objReporte.pstrCuenta, "D", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;
                                        objReporte.haberMovimientoMes = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasMesRequerido, objReporte.pstrCuenta, "H", pIntMoneda, pIntCuentasImputables == 1 ? true : false) - SumasDiversasCuentas(CuentasAnteriores, objReporte.pstrCuenta, "H", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;
                                    }
                                    else
                                    {
                                        objReporte.debeMovimientoMes = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasMesRequerido, objReporte.pstrCuenta, "D", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;
                                        objReporte.haberMovimientoMes = objReporte.pstrCuenta != null ? SumasDiversasCuentas(CuentasMesRequerido, objReporte.pstrCuenta, "H", pIntMoneda, pIntCuentasImputables == 1 ? true : false) : 0;
                                    }
                                    objReporte.debeSumaAcumulada = objReporte.debeSaldoAnterior + objReporte.debeMovimientoMes;
                                    objReporte.haberSumaAcumulada = objReporte.haberSaldoAnterior + objReporte.haberMovimientoMes;



                                    objReporte.deudorSaldosActuales = objReporte.debeSumaAcumulada > objReporte.haberSumaAcumulada ? objReporte.debeSumaAcumulada - objReporte.haberSumaAcumulada : 0;  //   Saldos[0] > Saldos[1] ? Saldos[0] - Saldos[1] : 0;// Antes del 20 de marzo
                                    objReporte.acreedorSaldosActuales = objReporte.haberSumaAcumulada > objReporte.debeSumaAcumulada ? objReporte.haberSumaAcumulada - objReporte.debeSumaAcumulada : 0;                          //Saldos[1] > Saldos[0] ? Saldos[1] - Saldos[0] : 0;

                                    objReporte.imputable = objReporte.pstrCuenta != "*No Existe*" ? imputable : -1;
                                    objReporte.Grupo2 = "";
                                    objReporte.Grupo1 = "Grupo1";

                                    ListaFinal.Add(objReporte);

                                }
                                pIntDigitos = pIntDigitos + 1;
                            }
                            pIntDigitos = 2;
                        }
                    }
                }

                objOperationResult.Success = 1;
                if (pIntCuentasImputables == 1)
                {
                    return ListaFinal.ToList().Where(y => y.imputable == 1 || (y.imputable == 0 && y.pstrCuenta.Length == 2)).OrderBy(x => x.pstrCuenta).ToList().Where(x => x.debeSaldoAnterior != 0 || x.haberSaldoAnterior != 0 || x.debeMovimientoMes != 0 || x.haberMovimientoMes != 0 || x.debeSumaAcumulada != 0 || x.haberSumaAcumulada != 0 || x.deudorSaldosActuales != 0 || x.acreedorSaldosActuales != 0).ToList();
                }
                else
                {


                    return ListaFinal.OrderBy(x => x.pstrCuenta).ToList().Where(x => x.debeSaldoAnterior != 0 || x.haberSaldoAnterior != 0 || x.debeMovimientoMes != 0 || x.haberMovimientoMes != 0 || x.debeSumaAcumulada != 0 || x.haberSumaAcumulada != 0 || x.deudorSaldosActuales != 0 || x.acreedorSaldosActuales != 0).ToList();
                }
            }
            catch (Exception ex)
            {



                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "ContabilidadBL.ReporteBalanceMensualSBS()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;


            }

        }

        public decimal SumasDiversasCuentas(List<ReporteBalanceMensual> Lista, string pstrCuenta, string pstrNaturaleza, int pIntMoneda, bool ReporteImputables)
        {
            decimal Valor = 0;
            //  var ListaSaldoAcumAnteriores = ReporteImputables ? Lista.FindAll(x => x.pstrCuenta == pstrCuenta).ToList() : Lista.FindAll(x => x.pstrCuenta.StartsWith(pstrCuenta)).ToList(); //Antes del 9 Mayo

            var ListaSaldoAcumAnteriores = Lista.FindAll(x => x.pstrCuenta.StartsWith(pstrCuenta)).ToList();
            if (pstrNaturaleza == "D")
            {
                return Valor = ListaSaldoAcumAnteriores.Count() != 0 ? pIntMoneda == 1 ? ListaSaldoAcumAnteriores.Sum(x => x.ImporteDebeSoles) : pIntMoneda == 2 ? ListaSaldoAcumAnteriores.Sum(x => x.ImporteDebeDolares) : 0 : 0;
            }

            else
            {
                return Valor = ListaSaldoAcumAnteriores.Count() != 0 ? pIntMoneda == 1 ? ListaSaldoAcumAnteriores.Sum(x => x.ImporteHaberSoles) : pIntMoneda == 2 ? ListaSaldoAcumAnteriores.Sum(x => x.ImporteHaberDolares) : 0 : 0;

            }
        }

        public List<decimal> CalcularSaldosActuales(List<ReporteBalanceMensual> CuentasAnteriores, List<ReporteBalanceMensual> CuentasMesRequerido, string pstrCuenta, int pIntMoneda, List<ReporteBalanceMensual> CuentasTotales, bool ReporteImputables)
        {
            List<decimal> ListaResultante = new List<decimal>();
            List<string> ListaCuentasImputables = new List<string>();
            decimal anteriorDebe = 0, MovMesDebe = 0, anteriorHaber = 0, MovMesHaber = 0, sumaDebe = 0, sumaHaber, deudorActual = 0, acreedorActual = 0, DeudorFinal = 0, AcreedorFinal = 0;
            // var ListaImputables = ReporteImputables ? CuentasTotales.Where(x => x.pstrCuenta == pstrCuenta).ToList() : CuentasTotales.Where(x => x.pstrCuenta.StartsWith(pstrCuenta)).ToList(); //Antes del 9 de Mayo

            var ListaImputables = CuentasTotales.Where(x => x.pstrCuenta.StartsWith(pstrCuenta)).ToList();

            foreach (var item in ListaImputables)
            {
                if (!ListaCuentasImputables.Contains(item.pstrCuenta))
                {
                    ListaCuentasImputables.Add(item.pstrCuenta);
                }
            }

            foreach (var item in ListaCuentasImputables.AsParallel())
            {

                if (pIntMoneda == 1)
                {

                    //anteriorDebe =  CuentasAnteriores.FindAll(x => x.pstrCuenta.StartsWith(item)).Sum(x => x.ImporteDebeSoles); //Antes del 6 de abril .starwith
                    //   MovMesDebe =   CuentasMesRequerido.FindAll(x => x.pstrCuenta.StartsWith(item)).Sum(x => x.ImporteDebeSoles);
                    //  anteriorDebe = CuentasAnteriores.FindAll(x => x.pstrCuenta == item).Sum(x => x.ImporteDebeSoles);
                    MovMesDebe = CuentasMesRequerido.FindAll(x => x.pstrCuenta == item).Sum(x => x.ImporteDebeSoles);
                }
                else
                {
                    // anteriorDebe = CuentasAnteriores.FindAll(x => x.pstrCuenta == item).Sum(x => x.ImporteDebeDolares);
                    MovMesDebe = CuentasMesRequerido.FindAll(x => x.pstrCuenta == item).Sum(x => x.ImporteDebeDolares);
                }


                if (pIntMoneda == 1)
                {
                    // anteriorHaber = CuentasAnteriores.FindAll(x => x.pstrCuenta == item).Sum(x => x.ImporteHaberSoles);
                    //  MovMesHaber =  CuentasMesRequerido.FindAll(x => x.pstrCuenta.StartsWith(item)).Sum(x => x.ImporteHaberSoles); antes del 6 de abril
                    MovMesHaber = CuentasMesRequerido.FindAll(x => x.pstrCuenta == item).Sum(x => x.ImporteHaberSoles);
                }
                else
                {
                    // anteriorHaber = CuentasAnteriores.FindAll(x => x.pstrCuenta == item).Sum(x => x.ImporteHaberDolares);
                    MovMesHaber = CuentasMesRequerido.FindAll(x => x.pstrCuenta == item).Sum(x => x.ImporteHaberDolares);
                }


                sumaDebe = anteriorDebe + MovMesDebe;
                sumaHaber = anteriorHaber + MovMesHaber;

                deudorActual = sumaDebe > sumaHaber ? sumaDebe - sumaHaber : 0;
                acreedorActual = sumaHaber > sumaDebe ? sumaHaber - sumaDebe : 0;


                DeudorFinal = deudorActual + DeudorFinal;
                AcreedorFinal = acreedorActual + AcreedorFinal;


            }
            ListaResultante.Add(DeudorFinal);
            ListaResultante.Add(AcreedorFinal);
            return ListaResultante;

        }


        public List<ReporteBalanceGeneral> ReporteBalanceGeneralNuevo(ref OperationResult objOperationResult, string pstrPeriodo, string pstrMes, int pIntNumeroDigitos, int pIntMoneda, int pImputables)
        {

            try
            {
                AsientosContablesBL _objAsientoContableBL = new AsientosContablesBL();
                objOperationResult.Success = 1;
                List<ReporteBalanceGeneral> ListaFinal = new List<ReporteBalanceGeneral>();
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                List<string> CuentasMayores = new List<string>();
                ReporteBalanceGeneral objReporte = new ReporteBalanceGeneral();
                List<string> ListaCuentasImputables = new List<string>();
                #region RecopilaCuentasExistentesMesRequerido

                var saldoscontables = _objAsientoContableBL.ObtenerSaldosContablesPorMes(ref  objOperationResult, int.Parse(pstrMes), int.Parse(pstrPeriodo));
                var CuentasMesRequerido = (from a in saldoscontables

                                           join b in dbContext.asientocontable on new { cuenta = a.v_NroCuenta, eliminado = 0, p = periodo }
                                                                                 equals new { cuenta = b.v_NroCuenta, eliminado = b.i_Eliminado.Value, p = b.v_Periodo } into b_join
                                           from b in b_join.DefaultIfEmpty()

                                           where (a.v_NroCuenta != null && a.v_NroCuenta != "" && a.v_NroCuenta != "*Cta Eliminada*")  //&&  a.v_Mes == pstrMes 

                                           orderby b.v_NroCuenta
                                           select new ReporteBalanceGeneral
                                           {
                                               pstrCuenta = a.v_NroCuenta.Trim(),
                                               pstrDenominacion = b == null ? "*No Existe*" : b.v_NombreCuenta.Trim(),
                                               mes = a.v_Mes,
                                               imputable = b == null ? -1 : b.i_Imputable.Value,
                                               ImporteDebeSoles = a.d_ImporteSolesD.Value,
                                               ImporteDebeDolares = a.d_ImporteDolaresD.Value,
                                               ImporteHaberSoles = a.d_ImporteSolesH.Value,
                                               ImporteHaberDolares = a.d_ImporteDolaresH.Value,
                                           }).ToList();








                var CuentasMesRequerido1059 = (from n in CuentasMesRequerido
                                               where int.Parse(n.pstrCuenta.Substring(0, 2)) >= 10 && int.Parse(n.pstrCuenta.Substring(0, 2)) <= 59

                                               select n).ToList();
                #endregion

                foreach (var item in CuentasMesRequerido1059)
                {
                    string NumCuenta = item.pstrCuenta.Substring(0, 2);
                    if (!CuentasMayores.Contains(NumCuenta))
                    {
                        CuentasMayores.Add(NumCuenta);
                    }
                }
                foreach (var item in CuentasMayores.AsParallel())
                {
                    ListaCuentasImputables = new List<string>();
                    objReporte = new ReporteBalanceGeneral();
                    int pIntDigitos = 2;
                    var ListaImputables = CuentasMesRequerido1059.Where(x => x.pstrCuenta.StartsWith(item.Substring(0, 2))).Select(x => x.pstrCuenta);
                    foreach (var cuentaImputable in ListaImputables)
                    {
                        if (!ListaCuentasImputables.Contains(cuentaImputable))
                        {
                            ListaCuentasImputables.Add(cuentaImputable);
                        }
                    }
                    while (pIntDigitos <= pIntNumeroDigitos)
                    {
                        objReporte = new ReporteBalanceGeneral();
                        objReporte.pstrCuenta = CuentasMesRequerido1059.Where(x => x.pstrCuenta.StartsWith(item)).Count() != 0 ? CuentasMesRequerido1059.Where(x => x.pstrCuenta.StartsWith(item)).FirstOrDefault().pstrCuenta.Length >= pIntDigitos ? CuentasMesRequerido1059.Where(x => x.pstrCuenta.StartsWith(item)).FirstOrDefault().pstrCuenta.Substring(0, pIntDigitos) : "*No Existe*" : "*No Existe*";
                        if (objReporte.pstrCuenta != "*No Existe*")
                        {
                            var CuentaImputable = NombreCuentaImputable(objReporte.pstrCuenta);
                            string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
                            int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
                            objReporte.pstrDenominacion = objReporte.pstrCuenta != "No Existe*" ? denominacion : "*No Existe*";
                            objReporte.debeSumaAcumulada = objReporte.pstrCuenta != "*No Existe*" ? SumaAcumuladasCuentasReporteBalanceGeneral(CuentasMesRequerido1059, objReporte.pstrCuenta.Trim(), "D", pIntMoneda) : 0;
                            objReporte.haberSumaAcumulada = objReporte.pstrCuenta != "*No Existe*" ? SumaAcumuladasCuentasReporteBalanceGeneral(CuentasMesRequerido1059, objReporte.pstrCuenta.Trim(), "H", pIntMoneda) : 0;
                            var Saldos = SaldosReporteBalanceGeneral(CuentasMesRequerido1059, objReporte.pstrCuenta, pIntMoneda);
                            objReporte.DeudorSaldo = Saldos[0];
                            objReporte.AcreedorSaldo = Saldos[1];
                            //objReporte.ActivoInventario = objReporte.pstrCuenta != "*No Existe*" ? int.Parse(objReporte.pstrCuenta.Substring(0, 2)) <= 59 ? objReporte.DeudorSaldo : 0 : 0; // se habilitò el domingo 20 marzo
                            //objReporte.PasivoInventario = objReporte.pstrCuenta != "*No Existe*" ? int.Parse(objReporte.pstrCuenta.Substring(0, 2)) <= 59 ? objReporte.AcreedorSaldo : 0 : 0;
                            objReporte.ActivoInventario = objReporte.pstrCuenta != "*No Existe*" ? int.Parse(objReporte.pstrCuenta.Substring(0, 2)) >= 10 && int.Parse(objReporte.pstrCuenta.Substring(0, 2)) <= 59 ? objReporte.DeudorSaldo > objReporte.AcreedorSaldo ? objReporte.DeudorSaldo - objReporte.AcreedorSaldo : 0 : 0 : 0; //deudor antes del 20 de marzo
                            objReporte.PasivoInventario = objReporte.pstrCuenta != "*No Existe*" ? int.Parse(objReporte.pstrCuenta.Substring(0, 2)) >= 10 && int.Parse(objReporte.pstrCuenta.Substring(0, 2)) <= 59 ? objReporte.AcreedorSaldo > objReporte.DeudorSaldo ? objReporte.AcreedorSaldo - objReporte.DeudorSaldo : 0 : 0 : 0; //acreedor
                            objReporte.imputable = imputable;


                            ListaFinal.Add(objReporte);
                        }
                        pIntDigitos = pIntDigitos + 1;
                    }

                    int Contador = 1;
                    pIntDigitos = 2;
                    if (Contador < ListaCuentasImputables.Count())
                    {
                        foreach (var subCuenta in ListaCuentasImputables.AsParallel())
                        {
                            while (pIntDigitos <= pIntNumeroDigitos)
                            {
                                objReporte = new ReporteBalanceGeneral();
                                objReporte.pstrCuenta = subCuenta.StartsWith(item) && subCuenta.Length >= pIntDigitos ? subCuenta.Substring(0, pIntDigitos) : "*No Existe*";
                                if (objReporte.pstrCuenta != "*No Existe*" && ListaFinal.Where(x => x.pstrCuenta == objReporte.pstrCuenta).Count() == 0)
                                {
                                    var CuentaImputable = NombreCuentaImputable(objReporte.pstrCuenta);
                                    string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
                                    int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
                                    objReporte.pstrDenominacion = objReporte.pstrCuenta != "No Existe*" ? denominacion : "*No Existe*";
                                    objReporte.debeSumaAcumulada = objReporte.pstrCuenta != "*No Existe*" ? SumaAcumuladasCuentasReporteBalanceGeneral(CuentasMesRequerido1059, objReporte.pstrCuenta.Trim(), "D", pIntMoneda) : 0;
                                    objReporte.haberSumaAcumulada = objReporte.pstrCuenta != "*No Existe*" ? SumaAcumuladasCuentasReporteBalanceGeneral(CuentasMesRequerido1059, objReporte.pstrCuenta.Trim(), "H", pIntMoneda) : 0;
                                    var Saldos = SaldosReporteBalanceGeneral(CuentasMesRequerido1059, objReporte.pstrCuenta, pIntMoneda);
                                    objReporte.DeudorSaldo = Saldos[0];
                                    objReporte.AcreedorSaldo = Saldos[1];
                                    //objReporte.ActivoInventario = objReporte.pstrCuenta != "*No Existe*" ? int.Parse(objReporte.pstrCuenta.Substring(0, 2)) <= 59 ? objReporte.DeudorSaldo : 0 : 0;
                                    //objReporte.PasivoInventario = objReporte.pstrCuenta != "*No Existe*" ? int.Parse(objReporte.pstrCuenta.Substring(0, 2)) <= 59 ? objReporte.AcreedorSaldo : 0 : 0;
                                    objReporte.ActivoInventario = objReporte.pstrCuenta != "*No Existe*" ? int.Parse(objReporte.pstrCuenta.Substring(0, 2)) >= 10 && int.Parse(objReporte.pstrCuenta.Substring(0, 2)) <= 59 ? objReporte.DeudorSaldo > objReporte.AcreedorSaldo ? objReporte.DeudorSaldo - objReporte.AcreedorSaldo : 0 : 0 : 0; //deudor
                                    objReporte.PasivoInventario = objReporte.pstrCuenta != "*No Existe*" ? int.Parse(objReporte.pstrCuenta.Substring(0, 2)) >= 10 && int.Parse(objReporte.pstrCuenta.Substring(0, 2)) <= 59 ? objReporte.AcreedorSaldo > objReporte.DeudorSaldo ? objReporte.AcreedorSaldo - objReporte.DeudorSaldo : 0 : 0 : 0; //acreedor
                                    objReporte.imputable = imputable;

                                    ListaFinal.Add(objReporte);
                                }
                                pIntDigitos = pIntDigitos + 1;
                            }
                            pIntDigitos = 2;
                        }
                    }
                }
                if (pImputables == 1)
                {

                    // return ListaFinal.ToList().Where(y => y.imputable == 1 ).OrderBy(x => x.pstrCuenta).ToList().Where(x => x.debeSumaAcumulada != 0 || x.haberSumaAcumulada != 0 || x.ActivoInventario != 0 || x.PasivoInventario != 0).ToList();

                    return ListaFinal.ToList().Where(y => y.imputable == 1 || (y.imputable == 0 && y.pstrCuenta.Length == 2)).OrderBy(x => x.pstrCuenta).ToList().Where(x => x.debeSumaAcumulada != 0 || x.haberSumaAcumulada != 0 || x.ActivoInventario != 0 || x.PasivoInventario != 0).ToList();
                }
                else
                {

                    return ListaFinal.OrderBy(x => x.pstrCuenta).ToList().Where(x => x.debeSumaAcumulada != 0 || x.haberSumaAcumulada != 0 || x.ActivoInventario != 0 || x.PasivoInventario != 0).ToList();
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "ContabilidadBL.ReporteBalanceGeneralNuevo()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }
        }







        public List<ReporteBalanceGeneral> ReporteBalanceGeneralSBS(ref OperationResult objOperationResult, string pstrPeriodo, string pstrMes, int pIntNumeroDigitos, int pIntMoneda, int pImputables)
        {

            try
            {
                AsientosContablesBL _objAsientoContableBL = new AsientosContablesBL();
                objOperationResult.Success = 1;
                List<ReporteBalanceGeneral> ListaFinal = new List<ReporteBalanceGeneral>();
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                List<string> CuentasMayores = new List<string>();
                ReporteBalanceGeneral objReporte = new ReporteBalanceGeneral();
                List<string> ListaCuentasImputables = new List<string>();
                #region RecopilaCuentasExistentesMesRequerido

                var saldoscontables = _objAsientoContableBL.ObtenerSaldosContablesPorMes(ref  objOperationResult, int.Parse(pstrMes), int.Parse(pstrPeriodo));
                var CuentasMesRequerido = (from a in saldoscontables

                                           join b in dbContext.asientocontable on new { cuenta = a.v_NroCuenta, eliminado = 0, p = periodo }
                                                                                 equals new { cuenta = b.v_NroCuenta, eliminado = b.i_Eliminado.Value, p = b.v_Periodo } into b_join
                                           from b in b_join.DefaultIfEmpty()

                                           where (a.v_NroCuenta != null && a.v_NroCuenta != "" && a.v_NroCuenta != "*Cta Eliminada*")  //&&  a.v_Mes == pstrMes 

                                           select new ReporteBalanceGeneral
                                           {
                                               pstrCuenta = a.v_NroCuenta.Trim(),
                                               pstrDenominacion = b == null ? "*No Existe*" : b.v_NombreCuenta.Trim(),
                                               mes = a.v_Mes,
                                               imputable = b == null ? -1 : b.i_Imputable.Value,
                                               ImporteDebeSoles = a.d_ImporteSolesD.Value,
                                               ImporteDebeDolares = a.d_ImporteDolaresD.Value,
                                               ImporteHaberSoles = a.d_ImporteSolesH.Value,
                                               ImporteHaberDolares = a.d_ImporteDolaresH.Value,
                                           }).ToList();


                var CuentasMesRequerido1059 = (from n in CuentasMesRequerido
                                               where n.pstrCuenta.StartsWith("1") || n.pstrCuenta.StartsWith("2") || n.pstrCuenta.StartsWith("3")

                                               select n).ToList();
                #endregion

                foreach (var item in CuentasMesRequerido1059)
                {
                    string NumCuenta = item.pstrCuenta.Substring(0, 2);
                    if (!CuentasMayores.Contains(NumCuenta))
                    {
                        CuentasMayores.Add(NumCuenta);
                    }
                }
                foreach (var item in CuentasMayores.AsParallel())
                {
                    ListaCuentasImputables = new List<string>();
                    objReporte = new ReporteBalanceGeneral();
                    int pIntDigitos = 2;
                    var ListaImputables = CuentasMesRequerido1059.Where(x => x.pstrCuenta.StartsWith(item.Substring(0, 2))).Select(x => x.pstrCuenta);
                    foreach (var cuentaImputable in ListaImputables)
                    {
                        if (!ListaCuentasImputables.Contains(cuentaImputable))
                        {
                            ListaCuentasImputables.Add(cuentaImputable);
                        }
                    }
                    while (pIntDigitos <= pIntNumeroDigitos)
                    {
                        objReporte = new ReporteBalanceGeneral();
                        objReporte.pstrCuenta = CuentasMesRequerido1059.Where(x => x.pstrCuenta.StartsWith(item)).Count() != 0 ? CuentasMesRequerido1059.Where(x => x.pstrCuenta.StartsWith(item)).FirstOrDefault().pstrCuenta.Length >= pIntDigitos ? CuentasMesRequerido1059.Where(x => x.pstrCuenta.StartsWith(item)).FirstOrDefault().pstrCuenta.Substring(0, pIntDigitos) : "*No Existe*" : "*No Existe*";
                        if (objReporte.pstrCuenta != "*No Existe*")
                        {
                            var CuentaImputable = NombreCuentaImputable(objReporte.pstrCuenta);
                            string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
                            int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
                            objReporte.pstrDenominacion = objReporte.pstrCuenta != "No Existe*" ? denominacion : "*No Existe*";
                            objReporte.debeSumaAcumulada = objReporte.pstrCuenta != "*No Existe*" ? SumaAcumuladasCuentasReporteBalanceGeneral(CuentasMesRequerido1059, objReporte.pstrCuenta.Trim(), "D", pIntMoneda) : 0;
                            objReporte.haberSumaAcumulada = objReporte.pstrCuenta != "*No Existe*" ? SumaAcumuladasCuentasReporteBalanceGeneral(CuentasMesRequerido1059, objReporte.pstrCuenta.Trim(), "H", pIntMoneda) : 0;
                            var Saldos = SaldosReporteBalanceGeneral(CuentasMesRequerido1059, objReporte.pstrCuenta, pIntMoneda);
                            objReporte.DeudorSaldo = Saldos[0];
                            objReporte.AcreedorSaldo = Saldos[1];
                            //objReporte.ActivoInventario = objReporte.pstrCuenta != "*No Existe*" ? int.Parse(objReporte.pstrCuenta.Substring(0, 2)) <= 59 ? objReporte.DeudorSaldo : 0 : 0; // se habilitò el domingo 20 marzo
                            //objReporte.PasivoInventario = objReporte.pstrCuenta != "*No Existe*" ? int.Parse(objReporte.pstrCuenta.Substring(0, 2)) <= 59 ? objReporte.AcreedorSaldo : 0 : 0;
                            objReporte.ActivoInventario = objReporte.pstrCuenta != "*No Existe*" ? int.Parse(objReporte.pstrCuenta.Substring(0, 2)) >= 10 && int.Parse(objReporte.pstrCuenta.Substring(0, 2)) <= 59 ? objReporte.DeudorSaldo > objReporte.AcreedorSaldo ? objReporte.DeudorSaldo - objReporte.AcreedorSaldo : 0 : 0 : 0; //deudor antes del 20 de marzo
                            objReporte.PasivoInventario = objReporte.pstrCuenta != "*No Existe*" ? int.Parse(objReporte.pstrCuenta.Substring(0, 2)) >= 10 && int.Parse(objReporte.pstrCuenta.Substring(0, 2)) <= 59 ? objReporte.AcreedorSaldo > objReporte.DeudorSaldo ? objReporte.AcreedorSaldo - objReporte.DeudorSaldo : 0 : 0 : 0; //acreedor
                            objReporte.imputable = imputable;


                            ListaFinal.Add(objReporte);
                        }
                        pIntDigitos = pIntDigitos + 1;
                    }

                    int Contador = 1;
                    pIntDigitos = 2;
                    if (Contador < ListaCuentasImputables.Count())
                    {
                        foreach (var subCuenta in ListaCuentasImputables.AsParallel())
                        {
                            while (pIntDigitos <= pIntNumeroDigitos)
                            {
                                objReporte = new ReporteBalanceGeneral();
                                objReporte.pstrCuenta = subCuenta.StartsWith(item) && subCuenta.Length >= pIntDigitos ? subCuenta.Substring(0, pIntDigitos) : "*No Existe*";
                                if (objReporte.pstrCuenta != "*No Existe*" && ListaFinal.Where(x => x.pstrCuenta == objReporte.pstrCuenta).Count() == 0)
                                {
                                    var CuentaImputable = NombreCuentaImputable(objReporte.pstrCuenta);
                                    string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
                                    int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
                                    objReporte.pstrDenominacion = objReporte.pstrCuenta != "No Existe*" ? denominacion : "*No Existe*";
                                    objReporte.debeSumaAcumulada = objReporte.pstrCuenta != "*No Existe*" ? SumaAcumuladasCuentasReporteBalanceGeneral(CuentasMesRequerido1059, objReporte.pstrCuenta.Trim(), "D", pIntMoneda) : 0;
                                    objReporte.haberSumaAcumulada = objReporte.pstrCuenta != "*No Existe*" ? SumaAcumuladasCuentasReporteBalanceGeneral(CuentasMesRequerido1059, objReporte.pstrCuenta.Trim(), "H", pIntMoneda) : 0;
                                    var Saldos = SaldosReporteBalanceGeneral(CuentasMesRequerido1059, objReporte.pstrCuenta, pIntMoneda);
                                    objReporte.DeudorSaldo = Saldos[0];
                                    objReporte.AcreedorSaldo = Saldos[1];
                                    //objReporte.ActivoInventario = objReporte.pstrCuenta != "*No Existe*" ? int.Parse(objReporte.pstrCuenta.Substring(0, 2)) <= 59 ? objReporte.DeudorSaldo : 0 : 0;
                                    //objReporte.PasivoInventario = objReporte.pstrCuenta != "*No Existe*" ? int.Parse(objReporte.pstrCuenta.Substring(0, 2)) <= 59 ? objReporte.AcreedorSaldo : 0 : 0;
                                    objReporte.ActivoInventario = objReporte.pstrCuenta != "*No Existe*" ? int.Parse(objReporte.pstrCuenta.Substring(0, 2)) >= 10 && int.Parse(objReporte.pstrCuenta.Substring(0, 2)) <= 59 ? objReporte.DeudorSaldo > objReporte.AcreedorSaldo ? objReporte.DeudorSaldo - objReporte.AcreedorSaldo : 0 : 0 : 0; //deudor
                                    objReporte.PasivoInventario = objReporte.pstrCuenta != "*No Existe*" ? int.Parse(objReporte.pstrCuenta.Substring(0, 2)) >= 10 && int.Parse(objReporte.pstrCuenta.Substring(0, 2)) <= 59 ? objReporte.AcreedorSaldo > objReporte.DeudorSaldo ? objReporte.AcreedorSaldo - objReporte.DeudorSaldo : 0 : 0 : 0; //acreedor
                                    objReporte.imputable = imputable;

                                    ListaFinal.Add(objReporte);
                                }
                                pIntDigitos = pIntDigitos + 1;
                            }
                            pIntDigitos = 2;
                        }
                    }
                }
                if (pImputables == 1)
                {

                    // return ListaFinal.ToList().Where(y => y.imputable == 1 ).OrderBy(x => x.pstrCuenta).ToList().Where(x => x.debeSumaAcumulada != 0 || x.haberSumaAcumulada != 0 || x.ActivoInventario != 0 || x.PasivoInventario != 0).ToList();

                    return ListaFinal.ToList().Where(y => y.imputable == 1 || (y.imputable == 0 && y.pstrCuenta.Length == 2)).OrderBy(x => x.pstrCuenta).ToList().Where(x => x.debeSumaAcumulada != 0 || x.haberSumaAcumulada != 0 || x.ActivoInventario != 0 || x.PasivoInventario != 0).ToList();
                }
                else
                {

                    return ListaFinal.OrderBy(x => x.pstrCuenta).ToList().Where(x => x.debeSumaAcumulada != 0 || x.haberSumaAcumulada != 0 || x.ActivoInventario != 0 || x.PasivoInventario != 0).ToList();
                }
            }
            catch (Exception ex)
            {


                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "ContabilidadBL.ReporteBalanceGeneralSBS()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }
        }

















        public decimal SumaAcumuladasCuentasReporteBalanceGeneral(List<ReporteBalanceGeneral> ListaBalanceGeneral, string NroCuenta, string pstrNaturaleza, int pIntMoneda)
        {
            decimal Valor;

            var Lista = ListaBalanceGeneral.FindAll(x => x.pstrCuenta.StartsWith(NroCuenta)).ToList();
            if (pstrNaturaleza == "D")
            {

                return Valor = Lista.Count() != 0 ? pIntMoneda == 1 ? Lista.Sum(x => x.ImporteDebeSoles) : Lista.Sum(x => x.ImporteDebeDolares) : 0;
            }

            else
            {
                return Valor = Lista.Count() != 0 ? pIntMoneda == 1 ? Lista.Sum(x => x.ImporteHaberSoles) : Lista.Sum(x => x.ImporteHaberDolares) : 0;

            }
        }
        public List<decimal> SaldosReporteBalanceGeneral(List<ReporteBalanceGeneral> ListaTotal, string pstrCuenta, int pIntMoneda)
        {
            List<decimal> ListaFinal = new List<decimal>();
            List<string> ListaCuentasImputables = new List<string>();


            var ListaImputables = ListaTotal.FindAll(x => x.pstrCuenta.StartsWith(pstrCuenta)).ToList();


            foreach (var cuentaImputable in ListaImputables)
            {
                if (!ListaCuentasImputables.Contains(cuentaImputable.pstrCuenta))
                {
                    ListaCuentasImputables.Add(cuentaImputable.pstrCuenta);
                }
            }

            decimal acumuladaDebe = 0, acumuladaHaber = 0, saldoDeudor = 0, saldoAcreedor = 0, DeudorFinal = 0, AcreedorFnal = 0;
            foreach (var item in ListaCuentasImputables.AsParallel())
            {
                if (pIntMoneda == 1)
                {
                    acumuladaDebe = ListaTotal.FindAll(x => x.pstrCuenta.StartsWith(item)).Sum(x => x.ImporteDebeSoles);
                    acumuladaHaber = ListaTotal.FindAll(x => x.pstrCuenta.StartsWith(item)).Sum(x => x.ImporteHaberSoles);
                }
                else
                {
                    acumuladaDebe = ListaTotal.FindAll(x => x.pstrCuenta.StartsWith(item)).Sum(x => x.ImporteDebeDolares);
                    acumuladaHaber = ListaTotal.FindAll(x => x.pstrCuenta.StartsWith(item)).Sum(x => x.ImporteHaberDolares);
                }
                saldoDeudor = acumuladaDebe > acumuladaHaber ? acumuladaDebe - acumuladaHaber : 0;
                saldoAcreedor = acumuladaDebe < acumuladaHaber ? (acumuladaDebe - acumuladaHaber) * -1 : 0;


                DeudorFinal = DeudorFinal + saldoDeudor;
                AcreedorFnal = AcreedorFnal + saldoAcreedor;

            }

            ListaFinal.Add(DeudorFinal);
            ListaFinal.Add(AcreedorFnal);
            return ListaFinal;
        }




        public List<ReporteGananciasPerdidasporFuncion> ReporteGananciasPerdidasporFuncion(ref OperationResult objOperationResult, string pstrPeriodo, string pstrMes, int pIntNumeroDigitos, int pIntMoneda, int pImputables)
        {
            try
            {
                objOperationResult.Success = 1;
                AsientosContablesBL _objAsientoContableBL = new AsientosContablesBL();
                List<ReporteGananciasPerdidasporFuncion> ListaFinal = new List<ReporteGananciasPerdidasporFuncion>();
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                List<string> CuentasMayores = new List<string>();
                ReporteGananciasPerdidasporFuncion objReporte = new ReporteGananciasPerdidasporFuncion();
                List<string> ListaCuentasImputables = new List<string>();
                #region RecopilaCuentasExistentesMesRequerido
                var saldoscontables = _objAsientoContableBL.ObtenerSaldosContablesPorMes(ref  objOperationResult, int.Parse(pstrMes), int.Parse(pstrPeriodo));

                var CuentasMesRequerido = (from a in saldoscontables

                                           join b in dbContext.asientocontable on new { cuenta = a.v_NroCuenta, eliminado = 0, p = periodo }
                                                                                 equals new { cuenta = b.v_NroCuenta, eliminado = 0, p = b.v_Periodo } into b_join
                                           from b in b_join.DefaultIfEmpty()

                                           where (a.v_NroCuenta != null && a.v_NroCuenta != "" && a.v_NroCuenta != "*Cta Eliminada*") && a.v_NroCuenta != null

                                           orderby b.v_NroCuenta
                                           select new ReporteGananciasPerdidasporFuncion
                                           {
                                               pstrCuenta = a.v_NroCuenta.Trim(),
                                               pstrDenominacion = b == null ? "*No Existe*" : b.v_NombreCuenta.Trim(),
                                               mes = a.v_Mes,
                                               imputable = b == null ? -1 : b.i_Imputable.Value,
                                               ImporteDebeSoles = a.d_ImporteSolesD.Value,
                                               ImporteDebeDolares = a.d_ImporteDolaresD.Value,
                                               ImporteHaberSoles = a.d_ImporteSolesH.Value,
                                               ImporteHaberDolares = a.d_ImporteDolaresH.Value,
                                               EsCuentaPerdidaFuncion = CuentaFuncion(a.v_NroCuenta.Trim()),

                                           }).ToList();



                //var CuentasMesRequeridoGananciaFuncion = (from n in CuentasMesRequerido
                //                                          where (int.Parse(n.pstrCuenta.Substring(0, 2)) >= 72 && int.Parse(n.pstrCuenta.Substring(0, 2)) <= 78) || n.pstrCuenta.StartsWith("9") || n.pstrCuenta.StartsWith("69") || n.pstrCuenta.StartsWith("70")

                //                                          select n).ToList();

                //la cuenta 69, toda la 7  (menos 79 ni 71 ), toda la 9 (Función)

                var CuentasMesRequeridoGananciaFuncion = CuentasMesRequerido.Where(o => o.EsCuentaPerdidaFuncion).ToList();


                #endregion

                foreach (var item in CuentasMesRequeridoGananciaFuncion.AsParallel())
                {
                    string NumCuenta = item.pstrCuenta.Substring(0, 2);
                    if (!CuentasMayores.Contains(NumCuenta))
                    {
                        CuentasMayores.Add(NumCuenta);
                    }
                }
                foreach (var item in CuentasMayores.AsParallel())
                {
                    objReporte = new ReporteGananciasPerdidasporFuncion();
                    int pIntDigitos = 2;
                    var ListaImputables = CuentasMesRequeridoGananciaFuncion.Where(x => x.pstrCuenta.StartsWith(item.Substring(0, 2))).Select(x => x.pstrCuenta);


                    foreach (var cuentaImputable in ListaImputables)
                    {
                        if (!ListaCuentasImputables.Contains(cuentaImputable))
                        {
                            ListaCuentasImputables.Add(cuentaImputable);
                        }
                    }

                    while (pIntDigitos <= pIntNumeroDigitos)
                    {

                        objReporte = new ReporteGananciasPerdidasporFuncion();
                        objReporte.pstrCuenta = CuentasMesRequeridoGananciaFuncion.Where(x => x.pstrCuenta.StartsWith(item)).Count() != 0 ? CuentasMesRequeridoGananciaFuncion.Where(x => x.pstrCuenta.StartsWith(item)).FirstOrDefault().pstrCuenta.Length >= pIntDigitos ? CuentasMesRequeridoGananciaFuncion.Where(x => x.pstrCuenta.StartsWith(item)).FirstOrDefault().pstrCuenta.Substring(0, pIntDigitos) : "*No Existe*" : "*No Existe*";
                        if (objReporte.pstrCuenta != "*No Existe*")
                        {
                            var CuentaImputable = NombreCuentaImputable(objReporte.pstrCuenta);
                            string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
                            int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
                            objReporte.pstrDenominacion = objReporte.pstrCuenta != "No Existe*" ? denominacion : "*No Existe*";
                            objReporte.MovimientoDebe = SumaImporteMovimientoPorFuncion(CuentasMesRequeridoGananciaFuncion, "D", pIntMoneda, objReporte.pstrCuenta);
                            objReporte.MovimientoHaber = SumaImporteMovimientoPorFuncion(CuentasMesRequeridoGananciaFuncion, "H", pIntMoneda, objReporte.pstrCuenta);
                            objReporte.perdida = objReporte.MovimientoDebe > objReporte.MovimientoHaber ? objReporte.MovimientoDebe - objReporte.MovimientoHaber : 0;
                            objReporte.gananacia = objReporte.MovimientoDebe < objReporte.MovimientoHaber ? (objReporte.MovimientoDebe - objReporte.MovimientoHaber) * -1 : 0;
                            objReporte.imputable = imputable;
                            ListaFinal.Add(objReporte);
                        }
                        pIntDigitos = pIntDigitos + 1;
                    }

                    int Contador = 1;
                    pIntDigitos = 2;
                    if (Contador < ListaCuentasImputables.Count())
                    {
                        foreach (var subCuenta in ListaCuentasImputables.AsParallel())
                        {
                            while (pIntDigitos <= pIntNumeroDigitos)
                            {
                                objReporte = new ReporteGananciasPerdidasporFuncion();
                                objReporte.pstrCuenta = CuentasMesRequeridoGananciaFuncion.Where(x => x.pstrCuenta.StartsWith(subCuenta)).Count() != 0 ? CuentasMesRequeridoGananciaFuncion.Where(x => x.pstrCuenta.StartsWith(subCuenta)).FirstOrDefault().pstrCuenta.Length >= pIntDigitos ? CuentasMesRequeridoGananciaFuncion.Where(x => x.pstrCuenta.StartsWith(subCuenta)).FirstOrDefault().pstrCuenta.Substring(0, pIntDigitos) : "*No Existe*" : "*No Existe*";
                                if (objReporte.pstrCuenta != "*No Existe*" && ListaFinal.Where(x => x.pstrCuenta == objReporte.pstrCuenta).Count() == 0)
                                {
                                    var CuentaImputable = NombreCuentaImputable(objReporte.pstrCuenta);
                                    string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
                                    int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
                                    objReporte.pstrDenominacion = objReporte.pstrCuenta != "No Existe*" ? denominacion : "*No Existe*";
                                    objReporte.MovimientoDebe = SumaImporteMovimientoPorFuncion(CuentasMesRequeridoGananciaFuncion, "D", pIntMoneda, objReporte.pstrCuenta);
                                    objReporte.MovimientoHaber = SumaImporteMovimientoPorFuncion(CuentasMesRequeridoGananciaFuncion, "H", pIntMoneda, objReporte.pstrCuenta);
                                    objReporte.perdida = objReporte.MovimientoDebe > objReporte.MovimientoHaber ? objReporte.MovimientoDebe - objReporte.MovimientoHaber : 0;
                                    objReporte.gananacia = objReporte.MovimientoDebe < objReporte.MovimientoHaber ? (objReporte.MovimientoDebe - objReporte.MovimientoHaber) * -1 : 0;
                                    objReporte.imputable = imputable;
                                    //if (objReporte.pstrCuenta != "*No Existe*" && ListaFinal.Where(x => x.pstrCuenta == objReporte.pstrCuenta).Count() == 0)
                                    //{
                                    ListaFinal.Add(objReporte);
                                }
                                pIntDigitos = pIntDigitos + 1;
                            }
                            pIntDigitos = 2;
                        }
                    }
                }
                if (pImputables == 1)
                {

                    // return ListaFinal.ToList().Where(y => y.imputable == 1).OrderBy(x => x.pstrCuenta).ToList().Where(x => x.MovimientoDebe != 0 || x.MovimientoHaber != 0 || x.perdida != 0 || x.gananacia != 0).ToList(); Antes del 9 de Mayo

                    return ListaFinal.ToList().Where(y => y.imputable == 1 || (y.imputable == 0 && y.pstrCuenta.Length == 2)).OrderBy(x => x.pstrCuenta).ToList().Where(x => x.MovimientoDebe != 0 || x.MovimientoHaber != 0 || x.perdida != 0 || x.gananacia != 0).ToList();
                }
                else
                {

                    return ListaFinal.OrderBy(x => x.pstrCuenta).ToList().Where(x => x.MovimientoDebe != 0 || x.MovimientoHaber != 0 || x.perdida != 0 || x.gananacia != 0).ToList();
                }

            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "ContabilidadBL.ReporteGananciasPerdidasporFuncion()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }


        }






        public List<ReporteGananciaPerdidaFuncionConfigurable> ReporteGananciasPerdidasporFuncionConfigurable(ref OperationResult objOperationResult, string pstrPeriodo, string pstrMes, int pIntNumeroDigitos, int pIntMoneda, int pImputables)
        {
            try
            {
                objOperationResult.Success = 1;
                List<ReporteGananciaPerdidaFuncionConfigurable> ListaFinal = new List<ReporteGananciaPerdidaFuncionConfigurable>();
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var aptitudeCertificate = new ContabilidadBL().ReporteHojaTrabajo(ref objOperationResult, pstrPeriodo, pstrMes, pIntNumeroDigitos, pIntMoneda, pImputables);
                    if (objOperationResult.Success == 0)
                    {
                        return null;
                    }
                    var ListaFuncion = aptitudeCertificate.Select(o => new { cuenta = o.cuenta, perdidaFuncion = o.perdidaFuncion, gananciaFuncion = o.gananciaFuncion, imputable = o.imputable, naturaleza = o.Naturaleza }).ToList();
                    var TableConfiguracionBalances = dbContext.configuracionbalances.Where(o => o.i_Eliminado == 0).ToList();
                    var Grupos = TableConfiguracionBalances.ToList().Where(o => o.v_TipoBalance == ((int)ConfiguracionBalances.EstadodeResultadosFuncion).ToString()).ToList();
                    var plancontable = dbContext.asientocontable.ToList().Where(o => o.i_Eliminado == 0 && o.v_Periodo == Globals.ClientSession.i_Periodo.ToString()).ToList();
                    var GruposPrincipales = Grupos.Where(o => o.v_Codigo.Length == 2).ToList();
                    foreach (var grupito in GruposPrincipales)
                    {
                        var GruposSecundarios = Grupos.Where(o => o.v_Codigo.Length == grupito.v_Codigo.Length + 1 && o.v_Codigo.StartsWith(grupito.v_Codigo)).ToList();
                        if (!GruposSecundarios.Any())
                        {
                            ReporteGananciaPerdidaFuncionConfigurable objReporte = new ReporteGananciaPerdidaFuncionConfigurable();
                            objReporte.Grupo1 = grupito.v_Nombre;
                            objReporte.Grupo2 = "";
                            objReporte.Detalle = "";
                            objReporte.Total = 0;
                            objReporte.CodigoGrupo = grupito.v_Codigo;
                            objReporte.NombreTotalesSubgrupo = "";
                            objReporte.NombreTotalesGrupo = grupito.v_Nombre;
                            ListaFinal.Add(objReporte);
                        }
                        else
                        {

                            foreach (var grupSecundario in GruposSecundarios)
                            {
                                var Detalles = Grupos.Where(o => o.v_Codigo.Length == grupSecundario.v_Codigo.Length + 1 && o.v_Codigo.StartsWith(grupSecundario.v_Codigo)).ToList();

                                foreach (var detail in Detalles)
                                {
                                    ReporteGananciaPerdidaFuncionConfigurable objReporte = new ReporteGananciaPerdidaFuncionConfigurable();
                                    objReporte.Grupo1 = grupito.v_Nombre;
                                    objReporte.Grupo2 = grupSecundario.v_Nombre;
                                    objReporte.Detalle = detail.v_Nombre;
                                    var Cuentasdetail = plancontable.Where(o => o.v_CodigoBalanceFuncion == detail.v_Codigo).ToList();
                                    var CuentasTenerCuenta = ListaFuncion.Where(o => Cuentasdetail.Select(x => x.v_NroCuenta).Contains(o.cuenta)).ToList();
                                    var Debe = CuentasTenerCuenta.Sum(o => o.perdidaFuncion);
                                    var Haber = CuentasTenerCuenta.Sum(o => o.gananciaFuncion);

                                    objReporte.Total = Haber - Debe;
                                    objReporte.NombreTotalesSubgrupo = grupSecundario.v_Codigo == "G11" || grupSecundario.v_Codigo == "G12" ? grupSecundario.v_NombreGrupo : "";
                                    objReporte.NombreTotalesGrupo = grupito.v_Nombre;
                                    objReporte.CodigoGrupo = grupito.v_Codigo;
                                    objReporte.CodigoGrupoReal = detail.v_Codigo;
                                    // objReporte.i_TipoOperacion = detail.i_TipoOperacion ?? -1;
                                    ListaFinal.Add(objReporte);
                                }
                            }
                        }
                    }

                    ListaFinal = ListaFinal.OrderBy(o => o.CodigoGrupo).ToList();
                    int ContadorGrupos = 1;
                    decimal TotalGrupo = 0;
                    foreach (var grupitos in GruposPrincipales.OrderBy(o => o.v_Codigo))
                    {



                        //var grupSumas = ListaFinal.Where(o => o.CodigoGrupo.StartsWith(grupitos.v_Codigo) && o.i_TipoOperacion == (int)TipoOperacionConfiguracionBalances.Suma).Sum(o => o.Total);
                        //var grupRestas = ListaFinal.Where(o => o.CodigoGrupo.StartsWith(grupitos.v_Codigo) && o.i_TipoOperacion == (int)TipoOperacionConfiguracionBalances.Resta).Sum(o => o.Total);
                        var grupTotales = ListaFinal.Where(o => o.CodigoGrupo.StartsWith(grupitos.v_Codigo)).Sum(o => o.Total);
                        TotalGrupo = ContadorGrupos == 1 ? grupTotales : TotalGrupo + grupTotales;
                        var detallesAsociados = ListaFinal.Where(o => o.CodigoGrupo.StartsWith(grupitos.v_Codigo));

                        foreach (var item in ListaFinal)
                        {
                            if (item.CodigoGrupo.StartsWith(grupitos.v_Codigo))
                            {
                                item.TotalGrupo = TotalGrupo;
                            }
                        }
                        ContadorGrupos++;
                    }


                    return ListaFinal.OrderBy(o => o.CodigoGrupoReal).ToList();
                }
            }
            catch (Exception ex)
            {


                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "ContabilidadBL.ReporteGananciaPerdidaFuncionConfigurable()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }
        }

        public List<ReporteGananciaPerdidaFuncionConfigurable> ReporteGananciasPerdidasporFuncionConfigurable_(ref OperationResult objOperationResult, string pstrPeriodo, string pstrMes, int pIntNumeroDigitos, int pIntMoneda, int pImputables, string MesInicio, string MesFin)
        {
            try
            {
                objOperationResult.Success = 1;
                List<ReporteGananciaPerdidaFuncionConfigurable> ListaFinal = new List<ReporteGananciaPerdidaFuncionConfigurable>();
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    var CuentasAsociadasBalanceFuncion = dbContext.asientocontable.Where(o => o.i_Eliminado == 0 && o.v_CodigoBalanceFuncion != null && o.v_Periodo == pstrPeriodo).ToList().ToDTOs();
                    var aptitudeCertificate = new ContabilidadBL().ReporteGananciasPerdidasporFuncionCcosto(ref objOperationResult, pstrPeriodo, pstrMes, pIntNumeroDigitos, pIntMoneda, pImputables, "", false, true, CuentasAsociadasBalanceFuncion, MesInicio, MesFin);

                    if (objOperationResult.Success == 0)
                    {
                        return null;
                    }
                    var ListaFuncion = aptitudeCertificate.Select(o => new
                    {
                        cuenta = o.pstrCuenta,
                        acumulado = o.Acumulado,
                        imputable = o.imputable,
                        naturaleza = o.Naturaleza,
                        mensual = o.Mensual,
                        Mes1 = o.Mes1,
                        Mes2 = o.Mes2,
                        Mes3 = o.Mes3,
                        Mes4 = o.Mes4,
                        Mes5 = o.Mes5,
                        Mes6 = o.Mes6,
                        Mes7 = o.Mes7,
                        Mes8 = o.Mes8,
                        Mes9 = o.Mes9,
                        Mes10 = o.Mes10,
                        Mes11 = o.Mes11,
                        Mes12 = o.Mes12,
                        NombreMes1 = o.NombreMes1,
                        NombreMes2 = o.NombreMes2,
                        NombreMes3 = o.NombreMes3,
                        NombreMes4 = o.NombreMes4,

                        NombreMes5 = o.NombreMes5,
                        NombreMes6 = o.NombreMes6,
                        NombreMes7 = o.NombreMes7,
                        NombreMes8 = o.NombreMes8,
                        NombreMes9 = o.NombreMes9,
                        NombreMes10 = o.NombreMes10,
                        NombreMes11 = o.NombreMes11,
                        NombreMes12 = o.NombreMes12


                    }).ToList();
                    var TableConfiguracionBalances = dbContext.configuracionbalances.Where(o => o.i_Eliminado == 0).ToList();
                    var Grupos = TableConfiguracionBalances.ToList().Where(o => o.v_TipoBalance == ((int)ConfiguracionBalances.EstadodeResultadosFuncion).ToString()).ToList();
                    var plancontable = dbContext.asientocontable.ToList().Where(o => o.i_Eliminado == 0 && o.v_Periodo == Globals.ClientSession.i_Periodo.ToString()).ToList();
                    var GruposPrincipales = Grupos.Where(o => o.v_Codigo.Length == 2).ToList();
                    foreach (var grupito in GruposPrincipales)
                    {
                        var GruposSecundarios = Grupos.Where(o => o.v_Codigo.Length == grupito.v_Codigo.Length + 1 && o.v_Codigo.StartsWith(grupito.v_Codigo)).ToList();
                        if (!GruposSecundarios.Any())
                        {
                            ReporteGananciaPerdidaFuncionConfigurable objReporte = new ReporteGananciaPerdidaFuncionConfigurable();
                            objReporte.Grupo1 = grupito.v_Nombre;
                            objReporte.Grupo2 = "";
                            objReporte.Detalle = "";
                            objReporte.Total = 0;
                            objReporte.CodigoGrupo = grupito.v_Codigo;
                            objReporte.Mensual = 0;
                            objReporte.NombreTotalesSubgrupo = "";
                            objReporte.NombreTotalesGrupo = grupito.v_Nombre;
                            ListaFinal.Add(objReporte);
                        }
                        else
                        {

                            foreach (var grupSecundario in GruposSecundarios)
                            {
                                var Detalles = Grupos.Where(o => o.v_Codigo.Length == grupSecundario.v_Codigo.Length + 1 && o.v_Codigo.StartsWith(grupSecundario.v_Codigo)).ToList();

                                foreach (var detail in Detalles)
                                {
                                    ReporteGananciaPerdidaFuncionConfigurable objReporte = new ReporteGananciaPerdidaFuncionConfigurable();
                                    objReporte.Grupo1 = grupito.v_Nombre;
                                    objReporte.Grupo2 = grupSecundario.v_Nombre;
                                    objReporte.Detalle = detail.v_Nombre;
                                    var Cuentasdetail = plancontable.Where(o => o.v_CodigoBalanceFuncion == detail.v_Codigo).ToList();
                                    var CuentasTenerCuenta = ListaFuncion.Where(o => Cuentasdetail.Select(x => x.v_NroCuenta).Contains(o.cuenta)).ToList();
                                    var acumulado = CuentasTenerCuenta.Sum(o => o.acumulado);
                                    objReporte.Total = acumulado;// Haber - Debe;
                                    objReporte.Mensual = CuentasTenerCuenta.Sum(o => o.mensual);
                                    objReporte.Mes1 = CuentasTenerCuenta.Sum(o => o.Mes1);
                                    objReporte.Mes2 = CuentasTenerCuenta.Sum(o => o.Mes2);
                                    objReporte.Mes3 = CuentasTenerCuenta.Sum(o => o.Mes3);

                                    objReporte.Mes4 = CuentasTenerCuenta.Sum(o => o.Mes4);
                                    objReporte.Mes5 = CuentasTenerCuenta.Sum(o => o.Mes5);
                                    objReporte.Mes6 = CuentasTenerCuenta.Sum(o => o.Mes6);
                                    objReporte.Mes7 = CuentasTenerCuenta.Sum(o => o.Mes7);
                                    objReporte.Mes8 = CuentasTenerCuenta.Sum(o => o.Mes8);
                                    objReporte.Mes9 = CuentasTenerCuenta.Sum(o => o.Mes9);
                                    objReporte.Mes10 = CuentasTenerCuenta.Sum(o => o.Mes10);
                                    objReporte.Mes11 = CuentasTenerCuenta.Sum(o => o.Mes11);
                                    objReporte.Mes12 = CuentasTenerCuenta.Sum(o => o.Mes12);


                                    int iMesDesde = int.Parse(MesInicio);
                                    int iMesHasta = int.Parse(MesFin);
                                    objReporte.NombreMes1 = CuentasTenerCuenta.FirstOrDefault() == null ? iMesDesde <= iMesHasta ? NombreMeses(iMesDesde) : "" : CuentasTenerCuenta.FirstOrDefault().NombreMes1;
                                    iMesDesde++;
                                    objReporte.NombreMes2 = CuentasTenerCuenta.FirstOrDefault() == null ? iMesDesde <= iMesHasta ? NombreMeses(iMesDesde) : "" : CuentasTenerCuenta.FirstOrDefault().NombreMes2;
                                    iMesDesde++;
                                    objReporte.NombreMes3 = CuentasTenerCuenta.FirstOrDefault() == null ? iMesDesde <= iMesHasta ? NombreMeses(iMesDesde) : "" : CuentasTenerCuenta.FirstOrDefault().NombreMes3;
                                    iMesDesde++;
                                    objReporte.NombreMes4 = CuentasTenerCuenta.FirstOrDefault() == null ? iMesDesde <= iMesHasta ? NombreMeses(iMesDesde) : "" : CuentasTenerCuenta.FirstOrDefault().NombreMes4;
                                    iMesDesde++;
                                    objReporte.NombreMes5 = CuentasTenerCuenta.FirstOrDefault() == null ? iMesDesde <= iMesHasta ? NombreMeses(iMesDesde) : "" : CuentasTenerCuenta.FirstOrDefault().NombreMes5;
                                    iMesDesde++;
                                    objReporte.NombreMes6 = CuentasTenerCuenta.FirstOrDefault() == null ? iMesDesde <= iMesHasta ? NombreMeses(iMesDesde) : "" : CuentasTenerCuenta.FirstOrDefault().NombreMes6;
                                    iMesDesde++;
                                    objReporte.NombreMes7 = CuentasTenerCuenta.FirstOrDefault() == null ? iMesDesde <= iMesHasta ? NombreMeses(iMesDesde) : "" : CuentasTenerCuenta.FirstOrDefault().NombreMes7;
                                    iMesDesde++;
                                    objReporte.NombreMes8 = CuentasTenerCuenta.FirstOrDefault() == null ? iMesDesde <= iMesHasta ? NombreMeses(iMesDesde) : "" : CuentasTenerCuenta.FirstOrDefault().NombreMes8;
                                    iMesDesde++;
                                    objReporte.NombreMes9 = CuentasTenerCuenta.FirstOrDefault() == null ? iMesDesde <= iMesHasta ? NombreMeses(iMesDesde) : "" : CuentasTenerCuenta.FirstOrDefault().NombreMes9;
                                    iMesDesde++;
                                    objReporte.NombreMes10 = CuentasTenerCuenta.FirstOrDefault() == null ? iMesDesde <= iMesHasta ? NombreMeses(iMesDesde) : "" : CuentasTenerCuenta.FirstOrDefault().NombreMes10;
                                    iMesDesde++;
                                    objReporte.NombreMes11 = CuentasTenerCuenta.FirstOrDefault() == null ? iMesDesde <= iMesHasta ? NombreMeses(iMesDesde) : "" : CuentasTenerCuenta.FirstOrDefault().NombreMes11;
                                    iMesDesde++;
                                    objReporte.NombreMes12 = CuentasTenerCuenta.FirstOrDefault() == null ? iMesDesde <= iMesHasta ? NombreMeses(iMesDesde) : "" : CuentasTenerCuenta.FirstOrDefault().NombreMes12;

                                    objReporte.NombreTotalesSubgrupo = grupSecundario.v_Codigo == "G11" || grupSecundario.v_Codigo == "G12" ? grupSecundario.v_NombreGrupo : "";
                                    objReporte.NombreTotalesGrupo = grupito.v_Nombre;
                                    objReporte.CodigoGrupo = grupito.v_Codigo;
                                    objReporte.CodigoGrupoReal = detail.v_Codigo;
                                    ListaFinal.Add(objReporte);
                                }
                            }
                        }
                    }

                    ListaFinal = ListaFinal.OrderBy(o => o.CodigoGrupo).ToList();
                    int ContadorGrupos = 1;
                    decimal TotalGrupo = 0;
                    decimal TotalGrupoMensual = 0;
                    foreach (var grupitos in GruposPrincipales.OrderBy(o => o.v_Codigo))
                    {
                        var grupTotales = ListaFinal.Where(o => o.CodigoGrupo.StartsWith(grupitos.v_Codigo)).Sum(o => o.Total);
                        var grupTotalesMensual = ListaFinal.Where(o => o.CodigoGrupo.StartsWith(grupitos.v_Codigo)).Sum(o => o.Mensual);
                        TotalGrupo = ContadorGrupos == 1 ? grupTotales : TotalGrupo + grupTotales;
                        TotalGrupoMensual = ContadorGrupos == 1 ? grupTotalesMensual : TotalGrupoMensual + grupTotalesMensual;
                        var detallesAsociados = ListaFinal.Where(o => o.CodigoGrupo.StartsWith(grupitos.v_Codigo));

                        foreach (var item in ListaFinal)
                        {
                            if (item.CodigoGrupo.StartsWith(grupitos.v_Codigo))
                            {
                                item.TotalGrupo = TotalGrupo;
                                item.TotalGrupoMensual = TotalGrupoMensual;
                            }
                        }
                        ContadorGrupos++;
                    }


                    return ListaFinal.OrderBy(o => o.CodigoGrupoReal).ToList();
                }
            }
            catch (Exception ex)
            {


                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "ContabilidadBL.ReporteGananciaPerdidaFuncionConfigurable()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }
        }


        public List<ReporteGananciaPerdidaFuncionConfigurable> ReporteGananciasPerdidasporNaturalezaConfigurable(ref OperationResult objOperationResult, string pstrPeriodo, string pstrMes, int pIntNumeroDigitos, int pIntMoneda, int pImputables)
        {
            try
            {
                objOperationResult.Success = 1;
                List<ReporteGananciaPerdidaFuncionConfigurable> ListaFinal = new List<ReporteGananciaPerdidaFuncionConfigurable>();
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var aptitudeCertificate = new ContabilidadBL().ReporteHojaTrabajo(ref objOperationResult, pstrPeriodo, pstrMes, pIntNumeroDigitos, pIntMoneda, pImputables);
                    if (objOperationResult.Success == 0)
                    {
                        return null;
                    }
                    var ListaFuncion = aptitudeCertificate.Select(o => new { cuenta = o.cuenta, perdidaNaturaleza = o.perdidaNaturaleza, gananciaNaturaleza = o.gananciaNaturaleza, imputable = o.imputable, naturaleza = o.Naturaleza }).ToList();
                    var TableConfiguracionBalances = dbContext.configuracionbalances.Where(o => o.i_Eliminado == 0).ToList();
                    var Grupos = TableConfiguracionBalances.ToList().Where(o => o.v_TipoBalance == ((int)ConfiguracionBalances.EstadodeResultadosNaturaleza).ToString()).ToList();
                    var plancontable = dbContext.asientocontable.ToList().Where(o => o.i_Eliminado == 0 && o.v_Periodo == Globals.ClientSession.i_Periodo.ToString()).ToList();
                    var GruposPrincipales = Grupos.Where(o => o.v_Codigo.Length == 2).ToList();
                    foreach (var grupito in GruposPrincipales)
                    {
                        var GruposSecundarios = Grupos.Where(o => o.v_Codigo.Length == grupito.v_Codigo.Length + 1 && o.v_Codigo.StartsWith(grupito.v_Codigo)).ToList();
                        if (!GruposSecundarios.Any())
                        {
                            ReporteGananciaPerdidaFuncionConfigurable objReporte = new ReporteGananciaPerdidaFuncionConfigurable();
                            objReporte.Grupo1 = grupito.v_Nombre;
                            objReporte.Grupo2 = "";
                            objReporte.Detalle = "";
                            objReporte.Total = 0;
                            objReporte.CodigoGrupo = grupito.v_Codigo;
                            objReporte.NombreTotalesSubgrupo = "";
                            objReporte.NombreTotalesGrupo = grupito.v_Nombre;
                            ListaFinal.Add(objReporte);
                        }
                        else
                        {

                            foreach (var grupSecundario in GruposSecundarios)
                            {
                                var Detalles = Grupos.Where(o => o.v_Codigo.Length == grupSecundario.v_Codigo.Length + 1 && o.v_Codigo.StartsWith(grupSecundario.v_Codigo)).ToList();

                                foreach (var detail in Detalles)
                                {
                                    ReporteGananciaPerdidaFuncionConfigurable objReporte = new ReporteGananciaPerdidaFuncionConfigurable();
                                    objReporte.Grupo1 = grupito.v_Nombre;
                                    objReporte.Grupo2 = grupSecundario.v_Nombre;
                                    objReporte.Detalle = detail.v_Nombre;
                                    var Cuentasdetail = plancontable.Where(o => o.v_CodigoBalanceNaturaleza == detail.v_Codigo).ToList();
                                    var CuentasTenerCuenta = ListaFuncion.Where(o => Cuentasdetail.Select(x => x.v_NroCuenta).Contains(o.cuenta)).ToList();
                                    var Debe = CuentasTenerCuenta.Sum(o => o.perdidaNaturaleza);
                                    var Haber = CuentasTenerCuenta.Sum(o => o.gananciaNaturaleza);

                                    objReporte.Total = Haber - Debe;
                                    objReporte.NombreTotalesSubgrupo = grupSecundario.v_Codigo == "G11" || grupSecundario.v_Codigo == "G12" ? grupSecundario.v_NombreGrupo : "";
                                    objReporte.NombreTotalesGrupo = grupito.v_Nombre;
                                    objReporte.CodigoGrupo = grupito.v_Codigo;
                                    objReporte.CodigoGrupoReal = detail.v_Codigo;
                                    //objReporte.i_TipoOperacion = detail.i_TipoOperacion ?? -1;
                                    ListaFinal.Add(objReporte);
                                }
                            }
                        }
                    }

                    ListaFinal = ListaFinal.OrderBy(o => o.CodigoGrupo).ToList();
                    int ContadorGrupos = 1;
                    decimal TotalGrupo = 0;
                    foreach (var grupitos in GruposPrincipales.OrderBy(o => o.v_Codigo))
                    {
                        //var grupSumas = ListaFinal.Where(o => o.CodigoGrupo.StartsWith(grupitos.v_Codigo) && o.i_TipoOperacion == (int)TipoOperacionConfiguracionBalances.Suma).Sum(o => o.Total);
                        //var grupRestas = ListaFinal.Where(o => o.CodigoGrupo.StartsWith(grupitos.v_Codigo) && o.i_TipoOperacion == (int)TipoOperacionConfiguracionBalances.Resta).Sum(o => o.Total);
                        var grupTotales = ListaFinal.Where(o => o.CodigoGrupo.StartsWith(grupitos.v_Codigo)).Sum(o => o.Total);
                        TotalGrupo = ContadorGrupos == 1 ? grupTotales : TotalGrupo + grupTotales;
                        var detallesAsociados = ListaFinal.Where(o => o.CodigoGrupo.StartsWith(grupitos.v_Codigo));

                        foreach (var item in ListaFinal)
                        {
                            if (item.CodigoGrupo.StartsWith(grupitos.v_Codigo))
                            {
                                item.TotalGrupo = TotalGrupo;
                            }
                        }
                        ContadorGrupos++;
                    }


                    return ListaFinal.OrderBy(o => o.CodigoGrupoReal).ToList();
                }
            }
            catch (Exception ex)
            {


                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "ContabilidadBL.ReporteGananciasPerdidasporNaturalezaConfigurable()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }
        }

        public List<ReporteGananciaPerdidaFuncionConfigurable> ReporteGananciasPerdidasporNaturalezaConfigurable_(ref OperationResult objOperationResult, string pstrPeriodo, string pstrMes, int pIntNumeroDigitos, int pIntMoneda, int pImputables, string MesInicio, string MesFin)
        {
            try
            {
                objOperationResult.Success = 1;
                List<ReporteGananciaPerdidaFuncionConfigurable> ListaFinal = new List<ReporteGananciaPerdidaFuncionConfigurable>();
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    //var aptitudeCertificate = new ContabilidadBL().ReporteHojaTrabajo(ref objOperationResult, pstrPeriodo, pstrMes, pIntNumeroDigitos, pIntMoneda, pImputables);
                    var CuentasAsociadasBalanceFuncion = dbContext.asientocontable.Where(o => o.i_Eliminado == 0 && o.v_CodigoBalanceNaturaleza != null && o.v_Periodo == pstrPeriodo).ToList().ToDTOs();
                    var aptitudeCertificate = new ContabilidadBL().ReporteGananciasPerdidasporNaturalezaCcosto(ref objOperationResult, pstrPeriodo, pstrMes, pIntNumeroDigitos, pIntMoneda, pImputables, "", false, true, CuentasAsociadasBalanceFuncion, MesInicio, MesFin);

                    if (objOperationResult.Success == 0)
                    {
                        return null;
                    }
                    var ListaFuncion = aptitudeCertificate.Select(o => new
                    {
                        cuenta = o.pstrCuenta,
                        acumulado = o.Acumulado,
                        mensual = o.Mensual,
                        imputable = o.imputable,
                        naturaleza = o.Naturaleza,
                        Mes1 = o.Mes1,
                        Mes2 = o.Mes2,
                        Mes3 = o.Mes3,
                        Mes4 = o.Mes4,
                        Mes5 = o.Mes5,
                        Mes6 = o.Mes6,
                        Mes7 = o.Mes7,
                        Mes8 = o.Mes8,
                        Mes9 = o.Mes9,
                        Mes10 = o.Mes10,
                        Mes11 = o.Mes11,
                        Mes12 = o.Mes12,

                        NombreMes1 = o.NombreMes1,
                        NombreMes2 = o.NombreMes2,
                        NombreMes3 = o.NombreMes3,
                        NombreMes4 = o.NombreMes4,
                        NombreMes5 = o.NombreMes5,
                        NombreMes6 = o.NombreMes6,
                        NombreMes7 = o.NombreMes7,
                        NombreMes8 = o.NombreMes8,
                        NombreMes9 = o.NombreMes9,
                        NombreMes10 = o.NombreMes10,
                        NombreMes11 = o.NombreMes11,
                        NombreMes12 = o.NombreMes12,




                    }).ToList();
                    var TableConfiguracionBalances = dbContext.configuracionbalances.Where(o => o.i_Eliminado == 0).ToList();
                    var Grupos = TableConfiguracionBalances.ToList().Where(o => o.v_TipoBalance == ((int)ConfiguracionBalances.EstadodeResultadosNaturaleza).ToString()).ToList();
                    var plancontable = dbContext.asientocontable.ToList().Where(o => o.i_Eliminado == 0 && o.v_Periodo == Globals.ClientSession.i_Periodo.ToString()).ToList();
                    var GruposPrincipales = Grupos.Where(o => o.v_Codigo.Length == 2).ToList();
                    foreach (var grupito in GruposPrincipales)
                    {
                        var GruposSecundarios = Grupos.Where(o => o.v_Codigo.Length == grupito.v_Codigo.Length + 1 && o.v_Codigo.StartsWith(grupito.v_Codigo)).ToList();
                        if (!GruposSecundarios.Any())
                        {
                            ReporteGananciaPerdidaFuncionConfigurable objReporte = new ReporteGananciaPerdidaFuncionConfigurable();
                            objReporte.Grupo1 = grupito.v_Nombre;
                            objReporte.Grupo2 = "";
                            objReporte.Detalle = "";
                            objReporte.Total = 0;
                            objReporte.TotalGrupoMensual = 0;
                            objReporte.CodigoGrupo = grupito.v_Codigo;
                            objReporte.NombreTotalesSubgrupo = "";
                            objReporte.NombreTotalesGrupo = grupito.v_Nombre;
                            ListaFinal.Add(objReporte);
                        }
                        else
                        {

                            foreach (var grupSecundario in GruposSecundarios)
                            {
                                var Detalles = Grupos.Where(o => o.v_Codigo.Length == grupSecundario.v_Codigo.Length + 1 && o.v_Codigo.StartsWith(grupSecundario.v_Codigo)).ToList();

                                foreach (var detail in Detalles)
                                {
                                    ReporteGananciaPerdidaFuncionConfigurable objReporte = new ReporteGananciaPerdidaFuncionConfigurable();
                                    objReporte.Grupo1 = grupito.v_Nombre;
                                    objReporte.Grupo2 = grupSecundario.v_Nombre;
                                    objReporte.Detalle = detail.v_Nombre;
                                    var Cuentasdetail = plancontable.Where(o => o.v_CodigoBalanceNaturaleza == detail.v_Codigo).ToList();
                                    var CuentasTenerCuenta = ListaFuncion.Where(o => Cuentasdetail.Select(x => x.v_NroCuenta).Contains(o.cuenta)).ToList();
                                    objReporte.Total = CuentasTenerCuenta.Sum(o => o.acumulado);
                                    objReporte.Mensual = CuentasTenerCuenta.Sum(o => o.mensual);

                                    objReporte.Mes1 = CuentasTenerCuenta.Sum(o => o.Mes1);
                                    objReporte.Mes2 = CuentasTenerCuenta.Sum(o => o.Mes2);
                                    objReporte.Mes3 = CuentasTenerCuenta.Sum(o => o.Mes3);

                                    objReporte.Mes4 = CuentasTenerCuenta.Sum(o => o.Mes4);
                                    objReporte.Mes5 = CuentasTenerCuenta.Sum(o => o.Mes5);
                                    objReporte.Mes6 = CuentasTenerCuenta.Sum(o => o.Mes6);
                                    objReporte.Mes7 = CuentasTenerCuenta.Sum(o => o.Mes7);
                                    objReporte.Mes8 = CuentasTenerCuenta.Sum(o => o.Mes8);
                                    objReporte.Mes9 = CuentasTenerCuenta.Sum(o => o.Mes9);
                                    objReporte.Mes10 = CuentasTenerCuenta.Sum(o => o.Mes10);
                                    objReporte.Mes11 = CuentasTenerCuenta.Sum(o => o.Mes11);
                                    objReporte.Mes12 = CuentasTenerCuenta.Sum(o => o.Mes12);
                                    int iMesDesde = int.Parse(MesInicio);
                                    int iMesHasta = int.Parse(MesFin);
                                    objReporte.NombreMes1 = CuentasTenerCuenta.FirstOrDefault() == null ? iMesDesde <= iMesHasta ? NombreMeses(iMesDesde) : "" : CuentasTenerCuenta.FirstOrDefault().NombreMes1;
                                    iMesDesde++;
                                    objReporte.NombreMes2 = CuentasTenerCuenta.FirstOrDefault() == null ? iMesDesde <= iMesHasta ? NombreMeses(iMesDesde) : "" : CuentasTenerCuenta.FirstOrDefault().NombreMes2;
                                    iMesDesde++;

                                    objReporte.NombreMes3 = CuentasTenerCuenta.FirstOrDefault() == null ? iMesDesde <= iMesHasta ? NombreMeses(iMesDesde) : "" : CuentasTenerCuenta.FirstOrDefault().NombreMes3;
                                    iMesDesde++;
                                    objReporte.NombreMes4 = CuentasTenerCuenta.FirstOrDefault() == null ? iMesDesde <= iMesHasta ? NombreMeses(iMesDesde) : "" : CuentasTenerCuenta.FirstOrDefault().NombreMes4;
                                    iMesDesde++;
                                    objReporte.NombreMes5 = CuentasTenerCuenta.FirstOrDefault() == null ? iMesDesde <= iMesHasta ? NombreMeses(iMesDesde) : "" : CuentasTenerCuenta.FirstOrDefault().NombreMes5;
                                    iMesDesde++;
                                    objReporte.NombreMes6 = CuentasTenerCuenta.FirstOrDefault() == null ? iMesDesde <= iMesHasta ? NombreMeses(iMesDesde) : "" : CuentasTenerCuenta.FirstOrDefault().NombreMes6;
                                    iMesDesde++;
                                    objReporte.NombreMes7 = CuentasTenerCuenta.FirstOrDefault() == null ? iMesDesde <= iMesHasta ? NombreMeses(iMesDesde) : "" : CuentasTenerCuenta.FirstOrDefault().NombreMes7;
                                    iMesDesde++;
                                    objReporte.NombreMes8 = CuentasTenerCuenta.FirstOrDefault() == null ? iMesDesde <= iMesHasta ? NombreMeses(iMesDesde) : "" : CuentasTenerCuenta.FirstOrDefault().NombreMes8;
                                    iMesDesde++;
                                    objReporte.NombreMes9 = CuentasTenerCuenta.FirstOrDefault() == null ? iMesDesde <= iMesHasta ? NombreMeses(iMesDesde) : "" : CuentasTenerCuenta.FirstOrDefault().NombreMes9;
                                    iMesDesde++;
                                    objReporte.NombreMes10 = CuentasTenerCuenta.FirstOrDefault() == null ? iMesDesde <= iMesHasta ? NombreMeses(iMesDesde) : "" : CuentasTenerCuenta.FirstOrDefault().NombreMes10;
                                    iMesDesde++;
                                    objReporte.NombreMes11 = CuentasTenerCuenta.FirstOrDefault() == null ? iMesDesde <= iMesHasta ? NombreMeses(iMesDesde) : "" : CuentasTenerCuenta.FirstOrDefault().NombreMes11;
                                    iMesDesde++;
                                    objReporte.NombreMes12 = CuentasTenerCuenta.FirstOrDefault() == null ? iMesDesde <= iMesHasta ? NombreMeses(iMesDesde) : "" : CuentasTenerCuenta.FirstOrDefault().NombreMes12;

                                    objReporte.NombreTotalesSubgrupo = grupSecundario.v_Codigo == "G11" || grupSecundario.v_Codigo == "G12" ? grupSecundario.v_NombreGrupo : "";
                                    objReporte.NombreTotalesGrupo = grupito.v_Nombre;
                                    objReporte.CodigoGrupo = grupito.v_Codigo;
                                    objReporte.CodigoGrupoReal = detail.v_Codigo;
                                    ListaFinal.Add(objReporte);
                                }
                            }
                        }
                    }

                    ListaFinal = ListaFinal.OrderBy(o => o.CodigoGrupo).ToList();
                    int ContadorGrupos = 1;
                    decimal TotalGrupo = 0;
                    decimal TotalGrupoMensual = 0;
                    foreach (var grupitos in GruposPrincipales.OrderBy(o => o.v_Codigo))
                    {
                        //var grupSumas = ListaFinal.Where(o => o.CodigoGrupo.StartsWith(grupitos.v_Codigo) && o.i_TipoOperacion == (int)TipoOperacionConfiguracionBalances.Suma).Sum(o => o.Total);
                        //var grupRestas = ListaFinal.Where(o => o.CodigoGrupo.StartsWith(grupitos.v_Codigo) && o.i_TipoOperacion == (int)TipoOperacionConfiguracionBalances.Resta).Sum(o => o.Total);
                        var grupTotales = ListaFinal.Where(o => o.CodigoGrupo.StartsWith(grupitos.v_Codigo)).Sum(o => o.Total);
                        var grupToralesMensual = ListaFinal.Where(o => o.CodigoGrupo.StartsWith(grupitos.v_Codigo)).Sum(o => o.Mensual);
                        TotalGrupo = ContadorGrupos == 1 ? grupTotales : TotalGrupo + grupTotales;
                        TotalGrupoMensual = ContadorGrupos == 1 ? grupToralesMensual : grupToralesMensual + grupToralesMensual;
                        var detallesAsociados = ListaFinal.Where(o => o.CodigoGrupo.StartsWith(grupitos.v_Codigo));

                        foreach (var item in ListaFinal)
                        {
                            if (item.CodigoGrupo.StartsWith(grupitos.v_Codigo))
                            {
                                item.TotalGrupo = TotalGrupo;
                                item.TotalGrupoMensual = TotalGrupoMensual;
                            }
                        }
                        ContadorGrupos++;
                    }


                    return ListaFinal.OrderBy(o => o.CodigoGrupoReal).ToList();
                }
            }
            catch (Exception ex)
            {


                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "ContabilidadBL.ReporteGananciasPerdidasporNaturalezaConfigurable()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }
        }

        public bool VerificarConfiguracionBalances(ref  OperationResult objOperationResult, string pMes)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    objOperationResult.Success = 1;
                    var TableConfiguracionBalances = (from a in dbContext.configuracionbalances


                                                      where a.i_Eliminado == 0 && a.v_Mes == pMes && a.v_Periodo == periodo
                                                      select a).ToList();

                    return TableConfiguracionBalances.Any();

                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "ContabilidadBL.VerificarConfiguracionBalances()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return false;

            }


        }

        public List<ReporteSituacionFinanciera> ReporteSituacionFinanciera(ref OperationResult objOperationResult, string pstrPeriodo, string pstrMes, int pIntNumeroDigitos, int pIntMoneda, int pImputables, List<ReporteHojaTrabajo> aptitudeCertificate, bool UsadoReporteNotas = false)
        {
            try
            {
                objOperationResult.Success = 1;
                List<ReporteSituacionFinanciera> ListaFinal = new List<ReporteSituacionFinanciera>();
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    //var aptitudeCertificate = new ContabilidadBL().ReporteHojaTrabajo(ref objOperationResult, pstrPeriodo, pstrMes, pIntNumeroDigitos, pIntMoneda, pImputables);
                    if (objOperationResult.Success == 0)
                    {
                        return null;
                    }
                    var ListaSituacionFinanciera = aptitudeCertificate.Select(o => new { cuenta = o.cuenta, activoInventario = o.activoInventario, pasivoInventario = o.pasivoInventario, imputable = o.imputable, naturaleza = o.Naturaleza, NombreCuenta = o.denominacion }).ToList();
                    string pMes = (int.Parse(pstrMes)).ToString();
                    var TableConfiguracionBalances = (from a in dbContext.configuracionbalances
                                                      join b in dbContext.datahierarchy on new { grupo = 172, eliminado = 0, nota = a.i_TipoNota.Value } equals new { grupo = b.i_GroupId, eliminado = b.i_IsDeleted.Value, nota = b.i_ItemId } into b_join
                                                      from b in b_join.DefaultIfEmpty()

                                                      where a.i_Eliminado == 0 && a.v_Mes == pMes && a.v_Periodo == pstrPeriodo
                                                      select new configuracionbalancesDto
                                                      {
                                                          v_TipoBalance = a.v_TipoBalance,
                                                          NombreNota = b.v_Value1,
                                                          v_Codigo = a.v_Codigo,
                                                          v_Nombre = a.v_Nombre,
                                                          v_NombreGrupo = a.v_NombreGrupo,
                                                          i_TipoNota = a.i_TipoNota,
                                                          DescripcionNota = b.v_Value2,
                                                          i_IdConfiguracionBalance = a.i_IdConfiguracionBalance,
                                                      }).ToList();

                    var Grupos = TableConfiguracionBalances.ToList().Where(o => o.v_TipoBalance == ((int)ConfiguracionBalances.SituacionFinanciera).ToString()).ToList();
                    var plancontable = dbContext.asientocontable.ToList().Where(o => o.i_Eliminado == 0 && o.v_Periodo == Globals.ClientSession.i_Periodo.ToString()).ToList();
                    List<configuracionbalancesDto> GruposPrincipales = new List<configuracionbalancesDto>();
                    GruposPrincipales = UsadoReporteNotas ? Grupos.Where(o => o.v_Codigo.Length == 2).ToList() : Grupos.Where(o => o.v_Codigo.Length == 2 && o.v_Nombre.Contains("ACTIVO")).ToList();
                    foreach (var grupito in GruposPrincipales)
                    {
                        var GruposSecundarios = Grupos.Where(o => o.v_Codigo.Length == grupito.v_Codigo.Length + 1 && o.v_Codigo.StartsWith(grupito.v_Codigo)).ToList();
                        if (!GruposSecundarios.Any())
                        {
                            ReporteSituacionFinanciera objReporte = new ReporteSituacionFinanciera();
                            objReporte.Grupo1 = grupito.v_Nombre;
                            objReporte.NombreTotalesGrupo = grupito.v_NombreGrupo;
                            objReporte.Grupo2 = "";
                            objReporte.NombreTotalesSubgrupo = "";
                            objReporte.Detalle = "";
                            objReporte.CodigoGrupo = grupito.v_Codigo;
                            objReporte.CodigoGrupoReal = grupito.v_Codigo;
                            objReporte.Total = 0;
                            ListaFinal.Add(objReporte);
                        }
                        else
                        {

                            foreach (var grupSecundario in GruposSecundarios)
                            {
                                var Detalles = Grupos.Where(o => o.v_Codigo.Length == grupSecundario.v_Codigo.Length + 1 && o.v_Codigo.StartsWith(grupSecundario.v_Codigo)).ToList();

                                if (!Detalles.Any())
                                {
                                    ReporteSituacionFinanciera objReporte = new ReporteSituacionFinanciera();
                                    objReporte.Grupo1 = grupito.v_Nombre;
                                    objReporte.Grupo2 = grupSecundario.v_Nombre;
                                    objReporte.Detalle = "";
                                    objReporte.Total = 0;
                                    objReporte.NombreTotalesGrupo = grupito.v_NombreGrupo;
                                    objReporte.NombreTotalesSubgrupo = grupSecundario.v_NombreGrupo;
                                    objReporte.CodigoGrupo = grupito.v_Codigo;
                                    objReporte.CodigoGrupoReal = grupito.v_Codigo;
                                    ListaFinal.Add(objReporte);

                                }
                                foreach (var detail in Detalles)
                                {

                                    if (UsadoReporteNotas)
                                    {
                                        var Cuentasdetail = plancontable.Where(o => o.v_CodigoSituacionFinaciera == detail.v_Codigo).ToList();
                                        var Cuentadetailcomas = plancontable.Where(o => o.v_CodigoSituacionFinaciera != null && o.v_CodigoSituacionFinaciera.Contains(detail.v_Codigo)).ToList();
                                        List<asientocontable> ListaCuentasComas = new List<asientocontable>();
                                        foreach (var cuentita in Cuentadetailcomas)
                                        {
                                            var CuentasContenidas = cuentita.v_CodigoSituacionFinaciera.Split(',');
                                            for (int i = 0; i < CuentasContenidas.Count(); i++)
                                            {
                                                if (CuentasContenidas[i] == detail.i_IdConfiguracionBalance.ToString())
                                                {
                                                    ListaCuentasComas.Add(cuentita);
                                                }
                                            }
                                        }

                                        Cuentasdetail = Cuentasdetail.Concat(ListaCuentasComas).ToList();
                                        var CuentasTenerCuenta = ListaSituacionFinanciera.Where(o => Cuentasdetail.Select(x => x.v_NroCuenta).Contains(o.cuenta)).ToList();
                                        foreach (var tc in CuentasTenerCuenta)
                                        {
                                            ReporteSituacionFinanciera objReporte = new ReporteSituacionFinanciera();
                                            objReporte.Grupo1 = grupito.v_Nombre;
                                            objReporte.Grupo2 = grupSecundario.v_Nombre;
                                            objReporte.Detalle = detail.v_Nombre;
                                            objReporte.Nota = detail.NombreNota + " .- " + objReporte.Detalle;
                                            objReporte.DescripcionNota = detail.DescripcionNota;
                                            objReporte.Cuenta = "";
                                            objReporte.Cuenta = tc.cuenta;
                                            objReporte.CuentaNombre = tc.NombreCuenta;
                                            objReporte.i_TipoNota = detail.i_TipoNota ?? -1;
                                            objReporte.Total = tc.activoInventario != 0 ? tc.activoInventario : tc.pasivoInventario * -1;
                                            objReporte.TotalReal = tc.activoInventario != 0 ? tc.activoInventario : tc.pasivoInventario;
                                            ListaFinal.Add(objReporte);
                                        }


                                    }
                                    else
                                    {
                                        ReporteSituacionFinanciera objReporte = new ReporteSituacionFinanciera();
                                        objReporte.Grupo1 = grupito.v_Nombre;
                                        objReporte.Grupo2 = grupSecundario.v_Nombre;
                                        objReporte.Detalle = detail.v_Nombre;
                                        objReporte.Nota = grupSecundario.NombreNota;
                                        objReporte.DescripcionNota = grupSecundario.DescripcionNota;
                                        //var Cuentasdetail = plancontable.Where(o => o.v_CodigoSituacionFinaciera == detail.v_Codigo).ToList();
                                        //var Cuentadetailcomas = plancontable.Where(o => o.v_CodigoSituacionFinaciera !=null && o.v_CodigoSituacionFinaciera.Contains(detail.v_Codigo)).ToList();
                                        var Cuentasdetail = plancontable.Where(o => o.v_CodigoSituacionFinaciera == detail.i_IdConfiguracionBalance.ToString()).ToList();
                                        var Cuentadetailcomas = plancontable.Where(o => o.v_CodigoSituacionFinaciera != null && o.v_CodigoSituacionFinaciera.Contains(detail.i_IdConfiguracionBalance.ToString())).ToList();
                                        List<asientocontable> ListaCuentasComas = new List<asientocontable>();
                                        foreach (var cuentita in Cuentadetailcomas)
                                        {
                                            var CuentasContenidas = cuentita.v_CodigoSituacionFinaciera.Split(',');
                                            for (int i = 0; i < CuentasContenidas.Count(); i++)
                                            {
                                                //if (CuentasContenidas[i] == detail.v_Codigo)
                                                if (CuentasContenidas[i] == detail.i_IdConfiguracionBalance.ToString())
                                                {
                                                    ListaCuentasComas.Add(cuentita);
                                                }
                                            }
                                        }



                                        Cuentasdetail = Cuentasdetail.Concat(ListaCuentasComas).ToList();
                                        var CuentasTenerCuenta = ListaSituacionFinanciera.Where(o => Cuentasdetail.Select(x => x.v_NroCuenta).Contains(o.cuenta)).ToList();
                                        objReporte.Cuenta = "";
                                        var Debe = CuentasTenerCuenta.Sum(o => o.activoInventario);
                                        var Haber = CuentasTenerCuenta.Sum(o => o.pasivoInventario);
                                        objReporte.Total = Debe - Haber;
                                        objReporte.TotalReal = objReporte.Total;
                                        objReporte.Total = objReporte.Total < 0 ? objReporte.Total * -1 : objReporte.Total;
                                        objReporte.Nota = detail.i_TipoNota == null ? "" : detail.i_TipoNota.ToString();
                                        objReporte.NombreTotalesGrupo = grupito.v_NombreGrupo;
                                        objReporte.NombreTotalesSubgrupo = grupSecundario.v_NombreGrupo;
                                        objReporte.CodigoGrupo = grupito.v_Codigo;
                                        objReporte.CodigoGrupoReal = detail.v_Codigo;
                                        ListaFinal.Add(objReporte);
                                    }


                                }
                            }
                        }
                    }

                    ListaFinal = ListaFinal.OrderBy(o => o.CodigoGrupo).ToList();
                    int ContadorGrupos = 1;
                    decimal TotalGrupo = 0;
                    foreach (var grupitos in GruposPrincipales.OrderBy(o => o.v_Codigo))
                    {
                        var grupTotales = ListaFinal.Where(o => o.CodigoGrupo != null && o.CodigoGrupo.StartsWith(grupitos.v_Codigo)).Sum(o => o.Total);
                        TotalGrupo = ContadorGrupos == 1 ? grupTotales : TotalGrupo + grupTotales;
                        var detallesAsociados = ListaFinal.Where(o => o.CodigoGrupo != null && o.CodigoGrupo.StartsWith(grupitos.v_Codigo));

                        foreach (var item in ListaFinal)
                        {
                            if (item.CodigoGrupo != null && item.CodigoGrupo.StartsWith(grupitos.v_Codigo))
                            {
                                item.TotalGrupo = TotalGrupo;
                            }
                        }
                        ContadorGrupos++;
                    }

                    if (UsadoReporteNotas)
                    {
                        var NotasSistemas = dbContext.datahierarchy.Where(o => o.i_GroupId == 172 && o.i_IsDeleted == 0).ToList();

                        var NotasFaltantes = NotasSistemas.Where(o => !ListaFinal.Select(x => x.i_TipoNota).Contains(o.i_ItemId)).ToList();
                        foreach (var faltante in NotasFaltantes)
                        {
                            ReporteSituacionFinanciera objReporte = new ReporteSituacionFinanciera();
                            objReporte.Nota = faltante.v_Value1 + " .-" + faltante.v_Field;
                            objReporte.i_TipoNota = faltante.i_ItemId;
                            objReporte.DescripcionNota = faltante.v_Value2;
                            objReporte.Cuenta = ".";
                            ListaFinal.Add(objReporte);

                        }
                        ListaFinal = ListaFinal.Where(o => !string.IsNullOrEmpty(o.Cuenta)).OrderBy(o => o.i_TipoNota).ToList();
                        return ListaFinal;
                        //return ListaFinal.OrderBy(o => o.i_TipoNota).ToList();
                    }
                    else
                    {
                        return ListaFinal.OrderBy(o => o.CodigoGrupoReal).ToList();
                    }
                }
            }
            catch (Exception ex)
            {


                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "ContabilidadBL.ReporteSituacionFinanciera()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }
        }


        public List<ReporteSituacionFinanciera2doInforme> ReporteSituacionFinanciera2DoInforme(ref OperationResult objOperationResult, string pstrPeriodo, string pstrMes, int pIntNumeroDigitos, int pIntMoneda, int pImputables, List<ReporteHojaTrabajo> aptitudeCertificate)
        {
            try
            {
                objOperationResult.Success = 1;
                string pMes = (int.Parse(pstrMes)).ToString();
                List<ReporteSituacionFinanciera2doInforme> ListaFinal = new List<ReporteSituacionFinanciera2doInforme>();
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    //var aptitudeCertificate = new ContabilidadBL().ReporteHojaTrabajo(ref objOperationResult, pstrPeriodo, pstrMes, pIntNumeroDigitos, pIntMoneda, pImputables);
                    if (objOperationResult.Success == 0)
                    {
                        return null;
                    }
                    var ListaSituacionFinanciera = aptitudeCertificate.Select(o => new { cuenta = o.cuenta, activoInventario = o.activoInventario, pasivoInventario = o.pasivoInventario, imputable = o.imputable, naturaleza = o.Naturaleza }).ToList();
                    var TableConfiguracionBalances = dbContext.configuracionbalances.Where(o => o.i_Eliminado == 0 && o.v_Mes == pMes && o.v_Periodo == pstrPeriodo).ToList().ToDTOs();
                    var Grupos = TableConfiguracionBalances.ToList().Where(o => o.v_TipoBalance == ((int)ConfiguracionBalances.SituacionFinanciera).ToString()).ToList();
                    var plancontable = dbContext.asientocontable.ToList().Where(o => o.i_Eliminado == 0 && o.v_Periodo == Globals.ClientSession.i_Periodo.ToString()).ToList();
                    var GruposPrincipales = Grupos.Where(o => o.v_Codigo.Length == 2 && (o.v_Nombre.Contains("PASIVO") || o.v_Nombre.Contains("PATRIMONIO"))).ToList();
                    var NombreTotalFictio = GruposPrincipales.Where(o => o.v_Codigo.Length == 2).ToList().OrderBy(o => o.v_Codigo).ToList();
                    var Nombre1 = NombreTotalFictio.Count == 2 ? NombreTotalFictio.FirstOrDefault().v_Nombre : "";
                    var Nombre2 = NombreTotalFictio.Count == 2 ? NombreTotalFictio.LastOrDefault().v_Nombre : "";
                    foreach (var grupito in GruposPrincipales)
                    {
                        var GruposSecundarios = Grupos.Where(o => o.v_Codigo.Length == grupito.v_Codigo.Length + 1 && o.v_Codigo.StartsWith(grupito.v_Codigo)).ToList();
                        if (!GruposSecundarios.Any())
                        {
                            ReporteSituacionFinanciera2doInforme objReporte = new ReporteSituacionFinanciera2doInforme();
                            objReporte.Grupo1SituacionFinanciera = grupito.v_Nombre;
                            objReporte.GrupoFicticioSituacionFinanciera = "";
                            objReporte.NombreTotalesGrupoSituacionFinanciera = grupito.v_NombreGrupo;
                            objReporte.Grupo2SituacionFinanciera = "";
                            objReporte.NombreTotalesSubgrupoSituacionFinanciera = "";
                            objReporte.DetalleSituacionFinanciera = "";
                            objReporte.CodigoGrupoSituacionFinanciera = grupito.v_Codigo;
                            objReporte.CodigoGrupoRealSituacionFinanciera = grupito.v_Codigo;
                            objReporte.TotalSituacionFinanciera = 0;
                            objReporte.TotalFictioSituacionFinanciera = 0;
                            ListaFinal.Add(objReporte);
                        }
                        else
                        {

                            foreach (var grupSecundario in GruposSecundarios)
                            {
                                var Detalles = Grupos.Where(o => o.v_Codigo.Length == grupSecundario.v_Codigo.Length + 1 && o.v_Codigo.StartsWith(grupSecundario.v_Codigo)).ToList();

                                if (!Detalles.Any())
                                {
                                    ReporteSituacionFinanciera2doInforme objReporte = new ReporteSituacionFinanciera2doInforme();
                                    objReporte.Grupo1SituacionFinanciera = grupito.v_Nombre;
                                    objReporte.Grupo2SituacionFinanciera = grupSecundario.v_Nombre;
                                    objReporte.DetalleSituacionFinanciera = "";
                                    objReporte.TotalSituacionFinanciera = 0;
                                    objReporte.GrupoFicticioSituacionFinanciera = "";
                                    objReporte.NombreTotalesGrupoSituacionFinanciera = grupito.v_NombreGrupo;
                                    objReporte.NombreTotalesSubgrupoSituacionFinanciera = grupSecundario.v_NombreGrupo;
                                    objReporte.CodigoGrupoSituacionFinanciera = grupito.v_Codigo;
                                    objReporte.CodigoGrupoRealSituacionFinanciera = grupito.v_Codigo;

                                    ListaFinal.Add(objReporte);

                                }
                                foreach (var detail in Detalles)
                                {


                                    ReporteSituacionFinanciera2doInforme objReporte = new ReporteSituacionFinanciera2doInforme();
                                    objReporte.Grupo1SituacionFinanciera = grupito.v_Nombre;
                                    objReporte.GrupoFicticioSituacionFinanciera = "";
                                    objReporte.Grupo2SituacionFinanciera = grupSecundario.v_Nombre;
                                    objReporte.DetalleSituacionFinanciera = detail.v_Nombre;
                                    var Cuentasdetail = plancontable.Where(o => o.v_CodigoSituacionFinaciera == detail.i_IdConfiguracionBalance.ToString()).ToList();
                                    var Cuentadetailcomas = plancontable.Where(o => o.v_CodigoSituacionFinaciera != null && o.v_CodigoSituacionFinaciera.Contains(detail.i_IdConfiguracionBalance.ToString())).ToList();
                                    List<asientocontable> ListaCuentasComas = new List<asientocontable>();
                                    foreach (var cuentita in Cuentadetailcomas)
                                    {
                                        var CuentasContenidas = cuentita.v_CodigoSituacionFinaciera.Split(',');
                                        for (int i = 0; i < CuentasContenidas.Count(); i++)
                                        {
                                            if (CuentasContenidas[i] == detail.i_IdConfiguracionBalance.ToString())
                                            {
                                                ListaCuentasComas.Add(cuentita);
                                            }
                                        }
                                    }
                                    decimal Debe = 0, Haber = 0;
                                    Cuentasdetail = Cuentasdetail.Concat(ListaCuentasComas).ToList();
                                    var CuentasTenerCuenta = ListaSituacionFinanciera.Where(o => Cuentasdetail.Select(x => x.v_NroCuenta).Contains(o.cuenta)).ToList();

                                    if (detail.v_Codigo == "G416")
                                    {

                                        var Activo = aptitudeCertificate.Where(o => o.imputable == 1).ToList().Sum(o => o.activoInventario);
                                        var Pasivo = aptitudeCertificate.Where(o => o.imputable == 1).ToList().Sum(o => o.pasivoInventario);
                                        Debe = Pasivo > Activo ? Pasivo - Activo : 0;
                                        Haber = Activo > Pasivo ? Activo - Pasivo : 0;
                                    }
                                    else
                                    {
                                        Debe = CuentasTenerCuenta.Sum(o => o.activoInventario);
                                        Haber = CuentasTenerCuenta.Sum(o => o.pasivoInventario);
                                    }
                                    objReporte.TotalSituacionFinanciera = Haber - Debe;
                                    objReporte.TotalSituacionFinancieraReal = objReporte.TotalSituacionFinanciera;
                                    objReporte.TotalSituacionFinanciera = objReporte.TotalSituacionFinanciera < 0 ? objReporte.TotalSituacionFinanciera * -1 : objReporte.TotalSituacionFinanciera;
                                    objReporte.Nota = detail.i_TipoNota == null ? "" : detail.i_TipoNota.ToString();
                                    objReporte.NombreTotalesGrupoSituacionFinanciera = grupito.v_NombreGrupo;
                                    objReporte.NombreTotalesSubgrupoSituacionFinanciera = grupSecundario.v_NombreGrupo;
                                    objReporte.CodigoGrupoSituacionFinanciera = grupito.v_Codigo;
                                    objReporte.CodigoGrupoRealSituacionFinanciera = detail.v_Codigo;
                                    objReporte.NombreTotalesGrupoFictioSituacionFinanciera = "TOTAL " + Nombre1 + " Y " + Nombre2;

                                    ListaFinal.Add(objReporte);
                                }
                            }
                        }
                    }

                    ListaFinal = ListaFinal.OrderBy(o => o.CodigoGrupoSituacionFinanciera).ToList();
                    int ContadorGrupos = 1;
                    decimal TotalGrupo = 0;
                    decimal TotalGrupoFictio = 0;

                    foreach (var grupitos in GruposPrincipales.OrderBy(o => o.v_Codigo))
                    {
                        var grupTotales = ListaFinal.Where(o => o.CodigoGrupoSituacionFinanciera.StartsWith(grupitos.v_Codigo)).Sum(o => o.TotalSituacionFinanciera);
                        var grupoFictio = ListaFinal.Sum(o => o.TotalSituacionFinanciera);
                        TotalGrupo = grupTotales;
                        TotalGrupoFictio = grupoFictio;
                        var detallesAsociados = ListaFinal.Where(o => o.CodigoGrupoSituacionFinanciera.StartsWith(grupitos.v_Codigo));

                        foreach (var item in ListaFinal)
                        {
                            if (item.CodigoGrupoSituacionFinanciera.StartsWith(grupitos.v_Codigo))
                            {
                                item.TotalGrupoSituacionFinanciera = TotalGrupo;
                            }
                            item.TotalFictioSituacionFinanciera = TotalGrupoFictio;
                        }
                        ContadorGrupos++;
                    }


                    return ListaFinal.OrderBy(o => o.CodigoGrupoRealSituacionFinanciera).ToList();
                }
            }
            catch (Exception ex)
            {


                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "ContabilidadBL.ReporteSituacionFinanciera()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }
        }


        public List<string> CuentasNoTomadasCuentaSituacionFinanciera(string pMes,List<ReporteHojaTrabajo> HojaTrabajo)
        {

            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                var pc = dbContext.asientocontable.ToList().Where(o => o.i_Eliminado == 0 && o.i_Imputable ==1 &&  o.v_Periodo == Globals.ClientSession.i_Periodo.ToString()).ToList();
                var configuraciones = dbContext.configuracionbalances.ToList().Where(o => o.v_TipoBalance == ((int)ConfiguracionBalances.SituacionFinanciera).ToString() && o.v_Periodo == periodo && o.v_Mes == pMes).ToList();
                var activospasivos = HojaTrabajo.Where(o => o.imputable ==1 && int.Parse(o.cuenta.Substring(0, 2)) >= 10 && int.Parse(o.cuenta.Substring(0, 2)) <= 59 &&  (o.activoInventario!=0 || o.pasivoInventario !=0) ).ToList().Select(o => o.cuenta).ToList();
                List<asientocontable> ListaCuentasComas = new List<asientocontable>();
                List<asientocontable> ListaFinal = new List<asientocontable>();
                foreach (var detail in configuraciones)
                {
                    var Cuentasdetail = pc.Where(o => o.v_CodigoSituacionFinaciera == detail.i_IdConfiguracionBalance.ToString ()).ToList().OrderBy (o=>o.v_NroCuenta).ToList ();
                    var Cuentadetailcomas = pc.Where(o => o.v_CodigoSituacionFinaciera != null && o.v_CodigoSituacionFinaciera.Contains(detail.i_IdConfiguracionBalance.ToString ())).ToList().OrderBy (o=>o.v_NroCuenta).ToList ();
                    
                    foreach (var cuentita in Cuentadetailcomas)
                    {
                        var CuentasContenidas = cuentita.v_CodigoSituacionFinaciera.Split(',');
                        for (int i = 0; i < CuentasContenidas.Count(); i++)
                        {
                            if (CuentasContenidas[i] == detail.i_IdConfiguracionBalance.ToString())
                            {
                                ListaCuentasComas.Add(cuentita);
                            }
                        }
                    }
                    ListaFinal= Cuentasdetail.Concat(ListaCuentasComas).ToList();

                }



                var CuentasTenerCuenta = ListaFinal.Select(o => o.v_NroCuenta).ToList().OrderBy(o=>o).ToList ();
                var ggh = ListaFinal.Where(o => o.v_NroCuenta == "4011101").ToList();
                var gg = activospasivos.Except(CuentasTenerCuenta).ToList();
                return gg;

            }
        }


        public List<ReporteGananciasPerdidasporFuncion> ReporteGananciasPerdidasporFuncionSBS(ref OperationResult objOperationResult, string pstrPeriodo, string pstrMes, int pIntNumeroDigitos, int pIntMoneda, int pImputables)
        {
            try
            {
                objOperationResult.Success = 1;
                AsientosContablesBL _objAsientoContableBL = new AsientosContablesBL();
                List<ReporteGananciasPerdidasporFuncion> ListaFinal = new List<ReporteGananciasPerdidasporFuncion>();
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                List<string> CuentasMayores = new List<string>();
                ReporteGananciasPerdidasporFuncion objReporte = new ReporteGananciasPerdidasporFuncion();
                List<string> ListaCuentasImputables = new List<string>();
                #region RecopilaCuentasExistentesMesRequerido
                var saldoscontables = _objAsientoContableBL.ObtenerSaldosContablesPorMes(ref  objOperationResult, int.Parse(pstrMes), int.Parse(pstrPeriodo));

                var CuentasMesRequerido = (from a in saldoscontables

                                           join b in dbContext.asientocontable on new { cuenta = a.v_NroCuenta, eliminado = 0, p = periodo }
                                                                                 equals new { cuenta = b.v_NroCuenta, eliminado = 0, p = b.v_Periodo } into b_join
                                           from b in b_join.DefaultIfEmpty()

                                           where (a.v_NroCuenta != null && a.v_NroCuenta != "" && a.v_NroCuenta != "*Cta Eliminada*") && a.v_NroCuenta != null

                                           orderby b.v_NroCuenta
                                           select new ReporteGananciasPerdidasporFuncion
                                           {
                                               pstrCuenta = a.v_NroCuenta.Trim(),
                                               pstrDenominacion = b == null ? "*No Existe*" : b.v_NombreCuenta.Trim(),
                                               mes = a.v_Mes,
                                               imputable = b == null ? -1 : b.i_Imputable.Value,
                                               ImporteDebeSoles = a.d_ImporteSolesD.Value,
                                               ImporteDebeDolares = a.d_ImporteDolaresD.Value,
                                               ImporteHaberSoles = a.d_ImporteSolesH.Value,
                                               ImporteHaberDolares = a.d_ImporteDolaresH.Value,
                                           }).ToList();



                var CuentasMesRequeridoGananciaFuncion = (from n in CuentasMesRequerido
                                                          where n.pstrCuenta.StartsWith("9") || n.pstrCuenta.StartsWith("5")

                                                          select n).ToList(); //la cuenta 69, toda la 7  menos 79, toda la 9


                #endregion

                foreach (var item in CuentasMesRequeridoGananciaFuncion.AsParallel())
                {
                    string NumCuenta = item.pstrCuenta.Substring(0, 2);
                    if (!CuentasMayores.Contains(NumCuenta))
                    {
                        CuentasMayores.Add(NumCuenta);
                    }
                }
                foreach (var item in CuentasMayores.AsParallel())
                {
                    objReporte = new ReporteGananciasPerdidasporFuncion();
                    int pIntDigitos = 2;
                    var ListaImputables = CuentasMesRequeridoGananciaFuncion.Where(x => x.pstrCuenta.StartsWith(item.Substring(0, 2))).Select(x => x.pstrCuenta);
                    foreach (var cuentaImputable in ListaImputables)
                    {
                        if (!ListaCuentasImputables.Contains(cuentaImputable))
                        {
                            ListaCuentasImputables.Add(cuentaImputable);
                        }
                    }

                    while (pIntDigitos <= pIntNumeroDigitos)
                    {

                        objReporte = new ReporteGananciasPerdidasporFuncion();
                        objReporte.pstrCuenta = CuentasMesRequeridoGananciaFuncion.Where(x => x.pstrCuenta.StartsWith(item)).Count() != 0 ? CuentasMesRequeridoGananciaFuncion.Where(x => x.pstrCuenta.StartsWith(item)).FirstOrDefault().pstrCuenta.Length >= pIntDigitos ? CuentasMesRequeridoGananciaFuncion.Where(x => x.pstrCuenta.StartsWith(item)).FirstOrDefault().pstrCuenta.Substring(0, pIntDigitos) : "*No Existe*" : "*No Existe*";
                        if (objReporte.pstrCuenta != "*No Existe*")
                        {
                            var CuentaImputable = NombreCuentaImputable(objReporte.pstrCuenta);
                            string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
                            int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
                            objReporte.pstrDenominacion = objReporte.pstrCuenta != "No Existe*" ? denominacion : "*No Existe*";
                            objReporte.MovimientoDebe = SumaImporteMovimientoPorFuncion(CuentasMesRequeridoGananciaFuncion, "D", pIntMoneda, objReporte.pstrCuenta);
                            objReporte.MovimientoHaber = SumaImporteMovimientoPorFuncion(CuentasMesRequeridoGananciaFuncion, "H", pIntMoneda, objReporte.pstrCuenta);
                            objReporte.perdida = objReporte.MovimientoDebe > objReporte.MovimientoHaber ? objReporte.MovimientoDebe - objReporte.MovimientoHaber : 0;
                            objReporte.gananacia = objReporte.MovimientoDebe < objReporte.MovimientoHaber ? (objReporte.MovimientoDebe - objReporte.MovimientoHaber) * -1 : 0;
                            objReporte.imputable = imputable;
                            ListaFinal.Add(objReporte);
                        }
                        pIntDigitos = pIntDigitos + 1;
                    }

                    int Contador = 1;
                    pIntDigitos = 2;
                    if (Contador < ListaCuentasImputables.Count())
                    {
                        foreach (var subCuenta in ListaCuentasImputables.AsParallel())
                        {
                            while (pIntDigitos <= pIntNumeroDigitos)
                            {
                                objReporte = new ReporteGananciasPerdidasporFuncion();
                                objReporte.pstrCuenta = CuentasMesRequeridoGananciaFuncion.Where(x => x.pstrCuenta.StartsWith(subCuenta)).Count() != 0 ? CuentasMesRequeridoGananciaFuncion.Where(x => x.pstrCuenta.StartsWith(subCuenta)).FirstOrDefault().pstrCuenta.Length >= pIntDigitos ? CuentasMesRequeridoGananciaFuncion.Where(x => x.pstrCuenta.StartsWith(subCuenta)).FirstOrDefault().pstrCuenta.Substring(0, pIntDigitos) : "*No Existe*" : "*No Existe*";
                                if (objReporte.pstrCuenta != "*No Existe*" && ListaFinal.Where(x => x.pstrCuenta == objReporte.pstrCuenta).Count() == 0)
                                {
                                    var CuentaImputable = NombreCuentaImputable(objReporte.pstrCuenta);
                                    string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
                                    int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
                                    objReporte.pstrDenominacion = objReporte.pstrCuenta != "No Existe*" ? denominacion : "*No Existe*";
                                    objReporte.MovimientoDebe = SumaImporteMovimientoPorFuncion(CuentasMesRequeridoGananciaFuncion, "D", pIntMoneda, objReporte.pstrCuenta);
                                    objReporte.MovimientoHaber = SumaImporteMovimientoPorFuncion(CuentasMesRequeridoGananciaFuncion, "H", pIntMoneda, objReporte.pstrCuenta);
                                    objReporte.perdida = objReporte.MovimientoDebe > objReporte.MovimientoHaber ? objReporte.MovimientoDebe - objReporte.MovimientoHaber : 0;
                                    objReporte.gananacia = objReporte.MovimientoDebe < objReporte.MovimientoHaber ? (objReporte.MovimientoDebe - objReporte.MovimientoHaber) * -1 : 0;
                                    objReporte.imputable = imputable;
                                    //if (objReporte.pstrCuenta != "*No Existe*" && ListaFinal.Where(x => x.pstrCuenta == objReporte.pstrCuenta).Count() == 0)
                                    //{
                                    ListaFinal.Add(objReporte);
                                }
                                pIntDigitos = pIntDigitos + 1;
                            }
                            pIntDigitos = 2;
                        }
                    }
                }
                if (pImputables == 1)
                {

                    // return ListaFinal.ToList().Where(y => y.imputable == 1).OrderBy(x => x.pstrCuenta).ToList().Where(x => x.MovimientoDebe != 0 || x.MovimientoHaber != 0 || x.perdida != 0 || x.gananacia != 0).ToList(); Antes del 9 de Mayo

                    return ListaFinal.ToList().Where(y => y.imputable == 1 || (y.imputable == 0 && y.pstrCuenta.Length == 2)).OrderBy(x => x.pstrCuenta).ToList().Where(x => x.MovimientoDebe != 0 || x.MovimientoHaber != 0 || x.perdida != 0 || x.gananacia != 0).ToList();
                }
                else
                {

                    return ListaFinal.OrderBy(x => x.pstrCuenta).ToList().Where(x => x.MovimientoDebe != 0 || x.MovimientoHaber != 0 || x.perdida != 0 || x.gananacia != 0).ToList();
                }

            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;
            }


        }


        public decimal SumaImporteMovimientoPorFuncion(List<ReporteGananciasPerdidasporFuncion> ListaTotal, string pstrNaturaleza, int pIntMoneda, string pstrCuenta)
        {
            var ListaImputables = ListaTotal.FindAll(x => x.pstrCuenta.StartsWith(pstrCuenta)).ToList();
            decimal ImporteMovimiento = 0;

            if (pIntMoneda == 1)
            {
                ImporteMovimiento = pstrNaturaleza == "D" ? ListaImputables.Sum(x => x.ImporteDebeSoles) : ListaImputables.Sum(x => x.ImporteHaberSoles);
            }
            else
            {
                ImporteMovimiento = pstrNaturaleza == "D" ? ListaImputables.Sum(x => x.ImporteDebeDolares) : ListaImputables.Sum(x => x.ImporteHaberDolares);
            }
            return ImporteMovimiento;
        }

        //public List<ReporteGananciasPerdidasporNaturaleza> ReporteGananciasPerdidasporNaturaleza(ref OperationResult objOperationResult, string pstrPeriodo, string pstrMes, int pIntNumeroDigitos, int pIntMoneda, int pImputables)
        //{
        //    try
        //    {
        //        objOperationResult.Success = 1;

        //        List<ReporteGananciasPerdidasporNaturaleza> ListaFinal = new List<ReporteGananciasPerdidasporNaturaleza>();
        //        SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
        //        List<string> CuentasMayores = new List<string>();
        //        List<string> ListaCuentasImputables = new List<string>();
        //        ReporteGananciasPerdidasporNaturaleza objReporte = new ReporteGananciasPerdidasporNaturaleza();

        //        #region RecopilaCuentasExistentesMesRequerido

        //        var CuentasMes = (from a in dbContext.saldoscontables

        //                          join b in dbContext.asientocontable on new { cuenta = a.v_NroCuenta, eliminado = 0, p = periodo }
        //                                                                equals new { cuenta = b.v_NroCuenta, eliminado = b.i_Eliminado.Value, p = b.v_Periodo } into b_join
        //                          from b in b_join.DefaultIfEmpty()

        //                          where a.v_Anio == pstrPeriodo && (a.v_NroCuenta != null || a.v_NroCuenta != "")  // && a.v_Mes == pstrMes  

        //                          orderby b.v_NroCuenta
        //                          select new ReporteGananciasPerdidasporNaturaleza
        //                          {
        //                              pstrCuenta = a.v_NroCuenta.Trim(),
        //                              pstrDenominacion = b == null ? "*No Existe*" : b.v_NombreCuenta.Trim(),
        //                              mes = a.v_Mes,
        //                              imputable = b == null ? -1 : b.i_Imputable.Value,
        //                              ImporteDebeSoles = a.d_ImporteSolesD.Value,
        //                              ImporteDebeDolares = a.d_ImporteDolaresD.Value,
        //                              ImporteHaberSoles = a.d_ImporteSolesH.Value,
        //                              ImporteHaberDolares = a.d_ImporteDolaresH.Value,
        //                          }).ToList();



        //        var CuentasMesRequerido = (from a in CuentasMes
        //                                   where int.Parse(a.mes) <= int.Parse(pstrMes)

        //                                   select new ReporteGananciasPerdidasporNaturaleza
        //                                   {
        //                                       pstrCuenta = a.pstrCuenta,
        //                                       pstrDenominacion = a.pstrDenominacion,
        //                                       mes = a.mes,
        //                                       imputable = a.imputable,
        //                                       ImporteDebeSoles = a.ImporteDebeSoles,
        //                                       ImporteDebeDolares = a.ImporteDebeDolares,
        //                                       ImporteHaberSoles = a.ImporteHaberSoles,
        //                                       ImporteHaberDolares = a.ImporteHaberDolares,
        //                                   }).ToList();


        //        var CuentasMesRequeridoGananciaNaturaleza = (from n in CuentasMesRequerido
        //                                                     where (int.Parse(n.pstrCuenta.Substring(0, 2)) >= 60 && int.Parse(n.pstrCuenta.Substring(0, 2)) <= 69) || (int.Parse(n.pstrCuenta.Substring(0, 2)) >= 70 && int.Parse(n.pstrCuenta.Substring(0, 2)) <= 78)

        //                                                     select n).ToList(); //De la 60-68 -- TODA la 70 menos 79


        //        #endregion

        //        foreach (var item in CuentasMesRequeridoGananciaNaturaleza.AsParallel())
        //        {
        //            string NumCuenta = item.pstrCuenta.Substring(0, 2);
        //            if (!CuentasMayores.Contains(NumCuenta))
        //            {
        //                CuentasMayores.Add(NumCuenta);
        //            }
        //        }
        //        foreach (var item in CuentasMayores.AsParallel())
        //        {
        //            objReporte = new ReporteGananciasPerdidasporNaturaleza();
        //            int pIntDigitos = 2;

        //            var ListaImputables = CuentasMesRequeridoGananciaNaturaleza.Where(x => x.pstrCuenta.StartsWith(item.Substring(0, 2))).Select(x => x.pstrCuenta);
        //            foreach (var cuentaImputable in ListaImputables)
        //            {
        //                if (!ListaCuentasImputables.Contains(cuentaImputable))
        //                {
        //                    ListaCuentasImputables.Add(cuentaImputable);
        //                }
        //            }

        //            while (pIntDigitos <= pIntNumeroDigitos)
        //            {

        //                objReporte = new ReporteGananciasPerdidasporNaturaleza();
        //                objReporte.pstrCuenta = CuentasMesRequeridoGananciaNaturaleza.Where(x => x.pstrCuenta.StartsWith(item)).Count() != 0 ? CuentasMesRequeridoGananciaNaturaleza.Where(x => x.pstrCuenta.StartsWith(item)).FirstOrDefault().pstrCuenta.Length >= pIntDigitos ? CuentasMesRequeridoGananciaNaturaleza.Where(x => x.pstrCuenta.StartsWith(item)).FirstOrDefault().pstrCuenta.Substring(0, pIntDigitos) : "*No Existe*" : "*No Existe*";

        //                if (objReporte.pstrCuenta != "*No Existe*")
        //                {
        //                    var CuentaImputable = NombreCuentaImputable(objReporte.pstrCuenta);
        //                    string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
        //                    int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
        //                    objReporte.pstrDenominacion = objReporte.pstrCuenta != "No Existe*" ? denominacion : "*No Existe*";
        //                    objReporte.MovimientoDebe = SumaImporteMovimientoPorNaturaleza(CuentasMesRequeridoGananciaNaturaleza, "D", pIntMoneda, objReporte.pstrCuenta, pImputables == 1 ? true : false);
        //                    objReporte.MovimientoHaber = SumaImporteMovimientoPorNaturaleza(CuentasMesRequeridoGananciaNaturaleza, "H", pIntMoneda, objReporte.pstrCuenta, pImputables == 1 ? true : false);
        //                    //objReporte.perdida = objReporte.pstrCuenta != "*No Existe*" ? objReporte.pstrCuenta.StartsWith("61") ? CuentaPerdidaNaturaleza61ReporteGananciaNaturaleza(objReporte.pstrCuenta, CuentasMesRequerido, pIntMoneda, "P") : objReporte.MovimientoDebe > objReporte.MovimientoHaber ? objReporte.MovimientoDebe - objReporte.MovimientoHaber : 0 : 0;
        //                    //objReporte.ganancia = objReporte.pstrCuenta != "*No Existe*" ? objReporte.pstrCuenta.StartsWith("61") ? CuentaPerdidaNaturaleza61ReporteGananciaNaturaleza(objReporte.pstrCuenta, CuentasMesRequerido, pIntMoneda, "G") : objReporte.MovimientoDebe < objReporte.MovimientoHaber ? (objReporte.MovimientoDebe - objReporte.MovimientoHaber) * -1 : 0 : 0;
        //                    objReporte.perdida = objReporte.pstrCuenta != "*No Existe*" ? objReporte.MovimientoDebe > objReporte.MovimientoHaber ? objReporte.MovimientoDebe - objReporte.MovimientoHaber : 0 : 0;
        //                    objReporte.ganancia = objReporte.pstrCuenta != "*No Existe*" ? objReporte.MovimientoDebe < objReporte.MovimientoHaber ? (objReporte.MovimientoDebe - objReporte.MovimientoHaber) * -1 : 0 : 0;
        //                    objReporte.imputable = imputable;
        //                    //pIntDigitos = pIntDigitos + 1;
        //                    //if (objReporte.pstrCuenta != "*No Existe*")
        //                    //{

        //                    ListaFinal.Add(objReporte);
        //                }
        //                pIntDigitos = pIntDigitos + 1;
        //            }

        //            int Contador = 1;
        //            pIntDigitos = 2;
        //            if (Contador < ListaCuentasImputables.Count())
        //            {
        //                foreach (var subCuenta in ListaCuentasImputables.AsParallel())
        //                {
        //                    while (pIntDigitos <= pIntNumeroDigitos)
        //                    {
        //                        objReporte = new ReporteGananciasPerdidasporNaturaleza();
        //                        objReporte.pstrCuenta = CuentasMesRequeridoGananciaNaturaleza.Where(x => x.pstrCuenta.StartsWith(subCuenta)).Count() != 0 ? CuentasMesRequeridoGananciaNaturaleza.Where(x => x.pstrCuenta.StartsWith(subCuenta)).FirstOrDefault().pstrCuenta.Length >= pIntDigitos ? CuentasMesRequeridoGananciaNaturaleza.Where(x => x.pstrCuenta.StartsWith(subCuenta)).FirstOrDefault().pstrCuenta.Substring(0, pIntDigitos) : "*No Existe*" : "*No Existe*";
        //                        if (objReporte.pstrCuenta != "*No Existe*" && ListaFinal.Where(x => x.pstrCuenta == objReporte.pstrCuenta).Count() == 0)
        //                        {
        //                            var CuentaImputable = NombreCuentaImputable(objReporte.pstrCuenta);
        //                            string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
        //                            int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
        //                            objReporte.pstrDenominacion = objReporte.pstrCuenta != "No Existe*" ? denominacion : "*No Existe*";
        //                            objReporte.MovimientoDebe = SumaImporteMovimientoPorNaturaleza(CuentasMesRequeridoGananciaNaturaleza, "D", pIntMoneda, objReporte.pstrCuenta, pImputables == 1 ? true : false);
        //                            objReporte.MovimientoHaber = SumaImporteMovimientoPorNaturaleza(CuentasMesRequeridoGananciaNaturaleza, "H", pIntMoneda, objReporte.pstrCuenta, pImputables == 1 ? true : false);

        //                            //objReporte.perdida = objReporte.pstrCuenta != "*No Existe*" ? objReporte.pstrCuenta.StartsWith("61") ? CuentaPerdidaNaturaleza61ReporteGananciaNaturaleza(objReporte.pstrCuenta, CuentasMesRequerido, pIntMoneda, "P") : objReporte.MovimientoDebe > objReporte.MovimientoHaber ? objReporte.MovimientoDebe - objReporte.MovimientoHaber : 0 : 0;
        //                            //objReporte.ganancia = objReporte.pstrCuenta != "*No Existe*" ? objReporte.pstrCuenta.StartsWith("61") ? CuentaPerdidaNaturaleza61ReporteGananciaNaturaleza(objReporte.pstrCuenta, CuentasMesRequerido, pIntMoneda, "G") : objReporte.MovimientoDebe < objReporte.MovimientoHaber ? (objReporte.MovimientoDebe - objReporte.MovimientoHaber) * -1 : 0 : 0;

        //                            objReporte.perdida = objReporte.pstrCuenta != "*No Existe*" ? objReporte.MovimientoDebe > objReporte.MovimientoHaber ? objReporte.MovimientoDebe - objReporte.MovimientoHaber : 0 : 0;
        //                            objReporte.ganancia = objReporte.pstrCuenta != "*No Existe*" ? objReporte.MovimientoDebe < objReporte.MovimientoHaber ? (objReporte.MovimientoDebe - objReporte.MovimientoHaber) * -1 : 0 : 0;


        //                            objReporte.imputable = imputable;

        //                            //if (objReporte.pstrCuenta != "*No Existe*" && ListaFinal.Where(x => x.pstrCuenta == objReporte.pstrCuenta).Count() == 0)
        //                            //{

        //                            ListaFinal.Add(objReporte);

        //                        }
        //                        pIntDigitos = pIntDigitos + 1;
        //                    }
        //                    pIntDigitos = 2;
        //                }
        //            }
        //        }
        //        if (pImputables == 1)
        //        {

        //            //  return ListaFinal.ToList().Where(y => y.imputable == 1).OrderBy(x => x.pstrCuenta).ToList().Where(x => x.MovimientoDebe != 0 || x.MovimientoHaber != 0 || x.perdida != 0 || x.ganancia != 0).ToList(); Antes del 9 Mayo

        //            return ListaFinal.ToList().Where(y => y.imputable == 1 || (y.imputable == 0 && y.pstrCuenta.Length == 2)).OrderBy(x => x.pstrCuenta).ToList().Where(x => x.MovimientoDebe != 0 || x.MovimientoHaber != 0 || x.perdida != 0 || x.ganancia != 0).ToList();
        //        }
        //        else
        //        {
        //            return ListaFinal.OrderBy(x => x.pstrCuenta).ToList().Where(x => x.MovimientoDebe != 0 || x.MovimientoHaber != 0 || x.perdida != 0 || x.ganancia != 0).ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objOperationResult.Success = 0;
        //        return null;
        //    }
        //}

        public List<ReporteGananciasPerdidasporNaturaleza> ReporteGananciasPerdidasporNaturalezaNuevo(ref OperationResult objOperationResult, string pstrPeriodo, string pstrMes, int pIntNumeroDigitos, int pIntMoneda, int pImputables)
        {
            try
            {
                objOperationResult.Success = 1;
                List<ReporteGananciasPerdidasporNaturaleza> ListaFinal = new List<ReporteGananciasPerdidasporNaturaleza>();
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                List<string> CuentasMayores = new List<string>();
                List<string> ListaCuentasImputables = new List<string>();
                ReporteGananciasPerdidasporNaturaleza objReporte = new ReporteGananciasPerdidasporNaturaleza();
                AsientosContablesBL _objAsientoContableBL = new AsientosContablesBL();
                #region RecopilaCuentasExistentesMesRequerido

                var saldoscontables = _objAsientoContableBL.ObtenerSaldosContablesPorMes(ref  objOperationResult, int.Parse(pstrMes), int.Parse(pstrPeriodo));
                var CuentasMesRequerido = (from a in saldoscontables

                                           join b in dbContext.asientocontable on new { cuenta = a.v_NroCuenta, eliminado = 0, p = periodo }
                                                                                 equals new { cuenta = b.v_NroCuenta, eliminado = b.i_Eliminado.Value, p = b.v_Periodo } into b_join
                                           from b in b_join.DefaultIfEmpty()

                                           where (a.v_NroCuenta != null && a.v_NroCuenta != "" && a.v_NroCuenta != "*Cta Eliminada*")  // && a.v_Mes == pstrMes  

                                           orderby b.v_NroCuenta
                                           select new ReporteGananciasPerdidasporNaturaleza
                                           {
                                               pstrCuenta = a.v_NroCuenta.Trim(),
                                               pstrDenominacion = b == null ? "*No Existe*" : b.v_NombreCuenta.Trim(),
                                               mes = a.v_Mes,
                                               imputable = b == null ? -1 : b.i_Imputable.Value,
                                               ImporteDebeSoles = a.d_ImporteSolesD.Value,
                                               ImporteDebeDolares = a.d_ImporteDolaresD.Value,
                                               ImporteHaberSoles = a.d_ImporteSolesH.Value,
                                               ImporteHaberDolares = a.d_ImporteDolaresH.Value,
                                               EsCuentaNaturaleza = CuentaPerdidaNaturaleza(a.v_NroCuenta.Trim()),
                                           }).ToList();


                //var CuentasMesRequeridoGananciaNaturaleza = (from n in CuentasMesRequerido
                //                                             where (int.Parse(n.pstrCuenta.Substring(0, 2)) >= 60 && int.Parse(n.pstrCuenta.Substring(0, 2)) <= 69) || (int.Parse(n.pstrCuenta.Substring(0, 2)) >= 70 && int.Parse(n.pstrCuenta.Substring(0, 2)) <= 78)

                //                                             select n).ToList(); //De la 60-69 -- TODA la 70 menos 79

                var CuentasMesRequeridoGananciaNaturaleza = CuentasMesRequerido.Where(o => o.EsCuentaNaturaleza).ToList();


                #endregion

                foreach (var item in CuentasMesRequeridoGananciaNaturaleza.AsParallel())
                {
                    string NumCuenta = item.pstrCuenta.Substring(0, 2);
                    if (!CuentasMayores.Contains(NumCuenta))
                    {
                        CuentasMayores.Add(NumCuenta);
                    }
                }
                foreach (var item in CuentasMayores.AsParallel())
                {
                    objReporte = new ReporteGananciasPerdidasporNaturaleza();
                    int pIntDigitos = 2;

                    var ListaImputables = CuentasMesRequeridoGananciaNaturaleza.Where(x => x.pstrCuenta.StartsWith(item.Substring(0, 2))).Select(x => x.pstrCuenta);
                    foreach (var cuentaImputable in ListaImputables)
                    {
                        if (!ListaCuentasImputables.Contains(cuentaImputable))
                        {
                            ListaCuentasImputables.Add(cuentaImputable);
                        }
                    }

                    while (pIntDigitos <= pIntNumeroDigitos)
                    {

                        objReporte = new ReporteGananciasPerdidasporNaturaleza();
                        objReporte.pstrCuenta = CuentasMesRequeridoGananciaNaturaleza.Where(x => x.pstrCuenta.StartsWith(item)).Count() != 0 ? CuentasMesRequeridoGananciaNaturaleza.Where(x => x.pstrCuenta.StartsWith(item)).FirstOrDefault().pstrCuenta.Length >= pIntDigitos ? CuentasMesRequeridoGananciaNaturaleza.Where(x => x.pstrCuenta.StartsWith(item)).FirstOrDefault().pstrCuenta.Substring(0, pIntDigitos) : "*No Existe*" : "*No Existe*";

                        if (objReporte.pstrCuenta != "*No Existe*")
                        {
                            var CuentaImputable = NombreCuentaImputable(objReporte.pstrCuenta);
                            string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
                            int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
                            objReporte.pstrDenominacion = objReporte.pstrCuenta != "No Existe*" ? denominacion : "*No Existe*";
                            objReporte.MovimientoDebe = SumaImporteMovimientoPorNaturaleza(CuentasMesRequeridoGananciaNaturaleza, "D", pIntMoneda, objReporte.pstrCuenta, pImputables == 1 ? true : false);
                            objReporte.MovimientoHaber = SumaImporteMovimientoPorNaturaleza(CuentasMesRequeridoGananciaNaturaleza, "H", pIntMoneda, objReporte.pstrCuenta, pImputables == 1 ? true : false);
                            objReporte.perdida = objReporte.pstrCuenta != "*No Existe*" ? objReporte.MovimientoDebe > objReporte.MovimientoHaber ? objReporte.MovimientoDebe - objReporte.MovimientoHaber : 0 : 0;
                            objReporte.ganancia = objReporte.pstrCuenta != "*No Existe*" ? objReporte.MovimientoDebe < objReporte.MovimientoHaber ? (objReporte.MovimientoDebe - objReporte.MovimientoHaber) * -1 : 0 : 0;
                            objReporte.imputable = imputable;


                            ListaFinal.Add(objReporte);
                        }
                        pIntDigitos = pIntDigitos + 1;
                    }

                    int Contador = 1;
                    pIntDigitos = 2;
                    if (Contador < ListaCuentasImputables.Count())
                    {
                        foreach (var subCuenta in ListaCuentasImputables.AsParallel())
                        {
                            while (pIntDigitos <= pIntNumeroDigitos)
                            {
                                objReporte = new ReporteGananciasPerdidasporNaturaleza();
                                objReporte.pstrCuenta = CuentasMesRequeridoGananciaNaturaleza.Where(x => x.pstrCuenta.StartsWith(subCuenta)).Count() != 0 ? CuentasMesRequeridoGananciaNaturaleza.Where(x => x.pstrCuenta.StartsWith(subCuenta)).FirstOrDefault().pstrCuenta.Length >= pIntDigitos ? CuentasMesRequeridoGananciaNaturaleza.Where(x => x.pstrCuenta.StartsWith(subCuenta)).FirstOrDefault().pstrCuenta.Substring(0, pIntDigitos) : "*No Existe*" : "*No Existe*";
                                if (objReporte.pstrCuenta != "*No Existe*" && ListaFinal.Where(x => x.pstrCuenta == objReporte.pstrCuenta).Count() == 0)
                                {
                                    var CuentaImputable = NombreCuentaImputable(objReporte.pstrCuenta);
                                    string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
                                    int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
                                    objReporte.pstrDenominacion = objReporte.pstrCuenta != "No Existe*" ? denominacion : "*No Existe*";
                                    objReporte.MovimientoDebe = SumaImporteMovimientoPorNaturaleza(CuentasMesRequeridoGananciaNaturaleza, "D", pIntMoneda, objReporte.pstrCuenta, pImputables == 1 ? true : false);
                                    objReporte.MovimientoHaber = SumaImporteMovimientoPorNaturaleza(CuentasMesRequeridoGananciaNaturaleza, "H", pIntMoneda, objReporte.pstrCuenta, pImputables == 1 ? true : false);
                                    objReporte.perdida = objReporte.pstrCuenta != "*No Existe*" ? objReporte.MovimientoDebe > objReporte.MovimientoHaber ? objReporte.MovimientoDebe - objReporte.MovimientoHaber : 0 : 0;
                                    objReporte.ganancia = objReporte.pstrCuenta != "*No Existe*" ? objReporte.MovimientoDebe < objReporte.MovimientoHaber ? (objReporte.MovimientoDebe - objReporte.MovimientoHaber) * -1 : 0 : 0;
                                    objReporte.imputable = imputable;
                                    ListaFinal.Add(objReporte);

                                }
                                pIntDigitos = pIntDigitos + 1;
                            }
                            pIntDigitos = 2;
                        }
                    }
                }
                if (pImputables == 1)
                {
                    return ListaFinal.ToList().Where(y => y.imputable == 1 || (y.imputable == 0 && y.pstrCuenta.Length == 2)).OrderBy(x => x.pstrCuenta).ToList().Where(x => x.MovimientoDebe != 0 || x.MovimientoHaber != 0 || x.perdida != 0 || x.ganancia != 0).ToList();
                }
                else
                {
                    return ListaFinal.OrderBy(x => x.pstrCuenta).ToList().Where(x => x.MovimientoDebe != 0 || x.MovimientoHaber != 0 || x.perdida != 0 || x.ganancia != 0).ToList();
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;
            }
        }



        public List<ReporteGananciasPerdidasporNaturaleza> ReporteGananciasPerdidasporNaturalezaSBS(ref OperationResult objOperationResult, string pstrPeriodo, string pstrMes, int pIntNumeroDigitos, int pIntMoneda, int pImputables)
        {
            try
            {
                objOperationResult.Success = 1;

                List<ReporteGananciasPerdidasporNaturaleza> ListaFinal = new List<ReporteGananciasPerdidasporNaturaleza>();
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                List<string> CuentasMayores = new List<string>();
                List<string> ListaCuentasImputables = new List<string>();
                ReporteGananciasPerdidasporNaturaleza objReporte = new ReporteGananciasPerdidasporNaturaleza();
                AsientosContablesBL _objAsientoContableBL = new AsientosContablesBL();
                #region RecopilaCuentasExistentesMesRequerido

                var saldoscontables = _objAsientoContableBL.ObtenerSaldosContablesPorMes(ref  objOperationResult, int.Parse(pstrMes), int.Parse(pstrPeriodo));
                var CuentasMesRequerido = (from a in saldoscontables

                                           join b in dbContext.asientocontable on new { cuenta = a.v_NroCuenta, eliminado = 0, p = periodo }
                                                                                 equals new { cuenta = b.v_NroCuenta, eliminado = b.i_Eliminado.Value, p = b.v_Periodo } into b_join
                                           from b in b_join.DefaultIfEmpty()

                                           where (a.v_NroCuenta != null && a.v_NroCuenta != "" && a.v_NroCuenta != "*Cta Eliminada*")  // && a.v_Mes == pstrMes  

                                           orderby b.v_NroCuenta
                                           select new ReporteGananciasPerdidasporNaturaleza
                                           {
                                               pstrCuenta = a.v_NroCuenta.Trim(),
                                               pstrDenominacion = b == null ? "*No Existe*" : b.v_NombreCuenta.Trim(),
                                               mes = a.v_Mes,
                                               imputable = b == null ? -1 : b.i_Imputable.Value,
                                               ImporteDebeSoles = a.d_ImporteSolesD.Value,
                                               ImporteDebeDolares = a.d_ImporteDolaresD.Value,
                                               ImporteHaberSoles = a.d_ImporteSolesH.Value,
                                               ImporteHaberDolares = a.d_ImporteDolaresH.Value,
                                           }).ToList();


                var CuentasMesRequeridoGananciaNaturaleza = (from n in CuentasMesRequerido
                                                             where n.pstrCuenta.StartsWith("4") || n.pstrCuenta.StartsWith("5")

                                                             select n).ToList();


                #endregion

                foreach (var item in CuentasMesRequeridoGananciaNaturaleza.AsParallel())
                {
                    string NumCuenta = item.pstrCuenta.Substring(0, 2);
                    if (!CuentasMayores.Contains(NumCuenta))
                    {
                        CuentasMayores.Add(NumCuenta);
                    }
                }
                foreach (var item in CuentasMayores.AsParallel())
                {
                    objReporte = new ReporteGananciasPerdidasporNaturaleza();
                    int pIntDigitos = 2;

                    var ListaImputables = CuentasMesRequeridoGananciaNaturaleza.Where(x => x.pstrCuenta.StartsWith(item.Substring(0, 2))).Select(x => x.pstrCuenta);
                    foreach (var cuentaImputable in ListaImputables)
                    {
                        if (!ListaCuentasImputables.Contains(cuentaImputable))
                        {
                            ListaCuentasImputables.Add(cuentaImputable);
                        }
                    }

                    while (pIntDigitos <= pIntNumeroDigitos)
                    {

                        objReporte = new ReporteGananciasPerdidasporNaturaleza();
                        objReporte.pstrCuenta = CuentasMesRequeridoGananciaNaturaleza.Where(x => x.pstrCuenta.StartsWith(item)).Count() != 0 ? CuentasMesRequeridoGananciaNaturaleza.Where(x => x.pstrCuenta.StartsWith(item)).FirstOrDefault().pstrCuenta.Length >= pIntDigitos ? CuentasMesRequeridoGananciaNaturaleza.Where(x => x.pstrCuenta.StartsWith(item)).FirstOrDefault().pstrCuenta.Substring(0, pIntDigitos) : "*No Existe*" : "*No Existe*";

                        if (objReporte.pstrCuenta != "*No Existe*")
                        {
                            var CuentaImputable = NombreCuentaImputable(objReporte.pstrCuenta);
                            string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
                            int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
                            objReporte.pstrDenominacion = objReporte.pstrCuenta != "No Existe*" ? denominacion : "*No Existe*";
                            objReporte.MovimientoDebe = SumaImporteMovimientoPorNaturaleza(CuentasMesRequeridoGananciaNaturaleza, "D", pIntMoneda, objReporte.pstrCuenta, pImputables == 1 ? true : false);
                            objReporte.MovimientoHaber = SumaImporteMovimientoPorNaturaleza(CuentasMesRequeridoGananciaNaturaleza, "H", pIntMoneda, objReporte.pstrCuenta, pImputables == 1 ? true : false);
                            objReporte.perdida = objReporte.pstrCuenta != "*No Existe*" ? objReporte.MovimientoDebe > objReporte.MovimientoHaber ? objReporte.MovimientoDebe - objReporte.MovimientoHaber : 0 : 0;
                            objReporte.ganancia = objReporte.pstrCuenta != "*No Existe*" ? objReporte.MovimientoDebe < objReporte.MovimientoHaber ? (objReporte.MovimientoDebe - objReporte.MovimientoHaber) * -1 : 0 : 0;
                            objReporte.imputable = imputable;


                            ListaFinal.Add(objReporte);
                        }
                        pIntDigitos = pIntDigitos + 1;
                    }

                    int Contador = 1;
                    pIntDigitos = 2;
                    if (Contador < ListaCuentasImputables.Count())
                    {
                        foreach (var subCuenta in ListaCuentasImputables.AsParallel())
                        {
                            while (pIntDigitos <= pIntNumeroDigitos)
                            {
                                objReporte = new ReporteGananciasPerdidasporNaturaleza();
                                objReporte.pstrCuenta = CuentasMesRequeridoGananciaNaturaleza.Where(x => x.pstrCuenta.StartsWith(subCuenta)).Count() != 0 ? CuentasMesRequeridoGananciaNaturaleza.Where(x => x.pstrCuenta.StartsWith(subCuenta)).FirstOrDefault().pstrCuenta.Length >= pIntDigitos ? CuentasMesRequeridoGananciaNaturaleza.Where(x => x.pstrCuenta.StartsWith(subCuenta)).FirstOrDefault().pstrCuenta.Substring(0, pIntDigitos) : "*No Existe*" : "*No Existe*";
                                if (objReporte.pstrCuenta != "*No Existe*" && ListaFinal.Where(x => x.pstrCuenta == objReporte.pstrCuenta).Count() == 0)
                                {
                                    var CuentaImputable = NombreCuentaImputable(objReporte.pstrCuenta);
                                    string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
                                    int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
                                    objReporte.pstrDenominacion = objReporte.pstrCuenta != "No Existe*" ? denominacion : "*No Existe*";
                                    objReporte.MovimientoDebe = SumaImporteMovimientoPorNaturaleza(CuentasMesRequeridoGananciaNaturaleza, "D", pIntMoneda, objReporte.pstrCuenta, pImputables == 1 ? true : false);
                                    objReporte.MovimientoHaber = SumaImporteMovimientoPorNaturaleza(CuentasMesRequeridoGananciaNaturaleza, "H", pIntMoneda, objReporte.pstrCuenta, pImputables == 1 ? true : false);
                                    objReporte.perdida = objReporte.pstrCuenta != "*No Existe*" ? objReporte.MovimientoDebe > objReporte.MovimientoHaber ? objReporte.MovimientoDebe - objReporte.MovimientoHaber : 0 : 0;
                                    objReporte.ganancia = objReporte.pstrCuenta != "*No Existe*" ? objReporte.MovimientoDebe < objReporte.MovimientoHaber ? (objReporte.MovimientoDebe - objReporte.MovimientoHaber) * -1 : 0 : 0;
                                    objReporte.imputable = imputable;
                                    ListaFinal.Add(objReporte);

                                }
                                pIntDigitos = pIntDigitos + 1;
                            }
                            pIntDigitos = 2;
                        }
                    }
                }
                if (pImputables == 1)
                {
                    return ListaFinal.ToList().Where(y => y.imputable == 1 || (y.imputable == 0 && y.pstrCuenta.Length == 2)).OrderBy(x => x.pstrCuenta).ToList().Where(x => x.MovimientoDebe != 0 || x.MovimientoHaber != 0 || x.perdida != 0 || x.ganancia != 0).ToList();
                }
                else
                {
                    return ListaFinal.OrderBy(x => x.pstrCuenta).ToList().Where(x => x.MovimientoDebe != 0 || x.MovimientoHaber != 0 || x.perdida != 0 || x.ganancia != 0).ToList();
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;
            }
        }


        public decimal CuentaPerdidaNaturaleza61ReporteGananciaNaturaleza(string pstrCuenta, List<ReporteGananciasPerdidasporNaturaleza> ReportePN, int pIntMoneda, string pstrNaturaleza)
        {
            try
            {
                decimal SumaDebe, SumaHaber, saldoDeudor, saldoAcreedor, perdida, ganancia;
                if (pstrCuenta.Length > 2)
                {
                    string pstrComplemento = pstrCuenta.Substring(2, (pstrCuenta.Length - 2));
                    string pstrcuentaNueva = "69" + pstrComplemento;
                    SumaDebe = pIntMoneda == 1 ? ReportePN.FindAll(y => y.pstrCuenta.Length >= pstrCuenta.Length).FindAll(x => x.pstrCuenta.Substring(0, pstrCuenta.Length) == pstrcuentaNueva).ToList().Sum(y => y.ImporteDebeSoles) : ReportePN.FindAll(x => x.pstrCuenta.Substring(0, pstrCuenta.Length) == pstrcuentaNueva).ToList().Sum(y => y.ImporteDebeDolares);
                    SumaHaber = pIntMoneda == 1 ? ReportePN.FindAll(y => y.pstrCuenta.Length >= pstrCuenta.Length).FindAll(x => x.pstrCuenta.Substring(0, pstrCuenta.Length) == pstrcuentaNueva).ToList().Sum(y => y.ImporteHaberSoles) : ReportePN.FindAll(x => x.pstrCuenta.Substring(0, pstrCuenta.Length) == pstrcuentaNueva).ToList().Sum(y => y.ImporteHaberDolares);
                    saldoDeudor = SumaDebe > SumaHaber ? SumaDebe - SumaHaber : 0;
                    saldoAcreedor = SumaDebe < SumaHaber ? (SumaDebe - SumaHaber) * -1 : 0;
                    perdida = saldoDeudor;
                    ganancia = saldoAcreedor;
                }

                else
                {
                    SumaDebe = pIntMoneda == 1 ? ReportePN.FindAll(x => x.pstrCuenta.Substring(0, 2) == "69").ToList().Sum(x => x.ImporteDebeSoles) : ReportePN.FindAll(x => x.pstrCuenta.Substring(0, 2) == "69").ToList().Sum(x => x.ImporteDebeDolares);
                    SumaHaber = pIntMoneda == 1 ? ReportePN.FindAll(x => x.pstrCuenta.Substring(0, 2) == "69").ToList().Sum(x => x.ImporteHaberSoles) : ReportePN.FindAll(x => x.pstrCuenta.Substring(0, 2) == "69").ToList().Sum(x => x.ImporteHaberDolares);
                    saldoDeudor = SumaDebe > SumaHaber ? SumaDebe - SumaHaber : 0;
                    saldoAcreedor = SumaDebe < SumaHaber ? (SumaDebe - SumaHaber) * -1 : 0;
                    perdida = saldoDeudor;
                    ganancia = saldoAcreedor;

                }
                if (pstrNaturaleza == "P")
                {
                    return perdida;
                }
                else
                {
                    return ganancia;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public decimal SumaImporteMovimientoPorNaturaleza(List<ReporteGananciasPerdidasporNaturaleza> ListaTotal, string pstrNaturaleza, int pIntMoneda, string pstrCuenta, bool ReporteImputables)
        {


            //var ListaImputables = ReporteImputables ? ListaTotal.FindAll(x => x.pstrCuenta == pstrCuenta).ToList() : ListaTotal.FindAll(x => x.pstrCuenta.StartsWith(pstrCuenta)).ToList(); Antes del 9 Mayo
            var ListaImputables = ListaTotal.FindAll(x => x.pstrCuenta.StartsWith(pstrCuenta)).ToList();
            decimal ImporteMovimiento = 0;
            if (pIntMoneda == 1)
            {
                ImporteMovimiento = pstrNaturaleza == "D" ? ListaImputables.Sum(x => x.ImporteDebeSoles) : ListaImputables.Sum(x => x.ImporteHaberSoles);
            }
            else
            {
                ImporteMovimiento = pstrNaturaleza == "D" ? ListaImputables.Sum(x => x.ImporteDebeDolares) : ListaImputables.Sum(x => x.ImporteHaberDolares);
            }
            return ImporteMovimiento;
        }

        //public List<ReporteBalanceComprobacion> ReporteBalanceComprobacion(ref OperationResult objOperationResult, string pstrPeriodo, string pstrMes, int pIntNumeroDigitos, int pIntMoneda, int pIntCuentasImputables, int pIntCuentasGastos79)
        //{
        //    //SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
        //    // List<ReporteBalanceComprobacion> ListaFinal = new List<ReporteBalanceComprobacion>();
        //    // Para cualquier mes ,siempre se presenta los asientos de apertura
        //    // Las sumas es del movimiento del Mes,pero no se tiene en cuenta el asiento de apertura
        //    // return ListaFinal;
        //    try
        //    {

        //        objOperationResult.Success = 1;
        //        List<ReporteBalanceComprobacion> ListaFinal = new List<ReporteBalanceComprobacion>();
        //        List<ReporteBalanceComprobacion> AsientoApertura = new List<ReporteBalanceComprobacion>();
        //        List<ReporteBalanceComprobacion> CuentasAnteriores = new List<ReporteBalanceComprobacion>();
        //        ReporteBalanceComprobacion objReporte = new ReporteBalanceComprobacion();
        //        List<ReporteBalanceComprobacion> CuentasTotales = new List<ReporteBalanceComprobacion>();
        //        List<string> ListaCuentasImputables = new List<string>();

        //        // string MesAnterior = pstrMes != "01" ? (int.Parse(pstrMes) - 1).ToString("00") : pstrMes;

        //        string MesAnterior = pstrMes;

        //        using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
        //        {
        //            #region Recopila Información de AsientoApertura
        //            AsientoApertura = (from a in dbContext.diario

        //                               join b in dbContext.diariodetalle on new { IdDiario = a.v_IdDiario, eliminado = 0 }
        //                                                                equals new { IdDiario = b.v_IdDiario, eliminado = b.i_Eliminado.Value } into b_join
        //                               from b in b_join.DefaultIfEmpty()

        //                               join c in dbContext.asientocontable on new { asiento = b.v_NroCuenta, eliminado = 0, p = periodo }
        //                                                                equals new { asiento = c.v_NroCuenta, eliminado = c.i_Eliminado.Value, p = c.v_Periodo } into c_join

        //                               from c in c_join.DefaultIfEmpty()

        //                               where a.v_Periodo == pstrPeriodo && a.i_IdTipoComprobante == 1 && a.i_Eliminado == 0 && (b.v_NroCuenta != null || b.v_NroCuenta != "") && a.v_Mes == "01"

        //                               orderby b.v_NroCuenta
        //                               select new ReporteBalanceComprobacion
        //                               {

        //                                   pstrCuenta = b.v_NroCuenta.Trim(),
        //                                   pstrDenominacion = c == null ? "*No Existe*" : c.v_NombreCuenta.Trim(),
        //                                   mes = a.v_Mes,
        //                                   imputable = c == null ? -1 : c.i_Imputable.Value,
        //                                   ImporteDebeSoles = a.i_IdMoneda == 1 && b.v_Naturaleza == "D" ? b.d_Importe.Value : a.i_IdMoneda == 2 && b.v_Naturaleza == "D" ? b.d_Cambio.Value : 0,
        //                                   ImporteDebeDolares = a.i_IdMoneda == 2 && b.v_Naturaleza == "D" ? b.d_Importe.Value : a.i_IdMoneda == 1 && b.v_Naturaleza == "D" ? b.d_Cambio.Value : 0,
        //                                   ImporteHaberSoles = a.i_IdMoneda == 1 && b.v_Naturaleza == "H" ? b.d_Importe.Value : a.i_IdMoneda == 2 && b.v_Naturaleza == "H" ? b.d_Cambio.Value : 0,
        //                                   ImporteHaberDolares = a.i_IdMoneda == 2 && b.v_Naturaleza == "H" ? b.d_Importe.Value : a.i_IdMoneda == 1 && b.v_Naturaleza == "H" ? b.d_Cambio.Value : 0,

        //                               }).ToList();

        //            #endregion


        //            var CuentasAnterioresMes = (from a in dbContext.saldoscontables

        //                                        join b in dbContext.asientocontable on new { cuenta = a.v_NroCuenta, eliminado = 0, p = periodo }
        //                                                                            equals new { cuenta = b.v_NroCuenta, eliminado = b.i_Eliminado.Value, p = b.v_Periodo } into b_join
        //                                        from b in b_join.DefaultIfEmpty()

        //                                        where a.v_Anio == pstrPeriodo && (a.v_NroCuenta != null || a.v_NroCuenta != "")

        //                                        orderby a.v_NroCuenta

        //                                        select new ReporteBalanceComprobacion
        //                                        {
        //                                            pstrCuenta = a.v_NroCuenta.Trim(),
        //                                            pstrDenominacion = b == null ? "*No Existe*" : b.v_NombreCuenta.Trim(),
        //                                            mes = a.v_Mes,
        //                                            imputable = b == null ? -1 : b.i_Imputable.Value,
        //                                            ImporteDebeSoles = a.d_ImporteSolesD.Value,
        //                                            ImporteDebeDolares = a.d_ImporteDolaresD.Value,
        //                                            ImporteHaberSoles = a.d_ImporteSolesH.Value,
        //                                            ImporteHaberDolares = a.d_ImporteDolaresH.Value,

        //                                        }).ToList();




        //            CuentasAnteriores = (from a in CuentasAnterioresMes

        //                                 where int.Parse(a.mes) <= int.Parse(MesAnterior)
        //                                 select new ReporteBalanceComprobacion
        //                                 {
        //                                     pstrCuenta = a.pstrCuenta,
        //                                     pstrDenominacion = a.pstrDenominacion,
        //                                     mes = a.mes,
        //                                     imputable = a.imputable,
        //                                     ImporteDebeSoles = a.ImporteDebeSoles,
        //                                     ImporteDebeDolares = a.ImporteDebeDolares,
        //                                     ImporteHaberSoles = a.ImporteHaberSoles,
        //                                     ImporteHaberDolares = a.ImporteHaberDolares,

        //                                 }).ToList();

        //        }

        //        #region RecopilaCuentasExistentesMesRequerido
        //        CuentasTotales = AsientoApertura.Concat(CuentasAnteriores).ToList();
        //        var CuentasMayores = CuentasTotales.Select(x => x.pstrCuenta).Select(x => x.Substring(0, 2)).Distinct();

        //        foreach (var item in CuentasMayores.AsParallel())
        //        {
        //            objReporte = new ReporteBalanceComprobacion();
        //            int pIntDigitos = 2;
        //            var ListaEmpiezaCuenta = CuentasTotales.Where(x => x.pstrCuenta.StartsWith(item.Substring(0, 2))).Select(y => y.pstrCuenta);
        //            while (pIntDigitos <= pIntNumeroDigitos)
        //            {

        //                objReporte = new ReporteBalanceComprobacion();
        //                objReporte.pstrCuenta = CuentasTotales.Where(x => x.pstrCuenta.StartsWith(item)).Count() != 0 ? CuentasTotales.Where(x => x.pstrCuenta.StartsWith(item)).FirstOrDefault().pstrCuenta.Length >= pIntDigitos ? CuentasTotales.Where(x => x.pstrCuenta.StartsWith(item)).FirstOrDefault().pstrCuenta.Substring(0, pIntDigitos) : "*No Existe*" : "*No Existe*";

        //                if (objReporte.pstrCuenta != "*No Existe*" && ListaFinal.Where(x => x.pstrCuenta == objReporte.pstrCuenta).Count() == 0)
        //                {
        //                    var CuentaImputable = NombreCuentaImputable(objReporte.pstrCuenta);
        //                    string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
        //                    int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
        //                    objReporte.pstrDenominacion = objReporte.pstrCuenta != "No Existe*" ? denominacion : "*No Existe*";
        //                    objReporte.imputable = imputable;
        //                    objReporte.debeApertura = pIntMoneda == (int)Currency.Soles ? AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeSoles) : AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeDolares);
        //                    objReporte.haberApertura = pIntMoneda == (int)Currency.Soles ? AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberSoles) : AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberDolares);

        //                    if (pIntCuentasGastos79 == 1)
        //                    {
        //                        objReporte.Grupo2 = objReporte.pstrCuenta.StartsWith("9") || objReporte.pstrCuenta.StartsWith("79") ? "Grupo2" : "";
        //                        objReporte.Grupo1 = !objReporte.pstrCuenta.StartsWith("9") && !objReporte.pstrCuenta.StartsWith("79") ? "Grupo1" : "";
        //                    }
        //                    else
        //                    {
        //                        objReporte.Grupo2 = objReporte.pstrCuenta.StartsWith("9") ? "Grupo2" : "";
        //                        objReporte.Grupo1 = !objReporte.pstrCuenta.StartsWith("9") ? "Grupo1" : "";
        //                    }

        //                    objReporte.debeSumaAcumulada = pIntMoneda == (int)Currency.Soles ? CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeSoles) - AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeSoles) : CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeDolares) - AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeDolares);
        //                    objReporte.haberSumaAcumulada = pIntMoneda == (int)Currency.Soles ? CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberSoles) - AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberSoles) : CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberDolares) - AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberDolares);
        //                    //objReporte.debeSumaAcumulada = pIntCuentasImputables == 1 ? pIntMoneda == (int)Currency.Soles ? CuentasTotales.Where(x => x.pstrCuenta == objReporte.pstrCuenta).Sum(x => x.ImporteDebeSoles) - AsientoApertura.Where(x => x.pstrCuenta == objReporte.pstrCuenta).Sum(x => x.ImporteDebeSoles) : CuentasTotales.Where(x => x.pstrCuenta == objReporte.pstrCuenta).Sum(x => x.ImporteDebeDolares) - AsientoApertura.Where(x => x.pstrCuenta == objReporte.pstrCuenta).Sum(x => x.ImporteDebeDolares) : pIntMoneda == (int)Currency.Soles ? CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeSoles) - AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeSoles) : CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeDolares) - AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeDolares);
        //                    //objReporte.haberSumaAcumulada = pIntCuentasImputables == 1 ? pIntMoneda == (int)Currency.Soles ? CuentasTotales.Where(x => x.pstrCuenta == objReporte.pstrCuenta).Sum(x => x.ImporteHaberSoles) - AsientoApertura.Where(x => x.pstrCuenta == objReporte.pstrCuenta).Sum(x => x.ImporteHaberSoles) : CuentasTotales.Where(x => x.pstrCuenta == objReporte.pstrCuenta).Sum(x => x.ImporteHaberDolares) - AsientoApertura.Where(x => x.pstrCuenta == objReporte.pstrCuenta).Sum(x => x.ImporteHaberDolares) : pIntMoneda == (int)Currency.Soles ? CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberSoles) - AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberSoles) : CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberDolares) - AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberDolares);
        //                    decimal MovMesDebe = 0, AperturaDebe = 0, TotalDebe = 0, deudorActual = 0, acreedorActual = 0, AperturaHaber = 0, MovMesHaber = 0, TotalHaber = 0, DeudorFinal = 0, AcreedorFinal = 0;
        //                    //   var ListaImputables = pIntCuentasImputables == 1 ? CuentasTotales.Where(x => x.pstrCuenta == objReporte.pstrCuenta).ToList() : CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).ToList(); //Antes del 9 Mayo
        //                    var ListaImputables = CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).ToList();


        //                    ListaCuentasImputables = new List<string>();
        //                    foreach (var cuenta in ListaImputables)
        //                    {
        //                        if (!ListaCuentasImputables.Contains(cuenta.pstrCuenta))
        //                        {
        //                            ListaCuentasImputables.Add(cuenta.pstrCuenta);
        //                        }
        //                    }

        //                    foreach (var cuenta in ListaCuentasImputables.AsParallel())
        //                    {

        //                        if (pIntMoneda == (int)Currency.Soles)
        //                        {
        //                            //AperturaDebe = AsientoApertura.FindAll(x => x.pstrCuenta.StartsWith(cuenta)).Sum(x => x.ImporteDebeSoles);
        //                            //MovMesDebe = CuentasTotales.FindAll(x => x.pstrCuenta.StartsWith(cuenta)).Sum(x => x.ImporteDebeSoles);
        //                            //AperturaHaber = AsientoApertura.FindAll(x => x.pstrCuenta.StartsWith(cuenta)).Sum(x => x.ImporteHaberSoles);
        //                            //MovMesHaber = CuentasTotales.FindAll(x => x.pstrCuenta.StartsWith(cuenta)).Sum(x => x.ImporteHaberSoles);

        //                            AperturaDebe = AsientoApertura.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteDebeSoles);
        //                            MovMesDebe = CuentasTotales.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteDebeSoles);
        //                            AperturaHaber = AsientoApertura.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteHaberSoles);
        //                            MovMesHaber = CuentasTotales.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteHaberSoles);
        //                        }
        //                        else
        //                        {

        //                            //AperturaDebe = AsientoApertura.FindAll(x => x.pstrCuenta.StartsWith(cuenta)).Sum(x => x.ImporteDebeDolares);
        //                            //MovMesDebe = CuentasTotales.FindAll(x => x.pstrCuenta.StartsWith(cuenta)).Sum(x => x.ImporteDebeDolares);
        //                            //AperturaHaber = AsientoApertura.FindAll(x => x.pstrCuenta.StartsWith(cuenta)).Sum(x => x.ImporteHaberDolares);
        //                            //MovMesHaber = CuentasTotales.FindAll(x => x.pstrCuenta.StartsWith(cuenta)).Sum(x => x.ImporteHaberDolares);

        //                            AperturaDebe = AsientoApertura.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteDebeDolares);
        //                            MovMesDebe = CuentasTotales.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteDebeDolares);
        //                            AperturaHaber = AsientoApertura.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteHaberDolares);
        //                            MovMesHaber = CuentasTotales.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteHaberDolares);

        //                        }

        //                        TotalDebe = MovMesDebe - AperturaDebe;
        //                        TotalHaber = MovMesHaber - AperturaHaber;
        //                        deudorActual = TotalDebe > TotalHaber ? TotalDebe - TotalHaber : 0;
        //                        acreedorActual = TotalHaber > TotalDebe ? TotalHaber - TotalDebe : 0;
        //                        DeudorFinal = deudorActual + DeudorFinal;
        //                        AcreedorFinal = acreedorActual + AcreedorFinal;
        //                    }

        //                    //objReporte.deudor = DeudorFinal;
        //                    //objReporte.acreedor = AcreedorFinal;
        //                    objReporte.deudor = DeudorFinal > AcreedorFinal ? DeudorFinal - AcreedorFinal : 0;//Antes del 20 de marzo
        //                    objReporte.acreedor = AcreedorFinal > DeudorFinal ? AcreedorFinal - DeudorFinal : 0;
        //                    ListaFinal.Add(objReporte);

        //                }

        //                pIntDigitos = pIntDigitos + 1;
        //            }


        //            int Contador = 1;
        //            pIntDigitos = 2;
        //            if (Contador < ListaEmpiezaCuenta.Count())
        //            {
        //                foreach (var subCuenta in ListaEmpiezaCuenta.AsParallel())
        //                {
        //                    while (pIntDigitos <= pIntNumeroDigitos)
        //                    {
        //                        objReporte = new ReporteBalanceComprobacion();
        //                        objReporte.pstrCuenta = subCuenta.StartsWith(item) && subCuenta.Length >= pIntDigitos ? subCuenta.Substring(0, pIntDigitos) : "*No Existe*";
        //                        if (objReporte.pstrCuenta != "*No Existe*" && ListaFinal.Where(x => x.pstrCuenta == objReporte.pstrCuenta).Count() == 0)
        //                        {

        //                            var CuentaImputable = NombreCuentaImputable(objReporte.pstrCuenta);
        //                            string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
        //                            int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
        //                            objReporte.pstrDenominacion = objReporte.pstrCuenta != "No Existe*" ? denominacion : "*No Existe*";
        //                            objReporte.imputable = imputable;

        //                            objReporte.debeApertura = pIntMoneda == (int)Currency.Soles ? AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeSoles) : AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeDolares);
        //                            objReporte.haberApertura = pIntMoneda == (int)Currency.Soles ? AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberSoles) : AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberDolares);


        //                            if (pIntCuentasGastos79 == 1)
        //                            {
        //                                objReporte.Grupo2 = objReporte.pstrCuenta.StartsWith("9") || objReporte.pstrCuenta.StartsWith("79") ? "Grupo2" : "";
        //                                objReporte.Grupo1 = !objReporte.pstrCuenta.StartsWith("9") && !objReporte.pstrCuenta.StartsWith("79") ? "Grupo1" : "";
        //                            }
        //                            else
        //                            {
        //                                objReporte.Grupo2 = objReporte.pstrCuenta.StartsWith("9") ? "Grupo2" : "";
        //                                objReporte.Grupo1 = !objReporte.pstrCuenta.StartsWith("9") ? "Grupo1" : "";
        //                            }

        //                            //objReporte.debeSumaAcumulada = pIntCuentasImputables == 1 ? pIntMoneda == (int)Currency.Soles ? CuentasTotales.Where(x => x.pstrCuenta == objReporte.pstrCuenta).Sum(x => x.ImporteDebeSoles) - AsientoApertura.Where(x => x.pstrCuenta == objReporte.pstrCuenta).Sum(x => x.ImporteDebeSoles) : CuentasTotales.Where(x => x.pstrCuenta == objReporte.pstrCuenta).Sum(x => x.ImporteDebeDolares) - AsientoApertura.Where(x => x.pstrCuenta == objReporte.pstrCuenta).Sum(x => x.ImporteDebeDolares) : pIntMoneda == (int)Currency.Soles ? CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeSoles) - AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeSoles) : CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeDolares) - AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeDolares);
        //                            //objReporte.haberSumaAcumulada = pIntCuentasImputables == 1 ? pIntMoneda == (int)Currency.Soles ? CuentasTotales.Where(x => x.pstrCuenta == objReporte.pstrCuenta).Sum(x => x.ImporteHaberSoles) - AsientoApertura.Where(x => x.pstrCuenta == objReporte.pstrCuenta).Sum(x => x.ImporteHaberSoles) : CuentasTotales.Where(x => x.pstrCuenta == objReporte.pstrCuenta).Sum(x => x.ImporteHaberDolares) - AsientoApertura.Where(x => x.pstrCuenta == objReporte.pstrCuenta).Sum(x => x.ImporteHaberDolares) : pIntMoneda == (int)Currency.Soles ? CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberSoles) - AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberSoles) : CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberDolares) - AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberDolares);

        //                            objReporte.debeSumaAcumulada = pIntMoneda == (int)Currency.Soles ? CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeSoles) - AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeSoles) : CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeDolares) - AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeDolares);
        //                            objReporte.haberSumaAcumulada = pIntMoneda == (int)Currency.Soles ? CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberSoles) - AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberSoles) : CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberDolares) - AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberDolares);

        //                            decimal MovMesDebe = 0, AperturaDebe = 0, TotalDebe = 0, deudorActual = 0, acreedorActual = 0, AperturaHaber = 0, MovMesHaber = 0, TotalHaber = 0, DeudorFinal = 0, AcreedorFinal = 0;
        //                            //  var ListaImputables = pIntCuentasImputables == 1 ? CuentasTotales.Where(x => x.pstrCuenta == objReporte.pstrCuenta).ToList() : CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).ToList(); Antes del 9 Mayo

        //                            var ListaImputables = CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).ToList();
        //                            ListaCuentasImputables = new List<string>();
        //                            foreach (var cuenta in ListaImputables)
        //                            {
        //                                if (!ListaCuentasImputables.Contains(cuenta.pstrCuenta))
        //                                {
        //                                    ListaCuentasImputables.Add(cuenta.pstrCuenta);
        //                                }
        //                            }

        //                            foreach (var cuenta in ListaCuentasImputables.AsParallel())
        //                            {

        //                                if (pIntMoneda == (int)Currency.Soles)
        //                                {
        //                                    //AperturaDebe = AsientoApertura.FindAll(x => x.pstrCuenta.StartsWith(cuenta)).Sum(x => x.ImporteDebeSoles);
        //                                    //MovMesDebe = CuentasTotales.FindAll(x => x.pstrCuenta.StartsWith(cuenta)).Sum(x => x.ImporteDebeSoles);
        //                                    //AperturaHaber = AsientoApertura.FindAll(x => x.pstrCuenta.StartsWith(cuenta)).Sum(x => x.ImporteHaberSoles);
        //                                    //MovMesHaber = CuentasTotales.FindAll(x => x.pstrCuenta.StartsWith(cuenta)).Sum(x => x.ImporteHaberSoles);

        //                                    AperturaDebe = AsientoApertura.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteDebeSoles);
        //                                    MovMesDebe = CuentasTotales.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteDebeSoles);
        //                                    AperturaHaber = AsientoApertura.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteHaberSoles);
        //                                    MovMesHaber = CuentasTotales.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteHaberSoles);
        //                                }
        //                                else
        //                                {

        //                                    //AperturaDebe = AsientoApertura.FindAll(x => x.pstrCuenta.StartsWith(cuenta)).Sum(x => x.ImporteDebeDolares);
        //                                    //MovMesDebe = CuentasTotales.FindAll(x => x.pstrCuenta.StartsWith(cuenta)).Sum(x => x.ImporteDebeDolares);
        //                                    //AperturaHaber = AsientoApertura.FindAll(x => x.pstrCuenta.StartsWith(cuenta)).Sum(x => x.ImporteHaberDolares);
        //                                    //MovMesHaber = CuentasTotales.FindAll(x => x.pstrCuenta.StartsWith(cuenta)).Sum(x => x.ImporteHaberDolares);
        //                                    AperturaDebe = AsientoApertura.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteDebeDolares);
        //                                    MovMesDebe = CuentasTotales.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteDebeDolares);
        //                                    AperturaHaber = AsientoApertura.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteHaberDolares);
        //                                    MovMesHaber = CuentasTotales.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteHaberDolares);

        //                                }

        //                                TotalDebe = MovMesDebe - AperturaDebe;
        //                                TotalHaber = MovMesHaber - AperturaHaber;
        //                                deudorActual = TotalDebe > TotalHaber ? TotalDebe - TotalHaber : 0;
        //                                acreedorActual = TotalHaber > TotalDebe ? TotalHaber - TotalDebe : 0;
        //                                DeudorFinal = deudorActual + DeudorFinal;
        //                                AcreedorFinal = acreedorActual + AcreedorFinal;
        //                            }

        //                            //objReporte.deudor = DeudorFinal;
        //                            //objReporte.acreedor = AcreedorFinal;
        //                            objReporte.deudor = DeudorFinal > AcreedorFinal ? DeudorFinal - AcreedorFinal : 0;// Antes del 20 de marzo
        //                            objReporte.acreedor = AcreedorFinal > DeudorFinal ? AcreedorFinal - DeudorFinal : 0;

        //                            ListaFinal.Add(objReporte);

        //                        }
        //                        pIntDigitos = pIntDigitos + 1;
        //                    }
        //                    pIntDigitos = 2;
        //                }
        //            }
        //        }
        //        if (pIntCuentasImputables == 1)
        //        {


        //            return ListaFinal.ToList().Where(y => y.imputable == 1 || (y.imputable == 0 && y.pstrCuenta.Length == 2)).OrderBy(x => x.pstrCuenta).ToList().Where(x => x.debeApertura != 0 || x.haberApertura != 0 || x.debeSumaAcumulada != 0 || x.haberSumaAcumulada != 0 || x.deudor != 0 || x.acreedor != 0).ToList();
        //        }
        //        else
        //        {

        //            return ListaFinal.OrderBy(x => x.pstrCuenta).ToList().Where(x => x.debeApertura != 0 || x.haberApertura != 0 || x.debeSumaAcumulada != 0 || x.haberSumaAcumulada != 0 || x.deudor != 0 || x.acreedor != 0).ToList();
        //        }

        //        #endregion

        //    }
        //    catch (Exception ex)
        //    {
        //        objOperationResult.Success = 0;
        //        return null;
        //    }
        //}

        public List<ReporteBalanceComprobacion> ReporteBalanceComprobacionNuevo(ref OperationResult objOperationResult, string pstrPeriodo, string pstrMes, int pIntNumeroDigitos, int pIntMoneda, int pIntCuentasImputables, int pIntCuentasGastos79)
        {

            // Para cualquier mes ,siempre se presenta los asientos de apertura
            // Las sumas es del movimiento del Mes,pero no se tiene en cuenta el asiento de apertura

            try
            {
                AsientosContablesBL _objAsientoContableBL = new AsientosContablesBL();
                objOperationResult.Success = 1;
                List<ReporteBalanceComprobacion> ListaFinal = new List<ReporteBalanceComprobacion>();
                List<ReporteBalanceComprobacion> AsientoApertura = new List<ReporteBalanceComprobacion>();
                List<ReporteBalanceComprobacion> CuentasAnteriores = new List<ReporteBalanceComprobacion>();
                ReporteBalanceComprobacion objReporte = new ReporteBalanceComprobacion();
                List<ReporteBalanceComprobacion> CuentasTotales = new List<ReporteBalanceComprobacion>();
                List<string> ListaCuentasImputables = new List<string>();

                // string MesAnterior = pstrMes != "01" ? (int.Parse(pstrMes) - 1).ToString("00") : pstrMes;

                string MesAnterior = pstrMes;

                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region Recopila Información de AsientoApertura
                    AsientoApertura = (from a in dbContext.diario

                                       join b in dbContext.diariodetalle on new { IdDiario = a.v_IdDiario, eliminado = 0 }
                                                                        equals new { IdDiario = b.v_IdDiario, eliminado = b.i_Eliminado.Value } into b_join
                                       from b in b_join.DefaultIfEmpty()

                                       join c in dbContext.asientocontable on new { asiento = b.v_NroCuenta, eliminado = 0, p = pstrPeriodo }
                                                                        equals new { asiento = c.v_NroCuenta, eliminado = c.i_Eliminado.Value, p = c.v_Periodo } into c_join

                                       from c in c_join.DefaultIfEmpty()

                                       where a.v_Periodo == pstrPeriodo && a.i_IdTipoComprobante == 1 && a.i_Eliminado == 0 && (b.v_NroCuenta != null || b.v_NroCuenta != "")// && a.v_Mes == "01"

                                       orderby b.v_NroCuenta
                                       select new ReporteBalanceComprobacion
                                       {

                                           pstrCuenta = b.v_NroCuenta.Trim(),
                                           pstrDenominacion = c == null ? "*No Existe*" : c.v_NombreCuenta.Trim(),
                                           mes = a.v_Mes,
                                           imputable = c == null ? -1 : c.i_Imputable.Value,
                                           ImporteDebeSoles = a.i_IdMoneda == 1 && b.v_Naturaleza == "D" ? b.d_Importe.Value : a.i_IdMoneda == 2 && b.v_Naturaleza == "D" ? b.d_Cambio.Value : 0,
                                           ImporteDebeDolares = a.i_IdMoneda == 2 && b.v_Naturaleza == "D" ? b.d_Importe.Value : a.i_IdMoneda == 1 && b.v_Naturaleza == "D" ? b.d_Cambio.Value : 0,
                                           ImporteHaberSoles = a.i_IdMoneda == 1 && b.v_Naturaleza == "H" ? b.d_Importe.Value : a.i_IdMoneda == 2 && b.v_Naturaleza == "H" ? b.d_Cambio.Value : 0,
                                           ImporteHaberDolares = a.i_IdMoneda == 2 && b.v_Naturaleza == "H" ? b.d_Importe.Value : a.i_IdMoneda == 1 && b.v_Naturaleza == "H" ? b.d_Cambio.Value : 0,

                                       }).ToList();

                    #endregion

                    var saldoscontables = _objAsientoContableBL.ObtenerSaldosContablesPorMes(ref  objOperationResult, int.Parse(MesAnterior), int.Parse(pstrPeriodo), 1, false, false, null, false, null, null, true);

                    CuentasAnteriores = (from a in saldoscontables

                                         join b in dbContext.asientocontable on new { cuenta = a.v_NroCuenta, eliminado = 0, p = periodo }
                                                                             equals new { cuenta = b.v_NroCuenta, eliminado = b.i_Eliminado.Value, p = b.v_Periodo } into b_join
                                         from b in b_join.DefaultIfEmpty()

                                         where (a.v_NroCuenta != null && a.v_NroCuenta != "" && a.v_NroCuenta != "*Cta Eliminada*")

                                         orderby a.v_NroCuenta

                                         select new ReporteBalanceComprobacion
                                         {
                                             pstrCuenta = a.v_NroCuenta.Trim(),
                                             pstrDenominacion = b == null ? "*No Existe*" : b.v_NombreCuenta.Trim(),
                                             mes = a.v_Mes,
                                             imputable = b == null ? -1 : b.i_Imputable.Value,
                                             ImporteDebeSoles = a.d_ImporteSolesD.Value,
                                             ImporteDebeDolares = a.d_ImporteDolaresD.Value,
                                             ImporteHaberSoles = a.d_ImporteSolesH.Value,
                                             ImporteHaberDolares = a.d_ImporteDolaresH.Value,

                                         }).ToList();


                }

                #region RecopilaCuentasExistentesMesRequerido
                CuentasTotales = CuentasAnteriores;
                var ListaTodasMovimientos = CuentasAnteriores.Concat(AsientoApertura).ToList();

                var CuentasApertura = AsientoApertura.Select(o => o.pstrCuenta).Select(x => x.Substring(0, 2)).Distinct();
                var CuentaTotales = CuentasTotales.Select(o => o.pstrCuenta).Select(x => x.Substring(0, 2)).Distinct();
                //var CuentasMayores = CuentasTotales.Concat(AsientoApertura).Select(x => x.pstrCuenta).Select(x => x.Substring(0, 2)).Distinct();

                var CuentasMayores = CuentasApertura.Concat(CuentaTotales).Distinct().ToList();
                foreach (var item in CuentasMayores.AsParallel())
                {
                    objReporte = new ReporteBalanceComprobacion();
                    int pIntDigitos = 2;
                    //var ListaEmpiezaCuenta = CuentaTotales.Where(x => x.pstrCuenta.StartsWith(item.Substring(0, 2))).Select(y => y.pstrCuenta); // Se cambio cuentasTotales por ListaMovimientos
                    var ListaEmpiezaCuenta = ListaTodasMovimientos.Where(x => x.pstrCuenta.StartsWith(item.Substring(0, 2))).Select(y => y.pstrCuenta);
                    while (pIntDigitos <= pIntNumeroDigitos)
                    {

                        objReporte = new ReporteBalanceComprobacion();
                        objReporte.pstrCuenta = ListaTodasMovimientos.Where(x => x.pstrCuenta.StartsWith(item)).Count() != 0 ? ListaTodasMovimientos.Where(x => x.pstrCuenta.StartsWith(item)).FirstOrDefault().pstrCuenta.Length >= pIntDigitos ? ListaTodasMovimientos.Where(x => x.pstrCuenta.StartsWith(item)).FirstOrDefault().pstrCuenta.Substring(0, pIntDigitos) : "*No Existe*" : "*No Existe*";

                        if (objReporte.pstrCuenta == "5011101")
                        {

                            string h = "";
                        }
                        if (objReporte.pstrCuenta != "*No Existe*" && ListaFinal.Where(x => x.pstrCuenta == objReporte.pstrCuenta).Count() == 0)
                        {
                            var CuentaImputable = NombreCuentaImputable(objReporte.pstrCuenta);
                            string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
                            int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
                            objReporte.pstrDenominacion = objReporte.pstrCuenta != "No Existe*" ? denominacion : "*No Existe*";
                            objReporte.imputable = imputable;
                            objReporte.debeApertura = pIntMoneda == (int)Currency.Soles ? AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeSoles) : AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeDolares);
                            objReporte.haberApertura = pIntMoneda == (int)Currency.Soles ? AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberSoles) : AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberDolares);

                            if (pIntCuentasGastos79 == 1)
                            {
                                objReporte.Grupo2 = objReporte.pstrCuenta.StartsWith("9") || objReporte.pstrCuenta.StartsWith("79") ? "Grupo2" : "";
                                objReporte.Grupo1 = !objReporte.pstrCuenta.StartsWith("9") && !objReporte.pstrCuenta.StartsWith("79") ? "Grupo1" : "";
                            }
                            else
                            {
                                objReporte.Grupo2 = objReporte.pstrCuenta.StartsWith("9") ? "Grupo2" : "";
                                objReporte.Grupo1 = !objReporte.pstrCuenta.StartsWith("9") ? "Grupo1" : "";
                            }

                            objReporte.debeSumaAcumulada = pIntMoneda == (int)Currency.Soles ? CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeSoles) : CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeDolares);
                            objReporte.haberSumaAcumulada = pIntMoneda == (int)Currency.Soles ? CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberSoles) : CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberDolares);

                            //- AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberSoles)
                            decimal MovMesDebe = 0, AperturaDebe = 0, TotalDebe = 0, deudorActual = 0, acreedorActual = 0, AperturaHaber = 0, MovMesHaber = 0, TotalHaber = 0, DeudorFinal = 0, AcreedorFinal = 0;
                            var ListaImputables = ListaTodasMovimientos.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).ToList();
                            ListaCuentasImputables = new List<string>();
                            foreach (var cuenta in ListaImputables)
                            {
                                if (!ListaCuentasImputables.Contains(cuenta.pstrCuenta))
                                {
                                    ListaCuentasImputables.Add(cuenta.pstrCuenta);
                                }
                            }

                            foreach (var cuenta in ListaCuentasImputables.AsParallel())
                            {

                                if (pIntMoneda == (int)Currency.Soles)
                                {
                                    //AperturaDebe = AsientoApertura.FindAll(x => x.pstrCuenta.StartsWith(cuenta)).Sum(x => x.ImporteDebeSoles);
                                    //MovMesDebe = CuentasTotales.FindAll(x => x.pstrCuenta.StartsWith(cuenta)).Sum(x => x.ImporteDebeSoles);
                                    //AperturaHaber = AsientoApertura.FindAll(x => x.pstrCuenta.StartsWith(cuenta)).Sum(x => x.ImporteHaberSoles);
                                    //MovMesHaber = CuentasTotales.FindAll(x => x.pstrCuenta.StartsWith(cuenta)).Sum(x => x.ImporteHaberSoles);

                                    AperturaDebe = AsientoApertura.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteDebeSoles);
                                    MovMesDebe = CuentasTotales.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteDebeSoles);
                                    AperturaHaber = AsientoApertura.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteHaberSoles);
                                    MovMesHaber = CuentasTotales.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteHaberSoles);
                                }
                                else
                                {

                                    //AperturaDebe = AsientoApertura.FindAll(x => x.pstrCuenta.StartsWith(cuenta)).Sum(x => x.ImporteDebeDolares);
                                    //MovMesDebe = CuentasTotales.FindAll(x => x.pstrCuenta.StartsWith(cuenta)).Sum(x => x.ImporteDebeDolares);
                                    //AperturaHaber = AsientoApertura.FindAll(x => x.pstrCuenta.StartsWith(cuenta)).Sum(x => x.ImporteHaberDolares);
                                    //MovMesHaber = CuentasTotales.FindAll(x => x.pstrCuenta.StartsWith(cuenta)).Sum(x => x.ImporteHaberDolares);

                                    AperturaDebe = AsientoApertura.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteDebeDolares);
                                    MovMesDebe = CuentasTotales.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteDebeDolares);
                                    AperturaHaber = AsientoApertura.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteHaberDolares);
                                    MovMesHaber = CuentasTotales.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteHaberDolares);

                                }

                                //TotalDebe = MovMesDebe - AperturaDebe;
                                //TotalHaber = MovMesHaber - AperturaHaber;
                                //deudorActual = TotalDebe > TotalHaber ? TotalDebe - TotalHaber : 0;
                                //acreedorActual = TotalHaber > TotalDebe ? TotalHaber - TotalDebe : 0;
                                //DeudorFinal = deudorActual + DeudorFinal;
                                //AcreedorFinal = acreedorActual + AcreedorFinal;



                                TotalDebe = MovMesDebe + AperturaDebe;
                                TotalHaber = MovMesHaber + AperturaHaber;
                                deudorActual = TotalDebe > TotalHaber ? TotalDebe - TotalHaber : 0;
                                acreedorActual = TotalHaber > TotalDebe ? TotalHaber - TotalDebe : 0;
                                DeudorFinal = deudorActual + DeudorFinal;
                                AcreedorFinal = acreedorActual + AcreedorFinal;

                            }

                            //objReporte.deudor = DeudorFinal;
                            //objReporte.acreedor = AcreedorFinal;
                            objReporte.deudor = DeudorFinal > AcreedorFinal ? DeudorFinal - AcreedorFinal : 0;//Antes del 20 de marzo
                            objReporte.acreedor = AcreedorFinal > DeudorFinal ? AcreedorFinal - DeudorFinal : 0;

                            if (objReporte.pstrCuenta == "1011110")
                            {
                                string h = "";
                            }
                            ListaFinal.Add(objReporte);

                        }

                        pIntDigitos = pIntDigitos + 1;
                    }


                    int Contador = 1;
                    pIntDigitos = 2;
                    if (Contador < ListaEmpiezaCuenta.Count())
                    {
                        foreach (var subCuenta in ListaEmpiezaCuenta.AsParallel())
                        {
                            while (pIntDigitos <= pIntNumeroDigitos)
                            {
                                objReporte = new ReporteBalanceComprobacion();
                                objReporte.pstrCuenta = subCuenta.StartsWith(item) && subCuenta.Length >= pIntDigitos ? subCuenta.Substring(0, pIntDigitos) : "*No Existe*";

                                if (objReporte.pstrCuenta == "5011101")
                                {

                                    string h = "";
                                }
                                if (objReporte.pstrCuenta != "*No Existe*" && ListaFinal.Where(x => x.pstrCuenta == objReporte.pstrCuenta).Count() == 0)
                                {

                                    var CuentaImputable = NombreCuentaImputable(objReporte.pstrCuenta);
                                    string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
                                    int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
                                    objReporte.pstrDenominacion = objReporte.pstrCuenta != "No Existe*" ? denominacion : "*No Existe*";
                                    objReporte.imputable = imputable;

                                    objReporte.debeApertura = pIntMoneda == (int)Currency.Soles ? AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeSoles) : AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeDolares);
                                    objReporte.haberApertura = pIntMoneda == (int)Currency.Soles ? AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberSoles) : AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberDolares);


                                    if (pIntCuentasGastos79 == 1)
                                    {
                                        objReporte.Grupo2 = objReporte.pstrCuenta.StartsWith("9") || objReporte.pstrCuenta.StartsWith("79") ? "Grupo2" : "";
                                        objReporte.Grupo1 = !objReporte.pstrCuenta.StartsWith("9") && !objReporte.pstrCuenta.StartsWith("79") ? "Grupo1" : "";
                                    }
                                    else
                                    {
                                        objReporte.Grupo2 = objReporte.pstrCuenta.StartsWith("9") ? "Grupo2" : "";
                                        objReporte.Grupo1 = !objReporte.pstrCuenta.StartsWith("9") ? "Grupo1" : "";
                                    }

                                    // antes del  24 marzo 2017
                                    //objReporte.debeSumaAcumulada = pIntMoneda == (int)Currency.Soles ? CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeSoles) - AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeSoles) : CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeDolares) - AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeDolares);
                                    //objReporte.haberSumaAcumulada = pIntMoneda == (int)Currency.Soles ? CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberSoles) - AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberSoles) : CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberDolares) - AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberDolares);


                                    objReporte.debeSumaAcumulada = pIntMoneda == (int)Currency.Soles ? CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeSoles) : CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeDolares);
                                    objReporte.haberSumaAcumulada = pIntMoneda == (int)Currency.Soles ? CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberSoles) : CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberDolares);


                                    decimal MovMesDebe = 0, AperturaDebe = 0, TotalDebe = 0, deudorActual = 0, acreedorActual = 0, AperturaHaber = 0, MovMesHaber = 0, TotalHaber = 0, DeudorFinal = 0, AcreedorFinal = 0;


                                    var ListaImputables = ListaTodasMovimientos.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).ToList();
                                    ListaCuentasImputables = new List<string>();
                                    foreach (var cuenta in ListaImputables)
                                    {
                                        if (!ListaCuentasImputables.Contains(cuenta.pstrCuenta))
                                        {
                                            ListaCuentasImputables.Add(cuenta.pstrCuenta);
                                        }
                                    }

                                    foreach (var cuenta in ListaCuentasImputables.AsParallel())
                                    {

                                        if (pIntMoneda == (int)Currency.Soles)
                                        {
                                            //AperturaDebe = AsientoApertura.FindAll(x => x.pstrCuenta.StartsWith(cuenta)).Sum(x => x.ImporteDebeSoles);
                                            //MovMesDebe = CuentasTotales.FindAll(x => x.pstrCuenta.StartsWith(cuenta)).Sum(x => x.ImporteDebeSoles);
                                            //AperturaHaber = AsientoApertura.FindAll(x => x.pstrCuenta.StartsWith(cuenta)).Sum(x => x.ImporteHaberSoles);
                                            //MovMesHaber = CuentasTotales.FindAll(x => x.pstrCuenta.StartsWith(cuenta)).Sum(x => x.ImporteHaberSoles);

                                            AperturaDebe = AsientoApertura.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteDebeSoles);
                                            MovMesDebe = CuentasTotales.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteDebeSoles);
                                            AperturaHaber = AsientoApertura.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteHaberSoles);
                                            MovMesHaber = CuentasTotales.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteHaberSoles);
                                        }
                                        else
                                        {

                                            //AperturaDebe = AsientoApertura.FindAll(x => x.pstrCuenta.StartsWith(cuenta)).Sum(x => x.ImporteDebeDolares);
                                            //MovMesDebe = CuentasTotales.FindAll(x => x.pstrCuenta.StartsWith(cuenta)).Sum(x => x.ImporteDebeDolares);
                                            //AperturaHaber = AsientoApertura.FindAll(x => x.pstrCuenta.StartsWith(cuenta)).Sum(x => x.ImporteHaberDolares);
                                            //MovMesHaber = CuentasTotales.FindAll(x => x.pstrCuenta.StartsWith(cuenta)).Sum(x => x.ImporteHaberDolares);
                                            AperturaDebe = AsientoApertura.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteDebeDolares);
                                            MovMesDebe = CuentasTotales.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteDebeDolares);
                                            AperturaHaber = AsientoApertura.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteHaberDolares);
                                            MovMesHaber = CuentasTotales.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteHaberDolares);

                                        }

                                        //TotalDebe = MovMesDebe - AperturaDebe;
                                        //TotalHaber = MovMesHaber - AperturaHaber;
                                        //deudorActual = TotalDebe > TotalHaber ? TotalDebe - TotalHaber : 0;
                                        //acreedorActual = TotalHaber > TotalDebe ? TotalHaber - TotalDebe : 0;
                                        //DeudorFinal = deudorActual + DeudorFinal;
                                        //AcreedorFinal = acreedorActual + AcreedorFinal;

                                        TotalDebe = MovMesDebe + AperturaDebe;
                                        TotalHaber = MovMesHaber + AperturaHaber;
                                        deudorActual = TotalDebe > TotalHaber ? TotalDebe - TotalHaber : 0;
                                        acreedorActual = TotalHaber > TotalDebe ? TotalHaber - TotalDebe : 0;
                                        DeudorFinal = deudorActual + DeudorFinal;
                                        AcreedorFinal = acreedorActual + AcreedorFinal;

                                    }

                                    //objReporte.deudor = DeudorFinal;
                                    //objReporte.acreedor = AcreedorFinal;
                                    objReporte.deudor = DeudorFinal > AcreedorFinal ? DeudorFinal - AcreedorFinal : 0;// Antes del 20 de marzo
                                    objReporte.acreedor = AcreedorFinal > DeudorFinal ? AcreedorFinal - DeudorFinal : 0;

                                    if (objReporte.pstrCuenta == "5011101")
                                    {
                                        string h = "";
                                    }
                                    ListaFinal.Add(objReporte);

                                }
                                pIntDigitos = pIntDigitos + 1;
                            }
                            pIntDigitos = 2;
                        }
                    }
                }
                if (pIntCuentasImputables == 1)
                {


                    return ListaFinal.ToList().Where(y => y.imputable == 1 || (y.imputable == 0 && y.pstrCuenta.Length == 2)).OrderBy(x => x.pstrCuenta).ToList().Where(x => x.debeApertura != 0 || x.haberApertura != 0 || x.debeSumaAcumulada != 0 || x.haberSumaAcumulada != 0 || x.deudor != 0 || x.acreedor != 0).ToList();
                }
                else
                {

                    return ListaFinal.OrderBy(x => x.pstrCuenta).ToList().Where(x => x.debeApertura != 0 || x.haberApertura != 0 || x.debeSumaAcumulada != 0 || x.haberSumaAcumulada != 0 || x.deudor != 0 || x.acreedor != 0).ToList();
                }

                #endregion

            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;
            }
        }



        public List<ReporteBalanceComprobacion> ReporteBalanceComprobacionSBS(ref OperationResult objOperationResult, string pstrPeriodo, string pstrMes, int pIntNumeroDigitos, int pIntMoneda, int pIntCuentasImputables, int pIntCuentasGastos79)
        {

            // Para cualquier mes ,siempre se presenta los asientos de apertura
            // Las sumas es del movimiento del Mes,pero no se tiene en cuenta el asiento de apertura

            try
            {
                AsientosContablesBL _objAsientoContableBL = new AsientosContablesBL();
                objOperationResult.Success = 1;
                List<ReporteBalanceComprobacion> ListaFinal = new List<ReporteBalanceComprobacion>();
                List<ReporteBalanceComprobacion> AsientoApertura = new List<ReporteBalanceComprobacion>();
                List<ReporteBalanceComprobacion> CuentasAnteriores = new List<ReporteBalanceComprobacion>();
                ReporteBalanceComprobacion objReporte = new ReporteBalanceComprobacion();
                List<ReporteBalanceComprobacion> CuentasTotales = new List<ReporteBalanceComprobacion>();
                List<string> ListaCuentasImputables = new List<string>();

                // string MesAnterior = pstrMes != "01" ? (int.Parse(pstrMes) - 1).ToString("00") : pstrMes;

                string MesAnterior = pstrMes;

                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region Recopila Información de AsientoApertura
                    AsientoApertura = (from a in dbContext.diario

                                       join b in dbContext.diariodetalle on new { IdDiario = a.v_IdDiario, eliminado = 0 }
                                                                        equals new { IdDiario = b.v_IdDiario, eliminado = b.i_Eliminado.Value } into b_join
                                       from b in b_join.DefaultIfEmpty()

                                       join c in dbContext.asientocontable on new { asiento = b.v_NroCuenta, eliminado = 0, p = periodo }
                                                                        equals new { asiento = c.v_NroCuenta, eliminado = c.i_Eliminado.Value, p = c.v_Periodo } into c_join

                                       from c in c_join.DefaultIfEmpty()

                                       where a.v_Periodo == pstrPeriodo && a.i_IdTipoComprobante == 1 && a.i_Eliminado == 0 && (b.v_NroCuenta != null || b.v_NroCuenta != "") && a.v_Mes == "01"

                                       orderby b.v_NroCuenta
                                       select new ReporteBalanceComprobacion
                                       {

                                           pstrCuenta = b.v_NroCuenta.Trim(),
                                           pstrDenominacion = c == null ? "*No Existe*" : c.v_NombreCuenta.Trim(),
                                           mes = a.v_Mes,
                                           imputable = c == null ? -1 : c.i_Imputable.Value,
                                           ImporteDebeSoles = a.i_IdMoneda == 1 && b.v_Naturaleza == "D" ? b.d_Importe.Value : a.i_IdMoneda == 2 && b.v_Naturaleza == "D" ? b.d_Cambio.Value : 0,
                                           ImporteDebeDolares = a.i_IdMoneda == 2 && b.v_Naturaleza == "D" ? b.d_Importe.Value : a.i_IdMoneda == 1 && b.v_Naturaleza == "D" ? b.d_Cambio.Value : 0,
                                           ImporteHaberSoles = a.i_IdMoneda == 1 && b.v_Naturaleza == "H" ? b.d_Importe.Value : a.i_IdMoneda == 2 && b.v_Naturaleza == "H" ? b.d_Cambio.Value : 0,
                                           ImporteHaberDolares = a.i_IdMoneda == 2 && b.v_Naturaleza == "H" ? b.d_Importe.Value : a.i_IdMoneda == 1 && b.v_Naturaleza == "H" ? b.d_Cambio.Value : 0,

                                       }).ToList();

                    #endregion

                    var saldoscontables = _objAsientoContableBL.ObtenerSaldosContablesPorMes(ref  objOperationResult, int.Parse(MesAnterior), int.Parse(pstrPeriodo));

                    CuentasAnteriores = (from a in saldoscontables

                                         join b in dbContext.asientocontable on new { cuenta = a.v_NroCuenta, eliminado = 0, p = periodo }
                                                                             equals new { cuenta = b.v_NroCuenta, eliminado = b.i_Eliminado.Value, p = b.v_Periodo } into b_join
                                         from b in b_join.DefaultIfEmpty()

                                         where (a.v_NroCuenta != null && a.v_NroCuenta != "" && a.v_NroCuenta != "*Cta Eliminada*")

                                         orderby a.v_NroCuenta

                                         select new ReporteBalanceComprobacion
                                         {
                                             pstrCuenta = a.v_NroCuenta.Trim(),
                                             pstrDenominacion = b == null ? "*No Existe*" : b.v_NombreCuenta.Trim(),
                                             mes = a.v_Mes,
                                             imputable = b == null ? -1 : b.i_Imputable.Value,
                                             ImporteDebeSoles = a.d_ImporteSolesD.Value,
                                             ImporteDebeDolares = a.d_ImporteDolaresD.Value,
                                             ImporteHaberSoles = a.d_ImporteSolesH.Value,
                                             ImporteHaberDolares = a.d_ImporteDolaresH.Value,

                                         }).ToList();


                }

                #region RecopilaCuentasExistentesMesRequerido
                CuentasTotales = AsientoApertura.Concat(CuentasAnteriores).ToList();
                var CuentasMayores = CuentasTotales.Select(x => x.pstrCuenta).Select(x => x.Substring(0, 2)).Distinct();

                foreach (var item in CuentasMayores.AsParallel())
                {
                    objReporte = new ReporteBalanceComprobacion();
                    int pIntDigitos = 2;
                    var ListaEmpiezaCuenta = CuentasTotales.Where(x => x.pstrCuenta.StartsWith(item.Substring(0, 2))).Select(y => y.pstrCuenta);
                    while (pIntDigitos <= pIntNumeroDigitos)
                    {

                        objReporte = new ReporteBalanceComprobacion();
                        objReporte.pstrCuenta = CuentasTotales.Where(x => x.pstrCuenta.StartsWith(item)).Count() != 0 ? CuentasTotales.Where(x => x.pstrCuenta.StartsWith(item)).FirstOrDefault().pstrCuenta.Length >= pIntDigitos ? CuentasTotales.Where(x => x.pstrCuenta.StartsWith(item)).FirstOrDefault().pstrCuenta.Substring(0, pIntDigitos) : "*No Existe*" : "*No Existe*";

                        if (objReporte.pstrCuenta != "*No Existe*" && ListaFinal.Where(x => x.pstrCuenta == objReporte.pstrCuenta).Count() == 0)
                        {
                            var CuentaImputable = NombreCuentaImputable(objReporte.pstrCuenta);
                            string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
                            int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
                            objReporte.pstrDenominacion = objReporte.pstrCuenta != "No Existe*" ? denominacion : "*No Existe*";
                            objReporte.imputable = imputable;
                            objReporte.debeApertura = pIntMoneda == (int)Currency.Soles ? AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeSoles) : AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeDolares);
                            objReporte.haberApertura = pIntMoneda == (int)Currency.Soles ? AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberSoles) : AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberDolares);
                            objReporte.debeApertura = objReporte.debeApertura - objReporte.haberApertura; //Se hizo la resta porque  para este formato solo se muestra una columna


                            //if (pIntCuentasGastos79 == 1)
                            //{
                            //    objReporte.Grupo2 = objReporte.pstrCuenta.StartsWith("9") || objReporte.pstrCuenta.StartsWith("79") ? "Grupo2" : "";
                            //    objReporte.Grupo1 = !objReporte.pstrCuenta.StartsWith("9") && !objReporte.pstrCuenta.StartsWith("79") ? "Grupo1" : "";
                            //}
                            //else
                            //{
                            objReporte.Grupo2 = "";
                            objReporte.Grupo1 = "Grupo1";
                            //}


                            objReporte.debeSumaAcumulada = pIntMoneda == (int)Currency.Soles ? CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeSoles) - AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeSoles) : CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeDolares) - AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeDolares);
                            objReporte.haberSumaAcumulada = pIntMoneda == (int)Currency.Soles ? CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberSoles) - AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberSoles) : CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberDolares) - AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberDolares);
                            decimal MovMesDebe = 0, AperturaDebe = 0, TotalDebe = 0, deudorActual = 0, acreedorActual = 0, AperturaHaber = 0, MovMesHaber = 0, TotalHaber = 0, DeudorFinal = 0, AcreedorFinal = 0;
                            var ListaImputables = CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).ToList();
                            ListaCuentasImputables = new List<string>();
                            foreach (var cuenta in ListaImputables)
                            {
                                if (!ListaCuentasImputables.Contains(cuenta.pstrCuenta))
                                {
                                    ListaCuentasImputables.Add(cuenta.pstrCuenta);
                                }
                            }

                            foreach (var cuenta in ListaCuentasImputables.AsParallel())
                            {

                                if (pIntMoneda == (int)Currency.Soles)
                                {


                                    AperturaDebe = AsientoApertura.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteDebeSoles);
                                    MovMesDebe = CuentasTotales.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteDebeSoles);
                                    AperturaHaber = AsientoApertura.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteHaberSoles);
                                    MovMesHaber = CuentasTotales.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteHaberSoles);
                                }
                                else
                                {



                                    AperturaDebe = AsientoApertura.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteDebeDolares);
                                    MovMesDebe = CuentasTotales.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteDebeDolares);
                                    AperturaHaber = AsientoApertura.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteHaberDolares);
                                    MovMesHaber = CuentasTotales.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteHaberDolares);

                                }

                                TotalDebe = MovMesDebe - AperturaDebe;
                                TotalHaber = MovMesHaber - AperturaHaber;
                                deudorActual = TotalDebe > TotalHaber ? TotalDebe - TotalHaber : 0;
                                acreedorActual = TotalHaber > TotalDebe ? TotalHaber - TotalDebe : 0;
                                DeudorFinal = deudorActual + DeudorFinal;
                                AcreedorFinal = acreedorActual + AcreedorFinal;
                            }


                            objReporte.deudor = DeudorFinal > AcreedorFinal ? DeudorFinal - AcreedorFinal : 0;//Antes del 20 de marzo
                            objReporte.acreedor = AcreedorFinal > DeudorFinal ? AcreedorFinal - DeudorFinal : 0;
                            objReporte.deudor = objReporte.deudor - objReporte.acreedor; // Se agrego  porque formato acepa una sola columna
                            ListaFinal.Add(objReporte);

                        }

                        pIntDigitos = pIntDigitos + 1;
                    }


                    int Contador = 1;
                    pIntDigitos = 2;
                    if (Contador < ListaEmpiezaCuenta.Count())
                    {
                        foreach (var subCuenta in ListaEmpiezaCuenta.AsParallel())
                        {
                            while (pIntDigitos <= pIntNumeroDigitos)
                            {
                                objReporte = new ReporteBalanceComprobacion();
                                objReporte.pstrCuenta = subCuenta.StartsWith(item) && subCuenta.Length >= pIntDigitos ? subCuenta.Substring(0, pIntDigitos) : "*No Existe*";
                                if (objReporte.pstrCuenta != "*No Existe*" && ListaFinal.Where(x => x.pstrCuenta == objReporte.pstrCuenta).Count() == 0)
                                {

                                    var CuentaImputable = NombreCuentaImputable(objReporte.pstrCuenta);
                                    string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
                                    int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
                                    objReporte.pstrDenominacion = objReporte.pstrCuenta != "No Existe*" ? denominacion : "*No Existe*";
                                    objReporte.imputable = imputable;

                                    objReporte.debeApertura = pIntMoneda == (int)Currency.Soles ? AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeSoles) : AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeDolares);
                                    objReporte.haberApertura = pIntMoneda == (int)Currency.Soles ? AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberSoles) : AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberDolares);

                                    objReporte.debeApertura = objReporte.debeApertura - objReporte.haberApertura;
                                    objReporte.Grupo2 = "";
                                    objReporte.Grupo1 = "Grupo1";

                                    objReporte.debeSumaAcumulada = pIntMoneda == (int)Currency.Soles ? CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeSoles) - AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeSoles) : CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeDolares) - AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteDebeDolares);
                                    objReporte.haberSumaAcumulada = pIntMoneda == (int)Currency.Soles ? CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberSoles) - AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberSoles) : CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberDolares) - AsientoApertura.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).Sum(x => x.ImporteHaberDolares);
                                    decimal MovMesDebe = 0, AperturaDebe = 0, TotalDebe = 0, deudorActual = 0, acreedorActual = 0, AperturaHaber = 0, MovMesHaber = 0, TotalHaber = 0, DeudorFinal = 0, AcreedorFinal = 0;
                                    var ListaImputables = CuentasTotales.Where(x => x.pstrCuenta.StartsWith(objReporte.pstrCuenta)).ToList();
                                    ListaCuentasImputables = new List<string>();
                                    foreach (var cuenta in ListaImputables)
                                    {
                                        if (!ListaCuentasImputables.Contains(cuenta.pstrCuenta))
                                        {
                                            ListaCuentasImputables.Add(cuenta.pstrCuenta);
                                        }
                                    }

                                    foreach (var cuenta in ListaCuentasImputables.AsParallel())
                                    {

                                        if (pIntMoneda == (int)Currency.Soles)
                                        {


                                            AperturaDebe = AsientoApertura.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteDebeSoles);
                                            MovMesDebe = CuentasTotales.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteDebeSoles);
                                            AperturaHaber = AsientoApertura.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteHaberSoles);
                                            MovMesHaber = CuentasTotales.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteHaberSoles);
                                        }
                                        else
                                        {


                                            AperturaDebe = AsientoApertura.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteDebeDolares);
                                            MovMesDebe = CuentasTotales.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteDebeDolares);
                                            AperturaHaber = AsientoApertura.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteHaberDolares);
                                            MovMesHaber = CuentasTotales.FindAll(x => x.pstrCuenta == cuenta).Sum(x => x.ImporteHaberDolares);

                                        }

                                        TotalDebe = MovMesDebe - AperturaDebe;
                                        TotalHaber = MovMesHaber - AperturaHaber;
                                        deudorActual = TotalDebe > TotalHaber ? TotalDebe - TotalHaber : 0;
                                        acreedorActual = TotalHaber > TotalDebe ? TotalHaber - TotalDebe : 0;
                                        DeudorFinal = deudorActual + DeudorFinal;
                                        AcreedorFinal = acreedorActual + AcreedorFinal;
                                    }

                                    //objReporte.deudor = DeudorFinal;
                                    //objReporte.acreedor = AcreedorFinal;
                                    objReporte.deudor = DeudorFinal > AcreedorFinal ? DeudorFinal - AcreedorFinal : 0;// Antes del 20 de marzo
                                    objReporte.acreedor = AcreedorFinal > DeudorFinal ? AcreedorFinal - DeudorFinal : 0;
                                    objReporte.deudor = objReporte.deudor - objReporte.acreedor;

                                    ListaFinal.Add(objReporte);

                                }
                                pIntDigitos = pIntDigitos + 1;
                            }
                            pIntDigitos = 2;
                        }
                    }
                }
                if (pIntCuentasImputables == 1)
                {


                    return ListaFinal.ToList().Where(y => y.imputable == 1 || (y.imputable == 0 && y.pstrCuenta.Length == 2)).OrderBy(x => x.pstrCuenta).ToList().Where(x => x.debeApertura != 0 || x.haberApertura != 0 || x.debeSumaAcumulada != 0 || x.haberSumaAcumulada != 0 || x.deudor != 0 || x.acreedor != 0).ToList();
                }
                else
                {

                    return ListaFinal.OrderBy(x => x.pstrCuenta).ToList().Where(x => x.debeApertura != 0 || x.haberApertura != 0 || x.debeSumaAcumulada != 0 || x.haberSumaAcumulada != 0 || x.deudor != 0 || x.acreedor != 0).ToList();
                }

                #endregion

            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;
            }
        }


        public List<decimal> CalcularSaldosActualesReporteBalanceComprobacion(List<ReporteBalanceComprobacion> CuentasAnteriores, List<ReporteBalanceComprobacion> CuentasMesRequerido, string pstrCuenta, int pIntMoneda, List<ReporteBalanceComprobacion> CuentasTotales)
        {
            List<decimal> ListaResultante = new List<decimal>();
            List<string> ListaCuentasImputables = new List<string>();
            decimal anteriorDebe = 0, MovMesDebe = 0, anteriorHaber = 0, MovMesHaber = 0, sumaDebe = 0, sumaHaber, deudorActual = 0, acreedorActual = 0, DeudorFinal = 0, AcreedorFinal = 0;
            var ListaImputables = CuentasTotales.Where(x => x.pstrCuenta.StartsWith(pstrCuenta)).ToList();

            foreach (var item in ListaImputables)
            {
                if (!ListaCuentasImputables.Contains(item.pstrCuenta))
                {
                    ListaCuentasImputables.Add(item.pstrCuenta);
                }
            }
            //  ListaCuentasImputables = ListaImputables.Select(x => x.pstrCuenta).Distinct().ToList (); 
            foreach (var item in ListaCuentasImputables.AsParallel())
            {

                if (pIntMoneda == 1)
                {
                    anteriorDebe = CuentasAnteriores.FindAll(x => x.pstrCuenta.StartsWith(item)).Sum(x => x.ImporteDebeSoles);
                    MovMesDebe = CuentasMesRequerido.FindAll(x => x.pstrCuenta.StartsWith(item)).Sum(x => x.ImporteDebeSoles);
                }
                else
                {
                    anteriorDebe = CuentasAnteriores.FindAll(x => x.pstrCuenta.StartsWith(item)).Sum(x => x.ImporteDebeDolares);
                    MovMesDebe = CuentasMesRequerido.FindAll(x => x.pstrCuenta.StartsWith(item)).Sum(x => x.ImporteDebeDolares);
                }


                if (pIntMoneda == 1)
                {
                    anteriorHaber = CuentasAnteriores.FindAll(x => x.pstrCuenta.StartsWith(item)).Sum(x => x.ImporteHaberSoles);
                    MovMesHaber = CuentasMesRequerido.FindAll(x => x.pstrCuenta.StartsWith(item)).Sum(x => x.ImporteHaberSoles);
                }
                else
                {
                    anteriorHaber = CuentasAnteriores.FindAll(x => x.pstrCuenta.StartsWith(item)).Sum(x => x.ImporteHaberDolares);
                    MovMesHaber = CuentasMesRequerido.FindAll(x => x.pstrCuenta.StartsWith(item)).Sum(x => x.ImporteHaberDolares);
                }


                sumaDebe = anteriorDebe + MovMesDebe;
                sumaHaber = anteriorHaber + MovMesHaber;

                deudorActual = sumaDebe > sumaHaber ? sumaDebe - sumaHaber : 0;
                acreedorActual = sumaHaber > sumaDebe ? sumaHaber - sumaDebe : 0;
                DeudorFinal = deudorActual + DeudorFinal;
                AcreedorFinal = acreedorActual + AcreedorFinal;

            }
            ListaResultante.Add(DeudorFinal);
            ListaResultante.Add(AcreedorFinal);
            return ListaResultante;

        }
        public List<ReporteDocumentosIngresados> ReporteDocumentosIngresados(ref OperationResult objOperationResult, DateTime FechaInicio, DateTime FechaFin, bool RegistroTesoreria, bool RegistroContabilidad, string NroCuenta, int TipoDocTesoreriaE, string NumeroTesoreria, int TipoDocContabilidadE, string NumeroContabilidad, int pIntTipoDocumentosDetalles, int pIntResumen, int Nivel, int pIntMostrarSoloResumen, int pIntAsientosDescuadradosSoles, string CentroCosto, int pIntAsientosDescuadradosAmbos)
        {
            try
            {
                objOperationResult.Success = 1;

                List<ReporteDocumentosIngresados> ListaTesoreriaContabilidad = new List<ReporteDocumentosIngresados>();
                List<ReporteDocumentosIngresados> RegistrosTesoreria = new List<ReporteDocumentosIngresados>();
                List<ReporteDocumentosIngresados> RegistrosContabilidad = new List<ReporteDocumentosIngresados>();
                List<ReporteDocumentosIngresados> RegistrosContabilidadSinDestinos = new List<ReporteDocumentosIngresados>();
                ReporteDocumentosIngresados objReporte = new ReporteDocumentosIngresados();
                List<ReporteDocumentosIngresados> ListaFinal = new List<ReporteDocumentosIngresados>();
                List<ReporteDocumentosIngresados> ListaResumen = new List<ReporteDocumentosIngresados>();
                List<ReporteDocumentosIngresados> RegistrosContabilidaTesoreria = new List<ReporteDocumentosIngresados>();
                List<ReporteDocumentosIngresados> RegistrosContabilidaTesoreriaBusqueda = new List<ReporteDocumentosIngresados>();
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();


                List<destinoDto> ListasOrigenDestinos = (from n in dbContext.destino
                                                         where n.i_Eliminado == 0 && n.v_Periodo == periodo

                                                         select new destinoDto
                                                         {
                                                             v_CuentaOrigen = n.v_CuentaOrigen,
                                                             v_CuentaDestino = n.v_CuentaDestino,
                                                         }).ToList();
                // var DestinosDictionary = ListasOrigenDestinos.ToDictionary(o => o.v_CuentaDestino, o => o);

                if (RegistroTesoreria)
                {



                    dbContext.tesoreriadetalle.MergeOption = MergeOption.AppendOnly;
                    List<tesoreriadetalle> tesoreriaDetalle;
                    var rangoCtas = Utils.Windows.RangoDeCuentas(null, null).ToList();
                    var queryToExecute = new AsientosContablesBL().ObtenerConsultaAnalitico(2, FechaInicio, FechaFin, null, rangoCtas, 0,
                        null, null);

                    switch (Globals.TipoMotor)
                    {
                        case TipoMotorBD.PostgreSQL:
                            using (var cnx = new Devart.Data.PostgreSql.PgSqlConnection(Globals.CadenaConexion))
                            {
                                tesoreriaDetalle = cnx.Query<tesoreriadetalle>(queryToExecute).ToList();
                            }
                            break;

                        default:
                            using (var cnx = new System.Data.SqlClient.SqlConnection(Globals.CadenaConexion))
                            {
                                tesoreriaDetalle = cnx.Query<tesoreriadetalle>(queryToExecute).ToList();
                            }
                            break;
                    }

                    RegistrosTesoreria =

                                          (from a in tesoreriaDetalle.Where(p => p.i_Eliminado == 0)

                                           join b in dbContext.tesoreria on new { IdTesoreria = a.v_IdTesoreria, eliminado = 0 } equals new { IdTesoreria = b.v_IdTesoreria, eliminado = b.i_Eliminado.Value } into b_join

                                           from b in b_join.DefaultIfEmpty()

                                           join c in dbContext.documento on new { IdTipoDoc = a.i_IdTipoDocumento ?? 0, eliminado = 0 } equals new { IdTipoDoc = c.i_CodigoDocumento, eliminado = c.i_Eliminado.Value } into c_join

                                           from c in c_join.DefaultIfEmpty()

                                           join d in dbContext.documento on b != null ? b.i_IdTipoDocumento ?? 0 : 0 equals d.i_CodigoDocumento into d_join

                                           from d in d_join.DefaultIfEmpty()

                                           join e in dbContext.cliente on new { IdCliente = a.v_IdCliente, eliminado = 0 } equals new { IdCliente = e.v_IdCliente, eliminado = e.i_Eliminado.Value } into e_join

                                           from e in e_join.DefaultIfEmpty()

                                           where a.i_Eliminado == 0 && b.t_FechaRegistro >= FechaInicio && b.t_FechaRegistro <= FechaFin

                                           && (a.v_NroCuenta == NroCuenta || NroCuenta == string.Empty) && (b.i_IdTipoDocumento == TipoDocTesoreriaE || TipoDocTesoreriaE == -1) && (b.v_Mes.Trim() + "-" + b.v_Correlativo.Trim() == NumeroTesoreria || NumeroTesoreria == "")

                                           && (b.i_IdTipoDocumento == pIntTipoDocumentosDetalles || pIntTipoDocumentosDetalles == -1)

                                           && b.i_IdEstado == 1 && b.i_Eliminado == 0

                                           //&& a.tesoreria != null

                                           && (b != null)
                                           orderby a.v_Naturaleza
                                           select new ReporteDocumentosIngresados
                                           {
                                               IdCabecera = a == null ? "" : a.v_IdTesoreria,
                                               Moneda = b == null ? "" : b.i_IdMoneda == (int)Currency.Soles ? "S/." : b.i_IdMoneda == (int)Currency.Dolares ? "D" : "",
                                               NroComprobante = c == null ? "" + a.v_NroDocumento : c.v_Siglas + " " + a.v_NroDocumento,
                                               Fecha = b == null ? DateTime.Now : b.t_FechaRegistro.Value,
                                               NroDocumento = b == null && d == null ? "" : b == null ? "" : d == null ? b.v_Mes.Trim() + "-" + b.v_Correlativo.Trim() : d.v_Siglas + " " + b.v_Mes.Trim() + "-" + b.v_Correlativo.Trim(),
                                               Anexo = b == null ? "" : e == null ? "" : (e.v_ApePaterno + " " + e.v_ApeMaterno + " " + e.v_PrimerNombre + " " + e.v_RazonSocial).Trim(),
                                               Analisis = b == null ? "" : string.IsNullOrEmpty(a.v_Analisis) || string.IsNullOrEmpty(b.v_Glosa) ? b.v_Glosa.Trim() : a.v_Analisis.Trim(),
                                               Cuenta = b == null ? "" : a.v_NroCuenta.Trim(),
                                               Caja = b == null ? 0 : a.i_IdCaja == null ? 0 : a.i_IdCaja.Value,
                                               CentroCosto = b == null ? "" : string.IsNullOrEmpty(a.i_IdCentroCostos) ? "" : a.i_IdCentroCostos,
                                               DebeSoles = b == null ? 0 : b.i_IdMoneda.Value == (int)Currency.Soles && a.v_Naturaleza == "D" ? a.d_Importe.Value : b.i_IdMoneda == (int)Currency.Dolares && a.v_Naturaleza == "D" ? a.d_Cambio.Value : 0,
                                               HaberSoles = b == null ? 0 : b.i_IdMoneda.Value == (int)Currency.Soles && a.v_Naturaleza == "H" ? a.d_Importe.Value : b.i_IdMoneda == (int)Currency.Dolares && a.v_Naturaleza == "H" ? a.d_Cambio.Value : 0,
                                               Dolares = b == null ? 0 : a.v_Naturaleza == "D" ? b.i_IdMoneda.Value == (int)Currency.Soles ? a.d_Cambio.Value : a.d_Importe.Value : b.i_IdMoneda.Value == (int)Currency.Soles ? a.d_Cambio.Value * -1 : a.d_Importe.Value * -1,
                                               Grupo1 = 1,
                                               DebeDolares = b == null ? 0 : b.i_IdMoneda.Value == (int)Currency.Soles && a.v_Naturaleza == "D" ? a.d_Cambio.Value : b.i_IdMoneda == (int)Currency.Dolares && a.v_Naturaleza == "D" ? a.d_Importe.Value : 0,
                                               HaberDolares = b == null ? 0 : b.i_IdMoneda.Value == (int)Currency.Soles && a.v_Naturaleza == "H" ? a.d_Cambio.Value : b.i_IdMoneda == (int)Currency.Dolares && a.v_Naturaleza == "H" ? a.d_Importe.Value : 0,
                                               NroCuenta = b == null ? "" : a.v_NroCuenta.Trim(),
                                               TotalDebeSoles = b == null ? 0 : b.i_IdMoneda.Value == (int)Currency.Soles ? b.d_TotalDebe_Importe.Value : b.d_TotalDebe_Cambio.Value,
                                               TotalHaberSoles = b == null ? 0 : b.i_IdMoneda.Value == (int)Currency.Soles ? b.d_TotalHaber_Importe.Value : b.d_TotalHaber_Cambio.Value,
                                               TotalDebeDolares = b == null ? 0 : b.i_IdMoneda.Value == (int)Currency.Soles ? b.d_TotalDebe_Cambio.Value : b.d_TotalDebe_Importe.Value,
                                               TotalHaberDolares = b == null ? 0 : b.i_IdMoneda.Value == (int)Currency.Soles ? b.d_TotalHaber_Cambio.Value : b.d_TotalHaber_Importe.Value,
                                               i_EsDestino = b == null ? 0 : a.i_EsDestino == null ? 0 : 1,

                                           }).ToList();

                    RegistrosTesoreria = RegistrosTesoreria.Where(o => o.CentroCosto == CentroCosto || CentroCosto == "SCC").ToList();

                    RegistrosTesoreria.AsParallel().ToList().ForEach(p =>
                    {

                        //p.Destino = p.i_EsDestino == 1 ? BuscarCuentaOrigen(RegistrosTesoreria, p.NroCuenta, p.IdCabecera, ListasOrigenDestinos) : "";

                        var CuentaOrigen = ListasOrigenDestinos.Where(o => o.v_CuentaDestino == p.Cuenta).FirstOrDefault();

                        p.Destino = p.i_EsDestino == 1 ? CuentaOrigen != null ? CuentaOrigen.v_CuentaOrigen : "" : "";

                    });

                }
                if (RegistroContabilidad)
                {


                    dbContext.diariodetalle.MergeOption = MergeOption.AppendOnly;

                    //Se usó dapper en esta seccion debido a la ineficiencia de EF en consultas grandes.
                    List<diariodetalle> diarioDetalle;
                    var rangoCtas = Utils.Windows.RangoDeCuentas(null, null).ToList();
                    var queryToExecute = new AsientosContablesBL().ObtenerConsultaAnalitico(1, FechaInicio, FechaFin, null, rangoCtas, 0,
                        null, null);

                    switch (Globals.TipoMotor)
                    {
                        case TipoMotorBD.PostgreSQL:
                            using (var cnx = new Devart.Data.PostgreSql.PgSqlConnection(Globals.CadenaConexion))
                            {
                                diarioDetalle = cnx.Query<diariodetalle>(queryToExecute).ToList();
                            }
                            break;

                        default:
                            using (var cnx = new System.Data.SqlClient.SqlConnection(Globals.CadenaConexion))
                            {
                                diarioDetalle = cnx.Query<diariodetalle>(queryToExecute).ToList();
                            }
                            break;
                    }


                    RegistrosContabilidadSinDestinos = (from a in diarioDetalle.Where(o => o.i_Eliminado == 0)

                                                        join b in dbContext.diario on new { IdDiario = a.v_IdDiario, eliminado = 0 } equals new { IdDiario = b.v_IdDiario, eliminado = b.i_Eliminado.Value } into b_join

                                                        from b in b_join.DefaultIfEmpty()

                                                        join c in dbContext.documento on new { IdTipoDoc = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { IdTipoDoc = c.i_CodigoDocumento, eliminado = c.i_Eliminado.Value } into c_join

                                                        from c in c_join.DefaultIfEmpty()

                                                        join d in dbContext.documento on b != null ? b.i_IdTipoDocumento ?? 0 : 0 equals d.i_CodigoDocumento into d_join

                                                        from d in d_join.DefaultIfEmpty()

                                                        join e in dbContext.cliente on new { IdCliente = a.v_IdCliente, eliminado = 0 } equals new { IdCliente = e.v_IdCliente, eliminado = e.i_Eliminado.Value } into e_join

                                                        from e in e_join.DefaultIfEmpty()

                                                        where a.i_Eliminado == 0 && b.t_Fecha >= FechaInicio && b.t_Fecha <= FechaFin

                                                        && (a.v_NroCuenta == NroCuenta || NroCuenta == string.Empty) && (b.i_IdTipoDocumento == TipoDocContabilidadE || TipoDocContabilidadE == -1) && (b.v_Mes.Trim() + "-" + b.v_Correlativo.Trim() == NumeroContabilidad || NumeroContabilidad == "")
                                                        && (b.i_IdTipoDocumento == pIntTipoDocumentosDetalles || pIntTipoDocumentosDetalles == -1)
                                                        && b.i_Eliminado == 0
                                                            //&& a.diario != null
                                                         && (b != null)

                                                        orderby a.v_Naturaleza
                                                        select new ReporteDocumentosIngresados
                                                        {
                                                            IdCabecera = a.v_IdDiario,
                                                            Moneda = b.i_IdMoneda == (int)Currency.Soles ? "S/." : b.i_IdMoneda == (int)Currency.Dolares ? "D" : "",

                                                            NroComprobante = c == null ? "" + a.v_NroDocumento.Trim() : c.v_Siglas + " " + a.v_NroDocumento.Trim(),
                                                            Fecha = b.t_Fecha.Value,

                                                            NroDocumento = b == null && d == null ? "" : b == null ? "" : d == null ? b.v_Mes.Trim() + "-" + b.v_Correlativo.Trim() : d.v_Siglas + " " + b.v_Mes.Trim() + "-" + b.v_Correlativo.Trim(),
                                                            Anexo = b == null ? "" : e == null ? "" : (e.v_ApePaterno + " " + e.v_ApeMaterno + " " + e.v_PrimerNombre + " " + e.v_RazonSocial).Trim(),
                                                            Analisis = b == null ? "" : a.v_Analisis == null || a.v_Analisis == string.Empty ? b.v_Glosa.Trim() : a.v_Analisis.Trim(),
                                                            Cuenta = b == null ? "" : a.v_NroCuenta.Trim(),
                                                            Caja = 0,
                                                            CentroCosto = b == null ? "" : string.IsNullOrEmpty(a.i_IdCentroCostos) ? "" : a.i_IdCentroCostos,
                                                            Destino = "",
                                                            DebeSoles = b == null ? 0 : b.i_IdMoneda.Value == (int)Currency.Soles && a.v_Naturaleza == "D" ? a.d_Importe.Value : b.i_IdMoneda == (int)Currency.Dolares && a.v_Naturaleza == "D" ? a.d_Cambio.Value : 0,
                                                            HaberSoles = b == null ? 0 : b.i_IdMoneda.Value == (int)Currency.Soles && a.v_Naturaleza == "H" ? a.d_Importe.Value : b.i_IdMoneda == (int)Currency.Dolares && a.v_Naturaleza == "H" ? a.d_Cambio.Value : 0,
                                                            Dolares = b == null ? 0 : a.v_Naturaleza == "D" ? b.i_IdMoneda.Value == (int)Currency.Soles ? a.d_Cambio.Value : a.d_Importe.Value : b.i_IdMoneda.Value == (int)Currency.Soles ? a.d_Cambio.Value * -1 : a.d_Importe.Value * -1,
                                                            Grupo1 = 1,
                                                            DebeDolares = b == null ? 0 : b.i_IdMoneda.Value == (int)Currency.Soles && a.v_Naturaleza == "D" ? a.d_Cambio.Value : b.i_IdMoneda == (int)Currency.Dolares && a.v_Naturaleza == "D" ? a.d_Importe.Value : 0,
                                                            HaberDolares = b == null ? 0 : b.i_IdMoneda.Value == (int)Currency.Soles && a.v_Naturaleza == "H" ? a.d_Cambio.Value : b.i_IdMoneda == (int)Currency.Dolares && a.v_Naturaleza == "H" ? a.d_Importe.Value : 0,
                                                            NroCuenta = b == null ? "" : a.v_NroCuenta.Trim(),
                                                            TotalDebeSoles = b == null ? 0 : b.i_IdMoneda.Value == (int)Currency.Soles ? b.d_TotalDebe.Value : b.d_TotalDebeCambio.Value,
                                                            TotalHaberSoles = b == null ? 0 : b.i_IdMoneda.Value == (int)Currency.Soles ? b.d_TotalHaber.Value : b.d_TotalHaberCambio.Value,
                                                            TotalDebeDolares = b == null ? 0 : b.i_IdMoneda.Value == (int)Currency.Soles ? b.d_TotalDebeCambio.Value : b.d_TotalDebe.Value,
                                                            TotalHaberDolares = b == null ? 0 : b.i_IdMoneda.Value == (int)Currency.Soles ? b.d_TotalHaberCambio.Value : b.d_TotalHaber.Value,
                                                            i_EsDestino = b == null ? 0 : a.i_EsDestino == null ? 0 : 1,


                                                        }).ToList();

                    RegistrosContabilidadSinDestinos = RegistrosContabilidadSinDestinos.Where(a => a.CentroCosto == CentroCosto || CentroCosto == "SCC").ToList();

                    RegistrosContabilidadSinDestinos.ToList().ForEach(p =>
                   {
                       //p.Destino = p.i_EsDestino == 1 ? BuscarCuentaOrigen(RegistrosContabilidadSinDestinos, p.NroCuenta, p.IdCabecera, ListasOrigenDestinos) : "";
                       var CuentaOrigen = ListasOrigenDestinos.Where(o => o.v_CuentaDestino == p.Cuenta).FirstOrDefault();
                       p.Destino = p.i_EsDestino == 1 ? CuentaOrigen != null ? CuentaOrigen.v_CuentaOrigen : "" : "";


                   });

                }


                if (pIntAsientosDescuadradosSoles == (int)AsientosDescuadrados.Activado || pIntAsientosDescuadradosAmbos == (int)AsientosDescuadrados.Activado)
                {
                    List<ReporteDocumentosIngresados> Rg = RegistrosTesoreria.Concat(RegistrosContabilidadSinDestinos).ToList();
                    var RegistrosAgrupados = Rg.GroupBy(x => new { x.IdCabecera }).Select(d =>
                {
                    var k = d.FirstOrDefault();
                    k.TotalDebeSoles = d.Sum(o => o.DebeSoles);
                    k.TotalDebeDolares = d.Sum(o => o.DebeDolares);
                    k.TotalHaberSoles = d.Sum(o => o.HaberSoles);
                    k.TotalHaberDolares = d.Sum(o => o.HaberDolares);
                    return k;
                }).ToList();
                    if (pIntAsientosDescuadradosAmbos == 1)
                    {
                        RegistrosAgrupados = RegistrosAgrupados.Where(o => o.TotalDebeSoles != o.TotalHaberSoles || o.TotalDebeDolares != o.TotalHaberDolares).ToList();
                    }
                    else
                    {
                        RegistrosAgrupados = RegistrosAgrupados.Where(o => o.TotalDebeSoles != o.TotalHaberSoles).ToList();
                    }
                    RegistrosContabilidaTesoreriaBusqueda = Rg.Where(o => RegistrosAgrupados.Select(p => p.IdCabecera).Contains(o.IdCabecera)).ToList();
                    RegistrosContabilidaTesoreria = RegistrosContabilidaTesoreriaBusqueda.ToList();

                }
                else
                {

                    RegistrosContabilidaTesoreria = RegistrosTesoreria.Concat(RegistrosContabilidadSinDestinos).ToList();

                }
                int Index = 1;
                foreach (var item in RegistrosContabilidaTesoreria)
                {

                    item.Index = Index;
                    ListaTesoreriaContabilidad.Add(item);
                    Index++;
                }


                if (pIntResumen == (int)Resumen.Activado || pIntMostrarSoloResumen == (int)MostrarSoloResumen.Activado)
                {

                    var Cuentas = ListaTesoreriaContabilidad.Select(x => x.Cuenta).Distinct();

                    foreach (var item in Cuentas)
                    {
                        objReporte = new ReporteDocumentosIngresados();
                        int pIntDigitos = 2;
                        var NuevaCuenta = item.Length >= Nivel ? item.Substring(0, Nivel) : "*No Existe*";
                        var ListaEmpiezaCuenta = ListaTesoreriaContabilidad.Where(x => x.Cuenta.StartsWith(NuevaCuenta)).Select(x => x.Cuenta).Distinct().Select(x => x.Substring(0, Nivel)).Distinct();
                        while (pIntDigitos <= Nivel)// pIntNumeroDigitos
                        {
                            objReporte = new ReporteDocumentosIngresados();
                            objReporte.IdCabecera = "";
                            objReporte.Moneda = "";
                            objReporte.NroComprobante = "";
                            objReporte.Fecha = DateTime.Parse("01/01/1981 12:12:10");
                            objReporte.NroDocumento = "";
                            objReporte.Anexo = "";
                            objReporte.Analisis = "";
                            string Cuenta = ListaTesoreriaContabilidad.Where(x => x.Cuenta.StartsWith(item)).Count() != 0 ? ListaTesoreriaContabilidad.Where(x => x.Cuenta.StartsWith(item)).FirstOrDefault().Cuenta.Length >= pIntDigitos ? ListaTesoreriaContabilidad.Where(x => x.Cuenta.StartsWith(item)).FirstOrDefault().Cuenta.Substring(0, pIntDigitos) : "*No Existe*" : "*No Existe*";
                            // string Cuenta = NuevaCuenta;
                            var NomCuenta = NombreCuentaImputable(Cuenta);
                            string NombreCuenta = Cuenta != "*No Existe*" ? NomCuenta.Count() > 0 ? NomCuenta.FirstOrDefault().cuenta : "*No Existe*" : "*No Existe*";
                            if (Cuenta != "*No Existe*" && ListaResumen.Where(x => x.NroCuenta == Cuenta).Count() == 0)
                            {
                                objReporte.Cuenta = Cuenta + "  " + NombreCuenta;
                                objReporte.Caja = 0;
                                objReporte.CentroCosto = "";
                                objReporte.Destino = "";
                                objReporte.DebeSoles = ListaTesoreriaContabilidad.FindAll(x => x.Cuenta.StartsWith(Cuenta)).Sum(x => x.DebeSoles);
                                objReporte.HaberSoles = ListaTesoreriaContabilidad.FindAll(x => x.Cuenta.StartsWith(Cuenta)).Sum(x => x.HaberSoles);
                                objReporte.Dolares = ListaTesoreriaContabilidad.FindAll(x => x.Cuenta.StartsWith(Cuenta)).Sum(x => x.DebeDolares); ; //Debe Dolares
                                objReporte.Grupo1 = 2;
                                objReporte.DebeDolares = ListaTesoreriaContabilidad.FindAll(x => x.Cuenta.StartsWith(Cuenta)).Sum(x => x.DebeDolares);// item.DebeDolares;
                                objReporte.HaberDolares = ListaTesoreriaContabilidad.FindAll(x => x.Cuenta.StartsWith(Cuenta)).Sum(x => x.HaberDolares);
                                objReporte.NroCuenta = Cuenta;
                                objReporte.Index = 0;
                                ListaResumen.Add(objReporte);
                            }
                            pIntDigitos = pIntDigitos + 1;
                        }

                        int Contador = 1;
                        pIntDigitos = 2;

                        if (Nivel != 2)
                        {
                            if (Contador < ListaEmpiezaCuenta.Count())
                            {
                                foreach (var subCuenta in ListaEmpiezaCuenta)
                                {
                                    while (pIntDigitos <= Nivel)
                                    {

                                        objReporte = new ReporteDocumentosIngresados();
                                        objReporte.IdCabecera = "";
                                        objReporte.Moneda = "";
                                        objReporte.NroComprobante = "";
                                        objReporte.Fecha = DateTime.Parse("01/01/1981 12:12:10");
                                        objReporte.NroDocumento = "";
                                        objReporte.Anexo = "";
                                        objReporte.Analisis = "";
                                        //string Cuenta = subCuenta;
                                        string Cuenta = ListaTesoreriaContabilidad.Where(x => x.Cuenta.StartsWith(item)).Count() != 0 ? ListaTesoreriaContabilidad.Where(x => x.Cuenta.StartsWith(item)).FirstOrDefault().Cuenta.Length >= pIntDigitos ? ListaTesoreriaContabilidad.Where(x => x.Cuenta.StartsWith(item)).FirstOrDefault().Cuenta.Substring(0, pIntDigitos) : "*No Existe*" : "*No Existe*";
                                        if (Cuenta != "*No Existe*" && ListaResumen.Where(x => x.NroCuenta == Cuenta).Count() == 0)
                                        {
                                            var NomCuenta = NombreCuentaImputable(Cuenta);
                                            string NombreCuenta = Cuenta != "*No Existe*" ? NomCuenta.Count() > 0 ? NomCuenta.FirstOrDefault().cuenta : "*No Existe*" : "*No Existe*";
                                            objReporte.Cuenta = Cuenta + "  " + NombreCuenta;
                                            objReporte.Caja = 0;
                                            objReporte.CentroCosto = "";
                                            objReporte.Destino = "";
                                            objReporte.DebeSoles = ListaTesoreriaContabilidad.FindAll(x => x.Cuenta.StartsWith(Cuenta)).Sum(x => x.DebeSoles);
                                            objReporte.HaberSoles = ListaTesoreriaContabilidad.FindAll(x => x.Cuenta.StartsWith(Cuenta)).Sum(x => x.HaberSoles);
                                            objReporte.Dolares = 0;
                                            objReporte.Grupo1 = 2;
                                            objReporte.DebeDolares = ListaTesoreriaContabilidad.FindAll(x => x.NroCuenta.StartsWith(Cuenta)).Sum(x => x.DebeDolares); //item.DebeDolares;
                                            objReporte.HaberDolares = ListaTesoreriaContabilidad.FindAll(x => x.NroCuenta.StartsWith(Cuenta)).Sum(x => x.HaberDolares);
                                            objReporte.NroCuenta = Cuenta;
                                            objReporte.Index = 0;
                                            ListaResumen.Add(objReporte);
                                        }
                                        pIntDigitos = pIntDigitos + 1;
                                    }
                                    pIntDigitos = 2;
                                }
                            }
                        }

                    }
                    return ListaTesoreriaContabilidad.Concat(ListaResumen).ToList().OrderBy(x => x.Grupo1).OrderBy(x => x.NroComprobante).OrderBy(x => x.NroCuenta).ToList();
                }
                else
                {
                    return ListaTesoreriaContabilidad.OrderBy(x => x.Grupo1).OrderBy(x => x.NroComprobante).OrderBy(x => x.NroCuenta).ToList();
                }
            }
            catch (Exception ex)
            {



                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "ContabilidadBL.ReporteDocumentosIngresados()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }



        }

        public string BuscarCuentaOrigen(List<ReporteDocumentosIngresados> RegContabilidad, string pstrCuenta, string IdCabecera, List<destinoDto> ListaDestinosOrigen)
        {

            List<string> ListaFinal = new List<string>();
            List<destinoDto> ListasOrigen = new List<destinoDto>();
            ListasOrigen = ListaDestinosOrigen.Where(x => x.v_CuentaDestino == pstrCuenta).ToList();

            foreach (var item in RegContabilidad.Where(x => x.IdCabecera == IdCabecera))
            {

                var CuentaOrigen = ListasOrigen.Where(x => x.v_CuentaOrigen.Trim() == item.Cuenta.Trim()).FirstOrDefault();

                if (CuentaOrigen != null)
                {

                    return CuentaOrigen.v_CuentaOrigen;
                }

            }
            return string.Empty;

            //}
        }


        public List<ReporteCuadroMensualSaldos> ReporteCuadroMensualSaldos(string MesInicio, string MesFin, string CuentaInicio, string CuentaFin, int pIntMoneda, int Naturaleza, int Nivel, int pstrResumen, string periodo, int pImputables)
        {
            List<ReporteCuadroMensualSaldos> ListaFinal = new List<ReporteCuadroMensualSaldos>();
            ReporteCuadroMensualSaldos objReporte = new ReporteCuadroMensualSaldos();
            List<ReporteCuadroMensualSaldos> SaldosRequeridos = new List<ReporteCuadroMensualSaldos>();
            OperationResult objOperationResult = new OperationResult();
            List<string> ListaCuentas = new List<string>();
            List<string> ListaCuentasImputables = new List<string>();
            List<string> ListaImputables = new List<string>();
            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
            List<asientocontable> ListaAsientoContable = dbContext.asientocontable.ToList();
            List<saldoscontablesDto> SaldosContables = new List<saldoscontablesDto>();


            var ObtenerSaldos = new AsientosContablesBL().ObtenerSaldosContablesPorMes(ref  objOperationResult, int.Parse(MesFin), int.Parse(periodo), int.Parse(MesInicio), false, false, null, true, CuentaInicio, CuentaFin);


            SaldosContables = (from n in ObtenerSaldos
                               select n)
                        .OrderBy(x => x.v_NroCuenta).ToList()
                        .GroupBy(x => new { x.v_NroCuenta, x.v_Mes })
                        .Select(x => new saldoscontablesDto
                        {
                            d_ImporteDolaresD = x.Sum(y => y.d_ImporteDolaresD),
                            d_ImporteDolaresH = x.Sum(y => y.d_ImporteDolaresH),
                            d_ImporteSolesD = x.Sum(y => y.d_ImporteSolesD),
                            d_ImporteSolesH = x.Sum(y => y.d_ImporteSolesH),
                            v_NroCuenta = x.FirstOrDefault().v_NroCuenta,
                            v_Mes = x.FirstOrDefault().v_Mes,
                        }).ToList();



            var SaldosContablesFinales = (from n in SaldosContables
                                          join b in dbContext.asientocontable on new { cuenta = n.v_NroCuenta, eliminado = 0, p = periodo }
                                                                           equals new { cuenta = b.v_NroCuenta, eliminado = 0, p = b.v_Periodo } into b_join
                                          from b in b_join.DefaultIfEmpty()
                                          //where   (n.v_NroCuenta != null  && n.v_NroCuenta != string.Empty)
                                          select new ReporteCuadroMensualSaldos
                                          {

                                              Cuenta = n.v_NroCuenta,
                                              NombreCuenta = b == null ? "*No Existe*" : b.v_NombreCuenta.Trim(),
                                              imputable = b == null ? -1 : b.i_Imputable.Value,
                                              DebeSoles = n.d_ImporteSolesD.Value,
                                              HaberSoles = n.d_ImporteSolesH.Value,
                                              DebeDolares = n.d_ImporteDolaresD.Value,
                                              HaberDolares = n.d_ImporteDolaresH.Value,
                                              Mes = n.v_Mes,

                                          }).ToList();

            //SaldosRequeridos = (from n in SaldosContablesFinales
            //                    where int.Parse(n.Cuenta) >= int.Parse(CuentaInicio) && int.Parse(n.Cuenta) <= int.Parse(CuentaFin)
            //                    && int.Parse(n.Mes) >= int.Parse(MesInicio) && int.Parse(n.Mes) <= int.Parse(MesFin)
            //                    select new ReporteCuadroMensualSaldos
            //                    {

            //                        Cuenta = n.Cuenta,
            //                        NombreCuenta = n.NombreCuenta,
            //                        DebeSoles = n.DebeSoles,
            //                        HaberSoles = n.HaberSoles,
            //                        DebeDolares = n.DebeDolares,
            //                        HaberDolares = n.HaberDolares,
            //                        Mes = n.Mes,

            //                    }).ToList();
            SaldosRequeridos = SaldosContablesFinales;
            ListaCuentas = SaldosRequeridos.GroupBy(x => x.Cuenta).ToList().Select(x => x.First()).Select(x => x.Cuenta).ToList();

            if (pstrResumen != (int)Resumen.Activado)
            {
                foreach (var registro in ListaCuentas)
                {
                    objReporte = new ReporteCuadroMensualSaldos();
                    objReporte.Cuenta = registro;
                    objReporte.NombreCuenta = NombreCuenta(objReporte.Cuenta, ListaAsientoContable);
                    objReporte.ValorEnero = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, registro, ((int)(Mes.Enero)).ToString("00"));
                    objReporte.ValorFebrero = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, registro, ((int)(Mes.Febrero)).ToString("00"));
                    objReporte.ValorMarzo = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, registro, ((int)(Mes.Marzo)).ToString("00"));
                    objReporte.ValorAbril = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, registro, ((int)(Mes.Abril)).ToString("00"));
                    objReporte.ValorMayo = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, registro, ((int)(Mes.Mayo)).ToString("00"));
                    objReporte.ValorJunio = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, registro, ((int)(Mes.Junio)).ToString("00"));
                    objReporte.ValorJulio = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, registro, ((int)(Mes.Julio)).ToString("00"));
                    objReporte.ValorAgosto = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, registro, ((int)(Mes.Agosto)).ToString("00"));
                    objReporte.ValorSetiembre = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, registro, ((int)(Mes.Setiembre)).ToString("00"));
                    objReporte.ValorOctubre = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, registro, ((int)(Mes.Octubre)).ToString("00"));
                    objReporte.ValorNoviembre = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, registro, ((int)(Mes.Noviembre)).ToString("00"));
                    objReporte.ValorDiciembre = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, registro, ((int)(Mes.Diciembre)).ToString("00"));
                    objReporte.Total = objReporte.ValorEnero + objReporte.ValorFebrero + objReporte.ValorMarzo + objReporte.ValorAbril + objReporte.ValorMayo + objReporte.ValorJunio + objReporte.ValorJulio + objReporte.ValorAgosto + objReporte.ValorSetiembre + objReporte.ValorOctubre + objReporte.ValorNoviembre + objReporte.ValorDiciembre;
                    ListaFinal.Add(objReporte);
                }


            }
            else
            {
                foreach (var item in ListaCuentas)
                {
                    objReporte = new ReporteCuadroMensualSaldos();
                    int pIntDigitos = 2;
                    ListaImputables = new List<string>();
                    ListaImputables = SaldosRequeridos.Where(x => x.Cuenta.StartsWith(item.Substring(0, 2))).Select(x => x.Cuenta).ToList().Distinct().ToList(); //cuanto pidan resumen

                    while (pIntDigitos <= Nivel)
                    {
                        objReporte = new ReporteCuadroMensualSaldos();
                        objReporte.Cuenta = SaldosRequeridos.Where(x => x.Cuenta.StartsWith(item)).Count() != 0 ? SaldosRequeridos.Where(x => x.Cuenta.StartsWith(item)).FirstOrDefault().Cuenta.Length >= pIntDigitos ? SaldosRequeridos.Where(x => x.Cuenta.StartsWith(item)).FirstOrDefault().Cuenta.Substring(0, pIntDigitos) : "*No Existe*" : "*No Existe*";

                        var CuentaImputable = NombreCuentaImputable(objReporte.Cuenta);
                        string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
                        int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
                        objReporte.NombreCuenta = objReporte.Cuenta != "*No Existe*" ? denominacion : "*No Existe*";

                        if (objReporte.Cuenta != "*No Existe*" && ListaFinal.Where(x => x.Cuenta.StartsWith(objReporte.Cuenta)).Count() == 0)
                        {
                            objReporte.imputable = imputable;
                            objReporte.ValorEnero = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Enero)).ToString("00"));
                            objReporte.ValorFebrero = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Febrero)).ToString("00"));
                            objReporte.ValorMarzo = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Marzo)).ToString("00"));
                            objReporte.ValorAbril = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Abril)).ToString("00"));
                            objReporte.ValorMayo = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Mayo)).ToString("00"));
                            objReporte.ValorJunio = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Junio)).ToString("00"));
                            objReporte.ValorJulio = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Julio)).ToString("00"));
                            objReporte.ValorAgosto = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Agosto)).ToString("00"));
                            objReporte.ValorSetiembre = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Setiembre)).ToString("00"));
                            objReporte.ValorOctubre = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Octubre)).ToString("00"));
                            objReporte.ValorNoviembre = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Noviembre)).ToString("00"));
                            objReporte.ValorDiciembre = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Diciembre)).ToString("00"));
                            objReporte.Total = objReporte.ValorEnero + objReporte.ValorFebrero + objReporte.ValorMarzo + objReporte.ValorAbril + objReporte.ValorMayo + objReporte.ValorJunio + objReporte.ValorJulio + objReporte.ValorAgosto + objReporte.ValorSetiembre + objReporte.ValorOctubre + objReporte.ValorNoviembre + objReporte.ValorDiciembre;
                            ListaFinal.Add(objReporte);
                        }
                        pIntDigitos = pIntDigitos + 1;

                    }

                    int Contador = 1;
                    pIntDigitos = 2;
                    if (Contador < ListaCuentasImputables.Count())
                    {
                        foreach (var subCuenta in ListaCuentasImputables.AsParallel())
                        {
                            while (pIntDigitos <= Nivel)
                            {
                                objReporte = new ReporteCuadroMensualSaldos();
                                objReporte.Cuenta = subCuenta.StartsWith(item) && subCuenta.Length >= pIntDigitos ? subCuenta.Substring(0, pIntDigitos) : "*No Existe*";
                                var CuentaImputable = NombreCuentaImputable(objReporte.Cuenta);
                                string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
                                int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;

                                if (objReporte.Cuenta != "*No Existe*" && ListaFinal.Where(x => x.Cuenta.StartsWith(objReporte.Cuenta)).Count() == 0)
                                {
                                    objReporte.NombreCuenta = objReporte.Cuenta != "*No Existe*" ? denominacion : "*No Existe*";
                                    objReporte.imputable = imputable;
                                    objReporte.ValorEnero = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Enero)).ToString("00"));
                                    objReporte.ValorFebrero = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Febrero)).ToString("00"));
                                    objReporte.ValorMarzo = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Marzo)).ToString("00"));
                                    objReporte.ValorAbril = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Abril)).ToString("00"));
                                    objReporte.ValorMayo = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Mayo)).ToString("00"));
                                    objReporte.ValorJunio = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Junio)).ToString("00"));
                                    objReporte.ValorJulio = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Julio)).ToString("00"));
                                    objReporte.ValorAgosto = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Agosto)).ToString("00"));
                                    objReporte.ValorSetiembre = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Setiembre)).ToString("00"));
                                    objReporte.ValorOctubre = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Octubre)).ToString("00"));
                                    objReporte.ValorNoviembre = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Noviembre)).ToString("00"));
                                    objReporte.ValorDiciembre = CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Diciembre)).ToString("00"));
                                    objReporte.Total = objReporte.ValorEnero + objReporte.ValorFebrero + objReporte.ValorMarzo + objReporte.ValorAbril + objReporte.ValorMayo + objReporte.ValorJunio + objReporte.ValorJulio + objReporte.ValorAgosto + objReporte.ValorSetiembre + objReporte.ValorOctubre + objReporte.ValorNoviembre + objReporte.ValorDiciembre;
                                    ListaFinal.Add(objReporte);

                                }

                                pIntDigitos = pIntDigitos + 1;
                            }
                            pIntDigitos = 2;
                        }
                    }
                }
                if (pImputables == 1)
                {

                    ListaFinal = ListaFinal.Where(x => x.imputable == 1).ToList();
                }

            }
            return ListaFinal.OrderBy(x => x.Cuenta).ToList();
        }


        public List<ReporteAnalisisCuentasCteAnalitico> ReporteAnalisisGasttosFuncioAnalitico(ref OperationResult objOperationResult, DateTime Desde, DateTime Hasta, string CuentaInicial, string CuentaFinal, int TipoReporte)
        {
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {

                var destinos = dbContext.destino.Where(o => o.i_Eliminado == 0 && o.v_Periodo == periodo && o.v_CuentaDestino.StartsWith("9")).Select(p => p.v_CuentaDestino).ToList();
                var origen = dbContext.destino.Where(o => o.i_Eliminado == 0 && o.v_Periodo == periodo).Select(p => p.v_CuentaOrigen).ToList();
                var AnalisisCuentas =
                    new AsientosContablesBL().ReporteAnalisisCuentasCteAnalitico(ref objOperationResult,
                        Desde, Hasta, null, null, false,
                         string.Empty,
                        TipoReporteAnalisis.AnalisisCuentaCorrienteAnalitico);

                var detallesConDestino = AnalisisCuentas.Where(p => destinos.Contains(p.CuentaImputable)).ToList();
                var cuentasGastos = AnalisisCuentas.Where(o => detallesConDestino.Select(z => z.IdDocumentoProvicion).Contains(o.IdDocumentoProvicion)).ToList();
                var cuentasDestinos = cuentasGastos.Where(o => destinos.Contains(o.CuentaImputable)).ToList();
                var CuentasOrigen = cuentasGastos.Where(o => origen.Contains(o.CuentaImputable)).ToList();
                var reporteTotal = cuentasDestinos.Concat(CuentasOrigen).ToList();
                var reporteTotal6 = reporteTotal.Where(o => o.CuentaImputable.StartsWith("6")).ToList();
                reporteTotal6 = reporteTotal6.OrderBy(o => o.CuentaImputable).ToList();
                var reporteTotal1 = reporteTotal6.GroupBy(o => new { ID = o.IdDocumentoProvicion, Cuenta = o.CuentaImputable }).ToList();


                List<ReporteAnalisisCuentasCteAnalitico> ListaFinal = new List<ReporteAnalisisCuentasCteAnalitico>();
                foreach (var item in reporteTotal1)
                {


                    var Id = item.FirstOrDefault().IdDocumentoProvicion;
                    var deta = reporteTotal.Where(o => o.IdDocumentoProvicion == Id).ToList();
                    if (deta.Any(o => o.CuentaImputable.StartsWith("9")))
                    {

                        var deta9 = deta.Where(o => o.CuentaImputable.StartsWith("9")).ToList();
                        foreach (var detallitos in deta9)
                        {

                            if (string.IsNullOrEmpty(CuentaInicial) && string.IsNullOrEmpty(CuentaFinal))
                            {

                                ReporteAnalisisCuentasCteAnalitico objReporte = new ReporteAnalisisCuentasCteAnalitico();
                                objReporte = (ReporteAnalisisCuentasCteAnalitico)detallitos.Clone();
                                if (objReporte.ImporteDolaresD == item.FirstOrDefault().ImporteDolaresD && objReporte.ImporteSolesH == item.FirstOrDefault().ImporteSolesH)
                                {
                                    //objReporte = (ReporteAnalisisCuentasCteAnalitico)item.Clone();
                                    objReporte.Grupo1 = deta9 != null ? detallitos.NombreCuenta.Substring(8, detallitos.NombreCuenta.Length - 8) : "";
                                    objReporte.Grupo2 = item.FirstOrDefault().NombreCuenta.Substring(8, item.FirstOrDefault().NombreCuenta.Length - 8);
                                    //objReporte.ImporteSolesD = item.FirstOrDefault().ImporteSolesD;
                                    //objReporte.ImporteSolesH = item.FirstOrDefault().ImporteSolesH;
                                    //objReporte.ImporteDolaresD = item.FirstOrDefault().ImporteDolaresD;
                                    //objReporte.ImporteDolaresH = item.FirstOrDefault().ImporteDolaresH;
                                    //objReporte.Analisis = item.FirstOrDefault().Analisis;
                                    var ExistenciaKey = ListaFinal.Select(o => o.IdDocumentoProvicionDetalle).ToList();
                                    if (!ExistenciaKey.Contains(objReporte.IdDocumentoProvicionDetalle))
                                    {
                                        ListaFinal.Add(objReporte);
                                    }
                                }

                            }
                            else
                            {
                                if (int.Parse(detallitos.CuentaImputable.Substring(0, 2)) >= int.Parse(CuentaInicial.Substring(0, 2)) && int.Parse(detallitos.CuentaImputable.Substring(0, 2)) <= int.Parse(CuentaFinal.Substring(0, 2)))
                                {
                                    ReporteAnalisisCuentasCteAnalitico objReporte = new ReporteAnalisisCuentasCteAnalitico();
                                    objReporte = (ReporteAnalisisCuentasCteAnalitico)detallitos.Clone();

                                    if (objReporte.ImporteDolaresD == item.FirstOrDefault().ImporteDolaresD && objReporte.ImporteSolesH == item.FirstOrDefault().ImporteSolesH)
                                    {
                                        //objReporte = (ReporteAnalisisCuentasCteAnalitico)item.Clone();
                                        objReporte.Grupo1 = deta9 != null ? detallitos.NombreCuenta.Substring(8, detallitos.NombreCuenta.Length - 8) : "";
                                        objReporte.Grupo2 = item.FirstOrDefault().NombreCuenta.Substring(8, item.FirstOrDefault().NombreCuenta.Length - 8);
                                        //objReporte.ImporteSolesD = item.FirstOrDefault().ImporteSolesD;
                                        //objReporte.ImporteSolesH = item.FirstOrDefault().ImporteSolesH;
                                        //objReporte.ImporteDolaresD = item.FirstOrDefault().ImporteDolaresD;
                                        //objReporte.ImporteDolaresH = item.FirstOrDefault().ImporteDolaresH;
                                        //objReporte.Analisis = item.FirstOrDefault().Analisis;
                                        var ExistenciaKey = ListaFinal.Select(o => o.IdDocumentoProvicionDetalle).ToList();
                                        if (!ExistenciaKey.Contains(objReporte.IdDocumentoProvicionDetalle))
                                        {
                                            ListaFinal.Add(objReporte);
                                        }
                                    }
                                }
                            }
                        }

                    }

                }

                if (TipoReporte == (int)TipoReporteAnalisisGastoFuncion6y9.Analitico)
                {

                    return ListaFinal.OrderBy(o => o.Grupo1).ThenBy(o => o.Grupo2).ThenBy(o => o.DocumentoProvicion).ToList();
                }
                else
                {


                    ListaFinal = ListaFinal.GroupBy(l => new { l.Grupo2 }).Select(d =>
                {

                    var k = d.FirstOrDefault();
                    k.ImporteSolesD = d.Sum(h => h.ImporteSolesD);
                    k.ImporteSolesH = d.Sum(h => h.ImporteSolesH);
                    k.ImporteDolaresH = d.Sum(h => h.ImporteDolaresH);
                    k.ImporteDolaresD = d.Sum(h => h.ImporteDolaresD);
                    return k;
                }).ToList();


                    return ListaFinal;

                }






            }
        }



        private decimal CalcularSaldosActualesPorMesReporteCuadroMensualSaldos(int pstrNaturaleza, int Moneda, List<ReporteCuadroMensualSaldos> Lista, string pstrCuenta, string pstrMes)
        {
            decimal saldo = 0;


            var SaldosAnteriores = Lista.FindAll(x => x.Cuenta.StartsWith(pstrCuenta) && (int.Parse(x.Mes)).ToString("00") == (int.Parse(pstrMes)).ToString("00")).ToList();
            if (Moneda == (int)Currency.Soles)
            {
                if (pstrNaturaleza == (int)Naturaleza.Deudor)
                {
                    saldo = SaldosAnteriores.Any() ? SaldosAnteriores.Sum(o => o.DebeSoles) - SaldosAnteriores.Sum(o => o.HaberSoles) : 0;   //.Sum(x => x.DebeSoles);
                }
                else
                {
                    saldo = SaldosAnteriores.Any() ? SaldosAnteriores.Sum(x => x.HaberSoles) - SaldosAnteriores.Sum(o => o.DebeSoles) : 0;
                }
            }
            else
            {
                if (pstrNaturaleza == (int)Naturaleza.Deudor)
                {
                    saldo = SaldosAnteriores.Any() ? SaldosAnteriores.Sum(o => o.DebeDolares) - SaldosAnteriores.Sum(o => o.HaberDolares) : 0; // .Sum(x => x.DebeDolares);
                }
                else
                {
                    saldo = SaldosAnteriores.Any() ? SaldosAnteriores.Sum(x => x.HaberDolares) - SaldosAnteriores.Sum(x => x.DebeDolares) : 0;
                }

            }

            return saldo;
        }
        public List<ReporteCuadroMensualSaldosCcosto> ReporteCuadroMensualSaldoCcosto(string MesInicio, string MesFin, string CuentaInicio, string CuentaFin, int pIntMoneda, int Naturaleza, int Nivel, int pstrResumen, string periodo, int pImputables, string Ccosto)
        {
            List<ReporteCuadroMensualSaldosCcosto> SaldosRequeridos = new List<ReporteCuadroMensualSaldosCcosto>();
            ReporteCuadroMensualSaldosCcosto objReporte = new ReporteCuadroMensualSaldosCcosto();
            List<ReporteCuadroMensualSaldosCcosto> ListaFinal = new List<ReporteCuadroMensualSaldosCcosto>();
            List<string> ListaCuentasImputables = new List<string>();
            List<string> ListaImputables = new List<string>();

            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                var datahierarchy = dbContext.datahierarchy.ToList();
                var RegTesoreriaA = (from a in dbContext.tesoreria

                                     join b in dbContext.tesoreriadetalle on new { IdTesoreria = a.v_IdTesoreria, eliminado = 0 } equals new { IdTesoreria = b.v_IdTesoreria, eliminado = b.i_Eliminado.Value } into b_join

                                     from b in b_join.DefaultIfEmpty()

                                     join c in dbContext.asientocontable on new { Cuenta = b.v_NroCuenta, eliminado = 0, p = periodo } equals new { Cuenta = c.v_NroCuenta, eliminado = c.i_Eliminado.Value, p = c.v_Periodo } into c_join

                                     from c in c_join.DefaultIfEmpty()

                                     join d in dbContext.datahierarchy on new { pCcosto = b.i_IdCentroCostos, eliminado = 0, Grupo = 31 } equals new { pCcosto = d.v_Value2, eliminado = d.i_IsDeleted.Value, Grupo = d.i_GroupId } into d_join

                                     from d in d_join.DefaultIfEmpty()
                                     where a.i_Eliminado == 0 && a.v_Periodo == periodo && (b.i_IdCentroCostos == Ccosto || Ccosto == "") && b.v_NroCuenta != null && a.i_IdEstado == 1

                                     select new ReporteCuadroMensualSaldosCcosto
                                     {
                                         Cuenta = b == null ? "*No Existe*" : b.v_NroCuenta.Trim(),
                                         NombreCuenta = c == null ? "*No Existe*" : c.v_NombreCuenta.Trim(),
                                         imputable = c == null ? -1 : c.i_Imputable.Value,
                                         DebeSoles = b == null ? 0 : a.i_IdMoneda == (int)Currency.Soles && b.v_Naturaleza == "D" ? b.d_Importe.Value : a.i_IdMoneda == (int)Currency.Dolares && b.v_Naturaleza == "D" ? b.d_Cambio.Value : 0,
                                         HaberSoles = b == null ? 0 : a.i_IdMoneda == (int)Currency.Soles && b.v_Naturaleza == "H" ? b.d_Importe.Value : a.i_IdMoneda == (int)Currency.Dolares && b.v_Naturaleza == "H" ? b.d_Cambio.Value : 0,
                                         DebeDolares = b == null ? 0 : a.i_IdMoneda == (int)Currency.Dolares && b.v_Naturaleza == "D" ? b.d_Importe.Value : a.i_IdMoneda == (int)Currency.Soles && b.v_Naturaleza == "D" ? b.d_Cambio.Value : 0,
                                         HaberDolares = b == null ? 0 : a.i_IdMoneda == (int)Currency.Dolares && b.v_Naturaleza == "H" ? b.d_Importe.Value : a.i_IdMoneda == (int)Currency.Soles && b.v_Naturaleza == "H" ? b.d_Cambio.Value : 0,
                                         Mes = a.v_Mes.Trim(),
                                         Ccosto = b == null ? "*No Existe Detalle*" : d == null ? "Sin Centro Costo" : b.i_IdCentroCostos,
                                         IdCcosto = b == null ? "*No Existe Detalle*" : d == null ? "Sin Centro Costo" : b.i_IdCentroCostos,
                                         ValueCcosto = b == null || d == null ? "" : d.v_Value2.Trim() + " " + d.v_Value1.Trim(),
                                     }).ToList();

                var RegTesoreria = (from a in RegTesoreriaA

                                    select new ReporteCuadroMensualSaldosCcosto
                                    {
                                        Cuenta = a.Cuenta,
                                        NombreCuenta = a.NombreCuenta,
                                        imputable = a.imputable,
                                        DebeSoles = a.DebeSoles,
                                        HaberSoles = a.HaberSoles,
                                        DebeDolares = a.DebeDolares,
                                        HaberDolares = a.HaberDolares,
                                        Mes = a.Mes,
                                        Ccosto = a.Ccosto == "*No Existe Detalle*" ? "*No Existe Detalle*" : a.Ccosto == "Sin Centro Costo" ? "Sin Centro Costo" : !EsCentroCostoValido(a.IdCcosto, datahierarchy) ? "" : a.ValueCcosto,
                                        IdCcosto = a.IdCcosto == "*No Existe Detalle*" ? "*No Existe Detalle" : a.IdCcosto == "Sin Centro Costo" ? "Sin Centro Costo" : !EsCentroCostoValido(a.IdCcosto, datahierarchy) ? "" : a.IdCcosto,
                                        ValueCcosto = a.ValueCcosto,

                                    }).ToList();

                var RegDiarioA = (from a in dbContext.diario

                                  join b in dbContext.diariodetalle on new { IdTesoreria = a.v_IdDiario, eliminado = 0 } equals new { IdTesoreria = b.v_IdDiario, eliminado = b.i_Eliminado.Value } into b_join

                                  from b in b_join.DefaultIfEmpty()

                                  join c in dbContext.asientocontable on new { Cuenta = b.v_NroCuenta, eliminado = 0, p = periodo } equals new { Cuenta = c.v_NroCuenta, eliminado = c.i_Eliminado.Value, p = c.v_Periodo } into c_join

                                  from c in c_join.DefaultIfEmpty()

                                  join d in dbContext.datahierarchy on new { pCcosto = b.i_IdCentroCostos, eliminado = 0, Grupo = 31 } equals new { pCcosto = d.v_Value2, eliminado = d.i_IsDeleted.Value, Grupo = d.i_GroupId } into d_join

                                  from d in d_join.DefaultIfEmpty()

                                  where a.i_Eliminado == 0 && a.v_Periodo == periodo && (b.i_IdCentroCostos == Ccosto || Ccosto == "") && b.v_NroCuenta != null

                                  select new ReporteCuadroMensualSaldosCcosto
                                  {
                                      Cuenta = b == null ? "*No Existe*" : b.v_NroCuenta.Trim(),
                                      NombreCuenta = c == null ? "*No Existe*" : c.v_NombreCuenta.Trim(),
                                      imputable = c == null ? -1 : c.i_Imputable.Value,
                                      DebeSoles = b == null ? 0 : a.i_IdMoneda == (int)Currency.Soles && b.v_Naturaleza == "D" ? b.d_Importe.Value : a.i_IdMoneda == (int)Currency.Dolares && b.v_Naturaleza == "D" ? b.d_Cambio.Value : 0,
                                      HaberSoles = b == null ? 0 : a.i_IdMoneda == (int)Currency.Soles && b.v_Naturaleza == "H" ? b.d_Importe.Value : a.i_IdMoneda == (int)Currency.Dolares && b.v_Naturaleza == "H" ? b.d_Cambio.Value : 0,
                                      DebeDolares = b == null ? 0 : a.i_IdMoneda == (int)Currency.Dolares && b.v_Naturaleza == "D" ? b.d_Importe.Value : a.i_IdMoneda == (int)Currency.Soles && b.v_Naturaleza == "D" ? b.d_Cambio.Value : 0,
                                      HaberDolares = b == null ? 0 : a.i_IdMoneda == (int)Currency.Dolares && b.v_Naturaleza == "H" ? b.d_Importe.Value : a.i_IdMoneda == (int)Currency.Soles && b.v_Naturaleza == "H" ? b.d_Cambio.Value : 0,
                                      Mes = a.v_Mes.Trim(),
                                      Ccosto = b == null ? "*No Existe Detalle*" : d == null ? "Sin Centro Costo" : b.i_IdCentroCostos,
                                      IdCcosto = b == null ? "*No Existe Detalle*" : d == null ? "Sin Centro Costo" : b.i_IdCentroCostos,
                                      //Ccosto = b == null ? "*No Existe Detalle*" : d == null ? "Sin Centro Costo" : b.i_IdCentroCostos == null || b.i_IdCentroCostos == string.Empty || b.i_IdCentroCostos == "0" || b.i_IdCentroCostos == "-1" ? "" : d.v_Value2.Trim() + " " + d.v_Value1.Trim(),
                                      //IdCcosto = b == null ? "*No Existe Detalle*" : d == null ? "Sin Centro Costo" : b.i_IdCentroCostos == null || b.i_IdCentroCostos == string.Empty || b.i_IdCentroCostos == "0" || b.i_IdCentroCostos == "-1" ? "" : b.i_IdCentroCostos,
                                      ValueCcosto = b == null || d == null ? "" : d.v_Value2.Trim() + " " + d.v_Value1.Trim(),

                                  }).ToList();

                var RegDiario = (from a in RegDiarioA

                                 select new ReporteCuadroMensualSaldosCcosto
                                 {
                                     Cuenta = a.Cuenta,
                                     NombreCuenta = a.NombreCuenta,
                                     imputable = a.imputable,
                                     DebeSoles = a.DebeSoles,
                                     HaberSoles = a.HaberSoles,
                                     DebeDolares = a.DebeDolares,
                                     HaberDolares = a.HaberDolares,
                                     Mes = a.Mes,
                                     Ccosto = a.Ccosto == "*No Existe Detalle*" ? "*No Existe Detalle*" : a.Ccosto == "Sin Centro Costo" ? "Sin Centro Costo" : !EsCentroCostoValido(a.IdCcosto, datahierarchy) ? "" : a.ValueCcosto,
                                     IdCcosto = a.IdCcosto == "*No Existe Detalle*" ? "*No Existe Detalle" : a.IdCcosto == "Sin Centro Costo" ? "Sin Centro Costo" : !EsCentroCostoValido(a.IdCcosto, datahierarchy) ? "" : a.IdCcosto,
                                     //Ccosto = b == null ? "*No Existe*" : d == null ? "Sin Centro Costo" : !EsCentroCostoValido(b.i_IdCentroCostos) ? "" : d.v_Value2.Trim() + " " + d.v_Value1.Trim(),
                                     //IdCcosto = b == null ? "*No Existe*" : d == null ? "Sin Centro Costo" : !EsCentroCostoValido(b.i_IdCentroCostos) ? "" : b.i_IdCentroCostos,
                                     ValueCcosto = a.ValueCcosto,
                                 }).ToList();


                var TodosRegistros = RegTesoreria.Concat(RegDiario).ToList();
                SaldosRequeridos = (from n in TodosRegistros
                                    where int.Parse(n.Cuenta) >= int.Parse(CuentaInicio) && int.Parse(n.Cuenta) <= int.Parse(CuentaFin)
                                    && int.Parse(n.Mes) >= int.Parse(MesInicio) && int.Parse(n.Mes) <= int.Parse(MesFin)
                                    select new ReporteCuadroMensualSaldosCcosto
                                    {

                                        Cuenta = n.Cuenta == null ? "" : n.Cuenta,
                                        NombreCuenta = n.NombreCuenta == null ? "" : n.NombreCuenta,
                                        DebeSoles = n.DebeSoles == null ? 0 : n.DebeSoles,
                                        HaberSoles = n.HaberSoles == null ? 0 : n.HaberSoles,
                                        DebeDolares = n.DebeDolares == null ? 0 : n.DebeDolares,
                                        HaberDolares = n.HaberDolares == null ? 0 : n.HaberDolares,
                                        Mes = n.Mes == null ? "" : n.Mes,
                                        Ccosto = n.Ccosto == null ? "" : n.Ccosto,
                                        IdCcosto = n.IdCcosto == null ? "" : n.IdCcosto,

                                    }).ToList();

                var ListaCuentas = SaldosRequeridos.GroupBy(x => new { y = x.Cuenta, a = x.IdCcosto }).ToList().Select(x => x.First()).Select(x => new { Cuenta = x.Cuenta, CentroCosto = x.Ccosto, IdCentroCosto = x.IdCcosto }).ToList();

                // var CentrosCostosRequeridos = SaldosRequeridos.GroupBy(x => x.IdCcosto).Select(x => x.First()).Select(x => x.IdCcosto); 
                List<asientocontable> ListaAsientoContable = dbContext.asientocontable.ToList();
                if (pstrResumen != (int)Resumen.Activado)
                {
                    foreach (var registro in ListaCuentas)
                    {
                        objReporte = new ReporteCuadroMensualSaldosCcosto();
                        objReporte.Cuenta = registro.Cuenta;
                        objReporte.NombreCuenta = NombreCuenta(objReporte.Cuenta, ListaAsientoContable);
                        objReporte.ValorEnero = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Enero)).ToString("00"), registro.IdCentroCosto);
                        objReporte.ValorFebrero = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Febrero)).ToString("00"), registro.IdCentroCosto);
                        objReporte.ValorMarzo = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Marzo)).ToString("00"), registro.IdCentroCosto);
                        objReporte.ValorAbril = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Abril)).ToString("00"), registro.IdCentroCosto);
                        objReporte.ValorMayo = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Mayo)).ToString("00"), registro.IdCentroCosto);
                        objReporte.ValorJunio = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Junio)).ToString("00"), registro.IdCentroCosto);
                        objReporte.ValorJulio = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Julio)).ToString("00"), registro.IdCentroCosto);
                        objReporte.ValorAgosto = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Agosto)).ToString("00"), registro.IdCentroCosto);
                        objReporte.ValorSetiembre = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Setiembre)).ToString("00"), registro.IdCentroCosto);
                        objReporte.ValorOctubre = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Octubre)).ToString("00"), registro.IdCentroCosto);
                        objReporte.ValorNoviembre = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Noviembre)).ToString("00"), registro.IdCentroCosto);
                        objReporte.ValorDiciembre = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Diciembre)).ToString("00"), registro.IdCentroCosto);
                        objReporte.Total = objReporte.ValorEnero + objReporte.ValorFebrero + objReporte.ValorMarzo + objReporte.ValorAbril + objReporte.ValorMayo + objReporte.ValorJunio + objReporte.ValorJulio + objReporte.ValorAgosto + objReporte.ValorSetiembre + objReporte.ValorOctubre + objReporte.ValorNoviembre + objReporte.ValorDiciembre;
                        objReporte.Ccosto = registro.CentroCosto;
                        objReporte.IdCcosto = registro.IdCentroCosto;
                        ListaFinal.Add(objReporte);
                    }
                }
                else
                {
                    foreach (var item in ListaCuentas)
                    {
                        objReporte = new ReporteCuadroMensualSaldosCcosto();
                        int pIntDigitos = 2;
                        ListaImputables = new List<string>();
                        ListaImputables = SaldosRequeridos.Where(x => x.Cuenta.StartsWith(item.Cuenta.Substring(0, 2)) && x.IdCcosto == item.IdCentroCosto).Select(x => x.Cuenta).ToList().Distinct().ToList(); //cuanto pidan resumen
                        while (pIntDigitos <= Nivel)
                        {
                            objReporte = new ReporteCuadroMensualSaldosCcosto();
                            objReporte.Cuenta = SaldosRequeridos.Where(x => x.Cuenta.StartsWith(item.Cuenta)).Count() != 0 ? SaldosRequeridos.Where(x => x.Cuenta.StartsWith(item.Cuenta)).FirstOrDefault().Cuenta.Length >= pIntDigitos ? SaldosRequeridos.Where(x => x.Cuenta.StartsWith(item.Cuenta)).FirstOrDefault().Cuenta.Substring(0, pIntDigitos) : "*No Existe*" : "*No Existe*";
                            var CuentaImputable = NombreCuentaImputable(objReporte.Cuenta);
                            string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
                            int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
                            objReporte.NombreCuenta = objReporte.Cuenta != "*No Existe*" ? denominacion : "*No Existe*";

                            if (objReporte.Cuenta != "*No Existe*" && ListaFinal.Where(x => x.Cuenta.StartsWith(objReporte.Cuenta) && x.IdCcosto == item.IdCentroCosto).Count() == 0)
                            {
                                objReporte.imputable = imputable;
                                objReporte.ValorEnero = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Enero)).ToString("00"), item.IdCentroCosto);
                                objReporte.ValorFebrero = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Febrero)).ToString("00"), item.IdCentroCosto);
                                objReporte.ValorMarzo = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Marzo)).ToString("00"), item.IdCentroCosto);
                                objReporte.ValorAbril = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Abril)).ToString("00"), item.IdCentroCosto);
                                objReporte.ValorMayo = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Mayo)).ToString("00"), item.IdCentroCosto);
                                objReporte.ValorJunio = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Junio)).ToString("00"), item.IdCentroCosto);
                                objReporte.ValorJulio = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Julio)).ToString("00"), item.IdCentroCosto);
                                objReporte.ValorAgosto = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Agosto)).ToString("00"), item.IdCentroCosto);
                                objReporte.ValorSetiembre = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Setiembre)).ToString("00"), item.IdCentroCosto);
                                objReporte.ValorOctubre = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Octubre)).ToString("00"), item.IdCentroCosto);
                                objReporte.ValorNoviembre = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Noviembre)).ToString("00"), item.IdCentroCosto);
                                objReporte.ValorDiciembre = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Diciembre)).ToString("00"), item.IdCentroCosto);
                                objReporte.Total = objReporte.ValorEnero + objReporte.ValorFebrero + objReporte.ValorMarzo + objReporte.ValorAbril + objReporte.ValorMayo + objReporte.ValorJunio + objReporte.ValorJulio + objReporte.ValorAgosto + objReporte.ValorSetiembre + objReporte.ValorOctubre + objReporte.ValorNoviembre + objReporte.ValorDiciembre;
                                objReporte.Ccosto = item.CentroCosto;
                                objReporte.IdCcosto = item.IdCentroCosto;

                                ListaFinal.Add(objReporte);
                            }
                            pIntDigitos = pIntDigitos + 1;

                        }

                        int Contador = 1;
                        pIntDigitos = 2;
                        if (Contador < ListaCuentasImputables.Count())
                        {
                            foreach (var subCuenta in ListaCuentasImputables.AsParallel())
                            {
                                while (pIntDigitos <= Nivel)
                                {
                                    objReporte = new ReporteCuadroMensualSaldosCcosto();
                                    objReporte.Cuenta = subCuenta.StartsWith(item.Cuenta) && subCuenta.Length >= pIntDigitos ? subCuenta.Substring(0, pIntDigitos) : "*No Existe*";
                                    var CuentaImputable = NombreCuentaImputable(objReporte.Cuenta);
                                    string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
                                    int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;

                                    if (objReporte.Cuenta != "*No Existe*" && ListaFinal.Where(x => x.Cuenta.StartsWith(objReporte.Cuenta) && x.IdCcosto == item.IdCentroCosto).Count() == 0)
                                    {
                                        objReporte.NombreCuenta = objReporte.Cuenta != "*No Existe*" ? denominacion : "*No Existe*";
                                        objReporte.imputable = imputable;
                                        objReporte.ValorEnero = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Enero)).ToString("00"), item.IdCentroCosto);
                                        objReporte.ValorFebrero = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Febrero)).ToString("00"), item.IdCentroCosto);
                                        objReporte.ValorMarzo = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Marzo)).ToString("00"), item.IdCentroCosto);
                                        objReporte.ValorAbril = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Abril)).ToString("00"), item.IdCentroCosto);
                                        objReporte.ValorMayo = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Mayo)).ToString("00"), item.IdCentroCosto);
                                        objReporte.ValorJunio = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Junio)).ToString("00"), item.IdCentroCosto);
                                        objReporte.ValorJulio = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Julio)).ToString("00"), item.IdCentroCosto);
                                        objReporte.ValorAgosto = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Agosto)).ToString("00"), item.IdCentroCosto);
                                        objReporte.ValorSetiembre = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Setiembre)).ToString("00"), item.IdCentroCosto);
                                        objReporte.ValorOctubre = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Octubre)).ToString("00"), item.IdCentroCosto);
                                        objReporte.ValorNoviembre = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Noviembre)).ToString("00"), item.IdCentroCosto);
                                        objReporte.ValorDiciembre = CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(Naturaleza, pIntMoneda, SaldosRequeridos, objReporte.Cuenta, ((int)(Mes.Diciembre)).ToString("00"), item.IdCentroCosto);
                                        objReporte.Total = objReporte.ValorEnero + objReporte.ValorFebrero + objReporte.ValorMarzo + objReporte.ValorAbril + objReporte.ValorMayo + objReporte.ValorJunio + objReporte.ValorJulio + objReporte.ValorAgosto + objReporte.ValorSetiembre + objReporte.ValorOctubre + objReporte.ValorNoviembre + objReporte.ValorDiciembre;
                                        objReporte.Ccosto = item.CentroCosto;

                                        objReporte.IdCcosto = item.IdCentroCosto;
                                        ListaFinal.Add(objReporte);

                                    }

                                    pIntDigitos = pIntDigitos + 1;
                                }
                                pIntDigitos = 2;
                            }
                        }
                    }
                    if (pImputables == 1)
                    {
                        // return ListaHojaTrabajo.ToList().Where(y => y.imputable == 1).OrderBy(x => x.cuenta).ToList();
                        ListaFinal = ListaFinal.Where(x => x.imputable == 1).ToList();
                    }

                }
                return ListaFinal.OrderBy(x => x.Cuenta).ToList();

            }

        }

        private bool EsCentroCostoValido(string CC, List<datahierarchy> ListaDatahierarchy)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var CCs = ListaDatahierarchy.Where(p => p.i_GroupId == 31 && p.i_IsDeleted == 0).ToList().Select(o => o.v_Value2).ToList();

                    return CCs.Contains(CC);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private decimal CalcularSaldosActualesPorMesReporteCuadroMensualSaldosCcosto(int pstrNaturaleza, int Moneda, List<ReporteCuadroMensualSaldosCcosto> Lista, string pstrCuenta, string pstrMes, string Ccosto)
        {
            decimal saldo = 0;
            if (Moneda == (int)Currency.Soles)
            {
                if (pstrNaturaleza == (int)Naturaleza.Deudor)
                {
                    saldo = Lista.FindAll(x => x.Cuenta.StartsWith(pstrCuenta) && x.Mes == pstrMes && x.IdCcosto == Ccosto).Sum(x => x.DebeSoles);
                }
                else
                {
                    saldo = Lista.FindAll(x => x.Cuenta.StartsWith(pstrCuenta) && x.Mes == pstrMes && x.IdCcosto == Ccosto).Sum(x => x.HaberSoles);
                }
            }
            else
            {
                if (pstrNaturaleza == (int)Naturaleza.Deudor)
                {
                    saldo = Lista.FindAll(x => x.Cuenta.StartsWith(pstrCuenta) && x.Mes == pstrMes && x.IdCcosto == Ccosto).Sum(x => x.DebeDolares);
                }
                else
                {
                    saldo = Lista.FindAll(x => x.Cuenta.StartsWith(pstrCuenta) && x.Mes == pstrMes && x.IdCcosto == Ccosto).Sum(x => x.HaberDolares);
                }

            }

            return saldo;
        }


        public string NombreCentroCosto(string IdCcosto, List<datahierarchyDto> ListaCentroCosto)
        {

            string NombreCentroCosto = "";
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                //var CentroCosto = dbContext.datahierarchy.Where(o => o.i_GroupId == 31 && o.i_IsDeleted == 0 && o.v_Value2.Trim() == IdCcosto.Trim()).FirstOrDefault();
                var CentroCosto = ListaCentroCosto.Where(o => o.v_Value2.Trim() == IdCcosto.Trim()).FirstOrDefault();
                if (CentroCosto != null)
                {
                    return CentroCosto.v_Value1;
                }
                else
                {
                    return NombreCentroCosto;
                }
            }

        }







        public decimal SumaImporteMovimientoPorFuncionCcosto(List<ReporteGananciasPerdidasporFuncionCcosto> ListaTotal, string pstrNaturaleza, int pIntMoneda, string pstrCuenta, string IdCcosto, string pstrMes, bool Acumulado)
        {
            decimal ImporteMovimiento = 0;
            if (Acumulado)
            {
                var ListaImputables = ListaTotal.FindAll(x => x.pstrCuenta.StartsWith(pstrCuenta) && x.IdCcosto == IdCcosto).ToList();

                if (pIntMoneda == 1)
                {
                    ImporteMovimiento = pstrNaturaleza == "D" ? ListaImputables.FindAll(x => x.Naturaleza == pstrNaturaleza).Sum(x => x.DebeSoles) : ListaImputables.FindAll(x => x.Naturaleza == pstrNaturaleza).Sum(x => x.HaberSoles);
                }
                else
                {
                    ImporteMovimiento = pstrNaturaleza == "D" ? ListaImputables.FindAll(x => x.Naturaleza == pstrNaturaleza).Sum(x => x.DebeDolares) : ListaImputables.FindAll(x => x.Naturaleza == pstrNaturaleza).Sum(x => x.HaberDolares);
                }
            }
            else
            {
                var ListaImputables = ListaTotal.Where(x => x.pstrCuenta.StartsWith(pstrCuenta) && x.IdCcosto == IdCcosto && x.mes == pstrMes).ToList();

                if (pIntMoneda == 1)
                {
                    ImporteMovimiento = pstrNaturaleza == "D" ? ListaImputables.FindAll(x => x.Naturaleza == pstrNaturaleza).Sum(x => x.DebeSoles) : ListaImputables.FindAll(x => x.Naturaleza == pstrNaturaleza).Sum(x => x.HaberSoles);
                }
                else
                {
                    ImporteMovimiento = pstrNaturaleza == "D" ? ListaImputables.FindAll(x => x.Naturaleza == pstrNaturaleza).Sum(x => x.DebeDolares) : ListaImputables.FindAll(x => x.Naturaleza == pstrNaturaleza).Sum(x => x.HaberDolares);
                }

            }
            return ImporteMovimiento;
        }







        //public decimal SumaImporteMovimientoPorFuncionCcosto_(List<ReporteGananciasPerdidasporNaturalezaCcosto> ListaTotal, string pstrNaturaleza, int pIntMoneda, string pstrCuenta, string IdCcosto, string pstrMes, bool Acumulado, int Imputable)
        //{
        //    decimal ImporteMovimiento = 0;


        //    if (Acumulado)
        //    {
        //        var ListaImputables = ListaTotal.FindAll(x => x.pstrCuenta.StartsWith(pstrCuenta) && x.Ccosto == IdCcosto).ToList();
        //        ImporteMovimiento = pIntMoneda == 1 ? pstrNaturaleza == "D" ? ListaImputables.Sum(x => x.DebeSoles) : ListaImputables.Sum(x => x.HaberSoles) : pstrNaturaleza == "D" ? ListaImputables.Sum(x => x.DebeDolares) : ListaImputables.Sum(x => x.HaberDolares);
        //    }
        //    else
        //    {
        //        var ListaImputables = ListaTotal.Where(x => x.pstrCuenta.StartsWith(pstrCuenta) && x.Ccosto == IdCcosto && x.mes == pstrMes).ToList();
        //        ImporteMovimiento = pIntMoneda == 1 ? pstrNaturaleza == "D" ? ListaImputables.Sum(x => x.DebeSoles) : ListaImputables.Sum(x => x.HaberSoles) : pstrNaturaleza == "D" ? ListaImputables.Sum(x => x.DebeDolares) : ListaImputables.Sum(x => x.HaberDolares);
        //    }
        //    return ImporteMovimiento;
        //}

        public decimal SumaImporteMovimientoPorFuncionCcosto(List<ReporteGananciasPerdidasporFuncionCcosto> ListaTotal, string pstrNaturaleza, int pIntMoneda, string pstrCuenta, string IdCcosto, string pstrMes, bool Acumulado, int Imputable)
        {
            decimal ImporteMovimiento = 0;


            if (Acumulado)
            {
                var ListaImputables = ListaTotal.FindAll(x => x.pstrCuenta.StartsWith(pstrCuenta) && x.Ccosto == IdCcosto).ToList();
                ImporteMovimiento = pIntMoneda == 1 ? pstrNaturaleza == "D" ? ListaImputables.Sum(x => x.DebeSoles) : ListaImputables.Sum(x => x.HaberSoles) : pstrNaturaleza == "D" ? ListaImputables.Sum(x => x.DebeDolares) : ListaImputables.Sum(x => x.HaberDolares);
            }
            else
            {
                var ListaImputables = ListaTotal.Where(x => x.pstrCuenta.StartsWith(pstrCuenta) && x.Ccosto == IdCcosto && x.mes == pstrMes).ToList();
                ImporteMovimiento = pIntMoneda == 1 ? pstrNaturaleza == "D" ? ListaImputables.Sum(x => x.DebeSoles) : ListaImputables.Sum(x => x.HaberSoles) : pstrNaturaleza == "D" ? ListaImputables.Sum(x => x.DebeDolares) : ListaImputables.Sum(x => x.HaberDolares);
            }
            return ImporteMovimiento;
        }
        public List<ReporteGananciasPerdidasporFuncionCcosto> ReporteGananciasPerdidasporFuncionCcosto(ref OperationResult objOperationResult, string pstrPeriodo, string pstrMes, int pIntNumeroDigitos, int pIntMoneda, int pImputables, string Ccosto, bool Detallado, bool UsadoDesdeEstadoResultado = false, List<asientocontableDto> ListaAsociadaBalanceFuncion = null, string MesDesde = "01", string MesHasta = "12")
        {
            //Ultimo
            try
            {
                objOperationResult.Success = 1;
                AsientosContablesBL _objAsientoContableBL = new AsientosContablesBL();
                List<ReporteGananciasPerdidasporFuncionCcosto> ListaFinal = new List<ReporteGananciasPerdidasporFuncionCcosto>();
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                List<CuentaMayorCcosto> CuentasMayores = new List<CuentaMayorCcosto>();
                ReporteGananciasPerdidasporFuncionCcosto objReporte = new ReporteGananciasPerdidasporFuncionCcosto();
                List<CuentaMayorCcosto> ListaCuentasImputables = new List<CuentaMayorCcosto>();
                List<saldoscontablesDto> Saldoscontables = new List<saldoscontablesDto>();
                #region RecopilaCuentasExistentesMesRequerido
                if (UsadoDesdeEstadoResultado)
                {
                    Saldoscontables = _objAsientoContableBL.ObtenerSaldosContablesPorMes(ref  objOperationResult, int.Parse(pstrMes), int.Parse(pstrPeriodo), 1, true, false, null, true);
                    Saldoscontables = Saldoscontables.Where(o => ListaAsociadaBalanceFuncion.Select(x => x.v_NroCuenta).Contains(o.v_NroCuenta)).ToList();
                }
                else
                {
                    Saldoscontables = _objAsientoContableBL.ObtenerSaldosContablesPorMes(ref  objOperationResult, int.Parse(pstrMes), int.Parse(pstrPeriodo), 1, true);
                }
                // List<saldoscontablesDto> Saldoscontables = UsadoDesdeEstadoResultadoFuncion ?  :;
                if (objOperationResult.Success == 0) return null;
                List<datahierarchyDto> ListaCentroCostos = dbContext.datahierarchy.Where(o => o.i_GroupId == 31 && o.i_IsDeleted == 0).ToList().ToDTOs();
                if (!string.IsNullOrEmpty(Ccosto))
                {
                    //Saldoscontables = Saldoscontables.Where(o => o.IdCentroCosto == Ccosto.Trim() + "-" + NombreCentroCosto(Ccosto.Trim(), ListaCentroCostos)).ToList();
                    var CC = Saldoscontables.Where(p => string.IsNullOrEmpty(p.IdCentroCosto)).ToList();
                    Saldoscontables = Saldoscontables.Where(o => o.IdCentroCosto == Ccosto.Trim()).ToList();
                }
                var CuentasMesRequerido = (from a in Saldoscontables

                                           join b in dbContext.asientocontable on new { cuenta = a.v_NroCuenta, eliminado = 0, p = periodo }
                                                                                 equals new { cuenta = b.v_NroCuenta, eliminado = b.i_Eliminado.Value, p = b.v_Periodo } into b_join
                                           from b in b_join.DefaultIfEmpty()

                                           where (a.v_NroCuenta != null && a.v_NroCuenta != "" && a.v_NroCuenta != "*Cta Eliminada*")  // && a.v_Mes == pstrMes  

                                           select new ReporteGananciasPerdidasporFuncionCcosto
                                           {
                                               pstrCuenta = a == null ? "Cuenta no Existe" : a.v_NroCuenta.Trim(),
                                               pstrDenominacion = b == null ? "*No Existe*" : b.v_NombreCuenta.Trim(),
                                               mes = int.Parse(a.v_Mes).ToString("00"),
                                               imputable = b == null ? -1 : b.i_Imputable.Value,
                                               DebeSoles = a.d_ImporteSolesD.Value,
                                               DebeDolares = a.d_ImporteDolaresD.Value,
                                               HaberSoles = a.d_ImporteSolesH.Value,
                                               HaberDolares = a.d_ImporteDolaresH.Value,
                                               Ccosto = a == null ? "" : a.IdCentroCosto == null ? "" : string.IsNullOrEmpty(a.IdCentroCosto.Trim()) ? "" : a.IdCentroCosto + "-" + NombreCentroCosto(a.IdCentroCosto, ListaCentroCostos),
                                               Naturaleza = a.Naturaleza,
                                               EsCuentaPerdidaFuncion = UsadoDesdeEstadoResultado ? true : a == null ? false : CuentaFuncion(a.v_NroCuenta.Trim()),
                                               IdCcosto = a.IdCentroCosto == null ? "" : string.IsNullOrEmpty(a.IdCentroCosto) ? "" : a.IdCentroCosto.Trim(),
                                           }).ToList();//.OrderBy(o => o.pstrCuenta).ToList();

                var CuentasMesRequeridoGananciaFuncion = CuentasMesRequerido.Where(o => o.EsCuentaPerdidaFuncion).ToList();

                foreach (var item in CuentasMesRequeridoGananciaFuncion.AsParallel())
                {
                    CuentaMayorCcosto ojbCuentaMayr = new CuentaMayorCcosto();
                    string key = string.IsNullOrEmpty(item.Ccosto.Trim()) ? item.pstrCuenta.Substring(0, 2) + "" : item.pstrCuenta.Substring(0, 2) + item.Ccosto;
                    if (!CuentasMayores.Where(o => o.Key == key).Any())
                    {
                        ojbCuentaMayr.Cuenta = item.pstrCuenta.Substring(0, 2);
                        ojbCuentaMayr.Ccosto = string.IsNullOrEmpty(item.Ccosto.Trim()) ? "" : item.Ccosto;
                        //ojbCuentaMayr.Key = ojbCuentaMayr.Cuenta + ojbCuentaMayr.Ccosto;
                        ojbCuentaMayr.IdCosto = string.IsNullOrEmpty(item.IdCcosto) ? "" : item.IdCcosto.Trim();
                        CuentasMayores.Add(ojbCuentaMayr);
                    }
                }

                CuentasMayores = CuentasMayores.OrderBy(o => o.Cuenta).ToList();

                foreach (var item in CuentasMayores.AsParallel())
                {

                    int pIntDigitos = 2;
                    List<ReporteGananciasPerdidasporFuncionCcosto> ListaImputables = CuentasMesRequeridoGananciaFuncion.Where(x => x.pstrCuenta.StartsWith(item.Cuenta.Substring(0, 2)) && x.Ccosto == item.Ccosto).
                        Select(x => new ReporteGananciasPerdidasporFuncionCcosto
                        {
                            pstrCuenta = x.pstrCuenta,
                            pstrDenominacion = x.pstrDenominacion,
                            mes = x.mes,
                            imputable = x.imputable,
                            DebeSoles = x.DebeSoles,
                            DebeDolares = x.DebeDolares,
                            HaberSoles = x.HaberSoles,
                            HaberDolares = x.HaberDolares,
                            Ccosto = x.Ccosto,
                            Naturaleza = x.Naturaleza,
                            IdCcosto = x.IdCcosto,
                        }).ToList();

                    foreach (var cuentaImputable in ListaImputables)
                    {

                        if (cuentaImputable.pstrCuenta.StartsWith("69"))
                        {
                            string h = "";
                        }
                        pIntDigitos = 2;
                        var formarImputables = CuentasMesRequeridoGananciaFuncion.Where(x => x.pstrCuenta.StartsWith(item.Cuenta) && x.Ccosto == item.Ccosto).ToList();

                        foreach (var imp in formarImputables)
                        {
                            pIntDigitos = 2;
                            while (pIntDigitos <= pIntNumeroDigitos)
                            {

                                objReporte = new ReporteGananciasPerdidasporFuncionCcosto();
                                objReporte.pstrCuenta = imp.pstrCuenta.Length >= pIntDigitos ? imp.pstrCuenta.Substring(0, pIntDigitos) : "*No Existe*";
                                objReporte.IdCcosto = item.Ccosto;
                                objReporte.ValueCcosto = item.IdCosto;
                                if (objReporte.pstrCuenta != "*No Existe*" && ListaFinal.Where(x => x.pstrCuenta == objReporte.pstrCuenta && x.IdCcosto == objReporte.IdCcosto).Count() == 0)
                                {
                                    var CuentaImputable = NombreCuentaImputable(objReporte.pstrCuenta);
                                    string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
                                    int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
                                    objReporte.pstrDenominacion = objReporte.pstrCuenta != "No Existe*" ? denominacion : "*No Existe*";
                                    objReporte.Ccosto = item.Ccosto;
                                    objReporte.MovimientoDebe = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, pstrMes, true, pImputables);
                                    objReporte.MovimientoHaber = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, pstrMes, true, pImputables);
                                    objReporte.perdida = objReporte.MovimientoDebe > objReporte.MovimientoHaber ? objReporte.MovimientoDebe - objReporte.MovimientoHaber : 0;
                                    objReporte.gananacia = objReporte.MovimientoDebe < objReporte.MovimientoHaber ? (objReporte.MovimientoDebe - objReporte.MovimientoHaber) * -1 : 0;
                                    objReporte.imputable = imputable;


                                    if (UsadoDesdeEstadoResultado)
                                    {

                                        int iMesDesde = int.Parse(MesDesde);
                                        decimal PerdidaMes2 = 0, GananciaMes2 = 0, PerdidaMes3 = 0, GananciaMes3 = 0, PerdidaMes4 = 0, GananciaMes4 = 0, PerdidaMes5 = 0, GananciaMes5 = 0, PerdidaMes6 = 0, GananciaMes6 = 0;
                                        decimal PerdidaMes7 = 0, GananciaMes7 = 0, PerdidaMes8 = 0, GananciaMes8 = 0, PerdidaMes9 = 0, GananciaMes9 = 0, PerdidaMes10 = 0, GananciaMes10 = 0, PerdidaMes11 = 0, GananciaMes11 = 0, PerdidaMes12 = 0, GananciaMes12 = 0;
                                        decimal PerdidaMes1 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                        decimal GananciaMes1 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                        string NombreMes1 = NombreMeses(iMesDesde);
                                        string NombreMes2 = "", NombreMes3 = "", NombreMes4 = "", NombreMes5 = "", NombreMes6 = "", NombreMes7 = "", NombreMes8 = "", NombreMes9 = "", NombreMes10 = "", NombreMes11 = "", NombreMes12 = "";

                                        iMesDesde++;
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes2 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes2 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes2 = NombreMeses(iMesDesde);
                                        }
                                        iMesDesde++;
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes3 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes3 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes3 = NombreMeses(iMesDesde);
                                        }

                                        iMesDesde++;
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes4 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes4 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes4 = NombreMeses(iMesDesde);
                                        }

                                        iMesDesde++;


                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes5 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes5 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes5 = NombreMeses(iMesDesde);
                                        }

                                        iMesDesde++;
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes6 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes6 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes6 = NombreMeses(iMesDesde);
                                        }
                                        iMesDesde++;
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes7 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes7 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes7 = NombreMeses(iMesDesde);
                                        }
                                        iMesDesde++;
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes8 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes8 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes8 = NombreMeses(iMesDesde);
                                        }
                                        iMesDesde++;
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes9 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes9 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes9 = NombreMeses(iMesDesde);
                                        }
                                        iMesDesde++;
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes10 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes10 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes10 = NombreMeses(iMesDesde);
                                            iMesDesde++;
                                        }
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes11 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes11 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes11 = NombreMeses(iMesDesde);
                                        }
                                        iMesDesde++;


                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes12 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes12 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes12 = NombreMeses(iMesDesde);
                                        }


                                        if (objReporte.pstrCuenta.StartsWith("6") || objReporte.pstrCuenta.StartsWith("9"))
                                        {
                                            objReporte.Acumulado = UsadoDesdeEstadoResultado ? objReporte.gananacia - objReporte.perdida : objReporte.perdida - objReporte.gananacia;
                                            //objReporte.Mensual = UsadoDesdeEstadoResultado ? Ganancia - Perdida : Perdida - Ganancia;
                                            objReporte.Mes1 = UsadoDesdeEstadoResultado ? GananciaMes1 - PerdidaMes1 : PerdidaMes1 - GananciaMes1;
                                            objReporte.Mes2 = UsadoDesdeEstadoResultado ? GananciaMes2 - PerdidaMes2 : PerdidaMes2 - GananciaMes2;
                                            objReporte.Mes3 = UsadoDesdeEstadoResultado ? GananciaMes3 - PerdidaMes3 : PerdidaMes3 - GananciaMes3;
                                            objReporte.Mes4 = UsadoDesdeEstadoResultado ? GananciaMes4 - PerdidaMes4 : PerdidaMes4 - GananciaMes4;
                                            objReporte.Mes5 = UsadoDesdeEstadoResultado ? GananciaMes5 - PerdidaMes5 : PerdidaMes5 - GananciaMes5;
                                            objReporte.Mes6 = UsadoDesdeEstadoResultado ? GananciaMes6 - PerdidaMes6 : PerdidaMes6 - GananciaMes6;
                                            objReporte.Mes7 = UsadoDesdeEstadoResultado ? GananciaMes7 - PerdidaMes7 : PerdidaMes7 - GananciaMes7;
                                            objReporte.Mes8 = UsadoDesdeEstadoResultado ? GananciaMes8 - PerdidaMes8 : PerdidaMes8 - GananciaMes8;
                                            objReporte.Mes9 = UsadoDesdeEstadoResultado ? GananciaMes9 - PerdidaMes9 : PerdidaMes9 - GananciaMes9;
                                            objReporte.Mes10 = UsadoDesdeEstadoResultado ? GananciaMes10 - PerdidaMes10 : PerdidaMes10 - GananciaMes10;

                                            objReporte.Mes11 = UsadoDesdeEstadoResultado ? GananciaMes11 - PerdidaMes11 : PerdidaMes11 - GananciaMes11;
                                            objReporte.Mes12 = UsadoDesdeEstadoResultado ? GananciaMes12 - PerdidaMes12 : PerdidaMes12 - GananciaMes12;

                                            objReporte.NombreMes1 = NombreMes1;
                                            objReporte.NombreMes2 = NombreMes2;
                                            objReporte.NombreMes3 = NombreMes3;
                                            objReporte.NombreMes4 = NombreMes4;
                                            objReporte.NombreMes5 = NombreMes5;
                                            objReporte.NombreMes6 = NombreMes6;
                                            objReporte.NombreMes7 = NombreMes7;
                                            objReporte.NombreMes8 = NombreMes8;
                                            objReporte.NombreMes9 = NombreMes9;
                                            objReporte.NombreMes10 = NombreMes10;
                                            objReporte.NombreMes11 = NombreMes11;
                                            objReporte.NombreMes12 = NombreMes12;
                                        }
                                        else if (objReporte.pstrCuenta.StartsWith("7"))
                                        {
                                            objReporte.Acumulado = objReporte.gananacia - objReporte.perdida;
                                            // objReporte.Mensual = Ganancia - Perdida;
                                            objReporte.Mes1 = GananciaMes1 - PerdidaMes1;
                                            objReporte.Mes2 = GananciaMes2 - PerdidaMes2;
                                            objReporte.Mes3 = GananciaMes3 - PerdidaMes3;
                                            objReporte.Mes4 = GananciaMes4 - PerdidaMes4;
                                            objReporte.Mes5 = GananciaMes5 - PerdidaMes5;
                                            objReporte.Mes6 = GananciaMes6 - PerdidaMes6;
                                            objReporte.Mes7 = GananciaMes7 - PerdidaMes7;
                                            objReporte.Mes8 = GananciaMes8 - PerdidaMes8;
                                            objReporte.Mes9 = GananciaMes9 - PerdidaMes9;
                                            objReporte.Mes10 = GananciaMes10 - PerdidaMes10;
                                            objReporte.Mes11 = GananciaMes11 - PerdidaMes11;
                                            objReporte.Mes12 = GananciaMes12 - PerdidaMes12;


                                            objReporte.NombreMes1 = NombreMes1;
                                            objReporte.NombreMes2 = NombreMes2;
                                            objReporte.NombreMes3 = NombreMes3;
                                            objReporte.NombreMes4 = NombreMes4;
                                            objReporte.NombreMes5 = NombreMes5;
                                            objReporte.NombreMes6 = NombreMes6;
                                            objReporte.NombreMes7 = NombreMes7;
                                            objReporte.NombreMes8 = NombreMes8;
                                            objReporte.NombreMes9 = NombreMes9;
                                            objReporte.NombreMes10 = NombreMes10;
                                            objReporte.NombreMes11 = NombreMes11;
                                            objReporte.NombreMes12 = NombreMes12;
                                        }

                                        objReporte.Ccosto = "CENTRO DE COSTO :" + item.Ccosto;
                                        objReporte.Grupo = objReporte.pstrCuenta.StartsWith("7") ? "INGRESOS" : objReporte.pstrCuenta.StartsWith("6") ? "COSTO" : objReporte.pstrCuenta.StartsWith("9") ? "GASTO" : "";
                                        objReporte.iGrupo = objReporte.pstrCuenta.StartsWith("7") ? 1 : objReporte.pstrCuenta.StartsWith("6") ? 2 : objReporte.pstrCuenta.StartsWith("9") ? 3 : 0;
                                        objReporte.IdCcosto = item.Ccosto;
                                        objReporte.imputable = imputable;
                                        ListaFinal.Add(objReporte);


                                    }
                                    else
                                    {


                                        decimal MovimientoDebe = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, pstrMes, false, pImputables);
                                        decimal MovimientoHaber = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, pstrMes, false, pImputables);
                                        decimal Perdida = MovimientoDebe > MovimientoHaber ? MovimientoDebe - MovimientoHaber : 0;
                                        decimal Ganancia = MovimientoDebe < MovimientoHaber ? (MovimientoDebe - MovimientoHaber) * -1 : 0;
                                        objReporte.Ccosto = "CENTRO DE COSTO :" + item.Ccosto;
                                        objReporte.Grupo = objReporte.pstrCuenta.StartsWith("7") ? "INGRESOS" : objReporte.pstrCuenta.StartsWith("6") ? "COSTO" : objReporte.pstrCuenta.StartsWith("9") ? "GASTO" : "";

                                        objReporte.iGrupo = objReporte.pstrCuenta.StartsWith("7") ? 1 : objReporte.pstrCuenta.StartsWith("6") ? 2 : objReporte.pstrCuenta.StartsWith("9") ? 3 : 0;
                                        if (objReporte.pstrCuenta.StartsWith("6") || objReporte.pstrCuenta.StartsWith("9"))
                                        {
                                            objReporte.Acumulado = UsadoDesdeEstadoResultado ? objReporte.gananacia - objReporte.perdida : objReporte.perdida - objReporte.gananacia;
                                            objReporte.Mensual = UsadoDesdeEstadoResultado ? Ganancia - Perdida : Perdida - Ganancia;

                                        }
                                        else if (objReporte.pstrCuenta.StartsWith("7"))
                                        {
                                            objReporte.Acumulado = objReporte.gananacia - objReporte.perdida;
                                            objReporte.Mensual = Ganancia - Perdida;
                                        }

                                        objReporte.IdCcosto = item.Ccosto;
                                        objReporte.imputable = imputable;
                                        ListaFinal.Add(objReporte);
                                    }
                                }
                                pIntDigitos = pIntDigitos + 1;
                            }
                        }
                    }

                    int Contador = 1;
                    pIntDigitos = 2;
                    ListaCuentasImputables = ListaCuentasImputables.Where(o => o.CuentaMayor == item.Cuenta && o.Ccosto == item.Ccosto).ToList();
                    if (Contador < ListaCuentasImputables.Count())
                    {
                        foreach (var subCuenta in ListaCuentasImputables.AsParallel())
                        {
                            while (pIntDigitos <= pIntNumeroDigitos)
                            {
                                objReporte = new ReporteGananciasPerdidasporFuncionCcosto();
                                objReporte.pstrCuenta = CuentasMesRequeridoGananciaFuncion.Where(x => x.pstrCuenta.StartsWith(subCuenta.Cuenta) && x.Ccosto == item.Ccosto).Count() != 0 ? CuentasMesRequeridoGananciaFuncion.Where(x => x.pstrCuenta.StartsWith(subCuenta.Cuenta) && x.Ccosto == item.Ccosto).FirstOrDefault().pstrCuenta.Length >= pIntDigitos ? CuentasMesRequeridoGananciaFuncion.Where(x => x.pstrCuenta.StartsWith(subCuenta.Cuenta) && x.Ccosto == item.Ccosto).FirstOrDefault().pstrCuenta.Substring(0, pIntDigitos) : "*No Existe*" : "*No Existe*";
                                objReporte.IdCcosto = item.Ccosto;
                                objReporte.ValueCcosto = item.IdCosto;
                                if (objReporte.pstrCuenta != "*No Existe*" && ListaFinal.Where(x => x.pstrCuenta == objReporte.pstrCuenta && x.IdCcosto == objReporte.IdCcosto).Count() == 0)
                                {
                                    var CuentaImputable = NombreCuentaImputable(objReporte.pstrCuenta);
                                    string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
                                    int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
                                    objReporte.pstrDenominacion = objReporte.pstrCuenta != "No Existe*" ? denominacion : "*No Existe*";
                                    objReporte.MovimientoDebe = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, pstrMes, true, pImputables);
                                    objReporte.MovimientoHaber = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, pstrMes, true, pImputables);
                                    objReporte.perdida = objReporte.MovimientoDebe > objReporte.MovimientoHaber ? objReporte.MovimientoDebe - objReporte.MovimientoHaber : 0;
                                    objReporte.gananacia = objReporte.MovimientoDebe < objReporte.MovimientoHaber ? (objReporte.MovimientoDebe - objReporte.MovimientoHaber) * -1 : 0;
                                    objReporte.imputable = imputable;



                                    if (UsadoDesdeEstadoResultado)
                                    {
                                        //int iMesDesde = int.Parse(MesDesde);
                                        //decimal PerdidaMes2 = 0, GananciaMes2 = 0;
                                        //decimal PerdidaMes1 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                        //decimal GananciaMes1 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;

                                        //string NombreMes1 = NombreMeses(iMesDesde);
                                        //string NombreMes2 = "";
                                        //iMesDesde++;




                                        //if (iMesDesde <= int.Parse(MesHasta))
                                        //{
                                        //    PerdidaMes2 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                        //    GananciaMes2 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                        //    NombreMes2 = NombreMeses(iMesDesde);
                                        //}
                                        //iMesDesde++;
                                        //if (objReporte.pstrCuenta.StartsWith("6") || objReporte.pstrCuenta.StartsWith("9"))
                                        //{
                                        //    objReporte.Acumulado = UsadoDesdeEstadoResultado ? objReporte.gananacia - objReporte.perdida : objReporte.perdida - objReporte.gananacia;
                                        //    //objReporte.Mensual = UsadoDesdeEstadoResultado ? Ganancia - Perdida : Perdida - Ganancia;
                                        //    objReporte.Mes1 = UsadoDesdeEstadoResultado ? GananciaMes1 - PerdidaMes1 : PerdidaMes1 - GananciaMes1;
                                        //    objReporte.Mes2 = UsadoDesdeEstadoResultado ? GananciaMes2 - PerdidaMes2 : PerdidaMes2 - GananciaMes2;
                                        //    objReporte.NombreMes1 = NombreMes1;

                                        //    objReporte.NombreMes2 = NombreMes2;

                                        //}
                                        //else if (objReporte.pstrCuenta.StartsWith("7"))
                                        //{
                                        //    objReporte.Acumulado = objReporte.gananacia - objReporte.perdida;
                                        //    // objReporte.Mensual = Ganancia - Perdida;
                                        //    objReporte.Mes1 = GananciaMes1 - PerdidaMes1;
                                        //    objReporte.Mes2 = GananciaMes2 - PerdidaMes2;

                                        //    objReporte.NombreMes1 = NombreMes1;
                                        //    objReporte.NombreMes2 = NombreMes2;
                                        //}

                                        //objReporte.Ccosto = "CENTRO DE COSTO :" + item.Ccosto;
                                        //objReporte.Grupo = objReporte.pstrCuenta.StartsWith("7") ? "INGRESOS" : objReporte.pstrCuenta.StartsWith("6") ? "COSTO" : objReporte.pstrCuenta.StartsWith("9") ? "GASTO" : "";
                                        //objReporte.iGrupo = objReporte.pstrCuenta.StartsWith("7") ? 1 : objReporte.pstrCuenta.StartsWith("6") ? 2 : objReporte.pstrCuenta.StartsWith("9") ? 3 : 0;
                                        //objReporte.IdCcosto = item.Ccosto;
                                        //objReporte.imputable = imputable;
                                        //ListaFinal.Add(objReporte);


                                        int iMesDesde = int.Parse(MesDesde);
                                        decimal PerdidaMes2 = 0, GananciaMes2 = 0, PerdidaMes3 = 0, GananciaMes3 = 0, PerdidaMes4 = 0, GananciaMes4 = 0, PerdidaMes5 = 0, GananciaMes5 = 0, PerdidaMes6 = 0, GananciaMes6 = 0;
                                        decimal PerdidaMes7 = 0, GananciaMes7 = 0, PerdidaMes8 = 0, GananciaMes8 = 0, PerdidaMes9 = 0, GananciaMes9 = 0, PerdidaMes10 = 0, GananciaMes10 = 0, PerdidaMes11 = 0, GananciaMes11 = 0, PerdidaMes12 = 0, GananciaMes12 = 0;
                                        decimal PerdidaMes1 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                        decimal GananciaMes1 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                        string NombreMes1 = NombreMeses(iMesDesde);
                                        string NombreMes2 = "", NombreMes3 = "", NombreMes4 = "", NombreMes5 = "", NombreMes6 = "", NombreMes7 = "", NombreMes8 = "", NombreMes9 = "", NombreMes10 = "", NombreMes11 = "", NombreMes12 = "";

                                        iMesDesde++;
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes2 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes2 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes2 = NombreMeses(iMesDesde);
                                        }
                                        iMesDesde++;
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes3 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes3 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes3 = NombreMeses(iMesDesde);
                                        }

                                        iMesDesde++;
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes4 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes4 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes4 = NombreMeses(iMesDesde);
                                        }

                                        iMesDesde++;


                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes5 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes5 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes5 = NombreMeses(iMesDesde);
                                        }

                                        iMesDesde++;
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes6 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes6 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes6 = NombreMeses(iMesDesde);
                                        }
                                        iMesDesde++;
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes7 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes7 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes7 = NombreMeses(iMesDesde);
                                        }
                                        iMesDesde++;
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes8 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes8 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes8 = NombreMeses(iMesDesde);
                                        }
                                        iMesDesde++;
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes9 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes9 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes9 = NombreMeses(iMesDesde);
                                        }
                                        iMesDesde++;
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes10 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes10 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes10 = NombreMeses(iMesDesde);
                                            iMesDesde++;
                                        }
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes11 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes11 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes11 = NombreMeses(iMesDesde);
                                        }
                                        iMesDesde++;


                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes12 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes12 = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes12 = NombreMeses(iMesDesde);
                                        }


                                        if (objReporte.pstrCuenta.StartsWith("6") || objReporte.pstrCuenta.StartsWith("9"))
                                        {
                                            objReporte.Acumulado = UsadoDesdeEstadoResultado ? objReporte.gananacia - objReporte.perdida : objReporte.perdida - objReporte.gananacia;
                                            //objReporte.Mensual = UsadoDesdeEstadoResultado ? Ganancia - Perdida : Perdida - Ganancia;
                                            objReporte.Mes1 = UsadoDesdeEstadoResultado ? GananciaMes1 - PerdidaMes1 : PerdidaMes1 - GananciaMes1;
                                            objReporte.Mes2 = UsadoDesdeEstadoResultado ? GananciaMes2 - PerdidaMes2 : PerdidaMes2 - GananciaMes2;
                                            objReporte.Mes3 = UsadoDesdeEstadoResultado ? GananciaMes3 - PerdidaMes3 : PerdidaMes3 - GananciaMes3;
                                            objReporte.Mes4 = UsadoDesdeEstadoResultado ? GananciaMes4 - PerdidaMes4 : PerdidaMes4 - GananciaMes4;
                                            objReporte.Mes5 = UsadoDesdeEstadoResultado ? GananciaMes5 - PerdidaMes5 : PerdidaMes5 - GananciaMes5;
                                            objReporte.Mes6 = UsadoDesdeEstadoResultado ? GananciaMes6 - PerdidaMes6 : PerdidaMes6 - GananciaMes6;
                                            objReporte.Mes7 = UsadoDesdeEstadoResultado ? GananciaMes7 - PerdidaMes7 : PerdidaMes7 - GananciaMes7;
                                            objReporte.Mes8 = UsadoDesdeEstadoResultado ? GananciaMes8 - PerdidaMes8 : PerdidaMes8 - GananciaMes8;
                                            objReporte.Mes9 = UsadoDesdeEstadoResultado ? GananciaMes9 - PerdidaMes9 : PerdidaMes9 - GananciaMes9;
                                            objReporte.Mes10 = UsadoDesdeEstadoResultado ? GananciaMes10 - PerdidaMes10 : PerdidaMes10 - GananciaMes10;

                                            objReporte.Mes11 = UsadoDesdeEstadoResultado ? GananciaMes11 - PerdidaMes11 : PerdidaMes11 - GananciaMes11;
                                            objReporte.Mes12 = UsadoDesdeEstadoResultado ? GananciaMes12 - PerdidaMes12 : PerdidaMes12 - GananciaMes12;

                                            objReporte.NombreMes1 = NombreMes1;
                                            objReporte.NombreMes2 = NombreMes2;
                                            objReporte.NombreMes3 = NombreMes3;
                                            objReporte.NombreMes4 = NombreMes4;
                                            objReporte.NombreMes5 = NombreMes5;
                                            objReporte.NombreMes6 = NombreMes6;
                                            objReporte.NombreMes7 = NombreMes7;
                                            objReporte.NombreMes8 = NombreMes8;
                                            objReporte.NombreMes9 = NombreMes9;
                                            objReporte.NombreMes10 = NombreMes10;
                                            objReporte.NombreMes11 = NombreMes11;
                                            objReporte.NombreMes12 = NombreMes12;
                                        }
                                        else if (objReporte.pstrCuenta.StartsWith("7"))
                                        {
                                            objReporte.Acumulado = objReporte.gananacia - objReporte.perdida;
                                            // objReporte.Mensual = Ganancia - Perdida;
                                            objReporte.Mes1 = GananciaMes1 - PerdidaMes1;
                                            objReporte.Mes2 = GananciaMes2 - PerdidaMes2;
                                            objReporte.Mes3 = GananciaMes3 - PerdidaMes3;
                                            objReporte.Mes4 = GananciaMes4 - PerdidaMes4;
                                            objReporte.Mes5 = GananciaMes5 - PerdidaMes5;
                                            objReporte.Mes6 = GananciaMes6 - PerdidaMes6;
                                            objReporte.Mes7 = GananciaMes7 - PerdidaMes7;
                                            objReporte.Mes8 = GananciaMes8 - PerdidaMes8;
                                            objReporte.Mes9 = GananciaMes9 - PerdidaMes9;
                                            objReporte.Mes10 = GananciaMes10 - PerdidaMes10;
                                            objReporte.Mes11 = GananciaMes11 - PerdidaMes11;
                                            objReporte.Mes12 = GananciaMes12 - PerdidaMes12;


                                            objReporte.NombreMes1 = NombreMes1;
                                            objReporte.NombreMes2 = NombreMes2;
                                            objReporte.NombreMes3 = NombreMes3;
                                            objReporte.NombreMes4 = NombreMes4;
                                            objReporte.NombreMes5 = NombreMes5;
                                            objReporte.NombreMes6 = NombreMes6;
                                            objReporte.NombreMes7 = NombreMes7;
                                            objReporte.NombreMes8 = NombreMes8;
                                            objReporte.NombreMes9 = NombreMes9;
                                            objReporte.NombreMes10 = NombreMes10;
                                            objReporte.NombreMes11 = NombreMes11;
                                            objReporte.NombreMes12 = NombreMes12;
                                        }

                                        objReporte.Ccosto = "CENTRO DE COSTO :" + item.Ccosto;
                                        objReporte.Grupo = objReporte.pstrCuenta.StartsWith("7") ? "INGRESOS" : objReporte.pstrCuenta.StartsWith("6") ? "COSTO" : objReporte.pstrCuenta.StartsWith("9") ? "GASTO" : "";
                                        objReporte.iGrupo = objReporte.pstrCuenta.StartsWith("7") ? 1 : objReporte.pstrCuenta.StartsWith("6") ? 2 : objReporte.pstrCuenta.StartsWith("9") ? 3 : 0;
                                        objReporte.IdCcosto = item.Ccosto;
                                        objReporte.imputable = imputable;
                                        ListaFinal.Add(objReporte);


                                    }
                                    else
                                    {
                                        decimal MovimientoDebe = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, pstrMes, false, pImputables);
                                        decimal MovimientoHaber = SumaImporteMovimientoPorFuncionCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, pstrMes, false, pImputables);

                                        decimal Perdida = MovimientoDebe > MovimientoHaber ? MovimientoDebe - MovimientoHaber : 0;
                                        decimal Ganancia = MovimientoDebe < MovimientoHaber ? (MovimientoDebe - MovimientoHaber) * -1 : 0;

                                        objReporte.Ccosto = "CENTRO DE COSTO :" + item.Ccosto;
                                        objReporte.Grupo = objReporte.pstrCuenta.StartsWith("7") ? "INGRESOS" : objReporte.pstrCuenta.StartsWith("6") ? "COSTO" : objReporte.pstrCuenta.StartsWith("9") ? "GASTO" : "";

                                        objReporte.iGrupo = objReporte.pstrCuenta.StartsWith("7") ? 1 : objReporte.pstrCuenta.StartsWith("6") ? 2 : objReporte.pstrCuenta.StartsWith("9") ? 3 : 0;
                                        if (objReporte.pstrCuenta.StartsWith("6") || objReporte.pstrCuenta.StartsWith("9"))
                                        {
                                            objReporte.Acumulado = UsadoDesdeEstadoResultado ? objReporte.gananacia - objReporte.perdida : objReporte.perdida - objReporte.gananacia;
                                            objReporte.Mensual = UsadoDesdeEstadoResultado ? Ganancia - Perdida : Perdida - Ganancia;
                                        }
                                        else if (objReporte.pstrCuenta.StartsWith("7"))
                                        {
                                            objReporte.Acumulado = objReporte.gananacia - objReporte.perdida;
                                            objReporte.Mensual = Ganancia - Perdida;

                                        }
                                        objReporte.imputable = imputable;
                                        ListaFinal.Add(objReporte);
                                    }

                                }
                                pIntDigitos = pIntDigitos + 1;
                            }
                            pIntDigitos = 2;
                        }
                    }
                }
                if (pImputables == 1)
                {
                    ListaFinal = ListaFinal.ToList().Where(y => y.imputable == 1 || (y.imputable == 0 && y.pstrCuenta.Length == 2)).OrderBy(x => x.pstrCuenta).ToList().Where(x => x.MovimientoDebe != 0 || x.MovimientoHaber != 0 || x.perdida != 0 || x.gananacia != 0).ToList();
                    if (Detallado)
                    {

                        var fechaInicio = new DateTime(int.Parse(pstrPeriodo), 1, 1);
                        var fechaFin = DateTime.Parse(new DateTime(int.Parse(pstrPeriodo), int.Parse(pstrMes), DateTime.DaysInMonth(int.Parse(pstrPeriodo), int.Parse(pstrMes))).ToShortDateString() + " 23:59");
                        List<ReporteAnalisisCuentasCteAnalitico> analisisCuentas = new List<ReporteAnalisisCuentasCteAnalitico>();
                        analisisCuentas = new AsientosContablesBL().ReporteAnalisisCuentasCteAnalitico(ref objOperationResult,
                       fechaInicio, fechaFin, null, null, false, null,
                       TipoReporteAnalisis.AnalisisCuentaCorrienteAnalitico, 0, null, null, false);
                        if (objOperationResult.Success == 0) return null;
                        List<ReporteGananciasPerdidasporFuncionCcosto> ListaDetallada = new List<ReporteGananciasPerdidasporFuncionCcosto>();
                        foreach (var item in ListaFinal)
                        {

                            var detalles = analisisCuentas.Where(o => o.CuentaImputable.StartsWith(item.pstrCuenta) && o.IdCentroCostos.Trim() == item.ValueCcosto).ToList();

                            foreach (var det in detalles)
                            {
                                ReporteGananciasPerdidasporFuncionCcosto objDetallado = new ReporteGananciasPerdidasporFuncionCcosto();
                                objDetallado = (ReporteGananciasPerdidasporFuncionCcosto)item.Clone();

                                objDetallado.GrupoCuenta = item.pstrCuenta;
                                objDetallado.NroDiario = det.DocumentoProvicion;
                                objDetallado.Analisis = det.Analisis;
                                objDetallado.MensualCuenta = int.Parse(det.Mes).ToString("00") == pstrMes ? pIntMoneda == 1 ? det.NaturalezaRegistro == "D" ? det.ImporteSolesD.Value : det.ImporteSolesH.Value : pIntMoneda == 2 ? det.ImporteDolaresD.Value : det.ImporteDolaresH.Value : 0;
                                objDetallado.AcumuladoCuenta = pIntMoneda == 1 ? det.NaturalezaRegistro == "D" ? det.ImporteSolesD.Value : det.ImporteSolesH.Value : pIntMoneda == 2 ? det.ImporteDolaresD.Value : det.ImporteDolaresH.Value;
                                ListaDetallada.Add(objDetallado);
                            }
                            if (!detalles.Any())
                            {
                                item.GrupoCuenta = item.pstrCuenta;
                                ListaDetallada.Add(item);

                            }

                        }
                        return ListaDetallada.OrderBy(o => o.Ccosto).ThenBy(o => o.iGrupo).ToList();
                    }
                    else
                    {
                        return ListaFinal.OrderBy(o => o.Ccosto).ThenBy(o => o.iGrupo).ToList();
                    }


                }
                else
                {
                    ListaFinal = ListaFinal.OrderBy(x => x.pstrCuenta).ToList().Where(x => x.MovimientoDebe != 0 || x.MovimientoHaber != 0 || x.perdida != 0 || x.gananacia != 0).ToList();

                    if (Detallado)
                    {

                        var fechaInicio = new DateTime(int.Parse(pstrPeriodo), 1, 1);
                        var fechaFin = DateTime.Parse(new DateTime(int.Parse(pstrPeriodo), int.Parse(pstrMes), DateTime.DaysInMonth(int.Parse(pstrPeriodo), int.Parse(pstrMes))).ToShortDateString() + " 23:59");
                        List<ReporteAnalisisCuentasCteAnalitico> analisisCuentas = new List<ReporteAnalisisCuentasCteAnalitico>();
                        analisisCuentas = new AsientosContablesBL().ReporteAnalisisCuentasCteAnalitico(ref objOperationResult,
                       fechaInicio, fechaFin, null, null, false, null,
                       TipoReporteAnalisis.AnalisisCuentaCorrienteAnalitico, 0, null, null, false);
                        if (objOperationResult.Success == 0) return null;
                        List<ReporteGananciasPerdidasporFuncionCcosto> ListaDetallada = new List<ReporteGananciasPerdidasporFuncionCcosto>();
                        int Cont = 1;
                        foreach (var item in ListaFinal)
                        {

                            if (Cont == 13)
                            {
                                string h = "";
                            }
                            var detalles = analisisCuentas.Where(o => o.CuentaImputable.StartsWith(item.pstrCuenta) && o.IdCentroCostos.Trim() == item.ValueCcosto).ToList();

                            foreach (var det in detalles)
                            {
                                ReporteGananciasPerdidasporFuncionCcosto objDetallado = new ReporteGananciasPerdidasporFuncionCcosto();
                                objDetallado = (ReporteGananciasPerdidasporFuncionCcosto)item.Clone();

                                objDetallado.GrupoCuenta = item.pstrCuenta;
                                objDetallado.NroDiario = det.DocumentoProvicion;
                                objDetallado.Analisis = det.Analisis;
                                objDetallado.MensualCuenta = int.Parse(det.Mes).ToString("00") == pstrMes ? pIntMoneda == 1 ? det.NaturalezaRegistro == "D" ? det.ImporteSolesD.Value : det.ImporteSolesH.Value : pIntMoneda == 2 ? det.ImporteDolaresD.Value : det.ImporteDolaresH.Value : 0;
                                objDetallado.AcumuladoCuenta = pIntMoneda == 1 ? det.NaturalezaRegistro == "D" ? det.ImporteSolesD.Value : det.ImporteSolesH.Value : pIntMoneda == 2 ? det.ImporteDolaresD.Value : det.ImporteDolaresH.Value;
                                ListaDetallada.Add(objDetallado);
                            }

                            Cont = Cont + 1;
                            if (!detalles.Any())
                            {
                                item.GrupoCuenta = item.pstrCuenta;
                                ListaDetallada.Add(item);

                            }

                        }
                        return ListaDetallada.OrderBy(o => o.Ccosto).ThenBy(o => o.iGrupo).ToList();

                    }
                    else
                    {
                        return ListaFinal.OrderBy(o => o.Ccosto).ThenBy(o => o.iGrupo).ToList();
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;

                objOperationResult.AdditionalInformation = "ContabilidadBL.ReporteGananciasPerdidasporFuncionCcosto()";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }


        }
        private string NombreMeses(int mes)
        {
            switch (mes)
            {
                case 1: return "ENERO";
                case 2: return "FEBRERO";
                case 3: return "MARZO";
                case 4: return "ABRIL";
                case 5: return "MAYO";
                case 6: return "JUNIO";
                case 7: return "JULIO";
                case 8: return "AGOSTO";
                case 9: return "SETIEMBRE";
                case 10: return "OCTUBRE";
                case 11: return "NOVIEMBRE";
                case 12: return "DICIEMBRE";


            }
            return string.Empty;
        }



        public List<ReporteGananciasPerdidasporNaturalezaCcosto> ReporteGananciasPerdidasporNaturalezaCcosto(ref OperationResult objOperationResult, string pstrPeriodo, string pstrMes, int pIntNumeroDigitos, int pIntMoneda, int pImputables, string Ccosto, bool Detallado, bool UsadoDesdeEstadoNaturaleza = false, List<asientocontableDto> ListaAsociadaBalanceNaturaleza = null, string MesDesde = "01", string MesHasta = "12")
        {

            try
            {
                objOperationResult.Success = 1;
                List<ReporteGananciasPerdidasporNaturalezaCcosto> ListaFinal = new List<ReporteGananciasPerdidasporNaturalezaCcosto>();
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                List<CuentaMayorCcosto> CuentasMayores = new List<CuentaMayorCcosto>();
                List<CuentaMayorCcosto> ListaCuentasImputables = new List<CuentaMayorCcosto>();
                ReporteGananciasPerdidasporNaturalezaCcosto objReporte = new ReporteGananciasPerdidasporNaturalezaCcosto();
                AsientosContablesBL _objAsientoContableBL = new AsientosContablesBL();
                List<saldoscontablesDto> Saldoscontables = new List<saldoscontablesDto>();
                #region RecopilaCuentasExistentesMesRequerido

                if (UsadoDesdeEstadoNaturaleza)
                {
                    Saldoscontables = _objAsientoContableBL.ObtenerSaldosContablesPorMes(ref  objOperationResult, int.Parse(pstrMes), int.Parse(pstrPeriodo), 1, true, false, null, true);
                    Saldoscontables = Saldoscontables.Where(o => ListaAsociadaBalanceNaturaleza.Select(x => x.v_NroCuenta).Contains(o.v_NroCuenta)).ToList();
                }
                else
                {


                    Saldoscontables = _objAsientoContableBL.ObtenerSaldosContablesPorMes(ref  objOperationResult, int.Parse(pstrMes), int.Parse(pstrPeriodo), 1, true);
                }
                if (objOperationResult.Success == 0) return null;
                List<datahierarchyDto> ListaCentroCostos = dbContext.datahierarchy.Where(o => o.i_GroupId == 31 && o.i_IsDeleted == 0).ToList().ToDTOs();
                if (!string.IsNullOrEmpty(Ccosto))
                {

                    Saldoscontables = Saldoscontables.Where(o => o.IdCentroCosto == Ccosto).ToList();
                }
                var CuentasMesRequerido = (from a in Saldoscontables

                                           join b in dbContext.asientocontable on new { cuenta = a.v_NroCuenta, eliminado = 0, p = periodo }
                                                                                 equals new { cuenta = b.v_NroCuenta, eliminado = b.i_Eliminado.Value, p = b.v_Periodo } into b_join
                                           from b in b_join.DefaultIfEmpty()
                                           where (a.v_NroCuenta != null && a.v_NroCuenta != "" && a.v_NroCuenta != "*Cta Eliminada*" && a != null)
                                           select new ReporteGananciasPerdidasporNaturalezaCcosto
                                           {
                                               pstrCuenta = a == null ? "" : a.v_NroCuenta.Trim(),
                                               pstrDenominacion = b == null ? "*No Existe*" : b.v_NombreCuenta.Trim(),
                                               mes = int.Parse(a.v_Mes).ToString("00"),
                                               imputable = b == null ? -1 : b.i_Imputable.Value,
                                               DebeSoles = a == null ? 0 : a.d_ImporteSolesD.Value,
                                               DebeDolares = a == null ? 0 : a.d_ImporteDolaresD.Value,
                                               HaberSoles = a == null ? 0 : a.d_ImporteSolesH.Value,
                                               HaberDolares = a == null ? 0 : a.d_ImporteDolaresH.Value,
                                               Ccosto = a.IdCentroCosto == null ? "" : string.IsNullOrEmpty(a.IdCentroCosto.Trim()) ? "" : a.IdCentroCosto + "-" + NombreCentroCosto(a.IdCentroCosto, ListaCentroCostos),
                                               Naturaleza = a == null ? "" : a.Naturaleza,
                                               EsCuentaNaturaleza = UsadoDesdeEstadoNaturaleza ? true : CuentaPerdidaNaturaleza(a.v_NroCuenta.Trim()),
                                               IdCcosto = string.IsNullOrEmpty(a.IdCentroCosto) ? "" : a.IdCentroCosto.Trim(),

                                           }).ToList().OrderBy(o => o.pstrCuenta).ToList();

                var CuentasMesRequeridoGananciaNaturaleza = CuentasMesRequerido.Where(o => o.EsCuentaNaturaleza).ToList();
                #endregion

                foreach (var item in CuentasMesRequeridoGananciaNaturaleza.AsParallel())
                {
                    CuentaMayorCcosto ojbCuentaMayr = new CuentaMayorCcosto();
                    string key = string.IsNullOrEmpty(item.Ccosto.Trim()) ? item.pstrCuenta.Substring(0, 2) + "" : item.pstrCuenta.Substring(0, 2) + item.Ccosto;
                    if (!CuentasMayores.Where(o => o.Key == key).Any())
                    {
                        ojbCuentaMayr.Cuenta = item.pstrCuenta.Substring(0, 2);
                        ojbCuentaMayr.Ccosto = item.Ccosto;
                        ojbCuentaMayr.Key = ojbCuentaMayr.Cuenta + ojbCuentaMayr.Ccosto;
                        ojbCuentaMayr.IdCosto = string.IsNullOrEmpty(item.IdCcosto) ? "" : item.IdCcosto.Trim();
                        CuentasMayores.Add(ojbCuentaMayr);
                    }
                }

                foreach (var item in CuentasMayores.AsParallel())
                {

                    int pIntDigitos = 2;
                    List<ReporteGananciasPerdidasporNaturalezaCcosto> ListaImputables = CuentasMesRequeridoGananciaNaturaleza.Where(x => x.pstrCuenta.StartsWith(item.Cuenta.Substring(0, 2)) && x.Ccosto == item.Ccosto).
                        Select(x => new ReporteGananciasPerdidasporNaturalezaCcosto
                        {
                            pstrCuenta = x.pstrCuenta,
                            pstrDenominacion = x.pstrDenominacion,
                            mes = x.mes,
                            imputable = x.imputable,
                            DebeSoles = x.DebeSoles,
                            DebeDolares = x.DebeDolares,
                            HaberSoles = x.HaberSoles,
                            HaberDolares = x.HaberDolares,
                            Ccosto = x.Ccosto,
                            Naturaleza = x.Naturaleza,
                            IdCcosto = x.IdCcosto,


                        }).ToList();

                    foreach (var cuentaImputable in ListaImputables)
                    {
                        pIntDigitos = 2;
                        var formarImputables = CuentasMesRequeridoGananciaNaturaleza.Where(x => x.pstrCuenta.StartsWith(item.Cuenta) && x.Ccosto == item.Ccosto).ToList();

                        foreach (var imp in formarImputables)
                        {
                            pIntDigitos = 2;
                            while (pIntDigitos <= pIntNumeroDigitos)
                            {

                                objReporte = new ReporteGananciasPerdidasporNaturalezaCcosto();
                                objReporte.pstrCuenta = imp.pstrCuenta.Length >= pIntDigitos ? imp.pstrCuenta.Substring(0, pIntDigitos) : "*No Existe*";
                                objReporte.IdCcosto = item.Ccosto;
                                objReporte.ValueCcosto = item.IdCosto;
                                if (objReporte.pstrCuenta != "*No Existe*" && ListaFinal.Where(x => x.pstrCuenta == objReporte.pstrCuenta && x.IdCcosto == objReporte.IdCcosto).Count() == 0)
                                {
                                    var CuentaImputable = NombreCuentaImputable(objReporte.pstrCuenta);
                                    string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
                                    int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
                                    objReporte.pstrDenominacion = objReporte.pstrCuenta != "No Existe*" ? denominacion : "*No Existe*";
                                    objReporte.Ccosto = item.Ccosto;
                                    objReporte.MovimientoDebe = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, pstrMes, true, pImputables);
                                    objReporte.MovimientoHaber = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, pstrMes, true, pImputables);
                                    objReporte.perdida = objReporte.MovimientoDebe > objReporte.MovimientoHaber ? objReporte.MovimientoDebe - objReporte.MovimientoHaber : 0;
                                    objReporte.gananacia = objReporte.MovimientoDebe < objReporte.MovimientoHaber ? (objReporte.MovimientoDebe - objReporte.MovimientoHaber) * -1 : 0;
                                    objReporte.imputable = imputable;


                                    decimal MovimientoDebe = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, pstrMes, false, pImputables);
                                    decimal MovimientoHaber = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, pstrMes, false, pImputables);

                                    decimal Perdida = MovimientoDebe > MovimientoHaber ? MovimientoDebe - MovimientoHaber : 0;
                                    decimal Ganancia = MovimientoDebe < MovimientoHaber ? (MovimientoDebe - MovimientoHaber) * -1 : 0;

                                    objReporte.Grupo = objReporte.pstrCuenta.StartsWith("7") ? "INGRESOS" : objReporte.pstrCuenta.StartsWith("6") ? "COSTO" : objReporte.pstrCuenta.StartsWith("9") ? "GASTO" : "";
                                    objReporte.iGrupo = objReporte.pstrCuenta.StartsWith("7") ? 1 : objReporte.pstrCuenta.StartsWith("6") ? 2 : objReporte.pstrCuenta.StartsWith("9") ? 3 : 0;



                                    if (UsadoDesdeEstadoNaturaleza)
                                    {

                                        int iMesDesde = int.Parse(MesDesde);
                                        decimal PerdidaMes2 = 0, GananciaMes2 = 0, PerdidaMes3 = 0, GananciaMes3 = 0, PerdidaMes4 = 0, GananciaMes4 = 0, PerdidaMes5 = 0, GananciaMes5 = 0, PerdidaMes6 = 0, GananciaMes6 = 0;
                                        decimal PerdidaMes7 = 0, GananciaMes7 = 0, PerdidaMes8 = 0, GananciaMes8 = 0, PerdidaMes9 = 0, GananciaMes9 = 0, PerdidaMes10 = 0, GananciaMes10 = 0, PerdidaMes11 = 0, GananciaMes11 = 0, PerdidaMes12 = 0, GananciaMes12 = 0;
                                        decimal PerdidaMes1 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                        decimal GananciaMes1 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                        string NombreMes1 = NombreMeses(iMesDesde);
                                        string NombreMes2 = "", NombreMes3 = "", NombreMes4 = "", NombreMes5 = "", NombreMes6 = "", NombreMes7 = "", NombreMes8 = "", NombreMes9 = "", NombreMes10 = "", NombreMes11 = "", NombreMes12 = "";



                                        iMesDesde++;
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes2 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes2 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes2 = NombreMeses(iMesDesde);
                                        }
                                        iMesDesde++;
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes3 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes3 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes3 = NombreMeses(iMesDesde);
                                        }

                                        iMesDesde++;
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes4 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes4 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes4 = NombreMeses(iMesDesde);
                                        }

                                        iMesDesde++;


                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes5 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes5 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes5 = NombreMeses(iMesDesde);
                                        }

                                        iMesDesde++;
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes6 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes6 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes6 = NombreMeses(iMesDesde);
                                        }
                                        iMesDesde++;
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes7 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes7 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes7 = NombreMeses(iMesDesde);
                                        }
                                        iMesDesde++;
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes8 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes8 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes8 = NombreMeses(iMesDesde);
                                        }
                                        iMesDesde++;
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes9 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes9 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes9 = NombreMeses(iMesDesde);
                                        }
                                        iMesDesde++;
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes10 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes10 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes10 = NombreMeses(iMesDesde);
                                            iMesDesde++;
                                        }
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes11 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes11 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes11 = NombreMeses(iMesDesde);
                                        }
                                        iMesDesde++;


                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes12 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes12 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes12 = NombreMeses(iMesDesde);
                                        }


                                        if (objReporte.pstrCuenta.StartsWith("6") || objReporte.pstrCuenta.StartsWith("9") || objReporte.pstrCuenta.StartsWith("7"))
                                        {
                                            objReporte.Acumulado = objReporte.gananacia - objReporte.perdida;
                                            objReporte.Mensual = Ganancia - Perdida;
                                            objReporte.Mes1 = GananciaMes1 - PerdidaMes1;
                                            objReporte.Mes2 = GananciaMes2 - PerdidaMes2;
                                            objReporte.Mes3 = GananciaMes3 - PerdidaMes3;
                                            objReporte.Mes4 = GananciaMes4 - PerdidaMes4;
                                            objReporte.Mes5 = GananciaMes5 - PerdidaMes5;
                                            objReporte.Mes6 = GananciaMes6 - PerdidaMes6;
                                            objReporte.Mes7 = GananciaMes7 - PerdidaMes7;
                                            objReporte.Mes8 = GananciaMes8 - PerdidaMes8;
                                            objReporte.Mes9 = GananciaMes9 - PerdidaMes9;
                                            objReporte.Mes10 = GananciaMes10 - PerdidaMes10;
                                            objReporte.Mes11 = GananciaMes11 - PerdidaMes11;
                                            objReporte.Mes12 = GananciaMes12 - PerdidaMes12;
                                            objReporte.NombreMes1 = NombreMes1;
                                            objReporte.NombreMes2 = NombreMes2;
                                            objReporte.NombreMes3 = NombreMes3;
                                            objReporte.NombreMes4 = NombreMes4;
                                            objReporte.NombreMes5 = NombreMes5;
                                            objReporte.NombreMes6 = NombreMes6;
                                            objReporte.NombreMes7 = NombreMes7;
                                            objReporte.NombreMes8 = NombreMes8;
                                            objReporte.NombreMes9 = NombreMes9;
                                            objReporte.NombreMes10 = NombreMes10;
                                            objReporte.NombreMes11 = NombreMes11;
                                            objReporte.NombreMes12 = NombreMes12;

                                        }
                                        //else if (objReporte.pstrCuenta.StartsWith("7"))
                                        //{
                                        //    objReporte.Acumulado = objReporte.gananacia - objReporte.perdida;
                                        //    objReporte.Mensual = Ganancia - Perdida;
                                        //}

                                        objReporte.Ccosto = "CENTRO DE COSTO :" + item.Ccosto;
                                        ListaFinal.Add(objReporte);
                                    }
                                    else
                                    {
                                        if (objReporte.pstrCuenta.StartsWith("6") || objReporte.pstrCuenta.StartsWith("9"))
                                        {
                                            objReporte.Acumulado = objReporte.perdida - objReporte.gananacia;
                                            objReporte.Mensual = Perdida - Ganancia;
                                        }
                                        else if (objReporte.pstrCuenta.StartsWith("7"))
                                        {
                                            objReporte.Acumulado = objReporte.gananacia - objReporte.perdida;
                                            objReporte.Mensual = Ganancia - Perdida;
                                        }

                                        objReporte.Ccosto = "CENTRO DE COSTO :" + item.Ccosto;
                                        ListaFinal.Add(objReporte);
                                    }


                                }
                                pIntDigitos = pIntDigitos + 1;
                            }
                        }
                    }

                    int Contador = 1;
                    pIntDigitos = 2;
                    ListaCuentasImputables = ListaCuentasImputables.Where(o => o.CuentaMayor == item.Cuenta && o.Ccosto == item.Ccosto).ToList();
                    if (Contador < ListaCuentasImputables.Count())
                    {
                        foreach (var subCuenta in ListaCuentasImputables.AsParallel())
                        {
                            while (pIntDigitos <= pIntNumeroDigitos)
                            {
                                objReporte = new ReporteGananciasPerdidasporNaturalezaCcosto();
                                objReporte.pstrCuenta = CuentasMesRequeridoGananciaNaturaleza.Where(x => x.pstrCuenta.StartsWith(subCuenta.Cuenta) && x.Ccosto == item.Ccosto).Count() != 0 ? CuentasMesRequeridoGananciaNaturaleza.Where(x => x.pstrCuenta.StartsWith(subCuenta.Cuenta) && x.Ccosto == item.Ccosto).FirstOrDefault().pstrCuenta.Length >= pIntDigitos ? CuentasMesRequeridoGananciaNaturaleza.Where(x => x.pstrCuenta.StartsWith(subCuenta.Cuenta) && x.Ccosto == item.Ccosto).FirstOrDefault().pstrCuenta.Substring(0, pIntDigitos) : "*No Existe*" : "*No Existe*";
                                objReporte.IdCcosto = item.Ccosto;
                                objReporte.ValueCcosto = item.IdCosto;
                                objReporte.Ccosto = item.Ccosto;
                                if (objReporte.pstrCuenta != "*No Existe*" && ListaFinal.Where(x => x.pstrCuenta == objReporte.pstrCuenta && x.IdCcosto == objReporte.IdCcosto).Count() == 0)
                                {
                                    var CuentaImputable = NombreCuentaImputable(objReporte.pstrCuenta);
                                    string denominacion = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().cuenta.Trim() : "*No Existe*";
                                    int imputable = CuentaImputable.FirstOrDefault() != null ? CuentaImputable.FirstOrDefault().imputable : -1;
                                    objReporte.pstrDenominacion = objReporte.pstrCuenta != "No Existe*" ? denominacion : "*No Existe*";
                                    objReporte.MovimientoDebe = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, pstrMes, true, pImputables);
                                    objReporte.MovimientoHaber = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, pstrMes, true, pImputables);
                                    objReporte.perdida = objReporte.MovimientoDebe > objReporte.MovimientoHaber ? objReporte.MovimientoDebe - objReporte.MovimientoHaber : 0;
                                    objReporte.gananacia = objReporte.MovimientoDebe < objReporte.MovimientoHaber ? (objReporte.MovimientoDebe - objReporte.MovimientoHaber) * -1 : 0;
                                    objReporte.imputable = imputable;
                                    decimal MovimientoDebe = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, pstrMes, false, pImputables);
                                    decimal MovimientoHaber = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, pstrMes, false, pImputables);

                                    decimal Perdida = MovimientoDebe > MovimientoHaber ? MovimientoDebe - MovimientoHaber : 0;
                                    decimal Ganancia = MovimientoDebe < MovimientoHaber ? (MovimientoDebe - MovimientoHaber) * -1 : 0;



                                    objReporte.Grupo = objReporte.pstrCuenta.StartsWith("7") ? "INGRESOS" : objReporte.pstrCuenta.StartsWith("6") ? "COSTO" : objReporte.pstrCuenta.StartsWith("9") ? "GASTO" : "";
                                    objReporte.iGrupo = objReporte.pstrCuenta.StartsWith("7") ? 1 : objReporte.pstrCuenta.StartsWith("6") ? 2 : objReporte.pstrCuenta.StartsWith("9") ? 3 : 0;


                                    //if (objReporte.pstrCuenta.StartsWith("6") || objReporte.pstrCuenta.StartsWith("9"))
                                    //{
                                    //    objReporte.Acumulado = objReporte.perdida - objReporte.gananacia;
                                    //    objReporte.Mensual = Perdida - Ganancia;
                                    //}
                                    //else if (objReporte.pstrCuenta.StartsWith("7"))
                                    //{
                                    //    objReporte.Acumulado = objReporte.gananacia - objReporte.perdida;
                                    //    objReporte.Mensual = Ganancia - Perdida;

                                    //}
                                    //objReporte.imputable = imputable;
                                    //ListaFinal.Add(objReporte);


                                    if (UsadoDesdeEstadoNaturaleza)
                                    {

                                        int iMesDesde = int.Parse(MesDesde);
                                        decimal PerdidaMes2 = 0, GananciaMes2 = 0, PerdidaMes3 = 0, GananciaMes3 = 0, PerdidaMes4 = 0, GananciaMes4 = 0, PerdidaMes5 = 0, GananciaMes5 = 0, PerdidaMes6 = 0, GananciaMes6 = 0;
                                        decimal PerdidaMes7 = 0, GananciaMes7 = 0, PerdidaMes8 = 0, GananciaMes8 = 0, PerdidaMes9 = 0, GananciaMes9 = 0, PerdidaMes10 = 0, GananciaMes10 = 0, PerdidaMes11 = 0, GananciaMes11 = 0, PerdidaMes12 = 0, GananciaMes12 = 0;
                                        decimal PerdidaMes1 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                        decimal GananciaMes1 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                        string NombreMes1 = NombreMeses(iMesDesde);
                                        string NombreMes2 = "", NombreMes3 = "", NombreMes4 = "", NombreMes5 = "", NombreMes6 = "", NombreMes7 = "", NombreMes8 = "", NombreMes9 = "", NombreMes10 = "", NombreMes11 = "", NombreMes12 = "";



                                        iMesDesde++;
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes2 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes2 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes2 = NombreMeses(iMesDesde);
                                        }
                                        iMesDesde++;
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes3 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes3 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes3 = NombreMeses(iMesDesde);
                                        }

                                        iMesDesde++;
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes4 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes4 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes4 = NombreMeses(iMesDesde);
                                        }

                                        iMesDesde++;


                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes5 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes5 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes5 = NombreMeses(iMesDesde);
                                        }

                                        iMesDesde++;
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes6 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes6 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes6 = NombreMeses(iMesDesde);
                                        }
                                        iMesDesde++;
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes7 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes7 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes7 = NombreMeses(iMesDesde);
                                        }
                                        iMesDesde++;
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes8 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes8 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes8 = NombreMeses(iMesDesde);
                                        }
                                        iMesDesde++;
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes9 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes9 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes9 = NombreMeses(iMesDesde);
                                        }
                                        iMesDesde++;
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes10 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes10 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes10 = NombreMeses(iMesDesde);
                                            iMesDesde++;
                                        }
                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes11 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes11 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes11 = NombreMeses(iMesDesde);
                                        }
                                        iMesDesde++;


                                        if (iMesDesde <= int.Parse(MesHasta))
                                        {
                                            PerdidaMes12 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) > SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) : 0;
                                            GananciaMes12 = SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) < SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) ? (SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "D", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables) - SumaImporteMovimientoPorNaturalezaCcosto(ListaImputables, "H", pIntMoneda, objReporte.pstrCuenta, objReporte.Ccosto, iMesDesde.ToString("00"), false, pImputables)) * -1 : 0;
                                            NombreMes12 = NombreMeses(iMesDesde);
                                        }


                                        if (objReporte.pstrCuenta.StartsWith("6") || objReporte.pstrCuenta.StartsWith("9") || objReporte.pstrCuenta.StartsWith("7"))
                                        {
                                            objReporte.Acumulado = objReporte.gananacia - objReporte.perdida;
                                            objReporte.Mensual = Ganancia - Perdida;
                                            objReporte.Mes1 = GananciaMes1 - PerdidaMes1;
                                            objReporte.Mes2 = GananciaMes2 - PerdidaMes2;
                                            objReporte.Mes3 = GananciaMes3 - PerdidaMes3;
                                            objReporte.Mes4 = GananciaMes4 - PerdidaMes4;
                                            objReporte.Mes5 = GananciaMes5 - PerdidaMes5;
                                            objReporte.Mes6 = GananciaMes6 - PerdidaMes6;
                                            objReporte.Mes7 = GananciaMes7 - PerdidaMes7;
                                            objReporte.Mes8 = GananciaMes8 - PerdidaMes8;
                                            objReporte.Mes9 = GananciaMes9 - PerdidaMes9;
                                            objReporte.Mes10 = GananciaMes10 - PerdidaMes10;
                                            objReporte.Mes11 = GananciaMes11 - PerdidaMes11;
                                            objReporte.Mes12 = GananciaMes12 - PerdidaMes12;
                                            objReporte.NombreMes1 = NombreMes1;
                                            objReporte.NombreMes2 = NombreMes2;
                                            objReporte.NombreMes3 = NombreMes3;
                                            objReporte.NombreMes4 = NombreMes4;
                                            objReporte.NombreMes5 = NombreMes5;
                                            objReporte.NombreMes6 = NombreMes6;
                                            objReporte.NombreMes7 = NombreMes7;
                                            objReporte.NombreMes8 = NombreMes8;
                                            objReporte.NombreMes9 = NombreMes9;
                                            objReporte.NombreMes10 = NombreMes10;
                                            objReporte.NombreMes11 = NombreMes11;
                                            objReporte.NombreMes12 = NombreMes12;

                                        }
                                        //else if (objReporte.pstrCuenta.StartsWith("7"))
                                        //{
                                        //    objReporte.Acumulado = objReporte.gananacia - objReporte.perdida;
                                        //    objReporte.Mensual = Ganancia - Perdida;
                                        //}

                                        objReporte.Ccosto = "CENTRO DE COSTO :" + item.Ccosto;
                                        ListaFinal.Add(objReporte);
                                    }
                                    else
                                    {
                                        if (objReporte.pstrCuenta.StartsWith("6") || objReporte.pstrCuenta.StartsWith("9"))
                                        {
                                            objReporte.Acumulado = objReporte.perdida - objReporte.gananacia;
                                            objReporte.Mensual = Perdida - Ganancia;
                                        }
                                        else if (objReporte.pstrCuenta.StartsWith("7"))
                                        {
                                            objReporte.Acumulado = objReporte.gananacia - objReporte.perdida;
                                            objReporte.Mensual = Ganancia - Perdida;
                                        }
                                        objReporte.IdCcosto = item.Ccosto;
                                        objReporte.imputable = imputable;
                                        objReporte.Ccosto = "CENTRO DE COSTO :" + item.Ccosto;
                                        ListaFinal.Add(objReporte);
                                    }




                                }
                                pIntDigitos = pIntDigitos + 1;
                            }
                            pIntDigitos = 2;
                        }
                    }
                }
                if (pImputables == 1)
                {
                    ListaFinal = ListaFinal.ToList().Where(y => y.imputable == 1 || (y.imputable == 0 && y.pstrCuenta.Length == 2)).OrderBy(x => x.pstrCuenta).ToList().Where(x => x.MovimientoDebe != 0 || x.MovimientoHaber != 0 || x.perdida != 0 || x.ganancia != 0).ToList();
                    if (Detallado)
                    {
                        var fechaInicio = new DateTime(int.Parse(pstrPeriodo), 1, 1);
                        var fechaFin = DateTime.Parse(new DateTime(int.Parse(pstrPeriodo), int.Parse(pstrMes), DateTime.DaysInMonth(int.Parse(pstrPeriodo), int.Parse(pstrMes))).ToShortDateString() + " 23:59");
                        List<ReporteAnalisisCuentasCteAnalitico> analisisCuentas = new List<ReporteAnalisisCuentasCteAnalitico>();
                        analisisCuentas = new AsientosContablesBL().ReporteAnalisisCuentasCteAnalitico(ref objOperationResult,
                       fechaInicio, fechaFin, null, null, false, null,
                       TipoReporteAnalisis.AnalisisCuentaCorrienteAnalitico, 0, null, null, false);
                        List<ReporteGananciasPerdidasporNaturalezaCcosto> ListaDetallada = new List<ReporteGananciasPerdidasporNaturalezaCcosto>();
                        foreach (var item in ListaFinal)
                        {

                            var detalles = analisisCuentas.Where(o => o.CuentaImputable == item.pstrCuenta && o.IdCentroCostos.Trim() == item.ValueCcosto).ToList();

                            foreach (var det in detalles)
                            {
                                ReporteGananciasPerdidasporNaturalezaCcosto objDetallado = new ReporteGananciasPerdidasporNaturalezaCcosto();
                                objDetallado = (ReporteGananciasPerdidasporNaturalezaCcosto)item.Clone();

                                objDetallado.GrupoCuenta = item.pstrCuenta;
                                objDetallado.NroDiario = det.DocumentoProvicion;
                                objDetallado.Analisis = det.Analisis;
                                objDetallado.MensualCuenta = int.Parse(det.Mes).ToString("00") == pstrMes ? pIntMoneda == 1 ? det.NaturalezaRegistro == "D" ? det.ImporteSolesD.Value : det.ImporteSolesH.Value * -1 : pIntMoneda == 2 ? det.ImporteDolaresD.Value : det.ImporteDolaresH.Value * -1 : 0;
                                objDetallado.AcumuladoCuenta = pIntMoneda == 1 ? det.NaturalezaRegistro == "D" ? det.ImporteSolesD.Value : det.ImporteSolesH.Value * -1 : pIntMoneda == 2 ? det.ImporteDolaresD.Value : det.ImporteDolaresH.Value * -1;
                                ListaDetallada.Add(objDetallado);
                            }
                            if (!detalles.Any())
                            {
                                item.GrupoCuenta = item.pstrCuenta;
                                ListaDetallada.Add(item);

                            }

                        }
                        return ListaDetallada.OrderBy(o => o.Ccosto).ThenBy(o => o.iGrupo).ToList();
                    }
                    else
                    {

                        return ListaFinal.OrderBy(o => o.Ccosto).ThenBy(o => o.iGrupo).ToList();
                    }
                }
                else
                {
                    ListaFinal = ListaFinal.OrderBy(x => x.pstrCuenta).ToList().Where(x => x.MovimientoDebe != 0 || x.MovimientoHaber != 0 || x.perdida != 0 || x.ganancia != 0).ToList();
                    if (Detallado)
                    {

                        var fechaInicio = new DateTime(int.Parse(pstrPeriodo), 1, 1);
                        var fechaFin = DateTime.Parse(new DateTime(int.Parse(pstrPeriodo), int.Parse(pstrMes), DateTime.DaysInMonth(int.Parse(pstrPeriodo), int.Parse(pstrMes))).ToShortDateString() + " 23:59");
                        List<ReporteAnalisisCuentasCteAnalitico> analisisCuentas = new List<ReporteAnalisisCuentasCteAnalitico>();
                        analisisCuentas = new AsientosContablesBL().ReporteAnalisisCuentasCteAnalitico(ref objOperationResult,
                       fechaInicio, fechaFin, null, null, false, null,
                       TipoReporteAnalisis.AnalisisCuentaCorrienteAnalitico, 0, null, null, false);
                        List<ReporteGananciasPerdidasporNaturalezaCcosto> ListaDetallada = new List<ReporteGananciasPerdidasporNaturalezaCcosto>();
                        foreach (var item in ListaFinal)
                        {

                            var detalles = analisisCuentas.Where(o => o.CuentaImputable.StartsWith(item.pstrCuenta) && o.IdCentroCostos.Trim() == item.ValueCcosto).ToList();

                            foreach (var det in detalles)
                            {
                                ReporteGananciasPerdidasporNaturalezaCcosto objDetallado = new ReporteGananciasPerdidasporNaturalezaCcosto();
                                objDetallado = (ReporteGananciasPerdidasporNaturalezaCcosto)item.Clone();

                                objDetallado.GrupoCuenta = item.pstrCuenta;
                                objDetallado.NroDiario = det.DocumentoProvicion;
                                objDetallado.Analisis = det.Analisis;
                                objDetallado.MensualCuenta = int.Parse(det.Mes).ToString("00") == pstrMes ? pIntMoneda == 1 ? det.NaturalezaRegistro == "D" ? det.ImporteSolesD.Value : det.ImporteSolesH.Value * -1 : pIntMoneda == 2 ? det.ImporteDolaresD.Value : det.ImporteDolaresH.Value * -1 : 0;
                                objDetallado.AcumuladoCuenta = pIntMoneda == 1 ? det.NaturalezaRegistro == "D" ? det.ImporteSolesD.Value : det.ImporteSolesH.Value * -1 : pIntMoneda == 2 ? det.ImporteDolaresD.Value : det.ImporteDolaresH.Value * -1;
                                ListaDetallada.Add(objDetallado);
                            }
                            if (!detalles.Any())
                            {
                                item.GrupoCuenta = item.pstrCuenta;
                                ListaDetallada.Add(item);

                            }

                        }
                        return ListaDetallada.OrderBy(o => o.Ccosto).ThenBy(o => o.iGrupo).ToList();

                    }
                    else
                    {
                        return ListaFinal.OrderBy(o => o.Ccosto).ThenBy(o => o.iGrupo).ToList();
                    }



                }



            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "ContabilidadBL.ReporteGananciasPerdidasporNaturalezaCcosto()";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }


        }




        public decimal SumaImporteMovimientoPorNaturalezaCcosto(List<ReporteGananciasPerdidasporNaturalezaCcosto> ListaTotal, string pstrNaturaleza, int pIntMoneda, string pstrCuenta, string IdCcosto, string pstrMes, bool Acumulado, int Imputable)
        {
            decimal ImporteMovimiento = 0;


            if (Acumulado)
            {
                var ListaImputables = ListaTotal.FindAll(x => x.pstrCuenta.StartsWith(pstrCuenta) && x.Ccosto == IdCcosto).ToList();
                ImporteMovimiento = pIntMoneda == 1 ? pstrNaturaleza == "D" ? ListaImputables.Sum(x => x.DebeSoles) : ListaImputables.Sum(x => x.HaberSoles) : pstrNaturaleza == "D" ? ListaImputables.Sum(x => x.DebeDolares) : ListaImputables.Sum(x => x.HaberDolares);
            }
            else
            {
                var ListaImputables = ListaTotal.Where(x => x.pstrCuenta.StartsWith(pstrCuenta) && x.Ccosto == IdCcosto && x.mes == pstrMes).ToList();
                ImporteMovimiento = pIntMoneda == 1 ? pstrNaturaleza == "D" ? ListaImputables.Sum(x => x.DebeSoles) : ListaImputables.Sum(x => x.HaberSoles) : pstrNaturaleza == "D" ? ListaImputables.Sum(x => x.DebeDolares) : ListaImputables.Sum(x => x.HaberDolares);
            }
            return ImporteMovimiento;
        }


        public List<KardexList> ReporteLibroInventarioBalanceMercaderias(ref OperationResult pobjOperationResult, DateTime pdtFechaInicio, DateTime pdtFechaFin, string pstrFilterExpression, int pintIdMoneda, string pstrAlmacen, string pstrMesIni, string pstrMesFin, string pstrCodigoProducto, string NroPedido, string Linea, string pstrGrupoLlave, string pstrOrden, int pIntIdEstablecimiento)
        {

            try
            {

                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    List<KardexList> Lista = new List<KardexList>();
                    List<KardexList> ListaAgrupada = new List<KardexList>();
                    KardexList oKardexList;
                    string IdProductoOld = "";
                    var query = (from A in dbContext.movimientodetalle
                                 join B in dbContext.productodetalle on new { IdProductoDetalle = A.v_IdProductoDetalle, eliminado = 0 } equals new { IdProductoDetalle = B.v_IdProductoDetalle, eliminado = B.i_Eliminado.Value } into B_join
                                 from B in B_join.DefaultIfEmpty()
                                 join C in dbContext.producto on new { IdProducto = B.v_IdProducto, eliminado = 0 } equals new { IdProducto = C.v_IdProducto, eliminado = C.i_Eliminado.Value } into C_join
                                 from C in C_join.DefaultIfEmpty()
                                 join F in dbContext.linea on new { IdLinea = C.v_IdLinea, eliminado = 0 } equals new { IdLinea = F.v_IdLinea, eliminado = F.i_Eliminado.Value } into F_join
                                 from F in F_join.DefaultIfEmpty()
                                 join D in dbContext.movimiento on new { IdMovimiento = A.v_IdMovimiento, eliminado = 0 } equals new { IdMovimiento = D.v_IdMovimiento, eliminado = D.i_Eliminado.Value } into D_join
                                 from D in D_join.DefaultIfEmpty()
                                 join E in dbContext.cliente on new { IdCliente = D.v_IdCliente, eliminado = 0 } equals new { IdCliente = E.v_IdCliente, eliminado = E.i_Eliminado.Value } into E_join
                                 from E in E_join.DefaultIfEmpty()
                                 join J1 in dbContext.datahierarchy on new { a = A.i_IdUnidad.Value, b = 17, eliminado = 0 }
                                                               equals new { a = J1.i_ItemId, b = J1.i_GroupId, eliminado = J1.i_IsDeleted.Value } into J1_join
                                 from J1 in J1_join.DefaultIfEmpty()

                                 join J2 in dbContext.datahierarchy on new { TipoProducto = C.i_IdTipoProducto.Value, Grupo = 6, eliminado = 0 } equals new { TipoProducto = J2.i_ItemId, Grupo = J2.i_GroupId, eliminado = J2.i_IsDeleted.Value } into J2_join

                                 from J2 in J2_join.DefaultIfEmpty()


                                 join J3 in dbContext.establecimientoalmacen on new { IdAlmacen = D.i_IdAlmacenOrigen.Value, eliminado = 0 } equals new { IdAlmacen = J3.i_IdAlmacen.Value, eliminado = J3.i_Eliminado.Value } into J3_join

                                 from J3 in J3_join.DefaultIfEmpty()

                                 join J4 in dbContext.almacen on new { IdAlmacen = J3.i_IdAlmacen.Value, eliminado = 0 } equals new { IdAlmacen = J4.i_IdAlmacen, eliminado = J4.i_Eliminado.Value } into J4_join

                                 from J4 in J4_join.DefaultIfEmpty()

                                 //join J5 in dbContext.documento on new { DocRef = A.i_IdTipoDocumento.Value, eliminado = 0 } equals new { DocRef=J5.i_CodigoDocumento,eliminado=0} into J5_join
                                 //from J5 in J5_join.DefaultIfEmpty ()

                                 where A.i_Eliminado == 0

                                 && (D.t_Fecha.Value >= pdtFechaInicio && D.t_Fecha.Value <= pdtFechaFin)
                                 && D.i_IdTipoMovimiento != (int)TipoDeMovimiento.Transferencia
                                 && (C.v_CodInterno == pstrCodigoProducto || pstrCodigoProducto == "") && (F.v_IdLinea == Linea || Linea == "-1")
                                 && C.i_EsServicio == 0 && D.i_IdEstablecimiento == pIntIdEstablecimiento
                                 orderby C.v_CodInterno, D.t_Fecha, A.v_NroPedido
                                 select new KardexList
                                 {
                                     v_IdMovimientoDetalle = A.v_IdMovimientoDetalle,
                                     v_IdProducto = C == null ? "Producto Eliminado" : C.v_IdProducto,
                                     v_NombreProducto = C == null ? "Producto Eliminado" : C.v_Descripcion,
                                     t_Fecha = D.t_Fecha,
                                     i_IdTipoMovimiento = D.i_IdTipoMovimiento,
                                     d_Cantidad = D.i_EsDevolucion == 1 ? A.d_CantidadEmpaque * -1 : A.d_CantidadEmpaque,
                                     d_Precio = C.d_Empaque.Value == 1 && J1.v_Value2 == "1" ? A.d_Precio : A.d_CantidadEmpaque == 0 ? 0 : A.d_Total / A.d_CantidadEmpaque,
                                     i_EsDevolucion = D.i_EsDevolucion,
                                     i_IdTipoMotivo = D.i_IdTipoMotivo,
                                     NroPedido = A.v_NroPedido == null ? string.Empty : A.v_NroPedido,
                                     v_IdLinea = C == null ? "Producto Eliminado" : C.v_IdLinea,
                                     IdMoneda = D.i_IdMoneda.Value,
                                     TipoCambio = D.d_TipoCambio.Value,
                                     Empresa = "",
                                     Ruc = "",
                                     Moneda = "",
                                     Almacen = J4.v_Nombre.Trim(),
                                     Mes = "",
                                     Al = "",
                                     ClienteProveedor = E.v_RazonSocial,
                                     Guia = A.v_NroGuiaRemision,
                                     Documento = A.v_NumeroDocumento,
                                     CodProducto = C.v_CodInterno,
                                     UnidadMedida = C.d_Empaque.Value == 1 && J1.v_Value2 == "1" ? J1.v_Value1 : "UNIDAD",
                                     IdAlmacen = D.i_IdAlmacenOrigen.Value,
                                     d_Empaque = C.d_Empaque,
                                     TipoExistencia = J2 == null ? "" : J2.v_Value2,
                                     NombreLinea = F == null ? "No Existe Linea" : F.v_Nombre.Trim(),
                                     GrupoLlave = pstrGrupoLlave == "LINEA" ? F == null ? "No Existe Linea" : F.v_Nombre.Trim() : J4.v_Nombre.Trim(),
                                     i_IdTipoProducto = C == null ? -1 : C.i_IdTipoProducto.Value,
                                     IdAlmacenOrigen = D.i_IdAlmacenOrigen.Value,
                                     i_IdTipoDocumentoDetalle = A.i_IdTipoDocumento.Value,
                                 }).ToList().AsQueryable();


                    if (!string.IsNullOrEmpty(pstrFilterExpression))
                    {
                        query = query.Where(pstrFilterExpression);
                    }
                    var queryList = query.ToList().OrderBy(x => x.IdAlmacenOrigen).ThenBy(x => x.v_IdProducto).ThenBy(x => x.t_Fecha).ThenBy(x => x.i_IdTipoMovimiento).ThenBy(x => x.NroPedido).ToList();
                    int Contador = query.Count();
                    var xxx = queryList.Where(x => x.CodProducto == "0066901839195" && x.t_Fecha == DateTime.Parse("07/12/2015"));

                    for (int i = 0; i < Contador; i++)
                    {
                        if (_objDocumentoBL.DocumentoEsContable(queryList[i].i_IdTipoDocumentoDetalle) || queryList[i].Documento == null) //Nulo porque no se generan de ningun compra o venta 
                        {

                            oKardexList = new KardexList();
                            oKardexList.v_IdProducto = queryList[i].v_IdProducto;
                            oKardexList.v_NombreProducto = queryList[i].v_NombreProducto;
                            oKardexList.t_Fecha = queryList[i].t_Fecha;
                            oKardexList.v_Fecha = queryList[i].t_Fecha.Value.ToString("dd-MMM");
                            oKardexList.v_NombreTipoMovimiento = queryList[i].i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso ? "INGRESO" : "SALIDA";
                            oKardexList.i_IdTipoMotivo = queryList[i].i_IdTipoMotivo;
                            oKardexList.Documento = queryList[i].Documento;
                            oKardexList.IdMoneda = queryList[i].IdMoneda;
                            oKardexList.TipoCambio = queryList[i].TipoCambio;
                            oKardexList.i_EsDevolucion = queryList[i].i_EsDevolucion;
                            oKardexList.ClienteProveedor = queryList[i].ClienteProveedor;
                            oKardexList.Empresa = queryList[i].Empresa;
                            oKardexList.Ruc = queryList[i].Ruc;
                            oKardexList.Moneda = queryList[i].Moneda;
                            oKardexList.Almacen = queryList[i].Almacen;
                            oKardexList.Mes = queryList[i].Mes;
                            oKardexList.Al = DateTime.Now.Year.ToString();
                            oKardexList.Guia = queryList[i].Guia;
                            oKardexList.Documento = queryList[i].Documento;
                            oKardexList.CodProducto = queryList[i].CodProducto;
                            oKardexList.UnidadMedida = queryList.Where(x => x.CodProducto == oKardexList.CodProducto && x.UnidadMedida == "UNIDAD").Count() != 0 ? "UNIDAD" : queryList[i].UnidadMedida;
                            oKardexList.NroPedido = queryList[i].NroPedido;
                            int Posicion = Lista.Count();
                            if (pintIdMoneda == (int)Currency.Soles)
                            {
                                if (queryList[i].i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso)
                                {
                                    oKardexList.Ingreso_Cantidad = queryList[i].d_Cantidad;
                                    oKardexList.Ingreso_Precio = pintIdMoneda == oKardexList.IdMoneda ? queryList[i].d_Precio : queryList[i].d_Precio * oKardexList.TipoCambio;
                                    oKardexList.Ingreso_Total = (oKardexList.Ingreso_Cantidad * oKardexList.Ingreso_Precio);

                                    if (i == 0)
                                    {
                                        oKardexList.Saldo_Cantidad = queryList[i].d_Cantidad;
                                        oKardexList.Saldo_Total = oKardexList.Ingreso_Total;
                                        oKardexList.Saldo_Precio = oKardexList.Ingreso_Precio;
                                    }
                                    else
                                    {
                                        if (IdProductoOld != queryList[i].v_IdProducto + " " + queryList[i].NroPedido)
                                        {
                                            oKardexList.Saldo_Cantidad = queryList[i].d_Cantidad;
                                            oKardexList.Saldo_Precio = pintIdMoneda == oKardexList.IdMoneda ? queryList[i].d_Precio : queryList[i].d_Precio * oKardexList.TipoCambio;
                                            oKardexList.Saldo_Total = (oKardexList.Saldo_Cantidad * oKardexList.Saldo_Precio);
                                        }
                                        else
                                        {
                                            oKardexList.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad + oKardexList.Ingreso_Cantidad;
                                            oKardexList.Saldo_Total = (Lista[Posicion - 1].Saldo_Total + oKardexList.Ingreso_Total);
                                            oKardexList.Saldo_Precio = (oKardexList.Saldo_Total == 0 || oKardexList.Saldo_Cantidad == 0) ? 0 : (oKardexList.Saldo_Total / oKardexList.Saldo_Cantidad);
                                        }
                                    }
                                }
                                else
                                {
                                    oKardexList.Salida_Cantidad = queryList[i].d_Cantidad;
                                    if (i != 0) oKardexList.Salida_Precio = Lista[Posicion - 1].Saldo_Precio;
                                    oKardexList.Salida_Total = (oKardexList.Salida_Cantidad * oKardexList.Salida_Precio);

                                    if (i == 0)
                                    {
                                        oKardexList.Saldo_Cantidad = oKardexList.Salida_Cantidad;
                                        oKardexList.Saldo_Total = oKardexList.Salida_Total == null ? 0 : oKardexList.Salida_Total;
                                        oKardexList.Saldo_Precio = oKardexList.Salida_Precio == null ? 0 : oKardexList.Salida_Precio;
                                    }
                                    else
                                    {
                                        if (IdProductoOld != queryList[i].v_IdProducto + " " + queryList[i].NroPedido)
                                        {
                                            oKardexList.Saldo_Cantidad = queryList[i].d_Cantidad;
                                            oKardexList.Saldo_Precio = pintIdMoneda == oKardexList.IdMoneda ? queryList[i].d_Precio : queryList[i].d_Precio * oKardexList.TipoCambio;
                                            oKardexList.Saldo_Total = pintIdMoneda == oKardexList.IdMoneda ? (oKardexList.Saldo_Cantidad * oKardexList.Saldo_Precio) : (oKardexList.Saldo_Cantidad * oKardexList.Saldo_Precio) * oKardexList.TipoCambio;
                                        }
                                        else
                                        {
                                            oKardexList.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad - queryList[i].d_Cantidad;
                                            oKardexList.Saldo_Total = (Lista[Posicion - 1].Saldo_Total - oKardexList.Salida_Total);
                                            oKardexList.Saldo_Precio = (oKardexList.Saldo_Total == 0 || oKardexList.Saldo_Cantidad == 0) ? 0 : (oKardexList.Saldo_Total / oKardexList.Saldo_Cantidad);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (queryList[i].i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso)
                                {
                                    oKardexList.Ingreso_Cantidad = queryList[i].d_Cantidad;
                                    oKardexList.Ingreso_Precio = pintIdMoneda == oKardexList.IdMoneda ? queryList[i].d_Precio : queryList[i].d_Precio / oKardexList.TipoCambio;
                                    oKardexList.Ingreso_Total = (oKardexList.Ingreso_Cantidad * oKardexList.Ingreso_Precio);

                                    if (i == 0)
                                    {
                                        oKardexList.Saldo_Cantidad = queryList[i].d_Cantidad;
                                        oKardexList.Saldo_Total = oKardexList.Ingreso_Total;
                                        oKardexList.Saldo_Precio = oKardexList.Ingreso_Precio;
                                    }
                                    else
                                    {
                                        if (IdProductoOld != queryList[i].v_IdProducto + " " + queryList[i].NroPedido)
                                        {
                                            oKardexList.Saldo_Cantidad = queryList[i].d_Cantidad;
                                            oKardexList.Saldo_Precio = pintIdMoneda == oKardexList.IdMoneda ? queryList[i].d_Precio : queryList[i].d_Precio / oKardexList.TipoCambio;
                                            oKardexList.Saldo_Total = (oKardexList.Saldo_Cantidad * oKardexList.Saldo_Precio);
                                        }
                                        else
                                        {
                                            oKardexList.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad + oKardexList.Ingreso_Cantidad;
                                            oKardexList.Saldo_Total = (Lista[Posicion - 1].Saldo_Total + oKardexList.Ingreso_Total);
                                            oKardexList.Saldo_Precio = (oKardexList.Saldo_Total == 0 || oKardexList.Saldo_Cantidad == 0) ? 0 : (oKardexList.Saldo_Total / oKardexList.Saldo_Cantidad);
                                        }

                                    }
                                }
                                else
                                {
                                    oKardexList.Salida_Cantidad = queryList[i].d_Cantidad;
                                    if (i != 0) oKardexList.Salida_Precio = Lista[Posicion - 1].Saldo_Precio;
                                    oKardexList.Salida_Total = (oKardexList.Salida_Precio * oKardexList.Salida_Cantidad);

                                    if (i == 0)
                                    {
                                        oKardexList.Saldo_Cantidad = queryList[i].d_Cantidad;
                                        oKardexList.Saldo_Total = oKardexList.Salida_Total == null ? 0 : oKardexList.Salida_Total;
                                        oKardexList.Saldo_Precio = oKardexList.Salida_Precio == null ? 0 : oKardexList.Salida_Precio;
                                    }
                                    else
                                    {
                                        if (IdProductoOld != queryList[i].v_IdProducto + " " + queryList[i].NroPedido)
                                        {
                                            oKardexList.Saldo_Cantidad = queryList[i].d_Cantidad;
                                            oKardexList.Saldo_Precio = pintIdMoneda == oKardexList.IdMoneda ? queryList[i].d_Precio : queryList[i].d_Precio / oKardexList.TipoCambio;
                                            oKardexList.Saldo_Total = pintIdMoneda == oKardexList.IdMoneda ? (oKardexList.Saldo_Cantidad * oKardexList.Saldo_Precio) : (oKardexList.Saldo_Cantidad * oKardexList.Saldo_Precio) / oKardexList.TipoCambio;
                                        }
                                        else
                                        {
                                            oKardexList.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad - queryList[i].d_Cantidad;
                                            oKardexList.Saldo_Total = (Lista[Posicion - 1].Saldo_Total - oKardexList.Salida_Total);
                                            oKardexList.Saldo_Precio = (oKardexList.Saldo_Total == 0 || oKardexList.Saldo_Cantidad == 0) ? 0 : (oKardexList.Saldo_Total / oKardexList.Saldo_Cantidad);
                                        }
                                    }
                                }
                            }
                            oKardexList.TipoExistencia = queryList[i].TipoExistencia;
                            oKardexList.NombreLinea = queryList[i].NombreLinea;
                            oKardexList.GrupoLlave = queryList[i].GrupoLlave;
                            IdProductoOld = queryList[i].v_IdProducto + " " + queryList[i].NroPedido;
                            Lista.Add(oKardexList);
                        }
                    }

                    if (pstrOrden == "CÓDIGO PRODUCTO")
                    {
                        ListaAgrupada = Lista.GroupBy(x => new { x.v_IdProducto, x.NroPedido })
                                    .Select(group => group.Last())
                                    .OrderBy(o => o.CodProducto).ToList();
                    }
                    else
                    {
                        ListaAgrupada = Lista.GroupBy(x => new { x.v_IdProducto, x.NroPedido })
                                      .Select(group => group.Last())
                                      .OrderBy(o => o.v_NombreProducto).ToList();

                    }


                    return ListaAgrupada;
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ContabilidadBL.ReporteLibroInventarioBalanceMercaderias";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public decimal CuentaPerdidaNaturaleza61ReporteGananciaNaturalezaCcosto(string pstrCuenta, List<ReporteGananciasPerdidasporNaturalezaCcosto> ReportePN, int pIntMoneda, string pstrNaturaleza, string pIntCcosto)
        {
            try
            {
                decimal SumaDebe, SumaHaber, saldoDeudor, saldoAcreedor, perdida, ganancia;
                if (pstrCuenta.Length > 2)
                {
                    string pstrComplemento = pstrCuenta.Substring(2, (pstrCuenta.Length - 2));
                    string pstrcuentaNueva = "69" + pstrComplemento;
                    SumaDebe = pIntMoneda == 1 ? ReportePN.FindAll(y => y.pstrCuenta.Length >= pstrCuenta.Length).FindAll(x => x.pstrCuenta.Substring(0, pstrCuenta.Length) == pstrcuentaNueva && x.Ccosto == pIntCcosto).ToList().Sum(y => y.DebeSoles) : ReportePN.FindAll(x => x.pstrCuenta.Substring(0, pstrCuenta.Length) == pstrcuentaNueva && x.Ccosto == pIntCcosto).ToList().Sum(y => y.DebeDolares);
                    SumaHaber = pIntMoneda == 1 ? ReportePN.FindAll(y => y.pstrCuenta.Length >= pstrCuenta.Length).FindAll(x => x.pstrCuenta.Substring(0, pstrCuenta.Length) == pstrcuentaNueva && x.Ccosto == pIntCcosto).ToList().Sum(y => y.HaberSoles) : ReportePN.FindAll(x => x.pstrCuenta.Substring(0, pstrCuenta.Length) == pstrcuentaNueva && x.Ccosto == pIntCcosto).ToList().Sum(y => y.HaberDolares);
                    saldoDeudor = SumaDebe > SumaHaber ? SumaDebe - SumaHaber : 0;
                    saldoAcreedor = SumaDebe < SumaHaber ? (SumaDebe - SumaHaber) * -1 : 0;
                    perdida = saldoDeudor;
                    ganancia = saldoAcreedor;
                }

                else
                {
                    SumaDebe = pIntMoneda == 1 ? ReportePN.FindAll(x => x.pstrCuenta.Substring(0, 2) == "69" && x.Ccosto == pIntCcosto).ToList().Sum(x => x.DebeSoles) : ReportePN.FindAll(x => x.pstrCuenta.Substring(0, 2) == "69" && x.Ccosto == pIntCcosto).ToList().Sum(x => x.DebeDolares);
                    SumaHaber = pIntMoneda == 1 ? ReportePN.FindAll(x => x.pstrCuenta.Substring(0, 2) == "69" && x.Ccosto == pIntCcosto).ToList().Sum(x => x.HaberSoles) : ReportePN.FindAll(x => x.pstrCuenta.Substring(0, 2) == "69" && x.Ccosto == pIntCcosto).ToList().Sum(x => x.HaberDolares);
                    saldoDeudor = SumaDebe > SumaHaber ? SumaDebe - SumaHaber : 0;
                    saldoAcreedor = SumaDebe < SumaHaber ? (SumaDebe - SumaHaber) * -1 : 0;
                    perdida = saldoDeudor;
                    ganancia = saldoAcreedor;

                }
                if (pstrNaturaleza == "P")
                {
                    return perdida;
                }
                else
                {
                    return ganancia;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }



        public List<ReporteBalanceCapital> ReporteBalanceCapital(ref OperationResult pobjOperationResult, int Mesecito, int Periodo, string NroCuenta)
        {

            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                try
                {
                    ReporteBalanceCapital objReporte = new ReporteBalanceCapital();

                    var Socios = dbContext.cliente.Where(o => o.i_Eliminado == 0 && o.v_FlagPantalla == "A").ToList();
                    List<ReporteBalanceCapital> ListaFinal = new List<ReporteBalanceCapital>();
                    var aptitudeCertificate = new ContabilidadBL().ReporteLibroInventarioBalanceClientes(ref pobjOperationResult, Mesecito, Periodo, NroCuenta, "");
                    decimal CapitalSocial = aptitudeCertificate.Sum(o => o.MontoCobrar);

                    if (pobjOperationResult.Success == 1)
                    {
                        objReporte = new ReporteBalanceCapital();
                        objReporte.Grupo = "DETALLE DE LA PARTICIPACIÓN ACCIONARIA O PARTICIPACIONES SOCIALES : ";
                        objReporte.Detalle = "CAPITAL SOCIAL O PARTICIPACIONES AL 31.12 :";
                        objReporte.DetalleValor = CapitalSocial;
                        ListaFinal.Add(objReporte);
                        objReporte = new ReporteBalanceCapital();
                        objReporte.Grupo = "DETALLE DE LA PARTICIPACIÓN ACCIONARIA O PARTICIPACIONES SOCIALES : ";
                        objReporte.Detalle = "VALOR NOMINAL POR ACCIÓN O PARTICIPACIÓN SOCIAL :";
                        objReporte.DetalleValor = 1;
                        ListaFinal.Add(objReporte);
                        objReporte = new ReporteBalanceCapital();
                        objReporte.Grupo = "DETALLE DE LA PARTICIPACIÓN ACCIONARIA O PARTICIPACIONES SOCIALES : ";
                        objReporte.Detalle = "NUMERO DE ACCIONES O PARTICIPACIONES SOCIALES SUSCRITAS :";
                        objReporte.DetalleValor = Socios.Sum(o => o.i_NumeroAccionesSuscritas ?? 0);
                        ListaFinal.Add(objReporte);
                        objReporte = new ReporteBalanceCapital();
                        objReporte.Grupo = "DETALLE DE LA PARTICIPACIÓN ACCIONARIA O PARTICIPACIONES SOCIALES : ";
                        objReporte.Detalle = "NUMERO DE ACCIONES O PARTICIPACIONES SOCIALES PAGADAS :";
                        objReporte.DetalleValor = Socios.Sum(o => o.i_NumeroAccionesPagadas ?? 0);
                        ListaFinal.Add(objReporte);
                        objReporte = new ReporteBalanceCapital();
                        objReporte.Grupo = "DETALLE DE LA PARTICIPACIÓN ACCIONARIA O PARTICIPACIONES SOCIALES : ";
                        objReporte.Detalle = "NÚMERO DE ACCIONISTAS O SOCIOS : ";
                        objReporte.DetalleValor = Socios.Count();
                        ListaFinal.Add(objReporte);
                    }
                    return ListaFinal;
                }

                catch (Exception ex)
                {

                    pobjOperationResult.Success = 0;
                    pobjOperationResult.AdditionalInformation = "ContabilidadBL.ReporteBalanceCapital";
                    pobjOperationResult.ErrorMessage = ex.Message;
                    pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                    Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                    return null;
                }
            }


        }

        public List<ReporteSocios> ReporteSocios(ref OperationResult objOperationResult, decimal Capital)
        {

            try
            {
                List<ReporteSocios> ListaSocios = new List<Common.BE.ReporteSocios>();
                ReporteSocios objReporte = new Common.BE.ReporteSocios();
                var filters = new Queue<string>();
                objOperationResult.Success = 1;
                filters.Enqueue("v_FlagPantalla.Contains(\"A\")");
                var strFilterExpression = filters.Count > 0 ? string.Join(" && ", filters) : null;
                var Clientes = new ClienteBL().ReporteCliente(ref  objOperationResult, "", -1, strFilterExpression, -1, -1, -1, -1);
                if (Clientes == null) return null;
                foreach (var item in Clientes)
                {
                    objReporte = new ReporteSocios();

                    objReporte.TipoDocumento = item.TipoDocumento;
                    objReporte.NumeroDocumento = item.v_NroDocIdentificacion;
                    objReporte.RazonSocial = item.NombreRazonSocial;
                    objReporte.TipoAccionSocio = item.TipoAcciones;
                    objReporte.NumeroAcciones = item.AccionesPagadas + item.AccionesSuscritas;
                    decimal Porc = Capital == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((objReporte.NumeroAcciones / Capital) * 100, 2);
                    objReporte.Porcentaje = Porc < 0 ? Porc * -1 + " %" : Porc * -1 + " %";
                    ListaSocios.Add(objReporte);
                }
                return ListaSocios;
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "ContabilidadBL.ReporteBalanceCapital";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);

                return null;
            }

        }

        public List<ReporteLibroInventarioBalanceCajaBanco> ReporteLibroInventarioBalanceCajaBanco(ref OperationResult objOperationResult, int Mes, int Periodo, string NroCuenta)
        {

            try
            {
                objOperationResult.Success = 1;
                AsientosContablesBL _objAsientosContablesBL = new AsientosContablesBL();
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        DateTime FechaInicio = DateTime.Parse("01/01/" + Periodo.ToString());
                        DateTime FechaFin = DateTime.Parse(DateTime.DaysInMonth(Periodo, Mes).ToString() + "/" + Mes.ToString("00") + "/" + Periodo.ToString() + " 23:59");
                        ReporteLibroInventarioBalanceCajaBanco objReporte = new ReporteLibroInventarioBalanceCajaBanco();
                        List<ReporteLibroInventarioBalanceCajaBanco> ListaReporte = new List<ReporteLibroInventarioBalanceCajaBanco>();
                        List<datahierarchyDto> ListaMoneda = new List<datahierarchyDto>();
                        ListaMoneda = dbContext.datahierarchy.Where(o => o.i_GroupId == 18 && o.i_IsDeleted == 0).ToList().ToDTOs();
                        #region Recolecta las cuentas a procesar
                        // var RegistrosDiarioTesoreria = DevuelveAsientosParaAjusteDiferenciaCambio(Mes, Periodo, NroCuenta);
                        var RegistrosDiarioTesoreria = _objAsientosContablesBL.ReporteAnalisisCuentasCteAnalitico(ref objOperationResult, FechaInicio, FechaFin, "", "", false, "", TipoReporteAnalisis.AnalisisCorrientesPendientesAnalitico, 0, "", NroCuenta);

                        var CuentasDiferenciaCambio = RegistrosDiarioTesoreria.Select(x => x.CuentaImputable).Distinct();
                        #endregion
                        if (RegistrosDiarioTesoreria != null)
                        {
                            foreach (string CuentaDiferenciaCambio in CuentasDiferenciaCambio)
                            {
                                var RegistrosPorCuenta = RegistrosDiarioTesoreria.Where(p => p.CuentaImputable == CuentaDiferenciaCambio).ToList();
                                if (RegistrosPorCuenta.Count > 0)
                                {

                                    #region Operacion de cuentas sin detalle

                                    var RegistroProvicional = RegistrosPorCuenta.Where(p => p.NaturalezaCuenta == p.NaturalezaRegistro).ToList();
                                    var RegistroCancelacion = RegistrosPorCuenta.Where(p => p.NaturalezaCuenta != p.NaturalezaRegistro).ToList();
                                    if (RegistroProvicional.Count > 0)
                                    {
                                        decimal ImporteSolesDebe, ImporteSolesHaber;


                                        if (RegistroProvicional.FirstOrDefault().NaturalezaCuenta == "D")
                                        {

                                            ImporteSolesDebe = RegistroProvicional.Sum(p => p.ImporteSolesD.Value);
                                            ImporteSolesHaber = RegistroCancelacion.Sum(p => p.ImporteSolesH.Value);
                                            //  ImporteDolaresDebe = RegistroProvicional.Sum(p => p.ImporteDolaresD.Value);
                                            // ImporteDolaresHaber = RegistroCancelacion.Sum(p => p.ImporteDolaresH.Value);

                                        }
                                        else
                                        {
                                            ImporteSolesDebe = RegistroCancelacion.Sum(p => p.ImporteSolesD.Value);
                                            ImporteSolesHaber = RegistroProvicional.Sum(p => p.ImporteSolesH.Value);
                                            //ImporteDolaresDebe = RegistroCancelacion.Sum(p => p.ImporteDolaresD.Value);
                                            //ImporteDolaresHaber = RegistroProvicional.Sum(p => p.ImporteDolaresH.Value);

                                        }
                                        var CodigoMonedaSunat = new ContabilidadBL().EncontrarCodigoSunatMoneda(ListaMoneda, RegistrosPorCuenta.FirstOrDefault().IdMonedaCuenta.Value);

                                        objReporte = new ReporteLibroInventarioBalanceCajaBanco();
                                        objReporte.CuentaImputable = RegistrosPorCuenta.FirstOrDefault().CuentaImputable;
                                        objReporte.EntidadFinanceriaCuenta = RegistrosPorCuenta.FirstOrDefault().EntidadFinanceriaCuenta;
                                        objReporte.NroCuentaEntidadFinanciera = RegistrosPorCuenta.FirstOrDefault().NroCuentaEntidadFinanciera;
                                        objReporte.TipoMonedaCuenta = CodigoMonedaSunat;
                                        objReporte.ImporteSolesH = ImporteSolesHaber > ImporteSolesDebe ? ImporteSolesHaber - ImporteSolesDebe : 0; //Acreedor
                                        objReporte.ImporteSolesD = ImporteSolesDebe > ImporteSolesHaber ? ImporteSolesDebe - ImporteSolesHaber : 0;//Debe
                                        objReporte.NombreCuenta = RegistrosPorCuenta.FirstOrDefault().NombreCuenta;

                                        objReporte.Detalle = RegistrosPorCuenta.FirstOrDefault().Detalle;
                                        objReporte.Documento = RegistrosPorCuenta.FirstOrDefault().Documento;

                                        if (objReporte.ImporteSolesH == 0 && objReporte.ImporteSolesD == 0)
                                        {
                                        }
                                        else
                                        {
                                            ListaReporte.Add(objReporte);
                                        }
                                    }

                                    else if (RegistroCancelacion.Count() > 0)
                                    {
                                        decimal ImporteSolesDebe, ImporteSolesHaber;
                                        //, ImporteDolaresDebe, ImporteDolaresHabe
                                        if (RegistroCancelacion.FirstOrDefault().NaturalezaCuenta == "D")
                                        {

                                            ImporteSolesDebe = RegistroProvicional.Sum(p => p.ImporteSolesD.Value);
                                            ImporteSolesHaber = RegistroCancelacion.Sum(p => p.ImporteSolesH.Value);
                                            //ImporteDolaresDebe = RegistroProvicional.Sum(p => p.ImporteDolaresD.Value);
                                            //ImporteDolaresHaber = RegistroCancelacion.Sum(p => p.ImporteDolaresH.Value);
                                        }
                                        else
                                        {


                                            ImporteSolesDebe = RegistroCancelacion.Sum(p => p.ImporteSolesD.Value);
                                            ImporteSolesHaber = RegistroProvicional.Sum(p => p.ImporteSolesH.Value);
                                            //  ImporteDolaresDebe = RegistroCancelacion.Sum(p => p.ImporteDolaresD.Value);
                                            // ImporteDolaresHaber = RegistroProvicional.Sum(p => p.ImporteDolaresH.Value);
                                        }
                                        objReporte = new ReporteLibroInventarioBalanceCajaBanco();

                                        var CodigoMonedaSunat = new ContabilidadBL().EncontrarCodigoSunatMoneda(ListaMoneda, RegistrosPorCuenta.FirstOrDefault().IdMonedaCuenta.Value);
                                        objReporte.CuentaImputable = RegistrosPorCuenta.FirstOrDefault().CuentaImputable;
                                        objReporte.EntidadFinanceriaCuenta = RegistrosPorCuenta.FirstOrDefault().EntidadFinanceriaCuenta;
                                        objReporte.NroCuentaEntidadFinanciera = RegistrosPorCuenta.FirstOrDefault().NroCuentaEntidadFinanciera;
                                        objReporte.TipoMonedaCuenta = CodigoMonedaSunat; // RegistrosPorCuenta.FirstOrDefault().IdMonedaCuenta.Value.ToString("00");
                                        objReporte.ImporteSolesH = ImporteSolesHaber > ImporteSolesDebe ? ImporteSolesHaber - ImporteSolesDebe : 0; //Acreedor
                                        objReporte.ImporteSolesD = ImporteSolesDebe > ImporteSolesHaber ? ImporteSolesDebe - ImporteSolesHaber : 0;//Debe
                                        objReporte.NombreCuenta = RegistrosPorCuenta.FirstOrDefault().NombreCuenta;
                                        objReporte.Detalle = RegistrosPorCuenta.FirstOrDefault().Detalle;
                                        objReporte.Documento = RegistrosPorCuenta.FirstOrDefault().Documento;
                                        if (objReporte.ImporteSolesH == 0 && objReporte.ImporteSolesD == 0)
                                        {
                                        }
                                        else
                                        {
                                            ListaReporte.Add(objReporte);
                                        }

                                    }

                                    #endregion
                                }
                            }
                        }
                        return ListaReporte.OrderBy(x => x.CuentaImputable).ToList();

                    }
                }
            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                return null;
            }
        }



        public string EncontrarCodigoSunatMoneda(List<datahierarchyDto> GrupoMoneda, int IdMoneda)
        {

            var CodigoMonedaSunat = GrupoMoneda.Where(o => o.i_ItemId == IdMoneda).FirstOrDefault();
            if (CodigoMonedaSunat != null)
            {

                return CodigoMonedaSunat.v_Field;

            }
            else
            {
                return "";
            }

        }
        private List<ReporteLibroInventarioBalanceCajaBanco> DevuelveAsientosParaAjusteDiferenciaCambio(int pintMes, int pintPeriodo, string NroCuenta)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    List<asientocontable> ListaAsientoContable = dbContext.asientocontable.ToList();
                    #region Recopila Cuentas de Tesorería
                    var GruposTesoreria = (from n in dbContext.tesoreriadetalle

                                           join J1 in dbContext.asientocontable on new { c = n.v_NroCuenta.Trim(), eliminado = 0, p = periodo } equals new { c = J1.v_NroCuenta.Trim(), eliminado = J1.i_Eliminado.Value, p = J1.v_Periodo } into J1_join
                                           from J1 in J1_join.DefaultIfEmpty()

                                           join J2 in dbContext.documento on new { td = n.i_IdTipoDocumento.Value, eliminado = 0 } equals new { td = J2.i_CodigoDocumento, eliminado = J2.i_Eliminado.Value } into J2_join
                                           from J2 in J2_join.DefaultIfEmpty()

                                           join J3 in dbContext.cliente on new { c = n.v_IdCliente, eliminado = 0 } equals new { c = J3.v_IdCliente, eliminado = J3.i_Eliminado.Value } into J3_join
                                           from J3 in J3_join.DefaultIfEmpty()

                                           join J4 in dbContext.tesoreria on new { IdTesoreria = n.v_IdTesoreria, eliminado = 0 } equals new { IdTesoreria = J4.v_IdTesoreria, eliminado = J4.i_Eliminado.Value } into J4_join
                                           from J4 in J4_join.DefaultIfEmpty()

                                           join J5 in dbContext.documento on n.i_IdTipoDocumentoRef.Value equals J5.i_CodigoDocumento into J5_join
                                           from J5 in J5_join.DefaultIfEmpty()

                                           join J6 in dbContext.documento on J4.i_IdTipoDocumento.Value equals J6.i_CodigoDocumento into J6_join
                                           from J6 in J6_join.DefaultIfEmpty()

                                           where n.i_Eliminado == 0 && n.t_Fecha.Value.Month >= 1 && n.t_Fecha.Value.Month <= pintMes && n.t_Fecha.Value.Year == pintPeriodo
                                           && n.v_NroCuenta.StartsWith(NroCuenta)
                                           && J4.i_IdEstado == 1
                                           select new
                                           {
                                               CuentaMayor = n.v_NroCuenta.Substring(0, 2),
                                               CuentaImputable = n.v_NroCuenta,
                                               NombreCuenta = J1 != null ? J1.v_NombreCuenta : string.Empty,
                                               FechaDocumento = n.t_Fecha,
                                               DocumentoTransaccion = J6 != null ? J6.v_Siglas + " " + J4.v_Mes.Trim() + J4.v_Correlativo.Trim() : string.Empty,
                                               Documento = J2.v_Siglas + " " + n.v_NroDocumento,
                                               DocumentoRef = J5 != null ? J5.v_Siglas + " " + n.v_NroDocumentoRef : string.Empty,
                                               Analisis = n.v_Analisis == null ? !string.IsNullOrEmpty(J4.v_Nombre) ? J4.v_Nombre : string.Empty : n.v_Analisis,
                                               IdMoneda = J4.i_IdMoneda,
                                               SiglaMoneda = J4.i_IdMoneda == 1 ? "S" : "D",
                                               Importe = n != null ? n.d_Importe : 0,
                                               ImporteCambio = n != null ? n.d_Cambio : 0,
                                               Key = J3 != null ? J3.v_IdCliente + n.v_NroCuenta + J2.v_Siglas + n.v_NroDocumento : n.v_NroCuenta + J2.v_Siglas + n.v_NroDocumento,
                                               NaturalezaCuenta = J1 != null ? J1.i_Naturaleza : 0,
                                               IdMonedaCuenta = J1 != null ? J1.i_IdMoneda : 0,
                                               NaturalezaRegistro = n.v_Naturaleza,
                                               CuentaConDetalle = J1.i_Detalle == 1 ? true : false,
                                               TipoCambio = J4.d_TipoCambio,
                                               IdAnexo = J3.v_IdCliente,
                                               Detalle_Ruc = J3 != null ? J3.v_NroDocIdentificacion : string.Empty,
                                               TipoDocumento = n != null ? n.i_IdTipoDocumento : 0,
                                               TipoDocumentoRef = n != null ? n.i_IdTipoDocumentoRef : 0,
                                               NroDocumento = n != null ? n.v_NroDocumento : "",
                                               NroDocumentoRef = n != null ? n.v_NroDocumentoRef : "",
                                               EntidadFinancieraCuenta = J1.i_IdentificaCtaBancos,
                                               TipoMonedaCuenta = J1.i_IdMoneda.Value,
                                               Cliente = J3 != null ? J3.v_NroDocIdentificacion.Trim() + "             " + J3.v_CodCliente.Trim() + "   " + (J3.v_ApePaterno.Trim() + " " + J3.v_ApeMaterno.Trim() + " " + J3.v_PrimerNombre.Trim() + J3.v_RazonSocial.Trim()).Trim() : string.Empty,
                                               FechaComprobante = J4.t_FechaRegistro.Value,
                                               Concepto = (n.v_Analisis == null || n.v_Analisis == string.Empty) ? J4.v_Glosa : n.v_Analisis,
                                               TipoIdentificacion = J3 != null ? J3.i_IdTipoIdentificacion.Value : -1,

                                           }
                                            ).ToList().Select(p => new ReporteLibroInventarioBalanceCajaBanco
                                            {
                                                CuentaMayor = p.CuentaMayor + "   " + NombreCuenta(p.CuentaMayor, ListaAsientoContable),
                                                CuentaImputable = p.CuentaImputable,
                                                NombreCuenta = p.NombreCuenta,
                                                Documento = p.DocumentoTransaccion + " \t" + (p.FechaDocumento.Value.ToShortDateString() + " \t" + p.Documento).Trim(),
                                                DocumentoRef = !string.IsNullOrEmpty(p.DocumentoRef) ? (p.DocumentoRef + " " + p.Analisis).Trim() : string.Empty,
                                                FechaDocumento = p.FechaDocumento.Value,
                                                DocumentoKey = p.Key,
                                                IdMoneda = (int)p.IdMoneda,
                                                IdMonedaCuenta = (int)p.IdMonedaCuenta,
                                                NaturalezaCuenta = p.NaturalezaCuenta == 1 ? "D" : "H",
                                                NaturalezaRegistro = p.NaturalezaRegistro,
                                                ImporteSolesD = p.NaturalezaRegistro == "D" ? p.IdMoneda.Value == 1 ? p.Importe.Value : p.ImporteCambio.Value : 0,
                                                ImporteSolesH = p.NaturalezaRegistro == "H" ? p.IdMoneda == 1 ? p.Importe : p.ImporteCambio : 0,
                                                ImporteDolaresD = p.NaturalezaRegistro == "D" ? p.IdMoneda == 1 ? p.ImporteCambio : p.Importe : 0,
                                                ImporteDolaresH = p.NaturalezaRegistro == "H" ? p.IdMoneda == 1 ? p.ImporteCambio : p.Importe : 0,
                                                CuentaConDetalle = p.CuentaConDetalle,
                                                TipoCambio = p.TipoCambio.Value,
                                                IdAnexo = p.IdAnexo,
                                                // TipoDocumento = p.TipoDocumento != 7 && p.TipoDocumento != 8 ? (int)p.TipoDocumento : (int)p.TipoDocumentoRef,
                                                TipoDocumento = !_objDocumentoBL.DocumentoEsInverso(p.TipoDocumento.Value) && p.TipoDocumento != 8 ? (int)p.TipoDocumento : (int)p.TipoDocumentoRef,
                                                //  NroDocumento = p.TipoDocumento != 7 && p.TipoDocumento != 8 ? p.NroDocumento : p.NroDocumentoRef,
                                                NroDocumento = !_objDocumentoBL.DocumentoEsInverso(p.TipoDocumento.Value) && p.TipoDocumento != 8 ? p.NroDocumento : p.NroDocumentoRef,
                                                EntidadFinanceriaCuenta = p.EntidadFinancieraCuenta.ToString(),
                                                TipoMonedaCuenta = p.TipoMonedaCuenta == (int)Currency.Soles ? "01" : "02",
                                                Cliente = p.TipoIdentificacion + "        " + p.Cliente,
                                                FechaComprobante = p.FechaComprobante,
                                                Concepto = p.Concepto,


                                            }
                                            ).ToList();
                    #endregion

                    #region Recopila Cuentas de Diario
                    var GruposDiario = (from n in dbContext.diariodetalle

                                        join J1 in dbContext.asientocontable on new { c = n.v_NroCuenta, eliminado = 0, p = periodo } equals new { c = J1.v_NroCuenta, eliminado = J1.i_Eliminado.Value, p = J1.v_Periodo } into J1_join
                                        from J1 in J1_join.DefaultIfEmpty()

                                        join J2 in dbContext.documento on n.i_IdTipoDocumento.Value equals J2.i_CodigoDocumento into J2_join
                                        from J2 in J2_join.DefaultIfEmpty()

                                        join J3 in dbContext.cliente on n.v_IdCliente equals J3.v_IdCliente into J3_join
                                        from J3 in J3_join.DefaultIfEmpty()

                                        join J4 in dbContext.diario on n.v_IdDiario equals J4.v_IdDiario into J4_join
                                        from J4 in J4_join.DefaultIfEmpty()

                                        join J5 in dbContext.documento on n.i_IdTipoDocumentoRef.Value equals J5.i_CodigoDocumento into J5_join
                                        from J5 in J5_join.DefaultIfEmpty()

                                        join J6 in dbContext.documento on J4.i_IdTipoDocumento.Value equals J6.i_CodigoDocumento into J6_join
                                        from J6 in J6_join.DefaultIfEmpty()

                                        where n.i_Eliminado == 0 && n.t_Fecha.Value.Year == pintPeriodo && n.t_Fecha.Value.Month >= 1 && n.t_Fecha.Value.Month <= pintMes
                                        && n.v_NroCuenta.StartsWith(NroCuenta)
                                        select new
                                        {
                                            CuentaMayor = n.v_NroCuenta.Substring(0, 2),
                                            CuentaImputable = n.v_NroCuenta,
                                            NombreCuenta = J1 != null ? J1.v_NombreCuenta : string.Empty,
                                            FechaDocumento = n.t_Fecha,
                                            DocumentoTransaccion = J6 != null ? J6.v_Siglas + " " + J4.v_Mes.Trim() + J4.v_Correlativo.Trim() : string.Empty,
                                            // DocumentoTransaccion = J4 != null ? J4.i_IdTipoComprobante.Value : 0,
                                            Documento = J2.v_Siglas + " " + n.v_NroDocumento,
                                            DocumentoRef = J5 != null && n.v_NroDocumentoRef != null ? J5.v_Siglas + " " + n.v_NroDocumentoRef : string.Empty,
                                            Analisis = n.v_Analisis == null ? !string.IsNullOrEmpty(J4.v_Nombre) ? J4.v_Nombre : string.Empty : n.v_Analisis,
                                            IdMoneda = J4.i_IdMoneda,
                                            IdMonedaCuenta = J1 != null ? J1.i_IdMoneda : 0,
                                            SiglaMoneda = J4.i_IdMoneda == 1 ? "S" : "D",
                                            Importe = n.d_Importe,
                                            ImporteCambio = n.d_Cambio,
                                            Key = J3.v_IdCliente + n.v_NroCuenta + J2.v_Siglas + n.v_NroDocumento,
                                            NaturalezaCuenta = J1 != null ? J1.i_Naturaleza : 0,
                                            NaturalezaRegistro = n.v_Naturaleza,
                                            CuentaConDetalle = J1.i_Detalle == 1 ? true : false,
                                            TipoCambio = J4.d_TipoCambio,
                                            IdAnexo = J3.v_IdCliente,
                                            Detalle_Ruc = J3 != null ? J3.v_NroDocIdentificacion : string.Empty,
                                            TipoDocumento = n.i_IdTipoDocumento,
                                            TipoDocumentoRef = n.i_IdTipoDocumentoRef,
                                            NroDocumento = n.v_NroDocumento,
                                            NroDocumentoRef = n.v_NroDocumentoRef,
                                            EntidadFinancieraCuenta = J1.i_IdentificaCtaBancos,
                                            TipoMonedaCuenta = J1.i_IdMoneda.Value,
                                            Cliente = J3 != null ? J3.v_NroDocIdentificacion.Trim() + "             " + J3.v_CodCliente.Trim() + "   " + (J3.v_ApePaterno.Trim() + " " + J3.v_ApeMaterno.Trim() + " " + J3.v_PrimerNombre.Trim() + J3.v_RazonSocial.Trim()).Trim() : string.Empty,
                                            FechaComprobante = J4.t_Fecha.Value,
                                            Concepto = (n.v_Analisis == null || n.v_Analisis == string.Empty) ? J4.v_Glosa : n.v_Analisis,
                                            TipoIdentificacion = J3 != null ? J3.i_IdTipoIdentificacion.Value : -1,

                                        }
                                        ).ToList().Select(p => new ReporteLibroInventarioBalanceCajaBanco
                                        {
                                            CuentaMayor = p.CuentaMayor + "   " + NombreCuenta(p.CuentaMayor, ListaAsientoContable),
                                            CuentaImputable = p.CuentaImputable,
                                            NombreCuenta = p.NombreCuenta,
                                            Documento = p.DocumentoTransaccion + " \t" + (p.FechaDocumento.Value.ToShortDateString() + " \t" + p.Documento).Trim(),
                                            DocumentoRef = !string.IsNullOrEmpty(p.DocumentoRef) ? (p.DocumentoRef + " " + p.Analisis).Trim() : string.Empty,
                                            FechaDocumento = p.FechaDocumento,
                                            DocumentoKey = p.Key,
                                            IdMoneda = (int)p.IdMoneda,
                                            IdMonedaCuenta = (int)p.IdMonedaCuenta,
                                            NaturalezaCuenta = p.NaturalezaCuenta == 1 ? "D" : "H",
                                            NaturalezaRegistro = p.NaturalezaRegistro,
                                            ImporteSolesD = p.NaturalezaRegistro == "D" ? p.IdMoneda == 1 ? p.Importe : p.ImporteCambio : 0,
                                            ImporteSolesH = p.NaturalezaRegistro == "H" ? p.IdMoneda == 1 ? p.Importe : p.ImporteCambio : 0,
                                            ImporteDolaresD = p.NaturalezaRegistro == "D" ? p.IdMoneda == 1 ? p.ImporteCambio : p.Importe : 0,
                                            ImporteDolaresH = p.NaturalezaRegistro == "H" ? p.IdMoneda == 1 ? p.ImporteCambio : p.Importe : 0,
                                            CuentaConDetalle = p.CuentaConDetalle,
                                            TipoCambio = p.TipoCambio.Value,
                                            IdAnexo = p.IdAnexo,
                                            // TipoDocumento = p.TipoDocumento != 7 && p.TipoDocumento != 8 ? (int)p.TipoDocumento : (int)p.TipoDocumentoRef,
                                            TipoDocumento = !_objDocumentoBL.DocumentoEsInverso(p.TipoDocumento.Value) && p.TipoDocumento != 8 ? (int)p.TipoDocumento : (int)p.TipoDocumentoRef,
                                            //NroDocumento = p.TipoDocumento != 7 && p.TipoDocumento != 8 ? p.NroDocumento : p.NroDocumentoRef,
                                            NroDocumento = !_objDocumentoBL.DocumentoEsInverso(p.TipoDocumento.Value) && p.TipoDocumento != 8 ? p.NroDocumento : p.NroDocumentoRef,
                                            EntidadFinanceriaCuenta = p.EntidadFinancieraCuenta.ToString(),
                                            TipoMonedaCuenta = p.TipoMonedaCuenta == (int)Currency.Soles ? "01" : "02",
                                            Cliente = p.TipoIdentificacion + "        " + p.Cliente,
                                            FechaComprobante = p.FechaComprobante,
                                            Concepto = p.Concepto,

                                        }).ToList();
                    #endregion


                    return GruposDiario.Concat(GruposTesoreria).ToList();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public List<ReporteLibroInventarioBalanceClientes> ReporteLibroInventarioBalanceClientes(ref OperationResult objOperationResult, int Mes, int Periodo, string NroCuenta, string IdCliente)
        {

            try
            {

                objOperationResult.Success = 1;
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    List<asientocontable> ListaAsientoContable = dbContext.asientocontable.ToList();
                    ReporteLibroInventarioBalanceClientes objReporte = new ReporteLibroInventarioBalanceClientes();
                    List<ReporteLibroInventarioBalanceClientes> ListaReporte = new List<ReporteLibroInventarioBalanceClientes>();
                    AsientosContablesBL _objAsientosContablesBL = new AsientosContablesBL();
                    string pstrCuentaImputable = String.Empty;
                    #region Recolecta las cuentas a procesar
                    DateTime FechaInicio = DateTime.Parse("01/01/" + Periodo.ToString());
                    DateTime FechaFin = DateTime.Parse(DateTime.DaysInMonth(Periodo, Mes).ToString() + "/" + Mes.ToString("00") + "/" + Periodo.ToString() + " 23:59");
                    var RegistrosDiarioTesoreria = _objAsientosContablesBL.ReporteAnalisisCuentasCteAnalitico(ref objOperationResult, FechaInicio, FechaFin, "", "", false, IdCliente, TipoReporteAnalisis.AnalisisCorrientesPendientesAnalitico, 0, null, NroCuenta);
                    var CuentasDiferenciaCambio = RegistrosDiarioTesoreria.Select(x => x.CuentaImputable).Distinct();
                    string NombreCuentaMayor = NombreCuenta(NroCuenta, ListaAsientoContable);
                    #endregion
                    if (RegistrosDiarioTesoreria != null)
                    {
                        foreach (string CuentaDiferenciaCambio in CuentasDiferenciaCambio)
                        {

                            var RegistrosPorCuenta = RegistrosDiarioTesoreria.Where(p => p.CuentaImputable == CuentaDiferenciaCambio).ToList();
                            if (RegistrosPorCuenta.Count > 0)
                            {

                                if (Utils.Windows.CuentaRequiereDetalle(CuentaDiferenciaCambio))
                                {
                                    #region Operacion por anexo, tipo de documento y nro documento
                                    //Las cuentas que tienen detalle se agrupan por Cliente, Documento y NroDOcumento
                                    var RegistrosPorDetalleDocumento = RegistrosPorCuenta.GroupBy(g => new { g.IdAnexo, g.TipoDocumento, g.NroDocumento }).ToList();

                                    //se recorren los valores agrupados, por cada grupo representa una fila en el asiento de diario.
                                    foreach (var Reg in RegistrosPorDetalleDocumento)
                                    {

                                        CodigoError = CodigoError + 1;
                                        if (CodigoError == 24)
                                        {
                                            // string h = "";
                                        }
                                        var RegistroProvicional = Reg.Where(p => p.NaturalezaCuenta == p.NaturalezaRegistro).ToList();
                                        var RegistroCancelacion = Reg.Where(p => p.NaturalezaCuenta != p.NaturalezaRegistro).ToList();


                                        if (RegistroProvicional.Count > 0)
                                        // foreach (var item in RegistroProvicional)
                                        {
                                            decimal ImporteSolesDebe, ImporteSolesHaber, ImporteDolaresDebe, ImporteDolaresHaber;

                                            if (RegistroProvicional.FirstOrDefault().NaturalezaCuenta == "D")
                                            //if(item.NaturalezaCuenta =="D")
                                            {

                                                ImporteSolesDebe = RegistroProvicional.Sum(p => p.ImporteSolesD.Value);
                                                ImporteSolesHaber = RegistroCancelacion.Sum(p => p.ImporteSolesH.Value);
                                                ImporteDolaresDebe = RegistroProvicional.Sum(p => p.ImporteDolaresD.Value);
                                                ImporteDolaresHaber = RegistroCancelacion.Sum(p => p.ImporteDolaresH.Value);


                                            }
                                            else
                                            {

                                                ImporteSolesDebe = RegistroCancelacion.Sum(p => p.ImporteSolesD.Value);
                                                ImporteSolesHaber = RegistroProvicional.Sum(P => P.ImporteSolesH.Value);
                                                ImporteDolaresDebe = RegistroCancelacion.Sum(p => p.ImporteDolaresD.Value);
                                                ImporteDolaresHaber = RegistroProvicional.Sum(p => p.ImporteDolaresH.Value);

                                            }
                                            objReporte = new ReporteLibroInventarioBalanceClientes();
                                            objReporte.CuentaMayor = RegistroProvicional.FirstOrDefault().CuentaMayor + " " + NombreCuentaMayor;
                                            pstrCuentaImputable = RegistroProvicional.FirstOrDefault().CuentaImputable;
                                            objReporte.CuentaImputable = pstrCuentaImputable + " " + NombreCuenta(pstrCuentaImputable, ListaAsientoContable);
                                            objReporte.SoloCuentaImputable = pstrCuentaImputable;
                                            objReporte.Cliente = RegistroProvicional.FirstOrDefault().Detalle.Trim().ToUpper() == "-1" ? "**NO EXISTE CLIENTE**" : RegistroProvicional.FirstOrDefault().Detalle_TipoIdentificacion.Trim().ToUpper() + "        " + RegistroProvicional.FirstOrDefault().Detalle_Ruc + "          " + RegistroProvicional.FirstOrDefault().Detalle.Trim().ToUpper();
                                            objReporte.Documento = RegistroProvicional.FirstOrDefault().Documento;
                                            objReporte.NroRegistro = RegistroProvicional.FirstOrDefault().NroRegistro;
                                            objReporte.Concepto = RegistroProvicional.FirstOrDefault().Analisis;
                                            objReporte.MontoCobrar = ImporteSolesDebe - ImporteSolesHaber;
                                            objReporte.FechaComprobante = RegistroProvicional.FirstOrDefault().FechaDocumento;
                                            objReporte.IdTipoComprobante = RegistroProvicional.FirstOrDefault().i_IdTipoComprobante;
                                            objReporte.TipoDocumentoCliente = RegistroProvicional.FirstOrDefault().Detalle_TipoIdentificacion;
                                            objReporte.NumeroDocumentoCliente = RegistroProvicional.FirstOrDefault().Detalle_Ruc;
                                            objReporte.NombreCliente = RegistroProvicional.FirstOrDefault().NombreCliente;
                                            objReporte.FlagCliente = RegistroProvicional.FirstOrDefault().FlagCliente;
                                            objReporte.TipoDocumentoDetalle = RegistroProvicional.FirstOrDefault().TipoDocumento == -1 ? "00" : RegistroProvicional.FirstOrDefault().TipoDocumento.ToString();
                                            objReporte.NroDocumentoDetalle = RegistroProvicional.FirstOrDefault().DocumentoRaw;
                                            objReporte.FechaDetalle = RegistroProvicional.FirstOrDefault().FechaDetalle;
                                            objReporte.CodigoCliente = RegistroProvicional.FirstOrDefault().CodigoCliente;
                                            objReporte.IdAnexo = RegistroProvicional.FirstOrDefault().IdAnexo;
                                            objReporte.SinDetalle = false;
                                            ListaReporte.Add(objReporte);

                                        }

                                        else if (RegistroCancelacion.Count() > 0)
                                        {

                                            decimal ImporteSolesDebe, ImporteSolesHaber, ImporteDolaresDebe, ImporteDolaresHaber;
                                            if (RegistroCancelacion.FirstOrDefault().NaturalezaCuenta == "D")
                                            {
                                                ImporteSolesDebe = RegistroProvicional.Sum(p => p.ImporteSolesD.Value);
                                                ImporteSolesHaber = RegistroCancelacion.Sum(p => p.ImporteSolesH.Value);
                                                ImporteDolaresDebe = RegistroProvicional.Sum(p => p.ImporteDolaresD.Value);
                                                ImporteDolaresHaber = RegistroCancelacion.Sum(p => p.ImporteDolaresH.Value);
                                            }
                                            else
                                            {

                                                ImporteSolesDebe = RegistroCancelacion.Sum(p => p.ImporteSolesD.Value);
                                                ImporteSolesHaber = RegistroProvicional.Sum(p => p.ImporteSolesH.Value);
                                                ImporteDolaresDebe = RegistroCancelacion.Sum(p => p.ImporteDolaresD.Value);
                                                ImporteDolaresHaber = RegistroProvicional.Sum(p => p.ImporteDolaresH.Value);

                                            }
                                            objReporte = new ReporteLibroInventarioBalanceClientes();
                                            objReporte.CuentaMayor = RegistroCancelacion.FirstOrDefault().CuentaMayor + " " + NombreCuentaMayor;
                                            pstrCuentaImputable = RegistroCancelacion.FirstOrDefault().CuentaImputable;
                                            objReporte.CuentaImputable = pstrCuentaImputable + " " + NombreCuenta(pstrCuentaImputable, ListaAsientoContable);
                                            objReporte.SoloCuentaImputable = pstrCuentaImputable;
                                            objReporte.Cliente = RegistroCancelacion.FirstOrDefault().Detalle.Trim().ToUpper() == "-1" ? "**NO EXISTE CLIENTE**" : RegistroCancelacion.FirstOrDefault().Detalle_TipoIdentificacion.Trim().ToUpper() + "        " + RegistroCancelacion.FirstOrDefault().Detalle_Ruc + "          " + RegistroCancelacion.FirstOrDefault().Detalle.Trim().ToUpper();
                                            objReporte.Documento = RegistroCancelacion.FirstOrDefault().Documento;
                                            objReporte.NroRegistro = RegistroCancelacion.FirstOrDefault().NroRegistro;
                                            objReporte.Concepto = RegistroCancelacion.FirstOrDefault().Analisis;
                                            objReporte.MontoCobrar = ImporteSolesDebe - ImporteSolesHaber;
                                            objReporte.FechaComprobante = RegistroCancelacion.FirstOrDefault().FechaDocumento;
                                            objReporte.SinDetalle = false;
                                            objReporte.IdTipoComprobante = RegistroCancelacion.FirstOrDefault().i_IdTipoComprobante;
                                            objReporte.TipoDocumentoCliente = RegistroCancelacion.FirstOrDefault().Detalle_TipoIdentificacion;
                                            objReporte.NumeroDocumentoCliente = RegistroCancelacion.FirstOrDefault().Detalle_Ruc;
                                            objReporte.NombreCliente = RegistroCancelacion.FirstOrDefault().NombreCliente;
                                            objReporte.FlagCliente = RegistroCancelacion.FirstOrDefault().FlagCliente;
                                            objReporte.TipoDocumentoDetalle = RegistroCancelacion.FirstOrDefault().TipoDocumento == -1 ? "00" : RegistroCancelacion.FirstOrDefault().TipoDocumento.ToString();
                                            objReporte.NroDocumentoDetalle = RegistroCancelacion.FirstOrDefault().DocumentoRaw;
                                            objReporte.FechaDetalle = RegistroCancelacion.FirstOrDefault().FechaDetalle;
                                            objReporte.CodigoCliente = RegistroCancelacion.FirstOrDefault().CodigoCliente;
                                            objReporte.IdAnexo = RegistroCancelacion.FirstOrDefault().IdAnexo;
                                            ListaReporte.Add(objReporte);

                                        }
                                    }
                                    #endregion
                                }
                                else
                                {

                                    #region Operacion de cuentas sin detalle

                                    var RegistroProvicional = RegistrosPorCuenta.Where(p => p.NaturalezaCuenta == p.NaturalezaRegistro).ToList();
                                    var RegistroCancelacion = RegistrosPorCuenta.Where(p => p.NaturalezaCuenta != p.NaturalezaRegistro).ToList();
                                    if (RegistroProvicional.Count > 0)
                                    {
                                        decimal ImporteSolesDebe, ImporteSolesHaber, ImporteDolaresDebe, ImporteDolaresHaber;


                                        if (RegistroProvicional.FirstOrDefault().NaturalezaCuenta == "D")
                                        {

                                            ImporteSolesDebe = RegistroProvicional.Sum(p => p.ImporteSolesD.Value);
                                            ImporteSolesHaber = RegistroCancelacion.Sum(p => p.ImporteSolesH.Value);
                                            ImporteDolaresDebe = RegistroProvicional.Sum(p => p.ImporteDolaresD.Value);
                                            ImporteDolaresHaber = RegistroCancelacion.Sum(p => p.ImporteDolaresH.Value);

                                        }
                                        else
                                        {

                                            ImporteSolesDebe = RegistroCancelacion.Sum(p => p.ImporteSolesD.Value);
                                            ImporteSolesHaber = RegistroProvicional.Sum(p => p.ImporteSolesH.Value);
                                            ImporteDolaresDebe = RegistroCancelacion.Sum(p => p.ImporteDolaresD.Value);
                                            ImporteDolaresHaber = RegistroProvicional.Sum(p => p.ImporteDolaresH.Value);

                                        }


                                        objReporte = new ReporteLibroInventarioBalanceClientes();
                                        objReporte.CuentaMayor = RegistroProvicional.FirstOrDefault().CuentaMayor + " " + NombreCuentaMayor;

                                        pstrCuentaImputable = RegistroProvicional.FirstOrDefault().CuentaImputable;
                                        objReporte.CuentaImputable = pstrCuentaImputable + " " + NombreCuenta(pstrCuentaImputable, ListaAsientoContable);
                                        objReporte.SoloCuentaImputable = pstrCuentaImputable;
                                        objReporte.Cliente = !String.IsNullOrEmpty(IdCliente) ? RegistroProvicional.FirstOrDefault().Detalle.Trim().ToUpper() == "-1" ? "**NO EXISTE CLIENTE**" : RegistroProvicional.FirstOrDefault().Detalle_TipoIdentificacion.Trim().ToUpper() + "        " + RegistroProvicional.FirstOrDefault().Detalle_Ruc + "          " + RegistroProvicional.FirstOrDefault().Detalle.Trim().ToUpper() : "";
                                        objReporte.Documento = string.Empty;
                                        objReporte.Concepto = string.Empty;
                                        objReporte.NroRegistro = RegistroProvicional.FirstOrDefault().NroRegistro;
                                        objReporte.MontoCobrar = ImporteSolesDebe - ImporteSolesHaber;
                                        objReporte.FechaComprobante = DateTime.MinValue;
                                        objReporte.SinDetalle = true;
                                        objReporte.IdTipoComprobante = RegistroProvicional.FirstOrDefault().i_IdTipoComprobante;
                                        objReporte.TipoDocumentoCliente = RegistroProvicional.FirstOrDefault().Detalle_TipoIdentificacion;
                                        objReporte.NumeroDocumentoCliente = RegistroProvicional.FirstOrDefault().Detalle_Ruc;
                                        objReporte.NombreCliente = RegistroProvicional.FirstOrDefault().NombreCliente;
                                        objReporte.FlagCliente = RegistroProvicional.FirstOrDefault().FlagCliente;
                                        objReporte.TipoDocumentoDetalle = RegistroProvicional.FirstOrDefault().TipoDocumento == -1 ? "00" : RegistroProvicional.FirstOrDefault().TipoDocumento.ToString("00");
                                        objReporte.NroDocumentoDetalle = RegistroProvicional.FirstOrDefault().DocumentoRaw;
                                        objReporte.FechaDetalle = RegistroProvicional.FirstOrDefault().FechaDetalle;
                                        objReporte.CodigoCliente = RegistroProvicional.FirstOrDefault().CodigoCliente;
                                        objReporte.IdAnexo = RegistroProvicional.FirstOrDefault().IdAnexo;
                                        ListaReporte.Add(objReporte);
                                    }

                                    else if (RegistroCancelacion.Count() > 0)
                                    {
                                        decimal ImporteSolesDebe, ImporteSolesHaber, ImporteDolaresDebe, ImporteDolaresHaber;

                                        if (RegistroCancelacion.FirstOrDefault().NaturalezaCuenta == "D")
                                        {
                                            ImporteSolesDebe = RegistroProvicional.Sum(p => p.ImporteSolesD.Value);
                                            ImporteSolesHaber = RegistroCancelacion.Sum(p => p.ImporteSolesH.Value);
                                            ImporteDolaresDebe = RegistroProvicional.Sum(p => p.ImporteDolaresD.Value);
                                            ImporteDolaresHaber = RegistroCancelacion.Sum(p => p.ImporteDolaresH.Value);
                                        }
                                        else
                                        {
                                            ImporteSolesDebe = RegistroCancelacion.Sum(p => p.ImporteSolesD.Value);
                                            ImporteSolesHaber = RegistroProvicional.Sum(p => p.ImporteSolesH.Value);
                                            ImporteDolaresDebe = RegistroCancelacion.Sum(p => p.ImporteDolaresD.Value);
                                            ImporteDolaresHaber = RegistroProvicional.Sum(p => p.ImporteDolaresH.Value);
                                        }
                                        objReporte = new ReporteLibroInventarioBalanceClientes();
                                        objReporte.CuentaMayor = RegistroCancelacion.FirstOrDefault().CuentaMayor + " " + NombreCuentaMayor;
                                        pstrCuentaImputable = RegistroCancelacion.FirstOrDefault().CuentaImputable;
                                        objReporte.CuentaImputable = pstrCuentaImputable + " " + NombreCuenta(pstrCuentaImputable, ListaAsientoContable);
                                        objReporte.SoloCuentaImputable = pstrCuentaImputable;
                                        objReporte.Cliente = !string.IsNullOrEmpty(IdCliente) ? RegistroCancelacion.FirstOrDefault().Detalle.Trim().ToUpper() == "-1" ? "**NO EXISTE CLIENTE**" : RegistroCancelacion.FirstOrDefault().Detalle_TipoIdentificacion.Trim().ToUpper() + "        " + RegistroCancelacion.FirstOrDefault().Detalle_Ruc + "          " + RegistroCancelacion.FirstOrDefault().Detalle.Trim().ToUpper() : "";
                                        objReporte.Documento = string.Empty;
                                        objReporte.NroRegistro = RegistroCancelacion.FirstOrDefault().NroRegistro;
                                        objReporte.Concepto = string.Empty;
                                        objReporte.MontoCobrar = ImporteSolesDebe - ImporteSolesHaber;
                                        objReporte.FechaComprobante = DateTime.MinValue;
                                        objReporte.SinDetalle = true;
                                        objReporte.IdTipoComprobante = RegistroCancelacion.FirstOrDefault().i_IdTipoComprobante;
                                        objReporte.TipoDocumentoCliente = RegistroCancelacion.FirstOrDefault().Detalle_TipoIdentificacion;
                                        objReporte.NumeroDocumentoCliente = RegistroCancelacion.FirstOrDefault().Detalle_Ruc;
                                        objReporte.NombreCliente = RegistroCancelacion.FirstOrDefault().NombreCliente;
                                        objReporte.FlagCliente = RegistroCancelacion.FirstOrDefault().FlagCliente;
                                        objReporte.TipoDocumentoDetalle = RegistroCancelacion.FirstOrDefault().TipoDocumento == -1 ? "00" : RegistroCancelacion.FirstOrDefault().TipoDocumento.ToString("00");
                                        objReporte.NroDocumentoDetalle = RegistroCancelacion.FirstOrDefault().DocumentoRaw;
                                        objReporte.FechaDetalle = RegistroCancelacion.FirstOrDefault().FechaDetalle;
                                        objReporte.CodigoCliente = RegistroCancelacion.FirstOrDefault().CodigoCliente;
                                        objReporte.IdAnexo = RegistroCancelacion.FirstOrDefault().IdAnexo;
                                        ListaReporte.Add(objReporte);

                                    }

                                    #endregion

                                }
                            }
                        }
                    }



                    ListaReporte = ListaReporte.Where(x => x.MontoCobrar != 0).OrderBy(x => x.CuentaImputable).ToList();
                    return ListaReporte = ListaReporte.GroupBy(o => new { CuentaMayor = o.CuentaMayor, CuentaImp = o.CuentaImputable, Cliente = o.IdAnexo, DocumentoDet = o.TipoDocumentoDetalle + " " + o.NroDocumentoDetalle, o.FechaComprobante }).Select(o =>
                              {
                                  ReporteLibroInventarioBalanceClientes obj = new ReporteLibroInventarioBalanceClientes();
                                  obj = o.LastOrDefault();
                                  obj.MontoCobrar = o.Sum(p => p.MontoCobrar);
                                  return obj;

                              }).ToList().Where(x => x.MontoCobrar != 0).OrderBy(x => x.CuentaImputable).ToList();
                }

            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "ContabilidadBL.ReporteLibroInventarioBalanceClientes ,Error en :" + CodigoError.ToString();
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }
        }

        //private List<ReporteLibroInventarioBalanceClientes> DevuelveAsientosParaAjusteDiferenciaCambioLibroInventarioClientes(int pintMes, int pintPeriodo, string NroCuenta, string pstrCodigoCliente)
        //{
        //    try
        //    {
        //        using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
        //        {
        //            #region Recopila Cuentas de Tesorería
        //            var GruposTesoreria = (from n in dbContext.tesoreriadetalle

        //                                   join J1 in dbContext.asientocontable on new { c = n.v_NroCuenta.Trim(), eliminado = 0, p = periodo } equals new { c = J1.v_NroCuenta.Trim(), eliminado = J1.i_Eliminado.Value, p = J1.v_Periodo } into J1_join
        //                                   from J1 in J1_join.DefaultIfEmpty()

        //                                   join J2 in dbContext.documento on n.i_IdTipoDocumento.Value equals J2.i_CodigoDocumento into J2_join
        //                                   from J2 in J2_join.DefaultIfEmpty()

        //                                   join J3 in dbContext.cliente on n.v_IdCliente equals J3.v_IdCliente into J3_join
        //                                   from J3 in J3_join.DefaultIfEmpty()

        //                                   join J4 in dbContext.tesoreria on new { IdTesoreria = n.v_IdTesoreria, eliminado = 0 } equals new { IdTesoreria = J4.v_IdTesoreria, eliminado = J4.i_Eliminado.Value } into J4_join
        //                                   from J4 in J4_join.DefaultIfEmpty()

        //                                   join J5 in dbContext.documento on n.i_IdTipoDocumentoRef.Value equals J5.i_CodigoDocumento into J5_join
        //                                   from J5 in J5_join.DefaultIfEmpty()

        //                                   join J6 in dbContext.documento on J4.i_IdTipoDocumento.Value equals J6.i_CodigoDocumento into J6_join
        //                                   from J6 in J6_join.DefaultIfEmpty()



        //                                   where n.i_Eliminado == 0 && n.t_Fecha.Value.Month >= 1 && n.t_Fecha.Value.Month <= pintMes && n.t_Fecha.Value.Year == pintPeriodo
        //                                   && n.v_NroCuenta.StartsWith(NroCuenta) && (J3.v_CodCliente == pstrCodigoCliente || pstrCodigoCliente == string.Empty)

        //                                   && J4.i_IdEstado == 1
        //                                   select new
        //                                   {
        //                                       CuentaMayor = n.v_NroCuenta.Substring(0, 2),
        //                                       CuentaImputable = n.v_NroCuenta,
        //                                       NombreCuenta = J1 != null ? J1.v_NombreCuenta : string.Empty,
        //                                       FechaDocumento = n.t_Fecha,
        //                                       DocumentoTransaccion = J6 != null ? J6.v_Siglas + " " + J4.v_Mes.Trim() + J4.v_Correlativo.Trim() : string.Empty,
        //                                       Documento = J2.v_Siglas + " " + n.v_NroDocumento,
        //                                       DocumentoRef = J5 != null ? J5.v_Siglas + " " + n.v_NroDocumentoRef : string.Empty,
        //                                       Analisis = n.v_Analisis == null ? !string.IsNullOrEmpty(J4.v_Nombre) ? J4.v_Nombre : string.Empty : n.v_Analisis,
        //                                       IdMoneda = J4.i_IdMoneda,
        //                                       SiglaMoneda = J4.i_IdMoneda == 1 ? "S" : "D",
        //                                       Importe = n.d_Importe,
        //                                       ImporteCambio = n.d_Cambio,
        //                                       Key = J3.v_IdCliente + n.v_NroCuenta + J2.v_Siglas + n.v_NroDocumento,
        //                                       NaturalezaCuenta = J1 != null ? J1.i_Naturaleza : 0,
        //                                       IdMonedaCuenta = J1 != null ? J1.i_IdMoneda : 0,
        //                                       NaturalezaRegistro = n.v_Naturaleza,
        //                                       CuentaConDetalle = J1.i_Detalle == 1 ? true : false,
        //                                       TipoCambio = J4.d_TipoCambio,
        //                                       IdAnexo = J3.v_IdCliente,
        //                                       Detalle_Ruc = J3 != null ? J3.v_NroDocIdentificacion : string.Empty,
        //                                       TipoDocumento = n.i_IdTipoDocumento,
        //                                       TipoDocumentoRef = n.i_IdTipoDocumentoRef,
        //                                       NroDocumento = n.v_NroDocumento,
        //                                       NroDocumentoRef = n.v_NroDocumentoRef,
        //                                       EntidadFinancieraCuenta = J1.i_IdentificaCtaBancos,
        //                                       TipoMonedaCuenta = J1.i_IdMoneda.Value,
        //                                       Cliente = J3 != null ? J3.v_NroDocIdentificacion.Trim() + "             " + J3.v_CodCliente.Trim() + "   " + (J3.v_ApePaterno.Trim() + " " + J3.v_ApeMaterno.Trim() + " " + J3.v_PrimerNombre.Trim() + J3.v_RazonSocial.Trim()).Trim() : string.Empty,
        //                                       FechaComprobante = J4.t_FechaRegistro.Value,
        //                                       Concepto = (n.v_Analisis == null || n.v_Analisis == string.Empty) ? J4.v_Glosa : n.v_Analisis,
        //                                       TipoIdentificacion = J3 != null ? J3.i_IdTipoIdentificacion.Value : -1,

        //                                   }
        //                                    ).ToList().Select(p => new ReporteLibroInventarioBalanceClientes
        //                                    {
        //                                        CuentaMayor = p.CuentaMayor + "   " + NombreCuenta(p.CuentaMayor),
        //                                        CuentaImputable = p.CuentaImputable,
        //                                        NombreCuenta = p.NombreCuenta,
        //                                        Documento = p.DocumentoTransaccion + " \t" + (p.FechaDocumento.Value.ToShortDateString() + " \t" + p.Documento).Trim() + " " + p.DocumentoRef,
        //                                        DocumentoRef = !string.IsNullOrEmpty(p.DocumentoRef) ? (p.DocumentoRef + " " + p.Analisis).Trim() : string.Empty,
        //                                        FechaDocumento = p.FechaDocumento.Value,
        //                                        DocumentoKey = p.Key,
        //                                        IdMoneda = (int)p.IdMoneda,
        //                                        IdMonedaCuenta = (int)p.IdMonedaCuenta,
        //                                        NaturalezaCuenta = p.NaturalezaCuenta == 1 ? "D" : "H",
        //                                        NaturalezaRegistro = p.NaturalezaRegistro,
        //                                        ImporteSolesD = p.NaturalezaRegistro == "D" ? p.IdMoneda.Value == 1 ? p.Importe.Value : p.ImporteCambio.Value : 0,
        //                                        ImporteSolesH = p.NaturalezaRegistro == "H" ? p.IdMoneda == 1 ? p.Importe : p.ImporteCambio : 0,
        //                                        ImporteDolaresD = p.NaturalezaRegistro == "D" ? p.IdMoneda == 1 ? p.ImporteCambio : p.Importe : 0,
        //                                        ImporteDolaresH = p.NaturalezaRegistro == "H" ? p.IdMoneda == 1 ? p.ImporteCambio : p.Importe : 0,
        //                                        CuentaConDetalle = p.CuentaConDetalle,
        //                                        TipoCambio = p.TipoCambio.Value,
        //                                        IdAnexo = p.IdAnexo,
        //                                        //TipoDocumento = p.TipoDocumento != 7 && p.TipoDocumento != 8 ? (int)p.TipoDocumento : (int)p.TipoDocumentoRef,
        //                                        TipoDocumento = !_objDocumentoBL.DocumentoEsInverso(p.TipoDocumento.Value) && p.TipoDocumento != 8 ? (int)p.TipoDocumento : (int)p.TipoDocumentoRef,
        //                                        //NroDocumento = p.TipoDocumento != 7 && p.TipoDocumento != 8 ? p.NroDocumento : p.NroDocumentoRef,
        //                                        NroDocumento = !_objDocumentoBL.DocumentoEsInverso(p.TipoDocumento.Value) && p.TipoDocumento != 8 ? p.NroDocumento : p.NroDocumentoRef,
        //                                        EntidadFinanceriaCuenta = p.EntidadFinancieraCuenta.ToString(),
        //                                        TipoMonedaCuenta = p.TipoMonedaCuenta == (int)Currency.Soles ? "01" : "02",
        //                                        Cliente = p.TipoIdentificacion + "        " + p.Cliente,
        //                                        FechaComprobante = p.FechaComprobante,
        //                                        Concepto = p.Concepto,


        //                                    }
        //                                    ).ToList();
        //            #endregion

        //            #region Recopila Cuentas de Diario
        //            var GruposDiario = (from n in dbContext.diariodetalle

        //                                join J1 in dbContext.asientocontable on new { c = n.v_NroCuenta, eliminado = 0, p = periodo } equals new { c = J1.v_NroCuenta, eliminado = J1.i_Eliminado.Value, p = J1.v_Periodo } into J1_join
        //                                from J1 in J1_join.DefaultIfEmpty()

        //                                join J2 in dbContext.documento on new { td = n.i_IdTipoDocumento.Value } equals new { td = J2.i_CodigoDocumento } into J2_join
        //                                from J2 in J2_join.DefaultIfEmpty()

        //                                join J3 in dbContext.cliente on n.v_IdCliente equals J3.v_IdCliente into J3_join
        //                                from J3 in J3_join.DefaultIfEmpty()

        //                                join J4 in dbContext.diario on n.v_IdDiario equals J4.v_IdDiario into J4_join
        //                                from J4 in J4_join.DefaultIfEmpty()

        //                                join J5 in dbContext.documento on n.i_IdTipoDocumentoRef.Value equals J5.i_CodigoDocumento into J5_join
        //                                from J5 in J5_join.DefaultIfEmpty()

        //                                join J6 in dbContext.documento on J4.i_IdTipoDocumento.Value equals J6.i_CodigoDocumento into J6_join
        //                                from J6 in J6_join.DefaultIfEmpty()

        //                                where n.i_Eliminado == 0 && n.t_Fecha.Value.Year == pintPeriodo && n.t_Fecha.Value.Month >= 1 && n.t_Fecha.Value.Month <= pintMes
        //                                && n.v_NroCuenta.StartsWith(NroCuenta) && (J3.v_CodCliente == pstrCodigoCliente || pstrCodigoCliente == string.Empty)
        //                                select new
        //                                {
        //                                    CuentaMayor = n.v_NroCuenta.Substring(0, 2),
        //                                    CuentaImputable = n.v_NroCuenta,
        //                                    NombreCuenta = J1 != null ? J1.v_NombreCuenta : string.Empty,
        //                                    FechaDocumento = n.t_Fecha,
        //                                    DocumentoTransaccion = J6 != null ? J6.v_Siglas + " " + J4.v_Mes.Trim() + J4.v_Correlativo.Trim() : string.Empty,

        //                                    Documento = J2.v_Siglas + " " + n.v_NroDocumento,
        //                                    DocumentoRef = J5 != null && n.v_NroDocumentoRef != null ? J5.v_Siglas + " " + n.v_NroDocumentoRef : string.Empty,
        //                                    Analisis = n.v_Analisis == null ? !string.IsNullOrEmpty(J4.v_Nombre) ? J4.v_Nombre : string.Empty : n.v_Analisis,
        //                                    IdMoneda = J4.i_IdMoneda,
        //                                    IdMonedaCuenta = J1 != null ? J1.i_IdMoneda : 0,
        //                                    SiglaMoneda = J4.i_IdMoneda == 1 ? "S" : "D",
        //                                    Importe = n.d_Importe,
        //                                    ImporteCambio = n.d_Cambio,
        //                                    Key = J3.v_IdCliente + n.v_NroCuenta + J2.v_Siglas + n.v_NroDocumento,
        //                                    NaturalezaCuenta = J1 != null ? J1.i_Naturaleza : 0,
        //                                    NaturalezaRegistro = n.v_Naturaleza,
        //                                    CuentaConDetalle = J1.i_Detalle == 1 ? true : false,
        //                                    TipoCambio = J4.d_TipoCambio,
        //                                    IdAnexo = J3.v_IdCliente,
        //                                    Detalle_Ruc = J3 != null ? J3.v_NroDocIdentificacion : string.Empty,
        //                                    TipoDocumento = n.i_IdTipoDocumento,
        //                                    TipoDocumentoRef = n.i_IdTipoDocumentoRef,
        //                                    NroDocumento = n.v_NroDocumento,
        //                                    NroDocumentoRef = n.v_NroDocumentoRef,
        //                                    EntidadFinancieraCuenta = J1.i_IdentificaCtaBancos,
        //                                    TipoMonedaCuenta = J1.i_IdMoneda.Value,
        //                                    Cliente = J3 != null ? J3.v_NroDocIdentificacion.Trim() + "             " + J3.v_CodCliente.Trim() + "   " + (J3.v_ApePaterno.Trim() + " " + J3.v_ApeMaterno.Trim() + " " + J3.v_PrimerNombre.Trim() + J3.v_RazonSocial.Trim()).Trim() : string.Empty,
        //                                    FechaComprobante = J4.t_Fecha.Value,
        //                                    Concepto = (n.v_Analisis == null || n.v_Analisis == string.Empty) ? J4.v_Glosa : n.v_Analisis,
        //                                    TipoIdentificacion = J3 != null ? J3.i_IdTipoIdentificacion.Value : -1,

        //                                }
        //                                ).ToList().Select(p => new ReporteLibroInventarioBalanceClientes
        //                                {
        //                                    CuentaMayor = p.CuentaMayor + "   " + NombreCuenta(p.CuentaMayor),
        //                                    CuentaImputable = p.CuentaImputable,
        //                                    NombreCuenta = p.NombreCuenta,
        //                                    Documento = p.DocumentoTransaccion + " \t" + (p.FechaDocumento.Value.ToShortDateString() + " \t" + p.Documento).Trim() + " " + p.DocumentoRef,
        //                                    DocumentoRef = !string.IsNullOrEmpty(p.DocumentoRef) ? (p.DocumentoRef + " " + p.Analisis).Trim() : string.Empty,
        //                                    FechaDocumento = p.FechaDocumento,
        //                                    DocumentoKey = p.Key,
        //                                    IdMoneda = (int)p.IdMoneda,
        //                                    IdMonedaCuenta = (int)p.IdMonedaCuenta,
        //                                    NaturalezaCuenta = p.NaturalezaCuenta == 1 ? "D" : "H",
        //                                    NaturalezaRegistro = p.NaturalezaRegistro,
        //                                    ImporteSolesD = p.NaturalezaRegistro == "D" ? p.IdMoneda == 1 ? p.Importe : p.ImporteCambio : 0,
        //                                    ImporteSolesH = p.NaturalezaRegistro == "H" ? p.IdMoneda == 1 ? p.Importe : p.ImporteCambio : 0,
        //                                    ImporteDolaresD = p.NaturalezaRegistro == "D" ? p.IdMoneda == 1 ? p.ImporteCambio : p.Importe : 0,
        //                                    ImporteDolaresH = p.NaturalezaRegistro == "H" ? p.IdMoneda == 1 ? p.ImporteCambio : p.Importe : 0,
        //                                    CuentaConDetalle = p.CuentaConDetalle,
        //                                    TipoCambio = p.TipoCambio.Value,
        //                                    IdAnexo = p.IdAnexo,
        //                                    //TipoDocumento = p.TipoDocumento != 7 && p.TipoDocumento != 8 ? (int)p.TipoDocumento : (int)p.TipoDocumentoRef,
        //                                    TipoDocumento = !_objDocumentoBL.DocumentoEsInverso(p.TipoDocumento.Value) && p.TipoDocumento != 8 ? (int)p.TipoDocumento : (int)p.TipoDocumentoRef,
        //                                    //NroDocumento = p.TipoDocumento != 7 && p.TipoDocumento != 8 ? p.NroDocumento : p.NroDocumentoRef,
        //                                    NroDocumento = !_objDocumentoBL.DocumentoEsInverso(p.TipoDocumento.Value) && p.TipoDocumento != 8 ? p.NroDocumento : p.NroDocumentoRef,
        //                                    EntidadFinanceriaCuenta = p.EntidadFinancieraCuenta.ToString(),
        //                                    TipoMonedaCuenta = p.TipoMonedaCuenta == (int)Currency.Soles ? "01" : "02",
        //                                    Cliente = p.TipoIdentificacion + "        " + p.Cliente,
        //                                    FechaComprobante = p.FechaComprobante,
        //                                    Concepto = p.Concepto,

        //                                }).ToList();
        //            #endregion


        //            return GruposDiario.Concat(GruposTesoreria).ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }

        //}

        public void ValidarMaxSerieCorrelativoDocumentos(int TipoDoc, string Serie, string Correlativo)
        {






        }
        //public List<DocumentosSerieCorrelativo> ListaDocumentosSerieCorrelativo()
        //{

        //    List<DocumentosSerieCorrelativo> ListaDocumentos = new List<DocumentosSerieCorrelativo> ();
        //    DocumentosSerieCorrelativo objDocumento = new DocumentosSerieCorrelativo();
        //       objDocumento.TipoDocumento = (int)TiposDocumentos.Otros;
        //    objDocumento.MaxSerie =20;
        //    objDocumento.MaxCorrelativo =20;
        //    ListaDocumentos.Add (objDocumento);
        //    objDocumento = new DocumentosSerieCorrelativo();
        //    objDocumento.TipoDocumento =(int) TiposDocumentos.Factura;
        //    objDocumento.MaxSerie = 4;

        //    objDocumento.MaxCorrelativo = 8;
        //    ListaDocumentos.Add(objDocumento);


        //    objDocumento.TipoDocumento = (int)TiposDocumentos.ReciboHonorario;
        //    objDocumento.TipoDocumento = (int)TiposDocumentos.BoletaVenta;
        //    objDocumento.TipoDocumento = (int)TiposDocumentos.LiquidacionCompra;
        //    objDocumento.TipoDocumento = (int)TiposDocumentos.NotaCredito;
        //    objDocumento.TipoDocumento = (int)TiposDocumentos.BME;





        //}
        #endregion

        #region ASIENTOS

        public List<NroDiario> GenerarAsientoCierre(ref OperationResult objOperatioResult, int TipoDocumento, string pAnalisis, string pCuentaUtilidad, string pCuentaPerdida, string pCuentaResult891, string pCuentaResult892, string pPeriodo, string MesDiario)
        {

            try
            {

                // Acreedora==>Naturaleza = 2;
                //Deudora==> 1;
                List<KeyValueDTO> _ListadoDiarios = new List<KeyValueDTO>();
                DiarioBL _objDiarioBL = new DiarioBL();
                OperationResult objOperationResult = new OperationResult();
                diarioDto _diarioDto = new diarioDto();
                diariodetalleDto _diarioDetalleDto = new diariodetalleDto();
                diariodetalleDto _diarioDetalleDto2 = new diariodetalleDto();
                List<diariodetalleDto> ListaDetalles = new List<diariodetalleDto>();
                List<NroDiario> ListaDiarioGenerados = new List<NroDiario>();
                NroDiario objNroDiario = new NroDiario();
                decimal DebeImporte = 0, HaberImporte = 0;
                decimal DebeCambio = 0, HaberCambio = 0;

                decimal TotDebe, TotHaber, TotDebeC, TotHaberC;
                int _MaxMovimiento;
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {

                        Globals.ProgressbarStatus.i_TotalProgress = 12;
                        string[] IdRegistroEliminadoAsiento1 = new string[3];
                        string[] IdRegistroEliminadoAsiento2 = new string[3];
                        string[] IdRegistroEliminadoAsiento4 = new string[3];
                        string[] IdRegistroEliminadoAsiento3 = new string[3];
                        string[] IdRegistroEliminadoAsiento5 = new string[3];
                        string[] IdRegistroEliminadoAsiento6 = new string[3];

                        IdRegistroEliminadoAsiento1 = _objDiarioBL.EliminarDiarioCierreApertura(ref objOperationResult, "AC-1-", Globals.ClientSession.GetAsList(), false, pPeriodo);
                        Globals.ProgressbarStatus.i_Progress++;
                        IdRegistroEliminadoAsiento2 = _objDiarioBL.EliminarDiarioCierreApertura(ref objOperationResult, "AC-2-", Globals.ClientSession.GetAsList(), false, pPeriodo);
                        Globals.ProgressbarStatus.i_Progress++;
                        IdRegistroEliminadoAsiento3 = _objDiarioBL.EliminarDiarioCierreApertura(ref objOperationResult, "AC-3-", Globals.ClientSession.GetAsList(), false, pPeriodo);
                        Globals.ProgressbarStatus.i_Progress++;
                        IdRegistroEliminadoAsiento4 = _objDiarioBL.EliminarDiarioCierreApertura(ref objOperationResult, "AC-4-", Globals.ClientSession.GetAsList(), false, pPeriodo);
                        Globals.ProgressbarStatus.i_Progress++;
                        IdRegistroEliminadoAsiento5 = _objDiarioBL.EliminarDiarioCierreApertura(ref objOperationResult, "AC-5-", Globals.ClientSession.GetAsList(), false, pPeriodo);
                        Globals.ProgressbarStatus.i_Progress++;
                        IdRegistroEliminadoAsiento6 = _objDiarioBL.EliminarDiarioCierreApertura(ref objOperationResult, "AC-6-", Globals.ClientSession.GetAsList(), false, pPeriodo);
                        Globals.ProgressbarStatus.i_Progress++;
                        var saldoscontables = new AsientosContablesBL().ObtenerSaldosContablesPorMes(ref  objOperationResult, 12, Globals.ClientSession.i_Periodo.Value);
                        var Saldos = (from a in saldoscontables
                                      join b in dbContext.asientocontable on new { cuenta = a.v_NroCuenta, eliminado = 0, per = periodo } equals new { cuenta = b.v_NroCuenta, eliminado = b.i_Eliminado.Value, per = b.v_Periodo } into b_join
                                      from b in b_join.DefaultIfEmpty()
                                      select new saldoscontablesDto
                                      {

                                          v_NroCuenta = a.v_NroCuenta,
                                          d_ImporteSolesD = a.d_ImporteSolesD,
                                          d_ImporteDolaresD = a.d_ImporteDolaresD,
                                          d_ImporteSolesH = a.d_ImporteSolesH,
                                          d_ImporteDolaresH = a.d_ImporteDolaresH,
                                          Naturaleza = b.i_Naturaleza == 1 ? "D" : "H",
                                          CuentaMayor = a.v_NroCuenta.Substring(0, 2),


                                      }).ToList();
                        #region Asiento1
                        DebeImporte = 0; HaberImporte = 0;
                        DebeCambio = 0; HaberCambio = 0;
                        var CuentasAsiento1 = Saldos.Where(x => x.v_NroCuenta.StartsWith("9") || x.v_NroCuenta.StartsWith("79")).ToList();
                        _ListadoDiarios = _objDiarioBL.ObtenerListadoDiarioParaAsientoCierre(ref objOperationResult, pPeriodo, MesDiario, TipoDocumento);
                        _MaxMovimiento = _ListadoDiarios.Count() > 0 ? int.Parse(_ListadoDiarios[_ListadoDiarios.Count() - 1].Value1.ToString()) : 0;
                        _MaxMovimiento++;
                        if (IdRegistroEliminadoAsiento1[0] != null)
                        {

                            _diarioDto.v_Periodo = IdRegistroEliminadoAsiento1[0];
                            _diarioDto.v_Mes = IdRegistroEliminadoAsiento1[1];
                            _diarioDto.v_Correlativo = IdRegistroEliminadoAsiento1[2];
                        }
                        else
                        {
                            _diarioDto.v_Periodo = pPeriodo.ToString();
                            _diarioDto.v_Mes = MesDiario.ToString();
                            _diarioDto.v_Correlativo = _MaxMovimiento.ToString("00000000");
                        }
                        _diarioDto.v_IdDocumentoReferencia = "AC-1-" + pPeriodo;
                        _diarioDto.v_Nombre = "ASIENTO CIERRE 1 : CANCELACIÓN CUENTA 9";
                        _diarioDto.v_Glosa = "ASIENTO CIERRE 1 : CANCELACIÓN CUENTA 9";
                        _diarioDto.d_TipoCambio = decimal.Parse("1");
                        _diarioDto.i_IdMoneda = (int)Currency.Soles;
                        _diarioDto.i_IdTipoDocumento = TipoDocumento;
                        _diarioDto.t_Fecha = DateTime.Parse("31/12/" + pPeriodo);
                        _diarioDto.i_IdTipoComprobante = 5;

                        var CuentasAgrupadasAsiento1 = CuentasAsiento1.GroupBy(x => x.v_NroCuenta);
                        foreach (var CuentaAsiento1 in CuentasAgrupadasAsiento1)
                        {

                            _diarioDetalleDto = new diariodetalleDto();
                            _diarioDetalleDto.v_NroCuenta = CuentaAsiento1.FirstOrDefault().v_NroCuenta;


                            if (CuentaAsiento1.Sum(x => x.d_ImporteSolesD.Value) < CuentaAsiento1.Sum(x => x.d_ImporteSolesH.Value))
                            {
                                _diarioDetalleDto.v_Naturaleza = "D";
                                _diarioDetalleDto.d_Importe = CuentaAsiento1.Sum(x => x.d_ImporteSolesD.Value) < CuentaAsiento1.Sum(x => x.d_ImporteSolesH.Value) ? Utils.Windows.DevuelveValorRedondeado(CuentaAsiento1.Sum(x => x.d_ImporteSolesD.Value) - CuentaAsiento1.Sum(x => x.d_ImporteSolesH.Value), 2) * -1 : Utils.Windows.DevuelveValorRedondeado(CuentaAsiento1.Sum(x => x.d_ImporteSolesD.Value) - CuentaAsiento1.Sum(x => x.d_ImporteSolesH.Value), 2);
                                _diarioDetalleDto.d_Cambio = CuentaAsiento1.Sum(x => x.d_ImporteDolaresD.Value) < CuentaAsiento1.Sum(x => x.d_ImporteDolaresH.Value) ? Utils.Windows.DevuelveValorRedondeado(CuentaAsiento1.Sum(x => x.d_ImporteDolaresD.Value) - CuentaAsiento1.Sum(x => x.d_ImporteDolaresH.Value), 2) * -1 : Utils.Windows.DevuelveValorRedondeado(CuentaAsiento1.Sum(x => x.d_ImporteDolaresD.Value) - CuentaAsiento1.Sum(x => x.d_ImporteDolaresH.Value), 2);
                            }
                            else
                            {

                                _diarioDetalleDto.v_Naturaleza = "H";
                                _diarioDetalleDto.d_Importe = CuentaAsiento1.Sum(x => x.d_ImporteSolesD.Value) < CuentaAsiento1.Sum(x => x.d_ImporteSolesH.Value) ? Utils.Windows.DevuelveValorRedondeado(CuentaAsiento1.Sum(x => x.d_ImporteSolesD.Value) - CuentaAsiento1.Sum(x => x.d_ImporteSolesH.Value), 2) * -1 : Utils.Windows.DevuelveValorRedondeado(CuentaAsiento1.Sum(x => x.d_ImporteSolesD.Value) - CuentaAsiento1.Sum(x => x.d_ImporteSolesH.Value), 2);
                                _diarioDetalleDto.d_Cambio = CuentaAsiento1.Sum(x => x.d_ImporteDolaresD.Value) < CuentaAsiento1.Sum(x => x.d_ImporteDolaresH.Value) ? Utils.Windows.DevuelveValorRedondeado(CuentaAsiento1.Sum(x => x.d_ImporteDolaresD.Value) - CuentaAsiento1.Sum(x => x.d_ImporteDolaresH.Value), 2) * -1 : Utils.Windows.DevuelveValorRedondeado(CuentaAsiento1.Sum(x => x.d_ImporteDolaresD.Value) - CuentaAsiento1.Sum(x => x.d_ImporteDolaresH.Value), 2);
                            }

                            _diarioDetalleDto.i_IdCentroCostos = "";
                            _diarioDetalleDto.i_IdTipoDocumento = TipoDocumento;
                            _diarioDetalleDto.i_IdTipoDocumentoRef = TipoDocumento;
                            _diarioDetalleDto.v_Analisis = pAnalisis;
                            _diarioDetalleDto.t_Fecha = DateTime.Parse("31/12/" + pPeriodo);
                            _diarioDetalleDto.v_OrigenDestino = null;
                            _diarioDetalleDto.v_Pedido = null;
                            if (IdRegistroEliminadoAsiento1[0] != null)
                            {
                                _diarioDetalleDto.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                                _diarioDetalleDto.v_NroDocumentoRef = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;

                            }
                            else
                            {

                                _diarioDetalleDto.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                                _diarioDetalleDto.v_NroDocumentoRef = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                            }
                            ListaDetalles.Add(_diarioDetalleDto);
                        }

                        TotDebe = ListaDetalles.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Importe.Value);
                        TotHaber = ListaDetalles.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Importe.Value);
                        TotDebeC = ListaDetalles.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Cambio.Value);
                        TotHaberC = ListaDetalles.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Cambio.Value);
                        _diarioDto.d_TotalDebe = Utils.Windows.DevuelveValorRedondeado(TotDebe, 2);
                        _diarioDto.d_TotalHaber = Utils.Windows.DevuelveValorRedondeado(TotHaber, 2);
                        _diarioDto.d_TotalDebeCambio = Utils.Windows.DevuelveValorRedondeado(TotDebeC, 2);
                        _diarioDto.d_TotalHaberCambio = Utils.Windows.DevuelveValorRedondeado(TotHaberC, 2);
                        _diarioDto.d_DiferenciaDebe = Utils.Windows.DevuelveValorRedondeado(TotDebe - TotHaber, 2);
                        _diarioDto.d_DiferenciaHaber = Utils.Windows.DevuelveValorRedondeado(TotDebeC - TotHaberC, 2);

                        if (ListaDetalles.Count() > 0 && TotDebe != 0 && TotHaber != 0)
                        {
                            _objDiarioBL.InsertarDiario(ref objOperationResult, _diarioDto, Globals.ClientSession.GetAsList(), ListaDetalles, (int)TipoMovimientoTesoreria.Ninguno);

                            if (objOperationResult.Success == 1)
                            {
                                objNroDiario.NumeroDiario = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                                objNroDiario.TipoAsiento = 1;
                                ListaDiarioGenerados.Add(objNroDiario);

                            }


                        }
                        else
                        {
                            _MaxMovimiento = _MaxMovimiento - 1;
                        }
                        Globals.ProgressbarStatus.i_Progress++;

                        #endregion

                        #region Asiento2
                        ///Cuenta 6 juega con la 892
                        //892 --->deudora en el DIARIO
                        //891 -->acreedora  en el DIARIO
                        DebeImporte = 0; HaberImporte = 0;
                        DebeCambio = 0; HaberCambio = 0;
                        _diarioDto = new diarioDto();
                        ListaDetalles = new List<diariodetalleDto>();
                        //List<saldoscontablesDto> CuentasAsiento2 = Saldos.Where(x => x.v_NroCuenta.StartsWith("6") && !x.v_NroCuenta.StartsWith("61") && !x.v_NroCuenta.StartsWith("69")).ToList();
                        List<saldoscontablesDto> CuentasAsiento2 = Saldos.Where(x => x.v_NroCuenta.StartsWith("6")).ToList();
                        _MaxMovimiento++;
                        if (CuentasAsiento2.Where(x => x.v_NroCuenta == pCuentaResult892).Count() == 0)
                        {

                            saldoscontablesDto objSaldo = new saldoscontablesDto();

                            var Asiento = (from a in dbContext.asientocontable
                                           where a.i_Eliminado == 0 && a.v_NroCuenta == pCuentaResult892 && a.v_Periodo == periodo
                                           select a).FirstOrDefault();

                            objSaldo.v_Anio = pPeriodo;
                            objSaldo.v_Mes = "12";
                            objSaldo.v_NroCuenta = Asiento.v_NroCuenta;
                            objSaldo.Naturaleza = Asiento.i_Naturaleza == 1 ? "D" : "H";
                            objSaldo.d_ImporteDolaresD = 0;
                            objSaldo.d_ImporteDolaresH = 0;
                            objSaldo.d_ImporteSolesD = 0;
                            objSaldo.d_ImporteSolesH = 0;
                            CuentasAsiento2.Add(objSaldo);
                        }

                        if (IdRegistroEliminadoAsiento2[0] != null)
                        {

                            _diarioDto.v_Periodo = IdRegistroEliminadoAsiento2[0];
                            _diarioDto.v_Mes = IdRegistroEliminadoAsiento2[1];
                            _diarioDto.v_Correlativo = IdRegistroEliminadoAsiento2[2];
                        }
                        else
                        {
                            _diarioDto.v_Periodo = pPeriodo.ToString();
                            _diarioDto.v_Mes = MesDiario.ToString();
                            _diarioDto.v_Correlativo = _MaxMovimiento.ToString("00000000");
                        }

                        _diarioDto.v_IdDocumentoReferencia = "AC-2-" + pPeriodo;
                        _diarioDto.v_Nombre = "ASIENTO CIERRE 2 : CANCELACIÓN CUENTA 6 - GASTOS";
                        _diarioDto.v_Glosa = "ASIENTO  CIERRE 2 : CANCELACIÓN CUENTA 6 - GASTOS";
                        _diarioDto.d_TipoCambio = decimal.Parse("1");
                        _diarioDto.i_IdMoneda = (int)Currency.Soles;
                        _diarioDto.i_IdTipoDocumento = TipoDocumento;
                        _diarioDto.t_Fecha = DateTime.Parse("31/12/" + pPeriodo);
                        _diarioDto.i_IdTipoComprobante = 5;

                        var CuentasAgrupadasAsiento2 = CuentasAsiento2.GroupBy(x => x.v_NroCuenta).OrderBy(x => x.FirstOrDefault().v_NroCuenta);
                        foreach (var CuentaAsiento2 in CuentasAgrupadasAsiento2)
                        {

                            _diarioDetalleDto = new diariodetalleDto();
                            _diarioDetalleDto.v_NroCuenta = CuentaAsiento2.FirstOrDefault().v_NroCuenta;

                            if (CuentaAsiento2.FirstOrDefault().v_NroCuenta == pCuentaResult892)
                            {
                                if (DebeImporte > HaberImporte)
                                {
                                    _diarioDetalleDto.v_Naturaleza = "H";
                                    _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(DebeImporte - HaberImporte, 2);
                                    _diarioDetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(DebeCambio - HaberCambio, 2);

                                }
                                else
                                {
                                    _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(HaberImporte - DebeImporte, 2);
                                    _diarioDetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(HaberCambio - DebeCambio, 2);
                                    _diarioDetalleDto.v_Naturaleza = "D";
                                }

                            }
                            else
                            {
                                if (CuentaAsiento2.Sum(x => x.d_ImporteSolesD.Value) < CuentaAsiento2.Sum(x => x.d_ImporteSolesH.Value))
                                {

                                    _diarioDetalleDto.v_Naturaleza = "D";
                                    _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(CuentaAsiento2.Sum(x => x.d_ImporteSolesD.Value) - CuentaAsiento2.Sum(x => x.d_ImporteSolesH.Value), 2) * -1;
                                    _diarioDetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(CuentaAsiento2.Sum(x => x.d_ImporteDolaresD.Value) - CuentaAsiento2.Sum(x => x.d_ImporteDolaresH.Value), 2) * -1;
                                    DebeImporte = DebeImporte + _diarioDetalleDto.d_Importe.Value;
                                    DebeCambio = DebeCambio + _diarioDetalleDto.d_Cambio.Value;

                                }
                                else
                                {
                                    _diarioDetalleDto.v_Naturaleza = "H";
                                    _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(CuentaAsiento2.Sum(x => x.d_ImporteSolesD.Value) - CuentaAsiento2.Sum(x => x.d_ImporteSolesH.Value), 2);
                                    _diarioDetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(CuentaAsiento2.Sum(x => x.d_ImporteDolaresD.Value) - CuentaAsiento2.Sum(x => x.d_ImporteDolaresH.Value), 2);

                                    HaberImporte = HaberImporte + _diarioDetalleDto.d_Importe.Value;
                                    HaberCambio = HaberCambio + _diarioDetalleDto.d_Cambio.Value;
                                }
                            }


                            _diarioDetalleDto.i_IdCentroCostos = "";
                            _diarioDetalleDto.i_IdTipoDocumento = TipoDocumento;
                            _diarioDetalleDto.i_IdTipoDocumentoRef = TipoDocumento;
                            _diarioDetalleDto.v_Analisis = pAnalisis;
                            _diarioDetalleDto.t_Fecha = DateTime.Parse("31/12/" + pPeriodo);
                            _diarioDetalleDto.v_OrigenDestino = null;
                            _diarioDetalleDto.v_Pedido = null;
                            if (IdRegistroEliminadoAsiento2[0] != null)
                            {
                                _diarioDetalleDto.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                                _diarioDetalleDto.v_NroDocumentoRef = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;

                            }
                            else
                            {

                                _diarioDetalleDto.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                                _diarioDetalleDto.v_NroDocumentoRef = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                            }
                            ListaDetalles.Add(_diarioDetalleDto);
                        }

                        TotDebe = ListaDetalles.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Importe.Value);
                        TotHaber = ListaDetalles.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Importe.Value);
                        TotDebeC = ListaDetalles.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Cambio.Value);
                        TotHaberC = ListaDetalles.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Cambio.Value);
                        _diarioDto.d_TotalDebe = Utils.Windows.DevuelveValorRedondeado(TotDebe, 2);
                        _diarioDto.d_TotalHaber = Utils.Windows.DevuelveValorRedondeado(TotHaber, 2);
                        _diarioDto.d_TotalDebeCambio = Utils.Windows.DevuelveValorRedondeado(TotDebeC, 2);
                        _diarioDto.d_TotalHaberCambio = Utils.Windows.DevuelveValorRedondeado(TotHaberC, 2);
                        _diarioDto.d_DiferenciaDebe = Utils.Windows.DevuelveValorRedondeado(TotDebe - TotHaber, 2);
                        _diarioDto.d_DiferenciaHaber = Utils.Windows.DevuelveValorRedondeado(TotDebeC - TotHaberC, 2);

                        if (ListaDetalles.Count() > 0 && TotDebe != 0 && TotHaber != 0)
                        {
                            _objDiarioBL.InsertarDiario(ref objOperationResult, _diarioDto, Globals.ClientSession.GetAsList(), ListaDetalles, (int)TipoMovimientoTesoreria.Ninguno);

                            objNroDiario = new NroDiario();
                            if (objOperationResult.Success == 1)
                            {
                                objNroDiario.NumeroDiario = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                                objNroDiario.TipoAsiento = 2;
                                ListaDiarioGenerados.Add(objNroDiario);
                            }
                        }
                        else
                        {
                            _MaxMovimiento = _MaxMovimiento - 1;
                        }
                        Globals.ProgressbarStatus.i_Progress++;

                        #endregion

                        #region Asiento3
                        DebeImporte = 0; HaberImporte = 0;
                        DebeCambio = 0; HaberCambio = 0;
                        //Cuenta 7 juega con la  891 
                        _diarioDto = new diarioDto();
                        ListaDetalles = new List<diariodetalleDto>();
                        // var CuentasAsiento3 = Saldos.Where(x => x.v_NroCuenta.StartsWith("7") && !x.v_NroCuenta.StartsWith("71") && !x.v_NroCuenta.StartsWith("79")).ToList();
                        var CuentasAsiento3 = Saldos.Where(x => x.v_NroCuenta.StartsWith("7") && !x.v_NroCuenta.StartsWith("79")).ToList();
                        _MaxMovimiento++;
                        if (CuentasAsiento3.Where(x => x.v_NroCuenta == pCuentaResult891).Count() == 0)
                        {

                            saldoscontablesDto objSaldo = new saldoscontablesDto();
                            var Asiento = (from a in dbContext.asientocontable
                                           where a.i_Eliminado == 0 && a.v_NroCuenta == pCuentaResult891 && a.v_Periodo == periodo
                                           select a).FirstOrDefault();
                            objSaldo.v_Anio = pPeriodo;
                            objSaldo.v_Mes = "12";
                            objSaldo.v_NroCuenta = Asiento.v_NroCuenta;
                            objSaldo.Naturaleza = Asiento.i_Naturaleza == 1 ? "D" : "H";
                            objSaldo.d_ImporteDolaresD = 0;
                            objSaldo.d_ImporteDolaresH = 0;
                            objSaldo.d_ImporteSolesD = 0;
                            objSaldo.d_ImporteSolesH = 0;
                            CuentasAsiento3.Add(objSaldo);
                        }
                        if (IdRegistroEliminadoAsiento3[0] != null)
                        {

                            _diarioDto.v_Periodo = IdRegistroEliminadoAsiento3[0];
                            _diarioDto.v_Mes = IdRegistroEliminadoAsiento3[1];
                            _diarioDto.v_Correlativo = IdRegistroEliminadoAsiento3[2];
                        }
                        else
                        {
                            _diarioDto.v_Periodo = pPeriodo.ToString();
                            _diarioDto.v_Mes = MesDiario.ToString();
                            _diarioDto.v_Correlativo = _MaxMovimiento.ToString("00000000");
                        }
                        _diarioDto.v_IdDocumentoReferencia = "AC-3-" + pPeriodo;
                        _diarioDto.v_Nombre = "ASIENTO CIERRE 3 : CANCELACIÓN CUENTA 7 - INGRESOS";
                        _diarioDto.v_Glosa = "ASIENTO  CIERRE 3 : CANCELACIÓN CUENTA 7 - INGRESOS";
                        _diarioDto.d_TipoCambio = decimal.Parse("1");
                        _diarioDto.i_IdMoneda = (int)Currency.Soles;
                        _diarioDto.i_IdTipoDocumento = TipoDocumento;
                        _diarioDto.t_Fecha = DateTime.Parse("31/12/" + pPeriodo);
                        _diarioDto.i_IdTipoComprobante = 5;

                        var CuentasAgrupadasAsiento3 = CuentasAsiento3.GroupBy(x => x.v_NroCuenta).OrderBy(x => x.FirstOrDefault().v_NroCuenta);
                        foreach (var CuentaAsiento3 in CuentasAgrupadasAsiento3)
                        {

                            _diarioDetalleDto = new diariodetalleDto();
                            _diarioDetalleDto.v_NroCuenta = CuentaAsiento3.FirstOrDefault().v_NroCuenta;

                            if (_diarioDetalleDto.v_NroCuenta == pCuentaResult891)
                            {
                                if (DebeImporte > HaberImporte)
                                {
                                    _diarioDetalleDto.v_Naturaleza = "H";
                                    _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(DebeImporte - HaberImporte, 2);
                                    _diarioDetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(DebeCambio - HaberCambio, 2);

                                }
                                else
                                {
                                    _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(HaberImporte - DebeImporte, 2);
                                    _diarioDetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(HaberCambio - DebeCambio, 2);
                                    _diarioDetalleDto.v_Naturaleza = "D";
                                }
                            }
                            else
                            {
                                if (CuentaAsiento3.Sum(x => x.d_ImporteSolesD.Value) < CuentaAsiento3.Sum(x => x.d_ImporteSolesH.Value))
                                {

                                    _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(CuentaAsiento3.Sum(x => x.d_ImporteSolesD.Value) - CuentaAsiento3.Sum(x => x.d_ImporteSolesH.Value), 2) * -1;
                                    _diarioDetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(CuentaAsiento3.Sum(x => x.d_ImporteDolaresD.Value) - CuentaAsiento3.Sum(x => x.d_ImporteDolaresH.Value), 2) * -1;
                                    _diarioDetalleDto.v_Naturaleza = "D";
                                    DebeImporte = DebeImporte + _diarioDetalleDto.d_Importe.Value;
                                    DebeCambio = DebeCambio + _diarioDetalleDto.d_Cambio.Value;

                                }
                                else
                                {
                                    _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(CuentaAsiento3.Sum(x => x.d_ImporteSolesD.Value) - CuentaAsiento3.Sum(x => x.d_ImporteSolesH.Value), 2);
                                    _diarioDetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(CuentaAsiento3.Sum(x => x.d_ImporteDolaresD.Value) - CuentaAsiento3.Sum(x => x.d_ImporteDolaresH.Value), 2);
                                    _diarioDetalleDto.v_Naturaleza = "H";
                                    HaberImporte = HaberImporte + _diarioDetalleDto.d_Importe.Value;
                                    HaberCambio = HaberCambio + _diarioDetalleDto.d_Cambio.Value;
                                }

                            }
                            _diarioDetalleDto.i_IdCentroCostos = "";
                            _diarioDetalleDto.i_IdTipoDocumento = TipoDocumento;
                            _diarioDetalleDto.i_IdTipoDocumentoRef = TipoDocumento;
                            _diarioDetalleDto.v_Analisis = pAnalisis;
                            _diarioDetalleDto.t_Fecha = DateTime.Parse("31/12/" + pPeriodo);
                            _diarioDetalleDto.v_OrigenDestino = null;
                            _diarioDetalleDto.v_Pedido = null;
                            if (IdRegistroEliminadoAsiento3[0] != null)
                            {
                                _diarioDetalleDto.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                                _diarioDetalleDto.v_NroDocumentoRef = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;

                            }
                            else
                            {

                                _diarioDetalleDto.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                                _diarioDetalleDto.v_NroDocumentoRef = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                            }
                            ListaDetalles.Add(_diarioDetalleDto);
                        }

                        TotDebe = ListaDetalles.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Importe.Value);
                        TotHaber = ListaDetalles.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Importe.Value);
                        TotDebeC = ListaDetalles.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Cambio.Value);
                        TotHaberC = ListaDetalles.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Cambio.Value);
                        _diarioDto.d_TotalDebe = Utils.Windows.DevuelveValorRedondeado(TotDebe, 2);
                        _diarioDto.d_TotalHaber = Utils.Windows.DevuelveValorRedondeado(TotHaber, 2);
                        _diarioDto.d_TotalDebeCambio = Utils.Windows.DevuelveValorRedondeado(TotDebeC, 2);
                        _diarioDto.d_TotalHaberCambio = Utils.Windows.DevuelveValorRedondeado(TotHaberC, 2);
                        _diarioDto.d_DiferenciaDebe = Utils.Windows.DevuelveValorRedondeado(TotDebe - TotHaber, 2);
                        _diarioDto.d_DiferenciaHaber = Utils.Windows.DevuelveValorRedondeado(TotDebeC - TotHaberC, 2);

                        if (ListaDetalles.Count() > 0 && TotDebe != 0 && TotHaber != 0)
                        {
                            _objDiarioBL.InsertarDiario(ref objOperationResult, _diarioDto, Globals.ClientSession.GetAsList(), ListaDetalles, (int)TipoMovimientoTesoreria.Ninguno);

                            objNroDiario = new NroDiario();
                            if (objOperationResult.Success == 1)
                            {
                                objNroDiario.NumeroDiario = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                                objNroDiario.TipoAsiento = 3;
                                ListaDiarioGenerados.Add(objNroDiario);

                            }
                        }
                        else
                        {
                            _MaxMovimiento = _MaxMovimiento - 1;
                        }

                        Globals.ProgressbarStatus.i_Progress++;
                        #endregion

                        #region Asiento4
                        _diarioDto = new diarioDto();
                        ListaDetalles = new List<diariodetalleDto>();
                        var CuentasAsiento4 = Saldos.Where(x => int.Parse(x.CuentaMayor) <= 9 && int.Parse(x.CuentaMayor) >= 1).ToList();
                        _MaxMovimiento++;

                        if (IdRegistroEliminadoAsiento4[0] != null)
                        {

                            _diarioDto.v_Periodo = IdRegistroEliminadoAsiento4[0];
                            _diarioDto.v_Mes = IdRegistroEliminadoAsiento4[1];
                            _diarioDto.v_Correlativo = IdRegistroEliminadoAsiento4[2];
                        }
                        else
                        {
                            _diarioDto.v_Periodo = pPeriodo.ToString();
                            _diarioDto.v_Mes = MesDiario.ToString();
                            _diarioDto.v_Correlativo = _MaxMovimiento.ToString("00000000");
                        }

                        _diarioDto.v_IdDocumentoReferencia = "AC-4-" + pPeriodo;
                        _diarioDto.v_Nombre = "ASIENTO CIERRE 4 : CANCELACIÓN CUENTAS DE ORDEN";
                        _diarioDto.v_Glosa = "ASIENTO CIERRE 4 : CANCELACIÓN CUENTAS DE ORDEN";
                        _diarioDto.d_TipoCambio = decimal.Parse("1");
                        _diarioDto.i_IdMoneda = (int)Currency.Soles;
                        _diarioDto.i_IdTipoDocumento = TipoDocumento;
                        _diarioDto.t_Fecha = DateTime.Parse("31/12/" + pPeriodo);
                        _diarioDto.i_IdTipoComprobante = 5;

                        var CuentasAgrupadasAsiento4 = CuentasAsiento4.GroupBy(x => x.v_NroCuenta);
                        foreach (var CuentaAsiento4 in CuentasAgrupadasAsiento4)
                        {

                            _diarioDetalleDto = new diariodetalleDto();
                            _diarioDetalleDto.v_NroCuenta = CuentaAsiento4.FirstOrDefault().v_NroCuenta;

                            if (CuentaAsiento4.Sum(x => x.d_ImporteSolesD.Value) < CuentaAsiento4.Sum(x => x.d_ImporteSolesH.Value))
                            {
                                _diarioDetalleDto.v_Naturaleza = "D";
                                _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(CuentaAsiento4.Sum(x => x.d_ImporteSolesD.Value) - CuentaAsiento4.Sum(x => x.d_ImporteSolesH.Value), 2) * -1;
                                _diarioDetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(CuentaAsiento4.Sum(x => x.d_ImporteDolaresD.Value) - CuentaAsiento4.Sum(x => x.d_ImporteDolaresH.Value), 2) * -1;
                            }
                            else
                            {
                                _diarioDetalleDto.v_Naturaleza = "H";
                                _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(CuentaAsiento4.Sum(x => x.d_ImporteSolesD.Value) - CuentaAsiento4.Sum(x => x.d_ImporteSolesH.Value), 2);
                                _diarioDetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(CuentaAsiento4.Sum(x => x.d_ImporteDolaresD.Value) - CuentaAsiento4.Sum(x => x.d_ImporteDolaresH.Value), 2);
                            }

                            _diarioDetalleDto.i_IdCentroCostos = "";
                            _diarioDetalleDto.i_IdTipoDocumento = TipoDocumento;
                            _diarioDetalleDto.i_IdTipoDocumentoRef = TipoDocumento;
                            _diarioDetalleDto.v_Analisis = pAnalisis;
                            _diarioDetalleDto.t_Fecha = DateTime.Parse("31/12/" + pPeriodo);
                            _diarioDetalleDto.v_OrigenDestino = null;
                            _diarioDetalleDto.v_Pedido = null;
                            if (IdRegistroEliminadoAsiento5[0] != null)
                            {
                                _diarioDetalleDto.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                                _diarioDetalleDto.v_NroDocumentoRef = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;

                            }
                            else
                            {

                                _diarioDetalleDto.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                                _diarioDetalleDto.v_NroDocumentoRef = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                            }
                            ListaDetalles.Add(_diarioDetalleDto);
                        }

                        TotDebe = ListaDetalles.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Importe.Value);
                        TotHaber = ListaDetalles.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Importe.Value);
                        TotDebeC = ListaDetalles.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Cambio.Value);
                        TotHaberC = ListaDetalles.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Cambio.Value);
                        _diarioDto.d_TotalDebe = Utils.Windows.DevuelveValorRedondeado(TotDebe, 2);
                        _diarioDto.d_TotalHaber = Utils.Windows.DevuelveValorRedondeado(TotHaber, 2);
                        _diarioDto.d_TotalDebeCambio = Utils.Windows.DevuelveValorRedondeado(TotDebeC, 2);
                        _diarioDto.d_TotalHaberCambio = Utils.Windows.DevuelveValorRedondeado(TotHaberC, 2);
                        _diarioDto.d_DiferenciaDebe = Utils.Windows.DevuelveValorRedondeado(TotDebe - TotHaber, 2);
                        _diarioDto.d_DiferenciaHaber = Utils.Windows.DevuelveValorRedondeado(TotDebeC - TotHaberC, 2);
                        if (ListaDetalles.Count() > 0 && TotDebe != 0 && TotHaber != 0)
                        {
                            _objDiarioBL.InsertarDiario(ref objOperationResult, _diarioDto, Globals.ClientSession.GetAsList(), ListaDetalles, (int)TipoMovimientoTesoreria.Ninguno);
                            objNroDiario = new NroDiario();
                            if (objOperationResult.Success == 1)
                            {
                                objNroDiario.NumeroDiario = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                                objNroDiario.TipoAsiento = 4;
                                ListaDiarioGenerados.Add(objNroDiario);

                            }
                        }
                        else
                        {
                            _MaxMovimiento = _MaxMovimiento - 1;
                        }
                        Globals.ProgressbarStatus.i_Progress++;

                        #endregion

                        dbContext.SaveChanges();

                        var saldoscontablesNuevos = new AsientosContablesBL().ObtenerSaldosContablesPorMes(ref  objOperationResult, 12, Globals.ClientSession.i_Periodo.Value);

                        var f = saldoscontablesNuevos.Where(o => o.v_NroCuenta.StartsWith("20")).ToList();
                        var NuevosSaldos = (from a in saldoscontablesNuevos
                                            join b in dbContext.asientocontable on new { cuenta = a.v_NroCuenta, eliminado = 0, per = periodo } equals new { cuenta = b.v_NroCuenta, eliminado = b.i_Eliminado.Value, per = b.v_Periodo } into b_join
                                            from b in b_join.DefaultIfEmpty()
                                            // where a.v_Anio == pPeriodo
                                            select new saldoscontablesDto
                                            {

                                                v_NroCuenta = a.v_NroCuenta,
                                                d_ImporteSolesD = a.d_ImporteSolesD,
                                                d_ImporteDolaresD = a.d_ImporteDolaresD,
                                                d_ImporteSolesH = a.d_ImporteSolesH,
                                                d_ImporteDolaresH = a.d_ImporteDolaresH,
                                                Naturaleza = b.i_Naturaleza == 1 ? "D" : "H",
                                                CuentaMayor = a.v_NroCuenta.Substring(0, 2),
                                            }).ToList();

                        #region Asiento5

                        _diarioDto = new diarioDto();
                        ListaDetalles = new List<diariodetalleDto>();
                        var CuentasAsiento5 = NuevosSaldos.Where(x => x.v_NroCuenta == pCuentaResult891 || x.v_NroCuenta == pCuentaResult892).ToList();
                        _MaxMovimiento++;

                        if (IdRegistroEliminadoAsiento4[0] != null)
                        {

                            _diarioDto.v_Periodo = IdRegistroEliminadoAsiento5[0];
                            _diarioDto.v_Mes = IdRegistroEliminadoAsiento5[1];
                            _diarioDto.v_Correlativo = IdRegistroEliminadoAsiento5[2];
                        }
                        else
                        {
                            _diarioDto.v_Periodo = pPeriodo.ToString();
                            _diarioDto.v_Mes = MesDiario.ToString();
                            _diarioDto.v_Correlativo = _MaxMovimiento.ToString("00000000");
                        }

                        _diarioDto.v_IdDocumentoReferencia = "AC-5-" + pPeriodo;
                        _diarioDto.v_Nombre = "ASIENTO CIERRE 5: CANCELACIÓN CUENTAS 891 - 892";
                        _diarioDto.v_Glosa = "ASIENTO CIERRE 5 : CANCELACIÓN CUENTAS 891 - 892";
                        _diarioDto.d_TipoCambio = decimal.Parse("1");
                        _diarioDto.i_IdMoneda = (int)Currency.Soles;
                        _diarioDto.i_IdTipoDocumento = TipoDocumento;
                        _diarioDto.t_Fecha = DateTime.Parse("31/12/" + pPeriodo);
                        _diarioDto.i_IdTipoComprobante = 5;
                        _diarioDetalleDto = new diariodetalleDto();
                        decimal Saldo891Soles = CuentasAsiento5.Where(x => x.v_NroCuenta == pCuentaResult891).Sum(x => x.d_ImporteSolesD.Value) - CuentasAsiento5.Where(x => x.v_NroCuenta == pCuentaResult891).Sum(x => x.d_ImporteSolesH.Value);
                        decimal Saldo892Soles = CuentasAsiento5.Where(x => x.v_NroCuenta == pCuentaResult892).Sum(x => x.d_ImporteSolesD.Value) - CuentasAsiento5.Where(x => x.v_NroCuenta == pCuentaResult892).Sum(x => x.d_ImporteSolesH.Value);
                        decimal Saldo891Dolares = CuentasAsiento5.Where(x => x.v_NroCuenta == pCuentaResult891).Sum(x => x.d_ImporteDolaresD.Value) - CuentasAsiento5.Where(x => x.v_NroCuenta == pCuentaResult891).Sum(x => x.d_ImporteDolaresH.Value);
                        decimal Saldo892Dolares = CuentasAsiento5.Where(x => x.v_NroCuenta == pCuentaResult892).Sum(x => x.d_ImporteDolaresD.Value) - CuentasAsiento5.Where(x => x.v_NroCuenta == pCuentaResult892).Sum(x => x.d_ImporteDolaresH.Value);


                        Saldo891Soles = Saldo891Soles < 0 ? Saldo891Soles * -1 : Saldo891Soles;
                        Saldo892Soles = Saldo892Soles < 0 ? Saldo892Soles * -1 : Saldo892Soles;

                        Saldo891Dolares = Saldo891Dolares < 0 ? Saldo891Dolares * -1 : Saldo891Dolares;
                        Saldo892Dolares = Saldo892Dolares < 0 ? Saldo892Dolares * -1 : Saldo892Dolares;

                        if (Utils.Windows.DevuelveValorRedondeado(Saldo891Soles, 2) > Utils.Windows.DevuelveValorRedondeado(Saldo892Soles, 2))
                        {
                            _diarioDetalleDto.v_Naturaleza = "D";
                            _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(Saldo891Soles - Saldo892Soles, 2);
                            _diarioDetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(Saldo891Dolares - Saldo892Dolares, 2);
                            _diarioDetalleDto.v_NroCuenta = pCuentaResult891;


                            _diarioDetalleDto2.v_Naturaleza = "H";
                            _diarioDetalleDto2.v_NroCuenta = pCuentaUtilidad;
                            _diarioDetalleDto2.d_Importe = Utils.Windows.DevuelveValorRedondeado(Saldo891Soles - Saldo892Soles, 2);
                            _diarioDetalleDto2.d_Cambio = Utils.Windows.DevuelveValorRedondeado(Saldo891Dolares - Saldo892Dolares, 2);

                        }
                        else
                        {
                            _diarioDetalleDto.v_Naturaleza = "H";
                            _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(Saldo892Soles - Saldo891Soles, 2);
                            _diarioDetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(Saldo892Dolares - Saldo891Dolares, 2);
                            _diarioDetalleDto.v_NroCuenta = pCuentaResult892;


                            _diarioDetalleDto2.v_Naturaleza = "D";
                            _diarioDetalleDto2.v_NroCuenta = pCuentaPerdida;
                            _diarioDetalleDto2.d_Importe = Utils.Windows.DevuelveValorRedondeado(Saldo892Soles - Saldo891Soles, 2);
                            _diarioDetalleDto2.d_Cambio = Utils.Windows.DevuelveValorRedondeado(Saldo892Dolares - Saldo891Dolares, 2);

                        }
                        _diarioDetalleDto.i_IdCentroCostos = "";
                        _diarioDetalleDto.i_IdTipoDocumento = TipoDocumento;
                        _diarioDetalleDto.i_IdTipoDocumentoRef = TipoDocumento;
                        _diarioDetalleDto.v_Analisis = pAnalisis;
                        _diarioDetalleDto.t_Fecha = DateTime.Parse("31/12/" + pPeriodo);
                        _diarioDetalleDto.v_OrigenDestino = null;
                        _diarioDetalleDto.v_Pedido = null;

                        _diarioDetalleDto2.i_IdCentroCostos = "";
                        _diarioDetalleDto2.i_IdTipoDocumento = 335;
                        _diarioDetalleDto2.i_IdTipoDocumentoRef = 335;
                        _diarioDetalleDto2.v_Analisis = pAnalisis;
                        _diarioDetalleDto2.t_Fecha = DateTime.Parse("31/12/" + pPeriodo);
                        _diarioDetalleDto2.v_OrigenDestino = null;
                        _diarioDetalleDto2.v_Pedido = null;
                        if (IdRegistroEliminadoAsiento5[0] != null)
                        {
                            _diarioDetalleDto.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                            _diarioDetalleDto.v_NroDocumentoRef = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;

                            _diarioDetalleDto2.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                            _diarioDetalleDto2.v_NroDocumentoRef = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;

                        }
                        else
                        {

                            _diarioDetalleDto.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                            _diarioDetalleDto.v_NroDocumentoRef = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                            _diarioDetalleDto2.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                            _diarioDetalleDto2.v_NroDocumentoRef = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                        }
                        ListaDetalles.Add(_diarioDetalleDto);
                        ListaDetalles.Add(_diarioDetalleDto2);
                        //}

                        TotDebe = ListaDetalles.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Importe.Value);
                        TotHaber = ListaDetalles.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Importe.Value);
                        TotDebeC = ListaDetalles.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Cambio.Value);
                        TotHaberC = ListaDetalles.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Cambio.Value);
                        _diarioDto.d_TotalDebe = Utils.Windows.DevuelveValorRedondeado(TotDebe, 2);
                        _diarioDto.d_TotalHaber = Utils.Windows.DevuelveValorRedondeado(TotHaber, 2);
                        _diarioDto.d_TotalDebeCambio = Utils.Windows.DevuelveValorRedondeado(TotDebeC, 2);
                        _diarioDto.d_TotalHaberCambio = Utils.Windows.DevuelveValorRedondeado(TotHaberC, 2);
                        _diarioDto.d_DiferenciaDebe = Utils.Windows.DevuelveValorRedondeado(TotDebe - TotHaber, 2);
                        _diarioDto.d_DiferenciaHaber = Utils.Windows.DevuelveValorRedondeado(TotDebeC - TotHaberC, 2);
                        if (ListaDetalles.Count() > 0 && TotDebe != 0 && TotHaber != 0)
                        {
                            _objDiarioBL.InsertarDiario(ref objOperationResult, _diarioDto, Globals.ClientSession.GetAsList(), ListaDetalles, (int)TipoMovimientoTesoreria.Ninguno);

                            objNroDiario = new NroDiario();
                            if (objOperationResult.Success == 1)
                            {
                                objNroDiario.NumeroDiario = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                                objNroDiario.TipoAsiento = 5;
                                ListaDiarioGenerados.Add(objNroDiario);

                            }
                        }
                        else
                        {
                            _MaxMovimiento = _MaxMovimiento - 1;
                        }
                        Globals.ProgressbarStatus.i_Progress++;
                        #endregion

                        dbContext.SaveChanges();
                        var saldosFin = new AsientosContablesBL().ObtenerSaldosContablesPorMes(ref  objOperationResult, 12, Globals.ClientSession.i_Periodo.Value, 1, false, true);
                        var SaldosFinales = (from a in saldosFin
                                             join b in dbContext.asientocontable on new { cuenta = a.v_NroCuenta, eliminado = 0, per = periodo } equals new { cuenta = b.v_NroCuenta, eliminado = b.i_Eliminado.Value, per = b.v_Periodo } into b_join
                                             from b in b_join.DefaultIfEmpty()

                                             select new saldoscontablesDto
                                             {

                                                 v_NroCuenta = a.v_NroCuenta,
                                                 d_ImporteSolesD = a.d_ImporteSolesD,
                                                 d_ImporteDolaresD = a.d_ImporteDolaresD,
                                                 d_ImporteSolesH = a.d_ImporteSolesH,
                                                 d_ImporteDolaresH = a.d_ImporteDolaresH,
                                                 Naturaleza = b.i_Naturaleza == 1 ? "D" : "H",
                                                 CuentaMayor = a.v_NroCuenta.Substring(0, 2),
                                             }).ToList();



                        var hhh = SaldosFinales.Where(o => o.v_NroCuenta.StartsWith("20")).ToList();

                        #region Asiento6
                        _diarioDto = new diarioDto();
                        ListaDetalles = new List<diariodetalleDto>();

                        var CuentasAsiento6 = SaldosFinales.Where(x => int.Parse(x.CuentaMayor) >= 10 && int.Parse(x.CuentaMayor) <= 59);

                        _MaxMovimiento++;

                        if (IdRegistroEliminadoAsiento6[0] != null)
                        {

                            _diarioDto.v_Periodo = IdRegistroEliminadoAsiento6[0];
                            _diarioDto.v_Mes = IdRegistroEliminadoAsiento6[1];
                            _diarioDto.v_Correlativo = IdRegistroEliminadoAsiento6[2];
                        }
                        else
                        {
                            _diarioDto.v_Periodo = pPeriodo.ToString();
                            _diarioDto.v_Mes = MesDiario.ToString();
                            _diarioDto.v_Correlativo = _MaxMovimiento.ToString("00000000");
                        }

                        _diarioDto.v_IdDocumentoReferencia = "AC-6-" + pPeriodo;
                        _diarioDto.v_Nombre = "ASIENTO 6 : CANCELACIÓN ACTIVO Y PASIVO";
                        _diarioDto.v_Glosa = "ASIENTO 6 : CANCELACIÓN ACTIVO Y PASIVO";
                        _diarioDto.d_TipoCambio = decimal.Parse("1");
                        _diarioDto.i_IdMoneda = (int)Currency.Soles;
                        _diarioDto.i_IdTipoDocumento = TipoDocumento;
                        _diarioDto.t_Fecha = DateTime.Parse("31/12/" + pPeriodo);
                        _diarioDto.i_IdTipoComprobante = 5;

                        var CuentasAgrupadasAsiento6 = CuentasAsiento6.GroupBy(x => x.v_NroCuenta).OrderBy(x => x.FirstOrDefault().v_NroCuenta);
                        foreach (var CuentaAsiento6 in CuentasAgrupadasAsiento6)
                        {

                            _diarioDetalleDto = new diariodetalleDto();
                            _diarioDetalleDto.v_NroCuenta = CuentaAsiento6.FirstOrDefault().v_NroCuenta;

                            if (CuentaAsiento6.Sum(x => x.d_ImporteSolesD.Value) < CuentaAsiento6.Sum(x => x.d_ImporteSolesH.Value))
                            {
                                _diarioDetalleDto.v_Naturaleza = "D";
                                _diarioDetalleDto.d_Importe = CuentaAsiento6.Sum(x => x.d_ImporteSolesD.Value) < CuentaAsiento6.Sum(x => x.d_ImporteSolesH.Value) ? Utils.Windows.DevuelveValorRedondeado(CuentaAsiento6.Sum(x => x.d_ImporteSolesD.Value) - CuentaAsiento6.Sum(x => x.d_ImporteSolesH.Value), 2) * -1 : Utils.Windows.DevuelveValorRedondeado(CuentaAsiento6.Sum(x => x.d_ImporteSolesD.Value) - CuentaAsiento6.Sum(x => x.d_ImporteSolesH.Value), 2);
                                _diarioDetalleDto.d_Cambio = CuentaAsiento6.Sum(x => x.d_ImporteDolaresD.Value) < CuentaAsiento6.Sum(x => x.d_ImporteDolaresH.Value) ? Utils.Windows.DevuelveValorRedondeado(CuentaAsiento6.Sum(x => x.d_ImporteDolaresD.Value) - CuentaAsiento6.Sum(x => x.d_ImporteDolaresH.Value), 2) * -1 : Utils.Windows.DevuelveValorRedondeado(CuentaAsiento6.Sum(x => x.d_ImporteDolaresD.Value) - CuentaAsiento6.Sum(x => x.d_ImporteDolaresH.Value), 2);
                            }
                            else
                            {

                                _diarioDetalleDto.v_Naturaleza = "H";
                                _diarioDetalleDto.d_Importe = CuentaAsiento6.Sum(x => x.d_ImporteSolesD.Value) < CuentaAsiento6.Sum(x => x.d_ImporteSolesH.Value) ? Utils.Windows.DevuelveValorRedondeado(CuentaAsiento6.Sum(x => x.d_ImporteSolesD.Value) - CuentaAsiento6.Sum(x => x.d_ImporteSolesH.Value), 2) * -1 : Utils.Windows.DevuelveValorRedondeado(CuentaAsiento6.Sum(x => x.d_ImporteSolesD.Value) - CuentaAsiento6.Sum(x => x.d_ImporteSolesH.Value), 2);
                                _diarioDetalleDto.d_Cambio = CuentaAsiento6.Sum(x => x.d_ImporteDolaresD.Value) < CuentaAsiento6.Sum(x => x.d_ImporteDolaresH.Value) ? Utils.Windows.DevuelveValorRedondeado(CuentaAsiento6.Sum(x => x.d_ImporteDolaresD.Value) - CuentaAsiento6.Sum(x => x.d_ImporteDolaresH.Value), 2) * -1 : Utils.Windows.DevuelveValorRedondeado(CuentaAsiento6.Sum(x => x.d_ImporteDolaresD.Value) - CuentaAsiento6.Sum(x => x.d_ImporteDolaresH.Value), 2);
                            }


                            _diarioDetalleDto.d_Importe = CuentaAsiento6.Sum(x => x.d_ImporteSolesD.Value) < CuentaAsiento6.Sum(x => x.d_ImporteSolesH.Value) ? Utils.Windows.DevuelveValorRedondeado(CuentaAsiento6.Sum(x => x.d_ImporteSolesD.Value) - CuentaAsiento6.Sum(x => x.d_ImporteSolesH.Value), 2) * -1 : Utils.Windows.DevuelveValorRedondeado(CuentaAsiento6.Sum(x => x.d_ImporteSolesD.Value) - CuentaAsiento6.Sum(x => x.d_ImporteSolesH.Value), 2);
                            _diarioDetalleDto.d_Cambio = CuentaAsiento6.Sum(x => x.d_ImporteDolaresD.Value) < CuentaAsiento6.Sum(x => x.d_ImporteDolaresH.Value) ? Utils.Windows.DevuelveValorRedondeado(CuentaAsiento6.Sum(x => x.d_ImporteDolaresD.Value) - CuentaAsiento6.Sum(x => x.d_ImporteDolaresH.Value), 2) * -1 : Utils.Windows.DevuelveValorRedondeado(CuentaAsiento6.Sum(x => x.d_ImporteDolaresD.Value) - CuentaAsiento6.Sum(x => x.d_ImporteDolaresH.Value), 2);
                            _diarioDetalleDto.i_IdCentroCostos = "";
                            _diarioDetalleDto.i_IdTipoDocumento = TipoDocumento;
                            _diarioDetalleDto.i_IdTipoDocumentoRef = TipoDocumento;
                            _diarioDetalleDto.v_Analisis = pAnalisis;
                            _diarioDetalleDto.t_Fecha = DateTime.Parse("31/12/" + pPeriodo);
                            _diarioDetalleDto.v_OrigenDestino = null;
                            _diarioDetalleDto.v_Pedido = null;
                            if (IdRegistroEliminadoAsiento5[0] != null)
                            {
                                _diarioDetalleDto.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                                _diarioDetalleDto.v_NroDocumentoRef = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;

                            }
                            else
                            {

                                _diarioDetalleDto.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                                _diarioDetalleDto.v_NroDocumentoRef = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                            }
                            ListaDetalles.Add(_diarioDetalleDto);
                        }

                        TotDebe = ListaDetalles.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Importe.Value);
                        TotHaber = ListaDetalles.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Importe.Value);
                        TotDebeC = ListaDetalles.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Cambio.Value);
                        TotHaberC = ListaDetalles.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Cambio.Value);
                        _diarioDto.d_TotalDebe = Utils.Windows.DevuelveValorRedondeado(TotDebe, 2);
                        _diarioDto.d_TotalHaber = Utils.Windows.DevuelveValorRedondeado(TotHaber, 2);
                        _diarioDto.d_TotalDebeCambio = Utils.Windows.DevuelveValorRedondeado(TotDebeC, 2);
                        _diarioDto.d_TotalHaberCambio = Utils.Windows.DevuelveValorRedondeado(TotHaberC, 2);
                        _diarioDto.d_DiferenciaDebe = Utils.Windows.DevuelveValorRedondeado(TotDebe - TotHaber, 2);
                        _diarioDto.d_DiferenciaHaber = Utils.Windows.DevuelveValorRedondeado(TotDebeC - TotHaberC, 2);
                        if (ListaDetalles.Count() > 0 && TotDebe != 0 && TotHaber != 0)
                        {
                            _objDiarioBL.InsertarDiario(ref objOperationResult, _diarioDto, Globals.ClientSession.GetAsList(), ListaDetalles, (int)TipoMovimientoTesoreria.Ninguno);

                            objNroDiario = new NroDiario();
                            if (objOperationResult.Success == 1)
                            {
                                objNroDiario.NumeroDiario = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                                objNroDiario.TipoAsiento = 6;
                                ListaDiarioGenerados.Add(objNroDiario);

                            }
                        }
                        Globals.ProgressbarStatus.i_Progress++;
                        #endregion


                        ts.Complete();
                        objOperatioResult.Success = 1;
                        return ListaDiarioGenerados;

                    }

                }
            }
            catch (Exception ex)
            {

                objOperatioResult.Success = 0;
                return null;

            }
        }

        public List<NroDiario> GenerarAsientoApertura(ref OperationResult objOperationResult, int TipoDocumento, string Cuenta14, string pPeriodo, string MesDiario, decimal TipoCambio1, decimal TipoCambio2, decimal TipoCambio3, DateTime fecha, string glosa)
        {
            DiarioBL _objDiarioBL = new DiarioBL();
            List<KeyValueDTO> _ListadoDiarios = new List<KeyValueDTO>();
            List<NroDiario> ListaDiarioGenerados = new List<NroDiario>();
            List<diariodetalleDto> ListaDetalles = new List<diariodetalleDto>();
            NroDiario objNroDiario = new NroDiario();
            string[] IdRegistroEliminadoAsiento1 = new string[3];
            string[] IdRegistroEliminadoAsiento2 = new string[3];
            string[] IdRegistroEliminadoAsiento3 = new string[3];
            diarioDto _diarioDto = new diarioDto();
            diariodetalleDto _diarioDetalleDto = new diariodetalleDto();
            int _MaxMovimiento;
            decimal TotDebe = 0, TotHaber = 0, TotDebeC = 0, TotHaberC = 0;

            using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    Globals.ProgressbarStatus.i_TotalProgress = 6;
                    IdRegistroEliminadoAsiento1 = _objDiarioBL.EliminarDiarioCierreApertura(ref objOperationResult, "AA-1-", Globals.ClientSession.GetAsList(), false, pPeriodo);
                    Globals.ProgressbarStatus.i_Progress++;
                    if (objOperationResult.Success == 0)
                    {
                        Globals.ProgressbarStatus.i_Progress = 6;
                        return null;
                    }
                    IdRegistroEliminadoAsiento2 = _objDiarioBL.EliminarDiarioCierreApertura(ref objOperationResult, "AA-2-", Globals.ClientSession.GetAsList(), false, pPeriodo);
                    if (objOperationResult.Success == 0)
                    {
                        Globals.ProgressbarStatus.i_Progress = 6;
                        return null;
                    }
                    Globals.ProgressbarStatus.i_Progress++;
                    IdRegistroEliminadoAsiento3 = _objDiarioBL.EliminarDiarioCierreApertura(ref objOperationResult, "AA-3-", Globals.ClientSession.GetAsList(), false, pPeriodo);
                    if (objOperationResult.Success == 0)
                    {
                        Globals.ProgressbarStatus.i_Progress = 6;
                        return null;
                    }
                    Globals.ProgressbarStatus.i_Progress++;
                    DateTime FechaInicio = DateTime.Parse("01/01/" + (int.Parse(pPeriodo) - 1).ToString());
                    DateTime FechaFin = DateTime.Parse("31/12/" + (int.Parse(pPeriodo) - 1).ToString() + "  23:59");
                    var AnalisisCuentas = new AsientosContablesBL().ReporteAnalisisCuentas(ref objOperationResult, FechaInicio, FechaFin, "", null, true, 5);

                    #region Asiento1
                    var CuentaAsiento1 = AnalisisCuentas.Where(x => int.Parse(x.CuentaMayor) >= 10 && int.Parse(x.CuentaMayor) <= 59).ToList();
                    _ListadoDiarios = _objDiarioBL.ObtenerListadoDiarioParaAsientoCierre(ref objOperationResult, pPeriodo, MesDiario, TipoDocumento);
                    _MaxMovimiento = _ListadoDiarios.Count() > 0 ? int.Parse(_ListadoDiarios[_ListadoDiarios.Count() - 1].Value1.ToString()) : 0;
                    _MaxMovimiento++;
                    if (IdRegistroEliminadoAsiento1[0] != null)
                    {
                        _diarioDto.v_Periodo = IdRegistroEliminadoAsiento1[0];
                        _diarioDto.v_Mes = IdRegistroEliminadoAsiento1[1];
                        _diarioDto.v_Correlativo = IdRegistroEliminadoAsiento1[2];
                    }
                    else
                    {
                        _diarioDto.v_Periodo = pPeriodo.ToString();
                        _diarioDto.v_Mes = MesDiario.ToString();
                        _diarioDto.v_Correlativo = _MaxMovimiento.ToString("00000000");
                    }

                    _diarioDto.v_IdDocumentoReferencia = "AA-1-" + pPeriodo;
                    _diarioDto.v_Nombre = "ASIENTO 1 : ASIENTO APERTURA EN SALDO SOLES";
                    _diarioDto.v_Glosa = glosa == string.Empty ? _diarioDto.v_Nombre : glosa;
                    _diarioDto.d_TipoCambio = TipoCambio1;
                    _diarioDto.i_IdMoneda = (int)Currency.Soles;
                    _diarioDto.i_IdTipoDocumento = TipoDocumento;
                    _diarioDto.t_Fecha = fecha;
                    _diarioDto.i_IdTipoComprobante = (int)TipoComprobanteLibroDiario.Apertura;

                    var CuentasSinDetalle = CuentaAsiento1.Where(x => !x.CuentaConDetalle && x.IdMonedaCuenta == (int)Currency.Soles); //Se filtra las Cuentas en Soles y sin Detalle
                    foreach (var detalle in CuentasSinDetalle)
                    {

                        _diarioDetalleDto = new diariodetalleDto();
                        _diarioDetalleDto.v_NroCuenta = detalle.CuentaImputable;

                        if (detalle.ImporteSoles > 0)
                        {
                            _diarioDetalleDto.v_Naturaleza = detalle.NaturalezaCuenta;
                            _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(detalle.ImporteSoles.Value, 2);

                        }
                        else
                        {
                            _diarioDetalleDto.v_Naturaleza = detalle.NaturalezaCuenta == "H" ? "D" : "H";
                            _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(detalle.ImporteSoles.Value * -1, 2);

                        }

                        _diarioDetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(_diarioDetalleDto.d_Importe.Value / TipoCambio1, 2);
                        _diarioDetalleDto.i_IdCentroCostos = "";
                        _diarioDetalleDto.i_IdTipoDocumento = TipoDocumento;
                        _diarioDetalleDto.i_IdTipoDocumentoRef = TipoDocumento;
                        _diarioDetalleDto.v_Analisis = "";
                        _diarioDetalleDto.t_Fecha = fecha;
                        _diarioDetalleDto.v_OrigenDestino = null;
                        _diarioDetalleDto.v_Pedido = null;
                        _diarioDetalleDto.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                        _diarioDetalleDto.v_NroDocumentoRef = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;

                        ListaDetalles.Add(_diarioDetalleDto);

                    }

                    var CuentasConDetalle = CuentaAsiento1.Where(x => x.CuentaConDetalle && x.IdMonedaCuenta == (int)Currency.Soles);
                    CuentasConDetalle = CuentasConDetalle.GroupBy(x => new { x.CuentaImputable, x.Detalle, x.TipoDocumento, x.NroDocumento }).ToList().Select(y => y.FirstOrDefault());

                    foreach (var detalle in CuentasConDetalle)
                    {

                        _diarioDetalleDto = new diariodetalleDto();
                        _diarioDetalleDto.v_NroCuenta = detalle.CuentaImputable;
                        if (detalle.ImporteSoles > 0)
                        {
                            _diarioDetalleDto.v_Naturaleza = detalle.NaturalezaCuenta;
                            _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(detalle.ImporteSoles.Value, 2);

                        }
                        else
                        {
                            _diarioDetalleDto.v_Naturaleza = detalle.NaturalezaCuenta == "D" ? "H" : "D";
                            _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(detalle.ImporteSoles.Value * -1, 2);

                        }
                        _diarioDetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(_diarioDetalleDto.d_Importe.Value / TipoCambio1, 2);
                        _diarioDetalleDto.i_IdCentroCostos = "";
                        _diarioDetalleDto.v_IdCliente = string.IsNullOrEmpty(detalle.IdAnexo) ? null : detalle.IdAnexo;
                        _diarioDetalleDto.i_IdTipoDocumento = detalle.TipoDocumento;
                        _diarioDetalleDto.v_NroDocumento = detalle.NroDocumento;
                        _diarioDetalleDto.i_IdTipoDocumentoRef = null;
                        _diarioDetalleDto.v_NroDocumentoRef = null;
                        _diarioDetalleDto.v_Analisis = detalle.NombreRazonSocial;
                        _diarioDetalleDto.t_Fecha = detalle.FechaDocumento;
                        _diarioDetalleDto.v_OrigenDestino = null;
                        _diarioDetalleDto.v_Pedido = null;


                        ListaDetalles.Add(_diarioDetalleDto);
                    }
                    var Asiento5Cierre = _objDiarioBL.BuscarDiaroDocumentoReferencia("AC-5-" + (int.Parse(pPeriodo) - 1).ToString());

                    if (Asiento5Cierre != null)
                    {
                        _diarioDetalleDto = new diariodetalleDto();
                        _diarioDetalleDto.v_NroCuenta = Asiento5Cierre.v_NroCuenta;
                        _diarioDetalleDto.v_Naturaleza = Asiento5Cierre.v_Naturaleza;
                        _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(Asiento5Cierre.d_Importe.Value, 2);
                        _diarioDetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(_diarioDetalleDto.d_Importe.Value / TipoCambio1, 2);
                        _diarioDetalleDto.i_IdCentroCostos = Asiento5Cierre.i_IdCentroCostos;
                        _diarioDetalleDto.v_IdCliente = string.IsNullOrEmpty(Asiento5Cierre.v_IdCliente) ? null : Asiento5Cierre.v_IdCliente;
                        _diarioDetalleDto.i_IdTipoDocumento = TipoDocumento;
                        _diarioDetalleDto.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                        _diarioDetalleDto.v_Analisis = Asiento5Cierre.v_Analisis;
                        _diarioDetalleDto.t_Fecha = Asiento5Cierre.t_Fecha;
                        _diarioDetalleDto.v_OrigenDestino = Asiento5Cierre.v_OrigenDestino;
                        _diarioDetalleDto.v_Pedido = Asiento5Cierre.v_Pedido;
                        _diarioDetalleDto.i_IdTipoDocumentoRef = Asiento5Cierre.i_IdTipoDocumentoRef;
                        _diarioDetalleDto.v_NroDocumentoRef = Asiento5Cierre.v_NroDocumentoRef;
                        ListaDetalles.Add(_diarioDetalleDto);


                    }

                    //Cuenta AJUSTE
                    TotDebe = Utils.Windows.DevuelveValorRedondeado(ListaDetalles.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Importe.Value), 2);
                    TotHaber = Utils.Windows.DevuelveValorRedondeado(ListaDetalles.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Importe.Value), 2);
                    if (TotDebe != TotHaber)
                    {
                        _diarioDetalleDto = new diariodetalleDto();
                        _diarioDetalleDto.v_NroCuenta = Cuenta14;

                        if (TotDebe < TotHaber)
                        {

                            _diarioDetalleDto.v_Naturaleza = "D";
                            _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(TotHaber - TotDebe, 2);
                            _diarioDetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(_diarioDetalleDto.d_Importe.Value / TipoCambio1, 2);

                        }
                        else
                        {
                            _diarioDetalleDto.v_Naturaleza = "H";
                            _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(TotDebe - TotHaber, 2);
                            _diarioDetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(_diarioDetalleDto.d_Importe.Value / TipoCambio1, 2);
                        }
                        _diarioDetalleDto.i_IdCentroCostos = "";
                        _diarioDetalleDto.v_IdCliente = null;
                        _diarioDetalleDto.i_IdTipoDocumento = TipoDocumento;
                        _diarioDetalleDto.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                        _diarioDetalleDto.v_Analisis = "CUENTA DE AJUSTE";
                        _diarioDetalleDto.t_Fecha = fecha;
                        _diarioDetalleDto.v_OrigenDestino = null;
                        _diarioDetalleDto.v_Pedido = null;
                        _diarioDetalleDto.i_IdTipoDocumentoRef = TipoDocumento;
                        _diarioDetalleDto.v_NroDocumentoRef = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                        ListaDetalles.Add(_diarioDetalleDto);

                    }

                    TotDebe = ListaDetalles.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Importe.Value);
                    TotHaber = ListaDetalles.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Importe.Value);
                    TotDebeC = ListaDetalles.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Cambio.Value);
                    TotHaberC = ListaDetalles.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Cambio.Value);
                    _diarioDto.d_TotalDebe = Utils.Windows.DevuelveValorRedondeado(TotDebe, 2);
                    _diarioDto.d_TotalHaber = Utils.Windows.DevuelveValorRedondeado(TotHaber, 2);
                    _diarioDto.d_TotalDebeCambio = Utils.Windows.DevuelveValorRedondeado(TotDebeC, 2);
                    _diarioDto.d_TotalHaberCambio = Utils.Windows.DevuelveValorRedondeado(TotHaberC, 2);
                    _diarioDto.d_DiferenciaDebe = Utils.Windows.DevuelveValorRedondeado(TotDebe - TotHaber, 2);
                    _diarioDto.d_DiferenciaHaber = Utils.Windows.DevuelveValorRedondeado(TotDebeC - TotHaberC, 2);

                    if (ListaDetalles.Count() > 0)
                    {
                        _objDiarioBL.InsertarDiario(ref objOperationResult, _diarioDto, Globals.ClientSession.GetAsList(), ListaDetalles, (int)TipoMovimientoTesoreria.Ninguno);

                        if (objOperationResult.Success == 1)
                        {
                            objNroDiario.NumeroDiario = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                            objNroDiario.TipoAsiento = 1;
                            ListaDiarioGenerados.Add(objNroDiario);
                        }
                        else
                        {
                            objOperationResult.Success = 0;
                            return null;
                        }
                    }
                    else
                    {
                        _MaxMovimiento = _MaxMovimiento - 1;
                    }
                    Globals.ProgressbarStatus.i_Progress++;
                    #endregion

                    #region Asiento2
                    _MaxMovimiento = _MaxMovimiento + 1;
                    ListaDetalles = new List<diariodetalleDto>();
                    var diario2 = new diarioDto();
                    diario2 = (diarioDto)_diarioDto.Clone();
                    diario2.v_Nombre = "ASIENTO 2 : ASIENTO APERTURA SALDOS DEL ACTIVO EN DOLARES";
                    diario2.v_Glosa = glosa == string.Empty ? diario2.v_Nombre : glosa;
                    diario2.d_TipoCambio = TipoCambio2;
                    diario2.v_IdDocumentoReferencia = "AA-2-" + pPeriodo;
                    diario2.i_IdMoneda = (int)Currency.Dolares;
                    if (IdRegistroEliminadoAsiento2[0] != null)
                    {

                        diario2.v_Periodo = IdRegistroEliminadoAsiento2[0];
                        diario2.v_Mes = IdRegistroEliminadoAsiento2[1];
                        diario2.v_Correlativo = IdRegistroEliminadoAsiento2[2];
                    }
                    else
                    {
                        diario2.v_Periodo = pPeriodo.ToString();
                        diario2.v_Mes = MesDiario.ToString();
                        diario2.v_Correlativo = _MaxMovimiento.ToString("00000000");
                    }

                    var CuentaAsiento2 = AnalisisCuentas.Where(x => int.Parse(x.CuentaMayor) >= 10 && int.Parse(x.CuentaMayor) <= 39).ToList();
                    var CuentasSinDetalleAsiento2 = CuentaAsiento2.Where(x => !x.CuentaConDetalle && x.IdMonedaCuenta == (int)Currency.Dolares); //Se filtra las Cuentas en Soles y sin Detalle

                    foreach (var detalle in CuentasSinDetalleAsiento2)
                    {

                        _diarioDetalleDto = new diariodetalleDto();
                        _diarioDetalleDto.v_NroCuenta = detalle.CuentaImputable;


                        if (detalle.ImporteDolares > 0)
                        {
                            _diarioDetalleDto.v_Naturaleza = detalle.NaturalezaCuenta;
                            _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(detalle.ImporteDolares.Value, 2);

                        }
                        else
                        {
                            _diarioDetalleDto.v_Naturaleza = detalle.NaturalezaCuenta == "D" ? "H" : "D";
                            _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(detalle.ImporteDolares.Value * -1, 2);

                        }
                        _diarioDetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(_diarioDetalleDto.d_Importe.Value * TipoCambio2, 2);

                        _diarioDetalleDto.i_IdCentroCostos = "";
                        _diarioDetalleDto.i_IdTipoDocumento = TipoDocumento;
                        _diarioDetalleDto.i_IdTipoDocumentoRef = TipoDocumento;
                        _diarioDetalleDto.v_Analisis = "";
                        _diarioDetalleDto.t_Fecha = fecha;
                        _diarioDetalleDto.v_OrigenDestino = null;
                        _diarioDetalleDto.v_Pedido = null;
                        _diarioDetalleDto.v_NroDocumento = diario2.v_Mes + "-" + diario2.v_Correlativo;
                        _diarioDetalleDto.v_NroDocumentoRef = diario2.v_Mes + "-" + diario2.v_Correlativo;

                        ListaDetalles.Add(_diarioDetalleDto);
                    }

                    var CuentasConDetalleAsiento2 = CuentaAsiento2.Where(x => x.CuentaConDetalle && x.IdMonedaCuenta == (int)Currency.Dolares);
                    CuentasConDetalleAsiento2 = CuentasConDetalleAsiento2.GroupBy(x => new { x.CuentaImputable, x.Detalle, x.TipoDocumento, x.NroDocumento }).ToList().Select(y => y.FirstOrDefault());

                    foreach (var detalle in CuentasConDetalleAsiento2)
                    {

                        _diarioDetalleDto = new diariodetalleDto();
                        _diarioDetalleDto.v_NroCuenta = detalle.CuentaImputable;
                        if (detalle.ImporteDolares > 0)
                        {
                            _diarioDetalleDto.v_Naturaleza = detalle.NaturalezaCuenta;
                            _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(detalle.ImporteDolares.Value, 2);

                        }
                        else
                        {
                            _diarioDetalleDto.v_Naturaleza = detalle.NaturalezaCuenta == "D" ? "H" : "D";
                            _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(detalle.ImporteDolares.Value * -1, 2);

                        }
                        _diarioDetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(_diarioDetalleDto.d_Importe.Value * TipoCambio2, 2);
                        _diarioDetalleDto.i_IdCentroCostos = "";
                        _diarioDetalleDto.v_IdCliente = string.IsNullOrEmpty(detalle.IdAnexo) ? null : detalle.IdAnexo;
                        _diarioDetalleDto.i_IdTipoDocumento = detalle.TipoDocumento;
                        _diarioDetalleDto.v_NroDocumento = detalle.NroDocumento;
                        _diarioDetalleDto.i_IdTipoDocumentoRef = null;
                        _diarioDetalleDto.v_NroDocumentoRef = null;
                        _diarioDetalleDto.v_Analisis = detalle.NombreRazonSocial;
                        _diarioDetalleDto.t_Fecha = detalle.FechaDocumento;
                        _diarioDetalleDto.v_OrigenDestino = null;
                        _diarioDetalleDto.v_Pedido = null;
                        ListaDetalles.Add(_diarioDetalleDto);
                    }
                    //Cuenta AJUSTE

                    TotDebe = Utils.Windows.DevuelveValorRedondeado(ListaDetalles.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Importe.Value), 2);
                    TotHaber = Utils.Windows.DevuelveValorRedondeado(ListaDetalles.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Importe.Value), 2);
                    if (TotDebe != TotHaber)
                    {
                        _diarioDetalleDto = new diariodetalleDto();
                        _diarioDetalleDto.v_NroCuenta = Cuenta14;
                        if (TotDebe < TotHaber)
                        {
                            _diarioDetalleDto.v_Naturaleza = "D";
                            _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(TotHaber - TotDebe, 2);
                            _diarioDetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(_diarioDetalleDto.d_Importe.Value * TipoCambio2, 2);

                        }
                        else
                        {
                            _diarioDetalleDto.v_Naturaleza = "H";
                            _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(TotDebe - TotHaber, 2);
                            _diarioDetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(_diarioDetalleDto.d_Importe.Value * TipoCambio2, 2);

                        }
                        _diarioDetalleDto.i_IdCentroCostos = "";
                        _diarioDetalleDto.v_IdCliente = null;
                        _diarioDetalleDto.i_IdTipoDocumento = TipoDocumento;
                        _diarioDetalleDto.v_NroDocumento = diario2.v_Mes + "-" + diario2.v_Correlativo;
                        _diarioDetalleDto.v_Analisis = "CUENTA DE AJUSTE";
                        _diarioDetalleDto.t_Fecha = fecha;
                        _diarioDetalleDto.v_OrigenDestino = null;
                        _diarioDetalleDto.v_Pedido = null;

                        _diarioDetalleDto.i_IdTipoDocumentoRef = TipoDocumento;
                        _diarioDetalleDto.v_NroDocumentoRef = diario2.v_Mes + "-" + diario2.v_Correlativo;
                        ListaDetalles.Add(_diarioDetalleDto);

                    }

                    TotDebe = ListaDetalles.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Importe.Value);
                    TotHaber = ListaDetalles.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Importe.Value);
                    TotDebeC = ListaDetalles.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Cambio.Value);
                    TotHaberC = ListaDetalles.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Cambio.Value);
                    diario2.d_TotalDebe = Utils.Windows.DevuelveValorRedondeado(TotDebe, 2);
                    diario2.d_TotalHaber = Utils.Windows.DevuelveValorRedondeado(TotHaber, 2);
                    diario2.d_TotalDebeCambio = Utils.Windows.DevuelveValorRedondeado(TotDebeC, 2);
                    diario2.d_TotalHaberCambio = Utils.Windows.DevuelveValorRedondeado(TotHaberC, 2);
                    diario2.d_DiferenciaDebe = Utils.Windows.DevuelveValorRedondeado(TotDebe - TotHaber, 2);
                    diario2.d_DiferenciaHaber = Utils.Windows.DevuelveValorRedondeado(TotDebeC - TotHaberC, 2);

                    if (ListaDetalles.Count() > 0)
                    {
                        _objDiarioBL.InsertarDiario(ref objOperationResult, diario2, Globals.ClientSession.GetAsList(), ListaDetalles, (int)TipoMovimientoTesoreria.Ninguno);
                        if (objOperationResult.Success == 1)
                        {
                            objNroDiario = new NroDiario();
                            objNroDiario.NumeroDiario = diario2.v_Mes + "-" + diario2.v_Correlativo;
                            objNroDiario.TipoAsiento = 2;
                            ListaDiarioGenerados.Add(objNroDiario);

                        }
                        else
                        {
                            objOperationResult.Success = 0;
                            return null;
                        }
                    }
                    else
                    {
                        _MaxMovimiento = _MaxMovimiento - 1;
                    }
                    Globals.ProgressbarStatus.i_Progress++;

                    #endregion

                    #region Asiento3
                    _MaxMovimiento = _MaxMovimiento + 1;
                    ListaDetalles = new List<diariodetalleDto>();
                    var diario3 = new diarioDto();
                    diario3 = (diarioDto)_diarioDto.Clone();
                    diario3.v_Nombre = "ASIENTO 3 : ASIENTO APERTURA SALDOS DEL PASIVO EN DOLARES";
                    diario3.v_Glosa = glosa == string.Empty ? diario2.v_Nombre : glosa;
                    diario3.d_TipoCambio = TipoCambio3;
                    diario3.v_IdDocumentoReferencia = "AA-3-" + pPeriodo;
                    diario3.i_IdMoneda = (int)Currency.Dolares;
                    if (IdRegistroEliminadoAsiento3[0] != null)
                    {

                        diario3.v_Periodo = IdRegistroEliminadoAsiento3[0];
                        diario3.v_Mes = IdRegistroEliminadoAsiento3[1];
                        diario3.v_Correlativo = IdRegistroEliminadoAsiento3[2];
                    }
                    else
                    {
                        diario3.v_Periodo = pPeriodo.ToString();
                        diario3.v_Mes = MesDiario.ToString();
                        diario3.v_Correlativo = _MaxMovimiento.ToString("00000000");
                    }

                    var CuentaAsiento3 = AnalisisCuentas.Where(x => int.Parse(x.CuentaMayor) >= 40 && int.Parse(x.CuentaMayor) <= 49).ToList();
                    var CuentasSinDetalleAsiento3 = CuentaAsiento3.Where(x => !x.CuentaConDetalle && x.IdMonedaCuenta == (int)Currency.Dolares); //Se filtra las Cuentas en Soles y sin Detalle
                    foreach (var detalle in CuentasSinDetalleAsiento3)
                    {

                        _diarioDetalleDto = new diariodetalleDto();
                        _diarioDetalleDto.v_NroCuenta = detalle.CuentaImputable;
                        if (detalle.ImporteDolares > 0)
                        {

                            _diarioDetalleDto.v_Naturaleza = detalle.NaturalezaCuenta;
                            _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(detalle.ImporteDolares.Value, 2);
                        }
                        else
                        {
                            _diarioDetalleDto.v_Naturaleza = detalle.NaturalezaCuenta == "D" ? "H" : "D";
                            _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(detalle.ImporteDolares.Value * -1, 2);
                        }
                        _diarioDetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(_diarioDetalleDto.d_Importe.Value * TipoCambio3, 2);
                        _diarioDetalleDto.i_IdCentroCostos = "";
                        _diarioDetalleDto.i_IdTipoDocumento = TipoDocumento;
                        _diarioDetalleDto.i_IdTipoDocumentoRef = TipoDocumento;
                        _diarioDetalleDto.v_Analisis = "";
                        _diarioDetalleDto.t_Fecha = fecha;
                        _diarioDetalleDto.v_OrigenDestino = null;
                        _diarioDetalleDto.v_Pedido = null;
                        _diarioDetalleDto.v_NroDocumento = diario3.v_Mes + "-" + diario3.v_Correlativo;
                        _diarioDetalleDto.v_NroDocumentoRef = diario3.v_Mes + "-" + diario3.v_Correlativo;

                        ListaDetalles.Add(_diarioDetalleDto);
                    }

                    var CuentasConDetalleAsiento3 = CuentaAsiento3.Where(x => x.CuentaConDetalle && x.IdMonedaCuenta == (int)Currency.Dolares);
                    CuentasConDetalleAsiento3 = CuentasConDetalleAsiento3.GroupBy(x => new { x.CuentaImputable, x.Detalle, x.TipoDocumento, x.NroDocumento }).ToList().Select(y => y.FirstOrDefault());

                    foreach (var detalle in CuentasConDetalleAsiento3)
                    {

                        _diarioDetalleDto = new diariodetalleDto();
                        _diarioDetalleDto.v_NroCuenta = detalle.CuentaImputable;

                        if (detalle.ImporteDolares > 0)
                        {

                            _diarioDetalleDto.v_Naturaleza = detalle.NaturalezaCuenta;
                            _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(detalle.ImporteDolares.Value, 2);

                        }
                        else
                        {
                            _diarioDetalleDto.v_Naturaleza = detalle.NaturalezaCuenta == "D" ? "H" : "D";
                            _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(detalle.ImporteDolares.Value * -1, 2);

                        }
                        _diarioDetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(_diarioDetalleDto.d_Importe.Value * TipoCambio3, 2);


                        _diarioDetalleDto.i_IdCentroCostos = "";
                        _diarioDetalleDto.v_IdCliente = string.IsNullOrEmpty(detalle.IdAnexo) ? null : detalle.IdAnexo;
                        _diarioDetalleDto.i_IdTipoDocumento = detalle.TipoDocumento;
                        _diarioDetalleDto.v_NroDocumento = detalle.NroDocumento;
                        _diarioDetalleDto.i_IdTipoDocumentoRef = null;
                        _diarioDetalleDto.v_NroDocumentoRef = null;
                        _diarioDetalleDto.v_Analisis = detalle.NombreRazonSocial;
                        _diarioDetalleDto.t_Fecha = detalle.FechaDocumento;
                        _diarioDetalleDto.v_OrigenDestino = null;
                        _diarioDetalleDto.v_Pedido = null;


                        ListaDetalles.Add(_diarioDetalleDto);
                    }

                    //Cuenta AJUSTE
                    TotDebe = Utils.Windows.DevuelveValorRedondeado(ListaDetalles.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Importe.Value), 2);
                    TotHaber = Utils.Windows.DevuelveValorRedondeado(ListaDetalles.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Importe.Value), 2);
                    if (TotDebe != TotHaber)
                    {
                        _diarioDetalleDto = new diariodetalleDto();
                        _diarioDetalleDto.v_NroCuenta = Cuenta14;

                        if (TotDebe < TotHaber)
                        {

                            _diarioDetalleDto.v_Naturaleza = "D";

                            _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(TotHaber - TotDebe, 2);
                            _diarioDetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(_diarioDetalleDto.d_Importe.Value * TipoCambio3, 2);
                        }
                        else
                        {
                            _diarioDetalleDto.v_Naturaleza = "H";
                            _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(TotDebe - TotHaber, 2);
                            _diarioDetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(_diarioDetalleDto.d_Importe.Value * TipoCambio3, 2);

                        }
                        _diarioDetalleDto.i_IdCentroCostos = "";
                        _diarioDetalleDto.v_IdCliente = null;
                        _diarioDetalleDto.i_IdTipoDocumento = TipoDocumento;
                        _diarioDetalleDto.v_NroDocumento = diario3.v_Mes + "-" + diario3.v_Correlativo;
                        _diarioDetalleDto.v_Analisis = "CUENTA DE AJUSTE";
                        _diarioDetalleDto.t_Fecha = fecha;
                        _diarioDetalleDto.v_OrigenDestino = null;
                        _diarioDetalleDto.v_Pedido = null;

                        _diarioDetalleDto.i_IdTipoDocumentoRef = TipoDocumento;
                        _diarioDetalleDto.v_NroDocumentoRef = diario3.v_Mes + "-" + diario3.v_Correlativo;
                        ListaDetalles.Add(_diarioDetalleDto);

                    }



                    TotDebe = ListaDetalles.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Importe.Value);
                    TotHaber = ListaDetalles.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Importe.Value);
                    TotDebeC = ListaDetalles.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Cambio.Value);
                    TotHaberC = ListaDetalles.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Cambio.Value);
                    diario3.d_TotalDebe = Utils.Windows.DevuelveValorRedondeado(TotDebe, 2);
                    diario3.d_TotalHaber = Utils.Windows.DevuelveValorRedondeado(TotHaber, 2);
                    diario3.d_TotalDebeCambio = Utils.Windows.DevuelveValorRedondeado(TotDebeC, 2);
                    diario3.d_TotalHaberCambio = Utils.Windows.DevuelveValorRedondeado(TotHaberC, 2);
                    diario3.d_DiferenciaDebe = Utils.Windows.DevuelveValorRedondeado(TotDebe - TotHaber, 2);
                    diario3.d_DiferenciaHaber = Utils.Windows.DevuelveValorRedondeado(TotDebeC - TotHaberC, 2);

                    if (ListaDetalles.Count() > 0)
                    {
                        _objDiarioBL.InsertarDiario(ref objOperationResult, diario3, Globals.ClientSession.GetAsList(), ListaDetalles, (int)TipoMovimientoTesoreria.Ninguno);

                        if (objOperationResult.Success == 1)
                        {
                            objNroDiario = new NroDiario();
                            objNroDiario.NumeroDiario = diario3.v_Mes + "-" + diario3.v_Correlativo;
                            objNroDiario.TipoAsiento = 3;
                            ListaDiarioGenerados.Add(objNroDiario);
                        }
                        else
                        {

                            objOperationResult.Success = 0;
                            return null;
                        }
                    }
                    else
                    {
                        _MaxMovimiento = _MaxMovimiento - 1;
                    }
                    Globals.ProgressbarStatus.i_Progress++;

                    #endregion

                    ts.Complete();
                }
            }


            return ListaDiarioGenerados;
        }


        public string ValidarCuentasLinea(ref OperationResult objOperationResult, DateTime FechaInicio, DateTime FechaFinal)
        {

            string Lineas = "";
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                var DetallesNotaSalida = (from a in dbContext.movimientodetalle

                                          join b in dbContext.movimiento on new { m = a.v_IdMovimiento, eliminado = 0 } equals new { m = b.v_IdMovimiento, eliminado = b.i_Eliminado.Value } into b_join
                                          from b in b_join.DefaultIfEmpty()

                                          join c in dbContext.productodetalle on new { p = a.v_IdProductoDetalle, eliminado = 0 } equals new { p = c.v_IdProductoDetalle, eliminado = c.i_Eliminado.Value } into c_join

                                          from c in c_join.DefaultIfEmpty()

                                          join d in dbContext.producto on new { pd = c.v_IdProducto, eliminado = 0 } equals new { pd = d.v_IdProducto, eliminado = d.i_Eliminado.Value } into d_join

                                          from d in d_join.DefaultIfEmpty()


                                          join e in dbContext.linea on new { l = d.v_IdLinea, eliminado = 0 } equals new { l = e.v_IdLinea, eliminado = e.i_Eliminado.Value } into e_join

                                          from e in e_join.DefaultIfEmpty()

                                          where b.t_Fecha >= FechaInicio && b.t_Fecha >= FechaInicio

                                          && b.i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeSalida

                                          && b.v_OrigenTipo == "V"

                                          select new AsientoConsumo
                                          {
                                              CuentaDebe = e.v_NroCuentaDConsumo,
                                              CuentaHaber = e.v_NroCuentaHConsumo,
                                              Linea = e.v_Nombre,

                                          }).ToList();


                if (DetallesNotaSalida.Any())
                {


                    var CuentasDebe = DetallesNotaSalida.Where(x => x.CuentaDebe == null || x.CuentaDebe == string.Empty).ToList().Select(x => x.Linea).Distinct();
                    var CuentasHaber = DetallesNotaSalida.Where(x => x.CuentaHaber == null || x.CuentaHaber == string.Empty).ToList().Select(x => x.Linea).Distinct();

                    if (CuentasDebe.Concat(CuentasHaber).Any())
                    {
                        foreach (var item in CuentasDebe)
                        {
                            Lineas = Lineas + "," + item;
                        }

                    }

                }

                return Lineas;

            }


        }
        public IdDiarioAsientosInventarios GenerarAsientoConsumo(ref OperationResult objOperationResult, DateTime FechaInicio, DateTime FechaFinal, DateTime FechaDiario, bool TomarCuentaNroPedido)
        {

            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                IdDiarioAsientosInventarios IdDiarios = new IdDiarioAsientosInventarios();
                DiarioBL _objDiarioBL = new DiarioBL();
                AlmacenBL _objAlmacenBL = new AlmacenBL();
                List<KeyValueDTO> _ListadoDiarios = new List<KeyValueDTO>();
                ProductoBL _objProductoBL = new ProductoBL();
                objOperationResult.Success = 1;
                EstablecimientoBL objEstablecimientoBL = new EstablecimientoBL();

                string _strFilterExpression = null, _whereAlmacenesConcatenados = string.Empty, _AlmacenesConcatenados = string.Empty; ;
                _AlmacenesConcatenados = string.Empty;
                List<KeyValueDTO> Almacenes = new List<KeyValueDTO>();
                DateTime FechaInicioValorizar = DateTime.Parse("01/01/" + Globals.ClientSession.i_Periodo.ToString());
                Almacenes = objEstablecimientoBL.GetAlmacenesXEstablecimiento(Globals.ClientSession.i_IdEstablecimiento.Value);
                List<string> Filters = new List<string>();
                if (Almacenes.Count > 0)
                {
                    foreach (var item in Almacenes)
                    {
                        _whereAlmacenesConcatenados = _whereAlmacenesConcatenados + "IdAlmacen==" + item.Id + " || ";
                        _AlmacenesConcatenados = _AlmacenesConcatenados + item.Value1 + "/ ";
                    }
                    _whereAlmacenesConcatenados = _whereAlmacenesConcatenados.Substring(0, _whereAlmacenesConcatenados.Length - 4);
                    //_AlmacenesConcatenados = cboEstablecimiento.Text + " / " + _AlmacenesConcatenados.Substring(0, _AlmacenesConcatenados.Length - 2);
                    _AlmacenesConcatenados = _AlmacenesConcatenados.Substring(0, _AlmacenesConcatenados.Length - 2);
                }

                Filters.Add("(" + _whereAlmacenesConcatenados + ")");

                if (Filters.Count > 0)
                {
                    foreach (string item in Filters)
                    {
                        _strFilterExpression = _strFilterExpression + item + " && ";
                    }
                    _strFilterExpression = _strFilterExpression.Substring(0, _strFilterExpression.Length - 4);
                }

                List<ReporteListadoSalidaAlmacenAnalitico> AsientosDetalles = _objAlmacenBL.ReporteAsientoConsumoNuevo(ref  objOperationResult, FechaInicio, FechaFinal, _strFilterExpression, false, "", TomarCuentaNroPedido);

                #region Genera Asiento Consumo
                diarioDto _diarioDto = new diarioDto();
                diariodetalleDto _diarioDetalleDto = new diariodetalleDto();
                List<diariodetalleDto> ListaDetalle = new List<diariodetalleDto>();


                _ListadoDiarios = _objDiarioBL.ObtenerListadoDiario(ref objOperationResult, FechaDiario.Date.Year.ToString(), FechaDiario.Date.Month.ToString("00"), (int)LibroDiarios.DiarioNormal);
                int _MaxMovimiento = _ListadoDiarios.Count() > 0 ? int.Parse(_ListadoDiarios[_ListadoDiarios.Count() - 1].Value1.ToString()) : 0;
                _MaxMovimiento++;
                _diarioDto.v_IdDocumentoReferencia = "AC-" + FechaDiario.Date.Month.ToString("00") + FechaDiario.Date.Year.ToString();
                _diarioDto.v_Periodo = FechaDiario.Date.Year.ToString();
                _diarioDto.v_Mes = FechaDiario.Date.Month.ToString("00");
                _diarioDto.v_Nombre = "CONSUMO DE ALMACÉN";
                _diarioDto.v_Glosa = "CONSUMO DE ALMACÉN";
                _diarioDto.v_Correlativo = _MaxMovimiento.ToString("00000000");
                _diarioDto.d_TipoCambio = 1;
                _diarioDto.i_IdMoneda = (int)Currency.Soles;
                _diarioDto.i_IdTipoDocumento = (int)LibroDiarios.DiarioNormal;
                _diarioDto.t_Fecha = FechaDiario;
                _diarioDto.i_IdTipoComprobante = 2;
                // foreach (var debe in DebeNotasSalida)
                foreach (var debe in AsientosDetalles.Where(x => x.CuentaDebe != null))
                {

                    _diarioDetalleDto = new diariodetalleDto();
                    _diarioDetalleDto.v_NroCuenta = debe.CuentaDebe;
                    _diarioDetalleDto.v_Naturaleza = "D";
                    _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(debe.DebeSoles.Value, 2);
                    _diarioDetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(debe.DebeDolares.Value, 2);
                    _diarioDetalleDto.i_IdCentroCostos = "";
                    _diarioDetalleDto.i_IdTipoDocumento = (int)LibroDiarios.DiarioNormal;
                    _diarioDetalleDto.v_NroDocumento = _diarioDto.v_Mes.Trim() + "-" + _diarioDto.v_Correlativo;
                    _diarioDetalleDto.i_IdTipoDocumentoRef = -1;
                    _diarioDetalleDto.v_NroDocumentoRef = String.Empty;
                    _diarioDetalleDto.v_Analisis = "CONSUMO DE ALMACÉN";
                    _diarioDetalleDto.t_Fecha = FechaDiario;
                    _diarioDetalleDto.v_OrigenDestino = null;
                    _diarioDetalleDto.v_Pedido = null;
                    ListaDetalle.Add(_diarioDetalleDto);

                    if (debe.HaberSoles != 0)
                    {

                        _diarioDetalleDto = new diariodetalleDto();
                        _diarioDetalleDto.v_NroCuenta = debe.CuentaDebe;
                        _diarioDetalleDto.v_Naturaleza = "H";
                        _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(debe.HaberSoles.Value, 2);
                        _diarioDetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(debe.HaberDolares.Value, 2);
                        _diarioDetalleDto.i_IdCentroCostos = "";
                        _diarioDetalleDto.i_IdTipoDocumento = (int)LibroDiarios.DiarioNormal;
                        _diarioDetalleDto.v_NroDocumento = _diarioDto.v_Mes.Trim() + "-" + _diarioDto.v_Correlativo;
                        _diarioDetalleDto.i_IdTipoDocumentoRef = -1;
                        _diarioDetalleDto.v_NroDocumentoRef = String.Empty;
                        _diarioDetalleDto.v_Analisis = "CONSUMO DE ALMACÉN";
                        _diarioDetalleDto.t_Fecha = FechaDiario;
                        _diarioDetalleDto.v_OrigenDestino = null;
                        _diarioDetalleDto.v_Pedido = null;
                        ListaDetalle.Add(_diarioDetalleDto);

                    }

                }


                foreach (var haber in AsientosDetalles.Where(k => k.CuentaHaber != null))
                {

                    _diarioDetalleDto = new diariodetalleDto();
                    _diarioDetalleDto.v_NroCuenta = haber.CuentaHaber;
                    _diarioDetalleDto.v_Naturaleza = "H";
                    _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(haber.HaberSoles.Value, 2);
                    _diarioDetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(haber.HaberDolares.Value, 2);
                    _diarioDetalleDto.i_IdCentroCostos = "";
                    _diarioDetalleDto.i_IdTipoDocumento = (int)LibroDiarios.DiarioNormal;
                    _diarioDetalleDto.v_NroDocumento = _diarioDto.v_Mes.Trim() + "-" + _diarioDto.v_Correlativo;
                    _diarioDetalleDto.i_IdTipoDocumentoRef = -1;
                    _diarioDetalleDto.v_NroDocumentoRef = String.Empty;
                    _diarioDetalleDto.v_Analisis = "CONSUMO DE ALMACÉN";
                    _diarioDetalleDto.t_Fecha = FechaDiario;
                    _diarioDetalleDto.v_OrigenDestino = null;
                    _diarioDetalleDto.v_Pedido = null;
                    ListaDetalle.Add(_diarioDetalleDto);



                    if (haber.DebeSoles != 0)
                    {

                        _diarioDetalleDto = new diariodetalleDto();
                        _diarioDetalleDto.v_NroCuenta = haber.CuentaHaber;
                        _diarioDetalleDto.v_Naturaleza = "D";
                        _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(haber.DebeSoles.Value, 2);
                        _diarioDetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(haber.DebeDolares.Value, 2);
                        _diarioDetalleDto.i_IdCentroCostos = "";
                        _diarioDetalleDto.i_IdTipoDocumento = (int)LibroDiarios.DiarioNormal;
                        _diarioDetalleDto.v_NroDocumento = _diarioDto.v_Mes.Trim() + "-" + _diarioDto.v_Correlativo;
                        _diarioDetalleDto.i_IdTipoDocumentoRef = -1;
                        _diarioDetalleDto.v_NroDocumentoRef = String.Empty;
                        _diarioDetalleDto.v_Analisis = "CONSUMO DE ALMACÉN";
                        _diarioDetalleDto.t_Fecha = FechaDiario;
                        _diarioDetalleDto.v_OrigenDestino = null;
                        _diarioDetalleDto.v_Pedido = null;
                        ListaDetalle.Add(_diarioDetalleDto);

                    }

                }



                decimal TotDebe = ListaDetalle.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Importe.Value);
                decimal TotHaber = ListaDetalle.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Importe.Value);
                decimal TotDebeC = ListaDetalle.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Cambio.Value);
                decimal TotHaberC = ListaDetalle.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Cambio.Value);
                _diarioDto.d_TotalDebe = Utils.Windows.DevuelveValorRedondeado(TotDebe, 2);
                _diarioDto.d_TotalHaber = Utils.Windows.DevuelveValorRedondeado(TotHaber, 2);
                _diarioDto.d_TotalDebeCambio = Utils.Windows.DevuelveValorRedondeado(TotDebeC, 2);
                _diarioDto.d_TotalHaberCambio = Utils.Windows.DevuelveValorRedondeado(TotHaberC, 2);
                _diarioDto.d_DiferenciaDebe = Utils.Windows.DevuelveValorRedondeado(TotDebe - TotHaber, 2);
                _diarioDto.d_DiferenciaHaber = Utils.Windows.DevuelveValorRedondeado(TotDebeC - TotHaberC, 2);


                var Diario = _objDiarioBL.BuscarDiarioInventarioPorDocReferencia("AC-" + FechaDiario.Date.Month.ToString("00") + FechaDiario.Date.Year.ToString());
                string[] IdRegistroEliminado = new string[3];
                if (Diario.Any())
                {
                    IdRegistroEliminado = _objDiarioBL.EliminarDiarioXDocRef(ref objOperationResult, Diario.FirstOrDefault().v_IdDocumentoReferencia, Globals.ClientSession.GetAsList(), false);
                    _diarioDto.v_Periodo = IdRegistroEliminado[0];
                    _diarioDto.v_Mes = IdRegistroEliminado[1];
                    _diarioDto.v_Correlativo = IdRegistroEliminado[2];
                }

                foreach (var item in ListaDetalle)
                {
                    item.v_NroDocumento = _diarioDto.v_Mes.Trim() + "-" + _diarioDto.v_Correlativo;
                    item.i_IdTipoDocumento = (int)LibroDiarios.DiarioNormal;
                }
                if (ListaDetalle.Any())
                {
                    _objDiarioBL.InsertarDiario(ref objOperationResult, _diarioDto, Globals.ClientSession.GetAsList(), ListaDetalle, (int)TipoMovimientoTesoreria.Ninguno);

                    if (objOperationResult.Success == 1)
                    {

                        IdDiarios.Id = _diarioDto.v_IdDocumentoReferencia;

                    }
                }


                #endregion


                return IdDiarios;
            }
        }

        public List<asientocontableDto> ObtenerCuentasImputablesContables(string periodo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var cuentas =
                        dbContext.asientocontable.Where(
                            p => p.i_Eliminado == 0 && p.i_Imputable == 1 && p.v_Periodo.Equals(periodo));
                    if (cuentas != null)
                        return cuentas.ToList().ToDTOs();
                    else
                        return null;
                }
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
