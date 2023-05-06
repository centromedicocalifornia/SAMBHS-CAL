using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using System.Linq.Dynamic;
using SAMBHS.CommonWIN.BL;

namespace SAMBHS.Tesoreria.BL
{
    public partial class SaldoEstadoBancarioBL
    {
        private string PeriodoActual = Globals.ClientSession.i_Periodo.ToString();
        public string ObtenerNombreCuenta(string pstrNumeroCuenta)
        {
            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
            try
            {
                var AsientoContable = (from n in dbContext.asientocontable

                                       where n.i_Eliminado == 0 && n.v_NroCuenta == pstrNumeroCuenta && n.i_Imputable == 1 && n.v_Periodo ==PeriodoActual 
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
            { throw e;
            }
        }

        public List<saldoestadobancarioDto > ObtenerListadoSaldoEstadoBancario(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression)
        {

            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = from A in dbContext.saldoestadobancario 


                            where A.i_Eliminado == 0

                            select new saldoestadobancarioDto 
                            {
                                v_NroCuenta = A.v_NroCuenta,
                                v_Mes = A.v_Mes,
                                v_IdSaldoEstadoBancario = A.v_IdSaldoEstadoBancario,
                                v_Anio =A.v_Anio,
                               d_SaldoBanco =A.d_SaldoBanco ,
                                i_ActualizaIdUsuario = A.i_ActualizaIdUsuario,
                                i_InsertaIdUsuario = A.i_InsertaIdUsuario,
                                t_ActualizaFecha = A.t_ActualizaFecha,
                                t_InsertaFecha = A.t_InsertaFecha

                            };

                if (!string.IsNullOrEmpty(pstrFilterExpression))
                {
                    query = query.Where(pstrFilterExpression);
                }
                if (!string.IsNullOrEmpty(pstrSortExpression))
                {
                    query = query.OrderBy(pstrSortExpression);
                }

                List<saldoestadobancarioDto> objData = query.ToList();
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

        public string InsertarSaldoEstadoBancario(ref OperationResult pobjOperationResult, saldoestadobancarioDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    SecuentialBL objSecuentialBL = new SecuentialBL();
                    saldoestadobancario objEntity = saldoestadobancarioAssembler.ToEntity(pobjDtoEntity);

                    objEntity.t_InsertaFecha = DateTime.Now;
                    objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntity.i_Eliminado = 0;

                    // Autogeneramos el Pk de la tabla
                    int intNodeId = int.Parse(ClientSession[0]);
                    var SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 65);
                    var newId = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "YC");
                    objEntity.v_IdSaldoEstadoBancario = newId;
                    dbContext.AddTosaldoestadobancario(objEntity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                    return newId; 
                }
            }
            catch (Exception ex)
            {
                
               
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "SaldoEstadoBancarioBL.InsertarSaldoEstadoBancario()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return string.Empty;
            }
        }

        public string ActualizarSaldoEstadoBancario(ref OperationResult pobjOperationResult,saldoestadobancarioDto pobjDtoEntity, List<string> ClientSession)
        {

            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.saldoestadobancario
                                       where a.v_IdSaldoEstadoBancario == pobjDtoEntity.v_IdSaldoEstadoBancario
                                       select a).FirstOrDefault();

                // Crear la entidad con los datos actualizados
                pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);


                saldoestadobancario objEntity = saldoestadobancarioAssembler.ToEntity(pobjDtoEntity);
                dbContext.saldoestadobancario.ApplyCurrentValues(objEntity);

                // Guardar los cambios
                dbContext.SaveChanges();

                pobjOperationResult.Success = 1;
                return pobjDtoEntity.v_IdSaldoEstadoBancario;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "SaldoEstadoBancarioBL.ActualizarSaldoEstadoBancario()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return string.Empty;
            }
        }

        public void EliminarSaldoEstadoBancario(ref OperationResult pobjOperationResult, string pstrIdSaldoEstadoBancario, List<string> ClientSession)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.saldoestadobancario
                                       where a.v_IdSaldoEstadoBancario == pstrIdSaldoEstadoBancario
                                       select a).FirstOrDefault();

                // Crear la entidad con los datos actualizados
                objEntitySource.t_ActualizaFecha = DateTime.Now;
                objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                objEntitySource.i_Eliminado = 1;


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
        public saldoestadobancarioDto ObtenerSaldoEstadoBancario(ref OperationResult pobjOperationResult, string pstrIdSaldoEstadoBancario)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                saldoestadobancarioDto objDtoEntity = null;
                var objEntity = (from A in dbContext.saldoestadobancario
                                 where A.v_IdSaldoEstadoBancario == pstrIdSaldoEstadoBancario
                                 select A
                                 ).FirstOrDefault();
                if (objEntity != null)
                    objDtoEntity = saldoestadobancarioAssembler.ToDTO(objEntity);

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
        public decimal ObtenerSaldoSegunBanco(string pstrCuenta, string pstrAnio, string pstrMes)
        {

            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

            var query = (from n in dbContext.saldoestadobancario
                         where n.i_Eliminado == 0 && n.v_NroCuenta == pstrCuenta && n.v_Anio  == pstrAnio && n.v_Mes == pstrMes

                         select new { n.d_SaldoBanco }).FirstOrDefault();

            if (query != null)
            {
                return query.d_SaldoBanco.Value;

            }
            else
            {
                return 0;
            }

       
        }


        public string ObtenerIdSaldoSegunBanco(string pstrCuenta, string pstrAnio, string pstrMes)
        {

            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

            var query = (from n in dbContext.saldoestadobancario

                         where n.i_Eliminado == 0 && n.v_Mes == pstrMes && n.v_Anio  == pstrAnio && n.v_NroCuenta == pstrCuenta

                         select new { n.v_IdSaldoEstadoBancario }).FirstOrDefault();
            if (query != null)
            {
                return query.v_IdSaldoEstadoBancario;

            }
            else

            {

                return null;
            }
        
        
        }
    }

}
