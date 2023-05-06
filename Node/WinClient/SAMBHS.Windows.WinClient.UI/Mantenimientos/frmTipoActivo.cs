using SAMBHS.Windows.WinClient.UI.Mantenimientos.UserControls;
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
    public partial class frmTipoActivo : Form
    {
        private const int pIntIdGroup = 104;

        public frmTipoActivo(string N)
        {
            InitializeComponent();
        }

        private void frmTipoActivo_Load(object sender, EventArgs e)
        {
            ucMantenimientoHierarchy uc = new ucMantenimientoHierarchy(pIntIdGroup);
            uc.Dock = DockStyle.Fill;
            panel_root.Controls.Add(uc);
        }
    }
}
