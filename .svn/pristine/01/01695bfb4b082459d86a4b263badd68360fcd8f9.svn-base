using SAMBHS.Almacen.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
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
    public partial class frmProductoRegistroRapido : Form
    {
        productoDto _productoDto = new productoDto();
        ProductoBL _objProductoBL = new ProductoBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        LineaBL _objLineaBL = new LineaBL();
        MarcaBL _objMarcaBL = new MarcaBL();
        public frmProductoRegistroRapido()
        {
            InitializeComponent();
        }

        private void frmProductoRegistroRapido_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            OperationResult objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList (cboDatosLinea, "Value1", "Id", _objLineaBL.LlenarComboLinea(ref objOperationResult, "v_CodLinea"), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboDatosMarca, "Value1", "Id", _objMarcaBL.LlenarComboMarca(ref objOperationResult, "v_CodMarca", "-1"), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboDatosTipoProducto, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 6, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboUndMedida, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 17, null), DropDownListAction.Select);
            cboDatosLinea.Value = "-1";
            cboDatosMarca.Value = "-1";
            cboDatosTipoProducto.Value = "-1";
            cboUndMedida.Value = "-1";
        }

        private void btnDatosGrabar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();

            
            if (uvDatos.Validate(true, false).IsValid)
            {
                if (txtDatosCodigo.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese una Código.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (txtDatosDescripcion.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese una Descripción.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (txtDatosEmpaque.Text.Trim ()== string.Empty  || (int.Parse ( txtDatosEmpaque.Text )==0))
                {
                    UltraMessageBox.Show("Por favor ingrese Datos Empaque Correcto", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDatosEmpaque.Focus();
                    return;
                
                }
              
                if (chkAfectoPercepcion.Checked == true)
                {
                    if (string.IsNullOrEmpty(txtTasaPercepcion.Text) || decimal.Parse(txtTasaPercepcion.Text.Trim()) == 0)
                    {
                        UltraMessageBox.Show("Por favor si el artículo es Afecto a Percepción ingrese una tasa (%)", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtTasaPercepcion.Focus();
                        return;
                    }
                }

                    if (_objProductoBL.ObtenerProductoCodigo(ref objOperationResult, txtDatosCodigo.Text) != null)
                    {
                        UltraMessageBox.Show("No se grabó porque el Código (" + txtDatosCodigo.Text + ") le pertenece a otro Producto ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    _productoDto = new productoDto();
                    _productoDto.v_CodInterno = txtDatosCodigo.Text.Trim();
                    _productoDto.v_IdLinea = cboDatosLinea.Value.ToString() == "-1" ? null : cboDatosLinea.Value.ToString();
                    _productoDto.v_IdMarca = cboDatosMarca.Value.ToString() == "-1" ? null : cboDatosMarca.Value.ToString();
                    _productoDto.v_Descripcion = txtDatosDescripcion.Text;
                    _productoDto.d_Empaque = txtDatosEmpaque.Text == string.Empty ? 0 : decimal.Parse(txtDatosEmpaque.Text.ToString());
                    _productoDto.i_IdUnidadMedida = int.Parse(cboUndMedida.Value.ToString());
                    _productoDto.d_Peso = txtDatosPeso.Text == string.Empty ? 0 : decimal.Parse(txtDatosPeso.Text.ToString());
                    _productoDto.v_Ubicacion = txtDatosUbicacion.Text;
                    _productoDto.v_Caracteristica = txtDatosCaracteristicas.Text;
                    _productoDto.v_CodProveedor = txtDatosCodProveedor.Text;
                    _productoDto.v_Descripcion2 = txtDatosDescripcion2.Text;
                    _productoDto.i_IdTipoProducto = int.Parse(cboDatosTipoProducto.Value.ToString());
                    _productoDto.i_EsServicio = chkDatosServicio.Checked == true ? 1 : 0;
                    _productoDto.i_EsLote = chkDatosLote.Checked == true ? 1 : 0;
                    _productoDto.i_EsAfectoDetraccion = chkAfectoDetraccion.Checked == true ? 1 : 0;
                    _productoDto.i_EsActivo = chkDatosActivo.Checked == true ? 1 : 0;
                    _productoDto.i_EsActivoFijo = 0;
                    _productoDto.d_PrecioVenta = txtDatosPrecioVenta.Text == string.Empty ? 0 : decimal.Parse(txtDatosPrecioVenta.Text.ToString());
                    _productoDto.d_PrecioCosto = txtDatosPrecioCosto.Text == string.Empty ? 0 : decimal.Parse(txtDatosPrecioCosto.Text.ToString());
                    _productoDto.d_StockMinimo = txtDatosStockMin.Text == string.Empty ? 0 : decimal.Parse(txtDatosStockMin.Text.ToString());
                    _productoDto.d_StockMaximo = txtDatosStockMax.Text == string.Empty ? 0 : decimal.Parse(txtDatosStockMax.Text.ToString());
                    _productoDto.i_NombreEditable = chkDescripcionEditable.Checked == true ? 1 : 0;
                    _productoDto.i_ValidarStock = chkNoStock.Checked == true ? 1 : 0;
                    _productoDto.i_IdProveedor = -1;
                    _productoDto.i_IdTipo = -1;
                    _productoDto.i_IdUsuario = -1;
                    _productoDto.i_IdTela = -1;
                    _productoDto.i_IdEtiqueta = -1;
                    _productoDto.i_IdCuello = -1;
                    _productoDto.i_IdAplicacion = -1;
                    _productoDto.i_IdArte = -1;
                    _productoDto.i_IdColeccion = -1;
                    _productoDto.i_IdTemporada = -1;
                    _productoDto.i_Anio = 0;
                    _productoDto.i_EsAfectoPercepcion = chkAfectoPercepcion.Checked == true ? 1 : 0;
                    _productoDto.d_TasaPercepcion = string.IsNullOrEmpty(txtTasaPercepcion.Text) ? 0 : decimal.Parse(txtTasaPercepcion.Text.Trim());

                    // Save the data
                    string valor = _objProductoBL.InsertarProducto(ref objOperationResult, _productoDto, Globals.ClientSession.GetAsList());

                    if (objOperationResult.Success == 1)
                    {
                        if (System.Windows.Forms.Application.OpenForms["frmOrdenCompra"] != null)
                        {
                            (System.Windows.Forms.Application.OpenForms["frmOrdenCompra"] as Procesos.frmOrdenCompra).RecibirEInsertarProductoCreado(_productoDto);
                        }
                    }
                    this.Close();
            }
        }

        private void chkDatosServicio_CheckedChanged(object sender, EventArgs e)
        {
            chkDescripcionEditable.Enabled = chkDatosServicio.Checked == true ? true : false;
        }
    }
}
