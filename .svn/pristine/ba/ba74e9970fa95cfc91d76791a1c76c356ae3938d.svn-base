using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using SAMBHS.Common.BL;
using SAMBHS.Venta.BL;
using SAMBHS.Compra.BL;
namespace SAMBHS.Windows.WinClient.UI.Reportes.Pago
{
    public partial class frmReporteEstadoCuentaProveedor : Form
    {
        ClienteBL _objClienteBL = new ClienteBL();
        public frmReporteEstadoCuentaProveedor(string pstrParametro)
        {
            InitializeComponent();
        }

        private void frmReporteEstadoCuentaProveedor_Load(object sender, EventArgs e)
        {
            cboOrden.Text = "PROVEEDOR";
        }

        private void ultraExpandableGroupBox1_ExpandedStateChanged(object sender, EventArgs e)
        {
            if (ultraExpandableGroupBox1.Expanded == true)
            {
                ultraGroupBox1.Location = new Point(ultraGroupBox1.Location.X, ultraExpandableGroupBox1.Location.Y + ultraExpandableGroupBox1.Height + 5);
                ultraGroupBox1.Height = this.Height - ultraGroupBox1.Location.Y - 7;
            }
            else
            {
                ultraGroupBox1.Location = new Point(ultraGroupBox1.Location.X, ultraExpandableGroupBox1.Location.Y + ultraExpandableGroupBox1.Height + 5);
                ultraGroupBox1.Height = this.Height - ultraGroupBox1.Location.Y - 7;
            }
        }

        private void btnVisualizar_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime FecHasta = dtpFecha.Value.Date.AddDays(-1);
                DateTime FechaSaldoHasta = DateTime.Parse(FecHasta.Date.Day.ToString("00") + "/" + FecHasta.Date.Month.ToString("00") + "/" + FecHasta.Date.Year.ToString() + " 23:59");                //                 DateTime orderDate = DateTime.Now.AddDays(-1);
                DateTime FechaHasta = DateTime.Now;
                DateTime FechaDesde = dtpFecha.Value.Date;
                if (uvValidar.Validate(true, false).IsValid)
                {
                    using (new LoadingClass.PleaseWait(this.Location, "Generando Reporte..."))
                        CargarReporte(FechaSaldoHasta, FechaHasta, FechaDesde);
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


        private void CargarReporte(DateTime FechaSaldoDeudor, DateTime FechaHasta, DateTime FechaDesde)
        {
            OperationResult objOperationResult = new OperationResult();
            var rp = new Reportes.Pago.crReporteEstaCuentaProveedor();
            var Reporte = new PagoBL().ReporteEstadoCuentaProveedor(txtProveedor.Text.Trim(), FechaSaldoDeudor, FechaDesde, FechaHasta);
            var Empresa= new NodeBL().ReporteEmpresa();
            DataSet ds1 = new DataSet();
            DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(Reporte);
            //DataTable dt2 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate2);
            ds1.Tables.Add(dt);
            //ds1.Tables.Add(dt2);
            ds1.Tables[0].TableName = "dsReporteEstadoCuentaProveedor";
           // ds1.Tables[1].TableName = "dsEmpresa";
            rp.SetDataSource(ds1);
            rp.SetParameterValue("Fecha","DEL " + FechaDesde.Date.Day.ToString("00") + "/" + FechaDesde.Date.Month.ToString("00") + "/" + FechaDesde.Date.Year.ToString() + "   AL   " + FechaHasta.Date.Day.ToString("00") + "/" + FechaHasta.Date.Month.ToString("00") + "/" + FechaHasta.Date.Year.ToString());
            rp.SetParameterValue ("CantidadDecimalPrecio",(int)Globals.ClientSession.i_PrecioDecimales);
            rp.SetParameterValue("NroRegistros", Reporte.Count());
            rp.SetParameterValue ("FechaSaldoDeudor", "SALDO DEUDOR AL   : " + FechaHasta.Date.Day.ToString("00") + "/" + FechaHasta.Date.Month.ToString("00") + "/" + FechaHasta.Date.Year.ToString());  // TextBox con el valor del Parametro)
            rp.SetParameterValue ("NombreEmpresa",Empresa.FirstOrDefault().NombreEmpresaPropietaria );
                rp.SetParameterValue ("RucEmpresa",Empresa.FirstOrDefault().RucEmpresaPropietaria );
            crystalReportViewer1.ReportSource = rp;
            crystalReportViewer1.Show();
        }

        private void txtProveedor_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscarProveedor")
            {
                OperationResult objOperationResult = new OperationResult();
                Mantenimientos.frmBuscarProveedor frm = new Mantenimientos.frmBuscarProveedor("", "RUC");
                frm.ShowDialog();
                if (frm._IdProveedor != null)
                {
                    txtRazonSocial.Text = frm._RazonSocial.Trim().ToUpper();
                    txtProveedor.Text = frm._CodigoProveedor.ToUpper();
                }
                else
                {

                }
            }
        }

        private void txtProveedor_TextChanged(object sender, EventArgs e)
        {
            if (txtProveedor.Text == string.Empty)
            {
                txtRazonSocial.Clear();
            }
        }

        private void txtProveedor_Validated(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (txtProveedor.Text.Trim() != string.Empty)
            {
                var Proveedor = _objClienteBL.ObtenerClienteCodigoBandejasCodigo(ref objOperationResult, txtProveedor.Text.Trim(), "V");
                if (Proveedor != null)
                {
                    txtRazonSocial.Text = (Proveedor.v_ApePaterno + " " + Proveedor.v_ApeMaterno.Trim() + " " + Proveedor.v_PrimerNombre.Trim() + " " + Proveedor.v_SegundoNombre.Trim() + " " + Proveedor.v_RazonSocial.Trim()).Trim().ToUpper();
                    txtProveedor.Text = Proveedor.v_CodCliente.Trim();
                }
                else
                {
                    txtRazonSocial.Clear();
                    txtProveedor.Clear();
                }

            }
            else
            {
                txtRazonSocial.Clear();
                txtProveedor.Clear();
            }
        }
    }
}
