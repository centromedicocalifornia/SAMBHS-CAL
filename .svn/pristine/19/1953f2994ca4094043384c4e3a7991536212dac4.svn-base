using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;

namespace SAMBHS.Planilla.BL
{
    public static class ConceptosBl
    {
        public static string ObtenerCodigoConcepto(int i)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var sufijo = i == 1 ? "I" : i == 2 ? "D" : "A";
                    var conceptos = dbContext.planillaconceptos.Where(p => p.i_Eliminado == 0 && p.i_IdTipoConcepto == i)
                                                               .OrderBy(o => o.v_Codigo).ToList();

                    if (conceptos.Any())
                    {
                        var max = conceptos.LastOrDefault();
                        if (max != null)
                        {
                            var c = int.Parse(max.v_Codigo.Substring(1, 3));
                            return string.Format("{0}{1:000}", sufijo, c + 1);
                        }
                    }

                    return string.Format("{0}{1:000}", sufijo, 1);
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static List<planillaconceptosDto> ListarConceptos(ref OperationResult pobjOperationResult,
            int idTipoPlanilla, int idTipoConcepto)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var conceptos = dbContext.planillaconceptos.Where(p => p.i_IdTipoPlanilla == idTipoPlanilla 
                        && p.i_Eliminado == 0 && p.i_IdTipoConcepto == idTipoConcepto).ToList();

                    pobjOperationResult.Success = 1;
                    return conceptos.OrderBy(o => o.v_Codigo).ToDTOs();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ConceptosBl.ListarConceptos()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public static planillaconceptosDto ObtenerConcepto(ref OperationResult pobjOperationResult, string idConcepto)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var concepto = dbContext.planillaconceptos.FirstOrDefault(p => p.v_IdConceptoPlanilla.Equals(idConcepto));
                    pobjOperationResult.Success = 1;
                    if (concepto != null)
                        return concepto.ToDTO();

                    return null;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ConceptosBl.ObtenerConcepto()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public static void ActualizarConcepto(ref OperationResult pobjOperationResult, planillaconceptosDto entityDto)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var session = Globals.ClientSession.GetAsList();
                    if (string.IsNullOrWhiteSpace(entityDto.v_IdConceptoPlanilla))
                    {
                        var objSecuentialBl = new SecuentialBL();
                        var objEntity = entityDto.ToEntity();
                        objEntity.t_InsertaFecha = DateTime.Now;
                        objEntity.i_InsertaIdUsuario = int.Parse(session[2]);
                        objEntity.i_Eliminado = 0;
                        var intNodeId = int.Parse(session[0]);
                        var secuentialId = objSecuentialBl.GetNextSecuentialId(intNodeId, 85);
                        var newId = Utils.GetNewId(int.Parse(session[0]), secuentialId, "PN");
                        objEntity.v_IdConceptoPlanilla = newId;
                        dbContext.planillaconceptos.AddObject(objEntity);
                    }
                    else
                    {
                        var concepto = dbContext.planillaconceptos.FirstOrDefault(p => p.v_IdConceptoPlanilla.Equals(entityDto.v_IdConceptoPlanilla));
                        if (concepto != null)
                        {
                            concepto = entityDto.ToEntity();
                            concepto.t_ActualizaFecha = DateTime.Now;
                            concepto.i_ActualizaIdUsuario = int.Parse(session[2]);
                            dbContext.planillaconceptos.ApplyCurrentValues(concepto);
                        }
                    }

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ConceptosBl.ActualizarConcepto()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }
    }
}
