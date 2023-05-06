using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.Security.BL;
using SAMBHS.Contabilidad.BL;
using SAMBHS.CommonWIN.BL;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Common.BL;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmPlanCuentas2 : Form
    {
        #region Fields
        AsientosContablesBL _objBL = new AsientosContablesBL();
        public asientocontableDto _objAsientosDto = new asientocontableDto();
        asientocontableDto _asientosDto = new asientocontableDto();
        destinoDto _objDestinoDto = new destinoDto();
        string _NroCuenta, _NroSubCuenta, _Mode = "", strFilterExpression;
        bool Encontrado = false;
        public string CuentaOrigen = string.Empty;
        List<destinoDto> _TempAgregarDestinoDto = new List<destinoDto>();
        List<destinoDto> _TempEditarDestinoDto = new List<destinoDto>();
        List<destinoDto> _TempEliminarDestinoDto = new List<destinoDto>();
        readonly SecurityBL _obSecurityBL = new SecurityBL();
        bool _btnNuevo, _btnEliminar, _btnEditar, _btnGuardar;
        bool ActiveRowInsert = false;
        string MensajeGuardar = "";
        #endregion

        #region Construct & Load
        public frmPlanCuentas2(string Parametro)
        {
            InitializeComponent();
            editors = new[] { txtSubCtaLv1, txtSubCtaLv2, txtSubCtaLv3, txtSubCtaLv4, txtSubCtaLv5, txtSubCtaLv6, txtSubCtaLv7, txtSubCtaLv8, txtSubCtaLv9 };
            Array.ForEach(editors.Take(8).ToArray(), r =>
            {
                r.KeyPress += txtSubCtaLv_KeyPress;
                r.TextChanged += txtSubCtaLv_TextChanged;
            });
        }

        private void frmPlanCuentas_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            panel1.BackColor = new GlobalFormColors().BannerColor;
            Tabs.BackColor = new GlobalFormColors().BannerColor;
            OperationResult objOperationResult = new OperationResult();
            DatahierarchyBL objDH = new DatahierarchyBL();
            Utils.Windows.LoadDropDownList(ddlMonedaID, "Value1", "Id", objDH.GetDataHierarchyForComboKeyValueDto(ref objOperationResult, 18, null), DropDownListAction.Select);
            ddlMonedaID.SelectedValue = "1";
            grdData.DoubleClick += btnEditarPlan_Click;
            #region ControlAcciones
            var _formActions = _obSecurityBL.GetFormAction(ref objOperationResult, Globals.ClientSession.i_CurrentExecutionNodeId, Globals.ClientSession.i_SystemUserId, "frmPlanCuentas", Globals.ClientSession.i_RoleId);

            _btnNuevo = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmPlanCuentas_New", _formActions);
            _btnEliminar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmPlanCuentas_Delete", _formActions);
            _btnEditar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmPlanCuentas_Edit", _formActions);
            _btnGuardar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmPlanCuentas_Save", _formActions);


            btnNuevoPlan.Enabled = _btnNuevo;
            btnEditarPlan.Enabled = _btnEditar;
            btnEliminarPlan.Enabled = _btnEliminar;
            btnGuardarPlan.Enabled = _btnGuardar;

            btnNuevoPlan.Enabled = false;
            btnEditarPlan.Enabled = false;
            btnEliminarPlan.Enabled = false;
            btnGuardarPlan.Enabled = false;


            #endregion

            ImputableOFF();
            //SubCuentaInputNuevo();
            txtNombreSubCuenta.Enabled = false;
            //chkImputable.Enabled = false;
            ActivarBotones(false, false, false, false, false);
            ShowTabs(false);
            SubCuentaInputNuevo();
            txtNombreSubCuenta.Enabled = false;
            txtNroCuentaMayor.Focus();
        }
        #endregion

        #region Events

        private void txtNroCuentaMayor_KeyPress(object sender, KeyPressEventArgs e)
        {
            Entrada_Numerica(e);
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                int numero;
                if (!string.IsNullOrEmpty(txtNroCuentaMayor.Text))
                {
                    if (IsNumeric(txtNroCuentaMayor.Text) == true)
                    {
                        numero = Convert.ToInt32(txtNroCuentaMayor.Text);
                        txtNroCuentaMayor.Text = string.Format("{0:00}", numero);
                    }
                    else
                    {
                        txtNroCuentaMayor.Text = "";
                    }
                }
                if (txtNroCuentaMayor.TextLength == 2)
                {

                    if (RevisaNroCuentaMayor() == 0)
                    {
                        _NroCuenta = txtNroCuentaMayor.Text;
                        OperationResult objOperationResult = new OperationResult();
                        _asientosDto = _objBL.ObtenAsientosPorNro(ref objOperationResult, _NroCuenta);
                        lblCuentaMayorNombre.Text = _asientosDto.v_NombreCuenta;
                        txtCuentaMayor.Text = _NroCuenta;
                        // ActivarBotones(false, false, false, false, _btnNuevo);
                        ActivarBotones(false, false, false, false, true);
                        BindGrid();
                        grdData.Enabled = true;
                        txtSubCtaLv1.Focus();

                    }
                    else
                    {
                        UltraMessageBox.Show("El Número de cuenta ingresado no existe en los registros", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        ActivarBotones(false, false, false, false, false);
                        SubCuentaInputOFF();
                    }
                }
                else
                {
                    ActivarBotones(false, false, false, false, false);
                    ImputableOFF();
                    SubCuentaInputOFF();
                    lblCuentaMayorNombre.Text = "";
                    txtCuentaMayor.Text = "";
                    BindGrid();
                }
            }
        }

        private void chkImputable_CheckedChanged(object sender, EventArgs e)
        {
            if (chkImputable.Checked)
            {
                if (!_Mode.Equals(""))
                    ImputableON();
            }
            else
            {
                ImputableOFF();
            }
        }
        private void Entrada_Numerica(KeyPressEventArgs e)
        {
            if (e.KeyChar == 8)
            {
                e.Handled = false;
                return;
            }
            bool IsDec = false;
            if (e.KeyChar >= 48 && e.KeyChar <= 57)
                e.Handled = false;
            else if (e.KeyChar == 46)
                e.Handled = (IsDec) ? true : false;
            else
                e.Handled = true;
        }
        private void grdData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            grdData.DisplayLayout.Bands[0].SortedColumns.Add("v_NroCuenta", false, true);
        }

        private void grdData_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            //colorea las celdas que cumplen con la condicion i_Imputable == 0
            if (e.Row.Cells["i_Imputable"].Value.ToString() == "0")
            {
                e.Row.Appearance.BackColor = Color.Gainsboro;
                e.Row.Appearance.BackColor2 = Color.Gainsboro;
                e.Row.Appearance.ForeColor = Color.Black;
            }
        }

        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            if (grdData.ActiveRow.IsActiveRow == true)
            {
                ActiveRowInsert = true;
                string nroCuenta = grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroCuenta"].Value.ToString();
                CuentaOrigen = nroCuenta;
                OperationResult objOperationResult = new OperationResult();
                _asientosDto = _objBL.ObtenAsientosPorNro(ref objOperationResult, nroCuenta);
                if (_asientosDto != null)
                    _Mode = "Edit";
                ProcesaNroCuentaTextBoxs(nroCuenta);
                txtNombreSubCuenta.Text = _asientosDto.v_NombreCuenta;
                chkImputable.Checked = _asientosDto.i_Imputable == 1;
                chkACM.Checked = _asientosDto.i_ACM == 1;
                chkActivarRubro.Checked = _asientosDto.i_ActivarRubro == 1;
                chkAnalisis.Checked = _asientosDto.i_Analisis == 1;
                chkAplicacion.Checked = _asientosDto.i_AplicacionDestino == 1;
                chkCentroCosto.Checked = _asientosDto.i_CentroCosto == 1;
                chkDetalle.Checked = _asientosDto.i_Detalle == 1;
                chkRendicionF.Checked = _asientosDto.i_RendicionFondos == 1;
                txtIdEntidadF.Text = _asientosDto.i_EntFinanciera == null ? "0" : _asientosDto.i_EntFinanciera.Value.ToString("00");
                txtNomEntidadF.Text = _asientosDto.i_EntFinanciera == null ? "" : this.ObtenerNombrePorCodigo(111, txtIdEntidadF.Text);
                txtAreasID.Text = _asientosDto.v_Area;
                txtTipoExistencia.Text = _asientosDto.v_TipoExistencia == null ? "" : _asientosDto.v_TipoExistencia.Trim();
                txtNomExistencia.Text = _asientosDto.v_TipoExistencia == null ? "" : this.ObtenerNombrePorCodigo(147, txtTipoExistencia.Text);
                if (_asientosDto.i_Naturaleza == 1) { rb_Deudora.Checked = true; } else { rb_Acreedora.Checked = true; }
                ddlMonedaID.SelectedValue = _asientosDto.i_IdMoneda.ToString();
                Encontrado = true;
                if (_Mode != "Edit")
                    EdicionOFF();
                bool mostrar = _Mode.Equals("Edit");
                // ActivarBotones(!mostrar, !mostrar, mostrar, mostrar, !mostrar);
                ActivarBotones(mostrar, mostrar, mostrar, mostrar, mostrar);
                LlenarGrillaDestino(nroCuenta);
                ActiveRowInsert = false;
            }
        }

        #region ComportamientoIngresoSubCuenta

        private void Leave_SubCtaL(object sender, EventArgs e)
        {

        }


        private Infragistics.Win.UltraWinEditors.UltraTextEditor[] editors;

        private void txtSubCtaLv_KeyPress(object sender, KeyPressEventArgs e)
        {
            Entrada_Numerica(e);
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                BuscarSubCuenta();
            }
        }

        private int GetIndex(object editor)
        {
            for (var i = 0; i < editors.Length; i++)
            {
                if (editors[i].Equals(editor)) return i;
            }
            return -1;
        }
        private void txtSubCtaLv_TextChanged(object sender, EventArgs e)
        {
            var i = GetIndex(sender);
            if(i == -1) return;
            var edit = editors[i];
            if (edit.TextLength == 1)
            {
                if (!_Mode.Equals("Edit") && !ActiveRowInsert)
                {
                    editors[i + 1].Enabled = true;
                    editors[i + 1].Focus();
                }
            }
            else
            {
                for (var j = i; j < editors.Length; j++)
                {
                    editors[i + 1].Enabled = false;
                    editors[i + 1].Clear();
                }
            }
        }
        #endregion
        #endregion

        #region Methods
        private void BindGrid()
        {
            var objData = GetData(0, null, "v_NroCuenta ASC", strFilterExpression);

            grdData.DataSource = objData;
        }

        private List<asientocontableDto> GetData(int pintPageIndex, int? pintPageSize, string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _objBL.ObtenPlanDeCuentasPaginadoFiltrado(ref objOperationResult, pintPageIndex, pintPageSize, pstrSortExpression, pstrFilterExpression, _NroCuenta);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }
        private int RevisaNroCuentaMayor()
        {
            OperationResult objOperationResult = new OperationResult();
            var _objCheck = _objBL.CheckByNroCuenta(ref objOperationResult, txtNroCuentaMayor.Text.Trim());
            if (_objCheck.Count == 0)
            {
                txtNroCuentaMayor.Focus();
                return 1;
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// Muestra el panel de Datos
        /// </summary>
        /// <param name="state">true = Show, false = Hide</param>
        private void ShowTabs(bool state)
        {
            panel_datos.Visible = state;
        }

        private void ImputableNuevo()
        {
            rb_Acreedora.Enabled = false;
            rb_Deudora.Enabled = false;
            rb_Acreedora.Checked = true;
            chkACM.Enabled = false;
            chkActivarRubro.Enabled = false;
            chkAnalisis.Enabled = false;
            chkAplicacion.Enabled = false;
            chkCentroCosto.Enabled = false;
            chkDetalle.Enabled = false;
            chkPermiteItem.Enabled = false;
            chkRendicionF.Enabled = false;
            chkACM.Checked = false;
            chkActivarRubro.Checked = false;
            chkAnalisis.Checked = false;
            chkAplicacion.Checked = false;
            chkCentroCosto.Checked = false;
            chkDetalle.Checked = false;
            chkPermiteItem.Checked = false;
            chkRendicionF.Checked = false;
            ddlMonedaID.Enabled = false;
            ddlMonedaID.SelectedValue = "1";
            txtAreasID.Enabled = false;
            txtAreasID.Text = "";
            txtIdEntidadF.Enabled = false;
            txtIdEntidadF.Text = "";
            txtNombreSubCuenta.Clear();
            txtNomEntidadF.Clear();
            txtTipoExistencia.Clear();
            txtNomExistencia.Clear();
            ShowTabs(true);
        }

        private void ImputableOFF()
        {
            rb_Acreedora.Enabled = false;
            rb_Deudora.Enabled = false;

            chkACM.Enabled = false;
            chkActivarRubro.Enabled = false;
            chkAnalisis.Enabled = false;
            chkAplicacion.Enabled = false;
            chkCentroCosto.Enabled = false;
            chkDetalle.Enabled = false;
            chkPermiteItem.Enabled = false;
            chkRendicionF.Enabled = false;

            ddlMonedaID.Enabled = false;
            txtAreasID.Enabled = false;
            txtIdEntidadF.Enabled = false;
            grdDataDestinos.Enabled = false;
            btnEliminar.Enabled = false;
            btnAgregar.Enabled = false;
            txtTipoExistencia.Enabled = false;
            txtTipoExistencia.Enabled = false;
            //ShowTabs(false);
        }

        private void EdicionOFF()
        {
            if (_Mode != "Edit")
            {
                Array.ForEach(editors, e => e.Enabled = true);
            }
            //chkImputable.Enabled = false;
            txtNombreSubCuenta.Enabled = false;
            rb_Acreedora.Enabled = false;
            rb_Deudora.Enabled = false;
            chkACM.Enabled = false;
            chkActivarRubro.Enabled = false;
            chkAnalisis.Enabled = false;
            chkAplicacion.Enabled = false;
            chkCentroCosto.Enabled = false;
            chkDetalle.Enabled = false;
            chkPermiteItem.Enabled = false;
            chkRendicionF.Enabled = false;
            ddlMonedaID.Enabled = false;
            txtAreasID.Enabled = false;
            txtIdEntidadF.Enabled = false;
            txtTipoExistencia.Enabled = false;

            grdDataDestinos.Enabled = false;
            btnAgregar.Enabled = false;
            btnEliminar.Enabled = false;
            ShowTabs(false);
        }

        private void EdicionON(int imputable)
        {
            Array.ForEach(editors, e => e.Enabled = false);
            //chkImputable.Enabled = true;
            txtNombreSubCuenta.Enabled = true;
            if (imputable == 1)
            {
                //chkImputable.Enabled = true;
                rb_Acreedora.Enabled = true;
                rb_Deudora.Enabled = true;
                chkACM.Enabled = true;
                chkActivarRubro.Enabled = true;
                chkAnalisis.Enabled = true;
                chkAplicacion.Enabled = true;
                chkCentroCosto.Enabled = true;
                chkDetalle.Enabled = true;
                chkPermiteItem.Enabled = true;
                chkRendicionF.Enabled = true;
                ddlMonedaID.Enabled = true;
                txtAreasID.Enabled = true;
                txtIdEntidadF.Enabled = true;
                txtTipoExistencia.Enabled = true;

                if (CuentaOrigen.Length > 2)
                {
                    int numero = int.Parse(CuentaOrigen.Substring(0, 2));
                    if (numero == 60 || (numero <= 68 && numero >= 62))
                    {
                        grdDataDestinos.Enabled = true;
                        btnAgregar.Enabled = true;
                        btnEliminar.Enabled = true;
                    }
                }
            }
            else
            {
                rb_Acreedora.Enabled = false;
                rb_Deudora.Enabled = false;
                chkACM.Enabled = false;
                chkActivarRubro.Enabled = false;
                chkAnalisis.Enabled = false;
                chkAplicacion.Enabled = false;
                chkCentroCosto.Enabled = false;
                chkDetalle.Enabled = false;
                chkPermiteItem.Enabled = false;
                chkRendicionF.Enabled = false;
                ddlMonedaID.Enabled = false;
                txtAreasID.Enabled = false;
                txtIdEntidadF.Enabled = false;

                grdDataDestinos.Enabled = false;
                btnAgregar.Enabled = false;
                btnEliminar.Enabled = false;

            }
            ShowTabs(true);
        }

        private void ImputableON()
        {
            rb_Acreedora.Enabled = true;
            rb_Deudora.Enabled = true;
            rb_Acreedora.Checked = true;
            chkACM.Enabled = true;
            chkActivarRubro.Enabled = true;
            chkAnalisis.Enabled = true;
            chkAplicacion.Enabled = true;
            chkCentroCosto.Enabled = true;
            chkDetalle.Enabled = true;
            chkPermiteItem.Enabled = true;
            chkRendicionF.Enabled = true;
            ddlMonedaID.Enabled = true;
            txtAreasID.Enabled = true;
            txtIdEntidadF.Enabled = true;

            btnAgregar.Enabled = true;
            btnEliminar.Enabled = true;
            grdDataDestinos.Enabled = true;
            txtTipoExistencia.Enabled = true;
            //ShowTabs(true);
        }

        private void SubCuentaInputNuevo()
        {
            Array.ForEach(editors, e => e.Enabled = false);
            //chkImputable.Enabled = true;
            chkImputable.Checked = false;

            txtNombreSubCuenta.Enabled = true;

        }

        private void SubCuentaInputOFF()
        {
            Array.ForEach(editors, e => e.Enabled = true);
            //chkImputable.Enabled = false;
            txtNombreSubCuenta.Enabled = false;
        }

        private string ObtenNroSubCuenta()
        {
            _NroSubCuenta = txtCuentaMayor.Text + string.Concat(editors.Select(r => r.Text));
            return _NroSubCuenta;
        }

        public void ActivarBotones(bool editar, bool eliminar, bool guardar, bool cancelar, bool nuevo)
        {
            btnEditarPlan.Enabled = editar;
            btnEliminarPlan.Enabled = eliminar;
            btnGuardarPlan.Enabled = guardar;
            btnCancelarPlan.Enabled = cancelar;
            btnNuevoPlan.Enabled = nuevo;
        }

        private int RevisaNroCuenta(string nro)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objCheck = _objBL.CheckByNroCuenta(ref objOperationResult, nro);
            if (_objCheck.Count != 0)
            {
                UltraMessageBox.Show("El Código ingresado ya existe en los registros", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Hand);

                return 0;
            }
            else
            {
                return 1;
            }
        }

        private void ProcesaNroCuentaTextBoxs(string NroCuenta)
        {
            Array.ForEach(editors, e => e.Clear());
            if(NroCuenta.Length < 3) return;
            txtCuentaMayor.Text = NroCuenta.Substring(0, 2);
            for (int i = 2; i < NroCuenta.Length; i++)
            {
                editors[i - 2].Text = NroCuenta.Substring(i, 1);
            }
        }

        private void BuscarSubCuenta()
        {
            Encontrado = false;

            foreach (var row in grdData.Rows)
            {
                if (grdData.ActiveRow != null) grdData.ActiveRow.Selected = false;
                if (row.Cells["v_NroCuenta"].Text == ObtenNroSubCuenta())
                {
                    grdData.ActiveRow = row;
                    grdData.ActiveRow.Selected = true;
                    Encontrado = true;

                    break;
                }
            }

            if (Encontrado == true)
            {
                string nroCuenta = grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroCuenta"].Value.ToString();
                OperationResult objOperationResult = new OperationResult();
                _asientosDto = _objBL.ObtenAsientosPorNro(ref objOperationResult, nroCuenta);
                txtNombreSubCuenta.Text = _asientosDto.v_NombreCuenta;
                chkImputable.Checked = _asientosDto.i_Imputable == 1;
                chkACM.Checked = _asientosDto.i_ACM == 1;
                chkActivarRubro.Checked = _asientosDto.i_ActivarRubro == 1;
                chkAnalisis.Checked = _asientosDto.i_Analisis == 1;
                chkAplicacion.Checked = _asientosDto.i_AplicacionDestino == 1;
                chkCentroCosto.Checked = _asientosDto.i_CentroCosto == 1;
                chkDetalle.Checked = _asientosDto.i_Detalle == 1;
                chkRendicionF.Checked = _asientosDto.i_RendicionFondos == 1;
                txtIdEntidadF.Text = _asientosDto.i_EntFinanciera.ToString();
                txtAreasID.Text = _asientosDto.v_Area;
                if (_asientosDto.i_Naturaleza == 1) { rb_Deudora.Checked = true; } else { rb_Acreedora.Checked = true; }
                ddlMonedaID.SelectedValue = _asientosDto.i_IdMoneda.ToString();
                ActivarBotones(true, true, false, false, true);
                // ActivarBotones(_btnEditar, _btnEliminar, false, false, _btnNuevo);
                ImputableOFF();
                grdData.Focus();
            }
            else
            {
                txtNombreSubCuenta.Text = "";
                //ImputableNuevo();
                //ImputableOFF();
            }

        }

        public bool IsNumeric(object Expression)
        {
            double retNum;
            var isNum = double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        private void MantenerSeleccion(string ValorSeleccionado)
        {
            foreach (var row in grdData.Rows)
            {
                if (row.Cells["v_NroCuenta"].Text == ObtenNroSubCuenta())
                {
                    grdData.ActiveRow = row;
                    grdData.ActiveRow.Selected = true;
                    break;
                }
            }
        }

        #endregion

        private void LlenarGrillaDestino(string numeroCuenta)
        {

            grdDataDestinos.DataSource = _objBL.ObtenerDestinos(numeroCuenta);

            for (int i = 0; i < grdDataDestinos.Rows.Count(); i++)
            {
                grdDataDestinos.Rows[i].Cells["i_RegistroTipo"].Value = "NoTemporal";
                grdDataDestinos.Rows[i].Cells["i_RegistroEstado"].Value = "NoModificado";
            }

        }

        private void txtNroCuentaMayor_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnNuevoPlan_Click(object sender, EventArgs e)
        {
            if (txtCuentaMayor.Text.Trim() != "")
            {
                _Mode = "New";
                ImputableNuevo();
                SubCuentaInputNuevo();
                //chkImputable.Enabled = true;
                txtNombreSubCuenta.Enabled = true;
                //ActivarBotones(false, false, _btnGuardar, true, false);
                ActivarBotones(false, false, true , true, false);
                grdData.Enabled = false;
                txtSubCtaLv1.Enabled = true;
                txtSubCtaLv1.Text = "";
                txtSubCtaLv1.Focus();

                grdDataDestinos.Selected.Rows.AddRange((UltraGridRow[])this.grdDataDestinos.Rows.All);
                grdDataDestinos.DeleteSelectedRows(false);
            }
        }

        private void btnEditarPlan_Click(object sender, EventArgs e)
        {
            EdicionON(chkImputable.Checked ? 1 : 0);
            _Mode = "Edit";
          //  ActivarBotones(false, false, _btnGuardar , true, false);

            ActivarBotones(false, false, true , true, false);
            //txtNombreSubCuenta.Focus();
            grdData.Focus();

            if (grdData.ActiveRow != null)
                grdData.ActiveRowScrollRegion.ScrollRowIntoView(grdData.ActiveRow);

            //grdData.ActiveRowScrollRegion.ScrollRowIntoView(grdData.ActiveRow);
        }

        private void btnEliminarPlan_Click(object sender, EventArgs e)
        {
            var strNroDoc = grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroCuenta"].Value.ToString();
            if (string.IsNullOrEmpty(strNroDoc)) return;
            OperationResult objOperationResult = new OperationResult();
            if (!_objBL.ExistenciaAsientoenTransacciones(strNroDoc,Globals.ClientSession.i_Periodo.ToString ()))
            {
                if (
                    UltraMessageBox.Show("¿Está seguro de Eliminar este pedido de los registros?", "Confirmación",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                _objBL.AsientoBorrar(ref objOperationResult, strNroDoc, Globals.ClientSession.GetAsList());
                _objBL.DestinoBorrar(ref  objOperationResult, strNroDoc, Globals.ClientSession.GetAsList());

                BindGrid();
                Array.ForEach(editors, r => r.Clear());
                txtNombreSubCuenta.Text = "";
                chkImputable.Checked = false;
                ImputableNuevo();
               // ActivarBotones(false, false, false, false, _btnNuevo);
                ActivarBotones(false, false, false, false, true );
                txtSubCtaLv1.Focus();
            }
            else
            {
                UltraMessageBox.Show("Imposible eliminar,La Cuenta esta siendo usada en Diferentes Transacciones", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnDatosGrabar_Click(object sender, EventArgs e)
        { 
            //if (!ComprobarNroSubCuenta(chkImputable.Checked))
            //{
            //    UltraMessageBox.Show("Porfavor ingrese un Nro de SubCuenta Valido", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    return;
            //}
            if (txtNombreSubCuenta.Text == "")
            {
                UltraMessageBox.Show("Porfavor ingrese un nombre para la subcuenta.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            OperationResult objOperationResult = new OperationResult();
            strFilterExpression = null;
            string strNumeroCuenta;
            strNumeroCuenta = ObtenNroSubCuenta();
            if (uvPlanCuentas.Validate(true, false).IsValid)
            {
                if (strNumeroCuenta == "")
                {
                    UltraMessageBox.Show("Por favor ingrese un Código apropiado para el Documento.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if  (chkImputable.Checked && (strNumeroCuenta.StartsWith ("60")|| strNumeroCuenta.StartsWith("33")||strNumeroCuenta.StartsWith("34")||strNumeroCuenta.StartsWith("35") ||strNumeroCuenta.StartsWith("62") ||strNumeroCuenta.StartsWith("63")  ||strNumeroCuenta.StartsWith("64") ||strNumeroCuenta.StartsWith("65") ||strNumeroCuenta.StartsWith("67")))
                {
                    if (txtTipoExistencia.Text == string.Empty)
                    {
                        UltraMessageBox.Show("Por favor ingrese un Código para tipo de existencia", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                if (ValidarNulosVacios() == true)
                {

                    if (_Mode == "New")
                    {
                        if (RevisaNroCuenta(strNumeroCuenta) == 0) return;
                        {
                            _asientosDto = new asientocontableDto();
                            _asientosDto.i_LongitudJerarquica = strNumeroCuenta.Length;
                            _asientosDto.v_NroCuenta = strNumeroCuenta;
                            _asientosDto.v_NombreCuenta = txtNombreSubCuenta.Text;
                            _asientosDto.i_IdMoneda = int.Parse(ddlMonedaID.SelectedValue.ToString());
                            _asientosDto.i_EntFinanciera = txtIdEntidadF.Text != "" ? int.Parse(txtIdEntidadF.Text) : 0;
                            _asientosDto.v_Area = txtAreasID.Text;
                            _asientosDto.i_Naturaleza = rb_Acreedora.Checked ? 2 : 1;
                            _asientosDto.i_ACM = chkACM.Checked ? 1 : 0;
                            _asientosDto.i_Detalle = chkDetalle.Checked ? 1 : 0;
                            _asientosDto.i_ActivarRubro = chkActivarRubro.Checked ? 1 : 0;
                            _asientosDto.i_Analisis = chkAnalisis.Checked ? 1 : 0;
                            _asientosDto.i_AplicacionDestino = chkAplicacion.Checked ? 1 : 0;
                            _asientosDto.i_CentroCosto = chkCentroCosto.Checked ? 1 : 0;
                            _asientosDto.i_Detalle = chkDetalle.Checked ? 1 : 0;
                            _asientosDto.i_Imputable = chkImputable.Checked ? 1 : 0;
                            _asientosDto.i_PermiteItem = chkPermiteItem.Checked ? 1 : 0;
                            _asientosDto.i_RendicionFondos = chkRendicionF.Checked ? 1 : 0;
                            _asientosDto.v_TipoExistencia = txtTipoExistencia.Text.Trim();
                            
                            LlenarTemporalesDestinos();
                            _objBL.AsientoNuevo(ref objOperationResult, _asientosDto, Globals.ClientSession.GetAsList(),_TempAgregarDestinoDto);
                           // _objBL.DestinoNuevo(ref objOperationResult, Globals.ClientSession.GetAsList(), _TempAgregarDestinoDto);

                        }
                    }
                    else if (_Mode == "Edit")
                    {
                       
                        _asientosDto.v_NroCuenta = strNumeroCuenta;
                        _asientosDto.v_NombreCuenta = txtNombreSubCuenta.Text;
                        _asientosDto.i_IdMoneda = int.Parse(ddlMonedaID.SelectedValue.ToString());
                        _asientosDto.i_EntFinanciera = txtIdEntidadF.Text != "" ? int.Parse(txtIdEntidadF.Text) : 0;
                        _asientosDto.i_Naturaleza = rb_Acreedora.Checked == true ? 2 : 1;
                        _asientosDto.v_Area = txtAreasID.Text;
                        _asientosDto.i_ACM = chkACM.Checked == true ? 1 : 0;
                        _asientosDto.i_Detalle = chkDetalle.Checked == true ? 1 : 0;
                        _asientosDto.i_ActivarRubro = chkActivarRubro.Checked == true ? 1 : 0;
                        _asientosDto.i_Analisis = chkAnalisis.Checked == true ? 1 : 0;
                        _asientosDto.i_AplicacionDestino = chkAplicacion.Checked == true ? 1 : 0;
                        _asientosDto.i_CentroCosto = chkCentroCosto.Checked == true ? 1 : 0;
                        _asientosDto.i_Detalle = chkDetalle.Checked == true ? 1 : 0;
                        _asientosDto.i_Imputable = chkImputable.Checked == true ? 1 : 0;
                        _asientosDto.i_PermiteItem = chkPermiteItem.Checked == true ? 1 : 0;
                        _asientosDto.i_RendicionFondos = chkRendicionF.Checked == true ? 1 : 0;

                        _asientosDto.v_TipoExistencia = txtTipoExistencia.Text.Trim();
                        // Save the data
                        LlenarTemporalesDestinos();
                        _objBL.AsientoActualizar(ref objOperationResult, _asientosDto, Globals.ClientSession.GetAsList(),_TempAgregarDestinoDto ,_TempEditarDestinoDto ,_TempEliminarDestinoDto);

                        //if (objOperationResult.Success == 1)
                        //{
                        //    _objBL.DestinoActualizar(ref objOperationResult, Globals.ClientSession.GetAsList(), _TempAgregarDestinoDto, _TempEditarDestinoDto, _TempEliminarDestinoDto);
                        //}

                    }

                    if (objOperationResult.Success == 1)  // Operación sin error
                    {
                        //this.DialogResult = System.Windows.Forms.DialogResult.OK;
                       // ActivarBotones(_btnEditar , _btnEliminar, false, false, _btnNuevo);
                        _Mode = "Edit";
                        ActivarBotones(false, false, true, true, true );
                        grdData.Enabled = true;
                        ImputableOFF();
                        SubCuentaInputOFF();
                        UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        UltraMessageBox.Show(MensajeGuardar, "ERROR!!!-", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    BindGrid();
                    EdicionOFF();
                    MantenerSeleccion(_asientosDto.v_NroCuenta);
                    _TempAgregarDestinoDto = new List<destinoDto>();
                    _TempEditarDestinoDto = new List<destinoDto>();
                    _TempEliminarDestinoDto = new List<destinoDto>();


                }
            }
            else
            {
                UltraMessageBox.Show("Por favor corrija la información ingresada. Vea los indicadores de error.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            //BindGrid();
            //EdicionOFF();
            //MantenerSeleccion(_asientosDto.v_NroCuenta);


        }
   
        private void LlenarTemporalesDestinos()
        {

            if (grdDataDestinos.Rows.Count() != 0)
            {
                foreach (var Fila in grdDataDestinos.Rows)
                {
                    switch (Fila.Cells["i_RegistroTipo"].Value.ToString())
                    {
                        case "Temporal":
                            if (Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {

                                _objDestinoDto = new destinoDto();
                                _objDestinoDto.v_IdDestino = _objDestinoDto.v_IdDestino;
                                _objDestinoDto.v_CuentaDestino = Fila.Cells["v_CuentaDestino"].Value == null ? null : Fila.Cells["v_CuentaDestino"].Value.ToString();
                                _objDestinoDto.v_CuentaTransferencia = Fila.Cells["v_CuentaTransferencia"].Value == null ? null : Fila.Cells["v_CuentaTransferencia"].Value.ToString();
                                _objDestinoDto.i_Porcentaje = Fila.Cells["i_Porcentaje"].Value == null ? 0 : int.Parse(Fila.Cells["i_Porcentaje"].Value.ToString());
                                _objDestinoDto.v_Periodo = Globals.ClientSession.i_Periodo.ToString();
                                _TempAgregarDestinoDto.Add(_objDestinoDto);

                            }
                            break;

                        case "NoTemporal":
                            if (Fila.Cells["i_RegistroEstado"].Value != null && Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {
                                _objDestinoDto = new destinoDto();
                                _objDestinoDto.v_IdDestino = Fila.Cells["v_IdDestino"].Value == null ? null : Fila.Cells["v_IdDestino"].Value.ToString();
                                _objDestinoDto.v_CuentaDestino = Fila.Cells["v_CuentaDestino"].Value == null ? null : Fila.Cells["v_CuentaDestino"].Value.ToString();
                                _objDestinoDto.v_CuentaTransferencia = Fila.Cells["v_CuentaTransferencia"].Value == null ? null : Fila.Cells["v_CuentaTransferencia"].Value.ToString();
                                _objDestinoDto.i_Porcentaje = Fila.Cells["i_Porcentaje"].Value == null ? 0 : int.Parse(Fila.Cells["i_Porcentaje"].Value.ToString());
                                _objDestinoDto.i_Eliminado = int.Parse(Fila.Cells["i_Eliminado"].Value.ToString());
                                _objDestinoDto.i_InsertaIdUsuario = int.Parse(Fila.Cells["i_InsertaIdUsuario"].Value.ToString());
                                _objDestinoDto.t_InsertaFecha = Convert.ToDateTime(Fila.Cells["t_InsertaFecha"].Value);
                                _TempEditarDestinoDto.Add(_objDestinoDto);
                            }
                            break;
                    }
                }
            }



        }

        private void btnCancelarPlan_Click(object sender, EventArgs e)
        {
            ImputableOFF();
            SubCuentaInputOFF();
           // ActivarBotones(false, false, false, false, _btnNuevo);
            ActivarBotones(false, false, false, false,true);
            grdData.Enabled = true;
            ShowTabs(false);
            _Mode = "";
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {


            UltraGridRow row = grdDataDestinos.DisplayLayout.Bands[0].AddNew();
            grdDataDestinos.Rows.Move(row, grdDataDestinos.Rows.Count() - 1);
            this.grdDataDestinos.ActiveRowScrollRegion.ScrollRowIntoView(row);
            row.Cells["i_RegistroEstado"].Value = "Agregado";
            row.Cells["i_RegistroTipo"].Value = "Temporal";
            row.Cells["i_Porcentaje"].Value = 0;
            row.Cells["v_CuentaDestino"].Value = string.Empty;
            row.Cells["v_CuentaTransferencia"].Value = string.Empty; 
            grdDataDestinos.Focus();
            UltraGridCell aCell = this.grdDataDestinos.ActiveRow.Cells["v_CuentaDestino"];
            this.grdDataDestinos.ActiveCell = aCell;
            grdDataDestinos.ActiveRow = aCell.Row;
            grdDataDestinos.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
            grdDataDestinos.PerformAction(UltraGridAction.EnterEditMode, false, false);


           
        }

        private void grdDataDestinos_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {

            switch (e.Cell.Column.Key)
            {
                case "v_CuentaDestino":
                    var asiento = _objBL.ObtenerAsientoImputable(e.Cell.Text);
                    if (asiento != null)
                    {
                        if (CuentaOrigen.Length >= 2)
                        {

                            foreach (UltraGridRow Fila in grdDataDestinos.Rows)
                            {
                                if (Fila.Cells["v_CuentaTransferencia"].Value != null && Fila.Cells["v_CuentaDestino"].Value != null && Fila.Index != grdDataDestinos.ActiveRow.Index)
                                {
                                    if (grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaTransferencia"].Value != null)
                                    {
                                        if (asiento.v_NroCuenta.Trim() == Fila.Cells["v_CuentaDestino"].Value.ToString().Trim() && grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaTransferencia"].Value.ToString().Trim() == Fila.Cells["v_CuentaTransferencia"].Value.ToString().Trim())
                                        {
                                            UltraMessageBox.Show("Está Cuenta Destino ya fue asignada ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaDestino"].Value = string.Empty;
                                            grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaDestino"].Value = string.Empty;
                                            return;
                                        }
                                    }
                                }
                            }


                            if ((CuentaOrigen.StartsWith("60") && (int.Parse(asiento.v_NroCuenta.Substring(0, 2)) >= 20 && int.Parse(asiento.v_NroCuenta.Substring(0, 2)) <= 28)) || ((decimal.Parse(CuentaOrigen.Substring(0, 2)) >= 62 && decimal.Parse(CuentaOrigen.Substring(0, 2)) <= 68) && (asiento.v_NroCuenta.ToString().StartsWith("90") ||   asiento.v_NroCuenta.ToString().StartsWith("91") ||   asiento.v_NroCuenta.ToString().StartsWith("92") || asiento.v_NroCuenta.ToString().StartsWith("94") || asiento.v_NroCuenta.ToString().StartsWith("95")  || asiento.v_NroCuenta.ToString().StartsWith("96") || asiento.v_NroCuenta.ToString().StartsWith("97"))))
                            {
                                grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaDestino"].Value = asiento.v_NombreCuenta;
                            }
                            else
                            {
                                if (CuentaOrigen.StartsWith("60"))
                                {
                                    grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaDestino"].Value = string.Empty;
                                    grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaDestino"].Value = string.Empty;
                                    UltraMessageBox.Show("Cuenta Destino debe pertenecer a la Cuenta 2", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                                else
                                {
                                    if (int.Parse(CuentaOrigen.Substring(0, 2)) >= 62 && int.Parse(CuentaOrigen.Substring(0, 2)) <= 68)
                                    {
                                        grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaDestino"].Value = string.Empty;
                                        grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaDestino"].Value = string.Empty;
                                        UltraMessageBox.Show("Cuenta Destino debe pertenecer a la Cuenta 90-91-92-94-95-96 o 97", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return;

                                    }
                                }
                                grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaDestino"].Value = string.Empty;
                            }

                        }

                    }

                    else
                    {

                        if (grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaDestino"].Text.Trim() == string.Empty)
                        {
                            string cuentaDestino = CuentaOrigen.StartsWith("60") ? "20" : "94";
                            frmPlanCuentasConsulta frmPcc = new frmPlanCuentasConsulta(cuentaDestino);
                            frmPcc.ShowDialog();
                            if (CuentaOrigen.Length >= 2 && frmPcc._NroSubCuenta != null)
                            {

                                foreach (UltraGridRow Fila in grdDataDestinos.Rows)
                                {
                                    if (Fila.Cells["v_CuentaTransferencia"].Value != null && Fila.Cells["v_CuentaDestino"].Value != null)
                                    {
                                        if (grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaTransferencia"].Value != null)
                                        {
                                            if (frmPcc._NroSubCuenta.Trim() == Fila.Cells["v_CuentaDestino"].Value.ToString().Trim() && grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaTransferencia"].Value.ToString().Trim() == Fila.Cells["v_CuentaTransferencia"].Value.ToString().Trim())
                                            {
                                                grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaDestino"].Value = string.Empty;
                                                grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaDestino"].Value = string.Empty;
                                                UltraMessageBox.Show("Está Cuenta Destino ya fue asignada ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                return;
                                            }
                                        }
                                    }
                                }


                                if ((CuentaOrigen.StartsWith("60") && (int.Parse(frmPcc._NroSubCuenta.ToString().Substring(0, 2)) >= 20 && int.Parse(frmPcc._NroSubCuenta.ToString().Substring(0, 2)) <= 28)) || ((decimal.Parse(CuentaOrigen.Substring(0, 2)) >= 62 && decimal.Parse(CuentaOrigen.Substring(0, 2)) <= 68) && (frmPcc._NroSubCuenta.ToString().StartsWith("90") || frmPcc._NroSubCuenta.ToString().StartsWith("91") || frmPcc._NroSubCuenta.ToString().StartsWith("92") || frmPcc._NroSubCuenta.ToString().StartsWith("94") || frmPcc._NroSubCuenta.ToString().StartsWith("95") || frmPcc._NroSubCuenta.ToString().StartsWith("97"))))
                                {

                                    grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaDestino"].Value = frmPcc._NombreCuenta.ToString();
                                    grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaDestino"].Value = frmPcc._NroSubCuenta;
                                }
                                else
                                {

                                    if (CuentaOrigen.StartsWith("60"))
                                    {
                                        grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaDestino"].Value = string.Empty;
                                        grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaDestino"].Value = string.Empty;
                                        UltraMessageBox.Show("Cuenta Destino  pertenece a la Cuenta 2", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return;
                                    }
                                    else
                                    {
                                        if (int.Parse(CuentaOrigen.Substring(0, 2)) >= 62 && int.Parse(CuentaOrigen.Substring(0, 2)) <= 68)
                                        {
                                            grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaDestino"].Value = string.Empty;
                                            grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaDestino"].Value = string.Empty;
                                            UltraMessageBox.Show("Cuenta Destino pertenece a la Cuenta 90-92-94-95 o 97 ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return;

                                        }
                                    }
                                    grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaDestino"].Value = string.Empty;
                                    grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaDestino"].Value = string.Empty;
                                }
                            }
                        }
                        else
                        {

                            if (CuentaOrigen.StartsWith("60"))
                            {
                                grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaDestino"].Value = string.Empty;
                                grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaDestino"].Value = string.Empty;
                                UltraMessageBox.Show("Cuenta Destino  pertenece a la Cuenta 2", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;

                            }
                            else
                            {
                                grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaDestino"].Value = string.Empty;
                                grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaDestino"].Value = string.Empty;
                                UltraMessageBox.Show("Cuenta Destino  pertenece a la Cuenta 90-92-94-95 o 97", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }


                        }
                    }
                    grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                    break;

                case "v_CuentaTransferencia":

                    var asientoTransferencia = _objBL.ObtenerAsientoImputable(e.Cell.Text);
                    if (asientoTransferencia != null)
                    {

                        if (CuentaOrigen.Length >= 2)
                        {
                            foreach (UltraGridRow Fila in grdDataDestinos.Rows)
                            {
                                if (Fila.Cells["v_CuentaTransferencia"].Value != null && Fila.Cells["v_CuentaDestino"].Value != null && grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaDestino"].Value != null && Fila.Index != grdDataDestinos.ActiveRow.Index)
                                {

                                    if (asientoTransferencia.v_NroCuenta.Trim() == Fila.Cells["v_CuentaTransferencia"].Value.ToString() && grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaDestino"].Value.ToString() == Fila.Cells["v_CuentaDestino"].Value.ToString())
                                    {
                                        UltraMessageBox.Show("Está Cuenta Transferencia ya fue asignada ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaTransferencia"].Value = string.Empty;
                                        grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaTransferencia"].Value = string.Empty;
                                        return;
                                    }
                                }
                            }

                            if (CuentaOrigen.StartsWith("60") && asientoTransferencia.v_NroCuenta.StartsWith("61") || (int.Parse(CuentaOrigen.Substring(0, 2)) >= 62 && int.Parse(CuentaOrigen.Substring(0, 2)) <= 68 && asientoTransferencia.v_NroCuenta.StartsWith("79")))
                            {
                                grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaTransferencia"].Value = asientoTransferencia.v_NombreCuenta;
                            }
                            else
                            {
                                if (CuentaOrigen.StartsWith("60"))
                                {
                                    grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaTransferencia"].Value = string.Empty;
                                    grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaTransferencia"].Value = string.Empty;
                                    UltraMessageBox.Show("Cuenta Transferencia  pertenece a la Cuenta 61", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                                else
                                {
                                    if (int.Parse(CuentaOrigen.Substring(0, 2)) >= 62 && int.Parse(CuentaOrigen.Substring(0, 2)) <= 68)
                                    {

                                        grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaTransferencia"].Value = string.Empty;
                                        grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaTransferencia"].Value = string.Empty;
                                        UltraMessageBox.Show("Cuenta Transferencia  pertenece la Cuenta 79", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return;

                                    }
                                }
                                grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaTransferencia"].Value = string.Empty;
                            }
                        }

                    }

                    else
                    {

                        if (grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaTransferencia"].Text.Trim() == string.Empty)
                        {

                            string cuentaTransferencia = CuentaOrigen.StartsWith("60") ? "61" : "79";
                            frmPlanCuentasConsulta frmPc = new frmPlanCuentasConsulta(cuentaTransferencia);
                            frmPc.ShowDialog();
                            if (CuentaOrigen.Length >= 2 && frmPc._NroSubCuenta != null)
                            {
                                foreach (UltraGridRow Fila in grdDataDestinos.Rows)
                                {
                                    if (Fila.Cells["v_CuentaTransferencia"].Value != null && Fila.Cells["v_CuentaDestino"].Value != null && grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaDestino"].Value != null)
                                    {

                                        if (frmPc._NroSubCuenta == Fila.Cells["v_CuentaTransferencia"].Value.ToString() && grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaDestino"].Value.ToString() == Fila.Cells["v_CuentaDestino"].Value.ToString())
                                        {

                                            //grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaTransferencia"].Value = string.Empty;
                                            //grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaTransferencia"].Value = string.Empty;
                                            UltraMessageBox.Show("Está Cuenta Transferencia ya fue asignada ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            return;
                                        }
                                    }
                                }


                                if (CuentaOrigen.StartsWith("60") && frmPc._NroSubCuenta.StartsWith("61") || (int.Parse(CuentaOrigen.Substring(0, 2)) >= 62 && int.Parse(CuentaOrigen.Substring(0, 2)) <= 68 && frmPc._NroSubCuenta.StartsWith("79")))
                                {
                                    grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaTransferencia"].Value = frmPc._NombreCuenta.ToString();
                                    grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaTransferencia"].Value = frmPc._NroSubCuenta;
                                }
                                else
                                {
                                    if (CuentaOrigen.StartsWith("60"))
                                    {

                                        grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaTransferencia"].Value = string.Empty;
                                        grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaTransferencia"].Value = string.Empty;
                                        UltraMessageBox.Show("Cuenta Transferencia   pertenece a la Cuenta 61", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return;
                                    }
                                    else
                                    {
                                        if (int.Parse(CuentaOrigen.Substring(0, 2)) >= 62 && int.Parse(CuentaOrigen.Substring(0, 2)) <= 68)
                                        {
                                            grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaTransferencia"].Value = string.Empty;
                                            grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaTransferencia"].Value = string.Empty;
                                            UltraMessageBox.Show("Cuenta Transferencia pertenece a la Cuenta 79", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return;

                                        }
                                    }
                                    grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaTransferencia"].Value = string.Empty;
                                    grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaTransferencia"].Value = string.Empty;

                                }
                            }
                        }
                        else
                        {

                            if (CuentaOrigen.StartsWith("60"))
                            {
                                grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaTransferencia"].Value = string.Empty;
                                grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaTransferencia"].Value = string.Empty;
                                UltraMessageBox.Show("Cuenta Transferencia pertenece a la Cuenta 61", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            else
                            {
                                grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaTransferencia"].Value = string.Empty;
                                grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaTransferencia"].Value = string.Empty;
                                UltraMessageBox.Show("Cuenta Transferencia pertenece a la Cuenta 79", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }


                        }

                    }
                    grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                    break;

            }
        }
  
        private void grdDataDestinos_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (grdDataDestinos.ActiveCell != null)
            {
                UltraGridCell Celda;
                switch (this.grdDataDestinos.ActiveCell.Column.Key)
                {
                    case "i_Porcentaje":

                        Celda = grdDataDestinos.ActiveCell;
                        Utils.Windows.NumeroEnteroCelda(Celda, e);
                        break;

                    case "v_CuentaDestino":
                        Celda = grdData.ActiveCell;
                        Utils.Windows.NumeroEnteroCelda(Celda, e);
                        break;

                    case "v_CuentaTransferencia":
                        Celda = grdData.ActiveCell;
                        Utils.Windows.NumeroEnteroCelda(Celda, e);
                        break;



                }
            }
        }

        private void grdDataDestinos_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            e.Row.Cells["Index"].Value = (e.Row.Index + 1).ToString("00");
        }

        private void grdDataDestinos_CellChange(object sender, CellEventArgs e)
        {
            grdDataDestinos.Rows[e.Cell.Row.Index].Cells["i_RegistroEstado"].Value = "Modificado";
        }

        private void grdData_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnEditarPlan_Click(sender, e);
                if (panel_datos.Visible) txtNombreSubCuenta.Focus();
            }
        }

        private void btnEliminar_Click_1(object sender, EventArgs e)
        {
            if (grdDataDestinos.ActiveRow == null) return;

            if (grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["i_RegistroTipo"].Value.ToString() == "NoTemporal")
            {
                if (UltraMessageBox.Show("¿Seguro de Eliminar este Registro?", "Sistemas", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _objDestinoDto = new destinoDto();
                    _objDestinoDto.v_IdDestino = grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_IdDestino"].Value.ToString();
                    _TempEliminarDestinoDto.Add(_objDestinoDto);
                    grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Delete(false);

                }
            }
            else
            {
                if (UltraMessageBox.Show("¿Seguro de Eliminar este Registro?", "Sistemas", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Delete(false);

                }
            }
        }

        private void grdDataDestinos_KeyDown(object sender, KeyEventArgs e)
        {
            if (grdDataDestinos.ActiveCell != null)
            {
                //UltraGridCell Celda;
                switch (this.grdDataDestinos.ActiveCell.Column.Key)
                {
                    case "v_CuentaDestino":
                        if (e.KeyCode == Keys.Back)
                        {
                            grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaDestino"].Value = string.Empty;

                        }

                        break;

                    case "v_CuentaTransferencia":
                        if (e.KeyCode == Keys.Back)
                        {
                            grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaTransferencia"].Value = string.Empty;

                        }

                        break;
                }


                switch (e.KeyCode)
                {
                    case Keys.Up:
                        grdDataDestinos.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        grdDataDestinos.PerformAction(UltraGridAction.AboveCell, false, false);
                        e.Handled = true;
                        grdDataDestinos.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                    case Keys.Down:
                        grdDataDestinos.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        grdDataDestinos.PerformAction(UltraGridAction.BelowCell, false, false);
                        e.Handled = true;
                        grdDataDestinos.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                    case Keys.Right:
                        grdDataDestinos.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        grdDataDestinos.PerformAction(UltraGridAction.NextCellByTab, false, false);
                        e.Handled = true;
                        grdDataDestinos.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                    case Keys.Left:
                        grdDataDestinos.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        grdDataDestinos.PerformAction(UltraGridAction.PrevCellByTab, false, false);
                        e.Handled = true;
                        grdDataDestinos.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                    case Keys.Enter:
                        DoubleClickCellEventArgs eventos = new DoubleClickCellEventArgs(grdDataDestinos.ActiveCell);
                        grdDataDestinos_DoubleClickCell(sender, eventos);
                        e.Handled = true;
                        break;

                }
            }

        }

        private bool ValidarNulosVacios()
        {


            if (grdDataDestinos.Rows.Any(p => p.Cells["v_CuentaDestino"].Value==null || p.Cells["v_CuentaDestino"].Value.ToString().Trim() == "" || p.Cells["v_CuentaDestino"].Value.ToString ().Trim() ==string.Empty))
            {
                UltraMessageBox.Show("Por favor ingrese correctamente todas las Cuentas Destino", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdDataDestinos.Rows.FirstOrDefault(x => x.Cells["v_CuentaDestino"].Value == null || x.Cells["v_CuentaDestino"].Value.ToString().Trim() == string.Empty ||x.Cells["v_CuentaDestino"].Value.ToString().Trim() =="");
                grdDataDestinos.Selected.Cells.Add(Row.Cells["v_CuentaDestino"]);
                grdDataDestinos.Focus();
                Row.Activate();
                grdDataDestinos.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = grdDataDestinos.ActiveRow.Cells["v_CuentaDestino"];
                grdDataDestinos.ActiveCell = aCell;
                return false;
            }

            //if (grdDataDestinos.Rows.Where(p => p.Cells["v_CuentaTransferencia"].Value.ToString().Trim() == string.Empty).Count() != 0)

            if (grdDataDestinos.Rows.Any(p => p.Cells["v_CuentaTransferencia"].Value==null  || p.Cells["v_CuentaTransferencia"].Value.ToString().Trim() == "" || p.Cells["v_CuentaTransferencia"].Value.ToString ().Trim ()==string.Empty ))
            {
                UltraMessageBox.Show("Por favor ingrese correctamente todas las Cuentas Transferencia", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdDataDestinos.Rows.FirstOrDefault(x => x.Cells["v_CuentaTransferencia"].Value == null || x.Cells["v_CuentaTransferencia"].Value.ToString().Trim() == string.Empty || x.Cells["v_CuentaTransferencia"].Value.ToString().Trim()=="");
                grdDataDestinos.Selected.Cells.Add(Row.Cells["v_CuentaTransferencia"]);
                grdDataDestinos.Focus();
                Row.Activate();
                grdDataDestinos.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdDataDestinos.ActiveRow.Cells["v_CuentaTransferencia"];
                this.grdDataDestinos.ActiveCell = aCell;
                return false;
            }
            if (grdDataDestinos.Rows.Any(p => p.Cells["i_Porcentaje"].Value == null || int.Parse(p.Cells["i_Porcentaje"].Value.ToString().Trim()) <= 0 || int.Parse(p.Cells["i_Porcentaje"].Value.ToString().Trim()) > 100))
            {
                UltraMessageBox.Show("Porcentaje no puede ser mayor de 100 % , ni igual a 0 % ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdDataDestinos.Rows.FirstOrDefault(x => x.Cells["i_Porcentaje"].Value == null || int.Parse(x.Cells["i_Porcentaje"].Value.ToString().Trim()) <= 0 || int.Parse(x.Cells["i_Porcentaje"].Value.ToString().Trim()) > 100);
                grdDataDestinos.Selected.Cells.Add(Row.Cells["i_Porcentaje"]);
                grdDataDestinos.Focus();
                Row.Activate();
                grdDataDestinos.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdDataDestinos.ActiveRow.Cells["i_Porcentaje"];
                this.grdDataDestinos.ActiveCell = aCell;
                return false;
            }

            

            foreach (var Fila in grdDataDestinos.Rows)
            { 

                if (! _objBL.EsImputable(Fila.Cells ["v_CuentaDestino"].Value.ToString ()))
                {
                  UltraMessageBox.Show("La cuenta Destino debe ser Válida  ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                  return false;
                 
                
                }
                if (!_objBL.EsImputable(Fila.Cells["v_CuentaTransferencia"].Value.ToString()))
                {
                    UltraMessageBox.Show("La cuenta Transferencia  debe ser Válida", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;

                }
            
            
            }

            if (grdDataDestinos.Rows.Any())
            {
                int totalPorcentajes = grdDataDestinos.Rows.Where(x => x.Cells["i_Porcentaje"].Value != null).Sum(p => int.Parse(p.Cells["i_Porcentaje"].Value.ToString()));
                if (totalPorcentajes > 100)
                {
                    UltraMessageBox.Show("Sumatoria de la Cuenta Origen " + CuentaOrigen + "debe ser 100 % \n Actualmente registra " + totalPorcentajes.ToString() + "%", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (totalPorcentajes < 100)
                {
                    UltraMessageBox.Show("Sumatoria de la Cuenta Origen " + CuentaOrigen + "debe ser 100 % \n Actualmente registra " + totalPorcentajes.ToString() + "%", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;

                }
            }
            return true;


        }

        private void grdDataDestinos_AfterExitEditMode(object sender, EventArgs e)
        {

            if (grdDataDestinos.ActiveCell.Column.Key == null) return;


            switch (grdDataDestinos.ActiveCell.Column.Key)
            {

                case "i_Porcentaje":


                    if (grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["i_Porcentaje"].Value == null) grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["i_Porcentaje"].Value = 0;

                    if (int.Parse(grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["i_Porcentaje"].Value.ToString()) == 0 || int.Parse(grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["i_Porcentaje"].Value.ToString()) > 100)
                    {
       
                        UltraMessageBox.Show("Porcentaje no puede ser mayor de 100 %,  ni igual a 0 %", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        UltraGridRow Row = grdDataDestinos.Rows.FirstOrDefault(p => int.Parse(p.Cells["i_Porcentaje"].Value.ToString().Trim()) == 0 || int.Parse(p.Cells["i_Porcentaje"].Value.ToString().Trim()) > 100);
                        grdDataDestinos.Selected.Cells.Add(Row.Cells["i_Porcentaje"]);
                        Row.Activate();
                        grdDataDestinos.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                        UltraGridCell aCell = this.grdDataDestinos.ActiveRow.Cells["i_Porcentaje"];
                        this.grdDataDestinos.ActiveCell = aCell;

                    }
                    break;

                case "v_CuentaDestino":
                         object sender1 = new object ();
                         DoubleClickCellEventArgs eventos = new DoubleClickCellEventArgs(grdDataDestinos.ActiveCell);
                         grdDataDestinos_DoubleClickCell(sender1, eventos);
                         break;

                case "v_CuentaTransferencia":
                     object sender2 = new object ();
                         DoubleClickCellEventArgs eventos2 = new DoubleClickCellEventArgs(grdDataDestinos.ActiveCell);
                         grdDataDestinos_DoubleClickCell(sender2, eventos2);
                       
                    break;

            }
        }

        private void txtIdEntidadF_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscarEntidadFinanciera")
            {
                var frm2 = new frmBuscarDatahierarchy(111, "Entidad Financiera");
                frm2.ShowDialog();
                if (frm2._itemId != null)
                {
                    txtIdEntidadF.Text = frm2._value2;
                    txtNomEntidadF.Text = frm2._value1;
                }
                else
                {
                   // txtIdEntidadF.Clear(); 
                }

            }
        }

        private void txtIdEntidadF_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtIdEntidadF, e); 
        }

        private void grdDataDestinos_ClickCellButton(object sender, CellEventArgs e)
        {
            switch (e.Cell.Column.Key)
            {
                case "v_CuentaDestino":
                    var asiento = _objBL.ObtenerAsientoImputable(e.Cell.Text);
                    if (asiento != null)
                    {
                        if (CuentaOrigen.Length >= 2)
                        {

                            foreach (UltraGridRow Fila in grdDataDestinos.Rows)
                            {
                                if (Fila.Cells["v_CuentaTransferencia"].Value != null && Fila.Cells["v_CuentaDestino"].Value != null && Fila.Index != grdDataDestinos.ActiveRow.Index)
                                {
                                    if (grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaTransferencia"].Value != null)
                                    {
                                        if (asiento.v_NroCuenta.Trim() == Fila.Cells["v_CuentaDestino"].Value.ToString().Trim() && grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaTransferencia"].Value.ToString().Trim() == Fila.Cells["v_CuentaTransferencia"].Value.ToString().Trim())
                                        {
                                            UltraMessageBox.Show("Está Cuenta Destino ya fue asignada ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaDestino"].Value = string.Empty;
                                            grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaDestino"].Value = string.Empty;
                                            return;
                                        }
                                    }
                                }
                            }


                            if ((CuentaOrigen.StartsWith("60") && (int.Parse(asiento.v_NroCuenta.Substring(0, 2)) >= 20 && int.Parse(asiento.v_NroCuenta.Substring(0, 2)) <= 28)) || ((decimal.Parse(CuentaOrigen.Substring(0, 2)) >= 62 && decimal.Parse(CuentaOrigen.Substring(0, 2)) <= 68) && (asiento.v_NroCuenta.ToString().StartsWith("90") || asiento.v_NroCuenta.ToString().StartsWith("94") || asiento.v_NroCuenta.ToString().StartsWith("95") || asiento.v_NroCuenta.ToString().StartsWith("97"))))
                            {
                                grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaDestino"].Value = asiento.v_NombreCuenta;
                            }
                            else
                            {
                                if (CuentaOrigen.StartsWith("60"))
                                {
                                    grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaDestino"].Value = string.Empty;
                                    grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaDestino"].Value = string.Empty;
                                    UltraMessageBox.Show("Cuenta Destino debe pertenecer a la Cuenta 2", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                                else
                                {
                                    if (int.Parse(CuentaOrigen.Substring(0, 2)) >= 62 && int.Parse(CuentaOrigen.Substring(0, 2)) <= 68)
                                    {
                                        grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaDestino"].Value = string.Empty;
                                        grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaDestino"].Value = string.Empty;
                                        UltraMessageBox.Show("Cuenta Destino debe pertenecer a la Cuenta 90-92-94-95 o 97", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return;

                                    }
                                }
                                grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaDestino"].Value = string.Empty;
                            }

                        }

                    }

                    else
                    {

                        if (grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaDestino"].Text.Trim() == string.Empty)
                        {
                            string cuentaDestino = CuentaOrigen.StartsWith("60") ? "20" : "94";
                            frmPlanCuentasConsulta frmPcc = new frmPlanCuentasConsulta(cuentaDestino);
                            frmPcc.ShowDialog();
                            if (CuentaOrigen.Length >= 2 && frmPcc._NroSubCuenta != null)
                            {

                                foreach (UltraGridRow Fila in grdDataDestinos.Rows)
                                {
                                    if (Fila.Cells["v_CuentaTransferencia"].Value != null && Fila.Cells["v_CuentaDestino"].Value != null)
                                    {
                                        if (grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaTransferencia"].Value != null)
                                        {
                                            if (frmPcc._NroSubCuenta.Trim() == Fila.Cells["v_CuentaDestino"].Value.ToString().Trim() && grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaTransferencia"].Value.ToString().Trim() == Fila.Cells["v_CuentaTransferencia"].Value.ToString().Trim())
                                            {
                                                grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaDestino"].Value = string.Empty;
                                                grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaDestino"].Value = string.Empty;
                                                UltraMessageBox.Show("Está Cuenta Destino ya fue asignada ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                return;
                                            }
                                        }
                                    }
                                }


                                if ((CuentaOrigen.StartsWith("60") && (int.Parse(frmPcc._NroSubCuenta.ToString().Substring(0, 2)) >= 20 && int.Parse(frmPcc._NroSubCuenta.ToString().Substring(0, 2)) <= 28)) || ((decimal.Parse(CuentaOrigen.Substring(0, 2)) >= 62 && decimal.Parse(CuentaOrigen.Substring(0, 2)) <= 68) && (frmPcc._NroSubCuenta.ToString().StartsWith("90") || frmPcc._NroSubCuenta.ToString().StartsWith("92") || frmPcc._NroSubCuenta.ToString().StartsWith("94") || frmPcc._NroSubCuenta.ToString().StartsWith("95") || frmPcc._NroSubCuenta.ToString().StartsWith("97"))))
                                {

                                    grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaDestino"].Value = frmPcc._NombreCuenta.ToString();
                                    grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaDestino"].Value = frmPcc._NroSubCuenta;
                                }
                                else
                                {

                                    if (CuentaOrigen.StartsWith("60"))
                                    {
                                        grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaDestino"].Value = string.Empty;
                                        grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaDestino"].Value = string.Empty;
                                        UltraMessageBox.Show("Cuenta Destino  pertenece a la Cuenta 2", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return;
                                    }
                                    else
                                    {
                                        if (int.Parse(CuentaOrigen.Substring(0, 2)) >= 62 && int.Parse(CuentaOrigen.Substring(0, 2)) <= 68)
                                        {
                                            grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaDestino"].Value = string.Empty;
                                            grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaDestino"].Value = string.Empty;
                                            UltraMessageBox.Show("Cuenta Destino pertenece a la Cuenta 90-92-94-95 o 97 ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return;

                                        }
                                    }
                                    grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaDestino"].Value = string.Empty;
                                    grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaDestino"].Value = string.Empty;
                                }
                            }
                        }
                        else
                        {

                            if (CuentaOrigen.StartsWith("60"))
                            {
                                grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaDestino"].Value = string.Empty;
                                grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaDestino"].Value = string.Empty;
                                UltraMessageBox.Show("Cuenta Destino  pertenece a la Cuenta 2", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;

                            }
                            else
                            {
                                grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaDestino"].Value = string.Empty;
                                grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaDestino"].Value = string.Empty;
                                UltraMessageBox.Show("Cuenta Destino  pertenece a la Cuenta 90-92-94-95 o 97", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }


                        }
                    }
                    grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                    break;

                case "v_CuentaTransferencia":

                    var asientoTransferencia = _objBL.ObtenerAsientoImputable(e.Cell.Text);
                    if (asientoTransferencia != null)
                    {

                        if (CuentaOrigen.Length >= 2)
                        {
                            foreach (UltraGridRow Fila in grdDataDestinos.Rows)
                            {
                                if (Fila.Cells["v_CuentaTransferencia"].Value != null && Fila.Cells["v_CuentaDestino"].Value != null && grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaDestino"].Value != null && Fila.Index != grdDataDestinos.ActiveRow.Index)
                                {

                                    if (asientoTransferencia.v_NroCuenta.Trim() == Fila.Cells["v_CuentaTransferencia"].Value.ToString() && grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaDestino"].Value.ToString() == Fila.Cells["v_CuentaDestino"].Value.ToString())
                                    {
                                        UltraMessageBox.Show("Está Cuenta Transferencia ya fue asignada ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaTransferencia"].Value = string.Empty;
                                        grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaTransferencia"].Value = string.Empty;
                                        return;
                                    }
                                }
                            }

                            if (CuentaOrigen.StartsWith("60") && asientoTransferencia.v_NroCuenta.StartsWith("61") || (int.Parse(CuentaOrigen.Substring(0, 2)) >= 62 && int.Parse(CuentaOrigen.Substring(0, 2)) <= 68 && asientoTransferencia.v_NroCuenta.StartsWith("79")))
                            {
                                grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaTransferencia"].Value = asientoTransferencia.v_NombreCuenta;
                            }
                            else
                            {
                                if (CuentaOrigen.StartsWith("60"))
                                {
                                    grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaTransferencia"].Value = string.Empty;
                                    grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaTransferencia"].Value = string.Empty;
                                    UltraMessageBox.Show("Cuenta Transferencia  pertenece a la Cuenta 61", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                                else
                                {
                                    if (int.Parse(CuentaOrigen.Substring(0, 2)) >= 62 && int.Parse(CuentaOrigen.Substring(0, 2)) <= 68)
                                    {

                                        grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaTransferencia"].Value = string.Empty;
                                        grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaTransferencia"].Value = string.Empty;
                                        UltraMessageBox.Show("Cuenta Transferencia  pertenece la Cuenta 79", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return;

                                    }
                                }
                                grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaTransferencia"].Value = string.Empty;
                            }
                        }

                    }

                    else
                    {

                        if (grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaTransferencia"].Text.Trim() == string.Empty)
                        {

                            string cuentaTransferencia = CuentaOrigen.StartsWith("60") ? "61" : "79";
                            frmPlanCuentasConsulta frmPc = new frmPlanCuentasConsulta(cuentaTransferencia);
                            frmPc.ShowDialog();
                            if (CuentaOrigen.Length >= 2 && frmPc._NroSubCuenta != null)
                            {
                                foreach (UltraGridRow Fila in grdDataDestinos.Rows)
                                {
                                    if (Fila.Cells["v_CuentaTransferencia"].Value != null && Fila.Cells["v_CuentaDestino"].Value != null && grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaDestino"].Value != null)
                                    {

                                        if (frmPc._NroSubCuenta == Fila.Cells["v_CuentaTransferencia"].Value.ToString() && grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaDestino"].Value.ToString() == Fila.Cells["v_CuentaDestino"].Value.ToString())
                                        {

                                            //grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaTransferencia"].Value = string.Empty;
                                            //grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaTransferencia"].Value = string.Empty;
                                            UltraMessageBox.Show("Está Cuenta Transferencia ya fue asignada ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            return;
                                        }
                                    }
                                }


                                if (CuentaOrigen.StartsWith("60") && frmPc._NroSubCuenta.StartsWith("61") || (int.Parse(CuentaOrigen.Substring(0, 2)) >= 62 && int.Parse(CuentaOrigen.Substring(0, 2)) <= 68 && frmPc._NroSubCuenta.StartsWith("79")))
                                {
                                    grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaTransferencia"].Value = frmPc._NombreCuenta.ToString();
                                    grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaTransferencia"].Value = frmPc._NroSubCuenta;
                                }
                                else
                                {
                                    if (CuentaOrigen.StartsWith("60"))
                                    {

                                        grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaTransferencia"].Value = string.Empty;
                                        grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaTransferencia"].Value = string.Empty;
                                        UltraMessageBox.Show("Cuenta Transferencia   pertenece a la Cuenta 61", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return;
                                    }
                                    else
                                    {
                                        if (int.Parse(CuentaOrigen.Substring(0, 2)) >= 62 && int.Parse(CuentaOrigen.Substring(0, 2)) <= 68)
                                        {
                                            grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaTransferencia"].Value = string.Empty;
                                            grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaTransferencia"].Value = string.Empty;
                                            UltraMessageBox.Show("Cuenta Transferencia pertenece a la Cuenta 79", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return;

                                        }
                                    }
                                    grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaTransferencia"].Value = string.Empty;
                                    grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaTransferencia"].Value = string.Empty;

                                }
                            }
                        }
                        else
                        {

                            if (CuentaOrigen.StartsWith("60"))
                            {
                                grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaTransferencia"].Value = string.Empty;
                                grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaTransferencia"].Value = string.Empty;
                                UltraMessageBox.Show("Cuenta Transferencia pertenece a la Cuenta 61", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            else
                            {
                                grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["v_CuentaTransferencia"].Value = string.Empty;
                                grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["NombreCuentaTransferencia"].Value = string.Empty;
                                UltraMessageBox.Show("Cuenta Transferencia pertenece a la Cuenta 79", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }


                        }

                    }
                    grdDataDestinos.Rows[grdDataDestinos.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                    break;

            }
        }

        private void txtTipoExistencia_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscarTipoExistencia")
            {
                Mantenimientos.frmBuscarDatahierarchy frm2 = new Mantenimientos.frmBuscarDatahierarchy(147, "CLASIFICACIÓN DE LOS BIENES Y SERVICIOS ADQUIRIDOS");
                frm2.ShowDialog();
                if (frm2._itemId != null)
                {
                    txtTipoExistencia.Text = frm2._value2;
                    txtNomExistencia.Text = frm2._value1;
                }
                else
                {
                    // txtIdEntidadF.Clear(); 
                }

            }
        }

        private void txtTipoExistencia_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtTipoExistencia, e);
        }

        private string ObtenerNombrePorCodigo(int pintGroupId, string pstrCode)
        {
            var objDatahierachy = new DatahierarchyBL().ObtenerDatahierarcyCodigo(pintGroupId, pstrCode);
            return ((objDatahierachy != null) ? objDatahierachy.v_Value1 : "");
        }

        private void btnBuscarYReemplazarCuentas_Click(object sender, EventArgs e)
        {
            var f = new FrmReemplazarCuentasContables();
            f.ShowDialog();
        }

        private void txtAreasID_KeyPress(object sender, KeyPressEventArgs e)
        {
            Entrada_Numerica(e);
        }
    }
}

