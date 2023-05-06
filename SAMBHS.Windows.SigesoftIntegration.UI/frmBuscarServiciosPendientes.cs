using Infragistics.Win.UltraWinGrid;
using SAMBHS.Common.BE;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    public partial class frmBuscarServiciosPendientes : Form
    {
        private string _organizationId;
        public string _nroLiquidacion;
        public BindingList<ventadetalleDto> ListadoVentaDetalle = new BindingList<ventadetalleDto>();
        public int Tipofact { get; set; }
        private string _location;
        public List<string> ServiciosSeleccionados
        {
            get
            {
                try
                {                    
                    var filas = ultraGrid1.Rows.Where(c => Convert.ToBoolean(c.Cells["Seleccionar"].Value.ToString())).Select(p => p.Cells["ServiceId"].Value.ToString());
                    return filas.ToList();
                }
                catch (Exception)
                {                    
                    throw;
                }
            }
        }

        TipoFacturacion _tipoFacturacion;
        public frmBuscarServiciosPendientes(TipoFacturacion tipoFacturacion, string ruc, string nombreEmpresa)
        {
            InitializeComponent();
            ObtenerOrganizationId(ruc);
            _tipoFacturacion = tipoFacturacion;
            Text = "Servicios pendientes de: " + nombreEmpresa;
        }

        private void frmBuscarServiciosPendientes_Load(object sender, EventArgs e)
        {
            if (CargarLocacionesCombos())
            {
                btnBuscar.Click += btnBuscar_Click;
            }
        }

        void btnBuscar_Click(object sender, EventArgs e)
        {
            if (cboTipoServicio.SelectedValue.ToString() == "2" && cboTipoServicio.SelectedValue.ToString() == "2")
            {
                if (cboServicio.SelectedValue.ToString() == "-1" || txtDni.Text.Trim() == "")
                {
                    MessageBox.Show(@"Seleccione Servicio y/o digite un DNI", @"Información");
                    return;
                }
            }
          

            try
            {
                var locationId = _location;
                var esoId = int.Parse(cboEso.Value.ToString());
                var gesoId = cboGeso.Value.ToString();
                var servico = int.Parse(cboServicio.SelectedValue.ToString());
                var cargo = rbMedicoTratante.Checked == true ? 1 : 2;
                if (rbMedicoTratante.Checked)
                {
                    Tipofact = 1;
                }
                else if(rbPaciente.Checked)
                {
                    Tipofact = 2;
                }
                var tipoServicio = int.Parse(cboServicio.SelectedValue.ToString());
                var ds = FacturacionServiciosBl.ObtenerServiciosPendientes(_tipoFacturacion, dtpF1.Value, dtpF2.Value.AddDays(1), _organizationId, locationId, esoId, gesoId, servico, cargo, tipoServicio, txtDni.Text, Tipofact, txtNroLiq.Text);
                ultraGrid1.DataSource = ds;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool ObtenerOrganizationId(string ruc)
        {
            try
            {
                _organizationId = FacturacionServiciosBl.ObtenerOrganizationIdByRuc(ruc);
                var notok = string.IsNullOrEmpty(_organizationId);
                lblAdvertencia.Visible = notok;
                ultraGroupBox1.Enabled = !notok;
                return !string.IsNullOrEmpty(_organizationId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private bool CargarLocacionesCombos()
        {
            try
            {
                if (!string.IsNullOrEmpty(_organizationId))
                {
                    cboTipoServicio.DataSource = new AgendaBl().LlenarComboTipoServicio(cboTipoServicio);
                    cboTipoServicio.SelectedIndex = 0;
                    var ds = FacturacionServiciosBl.GetOrganizationId(_organizationId);
                    _location = FacturacionServiciosBl.GetLocationsByOrganizationId(_organizationId)[0].LocationId;
                    if (ds != null && ds.Any())
                    {
                        cboEmpresa.DataSource = ds;
                        cboEmpresa.DisplayMember = "Value1";
                        cboEmpresa.ValueMember = "Id";
                        cboEmpresa.Enabled = ds.Count > 1;
                        if (ds.Count <= 0) return false;

                        cboEmpresa.ValueChanged += delegate
                        {
                            FacturacionServiciosBl.LlenarComboGeso(cboGeso, _organizationId, _location);
                        };

                        cboEmpresa.SelectedIndex = 0;

                        FacturacionServiciosBl.LlenarComboEso(cboEso);

                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private void ultraGrid1_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            lblContador.Text = string.Format("{0} Registros", ((UltraGrid)sender).Rows.Count());
        }

        private void cboAceptar_Click(object sender, EventArgs e)
        {
            try
            {

                var frm = new  frmElegirTipoDetalle();

                frm.ShowDialog();

                if (frm.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    var modoDetalle = frm._tipo;
                    if (cboServicio.SelectedValue.ToString() == "2")
                    {
                        _tipoFacturacion = TipoFacturacion.Ocupacional;
                    }
                    else if (cboServicio.SelectedValue.ToString() == "3")
                    {
                        _tipoFacturacion = TipoFacturacion.Asistencial;
                    }
                    else if (cboServicio.SelectedValue.ToString() == "19")
                    {
                        _tipoFacturacion = TipoFacturacion.Hospitalizacion;
                    }

                    var ids = ultraGrid1.Rows.Where(c => Convert.ToBoolean(c.Cells["Seleccionar"].Value.ToString()))
                        .Select(p => p.Cells["ServiceId"].Value.ToString()).ToArray();

                    var totales = ultraGrid1.Rows.Where(c => Convert.ToBoolean(c.Cells["Seleccionar"].Value.ToString()))
                        .Select(p => p.Cells["Total"].Value.ToString()).ToArray();

                    decimal totalhospi = 0;
                    foreach (var total in totales)
                    {
                        totalhospi += decimal.Parse(total);
                    }

                    pEspere.Visible = true;
                    ultraGroupBox1.Enabled = false;
                    ultraGroupBox2.Enabled = false;
                    cboAceptar.Enabled = false;
                    _nroLiquidacion = txtNroLiq.Text;
                    Task.Factory.StartNew(() =>
                    {
                        ListadoVentaDetalle = FacturacionServiciosBl.ObtenerDetalleVenta(ids, _tipoFacturacion, totalhospi, txtNroLiq.Text, modoDetalle);
                    }).ContinueWith(t =>
                    {
                        if (ListadoVentaDetalle.Any())
                        {
                            DialogResult = System.Windows.Forms.DialogResult.OK;
                        }
                        ultraGroupBox1.Enabled = true;
                        ultraGroupBox2.Enabled = true;
                        pEspere.Visible = false;
                        cboAceptar.Enabled = true;
                        Close();
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cboTipoServicio_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboTipoServicio.SelectedIndex == 0 || cboTipoServicio.SelectedIndex == -1)
            {
                cboServicio.DataSource = new AgendaBl().LlenarComboServicio(cboServicio, 1000);
                cboServicio.SelectedIndex = 0;
            }
            else
            {
                cboServicio.DataSource = new AgendaBl().LlenarComboServicio(cboServicio, int.Parse(cboTipoServicio.SelectedValue.ToString()));
                cboServicio.SelectedIndex = 0;
            }
        }
        private void frmBuscarServiciosPendientesAutocomplete(object sender, EventArgs e)
        {
          
            //if (txtNroLiq.TextLength == 1)
            //    txtNroLiq.Text = "N009-LQ00000000"+txtNroLiq.Text;
            //else if (txtNroLiq.TextLength == 2)
            //{
            //    txtNroLiq.Text = "N009-LQ0000000" + txtNroLiq.Text;
            //}
            //else if (txtNroLiq.TextLength == 3)
            //{
            //    txtNroLiq.Text = "N009-LQ000000" + txtNroLiq.Text;
            //}
            //else if (txtNroLiq.TextLength == 4)
            //{
            //    txtNroLiq.Text = "N009-LQ000000" + txtNroLiq.Text;
            //}
        }

        private void frmBuscarServiciosPendientesAutocomplete()
        {

        }

        private void txtNroLiq_Leave(object sender, EventArgs e)
        {
            int value = 0;
            if (int.TryParse(txtNroLiq.Text, out value))
            {
                var nro = int.Parse(txtNroLiq.Text);
                txtNroLiq.Text = string.Format("N{0}-{1}", "009", nro.ToString("000000000"));
            }
            else
            {
                
            }

           

           
        }

        private void btnBuscar_Click_1(object sender, EventArgs e)
        {

        }
    }
}
