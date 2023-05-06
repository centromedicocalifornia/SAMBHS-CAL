using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;

namespace SAMBHS.Planilla.BL
{
    public static class DiasNoLaborablesBl
    {
        public static planilladiasnolaborablesDto ObtenerDiaNoLaborable(ref OperationResult pobjOperationResult, int id)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var data = dbContext.planilladiasnolaborables.FirstOrDefault(p => p.i_IdDiaNoLaborable == id);
                    pobjOperationResult.Success = 1;
                    return data != null ? data.ToDTO() : null;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ControlAsistenciaBl.ObtenerDiaNoLaborable()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public static void InsertarDiaNoLaborable(ref OperationResult pobjOperationResult,
            planilladiasnolaborablesDto objEntity)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    if (!dbContext.planilladiasnolaborables.Any(p => p.t_DiaNoLaborable == objEntity.t_DiaNoLaborable))
                    {
                        var entity = objEntity.ToEntity();
                        dbContext.planilladiasnolaborables.AddObject(entity);
                        dbContext.SaveChanges();
                        pobjOperationResult.Success = 1;
                    }
                    else
                        throw new Exception("Ya hay una un registro con esta fecha para este periodo.");
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ControlAsistenciaBl.InsertarDiaNoLaborable()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static void ActualizarDiaNoLaborable(ref OperationResult pobjOperationResult,
            planilladiasnolaborablesDto objEntity)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entity =
                        dbContext.planilladiasnolaborables.FirstOrDefault(
                        p => p.i_IdDiaNoLaborable == objEntity.i_IdDiaNoLaborable);

                    if (entity == null) throw new Exception("El elemento ya no existe!");
                    entity = objEntity.ToEntity();
                    dbContext.planilladiasnolaborables.ApplyCurrentValues(entity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ControlAsistenciaBl.ActualizarDiaNoLaborable()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static void EliminarDiaNoLaborable(ref OperationResult pobjOperationResult, int id)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entity = dbContext.planilladiasnolaborables.FirstOrDefault(p => p.i_IdDiaNoLaborable == id);

                    if (entity == null) throw new Exception("El elemento ya no existe!");

                    dbContext.planilladiasnolaborables.DeleteObject(entity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ControlAsistenciaBl.EliminarDiaNoLaborable()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static List<planilladiasnolaborablesDto> ObtenerListadoDiasNoLaborables(ref OperationResult pobjOperationResult,
            int periodo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var listado = dbContext.planilladiasnolaborables.Where(p => p.t_DiaNoLaborable.Value.Year == periodo).ToDTOs();
                    pobjOperationResult.Success = 1;
                    return listado.OrderBy(o => o.t_DiaNoLaborable).ToList();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ControlAsistenciaBl.ObtenerListadoDiasNoLaborables()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }
    }
}
