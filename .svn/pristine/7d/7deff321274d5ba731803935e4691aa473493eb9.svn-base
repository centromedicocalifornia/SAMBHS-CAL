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
using System.IO;
namespace SAMBHS.Windows.WinClient.UI.Reportes.Almacen
{
    public partial class frmReporteTrazabilidad : Form
    {

        string _whereAlmacenesConcatenados;
        string _AlmacenesConcatenados;
        public int ConsiderarDocumentosInternos = -1;
        List<ReporteTrazabilidad> aptitudeCertificate = new List<ReporteTrazabilidad>();
        CancellationTokenSource _cts = new CancellationTokenSource();
        MarcaBL _objMarcaBL = new MarcaBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        ReportDocument rp = new ReportDocument();
        public frmReporteTrazabilidad(string Modo)
        {
            InitializeComponent();
            ConsiderarDocumentosInternos = Modo == "C" ? 0 : 1;
        }

        private void BtnImprimir_Click(object sender, EventArgs e)
        {
            //DestruirCrystal(rp);
            VisualizarReporte(false);

        }

        private void DestruirCrystal(ReportDocument doc1)
        {

            if (doc1 != null)
            {
                doc1.Close();
                doc1.Dispose();
                crystalReportViewer1.ReportSource = null;
                crystalReportViewer1.Dispose();
                crystalReportViewer1 = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        private void VisualizarReporte(bool ExcelExport)
        {
            try
            {
                OperationResult objOperationResult = new OperationResult();
                string CodigoProducto = string.Empty, nroPedido = string.Empty;
                if (uvValidar.Validate(true, false).IsValid)
                {
                    DateTime fecha1 = Convert.ToDateTime("01/01/" + Globals.ClientSession.i_Periodo.ToString() + " 00:00");
                    DateTime fecha2 = Convert.ToDateTime(dtpFechaInicio.Text + " 23:59");
                    string _strFilterExpression = string.Empty;
                    List<string> Filters = new List<string>();
                    if (txtCodArticulo.Text.Trim() != string.Empty) CodigoProducto = txtCodArticulo.Text.Trim();

                    if (cboAlmacen.Value.ToString() != "-1")
                    {
                        Filters.Add("IdAlmacen==" + cboAlmacen.Value.ToString());
                    }
                    else
                    {
                        Filters.Add("(" + _whereAlmacenesConcatenados + ")");
                    }
                    var Empresa = new NodeBL().ReporteEmpresa();

                    {

                        CargarReporte();
                    }
                }
            }
            catch (Exception f)
            {

                UltraMessageBox.Show("Ocurrió un error al realizar Reporte Stock Consolidado" + f.InnerException, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }



        }

        private void LimpiarFiltros()
        {
            cboEstablecimiento.Value = "-1";
        }
        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Reporte de Trazabilidad"
                : @"Reporte de Trazabilidad";
            pBuscando.Visible = estado;

            BtnImprimir.Enabled = !estado;
        }
        private void CargarReporte()
        {
            OperationResult objOperationResult = new OperationResult();


            OcultarMostrarBuscar(true);
            Cursor.Current = Cursors.WaitCursor;

            Task.Factory.StartNew(() =>
            {
                aptitudeCertificate = new AlmacenBL().ReporteTrazabilidad(ref objOperationResult, dtpFechaInicio.Value, dtpFechaHasta.Value, txtCodArticulo.Text.Trim(), int.Parse(cboAlmacen.Value.ToString()));

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
                        UltraMessageBox.Show("Ocurrió un error al realizar Reporte de Trazabilidad.\n Información Adicional :" + objOperationResult.AdditionalInformation, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show("Ocurrió un error al realizar Reporte de Trazabilidad.\n Información Adicional :" + objOperationResult.AdditionalInformation, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }
                var Empresa = new NodeBL().ReporteEmpresa();
                rp = new Reportes.Almacen.crReporteTrazabilidad();
                DataSet ds1 = new DataSet();
                DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
                dt.TableName = "dsReporteTrazabilidad";
                ds1.Tables.Add(dt);
                rp.SetDataSource(ds1);
                rp.SetParameterValue("Fecha", "AL  " + dtpFechaInicio.Value.Day.ToString("00") + "/" + dtpFechaInicio.Value.Month.ToString("00") + "/" + dtpFechaInicio.Value.Year.ToString());
                rp.SetParameterValue("Establecimiento", "ESTABLECIMIENTO : " + cboEstablecimiento.Text.Trim().ToUpper());
                rp.SetParameterValue("DecimalesCantidad", Globals.ClientSession.i_CantidadDecimales);
                rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count());
                rp.SetParameterValue("NombreEmpresa", Empresa.FirstOrDefault().NombreEmpresaPropietaria.Trim());
                rp.SetParameterValue("RucEmpresa", "R.U.C. : " + Empresa.FirstOrDefault().RucEmpresaPropietaria.Trim());
                rp.SetParameterValue("AlmacenElegido", int.Parse(cboAlmacen.Value.ToString()));
                crystalReportViewer1.ReportSource = rp;
                crystalReportViewer1.Show();
                crystalReportViewer1.Zoom(110);

            }
                 , TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void excel_EndSection(object sender, ExcelReportSectionEventArgs e)
        {
            //if (int.Parse(cboModalidad.Value.ToString()) == (int)TipoKardex.Fisico)
            //{
            if (e.StartPosition == e.EndPosition) return;
            var obj = (ExcelReport)sender;
            //obj[e.StartPosition - 1].Cells[1].Value = e.FirsRow["v_NombreProducto"];
            obj.SetFormulas(3, "TOTAL : ", string.Format("=SUM(E{0}:E{1})", e.StartPosition + 1, e.EndPosition));
            obj.CurrentPosition++;
            //}
            //else
            //{

            //if (e.StartPosition == e.EndPosition) return;
            //var obj = (ExcelReport)sender;
            //// obj[e.StartPosition - 1].Cells[1].Value = e.FirsRow["v_NombreProducto"];
            //obj.SetFormulas(3, "TOTAL : ", string.Format("=SUM(E{0}:E{1})", e.StartPosition + 1, e.EndPosition), "", "", string.Format("=SUM(H{0}:H{1})", e.StartPosition + 1, e.EndPosition));
            //obj.CurrentPosition++;


            //}
        }


        private void frmReporteTrazabilidad_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            EstablecimientoBL objEstablecimientoBL = new EstablecimientoBL();
            LineaBL _objLineaBL = new LineaBL();
            NodeWarehouseBL objNodeWarehouseBL = new NodeWarehouseBL();
            OperationResult objOperationResult = new OperationResult();

            List<KeyValueDTO> ListaTiposModalidad = new List<KeyValueDTO>();
            KeyValueDTO objDetallado = new KeyValueDTO();
            if (Globals.ClientSession.v_RucEmpresa == Constants.RucWortec)
            {
                objDetallado = new KeyValueDTO();
                objDetallado.Id = "3";
                objDetallado.Value1 = "FISICO AUXILIAR";
                ListaTiposModalidad.Add(objDetallado);

                objDetallado = new KeyValueDTO();
                objDetallado.Id = "4";
                objDetallado.Value1 = "VALORIZADO AUXILIAR";
                ListaTiposModalidad.Add(objDetallado);
            }


            objDetallado = new KeyValueDTO();
            objDetallado.Id = "1";
            objDetallado.Value1 = "FISICO";
            ListaTiposModalidad.Add(objDetallado);

            objDetallado = new KeyValueDTO();
            objDetallado.Id = "2";
            objDetallado.Value1 = "VALORIZADO";
            ListaTiposModalidad.Add(objDetallado);


            Utils.Windows.LoadUltraComboEditorList(cboEstablecimiento, "Value1", "Id", objEstablecimientoBL.ObtenerEstablecimientosValueDto(ref objOperationResult, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", objEstablecimientoBL.GetAlmacenesXEstablecimiento(-1), DropDownListAction.All);



            ValidarFechas();
            cboEstablecimiento.Value = Globals.ClientSession.i_IdEstablecimiento.ToString();
            cboAlmacen.Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();


            cboFormato.Value = ((int)FormatoCantidad.UnidadMedidaProducto).ToString();



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
            }
            else
            {
                dtpFechaInicio.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/01").ToString());
                dtpFechaInicio.MaxDate = DateTime.Parse((Periodo + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(Periodo), 12)).ToString()).ToString());
                dtpFechaInicio.Value = DateTime.Parse((Periodo + "/12/31").ToString());
            }
        }
        private void cboEstablecimiento_SelectedIndexChanged(object sender, EventArgs e)
        {
            EstablecimientoBL objEstablecimientoBL = new EstablecimientoBL();
            List<KeyValueDTO> x = new List<KeyValueDTO>();
            _whereAlmacenesConcatenados = string.Empty;
            _AlmacenesConcatenados = string.Empty;
            if (cboEstablecimiento.Value == null) return;

            x = objEstablecimientoBL.GetAlmacenesXEstablecimiento(int.Parse(cboEstablecimiento.Value.ToString()));

            if (x.Count > 0)
            {
                foreach (var item in x)
                {
                    _whereAlmacenesConcatenados = _whereAlmacenesConcatenados + "IdAlmacen==" + item.Id + " || ";
                    _AlmacenesConcatenados = _AlmacenesConcatenados + item.Value1 + "/ ";
                }
                _whereAlmacenesConcatenados = _whereAlmacenesConcatenados.Substring(0, _whereAlmacenesConcatenados.Length - 4);
                //_AlmacenesConcatenados = cboEstablecimiento.Text + " / " + _AlmacenesConcatenados.Substring(0, _AlmacenesConcatenados.Length - 2);
                _AlmacenesConcatenados = _AlmacenesConcatenados.Substring(0, _AlmacenesConcatenados.Length - 2);
            }

            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", x, DropDownListAction.All);
            cboAlmacen.Value = "-1";

        }

        private void txtCodArticulo_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscarArticulo")
            {

                OperationResult objOperationResult = new OperationResult();
                Mantenimientos.frmBuscarProducto frm = new Mantenimientos.frmBuscarProducto(-1, "REPORTE", null, null);
                frm.ShowDialog();
                if (frm._IdProducto != null)
                {
                    // txtNroCuenta.Text = frmPlanCuentasConsulta._NroSubCuenta.Trim();
                    txtCodArticulo.Text = frm._CodigoInternoProducto.Trim();
                }
                else
                {
                    // txtNroCuenta.Clear();
                    txtCodArticulo.Clear();
                }
            }

        }



        private void frmStock_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }

        private void ultraExpandableGroupBox1_ExpandedStateChanged(object sender, EventArgs e)
        {
            if (ultraExpandableGroupBox1.Expanded == true)
            {
                groupBox1.Location = new Point(groupBox1.Location.X, ultraExpandableGroupBox1.Location.Y + ultraExpandableGroupBox1.Height + 5);
                groupBox1.Height = this.Height - groupBox1.Location.Y - 7;
            }
            else
            {
                groupBox1.Location = new Point(groupBox1.Location.X, ultraExpandableGroupBox1.Location.Y + ultraExpandableGroupBox1.Height + 5);
                groupBox1.Height = this.Height - groupBox1.Location.Y - 7;
            }
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            VisualizarReporte(true);
        }








    }
}
