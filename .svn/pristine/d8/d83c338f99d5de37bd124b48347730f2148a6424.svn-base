using CrystalDecisions.CrystalReports.Engine;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.Tesoreria.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Tesoreria
{

    public partial class frmReporteRetenciones : Form
    {
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        public frmReporteRetenciones(string p)
        {
            InitializeComponent();
        }

        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Reporte de Retenciones"
                : @"Reporte de Retenciones";
            pBuscando.Visible = estado;
            btnImprimir.Enabled = btnExcel.Enabled = !estado;
        }


        private void Imprimir(bool Export)
        {
            try
            {


                if (Validar.Validate(true, false).IsValid)
                {
                    string fecha = "DEL " + dtpFechaInicio.Text + " AL " + dtpFechaFin.Text;
                    var objOperationResult = new OperationResult();
                    OcultarMostrarBuscar(true);

                    Cursor.Current = Cursors.WaitCursor;
                    List<ReporteRetenciones> _result = new List<ReporteRetenciones>();
                    Task.Factory.StartNew(() =>
                    {
                        _result = new TesoreriaBL().ReporteRetenciones(ref objOperationResult, dtpFechaInicio.Value.Date, DateTime.Parse(dtpFechaFin.Text + " 23:59"), txtProveedor.Text.Trim());


                    }, _cts.Token)
                    .ContinueWith(t =>
                    {
                        if (_cts.IsCancellationRequested) return;
                        OcultarMostrarBuscar(false);
                        Cursor.Current = Cursors.Default;
                        if (objOperationResult.Success == 0)
                        {
                            UltraMessageBox.Show("Ocurrió un error al realizar Reporte de Retenciones" + objOperationResult.AdditionalInformation, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        ReportDocument rp = new ReportDocument();
                        if (int.Parse(cboFormato.Value.ToString()) == 1)
                            rp = new Reportes.Tesoreria.crReporteRetenciones();
                        else rp = new Reportes.Tesoreria.crReporteRetencionesAuxiliar();
                        var dt = Utils.ConvertToDatatable(_result);

                        if (Export)
                        {
                            //#region Headers

                            //var columnas = new[]
                            //{
                            //    "v_Fecha", "v_NombreTipoMovimiento", "Guia", "Documento", "ClienteProveedor","Ingreso_CantidadInicial","Salida_Cantidad","Saldo_Cantidad"
                            //};


                            //var heads = new[]
                            //{
                            //    new ExcelHeader{
                            //        Title = "", 
                            //        Headers = new ExcelHeader[]
                            //        {
                            //             "FECHA", "MOVIMIENTO", "GUÍA", "DOCUMENTO", "CLIENTE"
                            //        }
                            //    },
                            //    new ExcelHeader
                            //    {
                            //        Title = "ENTRADAS", Headers = new ExcelHeader[]{"CANTIDAD"}
                            //    }
                            //    ,
                            //    new ExcelHeader 
                            //    {
                            //        Title = "SALIDAS", 
                            //        Headers = new ExcelHeader[]
                            //        {
                            //            "CANTIDAD"
                            //        }
                            //    },

                            //     new ExcelHeader 
                            //    {
                            //        Title = "SALDOS", 
                            //        Headers = new ExcelHeader[]
                            //        {
                            //            "CANTIDAD"
                            //        }
                            //    }
                            //};
                            //#endregion

                            //var excel = new ExcelReport(dt) { Headers = heads };
                            //excel.AutoSizeColumns(1, 20, 25, 25, 25, 50, 25, 25, 25);
                            //excel.SetTitle("KARDEX FÍSICO");

                            //excel[2].Cells[4].Value = fecha;
                            //excel.SetHeaders();
                            //excel.EndSection += excel_EndSection;
                            //excel.EndSectionGroup += excel_EndSectionGroup;
                            //var filtros = new[] { "Almacen", "v_NombreProducto" };
                            //excel.SetData(columnas, filtros);
                            //// InsertTable(excel, last);
                            //var path = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + @".xlsx");
                            //excel.Generate(path);
                            //System.Diagnostics.Process.Start(path);

                        }
                        else
                        {
                            dt.TableName = "dsReporteRetenciones";
                            using (var ds1 = new DataSet())
                            {
                                ds1.Tables.Add(dt);
                                rp.SetDataSource(ds1);
                            }
                            rp.SetParameterValue("Fecha", fecha);
                            rp.SetParameterValue("NroRegistros", _result.Count());
                            rp.SetParameterValue("NombreEmpresa", Globals.ClientSession.v_CurrentExecutionNodeName);
                            rp.SetParameterValue("RucEmpresa", "R.U.C. : " + Globals.ClientSession.v_RucEmpresa);
                            rp.SetParameterValue("IncluirNroPagina", uckNroPagina.Checked);
                            crystalReportViewer1.ReportSource = rp;
                            crystalReportViewer1.Show();
                            crystalReportViewer1.Zoom(110);
                        }

                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show("Ocurrió un error al realizar Reporte Retenciones .\n Información Adicional :" + ex.Message, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
        private void btnImprimir_Click(object sender, EventArgs e)
        {
            Imprimir(false);
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            Imprimir(true);
        }

        private void frmReporteRetenciones_Load(object sender, EventArgs e)
        {
            cboFormato.Value = "1";

        }

        private void txtProveedor_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            Mantenimientos.frmBuscarProveedor frm = new Mantenimientos.frmBuscarProveedor("REPORTE", "RUC");
            frm.ShowDialog();
            if (frm._IdProveedor != null)
            {

                txtProveedor.Text = frm._NroDocumento.Trim();
            }
        }
    }
}
