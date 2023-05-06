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
    public partial class frmBandejaListaPrecios : Form
    {
        #region Fields
        private string _strFilterExpression;
        private readonly ListaPreciosBL _objListaPreciosBl = new ListaPreciosBL();
        private readonly NodeWarehouseBL _objNodeWarehouseBl = new NodeWarehouseBL();
        private readonly DatahierarchyBL _objDatahierarchyBl = new DatahierarchyBL();
        private readonly SecurityBL _objSecurityBl = new SecurityBL();
        private bool _btnNuevaLista, _btnListaUtilidad, _btnListaDscto , _btnEliminarLista;
        private string _pstrIdListaPrecio;
        #endregion

        #region Init & Load
        public frmBandejaListaPrecios(string cadena)
        {
            InitializeComponent();
        }
     
        private void frmBandejaListaPrecios_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            OperationResult objOperationResult = new OperationResult();
            #region ControlAcciones
            var _formActions = _objSecurityBl.GetFormAction(ref objOperationResult, Globals.ClientSession.i_CurrentExecutionNodeId, Globals.ClientSession.i_SystemUserId, "frmBandejaListaPrecios", Globals.ClientSession.i_RoleId);

            _btnNuevaLista = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaListaPrecios_New", _formActions);
            _btnListaUtilidad = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaListaPrecios_ListUti", _formActions);
            _btnListaDscto = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaListaPrecios_ListDesc", _formActions);
            _btnEliminarLista = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaListaPrecios_Delete", _formActions);
            

            btnNuevaLista.Enabled = _btnNuevaLista;
            btnListaUtilidad.Enabled = _btnListaUtilidad;
            btnListaDscto.Enabled = _btnListaDscto;
            btnEliminarLista.Enabled = _btnEliminarLista;

            #endregion
            btnListaDscto.Enabled = false;
            btnListaUtilidad.Enabled = false;
            #region CargarCombos
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", _objNodeWarehouseBl.ObtenerAlmacenesParaCombo(ref objOperationResult, null, Globals.ClientSession.i_IdEstablecimiento.Value ), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboListaPrecios, "Value1", "Id", _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 47, null), DropDownListAction.Select);
            #endregion
            cboAlmacen.Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
            cboListaPrecios.Value = "-1";
            List<string> Filters = new List<string>();

            if (cboAlmacen.Value.ToString() != "-1") Filters.Add("i_IdAlmacen ==" + cboAlmacen.Value.ToString());
            if (cboListaPrecios.Value.ToString() != "-1") Filters.Add("i_IdLista==" + cboListaPrecios.Value.ToString());

            _strFilterExpression = null;
            if (Filters.Count > 0)
            {
                foreach (string item in Filters)
                {
                    _strFilterExpression = _strFilterExpression + item + " && ";
                }
                _strFilterExpression = _strFilterExpression.Substring(0, _strFilterExpression.Length - 4);
            }
        }
        #endregion

        #region Events
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            List<string> Filters = new List<string>();
            OperationResult objOperationResult = new OperationResult();

            if (Validar.Validate(true, false).IsValid)
            {


                if (cboAlmacen.Value.ToString() != "-1") Filters.Add("i_IdAlmacen ==" + cboAlmacen.Value.ToString());
                if (cboListaPrecios.Value.ToString() != "-1") Filters.Add("i_IdLista==" + cboListaPrecios.Value.ToString());

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
        }


        private void ListaUtilidad_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();

            string TipoCambio = _objListaPreciosBl.DevolverTipoCambioPorFecha(ref objOperationResult, DateTime.Now.Date);

            if (TipoCambio == "0")
            {

                UltraMessageBox.Show("Es necesario registrar un tipo de cambio válido", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                if (grdData.ActiveRow != null)
                {
                    _pstrIdListaPrecio = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdListaPrecios"].Value.ToString();
                    frmListaPreciosUtilidades frmListaPreciosUtilidades = new frmListaPreciosUtilidades("Edicion", _pstrIdListaPrecio);
                    frmListaPreciosUtilidades.ShowDialog();
                    BindGrid();
                    MantenerSeleccion(_pstrIdListaPrecio);
                }
            }
        }

        private void btnListaDscto_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();

            string TipoCambio = _objListaPreciosBl.DevolverTipoCambioPorFecha(ref objOperationResult, DateTime.Now.Date);

            if (TipoCambio == "0")
            {

                UltraMessageBox.Show("Es necesario registrar un tipo de cambio válido", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                if (grdData.ActiveRow != null)
                {
                    _pstrIdListaPrecio = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdListaPrecios"].Value.ToString();
                    frmListaPrecios frmListaPrecios = new frmListaPrecios("Edicion", _pstrIdListaPrecio);
                    frmListaPrecios.ShowDialog();
                    BindGrid();
                    MantenerSeleccion(_pstrIdListaPrecio);
                }
            }

        }

        private void grdData_MouseDown(object sender, MouseEventArgs e)
        {
            Point point = new System.Drawing.Point(e.X, e.Y);
            Infragistics.Win.UIElement uiElement = ((Infragistics.Win.UltraWinGrid.UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

            if (uiElement == null || uiElement.Parent == null) return;

            Infragistics.Win.UltraWinGrid.UltraGridRow row = (Infragistics.Win.UltraWinGrid.UltraGridRow)uiElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridRow));

            if (row == null)
            {

                btnListaUtilidad.Enabled = false;
                btnListaDscto.Enabled = false;
                btnEditarLista.Enabled = false;

                btnEliminarLista.Enabled = false;

            }
            else
            {

                btnListaDscto.Enabled = _btnListaDscto;
                btnListaUtilidad.Enabled = _btnListaUtilidad;
                btnEditarLista.Enabled = true;
                btnEliminarLista.Enabled = _btnEliminarLista;


            }
        }

        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            if (grdData.ActiveRow != null)
            {
                btnListaUtilidad.Enabled = _btnListaUtilidad;
                btnListaDscto.Enabled = _btnListaDscto;
                btnEditarLista.Enabled = true;
                btnEliminarLista.Enabled = _btnEliminarLista;

            }
            else
            {

                btnListaUtilidad.Enabled = false;
                btnListaDscto.Enabled = false;
                btnEditarLista.Enabled = false;
                btnEliminarLista.Enabled = false;
            }
        }

        private void btnEditarLista_Click(object sender, EventArgs e)
        {
            var frmCabeceraListaPrecio = new frmCabeceraListaPrecio("Edicion", grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdListaPrecios"].Value.ToString());
            frmCabeceraListaPrecio.ShowDialog();
            BindGrid();
        }

        private void btnNuevaLista_Click(object sender, EventArgs e)
        {
            var frmCabeceraListaPrecio = new frmCabeceraListaPrecio("Nuevo", "");
            frmCabeceraListaPrecio.ShowDialog();
            BindGrid();

        }

        private void btnEliminarLista_Click(object sender, EventArgs e)
        {
            var objOperationResult = new OperationResult();
            if (UltraMessageBox.Show("¿Está seguro de Eliminar Tarifario ,Lista Utilidades y Lista Descuentos ?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string idListaPrecios = grdData.ActiveRow.Cells["v_IdListaPrecios"].Value.ToString();
                _objListaPreciosBl.EliminarListaPrecios(ref objOperationResult, idListaPrecios, Globals.ClientSession.GetAsList());
                btnBuscar_Click(sender, e);
            }
        }   
        #endregion

        #region Methods
        private void BindGrid()
        {
            var objData = GetData("v_IdListaPrecios ASC", _strFilterExpression);
            grdData.DataSource = objData;
            lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", objData.Count);
        }

        private void MantenerSeleccion(string valorSeleccionado)
        {
            foreach (var row in grdData.Rows)
            {
                if (row.Cells["v_IdListaPrecios"].Text == valorSeleccionado)
                {
                    grdData.ActiveRow = row;
                    grdData.ActiveRow.Selected = true;
                    break;
                }
            }
        }

        private List<listaprecioDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            var objOperationResult = new OperationResult();
            var objData = _objListaPreciosBl.ListarBusquedaListaPrecios(ref objOperationResult, pstrSortExpression, pstrFilterExpression);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return objData;
        }
        #endregion
    }
}
