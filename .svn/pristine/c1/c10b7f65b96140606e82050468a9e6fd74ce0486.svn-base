using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.Common.BL;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.BE;
using System.Threading.Tasks;
using System.Threading;


namespace SAMBHS.Windows.WinClient.UI.Reportes.ActivoFijo
{
    public partial class frmReporteUbicacion : Form
    {
        CancellationTokenSource _cts = new CancellationTokenSource();
        public frmReporteUbicacion(string pstrParametro)
        {
            InitializeComponent();
        }

        private void frmReporteUbicacion_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
        }

        private void btnVuisualizar_Click(object sender, EventArgs e)
        {
            try
            {
                    CargarReporte();
            }
            catch (Exception g)
            {
                UltraMessageBox.Show("Se produjo un error  al mostrar el reporte ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
         private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Reporte de Ubicación"
                : @"Reporte de Ubicación";
            pBuscando.Visible = estado;
            btnVuisualizar.Enabled =!estado;
        }
        private void CargarReporte()
        {
            int Grupo = 103;
            OperationResult  objOperationResult = new OperationResult ();
            var rp = new Reportes.ActivoFijo.crReportesVarios();

             OcultarMostrarBuscar(true);
           List<datahierarchyDto> aptitudeCertificate = new List<datahierarchyDto>();
            Cursor.Current = Cursors.WaitCursor;

            Task.Factory.StartNew(() =>
            {


            aptitudeCertificate = new DatahierarchyBL().ReporteDatahierarchyCodigocuatroDigitos(ref objOperationResult , Grupo);

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
                        UltraMessageBox.Show("Ocurrió un error al realizar Reporte de Ubicación", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show("Ocurrió un error al realizar Reporte de Ubicación ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }
            var aptitudeCertificate1 = new NodeBL().ReporteEmpresa();
            DataSet ds1 = new DataSet();

            DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
            DataTable dt2 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate1);

            ds1.Tables.Add(dt);
            ds1.Tables.Add(dt2);
            ds1.Tables[0].TableName = "dsDataHierarchy";
            ds1.Tables[1].TableName = "dsEmpresa";

            rp.SetDataSource(ds1);
            rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count());
            rp.SetParameterValue("Titulo", "UBICACIÓN");
            crystalReportViewer1.ReportSource = rp;
            crystalReportViewer1.Show();
            }
                , TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void frmReporteUbicacion_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }


    }
}
