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
    public partial class frmRegistroPedidoPendientes : Form
    {
        VendedorBL _objVendedorBL = new VendedorBL();
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        List<GridKeyValueDTO> _ListadoComboDocumentos = new List<GridKeyValueDTO>();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private  string IdCliente="";
        string _strFilterExpression;
        public frmRegistroPedidoPendientes(string Periodo)
        {
            InitializeComponent();
        }
        #region Cargar Load
        private void frmRegistroPedido_Load(object sender, EventArgs e)
        {
            #region Declaraciones / Referencias
            DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
            #endregion

            this.BackColor = new GlobalFormColors().FormColor;
            int Mes = int.Parse(DateTime.Today.Month.ToString());
            OperationResult objOperationResult = new OperationResult();
            dtpFechaRegistroDe.MinDate = DateTime.Parse((Globals.ClientSession.i_Periodo.ToString() + "/" + 01 + "/01").ToString());
            dtpFechaRegistroDe.MaxDate = DateTime.Parse((Globals.ClientSession.i_Periodo.ToString() + "/" + 12 + "/" + (DateTime.DaysInMonth(int.Parse(Globals.ClientSession.i_Periodo.ToString()), Mes)).ToString()).ToString());
            dtpFechaRegistroAl.MinDate = DateTime.Parse((Globals.ClientSession.i_Periodo.ToString() + "/" + 01 + "/01").ToString());
            dtpFechaRegistroAl.MaxDate = DateTime.Parse((Globals.ClientSession.i_Periodo.ToString() + "/" + 12 + "/" + (DateTime.DaysInMonth(int.Parse(Globals.ClientSession.i_Periodo.ToString()), Mes)).ToString()).ToString());
            Utils.Windows.LoadUltraComboEditorList(cboVendedor, "Value1", "Id", _objVendedorBL.ObtenerListadoVendedorParaCombo(ref objOperationResult, null), DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboEstadoPedido, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 43, null), DropDownListAction.All);
            cboMoneda.Value = Globals.ClientSession.i_IdMoneda.ToString();
            cboVendedor.Value = "-1";
            cboEstadoPedido.Value = "-1";
            cboAgrupar.SelectedIndex =Globals.ClientSession.v_RucEmpresa ==Constants.RucCMR ? 2: 0;
            cboFormato.Value = "2";
            //_ListadoComboDocumentos = _objDocumentoBL.ObtenDocumentosPedidosParaComboGrid(ref objOperationResult);
            //Utils.Windows.LoadUltraComboList(cboDetalleDocumento, "Value2", "Id", _ListadoComboDocumentos, DropDownListAction.Select);
        }
        #endregion

        #region Controles Botones
        private void BtnImprimir_Click(object sender, EventArgs e)
        {
            Visualizar(false);

        }

        private void Visualizar(bool Excel)
        {

            try
            {
                if (uvDatos.Validate(true, false).IsValid)
                {
                    CargarReporte(Excel);

                }
                else
                {
                    UltraMessageBox.Show("Por favor llene los campos requeridos para visualizar el reporte", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }


            }
            catch
            {

                UltraMessageBox.Show("Se produjo un error en el reporte", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        
        }
        #endregion
        #region Cargar Reporte
        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando..."
                : @"Reporte de  Pedidos Pendientes";
            pBuscando.Visible = estado;
            BtnImprimir.Enabled = !estado;


        }
        private void CargarReporte(bool Excel)
        {
            var rp = new Reportes.Ventas.crRegistroPedidoPendientes();
            var Empresa = new NodeBL().ReporteEmpresa();
            OcultarMostrarBuscar(true);
            Cursor.Current = Cursors.WaitCursor;
            OperationResult objOperationResult = new OperationResult();

            List<ReporteRegistroPedidoPendientes> aptitudeCertificate = new List<ReporteRegistroPedidoPendientes>();
         
            Task.Factory.StartNew

                (() => aptitudeCertificate =
                new PedidoBL().ReportePedidosFaltantes(ref objOperationResult, DateTime.Parse(dtpFechaRegistroDe.Text), DateTime.Parse(dtpFechaRegistroAl.Text + " 23:59"), int.Parse(cboEstadoPedido.Value.ToString()), int.Parse(cboMoneda.Value.ToString()),cboVendedor.Value.ToString (),IdCliente , txtSeriePedido.Text.Trim (),txtCorrelativoPedido.Text.Trim (),txtProducto.Text.Trim (),cboAgrupar.Value.ToString (),int.Parse ( cboFormato.Value.ToString ()))

      , _cts.Token)
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

               DataSet ds1 = new DataSet();
               

               if (Excel)
               {
                   aptitudeCertificate = aptitudeCertificate.OrderBy(o => o.Grupo).ThenBy (o=>o.NroPedido).ThenBy(x => x.Producto).ToList();
                   DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
                   #region Headers
                   var columnas = new[]
                    {
                        "NroPedido", "FechaPedido", "CondicionPago", "Producto", "Cliente","ZonaCliente",
                        "CantidadPedido", "UnidadMedidaPedido", "PrecioUnitario", "Total", "MonedaOp","TipoDocumento","NroFactura","FechaEmisionDocVenta","CantidadFactura","UnidadMedidaVenta","Saldo","UnidadMedidaPedido"
                    };
                   var heads = new ExcelHeader[]
                    {
                        "PEDIDO", "FECHA PEDIDO", "CONDICIÓN PAGO", "PRODUCTO", "CLIENTE","ZONA", "CANTIDAD PEDIDO", "", "PRECIO UNTARIO", "TOTAL", "M" ,"TIPO DOC","NRO. FACTURA","FECHA DOC.","CANTIDAD DOC.","","SALDO",""
                    };
                   #endregion

                   var objexcel = new ExcelReport(dt) { Headers = heads };
                   objexcel.AutoSizeColumns(1, 20, 12, 12, 50, 50,15, 15, 5, 15, 15, 5,5,15,12,15,5,15,5);
                   objexcel.SetTitle( cboEstadoPedido.Value .ToString ()=="-1"? "REPORTE DE PEDIDOS": cboEstadoPedido.Value.ToString ()=="0"? "REPORTE DE PEDIDOS PENDIENTES" :"REPORTE DE PEDIDOS DESPACHADOS");
                   objexcel.SetHeaders();
                   //objexcel.EndSection += (_, e) =>
                   //{
                   //    var obj = (ExcelReport)_;
                   //    obj.SetFormulas(6, "SUB TOTAL:", Enumerable.Range(0, 3).Select(i => string.Format("=SUM(${2}{0}:${2}{1})", e.StartPosition + 1, e.EndPosition, (char)('H' + i))).ToArray());
                   //    obj.CurrentPosition++;
                   //};
                   var filtros = new[] { "Grupo" };
                   objexcel.SetData(ref objOperationResult ,columnas, filtros);
                   var path = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + @".xlsx");
                   objexcel.Generate(path);
                   System.Diagnostics.Process.Start(path);
               }
               else
               {
                   DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
                   ds1.Tables.Add(dt);
                   //ds1.Tables[0].TableName = "dsEmpresa";
                   ds1.Tables[0].TableName = "dsReporteRegistroPedidoPendientes";
                   rp.SetDataSource(ds1);

                   rp.SetParameterValue("NombreEmpresa", Empresa.FirstOrDefault().NombreEmpresaPropietaria);
                   rp.SetParameterValue("RucEmpresa","R.U.C. : "+  Empresa.FirstOrDefault().RucEmpresaPropietaria);
                   rp.SetParameterValue("FechaImpresion", "DEL " + dtpFechaRegistroDe.Text + " AL " + dtpFechaRegistroAl.Text);
                   rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count());
                   rp.SetParameterValue("MonedaReporte", "MONEDA DEL REPORTE : " + cboMoneda.Text);
                   rp.SetParameterValue("EstadoPedido", int.Parse(cboEstadoPedido.Value.ToString()));
                   rp.SetParameterValue("LetrasSaldoProducto", "SALDO POR PRODUCTO : ");
                   rp.SetParameterValue("LetrasTotales", "TOTALES : ");
                   rp.SetParameterValue("Titulo",cboEstadoPedido.Value .ToString ()=="-1"? "REPORTE DE PEDIDOS": cboEstadoPedido.Value.ToString ()=="0"? "REPORTE DE PEDIDOS PENDIENTES" :"REPORTE DE PEDIDOS DESPACHADOS");
                   crystalReportViewer1.ReportSource = rp;
                   crystalReportViewer1.Show();

                   crystalReportViewer1.Zoom(110);
               }
           }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        #endregion

        private void TxtCodigoCliente_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
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

        private void TxtCodigoCliente_Validating(object sender, CancelEventArgs e)
        {
            IdCliente = string.IsNullOrEmpty(TxtCodigoCliente.Text.Trim()) ? "" : IdCliente;
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            Visualizar(true);
        }

        private void txtSeriePedido_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtSeriePedido, "{0:0000}");
        }

        private void txtCorrelativoPedido_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtCorrelativoPedido, "{0:00000000}");
        }

        private void txtProducto_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key != "btnBuscarArticulo") return;
            using (var frm = new Mantenimientos.frmBuscarProducto(-1, "REPORTE", null, null))
            {
                frm.ShowDialog();
                txtProducto.Text = frm._IdProducto != null ? frm._CodigoInternoProducto.Trim() : string.Empty;
            }
        }


    }
}
