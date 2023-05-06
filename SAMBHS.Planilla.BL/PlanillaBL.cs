using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using System.Linq.Dynamic;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using System.ComponentModel;
using System.Globalization;
using System.Data;



namespace SAMBHS.Planilla.BL
{
    public class PlanillaBL
    {
        

        public string Error;
        public List<planillaconceptosDto> ObtenerListaPlanillaConceptos(ref OperationResult objOperationResult, string Filtro)
        {
            try
            {

                List<planillaconceptosDto> ListaConceptos = new List<planillaconceptosDto>();
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    ListaConceptos = (from planilla in dbContext.planillaconceptos

                                      join TpIngresos in dbContext.datahierarchy on new { Grupo = 112, eliminado = 0, t = planilla.i_IdTipoPlanilla.Value } equals new { Grupo = TpIngresos.i_GroupId, eliminado = TpIngresos.i_IsDeleted.Value, t = TpIngresos.i_ItemId } into TpIngresos_join

                                      from TpIngresos in TpIngresos_join.DefaultIfEmpty()
                                      where planilla.i_Eliminado == 0 && (planilla.v_IdConceptoPlanilla == Filtro || Filtro == "")

                                      select new planillaconceptosDto
                                      {
                                          v_IdConceptoPlanilla = planilla.v_IdConceptoPlanilla,
                                          i_IdTipoConcepto = planilla.i_IdTipoConcepto,
                                          v_Codigo = planilla.v_Codigo,
                                          TipoPlanilla = TpIngresos.v_Value1,
                                          i_IdTipoPlanilla = planilla.i_IdTipoPlanilla,
                                          v_Nombre = planilla.v_Nombre,
                                          i_ConsiderarVacaciones = planilla.i_ConsiderarVacaciones ?? 0,
                                          i_DescontarEsSalud = planilla.i_DescontarEsSalud ?? 0,
                                          i_DescontarSCTR = planilla.i_DescontarSCTR ?? 0,
                                          i_DescontarSCTRPens = planilla.i_DescontarSCTRPens ?? 0,
                                          v_ColumnaAfectaciones = planilla.v_ColumnaAfectaciones,
                                          v_ColumnaPorcentaje = planilla.v_ColumnaPorcentaje

                                      }
                                 ).ToList();
                    //ListaConceptos = query.ToDTOs();

                }
                objOperationResult.Success = 1;
                return ListaConceptos;
            }
            catch
            {
                objOperationResult.Success = 0;
                return null;
            }
        }

        public string InsertarConceptoPlanillas(ref OperationResult pobjOperationResult, planillaconceptosDto pobjDtoEntity, List<string> ClientSession)
        {
            string newId = string.Empty;

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    SecuentialBL objSecuentialBL = new SecuentialBL();
                    planillaconceptos objEntity = planillaconceptosAssembler.ToEntity(pobjDtoEntity);
                    objEntity.t_InsertaFecha = DateTime.Now;
                    objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntity.i_Eliminado = 0;
                    // Autogeneramos el Pk de la tabla
                    int intNodeId = int.Parse(ClientSession[0]);
                    var SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 85);
                    newId = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "PN");
                    objEntity.v_IdConceptoPlanilla = newId;
                    dbContext.AddToplanillaconceptos(objEntity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                    Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "planillaconceptos", newId);
                    return newId;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaBL.InsertarConceptoPlanillas()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return newId;
            }
        }

        public string ActualizarConceptoPlanillas(ref OperationResult pobjOperationResult, planillaconceptosDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.planillaconceptos
                                           where a.v_IdConceptoPlanilla == pobjDtoEntity.v_IdConceptoPlanilla
                                           select a).FirstOrDefault();

                    // Crear la entidad con los datos actualizados
                    pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                    pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                    planillaconceptos objEntity = planillaconceptosAssembler.ToEntity(pobjDtoEntity);

                    dbContext.planillaconceptos.ApplyCurrentValues(objEntity);

                    // Guardar los cambios
                    dbContext.SaveChanges();
                    Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "planilalaconceptos", objEntitySource.v_IdConceptoPlanilla);
                    pobjOperationResult.Success = 1;
                    return pobjDtoEntity.v_IdConceptoPlanilla;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaBL.ActualizarConceptoPlanillas()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public planillaconceptosDto ObtenerPlanillaConceptos(ref OperationResult objOperationResult, string Filtro, string Codigo)
        {
            try
            {
                List<systemparameterDto> JoinTemporal = new List<systemparameterDto>();
                using (SAMBHSEntitiesModel dbContextIntegrado = new SAMBHSEntitiesModel())
                {
                    JoinTemporal = dbContextIntegrado.systemparameter.Where(p => p.i_GroupId == 152 && p.i_IsDeleted == 0).ToList().ToDTOs();
                }
                planillaconceptosDto PlanillaConceptos = new planillaconceptosDto();
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {


                    PlanillaConceptos = (from planilla in dbContext.planillaconceptos

                                         join TpIngresos in dbContext.datahierarchy on new { Grupo = 112, eliminado = 0, t = planilla.i_IdTipoPlanilla.Value } equals new { Grupo = TpIngresos.i_GroupId, eliminado = TpIngresos.i_IsDeleted.Value, t = TpIngresos.i_ItemId } into TpIngresos_join

                                         from TpIngresos in TpIngresos_join.DefaultIfEmpty()

                                         where planilla.i_Eliminado == 0 && (planilla.v_IdConceptoPlanilla == Filtro || Filtro == "")

                                         && (planilla.v_Codigo == Codigo || Codigo == "")

                                         select new planillaconceptosDto
                                         {


                                             v_IdConceptoPlanilla = planilla.v_IdConceptoPlanilla,
                                             i_IdTipoConcepto = planilla.i_IdTipoConcepto,
                                             v_Codigo = planilla.v_Codigo,
                                             TipoPlanilla = TpIngresos.v_Value1,
                                             i_IdTipoPlanilla = planilla.i_IdTipoPlanilla,
                                             v_Nombre = planilla.v_Nombre,
                                             i_ConsiderarVacaciones = planilla.i_ConsiderarVacaciones == null ? 0 : planilla.i_ConsiderarVacaciones,
                                             i_DescontarEsSalud = planilla.i_DescontarEsSalud == null ? 0 : planilla.i_DescontarEsSalud,
                                             i_DescontarSCTR = planilla.i_DescontarSCTR == null ? 0 : planilla.i_DescontarSCTR,
                                             i_DescontarSCTRPens = planilla.i_DescontarSCTRPens == null ? 0 : planilla.i_DescontarSCTRPens,
                                             i_Eliminado = planilla.i_Eliminado,
                                             i_InsertaIdUsuario = planilla.i_InsertaIdUsuario,
                                             t_InsertaFecha = planilla.t_InsertaFecha,
                                             i_ActualizaIdUsuario = planilla.i_ActualizaIdUsuario,
                                             t_ActualizaFecha = planilla.t_ActualizaFecha,
                                             i_IdTipoConceptoPlanilla = planilla.i_IdTipoConceptoPlanilla,
                                             CodigoConceptoPlanilla = "",
                                             v_ColumnaAfectaciones = planilla.v_ColumnaAfectaciones,
                                             v_ColumnaPorcentaje = planilla.v_ColumnaPorcentaje

                                         }).FirstOrDefault();

                    if (PlanillaConceptos.i_IdTipoConceptoPlanilla != null)
                    {
                        var CodigoConceptoPlanilla = JoinTemporal.Where(x => x.i_ParameterId == PlanillaConceptos.i_IdTipoConceptoPlanilla.Value).Select(x => x.v_Value2).FirstOrDefault();
                        PlanillaConceptos.CodigoConceptoPlanilla = CodigoConceptoPlanilla != null ? CodigoConceptoPlanilla : "";
                    }

                    //ListaConceptos = query.ToDTOs();

                    //var queryFinal=( from n in PlanillaConceptos

                    //                 select new planillaconceptosDto()
                    //                 {


                    //                 }).
                }
                objOperationResult.Success = 1;
                return PlanillaConceptos;
            }
            catch
            {
                objOperationResult.Success = 0;
                return null;
            }
        }

        public planillaconceptosDto ObtenerPlanillaConceptosBusqueda(ref OperationResult objOperationResult, string Filtro, string Codigo)
        {
            try
            {
                List<systemparameterDto> JoinTemporal = new List<systemparameterDto>();
                using (SAMBHSEntitiesModel dbContextIntegrado = new SAMBHSEntitiesModel())
                {
                    JoinTemporal = dbContextIntegrado.systemparameter.Where(p => p.i_GroupId == 152 && p.i_IsDeleted == 0).ToList().ToDTOs();
                }
                planillaconceptosDto PlanillaConceptos = new planillaconceptosDto();
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {


                    PlanillaConceptos = (from planilla in dbContext.planillaconceptos

                                         join TpIngresos in dbContext.datahierarchy on new { Grupo = 112, eliminado = 0, t = planilla.i_IdTipoPlanilla.Value } equals new { Grupo = TpIngresos.i_GroupId, eliminado = TpIngresos.i_IsDeleted.Value, t = TpIngresos.i_ItemId } into TpIngresos_join

                                         from TpIngresos in TpIngresos_join.DefaultIfEmpty()

                                         where planilla.i_Eliminado == 0 && (planilla.v_IdConceptoPlanilla == Filtro || Filtro == "")

                                         && (planilla.v_Codigo == Codigo)

                                         select new planillaconceptosDto
                                         {


                                             v_IdConceptoPlanilla = planilla.v_IdConceptoPlanilla,
                                             i_IdTipoConcepto = planilla.i_IdTipoConcepto,
                                             v_Codigo = planilla.v_Codigo,
                                             TipoPlanilla = TpIngresos.v_Value1,
                                             i_IdTipoPlanilla = planilla.i_IdTipoPlanilla,
                                             v_Nombre = planilla.v_Nombre,
                                             i_ConsiderarVacaciones = planilla.i_ConsiderarVacaciones == null ? 0 : planilla.i_ConsiderarVacaciones,
                                             i_DescontarEsSalud = planilla.i_DescontarEsSalud == null ? 0 : planilla.i_DescontarEsSalud,
                                             i_DescontarSCTR = planilla.i_DescontarSCTR == null ? 0 : planilla.i_DescontarSCTR,
                                             i_DescontarSCTRPens = planilla.i_DescontarSCTRPens == null ? 0 : planilla.i_DescontarSCTRPens,
                                             i_Eliminado = planilla.i_Eliminado,
                                             i_InsertaIdUsuario = planilla.i_InsertaIdUsuario,
                                             t_InsertaFecha = planilla.t_InsertaFecha,
                                             i_ActualizaIdUsuario = planilla.i_ActualizaIdUsuario,
                                             t_ActualizaFecha = planilla.t_ActualizaFecha,
                                             i_IdTipoConceptoPlanilla = planilla.i_IdTipoConceptoPlanilla,
                                             CodigoConceptoPlanilla = "",


                                         }).FirstOrDefault();

                    if (PlanillaConceptos != null && PlanillaConceptos.i_IdTipoConceptoPlanilla != null)
                    {
                        var CodigoConceptoPlanilla = JoinTemporal.Where(x => x.i_ParameterId == PlanillaConceptos.i_IdTipoConceptoPlanilla.Value).Select(x => x.v_Value2).FirstOrDefault();
                        PlanillaConceptos.CodigoConceptoPlanilla = CodigoConceptoPlanilla != null ? CodigoConceptoPlanilla : "";
                    }

                    //ListaConceptos = query.ToDTOs();

                    //var queryFinal=( from n in PlanillaConceptos

                    //                 select new planillaconceptosDto()
                    //                 {


                    //                 }).
                }
                objOperationResult.Success = 1;
                return PlanillaConceptos;
            }
            catch
            {
                objOperationResult.Success = 0;
                return null;
            }
        }


        public void EliminarPlanillaConceptos(ref OperationResult pobjOperationResult, string IdConceptoPlanilla, List<string> ClientSession)
        {

            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                var objEntitySource = (from a in dbContext.planillaconceptos
                                       where a.v_IdConceptoPlanilla == IdConceptoPlanilla
                                       select a).FirstOrDefault();
                objEntitySource.i_Eliminado = 1;
                dbContext.SaveChanges();
                pobjOperationResult.Success = 1;
                Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "planillaconceptos", objEntitySource.v_IdConceptoPlanilla.ToString());
                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaBL.EliminarPlanillaConceptos()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }
        public planillaconceptosadministracionDto ObtenerPlanillaConcepto(string IdConceptoPlanilla)
        {
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                var pcd = (from n in dbContext.planillaconceptosadministracion
                           where (n.i_Eliminado == 0 || n.i_Eliminado == null) && n.v_IdConceptoPlanilla == IdConceptoPlanilla

                           select n).FirstOrDefault();

                planillaconceptosadministracionDto resultado = planillaconceptosadministracionAssembler.ToDTO(pcd);
                return resultado;


            }
        }


        public planillacalculoDto ObtenerConceptosPlanillaCalculo(string CodigoConcepto)
        {

            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {

                var conceptosPlanillaCalculo = (from a in dbContext.planillacalculo

                                                join b in dbContext.planillaconceptos on new { eliminado = 0, c = a.v_IdConceptoPlanilla } equals new { eliminado = b.i_Eliminado.Value, c = b.v_IdConceptoPlanilla } into b_join
                                                from b in b_join.DefaultIfEmpty()
                                                join c in dbContext.planillanumeracion on new { Numeracion = a.i_IdPlanillaNumeracion.Value, eliminado = 0 } equals new { Numeracion = c.i_Id, eliminado = c.i_Eliminado.Value } into c_join
                                                from c in c_join.DefaultIfEmpty()
                                                join d in dbContext.trabajador on new { t = a.v_IdTrabajador, eliminado = 0 } equals new { t = d.v_IdTrabajador, eliminado = d.i_Eliminado.Value } into d_join
                                                from d in d_join.DefaultIfEmpty()
                                                join e in dbContext.cliente on new { c = d.v_IdCliente, eliminado = 0 } equals new { c = e.v_IdCliente, eliminado = e.i_Eliminado.Value } into e_join
                                                from e in e_join.DefaultIfEmpty()
                                                where b.v_Codigo == CodigoConcepto
                                                select new planillacalculoDto
                                                {
                                                    v_IdConceptoPlanilla = c.v_Observaciones + " " + c.v_Periodo,
                                                    v_IdTrabajador = e.v_CodCliente,

                                                }).FirstOrDefault();

                return conceptosPlanillaCalculo;

            }


        }

        public BindingList<contratotrabajadorDto> ObtenerMovimientoContratos(ref OperationResult objOperationResult, string IdTrabajador)
        {
            try
            {

                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var contratos = (from a in dbContext.contratotrabajador

                                     join b in dbContext.datahierarchy on new { Grupo = 136, eliminado = 0, tc = a.i_IdTipoContrato.Value } equals new { Grupo = b.i_GroupId, eliminado = b.i_IsDeleted.Value, tc = b.i_ItemId } into b_join

                                     from b in b_join.DefaultIfEmpty()

                                     where a.i_Eliminado == 0 && a.v_IdTrabajador == IdTrabajador

                                     select new contratotrabajadorDto
                                     {

                                         v_IdContrato = a.v_IdContrato,
                                         v_IdTrabajador = a.v_IdTrabajador,
                                         v_NroContrato = a.v_NroContrato,
                                         i_IdTipoContrato = a.i_IdTipoContrato,
                                         i_IdTipoPlanilla = a.i_IdTipoPlanilla,
                                         i_IdMonedaContrato = a.i_IdMonedaContrato,
                                         d_Importe = a.d_Importe,
                                         t_FechaInicio = a.t_FechaInicio,
                                         t_FechaFin = a.t_FechaFin,
                                         i_Eliminado = a.i_Eliminado,
                                         i_InsertaIdUsuario = a.i_InsertaIdUsuario,
                                         t_InsertaFecha = a.t_InsertaFecha,
                                         i_ActualizaIdUsuario = a.i_ActualizaIdUsuario,
                                         t_ActualizaFecha = a.t_ActualizaFecha,
                                         t_HoraIngreso = a.t_HoraIngreso,
                                         t_HoraSalida = a.t_HoraSalida,
                                         TipoContrato = b == null ? "" : b.v_Value1,
                                         d_TipoCambio = a.d_TipoCambio,
                                         i_IdModalidadFormativa = a.i_IdModalidadFormativa,
                                         i_IdMotivoBaja = a.i_IdMotivoBaja,
                                         i_ContratoVigente = a.i_ContratoVigente,


                                     }).ToList().OrderBy(x => x.v_NroContrato).ToList();
                    // var ContratoDto = contratotrabajadorAssembler.ToDTOs(contratos);
                    var contratosResult = new BindingList<contratotrabajadorDto>(contratos);
                    objOperationResult.Success = 1;
                    return contratosResult;

                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;
            }
        }

        public BindingList<contratodetalletrabajadorDto> ObtenerContratoDetalle(ref OperationResult objOperationResult, string IdContrato)
        {
            try
            {

                BindingList<contratodetalletrabajadorDto> contratodetalleResult = new BindingList<contratodetalletrabajadorDto>();

                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    var contratoDetalle = (from a in dbContext.contratodetalletrabajador

                                           join b in dbContext.planillaconceptos on new { TipoConcepto = (int)TipoConceptosPlanillas.Ingresos, eliminado = 0, item = a.v_IdConcepto } equals new { TipoConcepto = b.i_IdTipoConcepto.Value, eliminado = b.i_Eliminado.Value, item = b.v_IdConceptoPlanilla }

                                           where a.i_Eliminado == 0 && a.v_IdContrato == IdContrato

                                           select new contratodetalletrabajadorDto
                                           {
                                               v_IdContratoDetalle = a.v_IdContratoDetalle,
                                               v_IdContrato = a.v_IdContrato,
                                               v_IdConcepto = a.v_IdConcepto,
                                               //i_IdMoneda = a.i_IdMoneda,
                                               i_Fijo = a.i_Fijo,
                                               d_ImporteMensual = a.d_ImporteMensual,
                                               i_Eliminado = a.i_Eliminado,
                                               t_InsertaFecha = a.t_InsertaFecha,
                                               //t_ActualizaIdUsuario = a.t_ActualizaIdUsuario,
                                               i_InsertaIdUsuario = a.i_InsertaIdUsuario,
                                               i_ActualizaIdUsuario = a.i_ActualizaIdUsuario,
                                               Concepto = b == null ? "" : b.v_Codigo,
                                               NombreConcepto = b == null ? "" : b.v_Nombre,

                                           }).ToList();
                    contratodetalleResult = new BindingList<contratodetalletrabajadorDto>(contratoDetalle);

                }
                objOperationResult.Success = 1;
                return contratodetalleResult;
            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                return null;

            }

        }

        public List<planillaconceptosDto> ObtenerBusquedaListaPlanillaConceptos(ref OperationResult objOperationResult, string Filtro, string pstrSortExpression)
        {
            try
            {

                // List<planillaconceptosDto> ListaConceptos = new List<planillaconceptosDto>();
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    var ListaConceptos = (from planilla in dbContext.planillaconceptos

                                          join TpIngresos in dbContext.datahierarchy on new { Grupo = 112, eliminado = 0, t = planilla.i_IdTipoPlanilla.Value } equals new { Grupo = TpIngresos.i_GroupId, eliminado = TpIngresos.i_IsDeleted.Value, t = TpIngresos.i_ItemId } into TpIngresos_join

                                          from TpIngresos in TpIngresos_join.DefaultIfEmpty()
                                          where planilla.i_Eliminado == 0

                                          select new planillaconceptosDto
                                          {
                                              v_IdConceptoPlanilla = planilla.v_IdConceptoPlanilla,
                                              i_IdTipoConcepto = planilla.i_IdTipoConcepto,
                                              v_Codigo = planilla.v_Codigo,
                                              TipoPlanilla = TpIngresos.v_Value1,
                                              i_IdTipoPlanilla = planilla.i_IdTipoPlanilla,
                                              v_Nombre = planilla.v_Nombre,
                                              i_ConsiderarVacaciones = planilla.i_ConsiderarVacaciones == null ? 0 : planilla.i_ConsiderarVacaciones,
                                              i_DescontarEsSalud = planilla.i_DescontarEsSalud == null ? 0 : planilla.i_DescontarEsSalud,
                                              i_DescontarSCTR = planilla.i_DescontarSCTR == null ? 0 : planilla.i_DescontarSCTR,
                                              i_DescontarSCTRPens = planilla.i_DescontarSCTRPens == null ? 0 : planilla.i_DescontarSCTRPens

                                          }
                                  ).AsQueryable();

                    if (!string.IsNullOrEmpty(Filtro))
                    {
                        ListaConceptos = ListaConceptos.Where(Filtro);
                    }
                    if (!string.IsNullOrEmpty(pstrSortExpression))
                    {
                        ListaConceptos = ListaConceptos.OrderBy(pstrSortExpression);
                    }


                    List<planillaconceptosDto> objData = ListaConceptos.ToList();
                    objOperationResult.Success = 1;
                    return objData;
                    //ListaConceptos = query.ToDTOs();

                }
                //objOperationResult.Success = 1;
                // return ListaConceptos;
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;
            }
        }

        public BindingList<regimenpensionariotrabajadorDto> ObtenerRegimenPensionario(ref OperationResult objOperationResult, string IdTrabajador)
        {
            BindingList<regimenpensionariotrabajadorDto> RegimenResult = new BindingList<regimenpensionariotrabajadorDto>();
            try
            {

                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var regimen = (from n in dbContext.regimenpensionariotrabajador

                                   where n.i_Eliminado == 0 && n.v_IdTrabajador == IdTrabajador

                                   select new regimenpensionariotrabajadorDto
                                   {
                                       v_IdRegimenPensionario = n.v_IdRegimenPensionario,
                                       v_IdTrabajador = n.v_IdTrabajador,
                                       i_IdRegimenPensionario = n.i_IdRegimenPensionario,
                                       t_FechaInscripcion = n.t_FechaInscripcion,
                                       v_NroCussp = n.v_NroCussp,
                                       v_NroCuenta = n.v_NroCuenta,
                                       i_IdModalidadRegimen = n.i_IdModalidadRegimen,
                                       i_Eliminado = n.i_Eliminado,
                                       i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                       t_InsertaFecha = n.t_InsertaFecha,
                                       i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                       t_ActualizaFecha = n.t_ActualizaFecha,
                                       i_RegimenVigente = n.i_RegimenVigente,


                                   }).ToList();

                    RegimenResult = new BindingList<regimenpensionariotrabajadorDto>(regimen);

                }
                objOperationResult.Success = 1;
                return RegimenResult;
            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                return null;


            }

        }


        public BindingList<derechohabientetrabajadorDto> ObtenerDerechoHabientes(ref OperationResult objOperationResult, string IdTrabajador)
        {
            try
            {

                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var derechohabientes = (from a in dbContext.derechohabientetrabajador

                                            where a.i_Eliminado == 0 && a.v_IdTrabajador == IdTrabajador

                                            select a).ToList();
                    var DHDto = derechohabientetrabajadorAssembler.ToDTOs(derechohabientes);
                    BindingList<derechohabientetrabajadorDto> contratosResult = new BindingList<derechohabientetrabajadorDto>(DHDto);
                    objOperationResult.Success = 1;
                    return contratosResult;

                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;
            }
        }

        public BindingList<areaslaboratrabajadorDto> ObtenerAreasLabora(ref OperationResult objOperationResult, string IdTrabajador)
        {
            try
            {

                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var areaslabora = (from a in dbContext.areaslaboratrabajador

                                       join b in dbContext.datahierarchy on new { Grupo = 31, eliminado = 0, item = a.i_IdCentroCosto.Value } equals new { Grupo = b.i_GroupId, eliminado = b.i_IsDeleted.Value, item = b.i_ItemId } into b_join

                                       from b in b_join.DefaultIfEmpty()

                                       where a.i_Eliminado == 0 && a.v_IdTrabajador == IdTrabajador

                                       select new areaslaboratrabajadorDto
                                       {
                                           v_IdAreaLaborada = a.v_IdAreaLaborada,
                                           i_IdCentroCosto = a.i_IdCentroCosto,
                                           v_FechaInicio = a.v_FechaInicio,
                                           v_FechaFin = a.v_FechaFin,
                                           v_Cargo = a.v_Cargo,
                                           i_Eliminado = a.i_Eliminado,
                                           t_InsertaFecha = a.t_InsertaFecha,
                                           t_ActualizaFecha = a.t_ActualizaFecha,
                                           i_ActualizaIdUsuario = a.i_ActualizaIdUsuario,
                                           i_InsertaIdUsuario = a.i_InsertaIdUsuario,
                                           v_IdTrabajador = a.v_IdTrabajador,
                                           NombreCentroCosto = b.v_Value1,
                                           i_AreaVigente = a.i_AreaVigente,
                                           i_IdTurno = a.i_IdTurno ?? -1
                                       }

                                            ).ToList();


                    BindingList<areaslaboratrabajadorDto> contratosResult = new BindingList<areaslaboratrabajadorDto>(areaslabora);
                    objOperationResult.Success = 1;
                    return contratosResult;

                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;
            }
        }


        public string DevolverTipoCambioPorFecha(ref OperationResult pobjOperationResult, DateTime Fecha)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = (from n in dbContext.tipodecambio
                             where n.i_IsDeleted == 0 && n.d_FechaTipoC == Fecha
                             select n
                             ).FirstOrDefault();

                pobjOperationResult.Success = 1;

                if (query != null)
                {
                    string TipoCambio = query.d_ValorVenta.ToString();
                    return TipoCambio;
                }
                else
                {
                    //return string.Empty;
                    return "0"; // cambie
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public trabajadorDto ObtenerTrabajador(ref OperationResult pobjOperationResult, string pstringIdcliente)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    //trabajadorDto objDtoEntity = null;
                    var objEntity = (from A in dbContext.trabajador
                                     where A.v_IdCliente == pstringIdcliente
                                     select new trabajadorDto
                                     {

                                         v_IdTrabajador = A.v_IdTrabajador,
                                         v_IdCliente = A.v_IdCliente,
                                         v_CodInterno = A.v_CodInterno,
                                         i_IdEstadoCivil = A.i_IdEstadoCivil ?? -1,
                                         v_NroRuc = A.v_NroRuc,
                                         i_IdPaisNac = A.i_IdPaisNac ?? -1,
                                         i_IdDepartamentoNac = A.i_IdDepartamentoNac ?? -1,
                                         i_IdProvinciaNac = A.i_IdProvinciaNac ?? -1,
                                         i_IdDistritoNac = A.i_IdDistritoNac ?? -1,
                                         i_IdTipoVia = A.i_IdTipoVia ?? -1,
                                         v_NombreVia = A.v_NombreVia,
                                         i_IdTipoZona = A.i_IdTipoZona ?? -1,
                                         v_NombreZona = A.v_NombreZona,
                                         v_NumeroDomicilio = A.v_NumeroDomicilio,
                                         v_DepartamentoDomicilio = A.v_DepartamentoDomicilio,
                                         v_InteriorDomicilio = A.v_InteriorDomicilio,
                                         v_KilometroDomicilio = A.v_KilometroDomicilio,
                                         v_ManzanaDomicilio = A.v_ManzanaDomicilio,
                                         v_LoteDomicilio = A.v_LoteDomicilio,
                                         v_BloqueDomicilio = A.v_BloqueDomicilio,
                                         v_Referencia = A.v_Referencia,
                                         b_Foto = A.b_Foto,
                                         i_Domiciliado = A.i_Domiciliado ?? -1,
                                         i_TieneOtrosIngresos5taCat = A.i_TieneOtrosIngresos5taCat ?? -1,
                                         i_Renta5taCatExonerada = A.i_Renta5taCatExonerada ?? -1,
                                         i_RegimenLaboral = A.i_RegimenLaboral ?? -1,
                                         i_TipoTrabajador = A.i_TipoTrabajador ?? -1,
                                         i_NivelEducativo = A.i_NivelEducativo ?? -1,
                                         t_FechaAlta = A.t_FechaAlta,
                                         i_CategoriaOcupacional = A.i_CategoriaOcupacional ?? -1,
                                         i_Ocupacion = A.i_Ocupacion ?? -1,
                                         i_SujetoAcumulativo = A.i_SujetoAcumulativo ?? -1,
                                         i_SujetoTrabajoMaximo = A.i_SujetoTrabajoMaximo ?? -1,
                                         i_SujetoHorarioNoct = A.i_SujetoHorarioNoct ?? -1,
                                         i_Sindicalizado = A.i_Sindicalizado ?? -1,
                                         i_Situacion = A.i_Situacion ?? -1,
                                         i_TipoPago = A.i_TipoPago ?? -1,
                                         i_PeriodoPago = A.i_PeriodoPago ?? -1,
                                         i_SituacionEspecial = A.i_SituacionEspecial ?? -1,
                                         i_IdTipoPensionista = A.i_IdTipoPensionista ?? -1,
                                         i_IdTipoModalidadFormativa = A.i_IdTipoModalidadFormativa ?? -1,
                                         i_IdCentroFormacion = A.i_IdCentroFormacion ?? -1,
                                         i_IdSituacionEducativa = A.i_IdSituacionEducativa ?? -1,
                                         i_EntidadFinCts = A.i_EntidadFinCts ?? -1,
                                         v_NroCuentaCts = A.v_NroCuentaCts,
                                         i_IdMonedaCts = A.i_IdMonedaCts ?? -1,
                                         i_EntidadCuentaAbono = A.i_EntidadCuentaAbono ?? -1,
                                         v_NroCuentaAbono = A.v_NroCuentaAbono,
                                         i_IdTipoCuentaAbono = A.i_IdTipoCuentaAbono ?? -1,
                                         i_IdMonedaCuentaAbono = A.i_IdMonedaCuentaAbono ?? -1,
                                         i_AportaEsSaludVida = A.i_AportaEsSaludVida ?? -1,
                                         i_AportaEsSaludSctr = A.i_AportaEsSaludSctr ?? -1,
                                         i_IdAportaEsSaludSctr = A.i_IdAportaEsSaludSctr ?? -1,
                                         i_AportaAseguraPension = A.i_AportaAseguraPension ?? -1,
                                         i_AportaPensionSctr = A.i_AportaPensionSctr ?? -1,
                                         i_IdAportaPensionSctr = A.i_IdAportaPensionSctr ?? -1,
                                         i_TieneDiscapacidad = A.i_TieneDiscapacidad ?? -1,
                                         i_AplicaConvenioDobleI = A.i_AplicaConvenioDobleI ?? -1,
                                         i_IdRegimenSalud = A.i_IdRegimenSalud ?? -1,
                                         v_NroEsSalud = A.v_NroEsSalud,
                                         i_IdEpsServicioPropio = A.i_IdEpsServicioPropio ?? -1,
                                         v_NroEps = A.v_NroEps,
                                         i_Eliminado = A.i_Eliminado,
                                         i_InsertaIdUsuario = A.i_Eliminado,
                                         t_InsertaFecha = A.t_InsertaFecha,
                                         i_ActualizaIdUsuario = A.i_ActualizaIdUsuario,
                                         t_ActualizaFecha = A.t_ActualizaFecha,
                                         v_CodCliente = A.cliente.v_CodCliente,
                                         v_ApePaterno = A.cliente.v_ApePaterno,
                                         v_ApeMaterno = A.cliente.v_ApeMaterno,
                                         v_PrimerNombre = A.cliente.v_PrimerNombre,
                                         v_SegundoNombre = A.cliente.v_SegundoNombre,
                                         i_IdSexo = A.cliente.i_IdSexo ?? -1,
                                         i_IdTipoIdentificacion = A.cliente.i_IdTipoIdentificacion ?? -1,
                                         v_NroDocIdentificacion = A.cliente.v_NroDocIdentificacion,
                                         i_IdPais = A.cliente.i_IdPais ?? -1,
                                         i_IdDepartamento = A.cliente.i_IdDepartamento ?? -1,
                                         i_IdProvincia = A.cliente.i_IdProvincia ?? -1,
                                         i_IdDistrito = A.cliente.i_IdDistrito ?? -1,
                                         i_Nacionalidad = A.cliente.i_Nacionalidad ?? -1,
                                         t_FechaNacimiento = A.cliente.t_FechaNacimiento,
                                         v_Correo = A.cliente.v_Correo,
                                         v_TelefonoFijo = A.cliente.v_TelefonoFijo,
                                         v_TelefonoMovil = A.cliente.v_TelefonoMovil,
                                         i_Activo = A.cliente.i_Activo,
                                         t_FechaCese = A.t_FechaCese,
                                         b_HojaVida = A.b_HojaVida

                                     }
                                     ).FirstOrDefault();
                    //if (objEntity != null)
                    // objDtoEntity = trabajadorAssembler.ToDTO(objEntity);

                    pobjOperationResult.Success = 1;
                    return objEntity;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }


        public trabajadorDto ObtenerTrabajadorbyIdTrabajador(ref OperationResult pobjOperationResult, string pstringIdRepresentante)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    //trabajadorDto objDtoEntity = null;
                    var objEntity = (from A in dbContext.trabajador
                                     where A.v_IdTrabajador == pstringIdRepresentante
                                     select new trabajadorDto
                                     {



                                         v_RazonSocial = A.cliente.v_ApePaterno + " " + A.cliente.v_ApeMaterno + " " + A.cliente.v_PrimerNombre + " " + A.cliente.v_SegundoNombre,
                                         v_NroDocIdentificacion = A.cliente.v_NroDocIdentificacion,


                                     }
                                     ).FirstOrDefault();


                    pobjOperationResult.Success = 1;
                    return objEntity;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }


        public List<Remuneraciones> VisualizarRemuneraciones(ref OperationResult objOperationResult, string IdTrabajador, string periodo)
        {

            try
            {
                List<Remuneraciones> Agrupados = new List<Remuneraciones>();
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    var planilla = (from a in dbContext.planillacalculo

                                    join b in dbContext.planillanumeracion on new { IdPlanilla = a.i_IdPlanillaNumeracion.Value, eliminado = 0 } equals new { IdPlanilla = b.i_Id, eliminado = b.i_Eliminado.Value } into b_join

                                    from b in b_join.DefaultIfEmpty()

                                    join e in dbContext.areaslaboratrabajador on new { IdArea = a.v_IdTrabajador, eliminado = 0, vigente = 1 } equals new { IdArea = e.v_IdTrabajador, eliminado = e.i_Eliminado.Value, vigente = e.i_AreaVigente.Value } into e_join
                                    from e in e_join.DefaultIfEmpty()

                                    join f in dbContext.planillavariablestrabajador on new { IdTrabajador = a.v_IdTrabajador, eliminado = 0 } equals new { IdTrabajador = f.v_IdTrabajador, eliminado = f.i_Eliminado.Value } into f_join
                                    from f in f_join.DefaultIfEmpty()
                                    where a.v_IdTrabajador == IdTrabajador && b.v_Periodo == periodo

                                    select new Remuneraciones
                                    {
                                        Mes = b.v_Mes,
                                        Cargo = e != null ? e.v_Cargo : "",
                                        IdPlanilla = a.i_IdPlanillaNumeracion.Value,
                                        NumeroPlanilla = b.v_Numero,
                                        IDiasLaboradas = f.i_DiasLaborados.Value,
                                        DHorasLaboradas = f.d_HorasLaboradosBP.Value,



                                    }).ToList().Select(n => new Remuneraciones
                                    {
                                        Mes = (new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(int.Parse(n.Mes))).ToUpper(),
                                        Cargo = n.Cargo,
                                        IdPlanilla = n.IdPlanilla,
                                        NumeroPlanilla = n.NumeroPlanilla,
                                        IDiasLaboradas = n.IDiasLaboradas,
                                        DHorasLaboradas = n.DHorasLaboradas,


                                    }).ToList();
                    Agrupados = planilla.GroupBy(x => new { x.Mes }).Select(x => x.First()).ToList();


                }


                objOperationResult.Success = 1;
                return Agrupados.OrderBy(x => x.Mes).ToList();
            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                return null;


            }


        }



        public List<PlameJornadaLaboral> PlameJornadaLaboral(ref OperationResult objOperationResult, string Filtro)
        {

            try
            {

                List<PlameJornadaLaboral> LPlameJornadaLaboral = new List<PlameJornadaLaboral>();
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    var VariablesTrabajador = (from a in dbContext.planillavariablestrabajador
                                               join b in dbContext.trabajador on new { Trabajador = a.v_IdTrabajador, eliminado = 0 } equals new { Trabajador = b.v_IdTrabajador, eliminado = b.i_Eliminado.Value } into b_join
                                               from b in b_join.DefaultIfEmpty()
                                               join c in dbContext.cliente on new { client = b.v_IdCliente } equals new { client = c.v_IdCliente } into c_join

                                               from c in c_join.DefaultIfEmpty()

                                               join d in dbContext.datahierarchy on new { TipoDocIdent = c.i_IdTipoIdentificacion.Value, eliminado = 0, Grupo = 132 } equals new { TipoDocIdent = d.i_ItemId, eliminado = d.i_IsDeleted.Value, Grupo = d.i_GroupId } into d_join

                                               from d in d_join.DefaultIfEmpty()

                                               where a.i_Eliminado == 0

                                               select new PlameJornadaLaboral
                                               {
                                                   Trabajador = d != null ? c.v_CodCliente + " " + c.v_ApePaterno + " " + c.v_ApeMaterno + " " + c.v_PrimerNombre : "",
                                                   TipoDocTrabajador = c != null ? string.IsNullOrEmpty(d.v_Value2) ? "0" : d.v_Value2 : "0",
                                                   NumeroDocumento = d != null ? c.v_NroDocIdentificacion : "",
                                                   DHorasTrabajadas = a != null ? a.d_HorasLaboradosBP ?? 0 : 0,
                                                   v_IdPlanillaVariablesTrabajador = a != null ? a.v_IdPlanillaVariablesTrabajador : "",
                                                   v_IdTrabajador = b == null ? "" : b.v_IdTrabajador,
                                                   i_IdPlanillaNumeracion = a != null ? a.i_IdPlanillaNumeracion == null ? -1 : a.i_IdPlanillaNumeracion : -1

                                               });

                    if (!string.IsNullOrEmpty(Filtro))
                    {
                        VariablesTrabajador = VariablesTrabajador.Where(Filtro);

                    }

                    List<PlameJornadaLaboral> TrabajadorPlameLaboral = VariablesTrabajador.ToList().GroupBy(x => new { x.v_IdTrabajador }).Select(g => g.First()).ToList();

                    // Filtro solo los tipos de documentos sean 01,04,07
                    TrabajadorPlameLaboral = TrabajadorPlameLaboral.Where(x => int.Parse(x.TipoDocTrabajador) == 1 || int.Parse(x.TipoDocTrabajador) == 4 || int.Parse(x.TipoDocTrabajador) == 7).ToList();
                    var planillavariableshorasextras = dbContext.planillavariableshorasextras.Where(o => o.i_Eliminado == 0).ToList();
                    foreach (var item in TrabajadorPlameLaboral)
                    {
                        PlameJornadaLaboral objReporte = new PlameJornadaLaboral();
                        Error = item.v_IdTrabajador;
                        objReporte = item;
                        string[] HorasTrabajadas = new string[2];
                        HorasTrabajadas = item.DHorasTrabajadas.ToString("#######.00").Split('.');

                        objReporte.SHorasTrabajadas = HorasTrabajadas[0];
                        objReporte.MinutosTrabajados = HorasTrabajadas[1];
                        objReporte.MinutosTrabajados = objReporte.MinutosTrabajados == null ? "0" : objReporte.MinutosTrabajados;

                        var HorasExtras = (from a in planillavariableshorasextras
                                           where a.v_IdPlanillaVariablesTrabajador == item.v_IdPlanillaVariablesTrabajador
                                           select a).ToList();
                        string[] he = new string[2];
                        if (HorasExtras.Any())
                        {
                            he = (HorasExtras.Sum(x => x.d_HorasExtras).Value).ToString().Split('.');
                            if (he.Count() == 1)
                            {
                                objReporte.HorasExtras = he[0];
                                objReporte.MinutosExtras = "0";
                            }
                            else if (int.Parse(he[1]) >= 60)
                            {
                                int r = (int.Parse(he[1]) / 60);
                                int rr = (int.Parse(he[1]) % 60);
                                objReporte.HorasExtras = (int.Parse(he[0]) + r).ToString();
                                objReporte.MinutosExtras = rr.ToString();
                            }
                            else
                            {

                                objReporte.HorasExtras = he[0];
                                objReporte.MinutosExtras = he[1];
                            }


                        }
                        else
                        {
                            objReporte.HorasExtras = "0";
                            objReporte.MinutosExtras = "0";
                        }
                        objReporte.TipoPlame = "TRABAJADOR - DATOS DE LA JORNADA LABORAL ";
                        objReporte.NroTipoPlame = 3;
                        LPlameJornadaLaboral.Add(objReporte);
                    }
                    if (!LPlameJornadaLaboral.Any())
                    {
                        PlameJornadaLaboral objReporte = new PlameJornadaLaboral();
                        objReporte.TipoPlame = "TRABAJADOR - DATOS DE LA JORNADA LABORAL ";
                        objReporte.Trabajador = "SIN OPERACIONES";
                        LPlameJornadaLaboral.Add(objReporte);
                    }
                }
                objOperationResult.Success = 1;
                return LPlameJornadaLaboral.OrderBy(o => o.Trabajador).ToList();
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = Error;
                return null;

            }
        }

        public List<PlameIngDesAport> PlameIngresDescuenAport(ref OperationResult objOperationResult, string Filtro)
        {

            try
            {
                List<systemparameterDto> ListaConceptos = new List<systemparameterDto>();
                List<PlameIngDesAport> plameFinal = new List<PlameIngDesAport>();

                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    using (SAMBHSEntitiesModel db = new SAMBHSEntitiesModel())
                    {

                        var conceptos = (from a in db.systemparameter
                                         where a.i_GroupId == 152 && a.i_IsDeleted == 0
                                         //where a.v_Value2 != "0100" && a.v_Value2 != "0200" && a.v_Value2 != "0300" && a.v_Value2 != "0400" && a.v_Value2 != "0500" && a.v_Value2 != "0600"
                                         //&& a.v_Value2 != "0603" && a.v_Value2 != "0604" && a.v_Value2 != "0607" && a.v_Value2 != "0610" && a.v_Value2 != "0612" && a.v_Value2 != "0800" && a.v_Value2 != "0802"
                                         //&& a.v_Value2 != "0804" && a.v_Value2 != "0806" && a.v_Value2 != "0808"
                                         select a).ToList();
                        ListaConceptos = systemparameterAssembler.ToDTOs(conceptos).ToList();
                    }


                    var plame = (from a in dbContext.planillacalculo

                                 join b in dbContext.planillaconceptos on new { IdConcepto = a.v_IdConceptoPlanilla, eliminado = 0 } equals new { IdConcepto = b.v_IdConceptoPlanilla, eliminado = b.i_Eliminado.Value } into b_join

                                 from b in b_join.DefaultIfEmpty()

                                 join c in dbContext.trabajador on new { IdTrabajador = a.v_IdTrabajador, eliminado = 0 } equals new { IdTrabajador = c.v_IdTrabajador, eliminado = c.i_Eliminado.Value } into c_join

                                 from c in c_join.DefaultIfEmpty()

                                 join d in dbContext.cliente on new { client = c.v_IdCliente } equals new { client = d.v_IdCliente } into d_join
                                 from d in d_join.DefaultIfEmpty()


                                 join e in dbContext.datahierarchy on new { TipoDocIdent = d.i_IdTipoIdentificacion.Value, eliminado = 0, Grupo = 132 } equals new { TipoDocIdent = e.i_ItemId, eliminado = e.i_IsDeleted.Value, Grupo = e.i_GroupId } into e_join

                                 from e in e_join.DefaultIfEmpty()



                                 // where b.i_IdTipoConceptoPlanilla != null   && c.i_IdTipoPensionista ==-1
                                 where c.i_IdTipoPensionista == -1
                                 select new PlameIngDesAport
                                 {
                                     Trabajador = d != null ? d.v_CodCliente.Trim() + " " + d.v_ApePaterno.Trim() + " " + d.v_ApeMaterno.Trim() + " " + d.v_PrimerNombre.Trim() : "",
                                     NumeroDocumento = d != null ? d.v_NroDocIdentificacion : "",
                                     TipoDocTrabajador = e != null ? string.IsNullOrEmpty(e.v_Value2) ? "0" : e.v_Value2 : "0",
                                     IdConceptoSunat = b.i_IdTipoConceptoPlanilla ?? -1,
                                     MontoDevengado = a.d_Importe != null ? a.d_Importe.Value : 0,
                                     MontoPagado = a.d_Importe != null ? a.d_Importe.Value : 0,
                                     i_IdPlanillaNumeracion = a.i_IdPlanillaNumeracion.Value,
                                     v_IdTrabajador = c.v_IdTrabajador,
                                     CodigoConceptoSistema = b.v_Codigo,

                                 });

                    if (!string.IsNullOrEmpty(Filtro))
                    {
                        plame = plame.Where(Filtro);
                    }

                    var plameF = plame.ToList().Where(x => int.Parse(x.TipoDocTrabajador) == 1 || int.Parse(x.TipoDocTrabajador) == 4 || int.Parse(x.TipoDocTrabajador) == 7).ToList();
                    foreach (var item in plameF)
                    {
                        PlameIngDesAport objReporte = new PlameIngDesAport();
                        objReporte = item;
                        objReporte.CodigoConcepto = ListaConceptos.Where(x => x.i_ParameterId == item.IdConceptoSunat).Select(x => x.v_Value2).ToList().Any() ? ListaConceptos.Where(x => x.i_ParameterId == item.IdConceptoSunat).Select(x => x.v_Value2).FirstOrDefault() : "";

                        if (objReporte.CodigoConcepto != "0100" &&
                                objReporte.CodigoConcepto != "0200" && objReporte.CodigoConcepto != "0300" && objReporte.CodigoConcepto != "0400" && objReporte.CodigoConcepto != "0500" && objReporte.CodigoConcepto != "0600"
                                         && objReporte.CodigoConcepto != "0603" && objReporte.CodigoConcepto != "0604" && objReporte.CodigoConcepto != "0607" && objReporte.CodigoConcepto != "0610" && objReporte.CodigoConcepto != "0612" && objReporte.CodigoConcepto != "0616" && objReporte.CodigoConcepto != "0800" && objReporte.CodigoConcepto != "0802"
                                         && objReporte.CodigoConcepto != "0804" && objReporte.CodigoConcepto != "0806" && objReporte.CodigoConcepto != "0808")
                        {
                            objReporte.TipoPlame = "TRABAJADOR - DETALLE DE LOS INGRESOS, TRIBUTOS  Y DESCUENTOS ";
                            objReporte.NroTipoPlame = 5;
                            objReporte.CodigoConceptoSistema = item.CodigoConceptoSistema.Trim();
                            var ExisteQuintaCliente = plame.Where(l => l.v_IdTrabajador == item.v_IdTrabajador && l.IdConceptoSunat == 72).ToList(); // Si ninguno de los conceptos estan relacionados con quinta categoria entonces  agregra este concepto
                            if (!ExisteQuintaCliente.Any())
                            {
                                PlameIngDesAport obj5 = new PlameIngDesAport();
                                obj5 = (PlameIngDesAport)item.Clone();
                                obj5.IdConceptoSunat = 72;
                                obj5.MontoDevengado = 0;
                                obj5.MontoPagado = 0;
                                obj5.CodigoConcepto = "0605";
                                plameFinal.Add(obj5);

                            }

                            plameFinal.Add(objReporte);
                        }

                    }
                    plameFinal = plameFinal.GroupBy(o => new { Trab = o.v_IdTrabajador, Cod = o.CodigoConcepto }).Select(x =>
                        {
                            var h = x.LastOrDefault();
                            h.MontoDevengado = x.Sum(y => y.MontoDevengado);
                            h.MontoPagado = x.Sum(y => y.MontoPagado);
                            return h;

                        }).ToList();
                }

                if (!plameFinal.Any())
                {
                    PlameIngDesAport objReporte = new PlameIngDesAport();
                    objReporte.TipoPlame = "TRABAJADOR - DETALLE DE LOS INGRESOS, TRIBUTOS  Y DESCUENTOS ";
                    objReporte.Trabajador = "SIN OPERACIONES";
                    plameFinal.Add(objReporte);

                }

                objOperationResult.Success = 1;

                return plameFinal.OrderBy(l => l.Trabajador).ToList();
            }
            catch (Exception ex)
            {


                objOperationResult.Success = 0;
                return null;
            }

        }



        public List<PlameIngDesAport> PlameIngresDescuenAportPensionistas(ref OperationResult objOperationResult, string Filtro)
        {

            try
            {
                List<systemparameterDto> ListaConceptos = new List<systemparameterDto>();
                List<PlameIngDesAport> plameFinal = new List<PlameIngDesAport>();

                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    using (SAMBHSEntitiesModel db = new SAMBHSEntitiesModel())
                    {

                        var conceptos = (from a in db.systemparameter
                                         where a.i_GroupId == 152 && a.i_IsDeleted == 0
                                         where a.v_Value2 != "0100" && a.v_Value2 != "0200" && a.v_Value2 != "0300" && a.v_Value2 != "0400" && a.v_Value2 != "0500" && a.v_Value2 != "0600"
                                         && a.v_Value2 != "0603" && a.v_Value2 != "0604" && a.v_Value2 != "0607" && a.v_Value2 != "0610" && a.v_Value2 != "0612" && a.v_Value2 != "0800" && a.v_Value2 != "0802"
                                         && a.v_Value2 != "0804" && a.v_Value2 != "0806" && a.v_Value2 != "0808"
                                         select a).ToList();
                        ListaConceptos = systemparameterAssembler.ToDTOs(conceptos).ToList();
                    }


                    var plame = (from a in dbContext.planillacalculo

                                 join b in dbContext.planillaconceptos on new { IdConcepto = a.v_IdConceptoPlanilla, eliminado = 0 } equals new { IdConcepto = b.v_IdConceptoPlanilla, eliminado = 0 } into b_join

                                 from b in b_join.DefaultIfEmpty()

                                 join c in dbContext.trabajador on new { IdTrabajador = a.v_IdTrabajador, eliminado = 0 } equals new { IdTrabajador = c.v_IdTrabajador, eliminado = c.i_Eliminado.Value } into c_join

                                 from c in c_join.DefaultIfEmpty()



                                 join e in dbContext.cliente on new { client = c.v_IdCliente } equals new { client = e.v_IdCliente } into e_join

                                 from e in e_join.DefaultIfEmpty()


                                 join f in dbContext.datahierarchy on new { TipoDocIdent = e.i_IdTipoIdentificacion.Value, eliminado = 0, Grupo = 132 } equals new { TipoDocIdent = f.i_ItemId, eliminado = f.i_IsDeleted.Value, Grupo = f.i_GroupId } into f_join

                                 from f in f_join.DefaultIfEmpty()



                                 where c.i_IdTipoPensionista != -1   //&& b.i_IdTipoConceptoPlanilla != null 

                                 select new PlameIngDesAport
                                 {
                                     Trabajador = e != null ? e.v_CodCliente + " " + e.v_ApePaterno + " " + e.v_ApeMaterno + " " + e.v_PrimerNombre : "",
                                     NumeroDocumento = e != null ? e.v_NroDocIdentificacion : "",
                                     TipoDocTrabajador = f != null ? f.v_Value2 : "",
                                     IdConceptoSunat = b.i_IdTipoConceptoPlanilla ?? -1,
                                     MontoDevengado = a.d_Importe != null ? a.d_Importe.Value : 0,
                                     MontoPagado = a.d_Importe != null ? a.d_Importe.Value : 0,
                                     i_IdPlanillaNumeracion = a.i_IdPlanillaNumeracion.Value,
                                     v_IdTrabajador = c.v_IdTrabajador,
                                     CodigoConceptoSistema = b.v_Codigo,
                                 });

                    if (!string.IsNullOrEmpty(Filtro))
                    {
                        plame = plame.Where(Filtro);
                    }

                    var plameF = plame.ToList().Where(x => x.TipoDocTrabajador == "01" || x.TipoDocTrabajador == "04" || x.TipoDocTrabajador == "07").ToList();
                    // Filtro solo los tipos de documentos sean 01,04,07

                    foreach (var item in plameF)
                    {
                        PlameIngDesAport objReporte = new PlameIngDesAport();

                        objReporte.CodigoConcepto = ListaConceptos.Where(x => x.i_ParameterId == item.IdConceptoSunat).Select(x => x.v_Value2).ToList().Any() ? ListaConceptos.Where(x => x.i_ParameterId == item.IdConceptoSunat).Select(x => x.v_Value2).FirstOrDefault() : "";
                        //if (objReporte.CodigoConcepto != "")
                        //{
                        objReporte.Trabajador = item.Trabajador;
                        objReporte.NumeroDocumento = item.NumeroDocumento;
                        objReporte.TipoDocTrabajador = item.TipoDocTrabajador;
                        objReporte.MontoPagado = item.MontoPagado;
                        objReporte.MontoDevengado = item.MontoDevengado;
                        objReporte.TipoPlame = "PENSIONISTA - DETALLE DE LOS INGRESOS, TRIBUTOS  Y DESCUENTOS ";
                        objReporte.v_IdTrabajador = item.v_IdTrabajador;
                        objReporte.NroTipoPlame = 6;
                        objReporte.CodigoConceptoSistema = item.CodigoConceptoSistema;
                        plameFinal.Add(objReporte);
                        //}

                    }

                }

                if (!plameFinal.Any())
                {
                    PlameIngDesAport objReporte = new PlameIngDesAport();
                    objReporte.TipoPlame = "PENSIONISTA - DETALLE DE LOS INGRESOS, TRIBUTOS  Y DESCUENTOS ";
                    objReporte.Trabajador = "SIN OPERACIONES";
                    plameFinal.Add(objReporte);
                }

                objOperationResult.Success = 1;

                return plameFinal;
            }
            catch (Exception ex)
            {


                objOperationResult.Success = 0;
                return null;
            }

        }





        public List<PlamePrestadorCuartaDetalles> PlameRestadorCuartaDetallesComprobantes(ref OperationResult objOperationResult, string Filtro, DateTime FechaInicio, DateTime FechaFin)
        {
            try
            {
                List<PlamePrestadorCuartaDetalles> ListaResultante = new List<PlamePrestadorCuartaDetalles>();
                PlamePrestadorCuartaDetalles objReporte = new PlamePrestadorCuartaDetalles();
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    var ListaPlamePrestadorCuartaDetalles = (from a in dbContext.recibohonorario

                                                             join c in dbContext.cliente on new { IdProv = a.v_IdProveedor, eliminado = 0 } equals new { IdProv = c.v_IdCliente, eliminado = c.i_Eliminado.Value } into c_join
                                                             from c in c_join.DefaultIfEmpty()
                                                             join d in dbContext.datahierarchy on new { TipoDocIdent = c.i_IdTipoIdentificacion.Value, eliminado = 0, Grupo = 132 } equals new { TipoDocIdent = d.i_ItemId, eliminado = d.i_IsDeleted.Value, Grupo = d.i_GroupId } into d_join

                                                             from d in d_join.DefaultIfEmpty()
                                                             where a.i_Eliminado == 0 && a.i_IdEstado == 1

                                                             && a.t_FechaRegistro >= FechaInicio && a.t_FechaRegistro <= FechaFin

                                                             select new PlamePrestadorCuartaDetalles
                                                             {
                                                                 TipoDocTrabajador = d.v_Value2,
                                                                 NumeroDocumento = c.v_NroDocIdentificacion,
                                                                 TipoComprobanteEmitido = a.i_IdTipoDocumento == 2 ? "R" : "N",
                                                                 SerieComprobante = a.v_SerieDocumento,
                                                                 NumeroComprobante = a.v_CorrelativoDocumento,
                                                                 MontoTotalServicio = a.d_Importe.Value,
                                                                 FechaEmision = a.t_FechaEmision.Value,
                                                                 IndicarRentaCuarta = a.i_RentaCuartaCategoria.Value == 1 ? "1" : "0",
                                                                 v_IdTrabajador = c.v_IdCliente,
                                                                 Trabajador = c != null ? c.v_CodCliente + " " + c.v_ApePaterno + " " + c.v_ApeMaterno + " " + c.v_PrimerNombre + " " : "",
                                                                 v_Periodo = a.v_Periodo,
                                                                 v_Mes = a.v_Mes,
                                                                 iTipoComprobanteEmitido = a.i_IdTipoDocumento.Value,

                                                             }).ToList();
                    //if (!string.IsNullOrEmpty(Filtro))
                    //{

                    //    recibohonorarios = recibohonorarios.Where(Filtro);
                    //}

                    // List<PlamePrestadorCuartaDetalles> ListaPlamePrestadorCuartaDetalles = recibohonorarios.ToList();

                    var Tesoreria = (from a in dbContext.tesoreria
                                     join b in dbContext.tesoreriadetalle on new { IdTesoreria = a.v_IdTesoreria, eliminado = 0 } equals new { IdTesoreria = b.v_IdTesoreria, eliminado = b.i_Eliminado.Value } into b_join
                                     from b in b_join.DefaultIfEmpty()
                                     where a.i_Eliminado == 0 && a.i_IdEstado == 1
                                     select new
                                     {

                                         fechaPago = a.t_FechaRegistro,
                                         v_NroDocumento = b.v_NroDocumento,
                                         i_IdTipoDocumento = b.i_IdTipoDocumento,
                                         t_FechaRegistro = a.t_FechaRegistro,

                                     }).ToList().OrderBy(x => x.fechaPago);

                    foreach (var item in ListaPlamePrestadorCuartaDetalles)
                    {
                        objReporte = new PlamePrestadorCuartaDetalles();

                        objReporte.TipoDocTrabajador = item.TipoDocTrabajador;
                        objReporte.NumeroDocumento = item.NumeroDocumento;
                        objReporte.TipoComprobanteEmitido = item.TipoComprobanteEmitido;
                        objReporte.SerieComprobante = item.SerieComprobante;
                        objReporte.NumeroComprobante = item.NumeroComprobante;
                        objReporte.MontoTotalServicio = Utils.Windows.DevuelveValorRedondeado(item.MontoTotalServicio, 2);
                        objReporte.Trabajador = item.Trabajador;
                        objReporte.FechaEmision = item.FechaEmision;

                        var PagadoRealizado = (from a in Tesoreria
                                               where
                                               a.t_FechaRegistro >= objReporte.FechaEmision && a.i_IdTipoDocumento == item.iTipoComprobanteEmitido
                                               && a.v_NroDocumento == item.SerieComprobante + "-" + item.NumeroComprobante
                                               select new
                                               {
                                                   fechaPago = a.t_FechaRegistro,
                                                   //v_NroDocumento =a.v_NroDocumento ,
                                                   //i_IdTipoDocumento = b.i_IdTipoDocumento ,

                                               }).ToList().OrderBy(x => x.fechaPago);
                        //Solo se generan las que ha sido pagadas x lo menos alguna parte ???
                        if (PagadoRealizado.Any())
                        {
                            //objReporte.FechaPago = Tesoreria.LastOrDefault().fechaPago.Value ;
                            objReporte.SFechaPago = PagadoRealizado.LastOrDefault().fechaPago.Value.Day.ToString("00") + "/" + PagadoRealizado.LastOrDefault().fechaPago.Value.Month.ToString("00") + "/" + PagadoRealizado.LastOrDefault().fechaPago.Value.Year.ToString("00");

                        }
                        else
                        {
                            objReporte.SFechaPago = "";
                        }
                        objReporte.IndicarRentaCuarta = item.IndicarRentaCuarta;
                        objReporte.TipoPlame = "TRABAJADOR - PRESTADOR DE SERVICIOS CON RENTAS DE CUARTA CATEGORIA : DETALLES DE COMPROBANTES";
                        objReporte.v_IdTrabajador = item.v_IdTrabajador;
                        objReporte.NroTipoPlame = 6;
                        ListaResultante.Add(objReporte);
                    }
                    objOperationResult.Success = 1;

                    if (!ListaResultante.Any())
                    {
                        objReporte = new PlamePrestadorCuartaDetalles();
                        objReporte.TipoPlame = "TRABAJADOR - PRESTADOR DE SERVICIOS CON RENTAS DE CUARTA CATEGORIA : DETALLES DE COMPROBANTES";
                        objReporte.Trabajador = "SIN OPERACIONES";
                        ListaResultante.Add(objReporte);

                    }
                    return ListaResultante;


                }
            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                return null;

            }
        }


        public List<PlameDiasSubsidiados> PlameDiasSubsidiados(ref OperationResult objOperationResult, string Filtro)
        {
            try
            {

                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var EquivalenciaTipoDocumento = dbContext.datahierarchy.Where(o => o.i_GroupId == 132 && o.i_IsDeleted.Value == 0).ToList();
                    var ListaSubsidios = dbContext.datahierarchy.Where(o => o.i_GroupId == 144 && o.i_IsDeleted.Value == 0).ToList();
                    var ListaNoTrabajados = dbContext.datahierarchy.Where(o => o.i_GroupId == 143 && o.i_IsDeleted.Value == 0).ToList();
                    var Sub = (from a in dbContext.planillavariablesdiasnolabsubsidiados

                               join b in dbContext.planillavariablestrabajador on new { IdPlanilla = a.v_IdPlanillaVariablesTrabajador, eliminado = 0 } equals new { IdPlanilla = b.v_IdPlanillaVariablesTrabajador, eliminado = b.i_Eliminado.Value } into b_join

                               from b in b_join.DefaultIfEmpty()


                               join d in dbContext.trabajador on new { Trab = b.v_IdTrabajador, eliminado = 0 } equals new { Trab = d.v_IdTrabajador, eliminado = d.i_Eliminado.Value } into d_join
                               from d in d_join.DefaultIfEmpty()

                               join g in dbContext.cliente on new { client = d.v_IdCliente } equals new { client = g.v_IdCliente } into g_join
                               from g in g_join.DefaultIfEmpty()

                               where a.i_Eliminado == 0

                               select new
                               {

                                   TipoDocTrabajador = g.i_IdTipoIdentificacion ?? -1,
                                   NumeroDocumento = g != null ? g.v_NroDocIdentificacion : "",

                                   DiasSuspension = a.i_CantidadDias == null ? 0 : a.i_CantidadDias.Value,
                                   v_IdTrabajador = d.v_IdTrabajador,
                                   Trabajador = d != null ? d.cliente.v_CodCliente + " " + d.cliente.v_ApePaterno + " " + d.cliente.v_ApeMaterno + " " + d.cliente.v_PrimerNombre + " " : "",
                                   i_IdPlanillaNumeracion = b.i_IdPlanillaNumeracion,
                                   i_IdTipoConepto = a.i_IdTipoConcepto.Value,
                                   TipoSubsidio = a.i_IdConcepto ?? -1,
                               }).ToList().Select(o =>
                               {

                                   var TipoIdentificacion = EquivalenciaTipoDocumento.Where(x => x.v_Value2 == o.TipoDocTrabajador.ToString()).FirstOrDefault();
                                   var TipoSubsidio = o.i_IdTipoConepto == 2 ? ListaSubsidios.Where(x => x.i_ItemId == o.TipoSubsidio).FirstOrDefault() : ListaNoTrabajados.Where(x => x.i_ItemId == o.TipoSubsidio).FirstOrDefault();
                                   return new PlameDiasSubsidiados
                                   {
                                       TipoDocTrabajador = TipoIdentificacion != null ? TipoIdentificacion.v_Value2 : "-1",
                                       NumeroDocumento = o.NumeroDocumento,
                                       TipoSuspensionLaboral = TipoSubsidio != null ? int.Parse(TipoSubsidio.v_Value2).ToString("00") : "-1",
                                       DiasSuspension = o.DiasSuspension,
                                       v_IdTrabajador = o.v_IdTrabajador,
                                       Trabajador = o.Trabajador,
                                       i_IdPlanillaNumeracion = o.i_IdPlanillaNumeracion,
                                   };

                               }).ToList().AsQueryable();

                    if (!string.IsNullOrEmpty(Filtro))
                    {
                        Sub = Sub.Where(Filtro);

                    }
                    List<PlameDiasSubsidiados> ListaSub = Sub.ToList().GroupBy(x => new { x.v_IdTrabajador }).Select(g => g.First()).ToList();

                    // List<PlameDiasSubsidiados> TrabajadorPlameLaboral = Sub.ToList().GroupBy(x => new { x.v_IdTrabajador }).Select(g => g.First()).ToList();
                    // Filtro solo los tipos de documentos sean 01,04,07
                    ListaSub = ListaSub.Where(x => x.TipoDocTrabajador == "1" || x.TipoDocTrabajador == "4" || x.TipoDocTrabajador == "7").ToList();

                    ListaSub = (from a in ListaSub
                                select new PlameDiasSubsidiados
                                  {
                                      TipoDocTrabajador = a.TipoDocTrabajador,
                                      NumeroDocumento = a.NumeroDocumento,
                                      TipoSuspensionLaboral = a.TipoSuspensionLaboral,
                                      DiasSuspension = a.DiasSuspension,
                                      TipoPlame = "TRABAJADOR - DIAS SUBSIDIADOS Y OTROS NO LABORADOS",
                                      v_IdTrabajador = a.v_IdTrabajador,
                                      NroTipoPlame = 4,
                                      Trabajador = a.Trabajador,
                                  }).ToList();
                    objOperationResult.Success = 1;

                    if (!ListaSub.Any())
                    {
                        PlameDiasSubsidiados objReporte = new PlameDiasSubsidiados();
                        objReporte.TipoPlame = "TRABAJADOR - DIAS SUBSIDIADOS Y OTROS NO LABORADOS";
                        objReporte.Trabajador = "SIN OPERACIONES";
                        ListaSub.Add(objReporte);
                    }
                    return ListaSub;

                }


            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;

            }


        }

        public List<PlamePrestadorCuarta> PlamePrestadorCuarta(ref OperationResult objOperationResult, string Filtro, DateTime FechaInici, DateTime FechaFin)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    var recibohonorarios = (from a in dbContext.recibohonorario

                                            join c in dbContext.cliente on new { IdProv = a.v_IdProveedor, eliminado = 0 } equals new { IdProv = c.v_IdCliente, eliminado = c.i_Eliminado.Value } into c_join
                                            from c in c_join.DefaultIfEmpty()
                                            join d in dbContext.datahierarchy on new { TipoDocIdent = c.i_IdTipoIdentificacion.Value, eliminado = 0, Grupo = 132 } equals new { TipoDocIdent = d.i_ItemId, eliminado = d.i_IsDeleted.Value, Grupo = d.i_GroupId } into d_join

                                            from d in d_join.DefaultIfEmpty()
                                            join e in dbContext.datahierarchy on new { DobleTribut = c.i_IdConvenioDobleTributacion.Value, eliminado = 0, Grupo = 146 } equals new { DobleTribut = e.i_ItemId, eliminado = e.i_IsDeleted.Value, Grupo = e.i_GroupId } into e_join

                                            from e in e_join.DefaultIfEmpty()
                                            where a.i_Eliminado == 0
                                                //&& c.i_IdConvenioDobleTributacion != -1 && c.i_IdConvenioDobleTributacion != null
                                            && a.i_IdEstado == 1
                                            && a.t_FechaRegistro >= FechaInici && a.t_FechaRegistro <= FechaFin

                                            select new PlamePrestadorCuarta
                                            {
                                                TipoDocTrabajador = d.v_Value2,
                                                NumeroDocumento = c.v_NroDocIdentificacion,
                                                ApellidoPaterno = c.v_ApePaterno,
                                                ApellidoMaterno = c.v_ApeMaterno,
                                                Nombres = c.v_PrimerNombre + " " + c.v_SegundoNombre,
                                                Domiciliado = "1",
                                                DobleTributacion = e == null ? "" : e.v_Value2,
                                                Trabajador = c != null ? c.v_CodCliente + " " + c.v_ApePaterno + " " + c.v_ApeMaterno + " " + c.v_PrimerNombre + " " : "",
                                                TipoPlame = "TRABAJADOR - PRESTADOR DE SERVICIOS CON RENTAS DE CUARTA CATEGORIA",
                                                v_IdTrabajador = c.v_IdCliente,
                                                v_Mes = a.v_Mes,
                                                v_Periodo = a.v_Periodo,

                                                NroTipoPlame = 1,
                                            });
                    //if (!string.IsNullOrEmpty(Filtro))
                    //{
                    //    recibohonorarios = recibohonorarios.Where(Filtro);
                    //}
                    List<PlamePrestadorCuarta> ListaFinal = recibohonorarios.ToList();
                    if (!ListaFinal.Any())
                    {
                        PlamePrestadorCuarta objReporte = new PlamePrestadorCuarta();
                        objReporte.TipoPlame = "TRABAJADOR - PRESTADOR DE SERVICIOS CON RENTAS DE CUARTA CATEGORIA";
                        objReporte.Trabajador = "SIN OPERACIONES";
                        ListaFinal.Add(objReporte);
                    }
                    objOperationResult.Success = 1;
                    return ListaFinal;
                }
            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                return null;
            }

        }

        public List<planillanumeracionDto> ObtenerPlanillas(ref OperationResult pobjOperationResult, string pstrPeriodo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var data =
                        dbContext.planillanumeracion.Where(p => p.v_Periodo == pstrPeriodo && p.i_Eliminado == 0)
                            .ToDTOs().ToList();
                    pobjOperationResult.Success = 1;
                    return data;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ObtenerBusquedaPlanillas()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }


        //public List<KeyValueDTO> DevuelveTrabajadores()
        //{
        //    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
        //    {
        //        try
        //        {

        //            List<KeyValueDTO> entityCliente = (from a in dbContext.trabajador
        //                                 where a.i_Eliminado == 0
        //                                 select new KeyValueDTO
        //                                 {
        //                                     Id = a.v_IdTrabajador,
        //                                     Value1 = a.v_CodInterno,
        //                                 }).ToList();


        //            return entityCliente;
        //        }
        //        catch (Exception)
        //        {
        //            return null;
        //        }
        //    }

        //}

        public List<KeyValueDTO> DevuelveTrabajadores()
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    List<KeyValueDTO> EntityCliente = (from n in dbContext.trabajador
                                                       join o in dbContext.contratotrabajador on new { ct = n.v_IdTrabajador, eliminado = 0 } equals new { ct = o.v_IdTrabajador, eliminado = o.i_Eliminado.Value } into o_join
                                                       from o in o_join.DefaultIfEmpty()

                                                       where n.i_Eliminado == 0
                                                       select new KeyValueDTO { Id = n.v_IdTrabajador, Value1 = n.v_CodInterno, Value2 = o == null ? null : o.v_IdContrato }).ToList();

                    return EntityCliente;

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IEnumerable<planillavariablestrabajadorDto> ObtenerVariablesTrabajadorbyIdTrabajador(ref OperationResult objOperationResult, string IdCliente)
        {
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {

                var variablesTrab = (from a in dbContext.planillavariablestrabajador

                                     join b in dbContext.planillanumeracion on new { planillanum = a.i_IdPlanillaNumeracion, eliminado = 0 } equals new { planillanum = b.i_Id, eliminado = b.i_Eliminado.Value } into b_join

                                     from b in b_join.DefaultIfEmpty()

                                     where a.i_Eliminado == 0 && a.trabajador.cliente.v_IdCliente == IdCliente
                                     select new planillavariablestrabajadorDto
                                     {
                                         v_IdPlanillaVariablesTrabajador = b.v_Observaciones,

                                     }).ToList();
                return variablesTrab;



            }

        }



        #region Reportes

        public List<ReportePlanillaConceptos> ReportePlanillaConceptos(string Filtro)
        {
            try
            {

                // List<ReportePlanillaConceptos> ListaConceptos = new List<ReportePlanillaConceptos>();
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    var ListaConceptos = (from planilla in dbContext.planillaconceptos

                                          join TpIngresos in dbContext.datahierarchy on new { Grupo = 112, eliminado = 0, t = planilla.i_IdTipoPlanilla.Value } equals new { Grupo = TpIngresos.i_GroupId, eliminado = TpIngresos.i_IsDeleted.Value, t = TpIngresos.i_ItemId } into TpIngresos_join

                                          from TpIngresos in TpIngresos_join.DefaultIfEmpty()
                                          where planilla.i_Eliminado == 0

                                          select new ReportePlanillaConceptos
                                          {
                                              // v_IdConceptoPlanilla = planilla.v_IdConceptoPlanilla,
                                              i_IdTipoConcepto = planilla.i_IdTipoConcepto.Value,
                                              v_Codigo = planilla.v_Codigo,
                                              TipoPlanilla = "TIPO DE PLANILLA  : " + TpIngresos.v_Value2 + " " + TpIngresos.v_Value1,
                                              i_IdTipoPlanilla = planilla.i_IdTipoPlanilla.Value,
                                              v_Nombre = planilla.v_Nombre,
                                              //i_ConsiderarVacaciones = planilla.i_ConsiderarVacaciones == null ? 0 : planilla.i_ConsiderarVacaciones,
                                              //i_DescontarEsSalud = planilla.i_DescontarEsSalud == null ? 0 : planilla.i_DescontarEsSalud,
                                              //i_DescontarSCTR = planilla.i_DescontarSCTR == null ? 0 : planilla.i_DescontarSCTR,
                                              //i_DescontarSCTRPens = planilla.i_DescontarSCTRPens == null ? 0 : planilla.i_DescontarSCTRPens

                                          }
                                    ).AsQueryable();
                    //ListaConceptos = query.ToDTOs();


                    if (!string.IsNullOrEmpty(Filtro))
                    {
                        ListaConceptos = ListaConceptos.Where(Filtro);
                    }
                    //if (!string.IsNullOrEmpty(pstrSortExpression))
                    //{
                    //    ListaConceptos = ListaConceptos.OrderBy(pstrSortExpression);
                    //}
                    List<ReportePlanillaConceptos> objData = ListaConceptos.ToList();
                    //objOperationResult.Success = 1;
                    return objData;

                }

            }
            catch
            {

                return null;
            }
        }

        public List<ReporteTrabajadores> ReporteTrabajadores(string Filtro, string Ordenado, bool TodosTrabajadores)
        {

            List<ReporteTrabajadores> trabajadoresFinal = new List<ReporteTrabajadores>();
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                var trabajadores = (from a in dbContext.trabajador

                                    join b in dbContext.datahierarchy on new { Grupo = 142, eliminado = 0, Item = a.cliente.i_IdSexo.Value } equals new { Grupo = b.i_GroupId, eliminado = b.i_IsDeleted.Value, Item = b.i_ItemId } into b_join

                                    from b in b_join.DefaultIfEmpty()

                                    join c in dbContext.datahierarchy on new { Grupo = 113, eliminado = 0, Item = a.i_IdTipoVia.Value } equals new { Grupo = c.i_GroupId, eliminado = c.i_IsDeleted.Value, Item = c.i_ItemId } into c_join

                                    from c in c_join.DefaultIfEmpty()

                                    where a.i_Eliminado == 0
                                    select new ReporteTrabajadores
                                    {
                                        v_Codigo = a.cliente.v_CodCliente.Trim(),
                                        NombreTrabajador = a.cliente.v_ApePaterno + " " + a.cliente.v_ApeMaterno + " " + a.cliente.v_PrimerNombre + " " + a.cliente.v_SegundoNombre,
                                        v_NroDocIdentificacion = a.cliente.v_NroDocIdentificacion,
                                        t_FechaAlta = a.t_FechaAlta.Value,
                                        t_FechaCese = a.t_FechaCese.Value,
                                        v_Direccion = (c.v_Value1 + " " + a.v_NombreVia + " ") +
                                       (a.v_NumeroDomicilio == string.Empty ? "" : "NRO " + a.v_NumeroDomicilio.Trim()),
                                        i_Activo = a.cliente.i_Activo.Value,
                                        Sexo = b.v_Value1,
                                        t_fechaNacimiento = a.cliente.t_FechaNacimiento.Value,
                                        Activo = a.cliente.i_Activo == 1 ? "✔" : "X",


                                    });

                if (!string.IsNullOrEmpty(Filtro) && !TodosTrabajadores)
                {
                    trabajadores = trabajadores.Where(Filtro);
                }

                if (!string.IsNullOrEmpty(Ordenado))
                {
                    trabajadores = trabajadores.OrderBy(Ordenado);
                }
                trabajadoresFinal = trabajadores.ToList();
            }
            List<ReporteTrabajadores> ListaTrabajadores = trabajadoresFinal.ToList();
            return ListaTrabajadores;
        }

        public List<ReporteTrabjadoresAdicionales> ReporteTrabajadoresAdicionales(string Filtro)
        {
            List<ReporteTrabjadoresAdicionales> trabajadoresFinal = new List<ReporteTrabjadoresAdicionales>();
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                var trabajadores = (from a in dbContext.trabajador

                                    join b in dbContext.datahierarchy on new { Grupo = 142, eliminado = 0, Item = a.cliente.i_IdSexo.Value } equals new { Grupo = b.i_GroupId, eliminado = b.i_IsDeleted.Value, Item = b.i_ItemId } into b_join

                                    from b in b_join.DefaultIfEmpty()

                                    join c in dbContext.datahierarchy on new { Grupo = 113, eliminado = 0, Item = a.i_IdTipoVia.Value } equals new { Grupo = c.i_GroupId, eliminado = c.i_IsDeleted.Value, Item = c.i_ItemId } into c_join
                                    from c in c_join.DefaultIfEmpty()

                                    join d in dbContext.regimenpensionariotrabajador on new { IdTrabajador = a.v_IdTrabajador, eliminado = 0, Vigente = 1 } equals new { IdTrabajador = d.v_IdTrabajador, eliminado = d.i_Eliminado.Value, Vigente = d.i_RegimenVigente.Value } into d_join

                                    from d in d_join.DefaultIfEmpty()

                                    join e in dbContext.datahierarchy on new { Grupo = 125, eliminado = 0, Item = d.i_IdRegimenPensionario.Value } equals new { Grupo = e.i_GroupId, eliminado = e.i_IsDeleted.Value, Item = e.i_ItemId } into e_join

                                    from e in e_join.DefaultIfEmpty()

                                    join f in dbContext.areaslaboratrabajador on new { IdTrabajador = a.v_IdTrabajador, eliminado = 0, Vigente = 1 } equals new { IdTrabajador = f.v_IdTrabajador, eliminado = f.i_Eliminado.Value, Vigente = f.i_AreaVigente.Value } into f_join

                                    from f in f_join.DefaultIfEmpty()

                                    join g in dbContext.datahierarchy on new { Grupo = 31, eliminado = 0, Item = f.i_IdCentroCosto.Value } equals new { Grupo = g.i_GroupId, eliminado = g.i_IsDeleted.Value, Item = g.i_ItemId } into g_join

                                    from g in g_join.DefaultIfEmpty()

                                    join h in dbContext.contratotrabajador on new { IdTrabajador = a.v_IdTrabajador, eliminado = 0, Vigente = 1 } equals new { IdTrabajador = h.v_IdTrabajador, eliminado = h.i_Eliminado.Value, Vigente = h.i_ContratoVigente.Value } into h_join

                                    from h in h_join.DefaultIfEmpty()

                                    where a.i_Eliminado == 0
                                    select new ReporteTrabjadoresAdicionales
                                    {
                                        v_Codigo = a.cliente.v_CodCliente.Trim(),
                                        NombreTrabajador = a.cliente.v_ApePaterno + " " + a.cliente.v_ApeMaterno + " " + a.cliente.v_PrimerNombre + " " + a.cliente.v_SegundoNombre,
                                        v_NroDocIdentificacion = a.cliente.v_NroDocIdentificacion,
                                        ruc = a.v_NroRuc == null || a.v_NroRuc == string.Empty ? a.cliente.v_CodCliente : a.v_NroRuc,
                                        afp = d == null ? "** NO TIENE REGIMEN PENSIONARIO **" : e.v_Value2 == null || e.v_Value2 == string.Empty ? "NO TIENE CÓDIGO R.P." : e.v_Value1,
                                        Ccosto = f == null ? "** NO TIENE  CENTRO COSTO **" : g.v_Value2.Trim() + "   " + g.v_Value1.Trim(),
                                        NroContrato = h == null ? "** NO TIENE CONTRATO **" : h.v_NroContrato,
                                        Ingresos = h.d_Importe == null ? 0 : h.d_Importe.Value,
                                        i_IdTipoContrato = h.i_IdTipoContrato,
                                        i_idTipoPlanilla = h.i_IdTipoPlanilla,



                                    });


                if (!string.IsNullOrEmpty(Filtro))
                {
                    trabajadores = trabajadores.Where(Filtro);
                }

                //if (!string.IsNullOrEmpty(Ordenado))
                //{
                //    trabajadores = trabajadores.OrderBy(Ordenado);
                //}
                trabajadoresFinal = trabajadores.ToList();
            }
            List<ReporteTrabjadoresAdicionales> ListaTrabajadores = trabajadoresFinal.ToList();
            return ListaTrabajadores;

        }

        public List<ReporteTrabjadoresAdicionalesAFP> ReporteTrabajadoresAdicionalesAFP(string Filtro)
        {
            List<ReporteTrabjadoresAdicionalesAFP> trabajadoresFinal = new List<ReporteTrabjadoresAdicionalesAFP>();
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                var trabajadores = (from a in dbContext.trabajador

                                    join d in dbContext.regimenpensionariotrabajador on new { IdTrabajador = a.v_IdTrabajador, eliminado = 0 } equals new { IdTrabajador = d.v_IdTrabajador, eliminado = d.i_Eliminado.Value } into d_join

                                    from d in d_join.DefaultIfEmpty()

                                    join e in dbContext.datahierarchy on new { Grupo = 125, eliminado = 0, Item = d.i_IdRegimenPensionario.Value } equals new { Grupo = e.i_GroupId, eliminado = e.i_IsDeleted.Value, Item = e.i_ItemId } into e_join

                                    from e in e_join.DefaultIfEmpty()

                                    //join f in dbContext.areaslaboratrabajador on new { IdTrabajador = a.v_IdTrabajador, eliminado = 0} equals new { IdTrabajador = f.v_IdTrabajador, eliminado = f.i_Eliminado.Value} into f_join

                                    //from f in f_join.DefaultIfEmpty()

                                    //join g in dbContext.datahierarchy on new { Grupo = 31, eliminado = 0, Item = f.i_IdCentroCosto.Value } equals new { Grupo = g.i_GroupId, eliminado = g.i_IsDeleted.Value, Item = g.i_ItemId } into g_join

                                    //from g in g_join.DefaultIfEmpty()

                                    join h in dbContext.contratotrabajador on new { IdTrabajador = a.v_IdTrabajador, eliminado = 0, Vigente = 1 } equals new { IdTrabajador = h.v_IdTrabajador, eliminado = h.i_Eliminado.Value, Vigente = h.i_ContratoVigente.Value } into h_join

                                    from h in h_join.DefaultIfEmpty()
                                    where a.i_Eliminado == 0
                                    select new ReporteTrabjadoresAdicionalesAFP
                                    {

                                        Trabajador = " TRABAJADOR : " + "CÓDIGO : " + a.cliente.v_CodCliente.Trim() + " NOMBRES : " + a.cliente.v_ApePaterno + " " + a.cliente.v_ApeMaterno + " " + a.cliente.v_PrimerNombre + " " + a.cliente.v_SegundoNombre,
                                        FechaAfiliacion = d.t_FechaInscripcion.Value,
                                        NroCuenta = d.v_NroCuenta,
                                        NroCussp = d.v_NroCussp,
                                        afp = d == null ? "** NO TIENE REGIMEN PENSIONARIO **" : e.v_Value2 == null || e.v_Value2 == string.Empty ? "NO TIENE CÓDIGO R.P." : e.v_Value2 + "                                    " + e.v_Value1,
                                        regimenactual = d.i_RegimenVigente == 1 ? "✔" : "X",
                                        i_IdTipoContrato = h.i_IdTipoContrato,
                                        i_idTipoPlanilla = h.i_IdTipoPlanilla,
                                        //ruc = a.v_NroRuc == null || a.v_NroRuc == string.Empty ? a.cliente.v_CodCliente : a.v_NroRuc,

                                        //Ccosto = f == null ? "** NO TIENE  CENTRO COSTO **" : g.v_Value2.Trim() + "   " + g.v_Value1.Trim(),
                                        //NroContrato = h == null ? "** NO TIENE CONTRATO **" : h.v_NroContrato,
                                        //Ingresos = h.d_Importe == null ? 0 : h.d_Importe.Value,
                                        //i_IdTipoContrato = h.i_IdTipoContrato,
                                        //i_idTipoPlanilla = h.i_IdTipoPlanilla,
                                        //TipoContrato = i == null ? "" : "TIPO DE CONTRATO : " + i.v_Value1,
                                        //TipoPlanilla = j == null ? "" : "TIPO DE PLANILLA : " + j.v_Value1,


                                    });


                if (!string.IsNullOrEmpty(Filtro))
                {
                    trabajadores = trabajadores.Where(Filtro);
                }

                //if (!string.IsNullOrEmpty(Ordenado))
                //{
                //    trabajadores = trabajadores.OrderBy(Ordenado);
                //}
                trabajadoresFinal = trabajadores.ToList();
            }
            List<ReporteTrabjadoresAdicionalesAFP> ListaTrabajadores = trabajadoresFinal.ToList();
            return ListaTrabajadores;

        }


        public List<ReporteTrabjadoresAdicionalesCcosto> ReporteTrabajadoresAdicionalesCcosto(string Filtro)
        {
            List<ReporteTrabjadoresAdicionalesCcosto> trabajadoresFinal = new List<ReporteTrabjadoresAdicionalesCcosto>();
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                var trabajadores = (from a in dbContext.trabajador


                                    join f in dbContext.areaslaboratrabajador on new { IdTrabajador = a.v_IdTrabajador, eliminado = 0 } equals new { IdTrabajador = f.v_IdTrabajador, eliminado = f.i_Eliminado.Value } into f_join

                                    from f in f_join.DefaultIfEmpty()

                                    join g in dbContext.datahierarchy on new { Grupo = 31, eliminado = 0, Item = f.i_IdCentroCosto.Value } equals new { Grupo = g.i_GroupId, eliminado = g.i_IsDeleted.Value, Item = g.i_ItemId } into g_join

                                    from g in g_join.DefaultIfEmpty()

                                    join h in dbContext.contratotrabajador on new { IdTrabajador = a.v_IdTrabajador, eliminado = 0, Vigente = 1 } equals new { IdTrabajador = h.v_IdTrabajador, eliminado = h.i_Eliminado.Value, Vigente = h.i_ContratoVigente.Value } into h_join

                                    from h in h_join.DefaultIfEmpty()
                                    where a.i_Eliminado == 0
                                    select new ReporteTrabjadoresAdicionalesCcosto
                                    {

                                        Trabajador = " TRABAJADOR : " + "CÓDIGO : " + a.cliente.v_CodCliente.Trim() + " NOMBRES : " + a.cliente.v_ApePaterno + " " + a.cliente.v_ApeMaterno + " " + a.cliente.v_PrimerNombre + " " + a.cliente.v_SegundoNombre,
                                        Ccosto = f == null ? "** NO TIENE  CENTRO COSTO **" : g.v_Value2.Trim() + "             " + g.v_Value1.Trim(),
                                        Cargo = f == null ? "** NO TIENE  CENTRO COSTO **" : f.v_Cargo,
                                        FechaInicio = f.v_FechaInicio,
                                        FechaFin = f.v_FechaFin,
                                        centrocostoActual = f.i_AreaVigente == 1 ? "✔" : "X",
                                        i_IdTipoContrato = h.i_IdTipoContrato,
                                        i_idTipoPlanilla = h.i_IdTipoPlanilla,

                                    });


                if (!string.IsNullOrEmpty(Filtro))
                {
                    trabajadores = trabajadores.Where(Filtro);
                }

                //if (!string.IsNullOrEmpty(Ordenado))
                //{
                //    trabajadores = trabajadores.OrderBy(Ordenado);
                //}
                trabajadoresFinal = trabajadores.ToList();
            }
            List<ReporteTrabjadoresAdicionalesCcosto> ListaTrabajadores = trabajadoresFinal.ToList();
            return ListaTrabajadores;

        }


        public List<ConceptosResultantesPlanilla> ObtenerListaConceptosPlanilla(int PlanillaNumeracion, string Origen = null)
        {
            List<ConceptosResultantesPlanilla> ListaConceptosPlanilla = new List<ConceptosResultantesPlanilla>();

            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                var conceptospla = (from a in dbContext.planillacalculo
                                    join b in dbContext.planillaconceptos on new { IdConcepto = a.v_IdConceptoPlanilla, eliminado = 0 } equals new { IdConcepto = b.v_IdConceptoPlanilla, eliminado = b.i_Eliminado.Value } into b_join

                                    from b in b_join.DefaultIfEmpty()
                                    where a.i_IdPlanillaNumeracion == PlanillaNumeracion

                                    select new ConceptosResultantesPlanilla
                                    {

                                        CodigoConcepto = b.v_Codigo,
                                        NombreConcepto = b.v_Nombre,
                                        TipoConcepto = b.i_IdTipoConcepto.Value,


                                    }).ToList();


                ListaConceptosPlanilla = conceptospla.GroupBy(x => x.CodigoConcepto).Select(x => x.FirstOrDefault()).ToList();

            }
            if (Origen == null)
            {

                ConceptosResultantesPlanilla obj = new ConceptosResultantesPlanilla();
                obj.NombreConcepto = "TOTAL INGRESOS";
                obj.CodigoConcepto = "TOTALINGRESOS";
                obj.TipoConcepto = 1.1M;
                ListaConceptosPlanilla.Add(obj);
                obj = new ConceptosResultantesPlanilla();
                obj.NombreConcepto = "TOTAL DESCUENTOS";
                obj.CodigoConcepto = "TOTALDESCUENTOS";
                obj.TipoConcepto = 2.1M;
                ListaConceptosPlanilla.Add(obj);
                obj = new ConceptosResultantesPlanilla();
                obj.NombreConcepto = "TOTAL APORTACIONES";
                obj.CodigoConcepto = "TOTALAPORTACIONES";
                obj.TipoConcepto = 3.1M;
                ListaConceptosPlanilla.Add(obj);

                obj = new ConceptosResultantesPlanilla();
                obj.NombreConcepto = "NETO PAGAR";
                obj.CodigoConcepto = "NETOPAGAR";
                obj.TipoConcepto = 4;
                ListaConceptosPlanilla.Add(obj);
            }
            else
            {

                ListaConceptosPlanilla = ListaConceptosPlanilla.Where(x => x.TipoConcepto == (int)TipoConceptosPlanillas.Descuentos).ToList();
            }
            ListaConceptosPlanilla = ListaConceptosPlanilla.OrderBy(x => x.TipoConcepto).ToList();
            return ListaConceptosPlanilla;

        }



        public List<ConceptosResultantesPlanilla> ObtenerListaConceptosPlanillaxTrabajador(string IdTrabajador)
        {
            List<ConceptosResultantesPlanilla> ListaConceptosPlanilla = new List<ConceptosResultantesPlanilla>();

            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                var conceptospla = (from a in dbContext.planillacalculo
                                    join b in dbContext.planillaconceptos on new { IdConcepto = a.v_IdConceptoPlanilla, eliminado = 0 } equals new { IdConcepto = b.v_IdConceptoPlanilla, eliminado = b.i_Eliminado.Value } into b_join

                                    from b in b_join.DefaultIfEmpty()
                                    where a.v_IdTrabajador == IdTrabajador

                                    select new ConceptosResultantesPlanilla
                                    {

                                        CodigoConcepto = b.v_Codigo,
                                        NombreConcepto = b.v_Nombre,
                                        TipoConcepto = b.i_IdTipoConcepto.Value,


                                    }).ToList();

                // ListaConceptosPlanilla = planillaconceptosAssembler.ToDTOs(conceptospla);    
                ListaConceptosPlanilla = conceptospla.GroupBy(x => x.CodigoConcepto).Select(x => x.FirstOrDefault()).ToList();

            }

            ConceptosResultantesPlanilla obj = new ConceptosResultantesPlanilla();
            obj.NombreConcepto = "TOTAL INGRESOS";
            obj.CodigoConcepto = "TOTALINGRESOS";
            obj.TipoConcepto = 1.1M;
            ListaConceptosPlanilla.Add(obj);
            obj = new ConceptosResultantesPlanilla();
            obj.NombreConcepto = "TOTAL DESCUENTOS";
            obj.CodigoConcepto = "TOTALDESCUENTOS";
            obj.TipoConcepto = 2.1M;
            ListaConceptosPlanilla.Add(obj);
            obj = new ConceptosResultantesPlanilla();
            obj.NombreConcepto = "TOTAL APORTACIONES";
            obj.CodigoConcepto = "TOTALAPORTACIONES";
            obj.TipoConcepto = 3.1M;
            ListaConceptosPlanilla.Add(obj);

            obj = new ConceptosResultantesPlanilla();
            obj.NombreConcepto = "NETO PAGAR";
            obj.CodigoConcepto = "NETOPAGAR";
            obj.TipoConcepto = 4;
            ListaConceptosPlanilla.Add(obj);

            ListaConceptosPlanilla = ListaConceptosPlanilla.OrderBy(x => x.TipoConcepto).ToList();
            return ListaConceptosPlanilla;

        }


        public List<planillacalculo> ObtenerListaPlanillaCalculoAssembler(ref  OperationResult objOperationResult)
        {
            try
            {
                objOperationResult.Success = 1;
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    return dbContext.planillacalculo.ToList();
                }
            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "PlanillaBL.ObtenerPlanillaCalculoAssembler()";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }
        }

        public List<planillaconceptos> ObtenerListaConceptosAssembler()
        {
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {

                return dbContext.planillaconceptos.ToList();
            }
        }





        public List<ReportePlanillaOficial> ReportePlanillaOficial(ref  OperationResult objOperationResult, int Filtro)
        {

            try
            {
                List<ReportePlanillaOficial> Agrupados = new List<ReportePlanillaOficial>();
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    var planilla = (from a in dbContext.planillacalculo
                                    join b in dbContext.contratotrabajador on new { IdContrato = a.v_IdContrato } equals new { IdContrato = b.v_IdContrato } into b_join
                                    from b in b_join.DefaultIfEmpty()
                                    join c in dbContext.trabajador on new { IdTrabajador = b.v_IdTrabajador, eliminado = 0 } equals new { IdTrabajador = c.v_IdTrabajador, eliminado = c.i_Eliminado.Value } into c_join
                                    from c in c_join.DefaultIfEmpty()
                                    join d in dbContext.cliente on new { IdCliente = c.v_IdCliente, eliminado = 0 } equals new { IdCliente = d.v_IdCliente, eliminado = d.i_Eliminado.Value } into d_join
                                    from d in d_join.DefaultIfEmpty()
                                    join e in dbContext.areaslaboratrabajador on new { IdArea = c.v_IdTrabajador, eliminado = 0, vigente = 1 } equals new { IdArea = e.v_IdTrabajador, eliminado = e.i_Eliminado.Value, vigente = e.i_AreaVigente.Value } into e_join
                                    from e in e_join.DefaultIfEmpty()
                                    join f in dbContext.regimenpensionariotrabajador on new { IdReg = c.v_IdTrabajador, eliminado = 0, vigente = 1 } equals new { IdReg = f.v_IdTrabajador, eliminado = f.i_Eliminado.Value, vigente = f.i_RegimenVigente.Value } into f_join
                                    from f in f_join.DefaultIfEmpty()
                                    join g in dbContext.planillavariablestrabajador on new { Trabajador = c.v_IdTrabajador, eliminado = 0, idPlanilla = Filtro } equals new { Trabajador = g.v_IdTrabajador, eliminado = g.i_Eliminado.Value, idPlanilla = g.i_IdPlanillaNumeracion } into g_join
                                    from g in g_join.DefaultIfEmpty()
                                    join h in dbContext.datahierarchy on new { Grupo = 31, eliminado = 0, Item = e.i_IdCentroCosto.Value } equals new { Grupo = h.i_GroupId, eliminado = h.i_IsDeleted.Value, Item = h.i_ItemId } into h_join
                                    from h in h_join.DefaultIfEmpty()

                                    join i in dbContext.datahierarchy on new { Grupo = 132, eliminado = 0, Item = d.i_IdTipoIdentificacion.Value } equals new { Grupo = i.i_GroupId, eliminado = i.i_IsDeleted.Value, Item = i.i_ItemId } into i_join

                                    from i in i_join.DefaultIfEmpty()

                                    join k in dbContext.datahierarchy on new { Grupo = 111, eliminado = 0, Item = c.i_EntidadCuentaAbono.Value } equals new { Grupo = k.i_GroupId, eliminado = k.i_IsDeleted.Value, Item = k.i_ItemId } into k_join
                                    from k in k_join.DefaultIfEmpty()

                                    join l in dbContext.datahierarchy on new { Grupo = 125, eliminado = 0, Item = f.i_IdRegimenPensionario.Value } equals new { Grupo = l.i_GroupId, eliminado = l.i_IsDeleted.Value, Item = l.i_ItemId } into l_join

                                    from l in l_join.DefaultIfEmpty()

                                    join o in dbContext.datahierarchy on new { GrupoRegimenL = 115, eliminado = 0, Item = c.i_RegimenLaboral.Value } equals new { GrupoRegimenL = o.i_GroupId, eliminado = o.i_IsDeleted.Value, Item = o.i_ItemId } into o_join
                                    from o in o_join.DefaultIfEmpty()


                                    join p in dbContext.datahierarchy on new { TipContrato = 136, eliminado = 0, Item = b.i_IdTipoContrato.Value } equals new { TipContrato = p.i_GroupId, eliminado = p.i_IsDeleted.Value, Item = p.i_ItemId } into p_join
                                    from p in p_join.DefaultIfEmpty()

                                    join m in dbContext.planillaconceptos on new { IdConcepto = a.v_IdConceptoPlanilla, eliminado = 0 } equals new { IdConcepto = m.v_IdConceptoPlanilla, eliminado = m.i_Eliminado.Value } into m_join

                                    from m in m_join.DefaultIfEmpty()



                                    where a.i_IdPlanillaNumeracion == Filtro

                                    select new ReportePlanillaOficial
                                    {

                                        CodigoTrabajador = d != null ? d.v_CodCliente.Trim() : "",
                                        Trabajador = d != null ? (d.v_ApePaterno + " " + d.v_ApeMaterno + " " + d.v_PrimerNombre + " " + d.v_SegundoNombre).Trim() : "** TRABAJADOR NO EXISTE **",
                                        Ccosto = h == null ? "" : h.v_Value2 + " " + h.v_Value1,
                                        FechaIngreso = c.t_FechaAlta,
                                        TipoDocumentoTrabajador = i != null ? i.v_Value1 : "",
                                        NroDocumentoTrabajador = d != null ? d.v_NroDocIdentificacion : "",
                                        CodBanco = k != null ? k.v_Value2 : "",
                                        Banco = k != null ? k.v_Value1 : "",
                                        CuentaBancaria = c.v_NroCuentaAbono,
                                        RegimenPensario = l != null ? l.v_Value1 : "",
                                        v_IdTrabajador = c.v_IdTrabajador,
                                        Cargo = e != null ? e.v_Cargo : "",
                                        NroCussp = f != null ? f.v_NroCussp : "",
                                        NroEssalud = c != null ? c.v_NroEsSalud : "",
                                        DiasTrabajados = g == null ? 0 : g.i_DiasLaborados.Value,
                                        HorasTrabajadas = g == null ? 0 : g.d_HorasLaboradosBP.Value,
                                        i_NumeroPlanilla = a.i_IdPlanillaNumeracion.Value,
                                        RegimenLaboral = o.v_Value1,
                                        TipoContrato = p.v_Value1,



                                    }).ToList().Select(n => new ReportePlanillaOficial
                                    {
                                        CodigoTrabajador = n.CodigoTrabajador,
                                        Trabajador = n.Trabajador,
                                        Ccosto = n.Ccosto,
                                        FechaIngreso = n.FechaIngreso,
                                        TipoDocumentoTrabajador = n.TipoDocumentoTrabajador,
                                        NroDocumentoTrabajador = n.NroDocumentoTrabajador,
                                        CodBanco = n.CodBanco,
                                        Banco = n.Banco,
                                        CuentaBancaria = n.CuentaBancaria,
                                        RegimenPensario = n.RegimenPensario,
                                        v_IdTrabajador = n.v_IdTrabajador,
                                        Cargo = n.Cargo,
                                        NroCussp = n.NroCussp,
                                        NroEssalud = n.NroEssalud,
                                        HorasTrabajadasS = n.DiasTrabajados.ToString() + " Dia(s)" + " " + n.HorasTrabajadas.ToString() + " Hora(s)",
                                        HorasExtras = 0,
                                        RegimenLaboral = n.RegimenLaboral,
                                        TipoContrato = n.TipoContrato,
                                    }).ToList();

                    var FFFFF = planilla.Where(o => o.CodigoTrabajador == "T070");
                    Agrupados = planilla.GroupBy(x => new { x.v_IdTrabajador }).Select(group => group.FirstOrDefault()).ToList();

                }


                objOperationResult.Success = 1;
                return Agrupados.OrderBy(x => x.CodigoTrabajador).ToList();
            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                return null;


            }



        }

        public decimal ObtenerCantidadConceptos(string IdTrabajador, int NumeroPlanilla, string CodigoConcepto)
        {



            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                var ConceptoSueldoBasico = (from a in dbContext.planillaconceptos
                                            where a.v_Codigo == CodigoConcepto && a.i_Eliminado == 0

                                            select a).FirstOrDefault();
                if (ConceptoSueldoBasico != null)
                {

                    var SueldoBasico = (from a in dbContext.planillacalculo

                                        where a.v_IdTrabajador == IdTrabajador && a.i_IdPlanillaNumeracion == NumeroPlanilla && a.v_IdConceptoPlanilla == ConceptoSueldoBasico.v_IdConceptoPlanilla
                                        select new
                                        {

                                            d_Importe = a.d_Importe.Value

                                        }).FirstOrDefault();

                    if (SueldoBasico != null)
                    {

                        return SueldoBasico.d_Importe;
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

        public string ObtenerCantidadConceptosPlanillaOficial(string IdTrabajador, int NumeroPlanilla, string CodigoConcepto, List<planillaconceptos> planillaconceptos, List<planillacalculo> planillacalculo)
        {

            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                var ConceptoSueldoBasico = (from a in planillaconceptos
                                            where a.v_Codigo == CodigoConcepto && a.i_Eliminado == 0

                                            select a).FirstOrDefault();
                if (ConceptoSueldoBasico != null)
                {

                    var SueldoBasico = (from a in planillacalculo

                                        where a.v_IdTrabajador == IdTrabajador && a.i_IdPlanillaNumeracion == NumeroPlanilla && a.v_IdConceptoPlanilla == ConceptoSueldoBasico.v_IdConceptoPlanilla
                                        select new
                                        {

                                            d_Importe = a.d_Importe.Value

                                        }).FirstOrDefault();

                    if (SueldoBasico != null)
                    {

                        return SueldoBasico.d_Importe.ToString();
                    }
                    else
                    {

                        return "0";
                    }

                }
                else
                {
                    return "0";
                }

            }

        }

        public decimal ObtenerTotalesxTrabajadorPlanilla(string IdTrabajador, int NumeroPlanilla, string Tipo, List<planillacalculo> planillacalculo)
        {
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {

                var Total = (from n in planillacalculo
                             where n.Tipo == Tipo && n.v_IdTrabajador == IdTrabajador && n.i_IdPlanillaNumeracion == NumeroPlanilla

                             select n).ToList();
                if (Total.Any())
                {

                    return Total.Sum(x => x.d_Importe).Value;
                }
                else
                {
                    return 0;
                }


            }

        }

        public planillavariablestrabajadorDto ObtenerDetallesAdicionales(string Filtro)
        {
            planillavariablestrabajadorDto ListaVariables = new planillavariablestrabajadorDto();
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                var planillavariable = (from a in dbContext.planillavariablestrabajador
                                        where a.i_Eliminado == 0

                                        select a).AsQueryable();
                if (!string.IsNullOrEmpty(Filtro))
                {
                    planillavariable = planillavariable.Where(Filtro);
                }
                ListaVariables = planillavariable.ToDTOs().FirstOrDefault();
            }


            return ListaVariables;
        }


        public decimal ObtenerNetoPagar(string IdTrabajador, int NumeroPlanilla, List<planillacalculo> planillacalculo)
        {
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {

                var Total = (from n in planillacalculo
                             where n.v_IdTrabajador == IdTrabajador && n.i_IdPlanillaNumeracion == NumeroPlanilla

                             select n).ToList();
                if (Total.Any())
                {

                    return Total.Where(x => x.Tipo == "I").Sum(X => X.d_Importe.Value) - Total.Where(x => x.Tipo == "D").Sum(X => X.d_Importe.Value);
                }
                else
                {
                    return 0;
                }
            }

        }

        public decimal ObtenerNetoPagarAfpNet(string idTrabajador, int numeroPlanilla)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var planillaTrabajador = dbContext.planillacalculo.Where(
                    n => n.v_IdTrabajador == idTrabajador && n.i_IdPlanillaNumeracion == numeroPlanilla).ToList();

                if (!planillaTrabajador.Any()) return 0m;
                var planilla = dbContext.planillanumeracion.FirstOrDefault(p => p.i_Id == numeroPlanilla);
                if (planilla == null) return 0m;

                var afectosAfp = dbContext.planillaafectacionesgenerales.Where(p => (p.i_AFP_Afecto ?? 0) == 1 && p.i_Eliminado == 0
                                 && p.v_Mes.Equals(planilla.v_Mes) && p.v_Periodo.Equals(planilla.v_Periodo))
                                 .Select(o => o.v_IdConceptoPlanilla);

                var ingresos = planillaTrabajador.Where(p => afectosAfp.Contains(p.v_IdConceptoPlanilla)
                    && p.Tipo.Equals("I")).Sum(s => s.d_Importe ?? 0);

                var descuentos = planillaTrabajador.Where(p => afectosAfp.Contains(p.v_IdConceptoPlanilla)
                    && p.Tipo.Equals("D")).Sum(s => s.d_Importe ?? 0);

                return ingresos - descuentos;
            }
        }

        public regimenpensionariotrabajador ObtenerAfpVigente(string pstrperiodo, string pstrMes, string pstrIdTrabajador, List<trabajador> Trabajador)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var nroDias = DateTime.DaysInMonth(Int32.Parse(pstrperiodo), Int32.Parse(pstrMes));
                var fechaProceso = DateTime.Parse(nroDias + "/" + pstrMes + "/" + pstrperiodo);
                var trabajador = Trabajador.FirstOrDefault(p => p.v_IdTrabajador == pstrIdTrabajador);

                if (trabajador != null)
                {
                    var regimenes = trabajador.regimenpensionariotrabajador.Where(o => o.i_Eliminado == 0 && o.v_IdTrabajador == pstrIdTrabajador).ToList()
                                                                           .OrderBy(p => p.t_FechaInscripcion.Value.Year).ThenBy(o => o.t_FechaInscripcion.Value.Month).ToList();

                    var afp = regimenes.LastOrDefault(g => g.t_FechaInscripcion.Value.Date <= fechaProceso);

                    return afp;
                }

                return null;
            }
        }

        public List<ReportePlanillaAFP> ReportePlanillaAFP(ref OperationResult objOperationResult, string Filtro, int pintIdPlanilla)
        {


            try
            {


                objOperationResult.Success = 1;
                List<ReportePlanillaAFP> Agrupados = new List<ReportePlanillaAFP>();
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    OperationResult pobjOperationResult = new OperationResult();


                    var entityPlanilla = dbContext.planillanumeracion.FirstOrDefault(p => p.i_Id == pintIdPlanilla && p.i_Eliminado == 0);
                    if (entityPlanilla == null) throw new ArgumentNullException("Planilla no existe!");
                    if (entityPlanilla.d_SueldoMinimo == null) throw new ArgumentNullException("El Sueldo Base de la Planilla creada no es válido.!");
                    var periodo = entityPlanilla.v_Periodo;
                    var mes = entityPlanilla.v_Mes;
                    var afps = dbContext.datahierarchy.Where(o => o.i_GroupId == 125 && o.i_IsDeleted == 0).ToList();
                    var Trabajadores = dbContext.trabajador.ToList();
                    var planilla = (from a in dbContext.planillacalculo
                                    join b in dbContext.contratotrabajador on new { IdContrato = a.v_IdContrato } equals new { IdContrato = b.v_IdContrato } into b_join
                                    from b in b_join.DefaultIfEmpty()
                                    join c in dbContext.trabajador on new { IdTrabajador = b.v_IdTrabajador, eliminado = 0 } equals new { IdTrabajador = c.v_IdTrabajador, eliminado = c.i_Eliminado.Value } into c_join
                                    from c in c_join.DefaultIfEmpty()
                                    join d in dbContext.cliente on new { IdCliente = c.v_IdCliente, eliminado = 0 } equals new { IdCliente = d.v_IdCliente, eliminado = d.i_Eliminado.Value } into d_join
                                    from d in d_join.DefaultIfEmpty()
                                    join e in dbContext.areaslaboratrabajador on new { IdArea = c.v_IdTrabajador, eliminado = 0, vigente = 1 } equals new { IdArea = e.v_IdTrabajador, eliminado = e.i_Eliminado.Value, vigente = e.i_AreaVigente.Value } into e_join
                                    from e in e_join.DefaultIfEmpty()

                                    join g in dbContext.planillavariablestrabajador on new { Trabajador = c.v_IdTrabajador, eliminado = 0 } equals new { Trabajador = g.v_IdTrabajador, eliminado = g.i_Eliminado.Value } into g_join
                                    from g in g_join.DefaultIfEmpty()


                                    join i in dbContext.datahierarchy on new { Ccosto = e.i_IdCentroCosto.Value, eliminado = 0, Grupo = 31 } equals new { Ccosto = i.i_ItemId, eliminado = i.i_IsDeleted.Value, Grupo = i.i_GroupId } into i_join
                                    from i in i_join.DefaultIfEmpty()
                                    where a.i_IdPlanillaNumeracion == pintIdPlanilla
                                    select new
                                    {


                                        Trabajador = d != null ? d.v_CodCliente.Trim() + "      " + (d.v_ApePaterno + " " + d.v_ApeMaterno + " " + d.v_PrimerNombre + " " + d.v_SegundoNombre).Trim() : "** TRABAJADOR NO EXISTE **",

                                        v_IdTrabajador = c.v_IdTrabajador,
                                        Dni = d != null ? d.v_NroDocIdentificacion : "",
                                        i_NumeroPlanilla = a.i_IdPlanillaNumeracion == null ? 0 : a.i_IdPlanillaNumeracion.Value,

                                        Ccosto = i != null ? "CENTRO DE COSTO :" + i.v_Value2 + " " + i.v_Value1 : "** NO EXISTE CENTRO COSTO **",
                                        i_IdPlanillaNumeracion = a.i_IdPlanillaNumeracion.Value,
                                        i_IdCentroCosto = e != null ? e.i_IdCentroCosto.Value : 0,
                                        TipoPlanilla = b == null ? 0 : b.i_IdTipoPlanilla == null ? 0 : b.i_IdTipoPlanilla.Value,
                                    }).ToList().Select(o =>
                                    {

                                        var afpVigente = ObtenerAfpVigente(periodo, mes, o.v_IdTrabajador, Trabajadores);
                                        var uu = afps.Where(h => h.i_ItemId == afpVigente.i_IdRegimenPensionario).FirstOrDefault();
                                        return new ReportePlanillaAFP
                                        {

                                            Afp = uu != null ? uu.v_Value2 + " " + uu.v_Value1 : "NO TIENE REGIMEN PENSIONARIO",
                                            NroCussp = afpVigente.v_NroCussp,
                                            Trabajador = o.Trabajador,
                                            v_IdTrabajador = o.v_IdTrabajador,
                                            Dni = o.Dni,
                                            i_NumeroPlanilla = o.i_NumeroPlanilla,
                                            //TipoRegimen = o.TipoRegimen ,
                                            TipoRegimen = afpVigente == null ? 0 : afpVigente.i_IdRegimenPensionario == null ? 0 : afpVigente.i_IdRegimenPensionario.Value,
                                            TotalS = "TOTAL " + uu.v_Value1 + " :",
                                            Ccosto = o.Ccosto,
                                            i_IdPlanillaNumeracion = o.i_IdPlanillaNumeracion,
                                            i_IdCentroCosto = o.i_IdCentroCosto,
                                            TipoPlanilla = o.TipoPlanilla,
                                        };
                                    }).ToList().AsQueryable();


                    if (!string.IsNullOrEmpty(Filtro))
                    {
                        planilla = planilla.Where(Filtro);
                    }

                    Agrupados = planilla.ToList().GroupBy(x => new { x.v_IdTrabajador }).Select(group => group.FirstOrDefault()).ToList();

                    Agrupados = Agrupados.Select(n => new ReportePlanillaAFP
                    {
                        Afp = n.Afp,
                        Trabajador = n.Trabajador,
                        NroCussp = n.NroCussp,
                        v_IdTrabajador = n.v_IdTrabajador,
                        Dni = n.Dni,
                        Remuneracion = ObtenerRemuneracionAsegurable(n.v_IdTrabajador, n.i_NumeroPlanilla),
                        AporteObligatorio = ObtenerAporteObligatorio(n.v_IdTrabajador, n.i_NumeroPlanilla, n.TipoRegimen),
                        PrimaSeguro = n.TipoRegimen == 6 || n.TipoRegimen == 7 ? 0 : ObtenerCantidadConceptos(n.v_IdTrabajador, n.i_NumeroPlanilla, "D008"),
                        ComisionFija = n.TipoRegimen == 6 || n.TipoRegimen == 7 ? 0 : ObtenerCantidadConceptos(n.v_IdTrabajador, n.i_NumeroPlanilla, "D010"),
                        ComisionRa = n.TipoRegimen == 6 || n.TipoRegimen == 7 ? 0 : ObtenerCantidadConceptos(n.v_IdTrabajador, n.i_NumeroPlanilla, "D011"),
                        Ipss = 0,
                        TotalS = n.TotalS,
                        Ccosto = n.Ccosto,

                    }).ToList();
                }
                return Agrupados.Where(x => x.Trabajador != "** TRABAJADOR NO EXISTE **").ToList().OrderBy(o => o.Trabajador).ToList();

            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "PlanillaBL.ReportePlanillaAFP()";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }

        }
        public decimal ObtenerAporteObligatorio(string IdTrabajador, int NumeroPlanilla, int tiporegimen)
        {
            string Codigo = "";
            if (tiporegimen == 6 || tiporegimen == 7)
            {
                Codigo = "D002";

            }
            else
            {
                Codigo = "D006";
            }
            return ObtenerCantidadConceptos(IdTrabajador, NumeroPlanilla, Codigo);

        }
        public decimal ObtenerRemuneracionAsegurable(string IdTrabajador, int NumeroPlanilla)
        {
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                var SueldoBasico = (from a in dbContext.planillacalculo
                                    where a.v_IdTrabajador == IdTrabajador && a.i_IdPlanillaNumeracion == NumeroPlanilla && a.Tipo == "I"
                                    select a).ToList();
                if (SueldoBasico.Any())
                {
                    return SueldoBasico.Sum(x => x.d_Importe.Value);
                }
                else
                {
                    return 0;
                }

            }

        }

        public List<ReportePlanillaEssalud> ReportePlanillaEsSalud(ref OperationResult objOperationResult, string Filtro)
        {
            try
            {
                objOperationResult.Success = 1;

                List<ReportePlanillaEssalud> Agrupados = new List<ReportePlanillaEssalud>();
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    var planilla = (from a in dbContext.planillacalculo
                                    join b in dbContext.contratotrabajador on new { IdContrato = a.v_IdContrato } equals new { IdContrato = b.v_IdContrato } into b_join
                                    from b in b_join.DefaultIfEmpty()
                                    join c in dbContext.trabajador on new { IdTrabajador = b.v_IdTrabajador, eliminado = 0 } equals new { IdTrabajador = c.v_IdTrabajador, eliminado = c.i_Eliminado.Value } into c_join
                                    from c in c_join.DefaultIfEmpty()
                                    join d in dbContext.cliente on new { IdCliente = c.v_IdCliente, eliminado = 0 } equals new { IdCliente = d.v_IdCliente, eliminado = d.i_Eliminado.Value } into d_join
                                    from d in d_join.DefaultIfEmpty()
                                    join e in dbContext.areaslaboratrabajador on new { IdArea = c.v_IdTrabajador, eliminado = 0, vigente = 1 } equals new { IdArea = e.v_IdTrabajador, eliminado = e.i_Eliminado.Value, vigente = e.i_AreaVigente.Value } into e_join
                                    from e in e_join.DefaultIfEmpty()
                                    join f in dbContext.regimenpensionariotrabajador on new { IdReg = c.v_IdTrabajador, eliminado = 0, vigente = 1 } equals new { IdReg = f.v_IdTrabajador, eliminado = f.i_Eliminado.Value, vigente = f.i_RegimenVigente.Value } into f_join
                                    from f in f_join.DefaultIfEmpty()
                                    join g in dbContext.planillavariablestrabajador on new { Trabajador = c.v_IdTrabajador, eliminado = 0 } equals new { Trabajador = g.v_IdTrabajador, eliminado = g.i_Eliminado.Value } into g_join
                                    from g in g_join.DefaultIfEmpty()
                                    join h in dbContext.datahierarchy on new { reg = f.i_IdRegimenPensionario.Value, eliminado = 0, Grupo = 125 } equals new { reg = h.i_ItemId, eliminado = h.i_IsDeleted.Value, Grupo = h.i_GroupId } into h_join
                                    from h in h_join.DefaultIfEmpty()

                                    join i in dbContext.datahierarchy on new { Ccosto = e.i_IdCentroCosto.Value, eliminado = 0, Grupo = 31 } equals new { Ccosto = i.i_ItemId, eliminado = i.i_IsDeleted.Value, Grupo = i.i_GroupId } into i_join
                                    from i in i_join.DefaultIfEmpty()



                                    select new ReportePlanillaEssalud
                                    {


                                        Trabajador = d != null ? d.v_CodCliente.Trim() + "      " + (d.v_ApePaterno + " " + d.v_ApeMaterno + " " + d.v_PrimerNombre + " " + d.v_SegundoNombre).Trim() : "** TRABAJADOR NO EXISTE **",
                                        NroEssalud = c != null ? c.v_NroEsSalud : "",
                                        v_IdTrabajador = c.v_IdTrabajador,
                                        i_IdPlanillaNumeracion = a.i_IdPlanillaNumeracion.Value,
                                        TipoPlanilla = b == null ? 0 : b.i_IdTipoPlanilla == null ? 0 : b.i_IdTipoPlanilla.Value,


                                    });


                    if (!string.IsNullOrEmpty(Filtro))
                    {
                        planilla = planilla.Where(Filtro);
                    }

                    Agrupados = planilla.ToList().GroupBy(x => new { x.v_IdTrabajador }).Select(group => group.FirstOrDefault()).ToList();

                    Agrupados = Agrupados.Select(n => new ReportePlanillaEssalud
                    {

                        Trabajador = n.Trabajador,
                        NroEssalud = n.NroEssalud,
                        RemuneracionAsegurable = ObtenerRemuneracionAsegurable(n.v_IdTrabajador, n.i_IdPlanillaNumeracion),
                        EsSaludVida = ObtenerCantidadConceptos(n.v_IdTrabajador, n.i_IdPlanillaNumeracion, "D014"),
                        EsSalud = ObtenerCantidadConceptos(n.v_IdTrabajador, n.i_IdPlanillaNumeracion, "A001"),

                    }).ToList();


                }



                return Agrupados.Where(x => x.Trabajador != "** TRABAJADOR NO EXISTE **").OrderBy(o => o.Trabajador).ToList();
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;

                objOperationResult.AdditionalInformation = "PlanillaBL.ReportePlanillaEsSalud()";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }



        }
        public List<ReporteResumenPlanillas> ReporteResumenPlanillas(ref  OperationResult objOperationResult, string Filtro)
        {

            try
            {
                objOperationResult.Success = 1;
                List<ReporteResumenPlanillas> Agrupados = new List<ReporteResumenPlanillas>();
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    var resumenplanilla = (from a in dbContext.planillacalculo
                                           join b in dbContext.contratotrabajador on new { IdTrabajador = a.v_IdTrabajador, eliminado = 0, vigente = 1 } equals new { IdTrabajador = b.v_IdTrabajador, eliminado = b.i_Eliminado.Value, vigente = b.i_ContratoVigente ?? 0 } into b_join
                                           from b in b_join.DefaultIfEmpty()
                                           join d in dbContext.planillaconceptos on new { Concepto = a.v_IdConceptoPlanilla, eliminado = 0 } equals new { Concepto = d.v_IdConceptoPlanilla, eliminado = d.i_Eliminado.Value } into d_join
                                           from d in d_join.DefaultIfEmpty()
                                           join e in dbContext.trabajador on new { t = b.v_IdTrabajador, eliminado = 0 } equals new { t = e.v_IdTrabajador, eliminado = e.i_Eliminado.Value } into e_join
                                           from e in e_join.DefaultIfEmpty()

                                           //where a.i_IdPlanillaNumeracion == Filtro

                                           select new ReporteResumenPlanillas
                                           {
                                               Importe = a.d_Importe.Value,
                                               TipoConcepto = d == null ? "" : d.i_IdTipoConcepto == (int)TipoConceptosPlanillas.Ingresos ? "INGRESOS" : d.i_IdTipoConcepto == (int)TipoConceptosPlanillas.Descuentos ? "DESCUENTOS" : d.i_IdTipoConcepto == (int)TipoConceptosPlanillas.Aportaciones ? "APORTACIONES" : "",
                                               Concepto = d == null ? "" : d.v_Codigo + " " + d.v_Nombre,
                                               i_IdPlanillaNumeracion = a.i_IdPlanillaNumeracion.Value,
                                               IdTipoConcepto = d.i_IdTipoConcepto.Value,
                                               TipoPlanilla = b == null ? 0 : b.i_IdTipoPlanilla.Value,
                                               CodigoConcepto = d.v_Codigo,
                                               CodifoTrabajador = e.cliente.v_CodCliente.Trim(),

                                           });


                    if (!string.IsNullOrEmpty(Filtro))
                    {
                        resumenplanilla = resumenplanilla.Where(Filtro);
                    }
                    var gg = resumenplanilla.ToList().OrderBy(o => o.CodifoTrabajador).Where(o => o.CodigoConcepto == "I022").ToList();
                    Agrupados = resumenplanilla.ToList().GroupBy(x => new { x.Concepto }).Select(group => group.FirstOrDefault()).ToList();

                    Agrupados = Agrupados.Select(n => new ReporteResumenPlanillas
                    {
                        Importe = resumenplanilla.Where(x => x.Concepto == n.Concepto).Sum(x => x.Importe),
                        Concepto = n.Concepto,
                        TipoConcepto = n.TipoConcepto,
                        Total = "TOTAL " + n.TipoConcepto + " : ",
                        DTotal = resumenplanilla.Where(x => x.IdTipoConcepto == (int)TipoConceptosPlanillas.Ingresos).Sum(x => x.Importe) - resumenplanilla.Where(x => x.IdTipoConcepto == (int)TipoConceptosPlanillas.Descuentos).Sum(x => x.Importe)

                    }).ToList();
                }

                return Agrupados.OrderBy(x => x.Concepto).ToList();
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;

                objOperationResult.AdditionalInformation = "PlanillaBL.ReporteResumenPlanillas()";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }

        }
        public List<ReporteResumenVsAnterior> ReporteResumenPlanillaVsAnterior(ref OperationResult pobjOperationResult, string Filtro, string FiltroPlanillaAnterior)
        {
            try
            {
                List<ReporteResumenVsAnterior> Agrupados = new List<ReporteResumenVsAnterior>();
                pobjOperationResult.Success = 1;
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {


                    var resumenPlanillaAnterior = (from a in dbContext.planillacalculo
                                                   join b in dbContext.contratotrabajador on new { IdTrabajador = a.v_IdTrabajador, eliminado = 0, vigente = 1 } equals new { IdTrabajador = b.v_IdTrabajador, eliminado = b.i_Eliminado.Value, vigente = b.i_ContratoVigente.Value } into b_join
                                                   from b in b_join.DefaultIfEmpty()
                                                   join d in dbContext.planillaconceptos on new { Concepto = a.v_IdConceptoPlanilla, eliminado = 0 } equals new { Concepto = d.v_IdConceptoPlanilla, eliminado = d.i_Eliminado.Value } into d_join
                                                   from d in d_join.DefaultIfEmpty()
                                                   join e in dbContext.planillanumeracion on new { Numeracion = a.i_IdPlanillaNumeracion.Value, eliminado = 0 } equals new { Numeracion = e.i_Id, eliminado = e.i_Eliminado.Value } into e_join
                                                   from e in e_join.DefaultIfEmpty()

                                                   select new ReporteResumenVsAnterior
                                                   {
                                                       Importe = a.d_Importe == null ? 0 : a.d_Importe.Value,
                                                       TipoConcepto = d == null ? "" : d.i_IdTipoConcepto == (int)TipoConceptosPlanillas.Ingresos ? "INGRESOS" : d.i_IdTipoConcepto == (int)TipoConceptosPlanillas.Descuentos ? "DESCUENTOS" : d.i_IdTipoConcepto == (int)TipoConceptosPlanillas.Aportaciones ? "APORTACIONES" : "",
                                                       Concepto = d == null ? "" : d.v_Codigo + " " + d.v_Nombre,
                                                       i_IdPlanillaNumeracion = a.i_IdPlanillaNumeracion.Value,
                                                       IdTipoConcepto = d.i_IdTipoConcepto.Value,
                                                       TipoPlanilla = b == null ? 0 : b.i_IdTipoPlanilla.Value,
                                                       PeriodoAnterior = e.v_Periodo,
                                                       MesAnterior = e.v_Mes,


                                                   });


                    if (!string.IsNullOrEmpty(FiltroPlanillaAnterior))
                    {
                        resumenPlanillaAnterior = resumenPlanillaAnterior.Where(FiltroPlanillaAnterior);
                    }


                    var resumenplanilla = (from a in dbContext.planillacalculo
                                           join b in dbContext.contratotrabajador on new { IdTrabajador = a.v_IdTrabajador, eliminado = 0, vigente = 1 } equals new { IdTrabajador = b.v_IdTrabajador, eliminado = b.i_Eliminado.Value, vigente = b.i_ContratoVigente ?? 0 } into b_join
                                           from b in b_join.DefaultIfEmpty()
                                           join d in dbContext.planillaconceptos on new { Concepto = a.v_IdConceptoPlanilla, eliminado = 0 } equals new { Concepto = d.v_IdConceptoPlanilla, eliminado = d.i_Eliminado.Value } into d_join
                                           from d in d_join.DefaultIfEmpty()

                                           select new ReporteResumenVsAnterior
                                           {
                                               Importe = a.d_Importe == null ? 0 : a.d_Importe.Value,
                                               TipoConcepto = d == null ? "" : d.i_IdTipoConcepto == (int)TipoConceptosPlanillas.Ingresos ? "INGRESOS" : d.i_IdTipoConcepto == (int)TipoConceptosPlanillas.Descuentos ? "DESCUENTOS" : d.i_IdTipoConcepto == (int)TipoConceptosPlanillas.Aportaciones ? "APORTACIONES" : "",
                                               Concepto = d == null ? "" : d.v_Codigo + " " + d.v_Nombre,
                                               i_IdPlanillaNumeracion = a.i_IdPlanillaNumeracion.Value,
                                               IdTipoConcepto = d.i_IdTipoConcepto.Value,
                                               TipoPlanilla = b == null ? 0 : b.i_IdTipoPlanilla.Value,

                                           });


                    if (!string.IsNullOrEmpty(Filtro))
                    {
                        resumenplanilla = resumenplanilla.Where(Filtro);
                    }

                    Agrupados = resumenplanilla.ToList().GroupBy(x => new { x.Concepto }).Select(group => group.FirstOrDefault()).ToList();

                    Agrupados = Agrupados.Select(n => new ReporteResumenVsAnterior
                    {
                        Importe = resumenplanilla.Any() ? resumenplanilla.Where(x => x.Concepto == n.Concepto).Sum(x => x.Importe) : 0,
                        Concepto = n.Concepto,
                        TipoConcepto = n.TipoConcepto,
                        Total = "TOTAL " + n.TipoConcepto + " : ",
                        DTotal = resumenplanilla.Any() ? resumenplanilla.Where(x => x.IdTipoConcepto == (int)TipoConceptosPlanillas.Ingresos).Sum(x => x.Importe) - resumenplanilla.Where(x => x.IdTipoConcepto == (int)TipoConceptosPlanillas.Descuentos).Sum(x => x.Importe) : 0,
                        ImporteMesAnterior = resumenPlanillaAnterior.Any() ? resumenPlanillaAnterior.Where(x => x.Concepto == n.Concepto).ToList().Count() > 0 ? resumenPlanillaAnterior.Where(x => x.Concepto == n.Concepto).Sum(x => x.Importe) : 0 : 0,
                        DTotalMesAnterior = resumenPlanillaAnterior.Any() ? resumenPlanillaAnterior.Where(x => x.IdTipoConcepto == (int)TipoConceptosPlanillas.Ingresos).Sum(x => x.Importe) - resumenPlanillaAnterior.Where(x => x.IdTipoConcepto == (int)TipoConceptosPlanillas.Descuentos).Sum(x => x.Importe) : 0,

                    }).ToList();


                    var ListaConceptosPlanillaAnterior = resumenPlanillaAnterior.Select(x => x.Concepto).Distinct().ToList();
                    var ListaConceptosAgrupados = Agrupados.Select(x => x.Concepto).Distinct().ToList();
                    var Faltantes = ListaConceptosPlanillaAnterior.Except(ListaConceptosAgrupados).ToList();

                    foreach (var pConceptofaltante in Faltantes)
                    {
                        ReporteResumenVsAnterior objReporte = new ReporteResumenVsAnterior();
                        objReporte.Importe = resumenplanilla.Any() ? resumenplanilla.Where(x => x.Concepto == pConceptofaltante).Count() > 0 ? resumenplanilla.Where(x => x.Concepto == pConceptofaltante).Sum(x => x.Importe) : 0 : 0;
                        objReporte.Concepto = pConceptofaltante;
                        objReporte.TipoConcepto = resumenPlanillaAnterior.Where(x => x.Concepto == pConceptofaltante).FirstOrDefault().TipoConcepto;
                        objReporte.Total = "TOTAL " + objReporte.TipoConcepto + " : ";
                        objReporte.DTotal = resumenplanilla.Any() ? resumenplanilla.Where(x => x.IdTipoConcepto == (int)TipoConceptosPlanillas.Ingresos).Sum(x => x.Importe) - resumenplanilla.Where(x => x.IdTipoConcepto == (int)TipoConceptosPlanillas.Descuentos).Sum(x => x.Importe) : 0;
                        objReporte.ImporteMesAnterior = resumenPlanillaAnterior.Any() ? resumenPlanillaAnterior.Where(x => x.Concepto == pConceptofaltante).ToList().Count() > 0 ? resumenPlanillaAnterior.Where(x => x.Concepto == pConceptofaltante).Sum(x => x.Importe) : 0 : 0;
                        objReporte.DTotalMesAnterior = resumenPlanillaAnterior.Any() ? resumenPlanillaAnterior.Where(x => x.IdTipoConcepto == (int)TipoConceptosPlanillas.Ingresos).Sum(x => x.Importe) - resumenPlanillaAnterior.Where(x => x.IdTipoConcepto == (int)TipoConceptosPlanillas.Descuentos).Sum(x => x.Importe) : 0;
                        Agrupados.Add(objReporte);
                    }


                }

                return Agrupados.OrderBy(x => x.Concepto).ToList();
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaBL.ReporteResumenPlanillaVsAnterior()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }

        }
        public List<ReportePlanillaNetoPagar> ReportePlanillasNetoPagar(ref OperationResult objOperationResult, int Filtro)
        {
            try
            {
                objOperationResult.Success = 1;
                List<ReportePlanillaNetoPagar> ListaFinal = new List<ReportePlanillaNetoPagar>();
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    List<planillacalculo> PlanillaCalculoAss = new List<planillacalculo>();
                    PlanillaCalculoAss = new PlanillaBL().ObtenerListaPlanillaCalculoAssembler(ref objOperationResult);
                    var planilla = (from a in dbContext.planillacalculo
                                    join b in dbContext.contratotrabajador on new { IdContrato = a.v_IdContrato } equals new { IdContrato = b.v_IdContrato } into b_join
                                    from b in b_join.DefaultIfEmpty()
                                    join c in dbContext.trabajador on new { IdTrabajador = b.v_IdTrabajador, eliminado = 0 } equals new { IdTrabajador = c.v_IdTrabajador, eliminado = c.i_Eliminado.Value } into c_join
                                    from c in c_join.DefaultIfEmpty()
                                    join d in dbContext.cliente on new { IdCliente = c.v_IdCliente, eliminado = 0 } equals new { IdCliente = d.v_IdCliente, eliminado = d.i_Eliminado.Value } into d_join
                                    from d in d_join.DefaultIfEmpty()
                                    where a.i_IdPlanillaNumeracion == Filtro
                                    select new ReportePlanillaNetoPagar
                                    {
                                        CodigoTrabajador = d != null ? d.v_CodCliente : "",
                                        Trabajador = d != null ? d.v_ApePaterno + " " + d.v_ApeMaterno + " " + d.v_PrimerNombre + " " + d.v_SegundoNombre : "",
                                        v_IdTrabajador = c != null ? c.v_IdTrabajador : "",

                                    });
                    ListaFinal = planilla.ToList().GroupBy(x => new { x.v_IdTrabajador }).Select(group => group.FirstOrDefault()).ToList();
                    ListaFinal = (from a in ListaFinal
                                  select new ReportePlanillaNetoPagar
                                  {
                                      CodigoTrabajador = a.CodigoTrabajador,
                                      Trabajador = a.Trabajador,
                                      NetoPagar = ObtenerNetoPagar(a.v_IdTrabajador, Filtro, PlanillaCalculoAss),

                                  }).ToList();
                }
                return ListaFinal;
            }
            catch (Exception ex)
            {


                objOperationResult.Success = 0;

                objOperationResult.AdditionalInformation = "PlanillaBL.ReportePlanillasNetoPagar()";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }





        }
        public List<ReporteResumenVsAnterior> ReporteResumenPlanillaAcumulado(ref OperationResult pobjOperationResult, string Filtro, string FiltroPlanillaAnterior, int MesAnterior)
        {

            try
            {
                pobjOperationResult.Success = 1;
                List<ReporteResumenVsAnterior> Agrupados = new List<ReporteResumenVsAnterior>();

                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {


                    var resumenPlanillaAnterior = (from a in dbContext.planillacalculo
                                                   join b in dbContext.contratotrabajador on new { IdTrabajador = a.v_IdTrabajador, eliminado = 0, vigente = 1 } equals new { IdTrabajador = b.v_IdTrabajador, eliminado = b.i_Eliminado.Value, vigente = b.i_ContratoVigente.Value } into b_join
                                                   from b in b_join.DefaultIfEmpty()
                                                   join d in dbContext.planillaconceptos on new { Concepto = a.v_IdConceptoPlanilla, eliminado = 0 } equals new { Concepto = d.v_IdConceptoPlanilla, eliminado = d.i_Eliminado.Value } into d_join
                                                   from d in d_join.DefaultIfEmpty()
                                                   join e in dbContext.planillanumeracion on new { Numeracion = a.i_IdPlanillaNumeracion.Value, eliminado = 0 } equals new { Numeracion = e.i_Id, eliminado = e.i_Eliminado.Value } into e_join
                                                   from e in e_join.DefaultIfEmpty()

                                                   select new ReporteResumenVsAnterior
                                                   {
                                                       Importe = a.d_Importe == null ? 0 : a.d_Importe ?? 0,
                                                       TipoConcepto = d == null ? "" : d.i_IdTipoConcepto == null ? "" : d.i_IdTipoConcepto == (int)TipoConceptosPlanillas.Ingresos ? "INGRESOS" : d.i_IdTipoConcepto == (int)TipoConceptosPlanillas.Descuentos ? "DESCUENTOS" : d.i_IdTipoConcepto == (int)TipoConceptosPlanillas.Aportaciones ? "APORTACIONES" : "",
                                                       Concepto = d == null ? "" : d.v_Codigo + " " + d.v_Nombre,
                                                       i_IdPlanillaNumeracion = a.i_IdPlanillaNumeracion ?? 0,
                                                       IdTipoConcepto = d.i_IdTipoConcepto ?? 0,
                                                       TipoPlanilla = b == null ? 0 : b.i_IdTipoPlanilla ?? 0,
                                                       PeriodoAnterior = e.v_Periodo,
                                                       MesAnterior = e.v_Mes,


                                                   });


                    if (!string.IsNullOrEmpty(FiltroPlanillaAnterior))
                    {
                        resumenPlanillaAnterior = resumenPlanillaAnterior.Where(FiltroPlanillaAnterior);
                    }
                    List<ReporteResumenVsAnterior> ResumenPlanillaAnt = resumenPlanillaAnterior.ToList();

                    ResumenPlanillaAnt = ResumenPlanillaAnt.Where(x => int.Parse(x.MesAnterior) <= MesAnterior).ToList();


                    var resumenplanilla = (from a in dbContext.planillacalculo
                                           join b in dbContext.contratotrabajador on new { IdTrabajador = a.v_IdTrabajador, eliminado = 0, vigente = 1 } equals new { IdTrabajador = b.v_IdTrabajador, eliminado = b.i_Eliminado.Value, vigente = b.i_ContratoVigente.Value } into b_join
                                           from b in b_join.DefaultIfEmpty()
                                           join d in dbContext.planillaconceptos on new { Concepto = a.v_IdConceptoPlanilla, eliminado = 0 } equals new { Concepto = d.v_IdConceptoPlanilla, eliminado = d.i_Eliminado.Value } into d_join
                                           from d in d_join.DefaultIfEmpty()

                                           select new ReporteResumenVsAnterior
                                           {
                                               Importe = a.d_Importe == null ? 0 : a.d_Importe ?? 0,
                                               TipoConcepto = d == null ? "" : d.i_IdTipoConcepto == null ? "" : d.i_IdTipoConcepto == (int)TipoConceptosPlanillas.Ingresos ? "INGRESOS" : d.i_IdTipoConcepto == (int)TipoConceptosPlanillas.Descuentos ? "DESCUENTOS" : d.i_IdTipoConcepto == (int)TipoConceptosPlanillas.Aportaciones ? "APORTACIONES" : "",
                                               Concepto = d == null ? "" : d.v_Codigo + " " + d.v_Nombre,
                                               i_IdPlanillaNumeracion = a.i_IdPlanillaNumeracion ?? 0,
                                               IdTipoConcepto = d.i_IdTipoConcepto ?? 0,
                                               TipoPlanilla = b == null ? 0 : b.i_IdTipoPlanilla ?? 0,

                                           });


                    if (!string.IsNullOrEmpty(Filtro))
                    {
                        resumenplanilla = resumenplanilla.Where(Filtro);
                    }

                    Agrupados = resumenplanilla.ToList().GroupBy(x => new { x.Concepto }).Select(group => group.FirstOrDefault()).ToList();

                    Agrupados = Agrupados.Select(n => new ReporteResumenVsAnterior
                    {
                        Importe = resumenplanilla.Any() ? resumenplanilla.Where(x => x.Concepto == n.Concepto).Sum(x => x.Importe) : 0,
                        Concepto = n.Concepto,
                        TipoConcepto = n.TipoConcepto,
                        Total = "TOTAL " + n.TipoConcepto + " : ",
                        DTotal = resumenplanilla.Any() ? resumenplanilla.Where(x => x.IdTipoConcepto == (int)TipoConceptosPlanillas.Ingresos).Sum(x => x.Importe) - resumenplanilla.Where(x => x.IdTipoConcepto == (int)TipoConceptosPlanillas.Descuentos).Sum(x => x.Importe) : 0,
                        ImporteMesAnterior = ResumenPlanillaAnt.Any() ? ResumenPlanillaAnt.Where(x => x.Concepto == n.Concepto).ToList().Count() > 0 ? ResumenPlanillaAnt.Where(x => x.Concepto == n.Concepto).Sum(x => x.Importe) : 0 : 0,
                        DTotalMesAnterior = ResumenPlanillaAnt.Any() ? ResumenPlanillaAnt.Where(x => x.IdTipoConcepto == (int)TipoConceptosPlanillas.Ingresos).Sum(x => x.Importe) - ResumenPlanillaAnt.Where(x => x.IdTipoConcepto == (int)TipoConceptosPlanillas.Descuentos).Sum(x => x.Importe) : 0,

                    }).ToList();

                    var ListaConceptosPlanillaAnterior = ResumenPlanillaAnt.Select(x => x.Concepto).Distinct().ToList();
                    var ListaConceptosAgrupados = Agrupados.Select(x => x.Concepto).Distinct().ToList();
                    var Faltantes = ListaConceptosPlanillaAnterior.Except(ListaConceptosAgrupados).ToList();

                    foreach (var pConceptofaltante in Faltantes)
                    {
                        ReporteResumenVsAnterior objReporte = new ReporteResumenVsAnterior();
                        objReporte.Importe = resumenplanilla.Any() ? resumenplanilla.Where(x => x.Concepto == pConceptofaltante).Count() > 0 ? resumenplanilla.Where(x => x.Concepto == pConceptofaltante).Sum(x => x.Importe) : 0 : 0;
                        objReporte.Concepto = pConceptofaltante;
                        objReporte.TipoConcepto = ResumenPlanillaAnt.Where(x => x.Concepto == pConceptofaltante).FirstOrDefault().TipoConcepto;
                        objReporte.Total = "TOTAL " + objReporte.TipoConcepto + " : ";
                        objReporte.DTotal = resumenplanilla.Any() ? resumenplanilla.Where(x => x.IdTipoConcepto == (int)TipoConceptosPlanillas.Ingresos).Sum(x => x.Importe) - resumenplanilla.Where(x => x.IdTipoConcepto == (int)TipoConceptosPlanillas.Descuentos).Sum(x => x.Importe) : 0;
                        objReporte.ImporteMesAnterior = ResumenPlanillaAnt.Any() ? ResumenPlanillaAnt.Where(x => x.Concepto == pConceptofaltante).ToList().Count() > 0 ? ResumenPlanillaAnt.Where(x => x.Concepto == pConceptofaltante).Sum(x => x.Importe) : 0 : 0;
                        objReporte.DTotalMesAnterior = ResumenPlanillaAnt.Any() ? ResumenPlanillaAnt.Where(x => x.IdTipoConcepto == (int)TipoConceptosPlanillas.Ingresos).Sum(x => x.Importe) - ResumenPlanillaAnt.Where(x => x.IdTipoConcepto == (int)TipoConceptosPlanillas.Descuentos).Sum(x => x.Importe) : 0;
                        Agrupados.Add(objReporte);
                    }

                }

                return Agrupados.OrderBy(x => x.Concepto).ToList();
            }
            catch (Exception ex)
            {

                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaBL.ReporteResumenPlanillaAcumulado()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;

            }

        }

        public List<ReporteResumenAnual> ReporteResumenPlanillaAnual(ref OperationResult pobjOperationResult, string Filtro)
        {
            try
            {
                pobjOperationResult.Success = 1;
                List<ReporteResumenAnual> Agrupados = new List<ReporteResumenAnual>();

                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {


                    var PlanillasPeriodo = (from a in dbContext.planillacalculo
                                            join b in dbContext.planillanumeracion on new { Id = a.i_IdPlanillaNumeracion.Value, eliminado = 0 } equals new { Id = b.i_Id, eliminado = b.i_Eliminado.Value } into b_join
                                            from b in b_join.DefaultIfEmpty()

                                            join c in dbContext.planillaconceptos on new { Concepto = a.v_IdConceptoPlanilla, eliminado = 0 } equals new { Concepto = c.v_IdConceptoPlanilla, eliminado = c.i_Eliminado.Value } into c_join
                                            from c in c_join.DefaultIfEmpty()

                                            select new ReporteResumenAnual
                                            {
                                                Periodo = b == null ? "" : b.v_Periodo,
                                                Importe = a.d_Importe == null ? 0 : a.d_Importe ?? 0,
                                                TipoConcepto = c == null ? "" : c.i_IdTipoConcepto == null ? "" : c.i_IdTipoConcepto == (int)TipoConceptosPlanillas.Ingresos ? "INGRESOS" : c.i_IdTipoConcepto == (int)TipoConceptosPlanillas.Descuentos ? "DESCUENTOS" : c.i_IdTipoConcepto == (int)TipoConceptosPlanillas.Aportaciones ? "APORTACIONES" : "",
                                                Concepto = c == null ? "" : c.v_Codigo + " " + c.v_Nombre,
                                                // i_IdPlanillaNumeracion = a.i_IdPlanillaNumeracion.Value,
                                                IdTipoConcepto = c.i_IdTipoConcepto ?? 0,
                                                Mes = b == null ? "" : b.v_Mes,
                                                v_IdConcepto = c == null ? "" : c.v_IdConceptoPlanilla,
                                                v_IdConceptoPC = a.v_IdConceptoPlanilla,

                                            });


                    var Nulo = PlanillasPeriodo.Where(o => o.Concepto == "").ToList();
                    if (!string.IsNullOrEmpty(Filtro))
                    {
                        PlanillasPeriodo = PlanillasPeriodo.Where(Filtro);
                    }

                    List<ReporteResumenAnual> ListaPlanillaPeriodo = PlanillasPeriodo.ToList();

                    var Meses = ListaPlanillaPeriodo.OrderBy(x => x.Mes).Select(x => x.Mes).Distinct().ToList();


                    Agrupados = ListaPlanillaPeriodo.ToList().GroupBy(x => new { x.Concepto }).Select(group => group.FirstOrDefault()).ToList();

                    Agrupados = Agrupados.Select(n => new ReporteResumenAnual
                    {

                        Concepto = n.Concepto,
                        TipoConcepto = n.TipoConcepto,
                        Mes1 = Meses.Count() > 0 ? ListaPlanillaPeriodo.Where(x => x.Concepto == n.Concepto && x.Mes == Meses[0]).Sum(x => x.Importe) : 0,
                        NombreMes1 = Meses.Count() > 0 ? (new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(int.Parse(Meses[0]))).ToUpper().Substring(0, 3) : "",

                        Mes2 = Meses.Count() > 1 ? ListaPlanillaPeriodo.Where(x => x.Concepto == n.Concepto && x.Mes == Meses[1]).Sum(x => x.Importe) : 0,
                        NombreMes2 = Meses.Count() > 1 ? (new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(int.Parse(Meses[1]))).ToUpper().Substring(0, 3) : "",

                        Mes3 = Meses.Count() > 2 ? ListaPlanillaPeriodo.Where(x => x.Concepto == n.Concepto && x.Mes == Meses[2]).Sum(x => x.Importe) : 0,
                        NombreMes3 = Meses.Count() > 2 ? (new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(int.Parse(Meses[2]))).ToUpper().Substring(0, 3) : "",


                        Mes4 = Meses.Count() > 3 ? ListaPlanillaPeriodo.Where(x => x.Concepto == n.Concepto && x.Mes == Meses[3]).Sum(x => x.Importe) : 0,
                        NombreMes4 = Meses.Count() > 3 ? (new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(int.Parse(Meses[3]))).ToUpper().Substring(0, 3) : "",

                        Mes5 = Meses.Count() > 4 ? ListaPlanillaPeriodo.Where(x => x.Concepto == n.Concepto && x.Mes == Meses[4]).Sum(x => x.Importe) : 0,
                        NombreMes5 = Meses.Count() > 4 ? (new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(int.Parse(Meses[4]))).ToUpper().Substring(0, 3) : "",

                        Mes6 = Meses.Count() > 5 ? ListaPlanillaPeriodo.Where(x => x.Concepto == n.Concepto && x.Mes == Meses[5]).Sum(x => x.Importe) : 0,
                        NombreMes6 = Meses.Count() > 5 ? (new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(int.Parse(Meses[5]))).ToUpper().Substring(0, 3) : "",

                        Mes7 = Meses.Count() > 6 ? ListaPlanillaPeriodo.Where(x => x.Concepto == n.Concepto && x.Mes == Meses[6]).Sum(x => x.Importe) : 0,
                        NombreMes7 = Meses.Count() > 6 ? (new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(int.Parse(Meses[6]))).ToUpper().Substring(0, 3) : "",

                        Mes8 = Meses.Count() > 7 ? ListaPlanillaPeriodo.Where(x => x.Concepto == n.Concepto && x.Mes == Meses[7]).Sum(x => x.Importe) : 0,
                        NombreMes8 = Meses.Count() > 7 ? (new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(int.Parse(Meses[7]))).ToUpper().Substring(0, 3) : "",

                        Mes9 = Meses.Count() > 8 ? ListaPlanillaPeriodo.Where(x => x.Concepto == n.Concepto && x.Mes == Meses[8]).Sum(x => x.Importe) : 0,
                        NombreMes9 = Meses.Count() > 8 ? (new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(int.Parse(Meses[8]))).ToUpper().Substring(0, 3) : "",

                        Mes10 = Meses.Count() > 9 ? ListaPlanillaPeriodo.Where(x => x.Concepto == n.Concepto && x.Mes == Meses[9]).Sum(x => x.Importe) : 0,
                        NombreMes10 = Meses.Count() > 9 ? (new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(int.Parse(Meses[9]))).ToUpper().Substring(0, 3) : "",

                        Mes11 = Meses.Count() > 10 ? ListaPlanillaPeriodo.Where(x => x.Concepto == n.Concepto && x.Mes == Meses[10]).Sum(x => x.Importe) : 0,
                        NombreMes11 = Meses.Count() > 10 ? (new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(int.Parse(Meses[10]))).ToUpper().Substring(0, 3) : "",

                        Mes12 = Meses.Count() > 11 ? ListaPlanillaPeriodo.Where(x => x.Concepto == n.Concepto && x.Mes == Meses[11]).Sum(x => x.Importe) : 0,
                        NombreMes12 = Meses.Count() > 11 ? (new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(int.Parse(Meses[11]))).ToUpper().Substring(0, 3) : "",

                        Total = "TOTAL " + n.TipoConcepto + " : ",
                    }).ToList();

                }
                return Agrupados.OrderBy(x => x.Concepto).ToList();
            }
            catch (Exception ex)
            {

                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaBL.ReporteResumenPlanillaAnual()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;


            }




        }

        public List<ReportePlanillaDetalleDescuentos> ReportePlanillaDescuentos(ref  OperationResult pobjOperationResult, string Filtro, int IdPlanilla)
        {
            try
            {
                pobjOperationResult.Success = 1;
                ReportePlanillaDetalleDescuentos objReporte = new ReportePlanillaDetalleDescuentos();
                List<ReportePlanillaDetalleDescuentos> ListaFinalReporte = new List<ReportePlanillaDetalleDescuentos>();
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    var ConceptosPlanilla = ObtenerListaConceptosPlanilla(IdPlanilla, "RPD");
                    var planilla = (from a in dbContext.planillacalculo

                                    join b in dbContext.trabajador on new { Trabajador = a.v_IdTrabajador, eliminado = 0 } equals new { Trabajador = b.v_IdTrabajador, eliminado = b.i_Eliminado.Value } into b_join
                                    from b in b_join.DefaultIfEmpty()
                                    join c in dbContext.cliente on new { Cliente = b.v_IdCliente, eliminado = 0 } equals new { Cliente = c.v_IdCliente, eliminado = c.i_Eliminado.Value } into c_join
                                    from c in c_join.DefaultIfEmpty()

                                    select new ReportePlanillaDetalleDescuentos
                                    {

                                        Trabajador = c.v_CodCliente.Trim() + "     " + (c.v_ApePaterno + " " + c.v_ApeMaterno + " " + c.v_PrimerNombre + " " + c.v_SegundoNombre).Trim(),
                                        v_IdTrabajador = b.v_IdTrabajador,
                                        i_IdPlanillaNumeracion = a.i_IdPlanillaNumeracion.Value,

                                    });

                    if (!string.IsNullOrEmpty(Filtro))
                    {

                        planilla = planilla.Where(Filtro);
                    }

                    List<ReportePlanillaDetalleDescuentos> Agrupados = planilla.ToList().GroupBy(x => new { x.v_IdTrabajador }).Select(x => x.FirstOrDefault()).ToList();
                    foreach (var grupito in Agrupados)
                    {
                        objReporte = new ReportePlanillaDetalleDescuentos();

                        for (int i = 0; i < ConceptosPlanilla.Count(); i++)
                        {

                            objReporte.Trabajador = grupito.Trabajador;
                            objReporte.Remuneracion = ObtenerRemuneracionAsegurable(grupito.v_IdTrabajador, grupito.i_IdPlanillaNumeracion);
                            switch (i)
                            {

                                case 0:
                                    objReporte.Descuento1 = ConceptosPlanilla[i].NombreConcepto;
                                    objReporte.DDescuento1 = ObtenerCantidadConceptos(grupito.v_IdTrabajador, IdPlanilla, ConceptosPlanilla[i].CodigoConcepto);
                                    break;

                                case 1:


                                    objReporte.Descuento2 = ConceptosPlanilla[i].NombreConcepto;
                                    objReporte.DDescuento2 = ObtenerCantidadConceptos(grupito.v_IdTrabajador, IdPlanilla, ConceptosPlanilla[i].CodigoConcepto);
                                    break;
                                case 2:


                                    objReporte.Descuento3 = ConceptosPlanilla[i].NombreConcepto;
                                    objReporte.DDescuento3 = ObtenerCantidadConceptos(grupito.v_IdTrabajador, IdPlanilla, ConceptosPlanilla[i].CodigoConcepto);
                                    break;
                                case 3:


                                    objReporte.Descuento4 = ConceptosPlanilla[i].NombreConcepto;
                                    objReporte.DDescuento4 = ObtenerCantidadConceptos(grupito.v_IdTrabajador, IdPlanilla, ConceptosPlanilla[i].CodigoConcepto);
                                    break;
                                case 4:


                                    objReporte.Descuento5 = ConceptosPlanilla[i].NombreConcepto;
                                    objReporte.DDescuento5 = ObtenerCantidadConceptos(grupito.v_IdTrabajador, IdPlanilla, ConceptosPlanilla[i].CodigoConcepto);
                                    break;
                                case 5:


                                    objReporte.Descuento6 = ConceptosPlanilla[i].NombreConcepto;
                                    objReporte.DDescuento6 = ObtenerCantidadConceptos(grupito.v_IdTrabajador, IdPlanilla, ConceptosPlanilla[i].CodigoConcepto);
                                    break;
                                case 6:


                                    objReporte.Descuento7 = ConceptosPlanilla[i].NombreConcepto;
                                    objReporte.DDescuento7 = ObtenerCantidadConceptos(grupito.v_IdTrabajador, IdPlanilla, ConceptosPlanilla[i].CodigoConcepto);
                                    break;
                                case 7:


                                    objReporte.Descuento8 = ConceptosPlanilla[i].NombreConcepto;
                                    objReporte.DDescuento8 = ObtenerCantidadConceptos(grupito.v_IdTrabajador, IdPlanilla, ConceptosPlanilla[i].CodigoConcepto);
                                    break;
                                case 8:


                                    objReporte.Descuento9 = ConceptosPlanilla[i].NombreConcepto;
                                    objReporte.DDescuento9 = ObtenerCantidadConceptos(grupito.v_IdTrabajador, IdPlanilla, ConceptosPlanilla[i].CodigoConcepto);
                                    break;

                                case 9:


                                    objReporte.Descuento10 = ConceptosPlanilla[i].NombreConcepto;
                                    objReporte.DDescuento10 = ObtenerCantidadConceptos(grupito.v_IdTrabajador, IdPlanilla, ConceptosPlanilla[i].CodigoConcepto);
                                    break;

                            }

                        }
                        ListaFinalReporte.Add(objReporte);
                    }


                }

                return ListaFinalReporte;
                // objOperationResult.Success = 1;
                //return Agrupados.OrderBy(x => x.Mes).ToList();
            }
            catch (Exception ex)
            {


                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaBL.ReportePlanillaDescuentos()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;


            }
        }


        public List<ReportePlanillaGenAFP> ReportePlanillaparaAFP(ref  OperationResult objOperationResult, int IdPlanilla)
        {
            List<ReportePlanillaGenAFP> res = null;
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var planilla = (from a in dbContext.planillacalculo
                                    join b in dbContext.contratotrabajador on new { IdContrato = a.v_IdContrato } equals new { IdContrato = b.v_IdContrato } into b_join
                                    from b in b_join.DefaultIfEmpty()

                                    join c in dbContext.trabajador on new { IdTrabajador = b.v_IdTrabajador, eliminado = 0 } equals new { IdTrabajador = c.v_IdTrabajador, eliminado = c.i_Eliminado.Value } into c_join
                                    from c in c_join.DefaultIfEmpty()

                                    join d in dbContext.cliente on new { IdCliente = c.v_IdCliente, eliminado = 0 } equals new { IdCliente = d.v_IdCliente, eliminado = d.i_Eliminado.Value } into d_join
                                    from d in d_join.DefaultIfEmpty()

                                    join e in dbContext.areaslaboratrabajador on new { IdArea = c.v_IdTrabajador, eliminado = 0, vigente = 1 } equals new { IdArea = e.v_IdTrabajador, eliminado = e.i_Eliminado.Value, vigente = e.i_AreaVigente.Value } into e_join
                                    from e in e_join.DefaultIfEmpty()

                                    join f in dbContext.regimenpensionariotrabajador on new { IdReg = c.v_IdTrabajador, eliminado = 0, vigente = 1 } equals new { IdReg = f.v_IdTrabajador, eliminado = f.i_Eliminado.Value, vigente = f.i_RegimenVigente.Value } into f_join
                                    from f in f_join.DefaultIfEmpty()

                                    join h in dbContext.contratotrabajador on new { IdTrabajador = a.v_IdTrabajador, eliminado = 0, Vigente = 1 } equals new { IdTrabajador = h.v_IdTrabajador, eliminado = h.i_Eliminado.Value, Vigente = h.i_ContratoVigente.Value } into h_join
                                    from h in h_join.DefaultIfEmpty()

                                    join i in dbContext.datahierarchy on new { Grupo = 132, eliminado = 0, Item = d.i_IdTipoIdentificacion.Value } equals new { Grupo = i.i_GroupId, eliminado = i.i_IsDeleted.Value, Item = i.i_ItemId } into i_join
                                    from i in i_join.DefaultIfEmpty()

                                    where a.i_IdPlanillaNumeracion == IdPlanilla && f.i_IdRegimenPensionario != 6

                                    select new ReportePlanillaGenAFP
                                    {
                                        v_IdTrabajador = c.v_IdTrabajador,
                                        NroCussp = f != null ? f.v_NroCussp : "",
                                        Nombre = d != null ? (d.v_PrimerNombre + " " + d.v_SegundoNombre) : "",
                                        ApPaterno = d != null ? d.v_ApePaterno : "",
                                        ApMaterno = d != null ? d.v_ApeMaterno : "",
                                        FechaIngreso = h.t_FechaInicio,
                                        FechaCese = h.t_FechaFin,
                                        TipoDoc = i != null ? i.i_ItemId : -1,
                                        NroDoc = d != null ? d.v_NroDocIdentificacion : "",
                                    }).ToList();
                    res = planilla.GroupBy(x => new { x.v_IdTrabajador }).Select(group => group.FirstOrDefault()).ToList();
                    objOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;

                objOperationResult.AdditionalInformation = "PlanillaBL.ReportePlanillaparaAFP()";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }
            return res;
        }


        #endregion
    }
}
