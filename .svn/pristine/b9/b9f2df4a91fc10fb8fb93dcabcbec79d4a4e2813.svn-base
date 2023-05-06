using SAMBHS.Venta.BL;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic;
using System.Threading.Tasks;


namespace SAMBHS.Venta.BL
{
    public class CondicionPagoBL
    {

        public List<condicionpagoDto> ObtenerListadoCondicionPago(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression)
        {

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = from A in dbContext.condicionpago
                        join J1 in dbContext.systemuser on new {i_InsertUserId = A.i_InsertaIdUsuario.Value}
                        equals new {i_InsertUserId = J1.i_SystemUserId} into J1_join
                        from J1 in J1_join.DefaultIfEmpty()

                        join J2 in dbContext.systemuser on new {i_UpdateUserId = A.i_ActualizaIdUsuario.Value}
                        equals new {i_UpdateUserId = J2.i_SystemUserId} into J2_join
                        from J2 in J2_join.DefaultIfEmpty()
                        where (A.i_Eliminado == 0 || A.i_Eliminado == null)

                        select new condicionpagoDto
                        {
                            v_NombreCondicion = A.v_NombreCondicion,
                            i_CreditoLetras = A.i_CreditoLetras,
                            i_Dias = A.i_Dias,
                            i_FlagCompraVenta = A.i_FlagCompraVenta,
                            i_Eliminado = A.i_Eliminado,
                            i_InsertaIdUsuario = A.i_InsertaIdUsuario,
                            t_InsertaFecha = A.t_InsertaFecha,
                            i_ActualizaIdUsuario = A.i_ActualizaIdUsuario,
                            t_ActualizaFecha = A.t_ActualizaFecha,
                            v_UsuarioCreacion = J1.v_UserName,
                            v_UsuarioModificacion = J2.v_UserName,
                            b_CreditoLetras = A.i_CreditoLetras == 0 ? false : true,
                            v_IdCondicionPago = A.v_IdCondicionPago
                        };

                    if (!string.IsNullOrEmpty(pstrFilterExpression))
                    {
                        query = query.Where(pstrFilterExpression);
                    }
                    if (!string.IsNullOrEmpty(pstrSortExpression))
                    {
                        query = query.OrderBy(pstrSortExpression);
                    }

                    List<condicionpagoDto> objData = query.ToList();
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

        public condicionpagoDto ObtenerCondicionPago(ref OperationResult pobjOperationResult, string pstrIdCondicionPago)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    condicionpagoDto objDtoEntity = null;

                    var objEntity = (from a in dbContext.condicionpago
                        where a.v_IdCondicionPago == pstrIdCondicionPago
                        select a).FirstOrDefault();
                    if (objEntity != null)
                        objDtoEntity = condicionpagoAssembler.ToDTO(objEntity);

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

        public void InsertarCondicionPago(ref OperationResult pobjOperationResult, condicionpagoDto pobjDtoEntity, List<string>ClientSession)
        {
            int SecuentialId = 0;
            string newId = string.Empty;
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    SecuentialBL objSecuentialBL = new SecuentialBL();
                    condicionpago objEntity = condicionpagoAssembler.ToEntity(pobjDtoEntity);
                    objEntity.t_InsertaFecha = DateTime.Now;
                    objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntity.i_Eliminado = 0;
                    // Auntogeneramos el Pk en la tabla
                    int intNodeId = int.Parse(ClientSession[0]);
                    SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 10);
                    newId = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "CP");
                    objEntity.v_IdCondicionPago = newId;

                    dbContext.AddTocondicionpago(objEntity);
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

        public void ActualizarCondicionPago(ref OperationResult pobjOperationResult, condicionpagoDto pobjDtoEntity, List<string> ClientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    // Obtener la Entidad Fuente
                    var objEntitySource = (from a in dbContext.condicionpago
                        where a.v_IdCondicionPago == pobjDtoEntity.v_IdCondicionPago
                        select a).FirstOrDefault();

                    // Crear una entidad con los datos actualizados
                    pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                    pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                    condicionpago objEntity = condicionpagoAssembler.ToEntity(pobjDtoEntity);
                    dbContext.condicionpago.ApplyCurrentValues(objEntity);

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

        public void EliminarCondicionPago(ref OperationResult pobjOperationResult, string pstrIdCondicionPago, List<string> ClientSession )
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    // Obtener la entidad fuente
                    var ObjEntitySource = (from a in dbContext.condicionpago
                        where a.v_IdCondicionPago == pstrIdCondicionPago
                        select a).FirstOrDefault();

                    ObjEntitySource.t_ActualizaFecha = DateTime.Now;
                    ObjEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                    ObjEntitySource.i_Eliminado = 1;

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
