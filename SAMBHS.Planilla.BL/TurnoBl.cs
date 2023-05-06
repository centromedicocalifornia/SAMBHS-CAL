using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;


namespace SAMBHS.Planilla.BL
{
    public class TurnoBl
    {
        public static planillaturnosDto PrepararNuevoTurno(ref OperationResult pobjOperationResult)
        {
            try
            {
                pobjOperationResult.Success = 1;
                var result = new planillaturnosDto
                {
                    Detalle = new List<planillaturnosdetalleDto>
                    {
                        new planillaturnosdetalleDto{ i_Asiste = 1, Asistencia = true, i_IdDia = (int)DayOfWeek.Monday, t_IngresoI = DateTime.MinValue, t_SalidaI = DateTime.MinValue, t_IngresoII = DateTime.MinValue,t_SalidaII = DateTime.MinValue},
                        new planillaturnosdetalleDto{ i_Asiste = 1, Asistencia = true, i_IdDia = (int)DayOfWeek.Tuesday, t_IngresoI = DateTime.MinValue, t_SalidaI = DateTime.MinValue, t_IngresoII = DateTime.MinValue,t_SalidaII = DateTime.MinValue},
                        new planillaturnosdetalleDto{ i_Asiste = 1, Asistencia = true, i_IdDia = (int)DayOfWeek.Wednesday, t_IngresoI = DateTime.MinValue, t_SalidaI = DateTime.MinValue, t_IngresoII = DateTime.MinValue,t_SalidaII = DateTime.MinValue},
                        new planillaturnosdetalleDto{ i_Asiste = 1, Asistencia = true, i_IdDia = (int)DayOfWeek.Thursday, t_IngresoI = DateTime.MinValue, t_SalidaI = DateTime.MinValue, t_IngresoII = DateTime.MinValue,t_SalidaII = DateTime.MinValue},
                        new planillaturnosdetalleDto{ i_Asiste = 1, Asistencia = true, i_IdDia = (int)DayOfWeek.Friday, t_IngresoI = DateTime.MinValue, t_SalidaI = DateTime.MinValue, t_IngresoII = DateTime.MinValue,t_SalidaII = DateTime.MinValue},
                        new planillaturnosdetalleDto{ i_Asiste = 1, Asistencia = true, i_IdDia = (int)DayOfWeek.Saturday, t_IngresoI = DateTime.MinValue, t_SalidaI = DateTime.MinValue, t_IngresoII = DateTime.MinValue,t_SalidaII = DateTime.MinValue},
                        new planillaturnosdetalleDto{ i_Asiste = 1, Asistencia = true, i_IdDia = (int)DayOfWeek.Sunday, t_IngresoI = DateTime.MinValue, t_SalidaI = DateTime.MinValue, t_IngresoII = DateTime.MinValue,t_SalidaII = DateTime.MinValue},
                    }
                };
                return result;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "TurnoBl.PrepararNuevoTurno()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public static void InsertarTurno(ref OperationResult pobjOperationResult, planillaturnosDto pobjEntity)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    if (pobjEntity == null || !pobjEntity.Detalle.Any()) throw new Exception("La entidad no tiene el formato correcto");
                    var entity = pobjEntity.ToEntity();
                    dbContext.planillaturnos.AddObject(entity);

                    foreach (var detalle in pobjEntity.Detalle)
                    {
                        detalle.i_Asiste = detalle.Asistencia ? 1 : 0;
                        detalle.i_IdTurno = entity.i_IdTurno;
                        dbContext.planillaturnosdetalle.AddObject(detalle.ToEntity());
                    }

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "TurnoBl.InsertarTurno()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static void ActualizarTurno(ref OperationResult pobjOperationResult, planillaturnosDto pobjEntity)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var turno = dbContext.planillaturnos.FirstOrDefault(p => p.i_IdTurno == pobjEntity.i_IdTurno);
                    if (turno == null) throw new Exception("La entidad no tiene el formato correcto");
                    turno = pobjEntity.ToEntity();
                    dbContext.planillaturnos.ApplyCurrentValues(turno);

                    foreach (var detalle in pobjEntity.Detalle)
                    {
                        var detEntity = dbContext.planillaturnosdetalle.FirstOrDefault(p => p.i_IdTurnoDetalle == detalle.i_IdTurnoDetalle);
                        if (detEntity == null) continue;
                        detEntity.i_Asiste = detalle.Asistencia ? 1 : 0;
                        detEntity.t_IngresoI = detalle.t_IngresoI;
                        detEntity.t_IngresoII = detalle.t_IngresoII;
                        detEntity.t_SalidaI = detalle.t_SalidaI;
                        detEntity.t_SalidaII = detalle.t_SalidaII;
                        dbContext.planillaturnosdetalle.ApplyCurrentValues(detEntity);
                    }

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "TurnoBl.ActualizarTurno()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static void EliminarTurno(ref OperationResult pobjOperationResult, int idTurno)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    //TODO: Validar si algun tiene este turno asignado primero.
                    var turno = dbContext.planillaturnos.FirstOrDefault(p => p.i_IdTurno == idTurno);
                    if (turno == null) return;
                    foreach (var detalle in dbContext.planillaturnosdetalle.Where(p => p.i_IdTurno == idTurno).ToList())
                    {
                        if (detalle == null) continue;
                        dbContext.planillaturnosdetalle.DeleteObject(detalle);
                    }

                    dbContext.planillaturnos.DeleteObject(turno);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "TurnoBl.EliminarTurno()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static planillaturnosDto ObtenerTurnoPorId(ref OperationResult pobjOperationResult, int idTurno)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var turno = dbContext.planillaturnos.FirstOrDefault(p => p.i_IdTurno == idTurno);
                    if (turno == null) throw new Exception("La entidad no existe.!");
                    var result = turno.ToDTO();
                    result.Detalle = turno.planillaturnosdetalle.OrderBy(o => o.i_IdTurnoDetalle).ToDTOs();
                    result.Detalle.ForEach(t => t.Asistencia = (t.i_Asiste ?? 0) == 1);
                    pobjOperationResult.Success = 1;
                    return result;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "TurnoBl.ObtenerTurnoPorId()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public static List<planillaturnosDto> ObtenerListadoTurnos(ref OperationResult pobjOperationResult)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    pobjOperationResult.Success = 1;
                    return dbContext.planillaturnos.ToDTOs();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "TurnoBl.ObtenerListadoTurnos()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public static List<GridKeyValueDTO> ObtenerListadoTurnosParaCombo(ref OperationResult pobjOperationResult)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    pobjOperationResult.Success = 1;
                    return dbContext.planillaturnos.ToList().Select(p => new GridKeyValueDTO
                    {
                        Id = p.i_IdTurno.ToString(),
                        Value1 = p.v_Descripcion
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "TurnoBl.ObtenerListadoTurnos()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }
    }
}
