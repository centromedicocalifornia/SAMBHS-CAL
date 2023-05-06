using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Compra.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Compras
{
    public partial class frmRegistroCompraResumenDAOT : Form
    {

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        ComprasBL _objComprasBL = new ComprasBL();
        public frmRegistroCompraResumenDAOT( string parametro)
        {
            InitializeComponent();
        }

        private void frmRegistroCompraResumenDAOT_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            CargarCombos();
        }

        private void CargarCombos()
        {
            OperationResult objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList(cboTipoCompra, "Value1", "Id", _objComprasBL.ObtenerConceptosParaCombo(ref objOperationResult, 1, null), DropDownListAction.Select);
            cboTipoCompra.Value = "-1";
            cboOrdenar.SelectedIndex = 0;
        }

        private void BtnVuisualizar_Click(object sender, EventArgs e)
        {
            CargarReporte();
        }


        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Reporte de Compras Proveedor DAOT Resumen"
                : @"Reporte de Compras Proveedor DAOT Resumen";
            pBuscando.Visible = estado;
            BtnVuisualizar.Enabled = !estado;
        }


        private void CargarReporte()
        {
            List<ReporteCompraDaotAnalitico> ListaReporte = new List<ReporteCompraDaotAnalitico>();

            OcultarMostrarBuscar(true);
            OperationResult objOperationResult = new OperationResult();
            Cursor.Current = Cursors.WaitCursor;
            Task.Factory.StartNew(() =>
            {


                ListaReporte = new ComprasBL().ReporteProveedorCompraDaotResumen(ref objOperationResult, dtpFechaRegistroDe.Value.Date, DateTime.Parse(dtpFechaRegistroAl.Text + " 23:59"), txtTope.Text.Trim() == string.Empty ? 0 : decimal.Parse(txtTope.Text.Trim()), int.Parse(cboTipoCompra.Value.ToString()), txtCuenta.Text.Trim(), txtProducto.Text, txtProveedor.Text.Trim(),cboOrdenar.Value.ToString ());


            }, _cts.Token)
            .ContinueWith(t =>
            {
                if (_cts.IsCancellationRequested) return;
                OcultarMostrarBuscar(false);
                Cursor.Current = Cursors.Default;
                if (objOperationResult.Success == 0)
                {
                    if (!string.IsNullOrEmpty(objOperationResult.ExceptionMessage))
                    {
                        UltraMessageBox.Show("Ocurrió un Error al generar Reporte de Compras Proveedor DAOT Resumen", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show("Ocurrió un Error al generar Reporte de Compras Proveedor DAOT Resumen", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }

                var rp = new Reportes.Compras.crRegistroCompraResumenDAOT();

                var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();
                DataSet ds1 = new DataSet();

                DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(ListaReporte);
                DataTable dt2 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate2);

                ds1.Tables.Add(dt2);
                ds1.Tables.Add(dt);

                ds1.Tables[0].TableName = "dsEmpresa";
                ds1.Tables[1].TableName = "dsReporteComprasDaotAnalitico";
                rp.SetDataSource(ds1);

                rp.SetParameterValue("FechaImpresion", "DEL " + dtpFechaRegistroDe.Text + " AL " + dtpFechaRegistroAl.Text);
                rp.SetParameterValue("NroRegistros", ListaReporte.Count());
            
                rp.SetParameterValue("NombreEmpresa", aptitudeCertificate2.FirstOrDefault().NombreEmpresaPropietaria.Trim());
                rp.SetParameterValue("RucEmpresa", "R.U.C. : " + aptitudeCertificate2.FirstOrDefault().RucEmpresaPropietaria.Trim());

                crystalReportViewer1.ReportSource = rp;
                crystalReportViewer1.Show();

                crystalReportViewer1.Zoom(110);





            }
                 , TaskScheduler.FromCurrentSynchronizationContext());



        }

        private void frmRegistroCompraAnaliticoDAOT_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }

        private void txtTope_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroDecimal2UltraTextBox(txtTope, e);
        }

        private void txtProducto_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscarArticulo")
            {
                OperationResult objOperationResult = new OperationResult();
                Mantenimientos.frmBuscarProducto frm = new Mantenimientos.frmBuscarProducto(-1, "REPORTE", null, null);
                frm.ShowDialog();

                if (frm._IdProducto != null)
                {

                    txtProducto.Text = frm._CodigoInternoProducto.Trim();
                }

            }
        }

        private void txtCuenta_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            string CuentaBuscar = txtCuenta.Text.Trim() == string.Empty ? "10" : string.Empty;
            Mantenimientos.frmPlanCuentasConsulta frmPlanCuentasConsulta = new Mantenimientos.frmPlanCuentasConsulta(CuentaBuscar);
            frmPlanCuentasConsulta.ShowDialog();
            if (frmPlanCuentasConsulta._NroSubCuenta != null)
            {
                txtCuenta.Text = frmPlanCuentasConsulta._NroSubCuenta;

            }

        }

        private void txtProveedor_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            Mantenimientos.frmBuscarProveedor frm = new Mantenimientos.frmBuscarProveedor("", "");
            frm.ShowDialog();
            if (frm._IdProveedor != null)
            {
                txtProveedor.Text = frm._CodigoProveedor;
            }

        }

        private void frmRegistroCompraResumenDAOT_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }

        private void ultraExpandableGroupBox1_ExpandedStateChanging(object sender, CancelEventArgs e)
        {
            if (ultraExpandableGroupBox1.Expanded)
            {
                groupBox1.Location = new Point(groupBox1.Location.X, ultraExpandableGroupBox1.Location.Y + ultraExpandableGroupBox1.Height + 5);
                groupBox1.Height = Height - groupBox1.Location.Y - 7;
            }
            else
            {
                groupBox1.Location = new Point(groupBox1.Location.X, ultraExpandableGroupBox1.Location.Y + ultraExpandableGroupBox1.Height + 5);
                groupBox1.Height = Height - groupBox1.Location.Y - 7;
            }
        }

       

        
    }
}
