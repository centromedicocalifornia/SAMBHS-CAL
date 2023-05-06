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
using SAMBHS.Almacen.BL;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Compra.BL;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using SAMBHS.Venta.BL;
using SAMBHS.Common.BE;
using System.Threading.Tasks;
using System.Threading;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Compras
{
    public partial class frmImportacionesProveedorResumen : Form
    {
        NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        ImportacionBL _objImportacionBL = new ImportacionBL();
        ClienteBL _objClienteBL = new ClienteBL();
        CancellationTokenSource _cts = new CancellationTokenSource();
        string v_IdProveedor = string.Empty;
        public frmImportacionesProveedorResumen(string parametro)
        {
            InitializeComponent();
        }
        private void frmImportacionesProveedorResumen_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            OperationResult objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", _objNodeWarehouseBL.ObtenerAlmacenesParaCombo(ref objOperationResult, null, Globals.ClientSession.i_IdEstablecimiento.Value), DropDownListAction.Select);
            cboAlmacen.Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
            dtpFechaInicio.Value = DateTime.Parse("01/" + DateTime.Today.Month.ToString() + "/" + DateTime.Today.Year.ToString());
            dtpFechaInicio.MaxDate = dtpFechaFin.Value;

        }

        #region Comportamiento Controles
        private void dtpFechaFin_ValueChanged(object sender, EventArgs e)
        {
            dtpFechaInicio.MaxDate = dtpFechaFin.Value;
        }

        //private void TxtCodigo_KeyDown(object sender, KeyEventArgs e)
        //{
        //    OperationResult objOperationResult = new OperationResult();
        //    if (e.KeyCode == Keys.Enter)
        //    {
        //        if (TxtCodigos.Text.Trim() != "" & TxtCodigos.TextLength <= 7)
        //        {

        //            Mantenimientos.frmBuscarProveedor frm = new Mantenimientos.frmBuscarProveedor(TxtCodigos.Text.Trim(), "RUC");
        //            frm.ShowDialog();

        //            if (frm._IdProveedor != null)
        //            {

        //                v_IdProveedor = frm._IdProveedor.Trim();
        //                TxtCodigos.Text = frm._NroDocumento.Trim();
        //                txtRazonSocial.Text = frm._RazonSocial.Trim();

        //            }
        //            else
        //            {
        //                txtRazonSocial.Clear();

        //            }
        //        }

        //        else
        //        {

        //            if (TxtCodigos.TextLength == 8 | TxtCodigos.TextLength == 11)
        //            {
        //                string[] DatosCliente = new string[3];


        //                DatosCliente = _objImportacionBL.DevolverProveedorPorNroDocumento(ref objOperationResult, TxtCodigos.Text.Trim());
        //                if (DatosCliente != null)
        //                {
        //                    v_IdProveedor = DatosCliente[0].Trim();
        //                    txtRazonSocial.Text = DatosCliente[2].Trim();

        //                }
        //            }
        //            else
        //            {

        //                Mantenimientos.frmBuscarCliente frm = new Mantenimientos.frmBuscarCliente("VV", TxtCodigos.Text.Trim());
        //                frm.ShowDialog();

        //                if (frm._IdCliente != null)
        //                {

        //                    v_IdProveedor = frm._IdCliente.Trim();
        //                    TxtCodigos.Text = frm._NroDocumento.Trim();
        //                    txtRazonSocial.Text = frm._RazonSocial.Trim();

        //                }
        //                else
        //                {
        //                    txtRazonSocial.Clear();
        //                    v_IdProveedor = string.Empty;

        //                }

        //            }
        //        }
        //    }

        //    if (e.KeyCode == Keys.Back)
        //    {
        //        txtRazonSocial.Clear();
        //    }
        //}

        //private void TxtCodigo_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    Utils.Windows.NumeroEntero(TxtCodigos, e);
        //}
        //private void TxtCodigo_TextChanged(object sender, EventArgs e)
        //{
        //    if (TxtCodigos.Text == string.Empty)
        //    {
        //        v_IdProveedor = string.Empty;
        //        txtRazonSocial.Clear();
        //    }
        //}
        #endregion
        //private void btnBuscar_Click(object sender, EventArgs e)
        //{
        //    Mantenimientos.frmBuscarProveedor frm = new Mantenimientos.frmBuscarProveedor(TxtCodigos.Text.Trim(), "RUC");
        //    frm.ShowDialog();
        //    if (frm._IdProveedor != null)
        //    {

        //        TxtCodigos.Text = frm._CodigoProveedor.Trim();
        //        txtRazonSocial.Text = frm._RazonSocial.Trim();
        //        v_IdProveedor = frm._IdProveedor.Trim();
        //    }
        //}



        private void btnVuisualizar_Click(object sender, EventArgs e)
        {
            if (uvReporteIR.Validate(true, false).IsValid)
            {

                string _strFilterExpression = null, CodigoProveedor = string.Empty, NroPedido = string.Empty;
                int Almacen;
                List<string> Filters = new List<string>();
                //if (cboAlmacen.SelectedValue.ToString() == "-1")
                //{
                //    UltraMessageBox.Show("Debe seleccionar un almacén ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    cboAlmacen.Focus();
                //    return;

                //}
                CodigoProveedor = txtProveedor.Text == string.Empty ? string.Empty : txtProveedor.Text.Trim();
                Almacen = int.Parse(cboAlmacen.Value.ToString());
                NroPedido = txtNroPedido.Text == string.Empty ? string.Empty : txtNroPedido.Text.Trim();
               CargarReporte(dtpFechaInicio.Value.Date, DateTime.Parse(dtpFechaFin.Text + " 23:59"), _strFilterExpression, CodigoProveedor, Almacen, NroPedido);

            }
        }
        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Reporte de Importaciones por Proveedor-Resumen"
                : @"Reporte de Importaciones por Proveedor-Resumen";
            pBuscando.Visible = estado;
            btnVuisualizar.Enabled = !estado;
        }

        private void CargarReporte(DateTime FechaInicio, DateTime FechaFin, string Filtro, string pstrCodigoProveedor, int Almacen, string NroPedido)
        {



            ReportDocument rp = new ReportDocument();
            OperationResult objOperationResult = new OperationResult();
            List<ReporteImportacionesProveeedorResumen> aptitudeCertificate = new List<ReporteImportacionesProveeedorResumen>();
            rp = new Reportes.Compras.crRegistroImportacionesProveedorResumen();

            OcultarMostrarBuscar(true);
            Cursor.Current = Cursors.WaitCursor;

            Task.Factory.StartNew(() =>
            {


                aptitudeCertificate = new ImportacionBL().ReporteImportacionesProveedorResumen(ref objOperationResult, FechaInicio, FechaFin, pstrCodigoProveedor, Almacen, NroPedido);
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
                        UltraMessageBox.Show(objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error " + " Reporte Kardex Físico / Valorizado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error" + " Reporte Kardex Físico / Valorizado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }
                var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();

                DataSet ds1 = new DataSet();
                DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
                DataTable dt2 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate2);


                ds1.Tables.Add(dt);
                ds1.Tables.Add(dt2);
                ds1.Tables[0].TableName = "dsImportacionProveedorResumen";
                ds1.Tables[1].TableName = "dsEmpresa";

                rp.SetDataSource(ds1);




                rp.SetParameterValue("Fecha", "DEL  " + FechaInicio.Date.Day.ToString("00") + "/" + FechaInicio.Date.Month.ToString("00") + "/" + FechaInicio.Date.Year + " AL " + FechaFin.Date.Day.ToString("00") + "/" + FechaFin.Date.Month.ToString("00") + "/" + FechaFin.Date.Year);
                rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count());
                rp.SetParameterValue("Almacen", cboAlmacen.Text.Trim());


                crystalReportViewer1.ReportSource = rp;
                crystalReportViewer1.Show();
                crystalReportViewer1.Zoom(110);
            }
                , TaskScheduler.FromCurrentSynchronizationContext());


        }

        private void txtProveedor_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            Mantenimientos.frmBuscarProveedor frm = new Mantenimientos.frmBuscarProveedor("REPORTE", "RUC");
            frm.ShowDialog();
            if (frm._IdProveedor != null)
            {

                txtProveedor.Text = frm._CodigoProveedor.Trim().ToUpper();
                txtRazonSocial.Text = frm._RazonSocial.Trim().ToUpper();

            }
        }

        private void txtProveedor_Validated(object sender, EventArgs e)
        {

            OperationResult objOperationResult = new OperationResult();
            if (txtProveedor.Text.Trim() != string.Empty)
            {
                var Cliente = _objClienteBL.ObtenerClienteCodigoBandejasCodigo(ref objOperationResult, txtProveedor.Text.Trim(), "V");
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
                // v_IdCliente = string.Empty;
            }

        }

        private void txtProveedor_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtProveedor, e);
        }

        private void frmImportacionesProveedorResumen_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }
    }
}
