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
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using SAMBHS.Cobranza.BL;
using SAMBHS.Common.BL;
using SAMBHS.Venta.BL;
using SAMBHS.Common.BE;
using System.Threading.Tasks;
using System.Threading;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Cobranza
{
    public partial class frmReporteEstadoCuentaCliente : Form
    {

        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        ClienteBL _objClienteBL = new ClienteBL();
        CancellationTokenSource _cts = new CancellationTokenSource();
        public frmReporteEstadoCuentaCliente(string pstrParametro)
        {
            InitializeComponent();
        }

        private void txtCliente_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscarCliente")
            {
                OperationResult objOperationResult = new OperationResult();
                Mantenimientos.frmBuscarCliente frm = new Mantenimientos.frmBuscarCliente("VV", txtCliente.Text.Trim());
                frm.ShowDialog();

                if (frm._IdCliente != null)
                {
                    txtCliente.Text = frm._CodigoCliente.Trim().ToUpper();
                    txtRazonSocial.Text = frm._RazonSocial.Trim().ToUpper();
                }
                else
                {

                }
            }
        }

        private void frmReporteEstadoCuentaCliente_Load(object sender, EventArgs e)
        {

            this.BackColor = new GlobalFormColors().FormColor;
            CargarCombos();
            ValidarFechas();

        }




        private void ValidarFechas()
        {
            string Periodo = Globals.ClientSession.i_Periodo.ToString();
            string Mes = DateTime.Today.Month.ToString();
            //if (DateTime.Now.Year.ToString().Trim() == Periodo)
            //{
            //    dtpFecha.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/" + "01").ToString());
            //    dtpFecha.MaxDate = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());
           
            //}
            //else
            //{
            //    dtpFecha.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/01").ToString());
            //    dtpFecha.MaxDate = DateTime.Parse((Periodo + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(Periodo), 12)).ToString()).ToString());
              
            //}

            dtpFecha.Value = DateTime.Parse((Periodo + "/" + " 01/01").ToString());
        }

        private void CargarCombos()
        {
            OperationResult objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList(cboOrden, "Value1", "Value2", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 80, null), DropDownListAction.Select);
            cboOrden.SelectedIndex = 1;
        }

        private void btnVisualizar_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime FecHasta = dtpFecha.Value.Date.AddDays(-1);
                DateTime FechaSaldoHasta = DateTime.Parse(FecHasta.Date.Day.ToString("00") + "/" + FecHasta.Date.Month.ToString("00") + "/" + FecHasta.Date.Year.ToString() + " 23:59");                //                 DateTime orderDate = DateTime.Now.AddDays(-1);
                DateTime FechaHasta = DateTime.Now;
                DateTime FechaDesde = dtpFecha.Value.Date;
                if (uvReporte.Validate(true, false).IsValid)
                {

                    CargarReporte(FechaSaldoHasta, FechaHasta, FechaDesde);
                }
                else
                {
                    UltraMessageBox.Show("Por favor llene los campos requeridos para visualizar el reporte", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch
            {
                UltraMessageBox.Show("Se produjo un error  al mostrar el reporte ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Estado de Cuenta Cliente"
                : @"Estado de Cuenta Cliente";
            pBuscando.Visible = estado;
            btnVisualizar.Enabled = !estado;

        }

        private void CargarReporte(DateTime FechaSaldoDeudor, DateTime FechaHasta, DateTime FechaDesde)
        {


            OperationResult objOperationResult = new OperationResult();
            List<string> Retonar = new List<string>();
            List<string> Retonar2 = new List<string>();
            var rp = new Reportes.Cobranza.crReporteEstadoCuentaCliente();
            List<ReporteEstadoCuentaCliente> aptitudeCertificate = new List<ReporteEstadoCuentaCliente>();


            OcultarMostrarBuscar(true);
            Cursor.Current = Cursors.WaitCursor;

            Task.Factory.StartNew(() =>
            {

                aptitudeCertificate = new CobranzaBL().ReporteEstaCuentaCliente(ref objOperationResult, txtCliente.Text.Trim(), FechaSaldoDeudor, FechaDesde, FechaHasta);
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
                        UltraMessageBox.Show(objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error " + " Estado de Cuenta Cliente", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error" + " Estado de Cuenta Cliente", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }


                var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();
                DataSet ds1 = new DataSet();
                DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
                DataTable dt2 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate2);

                ds1.Tables.Add(dt);
                ds1.Tables.Add(dt2);
                ds1.Tables[0].TableName = "dsReporteEstadoCuentaCliente";
                ds1.Tables[1].TableName = "dsEmpresa";

                rp.SetDataSource(ds1);
                rp.SetParameterValue("Fecha", "DEL " + FechaDesde.Date.Day.ToString("00") + "/" + FechaDesde.Date.Month.ToString("00") + "/" + FechaDesde.Date.Year.ToString() + "   AL   " + FechaHasta.Date.Day.ToString("00") + "/" + FechaHasta.Date.Month.ToString("00") + "/" + FechaHasta.Date.Year.ToString());
                rp.SetParameterValue("CantidadDecimalPrecio", (int)Globals.ClientSession.i_PrecioDecimales);
                rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count());
                rp.SetParameterValue("FechaSaldoDeudor", "SALDO DEUDOR AL   : " + FechaHasta.Date.Day.ToString("00") + "/" + FechaHasta.Date.Month.ToString("00") + "/" + FechaHasta.Date.Year.ToString());
                rp.SetParameterValue("NombreEmpresa", aptitudeCertificate2.FirstOrDefault().NombreEmpresaPropietaria.Trim());
                rp.SetParameterValue("RucEmpresa","R.U.C. : "+ aptitudeCertificate2.FirstOrDefault().RucEmpresaPropietaria.Trim());
                
                
                
                crystalReportViewer1.ReportSource = rp;
                crystalReportViewer1.Show();
            }
                , TaskScheduler.FromCurrentSynchronizationContext());



        }

        private void txtCliente_Validated(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (txtCliente.Text.Trim() != string.Empty)
            {
                var Cliente = _objClienteBL.ObtenerClienteCodigoBandejasCodigo(ref objOperationResult, txtCliente.Text.Trim(), "C");
                if (Cliente != null)
                {
                    txtRazonSocial.Text = (Cliente.v_ApePaterno + " " + Cliente.v_ApeMaterno.Trim() + " " + Cliente.v_PrimerNombre.Trim() + " " + Cliente.v_SegundoNombre.Trim() + " " + Cliente.v_RazonSocial.Trim()).Trim().ToUpper();
                }
                else
                {
                    txtRazonSocial.Clear();
                }

            }
            else
            {
                txtRazonSocial.Clear();
            }
        }

        private void frmReporteEstadoCuentaCliente_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }
    }
}
