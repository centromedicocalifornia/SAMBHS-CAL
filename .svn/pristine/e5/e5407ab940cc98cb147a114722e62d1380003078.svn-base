using System;
using System.Collections.Generic;
using System.Linq;
using SAMBHS.Common.Resource;
using SAMBHS.Common.BE;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Dynamic;
using System.Runtime.CompilerServices;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.BE.Custom;
using SAMBHS.Contabilidad.BL;
using SAMBHS.Common.BL;

namespace SAMBHS.Planilla.BL
{
    /// <summary>
    /// EQC Oct-2015
    /// Lógica de negocio para las tabla de aplicación en planillas
    /// </summary>
    public static class AplicacionBl
    {
        #region Porcentajes AFP

        /// <summary>
        /// Actualiza la data en la tabla planillaporcafp
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pTemp_Insertar"></param>
        /// <param name="pTemp_Modificar"></param>
        /// <param name="pTemp_Eliminar"></param>
        /// <param name="ClientSession"></param>
        public static void ActualizarPorcentajesAfp(ref OperationResult pobjOperationResult, List<planillaporcafpDto> pTemp_Insertar, List<planillaporcafpDto> pTemp_Modificar, List<planillaporcafpDto> pTemp_Eliminar, List<string> ClientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    planillaporcafp objEntity;

                    #region Insertar

                    foreach (var objEntityDto in pTemp_Insertar)
                    {
                        objEntity = objEntityDto.ToEntity();
                        objEntity.t_InsertaFecha = DateTime.Now;
                        objEntity.i_InsertaIdUsuario = int.Parse(ClientSession[2]);
                        objEntity.i_Eliminado = 0;
                        dbContext.planillaporcafp.AddObject(objEntity);
                    }

                    #endregion

                    #region Modificar

                    foreach (var objEntityDto in pTemp_Modificar)
                    {
                        var objTemporal = dbContext.planillaporcafp.FirstOrDefault(p => p.i_Id == objEntityDto.i_Id);
                        if (objTemporal == null) throw new ArgumentNullException("pTemp_Modificar");
                        objEntityDto.t_ActualizaFecha = DateTime.Now;
                        objEntityDto.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntity = objEntityDto.ToEntity();
                        dbContext.planillaporcafp.ApplyCurrentValues(objEntity);
                    }

                    #endregion

                    #region Eliminar

                    foreach (var objEntityDto in pTemp_Eliminar)
                    {
                        var objTemporal = dbContext.planillaporcafp.FirstOrDefault(p => p.i_Id == objEntityDto.i_Id);
                        if (objTemporal == null) throw new ArgumentNullException("pTemp_Eliminar");
                        objEntityDto.t_ActualizaFecha = DateTime.Now;
                        objEntityDto.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntityDto.i_Eliminado = 1;
                        objEntity = objEntityDto.ToEntity();
                        dbContext.planillaporcafp.ApplyCurrentValues(objEntity);
                    }

                    #endregion

                    pobjOperationResult.Success = 1;
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ActualizarPorcentajesAFP()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        /// <summary>
        /// Realiza una consulta a la tabla planillcaporcafp deacuerdo al periodo e id del afp
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstrPeriodo"></param>
        /// <param name="pintIdAfp"></param>
        /// <returns></returns>
        public static BindingList<planillaporcafpDto> ObtenerPorcentajesAfp(ref OperationResult pobjOperationResult, string pstrPeriodo, int pintIdAfp)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var consulta = dbContext.planillaporcafp
                        .Where(p => p.i_Eliminado == 0 && p.v_Periodo == pstrPeriodo && p.i_IdAfp == pintIdAfp)
                        .ToList();

                    pobjOperationResult.Success = 1;

                    {
                        var lista = consulta.ToDTOs();

                        return new BindingList<planillaporcafpDto>(lista);
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ObtenerPorcentajesAFP()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        #endregion

        #region Porcentaje Leyes Trabajador y Empleador

        /// <summary>
        /// Actualiza la tabla planillaporcleyesempleador.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pTemp_Insertar"></param>
        /// <param name="pTemp_Modificar"></param>
        /// <param name="pTemp_Eliminar"></param>
        /// <param name="ClientSession"></param>
        public static void ActualizarPorcentajeLeyesEmpleador(ref OperationResult pobjOperationResult,
            List<planillaporcleyesempleadorDto> pTemp_Insertar, List<planillaporcleyesempleadorDto> pTemp_Modificar,
            List<planillaporcleyesempleadorDto> pTemp_Eliminar, List<string> ClientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    planillaporcleyesempleador objEntity;

                    #region Insertar

                    foreach (var objEntityDto in pTemp_Insertar)
                    {
                        objEntity = objEntityDto.ToEntity();
                        objEntity.t_InsertaFecha = DateTime.Now;
                        objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntity.i_Eliminado = 0;
                        dbContext.planillaporcleyesempleador.AddObject(objEntity);
                    }

                    #endregion

                    #region Modificar

                    foreach (var objEntityDto in pTemp_Modificar)
                    {
                        var objTemporal =
                            dbContext.planillaporcleyesempleador.FirstOrDefault(p => p.i_Id == objEntityDto.i_Id);
                        if (objTemporal == null) throw new ArgumentNullException("pTemp_Modificar");
                        objEntityDto.t_ActualizaFecha = DateTime.Now;
                        objEntityDto.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntity = objEntityDto.ToEntity();
                        dbContext.planillaporcleyesempleador.ApplyCurrentValues(objEntity);
                    }

                    #endregion

                    #region Eliminar

                    foreach (var objEntityDto in pTemp_Eliminar)
                    {
                        var objTemporal_ =
                            dbContext.planillaporcleyesempleador.FirstOrDefault(p => p.i_Id == objEntityDto.i_Id);
                        if (objTemporal_ == null) throw new ArgumentNullException("pTemp_Eliminar");
                        objEntityDto.t_ActualizaFecha = DateTime.Now;
                        objEntityDto.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntityDto.i_Eliminado = 1;
                        objEntity = objEntityDto.ToEntity();
                        dbContext.planillaporcleyesempleador.ApplyCurrentValues(objEntity);
                    }

                    #endregion

                    pobjOperationResult.Success = 1;
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ActualizarPorcentajeLeyesEmpleador()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        /// <summary>
        /// Realiza una consulta a la tabla planillaporcleyesempleador deacuerdo al perdiodo
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstrPeriodo"></param>
        /// <returns></returns>
        public static BindingList<planillaporcleyesempleadorDto> ObtenerPorcentajesLeyesEmpleador(
            ref OperationResult pobjOperationResult, string pstrPeriodo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var consulta =
                        dbContext.planillaporcleyesempleador.Where(p => p.v_Periodo == pstrPeriodo && p.i_Eliminado == 0)
                            .ToList();

                    pobjOperationResult.Success = 1;

                    return new BindingList<planillaporcleyesempleadorDto>(consulta.ToDTOs());
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ObtenerPorcentajesLeyesEmpleador()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        /// <summary>
        /// Actualiza la data en la tabla planillaporcleyestrabajador
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pTemp_Insertar"></param>
        /// <param name="pTemp_Modificar"></param>
        /// <param name="pTemp_Eliminar"></param>
        /// <param name="ClientSession"></param>
        public static void ActualizarPorcentajeLeyesTrabajador(ref OperationResult pobjOperationResult,
            List<planillaporcleyestrabajadorDto> pTemp_Insertar, List<planillaporcleyestrabajadorDto> pTemp_Modificar,
            List<planillaporcleyestrabajadorDto> pTemp_Eliminar, List<string> ClientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    planillaporcleyestrabajador objEntity;

                    #region Insertar

                    foreach (var objEntityDto in pTemp_Insertar)
                    {
                        objEntity = objEntityDto.ToEntity();
                        objEntity.t_InsertaFecha = DateTime.Now;
                        objEntity.i_InsertaIdUsuario = int.Parse(ClientSession[2]);
                        objEntity.i_Eliminado = 0;
                        dbContext.planillaporcleyestrabajador.AddObject(objEntity);
                    }

                    #endregion

                    #region Modificar

                    foreach (var objEntityDto in pTemp_Modificar)
                    {
                        var objTemporal =
                            dbContext.planillaporcleyestrabajador.FirstOrDefault(p => p.i_Id == objEntityDto.i_Id);
                        if (objTemporal == null) throw new ArgumentNullException("pTemp_Modificar");
                        objEntityDto.t_ActualizaFecha = DateTime.Now;
                        objEntityDto.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntity = objEntityDto.ToEntity();
                        dbContext.planillaporcleyestrabajador.ApplyCurrentValues(objEntity);
                    }

                    #endregion

                    #region Eliminar

                    foreach (var objEntityDto in pTemp_Eliminar)
                    {
                        var objTemporal_ =
                            dbContext.planillaporcleyestrabajador.FirstOrDefault(p => p.i_Id == objEntityDto.i_Id);
                        if (objTemporal_ == null) throw new ArgumentNullException("pTemp_Eliminar");
                        objEntityDto.t_ActualizaFecha = DateTime.Now;
                        objEntityDto.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntityDto.i_Eliminado = 1;
                        objEntity = objEntityDto.ToEntity();
                        dbContext.planillaporcleyestrabajador.ApplyCurrentValues(objEntity);
                    }

                    #endregion

                    pobjOperationResult.Success = 1;
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ActualizarPorcentajeLeyesTrabajador()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        /// <summary>
        /// Consulta a la tabla planillaporcleyestrabajador deacuerdo al periodo
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstrPeriodo"></param>
        /// <returns></returns>
        public static BindingList<planillaporcleyestrabajadorDto> ObtenerPorcentajesLeyesTrabajador(
            ref OperationResult pobjOperationResult, string pstrPeriodo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var consulta =
                        dbContext.planillaporcleyestrabajador.Where(
                            p => p.v_Periodo == pstrPeriodo && p.i_Eliminado == 0).ToList();

                    pobjOperationResult.Success = 1;

                    return new BindingList<planillaporcleyestrabajadorDto>(consulta.ToDTOs());
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ObtenerPorcentajesLeyesTrabajador()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        #endregion

        #region Afectaciones

        public static BindingList<planillaafecafpDto> ObtenerAfectacionesAfp(ref OperationResult pobjOperationResult,
            string pstrPeriodo, string pstrMes)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var consulta = (from n in dbContext.planillaafecafp
                                    join J1 in dbContext.planillaconceptos on n.v_IdConceptoPlanilla equals J1.v_IdConceptoPlanilla
                                        into J1_join
                                    from J1 in J1_join.DefaultIfEmpty()
                                    where n.i_Eliminado == 0 && n.v_Periodo == pstrPeriodo && n.v_Mes == pstrMes
                                    select new planillaafecafpDto
                                    {
                                        i_Id = n.i_Id,
                                        NombreConcepto = J1.v_Nombre,
                                        CodigoConcepto = J1.v_Codigo,
                                        i_Eliminado = n.i_Eliminado,
                                        v_Mes = n.v_Mes,
                                        v_Periodo = n.v_Periodo,
                                        v_IdConceptoPlanilla = n.v_IdConceptoPlanilla,
                                        i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                        i_Afecto = n.i_Afecto,
                                        i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                        t_InsertaFecha = n.t_InsertaFecha,
                                        t_ActualizaFecha = n.t_ActualizaFecha
                                    }
                        ).ToList();
                    pobjOperationResult.Success = 1;
                    return new BindingList<planillaafecafpDto>(consulta);
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ObtenerAfectacionesAfp()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public static void ActualizaAfectacionesAfp(ref OperationResult pobjOperationResult,
            List<planillaafecafpDto> pTemInsertarPlanillaafecafps,
            List<planillaafecafpDto> pTemModificarPlanillaafecafps,
            List<planillaafecafpDto> pTemEliminarPlanillaafecafps, List<string> clientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region Insertar

                    foreach (var objEntity in pTemInsertarPlanillaafecafps.Select(entityDto => entityDto.ToEntity()))
                    {
                        objEntity.t_InsertaFecha = DateTime.Now;
                        objEntity.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                        objEntity.i_Eliminado = 0;
                        dbContext.AddToplanillaafecafp(objEntity);
                    }

                    #endregion

                    #region Modificar

                    foreach (var objEntity in pTemModificarPlanillaafecafps.Select(entityDto => entityDto.ToEntity()))
                    {
                        var objEntitySource = dbContext.planillaafecafp.FirstOrDefault(p => p.i_Id == objEntity.i_Id);
                        objEntity.t_ActualizaFecha = DateTime.Now;
                        objEntity.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                        dbContext.planillaafecafp.ApplyCurrentValues(objEntity);
                    }

                    #endregion

                    #region Eliminar

                    foreach (var objEntity in pTemEliminarPlanillaafecafps.Select(entityDto => entityDto.ToEntity()))
                    {
                        var objEntitySource = dbContext.planillaafecafp.FirstOrDefault(p => p.i_Id == objEntity.i_Id);
                        objEntity.t_ActualizaFecha = DateTime.Now;
                        objEntity.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                        objEntity.i_Eliminado = 1;
                        dbContext.planillaafecafp.ApplyCurrentValues(objEntity);
                    }

                    #endregion

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ActualizaAfectacionesAfp()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static BindingList<planillaafecrentquintaDto> ObtenerAfectacionesRenta5T(
            ref OperationResult pobjOperationResult, string pstrPeriodo, string pstrMes)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var consulta = (from n in dbContext.planillaafecrentquinta
                                    join J1 in dbContext.planillaconceptos on n.v_IdConceptoPlanilla equals J1.v_IdConceptoPlanilla
                                        into J1_join
                                    from J1 in J1_join.DefaultIfEmpty()
                                    where n.i_Eliminado == 0 && n.v_Periodo == pstrPeriodo && n.v_Mes == pstrMes
                                    select new planillaafecrentquintaDto
                                    {
                                        i_Id = n.i_Id,
                                        NombreConcepto = J1.v_Nombre,
                                        CodigoConcepto = J1.v_Codigo,
                                        i_Eliminado = n.i_Eliminado,
                                        v_Mes = n.v_Mes,
                                        v_Periodo = n.v_Periodo,
                                        v_IdConceptoPlanilla = n.v_IdConceptoPlanilla,
                                        i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                        i_Afecto = n.i_Afecto,
                                        i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                        t_InsertaFecha = n.t_InsertaFecha,
                                        t_ActualizaFecha = n.t_ActualizaFecha
                                    }
                        ).ToList();
                    pobjOperationResult.Success = 1;
                    return new BindingList<planillaafecrentquintaDto>(consulta);
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ObtenerAfectacionesRenta5T()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public static void ActualizaAfectacionesRenta5T(ref OperationResult pobjOperationResult,
            List<planillaafecrentquintaDto> pTemInsertarPlanillaafecafps,
            List<planillaafecrentquintaDto> pTemModificarPlanillaafecafps,
            List<planillaafecrentquintaDto> pTemEliminarPlanillaafecafps, List<string> clientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region Insertar

                    foreach (var objEntity in pTemInsertarPlanillaafecafps.Select(entityDto => entityDto.ToEntity()))
                    {
                        objEntity.t_InsertaFecha = DateTime.Now;
                        objEntity.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                        objEntity.i_Eliminado = 0;
                        dbContext.AddToplanillaafecrentquinta(objEntity);
                    }

                    #endregion

                    #region Modificar

                    foreach (var objEntity in pTemModificarPlanillaafecafps.Select(entityDto => entityDto.ToEntity()))
                    {
                        var objEntitySource =
                            dbContext.planillaafecrentquinta.FirstOrDefault(p => p.i_Id == objEntity.i_Id);
                        if (objEntitySource == null) throw new ArgumentNullException("objEntitySource");
                        objEntity.t_ActualizaFecha = DateTime.Now;
                        objEntity.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                        dbContext.planillaafecrentquinta.ApplyCurrentValues(objEntity);
                    }

                    #endregion

                    #region Eliminar

                    foreach (var objEntity in pTemEliminarPlanillaafecafps.Select(entityDto => entityDto.ToEntity()))
                    {
                        var objEntitySource =
                            dbContext.planillaafecrentquinta.FirstOrDefault(p => p.i_Id == objEntity.i_Id);
                        if (objEntitySource == null) throw new ArgumentNullException("objEntitySource");
                        objEntity.t_ActualizaFecha = DateTime.Now;
                        objEntity.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                        objEntity.i_Eliminado = 1;
                        dbContext.planillaafecrentquinta.ApplyCurrentValues(objEntity);
                    }

                    #endregion

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ActualizaAfectacionesRenta5T()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static BindingList<planillaafecgratiDto> ObtenerAfectacionesGratificaciones(
            ref OperationResult pobjOperationResult, string pstrPeriodo, string pstrMes)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var consulta = (from n in dbContext.planillaafecgrati
                                    join J1 in dbContext.planillaconceptos on n.v_IdConceptoPlanilla equals J1.v_IdConceptoPlanilla
                                        into J1_join
                                    from J1 in J1_join.DefaultIfEmpty()
                                    where n.i_Eliminado == 0 && n.v_Periodo == pstrPeriodo && n.v_Mes == pstrMes
                                    select new planillaafecgratiDto
                                    {
                                        i_Id = n.i_Id,
                                        NombreConcepto = J1.v_Nombre,
                                        CodigoConcepto = J1.v_Codigo,
                                        i_Eliminado = n.i_Eliminado,
                                        v_Mes = n.v_Mes,
                                        v_Periodo = n.v_Periodo,
                                        v_IdConceptoPlanilla = n.v_IdConceptoPlanilla,
                                        i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                        i_Afecto = n.i_Afecto,
                                        i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                        t_InsertaFecha = n.t_InsertaFecha,
                                        t_ActualizaFecha = n.t_ActualizaFecha,
                                        i_IdColumna = n.i_IdColumna
                                    }
                        ).ToList();
                    pobjOperationResult.Success = 1;
                    return new BindingList<planillaafecgratiDto>(consulta);
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ObtenerAfectacionesGratificaciones()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public static void ActualizaAfectacionesGratificaciones(ref OperationResult pobjOperationResult,
            List<planillaafecgratiDto> pTemInsertarPlanillaafecafps,
            List<planillaafecgratiDto> pTemModificarPlanillaafecafps,
            List<planillaafecgratiDto> pTemEliminarPlanillaafecafps, List<string> clientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region Insertar

                    foreach (var objEntity in pTemInsertarPlanillaafecafps.Select(entityDto => entityDto.ToEntity()))
                    {
                        objEntity.t_InsertaFecha = DateTime.Now;
                        objEntity.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                        objEntity.i_Eliminado = 0;
                        dbContext.AddToplanillaafecgrati(objEntity);
                    }

                    #endregion

                    #region Modificar

                    foreach (var objEntity in pTemModificarPlanillaafecafps.Select(entityDto => entityDto.ToEntity()))
                    {
                        var objEntitySource = dbContext.planillaafecgrati.FirstOrDefault(p => p.i_Id == objEntity.i_Id);
                        if (objEntitySource == null) throw new ArgumentNullException("objEntitySource");
                        objEntity.t_ActualizaFecha = DateTime.Now;
                        objEntity.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                        dbContext.planillaafecgrati.ApplyCurrentValues(objEntity);
                    }

                    #endregion

                    #region Eliminar

                    foreach (var objEntity in pTemEliminarPlanillaafecafps.Select(entityDto => entityDto.ToEntity()))
                    {
                        var objEntitySource = dbContext.planillaafecgrati.FirstOrDefault(p => p.i_Id == objEntity.i_Id);
                        if (objEntitySource == null) throw new ArgumentNullException("objEntitySource");
                        objEntity.t_ActualizaFecha = DateTime.Now;
                        objEntity.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                        objEntity.i_Eliminado = 1;
                        dbContext.planillaafecgrati.ApplyCurrentValues(objEntity);
                    }

                    #endregion

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ActualizaAfectacionesGratificaciones()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static BindingList<planillaafecctsDto> ObtenerAfectacionesCts(ref OperationResult pobjOperationResult,
            string pstrPeriodo, string pstrMes)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var consulta = (from n in dbContext.planillaafeccts
                                    join J1 in dbContext.planillaconceptos on n.v_IdConceptoPlanilla equals J1.v_IdConceptoPlanilla
                                        into J1_join
                                    from J1 in J1_join.DefaultIfEmpty()
                                    where n.i_Eliminado == 0 && n.v_Periodo == pstrPeriodo && n.v_Mes == pstrMes
                                    select new planillaafecctsDto
                                    {
                                        i_Id = n.i_Id,
                                        NombreConcepto = J1.v_Nombre,
                                        CodigoConcepto = J1.v_Codigo,
                                        i_Eliminado = n.i_Eliminado,
                                        v_Mes = n.v_Mes,
                                        v_Periodo = n.v_Periodo,
                                        v_IdConceptoPlanilla = n.v_IdConceptoPlanilla,
                                        i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                        i_Afecto = n.i_Afecto,
                                        i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                        t_InsertaFecha = n.t_InsertaFecha,
                                        t_ActualizaFecha = n.t_ActualizaFecha,
                                        i_IdColumna = n.i_IdColumna
                                    }
                        ).ToList();
                    pobjOperationResult.Success = 1;
                    return new BindingList<planillaafecctsDto>(consulta);
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ObtenerAfectacionesCts()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public static void ActualizaAfectacionesCts(ref OperationResult pobjOperationResult,
            List<planillaafecctsDto> pTemInsertarPlanillaafecafps,
            List<planillaafecctsDto> pTemModificarPlanillaafecafps,
            List<planillaafecctsDto> pTemEliminarPlanillaafecafps, List<string> clientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region Insertar

                    foreach (var objEntity in pTemInsertarPlanillaafecafps.Select(entityDto => entityDto.ToEntity()))
                    {
                        objEntity.t_InsertaFecha = DateTime.Now;
                        objEntity.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                        objEntity.i_Eliminado = 0;
                        dbContext.AddToplanillaafeccts(objEntity);
                    }

                    #endregion

                    #region Modificar

                    foreach (var objEntity in pTemModificarPlanillaafecafps.Select(entityDto => entityDto.ToEntity()))
                    {
                        var objEntitySource = dbContext.planillaafeccts.FirstOrDefault(p => p.i_Id == objEntity.i_Id);
                        if (objEntitySource == null) throw new ArgumentNullException("objEntitySource");
                        objEntity.t_ActualizaFecha = DateTime.Now;
                        objEntity.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                        dbContext.planillaafeccts.ApplyCurrentValues(objEntity);
                    }

                    #endregion

                    #region Eliminar

                    foreach (var objEntity in pTemEliminarPlanillaafecafps.Select(entityDto => entityDto.ToEntity()))
                    {
                        var objEntitySource = dbContext.planillaafeccts.FirstOrDefault(p => p.i_Id == objEntity.i_Id);
                        if (objEntitySource == null) throw new ArgumentNullException("objEntitySource");
                        objEntity.t_ActualizaFecha = DateTime.Now;
                        objEntity.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                        objEntity.i_Eliminado = 1;
                        dbContext.planillaafeccts.ApplyCurrentValues(objEntity);
                    }

                    #endregion

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ActualizaAfectacionesCts()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static BindingList<planillaafechrsexttardDto> ObtenerAfectacionesHrsExtTardanzas(
            ref OperationResult pobjOperationResult, string pstrPeriodo, string pstrMes)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var consulta = (from n in dbContext.planillaafechrsexttard
                                    join J1 in dbContext.planillaconceptos on n.v_IdConceptoPlanilla equals J1.v_IdConceptoPlanilla
                                        into J1_join
                                    from J1 in J1_join.DefaultIfEmpty()
                                    where n.i_Eliminado == 0 && n.v_Periodo == pstrPeriodo && n.v_Mes == pstrMes
                                    select new planillaafechrsexttardDto
                                    {
                                        i_Id = n.i_Id,
                                        NombreConcepto = J1.v_Nombre,
                                        CodigoConcepto = J1.v_Codigo,
                                        i_Eliminado = n.i_Eliminado,
                                        v_Mes = n.v_Mes,
                                        v_Periodo = n.v_Periodo,
                                        v_IdConceptoPlanilla = n.v_IdConceptoPlanilla,
                                        i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                        i_HorasExtras = n.i_HorasExtras,
                                        i_Tardanza = n.i_Tardanza,
                                        i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                        t_InsertaFecha = n.t_InsertaFecha,
                                        t_ActualizaFecha = n.t_ActualizaFecha
                                    }
                        ).ToList();
                    pobjOperationResult.Success = 1;
                    return new BindingList<planillaafechrsexttardDto>(consulta);
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ObtenerAfectacionesHrsExtTardanzas()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public static void ActualizaAfectacionesHrsExtTardanzas(ref OperationResult pobjOperationResult,
            List<planillaafechrsexttardDto> pTemInsertarPlanillaafecafps,
            List<planillaafechrsexttardDto> pTemModificarPlanillaafecafps,
            List<planillaafechrsexttardDto> pTemEliminarPlanillaafecafps, List<string> clientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region Insertar

                    foreach (var objEntity in pTemInsertarPlanillaafecafps.Select(entityDto => entityDto.ToEntity()))
                    {
                        objEntity.t_InsertaFecha = DateTime.Now;
                        objEntity.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                        objEntity.i_Eliminado = 0;
                        dbContext.AddToplanillaafechrsexttard(objEntity);
                    }

                    #endregion

                    #region Modificar

                    foreach (var objEntity in pTemModificarPlanillaafecafps.Select(entityDto => entityDto.ToEntity()))
                    {
                        var objEntitySource =
                            dbContext.planillaafechrsexttard.FirstOrDefault(p => p.i_Id == objEntity.i_Id);
                        if (objEntitySource == null) throw new ArgumentNullException("objEntitySource");
                        objEntity.t_ActualizaFecha = DateTime.Now;
                        objEntity.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                        dbContext.planillaafechrsexttard.ApplyCurrentValues(objEntity);
                    }

                    #endregion

                    #region Eliminar

                    foreach (var objEntity in pTemEliminarPlanillaafecafps.Select(entityDto => entityDto.ToEntity()))
                    {
                        var objEntitySource =
                            dbContext.planillaafechrsexttard.FirstOrDefault(p => p.i_Id == objEntity.i_Id);
                        if (objEntitySource == null) throw new ArgumentNullException("objEntitySource");
                        objEntity.t_ActualizaFecha = DateTime.Now;
                        objEntity.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                        objEntity.i_Eliminado = 1;
                        dbContext.planillaafechrsexttard.ApplyCurrentValues(objEntity);
                    }

                    #endregion

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ActualizaAfectacionesHrsExtTardanzas()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static BindingList<planillaafecleyesempDto> ObtenerAfectacionesLeyesSocialesEmp(
            ref OperationResult pobjOperationResult, string pstrPeriodo, string pstrMes)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var consulta = (from n in dbContext.planillaafecleyesemp
                                    join J1 in dbContext.planillaconceptos on n.v_IdConceptoPlanilla equals J1.v_IdConceptoPlanilla
                                        into J1_join
                                    from J1 in J1_join.DefaultIfEmpty()
                                    where n.i_Eliminado == 0 && n.v_Periodo == pstrPeriodo && n.v_Mes == pstrMes
                                    select new planillaafecleyesempDto
                                    {
                                        i_Id = n.i_Id,
                                        NombreConcepto = J1.v_Nombre,
                                        CodigoConcepto = J1.v_Codigo,
                                        i_Eliminado = n.i_Eliminado,
                                        v_Mes = n.v_Mes,
                                        v_Periodo = n.v_Periodo,
                                        v_IdConceptoPlanilla = n.v_IdConceptoPlanilla,
                                        i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                        i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                        t_InsertaFecha = n.t_InsertaFecha,
                                        t_ActualizaFecha = n.t_ActualizaFecha,
                                        i_Essalud = n.i_Essalud,
                                        i_SCTR = n.i_SCTR,
                                        i_SNT = n.i_SNT,
                                        i_SeguroPensiones = n.i_SeguroPensiones
                                    }
                        ).ToList();
                    pobjOperationResult.Success = 1;
                    return new BindingList<planillaafecleyesempDto>(consulta);
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ObtenerAfectacionesLeyesSocialesEmp()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public static void ActualizaAfectacionesLeyesSocialesEmp(ref OperationResult pobjOperationResult,
            List<planillaafecleyesempDto> pTemInsertarPlanillaafecafps,
            List<planillaafecleyesempDto> pTemModificarPlanillaafecafps,
            List<planillaafecleyesempDto> pTemEliminarPlanillaafecafps, List<string> clientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region Insertar

                    foreach (var objEntity in pTemInsertarPlanillaafecafps.Select(entityDto => entityDto.ToEntity()))
                    {
                        objEntity.t_InsertaFecha = DateTime.Now;
                        objEntity.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                        objEntity.i_Eliminado = 0;
                        dbContext.AddToplanillaafecleyesemp(objEntity);
                    }

                    #endregion

                    #region Modificar

                    foreach (var objEntity in pTemModificarPlanillaafecafps.Select(entityDto => entityDto.ToEntity()))
                    {
                        var objEntitySource =
                            dbContext.planillaafecleyesemp.FirstOrDefault(p => p.i_Id == objEntity.i_Id);
                        if (objEntitySource == null) throw new ArgumentNullException("objEntitySource");
                        objEntity.t_ActualizaFecha = DateTime.Now;
                        objEntity.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                        dbContext.planillaafecleyesemp.ApplyCurrentValues(objEntity);
                    }

                    #endregion

                    #region Eliminar

                    foreach (var objEntity in pTemEliminarPlanillaafecafps.Select(entityDto => entityDto.ToEntity()))
                    {
                        var objEntitySource =
                            dbContext.planillaafecleyesemp.FirstOrDefault(p => p.i_Id == objEntity.i_Id);
                        if (objEntitySource == null) throw new ArgumentNullException("objEntitySource");
                        objEntity.t_ActualizaFecha = DateTime.Now;
                        objEntity.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                        objEntity.i_Eliminado = 1;
                        dbContext.planillaafecleyesemp.ApplyCurrentValues(objEntity);
                    }

                    #endregion

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ActualizaAfectacionesLeyesSocialesEmp()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static BindingList<planillaafecleyestrabDto> ObtenerAfectacionesLeyesSocialesTrab(
            ref OperationResult pobjOperationResult, string pstrPeriodo, string pstrMes)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var consulta = (from n in dbContext.planillaafecleyestrab
                                    join J1 in dbContext.planillaconceptos on n.v_IdConceptoPlanilla equals J1.v_IdConceptoPlanilla
                                        into J1_join
                                    from J1 in J1_join.DefaultIfEmpty()
                                    where n.i_Eliminado == 0 && n.v_Periodo == pstrPeriodo && n.v_Mes == pstrMes
                                    select new planillaafecleyestrabDto
                                    {
                                        i_Id = n.i_Id,
                                        NombreConcepto = J1.v_Nombre,
                                        CodigoConcepto = J1.v_Codigo,
                                        i_Eliminado = n.i_Eliminado,
                                        v_Mes = n.v_Mes,
                                        v_Periodo = n.v_Periodo,
                                        v_IdConceptoPlanilla = n.v_IdConceptoPlanilla,
                                        i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                        i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                        t_InsertaFecha = n.t_InsertaFecha,
                                        t_ActualizaFecha = n.t_ActualizaFecha,
                                        i_Essalud = n.i_Essalud,
                                        i_SCTR = n.i_SCTR,
                                        i_SNT = n.i_SNT,
                                        i_SeguroPensiones = n.i_SeguroPensiones,
                                        i_SCTRP = n.i_SCTRP
                                    }
                        ).ToList();
                    pobjOperationResult.Success = 1;
                    return new BindingList<planillaafecleyestrabDto>(consulta);
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ObtenerAfectacionesLeyesSocialesEmp()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public static void ActualizaAfectacionesLeyesSocialesTrab(ref OperationResult pobjOperationResult,
            List<planillaafecleyestrabDto> pTemInsertarPlanillaafecafps,
            List<planillaafecleyestrabDto> pTemModificarPlanillaafecafps,
            List<planillaafecleyestrabDto> pTemEliminarPlanillaafecafps, List<string> clientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region Insertar

                    foreach (var objEntity in pTemInsertarPlanillaafecafps.Select(entityDto => entityDto.ToEntity()))
                    {
                        objEntity.t_InsertaFecha = DateTime.Now;
                        objEntity.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                        objEntity.i_Eliminado = 0;
                        dbContext.AddToplanillaafecleyestrab(objEntity);
                    }

                    #endregion

                    #region Modificar

                    foreach (var objEntity in pTemModificarPlanillaafecafps.Select(entityDto => entityDto.ToEntity()))
                    {
                        var objEntitySource =
                            dbContext.planillaafecleyestrab.FirstOrDefault(p => p.i_Id == objEntity.i_Id);
                        if (objEntitySource == null) throw new ArgumentNullException("objEntitySource");
                        objEntity.t_ActualizaFecha = DateTime.Now;
                        objEntity.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                        dbContext.planillaafecleyestrab.ApplyCurrentValues(objEntity);
                    }

                    #endregion

                    #region Eliminar

                    foreach (var objEntity in pTemEliminarPlanillaafecafps.Select(entityDto => entityDto.ToEntity()))
                    {
                        var objEntitySource =
                            dbContext.planillaafecleyestrab.FirstOrDefault(p => p.i_Id == objEntity.i_Id);
                        if (objEntitySource == null) throw new ArgumentNullException("objEntitySource");
                        objEntity.t_ActualizaFecha = DateTime.Now;
                        objEntity.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                        objEntity.i_Eliminado = 1;
                        dbContext.planillaafecleyestrab.ApplyCurrentValues(objEntity);
                    }

                    #endregion

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ActualizaAfectacionesLeyesSocialesTrab()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static BindingList<planillaafectacionesgeneralesDto> Obtenerplanillaafectacionesgenerales(
            ref OperationResult pobjOperationResult, string pstrPeriodo, string pstrMes)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var consulta = (from n in dbContext.planillaafectacionesgenerales
                                    join J1 in dbContext.planillaconceptos on n.v_IdConceptoPlanilla equals J1.v_IdConceptoPlanilla
                                        into J1_join
                                    from J1 in J1_join.DefaultIfEmpty()
                                    where n.i_Eliminado == 0 && n.v_Periodo == pstrPeriodo && n.v_Mes == pstrMes
                                    select new planillaafectacionesgeneralesDto
                                    {
                                        i_Id = n.i_Id,
                                        NombreConcepto = J1.v_Nombre,
                                        CodigoConcepto = J1.v_Codigo,
                                        i_Eliminado = n.i_Eliminado,
                                        v_Mes = n.v_Mes,
                                        v_Periodo = n.v_Periodo,
                                        v_IdConceptoPlanilla = n.v_IdConceptoPlanilla,
                                        i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                        i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                        t_InsertaFecha = n.t_InsertaFecha,
                                        t_ActualizaFecha = n.t_ActualizaFecha,
                                        i_AFP_Afecto = n.i_AFP_Afecto,
                                        i_Leyes_Emp_Essalud = n.i_Leyes_Emp_Essalud,
                                        i_Leyes_Emp_SCTR = n.i_Leyes_Emp_SCTR,
                                        i_Leyes_Trab_ONP = n.i_Leyes_Trab_ONP,
                                        i_Leyes_Trab_SCTR = n.i_Leyes_Trab_SCTR,
                                        i_Leyes_Trab_Senati = n.i_Leyes_Trab_Senati,
                                        i_Rent5ta_Afecto = n.i_Rent5ta_Afecto
                                    }
                        ).ToList();
                    pobjOperationResult.Success = 1;
                    return new BindingList<planillaafectacionesgeneralesDto>(consulta);
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.Obtenerplanillaafectacionesgenerales()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public static void ActualizaplanillaafectacionesgeneralesDto(ref OperationResult pobjOperationResult,
            List<planillaafectacionesgeneralesDto> pTemInsertarPlanillaafecafps,
            List<planillaafectacionesgeneralesDto> pTemModificarPlanillaafecafps,
            List<planillaafectacionesgeneralesDto> pTemEliminarPlanillaafecafps, List<string> clientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region Insertar

                    foreach (var objEntity in pTemInsertarPlanillaafecafps.Select(entityDto => entityDto.ToEntity()))
                    {
                        objEntity.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                        objEntity.i_Eliminado = 0;
                        dbContext.AddToplanillaafectacionesgenerales(objEntity);
                    }

                    #endregion

                    #region Modificar

                    foreach (var objEntity in pTemModificarPlanillaafecafps.Select(entityDto => entityDto.ToEntity()))
                    {
                        var objEntitySource =
                            dbContext.planillaafectacionesgenerales.FirstOrDefault(p => p.i_Id == objEntity.i_Id);
                        if (objEntitySource == null) throw new ArgumentNullException("objEntitySource");

                        objEntity.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                        dbContext.planillaafectacionesgenerales.ApplyCurrentValues(objEntity);
                    }

                    #endregion

                    #region Eliminar

                    foreach (var objEntity in pTemEliminarPlanillaafecafps.Select(entityDto => entityDto.ToEntity()))
                    {
                        var objEntitySource =
                            dbContext.planillaafectacionesgenerales.FirstOrDefault(p => p.i_Id == objEntity.i_Id);
                        if (objEntitySource == null) throw new ArgumentNullException("objEntitySource");
                        objEntity.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                        objEntity.i_Eliminado = 1;
                        dbContext.planillaafectacionesgenerales.ApplyCurrentValues(objEntity);
                    }

                    #endregion

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ActualizaplanillaafectacionesgeneralesDto()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static void MigrarAfectacionesNuevaTabla(ref OperationResult pobjOperationResult)
        {
            try
            {
                var listaInsertar = new List<planillaafectacionesgeneralesDto>();

                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var limpiarAfectacion = dbContext.planillaafectacionesgenerales.ToList();
                    limpiarAfectacion.ForEach(o => dbContext.DeleteObject(o));
                    dbContext.SaveChanges();

                    var leyestrab = dbContext.planillaafecleyestrab.Where(p => p.i_Eliminado == 0).ToList();
                    var leyesemp = dbContext.planillaafecleyesemp.Where(p => p.i_Eliminado == 0).ToList();
                    var afectosafp = dbContext.planillaafecafp.Where(p => p.i_Eliminado == 0).ToList();
                    var conceptosExistentes = leyestrab.Select(p => new { idConcepto = p.v_IdConceptoPlanilla, mes = p.v_Mes, periodo = p.v_Periodo, tipo = "O" });
                    conceptosExistentes = conceptosExistentes.Concat(
                        leyesemp.Select(p => new { idConcepto = p.v_IdConceptoPlanilla, mes = p.v_Mes, periodo = p.v_Periodo, tipo = "E" }));
                    conceptosExistentes = conceptosExistentes.Concat(
                        afectosafp.Select(p => new { idConcepto = p.v_IdConceptoPlanilla, mes = p.v_Mes, periodo = p.v_Periodo, tipo = "A" }));
                    conceptosExistentes = conceptosExistentes.Distinct();

                    foreach (var agrupadoMesPeriodo in conceptosExistentes.GroupBy(g => new { p = g.periodo, m = g.mes, i = g.idConcepto }))
                    {
                        listaInsertar.Add(
                                new planillaafectacionesgeneralesDto
                                {
                                    v_IdConceptoPlanilla = agrupadoMesPeriodo.Key.i,
                                    i_Leyes_Trab_ONP = agrupadoMesPeriodo.Any(i => i.tipo.Equals("O")) ? 1 : 0,
                                    i_Leyes_Emp_Essalud = agrupadoMesPeriodo.Any(i => i.tipo.Equals("E")) ? 1 : 0,
                                    i_AFP_Afecto = agrupadoMesPeriodo.Any(i => i.tipo.Equals("A")) ? 1 : 0,
                                    i_Leyes_Emp_SCTR = 0,
                                    i_Leyes_Trab_SCTR = 0,
                                    i_Leyes_Trab_Senati = 0,
                                    i_Rent5ta_Afecto = 0,
                                    v_Mes = agrupadoMesPeriodo.Key.m,
                                    v_Periodo = agrupadoMesPeriodo.Key.p
                                });
                        var conceptosEssalud =
                            dbContext.planillaconceptos.Where(p => p.v_ColumnaAfectaciones.Equals("i_Essalud")).ToList();
                        foreach (var planillaconceptose in conceptosEssalud)
                        {
                            planillaconceptose.v_ColumnaAfectaciones = "i_Leyes_Emp_Essalud";
                            dbContext.planillaconceptos.ApplyCurrentValues(planillaconceptose);
                        }
                    }

                    foreach (var objEntity in listaInsertar.Select(entityDto => entityDto.ToEntity()))
                    {
                        objEntity.i_InsertaIdUsuario = 1;
                        objEntity.i_Eliminado = 0;
                        dbContext.AddToplanillaafectacionesgenerales(objEntity);
                    }

                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.MigrarAfectacionesNuevaTabla()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }
        #endregion

        #region Numeracion de Planillas
        /// <summary>
        /// Obtiene el siguiente correlativo de la numeracion de las planillas.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <returns></returns>
        public static string ObtenerSiguenteRegistro(ref OperationResult pobjOperationResult)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    if (!dbContext.planillanumeracion.Any(o => o.i_Eliminado == 0))
                    {
                        pobjOperationResult.Success = 1;
                        return "00000001";
                    }
                    var ultimoReg = dbContext.planillanumeracion.Where(o => o.i_Eliminado == 0).ToList().Max(p => int.Parse(p.v_Numero));
                    pobjOperationResult.Success = 1;
                    return (ultimoReg + 1).ToString("00000000");
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ObtenerNumeracionPlanillas()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public static planillanumeracion SugerirPlanillaActual
        {
            get
            {
                try
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var periodo = Globals.ClientSession.i_Periodo.ToString();
                        var mes = DateTime.Now.Month.ToString("00");
                        const int idPlanillaSueldo = 2;
                        var r =
                            dbContext.planillanumeracion.FirstOrDefault(
                                p => p.v_Periodo.Equals(periodo) && p.v_Mes.Equals(mes)
                                    && p.i_Eliminado == 0 && p.i_IdTipoPlanilla == idPlanillaSueldo);

                        return r;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        public static IEnumerable<planillanumeracionDto> ObtenerNumeracionPlanillas(ref OperationResult pobjOperationResult, string pstrPeriodo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var result = (from n in dbContext.planillanumeracion

                                  join J1 in dbContext.datahierarchy on new { tp = n.i_IdTipoPlanilla.Value, grupo = 112 }
                                                                      equals new { tp = J1.i_ItemId, grupo = J1.i_GroupId } into J1_join
                                  from J1 in J1_join.DefaultIfEmpty()

                                  where n.i_Eliminado == 0 && n.v_Periodo == pstrPeriodo

                                  select new planillanumeracionDto
                                  {
                                      i_Id = n.i_Id,
                                      i_Eliminado = n.i_Eliminado,
                                      i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                      i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                      i_IdTipoPlanilla = n.i_IdTipoPlanilla,
                                      i_Gratificaciones = n.i_Gratificaciones,
                                      i_NoAplicaDctoLeyAFP = n.i_NoAplicaDctoLeyAFP,
                                      i_NoAplicaSNPAFP = n.i_NoAplicaSNPAFP,
                                      i_PlanillaCerrada = n.i_PlanillaCerrada,
                                      i_Remuneraciones = n.i_Remuneraciones,
                                      i_Vacaciones = n.i_Vacaciones,
                                      t_InsertaFecha = n.t_InsertaFecha,
                                      t_ActualizaFecha = n.t_ActualizaFecha,
                                      t_FechaInicio = n.t_FechaInicio,
                                      t_FechaTermino = n.t_FechaTermino,
                                      TipoPlanilla = J1.v_Value1,
                                      d_TipoCambio = n.d_TipoCambio,
                                      d_SueldoMinimo = n.d_SueldoMinimo,
                                      v_Numero = n.v_Numero,
                                      v_Periodo = n.v_Periodo,
                                      v_Mes = n.v_Mes,
                                      v_Observaciones = n.v_Observaciones
                                  }
                        ).ToList();

                    pobjOperationResult.Success = 1;
                    return result;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ObtenerNumeracionPlanillas()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public static void InsertarPlanillaNumeracion(ref OperationResult pobjOperationResult, planillanumeracionDto objEntityDto, List<string> clientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entity = objEntityDto.ToEntity();
                    entity.t_InsertaFecha = DateTime.Now;
                    entity.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                    entity.i_Eliminado = 0;
                    dbContext.AddToplanillanumeracion(entity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.InsertarNumeracion()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static void ActualizaPlanillaNumeracion(ref OperationResult pobjOperationResult, planillanumeracionDto objEntityDto, List<string> clientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entitySource = dbContext.planillanumeracion.FirstOrDefault(p => p.i_Id == objEntityDto.i_Id);
                    if (entitySource == null) throw new ArgumentNullException("entitySource");
                    var entity = objEntityDto.ToEntity();
                    entity.t_ActualizaFecha = DateTime.Now;
                    entity.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                    dbContext.planillanumeracion.ApplyCurrentValues(entity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.InsertarNumeracion()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static void EliminaPlanillaNumeracion(ref OperationResult pobjOperationResult, int pintIdPlanillaNumeracion, List<string> clientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entitySource = dbContext.planillanumeracion.FirstOrDefault(p => p.i_Id == pintIdPlanillaNumeracion);
                    if (entitySource == null) throw new ArgumentNullException(@"pintIdPlanillaNumeracion");
                    entitySource.t_ActualizaFecha = DateTime.Now;
                    entitySource.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                    entitySource.i_Eliminado = 1;
                    dbContext.planillanumeracion.ApplyCurrentValues(entitySource);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.InsertarNumeracion()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }
        #endregion

        #region Renta Quinta

        public static void InsertarParametrosRenta5T(ref OperationResult pobjOperationResult,
            planillavaloresuitDto pobjPlanillavaloresuitDto,
            planillavaloresrentaquintaDto pobjPlanillavaloresrentaquintaDto,
            List<planillavaloresproyeccionquintaDto> plistPlanillavaloresproyeccionquintaDtos)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var pobjPlanillavaloresuit = pobjPlanillavaloresuitDto.ToEntity();
                        pobjPlanillavaloresuit.i_Eliminado = 0;
                        pobjPlanillavaloresuit.i_InsertaIdUsuario = Globals.ClientSession.i_SystemUserId;
                        dbContext.planillavaloresuit.AddObject(pobjPlanillavaloresuit);

                        var pobjPlanillavaloresrentaquinta = pobjPlanillavaloresrentaquintaDto.ToEntity();
                        pobjPlanillavaloresrentaquinta.i_Eliminado = 0;
                        pobjPlanillavaloresrentaquinta.i_InsertaIdUsuario = Globals.ClientSession.i_SystemUserId;
                        dbContext.planillavaloresrentaquinta.AddObject(pobjPlanillavaloresrentaquinta);

                        foreach (var pPlanillavaloresproyeccionquintaDto in plistPlanillavaloresproyeccionquintaDtos)
                        {
                            pPlanillavaloresproyeccionquintaDto.i_Eliminado = 0;
                            dbContext.planillavaloresproyeccionquinta.AddObject(pPlanillavaloresproyeccionquintaDto.ToEntity());
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
                pobjOperationResult.AdditionalInformation = "AplicacionBL.InsertarParametrosRenta5T()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static planillavaloresuitDto ObtenerValoresUit(ref OperationResult pobjOperationResult, string periodo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var result =
                        dbContext.planillavaloresuit.FirstOrDefault(
                            p => p.v_Periodo.Equals(periodo) && p.i_Eliminado == 0);
                    pobjOperationResult.Success = 1;
                    if (result != null)
                        return result.ToDTO();
                    return null;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ObtenerValoresUIT()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public static planillavaloresrentaquintaDto ObtenerValores5T(ref OperationResult pobjOperationResult, string periodo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entidad = dbContext.planillavaloresrentaquinta.FirstOrDefault(p => p.v_Periodo.Equals(periodo) && p.i_Eliminado == 0);
                    if (entidad == null) return null;
                    var result = entidad.ToDTO();
                    if (result != null)
                        if (result.v_IdConceptoPlanillaGratificacion != null)
                        {
                            var concepto =
                                dbContext.planillaconceptos.FirstOrDefault(
                                    p => p.v_IdConceptoPlanilla.Equals(result.v_IdConceptoPlanillaGratificacion));
                            if (concepto != null)
                                result.ConceptoGratificacion = concepto.v_Codigo;
                        }

                    if (result != null)
                        if (result.v_IdConceptoPlanillaRenta5T != null)
                        {
                            var concepto =
                                dbContext.planillaconceptos.FirstOrDefault(
                                    p => p.v_IdConceptoPlanilla.Equals(result.v_IdConceptoPlanillaRenta5T));
                            if (concepto != null)
                                result.ConceptoRenta5T = concepto.v_Codigo;
                        }

                    pobjOperationResult.Success = 1;
                    return result;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ObtenerValoresUIT()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public static List<planillavaloresproyeccionquintaDto> ObtenerProyeccionAnual(ref OperationResult pobjOperationResult, string periodo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var result =
                        dbContext.planillavaloresproyeccionquinta.Where(
                            p => p.v_Periodo.Equals(periodo) && p.i_Eliminado == 0).ToList();
                    pobjOperationResult.Success = 1;
                    if (!result.Any())
                    {
                        return new List<planillavaloresproyeccionquintaDto>
                        {
                            new planillavaloresproyeccionquintaDto { v_Periodo = periodo, i_Eliminado = 0, i_IdMes = 1, i_MesesProyectados = 12, i_GratificacionesProyectadas = 2, i_Fraccionamiento = 12},
                            new planillavaloresproyeccionquintaDto { v_Periodo = periodo, i_Eliminado = 0, i_IdMes = 2, i_MesesProyectados = 11, i_GratificacionesProyectadas = 2, i_Fraccionamiento = 12},
                            new planillavaloresproyeccionquintaDto { v_Periodo = periodo, i_Eliminado = 0, i_IdMes = 3, i_MesesProyectados = 10, i_GratificacionesProyectadas = 2, i_Fraccionamiento = 12},
                            new planillavaloresproyeccionquintaDto { v_Periodo = periodo, i_Eliminado = 0, i_IdMes = 4, i_MesesProyectados = 9, i_GratificacionesProyectadas = 2, i_Fraccionamiento = 9},
                            new planillavaloresproyeccionquintaDto { v_Periodo = periodo, i_Eliminado = 0, i_IdMes = 5, i_MesesProyectados = 8, i_GratificacionesProyectadas = 2, i_Fraccionamiento = 8},
                            new planillavaloresproyeccionquintaDto { v_Periodo = periodo, i_Eliminado = 0, i_IdMes = 6, i_MesesProyectados = 7, i_GratificacionesProyectadas = 2, i_Fraccionamiento = 8},
                            new planillavaloresproyeccionquintaDto { v_Periodo = periodo, i_Eliminado = 0, i_IdMes = 7, i_MesesProyectados = 6, i_GratificacionesProyectadas = 1, i_Fraccionamiento = 8},
                            new planillavaloresproyeccionquintaDto { v_Periodo = periodo, i_Eliminado = 0, i_IdMes = 8, i_MesesProyectados = 5, i_GratificacionesProyectadas = 1, i_Fraccionamiento = 5},
                            new planillavaloresproyeccionquintaDto { v_Periodo = periodo, i_Eliminado = 0, i_IdMes = 9, i_MesesProyectados = 4, i_GratificacionesProyectadas = 1, i_Fraccionamiento = 4},
                            new planillavaloresproyeccionquintaDto { v_Periodo = periodo, i_Eliminado = 0, i_IdMes = 10, i_MesesProyectados = 3, i_GratificacionesProyectadas = 1, i_Fraccionamiento = 4 },
                            new planillavaloresproyeccionquintaDto { v_Periodo = periodo, i_Eliminado = 0, i_IdMes = 11, i_MesesProyectados = 2, i_GratificacionesProyectadas = 1, i_Fraccionamiento = 4},
                            new planillavaloresproyeccionquintaDto { v_Periodo = periodo, i_Eliminado = 0, i_IdMes = 12, i_MesesProyectados = 1, i_GratificacionesProyectadas = 0, i_Fraccionamiento = 1}
                        };
                    }

                    return result.ToDTOs();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ObtenerValoresUIT()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public static void EliminarParametrosRenta5T(ref OperationResult pobjOperationResult, string periodo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var objplanillavaloresuit =
                       dbContext.planillavaloresuit.FirstOrDefault(
                           p => p.v_Periodo.Equals(periodo) && p.i_Eliminado == 0);

                    var objplanillavaloresrentaquinta = dbContext.planillavaloresrentaquinta.FirstOrDefault(
                            p => p.v_Periodo.Equals(periodo) && p.i_Eliminado == 0);

                    dbContext.planillavaloresrentaquinta.DeleteObject(objplanillavaloresrentaquinta);
                    dbContext.planillavaloresuit.DeleteObject(objplanillavaloresuit);

                    var proyecciones =
                       dbContext.planillavaloresproyeccionquinta.Where(
                           p => p.v_Periodo.Equals(periodo) && p.i_Eliminado == 0);

                    foreach (var planillavaloresproyeccionquinta in proyecciones)
                    {
                        dbContext.planillavaloresproyeccionquinta.DeleteObject(planillavaloresproyeccionquinta);
                    }
                    pobjOperationResult.Success = 1;
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.EliminarParametrosRenta5T()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static void ActualizarParametrosRenta5T(ref OperationResult pobjOperationResult,
            planillavaloresuitDto pobjPlanillavaloresuitDto,
            planillavaloresrentaquintaDto pobjPlanillavaloresrentaquintaDto,
            List<planillavaloresproyeccionquintaDto> plistPlanillavaloresproyeccionquintaDtos, string periodo)
        {
            try
            {
                EliminarParametrosRenta5T(ref pobjOperationResult, periodo);
                if (pobjOperationResult.Success == 0) return;
                InsertarParametrosRenta5T(ref pobjOperationResult, pobjPlanillavaloresuitDto, pobjPlanillavaloresrentaquintaDto, plistPlanillavaloresproyeccionquintaDtos);
                if (pobjOperationResult.Success == 0) return;

                pobjOperationResult.Success = 1;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ActualizarParametrosRenta5T()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        #endregion
    }

    public static class CalculoRenta5T
    {
        /// <summary>
        /// Clase que sirve para calcular la renta de Quinta Categoría ya sea por proceso de calculo de planilla o de reporte.
        /// La clase se debe llenar diferente dependiendo del caso.
        /// </summary>
        public class ParametroRenta5T
        {
            /// <summary>
            /// Periodo de la planilla que se está calculando
            /// </summary>
            public string Periodo { get; set; }

            /// <summary>
            /// Mes de la planilla que se está calculando
            /// </summary>
            public int Mes { get; set; }

            /// <summary>
            /// Ingresos que se van a incluir al calculo.
            /// Si se calculara una proyeccion para reporte, deben incluir el sueldo basico al menos.
            /// </summary>
            public List<planillavariablesingresosDto> ListaIngresos { get; set; }

            /// <summary>
            /// Suma de las retenciones anteriores al mes que se está calculando.
            /// Dejar nulo si se calcula de una variable de trabajador y no de una proyección para reporte.
            /// </summary>
            public decimal? RetencionesAnteriores { get; set; }
        }

        /// <summary>
        /// Actualiza todas las variables de una planilla, para agregar la renta de quinta categoría.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="idPlanilla"></param>
        public static void ActualizaVariablesConRenta5TPorPlanilla(ref OperationResult pobjOperationResult, int idPlanilla)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var idVariables = (from n in dbContext.planillavariablestrabajador
                                           join p in dbContext.planillanumeracion on n.i_IdPlanillaNumeracion equals p.i_Id into pJoin
                                           from p in pJoin.DefaultIfEmpty()
                                           where p.i_Id == idPlanilla && p.i_Eliminado == 0 && n.i_Eliminado == 0
                                           select n.v_IdPlanillaVariablesTrabajador).ToList();

                        foreach (var idVariable in idVariables)
                        {
                            CalcularVariablesTrabajadorRenta5T(ref pobjOperationResult, idVariable);
                            if (pobjOperationResult.Success == 0) return;
                        }
                    }
                    pobjOperationResult.Success = 1;
                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "CalculoRenta5T.ActualizaVariablesConRenta5T()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        /// <summary>
        /// Actualiza las variables del trabajador colocandoles el concepto de den
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstrIdVariableTrabajador"></param>
        /// <returns></returns>
        private static void CalcularVariablesTrabajadorRenta5T(ref OperationResult pobjOperationResult,
            string pstrIdVariableTrabajador)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region Obtiene el importe de la renta.

                    var fechaPlanilla = (from n in dbContext.planillavariablestrabajador
                                         join j1 in dbContext.planillanumeracion on n.i_IdPlanillaNumeracion equals j1.i_Id into j1_join
                                         from j1 in j1_join.DefaultIfEmpty()
                                         where n.v_IdPlanillaVariablesTrabajador.Equals(pstrIdVariableTrabajador)
                                         select new { j1.v_Periodo, IdMes = j1.t_FechaInicio.Value.Month }).FirstOrDefault();

                    if (fechaPlanilla == null) return;

                    var importeRenta5T = ObtenerImporteVariablesTrabajadorRenta5T(ref pobjOperationResult, pstrIdVariableTrabajador);
                    if (pobjOperationResult.Success == 0) return;
                    #endregion

                    #region Actualizacion de variable del trabajador.

                    if (importeRenta5T == null || importeRenta5T.Retencion <= 0) return;
                    var parametrosRenta5T =
                                    dbContext.planillavaloresrentaquinta.FirstOrDefault(
                                        p => p.v_Periodo.Equals(fechaPlanilla.v_Periodo) && p.i_Eliminado == 0);
                    if (parametrosRenta5T == null)
                        throw new Exception("No se encontraron los parámetros para la renta de 5ta.");


                    var cabecera =
                        dbContext.planillavariablestrabajador.FirstOrDefault(
                            p => p.v_IdPlanillaVariablesTrabajador.Equals(pstrIdVariableTrabajador)).ToDTO();

                    var retencionesAnteriores = dbContext.planillavariablesdescuentos
                        .Where(p => p.v_IdPlanillaVariablesTrabajador.Equals(pstrIdVariableTrabajador) &&
                                    p.v_IdConceptoPlanilla.Equals(parametrosRenta5T.v_IdConceptoPlanillaRenta5T)).ToDTOs();

                    var descuentoRenta5T = new List<planillavariablesdescuentosDto>{
                        new planillavariablesdescuentosDto
                    {
                        d_Importe = importeRenta5T.Retencion,
                        v_IdPlanillaVariablesTrabajador = pstrIdVariableTrabajador,
                        v_IdConceptoPlanilla = parametrosRenta5T.v_IdConceptoPlanillaRenta5T
                    }};

                    TrabajadorBl.ActualizarVariablesTrabajador(ref pobjOperationResult, cabecera, null, null, null,
                        descuentoRenta5T, null, retencionesAnteriores, null, null, null, null, null, null, null, null, null,
                        Globals.ClientSession.GetAsList());
                    if (pobjOperationResult.Success == 0) return;

                    pobjOperationResult.Success = 1;
                    #endregion
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "CalculoRenta5T.CalcularVariablesTrabajadorRenta5T()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        /// <summary>
        /// Actualiza las variables del trabajador colocandoles el concepto de den
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstrIdVariableTrabajador"></param>
        /// <returns></returns>
        public static Rentecion5TReporte ObtenerImporteVariablesTrabajadorRenta5T(ref OperationResult pobjOperationResult,
            string pstrIdVariableTrabajador)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var parametroCalculo = new ParametroRenta5T();

                    #region Recopila información para el cálculo.

                    var fechaPlanilla = (from n in dbContext.planillavariablestrabajador
                                         join j1 in dbContext.planillanumeracion on n.i_IdPlanillaNumeracion equals j1.i_Id into j1_join
                                         from j1 in j1_join.DefaultIfEmpty()
                                         where n.v_IdPlanillaVariablesTrabajador.Equals(pstrIdVariableTrabajador)
                                         select new { j1.v_Periodo, IdMes = j1.t_FechaInicio.Value.Month }).FirstOrDefault();

                    if (fechaPlanilla == null) return null;

                    var trabajador = (from n in dbContext.planillavariablestrabajador
                                      join t in dbContext.trabajador on n.v_IdTrabajador equals t.v_IdTrabajador into t_join
                                      from t in t_join.DefaultIfEmpty()
                                      where n.v_IdPlanillaVariablesTrabajador.Equals(pstrIdVariableTrabajador) && t.i_Eliminado == 0
                                      select t).FirstOrDefault();

                    if (trabajador == null)
                        throw new Exception("No se encontró el trabajador relacionado a la variable.");

                    if (trabajador.i_Renta5taCatExonerada == 1)
                    {
                        pobjOperationResult.Success = 1;
                        return null;
                    }

                    var ingresosVariable = (from n in dbContext.planillavariablesingresos
                                            where n.v_IdPlanillaVariablesTrabajador.Equals(pstrIdVariableTrabajador)
                                                  && n.i_Eliminado == 0
                                            select n).ToDTOs();

                    #endregion

                    #region Obtiene el importe de la renta.
                    parametroCalculo.Periodo = fechaPlanilla.v_Periodo;
                    parametroCalculo.Mes = fechaPlanilla.IdMes;
                    parametroCalculo.ListaIngresos = ingresosVariable;

                    var importeRenta5T = ObtieneImporteRenta5T(ref pobjOperationResult, parametroCalculo,
                        trabajador.v_IdTrabajador);
                    if (pobjOperationResult.Success == 0) return null;
                    return importeRenta5T;
                    #endregion

                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "CalculoRenta5T.CalcularVariablesTrabajadorRenta5T()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        /// <summary>
        /// Realiza una serie de calculos y consultas para llegar al monto retencion por 5ta categoría.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="parametroCalculo"></param>
        /// <param name="idTrabajador"></param>
        /// <returns></returns>
        private static Rentecion5TReporte ObtieneImporteRenta5T(ref OperationResult pobjOperationResult,
            ParametroRenta5T parametroCalculo, string idTrabajador)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region Recopila informacion
                    var Ubigeo = new SystemParameterBL().GetSystemParameterForCombo(ref pobjOperationResult, 112, "");
                    var r = new Rentecion5TReporte { Mes = parametroCalculo.Mes };
                    var trabajador = dbContext.trabajador.FirstOrDefault(p => p.v_IdTrabajador.Equals(idTrabajador));

                    if (trabajador == null) return null;
                    var tipoVia = dbContext.datahierarchy.FirstOrDefault(o => o.i_IsDeleted == 0 && o.i_GroupId == 113 && o.i_ItemId == trabajador.i_IdTipoVia);
                    var ocupacion = dbContext.datahierarchy.FirstOrDefault(o => o.i_IsDeleted == 0 && o.i_GroupId == 118 && o.i_ItemId == trabajador.i_Ocupacion);

                    r.CodTrabajador = trabajador.v_CodInterno;
                    r.v_IdTrabajador = trabajador.v_IdTrabajador;

                    r.ApellidoNombres = string.Join(" ", trabajador.cliente.v_ApePaterno,
                        trabajador.cliente.v_ApeMaterno, trabajador.cliente.v_PrimerNombre,
                        trabajador.cliente.v_SegundoNombre);
                    r.Direccion = trabajador.v_NombreVia + " " + trabajador.v_NumeroDomicilio;
                    r.NroDocIdentidad = trabajador.cliente.v_NroDocIdentificacion;

                    var Provincia = trabajador.cliente.i_IdProvincia == -1 || trabajador.cliente.i_IdProvincia == null ? "" : Ubigeo.FirstOrDefault(l => l.Id == trabajador.cliente.i_IdProvincia.ToString()).Value1;
                    var distrito = trabajador.cliente.i_IdDistrito == -1 || trabajador.cliente.i_IdDistrito == null ? "" : Ubigeo.FirstOrDefault(l => l.Id == trabajador.cliente.i_IdDistrito.ToString()).Value1;

                    r.Provincia = Provincia;
                    r.Distrito = distrito;
                    r.TipoVia = tipoVia != null ? tipoVia.v_Value1 : "";
                    r.Ocupacion = ocupacion != null ? ocupacion.v_Value1 : "";


                    var contrato = trabajador.contratotrabajador.FirstOrDefault(o => o.i_ContratoVigente == 1 && o.i_Eliminado == 0);
                    if (contrato == null) return null;

                    const string conceptoSueldoBase = Constants.IdConceptoPlanillaSueldoBase;
                    var sb = contrato.contratodetalletrabajador.FirstOrDefault(cc => cc.v_IdConcepto == conceptoSueldoBase && cc.i_Eliminado == 0);
                    if (sb != null)
                        r.SueldoBasico = sb.d_ImporteMensual ?? 0;

                    var valoresUit =
                        dbContext.planillavaloresuit.FirstOrDefault(
                            p => p.v_Periodo.Equals(parametroCalculo.Periodo) && p.i_Eliminado == 0);
                    if (valoresUit == null)
                        throw new Exception("No se encontraron los parámetros para la renta de 5ta.");

                    var parametrosRenta5T =
                        dbContext.planillavaloresrentaquinta.FirstOrDefault(
                            p => p.v_Periodo.Equals(parametroCalculo.Periodo) && p.i_Eliminado == 0);
                    if (parametrosRenta5T == null)
                        throw new Exception("No se encontraron los parámetros para la renta de 5ta.");

                    var listaProyecciones =
                        dbContext.planillavaloresproyeccionquinta.Where(
                            p => p.v_Periodo.Equals(parametroCalculo.Periodo) && p.i_Eliminado == 0).ToList();

                    if (!listaProyecciones.Any())
                        throw new Exception("No se encontraron los parámetros para la renta de 5ta.");

                    var mesString = parametroCalculo.Mes.ToString("00");
                    var conceptosAfectos5T = dbContext.planillaafectacionesgenerales
                        .Where(p => p.i_Rent5ta_Afecto == 1 && p.v_Mes == mesString && p.i_Eliminado == 0)
                        .Select(o => o.v_IdConceptoPlanilla).ToList();

                    var ingresosVariable = parametroCalculo.ListaIngresos
                        .Where(n => !n.v_IdConceptoPlanilla.Equals(parametrosRenta5T.v_IdConceptoPlanillaGratificacion));

                    var ingresosGratificacionVariable = parametroCalculo.ListaIngresos
                        .Where(n => n.v_IdConceptoPlanilla.Equals(parametrosRenta5T.v_IdConceptoPlanillaGratificacion));

                    var ingresosAnteriores = ObtenerIngresosAfectosAnterioresPorTrabajador(int.Parse(parametroCalculo.Periodo), parametroCalculo.Mes - 1, idTrabajador, conceptosAfectos5T, contrato.contratodetalletrabajador.ToList());
                    var retencionesAnteriores = parametroCalculo.RetencionesAnteriores ?? ObtenerRetencionesAnterioresPorTrabajador(int.Parse(parametroCalculo.Periodo), parametroCalculo.Mes, idTrabajador, parametrosRenta5T.v_IdConceptoPlanillaRenta5T);

                    #endregion

                    var proyeccionDto = listaProyecciones.FirstOrDefault(p => p.i_IdMes == parametroCalculo.Mes);
                    if (proyeccionDto == null) return null;
                    var remuneracion = ingresosVariable.Where(p => conceptosAfectos5T.Contains(p.v_IdConceptoPlanilla)).Sum(s => s.d_Importe ?? 0);
                    var gratificacion = contrato.contratodetalletrabajador.ToList().Where(p => p.i_Eliminado == 0 && p.v_IdConcepto.Equals(parametrosRenta5T.v_IdConceptoPlanillaGratificacion))
                                                         .Sum(o => o.d_ImporteMensual ?? 0);
                    var gratificacionRecibida = ingresosGratificacionVariable.Sum(p => p.d_Importe ?? 0);
                    var nroProyeccionMeses = proyeccionDto.i_MesesProyectados ?? 1;
                    var nroProyeccionGratificaciones = proyeccionDto.i_GratificacionesProyectadas ?? 1;
                    var remuneracionProyectada = remuneracion * nroProyeccionMeses;
                    var gratificacionProyectada = gratificacion * nroProyeccionGratificaciones;
                    var totalIngresosProyectados = remuneracionProyectada + gratificacionProyectada + gratificacionRecibida;
                    var rentaBruta = totalIngresosProyectados + ingresosAnteriores;
                    var deduccionUit = valoresUit.d_ValorUIT * valoresUit.i_FactorUIT;
                    var rentaNeta = rentaBruta - (deduccionUit ?? 0);
                    var importeTope1 = CalcularTopeRenta(parametrosRenta5T.ToDTO(), 1, rentaNeta, valoresUit.d_ValorUIT ?? 0);
                    var importeTope2 = CalcularTopeRenta(parametrosRenta5T.ToDTO(), 2, rentaNeta, valoresUit.d_ValorUIT ?? 0);
                    var importeTope3 = CalcularTopeRenta(parametrosRenta5T.ToDTO(), 3, rentaNeta, valoresUit.d_ValorUIT ?? 0);
                    var importeTope4 = CalcularTopeRenta(parametrosRenta5T.ToDTO(), 4, rentaNeta, valoresUit.d_ValorUIT ?? 0);
                    var importeTope5 = CalcularTopeRenta(parametrosRenta5T.ToDTO(), 5, rentaNeta, valoresUit.d_ValorUIT ?? 0);
                    var imputableAnual = importeTope1 + importeTope2 + importeTope3 + importeTope4 + importeTope5;
                    var imputableNeto = imputableAnual - retencionesAnteriores;
                    var retencionMes = imputableNeto / (proyeccionDto.i_Fraccionamiento ?? 0);
                    retencionMes = retencionMes > 0 ? Utils.Windows.DevuelveValorRedondeado(retencionMes, 2) : 0;
                    pobjOperationResult.Success = 1;

                    r.RemuneracionBruta = remuneracion;
                    r.RemuneracionProyectada = remuneracionProyectada;
                    r.GratificacionesProyectadas = gratificacionProyectada;
                    r.TotalRemuneracion = totalIngresosProyectados;
                    r.RemuneracionesAnteriores = ingresosAnteriores;
                    r.DeduccionUit = deduccionUit ?? 0;
                    r.RentaNeta = rentaNeta;
                    r.ImporteResultante = imputableAnual;
                    r.RetencionMesesAnteriores = retencionesAnteriores;
                    r.ImportePorRetener = imputableNeto;
                    r.Retencion = retencionMes;
                    r.Tope1 = importeTope1;
                    r.Tope2 = importeTope2;
                    r.Tope3 = importeTope3;
                    r.Tope4 = importeTope4;
                    r.Tope5 = importeTope5;
                    return r;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "CalculoRenta5T.ObtieneImporteRenta5T()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        /// <summary>
        /// Obtiene el Importe anual por determinado tope.
        /// </summary>
        /// <param name="parametrosTope"></param>
        /// <param name="posicionTope"></param>
        /// <param name="rentaNeta"></param>
        /// <param name="valorUit"></param>
        /// <returns></returns>
        private static decimal CalcularTopeRenta(planillavaloresrentaquintaDto parametrosTope,
            int posicionTope, decimal rentaNeta, decimal valorUit)
        {
            var result = 0M;
            var valoresParametros = parametrosTope[posicionTope];
            var porcentaje = valoresParametros.Porcentaje / 100;
            var montoUit = valoresParametros.Tope * valorUit;
            var montoUitAfecto = valoresParametros.Multiplo * valorUit;
            var montoUitAnterior = valoresParametros.TopeAnterior * valorUit;
            if (posicionTope == 1)
                result = rentaNeta > montoUit ? montoUitAfecto * porcentaje : rentaNeta * porcentaje;
            else if (posicionTope < 5)
                result = rentaNeta > montoUit
                    ? montoUitAfecto * porcentaje
                    : rentaNeta - montoUitAnterior > 0 ? (rentaNeta - montoUitAnterior) * porcentaje : 0;
            else
                result = rentaNeta > montoUitAnterior ? (rentaNeta - montoUitAnterior) * porcentaje : 0;

            return result;
        }

        /// <summary>
        /// Obtiene los ingresos anteriores del mes buscado para fines de usarlo en el calculo.
        /// </summary>
        /// <param name="mesTope"></param>
        /// <param name="idtrabajador"></param>
        /// <param name="conceptosAfectos5T"></param>
        /// <param name="detalleContrato"></param>
        /// <returns></returns>
        private static decimal ObtenerIngresosAfectosAnterioresPorTrabajador(int periodo, int mesTope,
            string idtrabajador, List<string> conceptosAfectos5T, List<contratodetalletrabajador> detalleContrato)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var listaRetorno = new List<planillavariablesingresos>();
                if (mesTope == 0) return 0M;

                for (var mes = mesTope; mes > 0; mes--)
                {
                    var conceptos = (from n in dbContext.planillavariablesingresos
                                     join p in dbContext.planillavariablestrabajador on new { id = n.v_IdPlanillaVariablesTrabajador, t = idtrabajador }
                                       equals new { id = p.v_IdPlanillaVariablesTrabajador, t = p.v_IdTrabajador } into p_join
                                     from p in p_join.DefaultIfEmpty()
                                     join l in dbContext.planillanumeracion on p.i_IdPlanillaNumeracion equals l.i_Id into l_join
                                     from l in l_join.DefaultIfEmpty()
                                     where l.t_FechaInicio.Value.Year == periodo && l.t_FechaInicio.Value.Month == mes && p.v_IdTrabajador.Equals(idtrabajador)
                                     && p.i_Eliminado == 0 && n.i_Eliminado == 0
                                     select n).ToList();
                    if (conceptos.Any())
                        listaRetorno.AddRange(conceptos);
                    else
                        listaRetorno.AddRange(detalleContrato
                            .Where(p => p.i_Fijo == 1 && p.i_Eliminado == 0)
                            .Select(n => new planillavariablesingresos
                            {
                                d_Importe = n.d_ImporteMensual ?? 0,
                                v_IdConceptoPlanilla = n.v_IdConcepto
                            }));
                }

                var r = listaRetorno.Where(p => conceptosAfectos5T.Contains(p.v_IdConceptoPlanilla)).ToList();
                return r.Sum(p => p.d_Importe ?? 0M);
            }
        }

        /// <summary>
        /// Obtiene las retenciones anteriores del mes buscado para fines de usarlo en el calculo.
        /// </summary>
        /// <param name="periodo"></param>
        /// <param name="mesTope"></param>
        /// <param name="idtrabajador"></param>
        /// <param name="idConceptoRetencion"></param>
        /// <returns></returns>
        private static decimal ObtenerRetencionesAnterioresPorTrabajador(int periodo, int mesTope, string idtrabajador,
            string idConceptoRetencion)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                if (mesTope == 0) return 0;
                
                var lista = (from n in dbContext.planillavariablesdescuentos
                             join p in dbContext.planillavariablestrabajador on new { id = n.v_IdPlanillaVariablesTrabajador, t = idtrabajador, e = 0 }
                               equals new { id = p.v_IdPlanillaVariablesTrabajador, t = p.v_IdTrabajador, e = p.i_Eliminado.Value } into p_join
                             from p in p_join.DefaultIfEmpty()
                             join l in dbContext.planillanumeracion on p.i_IdPlanillaNumeracion equals l.i_Id into l_join
                             from l in l_join.DefaultIfEmpty()
                             where l.t_FechaInicio.Value.Year == periodo && l.t_FechaInicio.Value.Month < mesTope && p.v_IdTrabajador.Equals(idtrabajador)
                             && n.i_Eliminado == 0 && n.v_IdConceptoPlanilla.Equals(idConceptoRetencion) && l.i_Eliminado == 0
                             orderby l.t_FechaInicio.Value.Month descending
                             select new PatronRetencionAcumulada
                             {
                                 Mes = l.t_FechaInicio.Value.Month,
                                 Importe = n.d_Importe ?? 0
                             }).ToList();

                var acumulado = ObtieneAcumuladoRentas(mesTope);
                return lista.Where(p => acumulado.Contains(p.Mes)).Sum(s => s.Importe);
            }
        }

        private class PatronRetencionAcumulada
        {
            public int Mes { get; set; }
            public decimal Importe { get; set; }
        }

        private static int[] ObtieneAcumuladoRentas(int mes)
        {
            switch (mes)
            {
                case 1:
                    return new int[] { };

                case 2:
                    return new int[] { };

                case 3:
                    return new int[] { };

                case 4:
                    return new int[] { 1, 2, 3 };

                case 5:
                    return new int[] { 1, 2, 3, 4 };

                case 6:
                    return new int[] { 1, 2, 3, 4 };

                case 7:
                    return new int[] { 1, 2, 3, 4 };

                case 8:
                    return new int[] { 1, 2, 3, 4, 5, 6, 7 };

                case 9:
                    return new int[] { 1, 2, 3, 4, 5, 6, 7, 8 };

                case 10:
                    return new int[] { 1, 2, 3, 4, 5, 6, 7, 8 };

                case 11:
                    return new int[] { 1, 2, 3, 4, 5, 6, 7, 8 };

                default:
                    return new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
            }
        }

        public static List<Rentecion5TReporte> ReporteRenta5T(ref OperationResult pobjOperationResult, int mes)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var periodo = Globals.ClientSession.i_Periodo ?? DateTime.Now.Year;
                    List<Rentecion5TReporte> r = new List<Rentecion5TReporte>();
                    var contratos = (from n in dbContext.contratotrabajador
                                     join t in dbContext.trabajador on n.v_IdTrabajador equals t.v_IdTrabajador into t_join
                                     from t in t_join.DefaultIfEmpty()
                                     where n.i_Eliminado == 0 && n.i_ContratoVigente == 1 && t.i_Eliminado == 0
                                     select new
                                     {
                                         IdTrabajador = t.v_IdTrabajador,
                                         ContratoDetalle = from i in dbContext.contratodetalletrabajador
                                                           where i.v_IdContrato.Equals(n.v_IdContrato) && i.i_Fijo == 1 && i.i_Eliminado == 0
                                                           select i
                                     }).ToList();

                    var planillaMes = dbContext.planillanumeracion.FirstOrDefault(p => p.t_FechaInicio.Value.Month == mes 
                        && p.t_FechaInicio.Value.Year == periodo && p.i_Eliminado == 0);

                    foreach (var contrato in contratos)
                    {
                        var variableTrabajador =
                            dbContext.planillavariablestrabajador.FirstOrDefault(
                                p => p.i_IdPlanillaNumeracion == planillaMes.i_Id && p.i_Eliminado == 0
                                && p.v_IdTrabajador.Equals(contrato.IdTrabajador));

                        if (variableTrabajador != null)
                        {
                            var renta = ObtenerImporteVariablesTrabajadorRenta5T(ref pobjOperationResult,
                                variableTrabajador.v_IdPlanillaVariablesTrabajador);
                            if (renta != null)
                                if (renta.Retencion > 0)
                                    r.Add(renta);
                        }
                    }
                    return r;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "CalculoRenta5T.ReporteRenta5T()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public static List<Certificado5taCategoriaDetalles> CertificadoRenta5taDetalles(ref OperationResult objOperationResult, List<Certificado5taCategoria> Lista5TA)
        {
            objOperationResult.Success = 1;
            List<Certificado5taCategoriaDetalles> ListaDetalles = new List<Certificado5taCategoriaDetalles>();
            try
            {

                var TextoUIT = "";
                var ValoresUIT = AplicacionBl.ObtenerValoresUit(ref  objOperationResult, Globals.ClientSession.i_Periodo.ToString());
                if (ValoresUIT != null)
                {
                    var Factor = ValoresUIT.i_FactorUIT;
                    var ValorUit = ValoresUIT.d_ValorUIT;
                    TextoUIT = "- " + Factor.ToString() + "  UIT  (" + Factor.ToString() + " x S/. " + ValorUit + " )";
                }
                if (objOperationResult.Success == 0) return null;

                var Porcentajes = AplicacionBl.ObtenerValores5T(ref objOperationResult, Globals.ClientSession.i_Periodo.ToString());
                Certificado5taCategoriaDetalles objReporte = new Certificado5taCategoriaDetalles();
                foreach (var item in Lista5TA)
                {
                    int Grupo = 1;
                    while (Grupo <= 5)
                    {
                        objReporte = new Certificado5taCategoriaDetalles();
                        switch (Grupo)
                        {

                            case 1:

                                objReporte.Grupo = "1.RENTAS BRUTAS:";
                                objReporte.Conceptos = "Sueldo o salarios, asignaciones, primas, gratificaciones, Bonificaciones, aguinaldos, comisiones, etc.";
                                objReporte.Total = "S/." + item.RentaBruta.ToString();

                                break;

                            case 2:
                                objReporte.Grupo = "2.DEDUCCIONES DE LA RENTA DE 5TA. CATEGORÍA";
                                objReporte.Conceptos = TextoUIT;
                                objReporte.Total = "S/." + item.Deducciones.ToString();

                                break;

                            case 3:

                                objReporte.Grupo = "3.RENTA NETA ";
                                objReporte.Conceptos = "";
                                objReporte.Total = "S/." + item.RentaNeta.ToString();
                                break;
                            case 4:



                                objReporte.Grupo = "4.IMPUESTO A LA RENTA";
                                objReporte.Conceptos = "Hasta " + Porcentajes.i_Tope1.ToString() + " UIT ";
                                objReporte.Total = "S/." + item.Tope1;
                                objReporte.Porcentajes = Porcentajes.d_Porcentaje1.ToString() + " %";
                                objReporte.v_IdTrabajador = item.v_IdTrabajador;
                                ListaDetalles.Add(objReporte);



                                if (item.Tope2 > 0)
                                {
                                    objReporte = new Certificado5taCategoriaDetalles();
                                    objReporte.Grupo = "4.IMPUESTO A LA RENTA";
                                    objReporte.Conceptos = "Hasta " + Porcentajes.i_Tope2.ToString() + " UIT ";
                                    objReporte.Total = "S/." + item.Tope2;
                                    objReporte.Porcentajes = Porcentajes.d_Porcentaje2.ToString() + " %";
                                    objReporte.v_IdTrabajador = item.v_IdTrabajador;
                                    ListaDetalles.Add(objReporte);
                                }
                                if (item.Tope3 > 0)
                                {
                                    objReporte = new Certificado5taCategoriaDetalles();
                                    objReporte.Grupo = "4.IMPUESTO A LA RENTA";
                                    objReporte.Conceptos = "Hasta " + Porcentajes.i_Tope3.ToString() + " UIT ";
                                    objReporte.Total = "S/." + item.Tope3;
                                    objReporte.Porcentajes = Porcentajes.d_Porcentaje3.ToString() + " %";
                                    objReporte.v_IdTrabajador = item.v_IdTrabajador;
                                    ListaDetalles.Add(objReporte);
                                }
                                if (item.Tope4 > 0)
                                {
                                    objReporte = new Certificado5taCategoriaDetalles();
                                    objReporte.Grupo = "4.IMPUESTO A LA RENTA";
                                    objReporte.Conceptos = "Hasta " + Porcentajes.i_Tope4.ToString() + " UIT ";
                                    objReporte.Total = "S/." + item.Tope4;
                                    objReporte.Porcentajes = Porcentajes.d_Porcentaje4.ToString() + " %";
                                    objReporte.v_IdTrabajador = item.v_IdTrabajador;
                                    ListaDetalles.Add(objReporte);
                                }
                                if (item.Tope5 > 0)
                                {
                                    objReporte = new Certificado5taCategoriaDetalles();
                                    objReporte.Grupo = "4.IMPUESTO A LA RENTA";
                                    objReporte.Conceptos = "Hasta más " + " UIT ";
                                    objReporte.Total = "S/." + item.Tope5;
                                    objReporte.Porcentajes = Porcentajes.d_Porcentaje4Superior.ToString() + " %" + " (1)";
                                    objReporte.v_IdTrabajador = item.v_IdTrabajador;
                                    ListaDetalles.Add(objReporte);
                                }
                                break;

                            case 5:

                                objReporte.Grupo = "5.IMPUESTO TOTAL RETENIDO";
                                objReporte.Conceptos = "";
                                objReporte.Total = "S/." + item.ImpuestoTotalRetenido.ToString();
                                break;



                        }

                        if (Grupo != 4)
                        {
                            objReporte.v_IdTrabajador = item.v_IdTrabajador;
                            ListaDetalles.Add(objReporte);

                        }
                        Grupo++;
                    }

                }
                return ListaDetalles.OrderBy(o => o.v_IdTrabajador).ToList();

            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;
            }
        }

        public static List<Certificado5taCategoria> CertificadoRenta5ta(ref OperationResult objOperationResult, int mes)
        {
            try
            {
                objOperationResult.Success = 1;
                var Reporte5T = ReporteRenta5T(ref objOperationResult, mes);
                if (objOperationResult.Success == 0) return null;
                Certificado5taCategoria objReporte = new Certificado5taCategoria();
                List<Certificado5taCategoria> ListaCertificado5ta = new List<Certificado5taCategoria>();

                var Trabajador = new ReciboHonorarioBL().ObtenerTrabajadorbyIdTrabajador(ref objOperationResult, Globals.ClientSession.v_IdRepresentanteLegal);
                var NombreRepresentanteLegal = Trabajador != null ? Trabajador.v_RazonSocial : "";
                var RucRepresentanteLegal = Trabajador != null ? Trabajador.v_NroDocIdentificacion : "";
                var DireccionRepresentanteLegal = Trabajador != null ? Trabajador.Via + " " + Trabajador.v_NombreVia : "";
                var DistritoRepresentanteLegal = Trabajador != null ? Trabajador.Distrito + " , " + Trabajador.Departamento : "";
                foreach (var trab in Reporte5T)
                {
                    objReporte = new Certificado5taCategoria();
                    objReporte.Titulo = "CERTIFICADO DE RENTAS Y RETENCIONES POR RENTAS DE QUINTA CATEGORIA \n" + "EJERCICIO : " + Globals.ClientSession.i_Periodo.ToString();
                    objReporte.FechaEmision = "Fecha Emisión : " + DateTime.Now.ToShortDateString();
                    objReporte.Texto1 = NombreRepresentanteLegal.ToUpper() + " " + "con R.U.C. " + RucRepresentanteLegal + " " + "domiciliado en " + DireccionRepresentanteLegal + " " + DistritoRepresentanteLegal;
                    objReporte.Texto2 = "Certifica que Don(ña) " + trab.ApellidoNombres.ToUpper() + " , identificado(a) con DNI Nº" + trab.NroDocIdentidad + " ," + " domiciliado en " + trab.TipoVia + " " + trab.Direccion + " -  " + trab.Distrito + " - " + trab.Provincia + ", en su calidad de " + trab.Ocupacion + " se le ha retenido el importe de  S/." + trab.Retencion.ToString() + "  como pago a cuenta del Impuesto a la Renta de quinta categoría, correspondiente al ejercicio gravable " + Globals.ClientSession.i_Periodo.ToString() + " , calculado en base a las siguientes rentas, deducciones y créditos:";
                    objReporte.v_IdTrabajador = trab.v_IdTrabajador;
                    objReporte.RentaBruta = trab.GratificacionesProyectadas + trab.TotalRemuneracion;
                    objReporte.Deducciones = trab.DeduccionUit;
                    objReporte.RentaNeta = trab.RentaNeta;
                    objReporte.ImpuestoRenta = 0;
                    objReporte.Tope1 = trab.Tope1;
                    objReporte.Tope2 = trab.Tope2;
                    objReporte.Tope3 = trab.Tope3;
                    objReporte.Tope4 = trab.Tope4;
                    objReporte.Tope5 = trab.Tope5;
                    objReporte.ImpuestoTotalRetenido = trab.Retencion;

                    ListaCertificado5ta.Add(objReporte);

                }
                return ListaCertificado5ta.OrderBy(o => o.v_IdTrabajador).ToList();
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;
            }
        }
    }

    /// <summary>
    /// EQC Oct-2015
    /// Lógica de negocio para los procesos de trabajador dentro de planillas.
    /// </summary>
    public static class TrabajadorBl
    {
        /// <summary>
        /// Obtiene la coleccion de Variables del trabajador para mostar en la bandeja.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pintIdPlanilla">Id De la planilla por la que se quiere filtrar</param>
        /// <returns></returns>
        /// 


        public static IEnumerable<PlanillaVariablesBandejaDto> ObtenerVariablesTrabajadorBandeja(
            ref OperationResult pobjOperationResult, int pintIdPlanilla, string pstrIdTrabajador)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var consulta = (from n in dbContext.planillavariablestrabajador

                                    join J3 in dbContext.trabajador on n.v_IdTrabajador equals J3.v_IdTrabajador into J3_join
                                    from J3 in J3_join.DefaultIfEmpty()

                                    join J1 in dbContext.systemuser on new { i_InsertUserId = n.i_InsertaIdUsuario.Value }
                                        equals new { i_InsertUserId = J1.i_SystemUserId } into J1_join
                                    from J1 in J1_join.DefaultIfEmpty()

                                    join J2 in dbContext.systemuser on new { i_UpdateUserId = n.i_ActualizaIdUsuario.Value }
                                        equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                                    from J2 in J2_join.DefaultIfEmpty()

                                    where n.i_IdPlanillaNumeracion == pintIdPlanilla && n.i_Eliminado == 0

                                    select new PlanillaVariablesBandejaDto
                                    {
                                        NombreTrabajador =
                                            J3.cliente.v_ApePaterno + " " + J3.cliente.v_ApeMaterno + " " +
                                            J3.cliente.v_PrimerNombre,
                                        Contrato = n.contratotrabajador.v_NroContrato,
                                        IdVariableTrabajador = n.v_IdPlanillaVariablesTrabajador,
                                        IdTrabajador = n.v_IdTrabajador,
                                        UsuarioActualiza = J2.v_UserName,
                                        UsuarioCrea = J1.v_UserName,
                                        t_InsertaFecha = n.t_InsertaFecha,
                                        t_ActualizaFecha = n.t_ActualizaFecha,
                                        NroDocumentoTrabajador = J3.cliente.v_NroDocIdentificacion,

                                    }
                        ).ToList();

                    if (!string.IsNullOrWhiteSpace(pstrIdTrabajador))
                        consulta = consulta.Where(p => p.IdTrabajador == pstrIdTrabajador).ToList();

                    pobjOperationResult.Success = 1;
                    return consulta;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ObtenerVariablesTrabajadorBandeja()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        /// <summary>
        /// Obtiene los trabajadores para realizar una busqueda rápida.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <returns></returns>
        public static IEnumerable<trabajadorconsultaDto> ObtenerTrabajadoresParaBusqueda(
            ref OperationResult pobjOperationResult)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var consulta = dbContext.trabajador.ToList().Where(p => p.i_Eliminado == 0).ToList()
                        .Select(n => new trabajadorconsultaDto
                        {
                            NombreApellidos =
                                (n.cliente.v_ApePaterno + " " + n.cliente.v_ApeMaterno + " " + n.cliente.v_PrimerNombre)
                                    .Trim(),
                            NroDocumento = n.cliente.v_NroDocIdentificacion,
                            Codigo = n.v_CodInterno,
                            IdTrabajador = n.v_IdTrabajador
                        }).ToList();

                    foreach (var trabajador in consulta)
                    {
                        var contratoVigente = ObtenerContratoVigente(ref pobjOperationResult, trabajador.IdTrabajador);
                        if (pobjOperationResult.Success == 0) return null;

                        trabajador.NroContratoVigente = contratoVigente != null
                            ? contratoVigente.v_NroContrato
                            : @"#No Encontrado#";

                        trabajador.IdContratoVigente = contratoVigente != null
                            ? contratoVigente.v_IdContrato
                            : string.Empty;
                    }

                    pobjOperationResult.Success = 1;
                    return consulta;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ObtenerTrabajadoresParaBusqueda()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        /// <summary>
        /// Obtiene la entidad de contrato vigente del empleado.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstrIdTrabajador"></param>
        /// <returns></returns>
        private static contratotrabajador ObtenerContratoVigente(ref OperationResult pobjOperationResult,
            string pstrIdTrabajador)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var consulta =
                        dbContext.contratotrabajador.FirstOrDefault(
                            p => p.i_ContratoVigente == 1 && p.v_IdTrabajador == pstrIdTrabajador && p.i_Eliminado == 0);

                    pobjOperationResult.Success = 1;
                    return consulta;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ObtenerContratoVigente()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        /// <summary>
        /// Devuelve datos para llenar la busqueda de planillas.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstrPeriodo">Periodo en el que se quiere buscar.</param>
        /// <returns>planillanumeracionDto</returns>
        public static IEnumerable<planillanumeracionDto> ObtenerBusquedaPlanillas(
            ref OperationResult pobjOperationResult, string pstrPeriodo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var data =
                        dbContext.planillanumeracion.Where(p => p.v_Periodo == pstrPeriodo && p.i_Eliminado == 0)
                            .ToDTOs();
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

        /// <summary>
        /// Obtiene las variables del trabajador para la consulta.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstrIdVariablesTrabajador"></param>
        /// <returns></returns>
        public static planillavariablestrabajadorDto ObtenerVariablesTrabajador(
            ref OperationResult pobjOperationResult, string pstrIdVariablesTrabajador)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var consulta = (from n in dbContext.planillavariablestrabajador
                                    join J1 in dbContext.trabajador on n.v_IdTrabajador equals J1.v_IdTrabajador into J1_join
                                    from J1 in J1_join.DefaultIfEmpty()

                                    join J2 in dbContext.contratotrabajador on n.v_IdContrato equals J2.v_IdContrato into J2_join
                                    from J2 in J2_join.DefaultIfEmpty()

                                    join J3 in dbContext.planillanumeracion on n.i_IdPlanillaNumeracion equals J3.i_Id into J3_join
                                    from J3 in J3_join.DefaultIfEmpty()

                                    where n.v_IdPlanillaVariablesTrabajador == pstrIdVariablesTrabajador

                                    select new planillavariablestrabajadorDto
                                    {
                                        v_IdPlanillaVariablesTrabajador = n.v_IdPlanillaVariablesTrabajador,
                                        v_IdContrato = n.v_IdContrato,
                                        v_IdTrabajador = n.v_IdTrabajador,
                                        t_FechaVacacionesFin = n.t_FechaVacacionesFin,
                                        t_ActualizaFecha = n.t_ActualizaFecha,
                                        t_FechaVacacionesInicio = n.t_FechaVacacionesInicio,
                                        t_InsertaFecha = n.t_InsertaFecha,
                                        NombreTrabajador =
                                            J1 != null
                                                ? J1.cliente.v_ApePaterno + " " + J1.cliente.v_ApeMaterno + " " +
                                                  J1.cliente.v_PrimerNombre
                                                : "#No Encontrado#",
                                        NumeroContrato = J2 != null ? J2.v_NroContrato : "#No Encontrado#",
                                        i_DiasNoLaborados = n.i_DiasNoLaborados,
                                        NumeroPlanilla = J3 != null ? J3.v_Numero : "#No Encontrado#",
                                        i_IdPlanillaNumeracion = n.i_IdPlanillaNumeracion,
                                        i_Eliminado = n.i_Eliminado,
                                        i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                        i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                        i_AfectoSCTR = n.i_AfectoSCTR,
                                        i_AfectoSCTRPen = n.i_AfectoSCTRPen,
                                        i_AfectoSenati = n.i_AfectoSenati,
                                        i_DiasLaborados = n.i_DiasLaborados,
                                        i_DiasLaboradosBP = n.i_DiasLaboradosBP,
                                        i_DiasSubsidiados = n.i_DiasSubsidiados,
                                        i_Movilidad = n.i_Movilidad,
                                        i_NoAplicarDCTOLeyesAFP = n.i_NoAplicarDCTOLeyesAFP,
                                        i_Pago = n.i_Pago,
                                        d_Imp = n.d_Imp,
                                        d_TotalIngresoDolares = n.d_TotalIngresoDolares,
                                        d_TotalIngresoSoles = n.d_TotalIngresoSoles,
                                        d_TotalDescuentoDolares = n.d_TotalDescuentoDolares,
                                        d_TiempoTardanza = n.d_TiempoTardanza,
                                        d_TotalDescuentoSoles = n.d_TotalDescuentoSoles,
                                        d_HorasLaboradosBP = n.d_HorasLaboradosBP,
                                        i_TieneVacaciones = n.i_TieneVacaciones ?? 0,
                                    }
                        ).FirstOrDefault();

                    pobjOperationResult.Success = 1;
                    return consulta;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ObtenerVariablesTrabajador()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        /// <summary>
        /// Obtiene las horas extras de las variables del trabajador.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstrIdVariablesTrabajador"></param>
        /// <returns></returns>
        public static BindingList<planillavariableshorasextrasDto> ObtenerVariablesHorasExtras(
            ref OperationResult pobjOperationResult, string pstrIdVariablesTrabajador)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var consulta =
                        dbContext.planillavariableshorasextras.Where(
                            p => p.v_IdPlanillaVariablesTrabajador == pstrIdVariablesTrabajador && p.i_Eliminado == 0)
                            .ToList();
                    pobjOperationResult.Success = 1;

                    return new BindingList<planillavariableshorasextrasDto>(consulta.ToDTOs());
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ObtenerVariablesHorasExtras()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        /// <summary>
        /// Obtiene los descuentos de las variables del trabajador.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstrIdVariablesTrabajador"></param>
        /// <returns></returns>
        public static BindingList<planillavariablesdescuentosDto> ObtenerVariablesDescuentos(
            ref OperationResult pobjOperationResult, string pstrIdVariablesTrabajador)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var consulta = (from n in dbContext.planillavariablesdescuentos

                                    join J1 in dbContext.planillaconceptos on n.v_IdConceptoPlanilla equals J1.v_IdConceptoPlanilla
                                        into J1_join
                                    from J1 in J1_join.DefaultIfEmpty()

                                    where n.v_IdPlanillaVariablesTrabajador == pstrIdVariablesTrabajador && n.i_Eliminado == 0

                                    select new planillavariablesdescuentosDto
                                    {
                                        CodigoConcepto = J1.v_Codigo,
                                        DescripcionConcepto = J1.v_Nombre,
                                        v_IdPlanillaVariablesTrabajador = n.v_IdPlanillaVariablesTrabajador,
                                        v_IdConceptoPlanilla = n.v_IdConceptoPlanilla,
                                        v_IdPlanillaVariablesDescuentos = n.v_IdPlanillaVariablesDescuentos,
                                        t_InsertaFecha = n.t_InsertaFecha,
                                        t_ActualizaFecha = n.t_ActualizaFecha,
                                        i_Eliminado = n.i_Eliminado,
                                        i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                        i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                        i_IdMoneda = n.i_IdMoneda,
                                        d_Importe = n.d_Importe
                                    }
                        ).ToList();

                    pobjOperationResult.Success = 1;

                    return new BindingList<planillavariablesdescuentosDto>(consulta);
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ObtenerVariablesDescuentos()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        /// <summary>
        /// Obtiene los ingresos de las variables del trabajador.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstrIdVariablesTrabajador"></param>
        /// <returns></returns>
        public static BindingList<planillavariablesingresosDto> ObtenerVariablesIngresos(
            ref OperationResult pobjOperationResult, string pstrIdVariablesTrabajador)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var consulta = (from n in dbContext.planillavariablesingresos

                                    join J1 in dbContext.planillaconceptos on n.v_IdConceptoPlanilla equals J1.v_IdConceptoPlanilla
                                        into J1_join
                                    from J1 in J1_join.DefaultIfEmpty()

                                    where n.v_IdPlanillaVariablesTrabajador == pstrIdVariablesTrabajador && n.i_Eliminado == 0

                                    select new planillavariablesingresosDto
                                    {
                                        CodigoConcepto = J1.v_Codigo,
                                        DescripcionConcepto = J1.v_Nombre,
                                        v_IdPlanillaVariablesTrabajador = n.v_IdPlanillaVariablesTrabajador,
                                        v_IdConceptoPlanilla = n.v_IdConceptoPlanilla,
                                        v_IdPlanillaVariablesIngresos = n.v_IdPlanillaVariablesIngresos,
                                        t_InsertaFecha = n.t_InsertaFecha,
                                        t_ActualizaFecha = n.t_ActualizaFecha,
                                        i_Eliminado = n.i_Eliminado,
                                        i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                        i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                        i_IdMoneda = n.i_IdMoneda,
                                        d_Importe = n.d_Importe
                                    }
                        ).ToList();

                    pobjOperationResult.Success = 1;

                    return new BindingList<planillavariablesingresosDto>(consulta);
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ObtenerVariablesIngresos()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        /// <summary>
        /// Obtiene los ingresos que estan marcados como Fijos en el contrato del trabajador que se encuentra vigente.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstrIdTrabajador"></param>
        /// <returns></returns>
        public static BindingList<planillavariablesingresosDto> ObtenerIngresosFijosDesdeContrato(
            ref OperationResult pobjOperationResult, string pstrIdTrabajador)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var consulta = (from n in dbContext.contratodetalletrabajador

                                    join J2 in dbContext.planillaconceptos on n.v_IdConcepto equals J2.v_IdConceptoPlanilla into
                                        J2_join
                                    from J2 in J2_join.DefaultIfEmpty()

                                    where
                                        n.contratotrabajador.v_IdTrabajador == pstrIdTrabajador &&
                                        n.contratotrabajador.i_ContratoVigente == 1
                                        && n.i_Eliminado == 0 && n.contratotrabajador.i_Eliminado == 0 && n.i_Fijo == 1

                                    select new planillavariablesingresosDto
                                    {
                                        v_IdConceptoPlanilla = n.v_IdConcepto,
                                        CodigoConcepto = J2.v_Codigo,
                                        DescripcionConcepto = J2.v_Nombre,
                                        d_Importe = n.d_ImporteMensual ?? 0,
                                        i_IdMoneda = n.contratotrabajador.i_IdMonedaContrato ?? -1
                                    }
                        ).ToList();

                    pobjOperationResult.Success = 1;
                    return new BindingList<planillavariablesingresosDto>(consulta);
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ObtenerIngresosFijosDesdeContrato()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        /// <summary>
        /// Obtiene las aportaciones de las variables del trabajador.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstrIdVariablesTrabajador"></param>
        /// <returns></returns>
        public static BindingList<planillavariablesaportacionesDto> ObtenerVariablesAportaciones(
            ref OperationResult pobjOperationResult, string pstrIdVariablesTrabajador)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var consulta = (from n in dbContext.planillavariablesaportaciones

                                    join J1 in dbContext.planillaconceptos on n.v_IdConceptoPlanilla equals J1.v_IdConceptoPlanilla
                                        into J1_join
                                    from J1 in J1_join.DefaultIfEmpty()

                                    where n.v_IdPlanillaVariablesTrabajador == pstrIdVariablesTrabajador && n.i_Eliminado == 0

                                    select new planillavariablesaportacionesDto
                                    {
                                        CodigoConcepto = J1.v_Codigo,
                                        DescripcionConcepto = J1.v_Nombre,
                                        v_IdPlanillaVariablesTrabajador = n.v_IdPlanillaVariablesTrabajador,
                                        v_IdConceptoPlanilla = n.v_IdConceptoPlanilla,
                                        v_IdPlanillaVariablesAportaciones = n.v_IdPlanillaVariablesAportaciones,
                                        t_InsertaFecha = n.t_InsertaFecha,
                                        t_ActualizaFecha = n.t_ActualizaFecha,
                                        i_Eliminado = n.i_Eliminado,
                                        i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                        i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                        i_IdMoneda = n.i_IdMoneda,
                                        d_Importe = n.d_Importe,
                                        Guardado = true
                                    }
                        ).ToList();

                    pobjOperationResult.Success = 1;

                    if (!consulta.Any())
                    {
                        var aportacionEssaludObligatoria = new planillavariablesaportacionesDto
                        {
                            d_Importe = 0,
                            i_IdMoneda = 1,
                            v_IdConceptoPlanilla = "N001-PN000000096", //Essalud aporte obligatorio.
                            CodigoConcepto = "A001",
                            DescripcionConcepto = "ESSALUD",
                            Guardado = false
                        };

                        consulta.Add(aportacionEssaludObligatoria);
                    }

                    return new BindingList<planillavariablesaportacionesDto>(consulta);
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ObtenerVariablesAportaciones()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        /// <summary>
        /// Consulta si la variable del trabajador ya existe en la planilla indicada, si existe retorna el id de la variable del trabajador, sino returna nulo.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pintIdPlanilla">Entero Id de la planilla.</param>
        /// <param name="pstrIdTrabajador">Cadena id del trabajador.</param>
        /// <returns></returns>
        public static string ObtenerVariableTrabajadorSiExiste(ref OperationResult pobjOperationResult,
            int pintIdPlanilla, string pstrIdTrabajador)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var consulta =
                        dbContext.planillavariablestrabajador.FirstOrDefault(
                            p =>
                                p.i_IdPlanillaNumeracion == pintIdPlanilla && p.v_IdTrabajador == pstrIdTrabajador &&
                                p.i_Eliminado == 0);

                    pobjOperationResult.Success = 1;
                    return consulta != null ? consulta.v_IdPlanillaVariablesTrabajador : string.Empty;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ObtenerVariableTrabajadorSiExiste()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        /// <summary>
        /// Obtiene el listado de Dias no Laborados
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstrIdVariablesTrabajador"></param>
        /// <returns></returns>
        public static BindingList<planillavariablesdiasnolabsubsidiadosDto> ObtenerDiasNoLaborados(
            ref OperationResult pobjOperationResult, string pstrIdVariablesTrabajador)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var consulta = (from n in dbContext.planillavariablesdiasnolabsubsidiados

                                    join J1 in dbContext.datahierarchy on new { id = n.i_IdConcepto.Value, grupo = 143 }
                                        equals new { id = J1.i_ItemId, grupo = J1.i_GroupId } into J1_join
                                    from J1 in J1_join.DefaultIfEmpty()

                                    where
                                        n.v_IdPlanillaVariablesTrabajador == pstrIdVariablesTrabajador && n.i_IdTipoConcepto == 1 &&
                                        n.i_Eliminado == 0

                                    select new planillavariablesdiasnolabsubsidiadosDto
                                    {
                                        v_IdPlanillaVariablesTrabajador = n.v_IdPlanillaVariablesTrabajador,
                                        i_Eliminado = n.i_Eliminado,
                                        t_InsertaFecha = n.t_InsertaFecha,
                                        t_ActualizaFecha = n.t_ActualizaFecha,
                                        i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                        i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                        i_Id = n.i_Id,
                                        i_IdTipoConcepto = n.i_IdTipoConcepto,
                                        i_IdConcepto = n.i_IdConcepto,
                                        i_CantidadDias = n.i_CantidadDias,
                                        CodigoConcepto = J1.v_Value2,
                                        NombreConcepto = J1.v_Value1
                                    }
                        ).ToList();

                    pobjOperationResult.Success = 1;
                    return new BindingList<planillavariablesdiasnolabsubsidiadosDto>(consulta);
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ObtenerDiasNoLaborados()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        /// <summary>
        /// Obtiene el listado de Dias Subsidiados
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstrIdVariablesTrabajador"></param>
        /// <returns></returns>
        public static BindingList<planillavariablesdiasnolabsubsidiadosDto> ObtenerDiasSubsidiados(
            ref OperationResult pobjOperationResult, string pstrIdVariablesTrabajador)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var consulta = (from n in dbContext.planillavariablesdiasnolabsubsidiados

                                    join J1 in dbContext.datahierarchy on new { id = n.i_IdConcepto.Value, grupo = 144 }
                                        equals new { id = J1.i_ItemId, grupo = J1.i_GroupId } into J1_join
                                    from J1 in J1_join.DefaultIfEmpty()

                                    where
                                        n.v_IdPlanillaVariablesTrabajador == pstrIdVariablesTrabajador && n.i_IdTipoConcepto == 2 &&
                                        n.i_Eliminado == 0

                                    select new planillavariablesdiasnolabsubsidiadosDto
                                    {
                                        v_IdPlanillaVariablesTrabajador = n.v_IdPlanillaVariablesTrabajador,
                                        i_Eliminado = n.i_Eliminado,
                                        t_InsertaFecha = n.t_InsertaFecha,
                                        t_ActualizaFecha = n.t_ActualizaFecha,
                                        i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                        i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                        i_Id = n.i_Id,
                                        i_IdTipoConcepto = n.i_IdTipoConcepto,
                                        i_IdConcepto = n.i_IdConcepto,
                                        i_CantidadDias = n.i_CantidadDias,
                                        CodigoConcepto = J1.v_Value2,
                                        NombreConcepto = J1.v_Value1
                                    }
                        ).ToList();

                    pobjOperationResult.Success = 1;
                    return new BindingList<planillavariablesdiasnolabsubsidiadosDto>(consulta);
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ObtenerDiasNoLaborados()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        #region Métodos para el CRUD

        /// <summary>
        /// Inserta los datos de variables de trabajador.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pobjEntityDto">Entidad Dto de las variables del trabajador.</param>
        /// <param name="pTempInsertarHorasExtras">Lista para ingresar las horas extras.</param>
        /// <param name="pTempInsertarDescuentos">Lista para ingresar los decuentos.</param>
        /// <param name="pTempInsertarIngresos">Lista para ingresar los ingresos.</param>
        /// <param name="pTempInsertarAportaciones">Lista para ingresar las aportaciones.</param>
        /// <param name="clientSession"></param>
        public static string InsertarVariablesTrabajador(ref OperationResult pobjOperationResult,
            planillavariablestrabajadorDto pobjEntityDto,
            List<planillavariableshorasextrasDto> pTempInsertarHorasExtras,
            List<planillavariablesdescuentosDto> pTempInsertarDescuentos,
            List<planillavariablesingresosDto> pTempInsertarIngresos,
            List<planillavariablesdiasnolabsubsidiadosDto> pTempInsertarDiasLab,
            List<planillavariablesaportacionesDto> pTempInsertarAportaciones,
            List<string> clientSession)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var objSecuentialBl = new SecuentialBL();
                        var objEntity = pobjEntityDto.ToEntity();
                        var secuentialId = 0;

                        #region Inserta Cabecera

                        objEntity.t_InsertaFecha = DateTime.Now;
                        objEntity.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                        objEntity.i_Eliminado = 0;

                        var intNodeId = int.Parse(clientSession[0]);
                        secuentialId = objSecuentialBl.GetNextSecuentialId(intNodeId, 93);
                        var newIdVariableTrabajador = Utils.GetNewId(int.Parse(clientSession[0]), secuentialId, "VT");
                        objEntity.v_IdPlanillaVariablesTrabajador = newIdVariableTrabajador;
                        dbContext.AddToplanillavariablestrabajador(objEntity);
                        dbContext.SaveChanges();

                        #endregion

                        #region Insertar horas extras

                        foreach (
                            var objEntidad in pTempInsertarHorasExtras.Select(objEntidadDto => objEntidadDto.ToEntity())
                            )
                        {
                            objEntidad.t_InsertaFecha = DateTime.Now;
                            objEntidad.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                            objEntidad.i_Eliminado = 0;
                            objEntidad.v_IdPlanillaVariablesTrabajador = objEntity.v_IdPlanillaVariablesTrabajador;
                            dbContext.AddToplanillavariableshorasextras(objEntidad);
                        }

                        #endregion

                        #region Insertar descuentos

                        foreach (
                            var objEntidad in pTempInsertarDescuentos.Select(objEntidadDto => objEntidadDto.ToEntity()))
                        {
                            var newIdVariablesDescuentos = Utils.GetNewId(int.Parse(clientSession[0]),
                                objSecuentialBl.GetNextSecuentialId(intNodeId, 94), "VH");
                            objEntidad.v_IdPlanillaVariablesDescuentos = newIdVariablesDescuentos;
                            objEntidad.t_InsertaFecha = DateTime.Now;
                            objEntidad.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                            objEntidad.i_Eliminado = 0;
                            objEntidad.v_IdPlanillaVariablesTrabajador = objEntity.v_IdPlanillaVariablesTrabajador;
                            dbContext.AddToplanillavariablesdescuentos(objEntidad);
                        }

                        #endregion

                        #region Insertar ingresos

                        foreach (
                            var objEntidad in pTempInsertarIngresos.Select(objEntidadDto => objEntidadDto.ToEntity()))
                        {
                            var newIdVariablesIngresos = Utils.GetNewId(int.Parse(clientSession[0]),
                                objSecuentialBl.GetNextSecuentialId(intNodeId, 95), "VI");
                            objEntidad.v_IdPlanillaVariablesIngresos = newIdVariablesIngresos;
                            objEntidad.t_InsertaFecha = DateTime.Now;
                            objEntidad.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                            objEntidad.i_Eliminado = 0;
                            objEntidad.v_IdPlanillaVariablesTrabajador = objEntity.v_IdPlanillaVariablesTrabajador;
                            dbContext.AddToplanillavariablesingresos(objEntidad);
                        }

                        #endregion

                        foreach (
                            var objEntidad in
                                pTempInsertarAportaciones.Select(objEntidadDto => objEntidadDto.ToEntity()))
                        {
                            var newIdVariablesIngresos = Utils.GetNewId(int.Parse(clientSession[0]),
                                objSecuentialBl.GetNextSecuentialId(intNodeId, 100), "CK");
                            objEntidad.v_IdPlanillaVariablesAportaciones = newIdVariablesIngresos;
                            objEntidad.t_InsertaFecha = DateTime.Now;
                            objEntidad.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                            objEntidad.i_Eliminado = 0;
                            objEntidad.v_IdPlanillaVariablesTrabajador = objEntity.v_IdPlanillaVariablesTrabajador;
                            dbContext.AddToplanillavariablesaportaciones(objEntidad);
                        }

                        #region Insertar Dias No Laborados Subsidiados

                        foreach (
                            var objEntidad in pTempInsertarDiasLab.Select(objEntidadDto => objEntidadDto.ToEntity()))
                        {
                            objEntidad.t_InsertaFecha = DateTime.Now;
                            objEntidad.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                            objEntidad.i_Eliminado = 0;
                            objEntidad.v_IdPlanillaVariablesTrabajador = objEntity.v_IdPlanillaVariablesTrabajador;
                            dbContext.planillavariablesdiasnolabsubsidiados.AddObject(objEntidad);
                        }

                        #endregion

                        dbContext.SaveChanges();
                        pobjOperationResult.Success = 1;
                        ts.Complete();
                        return newIdVariableTrabajador;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.InsertarVariablesTrabajador()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        /// <summary>
        /// Actualiza los datos de las variables del trabajador.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pobjEntityDto">Entidad de variable trabajador.</param>
        /// <param name="pTempInsertarHorasExtras"></param>
        /// <param name="pTempActualizarHorasExtras"></param>
        /// <param name="pTempEliminarHorasExtras"></param>
        /// <param name="pTempInsertarDescuentos"></param>
        /// <param name="pTempActualizarDescuentos"></param>
        /// <param name="pTempEliminarDescuentos"></param>
        /// <param name="pTempInsertarIngresos"></param>
        /// <param name="pTempActualizarIngresos"></param>
        /// <param name="pTempEliminarIngresos"></param>
        /// <param name="pTempEliminarDiasLab"></param>
        /// <param name="pTempEliminarAportaciones"></param>
        /// <param name="clientSession"></param>
        /// <param name="pTempInsertarDiasLab"></param>
        /// <param name="pTempActualizarDiasLab"></param>
        /// <param name="pTempInsertarAportaciones"></param>
        /// <param name="pTempActualizarAportaciones"></param>
        public static void ActualizarVariablesTrabajador(ref OperationResult pobjOperationResult,
            planillavariablestrabajadorDto pobjEntityDto,
            List<planillavariableshorasextrasDto> pTempInsertarHorasExtras,
            List<planillavariableshorasextrasDto> pTempActualizarHorasExtras,
            List<planillavariableshorasextrasDto> pTempEliminarHorasExtras,
            List<planillavariablesdescuentosDto> pTempInsertarDescuentos,
            List<planillavariablesdescuentosDto> pTempActualizarDescuentos,
            List<planillavariablesdescuentosDto> pTempEliminarDescuentos,
            List<planillavariablesingresosDto> pTempInsertarIngresos,
            List<planillavariablesingresosDto> pTempActualizarIngresos,
            List<planillavariablesingresosDto> pTempEliminarIngresos,
            List<planillavariablesdiasnolabsubsidiadosDto> pTempInsertarDiasLab,
            List<planillavariablesdiasnolabsubsidiadosDto> pTempActualizarDiasLab,
            List<planillavariablesdiasnolabsubsidiadosDto> pTempEliminarDiasLab,
            List<planillavariablesaportacionesDto> pTempInsertarAportaciones,
            List<planillavariablesaportacionesDto> pTempActualizarAportaciones,
            List<planillavariablesaportacionesDto> pTempEliminarAportaciones,
            List<string> clientSession)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var objSecuentialBl = new SecuentialBL();
                        var intNodeId = int.Parse(clientSession[0]);

                        #region Cabecera
                        var entitySource =
                                            dbContext.planillavariablestrabajador.FirstOrDefault(
                                                p => p.v_IdPlanillaVariablesTrabajador == pobjEntityDto.v_IdPlanillaVariablesTrabajador);
                        if (entitySource == null) throw new ArgumentNullException("entitySourceHeader");
                        var entityHeader = pobjEntityDto.ToEntity();
                        entityHeader.t_ActualizaFecha = DateTime.Now;
                        entityHeader.i_ActualizaIdUsuario = Int32.Parse(clientSession[2]);
                        dbContext.planillavariablestrabajador.ApplyCurrentValues(entityHeader);
                        #endregion

                        #region Horas Extras

                        #region Insertar horas extras

                        if (pTempInsertarHorasExtras != null)
                            foreach (var objEntidad in pTempInsertarHorasExtras.Select(objEntidadDto => objEntidadDto.ToEntity()))
                            {
                                objEntidad.t_InsertaFecha = DateTime.Now;
                                objEntidad.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                                objEntidad.i_Eliminado = 0;
                                objEntidad.v_IdPlanillaVariablesTrabajador = entityHeader.v_IdPlanillaVariablesTrabajador;
                                dbContext.AddToplanillavariableshorasextras(objEntidad);
                            }
                        #endregion

                        #region Actualiza horas extras
                        if (pTempActualizarHorasExtras != null)
                            foreach (var objEntidad in pTempActualizarHorasExtras.Select(objEntidadDto => objEntidadDto.ToEntity()))
                            {
                                var entidadSource =
                                    dbContext.planillavariableshorasextras.FirstOrDefault(p => p.i_Id == objEntidad.i_Id);
                                if (entidadSource == null) throw new ArgumentNullException("entidadSourceHorasExtras");
                                objEntidad.t_InsertaFecha = DateTime.Now;
                                objEntidad.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                                objEntidad.i_Eliminado = 0;
                                dbContext.planillavariableshorasextras.ApplyCurrentValues(objEntidad);
                            }
                        #endregion

                        #region Eliminar horas extras
                        if (pTempEliminarHorasExtras != null)
                            foreach (var objEntidad in pTempEliminarHorasExtras.Select(objEntidadDto => objEntidadDto.ToEntity()))
                            {
                                var entidadSource =
                                    dbContext.planillavariableshorasextras.FirstOrDefault(p => p.i_Id == objEntidad.i_Id);
                                if (entidadSource == null) throw new ArgumentNullException("entidadSourceHorasExtras");
                                objEntidad.t_InsertaFecha = DateTime.Now;
                                objEntidad.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                                objEntidad.i_Eliminado = 1;
                                dbContext.planillavariableshorasextras.ApplyCurrentValues(objEntidad);
                            }
                        #endregion

                        #endregion

                        #region Ingresos

                        #region Insertar horas extras
                        if (pTempInsertarIngresos != null)
                            foreach (var objEntidad in pTempInsertarIngresos.Select(objEntidadDto => objEntidadDto.ToEntity()))
                            {
                                var newIdVariablesIngresos = Utils.GetNewId(int.Parse(clientSession[0]), objSecuentialBl.GetNextSecuentialId(intNodeId, 95), "VI");
                                objEntidad.t_InsertaFecha = DateTime.Now;
                                objEntidad.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                                objEntidad.i_Eliminado = 0;
                                objEntidad.v_IdPlanillaVariablesIngresos = newIdVariablesIngresos;
                                objEntidad.v_IdPlanillaVariablesTrabajador = entityHeader.v_IdPlanillaVariablesTrabajador;
                                dbContext.AddToplanillavariablesingresos(objEntidad);
                            }
                        #endregion

                        #region Actualiza Ingresos
                        if (pTempActualizarIngresos != null)
                            foreach (var objEntidad in pTempActualizarIngresos.Select(objEntidadDto => objEntidadDto.ToEntity()))
                            {
                                var entidadSource =
                                    dbContext.planillavariablesingresos.FirstOrDefault(p => p.v_IdPlanillaVariablesIngresos == objEntidad.v_IdPlanillaVariablesIngresos);
                                if (entidadSource == null) throw new ArgumentNullException("entidadSourceIngresos");
                                objEntidad.t_InsertaFecha = DateTime.Now;
                                objEntidad.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                                objEntidad.i_Eliminado = 0;
                                dbContext.planillavariablesingresos.ApplyCurrentValues(objEntidad);
                            }
                        #endregion

                        #region Eliminar Ingresos
                        if (pTempEliminarIngresos != null)
                            foreach (var objEntidad in pTempEliminarIngresos.Select(objEntidadDto => objEntidadDto.ToEntity()))
                            {
                                var entidadSource =
                                    dbContext.planillavariablesingresos.FirstOrDefault(p => p.v_IdPlanillaVariablesIngresos == objEntidad.v_IdPlanillaVariablesIngresos);
                                if (entidadSource == null) throw new ArgumentNullException("entidadSourceIngresos");
                                objEntidad.t_InsertaFecha = DateTime.Now;
                                objEntidad.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                                objEntidad.i_Eliminado = 1;
                                dbContext.planillavariablesingresos.ApplyCurrentValues(objEntidad);
                            }
                        #endregion

                        #endregion

                        #region Descuentos

                        #region Insertar Descuentos
                        if (pTempInsertarDescuentos != null)
                            foreach (var objEntidad in pTempInsertarDescuentos.Select(objEntidadDto => objEntidadDto.ToEntity()))
                            {
                                var newIdVariablesDescuentos = Utils.GetNewId(int.Parse(clientSession[0]), objSecuentialBl.GetNextSecuentialId(intNodeId, 94), "VH");
                                objEntidad.t_InsertaFecha = DateTime.Now;
                                objEntidad.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                                objEntidad.i_Eliminado = 0;
                                objEntidad.v_IdPlanillaVariablesDescuentos = newIdVariablesDescuentos;
                                objEntidad.v_IdPlanillaVariablesTrabajador = entityHeader.v_IdPlanillaVariablesTrabajador;
                                dbContext.AddToplanillavariablesdescuentos(objEntidad);
                            }
                        #endregion

                        #region Actualiza Descuentos
                        if (pTempActualizarDescuentos != null)
                            foreach (var objEntidad in pTempActualizarDescuentos.Select(objEntidadDto => objEntidadDto.ToEntity()))
                            {
                                var entidadSource =
                                    dbContext.planillavariablesdescuentos.FirstOrDefault(p => p.v_IdPlanillaVariablesDescuentos == objEntidad.v_IdPlanillaVariablesDescuentos);
                                if (entidadSource == null) throw new ArgumentNullException("entidadSourceDescuentos");
                                objEntidad.t_InsertaFecha = DateTime.Now;
                                objEntidad.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                                objEntidad.i_Eliminado = 0;
                                dbContext.planillavariablesdescuentos.ApplyCurrentValues(objEntidad);
                            }
                        #endregion

                        #region Eliminar Descuentos
                        if (pTempEliminarDescuentos != null)
                            foreach (var objEntidad in pTempEliminarDescuentos.Select(objEntidadDto => objEntidadDto.ToEntity()))
                            {
                                var entidadSource =
                                    dbContext.planillavariablesdescuentos.FirstOrDefault(p => p.v_IdPlanillaVariablesDescuentos == objEntidad.v_IdPlanillaVariablesDescuentos);
                                if (entidadSource == null) throw new ArgumentNullException("entidadSourceDescuentos");
                                objEntidad.t_InsertaFecha = DateTime.Now;
                                objEntidad.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                                objEntidad.i_Eliminado = 1;
                                dbContext.planillavariablesdescuentos.ApplyCurrentValues(objEntidad);
                            }
                        #endregion

                        #endregion

                        #region Dias No Laborados SUbsidiados

                        #region Insertar horas extras
                        if (pTempInsertarDiasLab != null)
                            foreach (var objEntidad in pTempInsertarDiasLab.Select(objEntidadDto => objEntidadDto.ToEntity()))
                            {
                                objEntidad.t_InsertaFecha = DateTime.Now;
                                objEntidad.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                                objEntidad.i_Eliminado = 0;
                                objEntidad.v_IdPlanillaVariablesTrabajador = entityHeader.v_IdPlanillaVariablesTrabajador;
                                dbContext.AddToplanillavariablesdiasnolabsubsidiados(objEntidad);
                            }
                        #endregion

                        #region Actualiza horas extras
                        if (pTempActualizarDiasLab != null)
                            foreach (var objEntidad in pTempActualizarDiasLab.Select(objEntidadDto => objEntidadDto.ToEntity()))
                            {
                                var entidadSource =
                                    dbContext.planillavariablesdiasnolabsubsidiados.FirstOrDefault(p => p.i_Id == objEntidad.i_Id);
                                if (entidadSource == null) throw new ArgumentNullException("entidadSourceHorasExtras");
                                objEntidad.t_InsertaFecha = DateTime.Now;
                                objEntidad.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                                objEntidad.i_Eliminado = 0;
                                dbContext.planillavariablesdiasnolabsubsidiados.ApplyCurrentValues(objEntidad);
                            }
                        #endregion

                        #region Eliminar horas extras
                        if (pTempEliminarDiasLab != null)
                            foreach (var objEntidad in pTempEliminarDiasLab.Select(objEntidadDto => objEntidadDto.ToEntity()))
                            {
                                var entidadSource =
                                    dbContext.planillavariablesdiasnolabsubsidiados.FirstOrDefault(p => p.i_Id == objEntidad.i_Id);
                                if (entidadSource == null) throw new ArgumentNullException("entidadSourceHorasExtras");
                                objEntidad.t_InsertaFecha = DateTime.Now;
                                objEntidad.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                                objEntidad.i_Eliminado = 1;
                                dbContext.planillavariablesdiasnolabsubsidiados.ApplyCurrentValues(objEntidad);
                            }
                        #endregion

                        #endregion

                        #region Descuentos

                        #region Insertar Descuentos
                        if (pTempInsertarAportaciones != null)
                            foreach (var objEntidad in pTempInsertarAportaciones.Select(objEntidadDto => objEntidadDto.ToEntity()))
                            {
                                var newIdVariablesDescuentos = Utils.GetNewId(int.Parse(clientSession[0]), objSecuentialBl.GetNextSecuentialId(intNodeId, 100), "CK");
                                objEntidad.t_InsertaFecha = DateTime.Now;
                                objEntidad.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                                objEntidad.i_Eliminado = 0;
                                objEntidad.v_IdPlanillaVariablesAportaciones = newIdVariablesDescuentos;
                                objEntidad.v_IdPlanillaVariablesTrabajador = entityHeader.v_IdPlanillaVariablesTrabajador;
                                dbContext.AddToplanillavariablesaportaciones(objEntidad);
                            }
                        #endregion

                        #region Actualiza Descuentos
                        if (pTempActualizarAportaciones != null)
                            foreach (var objEntidad in pTempActualizarAportaciones.Select(objEntidadDto => objEntidadDto.ToEntity()))
                            {
                                var entidadSource =
                                    dbContext.planillavariablesaportaciones.FirstOrDefault(p => p.v_IdPlanillaVariablesAportaciones == objEntidad.v_IdPlanillaVariablesAportaciones);
                                if (entidadSource == null) throw new ArgumentNullException("entidadSourceDescuentos");
                                objEntidad.t_InsertaFecha = DateTime.Now;
                                objEntidad.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                                objEntidad.i_Eliminado = 0;
                                dbContext.planillavariablesaportaciones.ApplyCurrentValues(objEntidad);
                            }
                        #endregion

                        #region Eliminar Descuentos
                        if (pTempEliminarAportaciones != null)
                            foreach (var objEntidad in pTempEliminarAportaciones.Select(objEntidadDto => objEntidadDto.ToEntity()))
                            {
                                var entidadSource =
                                    dbContext.planillavariablesaportaciones.FirstOrDefault(p => p.v_IdPlanillaVariablesAportaciones == objEntidad.v_IdPlanillaVariablesAportaciones);
                                if (entidadSource == null) throw new ArgumentNullException("entidadSourceDescuentos");
                                objEntidad.t_InsertaFecha = DateTime.Now;
                                objEntidad.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                                objEntidad.i_Eliminado = 1;
                                dbContext.planillavariablesaportaciones.ApplyCurrentValues(objEntidad);
                            }
                        #endregion

                        #endregion

                        dbContext.SaveChanges();
                        ts.Complete();
                        pobjOperationResult.Success = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AplicacionBL.ActualizarVariablesTrabajador()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        /// <summary>
        /// Elimina lógicamente las variables del trabajador y en cascada sus relaciones.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstrIdVariableTrabajador"></param>
        /// <param name="clientSession"></param>
        public static void EliminarVariablesTrabajador(ref OperationResult pobjOperationResult,
            string pstrIdVariableTrabajador, List<string> clientSession)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var variableTrabajador =
                            dbContext.planillavariablestrabajador.FirstOrDefault(
                                p => p.v_IdPlanillaVariablesTrabajador == pstrIdVariableTrabajador);

                        if (variableTrabajador == null) throw new ArgumentNullException("entitySourceHeader");
                        variableTrabajador.i_Eliminado = 1;
                        variableTrabajador.t_ActualizaFecha = DateTime.Now;
                        variableTrabajador.i_ActualizaIdUsuario = Int32.Parse(clientSession[2]);
                        dbContext.planillavariablestrabajador.ApplyCurrentValues(variableTrabajador);

                        #region planillavariablesingresos
                        variableTrabajador.planillavariablesingresos.Where(o => o.i_Eliminado == 0).ToList().ForEach(
                                           p =>
                                           {
                                               p.i_Eliminado = 1;
                                               p.t_ActualizaFecha = DateTime.Now;
                                               p.i_ActualizaIdUsuario = Int32.Parse(clientSession[2]);
                                               dbContext.planillavariablesingresos.ApplyCurrentValues(p);
                                           });
                        #endregion

                        #region planillavariablesdescuentos
                        variableTrabajador.planillavariablesdescuentos.Where(o => o.i_Eliminado == 0).ToList().ForEach(
                                           p =>
                                           {
                                               p.i_Eliminado = 1;
                                               p.t_ActualizaFecha = DateTime.Now;
                                               p.i_ActualizaIdUsuario = Int32.Parse(clientSession[2]);
                                               dbContext.planillavariablesdescuentos.ApplyCurrentValues(p);
                                           });
                        #endregion

                        #region planillavariablesdiasnolabsubsidiados
                        variableTrabajador.planillavariablesdiasnolabsubsidiados.Where(o => o.i_Eliminado == 0).ToList().ForEach(
                            p =>
                            {
                                p.i_Eliminado = 1;
                                p.t_ActualizaFecha = DateTime.Now;
                                p.i_ActualizaIdUsuario = Int32.Parse(clientSession[2]);
                                dbContext.planillavariablesdiasnolabsubsidiados.ApplyCurrentValues(p);
                            });
                        #endregion

                        #region planillavariableshorasextras
                        variableTrabajador.planillavariableshorasextras.Where(o => o.i_Eliminado == 0).ToList().ForEach(
                                            p =>
                                            {
                                                p.i_Eliminado = 1;
                                                p.t_ActualizaFecha = DateTime.Now;
                                                p.i_ActualizaIdUsuario = Int32.Parse(clientSession[2]);
                                                dbContext.planillavariableshorasextras.ApplyCurrentValues(p);
                                            });
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
                pobjOperationResult.AdditionalInformation = "AplicacionBL.EliminarVariablesTrabajador()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        /// <summary>
        /// Actualiza la hoja de vida del trabajador en la base de datos.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="strIdTrabajador"></param>
        /// <param name="objArchivo"></param>
        public static void ActualizarHojaVida(ref OperationResult pobjOperationResult, string strIdTrabajador,
            byte[] objArchivo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entidad = dbContext.trabajador.FirstOrDefault(p => p.v_IdTrabajador.Equals(strIdTrabajador));
                    if (entidad == null)
                        throw new ArgumentNullException("El trabajador ya no existe!");

                    entidad.b_HojaVida = objArchivo;
                    entidad.i_ActualizaIdUsuario = Globals.ClientSession.i_SystemUserId;
                    entidad.t_ActualizaFecha = DateTime.Now;
                    dbContext.trabajador.ApplyCurrentValues(entidad);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaConceptos.InsertarHojaVida()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        #endregion
    }

    public static class PlanillaConceptosBl
    {
        public static IEnumerable<planillaconceptosadministracionDto> ObtenerConceptodAdministracion(
            ref OperationResult pobjOperationResult)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var consulta = (from n in dbContext.planillaconceptosadministracion

                                    join J1 in dbContext.planillaconceptos on n.v_IdConceptoPlanilla equals J1.v_IdConceptoPlanilla
                                        into J1_join
                                    from J1 in J1_join.DefaultIfEmpty()

                                    join J2 in dbContext.datahierarchy on new { grupo = 145, id = n.i_IdGrupo.Value }
                                        equals new { grupo = J2.i_GroupId, id = J2.i_ItemId } into J2_join
                                    from J2 in J2_join.DefaultIfEmpty()

                                    select new planillaconceptosadministracionDto
                                    {
                                        i_Eliminado = n.i_Eliminado,
                                        t_InsertaFecha = n.t_InsertaFecha,
                                        t_ActualizaFecha = n.t_ActualizaFecha,
                                        i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                        i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                        v_IdConceptoPlanilla = n.v_IdConceptoPlanilla,
                                        i_IdGrupo = n.i_IdGrupo,
                                        ConceptoCodigo = J1.v_Codigo,
                                        ConceptoDescripcion = J1.v_Nombre,
                                        ConceptoGrupo = J2.v_Value1,
                                        i_IdColumnaEquivalente = n.i_IdColumnaEquivalente
                                    }).ToList();

                    pobjOperationResult.Success = 1;
                    return consulta;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaConceptos.ObtenerConceptodAdministracion()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public static void InsertarConceptoAdministracion(ref OperationResult pobjOperationResult,
            planillaconceptosadministracionDto objEntityDto, List<string> clientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entity = objEntityDto.ToEntity();
                    entity.t_InsertaFecha = DateTime.Now;
                    entity.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                    dbContext.AddToplanillaconceptosadministracion(entity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaConceptos.InsertarConceptoAdministracion()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static planillaconceptosDto ObtieneConceptoPorId(string id)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var c = dbContext.planillaconceptos.FirstOrDefault(p => p.v_IdConceptoPlanilla.Equals(id));
                    if (c != null)
                        return c.ToDTO();
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

    }

    /// <summary>
    /// EQC Nov - 2015 Lógica del Negocio para la elaboración del cálculo de planilla y para la impresión de boletas.
    /// </summary>
    public static class PlanillaCalculoBl
    {
        public static Boolean EsPlanillaCerrada(ref OperationResult pobjOperationResult, int pintIdPlanilla)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var consulta = dbContext.planillanumeracion.FirstOrDefault(p => p.i_Id == pintIdPlanilla);
                    pobjOperationResult.Success = 1;

                    if (consulta != null)
                    {
                        return consulta.i_PlanillaCerrada == 0;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaCalculo.EsPlanillaCerrada()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return false;
            }
        }

        /// <summary>
        /// EQC Nov-2015
        /// Obtiene el monto de los Ingresos, Egresos y Aportaciones ingresadas en Variables de Trabajador realizando cruce de información con otras tablas de planillas.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pintIdPlanilla"></param>
        /// <returns></returns>
        public static IEnumerable<planillacalculoDto> CalcularPlanilla(ref OperationResult pobjOperationResult, int pintIdPlanilla)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var resultado = new List<planillacalculoDto>();
                    var entityPlanilla = dbContext.planillanumeracion.FirstOrDefault(p => p.i_Id == pintIdPlanilla && p.i_Eliminado == 0);
                    if (entityPlanilla == null) throw new ArgumentNullException("Planilla no existe!");
                    if (entityPlanilla.d_SueldoMinimo == null) throw new ArgumentNullException("El Sueldo Base de la Planilla creada no es válido.!");
                    var periodo = entityPlanilla.v_Periodo;
                    var mes = entityPlanilla.v_Mes;

                    var conceptosGenerales = dbContext.planillaconceptos.Where(p => p.i_Eliminado == 0).ToList();

                    #region Recopila las Afectaciones
                    //var afectosAfp = dbContext.planillaafecafp.Where(p => p.i_Afecto.Value == 1 && p.v_Periodo == periodo && p.v_Mes == mes && p.i_Eliminado == 0).ToList().Select(o => o.v_IdConceptoPlanilla);
                    var afectosAfp =
                        dbContext.planillaafectacionesgenerales.Where(
                            p => p.i_AFP_Afecto == 1 && p.i_Eliminado == 0 && p.v_Mes == mes && p.v_Periodo == periodo)
                            .Select(o => o.v_IdConceptoPlanilla);

                    var afectosOnp = dbContext.planillaafectacionesgenerales.Where(
                            p => p.i_Leyes_Trab_ONP == 1 && p.i_Eliminado == 0 && p.v_Mes == mes && p.v_Periodo == periodo)
                            .Select(o => o.v_IdConceptoPlanilla);
                    //var afectosOnp = dbContext.planillaafecleyestrab.Where(p => p.i_SeguroPensiones == 1 && p.v_Periodo == periodo && p.v_Mes == mes && p.i_Eliminado == 0).ToList().Select(o => o.v_IdConceptoPlanilla);
                    //var afectosAportaciones = dbContext.planillaafecleyesemp.Where(p => p.v_Periodo == periodo && p.v_Mes == mes && p.i_Eliminado == 0).ToList();

                    var afectosAportaciones = dbContext.planillaafectacionesgenerales.Where(
                            p => (p.i_Leyes_Emp_Essalud == 1 || p.i_Leyes_Emp_SCTR == 1) && p.i_Eliminado == 0
                                && p.v_Mes == mes && p.v_Periodo == periodo).ToList();

                    if (!afectosAfp.Any()) throw new ArgumentNullException("No hay afectaciones de AFP para la fecha de la planilla seleccionada!");
                    if (!afectosOnp.Any()) throw new ArgumentNullException("No hay afectaciones de Onp para la fecha de la planilla seleccionada!");
                    if (!afectosAportaciones.Any()) throw new ArgumentNullException("No hay afectaciones de Essalud para la fecha de la planilla seleccionada!");
                    #endregion

                    #region Recopila las Variables de Trabajador
                    var variablesTrabajador = dbContext.planillavariablestrabajador.Where(p => p.planillanumeracion.i_Id == pintIdPlanilla && p.i_Pago == 1 && p.i_Eliminado == 0 && p.trabajador.i_Eliminado == 0).ToList();
                    if (!variablesTrabajador.Any()) throw new ArgumentNullException("No hay variables de trabajadores con pago activo que procesar!");
                    #endregion

                    #region Recorre cada trabajador para realizar el cálculo
                    foreach (var planillavariablestrabajador in variablesTrabajador)
                    {
                        var afpVigente = ObtenerAfpVigente(periodo, mes, planillavariablestrabajador.v_IdTrabajador);
                        if (afpVigente == null)
                            throw new ArgumentNullException("El trabajador " + planillavariablestrabajador.trabajador.v_CodInterno + " no contaba con un régimen pensionario registrado para la fecha de la planilla seleccionada!");

                        var afpId = afpVigente.i_IdRegimenPensionario.Value;
                        var afp = dbContext.datahierarchy.FirstOrDefault(p => p.i_GroupId == 125 && p.i_ItemId == afpId);
                        var tieneDiasSubsidiados = planillavariablestrabajador.planillavariablesdiasnolabsubsidiados.Any(p => p.i_IdTipoConcepto == 2 && p.i_Eliminado == 0);

                        #region Recopila los Porcentajes
                        var porcentajeAfp = dbContext.planillaporcafp.FirstOrDefault(p => p.v_Periodo == periodo && p.v_Mes == mes && p.i_Eliminado == 0 && p.i_IdAfp == afpId).ToDTO();
                        var porcentajeOnp = dbContext.planillaporcleyestrabajador.FirstOrDefault(p => p.v_Periodo == periodo && p.v_Mes == mes && p.i_Eliminado == 0).ToDTO();
                        var porcentajeAportaciones = dbContext.planillaporcleyesempleador.FirstOrDefault(p => p.v_Periodo == periodo && p.v_Mes == mes && p.i_Eliminado == 0).ToDTO();

                        if (porcentajeAfp == null) throw new ArgumentNullException("No hay porcentajes de AFP para la fecha de la planilla seleccionada! [" + afp.v_Value1 + "]");
                        if (porcentajeOnp == null) throw new ArgumentNullException("No hay porcentajes de Onp para la fecha de la planilla seleccionada!");
                        if (porcentajeAportaciones == null) throw new ArgumentNullException("No hay porcentajes de Essalud para la fecha de la planilla seleccionada!");
                        #endregion

                        #region Ingresos
                        var Ingresos = dbContext.planillavariablesingresos.Where(p =>
                                                        p.i_Eliminado == 0 &&
                                                        p.v_IdPlanillaVariablesTrabajador == planillavariablestrabajador.v_IdPlanillaVariablesTrabajador).ToList()
                                                        .Select(n => new planillacalculoDto
                                                        {
                                                            v_IdTrabajador = n.planillavariablestrabajador.v_IdTrabajador,
                                                            i_IdPlanillaNumeracion = n.planillavariablestrabajador.i_IdPlanillaNumeracion,
                                                            v_IdConceptoPlanilla = n.v_IdConceptoPlanilla,
                                                            d_Importe = n.d_Importe,
                                                            i_IdAfp = afpId,
                                                            v_IdContrato = n.planillavariablestrabajador.v_IdContrato,
                                                            Tipo = "I"
                                                        });

                        //Se elimina los detalles cuyo regimen pensionario no cumpla con la fecha de la planilla.
                        Ingresos = Ingresos.Where(p => p.i_IdAfp != -1).ToList();
                        resultado = resultado.Concat(Ingresos).ToList();
                        #endregion

                        #region Egresos
                        var Egresos = dbContext.planillavariablesdescuentos.Where(p =>
                                                       p.i_Eliminado == 0 &&
                                                       p.v_IdPlanillaVariablesTrabajador == planillavariablestrabajador.v_IdPlanillaVariablesTrabajador).ToList()
                                                       .Select(n => new planillacalculoDto
                                                       {
                                                           v_IdTrabajador = n.planillavariablestrabajador.v_IdTrabajador,
                                                           i_IdPlanillaNumeracion = n.planillavariablestrabajador.i_IdPlanillaNumeracion,
                                                           v_IdConceptoPlanilla = n.v_IdConceptoPlanilla,
                                                           d_Importe = n.d_Importe,
                                                           i_IdAfp = afpId,
                                                           v_IdContrato = n.planillavariablestrabajador.v_IdContrato,
                                                           Tipo = "D"
                                                       });

                        //Se elimina los detalles cuyo regimen pensionario no cumpla con la fecha de la planilla.
                        Egresos = Egresos.Where(p => p.i_IdAfp != -1).ToList();
                        resultado = resultado.Concat(Egresos).ToList();
                        #endregion

                        if (!Ingresos.Concat(Egresos).Any()) continue;

                        #region Cálculo de la AFP u ONP
                        if (afp != null)
                        {
                            var conceptosAsociadosAfp = afp.v_Field != null && !string.IsNullOrEmpty(afp.v_Field.Trim()) ? afp.v_Field.Split(';').ToList() : null;

                            if (conceptosAsociadosAfp != null)
                            {
                                foreach (var conceptoasociado in conceptosAsociadosAfp)
                                {
                                    var valores = conceptoasociado.Split('|').ToList();
                                    if (valores.Count != 3)
                                        throw new ArgumentNullException("La fórmula ingresada para el Regimen Pensionario [" + afp.v_Value1 + "] es incorrecta");

                                    var concepto = conceptosGenerales.FirstOrDefault(p => p.v_Codigo.Trim() == valores[0].Trim());
                                    var tipoConcepto = valores[1];
                                    var columnaEquivalente = valores[2];
                                    var edadTrabajador = 0;
                                    if (tipoConcepto.ToUpper() != "AFP" && tipoConcepto.ToUpper() != "ONP")
                                        throw new ArgumentNullException("El tipo de concepto ingresado en la fórmula no es correcto: " + tipoConcepto);

                                    if (concepto != null)
                                    {
                                        decimal porcentaje, topePrima = 0;

                                        switch (tipoConcepto.Trim().ToUpper())
                                        {
                                            case "AFP":

                                                #region AFP

                                                #region Obtiene las columnas equivalentes en la tabla planillaporcafp para obtener los porcentajes
                                                //la columna llamada Comision es palabra reservada para saber si se trata de un concepto de comision 
                                                //ya que este concepto llama a diferentes columas en planillaporcafp deacuerdo al trabajador
                                                if (columnaEquivalente != "Comision")
                                                {
                                                    if (columnaEquivalente == "d_PorcentajePrima")
                                                    {
                                                        topePrima = (decimal)porcentajeAfp["d_PorcentajeTope"];
                                                        edadTrabajador = Utils.Windows.CalcularEdad(planillavariablestrabajador.trabajador.cliente.t_FechaNacimiento ?? DateTime.Now);
                                                    }
                                                    try
                                                    {
                                                        porcentaje = (decimal)porcentajeAfp[columnaEquivalente.Trim()];
                                                    }
                                                    catch (Exception)
                                                    {
                                                        pobjOperationResult.Success = 0;
                                                        pobjOperationResult.ErrorMessage = "La columna indicada en la fórmula no existe: " + columnaEquivalente;
                                                        pobjOperationResult.AdditionalInformation = "PlanillaCalculo.CalcularPlanilla()";
                                                        return null;
                                                    }
                                                }
                                                else
                                                {
                                                    var idModalidadRegimen = afpVigente.i_IdModalidadRegimen.Value;
                                                    var columnaEquivalenteModalidadEntity =
                                                        dbContext.datahierarchy.FirstOrDefault(
                                                            p =>
                                                                p.i_GroupId == 140 &&
                                                                p.i_ItemId == idModalidadRegimen &&
                                                                p.i_IsDeleted == 0);
                                                    if (columnaEquivalenteModalidadEntity == null) throw new Exception("No existe una modalidad de régimen especificada para el régimen a procesar del trabajador!.");
                                                    if (string.IsNullOrEmpty(columnaEquivalenteModalidadEntity.v_Field)) throw new Exception("No existe una columna especificada para la modalidad de régimen " + columnaEquivalenteModalidadEntity.v_Value1 + " (d_ComisionFlujo ó d_ComisionSaldo) para las modalidades (Grupo 140)");
                                                    var columnaEquivalenteModalidad = columnaEquivalenteModalidadEntity.v_Field;
                                                    try
                                                    {
                                                        porcentaje = (decimal)porcentajeAfp[columnaEquivalenteModalidad.Trim()];
                                                    }
                                                    catch (Exception)
                                                    {
                                                        pobjOperationResult.Success = 0;
                                                        pobjOperationResult.ErrorMessage = "La columna indicada en la fórmula no existe: " + columnaEquivalenteModalidad;
                                                        pobjOperationResult.AdditionalInformation = "PlanillaCalculo.CalcularPlanilla()";
                                                        return null;
                                                    }
                                                }
                                                #endregion

                                                string filterExpressionIngresosAfp = null;
                                                string filterExpressionEgresosAfp = null;

                                                Ingresos.ToList().ForEach(i =>
                                                {
                                                    if (afectosAfp.Contains(i.v_IdConceptoPlanilla))
                                                    {
                                                        filterExpressionIngresosAfp = !string.IsNullOrEmpty(filterExpressionIngresosAfp)
                                                            ? filterExpressionIngresosAfp + "|| v_IdConceptoPlanilla == \"" + i.v_IdConceptoPlanilla + "\""
                                                            : "v_IdConceptoPlanilla == \"" + i.v_IdConceptoPlanilla + "\"";
                                                    }
                                                });

                                                Egresos.ToList().ForEach(i =>
                                                {
                                                    if (afectosAfp.Contains(i.v_IdConceptoPlanilla))
                                                    {
                                                        filterExpressionEgresosAfp = !string.IsNullOrEmpty(filterExpressionEgresosAfp)
                                                            ? filterExpressionEgresosAfp + "|| v_IdConceptoPlanilla == \"" + i.v_IdConceptoPlanilla + "\""
                                                            : "v_IdConceptoPlanilla == \"" + i.v_IdConceptoPlanilla + "\"";
                                                    }
                                                });

                                                var montoIngresosAfectosAfp = !string.IsNullOrEmpty(filterExpressionIngresosAfp) ? Ingresos.AsQueryable().Where(filterExpressionIngresosAfp).Sum(o => o.d_Importe) : 0;
                                                var montoEgresosAfectosAfp = !string.IsNullOrEmpty(filterExpressionEgresosAfp) ? Egresos.AsQueryable().Where(filterExpressionEgresosAfp).Sum(o => o.d_Importe) : 0;
                                                var montoAfectoAfp = montoIngresosAfectosAfp - montoEgresosAfectosAfp;

                                                decimal importe;

                                                if (columnaEquivalente != "d_PorcentajePrima")
                                                    importe = Utils.Windows.DevuelveValorRedondeado(montoAfectoAfp.Value * (porcentaje / 100), 2);
                                                else
                                                {
                                                    importe = edadTrabajador < 65
                                                        ? Utils.Windows.DevuelveValorRedondeado(montoAfectoAfp.Value < topePrima ? montoAfectoAfp.Value * (porcentaje / 100) : topePrima * (porcentaje / 100), 2)
                                                        : 0;
                                                }

                                                var calculoAfp = new planillacalculoDto
                                                {
                                                    v_IdTrabajador = planillavariablestrabajador.v_IdTrabajador,
                                                    i_IdPlanillaNumeracion = planillavariablestrabajador.i_IdPlanillaNumeracion,
                                                    v_IdConceptoPlanilla = concepto.v_IdConceptoPlanilla,
                                                    Tipo = "D",
                                                    i_IdAfp = afpId,
                                                    v_IdContrato = planillavariablestrabajador.v_IdContrato,
                                                    d_Importe = Utils.Windows.DevuelveValorRedondeado(importe, 2)
                                                };
                                                resultado.Add(calculoAfp);
                                                #endregion

                                                break;

                                            case "ONP":

                                                #region ONP
                                                try
                                                {
                                                    porcentaje = (decimal)porcentajeOnp[columnaEquivalente.Trim()];
                                                }
                                                catch (Exception)
                                                {
                                                    pobjOperationResult.Success = 0;
                                                    pobjOperationResult.ErrorMessage = "La columna indicada en la fórmula no existe: " + columnaEquivalente;
                                                    pobjOperationResult.AdditionalInformation = "PlanillaCalculo.CalcularPlanilla()";
                                                    return null;
                                                }

                                                string filterExpressionIngresosOnp = null;
                                                string filterExpressionEgresosOnp = null;

                                                Ingresos.ToList().ForEach(i =>
                                                {
                                                    if (afectosOnp.Contains(i.v_IdConceptoPlanilla))
                                                    {
                                                        filterExpressionIngresosOnp = !string.IsNullOrEmpty(filterExpressionIngresosOnp)
                                                            ? filterExpressionIngresosOnp + "|| v_IdConceptoPlanilla == \"" + i.v_IdConceptoPlanilla + "\""
                                                            : "v_IdConceptoPlanilla == \"" + i.v_IdConceptoPlanilla + "\"";
                                                    }
                                                });

                                                Egresos.ToList().ForEach(i =>
                                                {
                                                    if (afectosOnp.Contains(i.v_IdConceptoPlanilla))
                                                    {
                                                        filterExpressionEgresosOnp = !string.IsNullOrEmpty(filterExpressionEgresosOnp)
                                                            ? filterExpressionEgresosOnp + "|| v_IdConceptoPlanilla == \"" + i.v_IdConceptoPlanilla + "\""
                                                            : "v_IdConceptoPlanilla == \"" + i.v_IdConceptoPlanilla + "\"";
                                                    }
                                                });

                                                var montoIngresosAfectosOnp = !string.IsNullOrEmpty(filterExpressionIngresosOnp) ? Ingresos.AsQueryable().Where(filterExpressionIngresosOnp).Sum(o => o.d_Importe) : 0;
                                                var montoEgresosAfectosOnp = !string.IsNullOrEmpty(filterExpressionEgresosOnp) ? Egresos.AsQueryable().Where(filterExpressionEgresosOnp).Sum(o => o.d_Importe) : 0;
                                                var montoAfectoOnp = (float)montoIngresosAfectosOnp - (float)montoEgresosAfectosOnp;
                                                //// si el monto final es menor al sueldo minimo, se toma el sueldo minimo en su lugar.
                                                //if (montoAfectoOnp < (float)entityPlanilla.d_SueldoMinimo)
                                                //    montoAfectoOnp = (float)entityPlanilla.d_SueldoMinimo;

                                                var calculoOnp = new planillacalculoDto
                                                {
                                                    v_IdTrabajador = planillavariablestrabajador.v_IdTrabajador,
                                                    i_IdPlanillaNumeracion = planillavariablestrabajador.i_IdPlanillaNumeracion,
                                                    v_IdConceptoPlanilla = concepto.v_IdConceptoPlanilla,
                                                    Tipo = "D",
                                                    i_IdAfp = afpId,
                                                    v_IdContrato = planillavariablestrabajador.v_IdContrato,
                                                    d_Importe = Utils.Windows.DevuelveValorRedondeado((decimal)montoAfectoOnp * (porcentaje / 100), 2)
                                                };
                                                resultado.Add(calculoOnp);
                                                #endregion

                                                break;
                                        }
                                    }
                                    else
                                    {
                                        throw new ArgumentNullException("Uno de los conceptos relacionados al cálculo de la afp no existe. (AFP: " + afp.v_Value1 + " CONCEPTO: " + conceptoasociado + ")");
                                    }
                                }
                            }
                        }
                        #endregion

                        #region Cálculo de Essalud y Otras Aportaciones
                        var aportaciones = dbContext.planillavariablesaportaciones.Where(p =>
                            p.i_Eliminado == 0 &&
                            p.v_IdPlanillaVariablesTrabajador ==
                            planillavariablestrabajador.v_IdPlanillaVariablesTrabajador).ToList();

                        foreach (var aportacion in aportaciones)
                        {
                            if (aportacion.planillavariablestrabajador.v_IdTrabajador == "N001-TT000000006")
                            {
                                var ward = ":v";
                            }
                            var calculo = new planillacalculoDto();
                            calculo.i_IdAfp = afpId;
                            calculo.i_IdPlanillaNumeracion = aportacion.planillavariablestrabajador.i_IdPlanillaNumeracion;
                            calculo.v_IdConceptoPlanilla = aportacion.v_IdConceptoPlanilla;
                            calculo.v_IdContrato = aportacion.planillavariablestrabajador.v_IdContrato;
                            calculo.v_IdTrabajador = aportacion.planillavariablestrabajador.v_IdTrabajador;
                            calculo.Tipo = "A";
                            calculo.d_Importe = ObtenerImporteAportacion(ref pobjOperationResult, afectosAportaciones,
                                Ingresos.ToList(), Egresos.ToList(), aportacion, porcentajeAportaciones, entityPlanilla.d_SueldoMinimo.Value, tieneDiasSubsidiados);

                            if (pobjOperationResult.Success == 0) return null;
                            resultado.Add(calculo);
                        }
                        #endregion
                    }
                    #endregion

                    #region Relaciona el resultado

                    var result = (from r in resultado

                                  join J1 in dbContext.trabajador on r.v_IdTrabajador equals J1.v_IdTrabajador into J1_join
                                  from J1 in J1_join.DefaultIfEmpty()

                                  join J2 in dbContext.planillaconceptos on r.v_IdConceptoPlanilla equals J2.v_IdConceptoPlanilla
                                      into J2_join
                                  from J2 in J2_join.DefaultIfEmpty()

                                  select new planillacalculoDto
                                  {
                                      v_IdTrabajador = r.v_IdTrabajador,
                                      i_IdPlanillaNumeracion = r.i_IdPlanillaNumeracion,
                                      v_IdConceptoPlanilla = r.v_IdConceptoPlanilla,
                                      v_IdContrato = r.v_IdContrato,
                                      d_Importe = r.d_Importe,
                                      i_Id = r.i_Id,
                                      i_IdAfp = r.i_IdAfp,
                                      NombreConcepto = J2 != null ? J2.v_Nombre : "*CONCEPTO NO ENCONTRADO*",
                                      Tipo = r.Tipo,
                                      NombreTrabajador = J1 != null ?
                                          (J1.cliente.v_ApePaterno + " " + J1.cliente.v_ApeMaterno + " " +
                                           J1.cliente.v_PrimerNombre + " " + J1.cliente.v_RazonSocial).Trim() :
                                           "*TRABAJADOR NO ENCONTRADO*"
                                  }).ToList();

                    #endregion

                    pobjOperationResult.Success = 1;
                    return result;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaCalculo.CalcularPlanilla()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        /// <summary>
        /// Obtiene el Seguro de pensiones (AFP u ONP) vigente por el rango de la fecha de inscripcion. 
        /// Si no existe un regimen de pensiones que cumpla con el criterio se retorna nulo.
        /// </summary>
        /// <param name="pstrPeriodo"></param>
        /// <param name="pstrMes"></param>
        /// <param name="pstrIdTrabajador"></param>
        /// <returns></returns>
        public static regimenpensionariotrabajador ObtenerAfpVigente(string pstrperiodo, string pstrMes, string pstrIdTrabajador)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var nroDias = DateTime.DaysInMonth(Int32.Parse(pstrperiodo), Int32.Parse(pstrMes));
                var fechaProceso = DateTime.Parse(nroDias + "/" + pstrMes + "/" + pstrperiodo);
                var trabajador = dbContext.trabajador.FirstOrDefault(p => p.v_IdTrabajador == pstrIdTrabajador);

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

        /// <summary>
        /// Obtiene el importe afecto por las aportaciones del trabajador incluidas en variables del trabajador.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pobjafectosAportaciones"></param>
        /// <param name="pobjIngresos"></param>
        /// <param name="pobjEgresos"></param>
        /// <param name="pobjAportacion"></param>
        /// <param name="pobjPorcentajeEmpleador"></param>
        /// <param name="sueldoMinimo"></param>
        /// <param name="tieneDiasSubsidiados"></param>
        /// <returns></returns>
        private static decimal ObtenerImporteAportacion(
            ref OperationResult pobjOperationResult,
            List<planillaafectacionesgenerales> pobjafectosAportaciones,
            List<planillacalculoDto> pobjIngresos,
            List<planillacalculoDto> pobjEgresos,
            planillavariablesaportaciones pobjAportacion,
            planillaporcleyesempleadorDto pobjPorcentajeEmpleador, decimal sueldoMinimo, bool tieneDiasSubsidiados)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    List<string> conceptosAfectos;
                    var aportacionId = pobjAportacion.v_IdConceptoPlanilla;
                    var aportaconiEntity = dbContext.planillaconceptos.FirstOrDefault(p => p.v_IdConceptoPlanilla == aportacionId);

                    if (aportaconiEntity != null)
                    {
                        decimal porcentajeAfectacion;
                        var columnaAfectacion = aportaconiEntity.v_ColumnaAfectaciones;

                        #region Obtiene el Porcentaje de Afectación del Concepto Aportacion

                        try
                        {
                            if (string.IsNullOrEmpty(aportaconiEntity.v_ColumnaPorcentaje))
                                throw new ArgumentNullException(
                                    "La columna de porcentaje ingresada en la fórmula de Afectaciones de " +
                                    aportaconiEntity.v_Nombre + " está vacío");

                            porcentajeAfectacion =
                                (decimal)pobjPorcentajeEmpleador[aportaconiEntity.v_ColumnaPorcentaje];
                        }
                        catch (Exception)
                        {
                            pobjOperationResult.Success = 0;
                            throw new ArgumentNullException(
                                "La columna de porcentaje ingresada en la formula de Afectaciones de " +
                                aportaconiEntity.v_Nombre + " no existe: " + aportaconiEntity.v_ColumnaPorcentaje);
                        }

                        #endregion

                        #region Obtiene una Lista de los Ids de los Conceptos afectos a la Aportacion

                        try
                        {
                            if (string.IsNullOrEmpty(columnaAfectacion))
                                throw new ArgumentNullException(
                                    "La columna de afectación ingresada en la fórmula de Afectaciones de " +
                                    aportaconiEntity.v_Nombre + " está vacío");

                            conceptosAfectos =
                                pobjafectosAportaciones.AsQueryable()
                                    .Where(columnaAfectacion + "== 1")
                                    .ToList()
                                    .Select(p => p.v_IdConceptoPlanilla)
                                    .ToList();
                        }
                        catch (Exception)
                        {
                            pobjOperationResult.Success = 0;
                            throw new ArgumentNullException(
                                "La columna de afectación ingresada en la formula de Afectaciones de " +
                                aportaconiEntity.v_Nombre + " no existe: " + columnaAfectacion);
                        }
                        #endregion

                        string filterExpressionIngresos = null;
                        string filterExpressionEgresos = null;

                        #region Filtra de los aportes de variables del trabajador cuales son los afectos
                        pobjIngresos.ToList().ForEach(i =>
                                        {
                                            if (conceptosAfectos.Contains(i.v_IdConceptoPlanilla))
                                            {
                                                filterExpressionIngresos = !string.IsNullOrEmpty(filterExpressionIngresos)
                                                    ? filterExpressionIngresos + "|| v_IdConceptoPlanilla == \"" + i.v_IdConceptoPlanilla + "\""
                                                    : "v_IdConceptoPlanilla == \"" + i.v_IdConceptoPlanilla + "\"";
                                            }
                                        });

                        pobjEgresos.ToList().ForEach(i =>
                        {
                            if (conceptosAfectos.Contains(i.v_IdConceptoPlanilla))
                            {
                                filterExpressionEgresos = !string.IsNullOrEmpty(filterExpressionEgresos)
                                    ? filterExpressionEgresos + "|| v_IdConceptoPlanilla == \"" + i.v_IdConceptoPlanilla + "\""
                                    : "v_IdConceptoPlanilla == \"" + i.v_IdConceptoPlanilla + "\"";
                            }
                        });
                        #endregion

                        //se aplica el filtro de conceptos afectos a los ingresos y delos egresos y se obtiene la diferencia de ambos
                        var montoIngresosAfectos = !string.IsNullOrEmpty(filterExpressionIngresos) ?
                            pobjIngresos.AsQueryable().Where(filterExpressionIngresos).Sum(o => o.d_Importe) : 0;
                        var montoEgresosAfectos = !string.IsNullOrEmpty(filterExpressionEgresos) ?
                            pobjEgresos.AsQueryable().Where(filterExpressionEgresos).Sum(o => o.d_Importe) : 0;
                        var montoAfecto = montoIngresosAfectos - montoEgresosAfectos;

                        // si el monto final es menor al sueldo minimo, se toma el sueldo minimo en su lugar.
                        if (!tieneDiasSubsidiados && montoAfecto < sueldoMinimo) montoAfecto = sueldoMinimo;
                        pobjOperationResult.Success = 1;

                        //a la diferencia se le aplica el porcentaje obtenido anteriormente
                        return montoAfecto.HasValue && montoAfecto.Value > 0 ?
                            Utils.Windows.DevuelveValorRedondeado(montoAfecto.Value * (porcentajeAfectacion / 100), 2) : 0;
                    }

                    pobjOperationResult.Success = 1;
                    return 0;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaCalculo.ObtenerImporteAportacion()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return 0;
            }


        }

        /// <summary>
        /// Consulta para el reporte de boletas de las planillas guardadas.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pintIdPlanilla"></param>
        /// <param name="pstrIdTrabajador"></param>
        /// <returns></returns>
        public static List<ReportePlanillaBoleta> ObtenerCalculoPlanillaReporte(ref OperationResult pobjOperationResult,
           int pintIdPlanilla, string pstrIdTrabajador, bool duplicarBoletas)
        {
            try
            {
                var resultado = new List<ReportePlanillaBoleta>();
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var consulta = dbContext.planillacalculo.Where(p => p.i_IdPlanillaNumeracion == pintIdPlanilla);
                    if (consulta.Any())
                    {
                        #region Consulta principal

                        var HorasExtras = dbContext.planillavariableshorasextras.Where(o => o.i_Eliminado == 0).ToList();
                        var Empresa = dbContext.configuracionempresa.FirstOrDefault();
                        var result = (from r in dbContext.planillacalculo

                                      join J1 in dbContext.trabajador on r.v_IdTrabajador equals J1.v_IdTrabajador into J1_join
                                      from J1 in J1_join.DefaultIfEmpty()

                                      join J2 in dbContext.planillaconceptos on new { id = r.v_IdConceptoPlanilla, eliminado = 0 }
                                                                                equals new { id = J2.v_IdConceptoPlanilla, eliminado = J2.i_Eliminado.Value } into J2_join
                                      from J2 in J2_join.DefaultIfEmpty()

                                      join J3 in dbContext.areaslaboratrabajador on new { vigente = 1, idtrabajador = J1.v_IdTrabajador, eliminado = 0 }
                                                                                    equals new { vigente = J3.i_AreaVigente.Value, idtrabajador = J3.v_IdTrabajador, eliminado = J3.i_Eliminado.Value } into J3_join
                                      from J3 in J3_join.DefaultIfEmpty()

                                      join J4 in dbContext.contratotrabajador on new { id = r.v_IdTrabajador, eliminado = 0, vigente = 1 }
                                                                             equals new { id = J4.v_IdTrabajador, eliminado = J4.i_Eliminado.Value, vigente = J4.i_ContratoVigente.Value } into J4_join
                                      from J4 in J4_join.DefaultIfEmpty()

                                      join J4_1 in dbContext.datahierarchy on new { grupo = 136, id = J4.i_IdTipoContrato.Value }
                                                                            equals new { grupo = J4_1.i_GroupId, id = J4_1.i_ItemId } into J4_1_join
                                      from J4_1 in J4_1_join.DefaultIfEmpty()

                                      join J5 in dbContext.planillavariablestrabajador on new { idPlanilla = r.i_IdPlanillaNumeracion.Value, idtrabajador = r.v_IdTrabajador, eliminado = 0 }
                                                                                        equals new { idPlanilla = J5.i_IdPlanillaNumeracion, idtrabajador = J5.v_IdTrabajador, eliminado = J5.i_Eliminado.Value } into J5_join
                                      from J5 in J5_join.DefaultIfEmpty()

                                      join J6 in dbContext.regimenpensionariotrabajador on new { id = r.i_IdAfp, eliminado = 0, idtrabajador = r.v_IdTrabajador, vigente = 1 }
                                                                                        equals new { id = J6.i_IdRegimenPensionario, eliminado = J6.i_Eliminado.Value, idtrabajador = J6.v_IdTrabajador, vigente = J6.i_RegimenVigente.Value } into J6_join
                                      from J6 in J6_join.DefaultIfEmpty()

                                      join J7 in dbContext.datahierarchy on new { grupo = 125, id = r.i_IdAfp.Value }
                                                                        equals new { grupo = J7.i_GroupId, id = J7.i_ItemId } into J7_join
                                      from J7 in J7_join.DefaultIfEmpty()
                                      where r.i_IdPlanillaNumeracion == pintIdPlanilla

                                      select new
                                      {
                                          NombreTrabajador =
                                              (J1.cliente.v_ApePaterno + " " + J1.cliente.v_ApeMaterno + " " +
                                               J1.cliente.v_PrimerNombre + " " + J1.cliente.v_SegundoNombre).Trim(),
                                          r.Tipo,
                                          NroDocumento = J1.cliente.v_NroDocIdentificacion,
                                          AutogeneradoEsSalud = J1.v_NroEsSalud,
                                          CargoTrabajador = J3 != null ? J3.v_Cargo : string.Empty,
                                          CodigoTrabajador = J1.v_CodInterno,
                                          CondicionLaboral = J4_1 != null ? J4_1.v_Value1 : string.Empty,
                                          DiasInasistencia = J5.i_DiasNoLaborados.Value,
                                          DiasLaborados = J5.i_DiasLaboradosBP.Value,
                                          FechaIngreso = J1.t_FechaAlta,
                                          HorasExtrasTrabajadas = 0,
                                          HorasTrabajadas = J5.d_HorasLaboradosBP.Value,
                                          Id = r.i_Id,
                                          Mes = r.planillanumeracion.v_Mes,
                                          Tardanza = J5 != null ? J5.d_TiempoTardanza ?? 0 : 0,
                                          FechaInicioVacaciones = J5.t_FechaVacacionesInicio,
                                          FechaFinVacaciones = J5.t_FechaVacacionesFin,
                                          NombreConceptoIngreso = r.Tipo == "I" ? J2.v_Nombre : "",
                                          ImporteIngreso = r.Tipo == "I" ? r.d_Importe : 0,
                                          NombreConceptoDescuentos = r.Tipo == "D" ? J2.v_Nombre : "",
                                          ImporteDescuentos = r.Tipo == "D" ? r.d_Importe : 0,
                                          NombreConceptoAportaciones = r.Tipo == "A" ? J2.v_Nombre : "",
                                          ImporteAportaciones = r.Tipo == "A" ? r.d_Importe : 0,
                                          IdTrabajador = J1.v_IdTrabajador,
                                          RegimenPensionario = J7 != null ? J7.v_Value1 : "",
                                          CarnetAfp = J6 != null ? J6.v_NroCussp : "",
                                          InicioContrato = r.planillanumeracion.t_FechaInicio.Value,
                                          FinContrato = r.planillanumeracion.t_FechaTermino.Value,
                                          Anio = r.planillanumeracion.v_Periodo,
                                          i_TieneVacaciones = J5.i_TieneVacaciones ?? 0,
                                          v_IdPlanillaVariablesTrabajador = J5.v_IdPlanillaVariablesTrabajador,
                                          FechaCese = J1.t_FechaCese,
                                      }).ToList()
                                      .Select(p =>
                                          {
                                              var he = HorasExtras.Where(q => q.v_IdPlanillaVariablesTrabajador == p.v_IdPlanillaVariablesTrabajador).ToList();
                                              return new ReportePlanillaBoleta
                                                {
                                                    Tipo = p.Tipo,
                                                    ImporteIngreso = p.ImporteIngreso,
                                                    NombreTrabajador = p.NombreTrabajador,
                                                    IdTrabajador = p.IdTrabajador,
                                                    NroDocumento = p.NroDocumento,
                                                    ImporteAportaciones = p.ImporteAportaciones,
                                                    Id = p.Id,
                                                    ImporteDescuentos = p.ImporteDescuentos,
                                                    NombreConceptoAportaciones = p.NombreConceptoAportaciones,
                                                    NombreConceptoIngreso = p.NombreConceptoIngreso,
                                                    NombreConceptoDescuentos = p.NombreConceptoDescuentos,
                                                    AutogeneradoEsSalud = p.AutogeneradoEsSalud,
                                                    CargoTrabajador = p.CargoTrabajador,
                                                    CodigoTrabajador = p.CodigoTrabajador,
                                                    CondicionLaboral = p.CondicionLaboral,
                                                    DiasInasistencia = p.DiasInasistencia,
                                                    DiasLaborados = p.DiasLaborados,
                                                    FechaIngreso = p.FechaIngreso.Value.Date,
                                                    // HorasExtrasTrabajadas = p.HorasExtrasTrabajadas,
                                                    HorasExtrasTrabajadas = he != null ? he.Sum(o => o.d_HorasExtras).Value : 0,
                                                    HE = he != null ? he.Sum(o => o.d_HorasExtras).Value : 0,
                                                    HorasTrabajadas = p.HorasTrabajadas,
                                                    Tardanza = p.Tardanza,
                                                    Vacaciones = p.i_TieneVacaciones == 1 ? "DEL " + p.FechaInicioVacaciones.Value.ToShortDateString() + " AL  " + p.FechaFinVacaciones.Value.ToShortDateString() : "",
                                                    Mes = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(int.Parse(p.Mes)).ToUpper() + " - " + p.Anio.ToString(),
                                                    Periodo = p.InicioContrato.ToShortDateString() + " Al " + p.FinContrato.ToShortDateString(),
                                                    RegimenPensionario = p.RegimenPensionario,
                                                    CarnetAfp = p.CarnetAfp,
                                                    FechaCese = p.FechaCese == null || p.FechaCese.Value.ToString() == "" ? "" : p.FechaCese.Value.ToShortDateString(),
                                                    LogoEmpresa = Empresa == null ? null : Empresa.b_LogoEmpresa,
                                                    FirmaEmpleador= Empresa == null ? null : Empresa.b_FirmaDigitalEmpresa,
                                                };
                                          });

                        if (!result.Any()) throw new ArgumentNullException("La planilla seleccionada no fue procesada y guardada o ya no existe.!");
                        if (!string.IsNullOrEmpty(pstrIdTrabajador))
                            result = result.Where(p => p.IdTrabajador == pstrIdTrabajador).ToList();
                        #endregion

                        #region Lógica adicional para dar formato al detalle de la boleta
                        var grupo = result.Select(o => o.NombreTrabajador).ToList().Distinct();

                        foreach (var trabajador in grupo)
                        {
                            //separa los ingresos, descuentos y aportaciones.
                            var G_I = result.Where(p => p.Tipo == "I" && p.NombreTrabajador == trabajador).ToList();
                            var G_D = result.Where(p => p.Tipo == "D" && p.NombreTrabajador == trabajador).ToList();
                            var G_A = result.Where(p => p.Tipo == "A" && p.NombreTrabajador == trabajador).ToList();

                            // saca el grupo que contiene más elementos
                            var max = new[] { G_I.Count, G_D.Count, G_A.Count }.Max();
                            var maxDto = G_I.Count == max ? G_I : G_D.Count == max ? G_D : G_A;

                            #region Comprime las colecciones
                            for (int i = 0; i < max; i++)
                            {
                                switch (maxDto.FirstOrDefault().Tipo)
                                {
                                    case "I":
                                        var objdto = new ReportePlanillaBoleta();
                                        var gi = G_I.FirstOrDefault();
                                        var gd = G_D.FirstOrDefault();
                                        var ga = G_A.FirstOrDefault();
                                        if (gi == null) continue;

                                        objdto = (ReportePlanillaBoleta)gi.Clone();
                                        G_I.Remove(gi);

                                        if (gd != null)
                                        {
                                            objdto.NombreConceptoDescuentos = gd.NombreConceptoDescuentos;
                                            objdto.ImporteDescuentos = gd.ImporteDescuentos;
                                            G_D.Remove(gd);
                                        }

                                        if (ga != null)
                                        {
                                            objdto.NombreConceptoAportaciones = ga.NombreConceptoAportaciones;
                                            objdto.ImporteAportaciones = ga.ImporteAportaciones;
                                            G_A.Remove(ga);
                                        }

                                        objdto.Tipo = "X";
                                        resultado.Add(objdto);
                                        break;

                                    case "D":
                                        var _objdto = new ReportePlanillaBoleta();
                                        var _gi = G_I.FirstOrDefault();
                                        var _gd = G_D.FirstOrDefault();
                                        var _ga = G_A.FirstOrDefault();
                                        if (_gd == null) continue;

                                        _objdto = (ReportePlanillaBoleta)_gd.Clone();
                                        G_D.Remove(_gd);

                                        if (_gi != null)
                                        {
                                            _objdto.NombreConceptoIngreso = _gi.NombreConceptoIngreso;
                                            _objdto.ImporteIngreso = _gi.ImporteIngreso;
                                            G_I.Remove(_gi);
                                        }

                                        if (_ga != null)
                                        {
                                            _objdto.NombreConceptoAportaciones = _ga.NombreConceptoAportaciones;
                                            _objdto.ImporteAportaciones = _ga.ImporteAportaciones;
                                            G_A.Remove(_ga);
                                        }

                                        _objdto.Tipo = "X";
                                        resultado.Add(_objdto);
                                        break;

                                    case "A":
                                        var objdto_ = new ReportePlanillaBoleta();
                                        var gi_ = G_I.FirstOrDefault();
                                        var gd_ = G_D.FirstOrDefault();
                                        var ga_ = G_A.FirstOrDefault();
                                        if (ga_ == null) continue;

                                        objdto_ = (ReportePlanillaBoleta)ga_.Clone();
                                        G_A.Remove(ga_);

                                        if (gd_ != null)
                                        {
                                            objdto_.NombreConceptoDescuentos = gd_.NombreConceptoDescuentos;
                                            objdto_.ImporteDescuentos = gd_.ImporteDescuentos;
                                            G_D.Remove(gd_);
                                        }

                                        if (gi_ != null)
                                        {
                                            objdto_.NombreConceptoIngreso = gi_.NombreConceptoIngreso;
                                            objdto_.ImporteIngreso = gi_.ImporteIngreso;
                                            G_I.Remove(gi_);
                                        }

                                        objdto_.Tipo = "X";
                                        resultado.Add(objdto_);
                                        break;
                                }

                            }
                            #endregion
                        }

                        #region Ingresa filas falsas para fijar el tamaño de la boleta
                        var limitRows = 10;
                        foreach (var trabajador in grupo)
                        {
                            var cant = resultado.Count(p => p.NombreTrabajador == trabajador);

                            if (cant < limitRows)
                            {
                                var fillNumber = limitRows - cant;
                                var fakeRowT = (ReportePlanillaBoleta)resultado.LastOrDefault(p => p.NombreTrabajador == trabajador).Clone();

                                fakeRowT.ImporteIngreso = 0;
                                fakeRowT.ImporteAportaciones = 0;
                                fakeRowT.ImporteDescuentos = 0;

                                for (int i = 0; i < fillNumber; i++)
                                {
                                    var fakeRow = (ReportePlanillaBoleta)fakeRowT.Clone();
                                    fakeRow.Id = i * -1;
                                    resultado.Add(fakeRow);
                                }
                            }
                        }
                        #endregion

                        #endregion

                        pobjOperationResult.Success = 1;

                        if (!duplicarBoletas)
                            return resultado;

                        var resultadoX2 = new List<ReportePlanillaBoleta>();
                        resultadoX2 = resultado.Select(p => (ReportePlanillaBoleta)p.Clone()).ToList();
                        resultadoX2.ForEach(p => p.IdTrabajador = p.IdTrabajador + "X2");
                        pobjOperationResult.Success = 1;

                        return resultado.Concat(resultadoX2).OrderBy(p => p.NroDocumento).ToList();
                    }

                    throw new ArgumentNullException("No existe un cálculo guardado para esta planilla seleccionada.!");
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaCalculo.ObtenerCalculoPlanilla()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public static void CopiarPlanilla(ref OperationResult pobjOperationResult, string pstrPeriodoOrigen,
            string pstrMesOrigen, string pstrPeriodoDestino, string pstrMesDestino, bool copiaPlanilla, int pintIdPlanillaOrigen, int pintIdPlanillaDestino, List<string> clientSession)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        #region Afectaciones
                        dbContext.planillaafectacionesgenerales.Where(p => p.v_Periodo == pstrPeriodoDestino && p.v_Mes == pstrMesDestino && p.i_Eliminado == 0).ToList()
                                           .ForEach(o => { dbContext.DeleteObject(o); });

                        var oplanillaafectacionesgenerales =
                            dbContext.planillaafectacionesgenerales.Where(p => p.v_Periodo == pstrPeriodoOrigen && p.v_Mes == pstrMesOrigen && p.i_Eliminado == 0).ToList()
                                .Select(o => new planillaafectacionesgenerales
                                {
                                    v_IdConceptoPlanilla = o.v_IdConceptoPlanilla,
                                    t_InsertaFecha = DateTime.Now,
                                    i_Eliminado = o.i_Eliminado,
                                    i_InsertaIdUsuario = Globals.ClientSession.i_SystemUserId,
                                    v_Mes = pstrMesDestino,
                                    v_Periodo = pstrPeriodoDestino,
                                    i_AFP_Afecto = o.i_AFP_Afecto,
                                    i_Leyes_Emp_Essalud = o.i_Leyes_Emp_Essalud,
                                    i_Leyes_Emp_SCTR = o.i_Leyes_Emp_SCTR,
                                    i_Leyes_Trab_ONP = o.i_Leyes_Trab_ONP,
                                    i_Leyes_Trab_SCTR = o.i_Leyes_Trab_SCTR,
                                    i_Leyes_Trab_Senati = o.i_Leyes_Trab_Senati,
                                    i_Rent5ta_Afecto = o.i_Rent5ta_Afecto
                                });

                        oplanillaafectacionesgenerales.ToList().ForEach(p => dbContext.planillaafectacionesgenerales.AddObject(p));

                        #endregion

                        #region Leyes

                        #region planillaporcleyesempleador
                        dbContext.planillaporcleyesempleador.Where(p => p.v_Periodo == pstrPeriodoDestino && p.v_Mes == pstrMesDestino && p.i_Eliminado == 0).ToList()
                                           .ForEach(o => { dbContext.DeleteObject(o); });

                        var planillaporcleyesempleador =
                            dbContext.planillaporcleyesempleador.Where(p => p.v_Periodo == pstrPeriodoOrigen && p.v_Mes == pstrMesOrigen && p.i_Eliminado == 0).ToList()
                                .Select(o => new planillaporcleyesempleador
                                {
                                    t_InsertaFecha = o.t_InsertaFecha,
                                    i_Eliminado = o.i_Eliminado,
                                    t_ActualizaFecha = o.t_ActualizaFecha,
                                    i_ActualizaIdUsuario = o.i_ActualizaIdUsuario,
                                    i_InsertaIdUsuario = o.i_InsertaIdUsuario,
                                    v_Mes = pstrMesDestino,
                                    v_Periodo = pstrPeriodoDestino,
                                    d_EsSalud = o.d_EsSalud,
                                    d_SCTR = o.d_SCTR,
                                    d_SCTRPen = o.d_SCTRPen,
                                    d_Senati = o.d_Senati
                                });

                        planillaporcleyesempleador.ToList().ForEach(p => dbContext.planillaporcleyesempleador.AddObject(p));
                        #endregion

                        #region planillaporcafp
                        dbContext.planillaporcafp.Where(p => p.v_Periodo == pstrPeriodoDestino && p.v_Mes == pstrMesDestino && p.i_Eliminado == 0).ToList()
                                           .ForEach(o => { dbContext.DeleteObject(o); });

                        var planillaporcafp =
                            dbContext.planillaporcafp.Where(p => p.v_Periodo == pstrPeriodoOrigen && p.v_Mes == pstrMesOrigen && p.i_Eliminado == 0).ToList()
                                .Select(o => new planillaporcafp
                                {
                                    t_InsertaFecha = o.t_InsertaFecha,
                                    i_Eliminado = o.i_Eliminado,
                                    t_ActualizaFecha = o.t_ActualizaFecha,
                                    i_ActualizaIdUsuario = o.i_ActualizaIdUsuario,
                                    i_InsertaIdUsuario = o.i_InsertaIdUsuario,
                                    v_Mes = pstrMesDestino,
                                    v_Periodo = pstrPeriodoDestino,
                                    d_PorcentajeTope = o.d_PorcentajeTope,
                                    i_IdAfp = o.i_IdAfp,
                                    d_AporteObligatorio = o.d_AporteObligatorio,
                                    d_ComisionFlujo = o.d_ComisionFlujo,
                                    d_ComisionSaldo = o.d_ComisionSaldo,
                                    d_IPSS = o.d_IPSS,
                                    d_PorcentajePrima = o.d_PorcentajePrima
                                });

                        planillaporcafp.ToList().ForEach(p => dbContext.planillaporcafp.AddObject(p));
                        #endregion

                        #region planillaporcleyestrabajador
                        dbContext.planillaporcleyestrabajador.Where(p => p.v_Periodo == pstrPeriodoDestino && p.v_Mes == pstrMesDestino && p.i_Eliminado == 0).ToList()
                                           .ForEach(o => { dbContext.DeleteObject(o); });

                        var planillaporcleyestrabajador =
                            dbContext.planillaporcleyestrabajador.Where(p => p.v_Periodo == pstrPeriodoOrigen && p.v_Mes == pstrMesOrigen && p.i_Eliminado == 0).ToList()
                                .Select(o => new planillaporcleyestrabajador
                                {
                                    t_InsertaFecha = o.t_InsertaFecha,
                                    i_Eliminado = o.i_Eliminado,
                                    t_ActualizaFecha = o.t_ActualizaFecha,
                                    i_ActualizaIdUsuario = o.i_ActualizaIdUsuario,
                                    i_InsertaIdUsuario = o.i_InsertaIdUsuario,
                                    v_Mes = pstrMesDestino,
                                    v_Periodo = pstrPeriodoDestino,
                                    d_Senati = o.d_Senati,
                                    d_SCTR = o.d_SCTR,
                                    d_EsSalud = o.d_EsSalud,
                                    d_ONP = o.d_ONP
                                });

                        planillaporcleyestrabajador.ToList().ForEach(p => dbContext.planillaporcleyestrabajador.AddObject(p));
                        #endregion

                        #endregion

                        #region Variables

                        if (copiaPlanilla)
                        {
                            var SecuentialBL = new SecuentialBL();
                            var existePlanillaDestino = dbContext.planillanumeracion.Any(p => p.i_Id == pintIdPlanillaDestino);
                            if (existePlanillaDestino)
                            {
                                #region Elimina variables con la planilla de destino primero
                                dbContext.planillavariablesaportaciones.Where(
                                                            p => p.planillavariablestrabajador.i_IdPlanillaNumeracion == pintIdPlanillaDestino)
                                                            .ToList()
                                                            .ForEach(o => dbContext.DeleteObject(o));

                                dbContext.planillavariablesdescuentos.Where(
                                    p => p.planillavariablestrabajador.i_IdPlanillaNumeracion == pintIdPlanillaDestino)
                                    .ToList()
                                    .ForEach(o => dbContext.DeleteObject(o));

                                dbContext.planillavariablesingresos.Where(
                                    p => p.planillavariablestrabajador.i_IdPlanillaNumeracion == pintIdPlanillaDestino)
                                    .ToList()
                                    .ForEach(o => dbContext.DeleteObject(o));

                                dbContext.planillavariablesdiasnolabsubsidiados.Where(
                                    p => p.planillavariablestrabajador.i_IdPlanillaNumeracion == pintIdPlanillaDestino)
                                    .ToList()
                                    .ForEach(o => dbContext.DeleteObject(o));

                                dbContext.planillavariableshorasextras.Where(
                                    p => p.planillavariablestrabajador.i_IdPlanillaNumeracion == pintIdPlanillaDestino)
                                    .ToList()
                                    .ForEach(o => dbContext.DeleteObject(o));

                                dbContext.planillavariablestrabajador.Where(
                                    p => p.i_IdPlanillaNumeracion == pintIdPlanillaDestino)
                                    .ToList()
                                    .ForEach(o => dbContext.DeleteObject(o));

                                dbContext.planillacalculo.Where(p => p.i_IdPlanillaNumeracion == pintIdPlanillaDestino).ToList()
                                    .ForEach(o => dbContext.DeleteObject(o));
                                #endregion

                                #region Inserta duplicado de las variables de la planilla de origen

                                var variableOrigen =
                                    dbContext.planillavariablestrabajador.Where(
                                        p => p.i_IdPlanillaNumeracion == pintIdPlanillaOrigen && p.i_Eliminado == 0)
                                        .ToList();

                                var variablesTrabajadoresDestino =
                                    variableOrigen.Select(p => new planillavariablestrabajadorDto
                                    {
                                        i_InsertaIdUsuario = p.i_InsertaIdUsuario,
                                        t_ActualizaFecha = p.t_ActualizaFecha,
                                        t_InsertaFecha = p.t_InsertaFecha,
                                        d_HorasLaboradosBP = p.d_HorasLaboradosBP,
                                        d_Imp = p.d_Imp,
                                        d_TiempoTardanza = 0,
                                        d_TotalDescuentoDolares = 0,
                                        d_TotalDescuentoSoles = 0,
                                        d_TotalIngresoDolares = p.d_TotalDescuentoDolares,
                                        d_TotalIngresoSoles = p.d_TotalIngresoSoles,
                                        i_ActualizaIdUsuario = p.i_ActualizaIdUsuario,
                                        i_AfectoSCTR = p.i_AfectoSCTR,
                                        i_AfectoSCTRPen = p.i_AfectoSCTRPen,
                                        i_AfectoSenati = p.i_AfectoSenati,
                                        i_DiasLaborados = p.i_DiasLaborados,
                                        i_DiasLaboradosBP = p.i_DiasLaboradosBP,
                                        i_DiasNoLaborados = 0,
                                        i_DiasSubsidiados = 0,
                                        i_Eliminado = p.i_Eliminado,
                                        i_IdPlanillaNumeracion = pintIdPlanillaDestino,
                                        i_Movilidad = 0,
                                        i_NoAplicarDCTOLeyesAFP = p.i_NoAplicarDCTOLeyesAFP,
                                        i_Pago = p.i_Pago,
                                        t_FechaVacacionesFin = p.t_FechaVacacionesFin,
                                        t_FechaVacacionesInicio = p.t_FechaVacacionesInicio,
                                        v_IdContrato = p.v_IdContrato,
                                        v_IdTrabajador = p.v_IdTrabajador,
                                        v_IdPlanillaVariablesTrabajador =
                                            Utils.GetNewId(int.Parse(clientSession[0]),
                                                SecuentialBL.GetNextSecuentialId(int.Parse(clientSession[0]), 93), "VT"),
                                        ListaIngresos = p.planillavariablesingresos.Where(a => a.i_Eliminado == 0).ToDTOs(),
                                        ListaAportaciones = p.planillavariablesaportaciones.Where(b => b.i_Eliminado == 0).ToDTOs()
                                    }).ToList();

                                variablesTrabajadoresDestino.ToList().ForEach(p => dbContext.planillavariablestrabajador.AddObject(p.ToEntity()));
                                dbContext.SaveChanges();

                                var variablesTrabajadoresDestinoIngresos =
                                    variablesTrabajadoresDestino.Select(
                                        p => p.ListaIngresos.Select(o => new planillavariablesingresos
                                        {
                                            v_IdPlanillaVariablesTrabajador = p.v_IdPlanillaVariablesTrabajador,
                                            i_Eliminado = o.i_Eliminado,
                                            t_InsertaFecha = o.t_InsertaFecha,
                                            i_ActualizaIdUsuario = o.i_ActualizaIdUsuario,
                                            t_ActualizaFecha = o.t_ActualizaFecha,
                                            i_InsertaIdUsuario = o.i_InsertaIdUsuario,
                                            d_Importe = o.d_Importe,
                                            i_IdMoneda = o.i_IdMoneda,
                                            v_IdConceptoPlanilla = o.v_IdConceptoPlanilla,
                                            v_IdPlanillaVariablesIngresos = Utils.GetNewId(int.Parse(clientSession[0]),
                                                SecuentialBL.GetNextSecuentialId(int.Parse(clientSession[0]), 95),
                                                "VI")

                                        }));

                                variablesTrabajadoresDestinoIngresos.ToList().ForEach(p => p.ToList().ForEach(o => dbContext.planillavariablesingresos.AddObject(o)));

                                var variablesTrabajadoresDestinoAportaciones =
                                    variablesTrabajadoresDestino.Select(
                                        p => p.ListaAportaciones.Select(o => new planillavariablesaportaciones
                                        {
                                            v_IdPlanillaVariablesTrabajador = p.v_IdPlanillaVariablesTrabajador,
                                            i_Eliminado = o.i_Eliminado,
                                            t_InsertaFecha = o.t_InsertaFecha,
                                            i_ActualizaIdUsuario = o.i_ActualizaIdUsuario,
                                            t_ActualizaFecha = o.t_ActualizaFecha,
                                            i_InsertaIdUsuario = o.i_InsertaIdUsuario,
                                            d_Importe = o.d_Importe,
                                            i_IdMoneda = o.i_IdMoneda,
                                            v_IdConceptoPlanilla = o.v_IdConceptoPlanilla,
                                            v_IdPlanillaVariablesAportaciones = Utils.GetNewId(int.Parse(clientSession[0]),
                                                SecuentialBL.GetNextSecuentialId(int.Parse(clientSession[0]), 100),
                                                "CK")

                                        }));

                                variablesTrabajadoresDestinoAportaciones.ToList().ForEach(p => p.ToList().ForEach(o => dbContext.planillavariablesaportaciones.AddObject(o)));
                                #endregion
                            }
                        }
                        #endregion

                        dbContext.SaveChanges();
                        ts.Complete();
                        pobjOperationResult.Success = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaCalculo.CopiarPlanilla()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static bool ExisteInformacionEnPlanillaDestino(int pintIdPlanillaDestino)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var existeData =
                        dbContext.planillavariablestrabajador.Any(
                            p => p.i_IdPlanillaNumeracion == pintIdPlanillaDestino && p.i_Eliminado == 0);

                    return existeData;
                }
            }
            catch
            {
                return false;
            }

        }

        #region Métodos para el CRUD
        /// <summary>
        /// Retorna un booleano con true si existen registros con el id de la planilla seleccionada o false si no existen registros aún.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pintIdPlanilla">Id de la planilla por la que se desea consultar.</param>
        /// <returns></returns>
        public static Boolean ExisteCalculoAnterior(ref OperationResult pobjOperationResult, int pintIdPlanilla)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var result = dbContext.planillacalculo.Any(p => p.i_IdPlanillaNumeracion == pintIdPlanilla);
                    pobjOperationResult.Success = 1;
                    return result;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaCalculo.ExisteCalculoAnterior()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return false;
            }
        }

        /// <summary>
        /// Inserta una coleccion de calculoplanillaDto a la tabla planillacalculo, si existe data con el mismo Id Planilla la elimina primero y luego inserta la nueva colección.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="tempInsertar"></param>
        /// <param name="pintIdPlanilla">Id de la planilla del que se quiere insertar el calculo.</param>
        public static void ActualizarCalculoPlanillas(ref OperationResult pobjOperationResult,
            List<planillacalculoDto> tempInsertar, int pintIdPlanilla)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        if (!tempInsertar.Any()) throw new ArgumentNullException("No existen ítems para guardar.");

                        var existeData = dbContext.planillacalculo.Any(p => p.i_IdPlanillaNumeracion == pintIdPlanilla);

                        if (!existeData)
                        {
                            foreach (
                                var entity in tempInsertar.Select(planillacalculoDto => planillacalculoDto.ToEntity()))
                                dbContext.AddToplanillacalculo(entity);
                        }
                        else
                        {
                            LimpiarCalculoPlanillas(ref pobjOperationResult, pintIdPlanilla);
                            if (pobjOperationResult.Success == 0) return;

                            foreach (
                                var entity in tempInsertar.Select(planillacalculoDto => planillacalculoDto.ToEntity()))
                                dbContext.AddToplanillacalculo(entity);
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
                pobjOperationResult.AdditionalInformation = "PlanillaCalculo.ActualizarCalculoPlanillas()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        /// <summary>
        /// Elimina la colección anterior de calculos
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pintIdPlanilla"></param>
        private static void LimpiarCalculoPlanillas(ref OperationResult pobjOperationResult, int pintIdPlanilla)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var calculosPorEliminar =
                            dbContext.planillacalculo.Where(p => p.i_IdPlanillaNumeracion == pintIdPlanilla);

                        if (calculosPorEliminar.Any())
                        {
                            foreach (var planillacalculo in calculosPorEliminar)
                                dbContext.planillacalculo.DeleteObject(planillacalculo);

                            dbContext.SaveChanges();
                        }

                        pobjOperationResult.Success = 1;
                        ts.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaCalculo.LimpiarCalculoPlanillas()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static List<planillacalculoDto> ObtenerCalculoPlanilla(ref OperationResult pobjOperationResult,
            int pintIdPlanilla)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var consulta = dbContext.planillacalculo.Where(p => p.i_IdPlanillaNumeracion == pintIdPlanilla);
                    if (consulta.Any())
                    {
                        var result = (from r in dbContext.planillacalculo

                                      join J1 in dbContext.trabajador on r.v_IdTrabajador equals J1.v_IdTrabajador into J1_join
                                      from J1 in J1_join.DefaultIfEmpty()

                                      join J2 in dbContext.planillaconceptos on r.v_IdConceptoPlanilla equals J2.v_IdConceptoPlanilla
                                          into J2_join
                                      from J2 in J2_join.DefaultIfEmpty()

                                      where r.i_IdPlanillaNumeracion == pintIdPlanilla && J1.i_Eliminado == 0

                                      select new planillacalculoDto
                                      {
                                          v_IdTrabajador = r.v_IdTrabajador,
                                          i_IdPlanillaNumeracion = r.i_IdPlanillaNumeracion,
                                          v_IdConceptoPlanilla = r.v_IdConceptoPlanilla,
                                          v_IdContrato = r.v_IdContrato,
                                          d_Importe = r.d_Importe,
                                          i_Id = r.i_Id,
                                          i_IdAfp = r.i_IdAfp,
                                          NombreConcepto = J2.v_Nombre,
                                          Tipo = r.Tipo,
                                          NombreTrabajador =
                                              (J1.cliente.v_ApePaterno + " " + J1.cliente.v_ApeMaterno + " " +
                                               J1.cliente.v_PrimerNombre + " " + J1.cliente.v_RazonSocial).Trim()
                                      }).ToList();

                        pobjOperationResult.Success = 1;
                        return result;
                    }

                    pobjOperationResult.Success = 1;
                    return null;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PlanillaCalculo.ObtenerCalculoPlanilla()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }
        #endregion
    }

    public static class CalculoDescuentosIngresos
    {
        /// <summary>
        /// Devuelve el monto de descuento por faltas injustificadas.
        /// </summary>
        /// <param name="sueldoBase"></param>
        /// <param name="diasFaltados"></param>
        /// <returns></returns>
        public static decimal CalcularDescuentoPorFaltasInjustificadas(decimal sueldoBase, decimal diasFaltados)
        {
            try
            {
                //!jornada diaria * minutos/hora
                const int minutosDiarios = 8 * 60;

                //!días laborables a la semana.
                const int diasLaborables = 6;

                //!sueldo base/dias mes/horas al dia/minutos por hora
                var dctoPorMinuto = Utils.Windows.DevuelveValorRedondeado(sueldoBase / 30 / 8 / 60, 2);

                var minutos = Utils.Windows.DevuelveValorRedondeado(minutosDiarios * diasFaltados, 2);
                var descuentoNeto = Utils.Windows.DevuelveValorRedondeado(minutos * dctoPorMinuto, 2);
                var dominical = Utils.Windows.DevuelveValorRedondeado(descuentoNeto / diasLaborables, 2);
                var descuentoTotal = Globals.ClientSession.i_UsaDominicalCalculoDescuento == 1 ? descuentoNeto + dominical : descuentoNeto;
                return descuentoTotal;
            }
            catch
            {
                return 0M;
            }
        }

        /// <summary>
        /// Calcula el importe descontado por tardanzas.
        /// </summary>
        /// <param name="sueldoBase"></param>
        /// <param name="minutos"></param>
        /// <returns></returns>
        public static decimal CalcularDescuentoPorTardanzas(decimal sueldoBase, decimal minutos)
        {
            try
            {
                //!sueldo base/dias mes/horas al dia/minutos por hora
                var dctoPorMinuto = Utils.Windows.DevuelveValorRedondeado(sueldoBase / 30 / 8 / 60, 2);

                return Utils.Windows.DevuelveValorRedondeado(minutos * dctoPorMinuto, 2);
            }
            catch
            {
                return 0M;
            }
        }

        public static decimal ObtenerIngresoHorasExtras(decimal sueldoBase, decimal horas, int idTipoHrExtras, out string idConcepto)
        {
            idConcepto = string.Empty;
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    decimal d;
                    const int diasMes = 30;
                    const int horasDia = 8;
                    #region Recopila datos de Hora extra.

                    var dataHe = (from n in dbContext.datahierarchy
                                  join t in dbContext.datahierarchy on new { g = 163, id = n.i_Header.Value }
                                      equals new { g = t.i_GroupId, id = t.i_ItemId } into tjoin
                                  from t in tjoin.DefaultIfEmpty()
                                  where n.i_ItemId == idTipoHrExtras && n.i_GroupId == 162 && n.i_IsDeleted == 0
                                  select new
                                  {
                                      factorHrExtra = n.v_Value2,
                                      factorTurno = t != null ? t.v_Value2 : "0",
                                      idConcepto = n.v_Value4
                                  }).FirstOrDefault();

                    if (dataHe == null) return 0;

                    #endregion

                    idConcepto = dataHe.idConcepto;
                    var factorHrExtra = decimal.TryParse(dataHe.factorHrExtra, out d) ? d : 0M;
                    var factorTurno = decimal.TryParse(dataHe.factorTurno, out d) ? d : 0M;

                    var importeDia = Utils.Windows.DevuelveValorRedondeado((sueldoBase / diasMes) * factorTurno, 2);
                    var importeHora = Utils.Windows.DevuelveValorRedondeado(importeDia / horasDia, 2);
                    var importeNeto = Utils.Windows.DevuelveValorRedondeado(importeHora * factorHrExtra, 2);
                    return Utils.Windows.DevuelveValorRedondeado(importeNeto * horas, 2);
                }

            }
            catch (Exception)
            {
                return 0m;
            }
        }
    }

    public class ColumnasAfp
    {
        public string Nombre { get; set; }
        public PlanillaColumnasAFP Columna { get; set; }
    }

    public class ColumnasTrabajador
    {
        public string Nombre { get; set; }
        public PlanillaColumnasTrabajador Columna { get; set; }
    }

    public class ColumnasEmpleador
    {
        public string Nombre { get; set; }
        public PlanillaColumnasEmpleador Columna { get; set; }
    }
}


