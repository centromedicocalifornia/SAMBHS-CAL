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
    public class MarcaBL
    {
        public marcaDto ObtenerMarca(ref OperationResult pobjOperationResult, string pstrMarcaId)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    marcaDto objDtoEntity = null;

                    var objEntity = (from a in dbContext.marca
                        where a.v_IdMarca == pstrMarcaId
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

        public List<marcaDto> ListarMarca(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression)
        {

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    var query = (from n in dbContext.marca

                        join J1 in dbContext.systemuser on new {i_InsertUserId = n.i_InsertaIdUsuario.Value}
                        equals new {i_InsertUserId = J1.i_SystemUserId} into J1_join
                        from J1 in J1_join.DefaultIfEmpty()

                        join J2 in dbContext.systemuser on new {i_UpdateUserId = n.i_ActualizaIdUsuario.Value}
                        equals new {i_UpdateUserId = J2.i_SystemUserId} into J2_join
                        from J2 in J2_join.DefaultIfEmpty()
                        where n.i_Eliminado == 0

                        select new marcaDto
                        {
                            v_IdMarca = n.v_IdMarca,
                            v_CodMarca = n.v_CodMarca,
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

                    List<marcaDto> objData = query.ToList();
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

        public void InsertarMarca(ref OperationResult pobjOperationResult, marcaDto pobjDtoEntity, List<string> ClientSession)
        {
            int SecuentialId = 0;
            string newId = string.Empty;

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    SecuentialBL objSecuentialBL = new SecuentialBL();
                    marca objEntity = marcaAssembler.ToEntity(pobjDtoEntity);

                    objEntity.t_InsertaFecha = DateTime.Now;
                    objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntity.i_Eliminado = 0;


                    // Autogeneramos el Pk de la tabla
                    int intNodeId = int.Parse(ClientSession[0]);
                    SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 22);
                    newId = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "MC");
                    objEntity.v_IdMarca = newId;

                    dbContext.AddTomarca(objEntity);
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

        public void ActualizarMarca(ref OperationResult pobjOperationResult, marcaDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.marca
                        where a.v_IdMarca == pobjDtoEntity.v_IdMarca
                        select a).FirstOrDefault();

                    // Crear la entidad con los datos actualizados
                    pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                    pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);


                    marca objEntity = marcaAssembler.ToEntity(pobjDtoEntity);
                    dbContext.marca.ApplyCurrentValues(objEntity);

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

        public void EliminarMarca(ref OperationResult pobjOperationResult, string pstrMarcaId, List<string> ClientSession)
        {
            //mon.IsActive = true;
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.marca
                        where a.v_IdMarca == pstrMarcaId
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
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return;
            }
        }


        public marcaDto ObtenerMarcasPorNombre(ref  OperationResult pobjOperationResult, string NombreMarca,string CodigoMarca)
        {

            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                marcaDto Marca = new marcaDto();
                if (!string.IsNullOrEmpty(NombreMarca))
                {
                     Marca = (from a in dbContext.marca
                                 where a.v_Nombre.Trim() == NombreMarca.Trim ()

                                 select a).FirstOrDefault().ToDTO();
                }
                return Marca;
            
            }


        }

        public marcaDto ObtenerMarcasPorCodigo(ref  OperationResult pobjOperationResult, string NombreMarca, string CodigoMarca)
        {

            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                marcaDto Marca = new marcaDto();
                if (!string.IsNullOrEmpty(CodigoMarca))
                {

                    Marca = (from a in dbContext.marca
                             where a.v_CodMarca == CodigoMarca.Trim()

                             select a).FirstOrDefault().ToDTO();
                }
                return Marca;

            }


        }



        public marcaDto ObtenerMarcasPorNombreEditado(ref  OperationResult pobjOperationResult, string NombreMarca, string CodigoMarca,string IdMarca)
        {

            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                marcaDto Marca = new marcaDto();
                if (!string.IsNullOrEmpty(NombreMarca))
                {
                    Marca = (from a in dbContext.marca
                             where a.v_Nombre.Trim() == NombreMarca.Trim() && a.v_IdMarca != IdMarca

                             select a).FirstOrDefault().ToDTO();
                }
                return Marca;

            }


        }

        public marcaDto ObtenerMarcasPorCodigoEditado(ref  OperationResult pobjOperationResult, string NombreMarca, string CodigoMarca,string IdMarca)
        {

            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                marcaDto Marca = new marcaDto();
                if (!string.IsNullOrEmpty(CodigoMarca))
                {

                    Marca = (from a in dbContext.marca
                             where a.v_CodMarca == CodigoMarca.Trim() && a.v_IdMarca !=IdMarca

                             select a).FirstOrDefault().ToDTO();
                }
                return Marca;

            }


        }
       
        
        
        
        
        
        
        
        #region KeyValueDto

        public List<KeyValueDTO> LlenarComboMarca(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrIdLinea)
        {
            //mon.IsActive = true;

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    //var query = from a in dbContext.marca
                    //    where a.i_Eliminado == 0 && (a.v_IdLinea == pstrIdLinea || pstrIdLinea == "-1")
                    //    select a;

                    var query = from a in dbContext.marca
                                where a.i_Eliminado == 0 
                                select a;



                    query = query.OrderBy(pstrSortExpression);

                    var query2 = query.AsEnumerable()
                        .Select(x => new KeyValueDTO
                        {
                            Id = x.v_IdMarca.ToString(),
                            Value1 = x.v_Nombre,
                            Value2 = x.v_CodMarca
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




        public List<KeyValueDTO> LlenarComboModelo(ref OperationResult pobjOperationResult)
        {
            //mon.IsActive = true;

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    //var query = from a in dbContext.marca
                    //    where a.i_Eliminado == 0 && (a.v_IdLinea == pstrIdLinea || pstrIdLinea == "-1")
                    //    select a;

                    var query = (from a in dbContext.producto
                                 where a.i_Eliminado == 0 && a.v_Modelo != null && a.v_Modelo.Trim ()!=""
                                 select a).ToList().Select(o => o.v_Modelo).Distinct();

                    var query2 = query.AsEnumerable().Select(x => new KeyValueDTO
                        {
                            Id = x,
                            Value1 = x,
                            Value2 = "0",
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
