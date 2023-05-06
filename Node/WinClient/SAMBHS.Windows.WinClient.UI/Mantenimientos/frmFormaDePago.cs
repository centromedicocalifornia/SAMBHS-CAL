using System;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Almacen.BL;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.BE;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmFormaDePago : Form
    {
        #region Fields
        private datahierarchyDto _datahierarchyDto;
        private readonly DatahierarchyBL _objDataHierarchyBl = new DatahierarchyBL();
        private const short IdGrupo = 46;
        private string _mode = "New"; 
        #endregion

        public frmFormaDePago(string arg)
        {
            InitializeComponent();
        }

        private void frmFormaDePago_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;

            #region Fill Combos
            var objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboList(cboMedioPago, "Value1", "Value2", _objDataHierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 44, "v_Value2"), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(cboTipoDocRef, "Value2", "Id", new DocumentoBL().ObtenDocumentosParaComboGridTesoreria(ref objOperationResult, null), DropDownListAction.Select);
            var almacenes = new NodeWarehouseBL().ObtenerAlmacenesParaComboAll(ref objOperationResult, null);
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", almacenes, DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(ucAlmacen, "Value1", "Id", almacenes, DropDownListAction.Select);
            cboMedioPago.Value = cboTipoDocRef.Value = cboAlmacen.Value = "-1";
            #endregion

            BindGrid();

            #region LoadEvents
            grdDetalle.KeyDown += (_, ev) =>
            {
                if (ev.KeyCode == Keys.Enter) btnEditar_Click(_, null);
                else if (ev.KeyCode == Keys.Delete) btnEliminar_Click(_, null);
            };
            grdDetalle.DoubleClick +=  btnEditar_Click;
            cboMedioPago.KeyDown += Combo_KeyDown;
            cboTipoDocRef.KeyDown += Combo_KeyDown;
            #endregion
        }
        //Ejecuta dropDown al presionar Ctrl+ Down
        private void Combo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Down)
                ((UltraCombo) sender).PerformAction(UltraComboAction.Dropdown);
        }

        #region Events Button
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            txtValor1.Clear();
            txtValor1.Focus();
            cboMedioPago.Value = "-1";
            cboTipoDocRef.Value = "-1";
            _mode = "New";
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            var objOperationResult = new OperationResult();
            if (uvDetalle.Validate(true, false).IsValid)
            {
                if (txtValor1.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese el Titulo", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (_mode == "New")
                {
                    _datahierarchyDto = new datahierarchyDto
                    {
                        i_ItemId = _objDataHierarchyBl.ObtenerMaxino(ref objOperationResult, IdGrupo)
                    };
                }
                _datahierarchyDto.i_GroupId = IdGrupo;
                _datahierarchyDto.i_Sort =  0;
                _datahierarchyDto.v_Value1 = txtValor1.Text;
                _datahierarchyDto.v_Value2 = cboMedioPago.Value == null || cboMedioPago.Value.ToString() == "-1" ? "" : cboMedioPago.Value.ToString();
                _datahierarchyDto.v_Value4 = cboAlmacen.Value == null || cboAlmacen.Value.ToString() == "-1"
                                            ? string.Empty
                                            : cboAlmacen.Value.ToString();
                _datahierarchyDto.v_Field = cboTipoDocRef.Value == null || cboTipoDocRef.Value.ToString() == "-1" ? "" : cboTipoDocRef.Value.ToString();
                _datahierarchyDto.i_Header = 0;

                if (_mode == "New")
                    // Save the data
                    _objDataHierarchyBl.AddDataHierarchy(ref objOperationResult, _datahierarchyDto, Globals.ClientSession.GetAsList());

                else if (_mode == "Edit")
                    // Save the data
                    _objDataHierarchyBl.UpdateDataHierarchy(ref objOperationResult, _datahierarchyDto, Globals.ClientSession.GetAsList());

                //// Analizar el resultado de la operación
                if (objOperationResult.Success == 1)  // Operación sin error
                {
                    BindGrid();
                    MantenerSeleccion(_datahierarchyDto.i_ItemId.ToString());
                    btnEditar.Enabled = false;
                    btnEliminar.Enabled = false;
                }
                else  // Operación con error
                {
                    UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Se queda en el formulario.
                }
                txtValor1.Text = string.Empty;
                cboMedioPago.Value = "-1";
                cboTipoDocRef.Value= "-1";
                cboAlmacen.Value = "-1";
                _mode = "New";
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (grdDetalle.Selected.Rows.Count > 0)
            {
                var intIdItem = int.Parse(grdDetalle.Selected.Rows[0].Cells["i_ItemId"].Value.ToString());
                CargarDetalle(IdGrupo, intIdItem);
                txtValor1.Focus();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            // Obtener los IDs de la fila seleccionada
            var Result = UltraMessageBox.Show("¿Está seguro de eliminar este registro?:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (Result == DialogResult.Yes)
            {
                // Delete the item              
                int intIdItem = int.Parse(grdDetalle.Selected.Rows[0].Cells["i_ItemId"].Value.ToString());
                _objDataHierarchyBl.DeleteDataHierarchy(ref objOperationResult, IdGrupo, intIdItem, Globals.ClientSession.GetAsList());
                BindGrid();
                btnEditar.Enabled = false;
                btnEliminar.Enabled = false;

            }
        }
        #endregion

        #region Grilla
        private void BindGrid()
        {
            OperationResult objOperationResult = new OperationResult();
            var objData = _objDataHierarchyBl.GetDataHierarchiesPagedAndFiltered(ref objOperationResult, "", "", IdGrupo);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            grdDetalle.DataSource = objData;
        }
        private void grdDetalle_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point point = new System.Drawing.Point(e.X, e.Y);
                Infragistics.Win.UIElement uiElement = ((UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

                if (uiElement == null || uiElement.Parent == null)
                    return;

                var row = (UltraGridRow)uiElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridRow));

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
        private void MantenerSeleccion(string ValorSeleccionado)
        {
            foreach (var row in grdDetalle.Rows)
            {
                if (row.Cells["i_ItemId"].Text == ValorSeleccionado)
                {
                    grdDetalle.ActiveRow = row;
                    grdDetalle.ActiveRow.Selected = true;
                    break;
                }
            }
        }
        #endregion
        
        /// <summary>
        /// Carga el Detalle de un item para edicion
        /// </summary>
        /// <param name="pintIdGrupo">Id del Grupo</param>
        /// <param name="pintIdItem">Id del Item</param>
        void CargarDetalle(int pintIdGrupo, int pintIdItem)
        {
            var objOperationResult = new OperationResult();
             _datahierarchyDto = new datahierarchyDto();

            _datahierarchyDto = _objDataHierarchyBl.GetDataHierarchy(ref objOperationResult, pintIdGrupo, pintIdItem);
            //txtDetalleIdItem.Text = _datahierarchyDto.i_ItemId.ToString();
            txtValor1.Text = _datahierarchyDto.v_Value1;
            cboMedioPago.Value = _datahierarchyDto.v_Value2 == null || _datahierarchyDto.v_Value2.Equals(string.Empty) ? "-1" : _datahierarchyDto.v_Value2;
            cboTipoDocRef.Value = _datahierarchyDto.v_Field == null || _datahierarchyDto.v_Field.Equals(string.Empty) ? "-1" : _datahierarchyDto.v_Field;
            cboAlmacen.Value = _datahierarchyDto.v_Value4 == null || _datahierarchyDto.v_Value4.Equals(string.Empty) ? "-1" : _datahierarchyDto.v_Value4;
            _mode = "Edit";
        }

    }
}
