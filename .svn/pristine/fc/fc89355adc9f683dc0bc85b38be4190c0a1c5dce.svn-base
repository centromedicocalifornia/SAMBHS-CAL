using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.BL;
using System.ComponentModel;
using SAMBHS.Tesoreria.BL;
using System.Transactions;
using SAMBHS.Venta.BL;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SAMBHS.Cobranza.BL
{
    public class CobranzaBL
    {
        readonly DocumentoBL _objDocumentoBL = new DocumentoBL();
        private string periodo = Globals.ClientSession.i_Periodo.Value.ToString();
        public List<KeyValueDTO> ObtenerListadoCobranzas(ref OperationResult pobjOperationResult, string pstringPeriodo, string pstringMes, int IdTipoDocumento)
        {

            var almacenpredeterminado = Globals.ClientSession.i_IdAlmacenPredeterminado.Value.ToString("00");
            string replicationID = Globals.ClientSession.ReplicationNodeID;
            var dbcontext = new SAMBHSEntitiesModelWin();
            var query = (from n in dbcontext.cobranza
                         where n.i_Eliminado == 0 && n.v_Periodo == pstringPeriodo && n.v_Mes == pstringMes && n.i_IdTipoDocumento == IdTipoDocumento
                           && n.v_IdCobranza.Substring(2, 2) == almacenpredeterminado && n.v_IdCobranza.Substring(0, 1) == replicationID
                         orderby n.v_Correlativo ascending
                         select new
                         {
                             v_Correlativo = n.v_Correlativo,
                             v_IdVenta = n.v_IdCobranza
                         }
                         );

            var query2 = query.AsEnumerable()
                   .Select(x => new KeyValueDTO
                   {
                       Value1 = x.v_Correlativo,
                       Value2 = x.v_IdVenta
                   }).ToList();
            return query2;

        }

        public List<cobranzapendienteDto> ListarCobranzasPendientes(ref OperationResult pobjOperationResult, string pstrSortExpression,
            string pstrIdCliente, DateTime F_Ini, DateTime F_Fin, bool IncluirLetras,
            bool soloLetras = false, int idTipoDocumento = -1, string serieDocumento = null, string correlativoDocumento = null)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    pobjOperationResult.Success = 1;

                    var objData = new List<cobranzapendienteDto>();

                    if (!soloLetras)
                    {
                        var query = (from n in dbContext.cobranzapendiente

                                     join A in dbContext.venta on n.v_IdVenta equals A.v_IdVenta into A_join
                                     from A in A_join.DefaultIfEmpty()

                                     join B in dbContext.cliente on A.v_IdCliente equals B.v_IdCliente into B_join
                                     from B in B_join.DefaultIfEmpty()

                                     join J4 in dbContext.documento on new { i_IdTipoDocumento = A.i_IdTipoDocumento.Value }
                                         equals new { i_IdTipoDocumento = J4.i_CodigoDocumento } into J4_join
                                     from J4 in J4_join.DefaultIfEmpty()

                                     where
                                         n.i_Eliminado == 0 && A.t_FechaRegistro >= F_Ini && A.t_FechaRegistro <= F_Fin &&
                                         n.d_Saldo > 0
                                         && A.i_IdEstablecimiento == Globals.ClientSession.i_IdEstablecimiento.Value
                                         && B_join.Any(p => A.v_IdCliente == p.v_IdCliente)
                                         && (A.i_IdTipoDocumento == idTipoDocumento || idTipoDocumento == -1)

                                     select new cobranzapendienteDto
                                     {
                                         v_IdVenta = A.v_IdVenta,
                                         v_IdCliente = B.v_IdCliente,
                                         NombreCliente =
                                             A.v_IdCliente != "N002-CL000000000"
                                                 ? (B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " +
                                                    B.v_RazonSocial).Trim()
                                                 : (A.v_NombreClienteTemporal != null && A.v_NombreClienteTemporal.Trim() != "")
                                                     ? A.v_NombreClienteTemporal
                                                     : "PÚBLICO GENERAL",
                                         TipoDocumento = J4.v_Siglas,
                                         i_IdTipoDocumento = A.i_IdTipoDocumento,
                                         NroDocumento = A.v_SerieDocumento + "-" + A.v_CorrelativoDocumento,
                                         FechaRegistro = A.t_FechaRegistro.Value,
                                         Moneda = A.i_IdMoneda == 1 ? "S/." : "US$.",
                                         Importe = A.d_Total.Value,
                                         d_Acuenta = n.d_Acuenta,
                                         d_Saldo = n.d_Saldo,
                                         TipoCambio = A.d_TipoCambio.Value,
                                         ValorVenta = A.d_Valor.Value,
                                         EsLetra = false,
                                         EsDocInverso = J4.i_UsadoDocumentoInverso ?? 0,
                                         EsLetraDescuentoCobrada = false
                                     }
                            );

                        if (!string.IsNullOrEmpty(pstrIdCliente))
                            query = query.Where(p => p.v_IdCliente.Equals(pstrIdCliente));

                        if (idTipoDocumento != -1 && !string.IsNullOrEmpty(serieDocumento)
                            && !string.IsNullOrWhiteSpace(correlativoDocumento))
                        {
                            var nroDoc = string.Format("{0}-{1}", serieDocumento, correlativoDocumento);
                            query = query.Where(p => p.NroDocumento.Equals(nroDoc) && p.i_IdTipoDocumento == idTipoDocumento);
                        }

                        objData = query.OrderBy(p => p.FechaRegistro).ToList();
                    }

                    #region Agrega la consulta de letras pendientes por cobrar al resultado

                    if (IncluirLetras || soloLetras)
                    {
                        var query2 = (from n in dbContext.cobranzaletraspendiente

                            join A in dbContext.letrasdetalle on n.v_IdLetrasDetalle equals A.v_IdLetrasDetalle into
                                A_join
                            from A in A_join.DefaultIfEmpty()

                            join B in dbContext.cliente on A.v_IdCliente equals B.v_IdCliente into B_join
                            from B in B_join.DefaultIfEmpty()

                            join J4 in dbContext.documento on new {i_IdTipoDocumento = A.i_IdTipoDocumento.Value}
                                equals new {i_IdTipoDocumento = J4.i_CodigoDocumento} into J4_join
                            from J4 in J4_join.DefaultIfEmpty()

                            where n.i_Eliminado == 0 && A.t_FechaEmision >= F_Ini && A.t_FechaEmision <= F_Fin &&
                                  n.d_Saldo > 0 &&
                                  (J4.i_UsadoDocumentoInverso == null || J4.i_UsadoDocumentoInverso == 0)
                                  && (A.i_IdTipoDocumento == idTipoDocumento || idTipoDocumento == -1)
                            select new
                            {
                                v_IdVenta = A.v_IdLetrasDetalle,
                                v_IdCliente = B.v_IdCliente,
                                NombreCliente =
                                    A.v_IdCliente != "N002-CL000000000"
                                        ? (B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " +
                                           B.v_RazonSocial).Trim()
                                        : "CLIENTE PUBLICO GENERAL",
                                TipoDocumento = J4.v_Siglas,
                                i_IdTipoDocumento = A.i_IdTipoDocumento,
                                NroDocumento = A.v_Serie.Trim() + "-" + A.v_Correlativo,
                                FechaRegistro = A.t_FechaEmision.Value,
                                Moneda = A.i_IdMoneda == 1 ? "S/." : "US$.",
                                Importe = A.d_Importe.Value,
                                d_Acuenta = n.d_Acuenta,
                                d_Saldo = n.d_Saldo,
                                TipoCambio = A.d_TipoCambio.Value,
                                ValorVenta = A.d_Importe.Value,
                                EsLetra = true,
                                FechaVencimiento = A.t_FechaVencimiento.Value,
                                CobranzasRealizadas = (from i in dbContext.cobranzadetalle
                                    where i.v_IdVenta == n.v_IdLetrasDetalle && i.i_Eliminado == 0
                                    select i).Count(),
                                Correlativo = A.v_Correlativo
                            }
                            );

                        if (!string.IsNullOrEmpty(pstrIdCliente))
                            query2 = query2.Where(p => p.v_IdCliente.Equals(pstrIdCliente));

                        if (idTipoDocumento != -1 && !string.IsNullOrEmpty(serieDocumento)
                            && !string.IsNullOrWhiteSpace(correlativoDocumento))
                        {
                            var nroDoc = string.Format("{0}-{1}", serieDocumento, correlativoDocumento);
                            query2 = query2.Where(p => p.NroDocumento.Equals(nroDoc) && p.i_IdTipoDocumento == idTipoDocumento);
                        }

                        var letrasPendientes = query2.ToList();

                        if (!letrasPendientes.Any()) return objData;
                        var idLetras = letrasPendientes.Select(p => p.v_IdVenta).ToList();
                        var dsTemp = dbContext.letrasmantenimientodetalle.Where(p => p.i_Eliminado == 0 && idLetras.Contains(p.v_IdLetrasDetalle)).ToList();
                        var dsTempEstados = dbContext.datahierarchy.Where(p => p.i_GroupId == 110 && p.i_IsDeleted == 0).ToList();

                        var letras = letrasPendientes.Select(n =>
                            {
                                var ultimaUbicacion = ObtenerUltimaUbicacionLetra(n.v_IdVenta, dsTemp, dsTempEstados);
                                return new cobranzapendienteDto
                                {
                                    v_IdVenta = n.v_IdVenta,
                                    v_IdCliente = n.v_IdCliente,
                                    NombreCliente = n.NombreCliente,
                                    TipoDocumento = n.TipoDocumento,
                                    i_IdTipoDocumento = n.i_IdTipoDocumento,
                                    NroDocumento = n.NroDocumento,
                                    FechaRegistro = n.FechaRegistro,
                                    Moneda = n.Moneda,
                                    Importe = n.Importe,
                                    d_Acuenta = n.d_Acuenta,
                                    d_Saldo = n.d_Saldo,
                                    TipoCambio = n.TipoCambio,
                                    ValorVenta = n.ValorVenta,
                                    EsLetra = n.EsLetra,
                                    Ubicacion = ultimaUbicacion.Siglas,
                                    UbicacionNombreCompleto = ultimaUbicacion.NombreCompleto,
                                    FechaVencimiento = n.FechaVencimiento,
                                    Estado = ultimaUbicacion.Estado,
                                    EsLetraDescuentoCobrada = EsLetraDescuento(n.v_IdVenta, false) && (n.CobranzasRealizadas > 0 || int.Parse(n.Correlativo.Split('-')[1]) > 0)
                                };
                            }).ToList();

                        if (letrasPendientes.Any())
                        {
                            var objData2 = letras.OrderBy(p => p.FechaVencimiento).ToList();
                            objData = objData.Concat(objData2).ToList();
                        }
                    }
                    #endregion

                    
                    return objData;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        private static Ubicacion ObtenerUltimaUbicacionLetra(string pstrIdLetraDetalle, IEnumerable<letrasmantenimientodetalle> pdsTemp, IEnumerable<datahierarchy> pdsDataH)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var objUbicacion = new Ubicacion();

                    var consulta = pdsTemp.Where(p => p.v_IdLetrasDetalle == pstrIdLetraDetalle && p.i_Eliminado == 0).ToList();

                    if (consulta.Count > 0)
                    {
                        consulta = consulta.OrderBy(p => p.v_IdLetrasMantenimiento).ToList();

                        var ultimoRegistro = consulta.Last();

                        var documento = dbContext.documento.FirstOrDefault(p => p.i_CodigoDocumento == ultimoRegistro.i_IdUbicacion);
                        var estado = pdsDataH.FirstOrDefault(p => p.i_ItemId == (ultimoRegistro.i_IdEstado ?? 1));
                        if (documento != null)
                        {
                            objUbicacion.Siglas = documento.v_Siglas;
                            objUbicacion.NombreCompleto = documento.v_Nombre;
                        }
                        else
                        {
                            objUbicacion.Siglas = "*No Encontrado*";
                            objUbicacion.NombreCompleto = "*No Encontrado*";
                        }

                        objUbicacion.Estado = estado != null ? estado.v_Value1.Trim() : "*No Encontrado*";
                    }
                    else
                    {
                        objUbicacion.Siglas = "CJS";
                        objUbicacion.NombreCompleto = "OFICINA";
                        objUbicacion.Estado = "COBRANZA";
                    }

                    return objUbicacion;
                }
            }
            catch { return null; }
        }

        public cobranzaDto ObtenerCobranzaCabecera(ref OperationResult pobjOperationResult, string pstrIdCobranza)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var objEntity = (from a in dbContext.cobranza

                                     where a.v_IdCobranza == pstrIdCobranza
                                     select new cobranzaDto
                                     {
                                         v_IdCobranza = a.v_IdCobranza,
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
                                         i_IdEstablecimiento = a.i_IdEstablecimiento,
                                     }
                                             ).FirstOrDefault();

                    pobjOperationResult.Success = 1;

                    return objEntity;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public IQueryable ObtenerCobranzaDetalle(ref OperationResult pobjOperationResult, string pstrIdCobranza)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = (from n in dbContext.cobranzadetalle
                             where n.i_Eliminado == 0 && n.v_IdCobranza == pstrIdCobranza
                             orderby n.t_InsertaFecha ascending
                             select n
                             );

                pobjOperationResult.Success = 1;

                return query.AsQueryable();
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
                var query = new TipoCambioBL().DevolverTipoCambioPorFechaVenta(ref pobjOperationResult, Fecha);
                if (pobjOperationResult.Success == 0) return "0";
                return query;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return "0";
            }
        }

        public string[] DevolverNombres(string pstrIdVenta)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var EntityVenta = (from n in dbContext.venta

                                   join B in dbContext.cliente on n.v_IdCliente equals B.v_IdCliente into B_join
                                   from B in B_join.DefaultIfEmpty()

                                   join J4 in dbContext.documento on new { i_IdTipoDocumento = n.i_IdTipoDocumento.Value }
                                   equals new { i_IdTipoDocumento = J4.i_CodigoDocumento } into J4_join
                                   from J4 in J4_join.DefaultIfEmpty()

                                   where n.v_IdVenta == pstrIdVenta
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
        }

        public string[] DevolverNombresLetras(string pstrIdVenta)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var EntityVenta = (from n in dbContext.letrasdetalle

                                   join B in dbContext.cliente on n.v_IdCliente equals B.v_IdCliente into B_join
                                   from B in B_join.DefaultIfEmpty()

                                   join J4 in dbContext.documento on new { i_IdTipoDocumento = n.i_IdTipoDocumento.Value }
                                   equals new { i_IdTipoDocumento = J4.i_CodigoDocumento } into J4_join
                                   from J4 in J4_join.DefaultIfEmpty()

                                   where n.v_IdLetrasDetalle == pstrIdVenta
                                   select new
                                   {
                                       TipoDocumento = J4.v_Siglas,
                                       NroDocumento = n.v_Serie + " - " + n.v_Correlativo,
                                       NombreCliente = (B.v_PrimerNombre + " " + B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_RazonSocial).Trim(),
                                       IdMoneda = n.i_IdMoneda,
                                       Moneda = n.i_IdMoneda == 1 ? "S/." : "$.",
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
        }

        public string InsertarCobranza(ref OperationResult pobjOperationResult, cobranzaDto pobjDtoEntity, List<string> ClientSession, List<cobranzadetalleDto> pTemp_Insertar)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        SecuentialBL objSecuentialBL = new SecuentialBL();
                        cobranza objEntityCobranza = cobranzaAssembler.ToEntity(pobjDtoEntity);
                        DocumentoBL _objDocumentoBL = new DocumentoBL();
                        int SecuentialId = 0;
                        string newIdCobranza = string.Empty;
                        string newIdCobranzaDetalle = string.Empty;

                        #region Inserta Cabecera
                        objEntityCobranza.t_InsertaFecha = DateTime.Now;
                        objEntityCobranza.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntityCobranza.i_Eliminado = 0;
                        objEntityCobranza.i_IdEstablecimiento = Globals.ClientSession.i_IdEstablecimiento.Value;

                        var intNodeId = int.Parse(ClientSession[0]);
                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 44);
                        newIdCobranza = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "ZA");
                        objEntityCobranza.v_IdCobranza = newIdCobranza;
                        dbContext.AddTocobranza(objEntityCobranza);
                        dbContext.SaveChanges();
                        #endregion

                        #region Inserta Detalle
                        foreach (cobranzadetalleDto cobranzadetalleDto in pTemp_Insertar)
                        {
                            cobranzadetalle objEntityVentaDetalle = cobranzadetalleAssembler.ToEntity(cobranzadetalleDto);
                            SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 45);
                            newIdCobranzaDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "ZB");
                            objEntityVentaDetalle.v_IdCobranzaDetalle = newIdCobranzaDetalle;
                            objEntityVentaDetalle.v_IdCobranza = newIdCobranza;
                            objEntityVentaDetalle.t_InsertaFecha = DateTime.Now;
                            objEntityVentaDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                            objEntityVentaDetalle.i_Eliminado = 0;
                            dbContext.AddTocobranzadetalle(objEntityVentaDetalle);

                            #region Actualiza CobranzaPendiente
                            if (cobranzadetalleDto.i_EsLetra == 0 || cobranzadetalleDto.i_EsLetra == null)
                            {
                                ProcesaDetalleCobranza(ref pobjOperationResult, cobranzadetalleAssembler.ToDTO(objEntityVentaDetalle), pobjDtoEntity, ClientSession);
                                if (pobjOperationResult.Success == 0) return null;
                            }
                            else
                            {
                                ProcesaDetalleCobranzaLetras(ref pobjOperationResult, cobranzadetalleAssembler.ToDTO(objEntityVentaDetalle), pobjDtoEntity, ClientSession);
                                if (pobjOperationResult.Success == 0) return null;
                            }

                            #endregion
                        }
                        dbContext.SaveChanges();
                        #endregion

                        #region Genera Tesorería
                        GenerarTesoreria(ref pobjOperationResult, newIdCobranza);
                        if (pobjOperationResult.Success == 0) return null;
                        #endregion

                        pobjOperationResult.Success = 1;
                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "cobranza", newIdCobranza);
                        ts.Complete();
                        return newIdCobranza;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "CobranzaBL.InsertarCobranza()" + '\n' + "Linea: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = pobjOperationResult.ExceptionMessage != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public void ActualizarCobranza(ref OperationResult pobjOperationResult, cobranzaDto pobjDtoEntity, List<string> ClientSession, List<cobranzadetalleDto> pTemp_Insertar, List<cobranzadetalleDto> pTemp_Editar, List<cobranzadetalleDto> pTemp_Eliminar)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        int SecuentialId;
                        string newIdCobranzaDetalle = string.Empty;
                        int intNodeId;

                        #region Actualiza Cabecera
                        intNodeId = int.Parse(ClientSession[0]);
                        var objEntitySource = (from a in dbContext.cobranza
                                               where a.v_IdCobranza == pobjDtoEntity.v_IdCobranza
                                               select a).FirstOrDefault();

                        pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                        pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                        cobranza objEntity = cobranzaAssembler.ToEntity(pobjDtoEntity);
                        dbContext.cobranza.ApplyCurrentValues(objEntity);
                        #endregion

                        #region Actualiza Detalle
                        var objSecuentialBL = new SecuentialBL();
                        foreach (cobranzadetalleDto cobranzadetalleDto in pTemp_Insertar)
                        {
                            cobranzadetalle objEntityCobranzaDetalle = cobranzadetalleAssembler.ToEntity(cobranzadetalleDto);
                            SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 45);
                            newIdCobranzaDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "ZB");
                            objEntityCobranzaDetalle.v_IdCobranzaDetalle = newIdCobranzaDetalle;
                            objEntityCobranzaDetalle.t_InsertaFecha = DateTime.Now;
                            objEntityCobranzaDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                            objEntityCobranzaDetalle.i_Eliminado = 0;
                            dbContext.AddTocobranzadetalle(objEntityCobranzaDetalle);

                            #region Actualiza CobranzaPendiente
                            if (cobranzadetalleDto.i_EsLetra == 0 || cobranzadetalleDto.i_EsLetra == null)
                            {
                                ProcesaDetalleCobranza(ref pobjOperationResult, cobranzadetalleAssembler.ToDTO(objEntityCobranzaDetalle), pobjDtoEntity, ClientSession);
                                if (pobjOperationResult.Success == 0) return;
                            }
                            else
                            {
                                ProcesaDetalleCobranzaLetras(ref pobjOperationResult, cobranzadetalleAssembler.ToDTO(objEntityCobranzaDetalle), pobjDtoEntity, ClientSession);
                                if (pobjOperationResult.Success == 0) return;
                            }
                            #endregion

                            dbContext.SaveChanges();
                        }

                        foreach (cobranzadetalleDto cobranzadetalleDto in pTemp_Editar)
                        {
                            cobranzadetalle _objEntity = cobranzadetalleAssembler.ToEntity(cobranzadetalleDto);
                            var query = (from n in dbContext.cobranzadetalle
                                         where n.v_IdCobranzaDetalle == cobranzadetalleDto.v_IdCobranzaDetalle
                                         select n).FirstOrDefault();

                            _objEntity.t_ActualizaFecha = DateTime.Now;
                            _objEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                            dbContext.cobranzadetalle.ApplyCurrentValues(_objEntity);

                            dbContext.SaveChanges();

                            if (query != null)
                            {
                                if (query.i_EsLetra == 0 || query.i_EsLetra == null)
                                {
                                    RecalcularSaldoVenta(ref pobjOperationResult, query.v_IdVenta, ClientSession, false);
                                    if (pobjOperationResult.Success == 0) return;
                                }
                                else
                                {
                                    RestauraDetalleCobranzaLetras(ref pobjOperationResult, query.v_IdCobranzaDetalle, ClientSession);
                                    if (pobjOperationResult.Success == 0) return;
                                }

                                if (query.i_IdTipoDocumentoRef != null &&
                                  query.i_IdTipoDocumentoRef.Value == 433 &&
                                  query.v_DocumentoRef.Contains('-'))
                                {
                                    var serieCorrelativo = query.v_DocumentoRef.Split('-');
                                    new AdelantoBL().RecalcularSaldoAdelanto(ref pobjOperationResult, serieCorrelativo[0],
                                        serieCorrelativo[1], 433);
                                    if (pobjOperationResult.Success == 0) return;
                                }
                            }

                            Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "cobranzadetalle", query.v_IdCobranzaDetalle);

                        }

                        foreach (cobranzadetalleDto cobranzadetalleDto in pTemp_Eliminar)
                        {
                            cobranzadetalle _objEntity = cobranzadetalleAssembler.ToEntity(cobranzadetalleDto);
                            var query = (from n in dbContext.cobranzadetalle
                                         where n.v_IdCobranzaDetalle == cobranzadetalleDto.v_IdCobranzaDetalle
                                         select n).FirstOrDefault();

                            query.t_ActualizaFecha = DateTime.Now;
                            query.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            query.i_Eliminado = 1;

                            dbContext.SaveChanges();

                            if (query != null)
                            {
                                if (query.i_EsLetra == 0 || query.i_EsLetra == null)
                                {
                                    RecalcularSaldoVenta(ref pobjOperationResult, query.v_IdVenta, ClientSession, false);
                                    if (pobjOperationResult.Success == 0) return;
                                }
                                else
                                {
                                    RestauraDetalleCobranzaLetras(ref pobjOperationResult, query.v_IdCobranzaDetalle, ClientSession);
                                    if (pobjOperationResult.Success == 0) return;
                                }

                                if (query.i_IdTipoDocumentoRef != null &&
                                   query.i_IdTipoDocumentoRef.Value == 433 &&
                                   query.v_DocumentoRef.Contains('-'))
                                {
                                    var serieCorrelativo = query.v_DocumentoRef.Split('-');
                                    new AdelantoBL().RecalcularSaldoAdelanto(ref pobjOperationResult, serieCorrelativo[0],
                                        serieCorrelativo[1], 433);
                                    if (pobjOperationResult.Success == 0) return;
                                }
                            }

                            dbContext.cobranzadetalle.ApplyCurrentValues(query);
                            Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "cobranzadetalle", query.v_IdCobranzaDetalle);
                        }
                        dbContext.SaveChanges();
                        #endregion

                        #region Si es anulado restaura detalles

                        if (pobjDtoEntity.i_IdEstado == 0)
                        {
                            List<cobranzadetalle> objEntitySourceDetallesCobranza = (from a in dbContext.cobranzadetalle
                                                                                     where a.v_IdCobranza == pobjDtoEntity.v_IdCobranza
                                                                                     select a).ToList();

                            foreach (var RegistroCobranzaDetalle in objEntitySourceDetallesCobranza)
                            {
                                if (RegistroCobranzaDetalle.i_EsLetra == 0 || RegistroCobranzaDetalle.i_EsLetra == null)
                                {
                                    RecalcularSaldoVenta(ref pobjOperationResult, RegistroCobranzaDetalle.v_IdVenta, ClientSession, false);
                                    if (pobjOperationResult.Success == 0) return;
                                }
                                else
                                {
                                    RestauraDetalleCobranzaLetras(ref pobjOperationResult, RegistroCobranzaDetalle.v_IdCobranzaDetalle, ClientSession);
                                    if (pobjOperationResult.Success == 0) return;
                                }
                            }
                            EliminarTesoreria(ref pobjOperationResult, objEntitySource.v_IdCobranza);
                            if (pobjOperationResult.Success == 0) return;
                        }
                        else
                        {
                            RegenerarTesoreria(ref pobjOperationResult, objEntitySource.v_IdCobranza);
                            if (pobjOperationResult.Success == 0) return;
                        }
                        #endregion

                        dbContext.SaveChanges();
                        Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "cobranza", pobjDtoEntity.v_IdCobranza);
                        pobjOperationResult.Success = 1;
                        ts.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "CobranzaBL.ActualizarCobranza()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = pobjOperationResult.ExceptionMessage != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public void EliminarCobranza(ref OperationResult pobjOperationResult, string pstrIdCobranza, List<string> ClientSession)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {

                        #region Elimina Cabecera
                        // Obtener la entidad fuente
                        var objEntitySource = (from a in dbContext.cobranza
                                               where a.v_IdCobranza == pstrIdCobranza
                                               select a).FirstOrDefault();

                        // Crear la entidad con los datos actualizados
                        objEntitySource.t_ActualizaFecha = DateTime.Now;
                        objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntitySource.i_Eliminado = 1;
                        #endregion

                        #region Elimina Detalles
                        //Eliminar detalles del movimiento eliminado.
                        var objEntitySourceDetallesCobranza = (dbContext.cobranzadetalle.Where(
                            a => a.v_IdCobranza == pstrIdCobranza && a.i_Eliminado == 0)).ToList();

                        foreach (var RegistroCobranzaDetalle in objEntitySourceDetallesCobranza)
                        {
                            RegistroCobranzaDetalle.t_ActualizaFecha = DateTime.Now;
                            RegistroCobranzaDetalle.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            RegistroCobranzaDetalle.i_Eliminado = 1;
                            dbContext.cobranzadetalle.ApplyCurrentValues(RegistroCobranzaDetalle);
                            dbContext.SaveChanges();

                            if (RegistroCobranzaDetalle.i_EsLetra == null || RegistroCobranzaDetalle.i_EsLetra == 0)
                            {
                                RecalcularSaldoVenta(ref pobjOperationResult, RegistroCobranzaDetalle.v_IdVenta, ClientSession, false);
                                if (pobjOperationResult.Success == 0) return;
                            }
                            else
                            {
                                if (objEntitySource.i_IdEstado == 1) RestauraDetalleCobranzaLetras(ref pobjOperationResult, RegistroCobranzaDetalle.v_IdCobranzaDetalle, ClientSession);
                                if (pobjOperationResult.Success == 0) return;
                            }

                            if (RegistroCobranzaDetalle.i_IdTipoDocumentoRef != null &&
                                RegistroCobranzaDetalle.i_IdTipoDocumentoRef.Value == 433 &&
                                RegistroCobranzaDetalle.v_DocumentoRef.Contains('-'))
                            {
                                var serieCorrelativo = RegistroCobranzaDetalle.v_DocumentoRef.Split('-');
                                new AdelantoBL().RecalcularSaldoAdelanto(ref pobjOperationResult, serieCorrelativo[0],
                                    serieCorrelativo[1], 433);
                                if (pobjOperationResult.Success == 0) return;
                            }

                            Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "cobranzadetalle", RegistroCobranzaDetalle.v_IdCobranzaDetalle);
                        }
                        #endregion

                        #region Elimina Tesoreria
                        EliminarTesoreria(ref pobjOperationResult, pstrIdCobranza);
                        if (pobjOperationResult.Success == 0) return;
                        #endregion

                        dbContext.SaveChanges();
                        Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "cobranza", objEntitySource.v_IdCobranza);
                        pobjOperationResult.Success = 1;
                        ts.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "CobranzaBL.ActualizarCobranza()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public bool ExisteNroRegistro(string Periodo, string Mes, string Correlativo, int IdTipoDocumento)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                string replicationID = Globals.ClientSession.ReplicationNodeID;
                var Registro = (from n in dbContext.cobranza
                                where n.i_Eliminado == 0 && n.v_Periodo == Periodo && n.v_Mes == Mes && n.v_Correlativo == Correlativo
                                      && n.i_IdTipoDocumento == IdTipoDocumento && n.v_IdCobranza.Substring(0, 1) == replicationID
                                select n).FirstOrDefault();

                return Registro == null;
            }
        }

        public ventaDto ObtenerCobranzaPendientePorVenta(ref OperationResult pobjOperationResult, string pstrIdVenta)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    ventaDto _ventaDto = new ventaDto();

                    venta _Venta = (from n in dbContext.venta
                                    where n.v_IdVenta == pstrIdVenta
                                    select n
                                    ).FirstOrDefault();

                    _ventaDto = _Venta.ToDTO();

                    var Saldo = (from n in dbContext.cobranzapendiente
                                 where n.v_IdVenta == pstrIdVenta && n.i_Eliminado == 0
                                 select new { n.d_Saldo }).FirstOrDefault();

                    _ventaDto.SaldoPendiente = Saldo != null ? Saldo.d_Saldo.Value : 0;

                    pobjOperationResult.Success = 1;
                    return _ventaDto;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "CobranzaBL.ObtenerCobranzaPendientePorVenta()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = pobjOperationResult.ExceptionMessage != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public int DevuelveDocumento(ref OperationResult pobjOperationResult, int IdFormaPago)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var IdDocumento = (from n in dbContext.documento
                                       where n.i_IdFormaPago == IdFormaPago
                                       select new { n.i_CodigoDocumento }).FirstOrDefault();

                    if (IdDocumento != null)
                    {
                        pobjOperationResult.Success = 1;
                        int _Id = IdDocumento.i_CodigoDocumento;
                        return _Id;
                    }
                    else
                    {
                        return 0;
                    }
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "CobranzaBL.DevuelveDocumento()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = pobjOperationResult.ExceptionMessage != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return 0;
            }
        }

        public int DevuelveMedioPago(ref OperationResult pobjOperationResult, string NFormaPago)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var FormaPago = (from n in dbContext.datahierarchy
                                     where n.i_GroupId == 46 && n.v_Value1 == NFormaPago
                                     select new { n.v_Value2 }).FirstOrDefault();

                    var IdTipoPago = (from n in dbContext.datahierarchy
                                      where n.i_GroupId == 44 && n.v_Value2 == FormaPago.v_Value2
                                      select new { n.i_ItemId }).FirstOrDefault();

                    if (IdTipoPago != null)
                    {
                        pobjOperationResult.Success = 1;
                        int _Id = IdTipoPago.i_ItemId;
                        return _Id;
                    }
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

        public decimal DevuelveMontoNotaCredito(string nroDocumento)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var serieCorrelativo = nroDocumento.Split('-');
                    var serie = serieCorrelativo[0].Trim();
                    var correlativo = serieCorrelativo[1].Trim();
                    var ventaNrc = dbContext.venta.FirstOrDefault(p => p.v_SerieDocumento.Equals(serie) &&
                        p.v_CorrelativoDocumento.Equals(correlativo) && p.i_IdTipoDocumento.Value == 7 && p.i_Eliminado == 0);
                    if (ventaNrc == null) return 0;
                    {
                        var pendienteNrc = dbContext.cobranzapendiente
                            .FirstOrDefault(p => p.v_IdVenta == ventaNrc.v_IdVenta && p.i_Eliminado == 0);

                        if (pendienteNrc != null)
                        {
                            return pendienteNrc.d_Saldo ?? 0;
                        }
                        return 0;
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        public decimal DevuelveSaldoAdelanto(string nroDocumento)
        {
            try
            {
                var serieCorrelativo = nroDocumento.Split('-');
                var serie = serieCorrelativo[0].Trim();
                var correlativo = serieCorrelativo[1].Trim();

                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var monto = (dbContext.adelanto.Where(m => m.v_SerieDocumento == serie && m.v_CorrelativoDocumento == correlativo
                      && m.i_IdTipoDocumento == 433).Select(m => new { m.d_Saldo })).FirstOrDefault();

                    return monto != null ? monto.d_Saldo.Value : 0;
                }
            }
            catch (Exception ex)
            {
                var pobjOperationResult = new OperationResult
                {
                    Success = 0,
                    AdditionalInformation =
                        "CobranzaBL.InsertarCobranza()" + '\n' + "Linea: " +
                        ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')),
                    ErrorMessage = ex.Message
                };
                pobjOperationResult.ExceptionMessage = pobjOperationResult.ExceptionMessage != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return 0M;
            }
        }

        public decimal DevuelveValorRedondeado(decimal Valor)
        {
            return decimal.Parse(Math.Round(Valor, 1, MidpointRounding.AwayFromZero).ToString("0.00"));
        }

        public List<GridKeyValueDTO> ListadoFormasPago(int idDocumento = -1)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    if (idDocumento == -1)
                    {
                        var query = (dbContext.datahierarchy.Where(n => n.i_GroupId == 46 && n.i_IsDeleted == 0)
                            .OrderBy(n => n.i_Sort)).ToList();

                        var almacenes = (dbContext.almacen.Where(n => n.i_Eliminado == 0)
                            .Select(n => new { n.i_IdAlmacen, n.v_Nombre })).ToArray();

                        var result = query.Select(p =>
                        {
                            int id;
                            id = int.TryParse(p.v_Value4, out id) ? id : 0;
                            var almacen = almacenes.FirstOrDefault(r => r.i_IdAlmacen == id);
                            return new GridKeyValueDTO
                            {
                                Id = p.i_ItemId.ToString(),
                                Value1 = p.v_Value1 + (almacen == null ? string.Empty : "-" + almacen.v_Nombre),
                                Value2 = p.v_Value2,
                                Value3 = p.v_Field
                            };
                        }).ToList();

                        return result;
                    }
                    else
                    {
                        var almacenes = (dbContext.almacen.Where(n => n.i_Eliminado == 0)
                        .Select(n => new { n.i_IdAlmacen, n.v_Nombre })).ToArray();

                        var fp = dbContext.relacionformapagodocumento.Where(p => p.i_CodigoDocumento == idDocumento).Select(o => o.i_IdFormaPago);
                        var queryFormaPagos = (dbContext.datahierarchy.Where(n => n.i_GroupId == 46 && n.i_IsDeleted == 0 && fp.Contains(n.i_ItemId))
                                 .OrderBy(n => n.i_Sort)).ToList();

                        var queryComodines = (dbContext.datahierarchy.Where(n => n.i_GroupId == 46 && n.i_IsDeleted == 0 &&
                            (n.v_Value4 == "-1" || n.v_Value4 == null || n.v_Value4 == ""))
                                  .OrderBy(n => n.i_Sort)).ToList()
                                  .Select(o => new GridKeyValueDTO
                                  {
                                      Id = o.i_ItemId.ToString(),
                                      Value1 = o.v_Value1,
                                      Value2 = o.v_Value2,
                                      Value3 = o.v_Field
                                  }).ToList();

                        var result = queryFormaPagos.Select(p =>
                        {
                            int id;
                            id = int.TryParse(p.v_Value4, out id) ? id : 0;
                            var almacen = almacenes.FirstOrDefault(r => r.i_IdAlmacen == id);
                            return new GridKeyValueDTO
                            {
                                Id = p.i_ItemId.ToString(),
                                Value1 = p.v_Value1 + (almacen == null ? string.Empty : "-" + almacen.v_Nombre),
                                Value2 = p.v_Value2,
                                Value3 = p.v_Field
                            };
                        }).ToList();

                        return result.Concat(queryComodines).ToList();
                    }
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
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    TieneCuenta = true;
                    var documento = dbContext.documento.FirstOrDefault(p => p.i_CodigoDocumento == CodigoDocumento);
                    pobjOperationResult.Success = 1;

                    if (documento == null) return -1;
                    {
                        if (documento.v_NroCuenta != null && !string.IsNullOrEmpty(documento.v_NroCuenta.Trim()))
                        {
                            var cuenta = dbContext.asientocontable.FirstOrDefault(p => p.v_NroCuenta == documento.v_NroCuenta && p.v_Periodo == periodo && p.i_Eliminado == 0);

                            if (cuenta != null)
                            {
                                return cuenta.i_IdMoneda ?? -1;
                            }
                            TieneCuenta = false;
                            return -1;
                        }
                        TieneCuenta = false;
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                TieneCuenta = true;
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "CobranzaBL.DevuelveMonedaPorDocumento()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = pobjOperationResult.ExceptionMessage != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return -1;
            }
        }

        public void DisminuyeLineaCredito(ref OperationResult pobjOperationResult, string IdVenta, cobranzadetalleDto _cobranzadetalleDto, int IdMonedaCobranza)
        {
            try
            {
                DocumentoBL _objDocumentoBL = new DocumentoBL();
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {

                        #region Actualiza Linea de Crédito del Cliente
                        var Cliente = (from n in dbContext.venta
                                       join c in dbContext.cliente on n.v_IdCliente equals c.v_IdCliente into c_join
                                       from c in c_join.DefaultIfEmpty()
                                       where n.v_IdVenta == IdVenta
                                       select c).FirstOrDefault();

                        var Venta = dbContext.venta.Where(p => p.v_IdVenta == IdVenta).FirstOrDefault();

                        //if (Venta.i_IdTipoDocumento != 7)
                        if (!_objDocumentoBL.DocumentoEsInverso(Venta.i_IdTipoDocumento.Value))
                        {
                            if (Cliente != null)
                            {
                                if (Cliente.i_UsaLineaCredito != null && Cliente.i_UsaLineaCredito == 1)
                                {
                                    var LineaCredito = dbContext.lineacreditoempresa.Where(p => p.v_IdCliente == Cliente.v_IdCliente).FirstOrDefault();

                                    if (LineaCredito != null)
                                    {
                                        switch (LineaCredito.i_IdMoneda)
                                        {
                                            case 1:
                                                switch (IdMonedaCobranza)
                                                {
                                                    case 1:
                                                        LineaCredito.d_Acuenta = LineaCredito.d_Acuenta.Value - _cobranzadetalleDto.d_ImporteSoles.Value;
                                                        LineaCredito.d_Saldo = LineaCredito.d_Credito.Value - LineaCredito.d_Acuenta.Value;
                                                        break;

                                                    case 2:
                                                        LineaCredito.d_Acuenta = LineaCredito.d_Acuenta.Value - (_cobranzadetalleDto.d_ImporteSoles.Value * Venta.d_TipoCambio.Value);
                                                        LineaCredito.d_Saldo = LineaCredito.d_Credito.Value - LineaCredito.d_Acuenta.Value;
                                                        break;
                                                }
                                                break;

                                            case 2:
                                                switch (IdMonedaCobranza)
                                                {
                                                    case 1:
                                                        LineaCredito.d_Acuenta = LineaCredito.d_Acuenta.Value - (_cobranzadetalleDto.d_ImporteSoles.Value / Venta.d_TipoCambio.Value);
                                                        LineaCredito.d_Saldo = LineaCredito.d_Credito.Value - LineaCredito.d_Acuenta.Value;
                                                        break;

                                                    case 2:
                                                        LineaCredito.d_Acuenta = LineaCredito.d_Acuenta.Value - _cobranzadetalleDto.d_ImporteSoles.Value;
                                                        LineaCredito.d_Saldo = LineaCredito.d_Credito.Value - LineaCredito.d_Acuenta.Value;
                                                        break;
                                                }
                                                break;
                                        }

                                        dbContext.lineacreditoempresa.ApplyCurrentValues(LineaCredito);
                                    }
                                }
                            }

                            dbContext.SaveChanges();
                            Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "lineacreditoempresa", "");
                        }
                        #endregion

                        pobjOperationResult.Success = 1;
                        ts.Complete();
                    }
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "VentaBL.InsertarCobranzaPendiente()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }

        public void RestauraLineaCredito(ref OperationResult pobjOperationResult, string IdVenta, cobranzadetalleDto _cobranzadetalleDto, int IdMonedaCobranza)
        {
            try
            {
                var _objDocumentoBL = new DocumentoBL();
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {

                        #region Actualiza Linea de Crédito del Cliente
                        var Cliente = (from n in dbContext.venta
                                       join c in dbContext.cliente on n.v_IdCliente equals c.v_IdCliente into c_join
                                       from c in c_join.DefaultIfEmpty()
                                       where n.v_IdVenta == IdVenta
                                       select c).FirstOrDefault();

                        var Venta = dbContext.venta.Where(p => p.v_IdVenta == IdVenta).FirstOrDefault();

                        //if (Venta.i_IdTipoDocumento != 7)
                        if (!_objDocumentoBL.DocumentoEsInverso(Venta.i_IdTipoDocumento.Value))
                        {
                            if (Cliente != null)
                            {
                                if (Cliente.i_UsaLineaCredito != null && Cliente.i_UsaLineaCredito == 1)
                                {
                                    var LineaCredito = dbContext.lineacreditoempresa.Where(p => p.v_IdCliente == Cliente.v_IdCliente).FirstOrDefault();

                                    if (LineaCredito != null)
                                    {
                                        switch (LineaCredito.i_IdMoneda)
                                        {
                                            case 1:
                                                switch (IdMonedaCobranza)
                                                {
                                                    case 1:
                                                        LineaCredito.d_Acuenta = LineaCredito.d_Acuenta.Value + _cobranzadetalleDto.d_ImporteSoles.Value;
                                                        LineaCredito.d_Saldo = LineaCredito.d_Credito.Value - LineaCredito.d_Acuenta.Value;
                                                        break;

                                                    case 2:
                                                        LineaCredito.d_Acuenta = LineaCredito.d_Acuenta.Value + (_cobranzadetalleDto.d_ImporteSoles.Value * Venta.d_TipoCambio.Value);
                                                        LineaCredito.d_Saldo = LineaCredito.d_Credito.Value - LineaCredito.d_Acuenta.Value;
                                                        break;
                                                }
                                                break;

                                            case 2:
                                                switch (IdMonedaCobranza)
                                                {
                                                    case 1:
                                                        LineaCredito.d_Acuenta = LineaCredito.d_Acuenta.Value + (_cobranzadetalleDto.d_ImporteSoles.Value / Venta.d_TipoCambio.Value);
                                                        LineaCredito.d_Saldo = LineaCredito.d_Credito.Value - LineaCredito.d_Acuenta.Value;
                                                        break;

                                                    case 2:
                                                        LineaCredito.d_Acuenta = LineaCredito.d_Acuenta.Value + _cobranzadetalleDto.d_ImporteSoles.Value;
                                                        LineaCredito.d_Saldo = LineaCredito.d_Credito.Value - LineaCredito.d_Acuenta.Value;
                                                        break;
                                                }
                                                break;
                                        }

                                        dbContext.lineacreditoempresa.ApplyCurrentValues(LineaCredito);
                                    }
                                }
                            }

                            dbContext.SaveChanges();
                            Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "lineacreditoempresa", "");
                        }
                        #endregion

                        pobjOperationResult.Success = 1;
                        ts.Complete();
                    }
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "VentaBL.InsertarCobranzaPendiente()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        /// <summary>
        /// Método utilizado en las ventas al contado para realizar las cobranzas automáticas, por el mismo hecho de ser al contado.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pintIdMoneda">Id de la moneda de la cobranza</param>
        /// <param name="pintIdFormaPago">Id de la forma de pago, ya se en efectivo, mastercard, etc..</param>
        /// <param name="pstrNroOperacion">Nro de la operación si la cobranza se realizó con POS</param>
        /// <param name="pdecMontoCobrar">El monto a cobrar.</param>
        /// <param name="pstrIdVenta">El id de la venta para enlazar la cobranza con la venta.</param>
        public void RealizaCobranzaAlContado(ref OperationResult pobjOperationResult, int pintIdMoneda,
            int pintIdFormaPago, string pstrNroOperacion, decimal pdecMontoCobrar, string pstrIdVenta, DateTime pdateFechaVenta, decimal pdecTipoCambio)
        {
            try
            {
                if (pdecMontoCobrar == 0) return;
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var cobranzaCabecera = new cobranzaDto();
                    var cobranzaDetalleLista = new List<cobranzadetalleDto>();
                    cobranzaCabecera.d_TipoCambio = pdecTipoCambio;
                    cobranzaCabecera.d_TotalSoles = pdecMontoCobrar;
                    cobranzaCabecera.t_FechaRegistro = pdateFechaVenta;
                    cobranzaCabecera.i_IdMoneda = pintIdMoneda;
                    cobranzaCabecera.i_IdEstado = 1;
                    cobranzaCabecera.i_IdTipoDocumento = dbContext.relacionformapagodocumento.FirstOrDefault(p => p.i_IdFormaPago == pintIdFormaPago).i_CodigoDocumento.Value;
                    cobranzaCabecera.i_IdMedioPago = DevuelveMedioPago(ref pobjOperationResult, dbContext.datahierarchy.FirstOrDefault(p => p.i_GroupId == 46 && p.i_ItemId == pintIdFormaPago).v_Value1);
                    cobranzaCabecera.v_Periodo = Globals.ClientSession.i_Periodo.ToString();
                    cobranzaCabecera.v_Mes = pdateFechaVenta.Month.ToString("00");
                    cobranzaCabecera.v_Correlativo = Utils.Windows.RetornaCorrelativoPorFecha(ref pobjOperationResult, ListaProcesos.Cobranza, null, pdateFechaVenta, null, cobranzaCabecera.i_IdTipoDocumento.Value);
                    cobranzaCabecera.v_Glosa = "Cobranza del día " + pdateFechaVenta.ToShortDateString();

                    while (!ExisteNroRegistro(cobranzaCabecera.v_Periodo, cobranzaCabecera.v_Mes, cobranzaCabecera.v_Correlativo, cobranzaCabecera.i_IdTipoDocumento.Value))
                    {
                        cobranzaCabecera.v_Correlativo = (int.Parse(cobranzaCabecera.v_Correlativo) + 1).ToString("00000000");
                    }

                    int idDocRef;
                    cobranzaDetalleLista.Add(new cobranzadetalleDto
                    {
                        d_ImporteSoles = pdecMontoCobrar,
                        d_NetoXCobrar = pdecMontoCobrar,
                        i_EsLetra = 0,
                        i_IdTipoDocumentoRef = int.TryParse(dbContext.datahierarchy.FirstOrDefault(p => p.i_GroupId == 46 && p.i_ItemId == pintIdFormaPago).v_Field, out idDocRef) ? idDocRef : -1,
                        v_DocumentoRef = pstrNroOperacion,
                        v_IdVenta = pstrIdVenta,
                        i_IdFormaPago = pintIdFormaPago
                    });

                    InsertarCobranza(ref pobjOperationResult, cobranzaCabecera, Globals.ClientSession.GetAsList(), cobranzaDetalleLista);
                    if (pobjOperationResult.Success == 0) return;

                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "VentaBL.RealizaCobranzaAlContado()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        #region Procesos
        private void RestauraDetalleCobranza(ref OperationResult pobjOperationResult, string pstrIdCobranzaDetalle, List<string> ClientSession)
        {
            try
            {
                DocumentoBL _objDocumentoBL = new DocumentoBL();
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                    List<cobranzapendiente> cps = new List<cobranzapendiente>();

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
                        // if (cobranzadetalleDto.i_IdTipoDocumentoRef != 7) //Si la cobranza no fue pagada con una nota de credito solo se procede a recoger la cobranza de la venta.

                        if (!_objDocumentoBL.DocumentoEsInverso(cobranzadetalleDto.i_IdTipoDocumentoRef.Value)) //Si la cobranza no fue pagada con una nota de credito solo se procede a recoger la cobranza de la venta.
                        {
                            cps = (from n in dbContext.cobranzapendiente
                                   where n.v_IdVenta == cobranzadetalleDto.v_IdVenta && n.i_Eliminado == 0
                                   select n).ToList();
                        }
                        else
                        {
                            //si la cobranza fue hecha en parte con una nota de credito se procede a incluir a la NRC en la lista de ventas por cancelar.
                            string[] SerieCorrelativo = cobranzadetalleDto.v_DocumentoRef.Split(new Char[] { '-' });
                            string Serie = SerieCorrelativo[0].Trim();
                            string Correlativo = SerieCorrelativo[1].Trim();

                            // var VentaNRC = dbContext.venta.Where(p => p.i_IdTipoDocumento == 7 && p.v_CorrelativoDocumento == Correlativo && p.v_SerieDocumento == Serie).FirstOrDefault();
                            var VentaNRC = dbContext.venta.Where(p => _objDocumentoBL.DocumentoEsInverso(p.i_IdTipoDocumento.Value) && p.v_CorrelativoDocumento == Correlativo && p.v_SerieDocumento == Serie).FirstOrDefault();

                            if (VentaNRC != null)
                            {
                                cps = (from n in dbContext.cobranzapendiente
                                       where (n.v_IdVenta == cobranzadetalleDto.v_IdVenta || n.v_IdVenta == VentaNRC.v_IdVenta) && n.i_Eliminado == 0
                                       select n).ToList();
                            }
                            else
                            {
                                pobjOperationResult.Success = 0;
                                pobjOperationResult.ErrorMessage = "No se encontró la nota de crédito";
                                pobjOperationResult.AdditionalInformation = "CobranzaBL.ProcesaDetalleCobranza()";
                                return;
                            }
                        }
                        #endregion

                        if (cps != null)
                        {
                            #region Se procede a recorrer la lista de cobranzas a cancelar
                            foreach (var cp in cps)
                            {
                                int Moneda = (from m in dbContext.venta
                                              where m.v_IdVenta == cp.v_IdVenta && m.i_Eliminado == 0
                                              select new { m.i_IdMoneda }).FirstOrDefault().i_IdMoneda.Value;

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

                                RestauraLineaCredito(ref pobjOperationResult, cp.v_IdVenta, cobranzadetalleAssembler.ToDTO(cobranzadetalleDto), CobranzaEntity.i_IdMoneda.Value);
                                if (pobjOperationResult.Success == 0) return;
                                Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "cobranzapendiente", cp.v_IdCobranzaPendiente);
                            }
                            #endregion
                        }
                        else
                        {
                            pobjOperationResult.Success = 0;
                            pobjOperationResult.ErrorMessage = "No se encontró la cobranza pendiente";
                            pobjOperationResult.AdditionalInformation = "CobranzaBL.ProcesaDetalleCobranza()";
                            Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                        }
                    }
                    else
                    {
                        pobjOperationResult.Success = 0;
                        pobjOperationResult.ErrorMessage = "No se encontró la cobranza";
                        pobjOperationResult.AdditionalInformation = "CobranzaBL.RestauraDetalleCobranza()";
                        Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                    }

                    #region Actualizar Saldo de Adelanto
                    if (cobranzadetalleDto.i_IdTipoDocumentoRef.Value == 433) //se iguala al id del documento para adelantos.
                    {
                        string[] SerieCorrelativo = cobranzadetalleDto.v_DocumentoRef.Split(new Char[] { '-' });
                        string Serie = SerieCorrelativo[0].Trim();
                        string Correlativo = SerieCorrelativo[1].Trim();

                        adelanto _adelanto = (from a in dbContext.adelanto
                                              where a.v_SerieDocumento == Serie && a.v_CorrelativoDocumento == Correlativo
                                              select a).FirstOrDefault();

                        _adelanto.d_Consumo = _adelanto.d_Consumo != null ? _adelanto.d_Consumo.Value - cobranzadetalleDto.d_ImporteSoles : 0;
                        _adelanto.d_Saldo = _adelanto.d_Saldo.Value + cobranzadetalleDto.d_ImporteSoles;
                        dbContext.adelanto.ApplyCurrentValues(_adelanto);
                        Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "adelanto", _adelanto.v_IdAdelanto);
                    }
                    #endregion

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "CobranzaBL.RestauraDetalleCobranza()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = pobjOperationResult.ExceptionMessage != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }

        }

        private void ProcesaDetalleCobranza(ref OperationResult pobjOperationResult, cobranzadetalleDto cobranzadetalleDto, cobranzaDto CobranzaEntity, List<string> ClientSession)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    List<cobranzapendiente> cps = new List<cobranzapendiente>();

                    #region Obtiene el Listado de cobranzas a realizar
                    //if (cobranzadetalleDto.i_IdTipoDocumentoRef != 7) //Si la cobranza no fue pagada con una nota de credito solo se procede a recoger la cobranza de la venta.
                    if (!_objDocumentoBL.DocumentoEsInverso(cobranzadetalleDto.i_IdTipoDocumentoRef.Value)) //Si la cobranza no fue pagada con una nota de credito solo se procede a recoger la cobranza de la venta.
                    {
                        cps = (from n in dbContext.cobranzapendiente
                               where n.v_IdVenta == cobranzadetalleDto.v_IdVenta && n.i_Eliminado == 0
                               select n).ToList();
                    }
                    else
                    {
                        //si la cobranza fue hecha en parte con una nota de credito se procede a incluir a la NRC en la lista de ventas por cancelar.
                        string[] SerieCorrelativo = cobranzadetalleDto.v_DocumentoRef.Split(new Char[] { '-' });
                        string Serie = SerieCorrelativo[0].Trim();
                        string Correlativo = SerieCorrelativo[1].Trim();
                        //var VentaNRC = dbContext.venta.Where(p => p.i_IdTipoDocumento == 7 && p.v_CorrelativoDocumento == Correlativo && p.v_SerieDocumento == Serie).FirstOrDefault();
                        var VentaNRC = dbContext.venta.Where(p => p.v_CorrelativoDocumento == Correlativo && p.v_SerieDocumento == Serie && p.i_Eliminado == 0).ToList().FirstOrDefault(x => _objDocumentoBL.DocumentoEsInverso(x.i_IdTipoDocumento.Value));


                        if (VentaNRC != null)
                        {
                            cps = (from n in dbContext.cobranzapendiente
                                   where (n.v_IdVenta == cobranzadetalleDto.v_IdVenta || n.v_IdVenta == VentaNRC.v_IdVenta) && n.i_Eliminado == 0
                                   select n).ToList();
                        }
                        else
                        {
                            pobjOperationResult.Success = 0;
                            pobjOperationResult.ErrorMessage = "No se encontró la nota de crédito";
                            pobjOperationResult.AdditionalInformation = "CobranzaBL.ProcesaDetalleCobranza()";
                            return;
                        }
                    }
                    #endregion

                    if (cps != null)
                    {
                        #region Se procede a recorrer la lista de cobranzas a cancelar
                        foreach (var cp in cps)
                        {
                            int Moneda = (from m in dbContext.venta
                                          where m.v_IdVenta == cobranzadetalleDto.v_IdVenta && m.i_Eliminado == 0
                                          select new { m.i_IdMoneda }).FirstOrDefault().i_IdMoneda.Value;

                            decimal TCambio = CobranzaEntity.d_TipoCambio ?? 0;
                            //decimal importe = (cobranzadetalleDto.d_ImporteSoles ?? 0) + (cobranzadetalleDto.d_MontoRetencion ?? 0);
                            decimal importe = (cobranzadetalleDto.d_ImporteSoles ?? 0) + (cobranzadetalleDto.d_MontoRetencion ?? 0) + (cobranzadetalleDto.d_Redondeo ??0); //se agrego por REDONDEOCOBRANZA
                            switch (CobranzaEntity.i_IdMoneda)
                            {
                                case 1:
                                    switch (Moneda)
                                    {
                                        case 1:
                                            cp.d_Acuenta = cp.d_Acuenta.Value + importe;
                                            cp.d_Saldo = cp.d_Saldo.Value - importe >= 0 ? cp.d_Saldo.Value - importe : 0;

                                            break;

                                        case 2:
                                            cp.d_Acuenta = cp.d_Acuenta.Value + (importe / TCambio);
                                            cp.d_Saldo = cp.d_Saldo.Value - (importe / TCambio) >= 0 ? cp.d_Saldo.Value - (importe / TCambio) : 0;

                                            break;
                                    }
                                    break;

                                case 2:
                                    switch (Moneda)
                                    {
                                        case 1:
                                            cp.d_Acuenta = cp.d_Acuenta.Value + (importe * TCambio);
                                            cp.d_Saldo = cp.d_Saldo.Value - (importe * TCambio) >= 0 ? cp.d_Saldo.Value - (importe * TCambio) : 0;

                                            break;

                                        case 2:
                                            cp.d_Acuenta = cp.d_Acuenta.Value + importe;
                                            cp.d_Saldo = cp.d_Saldo.Value - importe >= 0 ? cp.d_Saldo.Value - importe : 0;

                                            break;
                                    }
                                    break;
                            }

                            dbContext.cobranzapendiente.ApplyCurrentValues(cp);

                            DisminuyeLineaCredito(ref pobjOperationResult, cp.v_IdVenta, cobranzadetalleDto, CobranzaEntity.i_IdMoneda.Value);
                            if (pobjOperationResult.Success == 0) return;
                            Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "cobranzapendiente", cp.v_IdCobranzaPendiente);
                        }
                        #endregion

                        #region Actualizar Saldo de Adelanto
                        if (cobranzadetalleDto.i_IdTipoDocumentoRef.Value == 433) //se iguala al id del documento para adelantos.
                        {
                            string[] SerieCorrelativo = cobranzadetalleDto.v_DocumentoRef.Split(new Char[] { '-' });
                            string Serie = SerieCorrelativo[0].Trim();
                            string Correlativo = SerieCorrelativo[1].Trim();

                            adelanto _adelanto = (from a in dbContext.adelanto
                                                  where a.v_SerieDocumento == Serie && a.v_CorrelativoDocumento == Correlativo
                                                  select a).FirstOrDefault();

                            _adelanto.d_Consumo = _adelanto.d_Consumo != null ? _adelanto.d_Consumo.Value + cobranzadetalleDto.d_ImporteSoles : 0;
                            _adelanto.d_Saldo = _adelanto.d_Saldo.Value - cobranzadetalleDto.d_ImporteSoles;
                            dbContext.adelanto.ApplyCurrentValues(_adelanto);
                            Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "adelanto", _adelanto.v_IdAdelanto);
                        }
                        #endregion
                    }
                    else
                    {
                        pobjOperationResult.Success = 0;
                        pobjOperationResult.ErrorMessage = "No se encontró la cobranza pendiente";
                        pobjOperationResult.AdditionalInformation = "CobranzaBL.ProcesaDetalleCobranza()";
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
                pobjOperationResult.AdditionalInformation = "CobranzaBL.ProcesaDetalleCobranza()";
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }

        }

        private void RestauraDetalleCobranzaLetras(ref OperationResult pobjOperationResult, string pstrIdCobranzaDetalle, List<string> ClientSession)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var cobranzaD =
                            dbContext.cobranzadetalle.FirstOrDefault(
                                p => p.v_IdCobranzaDetalle.Equals(pstrIdCobranzaDetalle));

                        if (cobranzaD != null)
                        {
                            var idLetra = cobranzaD.v_IdVenta;
                            var objLetra = dbContext.letrasdetalle.FirstOrDefault(p => p.v_IdLetrasDetalle == idLetra && p.i_Eliminado == 0);

                            if (objLetra != null && !EsLetraDescuento(objLetra.v_IdLetrasDetalle, false))
                            {
                                var objPendiente = dbContext.cobranzaletraspendiente.FirstOrDefault(p => p.v_IdLetrasDetalle == idLetra && p.i_Eliminado == 0);
                                IEnumerable<cobranzadetalle> objCobranzas = dbContext.cobranzadetalle.Where(p => p.v_IdVenta == idLetra && p.i_Eliminado == 0).ToList();
                                IEnumerable<letrascanje> objCanjes = dbContext.letrascanje.Where(p => p.v_IdVenta == idLetra && p.i_Eliminado == 0).ToList();

                                objPendiente.d_Saldo = objLetra.d_Importe ?? 0;
                                objPendiente.d_Acuenta = 0;
                                dbContext.SaveChanges();

                                foreach (var cobranza in objCobranzas.Where(p => p.cobranza.i_IdEstado == 1))
                                {
                                    ProcesaDetalleCobranzaLetras(ref pobjOperationResult, cobranza.ToDTO(), cobranza.cobranza.ToDTO(), ClientSession);
                                    if (pobjOperationResult.Success == 0) return;
                                }

                                foreach (var Canje in objCanjes)
                                {
                                    ProcesarCanjeLetra(ref pobjOperationResult, Canje.letras, Canje, idLetra);
                                    if (pobjOperationResult.Success == 0) return;
                                }
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
                pobjOperationResult.AdditionalInformation = "CobranzaBL.RestauraDetalleCobranzaLetras()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = pobjOperationResult.ExceptionMessage != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }

        }

        private void ProcesaDetalleCobranzaLetras(ref OperationResult pobjOperationResult, cobranzadetalleDto cobranzadetalleDto, cobranzaDto CobranzaEntity, List<string> ClientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var cps = new List<cobranzaletraspendiente>();

                    #region Obtiene el Listado de cobranzas a realizar
                    cps = (from n in dbContext.cobranzaletraspendiente
                           where n.v_IdLetrasDetalle == cobranzadetalleDto.v_IdVenta && n.i_Eliminado == 0
                           select n).ToList();
                    #endregion

                    if (cps.Any())
                    {
                        #region Se procede a recorrer la lista de cobranzas a cancelar
                        foreach (var cp in cps)
                        {
                            if (EsLetraDescuento(cp.v_IdLetrasDetalle, false)) continue;
                            var firstOrDefault = dbContext.letrasdetalle.FirstOrDefault(p => p.v_IdLetrasDetalle == cp.v_IdLetrasDetalle && p.i_Eliminado == 0);
                            if (firstOrDefault != null)
                            {
                                int Moneda = firstOrDefault.i_IdMoneda ?? 1;

                                decimal TCambio = CobranzaEntity.d_TipoCambio ?? 1M;
                                decimal Importe = cobranzadetalleDto.d_ImporteSoles.Value + cobranzadetalleDto.d_Redondeo ?? 0;
                                switch (CobranzaEntity.i_IdMoneda)
                                {
                                    case 1:
                                        switch (Moneda)
                                        {
                                            case 1:
                                                //cp.d_Acuenta = cp.d_Acuenta.Value + cobranzadetalleDto.d_ImporteSoles.Value;
                                                //cp.d_Saldo = cp.d_Saldo.Value - cobranzadetalleDto.d_ImporteSoles.Value >= 0 ? cp.d_Saldo.Value - cobranzadetalleDto.d_ImporteSoles.Value : 0;
                                                cp.d_Acuenta = cp.d_Acuenta.Value + Importe;
                                                cp.d_Saldo = cp.d_Saldo.Value -Importe >= 0 ? cp.d_Saldo.Value - Importe : 0;

                                                break;

                                            case 2:
                                                //cp.d_Acuenta = cp.d_Acuenta.Value + (cobranzadetalleDto.d_ImporteSoles.Value / TCambio);
                                                //cp.d_Saldo = cp.d_Saldo.Value - (cobranzadetalleDto.d_ImporteSoles.Value / TCambio) >= 0 ? cp.d_Saldo.Value - (cobranzadetalleDto.d_ImporteSoles.Value / TCambio) : 0;
                                                cp.d_Acuenta = cp.d_Acuenta.Value + (Importe / TCambio);
                                                cp.d_Saldo = cp.d_Saldo.Value - (Importe / TCambio) >= 0 ? cp.d_Saldo.Value - (Importe / TCambio) : 0;
                                                break;
                                        }
                                        break;

                                    case 2:
                                        switch (Moneda)
                                        {
                                            case 1:
                                                //cp.d_Acuenta = cp.d_Acuenta.Value + (cobranzadetalleDto.d_ImporteSoles.Value * TCambio);
                                                //cp.d_Saldo = cp.d_Saldo.Value - (cobranzadetalleDto.d_ImporteSoles.Value * TCambio) >= 0 ? cp.d_Saldo.Value - (cobranzadetalleDto.d_ImporteSoles.Value * TCambio) : 0;

                                                cp.d_Acuenta = cp.d_Acuenta.Value + (Importe * TCambio);
                                                cp.d_Saldo = cp.d_Saldo.Value - (Importe * TCambio) >= 0 ? cp.d_Saldo.Value - (Importe * TCambio) : 0;
                                                break;

                                            case 2:
                                                //cp.d_Acuenta = cp.d_Acuenta.Value + cobranzadetalleDto.d_ImporteSoles.Value;
                                                //cp.d_Saldo = cp.d_Saldo.Value - cobranzadetalleDto.d_ImporteSoles.Value >= 0 ? cp.d_Saldo.Value - cobranzadetalleDto.d_ImporteSoles.Value : 0;

                                                cp.d_Acuenta = cp.d_Acuenta.Value + Importe;
                                                cp.d_Saldo = cp.d_Saldo.Value - Importe >= 0 ? cp.d_Saldo.Value - Importe : 0;

                                                break;
                                        }
                                        break;
                                }
                            }

                            if (cp.d_Saldo == 0) cp.letrasdetalle.i_Pagada = 1;
                            else cp.letrasdetalle.i_Pagada = 0;

                            dbContext.cobranzaletraspendiente.ApplyCurrentValues(cp);

                            Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "cobranzapendiente", cp.v_IdLetrasDetalle);
                        }
                        #endregion
                    }
                    else
                    {
                        pobjOperationResult.Success = 0;
                        pobjOperationResult.ErrorMessage = "No se encontró la cobranza pendiente";
                        pobjOperationResult.AdditionalInformation = "CobranzaBL.ProcesaDetalleCobranzaLetras()";
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
                pobjOperationResult.AdditionalInformation = "CobranzaBL.ProcesaDetalleCobranzaLetras()";
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }

        }

        /// <summary>
        /// Recalcula el saldo de la venta, recibe como parametro el id de la venta. Este método se realizó para los casos en que se elimina o edita una cobranza.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstrIdVenta"></param>
        public void RecalcularSaldoVenta(ref OperationResult pobjOperationResult, string pstrIdVenta, List<string> ClientSession, bool EsLetra, bool recalcularReferencia = false)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var objVenta = dbContext.venta.FirstOrDefault(p => p.v_IdVenta == pstrIdVenta && p.i_Eliminado == 0);

                        if (objVenta != null && objVenta.i_IdEstado == 1)
                        {
                            #region Si se requiere recalcular al documento de referencia se aplica esta seccion.
                            if (_objDocumentoBL.DocumentoEsInverso(objVenta.i_IdTipoDocumento ?? -1) && recalcularReferencia)
                            {
                                objVenta = dbContext.venta.FirstOrDefault(p => p.i_Eliminado == 0 && p.i_IdTipoDocumento == objVenta.i_IdTipoDocumentoRef && p.v_SerieDocumento.Equals(objVenta.v_SerieDocumentoRef.Trim())
                                    && p.v_CorrelativoDocumento.Equals(objVenta.v_CorrelativoDocumentoRef.Trim()) && p.i_IdEstado == 1 && p.v_IdCliente.Equals(objVenta.v_IdCliente));

                                if (objVenta == null) return;
                                pstrIdVenta = objVenta.v_IdVenta;
                            }
                            #endregion

                            var objPendiente = dbContext.cobranzapendiente.FirstOrDefault(p => p.v_IdVenta == pstrIdVenta && p.i_Eliminado == 0);
                            IEnumerable<cobranzadetalle> objCobranzas = dbContext.cobranzadetalle.Where(p => p.v_IdVenta == pstrIdVenta && p.i_Eliminado == 0).ToList();
                            IEnumerable<letrascanje> objCanjes = dbContext.letrascanje.Where(p => p.v_IdVenta == pstrIdVenta && p.i_Eliminado == 0).ToList();
                            var objNotasCredito = dbContext.venta.Where(p => p.v_SerieDocumentoRef.Trim() == objVenta.v_SerieDocumento.Trim()
                                && p.v_CorrelativoDocumentoRef.Trim() == objVenta.v_CorrelativoDocumento.Trim()
                                && p.i_Eliminado == 0).ToList().Where(o => _objDocumentoBL.DocumentoEsInverso(o.i_IdTipoDocumento ?? -1));

                            if (objPendiente != null)
                            {
                                objPendiente.d_Saldo = objVenta.d_Total ?? 0;
                                objPendiente.d_Acuenta = 0;
                            }
                            dbContext.SaveChanges();

                            foreach (var cobranza in objCobranzas.Where(p => p.cobranza.i_IdEstado == 1))
                            {
                                ProcesaDetalleCobranza(ref pobjOperationResult, cobranza.ToDTO(), cobranza.cobranza.ToDTO(), ClientSession);
                                if (pobjOperationResult.Success == 0) return;
                            }

                            foreach (var canje in objCanjes)
                            {
                                ProcesarCanjeLetra(ref pobjOperationResult, canje.letras, canje, pstrIdVenta);
                                if (pobjOperationResult.Success == 0) return;
                            }

                            foreach (var ventaNcr in objNotasCredito)
                            {
                                new VentaBL().EliminarCobranzaPendiente(ref pobjOperationResult, ventaNcr.v_IdVenta, Globals.ClientSession.GetAsList(), ventaNcr.i_IdEstado ?? 0, ventaNcr.i_Eliminado ?? 0);
                                if (pobjOperationResult.Success == 0) return;
                                new VentaBL().InsertarCobranzaPendiente(ref pobjOperationResult, ventaNcr.v_IdVenta, ventaNcr.d_Total ?? 0, ClientSession);
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
                pobjOperationResult.AdditionalInformation = "CobranzaBL.RecalcularSaldoVenta()";
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        void ProcesarCanjeLetra(ref OperationResult pobjOperationResult, letras CobranzaEntity, letrascanje cobranzadetalleDto, string IdVenta)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region Actualiza la deuda de una venta

                    var cps = dbContext.cobranzapendiente.Where(p => p.v_IdVenta == IdVenta && p.i_Eliminado == 0).ToList();

                    if (cps != null)
                    {
                        #region Se procede a recorrer la lista de cobranzas a cancelar
                        foreach (var cp in cps)
                        {
                            var VentaOriginal = (from m in dbContext.venta
                                                 where m.v_IdVenta == cp.v_IdVenta && m.i_Eliminado == 0
                                                 select new { m.i_IdMoneda, m.d_TipoCambio }).FirstOrDefault();

                            int Moneda = VentaOriginal.i_IdMoneda.Value;

                            decimal TCambio = VentaOriginal.d_TipoCambio.Value;

                            switch (CobranzaEntity.i_IdMoneda)
                            {
                                case 1:
                                    switch (Moneda)
                                    {
                                        case 1:
                                            cp.d_Acuenta = cp.d_Acuenta.Value + cobranzadetalleDto.d_MontoCanjeado;
                                            cp.d_Saldo = cp.d_Saldo.Value - cobranzadetalleDto.d_MontoCanjeado >= 0 ? cp.d_Saldo.Value - cobranzadetalleDto.d_MontoCanjeado : 0;

                                            break;

                                        case 2:
                                            cp.d_Acuenta = cp.d_Acuenta.Value + (cobranzadetalleDto.d_MontoCanjeado / TCambio);
                                            cp.d_Saldo = cp.d_Saldo.Value - (cobranzadetalleDto.d_MontoCanjeado / TCambio) >= 0 ? cp.d_Saldo.Value - (cobranzadetalleDto.d_MontoCanjeado / TCambio) : 0;

                                            break;
                                    }
                                    break;

                                case 2:
                                    switch (Moneda)
                                    {
                                        case 1:
                                            cp.d_Acuenta = cp.d_Acuenta.Value + (cobranzadetalleDto.d_MontoCanjeado * TCambio);
                                            cp.d_Saldo = cp.d_Saldo.Value - (cobranzadetalleDto.d_MontoCanjeado * TCambio) >= 0 ? cp.d_Saldo.Value - (cobranzadetalleDto.d_MontoCanjeado * TCambio) : 0;

                                            break;

                                        case 2:
                                            cp.d_Acuenta = cp.d_Acuenta.Value + cobranzadetalleDto.d_MontoCanjeado;
                                            cp.d_Saldo = cp.d_Saldo.Value - cobranzadetalleDto.d_MontoCanjeado >= 0 ? cp.d_Saldo.Value - cobranzadetalleDto.d_MontoCanjeado : 0;

                                            break;
                                    }
                                    break;
                            }

                            dbContext.cobranzapendiente.ApplyCurrentValues(cp);
                            dbContext.SaveChanges();
                            Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "cobranzapendiente", cp.v_IdCobranzaPendiente);
                        }
                        #endregion
                    }
                    else
                    {
                        pobjOperationResult.Success = 0;
                        pobjOperationResult.ErrorMessage = "No se encontró la cobranza pendiente";
                        pobjOperationResult.AdditionalInformation = "CobranzaBL.ProcesaDetalleCobranza()";
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
                pobjOperationResult.AdditionalInformation = "CobranzaBL.ProcesarCanjeLetra()";
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }

        public void ReprocesarSaldosVentasMasivo(ref OperationResult pobjOperationResult, string pstrPeriodo, List<string> ClientSession)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var listVentas =
                            dbContext.venta.Where(
                                p =>
                                    p.i_Eliminado == 0 && p.i_IdEstado == 1 && p.v_Periodo.Equals(pstrPeriodo) &&
                                    p.i_IdTipoDocumento.Value != 7).ToList();

                        foreach (var idVenta in listVentas.Select(p => p.v_IdVenta).AsParallel())
                        {
                            RecalcularSaldoVenta(ref pobjOperationResult, idVenta, ClientSession, false);
                            if (pobjOperationResult.Success == 0) return;
                        }
                    }

                    ts.Complete();
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                pobjOperationResult.AdditionalInformation = "CobranzaBL.ReprocesarSaldosVentasMasivo()";
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        #endregion

        #region Bandeja
        public List<cobranzaDto> ListarBusquedaCobranzas(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression, DateTime F_Ini, DateTime F_Fin, string pstrIdCliente)
        {
            try
            {
                var dbContext = new SAMBHSEntitiesModelWin();

                var query = from n in dbContext.cobranza
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
                            select new cobranzaDto
                            {
                                v_IdCobranza = n.v_IdCobranza,
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
                                Moneda = n.i_IdMoneda == 1 ? "S" : "D",
                                IdClientes = (from i in dbContext.cobranzadetalle
                                              join J1 in dbContext.venta on i.v_IdVenta equals J1.v_IdVenta into J1_join
                                              from J1 in J1_join.DefaultIfEmpty()
                                              join jl in dbContext.letrasdetalle on i.v_IdVenta equals jl.v_IdLetrasDetalle into jl_join
                                              from jl in jl_join.DefaultIfEmpty()
                                              where i.v_IdCobranza == n.v_IdCobranza && i.i_Eliminado == 0
                                              select J1 != null ? J1.v_IdCliente : jl != null ? jl.v_IdCliente : "")
                            };

                if (!string.IsNullOrEmpty(pstrIdCliente))
                    query = query.Where(p => p.IdClientes.Contains(pstrIdCliente));

                var objData = query.ToList();
                pobjOperationResult.Success = 1;
                return objData;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }
        #endregion

        #region REPORTE
        public List<ReporteDocumentoVoucher> ReporteDocumentoVoucherCobranzaRapida(string pstrIdVenta)
        {
            NodeBL objNodeBL = new NodeBL();
            OperationResult objOperationResult = new OperationResult();
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = (from n in dbContext.cobranzadetalle

                             where n.i_Eliminado == 0 && n.v_IdVenta == pstrIdVenta

                             select new { n.d_NetoXCobrar }).FirstOrDefault();

                var query1 = (from n in dbContext.venta

                              join A in dbContext.cobranzadetalle on new { IdVenta = n.v_IdVenta, eliminado = 0 } equals new { IdVenta = A.v_IdVenta, eliminado = A.i_Eliminado.Value } into A_join
                              from A in A_join.DefaultIfEmpty()
                              join C in dbContext.cobranza on new { IdCobranza = A.v_IdCobranza, eliminado = 0 } equals new { IdCobranza = C.v_IdCobranza, eliminado = C.i_Eliminado.Value } into C_join
                              from C in C_join.DefaultIfEmpty()
                              join B in dbContext.cliente on new { IdCliente = n.v_IdCliente, eliminado = 0 } equals new { IdCliente = B.v_IdCliente, eliminado = B.i_Eliminado.Value } into B_join
                              from B in B_join.DefaultIfEmpty()

                              join J4 in dbContext.documento on new { i_IdTipoDocumento = n.i_IdTipoDocumento.Value }
                                                    equals new { i_IdTipoDocumento = J4.i_CodigoDocumento } into J4_join
                              from J4 in J4_join.DefaultIfEmpty()

                              join J5 in dbContext.documento on new { i_IdTipoDocumento = C.i_IdTipoDocumento.Value }
                                               equals new { i_IdTipoDocumento = J5.i_CodigoDocumento } into J5_join
                              from J5 in J5_join.DefaultIfEmpty()

                              join J6 in dbContext.documento on new { i_IdTipoDocumento = A.i_IdTipoDocumentoRef.Value }
                                              equals new { i_IdTipoDocumento = J6.i_CodigoDocumento } into J6_join
                              from J6 in J6_join.DefaultIfEmpty()

                              join J7 in dbContext.datahierarchy on new { i_IdMedioPago = C.i_IdMedioPago.Value, b = 44 }
                                             equals new { i_IdMedioPago = J7.i_ItemId, b = J7.i_GroupId } into J7_join
                              from J7 in J7_join.DefaultIfEmpty()

                              join J8 in dbContext.datahierarchy on new { i_IdFormaPago = A.i_IdFormaPago.Value, b = 46 }
                                             equals new { i_IdFormaPago = J8.i_ItemId, b = J8.i_GroupId } into J8_join
                              from J8 in J8_join.DefaultIfEmpty()


                              where A.v_IdVenta == pstrIdVenta

                              select new
                              {
                                  TipoDocumento = J4.v_Siglas,
                                  NroVoucher = "VOUCHER DE INGRESO A CAJA N° " + C.v_Periodo.Trim() + "-" + C.v_Mes.Trim() + "-" + C.v_Correlativo.Trim(),
                                  NroDocumento = n.v_SerieDocumento + " - " + n.v_CorrelativoDocumento,
                                  NombreCliente = n.v_NombreClienteTemporal == string.Empty ? (B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " + B.v_SegundoNombre + " " + B.v_RazonSocial).Trim() : n.v_NombreClienteTemporal,
                                  IdMoneda = n.i_IdMoneda.Value,
                                  FechaEmision = n.t_FechaRegistro.Value,
                                  Moneda = n.i_IdMoneda == 1 ? "S/." : "US$.",
                                  Monto = A.d_NetoXCobrar.Value,//Antes del 21 abril
                                  Titular = C.v_Nombre,
                                  Pago = A.d_ImporteSoles.Value,
                                  FechaCobranza = C.t_FechaRegistro.Value,
                                  TipoCambio = C.d_TipoCambio.Value,
                                  Glosa = C.v_Glosa,
                                  Letra = "",
                                  DocumentoReferencia = J8.v_Value1,
                                  NombreEmpresaPropietaria = "",
                                  DocumentoCobranza = J5.v_Nombre,
                                  MedioPago = J7.v_Value1,
                                  RucEmpresaPropietaria = "",
                                  MonedaPago = C.i_IdMoneda == (int)Currency.Soles ? "S/." : "US$.",
                                  TotalVenta = n.d_Total ?? 0,
                                  PagoConvertido = A.d_ImporteSoles.Value,
                              }
                              ).ToList().Select(p => new ReporteDocumentoVoucher
                              {
                                  TipoDocumento = p.TipoDocumento,
                                  NroVoucher = p.NroVoucher,
                                  NroDocumento = p.NroDocumento,
                                  NombreCliente = p.NombreCliente,
                                  IdMoneda = p.IdMoneda,
                                  FechaEmision = p.FechaEmision,
                                  Moneda = p.Moneda,
                                  Monto = p.Monto,
                                  Titular = p.Titular,
                                  Pago = p.Pago,
                                  FechaCobranza = p.FechaCobranza,
                                  TipoCambio = p.TipoCambio,
                                  Glosa = p.Glosa,
                                  Letra = p.Letra,
                                  DocumentoReferencia = p.DocumentoReferencia,
                                  NombreEmpresaPropietaria = p.NombreEmpresaPropietaria,
                                  DocumentoCobranza = p.DocumentoCobranza,
                                  MedioPago = p.MedioPago,
                                  RucEmpresaPropietaria = p.RucEmpresaPropietaria,
                                  //TotalValorFactura = query.d_NetoXCobrar,
                                  TotalValorFactura = 0,
                                  MonedaPago = p.MonedaPago,
                                  TotalVenta = p.TotalVenta,
                                  PagoConvertido = p.PagoConvertido,

                              }).ToList();
                List<ReporteDocumentoVoucher> objData = query1.ToList();
                List<ReporteDocumentoVoucher> ListaFinal = new List<ReporteDocumentoVoucher>();
                int i = 0;
                string MonedaVenta = objData != null ? objData.FirstOrDefault().Moneda : "";
                decimal TipoCambioCobranza = objData != null ? objData.FirstOrDefault().TipoCambio : 0;
                decimal TotalVenta = objData != null ? objData.FirstOrDefault().TotalVenta : 0;
                foreach (var item in objData)
                {
                    item.TotalValorFactura = TotalVenta;
                    if (item.Moneda == item.MonedaPago)
                    {
                        ListaFinal.Add(item);
                    }
                    else
                    {

                        if (i == 0)
                        {
                            item.Moneda = MonedaVenta;
                            item.Monto = query != null ? query1 != null ? query1.FirstOrDefault().TotalVenta : 0 : 0;
                            item.PagoConvertido = MonedaVenta == item.MonedaPago ? item.PagoConvertido : MonedaVenta == "S/." ? item.Pago.Value * TipoCambioCobranza : item.Pago.Value / TipoCambioCobranza;
                        }
                        else
                        {
                            if (MonedaVenta == ListaFinal[i - 1].MonedaPago)
                            {
                                item.Moneda = MonedaVenta;
                                item.Monto = ListaFinal[i - 1].Monto - ListaFinal[i - 1].Pago;
                                item.PagoConvertido = MonedaVenta != item.MonedaPago ? MonedaVenta == "S/." ? item.PagoConvertido = item.Pago.Value * TipoCambioCobranza : item.PagoConvertido = item.PagoConvertido / TipoCambioCobranza : item.PagoConvertido;
                                //if (MonedaVenta == item.MonedaPago)
                                //{
                                //}
                                //else
                                //{
                                //    if (MonedaVenta == "S/.")
                                //    {
                                //        item.PagoConvertido = item.Pago.Value * TipoCambioCobranza;
                                //    }
                                //    else
                                //    {
                                //        item.PagoConvertido = item.PagoConvertido / TipoCambioCobranza;
                                //    }
                                //}

                            }
                            else
                            {
                                //if (MonedaVenta == "S/.")
                                //{
                                decimal conversion = MonedaVenta == "S/." ? ListaFinal[i - 1].Pago.Value * TipoCambioCobranza : ListaFinal[i - 1].Pago.Value / TipoCambioCobranza;
                                conversion = Utils.Windows.DevuelveValorRedondeado(conversion, 2);
                                item.Monto = MonedaVenta == "S/." ? ListaFinal[i - 1].Monto - conversion : ListaFinal[i - 1].Monto - conversion;
                                item.Moneda = MonedaVenta;
                                item.PagoConvertido = MonedaVenta == "S/." ? item.Pago.Value * TipoCambioCobranza : item.PagoConvertido / TipoCambioCobranza;
                                //}
                                //else
                                //{
                                //    decimal conversion = ListaFinal[i - 1].Pago.Value / TipoCambioCobranza;
                                //    item.Monto = ListaFinal[i - 1].Monto - conversion;
                                //    item.Moneda = MonedaVenta;
                                //    item.PagoConvertido = item.PagoConvertido / TipoCambioCobranza;
                                //}
                            }

                        }

                        ListaFinal.Add(item);
                    }

                    i++;
                }

                // return objData ;

                return ListaFinal;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<ReporteVentaCobranza> ReporteVentasCobranzas(ref OperationResult objOperationResult, DateTime Fecha, int? IdTipoDocumentoCobranzaDetalle, string Orden, string idCliente)
        {


            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                List<ReporteVentaCobranza> query4 = new List<ReporteVentaCobranza>();

                var query = (from n in dbContext.cobranzapendiente

                             join A in dbContext.cobranzadetalle on new { cob = n.v_IdVenta, eliminado = 0 } equals new { cob = A.v_IdVenta, eliminado = A.i_Eliminado.Value } into A_join
                             from A in A_join.DefaultIfEmpty()

                             join a in dbContext.venta on new { v = n.v_IdVenta, eliminado = 0 } equals new { v = a.v_IdVenta, eliminado = a.i_Eliminado.Value } into a_join
                             from a in a_join.DefaultIfEmpty()

                             join C in dbContext.cobranza on new { b = A.v_IdCobranza, eliminado = 0 } equals new { b = C.v_IdCobranza, eliminado = C.i_Eliminado.Value } into C_join
                             from C in C_join.DefaultIfEmpty()

                             join B in dbContext.cliente on new { a.v_IdCliente } equals new { B.v_IdCliente } into B_join
                             from B in B_join.DefaultIfEmpty()

                             join J4 in dbContext.documento on new { i_IdTipoDocumento = a.i_IdTipoDocumento.Value, eliminado = 0 }
                                                   equals new { i_IdTipoDocumento = J4.i_CodigoDocumento, eliminado = J4.i_Eliminado.Value } into J4_join
                             from J4 in J4_join.DefaultIfEmpty()

                             join J5 in dbContext.documento on new { i_IdTipoDocumento = C.i_IdTipoDocumento.Value, eliminado = 0 }
                                              equals new { i_IdTipoDocumento = J5.i_CodigoDocumento, eliminado = J5.i_Eliminado.Value } into J5_join
                             from J5 in J5_join.DefaultIfEmpty()

                             join J6 in dbContext.documento on new { i_IdTipoDocumento = A.i_IdTipoDocumentoRef.Value, eliminado = 0 }
                                             equals new { i_IdTipoDocumento = J6.i_CodigoDocumento, eliminado = J6.i_Eliminado.Value } into J6_join
                             from J6 in J6_join.DefaultIfEmpty()

                             join J7 in dbContext.datahierarchy on new { i_idTipoPago = a.i_IdCondicionPago.Value, b = 23, eliminado = 0 }
                                                          equals new { i_idTipoPago = J7.i_ItemId, b = J7.i_GroupId, eliminado = J7.i_IsDeleted.Value } into J7_join
                             from J7 in J7_join.DefaultIfEmpty()


                             where C.t_FechaRegistro == Fecha && (C.i_IdTipoDocumento == IdTipoDocumentoCobranzaDetalle || IdTipoDocumentoCobranzaDetalle == -1)
                             && (a.v_IdCliente == idCliente || idCliente == "") && n.i_Eliminado == 0

                             && C.i_IdEstado == 1 && a.i_IdEstablecimiento == Globals.ClientSession.i_IdEstablecimiento.Value && a.i_IdEstablecimiento == Globals.ClientSession.i_IdEstablecimiento.Value

                             select new ReporteVentaCobranza
                             {
                                 TipoDocumento = J4.v_Siglas,
                                 NroDocumento = a.v_SerieDocumento + "-" + a.v_CorrelativoDocumento,
                                 FechaEmision = a.t_FechaRegistro,
                                 NombreCliente = a.i_IdEstado == 0 ? "**ANULADO**" : string.IsNullOrEmpty(a.v_NombreClienteTemporal) ? (B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " + B.v_SegundoNombre + " " + B.v_RazonSocial).Trim() : a.v_NombreClienteTemporal,
                                 Glosa = "",
                                 Cheq = J6.v_Siglas,
                                 Moneda = a.i_IdMoneda == 1 ? "S/" : "US$.",
                                 TotalFacturado = a.i_IdEstado == 1 ? a.i_IdMoneda == 1 ? a.d_Total : a.d_Total : 0,
                                 MontoPagado = a.i_IdEstado == 1 ? a.i_IdMoneda == 1 ? A.d_ImporteSoles : A.d_ImporteSoles : 0,
                                 Saldo = a.i_IdEstado == 1 ? a.i_IdMoneda == 1 ? A.d_NetoXCobrar - A.d_ImporteSoles : 0 : 0,
                                 MedioPago = J7.v_Value1,
                                 MontoCobrar = a.i_IdEstado == 1 ? a.i_IdMoneda == 1 ? A.d_NetoXCobrar : A.d_NetoXCobrar : 0,
                                 TotalFacturadoDolares = a.i_IdEstado == 1 ? a.i_IdMoneda == 2 ? a.d_Total : 0 : 0,
                                 MontoCobrarDolares = a.i_IdEstado == 1 ? C.i_IdMoneda == 2 ? A.d_NetoXCobrar : 0 : 0,
                                 MontoPagadoDolares = a.i_IdEstado == 1 ? C.i_IdMoneda == 2 ? A.d_ImporteSoles : 0 : 0,
                                 SaldoDolares = a.i_IdEstado == 1 ? C.i_IdMoneda == 2 ? A.d_NetoXCobrar - A.d_ImporteSoles : 0 : 0,
                                 idTipoDocumento = a.i_IdTipoDocumento,
                                 MedioPagoCobranza = C.i_IdMedioPago,
                                 v_IdCobranzaPendiente = n.v_IdCobranzaPendiente,
                                 IdTipoDocumentoCobranzaDetalle = A.i_IdTipoDocumentoRef,
                                 MonedaCobranza = C.i_IdMoneda == 1 ? "S/" : "US$.",
                                 i_FormaPagoCobranzaDetalle = A.i_IdFormaPago,
                                 LineaResumen = "",
                                 NombreFormaPagoResumen = "",
                                 MontoSolesResumen = 0,
                                 MontoDolaresResumen = 0,
                                 MontoSolesOtroDiaResumen = 0,
                                 MontoDolaresOtroDiaResumen = 0,
                                 MontoSolesDocumentoResumen = 0,
                                 MontoDolaresDocumentoResumen = 0,
                                 MontoSolesOtroDiaDocumentoResumen = 0,
                                 MontoDolaresOtroDiaDocumentoResumen = 0,
                                 FormatoDias = "",
                                 FormatoMonedas = "",
                                 //FechaInsercion = A.t_ActualizaFecha == null ? A.t_InsertaFecha.Value : A.t_ActualizaFecha.Value,
                                 Grupo = J4.v_Nombre,

                             }).ToList().Select(x => new ReporteVentaCobranza
                                 {
                                     TipoDocumento = x.TipoDocumento,
                                     NroDocumento = x.NroDocumento,
                                     FechaEmision = x.FechaEmision,
                                     NombreCliente = x.NombreCliente,
                                     Glosa = x.Glosa,
                                     Cheq = x.Cheq,
                                     Moneda = x.Moneda,
                                     TotalFacturado = x.TotalFacturado,
                                     MontoPagado = x.MontoPagado,
                                     Saldo = x.Saldo,
                                     MedioPago = x.MedioPago,
                                     MontoCobrar = x.MontoCobrar,
                                     TotalFacturadoDolares = x.TotalFacturadoDolares,
                                     MontoCobrarDolares = x.MontoCobrarDolares,
                                     MontoPagadoDolares = x.MontoPagadoDolares,
                                     SaldoDolares = x.SaldoDolares,
                                     idTipoDocumento = x.idTipoDocumento,
                                     MedioPagoCobranza = x.MedioPagoCobranza,
                                     v_IdCobranzaPendiente = x.v_IdCobranzaPendiente,
                                     IdTipoDocumentoCobranzaDetalle = x.IdTipoDocumentoCobranzaDetalle,
                                     MonedaCobranza = x.MonedaCobranza,
                                     i_FormaPagoCobranzaDetalle = x.i_FormaPagoCobranzaDetalle,
                                     LineaResumen = x.LineaResumen,
                                     NombreFormaPagoResumen = x.NombreFormaPagoResumen,
                                     MontoSolesResumen = x.MontoSolesResumen,
                                     MontoDolaresResumen = x.MontoDolaresResumen,
                                     MontoSolesOtroDiaResumen = x.MontoSolesOtroDiaResumen,
                                     MontoDolaresOtroDiaResumen = x.MontoDolaresOtroDiaResumen,
                                     MontoSolesDocumentoResumen = x.MontoSolesDocumentoResumen,
                                     MontoDolaresDocumentoResumen = x.MontoDolaresDocumentoResumen,
                                     MontoSolesOtroDiaDocumentoResumen = x.MontoSolesOtroDiaDocumentoResumen,
                                     MontoDolaresOtroDiaDocumentoResumen = x.MontoDolaresOtroDiaDocumentoResumen,
                                     FormatoDias = x.FormatoDias,
                                     FormatoMonedas = x.FormatoMonedas,
                                     // FechaInsercion = x.FechaInsercion,


                                     TipNumDocumento = x.idTipoDocumento.ToString() + " " + x.NroDocumento,

                                 });



                if (IdTipoDocumentoCobranzaDetalle == -1)
                {

                    var query3 = (from a in dbContext.venta

                                  join b in dbContext.cobranzapendiente on new { v = a.v_IdVenta, eliminado = 0 } equals new { v = b.v_IdVenta, eliminado = b.i_Eliminado.Value } into b_join

                                  from b in b_join.DefaultIfEmpty()

                                  join B in dbContext.cliente on new { a.v_IdCliente } equals new { B.v_IdCliente } into B_join
                                  from B in B_join.DefaultIfEmpty()

                                  join J4 in dbContext.documento on new { i_IdTipoDocumento = a.i_IdTipoDocumento.Value, eliminado = 0 }
                                                        equals new { i_IdTipoDocumento = J4.i_CodigoDocumento, eliminado = J4.i_Eliminado.Value } into J4_join
                                  from J4 in J4_join.DefaultIfEmpty()



                                  join J7 in dbContext.datahierarchy on new { i_idTipoPago = a.i_IdCondicionPago.Value, b = 23, eliminado = 0 }
                                                               equals new { i_idTipoPago = J7.i_ItemId, b = J7.i_GroupId, eliminado = J7.i_IsDeleted.Value } into J7_join
                                  from J7 in J7_join.DefaultIfEmpty()


                                  where a.t_FechaRegistro == Fecha && (a.v_IdCliente == idCliente || idCliente == "")

                                  && a.i_IdEstablecimiento == Globals.ClientSession.i_IdEstablecimiento.Value && a.i_Eliminado == 0
                                  select new ReporteVentaCobranza
                                 {
                                     TipoDocumento = J4.v_Siglas,
                                     NroDocumento = a.v_SerieDocumento + "-" + a.v_CorrelativoDocumento,
                                     FechaEmision = a.t_FechaRegistro,
                                     NombreCliente = a.i_IdEstado == 0 ? "**ANULADO**" : string.IsNullOrEmpty(a.v_NombreClienteTemporal) ? (B.v_PrimerNombre + " " + B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_RazonSocial).Trim() : a.v_NombreClienteTemporal,
                                     Cheq = "",
                                     Moneda = a.i_IdMoneda == 1 ? "S/" : "US$.",
                                     TotalFacturado = a.i_IdEstado == 1 ? a.i_IdMoneda == 1 ? a.d_Total : a.d_Total : 0,
                                     MontoPagado = 0,
                                     Saldo = a.i_IdEstado == 1 ? a.i_IdMoneda == 1 ? a.d_Total : a.d_Total : 0,
                                     MedioPago = J7.v_Value1,
                                     MontoCobrar = a.i_IdEstado == 1 ? a.i_IdMoneda == 1 ? a.d_Total : a.d_Total : 0,
                                     TotalFacturadoDolares = a.i_IdEstado == 1 ? a.i_IdMoneda == 2 ? a.d_Total : 0 : 0,
                                     MontoCobrarDolares = a.i_IdEstado == 1 ? a.i_IdMoneda == 2 ? a.d_Total : 0 : 0,
                                     MontoPagadoDolares = a.i_IdEstado == 1 ? a.i_IdMoneda == 2 ? 0 : 0 : 0,
                                     SaldoDolares = a.i_IdEstado == 1 ? a.i_IdMoneda == 2 ? 0 : 0 : 0,
                                     idTipoDocumento = a.i_IdTipoDocumento,
                                     MedioPagoCobranza = -1,
                                     v_IdCobranzaPendiente = b.v_IdCobranzaPendiente,
                                     IdTipoDocumentoCobranzaDetalle = -1,
                                     MonedaCobranza = a.i_IdMoneda == 1 ? "S/" : "US$.",
                                     i_FormaPagoCobranzaDetalle = -1,
                                     LineaResumen = "",
                                     NombreFormaPagoResumen = "",
                                     MontoSolesResumen = 0,
                                     MontoDolaresResumen = 0,
                                     MontoSolesOtroDiaResumen = 0,
                                     MontoDolaresOtroDiaResumen = 0,
                                     MontoSolesDocumentoResumen = 0,
                                     MontoDolaresDocumentoResumen = 0,
                                     MontoSolesOtroDiaDocumentoResumen = 0,
                                     MontoDolaresOtroDiaDocumentoResumen = 0,
                                     FormatoDias = "",
                                     FormatoMonedas = "",
                                     //FechaInsercion = b.t_ActualizaFecha == null ? b.t_InsertaFecha.Value : b.t_ActualizaFecha.Value,
                                     Grupo = J4.v_Nombre,

                                 }).ToList().Where(o => !query.Select(p => p.v_IdCobranzaPendiente).Contains(o.v_IdCobranzaPendiente)).Select(x => new ReporteVentaCobranza
                                 {

                                     TipoDocumento = x.TipoDocumento,
                                     NroDocumento = x.NroDocumento,
                                     FechaEmision = x.FechaEmision,
                                     NombreCliente = x.NombreCliente,
                                     Glosa = x.Glosa,
                                     Cheq = x.Cheq,
                                     Moneda = x.Moneda,
                                     TotalFacturado = x.TotalFacturado,
                                     MontoPagado = x.MontoPagado,
                                     Saldo = x.Saldo,
                                     MedioPago = x.MedioPago,
                                     MontoCobrar = x.MontoCobrar,
                                     TotalFacturadoDolares = x.TotalFacturadoDolares,
                                     MontoCobrarDolares = x.MontoCobrarDolares,
                                     MontoPagadoDolares = x.MontoPagadoDolares,
                                     SaldoDolares = x.SaldoDolares,
                                     idTipoDocumento = x.idTipoDocumento,
                                     MedioPagoCobranza = x.MedioPagoCobranza,
                                     v_IdCobranzaPendiente = x.v_IdCobranzaPendiente,
                                     IdTipoDocumentoCobranzaDetalle = x.IdTipoDocumentoCobranzaDetalle,
                                     MonedaCobranza = x.MonedaCobranza,
                                     i_FormaPagoCobranzaDetalle = x.i_FormaPagoCobranzaDetalle,
                                     LineaResumen = x.LineaResumen,
                                     NombreFormaPagoResumen = x.NombreFormaPagoResumen,
                                     MontoSolesResumen = x.MontoSolesResumen,
                                     MontoDolaresResumen = x.MontoDolaresResumen,
                                     MontoSolesOtroDiaResumen = x.MontoSolesOtroDiaResumen,
                                     MontoDolaresOtroDiaResumen = x.MontoDolaresOtroDiaResumen,
                                     MontoSolesDocumentoResumen = x.MontoSolesDocumentoResumen,
                                     MontoDolaresDocumentoResumen = x.MontoDolaresDocumentoResumen,
                                     MontoSolesOtroDiaDocumentoResumen = x.MontoSolesOtroDiaDocumentoResumen,
                                     MontoDolaresOtroDiaDocumentoResumen = x.MontoDolaresOtroDiaDocumentoResumen,
                                     FormatoDias = x.FormatoDias,
                                     FormatoMonedas = x.FormatoMonedas,
                                     //FechaInsercion = x.FechaInsercion,
                                     TipNumDocumento = x.idTipoDocumento.ToString() + " " + x.NroDocumento,


                                 });
                    query4 = query.Union(query3).ToList();
                }

                else
                {

                    query4 = query.ToList();
                }



                objOperationResult.Success = 1;
                if (Orden == "TipoDocumento")
                {
                    return query4.OrderBy(x => x.TipNumDocumento).ToList();
                }

                else
                {
                    return query4.OrderBy(x => x.NroDocumento).ToList();

                }



            }
            catch (Exception)
            {
                objOperationResult.Success = 0;
                return null;
            }
        }
        public List<TotalCobranzaDto> ReporteVentasCobranzasResumen(ref OperationResult objOperationResult, DateTime Fecha, int? IdTipoDocumentoCobranzaDetalle, string Orden, string idCliente)
        {
            NodeBL objNodeBL = new NodeBL();

            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                List<ReporteVentaCobranza> query4 = new List<ReporteVentaCobranza>();

                var query = (from n in dbContext.cobranzapendiente

                             join A in dbContext.cobranzadetalle on new { a = n.v_IdVenta, eliminado = 0 } equals new { a = A.v_IdVenta, eliminado = A.i_Eliminado.Value } into A_join
                             from A in A_join.DefaultIfEmpty()

                             join a in dbContext.venta on new { v = n.v_IdVenta, eliminado = 0 } equals new { v = a.v_IdVenta, eliminado = a.i_Eliminado.Value } into a_join
                             from a in a_join.DefaultIfEmpty()

                             join C in dbContext.cobranza on new { b = A.v_IdCobranza, eliminado = 0 } equals new { b = C.v_IdCobranza, eliminado = C.i_Eliminado.Value } into C_join
                             from C in C_join.DefaultIfEmpty()

                             join B in dbContext.cliente on new { a.v_IdCliente } equals new { B.v_IdCliente } into B_join
                             from B in B_join.DefaultIfEmpty()

                             join J4 in dbContext.documento on new { i_IdTipoDocumento = a.i_IdTipoDocumento.Value, eliminado = 0 }
                                                   equals new { i_IdTipoDocumento = J4.i_CodigoDocumento, eliminado = J4.i_Eliminado.Value } into J4_join
                             from J4 in J4_join.DefaultIfEmpty()

                             join J5 in dbContext.documento on new { i_IdTipoDocumento = C.i_IdTipoDocumento.Value, eliminado = 0 }
                                              equals new { i_IdTipoDocumento = J5.i_CodigoDocumento, eliminado = J5.i_Eliminado.Value } into J5_join
                             from J5 in J5_join.DefaultIfEmpty()

                             join J6 in dbContext.documento on new { i_IdTipoDocumento = A.i_IdTipoDocumentoRef.Value, eliminado = 0 }
                                             equals new { i_IdTipoDocumento = J6.i_CodigoDocumento, eliminado = J6.i_Eliminado.Value } into J6_join
                             from J6 in J6_join.DefaultIfEmpty()

                             join J7 in dbContext.datahierarchy on new { i_idTipoPago = a.i_IdCondicionPago.Value, b = 23, eliminado = 0 }
                                                          equals new { i_idTipoPago = J7.i_ItemId, b = J7.i_GroupId, eliminado = J7.i_IsDeleted.Value } into J7_join
                             from J7 in J7_join.DefaultIfEmpty()

                             join J8 in dbContext.datahierarchy on new { i_idFormaPago = A.i_IdFormaPago.Value, b = 46, eliminado = 0 }
                                                             equals new { i_idFormaPago = J8.i_ItemId, b = J8.i_GroupId, eliminado = J8.i_IsDeleted.Value } into J8_join
                             from J8 in J8_join.DefaultIfEmpty()


                             where C.t_FechaRegistro == Fecha && (C.i_IdTipoDocumento == IdTipoDocumentoCobranzaDetalle || IdTipoDocumentoCobranzaDetalle == -1)
                             && (a.v_IdCliente == idCliente || idCliente == "") && n.i_Eliminado == 0
                             && C.i_IdEstado == 1 && a.i_IdEstablecimiento == Globals.ClientSession.i_IdEstablecimiento.Value && C.i_IdEstablecimiento.Value == Globals.ClientSession.i_IdEstablecimiento.Value
                             select new ReporteVentaCobranza
                             {
                                 TipoDocumento = J4.v_Siglas,
                                 NroDocumento = a.v_SerieDocumento + "-" + a.v_CorrelativoDocumento,
                                 FechaEmision = a.t_FechaRegistro,
                                 NombreCliente = a.i_IdEstado == 0 ? "**ANULADO**" : string.IsNullOrEmpty(a.v_NombreClienteTemporal) ? (B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " + B.v_SegundoNombre + " " + B.v_RazonSocial).Trim() : a.v_NombreClienteTemporal,
                                 Glosa = "",
                                 Cheq = J6.v_Siglas,
                                 Moneda = a.i_IdMoneda == 1 ? "S/" : "US$.",
                                 TotalFacturado = a.i_IdEstado == 1 ? a.i_IdMoneda == 1 ? a.d_Total : a.d_Total : 0,
                                 MontoPagado = a.i_IdEstado == 1 ? a.i_IdMoneda == 1 ? A.d_ImporteSoles : A.d_ImporteSoles : 0,
                                 Saldo = a.i_IdEstado == 0 ? 0 : a.i_IdMoneda == 1 ? A.d_NetoXCobrar - A.d_ImporteSoles : A.d_NetoXCobrar - A.d_ImporteSoles,
                                 MedioPago = J7.v_Value1,
                                 MontoCobrar = a.i_IdEstado == 0 ? 0 : a.i_IdMoneda == 1 ? A.d_NetoXCobrar : A.d_NetoXCobrar,
                                 TotalFacturadoDolares = a.i_IdEstado == 0 ? 0 : a.i_IdMoneda == 2 ? a.d_Total : 0,
                                 MontoCobrarDolares = a.i_IdEstado == 0 ? 0 : C.i_IdMoneda == 2 ? A.d_NetoXCobrar : 0,
                                 MontoPagadoDolares = a.i_IdEstado == 0 ? 0 : C.i_IdMoneda == 2 ? A.d_ImporteSoles : 0,
                                 SaldoDolares = a.i_IdEstado == 0 ? 0 : C.i_IdMoneda == 2 ? A.d_NetoXCobrar - A.d_ImporteSoles : 0,
                                 idTipoDocumento = a.i_IdTipoDocumento,
                                 MedioPagoCobranza = C.i_IdMedioPago,
                                 v_IdCobranzaPendiente = n.v_IdCobranzaPendiente,
                                 IdTipoDocumentoCobranzaDetalle = A.i_IdTipoDocumentoRef,
                                 MonedaCobranza = C.i_IdMoneda == 1 ? "S/" : "US$.",
                                 i_FormaPagoCobranzaDetalle = A.i_IdFormaPago,
                                 v_FormaPagoCobranzaDetalle = J8.v_Value1,
                                 Grupo = J4.v_Nombre,
                             }).ToList();
                query4 = query.ToList();



                var lista = (from A in query4.ToList()
                             let CalcularTotales = MetodoCalcularTotalesCobranza(query4, A.i_FormaPagoCobranzaDetalle, Fecha)
                             select new TotalCobranzaDto
                             {

                                 FacturasDia = CalcularTotales[0],
                                 BoletasDia = CalcularTotales[1],
                                 NotasCreditoDia = CalcularTotales[2],
                                 NotasDebitoDia = CalcularTotales[3],
                                 FacturasOtroDia = CalcularTotales[4],
                                 BoletasOtroDia = CalcularTotales[5],
                                 NotasCreditoOtroDia = CalcularTotales[6],
                                 NotasDebitoOtroDia = CalcularTotales[7],
                                 FacturasDiaDolares = CalcularTotales[8],
                                 BoletasDiaDolares = CalcularTotales[9],
                                 NotasCreditoDiaDolares = CalcularTotales[10],
                                 NotasDebitoDiaDolares = CalcularTotales[11],
                                 FacturasOtroDiaDolares = CalcularTotales[12],
                                 BoletasOtroDiaDolares = CalcularTotales[13],
                                 NotasCreditoOtroDiaDolares = CalcularTotales[14],
                                 NotasDebitoOtroDiaDolares = CalcularTotales[15],
                                 MontoSolesDia = CalcularTotales[16],
                                 MontoSolesOtroDia = CalcularTotales[17],
                                 MontoDolaresDia = CalcularTotales[18],
                                 MontoDolaresOtroDia = CalcularTotales[19],
                                 MonedaCobranza = A.MonedaCobranza,
                                 i_FormaPagoCobranzaDetalle = A.i_FormaPagoCobranzaDetalle,
                                 v_FormaPagoCobranzaDetalle = A.v_FormaPagoCobranzaDetalle,

                                 Grupo = A.Grupo,

                             }).ToList();



                var objData = new List<TotalCobranzaDto>();
                objData = lista.GroupBy(w => new { w.i_FormaPagoCobranzaDetalle.Value })
                                            .Select(group => group.First())
                                            .OrderByDescending(o => o.i_FormaPagoCobranzaDetalle.Value).ToList();

                objOperationResult.Success = 1;
                return objData;


            }
            catch (Exception)
            {
                objOperationResult.Success = 0;
                return null;
            }

        }
        public List<CobranzaTipoDocumentoDto> ReporteVentasCobranzasResumenDocumentos(ref OperationResult objOperationResult, DateTime Fecha, int? IdTipoDocumentoCobranzaDetalle, string Orden, string idCliente)
        {
            NodeBL objNodeBL = new NodeBL();

            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                List<ReporteVentaCobranza> query4 = new List<ReporteVentaCobranza>();

                var query = (from n in dbContext.cobranzapendiente

                             join A in dbContext.cobranzadetalle on new { a = n.v_IdVenta, eliminado = 0 } equals new { a = A.v_IdVenta, eliminado = A.i_Eliminado.Value } into A_join
                             from A in A_join.DefaultIfEmpty()

                             join a in dbContext.venta on new { v = n.v_IdVenta, eliminado = 0 } equals new { v = a.v_IdVenta, eliminado = a.i_Eliminado.Value } into a_join
                             from a in a_join.DefaultIfEmpty()

                             join C in dbContext.cobranza on new { b = A.v_IdCobranza, eliminado = 0 } equals new { b = C.v_IdCobranza, eliminado = C.i_Eliminado.Value } into C_join
                             from C in C_join.DefaultIfEmpty()

                             join B in dbContext.cliente on new { a.v_IdCliente } equals new { B.v_IdCliente } into B_join
                             from B in B_join.DefaultIfEmpty()

                             join J4 in dbContext.documento on new { i_IdTipoDocumento = a.i_IdTipoDocumento.Value, eliminado = 0 }
                                                   equals new { i_IdTipoDocumento = J4.i_CodigoDocumento, eliminado = J4.i_Eliminado.Value } into J4_join
                             from J4 in J4_join.DefaultIfEmpty()

                             join J5 in dbContext.documento on new { i_IdTipoDocumento = C.i_IdTipoDocumento.Value, eliminado = 0 }
                                              equals new { i_IdTipoDocumento = J5.i_CodigoDocumento, eliminado = J5.i_Eliminado.Value } into J5_join
                             from J5 in J5_join.DefaultIfEmpty()

                             join J6 in dbContext.documento on new { i_IdTipoDocumento = A.i_IdTipoDocumentoRef.Value, eliminado = 0 }
                                             equals new { i_IdTipoDocumento = J6.i_CodigoDocumento, eliminado = J6.i_Eliminado.Value } into J6_join
                             from J6 in J6_join.DefaultIfEmpty()

                             join J7 in dbContext.datahierarchy on new { i_idTipoPago = a.i_IdCondicionPago.Value, b = 23, eliminado = 0 }
                                                          equals new { i_idTipoPago = J7.i_ItemId, b = J7.i_GroupId, eliminado = J7.i_IsDeleted.Value } into J7_join
                             from J7 in J7_join.DefaultIfEmpty()

                             join J8 in dbContext.datahierarchy on new { i_idFormaPago = A.i_IdFormaPago.Value, b = 46, eliminado = 0 }
                                                             equals new { i_idFormaPago = J8.i_ItemId, b = J8.i_GroupId, eliminado = J8.i_IsDeleted.Value } into J8_join
                             from J8 in J8_join.DefaultIfEmpty()


                             where C.t_FechaRegistro == Fecha && (C.i_IdTipoDocumento == IdTipoDocumentoCobranzaDetalle || IdTipoDocumentoCobranzaDetalle == -1)
                             && (a.v_IdCliente == idCliente || idCliente == "") && n.i_Eliminado == 0

                             && C.i_IdEstado == 1 && a.i_IdEstablecimiento == Globals.ClientSession.i_IdEstablecimiento.Value
                             select new ReporteVentaCobranza
                             {
                                 TipoDocumento = J4.v_Siglas,
                                 NroDocumento = a.v_SerieDocumento + "-" + a.v_CorrelativoDocumento,
                                 FechaEmision = a.t_FechaRegistro,
                                 NombreCliente = a.i_IdEstado == 0 ? "**ANULADO**" : string.IsNullOrEmpty(a.v_NombreClienteTemporal) ? (B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " + B.v_SegundoNombre + " " + B.v_RazonSocial).Trim() : a.v_NombreClienteTemporal,
                                 Glosa = "",
                                 Cheq = J6.v_Siglas,
                                 Moneda = a.i_IdMoneda == 1 ? "S/" : "US$.",
                                 TotalFacturado = a.i_IdEstado == 0 ? 0 : a.i_IdMoneda == 1 ? a.d_Total : a.d_Total,
                                 MontoPagado = a.i_IdEstado == 0 ? 0 : a.i_IdMoneda == 1 ? A.d_ImporteSoles : A.d_ImporteSoles,
                                 Saldo = a.i_IdEstado == 0 ? 0 : a.i_IdMoneda == 1 ? A.d_NetoXCobrar - A.d_ImporteSoles : A.d_NetoXCobrar - A.d_ImporteSoles,
                                 MedioPago = J7.v_Value1,
                                 MontoCobrar = a.i_IdEstado == 0 ? 0 : a.i_IdMoneda == 1 ? A.d_NetoXCobrar : A.d_NetoXCobrar,
                                 TotalFacturadoDolares = a.i_IdEstado == 0 ? 0 : a.i_IdMoneda == 2 ? a.d_Total : 0,
                                 MontoCobrarDolares = a.i_IdEstado == 0 ? 0 : C.i_IdMoneda == 2 ? A.d_NetoXCobrar : 0,
                                 MontoPagadoDolares = a.i_IdEstado == 0 ? 0 : C.i_IdMoneda == 2 ? A.d_ImporteSoles : 0,
                                 SaldoDolares = a.i_IdEstado == 0 ? 0 : C.i_IdMoneda == 2 ? A.d_NetoXCobrar - A.d_ImporteSoles : 0,
                                 idTipoDocumento = a.i_IdTipoDocumento,
                                 MedioPagoCobranza = C.i_IdMedioPago,
                                 v_IdCobranzaPendiente = n.v_IdCobranzaPendiente,
                                 IdTipoDocumentoCobranzaDetalle = A.i_IdTipoDocumentoRef,
                                 MonedaCobranza = C.i_IdMoneda == 1 ? "S/" : "US$.",
                                 i_FormaPagoCobranzaDetalle = A.i_IdFormaPago,
                                 v_FormaPagoCobranzaDetalle = J8.v_Value1,
                                 Grupo = J4.v_Nombre,
                             }).ToList();



                if (IdTipoDocumentoCobranzaDetalle == -1)
                {

                    var query3 = (from a in dbContext.venta

                                  join b in dbContext.cobranzapendiente on a.v_IdVenta equals b.v_IdVenta into b_join

                                  from b in b_join.DefaultIfEmpty()

                                  join B in dbContext.cliente on a.v_IdCliente equals B.v_IdCliente into B_join
                                  from B in B_join.DefaultIfEmpty()

                                  join J4 in dbContext.documento on new { i_IdTipoDocumento = a.i_IdTipoDocumento.Value }
                                                        equals new { i_IdTipoDocumento = J4.i_CodigoDocumento } into J4_join
                                  from J4 in J4_join.DefaultIfEmpty()

                                  join J7 in dbContext.datahierarchy on new { i_idTipoPago = a.i_IdCondicionPago.Value, b = 23 }
                                                               equals new { i_idTipoPago = J7.i_ItemId, b = J7.i_GroupId } into J7_join
                                  from J7 in J7_join.DefaultIfEmpty()

                                  join J8 in dbContext.datahierarchy on new { i_idFormaPago = a.i_IdCondicionPago.Value, b = 46 }
                                                              equals new { i_idFormaPago = J8.i_ItemId, b = J8.i_GroupId } into J8_join
                                  from J8 in J8_join.DefaultIfEmpty()

                                  where a.t_FechaRegistro == Fecha && (a.v_IdCliente == idCliente || idCliente == "") && b.i_Eliminado == 0 && a.i_Eliminado == 0

                                  && a.i_IdEstablecimiento == Globals.ClientSession.i_IdEstablecimiento.Value
                                  select new ReporteVentaCobranza
                                  {
                                      TipoDocumento = J4.v_Siglas,
                                      NroDocumento = a.v_SerieDocumento + "-" + a.v_CorrelativoDocumento,
                                      FechaEmision = a.t_FechaRegistro,
                                      NombreCliente = a.i_IdEstado == 0 ? "**ANULADO**" : string.IsNullOrEmpty(a.v_NombreClienteTemporal) ? (B.v_PrimerNombre + " " + B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_RazonSocial).Trim() : a.v_NombreClienteTemporal,
                                      Cheq = "",
                                      Moneda = a.i_IdMoneda == 1 ? "S/" : "US$.",
                                      TotalFacturado = a.i_IdEstado == 0 ? 0 : a.i_IdMoneda == 1 ? a.d_Total : a.d_Total,
                                      MontoPagado = 0,
                                      Saldo = a.i_IdEstado == 0 ? 0 : a.i_IdMoneda == 1 ? a.d_Total : a.d_Total,
                                      MedioPago = J7.v_Value1,
                                      MontoCobrar = a.i_IdEstado == 0 ? 0 : a.i_IdMoneda == 1 ? a.d_Total : a.d_Total,
                                      TotalFacturadoDolares = a.i_IdEstado == 0 ? 0 : a.i_IdMoneda == 2 ? a.d_Total : 0,
                                      MontoCobrarDolares = a.i_IdEstado == 0 ? 0 : a.i_IdMoneda == 2 ? a.d_Total : 0,
                                      MontoPagadoDolares = a.i_IdEstado == 0 ? 0 : a.i_IdMoneda == 2 ? 0 : 0,
                                      SaldoDolares = a.i_IdEstado == 0 ? 0 : a.i_IdMoneda == 2 ? 0 : 0,
                                      idTipoDocumento = a.i_IdTipoDocumento,
                                      MedioPagoCobranza = -1,
                                      v_IdCobranzaPendiente = b.v_IdCobranzaPendiente,
                                      IdTipoDocumentoCobranzaDetalle = -1,
                                      MonedaCobranza = a.i_IdMoneda == 1 ? "S/" : "US$.",
                                      i_FormaPagoCobranzaDetalle = -1,
                                      v_FormaPagoCobranzaDetalle = J8.v_Value1,
                                      Grupo = J4.v_Nombre,
                                  }).ToList().Where(o => !query.Select(p => p.v_IdCobranzaPendiente).Contains(o.v_IdCobranzaPendiente));
                    query4 = query.Union(query3).ToList();
                }

                else
                {

                    query4 = query.ToList();
                }


                var lista = (from A in query4.ToList()
                             let CalcularTotales = MetodoCalcularTotalesCobranzaDocumentos(query4, A.idTipoDocumento, Fecha)
                             select new CobranzaTipoDocumentoDto
                             {

                                 MontoSolesDiaDocumento = CalcularTotales[0],
                                 MontoDolaresDiaDocumento = CalcularTotales[1],
                                 MontoSolesOtroDiaDcumento = CalcularTotales[2],
                                 MontoDolaresOtroDiaDocumento = CalcularTotales[3],
                                 MonedaCobranza = A.MonedaCobranza,
                                 i_FormaPagoCobranzaDetalle = A.i_FormaPagoCobranzaDetalle,
                                 v_FormaPagoCobranzaDetalle = A.v_FormaPagoCobranzaDetalle,
                                 idTipoDocumento = A.idTipoDocumento,
                                 Documento = A.TipoDocumento,
                                 Grupo = A.Grupo,

                             }).ToList();




                var objData = new List<CobranzaTipoDocumentoDto>();
                objData = lista.GroupBy(w => new { w.idTipoDocumento.Value })
                                            .Select(group => group.First())
                                            .OrderByDescending(o => o.i_FormaPagoCobranzaDetalle.Value).ToList();

                objOperationResult.Success = 1;
                return objData;


            }
            catch (Exception)
            {
                objOperationResult.Success = 0;
                return null;
            }

        }

        public List<ReportePlanillaCobranza> ReportePlanillaCobranzas(ref OperationResult objOperationResult, DateTime pstrt_FechaRegistroIni, DateTime pstrt_FechaRegistroFin, int? IdTipoDocumentoCobranzaDetalle, string Orden, string idCliente, string pstrt_IdVendedor, string pstrt_Serie, string pstr_grupollave, string pstr_Nombregrupollave, string Correlativo, int TipoDocumento, bool SoloResumen, int IdUsuario, int Establecimiento)
        {


            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                Stopwatch _timer = new Stopwatch();
                _timer.Start();

                List<ReportePlanillaCobranza> queryVentasPagadas = (from n in dbContext.cobranzapendiente

                                                                    join A in dbContext.cobranzadetalle on new { a = n.v_IdVenta, eliminado = 0 } equals new { a = A.v_IdVenta, eliminado = A.i_Eliminado.Value } into A_join
                                                                    from A in A_join.DefaultIfEmpty()

                                                                    join a in dbContext.venta on new { IdVenta = n.v_IdVenta, eliminado = 0 } equals new { IdVenta = a.v_IdVenta, eliminado = a.i_Eliminado.Value } into a_join
                                                                    from a in a_join.DefaultIfEmpty()

                                                                    join C in dbContext.cobranza on new { b = A.v_IdCobranza, eliminado = 0 } equals new { b = C.v_IdCobranza, eliminado = C.i_Eliminado.Value } into C_join
                                                                    from C in C_join.DefaultIfEmpty()

                                                                    join B in dbContext.cliente on new { IdCliente = a.v_IdCliente, eliminado = 0 } equals new { IdCliente = B.v_IdCliente, eliminado = B.i_Eliminado.Value } into B_join
                                                                    from B in B_join.DefaultIfEmpty()

                                                                    join J4 in dbContext.documento on new { i_IdTipoDocumento = a.i_IdTipoDocumento.Value, eliminado = 0 }
                                                                                          equals new { i_IdTipoDocumento = J4.i_CodigoDocumento, eliminado = J4.i_Eliminado.Value } into J4_join
                                                                    from J4 in J4_join.DefaultIfEmpty()

                                                                    join J6 in dbContext.documento on new { i_IdTipoDocumento = A.i_IdTipoDocumentoRef.Value, eliminado = 0 }
                                                                                    equals new { i_IdTipoDocumento = J6.i_CodigoDocumento, eliminado = J6.i_Eliminado.Value } into J6_join
                                                                    from J6 in J6_join.DefaultIfEmpty()

                                                                    join J7 in dbContext.datahierarchy on new { i_idTipoPago = a.i_IdCondicionPago.Value, b = 23, eliminado = 0 }
                                                                                                 equals new { i_idTipoPago = J7.i_ItemId, b = J7.i_GroupId, eliminado = J7.i_IsDeleted.Value } into J7_join
                                                                    from J7 in J7_join.DefaultIfEmpty()

                                                                    join J8 in dbContext.documento on new { i_IdTipoDocumento = C.i_IdTipoDocumento.Value, eliminado = 0 }
                                                                                                   equals new { i_IdTipoDocumento = J8.i_CodigoDocumento, eliminado = J8.i_Eliminado.Value } into J8_join
                                                                    from J8 in J8_join.DefaultIfEmpty()

                                                                    join J9 in dbContext.datahierarchy on new { Grupo = 46, eliminado = 0, FormaPago = A.i_IdFormaPago.Value } equals new { Grupo = J9.i_GroupId, eliminado = J9.i_IsDeleted.Value, FormaPago = J9.i_ItemId } into J9_join

                                                                    from J9 in J9_join.DefaultIfEmpty()

                                                                    join J10 in dbContext.vendedor on new { Vendedor = a.v_IdVendedor, eliminado = 0 } equals new { Vendedor = J10.v_IdVendedor, eliminado = J10.i_Eliminado.Value } into J10_join
                                                                    from J10 in J10_join.DefaultIfEmpty()


                                                                    join J11 in dbContext.systemuser on new { usuarioCreacion = C.i_InsertaIdUsuario.Value, eliminado = 0 } equals new { usuarioCreacion = J11.i_SystemUserId, eliminado = J11.i_IsDeleted.Value } into J11_join
                                                                    from J11 in J11_join.DefaultIfEmpty()

                                                                    where (C.t_FechaRegistro >= pstrt_FechaRegistroIni && C.t_FechaRegistro <= pstrt_FechaRegistroFin) && (C.i_IdTipoDocumento == IdTipoDocumentoCobranzaDetalle || IdTipoDocumentoCobranzaDetalle == -1)
                                                                    && (B.v_CodCliente == idCliente || idCliente == "") && n.i_Eliminado == 0
                                                                    && (a.v_IdVendedor == pstrt_IdVendedor || pstrt_IdVendedor == "-1") && (a.v_SerieDocumento == pstrt_Serie || pstrt_Serie == "")
                                                                    && (a.v_CorrelativoDocumento == Correlativo || Correlativo == "") && (a.i_IdTipoDocumento == TipoDocumento || TipoDocumento == -1)
                                                                    && C.i_IdEstado == 1
                                                                    && (C.i_InsertaIdUsuario == IdUsuario || IdUsuario == -1)
                                                                        //&& (J8.v_NroCuenta != null && J8.v_NroCuenta.StartsWith("10")) // COMO LAS VIS SE CAMBIO DE CUENTA A 169903 , YA NO APARECIAN EN EL REPORTE
                                                                     && (J8.v_NroCuenta != null)
                                                                     && C.i_IdEstablecimiento == Establecimiento
                                                                     && a.i_IdEstablecimiento == Establecimiento
                                                                     && A_join.Any(o => o.v_IdVenta == a.v_IdVenta)

                                                                    select new ReportePlanillaCobranza
                                                                    {
                                                                        TipoDocumento = J4.v_Siglas,
                                                                        NroDocumento = a.v_SerieDocumento + "-" + a.v_CorrelativoDocumento,
                                                                        FechaEmision = C.t_FechaRegistro,
                                                                        NombreCliente = string.IsNullOrEmpty(a.v_NombreClienteTemporal) ? (B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " + B.v_SegundoNombre + " " + B.v_RazonSocial).Trim() : a.v_NombreClienteTemporal,
                                                                        Cheq = J6.v_Siglas,
                                                                        Moneda = a.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$.",
                                                                        TotalFacturado = a.d_Total,
                                                                        MontoPagado = C.i_IdMoneda == (int)Currency.Soles ? A.d_ImporteSoles : 0,
                                                                        Saldo = a.i_IdMoneda == (int)Currency.Soles ? A.d_NetoXCobrar - A.d_ImporteSoles : 0,
                                                                        idTipoDocumento = a.i_IdTipoDocumento,
                                                                        MedioPago = J7.v_Value1,
                                                                        MontoCobrar = A.d_NetoXCobrar - (A.d_ImporteSoles + (A.d_MontoRetencion ?? 0)),
                                                                        MontoPagadoDolares = C.i_IdMoneda == (int)Currency.Dolares ? A.d_ImporteSoles : 0,
                                                                        MonedaCobranza = C.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$.",
                                                                        SaldoDolares = C.i_IdMoneda == (int)Currency.Dolares ? A.d_NetoXCobrar - A.d_ImporteSoles : 0,
                                                                        MedioPagoCobranza = C.i_IdMedioPago,
                                                                        v_IdCobranzaPendiente = n.v_IdCobranzaPendiente,
                                                                        IdTipoDocumentoCobranzaDetalle = A.i_IdTipoDocumentoRef,
                                                                        Grupollave = pstr_grupollave == "NRODOCUMENTOCOBRANZA" ? J8 == null ? "** " + pstr_Nombregrupollave + " NO EXISTE **" : "DOCUMENTO DE COBRANZA :" + J8.v_Nombre : "" + pstr_grupollave == "MEDIOPAGO" ? J7 == null ? "** " + pstr_Nombregrupollave + " NO EXISTE **" : pstr_Nombregrupollave + " : " + J7.v_Value1 : "" + pstr_grupollave == "TIPODOCUMENTOVENTA" ? J4 == null ? "** TIPO DOCUMENTO NO EXISTE **" : "TIPO DE COMPROBANTE : " + J4.v_Nombre : "",
                                                                        TipoDocumentoCobranza = J8.v_Siglas,
                                                                        NroDocumentoCobranza = C.v_Mes.Trim() + "-" + C.v_Correlativo.Trim(),
                                                                        IdTipoDocumentoReferenciaCobranzaDetalle = A.i_IdTipoDocumentoRef.Value,
                                                                        NumeroDocumentoReferenciaCobranzaDetalle = J6 == null ? "" : (J6.v_Siglas.Trim() + " " + A.v_DocumentoRef.Replace(" ", "")),
                                                                        FormaPago = J9 == null ? "NO EXISTE" : J9.v_Value1,
                                                                        MontoPagadoF = C.i_IdMoneda == (int)Currency.Soles ? J6.i_UsadoDocumentoInverso == 1 ? 0 : A.d_ImporteSoles : 0,
                                                                        MontoPagadoDolaresF = C.i_IdMoneda == (int)Currency.Dolares ? J6.i_UsadoDocumentoInverso == 1 ? 0 : A.d_ImporteSoles : 0,
                                                                        FechaOrigen = a.t_FechaRegistro.Value,
                                                                        Vendedor = J10.v_NombreCompleto,
                                                                        UsuarioCreacion = J11.v_UserName,
                                                                        Retencion = A.d_MontoRetencion ?? 0,
                                                                        MonedaRetencion = C.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$.",
                                                                        DocumentoVenta = J4 == null ? "" : J4.v_Nombre,
                                                                        UsadoDocumentoInverso = J6.i_UsadoDocumentoInverso ?? 0,

                                                                    }).ToList().AsQueryable()
                                          .ToList().AsQueryable().Select(x => new ReportePlanillaCobranza
                                          {

                                              TipoDocumento = x.TipoDocumento,
                                              NroDocumento = x.NroDocumento,
                                              FechaEmision = x.FechaEmision,
                                              NombreCliente = x.NombreCliente,
                                              Cheq = x.Cheq,
                                              Moneda = x.Moneda,
                                              TotalFacturado = x.TotalFacturado,
                                              TotalFacturadoDolares = x.TotalFacturadoDolares,
                                              MontoPagado = x.MontoPagado,
                                              Saldo = x.Saldo,
                                              idTipoDocumento = x.idTipoDocumento,
                                              MedioPago = x.MedioPago,
                                              MontoCobrar = x.MontoCobrar,
                                              MontoCobrarDolares = x.MontoCobrarDolares,
                                              MontoPagadoDolares = x.MontoPagadoDolares,
                                              MonedaCobranza = x.MonedaCobranza,
                                              SaldoDolares = x.SaldoDolares,
                                              MedioPagoCobranza = x.MedioPagoCobranza,
                                              v_IdCobranzaPendiente = x.v_IdCobranzaPendiente,
                                              IdTipoDocumentoCobranzaDetalle = x.IdTipoDocumentoCobranzaDetalle,
                                              Grupollave = x.Grupollave,
                                              TipoDocumentoCobranza = x.TipoDocumentoCobranza,
                                              NroDocumentoCobranza = x.NroDocumentoCobranza,
                                              IdTipoDocumentoReferenciaCobranzaDetalle = x.IdTipoDocumentoReferenciaCobranzaDetalle,
                                              NumeroDocumentoReferenciaCobranzaDetalle = x.NumeroDocumentoReferenciaCobranzaDetalle,
                                              FormaPago = x.FormaPago,
                                              MontoPagadoF = x.MontoPagadoF,
                                              MontoPagadoDolaresF = x.MontoPagadoDolaresF,
                                              TipoDocumentoVenta = x.idTipoDocumento.Value.ToString() + " " + x.NroDocumento,
                                              FechaOrigen = x.FechaOrigen,
                                              Vendedor = x.Vendedor,
                                              UsuarioCreacion = x.UsuarioCreacion,
                                              Retencion = x.Retencion,
                                              MonedaRetencion = x.MonedaRetencion,
                                              DocumentoVenta = x.DocumentoVenta,
                                              UsadoDocumentoInverso = x.UsadoDocumentoInverso,

                                          }).ToList();

                _timer.Stop();
                var gg = _timer.Elapsed;
                List<ReportePlanillaCobranza> queryLetrasPagadas = new List<ReportePlanillaCobranza>();
                if (pstrt_IdVendedor == "-1")
                {
                    queryLetrasPagadas = (from n in dbContext.cobranzaletraspendiente

                                          join A in dbContext.cobranzadetalle on new { a = n.v_IdLetrasDetalle, eliminado = 0 } equals new { a = A.v_IdVenta, eliminado = A.i_Eliminado.Value } into A_join
                                          from A in A_join.DefaultIfEmpty()


                                          join a in dbContext.letrasdetalle on new { IdLetrasDetalle = A.v_IdVenta, eliminado = 0 } equals new { IdLetrasDetalle = a.v_IdLetrasDetalle, eliminado = a.i_Eliminado.Value } into a_join
                                          from a in a_join.DefaultIfEmpty()

                                          join C in dbContext.cobranza on new { b = A.v_IdCobranza, eliminado = 0 } equals new { b = C.v_IdCobranza, eliminado = C.i_Eliminado.Value } into C_join
                                          from C in C_join.DefaultIfEmpty()

                                          join B in dbContext.cliente on new { IdCliente = a.v_IdCliente, eliminado = 0 } equals new { IdCliente = B.v_IdCliente, eliminado = B.i_Eliminado.Value } into B_join
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

                                          join J11 in dbContext.systemuser on new { usuarioCreacion = C.i_InsertaIdUsuario.Value, eliminado = 0 } equals new { usuarioCreacion = J11.i_SystemUserId, eliminado = J11.i_IsDeleted.Value } into J11_join
                                          from J11 in J11_join.DefaultIfEmpty()
                                          where (C.t_FechaRegistro >= pstrt_FechaRegistroIni && C.t_FechaRegistro <= pstrt_FechaRegistroFin) && (C.i_IdTipoDocumento == IdTipoDocumentoCobranzaDetalle || IdTipoDocumentoCobranzaDetalle == -1)
                                          && (B.v_CodCliente == idCliente || idCliente == "") && n.i_Eliminado == 0 && A.i_Eliminado == 0 && a.i_Eliminado == 0 && C.i_Eliminado == 0
                                            && (a.v_Serie == pstrt_Serie || pstrt_Serie == "")
                                            && (a.v_Correlativo == Correlativo || Correlativo == "") && (a.i_IdTipoDocumento == TipoDocumento || TipoDocumento == -1)
                                            && C.i_IdEstado == 1
                                            && C.i_IdEstablecimiento == Establecimiento
                                            && (C.i_InsertaIdUsuario == IdUsuario || IdUsuario == -1)
                                           // && (J8.v_NroCuenta != null && J8.v_NroCuenta.StartsWith("10")) // COMO LAS VIS SE CAMBIO DE CUENTA A 169003 , YA NO APARECIAN EN EL REPORTE
                                             && (J8.v_NroCuenta != null)
                                          select new ReportePlanillaCobranza
                                          {
                                              TipoDocumento = J4.v_Siglas,
                                              NroDocumento = a.v_Serie + "-" + a.v_Correlativo,
                                              FechaEmision = C.t_FechaRegistro,
                                              NombreCliente = B == null ? "" : (B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " + B.v_SegundoNombre + " " + B.v_RazonSocial).Trim(),
                                              Cheq = J6.v_Siglas,
                                              Moneda = a.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$.",
                                              TotalFacturado = a.i_IdMoneda == (int)Currency.Soles ? n.d_Acuenta.Value + n.d_Saldo.Value : 0,
                                              TotalFacturadoDolares = a.i_IdMoneda == (int)Currency.Dolares ? n.d_Acuenta.Value + n.d_Saldo.Value : 0,
                                              MontoPagado = C.i_IdMoneda == (int)Currency.Soles ? A.d_ImporteSoles : 0,
                                              Saldo = a.i_IdMoneda == (int)Currency.Soles ? A.d_NetoXCobrar - A.d_ImporteSoles : 0,
                                              idTipoDocumento = a.i_IdTipoDocumento,
                                              MedioPago = "LETRA DE CAMBIO",
                                              MontoCobrar = A.d_NetoXCobrar - (A.d_ImporteSoles + (A.d_MontoRetencion ?? 0)),
                                              MontoCobrarDolares = C.i_IdMoneda == (int)Currency.Dolares ? A.d_NetoXCobrar : 0,
                                              MontoPagadoDolares = C.i_IdMoneda == (int)Currency.Dolares ? A.d_ImporteSoles : 0,
                                              MonedaCobranza = C.i_IdMoneda == (int)Currency.Soles ? "S/." : "US$.",
                                              SaldoDolares = C.i_IdMoneda == (int)Currency.Dolares ? A.d_NetoXCobrar - A.d_ImporteSoles : 0,
                                              MedioPagoCobranza = C.i_IdMedioPago,
                                              v_IdCobranzaPendiente = n.v_IdCobranzaLetrasPendiente,
                                              IdTipoDocumentoCobranzaDetalle = A.i_IdTipoDocumentoRef,
                                              Grupollave = pstr_grupollave == "NRODOCUMENTOCOBRANZA" ? J8 == null ? "NRO. DOCUMENTO NO EXISTE" : "DOCUMENTO DE COBRANZA :" + J8.v_Nombre : pstr_grupollave == "MEDIOPAGO" ? "MEDIO PAGO :LETRA " : pstr_grupollave == "TIPODOCUMENTOVENTA" ? "TIPO DE COMPROBANTE :  LETRA " : "",
                                              TipoDocumentoCobranza = J8.v_Siglas,
                                              NroDocumentoCobranza = C.v_Mes.Trim() + "-" + C.v_Correlativo.Trim(),
                                              IdTipoDocumentoReferenciaCobranzaDetalle = A.i_IdTipoDocumentoRef.Value,
                                              NumeroDocumentoReferenciaCobranzaDetalle = J6 == null ? "" : (J6.v_Siglas.Trim() + " " + A.v_DocumentoRef.Replace(" ", "")),
                                              FormaPago = J9 == null ? "NO EXISTE" : J9.v_Value1,
                                              MontoPagadoF = C.i_IdMoneda == (int)Currency.Soles ? J6.i_UsadoDocumentoInverso == 1 ? 0 : A.d_ImporteSoles : 0,
                                              MontoPagadoDolaresF = C.i_IdMoneda == (int)Currency.Dolares ? J6.i_UsadoDocumentoInverso == 1 ? 0 : A.d_ImporteSoles : 0,

                                              FechaOrigen = a.t_FechaEmision.Value,
                                              Retencion = A.d_MontoRetencion ?? 0,
                                              MonedaRetencion = C.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$.",
                                              UsuarioCreacion = J11.v_UserName,
                                              DocumentoVenta = "",
                                              UsadoDocumentoInverso = 0,

                                          }).ToList().AsQueryable().ToList().AsQueryable().Select(x => new ReportePlanillaCobranza
                                               {
                                                   TipoDocumento = x.TipoDocumento,
                                                   NroDocumento = x.NroDocumento,
                                                   FechaEmision = x.FechaEmision,
                                                   NombreCliente = x.NombreCliente,
                                                   Cheq = x.Cheq,
                                                   Moneda = x.Moneda,
                                                   TotalFacturado = x.TotalFacturado,
                                                   TotalFacturadoDolares = x.TotalFacturadoDolares,
                                                   MontoPagado = x.MontoPagado,
                                                   Saldo = x.Saldo,
                                                   idTipoDocumento = x.idTipoDocumento,
                                                   MedioPago = x.MedioPago,
                                                   MontoCobrar = x.MontoCobrar,
                                                   MontoCobrarDolares = x.MontoCobrarDolares,
                                                   MontoPagadoDolares = x.MontoPagadoDolares,
                                                   MonedaCobranza = x.MonedaCobranza,
                                                   SaldoDolares = x.SaldoDolares,
                                                   MedioPagoCobranza = x.MedioPagoCobranza,
                                                   v_IdCobranzaPendiente = x.v_IdCobranzaPendiente,
                                                   IdTipoDocumentoCobranzaDetalle = x.IdTipoDocumentoCobranzaDetalle,
                                                   Grupollave = x.Grupollave,
                                                   TipoDocumentoCobranza = x.TipoDocumentoCobranza,
                                                   NroDocumentoCobranza = x.NroDocumentoCobranza,
                                                   IdTipoDocumentoReferenciaCobranzaDetalle = x.IdTipoDocumentoReferenciaCobranzaDetalle,
                                                   NumeroDocumentoReferenciaCobranzaDetalle = x.NumeroDocumentoReferenciaCobranzaDetalle,
                                                   FormaPago = x.FormaPago,
                                                   MontoPagadoF = x.MontoPagadoF,
                                                   MontoPagadoDolaresF = x.MontoPagadoDolaresF,
                                                   TipoDocumentoVenta = x.idTipoDocumento.Value.ToString() + " " + x.NroDocumento,
                                                   FechaOrigen = x.FechaOrigen,
                                                   Retencion = x.Retencion,
                                                   UsuarioCreacion = x.UsuarioCreacion,
                                                   MonedaRetencion = x.MonedaRetencion,
                                                   DocumentoVenta = x.DocumentoVenta,
                                                   UsadoDocumentoInverso = 0,
                                               }).ToList();

                    if (!string.IsNullOrEmpty(Orden))
                    {
                        queryVentasPagadas = queryVentasPagadas.Concat(queryLetrasPagadas).AsQueryable().OrderBy(Orden).ToList();
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(Orden))
                    {
                        queryVentasPagadas = queryVentasPagadas.AsQueryable().OrderBy(Orden).ToList();
                    }
                }

                if (SoloResumen)
                {
                    List<ReportePlanillaCobranza> ReporteR1 = new List<ReportePlanillaCobranza>();
                    List<ReportePlanillaCobranza> ReporteR2 = new List<ReportePlanillaCobranza>();
                    List<ReportePlanillaCobranza> ReporteR1Inversos = new List<ReportePlanillaCobranza>();
                    List<ReportePlanillaCobranza> ReporteR2Inversos = new List<ReportePlanillaCobranza>();
                    var hhh = queryVentasPagadas.Where(l => l.idTipoDocumento > 1 && l.idTipoDocumento < 99).GroupBy(k => new { k.FormaPago }).ToList();


                    if (pstr_grupollave == "")
                    {

                        ReporteR1 = queryVentasPagadas.Where(l => l.idTipoDocumento >= 1 && l.idTipoDocumento <= 99 && l.UsadoDocumentoInverso == 0).GroupBy(k => new { Pago = k.FormaPago, MedioPado = k.MedioPago }).ToList().Select(p =>
                        {
                            var k = p.FirstOrDefault();
                            k.MontoPagado = p.Sum(l => l.MontoPagadoF);
                            k.Grupollave = Globals.ClientSession.v_RucEmpresa == Constants.RucNotariaBecerrSosaya ? "NOTARIALES" : "CONTABLES";
                            return k;

                        }).ToList();

                        ReporteR1Inversos = queryVentasPagadas.Where(l => l.idTipoDocumento >= 1 && l.idTipoDocumento <= 99 && l.UsadoDocumentoInverso == 1).GroupBy(k => new { Pago = k.FormaPago, MedioPado = k.MedioPago }).ToList().Select(p =>
                        {
                            var k = p.FirstOrDefault();
                            k.MontoPagado = p.Sum(o => o.MontoPagado); //p.Sum(l => l.MontoPagadoF);
                            k.Grupollave = Globals.ClientSession.v_RucEmpresa == Constants.RucNotariaBecerrSosaya ? "NOTARIALES" : "CONTABLES";
                            return k;

                        }).ToList();


                        ReporteR2 = queryVentasPagadas.Where(l => l.idTipoDocumento > 99 && l.UsadoDocumentoInverso == 0).GroupBy(k => new { Pago = k.FormaPago, MedioPago = k.MedioPago }).ToList().Select(p =>
                        {
                            var k = p.FirstOrDefault();
                            k.MontoPagado = p.Sum(l => l.MontoPagadoF);
                            k.Grupollave = Globals.ClientSession.v_RucEmpresa == Constants.RucNotariaBecerrSosaya ? "REGISTRALES" : "OTROS";
                            return k;

                        }).ToList();

                        ReporteR2Inversos = queryVentasPagadas.Where(l => l.idTipoDocumento > 99 && l.UsadoDocumentoInverso == 1).GroupBy(k => new { Pago = k.FormaPago, MedioPago = k.MedioPago }).ToList().Select(p =>
                        {
                            var k = p.FirstOrDefault();
                            k.MontoPagado = p.Sum(o => o.MontoPagado);// p.Sum(l => l.MontoPagadoF);
                            k.Grupollave = Globals.ClientSession.v_RucEmpresa == Constants.RucNotariaBecerrSosaya ? "REGISTRALES" : "OTROS";
                            return k;

                        }).ToList();
                    }
                    else
                    {
                        ReporteR1 = queryVentasPagadas.Where(l => l.idTipoDocumento >= 1 && l.idTipoDocumento <= 99 && l.UsadoDocumentoInverso == 0).GroupBy(k => new { Pago = k.FormaPago, MedioPado = k.MedioPago, DocVenta = k.DocumentoVenta }).ToList().Select(p =>
                            {
                                var k = p.FirstOrDefault();
                                k.MontoPagado = p.Sum(l => l.MontoPagadoF);
                                k.Grupollave = Globals.ClientSession.v_RucEmpresa == Constants.RucNotariaBecerrSosaya ? "NOTARIALES" : "CONTABLES";
                                return k;

                            }).ToList();


                        ReporteR2 = queryVentasPagadas.Where(l => l.idTipoDocumento > 99 && l.UsadoDocumentoInverso == 0).GroupBy(k => new { Pago = k.FormaPago, MedioPago = k.MedioPago, DocVenta = k.DocumentoVenta }).ToList().Select(p =>
                        {
                            var k = p.FirstOrDefault();
                            k.MontoPagado = p.Sum(l => l.MontoPagadoF);
                            k.Grupollave = Globals.ClientSession.v_RucEmpresa == Constants.RucNotariaBecerrSosaya ? "REGISTRALES" : "OTROS";
                            return k;

                        }).ToList();



                        ReporteR1Inversos = queryVentasPagadas.Where(l => l.idTipoDocumento >= 1 && l.idTipoDocumento <= 99 && l.UsadoDocumentoInverso == 1).GroupBy(k => new { Pago = k.FormaPago, MedioPado = k.MedioPago, DocVenta = k.DocumentoVenta }).ToList().Select(p =>
                          {
                              var k = p.FirstOrDefault();
                              k.MontoPagado = p.Sum(o => o.MontoPagado);// p.Sum(l => l.MontoPagadoF);
                              k.Grupollave = Globals.ClientSession.v_RucEmpresa == Constants.RucNotariaBecerrSosaya ? "NOTARIALES" : "CONTABLES";
                              return k;

                          }).ToList();


                        ReporteR2Inversos = queryVentasPagadas.Where(l => l.idTipoDocumento > 99 && l.UsadoDocumentoInverso == 1).GroupBy(k => new { Pago = k.FormaPago, MedioPago = k.MedioPago, DocVenta = k.DocumentoVenta }).ToList().Select(p =>
                        {
                            var k = p.FirstOrDefault();
                            k.MontoPagado = p.Sum(o => o.MontoPagado);// p.Sum(l => l.MontoPagadoF);
                            k.Grupollave = Globals.ClientSession.v_RucEmpresa == Constants.RucNotariaBecerrSosaya ? "REGISTRALES" : "OTROS";
                            return k;

                        }).ToList();


                    }
                    objOperationResult.Success = 1;
                    return ReporteR1.Concat(ReporteR2).Concat(ReporteR1Inversos).Concat(ReporteR2Inversos).ToList();


                }
                objOperationResult.Success = 1;
                List<ReportePlanillaCobranza> objData = queryVentasPagadas.ToList();
                return objData;

            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "CobranzaBL.ReportePlanillaCobranzas()";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;

            }
        }
        public List<ReporteCuentasPorCobrar> ReporteCuentaspoCobrar(ref OperationResult objOperationResult, DateTime pstrt_FechaRegistroIni, DateTime pstrt_FechaRegistroFin, string Orden, string pstr_grupollave, string pstr_Nombregrupollave, string Filtro, bool IncluirLetraCambio, bool RangoFecha,int Establecimiento,int CondicionPago)
        {
            NodeBL objNodeBL = new NodeBL();
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                List<ReporteCuentasPorCobrar> queryLetrasDetalle = new List<ReporteCuentasPorCobrar>();
                List<ReporteCuentasPorCobrar> queryLetrasIniciales = new List<ReporteCuentasPorCobrar>();
                SystemParameterBL _objSystemParameterBL = new SystemParameterBL();
                var Ubigeo = _objSystemParameterBL.GetSystemParameterForCombo(ref objOperationResult, 112, "");

                List<letrascanje> LetritasCanje = dbContext.letrascanje.Where(l => l.i_Eliminado == 0).ToList();
                List<venta> Ventitas = dbContext.venta.Where(l => l.i_Eliminado == 0).ToList();
                List<documento> documentitos = dbContext.documento.Where(l => l.i_Eliminado == 0).ToList();
                List<letrasdetalle> Ld = dbContext.letrasdetalle.Where(l => l.i_Eliminado == 0).ToList();
                if (RangoFecha)
                {
                    #region ConRangoFechas
                    var queryVentas = (from n in dbContext.cobranzapendiente

                                       join A in dbContext.venta on new { IdVenta = n.v_IdVenta, eliminado = 0 } equals new { IdVenta = A.v_IdVenta, eliminado = A.i_Eliminado.Value } into A_join
                                       from A in A_join.DefaultIfEmpty()

                                       join B in dbContext.cliente on new { IdCliente = A.v_IdCliente, eliminado = 0 } equals new { IdCliente = B.v_IdCliente, eliminado = B.i_Eliminado.Value } into B_join
                                       from B in B_join.DefaultIfEmpty()

                                       join C in dbContext.vendedor on new { IdVendedor = A.v_IdVendedor, eliminado = 0 } equals new { IdVendedor = C.v_IdVendedor, eliminado = C.i_Eliminado.Value } into C_join
                                       from C in C_join.DefaultIfEmpty()

                                       join J4 in dbContext.documento on new { i_IdTipoDocumento = A.i_IdTipoDocumento.Value, eliminado = 0 }
                                                                      equals new { i_IdTipoDocumento = J4.i_CodigoDocumento, eliminado = J4.i_Eliminado.Value } into J4_join
                                       from J4 in J4_join.DefaultIfEmpty()

                                       join J5 in dbContext.datahierarchy on new { i_idTipoPago = A.i_IdCondicionPago.Value, b = 23, eliminado = 0 }
                                                                    equals new { i_idTipoPago = J5.i_ItemId, b = J5.i_GroupId, eliminado = J5.i_IsDeleted.Value } into J5_join
                                       from J5 in J5_join.DefaultIfEmpty()

                                       where n.i_Eliminado == 0 && A.t_FechaRegistro >= pstrt_FechaRegistroIni && A.t_FechaRegistro <= pstrt_FechaRegistroFin &&
                                             n.d_Saldo > 0 && A.i_IdEstado == 1
                                             && (J4.i_UsadoDocumentoInverso == null || J4.i_UsadoDocumentoInverso == 0)
                                             && (A.i_IdEstablecimiento ==Establecimiento || Establecimiento ==-1)
                                             && (A.i_IdCondicionPago == CondicionPago || CondicionPago ==-1 )

                                       select new
                                       {

                                           Correlativo = "V " + A.v_Mes.Trim() + "-" + A.v_Correlativo.Trim(),
                                           NombreCliente = B == null ? A.i_IdEstado == 0 ? "**ANULADO**" : "**NO EXISTE CLIENTE**" : (B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " + B.v_RazonSocial).Trim(),
                                           MedioPago = J5 == null ? "" : J5.v_Value1,
                                           FechaEmision = A.t_FechaRegistro.Value,
                                           TipoDocumento = J4 == null ? "" : J4.v_Siglas + " " + A.v_SerieDocumento.Trim() + "-" + A.v_CorrelativoDocumento.Trim(),
                                           NroDocumento = "",
                                           GuiaRemision = A.v_NroGuiaRemisionSerie + "-" + A.v_NroGuiaRemisionCorrelativo,
                                           FechaVencimiento = A.t_FechaVencimiento.Value,
                                           Vendedor = C == null ? "**NO EXISTE VENDEDOR**" : C.v_CodVendedor.ToUpper(),
                                           Moneda = A.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$.",
                                           TotalFacturado = A.d_Total == 0 ? 0 : A.d_Total,
                                           Acuenta = A.d_Total == 0 ? 0 : n.d_Acuenta.Value,
                                           Saldo = A.d_Total == 0 ? 0 : A.i_IdMoneda == (int)Currency.Soles ? J4 != null && J4.i_UsadoDocumentoInverso == 1 ? n.d_Saldo * -1 : n.d_Saldo : 0,
                                           SaldoDolares = A.d_Total == 0 ? 0 : A.i_IdMoneda == (int)Currency.Dolares ? J4 != null && J4.i_UsadoDocumentoInverso == 1 ? n.d_Saldo * -1 : n.d_Saldo : 0,
                                           MonedaCobranza = A.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$.",
                                           Grupollave = pstr_grupollave == "NOMBRECLIENTE" ? B.v_IdCliente == null ? "** " + pstr_Nombregrupollave + " NO EXISTE **" : pstr_Nombregrupollave + " : " + (B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " + B.v_RazonSocial).Trim().ToUpper() : "" + pstr_grupollave == "MEDIOPAGO" ? J5.v_Value1 == null ? "** " + pstr_Nombregrupollave + " NO EXISTE **" : pstr_Nombregrupollave + " : " + J5.v_Value1 : "" + pstr_grupollave == "VENDEDOR" ? C.v_IdVendedor == null ? "** " + pstr_Nombregrupollave + " NO EXISTE **" : pstr_Nombregrupollave + " : " + C.v_NombreCompleto.Trim().ToUpper() : "",

                                           v_CodigoCliente = B == null ? "**NO EXISTE CLIENTE**" : B.v_CodCliente.Trim(),
                                           idTipoDocumento = A == null ? 0 : A.i_IdTipoDocumento.Value,
                                           v_IdVendedor = C == null ? "**NO EXISTE VENDEDOR**" : C.v_IdVendedor.Trim(),
                                           v_SerieDocumento = A.v_SerieDocumento.Trim(),
                                           v_CorrelativoDocumento = A.v_CorrelativoDocumento.Trim(),
                                           GrupoDetalle = "",
                                           iDepartamento = B.i_IdDepartamento ?? -1,
                                           idProvincia = B.i_IdProvincia ?? -1,
                                           idDistrito = B.i_IdDistrito ?? -1,
                                           NroDocCliente = B.v_NroDocIdentificacion,

                                       }).ToList().Select(p =>
                                           {
                                               var departamento = p.iDepartamento == -1 || p.iDepartamento == null ? "" : Ubigeo.Where(l => l.Id == p.iDepartamento.ToString()) != null ? Ubigeo.Where(l => l.Id == p.iDepartamento.ToString()).FirstOrDefault().Value1 : "";  //  _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, 1, 112, "").ToList().Where(a => a.Id == x.IdDepartamento.ToString()).FirstOrDefault().Value1.ToString();
                                               var provincia = p.idProvincia == -1 || p.idProvincia == null ? "" : Ubigeo.Where(l => l.Id == p.idProvincia.ToString()) != null ? Ubigeo.Where(l => l.Id == p.idProvincia.ToString()).FirstOrDefault().Value1 : "";
                                               var distrito = p.idDistrito == -1 || p.idDistrito == null ? "" : Ubigeo.Where(l => l.Id == p.idDistrito.ToString()) != null ? Ubigeo.Where(l => l.Id == p.idDistrito.ToString()).FirstOrDefault().Value1 : "";
                                               var DiasRetrasoPago = DateTime.Now.Subtract(p.FechaVencimiento).Days;
                                               var DiasFaltantesVencimiento = p.FechaVencimiento.Subtract(DateTime.Now).Days ;
                                               
                                               return new ReporteCuentasPorCobrar
                                           {
                                               Correlativo = p.Correlativo,
                                               NombreCliente = p.NombreCliente,
                                               MedioPago = p.MedioPago,
                                               FechaEmision = p.FechaEmision,
                                               TipoDocumento = p.TipoDocumento,
                                               NroDocumento = p.NroDocumento,
                                               GuiaRemision = p.GuiaRemision,
                                               FechaVencimiento = p.FechaVencimiento,
                                               Vendedor = p.Vendedor,
                                               Moneda = p.Moneda,
                                               TotalFacturado = p.TotalFacturado,
                                               Acuenta = p.Acuenta,
                                               Saldo = p.Saldo,
                                               SaldoDolares = p.SaldoDolares,
                                               MonedaCobranza = p.MonedaCobranza,
                                               Grupollave = p.Grupollave,
                                               v_CodigoCliente = p.v_CodigoCliente,
                                               idTipoDocumento = p.idTipoDocumento,
                                               v_IdVendedor = p.v_IdVendedor,
                                               v_SerieDocumento = p.v_SerieDocumento,
                                               v_CorrelativoDocumento = p.v_CorrelativoDocumento,
                                               GrupoDetalle = p.GrupoDetalle,
                                               Departamento = departamento,
                                               Provincia = provincia,
                                               Distrito = distrito,
                                               NroDocCliente = p.NroDocCliente,
                                               sFechaEmision =p.FechaEmision.Day.ToString ("00")+"/"+ p.FechaEmision.Month.ToString ("00")+"/"+ p.FechaEmision.Year.ToString().Substring (2,2),
                                               sFechaVencimiento = p.FechaVencimiento.Day.ToString("00") + "/" + p.FechaVencimiento.Month.ToString("00") + "/" + p.FechaVencimiento.Year.ToString().Substring(2, 2),
                                               DiasRetrasoPago = DiasRetrasoPago <0 ? "0": DiasRetrasoPago.ToString (),
                                               DiasFaltantesVencimiento =DiasFaltantesVencimiento.ToString (), 
                                           };
                                           }).AsQueryable();

                    if (!string.IsNullOrEmpty(Filtro))
                    {
                        queryVentas = queryVentas.Where(Filtro);
                    }


                    if (IncluirLetraCambio)
                    {

                        var queryLetrasDetalleConsulta = (from a in dbContext.cobranzaletraspendiente

                                                          join b in dbContext.letrasdetalle on new { IdLetrasDetalle = a.v_IdLetrasDetalle, eliminado = 0 } equals new { IdLetrasDetalle = b.v_IdLetrasDetalle, eliminado = b.i_Eliminado.Value } into b_join
                                                          from b in b_join.DefaultIfEmpty()

                                                          join c in dbContext.cliente on new { IdCliente = b.v_IdCliente, eliminado = 0 } equals new { IdCliente = c.v_IdCliente, eliminado = c.i_Eliminado.Value } into c_join
                                                          from c in c_join.DefaultIfEmpty()

                                                          join d in dbContext.documento on new { DocumentoLetraDet = b.i_IdTipoDocumento.Value, eliminado = 0 } equals new { DocumentoLetraDet = d.i_CodigoDocumento, eliminado = d.i_Eliminado.Value } into d_join

                                                          from d in d_join.DefaultIfEmpty()

                                                          where a.i_Eliminado == 0 && b.t_FechaEmision >= pstrt_FechaRegistroIni && b.t_FechaEmision <= pstrt_FechaRegistroFin &&
                                                           a.d_Saldo > 0// && b.i_EsSaldoInicial == 0


                                                          select new
                                                          {
                                                              Correlativo = b.i_EsSaldoInicial == 1 ? "SALDO INICIAL" : d.v_Siglas + " " + b.v_Serie.Trim() + "-" + b.v_Correlativo.Trim(),
                                                              NombreCliente = c == null ? "**NO EXISTE CLIENTE**" : (c.v_ApePaterno + " " + c.v_ApeMaterno + " " + c.v_PrimerNombre + c.v_RazonSocial).Trim(),
                                                              MedioPago = "LETRA DE CAMBIO",
                                                              FechaEmision = b.t_FechaEmision.Value,
                                                              //TipoDocumento = d == null ? "" : b.i_EsSaldoInicial == 1 ? d.v_Siglas.Trim() + " " + b.v_Serie.Trim() + "-" + b.v_Correlativo.Trim() : "    " + d.v_Siglas.Trim() + " " + b.v_Serie.Trim() + "-" + b.v_Correlativo.Trim(),
                                                              TipoDocumento = d == null ? "" : b.i_EsSaldoInicial == 1 ? d.v_Siglas.Trim() + " " + b.v_Serie.Trim() + "-" + b.v_Correlativo.Trim() : d.v_Siglas.Trim() + " " + b.v_Serie.Trim() + "-" + b.v_Correlativo.Trim(),
                                                              NroDocumento = "",
                                                              GuiaRemision = "",
                                                              FechaVencimiento = b.t_FechaVencimiento.Value,
                                                              Vendedor = "",
                                                              Moneda = b.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$.",
                                                              TotalFacturado = a.d_Saldo.Value + a.d_Acuenta.Value,
                                                              Acuenta = a.d_Acuenta.Value,
                                                              Saldo = b.i_IdMoneda == (int)Currency.Soles ? a.d_Saldo : 0,
                                                              SaldoDolares = b.i_IdMoneda == (int)Currency.Dolares ? a.d_Saldo : 0,
                                                              MonedaCobranza = b.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$.",
                                                              Grupollave = pstr_grupollave == "NOMBRECLIENTE" ? c.v_IdCliente == null ? "** CLIENTE  NO EXISTE **" : "CLIENTE : " + (c.v_ApePaterno + " " + c.v_ApeMaterno + " " + c.v_PrimerNombre + " " + c.v_RazonSocial).Trim() :
                                                              pstr_grupollave == "MEDIOPAGO" ? "COND. PAGO : LETRA" : pstr_Nombregrupollave == "VENDEDOR" ? "** VENDEDOR NO EXISTE PARA LETRAS **" : "",
                                                              //NombreGrupo = pstr_Nombregrupollave,
                                                              v_CodigoCliente = c == null ? "**NO EXISTE CLIENTE**" : c.v_CodCliente.Trim(),
                                                              idTipoDocumento = d == null ? 0 : d.i_CodigoDocumento,
                                                              v_IdVendedor = "",
                                                              v_SerieDocumento = b.v_Serie,
                                                              v_CorrelativoDocumento = b.v_Correlativo,
                                                              v_IdLetraDetalle = b.v_IdLetrasDetalle,
                                                              v_IdLetras = b.v_IdLetras,

                                                              GrupoDetalle = "",
                                                              NroDocCliente = c.v_NroDocIdentificacion,
                                                              iDepartamento = c.i_IdDepartamento,
                                                              idDistrito = c.i_IdDistrito,
                                                              idProvincia = c.i_IdProvincia,


                                                          }).ToList().Select(x =>
                                                  {
                                                      var departamento = x.iDepartamento == -1 || x.iDepartamento == null ? "" : Ubigeo.Where(l => l.Id == x.iDepartamento.ToString()) != null ? Ubigeo.Where(l => l.Id == x.iDepartamento.ToString()).FirstOrDefault().Value1 : "";  //  _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, 1, 112, "").ToList().Where(a => a.Id == x.IdDepartamento.ToString()).FirstOrDefault().Value1.ToString();
                                                      var provincia = x.idProvincia == -1 || x.idProvincia == null ? "" : Ubigeo.Where(l => l.Id == x.idProvincia.ToString()) != null ? Ubigeo.Where(l => l.Id == x.idProvincia.ToString()).FirstOrDefault().Value1 : "";
                                                      var distrito = x.idDistrito == -1 || x.idDistrito == null ? "" : Ubigeo.Where(l => l.Id == x.idDistrito.ToString()) != null ? Ubigeo.Where(l => l.Id == x.idDistrito.ToString()).FirstOrDefault().Value1 : "";
                                                      var DiasRetrasoPago = DateTime.Now.Subtract(x.FechaVencimiento).Days;
                                                      var DiasFaltantesVencimiento = x.FechaVencimiento.Subtract(DateTime.Now).Days;
                                                      return new ReporteCuentasPorCobrar
                                                  {
                                                      Correlativo = x.Correlativo,
                                                      NombreCliente = x.NombreCliente,
                                                      MedioPago = x.MedioPago,
                                                      FechaEmision = x.FechaEmision,
                                                      TipoDocumento = x.TipoDocumento,
                                                      NroDocumento = x.NroDocumento,
                                                      GuiaRemision = x.GuiaRemision,
                                                      FechaVencimiento = x.FechaVencimiento,
                                                      Vendedor = x.Vendedor,
                                                      Moneda = x.Moneda,
                                                      TotalFacturado = x.TotalFacturado,
                                                      Acuenta = x.Acuenta,
                                                      Saldo = x.Saldo,
                                                      SaldoDolares = x.SaldoDolares,
                                                      MonedaCobranza = x.MonedaCobranza,
                                                      Grupollave = x.Grupollave,
                                                      v_CodigoCliente = x.v_CodigoCliente,
                                                      idTipoDocumento = x.idTipoDocumento,
                                                      v_IdVendedor = x.v_IdVendedor,
                                                      v_SerieDocumento = x.v_SerieDocumento,
                                                      v_CorrelativoDocumento = x.v_CorrelativoDocumento,
                                                      NroDocCliente = x.NroDocCliente,
                                                      Departamento = departamento,
                                                      Provincia = provincia,
                                                      Distrito = distrito,
                                                      sFechaEmision = x.FechaEmision.Day.ToString("00") + "/" + x.FechaEmision.Month.ToString("00") + "/" + x.FechaEmision.Year.ToString().Substring(2, 2),
                                                      sFechaVencimiento = x.FechaVencimiento.Day.ToString("00") + "/" + x.FechaVencimiento.Month.ToString("00") + "/" + x.FechaVencimiento.Year.ToString().Substring(2, 2),
                                                      DiasRetrasoPago = DiasRetrasoPago <0 ?"0": DiasRetrasoPago.ToString (),
                                                      DiasFaltantesVencimiento = DiasFaltantesVencimiento.ToString (), 
                                                  };

                                                  }).ToList().AsQueryable();

                        if (!string.IsNullOrEmpty(Filtro))
                        {

                            queryLetrasDetalleConsulta = queryLetrasDetalleConsulta.Where(Filtro);
                        }

                        queryLetrasDetalle = queryLetrasDetalleConsulta.ToList();
                    }

                    var queryFinal = queryVentas.Concat(queryLetrasDetalle).Concat(queryLetrasIniciales);

                    if (!string.IsNullOrEmpty(Orden))
                    {

                        queryFinal = queryFinal.OrderBy(Orden);
                    }

                    objOperationResult.Success = 1;

                    List<ReporteCuentasPorCobrar> objData = queryFinal.ToList();
                    return objData;

                    #endregion
                }
                else
                {
                    #region SinRangoFechas
                    var queryVentas = (from n in dbContext.cobranzapendiente

                                       join A in dbContext.venta on new { IdVenta = n.v_IdVenta, eliminado = 0 } equals new { IdVenta = A.v_IdVenta, eliminado = A.i_Eliminado.Value } into A_join
                                       from A in A_join.DefaultIfEmpty()

                                       join B in dbContext.cliente on new { IdCliente = A.v_IdCliente, eliminado = 0 } equals new { IdCliente = B.v_IdCliente, eliminado = B.i_Eliminado.Value } into B_join
                                       from B in B_join.DefaultIfEmpty()

                                       join C in dbContext.vendedor on new { IdVendedor = A.v_IdVendedor, eliminado = 0 } equals new { IdVendedor = C.v_IdVendedor, eliminado = C.i_Eliminado.Value } into C_join
                                       from C in C_join.DefaultIfEmpty()

                                       join J4 in dbContext.documento on new { i_IdTipoDocumento = A.i_IdTipoDocumento.Value, eliminado = 0 }
                                                                      equals new { i_IdTipoDocumento = J4.i_CodigoDocumento, eliminado = J4.i_Eliminado.Value } into J4_join
                                       from J4 in J4_join.DefaultIfEmpty()

                                       join J5 in dbContext.datahierarchy on new { i_idTipoPago = A.i_IdCondicionPago.Value, b = 23, eliminado = 0 }
                                                                    equals new { i_idTipoPago = J5.i_ItemId, b = J5.i_GroupId, eliminado = J5.i_IsDeleted.Value } into J5_join
                                       from J5 in J5_join.DefaultIfEmpty()

                                       where n.i_Eliminado == 0 && A != null && A.v_IdVenta != null &&
                                             n.d_Saldo > 0 && A.i_IdEstado == 1
                                            && (J4.i_UsadoDocumentoInverso == null || J4.i_UsadoDocumentoInverso == 0)
                                            && (A.i_IdEstablecimiento ==Establecimiento || Establecimiento ==-1)
                                            && (A.i_IdCondicionPago == CondicionPago || CondicionPago == -1)

                                       select new
                                       {

                                           Correlativo = "V " + A.v_Mes.Trim() + "-" + A.v_Correlativo.Trim(),
                                           NombreCliente = B == null ? A.i_IdEstado == 0 ? "**ANULADO**" : "**NO EXISTE CLIENTE**" : (B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " + B.v_RazonSocial).Trim(),
                                           MedioPago = J5 == null ? "" : J5.v_Value1,
                                           FechaEmision = A.t_FechaRegistro.Value,
                                           TipoDocumento = J4 == null ? "" : J4.v_Siglas + " " + A.v_SerieDocumento.Trim() + "-" + A.v_CorrelativoDocumento.Trim(),
                                           NroDocumento = "",
                                           GuiaRemision = A.v_NroGuiaRemisionSerie + "-" + A.v_NroGuiaRemisionCorrelativo,
                                           FechaVencimiento = A.t_FechaVencimiento.Value,
                                           Vendedor = C == null ? "**NO EXISTE VENDEDOR**" : C.v_CodVendedor.ToUpper(),
                                           Moneda = A.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$.",
                                           TotalFacturado = A.i_IdEstado == 0 ? 0 : A.d_Total,
                                           Acuenta = A.i_IdEstado == 0 ? 0 : n.d_Acuenta.Value,
                                           Saldo = A.i_IdMoneda == (int)Currency.Soles ? J4 != null && J4.i_UsadoDocumentoInverso == 1 ? n.d_Saldo * -1 : n.d_Saldo : 0,
                                           SaldoDolares = A.i_IdMoneda == (int)Currency.Dolares ? J4 != null && J4.i_UsadoDocumentoInverso == 1 ? n.d_Saldo * -1 : n.d_Saldo : 0,
                                           MonedaCobranza = A.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$.",
                                           Grupollave = pstr_grupollave == "NOMBRECLIENTE" ? B.v_IdCliente == null ? "** " + pstr_Nombregrupollave + " NO EXISTE **" : pstr_Nombregrupollave + " : " + (B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " + B.v_RazonSocial).Trim().ToUpper() : "" + pstr_grupollave == "MEDIOPAGO" ? J5.v_Value1 == null ? "** " + pstr_Nombregrupollave + " NO EXISTE **" : pstr_Nombregrupollave + " : " + J5.v_Value1 : "" + pstr_grupollave == "VENDEDOR" ? C.v_IdVendedor == null ? "** " + pstr_Nombregrupollave + " NO EXISTE **" : pstr_Nombregrupollave + " : " + C.v_NombreCompleto.Trim().ToUpper() : "",
                                           v_CodigoCliente = B == null ? "**NO EXISTE CLIENTE**" : B.v_CodCliente.Trim(),
                                           idTipoDocumento = A == null ? 0 : A.i_IdTipoDocumento.Value,
                                           v_IdVendedor = C == null ? "**NO EXISTE VENDEDOR**" : C.v_IdVendedor.Trim(),
                                           v_SerieDocumento = A.v_SerieDocumento.Trim(),
                                           v_CorrelativoDocumento = A.v_CorrelativoDocumento.Trim(),
                                           GrupoDetalle = "",
                                           iDepartamento = B.i_IdDepartamento ?? -1,
                                           idProvincia = B.i_IdProvincia ?? -1,
                                           idDistrito = B.i_IdDistrito ?? -1,
                                           NroDocCliente = B.v_NroDocIdentificacion,
                                           Procedencia = A.v_IdVenta,

                                       }).ToList().Select(p =>
                                       {
                                           var departamento = p.iDepartamento == -1 || p.iDepartamento == null ? "" : Ubigeo.Where(l => l.Id == p.iDepartamento.ToString()) != null ? Ubigeo.Where(l => l.Id == p.iDepartamento.ToString()).FirstOrDefault().Value1 : "";  //  _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, 1, 112, "").ToList().Where(a => a.Id == x.IdDepartamento.ToString()).FirstOrDefault().Value1.ToString();
                                           var provincia = p.idProvincia == -1 || p.idProvincia == null ? "" : Ubigeo.Where(l => l.Id == p.idProvincia.ToString()) != null ? Ubigeo.Where(l => l.Id == p.idProvincia.ToString()).FirstOrDefault().Value1 : "";
                                           var distrito = p.idDistrito == -1 || p.idDistrito == null ? "" : Ubigeo.Where(l => l.Id == p.idDistrito.ToString()) != null ? Ubigeo.Where(l => l.Id == p.idDistrito.ToString()).FirstOrDefault().Value1 : "";
                                           var DiasRetrasoPago = DateTime.Now.Subtract(p.FechaVencimiento).Days;
                                           var DiasFaltantesVencimiento =p.FechaVencimiento.Subtract(DateTime.Now).Days;
                                           return new ReporteCuentasPorCobrar
                                           {
                                               Correlativo = p.Correlativo,
                                               NombreCliente = p.NombreCliente,
                                               MedioPago = p.MedioPago,
                                               FechaEmision = p.FechaEmision,
                                               TipoDocumento = p.TipoDocumento,
                                               NroDocumento = p.NroDocumento,
                                               GuiaRemision = p.GuiaRemision,
                                               FechaVencimiento = p.FechaVencimiento,
                                               Vendedor = p.Vendedor,
                                               Moneda = p.Moneda,
                                               TotalFacturado = p.TotalFacturado,
                                               Acuenta = p.Acuenta,
                                               Saldo = p.Saldo,
                                               SaldoDolares = p.SaldoDolares,
                                               MonedaCobranza = p.MonedaCobranza,
                                               Grupollave = p.Grupollave,
                                               v_CodigoCliente = p.v_CodigoCliente,
                                               idTipoDocumento = p.idTipoDocumento,
                                               v_IdVendedor = p.v_IdVendedor,
                                               v_SerieDocumento = p.v_SerieDocumento,
                                               v_CorrelativoDocumento = p.v_CorrelativoDocumento,
                                               GrupoDetalle = p.GrupoDetalle,
                                               Departamento = departamento,
                                               Provincia = provincia,
                                               Distrito = distrito,
                                               NroDocCliente = p.NroDocCliente,
                                               Procedencia = p.Procedencia,
                                               sFechaEmision = p.FechaEmision.Day.ToString("00") + "/" + p.FechaEmision.Month.ToString("00") + "/" + p.FechaEmision.Year.ToString().Substring(2, 2),
                                               sFechaVencimiento = p.FechaVencimiento.Day.ToString("00") + "/" + p.FechaVencimiento.Month.ToString("00") + "/" + p.FechaVencimiento.Year.ToString().Substring(2, 2),
                                               DiasRetrasoPago =  DiasRetrasoPago <0 ? "0 ": DiasRetrasoPago.ToString () ,
                                               DiasFaltantesVencimiento = DiasFaltantesVencimiento.ToString () ,
                                               i_Retraso = DiasRetrasoPago>0 ?1 :0,
                                           };
                                       }).AsQueryable();

                    if (!string.IsNullOrEmpty(Filtro))
                    {
                        queryVentas = queryVentas.Where(Filtro);
                    }


                    if (IncluirLetraCambio)
                    {


                        var queryLetrasDetalleConsulta = (from a in dbContext.cobranzaletraspendiente

                                                          join b in dbContext.letrasdetalle on new { IdLetrasDetalle = a.v_IdLetrasDetalle, eliminado = 0 } equals new { IdLetrasDetalle = b.v_IdLetrasDetalle, eliminado = b.i_Eliminado.Value } into b_join
                                                          from b in b_join.DefaultIfEmpty()

                                                          join c in dbContext.cliente on new { IdCliente = b.v_IdCliente, eliminado = 0 } equals new { IdCliente = c.v_IdCliente, eliminado = c.i_Eliminado.Value } into c_join
                                                          from c in c_join.DefaultIfEmpty()

                                                          join d in dbContext.documento on new { DocumentoLetraDet = b.i_IdTipoDocumento.Value, eliminado = 0 } equals new { DocumentoLetraDet = d.i_CodigoDocumento, eliminado = d.i_Eliminado.Value } into d_join

                                                          from d in d_join.DefaultIfEmpty()

                                                          where a.i_Eliminado == 0 && b != null && b.v_IdLetrasDetalle != null &&
                                                           a.d_Saldo > 0

                                                          select new
                                                          {
                                                              Correlativo = b.i_EsSaldoInicial == 1 ? "SALDO INICIAL" : d.v_Siglas + " " + b.v_Serie.Trim() + "-" + b.v_Correlativo.Trim(),
                                                              NombreCliente = c == null ? "**NO EXISTE CLIENTE**" : (c.v_ApePaterno + " " + c.v_ApeMaterno + " " + c.v_PrimerNombre + c.v_RazonSocial).Trim(),
                                                              MedioPago = "LETRA DE CAMBIO",
                                                              FechaEmision = b.t_FechaEmision.Value,
                                                              TipoDocumento = d == null ? "" : b.i_EsSaldoInicial == 1 ? d.v_Siglas.Trim() + " " + b.v_Serie.Trim() + "-" + b.v_Correlativo.Trim() : d.v_Siglas.Trim() + " " + b.v_Serie.Trim() + "-" + b.v_Correlativo.Trim(),
                                                              NroDocumento = "",
                                                              GuiaRemision = "",
                                                              FechaVencimiento = b.t_FechaVencimiento.Value,
                                                              Vendedor = "",
                                                              Moneda = b.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$.",
                                                              TotalFacturado = a.d_Saldo.Value + a.d_Acuenta.Value,
                                                              Acuenta = a.d_Acuenta.Value,
                                                              Saldo = b.i_IdMoneda == (int)Currency.Soles ? a.d_Saldo : 0,
                                                              SaldoDolares = b.i_IdMoneda == (int)Currency.Dolares ? a.d_Saldo : 0,
                                                              MonedaCobranza = b.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$.",
                                                              Grupollave = pstr_grupollave == "NOMBRECLIENTE" ? c.v_IdCliente == null ? "** CLIENTE  NO EXISTE **" : "CLIENTE : " + (c.v_ApePaterno + " " + c.v_ApeMaterno + " " + c.v_PrimerNombre + " " + c.v_RazonSocial).Trim() :
                                                              pstr_grupollave == "MEDIOPAGO" ? "COND. PAGO : LETRA" : pstr_Nombregrupollave == "VENDEDOR" ? "** VENDEDOR NO EXISTE PARA LETRAS **" : "",
                                                              //NombreGrupo = pstr_Nombregrupollave,
                                                              v_CodigoCliente = c == null ? "**NO EXISTE CLIENTE**" : c.v_CodCliente.Trim(),
                                                              idTipoDocumento = d == null ? 0 : d.i_CodigoDocumento,
                                                              v_IdVendedor = "",
                                                              v_SerieDocumento = b.v_Serie,
                                                              v_CorrelativoDocumento = b.v_Correlativo,
                                                              v_IdLetraDetalle = b.v_IdLetrasDetalle,
                                                              v_IdLetras = b.v_IdLetras,
                                                              GrupoDetalle = "",
                                                              NroDocCliente = c.v_NroDocIdentificacion,
                                                              iDepartamento = c.i_IdDepartamento,
                                                              idDistrito = c.i_IdDistrito,
                                                              idProvincia = c.i_IdProvincia,
                                                              Procedencia = "L",

                                                          }).ToList().Select(x =>
                                                          {
                                                              var departamento = x.iDepartamento == -1 || x.iDepartamento == null ? "" : Ubigeo.Where(l => l.Id == x.iDepartamento.ToString()) != null ? Ubigeo.Where(l => l.Id == x.iDepartamento.ToString()).FirstOrDefault().Value1 : "";  //  _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, 1, 112, "").ToList().Where(a => a.Id == x.IdDepartamento.ToString()).FirstOrDefault().Value1.ToString();
                                                              var provincia = x.idProvincia == -1 || x.idProvincia == null ? "" : Ubigeo.Where(l => l.Id == x.idProvincia.ToString()) != null ? Ubigeo.Where(l => l.Id == x.idProvincia.ToString()).FirstOrDefault().Value1 : "";
                                                              var distrito = x.idDistrito == -1 || x.idDistrito == null ? "" : Ubigeo.Where(l => l.Id == x.idDistrito.ToString()) != null ? Ubigeo.Where(l => l.Id == x.idDistrito.ToString()).FirstOrDefault().Value1 : "";
                                                              var DiasRetrasoPago = DateTime.Now.Subtract(x.FechaVencimiento).Days;
                                                              var DiasFaltantesVencimiento = x.FechaVencimiento.Subtract(DateTime.Now).Days;
                                                              return new ReporteCuentasPorCobrar
                                                              {

                                                                  Correlativo = x.Correlativo,
                                                                  NombreCliente = x.NombreCliente,
                                                                  MedioPago = x.MedioPago,
                                                                  FechaEmision = x.FechaEmision,
                                                                  TipoDocumento = x.TipoDocumento,
                                                                  NroDocumento = x.NroDocumento,
                                                                  GuiaRemision = x.GuiaRemision,
                                                                  FechaVencimiento = x.FechaVencimiento,
                                                                  Vendedor = x.Vendedor,
                                                                  Moneda = x.Moneda,
                                                                  TotalFacturado = x.TotalFacturado,
                                                                  Acuenta = x.Acuenta,
                                                                  Saldo = x.Saldo,
                                                                  SaldoDolares = x.SaldoDolares,
                                                                  MonedaCobranza = x.MonedaCobranza,
                                                                  Grupollave = x.Grupollave,
                                                                  v_CodigoCliente = x.v_CodigoCliente,
                                                                  idTipoDocumento = x.idTipoDocumento,
                                                                  v_IdVendedor = x.v_IdVendedor,
                                                                  v_SerieDocumento = x.v_SerieDocumento,
                                                                  v_CorrelativoDocumento = x.v_CorrelativoDocumento,
                                                                  NroDocCliente = x.NroDocCliente,
                                                                  Departamento = departamento,
                                                                  Provincia = provincia,
                                                                  Distrito = distrito,
                                                                  Procedencia = x.Procedencia,
                                                                  sFechaEmision = x.FechaEmision.Day.ToString("00") + "/" + x.FechaEmision.Month.ToString("00") + "/" + x.FechaEmision.Year.ToString().Substring(2, 2),
                                                                  sFechaVencimiento = x.FechaVencimiento.Day.ToString("00") + "/" + x.FechaVencimiento.Month.ToString("00") + "/" + x.FechaVencimiento.Year.ToString().Substring(2, 2),
                                                                  DiasRetrasoPago = DiasRetrasoPago <0 ?"0": DiasRetrasoPago.ToString (),
                                                                  DiasFaltantesVencimiento = DiasFaltantesVencimiento.ToString (), 
                                                              };

                                                          }).ToList().AsQueryable();


                        if (!string.IsNullOrEmpty(Filtro))
                        {

                            queryLetrasDetalleConsulta = queryLetrasDetalleConsulta.Where(Filtro);
                        }

                        queryLetrasDetalle = queryLetrasDetalleConsulta.ToList();
                    }

                    var queryFinal = queryVentas.Concat(queryLetrasDetalle).Concat(queryLetrasIniciales);

                    if (!string.IsNullOrEmpty(Orden))
                    {

                        queryFinal = queryFinal.OrderBy(Orden);
                    }

                    objOperationResult.Success = 1;

                    List<ReporteCuentasPorCobrar> objData = queryFinal.ToList();
                    return objData;
                    #endregion
                }

            }
            catch (Exception)
            {
                objOperationResult.Success = 0;

                return null;
            }
        }




        public List<ReporteCuentasPorCobrar> ReporteCuentaspoCobrar2(ref OperationResult objOperationResult, DateTime pstrt_FechaRegistroIni, DateTime pstrt_FechaRegistroFin, string Orden, string pstr_grupollave, string pstr_Nombregrupollave, string Filtro)
        {
            NodeBL objNodeBL = new NodeBL();
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                List<ReportePlanillaCobranza> query4 = new List<ReportePlanillaCobranza>();

                var queryVentas = (from n in dbContext.cobranzapendiente

                                   join A in dbContext.venta on new { IdVenta = n.v_IdVenta, eliminado = 0 } equals new { IdVenta = A.v_IdVenta, eliminado = A.i_Eliminado.Value } into A_join
                                   from A in A_join.DefaultIfEmpty()

                                   join B in dbContext.cliente on new { IdCliente = A.v_IdCliente, eliminado = 0 } equals new { IdCliente = B.v_IdCliente, eliminado = B.i_Eliminado.Value } into B_join
                                   from B in B_join.DefaultIfEmpty()

                                   join C in dbContext.vendedor on new { IdVendedor = A.v_IdVendedor, eliminado = 0 } equals new { IdVendedor = C.v_IdVendedor, eliminado = C.i_Eliminado.Value } into C_join
                                   from C in C_join.DefaultIfEmpty()

                                   join J4 in dbContext.documento on new { i_IdTipoDocumento = A.i_IdTipoDocumento.Value, eliminado = 0 }
                                                                  equals new { i_IdTipoDocumento = J4.i_CodigoDocumento, eliminado = J4.i_Eliminado.Value } into J4_join
                                   from J4 in J4_join.DefaultIfEmpty()

                                   join J5 in dbContext.datahierarchy on new { i_idTipoPago = A.i_IdCondicionPago.Value, b = 23, eliminado = 0 }
                                                                equals new { i_idTipoPago = J5.i_ItemId, b = J5.i_GroupId, eliminado = J5.i_IsDeleted.Value } into J5_join
                                   from J5 in J5_join.DefaultIfEmpty()

                                   where n.i_Eliminado == 0 && A.t_FechaRegistro >= pstrt_FechaRegistroIni && A.t_FechaRegistro <= pstrt_FechaRegistroFin &&
                                         n.d_Saldo > 0

                                   select new ReporteCuentasPorCobrar
                                   {

                                       Correlativo = "V " + A.v_Mes.Trim() + "-" + A.v_Correlativo.Trim(),
                                       NombreCliente = B == null ? "**NO EXISTE CLIENTE**" : (B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " + B.v_SegundoNombre + " " + B.v_RazonSocial).Trim(),
                                       MedioPago = J5 == null ? "" : J5.v_Value1,
                                       FechaEmision = A.t_FechaRegistro.Value,
                                       TipoDocumento = J4 == null ? "" : J4.v_Siglas + " " + A.v_SerieDocumento.Trim() + "-" + A.v_CorrelativoDocumento.Trim(),
                                       NroDocumento = "",
                                       GuiaRemision = A.v_NroGuiaRemisionSerie + "-" + A.v_NroGuiaRemisionCorrelativo,
                                       FechaVencimiento = null,
                                       Vendedor = C == null ? "**NO EXISTE VENDEDOR**" : C.v_CodVendedor.ToUpper(),
                                       Moneda = A.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$.",
                                       TotalFacturado = A.d_Total,
                                       Acuenta = n.d_Acuenta.Value,
                                       Saldo = A.i_IdMoneda == (int)Currency.Soles ? n.d_Saldo : 0,
                                       SaldoDolares = A.i_IdMoneda == (int)Currency.Dolares ? n.d_Saldo : 0,
                                       MonedaCobranza = A.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$.",
                                       Grupollave = pstr_grupollave == "NOMBRECLIENTE" ? B.v_IdCliente == null ? "** " + pstr_Nombregrupollave + " NO EXISTE **" : pstr_Nombregrupollave + " : " + (B.v_PrimerNombre + " " + B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_RazonSocial).Trim().ToUpper() : "" + pstr_grupollave == "MEDIOPAGO" ? J5.v_Value1 == null ? "** " + pstr_Nombregrupollave + " NO EXISTE **" : pstr_Nombregrupollave + " : " + J5.v_Value1 : "" + pstr_grupollave == "VENDEDOR" ? C.v_IdVendedor == null ? "** " + pstr_Nombregrupollave + " NO EXISTE **" : pstr_Nombregrupollave + " : " + C.v_NombreCompleto.Trim().ToUpper() : "",

                                       v_CodigoCliente = B == null ? "**NO EXISTE CLIENTE**" : B.v_CodCliente.Trim(),
                                       idTipoDocumento = A == null ? 0 : A.i_IdTipoDocumento.Value,
                                       v_IdVendedor = C == null ? "**NO EXISTE VENDEDOR**" : C.v_IdVendedor.Trim(),
                                       v_SerieDocumento = A.v_SerieDocumento.Trim(),
                                       v_CorrelativoDocumento = A.v_CorrelativoDocumento.Trim(),
                                       GrupoDetalle = "",



                                   }
                           ).ToList().AsQueryable();

                if (!string.IsNullOrEmpty(Filtro))
                {
                    queryVentas = queryVentas.Where(Filtro);
                }

                List<letrascanje> LetritasCanje = dbContext.letrascanje.Where(l => l.i_Eliminado == 0).ToList();
                List<venta> Ventitas = dbContext.venta.Where(l => l.i_Eliminado == 0).ToList();
                List<documento> documentitos = dbContext.documento.Where(l => l.i_Eliminado == 0).ToList();
                List<letrasdetalle> Ld = dbContext.letrasdetalle.Where(l => l.i_Eliminado == 0).ToList();
                var queryLetrasDetalle = (from a in dbContext.cobranzaletraspendiente

                                          join b in dbContext.letrasdetalle on new { IdLetrasDetalle = a.v_IdLetrasDetalle, eliminado = 0 } equals new { IdLetrasDetalle = b.v_IdLetrasDetalle, eliminado = b.i_Eliminado.Value } into b_join
                                          from b in b_join.DefaultIfEmpty()

                                          join c in dbContext.cliente on new { IdCliente = b.v_IdCliente, eliminado = 0 } equals new { IdCliente = c.v_IdCliente, eliminado = c.i_Eliminado.Value } into c_join
                                          from c in c_join.DefaultIfEmpty()

                                          join d in dbContext.documento on new { DocumentoLetraDet = b.i_IdTipoDocumento.Value, eliminado = 0 } equals new { DocumentoLetraDet = d.i_CodigoDocumento, eliminado = d.i_Eliminado.Value } into d_join

                                          from d in d_join.DefaultIfEmpty()

                                          where a.i_Eliminado == 0 && b.t_FechaEmision >= pstrt_FechaRegistroIni && b.t_FechaEmision <= pstrt_FechaRegistroFin &&
                                           a.d_Saldo > 0


                                          select new ReporteCuentasPorCobrar
                                          {
                                              Correlativo = "L " + b.v_Serie.Trim() + "-" + b.v_Correlativo.Trim(),
                                              NombreCliente = c == null ? "**NO EXISTE CLIENTE**" : (c.v_ApePaterno + " " + c.v_ApeMaterno + " " + c.v_PrimerNombre + " " + c.v_SegundoNombre + " " + c.v_RazonSocial).Trim(),
                                              MedioPago = "LETRA",
                                              FechaEmision = b.t_FechaEmision.Value,
                                              TipoDocumento = d == null ? "" : "    " + d.v_Siglas.Trim() + " " + b.v_Serie.Trim() + "-" + b.v_Correlativo.Trim(),
                                              NroDocumento = "",
                                              GuiaRemision = "",
                                              FechaVencimiento = b.t_FechaVencimiento.Value,
                                              Vendedor = "",
                                              Moneda = b.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$.",
                                              TotalFacturado = a.d_Saldo.Value + a.d_Acuenta.Value,
                                              Acuenta = a.d_Acuenta.Value,
                                              Saldo = b.i_IdMoneda == (int)Currency.Soles ? a.d_Saldo : 0,
                                              SaldoDolares = b.i_IdMoneda == (int)Currency.Dolares ? a.d_Saldo : 0,
                                              MonedaCobranza = b.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$.",
                                              Grupollave = pstr_grupollave == "NOMBRECLIENTE" ? c.v_IdCliente == null ? "** CLIENTE  NO EXISTE **" : "CLIENTE : " + (c.v_PrimerNombre + " " + c.v_ApePaterno + " " + c.v_ApeMaterno + " " + c.v_RazonSocial).Trim() :
                                              pstr_grupollave == "MEDIOPAGO" ? "COND. PAGO : LETRA" : pstr_Nombregrupollave == "VENDEDOR" ? "** VENDEDOR NO EXISTE PARA LETRAS **" : "",
                                              v_CodigoCliente = c == null ? "**NO EXISTE CLIENTE**" : c.v_CodCliente.Trim(),
                                              idTipoDocumento = d == null ? 0 : d.i_CodigoDocumento,
                                              v_IdVendedor = "",
                                              v_SerieDocumento = b.v_Serie,
                                              v_CorrelativoDocumento = b.v_Correlativo,
                                              v_IdLetraDetalle = b.v_IdLetrasDetalle,
                                              v_IdLetras = b.v_IdLetras,
                                              GrupoDetalle = "",


                                          }).ToList().Select(x =>
                                          {

                                              //var Buscar = BuscarLetrasCanje(x.v_IdLetras, LetritasCanje,Ventitas,documentitos ,Ld);

                                              return new ReporteCuentasPorCobrar
                                              {

                                                  Correlativo = x.Correlativo,
                                                  NombreCliente = x.NombreCliente,
                                                  MedioPago = x.MedioPago,
                                                  FechaEmision = x.FechaEmision,
                                                  TipoDocumento = x.TipoDocumento,
                                                  NroDocumento = x.NroDocumento,
                                                  GuiaRemision = x.GuiaRemision,
                                                  FechaVencimiento = x.FechaVencimiento,
                                                  Vendedor = x.Vendedor,
                                                  Moneda = x.Moneda,
                                                  TotalFacturado = x.TotalFacturado,
                                                  Acuenta = x.Acuenta,
                                                  Saldo = x.Saldo,
                                                  SaldoDolares = x.SaldoDolares,
                                                  MonedaCobranza = x.MonedaCobranza,
                                                  Grupollave = x.Grupollave,
                                                  v_CodigoCliente = x.v_CodigoCliente,
                                                  idTipoDocumento = x.idTipoDocumento,
                                                  v_IdVendedor = x.v_IdVendedor,
                                                  v_SerieDocumento = x.v_SerieDocumento,
                                                  v_CorrelativoDocumento = x.v_CorrelativoDocumento,
                                                  //GrupoDetalle = Buscar,
                                              };

                                          }).ToList().AsQueryable();

                if (!string.IsNullOrEmpty(Filtro))
                {

                    queryLetrasDetalle = queryLetrasDetalle.Where(Filtro);
                }

                var queryFinal = queryVentas.Concat(queryLetrasDetalle);

                if (!string.IsNullOrEmpty(Orden))
                {

                    queryFinal = queryFinal.OrderBy(Orden);
                }

                objOperationResult.Success = 1;

                List<ReporteCuentasPorCobrar> objData = queryFinal.ToList();
                return objData;

                //var queryVentas =( from a in dbContext.venta 

            }
            catch (Exception)
            {
                objOperationResult.Success = 0;

                return null;
            }
        }

        public string BuscarLetrasCanje(string IdLetra, List<letrascanje> letrascanje, List<venta> Ventas, List<documento> Documentos, List<letrasdetalle> LetrasDetalle)
        {

            string DocumentoDedondesegenero = string.Empty;

            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {

                var MonLetra = dbContext.letras.Where(j => j.v_IdLetras == IdLetra).FirstOrDefault();
                string MonedaLetra = MonLetra != null ? MonLetra.i_IdMoneda == 1 ? "S/" : "US$." : "";
                var DocumentosqueGeneraron = (from a in letrascanje

                                              where a.v_IdLetras == IdLetra

                                              select a).ToList();

                int NumeroRegistros = DocumentosqueGeneraron.Count();
                int Contador = 1;
                decimal ImporteDetalle = 0;
                decimal CanjeadoDetalle = 0;
                foreach (var item in DocumentosqueGeneraron)
                {


                    if (item.v_IdVenta != null)
                    {

                        var docVenta = (from a in Ventas
                                        join b in Documentos on new { v = a.i_IdTipoDocumento.Value } equals new { v = b.i_CodigoDocumento } into b_join
                                        from b in b_join.DefaultIfEmpty()

                                        where a.v_IdVenta == item.v_IdVenta
                                        select new
                                        {
                                            Documento = b.v_Siglas + " " + a.v_SerieDocumento + "-" + a.v_CorrelativoDocumento,
                                            Total = a.d_Total == null ? 0 : Utils.Windows.DevuelveValorRedondeado(a.d_Total.Value, 2),// ?? 0,
                                            Moned = a.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$.",
                                        }).FirstOrDefault();


                        if (docVenta != null)
                        {

                            ImporteDetalle = ImporteDetalle + docVenta.Total;
                            CanjeadoDetalle = CanjeadoDetalle + item.d_MontoCanjeado.Value;
                            if (Contador == NumeroRegistros)
                            {
                                if (NumeroRegistros != 1)
                                {
                                    DocumentoDedondesegenero = DocumentoDedondesegenero + docVenta.Documento + "               IMPORTE : " + docVenta.Moned + " " + docVenta.Total.ToString() + "        MONTO CANJEADO " + MonedaLetra + " " + item.d_MontoCanjeado + "\n " + "                                                __________________" + "\nSUB TOTAL  :                          IMPORTE : " + "       " + ImporteDetalle + "            MONTO CANJEADO :        " + CanjeadoDetalle + "    generó las(s) sgte(s) Letra(s) :\n ____________________________________________________________________________________________________________________________________________";

                                }
                                else
                                {
                                    DocumentoDedondesegenero = DocumentoDedondesegenero + docVenta.Documento + "               IMPORTE : " + docVenta.Moned + " " + docVenta.Total.ToString() + "        MONTO CANJEADO " + MonedaLetra + " " + item.d_MontoCanjeado + "      generó las(s) sgte(s) Letra(s) :\n ____________________________________________________________________________________________________________________________________________";
                                }
                            }

                            else
                            {
                                DocumentoDedondesegenero = DocumentoDedondesegenero + docVenta.Documento + "               IMPORTE : " + docVenta.Moned + " " + docVenta.Total.ToString() + "        MONTO CANJEADO " + MonedaLetra + " " + item.d_MontoCanjeado + "\n";

                            }
                        }

                    }
                    if (item.v_IdLetrasDetalle != null)
                    {
                        var docLetras = (from a in LetrasDetalle
                                         join b in Documentos on new { ld = a.i_IdTipoDocumento.Value } equals new { ld = b.i_CodigoDocumento } into b_join
                                         from b in b_join.DefaultIfEmpty()
                                         where a.v_IdLetrasDetalle == item.v_IdLetrasDetalle

                                         select new
                                         {
                                             Documento = b.v_Siglas + " " + a.v_Serie + "-" + a.v_Correlativo,
                                             Total = a.d_Importe == null ? 0 : Utils.Windows.DevuelveValorRedondeado(a.d_Importe.Value, 2),
                                             Moned = a.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$.",
                                         }).FirstOrDefault();

                        if (docLetras != null)
                        {
                            ImporteDetalle = ImporteDetalle + docLetras.Total;
                            CanjeadoDetalle = CanjeadoDetalle + item.d_MontoCanjeado.Value;
                            if (Contador == NumeroRegistros)
                            {

                                if (NumeroRegistros != 1)
                                {

                                    DocumentoDedondesegenero = DocumentoDedondesegenero + docLetras.Documento + "               IMPORTE : " + docLetras.Moned + " " + docLetras.Total.ToString() + "        MONTO CANJEADO " + MonedaLetra + " " + item.d_MontoCanjeado + "\n " + "                                                __________________" + "\nSUB TOTAL  :                          IMPORTE : " + "       " + ImporteDetalle + "            MONTO CANJEADO :        " + CanjeadoDetalle + "    generó las(s) sgte(s) Letra(s) :\n ____________________________________________________________________________________________________________________________________________";

                                }
                                else
                                {
                                    DocumentoDedondesegenero = DocumentoDedondesegenero + docLetras.Documento + "               IMPORTE : " + docLetras.Moned + " " + docLetras.Total.ToString() + "       MONTO CANJEADO " + MonedaLetra + " " + item.d_MontoCanjeado + "      generó las(s) sgte(s) Letra(s) :\n ____________________________________________________________________________________________________________________________________________";
                                }

                            }
                            else
                            {
                                DocumentoDedondesegenero = DocumentoDedondesegenero + docLetras.Documento + "               IMPORTE : " + docLetras.Moned + " " + docLetras.Total.ToString() + "        MONTO CANJEADO " + MonedaLetra + " " + item.d_MontoCanjeado + "\n";
                            }
                        }

                    }
                    Contador = Contador + 1;
                }

                return DocumentoDedondesegenero;

            }


        }





        private List<decimal> MetodoCalcularTotalesCobranza(List<ReporteVentaCobranza> query1, int? pintFormaPagoCobranzaDetalle, DateTime Fecha)
        {

            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
            List<cobranzadetalleDto> ListaCobranzaDetalle = new List<cobranzadetalleDto>();
            List<decimal> x = new List<decimal>();
            List<TotalCobranzaDto> CobranzasDelDia = new List<TotalCobranzaDto>();
            TotalCobranzaDto _TotalCobranzaDto = new TotalCobranzaDto();
            //Soles
            var PagoSolesDia = query1.FindAll(p => p.i_FormaPagoCobranzaDetalle == pintFormaPagoCobranzaDetalle && pintFormaPagoCobranzaDetalle != -1 && p.FechaEmision == Fecha && p.MonedaCobranza == "S/");
            decimal MontoSolesDia = PagoSolesDia.Sum(s => s.MontoPagado).Value;

            var PagoSolesOdia = query1.FindAll(p => p.i_FormaPagoCobranzaDetalle == pintFormaPagoCobranzaDetalle && pintFormaPagoCobranzaDetalle != -1 && p.FechaEmision != Fecha && p.MonedaCobranza == "S/");
            decimal MontoSolesOdia = PagoSolesOdia.Sum(s => s.MontoPagado).Value;

            var PagoDolaresDia = query1.FindAll(p => p.i_FormaPagoCobranzaDetalle == pintFormaPagoCobranzaDetalle && pintFormaPagoCobranzaDetalle != -1 && p.FechaEmision == Fecha && p.MonedaCobranza == "US$.");
            decimal MontoDolaresDia = PagoDolaresDia.Sum(s => s.MontoPagadoDolares).Value;

            var PagoDolaresOdia = query1.FindAll(p => p.i_FormaPagoCobranzaDetalle == pintFormaPagoCobranzaDetalle && pintFormaPagoCobranzaDetalle != -1 && p.FechaEmision != Fecha && p.MonedaCobranza == "US$.");
            decimal MontoDolaresOdia = PagoDolaresOdia.Sum(s => s.MontoPagadoDolares).Value;


            //Soles

            var FacturasDia = query1.Where(p => p.idTipoDocumento == (int)TiposDocumentos.Factura && p.FechaEmision == Fecha && p.Moneda == "S/").GroupBy(xx => xx.NroDocumento);
            decimal TotalFacturasDia = FacturasDia.Sum(s => s.First().TotalFacturado).Value;

            var BoletasDia = query1.Where(p => p.idTipoDocumento == (int)TiposDocumentos.BoletaVenta && p.FechaEmision == Fecha && p.Moneda == "S/").GroupBy(xx => xx.NroDocumento);
            decimal TotalBoletasDia = BoletasDia.Sum(s => s.First().TotalFacturado).Value;

            var NotaCreditoDia = query1.Where(p => p.idTipoDocumento == (int)TiposDocumentos.NotaCredito && p.FechaEmision == Fecha && p.Moneda == "S/").GroupBy(xx => xx.NroDocumento);
            decimal TotalNotaCreditoDia = NotaCreditoDia.Sum(s => s.First().TotalFacturado).Value;


            var NotaDebitoDia = query1.Where(p => p.idTipoDocumento == (int)TiposDocumentos.NotaDebito && p.FechaEmision == Fecha && p.Moneda == "S/").GroupBy(xx => xx.NroDocumento);
            decimal TotalNotaDebitoDia = NotaDebitoDia.Sum(s => s.First().TotalFacturado).Value;

            var FacturasOtroDia = query1.Where(p => p.idTipoDocumento == (int)TiposDocumentos.Factura && p.FechaEmision != Fecha && p.Moneda == "S/").GroupBy(xx => xx.NroDocumento);
            decimal TotalFacturasOtroDia = FacturasOtroDia.Sum(s => s.First().TotalFacturado).Value;

            var BoletasOtroDia = query1.Where(p => p.idTipoDocumento == (int)TiposDocumentos.BoletaVenta && p.FechaEmision != Fecha && p.Moneda == "S/").GroupBy(xx => xx.NroDocumento);
            decimal TotalBoletasOtroDia = BoletasOtroDia.Sum(s => s.First().TotalFacturado).Value;

            var NotaCreditoOtroDia = query1.Where(p => p.idTipoDocumento == (int)TiposDocumentos.NotaCredito && p.FechaEmision != Fecha && p.Moneda == "S/").GroupBy(xx => xx.NroDocumento);
            decimal TotalNotaCreditoOtroDia = NotaCreditoOtroDia.Sum(s => s.First().TotalFacturado).Value;


            var NotaDebitoOtroDia = query1.Where(p => p.idTipoDocumento == (int)TiposDocumentos.NotaDebito && p.FechaEmision != Fecha && p.Moneda == "S/").GroupBy(xx => xx.NroDocumento);
            decimal TotalNotaDebitoOtroDia = NotaDebitoOtroDia.Sum(s => s.First().TotalFacturado).Value;


            //Dolares

            var FacturasDiaDolares = query1.Where(p => p.idTipoDocumento == (int)TiposDocumentos.Factura && p.FechaEmision == Fecha && p.Moneda == "US$.").GroupBy(xx => xx.NroDocumento);
            decimal TotalFacturasDiaDolares = FacturasDiaDolares.Sum(s => s.First().TotalFacturadoDolares).Value;

            var BoletasDiaDolares = query1.Where(p => p.idTipoDocumento == (int)TiposDocumentos.BoletaVenta && p.FechaEmision == Fecha && p.Moneda == "US$.").GroupBy(xx => xx.NroDocumento);
            decimal TotalBoletasDiaDolares = BoletasDiaDolares.Sum(s => s.First().TotalFacturadoDolares).Value;

            var NotaCreditoDiaDolares = query1.Where(p => p.idTipoDocumento == (int)TiposDocumentos.NotaCredito && p.FechaEmision == Fecha && p.Moneda == "US$.").GroupBy(xx => xx.NroDocumento);
            decimal TotalNotaCreditoDiaDolares = NotaCreditoDiaDolares.Sum(s => s.First().TotalFacturadoDolares).Value;

            var NotaDebitoDiaDolares = query1.Where(p => p.idTipoDocumento == (int)TiposDocumentos.NotaDebito && p.FechaEmision == Fecha && p.Moneda == "US$.").GroupBy(xx => xx.NroDocumento);
            decimal TotalNotaDebitoDiaDolares = NotaDebitoDiaDolares.Sum(s => s.First().TotalFacturadoDolares).Value;

            var FacturasOtroDiaDolares = query1.Where(p => p.idTipoDocumento == (int)TiposDocumentos.Factura && p.FechaEmision != Fecha && p.Moneda == "US$.").GroupBy(xx => xx.NroDocumento);
            decimal TotalFacturasOtroDiaDolares = FacturasOtroDiaDolares.Sum(s => s.First().TotalFacturadoDolares).Value;

            var BoletasOtroDiaDolares = query1.Where(p => p.idTipoDocumento == (int)TiposDocumentos.BoletaVenta && p.FechaEmision != Fecha && p.Moneda == "US$.").GroupBy(xx => xx.NroDocumento);
            decimal TotalBoletasOtroDiaDolares = BoletasOtroDiaDolares.Sum(s => s.First().TotalFacturadoDolares).Value;

            var NotaCreditoOtroDiaDolares = query1.Where(p => p.idTipoDocumento == (int)TiposDocumentos.NotaCredito && p.FechaEmision != Fecha && p.Moneda == "US$.").GroupBy(xx => xx.NroDocumento);
            decimal TotalNotaCreditoOtroDiaDolares = NotaCreditoOtroDiaDolares.Sum(s => s.First().TotalFacturadoDolares).Value;

            var NotaDebitoOtroDiaDolares = query1.Where(p => p.idTipoDocumento == (int)TiposDocumentos.NotaDebito && p.FechaEmision != Fecha && p.Moneda == "US$.").GroupBy(xx => xx.NroDocumento);
            decimal TotalNotaDebitoOtroDiaDolares = NotaDebitoOtroDiaDolares.Sum(s => s.First().TotalFacturadoDolares).Value;

            x.Add(TotalFacturasDia); //0
            x.Add(TotalBoletasDia);
            x.Add(TotalNotaCreditoDia);
            x.Add(TotalNotaDebitoDia);
            x.Add(TotalFacturasOtroDia);
            x.Add(TotalBoletasOtroDia);
            x.Add(TotalNotaCreditoOtroDia);
            x.Add(TotalNotaDebitoOtroDia);
            x.Add(TotalFacturasDiaDolares);
            x.Add(TotalBoletasDiaDolares);
            x.Add(TotalNotaCreditoDiaDolares);
            x.Add(TotalNotaDebitoDiaDolares);
            x.Add(TotalFacturasOtroDiaDolares);
            x.Add(TotalBoletasOtroDiaDolares);
            x.Add(TotalNotaCreditoOtroDiaDolares);
            x.Add(TotalNotaDebitoOtroDiaDolares);
            x.Add(MontoSolesDia);
            x.Add(MontoSolesOdia);
            x.Add(MontoDolaresDia);
            x.Add(MontoDolaresOdia);

            return x;

        }
        private List<decimal> MetodoCalcularTotalesCobranzaDocumentos(List<ReporteVentaCobranza> query1, int? pintTipoDocumento, DateTime Fecha)
        {

            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
            List<cobranzadetalleDto> ListaCobranzaDetalle = new List<cobranzadetalleDto>();

            List<decimal> x = new List<decimal>();

            //Soles
            var DocumentoDia = query1.Where(p => p.idTipoDocumento == pintTipoDocumento && p.FechaEmision == Fecha && p.Moneda == "S/").GroupBy(xx => xx.NroDocumento);
            decimal TotalDocumentoDia = DocumentoDia.Sum(s => s.First().TotalFacturado).Value;

            var DocumentoOtroDia = query1.Where(p => p.idTipoDocumento == pintTipoDocumento && p.FechaEmision != Fecha && p.Moneda == "S/").GroupBy(xx => xx.NroDocumento);
            decimal TotalDocumentoOtroDia = DocumentoOtroDia.Sum(s => s.First().TotalFacturado).Value;


            //Dolares

            var DocumentoDiaDolares = query1.Where(p => p.idTipoDocumento == pintTipoDocumento && p.FechaEmision == Fecha && p.Moneda == "US$.").GroupBy(xx => xx.NroDocumento);
            decimal TotalDocumentoDiaDolares = DocumentoDiaDolares.Sum(s => s.First().TotalFacturadoDolares).Value;


            var DocumentoOtroDiaDolares = query1.Where(p => p.idTipoDocumento == pintTipoDocumento && p.FechaEmision != Fecha && p.Moneda == "US$.").GroupBy(xx => xx.NroDocumento);
            decimal TotalDocumentoOtroDiaDolares = DocumentoOtroDiaDolares.Sum(s => s.First().TotalFacturadoDolares).Value;

            x.Add(TotalDocumentoDia);
            x.Add(TotalDocumentoDiaDolares);
            x.Add(TotalDocumentoOtroDia);
            x.Add(TotalDocumentoOtroDiaDolares);

            return x;

        }
        public string DevolverNombreEmpresaPropietaria()
        {
            OperationResult objOperationResult = new OperationResult();

            NodeBL objNodeBL = new NodeBL();
            int _intNodeId = int.Parse(Utils.Windows.GetApplicationConfigValue("NodeId"));

            var x = objNodeBL.GetNodeByNodeId(ref objOperationResult, _intNodeId);

            return x.v_RazonSocial;

        }
        public List<ReporteDocumentoVoucher> ReporteDocumentoVoucherCobranza(string pstrIdCobranza)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                var queryVentas = (from n in dbContext.venta
                                   join A in dbContext.cobranzadetalle on new { IdVenta = n.v_IdVenta, eliminado = 0 } equals new { IdVenta = A.v_IdVenta, eliminado = A.i_Eliminado.Value } into A_join
                                   from A in A_join.DefaultIfEmpty()
                                   join C in dbContext.cobranza on new { IdCobranza = A.v_IdCobranza, eliminado = 0 } equals new { IdCobranza = C.v_IdCobranza, eliminado = C.i_Eliminado.Value } into C_join
                                   from C in C_join.DefaultIfEmpty()

                                   join B in dbContext.cliente on new { IdCliente = n.v_IdCliente, eliminado = 0 } equals new { IdCliente = B.v_IdCliente, eliminado = B.i_Eliminado.Value } into B_join
                                   from B in B_join.DefaultIfEmpty()

                                   join J4 in dbContext.documento on new { i_IdTipoDocumento = n.i_IdTipoDocumento.Value }
                                                         equals new { i_IdTipoDocumento = J4.i_CodigoDocumento } into J4_join
                                   from J4 in J4_join.DefaultIfEmpty()

                                   join J5 in dbContext.documento on new { i_IdTipoDocumento = C.i_IdTipoDocumento.Value }
                                                    equals new { i_IdTipoDocumento = J5.i_CodigoDocumento } into J5_join
                                   from J5 in J5_join.DefaultIfEmpty()

                                   join J7 in dbContext.datahierarchy on new { i_IdFormaPago = C.i_IdMedioPago.Value, b = 44, eliminado = 0 }
                                                                  equals new { i_IdFormaPago = J7.i_ItemId, b = J7.i_GroupId, eliminado = J7.i_IsDeleted.Value } into J7_join
                                   from J7 in J7_join.DefaultIfEmpty()

                                   join J8 in dbContext.datahierarchy on new { i_IdFormaPago = A.i_IdFormaPago.Value, b = 46, eliminado = 0 }
                                                  equals new { i_IdFormaPago = J8.i_ItemId, b = J8.i_GroupId, eliminado = J8.i_IsDeleted.Value } into J8_join
                                   from J8 in J8_join.DefaultIfEmpty()


                                   join J9 in dbContext.cobranzapendiente on new { IdVenta = A.v_IdVenta, eliminado = 0 } equals new { IdVenta = J9.v_IdVenta, eliminado = J9.i_Eliminado.Value } into J9_join

                                   from J9 in J9_join.DefaultIfEmpty()

                                   where C.v_IdCobranza == pstrIdCobranza

                                   select new ReporteDocumentoVoucher
                                   {
                                       TipoDocumento = J4.v_Siglas,
                                       NroVoucher = "VOUCHER DE INGRESO A CAJA N° " + C.v_Periodo + "-" + C.v_Mes + "-" + C.v_Correlativo,
                                       NroDocumento = n.v_SerieDocumento + " - " + n.v_CorrelativoDocumento,
                                       NombreCliente = B == null ? "" : (B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " + B.v_SegundoNombre + " " + B.v_RazonSocial).Trim(),
                                       IdMoneda = n.i_IdMoneda.Value,
                                       // FechaEmision = n.t_FechaRegistro.Value,
                                       FechaEmision = C.t_FechaRegistro.Value,
                                       Moneda = n.i_IdMoneda == 1 ? "S/." : "US$.",
                                       Monto = n.d_Total,
                                       Titular = C.v_Nombre,
                                       Pago = A.d_ImporteSoles.Value,
                                       FechaCobranza = C.t_FechaRegistro.Value,
                                       TipoCambio = C.d_TipoCambio.Value,
                                       Glosa = C.v_Glosa,
                                       Letra = "",
                                       DocumentoReferencia = J8.v_Value1,
                                       NombreEmpresaPropietaria = "",
                                       DocumentoCobranza = J5.v_Nombre,
                                       MedioPago = J7.v_Value1,
                                       RucEmpresaPropietaria = "",
                                       MontoPagado = n.d_Total.Value,
                                       SaldoFinal = J9.d_Saldo,
                                       MonedaPago = C.i_IdMoneda == (int)Currency.Soles ? "S/." : "US$.",

                                   }).ToList();

                var queryLetras = (from a in dbContext.cobranza

                                   join A in dbContext.cobranzadetalle on new { Cobranza = a.v_IdCobranza, eliminado = 0 } equals new { Cobranza = A.v_IdCobranza, eliminado = A.i_Eliminado.Value } into A_join

                                   from A in A_join.DefaultIfEmpty()

                                   join C in dbContext.letrasdetalle on new { LetraDetalle = A.v_IdVenta, eliminado = 0 } equals new { LetraDetalle = C.v_IdLetrasDetalle, eliminado = 0 } into C_join

                                   from C in C_join.DefaultIfEmpty()

                                   join B in dbContext.cliente on new { IdCliente = C.v_IdCliente, eliminado = 0 } equals new { IdCliente = B.v_IdCliente, eliminado = B.i_Eliminado.Value } into B_join
                                   from B in B_join.DefaultIfEmpty()

                                   join J4 in dbContext.documento on new { i_IdTipoDocumento = C.i_IdTipoDocumento.Value }
                                                         equals new { i_IdTipoDocumento = J4.i_CodigoDocumento } into J4_join
                                   from J4 in J4_join.DefaultIfEmpty()

                                   join J5 in dbContext.documento on new { i_IdTipoDocumento = C.i_IdTipoDocumento.Value }
                                                    equals new { i_IdTipoDocumento = J5.i_CodigoDocumento } into J5_join
                                   from J5 in J5_join.DefaultIfEmpty()

                                   join J7 in dbContext.datahierarchy on new { i_IdFormaPago = a.i_IdMedioPago.Value, b = 44, eliminado = 0 }
                                                  equals new { i_IdFormaPago = J7.i_ItemId, b = J7.i_GroupId, eliminado = J7.i_IsDeleted.Value } into J7_join
                                   from J7 in J7_join.DefaultIfEmpty()

                                   join J8 in dbContext.datahierarchy on new { i_IdFormaPago = A.i_IdFormaPago.Value, b = 46, eliminado = 0 }
                                                  equals new { i_IdFormaPago = J8.i_ItemId, b = J8.i_GroupId, eliminado = J8.i_IsDeleted.Value } into J8_join
                                   from J8 in J8_join.DefaultIfEmpty()


                                   join J9 in dbContext.cobranzapendiente on new { IdVenta = A.v_IdVenta, eliminado = 0 } equals new { IdVenta = J9.v_IdVenta, eliminado = J9.i_Eliminado.Value } into J9_join

                                   from J9 in J9_join.DefaultIfEmpty()

                                   where a.v_IdCobranza == pstrIdCobranza && a.i_Eliminado.Value == 0 && A.i_EsLetra == 1

                                   select new ReporteDocumentoVoucher
                                   {
                                       TipoDocumento = J4.v_Siglas,
                                       NroVoucher = "VOUCHER DE INGRESO A CAJA N° " + a.v_Periodo + "-" + a.v_Mes + "-" + C.v_Correlativo,
                                       NroDocumento = C == null ? "" : C.v_Serie + " - " + C.v_Correlativo,
                                       NombreCliente = B == null ? "" : (B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " + B.v_SegundoNombre + " " + B.v_RazonSocial).Trim(),
                                       IdMoneda = C == null ? -1 : C.i_IdMoneda.Value,
                                       FechaEmision = C == null ? DateTime.MinValue : C.t_FechaEmision.Value,
                                       Moneda = C == null ? "" : C.i_IdMoneda == 1 ? "S/." : "US$.",
                                       Monto = C == null ? 0 : C.d_Importe.Value,
                                       Titular = a.v_Nombre,
                                       Pago = A.d_ImporteSoles.Value,
                                       FechaCobranza = a.t_FechaRegistro.Value,
                                       TipoCambio = C == null ? 0 : C.d_TipoCambio.Value,
                                       Glosa = a.v_Glosa,
                                       Letra = "",
                                       DocumentoReferencia = J8.v_Value1,
                                       NombreEmpresaPropietaria = "",
                                       DocumentoCobranza = J5.v_Nombre,
                                       MedioPago = J7.v_Value1,
                                       RucEmpresaPropietaria = "",

                                       MontoPagado = C == null ? 0 : C.d_Importe.Value,
                                       SaldoFinal = J9.d_Saldo,
                                       MonedaPago = C == null ? "0" : C.i_IdMoneda == (int)Currency.Soles ? "S/." : "US$.",

                                   }).ToList();

                var querFinal = queryVentas.Concat(queryLetras);

                var queryFinal = (from n in querFinal

                                  select new ReporteDocumentoVoucher
                                  {

                                      TipoDocumento = n.TipoDocumento,
                                      NroVoucher = n.NroVoucher,
                                      NroDocumento = n.NroDocumento,
                                      NombreCliente = n.NombreCliente,
                                      IdMoneda = n.IdMoneda,
                                      FechaEmision = n.FechaEmision,
                                      Moneda = n.Moneda,
                                      Monto = 0,
                                      Titular = n.Titular,
                                      Pago = n.Pago,
                                      FechaCobranza = n.FechaCobranza,
                                      TipoCambio = n.TipoCambio,
                                      Glosa = n.Glosa,
                                      Letra = "",
                                      DocumentoReferencia = n.DocumentoReferencia,
                                      NombreEmpresaPropietaria = "",
                                      DocumentoCobranza = n.DocumentoCobranza,
                                      MedioPago = n.MedioPago,
                                      RucEmpresaPropietaria = "",
                                      MontoPagado = n.MontoPagado,
                                      MontoPagadoString = n.MontoPagado.Value.ToString("0.00"),
                                      SaldoFinal = n.SaldoFinal,
                                      MonedaPago = n.MonedaPago,

                                  });

                var queryFinalFinal = (from n in queryFinal

                                       select new ReporteDocumentoVoucher
                                       {

                                           TipoDocumento = n.TipoDocumento,
                                           NroVoucher = n.NroVoucher,
                                           NroDocumento = n.NroDocumento,
                                           NombreCliente = n.NombreCliente,
                                           IdMoneda = n.IdMoneda,
                                           FechaEmision = n.FechaEmision,
                                           Moneda = n.Moneda,
                                           Monto = decimal.Parse(n.MontoPagadoString),
                                           Titular = n.Titular,
                                           Pago = n.Pago,
                                           FechaCobranza = n.FechaCobranza,
                                           TipoCambio = n.TipoCambio,
                                           Glosa = n.Glosa,
                                           Letra = "",
                                           DocumentoReferencia = n.DocumentoReferencia,
                                           NombreEmpresaPropietaria = "",
                                           DocumentoCobranza = n.DocumentoCobranza,
                                           MedioPago = n.MedioPago,
                                           RucEmpresaPropietaria = "",
                                           MontoPagado = n.MontoPagado,
                                           MontoPagadoString = "",
                                           SaldoFinal = n.SaldoFinal,
                                           MonedaPago = n.MonedaPago,
                                       });
                List<ReporteDocumentoVoucher> objData = queryFinalFinal.ToList();

                return objData;
            }
            catch (Exception)
            {
                //throw;
                return null;
            }
        }
        public List<ReporteEstadoCuentaCliente> ReporteEstaCuentaCliente(ref  OperationResult objOperationResult, string pstrCodigoCliente, DateTime FechaSaldoDeudor, DateTime FechaDesde, DateTime FechaHasta)
        {
            List<ReporteEstadoCuentaCliente> ListaFinal = new List<ReporteEstadoCuentaCliente>();
            ReporteEstadoCuentaCliente objReporte = new ReporteEstadoCuentaCliente();

            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var queryDebe = (from a in dbContext.venta

                                     join B in dbContext.cliente on a.v_IdCliente equals B.v_IdCliente into B_join
                                     from B in B_join.DefaultIfEmpty()

                                     join J4 in dbContext.documento on new { i_IdTipoDocumento = a.i_IdTipoDocumento.Value }
                                                           equals new { i_IdTipoDocumento = J4.i_CodigoDocumento } into J4_join
                                     from J4 in J4_join.DefaultIfEmpty()

                                     join J7 in dbContext.datahierarchy on new { i_idTipoPago = a.i_IdCondicionPago.Value, b = 23 }
                                                                  equals new { i_idTipoPago = J7.i_ItemId, b = J7.i_GroupId } into J7_join
                                     from J7 in J7_join.DefaultIfEmpty()

                                     join J8 in dbContext.documento on new { i_IdTipoDocRef = a.i_IdTipoDocumentoRef.Value, eliminado = 0 }
                                                                    equals new { i_IdTipoDocRef = J8.i_CodigoDocumento, eliminado = J8.i_Eliminado.Value } into J8_join

                                     from J8 in J8_join.DefaultIfEmpty()


                                     where B.v_CodCliente == pstrCodigoCliente && a.i_Eliminado == 0 && a.i_IdEstado == 1
                                     select new ReporteEstadoCuentaCliente
                                     {

                                         Fecha = a.t_FechaRegistro.Value,
                                         Cliente = B == null ? "**NO EXISTE CLIENTE**" : ("CÓDIGO : " + B.v_CodCliente.Trim() + "   -    DENOMINACIÓN: " + B.v_ApePaterno.Trim() + " " + B.v_ApeMaterno.Trim() + " " + B.v_PrimerNombre.Trim() + " " + B.v_SegundoNombre.Trim() + " " + B.v_RazonSocial.Trim()).Trim().ToUpper(),
                                         pIntTipoDocumento = a.i_IdTipoDocumento.Value,
                                         Concepto = J4.v_Siglas.Trim() + ".  " + a.v_SerieDocumento.Trim() + "-" + a.v_CorrelativoDocumento.Trim(),
                                         Referencia = a.v_Concepto.Trim(),
                                         DebeSoles = (J4.i_UsadoDocumentoInverso == 0 || J4.i_UsadoDocumentoInverso == null) ? a.i_IdMoneda.Value == (int)Currency.Soles ? a.d_Total.Value : 0 : 0,
                                         DebeDolares = (J4.i_UsadoDocumentoInverso == 0 || J4.i_UsadoDocumentoInverso == null) ? a.i_IdMoneda.Value == (int)Currency.Dolares ? a.d_Total.Value : 0 : 0,
                                         HaberSoles = J4.i_UsadoDocumentoInverso == 1 ? a.i_IdMoneda.Value == (int)Currency.Soles ? a.d_Total.Value : 0 : 0,
                                         HaberDolares = J4.i_UsadoDocumentoInverso == 1 ? a.i_IdMoneda.Value == (int)Currency.Dolares ? a.d_Total.Value : 0 : 0,
                                         FechaInsercion = a.t_ActualizaFecha.Value == null ? a.t_InsertaFecha.Value : a.t_ActualizaFecha.Value,        //a.t_InsertaFecha.Value  ,
                                         DocumentoReferencia = J8 == null ? "" : J8.v_Siglas.Trim() + " " + a.v_SerieDocumentoRef.Trim() + "-" + a.v_CorrelativoDocumentoRef.Trim(),
                                         Naturaleza = "D",
                                         IdLetras = a.v_IdVenta,
                                     }).ToList();


                    var queryHaber = (from A in dbContext.cobranzadetalle

                                      join C in dbContext.cobranza on new { b = A.v_IdCobranza, eliminado = 0 } equals new { b = C.v_IdCobranza, eliminado = C.i_Eliminado.Value } into C_join
                                      from C in C_join.DefaultIfEmpty()

                                      join a in dbContext.venta on new { IdVenta = A.v_IdVenta, Estado = 1, eliminado = 0 } equals new { IdVenta = a.v_IdVenta, Estado = a.i_IdEstado.Value, eliminado = a.i_Eliminado.Value } into a_join
                                      from a in a_join.DefaultIfEmpty()

                                      join J9 in dbContext.letrasdetalle on new { IdVenta = A.v_IdVenta, eliminado = 0 } equals new { IdVenta = J9.v_IdLetrasDetalle, eliminado = J9.i_Eliminado.Value } into J9_join

                                      from J9 in J9_join.DefaultIfEmpty()

                                      join B in dbContext.cliente on new { ClienteVenta = a.v_IdCliente, eliminado = 0 } equals new { ClienteVenta = B.v_IdCliente, eliminado = B.i_Eliminado.Value } into B_join
                                      from B in B_join.DefaultIfEmpty()

                                      join J11 in dbContext.cliente on new { ClienteLetra = J9.v_IdCliente, eliminado = 0 } equals new { ClienteLetra = J11.v_IdCliente, eliminado = J11.i_Eliminado.Value } into J11_join

                                      from J11 in J11_join.DefaultIfEmpty()

                                      join J4 in dbContext.documento on new { DocumentoVenta = a.i_IdTipoDocumento.Value, eliminado = 0 }
                                                            equals new { DocumentoVenta = J4.i_CodigoDocumento, eliminado = J4.i_Eliminado.Value } into J4_join
                                      from J4 in J4_join.DefaultIfEmpty()

                                      join J5 in dbContext.documento on new { DocumentoCobranza = C.i_IdTipoDocumento.Value, eliminado = 0 }
                                                       equals new { DocumentoCobranza = J5.i_CodigoDocumento, eliminado = J5.i_Eliminado.Value } into J5_join
                                      from J5 in J5_join.DefaultIfEmpty()

                                      join J6 in dbContext.documento on new { DocumentoCobranzaDetalle = A.i_IdTipoDocumentoRef.Value, eliminado = 0 }
                                                      equals new { DocumentoCobranzaDetalle = J6.i_CodigoDocumento, eliminado = J6.i_Eliminado.Value } into J6_join
                                      from J6 in J6_join.DefaultIfEmpty()

                                      join J10 in dbContext.documento on new { DocumentoLetraDetalle = J9.i_IdTipoDocumento.Value, eliminado = 0 } equals new { DocumentoLetraDetalle = J10.i_CodigoDocumento, eliminado = J10.i_Eliminado.Value } into J10_join

                                      from J10 in J10_join.DefaultIfEmpty()


                                      join J12 in dbContext.letras on new { l = J9.v_IdLetras, eliminado = 0 } equals new { l = J12.v_IdLetras, eliminado = 0 } into J12_join
                                      from J12 in J12_join.DefaultIfEmpty()

                                      where A.i_Eliminado == 0 && (B.v_CodCliente == pstrCodigoCliente || J11.v_CodCliente == pstrCodigoCliente)
                                      && C.i_IdEstado == 1
                                      select new ReporteEstadoCuentaCliente
                                      {

                                          Fecha = C == null ? DateTime.MinValue : C.t_FechaRegistro.Value,
                                          Cliente = B == null ? J11 == null ? "**NO EXISTE CLIENTE**" : ("CÓDIGO : " + J11.v_CodCliente.Trim() + "   -    DENOMINACIÓN: " + J11.v_ApePaterno.Trim() + " " + J11.v_ApeMaterno.Trim() + " " + J11.v_PrimerNombre.Trim() + " " + J11.v_SegundoNombre.Trim() + " " + J11.v_RazonSocial.Trim()).Trim().ToUpper() : ("CÓDIGO : " + B.v_CodCliente.Trim() + "   -    DENOMINACIÓN: " + B.v_ApePaterno.Trim() + " " + B.v_ApeMaterno.Trim() + " " + B.v_PrimerNombre.Trim() + " " + B.v_SegundoNombre.Trim() + " " + B.v_RazonSocial.Trim()).Trim().ToUpper(),
                                          pIntTipoDocumento = J9 == null ? 0 : J9.i_IdTipoDocumento.Value,
                                          //Concepto = a == null ? J9 == null ? "" : J10.v_Siglas.Trim() + ". " + J9.v_Serie.Trim() + "-" + J9.v_Correlativo : J4.v_Siglas.Trim() + ". " + a.v_SerieDocumento.Trim() + "-" + a.v_CorrelativoDocumento.Trim(), Antes 16 junio
                                          //Concepto = a == null ? J9 == null ? "" : J10.v_Siglas.Trim() + ". " + J9.v_Serie.Trim() + "-" + J9.v_Correlativo : J5.v_Siglas.Trim() + ". " + C.v_Mes.Trim () + "-" + C.v_Correlativo , 16 junio
                                          Concepto = C == null ? J9 == null ? "" : J10.v_Siglas.Trim() + ". " + J9.v_Serie.Trim() + "-" + J9.v_Correlativo : J5.v_Siglas.Trim() + ". " + C.v_Mes.Trim() + "-" + C.v_Correlativo,
                                          //Referencia ="COBRANZA DE " +  J4.v_Siglas +" "+ a.v_SerieDocumento +" "+ a.v_CorrelativoDocumento ,  // C.v_Glosa.Trim(),
                                          Referencia = a == null || J4 == null ? "COBRANZA DE " + J10.v_Siglas + " " + J9.v_Serie + " " + J9.v_Correlativo : "COBRANZA DE " + J4.v_Siglas + " " + a.v_SerieDocumento + " " + a.v_CorrelativoDocumento,  // C.v_Glosa.Trim(),
                                          DebeSoles = 0,
                                          DebeDolares = 0,
                                          HaberSoles = a == null ? J12.i_IdMoneda == (int)Currency.Soles ? C.i_IdMoneda == (int)Currency.Soles ? A.d_ImporteSoles.Value : C.i_IdMoneda.Value == (int)Currency.Dolares ? A.d_ImporteSoles.Value * C.d_TipoCambio.Value : 0 : 0 : A == null ? 0 : a.i_IdMoneda.Value == (int)Currency.Soles ? C.i_IdMoneda.Value == (int)Currency.Soles ? A.d_ImporteSoles.Value : C.i_IdMoneda.Value == (int)Currency.Dolares ? A.d_ImporteSoles.Value * C.d_TipoCambio.Value : 0 : 0,
                                          HaberDolares = a == null ? J12.i_IdMoneda == (int)Currency.Dolares ? C.i_IdMoneda.Value == (int)Currency.Soles ? A.d_ImporteSoles.Value / C.d_TipoCambio.Value : C.i_IdMoneda.Value == (int)Currency.Dolares ? A.d_ImporteSoles.Value : 0 : 0 : A == null ? 0 : a.i_IdMoneda.Value == (int)Currency.Dolares ? C.i_IdMoneda.Value == (int)Currency.Soles ? A.d_ImporteSoles.Value / C.d_TipoCambio.Value : C.i_IdMoneda.Value == (int)Currency.Dolares ? A.d_ImporteSoles.Value : 0 : 0,
                                          IdCobranzaDetalle = A.v_IdCobranzaDetalle,
                                          FechaInsercion = C.t_ActualizaFecha == null ? C.t_InsertaFecha.Value : C.t_ActualizaFecha.Value,
                                          DocumentoReferencia = J6 == null ? "" : J6.v_Siglas.Trim() + " " + A.v_DocumentoRef.Trim(),
                                          Naturaleza = "H",

                                      }).ToList();


                    var LetrasDebe = (from a in dbContext.letras
                                      join b in dbContext.letrasdetalle on new { IdLetras = a.v_IdLetras, eliminado = 0 } equals new { IdLetras = b.v_IdLetras, eliminado = 0 } into b_join
                                      from b in b_join.DefaultIfEmpty()
                                      join e in dbContext.cliente on new { IdCliente = b.v_IdCliente, eliminado = 0 } equals new { IdCliente = e.v_IdCliente, eliminado = e.i_Eliminado.Value } into e_join

                                      from e in e_join.DefaultIfEmpty()
                                      join g in dbContext.documento on new { DocumentoLetrasDellate = b.i_IdTipoDocumento.Value, eliminado = 0 } equals new { DocumentoLetrasDellate = g.i_CodigoDocumento, eliminado = g.i_Eliminado.Value } into g_join

                                      from g in g_join.DefaultIfEmpty()

                                      where a.i_Eliminado == 0 && e.v_CodCliente == pstrCodigoCliente && e.v_FlagPantalla == "C"

                                      select new ReporteEstadoCuentaCliente()
                                      {
                                          Fecha = a == null ? DateTime.MinValue : a.t_FechaRegistro.Value,
                                          Cliente = e == null ? "**NO EXISTE CLIENTE**" : ("CÓDIGO : " + e.v_CodCliente.Trim() + "   -    DENOMINACIÓN: " + e.v_ApePaterno.Trim() + " " + e.v_ApeMaterno.Trim() + " " + e.v_PrimerNombre.Trim() + " " + e.v_SegundoNombre.Trim() + " " + e.v_RazonSocial.Trim()).Trim().ToUpper(),
                                          pIntTipoDocumento = 0,
                                          Concepto = g == null ? b.v_Serie + " " + b.v_Correlativo : g.v_Siglas.Trim() + ".  " + b.v_Serie.Trim() + " " + b.v_Correlativo.Trim(),
                                          Referencia = "REFERENCIA : ", // C.v_Glosa.Trim(),
                                          DebeSoles = a.i_IdMoneda == (int)Currency.Soles ? b.d_Importe.Value : 0,
                                          DebeDolares = a.i_IdMoneda == (int)Currency.Dolares ? b.d_Importe.Value : 0,
                                          HaberSoles = 0,
                                          HaberDolares = 0,
                                          FechaInsercion = b.t_ActualizaFecha == null ? b.t_InsertaFecha.Value : b.t_ActualizaFecha.Value,
                                          DocumentoReferencia = "",
                                          IdLetras = b.v_IdLetras,
                                          Naturaleza = "D",
                                          FechaVenc = b == null ? DateTime.Now : b.t_FechaVencimiento.Value,
                                      }).ToList();



                    var LetrasHaber = (from a in dbContext.letras
                                       join b in dbContext.letrascanje on new { IdLetras = a.v_IdLetras, eliminado = 0 } equals new { IdLetras = b.v_IdLetras, eliminado = 0 } into b_join
                                       from b in b_join.DefaultIfEmpty()


                                       join e in dbContext.cliente on new { IdCliente = a.v_IdCliente, eliminado = 0 } equals new { IdCliente = e.v_IdCliente, eliminado = e.i_Eliminado.Value } into e_join

                                       from e in e_join.DefaultIfEmpty()

                                       join f in dbContext.venta on new { IdVenta = b.v_IdVenta, eliminado = 0 } equals new { IdVenta = f.v_IdVenta, eliminado = f.i_Eliminado.Value } into f_join


                                       from f in f_join.DefaultIfEmpty()

                                       join g in dbContext.documento on new { DocumentoVenta = f.i_IdTipoDocumento.Value, eliminado = 0 } equals new { DocumentoVenta = g.i_CodigoDocumento, eliminado = g.i_Eliminado.Value } into g_join

                                       from g in g_join.DefaultIfEmpty()

                                       join h in dbContext.letrasdetalle on new { IdLetrasDetalle = b.v_IdLetrasDetalle, eliminado = 0 } equals new { IdLetrasDetalle = h.v_IdLetrasDetalle, eliminado = h.i_Eliminado.Value } into h_join

                                       from h in h_join.DefaultIfEmpty()

                                       join i in dbContext.documento on new { DocumentoLetrasDellate = h.i_IdTipoDocumento.Value, eliminado = 0 } equals new { DocumentoLetrasDellate = i.i_CodigoDocumento, eliminado = i.i_Eliminado.Value } into i_join

                                       from i in i_join.DefaultIfEmpty()


                                       where a.i_Eliminado == 0 && e.v_CodCliente == pstrCodigoCliente && e.v_FlagPantalla == "C"

                                       select new ReporteEstadoCuentaCliente()
                                       {
                                           Fecha = a == null ? DateTime.MinValue : a.t_FechaRegistro.Value,
                                           Cliente = e == null ? "**NO EXISTE CLIENTE**" : ("CÓDIGO : " + e.v_CodCliente.Trim() + "   -    DENOMINACIÓN: " + e.v_ApePaterno.Trim() + " " + e.v_ApeMaterno.Trim() + " " + e.v_PrimerNombre.Trim() + " " + e.v_SegundoNombre.Trim() + " " + e.v_RazonSocial.Trim()).Trim().ToUpper(),
                                           pIntTipoDocumento = 0,
                                           Concepto = g == null ? i == null ? "" : i.v_Siglas + ".  " + h.v_Serie + " " + h.v_Correlativo : g.v_Siglas.Trim() + ". " + f.v_SerieDocumento.Trim() + "-" + f.v_CorrelativoDocumento.Trim(),
                                           Referencia = b.v_IdVenta == null ? "REFINANCIACIÓN POR  : " : b.v_IdLetrasDetalle == null ? "CANJE DE " + g.v_Nombre + " POR  : " : "",
                                           // Referencia = b.v_IdVenta == null ? "REFINANCIACIÓN POR  LETRA  " : b.v_IdLetrasDetalle == null ? "CANJE  POR LETRA" : "",
                                           DebeSoles = 0,
                                           DebeDolares = 0,
                                           HaberSoles = a.i_IdMoneda == (int)Currency.Soles ? b.d_MontoCanjeado.Value : 0,
                                           HaberDolares = a.i_IdMoneda == (int)Currency.Dolares ? b.d_MontoCanjeado.Value : 0,
                                           FechaInsercion = b.t_ActualizaFecha == null ? b.t_InsertaFecha.Value : b.t_ActualizaFecha.Value,
                                           DocumentoReferencia = "",
                                           IdLetras = b.v_IdLetras,
                                           Naturaleza = "H",
                                           IdLetrasDetalle = b == null ? "" : b.v_IdLetrasDetalle == null ? "" : b.v_IdLetrasDetalle,
                                       }).ToList();

                    var SaldoInicialesDebe = (from a in dbContext.letrasdetalle
                                              join b in dbContext.cliente on new { IdCliente = a.v_IdCliente, eliminado = 0 } equals new { IdCliente = b.v_IdCliente, eliminado = b.i_Eliminado.Value } into b_join

                                              from b in b_join.DefaultIfEmpty()


                                              join c in dbContext.documento on new { TipoDoc = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { TipoDoc = c.i_CodigoDocumento, eliminado = c.i_Eliminado.Value } into c_join
                                              from c in c_join.DefaultIfEmpty()
                                              where a.i_Eliminado == 0 && b.v_CodCliente == pstrCodigoCliente && a.i_EsSaldoInicial == 1
                                              select new ReporteEstadoCuentaCliente()
                                              {
                                                  Fecha = a == null ? DateTime.MinValue : a.t_FechaEmision.Value,
                                                  Cliente = b == null ? "**NO EXISTE CLIENTE**" : ("CÓDIGO : " + b.v_CodCliente.Trim() + "   -    DENOMINACIÓN: " + b.v_ApePaterno.Trim() + " " + b.v_ApeMaterno.Trim() + " " + b.v_PrimerNombre.Trim() + " " + b.v_SegundoNombre.Trim() + " " + b.v_RazonSocial.Trim()).Trim().ToUpper(),
                                                  pIntTipoDocumento = 0,
                                                  Concepto = c == null ? a.v_Serie + " " + a.v_Correlativo : c.v_Siglas.Trim() + ".  " + a.v_Serie.Trim() + " " + a.v_Correlativo.Trim(),
                                                  Referencia = "LETRA INICIAL ", // C.v_Glosa.Trim(),
                                                  DebeSoles = a.i_IdMoneda == (int)Currency.Soles ? a.d_Importe.Value : 0,
                                                  DebeDolares = a.i_IdMoneda == (int)Currency.Dolares ? a.d_Importe.Value : 0,
                                                  HaberSoles = 0,
                                                  HaberDolares = 0,
                                                  FechaInsercion = a.t_ActualizaFecha == null ? a.t_InsertaFecha.Value : a.t_ActualizaFecha.Value,

                                                  DocumentoReferencia = "",
                                                  IdLetras = a.v_IdLetrasDetalle,
                                                  Naturaleza = "",
                                                  FechaVenc = a == null ? DateTime.Now : a.t_FechaVencimiento.Value,
                                              }).ToList();




                    var RegistrosTotal = queryDebe.Concat(queryHaber).ToList().Concat(LetrasHaber).Concat(LetrasDebe).Concat(SaldoInicialesDebe);
                    var RegistrosTotalFecha = RegistrosTotal.Where(x => x.Fecha >= FechaDesde && x.Fecha <= FechaHasta).ToList().OrderBy(x => x.Fecha).ThenBy(x => x.Concepto).ToList();
                    objReporte = new ReporteEstadoCuentaCliente();
                    objReporte.Cliente = queryDebe.Count() != 0 ? queryDebe.FirstOrDefault().Cliente : queryHaber.Count() != 0 ? queryHaber.FirstOrDefault().Cliente : "";
                    objReporte.pIntTipoDocumento = 1;
                    objReporte.Concepto = "";
                    objReporte.Referencia = " SALDO DEUDOR AL   " + FechaSaldoDeudor.Date.Day.ToString("00") + "/" + FechaSaldoDeudor.Date.Month.ToString("00") + "/" + FechaSaldoDeudor.Date.Year.ToString() + " : ";

                    objReporte.DebeSoles = RegistrosTotal.Where(x => x.Fecha <= FechaSaldoDeudor && !_objDocumentoBL.DocumentoEsInverso(x.pIntTipoDocumento)).ToList().OrderBy(x => x.Fecha).ThenBy(x => x.Concepto).ToList().Sum(x => x.DebeSoles) - RegistrosTotal.Where(x => x.Fecha <= FechaSaldoDeudor && _objDocumentoBL.DocumentoEsInverso(x.pIntTipoDocumento)).ToList().OrderBy(x => x.Fecha).ThenBy(x => x.Concepto).ToList().Sum(x => x.DebeSoles);
                    objReporte.DebeDolares = RegistrosTotal.Where(x => x.Fecha <= FechaSaldoDeudor && !_objDocumentoBL.DocumentoEsInverso(x.pIntTipoDocumento)).ToList().OrderBy(x => x.Fecha).ThenBy(x => x.Concepto).ToList().Sum(x => x.DebeDolares) - RegistrosTotal.Where(x => x.Fecha <= FechaSaldoDeudor && _objDocumentoBL.DocumentoEsInverso(x.pIntTipoDocumento)).ToList().OrderBy(x => x.Fecha).ThenBy(x => x.Concepto).ToList().Sum(x => x.DebeDolares);
                    objReporte.HaberSoles = RegistrosTotal.Where(x => x.Fecha <= FechaSaldoDeudor && !_objDocumentoBL.DocumentoEsInverso(x.pIntTipoDocumento)).ToList().OrderBy(x => x.Fecha).ThenBy(x => x.Concepto).ToList().Sum(x => x.HaberSoles) - RegistrosTotal.Where(x => x.Fecha <= FechaSaldoDeudor && _objDocumentoBL.DocumentoEsInverso(x.pIntTipoDocumento)).ToList().OrderBy(x => x.Fecha).ThenBy(x => x.Concepto).ToList().Sum(x => x.HaberSoles);
                    objReporte.HaberDolares = RegistrosTotal.Where(x => x.Fecha <= FechaSaldoDeudor && !_objDocumentoBL.DocumentoEsInverso(x.pIntTipoDocumento)).ToList().OrderBy(x => x.Fecha).ThenBy(x => x.Concepto).ToList().Sum(x => x.HaberDolares) - RegistrosTotal.Where(x => x.Fecha <= FechaSaldoDeudor && _objDocumentoBL.DocumentoEsInverso(x.pIntTipoDocumento)).ToList().OrderBy(x => x.Fecha).ThenBy(x => x.Concepto).ToList().Sum(x => x.HaberDolares);

                    objReporte.DocumentoReferencia = "";
                    objReporte.Movimiento = "SA";
                    ListaFinal.Add(objReporte);
                    RegistrosTotalFecha = RegistrosTotalFecha.OrderBy(x => x.Fecha).ThenBy(x => x.Naturaleza).ToList();
                    foreach (var item in RegistrosTotalFecha)
                    {
                        string Referencia = string.Empty;
                        string ReferenciaGenera = String.Empty;
                        objReporte = new ReporteEstadoCuentaCliente();
                        objReporte.Fecha = item.Fecha;
                        objReporte.Cliente = item.Cliente;
                        objReporte.pIntTipoDocumento = item.pIntTipoDocumento;
                        objReporte.Concepto = item.Concepto;
                        objReporte.Referencia = item.Referencia;

                        if (item.Naturaleza == "H")
                        {
                            var LetrasGeneradas = (from a in dbContext.letrasdetalle
                                                   join b in dbContext.documento on new { Doc = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { Doc = b.i_CodigoDocumento, eliminado = b.i_Eliminado.Value } into b_join

                                                   from b in b_join.DefaultIfEmpty()

                                                   where a.i_Eliminado == 0 && a.v_IdLetras == item.IdLetras

                                                   select new
                                                   {
                                                       NumeroDocumento = a.v_Serie.Trim() + "-" + a.v_Correlativo.Trim(),
                                                       TipoDocumento = b == null ? "" : b.v_Siglas.Trim(),
                                                   }).ToList().OrderBy(x => x.TipoDocumento).ThenBy(x => x.NumeroDocumento).ToList();
                            int i = 1;

                            if (LetrasGeneradas != null)
                            {
                                foreach (var letra in LetrasGeneradas)
                                {
                                    if (i == LetrasGeneradas.Count())
                                    {
                                        Referencia = Referencia + " " + letra.TipoDocumento + ". " + letra.NumeroDocumento.TrimStart();
                                    }
                                    else
                                    {
                                        Referencia = Referencia + " " + letra.TipoDocumento + ". " + letra.NumeroDocumento.TrimStart() + ",";
                                    }
                                    i = i + 1;
                                }

                            }
                            objReporte.Referencia = (item.Referencia + Referencia).Trim();
                            // objReporte.Referencia = (item.Referencia + " LETRA").Trim();
                        }
                        else
                        {

                            var DeDondeSeGeneraC = (from a in dbContext.letrascanje

                                                    join b in dbContext.venta on new { IdVenta = a.v_IdVenta, eliminado = 0 } equals new { IdVenta = b.v_IdVenta, eliminado = b.i_Eliminado.Value } into b_join

                                                    from b in b_join.DefaultIfEmpty()

                                                    join c in dbContext.documento on new { DocumentoVenta = b.i_IdTipoDocumento.Value, eliminado = 0 } equals new { DocumentoVenta = c.i_CodigoDocumento, eliminado = c.i_Eliminado.Value } into c_join

                                                    from c in c_join.DefaultIfEmpty()

                                                    where a.i_Eliminado == 0 && a.v_IdLetras == item.IdLetras && a.v_IdVenta != null && a.v_IdLetrasDetalle == null

                                                    select new
                                                   {
                                                       NumeroDocumento = c == null && b == null ? " " : c == null ? b.v_SerieDocumento + " " + b.v_CorrelativoDocumento : c.v_Siglas.Trim() + ". " + b.v_SerieDocumento + "-" + b.v_CorrelativoDocumento,


                                                   }).OrderBy(x => x.NumeroDocumento).ToList().Distinct();



                            var DeDondeSeGeneraL = (from a in dbContext.letrascanje

                                                    join d in dbContext.letrasdetalle on new { LetrasDetalle = a.v_IdLetrasDetalle, eliminado = 0 } equals new { LetrasDetalle = d.v_IdLetrasDetalle, eliminado = d.i_Eliminado.Value } into d_join

                                                    from d in d_join.DefaultIfEmpty()

                                                    join e in dbContext.documento on new { DocumentoLD = d.i_IdTipoDocumento.Value, eliminado = 0 } equals new { DocumentoLD = e.i_CodigoDocumento, eliminado = e.i_Eliminado.Value } into e_join

                                                    from e in e_join.DefaultIfEmpty()
                                                    where a.i_Eliminado == 0 && a.v_IdLetras == item.IdLetras && a.v_IdVenta == null && a.v_IdLetrasDetalle != null
                                                    select new
                                                    {
                                                        NumeroDocumento = d == null && e == null ? "" : e == null ? d.v_Serie + " " + d.v_Correlativo : e.v_Siglas + ". " + d.v_Serie + " " + d.v_Correlativo,
                                                    }).OrderBy(x => x.NumeroDocumento).ToList().Distinct();


                            var DeDondeSeGenera = DeDondeSeGeneraC.Concat(DeDondeSeGeneraL);
                            if (DeDondeSeGenera != null)
                            {
                                int contador = 1;
                                foreach (var letra in DeDondeSeGenera)
                                {
                                    var nume = DeDondeSeGenera.Count();
                                    if (DeDondeSeGenera.Count() == contador)
                                    {
                                        ReferenciaGenera = ReferenciaGenera + " " + letra.NumeroDocumento;
                                    }
                                    else
                                    {


                                        ReferenciaGenera = ReferenciaGenera + " " + letra.NumeroDocumento + " , ";
                                    }
                                    contador = contador + 1;
                                }

                            }
                            objReporte.Referencia = (item.Referencia + " " + ReferenciaGenera.Trim()).Trim();
                        }
                        objReporte.DebeSoles = item.DebeSoles;
                        objReporte.DebeDolares = item.DebeDolares;
                        objReporte.HaberSoles = item.HaberSoles;
                        objReporte.HaberDolares = item.HaberDolares;
                        objReporte.DocumentoReferencia = item.DocumentoReferencia;
                        objReporte.Movimiento = "MN";
                        objReporte.FechaInsercion = item.FechaInsercion;

                        var t = item.FechaVenc.Date.ToShortDateString();

                        objReporte.FechaVencimiento = item.FechaVenc.Date.ToShortDateString() == "01/01/0001" ? "" : item.FechaVenc.ToShortDateString();
                        if (_objDocumentoBL.DocumentoEsInverso(objReporte.pIntTipoDocumento)) //En caso solo se haga una Ncr para eliminar una factura ,ésta debe aparecer en el reporte, pero si ha sido generada y utilizada con otra factura ,ésta no debe aparecer
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


                            //if (item.Naturaleza == "V")
                            //{
                            //    if (ExisteLetrasCanje(item.IdLetras, FechaHasta))
                            //    {
                            //    }
                            //    else
                            //    {
                            //        ListaFinal.Add(objReporte);
                            //    }
                            //}
                            //else if (item.Naturaleza =="H")
                            //{
                            //    objReporte.DebeSoles = item.HaberSoles;
                            //    objReporte.HaberSoles = item.DebeSoles;
                            //    objReporte.DebeDolares = item.HaberDolares;
                            //    objReporte.HaberDolares = item.DebeDolares;
                            //    ListaFinal.Add(objReporte);
                            //}
                            //else{
                            //    objReporte.DebeSoles = item.HaberSoles;
                            //    objReporte.HaberSoles = item.DebeSoles;
                            //    objReporte.DebeDolares = item.HaberDolares;
                            //    objReporte.HaberDolares = item.DebeDolares;
                            //    ListaFinal.Add(objReporte);// D
                            //}
                        }
                    }
                    objOperationResult.Success = 1;
                    // return ListaFinal;
                    return ListaFinal.OrderBy(x => x.FechaInsercion).ToList();
                }

            }
            catch (Exception)
            {
                objOperationResult.Success = 0;
                return null;
            }


        }

        public bool ExisteLetrasCanje(string IdReferencia, DateTime FechaReporte)
        {
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {

                var Ventas = (from a in dbContext.letras

                              join b in dbContext.letrascanje on new { v = a.v_IdLetras, eliminado = 0 } equals new { v = b.v_IdLetras, eliminado = b.i_Eliminado.Value } into b_join

                              from b in b_join.DefaultIfEmpty()

                              where b.v_IdVenta == IdReferencia && a.t_FechaRegistro <= FechaReporte

                              select b).ToList();
                if (Ventas.Any())
                {
                    return true;
                }
                else
                {
                    return false;
                }
                // .letrascanje.Where(x => x.v_IdVenta == IdReferencia ).ToList();
                //var Letas = dbContext.letrascanje.Where(x => x.v_IdLetrasDetalle == IdReferencia).ToList();
                //if ( Ventas.Any () 

            }
        }
        #endregion

        #region Tesorerias
        private void GenerarTesoreria(ref OperationResult pobjOperationResult, string strIdCobranza, string[] IdRegistroEliminado = null)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    decimal RedondedoTotalGanancia = 0;
                    decimal RedondedoTotalPerdida = 0;
                    var pobjDtoEntity = dbContext.cobranza.FirstOrDefault(p => p.v_IdCobranza.Equals(strIdCobranza));
                    var pTemp_Insertar = pobjDtoEntity.cobranzadetalle.Where(p => p.i_Eliminado == 0).ToDTOs();
                    var _objTesoreriaBL = new TesoreriaBL();
                    var C_tesoreriaDto = new tesoreriaDto();
                    var _ListadoTesorerias = new List<KeyValueDTO>();
                    var _TesoreriaDetalleXInsertar = new List<tesoreriadetalleDto>();
                    var _TesoreriaDetalleGastos = new List<tesoreriadetalleDto>();
                    var _TesoreriaDetalleIntereses = new List<tesoreriadetalleDto>();
                    var _TesoreriaDetalleRetenciones = new List<tesoreriadetalleDto>();

                    _ListadoTesorerias = _objTesoreriaBL.ObtenerListadoTesorerias(ref pobjOperationResult, pobjDtoEntity.t_FechaRegistro.Value.Year.ToString(), pobjDtoEntity.t_FechaRegistro.Value.Month.ToString("00"), pobjDtoEntity.i_IdTipoDocumento.Value);

                    var _MaxMovimiento = _ListadoTesorerias.Any() ? int.Parse(_ListadoTesorerias[_ListadoTesorerias.Count() - 1].Value1) : 0;
                    _MaxMovimiento++;

                    if (IdRegistroEliminado != null && IdRegistroEliminado[0] != null && IdRegistroEliminado[1] != null &&
                            IdRegistroEliminado[2] != null)
                    {
                        C_tesoreriaDto.v_Mes = pobjDtoEntity.t_FechaRegistro.Value.Year.ToString() !=
                                               IdRegistroEliminado[0] ||
                                               pobjDtoEntity.t_FechaRegistro.Value.Month.ToString("00") !=
                                               IdRegistroEliminado[1].Trim()
                            ? pobjDtoEntity.t_FechaRegistro.Value.Month.ToString("00")
                            : IdRegistroEliminado[1].Trim();
                        C_tesoreriaDto.v_Periodo = pobjDtoEntity.t_FechaRegistro.Value.Year.ToString() !=
                                                   IdRegistroEliminado[0]
                            ? pobjDtoEntity.t_FechaRegistro.Value.Year.ToString()
                            : IdRegistroEliminado[0];
                        C_tesoreriaDto.v_Correlativo = pobjDtoEntity.t_FechaRegistro.Value.Year.ToString() !=
                                                       IdRegistroEliminado[0] ||
                                                       pobjDtoEntity.t_FechaRegistro.Value.Month.ToString("00") !=
                                                       IdRegistroEliminado[1].Trim()
                            ? _MaxMovimiento.ToString("00000000")
                            : IdRegistroEliminado[2];
                    }
                    else
                    {
                        C_tesoreriaDto.v_Mes = pobjDtoEntity.t_FechaRegistro.Value.Month.ToString("00");
                        C_tesoreriaDto.v_Periodo = pobjDtoEntity.t_FechaRegistro.Value.Year.ToString();
                        C_tesoreriaDto.v_Correlativo = _MaxMovimiento.ToString("00000000");
                    }
                    C_tesoreriaDto.d_TipoCambio = pobjDtoEntity.d_TipoCambio.Value;
                    C_tesoreriaDto.i_IdMoneda = pobjDtoEntity.i_IdMoneda.Value;
                    C_tesoreriaDto.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                    C_tesoreriaDto.v_Glosa = pobjDtoEntity.v_Glosa;
                    C_tesoreriaDto.v_NroCuentaCajaBanco = _objTesoreriaBL.DevuelveCuentaCajaBanco(ref pobjOperationResult, pobjDtoEntity.i_IdTipoDocumento.Value).NroCuenta;
                    if (pobjOperationResult.Success == 0) return;
                    C_tesoreriaDto.i_IdEstado = 1;

                    C_tesoreriaDto.i_IdMedioPago = pobjDtoEntity.i_IdMedioPago;
                    C_tesoreriaDto.t_FechaRegistro = pobjDtoEntity.t_FechaRegistro.Value;
                    C_tesoreriaDto.v_IdCobranza = pobjDtoEntity.v_IdCobranza;
                    C_tesoreriaDto.i_TipoMovimiento = (int?)TipoMovimientoTesoreria.Ingreso;

                    #region Agrega cuentas detalle
                    foreach (var Fila in pTemp_Insertar.Where(p => !_objDocumentoBL.DocumentoEsInverso(p.i_IdTipoDocumentoRef.Value)))
                    {
                        if (Fila.i_EsLetra == null || Fila.i_EsLetra == 0)
                        {
                            tesoreriadetalleDto DH_tesoreriadetalleDto = new tesoreriadetalleDto();
                            venta _venta = (dbContext.venta.Where(v => v.v_IdVenta == Fila.v_IdVenta)).FirstOrDefault();
                            string idConcepto = _venta.i_IdTipoVenta.Value.ToString("00");
                            decimal Redondeo  =Fila.d_Redondeo ==null ?0 : Fila.d_Redondeo.Value ;
                            decimal importe = 0;
                            if (Redondeo > 0)
                            {
                                Redondeo = Redondeo < 0 ? Redondeo * -1 : Redondeo;
                                 importe =  (Fila.d_ImporteSoles ?? 0) + (Fila.d_MontoRetencion ?? 0) +Redondeo;
                            }
                            else
                            {
                                
                               importe = (Fila.d_ImporteSoles ?? 0) + (Fila.d_MontoRetencion ?? 0) +Redondeo;
                            }
                            // se agrego por REDONDEOCOBRANZA
                            DH_tesoreriadetalleDto.v_IdTesoreria = C_tesoreriaDto.v_IdTesoreria;
                            DH_tesoreriadetalleDto.d_Cambio = C_tesoreriaDto.i_IdMoneda == 1 ? importe / C_tesoreriaDto.d_TipoCambio : importe * C_tesoreriaDto.d_TipoCambio;
                            DH_tesoreriadetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(DH_tesoreriadetalleDto.d_Cambio ?? 0, 2);

                            DH_tesoreriadetalleDto.d_Importe = importe;
                            DH_tesoreriadetalleDto.i_IdTipoDocumentoRef = Fila.i_IdTipoDocumentoRef;
                            DH_tesoreriadetalleDto.v_Naturaleza = !_objDocumentoBL.DocumentoEsInverso(_venta.i_IdTipoDocumento.Value) ? "H" : "D";

                            DH_tesoreriadetalleDto.v_Analisis = "";
                            DH_tesoreriadetalleDto.v_IdCliente = (dbContext.venta.Join(dbContext.cliente,
                                v => v.v_IdCliente, J1 => J1.v_IdCliente, (v, J1) => new { v, J1 })
                                .Where(@t => @t.v.v_IdVenta == Fila.v_IdVenta)
                                .Select(@t => new { @t.J1.v_IdCliente })).FirstOrDefault().v_IdCliente;

                            var docData = dbContext.documento.FirstOrDefault(p => p.i_CodigoDocumento.Equals(_venta.i_IdTipoDocumento.Value));
                            var esOperacionTransitoria = docData != null && docData.i_OperacionTransitoria.HasValue && docData.i_OperacionTransitoria.Value == 1;

                            if (!esOperacionTransitoria)
                            {
                                DH_tesoreriadetalleDto.v_NroCuenta = (dbContext.administracionconceptos.Where(
                                    v => v.v_Codigo == idConcepto && v.v_Periodo.Equals(periodo))
                                    .Select(v => new { v.v_CuentaPVenta })).FirstOrDefault().v_CuentaPVenta;
                            }
                            else
                                DH_tesoreriadetalleDto.v_NroCuenta = docData.v_NroCuenta;

                            DH_tesoreriadetalleDto.v_NroDocumento = _venta.v_SerieDocumento + "-" + _venta.v_CorrelativoDocumento;
                            DH_tesoreriadetalleDto.v_NroDocumentoRef = Fila.v_DocumentoRef;
                            DH_tesoreriadetalleDto.v_OrigenDestino = "";
                            DH_tesoreriadetalleDto.v_Pedido = "";
                            DH_tesoreriadetalleDto.t_Fecha = _venta.t_FechaRegistro;
                            DH_tesoreriadetalleDto.i_IdTipoDocumento = _venta.i_IdTipoDocumento.Value;
                            DH_tesoreriadetalleDto.i_IdCentroCostos = "0";
                            DH_tesoreriadetalleDto.i_IdCaja = 0;

                            if (_objDocumentoBL.DocumentoEsContable(_venta.i_IdTipoDocumento.Value) || docData.i_OperacionTransitoria == 1)// AGREGO 7DIC.
                            {
                                _TesoreriaDetalleXInsertar.Add(DH_tesoreriadetalleDto);
                            }

                            Fila.d_Redondeo = Fila.d_Redondeo ?? 0;
                            if (Fila.d_Redondeo != 0) //se agrego por REDONDEOCOBRANZA
                            {

                                tesoreriadetalleDto DH_tesoreriadetalleDto1 = new tesoreriadetalleDto();
                                DH_tesoreriadetalleDto1.v_IdTesoreria = C_tesoreriaDto.v_IdTesoreria;
                                DH_tesoreriadetalleDto1.d_Cambio = C_tesoreriaDto.i_IdMoneda == 1 ? Fila.d_Redondeo / C_tesoreriaDto.d_TipoCambio : Fila.d_Redondeo * C_tesoreriaDto.d_TipoCambio;
                                DH_tesoreriadetalleDto1.d_Cambio = DH_tesoreriadetalleDto1.d_Cambio < 0 ? Utils.Windows.DevuelveValorRedondeado(DH_tesoreriadetalleDto1.d_Cambio ?? 0, 2) * -1 : Utils.Windows.DevuelveValorRedondeado(DH_tesoreriadetalleDto1.d_Cambio ?? 0, 2);

                                DH_tesoreriadetalleDto1.d_Importe = Fila.d_Redondeo < 0 ? Fila.d_Redondeo * -1 : Fila.d_Redondeo;
                                DH_tesoreriadetalleDto1.i_IdTipoDocumentoRef = Fila.i_IdTipoDocumentoRef;
                                DH_tesoreriadetalleDto1.v_Naturaleza = Fila.d_Redondeo < 0 ? "H" : "D";    // Ganancia siempre es H 

                                DH_tesoreriadetalleDto1.v_Analisis = "";
                                DH_tesoreriadetalleDto1.v_IdCliente = (dbContext.venta.Join(dbContext.cliente, v => v.v_IdCliente, J1 => J1.v_IdCliente, (v, J1) => new { v, J1 }).Where(@t => @t.v.v_IdVenta == Fila.v_IdVenta)
                                    .Select(@t => new { @t.J1.v_IdCliente })).FirstOrDefault().v_IdCliente;

                                DH_tesoreriadetalleDto1.v_NroCuenta = Fila.d_Redondeo > 0 ? Globals.ClientSession.v_NroCuentaCobranzaRedondeoPerdida : Globals.ClientSession.v_NroCuentaCobranzaRedondeoGanancia;
                                DH_tesoreriadetalleDto1.v_NroDocumento = _venta.v_SerieDocumento + "-" + _venta.v_CorrelativoDocumento;
                                DH_tesoreriadetalleDto1.v_NroDocumentoRef = Fila.v_DocumentoRef;
                                DH_tesoreriadetalleDto1.v_OrigenDestino = "";
                                DH_tesoreriadetalleDto1.v_Pedido = "";
                                DH_tesoreriadetalleDto1.t_Fecha = _venta.t_FechaRegistro;
                                DH_tesoreriadetalleDto1.i_IdTipoDocumento = _venta.i_IdTipoDocumento.Value;
                                DH_tesoreriadetalleDto1.i_IdCentroCostos = "0";
                                DH_tesoreriadetalleDto1.i_IdCaja = 0;
                                if (_objDocumentoBL.DocumentoEsContable(_venta.i_IdTipoDocumento.Value) || docData.i_OperacionTransitoria == 1)// AGREGO 7DIC.
                                {
                                    if (Fila.d_Redondeo > 0)
                                    {
                                        RedondedoTotalGanancia = RedondedoTotalGanancia + Fila.d_Redondeo ?? 0;
                                    }
                                    else
                                    {
                                        RedondedoTotalPerdida = RedondedoTotalPerdida + Fila.d_Redondeo ?? 0;
                                    }
                                    _TesoreriaDetalleXInsertar.Add(DH_tesoreriadetalleDto1);
                                }


                            }
                        }
                        else
                        {
                            tesoreriadetalleDto DH_tesoreriadetalleDto = new tesoreriadetalleDto();

                            letrasdetalle _venta = (dbContext.letrasdetalle.Where(
                                v => v.v_IdLetrasDetalle == Fila.v_IdVenta)).FirstOrDefault();

                            string IDConcepto = _venta.i_IdMoneda == 1 ? "30" : "31";
                            DH_tesoreriadetalleDto.v_IdTesoreria = C_tesoreriaDto.v_IdTesoreria;
                            decimal Redondeo = Fila.d_Redondeo == null ? 0 : Fila.d_Redondeo.Value;
                            decimal importe = 0;
                            if (Redondeo > 0)
                            {
                                Redondeo = Redondeo < 0 ? Redondeo * -1 : Redondeo;
                                importe = (Fila.d_ImporteSoles ?? 0) + (Fila.d_MontoRetencion ?? 0) + Redondeo;
                            }
                            else
                            {

                                importe = (Fila.d_ImporteSoles ?? 0) + (Fila.d_MontoRetencion ?? 0) + Redondeo;
                            }



                            //DH_tesoreriadetalleDto.d_Cambio = C_tesoreriaDto.i_IdMoneda == 1 ? Fila.d_ImporteSoles / C_tesoreriaDto.d_TipoCambio : Fila.d_ImporteSoles * C_tesoreriaDto.d_TipoCambio;
                            //DH_tesoreriadetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(DH_tesoreriadetalleDto.d_Cambio.Value, 2);
                            //DH_tesoreriadetalleDto.d_Importe = Fila.d_ImporteSoles.Value;

                            DH_tesoreriadetalleDto.d_Cambio = C_tesoreriaDto.i_IdMoneda == 1 ? importe / C_tesoreriaDto.d_TipoCambio : importe * C_tesoreriaDto.d_TipoCambio;
                            DH_tesoreriadetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(DH_tesoreriadetalleDto.d_Cambio.Value, 2);
                            DH_tesoreriadetalleDto.d_Importe = importe;


                            DH_tesoreriadetalleDto.i_IdTipoDocumentoRef = Fila.i_IdTipoDocumentoRef;
                            var noEsInverso = !_objDocumentoBL.DocumentoEsInverso(_venta.i_IdTipoDocumento ?? -1);
                           
                            DH_tesoreriadetalleDto.v_Naturaleza = noEsInverso /*&& noEsLetraDescuento*/ ? "H" : "D";

                            DH_tesoreriadetalleDto.v_Analisis = "";
                            DH_tesoreriadetalleDto.v_IdCliente = (dbContext.letrasdetalle.Join(dbContext.cliente,
                                v => v.v_IdCliente, J1 => J1.v_IdCliente, (v, J1) => new { v, J1 })
                                .Where(@t => @t.v.v_IdLetrasDetalle == Fila.v_IdVenta)
                                .Select(@t => new { @t.J1.v_IdCliente })).FirstOrDefault().v_IdCliente;

                            var docData = dbContext.documento.FirstOrDefault(p => p.i_CodigoDocumento.Equals(_venta.i_IdTipoDocumento.Value));
                            var esOperacionTransitoria = docData != null && docData.i_OperacionTransitoria.HasValue && docData.i_OperacionTransitoria.Value == 1;

                            if (!esOperacionTransitoria)
                            {
                                if (!EsLetraDescuento(Fila.v_IdVenta, true))
                                    DH_tesoreriadetalleDto.v_NroCuenta = (dbContext.administracionconceptos.Where(
                                        v => v.v_Codigo == IDConcepto && v.v_Periodo.Equals(periodo))
                                        .Select(v => new { v.v_CuentaPVenta })).FirstOrDefault().v_CuentaPVenta;
                                else
                                {
                                    if (string.IsNullOrWhiteSpace(Globals.ClientSession.NroCtaObligacionesFinancieros) ||
                                        !Utils.Windows.EsCuentaImputable(Globals.ClientSession.NroCtaObligacionesFinancieros))
                                        throw new Exception("La cuenta para obligaciones financieras no está especificada!");

                                    DH_tesoreriadetalleDto.v_NroCuenta = Globals.ClientSession.NroCtaObligacionesFinancieros;
                                }
                            }
                            else
                                DH_tesoreriadetalleDto.v_NroCuenta = docData.v_NroCuenta;

                            DH_tesoreriadetalleDto.v_NroDocumento = _venta.v_Serie + "-" + _venta.v_Correlativo;
                            DH_tesoreriadetalleDto.v_NroDocumentoRef = Fila.v_DocumentoRef;
                            DH_tesoreriadetalleDto.v_OrigenDestino = "";
                            DH_tesoreriadetalleDto.v_Pedido = "";
                            DH_tesoreriadetalleDto.t_Fecha = _venta.t_FechaEmision;
                            DH_tesoreriadetalleDto.i_IdTipoDocumento = _venta.i_IdTipoDocumento ?? -1;
                            DH_tesoreriadetalleDto.i_IdCentroCostos = "0";
                            DH_tesoreriadetalleDto.i_IdCaja = 0;
                            if (docData != null && (_objDocumentoBL.DocumentoEsContable(_venta.i_IdTipoDocumento ?? -1) || docData.i_OperacionTransitoria == 1))// AGREGO 7DIC.
                            {
                                _TesoreriaDetalleXInsertar.Add(DH_tesoreriadetalleDto);
                            }

                            Fila.d_Redondeo = Fila.d_Redondeo ?? 0;
                            if (Fila.d_Redondeo != 0) //se agrego por REDONDEOCOBRANZA
                            {

                                tesoreriadetalleDto DH_tesoreriadetalleDto1 = new tesoreriadetalleDto();
                                DH_tesoreriadetalleDto1.v_IdTesoreria = C_tesoreriaDto.v_IdTesoreria;
                                DH_tesoreriadetalleDto1.d_Cambio = C_tesoreriaDto.i_IdMoneda == 1 ? Fila.d_Redondeo / C_tesoreriaDto.d_TipoCambio : Fila.d_Redondeo * C_tesoreriaDto.d_TipoCambio;
                                DH_tesoreriadetalleDto1.d_Cambio = DH_tesoreriadetalleDto1.d_Cambio < 0 ? Utils.Windows.DevuelveValorRedondeado(DH_tesoreriadetalleDto1.d_Cambio ?? 0, 2) * -1 : Utils.Windows.DevuelveValorRedondeado(DH_tesoreriadetalleDto1.d_Cambio ?? 0, 2);

                                DH_tesoreriadetalleDto1.d_Importe = Fila.d_Redondeo < 0 ? Fila.d_Redondeo * -1 : Fila.d_Redondeo;
                                DH_tesoreriadetalleDto1.i_IdTipoDocumentoRef = Fila.i_IdTipoDocumentoRef;
                                DH_tesoreriadetalleDto1.v_Naturaleza = Fila.d_Redondeo < 0 ? "H" : "D";    // Ganancia siempre es H 

                                DH_tesoreriadetalleDto1.v_Analisis = "";
                                DH_tesoreriadetalleDto1.v_IdCliente = DH_tesoreriadetalleDto.v_IdCliente;
                                DH_tesoreriadetalleDto1.v_NroCuenta = Fila.d_Redondeo > 0 ? Globals.ClientSession.v_NroCuentaCobranzaRedondeoPerdida : Globals.ClientSession.v_NroCuentaCobranzaRedondeoGanancia;
                                DH_tesoreriadetalleDto1.v_NroDocumento = _venta.v_Serie + "-" + _venta.v_Correlativo;
                                DH_tesoreriadetalleDto1.v_NroDocumentoRef = Fila.v_DocumentoRef;
                                DH_tesoreriadetalleDto1.v_OrigenDestino = "";
                                DH_tesoreriadetalleDto1.v_Pedido = "";
                                DH_tesoreriadetalleDto1.t_Fecha = _venta.t_FechaEmision;
                                DH_tesoreriadetalleDto1.i_IdTipoDocumento = _venta.i_IdTipoDocumento.Value;
                                DH_tesoreriadetalleDto1.i_IdCentroCostos = "0";
                                DH_tesoreriadetalleDto1.i_IdCaja = 0;
                                if (_objDocumentoBL.DocumentoEsContable(_venta.i_IdTipoDocumento.Value) || docData.i_OperacionTransitoria == 1)// AGREGO 7DIC.
                                {
                                    if (Fila.d_Redondeo > 0)
                                    {
                                        RedondedoTotalGanancia = RedondedoTotalGanancia + Fila.d_Redondeo ?? 0;
                                    }
                                    else
                                    {
                                        RedondedoTotalPerdida = RedondedoTotalPerdida + Fila.d_Redondeo ?? 0;
                                    }
                                    _TesoreriaDetalleXInsertar.Add(DH_tesoreriadetalleDto1);
                                }


                            }

                        }

                    }
                    #endregion

                    #region Procesa los cobros con documentos inversos
                    foreach (var Fila in pTemp_Insertar.Where(p => _objDocumentoBL.DocumentoEsInverso(p.i_IdTipoDocumentoRef.Value)).GroupBy(g => g.v_IdVenta))
                    {
                        string idVenta = Fila.FirstOrDefault().v_IdVenta;
                        venta _venta = (dbContext.venta.Where(v => v.v_IdVenta == idVenta)).FirstOrDefault();
                        string IDConcepto = _venta.i_IdTipoVenta.Value.ToString("00");

                        string NroCuenta = (dbContext.administracionconceptos.Where(v => v.v_Codigo == IDConcepto && v.v_Periodo.Equals(periodo))
                            .Select(v => new { v.v_CuentaPVenta })).FirstOrDefault().v_CuentaPVenta;

                        var FilaComun =
                            _TesoreriaDetalleXInsertar.FirstOrDefault(
                                p =>
                                    p.v_Naturaleza == "H" && p.v_NroCuenta == NroCuenta &&
                                    p.i_IdTipoDocumento == _venta.i_IdTipoDocumento &&
                                    p.v_NroDocumento == _venta.v_SerieDocumento + "-" + _venta.v_CorrelativoDocumento);

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
                            DH_tesoreriadetalleDto.v_Naturaleza = "H";
                            DH_tesoreriadetalleDto.v_IdCliente = (from v in dbContext.venta
                                                                  join J1 in dbContext.cliente on v.v_IdCliente equals J1.v_IdCliente
                                                                  where v.v_IdVenta == idVenta
                                                                  select new { J1.v_IdCliente }).FirstOrDefault().v_IdCliente;

                            DH_tesoreriadetalleDto.v_Analisis = "";
                            DH_tesoreriadetalleDto.v_NroCuenta = NroCuenta;

                            DH_tesoreriadetalleDto.v_NroDocumento = _venta.v_SerieDocumento + "-" + _venta.v_CorrelativoDocumento;
                            DH_tesoreriadetalleDto.v_NroDocumentoRef = Fila.FirstOrDefault().v_DocumentoRef;
                            DH_tesoreriadetalleDto.v_OrigenDestino = "";
                            DH_tesoreriadetalleDto.v_Pedido = "";
                            DH_tesoreriadetalleDto.t_Fecha = _venta.t_FechaRegistro;
                            DH_tesoreriadetalleDto.i_IdTipoDocumento = _venta.i_IdTipoDocumento.Value;
                            DH_tesoreriadetalleDto.i_IdCentroCostos = "0";
                            DH_tesoreriadetalleDto.i_IdCaja = 0;

                            if (_objDocumentoBL.DocumentoEsContable(_venta.i_IdTipoDocumento.Value))//Agrego 7 DIC
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
                        DD_tesoreriadetalleDto.v_Naturaleza = "D";
                        DD_tesoreriadetalleDto.v_IdCliente = (from v in dbContext.venta
                                                              join J1 in dbContext.cliente on v.v_IdCliente equals J1.v_IdCliente
                                                              where v.v_IdVenta == idVenta
                                                              select new { J1.v_IdCliente }).FirstOrDefault().v_IdCliente;
                        DD_tesoreriadetalleDto.v_Analisis = "";
                        DD_tesoreriadetalleDto.v_NroCuenta = NroCuenta;
                        DD_tesoreriadetalleDto.v_NroDocumento = _venta.v_SerieDocumento + "-" + _venta.v_CorrelativoDocumento;
                        DD_tesoreriadetalleDto.v_NroDocumentoRef = Fila.FirstOrDefault().v_DocumentoRef;
                        DD_tesoreriadetalleDto.v_OrigenDestino = "";
                        DD_tesoreriadetalleDto.v_Pedido = "";
                        DD_tesoreriadetalleDto.t_Fecha = _venta.t_FechaRegistro;
                        DD_tesoreriadetalleDto.i_IdTipoDocumento = _venta.i_IdTipoDocumento.Value;
                        DD_tesoreriadetalleDto.i_IdCentroCostos = "0";
                        DD_tesoreriadetalleDto.i_IdCaja = 0;

                        if (_objDocumentoBL.DocumentoEsContable(_venta.i_IdTipoDocumento.Value))//Agrego 7 DIC
                        {
                            _TesoreriaDetalleXInsertar.Add(DD_tesoreriadetalleDto);
                        }
                    }
                    #endregion

                    #region Agrega los gastos financieros si los hubiera
                    if (pTemp_Insertar.Sum(p => p.d_GastosFinancieros ?? 0) > 0)
                    {
                        var DD_tesoreriadetalleDto = new tesoreriadetalleDto();
                        var ctaGasto = Utils.Windows.DevuelveCuentaDatos(Globals.ClientSession.NroCtaGastosFinancieros);
                        if (ctaGasto == null) throw new ArgumentNullException(@"La cuenta de gasto financiero es inválida.");
                        var Importe = (pTemp_Insertar.Sum(p => p.d_GastosFinancieros ?? 0));

                        DD_tesoreriadetalleDto.v_IdTesoreria = C_tesoreriaDto.v_IdTesoreria;
                        DD_tesoreriadetalleDto.d_Cambio = C_tesoreriaDto.i_IdMoneda == 1 ? Importe / C_tesoreriaDto.d_TipoCambio : Importe * C_tesoreriaDto.d_TipoCambio;
                        DD_tesoreriadetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(DD_tesoreriadetalleDto.d_Cambio.Value, 2);
                        DD_tesoreriadetalleDto.d_Importe = Importe;
                        DD_tesoreriadetalleDto.i_IdTipoDocumentoRef = -1;
                        DD_tesoreriadetalleDto.v_Naturaleza = "D";
                        DD_tesoreriadetalleDto.v_Analisis = "GASTO FINANCIERO";
                        DD_tesoreriadetalleDto.v_NroCuenta = ctaGasto.v_NroCuenta;
                        DD_tesoreriadetalleDto.v_NroDocumento = string.Empty;
                        DD_tesoreriadetalleDto.v_NroDocumentoRef = string.Empty;
                        DD_tesoreriadetalleDto.v_OrigenDestino = "";
                        DD_tesoreriadetalleDto.v_Pedido = "";
                        DD_tesoreriadetalleDto.t_Fecha = C_tesoreriaDto.t_FechaRegistro;
                        DD_tesoreriadetalleDto.i_IdTipoDocumento = 309;
                        DD_tesoreriadetalleDto.i_IdCentroCostos = string.Empty;
                        DD_tesoreriadetalleDto.i_IdCaja = 0;

                        _TesoreriaDetalleGastos.Add(DD_tesoreriadetalleDto);
                    }
                    #endregion

                    #region Agrega la retención
                    if (pTemp_Insertar.Sum(p => p.d_MontoRetencion ?? 0) > 0)
                    {
                        foreach (var tesoreriaDetalle in pTemp_Insertar.Where(p => p.d_MontoRetencion > 0).ToList())
                        {
                            venta _venta = (from v in dbContext.venta
                                            where v.v_IdVenta == tesoreriaDetalle.v_IdVenta
                                            select v).FirstOrDefault();

                            var DD_tesoreriadetalleDto = new tesoreriadetalleDto();
                            var ctaGasto = Utils.Windows.DevuelveCuentaDatos(Globals.ClientSession.v_NroCtaRetenciones);
                            if (ctaGasto == null) throw new ArgumentNullException(@"La cuenta de retenciones para cobranzas es inválida.");
                            var Importe = tesoreriaDetalle.d_MontoRetencion;
                            DD_tesoreriadetalleDto.v_IdTesoreria = C_tesoreriaDto.v_IdTesoreria;
                            DD_tesoreriadetalleDto.d_Cambio = C_tesoreriaDto.i_IdMoneda == 1 ? Importe / C_tesoreriaDto.d_TipoCambio : Importe * C_tesoreriaDto.d_TipoCambio;
                            DD_tesoreriadetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(DD_tesoreriadetalleDto.d_Cambio.Value, 2);
                            DD_tesoreriadetalleDto.d_Importe = Importe;
                            DD_tesoreriadetalleDto.i_IdTipoDocumentoRef = 20;
                            DD_tesoreriadetalleDto.v_Naturaleza = "D";
                            DD_tesoreriadetalleDto.v_Analisis = "RETENCION";
                            DD_tesoreriadetalleDto.v_NroCuenta = ctaGasto.v_NroCuenta;
                            DD_tesoreriadetalleDto.v_NroDocumento = _venta.v_SerieDocumento + "-" + _venta.v_CorrelativoDocumento;
                            DD_tesoreriadetalleDto.v_NroDocumentoRef = tesoreriaDetalle.v_NroRetencion;
                            DD_tesoreriadetalleDto.v_OrigenDestino = "";
                            DD_tesoreriadetalleDto.v_Pedido = "";
                            DD_tesoreriadetalleDto.t_Fecha = C_tesoreriaDto.t_FechaRegistro;
                            DD_tesoreriadetalleDto.i_IdTipoDocumento = _venta.i_IdTipoDocumento.Value;
                            DD_tesoreriadetalleDto.i_IdCentroCostos = string.Empty;
                            DD_tesoreriadetalleDto.i_IdCaja = 0;
                            DD_tesoreriadetalleDto.v_IdCliente = _venta.v_IdCliente;

                            _TesoreriaDetalleRetenciones.Add(DD_tesoreriadetalleDto);
                        }

                    }
                    #endregion

                    #region Agrega los ingresos financieros si los hubiera
                    if (pTemp_Insertar.Sum(p => p.d_IngresosFinancieros ?? 0) > 0)
                    {
                        var DD_tesoreriadetalleDto = new tesoreriadetalleDto();
                        var ctaGasto = Utils.Windows.DevuelveCuentaDatos(Globals.ClientSession.NroCtaIngresosFinancieros);
                        if (ctaGasto == null) throw new ArgumentNullException(@"La cuenta de ingresos financieros es inválida.");
                        var Importe = (pTemp_Insertar.Sum(p => p.d_IngresosFinancieros ?? 0));

                        DD_tesoreriadetalleDto.v_IdTesoreria = C_tesoreriaDto.v_IdTesoreria;
                        DD_tesoreriadetalleDto.d_Cambio = C_tesoreriaDto.i_IdMoneda == 1 ? Importe / C_tesoreriaDto.d_TipoCambio : Importe * C_tesoreriaDto.d_TipoCambio;
                        DD_tesoreriadetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(DD_tesoreriadetalleDto.d_Cambio.Value, 2);
                        DD_tesoreriadetalleDto.d_Importe = Importe;
                        DD_tesoreriadetalleDto.i_IdTipoDocumentoRef = -1;
                        DD_tesoreriadetalleDto.v_Naturaleza = "H";
                        DD_tesoreriadetalleDto.v_Analisis = "INGRESOS FINANCIEROS";
                        DD_tesoreriadetalleDto.v_NroCuenta = ctaGasto.v_NroCuenta;
                        DD_tesoreriadetalleDto.v_NroDocumento = string.Empty;
                        DD_tesoreriadetalleDto.v_NroDocumentoRef = string.Empty;
                        DD_tesoreriadetalleDto.v_OrigenDestino = "";
                        DD_tesoreriadetalleDto.v_Pedido = "";
                        DD_tesoreriadetalleDto.t_Fecha = C_tesoreriaDto.t_FechaRegistro;
                        DD_tesoreriadetalleDto.i_IdTipoDocumento = 309;
                        DD_tesoreriadetalleDto.i_IdCentroCostos = string.Empty;
                        DD_tesoreriadetalleDto.i_IdCaja = 0;

                        _TesoreriaDetalleIntereses.Add(DD_tesoreriadetalleDto);
                    }
                    #endregion

                    if (pTemp_Insertar.Count() == 1)
                    {
                        if (pTemp_Insertar[0].i_EsLetra == null || pTemp_Insertar[0].i_EsLetra == 0)
                        {
                            string IdVenta = pTemp_Insertar[0].v_IdVenta;
                            C_tesoreriaDto.v_Nombre = (from v in dbContext.venta

                                                       join J1 in dbContext.cliente on v.v_IdCliente equals J1.v_IdCliente into J1_join
                                                       from J1 in J1_join.DefaultIfEmpty()

                                                       where v.v_IdVenta == IdVenta

                                                       select new
                                                       {
                                                           Nombre = v.v_IdCliente != "N002-CL000000000" ? (J1.v_PrimerNombre + " " + J1.v_ApePaterno + " " + J1.v_ApeMaterno + " " + J1.v_RazonSocial).Trim() : v.v_NombreClienteTemporal
                                                       }
                                                       ).FirstOrDefault().Nombre;
                        }
                        else
                        {
                            string IdVenta = pTemp_Insertar[0].v_IdVenta;
                            C_tesoreriaDto.v_Nombre = (from v in dbContext.letrasdetalle

                                                       join J1 in dbContext.cliente on v.v_IdCliente equals J1.v_IdCliente into J1_join
                                                       from J1 in J1_join.DefaultIfEmpty()

                                                       where v.v_IdLetrasDetalle == IdVenta

                                                       select new
                                                       {
                                                           Nombre = v.v_IdCliente != "N002-CL000000000" ? (J1.v_PrimerNombre + " " + J1.v_ApePaterno + " " + J1.v_ApeMaterno + " " + J1.v_RazonSocial).Trim() : "PÚBLICO GENERAL"
                                                       }
                                                       ).FirstOrDefault().Nombre;
                        }

                    }
                    else
                    {
                        C_tesoreriaDto.v_Nombre = "COBRANZAS VARIOS";
                    }

                    if (_TesoreriaDetalleXInsertar.Count(p => !_objDocumentoBL.DocumentoEsInverso(p.i_IdTipoDocumentoRef ?? 0)) > 0)
                    {
                        #region Se elabora la contracuenta cuando se cobra una letra que no esta en descuento
                        {
                            var DD_tesoreriadetalleDto = new tesoreriadetalleDto();
                            var cobranzaEfectiva = _TesoreriaDetalleXInsertar.Where(o => !_objDocumentoBL.DocumentoEsInverso(o.i_IdTipoDocumentoRef ?? 0)).Sum(p => p.d_Importe ?? 0);
                            var cobranzaEnDocumentosInversos = _TesoreriaDetalleXInsertar.Where(o => _objDocumentoBL.DocumentoEsInverso(o.i_IdTipoDocumentoRef ?? 0)).Sum(p => p.d_Importe ?? 0);
                            var retenciones = _TesoreriaDetalleRetenciones.Sum(p => p.d_Importe ?? 0);
                            var gastos = _TesoreriaDetalleGastos.Sum(p => p.d_Importe ?? 0);
                            var intereses = _TesoreriaDetalleIntereses.Sum(p => p.d_Importe ?? 0);
                            //var importe = cobranzaEfectiva - cobranzaEnDocumentosInversos - gastos - retenciones + intereses;
                            decimal importe = 0;
                            if (RedondedoTotalGanancia != 0 )
                            {
                                var Debe = _TesoreriaDetalleXInsertar.Where(o => !_objDocumentoBL.DocumentoEsInverso(o.i_IdTipoDocumentoRef ?? 0)).Where (o=>o.v_Naturaleza =="D").Sum(p => p.d_Importe ?? 0);
                                var Haber= _TesoreriaDetalleXInsertar.Where(o => !_objDocumentoBL.DocumentoEsInverso(o.i_IdTipoDocumentoRef ?? 0)).Where(o => o.v_Naturaleza == "H").Sum(p => p.d_Importe ?? 0);
                                 cobranzaEfectiva = Debe - Haber;
                                cobranzaEfectiva = cobranzaEfectiva < 0 ? cobranzaEfectiva * -1 : cobranzaEfectiva;
                                importe = cobranzaEfectiva + cobranzaEnDocumentosInversos - gastos - retenciones + intereses; // se agrego por REDONDEOCOBRANZA
                                importe = importe < 0 ? importe * -1 : importe;
                            } 
                            else
                            {
                                 importe = cobranzaEfectiva - cobranzaEnDocumentosInversos - gastos - retenciones + intereses;
                            }
                            var idTipoDocRef = _TesoreriaDetalleXInsertar.LastOrDefault() != null
                                ? _TesoreriaDetalleXInsertar.LastOrDefault().i_IdTipoDocumentoRef ?? -1
                                : -1;
                            DD_tesoreriadetalleDto.v_IdTesoreria = C_tesoreriaDto.v_IdTesoreria;
                            DD_tesoreriadetalleDto.d_Cambio = C_tesoreriaDto.i_IdMoneda == 1
                                ? importe / C_tesoreriaDto.d_TipoCambio
                                : importe * C_tesoreriaDto.d_TipoCambio;
                            DD_tesoreriadetalleDto.d_Cambio =
                                Utils.Windows.DevuelveValorRedondeado(DD_tesoreriadetalleDto.d_Cambio ?? 0, 2);
                            DD_tesoreriadetalleDto.d_Importe = importe;
                            DD_tesoreriadetalleDto.i_IdTipoDocumentoRef = _TesoreriaDetalleXInsertar.Count == 1
                                ? _TesoreriaDetalleXInsertar[0].i_IdTipoDocumento ?? -1
                                : -1;
                            DD_tesoreriadetalleDto.v_Naturaleza = "D";
                            DD_tesoreriadetalleDto.v_Analisis = "";
                            DD_tesoreriadetalleDto.v_NroCuenta = C_tesoreriaDto.v_NroCuentaCajaBanco;
                            DD_tesoreriadetalleDto.v_NroDocumento = _TesoreriaDetalleXInsertar[0].v_NroDocumentoRef;
                            DD_tesoreriadetalleDto.v_NroDocumentoRef = _TesoreriaDetalleXInsertar.Count == 1
                                ? _TesoreriaDetalleXInsertar[0].v_NroDocumento
                                : string.Empty;
                            DD_tesoreriadetalleDto.v_OrigenDestino = "";
                            DD_tesoreriadetalleDto.v_Pedido = "";
                            DD_tesoreriadetalleDto.t_Fecha = C_tesoreriaDto.t_FechaRegistro;
                            DD_tesoreriadetalleDto.i_IdTipoDocumento = idTipoDocRef;
                            DD_tesoreriadetalleDto.i_IdCentroCostos = "0";
                            DD_tesoreriadetalleDto.i_IdCaja = 0;
                            if (Utils.Windows.CuentaRequiereDetalle(DD_tesoreriadetalleDto.v_NroCuenta))
                            {
                                var filaCliente = _TesoreriaDetalleXInsertar.FirstOrDefault(p => p.v_IdCliente != null);
                                if (filaCliente != null)
                                    DD_tesoreriadetalleDto.v_IdCliente = filaCliente.v_IdCliente;
                            }
                            _TesoreriaDetalleXInsertar.Add(DD_tesoreriadetalleDto);

                        }

                        #endregion

                        
                    }

                    _TesoreriaDetalleXInsertar = _TesoreriaDetalleXInsertar.Concat(_TesoreriaDetalleGastos)
                            .Concat(_TesoreriaDetalleIntereses).Concat(_TesoreriaDetalleRetenciones).ToList();
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
                        if (pobjOperationResult.Success == 0) return;
                    }
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "CobranzaBL.GenerarTesoreria()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = pobjOperationResult.ExceptionMessage != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        private static string[] EliminarTesoreria(ref OperationResult pobjOperationResult, string strIdCobranza)
        {
            try
            {
                var IdRegistroEliminado = new TesoreriaBL().EliminarTesoreriaXDocRef(ref pobjOperationResult, strIdCobranza, Globals.ClientSession.GetAsList());
                if (pobjOperationResult.Success == 0) return null;
                pobjOperationResult.Success = 1;
                return IdRegistroEliminado;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "CobranzaBL.EliminarTesoreria()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = pobjOperationResult.ExceptionMessage != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public void RegenerarTesoreria(ref OperationResult pobjOperationResult, string strIdCobranza)
        {
            try
            {
                var reg = EliminarTesoreria(ref pobjOperationResult, strIdCobranza);
                if (pobjOperationResult.Success == 0) return;
                GenerarTesoreria(ref pobjOperationResult, strIdCobranza, reg);
                if (pobjOperationResult.Success == 0) return;
                pobjOperationResult.Success = 1;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "CobranzaBL.RegenerarTesoreria()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = pobjOperationResult.ExceptionMessage != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }
        #endregion

        private class Ubicacion
        {
            public string Siglas { get; set; }
            public string NombreCompleto { get; set; }
            public string Estado { get; set; }
        }

        /// <summary>
        /// Retorna una coleccion de cadenas que contiene los ids de las cobranzas seun la venta ingresada.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="idTipoDocumento"></param>
        /// <param name="serie"></param>
        /// <param name="correlativo"></param>
        /// <returns></returns>
        public List<string> ObtenerCobranzasPorVenta(ref OperationResult pobjOperationResult, int idTipoDocumento,
            string serie, string correlativo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var idVenta =
                        dbContext.venta.FirstOrDefault(
                            p =>
                                p.i_Eliminado == 0 && p.v_SerieDocumento.Contains(serie) &&
                                p.v_CorrelativoDocumento.Contains(correlativo) &&
                                p.i_IdTipoDocumento.Value == idTipoDocumento);
                    pobjOperationResult.Success = 1;

                    if (idVenta == null) return null;

                    var result = dbContext.cobranzadetalle.Where(p => p.v_IdVenta == idVenta.v_IdVenta && p.i_Eliminado == 0).ToList().Select(p => p.v_IdCobranza).Distinct().ToList();

                    return result;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "VentaBL.ObtenerCobranzasPorVenta()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public static int ObtenerFormaPagoPorDocumento(ref OperationResult pobjOperationResult, int idTipoDocumento)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var result = dbContext.relacionformapagodocumento.FirstOrDefault(p => p.i_CodigoDocumento == idTipoDocumento);
                    return result == null ? -1 : result.i_IdFormaPago;
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public void AcualizarSaldosCobranzasPendientes(ref OperationResult objOperationResult, List<SaldosCancelados> ListaCancelados)
        {

            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {

                    objOperationResult.Success = 1;

                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {

                        foreach (var item in ListaCancelados)
                        {
                            var CobranzaPendiente = (from a in dbContext.venta

                                                     join b in dbContext.cobranzapendiente on new { venta = a.v_IdVenta, eliminado = 0 } equals new { venta = b.v_IdVenta, eliminado = b.i_Eliminado.Value } into b_join

                                                     from b in b_join.DefaultIfEmpty()
                                                     where a.i_Eliminado == 0 && a.i_IdTipoDocumento == item.TipoDoc && a.v_SerieDocumento.Trim() == item.Serie.Trim() && a.v_CorrelativoDocumento.Trim() == item.Correlativo.Trim()



                                                     select b).FirstOrDefault();
                            decimal Total = 0;
                            var Venta = dbContext.venta.Where(l => l.i_IdTipoDocumento == item.TipoDoc && l.i_Eliminado.Value == 0 && l.v_SerieDocumento.Trim() == item.Serie.Trim() && l.v_CorrelativoDocumento == item.Correlativo.Trim()).FirstOrDefault();
                            if (Venta != null)
                            {

                                Total = Venta.d_Total.Value;

                            }
                            if (CobranzaPendiente != null)
                            {
                                CobranzaPendiente.d_Saldo = 0;
                                CobranzaPendiente.d_Acuenta = Total;

                                dbContext.cobranzapendiente.ApplyCurrentValues(CobranzaPendiente);
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

        /// <summary>
        /// Retorna verdadero si la letra en consulta está en descuento.
        /// </summary>
        /// <param name="pstrIdLetraDetalle"></param>
        /// <returns></returns>
        public static bool EsLetraDescuento(string pstrIdLetraDetalle, bool noIncluirRenovadas)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var consulta = dbContext.letrasmantenimientodetalle.Where(
                        p => p.v_IdLetrasDetalle == pstrIdLetraDetalle && p.i_Eliminado == 0).ToList();
                    var letradetalle = dbContext.letrasdetalle.FirstOrDefault(p => p.v_IdLetrasDetalle.Equals(pstrIdLetraDetalle));
                    if (letradetalle == null) return false;
                    if (int.Parse(letradetalle.v_Correlativo.Split('-')[1]) > 0 && noIncluirRenovadas) return false;

                    if (consulta.Count <= 0) return false;
                    {
                        consulta = consulta.OrderBy(p => p.v_IdLetrasMantenimiento).ToList();
                        var ultimoRegistro = consulta.Last();
                        var estado = dbContext.datahierarchy.FirstOrDefault(p => p.i_GroupId == 110 && p.i_ItemId == ultimoRegistro.i_IdEstado);
                        return estado != null && estado.v_Value1.Contains("DESCUENTO");
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Indica si la letra tiene un abono ya ingresado, ya que el abono no se puede ingresar mas de una vez.
        /// </summary>
        /// <param name="pstrIdLetraDetalle"></param>
        /// <returns></returns>
        public static bool EsAbonoLetraDescuento(string pstrIdLetraDetalle)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var letra =
                        dbContext.letrasdetalle.FirstOrDefault(p => p.v_IdLetrasDetalle.Equals(pstrIdLetraDetalle));

                    if (letra != null)
                    {
                        var esLetraRenovada = int.Parse(letra.v_Correlativo.Split('-')[1]) > 0;
                        var tieneAbono =
                            dbContext.cobranzadetalle.Any(
                                p =>
                                    p.v_IdVenta.Equals(pstrIdLetraDetalle) && p.i_Eliminado == 0 &&
                                    p.i_EsAbonoLetraDescuento == 1);

                        return !tieneAbono && !esLetraRenovada;
                    }

                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Revisa si alguna letra en cobranza esta ha sido asignada como cancelada o protestada
        /// </summary>
        /// <param name="pstrIdCobranza"></param>
        /// <returns></returns>
        public static List<string> LetrasEnCobranzaFueronCanceladasProtestadas(string pstrIdCobranza)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var itemsCobranza =
                        dbContext.cobranzadetalle.Where(p => p.v_IdCobranza.Equals(pstrIdCobranza) && p.i_Eliminado == 0)
                            .Select(o => o.v_IdVenta);

                    var letras = (from n in dbContext.letrasdescuentomantenimiento
                        join l in dbContext.letrasdetalle on new {id = n.v_IdLetrasDetalle, e = 0}
                            equals new {id = l.v_IdLetrasDetalle, e = l.i_Eliminado.Value} into lJoin
                        from l in lJoin.DefaultIfEmpty()
                        where itemsCobranza.Contains(n.v_IdLetrasDetalle) && n.i_Eliminado == 0
                        select l.v_Serie + " " + l.v_Correlativo).ToList();

                    return letras;
                } 
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

    }

    /// <summary>
    /// Clase para insertar las cobranzas masivamente
    /// </summary>
    public class CobranzaMigracion
    {
        public delegate void OnInsercion(string pstrCobranza);
        public delegate void OnError(OperationResult objOperationResult);
        public event OnInsercion OnInsercionEvent;
        public event OnError OnErrorEvent;
        private readonly BackgroundWorker _bwk;
        private readonly BackgroundWorker _bwkEliminacion;
        private List<cobranzaDto> _cobranzasParaInsertar;
        private string[] _idsEliminar;
        public string[] IdsEliminar
        {
            set { _idsEliminar = value; }
        }

        public CobranzaMigracion()
        {
            _bwk = new BackgroundWorker();
            _bwkEliminacion = new BackgroundWorker();
        }

        public List<cobranzaDto> CobranzasParaInsertar
        {
            set { _cobranzasParaInsertar = value; }
        }

        public void ComenzarAsync()
        {
            _bwk.DoWork += bwk_DoWork;
            _bwk.RunWorkerCompleted += bwk_RunWorkerCompleted;
            _bwk.RunWorkerAsync();
        }

        private void bwk_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (OnInsercionEvent != null)
                OnInsercionEvent("Proceso Terminado!");
        }

        private void bwk_DoWork(object sender, DoWorkEventArgs e)
        {
            var pobjOperationResult = new OperationResult();
            try
            {
                var total = _cobranzasParaInsertar.Count;
                var progreso = 0;
                foreach (var cobranza in _cobranzasParaInsertar.AsParallel())
                {
                    new CobranzaBL().InsertarCobranza(ref pobjOperationResult, cobranza,
                        Globals.ClientSession.GetAsList(), cobranza.CobranzadetalleDtos);

                    if (pobjOperationResult.Success == 1)
                    {
                        progreso++;
                        if (OnInsercionEvent != null)
                            OnInsercionEvent(progreso * 100 / total + "%");
                    }
                    else
                        throw new Exception("Error en inserción.");
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "CobranzaMigracion.Comenzar()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                if (OnErrorEvent != null)
                    OnErrorEvent(pobjOperationResult);
            }
        }

        protected virtual void OnOnInsercionEvent(string pstrcobranza)
        {
            var handler = OnInsercionEvent;
            if (handler != null) handler(pstrcobranza);
        }

        public void ComenzarEliminacionAsync()
        {
            _bwkEliminacion.DoWork += _bwkEliminacion_DoWork;
            _bwkEliminacion.RunWorkerCompleted += bwk_RunWorkerCompleted;
            _bwkEliminacion.RunWorkerAsync();
        }

        private void _bwkEliminacion_DoWork(object sender, DoWorkEventArgs e)
        {
            var pobjOperationResult = new OperationResult();
            try
            {
                if (_idsEliminar == null) throw new Exception("No se especificó el array de ids.");
                var total = _idsEliminar.ToList().Count;
                var progreso = 0;
                foreach (var cobranza in _idsEliminar.ToList())
                {
                    new CobranzaBL().EliminarCobranza(ref pobjOperationResult, cobranza.Trim(), Globals.ClientSession.GetAsList());

                    if (pobjOperationResult.Success == 1)
                    {
                        progreso++;
                        if (OnInsercionEvent != null)
                            OnInsercionEvent(progreso * 100 / total + "%");
                    }
                    else
                        throw new Exception("Error en eliminación.");
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "CobranzaMigracion.ComenzarEliminacionAsync()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                if (OnErrorEvent != null)
                    OnErrorEvent(pobjOperationResult);
            }
        }
    }

    public class RegenerarAsientosCobranzaWorker
    {
        public delegate void Proceso(int porcentaje);
        public delegate void Error(OperationResult pobjOperationResult);

        public event Proceso ProcesoEvent;
        public event Error ErrorEvent;
        public event EventHandler FinalizadoEvent;

        public void ComenzarAsync(int pintMes, int pintPeriodo)
        {
            var pobjOperationResult = new OperationResult();
            try
            {
                Task.Factory.StartNew(() =>
                {
                    lock (this)
                    {
                        using (var ts = TransactionUtils.CreateTransactionScope())
                        {
                            using (var dbContext = new SAMBHSEntitiesModelWin())
                            {
                                var pobjDtoEntities =
                                    dbContext.cobranza.Where(p =>
                                            p.t_FechaRegistro.Value.Month == pintMes && p.t_FechaRegistro.Value.Year == pintPeriodo &&
                                            p.i_Eliminado == 0).ToDTOs();

                                var total = pobjDtoEntities.Count;
                                var pos = 0;

                                foreach (var pobjDtoEntity in pobjDtoEntities)
                                {
                                    pos++;
                                    new CobranzaBL().RegenerarTesoreria(ref pobjOperationResult, pobjDtoEntity.v_IdCobranza);
                                    if (pobjOperationResult.Success == 0) return;
                                    if (ProcesoEvent != null)
                                        ProcesoEvent((pos * 100) / total);
                                }

                                ts.Complete();
                                pobjOperationResult.Success = 1;

                                if (ProcesoEvent != null)
                                    ProcesoEvent(100);

                                if (FinalizadoEvent != null)
                                    FinalizadoEvent(this, new EventArgs());
                            }
                        }
                    }

                }, TaskCreationOptions.LongRunning);
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "RegenerarAsientosCobranzaWorker.ComenzarAsync()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                if (ErrorEvent != null)
                    ErrorEvent(pobjOperationResult);
            }
        }




        public void ComenzarAsyncCobranzasNotaria(int pintMes, int pintPeriodo)
        {
            var pobjOperationResult = new OperationResult();
            try
            {
                pobjOperationResult.Success = 1;
                Task.Factory.StartNew(() =>
                {
                    lock (this)
                    {
                        using (var ts = TransactionUtils.CreateTransactionScope())
                        {
                            using (var dbContext = new SAMBHSEntitiesModelWin())
                            {
                                var pos = 0;
                                var teso = dbContext.tesoreria.Where (o=>o.i_Eliminado ==0).ToList ();
                                
                                #region Cobranzas
                                
                                var pobjDtoEntitiesDetalles =
                                    dbContext.cobranzadetalle.Where(p => p.i_Eliminado == 0).ToDTOs();

                                var Tesorerias = (from a in dbContext.tesoreria
                                                  where a.i_Eliminado == 0
                                                  select new
                                                  {
                                                      i_IdTipoDocumento = a.i_IdTipoDocumento,
                                                      Mes = a.v_Mes.Trim(),
                                                      Correlativo = a.v_Correlativo.Trim(),
                                                      v_IdCobranza = a.v_IdCobranza,
                                                  }).ToList().Select(o => new tesoreriaDto

                                                      {
                                                          v_IdCobranza = o.v_IdCobranza,
                                                          v_Correlativo = o.i_IdTipoDocumento.ToString() + o.Mes + o.Correlativo,

                                                      }).ToList();

                                
                                var SoloTesoreriasLibres = teso.Where(o => o.v_IdCobranza == null).ToList();
                                var formasPago = dbContext.relacionformapagodocumento.ToList();

                                var pobjDtoEntities =
                                   dbContext.cobranza.Where(p =>
                                           p.t_FechaRegistro.Value.Month == pintMes && p.t_FechaRegistro.Value.Year == pintPeriodo &&
                                           p.i_Eliminado == 0).ToList();

                                var total = pobjDtoEntities.Count * 2;
                              

                                foreach (var pobjDtoEntity in pobjDtoEntities)
                                {
                                    pos++;
                                    var cobranzadetalle = pobjDtoEntitiesDetalles.Where(o => o.v_IdCobranza == pobjDtoEntity.v_IdCobranza && o.i_Eliminado ==0).FirstOrDefault();
                                    relacionformapagodocumento rfp = new relacionformapagodocumento();


                                    if (pobjDtoEntity.v_Correlativo == "01000029")
                                    {
                                        string j = "";
                                    }
                                    if (cobranzadetalle != null)
                                    {
                                        rfp = formasPago.Where(o => o.i_IdFormaPago == cobranzadetalle.i_IdFormaPago ).FirstOrDefault();
                                    }
                                    

                                    if (cobranzadetalle.i_IdFormaPago == 2 || cobranzadetalle.i_IdFormaPago == 3)
                                    {
                                        pobjDtoEntity.i_IdTipoDocumento = rfp != null ? rfp.i_CodigoDocumento.Value : -1;
                                        dbContext.cobranza.ApplyCurrentValues(pobjDtoEntity);
                                        dbContext.SaveChanges();
                                        new CobranzaBL().RegenerarTesoreria(ref pobjOperationResult, pobjDtoEntity.v_IdCobranza);
                                    }
                                    if (pobjOperationResult.Success == 0) return;
                                    if (ProcesoEvent != null)
                                        ProcesoEvent((pos * 100) / total);

                                }
                                dbContext.SaveChanges();
                                
                                #endregion
                                
                                #region ReeditarCorrelativos

                              
                                var CobranzasRegeneradas =
                                  dbContext.cobranza.Where(p =>
                                          p.t_FechaRegistro.Value.Month == pintMes && p.t_FechaRegistro.Value.Year == pintPeriodo &&
                                          p.i_Eliminado == 0 &&  (p.i_IdTipoDocumento ==445  ||p.i_IdTipoDocumento ==446) ).ToList();



                                foreach (
                                    var operacionesPorMes in
                                        CobranzasRegeneradas.GroupBy(
                                            p =>
                                                new
                                                {
                                                    mes = p.t_FechaRegistro.Value.Month,
                                                    establecimiento = p.v_IdCobranza.Substring(2, 2)
                                                }))
                                {


                                    var listaOperacionesPorMes = operacionesPorMes.OrderBy(x => x.i_IdTipoDocumento).ThenBy(o => o.v_Mes)
                                        .ThenBy(p => p.v_Correlativo).ToList();

                                    var pTipoDoc = listaOperacionesPorMes.FirstOrDefault().i_IdTipoDocumento.Value;
                                    int counter = 0;
                                    //int counterTesoreria = 0;
                                    pos = 0;
                                    foreach (var operacion in listaOperacionesPorMes)
                                    {
                                        pos++;
                                        if (pTipoDoc == operacion.i_IdTipoDocumento)
                                        {
                                            counter++;
                                        }
                                        else
                                        {
                                            counter = 1;
                                        }

                                      
                                         
                                        var idAlmacen = int.Parse(operacion.v_IdCobranza.Substring(2, 2));
                                        var correlativo = idAlmacen.ToString("00") + counter.ToString("000000");

                                        if (correlativo == "01000521" || correlativo == "01000520")
                                        {
                                            string j = "";
                                        }
                                        var mes = operacion.t_FechaRegistro.Value.Month.ToString("00");
                                        operacion.v_Mes = mes;
                                        operacion.v_Correlativo = correlativo;
                                        var TesoreriaRelacionada = teso.FirstOrDefault(o => o.v_IdCobranza == operacion.v_IdCobranza);
                                        dbContext.cobranza.ApplyCurrentValues(operacion);
                                        if (TesoreriaRelacionada != null)
                                        {
                                            //counterTesoreria = counter;
                                            //while (SoloTesoreriasLibres.Select(o => o.i_IdTipoDocumento.ToString() + o.v_Mes.Trim() + o.v_Correlativo.Trim()).Contains(operacion.i_IdTipoDocumento + operacion.v_Mes.Trim() + counterTesoreria.ToString("000000")))
                                            //{

                                            //    counterTesoreria = counterTesoreria + 1;
                                            //}
                                            TesoreriaRelacionada.i_IdTipoDocumento = operacion.i_IdTipoDocumento;
                                            TesoreriaRelacionada.v_Mes = mes;
                                            TesoreriaRelacionada.v_Correlativo = operacion.v_Correlativo;  // idAlmacen.ToString("00") + counterTesoreria.ToString("000000");
                                            dbContext.tesoreria.ApplyCurrentValues(TesoreriaRelacionada);
                                            dbContext.SaveChanges();

                                        }
                                        pTipoDoc = operacion.i_IdTipoDocumento.Value;

                                    }


                                }
                                
                                #endregion

                                dbContext.SaveChanges();
                                ts.Complete();
                                pobjOperationResult.Success = 1;

                                if (ProcesoEvent != null)
                                    ProcesoEvent(100);

                                if (FinalizadoEvent != null)
                                    FinalizadoEvent(this, new EventArgs());
                            }
                        }
                    }

                }, TaskCreationOptions.LongRunning);
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "RegenerarAsientosCobranzaWorker.ComenzarAsync()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                if (ErrorEvent != null)
                    ErrorEvent(pobjOperationResult);
            }
        }





        public void ComenzarAsyncEditarCorrelativos(int pintMes, int pintPeriodo)
        {
            var pobjOperationResult = new OperationResult();
            try
            {
                Task.Factory.StartNew(() =>
                {
                    lock (this)
                    {
                        using (var ts = TransactionUtils.CreateTransactionScope())
                        {
                            using (var dbContext = new SAMBHSEntitiesModelWin())
                            {


                                var pobjDtoEntitiesDetalles =
                                    dbContext.cobranzadetalle.Where(p => p.i_Eliminado == 0).ToDTOs();

                                var Tesorerias = (from a in dbContext.tesoreria
                                                  where a.i_Eliminado == 0
                                                  select new
                                                  {
                                                      i_IdTipoDocumento = a.i_IdTipoDocumento,
                                                      Mes = a.v_Mes.Trim(),
                                                      Correlativo = a.v_Correlativo.Trim(),
                                                      v_IdCobranza = a.v_IdCobranza,
                                                  }).ToList().Select(o => new tesoreriaDto

                                                      {
                                                          v_IdCobranza = o.v_IdCobranza,
                                                          v_Correlativo = o.i_IdTipoDocumento.ToString() + o.Mes + o.Correlativo,

                                                      }).ToList();




                                var teso = dbContext.tesoreria.ToList();
                                var SoloTesoreriasLibres = teso.Where(o => o.v_IdCobranza == null).ToList();
                                var formasPago = dbContext.relacionformapagodocumento.ToList();

                                var pobjDtoEntities =
                                   dbContext.cobranza.Where(p =>
                                           p.t_FechaRegistro.Value.Month == pintMes && p.t_FechaRegistro.Value.Year == pintPeriodo &&
                                           p.i_Eliminado == 0).ToList();

                                var total = pobjDtoEntities.Count;
                                var pos = 0;
                                #region ReeditarCorrelativos

                                var CobranzasRegeneradas =
                                  dbContext.cobranza.Where(p =>
                                          p.t_FechaRegistro.Value.Month == pintMes && p.t_FechaRegistro.Value.Year == pintPeriodo &&
                                          p.i_Eliminado == 0).ToList();



                                foreach (
                                    var operacionesPorMes in
                                        CobranzasRegeneradas.GroupBy(
                                            p =>
                                                new
                                                {
                                                    mes = p.t_FechaRegistro.Value.Month,
                                                    establecimiento = p.v_IdCobranza.Substring(2, 2)
                                                }))
                                {


                                    var listaOperacionesPorMes = operacionesPorMes.OrderBy(x => x.i_IdTipoDocumento).ThenBy(o => o.v_Mes)
                                        .ThenBy(p => p.v_Correlativo).ToList();

                                    var pTipoDoc = listaOperacionesPorMes.FirstOrDefault().i_IdTipoDocumento.Value;
                                    int counter = 0;
                                    int counterTesoreria = 0;
                                    foreach (var operacion in listaOperacionesPorMes)
                                    {
                                        pos++;

                                        if (operacion.i_IdTipoDocumento == 202)
                                        {
                                            string h = "";
                                        }
                                        if (pTipoDoc == operacion.i_IdTipoDocumento)
                                        {
                                            counter++;
                                        }
                                        else
                                        {
                                            counter = 1; ;
                                        }
                                        var idAlmacen = int.Parse(operacion.v_IdCobranza.Substring(2, 2));
                                        var correlativo = idAlmacen.ToString("00") + counter.ToString("000000");
                                        var mes = operacion.t_FechaRegistro.Value.Month.ToString("00");
                                        operacion.v_Mes = mes;
                                        operacion.v_Correlativo = correlativo;
                                        var TesoreriaRelacionada = teso.FirstOrDefault(o => o.v_IdCobranza == operacion.v_IdCobranza);
                                        dbContext.cobranza.ApplyCurrentValues(operacion);
                                        if (TesoreriaRelacionada != null)
                                        {
                                            counterTesoreria = counter;
                                            while (SoloTesoreriasLibres.Select(o => o.i_IdTipoDocumento.ToString() + o.v_Mes.Trim() + o.v_Correlativo.Trim()).Contains(operacion.i_IdTipoDocumento + operacion.v_Mes.Trim() + counterTesoreria.ToString("000000")))
                                            {

                                                counterTesoreria = counterTesoreria + 1;
                                            }
                                            TesoreriaRelacionada.i_IdTipoDocumento = operacion.i_IdTipoDocumento;
                                            TesoreriaRelacionada.v_Mes = mes;
                                            TesoreriaRelacionada.v_Correlativo = idAlmacen.ToString("00") + counterTesoreria.ToString("000000");
                                            dbContext.tesoreria.ApplyCurrentValues(TesoreriaRelacionada);

                                        }
                                        pTipoDoc = operacion.i_IdTipoDocumento.Value;

                                    }


                                }

                                #endregion

                                dbContext.SaveChanges();
                                ts.Complete();
                                pobjOperationResult.Success = 1;

                                if (ProcesoEvent != null)
                                    ProcesoEvent(100);

                                if (FinalizadoEvent != null)
                                    FinalizadoEvent(this, new EventArgs());
                            }
                        }
                    }

                }, TaskCreationOptions.LongRunning);
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "RegenerarAsientosCobranzaWorker.ComenzarAsync()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                if (ErrorEvent != null)
                    ErrorEvent(pobjOperationResult);
            }
        }


        protected virtual void OnProcesoEvent(int mensaje)
        {
            var handler = ProcesoEvent;
            if (handler != null) handler(mensaje);
        }
    }
}