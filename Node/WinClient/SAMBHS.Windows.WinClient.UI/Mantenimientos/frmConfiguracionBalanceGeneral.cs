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
    public partial class frmConfiguracionBalanceGeneral : Form
    {
        AsientosContablesBL _objBL = new AsientosContablesBL();
        List<asientocontableDto> TempInsertar = new List<asientocontableDto>();
        public configuracionbalancesDto _objConfiguracionBalanceDto = new configuracionbalancesDto();
        asientocontableDto _asientosDto = new asientocontableDto();
        destinoDto _objDestinoDto = new destinoDto();
        string _Mode = "";
        public string CuentaOrigen = string.Empty;
        List<string> CuentasAsociadasSituacionFinanciera = new List<string>();
        List<string> CuentasAsociadasEstadoResultadosFuncion = new List<string>();
        List<string> CuentasAsociadasEstadoResultadosNaturaleza = new List<string>();
        readonly SecurityBL _obSecurityBL = new SecurityBL();
        string MensajeGuardar = "";
        private Dictionary<string, string> _tablesCodigosEstadosResultados;
        private Dictionary<string, string> _tablesCodigosEstadosSituacionFinanciera;
        public frmConfiguracionBalanceGeneral(string Parametro)
        {
            InitializeComponent();
            TablaCodigosEstadosResultados();
            TablaCodigosEstadosSituacionFinanciera();
        }
        private void TablaCodigosEstadosResultados()
        {
            _tablesCodigosEstadosResultados = new Dictionary<string, string>
            {
                {"G1", ""},
                {"G11", ""},
                {"G111", ""},
                {"G112", ""},
                {"G12", ""},
                {"G121", ""},
                {"G122", ""},
                {"G2", ""},
                {"G21", ""},
                {"G211", ""},
                {"G212", ""},
                {"G213", ""},
                {"G214", ""},
                {"G215", ""},
                {"G3", ""},
                {"G31", ""},
                {"G311", ""},
                {"G312", ""},
                {"G313", ""},
                {"G314", ""},
                {"G4", ""},
                {"G41", ""},
                {"G411", ""},
                {"G412", ""},
                {"G5", ""},
                {"G51", ""},
                {"G511", ""},          
           };

        }



        private void TablaCodigosEstadosSituacionFinanciera()
        {
            _tablesCodigosEstadosSituacionFinanciera = new Dictionary<string, string>
            {
                {"G1", ""},
                {"G11", ""},
                {"G111", ""},
                {"G112", ""},
                {"G113", ""},
                {"G114", ""},
                {"G115", ""},
                {"G116", ""},
                {"G12", ""},
                {"G121", ""},
                {"G122", ""},
                {"G123", ""},
                {"G124", ""},
                {"G125", ""},
                {"G2", ""},
                {"G3", ""},
                {"G31", ""},
                {"G311", ""},
                {"G312", ""},
                {"G313", ""},
                {"G314", ""},
                {"G32", ""},
                {"G321", ""},
                {"G322", ""},
                {"G323", ""},
                {"G4", ""},
                {"G41", ""},
                {"G411", ""},
                {"G412", ""},
                {"G413", ""},
                {"G414", ""},
                {"G415", ""},
                {"G416", ""},
                    
           };

        }

        private void frmConfiguracionBalanceGeneral_Load(object sender, EventArgs e)
        {
            cbMeses.Value = DateTime.Now.Month.ToString();
            this.BackColor = new GlobalFormColors().FormColor;
            BindGridBalanceFuncion();
            BindGridSituacionFinanciera(cbMeses.Value.ToString ());
            BindGridBalanceNaturaleza();
            CargarCombos();
        }
        private void CargarCombos()
        {
            DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
            OperationResult objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList(cboTipoNota, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 172, null).OrderBy (o=>o.Id).ToList (), DropDownListAction.Select);
            cboTipoNota.Value = "-1";
        }
        private void BindGridSituacionFinanciera(string Mes)
        {
            var objData = GetDataSituacionFinanciera(Mes,Globals.ClientSession.i_Periodo.ToString ());
            grdVisualizarSituacionFinanciera.DataSource = objData;

        }
        private void BindGridBalanceFuncion()
        {
            var objData = GetDataBalanceFuncion();
            grdVisualizarBalanceFuncion.DataSource = objData;

        }

        private void BindGridBalanceNaturaleza()
        {
            var objData = GetDataBalanceNaturaleza();
            grdVisualizarBalanceNaturaleza.DataSource = objData;
            

        }

        private List<configuracionbalancesDto> GetDataSituacionFinanciera(string Mes,string Periodo)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _objBL.ObtenerDatosBalances(ref objOperationResult, ((int)ConfiguracionBalances.SituacionFinanciera).ToString(),Mes ,Periodo);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }


        private List<configuracionbalancesDto> GetDataBalanceFuncion()
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _objBL.ObtenerDatosBalances(ref objOperationResult, ((int)ConfiguracionBalances.EstadodeResultadosFuncion).ToString(),DateTime.Now.Month.ToString ("00"),Globals.ClientSession.i_Periodo.ToString ());

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }

        private List<configuracionbalancesDto> GetDataBalanceNaturaleza()
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _objBL.ObtenerDatosBalances(ref objOperationResult, ((int)ConfiguracionBalances.EstadodeResultadosNaturaleza).ToString(),DateTime.Now.Month.ToString ("00"),Globals.ClientSession.i_Periodo.ToString ());

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }
        #region Clases/Validaciones
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


        public void ActivarBotones(bool editar, bool eliminar, bool guardar, bool cancelar, bool nuevo)
        {
            btnEditarPlan.Enabled = editar;
            btnEliminarSituacionFinanciera.Enabled = eliminar;
            btnGuardarSituacionFinanciera.Enabled = guardar;
            btnCancelarSituacionFinanciera.Enabled = cancelar;
            btnNuevoSituacionFinanciera.Enabled = nuevo;
        }
        public bool IsNumeric(object Expression)
        {
            bool isNum;
            double retNum;
            isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        private void MantenerSeleccionFuncion(int ValorSeleccionado)
        {

            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in grdVisualizarBalanceFuncion.Rows)
            {
                if (int.Parse(row.Cells["i_IdConfiguracionBalance"].Text) == ValorSeleccionado)
                {

                    grdVisualizarBalanceFuncion.ActiveRow = row;
                    grdVisualizarBalanceFuncion.ActiveRow.Selected = true;
                    break;
                }
            }
        }

        private void MantenerSeleccionSituacionFinanciera(int ValorSeleccionado)
        {

            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in grdVisualizarSituacionFinanciera.Rows)
            {
                if (int.Parse(row.Cells["i_IdConfiguracionBalance"].Text) == ValorSeleccionado)
                {

                    grdVisualizarSituacionFinanciera.ActiveRow = row;
                    grdVisualizarSituacionFinanciera.ActiveRow.Selected = true;
                    break;
                }
            }
        }

        private void MantenerSeleccionNaturaleza(int ValorSeleccionado)
        {

            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in grdVisualizarBalanceNaturaleza.Rows)
            {
                if (int.Parse(row.Cells["i_IdConfiguracionBalance"].Text) == ValorSeleccionado)
                {

                    grdVisualizarBalanceNaturaleza.ActiveRow = row;
                    grdVisualizarBalanceNaturaleza.ActiveRow.Selected = true;
                    break;
                }
            }
        }
        #endregion





        private void LLenarTemporalesCuentasEstadoResultadosFuncion()
        {

            foreach (var item in grdCuentasFuncion.Rows)
            {
                var Cuenta = item.Cells["v_NroCuenta"].Value.ToString();
                CuentasAsociadasEstadoResultadosFuncion.Add(Cuenta);
                

            }

        }

        private void LLenarTemporalesCuentasEstadoResultadosNaturaleza()
        {

            foreach (var item in grdCuentasNaturaleza.Rows)
            {
                var Cuenta = item.Cells["v_NroCuenta"].Value.ToString();
                CuentasAsociadasEstadoResultadosNaturaleza.Add(Cuenta);

            }

        }
        private void LLenarTemporalesCuentasSituacionFinanciera()
        {

            foreach (var item in grdCuentasSituacionFinanciera.Rows)
            {
                var Cuenta = item.Cells["v_NroCuenta"].Value.ToString();
                CuentasAsociadasSituacionFinanciera.Add(Cuenta);

            }

        }
        private void btnGuardarBalanceFuncion_Click(object sender, EventArgs e)
        {

            OperationResult objOperationResult = new OperationResult();
            int i_IdBalanceFuncion = 0;
            LLenarTemporalesCuentasEstadoResultadosFuncion();
            if (_Mode == "New")
            {
                _objConfiguracionBalanceDto = new configuracionbalancesDto
                {
                    v_Codigo = txtCodigoBalanceFuncion.Text,
                    v_Nombre = txtNombreBalanceFuncion.Text,
                    v_TipoBalance = ((int)ConfiguracionBalances.EstadodeResultadosFuncion).ToString(),
                    v_NombreGrupo = txtNombreTotalBalanceFuncion.Text,
                    //i_TipoOperacion = int.Parse(cboTipoOperacionBalanceFuncion.Value.ToString()),


                };

                i_IdBalanceFuncion = _objBL.ConfiguracionBalanceNuevo(ref objOperationResult, _objConfiguracionBalanceDto, Globals.ClientSession.GetAsList(), CuentasAsociadasEstadoResultadosFuncion, (int)ConfiguracionBalances.EstadodeResultadosFuncion);
            }
            else if (_Mode == "Edit")
            {
                _objConfiguracionBalanceDto.v_Codigo = txtCodigoBalanceFuncion.Text;
                _objConfiguracionBalanceDto.v_Nombre = txtNombreBalanceFuncion.Text;
                _objConfiguracionBalanceDto.v_NombreGrupo = txtNombreTotalBalanceFuncion.Text;
               // _objConfiguracionBalanceDto.i_TipoOperacion = int.Parse(cboTipoOperacionBalanceFuncion.Value.ToString());
                i_IdBalanceFuncion = _objBL.ConfiguracionBalanceActualizar(ref objOperationResult, _objConfiguracionBalanceDto, Globals.ClientSession.GetAsList(), CuentasAsociadasEstadoResultadosFuncion   , (int)ConfiguracionBalances.EstadodeResultadosFuncion);
            }

            if (objOperationResult.Success == 1)  // Operación sin error
            {

                _Mode = "Edit";

                CuentasAsociadasEstadoResultadosFuncion = new List<string>();
                UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                BindGridBalanceFuncion();
                CargarGrillaBalanceFuncion(txtCodigoBalanceFuncion.Text);
                MantenerSeleccionFuncion(i_IdBalanceFuncion);
            }
            else
            {
                UltraMessageBox.Show(MensajeGuardar, "ERROR!!!-", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }



        private void btnAgregarBalanceFuncion_Click(object sender, EventArgs e)
        {

            UltraGridRow row = grdCuentasFuncion.DisplayLayout.Bands[0].AddNew();
            grdCuentasFuncion.Rows.Move(row, grdCuentasFuncion.Rows.Count() - 1);
            this.grdCuentasFuncion.ActiveRowScrollRegion.ScrollRowIntoView(row);

            UltraGridCell aCell = this.grdCuentasFuncion.ActiveRow.Cells["v_NroCuenta"];
            this.grdCuentasFuncion.ActiveCell = aCell;
            grdCuentasFuncion.ActiveRow = aCell.Row;
            grdCuentasFuncion.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
            grdCuentasFuncion.PerformAction(UltraGridAction.EnterEditMode, false, false);
        }

        private void btnNuevoBalanceFuncion_Click(object sender, EventArgs e)
        {

            if (grdVisualizarBalanceFuncion.ActiveRow == null)
            {
            }
            else
            {

                string Codigo = grdVisualizarBalanceFuncion.Rows[grdVisualizarBalanceFuncion.ActiveRow.Index].Cells["v_Codigo"].Value.ToString();
                _Mode = "New";
                CargarGrillaBalanceFuncion("");
                btnGuardarBalanceFuncion.Enabled = true;
                txtCodigoBalanceFuncion.Focus();
                LimpiarBalanceFuncion(Codigo);
                HabilitarBotonesBalanceFuncion(true, false, true, false, true, "");
            }
        }
        private void HabilitarBotonesBalanceFuncion(bool Guardar, bool Eliminar, bool grillaCuentasBalanceFuncion, bool grillaVisualizarBalance, bool Cancelar, string Codigo)
        {
            btnGuardarBalanceFuncion.Enabled = Guardar;
            if (!_tablesCodigosEstadosResultados.ContainsKey(Codigo))
            {
                btnEliminarBalanceFuncion.Enabled = Eliminar;
            }
            grpEstadoResultadosNaturaleza.Enabled = grillaCuentasBalanceFuncion;
            grdVisualizarBalanceNaturaleza.Enabled = grillaVisualizarBalance;
            btnCancelarBalanceNaturaleza.Enabled = Cancelar;
           // txtCodigoBalanceFuncion.Enabled = !_tablesCodigosEstadosResultados.ContainsKey(Codigo);
            grpBotonesActualizarEstadoResultadosNaturaleza.Enabled = Codigo.Length > 3 ? true : false;


        }


        private void HabilitarBotonesBalanceNaturaleza(bool Guardar, bool Eliminar, bool grillaCuentasBalanceFuncion, bool grillaVisualizarBalance, bool Cancelar, string Codigo)
        {
            btnGuardarBalanceNaturaleza.Enabled = Guardar;
            if (!_tablesCodigosEstadosResultados.ContainsKey(Codigo))
            {
                btnEliminarBalanceNaturaleza.Enabled = Eliminar;
            }
            grpEstadoResultadosNaturaleza.Enabled = grillaCuentasBalanceFuncion;
            grdVisualizarBalanceNaturaleza.Enabled = grillaVisualizarBalance;
            btnCancelarBalanceNaturaleza.Enabled = Cancelar;
            // txtCodigoBalanceFuncion.Enabled = !_tablesCodigosEstadosResultados.ContainsKey(Codigo);
            grpBotonesActualizarEstadoResultadosNaturaleza.Enabled = Codigo.Length > 3 ? true : false;


        }


        private void CargarGrillaBalanceFuncion(string CodigoBalance)
        {
            OperationResult pobjOperationResult = new OperationResult();
            var DataSource = new AsientosContablesBL().DevuelveCuentasBalances(ref pobjOperationResult, CodigoBalance, (int)ConfiguracionBalances.EstadodeResultadosFuncion);
            if (pobjOperationResult.Success == 1)
            {
                grdCuentasFuncion.DataSource = DataSource;
            }
            else
            {
                UltraMessageBox.Show(pobjOperationResult.ErrorMessage + "\n\n" + pobjOperationResult.ExceptionMessage + "\n\nTARGET: " + pobjOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarGrillaBalanceNaturaleza(string CodigoBalance)
        {
            OperationResult pobjOperationResult = new OperationResult();
            var DataSource = new AsientosContablesBL().DevuelveCuentasBalances(ref pobjOperationResult, CodigoBalance, (int)ConfiguracionBalances.EstadodeResultadosNaturaleza);
            if (pobjOperationResult.Success == 1)
            {

                grdCuentasNaturaleza.DataSource = DataSource;
            }
            else
            {
                UltraMessageBox.Show(pobjOperationResult.ErrorMessage + "\n\n" + pobjOperationResult.ExceptionMessage + "\n\nTARGET: " + pobjOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarBalanceFuncion(string Codigo)
        {


            var MaxCodigos = _objBL.ObtenerMaximoCodigo(((int)ConfiguracionBalances.EstadodeResultadosFuncion).ToString(), Codigo.Length, Codigo);
            int codEntero = int.Parse(MaxCodigos.Substring(1, MaxCodigos.Length - 1));
            codEntero++;
           
            txtCodigoBalanceFuncion.Text = "G" + codEntero.ToString();
            txtNombreBalanceFuncion.Clear();
            //cboTipoOperacionBalanceFuncion.Value = "-1";
            txtNombreTotalBalanceFuncion.Clear();
            grdCuentasFuncion.ClearUndoHistory();
            txtCodigoBalanceFuncion.Enabled = false;
            txtNombreBalanceFuncion.Focus();


        }

        private void LimpiarBalanceNaturaleza(string Codigo)
        {


            var MaxCodigos = _objBL.ObtenerMaximoCodigo(((int)ConfiguracionBalances.EstadodeResultadosNaturaleza).ToString(), Codigo.Length, Codigo);
            int codEntero = int.Parse(MaxCodigos.Substring(1, MaxCodigos.Length - 1));
            codEntero++;
            txtCodigoBalanceNaturaleza.Text = "G" + codEntero.ToString();
            txtNombreBalanceNaturaleza.Clear();
            txtNombreTotalBalanceNaturaleza.Clear();
            grdCuentasNaturaleza.ClearUndoHistory();
            txtCodigoBalanceNaturaleza.Enabled = false;
            txtNombreBalanceNaturaleza.Focus();


        }

        private void btnRangoBalanceFuncion_Click(object sender, EventArgs e)
        {

        }

        private void f_TerminadoEventBalanceFuncion(List<asientocontableDto> lista)
        {
            try
            {
                foreach (var l in lista)
                {
                    if (grdCuentasFuncion.Rows.ToList().Any(f => f.Cells["v_Nrocuenta"].Value != null && f.Cells["v_Nrocuenta"].Value.ToString().Equals(l.v_NroCuenta)))
                        continue;

                    UltraGridRow row = grdCuentasFuncion.DisplayLayout.Bands[0].AddNew();
                    grdCuentasFuncion.Rows.Move(row, grdCuentasFuncion.Rows.Count() - 1);
                    this.grdCuentasFuncion.ActiveRowScrollRegion.ScrollRowIntoView(row);
                    row.Cells["v_NroCuenta"].SetValue(l.v_NroCuenta, false);
                    row.Cells["v_NombreCuenta"].SetValue(l.v_NombreCuenta, false);
                }

                var objOperationResult = new OperationResult();
                grdCuentasFuncion.Rows.ToList().ForEach(p => TempInsertar.Add((asientocontableDto)p.ListObject));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void f_TerminadoEventBalanceNaturaleza(List<asientocontableDto> lista)
        {
            try
            {
                foreach (var l in lista)
                {
                    if (grdCuentasNaturaleza.Rows.ToList().Any(f => f.Cells["v_Nrocuenta"].Value != null && f.Cells["v_Nrocuenta"].Value.ToString().Equals(l.v_NroCuenta)))
                        continue;

                    UltraGridRow row = grdCuentasNaturaleza.DisplayLayout.Bands[0].AddNew();
                    grdCuentasNaturaleza.Rows.Move(row, grdCuentasNaturaleza.Rows.Count() - 1);
                    this.grdCuentasNaturaleza.ActiveRowScrollRegion.ScrollRowIntoView(row);
                    row.Cells["v_NroCuenta"].SetValue(l.v_NroCuenta, false);
                    row.Cells["v_NombreCuenta"].SetValue(l.v_NombreCuenta, false);
                }

                var objOperationResult = new OperationResult();
                grdCuentasNaturaleza.Rows.ToList().ForEach(p => TempInsertar.Add((asientocontableDto)p.ListObject));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void grdVisualizarBalanceFuncion_AfterRowActivate(object sender, EventArgs e)
        {
            if (grdVisualizarBalanceFuncion.ActiveRow != null)
            {
                OperationResult objOperationResult = new OperationResult();
                using (new LoadingClass.PleaseWait(this.Location, "Por favor espere..."))
                {
                    _Mode = "Edit";
                    CuentasAsociadasEstadoResultadosFuncion = new List<string>();
                    int i_IdConfiguracionBalance = int.Parse(grdVisualizarBalanceFuncion.Rows[grdVisualizarBalanceFuncion.ActiveRow.Index].Cells["i_IdConfiguracionBalance"].Value.ToString());
                    _objConfiguracionBalanceDto = new configuracionbalancesDto();
                    _objConfiguracionBalanceDto = _objBL.ObtenerCabeceraBalance(ref objOperationResult, i_IdConfiguracionBalance, ((int)ConfiguracionBalances.EstadodeResultadosFuncion).ToString());
                    if (objOperationResult.Success == 1)
                    {
                        txtCodigoBalanceFuncion.Text = _objConfiguracionBalanceDto.v_Codigo;
                        txtNombreBalanceFuncion.Text = _objConfiguracionBalanceDto.v_Nombre;
                        //cboTipoOperacionBalanceFuncion.Value = _objConfiguracionBalanceDto.i_TipoOperacion == null ? "-1" : _objConfiguracionBalanceDto.i_TipoOperacion.Value.ToString();
                        txtNombreTotalBalanceFuncion.Text = _objConfiguracionBalanceDto.v_NombreGrupo;
                        CargarDetalleBalanceFuncion(_objConfiguracionBalanceDto.v_Codigo);
                        HabilitarBotonesBalanceFuncion(true, true, true, true, true, txtCodigoBalanceFuncion.Text);

                    }
                    else
                    {
                        UltraMessageBox.Show(Constants.GenericErrorMessage, "Ocurrió un error al cargar Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                }
            }
        }


        private void CargarDetalleBalanceFuncion(string CodigoBalance)
        {
            OperationResult objOperationResult = new OperationResult();
            var Detalles = _objBL.DevuelveCuentasBalances(ref objOperationResult, CodigoBalance, (int)ConfiguracionBalances.EstadodeResultadosFuncion);
            if (objOperationResult.Success == 1)
            {
                grdCuentasFuncion.DataSource = Detalles;
            }
            else
            {
                UltraMessageBox.Show(Constants.GenericErrorMessage, "Ocurrió un error al cargar Detalles", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarDetalleBalanceNaturaleza(string CodigoBalance)
        {
            OperationResult objOperationResult = new OperationResult();
            var Detalles = _objBL.DevuelveCuentasBalances(ref objOperationResult, CodigoBalance, (int)ConfiguracionBalances.EstadodeResultadosNaturaleza);
            if (objOperationResult.Success == 1)
            {
                grdCuentasNaturaleza.DataSource = Detalles;
            }
            else
            {
                UltraMessageBox.Show(Constants.GenericErrorMessage, "Ocurrió un error al cargar Detalles", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void grdCuentasFuncion_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {

        }





        private void btnEliminarCuentaBalanceFuncion_Click(object sender, EventArgs e)
        {
            if (grdCuentasFuncion.ActiveRow == null) return;
            if (UltraMessageBox.Show("¿Seguro de Eliminar este Registro?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                grdCuentasFuncion.Rows[grdCuentasFuncion.ActiveRow.Index].Delete(false);
            }
        }

        private void btnEliminarBalanceFuncion_Click(object sender, EventArgs e)
        {

            if (grdVisualizarBalanceFuncion.ActiveRow == null) return;
            OperationResult objOperationResult = new OperationResult();

            if (UltraMessageBox.Show("¿Seguro de Eliminar este Registro?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {

                _objBL.ConfiguracionBalanceEliminar(ref objOperationResult, int.Parse(grdVisualizarBalanceFuncion.Rows[grdVisualizarBalanceFuncion.ActiveRow.Index].Cells["i_IdConfiguracionBalance"].Value.ToString()), Globals.ClientSession.GetAsList(), (int)ConfiguracionBalances.EstadodeResultadosFuncion);


                if (objOperationResult.Success == 1)
                {
                    BindGridBalanceFuncion();
                }
                else
                {
                    UltraMessageBox.Show(Constants.GenericErrorMessage, "Ocurrió un error al cargar Eliminar Configuración", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnCancelarBalanceFuncion_Click(object sender, EventArgs e)
        {
            HabilitarBotonesBalanceFuncion(true, true, true, true, true, "");
            
            BindGridBalanceFuncion();
        }

        private void btnLimpiarBalanceFuncion_Click(object sender, EventArgs e)
        {
            try
            {

                var objOperationResult = new OperationResult();
                foreach (var row in grdCuentasFuncion.Rows)
                {
                    row.Selected = true;

                }
                grdCuentasFuncion.DeleteSelectedRows(false);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnGuardarSituacionFinanciera_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            int i_IdBalanceFuncion = 0;
            LLenarTemporalesCuentasSituacionFinanciera();
            if (_Mode == "New")
            {
                _objConfiguracionBalanceDto = new configuracionbalancesDto
                {
                    v_Codigo = txtCodigoSituacionFinanciera.Text,
                    v_Nombre = txtNombreSituacionFinanciera.Text,
                    v_TipoBalance = ((int)ConfiguracionBalances.SituacionFinanciera).ToString(),
                    v_NombreGrupo = txtNombreTotalSituacionFinanciera.Text,
                    i_TipoNota =int.Parse ( cboTipoNota.Value.ToString ()),
                    v_Mes = cboMesSituacionFinanciera.Value.ToString (),
                    v_Periodo = Globals.ClientSession.i_Periodo.ToString (),
                    //i_IdTipoElemento = int.Parse ( cboTipoElementoSituacionFinanciera.Value.ToString ()),


                };

                i_IdBalanceFuncion = _objBL.ConfiguracionBalanceNuevo(ref objOperationResult, _objConfiguracionBalanceDto, Globals.ClientSession.GetAsList(), CuentasAsociadasSituacionFinanciera, (int)ConfiguracionBalances.SituacionFinanciera);
            }
            else if (_Mode == "Edit")
            {
                _objConfiguracionBalanceDto.v_Codigo = txtCodigoSituacionFinanciera.Text;
                _objConfiguracionBalanceDto.v_Nombre = txtNombreSituacionFinanciera.Text;
                _objConfiguracionBalanceDto.v_NombreGrupo = txtNombreTotalSituacionFinanciera.Text;
                _objConfiguracionBalanceDto.i_TipoNota = int.Parse(cboTipoNota.Value.ToString());
                i_IdBalanceFuncion = _objBL.ConfiguracionBalanceActualizar(ref objOperationResult, _objConfiguracionBalanceDto, Globals.ClientSession.GetAsList(), CuentasAsociadasSituacionFinanciera, (int)ConfiguracionBalances.SituacionFinanciera);
            }

            if (objOperationResult.Success == 1)  // Operación sin error
            {

                _Mode = "Edit";
                CuentasAsociadasSituacionFinanciera = new List<string>();
                UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);

                BindGridSituacionFinanciera(cbMeses.Value.ToString());
                CargarGrillaSituacionFinanciera(txtCodigoSituacionFinanciera.Text);

                MantenerSeleccionSituacionFinanciera(i_IdBalanceFuncion);
            }
            else
            {
                UltraMessageBox.Show(MensajeGuardar, "ERROR!!!-", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        //private void MantenerSeleccionSituacionFinanciera(int ValorSeleccionado)
        //{


        //    foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in grdVisualizarSituacionFinanciera.Rows)
        //    {
        //        if (int.Parse(row.Cells["i_IdConfiguracionBalance"].Text) == ValorSeleccionado)
        //        {

        //            grdVisualizarSituacionFinanciera.ActiveRow = row;
        //            grdVisualizarSituacionFinanciera.ActiveRow.Selected = true;
        //            break;
        //        }
        //    }
        //}

        private void btnAgregarSituacionFinanciera_Click(object sender, EventArgs e)
        {
            UltraGridRow row = grdCuentasSituacionFinanciera.DisplayLayout.Bands[0].AddNew();
            grdCuentasSituacionFinanciera.Rows.Move(row, grdCuentasSituacionFinanciera.Rows.Count() - 1);
            this.grdCuentasSituacionFinanciera.ActiveRowScrollRegion.ScrollRowIntoView(row);
            UltraGridCell aCell = this.grdCuentasSituacionFinanciera.ActiveRow.Cells["v_NroCuenta"];
            this.grdCuentasSituacionFinanciera.ActiveCell = aCell;
            grdCuentasSituacionFinanciera.ActiveRow = aCell.Row;
            grdCuentasSituacionFinanciera.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
            grdCuentasSituacionFinanciera.PerformAction(UltraGridAction.EnterEditMode, false, false);
        }

        private void btnEliminarCuentaSituacionFinanciera_Click(object sender, EventArgs e)
        {

            if (grdCuentasSituacionFinanciera.ActiveRow == null) return;
            if (UltraMessageBox.Show("¿Seguro de Eliminar este Registro?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                grdCuentasSituacionFinanciera.Rows[grdCuentasSituacionFinanciera.ActiveRow.Index].Delete(false);
            }
        }

        private void btnRangoSituacionFinanciera_Click(object sender, EventArgs e)
        {

        }



        private void f_TerminadoEventSituacionFinanciera(List<asientocontableDto> lista)
        {
            try
            {

                foreach (var l in lista)
                {
                    if (grdCuentasSituacionFinanciera.Rows.ToList().Any(f => f.Cells["v_Nrocuenta"].Value != null && f.Cells["v_Nrocuenta"].Value.ToString().Equals(l.v_NroCuenta)))
                        continue;

                    UltraGridRow row = grdCuentasSituacionFinanciera.DisplayLayout.Bands[0].AddNew();
                    grdCuentasSituacionFinanciera.Rows.Move(row, grdCuentasSituacionFinanciera.Rows.Count() - 1);
                    this.grdCuentasSituacionFinanciera.ActiveRowScrollRegion.ScrollRowIntoView(row);
                    row.Cells["v_NroCuenta"].SetValue(l.v_NroCuenta, false);
                    row.Cells["v_NombreCuenta"].SetValue(l.v_NombreCuenta, false);
                }

                var objOperationResult = new OperationResult();
                grdCuentasSituacionFinanciera.Rows.ToList().ForEach(p => TempInsertar.Add((asientocontableDto)p.ListObject));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }





        private void btnNuevoSituacionFinanciera_Click(object sender, EventArgs e)
        {

            if (grdVisualizarSituacionFinanciera.ActiveRow == null)
            {
            }
            else
            {

                string Codigo = grdVisualizarSituacionFinanciera.Rows[grdVisualizarSituacionFinanciera.ActiveRow.Index].Cells["v_Codigo"].Value.ToString();
                _Mode = "New";
                CargarGrillaSituacionFinanciera("");
                txtCodigoSituacionFinanciera.Focus();
                LimpiarSituacionFinanciera(Codigo);
                HabilitarBotonesSituacionFinanciera(true, false, true, false, true, "");
            }
        }

        private void CargarGrillaSituacionFinanciera(string CodigoBalance)
        {
            OperationResult pobjOperationResult = new OperationResult();
            var DataSource = new AsientosContablesBL().DevuelveCuentasBalances(ref pobjOperationResult, CodigoBalance, (int)ConfiguracionBalances.SituacionFinanciera);
            if (pobjOperationResult.Success == 1)
            {

                grdCuentasSituacionFinanciera.DataSource = DataSource;
            }
            else
            {
                UltraMessageBox.Show(pobjOperationResult.ErrorMessage + "\n\n" + pobjOperationResult.ExceptionMessage + "\n\nTARGET: " + pobjOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void LimpiarSituacionFinanciera(string Codigo)
        {
            var MaxCodigos = _objBL.ObtenerMaximoCodigo(((int)ConfiguracionBalances.SituacionFinanciera).ToString(), Codigo.Length, Codigo);
            int codEntero = int.Parse(MaxCodigos.Substring(1, MaxCodigos.Length - 1));
            codEntero++;
            txtCodigoSituacionFinanciera.Text  = "G" + codEntero.ToString();
            txtNombreSituacionFinanciera.Clear();
            txtNombreTotalSituacionFinanciera.Clear();
            grdCuentasFuncion.ClearUndoHistory();
            txtCodigoSituacionFinanciera.Enabled = false;
            txtNombreSituacionFinanciera.Focus();
            cboMesSituacionFinanciera.Value = DateTime.Now.Month.ToString();
            

        }



        private void HabilitarBotonesSituacionFinanciera(bool Guardar, bool Eliminar, bool grillaCuentasSituacionFinanciera, bool grillaVisualizarSituacionFinanciera, bool Cancelar, string Codigo)
        {
            btnGuardarSituacionFinanciera.Enabled = Guardar;
            if (!_tablesCodigosEstadosSituacionFinanciera.ContainsKey(Codigo))
            {
                btnEliminarSituacionFinanciera.Enabled = Eliminar;
            }
            grpSituacionFinanciera.Enabled = grillaCuentasSituacionFinanciera;
            grdVisualizarSituacionFinanciera.Enabled = grillaVisualizarSituacionFinanciera;
            btnCancelarSituacionFinanciera.Enabled = Cancelar;
            grpBotonesActualizarSituacionFinanciera.Enabled = Codigo.Length > 3 ? true : false;
            //txtCodigoSituacionFinanciera.Enabled = !_tablesCodigosEstadosSituacionFinanciera.ContainsKey(Codigo); 
        }

        private void grdVisualizarSituacionFinanciera_AfterRowActivate(object sender, EventArgs e)
        {
            if (grdVisualizarSituacionFinanciera.ActiveRow != null)
            {
                OperationResult objOperationResult = new OperationResult();
                using (new LoadingClass.PleaseWait(this.Location, "Por favor espere..."))
                {
                    _Mode = "Edit";
                    CuentasAsociadasSituacionFinanciera = new List<string>();
                    int i_IdConfiguracionBalance = int.Parse(grdVisualizarSituacionFinanciera.Rows[grdVisualizarSituacionFinanciera.ActiveRow.Index].Cells["i_IdConfiguracionBalance"].Value.ToString());
                    _objConfiguracionBalanceDto = new configuracionbalancesDto();
                    _objConfiguracionBalanceDto = _objBL.ObtenerCabeceraBalance(ref objOperationResult, i_IdConfiguracionBalance, ((int)ConfiguracionBalances.SituacionFinanciera).ToString());
                    if (objOperationResult.Success == 1)
                    {

                        txtCodigoSituacionFinanciera.Text = _objConfiguracionBalanceDto.v_Codigo;
                        txtNombreSituacionFinanciera.Text = _objConfiguracionBalanceDto.v_Nombre;
                        txtNombreTotalSituacionFinanciera.Text = _objConfiguracionBalanceDto.v_NombreGrupo;
                        cboTipoNota.Value = _objConfiguracionBalanceDto.i_TipoNota == null ? "-1" : _objConfiguracionBalanceDto.i_TipoNota.Value.ToString ();
                        cboMesSituacionFinanciera.Value = _objConfiguracionBalanceDto.v_Mes.ToString();
                        //CargarDetalleSituacionFinanciera(_objConfiguracionBalanceDto.v_Codigo);
                        CargarDetalleSituacionFinanciera(_objConfiguracionBalanceDto.i_IdConfiguracionBalance.ToString());
                        HabilitarBotonesSituacionFinanciera(true, true, true, true, true, txtCodigoSituacionFinanciera.Text);

                    }
                    else
                    {
                        UltraMessageBox.Show(Constants.GenericErrorMessage, "Ocurrió un error al cargar Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                }
            }
        }

        private void CargarDetalleSituacionFinanciera(string CodigoBalance)
        {
            OperationResult objOperationResult = new OperationResult();
            //var Detalles = _objBL.DevuelveCuentasBalances(ref objOperationResult, CodigoBalance, (int)ConfiguracionBalances.SituacionFinanciera);
            var Detalles = _objBL.DevuelveCuentasBalances(ref objOperationResult, CodigoBalance, (int)ConfiguracionBalances.SituacionFinanciera);
            if (objOperationResult.Success == 1)
            {

                grdCuentasSituacionFinanciera.DataSource = Detalles;
            }
            else
            {
                UltraMessageBox.Show(Constants.GenericErrorMessage, "Ocurrió un error al cargar Detalles", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void btnLimpiarSituacionFinanciera_Click(object sender, EventArgs e)
        {
            try
            {
                var objOperationResult = new OperationResult();
                foreach (var row in grdCuentasSituacionFinanciera.Rows)
                {
                    row.Selected = true;

                }
                grdCuentasSituacionFinanciera.DeleteSelectedRows(false);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        private void grdCuentasSituacionFinanciera_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {
            switch (e.Cell.Column.Key)
            {
                case "v_NroCuenta":
                    frmPlanCuentasConsulta f = new frmPlanCuentasConsulta("");
                    f.ShowDialog();
                    if (f._NroSubCuenta != null)
                    {
                        if (grdCuentasSituacionFinanciera.Rows.ToList().Any(g => g.Cells["v_Nrocuenta"].Value != null && g.Cells["v_Nrocuenta"].Value.ToString().Equals(f._NroSubCuenta)))
                        {
                        }
                        else
                        {
                            var Fila = grdCuentasSituacionFinanciera.ActiveRow;
                            Fila.Cells["v_NroCuenta"].Value = f._NroSubCuenta;
                            Fila.Cells["v_NombreCuenta"].Value = f._NombreCuenta;
                        }
                    }
                    break;

            }
        }

        private void grdCuentasFuncion_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {
            switch (e.Cell.Column.Key)
            {
                case "v_NroCuenta":
                    frmPlanCuentasConsulta f = new frmPlanCuentasConsulta("");
                    f.ShowDialog();
                    if (f._NroSubCuenta != null)
                    {
                        if (grdCuentasFuncion.Rows.ToList().Any(g => g.Cells["v_Nrocuenta"].Value != null && g.Cells["v_Nrocuenta"].Value.ToString().Equals(f._NroSubCuenta)))
                        {
                        }
                        else
                        {
                            var Fila = grdCuentasFuncion.ActiveRow;
                            Fila.Cells["v_NroCuenta"].Value = f._NroSubCuenta;
                            Fila.Cells["v_NombreCuenta"].Value = f._NombreCuenta;
                        }
                    }
                    break;

            }
        }

        private void btnEliminarSituacionFinanciera_Click(object sender, EventArgs e)
        {

            if (grdVisualizarSituacionFinanciera.ActiveRow == null) return;
            OperationResult objOperationResult = new OperationResult();

            if (UltraMessageBox.Show("¿Seguro de Eliminar este Registro?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {

                _objBL.ConfiguracionBalanceEliminar(ref objOperationResult, int.Parse(grdVisualizarSituacionFinanciera.Rows[grdVisualizarSituacionFinanciera.ActiveRow.Index].Cells["i_IdConfiguracionBalance"].Value.ToString()), Globals.ClientSession.GetAsList(), (int)ConfiguracionBalances.SituacionFinanciera);

                if (objOperationResult.Success == 1)
                {
                    BindGridSituacionFinanciera(cbMeses.Value.ToString());
                }
                else
                {
                    UltraMessageBox.Show(Constants.GenericErrorMessage, "Ocurrió un error al cargar Eliminar Configuración", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnCancelarSituacionFinanciera_Click(object sender, EventArgs e)
        {
          
            HabilitarBotonesSituacionFinanciera(true, true, true, true, true, "");
            BindGridSituacionFinanciera(cbMeses.Value.ToString ());
            
        }

        private void btnNuevoBalanceNaturaleza_Click(object sender, EventArgs e)
        {

            if (grdVisualizarBalanceNaturaleza.ActiveRow == null)
            {
            }
            else
            {

                string Codigo = grdVisualizarBalanceNaturaleza.Rows[grdVisualizarBalanceNaturaleza.ActiveRow.Index].Cells["v_Codigo"].Value.ToString();
                _Mode = "New";
                CargarGrillaBalanceNaturaleza("");

                btnGuardarBalanceNaturaleza.Enabled = true;
                txtCodigoBalanceNaturaleza.Focus();
                LimpiarBalanceNaturaleza(Codigo);
                
                HabilitarBotonesBalanceFuncion(true, false, true, false, true, "");
            }
        }

        private void btnEliminarBalanceNaturaleza_Click(object sender, EventArgs e)
        {
            if (grdVisualizarBalanceNaturaleza.ActiveRow == null) return;
            OperationResult objOperationResult = new OperationResult();

            if (UltraMessageBox.Show("¿Seguro de Eliminar este Registro?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {

                _objBL.ConfiguracionBalanceEliminar(ref objOperationResult, int.Parse(grdVisualizarBalanceNaturaleza.Rows[grdVisualizarBalanceNaturaleza.ActiveRow.Index].Cells["i_IdConfiguracionBalance"].Value.ToString()), Globals.ClientSession.GetAsList(), (int)ConfiguracionBalances.EstadodeResultadosNaturaleza);

                if (objOperationResult.Success == 1)
                {
                    BindGridBalanceNaturaleza();
                }
                else
                {
                    UltraMessageBox.Show(Constants.GenericErrorMessage, "Ocurrió un error al cargar Eliminar Configuración", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnCancelarBalanceNaturaleza_Click(object sender, EventArgs e)
        {
            try
            {

                var objOperationResult = new OperationResult();
                foreach (var row in grdCuentasNaturaleza.Rows)
                {
                    row.Selected = true;

                }
                grdCuentasNaturaleza.DeleteSelectedRows(false);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAgregarBalanceNaturaleza_Click(object sender, EventArgs e)
        {
            UltraGridRow row = grdCuentasNaturaleza.DisplayLayout.Bands[0].AddNew();
            grdCuentasNaturaleza.Rows.Move(row, grdCuentasNaturaleza.Rows.Count() - 1);
            this.grdCuentasNaturaleza.ActiveRowScrollRegion.ScrollRowIntoView(row);

            UltraGridCell aCell = this.grdCuentasNaturaleza.ActiveRow.Cells["v_NroCuenta"];
            this.grdCuentasNaturaleza.ActiveCell = aCell;
            grdCuentasNaturaleza.ActiveRow = aCell.Row;
            grdCuentasNaturaleza.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
            grdCuentasNaturaleza.PerformAction(UltraGridAction.EnterEditMode, false, false);
        }

        private void btnEliminarCuentaBalanceNaturaleza_Click(object sender, EventArgs e)
        {
            if (grdCuentasNaturaleza.ActiveRow == null) return;
            if (UltraMessageBox.Show("¿Seguro de Eliminar este Registro?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                grdCuentasNaturaleza.Rows[grdCuentasNaturaleza.ActiveRow.Index].Delete(false);
            }
        }

        private void btnRangoBalanceNaturaleza_Click(object sender, EventArgs e)
        {
         
        }

        private void btnLimpiarBalanceNaturaleza_Click(object sender, EventArgs e)
        {
            try
            {

                var objOperationResult = new OperationResult();
                foreach (var row in grdCuentasNaturaleza.Rows)
                {
                    row.Selected = true;

                }
                grdCuentasNaturaleza.DeleteSelectedRows(false);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnGuardarBalanceNaturaleza_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            int i_IdBalanceFuncion = 0;
            LLenarTemporalesCuentasEstadoResultadosNaturaleza();
            if (_Mode == "New")
            {
                _objConfiguracionBalanceDto = new configuracionbalancesDto
                {
                    v_Codigo = txtCodigoBalanceNaturaleza.Text,
                    v_Nombre = txtNombreBalanceNaturaleza.Text,
                    v_TipoBalance = ((int)ConfiguracionBalances.EstadodeResultadosNaturaleza).ToString(),
                    v_NombreGrupo = txtNombreTotalBalanceNaturaleza.Text,
                    //i_TipoOperacion = int.Parse(cboTipoOperacionBalanceNaturaleza.Value.ToString()),
                };

                i_IdBalanceFuncion = _objBL.ConfiguracionBalanceNuevo(ref objOperationResult, _objConfiguracionBalanceDto, Globals.ClientSession.GetAsList(),  CuentasAsociadasEstadoResultadosNaturaleza  , (int)ConfiguracionBalances.EstadodeResultadosNaturaleza);
            }
            else if (_Mode == "Edit")
            {
                _objConfiguracionBalanceDto.v_Codigo = txtCodigoBalanceNaturaleza.Text;
                _objConfiguracionBalanceDto.v_Nombre = txtNombreBalanceNaturaleza.Text;
                _objConfiguracionBalanceDto.v_NombreGrupo = txtNombreTotalBalanceNaturaleza.Text;
               // _objConfiguracionBalanceDto.i_TipoOperacion = int.Parse(cboTipoOperacionBalanceFuncion.Value.ToString());
                i_IdBalanceFuncion = _objBL.ConfiguracionBalanceActualizar(ref objOperationResult, _objConfiguracionBalanceDto, Globals.ClientSession.GetAsList(), CuentasAsociadasEstadoResultadosNaturaleza, (int)ConfiguracionBalances.EstadodeResultadosNaturaleza);
            }

            if (objOperationResult.Success == 1)  // Operación sin error
            {

                _Mode = "Edit";
                CuentasAsociadasEstadoResultadosNaturaleza = new List<string>();
                UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                BindGridBalanceNaturaleza();
                CargarGrillaBalanceNaturaleza(txtCodigoBalanceNaturaleza.Text);

                MantenerSeleccionNaturaleza(i_IdBalanceFuncion);
            }
            else
            {
                UltraMessageBox.Show(MensajeGuardar, "ERROR!!!-", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void grdVisualizarBalanceNaturaleza_AfterRowActivate(object sender, EventArgs e)
        {
            if (grdVisualizarBalanceNaturaleza.ActiveRow != null)
            {
                OperationResult objOperationResult = new OperationResult();
                using (new LoadingClass.PleaseWait(this.Location, "Por favor espere..."))
                {
                    _Mode = "Edit";

                    CuentasAsociadasEstadoResultadosNaturaleza = new List<string>();
                    int i_IdConfiguracionBalance = int.Parse(grdVisualizarBalanceNaturaleza.Rows[grdVisualizarBalanceNaturaleza.ActiveRow.Index].Cells["i_IdConfiguracionBalance"].Value.ToString());
                    _objConfiguracionBalanceDto = new configuracionbalancesDto();
                    _objConfiguracionBalanceDto = _objBL.ObtenerCabeceraBalance(ref objOperationResult, i_IdConfiguracionBalance, ((int)ConfiguracionBalances.EstadodeResultadosNaturaleza).ToString());
                    if (objOperationResult.Success == 1)
                    {
                        txtCodigoBalanceNaturaleza.Text = _objConfiguracionBalanceDto.v_Codigo;
                        txtNombreBalanceNaturaleza.Text = _objConfiguracionBalanceDto.v_Nombre;
                        //cboTipoOperacionBalanceNaturaleza.Value = _objConfiguracionBalanceDto.i_TipoOperacion == null ? "-1" : _objConfiguracionBalanceDto.i_TipoOperacion.Value.ToString();
                        txtNombreTotalBalanceNaturaleza.Text = _objConfiguracionBalanceDto.v_NombreGrupo;
                        CargarDetalleBalanceNaturaleza(_objConfiguracionBalanceDto.v_Codigo);
                        HabilitarBotonesBalanceNaturaleza(true, true, true, true, true, txtCodigoBalanceNaturaleza.Text);

                    }
                    else
                    {
                        UltraMessageBox.Show(Constants.GenericErrorMessage, "Ocurrió un error al cargar Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                }
            }
        }

        private void cbMeses_ValueChanged(object sender, EventArgs e)
        {
            BindGridSituacionFinanciera(cbMeses.Value.ToString());
        }

        private void btnCopiarSituacionFinanciera_Click(object sender, EventArgs e)
        {
            frmCopiarConfiguracionBalance frm = new frmCopiarConfiguracionBalance();
            frm.ShowDialog();
        }
    }
}

