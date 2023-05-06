using Infragistics.Win.UltraWinGrid;
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Common.BE;

namespace SAMBHS.Windows.WinServer.UI.Administracion
{
    // ReSharper disable once InconsistentNaming
    public partial class frmBandejaRoles : Form
    {
        readonly RoleBL _objRoleBl = new RoleBL();

        public frmBandejaRoles(string N)
        {
            InitializeComponent();
        }

        private void frmBandejaRoles_Load(object sender, EventArgs e)
        {
            Consultar();
            this.BackColor = new GlobalFormColors().FormColor;
        }

        void Consultar(string filterExpression = null)
        {
            var objOperationResult = new OperationResult();

            var dsRoles = _objRoleBl.GetRolePagedAndFiltered(ref objOperationResult, null, null, null, filterExpression);

            if (objOperationResult.Success == 1)
            {
                grdData.DataSource = dsRoles;
            }
            else
                MessageBox.Show(objOperationResult.ErrorMessage, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            if (Globals.ClientSession.i_RoleId != 1)
            {
                var fila = grdData.Rows.FirstOrDefault(p => int.Parse(p.Cells["i_RoleId"].Value.ToString()) == 1);

                if (fila != null)
                    fila.Activation = Activation.Disabled;
            }
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            string filterExpression = null;

            if (!string.IsNullOrEmpty(txtNombre.Text.Trim()))
                filterExpression = "v_Name.Contains(" + txtNombre.Text.Trim() + ")";

            Consultar(filterExpression);
        }

        private void grdData_ClickCellButton(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            try
            {
                if (e.Cell.Column.Key != "_Eliminar")
                {
                    if (e.Cell.Row == null) return;
                    var fila = e.Cell.Row;

                    var iRoleId = int.Parse(fila.Cells["i_RoleId"].Value.ToString());

                    var f = new frmPermisos(iRoleId);
                    f.ShowDialog();
                }
                else
                {
                    var resp = MessageBox.Show(@"¿Seguro que desea Eliminar el Rol?", @"Confirmación",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (resp == DialogResult.No) return;
                    OperationResult objOperationResult = new OperationResult();
                    int id = int.Parse(grdData.ActiveRow.Cells["i_RoleId"].Value.ToString());

                    if (id != 1)
                    {
                        _objRoleBl.DeleteRole(ref objOperationResult, id, Globals.ClientSession.GetAsList());

                        if (objOperationResult.Success == 0)
                        {
                            MessageBox.Show(
                                (objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage).Trim(),
                                @"Error en la consulta.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        Consultar();
                    }
                    else
                    {
                        MessageBox.Show(@"Este rol no se puede eliminar", @"Sistema", MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ultraButton2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ultraTextEditor1.Text.Trim()))
            {
                ultraTextEditor1.Focus();
                return;
            }
            
            var objOperationResult = new OperationResult();

            if (!_objRoleBl.CheckIfRoleExistsByName(ultraTextEditor1.Text.Trim()))
            {
                var roleDto = new roleDto
                {
                    v_Name = ultraTextEditor1.Text.Trim()
                };

                _objRoleBl.AddRole(ref objOperationResult, roleDto, Globals.ClientSession.GetAsList());

                Consultar();
            }
            else
            {
                MessageBox.Show(@"Ya existe un rol con el mismo nombre!", @"Error de Validación", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
