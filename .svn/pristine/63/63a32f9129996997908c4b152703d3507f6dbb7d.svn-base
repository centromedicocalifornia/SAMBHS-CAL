using System;
using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using SAMBHS.CommonWIN.BL;
using System.ComponentModel;
using SAMBHS.Common.BE.Custom;
using System.Data.Objects.SqlClient;
using System.Transactions;
using System.Data.Objects;
using System.Diagnostics;
using Dapper;
using Devart.Data.PostgreSql;
using System.Data.SqlClient;
using SAMBHS.Tesoreria.BL;

namespace SAMBHS.Contabilidad.BL
{
    public class AsientosContablesBL
    {
        #region Formulario
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        private string periodo = Globals.ClientSession.i_Periodo.Value.ToString();
        public string AsientoNuevo(ref OperationResult pobjOperationResult, asientocontableDto pobjDtoEntity, List<string> ClientSession, List<destinoDto> _TempAgregarDestinoDto)
        {
            using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
            {
                try
                {

                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                    asientocontable objEntity = asientocontableAssembler.ToEntity(pobjDtoEntity);

                    objEntity.t_InsertaFecha = DateTime.Now;
                    objEntity.v_Periodo = Globals.ClientSession.i_Periodo.Value.ToString();
                    objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntity.i_Eliminado = 0;

                    dbContext.AddToasientocontable(objEntity);
                    dbContext.SaveChanges();


                    if (pobjDtoEntity.i_Imputable == 1 && (pobjDtoEntity.v_NroCuenta.StartsWith("60") || pobjDtoEntity.v_NroCuenta.StartsWith("62") || pobjDtoEntity.v_NroCuenta.StartsWith("63") ||

                      pobjDtoEntity.v_NroCuenta.StartsWith("64") || pobjDtoEntity.v_NroCuenta.StartsWith("65") || pobjDtoEntity.v_NroCuenta.StartsWith("66") || pobjDtoEntity.v_NroCuenta.StartsWith("67") || pobjDtoEntity.v_NroCuenta.StartsWith("68")))
                    {
                        if (!DestinoNuevo(ref pobjOperationResult, Globals.ClientSession.GetAsList(), _TempAgregarDestinoDto, objEntity.v_NroCuenta))
                        {
                            return "Imposible Guardar Destinos para la Cuenta : " + objEntity.v_NroCuenta + ".\nEstos tienen que ser 100 %";

                        }
                    }
                    dbContext.SaveChanges();

                    pobjOperationResult.Success = 1;
                    Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "asientocontable", objEntity.v_NroCuenta);
                    ts.Complete();
                    return "";
                }
                catch (Exception ex)
                {
                    pobjOperationResult.Success = 0;
                    pobjOperationResult.AdditionalInformation = "AsientosContablesBL.AsientoNuevo()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                    pobjOperationResult.ErrorMessage = ex.Message;
                    pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                    Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                    return pobjOperationResult.AdditionalInformation;
                }

            }
        }

        public void AsientosNuevos(ref OperationResult pobjOperationResult, List<asientocontableDto> pobjDtoEntitys, List<string> ClientSession)
        {
            string newId = string.Empty;
            SecuentialBL objSecuentialBL = new SecuentialBL();

            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                foreach (asientocontableDto pobjDtoEntity in pobjDtoEntitys.AsParallel())
                {
                    asientocontable objEntity = asientocontableAssembler.ToEntity(pobjDtoEntity);
                    objEntity.t_InsertaFecha = DateTime.Now;
                    objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntity.i_Eliminado = 0;
                    objEntity.v_Periodo = Globals.ClientSession.i_Periodo.Value.ToString();
                    dbContext.AddToasientocontable(objEntity);

                }

                dbContext.SaveChanges();

                pobjOperationResult.Success = 1;

                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return;
            }
        }

        private void SaveLog(string p1, string p2, string p3, LogEventType logEventType, string p4, string p5, Success success, string p6)
        {
            throw new NotImplementedException();
        }

        public List<asientocontableDto> ObtenCuentasMayoresPaginadoFiltrado(ref OperationResult pobjOperationResult, int? pintPageIndex, int? pintResultsPerPage, string pstrSortExpression, string pstrFilterExpression)
        {
            //mon.IsActive = true;
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = from A in dbContext.asientocontable
                            where A.i_Eliminado == 0 && A.i_LongitudJerarquica == 2 && A.v_Periodo == periodo
                            orderby A.v_NroCuenta ascending
                            select new asientocontableDto
                            {
                                v_NroCuenta = A.v_NroCuenta,
                                v_NombreCuenta = A.v_NombreCuenta,
                                i_Naturaleza = A.i_Naturaleza
                            };

                if (!string.IsNullOrEmpty(pstrFilterExpression))
                {
                    query = query.Where(pstrFilterExpression);
                }
                if (pintPageIndex.HasValue && pintResultsPerPage.HasValue)
                {
                    int intStartRowIndex = pintPageIndex.Value * pintResultsPerPage.Value;
                    query = query.Skip(intStartRowIndex);
                }
                if (pintResultsPerPage.HasValue)
                {
                    query = query.Take(pintResultsPerPage.Value);
                }
                //query = query.OrderBy("i_CodigoDocumento");
                List<asientocontableDto> objData = query.ToList();
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

        public List<asientocontableDto> ObtenPlanDeCuentasPaginadoFiltrado(ref OperationResult pobjOperationResult, int? pintPageIndex, int? pintResultsPerPage, string pstrSortExpression, string pstrFilterExpression, string pstrNroCuentaM)
        {
            //mon.IsActive = true;
            try
            {
                //SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();


                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                var query = from A in dbContext.asientocontable
                            where A.i_Eliminado == 0 && A.v_NroCuenta.Substring(0, 2) == pstrNroCuentaM && A.i_LongitudJerarquica > 2 && A.v_Periodo == periodo
                            orderby A.v_NroCuenta ascending
                            select new asientocontableDto
                            {
                                v_NroCuenta = A.v_NroCuenta,
                                v_NombreCuenta = A.v_NombreCuenta,
                                i_Imputable = A.i_Imputable,
                                i_CentroCosto = A.i_CentroCosto == null ? 0 : A.i_CentroCosto.Value,
                            };

                if (!string.IsNullOrEmpty(pstrFilterExpression))
                {
                    query = query.Where(pstrFilterExpression);
                }
                if (pintPageIndex.HasValue && pintResultsPerPage.HasValue)
                {
                    int intStartRowIndex = pintPageIndex.Value * pintResultsPerPage.Value;
                    query = query.Skip(intStartRowIndex);
                }
                if (pintResultsPerPage.HasValue)
                {
                    query = query.Take(pintResultsPerPage.Value);
                }
                //query = query.OrderBy("i_CodigoDocumento");
                List<asientocontableDto> objData = query.ToList();
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

        public asientocontableDto ObtenAsientosPorNro(ref OperationResult pobjOperationResult, string pstrNroAsiento)
        {

            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                asientocontableDto objDtoEntity = null;
                var objEntity = (from a in dbContext.asientocontable
                                 where a.v_NroCuenta == pstrNroAsiento && a.i_Eliminado == 0 && a.v_Periodo == periodo
                                 select a).FirstOrDefault();

                if (objEntity != null)
                    objDtoEntity = asientocontableAssembler.ToDTO(objEntity);

                pobjOperationResult.Success = 1;

                return objDtoEntity;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public List<asientocontableDto> CheckByNroCuenta(ref OperationResult pobjOperationResult, string pstrNroAsiento)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                var query = from A in dbContext.asientocontable
                            where A.v_NroCuenta == pstrNroAsiento && A.i_Eliminado == 0 && A.v_Periodo == periodo
                            select new asientocontableDto
                            {
                                v_NroCuenta = A.v_NroCuenta
                            };

                List<asientocontableDto> objData = query.ToList();
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

        public string AsientoActualizar(ref OperationResult pobjOperationResult, asientocontableDto pobjDtoEntity, List<string> ClientSession, List<destinoDto> _TempAgregarDestinoDto, List<destinoDto> _TempEditarDestinoDto, List<destinoDto> _TempEliminarDestinoDto)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {


                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.asientocontable
                                           where a.v_NroCuenta == pobjDtoEntity.v_NroCuenta && a.i_Eliminado == 0 && a.v_Periodo == periodo
                                           select a).FirstOrDefault();

                    pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                    pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                    asientocontable objEntity = asientocontableAssembler.ToEntity(pobjDtoEntity);

                    dbContext.asientocontable.ApplyCurrentValues(objEntity);

                    dbContext.SaveChanges();

                    if (pobjDtoEntity.i_Imputable == 1 && (pobjDtoEntity.v_NroCuenta.StartsWith("60") || pobjDtoEntity.v_NroCuenta.StartsWith("62") || pobjDtoEntity.v_NroCuenta.StartsWith("63") ||

                      pobjDtoEntity.v_NroCuenta.StartsWith("64") || pobjDtoEntity.v_NroCuenta.StartsWith("65") || pobjDtoEntity.v_NroCuenta.StartsWith("66") || pobjDtoEntity.v_NroCuenta.StartsWith("67") || pobjDtoEntity.v_NroCuenta.StartsWith("68")))
                    {
                        DestinoActualizar(ref pobjOperationResult, Globals.ClientSession.GetAsList(), _TempAgregarDestinoDto, _TempEditarDestinoDto, _TempEliminarDestinoDto, objEntity.v_NroCuenta);
                        if (pobjOperationResult.Success == 0)
                        {
                            return "Imposible Guardar Destinos para la Cuenta  " + objEntity.v_NroCuenta + ".\nEstos tienen que ser 100 %";
                        }
                    }

                    pobjOperationResult.Success = 1;
                    Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "asientocontable", objEntitySource.v_NroCuenta);
                    ts.Complete();
                    return "";
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AsientosContablesBL.AsientoActualizar()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return pobjOperationResult.AdditionalInformation;
            }
        }

        public void AsientoBorrar(ref OperationResult pobjOperationResult, string pstrNroCta, List<string> ClientSession)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                var objEntitySource = (from a in dbContext.asientocontable
                                       where a.v_NroCuenta == pstrNroCta && a.v_Periodo == periodo && a.i_Eliminado == 0
                                       select a).FirstOrDefault();

                //objEntitySource.i_IsDeletE = 1;

                objEntitySource.i_Eliminado = 1;

                objEntitySource.t_ActualizaFecha = DateTime.Now;
                objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                dbContext.SaveChanges();
                Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "asientocontable", objEntitySource.v_NroCuenta);
                pobjOperationResult.Success = 1;
                // LogBL.SaveLog(ClientSession[0], ClientSession[1], ClientSession[2], LogEventType.ELIMINACION, "ASIENTOS CONTABLES", "v_NroCuenta=" + objEntitySource.v_NroCuenta, Success.Ok, null);
                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AsientosContablesBL.AsientoBorrar()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }

        public List<string> ObtenerCuentas(ref OperationResult pobjOperationResult, string periodoAnterior, string CuentaMayor)
        {

            try
            {
                pobjOperationResult.Success = 1;
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var Cuentas = (from a in dbContext.asientocontable

                                   where a.i_Eliminado == 0 && a.v_Periodo == periodoAnterior && a.v_NroCuenta.StartsWith(CuentaMayor) && a.i_Imputable == 1
                                   select a.v_NroCuenta).ToList();
                    return Cuentas;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AsientosContablesBL.ObtenerCuentas()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public bool DestinoActualizar(ref OperationResult pobjOperationResult, List<string> ClientSession, List<destinoDto> pTemp_Insertar, List<destinoDto> pTemp_Editar, List<destinoDto> pTemp_Eliminar, string CuentaOrigen)
        {

            try
            {
                SecuentialBL objSecuentialBL = new SecuentialBL();
                OperationResult objOperationResult = new OperationResult();
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                int SecuentialId = 0;
                string newIdDestino = string.Empty;
                int intNodeId;
                intNodeId = int.Parse(ClientSession[0]);
                decimal PorcGuardadosBase = 0;
                decimal PorcInsertados = 0;
                decimal PorcEditados = 0;
                decimal PorcEliminados = 0;
                #region Actualiza Detalle
                List<destinoDto> ListaF = new List<destinoDto>();
                List<destinoDto> ListaE = new List<destinoDto>();
                var TodosGuardados = dbContext.destino.Where(o => o.i_Eliminado == 0 && o.v_CuentaOrigen == CuentaOrigen && o.v_Periodo.Equals(periodo)).ToList().ToDTOs();
                var SinModificar = TodosGuardados.Select(o => o.v_IdDestino).Except(pTemp_Editar.Select(o => o.v_IdDestino)).ToList();
                foreach (var item in SinModificar)
                {
                    destinoDto obj = new destinoDto();
                    var Des = TodosGuardados.FirstOrDefault(o => o.v_IdDestino == item);
                    ListaF.Add(Des);
                }
                foreach (var item in pTemp_Eliminar)
                {
                    destinoDto obj = new destinoDto();
                    var Des = TodosGuardados.FirstOrDefault(o => o.v_IdDestino == item.v_IdDestino);
                    ListaE.Add(Des);
                }

                PorcGuardadosBase = ListaF.Any() ? ListaF.Sum(o => o.i_Porcentaje).Value : 0;
                PorcInsertados = pTemp_Insertar.Any() ? pTemp_Insertar.Sum(o => o.i_Porcentaje).Value : 0;
                PorcEditados = pTemp_Editar.Any() ? pTemp_Editar.Sum(o => o.i_Porcentaje).Value : 0;
                PorcEliminados = ListaE.Any() ? ListaE.Sum(o => o.i_Porcentaje).Value : 0;
                var PorcentajeFinal = (PorcGuardadosBase + PorcInsertados + PorcEditados) - PorcEliminados;

                if (PorcentajeFinal > 100 || PorcentajeFinal < 100)
                {
                    return false;
                }
                foreach (destinoDto DestinoDto in pTemp_Insertar)
                {
                    destino objEntityDestino = destinoAssembler.ToEntity(DestinoDto);
                    SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 67);
                    newIdDestino = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "LM");
                    objEntityDestino.v_CuentaOrigen = CuentaOrigen;
                    objEntityDestino.v_IdDestino = newIdDestino;
                    objEntityDestino.t_InsertaFecha = DateTime.Now;
                    objEntityDestino.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntityDestino.i_Eliminado = 0;
                    dbContext.AddTodestino(objEntityDestino);
                    Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "destino", newIdDestino);
                }

                foreach (destinoDto DestinoDto in pTemp_Editar)
                {
                    //pedidodetalle _objEntity = pedidodetalleAssembler.ToEntity(pedidodetalleDto);
                    destino _objEntity = destinoAssembler.ToEntity(DestinoDto);
                    var query = (from n in dbContext.destino
                                 where n.v_IdDestino == DestinoDto.v_IdDestino
                                 select n).FirstOrDefault();
                    _objEntity.v_CuentaOrigen = CuentaOrigen;
                    _objEntity.t_ActualizaFecha = DateTime.Now;
                    _objEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                    dbContext.destino.ApplyCurrentValues(_objEntity);
                    Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "destino", query.v_IdDestino);
                }

                foreach (destinoDto DestinoDto in pTemp_Eliminar)
                {
                    destino _objEntity = destinoAssembler.ToEntity(DestinoDto);
                    var query = (from n in dbContext.destino
                                 where n.v_IdDestino == DestinoDto.v_IdDestino
                                 select n).FirstOrDefault();

                    if (query != null)
                    {
                        query.t_ActualizaFecha = DateTime.Now;
                        query.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        query.i_Eliminado = 1;
                    }


                    dbContext.destino.ApplyCurrentValues(query);
                    Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "destino", query.v_IdDestino);
                }
                #endregion

                dbContext.SaveChanges();
                pobjOperationResult.Success = 1;
                return true;

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return false;

            }
        }

        public bool DestinoNuevo(ref OperationResult pobjOperationResult, List<string> ClientSession, List<destinoDto> pTemp_Insertar, string CuentaOrigen)
        {
            SecuentialBL objSecuentialBL = new SecuentialBL();
            OperationResult objOperationResult = new OperationResult();
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                int SecuentialId = 0;
                string newIdDestino = string.Empty;
                int intNodeId;
                intNodeId = int.Parse(ClientSession[0]);
                decimal SumaPorcentajes = 0;
                var ListaDestinosPorcentajes = dbContext.destino.Where(o => o.i_Eliminado == 0 && o.v_CuentaOrigen == CuentaOrigen && o.v_Periodo.Equals(periodo)).ToList();

                decimal Por = ListaDestinosPorcentajes.Any() ? ListaDestinosPorcentajes.Sum(o => o.i_Porcentaje).Value : 0;
                SumaPorcentajes = Por + pTemp_Insertar.Sum(o => o.i_Porcentaje).Value;

                if (SumaPorcentajes > 100 || SumaPorcentajes < 100)
                {
                    return false;
                }

                foreach (destinoDto DestinoDto in pTemp_Insertar)
                {
                    destino objEntityDestino = destinoAssembler.ToEntity(DestinoDto);
                    SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 67);
                    newIdDestino = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "LM");
                    objEntityDestino.v_CuentaOrigen = CuentaOrigen;
                    objEntityDestino.v_IdDestino = newIdDestino;
                    objEntityDestino.t_InsertaFecha = DateTime.Now;
                    objEntityDestino.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntityDestino.i_Eliminado = 0;
                    dbContext.AddTodestino(objEntityDestino);
                }
                dbContext.SaveChanges();
                pobjOperationResult.Success = 1;
                Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "destino", newIdDestino);
                return true;
            }

        }

        public bool DestinoNuevoMigracion(ref OperationResult pobjOperationResult, List<string> ClientSession, List<destinoDto> pTemp_Insertar)
        {
            SecuentialBL objSecuentialBL = new SecuentialBL();
            OperationResult objOperationResult = new OperationResult();
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                int SecuentialId = 0;
                string newIdDestino = string.Empty;
                int intNodeId;
                intNodeId = int.Parse(ClientSession[0]);
                //var SumaPorcentajes = dbContext.destino.Where(o => o.i_Eliminado == 0 && o.v_CuentaOrigen == CuentaOrigen).Sum(o => o.i_Porcentaje).Value;
                //if (SumaPorcentajes > 100 || SumaPorcentajes < 100)
                //{
                //    return false;
                //}

                foreach (destinoDto DestinoDto in pTemp_Insertar)
                {
                    destino objEntityDestino = destinoAssembler.ToEntity(DestinoDto);
                    SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 67);
                    newIdDestino = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "LM");
                    // objEntityDestino.v_CuentaOrigen = CuentaOrigen;
                    objEntityDestino.v_IdDestino = newIdDestino;
                    objEntityDestino.t_InsertaFecha = DateTime.Now;
                    objEntityDestino.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntityDestino.i_Eliminado = 0;
                    dbContext.AddTodestino(objEntityDestino);
                }
                dbContext.SaveChanges();
                pobjOperationResult.Success = 1;
                Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "destino", newIdDestino);
                return true;
            }

        }

        public void DestinoBorrar(ref OperationResult pobjOperationResult, string pstrNroCta, List<string> ClientSession)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                var objEntitySource = (from a in dbContext.destino
                                       where a.v_CuentaOrigen == pstrNroCta && a.v_Periodo == periodo
                                       select a).ToList();



                foreach (var fila in objEntitySource)
                {
                    fila.i_Eliminado = 1;
                    fila.t_ActualizaFecha = DateTime.Now;
                    fila.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                    Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "destino", fila.v_IdDestino);
                }

                dbContext.SaveChanges();
                pobjOperationResult.Success = 1;

                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                //  LogBL.SaveLog(ClientSession[0], ClientSession[1], ClientSession[2], LogEventType.ELIMINACION, "ASIENTOS CONTABLES", "", Success.Failed, pobjOperationResult.ExceptionMessage);
                return;
            }



        }

        public BindingList<destinoDto> ObtenerDestinos(string pstrNumeroCuenta)
        {


            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

            var Destinos = (from n in dbContext.destino
                            where n.i_Eliminado == 0 && n.v_CuentaOrigen == pstrNumeroCuenta && n.v_Periodo == periodo
                            select new destinoDto
                            {
                                v_CuentaDestino = n.v_CuentaDestino,
                                v_CuentaTransferencia = n.v_CuentaTransferencia,
                                v_IdDestino = n.v_IdDestino,
                                i_Eliminado = n.i_Eliminado,
                                t_InsertaFecha = n.t_InsertaFecha,
                                i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                i_Porcentaje = n.i_Porcentaje,
                                v_Periodo = n.v_Periodo,
                            }).ToList();


            return new BindingList<destinoDto>(Destinos);



            //return new BindingList<destinoDto>(destinoAssembler.ToDTOs(Destinos));


        }

        public asientocontableDto ObtenerAsientoImputable(string pstrNumeroCuenta)
        {

            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

            var query = (from n in dbContext.asientocontable
                         where n.i_Eliminado == 0 && n.i_Imputable == 1 && n.v_NroCuenta == pstrNumeroCuenta && n.v_Periodo == periodo
                         select n).FirstOrDefault();
            return asientocontableAssembler.ToDTO(query);
        }

        public bool EsImputable(string pstrNumeroCuenta)
        {

            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

            var query = (from n in dbContext.asientocontable
                         where n.i_Eliminado == 0 && n.i_Imputable == 1 && n.v_NroCuenta == pstrNumeroCuenta && n.v_Periodo == periodo

                         select n).FirstOrDefault();
            if (query != null)
                return true;
            return false;

        }

        public bool ExistenciaAsientoenTransacciones(string pstrNroCuenta, string pPeriodo)
        {
            //using (var dbContext = new SAMBHSEntitiesModelWin())
            //{
            //    return dbContext.saldoscontables.Where(x => x.d_ImporteSolesD > 0 && x.d_ImporteSolesH > 0).Any(p => p.v_NroCuenta == pstrNroCuenta);
            //}
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {


                var diarios = (from a in dbContext.diario
                               join b in dbContext.diariodetalle on new { d = a.v_IdDiario, eliminado = 0 } equals new { d = b.v_IdDiario, eliminado = b.i_Eliminado.Value } into b_join
                               from b in b_join.DefaultIfEmpty()
                               where a.v_Periodo == pPeriodo && a.i_Eliminado == 0 && b.v_NroCuenta == pstrNroCuenta
                               select b).ToList();
                var tesorerias = (from a in dbContext.tesoreria
                                  join b in dbContext.tesoreriadetalle on new { t = a.v_IdTesoreria, eliminado = 0 } equals new { t = b.v_IdTesoreria, eliminado = b.i_Eliminado.Value } into b_join
                                  from b in b_join.DefaultIfEmpty()
                                  where b.v_NroCuenta == pstrNroCuenta && a.i_Eliminado == 0 && a.v_Periodo == pPeriodo
                                  select b).ToList();
                if (!diarios.Any() && !tesorerias.Any())
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public BindingList<asientocontableDto> DevuelveCuentasParaDiferenciaCambio(ref OperationResult pobjOperationResult, int pintActivoPasivo)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    List<asientocontableDto> Query = asientocontableAssembler.ToDTOs(dbContext.asientocontable.Where(p => p.i_AjusteDiferenciaCambio == pintActivoPasivo && p.v_Periodo == periodo && p.i_Eliminado == 0).ToList());

                    pobjOperationResult.Success = 1;
                    return new BindingList<asientocontableDto>(Query);
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AsientosContablesBL.DevuelveCuentasParaDiferenciaCambio()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public void ActualizaCuentasParaDiferenciaCambio(ref OperationResult pobjOperationResult, List<asientocontableDto> CuentasAgregadas, List<asientocontableDto> CuentasEliminadas, int pintActivoPasivo)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        #region Marcar cuentas para diferencia de cambio
                        foreach (asientocontableDto cuenta in CuentasAgregadas)
                        {
                            var cuentaOriginal = dbContext.asientocontable.FirstOrDefault(p => p.v_NroCuenta == cuenta.v_NroCuenta && p.v_Periodo == periodo && p.i_Eliminado == 0);

                            if (cuentaOriginal != null && cuentaOriginal.i_AjusteDiferenciaCambio != pintActivoPasivo)
                            {
                                cuentaOriginal.i_AjusteDiferenciaCambio = pintActivoPasivo;
                                cuentaOriginal.t_ActualizaFecha = DateTime.Now.Date;
                                cuentaOriginal.i_ActualizaIdUsuario = int.Parse(Globals.ClientSession.GetAsList()[2]);
                                dbContext.asientocontable.ApplyCurrentValues(cuentaOriginal);
                            }
                        }
                        #endregion

                        #region Desmarcar cuentas para diferencia de cambio
                        foreach (asientocontableDto cuenta in CuentasEliminadas)
                        {
                            var cuentaOriginal = dbContext.asientocontable.FirstOrDefault(p => p.v_NroCuenta == cuenta.v_NroCuenta && p.v_Periodo == periodo && p.i_Eliminado == 0);

                            if (cuentaOriginal != null && cuentaOriginal.i_AjusteDiferenciaCambio == pintActivoPasivo)
                            {
                                cuentaOriginal.i_AjusteDiferenciaCambio = null;
                                cuentaOriginal.t_ActualizaFecha = DateTime.Now.Date;
                                cuentaOriginal.i_ActualizaIdUsuario = int.Parse(Globals.ClientSession.GetAsList()[2]);
                                dbContext.asientocontable.ApplyCurrentValues(cuentaOriginal);
                            }
                        }
                        #endregion

                        dbContext.SaveChanges();
                        pobjOperationResult.Success = 1;
                        ts.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AsientosContablesBL.ActualizaCuentasParaDiferenciaCambio()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }


        public List<string> ValidarCuentasDestino(ref OperationResult objOperationResult)
        {
            try
            {



                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var Destinos = dbContext.destino.Where(x => x.i_Eliminado == 0 && x.v_Periodo == periodo).ToList();

                    List<string> ListaRetorno = new List<string>();



                    List<saldoscontablesDto> CuentasSaldosContables = ObtenerSaldosContablesPorMes(ref  objOperationResult, 12, Globals.ClientSession.i_Periodo.Value, 1, false, false, null);

                    List<saldoscontablesDto> AcumuladoAnterior = (from n in CuentasSaldosContables
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


                    var cuentasDestino = (from a in AcumuladoAnterior
                                          where ((a.v_NroCuenta.StartsWith("60") || a.v_NroCuenta.StartsWith("62") || a.v_NroCuenta.StartsWith("63") ||
                                          a.v_NroCuenta.StartsWith("64") || a.v_NroCuenta.StartsWith("65") || a.v_NroCuenta.StartsWith("66") || a.v_NroCuenta.StartsWith("67")
                                          || a.v_NroCuenta.StartsWith("68")))

                                          orderby (a.v_NroCuenta) ascending
                                          select a).ToList().Select(x =>
                                             {
                                                 string Mensaje = "";
                                                 var DestinosR = Destinos.Where(y => y.v_CuentaOrigen.Trim() == x.v_NroCuenta.Trim()).ToList();

                                                 return new asientocontableDto
                                                 {
                                                     v_NroCuenta = x.v_NroCuenta,
                                                     v_Area = !DestinosR.Any() ? "" : "TIENEDESTINO",

                                                 };

                                             }).ToList();
                    var CuentasNoTieneDestino = cuentasDestino.Where(x => x.v_Area != "TIENEDESTINO").ToList();

                    foreach (var item in CuentasNoTieneDestino)
                    {
                        if (CuentasSaldosContables.Select(o => o.v_NroCuenta).Contains(item.v_NroCuenta))
                        {
                            ListaRetorno.Add(item.v_NroCuenta);
                        }
                    }

                    return ListaRetorno;



                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;

            }


        }

        public void GenerarSaldoMesualBancos_(ref OperationResult pobOperationResult, int mes, string anioActual)
        {

            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
            DateTime fechaInicio = DateTime.Parse("01/" + "01" + "/" + anioActual);
            DateTime fechaFin = DateTime.Parse(DateTime.DaysInMonth(int.Parse(anioActual), mes).ToString() + "/" + mes + "/" + anioActual + " 23:59");
            saldomensualbancosDto objSaldoMensualBancoDto = new saldomensualbancosDto();
            string anio = string.Empty, mesAnterior = string.Empty;
            OperationResult objOperationResult = new OperationResult();
            string AnioApertura = (Globals.ClientSession.i_Periodo - 1).ToString();
            string MesApertura = "12";

            if (mes != (int)Mes.Enero)
            {
                anio = anioActual;
                mesAnterior = (mes - 1).ToString("00");

            }
            else
            {
                anio = (int.Parse(anioActual) - 1).ToString();
                mesAnterior = ((int)Mes.Diciembre).ToString("00");
            }


            var SaldoMensualBancoMesAnterior = (from n in dbContext.saldomensualbancos

                                                where n.v_Anio == anio && n.v_Mes == mesAnterior && n.v_NroCuenta.StartsWith("10") && n.i_Eliminado == 0

                                                select n).ToList();


            var analisisCuentas = new AsientosContablesBL().ReporteAnalisisCuentasCteAnalitico(ref objOperationResult,
                fechaInicio, fechaFin, null, null, false, null,
                TipoReporteAnalisis.AnalisisCuentaCorrienteAnalitico, 0, null, "10", false);

            var dataSource = analisisCuentas.GroupBy(p => p.CuentaImputable)
                .Select(reporteAnalisisCuentase => new saldoscontablesDto
                {
                    CuentaMayor = reporteAnalisisCuentase.Key.Substring(0, 2),
                    d_ImporteDolaresD = reporteAnalisisCuentase.Sum(o => o.ImporteDolaresD ?? 0),
                    d_ImporteDolaresH = reporteAnalisisCuentase.Sum(o => o.ImporteDolaresH ?? 0),
                    d_ImporteSolesD = reporteAnalisisCuentase.Sum(o => o.ImporteSolesD ?? 0),
                    d_ImporteSolesH = reporteAnalisisCuentase.Sum(o => o.ImporteSolesH ?? 0),
                    i_IdMoneda = 1,
                    v_NroCuenta = reporteAnalisisCuentase.Key
                }).ToList();


            List<string> Cuentas = new List<string>();
            List<string> CuentasMesAnterior = new List<string>();
            var ListaCuenta = dataSource.ToList();
            Cuentas = ListaCuenta.OrderBy(o => o.v_NroCuenta).Select(o => o.v_NroCuenta).Distinct().ToList();
            CuentasMesAnterior = SaldoMensualBancoMesAnterior.OrderBy(o => o.v_NroCuenta).Select(o => o.v_NroCuenta).Distinct().ToList();

            var CuentasFinal = Cuentas.Union(CuentasMesAnterior).ToList();
            foreach (var Cuenta in CuentasFinal)
            {
                var Detalles = ListaCuenta.Where(o => o.v_NroCuenta == Cuenta).ToList();
                decimal TotalDebeSoles = Detalles.Any() ? Detalles.LastOrDefault().d_ImporteSolesD.Value : 0;
                decimal TotalDebeDolares = Detalles.Any() ? Detalles.LastOrDefault().d_ImporteDolaresD.Value : 0;
                decimal TotalHaberSoles = Detalles.Any() ? Detalles.LastOrDefault().d_ImporteSolesH.Value : 0;
                decimal TotalHaberDolares = Detalles.Any() ? Detalles.LastOrDefault().d_ImporteDolaresH.Value : 0;
                objSaldoMensualBancoDto.v_Anio = anioActual;
                objSaldoMensualBancoDto.v_Mes = mes.ToString("00");
                objSaldoMensualBancoDto.v_NroCuenta = Cuenta;

                objSaldoMensualBancoDto.d_SaldoSoles = TotalDebeSoles - TotalHaberSoles;
                objSaldoMensualBancoDto.d_SaldoDolares = TotalDebeDolares - TotalHaberDolares;

                objSaldoMensualBancoDto.d_SaldoSoles = Utils.Windows.DevuelveValorRedondeado(objSaldoMensualBancoDto.d_SaldoSoles.Value, 2);
                objSaldoMensualBancoDto.d_SaldoDolares = Utils.Windows.DevuelveValorRedondeado(objSaldoMensualBancoDto.d_SaldoDolares.Value, 2);
                new SaldoMensualBancosBL().InsertarSaldoMensualBanco(ref objOperationResult, objSaldoMensualBancoDto, Globals.ClientSession.GetAsList());
            }


            pobOperationResult.Success = 1;


        }

        public void GenerarSaldoMesualBancos2(ref OperationResult pobOperationResult, int mes, string anioActual)
        {


        }


        #endregion

        #region Reporte

        public List<ReportePlanContable> ReportePlanContable(ref OperationResult objOperationResult, string Cuenta, int Imputables = 0)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                objOperationResult.Success = 1;

                if (Imputables == 0)
                {

                    List<ReportePlanContable> query = (from n in dbContext.asientocontable
                                                       where n.i_Eliminado == 0 && (n.v_NroCuenta.StartsWith(Cuenta) || Cuenta == string.Empty)
                                                                  && n.v_Periodo == periodo

                                                       orderby n.v_NroCuenta descending
                                                       select new ReportePlanContable
                                                       {
                                                           NumeroCuenta = n.v_NroCuenta,
                                                           Descripcion = n.v_NombreCuenta,
                                                           Nivel = n.i_Imputable == 1 ? "SI" : "NO",
                                                           Detalle = n.i_Detalle == 1 ? "SI" : "NO",
                                                           Moneda = n.i_IdMoneda == 1 ? "S/." : "US$.",
                                                           Acm = n.i_ACM == 1 ? "SI" : "NO",
                                                           Destino = n.i_AplicacionDestino == 1 ? "SI" : "NO",
                                                           Padre1 = "",
                                                           Padre2 = "",
                                                           Padre3 = "",
                                                           Padre4 = "",
                                                           Padre5 = "",
                                                           Padre6 = "",
                                                           LongitudJerarquica = n.i_LongitudJerarquica,
                                                           Padre = "",
                                                           Padre7 = "",
                                                           Padre8 = "",
                                                           Padre9 = "",
                                                           Padre10 = "",

                                                       }).ToList();


                    var queryFinal = (from n in query

                                      select new ReportePlanContable
                                      {
                                          NumeroCuenta = n.NumeroCuenta,
                                          Descripcion = n.Descripcion,
                                          Nivel = n.Nivel,
                                          Detalle = n.Detalle,
                                          Moneda = n.Moneda,
                                          Acm = n.Acm,
                                          Destino = n.Destino,
                                          Padre1 = n.LongitudJerarquica == 2 ? n.NumeroCuenta + "  " + n.Descripcion : "",
                                          Padre2 = n.LongitudJerarquica == 3 ? n.NumeroCuenta + "  " + n.Descripcion : "",
                                          Padre3 = n.LongitudJerarquica == 4 ? n.NumeroCuenta + "  " + n.Descripcion : "",
                                          Padre4 = n.LongitudJerarquica == 5 ? n.NumeroCuenta + "  " + n.Descripcion : "",
                                          Padre5 = n.LongitudJerarquica == 6 ? n.NumeroCuenta + "  " + n.Descripcion : "",
                                          Padre6 = n.LongitudJerarquica == 7 ? n.NumeroCuenta + "  " + n.Descripcion : "",
                                          LongitudJerarquica = n.LongitudJerarquica,
                                          Padre = n.NumeroCuenta,
                                          Padre7 = n.LongitudJerarquica == 8 ? n.NumeroCuenta + "  " + n.Descripcion : "",
                                          Padre8 = n.LongitudJerarquica == 9 ? n.NumeroCuenta + "  " + n.Descripcion : "",
                                          Padre9 = n.LongitudJerarquica == 10 ? n.NumeroCuenta + "  " + n.Descripcion : "",
                                          Padre10 = n.LongitudJerarquica == 11 ? n.NumeroCuenta + "  " + n.Descripcion : "",

                                      }).ToList();

                    return queryFinal.OrderBy(l => l.NumeroCuenta).ToList();
                }
                else
                {
                    var CodPlanContable = dbContext.datahierarchy.Where(l => l.i_GroupId == 154 && l.i_IsDeleted == 0 && l.i_ItemId == Globals.ClientSession.i_CodigoPlanContable).ToList();
                    if (!CodPlanContable.Any())
                        throw new Exception("Por favor elija un tipo de plan contable en la configuración de empresa");

                    List<ReportePlanContable> query = (from n in dbContext.asientocontable
                                                       where n.i_Eliminado == 0 && n.v_Periodo == periodo
                                                       && n.i_Imputable == 1
                                                       select new
                                                       {
                                                           NumeroCuenta = n.v_NroCuenta,
                                                           Descripcion = n.v_NombreCuenta,


                                                       }).ToList().Select(l => new ReportePlanContable
                                                       {

                                                           NumeroCuenta = l.NumeroCuenta,
                                                           Descripcion = l.Descripcion,
                                                           CodigoPlanContable = CodPlanContable != null ? CodPlanContable.FirstOrDefault().v_Value2 : "",

                                                       }).ToList().OrderBy(l => l.NumeroCuenta).ToList();
                    return query;


                }

            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                objOperationResult.ErrorMessage = ex.Message;
                return null;
            }

        }

        public List<ReporteAdministracionConceptos> ReporteAdministracionConceptos()
        {

            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

            var ReporteAdministracionConceptos = (from n in dbContext.administracionconceptos

                                                  where n.i_Eliminado == 0

                                                  select new ReporteAdministracionConceptos
                                                  {
                                                      Codigo = n.v_Codigo,
                                                      Descripcion = n.v_Nombre,
                                                      CuentaVenta = n.v_CuentaPVenta,
                                                      CuentaIgv = n.v_CuentaIGV,
                                                      CuentaDetraccion = n.v_CuentaDetraccion

                                                  }).OrderBy(x => x.Codigo).ToList();

            return ReporteAdministracionConceptos;
        }

        public List<ReporteDestinos> ReporteDestinos(ref  OperationResult objOperationResult, string NroCuenta, string CuentaInicial, string CuentaFinal, bool CuentasSinDestino, bool PorcentajeMayor100)
        {
            try
            {
                objOperationResult.Success = 1;
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                List<ReporteDestinos> ListaRetorno = new List<ReporteDestinos>();
                #region MuestraSoloCuentasSinDestino
                if (CuentasSinDestino && string.IsNullOrEmpty(NroCuenta))
                {



                    var Destinos = dbContext.destino.Where(x => x.i_Eliminado == 0 && x.v_Periodo == periodo).ToList();
                    var diarioC = dbContext.diariodetalle.Where(o => o.i_Eliminado == 0).Select(p => p.v_NroCuenta).Distinct().ToList();
                    var tesoreriaC = dbContext.tesoreriadetalle.Where(o => o.i_Eliminado == 0).Select(o => o.v_NroCuenta).Distinct().ToList();
                    var CuentasSaldosContables = diarioC.Concat(tesoreriaC).Distinct().ToList();
                    var cuentasDestino = (from a in dbContext.asientocontable
                                          where a.i_Eliminado == 0 && a.i_Imputable == 1 && a.v_Periodo == periodo
                                          && ((a.v_NroCuenta.StartsWith("60") || a.v_NroCuenta.StartsWith("62") || a.v_NroCuenta.StartsWith("63") ||
                                          a.v_NroCuenta.StartsWith("64") || a.v_NroCuenta.StartsWith("65") || a.v_NroCuenta.StartsWith("66") || a.v_NroCuenta.StartsWith("67")
                                          || a.v_NroCuenta.StartsWith("68")))

                                          orderby (a.v_NroCuenta) ascending
                                          select a).ToList().Select(x =>
                                          {
                                              string Mensaje = "";
                                              var DestinosR = Destinos.Where(y => y.v_CuentaOrigen.Trim() == x.v_NroCuenta.Trim()).ToList();

                                              return new ReporteDestinos
                                              {
                                                  // v_NroCuenta = x.v_NroCuenta,
                                                  v_Area = !DestinosR.Any() ? "" : "TIENEDESTINO",
                                                  CuentaOrigen = x.v_NroCuenta,
                                              };

                                          }).ToList();
                    var CuentasNoTieneDestino = cuentasDestino.Where(x => x.v_Area != "TIENEDESTINO").ToList();
                    foreach (var item in CuentasNoTieneDestino)
                    {
                        if (CuentasSaldosContables.Contains(item.CuentaOrigen))
                        {
                            ListaRetorno.Add(item);
                        }
                    }

                    return ListaRetorno;
                }

                #endregion

                else if (PorcentajeMayor100 && string.IsNullOrEmpty(NroCuenta))
                {
                    var CuentasAgrupadas = (from a in dbContext.destino
                                            where a.i_Eliminado == 0 && a.v_Periodo == periodo
                                            select new ReporteDestinos
                                            {
                                                CuentaOrigen = a == null ? "" : a.v_CuentaOrigen,
                                                CuentaDestino = a == null ? "" : a.v_CuentaDestino,
                                                CuentaTransferencia = a == null ? "" : a.v_CuentaTransferencia,
                                                Porcentaje = a.i_Porcentaje,


                                            }).ToList()
                                            .GroupBy(l => new { l.CuentaOrigen }).Select(d =>
                                        {
                                            var k = d.FirstOrDefault();
                                            k.Porcentaje = d.Sum(h => h.Porcentaje);

                                            return k;
                                        }).AsQueryable();
                    return CuentasAgrupadas.Where(l => l.Porcentaje != 100).ToList();




                }
                else
                {

                    #region Filtra todos los destinos
                    var Reporte = (from n in dbContext.destino

                                   join m in dbContext.asientocontable on new { cuentaOrigen = n.v_CuentaOrigen, eliminado = 0, p = periodo }
                                                                        equals new { cuentaOrigen = m.v_NroCuenta, eliminado = m.i_Eliminado.Value, p = m.v_Periodo } into m_join

                                   from m in m_join.DefaultIfEmpty()

                                   join o in dbContext.asientocontable on new { CuentaDestino = n.v_CuentaDestino, eliminado = 0, p = periodo }
                                                                    equals new { CuentaDestino = o.v_NroCuenta, eliminado = o.i_Eliminado.Value, p = o.v_Periodo } into o_join

                                   from o in o_join.DefaultIfEmpty()

                                   join p in dbContext.asientocontable on new { cuentaTrans = n.v_CuentaTransferencia, eliminado = 0, p = periodo }
                                                                    equals new { cuentaTrans = p.v_NroCuenta, eliminado = p.i_Eliminado.Value, p = p.v_Periodo } into p_join

                                   from p in p_join.DefaultIfEmpty()

                                   where n.i_Eliminado == 0 && (m.v_NroCuenta.Trim() == NroCuenta || NroCuenta == string.Empty)

                                   && n.v_Periodo == periodo

                                   select new ReporteDestinos
                                   {

                                       CuentaOrigen = m == null ? n.v_CuentaOrigen + "** NO EXISTE **" : m.v_NroCuenta + " " + m.v_NombreCuenta,
                                       CuentaDestino = o == null ? n.v_CuentaDestino + "** NO EXISTE **" : o.v_NroCuenta + " " + o.v_NombreCuenta,
                                       CuentaTransferencia = p == null ? n.v_CuentaTransferencia + "** NO EXISTE **" : p.v_NroCuenta + " " + p.v_NombreCuenta,
                                       Porcentaje = n.i_Porcentaje,

                                   }).OrderBy(x => x.CuentaOrigen).ToList();


                    #endregion


                    #region Filtra por rango de cuentas si es requerido
                    if (!string.IsNullOrEmpty(CuentaInicial) && !string.IsNullOrEmpty(CuentaFinal))
                    {
                        var rangoCtas = Utils.Windows.RangoDeCuentas(CuentaFinal, CuentaFinal);
                        Reporte = Reporte.Where(p => rangoCtas.Contains(p.CuentaOrigen)).ToList();
                    }
                    #endregion
                    return Reporte;

                }




            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                return null;
            }


        }

        public List<ReporteAnalisisHojaTrabajo> ReporteAnalisisHojaTrabajo6061(ref OperationResult objOperationResult, DateTime FechaInicio, DateTime FechaFin)
        {

            try
            {
                objOperationResult.Success = 1;
                using (SAMBHSEntitiesModelWin dbcontext = new SAMBHSEntitiesModelWin())
                {

                    var diario = (from a in dbcontext.diario
                                  join b in dbcontext.diariodetalle on new { d = a.v_IdDiario, eliminado = 0 } equals new { d = b.v_IdDiario, eliminado = b.i_Eliminado.Value } into b_join
                                  from b in b_join.DefaultIfEmpty()
                                  join c in dbcontext.documento on new { d = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { d = c.i_CodigoDocumento, eliminado = c.i_Eliminado.Value } into c_join
                                  from c in c_join.DefaultIfEmpty()
                                  where a.i_Eliminado == 0 && (b.v_NroCuenta.StartsWith("60") || b.v_NroCuenta.StartsWith("61"))

                                  && a.t_Fecha >= FechaInicio && a.t_Fecha <= FechaFin
                                  select new ReporteAnalisisHojaTrabajo
                                  {
                                      NroDiario = c.v_Siglas + " " + a.v_Mes + " " + a.v_Correlativo,
                                      v_IdDiario = a.v_IdDiario,
                                      Cuenta = b.v_NroCuenta,
                                      Mes = a.v_Mes,
                                      Importe = b.d_Importe ?? 0,
                                      Naturaleza = b.v_Naturaleza,
                                      Debe = b.v_Naturaleza == "D" ? b.d_Importe.Value : 0,
                                      Haber = b.v_Naturaleza == "H" ? b.d_Importe.Value : 0,
                                      Cambio = b.d_Cambio ?? 0,
                                      DebeCambio = b.v_Naturaleza == "D" ? b.d_Cambio.Value : 0,
                                      HaberCambio = b.v_Naturaleza == "H" ? b.d_Cambio.Value : 0,

                                  }).ToList();


                    var tesoreria = (from a in dbcontext.tesoreria
                                     join b in dbcontext.tesoreriadetalle on new { d = a.v_IdTesoreria, eliminado = 0 } equals new { d = b.v_IdTesoreria, eliminado = b.i_Eliminado.Value } into b_join
                                     from b in b_join.DefaultIfEmpty()
                                     join c in dbcontext.documento on new { d = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { d = c.i_CodigoDocumento, eliminado = c.i_Eliminado.Value } into c_join
                                     from c in c_join.DefaultIfEmpty()
                                     where a.i_Eliminado == 0 && (b.v_NroCuenta.StartsWith("60") || b.v_NroCuenta.StartsWith("61"))
                                     && a.t_FechaRegistro >= FechaInicio && a.t_FechaRegistro <= FechaFin
                                     select new ReporteAnalisisHojaTrabajo
                                     {
                                         NroDiario = c.v_Siglas + " " + a.v_Mes + " " + a.v_Correlativo,
                                         v_IdDiario = a.v_IdTesoreria,
                                         Cuenta = b.v_NroCuenta,
                                         Mes = a.v_Mes,
                                         Importe = b.d_Importe ?? 0,
                                         Naturaleza = b.v_Naturaleza,
                                         Debe = b.v_Naturaleza == "D" ? b.d_Importe.Value : 0,
                                         Haber = b.v_Naturaleza == "H" ? b.d_Importe.Value : 0,
                                         Cambio = b.d_Cambio ?? 0,
                                         DebeCambio = b.v_Naturaleza == "D" ? b.d_Cambio.Value : 0,
                                         HaberCambio = b.v_Naturaleza == "H" ? b.d_Cambio.Value : 0,

                                     }).ToList();

                    return diario.Concat(tesoreria).ToList();
                }
            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                return null;
            }





        }

        public List<ReporteAnalisisHojaTrabajo> ReporteAnalisisHojaTrabajoClase9yCuenta79(ref OperationResult objOperationResult, DateTime FechaInicio, DateTime FechaFin)
        {

            try
            {
                objOperationResult.Success = 1;
                using (SAMBHSEntitiesModelWin dbcontext = new SAMBHSEntitiesModelWin())
                {
                    var xx = "1015";
                    var uu = xx.Substring(0, 2);
                    var diario = (from a in dbcontext.diario
                                  join b in dbcontext.diariodetalle on new { d = a.v_IdDiario, eliminado = 0 } equals new { d = b.v_IdDiario, eliminado = b.i_Eliminado.Value } into b_join
                                  from b in b_join.DefaultIfEmpty()
                                  join c in dbcontext.documento on new { d = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { d = c.i_CodigoDocumento, eliminado = 0 } into c_join
                                  from c in c_join.DefaultIfEmpty()
                                  where a.i_Eliminado == 0 && (b.v_NroCuenta.StartsWith("9") || b.v_NroCuenta.StartsWith("79") || b.v_NroCuenta.StartsWith("62") || b.v_NroCuenta.StartsWith("63") || b.v_NroCuenta.StartsWith("64") || b.v_NroCuenta.StartsWith("65") || b.v_NroCuenta.StartsWith("66") || b.v_NroCuenta.StartsWith("67") || b.v_NroCuenta.StartsWith("68"))

                                  && a.t_Fecha >= FechaInicio && a.t_Fecha <= FechaFin
                                  select new ReporteAnalisisHojaTrabajo
                                  {
                                      NroDiario = c.v_Siglas + " " + a.v_Mes + " " + a.v_Correlativo,
                                      v_IdDiario = a.v_IdDiario,
                                      Cuenta = b.v_NroCuenta,
                                      Mes = a.v_Mes,
                                      Importe = b.d_Importe ?? 0,
                                      Naturaleza = b.v_Naturaleza,
                                      Debe = b.v_Naturaleza == "D" ? b.d_Importe.Value : 0,
                                      Haber = b.v_Naturaleza == "H" ? b.d_Importe.Value : 0,
                                      Cambio = b.d_Cambio ?? 0,
                                      DebeCambio = b.v_Naturaleza == "D" ? b.d_Cambio.Value : 0,
                                      HaberCambio = b.v_Naturaleza == "H" ? b.d_Cambio.Value : 0,
                                      Cuenta2Digitos = b.v_NroCuenta.Substring(0, 1),
                                  }).ToList();


                    var tesoreria = (from a in dbcontext.tesoreria
                                     join b in dbcontext.tesoreriadetalle on new { d = a.v_IdTesoreria, eliminado = 0 } equals new { d = b.v_IdTesoreria, eliminado = b.i_Eliminado.Value } into b_join
                                     from b in b_join.DefaultIfEmpty()
                                     join c in dbcontext.documento on new { d = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { d = c.i_CodigoDocumento, eliminado = 0 } into c_join
                                     from c in c_join.DefaultIfEmpty()
                                     where a.i_Eliminado == 0 && (b.v_NroCuenta.StartsWith("9") || b.v_NroCuenta.StartsWith("79") || b.v_NroCuenta.StartsWith("62") || b.v_NroCuenta.StartsWith("63") || b.v_NroCuenta.StartsWith("64") || b.v_NroCuenta.StartsWith("65") || b.v_NroCuenta.StartsWith("66") || b.v_NroCuenta.StartsWith("67") || b.v_NroCuenta.StartsWith("68"))
                                     && a.t_FechaRegistro >= FechaInicio && a.t_FechaRegistro <= FechaFin
                                     select new ReporteAnalisisHojaTrabajo
                                     {
                                         NroDiario = c.v_Siglas + " " + a.v_Mes + " " + a.v_Correlativo,
                                         v_IdDiario = a.v_IdTesoreria,
                                         Cuenta = b.v_NroCuenta,
                                         Mes = a.v_Mes,
                                         Importe = b.d_Importe ?? 0,
                                         Naturaleza = b.v_Naturaleza,
                                         Debe = b.v_Naturaleza == "D" ? b.d_Importe.Value : 0,
                                         Haber = b.v_Naturaleza == "H" ? b.d_Importe.Value : 0,
                                         Cambio = b.d_Cambio ?? 0,
                                         DebeCambio = b.v_Naturaleza == "D" ? b.d_Cambio.Value : 0,
                                         HaberCambio = b.v_Naturaleza == "H" ? b.d_Cambio.Value : 0,
                                         Cuenta2Digitos = b.v_NroCuenta.Substring(0, 1),
                                     }).ToList();
                    var Totales = diario.Concat(tesoreria).ToList();

                    var TotalesAgrupados = Totales.GroupBy(h => h.v_IdDiario).ToList();
                    List<ReporteAnalisisHojaTrabajo> ListaFinal = new List<ReporteAnalisisHojaTrabajo>();
                    foreach (var item in TotalesAgrupados)
                    {
                        decimal cuenta60Importe = Totales.Where(j => j.v_IdDiario == item.FirstOrDefault().v_IdDiario && j.Cuenta.StartsWith("6")).Select(k => k.Importe).Sum();
                        decimal cuenta60Cambio = Totales.Where(j => j.v_IdDiario == item.FirstOrDefault().v_IdDiario && j.Cuenta.StartsWith("6")).Select(k => k.Cambio).Sum();
                        decimal cuenta79Importe = Totales.Where(j => j.v_IdDiario == item.FirstOrDefault().v_IdDiario && j.Cuenta.StartsWith("7")).Select(k => k.Importe).Sum();
                        decimal cuenta79Cambio = Totales.Where(j => j.v_IdDiario == item.FirstOrDefault().v_IdDiario && j.Cuenta.StartsWith("7")).Select(k => k.Cambio).Sum();
                        decimal cuenta9Importe = Totales.Where(j => j.v_IdDiario == item.FirstOrDefault().v_IdDiario && j.Cuenta.StartsWith("9")).Select(k => k.Importe).Sum();
                        decimal cuenta9Cambio = Totales.Where(j => j.v_IdDiario == item.FirstOrDefault().v_IdDiario && j.Cuenta.StartsWith("9")).Select(k => k.Cambio).Sum();

                        if (cuenta60Importe == cuenta79Importe && cuenta79Importe == cuenta9Importe && cuenta60Cambio == cuenta79Cambio && cuenta79Cambio == cuenta9Cambio)
                        {
                        }
                        else
                        {
                            var Detalles = Totales.Where(k => k.v_IdDiario == item.FirstOrDefault().v_IdDiario).ToList();
                            foreach (var itemDetalles in Detalles)
                            {
                                ListaFinal.Add(itemDetalles);
                            }

                        }

                    }

                    return ListaFinal;


                }
            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                return null;
            }





        }

        #region Análisis de Cuentas
        /// <summary>
        /// Genera el asiento de diferencia de cambio decuerdo al mes ingresado. EQC 25/07/2015
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="objCabeceraDto"></param>
        /// <param name="CuentasDiferenciaCambio"></param>
        /// <param name="CuentaGanancia"></param>
        /// <param name="CuentaPerdida"></param>
        /// <param name="ClientSession"></param>
        public void GeneraAsientoPorDiferenciaCambio(ref OperationResult pobjOperationResult, diarioDto objCabeceraDto, List<string> CuentasDiferenciaCambio, string CuentaGanancia, string CuentaPerdida, List<string> ClientSession, out int AsientosGenerados)
        {
            AsientosGenerados = 0;
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    var objListaDiarioDetalles = new List<diariodetalleDto>();
                    var diarioDetalleContracuentaGanancia = new diariodetalleDto();
                    var diarioDetalleContracuentaPerdida = new diariodetalleDto();
                    var agregarCtaGanancia = false;
                    var agregarCtaPerdida = false;

                    #region Recolecta las cuentas a procesar
                    var mes = objCabeceraDto.t_Fecha.Value.Month;
                    var periodo = objCabeceraDto.t_Fecha.Value.Year;
                    var fechaInicio = new DateTime(periodo, 1, 1);
                    var fechaFin = DateTime.Parse(new DateTime(periodo, mes, DateTime.DaysInMonth(periodo, mes)).ToShortDateString() + " 23:59");
                    var registrosDiarioTesoreria = ReporteAnalisisCuentasCteAnalitico(ref pobjOperationResult, fechaInicio, fechaFin, null, null, false, null, TipoReporteAnalisis.AnalisisCuentaCorrienteAnalitico);
                    if (pobjOperationResult.Success == 0) return;

                    if (registrosDiarioTesoreria != null)
                    {
                        foreach (string cuentaDiferenciaCambio in CuentasDiferenciaCambio)
                        {
                            var registrosPorCuenta = registrosDiarioTesoreria.Where(p => p.CuentaImputable == cuentaDiferenciaCambio).ToList();
                            if (registrosPorCuenta.Count > 0)
                            {
                                if (Utils.Windows.CuentaRequiereDetalle(cuentaDiferenciaCambio))
                                {
                                    #region Operacion por anexo, tipo de documento y nro documento
                                    //Las cuentas que tienen detalle se agrupan por Anexo, Documento y NroDOcumento
                                    var registrosPorDetalleDocumento = registrosPorCuenta.GroupBy(g => new { anexo = g.IdAnexo, tipoDoc = g.TipoDocumento /*== 7 ? g.TipoDocumentoRef : g.TipoDocumento*/, nroDoc = g.NroDocumento }).ToList();

                                    //se recorren los valores agrupados, por cada grupo representa una fila en el asiento de diario.
                                    foreach (var Reg in registrosPorDetalleDocumento)
                                    {
                                        if (Reg.Key.anexo == "N001-CL000000232" && Reg.Key.nroDoc == "FAC F005-00001596")//<Para analizar en agrofergi.
                                        {
                                            var x = 1;
                                        }
                                        var diarioDetalle = new diariodetalleDto();
                                        var registroProvicional = Reg.Key.tipoDoc != 7 ? Reg.Where(p => p.NaturalezaCuenta == p.NaturalezaRegistro).ToList() : Reg.Where(p => p.NaturalezaCuenta != p.NaturalezaRegistro).ToList();
                                        var registroCancelacion = Reg.Key.tipoDoc != 7 ? Reg.Where(p => p.NaturalezaCuenta != p.NaturalezaRegistro).ToList() : Reg.Where(p => p.NaturalezaCuenta == p.NaturalezaRegistro).ToList();

                                        if (registroProvicional.Count > 0)
                                        {
                                            //Las cuentas que son diferentes a 10 se ajustan segun la moneda de su provicion,
                                            //las que comienzan con 10 se ajustan segun la moneda de su detalle de la cuenta.
                                            if (cuentaDiferenciaCambio.StartsWith("10")
                                                ? !registroProvicional.Any(p => p.IdMonedaCuenta.Equals(2))
                                                : !registroProvicional.Any(p => p.IdMoneda.Equals(2))) continue;

                                            decimal saldoSoles, saldoDolares;
                                            var tipoCambioFdm = objCabeceraDto.d_TipoCambio ?? 1;

                                            if (Reg.Key.tipoDoc != 7)    //-> excepcion a la regla porque una nota de credito juega como cancelacion
                                            {
                                                if (registroProvicional.FirstOrDefault().NaturalezaCuenta == "D")
                                                {
                                                    saldoSoles = registroProvicional.Sum(p => p.ImporteSolesD ?? 0) - registroCancelacion.Sum(p => p.ImporteSolesH ?? 0);
                                                    saldoDolares = registroProvicional.Sum(p => p.ImporteDolaresD ?? 0) - registroCancelacion.Sum(p => p.ImporteDolaresH ?? 0);
                                                }
                                                else
                                                {
                                                    saldoSoles = registroProvicional.Sum(p => p.ImporteSolesH ?? 0) - registroCancelacion.Sum(p => p.ImporteSolesD ?? 0);
                                                    saldoDolares = registroProvicional.Sum(p => p.ImporteDolaresH ?? 0) - registroCancelacion.Sum(p => p.ImporteDolaresD ?? 0);
                                                }
                                            }
                                            else
                                            {
                                                if (registroProvicional.FirstOrDefault().NaturalezaCuenta == "H")
                                                {
                                                    saldoSoles = registroProvicional.Sum(p => p.ImporteSolesD ?? 0) - registroCancelacion.Sum(p => p.ImporteSolesH ?? 0);
                                                    saldoDolares = registroProvicional.Sum(p => p.ImporteDolaresD ?? 0) - registroCancelacion.Sum(p => p.ImporteDolaresH ?? 0);
                                                }
                                                else
                                                {
                                                    saldoSoles = registroProvicional.Sum(p => p.ImporteSolesH ?? 0) - registroCancelacion.Sum(p => p.ImporteSolesD ?? 0);
                                                    saldoDolares = registroProvicional.Sum(p => p.ImporteDolaresH ?? 0) - registroCancelacion.Sum(p => p.ImporteDolaresD ?? 0);
                                                }
                                            }


                                            var saldoSolesAjuste = (saldoDolares * tipoCambioFdm) - saldoSoles;

                                            diarioDetalle.d_Importe = decimal.Parse(Math.Round(saldoSolesAjuste > 0 ? saldoSolesAjuste : saldoSolesAjuste * -1, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                                            diarioDetalle.d_Cambio = 0;
                                            diarioDetalle.i_IdCentroCostos = "0";
                                            diarioDetalle.i_IdTipoDocumento = registroProvicional.FirstOrDefault().TipoDocumento;
                                            diarioDetalle.t_Fecha = registroProvicional.FirstOrDefault().FechaDocumento;
                                            diarioDetalle.v_IdCliente = Reg.Key.anexo;
                                            diarioDetalle.v_Naturaleza = registroProvicional.FirstOrDefault().NaturalezaRegistro == "D" ? saldoSolesAjuste > 0 ? "D" : "H" : saldoSolesAjuste > 0 ? "H" : "D";
                                            diarioDetalle.v_NroDocumento = registroProvicional.FirstOrDefault().DocumentoRaw;
                                            diarioDetalle.v_NroCuenta = registroProvicional.FirstOrDefault().CuentaImputable;
                                            diarioDetalle.i_IdTipoDocumentoRef = Reg.Key.tipoDoc == 7 || Reg.Key.tipoDoc == 8 ? registroProvicional.FirstOrDefault().TipoDocumentoRef : -1;
                                            diarioDetalle.v_NroDocumentoRef = Reg.Key.tipoDoc == 7 || Reg.Key.tipoDoc == 8 ? registroProvicional.FirstOrDefault().DocumentoRefRaw : string.Empty;
                                            objListaDiarioDetalles.Add(diarioDetalle);
                                        }
                                        else
                                        {
                                            decimal saldoSoles, saldoDolares;
                                            var tipoCambioFdm = objCabeceraDto.d_TipoCambio ?? 1;
                                            if (!registroCancelacion.Any(p => p.IdMoneda.Equals(2))) continue;
                                            if (Reg.Key.tipoDoc != 7 && registroCancelacion.FirstOrDefault().NaturalezaCuenta == "D")
                                            {
                                                saldoSoles = registroCancelacion.Where(x => x.ImporteSolesH != null).Sum(p => p.ImporteSolesH.Value);
                                                saldoDolares = registroCancelacion.Where(x => x.ImporteDolaresH != null).Sum(p => p.ImporteDolaresH.Value);
                                            }
                                            else
                                            {
                                                saldoSoles = registroCancelacion.Where(x => x.ImporteSolesH != null).Sum(p => p.ImporteSolesD.Value);
                                                saldoDolares = registroCancelacion.Where(x => x.ImporteDolaresH != null).Sum(p => p.ImporteDolaresD.Value);
                                            }

                                            var saldoSolesAjuste = (saldoDolares * tipoCambioFdm) - saldoSoles;

                                            diarioDetalle.d_Importe = decimal.Parse(Math.Round(saldoSolesAjuste > 0 ? saldoSolesAjuste : saldoSolesAjuste * -1, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                                            diarioDetalle.d_Cambio = 0;
                                            diarioDetalle.i_IdCentroCostos = "0";
                                            diarioDetalle.i_IdTipoDocumento = registroCancelacion.FirstOrDefault().TipoDocumento;
                                            diarioDetalle.t_Fecha = registroCancelacion.FirstOrDefault().FechaDocumento;
                                            diarioDetalle.v_IdCliente = Reg.Key.anexo;
                                            diarioDetalle.v_Naturaleza = registroCancelacion.FirstOrDefault().NaturalezaRegistro == "D" ? saldoSolesAjuste > 0 ? "D" : "H" : saldoSolesAjuste > 0 ? "H" : "D";
                                            diarioDetalle.v_NroDocumento = registroCancelacion.FirstOrDefault().DocumentoRaw;
                                            diarioDetalle.v_NroCuenta = registroCancelacion.FirstOrDefault().CuentaImputable;
                                            diarioDetalle.i_IdTipoDocumentoRef = -1;
                                            diarioDetalle.v_NroDocumentoRef = string.Empty;
                                            objListaDiarioDetalles.Add(diarioDetalle);
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region Operacion de cuentas sin detalle
                                    var diarioDetalle = new diariodetalleDto();
                                    var registroProvicional = registrosPorCuenta.Where(p => p.NaturalezaCuenta == p.NaturalezaRegistro).ToList();
                                    var registroCancelacion = registrosPorCuenta.Where(p => p.NaturalezaCuenta != p.NaturalezaRegistro).ToList();

                                    if (registroProvicional.Count > 0)
                                    {
                                        if (cuentaDiferenciaCambio.StartsWith("10") ? !registroProvicional.Any(p => p.IdMonedaCuenta.Equals(2)) : !registroProvicional.Any(p => p.IdMoneda.Equals(2))) continue;
                                        decimal saldoSoles, saldoDolares;
                                        var tipoCambioFdm = objCabeceraDto.d_TipoCambio ?? 1;

                                        if (registroProvicional.FirstOrDefault().NaturalezaCuenta == "D")
                                        {
                                            saldoSoles = registroProvicional.Sum(p => p.ImporteSolesD ?? 0) - registroCancelacion.Sum(p => p.ImporteSolesH ?? 0);
                                            saldoDolares = registroProvicional.Sum(p => p.ImporteDolaresD ?? 0) - registroCancelacion.Sum(p => p.ImporteDolaresH ?? 0);
                                        }
                                        else
                                        {
                                            saldoSoles = registroProvicional.Sum(p => p.ImporteSolesH ?? 0) - registroCancelacion.Sum(p => p.ImporteSolesD ?? 0);
                                            saldoDolares = registroProvicional.Sum(p => p.ImporteDolaresH ?? 0) - registroCancelacion.Sum(p => p.ImporteDolaresD ?? 0);
                                        }

                                        var saldoSolesAjuste = (saldoDolares * tipoCambioFdm) - saldoSoles;

                                        diarioDetalle.d_Importe = decimal.Parse(Math.Round(saldoSolesAjuste > 0 ? saldoSolesAjuste : saldoSolesAjuste * -1, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                                        diarioDetalle.d_Cambio = 0;
                                        diarioDetalle.i_IdCentroCostos = "0";
                                        diarioDetalle.i_IdTipoDocumento = registroProvicional.FirstOrDefault().TipoDocumento;
                                        diarioDetalle.t_Fecha = registroProvicional.FirstOrDefault().FechaDocumento;
                                        diarioDetalle.v_Naturaleza = registroProvicional.FirstOrDefault().NaturalezaRegistro == "D" ? saldoSolesAjuste > 0 ? "D" : "H" : saldoSolesAjuste > 0 ? "H" : "D";
                                        diarioDetalle.v_NroDocumento = registroProvicional.FirstOrDefault().DocumentoRaw;
                                        diarioDetalle.v_NroCuenta = registroProvicional.FirstOrDefault().CuentaImputable;
                                        objListaDiarioDetalles.Add(diarioDetalle);
                                    }
                                    #endregion
                                }
                            }
                        }
                    }

                    objListaDiarioDetalles = objListaDiarioDetalles.Where(p => (p.d_Importe ?? 0) > 0).ToList();
                    #endregion

                    #region Agrega las contracuentas de ganancia y/o perdida
                    var CuentasD = objListaDiarioDetalles.Where(p => p.v_Naturaleza == "D").ToList();

                    if (CuentasD.Count > 0)
                    {
                        diarioDetalleContracuentaGanancia.d_Importe = CuentasD.Sum(p => p.d_Importe.Value);
                        diarioDetalleContracuentaGanancia.d_Cambio = 0;
                        diarioDetalleContracuentaGanancia.i_IdCentroCostos = "0";
                        diarioDetalleContracuentaGanancia.i_IdTipoDocumento = objCabeceraDto.i_IdTipoDocumento ?? -1;
                        diarioDetalleContracuentaGanancia.t_Fecha = objCabeceraDto.t_Fecha;
                        diarioDetalleContracuentaGanancia.v_Naturaleza = "H";
                        diarioDetalleContracuentaGanancia.v_NroDocumento = objCabeceraDto.v_Mes + "-" + objCabeceraDto.v_Correlativo;
                        diarioDetalleContracuentaGanancia.v_NroCuenta = CuentaGanancia;
                        agregarCtaGanancia = true;
                    }

                    var CuentasH = objListaDiarioDetalles.Where(p => p.v_Naturaleza == "H").ToList();

                    if (CuentasH.Count > 0)
                    {
                        diarioDetalleContracuentaPerdida.d_Importe = CuentasH.Sum(p => p.d_Importe ?? 0);
                        diarioDetalleContracuentaPerdida.d_Cambio = 0;
                        diarioDetalleContracuentaPerdida.i_IdCentroCostos = "0";
                        diarioDetalleContracuentaPerdida.i_IdTipoDocumento = objCabeceraDto.i_IdTipoDocumento ?? -1;
                        diarioDetalleContracuentaPerdida.t_Fecha = objCabeceraDto.t_Fecha;
                        diarioDetalleContracuentaPerdida.v_Naturaleza = "D";
                        diarioDetalleContracuentaPerdida.v_NroDocumento = objCabeceraDto.v_Mes + "-" + objCabeceraDto.v_Correlativo;
                        diarioDetalleContracuentaPerdida.v_NroCuenta = CuentaPerdida;
                        agregarCtaPerdida = true;
                    }

                    if (agregarCtaGanancia)
                        objListaDiarioDetalles.Add(diarioDetalleContracuentaGanancia);

                    if (agregarCtaPerdida)
                        objListaDiarioDetalles.Add(diarioDetalleContracuentaPerdida);
                    #endregion

                    #region Suma los totales de la cabecera
                    decimal TotDebe, TotHaber, TotDebeC, TotHaberC;
                    TotDebe = objListaDiarioDetalles.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Importe.Value);
                    TotHaber = objListaDiarioDetalles.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Importe.Value);
                    TotDebeC = 0;
                    TotHaberC = 0;

                    objCabeceraDto.d_TotalDebe = decimal.Parse(Math.Round(TotDebe, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                    objCabeceraDto.d_TotalHaber = decimal.Parse(Math.Round(TotHaber, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                    objCabeceraDto.d_TotalDebeCambio = decimal.Parse(Math.Round(TotDebeC, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                    objCabeceraDto.d_TotalHaberCambio = decimal.Parse(Math.Round(TotHaberC, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                    objCabeceraDto.d_DiferenciaDebe = decimal.Parse(Math.Round(TotDebe - TotHaber, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                    objCabeceraDto.d_DiferenciaHaber = decimal.Parse(Math.Round(TotDebeC - TotHaberC, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                    AsientosGenerados = objListaDiarioDetalles.Count;
                    #endregion

                    #region Inserta el asiento de diario
                    if (AsientosGenerados > 0 && registrosDiarioTesoreria != null)
                    {
                        new DiarioBL().InsertarDiario(ref pobjOperationResult, objCabeceraDto, ClientSession, objListaDiarioDetalles, (int)TipoMovimientoTesoreria.Ingreso);
                        if (pobjOperationResult.Success == 0) return;
                    }
                    #endregion

                    pobjOperationResult.Success = 1;
                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "DiarioBL.GeneraAsientoPorDiferenciaCambio() \nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public List<ReporteAnalisisCuentas> ReporteAnalisisCuentas(ref OperationResult pobjOperationResult, DateTime F_Ini, DateTime F_Fin, string nroCuenta = null, string nroDocumentoIdentidad = null, bool mostrarSoloPendientes = true, int TipoComprobante = -1)
        {
            try
            {
                F_Ini = F_Ini.Date;
                F_Fin = DateTime.Parse(F_Fin.Day + "/" + F_Fin.Month + "/" + F_Fin.Year + " 23:59");

                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var periodo = F_Ini.Year.ToString();
                    var cuentasDetallesDiccionario = dbContext.asientocontable
                        .Where(p => p.v_Periodo.Equals(periodo) && p.i_Eliminado == 0)
                        .ToDictionary(dd => dd.v_NroCuenta, p => p.i_Detalle ?? 0);

                    nroCuenta = string.IsNullOrWhiteSpace(nroCuenta) ? string.Empty : nroCuenta.Trim();
                    nroDocumentoIdentidad = string.IsNullOrWhiteSpace(nroDocumentoIdentidad) ? string.Empty : nroDocumentoIdentidad.Trim();

                    var reporteResultante = new List<ReporteAnalisisCuentas>();

                    #region Recopila las Cuentas Mayores
                    var cuentasMayores = (from n in dbContext.asientocontable
                                          where n.i_LongitudJerarquica == 2 && n.i_Eliminado == 0 && n.v_Periodo == periodo
                                          select n).ToList();

                    #endregion

                    #region Recopila Cuentas de Tesorería
                    List<tesoreriadetalle> tesoreriaDetalle;
                    var queryToExecute = ObtenerConsulta(1, F_Ini, F_Fin, nroCuenta, nroDocumentoIdentidad);
                    switch (Globals.TipoMotor)
                    {
                        case TipoMotorBD.PostgreSQL:
                            using (var cnx = new PgSqlConnection(Globals.CadenaConexion))
                            {
                                tesoreriaDetalle = cnx.Query<tesoreriadetalle>(queryToExecute).ToList();
                            }
                            break;

                        default:
                            using (var cnx = new SqlConnection(Globals.CadenaConexion))
                            {
                                tesoreriaDetalle = cnx.Query<tesoreriadetalle>(queryToExecute).ToList();
                            }
                            break;
                    }

                    var gruposTesoreriaSource = (from n in tesoreriaDetalle
                                                 join J1 in dbContext.asientocontable on new { cuenta = n.v_NroCuenta.Trim(), eliminado = 0, p = periodo }
                                                     equals new { cuenta = J1.v_NroCuenta.Trim(), eliminado = J1.i_Eliminado ?? 0, p = J1.v_Periodo } into J1_join
                                                 from J1 in J1_join.DefaultIfEmpty()
                                                 join J2 in dbContext.documento on n.i_IdTipoDocumento ?? 0 equals
                                                     J2.i_CodigoDocumento into J2_join
                                                 from J2 in J2_join.DefaultIfEmpty()
                                                 join J3 in dbContext.cliente on n.v_IdCliente equals J3.v_IdCliente into J3_join
                                                 from J3 in J3_join.DefaultIfEmpty()
                                                 join J4 in dbContext.tesoreria on n.v_IdTesoreria equals J4.v_IdTesoreria into
                                                     J4_join
                                                 from J4 in J4_join.DefaultIfEmpty()
                                                 join J5 in dbContext.documento on n.i_IdTipoDocumentoRef ?? 0 equals
                                                     J5.i_CodigoDocumento into J5_join
                                                 from J5 in J5_join.DefaultIfEmpty()
                                                 join J6 in dbContext.documento on J4.i_IdTipoDocumento ?? 0 equals
                                                     J6.i_CodigoDocumento into J6_join
                                                 from J6 in J6_join.DefaultIfEmpty()
                                                 where J4.i_IdEstado == 1 &&
                                                       J1_join.Any(p => p.v_NroCuenta.Equals(n.v_NroCuenta))
                                                 select new
                                                 {
                                                     CuentaMayor = n.v_NroCuenta.Substring(0, 2),
                                                     CuentaImputable = n.v_NroCuenta,
                                                     NombreCuenta = J1 != null ? J1.v_NombreCuenta : "",
                                                     FechaDocumento = J4.t_FechaRegistro,
                                                     Detalle =
                                                         J3 != null
                                                             ? J3.v_CodCliente + " " + n.v_IdCliente != "N002-CL000000000"
                                                                 ? (J3.v_ApePaterno + " " + J3.v_ApeMaterno + " " + J3.v_PrimerNombre + " " +
                                                                    J3.v_RazonSocial).Trim()
                                                                 : "PÚBLICO GENERAL"
                                                             : "SIN DETALLE",
                                                     DocumentoTransaccion =
                                                         J6 != null ? J6.v_Siglas + " " + J4.v_Mes.Trim() + J4.v_Correlativo.Trim() : "",
                                                     Documento = J2 != null ? J2.v_Siglas + " " + n.v_NroDocumento : string.Empty,
                                                     DocumentoRef = J5 != null ? J5.v_Siglas + " " + n.v_NroDocumentoRef : "",
                                                     Analisis = n.v_Analisis ?? (!string.IsNullOrEmpty(J4.v_Glosa) ? J4.v_Glosa : ""),
                                                     IdMoneda = J4.i_IdMoneda,
                                                     SiglaMoneda = J4 != null ? J4.i_IdMoneda == 1 ? "S" : "D" : string.Empty,
                                                     Importe = n.d_Importe,
                                                     ImporteCambio = n.d_Cambio,
                                                     Key =
                                                         J3 != null && J2 != null
                                                             ? J3.v_IdCliente + n.v_NroCuenta + J2.v_Siglas + n.v_NroDocumento
                                                             : string.Empty,
                                                     NaturalezaCuenta = J1 != null ? J1.i_Naturaleza : 0,
                                                     IdMonedaCuenta = J1 != null ? J1.i_IdMoneda : 0,
                                                     NaturalezaRegistro = n.v_Naturaleza,
                                                     CuentaConDetalle = J1 != null && J1.i_Detalle == 1,
                                                     TipoCambio = J4 != null ? J4.d_TipoCambio : 0,
                                                     IdAnexo = J3 != null ? J3.v_IdCliente : string.Empty,
                                                     Detalle_Ruc = J3 != null ? J3.v_NroDocIdentificacion : "",
                                                     TipoDocumento = n.i_IdTipoDocumento,
                                                     TipoDocumentoRef = n.i_IdTipoDocumentoRef,
                                                     NroDocumento = n.v_NroDocumento,
                                                     NroDocumentoRef = n.v_NroDocumentoRef,
                                                     NombreRazonSocial =
                                                         J3 != null && n.v_IdCliente != "N002-CL000000000"
                                                             ? (J3.v_ApePaterno + " " + J3.v_ApeMaterno + " " + J3.v_PrimerNombre + " " +
                                                                J3.v_RazonSocial).Trim()
                                                             : "PÚBLICO GENERAL",
                                                     Doc = J2 != null ? n.v_NroDocumento : string.Empty,
                                                     DocFecha = J4 != null ? J4.t_FechaRegistro : new DateTime(2016, 01, 01),
                                                     CodAnexo = J3 != null ? J3.v_CodCliente : "",
                                                     DocTipo = J2 != null ? J2.v_Siglas : string.Empty,
                                                 });

                    if (!string.IsNullOrWhiteSpace(nroDocumentoIdentidad))
                        gruposTesoreriaSource = gruposTesoreriaSource
                            .Where(p => p.Detalle_Ruc.Equals(nroDocumentoIdentidad));

                    var gruposTesoreria = gruposTesoreriaSource
                        .ToList()
                        .Select(p => new ReporteAnalisisCuentas
                        {
                            CuentaMayor = p.CuentaMayor,
                            CuentaImputable = p.CuentaImputable,
                            NombreCuenta = p.NombreCuenta,
                            Detalle = p.CuentaConDetalle ? p.Detalle_Ruc + ": " + p.Detalle : "",
                            Documento =
                                p.CuentaConDetalle
                                    ? (p.FechaDocumento.Value.ToShortDateString() + " \t" + p.Documento).Trim() + " \t" +
                                      p.Analisis
                                    : "",
                            DocumentoRef = !string.IsNullOrEmpty(p.DocumentoRef) ? (p.DocumentoRef).Trim() : "",
                            FechaDocumento = p.FechaDocumento,
                            DocumentoKey = p.Key,
                            IdMoneda = (int)p.IdMoneda,
                            IdMonedaCuenta = (int)p.IdMonedaCuenta,
                            NaturalezaCuenta = p.NaturalezaCuenta == 1 ? "D" : "H",
                            NaturalezaRegistro = p.NaturalezaRegistro,
                            ImporteSolesD =
                                p.NaturalezaRegistro == "D" ? p.IdMoneda == 1 ? p.Importe : p.ImporteCambio : 0,
                            ImporteSolesH =
                                p.NaturalezaRegistro == "H" ? p.IdMoneda == 1 ? p.Importe : p.ImporteCambio : 0,
                            ImporteDolaresD =
                                p.NaturalezaRegistro == "D" ? p.IdMoneda == 1 ? p.ImporteCambio : p.Importe : 0,
                            ImporteDolaresH =
                                p.NaturalezaRegistro == "H" ? p.IdMoneda == 1 ? p.ImporteCambio : p.Importe : 0,
                            CuentaConDetalle = p.CuentaConDetalle,
                            IdAnexo = p.IdAnexo,
                            TipoDocumento =
                                !_objDocumentoBL.DocumentoEsInverso(p.TipoDocumento ?? -1) && p.TipoDocumento != 8
                                    ? p.TipoDocumento ?? 0
                                    : p.TipoDocumentoRef ?? 0,
                            NroDocumento =
                                !_objDocumentoBL.DocumentoEsInverso(p.TipoDocumento ?? -1) && p.TipoDocumento != 8
                                    ? p.NroDocumento
                                    : p.NroDocumentoRef,
                            NombreRazonSocial = p.NombreRazonSocial,
                            Doc = p.Doc,
                            DocFecha = p.DocFecha,
                            CodAnexo = p.CodAnexo,
                            DocTipo = p.DocTipo
                        }
                        ).ToList();

                    gruposTesoreria = gruposTesoreria.Where(p => !string.IsNullOrEmpty(p.NombreCuenta)).ToList();
                    #endregion

                    #region Recopila Cuentas de Diario
                    List<diariodetalle> diarioDetalle;
                    queryToExecute = ObtenerConsulta(2, F_Ini, F_Fin, nroCuenta, nroDocumentoIdentidad);
                    switch (Globals.TipoMotor)
                    {
                        case TipoMotorBD.PostgreSQL:
                            using (var cnx = new PgSqlConnection(Globals.CadenaConexion))
                            {
                                diarioDetalle = cnx.Query<diariodetalle>(queryToExecute).ToList();
                            }
                            break;

                        default:
                            using (var cnx = new SqlConnection(Globals.CadenaConexion))
                            {
                                diarioDetalle = cnx.Query<diariodetalle>(queryToExecute).ToList();
                            }
                            break;
                    }

                    var gruposDiarioSource = (from n in diarioDetalle
                                              join J1 in dbContext.asientocontable on new { cuenta = n.v_NroCuenta.Trim(), eliminado = 0, p = periodo }
                                                          equals new { cuenta = J1.v_NroCuenta.Trim(), eliminado = J1.i_Eliminado ?? 0, p = J1.v_Periodo } into J1_join
                                              from J1 in J1_join.DefaultIfEmpty()
                                              join J2 in dbContext.documento on n.i_IdTipoDocumento ?? 0 equals
                                                  J2.i_CodigoDocumento into J2_join
                                              from J2 in J2_join.DefaultIfEmpty()
                                              join J3 in dbContext.cliente on n.v_IdCliente equals J3.v_IdCliente into J3_join
                                              from J3 in J3_join.DefaultIfEmpty()
                                              join J4 in dbContext.diario on n.v_IdDiario equals J4.v_IdDiario into J4_join
                                              from J4 in J4_join.DefaultIfEmpty()
                                              join J5 in dbContext.documento on n.i_IdTipoDocumentoRef ?? 0 equals
                                                  J5.i_CodigoDocumento into J5_join
                                              from J5 in J5_join.DefaultIfEmpty()
                                              join J6 in dbContext.documento on J4.i_IdTipoDocumento ?? 0 equals
                                                  J6.i_CodigoDocumento into J6_join
                                              from J6 in J6_join.DefaultIfEmpty()
                                              where J1_join.Any(p => p.v_NroCuenta.Equals(n.v_NroCuenta))
                                              && (J4.i_IdTipoComprobante != TipoComprobante || TipoComprobante == -1)
                                              select new
                                              {
                                                  CuentaMayor = n.v_NroCuenta.Substring(0, 2),
                                                  CuentaImputable = n.v_NroCuenta,
                                                  NombreCuenta = J1 != null ? J1.v_NombreCuenta : "",
                                                  FechaDocumento = J4 != null ? J4.t_Fecha : new DateTime(2016, 01, 01),
                                                  Detalle =
                                                      J3 != null
                                                          ? J3.v_CodCliente + " " + n.v_IdCliente != "N002-CL000000000"
                                                              ? (J3.v_ApePaterno + " " + J3.v_ApeMaterno + " " + J3.v_PrimerNombre + " " +
                                                                 J3.v_RazonSocial).Trim()
                                                              : "PÚBLICO GENERAL"
                                                          : "SIN DETALLE",
                                                  DocumentoTransaccion = J4 != null ? J4.i_IdTipoComprobante.Value : 0,
                                                  Documento = J2 != null ? J2.v_Siglas + " " + n.v_NroDocumento : string.Empty,
                                                  DocumentoRef =
                                                      J5 != null && n.v_NroDocumentoRef != null ? J5.v_Siglas + " " + n.v_NroDocumentoRef : "",
                                                  Analisis = n.v_Analisis ?? (!string.IsNullOrEmpty(J4.v_Glosa) ? J4.v_Glosa : ""),
                                                  IdMoneda = J4 != null ? J4.i_IdMoneda : 1,
                                                  IdMonedaCuenta = J1 != null ? J1.i_IdMoneda : 0,
                                                  SiglaMoneda = J4.i_IdMoneda == 1 ? "S" : "D",
                                                  Importe = n.d_Importe,
                                                  ImporteCambio = n.d_Cambio,
                                                  Key =
                                                      J3 != null && J2 != null
                                                          ? J3.v_IdCliente + n.v_NroCuenta + J2.v_Siglas + n.v_NroDocumento
                                                          : string.Empty,
                                                  NaturalezaCuenta = J1 != null ? J1.i_Naturaleza : 0,
                                                  NaturalezaRegistro = n.v_Naturaleza,
                                                  CuentaConDetalle = J1 != null && J1.i_Detalle == 1,
                                                  TipoCambio = J4.d_TipoCambio,
                                                  IdAnexo = J3 != null ? J3.v_IdCliente : string.Empty,
                                                  Detalle_Ruc = J3 != null ? J3.v_NroDocIdentificacion : "",
                                                  TipoDocumento = n.i_IdTipoDocumento,
                                                  TipoDocumentoRef = n.i_IdTipoDocumentoRef,
                                                  NroDocumento = n.v_NroDocumento,
                                                  NroDocumentoRef = n.v_NroDocumentoRef,
                                                  NombreRazonSocial =
                                                      J3 != null && n.v_IdCliente != "N002-CL000000000"
                                                          ? (J3.v_ApePaterno + " " + J3.v_ApeMaterno + " " + J3.v_PrimerNombre + " " +
                                                             J3.v_RazonSocial).Trim()
                                                          : "PÚBLICO GENERAL",
                                                  Doc = J2 != null ? n.v_NroDocumento : string.Empty,
                                                  DocFecha = J4 != null ? J4.t_Fecha : new DateTime(2016, 01, 01),
                                                  CodAnexo = J3 != null ? J3.v_CodCliente : string.Empty,
                                                  DocTipo = J2 != null ? J2.v_Siglas : string.Empty,
                                                  VentaDetraccion = J4 != null && J4.i_AfectaDetraccion == 1,
                                                  TasaDetraccion = J4 != null ? (J4.d_TasaDetraccion ?? 0m) : 0m
                                              }
                        );

                    if (!string.IsNullOrWhiteSpace(nroDocumentoIdentidad))
                        gruposDiarioSource = gruposDiarioSource
                            .Where(p => p.Detalle_Ruc.Equals(nroDocumentoIdentidad));

                    var gruposDiario = gruposDiarioSource.ToList()
                        .Select(p => new ReporteAnalisisCuentas
                        {
                            CuentaMayor = p.CuentaMayor,
                            CuentaImputable = p.CuentaImputable,
                            NombreCuenta = p.NombreCuenta,
                            Detalle = p.CuentaConDetalle ? p.Detalle_Ruc + ": " + p.Detalle : "",
                            Documento =
                                p.CuentaConDetalle
                                    ? (p.FechaDocumento.Value.ToShortDateString() + " \t" + p.Documento).Trim() + " \t" +
                                      p.Analisis
                                    : "",
                            DocumentoRef = !string.IsNullOrEmpty(p.DocumentoRef) ? (p.DocumentoRef).Trim() : "",
                            FechaDocumento = p.FechaDocumento,
                            DocumentoKey = p.Key,
                            IdMoneda = (int)p.IdMoneda,
                            IdMonedaCuenta = (int)p.IdMonedaCuenta,
                            NaturalezaCuenta = p.NaturalezaCuenta == 1 ? "D" : "H",
                            NaturalezaRegistro = p.NaturalezaRegistro,
                            ImporteSolesD =
                                p.NaturalezaRegistro == "D" ? p.IdMoneda == 1 ? p.Importe : p.ImporteCambio : 0,
                            ImporteSolesH =
                                p.NaturalezaRegistro == "H" ? p.IdMoneda == 1 ? p.Importe : p.ImporteCambio : 0,
                            ImporteDolaresD =
                                p.NaturalezaRegistro == "D" ? p.IdMoneda == 1 ? p.ImporteCambio : p.Importe : 0,
                            ImporteDolaresH =
                                p.NaturalezaRegistro == "H" ? p.IdMoneda == 1 ? p.ImporteCambio : p.Importe : 0,
                            CuentaConDetalle = p.CuentaConDetalle,
                            IdAnexo = p.IdAnexo,
                            TipoDocumento =
                                !_objDocumentoBL.DocumentoEsInverso(p.TipoDocumento.Value) && p.TipoDocumento != 8
                                    ? (int)p.TipoDocumento
                                    : (int)p.TipoDocumentoRef,
                            NroDocumento =
                                !_objDocumentoBL.DocumentoEsInverso(p.TipoDocumento.Value) && p.TipoDocumento != 8
                                    ? p.NroDocumento
                                    : p.NroDocumentoRef,
                            NombreRazonSocial = p.NombreRazonSocial,
                            Doc = p.Doc,
                            DocFecha = p.DocFecha,
                            CodAnexo = p.CodAnexo,
                            DocTipo = p.DocTipo,
                            VentaDetraccion = p.VentaDetraccion,
                            VentaTasaDetraccion = p.TasaDetraccion
                        }).ToList();

                    gruposDiario = gruposDiario.Where(p => !string.IsNullOrEmpty(p.NombreCuenta)).ToList();
                    #endregion

                    #region Une tesorerias con diarios en una sola lista de entidades

                    gruposDiario =
                        gruposDiario.Where(
                            n =>
                                int.Parse(n.CuentaMayor.Substring(0, 2)) >= 10 &&
                                int.Parse(n.CuentaMayor.Substring(0, 2)) <= 99).ToList();

                    gruposTesoreria =
                        gruposTesoreria.Where(
                            n =>
                                int.Parse(n.CuentaMayor.Substring(0, 2)) >= 10 &&
                                int.Parse(n.CuentaMayor.Substring(0, 2)) <= 99).ToList();

                    var Grupos = gruposDiario.Concat(gruposTesoreria)
                        .ToList()
                        .OrderBy(p => p.CuentaMayor)
                        .ThenBy(x => x.CuentaImputable)
                        .ThenBy(o => o.Detalle)
                        .ThenBy(f => f.FechaDocumento)
                        .ToList();

                    Grupos.ForEach(
                        p =>
                            p.NombreCuentaMayor =
                                " " + cuentasMayores.FirstOrDefault(o => o.v_NroCuenta == p.CuentaMayor).v_NombreCuenta);
                    #endregion

                    #region Realiza el cálculo de los saldos
                    var listadoCuentasPorMostrar = Grupos.Select(p => p.CuentaImputable).Distinct().ToList();
                    foreach (var cuentaDiferenciaCambio in listadoCuentasPorMostrar)
                    {
                        var registrosPorCuenta = Grupos.Where(p => p.CuentaImputable.Trim() == cuentaDiferenciaCambio.Trim()).ToList();

                        if (registrosPorCuenta.Count > 0)
                        {
                            var detalle = 0;
                            if ((cuentasDetallesDiccionario.TryGetValue(cuentaDiferenciaCambio, out detalle) ? detalle : 0) == 1)
                            {
                                #region Operacion por anexo, tipo de documento y nro documento
                                //Las cuentas que tienen detalle se agrupan por Anexo, Documento y NroDOcumento
                                var registrosPorDetalleDocumento = registrosPorCuenta.GroupBy(g => new { g.IdAnexo, g.TipoDocumento, NroDocumento = !string.IsNullOrWhiteSpace(g.NroDocumento) ? g.NroDocumento.Trim() : string.Empty }).ToList();

                                //se recorren los valores agrupados, por cada grupo representa una fila en el asiento de diario.
                                foreach (var Reg in registrosPorDetalleDocumento)
                                {
                                    var registroProvicional = Reg.Where(p => p.NaturalezaCuenta == p.NaturalezaRegistro).ToList();
                                    var registroCancelacion = Reg.Where(p => p.NaturalezaCuenta != p.NaturalezaRegistro).ToList();

                                    if (registroProvicional.Count > 0 || registroCancelacion.Count > 0)
                                    {
                                        decimal saldoSoles, saldoDolares;

                                        var naturalezaCuentaProvicional = registroProvicional.Count > 0 ? registroProvicional.FirstOrDefault().NaturalezaCuenta : registroCancelacion.FirstOrDefault().NaturalezaCuenta == "D" ? "D" : "H";

                                        if (naturalezaCuentaProvicional == "D")
                                        {
                                            saldoSoles = registroProvicional.Count > 0 ? registroProvicional.Sum(p => p.ImporteSolesD.Value) - registroCancelacion.Sum(p => p.ImporteSolesH.Value) : 0 - registroCancelacion.Sum(p => p.ImporteSolesH.Value);
                                            saldoDolares = registroProvicional.Count > 0 ? registroProvicional.Sum(p => p.ImporteDolaresD.Value) - registroCancelacion.Sum(p => p.ImporteDolaresH.Value) : 0 - registroCancelacion.Sum(p => p.ImporteDolaresH.Value);
                                        }
                                        else
                                        {
                                            saldoSoles = registroProvicional.Count > 0 ? registroProvicional.Sum(p => p.ImporteSolesH.Value) - registroCancelacion.Sum(p => p.ImporteSolesD.Value) : 0 - registroCancelacion.Sum(p => p.ImporteSolesD.Value);
                                            saldoDolares = registroProvicional.Count > 0 ? registroProvicional.Sum(p => p.ImporteDolaresH.Value) - registroCancelacion.Sum(p => p.ImporteDolaresD.Value) : 0 - registroCancelacion.Sum(p => p.ImporteDolaresD.Value);
                                        }

                                        var resultadoXAgregar = registroProvicional.Count > 0 ? registroProvicional.FirstOrDefault() : registroCancelacion.FirstOrDefault();
                                        if (resultadoXAgregar != null)
                                        {
                                            resultadoXAgregar.VentaDetraccion = resultadoXAgregar.VentaDetraccion && !registroCancelacion.Any();
                                            resultadoXAgregar.ImporteSoles = saldoSoles;
                                            resultadoXAgregar.ImporteDolares = saldoDolares;
                                            reporteResultante.Add(resultadoXAgregar);
                                        }
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                #region Operacion de cuentas sin detalle

                                ReporteAnalisisCuentas ResultadoXAgregar = new Common.BE.Custom.ReporteAnalisisCuentas();

                                var RegistroProvicional = registrosPorCuenta.Where(p => p.NaturalezaCuenta == p.NaturalezaRegistro).ToList();
                                var RegistroCancelacion = registrosPorCuenta.Where(p => p.NaturalezaCuenta != p.NaturalezaRegistro).ToList();

                                if (RegistroProvicional.Count > 0 || RegistroCancelacion.Count > 0)
                                {
                                    decimal SaldoSoles, SaldoDolares;

                                    var NaturalezaCuentaProvicional = RegistroProvicional.Count > 0 ? RegistroProvicional.FirstOrDefault().NaturalezaCuenta : RegistroCancelacion.FirstOrDefault().NaturalezaCuenta == "D" ? "D" : "H";

                                    if (NaturalezaCuentaProvicional == "D")
                                    {
                                        SaldoSoles = RegistroProvicional.Count > 0 ? RegistroProvicional.Sum(p => p.ImporteSolesD.Value) - RegistroCancelacion.Sum(p => p.ImporteSolesH.Value) : 0 - RegistroCancelacion.Sum(p => p.ImporteSolesH.Value);
                                        SaldoDolares = RegistroProvicional.Count > 0 ? RegistroProvicional.Sum(p => p.ImporteDolaresD.Value) - RegistroCancelacion.Sum(p => p.ImporteDolaresH.Value) : 0 - RegistroCancelacion.Sum(p => p.ImporteDolaresH.Value);
                                    }
                                    else
                                    {
                                        SaldoSoles = RegistroProvicional.Count > 0 ? RegistroProvicional.Sum(p => p.ImporteSolesH.Value) - RegistroCancelacion.Sum(p => p.ImporteSolesD.Value) : 0 - RegistroCancelacion.Sum(p => p.ImporteSolesD.Value);
                                        SaldoDolares = RegistroProvicional.Count > 0 ? RegistroProvicional.Sum(p => p.ImporteDolaresH.Value) - RegistroCancelacion.Sum(p => p.ImporteDolaresD.Value) : 0 - RegistroCancelacion.Sum(p => p.ImporteDolaresD.Value);
                                    }

                                    ResultadoXAgregar = RegistroProvicional.Count > 0 ? RegistroProvicional.FirstOrDefault() : RegistroCancelacion.FirstOrDefault();
                                    ResultadoXAgregar.ImporteSoles = SaldoSoles;
                                    ResultadoXAgregar.ImporteDolares = SaldoDolares;
                                    reporteResultante.Add(ResultadoXAgregar);
                                }
                                #endregion
                            }
                        }
                    }
                    #endregion

                    #region Mostrar solo los que tienen obligación pendiente
                    if (mostrarSoloPendientes)
                        reporteResultante = reporteResultante.Where(p => (p.ImporteSoles ?? 0) != 0).ToList();
                    #endregion

                    reporteResultante = reporteResultante.OrderBy(p => p.CuentaMayor).ThenBy(x => x.CuentaImputable).ThenBy(o => o.NombreRazonSocial).ThenBy(f => f.FechaDocumento).ToList();

                    pobjOperationResult.Success = 1;

                    return reporteResultante;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AsientosContablesBL.ReporteAnalisisCuentas()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public List<ReporteAnalisisCuentas> ReporteAnalisisCuentasSinCierre(ref OperationResult pobjOperationResult, DateTime F_Ini, DateTime F_Fin, string nroCuenta, string periodo)
        {
            try
            {
                F_Ini = F_Ini.Date;
                F_Fin = DateTime.Parse(F_Fin.Day.ToString() + "/" + F_Fin.Month.ToString() + "/" + F_Fin.Year.ToString() + " 23:59");

                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    List<ReporteAnalisisCuentas> Grupos = new List<ReporteAnalisisCuentas>();
                    List<ReporteAnalisisCuentas> ReporteResultante = new List<ReporteAnalisisCuentas>();

                    #region Recopila las Cuentas Mayores
                    var CuentasMayores = (from n in dbContext.asientocontable
                                          where n.i_LongitudJerarquica == 2 && n.i_Eliminado == 0 && n.v_Periodo == periodo
                                          select n).ToList();

                    #endregion

                    #region Recopila Cuentas de Tesorería
                    var GruposTesoreria = (from n in dbContext.tesoreriadetalle

                                           join J1 in dbContext.asientocontable on new { c = n.v_NroCuenta.Trim(), p = periodo, eliminado = 0 } equals new { c = J1.v_NroCuenta.Trim(), p = J1.v_Periodo, eliminado = J1.i_Eliminado.Value } into J1_join
                                           from J1 in J1_join.DefaultIfEmpty()

                                           join J2 in dbContext.documento on n.i_IdTipoDocumento.Value equals J2.i_CodigoDocumento into J2_join
                                           from J2 in J2_join.DefaultIfEmpty()

                                           join J3 in dbContext.cliente on n.v_IdCliente equals J3.v_IdCliente into J3_join
                                           from J3 in J3_join.DefaultIfEmpty()

                                           join J4 in dbContext.tesoreria on n.v_IdTesoreria equals J4.v_IdTesoreria into J4_join
                                           from J4 in J4_join.DefaultIfEmpty()

                                           join J5 in dbContext.documento on n.i_IdTipoDocumentoRef.Value equals J5.i_CodigoDocumento into J5_join
                                           from J5 in J5_join.DefaultIfEmpty()

                                           join J6 in dbContext.documento on J4.i_IdTipoDocumento.Value equals J6.i_CodigoDocumento into J6_join
                                           from J6 in J6_join.DefaultIfEmpty()

                                           where n.i_Eliminado == 0 && n.tesoreria.t_FechaRegistro.Value >= F_Ini && n.tesoreria.t_FechaRegistro.Value <= F_Fin
                                                && J4.i_IdEstado == 1
                                           select new
                                           {
                                               CuentaMayor = n.v_NroCuenta.Substring(0, 2),
                                               CuentaImputable = n.v_NroCuenta,
                                               NombreCuenta = J1 != null ? J1.v_NombreCuenta : "",
                                               FechaDocumento = J4.t_FechaRegistro,
                                               Detalle = J3 != null ? J3.v_CodCliente + " " + n.v_IdCliente != "N002-CL000000000" ? (J3.v_ApePaterno + " " + J3.v_ApeMaterno + " " + J3.v_PrimerNombre + " " + J3.v_RazonSocial).Trim() : "PÚBLICO GENERAL" : "SIN DETALLE",
                                               DocumentoTransaccion = J6 != null ? J6.v_Siglas + " " + J4.v_Mes.Trim() + J4.v_Correlativo.Trim() : "",
                                               Documento = J2.v_Siglas + " " + n.v_NroDocumento,
                                               DocumentoRef = J5 != null ? J5.v_Siglas + " " + n.v_NroDocumentoRef : "",
                                               Analisis = n.v_Analisis ?? (!string.IsNullOrEmpty(J4.v_Glosa) ? J4.v_Glosa : ""),
                                               IdMoneda = J4.i_IdMoneda,
                                               SiglaMoneda = J4.i_IdMoneda == 1 ? "S" : "D",
                                               Importe = n.d_Importe,
                                               ImporteCambio = n.d_Cambio,
                                               Key = J3.v_IdCliente + n.v_NroCuenta + J2.v_Siglas + n.v_NroDocumento,
                                               NaturalezaCuenta = J1 != null ? J1.i_Naturaleza : 0,
                                               IdMonedaCuenta = J1 != null ? J1.i_IdMoneda : 0,
                                               NaturalezaRegistro = n.v_Naturaleza,
                                               CuentaConDetalle = J1.i_Detalle == 1 ? true : false,
                                               TipoCambio = J4.d_TipoCambio,
                                               IdAnexo = J3.v_IdCliente,
                                               Detalle_Ruc = J3 != null ? J3.v_NroDocIdentificacion : "",
                                               TipoDocumento = n.i_IdTipoDocumento,
                                               TipoDocumentoRef = n.i_IdTipoDocumentoRef,
                                               NroDocumento = n.v_NroDocumento,
                                               NroDocumentoRef = n.v_NroDocumentoRef,
                                               NombreRazonSocial = n.v_IdCliente != "N002-CL000000000" ? (J3.v_ApePaterno + " " + J3.v_ApeMaterno + " " + J3.v_PrimerNombre + " " + J3.v_RazonSocial).Trim() : "PÚBLICO GENERAL"
                                           }
                                              ).ToList().Select(p => new ReporteAnalisisCuentas
                                              {
                                                  CuentaMayor = p.CuentaMayor,
                                                  CuentaImputable = p.CuentaImputable,
                                                  NombreCuenta = p.NombreCuenta,
                                                  Detalle = p.CuentaConDetalle ? p.Detalle_Ruc + ": " + p.Detalle : "",
                                                  Documento = p.CuentaConDetalle ? (p.FechaDocumento.Value.ToShortDateString() + " \t" + p.Documento).Trim() + " \t" + p.Analisis : "",
                                                  DocumentoRef = !string.IsNullOrEmpty(p.DocumentoRef) ? (p.DocumentoRef).Trim() : "",
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
                                                  IdAnexo = p.IdAnexo,
                                                  //TipoDocumento = p.TipoDocumento != 7 && p.TipoDocumento != 8 ? (int)p.TipoDocumento : (int)p.TipoDocumentoRef,
                                                  TipoDocumento = !_objDocumentoBL.DocumentoEsInverso(p.TipoDocumento.Value) && p.TipoDocumento != 8 ? (int)p.TipoDocumento : (int)p.TipoDocumentoRef,
                                                  // NroDocumento = p.TipoDocumento != 7 && p.TipoDocumento != 8 ? p.NroDocumento : p.NroDocumentoRef,
                                                  NroDocumento = !_objDocumentoBL.DocumentoEsInverso(p.TipoDocumento.Value) && p.TipoDocumento != 8 ? p.NroDocumento : p.NroDocumentoRef,
                                                  NombreRazonSocial = p.NombreRazonSocial
                                              }
                                              ).ToList();

                    GruposTesoreria = GruposTesoreria.Where(p => !string.IsNullOrEmpty(p.NombreCuenta)).ToList();
                    #endregion

                    #region Recopila Cuentas de Diario
                    var GruposDiario = (from n in dbContext.diariodetalle

                                        join J1 in dbContext.asientocontable on new { c = n.v_NroCuenta, eliminado = 0, p = periodo } equals new { c = J1.v_NroCuenta, eliminado = J1.i_Eliminado.Value, p = J1.v_Periodo } into J1_join
                                        from J1 in J1_join.DefaultIfEmpty()

                                        join J2 in dbContext.documento on new { doc = n.i_IdTipoDocumento.Value, eliminado = 0 } equals new { doc = J2.i_CodigoDocumento, eliminado = J2.i_Eliminado.Value } into J2_join
                                        from J2 in J2_join.DefaultIfEmpty()

                                        join J3 in dbContext.cliente on n.v_IdCliente equals J3.v_IdCliente into J3_join
                                        from J3 in J3_join.DefaultIfEmpty()

                                        join J4 in dbContext.diario on n.v_IdDiario equals J4.v_IdDiario into J4_join
                                        from J4 in J4_join.DefaultIfEmpty()

                                        join J5 in dbContext.documento on n.i_IdTipoDocumentoRef.Value equals J5.i_CodigoDocumento into J5_join
                                        from J5 in J5_join.DefaultIfEmpty()

                                        join J6 in dbContext.documento on J4.i_IdTipoDocumento.Value equals J6.i_CodigoDocumento into J6_join
                                        from J6 in J6_join.DefaultIfEmpty()

                                        where n.i_Eliminado == 0 && n.diario.t_Fecha.Value >= F_Ini.Date && n.diario.t_Fecha.Value <= F_Fin
                                       && J4.i_IdTipoComprobante != 5
                                        select new
                                        {
                                            CuentaMayor = n.v_NroCuenta.Substring(0, 2),
                                            CuentaImputable = n.v_NroCuenta,
                                            NombreCuenta = J1 != null ? J1.v_NombreCuenta : "",
                                            FechaDocumento = J4.t_Fecha,
                                            Detalle = J3 != null ? J3.v_CodCliente + " " + n.v_IdCliente != "N002-CL000000000" ? (J3.v_ApePaterno + " " + J3.v_ApeMaterno + " " + J3.v_PrimerNombre + " " + J3.v_RazonSocial).Trim() : "PÚBLICO GENERAL" : "SIN DETALLE",
                                            DocumentoTransaccion = J4 != null ? J4.i_IdTipoComprobante.Value : 0,
                                            Documento = J2.v_Siglas + " " + n.v_NroDocumento,
                                            DocumentoRef = J5 != null && n.v_NroDocumentoRef != null ? J5.v_Siglas + " " + n.v_NroDocumentoRef : "",
                                            Analisis = n.v_Analisis ?? (!string.IsNullOrEmpty(J4.v_Glosa) ? J4.v_Glosa : ""),
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
                                            Detalle_Ruc = J3 != null ? J3.v_NroDocIdentificacion : "",
                                            TipoDocumento = n.i_IdTipoDocumento,
                                            TipoDocumentoRef = n.i_IdTipoDocumentoRef,
                                            NroDocumento = n.v_NroDocumento,
                                            NroDocumentoRef = n.v_NroDocumentoRef,
                                            NombreRazonSocial = n.v_IdCliente != "N002-CL000000000" ? (J3.v_ApePaterno + " " + J3.v_ApeMaterno + " " + J3.v_PrimerNombre + " " + J3.v_RazonSocial).Trim() : "PÚBLICO GENERAL"
                                        }
                                        ).ToList().Select(p => new ReporteAnalisisCuentas
                                        {
                                            CuentaMayor = p.CuentaMayor,
                                            CuentaImputable = p.CuentaImputable,
                                            NombreCuenta = p.NombreCuenta,
                                            Detalle = p.CuentaConDetalle ? p.Detalle_Ruc + ": " + p.Detalle : "",
                                            Documento = p.CuentaConDetalle ? (p.FechaDocumento.Value.ToShortDateString() + " \t" + p.Documento).Trim() + " \t" + p.Analisis : "",
                                            DocumentoRef = !string.IsNullOrEmpty(p.DocumentoRef) ? (p.DocumentoRef).Trim() : "",
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
                                            IdAnexo = p.IdAnexo,
                                            //TipoDocumento = p.TipoDocumento != 7 && p.TipoDocumento != 8 ? (int)p.TipoDocumento : (int)p.TipoDocumentoRef,
                                            TipoDocumento = !_objDocumentoBL.DocumentoEsInverso(p.TipoDocumento.Value) && p.TipoDocumento != 8 ? (int)p.TipoDocumento : (int)p.TipoDocumentoRef,
                                            NroDocumento = !_objDocumentoBL.DocumentoEsInverso(p.TipoDocumento.Value) && p.TipoDocumento != 8 ? p.NroDocumento : p.NroDocumentoRef,
                                            NombreRazonSocial = p.NombreRazonSocial
                                        }).ToList();

                    GruposDiario = GruposDiario.Where(p => !string.IsNullOrEmpty(p.NombreCuenta)).ToList();
                    #endregion

                    #region Filtro por cuenta si es requerido
                    if (!string.IsNullOrEmpty(nroCuenta))
                    {
                        GruposDiario = GruposDiario.Where(p => p.CuentaImputable.StartsWith(nroCuenta)).ToList();
                        GruposTesoreria = GruposTesoreria.Where(p => p.CuentaImputable.StartsWith(nroCuenta)).ToList();
                    }
                    #endregion

                    #region Une tesorerias con diarios en una sola lista de entidades
                    GruposDiario = GruposDiario.Where(n => int.Parse(n.CuentaMayor.Substring(0, 2)) >= 10 && int.Parse(n.CuentaMayor.Substring(0, 2)) <= 70).ToList();

                    GruposTesoreria = GruposTesoreria.Where(n => int.Parse(n.CuentaMayor.Substring(0, 2)) >= 10 && int.Parse(n.CuentaMayor.Substring(0, 2)) <= 70).ToList();

                    Grupos = GruposDiario.Concat(GruposTesoreria).ToList();

                    Grupos.ForEach(p => p.NombreCuentaMayor = " " + CuentasMayores.FirstOrDefault(o => o.v_NroCuenta == p.CuentaMayor).v_NombreCuenta);
                    #endregion

                    #region Realiza el cálculo de los saldos
                    var ListadoCuentasPorMostrar = Grupos.Select(p => p.CuentaImputable).Distinct().ToList();
                    foreach (string CuentaDiferenciaCambio in ListadoCuentasPorMostrar)
                    {
                        var RegistrosPorCuenta = Grupos.Where(p => p.CuentaImputable.Trim() == CuentaDiferenciaCambio.Trim()).ToList();

                        if (RegistrosPorCuenta.Count > 0)
                        {
                            if (Utils.Windows.CuentaRequiereDetalle(CuentaDiferenciaCambio))
                            {
                                #region Operacion por anexo, tipo de documento y nro documento
                                //Las cuentas que tienen detalle se agrupan por Anexo, Documento y NroDOcumento
                                var RegistrosPorDetalleDocumento = RegistrosPorCuenta.GroupBy(g => new { g.IdAnexo, g.TipoDocumento, g.NroDocumento }).ToList();

                                //se recorren los valores agrupados, por cada grupo representa una fila en el asiento de diario.
                                foreach (var Reg in RegistrosPorDetalleDocumento)
                                {
                                    ReporteAnalisisCuentas ResultadoXAgregar = new Common.BE.Custom.ReporteAnalisisCuentas();

                                    var RegistroProvicional = Reg.Where(p => p.NaturalezaCuenta == p.NaturalezaRegistro).ToList();
                                    var RegistroCancelacion = Reg.Where(p => p.NaturalezaCuenta != p.NaturalezaRegistro).ToList();

                                    if (RegistroProvicional.Count > 0 || RegistroCancelacion.Count > 0)
                                    {
                                        decimal SaldoSoles, SaldoDolares;

                                        var NaturalezaCuentaProvicional = RegistroProvicional.Count > 0 ? RegistroProvicional.FirstOrDefault().NaturalezaCuenta : RegistroCancelacion.FirstOrDefault().NaturalezaCuenta == "D" ? "D" : "H";

                                        if (NaturalezaCuentaProvicional == "D")
                                        {
                                            SaldoSoles = RegistroProvicional.Count > 0 ? RegistroProvicional.Sum(p => p.ImporteSolesD.Value) - RegistroCancelacion.Sum(p => p.ImporteSolesH.Value) : 0 - RegistroCancelacion.Sum(p => p.ImporteSolesH.Value);
                                            SaldoDolares = RegistroProvicional.Count > 0 ? RegistroProvicional.Sum(p => p.ImporteDolaresD.Value) - RegistroCancelacion.Sum(p => p.ImporteDolaresH.Value) : 0 - RegistroCancelacion.Sum(p => p.ImporteDolaresH.Value);
                                        }
                                        else
                                        {
                                            SaldoSoles = RegistroProvicional.Count > 0 ? RegistroProvicional.Sum(p => p.ImporteSolesH.Value) - RegistroCancelacion.Sum(p => p.ImporteSolesD.Value) : 0 - RegistroCancelacion.Sum(p => p.ImporteSolesD.Value);
                                            SaldoDolares = RegistroProvicional.Count > 0 ? RegistroProvicional.Sum(p => p.ImporteDolaresH.Value) - RegistroCancelacion.Sum(p => p.ImporteDolaresD.Value) : 0 - RegistroCancelacion.Sum(p => p.ImporteDolaresD.Value);
                                        }

                                        ResultadoXAgregar = RegistroProvicional.Count > 0 ? RegistroProvicional.FirstOrDefault() : RegistroCancelacion.FirstOrDefault();
                                        ResultadoXAgregar.ImporteSoles = SaldoSoles;
                                        ResultadoXAgregar.ImporteDolares = SaldoDolares;
                                        ReporteResultante.Add(ResultadoXAgregar);
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                #region Operacion de cuentas sin detalle

                                ReporteAnalisisCuentas ResultadoXAgregar = new Common.BE.Custom.ReporteAnalisisCuentas();

                                var RegistroProvicional = RegistrosPorCuenta.Where(p => p.NaturalezaCuenta == p.NaturalezaRegistro).ToList();
                                var RegistroCancelacion = RegistrosPorCuenta.Where(p => p.NaturalezaCuenta != p.NaturalezaRegistro).ToList();

                                if (RegistroProvicional.Count > 0 || RegistroCancelacion.Count > 0)
                                {
                                    decimal SaldoSoles, SaldoDolares;

                                    var NaturalezaCuentaProvicional = RegistroProvicional.Count > 0 ? RegistroProvicional.FirstOrDefault().NaturalezaCuenta : RegistroCancelacion.FirstOrDefault().NaturalezaCuenta == "D" ? "D" : "H";

                                    if (NaturalezaCuentaProvicional == "D")
                                    {
                                        SaldoSoles = RegistroProvicional.Count > 0 ? RegistroProvicional.Sum(p => p.ImporteSolesD.Value) - RegistroCancelacion.Sum(p => p.ImporteSolesH.Value) : 0 - RegistroCancelacion.Sum(p => p.ImporteSolesH.Value);
                                        SaldoDolares = RegistroProvicional.Count > 0 ? RegistroProvicional.Sum(p => p.ImporteDolaresD.Value) - RegistroCancelacion.Sum(p => p.ImporteDolaresH.Value) : 0 - RegistroCancelacion.Sum(p => p.ImporteDolaresH.Value);
                                    }
                                    else
                                    {
                                        SaldoSoles = RegistroProvicional.Count > 0 ? RegistroProvicional.Sum(p => p.ImporteSolesH.Value) - RegistroCancelacion.Sum(p => p.ImporteSolesD.Value) : 0 - RegistroCancelacion.Sum(p => p.ImporteSolesD.Value);
                                        SaldoDolares = RegistroProvicional.Count > 0 ? RegistroProvicional.Sum(p => p.ImporteDolaresH.Value) - RegistroCancelacion.Sum(p => p.ImporteDolaresD.Value) : 0 - RegistroCancelacion.Sum(p => p.ImporteDolaresD.Value);
                                    }

                                    ResultadoXAgregar = RegistroProvicional.Count > 0 ? RegistroProvicional.FirstOrDefault() : RegistroCancelacion.FirstOrDefault();
                                    ResultadoXAgregar.ImporteSoles = SaldoSoles;
                                    ResultadoXAgregar.ImporteDolares = SaldoDolares;
                                    ReporteResultante.Add(ResultadoXAgregar);
                                }
                                #endregion
                            }
                        }
                    }
                    #endregion

                    #region Mostrar solo los que tienen obligación pendiente
                    ReporteResultante = ReporteResultante.Where(p => p.ImporteSoles.Value != 0).ToList();
                    #endregion

                    ReporteResultante = ReporteResultante.OrderBy(p => p.CuentaMayor).ThenBy(x => x.CuentaImputable).ThenBy(o => o.NombreRazonSocial).ThenBy(f => f.FechaDocumento).ToList();

                    pobjOperationResult.Success = 1;

                    ReporteResultante = ReporteResultante.OrderBy(p => p.CuentaMayor).ThenBy(x => x.CuentaImputable).ThenBy(o => o.Detalle).ThenBy(f => f.NroDocumento).ThenBy(j => j.FechaDocumento).ToList();
                    return ReporteResultante;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AsientosContablesBL.ReporteAnalisisCuentas()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public List<ReporteAnalisisCuentasCteAnalitico> ReporteAnalisisCuentasCteAnalitico(
            ref OperationResult pobjOperationResult, DateTime F_Ini, DateTime F_Fin, string nroCuentaIni,
            string nroCuentaFin, bool cuentaConDetalle, string idAnexo, TipoReporteAnalisis tipoReporteAnalisis,
            int idTipoDocumento = 0, string nroDocumento = null, string nroCuentaMayor = null, bool mostrarSoloSospechosos = false)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var periodo = Globals.ClientSession.i_Periodo.Value.ToString();
                    nroCuentaIni = string.IsNullOrWhiteSpace(nroCuentaIni) ? null : nroCuentaIni.Trim();
                    nroCuentaFin = string.IsNullOrWhiteSpace(nroCuentaFin) ? null : nroCuentaFin.Trim();
                    nroDocumento = string.IsNullOrWhiteSpace(nroDocumento) ? null : nroDocumento.Trim();

                    F_Ini = F_Ini.Date;
                    F_Fin = DateTime.Parse(F_Fin.Day + "/" + F_Fin.Month + "/" + F_Fin.Year + " 23:59");
                    List<ReporteAnalisisCuentasCteAnalitico> gruposTesoreria;
                    List<ReporteAnalisisCuentasCteAnalitico> gruposDiario;

                    #region Recopila Cuentas de Tesorería
                    try
                    {
                        dbContext.tesoreriadetalle.MergeOption = MergeOption.AppendOnly;

                        #region Consulta Base
                        List<tesoreriadetalle> tesoreriaDetalle;
                        var rangoCtas = Utils.Windows.RangoDeCuentas(nroCuentaIni, nroCuentaFin).ToList();
                        var queryToExecute = ObtenerConsultaAnalitico(2, F_Ini, F_Fin, idAnexo, rangoCtas, idTipoDocumento,
                            nroDocumento, nroCuentaMayor);

                        switch (Globals.TipoMotor)
                        {
                            case TipoMotorBD.PostgreSQL:
                                using (var cnx = new PgSqlConnection(Globals.CadenaConexion))
                                {
                                    tesoreriaDetalle = cnx.Query<tesoreriadetalle>(queryToExecute).ToList();
                                }
                                break;

                            default:
                                using (var cnx = new SqlConnection(Globals.CadenaConexion))
                                {
                                    tesoreriaDetalle = cnx.Query<tesoreriadetalle>(queryToExecute).ToList();
                                }
                                break;
                        }

                        #region Antigua consulta en linq
                        //tesoreriaDetalle = (from n in dbContext.tesoreriadetalle
                        //                    join J1 in dbContext.tesoreria on n.v_IdTesoreria equals J1.v_IdTesoreria into J1_join
                        //                    from J1 in J1_join.DefaultIfEmpty()
                        //                    where n.i_Eliminado == 0 && J1.t_FechaRegistro.Value >= F_Ini &&
                        //                          J1.t_FechaRegistro.Value <= F_Fin &&
                        //                          (idAnexo == "" || n.v_IdCliente.Equals(idAnexo))
                        //                          &&
                        //                          (rangoCtas.Count == 0 || rangoCtas.Contains(n.v_NroCuenta))
                        //                          &&
                        //                          ((idTipoDocumento < 1 && nroDocumento == null) ||
                        //                           ((n.i_IdTipoDocumento ?? -1) == idTipoDocumento && n.v_NroDocumento.Equals(nroDocumento)))
                        //                    select n).ToList();
                        #endregion
                        #endregion

                        #region Consulta principal

                        var gruposTesoreriaSource = (from n in tesoreriaDetalle.Where(p => p.i_Eliminado == 0)
                                                     join J1 in dbContext.asientocontable on new { cuenta = n.v_NroCuenta.Trim(), eliminado = 0, p = periodo }
                                                         equals new { cuenta = J1.v_NroCuenta.Trim(), eliminado = J1.i_Eliminado ?? 0, p = J1.v_Periodo } into J1_join
                                                     from J1 in J1_join.DefaultIfEmpty()
                                                     join J2 in dbContext.documento on n.i_IdTipoDocumento ?? 0
                                                         equals J2.i_CodigoDocumento into J2_join
                                                     from J2 in J2_join.DefaultIfEmpty()
                                                     join J3 in dbContext.cliente on n.v_IdCliente
                                                         equals J3.v_IdCliente into J3_join
                                                     from J3 in J3_join.DefaultIfEmpty()
                                                     join J4 in dbContext.tesoreria on n.v_IdTesoreria
                                                         equals J4.v_IdTesoreria into J4_join
                                                     from J4 in J4_join.DefaultIfEmpty()
                                                     join J5 in dbContext.documento on n.i_IdTipoDocumentoRef ?? 0
                                                         equals J5.i_CodigoDocumento into J5_join
                                                     from J5 in J5_join.DefaultIfEmpty()
                                                     join J6 in dbContext.documento on J4 != null ? J4.i_IdTipoDocumento ?? 0 : 0
                                                         equals J6.i_CodigoDocumento into J6_join
                                                     from J6 in J6_join.DefaultIfEmpty()


                                                     where n != null && n.i_Eliminado == 0 && (J4 != null && J4.i_IdEstado == 1)
                                                     select new
                                                     {
                                                         CuentaMayor = n.v_NroCuenta.Substring(0, 2),
                                                         CuentaImputable = n.v_NroCuenta,
                                                         NombreCuenta = J1 != null ? J1.v_NombreCuenta : "",
                                                         FechaDocumento = J4 != null ? J4.t_FechaRegistro : null,
                                                         Detalle =
                                                             J3 != null
                                                                 ? J3.v_CodCliente.Trim() + " " + n.v_IdCliente != "N002-CL000000000"
                                                                     ? (J3.v_ApePaterno.Trim() + " " + J3.v_ApeMaterno.Trim() + " " +
                                                                        J3.v_PrimerNombre.Trim() + " " + J3.v_RazonSocial.Trim()).Trim()
                                                                     : "PÚBLICO GENERAL"
                                                                 : "SIN DETALLE",
                                                         DocumentoTransaccion =
                                                             J6 != null
                                                                 ? J6.v_Siglas.Trim() + " " + J4.v_Mes.Trim() + J4.v_Correlativo.Trim()
                                                                 : "",
                                                         Documento =
                                                             J2 != null
                                                                 ? J2.v_Siglas.Trim() + " " +
                                                                   (n.v_NroDocumento != null ? n.v_NroDocumento.Trim() : "")
                                                                 : "",
                                                         DocumentoRef =
                                                             J5 != null && n.v_NroDocumentoRef != null
                                                                 ? J5.v_Siglas.Trim() + " " + n.v_NroDocumentoRef.Trim()
                                                                 : "",
                                                         // Analisis = n.v_Analisis != null && (J4.v_Glosa != null) ? J4.v_Glosa.Trim() : "",
                                                         Analisis = (n.v_Analisis == null || n.v_Analisis == string.Empty) ? J4.v_Glosa : n.v_Analisis,
                                                         IdMoneda = J4 != null ? J4.i_IdMoneda : 1,
                                                         IdMonedaCuenta = J1 != null ? J1.i_IdMoneda : 0,
                                                         SiglaMoneda = J4 != null ? J4.i_IdMoneda == 1 ? "S" : "D" : "",
                                                         Importe = n.d_Importe ?? 0,
                                                         ImporteCambio = n.d_Cambio ?? 0,
                                                         Key =
                                                             J3 != null && J2 != null
                                                                 ? J3.v_IdCliente + n.v_NroCuenta + (n.i_IdTipoDocumento != 7 && n.i_IdTipoDocumento != 8 ? J2.v_Siglas : J5 != null ? J5.v_Siglas : "") +
                                                                 (n.i_IdTipoDocumento != 7 && n.i_IdTipoDocumento != 8 ?
                                                                   (n.v_NroDocumento != null ? n.v_NroDocumento.Trim() : "") : (n.v_NroDocumentoRef != null ? n.v_NroDocumentoRef.Trim() : ""))
                                                                 : "",
                                                         NaturalezaCuenta = J1 != null ? J1.i_Naturaleza : 0,
                                                         NaturalezaRegistro = n.v_Naturaleza,
                                                         CuentaConDetalle = J1 != null && J1.i_Detalle == 1,
                                                         TipoCambio = J4 != null ? J4.d_TipoCambio : 0,
                                                         IdAnexo = J3 != null ? J3.v_IdCliente : "",
                                                         Detalle_Ruc = J3 != null ? J3.v_NroDocIdentificacion : "",
                                                         IdTipoDocumento = n.i_IdTipoDocumento ?? -1,
                                                         ID = n.v_IdTesoreria,
                                                         IDAsientoDetalle = n.v_IdTesoreriaDetalle,
                                                         EntidadFinanciera = J1 != null ? J1.i_EntFinanciera : 0,
                                                         Detalle_TipoIdentificacion = J3 != null ? J3.i_IdTipoIdentificacion : -1,
                                                         TipoDocumento = n.i_IdTipoDocumento ?? -1,
                                                         TipoDocumentoRef = n.i_IdTipoDocumentoRef ?? -1,
                                                         DocumentoRaw = n.v_NroDocumento,
                                                         DocumentoRefRaw = n.v_NroDocumentoRef,
                                                         //IdCentroCosto = n.i_IdCentroCostos,
                                                         IdCentroCosto = n.i_IdCentroCostos == null ? "" : n.i_IdCentroCostos.Trim(),
                                                         i_IdTipoComprobante = 1000,
                                                         NroCuentaEntidadFinanciera = J1 != null ? J1.v_Area : "-1",
                                                         NombreCliente = J3 != null ? (J3.v_ApePaterno + " " + J3.v_ApeMaterno + " " + J3.v_PrimerNombre + " " + J3.v_SegundoNombre + " " + J3.v_RazonSocial).Trim() : "",
                                                         NroRegistro = J4.v_Mes.Trim() + J4.v_Correlativo.Trim(),
                                                         TipoDocumentoCabecera = J4.i_IdTipoDocumento ?? -1,
                                                         FlagCliente = J3 != null ? J3.v_FlagPantalla : "",
                                                         FechaDetalle = n != null ? n.t_Fecha : J4.t_FechaRegistro,
                                                         CodigoCliente = J3 != null ? J3.v_CodCliente : "",
                                                         i_IdPatrimonioNeto = n.i_IdPatrimonioNeto,
                                                         CuentaDetraccion = J3 != null ? J3.v_NroCuentaDetraccion : "",
                                                         TipoOperacionDetraccion = -1,
                                                         i_CodigoDetraccion = -1,
                                                         FechaEmision = DateTime.Now ,
                                                        FechaVencimiento=DateTime.Now ,
                                                     });

                        #endregion

                        #region Filtra solo cuentas con detalle si se requiere
                        if (cuentaConDetalle)
                            gruposTesoreriaSource = gruposTesoreriaSource.Where(p => p.CuentaConDetalle);
                        #endregion

                        #region Consulta secundaria

                        gruposTesoreria = gruposTesoreriaSource.AsQueryable()
                            .Select(p => new ReporteAnalisisCuentasCteAnalitico
                            {
                                IdTipoDocumentoProvicion = p.TipoDocumentoCabecera,
                                CuentaMayor = p.CuentaMayor,
                                CuentaImputable = p.CuentaImputable,
                                NombreCuenta = "Cuenta: " + p.CuentaImputable + " " + p.NombreCuenta,
                                Detalle = p.Detalle_Ruc + ": " + p.Detalle,
                                Documento =
                                    p.SiglaMoneda + " \t" + p.DocumentoTransaccion + " \t" +
                                    (p.FechaDocumento.Value.ToShortDateString() + " \t" + p.Documento).Trim(),

                                FechaDocumento = p.FechaDocumento,
                                DocumentoKey = p.Key,
                                IdMoneda = (int)p.IdMoneda,
                                IdMonedaCuenta = (int)p.IdMonedaCuenta,
                                NaturalezaCuenta = p.NaturalezaCuenta == 1 ? "D" : "H",
                                NaturalezaRegistro = p.NaturalezaRegistro,
                                ImporteSolesD =
                                    p.NaturalezaRegistro == "D" ? p.IdMoneda == 1 ? p.Importe : p.ImporteCambio : 0,
                                ImporteSolesH =
                                    p.NaturalezaRegistro == "H" ? p.IdMoneda == 1 ? p.Importe : p.ImporteCambio : 0,
                                ImporteDolaresD =
                                    p.NaturalezaRegistro == "D" ? p.IdMoneda == 1 ? p.ImporteCambio : p.Importe : 0,
                                ImporteDolaresH =
                                    p.NaturalezaRegistro == "H" ? p.IdMoneda == 1 ? p.ImporteCambio : p.Importe : 0,
                                CuentaConDetalle = p.CuentaConDetalle,
                                TipoCambio = p.TipoCambio ?? 0,
                                IdAnexo = p.IdAnexo,
                                NroDocumento = p.TipoDocumento != 7 && p.TipoDocumento != 8 ? p.Documento != null ? (p.Documento).Trim() : "" : !string.IsNullOrEmpty(p.DocumentoRef)
                                        ? (p.DocumentoRef).Trim()
                                        : "",
                                DocumentoRef = p.TipoDocumento != 7 && p.TipoDocumento != 8 ? !string.IsNullOrEmpty(p.DocumentoRef) ? (p.DocumentoRef).Trim() : "" : p.Documento != null ? (p.Documento).Trim() : "",
                                DocumentoProvicion = p.DocumentoTransaccion,
                                IdDocumentoProvicion = p.ID,
                                EntidadFinanceriaCuenta = p.EntidadFinanciera != null ? p.EntidadFinanciera.ToString() : "-1",
                                Analisis = p.Analisis,
                                Detalle_TipoIdentificacion = p.Detalle_TipoIdentificacion != null ? p.Detalle_TipoIdentificacion.ToString() : "-1",
                                Detalle_Ruc = p.Detalle_Ruc,
                                EsAjusteDiferenciaCambio = false,
                                TipoDocumento = p.TipoDocumento,
                                TipoDocumentoRef = p.TipoDocumentoRef,
                                DocumentoRaw = p.DocumentoRaw,
                                DocumentoRefRaw = p.DocumentoRefRaw,
                                IdCentroCostos = p.IdCentroCosto,
                                Mes = p.FechaDocumento != null ? p.FechaDocumento.Value.Month.ToString() : "01",
                                CentroCosto = p.IdCentroCosto,
                                i_IdTipoComprobante = p.i_IdTipoComprobante,
                                NroCuentaEntidadFinanciera = p.NroCuentaEntidadFinanciera,
                                NombreCliente = p.NombreCliente,
                                NroRegistro = p.TipoDocumentoCabecera.ToString("000") + p.NroRegistro,
                                FlagCliente = p.FlagCliente,
                                FechaDetalle = p.FechaDetalle ?? DateTime.Now,
                                CodigoCliente = p.CodigoCliente,
                                i_IdPatrimonioNeto = p.i_IdPatrimonioNeto == null ? 0 : int.Parse(p.i_IdPatrimonioNeto),
                                IdDocumentoProvicionDetalle = p.IDAsientoDetalle,
                                TasaDetraccionVenta = 0,
                                CuentaDetraccion = p.CuentaDetraccion,
                                TipoOperacionDetraccion = p.TipoOperacionDetraccion,
                                i_CodigoDetraccion = p.i_CodigoDetraccion,
                                FechaEmision =p.FechaEmision ,
                                FechaVencimieto =p.FechaVencimiento ,
                            }
                            ).ToList();
                        #endregion
                    }
                    catch (AggregateException e)
                    {
                        pobjOperationResult.Success = 0;
                        pobjOperationResult.ErrorMessage = e.Message;
                        StringBuilder exeptions = new StringBuilder();
                        foreach (var ex in e.InnerExceptions)
                        {
                            exeptions.AppendLine(ex.Message);
                            if (ex is IndexOutOfRangeException)
                                pobjOperationResult.AdditionalInformation = "La data esta dañada.";
                        }
                        pobjOperationResult.ExceptionMessage = exeptions.ToString();
                        return null;
                    }
                    #endregion

                    #region Recopila Cuentas de Diario

                    try
                    {
                        dbContext.diariodetalle.MergeOption = MergeOption.AppendOnly;
                        #region Consulta Base

                        List<diariodetalle> diarioDetalle;
                        var rangoCtas = Utils.Windows.RangoDeCuentas(nroCuentaIni, nroCuentaFin).ToList();
                        var queryToExecute = ObtenerConsultaAnalitico(1, F_Ini, F_Fin, idAnexo, rangoCtas, idTipoDocumento,
                            nroDocumento, nroCuentaMayor);

                        switch (Globals.TipoMotor)
                        {
                            case TipoMotorBD.PostgreSQL:
                                using (var cnx = new PgSqlConnection(Globals.CadenaConexion))
                                {
                                    diarioDetalle = cnx.Query<diariodetalle>(queryToExecute).ToList();
                                }
                                break;

                            default:
                                using (var cnx = new SqlConnection(Globals.CadenaConexion))
                                {
                                    diarioDetalle = cnx.Query<diariodetalle>(queryToExecute).ToList();
                                }
                                break;
                        }

                        #endregion

                        #region Consulta principal

                        var gruposDiarioSource = (from n in diarioDetalle.Where(p => p.i_Eliminado == 0)
                                                  join J1 in dbContext.asientocontable on new { cuenta = n.v_NroCuenta.Trim(), eliminado = 0, p = periodo }
                                                         equals new { cuenta = J1.v_NroCuenta.Trim(), eliminado = J1.i_Eliminado ?? 0, p = J1.v_Periodo } into J1_join
                                                  from J1 in J1_join.DefaultIfEmpty()
                                                  join J2 in dbContext.documento on n.i_IdTipoDocumento ?? 0 equals
                                                      J2.i_CodigoDocumento into J2_join
                                                  from J2 in J2_join.DefaultIfEmpty()
                                                  join J3 in dbContext.cliente on n.v_IdCliente equals J3.v_IdCliente into
                                                      J3_join
                                                  from J3 in J3_join.DefaultIfEmpty()
                                                  join J4 in dbContext.diario on n.v_IdDiario equals J4.v_IdDiario into J4_join
                                                  from J4 in J4_join.DefaultIfEmpty()
                                                  join J5 in dbContext.documento on n.i_IdTipoDocumentoRef ?? 0 equals
                                                      J5.i_CodigoDocumento into J5_join
                                                  from J5 in J5_join.DefaultIfEmpty()
                                                  join J6 in dbContext.documento on J4 != null ? J4.i_IdTipoDocumento ?? 0 : 0
                                                      equals J6.i_CodigoDocumento into J6_join
                                                  from J6 in J6_join.DefaultIfEmpty()
                                                  where n.i_Eliminado == 0
                                                  select new
                                                  {
                                                      CuentaMayor = n.v_NroCuenta.Substring(0, 2),
                                                      CuentaImputable = J1 != null ? n.v_NroCuenta : "*Cta Eliminada*",
                                                      NombreCuenta = J1 != null ? J1.v_NombreCuenta : "",
                                                      FechaDocumento = J4 != null ? J4.t_Fecha : null,
                                                      Detalle =
                                                          J3 != null
                                                              ? J3.v_CodCliente.Trim() + " " + n.v_IdCliente != "N002-CL000000000"
                                                                  ? (J3.v_ApePaterno + " " + J3.v_ApeMaterno + " " + J3.v_PrimerNombre + " " +
                                                                     J3.v_RazonSocial).Trim()
                                                                  : "PÚBLICO GENERAL"
                                                              : "SIN DETALLE",
                                                      DocumentoTransaccion =
                                                          J6 != null
                                                              ? J6.v_Siglas.Trim() + " " + J4.v_Mes.Trim() + J4.v_Correlativo.Trim()
                                                              : "",
                                                      Documento =
                                                          J2 != null
                                                              ? J2.v_Siglas.Trim() + " " +
                                                                (n.v_NroDocumento != null ? n.v_NroDocumento.Trim() : "")
                                                              : "",
                                                      DocumentoRef =
                                                          J5 != null && n.v_NroDocumentoRef != null
                                                              ? J5.v_Siglas.Trim() + " " +
                                                                (n.v_NroDocumentoRef != null ? n.v_NroDocumentoRef.Trim() : "")
                                                              : "",
                                                      Analisis = string.IsNullOrEmpty(n.v_Analisis) ? J4.v_Glosa : n.v_Analisis,
                                                      IdMoneda = J4 != null ? J4.i_IdMoneda : 1,
                                                      IdMonedaCuenta = J1 != null ? J1.i_IdMoneda : 0,
                                                      SiglaMoneda = J4 != null ? J4.i_IdMoneda == 1 ? "S" : "D" : "",
                                                      Importe = n.d_Importe ?? 0,
                                                      ImporteCambio = n.d_Cambio ?? 0,
                                                      Key =
                                                          J3 != null && J2 != null
                                                              ? J3.v_IdCliente + n.v_NroCuenta + (n.i_IdTipoDocumento != 7 && n.i_IdTipoDocumento != 8 ? J2.v_Siglas : J5 != null ? J5.v_Siglas : "") +
                                                              (n.i_IdTipoDocumento != 7 && n.i_IdTipoDocumento != 8 ?
                                                                    (n.v_NroDocumento != null ? n.v_NroDocumento.Trim() : "") : (n.v_NroDocumentoRef != null ? n.v_NroDocumentoRef.Trim() : ""))
                                                              : "",
                                                      NaturalezaCuenta = J1 != null ? J1.i_Naturaleza : 0,
                                                      NaturalezaRegistro = n.v_Naturaleza,
                                                      CuentaConDetalle = J1 != null && (J1.i_Detalle ?? 0) == 1,
                                                      TipoCambio = J4 != null ? J4.d_TipoCambio : 0,
                                                      IdAnexo = J3 != null ? J3.v_IdCliente : "",
                                                      Detalle_Ruc = J3 != null ? J3.v_NroDocIdentificacion : "",
                                                      IdTipoDocumento = n.i_IdTipoDocumento ?? -1,
                                                      IdTipoDocumentoRef = n.i_IdTipoDocumentoRef ?? -1,
                                                      ID = n.v_IdDiario,
                                                      IDAsientoDetalle = n.v_IdDiarioDetalle,
                                                      EntidadFinanciera = J1 != null ? J1.i_EntFinanciera ?? 0 : 0,
                                                      Detalle_TipoIdentificacion = J3 != null ? J3.i_IdTipoIdentificacion ?? -1 : -1,
                                                      EsAjuste = J4 != null && (J4.i_IdTipoComprobante ?? 0) == 6,
                                                      DocumentoRaw = n.v_NroDocumento,
                                                      DocumentoRefRaw = n.v_NroDocumentoRef,
                                                      IdCentroCosto = n.i_IdCentroCostos == null ? "" : n.i_IdCentroCostos.Trim(),
                                                      i_IdTipoComprobante = J4.i_IdTipoComprobante,
                                                      NroCuentaEntidadFinanciera = J1 != null ? J1.v_Area : "",
                                                      NombreCliente = J3 != null ? (J3.v_ApePaterno + " " + J3.v_ApeMaterno + " " + J3.v_PrimerNombre + " " + J3.v_SegundoNombre + " " + J3.v_RazonSocial).Trim() : "",
                                                      NroRegistro = J4.v_Mes.Trim() + J4.v_Correlativo.Trim(),
                                                      TipoDocumentoCabecera = J4.i_IdTipoDocumento ?? -1,
                                                      FlagCliente = J3 != null ? J3.v_FlagPantalla : "",
                                                      FechaDetalle = n != null ? n.t_Fecha : J4.t_Fecha,
                                                      CodigoCliente = J3 != null ? J3.v_CodCliente : "",
                                                      i_IdPatrimonioNeto = n.i_IdPatrimonioNeto,
                                                      TasaDetraccion = J4 != null ? J4.d_TasaDetraccion ?? 0 : 0,
                                                      CuentaDetraccion = J3 != null ? J3.v_NroCuentaDetraccion : "",
                                                      TipoOperacionDetraccion = J4 != null ? J4.i_IdTipoOperacionDetraccion ?? -1 : -1,
                                                      i_CodigoDetraccion = J4 != null ? J4.i_IdCodigoDetraccion ?? -1 : -1,
                                                      FechaEmision =J4!=null ? J4.t_FechaEmision ==null ? J4.t_Fecha :J4.t_FechaEmision : DateTime.Now ,
                                                      FechaVencimiento =J4 !=null ? J4.t_FechaVencimiento==null ?J4.t_Fecha :J4.t_FechaVencimiento:DateTime.Now,
                                                  });

                        #endregion

                        #region Filtro de cuentas con detalle si se requiere
                        if (cuentaConDetalle)
                            gruposDiarioSource = gruposDiarioSource.Where(p => p.CuentaConDetalle);
                        #endregion

                        #region Consulta Secundaria

                        gruposDiario = gruposDiarioSource.AsQueryable()
                            .Select(p => new ReporteAnalisisCuentasCteAnalitico
                            {
                                IdTipoDocumentoProvicion = p.TipoDocumentoCabecera,
                                CuentaMayor = p.CuentaMayor,
                                CuentaImputable = p.CuentaImputable,
                                NombreCuenta = "Cuenta: " + p.CuentaImputable + " " + p.NombreCuenta,
                                Detalle = p.Detalle_Ruc + ": " + p.Detalle,
                                Documento =
                                    p.SiglaMoneda + " \t" + p.DocumentoTransaccion + " \t" +
                                    (p.FechaDocumento.Value.ToShortDateString() + " \t" + p.Documento).Trim(),

                                FechaDocumento = p.FechaDocumento,
                                DocumentoKey = p.Key,
                                IdMoneda = (int)p.IdMoneda,
                                IdMonedaCuenta = (int)p.IdMonedaCuenta,
                                NaturalezaCuenta = p.NaturalezaCuenta == 1 ? "D" : "H",
                                NaturalezaRegistro = p.NaturalezaRegistro,
                                ImporteSolesD =
                                    p.NaturalezaRegistro == "D" ? p.IdMoneda == 1 ? p.Importe : p.ImporteCambio : 0,
                                ImporteSolesH =
                                    p.NaturalezaRegistro == "H" ? p.IdMoneda == 1 ? p.Importe : p.ImporteCambio : 0,
                                ImporteDolaresD =
                                    p.NaturalezaRegistro == "D" ? p.IdMoneda == 1 ? p.ImporteCambio : p.Importe : 0,
                                ImporteDolaresH =
                                    p.NaturalezaRegistro == "H" ? p.IdMoneda == 1 ? p.ImporteCambio : p.Importe : 0,
                                CuentaConDetalle = p.CuentaConDetalle,
                                TipoCambio = p.TipoCambio ?? 0,
                                IdAnexo = p.IdAnexo,
                                NroDocumento = p.IdTipoDocumento != 7 && p.IdTipoDocumento != 8 ? (p.Documento).Trim() : !string.IsNullOrEmpty(p.DocumentoRef)
                                       ? (p.DocumentoRef).Trim()
                                       : "",
                                DocumentoRef = p.IdTipoDocumento != 7 && p.IdTipoDocumento != 8 ?
                                   !string.IsNullOrEmpty(p.DocumentoRef)
                                       ? (p.DocumentoRef).Trim()
                                       : "" : (p.Documento).Trim(),

                                DocumentoProvicion = p.DocumentoTransaccion,
                                IdDocumentoProvicion = p.ID,
                                EntidadFinanceriaCuenta = p.EntidadFinanciera != null ? p.EntidadFinanciera.ToString() : "-1",
                                Analisis = p.Analisis,
                                Detalle_TipoIdentificacion = p.Detalle_TipoIdentificacion != null ? p.Detalle_TipoIdentificacion.ToString() : "-1",
                                Detalle_Ruc = p.Detalle_Ruc,
                                EsAjusteDiferenciaCambio = p.EsAjuste,
                                TipoDocumento = p.IdTipoDocumento,
                                TipoDocumentoRef = p.IdTipoDocumentoRef,
                                DocumentoRaw = p.DocumentoRaw,
                                DocumentoRefRaw = p.DocumentoRefRaw,
                                IdCentroCostos = p.IdCentroCosto,
                                Mes = p.FechaDocumento.Value.Month.ToString(),
                                CentroCosto = p.IdCentroCosto,
                                i_IdTipoComprobante = p.i_IdTipoComprobante.Value,
                                NroCuentaEntidadFinanciera = p.NroCuentaEntidadFinanciera,
                                NombreCliente = p.NombreCliente,
                                NroRegistro = p.TipoDocumentoCabecera.ToString("000") + p.NroRegistro,
                                FlagCliente = p.FlagCliente,
                                FechaDetalle = p.FechaDetalle.Value,
                                CodigoCliente = p.CodigoCliente,
                                i_IdPatrimonioNeto = p.i_IdPatrimonioNeto == null ? 0 : int.Parse(p.i_IdPatrimonioNeto),
                                IdDocumentoProvicionDetalle = p.IDAsientoDetalle,
                                TasaDetraccionVenta = p.TasaDetraccion,
                                CuentaDetraccion = p.CuentaDetraccion,
                                TipoOperacionDetraccion = p.TipoOperacionDetraccion,
                                i_CodigoDetraccion = p.i_CodigoDetraccion,
                                FechaEmision =p.FechaEmision.Value ,
                                FechaVencimieto =p.FechaVencimiento.Value  ,
                            }).ToList();
                        #endregion

                    }
                    catch (AggregateException e)
                    {
                        pobjOperationResult.Success = 0;
                        pobjOperationResult.ErrorMessage = e.Message;
                        StringBuilder exeptions = new StringBuilder();
                        foreach (var ex in e.InnerExceptions)
                        {
                            exeptions.AppendLine(ex.Message);
                            if (ex is IndexOutOfRangeException)
                                pobjOperationResult.AdditionalInformation = "La data esta dañada.";
                        }
                        pobjOperationResult.ExceptionMessage = exeptions.ToString();
                        return null;
                    }

                    #endregion

                    #region Une tesorerias con diarios en una sola lista de entidades
                    gruposDiario = gruposDiario.Where(n => int.Parse(n.CuentaMayor.Substring(0, 2)) >= 00 && int.Parse(n.CuentaMayor.Substring(0, 2)) <= 99).ToList();

                    gruposTesoreria = gruposTesoreria.Where(n => int.Parse(n.CuentaMayor.Substring(0, 2)) >= 00 && int.Parse(n.CuentaMayor.Substring(0, 2)) <= 99).ToList();

                    var grupos = gruposDiario.Concat(gruposTesoreria).ToList();
                    #endregion

                    #region Realiza operaciones agregadas
                    if (tipoReporteAnalisis == TipoReporteAnalisis.AnalisisCuentaCorrienteResumen || tipoReporteAnalisis == TipoReporteAnalisis.AnalisisCorrientesPendientesAcumulado)
                    {
                        var registrosProvisionales = grupos.Where(p => p.NaturalezaCuenta.Equals(p.NaturalezaRegistro)).ToList();

                        foreach (var registroProvision in registrosProvisionales)
                        {
                            var contraCuentas = grupos.Where(p => p.DocumentoKey == registroProvision.DocumentoKey && p.NaturalezaRegistro != registroProvision.NaturalezaRegistro).ToList();

                            if (contraCuentas.Any())
                            {
                                var provision = registroProvision;
                                foreach (var contraCuenta in contraCuentas)
                                {
                                    if (provision.NaturalezaRegistro == "D")
                                    {
                                        provision.ImporteSolesH += contraCuenta.ImporteSolesH;
                                        provision.ImporteDolaresH += contraCuenta.ImporteDolaresH;
                                        grupos.Remove(contraCuenta);
                                    }
                                    else
                                    {
                                        provision.ImporteSolesD += contraCuenta.ImporteSolesD;
                                        provision.ImporteDolaresD += contraCuenta.ImporteDolaresD;
                                        grupos.Remove(contraCuenta);
                                    }
                                }
                            }
                        }

                        if (tipoReporteAnalisis == TipoReporteAnalisis.AnalisisCorrientesPendientesAcumulado)
                        {
                            grupos = grupos.Where(p => (p.ImporteSolesD ?? 0 - p.ImporteSolesH ?? 0) > 0).ToList();
                        }
                    }

                    if (tipoReporteAnalisis == TipoReporteAnalisis.AnalisisCorrientesPendientesAnalitico)
                    {
                        var gruposDiccionario = grupos.GroupBy(g => g.DocumentoKey).ToDictionary(k => k.Key, o => o.ToList());
                        var ajustesDiferenciaCambio = grupos.Where(p => p.EsAjusteDiferenciaCambio).GroupBy(g => g.DocumentoKey)
                                                            .ToDictionary(k => k.Key, o => o.ToList());

                        var registrosProvisionales = grupos.Where(p => !p.EsAjusteDiferenciaCambio && p.NaturalezaCuenta.Equals(p.NaturalezaRegistro)).ToList();

                        var agrupado = registrosProvisionales.GroupBy(p => p.DocumentoKey).ToList();
                        foreach (var registroProvision in agrupado.AsParallel())
                        {
                            var provicionPrimero = registroProvision.FirstOrDefault();
                            if (provicionPrimero == null) continue;

                            var naturalezaProvicion = provicionPrimero.NaturalezaRegistro;
                            List<ReporteAnalisisCuentasCteAnalitico> gruposDiccionarioResult;
                            var cCuentas = gruposDiccionario.TryGetValue(registroProvision.Key,
                                out gruposDiccionarioResult)
                                ? gruposDiccionarioResult
                                : new List<ReporteAnalisisCuentasCteAnalitico>();

                            var contraCuentas = cCuentas.Where(p => !p.NaturalezaCuenta.Equals(p.NaturalezaRegistro)).ToList();

                            List<ReporteAnalisisCuentasCteAnalitico> ajustes;
                            var ajuste = ajustesDiferenciaCambio.TryGetValue(registroProvision.Key, out ajustes) ? ajustes : new List<ReporteAnalisisCuentasCteAnalitico>();

                            if (naturalezaProvicion.Equals("D"))
                            {
                                var saldoD = provicionPrimero.IdMoneda == 1 ?
                                    Utils.Windows.DevuelveValorRedondeado(registroProvision.Sum(p => p.ImporteSolesD ?? 0) - contraCuentas.Sum(p => p.ImporteSolesH ?? 0), 1) :
                                    Utils.Windows.DevuelveValorRedondeado(registroProvision.Sum(p => p.ImporteDolaresD ?? 0) - contraCuentas.Sum(p => p.ImporteDolaresH ?? 0), 1);

                                if (saldoD == 0M)
                                {
                                    registroProvision.ToList().ForEach(p => grupos.Remove(p));
                                    ajuste.ForEach(p => grupos.Remove(p));
                                    contraCuentas.ForEach(p => grupos.Remove(p));
                                }

                                if (saldoD >= 0) continue;
                                registroProvision.ToList().ForEach(p => p.Sospechoso = true);
                                ajuste.ForEach(p => p.Sospechoso = true);
                                contraCuentas.ForEach(p => p.Sospechoso = true);
                            }
                            else
                            {
                                var saldoH = provicionPrimero.IdMoneda == 1 ?
                                    Utils.Windows.DevuelveValorRedondeado(registroProvision.Sum(p => p.ImporteSolesH ?? 0) - contraCuentas.Sum(p => p.ImporteSolesD ?? 0), 1) :
                                    Utils.Windows.DevuelveValorRedondeado(registroProvision.Sum(p => p.ImporteDolaresH ?? 0) - contraCuentas.Sum(p => p.ImporteDolaresD ?? 0), 1);


                                if (saldoH == 0M)
                                {
                                    registroProvision.ToList().ForEach(p => grupos.Remove(p));
                                    ajuste.ForEach(p => grupos.Remove(p));
                                    contraCuentas.ForEach(p => grupos.Remove(p));
                                }

                                if (saldoH >= 0) continue;
                                registroProvision.ToList().ForEach(p => p.Sospechoso = true);
                                ajuste.ForEach(p => p.Sospechoso = true);
                                contraCuentas.ForEach(p => p.Sospechoso = true);
                            }
                        }
                    }
                    #endregion

                    var grupoSinDetalle = grupos.Where(p => !p.CuentaConDetalle).OrderBy(t => t.FechaDocumento);
                    var grupoConDetalle =
                        grupos.Where(p => p.CuentaConDetalle)
                            .OrderBy(x => x.DocumentoKey)
                            .ThenBy(t => t.FechaDocumento)
                            .ToList();

                    grupos = grupoSinDetalle.Concat(grupoConDetalle).ToList();
                    if (mostrarSoloSospechosos)
                        grupos = grupos.Where(p => p.Sospechoso).ToList();
                    pobjOperationResult.Success = 1;

                    return grupos;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AsientosContablesBL.ReporteAnalisisCuentasCteAnalitico()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        /// <summary>
        /// Retorna una lista de saldos contables hasta el mes indicado. Reemplaza la tabla saldoscontables.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="mes"></param>
        /// <param name="periodo"></param>
        /// <param name="mesInicio"></param>
        /// <returns></returns>
        public List<saldoscontablesDto> ObtenerSaldosContablesPorMes(ref OperationResult pobjOperationResult, int mes, int periodo, int mesInicio = 1, bool CentroCosto = false, bool EsAsientoCierre = false, string CuentaMayor = null, bool EsReporteCierreMensual = false, string CuentaInicial = null, string CuentaFinal = null, bool balancecomprobacion = false, bool PatrimonioNeto = false)
        {
            try
            {
                var fechaInicio = new DateTime(periodo, mesInicio, 1);
                var fechaFin = DateTime.Parse(new DateTime(periodo, mes, DateTime.DaysInMonth(periodo, mes)).ToShortDateString() + " 23:59");

                List<ReporteAnalisisCuentasCteAnalitico> analisisCuentas = new List<ReporteAnalisisCuentasCteAnalitico>();
                analisisCuentas = ReporteAnalisisCuentasCteAnalitico(ref pobjOperationResult,
                    fechaInicio, fechaFin, CuentaInicial, CuentaFinal, false, null,
                    TipoReporteAnalisis.AnalisisCuentaCorrienteAnalitico, 0, null, CuentaMayor, false);

                if (!EsAsientoCierre)
                {


                    if (EsReporteCierreMensual)
                    {
                        # region Reporte Cierre Mensual Saldos
                        var dataSource = analisisCuentas.GroupBy(p => new { p.CuentaImputable, p.Mes })
                               .Select(reporteAnalisisCuentase => new saldoscontablesDto
                               {
                                   CuentaMayor = reporteAnalisisCuentase.Key.CuentaImputable.Substring(0, 2),
                                   d_ImporteDolaresD = reporteAnalisisCuentase.Sum(o => o.ImporteDolaresD ?? 0),
                                   d_ImporteDolaresH = reporteAnalisisCuentase.Sum(o => o.ImporteDolaresH ?? 0),
                                   d_ImporteSolesD = reporteAnalisisCuentase.Sum(o => o.ImporteSolesD ?? 0),
                                   d_ImporteSolesH = reporteAnalisisCuentase.Sum(o => o.ImporteSolesH ?? 0),
                                   i_IdMoneda = 1,
                                   v_NroCuenta = reporteAnalisisCuentase.Key.CuentaImputable,
                                   v_Mes = reporteAnalisisCuentase.Key.Mes,
                                   Naturaleza = reporteAnalisisCuentase.FirstOrDefault().NaturalezaRegistro,

                               }).ToList();
                        return dataSource;
                        #endregion

                    }

                    if (!CentroCosto)
                    {
                        if (PatrimonioNeto)
                        {
                            #region PatrimonioNeto
                            //No se toma en cuenta los asientos de apertura
                            var dataSource = analisisCuentas.Where(o => o.i_IdTipoComprobante != 1).GroupBy(p => new { p.CuentaImputable, p.i_IdPatrimonioNeto })
                                    .Select(reporteAnalisisCuentase => new saldoscontablesDto
                                    {
                                        CuentaMayor = reporteAnalisisCuentase.Key.CuentaImputable.Substring(0, 2),
                                        d_ImporteDolaresD = reporteAnalisisCuentase.Sum(o => o.ImporteDolaresD ?? 0),
                                        d_ImporteDolaresH = reporteAnalisisCuentase.Sum(o => o.ImporteDolaresH ?? 0),
                                        d_ImporteSolesD = reporteAnalisisCuentase.Sum(o => o.ImporteSolesD ?? 0),
                                        d_ImporteSolesH = reporteAnalisisCuentase.Sum(o => o.ImporteSolesH ?? 0),
                                        i_IdMoneda = 1,
                                        v_NroCuenta = reporteAnalisisCuentase.Key.CuentaImputable,
                                        i_IdPatrimonioNeto = reporteAnalisisCuentase.Key.i_IdPatrimonioNeto,
                                        Naturaleza = reporteAnalisisCuentase.FirstOrDefault().NaturalezaRegistro,

                                    }).ToList();
                            return dataSource;
                            #endregion
                        }

                        else
                        {

                            if (balancecomprobacion) // se agrego paar no tomar en  cuenta los asientos de apertura
                            {
                                var fff = analisisCuentas.Where(o => o.CuentaMayor.StartsWith("50")).ToList();
                                var dataSource = analisisCuentas.Where(o => o.i_IdTipoComprobante != 1).GroupBy(p => p.CuentaImputable)
                                 .Select(reporteAnalisisCuentase => new saldoscontablesDto
                                 {
                                     CuentaMayor = reporteAnalisisCuentase.Key.Substring(0, 2),
                                     d_ImporteDolaresD = reporteAnalisisCuentase.Sum(o => o.ImporteDolaresD ?? 0),
                                     d_ImporteDolaresH = reporteAnalisisCuentase.Sum(o => o.ImporteDolaresH ?? 0),
                                     d_ImporteSolesD = reporteAnalisisCuentase.Sum(o => o.ImporteSolesD ?? 0),
                                     d_ImporteSolesH = reporteAnalisisCuentase.Sum(o => o.ImporteSolesH ?? 0),
                                     i_IdMoneda = 1,
                                     v_NroCuenta = reporteAnalisisCuentase.Key,

                                 }).ToList();

                                var ggg = dataSource.Where(o => o.CuentaMayor == "50").ToList();
                                return dataSource;


                            }
                            else
                            {
                                #region Otros
                                var dataSource = analisisCuentas.GroupBy(p => p.CuentaImputable)
                                    .Select(reporteAnalisisCuentase => new saldoscontablesDto
                                    {
                                        CuentaMayor = reporteAnalisisCuentase.Key.Substring(0, 2),
                                        d_ImporteDolaresD = reporteAnalisisCuentase.Sum(o => o.ImporteDolaresD ?? 0),
                                        d_ImporteDolaresH = reporteAnalisisCuentase.Sum(o => o.ImporteDolaresH ?? 0),
                                        d_ImporteSolesD = reporteAnalisisCuentase.Sum(o => o.ImporteSolesD ?? 0),
                                        d_ImporteSolesH = reporteAnalisisCuentase.Sum(o => o.ImporteSolesH ?? 0),
                                        i_IdMoneda = 1,
                                        v_NroCuenta = reporteAnalisisCuentase.Key,

                                    }).ToList();

                                return dataSource;
                                #endregion
                            }
                        }
                    }
                    else
                    {
                        #region CentroCosto

                        var dataSource = analisisCuentas.GroupBy(p => new { p.CuentaImputable, p.IdCentroCostos, p.Mes })
                                .Select(reporteAnalisisCuentase => new saldoscontablesDto
                                {
                                    CuentaMayor = reporteAnalisisCuentase.Key.CuentaImputable.Substring(0, 2),
                                    d_ImporteDolaresD = reporteAnalisisCuentase.Sum(o => o.ImporteDolaresD ?? 0),
                                    d_ImporteDolaresH = reporteAnalisisCuentase.Sum(o => o.ImporteDolaresH ?? 0),
                                    d_ImporteSolesD = reporteAnalisisCuentase.Sum(o => o.ImporteSolesD ?? 0),
                                    d_ImporteSolesH = reporteAnalisisCuentase.Sum(o => o.ImporteSolesH ?? 0),
                                    i_IdMoneda = 1,
                                    v_NroCuenta = reporteAnalisisCuentase.Key.CuentaImputable,
                                    IdCentroCosto = reporteAnalisisCuentase.Key.IdCentroCostos,
                                    v_Mes = reporteAnalisisCuentase.Key.Mes,
                                    Naturaleza = reporteAnalisisCuentase.FirstOrDefault().NaturalezaRegistro,

                                }).ToList();
                        return dataSource;
                        #endregion

                    }
                }
                else
                {


                    var dataSource1 = analisisCuentas.Where(o => o.CuentaMayor.StartsWith("5")).ToList().GroupBy(p => p.CuentaImputable)
                              .Select(reporteAnalisisCuentase => new saldoscontablesDto
                              {
                                  CuentaMayor = reporteAnalisisCuentase.Key.Substring(0, 2),
                                  d_ImporteDolaresD = reporteAnalisisCuentase.Sum(o => o.ImporteDolaresD ?? 0),
                                  d_ImporteDolaresH = reporteAnalisisCuentase.Sum(o => o.ImporteDolaresH ?? 0),
                                  d_ImporteSolesD = reporteAnalisisCuentase.Sum(o => o.ImporteSolesD ?? 0),
                                  d_ImporteSolesH = reporteAnalisisCuentase.Sum(o => o.ImporteSolesH ?? 0),
                                  v_NroCuenta = reporteAnalisisCuentase.Key,


                              }).ToList();

                    var dataSource2 = analisisCuentas.Where(o => o.i_IdTipoComprobante != 5 && !o.CuentaMayor.StartsWith("5")).ToList().GroupBy(p => p.CuentaImputable)
                            .Select(reporteAnalisisCuentase => new saldoscontablesDto
                            {
                                CuentaMayor = reporteAnalisisCuentase.Key.Substring(0, 2),
                                d_ImporteDolaresD = reporteAnalisisCuentase.Sum(o => o.ImporteDolaresD ?? 0),
                                d_ImporteDolaresH = reporteAnalisisCuentase.Sum(o => o.ImporteDolaresH ?? 0),
                                d_ImporteSolesD = reporteAnalisisCuentase.Sum(o => o.ImporteSolesD ?? 0),
                                d_ImporteSolesH = reporteAnalisisCuentase.Sum(o => o.ImporteSolesH ?? 0),
                                v_NroCuenta = reporteAnalisisCuentase.Key,


                            }).ToList();

                    return dataSource1.Concat(dataSource2).ToList();

                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AsientosContablesBL.ObtenerSaldosContablesPorMes()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        private static string ObtenerConsulta(int tipo, DateTime fechaIni, DateTime fechaFin, string nroCuenta,
            string nroDocumentoIdentidad)
        {
            var strFIni = string.Format("{0}-{1}-{2}", fechaIni.Year, fechaIni.Month, fechaIni.Day);
            var strFFin = string.Format("{0}-{1}-{2}", fechaFin.Year, fechaFin.Month, fechaFin.Day);
            string query;

            if (tipo == 1)

                #region query para diariodetalle

                query = @"SELECT " +
                        "\"Filter1\".\"v_IdTesoreriaDetalle\", " +
                        "\"Filter1\".\"i_EsDestino\", " +
                        "\"Filter1\".\"v_IdTesoreria1\" AS \"v_IdTesoreria\", " +
                        "\"Filter1\".\"v_IdCliente\", " +
                        "\"Filter1\".\"v_NroCuenta\", " +
                        "\"Filter1\".\"v_Naturaleza\", " +
                        "\"Filter1\".\"d_Importe\", " +
                        "\"Filter1\".\"d_Cambio\", " +
                        "\"Filter1\".\"i_IdCentroCostos\", " +
                        "\"Filter1\".\"i_IdCaja\", " +
                        "\"Filter1\".\"i_IdTipoDocumento1\" AS \"i_IdTipoDocumento\", " +
                        "\"Filter1\".\"v_NroDocumento\", " +
                        "\"Filter1\".\"t_Fecha\", " +
                        "\"Filter1\".\"i_IdTipoDocumentoRef\", " +
                        "\"Filter1\".\"v_NroDocumentoRef\", " +
                        "\"Filter1\".\"v_Analisis\", " +
                        "\"Filter1\".\"v_OrigenDestino\", " +
                        "\"Filter1\".\"v_Pedido\", " +
                        "\"Filter1\".\"i_Eliminado1\" AS \"i_Eliminado\", " +
                        "\"Filter1\".\"i_InsertaIdUsuario1\" AS \"i_InsertaIdUsuario\", " +
                        "\"Filter1\".\"t_InsertaFecha1\" AS \"t_InsertaFecha\", " +
                        "\"Filter1\".\"i_ActualizaIdUsuario1\" AS \"i_ActualizaIdUsuario\", " +
                        "\"Filter1\".\"t_ActualizaFecha1\" AS \"t_ActualizaFecha\" " +
                        "FROM   (SELECT " +
                        "\"Extent1\".\"v_IdTesoreriaDetalle\", " +
                        "\"Extent1\".\"i_EsDestino\", " +
                        "\"Extent1\".\"v_IdTesoreria\" AS \"v_IdTesoreria1\", " +
                        "\"Extent1\".\"v_IdCliente\", " +
                        "\"Extent1\".\"v_NroCuenta\", " +
                        "\"Extent1\".\"v_Naturaleza\", " +
                        "\"Extent1\".\"d_Importe\", " +
                        "\"Extent1\".\"d_Cambio\", " +
                        "\"Extent1\".\"i_IdCentroCostos\", " +
                        "\"Extent1\".\"i_IdCaja\", " +
                        "\"Extent1\".\"i_IdTipoDocumento\" AS \"i_IdTipoDocumento1\", " +
                        "\"Extent1\".\"v_NroDocumento\", " +
                        "\"Extent1\".\"t_Fecha\", " +
                        "\"Extent1\".\"i_IdTipoDocumentoRef\", " +
                        "\"Extent1\".\"v_NroDocumentoRef\", " +
                        "\"Extent1\".\"v_Analisis\", " +
                        "\"Extent1\".\"v_OrigenDestino\", " +
                        "\"Extent1\".\"v_Pedido\", " +
                        "\"Extent1\".\"i_Eliminado\" AS \"i_Eliminado1\", " +
                        "\"Extent1\".\"i_InsertaIdUsuario\" AS \"i_InsertaIdUsuario1\", " +
                        "\"Extent1\".\"t_InsertaFecha\" AS \"t_InsertaFecha1\", " +
                        "\"Extent1\".\"i_ActualizaIdUsuario\" AS \"i_ActualizaIdUsuario1\", " +
                        "\"Extent1\".\"t_ActualizaFecha\" AS \"t_ActualizaFecha1\", " +
                        "\"Extent2\".\"v_IdTesoreria\" AS \"v_IdTesoreria2\", " +
                        "\"Extent2\".\"v_Periodo\", " +
                        "\"Extent2\".\"i_IdTipoDocumento\" AS \"i_IdTipoDocumento2\", " +
                        "\"Extent2\".\"v_Mes\", " +
                        "\"Extent2\".\"v_IdCobranza\", " +
                        "\"Extent2\".\"v_NroCuentaCajaBanco\", " +
                        "\"Extent2\".\"v_Correlativo\", " +
                        "\"Extent2\".\"i_TipoMovimiento\", " +
                        "\"Extent2\".\"t_FechaRegistro\", " +
                        "\"Extent2\".\"d_TipoCambio\", " +
                        "\"Extent2\".\"i_IdMedioPago\", " +
                        "\"Extent2\".\"v_Nombre\", " +
                        "\"Extent2\".\"v_Glosa\", " +
                        "\"Extent2\".\"i_IdMoneda\", " +
                        "\"Extent2\".\"i_IdEstado\", " +
                        "\"Extent2\".\"d_TotalDebe_Importe\", " +
                        "\"Extent2\".\"d_TotalDebe_Cambio\", " +
                        "\"Extent2\".\"d_TotalHaber_Importe\", " +
                        "\"Extent2\".\"d_TotalHaber_Cambio\", " +
                        "\"Extent2\".\"d_Diferencia_Importe\", " +
                        "\"Extent2\".\"d_Diferencia_Cambio\", " +
                        "\"Extent2\".\"i_AplicaRetencion\", " +
                        "\"Extent2\".\"i_Eliminado\" AS \"i_Eliminado2\", " +
                        "\"Extent2\".\"i_InsertaIdUsuario\" AS \"i_InsertaIdUsuario2\", " +
                        "\"Extent2\".\"t_InsertaFecha\" AS \"t_InsertaFecha2\", " +
                        "\"Extent2\".\"i_ActualizaIdUsuario\" AS \"i_ActualizaIdUsuario2\", " +
                        "\"Extent2\".\"t_ActualizaFecha\" AS \"t_ActualizaFecha2\" " +
                        "FROM  tesoreriadetalle AS \"Extent1\" " +
                        "LEFT OUTER JOIN tesoreria AS \"Extent2\" ON \"Extent1\".\"v_IdTesoreria\" = \"Extent2\".\"v_IdTesoreria\" " +
                        "WHERE \"Extent1\".\"i_Eliminado\" = 0 ) AS \"Filter1\" " +
                        "LEFT OUTER JOIN cliente AS \"Extent3\" ON \"Filter1\".\"v_IdCliente\" = \"Extent3\".\"v_IdCliente\" " +
                        "WHERE (((\"Filter1\".\"t_FechaRegistro\" >= '" + strFIni + "') " +
                        "AND (\"Filter1\".\"t_FechaRegistro\" <= '" + strFFin + " 23:59')) " +
                        "AND (('' = '" + nroCuenta + "') OR (\"Filter1\".\"v_NroCuenta\" = '" + nroCuenta + "'))) " +
                        "AND (('' = '" + nroDocumentoIdentidad + "') OR (\"Extent3\".\"v_NroDocIdentificacion\" = '" + nroDocumentoIdentidad + "')); ";

                #endregion

            else
                #region query para tesoreriadetalle

                query = "SELECT  " +
                        "\"Filter1\".\"v_IdDiarioDetalle\", " +
                        "\"Filter1\".\"i_EsDestino\", " +
                        "\"Filter1\".\"v_IdDiario1\" AS \"v_IdDiario\", " +
                        "\"Filter1\".\"v_IdCliente\", " +
                        "\"Filter1\".\"v_NroCuenta\", " +
                        "\"Filter1\".\"v_Naturaleza\", " +
                        "\"Filter1\".\"d_Importe\", " +
                        "\"Filter1\".\"d_Cambio\", " +
                        "\"Filter1\".\"i_IdCentroCostos\", " +
                        "\"Filter1\".\"i_IdTipoDocumento1\" AS \"i_IdTipoDocumento\", " +
                        "\"Filter1\".\"v_NroDocumento\", " +
                        "\"Filter1\".\"t_Fecha1\" AS \"t_Fecha\", " +
                        "\"Filter1\".\"i_IdTipoDocumentoRef\", " +
                        "\"Filter1\".\"v_NroDocumentoRef\", " +
                        "\"Filter1\".\"v_Analisis\", " +
                        "\"Filter1\".\"v_OrigenDestino\", " +
                        "\"Filter1\".\"v_Pedido\", " +
                        "\"Filter1\".\"i_Eliminado1\" AS \"i_Eliminado\", " +
                        "\"Filter1\".\"i_InsertaIdUsuario1\" AS \"i_InsertaIdUsuario\", " +
                        "\"Filter1\".\"t_InsertaFecha1\" AS \"t_InsertaFecha\", " +
                        "\"Filter1\".\"i_ActualizaIdUsuario1\" AS \"i_ActualizaIdUsuario\", " +
                        "\"Filter1\".\"t_ActualizaFecha1\" AS \"t_ActualizaFecha\" " +
                        "FROM   (SELECT " +
                        "\"Extent1\".\"v_IdDiarioDetalle\", " +
                        "\"Extent1\".\"i_EsDestino\", " +
                        "\"Extent1\".\"v_IdDiario\" AS \"v_IdDiario1\", " +
                        "\"Extent1\".\"v_IdCliente\", " +
                        "\"Extent1\".\"v_NroCuenta\", " +
                        "\"Extent1\".\"v_Naturaleza\", " +
                        "\"Extent1\".\"d_Importe\", " +
                        "\"Extent1\".\"d_Cambio\", " +
                        "\"Extent1\".\"i_IdCentroCostos\", " +
                        "\"Extent1\".\"i_IdTipoDocumento\" AS \"i_IdTipoDocumento1\", " +
                        "\"Extent1\".\"v_NroDocumento\", " +
                        "\"Extent1\".\"t_Fecha\" AS \"t_Fecha1\", " +
                        "\"Extent1\".\"i_IdTipoDocumentoRef\", " +
                        "\"Extent1\".\"v_NroDocumentoRef\", " +
                        "\"Extent1\".\"v_Analisis\", " +
                        "\"Extent1\".\"v_OrigenDestino\", " +
                        "\"Extent1\".\"v_Pedido\", " +
                        "\"Extent1\".\"i_Eliminado\" AS \"i_Eliminado1\", " +
                        "\"Extent1\".\"i_InsertaIdUsuario\" AS \"i_InsertaIdUsuario1\", " +
                        "\"Extent1\".\"t_InsertaFecha\" AS \"t_InsertaFecha1\", " +
                        "\"Extent1\".\"i_ActualizaIdUsuario\" AS \"i_ActualizaIdUsuario1\", " +
                        "\"Extent1\".\"t_ActualizaFecha\" AS \"t_ActualizaFecha1\", " +
                        "\"Extent2\".\"v_IdDiario\" AS \"v_IdDiario2\", " +
                        "\"Extent2\".\"v_IdDocumentoReferencia\", " +
                        "\"Extent2\".\"i_IdPlanillaNumeracion\", " +
                        "\"Extent2\".\"v_Periodo\", " +
                        "\"Extent2\".\"v_Mes\", " +
                        "\"Extent2\".\"i_IdTipoDocumento\" AS \"i_IdTipoDocumento2\", " +
                        "\"Extent2\".\"v_Correlativo\", " +
                        "\"Extent2\".\"d_TipoCambio\", " +
                        "\"Extent2\".\"i_IdTipoComprobante\", " +
                        "\"Extent2\".\"i_IdMoneda\", " +
                        "\"Extent2\".\"i_IdEstado\", " +
                        "\"Extent2\".\"v_Nombre\", " +
                        "\"Extent2\".\"v_Glosa\", " +
                        "\"Extent2\".\"t_Fecha\" AS \"t_Fecha2\", " +
                        "\"Extent2\".\"d_TotalDebe\", " +
                        "\"Extent2\".\"d_TotalHaber\", " +
                        "\"Extent2\".\"d_TotalDebeCambio\", " +
                        "\"Extent2\".\"d_TotalHaberCambio\", " +
                        "\"Extent2\".\"d_DiferenciaDebe\", " +
                        "\"Extent2\".\"d_DiferenciaHaber\", " +
                        "\"Extent2\".\"i_Eliminado\" AS \"i_Eliminado2\", " +
                        "\"Extent2\".\"i_InsertaIdUsuario\" AS \"i_InsertaIdUsuario2\", " +
                        "\"Extent2\".\"t_InsertaFecha\" AS \"t_InsertaFecha2\", " +
                        "\"Extent2\".\"i_ActualizaIdUsuario\" AS \"i_ActualizaIdUsuario2\", " +
                        "\"Extent2\".\"t_ActualizaFecha\" AS \"t_ActualizaFecha2\" " +
                        "FROM  diariodetalle AS \"Extent1\" " +
                        "LEFT OUTER JOIN diario AS \"Extent2\" ON \"Extent1\".\"v_IdDiario\" = \"Extent2\".\"v_IdDiario\" " +
                        "WHERE \"Extent1\".\"i_Eliminado\" = 0 ) AS \"Filter1\" " +
                        "LEFT OUTER JOIN cliente AS \"Extent3\" ON \"Filter1\".\"v_IdCliente\" = \"Extent3\".\"v_IdCliente\" " +
                        "WHERE (((\"Filter1\".\"t_Fecha2\" >= '" + strFIni + "') " +
                        "AND (\"Filter1\".\"t_Fecha2\" <= '" + strFFin + " 23:59')) " +
                        "AND (('' = '" + nroCuenta + "') OR (\"Filter1\".\"v_NroCuenta\" = '" + nroCuenta + "'))) " +
                        "AND (('' = '" + nroDocumentoIdentidad + "') OR (\"Extent3\".\"v_NroDocIdentificacion\" = '" + nroDocumentoIdentidad + "')); ";
                #endregion

            return query;

        }

        /// <summary>
        /// Se obtiene la consulta sql para ser ejecutada en dapper.
        /// </summary>
        /// <param name="tipo"></param>
        /// <param name="fechaIni"></param>
        /// <param name="fechaFin"></param>
        /// <param name="idAnexo"></param>
        /// <param name="cuentas"></param>
        /// <param name="idtipoDoc"></param>
        /// <param name="nroDoc"></param>
        /// <param name="nroCuentaMayor"></param>
        /// <returns></returns>
        public string ObtenerConsultaAnalitico(int tipo, DateTime fechaIni, DateTime fechaFin,
            string idAnexo, ICollection<string> cuentas, int idtipoDoc, string nroDoc, string nroCuentaMayor)
        {
            idAnexo = string.IsNullOrWhiteSpace(idAnexo) ? "" : idAnexo;
            var strFIni = string.Format("{0}-{1}-{2}", fechaIni.Year, fechaIni.Month, fechaIni.Day);
            var strFFin = string.Format("{0}-{1}-{2}", fechaFin.Year, fechaFin.Month, fechaFin.Day);
            var ctas = cuentas.Count > 0 ? string.Join(", ", cuentas.Select(p => "'" + p.Trim() + "'")) : "''";
            nroDoc = string.IsNullOrWhiteSpace(nroDoc) ? "null" : "'" + nroDoc + "'";
            string query;
            nroCuentaMayor = string.IsNullOrWhiteSpace(nroCuentaMayor) ? string.Empty : nroCuentaMayor.Trim();

            if (tipo == 1)
                #region query para diariodetalle

                query = @"SELECT " +
                        "\"Extent1\".\"v_IdDiarioDetalle\", " +
                        "\"Extent1\".\"i_EsDestino\"," +
                        "\"Extent1\".\"v_IdDiario\"," +
                        "\"Extent1\".\"v_IdCliente\"," +
                        "\"Extent1\".\"v_NroCuenta\"," +
                        "\"Extent1\".\"v_Naturaleza\"," +
                        "\"Extent1\".\"d_Importe\"," +
                        "\"Extent1\".\"d_Cambio\"," +
                        "\"Extent1\".\"i_IdCentroCostos\"," +
                         "\"Extent1\".\"i_IdPatrimonioNeto\"," +
                        "\"Extent1\".\"i_IdTipoDocumento\"," +
                        "\"Extent1\".\"v_NroDocumento\"," +
                        "\"Extent1\".\"t_Fecha\"," +
                        "\"Extent1\".\"i_IdTipoDocumentoRef\"," +
                        "\"Extent1\".\"v_NroDocumentoRef\"," +
                        "\"Extent1\".\"v_Analisis\"," +
                        "\"Extent1\".\"v_OrigenDestino\"," +
                        "\"Extent1\".\"v_Pedido\"," +
                        "\"Extent1\".\"i_Eliminado\"," +
                        "\"Extent1\".\"i_InsertaIdUsuario\"," +
                        "\"Extent1\".\"t_InsertaFecha\"," +
                        "\"Extent1\".\"i_ActualizaIdUsuario\"," +
                        "\"Extent1\".\"t_ActualizaFecha\" " +
                        "FROM  diariodetalle AS \"Extent1\" " +
                        "INNER JOIN diario AS \"Extent2\" ON \"Extent1\".\"v_IdDiario\" = \"Extent2\".\"v_IdDiario\" " +
                        "WHERE (((((\"Extent1\".\"i_Eliminado\" = 0) AND (\"Extent2\".\"t_Fecha\" >= '" + strFIni +
                        "')) " +
                        "AND (\"Extent2\".\"t_Fecha\" <= '" + strFFin + " 23:59')) AND (('' = '" + idAnexo + "') " +
                        "OR (\"Extent1\".\"v_IdCliente\" = '" + idAnexo + "'))) AND ((0 = " + cuentas.Count +
                        ") OR (\"Extent1\".\"v_NroCuenta\" IN (" + ctas + "))) AND (\"Extent1\".\"v_NroCuenta\" like '" + nroCuentaMayor + "%')) " +
                        "AND (((" + idtipoDoc + " < 1) AND ( CAST(" + nroDoc + " AS varchar) IS NULL)) " +
                        "OR (((CASE WHEN \"Extent1\".\"i_IdTipoDocumento\" IS NULL THEN -1 ELSE \"Extent1\".\"i_IdTipoDocumento\" END) = " +
                        idtipoDoc + ") " +
                        "AND (\"Extent1\".\"v_NroDocumento\" = " + nroDoc + "))) ";
                #endregion

            else
                #region query para tesoreriadetalle

                query = @"SELECT " +
                        "\"Extent1\".\"v_IdTesoreriaDetalle\", " +
                        "\"Extent1\".\"i_EsDestino\", " +
                        "\"Extent1\".\"v_IdTesoreria\", " +
                        "\"Extent1\".\"v_IdCliente\", " +
                        "\"Extent1\".\"v_NroCuenta\", " +
                        "\"Extent1\".\"v_Naturaleza\", " +
                        "\"Extent1\".\"d_Importe\", " +
                        "\"Extent1\".\"d_Cambio\", " +
                        "\"Extent1\".\"i_IdCentroCostos\", " +
                        "\"Extent1\".\"i_IdPatrimonioNeto\"," +
                        "\"Extent1\".\"i_IdCaja\", " +
                        "\"Extent1\".\"i_IdTipoDocumento\", " +
                        "\"Extent1\".\"v_NroDocumento\", " +
                        "\"Extent1\".\"t_Fecha\", " +
                        "\"Extent1\".\"i_IdTipoDocumentoRef\", " +
                        "\"Extent1\".\"v_NroDocumentoRef\", " +
                        "\"Extent1\".\"v_Analisis\", " +
                        "\"Extent1\".\"v_OrigenDestino\", " +
                        "\"Extent1\".\"v_Pedido\", " +
                        "\"Extent1\".\"i_Eliminado\", " +
                        "\"Extent1\".\"i_InsertaIdUsuario\", " +
                        "\"Extent1\".\"t_InsertaFecha\", " +
                        "\"Extent1\".\"i_ActualizaIdUsuario\", " +
                        "\"Extent1\".\"t_ActualizaFecha\" " +
                        "FROM  tesoreriadetalle AS \"Extent1\" " +
                        "INNER JOIN tesoreria AS \"Extent2\" ON \"Extent1\".\"v_IdTesoreria\" = \"Extent2\".\"v_IdTesoreria\" " +
                        "WHERE (((((\"Extent1\".\"i_Eliminado\" = 0) AND (\"Extent2\".\"t_FechaRegistro\" >= '" +
                        strFIni + "')) " +
                        "AND (\"Extent2\".\"t_FechaRegistro\" <= '" + strFFin + " 23:59')) AND (('' = '" + idAnexo +
                        "') " +
                        "OR (\"Extent1\".\"v_IdCliente\" = '" + idAnexo + "'))) AND ((0 = " + cuentas.Count +
                        ") OR (\"Extent1\".\"v_NroCuenta\" IN (" + ctas + "))) AND (\"Extent1\".\"v_NroCuenta\" like '" + nroCuentaMayor + "%')) " +
                        "AND (((" + idtipoDoc + " < 1) AND ( CAST(" + nroDoc + " AS varchar) IS NULL)) " +
                        "OR (((CASE WHEN \"Extent1\".\"i_IdTipoDocumento\" IS NULL THEN -1 ELSE \"Extent1\".\"i_IdTipoDocumento\" END) = " +
                        idtipoDoc + ") " +
                        "AND (\"Extent1\".\"v_NroDocumento\" = " + nroDoc + "))) ";

                #endregion

            return query;

        }

        #endregion

        public List<ReporteCuentasPagarDto> ReporteCuentasPagarContabilidad(ref OperationResult objOperationResult, DateTime fInicio, DateTime fFinal,string IdProveedor,int pIntMoneda,string Grupo,int DiasVencimiento,bool SoloVencidas, string Ordenar, string  Filtro)
        {
            try
            {
                var DiaReporte =DateTime.Now;
               var FechaVencimientoProx= DateTime.Parse(DiaReporte.AddDays (DiasVencimiento).Date.ToShortDateString ()+ " 23:59");
                var data = new AsientosContablesBL().ReporteAnalisisCuentasCteAnalitico(ref objOperationResult, fInicio,
                          fFinal, null, null, true, IdProveedor, TipoReporteAnalisis.AnalisisCorrientesPendientesAnalitico, 0, null, "42");

                var result = (from agrupado in data.GroupBy(g => new { anexo = g.IdAnexo, doc = g.DocumentoKey })
                              
                              select new ReporteCuentasPagarDto
                              {

                                  NombreCliente = agrupado.FirstOrDefault ().NombreCliente,
                                  FechaEmision =DateTime.Parse ( agrupado.FirstOrDefault().FechaEmision.ToShortDateString ()),
                                  NroDocumento = agrupado.FirstOrDefault().NroDocumento,
                                  FechaVencimiento = DateTime.Parse ( agrupado.FirstOrDefault ().FechaVencimieto.ToShortDateString ()) ,
                                  sFechaVencimiento = agrupado.FirstOrDefault().FechaVencimieto.ToShortDateString (),
                                  TotalFacturado = pIntMoneda == (int)Currency.Soles ? agrupado.Sum(o => o.ImporteSolesH) : agrupado.Sum(o => o.ImporteDolaresH),
                                  Acuenta = pIntMoneda == (int)Currency.Soles ? agrupado.Sum(o => o.ImporteSolesD) : agrupado.Sum(o => o.ImporteDolaresD),
                                  Moneda =pIntMoneda ==(int)Currency.Soles ?"S":"US$",
                                  Saldo = pIntMoneda == (int)Currency.Soles ? agrupado.Sum(o => o.ImporteSolesH) - agrupado.Sum(o => o.ImporteSolesD) : agrupado.Sum(o => o.ImporteDolaresH) - agrupado.Sum(o => o.ImporteDolaresD),
                                  Grupollave =Grupo =="PROVEEDOR"? agrupado.FirstOrDefault ().NombreCliente :"",
                              }).ToList();

                if (!string.IsNullOrEmpty(Ordenar))
                {
                    result = result.AsQueryable ().OrderBy(Ordenar).ToList();

                }
                if (!string.IsNullOrEmpty(Filtro))
                {
                    result = result.AsQueryable().Where(Filtro).ToList();
                }


                if (DiasVencimiento > 0)
                {
                    result = result.Where(o => o.FechaVencimiento <= FechaVencimientoProx).ToList();
                }
                else
                {
                    if (SoloVencidas)
                    {
                        result = result.Where(o => o.FechaVencimiento <= DiaReporte).ToList();
                    }
                }
                // pobjOperationResult.Success = 1;
                return result;
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "AsientoContableBL.ReporteCuentasPagarContabilidad()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }


        }

        #endregion

        #region Apertura Nuevo Periodo



        #endregion

        #region ConfiguracionBalances

        public int ConfiguracionBalanceNuevo(ref OperationResult pobjOperationResult, configuracionbalancesDto pobjDtoEntity, List<string> ClientSession, List<string> _TempCuentasAsociadas, int TipoConfiguracionBalances)
        {
            using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
            {
                try
                {

                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {


                        configuracionbalances objEntity = configuracionbalancesAssembler.ToEntity(pobjDtoEntity);
                        objEntity.t_InsertaFecha = DateTime.Now;
                        objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntity.i_Eliminado = 0;
                        dbContext.AddToconfiguracionbalances(objEntity);
                        dbContext.SaveChanges();

                        foreach (var item in _TempCuentasAsociadas)
                        {
                            var ListaCuentas = (from a in dbContext.asientocontable
                                                where a.v_NroCuenta == item && a.i_Eliminado == 0 && a.v_Periodo == periodo
                                                select a).ToList();

                            foreach (asientocontable lcuentas in ListaCuentas)
                            {
                                var Entity = (from a in dbContext.asientocontable
                                              where a.v_NroCuenta == lcuentas.v_NroCuenta && a.i_Eliminado == 0 && a.v_Periodo == periodo
                                              select a).FirstOrDefault();
                                var dtoAsientoContable = lcuentas.ToDTO();
                                dtoAsientoContable.t_ActualizaFecha = DateTime.Now;
                                dtoAsientoContable.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                                if (TipoConfiguracionBalances == (int)ConfiguracionBalances.SituacionFinanciera)
                                {
                                    //dtoAsientoContable.v_CodigoSituacionFinaciera = !string.IsNullOrEmpty(Entity.v_CodigoSituacionFinaciera) ? Entity.v_CodigoSituacionFinaciera + "," + pobjDtoEntity.v_Codigo : pobjDtoEntity.v_Codigo;
                                    dtoAsientoContable.v_CodigoSituacionFinaciera = !string.IsNullOrEmpty(Entity.v_CodigoSituacionFinaciera) ? Entity.v_CodigoSituacionFinaciera + "," + objEntity.i_IdConfiguracionBalance.ToString() : objEntity.i_IdConfiguracionBalance.ToString();
                                }
                                else if (TipoConfiguracionBalances == (int)ConfiguracionBalances.EstadodeResultadosFuncion)
                                {
                                    dtoAsientoContable.v_CodigoBalanceFuncion = pobjDtoEntity.v_Codigo;
                                }
                                else if (TipoConfiguracionBalances == (int)ConfiguracionBalances.EstadodeResultadosNaturaleza)
                                {
                                    dtoAsientoContable.v_CodigoBalanceNaturaleza = pobjDtoEntity.v_Codigo;
                                }
                                asientocontable asientoContable = asientocontableAssembler.ToEntity(dtoAsientoContable);
                                dbContext.asientocontable.ApplyCurrentValues(asientoContable);

                            }


                        }

                        dbContext.SaveChanges();
                        pobjOperationResult.Success = 1;
                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "configuracionbalances", objEntity.i_IdConfiguracionBalance.ToString());
                        ts.Complete();
                        return objEntity.i_IdConfiguracionBalance;
                    }
                }
                catch (Exception ex)
                {
                    pobjOperationResult.Success = 0;
                    pobjOperationResult.AdditionalInformation = "AsientosContablesBL.AsientoNuevo()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                    pobjOperationResult.ErrorMessage = ex.Message;
                    pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                    Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                    return 0;
                }


            }
        }
        public BindingList<asientocontableDto> DevuelveCuentasBalances(ref OperationResult pobjOperationResult, string CodigoBalance, int TipoBalance)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    if (TipoBalance == (int)ConfiguracionBalances.EstadodeResultadosFuncion)
                    {
                        List<asientocontableDto> Query = asientocontableAssembler.ToDTOs(dbContext.asientocontable.Where(p => p.v_CodigoBalanceFuncion == CodigoBalance && p.v_Periodo == periodo && p.i_Eliminado == 0).ToList());

                        pobjOperationResult.Success = 1;
                        return new BindingList<asientocontableDto>(Query);
                    }
                    else if (TipoBalance == (int)ConfiguracionBalances.SituacionFinanciera)
                    {

                        List<asientocontable> ListaAsientoContable = dbContext.asientocontable.Where(o => o.i_Eliminado == 0 && o.v_Periodo == periodo && o.v_CodigoSituacionFinaciera != null).ToList();
                        List<asientocontable> ListaFinal = new List<asientocontable>();
                        foreach (var item in ListaAsientoContable)
                        {
                            if (ListaCuentaCodigoBalancesBool(CodigoBalance, ListaAsientoContable, item.v_NroCuenta))
                                ListaFinal.Add(item);
                        }

                        //List<asientocontableDto> Query = asientocontableAssembler.ToDTOs(dbContext.asientocontable.Where(p => p.v_CodigoSituacionFinaciera == CodigoBalance && p.v_Periodo == periodo && p.i_Eliminado == 0).ToList());
                        //pobjOperationResult.Success = 1;
                        //return new BindingList<asientocontableDto>(Query);

                        List<asientocontableDto> Query = asientocontableAssembler.ToDTOs(ListaFinal);
                        pobjOperationResult.Success = 1;
                        return new BindingList<asientocontableDto>(Query);

                    }
                    else if (TipoBalance == (int)ConfiguracionBalances.EstadodeResultadosNaturaleza)
                    {
                        List<asientocontableDto> Query = asientocontableAssembler.ToDTOs(dbContext.asientocontable.Where(p => p.v_CodigoBalanceNaturaleza == CodigoBalance && p.v_Periodo == periodo && p.i_Eliminado == 0).ToList());

                        pobjOperationResult.Success = 1;
                        return new BindingList<asientocontableDto>(Query);
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AsientosContablesBL.DevuelveCuentasBalances()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }
        public List<configuracionbalancesDto> ObtenerDatosBalances(ref OperationResult pobjOperationResult, string v_TipoBalance, string Mes, string Periodo)
        {
            try
            {
                pobjOperationResult.Success = 1;
                List<configuracionbalancesDto> Balances = new List<configuracionbalancesDto>();
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    var AsientoContable = (from a in dbContext.asientocontable

                                           where a.i_Eliminado == 0 && a.v_Periodo == periodo && a.i_Imputable == 1

                                           select a).ToList();



                    if (((int)ConfiguracionBalances.EstadodeResultadosFuncion).ToString() == v_TipoBalance)
                    {

                        Balances = (from a in dbContext.configuracionbalances

                                    join b in dbContext.systemuser on new { i_UpdateUserId = a.i_ActualizaIdUsuario.Value, eliminado = 0 }
                                      equals new { i_UpdateUserId = b.i_SystemUserId, eliminado = b.i_IsDeleted.Value } into b_join
                                    from b in b_join.DefaultIfEmpty()

                                    join c in dbContext.systemuser on new { i_InsertUserId = a.i_InsertaIdUsuario.Value, eliminado = 0 }
                                        equals new { i_InsertUserId = c.i_SystemUserId, eliminado = c.i_IsDeleted.Value } into c_join
                                    from c in c_join.DefaultIfEmpty()

                                    where a.i_Eliminado == 0 && a.v_TipoBalance == v_TipoBalance

                                    select new configuracionbalancesDto
                                    {
                                        i_IdConfiguracionBalance = a.i_IdConfiguracionBalance,
                                        v_Codigo = a.v_Codigo,
                                        v_Nombre = a.v_Nombre,
                                        t_ActualizaFecha = a.t_ActualizaFecha,
                                        v_UsuarioModificacion = b.v_UserName,
                                        v_UsuarioCreacion = c.v_UserName,
                                        t_InsertaFecha = a.t_InsertaFecha,

                                    }).ToList().Select(o => new configuracionbalancesDto
                                    {
                                        i_IdConfiguracionBalance = o.i_IdConfiguracionBalance,
                                        v_Codigo = o.v_Codigo,
                                        v_Nombre = o.v_Nombre,
                                        t_ActualizaFecha = o.t_ActualizaFecha,
                                        v_UsuarioModificacion = o.v_UsuarioModificacion,
                                        v_UsuarioCreacion = o.v_UsuarioCreacion,
                                        t_InsertaFecha = o.t_InsertaFecha,
                                        ListaCuentasUsadas = (from d in AsientoContable
                                                              where d.v_CodigoBalanceFuncion == o.v_Codigo
                                                              select d.v_NroCuenta).AsEnumerable().OrderBy(z => z),

                                    }).ToList();
                    }
                    else if (((int)ConfiguracionBalances.SituacionFinanciera).ToString() == v_TipoBalance)
                    {

                        //var AsientoContable = (from a in dbContext.asientocontable

                        //                       where a.i_Eliminado == 0 && a.v_Periodo == periodo && a.i_Imputable ==1

                        //                       select a).ToList();


                        Balances = (from a in dbContext.configuracionbalances
                                    join b in dbContext.systemuser on new { i_UpdateUserId = a.i_ActualizaIdUsuario.Value, eliminado = 0 }
                                     equals new { i_UpdateUserId = b.i_SystemUserId, eliminado = b.i_IsDeleted.Value } into b_join
                                    from b in b_join.DefaultIfEmpty()

                                    join c in dbContext.systemuser on new { i_InsertUserId = a.i_InsertaIdUsuario.Value, eliminado = 0 }
                                        equals new { i_InsertUserId = c.i_SystemUserId, eliminado = c.i_IsDeleted.Value } into c_join
                                    from c in c_join.DefaultIfEmpty()
                                    where a.i_Eliminado == 0 && a.v_TipoBalance == v_TipoBalance && a.v_Mes == Mes && a.v_Periodo == Periodo

                                    select new configuracionbalancesDto
                                    {
                                        i_IdConfiguracionBalance = a.i_IdConfiguracionBalance,
                                        v_Codigo = a.v_Codigo,
                                        v_Nombre = a.v_Nombre,
                                        t_ActualizaFecha = a.t_ActualizaFecha,
                                        v_UsuarioModificacion = b.v_UserName,
                                        v_UsuarioCreacion = c.v_UserName,
                                        t_InsertaFecha = a.t_InsertaFecha,
                                        v_Mes = a.v_Mes,
                                        v_Periodo = a.v_Periodo,

                                    }).ToList().Select(o => new configuracionbalancesDto
                                    {
                                        i_IdConfiguracionBalance = o.i_IdConfiguracionBalance,
                                        v_Codigo = o.v_Codigo,
                                        v_Nombre = o.v_Nombre,
                                        t_ActualizaFecha = o.t_ActualizaFecha,
                                        v_UsuarioCreacion = o.v_UsuarioCreacion,
                                        v_UsuarioModificacion = o.v_UsuarioModificacion,
                                        t_InsertaFecha = o.t_InsertaFecha,
                                        v_Periodo = o.v_Periodo,
                                        v_Mes = o.v_Mes,
                                        ListaCuentasUsadas = ListaCuentaporCodigoBalances(o.i_IdConfiguracionBalance.ToString(), AsientoContable)

                                    }).ToList();

                    }
                    else
                        if (((int)ConfiguracionBalances.EstadodeResultadosNaturaleza).ToString() == v_TipoBalance)
                        {





                            Balances = (from a in dbContext.configuracionbalances

                                        join b in dbContext.systemuser on new { i_UpdateUserId = a.i_ActualizaIdUsuario.Value, eliminado = 0 }
                                          equals new { i_UpdateUserId = b.i_SystemUserId, eliminado = b.i_IsDeleted.Value } into b_join
                                        from b in b_join.DefaultIfEmpty()

                                        join c in dbContext.systemuser on new { i_InsertUserId = a.i_InsertaIdUsuario.Value, eliminado = 0 }
                                            equals new { i_InsertUserId = c.i_SystemUserId, eliminado = c.i_IsDeleted.Value } into c_join
                                        from c in c_join.DefaultIfEmpty()

                                        where a.i_Eliminado == 0 && a.v_TipoBalance == v_TipoBalance

                                        select new configuracionbalancesDto
                                        {
                                            i_IdConfiguracionBalance = a.i_IdConfiguracionBalance,
                                            v_Codigo = a.v_Codigo,
                                            v_Nombre = a.v_Nombre,
                                            t_ActualizaFecha = a.t_ActualizaFecha,
                                            v_UsuarioModificacion = b.v_UserName,
                                            v_UsuarioCreacion = c.v_UserName,
                                            t_InsertaFecha = a.t_InsertaFecha,


                                        }).ToList().Select(o => new configuracionbalancesDto
                                        {

                                            i_IdConfiguracionBalance = o.i_IdConfiguracionBalance,
                                            v_Codigo = o.v_Codigo,
                                            v_Nombre = o.v_Nombre,
                                            t_ActualizaFecha = o.t_ActualizaFecha,
                                            v_UsuarioModificacion = o.v_UsuarioModificacion,
                                            v_UsuarioCreacion = o.v_UsuarioCreacion,
                                            t_InsertaFecha = o.t_InsertaFecha,
                                            ListaCuentasUsadas = (from d in AsientoContable
                                                                  where d.v_CodigoBalanceNaturaleza == o.v_Codigo
                                                                  select d.v_NroCuenta).AsEnumerable().OrderBy(z => z),
                                        }).ToList();


                        }


                    var objData = Balances.ToList();
                    objData.AsParallel().ToList().ForEach(balance =>
                    {

                        balance.CtasVistaRapida = string.Join(", ", balance.ListaCuentasUsadas.Distinct());

                    });
                    return objData.OrderBy(o => o.v_Codigo).ToList();

                }
            }
            catch (Exception ex)
            {


                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AsientosContablesBL.ObtenerDatosBalances()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }


        }
        public List<string> ListaCuentaporCodigoBalances(string Codigo, List<asientocontable> Lista)
        {
            List<string> ListaCuentasUsadas = new List<string>();
            //var hhh = Lista.Where(o => o.v_CodigoSituacionFinaciera != null && o.v_CodigoSituacionFinaciera.Contains(Codigo)).ToList();
            var ListaSituacionFinanciera = Lista.Where(o => o.v_CodigoSituacionFinaciera != null && o.v_CodigoSituacionFinaciera.Contains(Codigo)).ToList();
            foreach (var item in ListaSituacionFinanciera)
            {
                var NroCuentas = item.v_CodigoSituacionFinaciera.Split(',');
                for (int i = 0; i < NroCuentas.Count(); i++)
                {
                    if (NroCuentas[i] == Codigo)
                    {
                        ListaCuentasUsadas.Add(item.v_NroCuenta);
                    }
                }
            }

            return ListaCuentasUsadas;

        }

        public bool ListaCuentaCodigoBalancesBool(string Codigo, List<asientocontable> Lista, string NroCuenta)
        {


            var hhh = Lista.Where(o => o.v_CodigoSituacionFinaciera.Contains(Codigo)).ToList();
            foreach (var item in hhh)
            {
                var NroCuentas = item.v_CodigoSituacionFinaciera.Split(',');
                for (int i = 0; i < NroCuentas.Count(); i++)
                {
                    if (NroCuentas[i] == Codigo && item.v_NroCuenta == NroCuenta)
                    {
                        return true;
                    }
                }
            }

            return false;

        }
        public string ObtenerMaximoCodigo(string TipoBalance, int LenghtCodigo, string Codigo)
        {
            var EmpiezaCon = Codigo.Substring(0, LenghtCodigo - 1);
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {

                var conf = (from a in dbContext.configuracionbalances
                            where a.i_Eliminado == 0 && a.v_TipoBalance == TipoBalance && a.v_Codigo.Length == LenghtCodigo && a.v_Codigo.StartsWith(EmpiezaCon)
                            select a.v_Codigo).Max();
                return conf;


            }

        }

        public configuracionbalancesDto ObtenerCabeceraBalance(ref OperationResult pobjOperationResult, int IdCodigoBalance, string TipoBalance)
        {
            try
            {
                pobjOperationResult.Success = 1;
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var balanceDto = (from a in dbContext.configuracionbalances
                                      where a.i_Eliminado == 0 && a.i_IdConfiguracionBalance == IdCodigoBalance && a.v_TipoBalance == TipoBalance
                                      select a).FirstOrDefault().ToDTO();
                    return balanceDto;

                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AsientosContablesBL.ObtenerCabeceraBalance()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public int ConfiguracionBalanceActualizar(ref OperationResult pobjOperationResult, configuracionbalancesDto pobjDtoEntity, List<string> ClientSession, List<string> _TempCuentasAsociadas, int TipoBalance)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {

                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.configuracionbalances
                                           where a.i_IdConfiguracionBalance == pobjDtoEntity.i_IdConfiguracionBalance && a.i_Eliminado == 0
                                           select a).FirstOrDefault();

                    pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                    pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                    configuracionbalances objEntity = configuracionbalancesAssembler.ToEntity(pobjDtoEntity);

                    dbContext.configuracionbalances.ApplyCurrentValues(objEntity);

                    #region pone en vacio todas las cuentas que se hayan asociado antes a este codigo

                    if (TipoBalance == (int)ConfiguracionBalances.EstadodeResultadosFuncion)
                    {
                        var CuentasAsociadas = dbContext.asientocontable.Where(o => o.v_CodigoBalanceFuncion == pobjDtoEntity.v_Codigo && o.i_Eliminado == 0 && o.v_Periodo == periodo).ToList();
                        CuentasAsociadas.ForEach(p => p.v_CodigoBalanceFuncion = null);
                        CuentasAsociadas.ForEach(p => dbContext.asientocontable.ApplyCurrentValues(p));
                        dbContext.SaveChanges();
                    }
                    else if (TipoBalance == (int)ConfiguracionBalances.SituacionFinanciera)
                    {

                        //var CuentasAsociadasComas = dbContext.asientocontable.Where(o => o.i_Eliminado == 0 && o.v_CodigoSituacionFinaciera != null && o.v_CodigoSituacionFinaciera.Contains(pobjDtoEntity.v_Codigo)).ToList();
                        var CuentasAsociadasComas = dbContext.asientocontable.ToList().Where(o => o.i_Eliminado == 0 && o.v_CodigoSituacionFinaciera != null && o.v_CodigoSituacionFinaciera.Contains(pobjDtoEntity.i_IdConfiguracionBalance.ToString()) && o.v_Periodo == periodo).ToList();
                        foreach (var cuentita in CuentasAsociadasComas)
                        {
                            string NuevaCuenta = "";
                            bool Ingreso = false;
                            var CuentasContenidas = cuentita.v_CodigoSituacionFinaciera.Split(',');
                            for (int i = 0; i < CuentasContenidas.Count(); i++)
                            {
                                //if (CuentasContenidas[i] != pobjDtoEntity.v_Codigo)
                                if (CuentasContenidas[i] != pobjDtoEntity.i_IdConfiguracionBalance.ToString())
                                {
                                    NuevaCuenta = !string.IsNullOrEmpty(CuentasContenidas[i]) ? !string.IsNullOrEmpty(NuevaCuenta) ? NuevaCuenta + "," + CuentasContenidas[i] : CuentasContenidas[i] : NuevaCuenta;
                                    Ingreso = true;
                                }
                            }
                            if (Ingreso)
                            {
                                cuentita.v_CodigoSituacionFinaciera = NuevaCuenta;
                                dbContext.asientocontable.ApplyCurrentValues(cuentita);
                                dbContext.SaveChanges();
                            }

                        }

                        //var CuentasAsociadas = dbContext.asientocontable.Where(o => o.v_CodigoSituacionFinaciera == pobjDtoEntity.v_Codigo && o.i_Eliminado == 0 && !o.v_CodigoSituacionFinaciera.Contains(",")).ToList();
                        var CuentasAsociadas = dbContext.asientocontable.ToList().Where(o => o.v_CodigoSituacionFinaciera == pobjDtoEntity.i_IdConfiguracionBalance.ToString() && o.i_Eliminado == 0 && !o.v_CodigoSituacionFinaciera.Contains(",") && o.v_Periodo == periodo).ToList();
                        CuentasAsociadas.ForEach(p => p.v_CodigoSituacionFinaciera = null);
                        CuentasAsociadas.ForEach(p => dbContext.asientocontable.ApplyCurrentValues(p));
                        dbContext.SaveChanges();
                    }
                    else if (TipoBalance == (int)ConfiguracionBalances.EstadodeResultadosNaturaleza)
                    {
                        var CuentasAsociadas = dbContext.asientocontable.Where(o => o.v_CodigoBalanceNaturaleza == pobjDtoEntity.v_Codigo && o.i_Eliminado == 0).ToList();
                        CuentasAsociadas.ForEach(p => p.v_CodigoBalanceNaturaleza = null);
                        CuentasAsociadas.ForEach(p => dbContext.asientocontable.ApplyCurrentValues(p));
                        dbContext.SaveChanges();
                    }

                    #endregion


                    var asientocontable = dbContext.asientocontable.Where(o => o.i_Eliminado == 0 && o.v_Periodo == periodo).ToList();
                    foreach (var item in _TempCuentasAsociadas)
                    {
                        var ListaCuentas = (from a in asientocontable
                                            where a.v_NroCuenta == item
                                            select a).ToList();
                        foreach (asientocontable x in ListaCuentas)
                        {
                            var Entity = (from a in asientocontable
                                          where a.v_NroCuenta == x.v_NroCuenta
                                          select a).FirstOrDefault();
                            var asientoDto = x.ToDTO();
                            asientoDto.t_ActualizaFecha = DateTime.Now;
                            asientoDto.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            if (TipoBalance == (int)ConfiguracionBalances.EstadodeResultadosFuncion)
                            {
                                asientoDto.v_CodigoBalanceFuncion = pobjDtoEntity.v_Codigo;
                            }
                            else if (TipoBalance == (int)ConfiguracionBalances.SituacionFinanciera)
                            {
                                //asientoDto.v_CodigoSituacionFinaciera = !string.IsNullOrEmpty(Entity.v_CodigoSituacionFinaciera) ? Entity.v_CodigoSituacionFinaciera + "," + pobjDtoEntity.v_Codigo : pobjDtoEntity.v_Codigo;
                                asientoDto.v_CodigoSituacionFinaciera = !string.IsNullOrEmpty(Entity.v_CodigoSituacionFinaciera) ? Entity.v_CodigoSituacionFinaciera + "," + pobjDtoEntity.i_IdConfiguracionBalance.ToString() : pobjDtoEntity.i_IdConfiguracionBalance.ToString();

                            }
                            else if (TipoBalance == (int)ConfiguracionBalances.EstadodeResultadosNaturaleza)
                            {
                                asientoDto.v_CodigoBalanceNaturaleza = pobjDtoEntity.v_Codigo;
                            }
                            asientocontable lc = asientocontableAssembler.ToEntity(asientoDto);
                            dbContext.asientocontable.ApplyCurrentValues(lc);

                        }


                    }
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                    Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "configuracionbalances", objEntitySource.i_IdConfiguracionBalance.ToString());
                    ts.Complete();
                    return objEntitySource.i_IdConfiguracionBalance;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AsientosContablesBL.ConfiguracionBalanceActualizar()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return 0;
            }
        }

        public void ConfiguracionBalanceEliminar(ref OperationResult pobjOperationResult, int IdConfiguracionBalances, List<string> ClientSession, int TipoBalance)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                var objEntitySource = (from a in dbContext.configuracionbalances
                                       where a.i_IdConfiguracionBalance == IdConfiguracionBalances
                                       select a).FirstOrDefault();

                //objEntitySource.i_IsDeletE = 1;

                objEntitySource.i_Eliminado = 1;

                objEntitySource.t_ActualizaFecha = DateTime.Now;
                objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                if (TipoBalance == (int)ConfiguracionBalances.EstadodeResultadosFuncion)
                {

                    #region pone en vacio todas las cuentas que se hayan asociado antes a este codigo

                    var CuentasAsociadas = dbContext.asientocontable.Where(o => o.v_CodigoBalanceFuncion == objEntitySource.v_Codigo && o.i_Eliminado == 0).ToList();

                    CuentasAsociadas.ForEach(p => p.v_CodigoBalanceFuncion = null);
                    CuentasAsociadas.ForEach(p => dbContext.asientocontable.ApplyCurrentValues(p));
                    dbContext.SaveChanges();

                    #endregion
                }
                else if (TipoBalance == (int)ConfiguracionBalances.SituacionFinanciera)
                {
                    #region pone en vacio todas las cuentas que se hayan asociado antes a este codigo

                    var CuentasAsociadas = dbContext.asientocontable.Where(o => o.v_CodigoSituacionFinaciera == objEntitySource.v_Codigo && o.i_Eliminado == 0).ToList();
                    CuentasAsociadas.ForEach(p => p.v_CodigoSituacionFinaciera = null);
                    CuentasAsociadas.ForEach(p => dbContext.asientocontable.ApplyCurrentValues(p));
                    dbContext.SaveChanges();
                    var CuentasAsociadasComas = dbContext.asientocontable.Where(o => o.i_Eliminado == 0 && o.v_CodigoSituacionFinaciera != null && o.v_CodigoSituacionFinaciera.Contains(",")).ToList();
                    foreach (var cuentita in CuentasAsociadasComas)
                    {
                        string NuevaCuenta = "";
                        bool Ingreso = false;
                        var CuentasContenidas = cuentita.v_CodigoSituacionFinaciera.Split(',');
                        for (int i = 0; i < CuentasContenidas.Count(); i++)
                        {
                            //if (CuentasContenidas[i] != objEntitySource.v_Codigo)
                            if (CuentasContenidas[i] != objEntitySource.i_IdConfiguracionBalance.ToString())
                            {
                                NuevaCuenta = !string.IsNullOrEmpty(NuevaCuenta) ? NuevaCuenta + "," + CuentasContenidas[i] : CuentasContenidas[i];
                                Ingreso = true;
                            }
                        }
                        if (Ingreso)
                        {
                            cuentita.v_CodigoSituacionFinaciera = NuevaCuenta;
                            dbContext.asientocontable.ApplyCurrentValues(cuentita);
                            dbContext.SaveChanges();
                        }

                    }
                    #endregion
                }
                else if (TipoBalance == (int)ConfiguracionBalances.EstadodeResultadosNaturaleza)
                {
                    #region pone en vacio todas las cuentas que se hayan asociado antes a este codigo

                    var CuentasAsociadas = dbContext.asientocontable.Where(o => o.v_CodigoBalanceNaturaleza == objEntitySource.v_Codigo && o.i_Eliminado == 0).ToList();

                    CuentasAsociadas.ForEach(p => p.v_CodigoBalanceNaturaleza = null);
                    CuentasAsociadas.ForEach(p => dbContext.asientocontable.ApplyCurrentValues(p));
                    dbContext.SaveChanges();

                    #endregion

                }
                dbContext.SaveChanges();
                Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "configuracionbalances", objEntitySource.i_IdConfiguracionBalance.ToString());
                pobjOperationResult.Success = 1;

                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AsientosContablesBL.ConfiguracionBalanceEliminar()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }



        public void CopiarBalances(ref OperationResult pobjOperationResult, string pstrPeriodoOrigen,
           string pstrMesOrigen, string pstrPeriodoDestino, string pstrMesDestino, List<string> clientSession)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {

                        var cb = dbContext.configuracionbalances.ToList().Where(p => p.v_Periodo == pstrPeriodoDestino && p.v_Mes == pstrMesDestino && p.i_Eliminado == 0 && p.v_TipoBalance == ((int)ConfiguracionBalances.SituacionFinanciera).ToString()).ToList();
                        cb.ForEach(p => p.i_Eliminado = 1);
                        cb.ForEach(p => dbContext.configuracionbalances.ApplyCurrentValues(p));
                        dbContext.SaveChanges();

                        var configuracionbalancesACopiar =
                            dbContext.configuracionbalances.ToList().Where(p => p.v_Periodo == pstrPeriodoOrigen && p.v_Mes == pstrMesOrigen && p.i_Eliminado == 0 && p.v_TipoBalance == ((int)ConfiguracionBalances.SituacionFinanciera).ToString()).ToList()
                                .Select(o => new configuracionbalances
                                {
                                    i_IdConfiguracionBalance = o.i_IdConfiguracionBalance,
                                    v_TipoBalance = o.v_TipoBalance,
                                    v_Codigo = o.v_Codigo,
                                    v_Nombre = o.v_Nombre,
                                    v_NombreGrupo = o.v_NombreGrupo,
                                    i_TipoNota = o.i_TipoNota,
                                    t_InsertaFecha = DateTime.Now,
                                    i_Eliminado = o.i_Eliminado,
                                    i_InsertaIdUsuario = Globals.ClientSession.i_SystemUserId,
                                    v_Mes = pstrMesDestino,
                                    v_Periodo = pstrPeriodoDestino,

                                }).OrderBy(o => o.v_Codigo).ToList();
                        var configuracionbalancesACopiar2 =
                             dbContext.configuracionbalances.ToList().Where(p => p.v_Periodo == pstrPeriodoOrigen && p.v_Mes == pstrMesOrigen && p.i_Eliminado == 0 && p.v_TipoBalance == ((int)ConfiguracionBalances.SituacionFinanciera).ToString()).ToList()
                                 .Select(o => new configuracionbalances
                                 {
                                     i_IdConfiguracionBalance = o.i_IdConfiguracionBalance,
                                     v_TipoBalance = o.v_TipoBalance,
                                     v_Codigo = o.v_Codigo,
                                     v_Nombre = o.v_Nombre,
                                     v_NombreGrupo = o.v_NombreGrupo,
                                     i_TipoNota = o.i_TipoNota,
                                     t_InsertaFecha = DateTime.Now,
                                     i_Eliminado = o.i_Eliminado,
                                     i_InsertaIdUsuario = Globals.ClientSession.i_SystemUserId,
                                     v_Mes = pstrMesDestino,
                                     v_Periodo = pstrPeriodoDestino,

                                 }).OrderBy(o => o.v_Codigo).ToList();

                        configuracionbalancesACopiar.ToList().ForEach(p => dbContext.configuracionbalances.AddObject(p));

                        foreach (var configCopiar in configuracionbalancesACopiar2)
                        {
                            var CuentasAsociadasComas = dbContext.asientocontable.ToList().Where(o => o.i_Eliminado == 0 && o.v_CodigoSituacionFinaciera != null && o.v_CodigoSituacionFinaciera.Contains(configCopiar.i_IdConfiguracionBalance.ToString())).ToList();
                            foreach (var cuentita in CuentasAsociadasComas)
                            {
                                bool Ingreso = false;
                                var CuentasContenidas = cuentita.v_CodigoSituacionFinaciera.Split(',');
                                for (int i = 0; i < CuentasContenidas.Count(); i++)
                                {
                                    if (CuentasContenidas[i] == configCopiar.i_IdConfiguracionBalance.ToString())
                                    {
                                        //NuevaCuenta = !string.IsNullOrEmpty(CuentasContenidas[i]) ? !string.IsNullOrEmpty(NuevaCuenta) ? NuevaCuenta + "," + CuentasContenidas[i] : CuentasContenidas[i] : NuevaCuenta;
                                        Ingreso = true;
                                    }

                                }
                                if (Ingreso)
                                {
                                    var codigo = configuracionbalancesACopiar.Where(o => o.v_Codigo == configCopiar.v_Codigo).FirstOrDefault();
                                    if (codigo != null)
                                    {
                                        cuentita.v_CodigoSituacionFinaciera = cuentita.v_CodigoSituacionFinaciera + "," + codigo.i_IdConfiguracionBalance.ToString();
                                        dbContext.asientocontable.ApplyCurrentValues(cuentita);
                                        dbContext.SaveChanges();
                                    }
                                }

                            }

                            //var CuentasAsociadas = dbContext.asientocontable.Where(o => o.v_CodigoSituacionFinaciera == pobjDtoEntity.v_Codigo && o.i_Eliminado == 0 && !o.v_CodigoSituacionFinaciera.Contains(",")).ToList();
                            var CuentasAsociadas = dbContext.asientocontable.ToList().Where(o => o.v_CodigoSituacionFinaciera != null && o.v_CodigoSituacionFinaciera == configCopiar.i_IdConfiguracionBalance.ToString() && o.i_Eliminado == 0 && !o.v_CodigoSituacionFinaciera.Contains(",")).ToList();
                            CuentasAsociadas.ForEach(p => p.v_CodigoSituacionFinaciera = null);
                            CuentasAsociadas.ForEach(p => dbContext.asientocontable.ApplyCurrentValues(p));
                            dbContext.SaveChanges();


                        }


                        dbContext.SaveChanges();
                        ts.Complete();
                        pobjOperationResult.Success = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AsientoBL.CopiarBalances()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }



        #endregion
    }
}
