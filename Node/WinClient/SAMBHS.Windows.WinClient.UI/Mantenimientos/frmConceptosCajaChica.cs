using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Contabilidad.BL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SAMBHS.Venta.BL;
namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmConceptosCajaChica : Form
    {
        #region Properties
        //private administracionconceptosDto _administracionconceptosDto = new administracionconceptosDto();
        //readonly AdministracionConceptosBL _objAdministracionConceptosBl = new AdministracionConceptosBL();
        //readonly AsientosContablesBL _objAsientosContablesBl = new AsientosContablesBL();

        //private asientocontableDto _cuentaVenta = new asientocontableDto();
        //private asientocontableDto _cuentaIgv = new asientocontableDto();
        //private asientocontableDto _cuentaDetraccion = new asientocontableDto();

        private readonly ConceptosChicaBL _objConceptoBl = new ConceptosChicaBL();
        private conceptoscajachicaDto _conceptoDto;
        private string _mode;
        #endregion

        #region Construct
        public frmConceptosCajaChica(string n)
        {
            InitializeComponent();

            Load += delegate
            {
                BackColor = new GlobalFormColors().FormColor;
                var objOperationResult = new OperationResult();
                btnEliminar.Enabled = false;
                btnEditar.Enabled = false;
                btnCancel.Click += delegate
                {
                    DetalleOff();
                    txtBuscarNombre.Focus();
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
            var obj = grdData.ActiveRow.ListObject as conceptoscajachicaDto;
            if (obj == null) return;
            CargarDetalle(obj.i_IdConceptosCajaChica);
        }
        #endregion

        #region Datos Concepto
        private void btnGrabar_Click(object sender, EventArgs e)
        {

            #region Validacion de Campos
            if (txtNombre.Text.Trim().Equals(""))
            {
                UltraMessageBox.Show("Por favor ingrese un Nombre de Concepto Correcto", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return;
            }
           
            if ( _conceptoDto!=null &&  _conceptoDto.i_IdConceptosCajaChica!=null &&  _conceptoDto.i_IdConceptosCajaChica !=1 && ( txtCuenta.Text.Trim() == "" || !Utils.Windows.EsCuentaImputable(txtCuenta.Text.Trim())))
            {
                UltraMessageBox.Show("Por favor ingrese una Cuenta  Imputable Valida.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtCuenta.Focus();
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
                    _conceptoDto = new conceptoscajachicaDto 
                    {
                        v_NombreConceptoCajaChica = txtNombre.Text.Trim (),
                        v_NroCuenta =txtCuenta.Text.Trim (),
                        i_RequiereTipoDocumento = chkRequiereTipoDocumento.Checked ?1:0,
                        i_RequiereAnexo=chkRequiereAnexo.Checked ?1:0,
                        i_RequiereNumeroDocumento =chkRequiereNumeroDocumento.Checked ?1:0,

                       
                    };

                    _objConceptoBl.InsertarConceptoCajaChica(ref objOperationResult, _conceptoDto, Globals.ClientSession.GetAsList());
                    if (objOperationResult.Success == 0) goto FinUsingTrans;
                }
                else // _mode == "Edit" || "EditNew"
                {
                   
                    _conceptoDto.v_NombreConceptoCajaChica  = txtNombre.Text.Trim();
                    _conceptoDto.v_NroCuenta = txtCuenta.Text .Trim ();
                    _conceptoDto.i_RequiereTipoDocumento = chkRequiereTipoDocumento.Checked ?1:0;
                        _conceptoDto.i_RequiereAnexo=chkRequiereAnexo.Checked ?1:0;
                        _conceptoDto.i_RequiereNumeroDocumento = chkRequiereNumeroDocumento.Checked ? 1 : 0;
                    _objConceptoBl.ActualizarConceptoCajaChica(ref objOperationResult, _conceptoDto, Globals.ClientSession.GetAsList());
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

        
        
        #endregion

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            var filters = new List<string>();
            if (!string.IsNullOrEmpty(txtBuscarNombre.Text)) filters.Add("v_NombreConceptoCajaChica.Contains(\"" + txtBuscarNombre.Text.Trim().ToUpper() + "\")");
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
            txtNombre.Focus();
            txtNombre.Enabled = true;

        }
        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null) return;
            var obj = grdData.ActiveRow.ListObject as conceptoscajachicaDto;
            if(obj == null) return;
            CargarDetalle(obj.i_IdConceptosCajaChica);
            DetalleOn();
            txtNombre.Focus();
        }
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if(grdData.ActiveRow == null) return;
            int  codigo = int.Parse ( grdData.ActiveRow.Cells["i_IdConceptosCajaChica"].Value.ToString());

            if (codigo == 1)
            {
                UltraMessageBox.Show("Este concepto no se puede eliminar","Sistena", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            if (!_objConceptoBl.ExistenciaConceptosDiversasCajaChicaDetalle(codigo))
            {
                string nombre = grdData.ActiveRow.Cells["v_NombreConceptoCajaChica"].Value.ToString();
                var resultado = UltraMessageBox.Show("¿Está seguro de eliminar el registro \"" + nombre + "\"?", "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (resultado != DialogResult.Yes) return;
                var objOperationResult = new OperationResult();
                    
                using (var objTransaccion = TransactionUtils.CreateTransactionScope())
                {
                    _objConceptoBl.EliminarConceptosCajaChica(ref objOperationResult, int.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdConceptosCajaChica"].Value.ToString()), Globals.ClientSession.GetAsList());
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
                UltraMessageBox.Show("No se puede eliminar,este código esta siendo utilizado en  el detalle de caja chica", "ADVERTENCIA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        #endregion

        #region Methods
  
        private void DetalleOff()
        {
            panelDatosConcepto.Visible = false;
           
        }
        private void DetalleOn()
        {
            panelDatosConcepto.Visible = true;

        }
        private void LimpiarDetalle()
        {
            txtNombre.Clear();
            txtCuenta.Clear();
            txtNombreCuenta.Clear();
           
        }
        /// <summary>
        ///  Se asocian Eventos a los TextBox del Form
        /// </summary> 
        private void LoadEventosTextBox()
        {
           // #region Events TextBox Codigo


            //txtCodigo.Leave += delegate
            //{
            //    if (txtCodigo.Text.Trim() == string.Empty) { LimpiarDetalle(); return; }

            //    string codigo = int.Parse(txtCodigo.Text.Trim()).ToString("00");
            //    txtCodigo.Text = codigo;
            //    var objOperatioResult = new OperationResult();
            //    var existeConcepto = _objConceptoBl.ObtenerCodigoConcepto(ref objOperatioResult, codigo.Trim());
            //    if (existeConcepto != null)
            //    {
            //        _conceptoDto = existeConcepto;
            //        txtNombre.Text = _conceptoDto.v_Nombre.Trim();
            //        cboArea.Value = _conceptoDto.i_IdArea.ToString();
            //        txtNombre.Enabled = !EsCodigoRequerido(_conceptoDto.v_Codigo); //Disable
            //    }
            //    else
            //    {
                   
            //        txtNombre.Clear();
            //    }
            //    var administracionConcepto = _objAdministracionConceptosBl.ObtenerAdministracionConceptosxCod(ref objOperatioResult, txtCodigo.Text.Trim());
            //    if (administracionConcepto != null)
            //    {
            //        _administracionconceptosDto = administracionConcepto;
            //        txtCuentaDetraccion.Text = _administracionconceptosDto.v_CuentaDetraccion;
            //        txtCuenta.Text = _administracionconceptosDto.v_CuentaIGV;
            //        txtCuentaVenta.Text = _administracionconceptosDto.v_CuentaPVenta;
            //        //var Cventa=_objAsientosContablesBL.ObtenAsientosPorNro();
            //        _cuentaVenta = _objAsientosContablesBl.ObtenAsientosPorNro(ref objOperatioResult, txtCuentaVenta.Text);
            //        if (_cuentaVenta != null)
            //            txtCventa.Text = _cuentaVenta.v_NombreCuenta;
            //        else
            //            txtCventa.Clear();

            //        _cuentaIgv = _objAsientosContablesBl.ObtenAsientosPorNro(ref objOperatioResult, txtCuenta.Text);
            //        if (_cuentaIgv != null)
            //            txtNombreCuenta.Text = _cuentaIgv.v_NombreCuenta;
            //        else
            //            txtNombreCuenta.Clear();

            //        _cuentaDetraccion = _objAsientosContablesBl.ObtenAsientosPorNro(ref objOperatioResult, txtCuentaDetraccion.Text);
            //        if (_cuentaDetraccion != null)
            //            txtCdetraccion.Text = _cuentaDetraccion.v_NombreCuenta;
            //        else
            //            txtCdetraccion.Clear();

            //        _mode = "Edit";
            //    }
            //    else
            //    {
            //        LimpiarDetalle();
            //        if (existeConcepto != null)
            //        {
            //            _mode = "EditNew";
            //            txtNombre.Text = _conceptoDto.v_NombreConceptoCajaChica.Trim();
            //            txtCuenta.Text = _conceptoDto.v_NroCuenta.Trim();
            //        }
            //        else
            //        {
            //            txtCodigo.Text = codigo;
            //            _mode = "New";
            //        }

            //    }
            //};
            //#endregion
            #region Eventos Textbox Cuenta Venta
            txtCuenta.TextChanged += delegate
            {
                if (txtCuenta.Text == "") txtCuenta.Clear();
            };
            
            #endregion
          
            #region TextBox Busqueda
           
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
            var objData = GetData("i_IdConceptosCajaChica ASC", filter);
            grdData.DataSource = objData;
            grdData.DisplayLayout.Bands[0].Columns["v_NombreConceptoCajaChica"].PerformAutoResize(Infragistics.Win.UltraWinGrid.PerformAutoSizeType.AllRowsInBand, Infragistics.Win.UltraWinGrid.AutoResizeColumnWidthOptions.All);
        }
        private List<conceptoscajachicaDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            var objOperationResult = new OperationResult();
            var objData = _objConceptoBl.ObtenerListadoConceptosCajaChica(ref objOperationResult, pstrSortExpression, pstrFilterExpression);
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
        /// <param name="ICodigoConcepto">Codigo del concepto</param>
        private void CargarDetalle( int  ICodigoConcepto)
        {
            var objOperationResult = new OperationResult();
            _mode = "Edit";
            _conceptoDto = new conceptoscajachicaDto();
            _conceptoDto = _objConceptoBl.ObtenerConceptoCajaChica(ref objOperationResult, ICodigoConcepto);
            if (_conceptoDto != null)
            {
                txtNombre.Enabled = _conceptoDto.i_IdConceptosCajaChica == 1 ? false : true;
                txtNombre.Text = _conceptoDto.v_NombreConceptoCajaChica;
                txtCuenta.Text = _conceptoDto.v_NroCuenta;
                txtNombreCuenta.Text = _conceptoDto.NombreCuenta;
                chkRequiereAnexo.Checked = _conceptoDto.i_RequiereAnexo == 1 ? true : false;
                chkRequiereTipoDocumento.Checked = _conceptoDto.i_RequiereTipoDocumento == 1 ? true : false;
                chkRequiereNumeroDocumento.Checked = _conceptoDto.i_RequiereNumeroDocumento == 1 ? true : false;
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
                if (row.Cells["i_IdConceptosCajaChica"].Text == valorSeleccionado)
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
       
        #endregion

        private void frmConceptosCajaChica_Load(object sender, EventArgs e)
        {

        }

        private void btnBuscarCuenta_Click(object sender, EventArgs e)
        {
            var frm = new frmPlanCuentasConsulta(txtCuenta.Text.Trim ()); // le mando N de que se esta utilizando en la NOTA DE SALIDA
            frm.ShowDialog();

            if (frm._NroSubCuenta == null) return;
            txtCuenta.Text = frm._NroSubCuenta;
            txtNombreCuenta.Text = frm._NombreCuenta;

            
          
        }

        private void chkRequiereTipoDocumento_CheckedChanged(object sender, EventArgs e)
        {
            chkRequiereNumeroDocumento.Checked = chkRequiereTipoDocumento.Checked;
        }
    }
}
