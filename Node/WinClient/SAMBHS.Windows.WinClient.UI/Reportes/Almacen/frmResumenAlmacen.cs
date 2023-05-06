using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Almacen.BL;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using SAMBHS.Common.BL;
using SAMBHS.Common.BE;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Almacen
{
    public partial class frmResumenAlmacen : Form
    {
        EstablecimientoBL objEstablecimientoBL = new EstablecimientoBL();
        LineaBL _objLineaBL = new LineaBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        string _whereAlmacenesConcatenados, _AlmacenesConcatenados;
        List<KeyValueDTO> _ListadoGruposOrdenar = new List<KeyValueDTO>();
        CancellationTokenSource _cts = new CancellationTokenSource();
        MarcaBL _objMarcaBL = new MarcaBL();
        public string v_CodigoInterno = string.Empty;
        public string v_Pedido = string.Empty;
        public frmResumenAlmacen(string pstrParametro)
        {
            InitializeComponent();
        }

        private void frmResumenAlmacen_Load(object sender, EventArgs e)
        {

            this.BackColor = new GlobalFormColors().FormColor;
            CargarCombos();
            ValidarFechas();
            chkIncluirNroPedido.Checked = Globals.ClientSession.v_RucEmpresa == Constants.RucWortec;
        }

        private void ValidarFechas()
        {
            string Periodo = Globals.ClientSession.i_Periodo.ToString();
            string Mes = DateTime.Today.Month.ToString();
            if (DateTime.Now.Year.ToString().Trim() == Periodo)
            {
                dtpFechaInicio.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/" + "01").ToString());
                //dtpFechaInicio.MaxDate = DateTime.Parse((Periodo + "/" + Mes + "/" + (DateTime.DaysInMonth(int.Parse(Periodo), int.Parse(Mes))).ToString()).ToString());
                dtpFechaInicio.MaxDate = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                dtpFechaInicio.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());

                dtpFechaFin.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/" + "01").ToString());
                //dtpFechaFin.MaxDate = DateTime.Parse((Periodo + "/" + Mes + "/" + (DateTime.DaysInMonth(int.Parse(Periodo), int.Parse(Mes))).ToString()).ToString());
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

        private void CargarCombos()
        {
            OperationResult objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList (cboEstablecimiento, "Value1", "Id", objEstablecimientoBL.ObtenerEstablecimientosValueDto(ref objOperationResult, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", objEstablecimientoBL.GetAlmacenesXEstablecimiento(-1), DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboOrdenar, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 96, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboLinea, "Value1", "Id", _objLineaBL.LlenarComboLinea(ref objOperationResult, "v_CodLinea"), DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboMarca, "Value1", "Id", _objMarcaBL.LlenarComboMarca(ref objOperationResult, "v_CodMarca", "-1"), DropDownListAction.All);

            cboMoneda.Value = Globals.ClientSession.i_IdMonedaCompra.Value.ToString();
            
            cboOrdenar.Value = "1";
            cboEstablecimiento.Value = Globals.ClientSession.i_IdEstablecimiento.ToString();
            cboEstablecimiento.Enabled = false;
            cboLinea.Value = "-1";
            cboAlmacen.Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
            cboMarca.Value = "-1";
            cboFormato.Value = ((int)FormatoCantidad.UnidadMedidaProducto).ToString();
         
        }
        private void cboEstablecimiento_SelectedIndexChanged(object sender, EventArgs e)
        {
            _whereAlmacenesConcatenados = string.Empty;
            _AlmacenesConcatenados = string.Empty;
            List<KeyValueDTO> x = new List<KeyValueDTO>();
            if (cboEstablecimiento.Value == null) return;

            x = objEstablecimientoBL.GetAlmacenesXEstablecimiento(int.Parse(cboEstablecimiento.Value.ToString()));

            if (x.Count > 0)
            {
                foreach (var item in x)
                {
                    _whereAlmacenesConcatenados = _whereAlmacenesConcatenados + "IdAlmacen==" + item.Id + " || ";
                    _AlmacenesConcatenados = _AlmacenesConcatenados + item.Value1 + ", ";
                }
                _whereAlmacenesConcatenados = _whereAlmacenesConcatenados.Substring(0, _whereAlmacenesConcatenados.Length - 4);
                _AlmacenesConcatenados = cboEstablecimiento.Text + " / " + _AlmacenesConcatenados.Substring(0, _AlmacenesConcatenados.Length - 2);
            }
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", x, DropDownListAction.All);
            cboAlmacen.Value = "-1";
        }

        private void btnVisualizar_Click(object sender, EventArgs e)
        {

            Reporte(false);
        }
        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Reporte de Resumen de Almacén"
                : @"Reporte de Resumen de Almacén";
            pBuscando.Visible = estado;

            
            btnVisualizar.Enabled = !estado;
        }
        private void CargarReporte(DateTime FechaInicialAnterior,DateTime FechaFinalAnterior, DateTime FechaInicial, DateTime FechaFinal, string pstrAlmacenes, int pIntEstablecimiento, string pstrOrdenar, int pIntMoneda, string pstrLinea,string pstrCodigoInterno,string pstrPedido,bool Export)
        {
           
            var rp = new Reportes.Almacen.crReporteResumenAlmacen();
            OperationResult objOperationResult= new OperationResult ();
            List<ReporteResumenAlmacen> aptitudeCertificate = new List<ReporteResumenAlmacen> ();
            var Empresa = new NodeBL().ReporteEmpresa();
            OcultarMostrarBuscar(true);

            Cursor.Current = Cursors.WaitCursor;
            Task.Factory.StartNew(() =>
            {

              
                aptitudeCertificate = new AlmacenBL().ReporteResumenAlmacen(ref objOperationResult, FechaInicialAnterior, FechaFinalAnterior, FechaInicial, FechaFinal, pstrAlmacenes, pIntEstablecimiento, pstrOrdenar, pIntMoneda, pstrLinea, pstrCodigoInterno, pstrPedido, cboMarca.Value.ToString(), int.Parse(cboFormato.Value.ToString()) , chkIncluirNroPedido.Checked ?1:0 ,uckSoloMovimientos.Checked ?1:0);
                },_cts.Token)
            .ContinueWith(t =>
            {
                if (_cts.IsCancellationRequested) return;
                OcultarMostrarBuscar(false);
                Cursor.Current = Cursors.Default;
                if (objOperationResult.Success == 0)
                {
                    if (!string.IsNullOrEmpty(objOperationResult.ExceptionMessage))
                    {
                        UltraMessageBox.Show("Ocurrió un error al realizar Reporte Resumen Almacén", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show("Ocurrió un error al realizar Reporte Resumen Almacén", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }

                
                var aptitudeCertificate1 = new NodeBL().ReporteEmpresa();

            DataSet ds1 = new DataSet();

            DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
            DataTable dt2 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate1);

            if (Export)
            {
                aptitudeCertificate = aptitudeCertificate.OrderBy(l => l.v_almacen).ToList();
                dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
                #region Headers

                var columnas = new[]
                    {
                        "codigoProducto", "descripcionProducto", "pedido","unidadMedida" ,"cantidadAnterior", "precioAnterior","valorAnterior","cantidadEntrada","valorEntrada","cantidadSalida","valorSalida","cantidadActual","precioActual","valorActual"
                    };


                var heads = new[]
                    {
                        new ExcelHeader{
                            Title = "", 
                            Headers = new ExcelHeader[]
                            {
                                 "CÓDIGO", "DESCRIPCIÓN", "PEDIDO","U.M."
                            }
                        },
                        new ExcelHeader
                        {
                            Title = "SALDO ANTERIOR", Headers = new ExcelHeader[]{"CANTIDAD","P. UNITARIO","VALOR"}
                        }
                        ,
                        new ExcelHeader 
                        {
                            Title = "ENTRADAS", 
                            Headers = new ExcelHeader[]
                            {
                                "CANTIDAD","VALOR"
                            }
                        },

                         new ExcelHeader 
                        {
                            Title = "SALIDAS", 
                            Headers = new ExcelHeader[]
                            {
                                "CANTIDAD","VALOR"
                            }
                        },
                         new ExcelHeader 
                        {
                            Title = "SALDO ACTUAL", 
                            Headers = new ExcelHeader[]
                            {
                                "CANTIDAD","P. UNITARIO","VALOR",
                            }
                        }


                    };

                #endregion

                var excel = new ExcelReport(dt) { Headers = heads };
                excel.AutoSizeColumns(1, 20, 50, 25, 25, 25, 25, 25, 25,25,25,25,25,25,25);
                excel.SetTitle("RESUMEN DE ALMACÉN");

                excel[2].Cells[4].Value = "DEL  " + dtpFechaInicio.Value.Date.Day.ToString("00") + "/" + dtpFechaInicio.Value.Date.Month.ToString("00") + "/" + dtpFechaInicio.Value.Year.ToString() + "  AL  " + dtpFechaFin.Value.Date.Day.ToString("00") + "/" + dtpFechaFin.Value.Month.ToString("00") + "/" + dtpFechaFin.Value.Year.ToString();
                excel.SetHeaders();
                var last = new int[0];
                var group = 0;
               // excel.EndSection += excel_EndSection;
                var filtros = new[] { "v_almacen" };
                excel.SetData(ref objOperationResult ,columnas, filtros);
                // InsertTable(excel, last);
                var path = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + @".xlsx");
                excel.Generate(path);
                System.Diagnostics.Process.Start(path);
            }
            else
            {
                ds1.Tables.Add(dt);
                ds1.Tables.Add(dt2);
                ds1.Tables[0].TableName = "dsReporteResumenAlmacen";
                ds1.Tables[1].TableName = "dsEmpresa";


                rp.SetDataSource(ds1);



                rp.SetParameterValue("Fecha", "DEL  " + dtpFechaInicio.Value.Date.Day.ToString("00") + "/" + dtpFechaInicio.Value.Date.Month.ToString("00") + "/" + dtpFechaInicio.Value.Year.ToString() + "  AL  " + dtpFechaFin.Value.Date.Day.ToString("00") + "/" + dtpFechaFin.Value.Month.ToString("00") + "/" + dtpFechaFin.Value.Year.ToString());
                rp.SetParameterValue("Moneda", "MONEDA DEL REPORTE :" + cboMoneda.Text.ToString());

                rp.SetParameterValue("Establecimiento", "ESTABLECIMIENTO : " + cboEstablecimiento.Text.Trim().ToUpper());
                rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count());
                rp.SetParameterValue("NombreEmpresa", Empresa.FirstOrDefault().NombreEmpresaPropietaria);
                rp.SetParameterValue("RucEmpresa", "R.U.C. : " + Empresa.FirstOrDefault().RucEmpresaPropietaria.Trim());
                rp.SetParameterValue("CantidadDecimales",Globals.ClientSession.i_CantidadDecimales.Value );

                crystalReportViewer1.ReportSource = rp;
                crystalReportViewer1.Show();
            }
            
            }
                , TaskScheduler.FromCurrentSynchronizationContext());
            

        }
        private void dtpFechaFin_ValueChanged(object sender, EventArgs e)
        {
            dtpFechaInicio.MaxDate = dtpFechaFin.Value;
        }

      

        private void txtCodigoProducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
            {
                v_CodigoInterno = string.Empty;
                
            }
        }

        private void txtCodigoProducto_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            Mantenimientos.frmBuscarProducto frm = new Mantenimientos.frmBuscarProducto(-1, "REPORTE", null, null);
            frm.ShowDialog();

            if (frm._IdProducto != null)
            {
                txtCodigoProducto.Text = frm._CodigoInternoProducto.Trim();
               
                v_CodigoInterno = frm._CodigoInternoProducto.Trim();

            }
            else
            {
                txtCodigoProducto.Text = string.Empty;
               
                v_CodigoInterno = string.Empty;
            }
        }

        private void frmResumenAlmacen_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }

        private void cboLinea_ValueChanged(object sender, EventArgs e)
        {

            OperationResult objOperationResult = new OperationResult();
            if (cboLinea.Value == null || cboLinea.Value.ToString() == "-1") return;

            Utils.Windows.LoadUltraComboEditorList(cboMarca, "Value1", "Id", _objMarcaBL.LlenarComboMarca(ref objOperationResult, "v_CodMarca", cboLinea.Value.ToString()), DropDownListAction.Select);
            cboMarca.Value = "-1";
        }

        private void ultraExpandableGroupBox1_ExpandedStateChanged(object sender, EventArgs e)
        {
            if (ultraExpandableGroupBox1.Expanded == true)
            {
                groupBox2.Location = new Point(groupBox2.Location.X, ultraExpandableGroupBox1.Location.Y + ultraExpandableGroupBox1.Height + 5);
                groupBox2.Height = this.Height - groupBox2.Location.Y - 7;
            }
            else
            {
                groupBox2.Location = new Point(groupBox2.Location.X, ultraExpandableGroupBox1.Location.Y + ultraExpandableGroupBox1.Height + 5);
                groupBox2.Height = this.Height - groupBox2.Location.Y - 7;
            }
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            Reporte(true);
        }
        private void Reporte(bool Export)
        {
            List<string> Filters = new List<string>();
            string _strFilterExpression = string.Empty, strOrderExpression = string.Empty, pstrCodigoProducto = string.Empty;
            OperationResult objOperationResult = new OperationResult();
            int pintMoneda = -1;
            v_Pedido = string.Empty; v_CodigoInterno = string.Empty;
            _ListadoGruposOrdenar = _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 96, null);
            if (uvValidar.Validate(true, false).IsValid)
            {
                if (cboAlmacen.Value.ToString() != "-1")
                {
                    Filters.Add("IdAlmacen==" + cboAlmacen.Value.ToString());
                }
                else
                {
                    Filters.Add(_whereAlmacenesConcatenados);
                }

                //Filters.Add(_whereAlmacenesConcatenados);
                if (Filters.Count > 0)
                {
                    foreach (string item in Filters)
                    {
                        _strFilterExpression = _strFilterExpression + item + " && ";
                    }
                    _strFilterExpression = _strFilterExpression.Substring(0, _strFilterExpression.Length - 4);
                }



                for (int i = 0; i <= _ListadoGruposOrdenar.Count - 1; i++)
                {
                    if (cboOrdenar.Text.Trim() == _ListadoGruposOrdenar[i].Value1.ToString().Trim())
                    {
                        strOrderExpression = _ListadoGruposOrdenar[i].Value3.ToString();
                    }

                }
                pintMoneda = int.Parse(cboMoneda.Value.ToString());
                pstrCodigoProducto = txtCodigoProducto.Text.Trim();
                DateTime FechaInicial = dtpFechaInicio.Value.Date;
                DateTime FechaFinal = DateTime.Parse(dtpFechaFin.Value.Day.ToString() + "/" + dtpFechaFin.Value.Month.ToString() + "/" + dtpFechaFin.Value.Year.ToString() + " 23:59");
                DateTime FechaFinalAnterior;
                DateTime FechaInicialAnterior = DateTime.Parse("01/01/" + Globals.ClientSession.i_Periodo.ToString());
                if (dtpFechaInicio.Value.Date.Day == 1 && dtpFechaInicio.Value.Month != 1)
                {


                    int dasymonth = DateTime.DaysInMonth(int.Parse(dtpFechaInicio.Value.Year.ToString()), int.Parse(dtpFechaInicio.Value.Month.ToString()) - 1);
                    FechaFinalAnterior = DateTime.Parse((DateTime.DaysInMonth(int.Parse(dtpFechaInicio.Value.Year.ToString()), int.Parse(dtpFechaInicio.Value.Month.ToString()) - 1)).ToString() + "/" + (int.Parse(dtpFechaInicio.Value.Month.ToString()) - 1).ToString() + "/" + dtpFechaInicio.Value.Year + " 23:59");

                }
                else if (dtpFechaInicio.Value.Date.Day == 1 && dtpFechaInicio.Value.Month == 1)
                {
                    FechaFinalAnterior = DateTime.Parse("01/01/" + Globals.ClientSession.i_Periodo.ToString() + " 23:59");
                }
                else
                {
                    FechaFinalAnterior = DateTime.Parse((dtpFechaInicio.Value.Day - 1).ToString() + "/" + dtpFechaInicio.Value.Month.ToString() + "/" + dtpFechaInicio.Value.Year.ToString() + " 23:59");
                }

                if (txtCodigoProducto.Text != string.Empty) v_CodigoInterno = txtCodigoProducto.Text.Trim();
                if (txtPedido.Text != string.Empty) v_Pedido = txtPedido.Text.Trim();


                CargarReporte(FechaInicialAnterior, FechaFinalAnterior, FechaInicial, FechaFinal, _strFilterExpression, int.Parse(cboEstablecimiento.Value.ToString()), strOrderExpression, pintMoneda, cboLinea.Value.ToString(), v_CodigoInterno, v_Pedido, Export);

            }
        }

       
    }
}
