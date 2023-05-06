using System;
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
    public partial class frmExtraerDetallesMovimientosCompras : Form
    {
        public BindingList<importaciondetalleproductoDto> ListaRetornoImportacionDetalleProducto { get; set; }
        private readonly OrdenCompraBL _objOrdenCompraBL = new OrdenCompraBL(); 
        public string IdPedido { get; set; }
        public List<string> ListaGuiasPorAnular { get; set; }
        public bool AnularGuias { get; set; }
        public bool AnularVentas { get; set; }
        private readonly DocumentoBL _objDocumentoBl = new DocumentoBL();
        public frmExtraerDetallesMovimientosCompras()
        {
            InitializeComponent();
            //chkCalcularDespachoPedido.Checked = UserConfig.Default.RealizarCalculoDespachoPedido;
            //chkAnularGuias.Checked = UserConfig.Default.AnularGuiaInternaExtraccion;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            #region OrdendesCompra

            if (rbOrdenesCompra.Checked)
            {


                var fechaIni = dtpFechaRegistroInicio.Value.ToString();
                var fechaFin = DateTime.Parse(dtpFechaRegistroFin.Text + " 23:59");
                var objOperationResult = new OperationResult();
                var objData = new List<ordendecompraShortDto>();
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
                        objData = _objOrdenCompraBL.ListarOrdenCompras(ref objOperationResult, dtpFechaRegistroInicio.Value.Date, dtpFechaRegistroFin.Value, dtpFechaRegistroInicio.Value.Date, dtpFechaRegistroFin.Value.Date,null,true);    
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
                                ID = fila.v_IdOrdenCompra ,
                                FechaDocumento = fila.FechaRegistro.Value.Date ,
                                NombreCliente = fila.RazonSocialProveedor,
                                NroDocumento = fila.NroOrdenCompra,
                                TipoDocumento = fila.TipoDocumento,
                                Estado = fila.i_IdEstado ?? 0,
                                Moneda = fila.Moneda,
                                Total = fila.Importe ?? 0,
                                iTipoDocumento = fila.iTipoDocumento ,
                                //Origen = fila.Origen,
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
            rbOrdenesCompra.Checked = Globals.ClientSession.v_RucEmpresa == Constants.RucAgrofergic || Globals.ClientSession.v_RucEmpresa == Constants.RucAgrofergic2 || Globals.ClientSession.v_RucEmpresa == Constants.RucDemo ? true : false;
            ListaRetornoImportacionDetalleProducto= new BindingList<importaciondetalleproductoDto>();
            ListaGuiasPorAnular = new List<string>();
            BackColor = new GlobalFormColors().FormColor;
            Utils.Windows.MostrarOcultarFiltrosGrilla(ultraGrid1);
            rbOrdenesCompra.Checked = true;
            
        }

        /// <summary>
        /// Llena el combo con los doce meses del año.
        /// </summary>
   

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            if (!ultraGrid1.Rows.Any() || !ultraGrid1.Rows.Any(fila => Convert.ToBoolean(fila.Cells["_Check"].Value.ToString()))) return;
            var filasMarcadas = ultraGrid1.Rows.Where(fila => Convert.ToBoolean(fila.Cells["_Check"].Value.ToString())).ToList();
            var objOperationResult = new OperationResult();
            #region OrdenCompra
            if (rbOrdenesCompra.Checked)
            {
                foreach (var fila in filasMarcadas)
                {
                    var IdOrdenCompra = fila.Cells["ID"].Value.ToString();
                    var listaDetalles = new OrdenCompraBL ().CargarDetalle(ref objOperationResult, IdOrdenCompra);
                    var result = listaDetalles.Select(n => new importaciondetalleproductoDto
                    {
                        i_Eliminado = 0,
                        v_NombreProducto = n.NombreProducto,
                        d_Cantidad = n.d_Cantidad,
                        d_CantidadEmpaque = n.d_CantidadEmpaque,
                        i_IdUnidadMedida =n.i_IdUnidadMedida ??-1,
                        d_Precio =n.d_PrecioUnitario??0,
                        IdOrdenCompra =n.v_IdOrdenCompra,
                        v_IdProductoDetalle =n.v_IdProductoDetalle ,
                        v_CodigoInterno=n.CodProducto,
                         Empaque=n.Empaque.ToString () ,
                         UMEmpaque=n.EmpaqueUM,
                        i_IdUnidadMedidaProducto = n.UMProducto,
                  
                       
                    }).ToList();
                    var detalle = listaDetalles.FirstOrDefault();
                    if (detalle != null) IdPedido = detalle.v_IdOrdenCompra;
                    //ListaRetornoImportacionDetalleProducto = new BindingList<importaciondetalleproductoDto>(result).ToList();
                    ListaRetornoImportacionDetalleProducto = new BindingList<importaciondetalleproductoDto>(ListaRetornoImportacionDetalleProducto.Concat(result).ToList());
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
                            rbOrdenesCompra.Checked ? Activation.Disabled : Activation.AllowEdit);
        }

        private void ultraButton2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void rbGuiaRemision_CheckedChanged(object sender, EventArgs e)
        {

            //chkAnularGuias.Visible = rbGuiaRemision.Checked;
            //chkAnularGuias.Checked = rbGuiaRemision.Checked;
            ListaGuiasPorAnular = new List<string>();
        }

        private void rbVentas_CheckedChanged(object sender, EventArgs e)
        {

            //chkAnularGuias.Visible = rbOrdenesCompra.Checked;
            //chkAnularGuias.Checked = UserConfig.Default.AnularGuiaInternaExtraccion;
            ListaGuiasPorAnular = new List<string>();
        }

        private void ultraGrid1_ClickCellButton(object sender, CellEventArgs e)
        {
            if (ultraGrid1.ActiveRow == null) return;
            switch (e.Cell.Column.Key)
            {
                case "Ver":
                    if (rbOrdenesCompra.Checked)
                    {
                        var frm = new frmRegistroVenta("Consulta", ultraGrid1.ActiveRow.Cells["ID"].Value.ToString(),
                            "KARDEX");
                        frm.ShowDialog();
                    }

                    //if (rbGuiaRemision.Checked)
                    //{
                    //    var frm = new frmGuiaRemision("Consulta",
                    //        ultraGrid1.ActiveRow.Cells["ID"].Value.ToString(), "KARDEX");
                    //    frm.ShowDialog();
                    //}

                    //if (rbPedidos.Checked)
                    //{
                    //    var f = new FrmExtraerDetallesMovimientosConsultaPedidos(ultraGrid1.ActiveRow.Cells["ID"].Value.ToString());
                    //    if (f.Items > 0)
                    //        f.ShowDialog();
                    //}
                    break;
            }
        }

        private void rbPedidos_CheckedChanged(object sender, EventArgs e)
        {
            //chkCalcularDespachoPedido.Visible = rbPedidos.Checked;
        }

        private void FrmExtraerDetallesMovimientos_FormClosed(object sender, FormClosedEventArgs e)
        {
            //UserConfig.Default.RealizarCalculoDespachoPedido = chkCalcularDespachoPedido.Checked;
            //UserConfig.Default.AnularGuiaInternaExtraccion = chkAnularGuias.Checked;
            UserConfig.Default.Save();
        }
    }
}
