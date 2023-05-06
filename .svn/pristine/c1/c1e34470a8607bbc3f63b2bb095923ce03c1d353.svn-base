#region Name Space
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Venta.BL;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Almacen.BL;
using System.Text.RegularExpressions;
using Infragistics.Win.UltraWinMaskedEdit;
using CrystalDecisions.CrystalReports.Engine;
using System.Data.Sql;
using System.Linq.Dynamic;
using System.Data.SqlClient;
using System.Configuration;
using SAMBHS.Security.BL;
using CrystalDecisions.Shared;
using System.Reflection;
using System.Threading;
#endregion

namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    public partial class frmRegistroVentaClienteResumen : Form
    {
        #region Declaraciones / Referencias
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        public int ConsideraDocumentosContables = -1;
         CancellationTokenSource _cts = new CancellationTokenSource();
         public string _Modalidad = "";
        #endregion
        #region Carga de inicializacion
        public frmRegistroVentaClienteResumen(string Modalidad)
        {
            InitializeComponent();
            _Modalidad = Modalidad;
            ConsideraDocumentosContables = Modalidad == Constants.ModuloContable ? 0 : 1;
          
        }
        #endregion
        #region Cargar Reporte
         private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Reporte de Ventas Por Cliente Resumen"
                : @"Reporte de Ventas Por Cliente Resumen";
            pBuscando.Visible = estado;

            BtnVuisualizar.Enabled = !estado;
        }
        private void CargarReporte( DateTime FechaRegistroIni, DateTime FechaRegistroFin, int IdMoneda, string FechaHoraImpresion, string IdCliente, string Orden)
        {
            var rp = new Reportes.Ventas.crRegistroVentaClienteResumen();
            List<ReporteRegistroVentaClienteResumen> aptitudeCertificate= new List<ReporteRegistroVentaClienteResumen> ();
            OperationResult objOperationResult= new OperationResult ();
            OcultarMostrarBuscar(true);
            Cursor.Current = Cursors.WaitCursor;

            Task.Factory.StartNew(() =>
            {
             aptitudeCertificate = new VentaBL().ReporteRegistroVentaClienteResumen(ref objOperationResult , 0, FechaRegistroIni, FechaRegistroFin, IdMoneda, IdCliente, Orden,int.Parse ( cboTipoDocumento.Value.ToString ()));
             },_cts.Token)
            .ContinueWith(t =>
            {
                if (_cts.IsCancellationRequested) return;
                OcultarMostrarBuscar(false);
                Cursor.Current = Cursors.Default;
                if (objOperationResult.Success == 0)
                {
                    if (!string.IsNullOrEmpty(objOperationResult.ExceptionMessage))
                    {
                        UltraMessageBox.Show(objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }
                
                var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();

            DataSet ds1 = new DataSet();

            DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
            DataTable dt2 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate2);

            ds1.Tables.Add(dt2);
            ds1.Tables.Add(dt);

            ds1.Tables[0].TableName = "dsEmpresa";
            ds1.Tables[1].TableName = "dsRegistroVentaClienteResumen";

            rp.SetDataSource(ds1);

            rp.SetParameterValue("FechaHoraImpresion", FechaHoraImpresion);
            rp.SetParameterValue("IdMoneda", IdMoneda);
            rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count());
            rp.SetParameterValue("Fecha", "DEL " + FechaRegistroIni.Date.Day.ToString("00") + "/" + FechaRegistroIni.Date.Month.ToString("00") + "/" + FechaRegistroIni.Date.Year.ToString() + " AL " + FechaRegistroFin.Date.Day.ToString("00") + "/" + FechaRegistroFin.Date.Month.ToString("00") + "/" + FechaRegistroFin.Date.Year.ToString());

            rp.SetParameterValue("RucEmpresa", aptitudeCertificate2.FirstOrDefault().RucEmpresaPropietaria.Trim());

            rp.SetParameterValue("NombreEmpresa", aptitudeCertificate2.FirstOrDefault().NombreEmpresaPropietaria.Trim ());

            crystalReportViewer1.ReportSource = rp;
            crystalReportViewer1.Show();
            }
                , TaskScheduler.FromCurrentSynchronizationContext());
        }

        #endregion
        #region Controles Botones
        private void BtnVuisualizar_Click(object sender, EventArgs e)
        {
            try
            {

                if (uvDatos.Validate(true, false).IsValid)
                {
                    int Moneda;
                    string ValorIdCliente, strOrderExpression;

                    var rp = new Reportes.Ventas.crRegistroVentaClienteResumen();
                    List<string> Filters = new List<string>();
                    ValorIdCliente = TxtCliente.Text.Trim() == "" ? "" : TxtCliente.Text.Trim();
                   
                    Moneda = int.Parse(cboMoneda.Value.ToString());
                    strOrderExpression = cboOrden.Value.ToString() == "-1" ? "" : cboOrden.Value.ToString();
                   
                        CargarReporte(DateTime.Parse(dtpFechaRegistroDe.Text), DateTime.Parse(dtpFechaRegistroAl.Text + " 23:59"), Moneda, chkHoraimpresion.Checked == true ? "1" : "0", ValorIdCliente, strOrderExpression);

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
        private void btnBuscarCliente_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            Mantenimientos.frmBuscarCliente frm = new Mantenimientos.frmBuscarCliente("VV", TxtCliente.Text.Trim());
            frm.ShowDialog();

            if (frm._IdCliente != null)
            {
                TxtCliente.Text = frm._CodigoCliente;
               
            }
        }
        #endregion

        private void frmRegistroVentaClienteResumen_Load(object sender, EventArgs e)
        {

            this.BackColor = new GlobalFormColors().FormColor;
            CargarCombos();
            if (_Modalidad == Constants.ModuloContable)
            {
                ConsideraDocumentosContables = 1;
                cboTipoDocumento.Value = ConsideraDocumentosContables == 1 ? "1" : "-1";
            }

            else ConsideraDocumentosContables = int.Parse(cboTipoDocumento.Value.ToString());
            cboTipoDocumento.Visible = ConsideraDocumentosContables != 1;
            lblTipoDocumento.Visible = ConsideraDocumentosContables != 1;
        }
        private void CargarCombos()
        {
            OperationResult objOperationResult = new OperationResult();

            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboOrden, "Value1", "Value2", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 69, null), DropDownListAction.Select);
            cboMoneda.Value = Globals.ClientSession.i_IdMoneda.ToString();
            cboOrden.SelectedIndex = 1;
            cboTipoDocumento.Value = "-1";
        
        }


        private void frmRegistroVentaClienteResumen_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }

        private void TxtCliente_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var frm = new Mantenimientos.frmBuscarCliente("V", TxtCliente.Text.Trim());
            frm.ShowDialog();

            if (frm._IdCliente == null) return;
            TxtCliente.Text = frm._CodigoCliente;
        }
    }
}
