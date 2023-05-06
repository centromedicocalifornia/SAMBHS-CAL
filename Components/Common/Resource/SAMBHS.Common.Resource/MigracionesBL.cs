using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAMBHS.Common;
using SAMBHS.Common.DataModel;
using System.Transactions;
using SAMBHS.Common.BE;
using System.Data.Objects;
namespace SAMBHS.Common.Resource
{
    public class MigracionesBL
    {
        public List<string> DevuelveDuplicados(List<string> pLista)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var Productos = (from p in dbContext.producto
                                     where p.i_Eliminado == 0
                                     select new 
                                     {
                                         p.v_CodInterno
                                     }
                                     ).ToList();

                    List<string> ListaProductos = Productos.Select(p => p.v_CodInterno.Trim()).ToList();

                    var Result = pLista.Except(ListaProductos);

                    return Result.ToList();
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public List<string> DevuelveDuplicadosClientesProveedores(List<string> pLista, string flag)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var Productos = (from p in dbContext.cliente
                                     where p.i_Eliminado == 0 && p.v_FlagPantalla == flag
                                     select new
                                     {
                                         p.v_CodCliente
                                     }
                                     ).ToList();

                    List<string> ListaProductos = Productos.Select(p => p.v_CodCliente.Trim()).ToList();

                    var Result = pLista.Except(ListaProductos);

                    return Result.ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
