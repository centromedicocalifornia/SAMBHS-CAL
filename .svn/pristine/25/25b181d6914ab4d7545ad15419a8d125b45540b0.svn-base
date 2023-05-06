using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using SAMBHS.Common.Resource;
using System.Threading.Tasks;
using SAMBHS.Common.BE;
using SAMBHS.Compra.BL;
using SAMBHS.Common.BL;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Compras
{
    public partial class frmReporteOrdenCompraEstado : Form
    {
        #region Fields
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        #endregion

        #region Init
        public frmReporteOrdenCompraEstado(string arg)
        {
            InitializeComponent();
            Load += frmReporteOrdenCompraEstado_Load;
            txtProveedor.Tag = string.Empty;
        }

        private void frmReporteOrdenCompraEstado_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
        }
        #endregion

        #region Methods
        private void OcultarMostrarBuscar(bool estado)
        {
            pBuscando.Visible = estado;
            btnVisualizar.Enabled = !estado;
        }

        private void CargarReporte()
        {
            OcultarMostrarBuscar(true);
            List<ReporteOrdenCompraEstado> content = null;
            var objOperationResult = new OperationResult();
            Task.Factory.StartNew(delegate
            {
                content = new OrdenCompraBL().ReporteOrdenCompraEstado(ref objOperationResult, dtpFechaRegistroDe.Value.Date, DateTime.Parse(dtpFechaRegistroAl.Text + " 23:59"), (string)txtProveedor.Tag ?? "");
            }, _cts.Token)
            .ContinueWith(t =>
            {
                if (_cts.IsCancellationRequested) return;
                OcultarMostrarBuscar(false);
                if (objOperationResult.Success == 0)
                {
                    UltraMessageBox.Show(objOperationResult.ExceptionMessage, "Error", Icono:MessageBoxIcon.Error);
                    return;
                }
                var rp = new crReporteOrdenCompraEstado(); 
                var dt = Utils.ConvertToDatatable(new NodeBL().ReporteEmpresa());
                dt.TableName = "dsEmpresa";
                var dt2 = Utils.ConvertToDatatable(content);
                dt2.TableName = "dsOrdenCompraEstado";
                var ds = new DataSet();
                ds.Tables.AddRange(new []{dt, dt2});
                rp.SetDataSource(ds);
                crystalReportViewer1.ReportSource = rp;
                crystalReportViewer1.Show();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        #endregion

        #region Events

        private void txtProveedor_Validated(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProveedor.Text))
                txtProveedor.Tag = string.Empty;
        }

        private void btnVisualizar_Click(object sender, EventArgs e)
        {
            CargarReporte();
        }

        private void txtProveedor_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var frm = new Mantenimientos.frmBuscarProveedor("", "");
            frm.ShowDialog();
            if (frm._IdProveedor != null)
            {
                txtProveedor.Text = frm._CodigoProveedor;
                txtProveedor.Tag = frm._IdProveedor;
            }
            else
                txtProveedor.Tag = string.Empty;
        }

        private void frmReporteOrdenCompraEstado_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }
        #endregion
    }
}
