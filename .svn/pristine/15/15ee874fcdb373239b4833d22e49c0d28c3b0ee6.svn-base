using Infragistics.Win.UltraWinGrid;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Security.BL;
using System;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinServer.UI.Administracion
{
    public partial class frmMantenimientoMenu : Form
    {
        ApplicationHierarchyBL _objProxySecurity = new ApplicationHierarchyBL();
        applicationhierarchyDto _applicationhierarchyDto = new applicationhierarchyDto();
        SystemParameterBL _objProxyCommon = new SystemParameterBL();
        TipoMantenimiento _TipoMantenimiento;

        public frmMantenimientoMenu(int IdApplicationHierarchy = 0, TipoMantenimiento oTipoMantenimiento = TipoMantenimiento.Nuevo)
        {
            InitializeComponent();

            OperationResult objOperationResultCommon = new OperationResult();
            var x = _objProxyCommon.GetSystemParameterForCombo(ref objOperationResultCommon, 106);
            var xx = cboAplicacion.DataSource;
            Utils.Windows.LoadUltraComboEditorList(cboAplicacion, "Value1", "Id",x , DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboScope, "Value1", "Id", _objProxyCommon.GetSystemParameterForCombo(ref objOperationResultCommon, 104), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboTipo, "Value1", "Id", _objProxyCommon.GetSystemParameterForCombo(ref objOperationResultCommon, 116), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboReglaNegocio, "Value1", "Id", _objProxyCommon.GetSystemParameterForCombo(ref objOperationResultCommon, 108), DropDownListAction.Select);

            CargarComboPadres();

            Cargar(oTipoMantenimiento, IdApplicationHierarchy);
            _TipoMantenimiento = oTipoMantenimiento;
        }

        private void frmMantenimientoMenu_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
        }

        void Cargar(TipoMantenimiento oTipoMantenimiento, int Id)
        {
            switch (oTipoMantenimiento)
            {
                case TipoMantenimiento.Nuevo:
                    cboAplicacion.SelectedIndex = 0;
                    cboScope.SelectedIndex = 0;
                    cboTipo.SelectedIndex = 0;
                    cboReglaNegocio.SelectedIndex = 0;
                    SeleccionarPadre(Id);
                    break;

                case TipoMantenimiento.Editar:
                    OperationResult objOperationResult = new OperationResult();
                    _applicationhierarchyDto = _objProxySecurity.GetApplicationHierarchy(ref objOperationResult, Id);

                    if (_applicationhierarchyDto != null)
                    {
                        this.Text = "Mantenmiento de Menú: " + _applicationhierarchyDto.v_Description;
                        cboAplicacion.Value = _applicationhierarchyDto.i_ApplicationHierarchyTypeId.ToString();
                        cboScope.Value = _applicationhierarchyDto.i_ScopeId.ToString();
                        txtCodigo.Text = _applicationhierarchyDto.v_Code;
                        txtDescripcion.Text = _applicationhierarchyDto.v_Description;
                        txtForm.Text = _applicationhierarchyDto.v_Form;
                        txtOrden.Text = _applicationhierarchyDto.i_Level.ToString();
                        cboTipo.Text = _applicationhierarchyDto.i_TypeFormId.ToString();
                        SeleccionarPadre(_applicationhierarchyDto.i_ParentId != null ? _applicationhierarchyDto.i_ParentId.Value : -1);
                    }

                    break;
            }
        }

        void SeleccionarPadre(int Id)
        {
            try
            {
                if (Id > 0)
                {
                    foreach (UltraGridRow oFila in grdData.Rows.GetRowEnumerator(GridRowType.DataRow, null, null))
                    {
                        if (oFila.Cells["Id"].Value != null && int.Parse(oFila.Cells["Id"].Value.ToString()) == Id)
                        {
                            oFila.Activate();
                            cboPadre.Text = oFila.Cells["Descripcion"].Value.ToString();
                            cboPadre.Tag = oFila.Cells["Id"].Value.ToString();
                            break;
                        }
                    }
                }
                else
                {
                    grdData.ActiveRow = null;
                    cboPadre.Text = "--Seleccionar--";
                    cboPadre.Tag = "-1";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        void CargarComboPadres()
        {
            try
            {
                OperationResult objOperationResult = new OperationResult();

                var MenusDS = _objProxySecurity.ObtenerMenus(ref objOperationResult);

                if (objOperationResult.Success == 0)
                {
                    MessageBox.Show((objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage).Trim(), "Error en la consulta.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (MenusDS != null && MenusDS.Count > 0)
                    {
                        int Counter = 0;

                        grdData.DataSource = MenusDS;
                        grdData.Rows.ExpandAll(true);
                        grdData.DisplayLayout.Override.RowSizing = RowSizing.AutoFree;
                        grdData.DisplayLayout.Override.ExpansionIndicator = Infragistics.Win.UltraWinGrid.ShowExpansionIndicator.CheckOnDisplay;

                        foreach (UltraGridBand Banda in grdData.DisplayLayout.Bands)
                        {
                            Banda.Columns["Descripcion"].CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
                            Banda.Columns["Descripcion"].CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
                            Banda.Columns["Id"].Hidden = true;
                            Banda.Columns["Form"].Hidden = true;
                            Banda.ColHeadersVisible = false;
                            Counter++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public enum TipoMantenimiento
        {
            Nuevo = 1,
            Editar = 2
        }

        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            if (grdData.ActiveRow != null)
            {
                cboPadre.Text = grdData.ActiveRow.Cells["Descripcion"].Value.ToString();
                cboPadre.Tag = grdData.ActiveRow.Cells["Id"].Value.ToString();
            }
            else
            {
                cboPadre.Text = "--Seleccionar--";
            }
        }

        private void cboPadre_Click(object sender, EventArgs e)
        {

        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (uvDatos.Validate(true, false).IsValid)
            {
                OperationResult objOperationResult = new OperationResult();
                switch (_TipoMantenimiento)
                {
                    case TipoMantenimiento.Nuevo:
                        _applicationhierarchyDto = new applicationhierarchyDto();
                        _applicationhierarchyDto.v_Code = txtCodigo.Text.Trim();
                        _applicationhierarchyDto.v_Description = txtDescripcion.Text.Trim();
                        _applicationhierarchyDto.v_Form = txtForm.Text.Trim();
                        _applicationhierarchyDto.i_ScopeId = int.Parse(cboScope.Value.ToString());
                        _applicationhierarchyDto.i_ParentId = int.Parse(cboPadre.Tag.ToString());
                        _applicationhierarchyDto.i_TypeFormId = int.Parse(cboTipo.Value.ToString());
                        _applicationhierarchyDto.i_Level = !string.IsNullOrEmpty(txtOrden.Text.Trim()) ? int.Parse(txtOrden.Text.Trim()) : 0;
                        _applicationhierarchyDto.i_ApplicationHierarchyTypeId = int.Parse(cboAplicacion.Value.ToString());
                        _objProxySecurity.AddApplicationHierarchy(ref objOperationResult, _applicationhierarchyDto, Globals.ClientSession.GetAsList());
                        break;

                    case TipoMantenimiento.Editar:
                        _applicationhierarchyDto.v_Code = txtCodigo.Text.Trim();
                        _applicationhierarchyDto.v_Description = txtDescripcion.Text.Trim();
                        _applicationhierarchyDto.v_Form = txtForm.Text.Trim();
                        _applicationhierarchyDto.i_ScopeId = int.Parse(cboScope.Value.ToString());
                        _applicationhierarchyDto.i_ParentId = int.Parse(cboPadre.Tag.ToString());
                        _applicationhierarchyDto.i_TypeFormId = int.Parse(cboTipo.Value.ToString());
                        _applicationhierarchyDto.i_Level = !string.IsNullOrEmpty(txtOrden.Text.Trim()) ? int.Parse(txtOrden.Text.Trim()) : 0;
                        _applicationhierarchyDto.i_ApplicationHierarchyTypeId = int.Parse(cboAplicacion.Value.ToString());
                        _objProxySecurity.UpdateApplicationHierarchy(ref objOperationResult, _applicationhierarchyDto, Globals.ClientSession.GetAsList());
                        break;
                }

                if (objOperationResult.Success == 0)
                {
                    MessageBox.Show((objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage).Trim(), "Error en la consulta.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    var VentanaMenus = Application.OpenForms["frmMenus"] as frmMenus;
                    VentanaMenus.RealizarAccionesMenu(frmMenus.AccionesMenu.Refrescar);
                    this.Close();
                }
            }
            
        }
    }
}
