using CrystalDecisions.Shared;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Windows.WinClient.UI.Reportes.Compras;
using SAMBHS.Windows.WinClient.UI.Requerimientos.Ablimatex.Printer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using SAMBHS.Almacen.BL;

namespace SAMBHS.Windows.WinClient.UI.Requerimientos.Ablimatex
{
    public partial class frmOrdenDeAvios : Form
    {
        private List<Product> _products;
        public string Glosa { get; set; }
        private readonly string _ordenCompra;
        public frmOrdenDeAvios(string nroOrdenCompra)
        {
            InitializeComponent();
            _ordenCompra = nroOrdenCompra;
            Load += frmOrdenDeAvios_Load;
        }

        void frmOrdenDeAvios_Load(object sender, EventArgs e)
        {
            var objDatahierarchyBl = new DatahierarchyBL();
            var objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList(cbCodes, "Value1", "Id", objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 167, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cbHantag, "Value1", "Id", objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 168, null), DropDownListAction.Select);
        }

        public void LoadItems(IEnumerable<Product> products)
        {
            var grTallas = from p in products
                group p by p.Codigo.Substring(8, 2);

            _products = new List<Product>();
            var bl = new TallaBL();
            var objResult = new OperationResult();
            foreach (var talla in grTallas)
            {
                var codTalla = talla.Key;
                var dto = bl.ObtenerTallaByCode(ref objResult, codTalla);
                var newP = new Product
                {
                    Codigo = dto != null ? dto.v_Nombre : codTalla,
                    Cantidad = talla.Sum(t =>t.Cantidad)
                };
                _products.Add(newP);

                var row = grdDataLista.DisplayLayout.Bands[0].AddNew();
                grdDataLista.Rows.Move(row, 0);
                grdDataLista.ActiveRowScrollRegion.ScrollRowIntoView(row);
                row.Cells["codArticulo"].Value = newP.Codigo;
                row.Cells["Descripcion"].Value = string.Empty;
                row.Cells["cantidad"].Value = newP.Cantidad;
                row.Cells["chkPrint"].Value = true;
                row.Cells["Cod1"].Value = "-1";
                row.Cells["Cod2"].Value = "-1";
                row.Cells["Cod3"].Value = "-1";
                row.Cells["Cod4"].Value = "-1";
                row.Cells["Cod5"].Value = "-1";
                row.Cells["CodHt"].Value = "-1";
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            var saver = new SaveFileDialog
            {
                Filter = @"Pdf Files (*.pdf)|*.pdf"
            };

            if (saver.ShowDialog() != DialogResult.OK) return;
            try
            {
                ExportToPdf(saver.FileName);
                if (File.Exists(saver.FileName))
                {
                    Process.Start(saver.FileName);
                }
                EndingPrint();
            }
            catch (Exception er)
            {
                UltraMessageBox.Show("Error", er.Message, Icono: MessageBoxIcon.Error);
            }
        }

        private void ExportToPdf(string filename)
        {
            DataSet ds = null;
            ReportDocument rep = null;
            try
            {
                var prods = GetAvios().ToList();
                if (prods.Count == 0)
                {
                    UltraMessageBox.Show("Tiene que existir al menos un elemento seleccionado.", "Alerta", Icono: MessageBoxIcon.Warning);
                    return;
                }
                var dt = Utils.ConvertToDatatable(prods);
                dt.TableName = "Avios";
                ds = new DataSet();
                ds.Tables.Add(dt);
                rep = new crAvioAblimatex();
                rep.SetDataSource(ds);
                rep.SetParameterValue("RucEmpresa", Globals.ClientSession.v_RucEmpresa);
                rep.SetParameterValue("OrdenCompra", _ordenCompra);
                rep.SetParameterValue("Glosa", Glosa);
                rep.ExportToDisk(ExportFormatType.PortableDocFormat, filename); 
            }
            finally
            {
                if (rep != null) rep.Dispose();
                if (ds != null) ds.Dispose();
            }
            
        }

        private IEnumerable<ProductAvio> GetAvios()
        {
            var count = grdDataLista.Rows.Count;
            if (count == 0) yield break;

            foreach (var row in grdDataLista.Rows)
            {
                var cells = row.Cells;
                if (!(bool)cells["chkPrint"].Value) continue;

                var val = cells["cantidad"].Value;
                var code = cells["codArticulo"].Value.ToString();
                var cant = val != null ? int.Parse(val.ToString()) : 0;

                var prod = new ProductAvio
                {
                    Codigo = code,
                    Descripcion = cells["Descripcion"].Value.ToString(),
                    Cantidad = cant,
                    Codigo1 = cells["Cod1"].Text,
                    Codigo2 = cells["Cod2"].Text,
                    Codigo3 = cells["Cod3"].Text,
                    Codigo4 = cells["Cod4"].Text,
                    Codigo5 = cells["Cod5"].Text,
                    Hantag = cells["CodHt"].Text
                };

                // Descontando Cantidades
                var talla = _products.FirstOrDefault(p => p.Codigo == code);
                if (talla != null)
                {
                    if (talla.Cantidad > cant)
                        talla.Cantidad -= cant;
                    else talla.Cantidad = 0;
                }
                yield return prod;
            }
        }

        private void grdDataLista_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            if (e.Cell.Row.Index != 0) return;

            string[] fields = { "codArticulo", "cantidad", "Descripcion", "chkPrint" };
            var col = e.Cell.Column.Key;
            if (fields.Contains(col)) return;

            var newVal = e.Cell.Value;
            foreach (var row in grdDataLista.Rows)
            {
                if (row.Index == 0) continue;
                row.Cells[col].Value = newVal;
            }
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
    }
    /// <summary>
    /// Class ProductAvio.
    /// </summary>
    internal class ProductAvio
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public decimal Cantidad { get; set; }
        public string Codigo1 { get; set; }
        public string Codigo2 { get; set; }
        public string Codigo3 { get; set; }
        public string Codigo4 { get; set; }
        public string Codigo5 { get; set; }
        public string Hantag { get; set; }
    }
}
