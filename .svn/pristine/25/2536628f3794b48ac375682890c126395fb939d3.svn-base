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
    public class NodeWarehouseBL
    {
        public List<KeyValueDTO> ObtenerAlmacenesParaCombo(ref OperationResult pobjOperationResult, string pstrSortExpression, int pIntEstablecimiento)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from a in dbContext.establecimientoalmacen

                        join b in dbContext.almacen on new {IdAlmacen = a.i_IdAlmacen.Value, eliminado = 0} 
                                 equals new {IdAlmacen = b.i_IdAlmacen, eliminado = b.i_Eliminado.Value} into b_join

                        from b in b_join.DefaultIfEmpty()

                        where a.i_Eliminado.Value == 0 && b != null &&  (a.i_IdEstablecimiento.Value == pIntEstablecimiento || pIntEstablecimiento ==-1)

                        select new
                        {
                            b.i_IdAlmacen,
                            b.v_Nombre

                        });
                    query = query.OrderBy(!string.IsNullOrEmpty(pstrSortExpression) ? pstrSortExpression : "v_Nombre");

                    var query2 = query.AsEnumerable()
                        .Select(x => new KeyValueDTO
                        {
                            Id = x.i_IdAlmacen.ToString(),
                            Value1 = int.Parse(x.i_IdAlmacen.ToString()).ToString("00") + " | " + x.v_Nombre
                        }).ToList();

                    pobjOperationResult.Success = 1;
                    return query2;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                pobjOperationResult.AdditionalInformation = "NodeWarehouseBL.ObtenerAlmacenesParaCombo()";
                return null;
            }
        }

        public List<KeyValueDTO> ObtenerAlmacenesParaComboAll(ref OperationResult pobjOperationResult, string pstrSortExpression)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    var query = from a in dbContext.almacen
                        where a.i_Eliminado == 0
                        select new
                        {
                             a.i_IdAlmacen,
                             a.v_Nombre
                        };



                    if (!string.IsNullOrEmpty(pstrSortExpression))
                    {
                        query = query.Where(pstrSortExpression);
                    }
                   
                    query = query.OrderBy("v_Nombre");
                    var query2 = query.AsEnumerable()
                        .Select(x => new KeyValueDTO
                        {
                            Id = x.i_IdAlmacen.ToString(),
                            Value1 = int.Parse(x.i_IdAlmacen.ToString()).ToString("00") + " | " + x.v_Nombre
                        }).ToList().OrderBy(o => o.Id).ToList();

                    pobjOperationResult.Success = 1;
                    return query2;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                pobjOperationResult.AdditionalInformation = "NodeWarehouseBL.ObtenerAlmacenesParaCombo()";
                return null;
            }
        }

        public List<GridKeyValueDTO> ObtenerAlmacenesParaComboGrid(ref OperationResult pobjOperationResult, string pstrSortExpression, int pintIdEmpresa)
        {
             //Para Grilla
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    //var query = (from a in dbContext.almacen
                    //            where a.i_Eliminado.Value ==0 
                    //             select a
                    //            );

                    var query = (from a in dbContext.establecimientoalmacen
                        join b in dbContext.almacen on new {IdAlmacen = a.i_IdAlmacen.Value, eliminado = 0} equals
                        new {IdAlmacen = b.i_IdAlmacen, eliminado = b.i_Eliminado.Value} into b_join
                        from b in b_join.DefaultIfEmpty()
                        where a.i_IdEstablecimiento == pintIdEmpresa && a.i_Eliminado.Value == 0
                        select b);


                    query = query.OrderBy(!string.IsNullOrEmpty(pstrSortExpression) ? pstrSortExpression : "v_Nombre");

                    var query2 = query.AsEnumerable()
                        .Select(x => new GridKeyValueDTO
                        {
                            Id = x.i_IdAlmacen.ToString(),
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

        public almacenDto ObtenerAlmacen(ref OperationResult pobjOperationResult, int pstringIdAlmacen)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    almacenDto objDtoEntity = null;
                    var objEntity = (from A in dbContext.almacen
                        where A.i_IdAlmacen == pstringIdAlmacen
                        select A
                    ).FirstOrDefault();
                    if (objEntity != null)
                        objDtoEntity = objEntity.ToDTO();

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

    }
}
