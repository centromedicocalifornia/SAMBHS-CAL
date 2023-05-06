using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.BE;

namespace SAMBHS.Almacen.BL
{
    /// <summary>
    /// Manage Table productoinventario.
    /// </summary>
    public class ProductoInventarioBL
    {
        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="pobjResult">The pobj result.</param>
        /// <param name="init">The initialize.</param>
        /// <param name="end">The end.</param>
        /// <param name="idEstablecimiento">The identifier establecimiento.</param>
        /// <param name="idAlmacen">The identifier almacen.</param>
        /// <param name="line">Line of products</param>
        /// <returns>List&lt;productoinventarioDto&gt;.</returns>
        public List<productoinventarioDto> GetData(ref OperationResult pobjResult, DateTime init, DateTime end, int idEstablecimiento, int idAlmacen, string line)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var q = (from n in dbContext.productoinventario
                             join p in dbContext.producto on n.v_IdProducto equals p.v_IdProducto into p_Join
                             from p in p_Join.DefaultIfEmpty()
                             where n.i_IdEstablecimiento == idEstablecimiento && n.i_IdAlmacen == idAlmacen
                             && n.t_Fecha >= init && n.t_Fecha <= end
                             && p.i_Eliminado == 0
                             && (p.v_IdLinea == line || line == null)
                             select new productoinventarioDto
                             {
                                 i_IdProductoInventario = n.i_IdProductoInventario,
                                 i_IdEstablecimiento = n.i_IdEstablecimiento,
                                 i_IdAlmacen = n.i_IdAlmacen,
                                 v_IdProducto = n.v_IdProducto,
                                 d_Cantidad = n.d_Cantidad,
                                 t_Fecha = n.t_Fecha,
                                 Codigo = p == null ? "" : p.v_CodInterno,
                                 Descripcion = p == null ? "" : p.v_Descripcion,
                             }).GroupBy(i => i.v_IdProducto).AsEnumerable().Select(o =>
                             {
                                 var item = o.FirstOrDefault();
                                 if (item != null)
                                 
                                    item.d_Cantidad = o.Sum(i => i.d_Cantidad);
                                 return item;
                             });
                    var res = q.ToList();
                    pobjResult.Success = 1;
                    return res;
                }
            }
            catch (Exception er)
            {
                pobjResult.Success = 0;
                pobjResult.ErrorMessage = er.Message;
                pobjResult.ExceptionMessage = er.InnerException != null ? er.InnerException.Message : string.Empty;
                pobjResult.AdditionalInformation = "ProductoInventarioBL.GetData()";
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjResult);
                return null;


               
            }
        }

        /// <summary>
        /// Adds the specified pobj result.
        /// </summary>
        /// <param name="pobjResult">The pobj result.</param>
        /// <param name="pobjDto">The pobj dto.</param>
        public void Add(ref OperationResult pobjResult, productoinventarioDto pobjDto)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var item = (from n in dbContext.productoinventario
                                where n.v_IdProducto == pobjDto.v_IdProducto  && n.i_IdEstablecimiento == pobjDto.i_IdEstablecimiento && n.i_IdAlmacen == pobjDto.i_IdAlmacen 
                                && n.t_Fecha == pobjDto.t_Fecha
                                select n).FirstOrDefault();
                    if (item == null)
                    {
                        dbContext.productoinventario.AddObject(pobjDto.ToEntity());
                    }
                    else
                    {
                        var obj = pobjDto.ToEntity();
                        obj.i_IdProductoInventario = item.i_IdProductoInventario;
                        obj.d_Cantidad += item.d_Cantidad;
                        dbContext.productoinventario.ApplyCurrentValues(obj);
                    }
                    dbContext.SaveChanges();
                    pobjResult.Success = 1;
                }
            }
            catch (Exception er)
            {
                pobjResult.Success = 0;
                pobjResult.ExceptionMessage = er.Message;
                pobjResult.ErrorMessage = er.InnerException != null ? er.InnerException.Message : null;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjResult);
            }
        }
    }
}
