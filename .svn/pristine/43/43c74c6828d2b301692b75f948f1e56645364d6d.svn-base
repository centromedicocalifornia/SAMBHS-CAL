using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Security.BL;
using System;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinServer.UI.Administracion
{
    public partial class frmUsuario : Form
    {
        PersonBL _objPersonBL = new PersonBL();
        SystemParameterBL _objSystemParameterBL = new SystemParameterBL();
        personDto _personDto = new personDto();
        systemuserDto _systemuserDto = new systemuserDto();

        Estado objEstado;
        int _IdUsuario;

        public frmUsuario(Estado _Estado, int IdUsuario = 0)
        {
            InitializeComponent();
            objEstado = _Estado;
            _IdUsuario = IdUsuario;
        }

        private void frmUsuario_Load(object sender, EventArgs e)
        {
            txtPassword.ButtonsLeft[0].Enabled = false;
            OperationResult objOperationResult = new OperationResult();
            this.BackColor = new GlobalFormColors().FormColor;
            var _DocType = _objSystemParameterBL.GetSystemParameterForCombo(ref objOperationResult, 150);

            Utils.Windows.LoadDropDownList(cboTipoDocumento, "Value1", "Id", _DocType, DropDownListAction.Select);
            Utils.Windows.LoadDropDownList(cboSexo, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForCombo(ref objOperationResult, 100), DropDownListAction.Select);
            Utils.Windows.LoadDropDownList(cboEstadoCivil, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForCombo(ref objOperationResult, 101), DropDownListAction.Select);
            Utils.Windows.LoadDropDownList(cboRolUsuario, "Value1", "Id", _objSystemParameterBL.GetRole(ref objOperationResult, ""), DropDownListAction.Select);
            CargarUsuario();
        }

        public enum Estado
        {
            Nuevo = 1,
            Edicion = 2
        }

        void CargarUsuario()
        {
            OperationResult objOperationResult = new OperationResult();
            switch (objEstado)
            {
                case Estado.Edicion:
                    _systemuserDto = new SecurityBL().GetSystemUser(ref objOperationResult, _IdUsuario);

                    if (_systemuserDto != null && _systemuserDto.i_PersonId != null)
                    {
                        txtPassword.ButtonsLeft[0].Enabled = true;
                        _personDto = _objPersonBL.GetPerson(ref objOperationResult, _systemuserDto.i_PersonId.Value);
                        txtNombreUsuario.Text = _systemuserDto.v_UserName;
                        txtPassword.Text = _systemuserDto.v_Password;
                        txtRepetirPassword.Text = _systemuserDto.v_Password;
                        cboRolUsuario.SelectedValue = _systemuserDto.i_RoleId.ToString();
                        chkUsuarioContable.Checked = _systemuserDto.i_UsuarioContable == 1;
                        if (_personDto != null)
                        {
                            txtApellidoMaterno.Text = _personDto.v_SecondLastName;
                            txtApellidoPaterno.Text = _personDto.v_FirstLastName;
                            txtPrimerNombre.Text = _personDto.v_FirstName;
                            txtNroDocumento.Text = _personDto.v_DocNumber;
                            txtDirección.Text = _personDto.v_AdressLocation;
                            txtCorreo.Text = _personDto.v_Mail;
                            txtLugarNacimiento.Text = _personDto.v_BirthPlace;
                            cboEstadoCivil.SelectedValue = _personDto.i_MaritalStatusId.ToString();
                            cboSexo.SelectedValue = _personDto.i_SexTypeId.ToString();
                            txtNroCelular.Text = _personDto.v_TelephoneNumber;
                            cboTipoDocumento.SelectedValue = _personDto.i_DocTypeId.ToString();
                            //dtpFechaNacimiento.Value = _personDto.d_Birthdate != null ? _personDto.d_Birthdate.Value : DateTime.MinValue;
                        }
                    }
                    txtPassword.ReadOnly = true;
                    txtRepetirPassword.ReadOnly = true;
                    txtNombreUsuario.Enabled = false;
                    break;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (uvDatos.Validate(true, false).IsValid)
            {
                OperationResult objOperationResult = new OperationResult();

                switch (objEstado)
                {
                    case Estado.Nuevo:
                        _personDto = new personDto();
                        _systemuserDto = new systemuserDto();
                        _personDto.d_Birthdate = dtpFechaNacimiento.Value;
                        _personDto.i_DocTypeId = int.Parse(cboTipoDocumento.SelectedValue.ToString());
                        _personDto.i_LevelOfId = -1;
                        _personDto.i_SexTypeId = int.Parse(cboSexo.SelectedValue.ToString());
                        _personDto.v_AdressLocation = txtDirección.Text.Trim();
                        _personDto.v_BirthPlace = txtLugarNacimiento.Text.Trim();
                        _personDto.v_ContactName = null;
                        _personDto.v_DocNumber = txtNroDocumento.Text.Trim();
                        _personDto.v_FirstName = txtPrimerNombre.Text.Trim();
                        _personDto.v_SecondLastName = txtApellidoMaterno.Text.Trim();
                        _personDto.v_FirstLastName = txtApellidoPaterno.Text.Trim();
                        _personDto.v_Mail = txtCorreo.Text.Trim();
                        _personDto.v_TelephoneNumber = txtNroCelular.Text.Trim();
                        _personDto.i_MaritalStatusId = int.Parse(cboEstadoCivil.SelectedValue.ToString());
                        _systemuserDto.i_UsuarioContable = chkUsuarioContable.Checked ? 1 : 0;
                        _systemuserDto.i_RoleId = int.Parse(cboRolUsuario.SelectedValue.ToString());
                        _systemuserDto.v_Password = Utils.Encrypt(txtPassword.Text.Trim());
                        _systemuserDto.v_UserName = txtNombreUsuario.Text.Trim();
                        _objPersonBL.AddPerson(ref objOperationResult, _personDto, _systemuserDto, Globals.ClientSession.GetAsList());
                        break;

                    case Estado.Edicion:
                        _personDto.d_Birthdate = dtpFechaNacimiento.Value;
                        _personDto.i_DocTypeId = int.Parse(cboTipoDocumento.SelectedValue.ToString());
                        _personDto.i_LevelOfId = -1;
                        _personDto.i_SexTypeId = int.Parse(cboSexo.SelectedValue.ToString());
                        _personDto.v_AdressLocation = txtDirección.Text.Trim();
                        _personDto.v_BirthPlace = txtLugarNacimiento.Text.Trim();
                        _personDto.v_ContactName = null;
                        _personDto.v_DocNumber = txtNroDocumento.Text.Trim();
                        _personDto.v_FirstName = txtPrimerNombre.Text.Trim();
                        _personDto.v_SecondLastName = txtApellidoMaterno.Text.Trim();
                        _personDto.v_FirstLastName = txtApellidoPaterno.Text.Trim();
                        _personDto.v_Mail = txtCorreo.Text.Trim();
                        _personDto.v_TelephoneNumber = txtNroCelular.Text.Trim();
                        _personDto.i_MaritalStatusId = int.Parse(cboEstadoCivil.SelectedValue.ToString());
                        _systemuserDto.i_RoleId = int.Parse(cboRolUsuario.SelectedValue.ToString());
                        _systemuserDto.i_UsuarioContable = chkUsuarioContable.Checked ? 1 : 0;
                        _objPersonBL.UpdatePerson(ref objOperationResult, false, _personDto, false, _systemuserDto, Globals.ClientSession.GetAsList());
                        break;
                }

                if (objOperationResult.Success == 1)
                {
                    UltraMessageBox.Show("Usuario Guardado");
                }
                else
                {
                    UltraMessageBox.Show(objOperationResult.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void txtNroCelular_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtNroCelular, e);
        }

        private void txtNroDocumento_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtNroDocumento, e);
        }

        private void txtPassword_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var resp =
                MessageBox.Show(
                    @"¿Seguro de Resetear su password?, Si continua su password se reseteara al por defecto: (12345678)",
                    @"Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resp != DialogResult.Yes) return;
            var objOperationResult = new OperationResult();
            SecurityBL.RestorePassword(ref objOperationResult, _systemuserDto.i_SystemUserId);

            if (objOperationResult.Success == 0)
                UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\n" + objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
