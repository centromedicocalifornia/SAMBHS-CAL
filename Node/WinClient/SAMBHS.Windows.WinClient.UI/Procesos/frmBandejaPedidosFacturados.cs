using Infragistics.Win.UltraWinGrid;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.Venta.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LoadingClass;
using SAMBHS.Windows.WinClient.UI.Reportes.Ventas;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmBandejaPedidosFacturados : Form
    {
        #region Fields
        private readonly VentaBL _objVentaBL = new VentaBL();
        private List<ventaDto> _dataSource = new List<ventaDto>();
        private string _idPedido;
        #endregion

        #region Init
        public frmBandejaPedidosFacturados(string IdPedido)
        { 
            InitializeComponent();
            _idPedido = IdPedido; 
        }

        private void frmBandejaPedidosFacturados_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            OperationResult pOperationResult = new OperationResult();
            _dataSource = _objVentaBL.ListarBusquedaVentasJerarquica(ref pOperationResult, null, _idPedido);

            if (_dataSource.Count > 0)
            {
                lblCliente.Text = _dataSource[0].NombreCliente;
                txtTotal.Text = _dataSource.Sum(p => p.d_Total.Value).ToString("0.00");
                grdData.DataSource = _dataSource;

                bool documentoImpreso;
                using (new PleaseWait(this.Location, "Imprimiendo..."))
                {
                    documentoImpreso = Imprimir();
                }
                btnAgregar.Enabled = documentoImpreso;
                if (documentoImpreso) Cobrar();
               // Cobrar();
            }
        }
        #endregion

        #region Events
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            Cobrar();
        }

        private void frmBandejaPedidosFacturados_FormClosing(object sender, FormClosingEventArgs e)
        {

            e.Cancel = btnImprimir.Enabled &&
                       MessageBox.Show(@"¿Seguro de salir sin imprimir el comprobante?", @"Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.No;
        }

        private void grdData_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (grdData.Rows.Count > 0)
                e.Row.Cells["Index"].Value = (e.Row.Index + 1).ToString("00");
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            Imprimir();        
        }

        private void frmBandejaPedidosFacturados_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keys.KeyCode == Keys.Escape)
                Close();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Levanta el formulario de cobranza despues que la impresion se haya dado correctamente.
        /// </summary>
        private void Cobrar()
        {
            if (grdData.Rows.Count == 1)
            {
                if (_objVentaBL.TieneCobranzaPendiente(_dataSource[0].v_IdVenta))
                {
                    frmCobranzaRapida frm = new frmCobranzaRapida(grdData.Rows[0].Cells["v_IdVenta"].Value.ToString(), 0, 0);
                    frm.ShowDialog();
                }
            }
            else
            {
                if (_objVentaBL.TieneCobranzaPendiente(_dataSource[0].v_IdVenta))
                {
                    List<string[]> ListaVentas = new List<string[]>();
                    foreach (UltraGridRow row in grdData.Rows)
                    {
                        string[] Cadena = new string[2];
                        Cadena[0] = row.Cells["v_IdVenta"].Value.ToString();
                        Cadena[1] = row.Cells["d_Total"].Value.ToString();
                        ListaVentas.Add(Cadena);
                    }
                    frmCobranza f = new frmCobranza("Nuevo", "", ListaVentas);
                    f.ShowDialog();
                }
            }
        }

        /// <summary>
        /// Imprime el documento ya sea boleta o factura y devuelve verdadero si la impresión se realizó correctamente.
        /// </summary>
        /// <returns></returns>
        private bool Imprimir()
        {
            try
            {
                bool res = false;
                //var IdVentas = grdData.Rows.Select(p => p.Cells["v_IdVenta"].Value.ToString()).Distinct().ToList();
                var IdVentas = _dataSource.Where(r => r.i_IdTipoDocumento == (int)DocumentType.Factura ||
                                r.i_IdTipoDocumento == (int)DocumentType.Boleta).Select(g => g.v_IdVenta).ToList();
                if (IdVentas != null && IdVentas.Count > 0)
                {
                    int IdDocumento = int.Parse(grdData.Rows[0].Cells["i_IdTipoDocumento"].Value.ToString());
                    if (IdDocumento == (int)DocumentType.Factura)
                    {

                        if (Globals.ClientSession.v_RucEmpresa == Constants.RucChayna)
                        {
                            var doc = new DocFactura(IdVentas);
                            doc.Print();
                            res = doc.Success;
                        }
                        else

                        using (Reportes.Ventas.frmDocumentoFacturaRapida frm = new Reportes.Ventas.frmDocumentoFacturaRapida(IdVentas))
                        {
                            btnImprimir.Enabled = false;
                            res = frm.ImpresionExitosa;
                        }
                    }
                    else if (IdDocumento == (int)DocumentType.Boleta)
                    {

                        if (Globals.ClientSession.v_RucEmpresa == Constants.RucChayna)
                        {
                            var doc = new DocBoleta(IdVentas);
                            doc.Print();
                            res = doc.Success;
                        }
                        else

                       using ( Reportes.Ventas.frmDocumentoBoletaRapida frm = new Reportes.Ventas.frmDocumentoBoletaRapida(IdVentas))
                        {
                        btnImprimir.Enabled = false;
                        res = frm.ImpresionExitosa;
                        }
                    }

                }
                var idTickets = _dataSource.Where(r => r.i_IdTipoDocumento == (int)DocumentType.TicketBoleta).Select(g => g.v_IdVenta).ToArray();
                if (idTickets.Length > 0)
                {
                    var printer = new Reportes.Ventas.Ablimatex.Ticket(idTickets);
                    printer.Print();
                    res = true;
                }
                return res;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        #endregion
    }
}
