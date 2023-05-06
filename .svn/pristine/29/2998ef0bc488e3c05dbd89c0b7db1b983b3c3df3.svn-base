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
    public class TransportistaUnidadTransporteBL
    {
        public List<transportistaunidadtransporteDto> ObtenerListadoTransportistaUnidadTransporte(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression, string pstrIdTransportista)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = from A in dbContext.transportistaunidadtransporte
                            join J1 in dbContext.systemuser on new { i_InsertUserId = A.i_InsertaIdUsuario.Value }
                                                            equals new { i_InsertUserId = J1.i_SystemUserId } into J1_join
                            from J1 in J1_join.DefaultIfEmpty()

                            join J2 in dbContext.systemuser on new { i_UpdateUserId = A.i_ActualizaIdUsuario.Value }
                                                            equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                            from J2 in J2_join.DefaultIfEmpty()
                            where A.i_Eliminado == 0 && A.v_IdTransportista == pstrIdTransportista
                            select new transportistaunidadtransporteDto
                            {
                                v_IdUnidadTransporte = A.v_IdUnidadTransporte,
                                v_TractoPlaca = A.v_TractoPlaca,
                                v_TractoMarca = A.v_TractoMarca,
                                v_TractoCertificado = A.v_TractoCertificado,
                                v_CarretaPlaca = A.v_CarretaPlaca,
                                v_CarretaMarca = A.v_CarretaMarca,
                                v_CarretaCertificado = A.v_CarretaCertificado,
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

                List<transportistaunidadtransporteDto> objData = query.ToList();
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

        public transportistaunidadtransporteDto ObtenerTransportistaUnidadTransporte(ref OperationResult pobjOperationResult, string pstringIdTransportistaUnidadTransporte)
        {

            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                transportistaunidadtransporteDto objDtoEntity = null;

                var objEntity = (from a in dbContext.transportistaunidadtransporte
                                 where a.v_IdUnidadTransporte == pstringIdTransportistaUnidadTransporte
                                 select a).FirstOrDefault();

                if (objEntity != null)
                    objDtoEntity = transportistaunidadtransporteAssembler.ToDTO(objEntity);

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

        public void InsertarTransportistaUnidadTransporte(ref OperationResult pobjOperationResult, transportistaunidadtransporteDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;
            int SecuentialId = 0;
            string newId = string.Empty;
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                SecuentialBL objSecuentialBL = new SecuentialBL();
                transportistaunidadtransporte objEntity = transportistaunidadtransporteAssembler.ToEntity(pobjDtoEntity);


                objEntity.t_InsertaFecha = DateTime.Now;
                objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                objEntity.i_Eliminado = 0;

                // Autogeneramos el Pk de la tabla
                int intNodeId = int.Parse(ClientSession[0]);
                SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 17);
                newId = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "UN");
                objEntity.v_IdUnidadTransporte = newId;


                dbContext.AddTotransportistaunidadtransporte(objEntity);
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

        public void ActualizarTransportistaUnidadTransporte(ref OperationResult pobjOperationResult, transportistaunidadtransporteDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.transportistaunidadtransporte
                                       where a.v_IdUnidadTransporte == pobjDtoEntity.v_IdUnidadTransporte
                                       select a).FirstOrDefault();

                // Crear la entidad con los datos actualizados
                pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);


                transportistaunidadtransporte objEntity = transportistaunidadtransporteAssembler.ToEntity(pobjDtoEntity);
                dbContext.transportistaunidadtransporte.ApplyCurrentValues(objEntity);

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

        public void EliminarTransportistaUnidadTransporte(ref OperationResult pobjOperationResult, string pstrIdTransportistaUnidadTransporte, List<string> ClientSession)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.transportistaunidadtransporte
                                       where a.v_IdUnidadTransporte == pstrIdTransportistaUnidadTransporte
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


        public List<KeyValueDTO> ObtenerListadoTransportistaUnidadTransporteParaCombo(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression, string pstrIdTransportista)
        {
            

            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = ( from A in dbContext.transportistaunidadtransporte
                            join J1 in dbContext.systemuser on new { i_InsertUserId = A.i_InsertaIdUsuario.Value }
                                                            equals new { i_InsertUserId = J1.i_SystemUserId } into J1_join
                            from J1 in J1_join.DefaultIfEmpty()

                            join J2 in dbContext.systemuser on new { i_UpdateUserId = A.i_ActualizaIdUsuario.Value }
                                                            equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                            from J2 in J2_join.DefaultIfEmpty()
                            where A.i_Eliminado == 0 && A.v_IdTransportista == pstrIdTransportista
                            select new
                            {
                               A.v_IdUnidadTransporte,
                               A.v_TractoPlaca,
                               A.v_TractoMarca,
                               A.v_TractoCertificado

                            });
                          

                if (!string.IsNullOrEmpty(pstrFilterExpression))
                {
                    query = query.OrderBy (pstrSortExpression);
                }
                if (!string.IsNullOrEmpty(pstrSortExpression))
                {
                    query = query.OrderBy("v_IdTransportista");
                }
                var query2 = query.AsEnumerable()
                    .Select (x => new KeyValueDTO
                            {
                                Id = x.v_IdUnidadTransporte,
                                Value1 = x.v_TractoPlaca ,
                                Value2 = x.v_TractoMarca,
                                Value3 = x.v_TractoCertificado,
                            }).ToList();
                

                //List<transportistaunidadtransporteDto> objData = query.ToList();
                //pobjOperationResult.Success = 1;
                return query2 ;
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