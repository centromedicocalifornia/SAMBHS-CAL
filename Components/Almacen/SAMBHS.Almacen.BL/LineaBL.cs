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
    public class LineaBL
    {
        private string periodo = Globals.ClientSession.i_Periodo.Value.ToString();
        public lineaDto ObtenerLinea(ref OperationResult pobjOperationResult, string pstrLineaId)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    lineaDto objDtoEntity = null;

                    var objEntity = (from a in dbContext.linea
                        where a.v_IdLinea == pstrLineaId
                        select a).FirstOrDefault();

                    if (objEntity != null)
                        objDtoEntity = lineaAssembler.ToDTO(objEntity);

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

        public List<lineaDto> ListarLinea(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression)
        {

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from n in dbContext.linea

                        join J1 in dbContext.systemuser on new {i_InsertUserId = n.i_InsertaIdUsuario.Value}
                        equals new {i_InsertUserId = J1.i_SystemUserId} into J1_join
                        from J1 in J1_join.DefaultIfEmpty()

                        join J2 in dbContext.systemuser on new {i_UpdateUserId = n.i_ActualizaIdUsuario.Value}
                        equals new {i_UpdateUserId = J2.i_SystemUserId} into J2_join
                        from J2 in J2_join.DefaultIfEmpty()
                        where n.i_Eliminado == 0 /*&& n.v_Periodo == periodo*/

                        select new lineaDto
                        {
                            v_IdLinea = n.v_IdLinea,
                            v_CodLinea = n.v_CodLinea,
                            v_Nombre = n.v_Nombre,
                            t_InsertaFecha = n.t_InsertaFecha,
                            t_ActualizaFecha = n.t_ActualizaFecha,
                            v_UsuarioCreacion = J1.v_UserName,
                            v_UsuarioModificacion = J2.v_UserName,
                            v_NroCuentaCompra = n.v_NroCuentaCompra,
                            v_NroCuentaDConsumo = n.v_NroCuentaDConsumo,
                            v_NroCuentaHConsumo = n.v_NroCuentaHConsumo,
                            v_NroCuentaVenta = n.v_NroCuentaVenta,
                            i_Header = n.i_Header,
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

                    List<lineaDto> objData = query.ToList();
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

        public void InsertarLinea(ref OperationResult pobjOperationResult, lineaDto pobjDtoEntity, List<string> ClientSession)
        {
            string newId = string.Empty;

            try
            {

                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var objSecuentialBl = new SecuentialBL();
                    var objEntity = pobjDtoEntity.ToEntity();

                    objEntity.t_InsertaFecha = DateTime.Now;
                    objEntity.i_InsertaIdUsuario = int.Parse(ClientSession[2]);
                    objEntity.v_Periodo = Globals.ClientSession.i_Periodo.ToString();
                    objEntity.i_Eliminado = 0;

                    var intNodeId = int.Parse(ClientSession[0]);
                    var secuentialId = objSecuentialBl.GetNextSecuentialId(intNodeId, 21);
                    newId = Utils.GetNewId(int.Parse(ClientSession[0]), secuentialId, "LN");
                    objEntity.v_IdLinea = newId;

                    dbContext.AddTolinea(objEntity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1; 
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                pobjOperationResult.AdditionalInformation = "LineaBL.InsertarLinea()";
            }
        }

        public void ActualizarLinea(ref OperationResult pobjOperationResult, lineaDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.linea
                        where a.v_IdLinea == pobjDtoEntity.v_IdLinea
                        select a).FirstOrDefault();

                    // Crear la entidad con los datos actualizados
                    pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                    pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);


                    linea objEntity = pobjDtoEntity.ToEntity();
                    dbContext.linea.ApplyCurrentValues(objEntity);

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

        public void EliminarLinea(ref OperationResult pobjOperationResult, string pstrLineaId, List<string> ClientSession)
        {
            //mon.IsActive = true;
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.linea
                        where a.v_IdLinea == pstrLineaId
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
            }
        }

        public void MigrarLineas(ref OperationResult pobjOperationResult, IEnumerable<lineaDto> pLineasDtos)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        dbContext.linea.ToList().ForEach(o => dbContext.linea.DeleteObject(o));
                        dbContext.SaveChanges();

                        foreach (var lineaDto in pLineasDtos)
                        {
                            InsertarLinea(ref pobjOperationResult, lineaDto, Globals.ClientSession.GetAsList());
                            if (pobjOperationResult.Success == 0) return;
                        }

                        pobjOperationResult.Success = 1;
                        ts.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                pobjOperationResult.AdditionalInformation = "LineaBL.MigrarLineas()";
            }
        }

        #region KeyValueDto

        public List<KeyValueDTO> LlenarComboLinea(ref OperationResult pobjOperationResult, string pstrSortExpression)
        {
            //mon.IsActive = true;

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = from a in dbContext.linea
                        where a.i_Eliminado == 0
                        select a;

                    if (!string.IsNullOrEmpty(pstrSortExpression))
                    {
                        query = query.OrderBy(pstrSortExpression);
                    }

                    var query2 = query.AsEnumerable()
                        .Select(x => new KeyValueDTO
                        {
                            Id = x.v_IdLinea.ToString(),
                            Value1 = x.v_Nombre,
                            Value2 = x.v_CodLinea
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

        public List<GridKeyValueDTO> ObtenLineasParaComboGrid(ref OperationResult pobjOperationResult)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from a in dbContext.linea
                        where a.i_Eliminado == 0
                        select a);

                    query = query.OrderBy("v_IdLinea");

                    var query2 = query.AsEnumerable()
                        .Select(x => new GridKeyValueDTO
                        {
                            Id = x.v_IdLinea.ToString(),
                            Value1 = x.v_Nombre
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
        #region Reporte
        public List<ReporteLinea> ReporteLinea(string pstrt_Orden)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region Query

                    var query =
                    (from A in dbContext.linea

                        where A.i_Eliminado == 0
                        select new ReporteLinea
                        {
                            IdLinea = A.v_CodLinea,
                            NombreLinea = A.v_Nombre,

                        });

                    #endregion

                    //query = query.OrderBy(pstrt_Orden);


                    return query.ToList();

                }
                //List<ventaDto> objData = query.ToList();

                //pobjOperationResult.Success = 1;

            }
            catch (Exception ex)
            {
                //pobjOperationResult.Success = 0;
                //pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        #endregion

    }
}
