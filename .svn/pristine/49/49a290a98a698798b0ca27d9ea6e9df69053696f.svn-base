using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Venta.BL;
using System.Threading;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    // ReSharper disable once InconsistentNaming
    public partial class frmRegistroVentaClienteAnalitico : Form
    {
        #region Fields
        private  int consideraDocumentosContables;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private string _strOrderExpression;
        public string  _Modalidad ;
       
        #endregion

        #region Init Load
        public frmRegistroVentaClienteAnalitico(string modalidad)
        {
            InitializeComponent();
            _Modalidad = modalidad;
            consideraDocumentosContables = modalidad == Constants.ModuloContable ? 0 : 1;
        }
        private void frmRegistroVentaClienteAnalitico_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
            CargarCombos();
            if (_Modalidad == Constants.ModuloContable)
            {
                consideraDocumentosContables = 1;
                cboTipoDocumento.Value = consideraDocumentosContables == 1 ? "1" : "-1";
            }

            else consideraDocumentosContables = int.Parse(cboTipoDocumento.Value.ToString());
            cboTipoDocumento.Visible = consideraDocumentosContables != 1;
            lblTipoDocumento.Visible = consideraDocumentosContables != 1;
        }

        private void CargarCombos()
        {
            var objOperationResult = new OperationResult();
            var objDatahierarchyBl = new DatahierarchyBL();
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboOrden, "Value1", "Value2", objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 68, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAgrupar, "Value1", "Value2", objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 83, null), DropDownListAction.Select);
            cboAgrupar.Value = "";
            cboMoneda.Value = Globals.ClientSession.i_IdMoneda.ToString();
            cboAgrupar.SelectedIndex = 2;
            cboOrden.SelectedIndex = 1;
            cboTipoDocumento.Value = "-1";
        
        }
        #endregion

        #region Methods
        private void Visualize(bool showExcel)
        {
            try
            {
                if (uvDatos.Validate(true, false).IsValid)
                {
                    if (cboAgrupar.Value != null && cboAgrupar.Value.ToString() == "-1")
                    {
                        UltraMessageBox.Show("Por favor llene los campos requeridos para visualizar el reporte", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        cboAgrupar.Focus();
                        return;
                    }
                    var valorIdCliente = TxtCliente.Text.Trim() == "" ? "" : TxtCliente.Text.Trim();
                    var moneda = int.Parse(cboMoneda.Value.ToString());
                    CargarReporte(DateTime.Parse(dtpFechaRegistroDe.Text), DateTime.Parse(dtpFechaRegistroAl.Text + " 23:59"), moneda, chkHoraimpresion.Checked ? "1" : "0", valorIdCliente, showExcel);
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
                ? @"Generando..."
                : @"Reporte de Ventas por Cliente Analítico";
            pBuscando.Visible = estado;
            BtnVuisualizar.Enabled = !estado;
            btnExcel.Enabled = !estado;
        }

        private void CargarReporte( DateTime fechaRegistroIni, DateTime fechaRegistroFin, int idMoneda, string fechaHoraImpresion, string idCliente, bool excel)
        {
            var retonar = new List<string>();
            // ReSharper disable once CollectionNeverQueried.Local
            var retonar2 = new List<string>();
            
            var rp = new crRegistroVentaClienteAnalitico();
            _strOrderExpression = "";
            var listadoGrupos = (List<KeyValueDTO>)cboAgrupar.DataSource;
            var seccion = cboAgrupar.Value == null ? "" : cboAgrupar.Value.ToString().ToLower();
            seccion = seccion.Replace("g", "G");
            seccion = seccion.Replace("s", "S");
            seccion = seccion.Replace("h", "H");

            var strGrupollave = "";
            var strNombreGrupollave = "";
            var nombreGrupollave = new List<string>();
            _strOrderExpression = cboAgrupar.Text == @"SIN AGRUPAR" ? "NombreCliente" : "NombreCliente,";
            for (var i = 0; i <= listadoGrupos.Count - 1; i++)
            {
                if (cboAgrupar.Value == null || listadoGrupos[i].Value2 == null || listadoGrupos[i].Value3 == null)
                    continue;
                if (cboAgrupar.Value.ToString().Trim() == listadoGrupos[i].Value2.Trim() && listadoGrupos[i].Value3.Trim() != "")
                    if (cboAgrupar.Text.Trim() == listadoGrupos[i].Value1)
                    {
                        strNombreGrupollave = listadoGrupos[i].Value1;
                        var splitNombreGrupollave = strNombreGrupollave.Split('/');
                        nombreGrupollave.AddRange(splitNombreGrupollave.Where(s => s.Trim() != ""));

                        strNombreGrupollave = nombreGrupollave[0];

                        _strOrderExpression += listadoGrupos[i].Value3;
                        strGrupollave = listadoGrupos[i].Value3;
                    }
                var split = listadoGrupos[i].Value2.Split(',');
                retonar.AddRange(split.Where(s => s.Trim() != ""));
            }
            retonar = retonar.Distinct().ToList();
            for (var i = 0; i <= retonar.Count - 1; i++)
            {

                retonar2.Add(retonar[0]);
                seccion = retonar[i].ToLower();
                seccion = seccion.Replace("g", "G");
                seccion = seccion.Replace("s", "S");
                seccion = seccion.Replace("h", "H");
                seccion = seccion.Replace("f", "F");
            }

            if (cboAgrupar.Value != null && cboAgrupar.Value.ToString().Trim() != "")
            {

                var split = cboAgrupar.Value.ToString().Split(',');
                foreach (var s in split)
                {
                    if (s.Trim() != "")
                        seccion = s.ToLower();
                    seccion = seccion.Replace("g", "G");
                    seccion = seccion.Replace("s", "S");
                    seccion = seccion.Replace("h", "H");
                    seccion = seccion.Replace("f", "F");
                    rp.ReportDefinition.Sections[seccion].SectionFormat.EnableSuppress = cboAgrupar.Text == @"SIN AGRUPAR";
                }
            }
            var splitGrupollave = strGrupollave.Split(',');
            var grupollave = splitGrupollave.Where(s => s.Trim() != "").ToList();
            if (grupollave.Count > 0)
            {
                strGrupollave = grupollave[0];
            }
            OcultarMostrarBuscar(true);
            Cursor.Current = Cursors.WaitCursor;
            var objOperationResult = new OperationResult();
            List<ReporteRegistroVentaClienteAnalitico> aptitudeCertificate = null;

            Task.Factory.StartNew(() => aptitudeCertificate = new VentaBL().ReporteRegistroVentaClienteAnalitico( ref objOperationResult, 0, fechaRegistroIni, fechaRegistroFin, idMoneda, idCliente, _strOrderExpression, strGrupollave, strNombreGrupollave, int.Parse (cboTipoDocumento.Value.ToString ())),_cts.Token)
            .ContinueWith(t =>
            {
                if (_cts.IsCancellationRequested) return;
                OcultarMostrarBuscar(false);
                Cursor.Current = Cursors.Default;
                if (objOperationResult.Success == 0)
                {
                    if (!string.IsNullOrEmpty(objOperationResult.ExceptionMessage))
                        UltraMessageBox.Show(objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var dt = Utils.ConvertToDatatable(aptitudeCertificate);
                if (excel)
                {
                    #region Headers
                    var columnas = new[]
                    {
                        "FechaRegistro", "TipoDocumento", "Documento", "NombreVendedor", "IdProducto",
                        "CantidadDetalle",int.Parse ( cboMoneda.Value.ToString ()) ==(int)Currency.Dolares ?  "PrecioDetalleD" :"PrecioDetalle",  int.Parse (cboMoneda.Value.ToString ()) ==(int)Currency.Dolares ? "ValorDetalleD":"ValorDetalle", int.Parse (cboMoneda.Value.ToString ())==(int)Currency.Dolares ? "DescuentoDetalleD":"DescuentoDetalle",
                        int.Parse (cboMoneda.Value.ToString ()) ==(int)Currency.Dolares ? "ValorVentaDetalleD":"ValorVentaDetalle" , int.Parse (cboMoneda.Value.ToString ()) ==(int)Currency.Dolares ? "PrecioVentaDetalleD":"PrecioVentaDetalle", "NombreMoneda", "CondicionVenta","DocumentoContable",
                    };
                    var heads = new ExcelHeader[]
                    {
                        "FECHA", "DOC.", "NUMERO" , "VENDEDOR", "DESCRIPCION", "CANTIDAD", "P. UNITARIO", "VALOR", "DESC", "V. VENTA", "P. VENTA", "MONEDA", "COND. PAGO","TIPO DOCUMENTO"
                    };
                    #endregion

                    var objexcel = new ExcelReport(dt) {Headers = heads};
                    objexcel.AutoSizeColumns(1, 15, 8, 15, 20, 50, 15, 20, 12, 12, 16, 16, 16, 20,12);
                    objexcel.SetTitle("VENTA-CLIENTE ANALITICO");
                    objexcel.SetHeaders();
                    objexcel.EndSection += (_, e) =>
                    {
                        var obj = (ExcelReport) _;
                        obj.SetFormulas(5, "SUB TOTAL:", Enumerable.Range(0, 6).Select(i => string.Format("=SUM(${2}{0}:${2}{1})", e.StartPosition + 1, e.EndPosition, (char)('G' + i))).ToArray());
                        obj.CurrentPosition++;
                    };
                    objexcel.EndSectionGroup += (_, e) =>
                    {
                        if (e.EndSections.Length == 0) return;
                        var obj = (ExcelReport)_;
                        obj.SetFormulas(5, "TOTAL:", Enumerable.Range(0, 6).Select(i => string.Format("=SUM(" + string.Join(",", e.EndSections.Select(n => "${0}" + n)) + ")", (char)('G' + i))).ToArray());
                        obj.CurrentPosition++;
                    };
                    var filtros = cboAgrupar.Text == @"SIN AGRUPAR" ? new []{"NombreCliente"} : new[] { "NombreCliente" , "Grupollave"};
                    objexcel.SetData(ref objOperationResult ,columnas, filtros);
                    var path = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + @".xlsx");
                    objexcel.Generate(path);
                    System.Diagnostics.Process.Start(path);
                }
                else
                {
                    var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();
                    var ds1 = new DataSet();
                    var dt2 = Utils.ConvertToDatatable(aptitudeCertificate2);

                    dt.TableName = "dsRegistroVentaClienteAnalitico";
                    dt2.TableName = "dsEmpresa";
                    ds1.Tables.Add(dt2);
                    ds1.Tables.Add(dt);

                    rp.SetDataSource(ds1);
                    rp.SetParameterValue("FechaHoraImpresion", fechaHoraImpresion);
                    rp.SetParameterValue("IdMoneda", idMoneda);
                    rp.SetParameterValue("Fecha",
                        "DEL " + fechaRegistroIni.Date.Day.ToString("00") + "/" +
                        fechaRegistroIni.Date.Month.ToString("00") + "/" + fechaRegistroIni.Date.Year.ToString() +
                        " AL " + fechaRegistroFin.Date.Day.ToString("00") + "/" +
                        fechaRegistroFin.Date.Month.ToString("00") + "/" + fechaRegistroFin.Date.Year.ToString());
                    rp.SetParameterValue("CantidadDecimal", Globals.ClientSession.i_CantidadDecimales);
                    rp.SetParameterValue("CantidadDeciamlPrecio", Globals.ClientSession.i_PrecioDecimales);
                    rp.SetParameterValue("NumeroRegistros", aptitudeCertificate.Count);
                    var orDefault = aptitudeCertificate2.FirstOrDefault();
                    if (orDefault != null)
                    {
                        rp.SetParameterValue("NombreEmpresaPropietaria", orDefault.NombreEmpresaPropietaria);
                        rp.SetParameterValue("RucEmpresaPropietaria", "R.U.C. : "+ orDefault.RucEmpresaPropietaria);
                    }
                    crystalReportViewer1.ReportSource = rp;
                    crystalReportViewer1.Show();
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
            
        }
        #endregion

        #region Event UI
        private void BtnVuisualizar_Click(object sender, EventArgs e)
        {    
            Visualize(false);
        }
        private void btnExcel_Click(object sender, EventArgs e)
        {
            Visualize(true);
        }
        
        private void frmRegistroVentaClienteAnalitico_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }
       
        #endregion

        private void TxtCliente_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var frm = new Mantenimientos.frmBuscarCliente("V", TxtCliente.Text.Trim());
            frm.ShowDialog();

            if (frm._IdCliente == null) return;
            TxtCliente.Text = frm._CodigoCliente;
           
        }

    }
}
