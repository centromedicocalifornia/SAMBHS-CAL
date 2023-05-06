using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.ComponentModel;

namespace SAMBHS.CommonWIN.BL
{
    public class DatahierarchyBL
    {
        public List<datahierarchyDto> GetDataHierarchiesPagedAndFiltered(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression, int pintGroupId)
        {
            //mon.IsActive = true;
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    var query = from A in dbContext.datahierarchy

                                join J1 in dbContext.systemuser on new { i_InsertUserId = A.i_InsertUserId.Value }
                                equals new { i_InsertUserId = J1.i_SystemUserId } into J1_join
                                from J1 in J1_join.DefaultIfEmpty()

                                join J2 in dbContext.systemuser on new { i_UpdateUserId = A.i_UpdateUserId.Value }
                                equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                                from J2 in J2_join.DefaultIfEmpty()

                                join J4 in dbContext.datahierarchy on
                                new { ItemId = A.i_ParentItemId.Value, groupId = A.i_GroupId }
                                equals new { ItemId = J4.i_ItemId, groupId = J4.i_GroupId } into J4_join
                                from J4 in J4_join.DefaultIfEmpty()

                                where A.i_GroupId == pintGroupId
                                      && (A.i_IsDeleted == 0 || A.i_IsDeleted == null)

                                orderby A.i_Sort, A.v_Value2, A.v_Value1 descending

                                select new datahierarchyDto
                                {
                                    i_GroupId = A.i_GroupId,
                                    i_ItemId = A.i_ItemId,
                                    v_Value1 = A.v_Value1,
                                    v_Value2 = A.v_Value2,
                                    v_Field = A.v_Field,
                                    i_ParentItemId = A.i_ParentItemId.Value,
                                    v_ParentItemName = J4.v_Value1,
                                    v_CreationUser = J1.v_UserName,
                                    v_UpdateUser = J2.v_UserName,
                                    d_CreationDate = A.d_InsertDate,
                                    d_UpdateDate = A.d_UpdateDate,
                                    i_IsDeleted = A.i_IsDeleted,
                                    i_Header = A.i_Header,
                                    v_Value4 = A.v_Value4,
                                };


                    if (!string.IsNullOrEmpty(pstrFilterExpression))
                    {
                        query = query.Where(pstrFilterExpression);
                    }
                    if (!string.IsNullOrEmpty(pstrSortExpression))
                    {
                        query = query.OrderBy(pstrSortExpression);
                    }

                    var objData = query.ToList();
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

        public datahierarchyDto GetDataHierarchy(ref OperationResult pobjOperationResult, int pintGroupId, int pintParameterId)
        {
            //mon.IsActive = true;

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    datahierarchyDto objDtoEntity = null;

                    var objEntity = (from a in dbContext.datahierarchy
                                     where a.i_GroupId == pintGroupId && a.i_ItemId == pintParameterId
                                     select a).FirstOrDefault();

                    if (objEntity != null)
                        objDtoEntity = datahierarchyAssembler.ToDTO(objEntity);

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

        public void AddDataHierarchy(ref OperationResult pobjOperationResult, datahierarchyDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    datahierarchy objEntity = datahierarchyAssembler.ToEntity(pobjDtoEntity);

                    objEntity.d_InsertDate = DateTime.Now;
                    objEntity.i_InsertUserId = Int32.Parse(ClientSession[2]);
                    objEntity.i_IsDeleted = 0;

                    dbContext.AddTodatahierarchy(objEntity);
                    dbContext.SaveChanges();

                    pobjOperationResult.Success = 1;
                    Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName,
                        "datahierarchy",
                        "i_GroupId = " + objEntity.i_GroupId.ToString() + "Value1 = " + objEntity.v_Value1);
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "DatahierarchyBL.AddDataHierarchy()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }

        public void UpdateDataHierarchy(ref OperationResult pobjOperationResult, datahierarchyDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.datahierarchy
                                           where a.i_GroupId == pobjDtoEntity.i_GroupId && a.i_ItemId == pobjDtoEntity.i_ItemId
                                           select a).FirstOrDefault();

                    // Crear la entidad con los datos actualizados
                    pobjDtoEntity.d_UpdateDate = DateTime.Now;
                    pobjDtoEntity.i_UpdateUserId = int.Parse(ClientSession[2]);


                    var objEntity = pobjDtoEntity.ToEntity();

                    // Copiar los valores desde la entidad actualizada a la Entidad Fuente
                    dbContext.datahierarchy.ApplyCurrentValues(objEntity);

                    // Guardar los cambios
                    dbContext.SaveChanges();

                    pobjOperationResult.Success = 1;
                    Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName,
                        "datahierarchy",
                        "i_GroupId = " + objEntitySource.i_GroupId + "Value1 = " + objEntitySource.v_Value1);
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "DatahierarchyBL.UpdateDataHierarchy()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public void DeleteDataHierarchy(ref OperationResult pobjOperationResult, int pintGroupId, int pintParameterId, List<string> ClientSession)
        {
            //mon.IsActive = true;

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.datahierarchy
                                           where a.i_GroupId == pintGroupId && a.i_ItemId == pintParameterId
                                           select a).FirstOrDefault();

                    // Crear la entidad con los datos actualizados
                    objEntitySource.d_UpdateDate = DateTime.Now;
                    objEntitySource.i_UpdateUserId = Int32.Parse(ClientSession[2]);
                    objEntitySource.i_IsDeleted = 1;

                    // Guardar los cambios
                    dbContext.SaveChanges();
                    Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName,
                        "datahierarchy",
                        "i_GroupId = " + objEntitySource.i_GroupId.ToString() + "Value1 = " + objEntitySource.v_Value1);
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "DatahierarchyBL.DeleteDataHierarchy()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public int ObtenerMaxino(ref OperationResult pobjOperationResult, int pintGroupId)
        {
            //mon.IsActive = true;

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from A in dbContext.datahierarchy
                                 where A.i_GroupId == pintGroupId

                                 select new datahierarchyDto
                                 {
                                     i_GroupId = A.i_GroupId,
                                     i_ItemId = A.i_ItemId,

                                 }).ToList();
                    //.Max(p => p.i_ItemId)


                    pobjOperationResult.Success = 1;
                    return query.Count == 0 ? 1 : query.Max(p => p.i_ItemId) + 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return 0;
            }
        }

        public bool EsValidoDtahierarchy(string strCodigoValue2, int Grupo)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                datahierarchy P = (from p in dbContext.datahierarchy
                                   where p.v_Value2 == strCodigoValue2 && p.i_IsDeleted == 0 && p.i_GroupId == Grupo
                                   select p).FirstOrDefault();

                return P != null;
            }
        }

        public bool EsValidoDtahierarchyItem(int  ItemdId, int Grupo)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                datahierarchy P = (from p in dbContext.datahierarchy
                                   where p.i_ItemId == ItemdId && p.i_IsDeleted == 0 && p.i_GroupId == Grupo
                                   select p).FirstOrDefault();

                return P != null;
            }
        }

        public datahierarchyDto ObtenerDatahierarcyCodigo(int Grupo, string pstrCodigoValue2)
        {

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    datahierarchyDto P = (from p in dbContext.datahierarchy
                                          where p.v_Value2 == pstrCodigoValue2 && p.i_IsDeleted == 0 && p.i_GroupId == Grupo
                                          select new datahierarchyDto
                                          {
                                              i_ItemId = p.i_ItemId,
                                              v_Value1 = p.v_Value1,
                                              v_Value2 = p.v_Value2,
                                              i_GroupId = p.i_GroupId,


                                          }).FirstOrDefault();
                    return P;
                }
            }
            catch (Exception)
            {

                throw;
            }


        }

        public datahierarchyDto ObtenerDatahierarcyValue1Contains(int Grupo, string Coincidencia)
        {

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    datahierarchyDto P = (from p in dbContext.datahierarchy
                                          where p.v_Value1.Contains(Coincidencia) && p.i_IsDeleted == 0 && p.i_GroupId == Grupo
                                          select new datahierarchyDto
                                          {
                                              i_ItemId = p.i_ItemId,
                                              v_Value1 = p.v_Value1,
                                              v_Value2 = p.v_Value2,
                                              i_GroupId = p.i_GroupId,


                                          }).FirstOrDefault();
                    return P;
                }
            }
            catch (Exception)
            {

                throw;
            }


        }

        //agrego al final Miercoles 15:30

        /// <summary>
        /// Obtiene el datahierarchyDto deacuerdo a un filtro de búsqueda por coincidencia del Value1.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pintGroupId"></param>
        /// <param name="pstrValue1"></param>
        /// <returns></returns>
        public datahierarchyDto ObtenerDataHierarchyPorValue1(ref OperationResult pobjOperationResult, int pintGroupId, string pstrValue1)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var data = dbContext.datahierarchy.Where(p => p.i_GroupId == pintGroupId && p.v_Value1.ToLower().Contains(pstrValue1.ToLower().Trim()) && p.i_IsDeleted == 0).ToList();
                    pobjOperationResult.Success = 1;

                    if (data.Count <= 0) return null;
                    var result = data.FirstOrDefault();
                    return result.ToDTO();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "DatahierarchyBL.ObtenerDataHierarchyPorValue1()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }


        #region KeyValueDto
        public List<KeyValueDTO> GetDataHierarchyForComboKeyValueDto(ref OperationResult pobjOperationResult, int pintGroupId, string pstrSortExpression)
        {
            //mon.IsActive = true;

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = from a in dbContext.datahierarchy
                                where a.i_GroupId == pintGroupId && a.i_IsDeleted == 0
                                select a;

                    query = query.OrderBy(!string.IsNullOrEmpty(pstrSortExpression) ? pstrSortExpression : "v_Value1");

                    var query2 = query.AsEnumerable()
                                .Select(x => new KeyValueDTO
                                {
                                    Id = x.i_ItemId.ToString(),
                                    Value1 = x.v_Value1,
                                    Value2 = x.v_Value2,
                                    Value3 = x.i_ParentItemId.ToString(),
                                    Value5 = x.v_Field
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
        public List<KeyValueDTO> GetDataHierarchyForComboUbigeoKeyValueDto(ref OperationResult pobjOperationResult, int? pintParentItemID, int pintGropudID, string pstrSortExpression)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = from a in dbContext.datahierarchy
                            where a.i_GroupId == pintGropudID && a.i_ParentItemId == pintParentItemID && a.i_IsDeleted == 0
                            select a;

                query = query.OrderBy(!string.IsNullOrEmpty(pstrSortExpression) ? pstrSortExpression : "v_Value1");

                var query2 = query.AsEnumerable()
                            .Select(x => new KeyValueDTO
                            {
                                Id = x.i_ItemId.ToString(),
                                Value1 = x.v_Value1,
                                Value2 = x.v_Value2,
                                Value3 = x.i_ParentItemId.ToString()

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
        public List<KeyValueDTO> GetDataHierarchyForComboWithIDValueDto(ref OperationResult pobjOperationResult, int pintGroupId, string pstrSortExpression)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = from a in dbContext.datahierarchy
                                where a.i_GroupId == pintGroupId && a.i_IsDeleted == 0
                                select a;

                    query = query.OrderBy(!string.IsNullOrEmpty(pstrSortExpression) ? pstrSortExpression : "i_ItemId");

                    var query2 = query.AsEnumerable()
                                .Select(x => new KeyValueDTO
                                {
                                    Id = x.i_ItemId.ToString(),
                                    Value1 = x.i_ItemId.ToString("00") + " | " + x.v_Value1,
                                    Value2 = x.v_Value2,
                                    Value3 = x.i_ParentItemId.ToString(),

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
        public List<KeyValueDTO> GetDataHierarchiesForCombo(ref OperationResult pobjOperationResult, int pintGroupId, string pstrSortExpression, bool usarlAlmacenPredeterminado = false)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = dbContext.datahierarchy.Where(a => a.i_GroupId == pintGroupId && a.i_IsDeleted == 0);
                    if (usarlAlmacenPredeterminado)
                    {
                        var idAlmacen = (Globals.ClientSession.i_IdAlmacenPredeterminado ?? 1).ToString();
                        query = query.Where(p => p.v_Value4.Trim().Equals(idAlmacen.Trim()));
                    }

                    query = query.OrderBy(!string.IsNullOrEmpty(pstrSortExpression) ? pstrSortExpression : "v_Value1");
                    var query2 = query.AsEnumerable()
                        .Select(x => new KeyValueDTO
                        {
                            Id = x.i_ItemId.ToString(),
                            Value1 = x.v_Value1,
                            Value2 = x.v_Value2,
                            Value3 = x.v_Field
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

        public List<KeyValueDTO> LlenarListaPreciosPorAlmacen(ref OperationResult pobjOperationResult, int pintGroupId, string pstrSortExpression, int pIntAlmacen)
        {
            //mon.IsActive = true;

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    //var query = from a in dbContext.datahierarchy
                    //            where a.i_GroupId == pintGroupId && a.i_IsDeleted == 0
                    //            select a;

                    var query = (from a in dbContext.listaprecio
                                 join b in dbContext.datahierarchy on
                                 new { Grupo = pintGroupId, eliminado = 0, lp = a.i_IdLista.Value } equals
                                 new { Grupo = b.i_GroupId, eliminado = b.i_IsDeleted.Value, lp = b.i_ItemId } into b_join
                                 from b in b_join.DefaultIfEmpty()
                                 where a.i_IdAlmacen == pIntAlmacen && a.i_Eliminado == 0

                                 select b);

                    query = query.OrderBy(!string.IsNullOrEmpty(pstrSortExpression) ? pstrSortExpression : "v_Value1");

                    var query2 = query.AsEnumerable()
                        .Select(x => new KeyValueDTO
                        {
                            Id = x.i_ItemId.ToString(),
                            Value1 = x.v_Value1,
                            Value2 = x.v_Value2,
                            Value3 = x.v_Field
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

        public List<GridKeyValueDTO> GetDataHierarchiesForComboGrid(ref OperationResult pobjOperationResult, int pintGroupId, string pstrSortExpression)
        {
            //mon.IsActive = true;

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = from a in dbContext.datahierarchy
                                where a.i_GroupId == pintGroupId && a.i_IsDeleted == 0
                                select a;

                    query = query.OrderBy(!string.IsNullOrEmpty(pstrSortExpression) ? pstrSortExpression : "i_ItemId");

                    var query2 = query.AsEnumerable()
                                .Select(x => new GridKeyValueDTO
                                {
                                    Id = x.i_ItemId.ToString(),
                                    Value1 = x.v_Value1,
                                    Value2 = x.v_Value2,
                                    Value3 = x.v_Field,
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


        public List<GridKeyValueDTO> Respuestas(ref OperationResult pobjOperationResult)
        {
            //mon.IsActive = true;
            List<GridKeyValueDTO> ListaRespuestas = new List<GridKeyValueDTO>();
            try
            {
                GridKeyValueDTO obj1 = new GridKeyValueDTO();
                obj1.Id = "1";
                obj1.Value1 = "SI";
                ListaRespuestas.Add(obj1);

                obj1 = new GridKeyValueDTO();
                obj1.Id = "0";
                obj1.Value1 = "NO";
                ListaRespuestas.Add(obj1);

                pobjOperationResult.Success = 1;
                return ListaRespuestas;

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }


        public List<KeyValueDTO> GetDataHierarchiesForComboWithValue2(ref OperationResult pobjOperationResult, int pintGroupId, string pstrSortExpression, string filter = null)
        {
            //mon.IsActive = true;

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    var query = from a in dbContext.datahierarchy
                                where a.i_GroupId == pintGroupId && a.i_IsDeleted == 0
                                select a;

                    query = query.OrderBy(!string.IsNullOrEmpty(pstrSortExpression) ? pstrSortExpression : "v_Value2");
                    if (filter != null) query = query.Where(filter);
                    var query2 = query.AsEnumerable()
                        .Select(x => new KeyValueDTO
                        {
                            Id = x.i_ItemId.ToString(),
                            Value1 = x.v_Value2 + " | " + x.v_Value1,

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

        public List<KeyValueDTO> GetDataHierarchiesForComboWithValue1yValue2(ref OperationResult pobjOperationResult, int pintGroupId, string pstrSortExpression, string Cadena1, string Cadena2)
        {
            //mon.IsActive = true;

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = from a in dbContext.datahierarchy
                                where a.i_GroupId == pintGroupId && a.i_IsDeleted == 0
                                select a;

                    query = query.OrderBy(!string.IsNullOrEmpty(pstrSortExpression) ? pstrSortExpression : "v_Value2");

                    var query2 = query.AsEnumerable()
                        .Select(x => new KeyValueDTO
                        {
                            Id = x.i_ItemId.ToString(),
                            Value1 = x.v_Value1 + Cadena1 + " | " + x.v_Value2 + Cadena2
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

        public List<KeyValueDTO> GetDataHierarchiesForComboWithValue1yValue2yValue3(ref OperationResult pobjOperationResult, int pintGroupId, string pstrSortExpression)
        {
            //mon.IsActive = true;

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = from a in dbContext.datahierarchy
                                where a.i_GroupId == pintGroupId && a.i_IsDeleted == 0
                                select a;

                    query = query.OrderBy(!string.IsNullOrEmpty(pstrSortExpression) ? pstrSortExpression : "v_Value2");

                    var query2 = query.AsEnumerable()
                        .Select(x => new KeyValueDTO
                        {
                            Id = x.i_ItemId.ToString(),
                            Value1 = x.v_Value2 + " | " + x.v_Value1,
                            Value2 = x.v_Value2,
                            Value3 = x.v_Field,

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


        public BindingList<MesesDto> ObtenerMeses()
        {

            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var query = (from n in dbContext.datahierarchy

                             where n.i_IsDeleted == 0 && n.i_GroupId == 94

                             select new MesesDto
                             {
                                 i_IdMes = n.i_ItemId,
                                 v_Mes = "",
                                 NombreMes = n.v_Value1,
                                 i_GroupId = n.i_GroupId,

                             }).ToList();


                //            var query = (from n in dbContext.datahierarchy.ToList ().Where (x=>x.i_IsDeleted ==0 && x.i_GroupId ==94).Select (x=>new {i_idmes=x.i_ItemId ,v_Mes="", NombreMes=x.v_Value1}).ToList ());

                //query =query .Where (n.i_IsDeleted == 0 && n.i_GroupId == 94

                //           select new MesesDto
                //           {
                //               i_IdMes = n.i_ItemId,
                //               v_Mes = string.Empty,
                //               NombreMes = n.v_Value1,

                //           }).ToList();

                query = query.Where(x => x.i_GroupId == 94).ToList();
                var queryFinal = (from a in query

                                  select new MesesDto
                                  {
                                      i_IdMes = a.i_IdMes,
                                      v_Mes = a.i_IdMes.Value.ToString("00"),
                                      NombreMes = a.NombreMes,

                                  }).ToList();

                return new BindingList<MesesDto>(queryFinal);
            }
        }

        #endregion

        #region KeyValueDto
        public List<ReporteDataHierarchy> ReporteDataHierarchy(string pstrt_Orden, int pintGroupId)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    #region Query

                    var query =
                    (from A in dbContext.datahierarchy

                     where A.i_IsDeleted == 0 && A.i_GroupId == pintGroupId
                     select new ReporteDataHierarchy
                     {
                         v_Value1 = A.v_Value1,
                         v_Value2 = A.v_Value2,
                         v_Field = A.v_Field ?? "",
                         i_ParentItemId = A.i_ParentItemId.Value == null ? 0 : A.i_ParentItemId.Value,
                         i_Sort = A.i_Sort.Value == null ? 0 : A.i_Sort.Value,
                     });

                    #endregion

                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                //pobjOperationResult.Success = 0;
                //pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }
        #endregion

        #region Reportes

        public List<ReporteCentroCostos> ReporteCentroCostos(ref OperationResult objOperationResult)
        {

            try
            {
                objOperationResult.Success = 1;
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    var query = (from n in dbContext.datahierarchy

                                 where n.i_GroupId == 31 & n.i_IsDeleted == 0

                                 select new ReporteCentroCostos
                                 {

                                     Codigo = "",
                                     Descripcion = n.v_Value1,
                                     CodigoInt = n.i_ItemId

                                 }).ToList();

                    var queryFinal = (from n in query

                                      select new ReporteCentroCostos
                                      {

                                          Codigo = n.CodigoInt.Value.ToString("00"),
                                          Descripcion = n.Descripcion
                                      }).OrderBy(x => x.Codigo).ToList();

                    return queryFinal;
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;
            }

        }

        public List<datahierarchyDto> ReporteDatahierarchyCodigocuatroDigitos(ref OperationResult objOperationResult, int Grupo)
        {
            try
            {
                objOperationResult.Success = 1;

                List<datahierarchyDto> ListaFinal = new List<datahierarchyDto>();
                datahierarchyDto objDatahierarchyDto = new datahierarchyDto();
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var Listadatahierarchys = (from n in dbContext.datahierarchy

                                               where n.i_IsDeleted == 0 && n.i_GroupId == Grupo

                                               select new datahierarchyDto
                                               {
                                                   v_Value1 = n.v_Value1,
                                                   v_Value2 = n.v_Value2,
                                                   i_GroupId = n.i_GroupId,
                                                   v_Field = n.v_Field,
                                                   i_ParentItemId = n.i_ParentItemId,
                                                   i_Sort = n.i_Sort,
                                                   i_Header = n.i_Header,
                                                   i_ItemId = n.i_ItemId,

                                               }).ToList();

                    var Cabeceras = Listadatahierarchys.Where(i => i.i_Header == 1).Select(x => x.v_Value2).ToList();

                    foreach (var item in Listadatahierarchys)
                    {
                        objDatahierarchyDto = new datahierarchyDto();
                        objDatahierarchyDto = item;
                        var valoresCabecera = Listadatahierarchys.Where(x => x.v_Value2.Substring(0, 2) == item.v_Value2.Substring(0, 2) && x.i_Header == 1).ToList();
                        objDatahierarchyDto.Grupo1 = valoresCabecera.FirstOrDefault().v_Value2 + "       " + valoresCabecera.FirstOrDefault().v_Value1;
                        objDatahierarchyDto.Grupo2 = "";
                        if (objDatahierarchyDto.i_Header != 1)
                        {
                            ListaFinal.Add(item);
                        }
                    }

                    foreach (var item in Cabeceras)
                    {
                        objDatahierarchyDto = new datahierarchyDto();
                        if (!ListaFinal.Select(x => x.v_Value2.Substring(0, 2)).Contains(item.Substring(0, 2)))
                        {
                            //objDatahierarchyDto.v_Value2 = item;
                            objDatahierarchyDto.v_Value2 = "";
                            objDatahierarchyDto.Grupo1 = item + "       " + Listadatahierarchys.Where(x => x.v_Value2 == item).FirstOrDefault().v_Value1;
                            objDatahierarchyDto.Grupo2 = "";
                            ListaFinal.Add(objDatahierarchyDto);
                        }
                    }

                    return ListaFinal.OrderBy(x => x.v_Value2).ToList();

                }
            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                return null;
            }
        }

        public List<datahierarchyDto> ReporteDatahierarchys(ref OperationResult objOperationResult, int Grupo)
        {
            try
            {
                objOperationResult.Success = 1;


                List<datahierarchyDto> ListaFinal = new List<datahierarchyDto>();
                datahierarchyDto objDatahierarchyDto = new datahierarchyDto();
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var Listadatahierarchys = (from n in dbContext.datahierarchy

                                               where n.i_IsDeleted == 0 && n.i_GroupId == Grupo

                                               select new datahierarchyDto
                                               {
                                                   v_Value1 = n.v_Value1,
                                                   v_Value2 = n.v_Value2,
                                                   i_GroupId = n.i_GroupId,
                                                   v_Field = n.v_Field,
                                                   i_ParentItemId = n.i_ParentItemId,
                                                   i_Sort = n.i_Sort,
                                                   i_Header = n.i_Header,
                                                   i_ItemId = n.i_ItemId,
                                                   i_IsDeleted = n.i_IsDeleted,
                                                   i_InsertUserId = n.i_InsertUserId,
                                                   i_RecordStatusId = n.i_RecordStatusId,
                                                   i_SyncStatusId = n.i_SyncStatusId,
                                                   i_UpdateUserId = n.i_UpdateUserId,
                                                   d_InsertDate = n.d_InsertDate,
                                                   d_CreationDate = n.d_InsertDate,


                                               }).ToList();
                    return Listadatahierarchys.OrderBy(x => x.v_Value2).ToList();

                }
            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                return null;
            }
        }

        public List<datahierarchyDto> ReporteDatahierarchysHorasExtras(ref OperationResult objOperationResult, int Grupo)
        {
            try
            {
                objOperationResult.Success = 1;

                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var listadatahierarchys = (from n in dbContext.datahierarchy
                                               join j in dbContext.datahierarchy on new {id = n.i_Header.Value, p = 163 } 
                                                                    equals new { id = j.i_ItemId, p = j.i_GroupId } into j_join
                                               from j in j_join.DefaultIfEmpty()

                                               join c in dbContext.planillaconceptos on n.v_Value4 equals c.v_IdConceptoPlanilla into c_join
                                               from c in c_join.DefaultIfEmpty()

                                               where n.i_IsDeleted == 0 && n.i_GroupId == Grupo

                                               select new datahierarchyDto
                                               {
                                                   v_Value1 = n.v_Value1,
                                                   v_Value2 = n.v_Value2,
                                                   i_GroupId = n.i_GroupId,
                                                   v_Field = n.v_Field,
                                                   i_ParentItemId = n.i_ParentItemId,
                                                   i_Sort = n.i_Sort,
                                                   i_Header = n.i_Header,
                                                   i_ItemId = n.i_ItemId,
                                                   i_IsDeleted = n.i_IsDeleted,
                                                   i_InsertUserId = n.i_InsertUserId,
                                                   i_RecordStatusId = n.i_RecordStatusId,
                                                   i_SyncStatusId = n.i_SyncStatusId,
                                                   i_UpdateUserId = n.i_UpdateUserId,
                                                   d_InsertDate = n.d_InsertDate,
                                                   d_CreationDate = n.d_InsertDate,
                                                   Grupo1 = j.v_Value1,
                                                   Grupo2 = c.v_Codigo + "-" + c.v_Nombre,
                                                   v_Value4 = n.v_Value4

                                               }).ToList();
                    return listadatahierarchys.OrderBy(x => x.v_Value2).ToList();

                }
            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                return null;
            }
        }
        #endregion

        #region Facturacion
        /// <summary>
        /// Elimina completamente un grupo del datahierarchy
        /// </summary>
        /// <param name="pobjOperationResult">resultado de la operacion</param>
        /// <param name="idGroup">id del Grupo a eliminar</param>
        public void EliminarGrupo(out OperationResult pobjOperationResult, int idGroup)
        {
            pobjOperationResult = new OperationResult();
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var items = (from n in
                                     dbContext.datahierarchy.AsParallel().WithMergeOptions(ParallelMergeOptions.NotBuffered)
                                 where n.i_GroupId == idGroup
                                 select n
                        );
                    foreach (var item in items)
                        dbContext.datahierarchy.DeleteObject(item);
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
        #endregion
    }
}
