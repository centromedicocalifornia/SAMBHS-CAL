using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using SAMBHS.Common.BL;
using System.Transactions;
using SAMBHS.Tesoreria.BL;

namespace SAMBHS.Cobranza.BL
{
    public class AdelantoBL
    {
        public List<KeyValueDTO> ObtenerListadoAdelantos(ref OperationResult pobjOperationResult, string pstringPeriodo, string pstringMes)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var query = from n in dbContext.adelanto
                    where n.i_Eliminado == 0 && n.v_Periodo == pstringPeriodo && n.v_Mes == pstringMes
                    orderby n.v_Correlativo ascending
                    select new
                    {
                        n.v_Correlativo, n.v_IdAdelanto
                    };
                if (query.Any())
                {
                    var query2 = query.AsEnumerable()
                        .Select(x => new KeyValueDTO
                        {
                            Value1 = x.v_Correlativo,
                            Value2 = x.v_IdAdelanto
                        }).ToList();
                    return query2;
                }
                else
                {
                    var query2 = query.AsEnumerable()
                        .Select(x => new KeyValueDTO
                        {
                            Value1 = "0",
                            Value2 = null
                        }).ToList();
                    return query2;
                } 
            }
        }

        public string InsertarAdelanto(ref OperationResult pobjOperationResult, adelantoDto pobjDtoEntity, List<string> ClientSession)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        SecuentialBL objSecuentialBL = new SecuentialBL();
                        adelanto objEntityAdelanto = pobjDtoEntity.ToEntity();
                        int SecuentialId = 0;
                        string newIdAdelanto = string.Empty;

                        #region Inserta Cabecera
                        objEntityAdelanto.t_InsertaFecha = DateTime.Now;
                        objEntityAdelanto.i_InsertaIdUsuario = int.Parse(ClientSession[2]);
                        objEntityAdelanto.i_Eliminado = 0;

                        var intNodeId = int.Parse(ClientSession[0]);
                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 50);
                        newIdAdelanto = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "ZG");
                        objEntityAdelanto.v_IdAdelanto = newIdAdelanto;
                        objEntityAdelanto.d_Saldo = pobjDtoEntity.d_Importe ?? 0;
                        dbContext.AddToadelanto(objEntityAdelanto);
                        dbContext.SaveChanges();
                        #endregion

                        #region Actualiza Correlativo EmpresaDetalle
                        new DocumentoBL().ActualizarCorrelativoPorSerie(ref pobjOperationResult, Globals.ClientSession.i_IdEstablecimiento, 433, pobjDtoEntity.v_SerieDocumento, int.Parse(pobjDtoEntity.v_CorrelativoDocumento) + 1);
                        if (pobjOperationResult.Success == 0) return null;
                        #endregion

                        #region Genera libro diario
                        //GenerarLibroDiario(ref pobjOperationResult, objEntityAdelanto.ToDTO());
                        //if (pobjOperationResult.Success == 0) return null;
                        #endregion

                        pobjOperationResult.Success = 1;
                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "adelanto", newIdAdelanto);
                        ts.Complete();
                        return newIdAdelanto; 
                    }
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AdelantoBL.InsertarAdelanto()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public void ActualizarAdelanto(ref OperationResult pobjOperationResult, adelantoDto pobjDtoEntity, List<string> ClientSession)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        int intNodeId;

                        #region Actualiza Cabecera
                        intNodeId = int.Parse(ClientSession[0]);

                        var objEntity = dbContext.adelanto.FirstOrDefault(a => a.v_IdAdelanto == pobjDtoEntity.v_IdAdelanto);

                        objEntity = pobjDtoEntity.ToEntity();
                        objEntity.d_Saldo = pobjDtoEntity.d_Importe ?? 0;
                        objEntity.t_ActualizaFecha = DateTime.Now;
                        objEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        dbContext.adelanto.ApplyCurrentValues(objEntity);
                        #endregion

                        #region Actualiza el asiento
                        //new DiarioBL().EliminarDiarioXDocRef(ref pobjOperationResult, objEntity.v_IdAdelanto, ClientSession, false);
                        //if (pobjOperationResult.Success == 0) return;
                        //GenerarLibroDiario(ref pobjOperationResult, objEntity.ToDTO());
                        //if (pobjOperationResult.Success == 0) return;
                        #endregion

                        dbContext.SaveChanges();
                        pobjOperationResult.Success = 1;
                        Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "adelanto", pobjDtoEntity.v_IdAdelanto);
                        ts.Complete(); 
                    }
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AdelantoBL.ActualizarAdelanto()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public void EliminarAdelanto(ref OperationResult pobjOperationResult, string pstrIdAdelanto, List<string> ClientSession)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {

                        #region Elimina Cabecera
                        // Obtener la entidad fuente
                        var objEntitySource = (from a in dbContext.adelanto
                                               where a.v_IdAdelanto == pstrIdAdelanto
                                               select a).FirstOrDefault();

                        // Crear la entidad con los datos actualizados
                        objEntitySource.t_ActualizaFecha = DateTime.Now;
                        objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntitySource.i_Eliminado = 1;
                        #endregion

                        #region Elimina el asiento contable.
                        //new DiarioBL().EliminarDiarioXDocRef(ref pobjOperationResult, pstrIdAdelanto, ClientSession, false);
                        //if (pobjOperationResult.Success == 0) return;
                        #endregion

                        dbContext.SaveChanges();
                        Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "adelanto", pstrIdAdelanto);
                        pobjOperationResult.Success = 1;
                        ts.Complete(); 
                    }
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AdelantoBL.EliminarAdelanto()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public adelantoDto DevuelveAdelanto(string pstrIdAdelanto)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var adelantoDto = (from a in dbContext.adelanto
                                       join J1 in dbContext.cliente on a.v_IdCliente equals J1.v_IdCliente into J1_join
                                       from J1 in J1_join.DefaultIfEmpty()
                                       where a.v_IdAdelanto == pstrIdAdelanto
                                       select new adelantoDto
                                       {
                                           v_Correlativo = a.v_Correlativo,
                                           v_CorrelativoDocumento = a.v_CorrelativoDocumento,
                                           v_Glosa = a.v_Glosa,
                                           v_IdAdelanto = a.v_IdAdelanto,
                                           v_IdCliente = a.v_IdCliente,
                                           v_Mes = a.v_Mes.Trim(),
                                           v_Periodo = a.v_Periodo,
                                           v_SerieDocumento = a.v_SerieDocumento,
                                           NombreCliente =
                                               (J1.v_PrimerNombre + " " + J1.v_ApePaterno + " " + J1.v_ApeMaterno + " " + J1.v_RazonSocial)
                                                   .Trim(),
                                           d_Importe = a.d_Importe,
                                           d_TipoCambio = a.d_TipoCambio,
                                           i_IdMoneda = a.i_IdMoneda,
                                           t_FechaAdelanto = a.t_FechaAdelanto,
                                           i_Eliminado = a.i_Eliminado,
                                           i_ActualizaIdUsuario = a.i_ActualizaIdUsuario,
                                           i_InsertaIdUsuario = a.i_InsertaIdUsuario,
                                           i_IdTipoDocumento = a.i_IdTipoDocumento,
                                           t_InsertaFecha = a.t_InsertaFecha,
                                           t_ActualizaFecha = a.t_ActualizaFecha,
                                           d_Consumo = a.d_Consumo,
                                           d_Saldo = a.d_Saldo,
                                           i_IdDocumentoCaja = a.i_IdDocumentoCaja ?? -1
                                       }
                                ).FirstOrDefault();
                    return adelantoDto; 
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<adelantoDto> DevuelveListadoAdelantos(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression, DateTime F_Ini, DateTime F_Fin)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query =  from n in dbContext.adelanto

                                 join J1 in dbContext.cliente on n.v_IdCliente equals J1.v_IdCliente into J1_join
                                 from J1 in J1_join.DefaultIfEmpty()

                                 join J2 in dbContext.systemuser on new { i_UpdateUserId = n.i_ActualizaIdUsuario.Value }
                                                                 equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                                 from J2 in J2_join.DefaultIfEmpty()

                                 join J3 in dbContext.systemuser on new { i_InsertUserId = n.i_InsertaIdUsuario.Value }
                                                                equals new { i_InsertUserId = J3.i_SystemUserId } into J3_join
                                 from J3 in J3_join.DefaultIfEmpty()

                                 where n.i_Eliminado == 0 && n.t_FechaAdelanto >= F_Ini && n.t_FechaAdelanto <= F_Fin

                                 select new adelantoDto
                                 {
                                     v_IdAdelanto = n.v_IdAdelanto,
                                     v_IdCliente = n.v_IdCliente,
                                     v_Glosa = n.v_Glosa,
                                     t_FechaAdelanto = n.t_FechaAdelanto,
                                     i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                     i_Eliminado = n.i_Eliminado,
                                     i_IdMoneda = n.i_IdMoneda,
                                     v_UsuarioModificacion = J2.v_UserName,
                                     v_UsuarioCreacion = J3.v_UserName,
                                     t_ActualizaFecha = n.t_ActualizaFecha,
                                     t_InsertaFecha = n.t_InsertaFecha,
                                     d_Importe = n.d_Importe,
                                     NombreCliente = (J1.v_PrimerNombre + " " + J1.v_ApePaterno + " " + J1.v_ApeMaterno + " " + J1.v_RazonSocial).Trim(),
                                     NroRegistro = n.v_SerieDocumento + "-" + n.v_CorrelativoDocumento
                                 };
                    if (!string.IsNullOrEmpty(pstrFilterExpression))
                    {
                        query = query.Where(pstrFilterExpression);
                    }
                    if (!string.IsNullOrEmpty(pstrSortExpression))
                    {
                        query = query.OrderBy(pstrSortExpression);
                    }

                    var objData = query.ToList();
                    pobjOperationResult.Success = 1;
                    return objData;                   
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AdelantoBL.DevuelveListadoAdelantos()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public string DevolverTipoCambioPorFecha(ref OperationResult pobjOperationResult, DateTime Fecha)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from n in dbContext.tipodecambio
                                 where n.i_IsDeleted == 0 && n.d_FechaTipoC == Fecha
                                 select n
                                         ).FirstOrDefault();

                    pobjOperationResult.Success = 1;

                    return query != null ? query.d_ValorVenta.ToString() : "0";
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public bool ExisteNroRegistro(string Periodo, string Mes, string Correlativo)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var registro = (from n in dbContext.venta
                                where n.i_Eliminado == 0 && n.v_Periodo == Periodo && n.v_Mes == Mes && n.v_Correlativo == Correlativo
                                select n).FirstOrDefault();

                return registro == null;
            }
        }

        public bool ExisteDocumento(string pstrSerie, string pstrCorrelativo)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var registro = (from n in dbContext.venta
                                where n.i_Eliminado == 0 && n.v_SerieDocumento == pstrSerie && n.v_CorrelativoDocumento == pstrCorrelativo
                                select n).FirstOrDefault();

                return registro == null;
            }
        }

        public List<KeyValueDTO> BuscarProveedoresParaCombo(ref OperationResult pobjOperationResult, string pstrRucRazonSocial, string Flag)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var query = (from n in dbContext.cliente
                    where n.i_Eliminado == 0 &&
                          n.v_PrimerNombre.Contains(pstrRucRazonSocial) | n.v_SegundoNombre.Contains(pstrRucRazonSocial) | n.v_ApeMaterno.Contains(pstrRucRazonSocial)
                          | n.v_ApePaterno.Contains(pstrRucRazonSocial) | n.v_RazonSocial.Contains(pstrRucRazonSocial) | n.v_NroDocIdentificacion.Contains(pstrRucRazonSocial)
                          && n.v_FlagPantalla == Flag && pstrRucRazonSocial.Trim() != string.Empty

                    orderby n.v_RazonSocial ascending
                    select new
                    {
                        v_IdCliente = n.v_IdCliente,
                        v_RazonSocial = (n.v_PrimerNombre + " " + n.v_ApePaterno + " " + n.v_ApeMaterno + " " + n.v_RazonSocial).Trim(),
                        v_NroDocIdentificacion = n.v_NroDocIdentificacion
                    }
                );

                var query2 = query.AsEnumerable()
                    .Select(x => new KeyValueDTO
                    {
                        Id = x.v_IdCliente,
                        Value1 = x.v_NroDocIdentificacion + " | " + x.v_RazonSocial
                    }).ToList();

                return query2; 
            }
        }

        public bool DevuelveEstadoBotonGuardar(string pstrSerie, string pstrCorrelativo)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var correlativo = pstrSerie + "-" + pstrCorrelativo;

                var Result = (from n in dbContext.cobranzadetalle
                    where n.i_IdTipoDocumentoRef == 433 && n.v_DocumentoRef == correlativo && n.i_Eliminado == 0
                    select n).FirstOrDefault();

                return Result == null;

            }
        }

        public List<ReporteDocumentoVoucherAdelanto> ReporteDocumentoVoucherAdelanto(string pstrIdAdelanto)
        {
            NodeBL objNodeBL = new NodeBL();
            OperationResult objOperationResult = new OperationResult();
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var _intNodeId = int.Parse(Utils.Windows.GetApplicationConfigValue("NodeId"));

                var x = objNodeBL.GetNodeByNodeId(ref objOperationResult, _intNodeId);

                var query1 = (from n in dbContext.adelanto.AsParallel()
                    join B in dbContext.cliente.AsParallel() on n.v_IdCliente equals B.v_IdCliente into B_join
                    from B in B_join.DefaultIfEmpty()
                    join J4 in dbContext.documento.AsParallel() on new { i_IdTipoDocumento = n.i_IdTipoDocumento ?? 433 }
                    equals new { i_IdTipoDocumento = J4.i_CodigoDocumento } into J4_join
                    from J4 in J4_join.DefaultIfEmpty()
                    where n.v_IdAdelanto == pstrIdAdelanto
                    select new ReporteDocumentoVoucherAdelanto
                    {
                        NombreEmpresaPropietaria = x.v_RazonSocial,
                        RucEmpresaPropietaria = x.v_RUC,
                        NroVocuher = n.v_Periodo.Trim() + "-" + n.v_Mes.Trim() + "-" + n.v_Correlativo.Trim(),
                        TipoDocumento = J4.v_Siglas,
                        NroDocumento = n.v_SerieDocumento + "-" + n.v_CorrelativoDocumento,
                        NombreCliente =
                        (B.v_ApePaterno.Trim() + " " + B.v_ApeMaterno.Trim() + " " + B.v_PrimerNombre.Trim() + " " +
                         B.v_SegundoNombre.Trim() + " " + B.v_RazonSocial.Trim()).Trim(),
                        Moneda = n.i_IdMoneda == 1 ? "S/." : "$.",
                        Fecha = n.t_FechaAdelanto ?? DateTime.Now,
                        TipoCambio = n.d_TipoCambio ?? 1,
                        Importe = n.d_Importe,
                        Glosa = n.v_Glosa
                    }).ToList();

                var objData = query1.ToList();

                return objData; 
            }
        }

        private static void GenerarLibroDiario(ref OperationResult pobjOperationResult, adelantoDto objAdelanto)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region Genera Libro Diario

                    if (string.IsNullOrWhiteSpace(Globals.ClientSession.NroCtaAdelanto) ||
                        !Utils.Windows.EsCuentaImputable(Globals.ClientSession.NroCtaAdelanto))
                        throw new Exception("La cuenta de obligaciones financieras no está configurada correctamente.!");

                    var documento = dbContext.documento.FirstOrDefault(
                            p => p.i_CodigoDocumento == objAdelanto.i_IdDocumentoCaja.Value && p.i_Eliminado == 0);

                    if (documento == null)
                        throw new Exception("El documento caja del anticipo ya no existe.");

                    var ctaCaja = documento.v_NroCuenta;

                    if (string.IsNullOrWhiteSpace(ctaCaja) ||
                       !Utils.Windows.EsCuentaImputable(ctaCaja))
                        throw new Exception("La cuenta de precio de venta en administración de conceptos no se encuentra correctamente configurada.!");

                    var objDiarioBl = new DiarioBL();
                    var diarioDto = new diarioDto();
                    var tempXInsertar = new List<diariodetalleDto>();

                    #region Diario Cabecera
                    var documentoDiario = 335;
                    var listadoDiarios = objDiarioBl.ObtenerListadoDiario(ref pobjOperationResult,
                        objAdelanto.t_FechaAdelanto.Value.Year.ToString(),
                        objAdelanto.t_FechaAdelanto.Value.Month.ToString("00"), documentoDiario);
                    var cliente = dbContext.cliente.FirstOrDefault(p => p.v_IdCliente.Equals(objAdelanto.v_IdCliente));
                    if (cliente == null) throw new Exception("El cliente ligado a este adelanto no existe.");
                    var maxMovimiento = listadoDiarios.Any() ? int.Parse(listadoDiarios[listadoDiarios.Count - 1].Value1) : 0;
                    maxMovimiento++;
                    diarioDto.v_IdDocumentoReferencia = objAdelanto.v_IdAdelanto;
                    diarioDto.v_Periodo = objAdelanto.t_FechaAdelanto.Value.Year.ToString();
                    diarioDto.v_Mes = int.Parse(objAdelanto.t_FechaAdelanto.Value.Month.ToString()).ToString("00");
                    diarioDto.v_Glosa = "ASIENTO DE ANTICIPO";
                    diarioDto.v_Nombre =
                        (cliente.v_ApePaterno + " " + cliente.v_ApeMaterno + " " + cliente.v_PrimerNombre + " " +
                         cliente.v_RazonSocial).Trim();

                    diarioDto.v_Correlativo = maxMovimiento.ToString("00000000");
                    diarioDto.d_TipoCambio = objAdelanto.d_TipoCambio ?? 1;
                    diarioDto.i_IdMoneda = objAdelanto.i_IdMoneda ?? 1;
                    diarioDto.i_IdTipoDocumento = documentoDiario;
                    diarioDto.t_Fecha = objAdelanto.t_FechaAdelanto.Value;
                    diarioDto.i_IdTipoComprobante = 2;

                    #endregion

                    #region Ventas Canjeadas
                    var subTotal = objAdelanto.d_Importe;
                    var dSubTotalVenta = new diariodetalleDto
                    {
                        d_Importe = subTotal > 0 ? subTotal : subTotal * -1,
                        d_Cambio = (objAdelanto.i_IdMoneda ?? 1) == 1
                            ? Utils.Windows.DevuelveValorRedondeado(
                                subTotal ?? 0 / objAdelanto.d_TipoCambio ?? 1, 2)
                            : Utils.Windows.DevuelveValorRedondeado(
                                subTotal ?? 0 * objAdelanto.d_TipoCambio ?? 1, 2),
                        i_IdCentroCostos = "0",
                        i_IdTipoDocumento = objAdelanto.i_IdTipoDocumento ?? -1,
                        t_Fecha = objAdelanto.t_FechaAdelanto.Value,
                        v_IdCliente = objAdelanto.v_IdCliente,
                        v_Naturaleza = "D",
                        v_NroDocumento = objAdelanto.v_SerieDocumento + "-" + objAdelanto.v_CorrelativoDocumento,
                        v_NroCuenta = ctaCaja,
                        i_IdTipoDocumentoRef = -1
                    };

                    tempXInsertar.Add(dSubTotalVenta);

                    var hSubTotalVenta = new diariodetalleDto
                    {
                        d_Importe = subTotal > 0 ? subTotal : subTotal * -1,
                        d_Cambio = (objAdelanto.i_IdMoneda ?? 1) == 1
                            ? Utils.Windows.DevuelveValorRedondeado(
                                subTotal ?? 0 / objAdelanto.d_TipoCambio ?? 1, 2)
                            : Utils.Windows.DevuelveValorRedondeado(
                                subTotal ?? 0 * objAdelanto.d_TipoCambio ?? 1, 2),
                        i_IdCentroCostos = "0",
                        i_IdTipoDocumento = objAdelanto.i_IdTipoDocumento ?? -1,
                        t_Fecha = objAdelanto.t_FechaAdelanto.Value,
                        v_IdCliente = objAdelanto.v_IdCliente,
                        v_Naturaleza = "H",
                        v_NroDocumento = objAdelanto.v_SerieDocumento + "-" + objAdelanto.v_CorrelativoDocumento,
                        v_NroCuenta = Globals.ClientSession.NroCtaAdelanto,
                        i_IdTipoDocumentoRef = -1
                    };
                    tempXInsertar.Add(hSubTotalVenta);
                    #endregion

                    var totDebe = tempXInsertar.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Importe.Value);
                    var totHaber = tempXInsertar.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Importe.Value);
                    var totDebeC = tempXInsertar.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Cambio.Value);
                    var totHaberC = tempXInsertar.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Cambio.Value);

                    diarioDto.d_TotalDebe =
                        decimal.Parse(Math.Round(totDebe, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                    diarioDto.d_TotalHaber =
                        decimal.Parse(Math.Round(totHaber, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                    diarioDto.d_TotalDebeCambio =
                        decimal.Parse(Math.Round(totDebeC, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                    diarioDto.d_TotalHaberCambio =
                        decimal.Parse(Math.Round(totHaberC, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                    diarioDto.d_DiferenciaDebe =
                        decimal.Parse(
                            Math.Round(totDebe - totHaber, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                    diarioDto.d_DiferenciaHaber =
                        decimal.Parse(
                            Math.Round(totDebeC - totHaberC, 2, MidpointRounding.AwayFromZero).ToString("0.00"));

                    objDiarioBl.InsertarDiario(ref pobjOperationResult, diarioDto,
                        Globals.ClientSession.GetAsList(), tempXInsertar, (int)TipoMovimientoTesoreria.Ingreso);
                    if (pobjOperationResult.Success == 0) return;

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;

                    #endregion
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation =
                    "LetrasCobrarDescuentoBl.GenerarLibroDiario()\nLinea:" +
                    ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static List<adelantoDto> ObtenerAdelantosParaCanje(ref OperationResult pobjOperationResult, int pintPeriodo,
            int pintMes, string pstrIdCliente)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    pstrIdCliente = pstrIdCliente ?? "";
                    var consulta =
                        dbContext.adelanto.Where(
                            p =>
                                p.i_Eliminado == 0 && (p.d_Saldo ?? 0) > 0 &&
                                p.t_FechaAdelanto.Value.Year == pintPeriodo
                                && p.t_FechaAdelanto.Value.Month == pintMes
                                && (pstrIdCliente == "" || p.v_IdCliente.Equals(pstrIdCliente)));

                    pobjOperationResult.Success = 1;
                    return consulta.ToDTOs();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AdelantoBL.ObtenerAdelantosParaCanje()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        /// <summary>
        /// Recalcula el saldo del adelanto.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstrSerie"></param>
        /// <param name="pstrCorrelativo"></param>
        /// <param name="pintIdTipoDocumento"></param>
        public void RecalcularSaldoAdelanto(ref OperationResult pobjOperationResult, string pstrSerie, string pstrCorrelativo, int pintIdTipoDocumento)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var objAdelanto =
                            dbContext.adelanto.FirstOrDefault(
                                p =>
                                    p.v_SerieDocumento.Equals(pstrSerie) &&
                                    p.v_CorrelativoDocumento.Equals(pstrCorrelativo) && p.i_Eliminado == 0 && p.i_IdTipoDocumento.Value == pintIdTipoDocumento);

                        if (objAdelanto != null)
                        {
                            objAdelanto.d_Consumo = 0;
                            objAdelanto.d_Saldo = objAdelanto.d_Importe;
                            dbContext.SaveChanges();
                            var serieCorrelativo = string.Format("{0}-{1}", objAdelanto.v_SerieDocumento,
                                objAdelanto.v_CorrelativoDocumento);

                            IEnumerable<cobranzadetalle> objCobranzas = dbContext.cobranzadetalle
                                .Where(p => p.i_IdTipoDocumentoRef.Value == pintIdTipoDocumento
                                            &&
                                            p.v_DocumentoRef.Equals(serieCorrelativo)
                                            && p.i_Eliminado == 0).ToList();

                            if (objCobranzas.Any())
                            {
                                foreach (var cobranza in objCobranzas.Where(p => p.cobranza.i_IdEstado == 1))
                                {
                                    objAdelanto.d_Consumo += cobranza.d_ImporteSoles ?? 0;
                                    objAdelanto.d_Saldo -= cobranza.d_ImporteSoles ?? 0;
                                }

                                dbContext.SaveChanges();
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

        public IEnumerable<ReporteAdelantoFactura> ObtenerReporteAdelantoFacturas(ref OperationResult pobjOperationResult, DateTime pFechaIni, DateTime pFechaFin,
            string pstrIdCliente, string serieCorrelativoAdelanto = null)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var tipoDocumentoAdelanto = 433;
                    pFechaFin = DateTime.Parse(pFechaFin.ToShortDateString() + " 23:59");
                    pstrIdCliente = string.IsNullOrWhiteSpace(pstrIdCliente) ? null : pstrIdCliente.Trim();

                    #region Obtiene Adelantos
                    var adelantos = (from p in dbContext.adelanto
                                     join J1 in dbContext.cliente on p.v_IdCliente equals J1.v_IdCliente into J1_join
                                     from J1 in J1_join.DefaultIfEmpty()
                                     where p.i_Eliminado == 0 && p.t_FechaAdelanto >= pFechaIni && p.t_FechaAdelanto <= pFechaFin &&
                                           (pstrIdCliente == null || p.v_IdCliente.Equals(pstrIdCliente))
                                     select new ReporteAdelantoFactura
                                     {
                                         EsAdelanto = true,
                                         Fecha = p.t_FechaAdelanto ?? DateTime.Now,
                                         IdCliente = p.v_IdCliente,
                                         ImporteAdelanto = p.d_Importe ?? 0,
                                         ImporteFactura = 0,
                                         Moneda = p.i_IdMoneda == 1 ? "S" : "D",
                                         NroDocumento = p.v_SerieDocumento + "-" + p.v_CorrelativoDocumento,
                                         NroDocumentoVenta = p.v_SerieDocumento + "-" + p.v_CorrelativoDocumento,
                                         TipoDocumentoVenta = "ANT",
                                         ApellidosNombres =
                                             (J1.v_ApePaterno + " " + J1.v_ApeMaterno + " " + J1.v_PrimerNombre + " " +
                                              J1.v_RazonSocial).Trim(),
                                         NroDocIdentidad = J1.v_NroDocIdentificacion,
                                         TipoCambio = p.d_TipoCambio ?? 0
                                     }).ToList();
                    #endregion

                    var cobranzas = (from n in dbContext.cobranzadetalle
                                     join J1 in dbContext.venta on n.v_IdVenta equals J1.v_IdVenta into J1_join
                                     from J1 in J1_join.DefaultIfEmpty()
                                     join J2 in dbContext.cliente on J1.v_IdCliente equals J2.v_IdCliente into J2_join
                                     from J2 in J2_join.DefaultIfEmpty()
                                     join J3 in dbContext.cobranza on n.v_IdCobranza equals J3.v_IdCobranza into J3_join
                                     from J3 in J3_join.DefaultIfEmpty()
                                     join J4 in dbContext.documento on J1.i_IdTipoDocumento equals J4.i_CodigoDocumento into J4_join
                                     from J4 in J4_join.DefaultIfEmpty()
                                     where
                                         J3.t_FechaRegistro >= pFechaIni && J3.t_FechaRegistro <= pFechaFin &&
                                         n.i_IdTipoDocumentoRef == tipoDocumentoAdelanto
                                         && (pstrIdCliente == null || J1.v_IdCliente.Equals(pstrIdCliente)) && n.i_Eliminado == 0  &&
                                         n.v_IdVenta.Contains("ZQ")
                                     select new ReporteAdelantoFactura
                                     {
                                         NroDocIdentidad = J2.v_NroDocIdentificacion,
                                         ApellidosNombres = (J2.v_ApePaterno + " " + J2.v_ApeMaterno + " " + J2.v_PrimerNombre + " " +
                                                            J2.v_RazonSocial).Trim(),
                                         EsAdelanto = false,
                                         Fecha = J3.t_FechaRegistro ?? DateTime.Now,
                                         IdCliente = J1.v_IdCliente,
                                         ImporteAdelanto = 0,
                                         ImporteFactura = n.d_ImporteSoles ?? 0,
                                         Moneda = J3.i_IdMoneda == 1 ? "S" : "D",
                                         TipoCambio = J3.d_TipoCambio ?? 0,
                                         TipoDocumentoVenta = J4.v_Siglas,
                                         NroDocumentoVenta = J1.v_SerieDocumento + "-" + J1.v_CorrelativoDocumento,
                                         TipoDocumento = "ANT",
                                         NroDocumento = n.v_DocumentoRef
                                     }).ToList();

                    var result = adelantos.Concat(cobranzas).OrderBy(o => o.NroDocumento).ToList();
                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public class ReporteAdelantoFactura
        {
            public string IdCliente { get; set; }
            public string NroDocIdentidad { get; set; }
            public string ApellidosNombres { get; set; }
            public string TipoDocumento { get; set; }
            public string NroDocumento { get; set; }
            public DateTime Fecha { get; set; }
            public decimal ImporteAdelanto { get; set; }
            public decimal ImporteFactura { get; set; }
            public bool EsAdelanto { get; set; }
            public string Moneda { get; set; }
            public decimal TipoCambio { get; set; }
            public string TipoDocumentoVenta { get; set; }
            public string NroDocumentoVenta { get; set; }
        }
    }
}
