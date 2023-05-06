using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using SAMBHS.CommonWIN.BL;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;
using System.ComponentModel;
using System.Runtime.InteropServices.ComTypes;
using SAMBHS.Common.BL;
using System.Transactions;


namespace SAMBHS.Tesoreria.BL
{
    public class TesoreriaBL
    {

        private string periodo = Globals.ClientSession.i_Periodo.Value.ToString();
        public List<KeyValueDTO> ObtenerListadoTesorerias(ref OperationResult pobjOperationResult, string pstringPeriodo,
            string pstringMes, int IdTipoDocumento)
        {
            try
            {
                var almacenpredeterminado = Globals.ClientSession.i_IdAlmacenPredeterminado.Value.ToString("00");
                var replicationID = Globals.ClientSession.ReplicationNodeID;
                SAMBHSEntitiesModelWin dbcontext = new SAMBHSEntitiesModelWin();
                var query = (from n in dbcontext.tesoreria
                             where
                                 n.i_Eliminado == 0 && n.v_Periodo == pstringPeriodo && n.v_Mes == pstringMes &&
                                 n.i_IdTipoDocumento == IdTipoDocumento &&
                                 n.v_IdTesoreria.Substring(2, 2) == almacenpredeterminado
                                 && n.v_IdTesoreria.Substring(0, 1) == replicationID
                             orderby n.v_Correlativo ascending
                             select new
                             {
                                 v_Correlativo = n.v_Correlativo,
                                 v_IdTesoreria = n.v_IdTesoreria
                             }
                    );
                if (query.Any())
                {
                    var query2 = query.AsEnumerable()
                        .Select(x => new KeyValueDTO
                        {
                            Value1 = x.v_Correlativo,
                            Value2 = x.v_IdTesoreria
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

        public tesoreriaDto ObtenerTesoreriaCabecera(ref OperationResult pobjOperationResult, string pstrIdTesoreria)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                var objEntity = (from a in dbContext.tesoreria
                                 where a.v_IdTesoreria == pstrIdTesoreria
                                 select a).FirstOrDefault();

                pobjOperationResult.Success = 1;
                return objEntity.ToDTO();
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public BindingList<tesoreriadetalleDto> ObtenerTesoreriaDetalles(ref OperationResult pobjOperationResult,
            string pstrIdTesoreria)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = (from n in dbContext.tesoreriadetalle

                             join J1 in dbContext.cliente on n.v_IdCliente equals J1.v_IdCliente into J1_join
                             from J1 in J1_join.DefaultIfEmpty()

                             where n.i_Eliminado == 0 && n.v_IdTesoreria == pstrIdTesoreria

                             select new tesoreriadetalleDto
                             {
                                 i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                 CodAnexo = J1.v_CodCliente,
                                 i_Eliminado = n.i_Eliminado,
                                 i_EsDestino = n.i_EsDestino,
                                 i_IdCaja = n.i_IdCaja,
                                 i_IdCentroCostos = n.i_IdCentroCostos,
                                 v_Naturaleza = n.v_Naturaleza,
                                 v_Analisis = n.v_Analisis,
                                 v_IdCliente = n.v_IdCliente,
                                 v_IdTesoreria = n.v_IdTesoreria,
                                 v_IdTesoreriaDetalle = n.v_IdTesoreriaDetalle,
                                 v_NroCuenta = n.v_NroCuenta,
                                 v_NroDocumento = n.v_NroDocumento,
                                 v_NroDocumentoRef = n.v_NroDocumentoRef,
                                 v_OrigenDestino = n.v_OrigenDestino,
                                 v_Pedido = n.v_Pedido,
                                 i_IdTipoDocumento = n.i_IdTipoDocumento,
                                 i_IdTipoDocumentoRef = n.i_IdTipoDocumentoRef,
                                 i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                 t_Fecha = n.t_Fecha,
                                 t_ActualizaFecha = n.t_Fecha,
                                 t_InsertaFecha = n.t_InsertaFecha,
                                 d_Cambio = n.d_Cambio,
                                 d_Importe = n.d_Importe,
                                 i_IdPatrimonioNeto =n.i_IdPatrimonioNeto ,
                             }
                    ).ToList();

                pobjOperationResult.Success = 1;

                return new BindingList<tesoreriadetalleDto>(query);
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public string Insertartesoreria(ref OperationResult pobjOperationResult, tesoreriaDto pobjDtoEntity,
            List<string> ClientSession, List<tesoreriadetalleDto> pTemp_Insertar)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                    SecuentialBL objSecuentialBL = new SecuentialBL();
                    tesoreria objEntitytesoreria = pobjDtoEntity.ToEntity();

                    int SecuentialId = 0;
                    string newIdtesoreria = string.Empty;
                    string newIdtesoreriaDetalle = string.Empty;

                    #region Inserta Cabecera

                    objEntitytesoreria.t_InsertaFecha = DateTime.Now;
                    objEntitytesoreria.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntitytesoreria.i_Eliminado = 0;

                    var intNodeId = int.Parse(ClientSession[0]);
                    SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 55);
                    newIdtesoreria = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "XE");
                    pobjDtoEntity.v_IdTesoreria = newIdtesoreria;
                    objEntitytesoreria.v_IdTesoreria = newIdtesoreria;
                    dbContext.AddTotesoreria(objEntitytesoreria);
                    dbContext.SaveChanges();

                    #endregion

                    #region Inserta los destinos al detalle si es necesario
                    var destinosAnteriores =
                               dbContext.tesoreriadetalle.Where(p => p.v_IdTesoreria.Equals(newIdtesoreria) && p.i_EsDestino.Equals("1"));

                    foreach (var destino in destinosAnteriores)
                    {
                        dbContext.DeleteObject(destino);
                    }

                    var destinos = dbContext.destino.Where(p => p.i_Eliminado == 0).ToList();
                    pTemp_Insertar =
                        pTemp_Insertar.Concat(ProcesaDestinosTesoreria(ref pobjOperationResult, pTemp_Insertar,
                            objEntitytesoreria.ToDTO(), destinos)).ToList();
                    if (pobjOperationResult.Success == 0) return null;

                    #endregion

                    #region Inserta Detalle Y Actualiza Pendientes Por Cobrar

                    foreach (var tesoreriadetalleDto in pTemp_Insertar)
                    {
                        var objEntitytesoreriaDetalle = tesoreriadetalleDto.ToEntity();
                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 56);
                        newIdtesoreriaDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "XF");
                        objEntitytesoreriaDetalle.v_IdTesoreriaDetalle = newIdtesoreriaDetalle;
                        objEntitytesoreriaDetalle.v_IdCliente = string.IsNullOrWhiteSpace(objEntitytesoreriaDetalle.v_IdCliente) ? null : objEntitytesoreriaDetalle.v_IdCliente.Trim();
                        objEntitytesoreriaDetalle.v_IdTesoreria = newIdtesoreria;
                        objEntitytesoreriaDetalle.t_InsertaFecha = DateTime.Now;
                        objEntitytesoreriaDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntitytesoreriaDetalle.i_Eliminado = 0;

                        dbContext.AddTotesoreriadetalle(objEntitytesoreriaDetalle);
                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "tesoreriadetalle", newIdtesoreriaDetalle);

                        #region Actualiza el estado de la retencion a procesada

                        if (!string.IsNullOrWhiteSpace(tesoreriadetalleDto.v_IdDocumentoRetencionDetalle))
                        {
                            var ret = dbContext.documentoretenciondetalle
                                .FirstOrDefault(
                                    p =>
                                        p.v_IdDocumentoRetencionDetalle.Equals(
                                            tesoreriadetalleDto.v_IdDocumentoRetencionDetalle));

                            if (ret != null)
                            {
                                if (ret.i_Procesado == 1)
                                    throw new Exception("Esta retención ya fue procesada anteriormente." +
                                                        tesoreriadetalleDto.v_IdDocumentoRetencionDetalle);

                                ret.i_Procesado = 1;
                                dbContext.documentoretenciondetalle.ApplyCurrentValues(ret);
                            }
                        }

                        #endregion

                        if ((pobjDtoEntity.i_IdEstado ?? 0) != 1) continue;
                        # region Genera Movimiento Estado Bancario
                        if (objEntitytesoreriaDetalle.v_NroCuenta.StartsWith("104"))
                        {
                            InsertarMovimientoEstadoBancarioXTesoreria(ref pobjOperationResult, pobjDtoEntity,
                                ClientSession, objEntitytesoreriaDetalle);
                            if (pobjOperationResult.Success == 0) return null;
                        }
                        #endregion
                    }
                    dbContext.SaveChanges();

                    #endregion

                    pobjOperationResult.Success = 1;
                    Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "tesoreria",
                        newIdtesoreria);
                    ts.Complete();
                    return newIdtesoreria;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "TesoreriaBL.Insertartesoreria()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                return null;
            }
        }

        public void InsertarMovimientoEstadoBancarioXTesoreria(ref OperationResult objOperationResult,
            tesoreriaDto objTesoreria, List<string> ClienteSession, tesoreriadetalle objTesoreriaDetalle)
        {
            try
            {
                objOperationResult.Success = 1;
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var MonedaCuenta = (from n in dbContext.asientocontable
                                        where n.v_NroCuenta == objTesoreriaDetalle.v_NroCuenta && n.v_Periodo == periodo && n.i_Eliminado == 0
                                        select new { n.i_IdMoneda }).FirstOrDefault();

                    var CodigoAsiento = (from n in dbContext.documento

                                         where n.i_Eliminado == 0 && n.i_CodigoDocumento == objTesoreria.i_IdTipoDocumento

                                         select new
                                         {
                                             siglas = n.v_Siglas
                                         }).FirstOrDefault();

                    List<movimientoestadobancarioDto> ListaMovimientoEstadoBancario =
                        new List<movimientoestadobancarioDto>();
                    string AnioTesoreria = objTesoreria.t_FechaRegistro.Value.Year.ToString();
                    var MovEstadoBancarioAnio = dbContext.movimientoestadobancario.Where(o => o.v_Anio == AnioTesoreria && o.i_Eliminado == 0).ToList();
                    var MaxMesEstadoBancario = MovEstadoBancarioAnio.Any() ? int.Parse(dbContext.movimientoestadobancario.Where(o => o.v_Anio == AnioTesoreria && o.i_Eliminado == 0).Max(o => o.v_Mes).ToString()) : 0;
                    List<movimientoestadobancario> MovEstadoBancario = dbContext.movimientoestadobancario.ToList();

                    int MesEjecutarTesoreria = int.Parse(objTesoreria.t_FechaRegistro.Value.Month.ToString("00"));
                    while (MesEjecutarTesoreria <= MaxMesEstadoBancario)
                    {
                        var ExisteTesoreria = MovEstadoBancario.ToList().Where(o => o.i_Eliminado == 0 && int.Parse(o.v_Mes) == MesEjecutarTesoreria && o.v_Anio == AnioTesoreria && o.v_IdReferencia == objTesoreriaDetalle.v_IdTesoreriaDetalle).FirstOrDefault();
                        if (ExisteTesoreria == null)
                        {
                            movimientoestadobancarioDto objMovimientoEB = new movimientoestadobancarioDto();
                            ListaMovimientoEstadoBancario = new List<movimientoestadobancarioDto>();
                            objMovimientoEB.t_Fecha = objTesoreria.t_FechaRegistro.Value;
                            objMovimientoEB.v_Concepto = objTesoreriaDetalle.v_Analisis == null ||
                                                         objTesoreriaDetalle.v_Analisis == string.Empty
                                ? objTesoreria.v_Glosa
                                : objTesoreriaDetalle.v_Analisis;
                            objMovimientoEB.d_Cargo = objTesoreriaDetalle.v_Naturaleza == "H" &&
                                                      MonedaCuenta.i_IdMoneda == 1
                                ? objTesoreriaDetalle.d_Importe
                                : objTesoreriaDetalle.v_Naturaleza == "H" && MonedaCuenta.i_IdMoneda == 2
                                    ? objTesoreriaDetalle.d_Importe
                                    : 0;
                            objMovimientoEB.d_Abono = objTesoreriaDetalle.v_Naturaleza == "D" &&
                                                      MonedaCuenta.i_IdMoneda == 1
                                ? objTesoreriaDetalle.d_Importe
                                : objTesoreriaDetalle.v_Naturaleza == "D" && MonedaCuenta.i_IdMoneda == 2
                                    ? objTesoreriaDetalle.d_Importe
                                    : 0;
                            objMovimientoEB.i_IdTipoDocumento = objTesoreriaDetalle.i_IdTipoDocumento;
                            objMovimientoEB.v_NumeroDocumento = objTesoreriaDetalle.v_NroDocumento.Trim();
                            objMovimientoEB.v_CodAsiento = CodigoAsiento.siglas.Trim();
                            objMovimientoEB.v_NumeroAsiento = objTesoreria.v_Mes + "-" + objTesoreria.v_Correlativo;
                            objMovimientoEB.i_Mes = 0;
                            objMovimientoEB.v_Anio = objTesoreria.t_FechaRegistro.Value.Date.Year.ToString();

                            objMovimientoEB.v_Mes = MesEjecutarTesoreria.ToString("00");
                            objMovimientoEB.v_NroCuenta = objTesoreriaDetalle.v_NroCuenta.Trim();
                            objMovimientoEB.v_IdReferencia = objTesoreriaDetalle.v_IdTesoreriaDetalle;
                            objMovimientoEB.i_IdTipoDocRef = objTesoreriaDetalle.i_IdTipoDocumentoRef == null
                                ? 0
                                : objTesoreriaDetalle.i_IdTipoDocumentoRef.Value;
                            objMovimientoEB.v_NroDocumentoRef = objTesoreriaDetalle.v_NroDocumentoRef;
                            ListaMovimientoEstadoBancario.Add(objMovimientoEB);
                            new MovimientoEstadoBancarioBL().InsertarMovimientoEstadoBancario(ref objOperationResult,
                                ClienteSession, ListaMovimientoEstadoBancario);
                        }

                        MesEjecutarTesoreria = MesEjecutarTesoreria + 1;
                    }



                }
            }

            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation =
                    "TesoreriaBL.InsertarMovimientoEstadoBancarioXTesoreria()\nLinea:" +
                    ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                return;
            }
        }

        public void Actualizartesoreria(ref OperationResult pobjOperationResult, tesoreriaDto pobjDtoEntity,
            List<string> ClientSession, List<tesoreriadetalleDto> pTemp_Insertar, List<tesoreriadetalleDto> pTemp_Editar,
            List<tesoreriadetalleDto> pTemp_Eliminar)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    SecuentialBL objSecuentialBL = new SecuentialBL();
                    tesoreria objEntitytesoreria = tesoreriaAssembler.ToEntity(pobjDtoEntity);
                    SaldoContableBL objSaldoContableBL = new SaldoContableBL();
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                    string newIdtesoreriaDetalle = string.Empty;

                    #region Actualiza Cabecera

                    var intNodeId = int.Parse(ClientSession[0]);
                    var objEntitySource = (from a in dbContext.tesoreria
                                           where a.v_IdTesoreria == pobjDtoEntity.v_IdTesoreria
                                           select a).FirstOrDefault();

                    pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                    pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                    tesoreria objEntity = pobjDtoEntity.ToEntity();
                    dbContext.tesoreria.ApplyCurrentValues(objEntity);

                    #endregion

                    #region Procesa los Destinos del diario
                    var destinosAnteriores =
                              dbContext.tesoreriadetalle.Where(p => p.v_IdTesoreria.Equals(objEntitySource.v_IdTesoreria) && p.i_EsDestino.Equals("1"));

                    foreach (var destino in destinosAnteriores)
                    {
                        dbContext.DeleteObject(destino);
                    }

                    var destinos = dbContext.destino.Where(p => p.i_Eliminado == 0).ToList();
                    pTemp_Insertar =
                        pTemp_Insertar.Concat(ProcesaDestinosTesoreria(ref pobjOperationResult,
                            pTemp_Insertar.Concat(pTemp_Editar).ToList(), objEntitySource.ToDTO(), destinos)).ToList();
                    if (pobjOperationResult.Success == 0) return;

                    #endregion

                    #region Actualiza Detalle

                    foreach (tesoreriadetalleDto tesoreriadetalleDto in pTemp_Insertar)
                    {
                        tesoreriadetalle objEntitytesoreriaDetalle = tesoreriadetalleDto.ToEntity();
                        var SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 56);
                        newIdtesoreriaDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "XF");
                        objEntitytesoreriaDetalle.v_IdTesoreriaDetalle = newIdtesoreriaDetalle;
                        objEntitytesoreriaDetalle.v_IdTesoreria = objEntitySource.v_IdTesoreria;
                        objEntitytesoreriaDetalle.t_InsertaFecha = DateTime.Now;
                        objEntitytesoreriaDetalle.v_IdCliente = string.IsNullOrWhiteSpace(objEntitytesoreriaDetalle.v_IdCliente) ? null : objEntitytesoreriaDetalle.v_IdCliente.Trim();
                        objEntitytesoreriaDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntitytesoreriaDetalle.i_Eliminado = 0;
                        dbContext.AddTotesoreriadetalle(objEntitytesoreriaDetalle);
                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName,
                              "tesoreriadetalle", objEntitytesoreriaDetalle.v_IdTesoreriaDetalle);

                        #region Actualiza el estado de la retencion a procesada

                        if (!string.IsNullOrWhiteSpace(tesoreriadetalleDto.v_IdDocumentoRetencionDetalle))
                        {
                            var ret = dbContext.documentoretenciondetalle
                                .FirstOrDefault(p => p.v_IdDocumentoRetencionDetalle.Equals(tesoreriadetalleDto.v_IdDocumentoRetencionDetalle));

                            if (ret != null)
                            {
                                if (ret.i_Procesado == 1)
                                    throw new Exception("Esta retención ya fue procesada anteriormente." +
                                                        tesoreriadetalleDto.v_IdDocumentoRetencionDetalle);

                                ret.i_Procesado = 1;
                                dbContext.documentoretenciondetalle.ApplyCurrentValues(ret);
                            }
                        }

                        #endregion

                        if ((pobjDtoEntity.i_IdEstado ?? 0) != 1) continue;
                        # region Genera Movimiento Estado Bancario

                        if (objEntitytesoreriaDetalle.v_NroCuenta.StartsWith("104"))
                        {
                            if (pobjDtoEntity.i_IdEstado.Value == 1)
                            {
                                InsertarMovimientoEstadoBancarioXTesoreria(ref pobjOperationResult, pobjDtoEntity,
                                    ClientSession, objEntitytesoreriaDetalle);
                                if (pobjOperationResult.Success == 0) return;
                            }
                        }

                        #endregion

                    }

                    foreach (tesoreriadetalleDto tesoreriadetalleDto in pTemp_Editar)
                    {
                        if (tesoreriadetalleDto.i_EsDestino != "1")
                        {
                            tesoreriadetalle _objEntity = tesoreriadetalleDto.ToEntity();
                            RestablecePendientePorCobrarDetalle(ref pobjOperationResult, _objEntity,
                                _objEntity.i_IdTipoDocumento.Value, _objEntity.v_NroDocumento,
                                objEntitytesoreria.i_TipoMovimiento.Value, _objEntity.v_IdCliente);
                            if (pobjOperationResult.Success == 0) return;

                            var query = (dbContext.tesoreriadetalle.Where(
                                n => n.v_IdTesoreriaDetalle == tesoreriadetalleDto.v_IdTesoreriaDetalle)).FirstOrDefault();

                            _objEntity.t_ActualizaFecha = DateTime.Now;
                            _objEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            dbContext.tesoreriadetalle.ApplyCurrentValues(_objEntity);
                            Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName,
                                "tesoreriadetalle", query.v_IdTesoreriaDetalle);


                            # region Actualiza Movimiento Estado Bancario

                            if (pobjDtoEntity.i_IdEstado.Value == 1) //&& tesoreriadetalleDto.v_NroCuenta.StartsWith("104"
                            {
                                ActualizaMovimientoEstadoBancario(ref pobjOperationResult, pobjDtoEntity, ClientSession,
                                    tesoreriadetalleDto);
                                if (pobjOperationResult.Success == 0) return;
                            }
                            else if (pobjDtoEntity.i_IdEstado == 0)// && tesoreriadetalleDto.v_NroCuenta.StartsWith("104"))
                            {
                                new MovimientoEstadoBancarioBL().EliminarMovimientoEstadoBancarioXReferencia(
                                    ref pobjOperationResult, tesoreriadetalleDto, ClientSession);

                                if (pobjOperationResult.Success == 0) return;
                            }

                            #endregion


                        }
                    }

                    foreach (tesoreriadetalleDto tesoreriadetalleDto in pTemp_Eliminar)
                    {
                        if (tesoreriadetalleDto.i_EsDestino != "1")
                        {
                            tesoreriadetalle _objEntity = tesoreriadetalleDto.ToEntity();
                            var query = (from n in dbContext.tesoreriadetalle
                                         where n.v_IdTesoreriaDetalle == tesoreriadetalleDto.v_IdTesoreriaDetalle
                                         select n).FirstOrDefault();

                            if (query != null && query.EntityState != EntityState.Deleted)
                            {
                                query.t_ActualizaFecha = DateTime.Now;
                                query.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                                query.i_Eliminado = 1;

                                #region Actualiza el estado de la retencion a procesada

                                if (!string.IsNullOrWhiteSpace(tesoreriadetalleDto.v_IdDocumentoRetencionDetalle))
                                {
                                    var ret = dbContext.documentoretenciondetalle
                                        .FirstOrDefault(
                                            p =>
                                                p.v_IdDocumentoRetencionDetalle.Equals(
                                                    tesoreriadetalleDto.v_IdDocumentoRetencionDetalle));

                                    if (ret != null)
                                    {
                                        if (ret.i_Procesado == 0)
                                            throw new Exception("Esta retención ya fue procesada anteriormente." +
                                                                tesoreriadetalleDto.v_IdDocumentoRetencionDetalle);

                                        ret.i_Procesado = 0;
                                        dbContext.documentoretenciondetalle.ApplyCurrentValues(ret);
                                    }
                                }

                                #endregion

                                if ((pobjDtoEntity.i_IdEstado ?? 0) != 1) continue;

                                #region Eliminar Mov. Estado Bancario
                                new MovimientoEstadoBancarioBL().EliminarMovimientoEstadoBancarioXReferencia(
                                    ref pobjOperationResult, tesoreriadetalleAssembler.ToDTO(query), ClientSession);
                                if (pobjOperationResult.Success == 0) return;
                                #endregion

                                dbContext.tesoreriadetalle.ApplyCurrentValues(query);
                                Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName,
                                    "tesoreriadetalle", query.v_IdTesoreriaDetalle);


                            }
                        }
                    }

                    #endregion

                    dbContext.SaveChanges();
                    Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName,
                        "tesoreria", pobjDtoEntity.v_IdTesoreria);
                    pobjOperationResult.Success = 1;
                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "TesoreriaBL.Actualizartesoreria()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }

        public void ActualizaMovimientoEstadoBancario(ref OperationResult objOperationResult, tesoreriaDto objTesoreria,
            List<string> ClienteSession, tesoreriadetalleDto objTesoreriaDetalleDto)
        {


            try
            {
                objOperationResult.Success = 1;
                movimientoestadobancario objEntityMovimientoEstadoBancario = new movimientoestadobancario();
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var MovEstadoBancario = (from n in dbContext.movimientoestadobancario

                                             where n.i_Eliminado == 0 && n.v_IdReferencia == objTesoreriaDetalleDto.v_IdTesoreriaDetalle

                                             select n).OrderBy(o => o.v_Mes).ToList();

                    var MonedaCuenta = (from n in dbContext.asientocontable
                                        where n.v_NroCuenta == objTesoreriaDetalleDto.v_NroCuenta && n.v_Periodo == periodo
                                        select new { n.i_IdMoneda }).FirstOrDefault();

                    var CodigoAsiento = (from n in dbContext.documento

                                         where n.i_Eliminado == 0 && n.i_CodigoDocumento == objTesoreria.i_IdTipoDocumento

                                         select new
                                         {
                                             siglas = n.v_Siglas
                                         }).FirstOrDefault();

                    foreach (var mov in MovEstadoBancario)
                    {
                        EliminaMovEstadoBancarioXDocRef(ref objOperationResult, mov.v_IdReferencia, ClienteSession);

                    }

                    if (objTesoreriaDetalleDto.v_NroCuenta.StartsWith("104"))
                    {

                        new TesoreriaBL().InsertarMovimientoEstadoBancarioXTesoreria(ref objOperationResult, objTesoreria,
                                          ClienteSession, objTesoreriaDetalleDto.ToEntity());
                    }


                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "TesoreriaBL.ActualizaMovimientoEstadoBancario()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return;



            }
        }

        public void Eliminartesoreria(ref OperationResult pobjOperationResult, string pstrIdtesoreria,
            List<string> ClientSession)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                    #region Elimina Cabecera

                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.tesoreria
                                           where a.v_IdTesoreria == pstrIdtesoreria
                                           select a).FirstOrDefault();

                    // Crear la entidad con los datos actualizados
                    objEntitySource.t_ActualizaFecha = DateTime.Now;
                    objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntitySource.i_Eliminado = 1;

                    #endregion

                    #region Elimina Detalles

                    //Eliminar detalles del movimiento eliminado.
                    var objEntitySourceDetallestesoreria = (from a in dbContext.tesoreriadetalle
                                                            where a.v_IdTesoreria == pstrIdtesoreria
                                                            select a).ToList();
                    foreach (var RegistrotesoreriaDetalle in objEntitySourceDetallestesoreria)
                    {
                        RegistrotesoreriaDetalle.t_ActualizaFecha = DateTime.Now;
                        RegistrotesoreriaDetalle.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        RegistrotesoreriaDetalle.i_Eliminado = 1;

                        #region Actualiza el estado de la retencion a procesada

                        if (!string.IsNullOrWhiteSpace(RegistrotesoreriaDetalle.v_IdDocumentoRetencionDetalle))
                        {
                            var ret = dbContext.documentoretenciondetalle
                                .FirstOrDefault(
                                    p =>
                                        p.v_IdDocumentoRetencionDetalle.Equals(
                                            RegistrotesoreriaDetalle.v_IdDocumentoRetencionDetalle));

                            if (ret != null)
                            {
                                if (ret.i_Procesado == 0)
                                    throw new Exception("Esta retención ya fue procesada anteriormente." +
                                                        RegistrotesoreriaDetalle.v_IdDocumentoRetencionDetalle);

                                ret.i_Procesado = 0;
                                dbContext.documentoretenciondetalle.ApplyCurrentValues(ret);
                            }
                        }

                        #endregion

                        #region Elimina Movimiento Estado Bancario

                        if (RegistrotesoreriaDetalle.v_NroCuenta.StartsWith("104"))
                        {
                            EliminaMovEstadoBancarioXDocRef(ref pobjOperationResult, RegistrotesoreriaDetalle.v_IdTesoreriaDetalle, ClientSession);
                            if (pobjOperationResult.Success == 0) return;
                        }

                        #endregion

                        RestablecePendientePorCobrarDetalle(ref pobjOperationResult, RegistrotesoreriaDetalle,
                            RegistrotesoreriaDetalle.i_IdTipoDocumento.Value, RegistrotesoreriaDetalle.v_NroDocumento,
                            objEntitySource.i_TipoMovimiento.Value, RegistrotesoreriaDetalle.v_IdCliente);
                        new SaldoContableBL().ActualizarSaldoContable(ref pobjOperationResult, ListaProcesos.Tesoreria,
                            objEntitySource.t_FechaRegistro.Value, objEntitySource.i_IdMoneda.Value,
                            tesoreriadetalleAssembler.ToDTO(RegistrotesoreriaDetalle), RecordStatus.EliminadoLogico,
                            RegistrotesoreriaDetalle.v_IdTesoreriaDetalle.Substring(0, 1));
                        if (pobjOperationResult.Success == 0)
                        {
                            return;
                        }
                        Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName,
                            "tesoreriadetalle", RegistrotesoreriaDetalle.v_IdTesoreriaDetalle);
                    }

                    #endregion

                    dbContext.SaveChanges();
                    Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName,
                        "tesoreria", pstrIdtesoreria);
                    pobjOperationResult.Success = 1;
                    ts.Complete();
                    return;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "TesoreriaBL.Eliminartesoreria()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException.Message;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
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
                pobjOperationResult.AdditionalInformation = "TesoreriaBL.DevolverTipoCambioPorFecha()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public List<string[]> DevolverNombresRelaciones(string pstrIdTesoreria)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var Result = (from n in dbContext.tesoreriadetalle

                                  join J1 in dbContext.cliente on n.v_IdCliente equals J1.v_IdCliente into J1_join
                                  from J1 in J1_join.DefaultIfEmpty()

                                  where n.v_IdTesoreria == pstrIdTesoreria && n.i_Eliminado == 0

                                  select new
                                  {
                                      Codigo = J1.v_CodCliente
                                  }
                        ).ToList();

                    List<string[]> LCodigos = new List<string[]>();

                    foreach (var Fila in Result)
                    {
                        string[] Cadena = new string[1];
                        Cadena[0] = Fila.Codigo;
                        LCodigos.Add(Cadena);
                    }

                    return LCodigos;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DocumentoData DevuelveCuentaCajaBanco(ref OperationResult pobjOperationResult, int pintIdDocumento)
        {
            try
            {
                var dbContext = new SAMBHSEntitiesModelWin();
                var datos = (from a in dbContext.documento

                             join J1 in dbContext.asientocontable on new { c = a.v_NroCuenta.Trim(), per = periodo } equals new { c = J1.v_NroCuenta.Trim(), per = periodo } into J1_join
                             from J1 in J1_join.DefaultIfEmpty()

                             where a.i_CodigoDocumento == pintIdDocumento
                             select new { a.v_NroCuenta, J1.v_NombreCuenta , a.i_BancoDetraccion }).FirstOrDefault();

                var cadena = new DocumentoData();

                if (datos != null && datos.v_NombreCuenta != null)
                {
                    cadena.NroCuenta = datos.v_NroCuenta.Trim();
                    cadena.NombreCuenta = datos.v_NombreCuenta.Trim();
                    cadena.BancoDetraccion = (datos.i_BancoDetraccion ?? 0) == 1;

                    pobjOperationResult.Success = 1;
                    return cadena;
                }
                throw new Exception("No se encontró una cuenta contable existente enlazada a este documento.");
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "TesoreriaBL.DevuelveCuentaCajaBanco()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return new DocumentoData();
            }
        }

        public string DevuelveNombreCuenta(string pstrCuenta)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var Nombre = (from c in dbContext.asientocontable
                              where c.v_NroCuenta == pstrCuenta && c.v_Periodo == periodo
                              select new { c.v_NombreCuenta }).FirstOrDefault();

                return Nombre != null ? Nombre.v_NombreCuenta.Trim() : string.Empty;
            }
        }

        public bool ExisteNroRegistro(int TipoDoc, string Periodo, string Mes, string Correlativo)
        {
            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
            var replicationID = Globals.ClientSession.ReplicationNodeID;
            var Registro = (from n in dbContext.tesoreria
                            where
                                n.i_Eliminado == 0 && n.v_Periodo == Periodo && n.v_Mes == Mes && n.v_Correlativo == Correlativo &&
                                n.i_IdTipoDocumento == TipoDoc
                                && n.v_IdTesoreria.Substring(0, 1) == replicationID
                            select n).FirstOrDefault();

            if (Registro == null)
            {
                return true;
            }
            return false;
        }

        public string[] EliminarTesoreriaXDocRef(ref OperationResult pobjOperationResult, string pstrIdDocRef,
            List<string> ClientSession)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                    SaldoContableBL objSaldoContableBL = new SaldoContableBL();
                    string[] IdRegistroEliminado = new string[3];

                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.tesoreria
                                           where a.v_IdCobranza == pstrIdDocRef && a.i_Eliminado == 0
                                           select a).FirstOrDefault();

                    if (objEntitySource != null)
                    {
                        IdRegistroEliminado[0] = objEntitySource.v_Periodo;
                        IdRegistroEliminado[1] = objEntitySource.v_Mes;
                        IdRegistroEliminado[2] = objEntitySource.v_Correlativo;

                        // Crear la entidad con los datos actualizados
                        objEntitySource.t_ActualizaFecha = DateTime.Now;
                        objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntitySource.i_Eliminado = 1;

                        #region Elimina Detalles

                        //Eliminar detalles del movimiento eliminado.
                        var objEntitySourceDetallestesoreria = (from a in dbContext.tesoreriadetalle
                                                                where a.v_IdTesoreria == objEntitySource.v_IdTesoreria && a.i_Eliminado == 0
                                                                select a).ToList();

                        foreach (var RegistrotesoreriaDetalle in objEntitySourceDetallestesoreria)
                        {
                            RegistrotesoreriaDetalle.t_ActualizaFecha = DateTime.Now;
                            RegistrotesoreriaDetalle.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            RegistrotesoreriaDetalle.i_Eliminado = 1;
                            RestablecePendientePorCobrarDetalle(ref pobjOperationResult, RegistrotesoreriaDetalle,
                                RegistrotesoreriaDetalle.i_IdTipoDocumento.Value,
                                RegistrotesoreriaDetalle.v_NroDocumento, objEntitySource.i_TipoMovimiento.Value,
                                RegistrotesoreriaDetalle.v_IdCliente);
                            if (pobjOperationResult.Success == 0) return null;

                            #region Elimina Movimiento Estado Bancario

                            if (RegistrotesoreriaDetalle.v_NroCuenta.StartsWith("104"))
                            {
                                EliminaMovEstadoBancarioXDocRef(ref pobjOperationResult, RegistrotesoreriaDetalle.v_IdTesoreriaDetalle, ClientSession);
                            }

                            #endregion

                            objSaldoContableBL.ActualizarSaldoContable(ref pobjOperationResult, ListaProcesos.Tesoreria,
                                objEntitySource.t_FechaRegistro.Value, objEntitySource.i_IdMoneda.Value,
                                tesoreriadetalleAssembler.ToDTO(RegistrotesoreriaDetalle), RecordStatus.EliminadoLogico,
                                RegistrotesoreriaDetalle.v_IdTesoreriaDetalle.Substring(0, 1));
                        }

                        #endregion
                    }


                    dbContext.SaveChanges();

                    pobjOperationResult.Success = 1;
                    ts.Complete();
                    return IdRegistroEliminado;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "TesoreriaBL.EliminarTesoreriaXDocRef()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                return null;
            }
        }

        private void EliminaMovEstadoBancarioXDocRef(ref OperationResult objOperationResult,
            string objTesoreriaDetalle, List<string> ClientSession)
        {
            try
            {
                objOperationResult.Success = 1;
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    var MovDetalles = (from n in dbContext.movimientoestadobancario

                                       where n.v_IdReferencia == objTesoreriaDetalle && n.i_Eliminado == 0

                                       select n).ToList();


                    if (MovDetalles.Count() != 0)
                    {

                        foreach (var estadobancario in MovDetalles)
                        {
                            //dbContext.movimientoestadobancario.DeleteObject(estadobancario);
                            estadobancario.i_Eliminado = 1;
                            estadobancario.t_ActualizaFecha = DateTime.Now;
                            estadobancario.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            dbContext.movimientoestadobancario.ApplyCurrentValues(estadobancario);
                            Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName,
                           "movimientoestadobancario", estadobancario.v_IdMovimientoEstadoBancario);
                        }
                        dbContext.SaveChanges();
                    }

                }

            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "TesoreriaBL.EliminaMovEstadoBancarioXDocRef()";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return;


            }

        }

        private void EliminarMovimientoEstadoBancarioXReferencia(ref OperationResult objOperationResult,
            tesoreriadetalleDto objTesoreriaDetalle, List<string> ClientSession)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var EstadoBancarioEliminar = (from n in dbContext.movimientoestadobancario
                                                  where n.v_IdReferencia == objTesoreriaDetalle.v_IdTesoreriaDetalle && n.i_Eliminado == 0
                                                  select n).ToList();

                    if (EstadoBancarioEliminar.Count() != 0)
                    {

                        foreach (var estadobancario in EstadoBancarioEliminar)
                        {
                            //dbContext.movimientoestadobancario.DeleteObject(estadobancario);

                            estadobancario.i_Eliminado = 1;
                            estadobancario.t_ActualizaFecha = DateTime.Now;
                            estadobancario.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            dbContext.movimientoestadobancario.ApplyCurrentValues(estadobancario);
                            Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName,
                           "movimientoestadobancario", estadobancario.v_IdMovimientoEstadoBancario);
                        }
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "TesoreriaBL.EliminarMovimientoEstadoBancarioXReferencia()";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return;
            }

        }

        public void ActualizaPendientePorCobrarDetalle(ref OperationResult pobjOperationResult,
            tesoreriaDto pobjDtoEntity, tesoreriadetalle objEntitytesoreriaDetalle, List<string> ClientSession,
            int TipoMovimiento, string RUCProveedor)
        {
            var idTesoreria = pobjDtoEntity.v_IdTesoreria;
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var objSecuentialBl = new SecuentialBL();

                        var pendienteCabecera = (from p in dbContext.pendientecobrar
                                                 where
                                                     p.i_IdTipoDocumento == objEntitytesoreriaDetalle.i_IdTipoDocumento &&
                                                     p.v_NroCuenta == objEntitytesoreriaDetalle.v_NroCuenta &&
                                                     p.v_NroDocumento == objEntitytesoreriaDetalle.v_NroDocumento.Trim() &&
                                                     p.i_FlagTipoMovimiento == TipoMovimiento && p.v_IdCliente.Equals(objEntitytesoreriaDetalle.v_IdCliente)
                                                 select p).FirstOrDefault();

                        if (pendienteCabecera != null) // Si es nulo se regulará con un proceso aparte.
                        {
                            var pendienteCobrarDetalle =
                                pendienteCabecera.pendientecobrardetalle.FirstOrDefault(
                                    p => objEntitytesoreriaDetalle.i_IdTipoDocumento.HasValue &&
                                        p.i_IdTipoDocumento.Equals(objEntitytesoreriaDetalle.i_IdTipoDocumento.Value) &&
                                        p.v_NroCuenta.Equals(objEntitytesoreriaDetalle.v_NroCuenta) &&
                                        p.v_NroDocumento.Equals(objEntitytesoreriaDetalle.v_NroDocumento.Trim()) &&
                                        p.v_IdCliente.Equals(objEntitytesoreriaDetalle.v_IdCliente) &&
                                        p.v_Naturaleza.Equals(objEntitytesoreriaDetalle.v_Naturaleza));

                            if (pendienteCobrarDetalle == null)
                            {
                                #region Inserta Detalle
                                var RUC = "";
                                if (TipoMovimiento == (int)TipoMovimientoTesoreria.Egreso)
                                {
                                    RUC = (from p in dbContext.cliente
                                           where p.v_IdCliente == pendienteCabecera.v_IdCliente
                                           select new { p.v_NroDocIdentificacion }).First().v_NroDocIdentificacion;
                                }

                                pendientecobrardetalle objEntityPendienteDetalle = new pendientecobrardetalle();
                                var intNodeId = int.Parse(ClientSession[0]);
                                var SecuentialId = objSecuentialBl.GetNextSecuentialId(intNodeId, 62);
                                var newIdtesoreriaDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "XL");
                                objEntityPendienteDetalle.v_IdPendienteCobrarDetalle = newIdtesoreriaDetalle;
                                objEntityPendienteDetalle.v_IdTesoreria = objEntitytesoreriaDetalle.v_IdTesoreria;
                                objEntityPendienteDetalle.v_IdPendienteCobrar = pendienteCabecera.v_IdPendienteCobrar;
                                objEntityPendienteDetalle.v_Naturaleza = objEntitytesoreriaDetalle.v_Naturaleza;
                                objEntityPendienteDetalle.v_NroCuenta = objEntitytesoreriaDetalle.v_NroCuenta;
                                objEntityPendienteDetalle.v_NroDocumento = pobjDtoEntity.v_Mes.Trim() + "-" +
                                                                           pobjDtoEntity.v_Correlativo.Trim();
                                objEntityPendienteDetalle.d_ImporteSaldo = pobjDtoEntity.i_IdMoneda == 1
                                    ? DevuelveValorRedondeado(objEntitytesoreriaDetalle.d_Importe ?? 0)
                                    : DevuelveValorRedondeado(objEntitytesoreriaDetalle.d_Cambio ?? 0 *
                                                              pobjDtoEntity.d_TipoCambio ?? 0);
                                objEntityPendienteDetalle.d_ImporteSaldoDolares = pobjDtoEntity.i_IdMoneda == 2
                                    ? DevuelveValorRedondeado(objEntitytesoreriaDetalle.d_Importe ?? 0)
                                    : DevuelveValorRedondeado(objEntitytesoreriaDetalle.d_Cambio ?? 0 /
                                                              pobjDtoEntity.d_TipoCambio ?? 0);
                                objEntityPendienteDetalle.t_InsertaFecha = DateTime.Now;
                                objEntityPendienteDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                                objEntityPendienteDetalle.t_FechaRegistro = pobjDtoEntity.t_FechaRegistro;
                                objEntityPendienteDetalle.v_IdCliente = pendienteCabecera.v_IdCliente;
                                objEntityPendienteDetalle.i_IdMoneda = pobjDtoEntity.i_IdMoneda;
                                objEntityPendienteDetalle.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento;
                                objEntityPendienteDetalle.v_NroRucProveedor = RUC;
                                dbContext.AddTopendientecobrardetalle(objEntityPendienteDetalle);
                                #endregion

                                #region Calculo de saldos
                                decimal saldoCabeceraSoles, saldoCabeceraDolares;
                                if (TipoMovimiento == (int)TipoMovimientoTesoreria.Egreso)
                                {
                                    saldoCabeceraSoles =
                                    (pendienteCabecera.pendientecobrardetalle.Where(p => p.v_Naturaleza.Equals("H"))
                                        .Sum(o => o.d_ImporteSaldo ?? 0)) -
                                    (pendienteCabecera.pendientecobrardetalle.Where(p => p.v_Naturaleza.Equals("D"))
                                        .Sum(o => o.d_ImporteSaldo ?? 0));
                                    saldoCabeceraDolares =
                                        (pendienteCabecera.pendientecobrardetalle.Where(p => p.v_Naturaleza.Equals("H"))
                                            .Sum(o => o.d_ImporteSaldoDolares ?? 0)) -
                                        (pendienteCabecera.pendientecobrardetalle.Where(p => p.v_Naturaleza.Equals("D"))
                                            .Sum(o => o.d_ImporteSaldoDolares ?? 0));
                                }
                                else
                                {
                                    saldoCabeceraSoles =
                                (pendienteCabecera.pendientecobrardetalle.Where(p => p.v_Naturaleza.Equals("D"))
                                    .Sum(o => o.d_ImporteSaldo ?? 0)) -
                                (pendienteCabecera.pendientecobrardetalle.Where(p => p.v_Naturaleza.Equals("H"))
                                    .Sum(o => o.d_ImporteSaldo ?? 0));
                                    saldoCabeceraDolares =
                                        (pendienteCabecera.pendientecobrardetalle.Where(p => p.v_Naturaleza.Equals("D"))
                                            .Sum(o => o.d_ImporteSaldoDolares ?? 0)) -
                                        (pendienteCabecera.pendientecobrardetalle.Where(p => p.v_Naturaleza.Equals("H"))
                                            .Sum(o => o.d_ImporteSaldoDolares ?? 0));
                                }
                                #endregion

                                pendienteCabecera.d_ImporteSaldo = saldoCabeceraSoles;
                                pendienteCabecera.d_ImporteSaldoDolares = saldoCabeceraDolares;
                                dbContext.pendientecobrar.ApplyCurrentValues(pendienteCabecera);
                                dbContext.SaveChanges();
                            }
                        }
                        else
                        {
                            if (pobjDtoEntity != null && objEntitytesoreriaDetalle != null)
                            {
                                new PendientesBL().InsertarPendientePorTesoreria(ref pobjOperationResult,
                                    pobjDtoEntity.v_IdTesoreria, objEntitytesoreriaDetalle, ClientSession,
                                    TipoMovimiento);
                                if (pobjOperationResult.Success == 0)
                                    return;
                            }
                        }
                        pobjOperationResult.Success = 1;
                        ts.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "TesoreriaBL.ActualizaPendientePorCobrarDetalle()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) + '\n' + idTesoreria;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }

        public decimal DevuelveValorRedondeado(decimal Valor)
        {
            return decimal.Parse(Math.Round(Valor, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
        }

        private void RestablecePendientePorCobrarDetalle(ref OperationResult pobjOperationResult,
            tesoreriadetalle Detalle, int IdTipoDoc, string NroDoc, int TipoMovimiento, string IdCliente)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        tesoreria Tesoreria = (from a in dbContext.tesoreria
                                               where a.v_IdTesoreria == Detalle.v_IdTesoreria
                                               select a).FirstOrDefault();

                        Detalle =
                            dbContext.tesoreriadetalle
                                .FirstOrDefault(p => p.v_IdTesoreriaDetalle == Detalle.v_IdTesoreriaDetalle);

                        if (TipoMovimiento == (int)TipoMovimientoTesoreria.Ingreso)
                        {
                            if (Detalle.v_NroCuenta.Trim().Substring(0, 2) == "12" &&
                                Detalle.i_IdTipoDocumento == IdTipoDoc && Detalle.v_NroDocumento == NroDoc)
                            {
                                pendientecobrar PendienteCabecera = (from p in dbContext.pendientecobrar
                                                                     where p.v_NroDocumento == Detalle.v_NroDocumento && p.v_IdCliente == IdCliente
                                                                     select p).FirstOrDefault();

                                if (PendienteCabecera != null)
                                {
                                    //Restablece el importe del pendiente
                                    PendienteCabecera.d_ImporteSaldo = Tesoreria.i_IdMoneda == 1
                                        ? DevuelveValorRedondeado(PendienteCabecera.d_ImporteSaldo.Value +
                                                                  Detalle.d_Importe.Value)
                                        : DevuelveValorRedondeado((PendienteCabecera.d_ImporteSaldo.Value +
                                                                   (Detalle.d_Importe.Value * Tesoreria.d_TipoCambio.Value)));
                                    PendienteCabecera.d_ImporteSaldoDolares = Tesoreria.i_IdMoneda == 2
                                        ? DevuelveValorRedondeado(PendienteCabecera.d_ImporteSaldoDolares.Value +
                                                                  Detalle.d_Importe.Value)
                                        : DevuelveValorRedondeado((PendienteCabecera.d_ImporteSaldoDolares.Value +
                                                                   (Detalle.d_Importe.Value / Tesoreria.d_TipoCambio.Value)));

                                    //Elimina Fisicamente los detalles de tesoreria del Pendiente.
                                    PendienteCabecera.pendientecobrardetalle.ToList()
                                        .Where(o => o.v_IdTesoreria == Tesoreria.v_IdTesoreria)
                                        .ToList()
                                        .ForEach(p => dbContext.pendientecobrardetalle.DeleteObject(p));

                                    dbContext.pendientecobrar.ApplyCurrentValues(PendienteCabecera);
                                }

                            }
                        }
                        else
                        {
                            if (Detalle.v_NroCuenta.Trim().Substring(0, 2) == "42" &&
                                Detalle.i_IdTipoDocumento == IdTipoDoc && Detalle.v_NroDocumento == NroDoc &&
                                Detalle.v_IdCliente == IdCliente)
                            {
                                pendientecobrar PendienteCabecera = (from p in dbContext.pendientecobrar
                                                                     where p.v_NroDocumento == Detalle.v_NroDocumento && p.v_IdCliente == IdCliente
                                                                     select p).FirstOrDefault();

                                if (PendienteCabecera != null)
                                {
                                    //Restablece el importe del pendiente
                                    PendienteCabecera.d_ImporteSaldo = Tesoreria.i_IdMoneda == 1
                                        ? DevuelveValorRedondeado(PendienteCabecera.d_ImporteSaldo.Value +
                                                                  Detalle.d_Importe.Value)
                                        : DevuelveValorRedondeado((PendienteCabecera.d_ImporteSaldo.Value +
                                                                   (Detalle.d_Importe.Value * Tesoreria.d_TipoCambio.Value)));
                                    PendienteCabecera.d_ImporteSaldoDolares = Tesoreria.i_IdMoneda == 2
                                        ? DevuelveValorRedondeado(PendienteCabecera.d_ImporteSaldoDolares.Value +
                                                                  Detalle.d_Importe.Value)
                                        : DevuelveValorRedondeado((PendienteCabecera.d_ImporteSaldoDolares.Value +
                                                                   (Detalle.d_Importe.Value / Tesoreria.d_TipoCambio.Value)));

                                    //Elimina Fisicamente los detalles de tesoreria del Pendiente.
                                    PendienteCabecera.pendientecobrardetalle.ToList()
                                        .Where(o => o.v_IdTesoreria == Tesoreria.v_IdTesoreria)
                                        .ToList()
                                        .ForEach(p => dbContext.pendientecobrardetalle.DeleteObject(p));

                                    dbContext.pendientecobrar.ApplyCurrentValues(PendienteCabecera);
                                }
                            }
                        }

                        dbContext.SaveChanges();
                    }

                    pobjOperationResult.Success = 1;
                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation =
                    "TesoreriaBL.RestablecePendientePorCobrarDetalle()\nLinea:" +
                    ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException.Message;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }

        }

        public int DevuelveCorrelativoCheque(ref OperationResult pobjOperationResult, string pstrNroCuenta)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    if (!string.IsNullOrEmpty(pstrNroCuenta))
                    {
                        var registros = dbContext.tesoreriadetalle.Where(n => n.v_NroCuenta == pstrNroCuenta && n.i_IdTipoDocumento == 309
                                                                            && n.i_Eliminado == 0).ToList();
                        if (registros.Count != 0)
                        {
                            pobjOperationResult.Success = 1;
                            return registros.Select(p =>
                            {
                                int i;
                                return int.TryParse(p.v_NroDocumento, out i) ? i : 0;
                            }).Max() + 1;
                        }
                        pobjOperationResult.Success = 1;
                        return 1;
                    }
                    pobjOperationResult.Success = 1;
                    return 0;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "TesoreriaBL.DevuelveCorrelativoCheque()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return 0;
            }
        }

        public bool ExisteNroCheque(string pstrNroCheque, string pstrNroCuenta, string vIdTesoreria)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    if (!pstrNroCuenta.StartsWith("104")) return false;

                    var resultado = dbContext.tesoreriadetalle.Where(n => n.i_IdTipoDocumento == 309
                            && n.v_NroCuenta == pstrNroCuenta && n.i_Eliminado == 0
                            && (string.IsNullOrEmpty(vIdTesoreria) || n.v_IdTesoreria != vIdTesoreria)).ToList();

                    var listado = resultado.Select(n => n.v_NroDocumento).OrderBy(n => n).ToList();
                    return resultado.Any() && listado.Any(r => int.Parse(r) == int.Parse(pstrNroCheque));
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string ObtenerCobranzaPagoOrigen(ref OperationResult pobjOperationResult, string pstrIdTesoreria)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var tesoreria = dbContext.tesoreria.FirstOrDefault(p => p.v_IdTesoreria == pstrIdTesoreria);
                    if (tesoreria != null)
                    {
                        if (!string.IsNullOrEmpty(tesoreria.v_IdCobranza))
                        {
                            string docOrigen = string.Empty;
                            if (tesoreria.v_IdCobranza.Contains("ZA"))
                            {
                                var origen =
                                    dbContext.cobranza.FirstOrDefault(p => p.v_IdCobranza == tesoreria.v_IdCobranza);
                                if (origen != null)
                                {
                                    var siglasDoc =
                                        dbContext.documento.FirstOrDefault(
                                            p => p.i_CodigoDocumento == origen.i_IdTipoDocumento.Value).v_Siglas;
                                    docOrigen = siglasDoc + "-" + origen.v_Periodo + "-" + origen.v_Mes.Trim() + "-" +
                                                origen.v_Correlativo;
                                }

                                return string.Format("Documento de Origen: Cobranza: {0}", docOrigen);
                            }
                            else if (tesoreria.v_IdCobranza.Contains("LY"))
                            {

                                var origen = dbContext.cajachica.FirstOrDefault(p => p.v_IdCajaChica == tesoreria.v_IdCobranza);
                                if (origen != null)
                                {
                                    var siglasDoc =
                                        dbContext.documento.FirstOrDefault(
                                            p => p.i_CodigoDocumento == origen.i_IdTipoDocumento.Value).v_Siglas;
                                    docOrigen = siglasDoc + "-" + origen.v_Periodo + "-" + origen.v_Mes.Trim() + "-" +
                                                origen.v_Correlativo;
                                    return string.Format("Documento de Origen: Caja Chica: {0}", docOrigen);
                                }



                            }
                            else
                            {
                                var origen = dbContext.pago.FirstOrDefault(p => p.v_IdPago == tesoreria.v_IdCobranza);
                                if (origen != null)
                                {
                                    var siglasDoc =
                                        dbContext.documento.FirstOrDefault(
                                            p => p.i_CodigoDocumento == origen.i_IdTipoDocumento.Value).v_Siglas;
                                    docOrigen = siglasDoc + "-" + origen.v_Periodo + "-" + origen.v_Mes.Trim() + "-" +
                                                origen.v_Correlativo;
                                    return string.Format("Documento de Origen: Pago: {0}", docOrigen);
                                }
                            }


                        }
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "TesoreriaBL.DevuelveCorrelativoCheque()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return string.Empty;
            }
        }

        public static bool CorrelativoYaExiste(ref OperationResult pobjOperationResult, string periodo, string pstrMes, string pstrCorrelativo, int pintTipoDocumento)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var result = dbContext.tesoreria.Any(p => p.v_Periodo == periodo && p.i_IdTipoDocumento == pintTipoDocumento && p.v_Mes.Trim() == pstrMes.Trim() && p.v_Correlativo.Trim() == pstrCorrelativo.Trim() && p.i_Eliminado == 0);
                    pobjOperationResult.Success = 1;
                    return result;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "TesoreriaBL.CorrelativoYaExiste()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return false;
            }
        }

        #region Bandeja
        public List<tesoreriaDto> ListarTesorerias(ref OperationResult pobjOperationResult, string _strFilterExpression, DateTime F_Ini, DateTime F_Fin)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var tesorerias = (from d in dbContext.tesoreria

                                      join J1 in dbContext.systemuser on new { id = d.i_InsertaIdUsuario.Value }
                                                                       equals new { id = J1.i_SystemUserId } into J1_join
                                      from J1 in J1_join.DefaultIfEmpty()

                                      join J2 in dbContext.systemuser on new { id = d.i_ActualizaIdUsuario.Value }
                                                                       equals new { id = J2.i_SystemUserId } into J2_join
                                      from J2 in J2_join.DefaultIfEmpty()

                                      join J3 in dbContext.datahierarchy on new { i_IdTipoDocumento = d.i_IdTipoDocumento.Value, b = 56 }
                                                                    equals new { i_IdTipoDocumento = J3.i_ItemId, b = J3.i_GroupId } into J3_join
                                      from J3 in J3_join.DefaultIfEmpty()

                                      join J4 in dbContext.documento on new { i_IdTipoDocumento = d.i_IdTipoDocumento.Value }
                                                                    equals new { i_IdTipoDocumento = J4.i_CodigoDocumento } into J4_join
                                      from J4 in J4_join.DefaultIfEmpty()


                                      where d.i_Eliminado == 0 && d.t_FechaRegistro >= F_Ini && d.t_FechaRegistro <= F_Fin

                                      select new tesoreriaDto
                                      {
                                          v_Periodo = d.v_Periodo,
                                          v_Mes = d.v_Mes,
                                          v_Correlativo = d.v_Correlativo,
                                          v_IdTesoreria = d.v_IdTesoreria,
                                          i_IdTipoDocumento = d.i_IdTipoDocumento,
                                          NroRegistro = d.v_Mes.Trim() + " - " + d.v_Correlativo,
                                          v_UsuarioCreacion = J1.v_UserName,
                                          v_UsuarioModificacion = J2.v_UserName,
                                          v_Nombre = d.v_Nombre,
                                          t_ActualizaFecha = d.t_ActualizaFecha,
                                          t_InsertaFecha = d.t_InsertaFecha,
                                          TipoComprobante = J3.v_Value1,
                                          TipoDocumento = J4.v_Siglas,
                                          v_IdCobranza = d.v_IdCobranza,
                                          t_FechaRegistro = d.t_FechaRegistro,
                                          ImporteSoles = d.i_IdMoneda == (int)Currency.Soles ? d.d_TotalDebe_Importe.Value : d.i_IdMoneda == (int)Currency.Dolares ? d.d_TotalDebe_Cambio.Value : 0,
                                          ImporteDolares = d.i_IdMoneda == (int)Currency.Dolares ? d.d_TotalDebe_Importe.Value : d.i_IdMoneda == (int)Currency.Soles ? d.d_TotalDebe_Cambio.Value : 0,
                                          Moneda = d.i_IdMoneda == 1 ? "S" : "D",
                                      });

                    if (!string.IsNullOrEmpty(_strFilterExpression))
                    {
                        tesorerias = tesorerias.Where(_strFilterExpression);
                    }

                    pobjOperationResult.Success = 1;
                    return tesorerias.ToList();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }
        #endregion

        #region AMC

        public List<tesoreriaDto> DevolverDataJerarquizadaTesoreria_TesoreriaDetalle(ref OperationResult pobjOperationResult, DateTime FechaIni, DateTime FechaFin)
        {
            try
            {
                SAMBHSEntitiesModelWin dbcontext = new SAMBHSEntitiesModelWin();
                tesoreriaDto objtesoreriaDto;
                List<tesoreriaDto> ListatesoreriaDto = new List<tesoreriaDto>();
                tesoreriadetalleDto objtesoreriadetalleDto;
                List<tesoreriadetalleDto> ListaTesoreriaDetalleDto;




                var Query1 = (from a in dbcontext.tesoreria
                              where a.t_FechaRegistro > FechaIni && a.t_FechaRegistro < FechaFin
                              select a).ToList();

                var Query2 = (from a in dbcontext.tesoreriadetalle
                              where a.t_Fecha > FechaIni && a.t_Fecha < FechaFin
                              select a).ToList();

                //Armar mi clase Jerárquica
                foreach (var item in Query1)
                {
                    //Cabecera
                    objtesoreriaDto = new tesoreriaDto();
                    objtesoreriaDto.v_IdTesoreria = item.v_IdTesoreria;
                    objtesoreriaDto.v_Periodo = item.v_Periodo;
                    objtesoreriaDto.i_IdTipoDocumento = item.i_IdTipoDocumento;
                    objtesoreriaDto.v_Mes = item.v_Mes;
                    objtesoreriaDto.v_NroCuentaCajaBanco = item.v_NroCuentaCajaBanco;
                    objtesoreriaDto.v_Correlativo = item.v_Correlativo;
                    objtesoreriaDto.i_TipoMovimiento = item.i_TipoMovimiento;
                    objtesoreriaDto.t_FechaRegistro = item.t_FechaRegistro;
                    objtesoreriaDto.d_TipoCambio = item.d_TipoCambio;
                    objtesoreriaDto.i_IdMedioPago = item.i_IdMedioPago;
                    objtesoreriaDto.v_Nombre = item.v_Nombre;
                    objtesoreriaDto.v_Glosa = item.v_Glosa;
                    objtesoreriaDto.i_IdMoneda = item.i_IdMoneda;
                    objtesoreriaDto.i_IdEstado = item.i_IdEstado;
                    objtesoreriaDto.d_TotalDebe_Importe = item.d_TotalDebe_Importe;
                    objtesoreriaDto.d_TotalDebe_Cambio = item.d_TotalDebe_Cambio;
                    objtesoreriaDto.d_TotalHaber_Importe = item.d_TotalHaber_Importe;
                    objtesoreriaDto.d_TotalHaber_Cambio = item.d_TotalHaber_Cambio;
                    objtesoreriaDto.d_Diferencia_Importe = item.d_Diferencia_Importe;
                    objtesoreriaDto.d_Diferencia_Cambio = item.d_Diferencia_Cambio;

                    //Detalle
                    ListaTesoreriaDetalleDto = new List<tesoreriadetalleDto>();
                    var TesoreriaDetalle = Query2.FindAll(p => p.v_IdTesoreria == objtesoreriaDto.v_IdTesoreria);

                    foreach (var item1 in TesoreriaDetalle)
                    {

                        objtesoreriadetalleDto = new tesoreriadetalleDto();
                        objtesoreriadetalleDto.v_IdTesoreriaDetalle = item1.v_IdTesoreriaDetalle;
                        objtesoreriadetalleDto.v_IdTesoreria = item1.v_IdTesoreria;
                        objtesoreriadetalleDto.v_NroCuenta = item1.v_NroCuenta;
                        objtesoreriadetalleDto.v_Naturaleza = item1.v_Naturaleza;
                        objtesoreriadetalleDto.d_Importe = item1.d_Importe;
                        objtesoreriadetalleDto.d_Cambio = item1.d_Cambio;
                        objtesoreriadetalleDto.i_IdCentroCostos = item1.i_IdCentroCostos;
                        objtesoreriadetalleDto.i_IdCaja = item1.i_IdCaja;
                        objtesoreriadetalleDto.v_IdCliente = item1.v_IdCliente;
                        objtesoreriadetalleDto.i_IdTipoDocumento = item1.i_IdTipoDocumento;
                        objtesoreriadetalleDto.v_NroDocumento = item1.v_NroDocumento;
                        objtesoreriadetalleDto.i_IdTipoDocumentoRef = item1.i_IdTipoDocumentoRef;
                        objtesoreriadetalleDto.v_NroDocumentoRef = item1.v_NroDocumentoRef;
                        objtesoreriadetalleDto.v_Analisis = item1.v_Analisis;
                        objtesoreriadetalleDto.t_Fecha = item1.t_Fecha;
                        objtesoreriadetalleDto.v_OrigenDestino = item1.v_OrigenDestino;
                        objtesoreriadetalleDto.v_Pedido = item1.v_Pedido;

                        ListaTesoreriaDetalleDto.Add(objtesoreriadetalleDto);
                    }

                    objtesoreriaDto.TesoreriaDetallePersonalizado = ListaTesoreriaDetalleDto;
                    ListatesoreriaDto.Add(objtesoreriaDto);
                }

                return ListatesoreriaDto;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<tesoreriadetalleDto> DevolverTesoreriaDetalle(ref OperationResult pobjOperationResult, DateTime FechaIni, DateTime FechaFin)
        {
            try
            {
                SAMBHSEntitiesModelWin dbcontext = new SAMBHSEntitiesModelWin();
                var Query = (from a in dbcontext.tesoreriadetalle
                             join b in dbcontext.tesoreria on a.v_IdTesoreria equals b.v_IdTesoreria
                             where (a.i_Eliminado == 0 && b.i_Eliminado == 0)
                             && (b.t_FechaRegistro > FechaIni && b.t_FechaRegistro < FechaFin)
                             && (a.v_Naturaleza == "H")
                             select new tesoreriadetalleDto
                             {
                                 v_NroCuenta = a.v_NroCuenta,
                                 v_Naturaleza = a.v_Naturaleza,
                                 d_Importe = a.d_Importe,
                                 i_IdTipoDocumento = a.i_IdTipoDocumento,
                                 v_NroDocumento = a.v_NroDocumento,
                                 t_Fecha = a.t_Fecha,
                                 i_IdMoneda = b.i_IdMoneda.Value,
                                 v_IdTesoreria = b.v_IdTesoreria,
                                 v_IdCliente = a.v_IdCliente

                             }

                                ).ToList();


                return Query;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region Reportes


        public decimal ObtenerSaldoMensualBanco(ref OperationResult pobjOperationResult, DateTime FechaInicio, DateTime FechaFin, string Cuenta1, int MonedaReporte, string Orden, int TipoDocDetalle, string NroDocDetalle, bool UsadoProcesoSM)
        {
            try
            {

                var SaldoFinal = ReporteLibroBancoSinRango(ref pobjOperationResult, FechaInicio, FechaFin, Cuenta1, MonedaReporte, Orden, TipoDocDetalle, NroDocDetalle, UsadoProcesoSM).LastOrDefault();

                return SaldoFinal == null ? 0 : SaldoFinal.SaldoFinal;
            }
            catch (Exception ex)
            {

                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "TesoreriaBL.ObtenerSaldoMensualBanco()\nLinea:" + Cuenta1 +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                return 0;
            }

            //return SaldoBanco;
        }
        public List<ReporteLibroBanco> ReporteLibroBancoSinRango(ref OperationResult objOperationResult, DateTime FechaInicio, DateTime FechaFin, string Cuenta1, int MonedaReporte, string Orden, int TipoDocDetalle, string NroDocDetalle, bool UsadoProcesoSaldoMensual)
        {

            try
            {

                objOperationResult.Success = 1;

                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                string SaldoMes, SaldoAnio;
                SaldoMes = int.Parse(FechaInicio.Month.ToString()) == (int)Mes.Enero ? ((int)Mes.Diciembre).ToString() : SaldoMes = (int.Parse(FechaInicio.Month.ToString()) - 1).ToString("00");
                SaldoAnio = int.Parse(FechaInicio.Month.ToString()) == (int)Mes.Enero ? (int.Parse(FechaInicio.Year.ToString()) - 1).ToString() : FechaInicio.Year.ToString();
                Orden = "NumeroCuenta," + Orden;
                DateTime FechaInicioAnterior = DateTime.Parse("01/01/" + FechaFin.Year.ToString());
                DateTime FechaFinAnterior = UsadoProcesoSaldoMensual ? DateTime.Parse("31/12/" + FechaFin.Year.ToString() + " 23:59") : DateTime.Parse(FechaInicio.AddDays(-1).ToShortDateString() + " 23:59");
                //var SaldoMensualBancos = dbContext.saldomensualbancos.Where(k => k.i_Eliminado == 0).ToList();
                var SaldoMensualBancos = int.Parse(FechaInicio.Month.ToString()) == (int)Mes.Enero ? 0 : ObtenerSaldoMensualBanco(ref objOperationResult, FechaInicioAnterior, FechaFinAnterior, Cuenta1, MonedaReporte, Orden, TipoDocDetalle, NroDocDetalle, UsadoProcesoSaldoMensual);

                if (objOperationResult.Success == 0)
                {
                    return null;

                }
                var ReporteLibroBancoInicialTesorerias = (from a in dbContext.tesoreria

                                                          join b in dbContext.tesoreriadetalle on new { idTesoreria = a.v_IdTesoreria, eliminado = 0 } equals new { idTesoreria = b.v_IdTesoreria, eliminado = b.i_Eliminado.Value } into b_join

                                                          from b in b_join.DefaultIfEmpty()


                                                          join c in dbContext.documento on new { TipoDoc = a.i_IdTipoDocumento.Value, eliminado = 0 }
                                                                                      equals new { TipoDoc = c.i_CodigoDocumento, eliminado = c.i_Eliminado.Value } into c_join

                                                          from c in c_join.DefaultIfEmpty()

                                                          join d in dbContext.documento on new { TipoDoc1 = b.i_IdTipoDocumento.Value, eliminado = 0 }
                                                                                      equals new { TipoDoc1 = d.i_CodigoDocumento, eliminado = d.i_Eliminado.Value } into d_join

                                                          from d in d_join.DefaultIfEmpty()

                                                          join e in dbContext.asientocontable on new { a = b.v_NroCuenta, eliminado = 0, p = periodo }
                                                                                                   equals new { a = e.v_NroCuenta, eliminado = e.i_Eliminado.Value, p = e.v_Periodo } into e_join

                                                          from e in e_join.DefaultIfEmpty()



                                                          where a.i_Eliminado == 0 && b.v_NroCuenta == Cuenta1 &&

                                                          a.t_FechaRegistro >= FechaInicio && a.t_FechaRegistro <= FechaFin


                                                          && (b.i_IdTipoDocumento == TipoDocDetalle || TipoDocDetalle == -1) && (b.v_NroDocumento.Contains(NroDocDetalle) || NroDocDetalle == "")



                                                          select new ReporteLibroBanco
                                                          {

                                                              Moneda = a.i_IdMoneda == 1 ? "S/." : "US$.",
                                                              NumeroCuenta = b.v_NroCuenta,
                                                              NombreCuenta = e == null ? "*No Existe*" : e.v_NombreCuenta,
                                                              Comprobante = c == null ? a.v_Mes.Trim() + "-" + a.v_Correlativo.Trim() : c.v_Siglas.Trim() + " " + a.v_Mes.Trim() + "-" + a.v_Correlativo.Trim(),
                                                              Documento = d == null ? b.v_NroDocumento.Trim() : d.v_Siglas.Trim() + " " + b.v_NroDocumento.Trim(),
                                                              iDia = a.t_FechaRegistro.Value.Day,
                                                              Detalle = a.i_IdEstado == 0 ? "A N U L A D O" : string.IsNullOrEmpty(b.v_Analisis) ? a.v_Glosa : b.v_Analisis,   //  a.v_Glosa,
                                                              Debe = a.i_IdEstado == 0 ? 0 : b == null ? 0 : b.v_Naturaleza == "D" && MonedaReporte == a.i_IdMoneda ? b.d_Importe.Value : b.v_Naturaleza == "D" && MonedaReporte != a.i_IdMoneda ? b.d_Cambio.Value : 0,
                                                              Haber = a.i_IdEstado == 0 ? 0 : b == null ? 0 : b.v_Naturaleza == "H" && MonedaReporte == a.i_IdMoneda ? b.d_Importe.Value : b.v_Naturaleza == "H" && MonedaReporte != a.i_IdMoneda ? b.d_Cambio.Value : 0,
                                                              v_IdTesoreria = a.v_IdTesoreria,
                                                              v_IdTesoreriaDetalle = b == null ? "0" : b.v_IdTesoreriaDetalle,
                                                              Fecha = a.t_FechaRegistro,
                                                          }).AsQueryable();






                var ReporteLibroBancoOrdenado = ReporteLibroBancoInicialTesorerias.OrderBy(Orden).ToList();
                List<ReporteLibroBanco> ReporteLibroBancoFinal = new List<ReporteLibroBanco>();
                int i = -1; decimal saldoanterior = 0; string cuenta = string.Empty;
                foreach (var Fila in ReporteLibroBancoOrdenado)
                {
                    //var SaldoAnterior  =  int.Parse(FechaInicio.Month.ToString()) == (int)Mes.Enero? dbContext.saldomensualbancos .Where (o=>o.i_Eliminado ==0 && o.v_Mes == SaldoMes && o.v_NroCuenta == Fila.NumeroCuenta && o.v_Anio == SaldoAnio).FirstOrDefault () : SaldoMensualBancos; //SaldoMensualBancos.Where(l => l.v_Mes == SaldoMes && l.v_NroCuenta == Fila.NumeroCuenta && l.v_Anio == SaldoAnio).FirstOrDefault();
                    if (MonedaReporte == 1)
                    {
                        if (int.Parse(FechaInicio.Month.ToString()) == (int)Mes.Enero)
                        {
                            var SaldoDiciembreSoles = dbContext.saldomensualbancos.Where(o => o.i_Eliminado == 0 && o.v_Mes == SaldoMes && o.v_NroCuenta == Fila.NumeroCuenta && o.v_Anio == SaldoAnio).FirstOrDefault();

                            Fila.SaldoSegunBanco = SaldoDiciembreSoles != null ? Fila.SaldoSegunBanco = SaldoDiciembreSoles.d_SaldoSoles : 0;


                        }
                        else
                        {
                            Fila.SaldoSegunBanco = SaldoMensualBancos;
                        }
                    }
                    else
                    {
                        if (int.Parse(FechaInicio.Month.ToString()) == (int)Mes.Enero)
                        {
                            var SaldoDiciembreDolares = dbContext.saldomensualbancos.Where(o => o.i_Eliminado == 0 && o.v_Mes == SaldoMes && o.v_NroCuenta == Fila.NumeroCuenta && o.v_Anio == SaldoAnio).FirstOrDefault();
                            Fila.SaldoSegunBanco = SaldoDiciembreDolares != null ? SaldoDiciembreDolares.d_SaldoDolares : 0;

                        }
                        else
                        {
                            Fila.SaldoSegunBanco = SaldoMensualBancos;
                        }

                    }

                    if (Fila.NumeroCuenta == cuenta || cuenta == string.Empty)
                    {
                        i = i + 1;

                    }
                    else
                    {
                        i = 0;
                        saldoanterior = 0;

                    }

                    Fila.Dia = Fila.iDia.ToString("00");
                    Fila.Saldo = i == 0 ? Fila.SaldoSegunBanco + Fila.Debe - Fila.Haber : saldoanterior + Fila.Debe - Fila.Haber;
                    saldoanterior = Fila.Saldo.Value;
                    Fila.SaldoFinal = saldoanterior;
                    ReporteLibroBancoFinal.Add(Fila);
                    cuenta = Fila.NumeroCuenta;


                }

                if (!ReporteLibroBancoOrdenado.Any())
                {

                    ReporteLibroBanco Fila = new ReporteLibroBanco();

                    //var saldos = SaldoMensualBancos.Where(x => x.v_NroCuenta == Cuenta1 && x.v_Mes == SaldoMes && x.v_Anio == SaldoAnio).ToList();
                    //Fila.SaldoSegunBanco = saldos.Any() ? MonedaReporte == 1 ? saldos.Sum(a => a.d_SaldoSoles).Value : saldos.Sum(a => a.d_SaldoDolares).Value : 0;
                    Fila.NumeroCuenta = Cuenta1;
                    if (MonedaReporte == 1)
                    {

                        if (int.Parse(FechaInicio.Month.ToString()) == (int)Mes.Enero)
                        {
                            var SaldoDiciembreSoles = dbContext.saldomensualbancos.Where(o => o.i_Eliminado == 0 && o.v_Mes == SaldoMes && o.v_NroCuenta == Fila.NumeroCuenta && o.v_Anio == SaldoAnio).FirstOrDefault();
                            Fila.SaldoSegunBanco = SaldoDiciembreSoles != null ? SaldoDiciembreSoles.d_SaldoSoles : 0;

                        }
                        else
                        {
                            Fila.SaldoSegunBanco = SaldoMensualBancos;
                        }
                        // Fila.SaldoSegunBanco = int.Parse(FechaInicio.Month.ToString()) == (int)Mes.Enero ? dbContext.saldomensualbancos.Where(o => o.i_Eliminado == 0 && o.v_Mes == SaldoMes && o.v_NroCuenta == Fila.NumeroCuenta && o.v_Anio == SaldoAnio).FirstOrDefault().d_SaldoSoles.Value : SaldoMensualBancos;
                    }
                    else
                    {
                        if (int.Parse(FechaInicio.Month.ToString()) == (int)Mes.Enero)
                        {
                            var SaldoDiciembreDolares = dbContext.saldomensualbancos.Where(o => o.i_Eliminado == 0 && o.v_Mes == SaldoMes && o.v_NroCuenta == Fila.NumeroCuenta && o.v_Anio == SaldoAnio).FirstOrDefault();
                            Fila.SaldoSegunBanco = SaldoDiciembreDolares != null ? SaldoDiciembreDolares.d_SaldoDolares : 0;
                        }
                        else
                        {
                            Fila.SaldoSegunBanco = SaldoMensualBancos; //int.Parse(FechaInicio.Month.ToString()) == (int)Mes.Enero ? dbContext.saldomensualbancos.Where(o => o.i_Eliminado == 0 && o.v_Mes == SaldoMes && o.v_NroCuenta == Fila.NumeroCuenta && o.v_Anio == SaldoAnio).FirstOrDefault().d_SaldoDolares.Value : SaldoMensualBancos;
                        }
                    }

                    saldoanterior = Fila.SaldoSegunBanco.Value;
                    Fila.Dia = Fila.iDia.ToString("00");

                    Fila.NombreCuenta = ObtenerNombreCuenta(Fila.NumeroCuenta);
                    Fila.SaldoFinal = saldoanterior;
                    ReporteLibroBancoFinal.Add(Fila);
                    cuenta = Fila.NumeroCuenta;

                }
                return ReporteLibroBancoFinal.ToList();
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "SaldoMensualBancosBL.ReporteLibroBancoSinRango()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                return null;
            }

        }
        public List<ReporteLibroBanco> ReporteLibroBancoRango(ref OperationResult objOperationResult, DateTime FechaInicio, DateTime FechaFin, int Cuenta1, int Cuenta2, int MonedaReporte, string Orden, int TipoDocDetalle, string NumeroDoc, bool UsadoProcesoSaldoMensual)
        {

            try
            {
                objOperationResult.Success = 1;
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                string SaldoMes, SaldoAnio;
                SaldoMes = int.Parse(FechaInicio.Month.ToString()) == (int)Mes.Enero ? ((int)Mes.Diciembre).ToString("00") : SaldoMes = (int.Parse(FechaInicio.Month.ToString()) - 1).ToString("00");
                SaldoAnio = int.Parse(FechaInicio.Month.ToString()) == (int)Mes.Enero ? (int.Parse(FechaInicio.Year.ToString()) - 1).ToString() : FechaInicio.Year.ToString();

                Orden = "NumeroCuentaInt," + Orden;
                DateTime FechaInicioAnterior = DateTime.Parse("01/01/" + FechaFin.Year.ToString());
                DateTime FechaFinAnterior = UsadoProcesoSaldoMensual ? DateTime.Parse("31/12/" + FechaFin.Year.ToString() + " 23:59") : DateTime.Parse(FechaInicio.AddDays(-1).ToShortDateString() + " 23:59");

                // var SaldoMensualBancos = dbContext.saldomensualbancos.Where(k => k.i_Eliminado == 0).ToList();
                var SaldoMensualBancos = int.Parse(FechaInicio.Month.ToString()) == (int)Mes.Enero ? 0 : ObtenerSaldoMensualBanco(ref objOperationResult, FechaInicioAnterior, FechaFinAnterior, Cuenta1.ToString(), MonedaReporte, Orden, TipoDocDetalle, NumeroDoc, UsadoProcesoSaldoMensual);

                var ListaCuentasMes = (from a in dbContext.tesoreria
                                       join b in dbContext.tesoreriadetalle on new { IdTesoreria = a.v_IdTesoreria, eliminado = 0 } equals new { IdTesoreria = b.v_IdTesoreria, eliminado = b.i_Eliminado.Value } into b_join

                                       from b in b_join.DefaultIfEmpty()

                                       where a.t_FechaRegistro >= FechaInicio && a.t_FechaRegistro <= FechaFin && a.i_Eliminado == 0
                                       select new
                                       {
                                           v_NroCuenta = b.v_NroCuenta,
                                           v_idTesoreriaDetalle = b.v_IdTesoreriaDetalle,


                                       });

                var ListaRequerida = (from A in ListaCuentasMes.ToList()
                                      select new
                                      {
                                          v_NroCuentaInt = int.Parse(A.v_NroCuenta),
                                      }).Distinct();

                var ListaFinalRequerida = ListaRequerida.Where(o => o.v_NroCuentaInt >= Cuenta1 && o.v_NroCuentaInt <= Cuenta2).ToList().OrderBy(x => x.v_NroCuentaInt);


                var ReporteLibroBancoInicial = (from a in dbContext.tesoreria

                                                join b in dbContext.tesoreriadetalle on new { Tesoreria = a.v_IdTesoreria, eliminado = 0 }
                                                                                    equals new { Tesoreria = b.v_IdTesoreria, eliminado = b.i_Eliminado.Value } into b_join

                                                from b in b_join.DefaultIfEmpty()


                                                join c in dbContext.documento on new { TipoDoc = a.i_IdTipoDocumento.Value, eliminado = 0 }
                                                                               equals new { TipoDoc = c.i_CodigoDocumento, eliminado = c.i_Eliminado.Value } into c_join

                                                from c in c_join.DefaultIfEmpty()

                                                join d in dbContext.documento on new { TipoDoc1 = b.i_IdTipoDocumento.Value, eliminado = 0 }
                                                                                equals new { TipoDoc1 = d.i_CodigoDocumento, eliminado = d.i_Eliminado.Value } into d_join

                                                from d in d_join.DefaultIfEmpty()

                                                join e in dbContext.asientocontable on new { b.v_NroCuenta, eliminado = 0, per = periodo }
                                                                                    equals new { e.v_NroCuenta, eliminado = e.i_Eliminado.Value, per = e.v_Periodo } into e_join

                                                from e in e_join.DefaultIfEmpty()



                                                where a.i_Eliminado == 0 &&

                                                a.t_FechaRegistro >= FechaInicio && a.t_FechaRegistro <= FechaFin

                                                && (b.i_IdTipoDocumento == TipoDocDetalle || TipoDocDetalle == -1) && (b.v_NroDocumento.Contains(NumeroDoc) || NumeroDoc == "")
                                                select new ReporteLibroBanco
                                                {

                                                    Moneda = a.i_IdMoneda == 1 ? "S/." : "US$.",
                                                    NumeroCuenta = b.v_NroCuenta,
                                                    NombreCuenta = e == null ? "*No Existe*" : e.v_NombreCuenta,
                                                    Comprobante = c == null ? a.v_Mes.Trim() + "-" + a.v_Correlativo.Trim() : c.v_Siglas.Trim() + " " + a.v_Mes.Trim() + "-" + a.v_Correlativo.Trim(),
                                                    Documento = d == null ? b.v_NroDocumento.Trim() : d.v_Siglas.Trim() + " " + b.v_NroDocumento.Trim(),

                                                    iDia = a.t_FechaRegistro.Value.Day,

                                                    Detalle = a.i_IdEstado == 0 ? "A N U L A D O" : string.IsNullOrEmpty(b.v_Analisis) ? a.v_Glosa : b.v_Analisis,
                                                    Debe = a.i_IdEstado == 0 ? 0 : b.v_Naturaleza == "D" && MonedaReporte == a.i_IdMoneda ? b.d_Importe.Value : b.v_Naturaleza == "D" && MonedaReporte != b.d_Importe.Value ? b.d_Cambio.Value : 0,
                                                    Haber = a.i_IdEstado == 0 ? 0 : b.v_Naturaleza == "H" && MonedaReporte == a.i_IdMoneda ? b.d_Importe.Value : b.v_Naturaleza == "H" && MonedaReporte != b.d_Importe.Value ? b.d_Cambio.Value : 0,
                                                    v_IdTesoreria = a.v_IdTesoreria,
                                                    v_IdTesoreriaDetalle = b.v_IdTesoreriaDetalle,

                                                    Fecha = a.t_FechaRegistro,

                                                });

                var ReporteLibroBancoConvertido = (from A in ReporteLibroBancoInicial.ToList()

                                                   select new ReporteLibroBanco
                                                   {
                                                       Moneda = A.Moneda,
                                                       NumeroCuenta = A.NumeroCuenta,
                                                       NombreCuenta = A.NombreCuenta,
                                                       Comprobante = A.Comprobante,
                                                       Documento = A.Documento,
                                                       // Dia = A.Dia,
                                                       iDia = A.iDia,
                                                       Detalle = A.Detalle,
                                                       Debe = A.Debe,
                                                       Haber = A.Haber,
                                                       SaldoSegunBanco = A.SaldoSegunBanco,
                                                       NumeroCuentaInt = int.Parse(A.NumeroCuenta),
                                                       v_IdTesoreria = A.v_IdTesoreria,
                                                       v_IdTesoreriaDetalle = A.v_IdTesoreriaDetalle,
                                                       Fecha = A.Fecha,
                                                       Dia = A.iDia.ToString("00"),
                                                   }).AsQueryable();

                var ReporteLibroBancoOrdenado = ReporteLibroBancoConvertido.OrderBy(Orden).ToList();

                var ReporteLibroBanco = (from B in ReporteLibroBancoOrdenado

                                         where B.NumeroCuentaInt >= Cuenta1 && B.NumeroCuentaInt <= Cuenta2

                                         select B).ToList().OrderBy(o => o.NumeroCuentaInt);

                List<ReporteLibroBanco> ReporteLibroBancoFinal = new List<ReporteLibroBanco>();
                int i = -1; decimal saldoanterior = 0; string cuenta = string.Empty;
                foreach (var Fila in ReporteLibroBanco.AsParallel())
                {
                    if (MonedaReporte == 1)
                    {
                        if (int.Parse(FechaInicio.Month.ToString()) == (int)Mes.Enero)
                        {
                            var SaldoDiciembreSoles = dbContext.saldomensualbancos.Where(o => o.i_Eliminado == 0 && o.v_Mes == SaldoMes && o.v_NroCuenta == Fila.NumeroCuenta && o.v_Anio == SaldoAnio).FirstOrDefault();
                            Fila.SaldoSegunBanco = SaldoDiciembreSoles != null ? Fila.SaldoSegunBanco = SaldoDiciembreSoles.d_SaldoSoles : 0;


                        }
                        else
                        {
                            Fila.SaldoSegunBanco = SaldoMensualBancos;
                        }
                    }
                    else
                    {
                        if (int.Parse(FechaInicio.Month.ToString()) == (int)Mes.Enero)
                        {
                            var SaldoDiciembreDolares = dbContext.saldomensualbancos.Where(o => o.i_Eliminado == 0 && o.v_Mes == SaldoMes && o.v_NroCuenta == Fila.NumeroCuenta && o.v_Anio == SaldoAnio).FirstOrDefault();
                            Fila.SaldoSegunBanco = SaldoDiciembreDolares != null ? SaldoDiciembreDolares.d_SaldoDolares : 0;

                        }
                        else
                        {
                            Fila.SaldoSegunBanco = SaldoMensualBancos;
                        }

                    }

                    if (Fila.NumeroCuenta == cuenta || cuenta == string.Empty)
                    {
                        i = i + 1;

                    }
                    else
                    {
                        i = 0;
                        saldoanterior = 0;

                    }

                    Fila.Dia = Fila.iDia.ToString("00");
                    Fila.Saldo = i == 0 ? Fila.SaldoSegunBanco + Fila.Debe - Fila.Haber : saldoanterior + Fila.Debe - Fila.Haber;
                    saldoanterior = Fila.Saldo.Value;
                    Fila.SaldoFinal = saldoanterior;
                    ReporteLibroBancoFinal.Add(Fila);
                    cuenta = Fila.NumeroCuenta;



                }

                var ListaNoContenidasReporteLibroBanco = ListaFinalRequerida.Select(o => o.v_NroCuentaInt).Except(ReporteLibroBanco.Select(h => h.NumeroCuentaInt)).ToList();

                if (ListaNoContenidasReporteLibroBanco.Count() != 0)  //No aparecen porque no tiene saldo Mensual Banco
                {

                    var ReporteLibroBancoFaltantes = (from a in dbContext.tesoreria

                                                      join b in dbContext.tesoreriadetalle on new { idTesoreria = a.v_IdTesoreria, eliminado = 0 } equals new { idTesoreria = b.v_IdTesoreria, eliminado = b.i_Eliminado.Value } into b_join

                                                      from b in b_join.DefaultIfEmpty()


                                                      join c in dbContext.documento on new { TipoDoc = a.i_IdTipoDocumento.Value, eliminado = 0 }
                                                                                     equals new { TipoDoc = c.i_CodigoDocumento, eliminado = c.i_Eliminado.Value } into c_join

                                                      from c in c_join.DefaultIfEmpty()

                                                      join d in dbContext.documento on new { TipoDoc1 = b.i_IdTipoDocumento.Value, eliminado = 0 }
                                                                                    equals new { TipoDoc1 = d.i_CodigoDocumento, eliminado = d.i_Eliminado.Value } into d_join

                                                      from d in d_join.DefaultIfEmpty()

                                                      join e in dbContext.asientocontable on new { b.v_NroCuenta, eliminado = 0, per = periodo }
                                                                                            equals new { e.v_NroCuenta, eliminado = e.i_Eliminado.Value, per = e.v_Periodo } into e_join

                                                      from e in e_join.DefaultIfEmpty()

                                                      where a.i_Eliminado == 0 &&

                                                       a.t_FechaRegistro >= FechaInicio && a.t_FechaRegistro <= FechaFin


                                                        && (b.i_IdTipoDocumento == TipoDocDetalle || TipoDocDetalle == -1) && (b.v_NroDocumento.Contains(NumeroDoc) || NumeroDoc == "")
                                                      select new ReporteLibroBanco
                                                      {

                                                          Moneda = a.i_IdMoneda == 1 ? "S/." : "US$.",
                                                          NumeroCuenta = b.v_NroCuenta,
                                                          NombreCuenta = e == null ? "*No Existe*" : e.v_NombreCuenta,
                                                          Comprobante = c == null ? a.v_Mes.Trim() + "-" + a.v_Correlativo.Trim() : c.v_Siglas.Trim() + " " + a.v_Mes.Trim() + "-" + a.v_Correlativo.Trim(),
                                                          Documento = d == null ? b.v_NroDocumento.Trim() : d.v_Siglas.Trim() + " " + b.v_NroDocumento.Trim(),
                                                          iDia = a.t_FechaRegistro.Value.Day,
                                                          Detalle = a.i_IdEstado == 0 ? "A N U L A D O" : string.IsNullOrEmpty(b.v_Analisis) ? a.v_Glosa : b.v_Analisis,
                                                          Debe = a.i_IdEstado == 0 ? 0 : b.v_Naturaleza == "D" && MonedaReporte == a.i_IdMoneda ? b.d_Importe.Value : b.v_Naturaleza == "D" && MonedaReporte != a.i_IdMoneda ? b.d_Cambio.Value : 0,
                                                          Haber = a.i_IdEstado == 0 ? 0 : b.v_Naturaleza == "H" && MonedaReporte == a.i_IdMoneda ? b.d_Importe.Value : b.v_Naturaleza == "H" && MonedaReporte != a.i_IdMoneda ? b.d_Cambio.Value : 0,
                                                          SaldoSegunBanco = 0,
                                                          v_IdTesoreria = a.v_IdTesoreria,
                                                          v_IdTesoreriaDetalle = b.v_IdTesoreriaDetalle,
                                                          Fecha = a.t_FechaRegistro,

                                                      }).ToList();

                    var ReporteLibroBancoFaltantesConvertido = (from A in ReporteLibroBancoFaltantes.ToList()

                                                                select new ReporteLibroBanco
                                                                {

                                                                    Moneda = A.Moneda,
                                                                    NumeroCuenta = A.NumeroCuenta,
                                                                    NombreCuenta = A.NombreCuenta,
                                                                    Comprobante = A.Comprobante,
                                                                    Documento = A.Documento,
                                                                    //Dia = A.Dia,
                                                                    Detalle = A.Detalle,
                                                                    Debe = A.Debe,
                                                                    Haber = A.Haber,
                                                                    SaldoSegunBanco = 0,
                                                                    v_IdTesoreria = A.v_IdTesoreria,
                                                                    v_IdTesoreriaDetalle = A.v_IdTesoreriaDetalle,
                                                                    NumeroCuentaInt = int.Parse(A.NumeroCuenta),
                                                                    Fecha = A.Fecha,
                                                                    Dia = A.iDia.ToString("00"),
                                                                }).AsQueryable();


                    var ReporteLibroBancoFaltantesOrdenado = ReporteLibroBancoFaltantesConvertido.OrderBy(Orden).ToList();

                    int j = -1; decimal SaldoAnterior = 0; string Cuenta = string.Empty;
                    List<ReporteLibroBanco> ListaFinal = new List<ReporteLibroBanco>();
                    foreach (var Fila in ReporteLibroBancoFaltantesOrdenado)
                    {
                        if (ListaNoContenidasReporteLibroBanco.Contains(Fila.NumeroCuentaInt))
                        {
                            if (Fila.NumeroCuenta == Cuenta || Cuenta == string.Empty)
                            {
                                j = j + 1;

                            }
                            else
                            {
                                j = 0;
                                SaldoAnterior = 0;

                            }
                            Fila.Dia = Fila.iDia.ToString("00");
                            Fila.Saldo = j == 0 ? Fila.SaldoSegunBanco + Fila.Debe - Fila.Haber : SaldoAnterior + Fila.Debe - Fila.Haber;
                            SaldoAnterior = Fila.Saldo.Value;
                            Fila.SaldoFinal = SaldoAnterior;
                            ListaFinal.Add(Fila);
                            Cuenta = Fila.NumeroCuenta;

                        }
                    }

                    return ReporteLibroBancoFinal.Union(ListaFinal).OrderBy(x => x.NumeroCuentaInt).ToList();

                }
                else
                {

                    return ReporteLibroBancoFinal.OrderBy(x => x.NumeroCuentaInt).ToList();
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;
            }

        }


        public string ObtenerNombreCuenta(string pstrNumeroCuenta)
        {
            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
            try
            {
                var AsientoContable = (from n in dbContext.asientocontable

                                       where n.i_Eliminado == 0 && n.v_NroCuenta == pstrNumeroCuenta && n.i_Imputable == 1 && n.v_Periodo == periodo
                                       select new { n.v_NombreCuenta }).FirstOrDefault();

                if (AsientoContable != null)
                {
                    return AsientoContable.v_NombreCuenta;

                }
                else
                {
                    return string.Empty;
                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string Cheque(string pstrIdTesoreriaDetalle)
        {
            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
            var Cheque = (from n in dbContext.tesoreriadetalle

                          where n.i_Eliminado == 0 && n.v_IdTesoreriaDetalle == pstrIdTesoreriaDetalle

                          select new
                          {
                              n.v_NroDocumento
                          }).FirstOrDefault();
            if (Cheque != null)
            {
                return Cheque.v_NroDocumento;
            }
            else
            {
                return "";

            }

        }
        public List<ReporteComprobanteTesoreria> ReporteComprobanteTesoreria(string pstrIdTesoreria)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                var Reporte = (from m in dbContext.tesoreria

                               join n in dbContext.tesoreriadetalle on new { m.v_IdTesoreria, eliminado = 0 }
                                                                equals new { n.v_IdTesoreria, eliminado = n.i_Eliminado.Value } into n_join

                               from n in n_join.DefaultIfEmpty()

                               join o in dbContext.documento on new { TipoDoc = m.i_IdTipoDocumento.Value, eliminado = 0 }
                                                            equals new { TipoDoc = o.i_CodigoDocumento, eliminado = o.i_Eliminado.Value } into o_join

                               from o in o_join.DefaultIfEmpty()

                               join p in dbContext.documento on new { TipoDoc = n.i_IdTipoDocumento.Value, eliminado = 0 }
                                                            equals new { TipoDoc = p.i_CodigoDocumento, eliminado = p.i_Eliminado.Value } into p_join

                               from p in p_join.DefaultIfEmpty()

                               join q in dbContext.asientocontable on new { cuenta = m.v_NroCuentaCajaBanco, eliminado = 0, per = periodo }
                                                                equals new { cuenta = q.v_NroCuenta, eliminado = q.i_Eliminado.Value, per = q.v_Periodo } into q_join
                               from q in q_join.DefaultIfEmpty()


                               join l in dbContext.cliente on new { proveedor = n.v_IdCliente, eliminado = 0 } equals new { proveedor = l.v_IdCliente, eliminado = l.i_Eliminado.Value } into l_join

                               from l in l_join.DefaultIfEmpty()
                               where m.i_Eliminado == 0 && m.v_IdTesoreria == pstrIdTesoreria

                               select new ReporteComprobanteTesoreria
                               {
                                   NumeroComprobanteTesoreria = o == null ? m.v_Mes.Trim() + "-" + m.v_Correlativo.Trim() : "COMPROBANTE DE TESORERIA N° " + o.v_Siglas + " " + m.v_Mes.Trim() + "-" + m.v_Correlativo.Trim(),
                                   TipoMovimiento = m.i_TipoMovimiento == 1 ? "INGRESO" : "EGRESO",
                                   Banco = q == null ? m.v_NroCuentaCajaBanco : m.v_NroCuentaCajaBanco + " " + q.v_NombreCuenta,
                                   Nombre = m.v_Nombre,
                                   Concepto = m.v_Glosa,
                                   Fecha = m.t_FechaRegistro.Value,
                                   NumeroCheque = n.i_IdTipoDocumento == 309 ? n.v_NroDocumento : string.Empty,
                                   Importe = 0,
                                   Moneda = m.i_IdMoneda == 1 ? "SOLES" : "DOLARES",
                                   Cuentas = n.v_NroCuenta,
                                   CentroCostos = n.i_IdCentroCostos == null || n.i_IdCentroCostos == "-1" ? "" : n.i_IdCentroCostos,
                                   iPartidaCaja = n.i_IdCaja == null ? 0 : n.i_IdCaja.Value,
                                   Detalle = l == null ? "" : (l.v_ApePaterno + " " + l.v_ApeMaterno + " " + l.v_PrimerNombre + " " + l.v_RazonSocial).Trim(),
                                   MonedaExtranjera = m.i_IdMoneda == 1 ? 0 : n.d_Importe.Value,
                                   TipoCambio = m.d_TipoCambio,
                                   Debe = n.v_Naturaleza == "D" && m.i_IdMoneda.Value == 1 ? n.d_Importe.Value : n.v_Naturaleza == "D" && m.i_IdMoneda.Value == 2 ? n.d_Cambio.Value : 0,
                                   Haber = n.v_Naturaleza == "H" && m.i_IdMoneda == 1 ? n.d_Importe.Value : n.v_Naturaleza == "H" && m.i_IdMoneda.Value == 2 ? n.d_Cambio.Value : 0,
                                   DocumentoReferencia = p == null ? n.v_NroDocumento : p.v_Siglas + " " + n.v_NroDocumento,
                                   DebeString = "",
                                   HaberString = "",
                                   v_Naturaleza = n.v_Naturaleza,
                                   i_IdTipoDocumento = n.i_IdTipoDocumento,
                                   i_IdTipoMovimiento = m.i_TipoMovimiento,
                                   d_Importe = n.d_Importe,
                               }).ToList();



                var ReporteFinal = (from a in Reporte

                                    let CalcularValores = CalcularImportes(Reporte, a.Banco)
                                    let NumeroCheque = ObtenerNumeroCheque(Reporte)

                                    select new ReporteComprobanteTesoreria
                                    {
                                        NumeroComprobanteTesoreria = a.NumeroComprobanteTesoreria,
                                        TipoMovimiento = a.TipoMovimiento,
                                        Banco = a.Banco,
                                        Nombre = a.Nombre,
                                        Concepto = a.Concepto,
                                        Fecha = a.Fecha,
                                        NumeroCheque = NumeroCheque,
                                        Importe = CalcularValores,
                                        Moneda = a.Moneda,
                                        Cuentas = a.Cuentas,
                                        CentroCostos = a.CentroCostos,
                                        PartidaCaja = a.iPartidaCaja==0?"" : a.PartidaCaja.ToString (),
                                        Detalle = a.Detalle,
                                        MonedaExtranjera = a.MonedaExtranjera,
                                        TipoCambio = a.TipoCambio,
                                        Debe = a.Debe,
                                        Haber = a.Haber,
                                        DocumentoReferencia = a.DocumentoReferencia,
                                        DebeString = a.Debe != 0 ? a.Debe.Value.ToString("#,#.##") : "",
                                        HaberString = a.Haber != 0 ? a.Haber.Value.ToString("#,#.##") : "",
                                        i_IdTipoDocumento = a.i_IdTipoDocumento,
                                        i_IdTipoMovimiento = a.i_IdTipoMovimiento
                                    }).ToList();


                return ReporteFinal;
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        private decimal CalcularImportes(List<ReporteComprobanteTesoreria> ReporteComprobanteTesoreria, string CuentaCajaBanco)
        {
            decimal ImporteTotal = 0;

            if (ReporteComprobanteTesoreria[0].i_IdTipoMovimiento == 2)
            {
                var Importe = ReporteComprobanteTesoreria.FindAll(p => p.Banco == CuentaCajaBanco && p.v_Naturaleza == "H" && p.i_IdTipoDocumento == 309);
                ImporteTotal = Importe.Sum(s => s.d_Importe).Value;
            }
            else
            {

                var Importe = ReporteComprobanteTesoreria.FindAll(p => p.Banco == CuentaCajaBanco && p.v_Naturaleza == "D");
                ImporteTotal = Importe.Sum(s => s.d_Importe).Value;
            }

            return ImporteTotal;

        }

        public string ObtenerNumeroCheque(List<ReporteComprobanteTesoreria> ReporteComprobanteTesoreria)
        {

            var Result = ReporteComprobanteTesoreria.FindAll(x => x.i_IdTipoDocumento == 309).FirstOrDefault();
            if (Result != null)
            {
                string NumeroCheque = Result.NumeroCheque;
                return NumeroCheque;
            }
            else
            {
                return string.Empty;
            }



        }
        private decimal SumaCheques(string pstrIdTesoreriaDetalle)
        {

            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

            var Suma = (from n in dbContext.tesoreriadetalle

                        where n.v_IdTesoreriaDetalle == pstrIdTesoreriaDetalle && n.i_IdTipoDocumento == 309

                        select n).Sum(o => o.d_Importe.Value);
            return Suma;


        }


        public List<ReporteChequesGirados> ReporteChequesGiradosSinRango(ref OperationResult objOperationResult, DateTime FechaInicio, DateTime FechaFin, string Orden, int Moneda)
        {
            try
            {
                objOperationResult.Success = 1;

                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();



                var Tesoreria = (from m in dbContext.tesoreria

                                 join n in dbContext.tesoreriadetalle on new { eliminado = 0, t = m.v_IdTesoreria, tipodoc = 309 } equals new { eliminado = n.i_Eliminado.Value, t = n.v_IdTesoreria, tipodoc = n.i_IdTipoDocumento.Value } into n_join

                                 from n in n_join

                                 join o in dbContext.asientocontable on new { eliminado = 0, c = n.v_NroCuenta, per = periodo } equals new { eliminado = o.i_Eliminado.Value, c = o.v_NroCuenta, per = o.v_Periodo } into o_join

                                 from o in o_join

                                 join p in dbContext.documento on new { doc = m.i_IdTipoDocumento.Value, eliminado = 0 } equals new { doc = p.i_CodigoDocumento, eliminado = p.i_Eliminado.Value } into p_join

                                 from p in p_join

                                 join q in dbContext.documento on new { td = n.i_IdTipoDocumento.Value, eliminado = 0 } equals new { td = q.i_CodigoDocumento, eliminado = q.i_Eliminado.Value } into q_join

                                 from q in q_join

                                 where m.t_FechaRegistro >= FechaInicio && m.t_FechaRegistro <= FechaFin && m.i_Eliminado.Value == 0

                                 select new ReporteChequesGirados
                                 {

                                     Cuenta = n == null ? "" : n.v_NroCuenta + " " + o.v_NombreCuenta,
                                     MonedaReporte = m.i_IdMoneda == 1 ? "S/." : "US$.",
                                     NumeroComprobante = p == null ? m.v_Mes + "-" + m.v_Correlativo : p.v_Siglas + " " + m.v_Mes + "-" + m.v_Correlativo,
                                     NumeroDocumento = q != null ? n != null ? q.v_Siglas + " " + n.v_NroDocumento : "" : "",
                                     Fecha = m.t_FechaRegistro.Value,
                                     Detalle = m.v_Glosa,
                                     ImporteSoles = m.i_IdMoneda == 1 ? n == null ? 0 : n.d_Importe : 0,
                                     ImporteDolares = m.i_IdMoneda == 2 ? n == null ? 0 : n.d_Importe : 0,
                                     Moneda = m.i_IdMoneda.Value,
                                     Importe = n == null ? 0 : n.d_Importe,
                                 });

                if (Moneda != -1)
                {
                    Tesoreria = Tesoreria.Where(o => o.Moneda == Moneda);

                }

                if (!string.IsNullOrEmpty(Orden))
                {
                    Tesoreria = Tesoreria.OrderBy(Orden);
                }

                List<ReporteChequesGirados> objData = Tesoreria.ToList();

                return objData;
            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                return null;
            }

        }



        public List<ReporteChequesGirados> ReporteChequesGiradosRango(ref OperationResult objOperationReuslt, DateTime FechaInicio, DateTime FechaFin, string Orden, int MonedaFiltrar, string Cuenta1, string Cuenta2)
        {


            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

            int CuentaInicial = int.Parse(Cuenta1);
            int CuentaFinal = int.Parse(Cuenta2);
            try
            {
                objOperationReuslt.Success = 1;

                var Tesoreria = (from m in dbContext.tesoreria

                                 join n in dbContext.tesoreriadetalle on new { t = m.v_IdTesoreria, eliminado = 0, tipodoc = 309 } equals new { t = n.v_IdTesoreria, eliminado = n.i_Eliminado.Value, tipodoc = n.i_IdTipoDocumento.Value } into n_join

                                 from n in n_join

                                 join o in dbContext.asientocontable on new { c = n.v_NroCuenta, eliminado = 0, per = periodo } equals new { c = o.v_NroCuenta, eliminado = o.i_Eliminado.Value, per = o.v_Periodo } into o_join

                                 from o in o_join

                                 join p in dbContext.documento on new { tp = m.i_IdTipoDocumento.Value, eliminado = 0 } equals new { tp = p.i_CodigoDocumento, eliminado = p.i_Eliminado.Value } into p_join

                                 from p in p_join

                                 join q in dbContext.documento on new { tipodoc = n.i_IdTipoDocumento.Value, eliminado = 0 } equals new { tipodoc = q.i_CodigoDocumento, eliminado = q.i_Eliminado.Value } into q_join

                                 from q in q_join

                                 where m.t_FechaRegistro >= FechaInicio && m.t_FechaRegistro <= FechaFin && m.i_Eliminado == 0

                                 select new ReporteChequesGirados
                                 {

                                     Cuenta = o != null && n != null ? n.v_NroCuenta + " " + o.v_NombreCuenta : n != null ? n.v_NroCuenta : "",
                                     MonedaReporte = m.i_IdMoneda == 1 ? "S/." : "US$.",
                                     NumeroComprobante = p != null && m != null ? p.v_Siglas + " " + m.v_Mes + "-" + m.v_Correlativo : "",
                                     NumeroDocumento = n != null && q != null ? q.v_Siglas + " " + n.v_NroDocumento : "",
                                     Fecha = m.t_FechaRegistro.Value,
                                     Detalle = m.v_Glosa,
                                     ImporteSoles = m.i_IdMoneda == 1 ? n.d_Importe : 0,
                                     ImporteDolares = m.i_IdMoneda == 2 ? n.d_Importe : 0,
                                     Moneda = m.i_IdMoneda.Value,
                                     CuentaString = n == null ? "0" : n.v_NroCuenta,
                                     CuentaInt = 0,
                                     Importe = n == null ? 0 : n.d_Importe,
                                 }).ToList();



                var TesoreriaInt = (from A in Tesoreria
                                    select new ReporteChequesGirados
                                    {
                                        Cuenta = A.Cuenta,
                                        MonedaReporte = A.MonedaReporte,
                                        NumeroComprobante = A.NumeroComprobante,
                                        NumeroDocumento = A.NumeroDocumento,
                                        Fecha = A.Fecha,
                                        Detalle = A.Detalle,
                                        ImporteSoles = A.ImporteSoles,
                                        ImporteDolares = A.ImporteDolares,
                                        Moneda = A.Moneda,
                                        CuentaString = A.CuentaString,
                                        CuentaInt = int.Parse(A.CuentaString),
                                        Importe = A.Importe,

                                    }).AsQueryable();

                if (MonedaFiltrar != -1)
                {
                    TesoreriaInt = TesoreriaInt.Where(o => o.Moneda == MonedaFiltrar);
                }

                var ListaFinal = TesoreriaInt.Where(o => o.CuentaInt.Value >= CuentaInicial && o.CuentaInt.Value <= CuentaFinal);

                if (!string.IsNullOrEmpty(Orden))
                {
                    TesoreriaInt = ListaFinal.OrderBy(Orden);
                }

                List<ReporteChequesGirados> objData = TesoreriaInt.ToList();

                return objData;
            }
            catch (Exception e)
            {
                objOperationReuslt.Success = 0;
                return null;
            }


        }



        public List<ReporteListadoComprobantesTesoreria> ReporteComprobanteTesoreriaIngresosEgresos(ref OperationResult objOperationResult, int pIntTipoMovimiento, string pstrPeriodo, int pIntTipoDocumento, int DocInicial, int DocFinal, int Orden)
        {
            try
            {
                objOperationResult.Success = 1;
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                var Reporte = (from m in dbContext.tesoreria

                               join n in dbContext.tesoreriadetalle on new { m.v_IdTesoreria, eliminado = 0 }
                                                                equals new { n.v_IdTesoreria, eliminado = n.i_Eliminado.Value } into n_join

                               from n in n_join.DefaultIfEmpty()

                               join o in dbContext.documento on new { TipoDoc = m.i_IdTipoDocumento.Value, eliminado = 0 }
                                                            equals new { TipoDoc = o.i_CodigoDocumento, eliminado = o.i_Eliminado.Value } into o_join

                               from o in o_join.DefaultIfEmpty()

                               join p in dbContext.documento on new { TipoDoc = n.i_IdTipoDocumento.Value, eliminado = 0 }
                                                            equals new { TipoDoc = p.i_CodigoDocumento, eliminado = p.i_Eliminado.Value } into p_join

                               from p in p_join.DefaultIfEmpty()

                               join q in dbContext.asientocontable on new { cuenta = m.v_NroCuentaCajaBanco, eliminado = 0, per = periodo }
                                                                equals new { cuenta = q.v_NroCuenta, eliminado = q.i_Eliminado.Value, per = q.v_Periodo } into q_join
                               from q in q_join.DefaultIfEmpty()
                               where m.i_Eliminado == 0 && m.v_Periodo == pstrPeriodo && m.i_TipoMovimiento == pIntTipoMovimiento && m.i_IdTipoDocumento == pIntTipoDocumento

                               select new ReporteListadoComprobantesTesoreria
                               {
                                   NumeroComprobanteTesoreria = o == null ? m.v_Mes.Trim() + "-" + m.v_Correlativo.Trim() : "COMPROBANTE DE TESORERIA N° " + o.v_Siglas + " " + m.v_Mes.Trim() + "-" + m.v_Correlativo.Trim(),
                                   TipoMovimiento = m.i_TipoMovimiento == 1 ? "INGRESO" : "EGRESO",
                                   NumeroyNombreBanco = q == null ? m.v_NroCuentaCajaBanco : m.v_NroCuentaCajaBanco + " " + q.v_NombreCuenta,
                                   Banco = m.v_NroCuentaCajaBanco,
                                   Nombre = m.v_Nombre,
                                   Concepto = m.v_Glosa,
                                   Fecha = m.t_FechaRegistro.Value,
                                   NumeroCheque = n.i_IdTipoDocumento == 309 ? n.v_NroDocumento : string.Empty,
                                   Importe = 0,
                                   Moneda = m.i_IdMoneda == 1 ? "SOLES" : "DOLARES",
                                   Cuenta = n.v_NroCuenta,
                                   CentroCostos = n.i_IdCentroCostos == null ? "" : n.i_IdCentroCostos,
                                   PartidaCaja = n.i_IdCaja == null ? -1 : n.i_IdCaja.Value,
                                   Detalle = "",
                                   MonedaExtranjera = m.i_IdMoneda == 1 ? 0 : n.d_Importe.Value,
                                   TipoCambio = m.d_TipoCambio.Value,
                                   //Debe = n.v_Naturaleza == "D" ? n.d_Cambio.Value : 0,
                                   //Haber = n.v_Naturaleza == "H" ? n.d_Cambio.Value : 0,

                                   Debe = n.v_Naturaleza == "D" && m.i_IdMoneda.Value == 1 ? n.d_Importe.Value : n.v_Naturaleza == "D" && m.i_IdMoneda.Value == 2 ? n.d_Cambio.Value : 0,
                                   Haber = n.v_Naturaleza == "H" && m.i_IdMoneda.Value == 1 ? n.d_Importe.Value : n.v_Naturaleza == "H" && m.i_IdMoneda.Value == 2 ? n.d_Cambio.Value : 0,


                                   DocumentoReferencia = p == null ? n.v_NroDocumento : p.v_Siglas + " " + n.v_NroDocumento,
                                   DebeString = "",
                                   HaberString = "",
                                   v_Naturaleza = n.v_Naturaleza,
                                   i_IdTipoDocumento = n.i_IdTipoDocumento.Value,
                                   i_IdTipoMovimiento = m.i_TipoMovimiento.Value,
                                   d_Importe = n.d_Importe.Value,
                                   NumeroDocumento = m.v_Mes.Trim() + m.v_Correlativo.Trim(),
                                   v_IdTesoreria = m.v_IdTesoreria,


                               }).ToList();

                var ReporteFinal = (from a in Reporte

                                    let CalcularValores = CalcularImportes(Reporte, a.Banco, a.v_IdTesoreria)
                                    let NumeroCheque = ObtenerNumeroCheque(Reporte, a.v_IdTesoreria)

                                    select new ReporteListadoComprobantesTesoreria
                                    {
                                        NumeroComprobanteTesoreria = a.NumeroComprobanteTesoreria,
                                        TipoMovimiento = a.TipoMovimiento,
                                        NumeroyNombreBanco = a.NumeroyNombreBanco,
                                        Nombre = a.Nombre,
                                        Concepto = a.Concepto,
                                        Fecha = a.Fecha,
                                        NumeroCheque = NumeroCheque,
                                        Importe = CalcularValores,
                                        Moneda = a.Moneda,
                                        Cuenta = a.Cuenta,
                                        CentroCostos = a.CentroCostos,
                                        PartidaCaja = a.PartidaCaja,
                                        Detalle = a.Detalle,
                                        MonedaExtranjera = a.MonedaExtranjera,
                                        TipoCambio = a.TipoCambio,
                                        Debe = a.Debe,
                                        Haber = a.Haber,
                                        DocumentoReferencia = a.DocumentoReferencia,
                                        i_IdTipoDocumento = a.i_IdTipoDocumento,
                                        i_IdTipoMovimiento = a.i_IdTipoMovimiento,
                                        DebeString = a.Debe != 0 ? a.Debe.Value.ToString("#,#.##") : "",
                                        HaberString = a.Haber != 0 ? a.Haber.Value.ToString("#,#.##") : "",
                                        NumeroDocumento = a.NumeroDocumento,
                                        v_Naturaleza = a.v_Naturaleza,
                                        TotalLetras = a.Moneda == "SOLES" ? SAMBHS.Common.Resource.Utils.ConvertLetter(CalcularValores.ToString(), "0") + " SOLES " + "S.E.U.O." : SAMBHS.Common.Resource.Utils.ConvertLetter(CalcularValores.ToString(), "0") + " DOLARES AMERICANOS " + "S.E.U.O.",
                                        v_IdTesoreria = a.v_IdTesoreria,

                                    }).ToList();

                if (DocInicial != 0 && DocFinal != 0)
                {
                    ReporteFinal = ReporteFinal.Where(x => int.Parse(x.NumeroDocumento) >= DocInicial && int.Parse(x.NumeroDocumento) <= DocFinal).ToList();
                }

                if (Orden == 0)
                {
                    return ReporteFinal.OrderBy(x => x.v_Naturaleza).ThenBy(y => y.Cuenta).ToList();
                }
                else
                {
                    return ReporteFinal;
                }
            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                return null;

            }
        }




        private decimal CalcularImportes(List<ReporteListadoComprobantesTesoreria> ReporteComprobanteTesoreria, string CuentaCajaBanco, string IdTesoreria)
        {
            decimal ImporteTotal = 0;

            if (ReporteComprobanteTesoreria[0].i_IdTipoMovimiento == 2)
            {
                var Importe = ReporteComprobanteTesoreria.FindAll(p => p.Banco == CuentaCajaBanco && p.v_IdTesoreria == IdTesoreria && p.v_Naturaleza == "H" && p.i_IdTipoDocumento == 309);
                ImporteTotal = Importe.Sum(s => s.d_Importe).Value;
            }
            else
            {

                var Importe = ReporteComprobanteTesoreria.FindAll(p => p.Banco == CuentaCajaBanco && p.v_IdTesoreria == IdTesoreria && p.v_Naturaleza == "D");
                ImporteTotal = Importe.Sum(s => s.d_Importe).Value;
            }

            return ImporteTotal;

        }


        public string ObtenerNumeroCheque(List<ReporteListadoComprobantesTesoreria> ReporteComprobanteTesoreria, string v_IdTesoreria)
        {

            var Result = ReporteComprobanteTesoreria.FindAll(x => x.i_IdTipoDocumento == 309 && x.v_IdTesoreria == v_IdTesoreria).FirstOrDefault();
            if (Result != null)
            {
                string NumeroCheque = Result.NumeroCheque;
                return NumeroCheque;
            }
            else
            {
                return string.Empty;
            }



        }


        public List<ReporteRetenciones> ReporteRetenciones(ref OperationResult objOperationResult, DateTime FechaInicio, DateTime FechaFin, string NumeroDocProveedor)
        {

            try
            {
                List<ReporteRetenciones> lista = new List<ReporteRetenciones>();
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {


                    var AdministracionConceptos = dbContext.administracionconceptos.ToList().Where(o => o.v_Periodo == Globals.ClientSession.i_Periodo.ToString() && o.i_Eliminado == 0 && int.Parse(o.v_Codigo) == (int)Concepto.ComprasMN || int.Parse(o.v_Codigo) == (int)Concepto.ComprasME).ToList();
                    var CuentasAdministracionCompra = AdministracionConceptos.Select(o => o.v_CuentaPVenta = o.v_CuentaPVenta).Distinct().ToList();
                    var CuentasAdministracionDetraccion = AdministracionConceptos.Select(o => o.v_CuentaPVenta = o.v_CuentaDetraccion).Distinct().ToList();
                    var CuentasFiltrar = CuentasAdministracionCompra.Concat(CuentasAdministracionDetraccion).Distinct().ToList();

                    var TodosDiferenciaCambio = (from a in dbContext.diario

                                                 join b in dbContext.diariodetalle on new { d = a.v_IdDiario, eliminado = 0 } equals new { d = b.v_IdDiario, eliminado = b.i_Eliminado.Value } into b_join
                                                 from b in b_join.DefaultIfEmpty()


                                                 join c in dbContext.cliente on new { c = b.v_IdCliente, eliminado = 0 } equals new { c = c.v_IdCliente, eliminado = c.i_Eliminado.Value } into c_join
                                                 from c in c_join.DefaultIfEmpty()

                                                 join d in dbContext.documento on new { doc = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { doc = d.i_CodigoDocumento, eliminado = d.i_Eliminado.Value } into d_join
                                                 from d in d_join.DefaultIfEmpty()

                                                 where a.i_Eliminado == 0 && a.i_IdTipoComprobante == (int)TipoComprobanteLibroDiario.DiferenciaCambio
                                                 && a.i_Eliminado == 0 && (c.v_NroDocIdentificacion == NumeroDocProveedor || NumeroDocProveedor == "")
                                                 select new
                                                 {
                                                     fechaTransaccion = a.t_Fecha,//  fechaemision o registro?
                                                     IdTipo = 5,
                                                     Tipo = "D/CAMBIO",
                                                     Anexo = b.v_IdCliente,
                                                     IdTipoDocumentoAnexo = b.i_IdTipoDocumento ?? 0,
                                                     IdTipoDocumentoAnexoRef = b.i_IdTipoDocumentoRef ?? 0,
                                                     NumeroDocumentoAnexoRef = b.v_NroDocumentoRef,
                                                     NumeroDocumentoAnexo = b.v_NroDocumento,
                                                     Debe = b.v_Naturaleza == "D" ? a.i_IdMoneda == (int)Currency.Soles ? b.d_Importe : b.d_Cambio : 0,
                                                     //Haber =  a.i_IdMoneda == (int)Currency.Soles ? b.d_Importe : b.d_Cambio,
                                                     Haber = b.v_Naturaleza == "H" ? a.i_IdMoneda == (int)Currency.Soles ? b.d_Importe : b.d_Cambio : 0,
                                                     proveedor = (c.v_NroDocIdentificacion + " " + c.v_ApePaterno + " " + c.v_ApeMaterno + " " + c.v_PrimerNombre + " " + c.v_RazonSocial).Trim(),
                                                     IdDiarioDetalle = b.v_IdDiarioDetalle,
                                                     v_NroCuenta = b.v_NroCuenta,
                                                     NroCompra = d.v_Siglas + " " + a.v_Mes + "-" + a.v_Correlativo,
                                                 }).ToList().Select(o =>
                                      {

                                          return new ReporteRetenciones
                                      {
                                          FechaTransaccion = o.fechaTransaccion.Value.ToShortDateString(),
                                          IdTipoTransaccion = o.IdTipo,
                                          Debe = o.Debe ?? 0,
                                          Haber = o.Haber ?? 0,
                                          TipoTransaccion = o.Tipo,
                                          Llave = o.Anexo + o.IdTipoDocumentoAnexo + o.NumeroDocumentoAnexo,
                                          LLaveReferencia = o.Anexo + o.IdTipoDocumentoAnexoRef + o.NumeroDocumentoAnexoRef,
                                          Proveedor = o.proveedor,
                                          IdDiarioDetalle = o.IdDiarioDetalle,
                                          v_NroCuenta = o.v_NroCuenta,
                                          dFechaTransaccion = o.fechaTransaccion.Value,
                                          NroCompra = o.NroCompra,

                                      };

                                      }).ToList();


                    TodosDiferenciaCambio = TodosDiferenciaCambio.Where(o => CuentasFiltrar.Contains(o.v_NroCuenta)).ToList(); // Se filtra todos los asientos que contengan a las cuentas de administracion conceptos


                    var TodosAsientoApertura = (from a in dbContext.diario

                                                join b in dbContext.diariodetalle on new { d = a.v_IdDiario, eliminado = 0 } equals new { d = b.v_IdDiario, eliminado = b.i_Eliminado.Value } into b_join
                                                from b in b_join.DefaultIfEmpty()


                                                join c in dbContext.cliente on new { c = b.v_IdCliente, eliminado = 0 } equals new { c = c.v_IdCliente, eliminado = c.i_Eliminado.Value } into c_join
                                                from c in c_join.DefaultIfEmpty()

                                                join d in dbContext.documento on new { doc = b.i_IdTipoDocumento.Value, eliminado = 0 } equals new { doc = d.i_CodigoDocumento, eliminado = d.i_Eliminado.Value } into d_join
                                                from d in d_join.DefaultIfEmpty()
                                                where a.i_Eliminado == 0 && a.i_IdTipoComprobante == (int)TipoComprobanteLibroDiario.Apertura
                                                && (c.v_NroDocIdentificacion == NumeroDocProveedor || NumeroDocProveedor == "")
                                                //&& b.v_NroCuenta.StartsWith("42")
                                                select new
                                                {
                                                    fechaTransaccion = b.t_Fecha,//  fechaemision o registro?
                                                    IdTipo = 1,
                                                    Tipo = "COMPRA",
                                                    Anexo = b.v_IdCliente,
                                                    IdTipoDocumentoAnexo = b.i_IdTipoDocumento ?? 0,
                                                    NumeroDocumentoAnexo = b.v_NroDocumento,
                                                    proveedor = (c.v_NroDocIdentificacion + " " + c.v_ApePaterno + " " + c.v_ApeMaterno + " " + c.v_PrimerNombre + " " + c.v_RazonSocial).Trim(),
                                                    Haber = a.i_IdMoneda == (int)Currency.Soles ? b.d_Importe : b.d_Cambio,
                                                    saldo = a.i_IdMoneda == (int)Currency.Soles ? b.d_Importe : b.d_Cambio,
                                                    ImporteDocumentoSustentatorio = a.i_IdMoneda == (int)Currency.Soles ? b.d_Importe : b.d_Cambio,
                                                    NroCompra = a.v_Mes + "-" + a.v_Correlativo,
                                                    IdCompra = b.v_IdDiarioDetalle,
                                                    NumeroDocumento = b.v_NroDocumento,
                                                    IdTipoDocumento = b.i_IdTipoDocumento.Value,
                                                    TipoDocumento = d.v_Siglas,
                                                    v_NroCuenta = b.v_NroCuenta,


                                                }).ToList().Select(o =>
                                                 {
                                                     var SerieCorr = o.NumeroDocumentoAnexo.Split('-');
                                                     return new ReporteRetenciones
                                                     {
                                                         FechaTransaccion = o.fechaTransaccion.Value.ToShortDateString(),
                                                         IdTipoTransaccion = o.IdTipo,
                                                         Haber = o.Haber ?? 0,
                                                         TipoTransaccion = o.Tipo,
                                                         Llave = o.Anexo + o.IdTipoDocumentoAnexo + o.NumeroDocumentoAnexo,
                                                         Proveedor = o.proveedor,
                                                         SerieDocumentoCompra = SerieCorr.Count() == 2 ? SerieCorr[0] : "",
                                                         CorrelativoDocumentoCompra = SerieCorr.Count() == 2 ? SerieCorr[1] : "",
                                                         IdTipoDocumentoAnexo = o.IdTipoDocumentoAnexo,
                                                         Anexo = o.Anexo,
                                                         Saldo = o.saldo ?? 0,
                                                         ImporteDocumentoSustentatorio = o.ImporteDocumentoSustentatorio ?? 0,
                                                         IdCompra = o.IdCompra,
                                                         NumeroDocumentoSustentatorio = o.NumeroDocumento,
                                                         TipoDocSustentatorio = o.TipoDocumento,
                                                         IdTipoDocumentoSustentatorio = o.IdTipoDocumento,
                                                         v_NroCuenta = o.v_NroCuenta,
                                                         dFechaTransaccion = o.fechaTransaccion.Value,
                                                         NroCompra = o.NroCompra,
                                                     };

                                                 }).ToList();


                    TodosAsientoApertura = TodosAsientoApertura.Where(o => CuentasFiltrar.Contains(o.v_NroCuenta)).ToList();

                    var Tesorerias = (from a in dbContext.tesoreriadetalle

                                      join b in dbContext.tesoreria on new { tes = a.v_IdTesoreria, eliminado = 0 } equals new { tes = b.v_IdTesoreria, eliminado = b.i_Eliminado.Value } into b_join
                                      from b in b_join.DefaultIfEmpty()

                                      join c in dbContext.documento on new { doc = b.i_IdTipoDocumento.Value, eliminado = 0 } equals new { doc = c.i_CodigoDocumento, eliminado = c.i_Eliminado.Value } into c_join
                                      from c in c_join.DefaultIfEmpty()

                                      where a.i_Eliminado == 0 && a.tesoreria.i_IdEstado == 1 && a.tesoreria.i_Eliminado == 0

                                      && a.v_NroCuenta.StartsWith("40")

                                      select new
                                      {
                                          NroTesoreria = c.v_Siglas + " " + a.tesoreria.v_Mes + "-" + a.tesoreria.v_Correlativo,
                                          v_IdDocumentoRetencionDetalle = a.v_IdDocumentoRetencionDetalle,
                                          v_IdProveedor = a.v_IdCliente,
                                          i_IdTipoDocumento = a.i_IdTipoDocumento ?? -1,
                                          v_NroDocumento = string.IsNullOrEmpty(a.v_NroDocumento) ? "" : a.v_NroDocumento,

                                      }).ToList().Select(o =>
                                      {
                                          var SerieCorr = o.v_NroDocumento.Split('-');
                                          return new ReporteRetenciones
                                          {
                                              NroTesoreria = o.NroTesoreria,
                                              v_IdDocumentoRetencionDetalle = o.v_IdDocumentoRetencionDetalle,
                                              IdProveedor = o.v_IdProveedor,
                                              IdTipoDocumentoAnexo = o.i_IdTipoDocumento,
                                              SerieDocumentoCompra = SerieCorr.Count() == 2 ? SerieCorr[0] : "",
                                              CorrelativoDocumentoCompra = SerieCorr.Count() == 2 ? SerieCorr[1] : ""
                                          };
                                      }).ToList();




                    var RegistroCompras = dbContext.compra.Where(o => o.i_Eliminado == 0 && o.i_IdEstado == 1).ToList();

                    var TodasCompras = (from a in dbContext.diariodetalle


                                        join b in dbContext.diario on new { d = a.v_IdDiario, eliminado = 0 } equals new { d = b.v_IdDiario, eliminado = b.i_Eliminado.Value } into b_join
                                        from b in b_join.DefaultIfEmpty()


                                        join c in dbContext.documento on new { td = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { td = c.i_CodigoDocumento, eliminado = c.i_Eliminado.Value } into c_join
                                        from c in c_join.DefaultIfEmpty()

                                        join d in dbContext.cliente on new { c = a.v_IdCliente, eliminado = 0 } equals new { c = d.v_IdCliente, eliminado = d.i_Eliminado.Value } into d_join
                                        from d in d_join.DefaultIfEmpty()
                                        where a.i_Eliminado == 0 && (d.v_NroDocIdentificacion == NumeroDocProveedor || NumeroDocProveedor == "")
                                        && b.i_IdTipoDocumento == (int)TipoComprobanteLibroDiario.DiarioCompras

                                        select new
                                        {
                                            fechaTransaccion = b.t_Fecha,//  fechaemision o registro?
                                            IdTipo = 1,
                                            Tipo = "COMPRA",
                                            NumeroDocumento = a.v_NroDocumento,
                                            IdTipoDocumento = a.i_IdTipoDocumento.Value,
                                            TipoDocumento = c.v_Siglas,
                                            proveedor = (d.v_NroDocIdentificacion + " " + d.v_ApePaterno + " " + d.v_ApeMaterno + " " + d.v_PrimerNombre + " " + d.v_RazonSocial).Trim(),
                                            IdProveedor = a.v_IdCliente,
                                            IdCompra = b.v_IdDocumentoReferencia,
                                            Haber = b.i_IdMoneda == (int)Currency.Soles ? a.d_Importe.Value : a.d_Cambio.Value,
                                            saldo = b.i_IdMoneda == (int)Currency.Soles ? a.d_Importe.Value : a.d_Cambio.Value,

                                            ImporteDocumentoSustentatorio = b.i_IdMoneda == (int)Currency.Soles ? a.d_Importe.Value : a.d_Cambio.Value,
                                            NroCompra = b.v_Mes + "-" + b.v_Correlativo,
                                            v_NroCuenta = a.v_NroCuenta,
                                            TipoDocNotaCredito = a.i_IdTipoDocumentoRef,
                                            NumeroDocNotaCredito = a.v_NroDocumentoRef ?? "",

                                        }).ToList().Select(o =>
                                        {

                                            var serCorr = o.NumeroDocumento.Split('-');
                                            var SerieCompra = "";
                                            var CorrelativoCompra = "";
                                            if (serCorr.Count() == 2)
                                            {
                                                SerieCompra = serCorr[0];
                                                CorrelativoCompra = serCorr[1];
                                            }
                                            var serrCorrReferencia = o.NumeroDocNotaCredito.Split('-');
                                            var SerieRef = "";
                                            var CorrelativoRef = "";
                                            if (serrCorrReferencia.Count() == 2)
                                            {
                                                SerieRef = serrCorrReferencia[0];
                                                CorrelativoRef = serrCorrReferencia[1];
                                            }
                                            var FechaTransaccion = RegistroCompras.Where(z => z.v_IdCompra == o.IdCompra).FirstOrDefault();

                                            return new ReporteRetenciones
                                            {

                                                FechaTransaccion = FechaTransaccion.t_FechaEmision.Value.ToShortDateString(),
                                                IdTipoTransaccion = o.IdTipoDocumento == (int)TiposDocumentos.NotaDebito || o.IdTipoDocumento == (int)TiposDocumentos.NotaCredito ? 3 : o.IdTipo,
                                                TipoTransaccion = o.IdTipoDocumento == (int)TiposDocumentos.NotaCredito ? "N/CREDITO" : o.IdTipoDocumento == (int)TiposDocumentos.NotaDebito ? "N/DÉBITO" : o.Tipo,  // o.Tipo,
                                                NumeroDocumentoSustentatorio = o.NumeroDocumento,
                                                TipoDocSustentatorio = o.TipoDocumento,
                                                IdTipoDocumentoSustentatorio = o.IdTipoDocumento,
                                                Proveedor = o.proveedor,
                                                Haber = o.IdTipoDocumento != (int)TiposDocumentos.NotaCredito ? o.Haber == null ? 0 : Utils.Windows.DevuelveValorRedondeado(o.Haber, 2) : 0,
                                                Saldo = o.saldo == null ? 0 : Utils.Windows.DevuelveValorRedondeado(o.saldo, 2),
                                                Debe = o.IdTipoDocumento == (int)TiposDocumentos.NotaCredito ? o.Haber == null ? 0 : Utils.Windows.DevuelveValorRedondeado(o.Haber, 2) : 0,
                                                IdProveedor = o.IdProveedor,
                                                IdCompra = o.IdCompra,
                                                SerieDocumentoCompra = serCorr.Count() == 2 ? SerieCompra : "",
                                                CorrelativoDocumentoCompra = serCorr.Count() == 2 ? CorrelativoCompra : "",
                                                ImporteDocumentoSustentatorio = o.ImporteDocumentoSustentatorio == null ? 0 : Utils.Windows.DevuelveValorRedondeado(o.ImporteDocumentoSustentatorio, 2),
                                                NroCompra = FechaTransaccion != null ? FechaTransaccion.v_Mes.Trim() + "-" + FechaTransaccion.v_Correlativo.Trim() : "",   //o.NroCompra,
                                                Llave = o.IdProveedor + o.IdTipoDocumento + SerieCompra + "-" + CorrelativoCompra,
                                                v_NroCuenta = o.v_NroCuenta,

                                                TipoDocNotaCredito = o.TipoDocNotaCredito ?? -1,
                                                SerieDocNotaCredito = SerieRef,
                                                CorrelativoDocNotaCredito = CorrelativoRef,
                                                dFechaTransaccion = FechaTransaccion.t_FechaEmision.Value,
                                                TipoDocReferencia = "",
                                                SerieDocReferencia = "",
                                                CorrelativoDocReferencia = "",
                                            };

                                        }).ToList();



                    TodasCompras = TodasCompras.Where(o => CuentasFiltrar.Contains(o.v_NroCuenta)).ToList(); // Se filtra todos los asientos que contengan a las cuentas de administracion conceptos


                    TodasCompras = TodasCompras.GroupBy(o => o.IdCompra).ToList().Select(d =>
                        {
                            var comp = d.FirstOrDefault();
                            comp.ImporteDocumentoSustentatorio = d.Sum(o => o.ImporteDocumentoSustentatorio);
                            comp.Haber = d.Sum(o => o.Haber);
                            return comp;

                        }).ToList();


                    var DocumentosRetencion = (from a in dbContext.documentoretenciondetalle

                                               join b in dbContext.documentoretencion on new { dr = a.v_IdDocumentoRetencion, eliminado = 0 } equals new { dr = b.v_IdDocumentoRetencion, eliminado = b.i_Eliminado.Value } into b_join
                                               from b in b_join.DefaultIfEmpty()

                                               join c in dbContext.cliente on new { c = b.v_IdCliente } equals new { c = c.v_IdCliente } into c_join
                                               from c in c_join.DefaultIfEmpty()
                                               where b.t_FechaRegistro >= FechaInicio && b.t_FechaRegistro <= FechaFin
                                               && b.i_Estado == 1 && a.i_Eliminado == 0

                                               && (c.v_NroDocIdentificacion == NumeroDocProveedor || NumeroDocProveedor == "")

                                               select new ReporteRetenciones
                                               {

                                                   Proveedor = (c.v_NroDocIdentificacion + " " + c.v_ApePaterno + " " + c.v_ApeMaterno + " " + c.v_PrimerNombre + " " + c.v_RazonSocial).Trim(),
                                                   dFechaTransaccion = b.t_FechaRegistro.Value, //fecha emision o registro ?
                                                   ImporteDocumentoSustentatorio = b.i_IdMoneda == (int)Currency.Soles ? a.d_MontoPago.Value : a.d_Cambio.Value,
                                                   IdTipoTransaccion = 4,
                                                   TipoTransaccion = "PAGO",
                                                   Debe = b.i_IdMoneda == (int)Currency.Soles ? a.d_MontoPago ?? 0 : a.d_Cambio ?? 0,
                                                   ComprobanteRetencion = string.IsNullOrEmpty(b.v_SerieDocumento) || string.IsNullOrEmpty(b.v_CorrelativoDocumento) ? "" : b.v_SerieDocumento + "-" + b.v_CorrelativoDocumento,
                                                   ImporteDocRetencion = a.d_MontoRetenido ?? 0,
                                                   IdTipoDocumentoSustentatorio = a.i_IdTipoDocumento.Value,
                                                   SerieDocSustentatorio = string.IsNullOrEmpty(a.v_SerieDocumento) ? "" : a.v_SerieDocumento,
                                                   CorrelativoDocSustentatorio = string.IsNullOrEmpty(a.v_CorrelativoDocumento) ? "" : a.v_CorrelativoDocumento,
                                                   IdProveedor = b.v_IdCliente,
                                                   RucProveedor = c.v_NroDocIdentificacion,
                                                   ApePaternoProveedor = c.v_ApePaterno,
                                                   ApeMaternoProveedor = c.v_ApeMaterno,
                                                   NombresProveedor = c.v_PrimerNombre + " " + c.v_SegundoNombre,
                                                   RazonSocialProveedor = c.v_RazonSocial,
                                                   SerieComprobanteRetencion = string.IsNullOrEmpty(b.v_SerieDocumento) ? "" : b.v_SerieDocumento,
                                                   CorrelativoComprobanteRetencion = string.IsNullOrEmpty(b.v_CorrelativoDocumento) ? "" : b.v_CorrelativoDocumento,
                                                   v_IdDocumentoRetencionDetalle = a.v_IdDocumentoRetencionDetalle,
                                                   NroRetencion = b.v_Mes + "-" + b.v_Correlativo,
                                                   v_IdDocumentoRetencion = b.v_IdDocumentoRetencion,
                                                   TipoDocReferencia = "",
                                                   SerieDocReferencia = "",
                                                   CorrelativoDocReferencia = "",

                                               }).ToList();




                    DocumentosRetencion.AsParallel().ToList().ForEach(detalles =>
                    {
                        detalles.Llave = detalles.IdProveedor + detalles.IdTipoDocumentoSustentatorio + detalles.SerieDocSustentatorio + "-" + detalles.CorrelativoDocSustentatorio;
                    });

                    List<ReporteRetenciones> Test = new List<ReporteRetenciones>();

                    DocumentosRetencion = DocumentosRetencion.OrderBy(o => o.Proveedor).OrderBy(o => o.Llave).ThenBy(o => o.dFechaTransaccion).ToList();
                    string LlaveAntigua = DocumentosRetencion.Any() ? DocumentosRetencion.FirstOrDefault().Llave : "";
                    decimal Saldo = 0;
                    int NumeroRegistros = DocumentosRetencion.Count();
                    int ContadorReg = 1;
                    List<ReporteRetenciones> ListaNotaCreditoNoTenerCuenta = new List<ReporteRetenciones>();
                    foreach (ReporteRetenciones item in DocumentosRetencion)
                    {
                        //Primero Busco Notas  de Credito o Debito
                        Saldo = 0;
                        ReporteRetenciones objReporte = new ReporteRetenciones();
                        List<ReporteRetenciones> Totales = new List<ReporteRetenciones>();
                        List<ReporteRetenciones> ListaNcr = new List<ReporteRetenciones>();
                        decimal ImporteCompra = 0;
                        List<ReporteRetenciones> Compra = new List<ReporteRetenciones>();
                        List<ReporteRetenciones> IdTes = new List<ReporteRetenciones>();
                       
                        var NotasCreditoDebito = TodasCompras.Where(p => p.TipoDocNotaCredito == item.IdTipoDocumentoSustentatorio && p.SerieDocNotaCredito == item.SerieDocSustentatorio && p.CorrelativoDocNotaCredito == item.CorrelativoDocSustentatorio && p.IdProveedor == item.IdProveedor).ToList();
                        foreach (var Ncr in NotasCreditoDebito)
                        {
                            objReporte = new ReporteRetenciones();
                            objReporte = Ncr;
                            objReporte.FechaTransaccion = Ncr.FechaTransaccion;
                            objReporte.Llave = item.IdProveedor + item.IdTipoDocumentoSustentatorio + item.SerieDocSustentatorio + "-" + item.CorrelativoDocSustentatorio;
                            Totales = DocumentosRetencion.Where(o => o.v_IdDocumentoRetencion == Ncr.v_IdDocumentoRetencion).ToList();
                            objReporte.ImporteTotalDocumentoSustentatorio = Totales.Any() ? Totales.Sum(o => o.ImporteDocumentoSustentatorio) : 0;
                            ImporteCompra = 0;
                            Compra = TodasCompras.Where(p => p.IdTipoDocumentoSustentatorio == item.IdTipoDocumentoSustentatorio && p.SerieDocumentoCompra == item.SerieDocSustentatorio && p.CorrelativoDocumentoCompra == item.CorrelativoDocSustentatorio && p.IdProveedor == item.IdProveedor).ToList();
                            if (Compra.Any())
                            {
                                ImporteCompra = Compra.FirstOrDefault().ImporteDocumentoSustentatorio;
                            }
                            else
                            {
                                Compra = TodosAsientoApertura.Where(o => o.IdTipoDocumentoAnexo == item.IdTipoDocumentoSustentatorio && o.SerieDocumentoCompra == item.SerieDocSustentatorio && o.CorrelativoDocumentoCompra == item.CorrelativoDocSustentatorio && o.Anexo == item.IdProveedor).ToList();
                                if (Compra.Any())
                                {
                                    ImporteCompra = Compra.FirstOrDefault().ImporteDocumentoSustentatorio;
                                }
                            }
                            // Solo se toman en cuenta las notas de credito o debito menor a las fechas de pago
                            if (Ncr.dFechaTransaccion <= item.dFechaTransaccion)
                            {

                                Saldo = ImporteCompra + Ncr.Haber - Ncr.Debe;
                                objReporte.Saldo = Saldo;
                                Saldo = objReporte.Saldo;
                                ImporteCompra = Compra.FirstOrDefault().ImporteDocumentoSustentatorio;
                                ImporteCompra = Compra.Any() ? Compra.FirstOrDefault().ImporteDocumentoSustentatorio : 0;
                                objReporte.IdCompra = Compra.Any() ? Compra.FirstOrDefault().IdCompra : null;
                                objReporte.TipoDocumentoCompra = !Compra.Any() ? "" : Compra.FirstOrDefault().IdTipoDocumentoSustentatorio.ToString();
                                objReporte.SerieDocumentoCompra = !Compra.Any() ? "" : Compra.FirstOrDefault().SerieDocumentoCompra;
                                objReporte.CorrelativoDocumentoCompra = !Compra.Any() ? "" : Compra.FirstOrDefault().CorrelativoDocumentoCompra;
                                objReporte.ValorTotalDocumentoCompra = !Compra.Any() ? decimal.Parse("0.00") : Compra.FirstOrDefault().ImporteDocumentoSustentatorio;
                                objReporte.FechaEmisionDocumentoCompra = !Compra.Any() ? "" : Compra.FirstOrDefault().FechaTransaccion;
                                objReporte.NroTesoreria = "";   // IdTes.Any() ? IdTes.FirstOrDefault().NroTesoreria : ""; En la Nota de Crédito no debe aparecer el Numerode tesoreria;
                                objReporte.TipoDocSustentatorio = ""; // no debe aparecer el numero de la nota de credito
                                objReporte.NumeroDocumentoSustentatorio = "";
                                ListaNcr.Add(objReporte);
                            }
                            else
                            {

                                ListaNotaCreditoNoTenerCuenta.Add(objReporte);
                            }
                        }

                        objReporte = new ReporteRetenciones();
                        objReporte = item;
                        objReporte.FechaTransaccion = item.dFechaTransaccion.ToShortDateString();
                        objReporte.Llave = item.IdProveedor + item.IdTipoDocumentoSustentatorio + item.SerieDocSustentatorio + "-" + item.CorrelativoDocSustentatorio;
                        Totales = DocumentosRetencion.Where(o => o.v_IdDocumentoRetencion == item.v_IdDocumentoRetencion).ToList();
                        objReporte.ImporteTotalDocumentoSustentatorio = Totales.Any() ? Totales.Sum(o => o.ImporteDocumentoSustentatorio) : 0;
                        ImporteCompra = 0;
                        Compra = new List<ReporteRetenciones>();
                        Compra = TodasCompras.Where(p => p.IdTipoDocumentoSustentatorio == item.IdTipoDocumentoSustentatorio && p.SerieDocumentoCompra == item.SerieDocSustentatorio && p.CorrelativoDocumentoCompra == item.CorrelativoDocSustentatorio && p.IdProveedor == item.IdProveedor).ToList();
                        if (Compra.Any())
                        {
                            ImporteCompra = Compra.FirstOrDefault().ImporteDocumentoSustentatorio;

                        }
                        else
                        {
                            Compra = TodosAsientoApertura.Where(o => o.IdTipoDocumentoAnexo == item.IdTipoDocumentoSustentatorio && o.SerieDocumentoCompra == item.SerieDocSustentatorio && o.CorrelativoDocumentoCompra == item.CorrelativoDocSustentatorio && o.Anexo == item.IdProveedor).ToList();
                            if (Compra.Any())
                            {
                                ImporteCompra = Compra.FirstOrDefault().ImporteDocumentoSustentatorio;
                            }
                        }

                        if (LlaveAntigua == objReporte.Llave)
                        {
                            objReporte.Saldo = Saldo != 0 ? Saldo + item.Haber - item.Debe : (ImporteCompra + item.Haber - item.Debe);
                            Saldo = objReporte.Saldo;
                            Test = Test.Concat(ListaNcr).ToList();
                        }
                        else
                        {
                            var LlaveDiferenciaCambio = Test.LastOrDefault().Llave;
                            Test = Test.Concat(ListaNcr).ToList();
                            decimal SaldAnterior = 0;
                            try
                            {
                                SaldAnterior = Test.Where(o => o.Llave == LlaveDiferenciaCambio).OrderBy(o => o.dFechaTransaccion).LastOrDefault().Saldo;
                            }
                            catch (Exception ex)
                            {
                                SaldAnterior = 0;
                            }
                            var DiferenciaCambio = TodosDiferenciaCambio.Where(o => o.Llave == LlaveDiferenciaCambio).ToList().OrderBy(x => x.dFechaTransaccion).ToList();
                            var Diferen1 = TodosDiferenciaCambio.Where(o => o.LLaveReferencia == LlaveDiferenciaCambio).ToList();
                            var DiferenciaCambioRef = Diferen1.Where(o => !ListaNotaCreditoNoTenerCuenta.Select(p => p.Llave).Contains(o.LLaveReferencia)).ToList();
                            var DiferenciaCambioTotal = DiferenciaCambio.Concat(DiferenciaCambioRef).ToList();
                            foreach (var dc in DiferenciaCambioTotal)
                            {

                                ReporteRetenciones objDc = new ReporteRetenciones();
                                objDc = dc;
                                objDc.Saldo = SaldAnterior + dc.Haber - dc.Debe;
                                objDc.Llave = LlaveDiferenciaCambio;
                                Test.Add(objDc);
                                SaldAnterior = objDc.Saldo;

                            }
                            Saldo = 0;
                            objReporte.Saldo = Saldo != 0 ? Saldo + item.Haber - item.Debe : (ImporteCompra + item.Haber - item.Debe);
                            Saldo = objReporte.Saldo;


                        }

                        ImporteCompra = Compra.Any() ? Compra.FirstOrDefault().ImporteDocumentoSustentatorio : 0;
                        objReporte.IdCompra = Compra.Any() ? Compra.FirstOrDefault().IdCompra : null;
                        objReporte.TipoDocumentoCompra = !Compra.Any() ? "" : Compra.FirstOrDefault().IdTipoDocumentoSustentatorio.ToString();
                        objReporte.SerieDocumentoCompra = !Compra.Any() ? "" : Compra.FirstOrDefault().SerieDocumentoCompra;
                        objReporte.CorrelativoDocumentoCompra = !Compra.Any() ? "" : Compra.FirstOrDefault().CorrelativoDocumentoCompra;
                        objReporte.ValorTotalDocumentoCompra = !Compra.Any() ? decimal.Parse("0.00") : Compra.FirstOrDefault().ImporteDocumentoSustentatorio;
                        objReporte.FechaEmisionDocumentoCompra = !Compra.Any() ? "" : Compra.FirstOrDefault().FechaTransaccion;
                        IdTes = Tesorerias.Where(o => o.v_IdDocumentoRetencionDetalle == item.v_IdDocumentoRetencionDetalle).ToList();

                        if (!IdTes.Any())
                        {
                            IdTes = Tesorerias.Where(o => o.IdTipoDocumentoAnexo == item.IdTipoDocumentoSustentatorio && o.SerieDocumentoCompra == item.SerieDocSustentatorio && o.CorrelativoDocumentoCompra == item.CorrelativoDocSustentatorio && o.IdProveedor == item.IdProveedor).ToList();
                        }
                        objReporte.NroTesoreria = IdTes.Any() ? IdTes.FirstOrDefault().NroTesoreria : "";
                        LlaveAntigua = objReporte.Llave;
                        objReporte.NroCompra = "";
                        Test.Add(objReporte);
                        if (ContadorReg == NumeroRegistros)
                        {

                            var DiferenciaCambio = TodosDiferenciaCambio.Where(o => o.Llave == Test.LastOrDefault().Llave).ToList().OrderBy(x => x.dFechaTransaccion);
                            var DiferenciaCambioRef = TodosDiferenciaCambio.Where(o => o.LLaveReferencia == Test.LastOrDefault().Llave).ToList().OrderBy(x => x.dFechaTransaccion).Except(ListaNotaCreditoNoTenerCuenta).ToList();
                            var DiferenciaCambioTotal = DiferenciaCambio.Concat(DiferenciaCambioRef).ToList();
                            foreach (var dc in DiferenciaCambioTotal)
                            {
                                ReporteRetenciones objDc = new ReporteRetenciones();
                                objDc = dc;
                                objDc.Saldo = Saldo + dc.Haber - dc.Debe;  ///dc.Haber != 0 ? Saldo - dc.Haber : Saldo - dc.Debe;
                                if (!Test.Select(o => o.IdDiarioDetalle).ToList().Contains(dc.IdDiarioDetalle))
                                {
                                    objDc.Llave = Test.LastOrDefault().Llave;
                                    Test.Add(objDc);
                                    Saldo = objDc.Saldo;
                                }

                            }
                        }
                        ContadorReg++;

                    }
                    objOperationResult.Success = 1;
                    var ComprasContenidasenTesoreria = TodasCompras.Concat(TodosAsientoApertura).Where(o => o.IdTipoDocumentoSustentatorio != 7 && o.IdTipoDocumentoSustentatorio != 8).Where(o => Test.Select(p => p.IdCompra).Contains(o.IdCompra)).ToList();
                    var To = Test.Concat(ComprasContenidasenTesoreria).ToList();
                    var ReporteFinal = To.OrderBy(o => o.Llave).ThenBy(o => o.IdTipoTransaccion).ThenBy(o => o.dFechaTransaccion).ToList();
                    return ReporteFinal;

                }
            }

            catch (Exception ex)
            {


                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "TesoreriaBL.ReporteRetenciones()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }

        }



        #endregion

        #region Destinos
        public List<destinoDto> ObtenerDestinosPorCuenta(string strCuentaOrigen)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    return dbContext.destino.Where(p => p.v_CuentaOrigen == strCuentaOrigen
                        && p.i_Eliminado.HasValue && p.i_Eliminado.Value.Equals(0)).ToList().ToDTOs();
                }
            }
            catch (Exception ex)
            {
                var err = new StringBuilder();
                err.AppendLine(@"No se pudieron obtener los destinos de la cuenta.!");
                err.AppendLine(ex.Message);
                if (ex.InnerException != null)
                    err.AppendLine(ex.InnerException.Message);
                throw new Exception(err.ToString());
            }
        }
        #endregion

        #region Procesos Para Calcular los Destinos
        public List<tesoreriadetalleDto> ProcesaDestinosTesoreria(ref OperationResult pobjOperationResult, List<tesoreriadetalleDto> pTempInsertar, tesoreriaDto tesoreriaCabecera, List<destino> ptemDestinosDs)
        {
            var idTesoreria = tesoreriaCabecera.v_IdTesoreria;
            try
            {
                var listaRetorno = new List<tesoreriadetalleDto>();

                #region Generación de destinos
                foreach (var agrupadoCuenta in pTempInsertar.GroupBy(p => new { cuenta = p.v_NroCuenta, cc = p.i_IdCentroCostos, naturaleza = p.v_Naturaleza }))
                {
                    var filaProcesar = agrupadoCuenta;
                    var destinosFila = ptemDestinosDs
                        .Where(p => p.v_CuentaOrigen.Equals(filaProcesar.Key.cuenta) && p.i_Eliminado == 0 && p.v_Periodo.Equals(periodo)).ToList();

                    if (!destinosFila.Any()) continue;
                    {
                        var preListaRetorno = new List<tesoreriadetalleDto>();
                        var importeOrigen = agrupadoCuenta.Sum(p => p.d_Importe ?? 0);
                        var cambioOrigen = agrupadoCuenta.Sum(p => p.d_Cambio ?? 0);
                        foreach (var destino in destinosFila)
                        {
                            var filaOrigenModelo = agrupadoCuenta.FirstOrDefault();
                            if (filaOrigenModelo == null) continue;

                            var porcentajeDestino = destino.i_Porcentaje ?? 100;
                            var importeDestino = Utils.Windows.DevuelveValorRedondeado((importeOrigen * porcentajeDestino) / 100, 2);
                            var cambioDestino = Utils.Windows.DevuelveValorRedondeado(cambioOrigen > 0 ? (cambioOrigen * porcentajeDestino) / 100 : 0, 2);

                            #region Preparacion de pre-lista
                            preListaRetorno.Add(new tesoreriadetalleDto
                            {
                                d_Importe = importeDestino,
                                d_Cambio = cambioDestino,
                                i_IdCentroCostos = agrupadoCuenta.Key.cc,
                                i_IdTipoDocumento = -1,
                                i_EsDestino = "1",
                                v_Naturaleza = agrupadoCuenta.Key.naturaleza.Trim(),
                                v_NroCuenta = destino.v_CuentaDestino,
                                v_NroDocumento = string.Empty,
                                t_Fecha = filaOrigenModelo.t_Fecha,
                                i_IdTipoDocumentoRef = -1,
                                v_NroDocumentoRef = string.Empty,
                                NroCuentaOrigen = destino.v_CuentaOrigen
                            });

                            preListaRetorno.Add(new tesoreriadetalleDto
                            {
                                d_Importe = importeDestino,
                                d_Cambio = cambioDestino,
                                i_IdCentroCostos = agrupadoCuenta.Key.cc,
                                i_IdTipoDocumento = -1,
                                i_EsDestino = "1",
                                v_Naturaleza = agrupadoCuenta.Key.naturaleza.Trim().Equals("D") ? "H" : "D",
                                v_NroCuenta = destino.v_CuentaTransferencia,
                                v_NroDocumento = string.Empty,
                                t_Fecha = filaOrigenModelo.t_Fecha,
                                i_IdTipoDocumentoRef = -1,
                                v_NroDocumentoRef = string.Empty,
                                NroCuentaOrigen = destino.v_CuentaOrigen
                            });
                            #endregion
                        }

                        #region Cuadre de pre-lista
                        var sumaImportePreListaD = preListaRetorno.Where(o => o.v_Naturaleza.Equals("D")).Sum(p => p.d_Importe ?? 0);
                        if (sumaImportePreListaD != importeOrigen)
                        {
                            var diferencia = sumaImportePreListaD - importeOrigen;
                            preListaRetorno.First(p => p.v_Naturaleza.Equals("D")).d_Importe -= diferencia;
                        }

                        var sumaImportePreListaH = preListaRetorno.Where(o => o.v_Naturaleza.Equals("H")).Sum(p => p.d_Importe ?? 0);
                        if (sumaImportePreListaH != importeOrigen)
                        {
                            var diferencia = sumaImportePreListaH - importeOrigen;
                            preListaRetorno.First(p => p.v_Naturaleza.Equals("H")).d_Importe -= diferencia;
                        }

                        var sumaCambioPreListaD = preListaRetorno.Where(o => o.v_Naturaleza.Equals("D")).Sum(p => p.d_Cambio ?? 0);
                        if (sumaCambioPreListaD != cambioOrigen)
                        {
                            var diferencia = sumaCambioPreListaD - cambioOrigen;
                            preListaRetorno.First(p => p.v_Naturaleza.Equals("D")).d_Cambio -= diferencia;
                        }

                        var sumaCambioPreListaH = preListaRetorno.Where(o => o.v_Naturaleza.Equals("H")).Sum(p => p.d_Cambio ?? 0);
                        if (sumaCambioPreListaH != cambioOrigen)
                        {
                            var diferencia = sumaCambioPreListaH - cambioOrigen;
                            preListaRetorno.First(p => p.v_Naturaleza.Equals("H")).d_Cambio -= diferencia;
                        }
                        #endregion

                        listaRetorno.AddRange(preListaRetorno);
                    }
                }
                #endregion

                pobjOperationResult.Success = 1;
                return listaRetorno;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "DiarioBL.ProcesaDestinosTesoreria() \nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) + '\n' + idTesoreria;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public void CuadrarTesoreria(ref OperationResult pobjOperationResult, string pstrTesoreriaId)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var Tesorerias = dbContext.tesoreria.Where(p => p.v_IdTesoreria.Equals(pstrTesoreriaId)).ToList(); // No se toma en cuenta DImportacion

                        foreach (var Tesoreria in Tesorerias.AsParallel())
                        {
                            var oTesoreriaDetalle = new List<tesoreriadetalle>();
                            foreach (var TesoreriaDetalle in Tesoreria.tesoreriadetalle.Where(p => p.i_Eliminado == 0))
                            {
                                TesoreriaDetalle.d_Cambio = Tesoreria.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(TesoreriaDetalle.d_Importe.Value / Tesoreria.d_TipoCambio.Value, 2) : Utils.Windows.DevuelveValorRedondeado(TesoreriaDetalle.d_Importe.Value * Tesoreria.d_TipoCambio.Value, 2);
                                oTesoreriaDetalle.Add(TesoreriaDetalle);
                            }

                            #region Revisa Cuadre Cambios
                            //Revisa que la suma de los cambios del H cuadre con la suma de los cambios del D, 
                            //si no cuadra agarra el ultimo de los H y le suma/resta la diferencia
                            decimal? d_TotalDebe = oTesoreriaDetalle.Where(p => p.v_Naturaleza == "D").Sum(p => p.d_Importe).Value;
                            decimal? d_TotalHaber = oTesoreriaDetalle.Where(p => p.v_Naturaleza == "H").Sum(p => p.d_Importe).Value;
                            decimal? d_TotalHaberCambio = oTesoreriaDetalle.Where(p => p.v_Naturaleza == "H").Sum(p => p.d_Cambio).Value;
                            decimal? d_TotalDebeCambio = oTesoreriaDetalle.Where(p => p.v_Naturaleza == "D").Sum(p => p.d_Cambio).Value;
                            decimal? d_DiferenciaDebe = d_TotalDebe.Value - d_TotalHaber.Value;
                            decimal? d_DiferenciaHaber = d_TotalDebeCambio.Value - d_TotalHaberCambio.Value;

                            if (d_DiferenciaDebe != d_DiferenciaHaber)
                            {
                                decimal? Diferencia = d_DiferenciaHaber - d_DiferenciaDebe;
                                var UltimaFilaH = oTesoreriaDetalle.Last(p => p.v_Naturaleza == "H" && p.i_EsDestino != "1");
                                UltimaFilaH.d_Cambio = UltimaFilaH.d_Cambio + Diferencia;

                                Tesoreria.d_TotalDebe_Importe = oTesoreriaDetalle.Where(p => p.v_Naturaleza == "D").Sum(p => p.d_Importe).Value;
                                Tesoreria.d_TotalHaber_Importe = oTesoreriaDetalle.Where(p => p.v_Naturaleza == "H").Sum(p => p.d_Importe).Value;
                                Tesoreria.d_TotalDebe_Cambio = oTesoreriaDetalle.Where(p => p.v_Naturaleza == "D").Sum(p => p.d_Cambio).Value;
                                Tesoreria.d_TotalHaber_Cambio = oTesoreriaDetalle.Where(p => p.v_Naturaleza == "H").Sum(p => p.d_Cambio).Value;
                                Tesoreria.d_Diferencia_Importe = d_TotalDebe.Value - d_TotalHaber.Value;
                                Tesoreria.d_Diferencia_Cambio = d_TotalDebeCambio.Value - d_TotalHaberCambio.Value;
                                dbContext.tesoreria.ApplyCurrentValues(Tesoreria);
                                dbContext.SaveChanges();
                                if (pobjOperationResult.Success == 0) return;
                            }
                            #endregion


                            var TotDebe = oTesoreriaDetalle.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Importe ?? 0);
                            var TotHaber = oTesoreriaDetalle.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Importe ?? 0);
                            var TotDebeC = oTesoreriaDetalle.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Cambio ?? 0);
                            var TotHaberC = oTesoreriaDetalle.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Cambio ?? 0);

                            Tesoreria.d_TotalDebe_Importe = decimal.Parse(Math.Round(TotDebe, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                            Tesoreria.d_TotalHaber_Importe = decimal.Parse(Math.Round(TotHaber, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                            Tesoreria.d_TotalDebe_Cambio = decimal.Parse(Math.Round(TotDebeC, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                            Tesoreria.d_TotalHaber_Cambio = decimal.Parse(Math.Round(TotHaberC, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                            Tesoreria.d_Diferencia_Importe = decimal.Parse(Math.Round(TotDebe - TotHaber, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                            Tesoreria.d_Diferencia_Cambio = decimal.Parse(Math.Round(TotDebeC - TotHaberC, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                            dbContext.tesoreria.ApplyCurrentValues(Tesoreria);
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
                pobjOperationResult.AdditionalInformation = "DiarioBL.CuadrarAsientos() \nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }

        public void RecalcularDestinosTesoreria(ref OperationResult pobjOperationResult, int Periodo, int mes)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {

                        pobjOperationResult.Success = 1;
                        var objSecuentialBL = new SecuentialBL();

                        var tesorerias = dbContext.tesoreria.Where(p => p.t_FechaRegistro.Value.Year == Periodo
                            && p.i_Eliminado == 0 && (mes == -1 || p.t_FechaRegistro.Value.Month == mes)).ToList();

                        Globals.ProgressbarStatus.i_TotalProgress = tesorerias.Count();

                        var ids = tesorerias.Select(p => p.v_IdTesoreria).ToList();

                        var destinosAnteriores =
                               dbContext.tesoreriadetalle.Where(p => ids.Contains(p.v_IdTesoreria) && p.i_EsDestino.Equals("1"));

                        foreach (var destino in destinosAnteriores)
                        {
                            dbContext.DeleteObject(destino);
                        }
                        dbContext.SaveChanges();

                        foreach (var tesoreria in tesorerias.AsParallel())
                        {
                            var pTemp_Insertar = tesoreria.tesoreriadetalle.Where(p => p.i_Eliminado == 0).ToList().ToDTOs();
                            var destinos = dbContext.destino.Where(p => p.i_Eliminado == 0).ToList();
                            var DestinosXIngresar = ProcesaDestinosTesoreria(ref pobjOperationResult, pTemp_Insertar, tesoreria.ToDTO(), destinos);
                            if (pobjOperationResult.Success == 0) break;

                            #region Inserta Destinos ReCalculados
                            foreach (var diariodetalleDto in DestinosXIngresar)
                            {
                                var objEntitytesoreriaDetalle = diariodetalleDto.ToEntity();
                                var SecuentialId = objSecuentialBL.GetNextSecuentialId(int.Parse(Globals.ClientSession.GetAsList()[0]), 60);
                                var newIdDiarioDetalle = Utils.GetNewId(int.Parse(Globals.ClientSession.GetAsList()[0]), SecuentialId, "XJ");
                                objEntitytesoreriaDetalle.v_IdTesoreriaDetalle = newIdDiarioDetalle;
                                objEntitytesoreriaDetalle.v_IdTesoreria = tesoreria.v_IdTesoreria;
                                objEntitytesoreriaDetalle.t_InsertaFecha = DateTime.Now;
                                objEntitytesoreriaDetalle.i_InsertaIdUsuario = Int32.Parse(Globals.ClientSession.GetAsList()[2]);
                                objEntitytesoreriaDetalle.i_Eliminado = 0;
                                dbContext.AddTotesoreriadetalle(objEntitytesoreriaDetalle);
                            }
                            #endregion
                            Globals.ProgressbarStatus.i_Progress++;
                        }

                        if (pobjOperationResult.Success == 0) return;
                        dbContext.SaveChanges();
                        pobjOperationResult.Success = 1;
                        ts.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "TesoreriaBL.RecalcularDestinosTesoreria() \nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }
        #endregion

    }
}
