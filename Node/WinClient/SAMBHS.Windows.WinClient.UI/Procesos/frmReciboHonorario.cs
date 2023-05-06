using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.Resource;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Contabilidad.BL;
using SAMBHS.Compra.BL;
using System.Globalization;
using Infragistics.Win.UltraWinMaskedEdit;
using SAMBHS.Security.BL;
using LoadingClass;
using SAMBHS.Venta.BL;

using SAMBHS.Tesoreria.BL;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmReciboHonorario : Form
    {


        public DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        UltraCombo ucCosto = new UltraCombo();
        string _Mode;
        string strModo = "Nuevo", strIdReciboHonorario;
        public string _pstrIdMovimiento_Nuevo, newIdRecibo, IdReciboHonorarioUtilizDocRef;
        ReciboHonorarioBL _objReciboHonorariosBL = new ReciboHonorarioBL();
        DiarioBL _objDiarioBL = new DiarioBL();
        List<KeyValueDTO> _ListadoReciboHonorarios = new List<KeyValueDTO>();
        List<KeyValueDTO> _ListadoReciboHonorariosCambioFecha = new List<KeyValueDTO>();
        recibohonorarioDto _recibohonorarioDto = new recibohonorarioDto();
        ComprasBL _objComprasBL = new ComprasBL();
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        CierreMensualBL _objCierreMensualBL = new CierreMensualBL();
        AsientosContablesBL _objAsientoBL = new AsientosContablesBL();
        recibohonorariodetalleDto _objrecibohonorariodetalleDto = new recibohonorariodetalleDto();
        List<GridKeyValueDTO> _ListadoComboDocumentos = new List<GridKeyValueDTO>();
        SecurityBL _obSecurityBL = new SecurityBL();
        ClienteBL _objClienteBL = new ClienteBL();
        public decimal cuartacat;//Inicializarlos en nuevo
        int _MaxV, _ActV, Scroll = 0;
        bool _btnGuardar, Edicion = false;
        bool EsDocRef = false;
        #region Temporales DetalleReciboHonorario
        List<recibohonorariodetalleDto> _TempDetalle_AgregarDto = new List<recibohonorariodetalleDto>();
        List<recibohonorariodetalleDto> _TempDetalle_ModificarDto = new List<recibohonorariodetalleDto>();
        List<recibohonorariodetalleDto> _TempDetalle_EliminarDto = new List<recibohonorariodetalleDto>();
        #endregion

        #region Evntos de navegacion
        public string IdRecibido
        {
            set { _idRecibido = value; }
        }
        private string _idRecibido;
        public delegate void OnSiguienteAnterior();
        public event OnSiguienteAnterior OnSiguiente;
        public event OnSiguienteAnterior OnAnterior;

        #endregion
        public frmReciboHonorario(string Modo, string IdReciboHonorario)
        {
            InitializeComponent();
            strModo = Modo;// 27-11
            strIdReciboHonorario = IdReciboHonorario; //27-11
        }
        private void frmReciboHonorario_Nuevo_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            panel1.BackColor = new GlobalFormColors().BannerColor;
           // linkAsiento.BackColor = ultraStatusBar1.Appearance.BackColor;
            OperationResult objOperationResult = new OperationResult();
            txtPeriodo.Text = Globals.ClientSession.i_Periodo.ToString();
            txtMes.Text = DateTime.Now.Month.ToString();
            Utils.Windows.FijarFormatoUltraTextBox(txtMes, "{0:00}");
            #region ControlAcciones
            if (_objCierreMensualBL.VerificarMesCerrado(Globals.ClientSession.i_Periodo.ToString().Trim(), DateTime.Now.Month.ToString("00").Trim(), (int)ModulosSistema.Compras))
            {

                btnGuardar.Visible = false;
                this.Text = "Recibo por honorarios [MES CERRADO]";
            }

            else
            {
                btnGuardar.Visible = true;
                this.Text = "Recibo por honorarios";
            }
            var _formActions = _obSecurityBL.GetFormAction(ref objOperationResult, Globals.ClientSession.i_CurrentExecutionNodeId, Globals.ClientSession.i_SystemUserId, "frmReciboHonoarios", Globals.ClientSession.i_RoleId);
            _btnGuardar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmReciboHonoarios_Save", _formActions);
            btnGuardar.Enabled = _btnGuardar;
            #endregion
            CargarCombos();
            ObtenerListadoReciboHonorarios(txtPeriodo.Text, txtMes.Text);
            ValidarFechas();
            Scroll = int.Parse(dtpFechaEmision.Value.Month.ToString());
            btnEliminar.Enabled = false;
            if (Application.OpenForms["LoadingForm"] != null) ((LoadingForm)Application.OpenForms["LoadingForm"]).CloseWindow();
            if (Application.OpenForms["frmMaster"] != null) ((frmMaster)Application.OpenForms["frmMaster"]).Activate();

        }

        private void CargarCombos()
        {


            OperationResult objOperationResult = new OperationResult();
            _ListadoComboDocumentos = _objReciboHonorariosBL.ObtenerDocumentosParaComboGridReciboHonorarios(ref objOperationResult, null);
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            Utils.Windows.LoadDropDownList(cboIGV, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 27, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboEstado, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 30, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(cboDocumento, "Value2", "Id", _ListadoComboDocumentos, DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(cboDocumentoRef, "Value2", "Id", _ListadoComboDocumentos.Where(x => int.Parse(x.Id.ToString()) != (int)TiposDocumentos.NotaCredito).ToList(), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboCuartaCategoria, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 32, null), DropDownListAction.Select);
            cboDocumento.Value = "2";
            cboDocumentoRef.Value = "-1";

        }

        private void ObtenerListadoReciboHonorarios(string pstrPeriodo, string pstrMes)
        {
            OperationResult objOperationResult = new OperationResult();
            _ListadoReciboHonorarios = _objReciboHonorariosBL.ObtenerListadoReciboHonorarios(ref objOperationResult, pstrPeriodo, pstrMes);
            switch (strModo)
            {
                case "Edicion":

                    EdicionBarraNavegacion(false);
                    Edicion = true;
                    CargarCabecera(strIdReciboHonorario);
                    Edicion = false;
                    btnRedondear.Enabled = true;
                    linkAsiento.Visible = true;
                    //cboDocumento.Enabled = false;
                    //txtSerieDoc.Enabled = false;
                    //txtCorrelativoDoc.Enabled = false;
                    break;

                case "Nuevo":
                    if (_ListadoReciboHonorarios.Count != 0)
                    {
                        _MaxV = _ListadoReciboHonorarios.Count() - 1;
                        _ActV = _MaxV;
                        LimpiarCabecera();
                        CargarDetalle("", 1); //Le mando 1 solo acá porque es nuevo
                       // txtCorrelativo.Text = (int.Parse(_ListadoReciboHonorarios[_MaxV].Value1) + 1).ToString("00000000");
                        _Mode = "New";
                        _recibohonorarioDto = new recibohonorarioDto();
                        EdicionBarraNavegacion(true);
                    }
                    else
                    {
                        txtCorrelativo.Text = "00000001";
                        _Mode = "New";
                        LimpiarCabecera();
                        CargarDetalle("", 1);
                        _MaxV = 1;
                        _ActV = 1;
                        _recibohonorarioDto = new recibohonorarioDto();
                        btnNuevoMovimiento.Enabled = false;
                        EdicionBarraNavegacion(true);

                    }
                    txtCorrelativo.Text = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult, ListaProcesos.ReciboHonoarios, _recibohonorarioDto.t_FechaRegistro, dtpFechaRegistro.Value, _recibohonorarioDto.v_Correlativo, 0);
                    linkAsiento.Visible = false;
                    txtMes.Enabled = false;
                    break;

                case "Guardado":

                    Edicion = true;
                    _MaxV = _ListadoReciboHonorarios.Count() - 1;
                    _ActV = _MaxV;
                    if (strIdReciboHonorario == "" | strIdReciboHonorario == null)
                    {
                        CargarCabecera(_ListadoReciboHonorarios[_MaxV].Value2);
                    }
                    else
                    {
                        CargarCabecera(strIdReciboHonorario);
                    }
                    btnNuevoMovimiento.Enabled = true;
                    txtMes.Enabled = false;
                    //txtCorrelativo.Enabled = false;
                    Edicion = false;
                    linkAsiento.Visible = true;
                    break;

                case "Consulta":
                    EdicionBarraNavegacion(true);
                    if (_ListadoReciboHonorarios.Count != 0)
                    {
                        _MaxV = _ListadoReciboHonorarios.Count() - 1;
                        _ActV = _MaxV;
                        txtCorrelativo.Text = (int.Parse(_ListadoReciboHonorarios[_MaxV].Value1)).ToString("00000000");
                        CargarCabecera(_ListadoReciboHonorarios[_MaxV].Value2);
                        _Mode = "Edit";
                        EdicionBarraNavegacion(true);
                    }
                    else
                    {
                        txtCorrelativo.Text = "00000001";
                        _Mode = "New";
                        LimpiarCabecera();
                        CargarDetalle("", 0);
                        _MaxV = 1;
                        _ActV = 1;
                        _recibohonorarioDto = new recibohonorarioDto();
                        btnNuevoMovimiento.Enabled = false;
                        _objReciboHonorariosBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaEmision.Value.Date);
                        EdicionBarraNavegacion(false);
                       
                    }
                    txtMes.Enabled = true;
                    linkAsiento.Visible = true;
                    break;
            }
        }

        #region  Grilla

        private void CargarComboDetalle()
        {
            OperationResult objOperationResult = new OperationResult();
            #region Configura Combo Centro Costo
            UltraGridBand ultraGridBanda = new UltraGridBand("Band 0", -1);
            UltraGridColumn ultraGridColumnaID = new UltraGridColumn("Id");
            UltraGridColumn ultraGridColumnaDescripcion = new UltraGridColumn("Value1");
            UltraGridColumn UltraGridColumnValue2 = new UltraGridColumn("Value2");
            ultraGridColumnaDescripcion.Header.Caption = "Descripción";
            ultraGridColumnaDescripcion.Header.VisiblePosition = 0;
            ultraGridColumnaDescripcion.Width = 267;
            ultraGridColumnaID.Hidden = false;
            UltraGridColumnValue2.Hidden = true;
            ultraGridColumnaID.Width = 25;
            ultraGridBanda.Columns.AddRange(new object[] { ultraGridColumnaDescripcion, ultraGridColumnaID, UltraGridColumnValue2 });
            ucCosto.DisplayLayout.BandsSerializer.Add(ultraGridBanda);
            ucCosto.DropDownWidth = 270;
            #endregion
            Utils.Windows.LoadUltraComboList(ucCosto, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForComboGrid(ref objOperationResult, 31, null), DropDownListAction.Select); //Centro Costo -31


        }
        private void btnAgregar_Click(object sender, EventArgs e)
        {

            if (rbSiCuartacategoria.Checked == true && cboCuartaCategoria.Value.ToString() == "-1")
            {
                UltraMessageBox.Show("Por favor seleccione un valor % Renta Cuarta Categoria", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cboCuartaCategoria.Focus();
                return;

            }

            if (cboIGV.SelectedValue.ToString() == "-1")
            {
                UltraMessageBox.Show("Por favor seleccione un valor para el IGV", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cboIGV.Focus();

            }
            if (cboMoneda.Value.ToString() == "-1")
            {
                UltraMessageBox.Show("Por favor seleccione un valor para la moneda", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cboMoneda.Focus();
                return;
            }

            if (decimal.Parse(txtTipoCambio.Text) <= 0)
            {
                UltraMessageBox.Show("Por favor ingrese un valor para el tipo de cambio", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtTipoCambio.Focus();
                return;
            }

            //if (grdData.ActiveRow != null)
            //{
            var ultimaFila = grdData.Rows.LastOrDefault();
            if (ultimaFila == null || (ultimaFila.Cells["v_NroCuenta"].Value != null && _objReciboHonorariosBL.ExistenciaCuentaImputable(ultimaFila.Cells["v_NroCuenta"].Value.ToString())))
            {
                UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
                grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                row.Cells["i_RegistroEstado"].Value = "Agregado";
                row.Cells["i_RegistroTipo"].Value = "Temporal";
                row.Cells["i_CCosto"].Value = "";
                //}
                //else
                //{
                //    UltraMessageBox.Show("Por favor ingrese una cuenta válida", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);

                //}
            }
            else
            {
                UltraMessageBox.Show("Por favor ingrese una cuenta válida", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            grdData.Focus();
            UltraGridCell aCell1 = this.grdData.ActiveRow.Cells["v_NroCuenta"];
            this.grdData.ActiveCell = aCell1;
            grdData.ActiveRow = aCell1.Row;
            grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
            grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);
            strModo = "Editado";
        }
        private void grdData_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.grdData.ActiveCell.Column.Key != "i_CCosto")
            {

                switch (e.KeyCode)
                {
                    case Keys.Up:
                        grdData.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        grdData.PerformAction(UltraGridAction.AboveCell, false, false);
                        e.Handled = true;
                        grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                    case Keys.Down:
                        grdData.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        grdData.PerformAction(UltraGridAction.BelowCell, false, false);
                        e.Handled = true;
                        grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                    case Keys.Right:
                        grdData.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        grdData.PerformAction(UltraGridAction.NextCellByTab, false, false);
                        e.Handled = true;
                        grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                    case Keys.Left:
                        grdData.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        grdData.PerformAction(UltraGridAction.PrevCellByTab, false, false);
                        e.Handled = true;
                        grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                    case Keys.Enter:
                        DoubleClickCellEventArgs eventos = new DoubleClickCellEventArgs(grdData.ActiveCell);
                        grdData_DoubleClickCell(sender, eventos);
                        e.Handled = true;
                        break;

                    case Keys.LButton | Keys.Back:
                        e.Handled = false;
                        break;


                }

            }
            else
            {

                switch (e.KeyCode)
                {
                    case Keys.Right:
                        grdData.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        grdData.PerformAction(UltraGridAction.NextCellByTab, false, false);
                        e.Handled = true;
                        grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                    case Keys.Left:
                        grdData.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        grdData.PerformAction(UltraGridAction.PrevCellByTab, false, false);
                        e.Handled = true;
                        grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                }


            }
        }

        private void grdData_MouseDown(object sender, MouseEventArgs e)
        {
            Point point = new System.Drawing.Point(e.X, e.Y);
            Infragistics.Win.UIElement uiElement = ((Infragistics.Win.UltraWinGrid.UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

            if (uiElement == null || uiElement.Parent == null) return;

            Infragistics.Win.UltraWinGrid.UltraGridRow row = (Infragistics.Win.UltraWinGrid.UltraGridRow)uiElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridRow));

            if (row == null)
            {
                btnEliminar.Enabled = false;
            }
            else
            {
                btnEliminar.Enabled = true;
            }
        }
        private void grdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            //e.Layout.Bands[0].Columns["i_CCosto"].EditorComponent = ucCosto;
            //e.Layout.Bands[0].Columns["i_CCosto"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
        }


        private void grdData_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {
            switch (e.Cell.Column.Key)
            {
                case "v_NroCuenta":
                    string CuentaPredeterminada = "60";
                    if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroCuenta"].Text != null && grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroCuenta"].Text != string.Empty)
                    {
                        CuentaPredeterminada = grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroCuenta"].Text.Trim();
                    }

                    
                    Mantenimientos.frmPlanCuentasConsulta frm = new Mantenimientos.frmPlanCuentasConsulta(CuentaPredeterminada);
                    frm.ShowDialog();
                    if (frm._NombreCuenta != null)
                    {
                        grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroCuenta"].Value = frm._NroSubCuenta.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_ValidarCentroCosto"].Value = frm._ValidarCentroCosto;
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";

                    }



                    break;


                case "i_CCosto":

                    if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_ValidarCentroCosto"].Value != null && grdData.Rows[grdData.ActiveRow.Index].Cells["i_ValidarCentroCosto"].Value.ToString() == "1")
                    {
                        Mantenimientos.frmBuscarDatahierarchy frm2 = new Mantenimientos.frmBuscarDatahierarchy(31, "Buscar Centro de Costos");
                        frm2.ShowDialog();
                        if (frm2._itemId != null)
                        {
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_CCosto"].Value = frm2._value2;
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                        }
                    }
                    break;


            }
        }


        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null) return;
            if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroTipo"].Value.ToString() == "NoTemporal")
            {
                if (UltraMessageBox.Show("¿Seguro de Eliminar este Registro?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _objrecibohonorariodetalleDto = new recibohonorariodetalleDto();
                    _objrecibohonorariodetalleDto.v_IdReciboHonorarioDetalle = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdReciboHonorarioDetalle"].Value.ToString();
                    _TempDetalle_EliminarDto.Add(_objrecibohonorariodetalleDto);
                    grdData.Rows[grdData.ActiveRow.Index].Delete(false);

                    if (grdData.Rows.Count != 0)
                    {
                        strModo = "Editado";
                        CalcularValores(0); // se agrego al final
                    }
                    else
                    {
                        //btnRedondear.Enabled = false;
                        //btnRedondear 
                        LimpiarValores();
                    }
                }
            }
            else
            {

                if (UltraMessageBox.Show("¿Seguro de Eliminar este Registro?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    grdData.Rows[grdData.ActiveRow.Index].Delete(false);


                    if (grdData.Rows.Count != 0)
                    {
                        CalcularValores(0); // se agrego al final
                        strModo = "Editado";
                    }
                    else
                    {
                        //btnRedondear.Enabled = false;
                        LimpiarValores();
                    }
                }
            }


        }

        private void grdData_CellChange(object sender, CellEventArgs e)
        {
            grdData.Rows[e.Cell.Row.Index].Cells["i_RegistroEstado"].Value = "Modificado";

        }

        private void grdData_AfterCellActivate(object sender, EventArgs e)
        {

        }

        private void grdData_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (grdData.ActiveCell != null)
            {
                UltraGridCell Celda;
                switch (this.grdData.ActiveCell.Column.Key)
                {
                    case "d_ImporteSoles":
                        Celda = grdData.ActiveCell;
                        Utils.Windows.NumeroDecimalCelda(Celda, e);
                        break;

                    case "d_ImporteDolares":
                        Celda = grdData.ActiveCell;
                        Utils.Windows.NumeroDecimalCelda(Celda, e);
                        break;

                    case "v_NroCuenta":
                        Celda = grdData.ActiveCell;
                        Utils.Windows.NumeroEnteroCelda(Celda, e);
                        break;
                    case "i_CCosto":
                        Celda = grdData.ActiveCell;
                        Utils.Windows.NumeroEnteroCelda(Celda, e);
                        break;
                }
            }
        }

        private void grdData_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            e.Row.Cells["Index"].Value = (e.Row.Index + 1).ToString("00");
        }
        private void grdData_AfterExitEditMode(object sender, EventArgs e)
        {
            strModo = "Edicion";
            CalcularValores(0);
            switch (grdData.ActiveCell.Column.Key)
            {

                case "v_NroCuenta":
                    

                    if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroCuenta"].Value != null && grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroCuenta"].Value.ToString() != string.Empty)
                    {
                        var CuentaIngresada = _objAsientoBL.ObtenerAsientoImputable(grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroCuenta"].Value.ToString());
                        if (CuentaIngresada != null)
                        {
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_ValidarCentroCosto"].Value = CuentaIngresada.i_CentroCosto.Value;
                        }
                        else
                        {
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_ValidarCentroCosto"].Value = 0;
                           // grdData.Rows[grdData.ActiveRow.Index].Cells["i_CCosto"].Value = string.Empty;
                            UltraMessageBox.Show("Por Favor ingrese una cuenta válida ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            grdData.Focus();
                            UltraGridCell aCell1 = this.grdData.ActiveRow.Cells["v_NroCuenta"];
                            this.grdData.ActiveCell = aCell1;
                            grdData.ActiveRow = aCell1.Row;
                            grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                            grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);

                           // return;
                        }

                        if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_ValidarCentroCosto"].Value != null && grdData.Rows[grdData.ActiveRow.Index].Cells["i_ValidarCentroCosto"].Value.ToString() == "0")
                        {
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_CCosto"].Activation = Activation.Disabled;
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_CCosto"].Activation = Activation.NoEdit;


                        }
                        else
                        {
                            
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_CCosto"].Activation = Activation.ActivateOnly;
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_CCosto"].Activation = Activation.AllowEdit;
                        }

                    }
                    else
                    {
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_ValidarCentroCosto"].Value = 0;
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_CCosto"].Value = string.Empty;
                    }

                    break;


            }


        }
        private void CalcularValores(int _Modonuevo)
        {
            if (cboCuartaCategoria.Value == null || cboMoneda.Value == null) return;
            OperationResult objOperationResult = new OperationResult();
            decimal sumaCuartacat = 0, sumaTImporte = 0, porPagar = 0, totalDebe = 0, totalHaber = 0, diferencia = 0, dolares = 0;
            decimal tipocambio = 0, soles = 0;
            tipocambio = (txtTipoCambio.Text.Trim() != "" && txtTipoCambio.Text.Trim() != "0.0000" & txtTipoCambio.Text.Trim() != ".") ? decimal.Parse(txtTipoCambio.Text) : 0;
            cuartacat = cboCuartaCategoria.Value == null ? 0 : cboCuartaCategoria.Value.ToString() != "-1" ? decimal.Parse(cboCuartaCategoria.Text.Trim()) : 0;

            if (_Modonuevo == 1)
            {
                LimpiarValores();

            }
            else
            {
                //if (strModo != "Guardado" & strModo != "Consulta") // Valores de la BD. strModo="Nuevo"
                //{
                foreach (UltraGridRow Fila in grdData.Rows)
                {
                    if (Fila.Cells["d_ImporteSoles"].Value != null)
                    {
                        dolares = 0;
                        switch (cboMoneda.Value.ToString())
                        {


                            case "1":
                                //Soles
                                sumaTImporte = sumaTImporte + decimal.Parse(Fila.Cells["d_ImporteSoles"].Value.ToString());
                                sumaCuartacat = rbSiCuartacategoria.Checked == true ? sumaTImporte * (cuartacat / 100) : 0; //antes el 0 era SumTImporte
                                porPagar = sumaTImporte - sumaCuartacat;
                                totalDebe = sumaTImporte;
                                totalHaber = sumaCuartacat + porPagar;
                                diferencia = totalDebe - totalHaber;
                                dolares = tipocambio != 0 ? decimal.Parse(Fila.Cells["d_ImporteSoles"].Value.ToString()) / tipocambio : 0;
                                Fila.Cells["d_ImporteDolares"].Value = decimal.Round(dolares, 2);

                                break;


                            case "2":
                                //dolares
                                sumaTImporte = sumaTImporte + decimal.Parse(Fila.Cells["d_ImporteDolares"].Value.ToString());
                                sumaCuartacat = rbSiCuartacategoria.Checked == true ? sumaTImporte * (cuartacat / 100) : 0;
                                porPagar = sumaTImporte - sumaCuartacat;
                                totalDebe = sumaTImporte;
                                totalHaber = sumaCuartacat + porPagar;
                                diferencia = totalDebe - totalHaber;
                                soles = tipocambio != 0 ? decimal.Parse(Fila.Cells["d_ImporteDolares"].Value.ToString()) * tipocambio : 0;
                                Fila.Cells["d_ImporteSoles"].Value = decimal.Round(soles, 2);

                                break;
                        }
                    }

                    else if (Fila.Cells["d_ImporteDolares"].Value != null)
                    {
                        // OperationResult objOperationResult = new OperationResult();
                        dolares = 0;
                        switch (cboMoneda.Value.ToString())
                        {


                            case "1":
                                //Soles
                                sumaTImporte = sumaTImporte + decimal.Parse(Fila.Cells["d_ImporteSoles"].Value.ToString());
                                sumaCuartacat = rbSiCuartacategoria.Checked == true ? sumaTImporte * (cuartacat / 100) : 0;
                                porPagar = sumaTImporte - sumaCuartacat;
                                totalDebe = sumaTImporte;
                                totalHaber = sumaCuartacat + porPagar;
                                diferencia = totalDebe - totalHaber;
                                dolares = tipocambio != 0 ? decimal.Parse(Fila.Cells["d_ImporteSoles"].Value.ToString()) / tipocambio : 0;
                                Fila.Cells["d_ImporteDolares"].Value = decimal.Round(dolares, 2);

                                break;


                            case "2":
                                //dolares
                                sumaTImporte = sumaTImporte + decimal.Parse(Fila.Cells["d_ImporteDolares"].Value.ToString());
                                sumaCuartacat = rbSiCuartacategoria.Checked == true ? sumaTImporte * (cuartacat / 100) : 0;
                                porPagar = sumaTImporte - sumaCuartacat;
                                totalDebe = sumaTImporte;
                                totalHaber = sumaCuartacat + porPagar;
                                diferencia = totalDebe - totalHaber;
                                soles = tipocambio != 0 ? decimal.Parse(Fila.Cells["d_ImporteDolares"].Value.ToString()) * tipocambio : 0;
                                Fila.Cells["d_ImporteSoles"].Value = decimal.Round(soles, 2);

                                break;
                        }
                    }


                }

                switch (cboMoneda.Value.ToString())
                {

                    case "1":

                        txtImporte.Text = sumaTImporte==0 ?"0.00" : Utils.Windows.DevuelveValorRedondeado ( sumaTImporte,2).ToString("0.00");
                        txtCuartacategoria.Text = sumaCuartacat==0?"0.00": Utils.Windows.DevuelveValorRedondeado(sumaCuartacat, 2).ToString("0.00");
                        txtPagar.Text =porPagar==0?"0.00":  Utils.Windows.DevuelveValorRedondeado ( porPagar,2).ToString("0.00");
                        txtDebe.Text = totalDebe == 0 ? "0.00" : Utils.Windows.DevuelveValorRedondeado(totalDebe, 2).ToString("0.00");
                        txtHaber.Text = totalHaber == 0 ? "0.00" : Utils.Windows.DevuelveValorRedondeado(totalHaber, 2).ToString("0.00");
                        txtDiferencia.Text = diferencia == 0 ? "0.00" : Utils.Windows.DevuelveValorRedondeado(diferencia, 2).ToString("0.00");
                        txtDebeDolares.Text = tipocambio != 0 && sumaTImporte > 0 ? Utils.Windows.DevuelveValorRedondeado((sumaTImporte / tipocambio), 2).ToString("0.00") : "0.00";
                       // txtDebeDolares.Text = tipocambio != 0 ? (sumaTImporte / tipocambio).ToString("0.00") : "0.00"; Antes 20 Mayo estaba con tostring
                        txtHaberDolares.Text = tipocambio != 0 && totalHaber > 0 ? Utils.Windows.DevuelveValorRedondeado((totalHaber / tipocambio), 2).ToString("0.00") : "0.00";
                        txtDiferenciaDolares.Text = tipocambio != 0 && diferencia > 0 ? Utils.Windows.DevuelveValorRedondeado((diferencia / tipocambio), 2).ToString("0.00") : "0.00";

                        if (txtDiferencia.Text != "0.00" || txtDiferenciaDolares.Text != "0.00")
                        {
                            btnRedondear.Enabled = true;
                        }
                        else
                        {
                            btnRedondear.Enabled = false;
                        }

                        break;

                    case "2":
                      //  txtDebe.Text = tipocambio != 0 ? (sumaTImporte * tipocambio).ToString("0.00") : "0.00";Antes 20 Mayo estaba asi
                        txtImporte.Text = sumaTImporte == 0 ? "0.00" : Utils.Windows.DevuelveValorRedondeado(sumaTImporte, 2).ToString("0.00");
                        txtCuartacategoria.Text = sumaCuartacat == 0 ? "0.00" : Utils.Windows.DevuelveValorRedondeado(sumaCuartacat, 2).ToString("0.00");
                        txtPagar.Text = porPagar == 0 ? "0.00" : Utils.Windows.DevuelveValorRedondeado(porPagar, 2).ToString("0.00");
                        txtDebe.Text = tipocambio != 0 && sumaTImporte > 0 ? Utils.Windows.DevuelveValorRedondeado((sumaTImporte * tipocambio), 2).ToString("0.00") : "0.00";
                        txtHaber.Text = tipocambio != 0 && totalHaber > 0 ? Utils.Windows.DevuelveValorRedondeado((totalHaber * tipocambio), 2).ToString("0.00") : "0.00";
                        txtDiferencia.Text = tipocambio != 0 && diferencia > 0 ? Utils.Windows.DevuelveValorRedondeado((diferencia * tipocambio), 2).ToString("0.00") : "0.00";
                        txtDebeDolares.Text = sumaTImporte == 0 ? "0.00" : Utils.Windows.DevuelveValorRedondeado(sumaTImporte, 2).ToString("0.00");
                        txtHaberDolares.Text = totalHaber == 0 ? "0.00" : Utils.Windows.DevuelveValorRedondeado(totalHaber, 2).ToString("0.00");
                        txtDiferenciaDolares.Text = diferencia == 0 ? "0.00" : Utils.Windows.DevuelveValorRedondeado(diferencia, 2).ToString("0.00");
                        if (txtDiferencia.Text != "0.00" || txtDiferenciaDolares.Text != "0.00")
                        {
                            btnRedondear.Enabled = true;
                        }
                        else
                        {
                            btnRedondear.Enabled = false;
                        }

                        break;
                }
            } 

        }
        private void btnRedondear_Click(object sender, EventArgs e)
        {
            if (txtDiferencia.Text != "0.00")
            {


                txtHaber.Text = decimal.Parse(Math.Round(decimal.Parse(txtHaber.Text.ToString()), 1, MidpointRounding.AwayFromZero).ToString("0.00")).ToString("0.00");
                txtDebe.Text = decimal.Parse(Math.Round(decimal.Parse(txtDebe.Text.ToString()), 1, MidpointRounding.AwayFromZero).ToString("0.00")).ToString("0.00");
                txtDiferencia.Text = (decimal.Parse(txtDebe.Text) - decimal.Parse(txtHaber.Text)).ToString("0.00");

               


            }
            else if (txtDiferenciaDolares.Text != "0.00")
            {

                txtHaberDolares.Text = decimal.Parse(Math.Round(decimal.Parse(txtHaberDolares.Text.ToString()), 1, MidpointRounding.AwayFromZero).ToString("0.00")).ToString("0.00");
                txtDebeDolares.Text = decimal.Parse(Math.Round(decimal.Parse(txtDebeDolares.Text.ToString()), 1, MidpointRounding.AwayFromZero).ToString("0.00")).ToString("0.00");
                txtDiferenciaDolares.Text = (decimal.Parse(txtDebeDolares.Text) - decimal.Parse(txtHaberDolares.Text)).ToString("0.00");
                
               
            }
            else
            {

                UltraMessageBox.Show("No es necesario redondear", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }
        #endregion

        #region Cabecera-Detalle
        private void EdicionBarraNavegacion(bool ON_OFF)
        {
            //txtCorrelativo.Enabled = ON_OFF;
            btnAnterior.Enabled = ON_OFF;
            btnSiguiente.Enabled = ON_OFF;
            txtMes.Enabled = ON_OFF;
            btnNuevoMovimiento.Enabled = ON_OFF;
        }
        private void LimpiarCabecera()
        {
            OperationResult objOperationResult = new OperationResult();
            txtTipoCambio.Text = _objReciboHonorariosBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaEmision.Value.Date);
            txtGlosa.Clear();
            txtCodigoProveedor.Clear();
            txtSerieDoc.Clear();
            txtCorrelativoDoc.Clear();
            txtRazonSocial.Clear();
            txtRucProveedor.Clear();
            cboMoneda.Value = Globals.ClientSession.i_IdMonedaCompra.ToString();
            cboEstado.Value = "-1";
            cboIGV.SelectedValue = Globals.ClientSession.i_IdIgv.ToString();
            txtImporte.Clear();
            txtCuartacategoria.Clear();
            txtPagar.Clear();
            txtDebeDolares.Clear();
            txtDebe.Clear();
            txtHaber.Clear();
            txtHaberDolares.Clear();
            txtDiferencia.Clear();
            txtDiferenciaDolares.Clear();
            rbSiCuartacategoria.Checked = true;
            rbNoCuartacategoria.Checked = false;
            cboEstado.Value = "1";
            btnRedondear.Enabled = false;
            dtpFechaRegistro.Value = DateTime.Parse((Globals.ClientSession.i_Periodo + "/" + DateTime.Today.Month + "/" + DateTime.Now.Day)); // DateTime.Now.Date;
            dtpFechaEmision.Value = DateTime.Parse((Globals.ClientSession.i_Periodo + "/" + DateTime.Today.Month + "/" + DateTime.Now.Day));// DateTime.Now.Date;


        }
        private void CargarDetalle(string pstringIdReciboHonorario, int nuevo)
        {
            OperationResult objOperationResult = new OperationResult();
            grdData.DataSource = _objReciboHonorariosBL.ObtenerReciboHonorariosDetalles(ref objOperationResult, pstringIdReciboHonorario);
            for (int i = 0; i < grdData.Rows.Count(); i++)
            {
                if (EsDocRef)
                {
                    grdData.Rows[i].Cells["i_RegistroTipo"].Value = "Temporal";
                    grdData.Rows[i].Cells["i_RegistroEstado"].Value = "Modificado";

                }
                else
                {

                    grdData.Rows[i].Cells["i_RegistroTipo"].Value = "NoTemporal";
                }

                if (grdData.Rows[i].Cells["i_ValidarCentroCosto"].Value != null && grdData.Rows[i].Cells["i_ValidarCentroCosto"].Value.ToString() == "0")
                {
                    grdData.Rows[i].Cells["i_CCosto"].Activation = Activation.Disabled;
                    grdData.Rows[i].Cells["i_CCosto"].Activation = Activation.NoEdit;


                }
                else
                {

                    grdData.Rows[i].Cells["i_CCosto"].Activation = Activation.ActivateOnly;
                    grdData.Rows[i].Cells["i_CCosto"].Activation = Activation.AllowEdit;
                }

            }

            if (EsDocRef)
            {
                _recibohonorarioDto.v_IdReciboHonorario = null;
            }
            //  CalcularValores(nuevo);
        }
        private void CargarCabecera(string idmovimiento)
        {
            OperationResult objOperationResult = new OperationResult();
            _recibohonorarioDto = new recibohonorarioDto();
            _recibohonorarioDto = _objReciboHonorariosBL.ObtenerReciboHonorarioCabecera(ref objOperationResult, idmovimiento);

            if (_recibohonorarioDto != null)
            {
                if (EsDocRef)
                {
                    _Mode = "New";
                }
                else
                {
                    _Mode = "Edit";
                    cboDocumento.Value = _recibohonorarioDto.i_IdTipoDocumento.ToString();
                    txtSerieDoc.Text = _recibohonorarioDto.v_SerieDocumento;
                    txtCorrelativoDoc.Text = _recibohonorarioDto.v_CorrelativoDocumento;
                    txtSerieDocRef.Text = _recibohonorarioDto.v_SerieDocumentoRef;
                    txtCorrelativoDocRef.Text = _recibohonorarioDto.v_CorrelativoDocumentoRef;
                    cboDocumentoRef.Value = _recibohonorarioDto.i_IdDocumentoReferencia == null ? -1 : int.Parse(_recibohonorarioDto.i_IdDocumentoReferencia.Value.ToString());
                    txtCorrelativo.Text = _recibohonorarioDto.v_Correlativo;
                    txtMes.Text = _recibohonorarioDto.v_Mes;
                    txtPeriodo.Text = _recibohonorarioDto.v_Periodo;
                }
                cboMoneda.Value = _recibohonorarioDto.i_IdMoneda.ToString();
                txtGlosa.Text = _recibohonorarioDto.v_Glosa;
                cboEstado.Value = _recibohonorarioDto.i_IdEstado.ToString();
                cboIGV.SelectedValue = _recibohonorarioDto.i_IdIgv.ToString();
                txtCodigoProveedor.Text = _recibohonorarioDto.CodigoProveedor != null ? _recibohonorarioDto.CodigoProveedor.ToString() : string.Empty;

                txtGlosa.Text = _recibohonorarioDto.v_Glosa;
                txtRazonSocial.Text = _recibohonorarioDto.NombreProveedor != null ? _recibohonorarioDto.NombreProveedor.ToString() : string.Empty;
                txtRucProveedor.Text = _recibohonorarioDto.RUCProveedor != null ? _recibohonorarioDto.RUCProveedor.ToString() : string.Empty;
                dtpFechaRegistro.Value = _recibohonorarioDto.t_FechaRegistro.Value;
                dtpFechaEmision.Value = _recibohonorarioDto.t_FechaEmision.Value;


                txtTipoCambio.Text = _recibohonorarioDto.d_TipoCambio.ToString();
                

                if (_recibohonorarioDto.i_IdMoneda == 1) //Soles
                {

                    txtDebeDolares.Text = (Convert.ToDecimal(_recibohonorarioDto.d_TotalDebe) / Convert.ToDecimal(_recibohonorarioDto.d_TipoCambio)).ToString("0.00");
                    txtHaberDolares.Text = (Convert.ToDecimal(_recibohonorarioDto.d_TotalHaber) / Convert.ToDecimal(_recibohonorarioDto.d_TipoCambio)).ToString("0.00");
                    txtDiferenciaDolares.Text = (Convert.ToDecimal(_recibohonorarioDto.d_Diferencia) / Convert.ToDecimal(_recibohonorarioDto.d_TipoCambio)).ToString("0.00");

                }
                else
                {
                    txtDebeDolares.Text = (Convert.ToDecimal(_recibohonorarioDto.d_TotalDebe) / Convert.ToDecimal(_recibohonorarioDto.d_TipoCambio)).ToString("0.00");
                    txtHaberDolares.Text = (Convert.ToDecimal(_recibohonorarioDto.d_TotalHaber) / Convert.ToDecimal(_recibohonorarioDto.d_TipoCambio)).ToString("0.00");
                    txtDiferenciaDolares.Text = (Convert.ToDecimal(_recibohonorarioDto.d_Diferencia) / Convert.ToDecimal(_recibohonorarioDto.d_TipoCambio)).ToString("0.00");

                }
                rbSiCuartacategoria.Checked = _recibohonorarioDto.i_RentaCuartaCategoria == 1 ? true : false;
                rbNoCuartacategoria.Checked = _recibohonorarioDto.i_RentaCuartaCategoria == 0 ? true : false;
                cboCuartaCategoria.Value = _recibohonorarioDto.i_PorcentajeCuartaCategoria.ToString();
                txtCuartacategoria.Text = Convert.ToDecimal(_recibohonorarioDto.d_RentaCuartaCategoria).ToString("0.00");
                txtImporte.Text = Convert.ToDecimal(_recibohonorarioDto.d_Importe).ToString("0.00");
                txtPagar.Text = Convert.ToDecimal(_recibohonorarioDto.d_PorPagar).ToString("0.00");
                txtDebe.Text = Convert.ToDecimal(_recibohonorarioDto.d_TotalDebe).ToString("0.00");
                txtHaber.Text = Convert.ToDecimal(_recibohonorarioDto.d_TotalHaber).ToString("0.00");
                txtDiferencia.Text = Convert.ToDecimal(_recibohonorarioDto.d_Diferencia).ToString("0.00");

                CargarDetalle(_recibohonorarioDto.v_IdReciboHonorario, 0);
            }
            else
            {
                UltraMessageBox.Show("Hubo un error al cargar el recibo por honorario", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void LlenarTemporales()
        {

            if (grdData.Rows.Count() != 0)
            {
                foreach (UltraGridRow Fila in grdData.Rows)
                {
                    string i = Fila.Cells["i_CCosto"].Value.ToString();

                    switch (Fila.Cells["i_RegistroTipo"].Value.ToString())
                    {

                        case "Temporal":
                            if (Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {
                                _objrecibohonorariodetalleDto = new recibohonorariodetalleDto();

                                _objrecibohonorariodetalleDto.v_IdReciboHonorario = _recibohonorarioDto.v_IdReciboHonorario;
                                _objrecibohonorariodetalleDto.v_NroCuenta = Fila.Cells["v_NroCuenta"].Value == null ? null : Fila.Cells["v_NroCuenta"].Value.ToString();
                                _objrecibohonorariodetalleDto.d_ImporteSoles = Fila.Cells["d_ImporteSoles"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ImporteSoles"].Value.ToString());
                                _objrecibohonorariodetalleDto.d_ImporteDolares = Fila.Cells["d_ImporteDolares"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ImporteDolares"].Value.ToString());
                                _objrecibohonorariodetalleDto.i_CCosto = Fila.Cells["i_CCosto"].Value == null ? "" : Fila.Cells["i_CCosto"].Value.ToString().Trim ();

                                _TempDetalle_AgregarDto.Add(_objrecibohonorariodetalleDto);
                            }
                            break;

                        case "NoTemporal":
                            if (Fila.Cells["i_RegistroEstado"].Value != null && Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {
                                _objrecibohonorariodetalleDto = new recibohonorariodetalleDto();
                                _objrecibohonorariodetalleDto.v_IdReciboHonorario = Fila.Cells["v_IdReciboHonorario"].Value == null ? null : Fila.Cells["v_IdReciboHonorario"].Value.ToString();
                                _objrecibohonorariodetalleDto.v_IdReciboHonorarioDetalle = Fila.Cells["v_IdReciboHonorarioDetalle"].Value == null ? null : Fila.Cells["v_IdReciboHonorarioDetalle"].Value.ToString();
                                _objrecibohonorariodetalleDto.v_NroCuenta = Fila.Cells["v_NroCuenta"].Value == null ? null : Fila.Cells["v_NroCuenta"].Value.ToString();
                                _objrecibohonorariodetalleDto.i_CCosto = Fila.Cells["i_CCosto"].Value == null ? "" : Fila.Cells["i_CCosto"].Value.ToString().Trim ();
                                _objrecibohonorariodetalleDto.d_ImporteSoles = Fila.Cells["d_ImporteSoles"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ImporteSoles"].Value.ToString());
                                _objrecibohonorariodetalleDto.d_ImporteDolares = Fila.Cells["d_ImporteDolares"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ImporteDolares"].Value.ToString());
                                _objrecibohonorariodetalleDto.i_Eliminado = int.Parse(Fila.Cells["i_Eliminado"].Value.ToString());
                                _objrecibohonorariodetalleDto.i_InsertaIdUsuario = int.Parse(Fila.Cells["i_InsertaIdUsuario"].Value.ToString());
                                _objrecibohonorariodetalleDto.t_InsertaFecha = Convert.ToDateTime(Fila.Cells["t_InsertaFecha"].Value);
                                _TempDetalle_ModificarDto.Add(_objrecibohonorariodetalleDto);
                            }
                            break;
                    }
                }
            }

        }
        private void btnBuscarCliente_Click(object sender, EventArgs e)
        {
            Mantenimientos.frmBuscarProveedor frm = new Mantenimientos.frmBuscarProveedor(txtRucProveedor.Text, "RUC", "FILTRARSOLORUCEMPIEZA10");
            frm.ShowDialog();
            if (frm._IdProveedor != null)
            {
                txtCodigoProveedor.Text = frm._CodigoProveedor;
                txtRazonSocial.Text = frm._RazonSocial;
                _recibohonorarioDto.v_IdProveedor = frm._IdProveedor;
                txtRucProveedor.Text = frm._NroDocumento;
                ValidarNumeroRegistro();


            }
        }

        private void LimpiarValores()
        {
            decimal sumaCuartacat = 0, sumaTImporte = 0, porPagar = 0, totalDebe = 0, totalHaber = 0, diferencia = 0;
            txtImporte.Text = sumaTImporte.ToString("0.00");
            txtCuartacategoria.Text = sumaCuartacat.ToString("0.00");
            txtPagar.Text = porPagar.ToString("0.00");
            txtDebe.Text = totalDebe.ToString("0.00");
            txtHaber.Text = totalHaber.ToString("0.00");
            txtDiferencia.Text = diferencia.ToString("0.00");
            txtDebeDolares.Text = totalDebe.ToString("0.00");
            txtDiferenciaDolares.Text = diferencia.ToString("0.00");
            txtHaberDolares.Text = totalHaber.ToString("0.00");

        }

        private void ValidarFechas()
        {

            if (DateTime.Now.Year.ToString().Trim() == txtPeriodo.Text.Trim())
            {
                if (strModo == "Nuevo")
                {
                    dtpFechaEmision.Value = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                    dtpFechaEmision.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                    dtpFechaEmision.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(txtMes.Text.Trim()))).ToString()).ToString());
                    dtpFechaRegistro.Value = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                    //dtpFechaRegistro.MinDate = dtpFechaEmision.Value;
                    dtpFechaRegistro.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                    dtpFechaRegistro.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(txtMes.Text.Trim()))).ToString()).ToString());


                }
                else
                {
                    if (int.Parse(_recibohonorarioDto.v_Mes.Trim()) <= int.Parse(DateTime.Now.Month.ToString()))
                    {
                        dtpFechaEmision.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(DateTime.Now.Month.ToString()))).ToString()).ToString());
                        dtpFechaRegistro.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(DateTime.Now.Month.ToString()))).ToString()).ToString());

                    }

                    dtpFechaEmision.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                    dtpFechaRegistro.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());

                }


            }
            else
            {
                if (strModo == "Nuevo")
                {
                    dtpFechaEmision.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                    dtpFechaRegistro.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                }

                dtpFechaEmision.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                dtpFechaEmision.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), 12)).ToString()).ToString());
                //dtpFechaRegistro.MinDate = dtpFechaEmision.Value;
                dtpFechaRegistro.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                dtpFechaRegistro.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), 12)).ToString()).ToString());

            }
        }

        #endregion


        #region Comportamiento-Controles
        private void txtRucProveedor_TextChanged(object sender, EventArgs e)
        {
            if (txtRucProveedor.Text == string.Empty)
            {
                _recibohonorarioDto.v_IdProveedor = null;
                txtCodigoProveedor.Clear();
                txtRazonSocial.Clear();

            }
        }

        private void cboMoneda_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboMoneda.Value.ToString() == "-1")
            {
                LimpiarValores();

            }
            else if (cboMoneda.Value.ToString() == "1")
            {

                grdData.DisplayLayout.Bands[0].Columns["d_ImporteSoles"].CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
                grdData.DisplayLayout.Bands[0].Columns["d_ImporteSoles"].CellActivation = Activation.AllowEdit;
                grdData.DisplayLayout.Bands[0].Columns["d_ImporteDolares"].CellActivation = Activation.NoEdit;
                CalcularValores(0);

            }
            else if (cboMoneda.Value.ToString() == "2")
            {

                grdData.DisplayLayout.Bands[0].Columns["d_ImporteDolares"].CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
                grdData.DisplayLayout.Bands[0].Columns["d_ImporteDolares"].CellActivation = Activation.AllowEdit;
                grdData.DisplayLayout.Bands[0].Columns["d_ImporteSoles"].CellActivation = Activation.NoEdit;
                CalcularValores(0);

            }
        }

        private void dtpFechaRegistro_ValueChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (EsDocRef)
            {
                return;
            }
            //if (strModo == "Nuevo")
            //{
            //    GenerarNumeroRegistro();
            //}

            //else
            //{
            //    string AnioCambiado = dtpFechaRegistro.Value.Year.ToString().Trim();
            //    string MesCambiado = dtpFechaRegistro.Value.Month.ToString("00"); //  int.Parse(dtpFechaRegistro.Value.Month.ToString()) <= 9 ? ("0" + dtpFechaRegistro.Value.Month.ToString()).Trim() : dtpFechaRegistro.Value.Month.ToString();
            //    if (MesCambiado.Trim() != _recibohonorarioDto.v_Mes.Trim() || AnioCambiado != _recibohonorarioDto.v_Periodo.Trim())
            //    {
            //        GenerarNumeroRegistro();

            //    }
            //    else
            //    {
            //        txtPeriodo.Text = _recibohonorarioDto.v_Periodo.Trim();
            //        txtMes.Text = _recibohonorarioDto.v_Mes.Trim();
            //        txtCorrelativo.Text = _recibohonorarioDto.v_Correlativo.Trim();


            //    }
            //}

            txtPeriodo.Text = dtpFechaRegistro.Value.Year.ToString();
            txtMes.Text = dtpFechaRegistro.Value.Month.ToString("00");

            txtCorrelativo.Text = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult, ListaProcesos.ReciboHonoarios, _recibohonorarioDto.t_FechaRegistro, dtpFechaRegistro.Value, _recibohonorarioDto.v_Correlativo, 0);


            if (_objCierreMensualBL.VerificarMesCerrado(txtPeriodo.Text.Trim(), txtMes.Text.Trim(), (int)ModulosSistema.Compras))
            {
                btnGuardar.Visible = false;
                this.Text = "Recibo por honorarios [MES CERRADO]";

            }
            else
            {
                btnGuardar.Visible = true;
                this.Text = "Recibo por honorarios ";
            }

            dtpFechaEmision.Value = dtpFechaRegistro.Value;
        }

        private void txtTipoCambio_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)

                cboMoneda.Focus();
        }

        private void cboIGV_SelectedIndexChanged(object sender, EventArgs e)
        {
            string i = cboIGV.SelectedValue.ToString();

        }
        private void txtSerieDoc_KeyDown(object sender, KeyEventArgs e)
        {


        }

        private void txtCorrelativoDoc_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtTipoCambio_Validated(object sender, EventArgs e)
        {





        }

        private void dtpFechaRegistro_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)

                dtpFechaEmision.Focus();
        }

        private void dtpFechaEmision_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)

                txtTipoCambio.Focus();
        }

        private void txtMes_Leave(object sender, EventArgs e)
        {

            Utils.Windows.FijarFormatoUltraTextBox(txtMes, "{0:00}");
            if (txtMes.Text != "")
            {
                int Mes;
                Mes = int.Parse(txtMes.Text);
                if (Mes >= 1 && Mes <= 12)
                {
                    if (strModo == "Nuevo")
                    {
                        ObtenerListadoReciboHonorarios(txtPeriodo.Text, txtMes.Text);
                    }
                    else if (strModo == "Guardado")
                    {
                        strModo = "Consulta";
                        ObtenerListadoReciboHonorarios(txtPeriodo.Text, txtMes.Text);
                    }
                    else
                    {
                        ObtenerListadoReciboHonorarios(txtPeriodo.Text, txtMes.Text);
                    }

                    txtSerieDoc.Focus();
                }
                else
                {
                    UltraMessageBox.Show("Mes inválido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }

        }

        private void txtMes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Utils.Windows.FijarFormatoUltraTextBox(txtMes, "{0:00}");
                if (txtMes.Text != "")
                {
                    int Mes;
                    Mes = int.Parse(txtMes.Text);
                    if (Mes >= 1 && Mes <= 12)
                    {
                        if (strModo == "Nuevo")
                        {
                            ObtenerListadoReciboHonorarios(txtPeriodo.Text, txtMes.Text);
                        }
                        else if (strModo == "Guardado")
                        {
                            strModo = "Consulta";
                            ObtenerListadoReciboHonorarios(txtPeriodo.Text, txtMes.Text);
                        }
                        else
                        {
                            ObtenerListadoReciboHonorarios(txtPeriodo.Text, txtMes.Text);
                        }

                        txtSerieDoc.Focus();
                    }
                    else
                    {
                        UltraMessageBox.Show("Mes inválido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                }
            }
        }

        private void txtMes_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtMes, e);
        }

        private void rbSiCuartacategoria_CheckedChanged(object sender, EventArgs e)
        {
            if (!Edicion)
            {
                if (rbSiCuartacategoria.Checked == true)
                {
                    cboCuartaCategoria.Enabled = true;
                    cboCuartaCategoria.Value = "2";
                }
                else
                {
                    cboCuartaCategoria.Enabled = false;
                    cboCuartaCategoria.Value = "-1";
                }
                CalcularValores(0);
            }

        }

        private void rbNoCuartacategoria_CheckedChanged(object sender, EventArgs e)
        {


            if (!Edicion)
            {
                if (rbNoCuartacategoria.Checked == true)
                {
                    cboCuartaCategoria.Enabled = false;
                    cboCuartaCategoria.Value = "-1";

                }
                else
                {
                    cboCuartaCategoria.Enabled = true;

                }
                CalcularValores(0);
            }
            else
            {
                if (rbNoCuartacategoria.Checked == true)
                {
                    cboCuartaCategoria.Enabled = false;
                }

            }
        }

        private void txtTipoCambio_TextChanged(object sender, EventArgs e)
        {
            CalcularValores(0);
        }

        private void txtCorrelativoDoc_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCorrelativoDoc, e);
        }

        private void txtCorrelativoDoc_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtCorrelativoDoc, "{0:00000000}");
            ActDesacDocReferencia();
            ValidarNumeroRegistro();

        }

        private void txtSerieDoc_Validated(object sender, EventArgs e)
        {
            if (txtSerieDoc.Text != string.Empty)
            {
                int Leng = txtSerieDoc.Text.Trim().Length, i = 2;
                string CadenaCeros = "0";
                if (Leng < 4)
                {
                    while (i <= (4 - Leng))
                    {
                        CadenaCeros = CadenaCeros + "0";
                        i = i + 1;
                    }
                    txtSerieDoc.Text = CadenaCeros + txtSerieDoc.Text.Trim();
                }
            }


            ActDesacDocReferencia();

        }

        private void txtSerieDoc_KeyPress(object sender, KeyPressEventArgs e)
        {

            e.Handled = !char.IsLetterOrDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(Keys.Tab) && e.KeyChar != Convert.ToChar(Keys.Delete) && e.KeyChar != Convert.ToChar(Keys.Back);
            if (!e.Handled && e.KeyChar == 46)
            {
                e.Handled = true;
            }
        }



        private void txtCorrelativo_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCorrelativo, e);
        }

        private void txtCorrelativo_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtCorrelativo, "{0:00000000}");
        }
        private void txtRucProveedor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (txtRucProveedor.Text.Trim() != "" && txtRucProveedor.TextLength <= 5)
                {
                    Mantenimientos.frmBuscarProveedor frm = new Mantenimientos.frmBuscarProveedor(txtRucProveedor.Text, "RUC");
                    frm.ShowDialog();
                    if (frm._IdProveedor != null)
                    {
                        txtCodigoProveedor.Text = frm._CodigoProveedor;
                        txtRazonSocial.Text = frm._RazonSocial;
                        _recibohonorarioDto.v_IdProveedor = frm._IdProveedor;
                        txtRucProveedor.Text = frm._NroDocumento;
                    }
                }
                else if (txtRucProveedor.TextLength == 11 | txtRucProveedor.TextLength == 8)
                {
                    OperationResult objOperationResult = new OperationResult();
                    string[] DatosProveedor = new string[3];

                    DatosProveedor = _objReciboHonorariosBL.DevolverProveedorPorNroDocumento(ref objOperationResult, txtRucProveedor.Text.Trim());
                    if (DatosProveedor != null)
                    {
                        _recibohonorarioDto.v_IdProveedor = DatosProveedor[0];
                        txtCodigoProveedor.Text = DatosProveedor[1];
                        txtRazonSocial.Text = DatosProveedor[2];
                        dtpFechaRegistro.Focus();
                    }
                    else
                    {
                        Mantenimientos.frmRegistroRapidoCliente frm = new Mantenimientos.frmRegistroRapidoCliente(txtRucProveedor.Text.Trim(), "V");
                        frm.ShowDialog();
                        if (frm._Guardado == true)
                        {
                            txtRucProveedor.Text = frm._NroDocumentoReturn;

                            DatosProveedor = _objReciboHonorariosBL.DevolverProveedorPorNroDocumento(ref objOperationResult, txtRucProveedor.Text.Trim());
                            if (DatosProveedor != null)
                            {
                                _recibohonorarioDto.v_IdProveedor = DatosProveedor[0];
                                txtCodigoProveedor.Text = DatosProveedor[1];
                                txtRazonSocial.Text = DatosProveedor[2];
                                dtpFechaRegistro.Focus();
                            }
                        }
                    }
                }
                else
                {
                    txtCodigoProveedor.Text = string.Empty;
                    txtRazonSocial.Text = string.Empty;
                }
            }
        }

        private void txtRucProveedor_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtRucProveedor, e);
        }

        private void dtpFechaEmision_ValueChanged(object sender, EventArgs e)
        {

            OperationResult objOperationResult = new OperationResult();

            string TipoCambio = _objReciboHonorariosBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaEmision.Value.Date);
            txtTipoCambio.Text = TipoCambio;

        }

        private void txtTipoCambio_ValueChanged(object sender, EventArgs e)
        {
            CalcularValores(0);
        }


        #endregion

        #region CRUD
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();

            if (uvReciboHonorarios.Validate(true, false).IsValid)
            {

                if (int.Parse(cboDocumento.Value.ToString()) == (int)TiposDocumentos.NotaCredito)
                {
                    if (!ValidarNCR.Validate(true, false).IsValid)
                    {
                        return;
                    }
                }

                if (txtSerieDoc.Text.Length < 4)
                {
                    UltraMessageBox.Show("La serie del Documento debe tener  cuatro caractéres", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;

                }
                if (!grdData.Rows.Any())
                {
                    UltraMessageBox.Show("No se permite guardar mientras el detalle está vacío", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (txtTipoCambio.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por Favor ingrese un Tipo de cambio válido ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtTipoCambio.Focus();
                    return;

                }
                else if (decimal.Parse(txtTipoCambio.Text) <= 0)
                {
                    UltraMessageBox.Show("Por Favor ingrese un Tipo de cambio válido ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtTipoCambio.Focus();
                    return;

                }
                if (_recibohonorarioDto.v_IdProveedor == null)
                {
                    UltraMessageBox.Show("Por Favor ingrese un Proveedor ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtRucProveedor.Focus();
                    return;

                }

                if (txtDiferencia.Text.Trim() != "0.00" || txtDiferenciaDolares.Text.Trim() != "0.00")
                {
                    UltraMessageBox.Show("Es necesario realizar un redondeo,antes de Guardar", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;

                }
                if (rbSiCuartacategoria.Checked == true)
                {
                    if (cboCuartaCategoria.Value.ToString() == "-1")
                    {
                        UltraMessageBox.Show("Por favor seleccione un valor % Renta Cuarta Categoria", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                }

                if (txtSerieDoc.Text.Trim() != string.Empty && cboDocumento.Value.ToString() != "-1" && txtCorrelativoDoc.Text.Trim() != string.Empty)
                {

                    if (_objReciboHonorariosBL.ValidarNumeroRegistro(int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text.Trim(), txtCorrelativoDoc.Text.Trim(), _recibohonorarioDto.v_IdReciboHonorario, _recibohonorarioDto.v_IdProveedor, txtPeriodo.Text.Trim()))
                    {
                        UltraMessageBox.Show("Este Documento ya ha sido Registrado para este Proveedor ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtCorrelativoDoc.Focus();
                        return;
                    }

                }

                if (string.IsNullOrEmpty(txtPagar.Text) || decimal.Parse(txtPagar.Text.ToString()) == 0)
                {
                    UltraMessageBox.Show("Importe por Pagar no puede ser cero", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPagar.Focus();
                    return;
                }
                if (ValidaCamposNulosVacios())
                {
                    if (ValidarCuentasGeneracionLibro())
                    {
                        if (_Mode == "New")
                        {
                            while (_objReciboHonorariosBL.ExisteNroRegistro(txtPeriodo.Text, txtMes.Text, txtCorrelativo.Text) == false)
                            {

                                txtCorrelativo.Text = (int.Parse(txtCorrelativo.Text) + 1).ToString("00000000");
                            }

                            _recibohonorarioDto.v_Periodo = txtPeriodo.Text;
                            _recibohonorarioDto.v_Mes = txtMes.Text;
                            _recibohonorarioDto.v_Correlativo = txtCorrelativo.Text;
                            _recibohonorarioDto.v_SerieDocumento = txtSerieDoc.Text;
                            _recibohonorarioDto.v_CorrelativoDocumento = txtCorrelativoDoc.Text;
                            _recibohonorarioDto.i_IdIgv = int.Parse(cboIGV.SelectedValue.ToString());
                            _recibohonorarioDto.i_IdTipoDocumento = int.Parse(cboDocumento.Value.ToString());
                            _recibohonorarioDto.t_FechaEmision = dtpFechaEmision.Value.Date;
                            _recibohonorarioDto.t_FechaRegistro = dtpFechaRegistro.Value.Date;
                            _recibohonorarioDto.d_TipoCambio = txtTipoCambio.Text == string.Empty ? 0 : decimal.Parse(txtTipoCambio.Text);
                            _recibohonorarioDto.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());
                            _recibohonorarioDto.i_IdEstado = int.Parse(cboEstado.Value.ToString());
                            _recibohonorarioDto.v_Glosa = txtGlosa.Text;
                            _recibohonorarioDto.d_Importe = decimal.Parse(txtImporte.Text);
                            _recibohonorarioDto.i_RentaCuartaCategoria = rbSiCuartacategoria.Checked == true ? 1 : 0;
                            _recibohonorarioDto.d_RentaCuartaCategoria = txtCuartacategoria.Text == string.Empty ? 0 : decimal.Parse(txtCuartacategoria.Text);
                            _recibohonorarioDto.d_PorPagar = txtPagar.Text == string.Empty ? 0 : decimal.Parse(txtPagar.Text);
                            _recibohonorarioDto.d_TotalDebe = txtDebe.Text == string.Empty ? 0 : decimal.Parse(txtDebe.Text);
                            _recibohonorarioDto.d_TotalHaber = txtHaber.Text == string.Empty ? 0 : decimal.Parse(txtHaber.Text);
                            _recibohonorarioDto.d_Diferencia = txtDiferencia.Text == string.Empty ? 0 : decimal.Parse(txtDiferencia.Text);
                            _recibohonorarioDto.i_PorcentajeCuartaCategoria = int.Parse(cboCuartaCategoria.Value.ToString());
                            _recibohonorarioDto.i_IdDocumentoReferencia = int.Parse(cboDocumentoRef.Value.ToString());
                            _recibohonorarioDto.v_SerieDocumentoRef = txtSerieDocRef.Text.Trim();
                            _recibohonorarioDto.v_CorrelativoDocumentoRef = txtCorrelativoDocRef.Text.Trim();
                            LlenarTemporales();
                            newIdRecibo = _objReciboHonorariosBL.InsertarReciboHonorarios(ref objOperationResult, _recibohonorarioDto, Globals.ClientSession.GetAsList(), _TempDetalle_AgregarDto);
                        }
                        else if (_Mode == "Edit")
                        {

                            var DiarioGenerado = _objReciboHonorariosBL.NroDiario(_recibohonorarioDto.v_IdReciboHonorario);
                            if (DiarioGenerado != string.Empty) //VERIFICAR SINCRONZACION
                            {
                                if (_objReciboHonorariosBL.ConsultarSiTieneTesorerias(DiarioGenerado ))
                                {
                                    //UltraMessageBox.Show("Éste documento no se puede Editar, ya que se realizado un pago desde una tesoreria", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    //return;

                                    var resp =
                               MessageBox.Show(
                                   string.Format(
                                       "El registro ya tiene registrado un pago en Tesorería ¿Desea Editarlo de todas formas?"), "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                    if (resp == DialogResult.No) return;
                                }
                            }

                            _recibohonorarioDto.v_Periodo = txtPeriodo.Text;
                            _recibohonorarioDto.v_Mes = txtMes.Text;
                            _recibohonorarioDto.v_Correlativo = txtCorrelativo.Text;
                            _recibohonorarioDto.v_SerieDocumento = txtSerieDoc.Text;
                            _recibohonorarioDto.v_CorrelativoDocumento = txtCorrelativoDoc.Text;
                            _recibohonorarioDto.i_IdIgv = int.Parse(cboIGV.SelectedValue.ToString());
                            _recibohonorarioDto.i_IdTipoDocumento = int.Parse(cboDocumento.Value.ToString());
                            _recibohonorarioDto.t_FechaEmision = dtpFechaEmision.Value.Date;
                            _recibohonorarioDto.t_FechaRegistro = dtpFechaRegistro.Value.Date;
                            _recibohonorarioDto.d_TipoCambio = txtTipoCambio.Text == string.Empty ? 0 : decimal.Parse(txtTipoCambio.Text);
                            _recibohonorarioDto.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());
                            _recibohonorarioDto.i_IdEstado = int.Parse(cboEstado.Value.ToString());
                            _recibohonorarioDto.v_Glosa = txtGlosa.Text;
                            _recibohonorarioDto.d_Importe = decimal.Parse(txtImporte.Text);
                            _recibohonorarioDto.i_RentaCuartaCategoria = rbSiCuartacategoria.Checked == true ? 1 : 0;
                            _recibohonorarioDto.d_RentaCuartaCategoria = txtCuartacategoria.Text == string.Empty ? 0 : decimal.Parse(txtCuartacategoria.Text);
                            _recibohonorarioDto.d_PorPagar = txtPagar.Text == string.Empty ? 0 : decimal.Parse(txtPagar.Text);
                            _recibohonorarioDto.d_TotalDebe = txtDebe.Text == string.Empty ? 0 : decimal.Parse(txtDebe.Text);
                            _recibohonorarioDto.d_TotalHaber = txtHaber.Text == string.Empty ? 0 : decimal.Parse(txtHaber.Text);
                            _recibohonorarioDto.d_Diferencia = txtDiferencia.Text == string.Empty ? 0 : decimal.Parse(txtDiferencia.Text);
                            _recibohonorarioDto.i_PorcentajeCuartaCategoria = int.Parse(cboCuartaCategoria.Value.ToString());
                            _recibohonorarioDto.i_IdDocumentoReferencia = int.Parse(cboDocumentoRef.Value.ToString());
                            _recibohonorarioDto.v_SerieDocumentoRef = txtSerieDocRef.Text.Trim();
                            _recibohonorarioDto.v_CorrelativoDocumentoRef = txtCorrelativoDocRef.Text.Trim();
                            LlenarTemporales();
                            newIdRecibo = _objReciboHonorariosBL.ActualizarReciboHonorarios(ref objOperationResult, _recibohonorarioDto, Globals.ClientSession.GetAsList(), _TempDetalle_AgregarDto, _TempDetalle_ModificarDto, _TempDetalle_EliminarDto);
                        }

                        if (objOperationResult.Success == 1)
                        {
                            strModo = "Guardado";
                            EdicionBarraNavegacion(true);
                            strIdReciboHonorario = newIdRecibo;
                            ObtenerListadoReciboHonorarios(txtPeriodo.Text, txtMes.Text);
                            _pstrIdMovimiento_Nuevo = newIdRecibo;
                            linkAsiento.Visible = true;
                            
                            if (UltraMessageBox.Show("El registro se ha guardado correctamente ,¿Desea Generar  Nuevo Recibo Honorario?", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
                            {
                                strModo = "Nuevo";
                                btnNuevoMovimiento_Click(sender, e);


                            }
                        }
                        else
                        {
                            UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        _TempDetalle_AgregarDto = new List<recibohonorariodetalleDto>();
                        _TempDetalle_ModificarDto = new List<recibohonorariodetalleDto>();
                        _TempDetalle_EliminarDto = new List<recibohonorariodetalleDto>();
                    }
                }
            }
        }

        #endregion

        #region BarraNavegacion
        private void btnAnterior_Click(object sender, EventArgs e)
        {
            if (_ListadoReciboHonorarios.Count() > 0)
            {
                if (_MaxV == 0) CargarCabecera(_ListadoReciboHonorarios[0].Value2);

                if (_ActV > 0 && _ActV <= _MaxV)
                {
                    _ActV = _ActV - 1;
                    txtCorrelativo.Text = _ListadoReciboHonorarios[_ActV].Value1;
                    CargarCabecera(_ListadoReciboHonorarios[_ActV].Value2);
                }
            }

        }

        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            if (_ActV >= 0 && _ActV < _MaxV)
            {
                _ActV = _ActV + 1;
                txtCorrelativo.Text = _ListadoReciboHonorarios[_ActV].Value1;
                CargarCabecera(_ListadoReciboHonorarios[_ActV].Value2);
            }

        }

        private void btnNuevoMovimiento_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            LimpiarCabecera();
            CargarDetalle("", 1);
           // txtCorrelativo.Text = (int.Parse(_ListadoReciboHonorarios[_MaxV].Value1) + 1).ToString("00000000");
            _ListadoReciboHonorariosCambioFecha = _objReciboHonorariosBL.ObtenerListadoReciboHonorarios(ref objOperationResult, txtPeriodo.Text.Trim(),txtMes.Text.Trim ());
            if (_ListadoReciboHonorariosCambioFecha.Count != 0)
            {
                int MaxMovimiento;
                MaxMovimiento = _ListadoReciboHonorariosCambioFecha.Count() > 0 ? int.Parse(_ListadoReciboHonorariosCambioFecha[_ListadoReciboHonorariosCambioFecha.Count() - 1].Value1.ToString()) : 0;
                MaxMovimiento++;
                txtCorrelativo.Text = MaxMovimiento.ToString("00000000");
           
            }
            else
            {
                txtCorrelativo.Text = "00000001"; 
            }


            _Mode = "New";
            _recibohonorarioDto = new recibohonorarioDto();
            EdicionBarraNavegacion(false);
            txtTipoCambio.Text = _objReciboHonorariosBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaEmision.Value.Date);
            linkAsiento.Visible = false;
        }

        #endregion


        #region Clases/Validaciones
        private bool ValidaCamposNulosVacios()
        {
            foreach (var Fila in grdData.Rows)
            {
                string Cuenta = Fila.Cells["v_NroCuenta"].Value.ToString().Trim();

                if (!_objReciboHonorariosBL.ExistenciaCuentaImputable(Cuenta))
                {

                    UltraMessageBox.Show("Por favor ingrese Nro. de Cuenta válida ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    grdData.Selected.Cells.Add(Fila.Cells["v_NroCuenta"]);
                    grdData.Focus();
                    Fila.Activate();
                    grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                    UltraGridCell aCell = this.grdData.ActiveRow.Cells["v_NroCuenta"];
                    this.grdData.ActiveCell = aCell;
                    return false;
                }

                string CentroCosto = Fila.Cells["i_CCosto"].Value.ToString().Trim();
                if ( Fila.Cells ["i_ValidarCentroCosto"].Value !=null && Fila.Cells ["i_ValidarCentroCosto"].Value.ToString ()=="1"&& !_objDatahierarchyBL.EsValidoDtahierarchy (CentroCosto,31))
                {
                    UltraMessageBox.Show("Por favor ingrese Centro Costo Válido ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    grdData.Selected.Cells.Add(Fila.Cells["i_CCosto"]);
                    grdData.Focus();
                    Fila.Activate();
                    grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                    UltraGridCell aCell = this.grdData.ActiveRow.Cells["i_CCosto"];
                    this.grdData.ActiveCell = aCell;
                    return false;
                }

            }

            if (grdData.Rows.Where(p => p.Cells["v_NroCuenta"].Value == null || p.Cells["v_NroCuenta"].Value.ToString().Trim() == string.Empty).Count() != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente las Cuentas", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdData.Rows.Where(x => x.Cells["v_NroCuenta"].Value == null || x.Cells["v_NroCuenta"].Value.ToString().Trim() == string.Empty).FirstOrDefault();
                grdData.Selected.Cells.Add(Row.Cells["v_NroCuenta"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["v_NroCuenta"];
                this.grdData.ActiveCell = aCell;
                return false;
            }

            if (grdData.Rows.Where((p => (p.Cells["i_CCosto"].Value == null || p.Cells["i_CCosto"].Value.ToString().Trim() == "-1" || p.Cells["i_CCosto"].Value.ToString().Trim() == string.Empty) && (p.Cells["i_ValidarCentroCosto"].Value != null && p.Cells["i_ValidarCentroCosto"].Value.ToString() == "1"))).Count() != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente los Centros de Costo", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdData.Rows.Where(x => (x.Cells["i_CCosto"].Value == null || x.Cells["i_CCosto"].Value.ToString().Trim() == "-1" || x.Cells["i_CCosto"].Value.ToString().Trim() == string.Empty) && (x.Cells["i_ValidarCentroCosto"].Value != null && x.Cells["i_ValidarCentroCosto"].Value.ToString() == "1")).FirstOrDefault();
                grdData.Selected.Cells.Add(Row.Cells["i_CCosto"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["i_CCosto"];
                this.grdData.ActiveCell = aCell;
                return false;
            }
            if (grdData.Rows.Where((p => (p.Cells["i_CCosto"].Value != null && p.Cells["i_CCosto"].Value.ToString().Trim() != string.Empty) && (p.Cells["i_ValidarCentroCosto"].Value != null && p.Cells["i_ValidarCentroCosto"].Value.ToString() == "0"))).Count() != 0)
            {
                UltraMessageBox.Show("La configuración de la  cuenta no tiene centro de costo", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdData.Rows.Where(x => (x.Cells["i_CCosto"].Value != null || x.Cells["i_CCosto"].Value.ToString().Trim() != "-1" || x.Cells["i_CCosto"].Value.ToString().Trim() != string.Empty) && (x.Cells["i_ValidarCentroCosto"].Value != null && x.Cells["i_ValidarCentroCosto"].Value.ToString() == "0")).FirstOrDefault();
                grdData.Selected.Cells.Add(Row.Cells["i_CCosto"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["i_CCosto"];
                this.grdData.ActiveCell = aCell;
                return false;
            }
            //if (cboMoneda.SelectedValue =="-1")return ;
            if (cboMoneda.Value.ToString() == "1")
            {

                if (grdData.Rows.Where(p => p.Cells["d_ImporteSoles"].Value == null || p.Cells["d_ImporteSoles"].Value.ToString().Trim() == string.Empty || decimal.Parse(p.Cells["d_ImporteSoles"].Value.ToString().Trim()) <= 0).Count() != 0)
                {
                    UltraMessageBox.Show("Por favor ingrese correctamente los Importes", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UltraGridRow Row = grdData.Rows.Where(x => x.Cells["d_ImporteSoles"].Value == null || x.Cells["d_ImporteSoles"].Value.ToString().Trim() == string.Empty || decimal.Parse(x.Cells["d_ImporteSoles"].Value.ToString().Trim()) <= 0).FirstOrDefault();
                    grdData.Selected.Cells.Add(Row.Cells["d_ImporteSoles"]);
                    grdData.Focus();
                    Row.Activate();
                    grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                    UltraGridCell aCell = this.grdData.ActiveRow.Cells["d_ImporteSoles"];
                    this.grdData.ActiveCell = aCell;
                    return false;
                }


            }
            else if (cboMoneda.Value.ToString() == "2")
            {

                if (grdData.Rows.Where(p => p.Cells["d_ImporteDolares"].Value == null || p.Cells["d_ImporteDolares"].Value.ToString().Trim() == string.Empty || decimal.Parse(p.Cells["d_ImporteDolares"].Value.ToString().Trim()) <= 0).Count() != 0)
                {
                    UltraMessageBox.Show("Por favor ingrese correctamente los Importes", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UltraGridRow Row = grdData.Rows.Where(x => x.Cells["d_ImporteDolares"].Value == null || x.Cells["d_ImporteDolares"].Value.ToString().Trim() == string.Empty || decimal.Parse(x.Cells["d_ImporteDolares"].Value.ToString().Trim()) <= 0).FirstOrDefault();
                    grdData.Selected.Cells.Add(Row.Cells["d_ImporteDolares"]);
                    grdData.Focus();
                    Row.Activate();
                    grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                    UltraGridCell aCell = this.grdData.ActiveRow.Cells["d_ImporteDolares"];
                    this.grdData.ActiveCell = aCell;
                    return false;
                }

            }

           


            return true;
        }

        private bool ValidarCuentasGeneracionLibro()
        {

            string CuartaCategoria = int.Parse(((int)Concepto.CuartaCategoria).ToString()) < 10 ? ("0" + ((int)Concepto.CuartaCategoria).ToString()).Trim() : (((int)Concepto.CuartaCategoria).ToString()).Trim();
            string PorPagarSoles = int.Parse(((int)Concepto.PorPagarSoles).ToString()) < 10 ? ("0" + ((int)Concepto.PorPagarSoles).ToString()).Trim() : (((int)Concepto.PorPagarSoles).ToString()).Trim();
            string PorPagarDolares = int.Parse(((int)Concepto.PorPagarDolares).ToString()) < 10 ? ("0" + ((int)Concepto.PorPagarDolares).ToString()).Trim() : (((int)Concepto.PorPagarDolares).ToString()).Trim();

            if (rbSiCuartacategoria.Checked)
            {
                if (!_objReciboHonorariosBL.ValidarNroCuentaGeneracionLibro(CuartaCategoria))
                {
                    UltraMessageBox.Show("Concepto Imp. 4ta. Categoria(Código 06) en  Administracion Conceptos no es correcto para poder generar el  Libro Diario", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            if (cboMoneda.Value.ToString() == "1") //  && _objReciboHonorariosBL.ValidarNroCuentaGeneracionLibro(CuartaCategoria) && _objReciboHonorariosBL.ValidarNroCuentaGeneracionLibro(PorPagarSoles))
            {

                if (!_objReciboHonorariosBL.ValidarNroCuentaGeneracionLibro(PorPagarSoles))
                {
                    UltraMessageBox.Show("Concepto Por Pagar (Código 08) en  Administracion Conceptos no es correcto para poder generar el  Libro Diario", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;

                }
            }
            else if (cboMoneda.Value.ToString() == "2")
            {
                if (!_objReciboHonorariosBL.ValidarNroCuentaGeneracionLibro(PorPagarDolares))
                {
                    UltraMessageBox.Show("Concepto Por Pagar M.E. (Código 09) en  Administracion Conceptos no es correcto  para poder generar el  Libro Diario", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;

                }

            }
            return true;

        }


        private void GenerarNumeroRegistro()
        {
            OperationResult objOperationResult = new OperationResult();
            string Mes;
            // Mes = int.Parse(dtpFechaRegistro.Value.Month.ToString()) <= 9 ? ("0" + dtpFechaRegistro.Value.Month.ToString()).Trim() : dtpFechaRegistro.Value.Month.ToString();
            Mes = dtpFechaRegistro.Value.Month.ToString("00");
            _ListadoReciboHonorariosCambioFecha = _objReciboHonorariosBL.ObtenerListadoReciboHonorarios(ref objOperationResult, txtPeriodo.Text.Trim(), Mes);
            if (_ListadoReciboHonorariosCambioFecha.Count != 0)
            {
                int MaxMovimiento;
                MaxMovimiento = _ListadoReciboHonorariosCambioFecha.Count() > 0 ? int.Parse(_ListadoReciboHonorariosCambioFecha[_ListadoReciboHonorariosCambioFecha.Count() - 1].Value1.ToString()) : 0;
                MaxMovimiento++;
                txtCorrelativo.Text = MaxMovimiento.ToString("00000000");
                txtMes.Text = int.Parse(dtpFechaRegistro.Value.Month.ToString()) <= 9 ? 0 + dtpFechaRegistro.Value.Month.ToString() : dtpFechaRegistro.Value.Month.ToString();
                txtPeriodo.Text = dtpFechaRegistro.Value.Year.ToString();
            }
            else
            {
                txtCorrelativo.Text = "00000001";
                txtMes.Text = int.Parse(dtpFechaRegistro.Value.Month.ToString()) <= 9 ? 0 + dtpFechaRegistro.Value.Month.ToString() : dtpFechaRegistro.Value.Month.ToString();
                txtPeriodo.Text = dtpFechaRegistro.Value.Year.ToString();
            }

        }

        #endregion
        private void frmReciboHonorario_Fill_Panel_PaintClient(object sender, PaintEventArgs e)
        {

        }

        private void cboCuartaCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Edicion)
            {
                CalcularValores(0);
            }
        }

        private void cboDocumento_KeyUp(object sender, KeyEventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboDocumento.Rows)
            {
                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
                {
                    return;
                }
                bool filterRow = true;
                foreach (UltraGridColumn column in cboDocumento.DisplayLayout.Bands[0].Columns)
                {
                    if (column.IsVisibleInLayout)
                    {
                        if (row.Cells[column].Text.Contains(cboDocumento.Text.ToUpper()))
                        {
                            filterRow = false;
                            break;
                        }
                    }
                }
                if (filterRow)
                {
                    row.Hidden = true;
                }
                else
                {
                    row.Hidden = false;
                }
            }
        }

        private void cboDocumento_Leave(object sender, EventArgs e)
        {
            //if (strModo == "Nuevo")
            //{
            if (cboDocumento.Text.Trim() == "")
            {
                cboDocumento.Value = "-1";
            }
            else
            {
                var x = _ListadoComboDocumentos.Find(p => p.Id == cboDocumento.Value.ToString() | p.Id == cboDocumento.Text);
                if (x == null)
                {
                    cboDocumento.Value = "-1";
                }
            }
            //txtSerieDoc.Text = _objDocumentoBL.DevolverSeriePorDocumento(Globals.ClientSession.i_IdEstablecimiento, int.Parse(cboDocumento.Value.ToString()));
            //txtNumeroDoc.Text = _objDocumentoBL.DevolverCorrelativoPorDocumento(Globals.ClientSession.i_IdEstablecimiento, int.Parse(cboDocumento.Value.ToString())).ToString("00000000");
            //ComprobarExistenciaCorrelativoDocumento();
            //}
        }

        private void cboDocumento_AfterDropDown(object sender, EventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboDocumento.Rows)
            {
                if (cboDocumento.Value == null) return;
                if (cboDocumento.Value.ToString() == "-1") cboDocumento.Text = string.Empty;
                bool filterRow = true;
                foreach (UltraGridColumn column in cboDocumento.DisplayLayout.Bands[0].Columns)
                {
                    if (column.IsVisibleInLayout)
                    {
                        if (row.Cells[column].Text.Contains(cboDocumento.Text.ToUpper()))
                        {
                            filterRow = false;
                            break;
                        }
                    }
                }
                if (filterRow)
                {
                    row.Hidden = true;
                }
                else
                {
                    row.Hidden = false;
                }

            }
        }



        private void ValidarNumeroRegistro()
        {
            if (txtSerieDoc.Text != string.Empty && cboDocumento.Value.ToString() != "-1" && txtCorrelativoDoc.Text != string.Empty)
            {

                if (_objReciboHonorariosBL.ValidarNumeroRegistro(int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text.Trim(), txtCorrelativoDoc.Text.Trim(), _recibohonorarioDto.v_IdReciboHonorario, _recibohonorarioDto.v_IdProveedor, txtPeriodo.Text.Trim()))
                {
                    UltraMessageBox.Show("Este Documento ya ha sido Registrado para este Proveedor ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //txtCorrelativoDoc.Focus();
                    //return;
                }

            }


        }
        private void txtRucProveedor_Validating(object sender, CancelEventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (txtRucProveedor.Text != string.Empty)
            {
                if (txtRucProveedor.TextLength != 11)
                {
                    UltraMessageBox.Show("RUC Inválido", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _recibohonorarioDto.v_IdProveedor = null;
                    txtCodigoProveedor.Clear();
                    txtRazonSocial.Clear();
                    return;

                }
                else
                {
                    if (Utils.Windows.ValidarRuc(txtRucProveedor.Text.Trim()) != true)
                    {
                        UltraMessageBox.Show("RUC Inválido", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _recibohonorarioDto.v_IdProveedor = null;
                        txtCodigoProveedor.Clear();
                        txtRazonSocial.Clear();
                        return;
                    }
                    else if (!txtRucProveedor.Text.StartsWith("1"))
                    {

                        UltraMessageBox.Show("RUC No Autorizado para Recibo Por Honorarios", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _recibohonorarioDto.v_IdProveedor = null;
                        txtCodigoProveedor.Clear();
                        txtRazonSocial.Clear();
                        return;

                    }
                    else
                    {
                        var Cliente = _objClienteBL.ObtenerClienteCodigoBandejas(ref objOperationResult, txtRucProveedor.Text.Trim(), "V");
                        if (Cliente != null)
                        {
                            txtRazonSocial.Text = (Cliente.v_ApePaterno + " " + Cliente.v_ApeMaterno.Trim() + " " + Cliente.v_PrimerNombre.Trim() + " " + Cliente.v_SegundoNombre.Trim() + " " + Cliente.v_RazonSocial.Trim()).Trim();
                            txtCodigoProveedor.Text = Cliente.v_CodCliente.Trim();
                            _recibohonorarioDto.v_IdProveedor = Cliente.v_IdCliente;

                        }
                        else
                        {
                            UltraMessageBox.Show("Cliente no Existe", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            txtRazonSocial.Clear();
                            txtCodigoProveedor.Clear();
                            _recibohonorarioDto.v_IdProveedor = null;
                            return;

                        }

                    }
                }
            }


            if (txtSerieDoc.Text != string.Empty && cboDocumento.Value.ToString() != "-1" && txtCorrelativoDoc.Text != string.Empty)
            {
                if (_objReciboHonorariosBL.ValidarNumeroRegistro(int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text.Trim(), txtCorrelativoDoc.Text.Trim(), _recibohonorarioDto.v_IdReciboHonorario, _recibohonorarioDto.v_IdProveedor, txtPeriodo.Text.Trim()))
                {
                    UltraMessageBox.Show("Este Documento ya ha sido Registrado para este Proveedor ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtCorrelativoDoc.Focus();
                    return;
                }

            }
        }

        private void grdData_ClickCellButton(object sender, CellEventArgs e)
        {
            switch (e.Cell.Column.Key)
            {
                case "v_NroCuenta":

                    string CuentaPredeterminada = "60";
                    if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroCuenta"].Text != null && grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroCuenta"].Text != string.Empty)
                    {
                        CuentaPredeterminada = grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroCuenta"].Text.Trim();
                    }


                    Mantenimientos.frmPlanCuentasConsulta frm = new Mantenimientos.frmPlanCuentasConsulta(CuentaPredeterminada);
                    frm.ShowDialog();

                    if (frm._NombreCuenta != null)
                    {
                        grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroCuenta"].Value = frm._NroSubCuenta.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                    }
                    break;


                case "i_CCosto":
                    if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_ValidarCentroCosto"].Value != null && grdData.Rows[grdData.ActiveRow.Index].Cells["i_ValidarCentroCosto"].Value.ToString() == "1")
                    {
                        Mantenimientos.frmBuscarDatahierarchy frm2 = new Mantenimientos.frmBuscarDatahierarchy(31, "Buscar Centro de Costos");
                        frm2.ShowDialog();
                        if (frm2._itemId != null)
                        {
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_CCosto"].Value = frm2._value2;
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                        }

                    }
                    break;

                //}
            }
        }

        private void ActDesacDocReferencia()
        {

            if (_objDocumentoBL.DocumentoEsInverso(int.Parse(cboDocumento.Value.ToString()))  && txtSerieDoc.Text.Trim() != string.Empty && txtCorrelativoDoc.Text.Trim() != string.Empty && strModo != "Edicion")
            {

                btnBuscarDocReferencia.Enabled = true;
                //cboDocumentoRef.Enabled = true;
                //txtSerieDocRef.Enabled = true;
                //txtCorrelativoDocRef.Enabled = true;

            }
            else
            {
                btnBuscarDocReferencia.Enabled = false;
                // cboDocumentoRef.Enabled = false;
                //txtSerieDocRef.Enabled = false;
                //txtCorrelativoDocRef.Enabled = false;

            }

        }

        private void btnBuscarDocReferencia_Click(object sender, EventArgs e)
        {
            frmBandejaReciboHonoarios frm = new frmBandejaReciboHonoarios("DocRef");
            frm.ShowDialog();

            if (frm._IdReciboHonorario != null)
            {
                if (_objReciboHonorariosBL.TieneDocReferenciasAsociados(frm._TipoDoc == null ? -1 : frm._TipoDoc, frm._Serie, frm._Correlativo))
                {
                    UltraMessageBox.Show("Este documento ya tiene asociado una  Notas de Crédito ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                cboDocumentoRef.Value = frm._TipoDoc.ToString();
                txtSerieDocRef.Text = frm._Serie;
                txtCorrelativoDocRef.Text = frm._Correlativo;
                IdReciboHonorarioUtilizDocRef = frm._IdReciboHonorario;
                EsDocRef = true;
                if (grdData.Rows.Any())
                {
                    if (UltraMessageBox.Show("Ya se ha cargado un documento previamente, ¿Desea cargar este otro documento?", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.No)
                    {
                        return;
                    }
                }
                CargarReciboHonorario(IdReciboHonorarioUtilizDocRef);
                EsDocRef = false;

            }

        }


        private void CargarReciboHonorario(string pIdReciboHonorario)
        {
            CargarCabecera(pIdReciboHonorario);
        }

        private void txtSerieDocRef_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtSerieDocRef, "{0:0000}");
        }

        private void txtCorrelativoDocRef_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtCorrelativoDocRef, "{0:0000}");
        }

        private void cboDocumento_Validated(object sender, EventArgs e)
        {
            ActDesacDocReferencia();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkAsiento_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

         
        }

        private void txtCorrelativo_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            switch (e.Button.Key)
            {
                case "btnAnterior":
                    if (OnAnterior == null) return;
                    OnAnterior();
                    if (!string.IsNullOrWhiteSpace(_idRecibido))
                    {
                        CargarCabecera(_idRecibido);
                    }

                    break;
                default:
                    if (OnSiguiente == null) return;
                    OnSiguiente();
                    if (!string.IsNullOrWhiteSpace(_idRecibido))
                    {
                        CargarCabecera(_idRecibido);
                    }
                    break;
            }
        }

        private void btnDuplicarFila_Click(object sender, EventArgs e)
        {
            try
            {
                var filaActiva = grdData.ActiveRow;
                if (filaActiva != null)
                {

                    var objectSource = (BindgListReciboHonorarioDetalleDto)filaActiva.ListObject;
                    var nroFilas = grdData.Rows.Count();
                    
                    btnAgregar_Click(sender, e);
                   
                    if (nroFilas != grdData.Rows.Count())
                    {
                        var nuevaFila = grdData.Rows.LastOrDefault();
                        if (nuevaFila != null)
                        {
                            nuevaFila.Cells["v_NroCuenta"].Value = objectSource.v_NroCuenta;
                            nuevaFila.Cells["i_CCosto"].Value = objectSource.i_CCosto;
                            nuevaFila.Cells["d_ImporteSoles"].Value = objectSource.d_ImporteSoles;
                            nuevaFila.Cells["d_ImporteDolares"].Value = objectSource.d_ImporteDolares;
                            nuevaFila.Cells["i_ValidarCentroCosto"].Value = objectSource.i_ValidarCentroCosto;
                            grdData.PerformAction(UltraGridAction.EnterEditMode);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

   

       
    }
}
