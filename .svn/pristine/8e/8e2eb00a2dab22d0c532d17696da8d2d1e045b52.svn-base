using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;

namespace SAMBHS.Almacen.BL
{
    public class GastosIndirectosBL
    {
        public delegate void Error(string error);
        public event Error ErrorEvent;
        public EventHandler Terminado;
        public BackgroundWorker Bgw;
        private int _periodo;
        private int _idAlmacen;
        private int _mes;
        private bool _generarIngreso;
        public List<GastosIndirectos> DataGastosIndirectosCalculado { get; private set; }
        public List<GastosIndirectos> DataGastosIndirectosReporte { get; private set; }

        public GastosIndirectosBL()
        {
            DataGastosIndirectosCalculado = new List<GastosIndirectos>();
            DataGastosIndirectosReporte = new List<GastosIndirectos>();
            Bgw = new BackgroundWorker();
            Bgw.DoWork += Bgw_DoWork;
            Bgw.RunWorkerCompleted += Bgw_RunWorkerCompleted;
        }

        public void ComenzarAsync(int periodo, int mes, bool generarIngreso, int idAlmacen = -1)
        {
            _periodo = periodo;
            _mes = mes;
            _idAlmacen = idAlmacen;
            _generarIngreso = generarIngreso;
            Bgw.RunWorkerAsync();
        }

        private void Bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Terminado != null) Terminado(this, EventArgs.Empty);
        }

        private void Bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region Consultas

                    #region Recoleta ventas
                    var ventasOtrosTributos = (from vd in dbContext.ventadetalle
                                               join v in dbContext.venta on new { id = vd.v_IdVenta, e = 0 }
                                                   equals new { id = v.v_IdVenta, e = v.i_Eliminado.Value } into vJoin
                                               from v in vJoin.DefaultIfEmpty()
                                               join pd in dbContext.productodetalle on vd.v_IdProductoDetalle equals pd.v_IdProductoDetalle
                                                   into pdJoin
                                               from pd in pdJoin.DefaultIfEmpty()
                                               join p in dbContext.producto on pd.v_IdProducto equals p.v_IdProducto into pJoin
                                               from p in pJoin.DefaultIfEmpty()
                                               where v.t_FechaRegistro.Value.Year == _periodo
                                                       && v.t_FechaRegistro.Value.Month == _mes && vd.i_Eliminado == 0
                                                       && p.i_IndicaFormaParteOtrosTributos == 1
                                                       && vd.v_PedidoExportacion != null && vd.v_PedidoExportacion.Trim() != ""
                                               select new DataVenta
                                               {
                                                   IdTipoDocumento = v.i_IdTipoVenta ?? -1,
                                                   Serie = v.v_SerieDocumento,
                                                   Correlativo = v.v_CorrelativoDocumento,
                                                   IdProductoDetalle = vd.v_IdProductoDetalle,
                                                   Cantidad = vd.d_CantidadEmpaque ?? 0,
                                                   NroPedido = vd.v_PedidoExportacion,
                                                   CodigoProducto = p.v_CodInterno,
                                                   Descripcion = p.v_Descripcion
                                               }).ToList();

                    if (!ventasOtrosTributos.Any()) return;
                    #endregion

                    var pedidos = ventasOtrosTributos.Select(p => p.NroPedido.Trim()).Distinct().ToArray();

                    #region Recoleta diarios
                    var dataDiarios = (from dd in dbContext.diariodetalle
                                       join d in dbContext.diario on dd.v_IdDiario equals d.v_IdDiario into dJOin
                                       from d in dJOin.DefaultIfEmpty()
                                       join d1 in dbContext.documento on d.i_IdTipoDocumento equals d1.i_CodigoDocumento into d1Join
                                       from d1 in d1Join.DefaultIfEmpty()
                                       join d2 in dbContext.documento on dd.i_IdTipoDocumento equals d2.i_CodigoDocumento into d2Join
                                       from d2 in d2Join.DefaultIfEmpty()
                                       join d3 in dbContext.documento on d.i_IdTipoDocumento equals d3.i_CodigoDocumento into d3Join
                                       from d3 in d3Join.DefaultIfEmpty()
                                       where d.t_Fecha.Value.Year == _periodo && d.t_Fecha.Value.Month == _mes && dd.i_Eliminado == 0
                                           && dd.v_Pedido != null && pedidos.Contains(dd.v_Pedido.Trim()) && d.i_IdTipoDocumento == 335
                                               && dd.v_Pedido.Trim() != "" && dd.v_NroCuenta.StartsWith("6")
                                       select new GastosIndirectos
                                       {
                                           NroVoucher = d1.v_Siglas + " " + d.v_Mes + "-" + d.v_Correlativo,
                                           Cambio = dd.d_Cambio ?? 0,
                                           IdMoneda = d.i_IdMoneda ?? 1,
                                           Importe = dd.d_Importe ?? 0,
                                           NroPedido = dd.v_Pedido.Trim(),
                                           Cuenta = dd.v_NroCuenta,
                                           Fecha = d.t_Fecha.Value,
                                           IdTipoDocumento = dd.i_IdTipoDocumento ?? 1,
                                           NroDocumento = dd.v_NroDocumento != null && d2 != null ? d2.v_Siglas + " " + dd.v_NroDocumento.Trim() : string.Empty,
                                           DocumentoContabilidad = d3 != null ? d3.v_Siglas + " " + d.v_Mes + "-" + d.v_Correlativo : string.Empty,
                                           GlosaContabilidad = d.v_Glosa
                                       }).ToList();
                    #endregion

                    #region Recolecta tesorerias
                    var dataTesorerias = (from dd in dbContext.tesoreriadetalle
                                          join d in dbContext.tesoreria on dd.v_IdTesoreria equals d.v_IdTesoreria into dJOin
                                          from d in dJOin.DefaultIfEmpty()
                                          join d1 in dbContext.documento on d.i_IdTipoDocumento equals d1.i_CodigoDocumento into d1Join
                                          from d1 in d1Join.DefaultIfEmpty()
                                          join d2 in dbContext.documento on dd.i_IdTipoDocumento equals d2.i_CodigoDocumento into d2Join
                                          from d2 in d2Join.DefaultIfEmpty()
                                          join d3 in dbContext.documento on d.i_IdTipoDocumento equals d3.i_CodigoDocumento into d3Join
                                          from d3 in d3Join.DefaultIfEmpty()
                                          where
                                              d.t_FechaRegistro.Value.Year == _periodo && d.t_FechaRegistro.Value.Month == _mes &&
                                              dd.i_Eliminado == 0
                                              && pedidos.Contains(dd.v_Pedido) && dd.v_Pedido != null && dd.v_Pedido.Trim() != ""
                                              && dd.v_NroCuenta.StartsWith("6")
                                          select new GastosIndirectos
                                          {
                                              NroVoucher = d1.v_Siglas + " " + d.v_Mes + "-" + d.v_Correlativo,
                                              Cambio = dd.d_Cambio ?? 0,
                                              IdMoneda = d.i_IdMoneda ?? 1,
                                              Importe = dd.d_Importe ?? 0,
                                              NroPedido = dd.v_Pedido.Trim(),
                                              Cuenta = dd.v_NroCuenta,
                                              Fecha = d.t_FechaRegistro.Value,
                                              IdTipoDocumento = dd.i_IdTipoDocumento ?? 1,
                                              NroDocumento = dd.v_NroDocumento != null && d2 != null ? d2.v_Siglas + " " + dd.v_NroDocumento.Trim() : string.Empty,
                                              DocumentoContabilidad = d3 != null ? d3.v_Siglas + " " + d.v_Mes + "-" + d.v_Correlativo : string.Empty,
                                              GlosaContabilidad = d.v_Glosa
                                          }).ToList();
                    #endregion

                    #endregion

                    #region Genera Cálculo de Gastos Indirectos.

                    var dataDiarioTesorerias = dataDiarios.Concat(dataTesorerias).ToList();
                    DataGastosIndirectosReporte = dataDiarios.Concat(dataTesorerias).ToList();
                    var ventasDiccionario = ventasOtrosTributos.GroupBy(g => g.NroPedido)
                        .ToDictionary(k => k.Key, o => o.ToList());

                    var agrupadoConta = dataDiarioTesorerias.GroupBy(g => new
                    {
                        pedido = g.NroPedido,
                        cta = g.Cuenta
                    });

                    foreach (var data in agrupadoConta)
                    {
                        var dataContabilidad = data.FirstOrDefault();
                        if (dataContabilidad == null) return;
                        var montoContabilidad = data.Sum(p => p.ImporteSoles);
                        List<DataVenta> ventas;
                        if (ventasDiccionario.TryGetValue(data.Key.pedido, out ventas))
                        {
                            if (ventas.Any())
                            {
                                var cantTotal = ventas.Sum(p => p.Cantidad);
                                foreach (var dataVenta in ventas)
                                {
                                    var dataCalculo = dataContabilidad.Clonar();
                                    dataCalculo.IdProductoDetalle = dataVenta.IdProductoDetalle;
                                    dataCalculo.CodigoProducto = dataVenta.CodigoProducto;
                                    dataCalculo.Descripcion = dataVenta.Descripcion;
                                    dataCalculo.GastoIndirecto = montoContabilidad / cantTotal * dataVenta.Cantidad;
                                    DataGastosIndirectosCalculado.Add(dataCalculo);
                                }
                            }
                        }
                    }

                    #endregion
                }

                if (_generarIngreso)
                {
                    var rucEmpresa = Globals.ClientSession.v_RucEmpresa;
                    var idCliente = ProductoBL.DevuelveIdCliente(rucEmpresa);
                    var fechaIngreso = new DateTime(_periodo, _mes, DateTime.DaysInMonth(_periodo, _mes));
                    var pobjOperationResult = new OperationResult();
                    var agrupado = DataGastosIndirectosCalculado.GroupBy(p => p.Cuenta);

                    var listaMovimientos = new MovimientoBL().ObtenerListadoMovimientos(ref pobjOperationResult, _periodo.ToString(),
                        _mes.ToString("00"), (int)TipoDeMovimiento.NotadeIngreso);

                    var maxMovimiento = listaMovimientos.Any()
                        ? int.Parse(listaMovimientos[listaMovimientos.Count - 1].Value1) : 0;

                    foreach (var listadoIngresos in agrupado)
                    {
                        var movimientoDto = new movimientoDto();
                        maxMovimiento++;
                        movimientoDto.d_TipoCambio = 3;
                        movimientoDto.i_IdAlmacenOrigen = _idAlmacen;
                        movimientoDto.i_IdMoneda = 1;
                        movimientoDto.i_IdTipoMotivo = 1;
                        movimientoDto.t_Fecha = fechaIngreso;
                        movimientoDto.v_Mes = _mes.ToString("00");
                        movimientoDto.v_Periodo = _periodo.ToString();
                        movimientoDto.i_IdTipoMovimiento = (int)TipoDeMovimiento.NotadeIngreso;
                        movimientoDto.v_Correlativo = maxMovimiento.ToString("00000000");
                        movimientoDto.v_IdCliente = idCliente;
                        movimientoDto.v_OrigenTipo = "C";
                        movimientoDto.i_EsDevolucion = 0;
                        movimientoDto.v_OrigenRegCorrelativo = string.Empty;
                        movimientoDto.v_OrigenRegMes = string.Empty;
                        movimientoDto.v_OrigenRegPeriodo = string.Empty;
                        movimientoDto.d_TotalPrecio = listadoIngresos.Sum(p => p.ImporteSoles);
                        movimientoDto.i_IdEstablecimiento = 1;
                        movimientoDto.v_IdMovimientoOrigen = string.Format("GASINID{0}{1}", _periodo, _mes);
                        movimientoDto.v_Glosa = "PROCESO GASTIS INDIRECTOS: " + Utils.Windows.DevuelveCuentaDatos(listadoIngresos.Key).v_NombreCuenta;

                        var movimientosDetalleDto = listadoIngresos.ToList()
                            .Select(d => new movimientodetalleDto
                            {
                                v_IdProductoDetalle = d.IdProductoDetalle,
                                i_IdTipoDocumento = d.IdTipoDocumento,
                                v_NumeroDocumento = d.NroDocumento,
                                v_NroGuiaRemision = string.Empty,
                                d_Cantidad = 0,
                                i_IdUnidad = 1,
                                d_CantidadEmpaque = 0,
                                d_Precio = d.ImporteSoles,
                                d_Total = d.ImporteSoles,
                                d_CantidadAdministrativa = 0,
                                d_CantidadEmpaqueAdministrativa = 0,
                                v_NroPedido = string.Empty,
                                i_IdCentroCosto = string.Empty
                            }).ToList();

                        movimientoDto.d_TotalCantidad = movimientosDetalleDto.Sum(p => p.d_Cantidad ?? 0);
                        movimientoDto.d_TotalPrecio = movimientosDetalleDto.Sum(p => p.d_Total ?? 0);
                        new MovimientoBL().InsertarMovimiento(ref pobjOperationResult, movimientoDto,
                            Globals.ClientSession.GetAsList(), movimientosDetalleDto);
                        if (pobjOperationResult.Success == 0) return;
                    }
                }

            }
            catch (Exception exception)
            {
                if (ErrorEvent != null) ErrorEvent(exception.Message);
            }
        }

        private class DataVenta
        {
            public int IdTipoDocumento { get; set; }
            public string Serie { get; set; }
            public string Correlativo { get; set; }
            public string IdProductoDetalle { get; set; }
            public string CodigoProducto { get; set; }
            public string Descripcion { get; set; }
            public decimal Cantidad { get; set; }
            public string NroPedido { get; set; }
        }

        public class GastosIndirectos
        {
            public string NroVoucher { get; set; }
            public string Cuenta { get; set; }
            public int IdTipoDocumento { get; set; }
            public string NroDocumento { get; set; }
            public DateTime Fecha { get; set; }
            public String FechaView
            {
                get { return Fecha.ToShortDateString(); }
            }
            public decimal Importe { get; set; }
            public decimal Cambio { get; set; }
            public int IdMoneda { get; set; }
            public string NroPedido { get; set; }
            public decimal ImporteSoles
            {
                get
                {
                    return IdMoneda == 1 ? Importe : Cambio;
                }
            }
            public decimal GastoIndirecto { get; set; }
            public string IdProductoDetalle { get; set; }
            public string CodigoProducto { get; set; }
            public string Descripcion { get; set; }
            public string DocumentoContabilidad { get; set; }
            public string GlosaContabilidad { get; set; }

            public GastosIndirectos Clonar()
            {
                return (GastosIndirectos)MemberwiseClone();
            }
        }
    }
}
