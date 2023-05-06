using SAMBHS.Windows.SigesoftIntegration.UI.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    public partial class frmAddSolicitudCarta : Form
    {
        string _service;
        public frmAddSolicitudCarta(string service)
        {
            _service = service;
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGrabar_Click(object sender, EventArgs e)
        {
            string nroCarta = txtNroCartaSolicitud.Text;
            string result = VerificarRegistro();
            RegistrarCarta(_service, nroCarta);
            result = VerificarRegistro();
            if (result == "")
            {
                MessageBox.Show("NO SE PRODUJO NINGÚN REGISTRO", "Validación!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                MessageBox.Show("Se registro correctamente", "Información!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            this.Close();
        }

        private void RegistrarCarta(string _service, string nroCarta)
        {
            #region Conexion SAM
            ConexionSigesoft conectasam = new ConexionSigesoft();
            conectasam.opensigesoft();
            #endregion
            var cadena1 = "update service set v_NroCartaSolicitud ='" + nroCarta + "' where v_ServiceId='" + _service + "'";
            SqlCommand comando = new SqlCommand(cadena1, connection: conectasam.conectarsigesoft);
            SqlDataReader lector = comando.ExecuteReader();
            lector.Close();
        }

        private string VerificarRegistro()
        {
            #region Conexion SAM
            ConexionSigesoft conectasam = new ConexionSigesoft();
            conectasam.opensigesoft();
            #endregion
            var cadena1 = "select v_NroCartaSolicitud from service where v_ServiceId='" + _service + "'";
            SqlCommand comando = new SqlCommand(cadena1, connection: conectasam.conectarsigesoft);
            SqlDataReader lector = comando.ExecuteReader();
            string confirm = "";
            while (lector.Read())
            {
                confirm = lector.GetValue(0).ToString();
            }
            lector.Close();
            return confirm;
        }

        private void frmAddSolicitudCarta_Load(object sender, EventArgs e)
        {
            string result = VerificarRegistro();
            if (result == "")
            {
                lblRegistrado.Text = "Registrar:";
            }
            else
            {
                lblRegistrado.Text = "Ya se encuentra registrado un Nro:";
                txtNroCartaSolicitud.Text = result;
                btnGrabar.Text = "Editar";
            }
        }


    }
}
