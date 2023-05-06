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
using SAMBHS.Compra.BL;
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
    public partial class frmRegistroVentaResumenDAOT : Form
    {
        #region Declaraciones / Referencias
        VendedorBL _objVendedorBL = new VendedorBL();
        ComprasBL _objComprasBL = new ComprasBL();
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        EstablecimientoBL objEstablecimientoBL = new EstablecimientoBL();
        NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL();
        LineaBL _objLineaBL = new LineaBL();
        private readonly int _ConsideraDocumentosInternos = -1;
        List<string> Grupollave = new List<string>();
        List<string> NombreGrupollave = new List<string>();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        #endregion    
        #region Carga de inicializacion

        public frmRegistroVentaResumenDAOT(string Modalidad)
        {
           
            _ConsideraDocumentosInternos = Modalidad == Constants.ModuloContable ? 0 : 1;
            InitializeComponent();
        }
        #endregion

        private void frmRegistroVentaResumenDAOT_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;

            OperationResult objOperationResult = new OperationResult();

            //Utils.Windows.LoadDropDownList(cboOrden, "Value1", "Value2", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 68, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList (cboTipoVenta, "Value1", "Id", _objComprasBL.ObtenerConceptosParaCombo(ref objOperationResult, 2, null), DropDownListAction.Select);
            //cboOrden.SelectedIndex = 1;
            cboTipoVenta.Value = "-1";
            cboOrdenar.SelectedIndex = 0;
        }
        #region Cargar Reporte

         private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Registro Venta Resumen DAOT"
                : @"Registro Venta Resumen DAOT";
            pBuscando.Visible = estado;
            BtnVuisualizar.Enabled = !estado;
        }



        private void CargarReporte( DateTime FechaRegistroIni, DateTime FechaRegistroFin, int IdMoneda, string FechaHoraImpresion, string IdCliente, string Orden, int IdTipoVenta)
        {

            var rp = new Reportes.Ventas.crRegistroVentaResumenDAOT();
            OperationResult objOperationResult = new OperationResult();
            List<ReporteRegistroVentaResumenDAOT> aptitudeCertificate = new List<ReporteRegistroVentaResumenDAOT> ();
             OcultarMostrarBuscar(true);
        
            Cursor.Current = Cursors.WaitCursor;
            Task.Factory.StartNew(() =>
            {

                aptitudeCertificate = new VentaBL().ReporteRegistroVentaResumenDAOT(ref objOperationResult, FechaRegistroIni, FechaRegistroFin, IdMoneda, IdCliente, IdTipoVenta, cboOrdenar.Value.ToString(), string.IsNullOrEmpty(TxtImporteTope.Text) ? 0 : decimal.Parse(TxtImporteTope.Text));
           
                
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
                        UltraMessageBox.Show("Ocurrió un Error al generar Registro Venta Resumen DAOT", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show("Ocurrió un Error al generar Registro Venta Resumen DAOT", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            ds1.Tables[1].TableName = "dsRegistroVentaResumenDAOT";
            rp.SetDataSource(ds1);

            rp.SetParameterValue("FechaHoraImpresion", FechaHoraImpresion);
            rp.SetParameterValue("IdMoneda", IdMoneda);
            rp.SetParameterValue("FechaRegistroIni", "DEL  " + dtpFechaRegistroDe.Text + " AL " + dtpFechaRegistroAl.Text);
            rp.SetParameterValue("FechaRegistroFin", FechaRegistroFin);
            rp.SetParameterValue("CantidadDecimal", (int)Globals.ClientSession.i_CantidadDecimales);
            rp.SetParameterValue("CantidadDeciamlPrecio", (int)Globals.ClientSession.i_PrecioDecimales);
            rp.SetParameterValue("ImpTope", decimal.Parse(TxtImporteTope.Text.ToString()));
            
           


            crystalReportViewer1.ReportSource = rp;
            crystalReportViewer1.Show();

            }
                 , TaskScheduler.FromCurrentSynchronizationContext());
        }
        #endregion

        private void BtnVuisualizar_Click(object sender, EventArgs e)
        {
            try
            {

                if (uvDatos.Validate(true, false).IsValid)
                {
                    int Moneda;
                    string ValorIdCliente, strOrderExpression;
                    var rp = new Reportes.Ventas.crRegistroVentaResumenDAOT();
                    List<string> Filters = new List<string>();
                   
                    Moneda = 1;
                    strOrderExpression = "";
                    //using (new LoadingClass.PleaseWait(this.Location, "Generando..."))
                        CargarReporte( DateTime.Parse(dtpFechaRegistroDe.Text), DateTime.Parse(dtpFechaRegistroAl.Text + " 23:59"), Moneda, chkHoraimpresion.Checked == true ? "1" : "0",  TxtCodigo.Text.Trim (), strOrderExpression, int.Parse(cboTipoVenta.Value.ToString()));


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

        private void frmRegistroVentaResumenDAOT_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }

        private void TxtCodigo_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var frm = new Mantenimientos.frmBuscarCliente("VV", TxtCodigo.Text.Trim());
            frm.ShowDialog();

            if (frm._IdCliente == null) return;
            TxtCodigo.Text = frm._NroDocumento;
            
        }
    }
}
