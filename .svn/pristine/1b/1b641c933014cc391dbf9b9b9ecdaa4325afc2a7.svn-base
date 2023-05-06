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
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.Tesoreria.BL;
using System.Threading.Tasks;
using System.Threading;
using Infragistics.Win.UltraWinGrid;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Tesoreria
{
    public partial class frmReporteComprobanteIngreso : Form
    {
        CancellationTokenSource _cts = new CancellationTokenSource();
        List<GridKeyValueDTO> _ListadoComboDocumentos = new List<GridKeyValueDTO>();
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        public frmReporteComprobanteIngreso(string pstrParameter)
        {
            InitializeComponent();
        }

        private void frmReporteComprobanteIngreso_Load(object sender, EventArgs e)
        {
            CargarCombos();
            this.BackColor = new GlobalFormColors().FormColor;
        }
        private void CargarCombos()
        {

            OperationResult objOperationResult = new OperationResult();
            lblPeriodo.Text = "Periodo :" + Globals.ClientSession.i_Periodo.ToString();
            _ListadoComboDocumentos = _objDocumentoBL.ObtenDocumentosCobranzaParaComboGrid(ref objOperationResult, null);
            Utils.Windows.LoadUltraComboList(cboTipoDocumento, "Value2", "Id", _ListadoComboDocumentos, DropDownListAction.Select);
            cboTipoDocumento.Value = "-1";

        }

        private void btnVisualizar_Click(object sender, EventArgs e)
        {

            if (uvReporteComprobanteIngreso.Validate(true, false).IsValid)
            {
                if (int.Parse(cboTipoDocumento.Value.ToString()) == -1)
                {
                    UltraMessageBox.Show("Debe elegir un Tipo Documento ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    cboTipoDocumento.Focus();
                    return;
                }

                int TipoDoc = int.Parse(cboTipoDocumento.Value.ToString()) != -1 ? int.Parse(cboTipoDocumento.Value.ToString()) : -1;

                if (txtComprobanteInicial.Text == txtComprobanteFinal.Text)
                {
                }
                else
                {

                    if (txtComprobanteInicial.Text != string.Empty)
                    {
                        if (txtComprobanteFinal.Text == string.Empty)
                        {
                            UltraMessageBox.Show("Debe Ingresar un N° de Documento Final ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtComprobanteFinal.Focus();
                            return;
                        }

                    }

                    else
                    {

                        if (txtComprobanteFinal.Text == string.Empty)
                        {
                            UltraMessageBox.Show("Debe Ingresar un N° de Documento Inicial ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtComprobanteInicial.Focus();
                            return;

                        }

                    }

                    if (int.Parse(txtComprobanteFinal.Text.ToString()) < int.Parse(txtComprobanteInicial.Text.ToString()))
                    {
                        UltraMessageBox.Show("El rango Inicial debe ser menor que el Final ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;

                    }
                }
                int TipoMovimiento = 1;
                string Periodo = Globals.ClientSession.i_Periodo.ToString();
                int DocInicial = txtComprobanteInicial.Text != string.Empty ? int.Parse(txtComprobanteInicial.Text.ToString()) : 0;
                int DocFinal = txtComprobanteFinal.Text != string.Empty ? int.Parse(txtComprobanteFinal.Text.ToString()) : 0;

                int Orden = chkOrden.Checked == true ? 1 : 0;

                try
                {
                    //using (new LoadingClass.PleaseWait(this.Location, "Generando..."))
                    CargarReporte(TipoMovimiento, Periodo, TipoDoc, DocInicial, DocFinal, Orden);

                }
                catch
                {
                    UltraMessageBox.Show("Hubo un error al realizar Reporte", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;

                }
            }
        }

        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Reporte de Comprobantes de  Ingresos"
                : @"Reporte de Comprobantes de  Ingresos";
            pBuscando.Visible = estado;
            btnVisualizar.Enabled = !estado;
        }

        private void CargarReporte(int pIntTipoMovimiento, string pstrPeriodo, int pIntTipoDocumento, int DocInicial, int DocFinal, int Orden)
        {

          
            int numeroElementos = 0;
            string MaxNumeroDocumento = "";
            OperationResult objOperationResult = new OperationResult();

            dynamic rp;
            rp = new Reportes.Tesoreria.crReporteListadoComprobantesTesoreria();
            List<ReporteListadoComprobantesTesoreria> aptitudeCertificate = new List<ReporteListadoComprobantesTesoreria>();

            OcultarMostrarBuscar(true);
            Cursor.Current = Cursors.WaitCursor;

            Task.Factory.StartNew(() =>
            {
                aptitudeCertificate = new TesoreriaBL().ReporteComprobanteTesoreriaIngresosEgresos(ref objOperationResult, pIntTipoMovimiento, pstrPeriodo, pIntTipoDocumento, DocInicial, DocFinal, Orden);
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
                        UltraMessageBox.Show(objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error " + " Reporte de Comprobantes de  Ingresos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error" + " Reporte de Comprobantes de  Ingresos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }


                var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();

                DataSet ds1 = new DataSet();
                DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
                DataTable dt2 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate2);

                ds1.Tables.Add(dt);
                ds1.Tables.Add(dt2);
                ds1.Tables[0].TableName = "dsReporteTesoreriaC";
                ds1.Tables[1].TableName = "dsEmpresa";

                rp.SetDataSource(ds1);

                if (aptitudeCertificate.Count() != 0)
                {
                    numeroElementos = aptitudeCertificate.Select(x => x.v_IdTesoreria).Distinct().Count();
                    MaxNumeroDocumento = aptitudeCertificate.Max(x => x.NumeroDocumento).ToString();
                }
                else
                {
                    numeroElementos = 0;
                }


          

                rp.SetParameterValue("NumeroElementos", numeroElementos);

                rp.SetParameterValue("MaxNumeroDocumento", MaxNumeroDocumento);
                crystalReportViewer1.ReportSource = rp;
                crystalReportViewer1.Show();
            }
                , TaskScheduler.FromCurrentSynchronizationContext());





        }

        private void frmReporteComprobanteIngreso_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }

        private void cboTipoDocumento_AfterDropDown(object sender, EventArgs e)
        {

            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboTipoDocumento.Rows)
            {
                if (cboTipoDocumento.Value == "-1") cboTipoDocumento.Text = string.Empty;
                bool filterRow = true;
                foreach (UltraGridColumn column in cboTipoDocumento.DisplayLayout.Bands[0].Columns)
                {
                    if (column.IsVisibleInLayout)
                    {
                        if (row.Cells[column].Text.Contains(cboTipoDocumento.Text.ToUpper()))
                        {
                            filterRow = false;
                            break;
                        }
                    }
                }
                if (filterRow)
                {
                    row.Hidden = true;
                }
                else
                {
                    row.Hidden = false;
                }

            }
        }

        private void cboTipoDocumento_KeyUp(object sender, KeyEventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboTipoDocumento.Rows)
            {
                if (cboTipoDocumento.Value == "-1") cboTipoDocumento.Text = string.Empty;
                bool filterRow = true;
                foreach (UltraGridColumn column in cboTipoDocumento.DisplayLayout.Bands[0].Columns)
                {
                    if (column.IsVisibleInLayout)
                    {
                        if (row.Cells[column].Text.Contains(cboTipoDocumento.Text.ToUpper()))
                        {
                            filterRow = false;
                            break;
                        }
                    }
                }
                if (filterRow)
                {
                    row.Hidden = true;
                }
                else
                {
                    row.Hidden = false;
                }

            }
        }

        private void cboTipoDocumento_Leave(object sender, EventArgs e)
        {
          
            if (cboTipoDocumento.Text.Trim() == "")
            {
                cboTipoDocumento.Value = "-1";
            }
            else
            {
                var x = _ListadoComboDocumentos.Find(p => p.Id == cboTipoDocumento.Value.ToString() || p.Id == cboTipoDocumento.Text);
                if (x == null)
                {
                    cboTipoDocumento.Value = "-1";
                }
            }
          
        }
    }
}
