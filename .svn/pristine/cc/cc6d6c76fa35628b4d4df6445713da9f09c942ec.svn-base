using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Security.BL;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmParametrosUsuario : Form
    {

        DatahierarchyBL _objDataHierarchyBL = new DatahierarchyBL();
        datahierarchyDto _datahierarchyDto ;
        SecurityBL _objSecurityBL = new SecurityBL();

        string _strFilterExpression;
        string  _mode;
        string _modeDetalle;
        bool _btnNuevoGrupo, _btnDetalle, _btnEliminarGrupo, _btnGrabar, _btnAgregarGuardar, _btnEditar, _btnEliminar;

        #region Cargar Página
        public frmParametrosUsuario(string Parametro)
        {
            InitializeComponent();
        }

        private void frmParametrosUsuario_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            OperationResult objOperationResult=new OperationResult ();

           _strFilterExpression = null;
            gbGrupo.Enabled = false;
            gbDetalle.Visible = false;

            #region ControlAcciones
            var _formActions = _objSecurityBL.GetFormAction(ref objOperationResult, Globals.ClientSession.i_CurrentExecutionNodeId, Globals.ClientSession.i_SystemUserId, "frmParametrosUsuario", Globals.ClientSession.i_RoleId);

            _btnNuevoGrupo = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmParametrosUsuario_NewG", _formActions);
            _btnDetalle = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmParametrosUsuario_Detail", _formActions);
            _btnEliminarGrupo = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmParametrosUsuario_DeleteG", _formActions);
            _btnGrabar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmParametrosUsuario_Save", _formActions);
            _btnAgregarGuardar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmParametrosUsuario_AddSave", _formActions);
            _btnEliminar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmParametrosUsuario_Delete", _formActions);
            #endregion

            btnEliminar.Enabled = false;
            btnCrearDetalle.Enabled = false;

        }             
        #endregion

        #region Bandeja de Búsqueda
        
        private void btnBuscar_Click(object sender, EventArgs e)
        {
                 // Get the filters from the UI
            List<string> Filters = new List<string>();
            if (!string.IsNullOrEmpty(txtParametroId.Text)) Filters.Add("i_ItemId==" + txtParametroId.Text);
            if (!string.IsNullOrEmpty(txtValor1.Text)) Filters.Add("v_Value1.Contains(\"" + txtValor1.Text.Trim().ToUpper() + "\")");
            Filters.Add("i_IsDeleted==0");
            // Create the Filter Expression
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
        }

        private void btnCrearGrupo_Click(object sender, EventArgs e)
        {
             OperationResult objOperationResult = new OperationResult();

            gbGrupo.Visible = true;
            gbDetalle.Visible = false;
            _mode = "New";
            LimpiarGrupo();
            gbGrupo.Enabled = true;
            btnGrabar.Enabled = true;
            txtGrupoIdGrupo.Text = "0";
            //Obtener el grupo que sigue
            //var maxValue = table.Max(x => x.Status);
            txtGrupoIdItem.Text = _objDataHierarchyBL.ObtenerMaxino(ref objOperationResult, 0).ToString();
            txtGrupoIdItem.Select();
            btnEditar.Enabled = false;
            btnEliminar.Enabled = false;
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
                    btnCrearDetalle.Enabled = true;
                    btnEliminarGrupo.Enabled = true;
                    btnCrearDetalle.Enabled = true;

                    int intIdItem = int.Parse(grdData.Selected.Rows[0].Cells["i_ItemId"].Value.ToString());

                    gbDetalle.Visible = false;
                    gbGrupo.Visible = true;
                    CargarGrupo(intIdItem);
                }
                else
                {
                    btnCrearDetalle.Enabled = false;
                    btnEliminarGrupo.Enabled = false;
                    btnCrearDetalle.Enabled = false;
                }
            }
        }

        private void txtParametroId_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtParametroId, e);
        }

        private void btnEliminarGrupo_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            // Obtener los IDs de la fila seleccionada

            DialogResult Result = UltraMessageBox.Show("¿Está seguro de eliminar este registro?:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (Result == System.Windows.Forms.DialogResult.Yes)
            {
                // Delete the item              
                int intIdItem = int.Parse(grdData.Selected.Rows[0].Cells["i_ItemId"].Value.ToString());

                _objDataHierarchyBL.DeleteDataHierarchy(ref objOperationResult, 0, intIdItem, Globals.ClientSession.GetAsList());

                btnBuscar_Click(sender, e);
                LimpiarGrupo();
                gbGrupo.Enabled = false;
                btnEliminarGrupo.Enabled = false;
                btnCrearDetalle.Enabled = false;

            }
        }

        private void btnCrearDetalle_Click(object sender, EventArgs e)
        {
            _modeDetalle = "New";
            gbGrupo.Visible = false;
            gbDetalle.Visible = true;

            btnAgregar.Enabled = true;


            int intIdItem = int.Parse(grdData.Selected.Rows[0].Cells["i_ItemId"].Value.ToString());

            OperationResult objOperationResult = new OperationResult();
            var _objData = _objDataHierarchyBL.GetDataHierarchiesPagedAndFiltered(ref objOperationResult, "", "", intIdItem);
            txtDetalleIdGrupo.Text = intIdItem.ToString();

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            grdDetalle.DataSource = _objData;
        }

        private void BindGrid()
        {
            var objData = GetData("v_Value1 ASC", _strFilterExpression, 0);

            grdData.DataSource = objData;
        }

        void LimpiarGrupo()
        {
            txtGrupoIdGrupo.Text = "0";
            txtGrupoIdItem.Text = string.Empty;
            txtGrupoOrdenamiento.Text = string.Empty;
            txtGrupoValor1.Text = string.Empty;
            txtGrupoValor2.Text = string.Empty;

            txtParametroId.Select();
        }

        private List<datahierarchyDto> GetData(string pstrSortExpression, string pstrFilterExpression, int pintGrupoId)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _objDataHierarchyBL.GetDataHierarchiesPagedAndFiltered(ref objOperationResult, pstrSortExpression, pstrFilterExpression, pintGrupoId);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }

        void CargarGrupo(int pintIdItem)
        {
            OperationResult objOperationResult = new OperationResult();
            _datahierarchyDto = new datahierarchyDto();
            _datahierarchyDto = _objDataHierarchyBL.GetDataHierarchy(ref objOperationResult, 0, pintIdItem);
            txtGrupoIdGrupo.Text = "0";
            txtGrupoIdItem.Text = _datahierarchyDto.i_ItemId.ToString();
            txtGrupoOrdenamiento.Text = _datahierarchyDto.i_Sort == 0 ? string.Empty : _datahierarchyDto.i_Sort.ToString();
            txtGrupoValor1.Text = _datahierarchyDto.v_Value1;
            txtGrupoValor2.Text = _datahierarchyDto.v_Value2;
            gbGrupo.Enabled = true;
            btnGrabar.Enabled = true;
            _mode = "Edit";
        }


        void CargarDetalle(int pintIdGrupo, int pintIdItem)
        {
            OperationResult objOperationResult = new OperationResult();
            _datahierarchyDto = new datahierarchyDto();

            _datahierarchyDto = _objDataHierarchyBL.GetDataHierarchy(ref objOperationResult, pintIdGrupo, pintIdItem);
            txtDetalleIdItem.Text = _datahierarchyDto.i_ItemId.ToString();
            txtDetalleOrdenamiento.Text = _datahierarchyDto.i_Sort == 0 ? "" : _datahierarchyDto.i_Sort.ToString();
            txtDetalleValor1.Text = _datahierarchyDto.v_Value1;
            txtDetalleValor2.Text = _datahierarchyDto.v_Value2;
            txtDetalleValor3.Text = _datahierarchyDto.v_Field;
            txtDetalleValor4.Text = _datahierarchyDto.v_Value4;
            chkCabecera.Enabled = txtDetalleOrdenamiento.Text.Trim().Length > 0 ? true : false;
            chkCabecera.Checked = _datahierarchyDto.i_Header == 1 ? true : false;
            _modeDetalle = "Edit";
        }

        private void frmParametrosUsuario_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }


        #endregion

        #region Edición Cabecera

        private void btnGrabar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (uvGrupo.Validate(true, false).IsValid)
            {
                if (txtGrupoIdItem.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese un IdItem.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (txtGrupoValor1.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese un Valor 1.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (_mode == "New")
                {
                    _datahierarchyDto = new datahierarchyDto();

                    _datahierarchyDto.i_GroupId = 0;
                    _datahierarchyDto.i_ItemId = int.Parse(txtGrupoIdItem.Text.ToString());
                    _datahierarchyDto.i_Sort = txtGrupoOrdenamiento.Text == string.Empty ? 0 : int.Parse(txtGrupoOrdenamiento.Text.ToString());
                    _datahierarchyDto.v_Value1 = txtGrupoValor1.Text.Trim();
                    _datahierarchyDto.v_Value2 = txtGrupoValor2.Text.Trim();

                    // Save the data
                    _objDataHierarchyBL.AddDataHierarchy(ref objOperationResult, _datahierarchyDto, Globals.ClientSession.GetAsList());

                }
                else if (_mode == "Edit")
                {
                    _datahierarchyDto.i_GroupId = 0;
                    _datahierarchyDto.i_ItemId = int.Parse(txtGrupoIdItem.Text.ToString());
                    _datahierarchyDto.i_Sort = txtGrupoOrdenamiento.Text == string.Empty ? 0 : int.Parse(txtGrupoOrdenamiento.Text.ToString());
                    _datahierarchyDto.v_Value1 = txtGrupoValor1.Text.Trim();
                    _datahierarchyDto.v_Value2 = txtGrupoValor2.Text.Trim();
                    // Save the data
                    _objDataHierarchyBL.UpdateDataHierarchy(ref objOperationResult, _datahierarchyDto, Globals.ClientSession.GetAsList());


                }
                //// Analizar el resultado de la operación
                if (objOperationResult.Success == 1)  // Operación sin error
                {
                    LimpiarGrupo();
                    btnBuscar_Click(sender, e);
                    btnEliminarGrupo.Enabled = false;
                    btnCrearDetalle.Enabled = false;
                }
                else  // Operación con error
                {
                    UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Se queda en el formulario.
                }

            }
        }

        private void txtGrupoIdItem_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtGrupoIdItem, e);
        }

        private void txtGrupoOrdenamiento_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtGrupoOrdenamiento, e);
        }

        #endregion

        #region Edición Detalle

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (uvDetalle.Validate(true, false).IsValid)
            {
                if (txtDetalleValor1.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese el Valor 1.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (_modeDetalle == "New")
                {
                    _datahierarchyDto = new datahierarchyDto();

                    _datahierarchyDto.i_GroupId = int.Parse(txtDetalleIdGrupo.Text.ToString());
                    _datahierarchyDto.i_ItemId = _objDataHierarchyBL.ObtenerMaxino(ref objOperationResult, _datahierarchyDto.i_GroupId);
                    _datahierarchyDto.i_Sort = txtDetalleOrdenamiento.Text == string.Empty ? 0 : int.Parse(txtDetalleOrdenamiento.Text.ToString());
                    _datahierarchyDto.v_Value1 = txtDetalleValor1.Text;
                    _datahierarchyDto.v_Value2 = txtDetalleValor2.Text;
                    _datahierarchyDto.v_Field = txtDetalleValor3.Text;
                    _datahierarchyDto.i_Header = chkCabecera.Checked ? 1 : 0;
                    _datahierarchyDto.v_Value4 = txtDetalleValor4.Text;
                    // Save the data
                    _objDataHierarchyBL.AddDataHierarchy(ref objOperationResult, _datahierarchyDto, Globals.ClientSession.GetAsList());

                }
                else if (_modeDetalle == "Edit")
                {
                    _datahierarchyDto.i_GroupId = int.Parse(txtDetalleIdGrupo.Text.ToString());
                    _datahierarchyDto.i_ItemId = int.Parse(txtDetalleIdItem.Text.ToString());
                    _datahierarchyDto.i_Sort = txtDetalleOrdenamiento.Text == string.Empty ? 0 : int.Parse(txtDetalleOrdenamiento.Text.ToString());
                    _datahierarchyDto.v_Value1 = txtDetalleValor1.Text;
                    _datahierarchyDto.v_Value2 = txtDetalleValor2.Text;
                    _datahierarchyDto.v_Field = txtDetalleValor3.Text;
                    _datahierarchyDto.i_Header = chkCabecera.Checked ? 1 : 0;
                    _datahierarchyDto.v_Value4 = txtDetalleValor4.Text.Trim();
                    // Save the data
                    _objDataHierarchyBL.UpdateDataHierarchy(ref objOperationResult, _datahierarchyDto, Globals.ClientSession.GetAsList());

                }
                //// Analizar el resultado de la operación
                if (objOperationResult.Success == 1)  // Operación sin error
                {

                    CargarGrillaDetalle(int.Parse(txtDetalleIdGrupo.Text.ToString()));
                    btnEditar.Enabled = false;
                    btnEliminar.Enabled = false;
                }
                else  // Operación con error
                {
                    UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Se queda en el formulario.
                }

                txtDetalleIdItem.Text = string.Empty;
                txtDetalleOrdenamiento.Text = string.Empty;
                txtDetalleValor1.Text = string.Empty;
                txtDetalleValor2.Text = string.Empty;
                txtDetalleValor3.Text = string.Empty;
                chkCabecera.Checked = false;
                _modeDetalle = "New";
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            // Obtener los IDs de la fila seleccionada
            DialogResult Result = UltraMessageBox.Show("¿Está seguro de eliminar este registro?:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (Result == System.Windows.Forms.DialogResult.Yes)
            {
                // Delete the item              
                int intIdItem = int.Parse(grdDetalle.Selected.Rows[0].Cells["i_ItemId"].Value.ToString());
                int intIdGrupo = int.Parse(txtDetalleIdGrupo.Text.ToString());
                _objDataHierarchyBL.DeleteDataHierarchy(ref objOperationResult, intIdGrupo, intIdItem, Globals.ClientSession.GetAsList());
                CargarGrillaDetalle(intIdGrupo);
                btnEditar.Enabled = false;
                btnEliminar.Enabled = false;

            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (grdDetalle.Selected.Rows.Count > 0)
            {
                int intIdItem = int.Parse(grdDetalle.Selected.Rows[0].Cells["i_ItemId"].Value.ToString());
                int intIdGrupo = int.Parse(txtDetalleIdGrupo.Text.ToString());
                CargarDetalle(intIdGrupo, intIdItem);
            }

        }

        private void txtDetalleIdItem_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtDetalleIdItem, e);
        }

        private void txtDetalleOrdenamiento_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtDetalleOrdenamiento, e);
        }

        private void grdDetalle_MouseDown(object sender, MouseEventArgs e)
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
                    grdDetalle.Rows[row.Index].Selected = true;

                    btnEditar.Enabled = true;
                    btnEliminar.Enabled = true;
                }
                else
                {
                    btnEditar.Enabled = false;
                    btnEliminar.Enabled = false;
                }
            }
        }

        void CargarGrillaDetalle(int pintIdGrupo)
        {
            grdDetalle.DataSource = GetData("", "", pintIdGrupo);
        }

        #endregion

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtDetalleOrdenamiento_TextChanged(object sender, EventArgs e)
        {
            if (txtDetalleOrdenamiento.Text.Trim().Length > 0)
            {
                chkCabecera.Enabled = true;
            }
            else
            {
                chkCabecera.Enabled = false;
            }
        }

        private void grdDetalle_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            
        }

        private void grdDetalle_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            if (e.Row.Cells["i_Header"].Value != null && e.Row.Cells["i_Header"].Value.ToString() == "1")
            {
                e.Row.Appearance.ForeColor = Color.Black;
                e.Row.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                e.Row.Appearance.BackColor = Color.Bisque;
                e.Row.Appearance.BackColor2 = Color.Bisque;
            }
            else if (e.Row.Cells["i_Header"].Value == null || e.Row.Cells["i_Header"].Value.ToString() == "0")
            {
                e.Row.Appearance.ForeColor = Color.Black;
                e.Row.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.False;
                e.Row.Appearance.BackColor = Color.White;
            }
        }
    }
}
