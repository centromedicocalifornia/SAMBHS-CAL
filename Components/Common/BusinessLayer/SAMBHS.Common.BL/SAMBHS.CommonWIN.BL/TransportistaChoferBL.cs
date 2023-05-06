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
    public class TransportistaChoferBL
    {
        public List<transportistachoferDto> ObtenerListadoTransportistaChofer(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression, string pstrIdTransportista)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = from A in dbContext.transportistachofer
                            join J1 in dbContext.systemuser on new { i_InsertUserId = A.i_InsertaIdUsuario.Value }
                                                            equals new { i_InsertUserId = J1.i_SystemUserId } into J1_join
                            from J1 in J1_join.DefaultIfEmpty()

                            join J2 in dbContext.systemuser on new { i_UpdateUserId = A.i_ActualizaIdUsuario.Value }
                                                            equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                            from J2 in J2_join.DefaultIfEmpty()
                            where A.i_Eliminado == 0 && A.v_IdTransportista == pstrIdTransportista
                            select new transportistachoferDto
                            {
                                v_IdChofer = A.v_IdChofer,
                                v_NombreCompleto = A.v_NombreCompleto,
                                v_Brevete = A.v_Brevete,
                                i_InsertaIdUsuario = A.i_InsertaIdUsuario,
                                t_InsertaFecha = A.t_InsertaFecha,
                                i_ActualizaIdUsuario = A.i_ActualizaIdUsuario,
                                t_ActualizaFecha = A.t_ActualizaFecha,
                                v_UsuarioCreacion = J1.v_UserName,
                                v_UsuarioModificacion = J2.v_UserName
                            };

                if (!string.IsNullOrEmpty(pstrFilterExpression))
                {
                    query = query.Where(pstrFilterExpression);
                }
                if (!string.IsNullOrEmpty(pstrSortExpression))
                {
                    query = query.OrderBy(pstrSortExpression);
                }

                List<transportistachoferDto> objData = query.ToList();
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

        public transportistachoferDto ObtenerTransportistaChofer(ref OperationResult pobjOperationResult, string pstringIdtransportistachofer)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                transportistachoferDto objDtoEntity = null;

                var objEntity = (from a in dbContext.transportistachofer
                                 where a.v_IdChofer == pstringIdtransportistachofer
                                 select a).FirstOrDefault();

                if (objEntity != null)
                    objDtoEntity = transportistachoferAssembler.ToDTO(objEntity);

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

        public void InsertarTransportistaChofer(ref OperationResult pobjOperationResult, transportistachoferDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;
            int SecuentialId = 0;
            string newId = string.Empty;
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                SecuentialBL objSecuentialBL = new SecuentialBL();
                transportistachofer objEntity = transportistachoferAssembler.ToEntity(pobjDtoEntity);


                objEntity.t_InsertaFecha = DateTime.Now;
                objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                objEntity.i_Eliminado = 0;

                // Autogeneramos el Pk de la tabla
                int intNodeId = int.Parse(ClientSession[0]);
                SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 16);
                newId = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "CF");
                objEntity.v_IdChofer = newId;


                dbContext.AddTotransportistachofer(objEntity);
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

        public void ActualizarTransportistaChofer(ref OperationResult pobjOperationResult, transportistachoferDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.transportistachofer
                                       where a.v_IdChofer == pobjDtoEntity.v_IdChofer
                                       select a).FirstOrDefault();

                // Crear la entidad con los datos actualizados
                pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);


                transportistachofer objEntity = transportistachoferAssembler.ToEntity(pobjDtoEntity);
                dbContext.transportistachofer.ApplyCurrentValues(objEntity);

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

        public void EliminarTransportistaChofer(ref OperationResult pobjOperationResult, string pstrIdtransportistachofer, List<string> ClientSession)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.transportistachofer
                                       where a.v_IdChofer == pstrIdtransportistachofer
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


        public List<KeyValueDTO> ObtenerListadoTransportistaChoferParaCombo(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression, string pstrIdTransportista)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = from A in dbContext.transportistachofer
                            join J1 in dbContext.systemuser on new { i_InsertUserId = A.i_InsertaIdUsuario.Value }
                                                            equals new { i_InsertUserId = J1.i_SystemUserId } into J1_join
                            from J1 in J1_join.DefaultIfEmpty()

                            join J2 in dbContext.systemuser on new { i_UpdateUserId = A.i_ActualizaIdUsuario.Value }
                                                            equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                            from J2 in J2_join.DefaultIfEmpty()
                            where A.i_Eliminado == 0 && A.v_IdTransportista == pstrIdTransportista
                            select new transportistachoferDto
                            {
                                v_IdChofer = A.v_IdChofer,
                                v_NombreCompleto = A.v_NombreCompleto,
                                v_Brevete = A.v_Brevete,
                               
                            };

                if (!string.IsNullOrEmpty(pstrFilterExpression))
                {
                    query = query.Where(pstrFilterExpression);
                }
                if (!string.IsNullOrEmpty(pstrSortExpression))
                {
                    query = query.OrderBy("v_IdChofer");
                }

                var query2 = query.AsEnumerable()
                   .Select(x => new KeyValueDTO
                   {
                       Id =x.v_IdChofer ,
                       Value1 =   x.v_NombreCompleto ,
                       Value2 = x.v_Brevete,
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