using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Devart.Data.PostgreSql;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using Dapper;
using SAMBHS.Common.DataModel;

namespace SAMBHS.Almacen.BL
{
    /// <summary>
    /// Super clase que implementa las bases para el calculo de stock y separación.
    /// </summary>
    public abstract class CalculoAlmacenBl
    {
        protected CalculoAlmacenBl()
        {
            BkRecalcularWorker = new BackgroundWorker();
            BkRecalcularWorker.DoWork += _bkWorker_DoWork;
            BkRecalcularWorker.RunWorkerCompleted += delegate
            {
                if (TerminadoEvent != null) TerminadoEvent();
            };
        }

        public delegate void Terminado();
        public event Terminado TerminadoEvent;
        public string Periodo { get; set; }
        public int Almacen { get; set; }
        public BackgroundWorker BkRecalcularWorker;

        public abstract void _bkWorker_DoWork(object sender, DoWorkEventArgs e);

        public void IniciarProcesoAsync()
        {
            BkRecalcularWorker.RunWorkerAsync();
        }

        protected virtual void OnTerminadoEvent()
        {
            var handler = TerminadoEvent;
            if (handler != null) handler();
        }

    }

    /// <summary>
    /// Clase que se encarga de gestionar el recalculo de stock de los productos, hereda las propiedades de la clase CalculoAlmacenBl
    /// </summary>
    public class StockBl : CalculoAlmacenBl
    {
        public override void _bkWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var objOperationResult = new OperationResult();
            RecalcularStock(ref objOperationResult, Periodo, Almacen);
        }

        private static void SetFullProcess(string periodo, int idAlmacen = -1)
        {
            try
            {
                #region Realiza consulta

                var queryToExcecute = "update productoalmacen set \"d_StockActual\" = 0 " +
                                      "where (-1 = " + idAlmacen + " or \"i_IdAlmacen\" = " + idAlmacen + ") " +
                                      "and \"v_Periodo\" = '" + periodo + "'";


                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entityConnection = dbContext.Connection as System.Data.EntityClient.EntityConnection;
                    if (entityConnection != null)
                    {
                        var cnx = entityConnection.StoreConnection;
                        cnx.Open();
                        var trans = cnx.BeginTransaction();
                        try
                        {
                            var cmd = cnx.CreateCommand();
                            cmd.Transaction = trans;
                            cmd.CommandText = queryToExcecute;
                            cmd.ExecuteNonQuery();
                            trans.Commit();
                        }
                        catch
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string QueryConsulta(string periodo, int idAlmacen = -1)
        {
            try
            {
                var GuiaCompra = Constants.OrigenGuiaCompra;
                var query = "select md.\"v_IdProductoDetalle\" as \"IdProductoDetalle\", " +
                            "md.\"d_CantidadEmpaque\" as \"Cantidad\", " +
                            "md.\"v_NroSerie\" as \"Serie\", " +
                            "md.\"v_NroLote\" as \"NroLote\", " +
                            "md.\"t_FechaCaducidad\" as \"FechaCaducidad\", " +
                            "m.\"i_IdAlmacenOrigen\" as \"Almacen\", " +
                            "CASE m.\"i_IdTipoMovimiento\" " +
                            "WHEN 1 THEN " +
                            "CASE m.\"i_EsDevolucion\" WHEN 1 THEN 'S' ELSE 'I' END " +
                            "ELSE " +
                            "CASE m.\"i_EsDevolucion\" WHEN 1 THEN 'I' ELSE 'S' END " +
                            "END as \"TipoMovimientoReal\" " +
                            "from movimientodetalle md " +
                            "join movimiento m on md.\"v_IdMovimiento\" = m.\"v_IdMovimiento\" " +
                            "where \"v_Periodo\" = '" + periodo + "' and md.\"i_Eliminado\" = 0 " +
                            "and m.\"i_IdTipoMovimiento\" <> 3 " +
                            "and (-1 = " + idAlmacen + " or m.\"i_IdAlmacenOrigen\" = " + idAlmacen + ") " +
                            "order by md.\"v_IdProductoDetalle\"";

                return query;

            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string QueryproductoalmacenDto(string periodo, int idAlmacen)
        {
            return "select \"v_IdProductoAlmacen\", \"d_StockActual\", \"i_IdAlmacen\", \"v_ProductoDetalleId\", \"t_FechaCaducidad\", \"v_NroSerie\", \"v_NroLote\" from productoalmacen " +
                   "where \"i_Eliminado\" = 0  " +
                   "and (-1 = " + idAlmacen + " or \"i_IdAlmacen\" = " + idAlmacen + ")  " +
                   "and \"v_Periodo\" = '" + periodo + "' " +
                   "and \"v_ProductoDetalleId\" in ( " +
                   "select \"v_IdProductoDetalle\" from productodetalle pd " +
                   "left join producto p on pd.\"v_IdProducto\" = p.\"v_IdProducto\" and pd.\"i_Eliminado\" = 0 " +
                   "where p.\"i_Eliminado\" = 0 and p.\"i_EsActivo\" = 1) ";
        }

        private static IEnumerable<ConsultaStockProductos> ObtenerStock(ref OperationResult pobjOperationResult, string periodo, int idAlmacen = -1)
        {
            try
            {
                List<ConsultaStockProductos> consulta = new List<ConsultaStockProductos>();

                #region Realiza consulta

                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entityConnection = dbContext.Connection as System.Data.EntityClient.EntityConnection;
                    if (entityConnection != null)
                    {
                        var cnx = entityConnection
                            .StoreConnection;
                        if (cnx.State == ConnectionState.Closed) cnx.Open();
                        consulta = cnx.Query<ConsultaStockProductos>(QueryConsulta(periodo, idAlmacen)).ToList();
                        cnx.Close();
                    }
                }

                #endregion

                if (!consulta.Any()) return null;

                var agrupado = consulta.GroupBy(g => new { p = g.Key, a = g.Almacen });

                var resultado = agrupado.Select(g =>
                {
                    var reg = g.FirstOrDefault();
                    var ingresos = g.Where(p => p.TipoMovimientoReal.Equals("I")).Sum(s => s.Cantidad);
                    var salidas = g.Where(p => p.TipoMovimientoReal.Equals("S")).Sum(s => s.Cantidad);
                    return new ConsultaStockProductos
                    {
                        Cantidad = ingresos - salidas,
                        TipoMovimientoReal = "*",
                        Almacen = g.Key.a,
                        IdProductoDetalle = reg.IdProductoDetalle,
                        Serie = reg.Serie,
                        NroLote = reg.NroLote,
                        FechaCaducidad = reg.FechaCaducidad
                    };
                }).ToList();

                pobjOperationResult.Success = 1;
                return resultado;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ConsultarStockBL.ObtenerStock()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        private static IEnumerable<productoalmacenDto> ObtenerproductoalmacenDto(int idAlmacen, string periodo)
        {
            try
            {
                List<productoalmacenDto> consulta = new List<productoalmacenDto>();

                #region Realiza consulta

                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entityConnection = dbContext.Connection as System.Data.EntityClient.EntityConnection;
                    if (entityConnection != null)
                    {
                        var cnx = entityConnection
                            .StoreConnection;
                        if (cnx.State == ConnectionState.Closed) cnx.Open();
                        consulta = cnx.Query<productoalmacenDto>(QueryproductoalmacenDto(periodo, idAlmacen)).ToList();
                        cnx.Close();
                    }

                }

                #endregion

                return consulta;
            }
            catch
            {
                return null;
            }
        }

        public void RecalcularStock(ref OperationResult pobjOperationResult, string periodo, int idAlmacen = -1, bool fullProcess = false)
        {
            try
            {
                pobjOperationResult.Success = 1;
                if (fullProcess) SetFullProcess(periodo, idAlmacen);
                var listadoUpdate = new List<Tuple<string, decimal>>();
                var stocks = ObtenerStock(ref pobjOperationResult, periodo, idAlmacen);
                if (stocks == null) return;

                var productoalmacenDtoes = ObtenerproductoalmacenDto(idAlmacen, periodo).ToList();
                var stocksAgrupado = stocks.GroupBy(g => new { almacen = g.Almacen, key = g.Key });
                foreach (var stock in stocksAgrupado)
                {
                    var almacen = stock.Key.almacen;
                    var key = stock.Key.key;

                    var diccionarioStocks = stock.ToDictionary(k => k.Key, o => o.Cantidad);
                    var productoPorAlmacen = productoalmacenDtoes.Where(p => p.i_IdAlmacen == almacen && p.Key == key);
                    foreach (var prodAlmacen in productoPorAlmacen)
                    {
                        decimal cant;
                        cant = diccionarioStocks.TryGetValue(prodAlmacen.Key, out cant) ? cant : 0m;
                        if (cant == (prodAlmacen.d_StockActual ?? 0)) continue;
                        listadoUpdate.Add(new Tuple<string, decimal>(prodAlmacen.v_IdProductoAlmacen, cant));
                    }
                }

                #region Guarda cambios

                if (listadoUpdate.Any())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var entityConnection = dbContext.Connection as System.Data.EntityClient.EntityConnection;
                        if (entityConnection != null)
                        {
                            var cnx = entityConnection.StoreConnection;
                            if (cnx.State == ConnectionState.Closed) cnx.Open();
                            var t = cnx.BeginTransaction();
                            foreach (var tuple in listadoUpdate)
                                cnx.Execute("Update productoalmacen set \"d_StockActual\" = " + tuple.Item2 +
                                            " where \"v_IdProductoAlmacen\" = '" + tuple.Item1 + "'", transaction: t);
                            t.Commit();
                            cnx.Close();
                        }
                    }
                }

                #endregion

                Globals.ActualizadoStocks = true;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ConsultarStockBL.RecalcularStock()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }
    }

    /// <summary>
    /// Clase que se encarga de gestionar el recalculo de la separación de los productos, hereda las propiedades de la clase CalculoAlmacenBl
    /// </summary>
    public class SeparacionBl : CalculoAlmacenBl
    {
        public override void _bkWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var objOperation = new OperationResult();
            RecalcularSeparacion(ref objOperation, Periodo, Almacen);
        }

        private static string QueryPedido(string periodo, int idAlmacen = -1)
        {
            try
            {
                var query = "select \"v_SerieDocumento\" as \"SeriePedido\", \"v_CorrelativoDocumento\" as \"CorrelativoPedido\",  " +
                            "pd.\"v_IdProductoDetalle\" as \"ProductoDetalle\", \"d_CantidadEmpaque\" as \"Cantidad\", \"i_IdAlmacen\" as \"Almacen\"  " +
                            "from pedidodetalle pd " +
                            "join pedido p on pd.\"v_IdPedido\" = p.\"v_IdPedido\" " +
                            "join productodetalle ppd on pd.\"v_IdProductoDetalle\" = ppd.\"v_IdProductoDetalle\" " +
                            "join producto pr on ppd.\"v_IdProducto\" = pr.\"v_IdProducto\" " +
                            "where p.\"v_Periodo\" = '" + periodo + "' and pd.\"i_Eliminado\" = 0 and pr.\"i_EsServicio\" = 0 " +
                            "and p.\"i_IdTipoDocumento\" = 430 and (-1 = " + idAlmacen + " or pd.\"i_IdAlmacen\" = " + idAlmacen + ") and p.\"i_IdEstado\" <> 3";

                return query;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string QueryVenta(string periodo, int idAlmacen = -1)
        {
            return "select vd.\"v_IdProductoDetalle\" as \"ProductoDetalle\", " +
                   "v.\"v_NroPedido\" as \"NroPedido\",  " +
                   "case  " +
                   "    when v.\"i_IdTipoDocumento\" <> 7  " +
                   "    then vd.\"d_CantidadEmpaque\"  " +
                   "    else vd.\"d_CantidadEmpaque\" * -1  " +
                   "    end as \"Cantidad\", " +
                   "vd.\"i_IdAlmacen\" as \"Almacen\"  " +
                   "from ventadetalle vd " +
                   "join venta v on vd.\"v_IdVenta\" = v.\"v_IdVenta\" " +
                   "join productodetalle pd on vd.\"v_IdProductoDetalle\" = pd.\"v_IdProductoDetalle\" " +
                   "join producto p on pd.\"v_IdProducto\" = p.\"v_IdProducto\" " +
                   "where v.\"i_IdEstado\" = 1 and vd.\"i_Eliminado\" = 0 " +
                   "and v.\"v_Periodo\" = '" + periodo + "' and rtrim(ltrim(v.\"v_NroPedido\")) <> '' " +
                   "and p.\"i_EsServicio\" = 0 and (-1 = " + idAlmacen + " or vd.\"i_IdAlmacen\" = " + idAlmacen + ") ";
        }

        private static string QueryAlmacen(string periodo, int idAlmacen = -1)
        {
            return "select pa.\"v_IdProductoAlmacen\" as \"ProductoAlmacen\", pa.\"v_ProductoDetalleId\" as \"ProductoDetalle\",  " +
                    "pa.\"i_IdAlmacen\" as \"Almacen\",  " +
                    "pa.\"d_SeparacionTotal\" as \"Separacion\"  " +
                    "from productoalmacen pa " +
                    "where pa.\"i_Eliminado\" = 0 and (-1 = " + idAlmacen + " or pa.\"i_IdAlmacen\" = " + idAlmacen + " ) " +
                    "and pa.\"v_Periodo\" = '" + periodo + "' ";
        }

        private static void SetFullProcess(string periodo, int idAlmacen = -1)
        {
            try
            {
                #region Realiza consulta

                var queryToExcecute = "update productoalmacen set \"d_SeparacionTotal\" = 0 " +
                                      "where (-1 = " + idAlmacen + " or \"i_IdAlmacen\" = " + idAlmacen + ") " +
                                      "and \"v_Periodo\" = '" + periodo + "'";

                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entityConnection = dbContext.Connection as System.Data.EntityClient.EntityConnection;
                    if (entityConnection != null)
                    {
                        var cnx = entityConnection
                            .StoreConnection;
                        if (cnx.State == ConnectionState.Closed) cnx.Open();
                        cnx.Execute(queryToExcecute);
                        cnx.Close();
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static IEnumerable<DataPedido> ObtenerPedidos(string periodo, int idAlmacen = -1)
        {
            try
            {
                List<DataPedido> consulta = new List<DataPedido>();

                #region Realiza consulta

                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entityConnection = dbContext.Connection as System.Data.EntityClient.EntityConnection;
                    if (entityConnection != null)
                    {
                        var cnx = entityConnection.StoreConnection;
                        if (cnx.State == ConnectionState.Closed) cnx.Open();
                        consulta = cnx.Query<DataPedido>(QueryPedido(periodo, idAlmacen)).ToList();
                        cnx.Close();
                    }
                }

                #endregion

                return consulta;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static IEnumerable<DataVenta> ObtenerVentas(string periodo, int idAlmacen = -1)
        {
            try
            {
                List<DataVenta> consulta = new List<DataVenta>();

                #region Realiza consulta
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entityConnection = dbContext.Connection as System.Data.EntityClient.EntityConnection;
                    if (entityConnection != null)
                    {
                        var cnx = entityConnection.StoreConnection;
                        if (cnx.State == ConnectionState.Closed) cnx.Open();
                        consulta = cnx.Query<DataVenta>(QueryVenta(periodo, idAlmacen)).ToList();
                        cnx.Close();
                    }
                }

                #endregion

                return consulta;
            }
            catch
            {
                return null;
            }
        }

        private static IEnumerable<DataSeparacion> ObtenerSeparacion(string periodo, int idAlmacen = -1)
        {
            try
            {
                List<DataSeparacion> consulta = new List<DataSeparacion>();

                #region Realiza consulta
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entityConnection = dbContext.Connection as System.Data.EntityClient.EntityConnection;
                    if (entityConnection != null)
                    {
                        var cnx = entityConnection.StoreConnection;
                        if (cnx.State == ConnectionState.Closed) cnx.Open();
                        consulta = cnx.Query<DataSeparacion>(QueryAlmacen(periodo, idAlmacen)).ToList();
                        cnx.Close();
                    }
                }

                #endregion

                return consulta;
            }
            catch
            {
                return null;
            }
        }

        public void RecalcularSeparacion(ref OperationResult pobjOperationResult, string periodo, int idAlmacen = -1, bool fullProcess = false)
        {
            var listadoUpdate = new List<Tuple<string, decimal>>();
            try
            {
                if (fullProcess) SetFullProcess(periodo, idAlmacen);
                pobjOperationResult.Success = 1;
                var pedidos = ObtenerPedidos(periodo, idAlmacen).ToList();
                var ventas = ObtenerVentas(periodo, idAlmacen).ToList();
                var separacion = ObtenerSeparacion(periodo, idAlmacen).ToList();
                var agrupadoProducto = pedidos.GroupBy(g => new { prod = g.ProductoDetalle, almacen = g.Almacen });
                var pedidosProducto = pedidos.GroupBy(g => g.ProductoDetalle).ToDictionary(k => k.Key, o => o.ToList());
                var separacionAlmacen = separacion.ToDictionary(k => new { p = k.ProductoDetalle, a = k.Almacen }, o => o);
                var ventaRelacion = ventas.GroupBy(g => new { ped = g.KeyPedido, prod = g.ProductoDetalle, alm = g.Almacen })
                                          .ToDictionary(k => k.Key, o => o.ToList());

                foreach (var lstPed in agrupadoProducto)
                {
                    List<DataPedido> pedRelacionados;

                    if (pedidosProducto.TryGetValue(lstPed.Key.prod, out pedRelacionados))
                    {
                        var ventasRelacionadas = new List<DataVenta>();
                        var nroPedidosRelacionados = pedRelacionados.Select(p => p.KeyPedido);
                        foreach (var pedRel in nroPedidosRelacionados)
                        {
                            List<DataVenta> v;
                            if (ventaRelacion.TryGetValue(new { ped = pedRel, lstPed.Key.prod, alm = lstPed.Key.almacen }, out v))
                                ventasRelacionadas.AddRange(v);
                        }

                        #region Calcula la separacion real.
                        DataSeparacion d;
                        var cantidadPedidos = lstPed.Sum(p => p.Cantidad);
                        var cantidadVentas = ventasRelacionadas.Sum(p => p.Cantidad);
                        var separacionReal = cantidadPedidos - cantidadVentas;
                        separacionReal = separacionReal >= 0 ? separacionReal : 0;

                        if (!separacionAlmacen.TryGetValue(new { p = lstPed.Key.prod, a = lstPed.Key.almacen }, out d)) continue;
                        if (separacionReal != d.Separacion)
                            listadoUpdate.Add(Tuple.Create(d.ProductoAlmacen, separacionReal));
                        #endregion
                    }

                }

                #region Guarda los cambios
                if (listadoUpdate.Any())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var entityConnection = dbContext.Connection as System.Data.EntityClient.EntityConnection;
                        if (entityConnection != null)
                        {
                            var cnx = entityConnection.StoreConnection;
                            if (cnx.State == ConnectionState.Closed) cnx.Open();
                            var t = cnx.BeginTransaction();
                            foreach (var tuple in listadoUpdate)
                                cnx.Execute("Update productoalmacen set \"d_SeparacionTotal\" = " + tuple.Item2 +
                                            " where \"v_IdProductoAlmacen\" = '" + tuple.Item1 + "'", transaction: t);
                            t.Commit();
                            cnx.Close();
                        }

                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "SeparacionBl.RecalcularSeparacion()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        private struct DataPedido
        {
            private string SeriePedido { get; set; }
            private string CorrelativoPedido { get; set; }
            public string ProductoDetalle { get; set; }
            public decimal Cantidad { get; set; }
            public int Almacen { get; set; }
            public string KeyPedido
            {
                get
                {
                    return string.Format("{0}-{1}", SeriePedido.Trim(), CorrelativoPedido.Trim());
                }
            }
        }

        private struct DataVenta
        {
            private string NroPedido { get; set; }
            public string ProductoDetalle { get; set; }
            public decimal Cantidad { get; set; }
            public int Almacen { get; set; }
            public string KeyPedido
            {
                get
                {
                    try
                    {
                        int i;
                        var sp = NroPedido.Split('-').Select(p => p.Trim()).ToArray();
                        if (sp.Length != 2) return null;
                        var serie = (int.TryParse(sp[0], out i) ? i : 0).ToString("0000");
                        var correlativo = (int.TryParse(sp[1], out i) ? i : 0).ToString("00000000");
                        return string.Format("{0}-{1}", serie, correlativo);
                    }
                    catch (Exception)
                    {

                        throw;
                    }

                }
            }
        }

        private struct DataSeparacion
        {
            public string ProductoDetalle { get; set; }
            public int Almacen { get; set; }
            public decimal Separacion { get; set; }
            public string ProductoAlmacen { get; set; }
        }
    }

    /// <summary>
    /// Clase que se encarga de realizar el reproceso de stock y separación consecutivamente.
    /// </summary>
    public class StockSeparacionBl
    {
        public event EventHandler Terminado;
        public void IniciarProceso(ref OperationResult pobjOperationResult, string periodo, int almacen)
        {
            try
            {
                pobjOperationResult.Success = 1;
                var stockProceso = new StockBl { Periodo = periodo, Almacen = almacen };
                var separacionProceso = new SeparacionBl { Periodo = periodo, Almacen = almacen };
                stockProceso.TerminadoEvent += delegate { separacionProceso.IniciarProcesoAsync(); };
                separacionProceso.TerminadoEvent += delegate { if (Terminado != null) Terminado(this, EventArgs.Empty); };
                stockProceso.IniciarProcesoAsync();
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "StockSeparacionBl.IniciarProceso()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }
    }
}
