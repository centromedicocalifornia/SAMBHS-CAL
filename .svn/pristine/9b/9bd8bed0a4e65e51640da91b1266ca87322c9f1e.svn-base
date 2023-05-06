#region Name Space
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
#endregion

namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    public partial class frmRegistroVentaProductoMensual : Form
    {
        #region Declaraciones / Referencias
        VendedorBL _objVendedorBL = new VendedorBL();
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        EstablecimientoBL objEstablecimientoBL = new EstablecimientoBL();
        NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL();
        MarcaBL _objMarcaBL = new MarcaBL();
        LineaBL _objLineaBL = new LineaBL();
        CancellationTokenSource _cts = new CancellationTokenSource();
        List<string> Grupollave = new List<string>();
        List<string> NombreGrupollave = new List<string>();
        int ConsideraDocumentosContables = -1;
        public string _Modalidad = "";
        string IdProductoDetalle = "";
        #endregion
        #region Carga de inicializacion
        public frmRegistroVentaProductoMensual(string Modalidad)
        {
            InitializeComponent();
            _Modalidad = Modalidad;
            ConsideraDocumentosContables = Modalidad == Constants.ModuloContable ? 0 : 1;
           
            
        }
        #endregion
        #region Cargar Load
        private void frmRegistroVentaProductoMensual_Load(object sender, EventArgs e)
        {

            this.BackColor = new GlobalFormColors().FormColor;
            CargarCombos();
            if (_Modalidad == Constants.ModuloContable)
            {
                ConsideraDocumentosContables = 1;
                cboTipoDocumento.Value = ConsideraDocumentosContables == 1 ? "1" : "-1";
            }

            else ConsideraDocumentosContables = int.Parse(cboTipoDocumento.Value.ToString());
            cboTipoDocumento.Visible = ConsideraDocumentosContables != 1;
            lblTipoDocumento.Visible = ConsideraDocumentosContables != 1;


        }

        private void CargarCombos()
        {
            OperationResult objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboOrden, "Value1", "Value2", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 85, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboConsiderar, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 84, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboLinea, "Value1", "Id", _objLineaBL.LlenarComboLinea(ref objOperationResult, "v_CodLinea"), DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboMarca, "Value1", "Id", _objMarcaBL.LlenarComboMarca(ref objOperationResult, "v_CodMarca", "-1"), DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboVendedor, "Value1", "Id", _objVendedorBL.ObtenerListadoVendedorParaCombo(ref objOperationResult, null), DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboEstablecimiento, "Value1", "Id", new EstablecimientoBL().ObtenerEstablecimientosValueDto(ref objOperationResult, null).OrderBy(r => r.Id).ToList(), DropDownListAction.All);
            cboEstablecimiento.Value = Globals.ClientSession.i_IdEstablecimiento.Value.ToString();
            cboMoneda.Value = Globals.ClientSession.i_IdMoneda.ToString();
            cboOrden.SelectedIndex = 1;
            cboConsiderar.Value = "1";
            cboLinea.Value = "-1";
            cboMarca.Value = "-1";
            cboVendedor.Value = "-1";
            dtpFechaRegistroDe.MinDate = DateTime.Parse("01/01/" + Globals.ClientSession.i_Periodo.ToString());

            if (Globals.ClientSession.v_RucEmpresa == Constants.RucHormiguita)
            {

                this.cboAgrupar.Items.Add("-1", "--Seleccionar--");
                this.cboAgrupar.Items.Add("1", "LINEA");
                this.cboAgrupar.Items.Add("2", "SIN AGRUPAR");
                this.cboAgrupar.Items.Add("3", "VENDEDOR");
                this.cboAgrupar.Items.Add("4", "LINEA, MARCA Y MODELO");
                this.cboAgrupar.Items.Add("5", "LINEA, MARCA , MODELO Y TALLA");


            }
            else
            {
                this.cboAgrupar.Items.Add("-1", "--Seleccionar--");
                this.cboAgrupar.Items.Add("1", "LINEA");
                this.cboAgrupar.Items.Add("2", "SIN AGRUPAR");
                this.cboAgrupar.Items.Add("3", "VENDEDOR");

            }

            cboAgrupar.Value = "2";
            cboTipoDocumento.Value = "-1";
        
        
        }
        #endregion
        #region Cargar Reporte
        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Reporte de Ventas por Producto (Mensual)"
                : @"Reporte de Ventas por Producto (Mensual)";
            pBuscando.Visible = estado;
            BtnVuisualizar.Enabled = !estado;
        }
        private void CargarReporte(DateTime FechaRegistroIni, DateTime FechaRegistroFin, int IdMoneda, string FechaHoraImpresion, string IdCliente, string IdProducto, string Orden, string Agrupar, bool Export)
        {
            List<ReporteRegistroVentaProductoMensual> aptitudeCertificate = new List<ReporteRegistroVentaProductoMensual>();
            OperationResult objOperationResult = new OperationResult();
            ReportDocument rp = new ReportDocument();
            if (int.Parse(cboConsiderar.Value.ToString()) == (int)TipoReporteVentasMensual.Unidades)
            {
                rp = new Reportes.Ventas.crRegistroVentaProductoMensualUnidades();
            }
            else
            {
                rp = new Reportes.Ventas.crRegistroVentaProductoMensual();
            }
            var idEstablecimiento = cboEstablecimiento.Value != null ? int.Parse(cboEstablecimiento.Value.ToString()) : -1;
            OcultarMostrarBuscar(true);
            Cursor.Current = Cursors.WaitCursor;
            var objVentaBL = new VentaBL();
            Task.Factory.StartNew(() =>
            {
               // aptitudeCertificate = objVentaBL.ReporteRegistroVentaProductoMensual(ref objOperationResult, 0, FechaRegistroIni, FechaRegistroFin, IdMoneda, IdCliente, IdProducto, Orden, ConsideraDocumentosInternos, Agrupar, cboLinea.Value.ToString(), cboMarca.Value.ToString());
                aptitudeCertificate = objVentaBL.ReporteRegistroVentaProductoMensualUltimo(ref objOperationResult, idEstablecimiento, FechaRegistroIni, FechaRegistroFin, IdMoneda, IdCliente, IdProducto, Orden, int.Parse ( cboTipoDocumento.Value.ToString ()), Agrupar, cboLinea.Value.ToString(), cboMarca.Value.ToString(), int.Parse(cboConsiderar.Value.ToString()), cboVendedor.Value.ToString(),Export);
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
                        UltraMessageBox.Show("Ocurrió un Error al realizar  Reporte de Ventas por Producto (Mensual) ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show("Ocurrió un Error al realizar  Reporte de Ventas por Producto (Mensual) ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }


                var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();
                DataSet ds1 = new DataSet();

                DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
                DataTable dt2 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate2);
                if (Export)
                {
                    aptitudeCertificate = aptitudeCertificate.OrderBy(l => l.GrupoLlave).ToList();
                    dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
                    #region Headers

                    //var columnas = new[]
                    //{
                    //    "IdProducto", "NombreProducto", int.Parse ( cboConsiderar.Value.ToString ())==(int)TipoReporteVentasMensual.Unidades ? "C_Enero":"T_Enero",
                    //    int.Parse ( cboConsiderar.Value.ToString ())==(int)TipoReporteVentasMensual.Unidades ? "C_Febrero":"T_Febrero", int.Parse ( cboConsiderar.Value.ToString ())==(int)TipoReporteVentasMensual.Unidades ? "C_Marzo":"T_Marzo",
                    //    int.Parse ( cboConsiderar.Value.ToString ())==(int)TipoReporteVentasMensual.Unidades ? "C_Abril":"T_Abril",int.Parse ( cboConsiderar.Value.ToString ())==(int)TipoReporteVentasMensual.Unidades ? "C_Mayo":"T_Mayo",
                    //    int.Parse ( cboConsiderar.Value.ToString ())==(int)TipoReporteVentasMensual.Unidades ? "C_Junio":"T_Junio",int.Parse ( cboConsiderar.Value.ToString ())==(int)TipoReporteVentasMensual.Unidades ? "C_Julio":"T_Julio",
                    //    int.Parse ( cboConsiderar.Value.ToString ())==(int)TipoReporteVentasMensual.Unidades ? "C_Agosto":"T_Agosto",int.Parse ( cboConsiderar.Value.ToString ())==(int)TipoReporteVentasMensual.Unidades ? "C_Setiembre":"T_Setiembre",
                    //    int.Parse ( cboConsiderar.Value.ToString ())==(int)TipoReporteVentasMensual.Unidades ? "C_Octubre":"T_Octubre", int.Parse ( cboConsiderar.Value.ToString ())==(int)TipoReporteVentasMensual.Unidades ? "C_Noviembre":"T_Noviembre",
                    //    int.Parse ( cboConsiderar.Value.ToString ())==(int)TipoReporteVentasMensual.Unidades ? "C_Diciembre":"T_Diciembre" ,int.Parse ( cboConsiderar.Value.ToString ())==(int)TipoReporteVentasMensual.Unidades ? "T_Cantidad":"T_Total"
                    //};

                    var columnas = new[]
                    {
                        "IdProducto", "NombreProducto","C_Enero","T_Enero", "C_Febrero","T_Febrero","C_Marzo","T_Marzo",
                         "C_Abril","T_Abril","C_Mayo","T_Mayo",
                       "C_Junio","T_Junio", "C_Julio","T_Julio",
                       "C_Agosto","T_Agosto", "C_Setiembre","T_Setiembre",
                        "C_Octubre","T_Octubre",  "C_Noviembre","T_Noviembre",
                        "C_Diciembre","T_Diciembre" , "T_Cantidad","T_Total"
                    };
                    //var heads = new[]
                    //{
                    //    new ExcelHeader{
                    //        Title = "", 
                    //        Headers = new ExcelHeader[]
                    //        {
                    //         "CÓDIGO", "DESCRIPCIÓN", "ENERO","FEBRERO","MARZO","ABRIL","MAYO","JUNIO","JULIO","AGOSTO","SETIEMBRE","OCTUBRE","NOVIEMBRE","DICIEMBRE","TOTAL"
                    //         //"CÓDIGO", "DESCRIPCIÓN"
                    //        }
                    //    },

                        

                    //};
                    var heads = new[]
                    {
                        new ExcelHeader{
                            Title = "", 
                            Headers = new ExcelHeader[]
                            {
                                  "CÓDIGO", "DESCRIPCIÓN",
                            }
                        },
                        new ExcelHeader
                        {
                            Title = "ENERO", Headers = new ExcelHeader[]{"CANTIDAD", int.Parse ( cboConsiderar.Value.ToString ())==(int)TipoReporteVentasMensual.Unidades ?"PRECIO VENTA" :"VALOR VENTA"}
                        }
                        ,
                        new ExcelHeader 
                        {
                            Title = "FEBRERO", 
                            Headers = new ExcelHeader[]
                            {
                                "CANTIDAD",int.Parse ( cboConsiderar.Value.ToString ())==(int)TipoReporteVentasMensual.Unidades ?"PRECIO VENTA":"VALOR VENTA"
                            }
                        },

                         new ExcelHeader 
                        {
                            Title = "MARZO", 
                            Headers = new ExcelHeader[]
                            {
                                "CANTIDAD",int.Parse ( cboConsiderar.Value.ToString ())==(int)TipoReporteVentasMensual.Unidades ?"PRECIO VENTA":"VALOR VENTA"
                            }
                        },

                         new ExcelHeader 
                        {
                            Title = "ABRIL", 
                            Headers = new ExcelHeader[]
                            {
                                "CANTIDAD",int.Parse ( cboConsiderar.Value.ToString ())==(int)TipoReporteVentasMensual.Unidades ?"PRECIO VENTA":"VALOR VENTA"
                            }
                        },
                         new ExcelHeader 
                        {
                            Title = "MAYO", 
                            Headers = new ExcelHeader[]
                            {
                                "CANTIDAD",int.Parse ( cboConsiderar.Value.ToString ())==(int)TipoReporteVentasMensual.Unidades ?"PRECIO VENTA":"VALOR VENTA"
                            }
                        },
                         new ExcelHeader 
                        {
                            Title = "JUNIO", 
                            Headers = new ExcelHeader[]
                            {
                                "CANTIDAD",int.Parse ( cboConsiderar.Value.ToString ())==(int)TipoReporteVentasMensual.Unidades ?"PRECIO VENTA":"VALOR VENTA"
                            }
                        },
                         new ExcelHeader 
                        {
                            Title = "JULIO", 
                            Headers = new ExcelHeader[]
                            {
                                "CANTIDAD",int.Parse ( cboConsiderar.Value.ToString ())==(int)TipoReporteVentasMensual.Unidades ?"PRECIO VENTA":"VALOR VENTA"
                            }
                        },
                         new ExcelHeader 
                        {
                            Title = "AGOSTO", 
                            Headers = new ExcelHeader[]
                            {
                                "CANTIDAD",int.Parse ( cboConsiderar.Value.ToString ())==(int)TipoReporteVentasMensual.Unidades ?"PRECIO VENTA":"VALOR VENTA"
                            }
                        },
                         new ExcelHeader 
                        {
                            Title = "SETIEMBRE", 
                            Headers = new ExcelHeader[]
                            {
                                "CANTIDAD",int.Parse ( cboConsiderar.Value.ToString ())==(int)TipoReporteVentasMensual.Unidades ?"PRECIO VENTA":"VALOR VENTA"
                            }
                        },

                         new ExcelHeader 
                        {
                            Title = "OCTUBRE", 
                            Headers = new ExcelHeader[]
                            {
                                "CANTIDAD",int.Parse ( cboConsiderar.Value.ToString ())==(int)TipoReporteVentasMensual.Unidades ?"PRECIO VENTA":"VALOR VENTA"
                            }
                        },

                         new ExcelHeader 
                        {
                            Title = "NOVIEMBRE", 
                            Headers = new ExcelHeader[]
                            {
                                "CANTIDAD",int.Parse ( cboConsiderar.Value.ToString ())==(int)TipoReporteVentasMensual.Unidades ?"PRECIO VENTA":"VALOR VENTA"
                            }
                        },
                         new ExcelHeader 
                        {
                            Title = "DICIEMBRE", 
                            Headers = new ExcelHeader[]
                            {
                                "CANTIDAD",int.Parse ( cboConsiderar.Value.ToString ())==(int)TipoReporteVentasMensual.Unidades ?"PRECIO VENTA":"VALOR VENTA"
                            }

                        },

                    };

                    #endregion

                    var excel = new ExcelReport(dt) { Headers = heads };
                    excel.AutoSizeColumns(1, 20, 50, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 35, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 35);
                    excel.SetTitle("VENTAS POR PRODUCTO MENSUAL");

                    excel[2].Cells[4].Value = "DEL " + FechaRegistroIni.Date.Day.ToString("00") + "/" + FechaRegistroIni.Date.Month.ToString("00") + "/" + FechaRegistroIni.Date.Year.ToString() + " AL " + FechaRegistroFin.Date.Day.ToString("00") + "/" + FechaRegistroFin.Date.Month.ToString("00") + "/" + FechaRegistroFin.Year.ToString();
                    excel.SetHeaders();
                    var last = new int[0];
                    var group = 0;
                    excel.EndSection += excel_EndSection;

                    if (cboAgrupar.Text != "SIN AGRUPAR")
                    {
                        excel.EndSectionGroup += (sender, e) =>
                        {
                            var obj = (ExcelReport)sender;

                            if (e.EndSections.Length == 0) return;

                            excel.EndSection += excel_EndSection;
                            //if (group == 0)
                            //{
                            //     obj.SetFormulas(2, "TOTAL FINAL", "=" + string.Join("+", e.EndSections.Select(i => "D" + i).ToArray()), "=" + string.Join("+", e.EndSections.Select(i => "E" + i).ToArray()), "=" + string.Join("+", e.EndSections.Select(i => "F" + i).ToArray()), "=" + string.Join("+", e.EndSections.Select(i => "G" + i).ToArray()), "=" + string.Join("+", e.EndSections.Select(i => "H" + i).ToArray()), "=" + string.Join("+", e.EndSections.Select(i => "I" + i).ToArray()), "=" + string.Join("+", e.EndSections.Select(i => "J" + i).ToArray()), "=" + string.Join("+", e.EndSections.Select(i => "K" + i).ToArray()), "=" + string.Join("+", e.EndSections.Select(i => "L" + i).ToArray()), "=" + string.Join("+", e.EndSections.Select(i => "M" + i).ToArray()), "=" + string.Join("+", e.EndSections.Select(i => "N" + i).ToArray()), "=" + string.Join("+", e.EndSections.Select(i => "O" + i).ToArray()), "=" + string.Join("+", e.EndSections.Select(i => "P" + i).ToArray()));
                            //}


                        };
                    }
                    var filtros = new[] { "GrupoLlave" };
                    excel.SetData(ref objOperationResult ,columnas, filtros);
                    // InsertTable(excel, last);
                    var path = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + @".xlsx");
                    excel.Generate(path);
                    System.Diagnostics.Process.Start(path);


                }
                else
                {

                    ds1.Tables.Add(dt2);
                    ds1.Tables.Add(dt);

                    ds1.Tables[0].TableName = "dsEmpresa";
                    ds1.Tables[1].TableName = "dsRegistroVentaProductoMensual";
                    rp.SetDataSource(ds1);
                    rp.SetParameterValue("FechaHoraImpresion", FechaHoraImpresion);

                    rp.SetParameterValue("IdMoneda", IdMoneda);

                    rp.SetParameterValue("Fecha", "DEL " + FechaRegistroIni.Date.Day.ToString("00") + "/" + FechaRegistroIni.Date.Month.ToString("00") + "/" + FechaRegistroIni.Date.Year.ToString() + " AL " + FechaRegistroFin.Date.Day.ToString("00") + "/" + FechaRegistroFin.Date.Month.ToString("00") + "/" + FechaRegistroFin.Year.ToString());
                    rp.SetParameterValue("Presentacion", cboConsiderar.Text);
                    rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count());
                    rp.SetParameterValue("NombreEmpresa", aptitudeCertificate2.FirstOrDefault().NombreEmpresaPropietaria.Trim());
                    rp.SetParameterValue("RucEmpresa", "R.U.C. : " + aptitudeCertificate2.FirstOrDefault().RucEmpresaPropietaria.Trim());
                    rp.SetParameterValue("Agrupar", cboAgrupar.Text.Trim());
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
            obj.SetFormulas(2, cboAgrupar.Text == "SIN AGRUPAR" ? "TOTAL POR MES : " : cboAgrupar.Text == "VENDEDOR" ? "TOTAL POR VENDEDOR" : "TOTAL POR LINEA", string.Format("=SUM(D{0}:D{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(E{0}:E{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(F{0}:F{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(G{0}:G{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(H{0}:H{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(I{0}:I{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(J{0}:J{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(K{0}:K{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(L{0}:L{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(M{0}:M{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(N{0}:N{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(O{0}:O{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(P{0}:P{1})", e.StartPosition + 1, e.EndPosition),
                string.Format("=SUM(Q{0}:Q{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(R{0}:R{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(S{0}:S{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(T{0}:T{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(U{0}:U{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(V{0}:V{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(W{0}:W{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(X{0}:X{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(Y{0}:Y{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(Z{0}:Z{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(AA{0}:AA{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(AB{0}:AB{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(AC{0}:AC{1})", e.StartPosition + 1, e.EndPosition));
            obj.CurrentPosition++;


        }



        #endregion





        private void BtnVuisualizar_Click(object sender, EventArgs e)
        {
            Reporte(false);
        }
        private void Reporte(bool Export)
        {
            try
            {

                if (uvDatos.Validate(true, false).IsValid)
                {
                  
                    CargarReporte(DateTime.Parse(dtpFechaRegistroDe.Text), DateTime.Parse(dtpFechaRegistroAl.Text + " 23:59"), int.Parse(cboMoneda.Value.ToString()), chkHoraimpresion.Checked == true ? "1" : "0", TxtCodigo.Text.Trim() == "" ? "" : TxtCodigo.Text.Trim(), txtCodigoProducto.Text.Trim() == "" ? "" : txtCodigoProducto.Text.Trim(), cboOrden.Value.ToString() == "-1" ? "" : cboOrden.Value.ToString(), cboAgrupar.Text.Trim(), Export);

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

        private void frmRegistroVentaProductoMensual_FormClosing(object sender, FormClosingEventArgs e)
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

        private void TxtCodigo_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

            OperationResult objOperationResult = new OperationResult();
            Mantenimientos.frmBuscarCliente frm = new Mantenimientos.frmBuscarCliente("VV", TxtCodigo.Text.Trim());
            frm.ShowDialog();

            if (  frm._IdCliente != null)
            {

                TxtCodigo.Text = frm._CodigoCliente;
               
            }
           

        }

        private void txtCodigoProducto_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            Mantenimientos.frmBuscarProducto frm = new Mantenimientos.frmBuscarProducto(-1, "REPORTE", null, null);
            frm.ShowDialog();

            if (!string.IsNullOrEmpty ( frm._IdProducto))
            {
                txtCodigoProducto.Text = frm._CodigoInternoProducto.Trim();
                IdProductoDetalle = frm._IdProducto;
            }
            else
            {

                IdProductoDetalle = "";
            }
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
            Reporte(true);
        }




    }
}
