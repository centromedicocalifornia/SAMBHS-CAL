using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Compra.BL;
using SAMBHS.Tesoreria.BL;


namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmRecalcularNroRegistros : Form
    {
        public frmRecalcularNroRegistros()
        {
            InitializeComponent();
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            var resp = MessageBox.Show(@"¿Seguro de recalcular los Nro. Registro de Todos los Procesos?", @"Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resp == DialogResult.No) return;

            ultraButton1.Enabled = false;
            OperationResult objOperationResult = new OperationResult();

            using (new LoadingClass.PleaseWait(this.Location, "Por favor espere..."))
            {
                new EstablecimientoBL().RecalcularNroRegistros(ref objOperationResult, txtPeriodo.Text, chkAplicarMes.Checked ? int.Parse(cboMeses.Value.ToString()) : -1, chkSoloCompras.Checked, chkSoloVentas.Checked);
            }

            if (objOperationResult.Success == 0)
            {
                MessageBox.Show(
                    objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
                    objOperationResult.AdditionalInformation, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show(@"Proceso Terminado Correctamente", @"Sistema", MessageBoxButtons.OK,
                MessageBoxIcon.Asterisk);
            
        }

        private void frmRecalcularNroRegistros_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
            txtPeriodo.Text = Globals.ClientSession.i_Periodo.ToString();
            LlenarComboMeses();
            cboMeses.SelectedIndex = 0;
            btnActualizarNombre.Visible = Globals.ClientSession.i_SystemUserId == 1;
        }

        /// <summary>
        /// Llena el combo con los doce meses del año.
        /// </summary>
        void LlenarComboMeses()
        {
        }

        private void chkAplicarMes_CheckedChanged(object sender, EventArgs e)
        {
            cboMeses.Enabled = chkAplicarMes.Checked;
        }

        private void btnEmparejar_Click(object sender, EventArgs e)
        {
            var resp = MessageBox.Show(@"¿Seguro de Proseguir?", @"Confirmación", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resp == DialogResult.No) return;
            var objOperationResult = new OperationResult();

            var f = new FrmRegenerarAsientosConfig();
            f.ShowDialog();
            if (f.DialogResult != DialogResult.OK) return;

            DiarioBL.EmparejarDiarios(ref objOperationResult, f.MesProceso);
            if (objOperationResult.Success == 0)
            {
                MessageBox.Show(
                    objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
                    objOperationResult.AdditionalInformation, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show(@"Proceso Terminado!");
        }

        private void btnEmparejarLiquidacionCompra_Click(object sender, EventArgs e)
         
        {
            var resp = MessageBox.Show(@"¿Seguro de Proseguir?", @"Confirmación", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resp == DialogResult.No) return;
            var objOperationResult = new OperationResult();
            DiarioBL.EmparejarDiariosLiquidaciónCompra(ref objOperationResult);
            if (objOperationResult.Success == 0)
            {
                MessageBox.Show(
                    objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
                    objOperationResult.AdditionalInformation, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show(@"Proceso Terminado!");

        }

        private void btnActualizarNombre_Click(object sender, EventArgs e)
        {
            var resp = MessageBox.Show(@"¿Seguro de Proseguir?", @"Confirmación", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resp == DialogResult.No) return;
            var objOperationResult = new OperationResult();
            DiarioBL.ActualizarNombreDiario(ref objOperationResult);
            if (objOperationResult.Success == 0)
            {
                MessageBox.Show(
                    objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
                    objOperationResult.AdditionalInformation, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show(@"Proceso Terminado!");
        }
    }
}
