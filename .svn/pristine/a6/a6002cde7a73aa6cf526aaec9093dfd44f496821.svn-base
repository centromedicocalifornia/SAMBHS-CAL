using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Almacen.BL;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmRecalculoStockBusqueda : Form
    {
        public List<ProductoBL.ProductoFiltro> ProductoFiltros = new List<ProductoBL.ProductoFiltro>();
        public frmRecalculoStockBusqueda()
        {
            InitializeComponent();
        }

        private void frmRecalculoStockBusqueda_Load(object sender, EventArgs e)
        {
            ultraGrid1.DataSource = new ProductoBL().ObtenerFiltrosProducto();
        }

        private void frmRecalculoStockBusqueda_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (var row in ultraGrid1.Selected.Rows)
                ProductoFiltros.Add((ProductoBL.ProductoFiltro) row.ListObject);

            DialogResult = DialogResult.OK;
        }
    }
}
