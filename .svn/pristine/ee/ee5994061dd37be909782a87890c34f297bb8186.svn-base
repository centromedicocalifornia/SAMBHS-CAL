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
    public partial class frmRegistroVentaAnaliticoDAOT : Form
    {
        #region Declaraciones / Referencias
        VendedorBL _objVendedorBL = new VendedorBL();
        ComprasBL _objComprasBL = new ComprasBL();
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        EstablecimientoBL objEstablecimientoBL = new EstablecimientoBL();
        NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL();
        private readonly int _ConsideraDocumentosInternos = -1;
        LineaBL _objLineaBL = new LineaBL();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
     
       
        List<string> Grupollave = new List<string>();
        List<string> NombreGrupollave = new List<string>();
        #endregion
        #region Carga de inicializacion
        public frmRegistroVentaAnaliticoDAOT(string Modalidad)
        {
            _ConsideraDocumentosInternos = Modalidad == Constants.ModuloContable ? 0 : 1;
            InitializeComponent();
        }
        #endregion
        #region Cargar Reporte
        private void CargarReporte(DateTime FechaRegistroIni, DateTime FechaRegistroFin, int IdMoneda, string FechaHoraImpresion, string IdCliente, string Orden, int IdTipoVenta)
        {

            var rp = new Reportes.Ventas.crRegistroVentaAnaliticoDAOT();
            OperationResult objOperationResult = new OperationResult();



            List<ReporteRegistroVentaAnaliticoDAOT> aptitudeCertificate = new List<ReporteRegistroVentaAnaliticoDAOT>();

            OcultarMostrarBuscar(true);

            Cursor.Current = Cursors.WaitCursor;
            Task.Factory.StartNew(() =>
            {

                aptitudeCertificate = new VentaBL().ReporteRegistroVentaAnaliticoDAOT(ref  objOperationResult, 0, FechaRegistroIni, FechaRegistroFin, IdCliente, IdTipoVenta, string.IsNullOrEmpty(TxtImporteTope.Text.Trim()) ? 0 : decimal.Parse(TxtImporteTope.Text), cboOrdenar.Value.ToString());
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
                        UltraMessageBox.Show("Ocurrió un Error al generar Registro Venta Analítico DAOT", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show("Ocurrió un Error al generar Registro Venta Analítico DAOT", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                ds1.Tables[1].TableName = "dsRegistroVentaAnaliticoDAOT";
                rp.SetDataSource(ds1);
                rp.SetParameterValue("FechaHoraImpresion", FechaHoraImpresion);
                rp.SetParameterValue("IdMoneda", IdMoneda);
                rp.SetParameterValue("FechaRegistroIni", " DEL  " + dtpFechaRegistroDe.Text + " AL " + dtpFechaRegistroAl.Text);
                rp.SetParameterValue("FechaRegistroFin", FechaRegistroFin);
                rp.SetParameterValue("CantidadDecimal", (int)Globals.ClientSession.i_CantidadDecimales);
                rp.SetParameterValue("ImpTope", decimal.Parse(TxtImporteTope.Text.ToString()));

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
                    var rp = new Reportes.Ventas.crRegistroVentaClienteAnalitico();
                    List<string> Filters = new List<string>();
                    ValorIdCliente = TxtCodigo.Text.Trim();
                    Moneda = 1;
                    strOrderExpression = "";

                    CargarReporte(DateTime.Parse(dtpFechaRegistroDe.Text), DateTime.Parse(dtpFechaRegistroAl.Text + " 23:59"), Moneda, chkHoraimpresion.Checked == true ? "1" : "0", ValorIdCliente, strOrderExpression, int.Parse(cboTipoVenta.Value.ToString()));


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
                ? @"Generando... " + "Registro Venta Analítico DAOT"
                : @"Registro Venta Analítico DAOT";
            pBuscando.Visible = estado;
            BtnVuisualizar.Enabled = !estado;
        }


        private void btnBuscarCliente_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            Mantenimientos.frmBuscarCliente frm = new Mantenimientos.frmBuscarCliente("V", TxtCodigo.Text.Trim());
            frm.ShowDialog();
            if (frm._IdCliente != null)
            {
                TxtCodigo.Text = frm._CodigoCliente.Trim();
            }
            else
            {
                TxtCodigo.Text = "";
            }
        }
        #endregion

        private void frmRegistroVentaAnaliticoDAOT_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            OperationResult objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList(cboTipoVenta, "Value1", "Id", _objComprasBL.ObtenerConceptosParaCombo(ref objOperationResult, 2, null), DropDownListAction.Select);
            cboTipoVenta.Value = "-1";
            cboOrdenar.SelectedIndex = 0;
        }

        private void frmRegistroVentaAnaliticoDAOT_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }

        private void TxtImporteTope_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(TxtImporteTope, e);
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
