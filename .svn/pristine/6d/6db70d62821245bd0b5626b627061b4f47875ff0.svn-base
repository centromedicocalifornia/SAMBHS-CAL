using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using SAMBHS.Almacen.BL;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmRecalculoStockFiltro : Form
    {
        private BindingList<ProductoBL.ProductoFiltro> ds;
        public List<string> FiltroNuevo;
        public frmRecalculoStockFiltro(List<string> filtro)
        {
            InitializeComponent();
            ds = new ProductoBL().ObtenerFiltrosProducto(filtro);
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            var f = new frmRecalculoStockBusqueda();
            f.ShowDialog();
            if (f.DialogResult != DialogResult.OK) return;
            foreach (var productoFiltro in f.ProductoFiltros)
                ds.Add(productoFiltro);
        }

        private void frmRecalculoStockFiltro_Load(object sender, EventArgs e)
        {
            ultraGrid1.DataSource = ds;
        }

        private void frmRecalculoStockFiltro_FormClosing(object sender, FormClosingEventArgs e)
        {
            FiltroNuevo = ultraGrid1.Rows.Any() ? ultraGrid1.Rows.Select(r => r.Cells["Id"].Value.ToString()).ToList() : new List<string>();
            DialogResult = DialogResult.OK;
        }
    }
}
