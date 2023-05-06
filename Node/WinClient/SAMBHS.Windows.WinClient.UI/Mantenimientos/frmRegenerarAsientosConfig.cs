using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using SAMBHS.Common.Resource;


namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class FrmRegenerarAsientosConfig : Form
    {
        public int MesProceso { get; set; }

        public FrmRegenerarAsientosConfig()
        {
            InitializeComponent();
        }

        private void frmRegenerarAsientosConfig_Load(object sender, EventArgs e)
        {
            LlenarComboMeses();
        }

        /// <summary>
        /// Llena el combo con los doce meses del año.
        /// </summary>
        private void LlenarComboMeses()
        {
            
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            MesProceso = int.Parse(dtpFechaRegistroDe.SelectedValue.ToString());
            DialogResult = DialogResult.OK;
        }
    }
}
