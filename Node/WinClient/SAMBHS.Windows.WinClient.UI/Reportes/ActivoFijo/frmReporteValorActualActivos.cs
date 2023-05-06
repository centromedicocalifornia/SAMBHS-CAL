using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using SAMBHS.ActivoFijo.BL;
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;
using System.Threading.Tasks;
using SAMBHS.Common.BE;
using System.Threading;

namespace SAMBHS.Windows.WinClient.UI.Reportes.ActivoFijo
{
    public partial class frmReporteValorActualActivos : Form
    {
        CancellationTokenSource _cts = new CancellationTokenSource();
        public frmReporteValorActualActivos(string Parametro)
        {
            InitializeComponent();
        }

        private void frmReporteValorActualActivos_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            LlenarCombo();
            dtpFechaCompra.Enabled = Globals.ClientSession.i_SystemUserId == 1;
            dtpFechaCompra.Maximum = 2090;
            dtpFechaCompra.Minimum = 2015;
            dtpFechaCompra.Value =int.Parse ( Globals.ClientSession.i_Periodo.ToString());
            
        }

        private void LlenarCombo()
        {
            //pstrOrigen = "Load";
            String[] Meses = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
            String[] MesesMayuscula = new String[12];
            int i = 0;
            foreach (var s in Meses)
            {
                if (i < 12)
                {
                    try
                    {
                        string a = s.ToUpper();
                        MesesMayuscula[i] = a;
                        i = i + 1;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }

            }

            cboMeses.Items.AddRange(MesesMayuscula);
            cboMeses.SelectedText = "--Seleccionar--";
            int x = int.Parse(DateTime.Now.Month.ToString());
            cboMeses.Text = Mes(x);
        }
        private int Mes(string pstrMes)
        {
            switch (pstrMes)
            {
                case "ENERO": return 1;
                case "FEBRERO": return 2;
                case "MARZO": return 3;
                case "ABRIL": return 4;
                case "MAYO": return 5;
                case "JUNIO": return 6;
                case "JULIO": return 7;
                case "AGOSTO": return 8;
                case "SEPTIEMBRE": return 9;
                case "OCTUBRE": return 10;
                case "NOVIEMBRE": return 11;
                case "DICIEMBRE": return 12;

            }
            return 0;
        }

        private string Mes(int mes)
        {
            switch (mes)
            {
                case 1: return "ENERO";
                case 2: return "FEBRERO";
                case 3: return "MARZO";
                case 4: return "ABRIL";
                case 5: return "MAYO";
                case 6: return "JUNIO";
                case 7: return "JULIO";
                case 8: return "AGOSTO";
                case 9: return "SEPTIEMBRE";
                case 10: return "OCTUBRE";
                case 11: return "NOVIEMBRE";
                case 12: return "DICIEMBRE";


            }
            return string.Empty;
        }

        private void btnVisualizar_Click(object sender, EventArgs e)
        {
            try
            {
                if (uvValidar.Validate(true, false).IsValid)
                {
                    string  CuentaDesde = "", CuentaHasta = "";
                    CuentaDesde = txtCuentaInicial.Text == string.Empty ? "" : txtCuentaInicial.Text.Trim();
                    CuentaHasta = txtCuentaFinal.Text == String.Empty ? "" :txtCuentaFinal.Text.Trim();
                 
                    CargarReporte(int.Parse ( dtpFechaCompra.Value.ToString()), Mes(cboMeses.Text.Trim()), CuentaDesde, CuentaHasta, txtCentroCosto.Text.Trim());
                }
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
                ? @"Generando... " + "Reporte de Valor Actual de Activos"
                : @"Reporte de Valor Actual de Activos";
            pBuscando.Visible = estado;
            
            btnVisualizar.Enabled = !estado;
        }
        private void CargarReporte(int periodo, int mes, string  CuentaDesde, string  CuentaHasta, string CentroCosto)
        {
            var rp = new Reportes.ActivoFijo.crReporteValorActualActivos();

            OperationResult objOperationResult = new OperationResult ();

              OcultarMostrarBuscar(true);
             List<ReporteValorActualActivosDto> aptitudeCertificate = new List<ReporteValorActualActivosDto>();
            Cursor.Current = Cursors.WaitCursor;

            Task.Factory.StartNew(() =>
            {

            aptitudeCertificate = new ActivoFijoBL().ReporteValorActualActivos(ref objOperationResult ,  periodo, mes, CuentaDesde, CuentaHasta, CentroCosto); 
                
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
            var Empresa = new NodeBL().ReporteEmpresa();
            DataSet ds1 = new DataSet();
            DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
            ds1.Tables.Add(dt);
            ds1.Tables[0].TableName = "dsReporteValorActualActivos";
            rp.SetDataSource(ds1);
            rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count());
            rp.SetParameterValue("NombreEmpresa", Empresa.FirstOrDefault().NombreEmpresaPropietaria.Trim());
            rp.SetParameterValue("RucEmpresa", "R.U.C. : "+Empresa.FirstOrDefault().RucEmpresaPropietaria.Trim());
            rp.SetParameterValue("Titulo", "VALOR ACTUAL DE ACTIVOS AL MES DE  " + cboMeses.Text.Trim() + " DEL " + periodo.ToString());
            rp.SetParameterValue("IncluirNroPagina", uckNroPagina.Checked);
            crystalReportViewer1.ReportSource = rp;
            crystalReportViewer1.Show();
            }
                , TaskScheduler.FromCurrentSynchronizationContext());




        }

        private void txtCuentaInicial_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            Mantenimientos.frmPlanCuentasConsulta frm = new Mantenimientos.frmPlanCuentasConsulta("33");
            frm.ShowDialog();
            txtCuentaInicial.Text = frm._NroSubCuenta;
        }

        private void txtCuentaFinal_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            Mantenimientos.frmPlanCuentasConsulta frm = new Mantenimientos.frmPlanCuentasConsulta("33");
            frm.ShowDialog();
            txtCuentaFinal.Text = frm._NroSubCuenta;
        }

        private void txtCentroCosto_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            Mantenimientos.frmBuscarDatahierarchy frm2 = new Mantenimientos.frmBuscarDatahierarchy(31, "Buscar Centro de Costos");
            frm2.ShowDialog();
            if (frm2._itemId != null)
            {
                txtCentroCosto.Text = frm2._value2.Trim();
            }
        }

        private void txtCuentaInicial_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCuentaInicial, e);
        }

        private void txtCuentaFinal_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCuentaFinal, e);
        }

        private void txtCentroCosto_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCentroCosto, e);
        }

        private void txtCuentaInicial_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtCuentaFinal.Text))
            {
                txtCuentaFinal.Text = txtCuentaInicial.Text != string.Empty ? txtCuentaInicial.Text.Trim() : "";
            }
        }

        private void txtCuentaFinal_Validating(object sender, CancelEventArgs e)
        {
            //txtCuentaFinal.Text = txtCuentaInicial.Text != string.Empty && txtCuentaFinal.Text == string.Empty ? txtCuentaInicial.Text.Trim() : "";

            if (txtCuentaFinal.Text == string.Empty && txtCuentaInicial.Text != string.Empty)
            {
                txtCuentaFinal.Text = txtCuentaInicial.Text;
            }
        }

        private void txtCuentaFinal_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void frmReporteValorActualActivos_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }

    }
}
