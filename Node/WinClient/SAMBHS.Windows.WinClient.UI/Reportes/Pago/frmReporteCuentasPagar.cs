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
using SAMBHS.Compra.BL;
using SAMBHS.Common.BL;
using Infragistics.Win.UltraWinMaskedEdit;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Venta.BL;
using System.Threading.Tasks;
using System.Threading;
using SAMBHS.Common.BE;
using System.IO;
namespace SAMBHS.Windows.WinClient.UI.Reportes.Pago
{
    public partial class frmReporteCuentasPagar : Form
    {
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        List<GridKeyValueDTO> _ListadoComboDocumentos = new List<GridKeyValueDTO>();
        CancellationTokenSource _cts = new CancellationTokenSource();
        string _strFilterExpression = String.Empty;
        ClienteBL _objClienteBL = new ClienteBL();
        public frmReporteCuentasPagar(string pstrParametro)
        {
            InitializeComponent();
        }


        private void frmReporteCuentasPagar_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            ValidarFechas();
            CargarCombos();
            object senderFechas = new object();
            EventArgs eFechas = new EventArgs();
            uckConsiderarRangoFechas_CheckedChanged(senderFechas, eFechas);
        }
        private void CargarCombos()
        {
            OperationResult objOperationResult = new OperationResult();
            _ListadoComboDocumentos = _objDocumentoBL.ObtenDocumentosParaComboGrid(ref objOperationResult, null, 0, 1);
            Utils.Windows.LoadUltraComboList(cboTipoDocumento, "Value2", "Id", _ListadoComboDocumentos, DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboEstablecimiento, "Value1", "Id", new EstablecimientoBL().ObtenerEstablecimientosValueDto(ref objOperationResult, null), DropDownListAction.All);
            cboOrden.Text = "NRO. REGISTRO";
            cboAgrupar.Text = "SIN AGRUPAR";
            cboTipoDocumento.Value = "-1";
            cboEstablecimiento.Value = Globals.ClientSession.i_IdEstablecimiento.Value.ToString();

        }

        private void ValidarFechas()
        {
            string Periodo = Globals.ClientSession.i_Periodo.ToString();
            string Mes = DateTime.Today.Month.ToString();
            if (DateTime.Now.Year.ToString().Trim() == Periodo)
            {
                dtpFecha.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/" + "01").ToString());
                dtpFecha.MaxDate = DateTime.Parse((Periodo + "/" + Mes + "/" + (DateTime.DaysInMonth(int.Parse(Periodo), int.Parse(Mes))).ToString()).ToString());
                dtpFecha.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());

                dtpFechaAl.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/" + "01").ToString());
                dtpFechaAl.MaxDate = DateTime.Parse((Periodo + "/" + Mes + "/" + (DateTime.DaysInMonth(int.Parse(Periodo), int.Parse(Mes))).ToString()).ToString());
                dtpFechaAl.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());

            }
            else
            {
                dtpFecha.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/01").ToString());
                dtpFecha.MaxDate = DateTime.Parse((Periodo + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(Periodo), 12)).ToString()).ToString());
                dtpFecha.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                dtpFechaAl.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/01").ToString());
                dtpFechaAl.MaxDate = DateTime.Parse((Periodo + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(Periodo), 12)).ToString()).ToString());
                dtpFechaAl.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());

            }
        }

        private void ultraExpandableGroupBox1_ExpandedStateChanged(object sender, EventArgs e)
        {
            if (ultraExpandableGroupBox1.Expanded == true)
            {
                ultraGroupBox1.Location = new Point(ultraGroupBox1.Location.X, ultraExpandableGroupBox1.Location.Y + ultraExpandableGroupBox1.Height + 5);
                ultraGroupBox1.Height = this.Height - ultraGroupBox1.Location.Y - 7;
            }
            else
            {
                ultraGroupBox1.Location = new Point(ultraGroupBox1.Location.X, ultraExpandableGroupBox1.Location.Y + ultraExpandableGroupBox1.Height + 5);
                ultraGroupBox1.Height = this.Height - ultraGroupBox1.Location.Y - 7;
            }
        }

        private void VisualizarReporte(bool Export)
        {
            try
            {
                if (uvValidar.Validate(true, false).IsValid)
                {

                    string Orden = string.Empty;
                    if (cboOrden.Text == "DOCUMENTO")
                    {
                        Orden = "NroDocumento";
                    }
                    else if (cboOrden.Text == "FECHA VENC.")
                    {
                        Orden = "FechaVencimiento";
                    }
                    else if (cboOrden.Text == "NRO. REGISTRO")
                    {
                        Orden = "Correlativo";
                    }
                   
                    CargarReporte( Orden,Export);
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


        private void btnVisualizar_Click(object sender, EventArgs e)
        {
            VisualizarReporte(false);
        }



        private void CargarReporte(string Orden, bool Excel)
        {
            List<ReporteCuentasPagarDto> ReporteCuentasPagar = new List<ReporteCuentasPagarDto>();
            OperationResult objOperationResult = new OperationResult();
            var rp = new Reportes.Pago.crReporteCuentasPagar();

            Cursor.Current = Cursors.WaitCursor;

            Task.Factory.StartNew(() =>
            {

                ReporteCuentasPagar = new PagoBL().ReporteCuentasPagar(ref objOperationResult, dtpFecha.Value.Date, DateTime.Parse(dtpFechaAl.Value.Date.Day.ToString() + "/" + dtpFechaAl.Value.Date.Month.ToString() + "/" + dtpFechaAl.Value.Date.Year.ToString() + " 23:59"), cboAgrupar.Text.Trim(), Orden, txtProveedor.Text.Trim (),uckConsiderarRangoFechas.Checked,chkIncluirLetrasCambio.Checked, int.Parse ( cboEstablecimiento.Value.ToString ()));
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
                        UltraMessageBox.Show("Ocurrió un error al realizar Reporte cuentas por pagar.\n Información Adicional :" + objOperationResult.AdditionalInformation, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show("Ocurrió un error al realizar Reporte cuentas por pagar.\n Información Adicional :" + objOperationResult.AdditionalInformation, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }
                var Empresa = new NodeBL().ReporteEmpresa();
                DataSet ds1 = new DataSet();
                DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(ReporteCuentasPagar);


                if (Excel)
                {
                    //Result = Result.OrderBy(l => l.v_NombreProducto).ToList();
                    //dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(Result);
                    #region Headers

                    var columnas = new[]
                    {
                        "NroDocumento", "FechaEmision", "FechaVencimiento","Soles", "Dolares", "NombreCliente", "Glosa",
                   
                    };
                    var heads = new[]
                    {
                        new ExcelHeader{
                           // Title = "DETALLE", 
                            Headers = new ExcelHeader[]
                            {
                                "NRO. DOCUMENTO", "EMISIÓN", "VENCIMIENTO", "SOLES", "DOLARES", "PROVEEDOR", "DESCRIPCIÓN",
                                
                            }
                        },
                        //new ExcelHeader
                        //{
                        //    Title = "DEUDA", Headers = new ExcelHeader[]{"SOLES", "DOLARES"}
                        //}
                        //,
                        //new ExcelHeader 
                        //{
                        //    Title = "UBIGEO CLIENTE", Headers = new ExcelHeader[]{"DEPARTAMENTO", "PROVINCIA","DISTRITO"}
                        //}
                    };
                    #endregion

                    var excel = new ExcelReport(dt) { Headers = heads };
                    excel.AutoSizeColumns(1, 20, 15, 15, 20, 20, 40, 60);
                    excel.SetTitle("CUENTAS POR PAGAR");

                    // excel[2].Cells[4].Value = fecha;
                    excel.SetHeaders();
                    var last = new int[0];
                    var group = 0;
                    excel.EndSection += excel_EndSection;
                    //var filtros = new[] { "Almacen", "v_NombreProducto" };
                    excel.SetData(ref objOperationResult ,columnas);
                    var path = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + @".xlsx");
                    excel.Generate(path);
                    System.Diagnostics.Process.Start(path);

                }
                else
                {


                    ds1.Tables.Add(dt);
                    ds1.Tables[0].TableName = "dsReporteCuentasPagar";
                    rp.SetDataSource(ds1);
                    rp.SetParameterValue("NombreEmpresa", Empresa.FirstOrDefault().NombreEmpresaPropietaria.Trim());
                    rp.SetParameterValue("RucEmpresa", Empresa.FirstOrDefault().RucEmpresaPropietaria.Trim());
                    rp.SetParameterValue("NroRegistros", ReporteCuentasPagar.Count());
                    rp.SetParameterValue("FechaReporte", "DEL " + dtpFecha.Value.Date.Day.ToString("00") + "/" + dtpFecha.Value.Date.Month.ToString("00") + "/" + dtpFecha.Value.Date.Year.ToString() + " AL " + dtpFechaAl.Value.Date.Day.ToString("00") + "/" + dtpFechaAl.Value.Date.Month.ToString("00") + "/" + dtpFechaAl.Value.Date.Year.ToString());
                    crystalReportViewer1.ReportSource = rp;
                    crystalReportViewer1.Show();
                }
            }


                , TaskScheduler.FromCurrentSynchronizationContext());
        }


        private void excel_EndSection(object sender, ExcelReportSectionEventArgs e)
        {

            if (e.StartPosition == e.EndPosition) return;
            var obj = (ExcelReport)sender;
            //obj[e.StartPosition - 1].Cells[1].Value = e.FirsRow["v_NombreProducto"];
            obj.SetFormulas(3, "TOTALES: ", string.Format("=SUM(E{0}:E{1})", e.StartPosition + 1, e.EndPosition),  string.Format("=SUM(F{0}:F{1})", e.StartPosition + 1, e.EndPosition));
            obj.CurrentPosition++;
            

        }

        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + " Reporte de Cuentas por Pagar"
                : @"Reporte de Cuentas por Pagar";
            pBuscando.Visible = estado;
            btnVisualizar.Enabled = !estado;

        }

        private void cboTipoDocumento_Leave(object sender, EventArgs e)
        {

            if (cboTipoDocumento.Text.Trim() == "")
            {
                cboTipoDocumento.Value = "-1";
            }
            else
            {
                var x = _ListadoComboDocumentos.Find(p => p.Id == cboTipoDocumento.Value.ToString() | p.Id == cboTipoDocumento.Text.Trim());
                if (x == null)
                {
                    cboTipoDocumento.Value = "-1";
                }
            }
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

        private void txtProveedor_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscarProveedor")
            {
                OperationResult objOperationResult = new OperationResult();
                Mantenimientos.frmBuscarProveedor frm = new Mantenimientos.frmBuscarProveedor("", "RUC");
                frm.ShowDialog();
                if (frm._IdProveedor != null)
                {
                    
                    txtProveedor.Text = frm._CodigoProveedor.ToUpper();
                }
              
            }
        }

        private void txtProveedor_Validated(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (txtProveedor.Text.Trim() != string.Empty)
            {
                var Proveedor = _objClienteBL.ObtenerClienteCodigoBandejasCodigo(ref objOperationResult, txtProveedor.Text.Trim(), "V");
                if (Proveedor != null)
                {
                   
                    txtProveedor.Text = Proveedor.v_CodCliente.Trim();
                }
                else
                {
                    
                    txtProveedor.Clear();
                }

            }
            else
            {
               
                txtProveedor.Clear();
            }
        }

        private void uckConsiderarRangoFechas_CheckedChanged(object sender, EventArgs e)
        {
            dtpFecha.Enabled = uckConsiderarRangoFechas.Checked;
            dtpFechaAl.Enabled = uckConsiderarRangoFechas.Checked;
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            VisualizarReporte(true);
        }

     

    }
}
