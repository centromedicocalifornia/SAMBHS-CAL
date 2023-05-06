using SAMBHS.Common.Resource;
using SAMBHS.Security.BL;
using System;
using System.Linq;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;

namespace SAMBHS.Windows.WinServer.UI.Administracion
{
    public partial class frmBandejaUsuarios : Form
    {
        public frmBandejaUsuarios(string N)
        {
            InitializeComponent();
        }
    
        private void frmBandejaUsuarios_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            CargarDatos();

        }

        void CargarDatos()
        {
            OperationResult objOperationResult = new OperationResult();
            grdData.DataSource = new SecurityBL().GetSystemUserPagedAndFiltered(ref objOperationResult, null, null, null, null);
            if (Globals.ClientSession.i_RoleId != 1)
            {
                var fila = grdData.Rows.FirstOrDefault(p => int.Parse(p.Cells["i_SystemUserId"].Value.ToString()) == 1);

                if (fila != null)
                    fila.Activation = Activation.Disabled;
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            frmUsuario f = new frmUsuario(frmUsuario.Estado.Nuevo);
            f.ShowDialog();
            CargarDatos();
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow != null && grdData.ActiveRow.Activation != Activation.Disabled)
            {
                int id = int.Parse(grdData.ActiveRow.Cells["i_SystemUserId"].Value.ToString());
                frmUsuario f = new frmUsuario(frmUsuario.Estado.Edicion, id);
                f.ShowDialog();
            }
        }

        private void grdData_ClickCellButton(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            if (e.Cell != null )
            {
                int id = int.Parse(grdData.ActiveRow.Cells["i_SystemUserId"].Value.ToString());
                string v_PersonName = grdData.ActiveRow.Cells["v_PersonName"].Value.ToString();
                frmMantenimientoUsuarioEmpresa f = new frmMantenimientoUsuarioEmpresa(id, v_PersonName);
                f.ShowDialog();
            }
        }

        private void ultraButton2_Click(object sender, EventArgs e)
        {

            if (grdData.ActiveRow == null || grdData.ActiveRow.Activation == Activation.Disabled) return;
            var resp = MessageBox.Show(@"¿Seguro que desea Eliminar el Usuario?", @"Confirmación",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resp == DialogResult.No) return;
            OperationResult objOperationResult = new OperationResult();

            int id = int.Parse(grdData.ActiveRow.Cells["i_SystemUserId"].Value.ToString());

            if (id != 1)
            {
                new SecurityBL().DeleteSystemUSer(ref objOperationResult, id, Globals.ClientSession.GetAsList());

                if (objOperationResult.Success == 0)
                {
                    MessageBox.Show((objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage).Trim(), @"Error en la consulta.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                CargarDatos();
            }
            else
            {
                MessageBox.Show("Este usuario no se puede eliminar!", "Sistema", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
        }

    }
}
