using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Almacen.BL;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL; 
using SAMBHS.Venta.BL ;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using SAMBHS.Compra.BL;
using SAMBHS.Common.BL;
using System.Threading.Tasks;
using SAMBHS.Common.BE;
using System.Threading;    

namespace SAMBHS.Windows.WinClient.UI.Reportes.Compras
{
    public partial class frmImportacionesProductoAnalitico : Form
    {
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL();
        ClienteBL _objClienteBL = new ClienteBL();
        List<KeyValueDTO> _ListadoGrupos = new List<KeyValueDTO>();
        List<KeyValueDTO> _ListadoGruposOrdenar = new List<KeyValueDTO>();
        CancellationTokenSource _cts = new CancellationTokenSource();
        public frmImportacionesProductoAnalitico(string pstrParametro)
        {
            InitializeComponent();
        }

        private void frmImportacionesProductoAnalitico_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            ValidarFechas();
            CargarCombos();

        }
        private void CargarCombos()
        {
            OperationResult objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList (cboMoneda, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", _objNodeWarehouseBL.ObtenerAlmacenesParaCombo(ref objOperationResult, null, Globals.ClientSession.i_IdEstablecimiento.Value), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboOrdenar, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 65, null), DropDownListAction.Select);
            cboMoneda.Value = Globals.ClientSession.i_IdMoneda.ToString();
            cboAlmacen.Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
            cboOrdenar.Value = "1";
            cboAgrupar.Value = "2";
        }
        private void ValidarFechas()
        {
            string Periodo = Globals.ClientSession.i_Periodo.ToString();
            string Mes = DateTime.Today.Month.ToString();
            if (DateTime.Now.Year.ToString().Trim() == Periodo)
            {
                dtpFechaInicio.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/" + "01").ToString());
                dtpFechaInicio.MaxDate = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                dtpFechaInicio.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                dtpFechaFin.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/" + "01").ToString());
                dtpFechaFin.MaxDate = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());
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
            }
        }
        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Reporte de Importaciones por Producto - Analítico"
                : @"Reporte de Importaciones por Producto - Analítico";
            pBuscando.Visible = estado;
            
            btnVisualizar.Enabled = !estado;
        }
        private void btnVisualizar_Click(object sender, EventArgs e)
        {
            try
            {
                if (uvReporte.Validate(true, false).IsValid)
                {

                    if (cboAgrupar.Value == "-1")
                    { 
                   // UltraMessageBox.Show ("Llene ")
                    
                    }
                    OperationResult objOperationResult = new OperationResult();
                    string strOrderExpression = string.Empty;
                    List<string> Filters = new List<string>();
                    string _strFilterExpression = null, Seccion = string.Empty, strNombreGrupollave = string.Empty, strGrupollave = string.Empty;
                    List<string> Retonar = new List<string>();
                  
                    _ListadoGruposOrdenar = _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 65, null);
                   // if (cboMoneda.Value.ToString() != "-1") Filters.Add("i_IdMoneda==" + cboMoneda.Value.ToString());
                    if (cboAlmacen.Value.ToString() != "-1") Filters.Add("i_IdAlmacen==" + cboAlmacen.Value.ToString());
                    if (txtCodigoProducto.Text != string.Empty) Filters.Add("CodProducto==\"" + txtCodigoProducto.Text.Trim() + "\"");
                    if (txtProveedor.Text != string.Empty) Filters.Add("CodProveedor==\"" + txtProveedor.Text.Trim() + "\"");
                    if (txtPedido.Text != string.Empty) Filters.Add("NroPedido==\"" + txtPedido.Text.Trim() + "\"");
                    if (Filters.Count > 0)
                    {
                        foreach (string item in Filters)
                        {
                            _strFilterExpression = _strFilterExpression + item + " && ";
                        }
                        _strFilterExpression = _strFilterExpression.Substring(0, _strFilterExpression.Length - 4);
                    }
                    using (new LoadingClass.PleaseWait(this.Location, "Generando Reporte..."))
                        CargarReporte(dtpFechaInicio.Value.Date, dtpFechaFin.Value, _strFilterExpression, " por " + cboAgrupar.Text.ToString());
                }
            }
            catch
            {
                UltraMessageBox.Show("Se produjo un error  al mostrar el reporte ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }
        }


        private void CargarReporte(DateTime FechaInicio, DateTime FechaFin, string Filtro, string TipoAgrupamiento)
        {
            string Seccion = string.Empty, strNombreGrupollave = string.Empty, strGrupollave = string.Empty, strOrderExpression = string.Empty;
            List<string> Retonar = new List<string>();
            List<ReporteImportacionesProductoAnalitico> aptitudeCertificate = new List<ReporteImportacionesProductoAnalitico> ();
            var rp = new Reportes.Compras.crReporteImportacionesProductoAnalitico();
            OperationResult objOperationResult= new OperationResult ();
            for (int i = 0; i <= _ListadoGruposOrdenar.Count - 1; i++)
            {
                if (cboOrdenar.Text.Trim() == _ListadoGruposOrdenar[i].Value1.ToString().Trim())
                {
                    strOrderExpression = _ListadoGruposOrdenar[i].Value3.ToString();

                }

            }
            strNombreGrupollave = cboAgrupar.Text.Trim();
            strGrupollave = cboAgrupar.Text.Trim();  


            OcultarMostrarBuscar(true);
            Cursor.Current = Cursors.WaitCursor;

            Task.Factory.StartNew(() =>
            {
             aptitudeCertificate = new ImportacionBL().ReporteImportacionesProductoAnalitico(ref objOperationResult , FechaInicio, DateTime.Parse(FechaFin.Day.ToString() + "/" + dtpFechaFin.Value.Month.ToString() + "/" + dtpFechaFin.Value.Year.ToString() + " 23:59"), strOrderExpression, Filtro, strGrupollave, strNombreGrupollave, int.Parse ( cboMoneda.Value.ToString ()));
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
            ds1.Tables[0].TableName = "dsReporteImportacionesProductoAnalitico";
            ds1.Tables[1].TableName = "dsEmpresa";

            rp.SetDataSource(ds1);

            rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count());
            rp.SetParameterValue("DecimalesCantidad", (int)Globals.ClientSession.i_CantidadDecimales);
            rp.SetParameterValue("Fecha", "DEL  " + FechaInicio.Date.Day.ToString("00") + "/" + FechaInicio.Date.Month.ToString("00") + "/" + FechaInicio.Date.Year + " AL " + FechaFin.Date.Day.ToString("00") + "/" + FechaFin.Date.Month.ToString("00") + "/" + FechaFin.Date.Year);
            rp.SetParameterValue("Moneda", cboMoneda.Text.Trim () );
            rp.SetParameterValue("TipoAgrupamiento", cboAgrupar.Text.Trim ());
            rp.SetParameterValue("NombreEmpresa", aptitudeCertificate2.FirstOrDefault ().NombreEmpresaPropietaria.Trim () );
            rp.SetParameterValue("RucEmpresa", aptitudeCertificate2.FirstOrDefault ().RucEmpresaPropietaria.Trim ());
            rp.SetParameterValue("Proveedor",   (txtProveedor.Text.Trim ()+" "+ txtRazonSocial.Text.Trim ()).Trim ());
            crystalReportViewer1.ReportSource = rp;
            crystalReportViewer1.Show();
            crystalReportViewer1.Zoom(110);

            }
                , TaskScheduler.FromCurrentSynchronizationContext());



        }

        private void txtCodigoProducto_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscarProducto")
            {
                OperationResult objOperationResult = new OperationResult();
                Mantenimientos.frmBuscarProducto frm = new Mantenimientos.frmBuscarProducto(-1, "REPORTE", null, null);
                frm.ShowDialog();

                if (frm._IdProducto != null)
                {

                    txtCodigoProducto.Text = frm._CodigoInternoProducto.Trim();

                }
                else
                {
                    txtCodigoProducto.Text = string.Empty;
                }
            }
        }

        private void frmImportacionesProductoAnalitico_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }
    }
}
