using System;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.Venta.BL;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class FrmExtraerDetallesMovimientosConsultaPedidos : Form
    {
        private readonly PedidoBL _objPedidoBl = new PedidoBL();
        public int Items { get; set; }

        public FrmExtraerDetallesMovimientosConsultaPedidos(string pstrIdPedido)
        {
            InitializeComponent();
            var objOperationResult = new OperationResult();
            var pedidoDetalle = _objPedidoBl.ObtenerPedidoDetallesParaExtraccion(ref objOperationResult,
                pstrIdPedido);
            if (objOperationResult.Success == 0)
            {
                MessageBox.Show(@"Occurrió un error al realizar la consulta", @"Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            Items = pedidoDetalle.Count;
            ultraGrid1.DataSource = pedidoDetalle;
            Text = string.Format("Faltantes de facturar del pedido {0}", Items > 0 ? pedidoDetalle[0].NroPedido : "");
        }

        public sealed override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        private void frmExtraerDetallesMovimientosConsultaPedidos_Load(object sender, EventArgs e)
        {
            Utils.Windows.MostrarOcultarFiltrosGrilla(ultraGrid1);
        }
    }
}
