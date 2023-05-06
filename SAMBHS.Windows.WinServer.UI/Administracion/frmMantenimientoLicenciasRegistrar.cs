using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.Security.BL;

namespace SAMBHS.Windows.WinServer.UI.Administracion
{
    public partial class frmMantenimientoLicenciasRegistrar : Form
    {
        private readonly SecurityBL _objSecurityBl = new SecurityBL();
        public frmMantenimientoLicenciasRegistrar()
        {
            InitializeComponent();
        }

        private void txtUID_Validating(object sender, CancelEventArgs e)
        {
            var uid = txtUID.Text.Trim();
            if (string.IsNullOrWhiteSpace(uid)) return;
            var seccionado = uid.Split('-');
            var longitudOk = seccionado.Length == 4;

            if (!longitudOk)
            {
                MessageBox.Show(@"El formato del GUI no es correcto", @"Sistema", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                e.Cancel = true;
            }
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ultraValidator1.Validate(true, false).IsValid) return;
                var lic = new licensesDto
                {
                    UID = txtUID.Text,
                    v_Descripcion = txtDescripcion.Text,
                    v_License = Convert.ToBoolean(txtLicenciaRuta.Tag.ToString()) ? File.ReadAllText(txtLicenciaRuta.Text) : txtLicenciaRuta.Text,
                    t_FechaCreacion = DateTime.Now
                };

                if (_objSecurityBl.ActualizarLicencia(lic))
                {
                    MessageBox.Show(@"Licencia registrada.", @"Sistema", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    Close();
                }
                else
                    MessageBox.Show(@"No se pudo registrar la licencia", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtLicenciaRuta_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var key = e.Button.Key;

            switch (key)
            {
                case "_File":
                    var choofdlog = new OpenFileDialog
                       {
                           Filter = @"Archivos Licencia (*.lic)| *.lic",
                           FilterIndex = 1,
                           Multiselect = false
                       };

                    if (choofdlog.ShowDialog() != DialogResult.OK) return;
                    txtLicenciaRuta.Text = choofdlog.FileName;
                    txtLicenciaRuta.ButtonsRight["_Input"].Enabled = false;
                    txtLicenciaRuta.Tag = true;
                    break;

                case "_Input":
                    var input = new frmLicenseInput { StartPosition = FormStartPosition.CenterScreen };
                    var r = input.ShowDialog();
                    if (r != DialogResult.OK) return;
                    txtLicenciaRuta.Text = input.LicenciaInput;
                    txtLicenciaRuta.ButtonsRight["_File"].Enabled = false;
                    txtLicenciaRuta.Tag = false;
                    break;
            }

        }

        private void frmMantenimientoLicenciasRegistrar_Load(object sender, EventArgs e)
        {
            txtLicenciaRuta.Tag = true;
        }
    }
}
