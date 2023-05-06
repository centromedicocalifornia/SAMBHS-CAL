using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using SAMBHS.CommonWIN.BL;

namespace SAMBHS.Almacen.BL
{
    public class TallaBL
    {
        public tallaDto ObtenerTalla(ref OperationResult pobjOperationResult, string pstrTallaId)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    tallaDto objDtoEntity = null;

                    var objEntity = (from a in dbContext.talla
                                     where a.v_IdTalla == pstrTallaId
                                     select a).FirstOrDefault();

                    if (objEntity != null)
                        objDtoEntity = tallaAssembler.ToDTO(objEntity);

                    pobjOperationResult.Success = 1;

                    return objDtoEntity; 
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public tallaDto ObtenerTallaByCode(ref OperationResult pobjOperationResult, string pstrCode)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var objEntity = (from a in dbContext.talla
                        where a.v_CodTalla == pstrCode && a.i_Eliminado == 0
                        select new tallaDto
                        {
                            v_IdTalla = a.v_IdTalla,
                            v_Nombre = a.v_Nombre,
                            v_CodTalla = a.v_CodTalla
                        }).FirstOrDefault();

                    pobjOperationResult.Success = 1;

                    return objEntity;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public List<tallaDto> ListarTalla(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression)
        {

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from n in dbContext.talla

                                 join J1 in dbContext.systemuser on new { i_InsertUserId = n.i_InsertaIdUsuario.Value }
                                                               equals new { i_InsertUserId = J1.i_SystemUserId } into J1_join
                                 from J1 in J1_join.DefaultIfEmpty()

                                 join J2 in dbContext.systemuser on new { i_UpdateUserId = n.i_ActualizaIdUsuario.Value }
                                                               equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                                 from J2 in J2_join.DefaultIfEmpty()
                                 where n.i_Eliminado == 0

                                 select new tallaDto
                                 {
                                     v_IdTalla = n.v_IdTalla,
                                     v_CodTalla = n.v_CodTalla,
                                     v_Nombre = n.v_Nombre,
                                     t_InsertaFecha = n.t_InsertaFecha,
                                     t_ActualizaFecha = n.t_ActualizaFecha,
                                     v_UsuarioCreacion = J1.v_UserName,
                                     v_UsuarioModificacion = J2.v_UserName
                                 }
                                        );



                    if (!string.IsNullOrEmpty(pstrFilterExpression))
                    {
                        query = query.Where(pstrFilterExpression);
                    }
                    if (!string.IsNullOrEmpty(pstrSortExpression))
                    {
                        query = query.OrderBy(pstrSortExpression);
                    }

                    List<tallaDto> objData = query.ToList();
                    pobjOperationResult.Success = 1;
                    return objData; 
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public void InsertarTalla(ref OperationResult pobjOperationResult, tallaDto pobjDtoEntity, List<string> ClientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var objEntity = pobjDtoEntity.ToEntity();

                    objEntity.t_InsertaFecha = DateTime.Now;
                    objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntity.i_Eliminado = 0;


                    // Autogeneramos el Pk de la tabla
                    int intNodeId = int.Parse(ClientSession[0]);
                    var SecuentialId = new SecuentialBL().GetNextSecuentialId(intNodeId, 20);
                    var newId = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "TL");
                    objEntity.v_IdTalla = newId;

                    dbContext.AddTotalla(objEntity);
                    dbContext.SaveChanges();

                    pobjOperationResult.Success = 1; 
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return;
            }
        }

        public void ActualizarTalla(ref OperationResult pobjOperationResult, tallaDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.talla
                                           where a.v_IdTalla == pobjDtoEntity.v_IdTalla
                                           select a).FirstOrDefault();

                    // Crear la entidad con los datos actualizados
                    pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                    pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);


                    talla objEntity = tallaAssembler.ToEntity(pobjDtoEntity);
                    dbContext.talla.ApplyCurrentValues(objEntity);

                    // Guardar los cambios
                    dbContext.SaveChanges();

                    pobjOperationResult.Success = 1; 
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return;
            }
        }

        public void EliminarTalla(ref OperationResult pobjOperationResult, string pstrTallaId, List<string> ClientSession)
        {
            //mon.IsActive = true;
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.talla
                                           where a.v_IdTalla == pstrTallaId
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
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
            }
        }

        #region KeyValueDto

        public List<KeyValueDTO> LlenarComboTalla(ref OperationResult pobjOperationResult, string pstrSortExpression)
        {
            //mon.IsActive = true;

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = from a in dbContext.talla
                                where a.i_Eliminado == 0
                                select a;

                    query = query.OrderBy(pstrSortExpression);

                    var query2 = query.AsEnumerable()
                                .Select(x => new KeyValueDTO
                                {
                                    Id = x.v_IdTalla.ToString(),
                                    Value1 = x.v_Nombre,
                                    Value2 = x.v_CodTalla
                                }).ToList();

                    pobjOperationResult.Success = 1;
                    return query2; 
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        #endregion
    }
}
