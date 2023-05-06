using System;
using System.Collections.Generic;
using System.Linq;
using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;

namespace SAMBHS.Planilla.BL
{
    public static class SemanasLaborablesBl
    {
        /// <summary>
        /// Obtiene la semana en la que se encuentra el presente día.
        /// Si la semana no existe, arroja valor 0.
        /// </summary>
        public static int SemanaActual
        {
            get
            {
                try
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var hoy = DateTime.Today;
                        var periodo = Globals.ClientSession.i_Periodo ?? DateTime.Now.Year;
                        var semanas = dbContext.planillasemanasperiodo.Where(p => p.t_FechaInicio.Value.Year == periodo).ToList();
                        var semanaActual = semanas.FirstOrDefault(p => p.t_FechaInicio.HasValue && p.t_FechaFin.HasValue && 
                            hoy >= p.t_FechaInicio.Value && hoy <= p.t_FechaFin.Value);
                        return semanaActual != null ? semanaActual.i_IdSemana : 0;
                    }
                }
                catch
                {
                    return 0;
                }
            }
        }

        public static planillasemanasperiodoDto ObtenerSemana(ref OperationResult pobjOperationResult, int id)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var data = dbContext.planillasemanasperiodo.FirstOrDefault(p => p.i_IdSemana == id);
                    pobjOperationResult.Success = 1;
                    return data != null ? data.ToDTO() : null;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "SemanasLaborablesBl.ObtenerSemana()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public static void InsertarSemana(ref OperationResult pobjOperationResult,
            planillasemanasperiodoDto objEntity)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var periodo = objEntity.t_FechaInicio.Value.Year;

                    if (!dbContext.planillasemanasperiodo.Any(p => p.i_NroSemana == objEntity.i_NroSemana
                                                                   && p.t_FechaInicio.Value.Year == periodo))
                    {
                        var entity = objEntity.ToEntity();
                        dbContext.planillasemanasperiodo.AddObject(entity);
                        dbContext.SaveChanges();
                        pobjOperationResult.Success = 1;
                    }
                    else
                        throw new Exception("Ya hay una un registro para esta semana en este periodo.");
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "SemanasLaborablesBl.InsertarSemana()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static void ActualizarSemana(ref OperationResult pobjOperationResult,
            planillasemanasperiodoDto objEntity)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entity =
                        dbContext.planillasemanasperiodo.FirstOrDefault(
                            p => p.i_IdSemana == objEntity.i_IdSemana);

                    if (entity == null) throw new Exception("El elemento ya no existe!");
                    entity = objEntity.ToEntity();
                    dbContext.planillasemanasperiodo.ApplyCurrentValues(entity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "SemanasLaborablesBl.ActualizarSemana()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static void EliminarSemana(ref OperationResult pobjOperationResult, int id)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entity = dbContext.planillasemanasperiodo.FirstOrDefault(p => p.i_IdSemana == id);

                    if (entity == null) throw new Exception("El elemento ya no existe!");

                    dbContext.planillasemanasperiodo.DeleteObject(entity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "SemanasLaborablesBl.EliminarSemana()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static List<planillasemanasperiodoDto> ObtenerListadoSemanas(ref OperationResult pobjOperationResult,
            int periodo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var listado = dbContext.planillasemanasperiodo.Where(p => p.t_FechaInicio.Value.Year == periodo).ToDTOs();
                    pobjOperationResult.Success = 1;
                    return listado.OrderBy(o => o.i_NroSemana).ToList();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "SemanasLaborablesBl.ObtenerListadoSemanas()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }
    }
}