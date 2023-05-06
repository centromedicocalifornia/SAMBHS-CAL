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
    public partial class frmReporteCuadresCajasChicas : Form
    {

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        public frmReporteCuadresCajasChicas(string N)
        {
            InitializeComponent();
        }

        private void btnVisualizar_Click(object sender, EventArgs e)
        {
            CargarReporte();
        }
        private void CargarReporte()
        {

            List<ReporteDocumentoCajaChica> Reporte = new List<ReporteDocumentoCajaChica>();
            OcultarMostrarBuscar(true);
            OperationResult objOperationResult = new OperationResult();
            CajaChicaBL _objCajaChicaBL = new CajaChicaBL();
            Cursor.Current = Cursors.WaitCursor;
            Task.Factory.StartNew(() =>
            {

                Reporte = _objCajaChicaBL.RepoorteDocumentoCajaChica(ref objOperationResult, null, DateTime.Parse(dtpFechaRegistroDe.Text), DateTime.Parse(dtpFechaRegistroAl.Text + " 23:59"), true);

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
                        UltraMessageBox.Show("Ocurrió un Error al generar Reportes Cuadres de Caja Chica", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show("Ocurrió un Error al generar Reportes Cuadres de Caja Chica", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }

                var rp = new Reportes.Ventas.crDocumentoCajaChica();

                var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();
                DataSet ds1 = new DataSet();

                DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(Reporte);
                ds1.Tables.Add(dt);
                ds1.Tables[0].TableName = "dsDocumentoCajaChica";
                rp.SetDataSource(ds1);

                rp.SetParameterValue("NombreEmpresa", aptitudeCertificate2.FirstOrDefault().NombreEmpresaPropietaria.Trim());
                rp.SetParameterValue("RucEmpresa","R.U.C. :"+ aptitudeCertificate2.FirstOrDefault().RucEmpresaPropietaria.Trim());
               
                crystalReportViewer1.ReportSource = rp;
                crystalReportViewer1.Show();

                crystalReportViewer1.Zoom(110);




            }
                 , TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void frmReporteCuadresCajasChicas_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }
        private void frmReporteCuadresCajasChicas_Load(object sender, EventArgs e)
        {
            
        }

        
        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Reportes Cuadres de Caja Chica"
                : @"Reportes Cuadres de Caja Chica";
            pBuscando.Visible = estado;

            btnVisualizar.Enabled = !estado;
        }

       
    }
}
