using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using SAMBHS.Windows.SigesoftIntegration.UI.Dtos;
using System.Windows.Forms;
using Dapper;
using System.Data;
using System.Transactions;
using SAMBHS.Windows.SigesoftIntegration.UI.Reports;
using System.Data.SqlClient;
//using SAMBHS.Windows.WinClient.UI.Sigesoft;

namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    public partial class frmPreCarga : Form
    {
        private string _modo;
        private string _dni;
        private string _idEmpresa;
        private string _idContrata;
        int busqueda;
        public frmPreCarga(int busqueda_)
        {
            busqueda = busqueda_;
            InitializeComponent();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (txtDocNumber.Text == "") 
            {
                MessageBox.Show("Debe de registrar un Nro de D.N.I", "Campo Obligatorio", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                int tipoAtencion = 0;
                if (rbocupacional.Checked) { tipoAtencion = 1; }
                else if (rbparticular.Checked) { tipoAtencion = 2; }
                else if (rbseguros.Checked) { tipoAtencion = 3; }
                _modo = "BUSCAR";
                _dni = txtDocNumber.Text;
                _idEmpresa = txtIdorganization.Text;
                if (cboContrata.Text == "") { _idContrata = ""; }
                else { _idContrata = txtContrata.Text; }

                if (rbocupacional.Checked)
                {
                    var frmAgendar = new frmAgendarTrabajador(_modo, _dni, _idEmpresa, tipoAtencion, _idContrata);
                    //this.Hide();
                    frmAgendar.ShowDialog();
                    if (frmAgendar.cierre == 1)
                    {
                        this.Close();
                    }
                    
                }
                else
                {
                    frmAgendaParticular frmAgendar = new frmAgendaParticular(_modo, _dni, _idEmpresa, tipoAtencion, _idContrata, "", "", 0, "", "", "", "", "", "", DateTime.Now,"");
                    //this.Hide();
                    frmAgendar.ShowDialog();
                    if (frmAgendar.cierre == 1)
                    {
                        this.Close();
                    }
                }
            }
        }

        private void frmPreCarga_Load(object sender, EventArgs e)
        {
            cboEmpresa.DataSource = new EmpresaBl().GetOrganizationFacturacion(cboEmpresa, 9);
            cboEmpresa.SelectedIndex = 0;
            cboContrata.DataSource = new EmpresaBl().GetOrganizationFacturacion(cboContrata, 9);
            cboContrata.SelectedIndex = 0;

            lblEmpresa.Text = "EMPRESA / COMP MINERA";
            cboEmpresa.Text = "";
            cboEmpresa.Enabled = true;
            cboEmpresa.Visible = true;
            lblContrata.Visible = true;
            lblContrata.Text = "CONTRATA";
            cboContrata.Text = "";
            cboContrata.Enabled = true;
            cboContrata.Visible = true;
            
            if (busqueda == 1)
            {
                gbServiceType.Visible = true;
                btnCancel.Visible = true;
                lblSeguros.Visible = false;
            }
            else
            {
                rbseguros.Checked = true;
                gbServiceType.Visible = false;
                btnCancel.Visible = false;
                lblSeguros.Visible = true;
            }

        }

        private void cboEmpresa_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConexionSigesoft conectasam = new ConexionSigesoft();
            conectasam.opensigesoft();
            var cadena1 = "select v_OrganizationId from organization where v_Name='" + cboEmpresa.Text + "'";
            SqlCommand comando = new SqlCommand(cadena1, connection: conectasam.conectarsigesoft);
            SqlDataReader lector = comando.ExecuteReader();
            while (lector.Read())
            {
                txtIdorganization.Text = lector.GetValue(0).ToString();
            }
            lector.Close();
            conectasam.closesigesoft();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            int tipoAtencion = 0;
            if (rbocupacional.Checked) { tipoAtencion = 1; }
            else if (rbparticular.Checked) { tipoAtencion = 2; }
            else if (rbseguros.Checked) { tipoAtencion = 3; }
            _modo = "BUSCAR";
            _dni = txtDocNumber.Text;
            _idEmpresa = txtIdorganization.Text;
            int a = 1;

            if (rbocupacional.Checked)
            {
                var frmAgendar = new frmAgendarTrabajador("", "", "", tipoAtencion, "");
                frmAgendar.ShowDialog();
                this.Close();
            }
            else
            {
                //frmAgendaParticular frmAgendar = new frmAgendaParticular("", "", "", tipoAtencion, "", "", null, "");
                frmAgendaParticular frmAgendar = new frmAgendaParticular("", "", "", tipoAtencion, "", "", null, 0, "", "", "", "", "", "", DateTime.Now,"");

                frmAgendar.ShowDialog();
                this.Close();
            }
            //this.Close();
        }

        private void rbparticular_CheckedChanged(object sender, EventArgs e)
        {
            if (rbparticular.Checked)
            {
                cboEmpresa.DataSource = new EmpresaBl().GetOrganizationFacturacion(cboEmpresa, 9);
                cboEmpresa.SelectedIndex = 0;
                lblEmpresa.Text = "CLÍNICA:";
                cboEmpresa.Text = "CLINICA SAN LORENZO S.R.L.";
                cboEmpresa.Enabled = false;
                cboEmpresa.Visible = true;
                lblContrata.Visible = false;
                cboContrata.Text = "";
                cboContrata.Enabled = false;
                cboContrata.Visible = false;
            }
        }

        private void rbocupacional_CheckedChanged(object sender, EventArgs e)
        {
            if (rbocupacional.Checked)
            {
                cboEmpresa.DataSource = new EmpresaBl().GetOrganizationFacturacion(cboEmpresa, 9);
                cboEmpresa.SelectedIndex = 0;
                lblEmpresa.Text = "EMPRESA / COMP MINERA";
                cboEmpresa.Text = "";
                cboEmpresa.Enabled = true;
                cboEmpresa.Visible = true;
                lblContrata.Visible = true;
                lblContrata.Text = "CONTRATA";
                cboContrata.Text = "";
                cboContrata.Enabled = true;
                cboContrata.Visible = true;
            }
        }

        private void rbseguros_CheckedChanged(object sender, EventArgs e)
        {
            if (rbseguros.Checked)
            {
                EmpresaBl.GetOrganizationSeguros(cboEmpresa, 9);
                lblEmpresa.Text = "SELECCIONE EMPRESA DE SEGUROS";
                cboEmpresa.Text = "";
                cboEmpresa.Enabled = true;
                cboEmpresa.Visible = true;
                lblContrata.Visible = true;
                lblContrata.Text = "EMPRESA DE TRABAJO";
                cboContrata.Text = "";
                cboContrata.Enabled = true;
                cboContrata.Visible = true;
            }
        }

        private void cboContrata_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConexionSigesoft conectasam = new ConexionSigesoft();
            conectasam.opensigesoft();
            var cadena1 = "select v_OrganizationId from organization where v_Name='" + cboContrata.Text + "'";
            SqlCommand comando = new SqlCommand(cadena1, connection: conectasam.conectarsigesoft);
            SqlDataReader lector = comando.ExecuteReader();
            while (lector.Read())
            {
                txtContrata.Text = lector.GetValue(0).ToString();
            }
            lector.Close();
            conectasam.closesigesoft();
        }

        private void txtDocNumber_HideSelectionChanged(object sender, EventArgs e)
        {

        }

        private void txtDocNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnBuscar_Click(null, null);
            }
        }

        private void cboEmpresa_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnBuscar_Click(null, null);
            }
        }

        private void cboContrata_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnBuscar_Click(null, null);
            }
        }
    }
}
