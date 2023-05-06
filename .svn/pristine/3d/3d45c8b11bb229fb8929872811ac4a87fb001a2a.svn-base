using System;
using System.Collections.Generic;
using System.Linq;
using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;

namespace SAMBHS.Almacen.BL
{
    /// <summary>
    /// Manage Table productoisc
    /// </summary>
    public class ProductoIscBL
    {
        public productoiscDto FromProduct(ref OperationResult pobjOperationResult, string pstrIdProducto,
            string pstrPeriodo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var item = (from n in dbContext.productoisc
                        where n.v_IdProducto == pstrIdProducto && n.v_Periodo == pstrPeriodo
                        select new productoiscDto
                        {
                            i_IdProductoIsc = n.i_IdProductoIsc,
                            i_IdSistemaIsc = n.i_IdSistemaIsc,
                            d_Porcentaje = n.d_Porcentaje,
                            d_Monto = n.d_Monto,
                            v_Periodo = n.v_Periodo,
                            i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                            t_InsertaFecha = n.t_InsertaFecha
                        }).FirstOrDefault();
                    pobjOperationResult.Success = 1;
                    return item;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public void Add(ref OperationResult pobjOperationResult, productoiscDto pobjDto, List<string> clientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var objEntity = pobjDto.ToEntity();
                    objEntity.t_InsertaFecha = DateTime.Now;
                    objEntity.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                    dbContext.productoisc.AddObject(objEntity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.AdditionalInformation = "ProductoIscBL.Add()";
                pobjOperationResult.ExceptionMessage = ex.Message;
            }
        }

        public void Update(ref OperationResult pobjOperationResult, productoiscDto pobjDto, List<string> clientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var source = (from a in dbContext.productoisc
                        where a.i_IdProductoIsc == pobjDto.i_IdProductoIsc
                        select a).FirstOrDefault();
                    var objEntity = pobjDto.ToEntity();
                    objEntity.t_ActualizaFecha = DateTime.Now;
                    objEntity.i_ActualizaIdUsuario = int.Parse(clientSession[2]);

                    dbContext.productoisc.ApplyCurrentValues(objEntity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.AdditionalInformation = "ProductoIscBL.Update()";
                pobjOperationResult.ExceptionMessage = ex.Message;
            }
        }

        public productoiscDto FromProductDetail(ref OperationResult pobjOperationResult, string pstrIdProductoDetail,
            string pstrPeriodo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var item = (
                                from p in dbContext.productodetalle
                                join n in dbContext.productoisc on p.v_IdProducto equals n.v_IdProducto
                                where p.v_IdProductoDetalle == pstrIdProductoDetail && n.v_Periodo == pstrPeriodo
                                select new productoiscDto
                                {
                                    i_IdProductoIsc = n.i_IdProductoIsc,
                                    i_IdSistemaIsc = n.i_IdSistemaIsc,
                                    d_Porcentaje = n.d_Porcentaje,
                                    d_Monto = n.d_Monto,
                                }).FirstOrDefault();
                    pobjOperationResult.Success = 1;
                    return item;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }
    }
}
