﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.Compra.BL;
using SAMBHS.Venta.BL;
using SAMBHS.CommonWIN.BL;


namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class FrmExtraerDetallesMovimientos : Form
    {
        public BindingList<ventadetalleDto> ListaRetornoVentas { get; set; }
        public string IdPedido { get; set; }
        public List<string> ListaGuiasPorAnular { get; set; }
        public bool AnularGuias { get; set; }
        public bool AnularVentas { get; set; }
        private readonly DocumentoBL _objDocumentoBl = new DocumentoBL();
        public FrmExtraerDetallesMovimientos()
        {
            InitializeComponent();
            chkCalcularDespachoPedido.Checked = UserConfig.Default.RealizarCalculoDespachoPedido;
            chkAnularGuias.Checked = UserConfig.Default.AnularGuiaInternaExtraccion;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            #region Ventas

            if (rbVentas.Checked)
            {

                var mes = int.Parse(dtpFechaRegistroDe.SelectedValue.ToString());
                var fechaIni = new DateTime(int.Parse(cboPeriodo.Text), mes, 1);
                var fechaFin = new DateTime(int.Parse(cboPeriodo.Text), mes, DateTime.DaysInMonth(int.Parse(cboPeriodo.Text), mes), 23, 59, 0);
                var objOperationResult = new OperationResult();
                var objData = new List<ventaDto>();
                Task.Factory.StartNew(() =>
                {
                    lock (this)
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            pBuscando.Visible = true;
                            ultraGrid1.Enabled = false;
                            btnBuscar.Enabled = false;
                        });
                        objData =
                            new VentaBL().ListarBusquedaVentas(ref objOperationResult, null, null, fechaIni, fechaFin, idEstablecimiento: Globals.ClientSession.i_IdEstablecimiento ?? -1)
                                .ToList();
                    }
                },
                TaskCreationOptions.LongRunning)
                .ContinueWith(t =>
                {
                    pBuscando.Visible = false;
                    ultraGrid1.Enabled = true;
                    btnBuscar.Enabled = true;
                    if (objOperationResult.Success != 1)
                    {
                        UltraMessageBox.Show(
                            "Error en operación:" + Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var ds = objData.Select(fila =>
                    {
                        try
                        {
                            return new BusquedaExtraccionDto
                            {
                                ID = fila.v_IdVenta,
                                FechaDocumento = fila.t_FechaRegistro,
                                NombreCliente = fila.NroDocCliente + "   " + fila.NombreCliente,
                                NroDocumento = fila.v_SerieDocumento + "-" + fila.v_CorrelativoDocumento,
                                TipoDocumento = fila.TipoDocumento,
                                Estado = fila.i_IdEstado ?? 0,
                                Moneda = fila.Moneda,
                                Total = fila.d_Total ?? 0,
                                iTipoDocumento = fila.i_IdTipoDocumento ?? -1,
                                Origen = fila.Origen,
                            };
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            return null;
                        }
                    }).ToList().OrderBy(x => x.iTipoDocumento).ThenBy(x => x.NroDocumento).ToList();

                    ultraGrid1.DataSource = ds;
                    ultraGrid1.Rows.ToList().ForEach(fila => fila.Cells["_Check"].Value = false);
                },
                TaskScheduler.FromCurrentSynchronizationContext());
            }
            #endregion

            #region Guía Remisión

            if (rbGuiaRemision.Checked)
            {
                var mes = int.Parse(dtpFechaRegistroDe.SelectedValue.ToString());
                //var fechaIni = new DateTime(Globals.ClientSession.i_Periodo ?? DateTime.Now.Year, mes, 1);
                //var fechaFin = new DateTime(Globals.ClientSession.i_Periodo ?? DateTime.Now.Year, mes, DateTime.DaysInMonth(Globals.ClientSession.i_Periodo ?? DateTime.Now.Year, mes), 23, 59, 0);
                var filters = new Queue<string>();
                var fechaIni = new DateTime(int.Parse(cboPeriodo.Text.ToString()), mes, 1);
                var fechaFin = new DateTime(int.Parse(cboPeriodo.Text.ToString()), mes, DateTime.DaysInMonth(int.Parse(cboPeriodo.Text.ToString()), mes), 23, 59, 0);
                var objOperationResult = new OperationResult();
                var objData = new List<guiaremisionDto>();
                filters.Enqueue("i_IdEstablecimiento==" + (Globals.ClientSession.i_IdEstablecimiento ?? 1));
                string _strFilterExpression = "";
                _strFilterExpression = string.Join(" && ", filters);
                Task.Factory.StartNew(() =>
                {
                    lock (this)
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            pBuscando.Visible = true;
                            ultraGrid1.Enabled = false;
                            btnBuscar.Enabled = false;
                        });
                        objData = new GuiaRemisionBL().ListarBusquedaGuiaRemision(ref objOperationResult, null, _strFilterExpression,
                            fechaIni, fechaFin);
                    }
                },
                TaskCreationOptions.LongRunning).ContinueWith(t =>
                {
                    pBuscando.Visible = false;
                    ultraGrid1.Enabled = true;
                    btnBuscar.Enabled = true;
                    if (objOperationResult.Success != 1)
                    {
                        UltraMessageBox.Show(
                            "Error en operación:" + Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var ds = objData.Select(fila =>
                    {
                        try
                        {
                            return new BusquedaExtraccionDto
                            {
                                ID = fila.v_IdGuiaRemision,
                                FechaDocumento = fila.t_FechaEmision,
                                NombreCliente = fila.NroDocCliente + "   " + fila.NombreCliente,
                                NroDocumento = fila.v_SerieGuiaRemision.Trim() + "-" + fila.v_NumeroGuiaRemision.Trim(),
                                TipoDocumento = fila.TipoDocumento,
                                Estado = fila.i_IdEstado ?? 0,
                                Total = fila.d_Total ?? 0,
                                Moneda = fila.Moneda,
                                iTipoDocumento = fila.i_IdTipoDocumento ?? -1,
                                Origen = fila.Origen,
                            };
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            return null;
                        }
                    }).ToList().OrderBy(x => x.iTipoDocumento).ThenBy(x => x.NroDocumento).ToList();

                    ultraGrid1.DataSource = ds;
                    ultraGrid1.Rows.ToList().ForEach(fila => fila.Cells["_Check"].Value = false);
                },
                TaskScheduler.FromCurrentSynchronizationContext());
            }
            #endregion

            #region Pedidos/Cotizaciones

            if (rbPedidos.Checked)
            {
                var mes = int.Parse(dtpFechaRegistroDe.SelectedValue.ToString());

                var fechaIni = new DateTime(int.Parse(cboPeriodo.Text), mes, 1);
                var fechaFin = new DateTime(int.Parse(cboPeriodo.Text), mes, DateTime.DaysInMonth(int.Parse(cboPeriodo.Text), mes), 23, 59, 0);
                var objOperationResult = new OperationResult();
                var objData = new List<pedidoDto>();



                Task.Factory.StartNew(() =>
                {
                    lock (this)
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            pBuscando.Visible = true;
                            ultraGrid1.Enabled = false;
                            btnBuscar.Enabled = false;
                        });

                        objData = chkCalcularDespachoPedido.Checked ?
                            new PedidoBL().ListarBusquedaPedidos_(ref objOperationResult, null, null, fechaIni, fechaFin) :
                            new PedidoBL().ListarBusquedaPedidos(ref objOperationResult, null, fechaIni, fechaFin, -1, "", "", "", "", "", 0, -1, "-1");
                    }
                },
                    TaskCreationOptions.LongRunning)
                    .ContinueWith(t =>
                    {
                        pBuscando.Visible = false;
                        ultraGrid1.Enabled = true;
                        btnBuscar.Enabled = true;
                        if (objOperationResult.Success != 1)
                        {
                            UltraMessageBox.Show(
                                "Error en operación:" + Environment.NewLine + objOperationResult.ExceptionMessage,
                                "ERROR!",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        var ds = objData.Select(fila =>
                        {
                            try
                            {
                                return new BusquedaExtraccionDto
                                {
                                    ID = fila.v_IdPedido,
                                    FechaDocumento = fila.t_FechaEmision,
                                    NombreCliente = fila.NroDocCliente + "   " + fila.NombreCliente,
                                    NroDocumento = fila.v_SerieDocumento + "-" + fila.v_CorrelativoDocumento,
                                    TipoDocumento = fila.TipoDocumento,
                                    Estado = fila.i_IdEstado ?? 0,
                                    Moneda = fila.Moneda,
                                    Total = fila.Total,
                                    iTipoDocumento = fila.i_IdTipoDocumento ?? -1,
                                    Origen = fila.Origen,
                                };
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                                return null;
                            }
                        }).ToList().OrderBy(x => x.iTipoDocumento).ThenBy(x => x.NroDocumento).ToList();

                        ultraGrid1.DataSource = ds;
                        ultraGrid1.Rows.ToList().ForEach(fila => fila.Cells["_Check"].Value = false);
                    },
                        TaskScheduler.FromCurrentSynchronizationContext());



            }
            #endregion
        }

        private void frmExtraerDetallesMovimientos_Load(object sender, EventArgs e)
        {
            rbVentas.Checked = Globals.ClientSession.v_RucEmpresa == Constants.RucAgrofergic || Globals.ClientSession.v_RucEmpresa == Constants.RucAgrofergic2 || Globals.ClientSession.v_RucEmpresa == Constants.RucDemo ? true : false;
            ListaRetornoVentas = new BindingList<ventadetalleDto>();
            ListaGuiasPorAnular = new List<string>();
            LlenarMeses();
            BackColor = new GlobalFormColors().FormColor;
            Utils.Windows.MostrarOcultarFiltrosGrilla(ultraGrid1);
            AnularGuias = false;
            AnularVentas = false;
            cboPeriodo.SelectedIndex = 3;
        }

        /// <summary>
        /// Llena el combo con los doce meses del año.
        /// </summary>
        private void ultraButton1_Click(object sender, EventArgs e)
        {
            if (!ultraGrid1.Rows.Any() || !ultraGrid1.Rows.Any(fila => Convert.ToBoolean(fila.Cells["_Check"].Value.ToString()))) return;
            var filasMarcadas = ultraGrid1.Rows.Where(fila => Convert.ToBoolean(fila.Cells["_Check"].Value.ToString())).ToList();

            var objOperationResult = new OperationResult();

            #region Ventas
            if (rbVentas.Checked)
            {
                foreach (var fila in filasMarcadas)
                {
                    if (chkAnularGuias.Checked)
                        if (!_objDocumentoBl.DocumentoEsContable(int.Parse(fila.Cells["iTipoDocumento"].Value.ToString())))
                        {
                            var id = fila.Cells["ID"].Value.ToString();
                            ListaGuiasPorAnular.Add(id);
                        }
                    var pstringIdVenta = fila.Cells["ID"].Value.ToString();
                    var lista = new VentaBL().ObtenerVentaDetalles(ref objOperationResult, pstringIdVenta);
                    ListaRetornoVentas = new BindingList<ventadetalleDto>(ListaRetornoVentas.Concat(lista.Where(p => p.v_IdProductoDetalle != "N002-PE000000000").ToList()).ToList());
                }

                DialogResult = DialogResult.OK;
                AnularVentas = chkAnularGuias.Checked;
            }
            #endregion

            #region Guía Remisión
            if (rbGuiaRemision.Checked)
            {
                foreach (var fila in filasMarcadas)
                {
                    var id = fila.Cells["ID"].Value.ToString();
                    if (chkAnularGuias.Checked) ListaGuiasPorAnular.Add(id);
                    var listaDetalles = new GuiaRemisionBL().ObtenerDetalleGuiaRemisionParaExtraccion(ref objOperationResult, id);

                    var result = listaDetalles.Select(n => new ventadetalleDto
                    {
                        i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                        i_Anticipio = 0,
                        i_Eliminado = n.i_Eliminado,
                        //i_IdAlmacen = n.i_IdAlmacen,
                        i_IdAlmacen = Globals.ClientSession.i_IdAlmacenPredeterminado.Value,
                        i_IdCentroCosto = "0",
                        i_IdUnidadMedida = n.i_IdUnidadMedida,
                        i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                        ProductoNombre = n.v_Descripcion,
                        v_DescripcionProducto = n.v_Descripcion,
                        v_IdMovimientoDetalle = n.v_IdMovimientoDetalle,
                        v_IdProductoDetalle = n.v_IdProductoDetalle,
                        v_NroCuenta = n.NroCuenta,
                        d_PrecioVenta = n.d_Precio,
                        d_Cantidad = n.d_Cantidad,
                        d_CantidadEmpaque = n.d_CantidadEmpaque,
                        d_Precio = n.d_Precio,
                        d_PrecioImpresion = n.d_Precio,
                        v_CodigoInterno = n.v_CodInterno,
                        Empaque = n.d_CantidadEmpaque,
                        UMEmpaque = n.i_IdUnidadMedidaProducto,
                        i_EsServicio = 0,
                        i_IdUnidadMedidaProducto = n.i_IdUnidadEmpaque,
                        t_ActualizaFecha = n.t_ActualizaFecha,
                        t_InsertaFecha = n.t_InsertaFecha,
                        v_FacturaRef = n.v_Descuento
                    }).ToList();

                    ListaRetornoVentas = new BindingList<ventadetalleDto>(ListaRetornoVentas.Concat(result).ToList());
                }
                DialogResult = DialogResult.OK;
                AnularGuias = chkAnularGuias.Checked;
            }
            #endregion

            #region Pedidos/Cotizacion
            if (rbPedidos.Checked)
            {
                foreach (var fila in filasMarcadas)
                {
                    var id = fila.Cells["ID"].Value.ToString();
                    var listaDetalles = new PedidoBL().ObtenerPedidoDetallesParaExtraccion(ref objOperationResult, id);

                    var result = listaDetalles.Select(n => new ventadetalleDto
                    {
                        i_Anticipio = 0,
                        i_Eliminado = n.i_Eliminado,
                        i_IdAlmacen = n.i_IdAlmacen,
                        i_IdCentroCosto = "0",
                        i_IdUnidadMedida = n.i_IdUnidadMedida,
                        i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                        ProductoNombre = n.v_NombreProducto,
                        v_DescripcionProducto = n.v_NombreProducto,
                        v_IdProductoDetalle = n.v_IdProductoDetalle,
                        v_NroCuenta = n.NroCuenta,
                        d_PrecioVenta = n.d_PrecioVenta,
                        d_Cantidad = n.d_Cantidad,
                        d_CantidadEmpaque = n.d_CantidadEmpaque,
                        d_Precio = n.d_PrecioUnitario,
                        d_PrecioImpresion = n.d_PrecioUnitario,
                        v_CodigoInterno = n.v_CodigoInterno,
                        Empaque = n.Empaque,
                        i_EsServicio = 0,
                        i_IdUnidadMedidaProducto = n.i_IdUnidadMedidaProducto,
                        v_FacturaRef = n.v_Descuento,
                        i_ValidarStock = n.i_ValidarStock ?? 0,
                        t_FechaCaducidad = n.t_FechaCaducidad,

                        i_SolicitarNroSerieSalida = n.i_SolicitarNroSerieSalida,
                        i_SolicitarNroLoteSalida = n.i_SolicitarNroLoteSalida,


                        v_NroLote = n.v_NroLote,
                        v_NroSerie = n.v_NroSerie,
                        v_PedidoExportacion = n.v_NroPedido



                    }).ToList();
                    var detalle = listaDetalles.FirstOrDefault();
                    if (detalle != null) IdPedido = detalle.v_IdPedido;
                    ListaRetornoVentas = new BindingList<ventadetalleDto>(ListaRetornoVentas.Concat(result).ToList());
                }

                DialogResult = DialogResult.OK;
            }
            #endregion

            Close();
        }

        private void ultraGrid1_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (e.Row.Cells["_Check"].Value != null && Convert.ToBoolean(e.Row.Cells["_Check"].Value.ToString()))
                e.Row.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
            else
                e.Row.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.False;

            if (e.Row.Cells["Estado"].Value.ToString() == "0" && e.Row.Cells["iTipoDocumento"].Value.ToString() != "430" && e.Row.Cells["iTipoDocumento"].Value.ToString() != "431")
            {
                e.Row.Appearance.BackColor = Color.Salmon;
                e.Row.Appearance.BackColor2 = Color.Salmon;
                e.Row.Appearance.ForeColor = Color.Black;
            }
        }

        private void ultraGrid1_CellChange(object sender, CellEventArgs e)
        {
            e.Cell.Value = e.Cell.EditorResolved.Value;
            ultraGrid1.Rows.Where(p => p != ultraGrid1.ActiveRow)
                .ToList().ForEach(o => o.Cells["_Check"].Activation =
                            ultraGrid1.Rows.Any(f => Convert.ToBoolean(f.Cells["_Check"].Value.ToString())) &&
                            rbPedidos.Checked ? Activation.Disabled : Activation.AllowEdit);
        }

        private void ultraButton2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void rbGuiaRemision_CheckedChanged(object sender, EventArgs e)
        {

            chkAnularGuias.Visible = rbGuiaRemision.Checked;
            chkAnularGuias.Checked = rbGuiaRemision.Checked;
            ListaGuiasPorAnular = new List<string>();
        }

        private void rbVentas_CheckedChanged(object sender, EventArgs e)
        {

            chkAnularGuias.Visible = rbVentas.Checked;
            chkAnularGuias.Checked = UserConfig.Default.AnularGuiaInternaExtraccion;
            ListaGuiasPorAnular = new List<string>();
        }

        private void ultraGrid1_ClickCellButton(object sender, CellEventArgs e)
        {
            if (ultraGrid1.ActiveRow == null) return;
            switch (e.Cell.Column.Key)
            {
                case "Ver":
                    if (rbVentas.Checked)
                    {
                        var frm = new frmRegistroVenta("Consulta", ultraGrid1.ActiveRow.Cells["ID"].Value.ToString(),
                            "KARDEX");
                        frm.ShowDialog();
                    }

                    if (rbGuiaRemision.Checked)
                    {
                        var frm = new frmGuiaRemision("Consulta",
                            ultraGrid1.ActiveRow.Cells["ID"].Value.ToString(), "KARDEX");
                        frm.ShowDialog();
                    }

                    if (rbPedidos.Checked)
                    {
                        var f = new FrmExtraerDetallesMovimientosConsultaPedidos(ultraGrid1.ActiveRow.Cells["ID"].Value.ToString());
                        if (f.Items > 0)
                            f.ShowDialog();
                    }
                    break;
            }
        }

        private void rbPedidos_CheckedChanged(object sender, EventArgs e)
        {
            chkCalcularDespachoPedido.Visible = rbPedidos.Checked;
        }

        private void FrmExtraerDetallesMovimientos_FormClosed(object sender, FormClosedEventArgs e)
        {
            UserConfig.Default.RealizarCalculoDespachoPedido = chkCalcularDespachoPedido.Checked;
            UserConfig.Default.AnularGuiaInternaExtraccion = chkAnularGuias.Checked;
            UserConfig.Default.Save();
        }

        private void LlenarMeses()
        {
            try
            {
                var meses = Enumerable.Range(1, 12);
                var data = new List<Tuple<int, string>>();

                foreach (var item in meses)
                {
                    data.Add(new Tuple<int,string>(item, CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item).ToUpper()));
                }

                dtpFechaRegistroDe.DataSource = data;
                dtpFechaRegistroDe.ValueMember = "Item1";
                dtpFechaRegistroDe.DisplayMember = "Item2";
            }
            catch (Exception ex)
            {
                MessageBox.Show(Utils.ExceptionFormatter(ex), "LlenarMeses()");
            }
        }
    }
}
