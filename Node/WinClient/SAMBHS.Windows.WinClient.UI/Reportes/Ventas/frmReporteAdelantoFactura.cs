using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using SAMBHS.Cobranza.BL;
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    public partial class frmReporteAdelantoFactura : Form
    {
        OperationResult _objOperationResult = new OperationResult();
        private string v_IdCliente;
        public frmReporteAdelantoFactura(string N)
        {
            InitializeComponent();
        }

        private void frmReporteAdelantoFactura_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            var rp = new crReporteAdelantoFactura();
            var ds = new AdelantoBL().ObtenerReporteAdelantoFacturas(ref _objOperationResult, dtpFechaInicio.Value, dtpFechaFin.Value, v_IdCliente);
            var empresa = new NodeBL().ReporteEmpresa().FirstOrDefault();
            if (ds == null) return;
            var ds1 = new DataSet();
            var dt = Utils.ConvertToDatatable(ds.ToList());
            ds1.Tables.Add(dt);
            ds1.Tables[0].TableName = "dsReporteAdelantoFactura";
            rp.SetDataSource(ds1);
            rp.SetParameterValue("_Empresa", empresa.NombreEmpresaPropietaria);
            rp.SetParameterValue("_RUC", empresa.RucEmpresaPropietaria);
            rp.SetParameterValue("_FIni", dtpFechaInicio.Value.ToShortDateString());
            rp.SetParameterValue("_FFIn", dtpFechaFin.Value.ToShortDateString());
            crystalReportViewer1.ReportSource = rp;
        }

        private void txtCliente_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            Mantenimientos.frmBuscarCliente frm = new Mantenimientos.frmBuscarCliente("VV", txtCliente.Text.Trim());
            frm.ShowDialog();
            if (frm._IdCliente != null)
            {
                txtCliente.Text = frm._NroDocumento + @" - " + frm._RazonSocial;
                v_IdCliente = frm._IdCliente;
            }
        }
    }
}
