using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.Common.BE;
using SAMBHS.Compra.BL;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Security.BL;
using SAMBHS.Contabilidad.BL;
using Infragistics.Win.UltraWinGrid.ExcelExport;
using System.Threading;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmBandejaGuiaRemision : Form
    {
        #region Fields
        private string _strFilterExpression, _pstrIdGuiaRemision;
        private readonly GuiaRemisionBL _objGuiaRemisionBl = new GuiaRemisionBL();
        private List<GridKeyValueDTO> _listadoComboDocumentos = new List<GridKeyValueDTO>();
        private bool _btnNuevo, _btnEliminar, _btnModificar;
        private string _vIdCliente = string.Empty;
        private Task _tarea;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        #endregion

        #region Init
        public frmBandejaGuiaRemision(string cadena)
        {
            InitializeComponent();
        }
        private void frmBandejaGuiaRemision_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
            var objOperation = new OperationResult();

            #region ControlAcciones
            var _formActions = new SecurityBL().GetFormAction(ref objOperation, Globals.ClientSession.i_CurrentExecutionNodeId, Globals.ClientSession.i_SystemUserId, "frmBandejaGuiaRemision", Globals.ClientSession.i_RoleId);
            _btnNuevo = Utils.Windows.IsActionEnabled("frmBandejaGuiaRemision_New", _formActions);
            _btnEliminar = Utils.Windows.IsActionEnabled("frmBandejaGuiaRemision_Delete", _formActions);
            _btnModificar = Utils.Windows.IsActionEnabled("frmBandejaGuiaRemision_Edit", _formActions);

            btnAgregar.Enabled = _btnNuevo;
            btnEliminar.Enabled = _btnEliminar;
            btnEditar.Enabled = _btnModificar;
            #endregion

            var filtersComboDocumento = new List<string> { "i_CodigoDocumento== 1 ||i_CodigoDocumento== 3 " };

            var strFilter = filtersComboDocumento.Count > 0 ? string.Join(" && ", filtersComboDocumento) : null;
            _listadoComboDocumentos = _objGuiaRemisionBl.ObtenerDocumentosParaComboGridGuiaRemision(ref objOperation, "", strFilter);
            Utils.Windows.LoadUltraComboList(cboDocumento, "Value2", "Id", _listadoComboDocumentos, DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", Globals.CacheCombosVentaDto.cboMoneda, DropDownListAction.Select);
            cboMoneda.Value = "-1";
            btnEditar.Enabled = false;
            btnEliminar.Enabled = false;
          //  ultraButton2.Visible = Globals.ClientSession.i_SystemUserId == 1;
           // ValidarFechas();
        }
        #endregion

        #region Búsqueda

        private List<guiaremisionDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();

            var objData = _objGuiaRemisionBl.ListarBusquedaGuiaRemision(ref objOperationResult, pstrSortExpression, pstrFilterExpression, dtpFechaInicio.Value.Date, DateTime.Parse(dtpFechaFin.Value.Day.ToString() + "/" + dtpFechaFin.Value.Month.ToString() + "/" + dtpFechaFin.Value.Year.ToString() + " 23:59"));
            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return objData;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            var filters = new Queue<string>();
            if (txtMes.Text != string.Empty) filters.Enqueue("v_Mes==\"" + txtMes.Text + "\"");
            if (txtCorrelativo.Text != string.Empty) filters.Enqueue("v_Correlativo==\"" + txtCorrelativo.Text + "\"");
            if (txtSerieDoc.Text != string.Empty) filters.Enqueue("v_SerieDocumentoRef==\"" + txtSerieDoc.Text + "\"");
            if (txtCorrelativoDoc.Text != string.Empty) filters.Enqueue("v_NumeroDocumentoRef==\"" + txtCorrelativoDoc.Text + "\"");
            if (txtSerieGuia.Text != string.Empty) filters.Enqueue("v_SerieGuiaRemision==\"" + txtSerieGuia.Text + "\"");
            if (txtCorrelativoGuia.Text != string.Empty) filters.Enqueue("v_NumeroGuiaRemision==\"" + txtCorrelativoGuia.Text + "\"");
            if (_vIdCliente != string.Empty) filters.Enqueue("v_IdCliente==\"" + _vIdCliente + "\"");  
            filters.Enqueue("i_IdEstablecimiento==" + (Globals.ClientSession.i_IdEstablecimiento));
            if (cboMoneda.Value.ToString() != "-1") filters.Enqueue("Moneda==\"" + (cboMoneda.Value.ToString().Equals("1") ? "S" : "D") + "\"");
            _strFilterExpression = string.Join(" && ", filters);
            using (new LoadingClass.PleaseWait(Location, "Por favor espere..."))
            BindGrid();
        }

        private void BindGrid()
        {

            var objData = GetData("v_NumeroRegistro ASC", _strFilterExpression);
            grdData.DataSource = objData;

            if (objData == null) return;
            if (objData.Count > 0)
            {
                btnEditar.Enabled = _btnModificar;
                btnEliminar.Enabled = _btnEliminar;
            }
            else
                btnEditar.Enabled = btnEliminar.Enabled = false;
            lblContadorFilas.Text =  @"Se encontraron " + objData.Count+ @" registros.";
            if (Globals.ClientSession.UsuarioEsContable == 0) CierraOperacionesContables();
        }
        private void CierraOperacionesContables()
        {
            //var filas = grdData.Rows.ToList();
            var filas = grdData.Rows.GetRowEnumerator(GridRowType.DataRow, null, null).Cast<UltraGridRow>().ToList();
            filas.ForEach(fila =>
            {
                var opContable = Globals.UsuariosContables.Contains(fila.Cells["v_UsuarioCreacion"].Value.ToString());
                fila.Activation = opContable ? Activation.Disabled : Activation.ActivateOnly;
            });
        }

        #endregion
        
        #region CRUD
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (Utils.Windows.HaveFormChild(this, typeof(frmGuiaRemision))) return;
            var frm = new frmGuiaRemision("Nuevo", "");
            frm.FormClosed += (_, ev)=>
            {
               // BindGrid();
                btnBuscar_Click(sender, e);
                MantenerSeleccion(((frmGuiaRemision)_).PstrIdMovimientoNuevo);
            };
            ((frmMaster)MdiParent).RegistrarForm(this, frm);
        }
        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow || grdData.ActiveRow.Activation == Activation.Disabled) return;
            if (Utils.Windows.HaveFormChild(this, typeof(frmGuiaRemision),true)) return;
            _pstrIdGuiaRemision = grdData.ActiveRow.Cells["v_IdGuiaRemision"].Value.ToString();
            var frm = new frmGuiaRemision("Edicion", _pstrIdGuiaRemision);

            #region Eventos
            frm.OnSiguiente += delegate
            {
                grdData.PerformAction(UltraGridAction.BelowRow);
                frm.IdRecibido = grdData.ActiveRow.Cells["v_IdGuiaRemision"].Value.ToString();
            };

            frm.OnAnterior += delegate
            {
                grdData.PerformAction(UltraGridAction.AboveRow);
                frm.IdRecibido = grdData.ActiveRow.Cells["v_IdGuiaRemision"].Value.ToString();
            };
            frm.FormClosed += (_, ev) =>
            {
                //BindGrid();
                btnBuscar_Click(sender, e);
                MantenerSeleccion(_pstrIdGuiaRemision);
            };
            #endregion

            ((frmMaster)MdiParent).RegistrarForm(this, frm);
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            //new frmBandejaGuiaElectronica("").Show();
            //return;
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow || grdData.ActiveRow.Activation == Activation.Disabled) return;
            
            if (UltraMessageBox.Show("¿Está seguro de Eliminar esta guia de remisión de los registros?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var objOperationResult = new OperationResult();
                _pstrIdGuiaRemision = grdData.ActiveRow.Cells["v_IdGuiaRemision"].Value.ToString();
                _objGuiaRemisionBl.EliminarGuiaRemision(ref objOperationResult, _pstrIdGuiaRemision, Globals.ClientSession.GetAsList());
                btnBuscar_Click(sender, e);
            }
        }
        
        #endregion
       
        #region Grilla
        private void MantenerSeleccion(string valorSeleccionado)
        {
            if (string.IsNullOrEmpty(valorSeleccionado)) return;
            var filas = grdData.Rows.GetRowEnumerator(GridRowType.DataRow, null, null).Cast<UltraGridRow>().ToList();
            var fila = filas.FirstOrDefault(f => f.Cells["v_IdGuiaRemision"].Value.ToString().Contains(valorSeleccionado));
            if (fila != null) fila.Activate();
        }

        private void grdData_MouseDown(object sender, MouseEventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow) return;
            Point point = new Point(e.X, e.Y);
            Infragistics.Win.UIElement uiElement = ((UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

            if (uiElement == null || uiElement.Parent == null) return;

            var row = (UltraGridRow)uiElement.GetContext(typeof(UltraGridRow));

            if (row == null)
            {
                btnEditar.Enabled = false;
                btnEliminar.Enabled = false;
            }
            else
            {
               
                btnEditar.Enabled = _btnModificar;
                btnEliminar.Enabled = _btnEliminar;
                btnEliminar.Enabled = ! new CierreMensualBL().VerificarMesCerrado(grdData.ActiveRow.Cells["v_Periodo"].Value.ToString(), grdData.ActiveRow.Cells["v_Mes"].Value.ToString().Trim().Substring(0, 2), (int)ModulosSistema.VentasFacturacion);
            }
        }

        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow) return;
            if (grdData.ActiveRow != null)
            {

                btnEliminar.Enabled = _btnEliminar;
                btnEditar.Enabled = _btnModificar;
                btnEliminar.Enabled = !new CierreMensualBL().VerificarMesCerrado(grdData.ActiveRow.Cells["v_Periodo"].Value.ToString(), grdData.ActiveRow.Cells["v_Mes"].Value.ToString().Trim().Substring(0, 2), (int)ModulosSistema.VentasFacturacion);
            }
            else
            {
                btnEliminar.Enabled = false;
                btnEditar.Enabled = false;
            }
        }

        private void grdData_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (grdData.ActiveRow == null) return;
            if (btnEditar.Enabled) InvokeOnClick(btnEditar, e);
        }

        private void grdData_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            var id = e.Row.Cells["i_IdEstado"].Value.ToString();
            if (Globals.ClientSession.v_RucEmpresa == Constants.RucHormiguita)
            {
                e.Row.Cells["estado"].Value = id == "2";
            }
            else
            {
                e.Row.Cells["estado"].Value = id == "1";
            }
            if (e.Row.Cells["i_IdEstado"].Value.ToString() == "0")
            {
                e.Row.Appearance.BackColor = Color.Salmon;
                e.Row.Appearance.BackColor2 = Color.Salmon;
                e.Row.Appearance.ForeColor = Color.Black;
            }
        }
        #endregion

        #region ComportamientoControles
        private void cboDocumento_Leave(object sender, EventArgs e)
        {
            if (cboDocumento.Text.Trim() == "")
            {
                cboDocumento.Value = "-1";
            }
            else
            {
                var x = _listadoComboDocumentos.Find(p => p.Id == cboDocumento.Value.ToString() || p.Id == cboDocumento.Text);
                if (x == null)
                {
                    cboDocumento.Value = "-1";
                }
            }
        }

        private void txtMes_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtMes, "{0:00}");
        }

        private void txtCorrelativo_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtCorrelativo, "{0:00000000}");
        }

        private void txtMes_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtMes, e);
        }

        private void txtCorrelativo_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCorrelativo, e);
        }

        private void txtSerieGuia_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtSerieGuia, "{0:0000}");
        }

        private void txtCorrelativoGuia_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtCorrelativoGuia, "{0:00000000}");
        }

        private void txtSerieGuia_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtSerieGuia, e);
        }

        private void txtCorrelativoGuia_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCorrelativoGuia, e);
        }

        private void dtpFechaFin_ValueChanged(object sender, EventArgs e)
        {
            dtpFechaInicio.MaxDate = dtpFechaFin.Value;
        }

        private void txtMes_TextChanged(object sender, EventArgs e)
        {
            if (txtMes.Text == "") return;
            if (int.Parse(txtMes.Text) > 12)
            {
                UltraMessageBox.Show("Por Favor ingrese un N° Registro válido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtMes.Clear();
            }
        }

        private void txtSerieDoc_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtSerieDoc, "{0:0000}");
        }
        private void txtCorrelativoDoc_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtCorrelativoDoc, "{0:00000000}");
        }

        private void txtCliente_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscarCliente")
            {
                using (var frm = new Mantenimientos.frmBuscarCliente("VV", txtCliente.Text.Trim()))
                {
                    frm.ShowDialog();

                    if (frm._IdCliente != null)
                    {
                        txtCliente.Text = string.Format("{0} - {1}", frm._NroDocumento, frm._RazonSocial);
                        _vIdCliente = frm._IdCliente;
                        txtCliente.ButtonsRight["btnEliminar"].Enabled = true;
                    } 
                }
            }
            else
            {
                txtCliente.Clear();
                _vIdCliente = string.Empty;
                txtCliente.ButtonsRight["btnEliminar"].Enabled = false;
            }
        }

        #endregion

        #region Validaciones
        private void ValidarFechas()
        {
            string Periodo = Globals.ClientSession.i_Periodo.ToString();
            string Mes = DateTime.Today.Month.ToString();
            if (DateTime.Now.Year.ToString().Trim() == Periodo)
            {
                dtpFechaInicio.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/" + "01"));
                dtpFechaInicio.MaxDate = DateTime.Parse((Periodo + "/" + Mes + "/" + (DateTime.DaysInMonth(int.Parse(Periodo), int.Parse(Mes)))));
                dtpFechaInicio.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day));

                dtpFechaFin.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/" + "01").ToString());
                dtpFechaFin.MaxDate = DateTime.Parse((Periodo + "/" + Mes + "/" + (DateTime.DaysInMonth(int.Parse(Periodo), int.Parse(Mes))).ToString()).ToString());
                dtpFechaFin.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());

            }
            else
            {
                dtpFechaInicio.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/01").ToString());
                dtpFechaInicio.MaxDate = DateTime.Parse((Periodo + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(Periodo), 12)).ToString()).ToString());
                dtpFechaInicio.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                dtpFechaFin.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/01").ToString());
                dtpFechaFin.MaxDate = DateTime.Parse((Periodo + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(Periodo), 12)).ToString()).ToString());
                dtpFechaFin.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());

            }
        }
        private void chkFiltroPersonalizado_CheckedChanged(object sender, EventArgs e)
        {
            Utils.Windows.MostrarOcultarFiltrosGrilla(grdData);
        }

        private void chkBandejaAgrupable_CheckedChanged(object sender, EventArgs e)
        {
            Utils.Windows.HacerGrillaAgrupable(grdData, chkBandejaAgrupable.Checked);
        }
        private void btnExportarBandeja_Click(object sender, EventArgs e)
        {

            if (!grdData.Rows.Any() || (_tarea != null && !_tarea.IsCompleted)) return;

            const string dummyFileName = "Bandeja Transferencias";
            var sf = new SaveFileDialog
            {
                DefaultExt = "xlsx",
                Filter = @"xlsx files (*.xlsx)|*.xlsx",
                FileName = dummyFileName
            };

            if (sf.ShowDialog() != DialogResult.OK) return;
            btnExportarBandeja.Appearance.Image = Resource.loadingfinal1;
            var ultraGridExcelExporter1 = new UltraGridExcelExporter();
            _tarea = Task.Factory.StartNew(() => { ultraGridExcelExporter1.Export(grdData, sf.FileName); }, _cts.Token)
                                 .ContinueWith(t => ActualizarLabel("Bandeja Exportada a Excel."), TaskScheduler.FromCurrentSynchronizationContext());
        }


        private void ActualizarLabel(string texto)
        {
            lblDocumentoExportado.Text = texto;
            btnExportarBandeja.Enabled = false;
            btnExportarBandeja.Appearance.Image = Resource.accept;
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            if (UltraMessageBox.Show("¿Está seguro de Regenerar la Notas de Salidas?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var objOperationResult = new OperationResult();
                _objGuiaRemisionBl.ActualizarNotasSalidas(ref objOperationResult);
                UltraMessageBox.Show(objOperationResult.Success == 1 ? "El registro se ha guardado correctamente" : "Error", "Sistema");
            }
        }

        #endregion

        //private void ultraButton2_Click(object sender, EventArgs e)
        //{
            //OperationResult objOperationResult = new OperationResult();
            //_objGuiaRemisionBl.Except(ref objOperationResult);
            //if (objOperationResult.Success == 1)
            //{
            //    UltraMessageBox.Show("Proceso terminado correctamente", "Sistema");
            //}
            //else
            //{
            //    UltraMessageBox.Show("Hubo un error al realizar proceso", "Sistema");
            //}
        //}

    }

}
