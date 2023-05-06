using SAMBHS.Common.Resource;
using SAMBHS.Windows.SigesoftIntegration.UI.BLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    public partial class frmSearchMedicamento : Form
    {
        private OperationResult _opbjOperationResult;
        private MedicamentoBL _objMedicamentoBl;
        public MedicamentoDto MedicamentoSeleccionado
        {
            get
            {
                var activeRow = ultraGrid1.ActiveRow;
                if (activeRow == null || activeRow.IsFilterRow) return null;
                return (MedicamentoDto)activeRow.ListObject;
            }
        }

        public frmSearchMedicamento()
        {
            InitializeComponent();
            _objMedicamentoBl = new MedicamentoBL();
            _opbjOperationResult = new OperationResult();
            Width = 940;
            Height = 626;
        }

        private void frmSearchMedicamento_Load(object sender, EventArgs e)
        {
            _opbjOperationResult = new OperationResult();
            gbBuscarMedicamento.Visible = true;
            gbRegistrarMedicamento.Visible = false;
            gbBuscarMedicamento.Dock = DockStyle.Fill;
            MedicamentoDao.ObtenerLineasParaCombo(cboLinea);
            MedicamentoDao.ObtenerUmParaCombo(cboUM);
        }

        private void btnRegistrarMedicamento_Click(object sender, EventArgs e)
        {
            gbBuscarMedicamento.Visible = false;
            gbRegistrarMedicamento.Visible = true;
            gbRegistrarMedicamento.Dock = DockStyle.Fill;
            txtNombreMedicamento.Focus();
            txtNombreMedicamento.Text = txtBuscarNombre.Text;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            gbBuscarMedicamento.Visible = true;
            gbRegistrarMedicamento.Visible = false;
            gbBuscarMedicamento.Dock = DockStyle.Fill;
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            CargarData();
        }

        private void CargarData()
        {
            try
            {
                _opbjOperationResult = new OperationResult();
                var data = _objMedicamentoBl.GetListMedicamentos(ref _opbjOperationResult, txtBuscarNombre.Text,
                    txtBuscarAccionF.Text);

                if (_opbjOperationResult.Success == 0)
                {
                    MessageBox.Show(_opbjOperationResult.ErrorMessage, @"Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                ultraGrid1.DataSource = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"CargarData()", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnGuardarMedicamento_Click(object sender, EventArgs e)
        {
            try
            {
                if (!uvRegistro.Validate(true, false).IsValid) return;

                _opbjOperationResult = new OperationResult();
                decimal d;
                int i;
                var entidad = new MedicamentoDto();
                entidad.Nombre = txtNombreMedicamento.Text.Trim();
                entidad.CodInterno = txtCodigo.Text.Trim();
                entidad.PrecioVenta = decimal.TryParse(txtPrecioVenta.Text.Trim(), out d) ? d : 0;
                entidad.AccionFarmaco = txtAccionFarmaco.Text.Trim();
                entidad.Concentracion = txtConcentracion.Text.Trim();
                entidad.PrincipioActivo = txtPrincipioActivo.Text.Trim();
                entidad.Presentacion = txtPresentacion.Text.Trim();
                entidad.Laboratorio = txtLaboratorio.Text.Trim();
                entidad.Ubicacion = txtUbicacion.Text.Trim();
                entidad.IdUnidadMedida = int.Parse(cboUM.Value.ToString());
                entidad.IdLinea = cboLinea.Value.ToString();
                _objMedicamentoBl.AddUpdateMedicamento(ref _opbjOperationResult, entidad, Globals.ClientSession.GetAsList());
                if (_opbjOperationResult.Success == 0)
                {
                    MessageBox.Show(_opbjOperationResult.ErrorMessage, @"Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                gbBuscarMedicamento.Visible = true;
                gbRegistrarMedicamento.Visible = false;
                gbBuscarMedicamento.Dock = DockStyle.Fill;
                txtBuscarNombre.Text = txtNombreMedicamento.Text;
                txtBuscarAccionF.Clear();
                ultraButton1_Click(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"btnGuardarMedicamento_Click()", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ultraGrid1_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            btnAceptar_Click(this, EventArgs.Empty);
        }

        private void txtPrecioVenta_Validating(object sender, CancelEventArgs e)
        {
            decimal d;
            if (!string.IsNullOrWhiteSpace(txtPrecioVenta.Text))
                txtPrecioVenta.Text = decimal.TryParse(txtPrecioVenta.Text.Trim(), out d) ? d.ToString(CultureInfo.InvariantCulture) : "0";
        }

        private void txtBuscarNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                ultraButton1_Click(null, null);
            }
        }

        private void txtBuscarAccionF_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                ultraButton1_Click(null, null);
            }
        }
    }
}
