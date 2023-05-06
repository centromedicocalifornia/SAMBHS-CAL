using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Venta.BL;
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

namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    public partial class frmReporteVentasDetraccion : Form
    {

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        public frmReporteVentasDetraccion(string N)
        {
            InitializeComponent();
        }

        private void btnVisualizar_Click(object sender, EventArgs e)
        {
            CargarReporte();
        }
        private void CargarReporte()
        {

            List<ReporteVentasDetraccion> Reporte = new List<ReporteVentasDetraccion>();
            OcultarMostrarBuscar(true);
            OperationResult objOperationResult = new OperationResult();
            VentaBL _objVentaBL = new VentaBL();
            Cursor.Current = Cursors.WaitCursor;
            Task.Factory.StartNew(() =>
            {
                Reporte = _objVentaBL.ReporteVentasDetraccion(ref objOperationResult, DateTime.Parse(dtpFechaRegistroDe.Text), DateTime.Parse(dtpFechaRegistroAl.Text.Trim() + " 23:59"), decimal.Parse(txtMontoDetraccion.Text.ToString()), int.Parse ( cboDetalleDocumento.Value.ToString()),txtProducto.Text.Trim (),txtProveedor.Text.Trim (),int.Parse (cboCondicionPago.Value.ToString ()));


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
                        UltraMessageBox.Show("Ocurrió un Error al generar Registro de Ventas con detracción", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show("Ocurrió un Error al generar Registro de Ventas con detracción", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }

                var rp = new Reportes.Ventas.crReporteVentasDetraccion();


                var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();
                DataSet ds1 = new DataSet();

                DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(Reporte);
                //DataTable dt2 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate2);

                //   ds1.Tables.Add(dt2);
                ds1.Tables.Add(dt);

                //  ds1.Tables[0].TableName = "dsEmpresa";
                ds1.Tables[0].TableName = "dsRegistroVentasDetraccion";
                rp.SetDataSource(ds1);

                rp.SetParameterValue("NombreEmpresa", aptitudeCertificate2.FirstOrDefault().NombreEmpresaPropietaria.Trim());
                rp.SetParameterValue("RucEmpresa","R.U.C. :"+ aptitudeCertificate2.FirstOrDefault().RucEmpresaPropietaria.Trim());
                rp.SetParameterValue("NroRegistros", Reporte.Count());
                rp.SetParameterValue("FechaReporte", "DEL " + dtpFechaRegistroDe.Text + " AL " + dtpFechaRegistroAl.Text);
                crystalReportViewer1.ReportSource = rp;
                crystalReportViewer1.Show();

                crystalReportViewer1.Zoom(110);




            }
                 , TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void frmReporteVentasDetraccion_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }


        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Registro de Ventas con detracción"
                : @"Registro de Ventas con detracción";
            pBuscando.Visible = estado;

            btnVisualizar.Enabled = !estado;
        }

        private void txtMontoDetraccion_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtMontoDetraccion, e);
        }

        private void frmReporteVentasDetraccion_Load(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            DocumentoBL _objDocumentoBL = new DocumentoBL();
            txtMontoDetraccion.Text = Globals.ClientSession.d_ValorMaximoBoletas.ToString();
            this.BackColor = new GlobalFormColors().FormColor;
            Utils.Windows.LoadUltraComboEditorList(cboDetalleDocumento, "Value1", "Id", _objDocumentoBL.ObtenDocumentosParaCombo(ref objOperationResult, null, 0, 1), DropDownListAction.All);
            cboDetalleDocumento.Value = "1";

            Utils.Windows.LoadUltraComboEditorList(cboCondicionPago, "Value1", "Id",
                   Globals.CacheCombosVentaDto.cboCondicionPago, DropDownListAction.All);
            cboCondicionPago.Value = "-1";
        }
    }
}
