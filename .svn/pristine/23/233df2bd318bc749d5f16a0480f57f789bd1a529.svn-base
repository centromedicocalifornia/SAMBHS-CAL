using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic;
using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;




namespace SAMBHS.Common.BL
{
    public class NodeBL
    {

        #region Node
        public nodeDto GetNodeByNodeId(ref OperationResult pobjOperationResult, int pintNodeId)
        {
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                nodeDto objDtoEntity = null;

                var objEntity = (from a in dbContext.node
                                 where a.i_NodeId == pintNodeId
                                 select a).FirstOrDefault();

                if (objEntity != null)
                    objDtoEntity = nodeAssembler.ToDTO(objEntity);

                pobjOperationResult.Success = 1;

                return objDtoEntity;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public ICollection<nodeDto> GetAllNode(ref OperationResult pobjOperationResult)
        {
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                var query = from a in dbContext.node where a.i_IsDeleted == 0 select a;

                ICollection<nodeDto> objData = nodeAssembler.ToDTOs(query.OrderBy(p => p.v_RazonSocial).ToArray());

                pobjOperationResult.Success = 1;
                return objData;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                //pobjOperationResult.ExceptionMessage = Common.Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public List<nodeDto> GetNodePagedAndFiltered(ref OperationResult pobjOperationResult, int? pintPageIndex, int? pintResultsPerPage, string pstrSortExpression, string pstrFilterExpression)
        {

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                var query = (from n in dbContext.node

                             join su2 in dbContext.systemuser on new { i_InsertUserId = n.i_InsertUserId.Value }
                                                           equals new { i_InsertUserId = su2.i_SystemUserId } into su2_join
                             from su2 in su2_join.DefaultIfEmpty()

                             join su3 in dbContext.systemuser on new { i_UpdateUserId = n.i_UpdateUserId.Value }
                                                           equals new { i_UpdateUserId = su3.i_SystemUserId } into su3_join
                             from su3 in su3_join.DefaultIfEmpty()

                             where n.i_IsDeleted == 0

                             select new nodeDto
                             {
                                 i_NodeId = n.i_NodeId,
                                 v_RazonSocial = n.v_RazonSocial,
                                 v_Direccion = n.v_Direccion,
                                 d_InsertDate = n.d_InsertDate,
                                 d_UpdateDate = n.d_UpdateDate,
                                 v_InsertUser = su2.v_UserName,
                                 v_UpdateUser = su3.v_UserName,
                                 v_RUC = n.v_RUC
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
                if (pintPageIndex.HasValue && pintResultsPerPage.HasValue)
                {
                    int intStartRowIndex = pintPageIndex.Value * pintResultsPerPage.Value;
                    query = query.Skip(intStartRowIndex);
                }
                if (pintResultsPerPage.HasValue)
                {
                    query = query.Take(pintResultsPerPage.Value);
                }

                List<nodeDto> objData = query.ToList();
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

        public int GetNodeCount(ref OperationResult pobjOperationResult, string filterExpression)
        {
            //mon.IsActive = true;
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                var query = from a in dbContext.node select a;

                string _filterEx = "i_IsDeleted==0";
                query = query.Where(_filterEx);

                if (!string.IsNullOrEmpty(filterExpression))
                    query = query.Where(filterExpression);

                int intResult = query.Count();

                pobjOperationResult.Success = 1;
                return intResult;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return 0;
            }
        }

        public void AddNode(ref OperationResult pobjOperationResult, nodeDto pobjDtoEntity, List<string> ClientSession)
        {
            int SecuentialId = 0;
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                node objEntity = nodeAssembler.ToEntity(pobjDtoEntity);

                objEntity.d_InsertDate = DateTime.Now;
                objEntity.i_InsertUserId = Int32.Parse(ClientSession[2]);
                objEntity.i_IsDeleted = 0;

                // Autogeneramos el Pk de la tabla

                dbContext.AddTonode(objEntity);
                dbContext.SaveChanges();

                systemusernode _systemusernode = new systemusernode();
                _systemusernode.i_SystemUserId = 1;
                _systemusernode.i_NodeId = dbContext.node.FirstOrDefault(p => p.v_RUC == objEntity.v_RUC && p.i_IsDeleted == 0).i_NodeId;
                _systemusernode.d_InsertDate = DateTime.Now;
                _systemusernode.i_InsertUserId = Int32.Parse(ClientSession[2]);
                _systemusernode.i_IsDeleted = 0;

                dbContext.systemusernode.AddObject(_systemusernode);
                dbContext.SaveChanges();

                pobjOperationResult.Success = 1;
                // Llenar entidad Log
                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                // Llenar entidad Log
                return;
            }
        }

        public void UpdateNode(ref OperationResult pobjOperationResult, nodeDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.node
                                       where a.i_NodeId == pobjDtoEntity.i_NodeId
                                       select a).FirstOrDefault();

                // Crear la entidad con los datos actualizados
                pobjDtoEntity.d_UpdateDate = DateTime.Now;
                pobjDtoEntity.i_UpdateUserId = Int32.Parse(ClientSession[2]);

                node objEntity = nodeAssembler.ToEntity(pobjDtoEntity);

                // Copiar los valores desde la entidad actualizada a la Entidad Fuente
                dbContext.node.ApplyCurrentValues(objEntity);

                // Guardar los cambios
                dbContext.SaveChanges();

                pobjOperationResult.Success = 1;
                // Llenar entidad Log
                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                // Llenar entidad Log
                return;
            }
        }

        public string GetNextNodeId()
        {
            using (SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel())
            {
                if (dbContext.node.Any())
                {
                    return (dbContext.node.Max(p => p.i_NodeId) + 1).ToString();
                }
                else { return "1"; }
            }
        }

        public void DeleteNode(ref OperationResult pobjOperationResult, int pintNodeId, List<string> ClientSession)
        {
            //mon.IsActive = true;
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.node
                                       where a.i_NodeId == pintNodeId
                                       select a).FirstOrDefault();

                // Crear la entidad con los datos actualizados
                objEntitySource.d_UpdateDate = DateTime.Now;
                objEntitySource.i_UpdateUserId = Int32.Parse(ClientSession[2]);
                objEntitySource.i_IsDeleted = 1;

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


        #endregion

        #region NodeWarehouse

        public List<nodewarehouseDto> GetNodeWarehouse(ref OperationResult pobjOperationResult, int pintNodeId)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                var query = (from a in dbContext.nodewarehouse
                             join b in dbContext.warehouse on a.i_WarehouseId equals b.i_WarehouseId
                             join e in dbContext.node on a.i_NodeId equals e.i_NodeId

                             where a.i_NodeId == pintNodeId &&
                                    a.i_IsDeleted == 0
                             select new nodewarehouseDto
                             {
                                 v_RUCEmpresa = e.v_RUC,
                                 i_NodeWarehouseId = a.i_NodeWarehouseId,
                                 i_WarehouseId = a.i_WarehouseId,
                                 v_WarehouseName = b.v_Name
                             }).ToList();

                pobjOperationResult.Success = 1;
                return query;

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public void AddNodeWarehouse(ref OperationResult pobjOperationResult, nodewarehouseDto pobjDtoEntity, List<string> ClientSession)
        {
            //int SecuentialId = 0;
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                var objEntitySource = (from a in dbContext.nodewarehouse
                                       where a.i_WarehouseId == pobjDtoEntity.i_WarehouseId &&
                                            a.i_NodeId == pobjDtoEntity.i_NodeId
                                       select a).FirstOrDefault();



                if (objEntitySource != null)
                {
                    if (objEntitySource.i_IsDeleted == 0)   // Registro ya esta grabado
                    {
                        // validar que no se vuelva a registrar datos ya existentes
                        pobjOperationResult.ErrorMessage = "Este almacén ya existe para este nodo, agregue otro por favor.)";
                        pobjOperationResult.Success = 1;
                        return;
                    }
                    else if (objEntitySource.i_IsDeleted == 1)  // Registro macado como eliminado
                    {
                        // Actualizar registro (dar de alta al registro ya existente "no volver a insertar")
                        OperationResult objOperationResult2 = new OperationResult();

                        //UpdateNodeWarehouse(ref objOperationResult2,
                        //                         pobjDtoEntity.i_WarehouseId,
                        //                         pobjDtoEntity.i_NodeId,
                        //                         ClientSession);

                        #region Replica cambios a la BD de la Empresa
                        var Almacen = (from a in dbContext.warehouse
                                       where a.i_WarehouseId == pobjDtoEntity.i_WarehouseId
                                       select a).FirstOrDefault();

                        var Node = (from a in dbContext.node
                                    where a.i_NodeId == pobjDtoEntity.i_NodeId
                                    select new { a.v_RUC }).FirstOrDefault();

                        if (Node != null)
                        {
                            //dbContext.spi_almacen_empresa(Node.v_RUC, pobjDtoEntity.i_WarehouseId, Almacen.v_Name, int.Parse(ClientSession[2]), Almacen.v_Address, DateTime.Today.ToShortDateString());
                        }
                        #endregion

                        pobjOperationResult = objOperationResult2;

                        return;
                    }
                }
                else
                {
                    // Autogeneramos el Pk de la tabla
                    int intNodeId = int.Parse(ClientSession[0]);

                    // Grabar nuevo registro
                    nodewarehouse objEntity;

                    //// Sub-consulta para alberto
                    //var objEntitySource11 = (from a in dbContext.systemparameter
                    //                         where a.i_GroupId == 119 && a.i_ParameterId == NodeServiceProfile.i_MasterServiceId
                    //                         select a).FirstOrDefault();

                    objEntity = nodewarehouseAssembler.ToEntity(pobjDtoEntity);
                    objEntity.i_NodeId = pobjDtoEntity.i_NodeId;
                    objEntity.i_WarehouseId = pobjDtoEntity.i_WarehouseId;
                    objEntity.d_InsertDate = DateTime.Now;
                    objEntity.i_InsertUserId = int.Parse(ClientSession[2]);
                    objEntity.i_IsDeleted = 0;

                    dbContext.AddTonodewarehouse(objEntity);
                    dbContext.SaveChanges();

                    #region Replica cambios a la BD de la Empresa
                    var Almacen = (from a in dbContext.warehouse
                                   where a.i_WarehouseId == objEntity.i_WarehouseId
                                   select a).FirstOrDefault();

                    var Node = (from a in dbContext.node
                                where a.i_NodeId == objEntity.i_NodeId
                                select new { a.v_RUC }).FirstOrDefault();

                    if (Node != null)
                    {
                        //dbContext.spi_almacen_empresa(Node.v_RUC, pobjDtoEntity.i_WarehouseId, Almacen.v_Name, int.Parse(ClientSession[2]), Almacen.v_Address, DateTime.Today.ToShortDateString());
                    }
                    #endregion

                    pobjOperationResult.Success = 1;

                    // Llenar entidad Log

                    return;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                // Llenar entidad Log
                return;
            }
        }

        public void UpdateNodeWarehouse(ref OperationResult pobjOperationResult, int? pstrWarehouseId, int pintNodeId, List<string> ClientSession)
        {
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                var objEntitySource = (from a in dbContext.nodewarehouse
                                       where a.i_WarehouseId == pstrWarehouseId &&
                                             a.i_NodeId == pintNodeId
                                       select a).FirstOrDefault();

                string Node_RUC = (from a in dbContext.node
                                   where a.i_NodeId == pintNodeId
                                   select new { a.v_RUC }).FirstOrDefault().v_RUC;

                var Almacen = (from a in dbContext.warehouse
                               where a.i_WarehouseId == pstrWarehouseId
                               select a).FirstOrDefault();

                // Crear la entidad con los datos actualizados
                objEntitySource.d_UpdateDate = DateTime.Now;
                objEntitySource.i_UpdateUserId = Int32.Parse(ClientSession[2]);
                objEntitySource.i_IsDeleted = 0;

                // Guardar los cambios
                dbContext.SaveChanges();
                //dbContext.spd_almacen_empresa(Node_RUC, Almacen.i_WarehouseId);
                pobjOperationResult.Success = 1;
                // Llenar entidad Log
                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return;
            }
        }

        public void DeleteNodeWarehouse(ref OperationResult pobjOperationResult, int pstrNodeWarehouseId, List<string> ClientSession)
        {
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                var objEntitySource = (from a in dbContext.nodewarehouse
                                       where a.i_NodeWarehouseId == pstrNodeWarehouseId
                                       select a).FirstOrDefault();

                string Node_RUC = (from a in dbContext.node
                                   where a.i_NodeId == objEntitySource.i_NodeId
                                   select new { a.v_RUC }).FirstOrDefault().v_RUC;

                var Almacen = (from a in dbContext.warehouse
                               where a.i_WarehouseId == objEntitySource.i_WarehouseId
                               select a).FirstOrDefault();

                // Crear la entidad con los datos actualizados
                objEntitySource.d_UpdateDate = DateTime.Now;
                objEntitySource.i_UpdateUserId = Int32.Parse(ClientSession[2]);
                objEntitySource.i_IsDeleted = 1;

                // Guardar los cambios
                dbContext.SaveChanges();
                //dbContext.spd_almacen_empresa(Node_RUC, Almacen.i_WarehouseId);
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

        public int FillWarehouseId(ref OperationResult pobjOperationResult, int? pstrWarehouseId, int? pintNodeId)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                //var query = from a in dbContext.node select a;
                var query = from a in dbContext.nodewarehouse
                            where a.i_WarehouseId == pstrWarehouseId && a.i_NodeId != pintNodeId &&
                                   a.i_IsDeleted == 0
                            select a;

                pobjOperationResult.Success = 1;
                return query.Count();

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return 0;
            }
        }

        #endregion

        #region KeyValueDto
        public static List<KeyValueDTO> GetAllNodeForCombo(ref OperationResult pobjOperationResult)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                var query = from a in dbContext.node
                            where a.i_IsDeleted == 0
                            select a;

                var q = from a in query.ToList()
                        select new KeyValueDTO
                        {
                            Id = a.i_NodeId.ToString(),
                            Value1 = a.v_RazonSocial
                        };

                List<KeyValueDTO> NodeList = q.OrderBy(p => p.Value1).ToList();
                pobjOperationResult.Success = 1;
                return NodeList;

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

        public List<ReporteEmpresa> ReporteEmpresa()
        {
            try
            {

                using (SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel())
                {
                    int _intNodeId = int.Parse(Utils.Windows.GetApplicationConfigValue("NodeId"));

                    #region Query

                    var query =
                    (from A in dbContext.node
                        where (A.i_IsDeleted == 0 && A.i_NodeId == Globals.ClientSession.i_CurrentExecutionNodeId)

                        select new ReporteEmpresa
                        {
                            NombreEmpresaPropietaria = A.v_RazonSocial.ToUpper(),
                            RucEmpresaPropietaria = A.v_RUC,
                            Direccion = A.v_Direccion
                        });

                    #endregion


                    return query.ToList();
                }
            }
            catch
            {
                //pobjOperationResult.Success = 0;
                //pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        #endregion
    }
}
