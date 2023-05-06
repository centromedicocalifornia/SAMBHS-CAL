using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Contabilidad.BL;
using SAMBHS.Common.Resource;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmCopiarConfiguracionBalance : Form
    {
        public frmCopiarConfiguracionBalance()
        {
            InitializeComponent();
        }

        private void btnCopiar_Click(object sender, EventArgs e)
        {

     
            var resp = MessageBox.Show(string.Format("¿Está seguro de realizar este proceso ?"), "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resp == DialogResult.No) return;
            OperationResult objOperationResult = new OperationResult ();
            new AsientosContablesBL().CopiarBalances(ref objOperationResult, txtPeriodoOrigen.Text.Trim(), cboMesOrigen.Value.ToString(), txtPeriodoDestino.Text.Trim(), cboMesDestino.Value.ToString(), Globals.ClientSession.GetAsList());
            if (objOperationResult.Success == 1)
            {
                UltraMessageBox.Show("El proceso terminó correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                UltraMessageBox.Show("Hubo un error al realizar el proceso de copia", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        
        }

        private void txtPeriodoOrigen_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox((sender as Infragistics.Win.UltraWinEditors.UltraTextEditor), e);
        }

        private void txtPeriodoDestino_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox((sender as Infragistics.Win.UltraWinEditors.UltraTextEditor), e);
        }

        private void frmCopiarConfiguracionBalance_Load(object sender, EventArgs e)
        {
            txtPeriodoDestino.Text = Globals.ClientSession.i_Periodo.ToString();
            txtPeriodoOrigen.Text = Globals.ClientSession.i_Periodo.ToString();
            cboMesOrigen.Value = DateTime.Now.Month.ToString();
            cboMesDestino.Value = DateTime.Now.Month.ToString();
        }
    }
}
