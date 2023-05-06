using System;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class FrmReemplazarCuentasContables : Form
    {
        public FrmReemplazarCuentasContables()
        {
            InitializeComponent();
        }

        private void frmReemplazarCuentasContables_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
        }

        private void btnProcesar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCuentaAntigua.Text) || string.IsNullOrWhiteSpace(txtCuentaNueva.Text)) return;
            var resp = MessageBox.Show(
                @"ATENCION!: Este proceso debe ser manejado por personal estrictamente contable con fines correctivos solamente de emergencia. ¿Desea Continuar?",
                @"Advertencia!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (resp == DialogResult.No) return;

            if (!Utils.Windows.EsCuentaImputable(txtCuentaAntigua.Text.Trim()) ||
                !Utils.Windows.EsCuentaImputable(txtCuentaNueva.Text.Trim()))
            {
                MessageBox.Show(@"Por favor verifique que ambas cuentas sean válidas", @"Sistema", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }
            btnProcesar.Enabled = false;
            var objBuscarReemplazarCuentas = new BuscarReemplazarCuentas();
            objBuscarReemplazarCuentas.OnProgresoEvent += objBuscarReemplazarCuentas_OnProgresoEvent;
            objBuscarReemplazarCuentas.OnErrorEvent += objBuscarReemplazarCuentas_OnErrorEvent;
            objBuscarReemplazarCuentas.ComenzarAsync(txtCuentaAntigua.Text.Trim(), txtCuentaNueva.Text.Trim());
        }

        private static void objBuscarReemplazarCuentas_OnErrorEvent(OperationResult pobjOperationResult)
        {
            if (pobjOperationResult.Success == 0)
            {
                UltraMessageBox.Show(
                    pobjOperationResult.ErrorMessage + "\n\n" + pobjOperationResult.ExceptionMessage +
                    "\n\nTARGET: " + pobjOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void objBuscarReemplazarCuentas_OnProgresoEvent(string progreso)
        {
            Invoke((MethodInvoker) delegate
            {
                lblStatus.Text = progreso;
            });
        }

        private void txtCuentaAntigua_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var f = new frmPlanCuentasConsulta("");
            f.ShowDialog();

            if (f._NroSubCuenta != null)
            {
                txtCuentaAntigua.Text = f._NroSubCuenta;
            }
        }

        private void txtCuentaNueva_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var f = new frmPlanCuentasConsulta("");
            f.ShowDialog();

            if (f._NroSubCuenta != null)
            {
                txtCuentaNueva.Text = f._NroSubCuenta;
            }
        }
    }
}
