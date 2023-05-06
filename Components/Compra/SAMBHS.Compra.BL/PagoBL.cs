using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAMBHS.Common.Resource;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.BE;
using System.Linq.Dynamic;
using System.ComponentModel;
using System.Transactions;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Tesoreria.BL;
using SAMBHS.Common.BL;
using SAMBHS.Common.BE.Custom;

namespace SAMBHS.Compra.BL
{
    public class PagoBL
    {
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        private static string periodo = Globals.ClientSession != null ? (Globals.ClientSession.i_Periodo ?? DateTime.Now.Year).ToString() : DateTime.Now.Year.ToString();
        #region Formulario Pago
        public List<GridKeyValueDTO> ListadoFormasPago()
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var Query = (from n in dbContext.datahierarchy
                                 where n.i_GroupId == 46 && n.i_IsDeleted == 0
                                 orderby n.i_Sort
                                 select n).ToList();

                    List<GridKeyValueDTO> Result = Query.Select(p => new GridKeyValueDTO
                    {
                        Id = p.i_ItemId.ToString(),
                        Value1 = p.v_Value1,
                        Value2 = p.v_Value2,
                        Value3 = p.v_Field
                    }).ToList();

                    if (Result != null)
                    {
                        return Result;
                    }
                    else
                    {
                        return null;
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<KeyValueDTO> ObtenerListadoPagos(ref OperationResult pobjOperationResult, string pstringPeriodo, string pstringMes, int IdTipoDocumento)
        {
            try
            {
                SAMBHSEntitiesModelWin dbcontext = new SAMBHSEntitiesModelWin();
                var almacenpredeterminado = Globals.ClientSession.i_IdAlmacenPredeterminado.Value.ToString("00");
                var query = (from n in dbcontext.pago
                             where n.i_Eliminado == 0 && n.v_Periodo == pstringPeriodo && n.v_Mes == pstringMes && n.i_IdTipoDocumento == IdTipoDocumento
                              && n.v_IdPago.Substring(2, 2) == almacenpredeterminado
                             orderby n.v_Correlativo ascending
                             select new
                             {
                                 v_Correlativo = n.v_Correlativo,
                                 // v_IdVenta = n.v_Idpago
                                 v_IdPago = n.v_IdPago,
                             }
                             );
                if (query.Any())
                {
                    var query2 = query.AsEnumerable()
                           .Select(x => new KeyValueDTO
                           {
                               Value1 = x.v_Correlativo,

                               Value2 = x.v_IdPago,

                           }).ToList();
                    return query2;
                }
                else
                {

                    return new List<KeyValueDTO> { new KeyValueDTO { Value1 = almacenpredeterminado + "000000" } };


                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public int DevuelveMonedaPorDocumento(ref OperationResult pobjOperationResult, int CodigoDocumento, out bool TieneCuenta)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    TieneCuenta = true;
                    var documento = dbContext.documento.Where(p => p.i_CodigoDocumento == CodigoDocumento).FirstOrDefault();
                    pobjOperationResult.Success = 1;

                    if (documento != null)
                    {
                        if (documento.v_NroCuenta != null && !string.IsNullOrEmpty(documento.v_NroCuenta.Trim()))
                        {
                            var cuenta = dbContext.asientocontable.Where(p => p.v_NroCuenta == documento.v_NroCuenta &&p.i_Eliminado ==0 && p.v_Periodo ==periodo).FirstOrDefault();

                            if (cuenta != null)
                            {
                                return (int)cuenta.i_IdMoneda;
                            }
                            else
                            {
                                TieneCuenta = false;
                                return -1;
                            }
                        }
                        else
                        {
                            TieneCuenta = false;
                            return -1;
                        }
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                TieneCuenta = true;
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PagoBL.DevuelveMonedaPorDocumento()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = pobjOperationResult.ExceptionMessage != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return -1;
            }
        }

        public List<pagopendienteDto> ListarPagosPendientes(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression, DateTime F_Ini, DateTime F_Fin, bool IncluirLetras, bool SoloLetras = false)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                List<pagopendienteDto> PagosPendientes = new List<pagopendienteDto>();

                if (!SoloLetras)
                {
                    var query = (from n in dbContext.pagopendiente

                                 join A in dbContext.compra on new { IdCompra = n.v_IdCompra, eliminado = 0 } equals new { IdCompra = A.v_IdCompra, eliminado = A.i_Eliminado.Value } into A_join
                                 from A in A_join.DefaultIfEmpty()

                                 join B in dbContext.cliente on new { IdProveedor = A.v_IdProveedor, eliminado = A.i_Eliminado.Value } equals new { IdProveedor = B.v_IdCliente, eliminado = B.i_Eliminado.Value } into B_join
                                 from B in B_join.DefaultIfEmpty()

                                 join J4 in dbContext.documento on new { i_IdTipoDocumento = A.i_IdTipoDocumento.Value, eliminado = 0 }
                                                                equals new { i_IdTipoDocumento = J4.i_CodigoDocumento, eliminado = J4.i_Eliminado.Value } into J4_join
                                 from J4 in J4_join.DefaultIfEmpty()

                                 where n.i_Eliminado == 0 && A.t_FechaRegistro >= F_Ini && A.t_FechaRegistro <= F_Fin && n.d_Saldo > 0
                                 //&& (J4.i_UsadoDocumentoInverso == 0 || J4.i_UsadoDocumentoInverso == null) && A.i_IdEstablecimiento == Globals.ClientSession.i_IdEstablecimiento.Value

                                 select new pagopendienteDto
                                 {
                                     v_IdCompra = A.v_IdCompra,
                                     v_IdProveedor = B.v_IdCliente,
                                     NombreProveedor = A.v_IdProveedor != "N002-CL000000000" ? (B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " + B.v_RazonSocial).Trim() : "",
                                     TipoDocumento = J4.v_Siglas,
                                     i_IdTipoDocumento = A.i_IdTipoDocumento.Value,
                                     NroDocumento = A.v_SerieDocumento + "-" + A.v_CorrelativoDocumento,
                                     FechaRegistro = A.t_FechaRegistro.Value,
                                     Moneda = A.i_IdMoneda.Value == 1 ? "S/." : "US$.",
                                     Importe = A.d_Total.Value,
                                     d_Acuenta = n.d_Acuenta,
                                     d_Saldo = n.d_Saldo,
                                     TipoCambio = A.d_TipoCambio.Value,
                                     ValorCompra = A.d_ValorVenta.Value,
                                     EsLetra = false,
                                     EsDocInverso = J4.i_UsadoDocumentoInverso ?? 0
                                 });


                    if (!string.IsNullOrEmpty(pstrFilterExpression))
                    {
                        query = query.Where(pstrFilterExpression);
                    }
                    if (!string.IsNullOrEmpty(pstrSortExpression))
                    {
                        query = query.OrderBy(pstrSortExpression);
                    }

                    PagosPendientes = query.ToList();
                }

                #region Agrega la consulta de letras pendientes por cobrar al resultado
                if (IncluirLetras || SoloLetras)
                {
                    var query2 = (from n in dbContext.cobranzaletraspagarpendiente

                                  join A in dbContext.letraspagardetalle on n.v_IdLetrasPagarDetalle equals A.v_IdLetrasPagarDetalle into A_join
                                  from A in A_join.DefaultIfEmpty()

                                  join B in dbContext.cliente on A.v_IdProveedor equals B.v_IdCliente into B_join
                                  from B in B_join.DefaultIfEmpty()

                                  join J4 in dbContext.documento on new { i_IdTipoDocumento = A.i_IdTipoDocumento.Value }
                                                                 equals new { i_IdTipoDocumento = J4.i_CodigoDocumento } into J4_join
                                  from J4 in J4_join.DefaultIfEmpty()

                                  where n.i_Eliminado == 0 && A.t_FechaEmision >= F_Ini && A.t_FechaEmision <= F_Fin &&
                                      //  n.d_Saldo > 0 && A.i_IdTipoDocumento != 7 /*&& A.i_IdEstablecimiento == Globals.ClientSession.i_IdEstablecimiento.Value*/
                                         n.d_Saldo > 0 && (J4.i_UsadoDocumentoInverso == 0 || J4.i_UsadoDocumentoInverso == null) /*&& A.i_IdEstablecimiento == Globals.ClientSession.i_IdEstablecimiento.Value*/
                                  select new
                                      {
                                          v_IdVenta = A.v_IdLetrasPagarDetalle,
                                          v_IdCliente = B.v_IdCliente,
                                          NombreCliente = A.v_IdProveedor != "N002-CL000000000" ? (B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " + B.v_RazonSocial).Trim() : "CLIENTE PUBLICO GENERAL",
                                          TipoDocumento = J4.v_Siglas,
                                          i_IdTipoDocumento = A.i_IdTipoDocumento,
                                          NroDocumento = A.v_Serie + "-" + A.v_Correlativo,
                                          FechaRegistro = A.t_FechaEmision.Value,
                                          Moneda = A.i_IdMoneda == 1 ? "S/." : "US$.",
                                          Importe = A.d_Importe.Value,
                                          d_Acuenta = n.d_Acuenta,
                                          d_Saldo = n.d_Saldo,
                                          TipoCambio = A.d_TipoCambio.Value,
                                          ValorVenta = A.d_Importe.Value,
                                          EsLetra = true
                                      }
                                       ).ToList().Select(n => new pagopendienteDto
                                       {
                                           v_IdCompra = n.v_IdVenta,
                                           v_IdProveedor = n.v_IdCliente,
                                           NombreProveedor = n.NombreCliente,
                                           TipoDocumento = n.TipoDocumento,
                                           i_IdTipoDocumento = n.i_IdTipoDocumento,
                                           NroDocumento = n.NroDocumento,
                                           FechaRegistro = n.FechaRegistro,
                                           Moneda = n.Moneda,
                                           Importe = n.Importe,
                                           d_Acuenta = n.d_Acuenta,
                                           d_Saldo = n.d_Saldo,
                                           TipoCambio = n.TipoCambio,
                                           ValorCompra = n.ValorVenta,
                                           EsLetra = n.EsLetra,
                                           Ubicacion = ObtenerUltimaUbicacionLetra(n.v_IdVenta).Siglas,
                                           UbicacionNombreCompleto = ObtenerUltimaUbicacionLetra(n.v_IdVenta).NombreCompleto
                                       }
                                       ).AsQueryable();

                    if (query2 != null)
                    {
                        if (!string.IsNullOrEmpty(pstrFilterExpression))
                        {
                            query2 = query2.Where(pstrFilterExpression);
                        }
                        if (!string.IsNullOrEmpty(pstrSortExpression))
                        {
                            query2 = query2.OrderBy(pstrSortExpression);
                        }

                        List<pagopendienteDto> objData2 = query2.ToList();

                        PagosPendientes = PagosPendientes.Concat(objData2).ToList();
                    }
                }
                #endregion

                pobjOperationResult.Success = 1;
                return PagosPendientes;

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        private Ubicacion ObtenerUltimaUbicacionLetra(string pstrIdLetraDetalle)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    Ubicacion objUbicacion = new Ubicacion();

                    var Consulta = dbContext.letrasmantenimientodetalle.Where(p => p.v_IdLetrasPagarDetalle == pstrIdLetraDetalle && p.i_Eliminado == 0).ToList();

                    if (Consulta.Count > 0)
                    {
                        Consulta = Consulta.OrderBy(p => p.v_IdLetrasMantenimiento).ToList();

                        var UltimoRegistro = Consulta.Last();

                        var Documento = dbContext.documento.Where(p => p.i_CodigoDocumento == UltimoRegistro.i_IdUbicacion).FirstOrDefault();

                        if (Documento != null)
                        {
                            objUbicacion.Siglas = Documento.v_Siglas;
                            objUbicacion.NombreCompleto = Documento.v_Nombre;
                        }
                        else
                        {
                            objUbicacion.Siglas = "*No Encontrado*";
                            objUbicacion.NombreCompleto = "*No Encontrado*";
                        }
                    }
                    else
                    {
                        objUbicacion.Siglas = "CJS";
                        objUbicacion.NombreCompleto = "OFICINA";
                    }

                    return objUbicacion;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public pagoDto ObtenerPagoCabecera(ref OperationResult pobjOperationResult, string pstrIdPago)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var objEntity = (from a in dbContext.pago

                                 where a.v_IdPago == pstrIdPago
                                 select new pagoDto
                                 {

                                     v_IdPago = a.v_IdPago,
                                     v_Periodo = a.v_Periodo,
                                     i_IdTipoDocumento = a.i_IdTipoDocumento,
                                     v_Mes = a.v_Mes,
                                     v_Correlativo = a.v_Correlativo,
                                     i_TipoMovimiento = a.i_TipoMovimiento,
                                     t_FechaRegistro = a.t_FechaRegistro,
                                     d_TipoCambio = a.d_TipoCambio,
                                     i_IdMedioPago = a.i_IdMedioPago,
                                     v_Nombre = a.v_Nombre,
                                     v_Glosa = a.v_Glosa,
                                     i_IdMoneda = a.i_IdMoneda,
                                     i_IdEstado = a.i_IdEstado,
                                     d_TotalSoles = a.d_TotalSoles,
                                     d_TotalDolares = a.d_TotalDolares,
                                     i_Eliminado = a.i_Eliminado,
                                     i_InsertaIdUsuario = a.i_InsertaIdUsuario,
                                     i_ActualizaIdUsuario = a.i_ActualizaIdUsuario,
                                     t_ActualizaFecha = a.t_ActualizaFecha,
                                     t_InsertaFecha = a.t_InsertaFecha,
                                     i_IdEstablecimiento = a.i_IdEstablecimiento
                                 }
                                 ).FirstOrDefault();

                pobjOperationResult.Success = 1;

                return objEntity;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public BindingList<GrillaPagoDetalleDto> ObtenerPagoDetalle(ref OperationResult pobjOperationResult, string pstrIdPago)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = (from a in dbContext.pagodetalle

                             join b in dbContext.compra on new { IdCompra = a.v_IdCompra, eliminado = 0 } equals new { IdCompra = b.v_IdCompra, eliminado = b.i_Eliminado.Value } into b_join

                             from b in b_join.DefaultIfEmpty()

                             join c in dbContext.cliente on new { IdProv = b.v_IdProveedor, eliminado = 0, Flag = "V" } equals new { IdProv = c.v_IdCliente, eliminado = c.i_Eliminado.Value, Flag = c.v_FlagPantalla } into c_join

                             from c in c_join.DefaultIfEmpty()

                             join d in dbContext.documento on new { IdDocumento = b.i_IdTipoDocumento.Value, eliminado = 0 } equals new { IdDocumento = d.i_CodigoDocumento, eliminado = d.i_Eliminado.Value } into d_join

                             from d in d_join.DefaultIfEmpty()

                             join J1 in dbContext.letraspagardetalle on a.v_IdCompra equals J1.v_IdLetrasPagarDetalle into J1_join
                             from J1 in J1_join.DefaultIfEmpty()

                             join J2 in dbContext.cliente on J1.v_IdProveedor equals J2.v_IdCliente into J2_join
                             from J2 in J2_join.DefaultIfEmpty()

                             join J3 in dbContext.documento on J1.i_IdTipoDocumento equals J3.i_CodigoDocumento into J3_join
                             from J3 in J3_join.DefaultIfEmpty()

                             where a.i_Eliminado == 0 && a.v_IdPago == pstrIdPago
                             orderby a.t_InsertaFecha ascending

                             select new GrillaPagoDetalleDto

                             {
                                 v_IdPagoDetalle = a.v_IdPagoDetalle,
                                 v_IdPago = a.v_IdPago,
                                 v_IdCompra = a.v_IdCompra,
                                 v_Observacion = a.v_Observacion,
                                 v_DocumentoRef = a.v_DocumentoRef,
                                 i_IdFormaPago = a.i_IdFormaPago.Value,
                                 i_IdTipoDocumentoRef = a.i_IdTipoDocumentoRef.Value,
                                 i_EsLetra = a.i_EsLetra.Value,
                                 d_ImporteSoles = a.d_ImporteSoles.Value,
                                 d_ImporteDolares = a.d_ImporteDolares.Value,
                                 d_NetoXCobrar = a.d_NetoXCobrar.Value,
                                 i_Eliminado = a.i_Eliminado.Value,
                                 i_InsertaIdUsuario = a.i_InsertaIdUsuario.Value,
                                 t_InsertaFecha = a.t_InsertaFecha.Value,
                                 i_IdMoneda = b != null ? b.i_IdMoneda : J1.i_IdMoneda,
                                 TipoDocumento = d != null ? d.v_Siglas : J3.v_Siglas,
                                 NroDocumento = b != null ? b.v_SerieDocumento + "-" + b.v_CorrelativoDocumento : J1.v_Serie + "-" + J1.v_Correlativo,
                                 NombreProveedor = c != null ? (c.v_ApePaterno + " " + c.v_ApeMaterno + " " + c.v_PrimerNombre + " " + c.v_RazonSocial).Trim() : (J2.v_ApePaterno + " " + J2.v_ApeMaterno + " " + J2.v_PrimerNombre + " " + J2.v_RazonSocial).Trim(),
                                 IdMonedaCompra = b != null ? b.i_IdMoneda : J1.i_IdMoneda,
                                 MonedaOriginal = b != null ? b.i_IdMoneda.Value == 1 ? "S./" : "US$." : J1.i_IdMoneda.Value == 1 ? "S./" : "US$."
                             }
                             ).ToList();

                var Result = new BindingList<GrillaPagoDetalleDto>(query);
                pobjOperationResult.Success = 1;
                return Result;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public string DevolverTipoCambioPorFecha(ref OperationResult pobjOperationResult, DateTime Fecha)
        {
            try
            {
                var query = new TipoCambioBL().DevolverTipoCambioPorFechaCompra(ref pobjOperationResult, Fecha);
                if (pobjOperationResult.Success == 0) return "0";
                return query;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public string[] DevolverNombres(string pstrIdCompra)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                var EntityVenta = (from n in dbContext.compra

                                   join B in dbContext.cliente on new { IdProveedor = n.v_IdProveedor, eliminado = 0 } equals new { IdProveedor = B.v_IdCliente, eliminado = B.i_Eliminado.Value } into B_join
                                   from B in B_join.DefaultIfEmpty()

                                   join J4 in dbContext.documento on new { i_IdTipoDocumento = n.i_IdTipoDocumento.Value }
                                                         equals new { i_IdTipoDocumento = J4.i_CodigoDocumento } into J4_join
                                   from J4 in J4_join.DefaultIfEmpty()

                                   where n.v_IdCompra == pstrIdCompra
                                   select new
                                   {
                                       TipoDocumento = J4.v_Siglas,
                                       NroDocumento = n.v_SerieDocumento + " - " + n.v_CorrelativoDocumento,
                                       NombreCliente = (B.v_PrimerNombre + " " + B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_RazonSocial).Trim(),
                                       IdMoneda = n.i_IdMoneda,
                                       Moneda = n.i_IdMoneda == 1 ? "S/." : "$."
                                   }
                                     ).FirstOrDefault();

                string[] Cadena = new string[5];
                if (EntityVenta != null)
                {
                    Cadena[0] = EntityVenta.TipoDocumento;
                    Cadena[1] = EntityVenta.NroDocumento;
                    Cadena[2] = EntityVenta.NombreCliente.ToString();
                    Cadena[3] = EntityVenta.IdMoneda.ToString();
                    Cadena[4] = EntityVenta.Moneda;
                }

                return Cadena;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool ExisteNroRegistro(string Periodo, string Mes, string Correlativo, int IdTipoDocumento)
        {
            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

            var Registro = (from n in dbContext.pago
                            where n.i_Eliminado == 0 && n.v_Periodo == Periodo && n.v_Mes == Mes && n.v_Correlativo == Correlativo
                                  && n.i_IdTipoDocumento == IdTipoDocumento
                            select n).FirstOrDefault();

            if (Registro == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string InsertarPago(ref OperationResult pobjOperationResult, pagoDto pobjDtoEntity, List<string> ClientSession, List<pagodetalleDto> pTemp_Insertar)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                    SecuentialBL objSecuentialBL = new SecuentialBL();
                    pago objEntityPago = pagoAssembler.ToEntity(pobjDtoEntity);
                    compradetalleDto pobjDtoPagoDetalle = new compradetalleDto();
                    DocumentoBL _objDocumentoBL = new DocumentoBL();
                    //compradetalleDto pobjDtoPagoDetalle = new compradetalleDto();
                    int SecuentialId = 0;
                    string newIdPago = string.Empty;
                    string newIdPagoDetalle = string.Empty;
                    int intNodeId;
                    #region Inserta Cabecera
                    objEntityPago.t_InsertaFecha = DateTime.Now;
                    objEntityPago.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntityPago.i_Eliminado = 0;
                    objEntityPago.i_IdEstablecimiento = Globals.ClientSession.i_IdEstablecimiento.Value;
                    intNodeId = int.Parse(ClientSession[0]);
                    SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 79);
                    newIdPago = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "PB");
                    objEntityPago.v_IdPago = newIdPago;
                    dbContext.AddTopago(objEntityPago);
                    dbContext.SaveChanges();
                    #endregion

                    #region Inserta Detalle
                    foreach (pagodetalleDto pagodetalleDto in pTemp_Insertar)
                    {
                        pagodetalle objEntityPagoDetalle = pagodetalleAssembler.ToEntity(pagodetalleDto);
                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 81);
                        newIdPagoDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "PD");
                        objEntityPagoDetalle.v_IdPagoDetalle = newIdPagoDetalle;
                        objEntityPagoDetalle.v_IdPago = newIdPago;
                        objEntityPagoDetalle.t_InsertaFecha = DateTime.Now;
                        objEntityPagoDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntityPagoDetalle.i_Eliminado = 0;
                        dbContext.AddTopagodetalle(objEntityPagoDetalle);
                        #region Actualiza PagoPendiente
                        if (pagodetalleDto.i_EsLetra == 0)
                        {
                            ProcesaDetallePago(ref pobjOperationResult, pagodetalleAssembler.ToDTO(objEntityPagoDetalle), pobjDtoEntity, ClientSession);
                            if (pobjOperationResult.Success == 0) return null;
                        }
                        else
                        {
                            ProcesaDetallePagoLetras(ref pobjOperationResult, pagodetalleAssembler.ToDTO(objEntityPagoDetalle), pobjDtoEntity, ClientSession);
                            if (pobjOperationResult.Success == 0) return null;
                        }

                        #endregion

                        dbContext.SaveChanges();
                    }
                    #endregion

                    #region Genera Tesorería

                    TesoreriaBL _objTesoreriaBL = new TesoreriaBL();
                    tesoreriaDto C_tesoreriaDto = new tesoreriaDto();
                    List<KeyValueDTO> _ListadoTesorerias = new List<KeyValueDTO>();
                    List<tesoreriadetalleDto> _TesoreriaDetalleXInsertar = new List<tesoreriadetalleDto>();
                    _ListadoTesorerias = _objTesoreriaBL.ObtenerListadoTesorerias(ref pobjOperationResult, pobjDtoEntity.t_FechaRegistro.Value.Year.ToString(), pobjDtoEntity.t_FechaRegistro.Value.Month.ToString("00"), pobjDtoEntity.i_IdTipoDocumento.Value);

                    int _MaxMovimiento;
                    _MaxMovimiento = _ListadoTesorerias.Count() > 0 ? int.Parse(_ListadoTesorerias[_ListadoTesorerias.Count() - 1].Value1.ToString()) : 0;
                    _MaxMovimiento++;
                    C_tesoreriaDto.d_TipoCambio = pobjDtoEntity.d_TipoCambio.Value;
                    C_tesoreriaDto.i_IdMoneda = pobjDtoEntity.i_IdMoneda.Value;
                    C_tesoreriaDto.v_Mes = pobjDtoEntity.t_FechaRegistro.Value.Month.ToString("00");
                    C_tesoreriaDto.v_Periodo = pobjDtoEntity.t_FechaRegistro.Value.Year.ToString();
                    C_tesoreriaDto.v_Correlativo = _MaxMovimiento.ToString("00000000");
                    C_tesoreriaDto.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                    C_tesoreriaDto.v_Glosa = pobjDtoEntity.v_Glosa;
                    C_tesoreriaDto.v_NroCuentaCajaBanco = _objTesoreriaBL.DevuelveCuentaCajaBanco(ref pobjOperationResult, pobjDtoEntity.i_IdTipoDocumento.Value).NroCuenta;
                    if (pobjOperationResult.Success == 0) return null;
                    C_tesoreriaDto.i_IdEstado = 1;

                    C_tesoreriaDto.i_IdMedioPago = pobjDtoEntity.i_IdMedioPago;
                    C_tesoreriaDto.t_FechaRegistro = pobjDtoEntity.t_FechaRegistro.Value;
                    C_tesoreriaDto.v_IdCobranza = newIdPago;
                    C_tesoreriaDto.i_TipoMovimiento = (int?)TipoMovimientoTesoreria.Egreso;

                    // foreach (var Fila in pTemp_Insertar.Where(p => p.i_IdTipoDocumentoRef != 7))

                    foreach (var Fila in pTemp_Insertar.Where(p => !_objDocumentoBL.DocumentoEsInverso(p.i_IdTipoDocumentoRef.Value)))
                    {
                        if (Fila.i_EsLetra == null || Fila.i_EsLetra == 0)
                        {
                            tesoreriadetalleDto DH_tesoreriadetalleDto = new tesoreriadetalleDto();

                            compra _compra = (from v in dbContext.compra
                                              where v.v_IdCompra == Fila.v_IdCompra && v.i_Eliminado == 0
                                              select v).FirstOrDefault();

                            string IDConcepto = _compra.i_IdTipoCompra.Value.ToString("00");

                            DH_tesoreriadetalleDto.v_IdTesoreria = C_tesoreriaDto.v_IdTesoreria;
                            DH_tesoreriadetalleDto.d_Cambio = C_tesoreriaDto.i_IdMoneda == 1 ? Fila.d_ImporteSoles / C_tesoreriaDto.d_TipoCambio : Fila.d_ImporteSoles * C_tesoreriaDto.d_TipoCambio;
                            DH_tesoreriadetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(DH_tesoreriadetalleDto.d_Cambio.Value, 2);

                            DH_tesoreriadetalleDto.d_Importe = Fila.d_ImporteSoles.Value;
                            DH_tesoreriadetalleDto.i_IdTipoDocumentoRef = Fila.i_IdTipoDocumentoRef;
                            //DH_tesoreriadetalleDto.v_Naturaleza = DH_tesoreriadetalleDto.i_IdTipoDocumentoRef != 7 ? "D" : "H";

                            DH_tesoreriadetalleDto.v_Naturaleza = !_objDocumentoBL.DocumentoEsInverso(DH_tesoreriadetalleDto.i_IdTipoDocumentoRef.Value) ? "D" : "H";
                            DH_tesoreriadetalleDto.v_Analisis = "";
                            DH_tesoreriadetalleDto.v_IdCliente = (from v in dbContext.compra
                                                                  join J1 in dbContext.cliente on new { IdProv = v.v_IdProveedor, eliminado = 0 } equals new { IdProv = J1.v_IdCliente, eliminado = J1.i_Eliminado.Value }
                                                                  where v.v_IdCompra == Fila.v_IdCompra
                                                                  select new { J1.v_IdCliente }).FirstOrDefault().v_IdCliente;

                            DH_tesoreriadetalleDto.v_NroCuenta = (from v in dbContext.administracionconceptos
                                                                  where v.v_Codigo == IDConcepto && v.i_Eliminado == 0 && v.v_Periodo.Equals(periodo)
                                                                  select new { v.v_CuentaPVenta }).FirstOrDefault().v_CuentaPVenta;

                            DH_tesoreriadetalleDto.v_NroDocumento = _compra.v_SerieDocumento + "-" + _compra.v_CorrelativoDocumento;
                            DH_tesoreriadetalleDto.v_NroDocumentoRef = Fila.v_DocumentoRef;
                            DH_tesoreriadetalleDto.v_OrigenDestino = "";
                            DH_tesoreriadetalleDto.v_Pedido = "";
                            DH_tesoreriadetalleDto.t_Fecha = _compra.t_FechaRegistro;
                            DH_tesoreriadetalleDto.i_IdTipoDocumento = _compra.i_IdTipoDocumento.Value;
                            DH_tesoreriadetalleDto.i_IdCentroCostos = "0";
                            DH_tesoreriadetalleDto.i_IdCaja = 0;
                            if (_objDocumentoBL.DocumentoEsContable(DH_tesoreriadetalleDto.i_IdTipoDocumento.Value))// AGREGO 7DIC.
                            {
                                _TesoreriaDetalleXInsertar.Add(DH_tesoreriadetalleDto);
                            }
                        }
                        else
                        {
                            tesoreriadetalleDto DH_tesoreriadetalleDto = new tesoreriadetalleDto();

                            letraspagardetalle _venta = (from v in dbContext.letraspagardetalle
                                                         where v.v_IdLetrasPagarDetalle == Fila.v_IdCompra
                                                         select v).FirstOrDefault();

                            string IDConcepto = _venta.i_IdMoneda == 1 ? "32" : "33";

                            DH_tesoreriadetalleDto.v_IdTesoreria = C_tesoreriaDto.v_IdTesoreria;
                            DH_tesoreriadetalleDto.d_Cambio = C_tesoreriaDto.i_IdMoneda == 1 ? Fila.d_ImporteSoles / C_tesoreriaDto.d_TipoCambio : Fila.d_ImporteSoles * C_tesoreriaDto.d_TipoCambio;
                            DH_tesoreriadetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(DH_tesoreriadetalleDto.d_Cambio.Value, 2);

                            DH_tesoreriadetalleDto.d_Importe = Fila.d_ImporteSoles.Value;
                            DH_tesoreriadetalleDto.i_IdTipoDocumentoRef = Fila.i_IdTipoDocumentoRef;
                            // DH_tesoreriadetalleDto.v_Naturaleza = DH_tesoreriadetalleDto.i_IdTipoDocumentoRef != 7 ? "D" : "H";
                            DH_tesoreriadetalleDto.v_Naturaleza = !_objDocumentoBL.DocumentoEsInverso(DH_tesoreriadetalleDto.i_IdTipoDocumentoRef.Value) ? "D" : "H";

                            DH_tesoreriadetalleDto.v_Analisis = "";
                            DH_tesoreriadetalleDto.v_IdCliente = (from v in dbContext.letraspagardetalle
                                                                  join J1 in dbContext.cliente on v.v_IdProveedor equals J1.v_IdCliente
                                                                  where v.v_IdLetrasPagarDetalle == Fila.v_IdCompra
                                                                  select new { J1.v_IdCliente }).FirstOrDefault().v_IdCliente;

                            DH_tesoreriadetalleDto.v_NroCuenta = (from v in dbContext.administracionconceptos
                                                                  where v.v_Codigo == IDConcepto && v.v_Periodo.Equals(periodo) && v.i_Eliminado == 0
                                                                  select new { v.v_CuentaPVenta }).FirstOrDefault().v_CuentaPVenta;

                            DH_tesoreriadetalleDto.v_NroDocumento = _venta.v_Serie + "-" + _venta.v_Correlativo;
                            DH_tesoreriadetalleDto.v_NroDocumentoRef = Fila.v_DocumentoRef;
                            DH_tesoreriadetalleDto.v_OrigenDestino = "";
                            DH_tesoreriadetalleDto.v_Pedido = "";
                            DH_tesoreriadetalleDto.t_Fecha = _venta.t_FechaEmision;
                            DH_tesoreriadetalleDto.i_IdTipoDocumento = _venta.i_IdTipoDocumento.Value;
                            DH_tesoreriadetalleDto.i_IdCentroCostos = "0";
                            DH_tesoreriadetalleDto.i_IdCaja = 0;
                            if (_objDocumentoBL.DocumentoEsContable(DH_tesoreriadetalleDto.i_IdTipoDocumento.Value))// AGREGO 7DIC.
                            {
                                _TesoreriaDetalleXInsertar.Add(DH_tesoreriadetalleDto);
                            }
                        }

                    }
                    //   foreach (var Fila in pTemp_Insertar.Where(p => p.i_IdTipoDocumentoRef == 7).GroupBy(g => g.v_IdCompra))
                    foreach (var Fila in pTemp_Insertar.Where(p => _objDocumentoBL.DocumentoEsInverso(p.i_IdTipoDocumentoRef.Value)).GroupBy(g => g.v_IdCompra))
                    {
                        string idCompra = Fila.FirstOrDefault().v_IdCompra;
                        compra _compra = (from v in dbContext.compra
                                          where v.v_IdCompra == idCompra && v.i_Eliminado == 0
                                          select v).FirstOrDefault();
                        string IDConcepto = _compra.i_IdTipoCompra.Value.ToString("00");

                        string NroCuenta = (from v in dbContext.administracionconceptos
                                            where v.v_Codigo == IDConcepto && v.i_Eliminado == 0 && v.v_Periodo.Equals(periodo)
                                            select new { v.v_CuentaPVenta }).FirstOrDefault().v_CuentaPVenta;

                        // var FilaComun = _TesoreriaDetalleXInsertar.Where(p => p.v_Naturaleza == "H" && p.v_NroCuenta == NroCuenta && p.i_IdTipoDocumento == _compra.i_IdTipoDocumento && p.v_NroDocumento == _compra.v_SerieDocumento + "-" + _compra.v_CorrelativoDocumento).FirstOrDefault();
                        var FilaComun = _TesoreriaDetalleXInsertar.Where(p => p.v_Naturaleza == "D" && p.v_NroCuenta == NroCuenta && p.i_IdTipoDocumento == _compra.i_IdTipoDocumento && p.v_NroDocumento == _compra.v_SerieDocumento + "-" + _compra.v_CorrelativoDocumento).FirstOrDefault();

                        if (FilaComun != null)
                        {
                            FilaComun.d_Importe = FilaComun.d_Importe + Fila.Sum(p => p.d_ImporteSoles.Value);
                            FilaComun.d_Cambio = C_tesoreriaDto.i_IdMoneda == 1 ? FilaComun.d_Importe / C_tesoreriaDto.d_TipoCambio : FilaComun.d_Importe * C_tesoreriaDto.d_TipoCambio;
                            FilaComun.d_Cambio = Utils.Windows.DevuelveValorRedondeado(FilaComun.d_Cambio.Value, 2);
                        }
                        else
                        {
                            tesoreriadetalleDto DH_tesoreriadetalleDto = new tesoreriadetalleDto();
                            decimal ImporteH = Fila.Sum(p => p.d_ImporteSoles.Value);
                            DH_tesoreriadetalleDto.v_IdTesoreria = C_tesoreriaDto.v_IdTesoreria;
                            DH_tesoreriadetalleDto.d_Cambio = C_tesoreriaDto.i_IdMoneda == 1 ? ImporteH / C_tesoreriaDto.d_TipoCambio : ImporteH * C_tesoreriaDto.d_TipoCambio;
                            DH_tesoreriadetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(DH_tesoreriadetalleDto.d_Cambio.Value, 2);
                            DH_tesoreriadetalleDto.d_Importe = ImporteH;
                            DH_tesoreriadetalleDto.i_IdTipoDocumentoRef = Fila.FirstOrDefault().i_IdTipoDocumentoRef;
                            //DH_tesoreriadetalleDto.v_Naturaleza = "H";
                            DH_tesoreriadetalleDto.v_Naturaleza = "D";

                            DH_tesoreriadetalleDto.v_IdCliente = (from v in dbContext.compra
                                                                  join J1 in dbContext.cliente on new { IdProv = v.v_IdProveedor, eliminado = 0 } equals new { IdProv = J1.v_IdCliente, eliminado = J1.i_Eliminado.Value }
                                                                  where v.v_IdCompra == idCompra
                                                                  select new { J1.v_IdCliente }).FirstOrDefault().v_IdCliente;

                            DH_tesoreriadetalleDto.v_Analisis = "";
                            DH_tesoreriadetalleDto.v_NroCuenta = NroCuenta;

                            DH_tesoreriadetalleDto.v_NroDocumento = _compra.v_SerieDocumento + "-" + _compra.v_CorrelativoDocumento;
                            DH_tesoreriadetalleDto.v_NroDocumentoRef = Fila.FirstOrDefault().v_DocumentoRef;
                            DH_tesoreriadetalleDto.v_OrigenDestino = "";
                            DH_tesoreriadetalleDto.v_Pedido = "";
                            DH_tesoreriadetalleDto.t_Fecha = _compra.t_FechaRegistro;
                            DH_tesoreriadetalleDto.i_IdTipoDocumento = _compra.i_IdTipoDocumento.Value;
                            DH_tesoreriadetalleDto.i_IdCentroCostos = "0";
                            DH_tesoreriadetalleDto.i_IdCaja = 0;

                            if (_objDocumentoBL.DocumentoEsContable(DH_tesoreriadetalleDto.i_IdTipoDocumento.Value))// AGREGO 7DIC.
                            {
                                _TesoreriaDetalleXInsertar.Add(DH_tesoreriadetalleDto);
                            }
                        }

                        tesoreriadetalleDto DD_tesoreriadetalleDto = new tesoreriadetalleDto();
                        decimal ImporteD = Fila.Sum(p => p.d_ImporteSoles.Value);
                        DD_tesoreriadetalleDto.v_IdTesoreria = C_tesoreriaDto.v_IdTesoreria;
                        DD_tesoreriadetalleDto.d_Cambio = C_tesoreriaDto.i_IdMoneda == 1 ? ImporteD / C_tesoreriaDto.d_TipoCambio : ImporteD * C_tesoreriaDto.d_TipoCambio;
                        DD_tesoreriadetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(DD_tesoreriadetalleDto.d_Cambio.Value, 2);
                        DD_tesoreriadetalleDto.d_Importe = ImporteD;
                        DD_tesoreriadetalleDto.i_IdTipoDocumentoRef = Fila.FirstOrDefault().i_IdTipoDocumentoRef;
                        DD_tesoreriadetalleDto.v_Naturaleza = "H";
                        DD_tesoreriadetalleDto.v_IdCliente = (from v in dbContext.compra
                                                              join J1 in dbContext.cliente on new { IdProv = v.v_IdProveedor } equals new { IdProv = J1.v_IdCliente }
                                                              where v.v_IdCompra == idCompra
                                                              select new { J1.v_IdCliente }).FirstOrDefault().v_IdCliente;
                        DD_tesoreriadetalleDto.v_Analisis = "";
                        DD_tesoreriadetalleDto.v_NroCuenta = NroCuenta;
                        DD_tesoreriadetalleDto.v_NroDocumento = _compra.v_SerieDocumento + "-" + _compra.v_CorrelativoDocumento;
                        DD_tesoreriadetalleDto.v_NroDocumentoRef = Fila.FirstOrDefault().v_DocumentoRef;
                        DD_tesoreriadetalleDto.v_OrigenDestino = "";
                        DD_tesoreriadetalleDto.v_Pedido = "";
                        DD_tesoreriadetalleDto.t_Fecha = _compra.t_FechaRegistro;
                        DD_tesoreriadetalleDto.i_IdTipoDocumento = _compra.i_IdTipoDocumento.Value;
                        DD_tesoreriadetalleDto.i_IdCentroCostos = "0";
                        DD_tesoreriadetalleDto.i_IdCaja = 0;
                        if (_objDocumentoBL.DocumentoEsContable(DD_tesoreriadetalleDto.i_IdTipoDocumento.Value))// AGREGO 7DIC.
                        {
                            _TesoreriaDetalleXInsertar.Add(DD_tesoreriadetalleDto);
                        }
                    }

                    if (pTemp_Insertar.Count() == 1)
                    {
                        if (pTemp_Insertar[0].i_EsLetra == null || pTemp_Insertar[0].i_EsLetra == 0)
                        {
                            string IdCompra = pTemp_Insertar[0].v_IdCompra;
                            C_tesoreriaDto.v_Nombre = (from v in dbContext.compra

                                                       join J1 in dbContext.cliente on new { IdProv = v.v_IdProveedor } equals new { IdProv = J1.v_IdCliente } into J1_join
                                                       from J1 in J1_join.DefaultIfEmpty()
                                                       where v.v_IdCompra == IdCompra

                                                       select new
                                                       {
                                                           Nombre = v.v_IdProveedor != "N002-CL000000000" ? (J1.v_PrimerNombre + " " + J1.v_ApePaterno + " " + J1.v_ApeMaterno + " " + J1.v_RazonSocial).Trim() : ""
                                                       }
                                                       ).FirstOrDefault().Nombre;
                        }
                        else
                        {
                            string IdCompra = pTemp_Insertar[0].v_IdCompra;
                            C_tesoreriaDto.v_Nombre = (from v in dbContext.letraspagardetalle

                                                       join J1 in dbContext.cliente on v.v_IdProveedor equals J1.v_IdCliente into J1_join
                                                       from J1 in J1_join.DefaultIfEmpty()

                                                       where v.v_IdLetrasPagarDetalle == IdCompra

                                                       select new
                                                       {
                                                           Nombre = v.v_IdProveedor != "N002-CL000000000" ? (J1.v_PrimerNombre + " " + J1.v_ApePaterno + " " + J1.v_ApeMaterno + " " + J1.v_RazonSocial).Trim() : "PÚBLICO GENERAL"
                                                       }
                                                       ).FirstOrDefault().Nombre;
                        }

                    }
                    else
                    {
                        C_tesoreriaDto.v_Nombre = "PAGOS VARIOS";
                    }


                    // if (_TesoreriaDetalleXInsertar.Count(p => p.i_IdTipoDocumentoRef != 7) > 0)
                    if (_TesoreriaDetalleXInsertar.Count(p => !_objDocumentoBL.DocumentoEsInverso(p.i_IdTipoDocumentoRef.Value)) > 0)
                    {
                        tesoreriadetalleDto DD_tesoreriadetalleDto = new tesoreriadetalleDto();
                        //  decimal Importe = _TesoreriaDetalleXInsertar.Where(o => o.i_IdTipoDocumentoRef != 7).Sum(p => p.d_Importe.Value) - _TesoreriaDetalleXInsertar.Where(o => o.i_IdTipoDocumentoRef == 7).Sum(p => p.d_Importe.Value);

                        decimal Importe = _TesoreriaDetalleXInsertar.Where(o => !_objDocumentoBL.DocumentoEsInverso(o.i_IdTipoDocumentoRef.Value)).Sum(p => p.d_Importe.Value) - _TesoreriaDetalleXInsertar.Where(o => _objDocumentoBL.DocumentoEsInverso(o.i_IdTipoDocumentoRef.Value)).Sum(p => p.d_Importe.Value);


                        DD_tesoreriadetalleDto.v_IdTesoreria = C_tesoreriaDto.v_IdTesoreria;
                        DD_tesoreriadetalleDto.d_Cambio = C_tesoreriaDto.i_IdMoneda == 1 ? Importe / C_tesoreriaDto.d_TipoCambio : Importe * C_tesoreriaDto.d_TipoCambio;
                        DD_tesoreriadetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(DD_tesoreriadetalleDto.d_Cambio.Value, 2);
                        DD_tesoreriadetalleDto.d_Importe = Importe;
                        DD_tesoreriadetalleDto.i_IdTipoDocumentoRef = _TesoreriaDetalleXInsertar.Count() == 1 ? _TesoreriaDetalleXInsertar[0].i_IdTipoDocumento.Value : -1;
                        //  DD_tesoreriadetalleDto.v_Naturaleza = "D";
                        DD_tesoreriadetalleDto.v_Naturaleza = "H";
                        DD_tesoreriadetalleDto.v_Analisis = "";
                        DD_tesoreriadetalleDto.v_NroCuenta = C_tesoreriaDto.v_NroCuentaCajaBanco;
                        //  DD_tesoreriadetalleDto.v_NroDocumento = _objTesoreriaBL.DevuelveCorrelativoCheque(ref pobjOperationResult, DD_tesoreriadetalleDto.v_NroCuenta).ToString("00000000");
                        DD_tesoreriadetalleDto.v_NroDocumento = pobjDtoEntity.v_Mes.Trim() + "-" + pobjDtoEntity.v_Correlativo.Trim();
                        DD_tesoreriadetalleDto.v_NroDocumentoRef = _TesoreriaDetalleXInsertar.Count() == 1 ? _TesoreriaDetalleXInsertar[0].v_NroDocumento : string.Empty;
                        DD_tesoreriadetalleDto.v_OrigenDestino = "";
                        DD_tesoreriadetalleDto.v_Pedido = "";
                        DD_tesoreriadetalleDto.t_Fecha = C_tesoreriaDto.t_FechaRegistro;
                        //DD_tesoreriadetalleDto.i_IdTipoDocumento = 309;
                        DD_tesoreriadetalleDto.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                        DD_tesoreriadetalleDto.i_IdCentroCostos = "0";
                        DD_tesoreriadetalleDto.i_IdCaja = 0;
                        _TesoreriaDetalleXInsertar.Add(DD_tesoreriadetalleDto);
                    }

                    decimal TotDebe, TotHaber, TotDebeC, TotHaberC;
                    TotDebe = _TesoreriaDetalleXInsertar.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Importe.Value);
                    TotHaber = _TesoreriaDetalleXInsertar.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Importe.Value);
                    TotDebeC = _TesoreriaDetalleXInsertar.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Cambio.Value);
                    TotHaberC = _TesoreriaDetalleXInsertar.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Cambio.Value);
                    C_tesoreriaDto.d_TotalDebe_Importe = TotDebe;
                    C_tesoreriaDto.d_TotalHaber_Importe = TotHaber;
                    C_tesoreriaDto.d_TotalDebe_Cambio = TotDebeC;
                    C_tesoreriaDto.d_TotalHaber_Cambio = TotHaberC;
                    C_tesoreriaDto.d_Diferencia_Importe = (TotDebe - TotHaber);
                    C_tesoreriaDto.d_Diferencia_Cambio = (TotDebeC - TotHaberC);


                    if (_TesoreriaDetalleXInsertar.Any())
                    {
                        _objTesoreriaBL.Insertartesoreria(ref pobjOperationResult, C_tesoreriaDto, Globals.ClientSession.GetAsList(), _TesoreriaDetalleXInsertar);

                        if (pobjOperationResult.Success == 0) return null;

                        C_tesoreriaDto = new tesoreriaDto();
                    }
                    _TesoreriaDetalleXInsertar = new List<tesoreriadetalleDto>();

                    #endregion

                    pobjOperationResult.Success = 1;
                    Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "pago", newIdPago);
                    ts.Complete();
                    return newIdPago;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PagoBL.InsertarPago()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = pobjOperationResult.ExceptionMessage != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public string ActualizarPago(ref OperationResult pobjOperationResult, pagoDto pobjDtoEntity, List<string> ClientSession, List<pagodetalleDto> pTemp_Insertar, List<pagodetalleDto> pTemp_Editar, List<pagodetalleDto> pTemp_Eliminar)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    SecuentialBL objSecuentialBL = new SecuentialBL();
                    pago objEntityCompra = pagoAssembler.ToEntity(pobjDtoEntity);
                    pagodetalleDto pobjDtopagoDetalle = new pagodetalleDto();
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                    DocumentoBL _objDocumentoBL = new DocumentoBL();
                    int SecuentialId = 0;
                    string newIdpagoDetalle = string.Empty;
                    int intNodeId;

                    #region Actualiza Cabecera
                    intNodeId = int.Parse(ClientSession[0]);
                    var objEntitySource = (from a in dbContext.pago
                                           where a.v_IdPago == pobjDtoEntity.v_IdPago
                                           select a).FirstOrDefault();

                    pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                    pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                    pago objEntity = pagoAssembler.ToEntity(pobjDtoEntity);
                    dbContext.pago.ApplyCurrentValues(objEntity);
                    #endregion

                    #region Actualiza Detalle
                    foreach (pagodetalleDto pagodetalleDto in pTemp_Insertar)
                    {
                        pagodetalle objEntitypagoDetalle = pagodetalleAssembler.ToEntity(pagodetalleDto);
                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 81);
                        newIdpagoDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "PD");
                        objEntitypagoDetalle.v_IdPagoDetalle = newIdpagoDetalle;
                        objEntitypagoDetalle.t_InsertaFecha = DateTime.Now;
                        objEntitypagoDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntitypagoDetalle.i_Eliminado = 0;
                        dbContext.AddTopagodetalle(objEntitypagoDetalle);
                        #region Actualiza pagoPendiente
                        if (pagodetalleDto.i_EsLetra == 0)
                        {
                            ProcesaDetallePago(ref pobjOperationResult, pagodetalleAssembler.ToDTO(objEntitypagoDetalle), pobjDtoEntity, ClientSession);
                            if (pobjOperationResult.Success == 0) return null;
                        }
                        else
                        {
                            ProcesaDetallePagoLetras(ref pobjOperationResult, pagodetalleAssembler.ToDTO(objEntitypagoDetalle), pobjDtoEntity, ClientSession);
                            if (pobjOperationResult.Success == 0) return null;
                        }
                        #endregion

                        dbContext.SaveChanges();
                    }

                    foreach (pagodetalleDto pagodetalleDto in pTemp_Editar)
                    {
                        pagodetalle _objEntity = pagodetalleAssembler.ToEntity(pagodetalleDto);
                        var query = (from n in dbContext.pagodetalle
                                     where n.v_IdPagoDetalle == pagodetalleDto.v_IdPagoDetalle
                                     select n).FirstOrDefault();

                        _objEntity.t_ActualizaFecha = DateTime.Now;
                        _objEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                        dbContext.pagodetalle.ApplyCurrentValues(_objEntity);

                        dbContext.SaveChanges();

                        if (query != null)
                        {
                            if (query.i_EsLetra == 0)
                            {
                                RecalcularSaldoCompra(ref pobjOperationResult, query.v_IdCompra, ClientSession, false);
                                if (pobjOperationResult.Success == 0) return null;
                            }
                            else
                            {
                                RestauraDetallePagoLetras(ref pobjOperationResult, query.v_IdPagoDetalle, ClientSession);
                                if (pobjOperationResult.Success == 0) return null;
                            }
                        }

                        Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "pagodetalle", query.v_IdPagoDetalle);

                    }

                    foreach (pagodetalleDto pagodetalleDto in pTemp_Eliminar)
                    {
                        pagodetalle _objEntity = pagodetalleAssembler.ToEntity(pagodetalleDto);
                        var query = (from n in dbContext.pagodetalle
                                     where n.v_IdPagoDetalle == pagodetalleDto.v_IdPagoDetalle
                                     select n).FirstOrDefault();

                        query.t_ActualizaFecha = DateTime.Now;
                        query.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        query.i_Eliminado = 1;

                        dbContext.SaveChanges();

                        if (query != null)
                        {
                            if (query.i_EsLetra == 0)
                            {
                                RecalcularSaldoCompra(ref pobjOperationResult, query.v_IdCompra, ClientSession, false);
                                if (pobjOperationResult.Success == 0) return null;
                            }
                            else
                            {
                                RestauraDetallePagoLetras(ref pobjOperationResult, query.v_IdPagoDetalle, ClientSession);
                                if (pobjOperationResult.Success == 0) return null;
                            }
                        }

                        dbContext.pagodetalle.ApplyCurrentValues(query);
                        Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "pagodetalle", query.v_IdPagoDetalle);
                    }
                    dbContext.SaveChanges();
                    #endregion

                    #region Si es anulado restaura detalles

                    if (pobjDtoEntity.i_IdEstado == 0)
                    {
                        List<pagodetalle> objEntitySourceDetallespago = (from a in dbContext.pagodetalle
                                                                         where a.v_IdPago == pobjDtoEntity.v_IdPago
                                                                         select a).ToList();

                        foreach (pagodetalle RegistropagoDetalle in objEntitySourceDetallespago)
                        {
                            if (RegistropagoDetalle.i_EsLetra == 0)
                            {
                                RecalcularSaldoCompra(ref pobjOperationResult, RegistropagoDetalle.v_IdCompra, ClientSession, false);
                                //RestauraDetallePago(ref pobjOperationResult, RegistropagoDetalle.v_IdPagoDetalle, ClientSession);
                                if (pobjOperationResult.Success == 0) return null;
                            }
                            else
                            {
                                RestauraDetallePagoLetras(ref pobjOperationResult, RegistropagoDetalle.v_IdPagoDetalle, ClientSession);
                                if (pobjOperationResult.Success == 0) return null;
                            }
                        }
                    }
                    #endregion

                    #region Regenera Tesorería
                    TesoreriaBL _objTesoreriaBL = new TesoreriaBL();
                    tesoreriaDto C_tesoreriaDto = new tesoreriaDto();
                    List<KeyValueDTO> _ListadoTesorerias = new List<KeyValueDTO>();
                    List<tesoreriadetalleDto> _TesoreriaDetalleXInsertar = new List<tesoreriadetalleDto>();

                    string[] IdRegistroEliminado = new string[3];

                    IdRegistroEliminado = _objTesoreriaBL.EliminarTesoreriaXDocRef(ref pobjOperationResult, objEntitySource.v_IdPago, ClientSession);
                    if (pobjOperationResult.Success == 0) return null;

                    if (pobjDtoEntity.i_IdEstado == 1)
                    {
                        var pagoDetalles = (from c in dbContext.pagodetalle
                                            where c.i_Eliminado == 0 && c.v_IdPago == objEntitySource.v_IdPago
                                            select c).ToList();

                        _ListadoTesorerias = _objTesoreriaBL.ObtenerListadoTesorerias(ref pobjOperationResult, pobjDtoEntity.t_FechaRegistro.Value.Year.ToString(), pobjDtoEntity.t_FechaRegistro.Value.Month.ToString("00"), pobjDtoEntity.i_IdTipoDocumento.Value);
                        int _MaxMovimiento;
                        _MaxMovimiento = _ListadoTesorerias.Count() > 0 ? int.Parse(_ListadoTesorerias[_ListadoTesorerias.Count() - 1].Value1.ToString()) : 0;
                        _MaxMovimiento++;

                        C_tesoreriaDto.d_TipoCambio = pobjDtoEntity.d_TipoCambio.Value;
                        C_tesoreriaDto.i_IdMoneda = pobjDtoEntity.i_IdMoneda.Value;

                        if (IdRegistroEliminado[0] != null && IdRegistroEliminado[1] != null && IdRegistroEliminado[2] != null)
                        {
                            C_tesoreriaDto.v_Mes = pobjDtoEntity.t_FechaRegistro.Value.Year.ToString() != IdRegistroEliminado[0] || pobjDtoEntity.t_FechaRegistro.Value.Month.ToString("00") != IdRegistroEliminado[1].Trim() ? pobjDtoEntity.t_FechaRegistro.Value.Month.ToString("00") : IdRegistroEliminado[1].Trim();
                            C_tesoreriaDto.v_Periodo = pobjDtoEntity.t_FechaRegistro.Value.Year.ToString() != IdRegistroEliminado[0] ? pobjDtoEntity.t_FechaRegistro.Value.Year.ToString() : IdRegistroEliminado[0];
                            C_tesoreriaDto.v_Correlativo = pobjDtoEntity.t_FechaRegistro.Value.Year.ToString() != IdRegistroEliminado[0] || pobjDtoEntity.t_FechaRegistro.Value.Month.ToString("00") != IdRegistroEliminado[1].Trim() ? _MaxMovimiento.ToString("00000000") : IdRegistroEliminado[2];
                        }
                        else
                        {
                            C_tesoreriaDto.v_Mes = pobjDtoEntity.t_FechaRegistro.Value.Month.ToString("00");
                            C_tesoreriaDto.v_Periodo = pobjDtoEntity.t_FechaRegistro.Value.Year.ToString();
                            C_tesoreriaDto.v_Correlativo = _MaxMovimiento.ToString("00000000");
                        }

                        C_tesoreriaDto.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                        C_tesoreriaDto.v_Glosa = pobjDtoEntity.v_Glosa;
                        C_tesoreriaDto.v_NroCuentaCajaBanco = _objTesoreriaBL.DevuelveCuentaCajaBanco(ref pobjOperationResult, pobjDtoEntity.i_IdTipoDocumento.Value).NroCuenta;
                        C_tesoreriaDto.i_IdEstado = 1;
                        C_tesoreriaDto.i_IdMedioPago = pobjDtoEntity.i_IdMedioPago;
                        C_tesoreriaDto.t_FechaRegistro = pobjDtoEntity.t_FechaRegistro.Value;
                        C_tesoreriaDto.v_IdCobranza = objEntitySource.v_IdPago;
                        C_tesoreriaDto.i_TipoMovimiento = (int?)TipoMovimientoTesoreria.Egreso;

                        //foreach (var Fila in pagoDetalles.Where(p => p.i_IdTipoDocumentoRef != 7))
                        foreach (var Fila in pagoDetalles.Where(p => !_objDocumentoBL.DocumentoEsInverso(p.i_IdTipoDocumentoRef.Value)))
                        {
                            if (Fila.i_EsLetra == 0)
                            {
                                tesoreriadetalleDto DH_tesoreriadetalleDto = new tesoreriadetalleDto();

                                compra _compra = (from v in dbContext.compra
                                                  where v.v_IdCompra == Fila.v_IdCompra
                                                  select v).FirstOrDefault();

                                string IDConcepto = _compra.i_IdTipoCompra.Value.ToString("00");

                                DH_tesoreriadetalleDto.v_IdTesoreria = C_tesoreriaDto.v_IdTesoreria;
                                DH_tesoreriadetalleDto.d_Cambio = C_tesoreriaDto.i_IdMoneda == 1 ? Fila.d_ImporteSoles / C_tesoreriaDto.d_TipoCambio : Fila.d_ImporteSoles * C_tesoreriaDto.d_TipoCambio;
                                DH_tesoreriadetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(DH_tesoreriadetalleDto.d_Cambio.Value, 2);
                                DH_tesoreriadetalleDto.d_Importe = Fila.d_ImporteSoles.Value;
                                DH_tesoreriadetalleDto.i_IdTipoDocumentoRef = Fila.i_IdTipoDocumentoRef;
                                // DH_tesoreriadetalleDto.v_Naturaleza = DH_tesoreriadetalleDto.i_IdTipoDocumentoRef != 7 ? "D" : "H";
                                DH_tesoreriadetalleDto.v_Naturaleza = !_objDocumentoBL.DocumentoEsInverso(DH_tesoreriadetalleDto.i_IdTipoDocumentoRef.Value) ? "D" : "H";


                                DH_tesoreriadetalleDto.v_Analisis = "";
                                DH_tesoreriadetalleDto.v_IdCliente = (from v in dbContext.compra
                                                                      join J1 in dbContext.cliente on v.v_IdProveedor equals J1.v_IdCliente
                                                                      where v.v_IdCompra == Fila.v_IdCompra
                                                                      select new { J1.v_IdCliente }).FirstOrDefault().v_IdCliente;

                                DH_tesoreriadetalleDto.v_NroCuenta = (from v in dbContext.administracionconceptos
                                                                      where v.v_Codigo == IDConcepto && v.i_Eliminado == 0 && v.v_Periodo.Equals(periodo)
                                                                      select new { v.v_CuentaPVenta }).FirstOrDefault().v_CuentaPVenta;

                                DH_tesoreriadetalleDto.v_NroDocumento = _compra.v_SerieDocumento + "-" + _compra.v_CorrelativoDocumento;
                                DH_tesoreriadetalleDto.v_NroDocumentoRef = Fila.v_DocumentoRef;
                                DH_tesoreriadetalleDto.v_OrigenDestino = "";
                                DH_tesoreriadetalleDto.v_Pedido = "";
                                DH_tesoreriadetalleDto.t_Fecha = _compra.t_FechaRegistro;
                                DH_tesoreriadetalleDto.i_IdTipoDocumento = _compra.i_IdTipoDocumento.Value;
                                DH_tesoreriadetalleDto.i_IdCentroCostos = "0";
                                DH_tesoreriadetalleDto.i_IdCaja = 0;
                                if (_objDocumentoBL.DocumentoEsContable(DH_tesoreriadetalleDto.i_IdTipoDocumento.Value))
                                {
                                    _TesoreriaDetalleXInsertar.Add(DH_tesoreriadetalleDto);
                                }
                            }
                            else
                            {
                                tesoreriadetalleDto DH_tesoreriadetalleDto = new tesoreriadetalleDto();

                                letraspagardetalle _compra = (from v in dbContext.letraspagardetalle
                                                              where v.v_IdLetrasPagarDetalle == Fila.v_IdCompra
                                                              select v).FirstOrDefault();

                                string IDConcepto = _compra.i_IdMoneda == 1 ? "32" : "33";

                                DH_tesoreriadetalleDto.v_IdTesoreria = C_tesoreriaDto.v_IdTesoreria;
                                DH_tesoreriadetalleDto.d_Cambio = C_tesoreriaDto.i_IdMoneda == 1 ? Fila.d_ImporteSoles / C_tesoreriaDto.d_TipoCambio : Fila.d_ImporteSoles * C_tesoreriaDto.d_TipoCambio;
                                DH_tesoreriadetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(DH_tesoreriadetalleDto.d_Cambio.Value, 2);

                                DH_tesoreriadetalleDto.d_Importe = Fila.d_ImporteSoles.Value;
                                DH_tesoreriadetalleDto.i_IdTipoDocumentoRef = Fila.i_IdTipoDocumentoRef;
                                DH_tesoreriadetalleDto.v_Naturaleza = !_objDocumentoBL.DocumentoEsInverso(DH_tesoreriadetalleDto.i_IdTipoDocumentoRef.Value) ? "D" : "H";

                                DH_tesoreriadetalleDto.v_Analisis = "";
                                DH_tesoreriadetalleDto.v_IdCliente = (from v in dbContext.letraspagardetalle
                                                                      join J1 in dbContext.cliente on v.v_IdProveedor equals J1.v_IdCliente
                                                                      where v.v_IdLetrasPagarDetalle == Fila.v_IdCompra
                                                                      select new { J1.v_IdCliente }).FirstOrDefault().v_IdCliente;

                                DH_tesoreriadetalleDto.v_NroCuenta = (from v in dbContext.administracionconceptos
                                                                      where v.v_Codigo == IDConcepto && v.v_Periodo.Equals(periodo)   && v.i_Eliminado== 0
                                                                      select new { v.v_CuentaPVenta }).FirstOrDefault().v_CuentaPVenta;

                                DH_tesoreriadetalleDto.v_NroDocumento = _compra.v_Serie + "-" + _compra.v_Correlativo;
                                DH_tesoreriadetalleDto.v_NroDocumentoRef = Fila.v_DocumentoRef;
                                DH_tesoreriadetalleDto.v_OrigenDestino = "";
                                DH_tesoreriadetalleDto.v_Pedido = "";
                                DH_tesoreriadetalleDto.t_Fecha = _compra.t_FechaEmision;
                                DH_tesoreriadetalleDto.i_IdTipoDocumento = _compra.i_IdTipoDocumento.Value;
                                DH_tesoreriadetalleDto.i_IdCentroCostos = "0";
                                DH_tesoreriadetalleDto.i_IdCaja = 0;
                                if (_objDocumentoBL.DocumentoEsContable(DH_tesoreriadetalleDto.i_IdTipoDocumento.Value))
                                {
                                    _TesoreriaDetalleXInsertar.Add(DH_tesoreriadetalleDto);
                                }
                            }
                        }

                        //foreach (var Fila in pagoDetalles.Where(p => p.i_IdTipoDocumentoRef == 7).GroupBy(g => g.v_IdCompra))
                        foreach (var Fila in pagoDetalles.Where(p => _objDocumentoBL.DocumentoEsInverso(p.i_IdTipoDocumentoRef.Value)).GroupBy(g => g.v_IdCompra))
                        {
                            string idCompra = Fila.FirstOrDefault().v_IdCompra;
                            compra _compra = (from v in dbContext.compra
                                              where v.v_IdCompra == idCompra
                                              select v).FirstOrDefault();

                            string IDConcepto = _compra.i_IdTipoCompra.Value.ToString("00");

                            string NroCuenta = (from v in dbContext.administracionconceptos
                                                where v.v_Codigo == IDConcepto  && v.i_Eliminado == 0 && v.v_Periodo.Equals(periodo)
                                                select new { v.v_CuentaPVenta }).FirstOrDefault().v_CuentaPVenta;

                            // var FilaComun = _TesoreriaDetalleXInsertar.Where(p => p.v_Naturaleza == "H" && p.v_NroCuenta == NroCuenta && p.i_IdTipoDocumento == _compra.i_IdTipoDocumento && p.v_NroDocumento == _compra.v_SerieDocumento + "-" + _compra.v_CorrelativoDocumento).FirstOrDefault();
                            var FilaComun = _TesoreriaDetalleXInsertar.Where(p => p.v_Naturaleza == "D" && p.v_NroCuenta == NroCuenta && p.i_IdTipoDocumento == _compra.i_IdTipoDocumento && p.v_NroDocumento == _compra.v_SerieDocumento + "-" + _compra.v_CorrelativoDocumento).FirstOrDefault();

                            if (FilaComun != null)
                            {
                                FilaComun.d_Importe = FilaComun.d_Importe + Fila.Sum(p => p.d_ImporteSoles.Value);
                                FilaComun.d_Cambio = C_tesoreriaDto.i_IdMoneda == 1 ? FilaComun.d_Importe / C_tesoreriaDto.d_TipoCambio : FilaComun.d_Importe * C_tesoreriaDto.d_TipoCambio;
                                FilaComun.d_Cambio = Utils.Windows.DevuelveValorRedondeado(FilaComun.d_Cambio.Value, 2);
                            }
                            else
                            {
                                tesoreriadetalleDto DH_tesoreriadetalleDto = new tesoreriadetalleDto();
                                decimal ImporteH = Fila.Sum(p => p.d_ImporteSoles.Value);
                                DH_tesoreriadetalleDto.v_IdTesoreria = C_tesoreriaDto.v_IdTesoreria;
                                DH_tesoreriadetalleDto.d_Cambio = C_tesoreriaDto.i_IdMoneda == 1 ? ImporteH / C_tesoreriaDto.d_TipoCambio : ImporteH * C_tesoreriaDto.d_TipoCambio;
                                DH_tesoreriadetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(DH_tesoreriadetalleDto.d_Cambio.Value, 2);
                                DH_tesoreriadetalleDto.d_Importe = ImporteH;
                                DH_tesoreriadetalleDto.i_IdTipoDocumentoRef = Fila.FirstOrDefault().i_IdTipoDocumentoRef;

                                DH_tesoreriadetalleDto.v_Naturaleza = "D";
                                DH_tesoreriadetalleDto.v_IdCliente = (from v in dbContext.compra
                                                                      join J1 in dbContext.cliente on v.v_IdProveedor equals J1.v_IdCliente
                                                                      where v.v_IdCompra == idCompra
                                                                      select new { J1.v_IdCliente }).FirstOrDefault().v_IdCliente;

                                DH_tesoreriadetalleDto.v_Analisis = "";
                                DH_tesoreriadetalleDto.v_NroCuenta = NroCuenta;

                                DH_tesoreriadetalleDto.v_NroDocumento = _compra.v_SerieDocumento + "-" + _compra.v_CorrelativoDocumento;
                                DH_tesoreriadetalleDto.v_NroDocumentoRef = Fila.FirstOrDefault().v_DocumentoRef;
                                DH_tesoreriadetalleDto.v_OrigenDestino = "";
                                DH_tesoreriadetalleDto.v_Pedido = "";
                                DH_tesoreriadetalleDto.t_Fecha = _compra.t_FechaRegistro;
                                DH_tesoreriadetalleDto.i_IdTipoDocumento = _compra.i_IdTipoDocumento.Value;
                                DH_tesoreriadetalleDto.i_IdCentroCostos = "0";
                                DH_tesoreriadetalleDto.i_IdCaja = 0;
                                if (_objDocumentoBL.DocumentoEsContable(DH_tesoreriadetalleDto.i_IdTipoDocumento.Value))
                                {
                                    _TesoreriaDetalleXInsertar.Add(DH_tesoreriadetalleDto);
                                }
                            }

                            tesoreriadetalleDto DD_tesoreriadetalleDto = new tesoreriadetalleDto();
                            decimal ImporteD = Fila.Sum(p => p.d_ImporteSoles.Value);
                            DD_tesoreriadetalleDto.v_IdTesoreria = C_tesoreriaDto.v_IdTesoreria;
                            DD_tesoreriadetalleDto.d_Cambio = C_tesoreriaDto.i_IdMoneda == 1 ? ImporteD / C_tesoreriaDto.d_TipoCambio : ImporteD * C_tesoreriaDto.d_TipoCambio;
                            DD_tesoreriadetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(DD_tesoreriadetalleDto.d_Cambio.Value, 2);
                            DD_tesoreriadetalleDto.d_Importe = ImporteD;
                            DD_tesoreriadetalleDto.i_IdTipoDocumentoRef = Fila.FirstOrDefault().i_IdTipoDocumentoRef;

                            DD_tesoreriadetalleDto.v_Naturaleza = "H";
                            DD_tesoreriadetalleDto.v_IdCliente = (from v in dbContext.compra
                                                                  join J1 in dbContext.cliente on v.v_IdProveedor equals J1.v_IdCliente
                                                                  where v.v_IdCompra == idCompra
                                                                  select new { J1.v_IdCliente }).FirstOrDefault().v_IdCliente;
                            DD_tesoreriadetalleDto.v_Analisis = "";
                            DD_tesoreriadetalleDto.v_NroCuenta = NroCuenta;
                            DD_tesoreriadetalleDto.v_NroDocumento = _compra.v_SerieDocumento + "-" + _compra.v_CorrelativoDocumento;
                            DD_tesoreriadetalleDto.v_NroDocumentoRef = Fila.FirstOrDefault().v_DocumentoRef;
                            DD_tesoreriadetalleDto.v_OrigenDestino = "";
                            DD_tesoreriadetalleDto.v_Pedido = "";
                            DD_tesoreriadetalleDto.t_Fecha = _compra.t_FechaRegistro;
                            DD_tesoreriadetalleDto.i_IdTipoDocumento = _compra.i_IdTipoDocumento.Value;
                            DD_tesoreriadetalleDto.i_IdCentroCostos = "0";
                            DD_tesoreriadetalleDto.i_IdCaja = 0;
                            if (_objDocumentoBL.DocumentoEsContable(DD_tesoreriadetalleDto.i_IdTipoDocumento.Value))
                            {
                                _TesoreriaDetalleXInsertar.Add(DD_tesoreriadetalleDto);
                            }
                        }

                        List<String> IdVentas = new List<string>();
                        string NombreTesoreria = string.Empty;
                        IdVentas = pagoDetalles.Select(p => p.v_IdCompra).ToList();

                        if (pagoDetalles.Count() == 1)
                        {
                            if (pagoDetalles[0].i_EsLetra != 1)
                            {
                                string IdVenta = pagoDetalles[0].v_IdCompra;
                                C_tesoreriaDto.v_Nombre = (from v in dbContext.compra

                                                           join J1 in dbContext.cliente on v.v_IdProveedor equals J1.v_IdCliente into J1_join
                                                           from J1 in J1_join.DefaultIfEmpty()

                                                           where v.v_IdCompra == IdVenta

                                                           select new
                                                           {
                                                               Nombre = v.v_IdProveedor != "N002-CL000000000" ? (J1.v_PrimerNombre + " " + J1.v_ApePaterno + " " + J1.v_ApeMaterno + " " + J1.v_RazonSocial).Trim() : string.Empty
                                                           }
                                                           ).FirstOrDefault().Nombre;
                            }
                            else
                            {
                                string IdVenta = pagoDetalles[0].v_IdCompra;
                                C_tesoreriaDto.v_Nombre = (from v in dbContext.letraspagardetalle

                                                           join J1 in dbContext.cliente on v.v_IdProveedor equals J1.v_IdCliente into J1_join
                                                           from J1 in J1_join.DefaultIfEmpty()

                                                           where v.v_IdLetrasPagarDetalle == IdVenta

                                                           select new
                                                           {
                                                               Nombre = v.v_IdProveedor != "N002-CL000000000" ? (J1.v_PrimerNombre + " " + J1.v_ApePaterno + " " + J1.v_ApeMaterno + " " + J1.v_RazonSocial).Trim() : "PÚBLICO GENERAL"
                                                           }
                                                           ).FirstOrDefault().Nombre;
                            }
                        }
                        else
                        {
                            C_tesoreriaDto.v_Nombre = "PAGOS VARIOS";
                        }

                        //if (_TesoreriaDetalleXInsertar.Count(p => p.i_IdTipoDocumentoRef != 7) > 0)
                        if (_TesoreriaDetalleXInsertar.Count(p => !_objDocumentoBL.DocumentoEsInverso(p.i_IdTipoDocumentoRef.Value)) > 0)
                        {
                            tesoreriadetalleDto DD_tesoreriadetalleDto = new tesoreriadetalleDto();
                            //decimal Importe = _TesoreriaDetalleXInsertar.Where(o => o.i_IdTipoDocumentoRef != 7).Sum(p => p.d_Importe.Value) - _TesoreriaDetalleXInsertar.Where(o => o.i_IdTipoDocumentoRef == 7).Sum(p => p.d_Importe.Value);

                            decimal Importe = _TesoreriaDetalleXInsertar.Where(o => !_objDocumentoBL.DocumentoEsInverso(o.i_IdTipoDocumentoRef.Value)).Sum(p => p.d_Importe.Value) - _TesoreriaDetalleXInsertar.Where(o => _objDocumentoBL.DocumentoEsInverso(o.i_IdTipoDocumentoRef.Value)).Sum(p => p.d_Importe.Value);

                            DD_tesoreriadetalleDto.v_IdTesoreria = C_tesoreriaDto.v_IdTesoreria;
                            DD_tesoreriadetalleDto.d_Cambio = C_tesoreriaDto.i_IdMoneda == 1 ? Importe / C_tesoreriaDto.d_TipoCambio : Importe * C_tesoreriaDto.d_TipoCambio;
                            DD_tesoreriadetalleDto.d_Importe = Importe;
                            DD_tesoreriadetalleDto.i_IdTipoDocumentoRef = _TesoreriaDetalleXInsertar.Count() == 1 ? _TesoreriaDetalleXInsertar[0].i_IdTipoDocumento.Value : -1;

                            DD_tesoreriadetalleDto.v_Naturaleza = "H";
                            DD_tesoreriadetalleDto.v_Analisis = "";
                            DD_tesoreriadetalleDto.v_NroCuenta = C_tesoreriaDto.v_NroCuentaCajaBanco;
                            DD_tesoreriadetalleDto.v_NroDocumento = "";
                            DD_tesoreriadetalleDto.v_NroDocumentoRef = _TesoreriaDetalleXInsertar.Count() == 1 ? _TesoreriaDetalleXInsertar[0].v_NroDocumento : string.Empty;
                            DD_tesoreriadetalleDto.v_OrigenDestino = "";
                            DD_tesoreriadetalleDto.v_Pedido = "";
                            DD_tesoreriadetalleDto.t_Fecha = C_tesoreriaDto.t_FechaRegistro;
                            DD_tesoreriadetalleDto.i_IdTipoDocumento = 0;
                            DD_tesoreriadetalleDto.i_IdCentroCostos = "";
                            DD_tesoreriadetalleDto.i_IdCaja = 0;
                            _TesoreriaDetalleXInsertar.Add(DD_tesoreriadetalleDto);
                        }

                        decimal TotDebe, TotHaber, TotDebeC, TotHaberC;
                        TotDebe = _TesoreriaDetalleXInsertar.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Importe.Value);
                        TotHaber = _TesoreriaDetalleXInsertar.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Importe.Value);
                        TotDebeC = _TesoreriaDetalleXInsertar.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Cambio.Value);
                        TotHaberC = _TesoreriaDetalleXInsertar.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Cambio.Value);
                        C_tesoreriaDto.d_TotalDebe_Importe = TotDebe;
                        C_tesoreriaDto.d_TotalHaber_Importe = TotHaber;
                        C_tesoreriaDto.d_TotalDebe_Cambio = TotDebeC;
                        C_tesoreriaDto.d_TotalHaber_Cambio = TotHaberC;
                        C_tesoreriaDto.d_Diferencia_Importe = TotDebe - TotHaber;
                        C_tesoreriaDto.d_Diferencia_Cambio = TotDebeC - TotHaberC;
                        if (_TesoreriaDetalleXInsertar.Any())
                        {
                            _objTesoreriaBL.Insertartesoreria(ref pobjOperationResult, C_tesoreriaDto, Globals.ClientSession.GetAsList(), _TesoreriaDetalleXInsertar);
                            if (pobjOperationResult.Success == 0) return null;
                        }

                        C_tesoreriaDto = new tesoreriaDto();
                        _TesoreriaDetalleXInsertar = new List<tesoreriadetalleDto>();
                    }

                    #endregion

                    dbContext.SaveChanges();
                    Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "pago", pobjDtoEntity.v_IdPago);
                    pobjOperationResult.Success = 1;
                    ts.Complete();
                    return pobjDtoEntity.v_IdPago;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "pagoBL.Actualizarpago()" + pobjOperationResult.AdditionalInformation;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = pobjOperationResult.ExceptionMessage != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public void RecalcularSaldoCompra(ref OperationResult pobjOperationResult, string pstrIdCompra, List<string> ClientSession, bool EsLetra, bool recalcularReferencia = false)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        compra objCompra = dbContext.compra.FirstOrDefault(p => p.v_IdCompra == pstrIdCompra && p.i_Eliminado.Value == 0);
                        if (objCompra != null)
                        {
                            if (_objDocumentoBL.DocumentoEsInverso(objCompra.i_IdTipoDocumento.Value) && recalcularReferencia)
                            {
                                objCompra = dbContext.compra.FirstOrDefault(p => p.i_Eliminado == 0 && p.i_IdTipoDocumento == objCompra.i_IdTipoDocumentoRef && p.v_SerieDocumento.Equals(objCompra.v_SerieDocumentoRef.Trim())
                                 && p.v_CorrelativoDocumento.Equals(objCompra.v_CorrelativoDocumentoRef.Trim()) && p.i_IdEstado == 1 && p.v_IdProveedor.Equals(objCompra.v_IdProveedor));
                                if (objCompra == null)
                                {
                                    ts.Complete();
                                    return
                                       ;
                                }
                                pstrIdCompra = objCompra.v_IdCompra;

                            }
                            pagopendiente objPendiente = dbContext.pagopendiente.FirstOrDefault(p => p.v_IdCompra == pstrIdCompra && p.i_Eliminado == 0);
                            IEnumerable<pagodetalle> objPagos = dbContext.pagodetalle.Where(p => p.v_IdCompra == pstrIdCompra && p.i_Eliminado == 0).ToList();
                            IEnumerable<letraspagarcanje> objCanjes = dbContext.letraspagarcanje.Where(p => p.v_IdCompra == pstrIdCompra && p.i_Eliminado == 0).ToList();

                            IEnumerable<compra> objNotasCredito = dbContext.compra.Where(p => p.v_SerieDocumentoRef.Trim() == objCompra.v_SerieDocumento.Trim() && p.v_CorrelativoDocumentoRef.Trim() == objCompra.v_CorrelativoDocumento.Trim()
                                       && p.i_Eliminado == 0).ToList().Where(o => _objDocumentoBL.DocumentoEsInverso(o.i_IdTipoDocumento.Value));


                            if (objPendiente != null)
                            {
                                objPendiente.d_Saldo = objCompra.d_Total.Value;
                                objPendiente.d_Acuenta = 0;
                            }
                            else
                            {
                                //var objPagoPendienteEntity = new pagopendienteDto();
                                //var objEntity = objPagoPendienteEntity.ToEntity();
                                //objEntity.v_IdCompra = pstrIdCompra;
                                //objEntity.d_Acuenta = 0;
                                //objEntity.d_Saldo =
                                //    objCompra.d_Total.Value;
                                //objEntity.t_InsertaFecha = DateTime.Now;
                                //objEntity.i_InsertaIdUsuario = int.Parse(ClientSession[2]);
                                //objEntity.i_Eliminado = 0;
                                //// Autogeneramos el Pk de la tabla
                                //var intNodeId = int.Parse(ClientSession[0]);
                                //int  SecuentialId = new SecuentialBL  ().GetNextSecuentialId(intNodeId, 80);
                                //string newId = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "PC");
                                //objEntity.v_IdPagoPendiente = newId;
                                //dbContext.AddTopagopendiente(objEntity);
                                //dbContext.SaveChanges();
                                //Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName,
                                //    "pagopendiente", newId);

                            }
                            dbContext.SaveChanges();

                            foreach (var Pago in objPagos.Where(p => p.pago.i_IdEstado == 1))
                            {
                                ProcesaDetallePago(ref pobjOperationResult, pagodetalleAssembler.ToDTO(Pago), pagoAssembler.ToDTO(Pago.pago), ClientSession);
                                if (pobjOperationResult.Success == 0) return;

                            }

                            foreach (var Canje in objCanjes)
                            {
                                ProcesarCanjeLetraPago(ref pobjOperationResult, Canje.letraspagar, Canje, pstrIdCompra);
                                if (pobjOperationResult.Success == 0) return;
                            }

                            foreach (var comprasNCR in objNotasCredito)
                            {

                                new ComprasBL().EliminarPagoPendiente(ref pobjOperationResult, comprasNCR.v_IdCompra, Globals.ClientSession.GetAsList(), comprasNCR.i_IdEstado.Value, comprasNCR.i_Eliminado.Value);
                                if (pobjOperationResult.Success == 0) return;
                                new ComprasBL().InsertaPagosPendiente(ref pobjOperationResult, comprasNCR.v_IdCompra, comprasNCR.d_Total.Value, ClientSession);
                                if (pobjOperationResult.Success == 0) return;
                            }
                        }

                        ts.Complete();
                        pobjOperationResult.Success = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                pobjOperationResult.AdditionalInformation = "PagoBL.RecalcularSaldoCompra()" + pstrIdCompra;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        void ProcesarCanjeLetraPago(ref OperationResult pobjOperationResult, letraspagar PagoEntity, letraspagarcanje pagodetalleDto, string IdCompra)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region Actualiza la deuda de una venta

                    var cps = dbContext.pagopendiente.Where(p => p.v_IdCompra == IdCompra && p.i_Eliminado == 0).ToList();

                    if (cps != null)
                    {
                        #region Se procede a recorrer la lista de cobranzas a cancelar
                        foreach (var cp in cps)
                        {
                            var CompraOriginal = (from m in dbContext.compra
                                                  where m.v_IdCompra == cp.v_IdCompra && m.i_Eliminado == 0
                                                  select new { m.i_IdMoneda, m.d_TipoCambio }).FirstOrDefault();

                            int Moneda = CompraOriginal.i_IdMoneda.Value;

                            decimal TCambio = CompraOriginal.d_TipoCambio.Value;

                            switch (PagoEntity.i_IdMoneda)
                            {
                                case 1:
                                    switch (Moneda)
                                    {
                                        case 1:
                                            cp.d_Acuenta = cp.d_Acuenta.Value + pagodetalleDto.d_MontoCanjeado;
                                            cp.d_Saldo = cp.d_Saldo.Value - pagodetalleDto.d_MontoCanjeado >= 0 ? cp.d_Saldo.Value - pagodetalleDto.d_MontoCanjeado : 0;

                                            break;

                                        case 2:
                                            cp.d_Acuenta = cp.d_Acuenta.Value + (pagodetalleDto.d_MontoCanjeado / TCambio);
                                            cp.d_Saldo = cp.d_Saldo.Value - (pagodetalleDto.d_MontoCanjeado / TCambio) >= 0 ? cp.d_Saldo.Value - (pagodetalleDto.d_MontoCanjeado / TCambio) : 0;

                                            break;
                                    }
                                    break;

                                case 2:
                                    switch (Moneda)
                                    {
                                        case 1:
                                            cp.d_Acuenta = cp.d_Acuenta.Value + (pagodetalleDto.d_MontoCanjeado * TCambio);
                                            cp.d_Saldo = cp.d_Saldo.Value - (pagodetalleDto.d_MontoCanjeado * TCambio) >= 0 ? cp.d_Saldo.Value - (pagodetalleDto.d_MontoCanjeado * TCambio) : 0;

                                            break;

                                        case 2:
                                            cp.d_Acuenta = cp.d_Acuenta.Value + pagodetalleDto.d_MontoCanjeado;
                                            cp.d_Saldo = cp.d_Saldo.Value - pagodetalleDto.d_MontoCanjeado >= 0 ? cp.d_Saldo.Value - pagodetalleDto.d_MontoCanjeado : 0;

                                            break;
                                    }
                                    break;
                            }

                            dbContext.pagopendiente.ApplyCurrentValues(cp);
                            dbContext.SaveChanges();
                            Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "pagopendiente", cp.v_IdPagoPendiente);
                        }
                        #endregion
                    }
                    else
                    {
                        pobjOperationResult.Success = 0;
                        pobjOperationResult.ErrorMessage = "No se encontró el pago pendiente";
                        pobjOperationResult.AdditionalInformation = "PagoBL.ProcesarCanjeLetraPago()";
                        Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                    }
                    #endregion

                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                pobjOperationResult.AdditionalInformation = "PagoBL.ProcesarCanjeLetraPago()";
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }



        public void ProcesoRecalcularSaldoTodosPagos(ref OperationResult objOperationResult)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {


                        //new CobranzaBL().RecalcularSaldoVenta
                        var Periodo = Globals.ClientSession.i_Periodo.ToString();
                        var TodasCompras = dbContext.compra.Where(l => l.i_Eliminado == 0 && l.v_Periodo == Periodo).ToList().ToDTOs().OrderBy(l => l.v_IdCompra).ToList();

                        foreach (var item in TodasCompras)
                        {

                            if (item.v_IdCompra == "N001-ZZ000000061")
                            {
                                string h = "";
                            }
                            if (item.v_IdCompra == "N001-ZZ000000060")
                            {
                                string h = "";
                            }
                            new PagoBL().RecalcularSaldoCompra(ref objOperationResult, item.v_IdCompra,
                             Globals.ClientSession.GetAsList(), false, new DocumentoBL().DocumentoEsInverso(item.i_IdTipoDocumento ?? 0));
                            if (objOperationResult.Success == 0)
                            {

                                return;
                            }
                        }

                        dbContext.SaveChanges();


                    }
                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;

            }


        }
        private void ProcesaDetallePago(ref OperationResult pobjOperationResult, pagodetalleDto pagodetalleDto, pagoDto PagoEntity, List<string> ClientSession)
        {
            try
            {
                DocumentoBL _objDocumentoBL = new DocumentoBL();
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        List<pagopendiente> cps = new List<pagopendiente>();

                        #region Obtiene el Listado de pagos a realizar
                        //if (pagodetalleDto.i_IdTipoDocumentoRef != 7) //Si la pago no fue pagada con una nota de credito solo se procede a recoger la pago de la venta.
                        if (!_objDocumentoBL.DocumentoEsInverso(pagodetalleDto.i_IdTipoDocumentoRef.Value)) //Si la pago no fue pagada con una nota de credito solo se procede a recoger la pago de la venta.
                        {
                            cps = (from n in dbContext.pagopendiente
                                   where n.v_IdCompra == pagodetalleDto.v_IdCompra && n.i_Eliminado == 0
                                   select n).ToList();
                        }
                        else
                        {
                            //si la pago fue hecha en parte con una nota de credito se procede a incluir a la NRC en la lista de ventas por cancelar.
                            string[] SerieCorrelativo = pagodetalleDto.v_DocumentoRef.Split(new Char[] { '-' });

                            if (SerieCorrelativo.Count() == 2)
                            {
                                string Serie = SerieCorrelativo[0].Trim();
                                string Correlativo = SerieCorrelativo[1].Trim();
                                // var CompraNRC = dbContext.compra.Where(p => p.i_IdTipoDocumento == 7 && p.v_CorrelativoDocumento == Correlativo && p.v_SerieDocumento == Serie).FirstOrDefault();
                                var CompraNRC = dbContext.compra.Where(p => p.v_CorrelativoDocumento == Correlativo && p.v_SerieDocumento == Serie).ToList().FirstOrDefault(p => _objDocumentoBL.DocumentoEsInverso(p.i_IdTipoDocumento.Value));
                                if (CompraNRC != null)
                                {
                                    cps = (from n in dbContext.pagopendiente
                                           where (n.v_IdCompra == pagodetalleDto.v_IdCompra || n.v_IdCompra == CompraNRC.v_IdCompra) && n.i_Eliminado == 0
                                           select n).ToList();
                                }
                                else
                                {
                                    pobjOperationResult.Success = 0;
                                    pobjOperationResult.ErrorMessage = "No se encontró la nota de crédito";
                                    pobjOperationResult.AdditionalInformation = "PagoBL.ProcesaDetallePago()";
                                    return;
                                }
                            }
                            else
                            {
                                pobjOperationResult.Success = 0;
                                pobjOperationResult.ErrorMessage = "Nota de credito no válida";
                                pobjOperationResult.AdditionalInformation = "PagoBL.ProcesaDetallePago()";
                                return;
                            }
                        }
                        #endregion

                        if (cps != null)
                        {
                            #region Se procede a recorrer la lista de pagos a cancelar
                            foreach (var cp in cps)
                            {
                                int Moneda = (from m in dbContext.compra
                                              where m.v_IdCompra == pagodetalleDto.v_IdCompra && m.i_Eliminado == 0
                                              select new { m.i_IdMoneda }).FirstOrDefault().i_IdMoneda.Value;

                                decimal TCambio = PagoEntity.d_TipoCambio.Value;

                                switch (PagoEntity.i_IdMoneda)
                                {
                                    case 1:
                                        switch (Moneda)
                                        {
                                            case 1:
                                                cp.d_Acuenta = cp.d_Acuenta.Value + pagodetalleDto.d_ImporteSoles.Value;
                                                cp.d_Saldo = cp.d_Saldo.Value - pagodetalleDto.d_ImporteSoles.Value >= 0 ? cp.d_Saldo.Value - pagodetalleDto.d_ImporteSoles.Value : 0;

                                                break;

                                            case 2:
                                                cp.d_Acuenta = cp.d_Acuenta.Value + (pagodetalleDto.d_ImporteSoles.Value / TCambio);
                                                cp.d_Saldo = cp.d_Saldo.Value - (pagodetalleDto.d_ImporteSoles.Value / TCambio) >= 0 ? cp.d_Saldo.Value - (pagodetalleDto.d_ImporteSoles.Value / TCambio) : 0;

                                                break;
                                        }
                                        break;

                                    case 2:
                                        switch (Moneda)
                                        {
                                            case 1:
                                                cp.d_Acuenta = cp.d_Acuenta.Value + (pagodetalleDto.d_ImporteSoles.Value * TCambio);
                                                cp.d_Saldo = cp.d_Saldo.Value - (pagodetalleDto.d_ImporteSoles.Value * TCambio) >= 0 ? cp.d_Saldo.Value - (pagodetalleDto.d_ImporteSoles.Value * TCambio) : 0;

                                                break;

                                            case 2:
                                                cp.d_Acuenta = cp.d_Acuenta.Value + pagodetalleDto.d_ImporteSoles.Value;
                                                cp.d_Saldo = cp.d_Saldo.Value - pagodetalleDto.d_ImporteSoles.Value >= 0 ? cp.d_Saldo.Value - pagodetalleDto.d_ImporteSoles.Value : 0;

                                                break;
                                        }
                                        break;
                                }
                                //dbContext.pagopendiente.ApplyCurrentValues(cp);
                                dbContext.pagopendiente.ApplyCurrentValues(cp);
                                /* DisminuyeLineaCredito(ref pobjOperationResult, cp.v_IdVenta, pagodetalleDto, PagoEntity.i_IdMoneda.Value);*/
                                // if (pobjOperationResult.Success == 0) return;
                                Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "pagopendiente", cp.v_IdPagoPendiente);
                            }
                            #endregion

                            /*  #region Actualizar Saldo de Adelanto
                       if (pagodetalleDto.i_IdTipoDocumentoRef.Value == 433) //se iguala al id del documento para adelantos.
                       {
                           string[] SerieCorrelativo = pagodetalleDto.v_DocumentoRef.Split(new Char[] { '-' });
                           string Serie = SerieCorrelativo[0].Trim();
                           string Correlativo = SerieCorrelativo[1].Trim();

                           adelanto _adelanto = (from a in dbContext.adelanto
                                                 where a.v_SerieDocumento == Serie && a.v_CorrelativoDocumento == Correlativo
                                                 select a).FirstOrDefault();

                           _adelanto.d_Consumo = _adelanto.d_Consumo != null ? _adelanto.d_Consumo.Value + pagodetalleDto.d_ImporteSoles : 0;
                           _adelanto.d_Saldo = _adelanto.d_Saldo.Value - pagodetalleDto.d_ImporteSoles;
                           dbContext.adelanto.ApplyCurrentValues(_adelanto);
                           Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "adelanto", _adelanto.v_IdAdelanto);
                       }
                       #endregion*/

                        }
                        else
                        {
                            pobjOperationResult.Success = 0;
                            pobjOperationResult.ErrorMessage = "No se encontró la pago pendiente";
                            pobjOperationResult.AdditionalInformation = "pagoBL.ProcesaDetallepago()";
                            Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                        }

                        dbContext.SaveChanges();
                        pobjOperationResult.Success = 1;
                        ts.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                pobjOperationResult.AdditionalInformation = "pagoBL.ProcesaDetallepago()";
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }

        }

        public decimal DevuelveMontoNotaCredito(string NroDocumento)
        {
            try
            {
                DocumentoBL _objDocumentoBL = new DocumentoBL();
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    string[] SerieCorrelativo = new string[2];
                    string Serie, Correlativo;
                    SerieCorrelativo = NroDocumento.Split(new Char[] { '-' });
                    Serie = SerieCorrelativo[0].Trim();
                    Correlativo = SerieCorrelativo[1].Trim();
                    // compra CompraNRC = dbContext.compra.Where(p => p.i_IdTipoDocumento == 7 && p.v_SerieDocumento == Serie && p.v_CorrelativoDocumento == Correlativo).FirstOrDefault();
                    compra CompraNRC = dbContext.compra.Where(p => p.v_SerieDocumento == Serie && p.v_CorrelativoDocumento == Correlativo && p.i_Eliminado == 0).ToList().FirstOrDefault(x => _objDocumentoBL.DocumentoEsInverso(x.i_IdTipoDocumento.Value));
                    if (CompraNRC != null)
                    {
                        pagopendiente PendienteNRC = dbContext.pagopendiente.Where(p => p.v_IdCompra == CompraNRC.v_IdCompra && p.i_Eliminado == 0).FirstOrDefault();
                        if (PendienteNRC != null)
                        {
                            return PendienteNRC.d_Saldo.Value;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        return 0;
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void RestauraDetallePagoLetras(ref OperationResult pobjOperationResult, string pstrIdPagoDetalle, List<string> ClientSession)
        {
            try
            {
                //    using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                //    {
                //        SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                //        List<cobranzaletraspagarpendiente> cps = new List<cobranzaletraspagarpendiente>();

                //        var cobranzadetalleDto = (from n in dbContext.pagodetalle
                //                                  where n.v_IdPagoDetalle == pstrIdCobranzaDetalle
                //                                  select n).FirstOrDefault();

                //        var CobranzaEntity = (from c in dbContext.pago
                //                              where c.v_IdPago == cobranzadetalleDto.v_IdPago
                //                              select c).FirstOrDefault();

                //        if (cobranzadetalleDto != null)
                //        {
                //            decimal TCambio = CobranzaEntity.d_TipoCambio.Value;

                //            #region Obtiene el Listado de cobranzas a realizar

                //            cps = (from n in dbContext.cobranzaletraspagarpendiente
                //                   where n.v_IdLetrasPagarDetalle == cobranzadetalleDto.v_IdCompra && n.i_Eliminado == 0
                //                   select n).ToList();

                //            #endregion

                //            if (cps != null)
                //            {
                //                #region Se procede a recorrer la lista de cobranzas a cancelar
                //                foreach (var cp in cps)
                //                {
                //                    int Moneda = dbContext.letraspagardetalle.Where(p => p.v_IdLetrasPagarDetalle == cp.v_IdLetrasPagarDetalle && p.i_Eliminado == 0).FirstOrDefault().i_IdMoneda.Value;

                //                    switch (CobranzaEntity.i_IdMoneda)
                //                    {
                //                        case 1:
                //                            switch (Moneda)
                //                            {
                //                                case 1:
                //                                    cp.d_Acuenta = cp.d_Acuenta.Value - cobranzadetalleDto.d_ImporteSoles.Value;
                //                                    cp.d_Saldo = cp.d_Saldo.Value + cobranzadetalleDto.d_ImporteSoles.Value >= 0 ? cp.d_Saldo.Value + cobranzadetalleDto.d_ImporteSoles.Value : 0;

                //                                    break;

                //                                case 2:
                //                                    cp.d_Acuenta = cp.d_Acuenta.Value - (cobranzadetalleDto.d_ImporteSoles.Value / TCambio);
                //                                    cp.d_Saldo = cp.d_Saldo.Value + (cobranzadetalleDto.d_ImporteSoles.Value / TCambio) >= 0 ? cp.d_Saldo.Value + (cobranzadetalleDto.d_ImporteSoles.Value / TCambio) : 0;

                //                                    break;
                //                            }
                //                            break;

                //                        case 2:
                //                            switch (Moneda)
                //                            {
                //                                case 1:
                //                                    cp.d_Acuenta = cp.d_Acuenta.Value - (cobranzadetalleDto.d_ImporteSoles.Value * TCambio);
                //                                    cp.d_Saldo = cp.d_Saldo.Value + (cobranzadetalleDto.d_ImporteSoles.Value * TCambio) >= 0 ? cp.d_Saldo.Value + (cobranzadetalleDto.d_ImporteSoles.Value * TCambio) : 0;

                //                                    break;

                //                                case 2:
                //                                    cp.d_Acuenta = cp.d_Acuenta.Value - cobranzadetalleDto.d_ImporteSoles.Value;
                //                                    cp.d_Saldo = cp.d_Saldo.Value + cobranzadetalleDto.d_ImporteSoles.Value >= 0 ? cp.d_Saldo.Value + cobranzadetalleDto.d_ImporteSoles.Value : 0;

                //                                    break;
                //                            }
                //                            break;
                //                    }

                //                    if (cp.d_Saldo == 0) cp.letraspagardetalle.i_Pagada = 1;
                //                    else cp.letraspagardetalle.i_Pagada = 0;

                //                    dbContext.cobranzaletraspagarpendiente.ApplyCurrentValues(cp);
                //                    Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "cobranzaletraspagarpendiente", cp.v_IdLetrasPagarDetalle);
                //                }
                //                #endregion
                //            }
                //            else
                //            {
                //                pobjOperationResult.Success = 0;
                //                pobjOperationResult.ErrorMessage = "No se encontró la cobranza pendiente";
                //                pobjOperationResult.AdditionalInformation = "CobranzaBL.RestauraDetallePagoLetras()";
                //                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                //            }
                //        }
                //        else
                //        {
                //            pobjOperationResult.Success = 0;
                //            pobjOperationResult.ErrorMessage = "No se encontró el pago.";
                //            pobjOperationResult.AdditionalInformation = "CobranzaBL.RestauraDetalleCobranzaLetras()";
                //            Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                //        }

                //        dbContext.SaveChanges();
                //        pobjOperationResult.Success = 1;
                //        ts.Complete();
                //    }


                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var pagoD =
                            dbContext.pagodetalle.FirstOrDefault(
                                p => p.v_IdPagoDetalle.Equals(pstrIdPagoDetalle));

                        var idLetra = pagoD.v_IdCompra;
                        letraspagardetalle objCompra = dbContext.letraspagardetalle.FirstOrDefault(p => p.v_IdLetrasPagarDetalle == idLetra && p.i_Eliminado == 0);

                        if (objCompra != null)
                        {

                            cobranzaletraspagarpendiente objPendiente = dbContext.cobranzaletraspagarpendiente.FirstOrDefault(p => p.v_IdLetrasPagarDetalle == idLetra && p.i_Eliminado == 0);
                            // cobranzaletraspendiente objPendiente = dbContext.cobranzaletraspendiente.FirstOrDefault(p => p.v_IdLetrasDetalle == idLetra && p.i_Eliminado == 0);
                            //IEnumerable<cobranzadetalle> objCobranzas = dbContext.cobranzadetalle.Where(p => p.v_IdVenta == idLetra && p.i_Eliminado == 0).ToList();
                            //IEnumerable<letrascanje> objCanjes = dbContext.letrascanje.Where(p => p.v_IdVenta == idLetra && p.i_Eliminado == 0).ToList();

                            IEnumerable<pagodetalle> objPagos = dbContext.pagodetalle.Where(p => p.v_IdCompra == idLetra && p.i_Eliminado == 0).ToList();
                            IEnumerable<letraspagarcanje> objCanjes = dbContext.letraspagarcanje.Where(p => p.v_IdCompra == idLetra && p.i_Eliminado == 0).ToList();

                            objPendiente.d_Saldo = objCompra.d_Importe ?? 0;
                            objPendiente.d_Acuenta = 0;
                            dbContext.SaveChanges();

                            foreach (var Pago in objPagos.Where(p => p.pago.i_IdEstado == 1))
                            {
                                ProcesaDetallePagoLetras(ref pobjOperationResult, Pago.ToDTO(), Pago.pago.ToDTO(), ClientSession);
                                if (pobjOperationResult.Success == 0) return;
                            }

                            foreach (var Canje in objCanjes)
                            {
                                ProcesarCanjeLetraPago(ref pobjOperationResult, Canje.letraspagar, Canje, idLetra);
                                if (pobjOperationResult.Success == 0) return;
                            }
                        }

                        ts.Complete();
                        pobjOperationResult.Success = 1;
                    }
                }



            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "CobranzaBL.RestauraDetallePagoLetras()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = pobjOperationResult.ExceptionMessage != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }

        }

        private void ProcesaDetallePagoLetras(ref OperationResult pobjOperationResult, pagodetalleDto pagodetalleDto, pagoDto PagoEntity, List<string> ClientSession)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    List<cobranzaletraspagarpendiente> cps = new List<cobranzaletraspagarpendiente>();

                    #region Obtiene el Listado de cobranzas a realizar

                    cps = (from n in dbContext.cobranzaletraspagarpendiente
                           where n.v_IdLetrasPagarDetalle == pagodetalleDto.v_IdCompra && n.i_Eliminado == 0
                           select n).ToList();

                    #endregion

                    if (cps != null)
                    {
                        #region Se procede a recorrer la lista de cobranzas a cancelar
                        foreach (var cp in cps)
                        {
                            //var firstOrDefault = dbContext.letraspagardetalle.FirstOrDefault(p => p.v_IdLetrasPagarDetalle == cp.v_IdLetrasPagarDetalle && p.i_Eliminado == 0);


                            var firstOrDefault = dbContext.letraspagardetalle.FirstOrDefault(p => p.v_IdLetrasPagarDetalle == cp.v_IdLetrasPagarDetalle && p.i_Eliminado == 0);
                            if (firstOrDefault != null)
                            {
                                int Moneda = firstOrDefault.i_IdMoneda ?? 1;

                                decimal TCambio = PagoEntity.d_TipoCambio.Value;

                                switch (PagoEntity.i_IdMoneda)
                                {
                                    case 1:
                                        switch (Moneda)
                                        {
                                            case 1:
                                                cp.d_Acuenta = cp.d_Acuenta.Value + pagodetalleDto.d_ImporteSoles.Value;
                                                cp.d_Saldo = cp.d_Saldo.Value - pagodetalleDto.d_ImporteSoles.Value >= 0 ? cp.d_Saldo.Value - pagodetalleDto.d_ImporteSoles.Value : 0;

                                                break;

                                            case 2:
                                                cp.d_Acuenta = cp.d_Acuenta.Value + (pagodetalleDto.d_ImporteSoles.Value / TCambio);
                                                cp.d_Saldo = cp.d_Saldo.Value - (pagodetalleDto.d_ImporteSoles.Value / TCambio) >= 0 ? cp.d_Saldo.Value - (pagodetalleDto.d_ImporteSoles.Value / TCambio) : 0;

                                                break;
                                        }
                                        break;

                                    case 2:
                                        switch (Moneda)
                                        {
                                            case 1:
                                                cp.d_Acuenta = cp.d_Acuenta.Value + (pagodetalleDto.d_ImporteSoles.Value * TCambio);
                                                cp.d_Saldo = cp.d_Saldo.Value - (pagodetalleDto.d_ImporteSoles.Value * TCambio) >= 0 ? cp.d_Saldo.Value - (pagodetalleDto.d_ImporteSoles.Value * TCambio) : 0;

                                                break;

                                            case 2:
                                                cp.d_Acuenta = cp.d_Acuenta.Value + pagodetalleDto.d_ImporteSoles.Value;
                                                cp.d_Saldo = cp.d_Saldo.Value - pagodetalleDto.d_ImporteSoles.Value >= 0 ? cp.d_Saldo.Value - pagodetalleDto.d_ImporteSoles.Value : 0;

                                                break;
                                        }
                                        break;
                                }

                            }

                            if (cp.d_Saldo == 0) cp.letraspagardetalle.i_Pagada = 1;
                            else cp.letraspagardetalle.i_Pagada = 0;

                            dbContext.cobranzaletraspagarpendiente.ApplyCurrentValues(cp);

                            Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "cobranzaletraspagarpendiente", cp.v_IdLetrasPagarDetalle);
                        }
                        #endregion
                    }
                    else
                    {
                        pobjOperationResult.Success = 0;
                        pobjOperationResult.ErrorMessage = "No se encontró la pago pendiente";
                        pobjOperationResult.AdditionalInformation = "PagoBL.ProcesaDetallePagoLetras()";
                        Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                    }

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;

                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                pobjOperationResult.AdditionalInformation = "CobranzaBL.ProcesaDetallePagoLetras()";
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }

        }

        public compraDto ObtenerCobranzaPendientePorCompra(ref OperationResult pobjOperationResult, string pstrIdCompra)
        {
            try
            {
                SAMBHSEntitiesModelWin dbcontext = new SAMBHSEntitiesModelWin();
                compraDto _compraDto = new compraDto();

                compra _Compra = (from n in dbcontext.compra
                                  where n.v_IdCompra == pstrIdCompra
                                  select n
                                ).FirstOrDefault();

                _compraDto = compraAssembler.ToDTO(_Compra);

                var Saldo = (from n in dbcontext.cobranzapendiente
                             where n.v_IdVenta == pstrIdCompra && n.i_Eliminado == 0
                             select new { n.d_Saldo }).FirstOrDefault();

                _compraDto.SaldoPendiente = Saldo != null ? Saldo.d_Saldo.Value : 0;


                pobjOperationResult.Success = 1;
                return _compraDto;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PagoBL.ObtenerCobranzaPendientePorCompra()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = pobjOperationResult.ExceptionMessage != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public int DevuelveMedioPago(ref OperationResult pobjOperationResult, string NFormaPago)
        {
            try
            {
                SAMBHSEntitiesModelWin dbcontext = new SAMBHSEntitiesModelWin();

                var FormaPago = (from n in dbcontext.datahierarchy
                                 where n.i_GroupId == 46 && n.v_Value1 == NFormaPago
                                 select new { n.v_Value2 }).FirstOrDefault();

                var IdTipoPago = (from n in dbcontext.datahierarchy
                                  where n.i_GroupId == 44 && n.v_Value2 == FormaPago.v_Value2
                                  select new { n.i_ItemId }).FirstOrDefault();

                if (IdTipoPago != null)
                {
                    pobjOperationResult.Success = 1;
                    int _Id = IdTipoPago.i_ItemId;
                    return _Id;
                }
                else
                {
                    return 0;
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "CobranzaBL.DevuelveMedioPago()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = pobjOperationResult.ExceptionMessage != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return 0;
            }
        }

        public BindingList<GrillaPagoDetalleDto> ObtenerDetalleCompras(ref OperationResult pobjOperationResult, string pstrIdCompras)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = (from a in dbContext.pagopendiente

                             join b in dbContext.compra on new { IdCompra = a.v_IdCompra, eliminado = 0 } equals new { IdCompra = b.v_IdCompra, eliminado = b.i_Eliminado.Value } into b_join

                             from b in b_join.DefaultIfEmpty()

                             join c in dbContext.cliente on new { IdProv = b.v_IdProveedor, eliminado = 0, Flag = "V" } equals new { IdProv = c.v_IdCliente, eliminado = c.i_Eliminado.Value, Flag = c.v_FlagPantalla } into c_join

                             from c in c_join.DefaultIfEmpty()

                             join d in dbContext.documento on new { IdDocumento = b.i_IdTipoDocumento.Value, eliminado = 0 } equals new { IdDocumento = d.i_CodigoDocumento, eliminado = d.i_Eliminado.Value } into d_join

                             from d in d_join.DefaultIfEmpty()
                             where b.i_Eliminado == 0 && b.v_IdCompra == pstrIdCompras && a.i_Eliminado == 0
                             orderby b.t_InsertaFecha ascending
                             //select new pagodetalleDto 
                             select new GrillaPagoDetalleDto

                             {
                                 //v_IdPagoDetalle = a.v_IdPagoDetalle,
                                 //v_IdPago = a.v_IdPago,
                                 v_IdCompra = b.v_IdCompra,
                                 //i_IdFormaPago = b.i_IdFormaPago.Value,
                                 i_IdFormaPago = -1,
                                 i_IdTipoDocumentoRef = b.i_IdTipoDocumentoRef.Value,
                                 //i_IdMoneda = a.i_IdMoneda.Value,
                                 i_IdMoneda = b.i_IdMoneda,
                                 TipoDocumento = d.v_Siglas,
                                 NroDocumento = b.v_SerieDocumento + "-" + b.v_CorrelativoDocumento,
                                 NombreProveedor = (c.v_ApeMaterno + " " + c.v_ApeMaterno + " " + c.v_PrimerNombre + " " + c.v_RazonSocial).Trim(),
                                 IdMonedaCompra = b.i_IdMoneda.Value,
                                 MonedaOriginal = b.i_IdMoneda.Value == 1 ? "S./" : "US$.",
                                 i_EsLetra = 0,
                                 //d_ImporteSoles = b.d_Total, 
                                 d_ImporteDolares = 0,
                                 //d_NetoXCobrar = a.d_NetoXCobrar.Value,
                                 d_ImporteSoles = a.d_Saldo,
                                 d_NetoXCobrar = a.d_Saldo, // Lo que falta pagar
                                 i_Eliminado = a.i_Eliminado.Value,
                                 i_InsertaIdUsuario = a.i_InsertaIdUsuario.Value,
                                 t_InsertaFecha = a.t_InsertaFecha.Value,
                             }
                             ).ToList();

                var Result = new BindingList<GrillaPagoDetalleDto>(query);
                pobjOperationResult.Success = 1;
                return Result;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }


        #endregion

        #region Bandeja
        public List<pagoDto> ListarBusquedaPagos(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression, DateTime F_Ini, DateTime F_Fin, int TipoDocCompra, string SerieDocCompra, string CorrelativoDocCompra)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                List<pagoDto> objData = new List<pagoDto>();
                pobjOperationResult.Success = 1;
                if (TipoDocCompra != -1)
                {
                    var Compra = dbContext.compra.Where(k => k.i_IdTipoDocumento == TipoDocCompra && k.v_SerieDocumento == SerieDocCompra && k.v_CorrelativoDocumento == CorrelativoDocCompra);
                    if (Compra.Any())
                    {
                        var queryPagos = (from n in dbContext.pago
                                          join b in dbContext.pagodetalle on new { pd = n.v_IdPago, eliminado = 0 } equals new { pd = b.v_IdPago, eliminado = b.i_Eliminado.Value } into b_join
                                          from b in b_join.DefaultIfEmpty()

                                          join J2 in dbContext.systemuser on new { i_UpdateUserId = n.i_ActualizaIdUsuario.Value }
                                                                   equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                                          from J2 in J2_join.DefaultIfEmpty()

                                          join J3 in dbContext.systemuser on new { i_InsertUserId = n.i_InsertaIdUsuario.Value }
                                                                         equals new { i_InsertUserId = J3.i_SystemUserId } into J3_join
                                          from J3 in J3_join.DefaultIfEmpty()

                                          join J4 in dbContext.documento on new { i_IdTipoDocumento = n.i_IdTipoDocumento.Value }
                                                                         equals new { i_IdTipoDocumento = J4.i_CodigoDocumento } into J4_join
                                          from J4 in J4_join.DefaultIfEmpty()

                                          join J5 in dbContext.datahierarchy on new { i_IdMedioPago = n.i_IdMedioPago.Value, b = 44 }
                                                                         equals new { i_IdMedioPago = J5.i_ItemId, b = J5.i_GroupId } into J5_join
                                          from J5 in J5_join.DefaultIfEmpty()

                                          where n.i_Eliminado == 0 && n.t_FechaRegistro >= F_Ini && n.t_FechaRegistro <= F_Fin
                                          && n.i_IdEstablecimiento == Globals.ClientSession.i_IdEstablecimiento.Value

                                          select new pagoDto
                                          {

                                              v_IdPago = n.v_IdPago,
                                              v_Mes = n.v_Mes,
                                              v_Correlativo = n.v_Correlativo,
                                              i_IdMedioPago = n.i_IdMedioPago,
                                              i_IdTipoDocumento = n.i_IdTipoDocumento,
                                              NroRegistro = n.v_Mes.Trim() + "-" + n.v_Correlativo,
                                              TipoDocumento = J4.v_Siglas,
                                              t_FechaRegistro = n.t_FechaRegistro,
                                              d_TotalSoles = n.d_TotalSoles,
                                              d_TotalDolares = n.d_TotalDolares,
                                              i_IdEstado = n.i_IdEstado,
                                              t_InsertaFecha = n.t_InsertaFecha,
                                              t_ActualizaFecha = n.t_ActualizaFecha,
                                              v_UsuarioModificacion = J2.v_UserName,
                                              v_UsuarioCreacion = J3.v_UserName,
                                              MedioPago = J5.v_Value1,
                                              v_Periodo = n.v_Periodo,
                                              Moneda = n.i_IdMoneda == 1 ? "S" : "D",
                                              IdCompra = b.v_IdCompra,
                                          }
                                   );

                        if (!string.IsNullOrEmpty(pstrFilterExpression))
                        {
                            queryPagos = queryPagos.Where(pstrFilterExpression);
                        }
                        if (!string.IsNullOrEmpty(pstrSortExpression))
                        {
                            queryPagos = queryPagos.OrderBy(pstrSortExpression);
                        }
                        objData = queryPagos.ToList();

                        //objData = objData.Where(o => o.IdCompra.Contains(Compra.Select(o => o.v_IdCompra))).ToList();


                    }
                    return objData;

                }
                else
                {
                    var query = (from n in dbContext.pago

                                 join J2 in dbContext.systemuser on new { i_UpdateUserId = n.i_ActualizaIdUsuario.Value }
                                                                equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                                 from J2 in J2_join.DefaultIfEmpty()

                                 join J3 in dbContext.systemuser on new { i_InsertUserId = n.i_InsertaIdUsuario.Value }
                                                                equals new { i_InsertUserId = J3.i_SystemUserId } into J3_join
                                 from J3 in J3_join.DefaultIfEmpty()

                                 join J4 in dbContext.documento on new { i_IdTipoDocumento = n.i_IdTipoDocumento.Value }
                                                                equals new { i_IdTipoDocumento = J4.i_CodigoDocumento } into J4_join
                                 from J4 in J4_join.DefaultIfEmpty()

                                 join J5 in dbContext.datahierarchy on new { i_IdMedioPago = n.i_IdMedioPago.Value, b = 44 }
                                                                equals new { i_IdMedioPago = J5.i_ItemId, b = J5.i_GroupId } into J5_join
                                 from J5 in J5_join.DefaultIfEmpty()

                                 where n.i_Eliminado == 0 && n.t_FechaRegistro >= F_Ini && n.t_FechaRegistro <= F_Fin
                                 && n.i_IdEstablecimiento == Globals.ClientSession.i_IdEstablecimiento.Value

                                 select new pagoDto
                                 {

                                     v_IdPago = n.v_IdPago,
                                     v_Mes = n.v_Mes,
                                     v_Correlativo = n.v_Correlativo,
                                     i_IdMedioPago = n.i_IdMedioPago,
                                     i_IdTipoDocumento = n.i_IdTipoDocumento,
                                     NroRegistro = n.v_Mes.Trim() + "-" + n.v_Correlativo,
                                     TipoDocumento = J4.v_Siglas,
                                     t_FechaRegistro = n.t_FechaRegistro,
                                     d_TotalSoles = n.d_TotalSoles,
                                     d_TotalDolares = n.d_TotalDolares,
                                     i_IdEstado = n.i_IdEstado,
                                     t_InsertaFecha = n.t_InsertaFecha,
                                     t_ActualizaFecha = n.t_ActualizaFecha,
                                     v_UsuarioModificacion = J2.v_UserName,
                                     v_UsuarioCreacion = J3.v_UserName,
                                     MedioPago = J5.v_Value1,
                                     v_Periodo = n.v_Periodo,
                                     Moneda = n.i_IdMoneda == 1 ? "S" : "D",
                                 }
                                 );

                    if (!string.IsNullOrEmpty(pstrFilterExpression))
                    {
                        query = query.Where(pstrFilterExpression);
                    }
                    if (!string.IsNullOrEmpty(pstrSortExpression))
                    {
                        query = query.OrderBy(pstrSortExpression);
                    }

                    objData = query.ToList();

                    return objData;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PagoBL.ListarBusquedaPagos()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }


        public void EliminarPago(ref OperationResult pobjOperationResult, string pstrIdPago, List<string> ClientSession)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                    #region Elimina Cabecera
                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.pago
                                           where a.v_IdPago == pstrIdPago
                                           select a).FirstOrDefault();

                    // Crear la entidad con los datos actualizados
                    objEntitySource.t_ActualizaFecha = DateTime.Now;
                    objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntitySource.i_Eliminado = 1;
                    #endregion

                    #region Elimina Detalles
                    //Eliminar detalles del movimiento eliminado.
                    var objEntitySourceDetallesPago = (from a in dbContext.pagodetalle
                                                       where a.v_IdPago == pstrIdPago
                                                       select a).ToList();

                    foreach (var RegistroPagoDetalle in objEntitySourceDetallesPago)
                    {
                        RegistroPagoDetalle.t_ActualizaFecha = DateTime.Now;
                        RegistroPagoDetalle.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        RegistroPagoDetalle.i_Eliminado = 1;
                        dbContext.pagodetalle.ApplyCurrentValues(RegistroPagoDetalle);
                        dbContext.SaveChanges();

                        if (RegistroPagoDetalle.i_EsLetra == 0 || RegistroPagoDetalle.i_EsLetra == null)
                        {
                            RecalcularSaldoCompra(ref pobjOperationResult, RegistroPagoDetalle.v_IdCompra, ClientSession, false);
                            if (pobjOperationResult.Success == 0) return;
                        }
                        else
                        {
                            if (objEntitySource.i_IdEstado == 1) RestauraDetallePagoLetras(ref pobjOperationResult, RegistroPagoDetalle.v_IdPagoDetalle, ClientSession);
                            if (pobjOperationResult.Success == 0) return;
                        }

                        Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "pagodetalle", RegistroPagoDetalle.v_IdPagoDetalle);
                    }
                    #endregion

                    #region Elimina Tesoreria
                    new TesoreriaBL().EliminarTesoreriaXDocRef(ref pobjOperationResult, pstrIdPago, ClientSession);
                    if (pobjOperationResult.Success == 0) return;
                    #endregion

                    dbContext.SaveChanges();
                    Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "pago", objEntitySource.v_IdPago);
                    pobjOperationResult.Success = 1;
                    ts.Complete();
                    return;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "CobranzaBL.ActualizarCobranza()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = pobjOperationResult.ExceptionMessage != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }

        public void EliminarTodosPagos(ref OperationResult objOperationResult, List<string> ClientSession)
        {

            using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    string periodo =Globals.ClientSession.i_Periodo.ToString ();

                    var TodosPagos = (from a in dbContext.pago
                                      where a.i_Eliminado == 0 && a.v_Periodo ==periodo 
                                      select a).ToList();

                    foreach (var item in TodosPagos)
                    {
                        EliminarPago(ref objOperationResult, item.v_IdPago, ClientSession);
                    }

                    ts.Complete();
                }



            }



        }

        #endregion



        /* private void RestauraDetallePagoLetras(ref OperationResult pobjOperationResult, string pstrIdCobranzaDetalle, List<string> ClientSession)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                    List<cobranzaletraspendiente> cps = new List<cobranzaletraspendiente>();

                    var cobranzadetalleDto = (from n in dbContext.cobranzadetalle
                                              where n.v_IdCobranzaDetalle == pstrIdCobranzaDetalle
                                              select n).FirstOrDefault();

                    var CobranzaEntity = (from c in dbContext.cobranza
                                          where c.v_IdCobranza == cobranzadetalleDto.v_IdCobranza
                                          select c).FirstOrDefault();

                    if (cobranzadetalleDto != null)
                    {
                        decimal TCambio = CobranzaEntity.d_TipoCambio.Value;

                        #region Obtiene el Listado de cobranzas a realizar

                        cps = (from n in dbContext.cobranzaletraspendiente
                               where n.v_IdLetrasDetalle == cobranzadetalleDto.v_IdVenta && n.i_Eliminado == 0
                               select n).ToList();

                        #endregion

                        if (cps != null)
                        {
                            #region Se procede a recorrer la lista de cobranzas a cancelar
                            foreach (var cp in cps)
                            {
                                int Moneda = dbContext.letrasdetalle.Where(p => p.v_IdLetrasDetalle == cp.v_IdLetrasDetalle && p.i_Eliminado == 0).FirstOrDefault().i_IdMoneda.Value;

                                switch (CobranzaEntity.i_IdMoneda)
                                {
                                    case 1:
                                        switch (Moneda)
                                        {
                                            case 1:
                                                cp.d_Acuenta = cp.d_Acuenta.Value - cobranzadetalleDto.d_ImporteSoles.Value;
                                                cp.d_Saldo = cp.d_Saldo.Value + cobranzadetalleDto.d_ImporteSoles.Value >= 0 ? cp.d_Saldo.Value + cobranzadetalleDto.d_ImporteSoles.Value : 0;

                                                break;

                                            case 2:
                                                cp.d_Acuenta = cp.d_Acuenta.Value - (cobranzadetalleDto.d_ImporteSoles.Value / TCambio);
                                                cp.d_Saldo = cp.d_Saldo.Value + (cobranzadetalleDto.d_ImporteSoles.Value / TCambio) >= 0 ? cp.d_Saldo.Value + (cobranzadetalleDto.d_ImporteSoles.Value / TCambio) : 0;

                                                break;
                                        }
                                        break;

                                    case 2:
                                        switch (Moneda)
                                        {
                                            case 1:
                                                cp.d_Acuenta = cp.d_Acuenta.Value - (cobranzadetalleDto.d_ImporteSoles.Value * TCambio);
                                                cp.d_Saldo = cp.d_Saldo.Value + (cobranzadetalleDto.d_ImporteSoles.Value * TCambio) >= 0 ? cp.d_Saldo.Value + (cobranzadetalleDto.d_ImporteSoles.Value * TCambio) : 0;

                                                break;

                                            case 2:
                                                cp.d_Acuenta = cp.d_Acuenta.Value - cobranzadetalleDto.d_ImporteSoles.Value;
                                                cp.d_Saldo = cp.d_Saldo.Value + cobranzadetalleDto.d_ImporteSoles.Value >= 0 ? cp.d_Saldo.Value + cobranzadetalleDto.d_ImporteSoles.Value : 0;

                                                break;
                                        }
                                        break;
                                }

                                if (cp.d_Saldo == 0) cp.letrasdetalle.i_Pagada = 1;
                                else cp.letrasdetalle.i_Pagada = 0;

                                dbContext.cobranzaletraspendiente.ApplyCurrentValues(cp);
                                Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "cobranzaletraspendiente", cp.v_IdLetrasDetalle);
                            }
                            #endregion
                        }
                        else
                        {
                            pobjOperationResult.Success = 0;
                            pobjOperationResult.ErrorMessage = "No se encontró la cobranza pendiente";
                            pobjOperationResult.AdditionalInformation = "CobranzaBL.RestauraDetalleCobranzaLetras()";
                            Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                        }
                    }
                    else
                    {
                        pobjOperationResult.Success = 0;
                        pobjOperationResult.ErrorMessage = "No se encontró la cobranza";
                        pobjOperationResult.AdditionalInformation = "CobranzaBL.RestauraDetalleCobranzaLetras()";
                        Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                    }

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "CobranzaBL.RestauraDetalleCobranzaLetras()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = pobjOperationResult.ExceptionMessage != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }

        }
        */


        #region Reportes


       

        public List<ReporteCuentasPagarDto> ReporteCuentasPagar(ref OperationResult objOperationResult, DateTime FechaDesde, DateTime FechaHasta, string GrupoLlave, string Orden, string CodProveedor, bool RangoFechas, bool IncluirLetrasCambio,int Establecimiento)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    objOperationResult.Success = 1;
                    List<ReporteCuentasPagarDto> queryLetrasDetalle = new List<ReporteCuentasPagarDto>();
                    if (RangoFechas)
                    {
                        var queryCompras = (from n in dbContext.pagopendiente

                                            join A in dbContext.compra on new { IdCompra = n.v_IdCompra, eliminado = 0 } equals new { IdCompra = A.v_IdCompra, eliminado = A.i_Eliminado.Value } into A_join
                                            from A in A_join.DefaultIfEmpty()

                                            join B in dbContext.cliente on new { IdProveedor = A.v_IdProveedor, eliminado = 0, Flag = "V" } equals new { IdProveedor = B.v_IdCliente, eliminado = B.i_Eliminado.Value, Flag = B.v_FlagPantalla } into B_join
                                            from B in B_join.DefaultIfEmpty()

                                            join C in dbContext.documento on new { i_IdTipoDocumento = A.i_IdTipoDocumento.Value, eliminado = 0 }
                                                                           equals new { i_IdTipoDocumento = C.i_CodigoDocumento, eliminado = C.i_Eliminado.Value } into C_join
                                            from C in C_join.DefaultIfEmpty()

                                            join D in dbContext.datahierarchy on new { i_idTipoPago = A.i_IdCondicionPago.Value, b = 23, eliminado = 0 }
                                                                         equals new { i_idTipoPago = D.i_ItemId, b = D.i_GroupId, eliminado = D.i_IsDeleted.Value } into D_join
                                            from D in D_join.DefaultIfEmpty()

                                            where n.i_Eliminado == 0 && A.t_FechaRegistro >= FechaDesde && A.t_FechaRegistro <= FechaHasta &&
                                                  n.d_Saldo > 0 && (B.v_CodCliente == CodProveedor || CodProveedor == "")
                                                  && (A.i_IdEstablecimiento ==Establecimiento || Establecimiento ==-1)

                                            select new ReporteCuentasPagarDto
                                            {

                                                Correlativo = "C " + A.v_Mes.Trim() + "-" + A.v_Correlativo,
                                                NombreCliente = B == null ? "**NO EXISTE PROVEEDOR**" : (B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " + B.v_SegundoNombre + " " + B.v_RazonSocial).Trim(),
                                                MedioPago = D == null ? "" : D.v_Value1,
                                                FechaEmision = A.t_FechaRegistro.Value,
                                                TipoDocumento = C == null ? "" : C.v_Siglas,
                                                NroDocumento = C == null ? A.v_SerieDocumento.Trim() + "-" + A.v_CorrelativoDocumento.Trim() : C.v_Siglas + " " + A.v_SerieDocumento.Trim() + "-" + A.v_CorrelativoDocumento.Trim(),
                                                FechaVencimiento = A.t_FechaVencimiento.Value,
                                                Moneda = A.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$.",
                                                TotalFacturado = A.d_Total,
                                                Acuenta = n.d_Acuenta.Value,
                                                Saldo = A.i_IdMoneda == (int)Currency.Soles ? n.d_Saldo : 0,
                                                SaldoDolares = A.i_IdMoneda.Value == (int)Currency.Dolares ? n.d_Saldo.Value : 0,
                                                MonedaCobranza = A.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$.",
                                                Grupollave =
                                                GrupoLlave == "PROVEEDOR" ? B.v_IdCliente == null ? "** PROVEEDOR NO EXISTE **" : "PROVEEDOR : " + (B.v_PrimerNombre + " " + B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_RazonSocial).Trim() :
                                                GrupoLlave == "COND. PAGO" ? D.v_Value1 == null ? "**COND. PAGO NO EXISTE **" : "COND. PAGO :" + D.v_Value1 : "",
                                                // NombreGrupo = GrupoLlave,
                                                v_CodigoProveedor = B == null ? "**NO EXISTE PROVEEDOR**" : B.v_CodCliente.Trim(),
                                                idTipoDocumento = A == null ? 0 : A.i_IdTipoDocumento.Value,
                                                v_SerieDocumento = A.v_SerieDocumento.Trim(),
                                                v_CorrelativoDocumento = A.v_CorrelativoDocumento.Trim(),
                                                Glosa = A.v_Glosa,
                                                Soles = A.i_IdMoneda == (int)Currency.Soles ? A.d_Total : 0,
                                                Dolares = A.i_IdMoneda == (int)Currency.Dolares ? A.d_Total : 0,


                                            }
                                      ).ToList().AsQueryable();

                        if (IncluirLetrasCambio)
                        {

                            queryLetrasDetalle = (from a in dbContext.cobranzaletraspagarpendiente

                                                  join b in dbContext.letraspagardetalle on new { IdLetrasDetalle = a.v_IdLetrasPagarDetalle, eliminado = 0 } equals new { IdLetrasDetalle = b.v_IdLetrasPagarDetalle, eliminado = b.i_Eliminado.Value } into b_join
                                                  from b in b_join.DefaultIfEmpty()

                                                  join c in dbContext.cliente on new { IdCliente = b.v_IdProveedor, eliminado = 0 } equals new { IdCliente = c.v_IdCliente, eliminado = c.i_Eliminado.Value } into c_join
                                                  from c in c_join.DefaultIfEmpty()

                                                  join d in dbContext.documento on new { DocumentoLetraDet = b.i_IdTipoDocumento.Value, eliminado = 0 } equals new { DocumentoLetraDet = d.i_CodigoDocumento, eliminado = d.i_Eliminado.Value } into d_join

                                                  from d in d_join.DefaultIfEmpty()

                                                  where a.i_Eliminado == 0 && b.t_FechaEmision >= FechaDesde && b.t_FechaEmision <= FechaHasta &&
                                                   a.d_Saldo > 0

                                                  select new ReporteCuentasPagarDto
                                                  {
                                                      Correlativo = b.i_EsSaldoInicial == 1 ? "SALDO INICIAL" : "L " + b.v_Serie.Trim() + "-" + b.v_Correlativo.Trim(),
                                                      NombreCliente = c == null ? "**NO EXISTE CLIENTE**" : (c.v_ApePaterno + " " + c.v_ApeMaterno + " " + c.v_PrimerNombre + " " + c.v_SegundoNombre + " " + c.v_RazonSocial).Trim(),
                                                      MedioPago = "LETRA",
                                                      FechaEmision = b.t_FechaEmision.Value,
                                                      TipoDocumento = d == null ? "" : d.v_Siglas.Trim(),
                                                      NroDocumento = d == null ? b.v_Serie.Trim() + "-" + b.v_Correlativo.Trim() : d.v_Siglas + " " + b.v_Serie.Trim() + "-" + b.v_Correlativo.Trim(),
                                                      FechaVencimiento = b.t_FechaVencimiento.Value,
                                                      Vendedor = "",
                                                      Moneda = b.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$.",
                                                      TotalFacturado = a.d_Saldo.Value + a.d_Acuenta.Value,
                                                      Acuenta = a.d_Acuenta.Value,
                                                      Saldo = b.i_IdMoneda == (int)Currency.Soles ? a.d_Saldo : 0,
                                                      SaldoDolares = b.i_IdMoneda.Value == (int)Currency.Dolares ? a.d_Saldo.Value : 0,
                                                      MonedaCobranza = b.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$.",
                                                      Grupollave = GrupoLlave == "PROVEEDOR" ? c.v_IdCliente == null ? "** PROVEEDOR NO EXISTE **" : "PROVEEDOR : " + (c.v_PrimerNombre + " " + c.v_ApePaterno + " " + c.v_ApeMaterno + " " + c.v_RazonSocial).Trim() :
                                                      GrupoLlave == "COND. PAGO" ? "COND. DE PAGO :LETRA" : "",
                                                      //  NombreGrupo = GrupoLlave,
                                                      v_CodigoProveedor = c == null ? "**NO EXISTE CLIENTE**" : c.v_CodCliente.Trim(),
                                                      idTipoDocumento = d == null ? 0 : d.i_CodigoDocumento,
                                                      v_IdVendedor = "",
                                                      v_SerieDocumento = b.v_Serie,
                                                      v_CorrelativoDocumento = b.v_Correlativo,
                                                      Glosa = "LETRA DE CAMBIO",
                                                      Soles = b.i_IdMoneda == (int)Currency.Soles ? a.d_Saldo.Value + a.d_Acuenta.Value : 0,
                                                      Dolares = b.i_IdMoneda == (int)Currency.Dolares ? a.d_Saldo.Value + a.d_Acuenta.Value : 0,
                                                  }).ToList();
                        }

                        var queryFinal = queryCompras.Concat(queryLetrasDetalle);
                        if (!string.IsNullOrEmpty(Orden))
                        {
                            queryFinal = queryFinal.OrderBy(Orden);
                        }
                        List<ReporteCuentasPagarDto> objData = queryFinal.ToList();
                        return objData;
                    }
                    else
                    {

                        #region Sin RangoFechas

                        var queryCompras = (from n in dbContext.pagopendiente

                                            join A in dbContext.compra on new { IdCompra = n.v_IdCompra, eliminado = 0 } equals new { IdCompra = A.v_IdCompra, eliminado = A.i_Eliminado.Value } into A_join
                                            from A in A_join.DefaultIfEmpty()

                                            join B in dbContext.cliente on new { IdProveedor = A.v_IdProveedor, eliminado = 0, Flag = "V" } equals new { IdProveedor = B.v_IdCliente, eliminado = B.i_Eliminado.Value, Flag = B.v_FlagPantalla } into B_join
                                            from B in B_join.DefaultIfEmpty()

                                            join C in dbContext.documento on new { i_IdTipoDocumento = A.i_IdTipoDocumento.Value, eliminado = 0 }
                                                                           equals new { i_IdTipoDocumento = C.i_CodigoDocumento, eliminado = C.i_Eliminado.Value } into C_join
                                            from C in C_join.DefaultIfEmpty()

                                            join D in dbContext.datahierarchy on new { i_idTipoPago = A.i_IdCondicionPago.Value, b = 23, eliminado = 0 }
                                                                         equals new { i_idTipoPago = D.i_ItemId, b = D.i_GroupId, eliminado = D.i_IsDeleted.Value } into D_join
                                            from D in D_join.DefaultIfEmpty()

                                            where n.i_Eliminado == 0 &&
                                                  n.d_Saldo > 0 && (B.v_CodCliente == CodProveedor || CodProveedor == "")

                                                  && (A.i_IdEstablecimiento ==Establecimiento || Establecimiento ==-1)

                                            select new ReporteCuentasPagarDto
                                            {

                                                Correlativo = "C " + A.v_Mes.Trim() + "-" + A.v_Correlativo,
                                                NombreCliente = B == null ? "**NO EXISTE PROVEEDOR**" : (B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " + B.v_SegundoNombre + " " + B.v_RazonSocial).Trim(),
                                                MedioPago = D == null ? "" : D.v_Value1,
                                                FechaEmision = A.t_FechaRegistro.Value,
                                                TipoDocumento = C == null ? "" : C.v_Siglas,
                                                NroDocumento = C == null ? A.v_SerieDocumento.Trim() + "-" + A.v_CorrelativoDocumento.Trim() : C.v_Siglas + " " + A.v_SerieDocumento.Trim() + "-" + A.v_CorrelativoDocumento.Trim(),
                                                FechaVencimiento = A.t_FechaVencimiento.Value,
                                                Moneda = A.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$.",
                                                TotalFacturado = A.d_Total,
                                                Acuenta = n.d_Acuenta.Value,
                                                Saldo = A.i_IdMoneda == (int)Currency.Soles ? n.d_Saldo : 0,
                                                SaldoDolares = A.i_IdMoneda.Value == (int)Currency.Dolares ? n.d_Saldo.Value : 0,
                                                MonedaCobranza = A.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$.",
                                                Grupollave =
                                                GrupoLlave == "PROVEEDOR" ? B.v_IdCliente == null ? "** PROVEEDOR NO EXISTE **" : "PROVEEDOR : " + (B.v_PrimerNombre + " " + B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_RazonSocial).Trim() :
                                                GrupoLlave == "COND. PAGO" ? D.v_Value1 == null ? "**COND. PAGO NO EXISTE **" : "COND. PAGO :" + D.v_Value1 : "",
                                                // NombreGrupo = GrupoLlave,
                                                v_CodigoProveedor = B == null ? "**NO EXISTE PROVEEDOR**" : B.v_CodCliente.Trim(),
                                                idTipoDocumento = A == null ? 0 : A.i_IdTipoDocumento.Value,
                                                v_SerieDocumento = A.v_SerieDocumento.Trim(),
                                                v_CorrelativoDocumento = A.v_CorrelativoDocumento.Trim(),
                                                Glosa = A.v_Glosa,
                                                Soles = A.i_IdMoneda == (int)Currency.Soles ? A.d_Total : 0,
                                                Dolares = A.i_IdMoneda == (int)Currency.Dolares ? A.d_Total : 0,

                                            }
                                      ).ToList().AsQueryable();

                        if (IncluirLetrasCambio)
                        {

                            queryLetrasDetalle = (from a in dbContext.cobranzaletraspagarpendiente

                                                  join b in dbContext.letraspagardetalle on new { IdLetrasDetalle = a.v_IdLetrasPagarDetalle, eliminado = 0 } equals new { IdLetrasDetalle = b.v_IdLetrasPagarDetalle, eliminado = b.i_Eliminado.Value } into b_join
                                                  from b in b_join.DefaultIfEmpty()

                                                  join c in dbContext.cliente on new { IdCliente = b.v_IdProveedor, eliminado = 0 } equals new { IdCliente = c.v_IdCliente, eliminado = c.i_Eliminado.Value } into c_join
                                                  from c in c_join.DefaultIfEmpty()

                                                  join d in dbContext.documento on new { DocumentoLetraDet = b.i_IdTipoDocumento.Value, eliminado = 0 } equals new { DocumentoLetraDet = d.i_CodigoDocumento, eliminado = d.i_Eliminado.Value } into d_join

                                                  from d in d_join.DefaultIfEmpty()

                                                  where a.i_Eliminado == 0 &&
                                                   a.d_Saldo > 0

                                                  select new ReporteCuentasPagarDto
                                                  {
                                                      Correlativo = "L " + b.v_Serie.Trim() + "-" + b.v_Correlativo.Trim(),
                                                      NombreCliente = c == null ? "**NO EXISTE CLIENTE**" : (c.v_ApePaterno + " " + c.v_ApeMaterno + " " + c.v_PrimerNombre + " " + c.v_SegundoNombre + " " + c.v_RazonSocial).Trim(),
                                                      MedioPago = "LETRA",
                                                      FechaEmision = b.t_FechaEmision.Value,
                                                      TipoDocumento = d == null ? "" : d.v_Siglas.Trim(),
                                                      NroDocumento = d == null ? b.v_Serie.Trim() + "-" + b.v_Correlativo.Trim() : d.v_Siglas + " " + b.v_Serie.Trim() + "-" + b.v_Correlativo.Trim(),
                                                      FechaVencimiento = b.t_FechaVencimiento.Value,
                                                      Vendedor = "",
                                                      Moneda = b.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$.",
                                                      TotalFacturado = a.d_Saldo.Value + a.d_Acuenta.Value,
                                                      Acuenta = a.d_Acuenta.Value,
                                                      Saldo = b.i_IdMoneda == (int)Currency.Soles ? a.d_Saldo : 0,
                                                      SaldoDolares = b.i_IdMoneda.Value == (int)Currency.Dolares ? a.d_Saldo.Value : 0,
                                                      MonedaCobranza = b.i_IdMoneda == (int)Currency.Soles ? "S/." : "US$.",
                                                      Grupollave = GrupoLlave == "PROVEEDOR" ? c.v_IdCliente == null ? "** PROVEEDOR NO EXISTE **" : "PROVEEDOR : " + (c.v_PrimerNombre + " " + c.v_ApePaterno + " " + c.v_ApeMaterno + " " + c.v_RazonSocial).Trim() :
                                                      GrupoLlave == "COND. PAGO" ? "COND. DE PAGO :LETRA" : "",
                                                      //  NombreGrupo = GrupoLlave,
                                                      v_CodigoProveedor = c == null ? "**NO EXISTE CLIENTE**" : c.v_CodCliente.Trim(),
                                                      idTipoDocumento = d == null ? 0 : d.i_CodigoDocumento,
                                                      v_IdVendedor = "",
                                                      v_SerieDocumento = b.v_Serie,
                                                      v_CorrelativoDocumento = b.v_Correlativo,
                                                      Glosa = "LETRA DE CAMBIO",
                                                      Soles = b.i_IdMoneda == (int)Currency.Soles ? a.d_Saldo.Value + a.d_Acuenta.Value : 0,
                                                      Dolares = b.i_IdMoneda == (int)Currency.Dolares ? a.d_Saldo.Value + a.d_Acuenta.Value : 0,
                                                  }).ToList();
                        }




                        var queryFinal = queryCompras.Concat(queryLetrasDetalle);

                        if (!string.IsNullOrEmpty(Orden))
                        {
                            queryFinal = queryFinal.OrderBy(Orden);
                        }
                        List<ReporteCuentasPagarDto> objData = queryFinal.ToList();
                        return objData;
                        #endregion



                    }
                }
            }
            catch (Exception)
            {
                objOperationResult.Success = 0;
                return null;
            }

        }

        public List<ReportePlanillaPagos> ReportePlanillaPagos(DateTime FechaInicio, DateTime FechaHasta, int? IdTipoDocumentoCobranzaDetalle, string Orden, string CodProveedor, string pstrt_Serie, string pstr_grupollave, string Correlativo, int TipoDocumento)
        {

            OperationResult objOperationResult = new OperationResult();
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();


                var queryComprasPagadas = (from n in dbContext.pagopendiente

                                           join A in dbContext.pagodetalle on new { a = n.v_IdCompra, eliminado = 0 } equals new { a = A.v_IdCompra, eliminado = A.i_Eliminado.Value } into A_join
                                           from A in A_join.DefaultIfEmpty()

                                           join a in dbContext.compra on new { IdCompra = n.v_IdCompra, eliminado = 0 } equals new { IdCompra = a.v_IdCompra, eliminado = a.i_Eliminado.Value } into a_join
                                           from a in a_join.DefaultIfEmpty()

                                           join C in dbContext.pago on new { b = A.v_IdPago, eliminado = 0 } equals new { b = C.v_IdPago, eliminado = C.i_Eliminado.Value } into C_join
                                           from C in C_join.DefaultIfEmpty()

                                           join B in dbContext.cliente on new { IdCliente = a.v_IdProveedor, eliminado = 0 } equals new { IdCliente = B.v_IdCliente, eliminado = B.i_Eliminado.Value } into B_join
                                           from B in B_join.DefaultIfEmpty()

                                           join J4 in dbContext.documento on new { i_IdTipoDocumento = a.i_IdTipoDocumento.Value, eliminado = 0 }
                                                                 equals new { i_IdTipoDocumento = J4.i_CodigoDocumento, eliminado = J4.i_Eliminado.Value } into J4_join
                                           from J4 in J4_join.DefaultIfEmpty()

                                           //join J5 in dbContext.documento on new { DocumentoPago= C.i_IdTipoDocumento.Value, eliminado = 0 }
                                           //                 equals new { DocumentoPago = J5.i_CodigoDocumento, eliminado = J5.i_Eliminado.Value } into J5_join
                                           //from J5 in J5_join.DefaultIfEmpty()

                                           join J6 in dbContext.documento on new { DocumentoPagoDetalle = A.i_IdTipoDocumentoRef.Value, eliminado = 0 }
                                                           equals new { DocumentoPagoDetalle = J6.i_CodigoDocumento, eliminado = J6.i_Eliminado.Value } into J6_join
                                           from J6 in J6_join.DefaultIfEmpty()

                                           join J7 in dbContext.datahierarchy on new { i_idTipoPago = a.i_IdCondicionPago.Value, b = 23, eliminado = 0 }
                                                                        equals new { i_idTipoPago = J7.i_ItemId, b = J7.i_GroupId, eliminado = J7.i_IsDeleted.Value } into J7_join
                                           from J7 in J7_join.DefaultIfEmpty()

                                           join J8 in dbContext.documento on new { DocumentoPago = C.i_IdTipoDocumento.Value, eliminado = 0 }
                                                                          equals new { DocumentoPago = J8.i_CodigoDocumento, eliminado = J8.i_Eliminado.Value } into J8_join
                                           from J8 in J8_join.DefaultIfEmpty()

                                           join J9 in dbContext.datahierarchy on new { Grupo = 46, eliminado = 0, FormaPago = A.i_IdFormaPago.Value } equals new { Grupo = J9.i_GroupId, eliminado = J9.i_IsDeleted.Value, FormaPago = J9.i_ItemId } into J9_join

                                           from J9 in J9_join.DefaultIfEmpty()
                                           where (C.t_FechaRegistro >= FechaInicio && C.t_FechaRegistro <= FechaHasta) && (C.i_IdTipoDocumento == IdTipoDocumentoCobranzaDetalle || IdTipoDocumentoCobranzaDetalle == -1)
                                           && (B.v_CodCliente == CodProveedor || CodProveedor == "") && n.i_Eliminado == 0
                                             && (a.v_SerieDocumento == pstrt_Serie || pstrt_Serie == "")
                                             && (a.v_CorrelativoDocumento == Correlativo || Correlativo == "") && (a.i_IdTipoDocumento == TipoDocumento || TipoDocumento == -1)
                                           select new ReportePlanillaPagos
                                           {
                                               TipoDocumento = J4.v_Siglas,
                                               NroDocumento = a.v_SerieDocumento + "-" + a.v_CorrelativoDocumento,
                                               FechaEmision = C.t_FechaRegistro,
                                               NombreProveedor = B == null ? "" : (B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " + B.v_SegundoNombre + " " + B.v_RazonSocial).Trim().ToUpper(),
                                               Cheq = J6.v_Siglas,
                                               Moneda = a.i_IdMoneda == (int)Currency.Soles ? "S/." : "US$.",
                                               TotalFacturado = a.i_IdMoneda == (int)Currency.Soles ? a.d_Total : 0,
                                               TotalFacturadoDolares = a.i_IdMoneda == (int)Currency.Dolares ? a.d_Total : 0,
                                               MontoPagado = C.i_IdMoneda == (int)Currency.Soles ? A.d_ImporteSoles : 0,
                                               Saldo = a.i_IdMoneda == (int)Currency.Soles ? A.d_NetoXCobrar - A.d_ImporteSoles : 0,
                                               idTipoDocumento = a.i_IdTipoDocumento,
                                               MedioPago = J7.v_Value1,
                                               MontoCobrar = a.i_IdMoneda == (int)Currency.Soles ? A.d_NetoXCobrar : 0,
                                               MontoCobrarDolares = C.i_IdMoneda == (int)Currency.Dolares ? A.d_NetoXCobrar : 0,
                                               MontoPagadoDolares = C.i_IdMoneda == (int)Currency.Dolares ? A.d_ImporteSoles : 0,
                                               MonedaCobranza = C.i_IdMoneda == (int)Currency.Soles ? "S/." : "US$.",
                                               SaldoDolares = C.i_IdMoneda == (int)Currency.Dolares ? A.d_NetoXCobrar - A.d_ImporteSoles : 0,
                                               MedioPagoCobranza = C.i_IdMedioPago,
                                               // v_IdCobranzaPendiente = n.v_IdCobranzaPendiente,
                                               IdTipoDocumentoCobranzaDetalle = A.i_IdTipoDocumentoRef,
                                               Grupollave = pstr_grupollave == "TIPO COMPROBANTE" ? J8 == null ? "** TIPO COMPROBANTE NO EXISTE **" : "TIPO COMPROBANTE : " + J8.v_Nombre :
                                               pstr_grupollave == "COND. PAGO" ? J7 == null ? "** COND. PAGO NO EXISTE **" : "COND. PAGO :" + J7.v_Value1 : pstr_grupollave == "PROVEEDOR" ? B == null ? "** PROVEEDOR NO EXISTE**" : "PROVEEDOR : " + (B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " + B.v_SegundoNombre + " " + B.v_RazonSocial).Trim().ToUpper() : "",
                                               // NombreGrupo = pstr_Nombregrupollave,
                                               TipoDocumentoCobranza = J8.v_Siglas,
                                               NroDocumentoCobranza = C.v_Mes.Trim() + "-" + C.v_Correlativo.Trim(),
                                               IdTipoDocumentoReferenciaCobranzaDetalle = A.i_IdTipoDocumentoRef.Value,
                                               NumeroDocumentoReferenciaCobranzaDetalle = J6 == null ? "" : (J6.v_Siglas.Trim() + " " + A.v_DocumentoRef.Replace(" ", "")),
                                               FormaPago = J9 == null ? "" : J9.v_Value1,

                                           }).ToList().AsQueryable();


                var queryLetrasPagadas = (from n in dbContext.cobranzaletraspagarpendiente

                                          join a in dbContext.letraspagardetalle on new { IdLetrasDetalle = n.v_IdLetrasPagarDetalle, eliminado = 0 } equals new { IdLetrasDetalle = a.v_IdLetrasPagarDetalle, eliminado = a.i_Eliminado.Value } into a_join
                                          from a in a_join.DefaultIfEmpty()

                                          join A in dbContext.pagodetalle on new { a = a.v_IdLetrasPagarDetalle, eliminado = 0 } equals new { a = A.v_IdCompra, eliminado = A.i_Eliminado.Value } into A_join
                                          from A in A_join.DefaultIfEmpty()

                                          join C in dbContext.pago on new { b = A.v_IdPago, eliminado = 0 } equals new { b = C.v_IdPago, eliminado = C.i_Eliminado.Value } into C_join
                                          from C in C_join.DefaultIfEmpty()

                                          join B in dbContext.cliente on new { IdProveedor = a.v_IdProveedor, eliminado = 0 } equals new { IdProveedor = B.v_IdCliente, eliminado = B.i_Eliminado.Value } into B_join
                                          from B in B_join.DefaultIfEmpty()

                                          join J4 in dbContext.documento on new { DocumentoLetraDetalle = a.i_IdTipoDocumento.Value, eliminado = 0 }
                                                                equals new { DocumentoLetraDetalle = J4.i_CodigoDocumento, eliminado = J4.i_Eliminado.Value } into J4_join
                                          from J4 in J4_join.DefaultIfEmpty()

                                          join J5 in dbContext.documento on new { DocumentoCobranza = C.i_IdTipoDocumento.Value, eliminado = 0 }
                                                           equals new { DocumentoCobranza = J5.i_CodigoDocumento, eliminado = J5.i_Eliminado.Value } into J5_join
                                          from J5 in J5_join.DefaultIfEmpty()

                                          join J6 in dbContext.documento on new { DocumentoCobranzaDetalle = A.i_IdTipoDocumentoRef.Value, eliminado = 0 }
                                                          equals new { DocumentoCobranzaDetalle = J6.i_CodigoDocumento, eliminado = J6.i_Eliminado.Value } into J6_join
                                          from J6 in J6_join.DefaultIfEmpty()


                                          join J8 in dbContext.documento on new { i_IdTipoDocumento = C.i_IdTipoDocumento.Value, eliminado = 0 }
                                                                         equals new { i_IdTipoDocumento = J8.i_CodigoDocumento, eliminado = J8.i_Eliminado.Value } into J8_join
                                          from J8 in J8_join.DefaultIfEmpty()

                                          join J9 in dbContext.datahierarchy on new { Grupo = 46, eliminado = 0, FormaPago = A.i_IdFormaPago.Value } equals new { Grupo = J9.i_GroupId, eliminado = J9.i_IsDeleted.Value, FormaPago = J9.i_ItemId } into J9_join

                                          from J9 in J9_join.DefaultIfEmpty()
                                          where (C.t_FechaRegistro >= FechaInicio && C.t_FechaRegistro <= FechaHasta) && (C.i_IdTipoDocumento == IdTipoDocumentoCobranzaDetalle || IdTipoDocumentoCobranzaDetalle == -1)
                                          && (B.v_CodCliente == CodProveedor || CodProveedor == "") && n.i_Eliminado == 0
                                            && (a.v_Serie == pstrt_Serie || pstrt_Serie == "")
                                            && (a.v_Correlativo == Correlativo || Correlativo == "") && (a.i_IdTipoDocumento == TipoDocumento || TipoDocumento == -1)
                                          select new ReportePlanillaPagos
                                          {
                                              TipoDocumento = J4.v_Siglas,
                                              NroDocumento = a.v_Serie + "-" + a.v_Correlativo,
                                              FechaEmision = C.t_FechaRegistro,
                                              NombreProveedor = B == null ? "" : (B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " + B.v_SegundoNombre + " " + B.v_RazonSocial).Trim(),
                                              Cheq = J6.v_Siglas,
                                              Moneda = a.i_IdMoneda == (int)Currency.Soles ? "S/." : "US$.",
                                              // TotalFacturado = a.d_Total,
                                              TotalFacturado = a.i_IdMoneda == (int)Currency.Soles ? n.d_Acuenta.Value + n.d_Saldo.Value : 0,
                                              TotalFacturadoDolares = a.i_IdMoneda == (int)Currency.Dolares ? n.d_Acuenta.Value + n.d_Saldo.Value : 0,
                                              MontoPagado = C.i_IdMoneda == (int)Currency.Soles ? A.d_ImporteSoles : 0,
                                              Saldo = a.i_IdMoneda == (int)Currency.Soles ? A.d_NetoXCobrar - A.d_ImporteSoles : 0,
                                              idTipoDocumento = a.i_IdTipoDocumento,
                                              //MedioPago = J7.v_Value1,
                                              MedioPago = "LETRA",
                                              MontoCobrar = a.i_IdMoneda == (int)Currency.Soles ? A.d_NetoXCobrar : 0,
                                              MontoCobrarDolares = C.i_IdMoneda == (int)Currency.Dolares ? A.d_NetoXCobrar : 0,
                                              MontoPagadoDolares = C.i_IdMoneda == (int)Currency.Dolares ? A.d_ImporteSoles : 0,
                                              MonedaCobranza = C.i_IdMoneda == (int)Currency.Soles ? "S/." : "US$.",
                                              SaldoDolares = C.i_IdMoneda == (int)Currency.Dolares ? A.d_NetoXCobrar - A.d_ImporteSoles : 0,
                                              MedioPagoCobranza = C.i_IdMedioPago,
                                              IdTipoDocumentoCobranzaDetalle = A.i_IdTipoDocumentoRef,
                                              Grupollave = pstr_grupollave == "TIPO COMPROBANTE" ? J8 == null ? "** TIPO COMPROBANTE NO EXISTE **" : "TIPO COMPROBANTE : " + J8.v_Nombre :
                                              pstr_grupollave == "COND. PAGO" ? "COND. PAGO : LETRA " : pstr_grupollave == "PROVEEDOR" ? B == null ? "** PROVEEDOR NO EXISTE**" : "PROVEEDOR : " + (B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " + B.v_SegundoNombre + " " + B.v_RazonSocial).Trim().ToUpper() : "",
                                              TipoDocumentoCobranza = J8.v_Siglas,
                                              NroDocumentoCobranza = C.v_Mes.Trim() + "-" + C.v_Correlativo.Trim(),
                                              IdTipoDocumentoReferenciaCobranzaDetalle = A.i_IdTipoDocumentoRef.Value,
                                              NumeroDocumentoReferenciaCobranzaDetalle = J6 == null ? "" : (J6.v_Siglas.Trim() + " " + A.v_DocumentoRef.Replace(" ", "")),
                                              FormaPago = J9 == null ? "" : J9.v_Value1,

                                          }).ToList().AsQueryable();


                var queryFinal = queryComprasPagadas.Concat(queryLetrasPagadas);

                if (!string.IsNullOrEmpty(Orden))
                {
                    queryFinal = queryFinal.OrderBy(Orden);
                }
                List<ReportePlanillaPagos> objData = queryFinal.ToList();
                return objData;

            }
            catch (Exception)
            {
                return null;
            }
        }




        public List<ReporteEstadoCuentaProveedor> ReporteEstadoCuentaProveedor(string pstrCodigoProveedor, DateTime FechaSaldoDeudor, DateTime FechaDesde, DateTime FechaHasta)
        {
            List<ReporteEstadoCuentaProveedor> ListaFinal = new List<ReporteEstadoCuentaProveedor>();
            ReporteEstadoCuentaProveedor objReporte = new ReporteEstadoCuentaProveedor();

            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var queryDebe = (from a in dbContext.compra

                                     join B in dbContext.cliente on new { Proveedor = a.v_IdProveedor, eliminado = 0 } equals new { Proveedor = B.v_IdCliente, eliminado = B.i_Eliminado.Value } into B_join
                                     from B in B_join.DefaultIfEmpty()

                                     join J4 in dbContext.documento on new { i_IdTipoDocumento = a.i_IdTipoDocumento.Value, eliminado = 0 }
                                                           equals new { i_IdTipoDocumento = J4.i_CodigoDocumento, eliminado = J4.i_Eliminado.Value } into J4_join
                                     from J4 in J4_join.DefaultIfEmpty()

                                     join J7 in dbContext.datahierarchy on new { i_idTipoPago = a.i_IdCondicionPago.Value, b = 23, eliminado = 0 }
                                                                  equals new { i_idTipoPago = J7.i_ItemId, b = J7.i_GroupId, eliminado = J7.i_IsDeleted.Value } into J7_join
                                     from J7 in J7_join.DefaultIfEmpty()

                                     join J8 in dbContext.documento on new { i_IdTipoDocRef = a.i_IdTipoDocumentoRef.Value, eliminado = 0 }
                                                                    equals new { i_IdTipoDocRef = J8.i_CodigoDocumento, eliminado = J8.i_Eliminado.Value } into J8_join

                                     from J8 in J8_join.DefaultIfEmpty()


                                     where B.v_CodCliente == pstrCodigoProveedor && a.i_Eliminado == 0 && a.i_IdEstado == 1
                                     select new ReporteEstadoCuentaProveedor
                                     {

                                         Fecha = a.t_FechaRegistro.Value,
                                         Proveedor = B == null ? "**NO EXISTE PROVEEDOR**" : ("CÓDIGO : " + B.v_CodCliente.Trim() + "   -    DENOMINACIÓN: " + B.v_ApePaterno.Trim() + " " + B.v_ApeMaterno.Trim() + " " + B.v_PrimerNombre.Trim() + " " + B.v_SegundoNombre.Trim() + " " + B.v_RazonSocial.Trim()).Trim().ToUpper(),
                                         pIntTipoDocumento = a.i_IdTipoDocumento.Value,
                                         Concepto = J4.v_Siglas.Trim() + ".  " + a.v_SerieDocumento.Trim() + "-" + a.v_CorrelativoDocumento.Trim(),
                                         Referencia = a.v_Glosa, // a.v_Concepto.Trim(),

                                         //DebeSoles = a.i_IdTipoDocumento.Value != (int)TiposDocumentos.NotaCredito ? a.i_IdMoneda.Value == (int)Currency.Soles ? a.d_Total.Value : 0 : 0,
                                         //DebeDolares = a.i_IdTipoDocumento.Value != (int)TiposDocumentos.NotaCredito ? a.i_IdMoneda.Value == (int)Currency.Dolares ? a.d_Total.Value : 0 : 0,
                                         //HaberSoles = a.i_IdTipoDocumento == (int)TiposDocumentos.NotaCredito ? a.i_IdMoneda.Value == (int)Currency.Soles ? a.d_Total.Value : 0 : 0,
                                         //HaberDolares = a.i_IdTipoDocumento == (int)TiposDocumentos.NotaCredito ? a.i_IdMoneda.Value == (int)Currency.Dolares ? a.d_Total.Value : 0 : 0,

                                         DebeSoles = (J4.i_UsadoDocumentoInverso == 0 || J4.i_UsadoDocumentoInverso == null) ? a.i_IdMoneda.Value == (int)Currency.Soles ? a.d_Total.Value : 0 : 0,
                                         DebeDolares = (J4.i_UsadoDocumentoInverso == 0 || J4.i_UsadoDocumentoInverso == null) ? a.i_IdMoneda.Value == (int)Currency.Dolares ? a.d_Total.Value : 0 : 0,
                                         HaberSoles = J4.i_UsadoDocumentoInverso == 1 ? a.i_IdMoneda.Value == (int)Currency.Soles ? a.d_Total.Value : 0 : 0,
                                         HaberDolares = J4.i_UsadoDocumentoInverso == 1 ? a.i_IdMoneda.Value == (int)Currency.Dolares ? a.d_Total.Value : 0 : 0,



                                         FechaInsercion = a.t_ActualizaFecha.Value == null ? a.t_InsertaFecha.Value : a.t_ActualizaFecha.Value,        //a.t_InsertaFecha.Value  ,
                                         DocumentoReferencia = J8 == null ? "" : J8.v_Siglas.Trim() + " " + a.v_SerieDocumentoRef.Trim() + "-" + a.v_CorrelativoDocumentoRef.Trim(),
                                         Naturaleza = "",
                                     }).ToList();


                    var queryHaber = (from A in dbContext.pagodetalle

                                      join C in dbContext.pago on new { b = A.v_IdPago, eliminado = 0 } equals new { b = C.v_IdPago, eliminado = C.i_Eliminado.Value } into C_join
                                      from C in C_join.DefaultIfEmpty()

                                      join a in dbContext.compra on new { IdCompra = A.v_IdCompra, Estado = 1, eliminado = 0 } equals new { IdCompra = a.v_IdCompra, Estado = a.i_IdEstado.Value, eliminado = a.i_Eliminado.Value } into a_join
                                      from a in a_join.DefaultIfEmpty()

                                      join J9 in dbContext.letraspagardetalle on new { IdVenta = A.v_IdCompra, eliminado = 0 } equals new { IdVenta = J9.v_IdLetrasPagarDetalle, eliminado = J9.i_Eliminado.Value } into J9_join

                                      from J9 in J9_join.DefaultIfEmpty()

                                      join B in dbContext.cliente on new { ProveedorCompra = a.v_IdProveedor, eliminado = 0 } equals new { ProveedorCompra = B.v_IdCliente, eliminado = B.i_Eliminado.Value } into B_join
                                      from B in B_join.DefaultIfEmpty()

                                      join J11 in dbContext.cliente on new { ProveedorLetra = J9.v_IdProveedor, eliminado = 0 } equals new { ProveedorLetra = J11.v_IdCliente, eliminado = J11.i_Eliminado.Value } into J11_join

                                      from J11 in J11_join.DefaultIfEmpty()

                                      join J4 in dbContext.documento on new { DocumentoCompra = a.i_IdTipoDocumento.Value, eliminado = 0 }
                                                            equals new { DocumentoCompra = J4.i_CodigoDocumento, eliminado = J4.i_Eliminado.Value } into J4_join
                                      from J4 in J4_join.DefaultIfEmpty()

                                      join J5 in dbContext.documento on new { DocumentoPago = C.i_IdTipoDocumento.Value, eliminado = 0 }
                                                       equals new { DocumentoPago = J5.i_CodigoDocumento, eliminado = J5.i_Eliminado.Value } into J5_join
                                      from J5 in J5_join.DefaultIfEmpty()

                                      join J6 in dbContext.documento on new { DocumentoPagoDetalle = A.i_IdTipoDocumentoRef.Value, eliminado = 0 }
                                                      equals new { DocumentoPagoDetalle = J6.i_CodigoDocumento, eliminado = J6.i_Eliminado.Value } into J6_join
                                      from J6 in J6_join.DefaultIfEmpty()

                                      join J10 in dbContext.documento on new { DocumentoLetraDetalle = J9.i_IdTipoDocumento.Value, eliminado = 0 } equals new { DocumentoLetraDetalle = J10.i_CodigoDocumento, eliminado = J10.i_Eliminado.Value } into J10_join

                                      from J10 in J10_join.DefaultIfEmpty()

                                      where A.i_Eliminado == 0 && (B.v_CodCliente == pstrCodigoProveedor || J11.v_CodCliente == pstrCodigoProveedor)
                                      select new ReporteEstadoCuentaProveedor
                                      {

                                          Fecha = C == null ? DateTime.MinValue : C.t_FechaRegistro.Value,
                                          Proveedor = B == null ? J11 == null ? "**NO EXISTE PROVEEDOR**" : ("CÓDIGO : " + J11.v_CodCliente.Trim() + "   -    DENOMINACIÓN: " + J11.v_ApePaterno.Trim() + " " + J11.v_ApeMaterno.Trim() + " " + J11.v_PrimerNombre.Trim() + " " + J11.v_SegundoNombre.Trim() + " " + J11.v_RazonSocial.Trim()).Trim().ToUpper() : ("CÓDIGO : " + B.v_CodCliente.Trim() + "   -    DENOMINACIÓN: " + B.v_ApePaterno.Trim() + " " + B.v_ApeMaterno.Trim() + " " + B.v_PrimerNombre.Trim() + " " + B.v_SegundoNombre.Trim() + " " + B.v_RazonSocial.Trim()).Trim().ToUpper(),
                                          pIntTipoDocumento = J9 == null ? 0 : J9.i_IdTipoDocumento.Value,
                                          Concepto = a == null ? J9 == null ? "" : J10.v_Siglas.Trim() + ". " + J9.v_Serie.Trim() + "-" + J9.v_Correlativo : J4.v_Siglas.Trim() + ". " + a.v_SerieDocumento.Trim() + "-" + a.v_CorrelativoDocumento.Trim(), // J4.v_Siglas.Trim() + ".  " + a.v_SerieDocumento.Trim() + "-" + a.v_CorrelativoDocumento.Trim(),
                                          Referencia = C.v_Glosa.Trim(),
                                          DebeSoles = 0,
                                          DebeDolares = 0,
                                          HaberSoles = A == null ? 0 : C.i_IdMoneda.Value == (int)Currency.Soles ? A.d_ImporteSoles.Value : 0,
                                          HaberDolares = A == null ? 0 : C.i_IdMoneda.Value == (int)Currency.Dolares ? A.d_ImporteSoles.Value : 0,
                                          IdPagoDetalle = A.v_IdPagoDetalle,
                                          FechaInsercion = C.t_ActualizaFecha == null ? C.t_InsertaFecha.Value : C.t_ActualizaFecha.Value,
                                          DocumentoReferencia = J6 == null ? "" : J6.v_Siglas.Trim() + " " + A.v_DocumentoRef.Trim(),
                                          Naturaleza = "",
                                      }).ToList();

                    /// mequedeeeeeeeeeeee//
                    var LetrasHaberCompra = (from a in dbContext.letraspagar

                                             join b in dbContext.letraspagarcanje on new { IdLetras = a.v_IdLetrasPagar, eliminado = 0 } equals new { IdLetras = b.v_IdLetrasPagar, eliminado = b.i_Eliminado.Value } into b_join

                                             from b in b_join.DefaultIfEmpty()

                                             //join b in dbContext.letraspagardetalle on new { IdLetras = a.v_IdLetrasPagar, eliminado = 0 } equals new { IdLetras = b.v_IdLetrasPagar, eliminado = 0 } into b_join
                                             //from b in b_join.DefaultIfEmpty()


                                             join c in dbContext.compra on new { IdCompra = b.v_IdCompra, eliminado = 0 } equals new { IdCompra = c.v_IdCompra, eliminado = c.i_Eliminado.Value } into c_join

                                             from c in c_join.DefaultIfEmpty()

                                             join e in dbContext.cliente on new { IdCliente = a.v_IdProveedor, eliminado = 0 } equals new { IdCliente = e.v_IdCliente, eliminado = e.i_Eliminado.Value } into e_join

                                             from e in e_join.DefaultIfEmpty()

                                             join g in dbContext.documento on new { DocumentoLetrasCanje = c.i_IdTipoDocumento.Value, eliminado = 0 } equals new { DocumentoLetrasCanje = g.i_CodigoDocumento, eliminado = g.i_Eliminado.Value } into g_join

                                             from g in g_join.DefaultIfEmpty()

                                             where a.i_Eliminado == 0 && e.v_CodCliente == pstrCodigoProveedor && b.v_IdCompra != null && b.v_IdLetrasPagarDetalle == null

                                             select new ReporteEstadoCuentaProveedor()
                                             {
                                                 Fecha = a == null ? DateTime.MinValue : a.t_FechaRegistro.Value,
                                                 Proveedor = e == null ? "**NO EXISTE PROVEEDOR**" : ("CÓDIGO : " + e.v_CodCliente.Trim() + "   -    DENOMINACIÓN: " + e.v_ApePaterno.Trim() + " " + e.v_ApeMaterno.Trim() + " " + e.v_PrimerNombre.Trim() + " " + e.v_SegundoNombre.Trim() + " " + e.v_RazonSocial.Trim()).Trim().ToUpper(),
                                                 pIntTipoDocumento = 0,
                                                 Concepto = g == null ? c == null ? "" : c.v_SerieDocumento + " " + c.v_CorrelativoDocumento : g.v_Siglas.Trim() + ".  " + c.v_SerieDocumento.Trim() + " " + c.v_CorrelativoDocumento.Trim(),
                                                 Referencia = "REFINANCIACIÓN POR ", // C.v_Glosa.Trim(),
                                                 HaberSoles = a.i_IdMoneda == (int)Currency.Soles ? b.d_MontoCanjeado.Value : 0,
                                                 HaberDolares = a.i_IdMoneda == (int)Currency.Dolares ? b.d_MontoCanjeado.Value : 0,
                                                 DebeSoles = 0,
                                                 DebeDolares = 0,
                                                 FechaInsercion = b == null ? DateTime.MinValue : b.t_ActualizaFecha == null ? b.t_InsertaFecha.Value : b.t_ActualizaFecha.Value,
                                                 // DocumentoReferencia = g== null ?""  : "CANJDE DE " +g.v_Nombre.Trim (),
                                                 DocumentoReferencia = "",
                                                 IdLetras = b == null ? "" : b.v_IdLetrasPagar,
                                                 Naturaleza = "H",
                                             }).ToList();


                    var LetrasHaberDesdeLetras = (from a in dbContext.letraspagar

                                                  join b in dbContext.letraspagarcanje on new { IdLetras = a.v_IdLetrasPagar, eliminado = 0 } equals new { IdLetras = b.v_IdLetrasPagar, eliminado = b.i_Eliminado.Value } into b_join

                                                  from b in b_join.DefaultIfEmpty()

                                                  //join b in dbContext.letraspagardetalle on new { IdLetras = a.v_IdLetrasPagar, eliminado = 0 } equals new { IdLetras = b.v_IdLetrasPagar, eliminado = 0 } into b_join
                                                  //from b in b_join.DefaultIfEmpty()

                                                  join c in dbContext.letraspagardetalle on new { IdLetrasPagarDetalle = b.v_IdLetrasPagarDetalle, eliminado = 0 } equals new { IdLetrasPagarDetalle = c.v_IdLetrasPagarDetalle, eliminado = c.i_Eliminado.Value } into c_join

                                                  from c in c_join.DefaultIfEmpty()

                                                  join e in dbContext.cliente on new { IdCliente = a.v_IdProveedor, eliminado = 0 } equals new { IdCliente = e.v_IdCliente, eliminado = e.i_Eliminado.Value } into e_join

                                                  from e in e_join.DefaultIfEmpty()

                                                  join g in dbContext.documento on new { DocumentoLetrasCanje = c.i_IdTipoDocumento.Value, eliminado = 0 } equals new { DocumentoLetrasCanje = g.i_CodigoDocumento, eliminado = g.i_Eliminado.Value } into g_join

                                                  from g in g_join.DefaultIfEmpty()

                                                  where a.i_Eliminado == 0 && e.v_CodCliente == pstrCodigoProveedor && b.v_IdCompra == null && b.v_IdLetrasPagarDetalle != null

                                                  select new ReporteEstadoCuentaProveedor()
                                                  {
                                                      Fecha = a == null ? DateTime.MinValue : a.t_FechaRegistro.Value,
                                                      Proveedor = e == null ? "**NO EXISTE PROVEEDOR**" : ("CÓDIGO : " + e.v_CodCliente.Trim() + "   -    DENOMINACIÓN: " + e.v_ApePaterno.Trim() + " " + e.v_ApeMaterno.Trim() + " " + e.v_PrimerNombre.Trim() + " " + e.v_SegundoNombre.Trim() + " " + e.v_RazonSocial.Trim()).Trim().ToUpper(),
                                                      pIntTipoDocumento = 0,
                                                      Concepto = g == null ? c == null ? "" : c.v_Serie + " " + c.v_Correlativo : g.v_Siglas.Trim() + ".  " + c.v_Serie.Trim() + " " + c.v_Correlativo.Trim(),
                                                      Referencia = "REFINANCIACIÓN POR ", // C.v_Glosa.Trim(),
                                                      HaberSoles = a.i_IdMoneda == (int)Currency.Soles ? b.d_MontoCanjeado.Value : 0,
                                                      HaberDolares = a.i_IdMoneda == (int)Currency.Dolares ? b.d_MontoCanjeado.Value : 0,
                                                      DebeSoles = 0,
                                                      DebeDolares = 0,
                                                      FechaInsercion = b == null ? DateTime.MinValue : b.t_ActualizaFecha == null ? b.t_InsertaFecha.Value : b.t_ActualizaFecha.Value,
                                                      // DocumentoReferencia = g== null ?""  : "CANJDE DE " +g.v_Nombre.Trim (),
                                                      DocumentoReferencia = "",
                                                      IdLetras = b == null ? "" : b.v_IdLetrasPagar,
                                                      Naturaleza = "H",
                                                  }).ToList();





                    //var LetrasDebeCompra = (from a in dbContext.letraspagar
                    //                        join b in dbContext.letraspagarcanje on new { IdLetras = a.v_IdLetrasPagar, eliminado = 0 } equals new { IdLetras = b.v_IdLetrasPagar, eliminado = 0 } into b_join
                    //                        from b in b_join.DefaultIfEmpty()

                    //                        join e in dbContext.cliente on new { IdProveedor = a.v_IdProveedor, eliminado = 0 } equals new { IdProveedor = e.v_IdCliente, eliminado = e.i_Eliminado.Value } into e_join

                    //                        from e in e_join.DefaultIfEmpty()

                    //                        join f in dbContext.compra on new { IdCompra = b.v_IdCompra, eliminado = 0 } equals new { IdCompra = f.v_IdCompra, eliminado = f.i_Eliminado.Value } into f_join


                    //                        from f in f_join.DefaultIfEmpty()

                    //                        join g in dbContext.documento on new { DocumentoCompra = f.i_IdTipoDocumento.Value, eliminado = 0 } equals new { DocumentoCompra = g.i_CodigoDocumento, eliminado = g.i_Eliminado.Value } into g_join

                    //                        from g in g_join.DefaultIfEmpty()

                    //                        where a.i_Eliminado == 0 && e.v_CodCliente == pstrCodigoProveedor && b.v_IdCompra !=null && b.v_IdLetrasPagarDetalle ==null 
                    //                        select new ReporteEstadoCuentaProveedor()
                    //                        {
                    //                            Fecha = a == null ? DateTime.MinValue : a.t_FechaRegistro.Value,
                    //                            Proveedor = e == null ? "**NO EXISTE PROVEEDOR**" : ("CÓDIGO : " + e.v_CodCliente.Trim() + "   -    DENOMINACIÓN: " + e.v_ApePaterno.Trim() + " " + e.v_ApeMaterno.Trim() + " " + e.v_PrimerNombre.Trim() + " " + e.v_SegundoNombre.Trim() + " " + e.v_RazonSocial.Trim()).Trim().ToUpper(),
                    //                            pIntTipoDocumento = 0,
                    //                            //Concepto =  i.v_Siglas + ".  " + h.v_Serie + " " + h.v_Correlativo,
                    //                            Concepto = g.v_Siglas + ". " + f.v_SerieDocumento + " " + f.v_CorrelativoDocumento,
                    //                            Referencia = "REFERENCIA ",
                    //                            HaberSoles = 0,
                    //                            HaberDolares = 0,
                    //                            DebeSoles = a.i_IdMoneda == (int)Currency.Soles ? b == null ? 0 : b.d_MontoCanjeado.Value : 0,
                    //                            DebeDolares = a.i_IdMoneda == (int)Currency.Dolares ? b == null ? 0 : b.d_MontoCanjeado.Value : 0,
                    //                            FechaInsercion = b.t_ActualizaFecha == null ? b.t_InsertaFecha.Value : b.t_ActualizaFecha.Value,
                    //                            DocumentoReferencia = "",
                    //                            IdLetras = b == null ? "" : b.v_IdLetrasPagar,
                    //                            Naturaleza = "D",
                    //                            IdLetrasDetalle = b == null ? "" : b.v_IdLetrasPagarDetalle == null ? "" : b.v_IdLetrasPagarDetalle,
                    //                        }).ToList();



                    var LetrasDebeDesdeLetras = (from a in dbContext.letraspagar


                                                 join h in dbContext.letraspagardetalle on new { IdLetrasDetalle = a.v_IdLetrasPagar, eliminado = 0 } equals new { IdLetrasDetalle = h.v_IdLetrasPagar, eliminado = h.i_Eliminado.Value } into h_join

                                                 from h in h_join.DefaultIfEmpty()

                                                 join i in dbContext.documento on new { DocumentoLetrasDellate = h.i_IdTipoDocumento.Value, eliminado = 0 } equals new { DocumentoLetrasDellate = i.i_CodigoDocumento, eliminado = i.i_Eliminado.Value } into i_join

                                                 from i in i_join.DefaultIfEmpty()

                                                 join e in dbContext.cliente on new { IdProveedor = h.v_IdProveedor, eliminado = 0 } equals new { IdProveedor = e.v_IdCliente, eliminado = e.i_Eliminado.Value } into e_join

                                                 from e in e_join.DefaultIfEmpty()
                                                 where a.i_Eliminado == 0 && e.v_CodCliente == pstrCodigoProveedor

                                                 select new ReporteEstadoCuentaProveedor()
                                                 {
                                                     Fecha = a == null ? DateTime.MinValue : a.t_FechaRegistro.Value,
                                                     Proveedor = e == null ? "**NO EXISTE PROVEEDOR**" : ("CÓDIGO : " + e.v_CodCliente.Trim() + "   -    DENOMINACIÓN: " + e.v_ApePaterno.Trim() + " " + e.v_ApeMaterno.Trim() + " " + e.v_PrimerNombre.Trim() + " " + e.v_SegundoNombre.Trim() + " " + e.v_RazonSocial.Trim()).Trim().ToUpper(),
                                                     pIntTipoDocumento = 0,
                                                     //Concepto =  i.v_Siglas + ".  " + h.v_Serie + " " + h.v_Correlativo,
                                                     Concepto = i.v_Siglas + ". " + h.v_Serie + " " + h.v_Correlativo,
                                                     Referencia = "REFERENCIA ",
                                                     HaberSoles = 0,
                                                     HaberDolares = 0,
                                                     DebeSoles = a.i_IdMoneda == (int)Currency.Soles ? h == null ? 0 : h.d_Importe.Value : 0,
                                                     DebeDolares = a.i_IdMoneda == (int)Currency.Dolares ? h == null ? 0 : h.d_Importe.Value : 0,
                                                     FechaInsercion = h.t_ActualizaFecha == null ? h.t_InsertaFecha.Value : h.t_ActualizaFecha.Value,
                                                     DocumentoReferencia = "",
                                                     IdLetras = h == null ? "" : h.v_IdLetrasPagar,
                                                     Naturaleza = "D",
                                                     IdLetrasDetalle = h == null ? "" : h.v_IdLetrasPagarDetalle == null ? "" : h.v_IdLetrasPagarDetalle,
                                                 }).ToList();



                    var SaldoInicialesDebe = (from a in dbContext.letraspagardetalle
                                              join b in dbContext.cliente on new { IdCliente = a.v_IdProveedor, eliminado = 0 } equals new { IdCliente = b.v_IdCliente, eliminado = b.i_Eliminado.Value } into b_join

                                              from b in b_join.DefaultIfEmpty()
                                              join c in dbContext.documento on new { TipoDoc = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { TipoDoc = c.i_CodigoDocumento, eliminado = c.i_Eliminado.Value } into c_join
                                              from c in c_join.DefaultIfEmpty()
                                              where a.i_Eliminado == 0 && b.v_CodCliente == pstrCodigoProveedor && a.i_EsSaldoInicial == 1
                                              select new ReporteEstadoCuentaProveedor()
                                              {
                                                  Fecha = a == null ? DateTime.MinValue : a.t_FechaEmision.Value,
                                                  Proveedor = b == null ? "**NO EXISTE PROVEEDOR**" : ("CÓDIGO : " + b.v_CodCliente.Trim() + "   -    DENOMINACIÓN: " + b.v_ApePaterno.Trim() + " " + b.v_ApeMaterno.Trim() + " " + b.v_PrimerNombre.Trim() + " " + b.v_SegundoNombre.Trim() + " " + b.v_RazonSocial.Trim()).Trim().ToUpper(),
                                                  pIntTipoDocumento = 0,
                                                  Concepto = c == null ? a.v_Serie + " " + a.v_Correlativo : c.v_Siglas.Trim() + ".  " + a.v_Serie.Trim() + " " + a.v_Correlativo.Trim(),
                                                  Referencia = "LETRA INICIAL ", // C.v_Glosa.Trim(),
                                                  DebeSoles = a.i_IdMoneda == (int)Currency.Soles ? a.d_Importe.Value : 0,
                                                  DebeDolares = a.i_IdMoneda == (int)Currency.Dolares ? a.d_Importe.Value : 0,
                                                  HaberSoles = 0,
                                                  HaberDolares = 0,
                                                  FechaInsercion = a.t_ActualizaFecha == null ? a.t_InsertaFecha.Value : a.t_ActualizaFecha.Value,
                                                  // DocumentoReferencia = g== null ?""  : "CANJDE DE " +g.v_Nombre.Trim (),
                                                  DocumentoReferencia = "",
                                                  IdLetras = a.v_IdLetrasPagarDetalle,
                                                  Naturaleza = "",
                                              }).ToList();
                    // var ExisteCanjeado = (LetrasHaber.Where (IdLetrasDetalle)
                    var RegistrosTotal = queryDebe.Concat(queryHaber).ToList().Concat(LetrasHaberCompra).Concat(LetrasHaberDesdeLetras).Concat(LetrasDebeDesdeLetras).Concat(SaldoInicialesDebe);
                    var RegistrosTotalFecha = RegistrosTotal.Where(x => x.Fecha >= FechaDesde && x.Fecha <= FechaHasta).ToList().OrderBy(x => x.Fecha).ThenBy(x => x.Concepto).ToList();
                    objReporte = new ReporteEstadoCuentaProveedor();
                    objReporte.Proveedor = queryDebe.Count() != 0 ? queryDebe.FirstOrDefault().Proveedor : queryHaber.Count() != 0 ? queryHaber.FirstOrDefault().Proveedor : "";
                    objReporte.pIntTipoDocumento = 1;
                    objReporte.Concepto = "";
                    objReporte.Referencia = " SALDO DEUDOR AL   " + FechaSaldoDeudor.Date.Day.ToString("00") + "/" + FechaSaldoDeudor.Date.Month.ToString("00") + "/" + FechaSaldoDeudor.Date.Year.ToString() + " : ";
                    //objReporte.DebeSoles = RegistrosTotal.Where(x => x.Fecha <= FechaSaldoDeudor && x.pIntTipoDocumento != (int)TiposDocumentos.NotaCredito).ToList().OrderBy(x => x.Fecha).ThenBy(x => x.Concepto).ToList().Sum(x => x.DebeSoles) - RegistrosTotal.Where(x => x.Fecha <= FechaSaldoDeudor && x.pIntTipoDocumento == (int)TiposDocumentos.NotaCredito).ToList().OrderBy(x => x.Fecha).ThenBy(x => x.Concepto).ToList().Sum(x => x.DebeSoles);
                    //objReporte.DebeDolares = RegistrosTotal.Where(x => x.Fecha <= FechaSaldoDeudor && x.pIntTipoDocumento != (int)TiposDocumentos.NotaCredito).ToList().OrderBy(x => x.Fecha).ThenBy(x => x.Concepto).ToList().Sum(x => x.DebeDolares) - RegistrosTotal.Where(x => x.Fecha <= FechaSaldoDeudor && x.pIntTipoDocumento == (int)TiposDocumentos.NotaCredito).ToList().OrderBy(x => x.Fecha).ThenBy(x => x.Concepto).ToList().Sum(x => x.DebeDolares);
                    //objReporte.HaberSoles = RegistrosTotal.Where(x => x.Fecha <= FechaSaldoDeudor && x.pIntTipoDocumento != (int)TiposDocumentos.NotaCredito).ToList().OrderBy(x => x.Fecha).ThenBy(x => x.Concepto).ToList().Sum(x => x.HaberSoles) - RegistrosTotal.Where(x => x.Fecha <= FechaSaldoDeudor && x.pIntTipoDocumento == (int)TiposDocumentos.NotaCredito).ToList().OrderBy(x => x.Fecha).ThenBy(x => x.Concepto).ToList().Sum(x => x.HaberSoles);
                    //objReporte.HaberDolares = RegistrosTotal.Where(x => x.Fecha <= FechaSaldoDeudor && x.pIntTipoDocumento != (int)TiposDocumentos.NotaCredito).ToList().OrderBy(x => x.Fecha).ThenBy(x => x.Concepto).ToList().Sum(x => x.HaberDolares) - RegistrosTotal.Where(x => x.Fecha <= FechaSaldoDeudor && x.pIntTipoDocumento == (int)TiposDocumentos.NotaCredito).ToList().OrderBy(x => x.Fecha).ThenBy(x => x.Concepto).ToList().Sum(x => x.HaberDolares);
                    objReporte.DebeSoles = RegistrosTotal.Where(x => x.Fecha <= FechaSaldoDeudor && !_objDocumentoBL.DocumentoEsInverso(x.pIntTipoDocumento)).ToList().OrderBy(x => x.Fecha).ThenBy(x => x.Concepto).ToList().Sum(x => x.DebeSoles) - RegistrosTotal.Where(x => x.Fecha <= FechaSaldoDeudor && _objDocumentoBL.DocumentoEsInverso(x.pIntTipoDocumento)).ToList().OrderBy(x => x.Fecha).ThenBy(x => x.Concepto).ToList().Sum(x => x.DebeSoles);
                    objReporte.DebeDolares = RegistrosTotal.Where(x => x.Fecha <= FechaSaldoDeudor && !_objDocumentoBL.DocumentoEsInverso(x.pIntTipoDocumento)).ToList().OrderBy(x => x.Fecha).ThenBy(x => x.Concepto).ToList().Sum(x => x.DebeDolares) - RegistrosTotal.Where(x => x.Fecha <= FechaSaldoDeudor && _objDocumentoBL.DocumentoEsInverso(x.pIntTipoDocumento)).ToList().OrderBy(x => x.Fecha).ThenBy(x => x.Concepto).ToList().Sum(x => x.DebeDolares);
                    objReporte.HaberSoles = RegistrosTotal.Where(x => x.Fecha <= FechaSaldoDeudor && !_objDocumentoBL.DocumentoEsInverso(x.pIntTipoDocumento)).ToList().OrderBy(x => x.Fecha).ThenBy(x => x.Concepto).ToList().Sum(x => x.HaberSoles) - RegistrosTotal.Where(x => x.Fecha <= FechaSaldoDeudor && _objDocumentoBL.DocumentoEsInverso(x.pIntTipoDocumento)).ToList().OrderBy(x => x.Fecha).ThenBy(x => x.Concepto).ToList().Sum(x => x.HaberSoles);
                    objReporte.HaberDolares = RegistrosTotal.Where(x => x.Fecha <= FechaSaldoDeudor && !_objDocumentoBL.DocumentoEsInverso(x.pIntTipoDocumento)).ToList().OrderBy(x => x.Fecha).ThenBy(x => x.Concepto).ToList().Sum(x => x.HaberDolares) - RegistrosTotal.Where(x => x.Fecha <= FechaSaldoDeudor && _objDocumentoBL.DocumentoEsInverso(x.pIntTipoDocumento)).ToList().OrderBy(x => x.Fecha).ThenBy(x => x.Concepto).ToList().Sum(x => x.HaberDolares);
                    objReporte.DocumentoReferencia = "";
                    objReporte.Movimiento = "SA";
                    ListaFinal.Add(objReporte);

                    foreach (var item in RegistrosTotalFecha)
                    {
                        string Referencia = string.Empty;
                        string ReferenciaGenera = String.Empty;
                        objReporte = new ReporteEstadoCuentaProveedor();
                        objReporte.Fecha = item.Fecha;
                        objReporte.Proveedor = item.Proveedor;
                        objReporte.pIntTipoDocumento = item.pIntTipoDocumento;
                        objReporte.Concepto = item.Concepto;

                        if (item.Naturaleza == "H")
                        {
                            var LetrasGeneradas = (from a in dbContext.letraspagardetalle
                                                   join b in dbContext.documento on new { Doc = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { Doc = b.i_CodigoDocumento, eliminado = b.i_Eliminado.Value } into b_join

                                                   from b in b_join.DefaultIfEmpty()

                                                   where a.i_Eliminado == 0 && a.v_IdLetrasPagar == item.IdLetras

                                                   select new
                                                   {
                                                       NumeroDocumento = a.v_Serie.Trim() + "-" + a.v_Correlativo.Trim(),
                                                       TipoDocumento = b == null ? "" : b.v_Siglas.Trim(),
                                                   }).ToList().OrderBy(x => x.TipoDocumento).ThenBy(x => x.NumeroDocumento).ToList();

                            if (LetrasGeneradas != null)
                            {
                                foreach (var letra in LetrasGeneradas)
                                {
                                    Referencia = Referencia + " " + letra.TipoDocumento + ". " + letra.NumeroDocumento.Trim();
                                }

                            }
                            objReporte.Referencia = (item.Referencia + " " + Referencia).Trim();
                        }
                        else
                        {

                            var DeDondeSeGenera1 = (from a in dbContext.letraspagarcanje

                                                    join b in dbContext.compra on new { IdCompra = a.v_IdCompra, eliminado = 0 } equals new { IdCompra = b.v_IdCompra, eliminado = b.i_Eliminado.Value } into b_join

                                                    from b in b_join.DefaultIfEmpty()

                                                    join c in dbContext.documento on new { DocumentoCompra = b.i_IdTipoDocumento.Value, eliminado = 0 } equals new { DocumentoCompra = c.i_CodigoDocumento, eliminado = c.i_Eliminado.Value } into c_join

                                                    from c in c_join.DefaultIfEmpty()

                                                    where a.i_Eliminado == 0 && a.v_IdCompra != null && a.v_IdLetrasPagarDetalle == null && a.v_IdLetrasPagar == item.IdLetras

                                                    select new
                                                    {
                                                        NumeroDocumento = c == null ? b.v_SerieDocumento.Trim() + "-" + b.v_CorrelativoDocumento.Trim() : c.v_Siglas.Trim() + ". " + b.v_SerieDocumento + "-" + b.v_CorrelativoDocumento,
                                                    }).OrderBy(x => x.NumeroDocumento).ToList().Distinct();


                            var DeDondeSeGenera2 = (from a in dbContext.letraspagarcanje

                                                    join d in dbContext.letraspagardetalle on new { LetrasDetalle = a.v_IdLetrasPagarDetalle, eliminado = 0 } equals new { LetrasDetalle = d.v_IdLetrasPagarDetalle, eliminado = d.i_Eliminado.Value } into d_join

                                                    from d in d_join.DefaultIfEmpty()

                                                    join e in dbContext.documento on new { DocumentoLD = d.i_IdTipoDocumento.Value, eliminado = 0 } equals new { DocumentoLD = e.i_CodigoDocumento, eliminado = e.i_Eliminado.Value } into e_join

                                                    from e in e_join.DefaultIfEmpty()

                                                    where a.i_Eliminado == 0 && a.v_IdCompra == null && a.v_IdLetrasPagarDetalle != null && a.v_IdLetrasPagar == item.IdLetras
                                                    select new
                                                    {
                                                        NumeroDocumento = e == null ? d.v_Serie + "-" + d.v_Correlativo : e.v_Siglas + " " + d.v_Serie + "-" + d.v_Correlativo,
                                                    }).OrderBy(x => x.NumeroDocumento).ToList().Distinct();


                            var DondeSeGenera = DeDondeSeGenera1.Concat(DeDondeSeGenera2);
                            //objReporte.Referencia = item.Referencia;
                            if (DondeSeGenera != null)
                            {
                                foreach (var letra in DondeSeGenera)
                                {
                                    ReferenciaGenera = ReferenciaGenera + " " + letra.NumeroDocumento;
                                }

                            }
                            objReporte.Referencia = (item.Referencia + " " + ReferenciaGenera).Trim();
                        }
                        objReporte.DebeSoles = item.DebeSoles;
                        objReporte.DebeDolares = item.DebeDolares;
                        objReporte.HaberSoles = item.HaberSoles;
                        objReporte.HaberDolares = item.HaberDolares;
                        objReporte.DocumentoReferencia = item.DocumentoReferencia;
                        objReporte.Movimiento = "MN";
                        objReporte.FechaInsercion = item.FechaInsercion;

                        //if (objReporte.pIntTipoDocumento == (int)TiposDocumentos.NotaCredito) //En caso solo se haga una Ncr para eliminar una factura ,ésta debe aparecer en el reporte, pero si ha sido generada y utilizada con otra factura ,ésta no debe aparecer
                        if (_objDocumentoBL.DocumentoEsInverso(objReporte.pIntTipoDocumento))
                        {
                            var Nrc = RegistrosTotal.Where(x => x.DocumentoReferencia == objReporte.Concepto).ToList();
                            if (Nrc.Count == 0)
                            {
                                ListaFinal.Add(objReporte);
                            }

                        }
                        else
                        {
                            ListaFinal.Add(objReporte);
                        }
                    }
                    return ListaFinal.OrderBy(x => x.FechaInsercion).ToList();
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        private class Ubicacion
        {
            public string Siglas { get; set; }
            public string NombreCompleto { get; set; }
        }
    }
}
