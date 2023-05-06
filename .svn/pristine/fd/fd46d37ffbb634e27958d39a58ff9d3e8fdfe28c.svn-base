using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using SAMBHS.Cobranza.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Venta.BL;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class FrmConsultaAdelantosParaCanje : Form
    {
        private readonly string _idCliente;
        private OperationResult _objOperationResult = new OperationResult();
        public delegate void Terminado(List<cobranzapendienteDto> resultado);
        public event Terminado OnTerminado;

        public FrmConsultaAdelantosParaCanje(string idCliente)
        {
            _idCliente = idCliente;
            InitializeComponent();
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            var ds = AdelantoBL.ObtenerAdelantosParaCanje(ref _objOperationResult, Globals.ClientSession.i_Periodo ?? DateTime.Now.Year,
                dtpFechaRegistroDe.SelectedValue != null ? int.Parse(dtpFechaRegistroDe.SelectedValue.ToString()) : 1, _idCliente);
            if (_objOperationResult.Success == 0)
            {
                MessageBox.Show(
                    string.Format("{0}\n{1}\n{2}", _objOperationResult.ErrorMessage, _objOperationResult.ExceptionMessage, _objOperationResult.AdditionalInformation), @"Error en la consulta", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            grdData.DataSource = ds;
        }

        /// <summary>
        /// Llena el combo con los doce meses del año.
        /// </summary>
        private void LlenarComboMeses()
        {
         
        }

        protected virtual void OnOnTerminado(List<cobranzapendienteDto> resultado)
        {
            var handler = OnTerminado;
            if (handler != null) handler(resultado);
        }

        private void FrmConsultaAdelantosParaCanje_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
            LlenarComboMeses();
            var cliente = new ClienteBL().ObtenerClientePorID(ref _objOperationResult, _idCliente);
            if (cliente != null)
            {
                txtCliente.Text = cliente.NombreRazonSocial;
                txtCliente.Tag = _idCliente;
            }
        }

        private void btnTerminar_Click(object sender, EventArgs e)
        {
            var filasSeleccionadas = ((List<adelantoDto>)grdData.DataSource).Where(p => p.Seleccion).ToList();
            if (filasSeleccionadas.Any())
            {
                var result = filasSeleccionadas
                    .Select(p => new cobranzapendienteDto
                {
                    NroDocumento = string.Format("{0}-{1}", p.v_SerieDocumento, p.v_CorrelativoDocumento),
                    FechaRegistro = p.t_FechaAdelanto ?? DateTime.Now,
                    d_Saldo = p.d_Saldo ?? 0,
                    Moneda = p.Moneda,
                    i_IdTipoDocumento = p.i_IdTipoDocumento,
                    v_IdVenta = p.v_IdAdelanto,
                    TipoCambio = p.d_TipoCambio ?? 0,
                    TipoDocumento = new DocumentoBL().ObtenDocumentosPorID(ref _objOperationResult, p.i_IdTipoDocumento ?? 1).v_Siglas,
                    EsDocInverso = 0,
                    EsAdelanto = true
                }).ToList();

                if (OnTerminado != null)
                    OnTerminado(result);  
            }
            Close();
        }
    }
}
