
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
    public partial class frmReporteCostoComercializacion : Form
    {
        EstablecimientoBL objEstablecimientoBL = new EstablecimientoBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        CancellationTokenSource _cts = new CancellationTokenSource();
        public frmReporteCostoComercializacion(string N)
        {
            InitializeComponent();
        }

        private void frmReporteCostoComercializacion_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            CargarCombos();
            ValidarFechas();
        }

        public void CargarCombos()
        {

            OperationResult objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            cboMoneda.Value = "-1";
            cboMoneda.Value = Globals.ClientSession.i_IdMonedaCompra.ToString();
        }



        private void ValidarFechas()
        {
            string Periodo = Globals.ClientSession.i_Periodo.ToString();
            string Mes = DateTime.Today.Month.ToString();
            if (DateTime.Now.Year.ToString().Trim() == Periodo)
            {
                dtpFechaRegistroDe.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/" + "01").ToString());
                dtpFechaRegistroDe.MaxDate = DateTime.Parse((Periodo + "/12/31"));
                dtpFechaRegistroDe.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                dtpFechaRegistroAl.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/" + "01").ToString());
                dtpFechaRegistroAl.MaxDate = DateTime.Parse((Periodo + "/12/31"));
                dtpFechaRegistroAl.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());

            }
            else
            {
                dtpFechaRegistroDe.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/01").ToString());
                dtpFechaRegistroDe.MaxDate = DateTime.Parse((Periodo + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(Periodo), 12)).ToString()).ToString());
                dtpFechaRegistroDe.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                dtpFechaRegistroAl.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/01").ToString());
                dtpFechaRegistroAl.MaxDate = DateTime.Parse((Periodo + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(Periodo), 12)).ToString()).ToString());
                dtpFechaRegistroAl.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());

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
            List<ReporteCostoComercializacion> Reporte = new List<ReporteCostoComercializacion>();
            OperationResult objOperationResult = new OperationResult();
            OcultarMostrarBuscar(true);
            Cursor.Current = Cursors.WaitCursor;
            var rp = new Reportes.Ventas.crReporteCostoComercializacion();
        
            Task.Factory.StartNew(() =>
            {
               
                Reporte = new VentaBL().ReporteCostoComercializacion(ref objOperationResult, DateTime.Parse(dtpFechaRegistroDe.Text), DateTime.Parse(dtpFechaRegistroAl.Text + " 23:59"), int.Parse(cboMoneda.Value.ToString()),txtCuentaDiferente.Text.Trim (),decimal.Parse (txtFactor.Text.Trim ()));
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
                        UltraMessageBox.Show("Ocurrió un Error en Reporte de Costo de Comercialización", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show("Ocurrió un Error en Reporte de Costo de Comercialización", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }

                //DataTable dt = new DataTable();
                

                var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();
                DataSet ds1 = new DataSet();
                DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(Reporte);
                //DataTable dt2 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(Reporte);

                ds1.Tables.Add(dt);
               // ds1.Tables.Add(dt2);
                ds1.Tables[0].TableName = "dsReporteComercializacion";
               // ds1.Tables[1].TableName = "dsReporteCostoUtilidadDetalladoDetalles";

                rp.SetDataSource(ds1);
                //rp.SetDataSource(ds1);
                rp.SetParameterValue("NombreEmpresa", aptitudeCertificate2.FirstOrDefault().NombreEmpresaPropietaria.Trim());
                rp.SetParameterValue("RucEmpresa", "R.U.C. : " + aptitudeCertificate2.FirstOrDefault().RucEmpresaPropietaria.Trim());
                rp.SetParameterValue("NroRegistros", Reporte.Count ());
                rp.SetParameterValue("MonedaReporte", "MONEDA REPORTE : " + cboMoneda.Text);
                rp.SetParameterValue("FechaReporte", "DE :" + dtpFechaRegistroDe.Text + " " + " AL " + dtpFechaRegistroAl.Text);
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

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {

        }

        //private void txtPedidoInicio_TextChanged(object sender, EventArgs e)
        //{
        //    txtPedidoFin.Text = txtPedidoInicio.Text.Trim();
        //}

        //private void txtPedidoFin_Validated(object sender, EventArgs e)
        //{
        //    if (txtPedidoFin.Text.Trim() == string.Empty && txtPedidoInicio.Text.Trim() != string.Empty)
        //    {
        //        txtPedidoFin.Text = txtPedidoInicio.Text.Trim();
        //    }
        //}

        //private void label5_Click(object sender, EventArgs e)
        //{

        //}

        //private void cboMoneda_ValueChanged(object sender, EventArgs e)
        //{

        //}



    }
}
