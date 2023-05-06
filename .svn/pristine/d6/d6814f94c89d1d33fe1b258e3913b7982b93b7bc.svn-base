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
using SAMBHS.Almacen.BL;
using SAMBHS.Common.BL;
using SAMBHS.Compra.BL;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using SAMBHS.Venta.BL;
using SAMBHS.Common.BE;
using System.Threading.Tasks;
using System.Threading;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Compras
{
    public partial class frmImportacionesProveedorAnalitico : Form
    {
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL();
        ImportacionBL _objImportacionesBL = new ImportacionBL();
        List<KeyValueDTO> _ListadoGrupos = new List<KeyValueDTO>();
        List<KeyValueDTO> _ListadoGruposOrdenar = new List<KeyValueDTO>();
        ClienteBL _objClienteBL = new ClienteBL();
        CancellationTokenSource _cts = new CancellationTokenSource();
        public frmImportacionesProveedorAnalitico(string parametro)
        {
            InitializeComponent();
        }

        private void frmImportacionesProveedorAnalitico_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            CargarCombos();
            dtpFechaInicio.Value = DateTime.Parse("01/" + DateTime.Today.Month.ToString() + "/" + DateTime.Today.Year.ToString());
            dtpFechaInicio.MaxDate = dtpFechaFin.Value;
        }

        private void CargarCombos()
        {

            OperationResult objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", _objNodeWarehouseBL.ObtenerAlmacenesParaCombo(ref objOperationResult, null, Globals.ClientSession.i_IdEstablecimiento.Value), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboOrdenar, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 65, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAgrupar, "Value1", "Value2", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 66, null), DropDownListAction.Select);
            cboMoneda.Value = Globals.ClientSession.i_IdMoneda.Value.ToString();
            cboAlmacen.Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
            cboOrdenar.Value = "1";
            cboAgrupar.Text = "SIN AGRUPAR";
        }
        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Reporte de Importaciones por Proveedor-Analítico"
                : @"Reporte de Importaciones por Proveedor-Analítico";
            pBuscando.Visible = estado;
            btnVuisualizar.Enabled = !estado;
        }
        private void CargarReporte(DateTime FechaInicio, DateTime FechaFin, string Filtro, string TipoAgrupamiento)
        {
            string Seccion = string.Empty, strNombreGrupollave = string.Empty, strGrupollave = string.Empty, strOrderExpression = string.Empty;
            List<ReporteImportacionesProveeedorAnalitico> aptitudeCertificate = new List<ReporteImportacionesProveeedorAnalitico>();
            List<string> Retonar = new List<string>();
           OperationResult objOperationResult= new OperationResult ();
            var rp = new Reportes.Compras.crRegistroImportacionesProveedorAnalitico();

            for (int i = 0; i <= _ListadoGruposOrdenar.Count - 1; i++)
            {
                if (cboOrdenar.Text.Trim() == _ListadoGruposOrdenar[i].Value1.ToString().Trim())
                {
                    strOrderExpression = _ListadoGruposOrdenar[i].Value3.ToString();

                }

            }

            for (int i = 0; i <= _ListadoGrupos.Count - 1; i++) // recorremos para saber que tipo de  grupo es del reporte
            {
                if (cboAgrupar.Value != null && _ListadoGrupos[i].Value2 != null && _ListadoGrupos[i].Value3 != null)

               {
                   if (cboAgrupar.Value.ToString().Trim() == _ListadoGrupos[i].Value2.ToString().Trim() && _ListadoGrupos[i].Value3.ToString().Trim() != "")
                {

                    if (cboAgrupar.Text.Trim() == _ListadoGrupos[i].Value1.ToString())
                    {


                        strNombreGrupollave = _ListadoGrupos[i].Value1.ToString();
                        strGrupollave = _ListadoGrupos[i].Value3.ToString();
                    }
                }

                if (_ListadoGrupos[i].Value2.Contains(","))
                {
                    string[] split = _ListadoGrupos[i].Value2.Split(new Char[] { ',' });
                    foreach (string s in split)
                    {
                        if (s.Trim() != "")
                        {
                            Retonar.Add(s);
                        }

                    }
                }
                }
            }


            Retonar = Retonar.Distinct().ToList();
            for (int i = 0; i <= Retonar.Count() - 1; i++)
            {

                //Retonar2.Add(Retonar[0]);
                Seccion = Retonar[i].ToLower();
                Seccion = Seccion.Replace("g", "G");
                Seccion = Seccion.Replace("s", "S");
                Seccion = Seccion.Replace("h", "H");
                Seccion = Seccion.Replace("f", "F");

            }

            if (cboAgrupar.Value != null)
            {
                string[] split_ = cboAgrupar.Value.ToString().Split(new Char[] { ',' });
            foreach (string s in split_)
            {
                if (s.Trim() != "")
                    Seccion = s.ToLower();
                Seccion = Seccion.Replace("g", "G");
                Seccion = Seccion.Replace("s", "S");
                Seccion = Seccion.Replace("h", "H");
                Seccion = Seccion.Replace("f", "F");


                if (cboAgrupar.Text.ToString() == "SIN AGRUPAR") // para oculpar las secciones que no utilizaremos
                {

                    rp.ReportDefinition.Sections[Seccion].SectionFormat.EnableSuppress = true;
                }
                else
                {
                    rp.ReportDefinition.Sections[Seccion].SectionFormat.EnableSuppress = false;
                }
            }

            }

            OcultarMostrarBuscar(true);
            Cursor.Current = Cursors.WaitCursor;

            Task.Factory.StartNew(() =>
            {

                aptitudeCertificate = new ImportacionBL().ReporteImportacionesProveedorAnalitico(ref objOperationResult, FechaInicio, FechaFin, strOrderExpression, Filtro, strGrupollave, strNombreGrupollave, int.Parse(cboMoneda.Value.ToString()));
            } , _cts.Token)
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
                ds1.Tables[0].TableName = "dsImportacionProveedorAnalitico";
                ds1.Tables[1].TableName = "dsEmpresa";

                rp.SetDataSource(ds1);
                rp.SetParameterValue("FechaInicio", "DEL  " + FechaInicio.Date.Day.ToString("00") + "/" + FechaInicio.Date.Month.ToString("00") + "/" + FechaInicio.Date.Year + " AL " + FechaFin.Date.Day.ToString("00") + "/" + FechaFin.Date.Month.ToString("00") + "/" + FechaFin.Date.Year);
                rp.SetParameterValue("CantidadDecimalC", (int)Globals.ClientSession.i_CantidadDecimales);
                rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count());
                rp.SetParameterValue("TipoAgrupamiento", TipoAgrupamiento);
                rp.SetParameterValue("Moneda", cboMoneda.Text.Trim());
                rp.SetParameterValue("Almacen", cboAlmacen.Text.Trim());

                crystalReportViewer1.ReportSource = rp;
                crystalReportViewer1.Show();
                crystalReportViewer1.Zoom(110);
            }
                , TaskScheduler.FromCurrentSynchronizationContext());
        }
        #region ComportamientoControles
        private void btnVuisualizar_Click(object sender, EventArgs e)
        {

            try
            {
                if (uvReporteIA.Validate(true, false).IsValid)
                {
                    if (cboAgrupar.Value == null || cboAgrupar.Value.ToString() == "-1")
                    {
                        UltraMessageBox.Show("Por favor llene los campos requeridos para visualizar el reporte", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        cboAgrupar.Focus();
                        return;
                    }
                    
                    OperationResult objOperationResult = new OperationResult();
                    string strOrderExpression = string.Empty;
                    List<string> Filters = new List<string>();
                    string _strFilterExpression = null, Seccion = string.Empty, strNombreGrupollave = string.Empty, strGrupollave = string.Empty;
                    List<string> Retonar = new List<string>();
                  
                    _ListadoGrupos = _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 66, null);
                    _ListadoGruposOrdenar = _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 65, null);
                    if (cboAlmacen.Value.ToString() != "-1") Filters.Add("i_IdAlmacen==" + cboAlmacen.Value.ToString());
                    if (txtCodigoProducto.Text != string.Empty) Filters.Add("CodProducto==\"" + txtCodigoProducto.Text.Trim() + "\"");
                    if (txtProveedor.Text != string.Empty) Filters.Add("CodProveedor==\"" + txtProveedor.Text.Trim() + "\"");
                    if (txtNroPedido.Text != string.Empty) Filters.Add("NroPedido==\"" + txtNroPedido.Text.Trim() + "\"");
                    if (Filters.Count > 0)
                    {
                        foreach (string item in Filters)
                        {
                            _strFilterExpression = _strFilterExpression + item + " && ";
                        }
                        _strFilterExpression = _strFilterExpression.Substring(0, _strFilterExpression.Length - 4);
                    }
                   
                    CargarReporte(dtpFechaInicio.Value.Date,DateTime.Parse ( dtpFechaFin.Text +" 23:59"), _strFilterExpression, " por " + cboAgrupar.Text.ToString());
                }
            }
            catch
            {
                UltraMessageBox.Show("Se produjo un error  al mostrar el reporte ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }

        }

        private void dtpFechaRegistroAl_ValueChanged(object sender, EventArgs e)
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
               

            }
        }

        private void txtProveedor_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtProveedor, e);
        }

        private void txtProveedor_Validated(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (txtProveedor.Text.Trim() != string.Empty)
            {
                var Cliente = _objClienteBL.ObtenerClienteCodigoBandejasCodigo(ref objOperationResult, txtProveedor.Text.Trim(), "V");
            }
                
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
        
        #endregion

        private void frmImportacionesProveedorAnalitico_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }

       
    }
}
