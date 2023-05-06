using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Almacen.BL;
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Almacen
{
    public partial class frmReporteProcesoGastosIndirectos : Form
    {
        private readonly GastosIndirectosBL _objGastosIndirectosBl;
        private bool _generarIngresos = false;
        public frmReporteProcesoGastosIndirectos(string N)
        {
            _objGastosIndirectosBl = new GastosIndirectosBL();
            InitializeComponent();
            _objGastosIndirectosBl.ErrorEvent += _objGastosIndirectosBl_ErrorEvent;
            _objGastosIndirectosBl.Terminado += _objGastosIndirectosBl_Terminado;
        }

        private void _objGastosIndirectosBl_Terminado(object sender, EventArgs e)
        {
            if (!_generarIngresos)
            {
                var empresa = new NodeBL().ReporteEmpresa().FirstOrDefault();

                {
                    var rp = new crGastosIndirectos();
                    pBuscando.Visible = false;
                    var ds1 = new DataSet();
                    var dt = Utils.ConvertToDatatable(_objGastosIndirectosBl.DataGastosIndirectosCalculado);

                    ds1.Tables.Add(dt);
                    ds1.Tables[0].TableName = "dsGastosIndirectos";
                    rp.SetDataSource(ds1);
                    rp.SetParameterValue("_RazonSocial",
                        empresa != null ? empresa.NombreEmpresaPropietaria : string.Empty);
                    rp.SetParameterValue("_RUC", empresa != null ? empresa.RucEmpresaPropietaria : string.Empty);
                    rp.SetParameterValue("_Periodo", string.Format("{0} - {1}", cboMes.Text, txtPeriodo.Text));
                    crystalReportViewer1.ReportSource = rp;
                }

                {
                    var rp = new crGastosIndirectosConsulta();
                    pBuscando.Visible = false;
                    var ds1 = new DataSet();
                    var dt = Utils.ConvertToDatatable(_objGastosIndirectosBl.DataGastosIndirectosReporte);

                    ds1.Tables.Add(dt);
                    ds1.Tables[0].TableName = "dsGastosIndirectos";
                    rp.SetDataSource(ds1);
                    rp.SetParameterValue("_RazonSocial",
                        empresa != null ? empresa.NombreEmpresaPropietaria : string.Empty);
                    rp.SetParameterValue("_RUC", empresa != null ? empresa.RucEmpresaPropietaria : string.Empty);
                    rp.SetParameterValue("_Periodo", string.Format("{0} - {1}", cboMes.Text, txtPeriodo.Text));
                    crystalReportViewer2.ReportSource = rp;
                }
            }
            else
            {
                btnGenerarIngresos.Enabled = true;
                lblEstadoProceso.Text = @"Terminado.";
                pbEstado.Image = Resource.accept;
                MessageBox.Show(@"El proceso finalizó!");
            }
        }

        private void _objGastosIndirectosBl_ErrorEvent(string error)
        {
            btnGenerarIngresos.Enabled = true;
            MessageBox.Show(error, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void frmReporteProcesoGastosIndirectos_Load(object sender, EventArgs e)
        {
            var objOperationResult = new OperationResult();
            BackColor = new GlobalFormColors().FormColor;
            txtPeriodo.Text = Globals.ClientSession.i_Periodo.ToString();
            cboMes.SelectedIndex = 0;
            Utils.Windows.LoadUltraComboEditorList(cboAlmacenDestino, "Value1", "Id", new NodeWarehouseBL().ObtenerAlmacenesParaCombo(ref objOperationResult, null, -1), DropDownListAction.Select);
            cboAlmacenDestino.SelectedIndex = 0;
        }

        private void btnProcesar_Click(object sender, EventArgs e)
        {
            _generarIngresos = false;
            pBuscando.Visible = true;
            _objGastosIndirectosBl.ComenzarAsync(int.Parse(txtPeriodo.Text), int.Parse(cboMes.Value.ToString()), _generarIngresos);
        }

        private void btnGenerarIngresos_Click(object sender, EventArgs e)
        {
            try
            {
                int idAlmacen = int.Parse(cboAlmacenDestino.Value.ToString());
                _generarIngresos = true;
                btnGenerarIngresos.Enabled = false;
                _objGastosIndirectosBl.ComenzarAsync(int.Parse(txtPeriodo.Text), int.Parse(cboMes.Value.ToString()), _generarIngresos, idAlmacen);
                lblEstadoProceso.Text = @"Procesando por favor espere...";
                pbEstado.Image = Resource.loading;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
           
        }
    }
}
