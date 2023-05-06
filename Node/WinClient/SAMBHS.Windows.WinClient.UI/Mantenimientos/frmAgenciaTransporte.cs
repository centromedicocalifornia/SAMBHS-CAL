using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.Resource;


namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmAgenciaTransporte : Form
    {

        string _strFilterExpression;
        string _mode = "";
        public frmAgenciaTransporte(string Parametro)
        {
            InitializeComponent();
        }

        #region BussinesLayers
        SystemParameterBL _objSystemParameterBL = new SystemParameterBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        AgenciaTransporteBL _objAgenciaTransporteBL = new AgenciaTransporteBL();
        //TransportistaChoferBL _objTransportistaChoferBL = new TransportistaChoferBL();
        //TransportistaUnidadTransporteBL _objTransportistaUnidadTransporteBL = new TransportistaUnidadTransporteBL();
        #endregion

        #region Entities
        agenciatransporteDto _agenciatransporteDto = new agenciatransporteDto();

        #endregion

        private void frmAgenciaTransporte_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            tpDatos.BackColor = new GlobalFormColors().FormColor;
            OperationResult objOperationResult = new OperationResult();

            #region Cargar combos
            //Combo País
            var ListaPaises = (_objSystemParameterBL.GetSystemParameterForComboKeyValueDto(ref objOperationResult, 112, null)).FindAll(p => p.Value3 == "-1");
            Utils.Windows.LoadDropDownList(ddlPais, "Value1", "Id", ListaPaises, DropDownListAction.Select);
            //Combo Departamento
            Utils.Windows.LoadDropDownList(ddlDepartamento, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);
            //Combo Provincia
            Utils.Windows.LoadDropDownList(ddlProvincia, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);
            //Combo Distrito
            Utils.Windows.LoadDropDownList(ddlDistrito, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);

            Utils.Windows.LoadDropDownList(ddlbTipoPersona, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 2, null), DropDownListAction.Select);


            #endregion

            tcDetalle.Enabled = false;
            _mode = "New";
            ddlPais.SelectedValue = "1";
            ddlDepartamento.SelectedValue = "1391";
            ddlProvincia.SelectedValue = "1392";
            ddlDistrito.SelectedValue = "1393";
        }

        #region Clases/Validaciones

        private void LimpiaDetalle()
        {
            txtCodigo.Clear();
            txtContacto.Clear();
            txtDireccion.Clear();
            txtEMail.Clear();
            txtFax.Clear();
            txtNombre.Clear();
            txtNroDocumento.Clear();
            txtTelefono.Clear();
            ddlPais.SelectedValue = "-1";
            ddlProvincia.SelectedValue = "-1";
            ddlDepartamento.SelectedValue = "-1";
            ddlTipoIdentificacion.SelectedValue = "-1";
            ddlDistrito.SelectedValue = "-1";
            ddlbTipoPersona.SelectedValue = "-1";
            _agenciatransporteDto = new agenciatransporteDto();
        }



        private void MantenerSeleccion(string ValorSeleccionado)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in grdData.Rows)
            {
                if (row.Cells["v_CodTransportista"].Text == ValorSeleccionado)
                {
                    grdData.ActiveRow = row;
                    grdData.ActiveRow.Selected = true;
                    break;
                }
            }
        }


        #endregion

        #region Cabecera

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            // Get the filters from the UI
            List<string> Filters = new List<string>();
            if (!string.IsNullOrEmpty(txtBuscarCodigo.Text)) Filters.Add("v_CodTransportista.Contains(\"" + txtBuscarCodigo.Text + "\")");
            if (!string.IsNullOrEmpty(txtBuscarDoc.Text)) Filters.Add("v_NumeroDocumento.Contains(\"" + txtBuscarDoc.Text.Trim().ToUpper() + "\")");
            if (!string.IsNullOrEmpty(txtBuscarNombre.Text)) Filters.Add("v_NombreRazonSocial.Contains(\"" + txtBuscarNombre.Text.Trim().ToUpper() + "\")");
            _strFilterExpression = null;
            if (Filters.Count > 0)
            {
                foreach (string item in Filters)
                {
                    _strFilterExpression = _strFilterExpression + item + " && ";
                }
                _strFilterExpression = _strFilterExpression.Substring(0, _strFilterExpression.Length - 4);
            }

            this.BindGrid();
            if (grdData.Rows.Count() == 0)
            {
                btnEliminar.Enabled = false;
                tcDetalle.Enabled = false;
            }
            else
            {
                btnEliminar.Enabled = true;
                tcDetalle.Enabled = true;
            }
        }

        private void BindGrid()
        {
            var objData = GetData("v_NombreRazonSocial ASC", _strFilterExpression, 0);

            grdData.DataSource = objData;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            _mode = "New";
            tcDetalle.Enabled = true;
            txtCodigo.Focus();
            LimpiaDetalle();

            ddlPais.SelectedValue = "1";
            ddlDepartamento.SelectedValue = "1391";
            ddlProvincia.SelectedValue = "1392";
            ddlDistrito.SelectedValue = "1393";
        }

        private List<agenciatransporteDto> GetData(string pstrSortExpression, string pstrFilterExpression, int pintGrupoId)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _objAgenciaTransporteBL.ObtenerListadoAgenciaTransporte(ref objOperationResult, pstrSortExpression, pstrFilterExpression);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            DialogResult Result = UltraMessageBox.Show("¿Está seguro de eliminar este registro?:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (Result == System.Windows.Forms.DialogResult.Yes)
            {
                _objAgenciaTransporteBL.EliminarAgenciaTransporte(ref objOperationResult, grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdAgenciaTransporte"].Value.ToString(), Globals.ClientSession.GetAsList());
                btnBuscar_Click(sender, e);
                LimpiaDetalle();
            }
        }

        private void grdData_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point point = new System.Drawing.Point(e.X, e.Y);
                Infragistics.Win.UIElement uiElement = ((Infragistics.Win.UltraWinGrid.UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

                if (uiElement == null || uiElement.Parent == null)
                    return;

                Infragistics.Win.UltraWinGrid.UltraGridRow row = (Infragistics.Win.UltraWinGrid.UltraGridRow)uiElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridRow));

                if (row != null)
                {
                    grdData.Rows[row.Index].Selected = true;
                    btnEliminar.Enabled = true;
                    //string intIdItem = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdAgenciaTransporte"].Value.ToString();
                    //CargarAgenciaTransporte(intIdItem);
                }
                else
                {
                    btnEliminar.Enabled = false;
                }
            }
        }

        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            string intIdItem = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdAgenciaTransporte"].Value.ToString();
            CargarAgenciaTransporte(intIdItem);
        }

        #endregion

        #region Detalle

        private void ddlPais_SelectedIndexChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (ddlPais.SelectedValue == null) return;

            //si el combo esta en seleccione tengo que reiniciar el combo departamento
            if (ddlPais.SelectedValue.ToString() == "-1")
            {
                Utils.Windows.LoadDropDownList(ddlDepartamento, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);
            }
            else
            {
                Utils.Windows.LoadDropDownList(ddlDepartamento, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, int.Parse(ddlPais.SelectedValue.ToString()), 112, ""), DropDownListAction.Select);
            }
        }

        private void ddlDepartamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (ddlDepartamento.SelectedValue == null) return;

            if (ddlDepartamento.SelectedValue.ToString() == "-1")
            {
                Utils.Windows.LoadDropDownList(ddlProvincia, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);
            }
            else
            {
                Utils.Windows.LoadDropDownList(ddlProvincia, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, int.Parse(ddlDepartamento.SelectedValue.ToString()), 112, ""), DropDownListAction.Select);
            }
        }

        private void ddlProvincia_SelectedIndexChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (ddlProvincia.SelectedValue == null) return;

            if (ddlProvincia.SelectedValue == "-1")
            {
                Utils.Windows.LoadDropDownList(ddlDistrito, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);
            }
            else
            {
                Utils.Windows.LoadDropDownList(ddlDistrito, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, int.Parse(ddlProvincia.SelectedValue.ToString()), 112, ""), DropDownListAction.Select);
            }
        }

        private void ddlbTipoPersona_SelectedIndexChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (ddlbTipoPersona.SelectedValue.ToString() != "-1")
            {
                Utils.Windows.LoadDropDownList(ddlTipoIdentificacion, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboKeyValueDto(ref objOperationResult, 150, null), DropDownListAction.Select);

                if (ddlbTipoPersona.SelectedValue.ToString() == "2")
                {

                    ddlTipoIdentificacion.SelectedValue = "6";
                    ddlTipoIdentificacion.Enabled = false;
                }
                else
                {
                    ddlTipoIdentificacion.SelectedValue = "-1";
                    ddlTipoIdentificacion.Enabled = true;
                }
            }
            else
            {
                Utils.Windows.LoadDropDownList(ddlTipoIdentificacion, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboKeyValueDto(ref objOperationResult, -1, null), DropDownListAction.Select);
            }
        }

        private void ddlTipoIdentificacion_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (ddlTipoIdentificacion.SelectedValue == null) return;

            if (ddlTipoIdentificacion.SelectedValue.ToString() != "-1")
            {
                var x = (KeyValueDTO)ddlTipoIdentificacion.SelectedItem;
                if (x == null) return;
                txtNroDocumento.MaxLength = int.Parse(x.Value2);
                txtNroDocumento.Focus();
                btnConsultaInternet.Enabled = ddlTipoIdentificacion.SelectedValue.ToString() == "6" || ddlTipoIdentificacion.SelectedValue.ToString() == "1";
            }
        }

        private void btnGrabar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();

            if (uvDatos.Validate(true, false).IsValid)
            {
                if (txtCodigo.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese un Código.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCodigo.Focus();
                    return;
                }

                if (txtNombre.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese un Nombre/Razón Social.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNombre.Focus();
                    return;
                }

                if (txtNroDocumento.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese un Número de Documento.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNroDocumento.Focus();
                    return;
                }

                if (ValidarTransportista())
                {

                    if (_mode == "New")
                    {
                        _agenciatransporteDto = new agenciatransporteDto();
                        _agenciatransporteDto.v_CodTransportista = txtCodigo.Text.Trim();
                        _agenciatransporteDto.v_NombreRazonSocial = txtNombre.Text.Trim();
                        _agenciatransporteDto.v_NombreContacto = txtContacto.Text.Trim();
                        _agenciatransporteDto.v_NumeroDocumento = txtNroDocumento.Text.Trim();
                        _agenciatransporteDto.v_Direccion = txtDireccion.Text.Trim();
                        _agenciatransporteDto.v_CorreoElectronico = txtEMail.Text.Trim();
                        _agenciatransporteDto.v_Fax = txtFax.Text.Trim();
                        _agenciatransporteDto.v_Telefono = txtTelefono.Text.Trim();
                        _agenciatransporteDto.i_IdTipoIdentificacion = int.Parse(ddlTipoIdentificacion.SelectedValue.ToString());
                        _agenciatransporteDto.i_IdTipoPersona = int.Parse(ddlbTipoPersona.SelectedValue.ToString());
                        _agenciatransporteDto.i_IdPais = int.Parse(ddlPais.SelectedValue.ToString());
                        _agenciatransporteDto.i_IdDistrito = int.Parse(ddlDistrito.SelectedValue.ToString());
                        _agenciatransporteDto.i_IdDepartamento = int.Parse(ddlDepartamento.SelectedValue.ToString());
                        _agenciatransporteDto.i_IdProvincia = int.Parse(ddlProvincia.SelectedValue.ToString());
                        _agenciatransporteDto.v_CorreoElectronico = txtEMail.Text.Trim();
                        // Save the data
                        _objAgenciaTransporteBL.InsertarAgenciaTransporte(ref objOperationResult, _agenciatransporteDto, Globals.ClientSession.GetAsList());
                    }
                    else if (_mode == "Edit")
                    {
                        _agenciatransporteDto.v_CodTransportista = txtCodigo.Text.Trim();
                        _agenciatransporteDto.v_NombreRazonSocial = txtNombre.Text.Trim();
                        _agenciatransporteDto.v_NombreContacto = txtContacto.Text.Trim();
                        _agenciatransporteDto.v_NumeroDocumento = txtNroDocumento.Text.Trim();
                        _agenciatransporteDto.v_Direccion = txtDireccion.Text.Trim();
                        _agenciatransporteDto.v_CorreoElectronico = txtEMail.Text.Trim();
                        _agenciatransporteDto.v_Fax = txtFax.Text.Trim();
                        _agenciatransporteDto.v_Telefono = txtTelefono.Text.Trim();
                        _agenciatransporteDto.i_IdTipoIdentificacion = int.Parse(ddlTipoIdentificacion.SelectedValue.ToString());
                        _agenciatransporteDto.i_IdTipoPersona = int.Parse(ddlbTipoPersona.SelectedValue.ToString());
                        _agenciatransporteDto.i_IdPais = int.Parse(ddlPais.SelectedValue.ToString());
                        _agenciatransporteDto.i_IdDistrito = int.Parse(ddlDistrito.SelectedValue.ToString());
                        _agenciatransporteDto.i_IdDepartamento = int.Parse(ddlDepartamento.SelectedValue.ToString());
                        _agenciatransporteDto.i_IdProvincia = int.Parse(ddlProvincia.SelectedValue.ToString());
                        _agenciatransporteDto.v_CorreoElectronico = txtEMail.Text.Trim();
                        // Save the data
                        _objAgenciaTransporteBL.ActualizarAgenciaTransporte(ref objOperationResult, _agenciatransporteDto, Globals.ClientSession.GetAsList());
                    }
                    //// Analizar el resultado de la operación
                    if (objOperationResult.Success == 1)  // Operación sin error
                    {
                        btnBuscar_Click(sender, e);
                        MantenerSeleccion(_agenciatransporteDto.v_CodTransportista);
                        UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else  // Operación con error
                    {
                        UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        void CargarAgenciaTransporte(string IdAgenciaTransporte)
        {
            OperationResult objOperationResult = new OperationResult();
            _agenciatransporteDto = new agenciatransporteDto();
            _agenciatransporteDto = _objAgenciaTransporteBL.ObtenerAgenciaTransporte(ref objOperationResult, IdAgenciaTransporte);
            txtCodigo.Text = _agenciatransporteDto.v_CodTransportista;
            txtContacto.Text = _agenciatransporteDto.v_NombreContacto;
            txtDireccion.Text = _agenciatransporteDto.v_Direccion;
            txtEMail.Text = _agenciatransporteDto.v_CorreoElectronico;
            txtFax.Text = _agenciatransporteDto.v_Fax;
            txtNombre.Text = _agenciatransporteDto.v_NombreRazonSocial;
            txtNroDocumento.Text = _agenciatransporteDto.v_NumeroDocumento;
            txtTelefono.Text = _agenciatransporteDto.v_Telefono;
            ddlbTipoPersona.SelectedValue = _agenciatransporteDto.i_IdTipoPersona.ToString();
            ddlPais.SelectedValue = _agenciatransporteDto.i_IdPais.ToString();
            ddlDepartamento.SelectedValue = _agenciatransporteDto.i_IdDepartamento.ToString();
            ddlProvincia.SelectedValue = _agenciatransporteDto.i_IdProvincia.ToString();
            ddlDistrito.SelectedValue = _agenciatransporteDto.i_IdDistrito.ToString();
            ddlTipoIdentificacion.SelectedValue = _agenciatransporteDto.i_IdTipoIdentificacion.ToString();
            tcDetalle.Enabled = true;
            btnGrabar.Enabled = true;
            _mode = "Edit";
        }

        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.InvokeOnClick(btnBuscar, e);
        }

        private void btnConsultaInternet_Click(object sender, EventArgs e)
        {
            var nroDoc = txtNroDocumento.Text.Trim();
            if (int.Parse(ddlTipoIdentificacion.SelectedValue.ToString()) == (int)TipoDocumentosPersona.Ruc)//evaluar
            {
                if (nroDoc.Length != txtNroDocumento.MaxLength)
                {
                    UltraMessageBox.Show("El RUC Ingresado es incorrecto", "Error!");
                    txtNombre.Clear();
                    txtDireccion.Clear();
                    return;
                }
                if (Utils.Windows.ValidarRuc(nroDoc) != true)
                {
                    UltraMessageBox.Show("El RUC Ingresado es incorrecto", "Error!");
                    txtNombre.Clear();
                    txtDireccion.Clear();
                    return;
                }

                var frm = new frmCustomerCapchaSUNAT(nroDoc);
                frm.ShowDialog();
                if (frm.ConectadoRecibido)
                {
                    var contribuyente = frm.DatosContribuyente;

                    if (txtNroDocumento.Text.StartsWith("1") && ddlbTipoPersona.SelectedValue.ToString() == "1")
                    {
                        var cadena = contribuyente[0].ToUpper().Trim().Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (cadena.GetUpperBound(0) > 0)
                            txtNombre.Text = string.Join(" ", cadena[0], cadena[1], cadena[cadena.Length - 1]);
                    }
                    else
                    {
                        txtNombre.Text = contribuyente[0].ToUpper().Trim();
                    }
                    txtDireccion.Text = Regex.Replace(contribuyente[5], @"[ ]+", " ");
                    var resultUbigueo = Utils.Ubigeo.GetUbigueo(txtDireccion.Text);
                    if (resultUbigueo != null)
                    {
                        ddlDepartamento.SelectedValue = resultUbigueo[0].Key.ToString();
                        ddlProvincia.SelectedValue = resultUbigueo[1].Key.ToString();
                        ddlDistrito.SelectedValue = resultUbigueo[2].Key.ToString();
                    }
                    txtTelefono.Text = contribuyente[6];
                }
            }

            if (int.Parse(ddlTipoIdentificacion.SelectedValue.ToString()) == (int)TipoDocumentosPersona.Dni)
            {
                if (nroDoc.Length != txtNroDocumento.MaxLength)
                {
                    UltraMessageBox.Show("El DNI Ingresado es incorrecto", "Error!");
                    return;
                }

                var frm = new frmCustomerCapchaRENIEC(nroDoc);
                frm.ShowDialog();
                if (!frm.ConectadoRecibido) return;
                var persona = frm.DatosPersona;
                if (persona != null)
                {
                    var cadena = string.Join(" ", persona[0], persona[1], persona[2]).ToUpper().Trim().Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

                    if (cadena.GetUpperBound(0) > 0)
                    {
                        txtNombre.Text = string.Join(" ", cadena[0], cadena[1], cadena[cadena.Length - 1]);
                    }
                }
                txtDireccion.Focus();
            }
        }
        #endregion

        private void txtNroDocumento_ValueChanged(object sender, EventArgs e)
        {

        }

        private void txtNroDocumento_Validating(object sender, CancelEventArgs e)
        {

           
            ValidarTransportista();
        }
        private bool  ValidarTransportista()
        {
            OperationResult objOperationResult = new OperationResult();
           

            var AgenciaTransporte = _objAgenciaTransporteBL.ValidarExistenciaAgenciaTransporte(ref objOperationResult, _agenciatransporteDto, txtNroDocumento.Text.Trim());
            if (_mode == "New" && AgenciaTransporte != null)
            {
                UltraMessageBox.Show("Este transportista ya fue registrado con el código : " + AgenciaTransporte.v_CodTransportista, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            else if (_mode == "Edit" && AgenciaTransporte != null)
            {
                UltraMessageBox.Show("Este transportista ya fue registrado con el código : " + AgenciaTransporte.v_CodTransportista, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            return true;
        
        
        }
    }
}
