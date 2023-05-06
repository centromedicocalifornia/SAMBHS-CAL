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
    public partial class frmLibroRegistroActivoFijo : Form
    {
        CancellationTokenSource _cts = new CancellationTokenSource();

        public frmLibroRegistroActivoFijo(string pstr)
        {
            InitializeComponent();
        }

        private void frmLibroRegistroActivoFijo_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            ValidarFecha();
            nupAnio.Enabled = Globals.ClientSession.i_SystemUserId == 1;
        }
        private void ValidarFecha()
        {

            nupAnio.Minimum = 1960;
            nupAnio.Maximum = 3000;
            nupAnio.Value = int.Parse(Globals.ClientSession.i_Periodo.ToString());

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
                int Periodo = int.Parse(nupAnio.Value.ToString());
                int Mes = int.Parse(DateTime.Now.Date.Month.ToString());
                int CodigoDesde = 0, CodigoHasta = 0;
                CodigoDesde = txtCodigoDesde.Text == string.Empty ? 0 : int.Parse(txtCodigoDesde.Text.Trim());
                CodigoHasta = txtCodigoHasta.Text == String.Empty ? 0 : int.Parse(txtCodigoHasta.Text.Trim());
               
                    CargarReporte(Periodo, 12, CodigoDesde, CodigoHasta);
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
                ? @"Generando... " + "Reporte de Libro Registro de Activo Fijo"
                : @"Reporte de Libro Registro de Activo Fijo";
            pBuscando.Visible = estado;
            btnVisualizar.Enabled = !estado;
        }

        private void CargarReporte(int periodo, int mes, int CodigoDesde, int CodigoHasta)
        {
            var rp = new Reportes.ActivoFijo.crReporteLibroRegistroActivoFijo();
            OperationResult objOperationResult = new OperationResult();
             OcultarMostrarBuscar(true);

            Cursor.Current = Cursors.WaitCursor;
            List<ReporteLibroActivoDto> aptitudeCertificate = new List<ReporteLibroActivoDto>();
            Task.Factory.StartNew(() =>
            {

            

                aptitudeCertificate = new ActivoFijoBL().ReporteLibroActivo(ref objOperationResult, periodo, mes, CodigoDesde, CodigoHasta, false, null);

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
            ds1.Tables[0].TableName = "dsReporteLibroActivos";
            rp.SetDataSource(ds1);
            rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count());
            rp.SetParameterValue("NombreEmpresa", aptitudeCertificate1.FirstOrDefault().NombreEmpresaPropietaria.Trim());
            rp.SetParameterValue("RucEmpresa","R.U.C. :"+ aptitudeCertificate1.FirstOrDefault().RucEmpresaPropietaria.Trim());
            rp.SetParameterValue("UltimoElemento",aptitudeCertificate.Count ()>0 ? aptitudeCertificate.GroupBy (x=>new {x.DescripcionActivo }).Select (group=> group.Last ()).ToList ().OrderBy (x=>x.DescripcionActivo).LastOrDefault ().DescripcionActivo  : ""  );
            //aptitudeCertificate.LastOrDefault ().DescripcionActivo
            rp.SetParameterValue("Periodo","PERIODO : "+ nupAnio.Value.ToString () );
            rp.SetParameterValue("IncluirNroPagina", uckNroPagina.Checked);
            crystalReportViewer1.ReportSource = rp;
            crystalReportViewer1.Show();
            }
                , TaskScheduler.FromCurrentSynchronizationContext());

        }

        private void txtCodigoDesde_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCodigoDesde, e); 
        }

        private void txtCodigoHasta_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCodigoHasta, e); 
        }

        private void txtCodigoDesde_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtCodigoDesde, "{0:00000000}");
        }

        private void txtCodigoHasta_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtCodigoHasta, "{0:00000000}");
            
        }

        private void txtCodigoDesde_Validating(object sender, CancelEventArgs e)
        {
            if (txtCodigoDesde.Text != string.Empty)
            {
                txtCodigoHasta.Text = txtCodigoDesde.Text;
            }
            else
            {
                txtCodigoHasta.Text = string.Empty;
            }

        }

        private void txtCodigoHasta_Validating(object sender, CancelEventArgs e)
        {
            if (txtCodigoHasta.Text == string.Empty && txtCodigoDesde.Text != string.Empty)
            {
                txtCodigoHasta.Text = txtCodigoDesde.Text;
            }
            //else
            //{
            //    txtCodigoHasta.Text = string.Empty; 
            //}
        }

        private void frmLibroRegistroActivoFijo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }
    }
}
