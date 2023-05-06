using System;
using System.Linq;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.Contabilidad.BL;
using SAMBHS.Security.BL;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmPlanCuentasExtraccion : Form
    {
        OperationResult objOperationResult = new OperationResult();
        public frmPlanCuentasExtraccion()
        {
            InitializeComponent();
            Text = @"Copia Plan Contable para el Periodo " + Globals.ClientSession.i_Periodo;
        }

        private void rbExtraerModificar_CheckedChanged(object sender, EventArgs e)
        {
            txtInfo.Text = @"Las cuentas que no existan en el modelo se ingresarán y las que existan y sean diferentes, se modificarán para que sean igual a las del modelo.";
        }

        private void rbReemplazarTodo_CheckedChanged(object sender, EventArgs e)
        {
            txtInfo.Text = @"Realiza la Copia Parcial, con la diferencia de que las cuentas locales que no existan en el modelo serán eliminadas. Si estan siendo usadas no se eliminará y el proceso fallará.";
        }

        private void frmPlanCuentasExtraccion_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
            var nodos = new SecurityBL().ReturnNodes(ref objOperationResult);
            nodos = nodos.Where(p => !p.Value1.Equals(Globals.ClientSession.v_RucEmpresa)).ToList();
            Utils.Windows.LoadUltraComboList(ucNodos, "Value2", "Id", nodos, DropDownListAction.Select);
            ucNodos.SelectedIndex = 0;
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (!ultraValidator1.Validate(true, false).IsValid) return;
            if (!rbExtraerModificar.Checked && !rbReemplazarTodo.Checked)
            {
                MessageBox.Show(@"Por favor elija una acción para el proceso.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var msg = MessageBox.Show("¿Seguro de continuar?.\nPara verificar si el proceso se puede revertir contacte con el proveedor",
                @"Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if (msg == DialogResult.No) return;

            try
            {
                var objBl = new PlanCuentasBl
                {
                    TipoMotor = UserConfig.Default.csTipoMotorBD,
                    Database = ((GridKeyValueDTO)ucNodos.SelectedItem.ListObject).Value1,
                    Host = UserConfig.Default.csServidor,
                    Password = UserConfig.Default.csPassword,
                    Username = UserConfig.Default.csUsuario
                };

                if (rbExtraerModificar.Checked)
                {
                    int modificados, insertados;
                    objBl.CopiaParcial(out modificados, out insertados);
                    MessageBox.Show(string.Format("Proceso terminado con éxito\n- {0} Modificados\n- {1} Insertados",
                        modificados, insertados), @"Sistema", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                {
                    int modificados, insertados, eliminados;
                    objBl.CopiaCompleta(out modificados, out insertados, out eliminados);
                    MessageBox.Show(string.Format("Proceso terminado con éxito\n- {0} Modificados\n- {1} Insertados\n- {2} Eliminados",
                        modificados, insertados, eliminados), @"Sistema", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                MessageBox.Show(exception.Message, @"Error en el proceso", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
