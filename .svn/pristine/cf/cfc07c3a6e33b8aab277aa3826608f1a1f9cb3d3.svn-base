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
using SAMBHS.Venta.BL;
using SAMBHS.Almacen.BL;
using SAMBHS.Compra.BL;
using Infragistics.Win.UltraWinGrid;
using System.Text.RegularExpressions;
using Infragistics.Win.UltraWinMaskedEdit;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmEstablecimiento : Form
    {
        EstablecimientoBL _objEstablecimientoBL = new EstablecimientoBL();

        establecimientoDto _EstablecimientoDto;
        establecimientodetalleDto _establecimientodetalleDto;
        establecimientoalmacenDto _establecimientoalmacenDto;
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL();
        List<GridKeyValueDTO> _ListadoComboDocumentos = new List<GridKeyValueDTO>();
        string _strFilterExpression;
        string _mode;
        string _modeDetalle;
        string _modeAlmacen;
        int intIdEstablecimiento;
        int intIdEstablecimientoDetalle;
        int intIdEstablecimientoAlmacen;

        #region Cargar Página

        public frmEstablecimiento(string parametro)
        {
            InitializeComponent();
        }

        private void frmEstablecimiento_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            _strFilterExpression = null;
            gbGrupo.Enabled = false;
            gbDetalle.Visible = false;
            gbAlmacenes.Visible = false;

            OperationResult objOperationResult = new OperationResult();
            _ListadoComboDocumentos = _objDocumentoBL.ObtenDocumentosParaComboGrid(ref objOperationResult, null, 0, 1);
            Utils.Windows.LoadUltraComboEditorList(cboDetalleAlmacen, "Value1", "Id", _objNodeWarehouseBL.ObtenerAlmacenesParaCombo(ref objOperationResult, null, Globals.ClientSession.i_IdEstablecimiento.Value), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboEstablecimientoAlmacen, "Value1", "Id", _objNodeWarehouseBL.ObtenerAlmacenesParaComboAll(ref objOperationResult, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboDetalleDocumento, "Value1", "Id", _objDocumentoBL.ObtenDocumentosParaCombo(ref objOperationResult, null, 0,2), DropDownListAction.Select);
        }
        #endregion

        #region Bandeja de Búsqueda
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            // Get the filters from the UI
            List<string> Filters = new List<string>();
            if (!string.IsNullOrEmpty(txtEstablecimiento.Text)) Filters.Add("v_Nombre.Contains(\"" + txtEstablecimiento.Text.Trim().ToUpper() + "\")");
            Filters.Add("i_Eliminado==0");
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
            txtGrupoNombre.Focus();
            btnEditar.Enabled = false;
            btnEliminar.Enabled = false;
            gbGrupo.Visible = true;
            gbDetalle.Visible = false;
            gbAlmacenes.Visible = false;
            btnEditar.Enabled = false;
            btnEliminar.Enabled = false;

            _mode = "New";
            LimpiarGrupo();
            gbGrupo.Enabled = true;
            btnGrabar.Enabled = true;
        }

        private void BindGrid()
        {
            var objData = GetData("v_Nombre ASC", _strFilterExpression);

            grdData.DataSource = objData;
        }

        private List<establecimientoDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _objEstablecimientoBL.GetestablecimientoPagedAndFiltered(ref objOperationResult, pstrSortExpression, pstrFilterExpression);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            return _objData;
        }

        void LimpiarGrupo()
        {
            txtEstablecimiento.Text = string.Empty;
            txtGrupoNombre.Text = string.Empty;
            txtGrupoDireccion.Text = string.Empty;
            txtCentroCosto.Clear();
        }
        void LimpiarGrupoDetalle()
        {
            cboDetalleDocumento.Value = "-1";
            cboDetalleAlmacen.Value = "-1";
            txtDetalleSerie.Text = string.Empty;
            txtDetalleCorrelativoDocIni.Text = string.Empty;
            uckImpresionPrevia.Checked = false;
            txtNombreImpresora.Clear();
        }


        void LimpiarGrupoAlmacen()
        {
            cboEstablecimientoAlmacen.Value = "-1";

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
                    btnAsignarAlmacen.Enabled = true;

                    intIdEstablecimiento = int.Parse(grdData.Selected.Rows[0].Cells["i_IdEstablecimiento"].Value.ToString());

                    gbDetalle.Visible = false;
                    gbGrupo.Visible = true;
                    gbAlmacenes.Visible = false;
                    CargarGrupo(intIdEstablecimiento);
                }
                else
                {
                    btnCrearDetalle.Enabled = false;
                    btnEliminarGrupo.Enabled = false;
                    btnAsignarAlmacen.Enabled = false;
                }
            }
        }

        void CargarGrupo(int pintIdEstablecimiento)
        {
            OperationResult objOperationResult = new OperationResult();
            _EstablecimientoDto = new establecimientoDto();

            _EstablecimientoDto = _objEstablecimientoBL.Getestablecimiento(ref objOperationResult, pintIdEstablecimiento);

            txtGrupoNombre.Text = _EstablecimientoDto.v_Nombre;
            txtGrupoDireccion.Text = _EstablecimientoDto.v_Direccion;
            txtCentroCosto.Tag = _EstablecimientoDto.i_IdCentroCosto;
            txtCentroCosto.Text = _EstablecimientoDto.CentroCosto;

            gbGrupo.Enabled = true;
            btnGrabar.Enabled = true;
            _mode = "Edit";
        }
        private void btnCrearDetalle_Click(object sender, EventArgs e)
        {
            _modeDetalle = "New";
            gbGrupo.Visible = false;
            gbAlmacenes.Visible = false;
            gbDetalle.Visible = true;
            gbDetalle.Dock = DockStyle.Fill;
            btnAgregar.Enabled = true;
            LimpiarGrupoDetalle();


            int intIdEstablecimiento = int.Parse(grdData.Selected.Rows[0].Cells["i_IdEstablecimiento"].Value.ToString());

            OperationResult objOperationResult = new OperationResult();
            var _objData = _objEstablecimientoBL.GetestablecimientodetallePagedAndFiltered(ref objOperationResult, "", "", intIdEstablecimiento);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            grdDetalle.DataSource = _objData;

            Utils.Windows.LoadUltraComboEditorList(cboDetalleAlmacen, "Value1", "Id", _objNodeWarehouseBL.ObtenerAlmacenesParaCombo(ref objOperationResult, null, intIdEstablecimiento), DropDownListAction.Select);
        }

        void CargarGrillaDetalle(int intIdEstablecimiento_)
        {
            grdDetalle.DataSource = GetData("", "", intIdEstablecimiento_);
        }

        private List<establecimientodetalleDto> GetData(string pstrSortExpression, string pstrFilterExpression, int intIdEstablecimiento_)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _objEstablecimientoBL.GetestablecimientodetallePagedAndFiltered(ref objOperationResult, pstrSortExpression, pstrFilterExpression, intIdEstablecimiento_);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }

        private void btnEliminarGrupo_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            // Obtener los IDs de la fila seleccionada

            DialogResult Result = UltraMessageBox.Show("¿Está seguro de eliminar este registro?:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (Result == System.Windows.Forms.DialogResult.Yes)
            {
                // Delete the item              
                _objEstablecimientoBL.DeleteEstablecimiento(ref objOperationResult, intIdEstablecimiento, Globals.ClientSession.GetAsList());

                btnBuscar_Click(sender, e);
                LimpiarGrupo();
                gbGrupo.Enabled = false;
                btnEliminarGrupo.Enabled = false;
                btnCrearDetalle.Enabled = false;
                btnAgregarEstablecimientoAlmacen.Enabled = false;

            }
        }
        #endregion

        #region Edición Cabecera
        private void btnGrabar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (uvGrupo.Validate("Grupo", true, false).IsValid)
            {
                if (txtGrupoNombre.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese un Nombre.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (txtGrupoDireccion.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese una Dirección.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (_mode == "New")
                {
                    _EstablecimientoDto = new establecimientoDto();


                    _EstablecimientoDto.v_Nombre = txtGrupoNombre.Text;
                    _EstablecimientoDto.v_Direccion = txtGrupoDireccion.Text;
                    _EstablecimientoDto.i_IdCentroCosto = txtCentroCosto.Tag != null ? txtCentroCosto.Tag.ToString() : string.Empty;


                    // Save the data
                    _objEstablecimientoBL.Addestablecimiento(ref objOperationResult, _EstablecimientoDto, Globals.ClientSession.GetAsList());

                }
                else if (_mode == "Edit")
                {
                    _EstablecimientoDto.i_IdEstablecimiento = intIdEstablecimiento;
                    _EstablecimientoDto.v_Nombre = txtGrupoNombre.Text;
                    _EstablecimientoDto.v_Direccion = txtGrupoDireccion.Text;
                    _EstablecimientoDto.i_IdCentroCosto = txtCentroCosto.Tag != null ? txtCentroCosto.Tag.ToString() : string.Empty;


                    _objEstablecimientoBL.Updateestablecimiento(ref objOperationResult, _EstablecimientoDto, Globals.ClientSession.GetAsList());

                }
                //// Analizar el resultado de la operación
                if (objOperationResult.Success == 1)  // Operación sin error
                {
                    LimpiarGrupo();
                    btnBuscar_Click(sender, e);
                    btnEliminarGrupo.Enabled = false;
                    btnCrearDetalle.Enabled = false;
                    btnAsignarAlmacen.Enabled = false;
                }
                else  // Operación con error
                {
                    UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Se queda en el formulario.
                }

            }
        }

        #endregion

        #region Edición Detalle

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (uvGrupo.Validate("Detalle", true, false).IsValid)
            {

                if (txtDetalleSerie.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese Serie.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrEmpty(txtDetalleSerie.Text.Trim()))
                {
                    UltraMessageBox.Show("Por favor ingrese Serie válido.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (txtDetalleCorrelativoDocIni.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese Correlativo Inicial.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (int.Parse(txtDetalleCorrelativoDocIni.Text) == 0)
                {
                    UltraMessageBox.Show("Por favor el Correlativo debe ser mayor o igual a 1.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (uckDocumentoPredeterminado.Checked)
                {

                    var DocumentoPredeterminado = _objEstablecimientoBL.ComprobarExistenciaComprobantePredeterminado(ref objOperationResult, intIdEstablecimiento, int.Parse(cboDetalleDocumento.Value.ToString()), _establecimientodetalleDto, int.Parse(cboDetalleAlmacen.Value.ToString()));
                    if (DocumentoPredeterminado != "")
                    {
                        UltraMessageBox.Show("La " + cboDetalleDocumento.Text + " serie " + DocumentoPredeterminado + "se encuentra registrado el establecimiento como predeterminado ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;

                    }
                }

                if (ComprobarExistenciaSerie() == true && _modeDetalle == "New")
                {


                    _objEstablecimientoBL = new EstablecimientoBL();
                    string NombreEstableciliento = _objEstablecimientoBL.GetestablecimientoDetalleNombreAlmacen(ref objOperationResult, txtDetalleSerie.Text.ToString().Trim(), int.Parse(cboDetalleDocumento.Value.ToString()));
                    UltraMessageBox.Show("El nro de serie " + txtDetalleSerie.Text.ToString().Trim() + " ya se encuentra registrado el establecimiento " + NombreEstableciliento + "." + cboDetalleAlmacen.SelectedText.ToString(), "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                    if (ComprobarExistenciaCorrelativoDocumento() == true && _modeDetalle == "New")
                    {
                        UltraMessageBox.Show("El Nro de Serie y Documento ingresado ya le Corresponden al Almacen " + cboDetalleAlmacen.SelectedText.ToString(), "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                }

                if (_modeDetalle == "New")
                {

                    _EstablecimientoDto = new establecimientoDto();
                    _establecimientodetalleDto = new establecimientodetalleDto();

                    _establecimientodetalleDto.i_IdEstablecimiento = intIdEstablecimiento;
                    _establecimientodetalleDto.i_IdTipoDocumento = int.Parse(cboDetalleDocumento.Value.ToString());
                    _establecimientodetalleDto.v_Serie = txtDetalleSerie.Text.Trim();
                    _establecimientodetalleDto.i_Correlativo = int.Parse(txtDetalleCorrelativoDocIni.Text);
                    _establecimientodetalleDto.i_Almacen = int.Parse(cboDetalleAlmacen.Value.ToString());
                    _establecimientodetalleDto.i_ImpresionVistaPrevia = uckImpresionPrevia.Checked ? 1 : 0;
                    _establecimientodetalleDto.v_NombreImpresora = txtNombreImpresora.Text.Trim();
                    _establecimientodetalleDto.i_DocumentoPredeterminado = uckDocumentoPredeterminado.Checked ? 1 : 0;
                    _establecimientodetalleDto.i_NumeroItems = txtMaxNumeroItems.Text ==string.Empty ? 0 : int.Parse(txtMaxNumeroItems.Text); 
                    // Save the data
                    _objEstablecimientoBL.AddestablecimientoDetalle(ref objOperationResult, _establecimientodetalleDto, Globals.ClientSession.GetAsList());


                }
                else if (_modeDetalle == "Edit")
                {

                    _establecimientodetalleDto.i_IdEstablecimientoDetalle = intIdEstablecimientoDetalle;
                    _establecimientodetalleDto.i_IdEstablecimiento = intIdEstablecimiento;
                    _establecimientodetalleDto.i_IdTipoDocumento = int.Parse(cboDetalleDocumento.Value.ToString());
                    _establecimientodetalleDto.v_Serie = txtDetalleSerie.Text.Trim();
                    _establecimientodetalleDto.i_Correlativo = int.Parse(txtDetalleCorrelativoDocIni.Text);
                    _establecimientodetalleDto.i_Almacen = int.Parse(cboDetalleAlmacen.Value.ToString());
                    _establecimientodetalleDto.i_ImpresionVistaPrevia = uckImpresionPrevia.Checked ? 1 : 0;
                    _establecimientodetalleDto.v_NombreImpresora = txtNombreImpresora.Text.Trim();
                    _establecimientodetalleDto.i_DocumentoPredeterminado = uckDocumentoPredeterminado.Checked ? 1 : 0;
                    _establecimientodetalleDto.i_NumeroItems = txtMaxNumeroItems.Text == string.Empty ? 0 : int.Parse(txtMaxNumeroItems.Text);
                    // Save the data
                    _objEstablecimientoBL.UpdateestablecimientoDetalle(ref objOperationResult, _establecimientodetalleDto, Globals.ClientSession.GetAsList());

                }
                //// Analizar el resultado de la operación
                if (objOperationResult.Success == 1)  // Operación sin error
                {
                    CargarGrillaDetalle(intIdEstablecimiento);
                    btnEditar.Enabled = false;
                    btnEliminar.Enabled = false;
                    Globals.ListaEstablecimientoDetalle = _objDocumentoBL.ListaEstablecimientoDetalle();
                }
                else  // Operación con error
                {
                    UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Se queda en el formulario.
                }

                txtDetalleSerie.Text = string.Empty;
                txtDetalleCorrelativoDocIni.Text = string.Empty;
                cboDetalleDocumento.Value = "-1";
                cboDetalleAlmacen.Value = "-1";
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
                int intIdItem = int.Parse(grdDetalle.Selected.Rows[0].Cells["i_IdEstablecimientoDetalle"].Value.ToString());
                _objEstablecimientoBL.DeleteestablecimientoDetalle(ref objOperationResult, intIdItem, Globals.ClientSession.GetAsList());
                CargarGrillaDetalle(intIdEstablecimiento);
                btnEditar.Enabled = false;
                btnEliminar.Enabled = false;
                Globals.ListaEstablecimientoDetalle = _objDocumentoBL.ListaEstablecimientoDetalle();

            }
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

        private void btnEditar_Click(object sender, EventArgs e)
        {
            // intIdEstablecimientoDetalle = int.Parse(grdDetalle.Selected.Rows[0].Cells["i_IdEstablecimientoDetalle"].Value.ToString());
            intIdEstablecimientoDetalle = int.Parse(grdDetalle.Rows[grdDetalle.ActiveRow.Index].Cells["i_IdEstablecimientoDetalle"].Value.ToString().Trim());
            CargarDetalle(intIdEstablecimientoDetalle);
        }

        void CargarDetalle(int intIdEstablecimientoDetalle_)
        {
            OperationResult objOperationResult = new OperationResult();
            int IdEstablecimiento = grdDetalle.ActiveRow != null ? int.Parse(grdDetalle.ActiveRow.Cells["i_IdEstablecimiento"].Value.ToString()) : 0;
            _establecimientodetalleDto = _objEstablecimientoBL.GetestablecimientoDetalle(ref objOperationResult, intIdEstablecimientoDetalle_);

            Utils.Windows.LoadUltraComboEditorList(cboDetalleAlmacen, "Value1", "Id", _objNodeWarehouseBL.ObtenerAlmacenesParaCombo(ref objOperationResult, null, IdEstablecimiento), DropDownListAction.Select);

            cboDetalleDocumento.Value = _establecimientodetalleDto.i_IdTipoDocumento.ToString();
            cboDetalleAlmacen.Value = _establecimientodetalleDto.i_Almacen.ToString();

            txtDetalleSerie.Text = _establecimientodetalleDto.v_Serie.Trim();
            txtDetalleCorrelativoDocIni.Text = _establecimientodetalleDto.i_Correlativo.ToString();
            Utils.Windows.FijarFormatoUltraTextBox(txtDetalleCorrelativoDocIni, "{0:00000000}");
            txtDetalleCorrelativoDocIni.Text = txtDetalleCorrelativoDocIni.Text.Trim();
            uckImpresionPrevia.Checked = _establecimientodetalleDto.i_ImpresionVistaPrevia == null || _establecimientodetalleDto.i_ImpresionVistaPrevia.Value == 0 ? false : true;
            txtNombreImpresora.Text = _establecimientodetalleDto.v_NombreImpresora;
            uckDocumentoPredeterminado.Checked = _establecimientodetalleDto.i_DocumentoPredeterminado == null || _establecimientodetalleDto.i_DocumentoPredeterminado == 0 ? false : true;
            txtMaxNumeroItems.Text = _establecimientodetalleDto.i_NumeroItems.ToString();
            _modeDetalle = "Edit";
        }

        void CargarAlmacen(int pintIdEstablecimientoAlmacen)
        {
            OperationResult objOperationResult = new OperationResult();

            _establecimientoalmacenDto = _objEstablecimientoBL.GetestablecimientoAlmacen(ref objOperationResult, pintIdEstablecimientoAlmacen);
            cboEstablecimientoAlmacen.Value = _establecimientoalmacenDto.i_IdEstablecimientoAlmacen.ToString();
            _modeAlmacen = "Edit";

        }

        private void txtDetalleSerie_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Utils.Windows.NumeroEnteroUltraTextBox(txtDetalleSerie, e);
        }

        private void txtDetalleCorrelativoDocIni_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtDetalleCorrelativoDocIni, e);
        }

        private void txtDetalleCorrelativoDocIni_Validated(object sender, EventArgs e)
        {

            Utils.Windows.FijarFormatoUltraTextBox(txtDetalleCorrelativoDocIni, "{0:00000000}");
            if (cboDetalleDocumento.Value != null)
            {
                if (cboDetalleDocumento.Value.ToString() == "3" | cboDetalleDocumento.Value.ToString() == "12")
                {
                    txtDetalleCorrelativoDocIni.Text = txtDetalleCorrelativoDocIni.Text;
                }
            }
        }

        private void txtDetalleSerie_Validated(object sender, EventArgs e)
        {
            int nro = 0;
            if (int.TryParse(txtDetalleSerie.Text, out nro))
                Utils.Windows.FijarFormatoUltraTextBox(txtDetalleSerie, "{0:0000}");

            if (cboDetalleDocumento.Value != null)
            {
                if (cboDetalleDocumento.Value.ToString() == "3" || cboDetalleDocumento.Value.ToString() == "12")
                {
                    txtDetalleSerie.Text = txtDetalleSerie.Text;
                }
            }
        }

        private bool ComprobarExistenciaCorrelativoDocumento()
        {
            OperationResult objOperationResult = new OperationResult();
            if (_objEstablecimientoBL.ComprobarExistenciaCorrelativoDocumento(ref objOperationResult, int.Parse(cboDetalleAlmacen.Value.ToString()), txtDetalleSerie.Text.ToString(), int.Parse(cboDetalleDocumento.Value.ToString())) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool ComprobarExistenciaSerie()
        {
            OperationResult objOperationResult = new OperationResult();
            if (_objEstablecimientoBL.ComprobarExistenciaSerie(ref objOperationResult, intIdEstablecimiento, txtDetalleSerie.Text.ToString().Trim(), int.Parse(cboDetalleDocumento.Value.ToString())) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void txtDetalleCorrelativoDocIni_TextChanged(object sender, EventArgs e)
        {

        }
        #endregion

        #region Asignar Alamcenes
        private void btnAgregarEstablecimientoAlmacen_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (uvGrupo.Validate("Almacen", true, false).IsValid)
            {
                if (_modeAlmacen == "New")
                {

                    if (_objEstablecimientoBL.ValidarDuplicidadAlmacen(intIdEstablecimiento, int.Parse(cboEstablecimientoAlmacen.Value.ToString())))
                    {
                        UltraMessageBox.Show("El almacén: " + cboEstablecimientoAlmacen.Text.ToUpper() + " ya está asignado en el establecimiento", "¡ ERROR DE VALIDACIÓN !", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (_objEstablecimientoBL.ValidarSiAlmacenPerteneceAOtroEstablecimiento(intIdEstablecimiento, int.Parse(cboEstablecimientoAlmacen.Value.ToString())))
                    {
                        UltraMessageBox.Show("El almacén: " + cboEstablecimientoAlmacen.Text.ToUpper() + " ya se encuentra asignado en otro establecimiento", "¡ ERROR DE VALIDACIÓN !", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    _EstablecimientoDto = new establecimientoDto();
                    _establecimientoalmacenDto = new establecimientoalmacenDto();

                    _establecimientoalmacenDto.i_IdEstablecimiento = intIdEstablecimiento;
                    _establecimientoalmacenDto.i_IdAlmacen = int.Parse(cboEstablecimientoAlmacen.Value.ToString());

                    // Save the data
                    _objEstablecimientoBL.AddestablecimientoAlmacen(ref objOperationResult, _establecimientoalmacenDto, Globals.ClientSession.GetAsList());

                }
                else if (_modeAlmacen == "Edit")
                {

                    _establecimientoalmacenDto.i_IdEstablecimientoAlmacen = intIdEstablecimientoDetalle;
                    _establecimientoalmacenDto.i_IdEstablecimiento = intIdEstablecimiento;
                    _establecimientoalmacenDto.i_IdAlmacen = int.Parse(cboEstablecimientoAlmacen.Value.ToString());
                    // Save the data
                    _objEstablecimientoBL.UpdateestablecimientoDetalle(ref objOperationResult, _establecimientodetalleDto, Globals.ClientSession.GetAsList());

                }
                //// Analizar el resultado de la operación
                if (objOperationResult.Success == 1)  // Operación sin error
                {

                    CargarGrillaAlmacen(intIdEstablecimiento);
                    btnEditar.Enabled = false;
                    btnEliminar.Enabled = false;
                }
                else  // Operación con error
                {
                    UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Se queda en el formulario.
                }


                cboEstablecimientoAlmacen.Value= "-1";
                _modeAlmacen = "New";
            }
        }

        private void btnEditarAlmacen_Click(object sender, EventArgs e)
        {
            intIdEstablecimientoAlmacen = int.Parse(grdAlmacen.Selected.Rows[0].Cells["i_IdEstablecimientoAlmacen"].Value.ToString());
            CargarAlmacen(intIdEstablecimientoAlmacen);
        }

        private void btnEliminarAlmacen_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            // Obtener los IDs de la fila seleccionada

            DialogResult Result = UltraMessageBox.Show("¿Está seguro de eliminar este registro?:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (Result == System.Windows.Forms.DialogResult.Yes)
            {
                // Delete the item              
                int intIdItem = int.Parse(grdAlmacen.Selected.Rows[0].Cells["i_IdEstablecimientoAlmacen"].Value.ToString());
                _objEstablecimientoBL.DeleteestablecimientoAlmacen(ref objOperationResult, intIdItem, Globals.ClientSession.GetAsList());

                CargarGrillaAlmacen(intIdEstablecimiento);
                btnEditar.Enabled = false;
                btnEliminar.Enabled = false;

            }
        }

        void CargarGrillaAlmacen(int intIdEstablecimiento_)
        {
            grdAlmacen.DataSource = GetDataAlmacen("", "", intIdEstablecimiento_);
        }

        private List<establecimientoalmacenDto> GetDataAlmacen(string pstrSortExpression, string pstrFilterExpression, int intIdEstablecimiento_)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _objEstablecimientoBL.GetEstablecimientoAlmacenPagedAndFiltered(ref objOperationResult, pstrSortExpression, pstrFilterExpression, intIdEstablecimiento_);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }


        private void btnAsignarAlmacen_Click(object sender, EventArgs e)
        {
            _modeAlmacen = "New";
            gbGrupo.Visible = false;
            gbDetalle.Visible = false;
            gbAlmacenes.Visible = true;
            gbAlmacenes.Dock = DockStyle.Fill;
            btnAgregarEstablecimientoAlmacen.Enabled = true;
            LimpiarGrupoAlmacen();

            int intIdEstablecimiento = int.Parse(grdData.Selected.Rows[0].Cells["i_IdEstablecimiento"].Value.ToString());

            OperationResult objOperationResult = new OperationResult();
            var _objData = _objEstablecimientoBL.GetEstablecimientoAlmacenPagedAndFiltered(ref objOperationResult, "", "", intIdEstablecimiento);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            grdAlmacen.DataSource = _objData;


        }

        private void grdAlmacen_MouseDown(object sender, MouseEventArgs e)
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
                    grdAlmacen.Rows[row.Index].Selected = true;

                    btnEditarAlmacen.Enabled = true;
                    btnEliminarAlmacen.Enabled = true;
                }
                else
                {
                    btnEditarAlmacen.Enabled = false;
                    btnEliminarAlmacen.Enabled = false;
                }
            }
        }

        #endregion

        private void grdData_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {
            if (e.Cell.Column.Key == "i_IdCentroCosto")
            {

            }
        }

        private void ultraTextEditor1_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscar")
            {
                frmBuscarDatahierarchy f = new frmBuscarDatahierarchy(31, "Buscar Centro Costo");
                f.ShowDialog();
                if (f._value2 != null)
                {
                    txtCentroCosto.Text = f._value1.ToString().Trim();
                    txtCentroCosto.Tag = f._value2.ToString().Trim();
                }
            }
        }

        private void frmEstablecimiento_FormClosing(object sender, FormClosingEventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            Globals.CacheCombosVentaDto.cboDocumentos = new DocumentoBL().ObtenDocumentosParaComboGrid(ref objOperationResult, null, 0, 1);
            Globals.CacheCombosVentaDto.cboDocumentosRef = new DocumentoBL().ObtenDocumentosParaComboGrid(ref objOperationResult, null, 0, 1);
        }

        private void txtMaxNumeroItems_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtMaxNumeroItems, e);
        }

        private void grdDetalle_AfterExitEditMode(object sender, EventArgs e)
        {
            btnEditar_Click(sender, e);
        }

        private void grdDetalle_AfterRowActivate(object sender, EventArgs e)
        {
            //btnEditar_Click(sender, e);
        }

        private void txtNombreImpresora_BeforeDropDown(object sender, CancelEventArgs e)
        {
            if (txtNombreImpresora.Items.Count == 0)
            {
                var items = new Queue<Infragistics.Win.ValueListItem>();
                foreach (var item in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                    items.Enqueue(new Infragistics.Win.ValueListItem(item));
                if (items.Count > 0)
                {
                    txtNombreImpresora.Items.AddRange(items.ToArray());
                    items.Clear();
                }
            }
        }

        private void cboDetalleDocumento_ValueChanged(object sender, EventArgs e)
        {
            if (cboDetalleDocumento.Value == null) return;
            var id = cboDetalleDocumento.Value.ToString();
            txtDetalleSerie.MaxLength = id.Equals("12") ? 20 : 4;
        }
    }
}
