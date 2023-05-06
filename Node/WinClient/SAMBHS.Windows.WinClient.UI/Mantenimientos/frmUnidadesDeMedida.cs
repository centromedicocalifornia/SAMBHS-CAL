using System;
using System.Linq;
using System.Windows.Forms;
using Infragistics.Win;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmUnidadesDeMedida : Form
    {
        #region Fields
        private const byte IdGroupUnidadMedida = 17;
        readonly DatahierarchyBL _objDataHierarchyBl = new DatahierarchyBL();
        datahierarchyDto _datahierarchyDto;
        string _modeDetalle;
        #endregion

        #region Construct
        public frmUnidadesDeMedida(string N)
        {
            InitializeComponent();
            Load += (_, __) =>
            {
                BackColor = new GlobalFormColors().FormColor;
                var opResult = new OperationResult();
                var data = _objDataHierarchyBl.GetDataHierarchyForComboKeyValueDto(ref opResult, 159, null);
                Utils.Windows.LoadUltraComboList(cbUndInter, "Value1", "Id", data, DropDownListAction.Select);
                grdData.DisplayLayout.ValueLists["listUM"].ValueListItems.AddRange(data.Select(o => new ValueListItem
                {
                    DataValue = o.Id,
                    DisplayText = o.Value2,
                }).ToArray());
                BindGrid();
                SetDatos(false);
            };
        }
        #endregion

        #region Methods
        private void BindGrid()
        {
            OperationResult objOperationResult = new OperationResult();
            var objData = _objDataHierarchyBl.GetDataHierarchiesPagedAndFiltered(ref objOperationResult, null, null, IdGroupUnidadMedida);

            if (objOperationResult.Success == 0)
            {
                UltraMessageBox.Show("Error en operación:" + Environment.NewLine + 
                    objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            grdData.DataSource = objData;
        }
        private void SetDatos(bool enable)
        {
            grbDescripcion.Enabled = enable;
            if (enable)
            {
                txtNombreUnidad.Clear();
                cbUndInter.Value = "-1";
                txtNombreUnidad.Focus();
            }
        }
        #endregion

        #region Event UI
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            SetDatos(true);
            _modeDetalle = "New";
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow != null)
            {
                SetDatos(true);
                _datahierarchyDto = (datahierarchyDto)grdData.ActiveRow.ListObject;
                txtNombreUnidad.Text = _datahierarchyDto.v_Value1;
                cbUndInter.Value = _datahierarchyDto.i_ParentItemId.ToString();
                _modeDetalle = "Edit";
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null) return;
            if (UltraMessageBox.Show("¿Desea eliminar esta Unidad de Medida?", "Eliminar") !=
                DialogResult.OK) return;
            var intIdItem = ((datahierarchyDto)grdData.ActiveRow.ListObject).i_ItemId;
            var objOperationResult = new OperationResult();
            _objDataHierarchyBl.DeleteDataHierarchy(ref objOperationResult, IdGroupUnidadMedida, intIdItem, Globals.ClientSession.GetAsList());
            BindGrid();
            SetDatos(false);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (uvDatos.Validate(true, false).IsValid)
            {
                int idUnidad;
                if (cbUndInter.Value == null || !int.TryParse((string)cbUndInter.Value, out idUnidad))
                {
                    cbUndInter.PerformAction(Infragistics.Win.UltraWinGrid.UltraComboAction.Dropdown);
                    return;
                }
                var objOperationResult = new OperationResult();
                if (_modeDetalle == "New")
                {
                    _datahierarchyDto = new datahierarchyDto
                    {
                        i_ItemId = _objDataHierarchyBl.ObtenerMaxino(ref objOperationResult, IdGroupUnidadMedida)
                    };
                }
                
                    _datahierarchyDto.i_GroupId = IdGroupUnidadMedida;
                    _datahierarchyDto.v_Value1 = txtNombreUnidad.Text;
                    _datahierarchyDto.i_ParentItemId = idUnidad;

                if (_modeDetalle == "New")
                    _objDataHierarchyBl.AddDataHierarchy(ref objOperationResult, _datahierarchyDto, Globals.ClientSession.GetAsList());
                else if (_modeDetalle == "Edit")
                    _objDataHierarchyBl.UpdateDataHierarchy(ref objOperationResult, _datahierarchyDto, Globals.ClientSession.GetAsList());

                if (objOperationResult.Success == 1) 
                {
                    BindGrid();
                    SetDatos(false);
                }
                else 
                    UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

    }
}
