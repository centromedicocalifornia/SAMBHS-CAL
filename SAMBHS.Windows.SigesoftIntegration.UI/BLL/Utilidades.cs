using SAMBHS.Common.BE.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using SAMBHS.Windows.SigesoftIntegration.UI.Dtos;
using SAMBHS.Common.Resource;
using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;

namespace SAMBHS.Windows.SigesoftIntegration.UI.BLL
{
    public class Utilidades
    {
        public static List<KeyValueDTO> GetSystemParameterForCombo(ref OperationResult pobjOperationResult, int pintGroupId, string pstrSortExpression)
        {
            //mon.IsActive = true;

            try
            {
                //SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query = "select * from systemparameter where i_GroupId = " + pintGroupId +
                                " and i_IsDeleted = 0";

                    var List = cnx.Query<systemparameterDto>(query).ToList();

                    var List2 = List.OrderBy(x => x.v_Value1);

                    var query2 = List2.AsEnumerable()
                        .Select(x => new KeyValueDTO
                        {
                            Id = x.i_ParameterId.ToString(),
                            Value1 = x.v_Value1,
                            Value2 = x.v_Value2,
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

        public static List<KeyValueDTO> GetDataHierarchyForCombo(ref OperationResult pobjOperationResult, int pintGroupId, string pstrSortExpression)
        {
            //mon.IsActive = true;

            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query = "select * from datahierarchy where i_GroupId = " + pintGroupId +
                                " and i_IsDeleted = 0";

                    var List = cnx.Query<datahierarchyDto>(query).ToList();

                    var List2 = List.OrderBy(x => x.v_Value1);

                    var query2 = List2.AsEnumerable()
                        .Select(x => new KeyValueDTO
                        {
                            Id = x.i_ItemId.ToString(),
                            Value1 = x.v_Value1,
                            Value2 = x.v_Value2,
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

        public static List<KeyValueDTO> GetDataHierarchyForComboDepartamento(ref OperationResult pobjOperationResult, int pintGroupId, string pstrSortExpression)
        {
            //mon.IsActive = true;

            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query = "select * from datahierarchy where i_GroupId = " + pintGroupId +
                                " and i_IsDeleted = 0 and i_ParentItemId = -1";

                    var List = cnx.Query<datahierarchyDto>(query).ToList();

                    var List2 = List.OrderBy(x => x.v_Value1); 

                    var query2 = List2.AsEnumerable()
                        .Select(x => new KeyValueDTO
                        {
                            Id = x.i_ItemId.ToString(),
                            Value1 = x.v_Value1,
                            Value2 = x.v_Value2,
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

        public static List<KeyValueDTO> GetDataHierarchyForComboProvincia(ref OperationResult pobjOperationResult, int pintGroupId, int? pintParentItemId)
        {
            //mon.IsActive = true;

            try
            {
                if (pintParentItemId == null)
                {
                    return new List<KeyValueDTO>();
                }
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query = "select * from datahierarchy where i_GroupId = " + pintGroupId +
                                " and i_IsDeleted = 0 and i_ParentItemId = " + pintParentItemId + "";

                    var List = cnx.Query<datahierarchyDto>(query).ToList();

                    var List2 = List.OrderBy(x => x.v_Value1);
                    var query2 = List2.AsEnumerable()
                        .Select(x => new KeyValueDTO
                        {
                            Id = x.i_ItemId.ToString(),
                            Value1 = x.v_Value1,
                            Value2 = x.v_Value2,
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

        public static List<KeyValueDTO> GetDataHierarchyForComboDistrito_(ref OperationResult pobjOperationResult, int pintGroupId)
        {
            //mon.IsActive = true;

            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query = "select * from datahierarchy where i_GroupId = " + pintGroupId +
                                " and i_IsDeleted = 0 and i_ParentItemId != -1";

                    var List = cnx.Query<datahierarchyDto>(query).ToList();

                    var List2 = List.OrderBy(x => x.v_Value1);
                    var query2 = List2.AsEnumerable()
                        .Select(x => new KeyValueDTO
                        {
                            Id = x.i_ItemId.ToString(),
                            Value1 = x.v_Value1,
                            Value2 = x.v_Value2,
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


        public static int GetNextSecuentialId_SAMBHS(int pintNodeId, int pintTableId)
        {
            using (var cnx = ConnectionHelper.GetNewContasolConnection)
            {
                var query = "select * from secuential where i_TableId = " + pintTableId + " and i_NodeId = " +
                            pintNodeId + "";

                var objSecuential = cnx.Query<secuentialDto>(query).SingleOrDefault();
                // Actualizar el campo con el nuevo valor a efectos de reservar el ID autogenerado para evitar colisiones entre otros nodos
                if (objSecuential != null)
                {
                    objSecuential.i_SecuentialId = objSecuential.i_SecuentialId + 1;

                    var insert = "UPDATE secuential " +
                                 "set i_SecuentialId = " + objSecuential.i_SecuentialId + "" +
                                 "where i_TableId = " + pintTableId + " and i_NodeId = " + pintNodeId + "";
                    cnx.Execute(insert);
                }
                else
                {
                    var insert = "INSERT INTO secuential (i_NodeId, i_TableId, i_SecuentialId)" +
                                 "VALUES (" + pintNodeId + ", " + pintTableId + ", 0)";

                    cnx.Execute(insert);
                    objSecuential.i_SecuentialId = 0;
                }

                return objSecuential.i_SecuentialId.Value;
            }

        }


        public static int GetNextSecuentialId(int pintNodeId, int pintTableId)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                var query = "select * from secuential where i_TableId = " + pintTableId + " and i_NodeId = " +
                            pintNodeId + "";

                var objSecuential = cnx.Query<secuentialDto>(query).SingleOrDefault();
                // Actualizar el campo con el nuevo valor a efectos de reservar el ID autogenerado para evitar colisiones entre otros nodos
                if (objSecuential != null)
                {
                    objSecuential.i_SecuentialId = objSecuential.i_SecuentialId + 1;

                    var insert = "UPDATE secuential " +
                                 "set i_SecuentialId = " + objSecuential.i_SecuentialId + "" +
                                 "where i_TableId = " + pintTableId + " and i_NodeId = " + pintNodeId + "";
                    cnx.Execute(insert);
                }
                else
                {
                    var insert = "INSERT INTO secuential (i_NodeId, i_TableId, i_SecuentialId)" +
                                 "VALUES (" + pintNodeId + ", " + pintTableId + ", 0)";

                    cnx.Execute(insert);
                    objSecuential.i_SecuentialId = 0;
                }

                return objSecuential.i_SecuentialId.Value;
            }
        
        }

        public static string GetNewId(int pintNodeId, int pintSequential, string pstrPrefix)
        {
            return string.Format("N{0}-{1}{2}", pintNodeId.ToString("000"), pstrPrefix, pintSequential.ToString("000000000"));
        }

    }
}
