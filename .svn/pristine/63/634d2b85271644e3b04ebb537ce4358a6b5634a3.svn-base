using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Common.Resource;

namespace SAMBHS.Windows.WinClient.UI.Requerimientos.Ablimatex.Printer
{
    public partial class frmListadeImpresionPopup : Form
    {
        private List<Product> _products;
        private CancellationTokenSource _cancelSource;
        public frmListadeImpresionPopup()
        {
            InitializeComponent();
        }

        private void frmListadeImpresionPopup_Load(object sender, EventArgs e)
        {
            Disposed += delegate
            {
                if (_cancelSource != null)
                    _cancelSource.Dispose();
            };
            txtPrinterName.Text = UserConfig.Default.ThermalPrinter;
        }

        public void LoadItems(IEnumerable<Product> products)
        {
            _products = products.ToList();
            foreach (var product in _products)
            {
                UltraGridRow row = grdDataLista.DisplayLayout.Bands[0].AddNew();
                grdDataLista.Rows.Move(row, 0);
                grdDataLista.ActiveRowScrollRegion.ScrollRowIntoView(row);
                row.Cells["codArticulo"].Value = product.Codigo;
                row.Cells["descripcion"].Value = product.Descripcion;
                row.Cells["cantidad"].Value = product.Cantidad;
                row.Cells["precioV"].Value = product.Precio;
                row.Cells["chkPrint"].Value = true;
            }
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            // "TSC TTP-244 Plus (Copiar 1)"
            if (string.IsNullOrEmpty(txtPrinterName.Text))
            {
                UltraMessageBox.Show("Ingrese un nombre de impresora valido", "Validacion", Icono: MessageBoxIcon.Warning);
                return;
            }
            var printerName = txtPrinterName.Text;
            var items = from r in grdDataLista.Rows
                let chk = r.Cells["chkPrint"].Value
                where chk != null && (bool) chk
                select new Product
                {
                    Descripcion = r.GetCellValue("descripcion").ToString(),
                    Codigo = r.GetCellValue("codArticulo").ToString(),
                    Cantidad = int.Parse(r.GetCellValue("cantidad").ToString()),
                    Precio = (decimal)r.GetCellValue("precioV")
                };

            var products = items.ToArray();
            if (!products.Any())
            {
                UltraMessageBox.Show("Tiene que existir al menos un elemento seleccionado.", "Alerta", Icono: MessageBoxIcon.Warning);
                return;
            }
            // Descontando cantidades.
            foreach (var item in products)
            {
                var prod = _products.FirstOrDefault(r => r.Codigo == item.Codigo);
                if (prod == null) continue;
                if (prod.Cantidad > item.Cantidad)
                    prod.Cantidad -= item.Cantidad;
                else prod.Cantidad = 0;
            }

            _cancelSource = new CancellationTokenSource();
            var token = _cancelSource.Token;

            btnImprimir.Enabled = false;
            Task.Factory.StartNew(data =>
            {
                using (var term = new ThermalPrinterManager(printerName, token))
                {
                    try
                    {
                        term.Open();
                        term.Print((IEnumerable<Product>)data);
                        Invoke((MethodInvoker) EndingPrint);
                    }
                    catch (Exception ex)
                    {
                        UltraMessageBox.Show(ex.Message, "Ocurrio un eror imprimiendo!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }, items, token)
            .ContinueWith(t =>
            {
                UserConfig.Default.ThermalPrinter = printerName;
                UserConfig.Default.Save();
                UltraMessageBox.Show("Finalizo la impresión", "Completado!");
                btnImprimir.Enabled = true;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (grdDataLista.ActiveRow == null) return;
            grdDataLista.ActiveRow.Delete(false);
            //grdDataLista.DeleteSelectedRows(false);
        }

        private void EndingPrint()
        {
            foreach (var list in grdDataLista.Rows)
            {
                var cod = (string)list.Cells["codArticulo"].Value;
                var prod = _products.FirstOrDefault(r => r.Codigo == cod);
                if (prod == null) continue;
                list.Cells["cantidad"].Value = prod.Cantidad;
                if (prod.Cantidad == 0)
                {
                    list.Cells["chkPrint"].Value = false;
                } 
            }
            grdDataLista.Refresh();
        }

        private void DuplicateToolItem_Click(object sender, EventArgs e)
        {
            if (grdDataLista.ActiveRow == null) return;
            var active = grdDataLista.ActiveRow;
            var row = grdDataLista.DisplayLayout.Bands[0].AddNew();
            row.Cells["codArticulo"].Value = active.GetCellValue("codArticulo");
            row.Cells["descripcion"].Value = active.GetCellValue("descripcion");
            row.Cells["cantidad"].Value = active.GetCellValue("cantidad");
            row.Cells["precioV"].Value = active.GetCellValue("precioV");
            row.Cells["chkPrint"].Value = active.GetCellValue("chkPrint");
            grdDataLista.Rows.Move(row, active.Index);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (_cancelSource != null && !_cancelSource.IsCancellationRequested)
                _cancelSource.Cancel();
        }

        private void txtPrecioGlobal_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPrecioGlobal.Text)) return;

            decimal mount;
            if (!decimal.TryParse(txtPrecioGlobal.Text, out mount))
            {
                txtPrecioGlobal.Text = @"0.00";
                return;
            }

            foreach (var row in grdDataLista.Rows)
            {
                row.Cells["precioV"].Value = mount;
            }
        }
  }
}

