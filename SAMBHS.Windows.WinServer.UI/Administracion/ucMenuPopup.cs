using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinServer.UI.Administracion
{
    public partial class ucMenuPopup : UserControl
    {
        int oApplicationHierarchyID;

        public ucMenuPopup(int ApplicationHierarchyID, string NombreMenu)
        {
            InitializeComponent();
            oApplicationHierarchyID = ApplicationHierarchyID;
            NombreMenu = NombreMenu.Length > 18 ? NombreMenu.Substring(0, 15) + "..." : NombreMenu;
            lblNombreMenu.Text = NombreMenu;
        }

        private void ucMenuPopup_Load(object sender, EventArgs e)
        {

        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            var VentanaMenus = Application.OpenForms["frmMenus"] as frmMenus;
            VentanaMenus.RealizarAccionesMenu(frmMenus.AccionesMenu.Modificar, oApplicationHierarchyID);
        }

        private void btnContraer_Click(object sender, EventArgs e)
        {
            var VentanaMenus = Application.OpenForms["frmMenus"] as frmMenus;
            VentanaMenus.RealizarAccionesMenu(frmMenus.AccionesMenu.ExpanderMenusHijos);
        }

        private void btnColapsar_Click(object sender, EventArgs e)
        {
            var VentanaMenus = Application.OpenForms["frmMenus"] as frmMenus;
            VentanaMenus.RealizarAccionesMenu(frmMenus.AccionesMenu.ColapsarMenusHijos);
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            var VentanaMenus = Application.OpenForms["frmMenus"] as frmMenus;
            VentanaMenus.RealizarAccionesMenu(frmMenus.AccionesMenu.Eliminar, oApplicationHierarchyID);
        }

        private void btnAgregarMenuHijo_Click(object sender, EventArgs e)
        {
            var VentanaMenus = Application.OpenForms["frmMenus"] as frmMenus;
            VentanaMenus.RealizarAccionesMenu(frmMenus.AccionesMenu.AgregarMenuHijo, oApplicationHierarchyID);
        }
    }
}
