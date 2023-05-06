using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.Resource;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    // ReSharper disable once InconsistentNaming
    public partial class frmBuscarTransportista : Form
    {
        #region Fields
        private readonly string _strCadena;
        private readonly TransportistaBL _objTransportistaBl = new TransportistaBL();
        private readonly Timer _myTimer;
        #endregion

        #region Properties
        public string StrIdTransportista, StrRazonSocial, StrCodigo, StrNroDocumento;
        #endregion

        #region Init & Load
        public frmBuscarTransportista(string cadena)
        {
            _strCadena = cadena;
            InitializeComponent();
            _myTimer = new Timer {Interval = 500};
            _myTimer.Tick += OnTimedEvent;
        }
        private void frmBuscarTransportista_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
            txtBuscarNombre.Text = _strCadena;
            BindGrid(null);
            if(_strCadena != "")
                _myTimer.Start();
        }
        #endregion  

        #region Events
        private void grdData_KeyDown(object sender, KeyEventArgs e)
        {
            if (grdData.ActiveRow == null) return;
            if (e.KeyCode == Keys.Enter)
                EndAndRerturn((transportistaDto)grdData.ActiveRow.ListObject);
        }

        private void grdData_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            EndAndRerturn((transportistaDto) e.Row.ListObject);
        }

        private void frmBuscarTransportista_KeyDown(object sender, KeyEventArgs e)
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

        private void OnTimedEvent(object sender, EventArgs myEventArgs)
        {
            _myTimer.Stop();
            lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", Utils.Windows.FiltrarGrilla(grdData, txtBuscarNombre.Text.Trim()));
        }
        #endregion

        #region Methods
        private void BindGrid(string filter)
        {
            var objData = GetData("v_Codigo ASC", filter);
            grdData.DataSource = objData;
            lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", objData.Count);
            grdData.DisplayLayout.PerformAutoResizeColumns(false, Infragistics.Win.UltraWinGrid.PerformAutoSizeType.AllRowsInBand, Infragistics.Win.UltraWinGrid.AutoResizeColumnWidthOptions.All);
        }

        private List<transportistaDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            var objOperationResult = new OperationResult();
            var objData = _objTransportistaBl.ObtenerListadoTransportista(ref objOperationResult, pstrSortExpression, pstrFilterExpression);
            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return objData;
        }

        private void EndAndRerturn(transportistaDto dto)
        {
            StrIdTransportista = dto.v_IdTransportista;
            StrRazonSocial = dto.v_NombreRazonSocial;
            StrCodigo = dto.v_Codigo;
            StrNroDocumento = dto.v_NumeroDocumento;
            Close();
        }
        #endregion
    }
}
