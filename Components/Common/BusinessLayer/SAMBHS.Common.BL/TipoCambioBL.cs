using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using SAMBHS.Common.DataModel;

namespace SAMBHS.Common.BL
{
    public class TipoCambioBL
    {
        public List<tipodecambioDto> ObtenerListadoTipoCambio(ref OperationResult pobjOperationResult, int periodo, int mes)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                var query = from A in dbContext.tipodecambio
                            join J1 in dbContext.systemuser on new { i_InsertUserId = A.i_InsertUserId.Value }
                                                        equals new { i_InsertUserId = J1.i_SystemUserId } into J1_join
                            from J1 in J1_join.DefaultIfEmpty()

                            join J2 in dbContext.systemuser on new { i_UpdateUserId = A.i_UpdateUserId.Value }
                                                        equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                            from J2 in J2_join.DefaultIfEmpty()

                            where A.i_IsDeleted == 0 && A.d_FechaTipoC.Value.Year == periodo && A.d_FechaTipoC.Value.Month == mes

                            select new tipodecambioDto
                            {
                                i_CodTipoCambio = A.i_CodTipoCambio,
                                i_Periodo = A.i_Periodo,
                                d_FechaTipoC = A.d_FechaTipoC,
                                d_ValorCompra = A.d_ValorCompra,
                                d_ValorVenta = A.d_ValorVenta,
                                d_InsertDate = A.d_InsertDate,
                                d_UpdateDate = A.d_UpdateDate,
                                v_UsuarioCreacion = J1.v_UserName,
                                v_UsuarioModificacion = J2.v_UserName,
                                d_ValorCompraContable =A.d_ValorCompraContable ,
                                d_ValorVentaContable =A.d_ValorVentaContable ,
                            };

                List<tipodecambioDto> objData = query.ToList();
                pobjOperationResult.Success = 1;
                return objData;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public tipodecambioDto ObtenerTipoCambio(ref OperationResult pobjOperationResult, int pintCodTipoCambio)
        {
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                tipodecambioDto objDtoEntity = null;
                var objEntity = (from A in dbContext.tipodecambio
                                 where A.i_CodTipoCambio == pintCodTipoCambio
                                 select A
                                 ).FirstOrDefault();
                if (objEntity != null)
                    objDtoEntity = tipodecambioAssembler.ToDTO(objEntity);

                pobjOperationResult.Success = 1;
                return objDtoEntity;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public void InsertarTipoCambio(ref OperationResult pobjOperationResult, tipodecambioDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;
            int SecuentialId = 0;
            string newId = string.Empty;
            try
            {
                if (ExisteTipoCambioPorFecha(ref pobjOperationResult, pobjDtoEntity.d_FechaTipoC.Value)) return;

                using (SAMBHSEntitiesModel dbContext = new Common.DataModel.SAMBHSEntitiesModel())
                {
                    tipodecambio objEntity = tipodecambioAssembler.ToEntity(pobjDtoEntity);
                    objEntity.i_CodTipoCambio = dbContext.tipodecambio.Any() ? dbContext.tipodecambio.Max(p => p.i_CodTipoCambio) + 1 : 1;
                    objEntity.d_InsertDate = DateTime.Now;
                    objEntity.i_InsertUserId = Int32.Parse(ClientSession[2]);
                    objEntity.i_IsDeleted = 0;

                    dbContext.AddTotipodecambio(objEntity);
                    dbContext.SaveChanges();

                    pobjOperationResult.Success = 1;

                    return;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return;
            }
        }

        public void Actualizartipodecambio(ref OperationResult pobjOperationResult, tipodecambioDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.tipodecambio
                                       where a.i_CodTipoCambio == pobjDtoEntity.i_CodTipoCambio
                                       select a).FirstOrDefault();

                // Crear la entidad con los datos actualizados
                pobjDtoEntity.d_UpdateDate = DateTime.Now;
                pobjDtoEntity.i_UpdateUserId = Int32.Parse(ClientSession[2]);


                tipodecambio objEntity = tipodecambioAssembler.ToEntity(pobjDtoEntity);
                dbContext.tipodecambio.ApplyCurrentValues(objEntity);

                // Guardar los cambios
                dbContext.SaveChanges();

                pobjOperationResult.Success = 1;
                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return;
            }
        }

        public void Eliminartipodecambio(ref OperationResult pobjOperationResult, int pintCodTipoCambio, List<string> ClientSession)
        {
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.tipodecambio
                                       where a.i_CodTipoCambio == pintCodTipoCambio
                                       select a).FirstOrDefault();

                // Crear la entidad con los datos actualizados
                objEntitySource.d_UpdateDate = DateTime.Now;
                objEntitySource.i_UpdateUserId = Int32.Parse(ClientSession[2]);
                objEntitySource.i_IsDeleted = 1;

                // Guardar los cambios
                dbContext.SaveChanges();

                pobjOperationResult.Success = 1;
                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return;
            }
        }

        public bool ExisteTipoCambioPorFecha(ref OperationResult pobjOperationResult, DateTime pdatetimeFecha)
        {
            try
            {
                using (SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel())
                {
                    int Periodo = pdatetimeFecha.Year;
                    int Mes = pdatetimeFecha.Month;
                    int Dia = pdatetimeFecha.Day;

                    return dbContext.tipodecambio.Count(p => p.d_FechaTipoC.Value.Year == Periodo
                        && p.d_FechaTipoC.Value.Month == Mes
                        && p.d_FechaTipoC.Value.Day == Dia && p.i_IsDeleted == 0
                        ) > 0;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return false;
            }

        }

        public void InsertarTiposCambios(ref OperationResult pobjOperationResult, List<tipodecambioDto> l_pobjDtoEntity, List<string> ClientSession)
        {
            string newId = string.Empty;
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                foreach (tipodecambioDto pobjDtoEntity in l_pobjDtoEntity)
                {
                    tipodecambio objEntity = tipodecambioAssembler.ToEntity(pobjDtoEntity);

                    objEntity.d_InsertDate = DateTime.Now;
                    objEntity.i_InsertUserId = Int32.Parse(ClientSession[2]);
                    objEntity.i_IsDeleted = 0;

                    dbContext.AddTotipodecambio(objEntity);
                }

                dbContext.SaveChanges();

                pobjOperationResult.Success = 1;

                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return;
            }
        }

        public string TipoCambioCompraVenta(DateTime Fecha, string tipo)
        {
            try
            {
                using (SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel())
                {
                    var query = dbContext.tipodecambio.FirstOrDefault(n => n.i_IsDeleted == 0 && n.d_FechaTipoC == Fecha);

                    return tipo.Trim().Equals("C") ? query.d_ValorCompraContable.Value.ToString() : query.d_ValorVentaContable.Value.ToString();
                }
            }
            catch (Exception)
            {
                return "0";
            }
        }

        public string DevolverTipoCambioPorFechaCompra(ref OperationResult pobjOperationResult, DateTime Fecha)
        {

            try
            {
                using (SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel())
                {

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
                    return "0"; 
                }
            }
            catch (Exception ex)
            {
                
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ComprasBL.DevolverTipoCambioPorFecha()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }


        public string DevolverTipoCambioPorFechaVenta(ref OperationResult pobjOperationResult, DateTime Fecha)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModel())
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
                pobjOperationResult.AdditionalInformation = "ComprasBL.DevolverTipoCambioPorFecha()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }
      
        
        #region Reportes

        public List<ReporteTipoCambio> ReporteTipoCambio(decimal pstrMes, string pstrPeriodo)
        {

            string Mes = pstrMes.ToString("00");
            SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
            var ReporteTipoCambio = (from n in dbContext.tipodecambio

                                     where n.i_IsDeleted == 0
                                     select n).ToList();



            var ReporteTipoCambioFinal = (from a in ReporteTipoCambio

                                          where a.d_FechaTipoC.Value.Month.ToString("00") == Mes && a.d_FechaTipoC.Value.Year.ToString() == pstrPeriodo

                                          select new ReporteTipoCambio
                                          {

                                              Dia = a.d_FechaTipoC.Value.Day,
                                              TipoCambioCompraA = a.d_ValorCompra,
                                              TipoCambioVentaA = a.d_ValorVenta,
                                              TipoCambioCompraC = a.d_ValorCompraContable,
                                              TipoCambioVentaC = a.d_ValorVentaContable
                                          }).OrderBy(x => x.Dia).ToList();



            return ReporteTipoCambioFinal;


        }
        #endregion

    }
}