using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.Requerimientos.NBS;

namespace SAMBHS.Windows.WinClient.UI.Requerimientos.NotariaBecerraSosaya
{
    public partial class FrmClientesPorKardex : Form
    {
        private string _mensajeLabel;
        private clienteDto _clienteDto;

        public clienteDto ClienteDto
        {
            get { return _clienteDto; }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
        }

        public string RucCliente;

        public FrmClientesPorKardex(IEnumerable<DbfConnector.KardexClienteDto> datasource)
        {
            InitializeComponent();
            grdData.DataSource = datasource.ToList();
        }

        private void frmClientesPorKardex_Load(object sender, EventArgs e)
        {
            Activate();
            _mensajeLabel = ultraLabel1.Text;
            BackColor = new GlobalFormColors().FormColor;
            if (!grdData.Rows.Any()) return;
            var ultimaFila = grdData.Rows.LastOrDefault();
            if (ultimaFila != null)
                ultimaFila.Activate();
        }

        private void grdData_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Alt && e.KeyCode == Keys.A)
                {
                    var clientManager = new ClientSincroManager();
                    clientManager.ErrorEvent += clientManager_ErrorEvent;
                    var kardexCliente = (DbfConnector.KardexClienteDto)grdData.ActiveRow.ListObject;
                    RucCliente = kardexCliente.NroDocumento;
                    _clienteDto = clientManager.ObtenerRegistrarCliente(kardexCliente);
                    if (_clienteDto == null) return;
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void clientManager_ErrorEvent(string error)
        {
            _errorMessage = error;
            DialogResult = DialogResult.Abort;
            Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null) return;
            var boundedEntity = (DbfConnector.KardexClienteDto)grdData.ActiveRow.ListObject;
            if (boundedEntity == null) return;
            if (string.IsNullOrWhiteSpace(boundedEntity.NroDocumento) ||
                string.IsNullOrWhiteSpace(boundedEntity.Codigo) ||
                string.IsNullOrWhiteSpace(boundedEntity.NombreApellidosRazonSocial) ||
                (boundedEntity.TipoPersona.Equals("J") && string.IsNullOrWhiteSpace(boundedEntity.Direccion)))
            {
                ultraLabel1.Text = @"Se deberá registrar manualmente a este cliente";
                pbEstadoCondicion.Image = Resource.alerta;
            }
            else
            {
                ultraLabel1.Text = _mensajeLabel;
                pbEstadoCondicion.Image = Resource.asterisk_orange;
            }
        }
    }
}
