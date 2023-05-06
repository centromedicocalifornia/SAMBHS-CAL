using SAMBHS.Windows.SigesoftIntegration.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmComprobante : Form
    {
        string servicio_;
        string paciente_;
        int facturado = 0;
        string comprobante = "";
        string liquidacion = "";
        public frmComprobante(string servicio, string paciente)
        {
            paciente_ = paciente;
            servicio_ = servicio;
            InitializeComponent();
        }

        private void frmComprobante_Load(object sender, EventArgs e)
        {
            ConexionSigesoft conectasam = new ConexionSigesoft();
            conectasam.opensigesoft();
            var cadena1 =
                "select i_IsFac, ISNULL(v_ComprobantePago, '- - -') as v_ComprobantePago, ISNULL(v_NroLiquidacion, '- - -') as v_NroLiquidacion from service " +
                " where v_ServiceId='" + servicio_ + "' ";
            SqlCommand comando = new SqlCommand(cadena1, connection: conectasam.conectarsigesoft);
            SqlDataReader lector = comando.ExecuteReader();
            float total = 0;
            while (lector.Read())
            {
                facturado = int.Parse(lector.GetValue(0).ToString());
                comprobante = lector.GetValue(1).ToString();
                liquidacion = lector.GetValue(2).ToString();
            }
            lblPaciente.Text = paciente_;
            if (facturado == 2)
            {
                rbFacturado.Checked = true;
            }
            else
            {
                rbNoFacturado.Checked = true;
            }

            txtComprobante.Text = comprobante.Trim();
            txtLiquidacion.Text = liquidacion.Trim();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            var DialogResult =
                        MessageBox.Show("¿Desea actualizar datos de facturación?","INFORMACIÓN!", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (DialogResult == DialogResult.Yes)
            {
                int fact = 0;
                string comprobanteG = "NULL";
                string liquidacionG = "NULL";
                if (rbFacturado.Checked == true)
                    fact = 2;
                else
                    fact = 1;

                if (txtComprobante.Text == "- - -" || string.IsNullOrEmpty(txtComprobante.Text))
                {
                    comprobanteG = "NULL";
                }
                else
                {
                    comprobanteG = "'" + txtComprobante.Text.Trim() + "'";
                }

                if (txtLiquidacion.Text == "- - -" || string.IsNullOrEmpty(txtLiquidacion.Text))
                {
                    liquidacionG = "NULL";
                }
                else
                {
                    liquidacionG = "'" + txtLiquidacion.Text.Trim() + "'";
                }

                AgendaBl.UpdateComprobanteNew(fact, comprobanteG, liquidacionG, servicio_);

                MessageBox.Show("GUARDADO CORRECTAMENTE", "CORRECTO", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Close();
            }
            else
            {
                this.Close();
            }
        }
    }
}
