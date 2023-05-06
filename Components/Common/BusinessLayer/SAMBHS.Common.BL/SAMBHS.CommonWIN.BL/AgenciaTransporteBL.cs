using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic;
using System.Threading.Tasks;


namespace SAMBHS.CommonWIN.BL  
{
  public  class AgenciaTransporteBL
    {
        public List<agenciatransporteDto> ObtenerListadoAgenciaTransporte(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = from A in dbContext.agenciatransporte
                            join J1 in dbContext.systemuser on new { i_InsertUserId = A.i_InsertaIdUsuario.Value }
                                                            equals new { i_InsertUserId = J1.i_SystemUserId } into J1_join
                            from J1 in J1_join.DefaultIfEmpty()

                            join J2 in dbContext.systemuser on new { i_UpdateUserId = A.i_ActualizaIdUsuario.Value }
                                                            equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                            from J2 in J2_join.DefaultIfEmpty()

                            join J3 in dbContext.systemparameter on new { IdTipoDocumento = A.i_IdTipoIdentificacion.Value}
                                                            equals new { IdTipoDocumento = J3.i_ParameterId } into J3_join
                            from J3 in J3_join.DefaultIfEmpty()

                            where A.i_Eliminado == 0 && J3.i_GroupId == 150
                            select new agenciatransporteDto

                            {
                                v_CodTransportista = A.v_CodTransportista,
                                v_IdAgenciaTransporte = A.v_IdAgenciaTransporte,
                                v_NombreRazonSocial = A.v_NombreRazonSocial,
                                v_NombreContacto = A.v_NombreContacto,
                                v_Direccion = A.v_Direccion,
                                i_IdTipoPersona = A.i_IdTipoPersona,
                                i_IdTipoIdentificacion = A.i_IdTipoIdentificacion,
                                v_NumeroDocumento = A.v_NumeroDocumento,
                                i_IdPais = A.i_IdPais,
                                i_IdDepartamento = A.i_IdDepartamento,
                                i_IdProvincia = A.i_IdProvincia,
                                i_IdDistrito = A.i_IdDistrito,
                                v_Telefono = A.v_Telefono,
                                v_Fax = A.v_Fax,
                                v_CorreoElectronico = A.v_CorreoElectronico,
                                i_InsertaIdUsuario = A.i_InsertaIdUsuario,
                                t_InsertaFecha = A.t_InsertaFecha,
                                i_ActualizaIdUsuario = A.i_ActualizaIdUsuario,
                                t_ActualizaFecha = A.t_ActualizaFecha,
                                v_UsuarioCreacion = J1.v_UserName,
                                v_UsuarioModificacion = J2.v_UserName,
                                TipoDocumento = J3.v_Value1
                            };

                if (!string.IsNullOrEmpty(pstrFilterExpression))
                {
                    query = query.Where(pstrFilterExpression);
                }
                if (!string.IsNullOrEmpty(pstrSortExpression))
                {
                    query = query.OrderBy(pstrSortExpression);
                }

                List<agenciatransporteDto> objData = query.ToList();
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

        public agenciatransporteDto ObtenerAgenciaTransporte(ref OperationResult pobjOperationResult, string pstringIdAgenciaTransporte)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                agenciatransporteDto objDtoEntity = null;

                var objEntity = (from a in dbContext.agenciatransporte
                                 where a.v_IdAgenciaTransporte == pstringIdAgenciaTransporte
                                 select a).FirstOrDefault();

                if (objEntity != null)
                    objDtoEntity = agenciatransporteAssembler.ToDTO(objEntity);

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

        public void InsertarAgenciaTransporte(ref OperationResult pobjOperationResult, agenciatransporteDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;
            int SecuentialId = 0;
            string newId = string.Empty;
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                SecuentialBL objSecuentialBL = new SecuentialBL();
                agenciatransporte objEntity = agenciatransporteAssembler.ToEntity(pobjDtoEntity);


                objEntity.t_InsertaFecha = DateTime.Now;
                objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                objEntity.i_Eliminado = 0;

                // Autogeneramos el Pk de la tabla
                int intNodeId = int.Parse(ClientSession[0]);
                SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 12);
                newId = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "AT");
                objEntity.v_IdAgenciaTransporte = newId;


                dbContext.AddToagenciatransporte(objEntity);
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


        public agenciatransporteDto  ValidarExistenciaAgenciaTransporte(ref OperationResult objOperationResult, agenciatransporteDto pobjEntity,string NumeroRuc)
        {
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {

                if (pobjEntity.v_IdAgenciaTransporte == null)
                {
                    var agencia = (from a in dbContext.agenciatransporte
                                   where a.i_Eliminado == 0  && a.v_NumeroDocumento == NumeroRuc
                                   select a).FirstOrDefault();

                    if (agencia != null)
                    {
                        return agencia.ToDTO();
                    }
                }
                else
                {
                    var agencia = (from a in dbContext.agenciatransporte
                                   where a.i_Eliminado == 0 && a.v_IdAgenciaTransporte != pobjEntity.v_IdAgenciaTransporte && a.v_NumeroDocumento ==NumeroRuc 
                                   select a).FirstOrDefault();
                    if (agencia != null)
                    {
                        return agencia.ToDTO();
                    }
             }

                return null;
            
            }
        
        
        }
        public void ActualizarAgenciaTransporte(ref OperationResult pobjOperationResult, agenciatransporteDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.agenciatransporte
                                       where a.v_IdAgenciaTransporte == pobjDtoEntity.v_IdAgenciaTransporte
                                       select a).FirstOrDefault();

                // Crear la entidad con los datos actualizados
                pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);


                agenciatransporte objEntity = agenciatransporteAssembler.ToEntity(pobjDtoEntity);
                dbContext.agenciatransporte.ApplyCurrentValues(objEntity);

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

        public void EliminarAgenciaTransporte(ref OperationResult pobjOperationResult, string pstrIdAgenciaTransporte, List<string> ClientSession)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.agenciatransporte
                                       where a.v_IdAgenciaTransporte == pstrIdAgenciaTransporte
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


        public List<KeyValueDTO> ObtenerListadoAgenciaTransporteParaCombo(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression)
        {
            

            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = from A in dbContext.agenciatransporte
                            join J1 in dbContext.systemuser on new { i_InsertUserId = A.i_InsertaIdUsuario.Value }
                                                            equals new { i_InsertUserId = J1.i_SystemUserId } into J1_join
                            from J1 in J1_join.DefaultIfEmpty()

                            join J2 in dbContext.systemuser on new { i_UpdateUserId = A.i_ActualizaIdUsuario.Value }
                                                            equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                            from J2 in J2_join.DefaultIfEmpty()

                            join J3 in dbContext.systemparameter on new { IdTipoDocumento = A.i_IdTipoIdentificacion.Value }
                                                            equals new { IdTipoDocumento = J3.i_ParameterId } into J3_join
                            from J3 in J3_join.DefaultIfEmpty()

                            where A.i_Eliminado == 0 && J3.i_GroupId == 150
                            select new agenciatransporteDto

                            {
                                
                                v_IdAgenciaTransporte = A.v_IdAgenciaTransporte,
                                v_NombreRazonSocial = A.v_NombreRazonSocial,
                                v_NumeroDocumento = A.v_NumeroDocumento,
                                v_Direccion = A.v_Direccion,
                                
                            };

                if (!string.IsNullOrEmpty(pstrFilterExpression))
                {
                    query = query.Where(pstrFilterExpression);
                }
                if (!string.IsNullOrEmpty(pstrSortExpression))
                {
                    query = query.OrderBy("v_NombreRazonSocial");
                }

                 var query2 = query.AsEnumerable()
                   .Select(x => new KeyValueDTO
                   {
                       Id = x.v_IdAgenciaTransporte ,
                       Value1 = x.v_NombreRazonSocial,
                       Value2 = x.v_NumeroDocumento,
                       Value3 =x.v_Direccion ,
                   }).ToList();


                //List<transportistaunidadtransporteDto> objData = query.ToList();
                //pobjOperationResult.Success = 1;
                return query2;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }


        public List<KeyValueDTO> ObtenerListadoAgenciaTransporteParaComboImportacion(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression)
        {


            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = from A in dbContext.agenciatransporte
                            join J1 in dbContext.systemuser on new { i_InsertUserId = A.i_InsertaIdUsuario.Value }
                                                            equals new { i_InsertUserId = J1.i_SystemUserId } into J1_join
                            from J1 in J1_join.DefaultIfEmpty()

                            join J2 in dbContext.systemuser on new { i_UpdateUserId = A.i_ActualizaIdUsuario.Value }
                                                            equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                            from J2 in J2_join.DefaultIfEmpty()

                            join J3 in dbContext.systemparameter on new { IdTipoDocumento = A.i_IdTipoIdentificacion.Value }
                                                            equals new { IdTipoDocumento = J3.i_ParameterId } into J3_join
                            from J3 in J3_join.DefaultIfEmpty()

                            where A.i_Eliminado == 0 && J3.i_GroupId == 150
                            select new agenciatransporteDto

                            {

                                v_IdAgenciaTransporte = A.v_IdAgenciaTransporte,
                                v_NombreRazonSocial = A.v_NombreRazonSocial,
                                v_NumeroDocumento = A.v_NumeroDocumento,
                                v_Direccion = A.v_Direccion,

                            };

                if (!string.IsNullOrEmpty(pstrFilterExpression))
                {
                    query = query.Where(pstrFilterExpression);
                }
                if (!string.IsNullOrEmpty(pstrSortExpression))
                {
                    query = query.OrderBy("v_NombreRazonSocial");
                }

                var query2 = query.AsEnumerable()
                  .Select(x => new KeyValueDTO
                  {
                      Id = x.v_IdAgenciaTransporte,
                      Value1 = x.v_NombreRazonSocial,
                      Value2 = x.v_NumeroDocumento,
                      Value3 = x.v_Direccion,
                  }).ToList();


                //List<transportistaunidadtransporteDto> objData = query.ToList();
                //pobjOperationResult.Success = 1;
                return query2;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }




    }
}
