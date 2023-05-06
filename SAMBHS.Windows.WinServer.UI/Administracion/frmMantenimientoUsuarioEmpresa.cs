using System;
using System.Drawing;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Security.BL;

namespace SAMBHS.Windows.WinServer.UI.Administracion
{
    public partial class frmMantenimientoUsuarioEmpresa : Form
    {
        private readonly int _IdUsuario;
        public frmMantenimientoUsuarioEmpresa(int IdUsuario, string NombreUsuario)
        {
            string titulo = String.Format("Empresas Asigandas al Usuario: {0}", NombreUsuario);
            _IdUsuario = IdUsuario;
            InitializeComponent();
            Text = titulo;
            Cargar(IdUsuario);
            OperationResult objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList(cboEmpresas, "Value1", "Id", NodeBL.GetAllNodeForCombo(ref objOperationResult), DropDownListAction.Select);
            cboEmpresas.Value = "-1";
        }

        private void Cargar(int IdUsuario)
        {
            OperationResult objOperationResult = new OperationResult();
            var ds = new SecurityBL().GetSystemUserNode(ref objOperationResult, IdUsuario);

            if (objOperationResult.Success == 0)
            {
                MessageBox.Show((objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage).Trim(), @"Error en la consulta.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            grdData.DataSource = ds;
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (!ultraValidator1.Validate(true, false).IsValid) return;
            var objOperationResult = new OperationResult();

            var systemusernodeDto = new systemusernodeDto
            {
                i_NodeId = int.Parse(cboEmpresas.Value.ToString()),
                i_SystemUserId = _IdUsuario
            };

            new SecurityBL().AddSystemUserNode(ref objOperationResult, systemusernodeDto, Globals.ClientSession.GetAsList());

            if (objOperationResult.Success == 0)
            {
                MessageBox.Show((objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage).Trim(), @"Error en la consulta.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Cargar(_IdUsuario);
        }

        private void frmMantenimientoUsuarioEmpresa_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
        }

        private void grdData_ClickCellButton(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            if (e.Cell == null) return;
            var resp = MessageBox.Show(@"¿Seguro que desea Eliminar el registro?", @"Confirmación",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resp == DialogResult.No) return;

            OperationResult objOperationResult = new OperationResult();
            var id = int.Parse(e.Cell.Row.Cells["i_SystemUserNodeId"].Value.ToString());
            new SecurityBL().DeleteSystemUserNode(ref objOperationResult, id, Globals.ClientSession.GetAsList());

            if (objOperationResult.Success == 0)
            {
                MessageBox.Show((objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage).Trim(), @"Error en la consulta.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Cargar(_IdUsuario);
        }
    }
}
