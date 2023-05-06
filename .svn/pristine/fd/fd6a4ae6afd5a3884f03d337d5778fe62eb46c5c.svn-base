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

namespace SAMBHS.Almacen.BL
{
    public class ColorBL
    {
        public colorDto ObtenerColor(ref OperationResult pobjOperationResult, string pstrColorId)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    colorDto objDtoEntity = null;

                    var objEntity = (from a in dbContext.color
                        where a.v_IdColor == pstrColorId
                        select a).FirstOrDefault();

                    if (objEntity != null)
                        objDtoEntity = objEntity.ToDTO();

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

        public List<colorDto> ListarColor(ref OperationResult pobjOperationResult,string pstrSortExpression, string pstrFilterExpression)
        {

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from n in dbContext.color

                        join J1 in dbContext.systemuser on new {i_InsertUserId = n.i_InsertaIdUsuario.Value}
                        equals new {i_InsertUserId = J1.i_SystemUserId} into J1_join
                        from J1 in J1_join.DefaultIfEmpty()

                        join J2 in dbContext.systemuser on new {i_UpdateUserId = n.i_ActualizaIdUsuario.Value}
                        equals new {i_UpdateUserId = J2.i_SystemUserId} into J2_join
                        from J2 in J2_join.DefaultIfEmpty()
                        where n.i_Eliminado == 0

                        select new colorDto
                        {
                            v_IdColor = n.v_IdColor,
                            v_CodColor = n.v_CodColor,
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

                    List<colorDto> objData = query.ToList();
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
          
        public void InsertarColor(ref OperationResult pobjOperationResult, colorDto pobjDtoEntity, List<string> ClientSession)
        {
            string newId = string.Empty;

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    SecuentialBL objSecuentialBL = new SecuentialBL();
                    color objEntity = pobjDtoEntity.ToEntity();

                    objEntity.t_InsertaFecha = DateTime.Now;
                    objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntity.i_Eliminado = 0;


                    // Autogeneramos el Pk de la tabla
                    var intNodeId = int.Parse(ClientSession[0]);
                    var SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 19);
                    newId = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "CO");
                    objEntity.v_IdColor = newId;

                    dbContext.AddTocolor(objEntity);
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

        public void ActualizarColor(ref OperationResult pobjOperationResult, colorDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.color
                        where a.v_IdColor == pobjDtoEntity.v_IdColor
                        select a).FirstOrDefault();

                    // Crear la entidad con los datos actualizados
                    pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                    pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);


                    color objEntity = colorAssembler.ToEntity(pobjDtoEntity);
                    dbContext.color.ApplyCurrentValues(objEntity);

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

        public void EliminarColor(ref OperationResult pobjOperationResult, string pstrColorId, List<string> ClientSession)
        {
            //mon.IsActive = true;
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.color
                        where a.v_IdColor == pstrColorId
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

        public List<KeyValueDTO> LlenarComboColor(ref OperationResult pobjOperationResult, string pstrSortExpression)
        {
            //mon.IsActive = true;

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = from a in dbContext.color
                        where a.i_Eliminado == 0
                        select a;

                    if (!string.IsNullOrEmpty(pstrSortExpression))
                    {
                        query = query.OrderBy(pstrSortExpression);
                    }
                    else
                    {
                        query = query.OrderBy(pstrSortExpression);
                    }

                    var query2 = query.AsEnumerable()
                        .Select(x => new KeyValueDTO
                        {
                            Id = x.v_IdColor.ToString(),
                            Value1 = x.v_Nombre,
                            Value2 = x.v_CodColor,
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
