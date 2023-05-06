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
    public partial class frmRegistroVentaClienteMensual : Form
    {
        #region Declaraciones / Referencias
        VendedorBL _objVendedorBL = new VendedorBL();
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        EstablecimientoBL objEstablecimientoBL = new EstablecimientoBL();
        NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL();
        MarcaBL _objMarcaBL = new MarcaBL();
        LineaBL _objLineaBL = new LineaBL();
        SystemParameterBL _objSystemParameterBL = new SystemParameterBL();
        CancellationTokenSource _cts = new CancellationTokenSource();
        List<string> Grupollave = new List<string>();
        List<string> NombreGrupollave = new List<string>();
        string IdCliente = string.Empty;
        int ConsideraDocumentosContables = -1;
        public string _Modalidad = "";
        #endregion
        #region Carga de inicializacion
        public frmRegistroVentaClienteMensual(string Modalidad)
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
            var ListaPaises = (_objSystemParameterBL.GetSystemParameterForComboKeyValueDto(ref objOperationResult, 112, null)).FindAll(p => p.Value3 == "-1");
            Utils.Windows.LoadUltraComboEditorList(cboPais, "Value1", "Id", ListaPaises, DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboDepartamento, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            // Utils.Windows.LoadUltraComboEditorList(cboOrden, "Value1", "Value2", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 85, null), DropDownListAction.Select);
            dtpFechaRegistroDe.MinDate = DateTime.Parse("01/01/" + Globals.ClientSession.i_Periodo.ToString());
            Utils.Windows.LoadUltraComboEditorList(cboEstablecimiento, "Value1", "Id", objEstablecimientoBL.ObtenerEstablecimientosValueDto(ref objOperationResult, null), DropDownListAction.Select);
            cboEstablecimiento.Value = Globals.ClientSession.i_IdEstablecimiento.Value.ToString();
            cboMoneda.Value = Globals.ClientSession.i_IdMoneda.ToString();
            cboOrden.Value = "-1";
            cboAgrupar.Value = "1";
            cboPais.Value = "1";
            cboDepartamento.Value = "-1";
            cboOrden.SelectedIndex = 1;
            cboTipoDocumento.Value = "-1";
        }
        #endregion
        #region Cargar Reporte


        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando..."
                : @"Reporte de Ventas por Cliente Mensual";
            pBuscando.Visible = estado;
            BtnVuisualizar.Enabled = !estado;
            btnExportar.Enabled = !estado;
             
        }


        private void CargarReporte(bool Export)
        {
            List<ReporteVentaClienteMensual> aptitudeCertificate = new List<ReporteVentaClienteMensual>();
            OperationResult objOperationResult = new OperationResult();
            ReportDocument rp = new ReportDocument();
            rp = new crRegistroVentaClienteMensual();
            OcultarMostrarBuscar(true);
            Cursor.Current = Cursors.WaitCursor;
            var objVentaBL = new VentaBL();
            Task.Factory.StartNew(() =>
            {
                //aptitudeCertificate = objVentaBL.ReporteVentasClienteMensual(ref objOperationResult, dtpFechaRegistroDe.Value.Date , DateTime.Parse(dtpFechaRegistroAl.Text + " 23:59"), IdCliente, int.Parse(cboEstablecimiento.Value.ToString()), int.Parse(cboMoneda.Value.ToString()), cboAgrupar.Text.Trim(), int.Parse(cboDepartamento.Value.ToString()));       
                aptitudeCertificate = objVentaBL.ReporteVentasClienteMensual(ref objOperationResult, dtpFechaRegistroDe.Value.Date, DateTime.Parse(dtpFechaRegistroAl.Text + " 23:59"), IdCliente, int.Parse(cboEstablecimiento.Value.ToString()), int.Parse(cboMoneda.Value.ToString()), cboAgrupar.Text.Trim(), int.Parse(cboDepartamento.Value.ToString()), int.Parse (cboTipoDocumento.Value.ToString ()), cboOrden.Value.ToString());

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
                        UltraMessageBox.Show("Ocurrió un Error al realizar  Reporte de Ventas por Cliente (Mensual) ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show("Ocurrió un Error al realizar  Reporte de Ventas por Cliente (Mensual) ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }


                var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();
                DataSet ds1 = new DataSet();

                DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
                //DataTable dt2 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate2);
                if (Export)
                {
                    aptitudeCertificate = aptitudeCertificate.OrderBy(l => l.Grupo).ToList();
                    dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
                    #region Headers

                    var columnas = new[]
                    {
                        "CodigoCliente", "NombreCliente","Departamento","MesEnero","MesFebrero","MesMarzo","MesAbril","MesMayo","MesJunio","MesJulio","MesAgosto","MesSetiembre","MesOctubre","MesNoviembre","MesDiciembre","Total"
                    };


                    var heads = new[]
                    {
                        new ExcelHeader{
                            Title = "", 
                            Headers = new ExcelHeader[]
                            {
                                 "CÓDIGO CLIENTE", "CLIENTE","DEPARTAMENTO" ,"ENERO","FEBRERO","MARZO","ABRIL","MAYO","JUNIO","JULIO","AGOSTO","SETIEMBRE","OCTUBRE","NOVIEMBRE","DICIEMBRE","TOTAL"
                            }
                        },
                       

                    };

                    #endregion

                    var excel = new ExcelReport(dt) { Headers = heads };
                    excel.AutoSizeColumns(1, 20, 50,40, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 35);
                    excel.SetTitle("VENTAS POR CLIENTE MENSUAL");

                    excel[2].Cells[4].Value = "DEL " + dtpFechaRegistroDe.Text + " AL " + dtpFechaRegistroAl.Text;
                    excel.SetHeaders();
                    var last = new int[0];
                    var group = 0;
                    //excel.EndSection += excel_EndSection;

                    //if (cboAgrupar.Text != "SIN AGRUPAR")
                    //{
                    //    excel.EndSectionGroup += (sender, e) =>
                    //    {
                    //        var obj = (ExcelReport)sender;
                    //        if (e.EndSections.Length == 0) return;
                    //        if (group == 0)
                    //        {
                    //            obj.SetFormulas(2, "TOTAL FINAL", "=" + string.Join("+", e.EndSections.Select(i => "D" + i).ToArray()), "=" + string.Join("+", e.EndSections.Select(i => "E" + i).ToArray()), "=" + string.Join("+", e.EndSections.Select(i => "F" + i).ToArray()), "=" + string.Join("+", e.EndSections.Select(i => "G" + i).ToArray()), "=" + string.Join("+", e.EndSections.Select(i => "H" + i).ToArray()), "=" + string.Join("+", e.EndSections.Select(i => "I" + i).ToArray()), "=" + string.Join("+", e.EndSections.Select(i => "J" + i).ToArray()), "=" + string.Join("+", e.EndSections.Select(i => "K" + i).ToArray()), "=" + string.Join("+", e.EndSections.Select(i => "L" + i).ToArray()), "=" + string.Join("+", e.EndSections.Select(i => "M" + i).ToArray()), "=" + string.Join("+", e.EndSections.Select(i => "N" + i).ToArray()), "=" + string.Join("+", e.EndSections.Select(i => "O" + i).ToArray()), "=" + string.Join("+", e.EndSections.Select(i => "P" + i).ToArray()));
                    //            obj.CurrentPosition++;
                    //        }
                    //        last = e.EndSections;
                    //        group++;
                    //    };
                    //}



                    var filtros = new[] { "Grupo" };
                    excel.SetData(ref objOperationResult ,columnas, filtros);
                    // InsertTable(excel, last);
                    var path = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + @".xlsx");
                    excel.Generate(path);
                    System.Diagnostics.Process.Start(path);


                }
                else
                {


                    ds1.Tables.Add(dt);
                    ds1.Tables[0].TableName = "dsReporteVentaClienteMensual";
                    rp.SetDataSource(ds1);
                    rp.SetParameterValue("MonedaReporte", "MONEDA : " + cboMoneda.Text.Trim());
                    rp.SetParameterValue("Fecha", "DEL " + dtpFechaRegistroDe.Text + " AL " + dtpFechaRegistroAl.Text);
                    rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count());
                    rp.SetParameterValue("NombreEmpresa", aptitudeCertificate2.FirstOrDefault().NombreEmpresaPropietaria.Trim());
                    rp.SetParameterValue("RucEmpresa", "R.U.C. : " + aptitudeCertificate2.FirstOrDefault().RucEmpresaPropietaria.Trim());
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
            obj.SetFormulas(2, cboAgrupar.Text == "SIN AGRUPAR" ? "TOTAL POR MES : " : cboAgrupar.Text == "VENDEDOR" ? "TOTAL POR VENDEDOR" : "TOTAL POR LINEA", string.Format("=SUM(D{0}:D{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(E{0}:E{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(F{0}:F{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(G{0}:G{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(H{0}:H{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(I{0}:I{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(J{0}:J{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(K{0}:K{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(L{0}:L{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(M{0}:M{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(N{0}:N{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(O{0}:O{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(P{0}:P{1})", e.StartPosition + 1, e.EndPosition));
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

                    CargarReporte(Export);

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




        private void TxtCodigo_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

            OperationResult objOperationResult = new OperationResult();
            Mantenimientos.frmBuscarCliente frm = new Mantenimientos.frmBuscarCliente("VV", TxtCodigoCliente.Text.Trim());
            frm.ShowDialog();
            if (frm._IdCliente != null)
            {

                TxtCodigoCliente.Text = frm._CodigoCliente;
                IdCliente = frm._IdCliente;
            }
            else
            {
                IdCliente = "";
            }

        }

        private void txtCodigoProducto_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            Mantenimientos.frmBuscarProducto frm = new Mantenimientos.frmBuscarProducto(-1, "REPORTE", null, null);
            frm.ShowDialog();

            if (frm._IdProducto != null)
            {
                //  txtCodigoProducto.Text = frm._CodigoInternoProducto.Trim();
            }
            else
            {

                // txtCodigoProducto.Clear();
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

      

        private void TxtCodigoCliente_Validating(object sender, CancelEventArgs e)
        {
            IdCliente = string.IsNullOrEmpty(TxtCodigoCliente.Text.Trim()) ? "" : IdCliente;
        }

        private void cboPais_ValueChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();

            if (cboPais.Value == null) return;

            if ((string)cboPais.Value == "-1")
            {
                Utils.Windows.LoadUltraComboEditorList(cboDepartamento, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);
            }
            else
            {
                Utils.Windows.LoadUltraComboEditorList(cboDepartamento, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, int.Parse(cboPais.Value.ToString()), 112, ""), DropDownListAction.Select);
            }
            cboDepartamento.Value = "-1";
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            Reporte(true);
        }

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {

        }

       






    }
}
