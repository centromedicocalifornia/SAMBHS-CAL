using SAMBHS.Windows.SigesoftIntegration.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Requerimientos.Manguifajas
{
    public partial class frmEditarStock : Form
    {
        public string CodigoInterno { get; set; }
        public string _ProductoDetalleId { get; set; }
        public frmEditarStock(string _CodigoInterno)
        {
            CodigoInterno = _CodigoInterno;
            InitializeComponent();
        }

        private void frmEditarStock_Load(object sender, EventArgs e)
        {
            var objProducto = new AgendaBl().UpdateStockProducto(CodigoInterno);
            lblNombre.Text = objProducto.v_Descripcion.ToString();
            _ProductoDetalleId = objProducto.v_ProductoDetalleId;
            txtStock.Text = (decimal.Round((objProducto.d_StockActual),2)).ToString();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            decimal newStock = decimal.Round((decimal.Parse(txtStock.Text)),2);
            new AgendaBl().UpdateStockActual(_ProductoDetalleId, newStock);

            MessageBox.Show("GUARDADO CORRECTAMENTE", "CORRECTO", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //this.Close();
        }
    }
}
