using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.ActivoFijo.BL;
using System.Globalization;

using SAMBHS.Common.BL;
using System.Threading.Tasks;
using System.Threading;
using SAMBHS.Common.BE;
using CrystalDecisions.CrystalReports.Engine;
using System.IO; 

namespace SAMBHS.Windows.WinClient.UI.Reportes.ActivoFijo
{
    public partial class frmReporteResumenDepreciacion : Form
    {
        CancellationTokenSource _cts = new CancellationTokenSource();
        public frmReporteResumenDepreciacion(string pstrParametro)
        {
            InitializeComponent();
        }

        private void frmReporteResumenDepreciacion_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            ValidarPeriodo();
        }
        private void ValidarPeriodo()
        {
            nupAnio.Minimum = 1900;
            nupAnio.Maximum = 2050;
            nupMes.Minimum = 1;
            nupMes.Maximum = 12;
            nupAnio.Value = decimal.Parse(Globals.ClientSession.i_Periodo.ToString());
            nupMes.Value = decimal.Parse(DateTime.Now.Month.ToString());
        }
        private void Reporte(bool Excel)
        {
            try
            {

                CargarReporte(int.Parse(nupAnio.Value.ToString()), int.Parse(nupMes.Value.ToString()),Excel);
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show("Se produjo un error  al mostrar el reporte ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }
        
        }
        private void btnVisualizar_Click(object sender, EventArgs e)
        {
            Reporte(false);
        }
        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Resumen de Depreciación"
                : @"Resumen de Depreciación";
            pBuscando.Visible = estado;

            btnVisualizar.Enabled = !estado;
        }

        private void CargarReporte(int Periodo,int mes,bool Excel)
        {

            ReportDocument rp = new ReportDocument();
            string FechaPeriodo = new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(int.Parse(nupMes.Value.ToString())).ToUpper() + " DEL " + nupAnio.Value.ToString();
          
            if (uckMostrarDetalles.Checked)
            {
                rp = new Reportes.ActivoFijo.crReporteResumenDepreciacionDetallado();
            }
            else
            {
                rp = new Reportes.ActivoFijo.crReporteResumenDepreciacion();
            }
            OperationResult objOperationResult = new OperationResult();
              OcultarMostrarBuscar(true);
              List<ReporteResumenDepreciacionDto> aptitudeCertificate = new List<ReporteResumenDepreciacionDto>();
            Cursor.Current = Cursors.WaitCursor;

            Task.Factory.StartNew(() =>
            {
                aptitudeCertificate = new ActivoFijoBL().ReporteResumenDepreciacion(ref  objOperationResult, Periodo, mes,uckMostrarDetalles.Checked );
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
                        UltraMessageBox.Show("Ocurrió un error al realizar Resumen de Activo Fijo", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show("Ocurrió un error al realizar Resumen de Activo Fijo", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }
                var Empresa = new NodeBL().ReporteEmpresa();
                DataSet ds1 = new DataSet();
                DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
                if (Excel)
                {
                    if (uckMostrarDetalles.Checked)
                    {
                        #region Headers
                        var columnas = new[]
                            {
                                "CodigoActivoFijo", "SaldoAl", "DepreciacionActual", "Adiciones","Retiros","Total","Ajuste","SaldoTotal"
                            };


                        var heads = new[]
                            {
                                new ExcelHeader{
                                    //Title = "", 
                                    Headers = new ExcelHeader[]
                                    {
                                         "DESCRIPCIÓN" ,"SALDO AL 31/12","DEPRECIADOS EN EL"+ Globals.ClientSession.i_Periodo.ToString (),"ADICIONES (ALTAS)"+ Globals.ClientSession.i_Periodo.ToString (),"RETIROS(BAJAS)"+ Globals.ClientSession.i_Periodo.ToString (),"DEPRECIACIÓN ACUM. EN EL "+ Globals.ClientSession.i_Periodo.ToString (),"AJUSTE ACUMULADO EN EL PERIODO "+ Globals.ClientSession.i_Periodo.ToString (),"SALDO DEPRECIACIÓN AL 12/"+Globals.ClientSession.i_Periodo.ToString ()
                                    }
                                },
                            };
                        #endregion

                        var excel = new ExcelReport(dt) { Headers = heads };
                        excel.AutoSizeColumns(1, 80, 25, 25, 25, 25, 25, 25, 25);
                        excel.SetTitle("RESUMEN DE DEPRECIACIÓN");
                        excel[2].Cells[4].Value = FechaPeriodo;
                        excel.SetHeaders();
                        excel.EndSection += excel_EndSection;
                        //excel.EndSectionGroup += excel_EndSectionGroup;
                        var filtros = new[] { "KeyCuenta" };
                        excel.SetData( ref objOperationResult ,columnas,filtros); 
                        // InsertTable(excel, last);
                        var path = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + @".xlsx");
                        excel.Generate(path);
                        System.Diagnostics.Process.Start(path);

                    }
                    else
                    { 
                    
                    
                    }

                }
                else
                {

                    
                    ds1.Tables.Add(dt);
                    ds1.Tables[0].TableName = "dsReporteResumenDepreciacion";
                    rp.SetDataSource(ds1);
                    rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count());
                    rp.SetParameterValue("NombreEmpresa", Empresa.FirstOrDefault().NombreEmpresaPropietaria.Trim());
                    rp.SetParameterValue("RucEmpresa", Empresa.FirstOrDefault().RucEmpresaPropietaria.Trim());
                    rp.SetParameterValue("Titulo", "AL MES DE  : " + new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(int.Parse(nupMes.Value.ToString())).ToUpper() + " DEL " + nupAnio.Value.ToString());
                    rp.SetParameterValue("Saldo", "SALDO AL " + "31/12/" + (Periodo - 1).ToString());
                    rp.SetParameterValue("Depreciacion", "DEPRECIADOS EN EL  " + (Periodo).ToString());
                    rp.SetParameterValue("Adiciones", "ADICIONES(ALTAS) \n " + (Periodo).ToString());
                    rp.SetParameterValue("Retiros", "RETIROS(BAJAS) \n" + Periodo.ToString());
                    rp.SetParameterValue("DepreciacionAl", "DEPRECIACIONES ACUM. \n EN EL PERIODO " + Periodo.ToString());
                    rp.SetParameterValue("Ajuste", "AJUSTE ACUM. \n EN EL PERIODO " + Periodo.ToString());
                    rp.SetParameterValue("SaldoTotal", "SALDO DEPRECIACIÓN AL " + mes.ToString("00") + "/" + Periodo.ToString());

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
           // obj[e.StartPosition - 1].Cells[1].Value = e.FirsRow["v_NombreProducto"];
           // obj.SetFormulas(2, "TOTAL POR PRODUCTO : ", string.Format("=SUM(C{0}:C{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(H{0}:H{1})", e.StartPosition + 1, e.EndPosition), string.Format("=I{0}", e.EndPosition));
            obj.SetFormulas(2, "TOTAL POR SUB-CUENTA", Enumerable.Range(0, 2).Select(i => string.Format("=${2}{0}+${2}{1}", e.EndPosition + 1, e.EndPosition + 2, (char)('C' + i))).ToArray());
            obj.CurrentPosition++;


            

        }



        private void frmReporteResumenDepreciacion_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            Reporte(true);
        }


    }
}
