using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Text;
using System.Transactions;
using SAMBHS.Common.BE.Custom;
using System.Threading.Tasks;


namespace SAMBHS.Tesoreria.BL
{
    public class DiarioBL
    {
        private string periodo = Globals.ClientSession.i_Periodo.Value.ToString();
        int error = 1;
        public List<KeyValueDTO> ObtenerListadoDiario(ref OperationResult pobjOperationResult, string pstringPeriodo, string pstringMes, int IdTipoDocumento)
        {
            using (var dbcontext = new SAMBHSEntitiesModelWin())
            {
                var almacenpredeterminado = Globals.ClientSession.i_IdAlmacenPredeterminado.Value.ToString("00");
                var replicationID = Globals.ClientSession.ReplicationNodeID;
                var query = (from n in dbcontext.diario
                             where n.i_Eliminado == 0 && n.v_Periodo == pstringPeriodo && n.v_Mes == pstringMes && n.i_IdTipoDocumento == IdTipoDocumento && n.v_IdDiario.Substring(2, 2) == almacenpredeterminado
                                   && n.v_IdDiario.Substring(0, 1) == replicationID
                             orderby n.v_Correlativo ascending
                             select new
                             {
                                 v_Correlativo = n.v_Correlativo,
                                 v_IdDiario = n.v_IdDiario
                             }
                );
                if (query.Any())
                {
                    var query2 = query.AsEnumerable()
                        .Select(x => new KeyValueDTO
                        {
                            Value1 = x.v_Correlativo,
                            Value2 = x.v_IdDiario
                        }).ToList();
                    return query2;
                }

                return new List<KeyValueDTO> { new KeyValueDTO { Value1 = almacenpredeterminado + "000000" } };
            }
        }
        public List<KeyValueDTO> ObtenerListadoDiarioTipoDocumentoyReferencia(ref OperationResult pobjOperationResult, string pstringPeriodo, int IdTipoDocumento, string DocumentoReferencia)
        {
            using (var dbcontext = new SAMBHSEntitiesModelWin())
            {
                var almacenpredeterminado = Globals.ClientSession.i_IdAlmacenPredeterminado.Value.ToString("00");
                var replicationID = Globals.ClientSession.ReplicationNodeID;
                var query = (from n in dbcontext.diario
                             where n.i_Eliminado == 0 && n.v_Periodo == pstringPeriodo && n.i_IdTipoDocumento == IdTipoDocumento
                                   && n.v_IdDocumentoReferencia == null
                             orderby n.v_Correlativo ascending
                             select new
                             {
                                 v_Correlativo = n.v_Correlativo,
                                 v_IdDiario = n.v_IdDiario
                             }
                );
                if (query.Any())
                {
                    var query2 = query.AsEnumerable()
                        .Select(x => new KeyValueDTO
                        {
                            Value1 = x.v_Correlativo,
                            Value2 = x.v_IdDiario
                        }).ToList();
                    return query2;
                }
                return null;
            }
            //return new List<KeyValueDTO> { new KeyValueDTO { Value1 = almacenpredeterminado + "000000" } };
        }
        public List<KeyValueDTO> ObtenerListadoDiarioParaAsientoCierre(ref OperationResult pobjOperationResult, string pstringPeriodo, string pstringMes, int IdTipoDocumento)
        {
            using (var dbcontext = new SAMBHSEntitiesModelWin())
            {
                var query = (from n in dbcontext.diario
                             where n.i_Eliminado == 0 && n.v_Periodo == pstringPeriodo && n.v_Mes == pstringMes && n.i_IdTipoDocumento == IdTipoDocumento
                                   && n.i_IdTipoComprobante != 5
                             orderby n.v_Correlativo ascending
                             select new
                             {
                                 n.v_Correlativo,
                                 n.v_IdDiario
                             }
                );
                if (query.Any())
                {
                    var query2 = query.AsEnumerable()
                        .Select(x => new KeyValueDTO
                        {
                            Value1 = x.v_Correlativo,
                            Value2 = x.v_IdDiario
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

        public List<KeyValueDTO> ObtenerListadoDiarioParaAsientoApertura(ref OperationResult pobjOperationResult, string pstringPeriodo, string pstringMes, int IdTipoDocumento)
        {
            using (var dbcontext = new SAMBHSEntitiesModelWin())
            {
                var query = (from n in dbcontext.diario
                             where n.i_Eliminado == 0 && n.v_Periodo == pstringPeriodo && n.v_Mes == pstringMes && n.i_IdTipoDocumento == IdTipoDocumento
                                   && n.i_IdTipoComprobante != 1
                             orderby n.v_Correlativo ascending
                             select new
                             {
                                 n.v_Correlativo,
                                 n.v_IdDiario
                             });
                if (query.Any())
                {
                    var query2 = query.AsEnumerable()
                        .Select(x => new KeyValueDTO
                        {
                            Value1 = x.v_Correlativo,
                            Value2 = x.v_IdDiario
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

        public diarioDto ObtenerDiarioCabecera(ref OperationResult pobjOperationResult, string pstrIdTesoreria)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var objEntity = (from a in dbContext.diario
                                     where a.v_IdDiario == pstrIdTesoreria
                                     select a).FirstOrDefault();

                    pobjOperationResult.Success = 1;
                    return diarioAssembler.ToDTO(objEntity);
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public BindingList<diariodetalleDto> ObtenerDiarioDetalles(ref OperationResult pobjOperationResult, string pstrIdTesoreria)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from n in dbContext.diariodetalle

                                 join J1 in dbContext.cliente on n.v_IdCliente equals J1.v_IdCliente into J1_join
                                 from J1 in J1_join.DefaultIfEmpty()

                                 where n.i_Eliminado == 0 && n.v_IdDiario == pstrIdTesoreria
                                 orderby n.t_InsertaFecha ascending
                                 select new diariodetalleDto
                                 {
                                     d_Cambio = n.d_Cambio,
                                     d_Importe = n.d_Importe,
                                     i_EsDestino = n.i_EsDestino,
                                     i_Eliminado = n.i_Eliminado,
                                     i_IdTipoDocumento = n.i_IdTipoDocumento,
                                     i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                     t_ActualizaFecha = n.t_ActualizaFecha,
                                     i_IdCentroCostos = n.i_IdCentroCostos,
                                     i_IdTipoDocumentoRef = n.i_IdTipoDocumentoRef,
                                     i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                     t_InsertaFecha = n.t_InsertaFecha,
                                     v_IdCliente = n.v_IdCliente,
                                     v_Analisis = n.v_Analisis,
                                     v_IdDiario = n.v_IdDiario,
                                     v_IdDiarioDetalle = n.v_IdDiarioDetalle,
                                     v_Naturaleza = n.v_Naturaleza,
                                     v_NroCuenta = n.v_NroCuenta,
                                     v_NroDocumento = n.v_NroDocumento,
                                     v_NroDocumentoRef = n.v_NroDocumentoRef,
                                     v_OrigenDestino = n.v_OrigenDestino,
                                     v_Pedido = n.v_Pedido,
                                     t_Fecha = n.t_Fecha,
                                     v_CodAnexo = J1.v_CodCliente,
                                     i_IdPatrimonioNeto = n.i_IdPatrimonioNeto,
                                 }
                                         ).ToList();

                    pobjOperationResult.Success = 1;

                    return new BindingList<diariodetalleDto>(query);
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public void InsertarDiario(ref OperationResult pobjOperationResult, diarioDto pobjDtoEntity, List<string> ClientSession, List<diariodetalleDto> pTemp_Insertar, int FlagTipoMovimiento, bool CambiarUsuario = false)
        {
            var nroRegistro = string.Empty;
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        SecuentialBL objSecuentialBL = new SecuentialBL();
                        diario objEntitytesoreria = pobjDtoEntity.ToEntity();

                        int SecuentialId = 0;
                        string newIdDiario = string.Empty;
                        string newIdDiarioDetalle = string.Empty;

                        //var descuadre = pTemp_Insertar.Where(p => p.v_Naturaleza.Equals("D")).Sum(s => s.d_Importe ?? 0) - pTemp_Insertar.Where(p => p.v_Naturaleza.Equals("H")).Sum(s => s.d_Importe ?? 0);
                        //if (descuadre != 0)
                        //    throw new Exception("El asiento generado está descuadrado por " + descuadre);

                        #region Inserta Cabecera

                        if (!CambiarUsuario)
                        {
                            objEntitytesoreria.t_InsertaFecha = DateTime.Now;
                            objEntitytesoreria.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        }

                        objEntitytesoreria.i_Eliminado = 0;

                        var intNodeId = int.Parse(ClientSession[0]);
                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 59);
                        newIdDiario = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "XI");
                        objEntitytesoreria.v_IdDiario = newIdDiario;
                        dbContext.AddTodiario(objEntitytesoreria);
                        dbContext.SaveChanges();
                        nroRegistro = objEntitytesoreria.v_Mes.Trim() + "-" + objEntitytesoreria.v_Correlativo.Trim();
                        #endregion

                        #region Revisa Cuadre Cambios
                        //Revisa que la suma de los cambios del H cuadre con la suma de los cambios del D, 
                        //si no cuadra agarra el ultimo de los H y le suma/resta la diferencia
                        decimal? d_TotalDebe = pTemp_Insertar.Where(p => p.v_Naturaleza == "D").Sum(p => p.d_Importe).Value;
                        decimal? d_TotalHaber = pTemp_Insertar.Where(p => p.v_Naturaleza == "H").Sum(p => p.d_Importe).Value;
                        decimal? d_TotalHaberCambio = pTemp_Insertar.Where(p => p.v_Naturaleza == "H").Sum(p => p.d_Cambio).Value;
                        decimal? d_TotalDebeCambio = pTemp_Insertar.Where(p => p.v_Naturaleza == "D").Sum(p => p.d_Cambio).Value;
                        decimal? d_DiferenciaDebe = d_TotalDebe.Value - d_TotalHaber.Value;
                        decimal? d_DiferenciaHaber = d_TotalDebeCambio.Value - d_TotalHaberCambio.Value;

                        if (d_DiferenciaDebe != d_DiferenciaHaber)
                        {
                            var Diferencia = d_DiferenciaHaber - d_DiferenciaDebe;
                            var UltimaFilaH = pTemp_Insertar.Last(p => p.v_Naturaleza.Trim() == "H" && p.i_EsDestino != "1");
                            UltimaFilaH.d_Cambio = UltimaFilaH.d_Cambio + Diferencia;

                            var Cabecera = dbContext.diario.FirstOrDefault(p => p.v_IdDiario == newIdDiario);
                            Cabecera.d_TotalDebe = pTemp_Insertar.Where(p => p.v_Naturaleza == "D").Sum(p => p.d_Importe).Value;
                            Cabecera.d_TotalHaber = pTemp_Insertar.Where(p => p.v_Naturaleza == "H").Sum(p => p.d_Importe).Value;
                            Cabecera.d_TotalDebeCambio = pTemp_Insertar.Where(p => p.v_Naturaleza == "D").Sum(p => p.d_Cambio).Value;
                            Cabecera.d_TotalHaberCambio = pTemp_Insertar.Where(p => p.v_Naturaleza == "H").Sum(p => p.d_Cambio).Value;
                            Cabecera.d_DiferenciaDebe = d_TotalDebe.Value - d_TotalHaber.Value;
                            Cabecera.d_DiferenciaHaber = d_TotalDebeCambio.Value - d_TotalHaberCambio.Value;
                            dbContext.diario.ApplyCurrentValues(Cabecera);
                        }
                        #endregion



                        #region Inserta los destinos al detalle si es necesario
                        var destinosAnteriores =
                              dbContext.diariodetalle.Where(p => p.v_IdDiario.Equals(newIdDiario) && p.i_EsDestino.Equals("1"));

                        foreach (var destino in destinosAnteriores)
                        {
                            dbContext.DeleteObject(destino);
                        }

                        var destinos = dbContext.destino.Where(p => p.i_Eliminado == 0 && p.v_Periodo == periodo).ToList();
                        pTemp_Insertar = pTemp_Insertar.Concat(ProcesaDestinosDiario(ref pobjOperationResult, pTemp_Insertar, objEntitytesoreria.ToDTO(), destinos)).ToList();
                        if (pobjOperationResult.Success == 0) return;
                        #endregion

                        #region Inserta Detalle

                        foreach (var diariodetalleDto in pTemp_Insertar)
                        {
                            if (diariodetalleDto.d_Importe == 0) continue;
                            diariodetalle objEntitytesoreriaDetalle = diariodetalleDto.ToEntity();
                            SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 60);
                            newIdDiarioDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "XJ");
                            objEntitytesoreriaDetalle.v_IdDiarioDetalle = newIdDiarioDetalle;
                            objEntitytesoreriaDetalle.v_IdDiario = newIdDiario;
                            objEntitytesoreriaDetalle.t_InsertaFecha = DateTime.Now;
                            objEntitytesoreriaDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                            objEntitytesoreriaDetalle.i_Eliminado = 0;
                            if (objEntitytesoreriaDetalle.d_Importe < 0)
                                throw new Exception("No se permiten cantidades negativas en el registro de diario");

                            if (!dbContext.asientocontable.Any(p => p.v_NroCuenta == objEntitytesoreriaDetalle.v_NroCuenta && p.v_Periodo == periodo))
                                throw new NullReferenceException(string.Format("Cuenta no existe en el plan contable: {0}",
                                   objEntitytesoreriaDetalle.v_NroCuenta));

                            dbContext.AddTodiariodetalle(objEntitytesoreriaDetalle);
                            Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "diariodetalle", newIdDiarioDetalle);

                            error = error + 1;

                        }
                        #endregion

                        #region Historial
                        dbContext.SaveChanges();
                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "diario", newIdDiario);
                        #endregion

                        pobjOperationResult.Success = 1;
                        ts.Complete();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "DiarioBL.InsertarDiario() \nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) + '\n' + nroRegistro + "Error en :" + error.ToString();
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }

        public void ActualizarDiario(ref OperationResult pobjOperationResult, diarioDto pobjDtoEntity, List<string> ClientSession, List<diariodetalleDto> pTemp_Insertar, List<diariodetalleDto> pTemp_Editar, List<diariodetalleDto> pTemp_Eliminar, int FlagTipoMovimiento)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    SecuentialBL objSecuentialBL = new SecuentialBL();
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                    string newIdtesoreriaDetalle = string.Empty;

                    #region Actualiza Cabecera
                    var intNodeId = int.Parse(ClientSession[0]);
                    var objEntitySource = (dbContext.diario.Where(a => a.v_IdDiario == pobjDtoEntity.v_IdDiario)).FirstOrDefault();

                    pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                    pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                    //se hizo para que el calculo de los destinos sea correcto.
                    var existentesDetalle = objEntitySource.diariodetalle.Where(p => p.i_Eliminado == 0 &&
                                !pTemp_Editar.Select(o => o.v_IdDiarioDetalle).Contains(p.v_IdDiarioDetalle)).ToList().ToDTOs();

                    var objEntity = pobjDtoEntity.ToEntity();
                    dbContext.diario.ApplyCurrentValues(objEntity);
                    #endregion

                    #region Procesa los Destinos del diario
                    var destinosAnteriores = dbContext.diariodetalle
                        .Where(p => p.v_IdDiario.Equals(objEntitySource.v_IdDiario) && p.i_EsDestino.Equals("1"));

                    foreach (var destino in destinosAnteriores)
                    {
                        dbContext.DeleteObject(destino);
                    }

                    var destinos = dbContext.destino.Where(p => p.i_Eliminado == 0).ToList();
                    pTemp_Insertar = pTemp_Insertar.Concat(ProcesaDestinosDiario(ref pobjOperationResult, pTemp_Insertar.Concat(pTemp_Editar)
                        .Concat(existentesDetalle).ToList(), objEntitySource.ToDTO(), destinos)).ToList();
                    if (pobjOperationResult.Success == 0) return;
                    #endregion

                    #region Actualiza Detalle
                    foreach (var diariodetalleDto in pTemp_Insertar)
                    {
                        if (diariodetalleDto.d_Importe == 0) continue;
                        diariodetalle objEntitytesoreriaDetalle = diariodetalleDto.ToEntity();
                        var SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 60);
                        newIdtesoreriaDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "XJ");
                        objEntitytesoreriaDetalle.v_IdDiarioDetalle = newIdtesoreriaDetalle;
                        objEntitytesoreriaDetalle.t_InsertaFecha = DateTime.Now;
                        objEntitytesoreriaDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntitytesoreriaDetalle.i_Eliminado = 0;
                        objEntitytesoreriaDetalle.v_IdDiario = objEntitySource.v_IdDiario;
                        if (objEntitytesoreriaDetalle.d_Importe < 0)
                            throw new Exception("No se permiten cantidades negativas en el registro de diario");

                        dbContext.AddTodiariodetalle(objEntitytesoreriaDetalle);

                        if (!dbContext.asientocontable.Any(p => p.v_NroCuenta == objEntitytesoreriaDetalle.v_NroCuenta && p.v_Periodo == periodo))
                            throw new NullReferenceException(string.Format("Cuenta no existe en el plan contable: {0}",
                               objEntitytesoreriaDetalle.v_NroCuenta));

                        dbContext.SaveChanges();
                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "diariodetalle", newIdtesoreriaDetalle);
                        //objSaldoContableBL.ActualizarSaldoContable(ref pobjOperationResult, ListaProcesos.Diario, pobjDtoEntity.t_Fecha.Value, pobjDtoEntity.i_IdMoneda.Value, diariodetalleDto, RecordStatus.Agregado, newIdtesoreriaDetalle.Substring(0, 1));
                        //if (pobjOperationResult.Success == 0) return;
                    }

                    foreach (diariodetalleDto diariodetalleDto in pTemp_Editar.Where(p => p.i_EsDestino == null))
                    {
                        if (diariodetalleDto.d_Importe == 0) continue;
                        diariodetalle _objEntity = diariodetalleDto.ToEntity();
                        var query = (from n in dbContext.diariodetalle
                                     where n.v_IdDiarioDetalle == diariodetalleDto.v_IdDiarioDetalle
                                     select n).FirstOrDefault();

                        _objEntity.t_ActualizaFecha = DateTime.Now;
                        _objEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                        dbContext.diariodetalle.ApplyCurrentValues(_objEntity);
                        Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "diariodetalle", query.v_IdDiarioDetalle);
                        //objSaldoContableBL.ActualizarSaldoContable(ref pobjOperationResult, ListaProcesos.Diario, pobjDtoEntity.t_Fecha.Value, pobjDtoEntity.i_IdMoneda.Value, diariodetalleDto, RecordStatus.Modificado, query.v_IdDiarioDetalle.Substring(0, 1), pobjDtoEntity.i_IdMoneda.Value == 1 ? query.d_Importe.Value : query.d_Cambio.Value, pobjDtoEntity.i_IdMoneda.Value == 2 ? query.d_Importe.Value : query.d_Cambio.Value);
                        //if (pobjOperationResult.Success == 0) return;
                    }

                    foreach (diariodetalleDto diariodetalleDto in pTemp_Eliminar)
                    {
                        diariodetalle _objEntity = diariodetalleDto.ToEntity();
                        var query = (from n in dbContext.diariodetalle
                                     where n.v_IdDiarioDetalle == diariodetalleDto.v_IdDiarioDetalle
                                     select n).FirstOrDefault();

                        if (query != null && query.EntityState != System.Data.EntityState.Deleted)
                        {
                            query.t_ActualizaFecha = DateTime.Now;
                            query.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            query.i_Eliminado = 1;
                            dbContext.diariodetalle.ApplyCurrentValues(query);
                            Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "diariodetalle", query.v_IdDiarioDetalle);
                            //objSaldoContableBL.ActualizarSaldoContable(ref pobjOperationResult, ListaProcesos.Diario, pobjDtoEntity.t_Fecha.Value, pobjDtoEntity.i_IdMoneda.Value, diariodetalleDto, RecordStatus.EliminadoLogico, query.v_IdDiarioDetalle.Substring(0, 1));
                            //if (pobjOperationResult.Success == 0) return;
                        }
                    }
                    #endregion

                    dbContext.SaveChanges();
                    Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "diario", pobjDtoEntity.v_IdDiario);
                    //#region Elimina Pendientes Por Cobrar
                    //RestablecePendientesXCobrar(ref pobjOperationResult, objEntitySource.v_IdDiario);
                    //if (pobjOperationResult.Success == 0) return;
                    //#endregion
                    //#region Genera Pendiente Por Cobrar/Pagar
                    //new PendientesBL().InsertarPendientePorDiario(ref pobjOperationResult, objEntitySource.v_IdDiario, ClientSession, FlagTipoMovimiento);
                    //if (pobjOperationResult.Success == 0) return;
                    //#endregion
                    pobjOperationResult.Success = 1;
                    ts.Complete();
                    return;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "DiarioBL.ActualizarDiario()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }

        public void EliminarDiario(ref OperationResult pobjOperationResult, string pstrIdDiario, List<string> ClientSession)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                    SaldoContableBL objSaldoContableBL = new SaldoContableBL();

                    #region Elimina Cabecera
                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.diario
                                           where a.v_IdDiario == pstrIdDiario
                                           select a).FirstOrDefault();

                    // Crear la entidad con los datos actualizados
                    objEntitySource.t_ActualizaFecha = DateTime.Now;
                    objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntitySource.i_Eliminado = 1;
                    #endregion

                    #region Elimina Detalles
                    //Eliminar detalles del movimiento eliminado.
                    var objEntitySourceDetallesDiarios = (from a in dbContext.diariodetalle
                                                          where a.v_IdDiario == pstrIdDiario && a.i_Eliminado == 0
                                                          select a).ToList();

                    foreach (var diariodetalleDto in objEntitySourceDetallesDiarios)
                    {
                        diariodetalleDto.t_ActualizaFecha = DateTime.Now;
                        diariodetalleDto.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        diariodetalleDto.i_Eliminado = 1;
                        Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "diariodetalle", diariodetalleDto.v_IdDiarioDetalle);
                        //objSaldoContableBL.ActualizarSaldoContable(ref pobjOperationResult, ListaProcesos.Diario, objEntitySource.t_Fecha.Value, objEntitySource.i_IdMoneda.Value, diariodetalleAssembler.ToDTO(diariodetalleDto), RecordStatus.EliminadoLogico, diariodetalleDto.v_IdDiarioDetalle.Substring(0, 1));
                    }
                    #endregion

                    #region Elimina Pendientes Por Cobrar
                    RestablecePendientesXCobrar(ref pobjOperationResult, objEntitySource.v_IdDiario);
                    if (pobjOperationResult.Success == 0) return;
                    #endregion

                    dbContext.SaveChanges();

                    Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "diario", pstrIdDiario);

                    pobjOperationResult.Success = 1;
                    ts.Complete();
                    return;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "DiarioBL.EliminarDiario()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
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
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public string[] DevuelveCuentaCajaBanco(int pintIdDocumento)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                var Datos = (from a in dbContext.documento

                             join J1 in dbContext.asientocontable on new { c = a.v_NroCuenta, eliminado = 0, per = periodo } equals new { c = J1.v_NroCuenta, eliminado = J1.i_Eliminado.Value, per = J1.v_Periodo } into J1_join
                             from J1 in J1_join.DefaultIfEmpty()

                             where a.i_CodigoDocumento == pintIdDocumento
                             select new { a.v_NroCuenta, a.v_Nombre }).FirstOrDefault();

                string[] Cadena = new string[2];

                if (Datos != null)
                {
                    Cadena[0] = Datos.v_NroCuenta.Trim();
                    Cadena[1] = Datos.v_Nombre.Trim();
                    return Cadena;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool ExisteNroRegistro(string Periodo, string Mes, string Correlativo, int idTipoDocumento)
        {
            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
            var replicationID = Globals.ClientSession.ReplicationNodeID;
            var Registro = (from n in dbContext.diario
                            where n.i_Eliminado == 0 && n.v_Periodo == Periodo && n.v_Mes == Mes && n.v_Correlativo == Correlativo && n.i_IdTipoDocumento == idTipoDocumento
                            && n.v_IdDiario.Substring(0, 1) == replicationID
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

        public string[] EliminarDiarioXDocRef(ref OperationResult pobjOperationResult, string pstrIdDocRef, List<string> ClientSession, bool Origen)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                    SaldoContableBL objSaldoContableBL = new SaldoContableBL();
                    string[] IdRegistroEliminado = new string[3];

                    // Obtener la entidad fuente

                    if (Origen) // Eliminado Logico
                    {
                        #region Elimina Cabecera
                        var objEntitySources = (from a in dbContext.diario
                                                where a.v_IdDocumentoReferencia == pstrIdDocRef && a.i_Eliminado == 0
                                                select a).ToList();
                        foreach (var objEntitySource in objEntitySources)
                        {
                            if (objEntitySource != null)
                            {
                                #region Elimina Pendientes Por Cobrar

                                RestablecePendientesXCobrar(ref pobjOperationResult, objEntitySource.v_IdDiario);

                                if (pobjOperationResult.Success == 0) return null;

                                #endregion

                                IdRegistroEliminado[0] = objEntitySource.v_Periodo;
                                IdRegistroEliminado[1] = objEntitySource.v_Mes;
                                IdRegistroEliminado[2] = objEntitySource.v_Correlativo;

                                // Crear la entidad con los datos actualizados
                                objEntitySource.t_ActualizaFecha = DateTime.Now;
                                objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                                objEntitySource.i_Eliminado = 1;



                                #region Elimina Detalles

                                ////Eliminar detalles del movimiento eliminado.
                                //var objEntitySourceDetallesdiario = (from a in dbContext.diariodetalle
                                //                                     where a.v_IdDiario == objEntitySource.v_IdDiario && a.i_Eliminado == 0
                                //                                     select a).ToList();

                                foreach (var RegistrodiarioDetalle in objEntitySource.diariodetalle)
                                {
                                    RegistrodiarioDetalle.t_ActualizaFecha = DateTime.Now;
                                    RegistrodiarioDetalle.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                                    RegistrodiarioDetalle.i_Eliminado = 1;
                                    objSaldoContableBL.ActualizarSaldoContable(ref pobjOperationResult, ListaProcesos.Diario,
                                        objEntitySource.t_Fecha.Value, objEntitySource.i_IdMoneda.Value,
                                        diariodetalleAssembler.ToDTO(RegistrodiarioDetalle), RecordStatus.EliminadoLogico, RegistrodiarioDetalle.v_IdDiarioDetalle.Substring(0, 1));
                                    if (pobjOperationResult.Success == 0) return null;
                                }

                                #endregion
                            }
                        }
                        #endregion
                        //RestablecePendientesXCobrar(ref pobjOperationResult, objEntitySource.v_IdDiario);
                    }
                    else //Eliminado Fisico
                    {
                        #region Elimina Cabecera
                        var objEntitySources = (from a in dbContext.diario
                                                where a.v_IdDocumentoReferencia == pstrIdDocRef && a.i_Eliminado == 0
                                                select a).ToList();
                        foreach (var objEntitySource in objEntitySources)
                        {
                            if (objEntitySource != null)
                            {
                                #region Elimina Pendientes Por Cobrar
                                RestablecePendientesXCobrar(ref pobjOperationResult, objEntitySource.v_IdDiario);

                                if (pobjOperationResult.Success == 0) return null;
                                #endregion

                                IdRegistroEliminado[0] = objEntitySource.v_Periodo;
                                IdRegistroEliminado[1] = objEntitySource.v_Mes;
                                IdRegistroEliminado[2] = objEntitySource.v_Correlativo;

                                #region Elimina Detalles
                                var objEntityDiarioDetalles = (from a in dbContext.diariodetalle
                                                               where a.v_IdDiario == objEntitySource.v_IdDiario && a.i_Eliminado == 0
                                                               select a).ToList();

                                foreach (var RegistroDiarioDetalle in objEntityDiarioDetalles)
                                {
                                    objSaldoContableBL.ActualizarSaldoContable(ref pobjOperationResult, ListaProcesos.Diario, objEntitySource.t_Fecha.Value, objEntitySource.i_IdMoneda.Value, diariodetalleAssembler.ToDTO(RegistroDiarioDetalle), RecordStatus.EliminadoLogico, RegistroDiarioDetalle.v_IdDiarioDetalle.Substring(0, 1));
                                    if (pobjOperationResult.Success == 0) return null;
                                    dbContext.diariodetalle.DeleteObject(RegistroDiarioDetalle);
                                }
                                #endregion

                                dbContext.diario.DeleteObject(objEntitySource);
                            }
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
                pobjOperationResult.AdditionalInformation = "DiarioBL.EliminarDiarioXDocRef()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException.Message;
                return null;
            }
        }

        public string[] EliminarDiarioXIdPlanilla(ref OperationResult pobjOperationResult, int pstrIdDocRef, List<string> ClientSession, bool Origen)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                    SaldoContableBL objSaldoContableBL = new SaldoContableBL();
                    string[] IdRegistroEliminado = new string[3];

                    // Obtener la entidad fuente

                    if (Origen) // Eliminado Logico
                    {
                        #region Elimina Cabecera
                        var objEntitySource = (from a in dbContext.diario
                                               where a.i_IdPlanillaNumeracion == pstrIdDocRef && a.i_Eliminado == 0
                                               select a).FirstOrDefault();

                        #region Elimina Pendientes Por Cobrar
                        RestablecePendientesXCobrar(ref pobjOperationResult, objEntitySource.v_IdDiario);
                        if (pobjOperationResult.Success == 0) return null;
                        #endregion

                        IdRegistroEliminado[0] = objEntitySource.v_Periodo;
                        IdRegistroEliminado[1] = objEntitySource.v_Mes;
                        IdRegistroEliminado[2] = objEntitySource.v_Correlativo;

                        // Crear la entidad con los datos actualizados
                        objEntitySource.t_ActualizaFecha = DateTime.Now;
                        objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntitySource.i_Eliminado = 1;
                        #endregion

                        #region Elimina Detalles
                        ////Eliminar detalles del movimiento eliminado.
                        //var objEntitySourceDetallesdiario = (from a in dbContext.diariodetalle
                        //                                     where a.v_IdDiario == objEntitySource.v_IdDiario && a.i_Eliminado == 0
                        //                                     select a).ToList();

                        foreach (var RegistrodiarioDetalle in objEntitySource.diariodetalle)
                        {
                            RegistrodiarioDetalle.t_ActualizaFecha = DateTime.Now;
                            RegistrodiarioDetalle.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            RegistrodiarioDetalle.i_Eliminado = 1;
                            objSaldoContableBL.ActualizarSaldoContable(ref pobjOperationResult, ListaProcesos.Diario, objEntitySource.t_Fecha.Value, objEntitySource.i_IdMoneda.Value, diariodetalleAssembler.ToDTO(RegistrodiarioDetalle), RecordStatus.EliminadoLogico, RegistrodiarioDetalle.v_IdDiarioDetalle.Substring(0, 1));
                        }
                        #endregion
                        //RestablecePendientesXCobrar(ref pobjOperationResult, objEntitySource.v_IdDiario);
                    }
                    else //Eliminado Fisico
                    {
                        #region Elimina Cabecera
                        var objEntitySource = (from a in dbContext.diario
                                               where a.i_IdPlanillaNumeracion == pstrIdDocRef && a.i_Eliminado == 0
                                               select a).FirstOrDefault();

                        if (objEntitySource != null)
                        {
                            #region Elimina Pendientes Por Cobrar
                            RestablecePendientesXCobrar(ref pobjOperationResult, objEntitySource.v_IdDiario);
                            if (pobjOperationResult.Success == 0) return null;
                            #endregion

                            IdRegistroEliminado[0] = objEntitySource.v_Periodo;
                            IdRegistroEliminado[1] = objEntitySource.v_Mes;
                            IdRegistroEliminado[2] = objEntitySource.v_Correlativo;

                            #region Elimina Detalles
                            var objEntityDiarioDetalles = (from a in dbContext.diariodetalle
                                                           where a.v_IdDiario == objEntitySource.v_IdDiario && a.i_Eliminado == 0
                                                           select a).ToList();

                            foreach (var RegistroDiarioDetalle in objEntityDiarioDetalles)
                            {
                                objSaldoContableBL.ActualizarSaldoContable(ref pobjOperationResult, ListaProcesos.Diario, objEntitySource.t_Fecha.Value, objEntitySource.i_IdMoneda.Value, diariodetalleAssembler.ToDTO(RegistroDiarioDetalle), RecordStatus.EliminadoLogico, RegistroDiarioDetalle.v_IdDiarioDetalle.Substring(0, 1));
                                dbContext.diariodetalle.DeleteObject(RegistroDiarioDetalle);
                            }
                            #endregion

                            dbContext.diario.DeleteObject(objEntitySource);
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
                pobjOperationResult.AdditionalInformation = "DiarioBL.EliminarDiarioXDocRef()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException.Message;
                return null;
            }
        }

        public string[] EliminarDiarioCierreApertura(ref OperationResult pobjOperationResult, string pstrIdDocRef, List<string> ClientSession, bool Origen, string periodo)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                    SaldoContableBL objSaldoContableBL = new SaldoContableBL();
                    string[] IdRegistroEliminado = new string[3];

                    // Obtener la entidad fuente

                    //if (Origen) // Eliminado Logico
                    //{
                    //    #region Elimina Cabecera
                    //    var objEntitySource = (from a in dbContext.diario
                    //                           where a.v_IdDocumentoReferencia == pstrIdDocRef && a.i_Eliminado == 0
                    //                           select a).FirstOrDefault();

                    //    #region Elimina Pendientes Por Cobrar
                    //    RestablecePendientesXCobrar(ref pobjOperationResult, objEntitySource.v_IdDiario);
                    //    if (pobjOperationResult.Success == 0) return null;
                    //    #endregion

                    //    IdRegistroEliminado[0] = objEntitySource.v_Periodo;
                    //    IdRegistroEliminado[1] = objEntitySource.v_Mes;
                    //    IdRegistroEliminado[2] = objEntitySource.v_Correlativo;

                    //    // Crear la entidad con los datos actualizados
                    //    objEntitySource.t_ActualizaFecha = DateTime.Now;
                    //    objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                    //    objEntitySource.i_Eliminado = 1;
                    //    #endregion

                    //    #region Elimina Detalles
                    //    ////Eliminar detalles del movimiento eliminado.
                    //    //var objEntitySourceDetallesdiario = (from a in dbContext.diariodetalle
                    //    //                                     where a.v_IdDiario == objEntitySource.v_IdDiario && a.i_Eliminado == 0
                    //    //                                     select a).ToList();

                    //    foreach (var RegistrodiarioDetalle in objEntitySource.diariodetalle)
                    //    {
                    //        RegistrodiarioDetalle.t_ActualizaFecha = DateTime.Now;
                    //        RegistrodiarioDetalle.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                    //        RegistrodiarioDetalle.i_Eliminado = 1;
                    //        objSaldoContableBL.ActualizarSaldoContable(ref pobjOperationResult, ListaProcesos.Diario, objEntitySource.t_Fecha.Value, objEntitySource.i_IdMoneda.Value, diariodetalleAssembler.ToDTO(RegistrodiarioDetalle), RecordStatus.EliminadoLogico);
                    //    }
                    //    #endregion
                    //    //RestablecePendientesXCobrar(ref pobjOperationResult, objEntitySource.v_IdDiario);
                    //}
                    //else //Eliminado Fisico
                    //{
                    #region Elimina Cabecera
                    var objEntitySource = (from a in dbContext.diario
                                           where a.v_IdDocumentoReferencia.StartsWith(pstrIdDocRef) && a.i_Eliminado == 0
                                           && a.v_Periodo == periodo
                                           select a).FirstOrDefault();

                    if (objEntitySource != null)
                    {
                        #region Elimina Pendientes Por Cobrar
                        RestablecePendientesXCobrar(ref pobjOperationResult, objEntitySource.v_IdDiario);
                        if (pobjOperationResult.Success == 0) return null;
                        #endregion

                        IdRegistroEliminado[0] = objEntitySource.v_Periodo;
                        IdRegistroEliminado[1] = objEntitySource.v_Mes;
                        IdRegistroEliminado[2] = objEntitySource.v_Correlativo;

                        #region Elimina Detalles
                        var objEntityDiarioDetalles = (from a in dbContext.diariodetalle
                                                       where a.v_IdDiario == objEntitySource.v_IdDiario && a.i_Eliminado == 0
                                                       select a).ToList();

                        foreach (var RegistroDiarioDetalle in objEntityDiarioDetalles)
                        {
                            // objSaldoContableBL.ActualizarSaldoContable(ref pobjOperationResult, ListaProcesos.Diario, objEntitySource.t_Fecha.Value, objEntitySource.i_IdMoneda.Value, diariodetalleAssembler.ToDTO(RegistroDiarioDetalle), RecordStatus.EliminadoLogico, RegistroDiarioDetalle.v_IdDiarioDetalle.Substring(0, 1));
                            dbContext.diariodetalle.DeleteObject(RegistroDiarioDetalle);
                        }
                        #endregion

                        dbContext.diario.DeleteObject(objEntitySource);
                    }
                    #endregion
                    //}

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                    ts.Complete();
                    return IdRegistroEliminado;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "DiarioBL.EliminarDiarioXDocRef()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException.Message;
                return null;
            }
        }

        public diariodetalleDto BuscarDiaroDocumentoReferencia(string DocReferencia)
        {

            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {


                var diario = (from a in dbContext.diario
                              join b in dbContext.documento on new { TipoDoc = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { TipoDoc = b.i_CodigoDocumento, eliminado = b.i_Eliminado.Value } into b_join
                              from b in b_join.DefaultIfEmpty()
                              join c in dbContext.diariodetalle on new { DiarioDetalle = a.v_IdDiario, eliminado = 0 } equals new { DiarioDetalle = c.v_IdDiario, eliminado = c.i_Eliminado.Value } into c_join
                              from c in c_join.DefaultIfEmpty()
                              where a.i_Eliminado == 0 && a.v_IdDocumentoReferencia == DocReferencia && c.v_NroCuenta.StartsWith("59")

                              select c).FirstOrDefault();

                diariodetalleDto objDiarioDetalle = diariodetalleAssembler.ToDTO(diario);
                return objDiarioDetalle;

            }


        }

        public void RestablecePendientesXCobrar(ref OperationResult pobjOperationResult, string pstrIdDiario)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var pendientesDetalle = dbContext.pendientecobrardetalle.Where(p => p.v_IdDiario.Equals(pstrIdDiario)).ToList();

                        if (pendientesDetalle.Any())
                        {
                            foreach (var pendienteDetalleAgrupado in pendientesDetalle.GroupBy(o => o.pendientecobrar))
                            {
                                foreach (var pendienteDetalle in pendienteDetalleAgrupado)
                                {
                                    dbContext.pendientecobrardetalle.DeleteObject(pendienteDetalle);
                                }
                                dbContext.SaveChanges();

                                string idPendienteCobrar = pendienteDetalleAgrupado.Key.v_IdPendienteCobrar;
                                var objPendienteCobrarEntity = dbContext.pendientecobrar.FirstOrDefault(p => p.v_IdPendienteCobrar.Equals(idPendienteCobrar));

                                if (objPendienteCobrarEntity != null && objPendienteCobrarEntity.pendientecobrardetalle != null)
                                {
                                    var debeSoles = objPendienteCobrarEntity.pendientecobrardetalle.Where(o => o.v_Naturaleza == "D").Sum(p => p.d_ImporteSaldo);
                                    var debeDolares = objPendienteCobrarEntity.pendientecobrardetalle.Where(o => o.v_Naturaleza == "D").Sum(p => p.d_ImporteSaldoDolares);
                                    var haberSoles = objPendienteCobrarEntity.pendientecobrardetalle.Where(o => o.v_Naturaleza == "H").Sum(p => p.d_ImporteSaldo);
                                    var haberDolares = objPendienteCobrarEntity.pendientecobrardetalle.Where(o => o.v_Naturaleza == "H").Sum(p => p.d_ImporteSaldoDolares);

                                    objPendienteCobrarEntity.d_ImporteSaldo = (decimal)((float)debeSoles.Value - (float)haberSoles.Value);
                                    objPendienteCobrarEntity.d_ImporteSaldoDolares = (decimal)((float)debeDolares.Value - (float)haberDolares.Value);
                                    dbContext.pendientecobrar.ApplyCurrentValues(objPendienteCobrarEntity);
                                    dbContext.SaveChanges();
                                }
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
                pobjOperationResult.AdditionalInformation = "DiarioBL.RestablecePendientesXCobrar()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException.Message;
                return;
            }
        }

        //public void RestablecePendientesXPagar(ref OperationResult pobjOperationResult, string pstrIdDiario)
        //{
        //    try
        //    {
        //        using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
        //        {
        //            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
        //            {
        //                var xx = (from v in dbContext
        //                          where v.v_IdDiario == pstrIdDiario
        //                          select new { v.v_IdPendienteCobrar }).ToList();

        //                if (xx != null && xx.Count > 0)
        //                {
        //                    foreach (var x in xx)
        //                    {
        //                        string IdPendienteCobrar = x.v_IdPendienteCobrar;
        //                        pendientecobrar Entidad = (from e in dbContext.pendientecobrar
        //                                                   where e.v_IdPendienteCobrar == IdPendienteCobrar
        //                                                   select e).FirstOrDefault();
        //                        if (Entidad != null)
        //                        {
        //                            Entidad.pendientecobrardetalle.ToList().ForEach(p => dbContext.DeleteObject(p));

        //                            dbContext.DeleteObject(Entidad);
        //                            dbContext.SaveChanges();
        //                        }
        //                    }
        //                }
        //                pobjOperationResult.Success = 1;
        //                ts.Complete();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        pobjOperationResult.Success = 0;
        //        pobjOperationResult.AdditionalInformation = "DiarioBL.RestablecePendientesXCobrar()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
        //        pobjOperationResult.ErrorMessage = ex.Message;
        //        pobjOperationResult.ExceptionMessage = ex.InnerException.Message;
        //        return;
        //    }
        //}

        public bool ConsultarSiTieneTesorerias(string psrtIdDiario)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var xx = (from v in dbContext.pendientecobrardetalle
                              where v.v_IdDiario == psrtIdDiario
                              select new { v.v_IdPendienteCobrar }).FirstOrDefault();

                    string IdPendienteCobrar = xx != null ? xx.v_IdPendienteCobrar : string.Empty;


                    if (!string.IsNullOrEmpty(IdPendienteCobrar))
                    {
                        pendientecobrar Entidad = (from e in dbContext.pendientecobrar
                                                   where e.v_IdPendienteCobrar == IdPendienteCobrar
                                                   select e).FirstOrDefault();

                        if (Entidad.pendientecobrardetalle.ToList().Where(p => p.v_IdTesoreria != null).Count() != 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool AsientoAperturaUbicacionValida(string periodo, string mes, ref string Registro)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var AAActual = dbContext.diario.Where(p => p.i_IdTipoComprobante == 1 && p.v_Periodo == periodo && p.i_Eliminado == 0).ToList();

                    if (AAActual.Count == 0)
                    {
                        return true;
                    }
                    else
                    {
                        if (AAActual.Where(p => p.v_Mes != mes).ToList().Count != 0)
                        {
                            Registro = AAActual.Where(p => p.v_Mes != mes).FirstOrDefault().v_Periodo + "-" + AAActual.Where(p => p.v_Mes != mes).FirstOrDefault().v_Mes + "-" + AAActual.Where(p => p.v_Mes != mes).FirstOrDefault().v_Correlativo;
                            return true; //POR AHORA SE CAMBIA A TRUE EL RETURN POR UN CAMBIO DE ANALISIS CON EL SR VICTOR
                        }
                        else
                        {
                            return true;
                        }
                    }

                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void CuadrarAsientos(ref OperationResult pobjOperationResult)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var Diarios = dbContext.diario.Where(p => p.i_Eliminado == 0 && p.i_IdTipoDocumento.Value != 340).ToList(); // No se toma en cuenta DImportacion

                        foreach (var Diario in Diarios.AsParallel())
                        {
                            List<diariodetalle> oDiarioDetalle = new List<diariodetalle>();
                            if (Diario.i_IdTipoComprobante.HasValue && Diario.i_IdTipoComprobante.Value != 6)
                            {
                                foreach (var DiarioDetalle in Diario.diariodetalle.Where(p => p.i_Eliminado == 0))
                                {
                                    DiarioDetalle.d_Cambio = Diario.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(DiarioDetalle.d_Importe.Value / Diario.d_TipoCambio.Value, 2) : Utils.Windows.DevuelveValorRedondeado(DiarioDetalle.d_Importe.Value * Diario.d_TipoCambio.Value, 2);
                                    oDiarioDetalle.Add(DiarioDetalle);
                                }

                                #region Revisa Cuadre Cambios
                                //Revisa que la suma de los cambios del H cuadre con la suma de los cambios del D, 
                                //si no cuadra agarra el ultimo de los H y le suma/resta la diferencia
                                decimal? d_TotalDebe = oDiarioDetalle.Where(p => p.v_Naturaleza == "D").Sum(p => p.d_Importe).Value;
                                decimal? d_TotalHaber = oDiarioDetalle.Where(p => p.v_Naturaleza == "H").Sum(p => p.d_Importe).Value;
                                decimal? d_TotalHaberCambio = oDiarioDetalle.Where(p => p.v_Naturaleza == "H").Sum(p => p.d_Cambio).Value;
                                decimal? d_TotalDebeCambio = oDiarioDetalle.Where(p => p.v_Naturaleza == "D").Sum(p => p.d_Cambio).Value;
                                decimal? d_DiferenciaDebe = d_TotalDebe.Value - d_TotalHaber.Value;
                                decimal? d_DiferenciaHaber = d_TotalDebeCambio.Value - d_TotalHaberCambio.Value;

                                if (d_DiferenciaDebe != d_DiferenciaHaber)
                                {
                                    decimal? Diferencia = d_DiferenciaHaber - d_DiferenciaDebe;
                                    var UltimaFilaH = oDiarioDetalle.Last(p => p.v_Naturaleza == "H" && p.i_EsDestino != "1");
                                    UltimaFilaH.d_Cambio = UltimaFilaH.d_Cambio + Diferencia;

                                    Diario.d_TotalDebe = oDiarioDetalle.Where(p => p.v_Naturaleza == "D").Sum(p => p.d_Importe).Value;
                                    Diario.d_TotalHaber = oDiarioDetalle.Where(p => p.v_Naturaleza == "H").Sum(p => p.d_Importe).Value;
                                    Diario.d_TotalDebeCambio = oDiarioDetalle.Where(p => p.v_Naturaleza == "D").Sum(p => p.d_Cambio).Value;
                                    Diario.d_TotalHaberCambio = oDiarioDetalle.Where(p => p.v_Naturaleza == "H").Sum(p => p.d_Cambio).Value;
                                    Diario.d_DiferenciaDebe = d_TotalDebe.Value - d_TotalHaber.Value;
                                    Diario.d_DiferenciaHaber = d_TotalDebeCambio.Value - d_TotalHaberCambio.Value;
                                    dbContext.diario.ApplyCurrentValues(Diario);
                                    dbContext.SaveChanges();
                                }
                                #endregion
                            }
                            else
                            {
                                foreach (var DiarioDetalle in Diario.diariodetalle.Where(p => p.i_Eliminado == 0))
                                {
                                    DiarioDetalle.d_Cambio = 0;
                                    oDiarioDetalle.Add(DiarioDetalle);
                                }
                            }

                            var TotDebe = oDiarioDetalle.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Importe ?? 0);
                            var TotHaber = oDiarioDetalle.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Importe ?? 0);
                            var TotDebeC = oDiarioDetalle.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Cambio ?? 0);
                            var TotHaberC = oDiarioDetalle.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Cambio ?? 0);

                            Diario.d_TotalDebe = decimal.Parse(Math.Round(TotDebe, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                            Diario.d_TotalHaber = decimal.Parse(Math.Round(TotHaber, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                            Diario.d_TotalDebeCambio = decimal.Parse(Math.Round(TotDebeC, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                            Diario.d_TotalHaberCambio = decimal.Parse(Math.Round(TotHaberC, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                            Diario.d_DiferenciaDebe = decimal.Parse(Math.Round(TotDebe - TotHaber, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                            Diario.d_DiferenciaHaber = decimal.Parse(Math.Round(TotDebeC - TotHaberC, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                            dbContext.diario.ApplyCurrentValues(Diario);
                        }

                        RegeneraDiariosImportacion(ref  pobjOperationResult);  // Se regeneran los Diarios Importacion
                        if (pobjOperationResult.Success == 1)
                        {
                            dbContext.SaveChanges();
                            pobjOperationResult.Success = 1;
                            ts.Complete();
                        }
                        else
                        {
                            pobjOperationResult.Success = 0;
                        }
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

        public void CuadrarAsientos(ref OperationResult pobjOperationResult, string pstrDiarioId)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var Diarios = dbContext.diario.Where(p => p.v_IdDiario.Equals(pstrDiarioId)).ToList(); // No se toma en cuenta DImportacion

                        foreach (var Diario in Diarios.AsParallel())
                        {
                            List<diariodetalle> oDiarioDetalle = new List<diariodetalle>();
                            if (Diario.i_IdTipoComprobante.HasValue && Diario.i_IdTipoComprobante.Value != 6)
                            {
                                foreach (var DiarioDetalle in Diario.diariodetalle.Where(p => p.i_Eliminado == 0))
                                {
                                    DiarioDetalle.d_Cambio = Diario.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(DiarioDetalle.d_Importe.Value / Diario.d_TipoCambio.Value, 2) : Utils.Windows.DevuelveValorRedondeado(DiarioDetalle.d_Importe.Value * Diario.d_TipoCambio.Value, 2);
                                    oDiarioDetalle.Add(DiarioDetalle);
                                }

                                #region Revisa Cuadre Cambios
                                //Revisa que la suma de los cambios del H cuadre con la suma de los cambios del D, 
                                //si no cuadra agarra el ultimo de los H y le suma/resta la diferencia
                                decimal? d_TotalDebe = oDiarioDetalle.Where(p => p.v_Naturaleza == "D").Sum(p => p.d_Importe).Value;
                                decimal? d_TotalHaber = oDiarioDetalle.Where(p => p.v_Naturaleza == "H").Sum(p => p.d_Importe).Value;
                                decimal? d_TotalHaberCambio = oDiarioDetalle.Where(p => p.v_Naturaleza == "H").Sum(p => p.d_Cambio).Value;
                                decimal? d_TotalDebeCambio = oDiarioDetalle.Where(p => p.v_Naturaleza == "D").Sum(p => p.d_Cambio).Value;
                                decimal? d_DiferenciaDebe = d_TotalDebe.Value - d_TotalHaber.Value;
                                decimal? d_DiferenciaHaber = d_TotalDebeCambio.Value - d_TotalHaberCambio.Value;

                                if (d_DiferenciaDebe != d_DiferenciaHaber)
                                {
                                    decimal? Diferencia = d_DiferenciaHaber - d_DiferenciaDebe;
                                    var UltimaFilaH = oDiarioDetalle.Last(p => p.v_Naturaleza == "H" && p.i_EsDestino != "1");
                                    UltimaFilaH.d_Cambio = UltimaFilaH.d_Cambio + Diferencia;

                                    Diario.d_TotalDebe = oDiarioDetalle.Where(p => p.v_Naturaleza == "D").Sum(p => p.d_Importe).Value;
                                    Diario.d_TotalHaber = oDiarioDetalle.Where(p => p.v_Naturaleza == "H").Sum(p => p.d_Importe).Value;
                                    Diario.d_TotalDebeCambio = oDiarioDetalle.Where(p => p.v_Naturaleza == "D").Sum(p => p.d_Cambio).Value;
                                    Diario.d_TotalHaberCambio = oDiarioDetalle.Where(p => p.v_Naturaleza == "H").Sum(p => p.d_Cambio).Value;
                                    Diario.d_DiferenciaDebe = d_TotalDebe.Value - d_TotalHaber.Value;
                                    Diario.d_DiferenciaHaber = d_TotalDebeCambio.Value - d_TotalHaberCambio.Value;
                                    dbContext.diariodetalle.ApplyCurrentValues(UltimaFilaH);
                                    dbContext.diario.ApplyCurrentValues(Diario);
                                    dbContext.SaveChanges();
                                    if (pobjOperationResult.Success == 0) return;
                                }
                                #endregion
                            }
                            else
                            {
                                foreach (var DiarioDetalle in Diario.diariodetalle.Where(p => p.i_Eliminado == 0))
                                {
                                    DiarioDetalle.d_Cambio = 0;
                                    oDiarioDetalle.Add(DiarioDetalle);
                                }
                            }

                            var TotDebe = oDiarioDetalle.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Importe ?? 0);
                            var TotHaber = oDiarioDetalle.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Importe ?? 0);
                            var TotDebeC = oDiarioDetalle.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Cambio ?? 0);
                            var TotHaberC = oDiarioDetalle.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Cambio ?? 0);

                            Diario.d_TotalDebe = decimal.Parse(Math.Round(TotDebe, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                            Diario.d_TotalHaber = decimal.Parse(Math.Round(TotHaber, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                            Diario.d_TotalDebeCambio = decimal.Parse(Math.Round(TotDebeC, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                            Diario.d_TotalHaberCambio = decimal.Parse(Math.Round(TotHaberC, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                            Diario.d_DiferenciaDebe = decimal.Parse(Math.Round(TotDebe - TotHaber, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                            Diario.d_DiferenciaHaber = decimal.Parse(Math.Round(TotDebeC - TotHaberC, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                            dbContext.diario.ApplyCurrentValues(Diario);
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

        public void RegeneraDiariosImportacion(ref OperationResult pobjOperationResult)
        {
            string ErrorImportacion = "";
            try
            {

                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        string Periodo = Globals.ClientSession.i_Periodo.ToString();
                        var DiariosImportacion = dbContext.importacion.Where(p => p.i_Eliminado == 0 && p.v_Periodo == Periodo).ToList().ToDTOs();
                        DiarioBL _objDiarioBL = new DiarioBL();
                        DocumentoBL _objDocumentoBL = new DocumentoBL();
                        pobjOperationResult.Success = 1;

                        string ConceptoFob = ((int)Concepto.ValorFob).ToString();

                        var CuentaDetalleFob = (from n in dbContext.administracionconceptos

                                                where n.v_Codigo == ConceptoFob && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)

                                                select new { n.v_CuentaPVenta }).FirstOrDefault();

                        string ConceptoFlete = ((int)Concepto.Flete).ToString();
                        var CuentaDetalleFlete = (from n in dbContext.administracionconceptos

                                                  where n.v_Codigo == ConceptoFlete && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)

                                                  select new { n.v_CuentaPVenta }).FirstOrDefault();

                        string ConceptoSeguro = ((int)Concepto.Seguro).ToString();
                        var CuentaDetalleSeguro = (from n in dbContext.administracionconceptos

                                                   where n.v_Codigo == ConceptoSeguro && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)

                                                   select new { n.v_CuentaPVenta }).FirstOrDefault();

                        string ConceptoAdValorem = ((int)Concepto.AdValorem).ToString();
                        var CuentaDetalleAdValorem = (from n in dbContext.administracionconceptos
                                                      where n.v_Codigo == ConceptoAdValorem && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)
                                                      select new { n.v_CuentaPVenta }).FirstOrDefault();


                        string ConceptoIgv = ((int)Concepto.Igv).ToString();
                        var CuentaDetalleIgv = (from n in dbContext.administracionconceptos
                                                where n.v_Codigo == ConceptoIgv && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)
                                                select new { n.v_CuentaPVenta }).FirstOrDefault();

                        string ConceptoPercepcion = ((int)Concepto.Percepcion).ToString();

                        var CuentaDetallePercepcion = (from n in dbContext.administracionconceptos

                                                       where n.v_Codigo == ConceptoPercepcion && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)

                                                       select new { n.v_CuentaPVenta }).FirstOrDefault();

                        string ConceptoProveedoresFob = ((int)Concepto.ProveedoresFob).ToString();
                        var CuentaDetalleProveedorFob = (from n in dbContext.administracionconceptos

                                                         where n.v_Codigo == ConceptoProveedoresFob && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)

                                                         select new { n.v_CuentaPVenta }).FirstOrDefault();

                        string ConceptoProveedoresFlete = ((int)Concepto.ProveedoresFlete).ToString();
                        var CuentaDetalleProveedoresFlete = (from n in dbContext.administracionconceptos

                                                             where n.v_Codigo == ConceptoProveedoresFlete && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)

                                                             select new { n.v_CuentaPVenta }).FirstOrDefault();


                        string ConceptoProveedoresSeguro = ((int)Concepto.ProveedoresSeguro).ToString();
                        var CuentaDetalleProveedorSeguro = (from n in dbContext.administracionconceptos

                                                            where n.v_Codigo == ConceptoProveedoresSeguro && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)

                                                            select new { n.v_CuentaPVenta }).FirstOrDefault();

                        string ConceptoProveedoresAdvalorem = ((int)Concepto.ProveedoresAdvalorem).ToString();
                        var CuentaDetalleProveedorAdvalorem = (from n in dbContext.administracionconceptos

                                                               where n.v_Codigo == ConceptoProveedoresAdvalorem && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)
                                                               select new { n.v_CuentaPVenta }).FirstOrDefault();

                        string ConceptoProveedoresIgv = ((int)Concepto.ProveedorIgv).ToString();
                        var CuentaDetalleProveedorIgv = (from n in dbContext.administracionconceptos

                                                         where n.v_Codigo == ConceptoProveedoresIgv && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)
                                                         select new { n.v_CuentaPVenta }).FirstOrDefault();


                        string ConceptoProveedoresPercepcion = ((int)Concepto.ProveedorPercepcion).ToString();

                        var CuentaDetalleProveedoresPercepcion = (from n in dbContext.administracionconceptos

                                                                  where n.v_Codigo == ConceptoProveedoresPercepcion && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)

                                                                  select new { n.v_CuentaPVenta }).FirstOrDefault();
                        if (DiariosImportacion.Any())
                        {
                            Globals.ProgressbarStatus.i_Progress = 1;
                            Globals.ProgressbarStatus.i_TotalProgress = 1;
                            Globals.ProgressbarStatus.b_Cancelado = false;
                            Globals.ProgressbarStatus.i_TotalProgress = DiariosImportacion.Count;
                        }
                        foreach (importacionDto Imp in DiariosImportacion)
                        {
                            ErrorImportacion = Imp.v_IdImportacion;
                            diarioDto _diarioDto = new diarioDto();
                            diariodetalleDto D_Totales = new diariodetalleDto();
                            diariodetalleDto H_Totales = new diariodetalleDto();
                            List<KeyValueDTO> _ListadoDiarios = new List<KeyValueDTO>();
                            List<diariodetalleDto> TempDiarioInsertarImportaciones = new List<diariodetalleDto>();

                            var DiarioImp = dbContext.diario.Where(x => x.i_Eliminado == 0 && x.v_IdDocumentoReferencia == Imp.v_IdImportacion).FirstOrDefault().ToDTO();
                            var DiarioDetalles = dbContext.diariodetalle.Where(x => x.v_IdDiario == DiarioImp.v_IdDiario).ToList().ToDTOs();
                            #region Revisa Cuadre Cambios
                            //Revisa que la suma de los cambios del H cuadre con la suma de los cambios del D, 
                            //si no cuadra agarra el ultimo de los H y le suma/resta la diferencia
                            decimal? d_TotalDebe = DiarioDetalles.Where(p => p.v_Naturaleza == "D").Sum(p => p.d_Importe).Value;
                            decimal? d_TotalHaber = DiarioDetalles.Where(p => p.v_Naturaleza == "H").Sum(p => p.d_Importe).Value;
                            decimal? d_TotalHaberCambio = DiarioDetalles.Where(p => p.v_Naturaleza == "H").Sum(p => p.d_Cambio).Value;
                            decimal? d_TotalDebeCambio = DiarioDetalles.Where(p => p.v_Naturaleza == "D").Sum(p => p.d_Cambio).Value;
                            decimal? d_DiferenciaDebe = d_TotalDebe.Value - d_TotalHaber.Value;
                            decimal? d_DiferenciaHaber = d_TotalDebeCambio.Value - d_TotalHaberCambio.Value;

                            //if (d_DiferenciaDebe != d_DiferenciaHaber)
                            //{
                            #region Genera Libro Diario
                            if (decimal.Parse(Imp.d_TotalValorFob.ToString()) > 0)
                            {

                                string[] IdRegistroEliminado = new string[3];
                                int _MaxMovimiento;
                                var DetalleImportacionesFOB = (from d in dbContext.importaciondetallefob
                                                               where d.v_IdImportacion == Imp.v_IdImportacion && d.i_Eliminado == 0
                                                               select d).ToList();

                                var queryExisteDiario = (from n in dbContext.diario
                                                         where n.v_IdDocumentoReferencia == Imp.v_IdImportacion
                                                         select n).FirstOrDefault();
                                if (queryExisteDiario != null)
                                {

                                    IdRegistroEliminado = _objDiarioBL.EliminarDiarioXDocRef(ref pobjOperationResult, Imp.v_IdImportacion, Globals.ClientSession.GetAsList(), false);
                                    if (IdRegistroEliminado == null || (IdRegistroEliminado != null && (IdRegistroEliminado[1].Trim() != Imp.t_FechaRegistro.Value.Month.ToString("00") || IdRegistroEliminado[0] != Imp.t_FechaRegistro.Value.Year.ToString())))
                                    {
                                        _ListadoDiarios = _objDiarioBL.ObtenerListadoDiario(ref pobjOperationResult, Imp.v_Periodo, Imp.v_Mes, (int)LibroDiarios.Importaciones);
                                        _MaxMovimiento = _ListadoDiarios.Count() > 0 ? int.Parse(_ListadoDiarios[_ListadoDiarios.Count() - 1].Value1.ToString()) : 0;
                                        _MaxMovimiento++;
                                        _diarioDto.v_Periodo = Imp.v_Periodo;
                                        _diarioDto.v_Mes = Imp.v_Mes;
                                        _diarioDto.v_Correlativo = _MaxMovimiento.ToString("00000000");

                                    }
                                    else
                                    {
                                        _diarioDto.v_Periodo = IdRegistroEliminado[0];
                                        _diarioDto.v_Mes = IdRegistroEliminado[1];
                                        _diarioDto.v_Correlativo = IdRegistroEliminado[2];

                                    }
                                }
                                else
                                {
                                    _ListadoDiarios = _objDiarioBL.ObtenerListadoDiario(ref pobjOperationResult, Imp.v_Periodo, Imp.v_Mes, (int)LibroDiarios.Importaciones);
                                    _MaxMovimiento = _ListadoDiarios.Count() > 0 ? int.Parse(_ListadoDiarios[_ListadoDiarios.Count() - 1].Value1.ToString()) : 0;
                                    _MaxMovimiento++;
                                    _diarioDto.v_Periodo = Imp.v_Periodo;
                                    _diarioDto.v_Mes = Imp.v_Mes;
                                    _diarioDto.v_Correlativo = _MaxMovimiento.ToString("00000000");
                                }

                                if (Imp.i_IdEstado == 1)
                                {

                                    _diarioDto.t_InsertaFecha = DiarioImp.t_InsertaFecha;
                                    _diarioDto.t_ActualizaFecha = DiarioImp.t_ActualizaFecha;
                                    _diarioDto.i_InsertaIdUsuario = DiarioImp.i_InsertaIdUsuario;
                                    _diarioDto.i_ActualizaIdUsuario = DiarioImp.i_ActualizaIdUsuario;
                                    _diarioDto.v_IdDocumentoReferencia = Imp.v_IdImportacion;
                                    _diarioDto.v_Nombre = "IMPORTACIÓN" + " " + Imp.v_Mes.Trim() + "-" + Imp.v_Correlativo.Trim();
                                    _diarioDto.v_Glosa = "IMPORTACIÓN DE MERCADERÍA";
                                    _diarioDto.d_TipoCambio = Imp.d_TipoCambio.Value;
                                    _diarioDto.i_IdMoneda = Imp.i_IdMoneda.Value;
                                    _diarioDto.i_IdTipoDocumento = (int)LibroDiarios.Importaciones;
                                    _diarioDto.t_Fecha = Imp.t_FechaRegistro.Value;
                                    _diarioDto.i_IdTipoComprobante = 2;
                                    #region Fob



                                    if (CuentaDetalleFob.v_CuentaPVenta != string.Empty & CuentaDetalleFob.v_CuentaPVenta != null)
                                    {

                                        foreach (var Proveedor in DetalleImportacionesFOB)
                                        {

                                            var Cliente = (from n in dbContext.cliente
                                                           where n.v_IdCliente == Proveedor.v_IdCliente && n.i_Eliminado == 0 && n.v_FlagPantalla == "V"
                                                           select new { n.v_CodCliente }).FirstOrDefault();


                                            D_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(Proveedor.d_ValorFob.Value, 2);
                                            D_Totales.d_Cambio = Imp.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(Proveedor.d_ValorFob.Value / Proveedor.d_TipoCambio.Value, 2) : Utils.Windows.DevuelveValorRedondeado(Proveedor.d_ValorFob.Value * Proveedor.d_TipoCambio.Value, 2);
                                            D_Totales.i_IdTipoDocumento = (int)LibroDiarios.Importaciones;
                                            D_Totales.t_Fecha = Imp.t_FechaRegistro.Value;
                                            //D_Totales.v_CodAnexo = Cliente.v_CodCliente;
                                            D_Totales.v_IdCliente = Proveedor.v_IdCliente;
                                            // D_Totales.v_Naturaleza = Proveedor.d_ValorFob > 0 ? pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H" : pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D";
                                            D_Totales.v_Naturaleza = Proveedor.d_ValorFob > 0 ? !_objDocumentoBL.DocumentoEsInverso(Imp.i_IdTipoDocumento.Value) ? "D" : "H" : !_objDocumentoBL.DocumentoEsInverso(Imp.i_IdTipoDocumento.Value) ? "H" : "D";
                                            D_Totales.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                                            D_Totales.v_NroCuenta = CuentaDetalleFob.v_CuentaPVenta;
                                            D_Totales.i_IdCentroCostos = "";
                                            D_Totales.i_IdTipoDocumentoRef = -1;
                                            TempDiarioInsertarImportaciones.Add(D_Totales);
                                            D_Totales = new diariodetalleDto();

                                        }
                                    }

                                    #endregion

                                    #region Flete


                                    if (Imp.d_Flete > 0 && CuentaDetalleFlete.v_CuentaPVenta != string.Empty && CuentaDetalleFlete.v_CuentaPVenta != null)
                                    {
                                        var Cliente = (from n in dbContext.cliente
                                                       where n.v_IdCliente == Imp.v_IdClienteDoc1 && n.i_Eliminado == 0 && n.v_FlagPantalla == "V"
                                                       select new { n.v_CodCliente }).FirstOrDefault();

                                        D_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(Imp.d_Flete.Value, 2);
                                        D_Totales.d_Cambio = Imp.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(Imp.d_Flete.Value / Imp.d_TipoCambioDoc1.Value, 2) : Utils.Windows.DevuelveValorRedondeado(Imp.d_Flete.Value * Imp.d_TipoCambioDoc1.Value, 2);
                                        D_Totales.i_IdTipoDocumento = (int)LibroDiarios.Importaciones;
                                        D_Totales.t_Fecha = Imp.t_FechaRegistro.Value;
                                        //D_Totales.v_CodAnexo = Cliente.v_CodCliente;
                                        D_Totales.v_IdCliente = Imp.v_IdClienteDoc1;
                                        //D_Totales.v_Naturaleza = pobjDtoEntity.d_Flete > 0 ? pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H" : pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D";
                                        D_Totales.v_Naturaleza = Imp.d_Flete > 0 ? !_objDocumentoBL.DocumentoEsInverso(Imp.i_IdTipoDocumento.Value) ? "D" : "H" : !_objDocumentoBL.DocumentoEsInverso(Imp.i_IdTipoDocumento.Value) ? "H" : "D";
                                        D_Totales.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                                        D_Totales.v_NroCuenta = CuentaDetalleFlete.v_CuentaPVenta;
                                        D_Totales.i_IdCentroCostos = "";
                                        D_Totales.i_IdTipoDocumentoRef = -1;
                                        TempDiarioInsertarImportaciones.Add(D_Totales);
                                        D_Totales = new diariodetalleDto();
                                    }
                                    #endregion

                                    #region Seguro


                                    if (Imp.i_PagaSeguro == 1 && CuentaDetalleSeguro.v_CuentaPVenta != string.Empty && CuentaDetalleSeguro.v_CuentaPVenta != null && decimal.Parse(Imp.d_PagoSeguro.ToString()) > 0)
                                    {

                                        var Cliente = (from n in dbContext.cliente
                                                       where n.v_IdCliente == Imp.v_IdClienteDoc2 && n.i_Eliminado == 0 && n.v_FlagPantalla == "V"
                                                       select new { n.v_CodCliente }).FirstOrDefault();

                                        D_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(Imp.d_PagoSeguro.Value, 2);
                                        D_Totales.d_Cambio = Imp.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(Imp.d_PagoSeguro.Value / Imp.d_TipoCambioDoc2.Value, 2) : Utils.Windows.DevuelveValorRedondeado(Imp.d_PagoSeguro.Value * Imp.d_TipoCambioDoc2.Value, 2);
                                        D_Totales.i_IdTipoDocumento = (int)LibroDiarios.Importaciones;
                                        D_Totales.t_Fecha = Imp.t_FechaRegistro.Value;
                                        //D_Totales.v_CodAnexo = Cliente.v_CodCliente;
                                        D_Totales.v_IdCliente = Imp.v_IdClienteDoc2;
                                        // D_Totales.v_Naturaleza = pobjDtoEntity.d_PagoSeguro > 0 ? pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H" : pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D";
                                        D_Totales.v_Naturaleza = Imp.d_PagoSeguro > 0 ? !_objDocumentoBL.DocumentoEsInverso(Imp.i_IdTipoDocumento.Value) ? "D" : "H" : !_objDocumentoBL.DocumentoEsInverso(Imp.i_IdTipoDocumento.Value) ? "H" : "D";
                                        D_Totales.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                                        D_Totales.v_NroCuenta = CuentaDetalleSeguro.v_CuentaPVenta;
                                        D_Totales.i_IdCentroCostos = "";
                                        D_Totales.i_IdTipoDocumentoRef = -1;
                                        TempDiarioInsertarImportaciones.Add(D_Totales);
                                        D_Totales = new diariodetalleDto();

                                    }
                                    #endregion

                                    #region Advaloren


                                    if (Imp.i_AdValorem == 1 && CuentaDetalleAdValorem.v_CuentaPVenta != string.Empty && CuentaDetalleAdValorem.v_CuentaPVenta != null && decimal.Parse(Imp.d_AdValorem.ToString()) > 0)
                                    {
                                        var Cliente = (from n in dbContext.cliente
                                                       where n.v_IdCliente == Imp.v_IdClienteDoc3 && n.i_Eliminado == 0 && n.v_FlagPantalla == "V"
                                                       select new { n.v_CodCliente }).FirstOrDefault();

                                        D_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(Imp.d_AdValorem.Value, 2);
                                        D_Totales.d_Cambio = Imp.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(Imp.d_AdValorem.Value / Imp.d_TipoCambioDoc3.Value, 2) : Utils.Windows.DevuelveValorRedondeado(Imp.d_AdValorem.Value * Imp.d_TipoCambioDoc3.Value, 2);
                                        D_Totales.i_IdTipoDocumento = (int)LibroDiarios.Importaciones;
                                        D_Totales.t_Fecha = Imp.t_FechaRegistro.Value;
                                        //D_Totales.v_CodAnexo = Cliente.v_CodCliente;
                                        D_Totales.v_IdCliente = Imp.v_IdClienteDoc3;
                                        // D_Totales.v_Naturaleza = pobjDtoEntity.d_AdValorem > 0 ? pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H" : pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D";
                                        D_Totales.v_Naturaleza = Imp.d_AdValorem > 0 ? !_objDocumentoBL.DocumentoEsInverso(Imp.i_IdTipoDocumento.Value) ? "D" : "H" : !_objDocumentoBL.DocumentoEsInverso(Imp.i_IdTipoDocumento.Value) ? "H" : "D";
                                        D_Totales.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                                        D_Totales.v_NroCuenta = CuentaDetalleAdValorem.v_CuentaPVenta;
                                        D_Totales.i_IdCentroCostos = "";
                                        D_Totales.i_IdTipoDocumentoRef = -1;
                                        TempDiarioInsertarImportaciones.Add(D_Totales);
                                        D_Totales = new diariodetalleDto();


                                    }
                                    #endregion
                                    #region Igv


                                    if (decimal.Parse(Imp.d_Igv.ToString()) > 0 && CuentaDetalleIgv.v_CuentaPVenta != null & CuentaDetalleIgv.v_CuentaPVenta != string.Empty)
                                    {
                                        var Cliente = (from n in dbContext.cliente
                                                       where n.v_IdCliente == Imp.v_IdClienteDoc4 && n.i_Eliminado == 0 && n.v_FlagPantalla == "V"
                                                       select new { n.v_CodCliente }).FirstOrDefault();

                                        D_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(Imp.d_Igv.Value, 2);
                                        D_Totales.d_Cambio = Imp.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(Imp.d_Igv.Value / Imp.d_TipoCambioDoc4.Value, 2) : Utils.Windows.DevuelveValorRedondeado(Imp.d_Igv.Value * Imp.d_TipoCambioDoc4.Value, 2);
                                        D_Totales.i_IdTipoDocumento = (int)LibroDiarios.Importaciones;
                                        D_Totales.t_Fecha = Imp.t_FechaRegistro.Value;
                                        // D_Totales.v_CodAnexo = Cliente.v_CodCliente;
                                        D_Totales.v_IdCliente = Imp.v_IdClienteDoc4;
                                        //  D_Totales.v_Naturaleza = pobjDtoEntity.d_Igv > 0 ? pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H" : pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D";
                                        D_Totales.v_Naturaleza = Imp.d_Igv > 0 ? !_objDocumentoBL.DocumentoEsInverso(Imp.i_IdTipoDocumento.Value) ? "D" : "H" : _objDocumentoBL.DocumentoEsInverso(Imp.i_IdTipoDocumento.Value) ? "H" : "D";

                                        D_Totales.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                                        D_Totales.v_NroCuenta = CuentaDetalleIgv.v_CuentaPVenta;
                                        D_Totales.i_IdCentroCostos = "";
                                        D_Totales.i_IdTipoDocumentoRef = -1;
                                        TempDiarioInsertarImportaciones.Add(D_Totales);
                                        D_Totales = new diariodetalleDto();
                                    }
                                    #endregion

                                    #region Percepcion



                                    if (decimal.Parse(Imp.d_Percepcion.ToString()) > 0 && CuentaDetallePercepcion.v_CuentaPVenta != null && CuentaDetallePercepcion.v_CuentaPVenta != string.Empty)
                                    {
                                        var Cliente = (from n in dbContext.cliente
                                                       where n.v_IdCliente == Imp.v_IdClienteDoc4 && n.i_Eliminado == 0 && n.v_FlagPantalla == "V"
                                                       select new { n.v_CodCliente }).FirstOrDefault();

                                        D_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(Imp.d_Percepcion.Value, 2);
                                        D_Totales.d_Cambio = Imp.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(Imp.d_Percepcion.Value / Imp.d_TipoCambioDoc4.Value, 2) : Utils.Windows.DevuelveValorRedondeado(Imp.d_Percepcion.Value * Imp.d_TipoCambioDoc4.Value, 2);
                                        D_Totales.i_IdTipoDocumento = (int)LibroDiarios.Importaciones;
                                        D_Totales.t_Fecha = Imp.t_FechaRegistro.Value;

                                        D_Totales.v_IdCliente = Imp.v_IdClienteDoc4;

                                        D_Totales.v_Naturaleza = Imp.d_Percepcion > 0 ? !_objDocumentoBL.DocumentoEsInverso(Imp.i_IdTipoDocumento.Value) ? "D" : "H" : !_objDocumentoBL.DocumentoEsInverso(int.Parse(Imp.i_IdTipoDocumento.Value.ToString())) ? "H" : "D";
                                        D_Totales.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;


                                        D_Totales.v_NroCuenta = CuentaDetallePercepcion.v_CuentaPVenta;
                                        D_Totales.i_IdCentroCostos = "";
                                        D_Totales.i_IdTipoDocumentoRef = -1;
                                        TempDiarioInsertarImportaciones.Add(D_Totales);
                                        D_Totales = new diariodetalleDto();
                                    }

                                    #endregion
                                    #region ProveedoresFob


                                    if (CuentaDetalleProveedorFob.v_CuentaPVenta != string.Empty & CuentaDetalleProveedorFob.v_CuentaPVenta != null)
                                    {

                                        foreach (var Proveedor in DetalleImportacionesFOB)
                                        {

                                            var Cliente = (from n in dbContext.cliente
                                                           where n.v_IdCliente == Proveedor.v_IdCliente && n.i_Eliminado == 0 && n.v_FlagPantalla == "V"
                                                           select new { n.v_CodCliente }).FirstOrDefault();

                                            H_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(Proveedor.d_ValorFob.Value, 2);
                                            H_Totales.d_Cambio = Imp.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(Proveedor.d_ValorFob.Value / Proveedor.d_TipoCambio.Value, 2) : Utils.Windows.DevuelveValorRedondeado(Proveedor.d_ValorFob.Value * Proveedor.d_TipoCambio.Value, 2);
                                            //H_Totales.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;

                                            H_Totales.i_IdTipoDocumento = Proveedor.i_IdTipoDocumento.Value;
                                            H_Totales.t_Fecha = Imp.t_FechaRegistro.Value;
                                            //H_Totales.v_CodAnexo = Cliente.v_CodCliente;
                                            H_Totales.v_IdCliente = Proveedor.v_IdCliente;
                                            //H_Totales.v_Naturaleza = Proveedor.d_ValorFob > 0 ? pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D" : pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H";

                                            H_Totales.v_Naturaleza = Proveedor.d_ValorFob > 0 ? !_objDocumentoBL.DocumentoEsInverso(Imp.i_IdTipoDocumento.Value) ? "H" : "D" : !_objDocumentoBL.DocumentoEsInverso(Imp.i_IdTipoDocumento.Value) ? "D" : "H";

                                            H_Totales.v_NroDocumento = Proveedor.v_SerieDocumento + "-" + Proveedor.v_CorrelativoDocumento;
                                            H_Totales.v_NroCuenta = CuentaDetalleProveedorFob.v_CuentaPVenta;
                                            H_Totales.i_IdCentroCostos = "";
                                            D_Totales.i_IdTipoDocumentoRef = -1;
                                            TempDiarioInsertarImportaciones.Add(H_Totales);
                                            H_Totales = new diariodetalleDto();

                                        }

                                    #endregion


                                        #region ProveedoresFlete



                                        if (Imp.d_Flete > 0 && CuentaDetalleProveedoresFlete.v_CuentaPVenta != null && CuentaDetalleProveedoresFlete.v_CuentaPVenta != string.Empty)
                                        {

                                            var Cliente = (from n in dbContext.cliente
                                                           where n.i_Eliminado == 0 && n.v_FlagPantalla == "V" && n.v_IdCliente == Imp.v_IdClienteDoc1
                                                           select new { n.v_CodCliente }).FirstOrDefault();


                                            H_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(Imp.d_Flete.Value, 2);
                                            H_Totales.d_Cambio = Imp.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(Imp.d_Flete.Value / Imp.d_TipoCambioDoc1.Value, 2) : Utils.Windows.DevuelveValorRedondeado(Imp.d_Flete.Value * Imp.d_TipoCambioDoc1.Value, 2);
                                            // H_Totales.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                                            H_Totales.i_IdTipoDocumento = Imp.i_IdTipoDocumento1.Value;
                                            H_Totales.t_Fecha = Imp.t_FechaRegistro.Value;
                                            //H_Totales.v_CodAnexo = Cliente.v_CodCliente;
                                            H_Totales.v_IdCliente = Imp.v_IdClienteDoc1;
                                            // H_Totales.v_Naturaleza = pobjDtoEntity.d_Flete > 0 ? pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D" : pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H";

                                            H_Totales.v_Naturaleza = Imp.d_Flete > 0 ? !_objDocumentoBL.DocumentoEsInverso(Imp.i_IdTipoDocumento.Value) ? "H" : "D" : !_objDocumentoBL.DocumentoEsInverso(Imp.i_IdTipoDocumento.Value) ? "D" : "H";
                                            H_Totales.v_NroDocumento = Imp.v_SerieDocumento1 + "-" + Imp.v_CorrelativoDocumento1;
                                            H_Totales.v_NroCuenta = CuentaDetalleProveedoresFlete.v_CuentaPVenta;
                                            H_Totales.i_IdCentroCostos = "";
                                            D_Totales.i_IdTipoDocumentoRef = -1;
                                            TempDiarioInsertarImportaciones.Add(H_Totales);
                                            H_Totales = new diariodetalleDto();
                                        }

                                        #endregion

                                        #region Proveedores Seguro


                                        if (Imp.i_PagaSeguro == 1 && decimal.Parse(Imp.d_PagoSeguro.ToString()) > 0 && CuentaDetalleProveedorSeguro.v_CuentaPVenta != null && CuentaDetalleProveedorSeguro.v_CuentaPVenta != string.Empty)
                                        {

                                            var Cliente = (from n in dbContext.cliente
                                                           where n.i_Eliminado == 0 && n.v_FlagPantalla == "V" && n.v_IdCliente == Imp.v_IdClienteDoc2
                                                           select new { n.v_CodCliente }).FirstOrDefault();

                                            H_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(Imp.d_PagoSeguro.Value, 2);
                                            H_Totales.d_Cambio = Imp.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(Imp.d_PagoSeguro.Value / Imp.d_TipoCambioDoc2.Value, 2) : Utils.Windows.DevuelveValorRedondeado(Imp.d_PagoSeguro.Value * Imp.d_TipoCambioDoc2.Value, 2);
                                            H_Totales.i_IdTipoDocumento = Imp.i_IdTipoDocumento2.Value;
                                            // H_Totales.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                                            H_Totales.t_Fecha = Imp.t_FechaRegistro.Value;
                                            //H_Totales.v_CodAnexo = Cliente.v_CodCliente;
                                            H_Totales.v_IdCliente = Imp.v_IdClienteDoc2;
                                            // H_Totales.v_Naturaleza = pobjDtoEntity.d_PagoSeguro > 0 ? pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D" : pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H";

                                            H_Totales.v_Naturaleza = Imp.d_PagoSeguro > 0 ? !_objDocumentoBL.DocumentoEsInverso(Imp.i_IdTipoDocumento.Value) ? "H" : "D" : !_objDocumentoBL.DocumentoEsInverso(Imp.i_IdTipoDocumento.Value) ? "D" : "H";

                                            H_Totales.v_NroDocumento = Imp.v_SerieDocumento2 + "-" + Imp.v_CorrelativoDocumento2;
                                            H_Totales.v_NroCuenta = CuentaDetalleProveedorSeguro.v_CuentaPVenta;
                                            H_Totales.i_IdCentroCostos = "";
                                            D_Totales.i_IdTipoDocumentoRef = -1;
                                            TempDiarioInsertarImportaciones.Add(H_Totales);
                                            H_Totales = new diariodetalleDto();

                                        }

                                        #endregion

                                        #region ProveedoresAdvalorem



                                        if (Imp.i_AdValorem == 1 && decimal.Parse(Imp.d_AdValorem.ToString()) > 0 && CuentaDetalleProveedorAdvalorem.v_CuentaPVenta != null && CuentaDetalleProveedorAdvalorem.v_CuentaPVenta != string.Empty)
                                        {
                                            var Cliente = (from n in dbContext.cliente
                                                           where n.i_Eliminado == 0 && n.v_FlagPantalla == "V" && n.v_IdCliente == Imp.v_IdClienteDoc3
                                                           select new { n.v_CodCliente }).FirstOrDefault();

                                            H_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(Imp.d_AdValorem.Value, 2);
                                            H_Totales.d_Cambio = Imp.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(Imp.d_AdValorem.Value / Imp.d_TipoCambioDoc3.Value, 2) : Utils.Windows.DevuelveValorRedondeado(Imp.d_AdValorem.Value * Imp.d_TipoCambioDoc3.Value, 2);
                                            //H_Totales.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                                            H_Totales.i_IdTipoDocumento = Imp.i_IdTipoDocumento3.Value;
                                            H_Totales.t_Fecha = Imp.t_FechaRegistro.Value;
                                            //H_Totales.v_CodAnexo = Cliente.v_CodCliente;
                                            H_Totales.v_IdCliente = Imp.v_IdClienteDoc3;
                                            //H_Totales.v_Naturaleza = pobjDtoEntity.d_AdValorem > 0 ? pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D" : pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H";
                                            H_Totales.v_Naturaleza = Imp.d_AdValorem > 0 ? !_objDocumentoBL.DocumentoEsInverso(Imp.i_IdTipoDocumento.Value) ? "H" : "D" : !_objDocumentoBL.DocumentoEsInverso(Imp.i_IdTipoDocumento.Value) ? "D" : "H";
                                            H_Totales.v_NroDocumento = Imp.v_SerieDocumento3 + "-" + Imp.v_CorrelativoDocumento3;
                                            H_Totales.v_NroCuenta = CuentaDetalleProveedorAdvalorem.v_CuentaPVenta;
                                            H_Totales.i_IdCentroCostos = "";
                                            D_Totales.i_IdTipoDocumentoRef = -1;
                                            TempDiarioInsertarImportaciones.Add(H_Totales);
                                            H_Totales = new diariodetalleDto();

                                        }

                                        #endregion

                                        #region Proveedores Igv


                                        if (decimal.Parse(Imp.d_Igv.ToString()) > 0 && CuentaDetalleProveedorIgv.v_CuentaPVenta != null && CuentaDetalleProveedorIgv.v_CuentaPVenta != string.Empty)
                                        {

                                            var Cliente = (from n in dbContext.cliente
                                                           where n.i_Eliminado == 0 && n.v_FlagPantalla == "V" && n.v_IdCliente == Imp.v_IdClienteDoc4
                                                           select new { n.v_CodCliente }).FirstOrDefault();

                                            H_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(Imp.d_Igv.Value, 2);
                                            H_Totales.d_Cambio = Imp.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(Imp.d_Igv.Value / Imp.d_TipoCambioDoc4.Value, 2) : Utils.Windows.DevuelveValorRedondeado(Imp.d_Igv.Value * Imp.d_TipoCambioDoc4.Value, 2);
                                            // H_Totales.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                                            H_Totales.i_IdTipoDocumento = Imp.i_IdTipoDocumento4.Value;
                                            H_Totales.t_Fecha = Imp.t_FechaRegistro.Value;
                                            //H_Totales.v_CodAnexo = Cliente.v_CodCliente;
                                            H_Totales.v_IdCliente = Imp.v_IdClienteDoc4;
                                            // H_Totales.v_Naturaleza = pobjDtoEntity.d_Igv > 0 ? pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D" : pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H";
                                            H_Totales.v_Naturaleza = Imp.d_Igv > 0 ? !_objDocumentoBL.DocumentoEsInverso(Imp.i_IdTipoDocumento.Value) ? "H" : "D" : !_objDocumentoBL.DocumentoEsInverso(Imp.i_IdTipoDocumento.Value) ? "D" : "H";
                                            H_Totales.v_NroDocumento = Imp.v_SerieDocumento4 + "-" + Imp.v_CorrelativoDoc4;
                                            H_Totales.v_NroCuenta = CuentaDetalleProveedorIgv.v_CuentaPVenta;
                                            H_Totales.i_IdCentroCostos = "";
                                            D_Totales.i_IdTipoDocumentoRef = -1;
                                            TempDiarioInsertarImportaciones.Add(H_Totales);
                                            H_Totales = new diariodetalleDto();
                                        }
                                        #endregion

                                        #region ProveedorPercepcion
                                        if (decimal.Parse(Imp.d_Percepcion.ToString()) > 0 && CuentaDetalleProveedoresPercepcion.v_CuentaPVenta != string.Empty && CuentaDetalleProveedoresPercepcion.v_CuentaPVenta != null)
                                        {

                                            var Cliente = (from n in dbContext.cliente
                                                           where n.i_Eliminado == 0 && n.v_FlagPantalla == "V" && n.v_IdCliente == Imp.v_IdClienteDoc4
                                                           select new { n.v_CodCliente }).FirstOrDefault();

                                            H_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(Imp.d_Percepcion.Value, 2);
                                            H_Totales.d_Cambio = Imp.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(Imp.d_Percepcion.Value / Imp.d_TipoCambioDoc4.Value, 2) : Utils.Windows.DevuelveValorRedondeado(Imp.d_Percepcion.Value * Imp.d_TipoCambioDoc4.Value, 2);
                                            H_Totales.i_IdTipoDocumento = Imp.i_IdTipoDocumento4.Value;
                                            H_Totales.t_Fecha = Imp.t_FechaRegistro.Value;
                                            H_Totales.v_IdCliente = Imp.v_IdClienteDoc4;
                                            H_Totales.v_Naturaleza = Imp.d_Percepcion > 0 ? !_objDocumentoBL.DocumentoEsInverso(Imp.i_IdTipoDocumento.Value) ? "H" : "D" : !_objDocumentoBL.DocumentoEsInverso(Imp.i_IdTipoDocumento.Value) ? "D" : "H";
                                            // H_Totales.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                                            H_Totales.v_NroDocumento = Imp.v_SerieDocumento4 + "-" + Imp.v_CorrelativoDoc4;
                                            H_Totales.v_NroCuenta = CuentaDetalleProveedoresPercepcion.v_CuentaPVenta;
                                            H_Totales.i_IdCentroCostos = "";
                                            D_Totales.i_IdTipoDocumentoRef = -1;
                                            TempDiarioInsertarImportaciones.Add(H_Totales);
                                            H_Totales = new diariodetalleDto();
                                        }

                                        #endregion
                                    }


                                    decimal TotDebe, TotHaber, TotDebeC, TotHaberC;
                                    TotDebe = TempDiarioInsertarImportaciones.Where(p => p.v_Naturaleza == "D" && p.v_NroCuenta != string.Empty).Sum(o => o.d_Importe.Value);
                                    TotHaber = TempDiarioInsertarImportaciones.Where(p => p.v_Naturaleza == "H" && p.v_NroCuenta != string.Empty).Sum(o => o.d_Importe.Value);
                                    TotDebeC = TempDiarioInsertarImportaciones.Where(p => p.v_Naturaleza == "D" && p.v_NroCuenta != string.Empty).Sum(o => o.d_Cambio.Value);
                                    TotHaberC = TempDiarioInsertarImportaciones.Where(p => p.v_Naturaleza == "H" && p.v_NroCuenta != string.Empty).Sum(o => o.d_Cambio.Value);
                                    _diarioDto.d_TotalDebe = Utils.Windows.DevuelveValorRedondeado(TotDebe, 2);
                                    _diarioDto.d_TotalHaber = Utils.Windows.DevuelveValorRedondeado(TotHaber, 2);
                                    _diarioDto.d_TotalDebeCambio = Utils.Windows.DevuelveValorRedondeado(TotDebeC, 2);
                                    _diarioDto.d_TotalHaberCambio = Utils.Windows.DevuelveValorRedondeado(TotHaberC, 2);
                                    _diarioDto.d_DiferenciaDebe = Utils.Windows.DevuelveValorRedondeado(TotDebe - TotHaber, 2);
                                    _diarioDto.d_DiferenciaHaber = Utils.Windows.DevuelveValorRedondeado(TotDebeC - TotHaberC, 2);
                                    if (TempDiarioInsertarImportaciones.Where(a => a.v_NroCuenta != String.Empty).ToList().Any())
                                    {
                                        _objDiarioBL.InsertarDiario(ref pobjOperationResult, _diarioDto, Globals.ClientSession.GetAsList(), TempDiarioInsertarImportaciones.Where(a => a.v_NroCuenta != String.Empty).ToList(), (int)TipoMovimientoTesoreria.Egreso, true);
                                    }

                                }

                            }
                            #endregion


                            ////}
                            #endregion
                            Globals.ProgressbarStatus.i_Progress++;
                        }
                    }
                    ts.Complete();
                }



            }
            catch (Exception ex)
            {

                pobjOperationResult.Success = 0;



                pobjOperationResult.AdditionalInformation = "DiarioBL.RegeneraDiariosImportacion() \nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) +
                                                            '\n' + ErrorImportacion;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);

            }
        }

        /// <summary>
        /// Genera la consulta para el reporte del libro Diario
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstrIdDiario"></param>
        /// <returns></returns>
        //public List<ReporteAsientoDiario> ReporteAsientoDiario(ref OperationResult pobjOperationResult,
        //    string pstrIdDiario)
        //{
        //    try
        //    {
        //        using (var dbContext = new SAMBHSEntitiesModelWin())
        //        {
        //            var diario = dbContext.diario.FirstOrDefault(p => p.v_IdDiario == pstrIdDiario);
        //            if (diario == null) throw new Exception("Diario ya no existe!");

        //            var diarioDetalles = diario.diariodetalle.Where(p => p.i_Eliminado == 0).ToList();
        //            if (!diarioDetalles.Any()) return null;
        //            string fechaVencimiento = string.Empty;

        //            var result = diarioDetalles.Select(p =>
        //            {
        //                var cliente = dbContext.cliente.FirstOrDefault(o => o.v_IdCliente == p.v_IdCliente);
        //                var documento = dbContext.documento.FirstOrDefault(o => o.i_CodigoDocumento == p.i_IdTipoDocumento.Value);
        //                var tipoDiario = dbContext.documento.FirstOrDefault(o => o.i_CodigoDocumento == p.diario.i_IdTipoDocumento.Value);

        //                #region Obtener Fecha Vencimiento si es letra por cobrar o pagar
        //                if (p.i_IdTipoDocumento.Value == 333 || p.i_IdTipoDocumento.Value == 334)
        //                {
        //                    var documentoSplit = p.v_NroDocumento.Split('-');

        //                    if (p.i_IdTipoDocumento.Value == 333)
        //                    {
        //                        if (documentoSplit.Count() == 3 || documentoSplit.Count() == 2)
        //                        {
        //                            var correlativo = documentoSplit.Count() == 3 ? documentoSplit[1] + "-" + documentoSplit[2] : documentoSplit[1];
        //                            var serie = documentoSplit[0];
        //                            var letradetalle = dbContext.letrasdetalle.FirstOrDefault(x => x.v_Serie == serie && x.v_Correlativo == correlativo);
        //                            fechaVencimiento = letradetalle != null && letradetalle.t_FechaVencimiento.HasValue ? letradetalle.t_FechaVencimiento.Value.ToShortDateString() : p.t_Fecha.Value.ToShortDateString();
        //                        }
        //                    }

        //                    if (p.i_IdTipoDocumento.Value == 334)
        //                    {
        //                        if (documentoSplit.Count() == 2)
        //                        {
        //                            var correlativo = documentoSplit[1];
        //                            var serie = documentoSplit[0];

        //                            var letradetalle = dbContext.letraspagardetalle.FirstOrDefault(x => x.v_Serie == serie && x.v_Correlativo == correlativo);
        //                            fechaVencimiento = letradetalle != null ? letradetalle.t_FechaVencimiento.Value.ToShortDateString() : p.t_Fecha.Value.ToShortDateString();
        //                        }
        //                    }

        //                }
        //                #endregion

        //                return new ReporteAsientoDiario
        //                    {
        //                        Cuenta = p.v_NroCuenta,
        //                        TipoCambio = p.diario.d_TipoCambio ?? 0,
        //                        Anexo = cliente != null ? (cliente.v_ApePaterno + " " + cliente.v_ApeMaterno + " " + cliente.v_PrimerNombre + " " + cliente.v_RazonSocial).Trim() : string.Empty,
        //                        Debe = p.v_Naturaleza == "D" ? p.diario.i_IdMoneda.HasValue && p.diario.i_IdMoneda.Value == 1 ? p.d_Importe ?? 0 : p.d_Cambio ?? 0 : 0,
        //                        Haber = p.v_Naturaleza == "H" ? p.diario.i_IdMoneda.HasValue && p.diario.i_IdMoneda.Value == 1 ? p.d_Importe ?? 0 : p.d_Cambio ?? 0 : 0,
        //                        Documento = documento != null ? documento.v_Siglas + " " + p.v_NroDocumento : string.Empty,
        //                        FechaAsiento = p.diario.t_Fecha.HasValue ? p.diario.t_Fecha.Value.ToShortDateString() : string.Empty,
        //                        FechaVencimiento = p.i_IdTipoDocumento.Value == 333 || p.i_IdTipoDocumento.Value == 334 ? fechaVencimiento : string.Empty,
        //                        GlosaAsiento = p.diario.v_Glosa,
        //                        MonedaExtranjera = p.diario.i_IdMoneda.HasValue ? p.diario.i_IdMoneda.Value == 2 ? p.d_Importe ?? 0 : 0 : 0,
        //                        NombreAsiento = p.diario.v_Nombre,
        //                        NroAsiento = tipoDiario != null ? "ASIENTOS DE DIARIO N°: " + tipoDiario.v_Siglas + " " + p.diario.v_Mes + p.diario.v_Correlativo : string.Empty
        //                    };
        //            });

        //            pobjOperationResult.Success = 1;
        //            return result != null ? result.ToList() : null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        pobjOperationResult.Success = 0;
        //        pobjOperationResult.AdditionalInformation = "DiarioBL.ReporteAsientoDiario() \nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
        //        pobjOperationResult.ErrorMessage = ex.Message;
        //        pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
        //        Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
        //        return null;
        //    }
        //}




        public List<ReporteAsientoDiario> ReporteAsientoDiario(ref OperationResult pobjOperationResult,
         int TipoDocumento, string CorrelativoInicial, string CorrelativoFinal, string Periodo, string Mes, string SoloCorrelativo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    var clientes = dbContext.cliente.Where(o => o.i_Eliminado == 0).ToList();
                    var documentosDetalle = dbContext.documento.Where(o => o.i_Eliminado == 0).ToList();
                    var letrasdetalle = dbContext.letrasdetalle.Where(o => o.i_Eliminado == 0).ToList();
                    var letraspagardetalle = dbContext.letraspagardetalle.Where(o => o.i_Eliminado == 0).ToList();
                    List<ReporteAsientoDiario> ListadoDiarios = new List<ReporteAsientoDiario>();

                    if (CorrelativoInicial == CorrelativoFinal)
                    {
                        ListadoDiarios = (from a in dbContext.diariodetalle

                                          join b in dbContext.diario on new { d = a.v_IdDiario, eliminado = 0 } equals new { d = b.v_IdDiario, eliminado = b.i_Eliminado.Value } into b_join
                                          from b in b_join.DefaultIfEmpty()

                                          join c in dbContext.documento on new { td = b.i_IdTipoDocumento.Value, eliminado = 0 } equals new { td = c.i_CodigoDocumento, eliminado = c.i_Eliminado.Value } into c_join
                                          from c in c_join.DefaultIfEmpty()

                                          join d in dbContext.documento on new { tdd = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { tdd = d.i_CodigoDocumento, eliminado = d.i_Eliminado.Value } into d_join
                                          from d in d_join.DefaultIfEmpty()

                                          join e in dbContext.documento on new { tdr = a.i_IdTipoDocumentoRef.Value, eliminado = 0 } equals new { tdr = e.i_CodigoDocumento, eliminado = e.i_Eliminado.Value } into e_join
                                          from e in e_join.DefaultIfEmpty()
                                          where b.i_IdTipoDocumento == TipoDocumento && b.v_Periodo == Periodo && a.i_Eliminado == 0 && b.v_Mes == Mes

                                          && b.v_Correlativo == SoloCorrelativo

                                          select new
                                          {

                                              Cuenta = a.v_NroCuenta,
                                              TipoCambio = b.d_TipoCambio ?? 0,
                                              Debe = a.v_Naturaleza == "D" ? a.diario.i_IdMoneda.HasValue && a.diario.i_IdMoneda.Value == 1 ? a.d_Importe ?? 0 : a.d_Cambio ?? 0 : 0,
                                              Haber = a.v_Naturaleza == "H" ? a.diario.i_IdMoneda.HasValue && a.diario.i_IdMoneda.Value == 1 ? a.d_Importe ?? 0 : a.d_Cambio ?? 0 : 0,
                                              //Documento = d != null ? d.v_Siglas + " " + a.v_NroDocumento : string.Empty,
                                              Documento = a.v_NroDocumento,
                                              GlosaAsiento = a.diario.v_Glosa,
                                              MonedaExtranjera = a.diario.i_IdMoneda.HasValue ? a.diario.i_IdMoneda.Value == 2 ? a.d_Importe ?? 0 : 0 : 0,
                                              NombreAsiento = a.diario.v_Nombre,
                                              NroAsiento = c != null ? "ASIENTOS DE DIARIO N°: " + c.v_Siglas + " " + b.v_Mes + b.v_Correlativo : string.Empty,
                                              IdAnexo = a.v_IdCliente,
                                              FechaAsiento = b.t_Fecha,
                                              TipoDocumentoDetalle = a.i_IdTipoDocumento,
                                              v_NroDocumento = a.v_NroDocumento,
                                              CorrelativoInt = b.v_Mes + b.v_Correlativo,
                                              FechaDetalle = a.t_Fecha,
                                              IdDiario = b.v_IdDiario,
                                              i_IdCentroCosto = a.i_IdCentroCostos,
                                              SiglasDocDetalle = d.v_Siglas,
                                              SiglasDocumentoRefDetalle = e.v_Siglas,
                                              NroDocRef = a.v_NroDocumentoRef,
                                              Analisis = a.v_Analisis,
                                              MonedaOperacion = b.i_IdMoneda == (int)Currency.Soles ? "SOLES" : "DÓLARES",
                                              v_IdDiarioDetalle = a.v_IdDiarioDetalle,
                                          }).ToList().Select(o =>
                                          {

                                              var Cliente = clientes.Where(x => x.v_IdCliente == o.IdAnexo).FirstOrDefault();
                                              string fechaVencimiento = "";
                                              #region Obtener Fecha Vencimiento si es letra por cobrar o pagar
                                              if (o.TipoDocumentoDetalle.Value == 333 || o.TipoDocumentoDetalle.Value == 334)
                                              {
                                                  var documentoSplit = o.v_NroDocumento.Split('-');

                                                  if (o.TipoDocumentoDetalle.Value == 333)
                                                  {
                                                      if (documentoSplit.Count() == 3 || documentoSplit.Count() == 2)
                                                      {
                                                          var correlativo = documentoSplit.Count() == 3 ? documentoSplit[1] + "-" + documentoSplit[2] : documentoSplit[1];
                                                          var serie = documentoSplit[0];
                                                          var letradetalle = letrasdetalle.FirstOrDefault(x => x.v_Serie == serie && x.v_Correlativo == correlativo);
                                                          fechaVencimiento = letradetalle != null && letradetalle.t_FechaVencimiento.HasValue ? letradetalle.t_FechaVencimiento.Value.ToShortDateString() : o.FechaDetalle.Value.ToShortDateString();
                                                      }
                                                  }

                                                  if (o.TipoDocumentoDetalle.Value == 334)
                                                  {
                                                      if (documentoSplit.Count() == 2)
                                                      {
                                                          var correlativo = documentoSplit[1];
                                                          var serie = documentoSplit[0];

                                                          var letradetalle = letraspagardetalle.FirstOrDefault(x => x.v_Serie == serie && x.v_Correlativo == correlativo);
                                                          fechaVencimiento = letradetalle != null ? letradetalle.t_FechaVencimiento.Value.ToShortDateString() : o.FechaDetalle.Value.ToShortDateString();
                                                      }
                                                  }

                                              }
                                              #endregion


                                              return new ReporteAsientoDiario
                                              {

                                                  Cuenta = o.Cuenta,
                                                  TipoCambio = o.TipoCambio,
                                                  Anexo = Cliente != null ? (Cliente.v_ApePaterno + " " + Cliente.v_ApeMaterno + " " + Cliente.v_PrimerNombre + " " + Cliente.v_RazonSocial).Trim() : string.Empty,
                                                  Debe = o.Debe,
                                                  Haber = o.Haber,
                                                  Documento = o.Documento,
                                                  FechaAsiento = o.FechaAsiento.HasValue ? o.FechaAsiento.Value.ToShortDateString() : string.Empty,
                                                  FechaVencimiento = o.TipoDocumentoDetalle == 333 || o.TipoDocumentoDetalle == 334 ? fechaVencimiento : string.Empty,   //p.i_IdTipoDocumento.Value == 333 || p.i_IdTipoDocumento.Value == 334 ? fechaVencimiento : string.Empty,
                                                  GlosaAsiento = o.GlosaAsiento,
                                                  NombreAsiento = o.NombreAsiento,
                                                  NroAsiento = o.NroAsiento,
                                                  CorrelativoInt = int.Parse(o.CorrelativoInt),
                                                  Grupo = o.IdDiario,
                                                  MonedaExtranjera = o.MonedaExtranjera,
                                                  CodigoAnexo = Cliente != null ? Cliente.v_NroDocIdentificacion : "",
                                                  i_IdCentroCosto = o.i_IdCentroCosto,
                                                  SiglasDocumentoDetalle = o.SiglasDocDetalle,
                                                  SiglasDocumentoRefDetalle = o.SiglasDocumentoRefDetalle,
                                                  NroDocRef = o.NroDocRef,
                                                  Analisis = o.Analisis,
                                                  MonedaOperacion = o.MonedaOperacion,
                                                  v_IdDiarioDetalle = o.v_IdDiarioDetalle,

                                              };

                                          }).ToList();

                    }
                    else
                    {
                        ListadoDiarios = (from a in dbContext.diariodetalle

                                          join b in dbContext.diario on new { d = a.v_IdDiario, eliminado = 0 } equals new { d = b.v_IdDiario, eliminado = b.i_Eliminado.Value } into b_join
                                          from b in b_join.DefaultIfEmpty()

                                          join c in dbContext.documento on new { td = b.i_IdTipoDocumento.Value, eliminado = 0 } equals new { td = c.i_CodigoDocumento, eliminado = c.i_Eliminado.Value } into c_join
                                          from c in c_join.DefaultIfEmpty()

                                          join d in dbContext.documento on new { tdd = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { tdd = d.i_CodigoDocumento, eliminado = d.i_Eliminado.Value } into d_join
                                          from d in d_join.DefaultIfEmpty()

                                          join e in dbContext.documento on new { tdr = a.i_IdTipoDocumentoRef.Value, eliminado = 0 } equals new { tdr = e.i_CodigoDocumento, eliminado = e.i_Eliminado.Value } into e_join
                                          from e in e_join.DefaultIfEmpty()

                                          where b.i_IdTipoDocumento == TipoDocumento && b.v_Periodo == Periodo && a.i_Eliminado == 0 && b.v_Mes == Mes

                                          select new
                                          {

                                              Cuenta = a.v_NroCuenta,
                                              TipoCambio = b.d_TipoCambio ?? 0,
                                              Debe = a.v_Naturaleza == "D" ? a.diario.i_IdMoneda.HasValue && a.diario.i_IdMoneda.Value == 1 ? a.d_Importe ?? 0 : a.d_Cambio ?? 0 : 0,
                                              Haber = a.v_Naturaleza == "H" ? a.diario.i_IdMoneda.HasValue && a.diario.i_IdMoneda.Value == 1 ? a.d_Importe ?? 0 : a.d_Cambio ?? 0 : 0,
                                              //Documento = d != null ? d.v_Siglas + " " + a.v_NroDocumento : string.Empty,
                                              Documento = a.v_NroDocumento,
                                              GlosaAsiento = a.diario.v_Glosa,
                                              MonedaExtranjera = a.diario.i_IdMoneda.HasValue ? a.diario.i_IdMoneda.Value == 2 ? a.d_Importe ?? 0 : 0 : 0,
                                              NombreAsiento = a.diario.v_Nombre,
                                              NroAsiento = c != null ? "ASIENTOS DE DIARIO N°: " + c.v_Siglas + " " + b.v_Mes + b.v_Correlativo : string.Empty,
                                              IdAnexo = a.v_IdCliente,
                                              FechaAsiento = b.t_Fecha,
                                              TipoDocumentoDetalle = a.i_IdTipoDocumento,
                                              v_NroDocumento = a.v_NroDocumento,
                                              CorrelativoInt = b.v_Mes + b.v_Correlativo,
                                              FechaDetalle = a.t_Fecha,
                                              IdDiario = b.v_IdDiario,
                                              i_IdCentroCosto = a.i_IdCentroCostos,
                                              SiglasDocDetalle = d.v_Siglas,
                                              SiglasDocumentoRefDetalle = e == null ? "" : e.v_Siglas,
                                              NroDocRef = a.v_NroDocumentoRef,
                                              Analisis = a.v_Analisis,
                                              MonedaOperacion = b.i_IdMoneda == (int)Currency.Soles ? "SOLES" : "DÓLARES",
                                              v_IdDiarioDetalle = a.v_IdDiarioDetalle,
                                          }).ToList().Select(o =>
                                              {

                                                  var Cliente = clientes.Where(x => x.v_IdCliente == o.IdAnexo).FirstOrDefault();
                                                  string fechaVencimiento = "";
                                                  #region Obtener Fecha Vencimiento si es letra por cobrar o pagar
                                                  if (o.TipoDocumentoDetalle.Value == 333 || o.TipoDocumentoDetalle.Value == 334)
                                                  {
                                                      var documentoSplit = o.v_NroDocumento.Split('-');

                                                      if (o.TipoDocumentoDetalle.Value == 333)
                                                      {
                                                          if (documentoSplit.Count() == 3 || documentoSplit.Count() == 2)
                                                          {
                                                              var correlativo = documentoSplit.Count() == 3 ? documentoSplit[1] + "-" + documentoSplit[2] : documentoSplit[1];
                                                              var serie = documentoSplit[0];
                                                              var letradetalle = letrasdetalle.FirstOrDefault(x => x.v_Serie == serie && x.v_Correlativo == correlativo);
                                                              fechaVencimiento = letradetalle != null && letradetalle.t_FechaVencimiento.HasValue ? letradetalle.t_FechaVencimiento.Value.ToShortDateString() : o.FechaDetalle.Value.ToShortDateString();
                                                          }
                                                      }

                                                      if (o.TipoDocumentoDetalle.Value == 334)
                                                      {
                                                          if (documentoSplit.Count() == 2)
                                                          {
                                                              var correlativo = documentoSplit[1];
                                                              var serie = documentoSplit[0];

                                                              var letradetalle = letraspagardetalle.FirstOrDefault(x => x.v_Serie == serie && x.v_Correlativo == correlativo);
                                                              fechaVencimiento = letradetalle != null ? letradetalle.t_FechaVencimiento.Value.ToShortDateString() : o.FechaDetalle.Value.ToShortDateString();
                                                          }
                                                      }

                                                  }
                                                  #endregion


                                                  return new ReporteAsientoDiario
                                               {

                                                   Cuenta = o.Cuenta,
                                                   TipoCambio = o.TipoCambio,
                                                   Anexo = Cliente != null ? (Cliente.v_ApePaterno + " " + Cliente.v_ApeMaterno + " " + Cliente.v_PrimerNombre + " " + Cliente.v_RazonSocial).Trim() : string.Empty,
                                                   Debe = o.Debe,
                                                   Haber = o.Haber,
                                                   Documento = o.Documento,
                                                   FechaAsiento = o.FechaAsiento.HasValue ? o.FechaAsiento.Value.ToShortDateString() : string.Empty,
                                                   FechaVencimiento = o.TipoDocumentoDetalle == 333 || o.TipoDocumentoDetalle == 334 ? fechaVencimiento : string.Empty,   //p.i_IdTipoDocumento.Value == 333 || p.i_IdTipoDocumento.Value == 334 ? fechaVencimiento : string.Empty,
                                                   GlosaAsiento = o.GlosaAsiento,
                                                   NombreAsiento = o.NombreAsiento,
                                                   NroAsiento = o.NroAsiento,
                                                   CorrelativoInt = int.Parse(o.CorrelativoInt),
                                                   Grupo = o.IdDiario,
                                                   MonedaExtranjera = o.MonedaExtranjera,
                                                   CodigoAnexo = Cliente != null ? Cliente.v_NroDocIdentificacion : "",
                                                   i_IdCentroCosto = o.i_IdCentroCosto,
                                                   SiglasDocumentoDetalle = o.SiglasDocDetalle,
                                                   SiglasDocumentoRefDetalle = o.SiglasDocumentoRefDetalle,
                                                   NroDocRef = o.NroDocRef,
                                                   Analisis = o.Analisis,
                                                   MonedaOperacion = o.MonedaOperacion,
                                                   v_IdDiarioDetalle = o.v_IdDiarioDetalle,
                                               };

                                              }).ToList();
                    }


                    int CorreInicial = int.Parse(CorrelativoInicial);
                    int CorreFinal = int.Parse(CorrelativoFinal);
                    pobjOperationResult.Success = 1;
                    ListadoDiarios = ListadoDiarios.Where(o => o.CorrelativoInt >= CorreInicial && o.CorrelativoInt <= CorreFinal).OrderBy(o => o.NroAsiento).ThenBy(o => o.v_IdDiarioDetalle).ToList();
                    return ListadoDiarios;


                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "DiarioBL.ReporteAsientoDiario() \nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }



        #region Destinos
        public List<destinoDto> ObtenerDestinosPorCuenta(string strCuentaOrigen)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    return dbContext.destino.Where(p => p.v_CuentaOrigen == strCuentaOrigen && p.i_Eliminado == 0).ToList().ToDTOs();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Bandeja
        public List<diarioDto> ListarDiarios(ref OperationResult pobjOperationResult, string _strFilterExpression, DateTime F_Ini, DateTime F_Fin, string Orden)
        {
            try
            {
                F_Ini = F_Ini.Date;
                F_Fin = DateTime.Parse(F_Fin.Day + "/" + F_Fin.Month + "/" + F_Fin.Year + " 23:59");

                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var diarios = (from d in dbContext.diario

                                   join J1 in dbContext.systemuser on new { id = d.i_InsertaIdUsuario.Value }
                                                                    equals new { id = J1.i_SystemUserId } into J1_join
                                   from J1 in J1_join.DefaultIfEmpty()

                                   join J2 in dbContext.systemuser on new { id = d.i_ActualizaIdUsuario.Value }
                                                                    equals new { id = J2.i_SystemUserId } into J2_join
                                   from J2 in J2_join.DefaultIfEmpty()

                                   join J3 in dbContext.datahierarchy on new { i_IdTipoDocumento = d.i_IdTipoComprobante.Value, b = 56 }
                                                                 equals new { i_IdTipoDocumento = J3.i_ItemId, b = J3.i_GroupId } into J3_join
                                   from J3 in J3_join.DefaultIfEmpty()

                                   join J4 in dbContext.documento on new { i_IdTipoDocumento = d.i_IdTipoDocumento.Value }
                                                                 equals new { i_IdTipoDocumento = J4.i_CodigoDocumento } into J4_join
                                   from J4 in J4_join.DefaultIfEmpty()

                                   where d.i_Eliminado == 0 && d.t_Fecha >= F_Ini && d.t_Fecha <= F_Fin

                                   select new diarioDto
                                   {
                                       v_Periodo = d.v_Periodo,
                                       v_Mes = d.v_Mes,
                                       v_Correlativo = d.v_Correlativo,
                                       v_IdDiario = d.v_IdDiario,
                                       i_IdTipoComprobante = d.i_IdTipoComprobante,
                                       i_IdTipoDocumento = d.i_IdTipoDocumento,
                                       NroRegistro = d.v_Mes + " - " + d.v_Correlativo,
                                       v_UsuarioCreacion = J1.v_UserName,
                                       v_UsuarioModificacion = J2.v_UserName,
                                       v_Nombre = d.v_Nombre,
                                       t_ActualizaFecha = d.t_ActualizaFecha,
                                       t_InsertaFecha = d.t_InsertaFecha,
                                       TipoComprobante = J3.v_Value1,
                                       TipoDocumento = J4.v_Siglas,
                                       v_IdDocumentoReferencia = d.v_IdDocumentoReferencia
                                   });

                    if (!string.IsNullOrEmpty(_strFilterExpression))
                    {
                        diarios = diarios.Where(_strFilterExpression);
                    }

                    if (!string.IsNullOrEmpty(Orden))
                    {
                        diarios = diarios.OrderBy(Orden);
                    }
                    pobjOperationResult.Success = 1;
                    return diarios.ToList();
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

        public List<diarioDto> DevolverDataJerarquizadaDiario_DiarioDetalle(ref OperationResult pobjOperationResult, DateTime FechaIni, DateTime FechaFin)
        {
            try
            {
                SAMBHSEntitiesModelWin dbcontext = new SAMBHSEntitiesModelWin();
                diarioDto objdiarioDto;
                List<diarioDto> ListaDiarioDto = new List<diarioDto>();
                diariodetalleDto objdiariodetalleDto;
                List<diariodetalleDto> ListaDiarioDetalleDto;


                var Query1 = (from a in dbcontext.diario
                              where a.t_Fecha > FechaIni && a.t_Fecha < FechaFin
                              select a).ToList();

                var Query2 = (from a in dbcontext.diariodetalle
                              where a.t_Fecha > FechaIni && a.t_Fecha < FechaFin
                              select a).ToList();

                //Armar mi clase Jerárquica
                foreach (var item in Query1)
                {
                    //Cabecera
                    objdiarioDto = new diarioDto();
                    objdiarioDto.v_IdDiario = item.v_IdDiario;
                    objdiarioDto.v_IdDocumentoReferencia = item.v_IdDocumentoReferencia;
                    objdiarioDto.v_Periodo = item.v_Periodo;
                    objdiarioDto.v_Mes = item.v_Mes;
                    objdiarioDto.i_IdTipoDocumento = item.i_IdTipoDocumento;
                    objdiarioDto.v_Correlativo = item.v_Correlativo;
                    objdiarioDto.d_TipoCambio = item.d_TipoCambio;
                    objdiarioDto.i_IdTipoComprobante = item.i_IdTipoComprobante;
                    objdiarioDto.i_IdMoneda = item.i_IdMoneda;
                    objdiarioDto.i_IdEstado = item.i_IdEstado;
                    objdiarioDto.v_Nombre = item.v_Nombre;
                    objdiarioDto.v_Glosa = item.v_Glosa;
                    objdiarioDto.t_Fecha = item.t_Fecha;
                    objdiarioDto.d_TotalDebe = item.d_TotalDebe;
                    objdiarioDto.d_TotalHaber = item.d_TotalHaber;
                    objdiarioDto.d_TotalDebeCambio = item.d_TotalDebeCambio;
                    objdiarioDto.d_TotalHaberCambio = item.d_TotalHaberCambio;
                    objdiarioDto.d_DiferenciaDebe = item.d_DiferenciaDebe;
                    objdiarioDto.d_DiferenciaHaber = item.d_DiferenciaHaber;

                    //Detalle
                    ListaDiarioDetalleDto = new List<diariodetalleDto>();
                    var DiarioDetalle = Query2.FindAll(p => p.v_IdDiario == objdiarioDto.v_IdDiario);

                    foreach (var item1 in DiarioDetalle)
                    {

                        objdiariodetalleDto = new diariodetalleDto();
                        objdiariodetalleDto.v_IdDiarioDetalle = item1.v_IdDiarioDetalle;
                        objdiariodetalleDto.v_IdDiario = item1.v_IdDiario;
                        objdiariodetalleDto.v_NroCuenta = item1.v_NroCuenta;
                        objdiariodetalleDto.v_Naturaleza = item1.v_Naturaleza;
                        objdiariodetalleDto.d_Importe = item1.d_Importe;
                        objdiariodetalleDto.d_Cambio = item1.d_Cambio;
                        objdiariodetalleDto.i_IdCentroCostos = item1.i_IdCentroCostos;
                        objdiariodetalleDto.v_IdCliente = item1.v_IdCliente;
                        objdiariodetalleDto.i_IdTipoDocumento = item1.i_IdTipoDocumento;
                        objdiariodetalleDto.v_NroDocumento = item1.v_NroDocumento;
                        objdiariodetalleDto.i_IdTipoDocumentoRef = item1.i_IdTipoDocumentoRef;
                        objdiariodetalleDto.v_NroDocumentoRef = item1.v_NroDocumentoRef;
                        objdiariodetalleDto.v_Analisis = item1.v_Analisis;
                        objdiariodetalleDto.t_Fecha = item1.t_Fecha;
                        objdiariodetalleDto.v_OrigenDestino = item1.v_OrigenDestino;
                        objdiariodetalleDto.v_Pedido = item1.v_Pedido;

                        ListaDiarioDetalleDto.Add(objdiariodetalleDto);
                    }

                    objdiarioDto.DiarioDetallePersonalizado = ListaDiarioDetalleDto;
                    ListaDiarioDto.Add(objdiarioDto);
                }

                return ListaDiarioDto;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region Procesos Para Calcular los Destinos

        /// <summary>
        /// Agrega al temporal de insertar del metodo InsertarDiario los destinos para q se inserten en la tabla. Primero elimina los destinos anteriores y los re-agrega.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pTempInsertar"></param>
        /// <param name="diarioCabecera"></param>
        /// <param name="ptemDestinosDs"></param>
        /// <returns>Retorna una lista de diariodetalleDto.</returns>
        public List<diariodetalleDto> ProcesaDestinosDiario(ref OperationResult pobjOperationResult,
            List<diariodetalleDto> pTempInsertar, diarioDto diarioCabecera, List<destino> ptemDestinosDs)
        {
            var idDiario = diarioCabecera.v_IdDiario;
            try
            {
                pobjOperationResult.Success = 1;
                var listaRetorno = new List<diariodetalleDto>();
                var esAsientoAjuste = diarioCabecera.i_IdTipoComprobante.HasValue
                    && diarioCabecera.i_IdTipoComprobante.Value == 6;

                if (diarioCabecera.i_IdTipoComprobante == 5) return new List<diariodetalleDto>();// Cuando se inserta un Asiento de Cierre no se generan destinos
                var agrupadoProcesar =
                    pTempInsertar.GroupBy(
                        p =>
                            new
                            {
                                cuenta = p.v_NroCuenta,
                                cc = p.i_IdCentroCostos,
                                anexo = p.v_IdCliente,
                                naturaleza = p.v_Naturaleza,
                                tipoDoc = p.i_IdTipoDocumento,
                                nroDoc = p.v_NroDocumento
                            });

                #region Generación de destinos
                foreach (var filaOrigen in agrupadoProcesar)
                {
                    var filaProcesar = filaOrigen;
                    var destinosFila = ptemDestinosDs
                        .Where(p => p.v_CuentaOrigen.Equals(filaProcesar.Key.cuenta) && p.i_Eliminado == 0 && p.v_Periodo.Equals(periodo)).ToList();

                    if (!destinosFila.Any()) continue;
                    {
                        var preListaRetorno = new List<diariodetalleDto>();
                        var importeOrigen = filaOrigen.Sum(p => p.d_Importe ?? 0);
                        var cambioOrigen = !esAsientoAjuste ? filaOrigen.Sum(p => p.d_Cambio ?? 0) : 0;
                        foreach (var destino in destinosFila)
                        {
                            var filaOrigenModelo = filaOrigen.FirstOrDefault();
                            if (filaOrigenModelo == null) continue;

                            var porcentajeDestino = destino.i_Porcentaje ?? 100;
                            var importeDestino = Utils.Windows.DevuelveValorRedondeado((importeOrigen * porcentajeDestino) / 100, 2);
                            var cambioDestino = Utils.Windows.DevuelveValorRedondeado(cambioOrigen > 0 ? (cambioOrigen * porcentajeDestino) / 100 : 0, 2);

                            #region Preparacion de pre-lista
                            preListaRetorno.Add(new diariodetalleDto
                            {
                                d_Importe = importeDestino,
                                d_Cambio = cambioDestino,
                                i_IdCentroCostos = filaOrigen.Key.cc,
                                i_IdTipoDocumento = filaOrigen.Key.tipoDoc,
                                v_NroDocumento = filaOrigen.Key.nroDoc,
                                i_EsDestino = "1",
                                v_Naturaleza = filaOrigen.Key.naturaleza.Trim(),
                                v_NroCuenta = destino.v_CuentaDestino,
                                t_Fecha = filaOrigenModelo.t_Fecha,
                                i_IdTipoDocumentoRef = -1,
                                v_NroDocumentoRef = string.Empty,
                                NroCuentaOrigen = destino.v_CuentaOrigen,
                                v_IdCliente = filaOrigen.Key.anexo
                            });

                            preListaRetorno.Add(new diariodetalleDto
                            {
                                d_Importe = importeDestino,
                                d_Cambio = cambioDestino,
                                i_IdCentroCostos = filaOrigen.Key.cc,
                                i_EsDestino = "1",
                                v_Naturaleza = filaOrigen.Key.naturaleza.Trim().Equals("D") ? "H" : "D",
                                v_NroCuenta = destino.v_CuentaTransferencia,
                                i_IdTipoDocumento = filaOrigen.Key.tipoDoc,
                                v_NroDocumento = filaOrigen.Key.nroDoc,
                                t_Fecha = filaOrigenModelo.t_Fecha,
                                i_IdTipoDocumentoRef = -1,
                                v_NroDocumentoRef = string.Empty,
                                NroCuentaOrigen = destino.v_CuentaOrigen,
                                v_IdCliente = filaOrigen.Key.anexo
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

                //pobjOperationResult.Success = 1;
                return listaRetorno;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "DiarioBL.ProcesaDestinosDiario() \nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) + '\n' + idDiario;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public void RecalcularDestinosDiario(ref OperationResult pobjOperationResult, int Periodo, int mes)
        {
            string idDiarioException = string.Empty;
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var objSecuentialBl = new SecuentialBL();
                        var diarios = dbContext.diario.Where(p => p.t_Fecha.Value.Year == Periodo && p.i_Eliminado == 0 && (mes == -1 || p.t_Fecha.Value.Month == mes)).ToList();
                        Globals.ProgressbarStatus.i_TotalProgress = diarios.Count();
                        var destinos = dbContext.destino.ToList();

                        var ids = diarios.Select(p => p.v_IdDiario).ToList();

                        var destinosAnteriores =
                               dbContext.diariodetalle.Where(p => ids.Contains(p.v_IdDiario) && p.i_EsDestino.Equals("1"));

                        foreach (var destino in destinosAnteriores)
                        {
                            dbContext.DeleteObject(destino);
                        }
                        dbContext.SaveChanges();

                        foreach (var Diario in diarios.AsParallel())
                        {
                            idDiarioException = Diario.v_IdDiario;
                            var pTempInsertar = Diario.diariodetalle.Where(p => p.i_Eliminado == 0).ToList().ToDTOs();

                            var DestinosXIngresar = ProcesaDestinosDiario(ref pobjOperationResult, pTempInsertar, Diario.ToDTO(), destinos);
                            if (pobjOperationResult.Success == 0) break;

                            #region Inserta Destinos ReCalculados
                            foreach (var diariodetalleDto in DestinosXIngresar)
                            {
                                var objEntitytesoreriaDetalle = diariodetalleDto.ToEntity();
                                var secuentialId = objSecuentialBl.GetNextSecuentialId(int.Parse(Globals.ClientSession.GetAsList()[0]), 60);
                                var newIdDiarioDetalle = Utils.GetNewId(int.Parse(Globals.ClientSession.GetAsList()[0]), secuentialId, "XJ");
                                objEntitytesoreriaDetalle.v_IdDiarioDetalle = newIdDiarioDetalle;
                                objEntitytesoreriaDetalle.v_IdDiario = Diario.v_IdDiario;
                                objEntitytesoreriaDetalle.t_InsertaFecha = DateTime.Now;
                                objEntitytesoreriaDetalle.i_InsertaIdUsuario = Int32.Parse(Globals.ClientSession.GetAsList()[2]);
                                objEntitytesoreriaDetalle.i_Eliminado = 0;
                                dbContext.AddTodiariodetalle(objEntitytesoreriaDetalle);
                            }
                            #endregion

                            Globals.ProgressbarStatus.i_Progress++;
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
                pobjOperationResult.AdditionalInformation = "DiarioBL.RecalcularDestinosDiario() \nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')) +
                                                            '\n' + idDiarioException;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }
        #endregion

        #region AsientoInventario


        public List<diarioDto> BuscarDiarioInventario(string mes, string Periodo, int TipoDocumento)
        {
            List<diarioDto> ListaDiario = new List<diarioDto>();
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                var Diario = (from n in dbContext.diario

                              where n.i_Eliminado == 0 && n.v_Mes == mes && n.v_Periodo == Periodo && n.i_IdTipoDocumento == TipoDocumento

                              && n.v_IdDocumentoReferencia != null && n.v_IdDocumentoReferencia.Substring(0, 2) == "AF"

                              select n).ToList();
                if (Diario.Count() != 0)
                    ListaDiario = Diario.ToDTOs();
            }
            return ListaDiario;

        }

        public List<diarioDto> BuscarDiarioInventarioPorDocReferencia(string DocReferencia)
        {


            List<diarioDto> ListaDiario = new List<diarioDto>();
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                var Diario = (from n in dbContext.diario

                              where n.i_Eliminado == 0 && n.v_IdDocumentoReferencia.Trim() == DocReferencia.Trim()

                              select n).ToList();
                if (Diario.Count() != 0)
                    ListaDiario = Diario.ToDTOs();
            }
            return ListaDiario;

        }

        #endregion

        public static void EmparejarDiarios(ref OperationResult pobjOperationResult, int mesProceso)
        {
            try
            {
                int periodo = Globals.ClientSession.i_Periodo ?? DateTime.Now.Year;
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var diarios = dbContext.diario.Where(p => p.t_Fecha.Value.Year == periodo && p.t_Fecha.Value.Month == mesProceso
                        && p.v_IdDocumentoReferencia != null && p.v_IdDocumentoReferencia != "" && p.i_Eliminado == 0).ToList();

                    if (!diarios.Any()) return;

                    foreach (var diario in diarios)
                    {
                        switch (diario.v_IdDocumentoReferencia.Substring(5, 2))
                        {
                            case "ZQ":
                                var referenciaV = dbContext.venta.FirstOrDefault(p => p.v_IdVenta.Equals(diario.v_IdDocumentoReferencia));
                                if (referenciaV != null)
                                {
                                    diario.v_Mes = referenciaV.v_Mes;
                                    diario.v_Correlativo = referenciaV.v_Correlativo;
                                    dbContext.diario.ApplyCurrentValues(diario);
                                }
                                break;

                            case "ZZ":
                                var referenciaC = dbContext.compra.FirstOrDefault(p => p.v_IdCompra.Equals(diario.v_IdDocumentoReferencia));
                                if (referenciaC != null)
                                {
                                    diario.v_Mes = referenciaC.v_Mes;
                                    diario.v_Correlativo = referenciaC.v_Correlativo;
                                    dbContext.diario.ApplyCurrentValues(diario);
                                }
                                break;
                        }
                    }

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "DiarioBL.EmparejarDiarios() \nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static void ActualizarNombreDiario(ref OperationResult pobjOperationResult)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var diarios = dbContext.diario.Where(p => p.i_IdTipoDocumento == 337 && p.i_Eliminado == 0 && string.IsNullOrEmpty(p.v_Nombre) && p.v_Mes == "10").ToList();

                    var diariosdetalle = (from a in dbContext.diariodetalle

                                          join b in dbContext.cliente on new { c = a.v_IdCliente, eliminado = 0 } equals new { c = b.v_IdCliente, eliminado = 0 } into b_join

                                          from b in b_join.DefaultIfEmpty()

                                          where a.v_IdCliente != null

                                          select new
                                          {

                                              cliente = (b.v_ApePaterno + " " + b.v_ApeMaterno + " " + b.v_PrimerNombre + " " + b.v_RazonSocial).Trim(),
                                              v_IdDiario = a.v_IdDiario,


                                          }).ToList();

                    if (!diarios.Any()) return;

                    foreach (var diario in diarios)
                    {
                        //switch (diario.v_IdDocumentoReferencia.Substring(5, 2))
                        //{
                        //    case "CQ":
                        //        var referenciaV = dbContext.liquidacioncompra.FirstOrDefault(p => p.v_IdLiquidacionCompra.Equals(diario.v_IdDocumentoReferencia));
                        //        if (referenciaV != null)
                        //        {
                        //            diario.v_Mes = referenciaV.v_Mes;
                        //            diario.v_Correlativo = referenciaV.v_Correlativo;
                        //            dbContext.diario.ApplyCurrentValues(diario);
                        //        }
                        //        break;


                        //}


                        var Nombre = diariosdetalle.Where(l => l.v_IdDiario == diario.v_IdDiario).FirstOrDefault().cliente;
                        diario.v_Nombre = Nombre;
                        dbContext.diario.ApplyCurrentValues(diario);
                    }

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "DiarioBL.EmparejarDiarios() \nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }


        public static void EmparejarDiariosLiquidaciónCompra(ref OperationResult pobjOperationResult)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var diarios = dbContext.diario.Where(p => p.v_IdDocumentoReferencia != null && p.v_IdDocumentoReferencia != "" && p.i_Eliminado == 0).ToList();

                    if (!diarios.Any()) return;

                    foreach (var diario in diarios)
                    {
                        switch (diario.v_IdDocumentoReferencia.Substring(5, 2))
                        {
                            case "CQ":
                                var referenciaV = dbContext.liquidacioncompra.FirstOrDefault(p => p.v_IdLiquidacionCompra.Equals(diario.v_IdDocumentoReferencia));
                                if (referenciaV != null)
                                {
                                    diario.v_Mes = referenciaV.v_Mes;
                                    diario.v_Correlativo = referenciaV.v_Correlativo;
                                    dbContext.diario.ApplyCurrentValues(diario);
                                }
                                break;


                        }
                    }

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "DiarioBL.EmparejarDiarios() \nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }


        /// <summary>
        /// Sirve para reubicar los documentos de los asientos contables del canje de las letras en el caso que se desee reubicar su nro de documentos.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="idDocumentoNuevo"></param>
        /// <param name="idCobrarPagar">1 para reubicar las letras por cobrar y 2 las letras por pagar</param>
        public void ReubicarAsientosCanjeLetras(ref OperationResult pobjOperationResult, int idDocumentoNuevo, int idCobrarPagar)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var criterio = idCobrarPagar == 1 ? "-LA" : "-PX";

                    var asientos = dbContext.diario
                        .Where(p => p.v_IdDocumentoReferencia.Contains(criterio) && p.i_Eliminado == 0).ToList();
                    if (!asientos.Any()) return;
                    foreach (var asiento in asientos.GroupBy(p => new { periodo = p.t_Fecha.Value.Year, mes = p.t_Fecha.Value.Month }))
                    {
                        var listadoDiarios = ObtenerListadoDiario(ref pobjOperationResult,
                            asiento.Key.periodo.ToString(),
                            asiento.Key.mes.ToString("00"), idDocumentoNuevo);

                        var maxMovimiento = (listadoDiarios.Any() ? int.Parse(listadoDiarios[listadoDiarios.Count - 1].Value1) : 0) + 1;

                        foreach (var a in asiento.OrderBy(o => o.t_Fecha))
                        {
                            a.i_IdTipoDocumento = idDocumentoNuevo;
                            a.v_Correlativo = maxMovimiento.ToString("00000000");
                            dbContext.diario.ApplyCurrentValues(a);
                            maxMovimiento++;
                        }
                    }

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "DocumentoBL.ReubicarAsientosCanjeLetras()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public string DevolverAsientoContable(string pstrIdProcesos)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var asientoRef = dbContext.diario.FirstOrDefault(p => p.v_IdDocumentoReferencia.Equals(pstrIdProcesos) && p.i_Eliminado == 0);
                    return asientoRef != null ? asientoRef.v_IdDiario : null;
                }
            }
            catch
            {
                return null;
            }
        }

        public string DevolverAsientoContableTesoreria(string pstrIdReferencia)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var asientoRef = dbContext.tesoreria.FirstOrDefault(p => p.v_IdCobranza.Equals(pstrIdReferencia) && p.i_Eliminado == 0);
                    return asientoRef != null ? asientoRef.v_IdTesoreria : null;
                }
            }
            catch
            {
                return null;
            }
        }


        public static bool CorrelativoYaExiste(ref OperationResult pobjOperationResult, string periodo, string pstrMes, string pstrCorrelativo, int pintTipoDocumento)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var result = dbContext.diario.Any(p => p.v_Periodo == periodo && p.i_IdTipoDocumento == pintTipoDocumento && p.v_Mes.Trim() == pstrMes.Trim() && p.v_Correlativo.Trim() == pstrCorrelativo.Trim() && p.i_Eliminado == 0);
                    pobjOperationResult.Success = 1;
                    return result;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "DiarioBL.CorrelativoYaExiste()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return false;
            }
        }

    }


}
