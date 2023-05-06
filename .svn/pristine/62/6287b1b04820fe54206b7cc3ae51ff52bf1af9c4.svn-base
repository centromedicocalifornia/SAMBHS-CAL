using System;
using SAMBHS.Almacen.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    // ReSharper disable once InconsistentNaming
    public partial class frmGenerarProductoMatriz : Form
    {
        #region Fields
        private string _codLinea = "", _codMarca = "";
        private readonly bool _returnValues;
        private readonly List<Dictionary<string, string>> _result;
        #endregion

        #region Properties
        public List<Dictionary<string, string>> Resultado
        {
            get
            {
                return _result;
            }
        }
        public string CodigoProveedor { get; set; }
        #endregion

        #region Init & Load
        public frmGenerarProductoMatriz(bool preturn = false)
        {
            InitializeComponent();
            _returnValues = preturn;
            if (_returnValues)
                _result = new List<Dictionary<string, string>>();
        }

        private void frmAgregarProducto_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
            var objOperationResult = new OperationResult();
            #region Combos
            Utils.Windows.LoadUltraComboList(cbLinea, "Value1", "Id", new LineaBL().LlenarComboLinea(ref objOperationResult, "v_CodLinea"), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(cbMarca, "Value1", "Id", new MarcaBL().LlenarComboMarca(ref objOperationResult, "v_CodMarca", "-1"), DropDownListAction.Select);
            var objDatahierarchyBl = new CommonWIN.BL.DatahierarchyBL();
            Utils.Windows.LoadUltraComboEditorList(cboMaterial, "Value1", "Id", objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 14, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboColeccion, "Value1", "Id", objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 15, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAtributosTemporada, "Value1", "Id", objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 16, null), DropDownListAction.Select);
            // Utils.Windows.LoadUltraComboEditorList(cboAtributosTipo, "Value1", "Id", objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 8, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAtributosUsuario, "Value1", "Id", objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 9, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboDatosTipoProducto, "Value1", "Id", objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 6, null), DropDownListAction.Select);
            cbLinea.Value = "-1";
            cbMarca.Value = "-1";
            cboMaterial.Value = "-1";
            cboColeccion.Value = "-1";
            cboAtributosTemporada.Value = "-1";
            cboAtributosUsuario.Value = "-1";
            cboDatosTipoProducto.Value = "-1";
            txtAtributosAño.Text = Globals.ClientSession.i_Periodo.ToString();
            //cboAtributosTipo.Value = "-1";
            cbLinea.KeyDown += UltraCombo_KeyDown;
            cbMarca.KeyDown += UltraCombo_KeyDown;
            #endregion

            #region HideResultados
            PanelRegView(false);
            #endregion

            #region Grid
            grColor.DataSource = new ColorBL().LlenarComboColor(ref objOperationResult, "v_CodColor");
            grTalla.DataSource = new TallaBL().LlenarComboTalla(ref objOperationResult, "v_CodTalla");
            grColor.KeyDown += (_, ev) =>
            {
                if (ev.Control && ev.KeyCode == Keys.Tab)
                {
                    if (ev.Shift)
                        txtPrecioC.Focus();
                    else
                        grTalla.Focus();
                }
            };
            grTalla.KeyDown += (_, ev) =>
            {
                if (ev.Control && ev.KeyCode == Keys.Tab)
                {
                    if (ev.Shift)
                        grColor.Focus();
                    else
                        btnSave.Focus();
                }
            };
            #endregion

            #region ESC
            var cancelBtn = new Button();
            cancelBtn.Click += delegate { Close(); };
            CancelButton = cancelBtn;
            #endregion
        }
        #endregion

        #region Eventos
        private void txtModeloNro_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtModeloNro, e);
        }

        private void txtPrecio_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroDecimalUltraTextBox((Infragistics.Win.UltraWinEditors.UltraTextEditor)sender, e);
        }

        private void UltraCombo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Down)
            {
                ((Infragistics.Win.UltraWinGrid.UltraCombo)sender).PerformAction(Infragistics.Win.UltraWinGrid.UltraComboAction.Dropdown);
                e.Handled = true;
            }
        }

        private void cbLinea_AfterCloseUp(object sender, EventArgs e)
        {
            _codLinea = cbLinea.Value.ToString() == "-1" ? "" : cbLinea.ActiveRow.Cells["Value2"].Value.ToString().TrimEnd();
            BindGridReg();
        }

        private void cbMarca_AfterCloseUp(object sender, EventArgs e)
        {
            _codMarca = cbMarca.Value.ToString() == "-1" ? "" : cbMarca.ActiveRow.Cells["Value2"].Value.ToString();
            BindGridReg();
        }

        private void Grid_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            if (e.Cell.Column.Key != "chk") return;

            BindGridReg();
        }

        private void txtModelo_Leave(object sender, EventArgs e)
        {
            BindGridReg();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            if (uvDatos.Validate(true, false).IsValid)
            {
                #region Validaciones
                if ((string)cbLinea.Value == "-1")
                {
                    UltraMessageBox.Show("Seleccione una Linea", "Error de Validación!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cbLinea.Focus();
                    return;
                }
                if ((string)cbMarca.Value == "-1")
                {
                    UltraMessageBox.Show("Seleccione una Marca", "Error de Validación!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cbMarca.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtModeloDesc.Text))
                {
                    UltraMessageBox.Show("Escriba un Modelo Válido", "Error de Validación!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtModeloDesc.Focus();
                    return;
                }

                if ((string)cboAtributosTemporada.Value == "-1")
                {
                    UltraMessageBox.Show("Seleccione Temporada", "Error de Validación!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cboAtributosTemporada.Focus();
                    return;
                }

                if (txtModeloNro.TextLength != 3)
                {
                    UltraMessageBox.Show("Necesita ingresar 3 digitos del Modelo", "Error de Validación!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtModeloNro.Focus();
                    return;
                }

                if (cboDatosTipoProducto.Text.Contains ("TERMINADO") && string.IsNullOrEmpty(txtNroOrdenProduccion.Text))
                {
                    UltraMessageBox.Show("Los Productos terminados siempre van asociados a un Nro. Orden Producción", "Error de Validación!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNroOrdenProduccion.Focus();
                    return;
                }
                var cantCod = GetCountCodesRepeat();
                if (cantCod != 0)
                {
                    MessageBox.Show(@"Existen " + cantCod + @" codigos ya registrados.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                #endregion

                var objOperationResult = new OperationResult();
                var productos = new List<productoDto>(grRegistros.Rows.Count);
                List<string> idsProdDetalle;
                using (new LoadingClass.PleaseWait(Location, "Insertando Registros..."))
                {
                    foreach (var item in grRegistros.Rows)
                    {
                        var prodDto = new productoDto
                        {
                            v_IdLinea = cbLinea.Value.ToString(),
                            v_IdMarca = cbMarca.Value.ToString(),
                            v_Descripcion = txtModeloDesc.Text,
                            v_Modelo = txtModeloDesc.Text.Trim(),
                            d_Empaque = 1,
                            i_IdUnidadMedida = 15,
                            d_Peso = 0,
                            v_Ubicacion = string.Empty,
                            v_Caracteristica = string.Empty,
                            v_CodProveedor = CodigoProveedor,
                            v_Descripcion2 = string.Empty,
                            i_IdTipoProducto =int.Parse (cboDatosTipoProducto.Value.ToString ()),
                            i_EsServicio = 0,
                            i_EsLote = 0,
                            i_EsAfectoDetraccion = 0,
                            i_EsActivo = 1,
                            d_PrecioVenta = GetValueFromText(txtPrecioV.Text),
                            d_PrecioCosto = GetValueFromText(txtPrecioC.Text),
                            d_StockMinimo = 0.00M,
                            d_StockMaximo = 0.00M,
                            i_NombreEditable = 0,
                            i_ValidarStock = 1,
                            i_IdProveedor = -1,
                            i_IdTipo = -1,
                            i_IdUsuario = int.Parse(cboAtributosUsuario.Value.ToString()),
                            i_IdTela = -1,
                            i_IdEtiqueta = -1,
                            i_IdCuello = -1,
                            i_IdAplicacion = -1,
                            i_IdArte = cboMaterial.Value != null ? int.Parse(cboMaterial.Value.ToString()) : -1,
                            i_IdColeccion = cboColeccion.Value != null ? int.Parse(cboColeccion.Value.ToString()) : -1,
                            i_IdTemporada = cboAtributosTemporada.Value != null ? int.Parse(cboAtributosTemporada.Value.ToString()) : -1,
                            i_Anio = txtAtributosAño.Text == string.Empty ? 0 : int.Parse(txtAtributosAño.Text),
                            i_EsAfectoPercepcion = 0,
                            d_TasaPercepcion = 0.00M,
                            i_PrecioEditable = 0,
                            i_EsActivoFijo = 0,
                            i_EsAfectoIsc = 0,
                            v_CodInterno = item.Cells["Id"].Value.ToString(),
                            v_NroOrdenProduccion = txtNroOrdenProduccion.Text.Trim(),
                            d_PrecioMayorista = GetValueFromText(txtPrecioMayorista.Text),
                            
                        };

                        prodDto.v_Descripcion = item.Cells["Value1"].Value.ToString();
                        prodDto.v_IdColor = item.Cells["Value2"].Value.ToString();
                        prodDto.v_IdTalla = item.Cells["Value3"].Value.ToString();

                        productos.Add(prodDto);
                        if (_returnValues)
                            _result.Add(new Dictionary<string, string>()
                            {
                                {"NomProd", prodDto.v_Descripcion},
                                {"CodProd", prodDto.v_CodInterno},
                                {"PrecioUnitario", prodDto.d_PrecioCosto.ToString()},
                                {"PrecioVenta", prodDto.d_PrecioVenta.ToString()}
                            });
                    }
                    idsProdDetalle = new ProductoBL().InsertarProductos(ref objOperationResult, productos, Globals.ClientSession.GetAsList());
                }
                Activate();
                Focus();
                if (objOperationResult.Success == 1)
                {
                    ResetSelect();
                    MessageBox.Show(@"Se Insertaron " + idsProdDetalle.Count + @" producto(s)");

                    if (_returnValues)
                    {
                        for (short i = 0; i < idsProdDetalle.Count; i++)
                        {
                            _result[i].Add("IdProd", idsProdDetalle[i]);
                        }

                        if (Modal)
                        {
                            DialogResult = DialogResult.OK;
                        }
                        //Close();
                    }
                }
                else
                    MessageBox.Show(string.Format("Hubieron errores al Insertar Registros \n Error: {0} \n Additional: {1}", objOperationResult.ErrorMessage, objOperationResult.AdditionalInformation),
                        @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Methods

        private decimal GetValueFromText(string text)
        {
            decimal res;
            decimal.TryParse(text, out res);

            return res;
        }

        private void ResetSelect()
        {
            grRegistros.Selected.Rows.AddRange(grRegistros.Rows.ToArray());
            grRegistros.DeleteSelectedRows(false);
            grColor.AfterCellUpdate -= Grid_AfterCellUpdate;
            grTalla.AfterCellUpdate -= Grid_AfterCellUpdate;
            foreach (var row in grColor.Rows)
            {
                row.Cells["chk"].Value = false;
            }

            foreach (var row in grTalla.Rows)
            {
                row.Cells["chk"].Value = false;
            }

            grColor.AfterCellUpdate += Grid_AfterCellUpdate;
            grTalla.AfterCellUpdate += Grid_AfterCellUpdate;
            PanelRegView(false);
        }

        /// <summary>
        /// Carga los datos a la Grilla de Los Nuevos Registros
        /// </summary>
        private void BindGridReg()
        {
            var items = new List<KeyValueDTO>();
            var codInit = _codLinea + _codMarca;
            foreach (var itemColor in grColor.Rows)
            {
                var chk = (bool)itemColor.Cells["chk"].Value;
                if (!chk) continue;

                var value2Color = itemColor.Cells["Value2"].Value.ToString();
                var value1Color = itemColor.Cells["Value1"].Value.ToString();
                foreach (var itemTalla in grTalla.Rows)
                {
                    chk = (bool)itemTalla.Cells["chk"].Value;
                    if (!chk) continue;
                    items.Add(new KeyValueDTO
                    {
                        Id = string.Concat(codInit, txtModeloNro.Text, itemTalla.Cells["Value2"].Value.ToString(), value2Color),
                        Value1 = string.Join(" ", txtModeloDesc.Text, value1Color, itemTalla.Cells["Value1"].Value),
                        Value2 = itemColor.Cells["Id"].Value.ToString(),
                        Value3 = itemTalla.Cells["Id"].Value.ToString()
                    });
                }
            }
            if (items.Count > 0 && !gbResult.Visible)
                PanelRegView(true);
            grRegistros.DataSource = items;
        }

        private int GetCountCodesRepeat()
        {
            var objResult = new OperationResult();
            var prodsCode = new ProductoBL().ListarProducto(ref objResult, "", "", -1).Select(i => i.v_CodInterno).ToList();
            short count = 0;
            Infragistics.Win.UltraWinGrid.UltraGridRow row = null;
            foreach (var item in grRegistros.Rows)
            {
                if (prodsCode.Contains(item.Cells["Id"].Value.ToString()))
                {
                    if (count == 0) row = item;
                    count++;
                    item.Appearance.BackColor = Color.Salmon;
                    grRegistros.ActiveRowScrollRegion.ScrollRowIntoView(item);
                }
            }
            if (count > 0) grRegistros.ActiveRowScrollRegion.ScrollRowIntoView(row);
            return count;
        }

        private void PanelRegView(bool show)
        {
            //if (show)
            //{
            //    Height = 667;
            //    gbResult.Visible = true;
            //}
            //else
            //{
            //    Height = 453;
            //    gbResult.Visible = false;
            //}
        }
        #endregion

        private void cboDatosTipoProducto_ValueChanged(object sender, EventArgs e)
        {

            txtNroOrdenProduccion.Enabled = cboDatosTipoProducto.Text.Contains("TERMINADO") ? true : false;
            if (!cboDatosTipoProducto.Text.Contains("TERMINADO"))
                txtNroOrdenProduccion.Text = "";

        }

    }
}
