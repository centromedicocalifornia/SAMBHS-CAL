
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Venta.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    public partial class frmReporteCostoUtilidadResumen : Form
    {
        EstablecimientoBL objEstablecimientoBL = new EstablecimientoBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        CancellationTokenSource _cts = new CancellationTokenSource();
        public frmReporteCostoUtilidadResumen(string N)
        {
            InitializeComponent();
        }

        private void frmReporteCostoUtilidadDetallado_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            CargarCombos();
            ValidarFecha();
        }

        public void CargarCombos()
        {

            OperationResult objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList(cboVendedor, "Value1", "Id", Globals.CacheCombosVentaDto.cboVendedor, DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", objEstablecimientoBL.GetAlmacenesXEstablecimiento(Globals.ClientSession.i_IdEstablecimiento.Value), DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            cboVendedor.Value = "-1";
            cboMoneda.Value = "-1";
            cboAlmacen.Value = "-1";
            cboMoneda.Value = Globals.ClientSession.i_IdMoneda.ToString();
        }

        private void ValidarFecha()
        {
            lblPeriodo.Text = "Periodo : " + Globals.ClientSession.i_Periodo.ToString();
            nupMesInicio.Minimum = 1;
            nupMesInicio.Value = 1;
            nupMesFin.Minimum = nupMesInicio.Value;
            nupMesFin.Value = int.Parse(DateTime.Now.Month.ToString());

            if (Globals.ClientSession.i_Periodo.ToString() == DateTime.Now.Year.ToString())
            {
                nupMesInicio.Maximum = int.Parse(DateTime.Now.Month.ToString());
            }
            else
            {
                nupMesInicio.Maximum = 12;
            }

        }

        private void btnVisualizar_Click(object sender, EventArgs e)
        {
            if (Validar.Validate(true, false).IsValid)
            {

                CargarReporte();
            }
        }
        private void CargarReporte()
        {
            List<ReporteCostoUtilidadResumen> ReporteFinal = new List<ReporteCostoUtilidadResumen>();
            List<ReporteCostoUtilidadResumen> ReporteFiltrado = new List<ReporteCostoUtilidadResumen>();
            OperationResult objOperationResult = new OperationResult();
            OcultarMostrarBuscar(true);
            Cursor.Current = Cursors.WaitCursor;
            var rp = new Reportes.Ventas.crReporteVentasCostoUtilidadResumen();
            Task.Factory.StartNew(() =>
            {
                
                ReporteFinal = new VentaBL().ReporteCostoUtilidadResumen(ref objOperationResult, int.Parse(nupMesInicio.Value.ToString()), int.Parse(nupMesFin.Value.ToString()), cboVendedor.Value.ToString(), int.Parse(cboAlmacen.Value.ToString()), txtPedidoInicio.Text.Trim(), txtPedidoFin.Text.Trim(), int.Parse(cboMoneda.Value.ToString()),Globals.ClientSession.i_Periodo.ToString ());

               


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
                        UltraMessageBox.Show("Ocurrió un Error en Reporte Costo Utilidad sobre venta por Pedido(Resumen)", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show("Ocurrió un Error en Reporte Costo Utilidad sobre venta por Pedido(Resumen)", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }

                var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();
                DataTable dt = new DataTable();
                if (txtPedidoInicio.Text.Trim() != string.Empty && txtPedidoFin.Text.Trim() != string.Empty)
                {
                    bool DentroRango=false ;
                    ReporteFinal = ReporteFinal.OrderBy(x => x.NroPedido).ToList();

                    if (txtPedidoInicio.Text.Trim() == txtPedidoFin.Text.Trim())
                    {
                        foreach (var item in ReporteFinal)
                        {
                            if (item.NroPedido == txtPedidoInicio.Text.Trim())
                            {
                                ReporteFiltrado.Add(item);
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in ReporteFinal)
                        {
                            if (item.NroPedido == txtPedidoInicio.Text.Trim())
                            {

                                ReporteFiltrado.Add(item);
                                DentroRango = true;
                            }
                            else if (item.NroPedido == txtPedidoFin.Text.Trim())
                            {
                                ReporteFiltrado.Add(item);
                                DentroRango = false;
                            }
                            else if (DentroRango)
                            {
                                ReporteFiltrado.Add(item);

                            }
                        }
                    }

                    dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(ReporteFiltrado);
                }
                else
                {
                   dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(ReporteFinal);
                }

                DataSet ds1 = new DataSet();
                ds1.Tables.Add(dt);
            
                ds1.Tables[0].TableName = "dsReporteCostoUtilidadDetallado";
                rp.SetDataSource(ds1);
                rp.SetParameterValue("NombreEmpresa", aptitudeCertificate2.FirstOrDefault().NombreEmpresaPropietaria.Trim());
                rp.SetParameterValue("RucEmpresa", "R.U.C. : " + aptitudeCertificate2.FirstOrDefault().RucEmpresaPropietaria.Trim());
                rp.SetParameterValue("NroRegistros", txtPedidoInicio.Text.Trim() != string.Empty && txtPedidoFin.Text.Trim() != string.Empty ? ReporteFiltrado.Count() : ReporteFinal.Count ());
                rp.SetParameterValue("MonedaReporte", "MONEDA REPORTE : " + cboMoneda.Text);
                rp.SetParameterValue("FechaReporte", "DE :" + new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(int.Parse(nupMesInicio.Value.ToString())).ToUpper() + " A " + new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(int.Parse(nupMesFin.Value.ToString())).ToUpper() + " DEL " + Globals.ClientSession.i_Periodo.ToString());
                crystalReportViewer1.ReportSource = rp;
                crystalReportViewer1.Show();




            }
                , TaskScheduler.FromCurrentSynchronizationContext());

        }


        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Reporte Costo Utilidad sobre venta por Pedido( Detallado)"
                : @"Reporte Costo Utilidad sobre venta por Pedido( Detallado)";
            pBuscando.Visible = estado;
            btnVisualizar.Enabled = !estado;
        }

        private void frmReporteCostoUtilidadDetallado_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }

        private void txtPedidoInicio_TextChanged(object sender, EventArgs e)
        {
            txtPedidoFin.Text = txtPedidoInicio.Text.Trim();
        }

        private void txtPedidoFin_TextChanged(object sender, EventArgs e)
        {



        }

        private void txtPedidoFin_Validated(object sender, EventArgs e)
        {
            if (txtPedidoFin.Text.Trim() == string.Empty && txtPedidoInicio.Text.Trim() != string.Empty)
            {
                txtPedidoFin.Text = txtPedidoInicio.Text.Trim();
            }
        }



    }
}
