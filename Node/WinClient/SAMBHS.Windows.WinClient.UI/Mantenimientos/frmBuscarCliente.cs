using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.Venta.BL;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    // ReSharper disable once InconsistentNaming
    public partial class frmBuscarCliente : Form
    {
        #region Fields
        private readonly ClienteBL _objClienteBl = new ClienteBL();
        private readonly string _strNombre;
        private readonly string _frmOrigen;
        private readonly Timer _myTimer;
        #endregion

        #region Properties
        public string _IdCliente;
        public int _IdLista;
        public string _RazonSocial;
        public string _CodigoCliente;
        public int _TipoDocumento, _TipoPersona;
        public int _IdDireccionCliente;
        public string _NroDocumento, _Direccion;
        #endregion

        #region Init & Load
        public frmBuscarCliente(string frmOrigen, string pstrNombre)
        {
            _strNombre = pstrNombre;
            _frmOrigen = frmOrigen;
            _myTimer = new Timer { Interval = 300 };
            _myTimer.Tick += OnTimedEvent;
            InitializeComponent();
        }

        private void frmBuscarCliente_Load(object sender, EventArgs e)
        {


            BackColor = new GlobalFormColors().FormColor;
            panel1.BackColor = new GlobalFormColors().BannerColor;
            if (_strNombre != null | _strNombre != string.Empty)
            {
                if (_frmOrigen == "V")
                {
                    txtBuscarNombre.Text = _strNombre;
                    btnBuscar_Click(sender, e);
                }
                else if (_frmOrigen == "VV" || _frmOrigen.Contains("P"))
                {
                    Text = @"Buscar Cliente";
                    btnBuscar_Click(sender, e);
                }
                txtBuscarNombre.Text = _strNombre;
                txtBuscarNombre_KeyUp(null, new KeyEventArgs(Keys.Enter));
            }
        }
        #endregion

        #region Events
        private void OnTimedEvent(Object myObject, EventArgs myEventArgs)
        {
            _myTimer.Stop();
            var cantidad = Utils.Windows.FiltrarGrilla(grdData, txtBuscarNombre.Text.Trim());
            lblContadorFilas.Text = string.Format("Se encontraron {0} registros", cantidad);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {

            var filters = new Queue<string>();
            if (_frmOrigen.Contains("V")) filters.Enqueue("v_FlagPantalla.Contains(\"C\")");
            else if (_frmOrigen.Contains("P"))
            {
                filters.Enqueue("v_FlagPantalla.Contains(\"V\")");
                if (_frmOrigen.Equals("PD"))
                    filters.Enqueue("i_idTipoIdentificacion == 1");
            }
            var strFilterExpression = filters.Count > 0 ? string.Join(" && ", filters) : null;
            BindGrid(strFilterExpression);
            grdData.Focus();
        }

        private void grdData_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (grdData.ActiveRow == null) return;
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
                EndAndReturn((clienteshortDto)grdData.ActiveRow.ListObject);
        }
        private void frmBuscarCliente_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        private void txtBuscarNombre_KeyUp(object sender, KeyEventArgs e)
        {
            _myTimer.Stop();
            _myTimer.Start();
        }

        private void grdData_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            EndAndReturn((clienteshortDto)e.Row.ListObject);
        }
        #endregion

        #region Methods

        private void BindGrid(string filter)
        {
            var objData = GetData("v_RazonSocial ASC", filter, 0);
            if (objData != null)
            {
                grdData.DataSource = objData;
                //AutoAjusta el ancho de la columna segun su contenido y encabezado.
                grdData.DisplayLayout.Bands[0].Columns["v_CodCliente"].PerformAutoResize(Infragistics.Win.UltraWinGrid.PerformAutoSizeType.AllRowsInBand,
                                                                      Infragistics.Win.UltraWinGrid.AutoResizeColumnWidthOptions.All);
            }

        }

        private List<clienteshortDto> GetData(string pstrSortExpression, string pstrFilterExpression, int pintGrupoId)
        {
            var objOperationResult = new OperationResult();
            var objData = _objClienteBl.ObtenerListadoClienteBusqueda(ref objOperationResult, pstrSortExpression, pstrFilterExpression);
            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return objData;
        }

        private void EndAndReturn(clienteshortDto dto)
        {
            if (dto == null) return;
            _IdCliente = dto.v_IdCliente;
            _RazonSocial = dto.NombreRazonSocial;
            _NroDocumento = dto.v_NroDocIdentificacion;
            _CodigoCliente = dto.v_CodCliente;
            _IdLista = dto.i_IdLista ?? 1;
            _Direccion = dto.v_Direccion ?? string.Empty;
            _TipoDocumento = dto.i_ParameterId ?? 0;
            _TipoPersona = dto.i_IdTipoPersona ?? 6;
            _IdDireccionCliente = dto.i_IdDireccionCliente.Value;
            Close();
        }
        #endregion
    }
}

