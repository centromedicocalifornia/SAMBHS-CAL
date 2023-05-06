using SAMBHS.Common.BE.Custom;
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
    public partial class frmConfigurarComprobantes : Form
    {
        string Comprobante;
        string cadenaImprimir = "";
        int estado = 0;

        public frmConfigurarComprobantes(string _Comprobante)
        {
            Comprobante = _Comprobante;
            InitializeComponent();
        }

        private void frmConfigurarComprobantes_Load(object sender, EventArgs e)
        {
            ConexionSigesoft conectasam = new ConexionSigesoft();
            conectasam.opensigesoft();
            var cadena1 =
                "select ISNULL(i_EstadoSunat,0), ISNULL(v_EnlaceEnvio, 'https://www.pse.pe/cpe/') as v_ComprobantePago from [20505310072].dbo.venta " +
                "  where v_IdVenta ='" + Comprobante + "' ";
            SqlCommand comando = new SqlCommand(cadena1, connection: conectasam.conectarsigesoft);
            SqlDataReader lector = comando.ExecuteReader();
            float total = 0;
            while (lector.Read())
            {
                estado = int.Parse(lector.GetValue(0).ToString());
                cadenaImprimir = lector.GetValue(1).ToString();
            }
            lblPaciente.Text = Comprobante;
            if (estado == 0)
            {
                rbNoFacturado.Checked = true;
            }
            else
            {
                rbFacturado.Checked = true;
            }

            txtComprobante.Text = cadenaImprimir.Trim();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {

        }

        private void btnGuardar_Click_1(object sender, EventArgs e)
        {
            var DialogResult =
                        MessageBox.Show("¿Desea actualizar datos de facturación?", "INFORMACIÓN!", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (DialogResult == DialogResult.Yes)
            {
                int fact = 0;
                string comprobanteG = "NULL";
                if (rbFacturado.Checked == true)
                    fact = 9;
                else
                    fact = 0;

                if (txtComprobante.Text == "- - -" || txtComprobante.Text == "https://www.pse.pe/cpe/" || string.IsNullOrEmpty(txtComprobante.Text))
                {
                    comprobanteG = "NULL";
                }
                else
                {
                    comprobanteG = "'" + txtComprobante.Text.Trim() + "'";
                }

                AgendaBl.UpdateComprobante_Estado(fact, comprobanteG, Comprobante);

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
