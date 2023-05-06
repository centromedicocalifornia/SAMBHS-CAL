using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.BL;
using SAMBHS.ActivoFijo.BL;
using System.Threading;
using System.Threading.Tasks;
using SAMBHS.Common.BE;

namespace SAMBHS.Windows.WinClient.UI.Reportes.ActivoFijo
{
    public partial class frmReporteAsientoInventario : Form
    {
        CancellationTokenSource _cts = new CancellationTokenSource();

        public frmReporteAsientoInventario(string pstr)
        {
            InitializeComponent();
        }

        private void frmReporteAsientoInventario_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            ValidarFecha();
        }
        private void ValidarFecha()
        {

            nupAnio.Minimum = 1960;
            nupAnio.Maximum = 3000;
            nupAnio.Value = int.Parse(Globals.ClientSession.i_Periodo.ToString());
            nupMes.Minimum = 1;
            nupMes.Maximum = 12;
            nupMes.Value = decimal.Parse(DateTime.Now.Month.ToString());




        }

        private void txtRangoBienes_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {



        }

        private void txtCodigoHasta_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

        }

        private void btnVisualizar_Click(object sender, EventArgs e)
        {
            try
            {

                if (string.IsNullOrEmpty(txtTipoCambio.Text.Trim()) || decimal.Parse(txtTipoCambio.Text.Trim()) == 0)
                {
                    UltraMessageBox.Show("Ingrese Tipo Cambio válido");
                    return;
                }

                int Periodo = int.Parse(nupAnio.Value.ToString());
                int Mes = int.Parse(DateTime.Now.Date.Month.ToString());
                int CodigoDesde = 0, CodigoHasta = 0;


                CargarReporte(Periodo, Mes, CodigoDesde, CodigoHasta);
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show("Ocurrió un Error al Realizar el Reporte", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

        }

        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Reporte Asiento Inventario"
                : @"Reporte Asiento Inventario";
            pBuscando.Visible = estado;
            btnVisualizar.Enabled = !estado;
        }

        private void CargarReporte(int periodo, int mes, int CodigoDesde, int CodigoHasta)
        {
            var rp = new Reportes.ActivoFijo.crReporteAsientoInventario();
            OperationResult objOperationResult = new OperationResult();
            OcultarMostrarBuscar(true);

            Cursor.Current = Cursors.WaitCursor;
            List<ReporteAsientoDiario> aptitudeCertificate = new List<ReporteAsientoDiario>();
            Task.Factory.StartNew(() =>
            {


                aptitudeCertificate = new ActivoFijoBL().ReporteAsientoInventario(ref objOperationResult, int.Parse(nupAnio.Value.ToString()), int.Parse(nupMes.Value.ToString()), uckAsientoDepreciacion.Checked, uckAsientoAjuste.Checked, decimal.Parse(txtTipoCambio.Text.Trim()));

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
                        UltraMessageBox.Show("Ocurrió un error al realizar Reporte de Libro Registro de Activo Fijo", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show("Ocurrió un error al realizar Reporte de Libro Registro de Activo Fijo", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }



                var aptitudeCertificate1 = new NodeBL().ReporteEmpresa();
                DataSet ds1 = new DataSet();
                DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);

                ds1.Tables.Add(dt);
                ds1.Tables[0].TableName = "dsReporteAsientoDiario";

                rp.SetDataSource(ds1);
                rp.SetParameterValue("_NombreEmpresa", "");
                rp.SetParameterValue("_FechaHoraImpresion", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                rp.SetParameterValue("_UsuarioReporte", Globals.ClientSession.v_UserName);
                crystalReportViewer1.ReportSource = rp;


                crystalReportViewer1.ReportSource = rp;
                crystalReportViewer1.Show();
            }
                , TaskScheduler.FromCurrentSynchronizationContext());

        }




        private void frmLibroRegistroActivoFijo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }

        private void txtTipoCambio_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Utils.Windows.NumeroDecimalUltraTextBox((sender as Infragistics.Win.UltraWinEditors.UltraTextEditor), e);
            //Utils.Windows.NumeroDecimalUltraTextBox(txtTipoCambio.Text, e);
            //Utils.Windows.NumeroDecimal2UltraTextBox(txtTipoCambio, e);
        }
    }
}
