using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using CrystalDecisions.Shared;
using System.Reflection;
using CrystalDecisions.CrystalReports.Engine;
using SAMBHS.Tesoreria.BL;
using SAMBHS.Common.BL;
using SAMBHS.Common.BE;
using System.Threading.Tasks;
using System.Threading;


namespace SAMBHS.Windows.WinClient.UI.Reportes.Tesoreria
{
    public partial class frmReporteChequesGirados : Form
    {
        TesoreriaBL _objTesoreriaBL = new TesoreriaBL();
        CancellationTokenSource _cts = new CancellationTokenSource();
        public frmReporteChequesGirados(string parametro)
        {
            InitializeComponent();
        }

        private void frmReporteChequesGirados_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            //rbtnTodos.Checked = true;
            //rbtnFecha.Checked = true;
            cboMoneda.Value = "-1";
            cboOrdenar.Value = "Fecha";
            ValidarFechas();
        }

        private void btnVisualizar_Click(object sender, EventArgs e)
        {
            string FechaImpresion = string.Empty;
            string Cuenta1 = string.Empty, Cuenta2 = string.Empty, strOrderExpression;
            int MonedaFiltrar = -1;
            try
            {
                if (Validaciones.Validate(true, false).IsValid)
                {
                    if (CuentasValidas())
                    {
                        FechaImpresion = "DEL " + dtpFechaInicio.Text + " AL " + dtpFechaFin.Text;
                        //strOrderExpression = rbtnComprobante.Checked == true ? "NumeroComprobante" : "";
                        //strOrderExpression += rbtnFecha.Checked == true ? "Fecha" : "";
                        //strOrderExpression += rbtnDocumento.Checked == true ? "NumeroDocumento" : "";
                        //strOrderExpression += rbtnNombre.Checked == true ? "Detalle" : "";
                        //strOrderExpression += rbtnImporte.Checked == true ? "Importe" : "";
                        strOrderExpression = cboOrdenar.Value.ToString();
                        MonedaFiltrar = int.Parse ( cboMoneda.Value.ToString());
                        //if (rbtnSoles.Checked == true)
                        //{
                        //    MonedaFiltrar = 1;

                        //}
                        //if (rbtnDolares.Checked == true)
                        //{
                        //    MonedaFiltrar = 2;

                        //}
                        //if (rbtnTodos.Checked == true)
                        //{
                        //    MonedaFiltrar = -1;
                        //}


                        CargarReporte(FechaImpresion, dtpFechaInicio.Value.Date, DateTime.Parse(dtpFechaFin.Value.Day.ToString() + "/" + dtpFechaFin.Value.Month.ToString() + "/" + dtpFechaFin.Value.Year.ToString() + " 23:59"), txtCuenta1.Text.Trim(), txtCuenta2.Text.Trim(), strOrderExpression, MonedaFiltrar);
                    }
                }
            }
            catch
            {
                UltraMessageBox.Show("Hubo un error al realizar Reporte", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }



        private bool CuentasValidas()
        {
            string NombreCuenta1 = string.Empty, NombreCuenta2 = string.Empty;
            NombreCuenta1 = _objTesoreriaBL.ObtenerNombreCuenta(txtCuenta1.Text.Trim());
            NombreCuenta2 = _objTesoreriaBL.ObtenerNombreCuenta(txtCuenta2.Text.Trim());
            if (txtCuenta1.Text == string.Empty && txtCuenta2.Text == string.Empty) return true;
            if (txtCuenta1.Text != string.Empty && txtCuenta2.Text != string.Empty)
            {
                NombreCuenta1 = _objTesoreriaBL.ObtenerNombreCuenta(txtCuenta1.Text.Trim());
                NombreCuenta2 = _objTesoreriaBL.ObtenerNombreCuenta(txtCuenta2.Text.Trim());

                if (NombreCuenta1 == string.Empty)
                {
                    UltraMessageBox.Show("Debe ingresar una cuenta Inicial Válida", "Error de Validación ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtCuenta1.Focus();
                    return false;

                }
                if (NombreCuenta2 == string.Empty)
                {
                    UltraMessageBox.Show("Debe ingresar una cuenta Final Válida", "Error de Validación ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtCuenta2.Focus();
                    return false;

                }


            }
            else
            {

                if (txtCuenta1.Text != string.Empty && txtCuenta2.Text == string.Empty)
                {
                    UltraMessageBox.Show("Debe ingresar una cuenta Final Válida", "Error de Validación ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtCuenta2.Focus();
                    return false;

                }
                else
                {

                    UltraMessageBox.Show("Debe ingresar una cuenta Inicial  Válida", "Error de Validación ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtCuenta1.Focus();
                    return false;
                }

            }

            if (NombreCuenta1 != string.Empty && NombreCuenta2 != string.Empty)
            {
                if (int.Parse(txtCuenta1.Text.Trim()) > int.Parse(txtCuenta2.Text.Trim()))
                {
                    UltraMessageBox.Show("Debe ingresar los rangos de menor a mayor", "Error de Validación ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

            }
            return true;

        }

        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Reporte de Cheques Girados"
                : @"Reporte de Cheques Girados";
            pBuscando.Visible = estado;
            btnVisualizar.Enabled = !estado;
        }
        private void CargarReporte(string FechaImpresion, DateTime FechaInicio, DateTime FechaFin, string Cuenta1, string Cuenta2, string Orden, int MonedaFiltrar)
        {

            var rp = new Reportes.Tesoreria.crReporteChequesGirados();
            OperationResult objOperationResult = new OperationResult();
            List<ReporteChequesGirados> aptitudeCertificate = new List<ReporteChequesGirados>();

            OcultarMostrarBuscar(true);
            Cursor.Current = Cursors.WaitCursor;

            Task.Factory.StartNew(() =>
            {

                if (Cuenta2 == string.Empty && Cuenta1 == string.Empty)
                {
                    aptitudeCertificate = new TesoreriaBL().ReporteChequesGiradosSinRango(ref objOperationResult, FechaInicio, FechaFin, Orden, MonedaFiltrar);

                }
                else
                {
                    aptitudeCertificate = new TesoreriaBL().ReporteChequesGiradosRango(ref objOperationResult, FechaInicio, FechaFin, Orden, MonedaFiltrar, Cuenta1, Cuenta2);
                }
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
                        UltraMessageBox.Show(objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error " + " Reporte de Cheques Girados", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error" + " Reporte de Cheques Girados", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }



                var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();

                DataSet ds1 = new DataSet();
                DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
                DataTable dt2 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate2);

                ds1.Tables.Add(dt);
                ds1.Tables.Add(dt2);
                ds1.Tables[0].TableName = "dsReporteChequesGirados";
                ds1.Tables[1].TableName = "dsEmpresa";
                rp.SetDataSource(ds1);
                rp.SetParameterValue("FechaImpresion", FechaImpresion);
                rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count());
                crystalReportViewer1.ReportSource = rp;
                crystalReportViewer1.Show();
            }
                    , TaskScheduler.FromCurrentSynchronizationContext());


        }

        private void btnBuscarCuenta1_Click(object sender, EventArgs e)
        {
            string NombreCuenta = string.Empty;
            string NumeroCuenta1 = txtCuenta1.Text == string.Empty ? "10" : txtCuenta1.Text.Trim();
            Mantenimientos.frmPlanCuentasConsulta frmPlanCuentasConsulta = new Mantenimientos.frmPlanCuentasConsulta(NumeroCuenta1);

            frmPlanCuentasConsulta.ShowDialog();
            if (frmPlanCuentasConsulta._NroSubCuenta != null)
            {
                txtCuenta1.Text = frmPlanCuentasConsulta._NroSubCuenta;
                           }
            else
            {

                txtCuenta1.Clear();
               
            }

        }

        private void btnBuscarCuenta2_Click(object sender, EventArgs e)
        {
            string NombreCuenta = string.Empty;
            string NumeroCuenta2 = txtCuenta1.Text == string.Empty ? "10" : txtCuenta1.Text.Trim();
            Mantenimientos.frmPlanCuentasConsulta frmPlanCuentasConsulta = new Mantenimientos.frmPlanCuentasConsulta(NumeroCuenta2);

            frmPlanCuentasConsulta.ShowDialog();
            if (frmPlanCuentasConsulta._NroSubCuenta != null)
            {
                txtCuenta2.Text = frmPlanCuentasConsulta._NroSubCuenta;
                
            }
            else
            {

                txtCuenta2.Clear();
             

            }
        }

        private void txtCuenta1_Validating(object sender, CancelEventArgs e)
        {
            string NombreCuenta = string.Empty;
            NombreCuenta = _objTesoreriaBL.ObtenerNombreCuenta(txtCuenta1.Text.Trim());
            if (NombreCuenta != string.Empty)
            {
                txtCuenta2.Text = txtCuenta1.Text;

            }
           
        }

        private void txtCuenta2_Validating(object sender, CancelEventArgs e)
        {
            string NombreCuenta = string.Empty;
            NombreCuenta = _objTesoreriaBL.ObtenerNombreCuenta(txtCuenta2.Text.Trim());
           
        }

        private void txtCuenta1_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCuenta1, e);
        }

        private void txtCuenta2_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCuenta2, e);
        }

        private void ValidarFechas()
        {
            string Periodo = Globals.ClientSession.i_Periodo.ToString();
            string Mes = DateTime.Today.Month.ToString();
            if (DateTime.Now.Year.ToString().Trim() == Periodo)
            {
                dtpFechaInicio.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/" + "01").ToString());
                dtpFechaInicio.MaxDate = DateTime.Parse((Periodo + "/" + Mes + "/" + (DateTime.DaysInMonth(int.Parse(Periodo), int.Parse(Mes))).ToString()).ToString());
                dtpFechaInicio.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());

                dtpFechaFin.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/" + "01").ToString());
                dtpFechaFin.MaxDate = DateTime.Parse((Periodo + "/" + Mes + "/" + (DateTime.DaysInMonth(int.Parse(Periodo), int.Parse(Mes))).ToString()).ToString());
                dtpFechaFin.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());

            }
            else
            {
                dtpFechaInicio.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/01").ToString());
                dtpFechaInicio.MaxDate = DateTime.Parse((Periodo + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(Periodo), 12)).ToString()).ToString());
                dtpFechaInicio.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                dtpFechaFin.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/01").ToString());
                dtpFechaFin.MaxDate = DateTime.Parse((Periodo + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(Periodo), 12)).ToString()).ToString());
                dtpFechaFin.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());

            }
        }

        private void dtpFechaFin_ValueChanged(object sender, EventArgs e)
        {
            dtpFechaInicio.MaxDate = dtpFechaFin.Value;
        }

        private void frmReporteChequesGirados_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }

        private void txtNroCuenta_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            string NombreCuenta = string.Empty;
            string NumeroCuenta1 = txtCuenta1.Text == string.Empty ? "10" : txtCuenta1.Text.Trim();
            Mantenimientos.frmPlanCuentasConsulta frmPlanCuentasConsulta = new Mantenimientos.frmPlanCuentasConsulta(NumeroCuenta1);

            frmPlanCuentasConsulta.ShowDialog();
            if (frmPlanCuentasConsulta._NroSubCuenta != null)
            {
                txtCuenta1.Text = frmPlanCuentasConsulta._NroSubCuenta;
                
            }
            else
            {

                txtCuenta1.Clear();
               

            }

        }

        private void ultraExpandableGroupBox1_ExpandedStateChanged(object sender, EventArgs e)
        {
            if (ultraExpandableGroupBox1.Expanded == true)
            {
                groupBox2.Location = new Point(groupBox2.Location.X, ultraExpandableGroupBox1.Location.Y + ultraExpandableGroupBox1.Height + 5);
                groupBox2.Height = this.Height - groupBox2.Location.Y - 7;
            }
            else
            {
                groupBox2.Location = new Point(groupBox2.Location.X, ultraExpandableGroupBox1.Location.Y + ultraExpandableGroupBox1.Height + 5);
                groupBox2.Height = this.Height - groupBox2.Location.Y - 7;
            }
        }


    }
}
