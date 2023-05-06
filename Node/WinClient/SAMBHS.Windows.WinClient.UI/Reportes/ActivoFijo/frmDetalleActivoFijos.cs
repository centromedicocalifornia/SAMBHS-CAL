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
using SAMBHS.Common.BL;
using System.Globalization;
using System.Threading.Tasks;
using System.Threading;
using SAMBHS.Common.BE;
using System.IO;

namespace SAMBHS.Windows.WinClient.UI.Reportes.ActivoFijo
{
    public partial class frmDetalleActivoFijos : Form
    {
        CancellationTokenSource _cts = new CancellationTokenSource();
        public frmDetalleActivoFijos(string pstrParametro)
        {
            InitializeComponent();
        }

        private void frmDetalleActivoFijos_Load(object sender, EventArgs e)
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
            nupAnio.Value =decimal.Parse ( Globals.ClientSession.i_Periodo.ToString ());
            nupMes.Value = decimal.Parse(DateTime.Now.Month.ToString());  
        }
        private void btnVisualizar_Click(object sender, EventArgs e)
        {
            Reporte(false);
        }

        private void Reporte(bool Excel)
        {
            try
            {

                CargarReporte(int.Parse(nupAnio.Value.ToString()), int.Parse(nupMes.Value.ToString()),Excel);
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show("Se produjo un error  al mostrar el reporte ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }
        
        }


        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Detalles de los Activos Fijos"
                : @"Detalles de los Activos Fijos";
            pBuscando.Visible = estado;

            btnVisualizar.Enabled = !estado;
        }


        private void CargarReporte(int Periodo, int mes,bool Excel)
        {

            var rp = new Reportes.ActivoFijo.crDetallesActivoFijos();
            OperationResult objOperationResult = new OperationResult();
              OcultarMostrarBuscar(true);
              List<ReporteRelacionBienesDto> aptitudeCertificate = new List<ReporteRelacionBienesDto>();
            Cursor.Current = Cursors.WaitCursor;
            string Orden = "CodigoActivoFijo";
            Task.Factory.StartNew(() =>
            {
                aptitudeCertificate = new ActivoFijoBL().ReporteRelacionBienes(ref objOperationResult,"" ,"","",Orden,int.Parse (nupAnio.Value.ToString ()), int.Parse (nupMes.Value.ToString ()),string.IsNullOrEmpty (txtCodigoDesde.Text.Trim ())?0:  int.Parse (txtCodigoDesde.Text) ,string.IsNullOrEmpty (txtCodigoHasta.Text.Trim ())?0: int.Parse (txtCodigoHasta.Text) );
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

                var TituloPeriodo = "PERIODO : " + new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(int.Parse(nupMes.Value.ToString())).ToUpper() + " DEL " + nupAnio.Value.ToString();
                var Empresa = new NodeBL().ReporteEmpresa();
                DataSet ds1 = new DataSet();
                DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
                if (Excel)
                {


                    {
                        #region Headers

                        var columnas = new[]
                            {
                                "CodigoActivoFijo", "Cuenta33", "DescripcionActivoFijo", "Marca", "Serie","Modelo","ValorCompra","AdquisionesAdicionales","Mejoras","Bajas","ValorHistorico","sFechaAdquisicion","FechaUso","PorcentajeDepreciacion","DepreciacionCierreAnterior","DepreciacionEjercicio","Ajuste","DepreciacionAcumulada"
                            };


                        var heads = new[]
                            {
                                new ExcelHeader{
                                    Title = "", 
                                    Headers = new ExcelHeader[]
                                    {
                                         "CÓDIGO  DEL ACT. FIJO",
                                    }
                                },
                                new ExcelHeader
                                {
                                    Title ="",
                                    Headers= new ExcelHeader []
                                    {
                                    "CUENTA CONTABLE DEL ACT. FIJO",
                                    }
                                }
                                ,
                                new ExcelHeader 
                                {
                                    Title = "DETALLE DEL ACTIVO FIJO", 
                                    Headers = new ExcelHeader[]
                                    {
                                        "DESCRIPCIÓN DEL ACTIVO FIJO","MARCA","SERIE","PLACA"
                                    }
                                },

                                 new ExcelHeader 
                                {
                                    Title = "", 
                                    Headers = new ExcelHeader[]
                                    {
                                        "SALDO INICIAL"
                                    }
                                },

                                 new ExcelHeader 
                                {
                                    Title = "", 
                                    Headers = new ExcelHeader[]
                                    {
                                        "ADQ. ADICIONALES"
                                    }
                                },
                                 new ExcelHeader 
                                {
                                    Title = "", 
                                    Headers = new ExcelHeader[]
                                    {
                                        "MEJORAS"
                                    }
                                },
                                 new ExcelHeader 
                                {
                                    Title = "", 
                                    Headers = new ExcelHeader[]
                                    {
                                        "RETIROS Y/O BAJAS"
                                    }
                                },

                                 new ExcelHeader 
                                {
                                    Title = "", 
                                    Headers = new ExcelHeader[]
                                    {
                                        "VALOR HISTORICO AL 31/12"
                                    }
                                },
                                 new ExcelHeader 
                                {
                                    Title = "", 
                                    Headers = new ExcelHeader[]
                                    {
                                        "FECHA ADQ."
                                    }
                                },
                                 new ExcelHeader 
                                {
                                    Title = "", 
                                    Headers = new ExcelHeader[]
                                    {
                                        "FECHA USO"
                                    }
                                },

                                 new ExcelHeader 
                                {
                                    Title = "", 
                                    Headers = new ExcelHeader[]
                                    {
                                        "% DEPREC."
                                    }
                                },
                                 new ExcelHeader 
                                {
                                    Title = "", 
                                    Headers = new ExcelHeader[]
                                    {
                                        "DEPRECIACIÓN ACUMULADA AL CIERRE DEL "
                                    }
                                },


                                 new ExcelHeader 
                                {
                                    Title = "", 
                                    Headers = new ExcelHeader[]
                                    {
                                        "DEPRECIACIÓN DEL EJERCICIO AL 31/12"
                                    }
                                },

                                   new ExcelHeader 
                                {
                                    Title = "", 
                                    Headers = new ExcelHeader[]
                                    {
                                        "AJUSTE DEL EJERCICIO HASTA 31/12"
                                    }
                                },


                                
                                   new ExcelHeader 
                                {
                                    Title = "", 
                                    Headers = new ExcelHeader[]
                                    {
                                        "DEPRECIACIÓN ACUMULADA HISTÓRICA"
                                    }
                                },



                            };
                        #endregion

                        var excel = new ExcelReport(dt) { Headers = heads };
                        excel.AutoSizeColumns(1, 20, 25, 50, 25, 25, 25, 25, 25, 25, 25, 25, 25,25,25,25,25,25,25);
                        excel.SetTitle("REGISTRO DE ACTIVOS FIJOS - DETALLE DE LOS ACTIVOS FIJOS");

                        excel[2].Cells[4].Value = TituloPeriodo;
                        excel.SetHeaders();
                        //excel.EndSection += excel_EndSection;
                        //excel.EndSectionGroup += excel_EndSectionGroup;
                        //var filtros = new[] { "Almacen", "v_NombreProducto" };
                        excel.SetData( ref objOperationResult ,columnas); //, filtros);
                        // InsertTable(excel, last);
                        var path = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + @".xlsx");
                        excel.Generate(path);
                        System.Diagnostics.Process.Start(path);

                    }


                }
                else
                {

                   
                    ds1.Tables.Add(dt);
                    ds1.Tables[0].TableName = "dsReporteRelacionBienes";
                    rp.SetDataSource(ds1);
                    rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count());
                    rp.SetParameterValue("NombreEmpresa", Empresa.FirstOrDefault().NombreEmpresaPropietaria.Trim());
                    rp.SetParameterValue("RucEmpresa", "R.U.C. : " + Empresa.FirstOrDefault().RucEmpresaPropietaria.Trim());
                    rp.SetParameterValue("Periodo", TituloPeriodo);
                    rp.SetParameterValue("TituloDepreciacionEjercicio", "DEPRECIACIÓN DEL EJERCICIO HASTA  31/12"); //+new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(int.Parse ( nupMes.Value.ToString ())).ToUpper());
                    rp.SetParameterValue("TituloAjuste", "AJUSTE DEL EJERCICIO HASTA EL 31/12");
                    //rp.SetParameterValue ("Saldo","SALDO AL " +"31/12/"+  (Periodo -1 ).ToString () );
                    //rp.SetParameterValue("Adiciones", "ADICIONES \n (COMPRAS) " + ( Periodo ).ToString());
                    //rp.SetParameterValue("Retiros", "RETIROS(BAJAS) " + (Periodo).ToString());
                    //rp.SetParameterValue("SaldoAlMesAnio", "SALDO AL \n" + mes.ToString ("00")+"/"+ Periodo.ToString ());
                    //rp.SetParameterValue("Depreciaciones", "DEPRECIACIONES ACUM. AL " + mes.ToString("00") + "/" + Periodo.ToString()); 
                    crystalReportViewer1.ReportSource = rp;
                    crystalReportViewer1.Show();
                }
            }
                , TaskScheduler.FromCurrentSynchronizationContext());
        
        }

        private void frmReporteResumenActivoFijo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }

        private void txtCodigoDesde_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

        }

        private void txtCodigoHasta_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

        }

        private void txtCodigoDesde_Validating(object sender, CancelEventArgs e)
        {
            if (txtCodigoDesde.Text != string.Empty)
            {
                txtCodigoHasta.Text = txtCodigoDesde.Text;
            }
            else
            {
                txtCodigoHasta.Text = string.Empty;
            }
        }

        private void txtCodigoHasta_Validating(object sender, CancelEventArgs e)
        {
            if (txtCodigoHasta.Text == string.Empty && txtCodigoDesde.Text != string.Empty)
            {
                txtCodigoHasta.Text = txtCodigoDesde.Text;
            }
        }

        private void txtCodigoDesde_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCodigoDesde, e); 
        }

        private void txtCodigoHasta_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCodigoHasta, e); 
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            Reporte(true);
        }
    }
}
