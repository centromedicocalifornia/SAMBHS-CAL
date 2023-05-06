using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
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
    public partial class frmNotasEstadosSituacionFinanciera : Form
    {
        private const int pIntIdGroup = 172;

        public frmNotasEstadosSituacionFinanciera(string N)
        {
            InitializeComponent();
        }

        private void frmNotasEstadosSituacionFinanciera_Load(object sender, EventArgs e)
        {
            ucMantenimientoHierarchy uc = new ucMantenimientoHierarchy(pIntIdGroup);
            uc.Dock = DockStyle.Fill;
            panel_root.Controls.Add(uc);
        }
    }
}
