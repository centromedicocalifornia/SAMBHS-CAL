using Infragistics.Win.UltraWinGrid;
using SAMBHS.Almacen.BL;
using SAMBHS.Cobranza.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Security.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Requerimientos.NBS;
using SAMBHS.Windows.WinClient.UI.Mantenimientos;
using SAMBHS.Windows.WinClient.UI.Mantenimientos.Migraciones;
using SAMBHS.Windows.WinClient.UI.Requerimientos.NotariaBecerraSosaya;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmBandejaCobranza : Form
    {
        SecurityBL _objSecurityBL = new SecurityBL();
        CobranzaBL _objCobranzaBL = new CobranzaBL();
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        List<GridKeyValueDTO> _ListadoComboDocumentos = new List<GridKeyValueDTO>();
        string _strFilterExpression;
        #region Permisos Botones
        bool _btnAgregar = false;
        bool _btnEditar = false;
        bool _btnEliminar = false;
        #endregion

        public frmBandejaCobranza(string Parametro)
        {
            InitializeComponent();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            List<string> Filters = new List<string>();
            _strFilterExpression = null;
            //if ( cboTipoDocumento.Value!=null && cboTipoDocumento.Value.ToString() != "-1")
            //{
            //    Filters.Add("i_IdTipoDocumento == " + cboTipoDocumento.Value );
            //}

            //if (txtCorrelativoMes.Text.Trim() != string.Empty && txtMes.Text.Trim() != string.Empty)
            //{
            //    Filters.Add("v_Mes == \"" + txtMes.Text + "\" && " + "v_Correlativo == \"" + txtCorrelativoMes.Text + "\"");
            //}
            if (cboMoneda.Value.ToString() != "-1") Filters.Add("Moneda==\"" + (cboMoneda.Value.ToString().Equals("1") ? "S" : "D") + "\"");

            if (Filters.Count > 0)
            {
                foreach (string item in Filters)
                {
                    _strFilterExpression = _strFilterExpression + item + " && ";
                }
                _strFilterExpression = _strFilterExpression.Substring(0, _strFilterExpression.Length - 4);
            }

            using (new LoadingClass.PleaseWait(this.Location, "Por favor espere..."))
            {
                this.BindGrid();
            }
            if (!grdData.Rows.Any())
            {
                btnEliminar.Enabled = false;
                btnEditar.Enabled = false;
            }
        }

        private void frmBandejaCobranza_Load(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            var _ListadoComboDocumentosV = _objDocumentoBL.ObtenDocumentosParaComboGrid(ref objOperationResult, null, 0, 1);
            Utils.Windows.LoadUltraComboList(cboComboDocVentas, "Value2", "Id", _ListadoComboDocumentosV, DropDownListAction.All);
            #region ControlAcciones
            var _formActions = _objSecurityBL.GetFormAction(ref objOperationResult, Globals.ClientSession.i_CurrentExecutionNodeId, Globals.ClientSession.i_SystemUserId, "frmBandejaCobranza", Globals.ClientSession.i_RoleId);

            _btnAgregar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaCobranza_ADD", _formActions);
            _btnEditar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaCobranza_EDIT", _formActions);
            _btnEliminar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaCobranza_DELETE", _formActions);

            btnAgregar.Enabled = _btnAgregar;
            #endregion
            this.BackColor = new GlobalFormColors().FormColor;
            _ListadoComboDocumentos = _objDocumentoBL.ObtenDocumentosCobranzaParaComboGrid(ref objOperationResult, null);
            //Utils.Windows.LoadUltraComboList(cboTipoDocumento, "Value2", "Id", _ListadoComboDocumentos, DropDownListAction.All);
            //cboTipoDocumento.Value = "-1";
            var fecha = DateTime.Parse(string.Format("{0}/{1}/{2}", DateTime.Today.Day, DateTime.Today.Month, Globals.ClientSession.i_Periodo));
            dtpFechaInicio.Value = fecha;
            dtpFechaFin.Value = fecha;
            dtpFechaInicio.MaxDate = dtpFechaFin.Value;
            cboComboDocVentas.Value = "-1";
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", Globals.CacheCombosVentaDto.cboMoneda, DropDownListAction.Select);
            cboMoneda.Value = "-1";
            btnMigrarCobranzas.Visible = Globals.ClientSession.i_SystemUserId.Equals(1);
            btnActualizarSaldos.Visible = Globals.ClientSession.i_SystemUserId.Equals(1);
        }

        private void BindGrid()
        {
            var objData = GetData("NroRegistro ASC", _strFilterExpression);

            grdData.DataSource = objData;
            lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", objData.Count);
        }

        private List<cobranzaDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            var objOperationResult = new OperationResult();
            var idCliente = txtCliente.Tag != null ? txtCliente.Tag.ToString() : null;
            var _objData = _objCobranzaBL.ListarBusquedaCobranzas(ref objOperationResult, pstrSortExpression, pstrFilterExpression, dtpFechaInicio.Value.Date, DateTime.Parse(dtpFechaFin.Value.Day.ToString() + "/" + dtpFechaFin.Value.Month.ToString() + "/" + dtpFechaFin.Value.Year.ToString() + " 23:59"), idCliente);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (Utils.Windows.HaveFormChild(this, typeof(frmCobranza))) return;
            new LoadingClass.PleaseWait(this.Location, "Por favor espere...");
            frmCobranza frm = new frmCobranza("Nuevo", null, null);
            (this.MdiParent as frmMaster).RegistrarForm(this, frm);
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow != null)
            {
                if (grdData.ActiveRow.Index < 0) return;
                if (Utils.Windows.HaveFormChild(this, typeof(frmCobranza), true)) return;
                new LoadingClass.PleaseWait(this.Location, "Por favor espere...");
                var pstrIdCobranza = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdCobranza"].Value.ToString();
                frmCobranza frm = new frmCobranza("Edicion", pstrIdCobranza, null);
                frm.FormClosed += delegate
                {
                    BindGrid();
                    MantenerSeleccion(pstrIdCobranza);
                };
                (this.MdiParent as frmMaster).RegistrarForm(this, frm);
            }
        }

        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow) return;
            if (grdData.ActiveRow != null)
            {
                btnEditar.Enabled = _btnEditar == true ? true : false;
                btnEliminar.Enabled = _btnEliminar == true ? true : false;
            }
            else
            {
                btnEditar.Enabled = false;
                btnEliminar.Enabled = false;
            }
        }

        private void MantenerSeleccion(string ValorSeleccionado)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in grdData.Rows)
            {
                if (row.Cells["v_IdCobranza"].Text == ValorSeleccionado)
                {
                    grdData.ActiveRow = row;
                    grdData.ActiveRow.Selected = true;
                    break;
                }
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow) return;
            var pstrIdCobranza = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdCobranza"].Value.ToString();

            var letrasMantenimiento = CobranzaBL.LetrasEnCobranzaFueronCanceladasProtestadas(pstrIdCobranza);
            if (letrasMantenimiento.Any())
            {
                MessageBox.Show(
                    string.Format("Las siguientes letras tienen mantenimiento: \n - LEC {0}", string.Join("\n - LEC ", letrasMantenimiento)),
                    @"No se pudo eliminar", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            OperationResult _objOperationResult = new OperationResult();
            if (UltraMessageBox.Show("¿Está seguro de Eliminar esta cobranza  de los registros?", "Confirmación",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (new LoadingClass.PleaseWait(this.Location, "Por favor espere..."))
                {

                    _objCobranzaBL.EliminarCobranza(ref _objOperationResult, pstrIdCobranza,
                        Globals.ClientSession.GetAsList());

                    if (_objOperationResult.Success == 0)
                    {
                        UltraMessageBox.Show(
                            _objOperationResult.ErrorMessage + "\n\n" + _objOperationResult.ExceptionMessage +
                            "\n\nTARGET: " + _objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                    else
                    {
                        #region Requerimientos para Notaria Becerra Sosaya

                        if (Globals.ClientSession.v_RucEmpresa.Equals(Constants.RucNotariaBecerrSosaya))
                        {
                            var objDbfSincro = new DbfSincronizador();
                            var objOperationResult2 = new OperationResult();
                            objDbfSincro.RutaDbfCabecera = NBS_DBF_PathSettings.Default.dbfSincro_Cabecera;
                            objDbfSincro.RutaDbfDetalle = NBS_DBF_PathSettings.Default.dbfSincro_Detalle;

                            objDbfSincro.EliminarCobranza(ref objOperationResult2, pstrIdCobranza);

                            if (objOperationResult2.Success == 0)
                            {
                                MessageBox.Show(objOperationResult2.ErrorMessage,
                                    @"Error al sincronizar DBF", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }  
                        }  
                        #endregion
                    }


                    btnBuscar_Click(sender, e);
                }
            }
        }


        private void cboTipoDocumento_AfterDropDown(object sender, EventArgs e)
        {
            //foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboTipoDocumento.Rows)
            //{
            //    if (cboTipoDocumento.Value == "-1") cboTipoDocumento.Text = string.Empty;
            //    bool filterRow = true;
            //    foreach (UltraGridColumn column in cboTipoDocumento.DisplayLayout.Bands[0].Columns)
            //    {
            //        if (column.IsVisibleInLayout)
            //        {
            //            if (row.Cells[column].Text.Contains(cboTipoDocumento.Text.ToUpper()))
            //            {
            //                filterRow = false;
            //                break;
            //            }
            //        }
            //    }
            //    if (filterRow)
            //    {
            //        row.Hidden = true;
            //    }
            //    else
            //    {
            //        row.Hidden = false;
            //    }

            //}
        }

        private void cboTipoDocumento_KeyUp(object sender, KeyEventArgs e)
        {
            //foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboTipoDocumento.Rows)
            //{
            //if (cboTipoDocumento.Value == "-1") cboTipoDocumento.Text = string.Empty;
            //bool filterRow = true;
            //foreach (UltraGridColumn column in cboTipoDocumento.DisplayLayout.Bands[0].Columns)
            //{
            //    if (column.IsVisibleInLayout)
            //    {
            //        if (row.Cells[column].Text.Contains(cboTipoDocumento.Text.ToUpper()))
            //        {
            //            filterRow = false;
            //            break;
            //        }
            //    }
            //}
            //if (filterRow)
            //{
            //    row.Hidden = true;
            //}
            //else
            //{
            //    row.Hidden = false;
            //}

            //}
        }

        private void cboTipoDocumento_Leave(object sender, EventArgs e)
        {
            //if (cboTipoDocumento.Text.Trim() == "")
            //{
            //    cboTipoDocumento.Value = "-1";
            //}
            //else
            //{
            //    var x = _ListadoComboDocumentos.Find(p => p.Id == cboTipoDocumento.Value | p.Id == cboTipoDocumento.Text);
            //    if (x == null)
            //    {
            //        cboTipoDocumento.Value = "-1";
            //    }
            //}
        }

        private void grdData_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            try
            {
                if (e.Row.Cells["i_IdEstado"].Value.ToString() == "0")
                {
                    e.Row.Appearance.BackColor = Color.Salmon;
                    e.Row.Appearance.BackColor2 = Color.Salmon;
                    e.Row.Appearance.ForeColor = Color.Black;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void txtCorrelativoDoc_Validated(object sender, EventArgs e)
        {
            //Utils.Windows.FijarFormato(txtCorrelativoDoc,"00000000");
        }

        private void dtpFechaFin_ValueChanged(object sender, EventArgs e)
        {
            dtpFechaInicio.MaxDate = dtpFechaFin.Value;
        }

        private void txtMes_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Utils.Windows.NumeroEnteroUltraTextBox(txtMes, e);
        }

        private void txtCorrelativoMes_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Utils.Windows.NumeroEnteroUltraTextBox(txtCorrelativoMes, e);
        }

        private void txtCorrelativoMes_Validated(object sender, EventArgs e)
        {
            //Utils.Windows.FijarFormatoUltraTextBox(txtCorrelativoMes, "{0:00000000}");
        }

        private void txtMes_Validated(object sender, EventArgs e)
        {
            //Utils.Windows.FijarFormatoUltraTextBox(txtMes, "{0:00}");
        }

        private void dtpFechaInicio_ValueChanged(object sender, EventArgs e)
        {

        }

        private void grdData_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (grdData.ActiveRow != null)
            {
                if (btnEditar.Enabled) btnEditar_Click(sender, e);
            }
        }

        private void grdData_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtSerieDoc_Validating(object sender, CancelEventArgs e)
        {
            int serie;
            txtSerieDoc.Text = int.TryParse(txtSerieDoc.Text.Trim(), out serie) ? serie.ToString("0000") : string.Empty;
        }

        private void txtCorrelativoDoc_Validating(object sender, CancelEventArgs e)
        {
            int correlativo;
            txtCorrelativoDoc.Text = int.TryParse(txtCorrelativoDoc.Text.Trim(), out correlativo) ? correlativo.ToString("00000000") : string.Empty;
        }

        private void txtCorrelativoDoc_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            int correlativo;
            txtCorrelativoDoc.Text = int.TryParse(txtCorrelativoDoc.Text.Trim(), out correlativo) ? correlativo.ToString("00000000") : string.Empty;

            if (string.IsNullOrEmpty(txtSerieDoc.Text) || string.IsNullOrEmpty(txtCorrelativoDoc.Text) || cboComboDocVentas.Value.ToString() == "-1") return;

            var objOperationResult = new OperationResult();

            var serie = txtSerieDoc.Text;
            var vcorrelativo = txtCorrelativoDoc.Text;
            var idDoc = int.Parse(cboComboDocVentas.Value.ToString());
            var result = _objCobranzaBL.ObtenerCobranzasPorVenta(ref objOperationResult, idDoc, serie, vcorrelativo);
            if (objOperationResult.Success == 0)
            {
                MessageBox.Show(
                    objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
                    objOperationResult.AdditionalInformation, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (result != null)
            {
                var filas = grdData.Rows.ToList();
                var filasSeleccionadas = filas.Where(c => !result.Contains(c.Cells["v_IdCobranza"].Value.ToString())).ToList();
                filasSeleccionadas.ForEach(fila => fila.Hidden = true);
            }
        }

        private void btnMigrarCobranzas_Click(object sender, EventArgs e)
        {
            var f = new frmMigrarCobranzasAdministrativas();
            f.ShowDialog();
        }

        private void btnActualizarSaldos_Click(object sender, EventArgs e)
        {
            var ActulizarSaldos = new frmActualizarSaldosCobranzas();
            ActulizarSaldos.ShowDialog();
        }

        private void txtCliente_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscarCliente")
            {
                frmBuscarCliente frm = new frmBuscarCliente("VV", "");
                frm.ShowDialog();
                if (frm._IdCliente != null)
                {
                    txtCliente.Text = string.Format("{0} - {1}", frm._NroDocumento, frm._RazonSocial);
                    txtCliente.Tag = frm._IdCliente;
                    txtCliente.ButtonsRight["btnEliminar"].Enabled = true;
                }
            }
            else
            {
                txtCliente.Clear();
                txtCliente.Tag = string.Empty;
                txtCliente.ButtonsRight["btnEliminar"].Enabled = false;
            }
        }

        private void chkFiltroPersonalizado_CheckedChanged(object sender, EventArgs e)
        {
            Utils.Windows.MostrarOcultarFiltrosGrilla(grdData);
        }
    }
}
