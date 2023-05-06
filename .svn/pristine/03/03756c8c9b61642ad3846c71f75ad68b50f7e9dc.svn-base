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
    public partial class frmProgramacionFabricacion : Form
    {

        string _whereAlmacenesConcatenados;
        string _AlmacenesConcatenados;
        public int ConsiderarDocumentosInternos = -1;
        List<ProgramacionFabricacion> aptitudeCertificate = new List<ProgramacionFabricacion>();
        CancellationTokenSource _cts = new CancellationTokenSource();
        MarcaBL _objMarcaBL = new MarcaBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        ReportDocument rp = new ReportDocument();
        public frmProgramacionFabricacion(string Modo)
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
                string CodigoProducto = string.Empty, nroPedido = string.Empty;

                DateTime fecha1 = Convert.ToDateTime("01/01/" + Globals.ClientSession.i_Periodo.ToString() + " 00:00");


                CargarReporte(ExcelExport);


            }
            catch (Exception f)
            {

                UltraMessageBox.Show("Ocurrió un error al realizar Reporte Stock Consolidado" + f.InnerException, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }



        }


        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Programación de Fabricación"
                : @"Programación de Fabricación";
            pBuscando.Visible = estado;

            BtnImprimir.Enabled = !estado;
        }
        private void CargarReporte(bool ExcelExport)
        {
            OperationResult objOperationResult = new OperationResult();
            OcultarMostrarBuscar(true);
            Cursor.Current = Cursors.WaitCursor;
            Task.Factory.StartNew(() =>
            {
                aptitudeCertificate = new AlmacenBL().ProgramacionFabricacion(ref objOperationResult, DateTime.Parse(dtpFechaHasta.Text + " 23:59"));//.ReporteStock(ref objOperationResult,int.Parse ( cboEstablecimiento.Value.ToString ()), pdtFechaInicio, pdtFechaFin, pstrFilterExpression, pintIdMoneda, pstrCodigoProducto, NroPedido, Linea,chkMostrarconstock0.Checked ?1:0 ,chkMostrarconstock.Checked ?1 :0 ,chkMostrarconstockNegativo.Checked ?1:0,chkMostrarSoloStockMinimo.Checked ?1 :0, ConsideraDocInternos, FormatoCantidad);

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
                        UltraMessageBox.Show("Ocurrió un error al realizar Reporte Programación Fabricación.\n Información Adicional :" + objOperationResult.AdditionalInformation, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show("Ocurrió un error al realizar Reporte Programación Fabricación.\n Información Adicional :" + objOperationResult.AdditionalInformation, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }
                var Empresa = new NodeBL().ReporteEmpresa();
                rp = new Reportes.Almacen.crProgramacionFabricacion();
                DataSet ds1 = new DataSet();
                DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);

                if (ExcelExport)
                {
                   // aptitudeCertificate = aptitudeCertificate.OrderBy(l => l.CodProducto).ToList();
                    dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
                    #region Headers

                    var columnas = new[]
                    {
                        "CodigoProducto", "DescripcionProducto", "Stock","CantidadPedido", "SaldoUnidades","CantidadFabricacion","SaldoDias","sFechaFabricacion","Situacion"
                    };


                    var heads = new[]
                    {
                        new ExcelHeader{
                            Title = "", 
                            Headers = new ExcelHeader[]
                            {
                                 "CÓDIGO PRODUCTO ", "DESCRIPCIÓN", "STOCK", "CANTIDAD PEDIDO", "SALDO","CANT. FABRICACIÓN","SALDO (DIAS)","FECHA FABR.","SITUACIÓN"
                            }
                        },
                       

                    };

                    #endregion

                    var excel = new ExcelReport(dt) { Headers = heads };
                    excel.AutoSizeColumns(1, 20, 50, 25, 25, 25,25,25,25,100);
                    excel.SetTitle("PROGRAMACIÓN DE FABRICACIÓN");

                    excel[2].Cells[6].Value = "AL  " + dtpFechaHasta.Value.Day.ToString("00") + "/" + dtpFechaHasta.Value.Month.ToString("00") + "/" + dtpFechaHasta.Value.Year.ToString();

                    excel.SetHeaders();
                    //var last = new int[0];
                    //  var group = 0;
                    excel.EndSection += excel_EndSection;
                    // var filtros = new[] { "Almacen", "v_NombreProducto" };
                    excel.SetData(ref objOperationResult ,columnas);
                    // InsertTable(excel, last);
                    var path = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + @".xlsx");
                    excel.Generate(path);
                    System.Diagnostics.Process.Start(path);

                }
                else
                {


                    dt.TableName = "dsProgramacionFabricacion";
                    ds1.Tables.Add(dt);
                    rp.SetDataSource(ds1);
                    rp.SetParameterValue("Fecha", "AL  " + dtpFechaHasta.Value.Day.ToString("00") + "/" + dtpFechaHasta.Value.Month.ToString("00") + "/" + dtpFechaHasta.Value.Year.ToString());
                    rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count());
                    rp.SetParameterValue("NombreEmpresa", Empresa.FirstOrDefault().NombreEmpresaPropietaria.Trim());
                    rp.SetParameterValue("RucEmpresa", "R.U.C. : " + Empresa.FirstOrDefault().RucEmpresaPropietaria.Trim());
                   
                    crystalReportViewer1.ReportSource = rp;
                    crystalReportViewer1.Show();
                    crystalReportViewer1.Zoom(110);

                }




            }
                 , TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void excel_EndSection(object sender, ExcelReportSectionEventArgs e)
        {

        }


        private void frmProgramacionFabricacion_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            EstablecimientoBL objEstablecimientoBL = new EstablecimientoBL();
            LineaBL _objLineaBL = new LineaBL();
            NodeWarehouseBL objNodeWarehouseBL = new NodeWarehouseBL();

            //ValidarFechas();


        }
        //private void ValidarFechas()
        //{
        //    string Periodo = Globals.ClientSession.i_Periodo.ToString();
        //    string Mes = DateTime.Today.Month.ToString();
        //    if (DateTime.Now.Year.ToString().Trim() == Periodo)
        //    {
        //        dtpFechaHasta.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/" + "01").ToString());
        //        dtpFechaHasta.MaxDate = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());
        //        dtpFechaHasta.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());
        //    }
        //    else
        //    {
        //        dtpFechaHasta.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/01").ToString());
        //        dtpFechaHasta.MaxDate = DateTime.Parse((Periodo + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(Periodo), 12)).ToString()).ToString());
        //        dtpFechaHasta.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());
        //    }
        //}







        private void frmStock_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }



        private void btnExcel_Click(object sender, EventArgs e)
        {
            VisualizarReporte(true);
        }






    }
}
