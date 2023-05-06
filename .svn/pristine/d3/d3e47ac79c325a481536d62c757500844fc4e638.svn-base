using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Common.Resource;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class FrmMigrarDataPeriodos : Form
    {
        private bool Bussy { get; set; }
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        public FrmMigrarDataPeriodos()
        {
            InitializeComponent();
        }

        private void frmMigrarDataPeriodos_Load(object sender, EventArgs e)
        {
            Bussy = false;
            txtPeriodoOrigen.Text = (Globals.ClientSession.i_Periodo ?? DateTime.Now.Year).ToString();
            txtPeriodoDestino.Text = ((Globals.ClientSession.i_Periodo ?? DateTime.Now.Year) + 1).ToString();
            BackColor = new GlobalFormColors().FormColor;
            panel1.BackColor = new GlobalFormColors().BannerColor;
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            if (!Bussy)
            {
                var msj = MessageBox.Show(@"¿Seguro de continuar?", @"Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (msj == DialogResult.No) return;
                var objOperationResult = new OperationResult();
                var periodoO = txtPeriodoOrigen.Text.Trim();
                var periodoD = txtPeriodoDestino.Text.Trim();
                var lineas = chkLineas.Checked;
                var plancuentas = chkPlanCuentas.Checked;
                var almacen = chkProductoAlmacen.Checked;
                var conceptos = chkConceptos.Checked;
                var planilla = chkPlanillas.Checked;
                var destinos = chkDestinos.Checked;

                Task.Factory.StartNew(() =>
                {
                    Bussy = true;
                    Invoke((MethodInvoker)delegate
                    {
                        ultraButton1.Appearance.Image = Resource.ajax_loaderMin;
                        ultraButton1.Text = @"Procesando.";
                    });
                    Utils.AperturaData.AperturarCuentasPorPeriodo(ref objOperationResult, periodoO, periodoD, lineas,
                        plancuentas, conceptos, almacen, planilla, destinos);
                }, _cts.Token).ContinueWith(t =>
                {
                    if (objOperationResult.Success == 0)
                    {
                        MessageBox.Show(
                            string.Format("{0}\n{1}\n{2}", objOperationResult.ErrorMessage,
                                objOperationResult.ExceptionMessage,
                                objOperationResult.AdditionalInformation), @"Error", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show("Proceso terminado correctamente", "Sistema");
                    }
                    ultraButton1.Appearance.Image = Resource.database_gear;
                    ultraButton1.Text = @"Comenzar!";
                    Bussy = false;
                }, _cts.Token, TaskContinuationOptions.LongRunning, TaskScheduler.FromCurrentSynchronizationContext());
            }    
        }

        private void frmMigrarDataPeriodos_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = Bussy;
            if (Bussy)
                MessageBox.Show(@"Por favor esperar a que el proceso culmine.", @"Sistema", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
