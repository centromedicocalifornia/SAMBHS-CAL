using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using SAMBHS.CommonWIN.BL;

namespace SAMBHS.Venta.BL
{
    public class ClienteAvalBL
    {
        public List<avalclienteDto> ObtenerListadoAvalCliente(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression, string pstrIdCliente)
        {
            //mon.IsActive = true;

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = from A in dbContext.avalcliente
                        join J1 in dbContext.systemuser on new {i_InsertUserId = A.i_InsertaIdUsuario.Value}
                        equals new {i_InsertUserId = J1.i_SystemUserId} into J1_join
                        from J1 in J1_join.DefaultIfEmpty()

                        join J2 in dbContext.systemuser on new {i_UpdateUserId = A.i_ActualizaIdUsuario.Value}
                        equals new {i_UpdateUserId = J2.i_SystemUserId} into J2_join
                        from J2 in J2_join.DefaultIfEmpty()
                        where A.i_Eliminado == 0 && A.v_IdCliente == pstrIdCliente
                        select new avalclienteDto
                        {
                            v_IdAvalCliente = A.v_IdAvalCliente,
                            v_IdCliente = A.v_IdCliente,
                            v_Nombres = A.v_Nombres,
                            v_NroDocIdentificacion = A.v_NroDocIdentificacion,
                            v_Direccion = A.v_Direccion,
                            v_Localidad = A.v_Localidad,
                            v_Telefono = A.v_Telefono,
                            t_ActualizaFecha = A.t_ActualizaFecha,
                            t_InsertaFecha = A.t_InsertaFecha,
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

                    List<avalclienteDto> objData = query.ToList();
                    pobjOperationResult.Success = 1;
                    return objData;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public avalclienteDto ObtenerAvalCliente(ref OperationResult pobjOperationResult, string pstringIdavalcliente)
        {
            //mon.IsActive = true;

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    avalclienteDto objDtoEntity = null;

                    var objEntity = (from a in dbContext.avalcliente
                        where a.v_IdAvalCliente == pstringIdavalcliente
                        select a).FirstOrDefault();

                    if (objEntity != null)
                        objDtoEntity = avalclienteAssembler.ToDTO(objEntity);

                    pobjOperationResult.Success = 1;
                    return objDtoEntity;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public void InsertarAvalCliente(ref OperationResult pobjOperationResult, avalclienteDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;
            int SecuentialId = 0;
            string newId = string.Empty;
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    SecuentialBL objSecuentialBL = new SecuentialBL();
                    avalcliente objEntity = avalclienteAssembler.ToEntity(pobjDtoEntity);


                    objEntity.t_InsertaFecha = DateTime.Now;
                    objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntity.i_Eliminado = 0;

                    // Autogeneramos el Pk de la tabla
                    int intNodeId = int.Parse(ClientSession[0]);
                    SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 18);
                    newId = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "CA");
                    objEntity.v_IdAvalCliente = newId;


                    dbContext.AddToavalcliente(objEntity);
                    dbContext.SaveChanges();

                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
            }
        }

        public void ActualizarAvalCliente(ref OperationResult pobjOperationResult, avalclienteDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.avalcliente
                        where a.v_IdAvalCliente == pobjDtoEntity.v_IdAvalCliente
                        select a).FirstOrDefault();

                    // Crear la entidad con los datos actualizados
                    pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                    pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);


                    avalcliente objEntity = avalclienteAssembler.ToEntity(pobjDtoEntity);
                    dbContext.avalcliente.ApplyCurrentValues(objEntity);

                    // Guardar los cambios
                    dbContext.SaveChanges();

                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
            }
        }

        public void EliminarAvalCliente(ref OperationResult pobjOperationResult, string pstrIdavalcliente, List<string> ClientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.avalcliente
                        where a.v_IdAvalCliente == pstrIdavalcliente
                        select a).FirstOrDefault();

                    // Crear la entidad con los datos actualizados
                    objEntitySource.t_ActualizaFecha = DateTime.Now;
                    objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntitySource.i_Eliminado = 1;

                    // Guardar los cambios
                    dbContext.SaveChanges();

                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
            }
        }
    }
}
