using System;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.Requerimientos.NBS;

namespace SAMBHS.Windows.WinClient.UI.Requerimientos.NotariaBecerraSosaya
{
    public partial class FrmExportarIrpes : Form
    {
        public FrmExportarIrpes(string N)
        {
            InitializeComponent();
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtEmpresaDestino.Text))
            {
                NBS_DBF_PathSettings.Default.BdEmpresaDestino = txtEmpresaDestino.Text.Trim();
                NBS_DBF_PathSettings.Default.Save();
                var objOperationResult = new OperationResult();
                var exportaIrpeClass = new ExportarIrpesBl
                {
                    BaseDatosDestino = txtEmpresaDestino.Text,
                    Host = txtHost.Text,
                    Password = txtPassword.Text,
                    Usuario = txtUsuario.Text
                };

                lblStatus.Text = @"Procesando...";
                exportaIrpeClass.IniciarProceso(ref objOperationResult, dtpFecha.Value);
                lblStatus.Text = objOperationResult.Success == 0
                    ? string.Format("{0} {1} {2}", objOperationResult.ErrorMessage, objOperationResult.ExceptionMessage,
                        objOperationResult.AdditionalInformation)
                    : @"Proceso terminado correctamente!";
            }
            else
            {
                MessageBox.Show(@"Falta indicar la empresa destino!");
                gbConfiguracion.Visible = true;
            }
        }

        private void frmExportarIrpes_Load(object sender, EventArgs e)
        {
            linkConfigurar.Visible = Globals.ClientSession.i_SystemUserId == 1;
            txtEmpresaDestino.Text = NBS_DBF_PathSettings.Default.BdEmpresaDestino;
            txtHost.Text = UserConfig.Default.csServidor;
            txtPassword.Text = @"passwd";
            txtUsuario.Text = UserConfig.Default.csUsuario;
        }

        private void linkConfigurar_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            gbConfiguracion.Visible = !gbConfiguracion.Visible;
        }
    }
}
