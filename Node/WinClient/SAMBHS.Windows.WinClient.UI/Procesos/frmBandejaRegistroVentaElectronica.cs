using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Venta.BL;
using Infragistics.Win.UltraWinGrid;
using System.Threading;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid.ExcelExport;
using LoadingClass;
using SAMBHS.Almacen.BL;
using SAMBHS.Letras.BL;
using SAMBHS.Windows.NubefactIntegration;
using SAMBHS.Windows.NubefactIntegration.Modelos;
using SAMBHS.Windows.WinClient.UI.Mantenimientos;
using SAMBHS.Windows.WinClient.UI.Mantenimientos.UserControls;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmBandejaRegistroVentaElectronica : Form
    {
        private delegate void EnvioTerminado(int[] bloqueIndices);
        private event EnvioTerminado EnvioTerminadoEvent;
        private Task _tarea;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly VentaBL _objVentaBl = new VentaBL();
        private string _pstrIdVenta;
        private string _vIdCliente = string.Empty;
        private readonly string _modo;
        private bool _seleccionarVentaRapida;

        public frmBandejaRegistroVentaElectronica(string modo)
        {
            InitializeComponent();
            _modo = modo;
        }



        private void frmBandejaRegistroVenta_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;

            var fecha = DateTime.Parse(string.Format("{0}/{1}/{2}", DateTime.Today.Day, DateTime.Today.Month, Globals.ClientSession.i_Periodo));
            dtpFechaInicio.Value = dtpFechaFin.Value = fecha;
            dtpFechaInicio.MaxDate = dtpFechaFin.Value;

            if (Globals.ClientSession.EsEmisorElectronico)
            {
                var col = grdData.DisplayLayout.Bands[0].Columns.Add("imagen", "Sunat");
                col.Header.VisiblePosition = 8;
                col.DataType = typeof(Bitmap);
                col.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Image;
                col.CellAppearance.ImageHAlign = HAlign.Center;
                col.Width = 50;
                grdData.InitializeRow += grdData_InitializeRowFacturacion;
            }

            RecalcularStock();
        }

        private void RecalcularStock()
        {
            try
            {
                if (Globals.ActualizadoStocks) return;
                var op = new OperationResult();
                pbRecalculandoStock.Visible = true;
                var objStockBl = new StockSeparacionBl();
                objStockBl.Terminado += delegate { pbRecalculandoStock.Visible = false; };
                objStockBl.IniciarProceso(ref op, Globals.ClientSession.i_Periodo.ToString(), Globals.ClientSession.i_IdAlmacenPredeterminado ?? -1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {

            if (uvDatos.Validate(true, false).IsValid)
            {
                BindGrid();
                grdData.Focus();
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow != null && !grdData.ActiveRow.IsGroupByRow && !grdData.ActiveRow.IsFilterRow && grdData.ActiveRow.Activation != Activation.Disabled)
            {
                if (Utils.Windows.HaveFormChild(this, typeof(frmRegistroVenta), true)) return;
                _pstrIdVenta = grdData.ActiveRow.Cells["v_IdVenta"].Value.ToString();

                var frm = new frmRegistroVenta("Edicion", _pstrIdVenta);

                #region Eventos
                frm.OnSiguiente += delegate
                {
                    grdData.PerformAction(UltraGridAction.BelowRow);
                    frm.IdRecibido = grdData.ActiveRow.Cells["v_IdVenta"].Value.ToString();
                };

                frm.OnAnterior += delegate
                {
                    grdData.PerformAction(UltraGridAction.AboveRow);
                    frm.IdRecibido = grdData.ActiveRow.Cells["v_IdVenta"].Value.ToString();
                };

                frm.FormClosed += delegate
                {
                    if (frm._modificada)
                        BindGrid();
                    MantenerSeleccion(_pstrIdVenta);
                };
                #endregion

                ((frmMaster)MdiParent).RegistrarForm(this, frm);
            }
        }

        private void BindGrid()
        {
            GetData("NroRegistro ASC");
        }

        private void GetData(string pstrSortExpression)
        {
            var objOperationResult = new OperationResult();
            var objData = new List<ventaDto>();
            var fIni = dtpFechaInicio.Value.Date;
            var fFin = DateTime.Parse(dtpFechaFin.Text + " 23:59");
            Task.Factory.StartNew(() =>
                {
                    Invoke((MethodInvoker)delegate
                    {
                        pBuscando.Visible = true;
                        grdData.Enabled = false;
                    });
                    objData = _objVentaBl.ListarBusquedaVentas(ref objOperationResult, pstrSortExpression, _vIdCliente,
                        fIni, fFin, -1, "", "", -1, Globals.ClientSession.i_IdEstablecimiento ?? -1, true);
                }, _cts.Token)
                .ContinueWith(t =>
                {
                    if (objOperationResult.Success != 1)
                    {
                        UltraMessageBox.Show(
                            "Error en operación:" + Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Invoke((MethodInvoker)delegate
                        {
                            pBuscando.Visible = false;
                            grdData.Enabled = true;
                        });
                        return;
                    }
                    grdData.DataSource = objData;
                    if (objData == null) return;
                    lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", objData.Count);

                    if (Globals.ClientSession.UsuarioEsContable == 0) CierraOperacionesContables();
                    foreach (var row in grdData.Rows)
                    {
                        if (row.Cells["i_EstadoSunat"].Value == null) continue;
                        var estado = (EstadoSunat)int.Parse(row.Cells["i_EstadoSunat"].Value.ToString());
                        row.Cells["EstadoSunat"].SetValue(estado.ToString(), false);
                    }
                    btnEnviarPendientes.Enabled = grdData.Rows.Count > 0;
                }, _cts.Token, TaskContinuationOptions.LongRunning, TaskScheduler.FromCurrentSynchronizationContext())
                .ContinueWith(c =>
                {
                    if (objOperationResult.Success == 1)
                    {
                        
                        Invoke((MethodInvoker)delegate
                        {
                            lblState.Text = @"Revisando Conexión...";
                            lblState.Refresh();
                            var conectado = NubeFacTManager.CheckConnection();
                            if (!conectado) MessageBox.Show(@"Nubefact.com no se encuentra disponible actualmente", @"Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            grdData.Enabled = conectado;
                            pBuscando.Visible = false;
                        });
                    }

                }, TaskScheduler.Default);
        }

        private void MantenerSeleccion(string valorSeleccionado)
        {
            if (string.IsNullOrEmpty(valorSeleccionado)) return;
            var filas = grdData.DisplayLayout.Bands[0].GetRowEnumerator(GridRowType.DataRow).Cast<UltraGridRow>();
            var fila = filas.FirstOrDefault(f => f.Cells["v_IdVenta"].Value.ToString().Contains(valorSeleccionado));
            if (fila != null) fila.Activate();
        }

        private void grdData_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void grdData_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (e.Row.Band.Index == 0 && e.Row.Cells["i_IdEstado"].Value.ToString() == "0")
            {
                e.Row.Appearance.BackColor = Color.Salmon;
                e.Row.Appearance.BackColor2 = Color.Salmon;
                e.Row.Appearance.ForeColor = Color.Black;
            }
        }

        private void grdData_InitializeRowFacturacion(object sender, InitializeRowEventArgs e)
        {
            if (!Globals.ClientSession.EsEmisorElectronico) grdData.InitializeRow -= grdData_InitializeRowFacturacion;
            if (!Globals.ClientSession.EsEmisorElectronico || e.Row.GetCellValue("i_EstadoSunat") == null) return;
            Bitmap ico;
            var estado = (EstadoSunat)Enum.ToObject(typeof(EstadoSunat), (short)e.Row.GetCellValue("i_EstadoSunat"));
            switch (estado)
            {
                case EstadoSunat.PENDIENTE:
                    ico = Resource.alerta;
                    break;
                case EstadoSunat.CDR_ACEPTADO:
                    ico = Resource.tick;
                    break;
                case EstadoSunat.CDR_ACEPTADO_CON_OBSERV:
                    ico = Resource.user_alert;
                    break;
                case EstadoSunat.CDR_RECHAZADO:
                    ico = Resource.door_out;
                    break;
                case EstadoSunat.DE_BAJA:
                    ico = Resource.user_cross;
                    break;
                case EstadoSunat.DE_BAJA_CDR_ACEPTADO:
                    ico = Resource.cross;
                    break;
                case EstadoSunat.DE_BAJA_CDR_RECHAZADO:
                    ico = Resource.cancel;
                    break;
                case EstadoSunat.ENVIADO_POR_CONSULTAR_ESTADO:
                    ico = Resource.clock_pause;
                    break;
                case EstadoSunat.ENVIADO_ANTERIORMENTE:
                    ico = Resource.arrow_undo;
                    break;
                default:
                    ico = Resource.delete;
                    break;
            }
            e.Row.Cells["imagen"].Value = ico;
            var procesando = (bool)e.Row.Cells["_Procesando"].Value;
            e.Row.Cells["_btnEnviar"].Activation = estado != EstadoSunat.PENDIENTE || procesando ? Activation.Disabled : Activation.AllowEdit;
        }

        private void dtpFechaFin_ValueChanged(object sender, EventArgs e)
        {
            dtpFechaInicio.MaxDate = dtpFechaFin.Value;
        }

        private void grdData_DoubleClick(object sender, EventArgs e)
        {
            if (_modo == "GuiaRemisión")
            {
                if (grdData.Rows.Count == 0) return; //se cambio

                if (grdData.ActiveRow != null)
                {

                    if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdVenta"].Value != null)
                    {
                        Close();
                    }
                }
            }
        }

        private void grdData_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F)
            {
                grdData.DisplayLayout.Override.FilterUIType = grdData.DisplayLayout.Override.FilterUIType == FilterUIType.Default ? FilterUIType.FilterRow : FilterUIType.Default;
            }
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            if (uvDatos.Validate(true, false).IsValid)
            {
                BindGrid();
                grdData.Focus();

            }
        }

        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            if (Application.OpenForms["frmRegistroVentaRapida"] != null && grdData.ActiveRow != null && _seleccionarVentaRapida)
            {
                var ventaRapidaForm = (frmRegistroVentaRapida)Application.OpenForms["frmRegistroVentaRapida"];
                var idVenta = grdData.ActiveRow.Cells["v_IdVenta"].Value.ToString();
                ventaRapidaForm.CargarCabecera(idVenta);
            }
        }

        public bool MarcarFilaAnterior()
        {
            _seleccionarVentaRapida = true;
            return grdData.PerformAction(UltraGridAction.AboveRow);
        }

        public bool MarcarFilaSiguiente()
        {
            _seleccionarVentaRapida = true;
            return grdData.PerformAction(UltraGridAction.BelowRow);
        }

        public void ActualizarBandeja()
        {
            _seleccionarVentaRapida = false;
            btnBuscar_Click(this, new EventArgs());
            _seleccionarVentaRapida = true;
            var ultimaFila = grdData.Rows.LastOrDefault();
            if (ultimaFila != null) ultimaFila.Activate();
        }

        private void chkFiltroPersonalizado_CheckedChanged(object sender, EventArgs e)
        {
            Utils.Windows.MostrarOcultarFiltrosGrilla(grdData);
        }

        private void btnExportarBandeja_Click(object sender, EventArgs e)
        {
            if (!grdData.Rows.Any() || (_tarea != null && !_tarea.IsCompleted)) return;

            const string dummyFileName = "Bandeja Ventas";

            using (var sf = new SaveFileDialog
            {
                DefaultExt = "xlsx",
                Filter = @"xlsx files (*.xlsx)|*.xlsx",
                FileName = dummyFileName
            })
            {
                if (sf.ShowDialog() != DialogResult.OK) return;
                var filename = sf.FileName;
                btnExportarBandeja.Appearance.Image = Resource.loadingfinal1;

                _tarea = Task.Factory.StartNew(() =>
                        {
                            using (var ultraGridExcelExporter1 = new UltraGridExcelExporter())
                                ultraGridExcelExporter1.Export(grdData, filename);
                        }, _cts.Token)
                    .ContinueWith(t => ActualizarLabel("Bandeja Exportada a Excel."),
                    TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private void ActualizarLabel(string texto)
        {
            lblDocumentoExportado.Text = texto;
            btnExportarBandeja.Enabled = false;
            btnExportarBandeja.Appearance.Image = Resource.accept;
        }

        private void frmBandejaRegistroVenta_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_tarea != null && !_tarea.IsCompleted)
                _cts.Cancel();
        }

        private void CierraOperacionesContables()
        {
            var filas = grdData.Rows.GetRowEnumerator(GridRowType.DataRow, null, null).Cast<UltraGridRow>().ToList();
            filas.ForEach(fila =>
            {
                if (fila.Cells["v_UsuarioCreacion"].Value != null)
                {
                    var opContable = Globals.UsuariosContables.Contains(fila.Cells["v_UsuarioCreacion"].Value.ToString());
                    fila.Activation = opContable ? Activation.Disabled : Activation.ActivateOnly;
                }
            });
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            var control = new frmConfigFe();
            pConfig.Size = control.Size;
            pConfig.ClientArea.Controls.Clear();
            pConfig.ClientArea.Controls.Add(control);
            pConfig.ClientArea.Controls[0].Dock = DockStyle.Fill;
            pPopUp.Show();
        }

        private void grdData_ClickCellButton(object sender, CellEventArgs e)
        {
            e.Cell.Row.Activate();
            var item = (ventaDto)e.Cell.Row.ListObject;
            if (e.Cell.Column.Key.Equals("_btnEnviar"))
            {
                EnviarComprobante(NubeFacTManager.TipoAccion.EnviarComprobante, new[] { e.Cell.Row.Index });
            }

            if (e.Cell.Column.Key.Equals("_Opciones"))
            {
                contextMenu1.Show(grdData, grdData.PointToClient(Cursor.Position), ToolStripDropDownDirection.BelowLeft);
            }

            if (e.Cell.Column.Key.Equals("_btnPdf"))
            {
                var link = VentaHelper.GetPdf(item.v_IdVenta, VentaHelper.TipoConstancia.ENVIO, VentaHelper.TipoRepresentacion.PDF);
                if (!string.IsNullOrEmpty(link)) 
                System.Diagnostics.Process.Start(link);
                else
                    e.Cell.Activation = Activation.Disabled;
            }

            if (e.Cell.Column.Key.Equals("_btnXml"))
            {
                var link = VentaHelper.GetPdf(item.v_IdVenta, VentaHelper.TipoConstancia.ENVIO, VentaHelper.TipoRepresentacion.XML);
                if (!string.IsNullOrEmpty(link))
                    System.Diagnostics.Process.Start(link);
                else
                    e.Cell.Activation = Activation.Disabled;
            }

            if (e.Cell.Column.Key.Equals("_btnCdr"))
            {
                var link = VentaHelper.GetPdf(item.v_IdVenta, VentaHelper.TipoConstancia.ENVIO, VentaHelper.TipoRepresentacion.CDR);
                if (!string.IsNullOrEmpty(link))
                    System.Diagnostics.Process.Start(link);
                else
                    e.Cell.Activation = Activation.Disabled;
            }

            if (e.Cell.Column.Key.Equals("_btnPdfBaja"))
            {
                var link = VentaHelper.GetPdf(item.v_IdVenta, VentaHelper.TipoConstancia.BAJA, VentaHelper.TipoRepresentacion.PDF);
                if (!string.IsNullOrEmpty(link))
                    System.Diagnostics.Process.Start(link);
                else
                    e.Cell.Activation = Activation.Disabled;
            }

            if (e.Cell.Column.Key.Equals("_btnXmlBaja"))
            {
                var link = VentaHelper.GetPdf(item.v_IdVenta, VentaHelper.TipoConstancia.BAJA, VentaHelper.TipoRepresentacion.XML);
                if (!string.IsNullOrEmpty(link))
                    System.Diagnostics.Process.Start(link);
                else
                    e.Cell.Activation = Activation.Disabled;
            }

            if (e.Cell.Column.Key.Equals("_btnCdrBaja"))
            {
                var link = VentaHelper.GetPdf(item.v_IdVenta, VentaHelper.TipoConstancia.BAJA, VentaHelper.TipoRepresentacion.CDR);
                if (!string.IsNullOrEmpty(link))
                    System.Diagnostics.Process.Start(link);
                else
                    e.Cell.Activation = Activation.Disabled;
            }
        }

        /// <summary>
        /// Retorna items de Menu para comprobantes electronicos
        /// </summary>
        /// <param name="objventa">Object venta</param>
        /// <returns>Array of ToolItems</returns>
        private ToolStripItem[] GetMenuComprobantes(ventaDto objventa)
        {
            var estado = (objventa.i_EstadoSunat == null
                ? EstadoSunat.PENDIENTE
                : (EstadoSunat)Enum.ToObject(typeof(EstadoSunat), objventa.i_EstadoSunat.Value));

            #region Crear opcion de dar de baja
            var tools = new List<ToolStripItem>();
            var estadosEnLosQueSePuedeAnular = estado == EstadoSunat.CDR_ACEPTADO ||
                                               estado == EstadoSunat.CDR_ACEPTADO_CON_OBSERV ||
                                               estado == EstadoSunat.ENVIADO_ANTERIORMENTE ||
                                               estado == EstadoSunat.DE_BAJA_CDR_RECHAZADO;

            if (objventa.i_IdEstado == 0 && estadosEnLosQueSePuedeAnular)
            {
                tools.Add(new ToolStripMenuItem("Dar de Baja", Resource.cross, EnviarDarDeBaja));
            }
            #endregion

            #region Crear opcion de consultar estado
            var estadosEnLosQueSePuedeConsultarEstado = estado == EstadoSunat.ENVIADO_ANTERIORMENTE ||
                                                            estado == EstadoSunat.ERROR_RECEPCION_ENVIO ||
                                                            estado == EstadoSunat.ERROR_RECEPCION_BAJA ||
                                                            estado == EstadoSunat.ENVIADO_POR_CONSULTAR_ESTADO ||
                                                            estado == EstadoSunat.DE_BAJA;

            if (estadosEnLosQueSePuedeConsultarEstado)
            {
                tools.AddRange(new[]
                {
                    new ToolStripMenuItem("Consultar Estado", Resource.email_transfer, ConsultarEstado),
                });
            }
            #endregion

            return tools.ToArray();
        }

        private void ConsultarEstado(object sender, EventArgs e)
        {
            var activeRow = grdData.ActiveRow;
            if (activeRow == null || activeRow.IsFilterRow) return;
            var item = (ventaDto)activeRow.ListObject;
            var estado = (item.i_EstadoSunat == null
                ? EstadoSunat.PENDIENTE
                : (EstadoSunat)Enum.ToObject(typeof(EstadoSunat), item.i_EstadoSunat.Value));

            EnviarComprobante(estado != EstadoSunat.DE_BAJA
                ? NubeFacTManager.TipoAccion.ConsultarComprobante
                : NubeFacTManager.TipoAccion.ConsultarBajaComprobante, new int[] { activeRow.Index });
        }

        private void EnviarDarDeBaja(object sender, EventArgs e)
        {
            var activeRow = grdData.ActiveRow;
            if (activeRow == null || activeRow.IsFilterRow) return;

            EnviarComprobante(NubeFacTManager.TipoAccion.DarBajaComprobante, new int[] { activeRow.Index });
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (((UltraGrid)contextMenu1.SourceControl).ActiveRow == null) return;
            contextMenu1.Items.Clear();
            if (contextMenu1.SourceControl == grdData)
                contextMenu1.Items.AddRange(GetMenuComprobantes((ventaDto)grdData.ActiveRow.ListObject));
            e.Cancel = contextMenu1.Items.Count == 0;
        }

        #region NUBEFACT

        /// <summary>
        /// Método principal que interactua con nubefact para realizar las acciones.
        /// </summary>
        /// <param name="nubefactAction">Accion que deseas ejecutar</param>
        /// <param name="indexRow">Array de indices de las filas que se procesarán en un solo bloque.</param>
        private void EnviarComprobante(NubeFacTManager.TipoAccion nubefactAction, int[] primerBloqueIndex, int[] segundoBloqueIndex = null)
        {
            btnBuscar.Enabled = false;
            var enviados = new List<int>();
            var nfm = new NubeFacTManager
            {
                Ruta = UserConfig.Default.NubefactRuta,
                Token = UserConfig.Default.NubefactToken,
                TipoAccionRealizar = nubefactAction,
                FormatoImpresion = UserConfig.Default.NubefactFormato,
                EnviarAutomaticamente = UserConfig.Default.NubefactAutoEnvio
            };

            nfm.ErrorEvent += OnNfmOnErrorEvent;
            nfm.TerminadoEvent += delegate(IRespuesta rpt, int rowIndex, EstadoSunat estadoSunat)
            {
                btnBuscar.Enabled = true;
                lock (enviados)
                {
                    enviados.Add(rowIndex);
                    var row = grdData.Rows[rowIndex];
                    row.Cells["_btnEnviar"].Activation = Activation.AllowEdit;
                    row.Cells["_Procesando"].SetValue(false, false);
                    var item = (ventaDto)row.ListObject;
                    item.i_EstadoSunat = (short)estadoSunat;
                    row.Refresh(RefreshRow.FireInitializeRow);
                    var error = rpt as RespuestaError;
                    if (error != null)
                        row.Cells["EstadoSunat"].SetValue(error.Errors, false);
                    else row.Cells["EstadoSunat"].SetValue(estadoSunat.ToString(), false);
                }

                if (!primerBloqueIndex.Except(enviados).Any() && segundoBloqueIndex != null && EnvioTerminadoEvent != null)
                    EnvioTerminadoEvent(segundoBloqueIndex);
            };

            nfm.EstadoEvent += nfm_EstadoEvent;

            var motivoBaja = string.Empty;

            if (nubefactAction == NubeFacTManager.TipoAccion.DarBajaComprobante)
                motivoBaja = Microsoft.VisualBasic.Interaction.InputBox("Motivo o Razon por la que se da de Baja este Comprobante", "SUNAT", "CANCELACION");

            foreach (var indexRow in primerBloqueIndex)
            {
                var activeRow = grdData.Rows[indexRow];
                if (activeRow == null || activeRow.IsFilterRow) return;
                var item = (ventaDto)activeRow.ListObject;
                var id = item.v_IdVenta;
                var cel = activeRow.Cells["_btnEnviar"];
                cel.Activation = Activation.Disabled;
                cel.Row.Cells["_Procesando"].SetValue(true, false);

                nfm.Comenzar(id, activeRow.Index, motivoBaja);
            }
        }

        private void nfm_EstadoEvent(string msg, int rowIndex)
        {
            Invoke((MethodInvoker)delegate
            {
                grdData.Rows[rowIndex].Cells["EstadoSunat"].SetValue(msg, false);
            });
        }

        void OnNfmOnErrorEvent(Exception ex, int rowIndex, NubeFacTManager.TipoAccion nubefactAction)
        {
            if (nubefactAction == NubeFacTManager.TipoAccion.EnviarComprobante)
            {
                Invoke((MethodInvoker)delegate { grdData.Rows[rowIndex].Cells["_btnEnviar"].Activation = Activation.AllowEdit; });
            }
            grdData.Rows[rowIndex].Cells["EstadoSunat"].SetValue(ex.Message, false);
        }

        #endregion

        private void btnEnviarPendientes_Click(object sender, EventArgs e)
        {
            try
            {
                var msg = MessageBox.Show(@"¿Seguro de enviar las ventas mostradas con estado Pendiente?",
                   @"Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (msg == System.Windows.Forms.DialogResult.No) return;

                btnEnviarPendientes.Enabled = false;
                grdData.Rows.ToList().ForEach(r => r.Cells["_Procesando"].SetValue(true, false));
                var data = grdData.Rows
                                .Select(r => new { vta = (ventaDto)r.ListObject, index = r.Index })
                                .Where(p => (EstadoSunat)p.vta.i_EstadoSunat == EstadoSunat.PENDIENTE)
                                .Select(o => new { o.index, esNcr = o.vta.i_IdTipoDocumento == 7 }).ToList();

                var cpeNotNcr = data.Where(p => !p.esNcr).Select(i => i.index).ToArray();
                var cpeNcr = data.Where(p => p.esNcr).Select(i => i.index).ToArray();

                EnvioTerminadoEvent += frmBandejaRegistroVentaElectronica_EnvioTerminadoEvent;
                EnviarComprobante(NubeFacTManager.TipoAccion.EnviarComprobante, cpeNotNcr, cpeNcr);
            }
            catch (Exception ex)
            {
                MessageBox.Show(Utils.ExceptionFormatter(ex));
            }
        }

        void frmBandejaRegistroVentaElectronica_EnvioTerminadoEvent(int[] bloqueIndices)
        {
            EnviarComprobante(NubeFacTManager.TipoAccion.EnviarComprobante, bloqueIndices);
            EnvioTerminadoEvent -= frmBandejaRegistroVentaElectronica_EnvioTerminadoEvent;
        }
    }
}
