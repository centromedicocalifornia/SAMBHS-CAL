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
    public class WarehouseBL
    {
        public warehouseDto GetwarehouseBywarehouseId(ref OperationResult pobjOperationResult, int pstrwarehouseId)
        {
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                warehouseDto objDtoEntity = null;

                var objEntity = (from a in dbContext.warehouse
                                 where a.i_WarehouseId == pstrwarehouseId
                                 select a).FirstOrDefault();

                if (objEntity != null)
                    objDtoEntity = warehouseAssembler.ToDTO(objEntity);

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

        public List<warehouseDto> GetwarehousePagedAndFiltered(ref OperationResult pobjOperationResult, int? pintPageIndex, int? pintResultsPerPage, string pstrSortExpression, string pstrFilterExpression)
        {

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                var query = (from n in dbContext.warehouse

                             join su2 in dbContext.systemuser on new { i_InsertUserId = n.i_InsertUserId.Value }
                                                           equals new { i_InsertUserId = su2.i_SystemUserId } into su2_join
                             from su2 in su2_join.DefaultIfEmpty()

                             join su3 in dbContext.systemuser on new { i_UpdateUserId = n.i_UpdateUserId.Value }
                                                           equals new { i_UpdateUserId = su3.i_SystemUserId } into su3_join
                             from su3 in su3_join.DefaultIfEmpty()
                             where n.i_IsDeleted == 0


                             select new warehouseDto
                             {
                                 i_WarehouseId = n.i_WarehouseId,
                                 v_Name = n.v_Name,
                                 v_Address = n.v_Address,
                                 v_CommercialName = n.v_CommercialName,
                                 v_TicketSerialNumber = n.v_TicketSerialNumber,
                                 v_EstablishmentCode = n.v_EstablishmentCode,
                                 v_PhoneNumber = n.v_PhoneNumber,
                                 d_InsertDate = n.d_InsertDate,
                                 d_UpdateDate = n.d_UpdateDate,
                                 v_InsertUser = su2.v_UserName,
                                 v_UpdateUser = su3.v_UserName

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

                List<warehouseDto> objData = query.ToList();
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

        public int GetwarehouseCount(ref OperationResult pobjOperationResult, string filterExpression)
        {
            //mon.IsActive = true;
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                var query = from a in dbContext.warehouse select a;

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

        public void Addwarehouse(ref OperationResult pobjOperationResult, warehouseDto pobjDtoEntity, List<string> ClientSession)
        {

            string newId = string.Empty;
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                warehouse objEntity = warehouseAssembler.ToEntity(pobjDtoEntity);

                objEntity.d_InsertDate = DateTime.Now;
                objEntity.i_InsertUserId = Int32.Parse(ClientSession[2]);
                objEntity.i_IsDeleted = 0;

               
                // Autogeneramos el Pk de la tabla

                dbContext.AddTowarehouse(objEntity);
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

        public void Updatewarehouse(ref OperationResult pobjOperationResult, warehouseDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.warehouse
                                       where a.i_WarehouseId == pobjDtoEntity.i_WarehouseId
                                       select a).FirstOrDefault();

                var NodeWarehouse = (from a in dbContext.nodewarehouse

                                    join J1 in dbContext.node on a.i_NodeId equals J1.i_NodeId into J1_join
                                    from J1 in J1_join.DefaultIfEmpty()

                                    join J2 in dbContext.warehouse on a.i_WarehouseId equals J2.i_WarehouseId into J2_join
                                    from J2 in J2_join.DefaultIfEmpty()

                                    where a.i_WarehouseId == objEntitySource.i_WarehouseId
                                    select new { J1.v_RUC, J2.v_Name, J2.v_Address }).FirstOrDefault();

                // Crear la entidad con los datos actualizados
                pobjDtoEntity.d_UpdateDate = DateTime.Now;
                pobjDtoEntity.i_UpdateUserId = Int32.Parse(ClientSession[2]);

                warehouse objEntity = warehouseAssembler.ToEntity(pobjDtoEntity);

                // Copiar los valores desde la entidad actualizada a la Entidad Fuente
                dbContext.warehouse.ApplyCurrentValues(objEntity);

                // Guardar los cambios
                dbContext.SaveChanges();

                if (NodeWarehouse != null)
                {
                    //dbContext.spe_almacen_empresa(NodeWarehouse.v_RUC, objEntitySource.i_WarehouseId, NodeWarehouse.v_Name, NodeWarehouse.v_Address);
                    //dbContext.spi_almacen_empresa(NodeWarehouse.v_RUC, objEntitySource.i_WarehouseId, NodeWarehouse.v_Name, Int32.Parse(ClientSession[2]), NodeWarehouse.v_Address, DateTime.Today.ToShortDateString());
                }

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

        public void Deletewarehouse(ref OperationResult pobjOperationResult, int pstrwarehouseId, List<string> ClientSession)
        {
            //mon.IsActive = true;
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.warehouse
                                       where a.i_WarehouseId == pstrwarehouseId
                                       select a).FirstOrDefault();

                // Crear la entidad con los datos actualizados
                objEntitySource.d_UpdateDate = DateTime.Now;
                objEntitySource.i_UpdateUserId = Int32.Parse(ClientSession[2]);
                objEntitySource.i_IsDeleted = 1;


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
    }
}
