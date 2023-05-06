using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Contabilidad.BL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmAdministracionConcepto_2 : Form
    {
        #region Properties
        private administracionconceptosDto _administracionconceptosDto = new administracionconceptosDto();
        readonly AdministracionConceptosBL _objAdministracionConceptosBl = new AdministracionConceptosBL();
        readonly AsientosContablesBL _objAsientosContablesBl = new AsientosContablesBL();

        private asientocontableDto _cuentaVenta = new asientocontableDto();
        private asientocontableDto _cuentaIgv = new asientocontableDto();
        private asientocontableDto _cuentaDetraccion = new asientocontableDto();

        private readonly ConceptoBL _objConceptoBl = new ConceptoBL();
        private conceptoDto _conceptoDto;
        private string _mode;
        #endregion

        #region Construct
        public frmAdministracionConcepto_2(string n)
        {
            InitializeComponent();

            Load += delegate
            {
                BackColor = new GlobalFormColors().FormColor;
                var objOperationResult = new OperationResult();
                Utils.Windows.LoadUltraComboEditorList(cboArea, "Value1", "Id", new DatahierarchyBL().GetDataHierarchiesForCombo(ref objOperationResult, 28, null),
                                               DropDownListAction.Select);
                cboArea.SelectedIndex = 0;
                btnEliminar.Enabled = false;
                btnEditar.Enabled = false;
                btnCancel.Click += delegate
                {
                    DetalleOff();
                    txtBuscarCodigo.Focus();
                };
                LoadEventosTextBox();
                grdData.KeyDown += (sender, e) =>
                {
                    switch (e.KeyCode)
                    {
                        case Keys.Enter:
                            InvokeOnClick(btnEditar, e);
                            break;
                        case Keys.Delete:
                            InvokeOnClick(btnEliminar, e);
                            break;
                    }
                };
                DetalleOff();
                txtCodigo.Enabled = false;
            };
        }
        #endregion

        #region Events UI

        #region Grilla

        private void grdData_MouseDown(object sender, MouseEventArgs e)
        {
            var point = new Point(e.X, e.Y);
            var uiElement = ((Infragistics.Win.UltraWinGrid.UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

            if (uiElement == null || uiElement.Parent == null) return;

            Infragistics.Win.UltraWinGrid.UltraGridRow row = (Infragistics.Win.UltraWinGrid.UltraGridRow)uiElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridRow));

            if (row == null)
            {
                btnEliminar.Enabled = false;
                btnEditar.Enabled = false;
            }
            else
            {
                btnEditar.Enabled = true;
                btnEliminar.Enabled = true;
            }
        }

        private void grdData_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            btnEditar_Click(sender, e);
        }

        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || !panelDatosConcepto.Visible) return;
            var obj = grdData.ActiveRow.ListObject as administracionconceptosDto;
            if (obj == null) return;
            CargarDetalle(obj.v_IdAdministracionConceptos, obj.v_Codigo);
        }
        #endregion

        #region Datos Concepto
        private void btnGrabar_Click(object sender, EventArgs e)
        {

            #region Validacion de Campos
            if (txtCodigo.Text.Trim().Equals(""))
            {
                UltraMessageBox.Show("Por favor ingrese un Codigo Correcto.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (txtNombre.Text.Trim().Equals(""))
            {
                UltraMessageBox.Show("Por favor ingrese un Nombre de Concepto Correcto", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return;
            }
            if (cboArea.SelectedIndex < 1)
            {
                UltraMessageBox.Show("Por favor ingrese un Tipo de Area Correcto", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboArea.Focus();
                return;
            }
            if (txtCuentaVenta.Text.Trim() == "" || _cuentaVenta == null || !Utils.Windows.EsCuentaImputable(txtCuentaVenta.Text.Trim()))
            {
                UltraMessageBox.Show("Por favor ingrese una Cuenta Venta Imputable Valida.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtCuentaVenta.Focus();
                return;
            }

            if (txtCuentaIgv.Text.Trim() != String.Empty && (_cuentaIgv == null || !Utils.Windows.EsCuentaImputable(txtCuentaIgv.Text.Trim())))
            {
                UltraMessageBox.Show("Por favor ingrese Cuenta Igv Imputable", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtCuentaIgv.Focus();
                return;

            }
            if (txtCuentaDetraccion.Text.Trim() != string.Empty && (_cuentaDetraccion == null || !Utils.Windows.EsCuentaImputable(txtCuentaDetraccion.Text.Trim())))
            {
                UltraMessageBox.Show("Por favor ingrese Cuenta Detracción Imputable", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);

                txtCuentaDetraccion.Focus();
                return;
            }
            #endregion

            var idAdministracion = string.Empty;
            var objOperationResult = new OperationResult();

            using (var objTransaccion = TransactionUtils.CreateTransactionScope())
            {
                // Concepto
                if (_mode == "New")
                {
                    _conceptoDto = new conceptoDto
                    {
                        v_Codigo = txtCodigo.Text.Trim(),
                        v_Nombre = txtNombre.Text.Trim(),
                        i_IdArea = int.Parse(cboArea.Value.ToString()),
                        v_Periodo =Globals.ClientSession.i_Periodo.ToString (),
                    };

                    _objConceptoBl.InsertarConcepto(ref objOperationResult, _conceptoDto, Globals.ClientSession.GetAsList());
                    if (objOperationResult.Success == 0) goto FinUsingTrans;
                }
                else // _mode == "Edit" || "EditNew"
                {
                    _conceptoDto.v_Codigo = txtCodigo.Text.Trim();
                    _conceptoDto.v_Nombre = txtNombre.Text.Trim();
                    _conceptoDto.i_IdArea = int.Parse(cboArea.Value.ToString());
                    _conceptoDto.v_Periodo = Globals.ClientSession.i_Periodo.ToString();
                    _objConceptoBl.ActualizarConcepto(ref objOperationResult, _conceptoDto, Globals.ClientSession.GetAsList());
                    if (objOperationResult.Success == 0) goto FinUsingTrans;
                }

                //Administracion de Conceptos
                if (_mode == "New" || _mode == "EditNew")
                {
                    _administracionconceptosDto = new administracionconceptosDto
                    {
                        v_Codigo = txtCodigo.Text.Trim(),
                        v_Nombre = txtNombre.Text.Trim(),
                        v_CuentaPVenta = txtCuentaVenta.Text.Trim(),
                        v_CuentaIGV = txtCuentaIgv.Text.Trim(),
                        v_CuentaDetraccion = txtCuentaDetraccion.Text.Trim(),
                        v_Periodo =Globals.ClientSession.i_Periodo.ToString (),
                    };
                    idAdministracion = _objAdministracionConceptosBl.InsertarAdministracionConceptos(ref objOperationResult, _administracionconceptosDto, Globals.ClientSession.GetAsList());
                    if (objOperationResult.Success == 0) goto FinUsingTrans;
                }
                else // _mode = "Edit"
                {
                    _administracionconceptosDto.v_Codigo = txtCodigo.Text.Trim();
                    _administracionconceptosDto.v_Nombre = txtNombre.Text.Trim();
                    _administracionconceptosDto.v_CuentaPVenta = txtCuentaVenta.Text.Trim();
                    _administracionconceptosDto.v_CuentaIGV = txtCuentaIgv.Text.Trim();
                    _administracionconceptosDto.v_CuentaDetraccion = txtCuentaDetraccion.Text.Trim();
                    _administracionconceptosDto.v_Periodo = Globals.ClientSession.i_Periodo.ToString();
                    idAdministracion = _objAdministracionConceptosBl.ActualizarAdministracionConceptos(ref objOperationResult, _administracionconceptosDto, Globals.ClientSession.GetAsList());
                    if (objOperationResult.Success == 0) goto FinUsingTrans;
                }
                objTransaccion.Complete();
            }
        FinUsingTrans:

            if (objOperationResult.Success == 1)
            {
                btnBuscar_Click(sender, e);
                MantenerSeleccion(idAdministracion);
                DetalleOff();
                UltraMessageBox.Show("El registro se ha " + (_mode.Equals("New") ? "guardado" : "actualizado") + " correctamente", "Sistemas");
            }
            else
                UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnBuscarCventa_Click(object sender, EventArgs e)
        {
            var frm = new frmPlanCuentasConsulta(txtCuentaVenta.Text.Trim()); // le mando N de que se esta utilizando en la NOTA DE SALIDA
            frm.ShowDialog();

            if (frm._NroSubCuenta == null) return;
            txtCuentaVenta.Text = frm._NroSubCuenta;
            txtCventa.Text = frm._NombreCuenta;
            _cuentaVenta = _objAsientosContablesBl.ObtenerAsientoImputable(txtCuentaVenta.Text.Trim());
        }

        private void btnBuscarCigv_Click(object sender, EventArgs e)
        {
            var frm = new frmPlanCuentasConsulta(txtCuentaIgv.Text.Trim()); // le mando N de que se esta utilizando en la NOTA DE SALIDA
            frm.ShowDialog();

            if (frm._NroSubCuenta == null) return;
            txtCuentaIgv.Text = frm._NroSubCuenta;

            txtCigv.Text = frm._NombreCuenta;
            _cuentaIgv = _objAsientosContablesBl.ObtenerAsientoImputable(txtCuentaIgv.Text.Trim());
        }

        private void btnBuscarCdetraccion_Click(object sender, EventArgs e)
        {
            var frm = new frmPlanCuentasConsulta(txtCuentaDetraccion.Text.Trim()); // le mando N de que se esta utilizando en la NOTA DE SALIDA
            frm.ShowDialog();

            if (frm._NroSubCuenta == null) return;
            txtCuentaDetraccion.Text = frm._NroSubCuenta;
            txtCdetraccion.Text = frm._NombreCuenta;
            _cuentaDetraccion = _objAsientosContablesBl.ObtenerAsientoImputable(txtCuentaDetraccion.Text.Trim());
        }
        #endregion

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            var filters = new List<string>();

            if (!string.IsNullOrEmpty(txtBuscarCodigo.Text)) filters.Add("v_Codigo.Contains(\"" + txtBuscarCodigo.Text.Trim().ToUpper() + "\")");
            if (!string.IsNullOrEmpty(txtBuscarNombre.Text)) filters.Add("v_Nombre.Contains(\"" + txtBuscarNombre.Text.Trim().ToUpper() + "\")");
            string strFilterExpression = null;

            if (filters.Any())
                strFilterExpression = string.Join(" && ", filters);

            BindGrid(strFilterExpression);
            btnEliminar.Enabled = btnEditar.Enabled = grdData.Rows.Any();
        }
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            _mode = "New";
            LimpiarDetalle();
            DetalleOn();
            txtCodigo.Enabled = true;
            txtCodigo.Text = GetSuggestCode();
            txtCodigo.Focus();
        }
        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null) return;
            var obj = grdData.ActiveRow.ListObject as administracionconceptosDto;
            if(obj == null) return;
            CargarDetalle(obj.v_IdAdministracionConceptos, obj.v_Codigo);
            DetalleOn();
            txtNombre.Focus();
        }
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if(grdData.ActiveRow == null) return;
            string codigo = grdData.ActiveRow.Cells["v_Codigo"].Value.ToString();
            
            if (!_objAdministracionConceptosBl.ExistenciaAdmConceptosDiversosProcesos(int.Parse(codigo)) && !EsCodigoRequerido(codigo))
            {
                var resultado = UltraMessageBox.Show("¿Está seguro de eliminar el registro con codigo \"" + codigo +"\"?", "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (resultado != DialogResult.Yes) return;
                var objOperationResult = new OperationResult();
                    
                using (var objTransaccion = TransactionUtils.CreateTransactionScope())
                {
                    var idConcepto = _objConceptoBl.ObtenerCodigoConcepto(ref objOperationResult, codigo).v_IdConcepto;
                    _objConceptoBl.EliminarConcepto(ref objOperationResult, idConcepto, Globals.ClientSession.GetAsList());
                    if (objOperationResult.Success == 0) goto FinUsing; // No pudo eliminar Concepto
                    _objAdministracionConceptosBl.EliminarAdministracionConceptos(ref objOperationResult, grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdAdministracionConceptos"].Value.ToString(), Globals.ClientSession.GetAsList());
                    if (objOperationResult.Success == 1)
                        objTransaccion.Complete();
                }
                FinUsing:
                if (objOperationResult.Success == 1)
                {
                    DetalleOff();
                    btnBuscar_Click(sender, e);
                }
                else
                    UltraMessageBox.Show("No se pudo Eliminar los Elemento" + Environment.NewLine + objOperationResult.ErrorMessage,
                        "Error", Icono : MessageBoxIcon.Error);
            }
            else
                UltraMessageBox.Show("No se puede eliminar,este código esta siendo utilizado en Otros procesos", "ADVERTENCIA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        #endregion

        #region Methods
        private bool EsCodigoRequerido(string pstrCodigo)
        {
            int cod;
            if (!int.TryParse(pstrCodigo, out cod)) return false;
            return cod <= 20 || cod == 50 || cod == 51;
        }
        private void DetalleOff()
        {
            panelDatosConcepto.Visible = false;
            txtCodigo.Enabled = false;
        }
        private void DetalleOn()
        {
            panelDatosConcepto.Visible = true;
        }
        private void LimpiarDetalle()
        {
            txtNombre.Clear();
            txtCodigo.Clear();
            cboArea.SelectedIndex = 0;
            _cuentaVenta = null;
            _cuentaDetraccion = null;
            _cuentaIgv = null;
            txtCuentaVenta.Clear();
            txtCuentaIgv.Clear();
            txtCuentaDetraccion.Clear();
        }
        /// <summary>
        ///  Se asocian Eventos a los TextBox del Form
        /// </summary> 
        private void LoadEventosTextBox()
        {
            #region Events TextBox Codigo
            txtCodigo.KeyPress += (sender, e) =>
            {
                Utils.Windows.NumeroEnteroUltraTextBox(txtCodigo, e);
            };

            txtCodigo.Leave += delegate
            {
                if (txtCodigo.Text.Trim() == string.Empty) { LimpiarDetalle(); return; }

                string codigo = int.Parse(txtCodigo.Text.Trim()).ToString("00");
                txtCodigo.Text = codigo;
                var objOperatioResult = new OperationResult();
                var existeConcepto = _objConceptoBl.ObtenerCodigoConcepto(ref objOperatioResult, codigo.Trim());
                if (existeConcepto != null)
                {
                    _conceptoDto = existeConcepto;
                    txtNombre.Text = _conceptoDto.v_Nombre.Trim();
                    cboArea.Value = _conceptoDto.i_IdArea.ToString();
                    txtNombre.Enabled = !EsCodigoRequerido(_conceptoDto.v_Codigo); //Disable
                }
                else
                {
                    cboArea.SelectedIndex = 0;
                    txtNombre.Clear();
                }
                var administracionConcepto = _objAdministracionConceptosBl.ObtenerAdministracionConceptosxCod(ref objOperatioResult, txtCodigo.Text.Trim());
                if (administracionConcepto != null)
                {
                    _administracionconceptosDto = administracionConcepto;
                    txtCuentaDetraccion.Text = _administracionconceptosDto.v_CuentaDetraccion;
                    txtCuentaIgv.Text = _administracionconceptosDto.v_CuentaIGV;
                    txtCuentaVenta.Text = _administracionconceptosDto.v_CuentaPVenta;
                    //var Cventa=_objAsientosContablesBL.ObtenAsientosPorNro();
                    _cuentaVenta = _objAsientosContablesBl.ObtenAsientosPorNro(ref objOperatioResult, txtCuentaVenta.Text);
                    if (_cuentaVenta != null)
                        txtCventa.Text = _cuentaVenta.v_NombreCuenta;
                    else
                        txtCventa.Clear();

                    _cuentaIgv = _objAsientosContablesBl.ObtenAsientosPorNro(ref objOperatioResult, txtCuentaIgv.Text);
                    if (_cuentaIgv != null)
                        txtCigv.Text = _cuentaIgv.v_NombreCuenta;
                    else
                        txtCigv.Clear();

                    _cuentaDetraccion = _objAsientosContablesBl.ObtenAsientosPorNro(ref objOperatioResult, txtCuentaDetraccion.Text);
                    if (_cuentaDetraccion != null)
                        txtCdetraccion.Text = _cuentaDetraccion.v_NombreCuenta;
                    else
                        txtCdetraccion.Clear();

                    _mode = "Edit";
                }
                else
                {
                    LimpiarDetalle();
                    if (existeConcepto != null)
                    {
                        _mode = "EditNew";
                        txtCodigo.Text = _conceptoDto.v_Codigo;
                        txtNombre.Text = _conceptoDto.v_Nombre.Trim();
                        cboArea.Value = _conceptoDto.i_IdArea.ToString();
                    }
                    else
                    {
                        txtCodigo.Text = codigo;
                        _mode = "New";
                    }

                }
            };
            #endregion
            #region Eventos Textbox Cuenta Venta
            txtCuentaVenta.TextChanged += delegate
            {
                if (txtCuentaVenta.Text == "") txtCventa.Clear();
            };
            txtCuentaVenta.KeyPress += (sender, e) =>
            {
                Utils.Windows.NumeroEnteroUltraTextBox(txtCuentaVenta, e);
            };
            txtCuentaVenta.Leave += delegate
            {
                _cuentaVenta = _objAsientosContablesBl.ObtenerAsientoImputable(txtCuentaVenta.Text.Trim());
                txtCventa.Text = (_cuentaVenta != null) ? _cuentaVenta.v_NombreCuenta : "";
            };
            #endregion
            #region Eventos Textbox Cuenta Detraccion
            txtCuentaDetraccion.TextChanged += delegate
            {
                if (txtCuentaDetraccion.Text == "") txtCdetraccion.Clear();
            };
            txtCuentaDetraccion.KeyPress += (sender, e) =>
            {
                Utils.Windows.NumeroEnteroUltraTextBox(txtCuentaDetraccion, e);
            };
            txtCuentaDetraccion.Leave += delegate
            {
                _cuentaDetraccion = _objAsientosContablesBl.ObtenerAsientoImputable(txtCuentaDetraccion.Text.Trim());
                txtCdetraccion.Text = (_cuentaDetraccion != null) ? _cuentaDetraccion.v_NombreCuenta : "";
            };
            #endregion
            #region Eventos Textbox Cuenta IGV
            txtCuentaIgv.TextChanged += delegate
            {
                if (txtCuentaIgv.Text == "") txtCigv.Clear();
            };
            txtCuentaIgv.KeyPress += (sender, e) =>
            {
                Utils.Windows.NumeroEnteroUltraTextBox(txtCuentaIgv, e);
            };
            txtCuentaIgv.Leave += delegate
            {
                _cuentaIgv = _objAsientosContablesBl.ObtenerAsientoImputable(txtCuentaIgv.Text.Trim());
                txtCigv.Text = (_cuentaIgv != null) ? _cuentaIgv.v_NombreCuenta : "";
            };
            #endregion
            #region TextBox Busqueda
            txtBuscarCodigo.KeyPress += (sender, e) =>
            {
                Utils.Windows.NumeroEnteroUltraTextBox(txtBuscarCodigo, e);
            };
            txtBuscarCodigo.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                    btnBuscar_Click(sender, e);
            };
            txtBuscarNombre.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnBuscar_Click(sender, e);

                }
            };
            #endregion
        }
        private void BindGrid(string filter)
        {
            var objData = GetData("v_Codigo ASC", filter);
            grdData.DataSource = objData;
            grdData.DisplayLayout.Bands[0].Columns["v_Codigo"].PerformAutoResize(Infragistics.Win.UltraWinGrid.PerformAutoSizeType.AllRowsInBand, Infragistics.Win.UltraWinGrid.AutoResizeColumnWidthOptions.All);
        }
        private List<administracionconceptosDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            var objOperationResult = new OperationResult();
            var objData = _objAdministracionConceptosBl.ObtenerListadoAdministracionConceptos(ref objOperationResult, pstrSortExpression, pstrFilterExpression);
            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return objData;
        }
        /// <summary>
        /// Se carga el datos del Concepto y Administracion de concepto, para edicion
        /// </summary>
        /// <param name="idAdministracionConceptos">Id del Administracion de Concepto</param>
        /// <param name="codigoConcepto">Codigo del concepto</param>
        private void CargarDetalle(string idAdministracionConceptos, string codigoConcepto)
        {
            var objOperationResult = new OperationResult();
            _mode = "Edit";

            _conceptoDto = new conceptoDto();
            _conceptoDto = _objConceptoBl.ObtenerCodigoConcepto(ref objOperationResult, codigoConcepto);
            if (_conceptoDto != null)
            {
                txtCodigo.Text = _conceptoDto.v_Codigo;
                txtNombre.Enabled = !EsCodigoRequerido(_conceptoDto.v_Codigo); //Disable
                txtNombre.Text = _conceptoDto.v_Nombre;
                cboArea.Value = _conceptoDto.i_IdArea.ToString();
                _administracionconceptosDto = new administracionconceptosDto();
                _administracionconceptosDto = _objAdministracionConceptosBl.ObtenerAdministracionConceptos(ref objOperationResult, idAdministracionConceptos);
                txtCuentaVenta.Text = _administracionconceptosDto.v_CuentaPVenta;
                txtCuentaIgv.Text = _administracionconceptosDto.v_CuentaIGV;
                txtCuentaDetraccion.Text = _administracionconceptosDto.v_CuentaDetraccion;
                _cuentaVenta = _objAsientosContablesBl.ObtenAsientosPorNro(ref objOperationResult, txtCuentaVenta.Text);
                if (_cuentaVenta != null)
                    txtCventa.Text = _cuentaVenta.v_NombreCuenta;
                else
                    txtCventa.Clear();

                _cuentaIgv = _objAsientosContablesBl.ObtenAsientosPorNro(ref objOperationResult, txtCuentaIgv.Text);
                if (_cuentaIgv != null)
                    txtCigv.Text = _cuentaIgv.v_NombreCuenta;
                else
                    txtCigv.Clear();

                _cuentaDetraccion = _objAsientosContablesBl.ObtenAsientosPorNro(ref objOperationResult, txtCuentaDetraccion.Text);
                if (_cuentaDetraccion != null)
                    txtCdetraccion.Text = _cuentaDetraccion.v_NombreCuenta;
                else
                    txtCdetraccion.Clear();
            }
            else
            {
                LimpiarDetalle();
            }
        }
        private void MantenerSeleccion(string valorSeleccionado)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in grdData.Rows)
            {
                if (row.Cells["v_IdAdministracionConceptos"].Text == valorSeleccionado)
                {
                    grdData.ActiveRow = row;
                    grdData.ActiveRow.Selected = true;
                    break;
                }
            }
        }
        /// <summary>
        /// Obtiene un codigo sugerido para el nuevo Concepto
        /// </summary>
        /// <returns>codigo sugerido</returns>
        private string GetSuggestCode()
        {
            var list = GetData("","");
            if (list.Count == 0) return "01";
            var codigo = (from n in list
                       let cod = int.Parse(n.v_Codigo)
                       select cod).Max() + 1;
            return codigo.ToString();
        }
        #endregion

        private void frmAdministracionConcepto_2_Load(object sender, EventArgs e)
        {

        }
    }
}
