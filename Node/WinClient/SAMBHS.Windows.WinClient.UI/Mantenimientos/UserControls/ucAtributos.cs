using System;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos.UserControls
{
    public partial class ucAtributos : UserControl
    {
        #region Fields
        private readonly DatahierarchyBL _objDataHierarchyBl = new DatahierarchyBL();
        #endregion

        #region Properties
        public int GroupId { get; set; }
        #endregion

        #region Construct
        public ucAtributos()
        {
            InitializeComponent();
            Load += delegate
            {
                BackColor = new GlobalFormColors().FormColor; 
            };
        }
        #endregion

        #region Methods

        private void BindGrid()
        {
            var objOperationResult = new OperationResult();
            var objData = _objDataHierarchyBl.GetDataHierarchiesPagedAndFiltered(ref objOperationResult, "i_ItemId asc", null, GroupId);

            if (objOperationResult.Success != 1)
                UltraMessageBox.Show("Error en operación:" + Environment.NewLine + objOperationResult.ExceptionMessage,
                                     "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            grdData.DataSource = objData;
            grbDescripcion.Enabled = false;
        }
        private void CargarDetalle(int pintIdItem)
        {
            var objOperationResult = new OperationResult();
            var dh = _objDataHierarchyBl.GetDataHierarchy(ref objOperationResult, GroupId, pintIdItem);

            txtTitulo.Text = dh.v_Value1;
            grbDescripcion.Enabled = true;
            Tag = dh;
        }
        #endregion

        #region Events
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTitulo.Text))
            {
                UltraMessageBox.Show("Falta Descripcion", Text, Icono:MessageBoxIcon.Warning);
                txtTitulo.Focus();
                return;
            }
            var objOperationResult = new OperationResult();
            if (Tag == null)
            {
                var dto = new datahierarchyDto
                {
                    i_ItemId = _objDataHierarchyBl.ObtenerMaxino(ref objOperationResult, GroupId),
                    v_Value1 = txtTitulo.Text,
                    i_GroupId = GroupId
                };
                _objDataHierarchyBl.AddDataHierarchy(ref objOperationResult, dto, Globals.ClientSession.GetAsList());
            }
            else
            {
                var dto = (datahierarchyDto)Tag;
                dto.v_Value1 = txtTitulo.Text;
                _objDataHierarchyBl.UpdateDataHierarchy(ref objOperationResult, dto, Globals.ClientSession.GetAsList());
            }
            grbDescripcion.Enabled = false;
            if (objOperationResult.Success == 1)
            {
                BindGrid();
            }
            else
            {
                UltraMessageBox.Show(objOperationResult.ErrorMessage, "Error al Crear", Icono: MessageBoxIcon.Exclamation);
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            grbDescripcion.Enabled = true;
            txtTitulo.Clear();
            txtTitulo.Focus();
            Tag = null;
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if(grdData.ActiveRow == null) return;
            CargarDetalle(((datahierarchyDto)grdData.ActiveRow.ListObject).i_ItemId);
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null) return;
            var obj = (datahierarchyDto)grdData.ActiveRow.ListObject;
            if (UltraMessageBox.Show("¿Desea eliminar " + obj.v_Value1 + " ?", Text, MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes) return;
            var opRes = new OperationResult();
            _objDataHierarchyBl.DeleteDataHierarchy(ref opRes, GroupId, obj.i_ItemId, Globals.ClientSession.GetAsList());
            if (opRes.Success == 1)
            {
                BindGrid();
            }
            else
            {
                UltraMessageBox.Show(opRes.ErrorMessage, "Error al Eliminar", Icono:MessageBoxIcon.Exclamation);
            }
        }
        private void btnMarcaBuscar_Click(object sender, EventArgs e)
        {
            BindGrid();
        }


        private void grdData_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            var obj = (datahierarchyDto)grdData.ActiveRow.ListObject;
            CargarDetalle(obj.i_ItemId);
        }
        #endregion

    }
}
