using SAMBHS.Windows.SigesoftIntegration.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmProfesion : Form
    {
        public int grupo = 0;
        public frmProfesion(int _grupo)
        {
            grupo = _grupo;
            InitializeComponent();
        }


        private void frmProfesion_Load(object sender, EventArgs e)
        {
            AgendaBl.LlenarComboProfesion(cboMarketing);
            cboMarketing.SelectedValue = grupo == 0 ? -1 : grupo;

        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            if (cboMarketing.SelectedValue == null && cboMarketing.Text != string.Empty)
            {
                grupo = AgendaBl.CreateItemMarketing(cboMarketing.Text);
            }
            else if (cboMarketing.Text == string.Empty || cboMarketing.SelectedValue.ToString() == "-1")
            {
                MessageBox.Show("ERROR, SELECCIONE PROFESION A MODIFICAR", "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return;
            }
            else
            {
                grupo = int.Parse(cboMarketing.SelectedValue.ToString());

            }
            this.Close();
        }


    }
}
