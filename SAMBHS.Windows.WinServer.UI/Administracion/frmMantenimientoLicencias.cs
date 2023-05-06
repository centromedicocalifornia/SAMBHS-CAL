using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.Security.BL;

namespace SAMBHS.Windows.WinServer.UI.Administracion
{
    public partial class frmMantenimientoLicencias : Form
    {
        private readonly SecurityBL _objSecurityBl = new SecurityBL();
        public frmMantenimientoLicencias(string N)
        {
            InitializeComponent();
        }

        private void frmMantenimientoLicencias_Load(object sender, EventArgs e)
        {
            BackColor = Color.White;
            Cargar();
        }

        private void Cargar(string descripcion = null)
        {
            try
            {
                var maximo = SystemValidator.ObtenerLimiteLicencias;
                var usadas = SystemValidator.ObtenerLicenciasUsadas;

                var listado = _objSecurityBl.ObtenerLicencias(descripcion);
                ultraGrid1.DataSource = listado;
                lblLicenciasDisponibles.Text = string.Format("Licencias utilizadas:  {0}/{1}", usadas, maximo);
                ultraButton2.Enabled = maximo > 0 && usadas < maximo;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ultraGrid1_ClickCellButton(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            try
            {
                var buttonCell = e.Cell.Column.Key;
                var desc = e.Cell.Row.Cells["v_Descripcion"].Value.ToString();
                switch (buttonCell)
                {
                    case "_Eliminar":
                        var msg = MessageBox.Show(string.Format("¿Seguro de Eliminar la licencia de {0}?", desc), 
                                                    @"Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (msg == DialogResult.No) return;

                        var uid = e.Cell.Row.Cells["UID"].Value.ToString();
                        _objSecurityBl.EliminarLicencia(uid);
                        Cargar();
                        break;

                    case "_Editar":
                        var choofdlog = new OpenFileDialog
                           {
                               Filter = @"Archivos Licencia (*.lic)| *.lic",
                               FilterIndex = 1,
                               Multiselect = false
                           };

                        if (choofdlog.ShowDialog() != DialogResult.OK) return;
                        var msgA = MessageBox.Show(string.Format("¿Seguro de Actualizar la licencia de {0}?", desc), 
                                                    @"Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (msgA == DialogResult.No) return;

                        var lic = (licensesDto)e.Cell.Row.ListObject;
                        lic.v_License = File.ReadAllText(choofdlog.FileName);
                        if (_objSecurityBl.ActualizarLicencia(lic))
                        {
                            MessageBox.Show(@"Licencia Actualizada!", @"Sistema", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            Cargar();
                        }
                        else
                            MessageBox.Show(@"No se pudo actualizar la licencia", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            Cargar(ultraTextEditor1.Text);
        }

        private void ultraButton2_Click(object sender, EventArgs e)
        {
            var r = new frmMantenimientoLicenciasRegistrar { StartPosition = FormStartPosition.CenterScreen };
            r.ShowDialog();
            Cargar();
        }
    }
}
