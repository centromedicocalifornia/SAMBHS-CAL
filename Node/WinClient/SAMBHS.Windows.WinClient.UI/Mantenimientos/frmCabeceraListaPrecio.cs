using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Compra.BL;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Venta.BL;
using SAMBHS.Almacen.BL;
using SAMBHS.Security.BL;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmCabeceraListaPrecio : Form
    {

        NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        listaprecioDto _objListaPrecioDto = new listaprecioDto();
        ListaPreciosBL _objListaPrecioBL = new ListaPreciosBL();
        List<listaprecioDto> _ListadoListaPrecio = new List<listaprecioDto>();
        List<productoalmacenDto> pTemp_Insertar = new List<productoalmacenDto>();
        int _MaxV, _ActV;
        bool _btnGuardar;
        public string strModo = string.Empty, strIdListaPrecio, _pstrIdMovimiento_Nuevo, _Mode;
        public string IdListaPrecios = "";
        public frmCabeceraListaPrecio(string Modo, string pListaPrecios)
        {
            strModo = Modo;
            IdListaPrecios = pListaPrecios;
            InitializeComponent();
        }

        private void frmCabeceraListaPrecio_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            OperationResult objOperationResult = new OperationResult();
            //#endregion
            #region CargarCombos
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", Globals.CacheCombosVentaDto.cboMoneda, DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", _objNodeWarehouseBL.ObtenerAlmacenesParaComboAll(ref objOperationResult, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboListaPrecios, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 47, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAlmacenAcopiar, "Value1", "Id", _objNodeWarehouseBL.ObtenerAlmacenesParaComboAll(ref objOperationResult,null), DropDownListAction.Select);


            cboMoneda.Value = "-1";
            cboListaPrecios.Value = "-1";
            cboAlmacenAcopiar.Value = "-1";
            cboAlmacenAcopiar.Enabled = false;
            cboListaPrecioAcopiar.Enabled = false;

            #endregion
            cboAlmacen.Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
            ObtenerListadoCabeceras(strModo);
        }
        private void ObtenerListadoCabeceras(string strModo)
        {
            switch (strModo)
            {
                case "Edicion":
                    CargarCabecera(IdListaPrecios);
                    cboAlmacen.Enabled = false;
                    cboListaPrecios.Enabled = false;

                    break;

                case "Nuevo":
                    cboAlmacen.Enabled = true;
                    cboListaPrecios.Enabled = true;
                    break;
            }
        }

        private void CargarCabecera(string IdListaPrecios)
        {
            var objOperationResult = new OperationResult();
            _objListaPrecioDto = _objListaPrecioBL.ObtenerCabeceraListaPrecios(ref objOperationResult, IdListaPrecios);
            cboAlmacen.Value = _objListaPrecioDto.i_IdAlmacen.ToString();
            cboListaPrecios.Value = _objListaPrecioDto.i_IdLista.ToString();
            cboMoneda.Value = _objListaPrecioDto.i_IdMoneda.ToString();


        }
        private void btnGuardar_Click(object sender, EventArgs e)
        {


            OperationResult objOperationResult = new OperationResult();
            var NumListaPrecios =_objListaPrecioBL.VerificarNumeroListaPrecios(ref objOperationResult);
            if (NumListaPrecios == 1 && Globals.ClientSession.i_SeUsaraSoloUnaListaPrecioEmpresa == 1)
            {
                UltraMessageBox.Show("No se puede crear una Lista adicional , ya tiene una Lista", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;

            }

            if (chkCopiardeAlmacen.Checked)
            {
                if (uvValidarCopiarOtraLista.Validate(true, false).IsValid)
                {

                    if (UltraMessageBox.Show("Se copiará la Lista " + cboListaPrecioAcopiar.Text + " del almacén " + cboAlmacenAcopiar.Text + " al almacén " + cboAlmacen.Text, "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
                    {
                       
                        _objListaPrecioDto.i_IdLista = int.Parse(cboListaPrecioAcopiar.Value.ToString ());
                        _objListaPrecioDto.i_IdAlmacen = int.Parse(cboAlmacen.Value.ToString());
                        _objListaPrecioDto.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());
                    }

                }

            }
            else 
            {
                if (uvListaPrecio.Validate(true, false).IsValid)
                {
                   
                    _objListaPrecioDto.i_IdLista = int.Parse(cboListaPrecios.Value.ToString());
                    _objListaPrecioDto.i_IdAlmacen = int.Parse(cboAlmacen.Value.ToString());
                    _objListaPrecioDto.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());

                }
            }
                if (chkCopiardeAlmacen.Checked)
                {

                    pTemp_Insertar = _objListaPrecioBL.InsertarListaPreciosCopiadoDesdeOtraListaPrecios(ref objOperationResult, _objListaPrecioDto.i_IdAlmacen.Value, int.Parse(cboAlmacenAcopiar.Value.ToString()), int.Parse(cboListaPrecioAcopiar.Value.ToString()));
                }
                else
                {
                    pTemp_Insertar = _objListaPrecioBL.ProductosAlmacenInsertarListaPrecios(ref objOperationResult, _objListaPrecioDto.i_IdAlmacen.Value);
                }
                string TipoCambio = _objListaPrecioBL.DevolverTipoCambioPorFecha(ref objOperationResult, DateTime.Now.Date);
                if (strModo != "Edicion")
                {
                    if (TipoCambio == "0")
                    {

                        UltraMessageBox.Show("Es necesario registrar un tipo de cambio válido", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (pTemp_Insertar.Count == 0)
                    {
                        UltraMessageBox.Show("No se puede asignar Lista a este almacén, no tiene productos", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;

                    }
                }


                if (strModo != "Edicion" && _objListaPrecioBL.ObtenerIdListaPrecio(ref objOperationResult, int.Parse(cboAlmacen.Value.ToString()), int.Parse(cboListaPrecios.Value.ToString())))
                {
                    UltraMessageBox.Show("La asignación ya se ha realizado anteriormente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    if (strModo == "Nuevo")
                    {

                        if (chkCopiardeAlmacen.Checked)
                        {
                            _objListaPrecioBL.InsertarListaPrecios(ref objOperationResult, _objListaPrecioDto, Globals.ClientSession.GetAsList(), pTemp_Insertar, TipoCambio, true);
                        }
                        else
                        {
                            _objListaPrecioBL.InsertarListaPrecios(ref objOperationResult, _objListaPrecioDto, Globals.ClientSession.GetAsList(), pTemp_Insertar, TipoCambio, false);
                        }
                    }

                    else
                    {
                        _objListaPrecioBL.ActualizarCabeceraListaPrecios(ref objOperationResult, _objListaPrecioDto, Globals.ClientSession.GetAsList());

                    }
                    if (objOperationResult.Success == 1)
                    {
                        strModo = "Guardado";
                        strIdListaPrecio = _objListaPrecioDto.v_IdListaPrecios;
                        ObtenerListadoListaPrecios();
                        _pstrIdMovimiento_Nuevo = _objListaPrecioDto.v_IdListaPrecios;
                        UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

            }

 
        private void ObtenerListadoListaPrecios()
        {
            OperationResult objOperationResult = new OperationResult();

            _ListadoListaPrecio = _objListaPrecioBL.ObtenerListadoListaPrecios(ref objOperationResult);

            //_ListaPrecios=_objListaPrecioBL.
            switch (strModo)
            {
                case "Edicion":
                    //EdicionBarraNavegacion(false);
                    //CargarCabecera(strIdGuiaRemision);
                    //cboDocumento.Enabled = false;
                    // cboAlmacen;
                    break;

                case "Nuevo":
                    if (_ListadoListaPrecio.Count != 0)
                    {
                        //_MaxV = _ListadoListaPrecio.Count() - 1;
                        //_ActV = _MaxV;
                        //LimpiarCabecera();
                        //CargarDetalle("");
                        //txtCorrelativo.Text = (int.Parse(_ListadoPedidos[_MaxV].Value1) + 1).ToString("00000000");
                        _Mode = "New";
                        //_objPedido = new pedidoDto();
                        //EdicionBarraNavegacion(false);


                    }
                    else
                    {
                        //txtCorrelativo.Text = "00000001";
                        _Mode = "New";
                        //LimpiarCabecera();
                        //CargarDetalle("");
                        _MaxV = 1;
                        _ActV = 1;
                        //_objPedido = new pedidoDto();
                        //btnNuevoMovimiento.Enabled = false;
                        //EdicionBarraNavegacion(false);

                    }
                    //cboEstados.Value = "0";
                    //txtTipoCambio.Text = _objPedidoBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaEmision.Value.Date);
                    //cboDocumento.Enabled = true_

                    break;

                case "Guardado":

                    if (strIdListaPrecio == "" | strIdListaPrecio == null)
                    {
                        //  CargarCabecera(_ListadoListaPrecio[_MaxV].v_IdListaPrecios);
                    }
                    else
                    {
                        //CargarCabecera(strIdListaPrecio);
                    }
                    //btnNuevoMovimiento.Enabled = true;
                    //cboDocumento.Enabled = false;
                    break;

                case "Consulta":
                    if (_ListadoListaPrecio.Count != 0)
                    {
                        _MaxV = _ListadoListaPrecio.Count() - 1;
                        _ActV = _MaxV;
                        //txtCorrelativo.Text = (int.Parse(_ListadoPedidos[_MaxV].Value1)).ToString("00000000");
                        //  CargarCabecera(_ListadoListaPrecio[_MaxV].v_IdListaPrecios);
                        _Mode = "Edit";
                        //EdicionBarraNavegacion(true);
                    }
                    else
                    {
                        //txtCorrelativo.Text = "00000001";
                        _Mode = "New";
                        //LimpiarCabecera();
                        //  CargarDetalle("");
                        _MaxV = 1;
                        _ActV = 1;
                        //_objPedido = new pedidoDto();
                        //btnNuevoMovimiento.Enabled = false;
                        //_objPedidoBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaEmision.Value.Date);
                        //EdicionBarraNavegacion(false);
                        //txtMes.Enabled = true;
                    }
                    //cboDocumento.Enabled = false;
                    break;
            }


        }

        private void cboAlmacenAcopiar_ValueChanged(object sender, EventArgs e)
        {
            EstablecimientoBL objEstablecimientoBL = new EstablecimientoBL();
            OperationResult objOperationResult = new OperationResult();
            List<KeyValueDTO> ListaPreciosporAlmacen = new List<KeyValueDTO>();
            if (cboAlmacenAcopiar.Value == null || cboAlmacenAcopiar.Value.ToString() == "-1") return;
            ListaPreciosporAlmacen = _objDatahierarchyBL.LlenarListaPreciosPorAlmacen(ref  objOperationResult, 47, "", int.Parse(cboAlmacenAcopiar.Value.ToString())); // objEstablecimientoBL.GetAlmacenesXEstablecimiento(int.Parse(cboEstablecimiento.Value.ToString()));
            Utils.Windows.LoadUltraComboEditorList(cboListaPrecioAcopiar, "Value1", "Id", ListaPreciosporAlmacen, DropDownListAction.Select);
            cboListaPrecioAcopiar.Value = "-1";
        }

        private void chkCopiardeAlmacen_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCopiardeAlmacen.Checked)
            {
                cboAlmacenAcopiar.Enabled = true;
                cboListaPrecioAcopiar.Enabled = true;
                cboListaPrecioAcopiar.Value = "-1";


            }
            else
            {

                cboAlmacenAcopiar.Enabled = false;
                cboListaPrecioAcopiar.Enabled = false;


            }
        }

        private void cboAlmacen_ValueChanged(object sender, EventArgs e)
        {

        }

       
    }
}
