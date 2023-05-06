using System;
using System.ComponentModel;
using System.Deployment.Application;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    // ReSharper disable once InconsistentNaming
    public partial class frmActualizacionesSistema : Form
    {
        #region Construct
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once UnusedParameter.Local
        public frmActualizacionesSistema(string N)
        {
            InitializeComponent();
        }
        #endregion

        #region Method
        private void UpdateApplication()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                var ad = ApplicationDeployment.CurrentDeployment;
                ad.CheckForUpdateCompleted += ad_CheckForUpdateCompleted;
                ad.CheckForUpdateProgressChanged += ad_CheckForUpdateProgressChanged;

                ad.CheckForUpdateAsync();
            }
            else
            {
                label2.Visible = true;
                ultraTextEditor1.Visible = true;
                ultraTextEditor1.Text = @"ESTA APLICACIÓN NO ESTA VERSIONADA";
            }
        }
        private string GetProgressString(DeploymentProgressState state)
        {
            switch (state)
            {
                case DeploymentProgressState.DownloadingApplicationFiles:
                    return "application files";
                case DeploymentProgressState.DownloadingApplicationInformation:
                    return "application manifest";
                default:
                    return "deployment manifest";
            }
        }

        private void BeginUpdate()
        {
            ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;
            ad.UpdateCompleted += ad_UpdateCompleted;

            // Indicate progress in the application's status bar.
            ad.UpdateProgressChanged += ad_UpdateProgressChanged;
            ad.UpdateAsync();
        }
        #endregion

        #region Events
        private void ad_CheckForUpdateProgressChanged(object sender, DeploymentProgressChangedEventArgs e)
        {
            downloadStatus.Text = string.Format("Descarga: {0}. {1:D}K de {2:D}K descargados.", GetProgressString(e.State), e.BytesCompleted / 1024, e.BytesTotal / 1024);
        }

        private void ad_CheckForUpdateCompleted(object sender, CheckForUpdateCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                label2.Visible = true;
                ultraTextEditor1.Visible = true;
                ultraTextEditor1.Text = @"ERROR: Could not retrieve new version of the application. Reason: " +
                    e.Error.Message + @"Please report this error to the system administrator.";
                return;
            }
            if (e.Cancelled)
            {
                MessageBox.Show(@"The update was cancelled.");
            }

            // Ask the user if they would like to update the application now. 
            if (e.UpdateAvailable)
            {
                BeginUpdate();
            }
        }

        private void ad_UpdateProgressChanged(object sender, DeploymentProgressChangedEventArgs e)
        {
            var form = Application.OpenForms["frmMaster"] as frmMaster;
            if(form != null) form.ReinicioRequerido = true;
            downloadStatus.Visible = true;
            label1.Visible = true;
            ultraButton1.Enabled = false;
            var progressText = string.Format("{0:D}K de {1:D}K descargado - {2:D}% completado", e.BytesCompleted/1024, e.BytesTotal/1024, e.ProgressPercentage);
            downloadStatus.Text = progressText;
        }

        private void ad_UpdateCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                label2.Visible = true;
                ultraTextEditor1.Visible = true;
                ultraTextEditor1.Text = @"The update of the application's latest version was cancelled.";
                return;
            }
            if (e.Error != null)
            {
                label2.Visible = true;
                ultraTextEditor1.Visible = true;
                ultraTextEditor1.Text = @"ERROR: Could not install the latest version of the application. Reason: " + 
                    e.Error.Message + @"Please report this error to the system administrator.";
                return;
            }
            Application.Restart();
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            var thisprocessname = Process.GetCurrentProcess().ProcessName;
            if (Process.GetProcesses().Count(p => p.ProcessName == thisprocessname) == 1) // busca actualizaciones sólo si hay UNA instancia del sistema corriendo en la pc
            {
                label2.Visible = false;
                ultraTextEditor1.Visible = false;
                try
                {
                    UpdateApplication();
                }
                catch
                {
                    try
                    {
                        using (NetworkShareAccesser.Access("198.50.230.132", "tisoluciones", "abcABC123")) // intenta actualizar abriendo un tunel al servidor
                        {
                            UpdateApplication();
                        }
                    }
                    catch (Exception ex)
                    {
                        label2.Visible = true;
                        ultraTextEditor1.Visible = true;
                        ultraTextEditor1.Text = ex.Message + @"\n" + (ex.InnerException != null ? ex.InnerException.Message : string.Empty);
                    }
                }
            }
            else
            {
                label2.Visible = true;
                ultraTextEditor1.Visible = true;
                ultraTextEditor1.Text = @"HAY MÁS DE UNA INSTANCIA DE LA APLICACIÓN EN EJECUCIÓN";
            }
        }
        #endregion

        private void frmActualizacionesSistema_Load(object sender, EventArgs e)
        {
            ultraTextEditor1.Text = string.Empty;
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                var deploy = ApplicationDeployment.CurrentDeployment;
                var uri = deploy.ActivationUri;
                var url = deploy.DataDirectory;
                if (uri != null) ultraTextEditor1.Text = uri.AbsolutePath + " -- " + uri.Host;
                ultraTextEditor1.Text += " -- " + url;
            }
        }
    }
}
