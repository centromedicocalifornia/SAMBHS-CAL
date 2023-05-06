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
    public class ModeloBL
    {
        public modeloDto ObtenerModelo(ref OperationResult pobjOperationResult,string pstrMarcaId , string pstrModeloId)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var objEntity = (from a in dbContext.modelo
                                 join b in dbContext.marca on a.v_IdMarca equals b.v_IdMarca
                                 join J1 in dbContext.systemuser on new { i_InsertUserId = a.i_InsertaIdUsuario.Value }
                                                equals new { i_InsertUserId = J1.i_SystemUserId } into J1_join
                                 from J1 in J1_join.DefaultIfEmpty()

                                 join J2 in dbContext.systemuser on new { i_UpdateUserId = a.i_ActualizaIdUsuario.Value }
                                                               equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                                 from J2 in J2_join.DefaultIfEmpty()
                                 where a.v_IdModelo == pstrModeloId && a.v_IdMarca == pstrMarcaId
                                 select new modeloDto
                                 {
                                     v_IdModelo = a.v_IdModelo,
                                     v_Nombre = a.v_Nombre,
                                     v_CodModelo = a.v_CodModelo,
                                     v_IdLinea = b.v_IdLinea,
                                     v_IdMarca = b.v_IdMarca,
                                     i_Eliminado = a.i_Eliminado,
                                     i_ActualizaIdUsuario = a.i_ActualizaIdUsuario,
                                     t_InsertaFecha = a.t_InsertaFecha,
                                     t_ActualizaFecha = a.t_ActualizaFecha,
                                     v_UsuarioCreacion = J1.v_UserName,
                                     v_UsuarioModificacion = J2.v_UserName
                                 }
                                 ).FirstOrDefault();

                pobjOperationResult.Success = 1;

                return objEntity;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public List<modeloDto> ListarModelo(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression)
        {

            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = (from n in dbContext.modelo
                             join b in dbContext.marca on n.v_IdMarca equals b.v_IdMarca
                             join c in dbContext.linea on b.v_IdLinea equals c.v_IdLinea
                             join J1 in dbContext.systemuser on new { i_InsertUserId = n.i_InsertaIdUsuario.Value }
                                                           equals new { i_InsertUserId = J1.i_SystemUserId } into J1_join
                             from J1 in J1_join.DefaultIfEmpty()

                             join J2 in dbContext.systemuser on new { i_UpdateUserId = n.i_ActualizaIdUsuario.Value }
                                                           equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                             from J2 in J2_join.DefaultIfEmpty()
                             where n.i_Eliminado == 0

                             select new modeloDto
                             {
                                 v_IdModelo = n.v_IdModelo,
                                 LineaNombre = c.v_Nombre,
                                 MarcaNombre = b.v_Nombre,
                                 v_IdMarca = n.v_IdMarca,
                                 v_CodModelo = n.v_CodModelo,
                                 v_Nombre = n.v_Nombre,
                                 i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
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

                List<modeloDto> objData = query.ToList();
                pobjOperationResult.Success = 1;
                return objData;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public void InsertarModelo(ref OperationResult pobjOperationResult, modeloDto pobjDtoEntity, List<string> ClientSession)
        {
            int SecuentialId = 0;
            string newId = string.Empty;

            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                SecuentialBL objSecuentialBL = new SecuentialBL();
                modelo objEntity = modeloAssembler.ToEntity(pobjDtoEntity);

                objEntity.t_InsertaFecha = DateTime.Now;
                objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                objEntity.i_Eliminado = 0;


                // Autogeneramos el Pk de la tabla
                int intNodeId = int.Parse(ClientSession[0]);
                SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 23);
                newId = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "MD");
                objEntity.v_IdModelo = newId;

                dbContext.AddTomodelo(objEntity);
                dbContext.SaveChanges();

                pobjOperationResult.Success = 1;
                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return;
            }
        }

        public void ActualizarModelo(ref OperationResult pobjOperationResult, modeloDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.modelo
                                       where a.v_IdModelo == pobjDtoEntity.v_IdModelo
                                       select a).FirstOrDefault();

                // Crear la entidad con los datos actualizados
                pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);


                modelo objEntity = modeloAssembler.ToEntity(pobjDtoEntity);
                dbContext.modelo.ApplyCurrentValues(objEntity);

                // Guardar los cambios
                dbContext.SaveChanges();

                pobjOperationResult.Success = 1;
                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return;
            }
        }

        public void EliminarModelo(ref OperationResult pobjOperationResult, string pstrModeloId, List<string> ClientSession)
        {
            //mon.IsActive = true;
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.modelo
                                       where a.v_IdModelo == pstrModeloId
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
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return;
            }
        }

        #region KeyValueDto

        public List<KeyValueDTO> LlenarComboModelo(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrIdMarca)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = from a in dbContext.modelo
                            where a.i_Eliminado == 0 && (a.v_IdMarca == pstrIdMarca ||  pstrIdMarca=="-1")
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
                                Id = x.v_IdModelo.ToString(),
                                Value1 = x.v_Nombre
                            }).ToList();

                pobjOperationResult.Success = 1;
                return query2;
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
