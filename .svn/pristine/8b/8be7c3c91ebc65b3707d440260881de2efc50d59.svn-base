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
using Infragistics.Win.UltraWinGrid;
using LoadingClass;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmBuscarProveedor : Form
    {
        ClienteBL _objClienteBL = new ClienteBL();
        clienteDto _clienteDto = new clienteDto();
        string _strFilterExpression;
        public string _IdProveedor, _TipoFiltro,_FlagPantalla;
        public string _RazonSocial, _CodigoProveedor, _NroDocumento;
        string _pstrCadena, _pstrOpcion;
        System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();

        public frmBuscarProveedor(string pstrCadena, string Opcion, string TipoFiltro = null ,string Flag="V")
        {
            _pstrCadena = pstrCadena;
            _pstrOpcion = Opcion;
            _TipoFiltro = TipoFiltro;
            _FlagPantalla = Flag;
            InitializeComponent();

            if (pstrCadena == "REPORTE")
            {

                this.btnRegistroRapido.Visible = false;
            }
            if (_TipoFiltro == "TRABAJADORES")
            {
                this.Text = "Buscar Responsable";
            }
            this.Text = TipoFiltro == "TRABAJADORES" ? "Buscar Trabajador" : "Buscar Proveedor";

        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            // Get the filters from the UI
            List<string> Filters = new List<string>();
           
            if (_pstrOpcion.Equals("DNI")) { Filters.Add("i_IdTipoIdentificacion==1"); }
            if (_TipoFiltro == "FILTRARSOLORUCEMPIEZA10")
            {
                Filters.Add("i_IdTipoIdentificacion == 6");
                Filters.Add("i_Activo==1");
                Filters.Add("i_EsProveedorServicios==1");
            }
            if (_TipoFiltro == "TRABAJADORES")
            {
                Filters.Add("v_FlagPantalla==\"T\"");

            }
            else
            {

                Filters.Add("v_FlagPantalla==\"V\"");
            }
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
            grdData.Focus();
        }

        private void BindGrid()
        {
            var objData = GetData("v_PrimerNombre ASC", _strFilterExpression);

            grdData.DataSource = objData;
            lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", objData.Count);
        }

        private List<clienteshortDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _objClienteBL.ObtenerListadoCliente(ref objOperationResult, pstrSortExpression, pstrFilterExpression, _TipoFiltro);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }

        private void grdData_DoubleClick(object sender, EventArgs e)
        {
            if (grdData.Rows.Count == 0) return;
            _IdProveedor = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdCliente"].Value.ToString();
            _RazonSocial = grdData.Rows[grdData.ActiveRow.Index].Cells["NombreRazonSocial"].Value.ToString();
            _CodigoProveedor = grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodCliente"].Value.ToString();
            _NroDocumento = grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroDocIdentificacion"].Value.ToString();
            this.Close();
        }

        private void grdData_KeyDown(object sender, KeyEventArgs e)
        {
            if (grdData.Rows.Count == 0) return;
            if (e.KeyCode == Keys.Enter)
            {
                _IdProveedor = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdCliente"].Value.ToString();
                _RazonSocial = grdData.Rows[grdData.ActiveRow.Index].Cells["NombreRazonSocial"].Value.ToString();
                _CodigoProveedor = grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodCliente"].Value.ToString();
                _NroDocumento = grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroDocIdentificacion"].Value.ToString();
                this.Close();
            }
        }

        private void frmBuscarProveedor_Load(object sender, EventArgs e)
        {
            myTimer.Tick += new EventHandler(OnTimedEvent);       //EDIT: should not be `ElapsedEventHandler`  
            myTimer.Interval = 300;

            this.BackColor = new GlobalFormColors().FormColor;
            panel1.BackColor = new GlobalFormColors().BannerColor;
            btnBuscar_Click(sender, e);
            txtBuscarNombre.Focus();
            lblMensaje.Text = "SE MUESTRAS SÓLO PERSONAS NATURALES CON RUC,QUE SON PRESTADORES DE SERVICIOS";
            lblMensaje.Visible = (_TipoFiltro == "FILTRARSOLORUCEMPIEZA10") ? true : false;
            if (Application.OpenForms["LoadingForm"] != null) ((LoadingForm)Application.OpenForms["LoadingForm"]).CloseWindow();
            if (Application.OpenForms["frmMaster"] != null) ((frmMaster)Application.OpenForms["frmMaster"]).Activate();
        }

        private void OnTimedEvent(Object myObject, EventArgs myEventArgs)
        {
            myTimer.Stop();
            LabelContador(Utils.Windows.FiltrarGrilla(grdData, txtBuscarNombre.Text.Trim()));
        }

        private void grdData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

        }

        private void frmBuscarProveedor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void txtBuscarNombre_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void txtBuscarDoc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                btnBuscar_Click(sender, e);
            }
        }

        private void txtBuscarCod_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                btnBuscar_Click(sender, e);
            }
        }
        private void btnRegistroRapido_Click(object sender, EventArgs e)
        {
            object sender1 = new object();
            EventArgs e1 = new EventArgs();
            //V :PROVEEDOR
            //C :CLIENTE 
            // T :TRABAJADOR
            frmRegistroRapidoCliente frm = new frmRegistroRapidoCliente(null, _FlagPantalla);
            frm.ShowDialog();
            if (frm._Guardado == true)
            {
                btnBuscar_Click(sender, e);
                txtBuscarNombre.Text = frm._Nombres.Trim();
                Utils.Windows.FiltrarGrilla(grdData, txtBuscarNombre.Text);
            }
        }

        private void txtBuscarNombre_KeyUp(object sender, KeyEventArgs e)
        {
            myTimer.Stop();
            myTimer.Start();
        }

        void LabelContador(int Cantidad)
        {
            lblContadorFilas.Text = String.Format("Se encontraron {0} registros", Cantidad);
        }
    }
}
