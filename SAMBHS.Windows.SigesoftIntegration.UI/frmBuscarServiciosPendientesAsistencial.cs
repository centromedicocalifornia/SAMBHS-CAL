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
    public partial class frmBuscarServiciosPendientesAsistencial : Form
    {
        public BindingList<ventadetalleDto> ListadoVentaDetalle = new BindingList<ventadetalleDto>();
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
        public frmBuscarServiciosPendientesAsistencial(string ruc, string nombreEmpresa, TipoFacturacion tipoFacturacion, string dni)
        {
            InitializeComponent();
            _tipoFacturacion = tipoFacturacion;
            Text = "Servicios pendientes de: " + nombreEmpresa;
            txtDni.Text = dni;
        }

        private void frmBuscarServiciosPendientes_Load(object sender, EventArgs e)
        {
                btnBuscar.Click += btnBuscar_Click;
        }

        void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                int Tipofact = -1;
                if (rbMedicoTratante.Checked)
                {
                    Tipofact = 1;
                }
                else if (rbPaciente.Checked)
                {
                    Tipofact = 2;
                }

                var ds = FacturacionServiciosBl.ObtenerServiciosPendientes(_tipoFacturacion, dtpF1.Value, dtpF2.Value, txtDni.Text.Trim(), null, -1, null, -1, Tipofact,-1,txtDni.Text.Trim());
                ultraGrid1.DataSource = ds;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                var ids = ultraGrid1.Rows.Where(c => Convert.ToBoolean(c.Cells["Seleccionar"].Value.ToString()))
                    .Select(p => p.Cells["ServiceId"].Value.ToString()).ToArray();
                pEspere.Visible = true;
                ultraGroupBox1.Enabled = false;
                ultraGroupBox2.Enabled = false;
                cboAceptar.Enabled = false;


                Task.Factory.StartNew(() =>
                {
                    ListadoVentaDetalle = FacturacionServiciosBl.ObtenerDetalleVenta(ids,_tipoFacturacion,0,"","");
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnBuscar_Click_1(object sender, EventArgs e)
        {

        }

    }
}
