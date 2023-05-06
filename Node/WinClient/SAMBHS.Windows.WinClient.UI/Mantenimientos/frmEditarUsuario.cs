using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmEditarUsuario : Form
    {
        SystemParameterBL _objSystemParameterBL = new SystemParameterBL();
        UsuarioBL _objUsuarioBL = new UsuarioBL();
        public frmEditarUsuario(string N)
        {
            InitializeComponent();
        }

        private void ultraTextEditor9_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            ultraGroupBox3.Visible = true;
            txtContrasenaActual.Focus();
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtContrasenaActual.Text.Trim()))
            {
                txtContrasenaActual.Focus();
            }

            if (string.IsNullOrEmpty(txtNuevaContrasena.Text.Trim()))
            {
                txtNuevaContrasena.Focus();
            }

            if (string.IsNullOrEmpty(txtRepetirContrasena.Text.Trim()))
            {
                txtRepetirContrasena.Focus();
            }


            if (uvUsuario.Validate(true, false).IsValid)
            {
                if (Utils.Encrypt(txtContrasenaActual.Text.Trim()) == txtContrasena.Text)
                {
                    if (txtNuevaContrasena.Text.Trim() == txtRepetirContrasena.Text.Trim())
                    {
                        OperationResult objOperationResult = new OperationResult();

                        _objUsuarioBL.ActualizarContrasena(ref objOperationResult, Utils.Encrypt(txtNuevaContrasena.Text.Trim()), Globals.ClientSession.i_SystemUserId);

                        if (objOperationResult.Success == 0)
                        {
                            UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n" + objOperationResult.ExceptionMessage);
                        }
                        else
                        {
                            txtContrasena.Text = Utils.Encrypt(txtNuevaContrasena.Text.Trim());
                            UltraMessageBox.Show("Contraseña Actualizada");
                            ultraGroupBox3.Visible = false;
                            txtRepetirContrasena.Clear();
                            txtNuevaContrasena.Clear();
                            txtContrasenaActual.Clear();
                        }
                    }
                    else
                    {
                        UltraMessageBox.Show("Las contraseñas no son iguales");
                        txtNuevaContrasena.Focus();
                    }
                }
                else
                {
                    UltraMessageBox.Show("La contraseña no es correcta");
                    txtContrasenaActual.Focus();
                }
            }
        }

        private void frmEditarUsuario_Load(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();

            Utils.Windows.LoadUltraComboEditorList(cboTipoDocumento, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboKeyValueDto(ref objOperationResult, 150, null), DropDownListAction.Select);
            
            this.BackColor = new GlobalFormColors().FormColor;
            var Persona = _objUsuarioBL.DevolverUsuario(ref objOperationResult, Globals.ClientSession.i_SystemUserId);
            txtApellidoMaterno.Text = Persona.v_SecondLastName;
            txtPrimerNombre.Text = Persona.v_FirstName;
            txtApellidoPaterno.Text = Persona.v_FirstLastName;
            txtLugarNacimiento.Text = Persona.v_BirthPlace;
            dtpFechaNacimiento.Value = Persona.d_Birthdate ?? DateTime.Parse("01/01/1990");
            txtID.Text = Globals.ClientSession.i_SystemUserId.ToString();
            txtNombreUsuario.Text = Persona.UserName;
            txtContrasena.Text = Persona.Password;
            cboTipoDocumento.Value = Persona.i_DocTypeId.ToString();
            txtNroDocumento.Text = Persona.v_DocNumber;

            var EmpresasAsignadas = _objUsuarioBL.DevolverEmpresasAsignadas(Globals.ClientSession.i_SystemUserId);
            foreach (var Empresa in EmpresasAsignadas)
            {
                listEmpresas.Items.Add("-" + Empresa.Ruc + " | " + Empresa.Nombre);
            }

            txtRolSistema.Text = Globals.ClientSession.v_RoleName;
        }

        private void ultraButton1_Click_1(object sender, EventArgs e)
        {
            ultraGroupBox3.Visible = false;
        }

        private void btnGuardarDatos_Click(object sender, EventArgs e)
        {
            if (uvPersona.Validate(true, false).IsValid)
            {
                OperationResult objOperationResult = new OperationResult();

                personDto Persona = new personDto();
                Persona.v_FirstName = txtPrimerNombre.Text.Trim();
                Persona.v_FirstLastName = txtApellidoPaterno.Text.Trim();
                Persona.v_SecondLastName = txtApellidoMaterno.Text.Trim();
                Persona.i_PersonId = (int)Globals.ClientSession.i_PersonId;
                Persona.i_DocTypeId = Int32.Parse(cboTipoDocumento.Value.ToString());
                Persona.v_DocNumber = txtNroDocumento.Text.Trim();
                Persona.v_BirthPlace = txtLugarNacimiento.Text.Trim();
                Persona.d_Birthdate = dtpFechaNacimiento.Value;

                _objUsuarioBL.ActualizarPersona(ref objOperationResult, Persona);

                if (objOperationResult.Success == 0)
                {
                    UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n" + objOperationResult.ExceptionMessage);
                }
            }
            
        }

        private void txtNroDocumento_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtNroDocumento, e);
        }
    }
}
