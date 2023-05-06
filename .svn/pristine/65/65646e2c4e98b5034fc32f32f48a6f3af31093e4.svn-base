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
using SAMBHS.Windows.WinClient.UI.Mantenimientos;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmCajaChica : Form
    {
        List<UltraComboTipoMovimiento> ListaUltraCombo = new List<UltraComboTipoMovimiento>();
        List<GridKeyValueDTO> _ListadoComboDocumentos = new List<GridKeyValueDTO>();
        public DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        UltraCombo ucCosto = new UltraCombo();
        UltraCombo ucTipoDocumento = new UltraCombo();
        UltraCombo ucMotivo = new UltraCombo();
        string _Mode;
        string strModo = "Nuevo";
        // int Scroll = 0;
        UltraCombo ucbTipoMovimiento = new UltraCombo();
        public string _pstrIdCajaChica_Nuevo, newIdCajaChica;
        CajaChicaBL _objCajaChicaBL = new CajaChicaBL();
        DiarioBL _objDiarioBL = new DiarioBL();
        cajachicadetalleDto _objcajadetalleDto = new cajachicadetalleDto();
        TesoreriaBL _objTesoreriaBL = new TesoreriaBL();
        ConceptosChicaBL _objConceptosCajaChicaBL = new ConceptosChicaBL();
        //List<KeyValueDTO> _ListadoReciboHonorarios = new List<KeyValueDTO>();
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        List<KeyValueDTO> _ListadoReciboHonorariosCambioFecha = new List<KeyValueDTO>();
        cajachicaDto objCajaChica = new cajachicaDto();
        int _MaxV, _ActV, Scroll = 0;
        List<conceptoscajachicaDto> ListaConceptosCajaChica = new List<conceptoscajachicaDto>();
        bool _btnGuardar, Edicion = false;
        #region Temporales DetalleCajaChica
        List<cajachicadetalleDto> _TempDetalle_AgregarDto = new List<cajachicadetalleDto>();
        List<cajachicadetalleDto> _TempDetalle_ModificarDto = new List<cajachicadetalleDto>();
        List<cajachicadetalleDto> _TempDetalle_EliminarDto = new List<cajachicadetalleDto>();
        #endregion

        #region Eventos de navegacion
        public string IdRecibido
        {
            set { _idRecibido = value; }
        }
        private string _idRecibido;
        public delegate void OnSiguienteAnterior();
        public event OnSiguienteAnterior OnSiguiente;
        public event OnSiguienteAnterior OnAnterior;

        #endregion
        public frmCajaChica(string Modo, string IdCajaChica)
        {
            InitializeComponent();
            strModo = Modo;
            newIdCajaChica = IdCajaChica;
        }
        private void frmCajaChica_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            panel1.BackColor = new GlobalFormColors().BannerColor;
            OperationResult objOperationResult = new OperationResult();
            txtPeriodo.Text = Globals.ClientSession.i_Periodo.ToString();
            txtMes.Text = DateTime.Now.Month.ToString();
            Utils.Windows.FijarFormatoUltraTextBox(txtMes, "{0:00}");
            #region Controla Cierre de Mes
            if (new CierreMensualBL().VerificarMesCerrado(txtPeriodo.Text, txtMes.Text, (int)ModulosSistema.Tesoreria))
            {
                btnGuardar.Visible = false;
                this.Text = "Caja Chica [MES CERRADO]";
            }
            else
            {
                btnGuardar.Visible = true;
                this.Text = "Caja Chica";
            }
            #endregion
            ConfigurarCombosDetalle();
            CargarCombos();
            // ValidarFechas();
            Scroll = int.Parse(dtpFechaRegistro.Value.Month.ToString());
            btnEliminar.Enabled = false;
            if (Application.OpenForms["LoadingForm"] != null) ((LoadingForm)Application.OpenForms["LoadingForm"]).CloseWindow();
            if (Application.OpenForms["frmMaster"] != null) ((frmMaster)Application.OpenForms["frmMaster"]).Activate();
            cboEstado.Value = "1";
            ObtenerListadoCajaChica();
            #region Combo Motivo
            UltraGridBand _ultraGridBandaMotivo = new UltraGridBand("Band 0", -1);
            UltraGridColumn _ultraGridColumnaIDMotivo = new UltraGridColumn("Id");
            UltraGridColumn _ultraGridColumnaDescripcionMotivo = new UltraGridColumn("Value1");
           
            _ultraGridColumnaIDMotivo.Header.Caption = "Cod.";
            _ultraGridColumnaDescripcionMotivo.Header.Caption = "Descripción";
           
            _ultraGridColumnaIDMotivo.Width = 40;
            _ultraGridColumnaDescripcionMotivo.Width = 200;
            _ultraGridBandaMotivo.Columns.AddRange(new object[] { _ultraGridColumnaIDMotivo, _ultraGridColumnaDescripcionMotivo });
            ucMotivo.DisplayLayout.BandsSerializer.Add(_ultraGridBandaMotivo);
            ucMotivo.DropDownStyle = Infragistics.Win.UltraWinGrid.UltraComboStyle.DropDownList;
            ucMotivo.DropDownWidth = 330;

            Utils.Windows.LoadUltraComboList(ucMotivo, "Value1", "Id", _objConceptosCajaChicaBL.ObtenerConceptosCajaChica(ref objOperationResult), DropDownListAction.Select);
            ucMotivo.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
           
            #endregion
            ListaConceptosCajaChica = _objConceptosCajaChicaBL.ObtenerListadoConceptosCajaChica(ref objOperationResult, null, null);


        }
        private void CargarCombos()
        {
            OperationResult objOperationResult = new OperationResult();
            _ListadoComboDocumentos = _objDocumentoBL.ObtenDocumentosCobranzaParaComboGrid(ref objOperationResult, null);
            Utils.Windows.LoadUltraComboList(cboTipoDocumento, "Value2", "Id", _ListadoComboDocumentos, DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            cboTipoDocumento.Value = "-1";
            cboMoneda.Value = "1";
        }
        private void ConfigurarCombosDetalle()
        {
            OperationResult objOperationResult = new OperationResult();

            #region Configura Combo Motivo

            ListaUltraCombo.Add(new UltraComboTipoMovimiento() { respuesta = "INGRESO", Codigo = 1 });
            ListaUltraCombo.Add(new UltraComboTipoMovimiento() { respuesta = "SALIDA", Codigo = 0 });

            UltraGridBand ultraGridBanda_ = new UltraGridBand("Band 0", -1);
            UltraGridColumn ultraGridColumnaID_ = new UltraGridColumn("Codigo");
            UltraGridColumn ultraGridColumnaDescripcion_ = new UltraGridColumn("respuesta");
            ultraGridColumnaDescripcion_.Header.Caption = "Descripción";
            ultraGridColumnaDescripcion_.Header.VisiblePosition = 0;
            ultraGridColumnaDescripcion_.Width = 267;
            ultraGridColumnaID_.Hidden = true;
            ultraGridBanda_.Columns.AddRange(new object[] { ultraGridColumnaDescripcion_, ultraGridColumnaID_ });
            ucbTipoMovimiento.DisplayLayout.BandsSerializer.Add(ultraGridBanda_);
            ucbTipoMovimiento.DropDownWidth = 270;
            ucbTipoMovimiento.DropDownStyle = UltraComboStyle.DropDownList;
            ucbTipoMovimiento.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
            ucbTipoMovimiento.DataSource = ListaUltraCombo;
            ucbTipoMovimiento.DisplayMember = "respuesta";
            ucbTipoMovimiento.ValueMember = "Codigo";
            #endregion




            #region Configura Combo Tipo Documento
            UltraGridBand _ultraGridBanda = new UltraGridBand("Band 0", -1);
            UltraGridColumn _ultraGridColumnaID = new UltraGridColumn("Id");
            UltraGridColumn _ultraGridColumnaDescripcion = new UltraGridColumn("Value1");
            UltraGridColumn _ultraGridColumnaSiglas = new UltraGridColumn("Value2");
            _ultraGridColumnaID.Header.Caption = "Cod.";
            _ultraGridColumnaDescripcion.Header.Caption = "Descripción";
            _ultraGridColumnaSiglas.Header.Caption = "Siglas";
            _ultraGridColumnaID.Width = 40;
            _ultraGridColumnaDescripcion.Width = 200;
            _ultraGridColumnaSiglas.Width = 80;
            _ultraGridBanda.Columns.AddRange(new object[] { _ultraGridColumnaID, _ultraGridColumnaDescripcion, _ultraGridColumnaSiglas });
            ucTipoDocumento.DisplayLayout.BandsSerializer.Add(_ultraGridBanda);
            ucTipoDocumento.DropDownStyle = Infragistics.Win.UltraWinGrid.UltraComboStyle.DropDownList;
            ucTipoDocumento.DropDownWidth = 330;
            Utils.Windows.LoadUltraComboList(ucTipoDocumento, "Value2", "Id", _objDocumentoBL.ObtenDocumentosParaComboGridLibroDiarios(ref objOperationResult), DropDownListAction.Select);
            #endregion




            ucbTipoMovimiento.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
            ucTipoDocumento.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;



        }
        private void ObtenerListadoCajaChica()
        {
            OperationResult objOperationResult = new OperationResult();
            //  _ListadoReciboHonorarios = _objCuadreCajaBL.ObtenerListadoReciboHonorarios(ref objOperationResult, pstrPeriodo, pstrMes);
            switch (strModo)
            {
                case "Edicion":

                    EdicionBarraNavegacion(false);
                    cboTipoDocumento.Enabled = false;
                    Edicion = true;
                    CargarCabecera(newIdCajaChica);
                    Edicion = false;
                    linkAsiento.Visible = true;
                    break;

                case "Nuevo":
                    _Mode = "New";
                    LimpiarCabecera();
                    CargarDetalle("");
                    linkAsiento.Visible = false;
                    txtMes.Enabled = false;
                    break;


                case "Consulta":
                    CargarCabecera(newIdCajaChica);
                    cboTipoDocumento.Enabled = false;
                    RestringirEdicion();
                    break;

            }
        }


        private void RestringirEdicion()
        {
            btnGuardar.Enabled = true;
            linkAsiento.Visible = false;

        }
        #region  Grilla


        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (cboEstado.Value.ToString() == "1")
            {
                //if (grdData.ActiveRow != null)
                //{
                var ultimaFila = grdData.Rows.LastOrDefault();
                if (ultimaFila == null || (ultimaFila.Cells["i_IdConceptosCajaChica"].Value != null))
                {
                    UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
                    grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                    this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                    row.Cells["i_RegistroEstado"].Value = "Agregado";
                    row.Cells["i_RegistroTipo"].Value = "Temporal";
                    row.Cells["i_IdTipoDocumento"].Value = "-1";
                    row.Cells["i_Motivo"].Value = grdData.Rows.Count == 1 ? 1 : 0;
                    row.Cells["d_Importe"].Value = grdData.Rows.Count == 1 ? _objCajaChicaBL.ObtenerSaldoUltimaCajaChica(Globals.ClientSession.GetAsList(), int.Parse(cboTipoDocumento.Value.ToString())).ToString() : "0.000";
                    row.Cells["i_IdConceptosCajaChica"].Value = grdData.Rows.Count == 1 ? 1 : -1;


                }
                else
                {
                    UltraMessageBox.Show("Por favor ingrese una cuenta válida", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


                grdData.Focus();
                UltraGridCell aCell1 = this.grdData.ActiveRow.Cells["d_Importe"];
                this.grdData.ActiveCell = aCell1;
                grdData.ActiveRow = aCell1.Row;
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                strModo = "Editado";
            }
        }
        private void grdData_KeyDown(object sender, KeyEventArgs e)
        {


            if (grdData.ActiveRow == null) return;

            switch (grdData.ActiveCell.Column.Key)
            {

                case "d_Importe":

                    if (EsCuentaApertura(grdData.Rows[grdData.ActiveRow.Index].Cells["i_Motivo"].Value == null ? -1 : int.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["i_Motivo"].Value.ToString()), grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdConceptosCajaChica"].Value == null ? 0 : int.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdConceptosCajaChica"].Value.ToString()), grdData.Rows[grdData.ActiveRow.Index].Cells["Index"].Value == null ? -5 : int.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["Index"].Value.ToString())))
                    {
                        if (grdData.Rows[grdData.ActiveRow.Index].Cells["d_Importe"].Value != null && decimal.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["d_Importe"].Value.ToString()) == 0)
                        {
                            e.SuppressKeyPress = false;
                        }
                        else
                        {
                            e.SuppressKeyPress = true;
                        }
                    }
                    break;

                case "i_IdTipoDocumento":
                            e.SuppressKeyPress =  grdData.Rows[grdData.ActiveRow.Index].Cells["i_RequiereTipoDocumento"].Value.ToString() == "0" ?true:false ;
                       
                    break;

                case "v_NroDocumento":

                        e.SuppressKeyPress = grdData.Rows[grdData.ActiveRow.Index].Cells["i_RequiereNumeroDocumento"].Value.ToString() == "0" ? true:false ;
                   
                    break;

                case "CodigoAnexo":
                    e.SuppressKeyPress = true;//grdData.Rows[grdData.ActiveRow.Index].Cells["i_RequiereAnexo"].Value.ToString() == "0" ? true : false;
                    break;
            }
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
            e.Layout.Bands[0].Columns["i_IdTipoDocumento"].EditorComponent = ucTipoDocumento;
            e.Layout.Bands[0].Columns["i_IdTipoDocumento"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;

            e.Layout.Bands[0].Columns["i_Motivo"].EditorComponent = ucbTipoMovimiento;
            e.Layout.Bands[0].Columns["i_Motivo"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;

            e.Layout.Bands[0].Columns["i_IdConceptosCajaChica"].EditorComponent = ucMotivo;
            e.Layout.Bands[0].Columns["i_IdConceptosCajaChica"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
        }


        private void grdData_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {
            switch (e.Cell.Column.Key)
            {
                case "CodigoAnexo":

                    if (grdData.ActiveRow.Cells["i_RequiereAnexo"].Value != null && grdData.ActiveRow.Cells["i_RequiereAnexo"].Value.ToString() == "1")
                    {
                        var f = new frmBuscarAnexo();
                        f.ShowDialog();
                        if (f.DialogResult == DialogResult.OK)
                        {
                            e.Cell.Value = f.Anexo.Codigo;
                            grdData.ActiveRow.Cells["v_IdCliente"].Value = f.Anexo.IdAnexo;
                            grdData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";

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

                    _objcajadetalleDto = new cajachicadetalleDto();
                    _objcajadetalleDto.v_IdCajaChicaDetalle = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdCajaChicaDetalle"].Value.ToString();
                    _TempDetalle_EliminarDto.Add(_objcajadetalleDto);
                    grdData.Rows[grdData.ActiveRow.Index].Delete(false);

                    if (grdData.Rows.Count != 0)
                    {
                        strModo = "Editado";

                    }
                    else
                    {

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

                        strModo = "Editado";
                    }
                    else
                    {

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
                    //case "i_CCosto":
                    //    Celda = grdData.ActiveCell;
                    //    Utils.Windows.NumeroEnteroCelda(Celda, e);
                    //    break;
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
           
            CalcularTotales();
            if (grdData.ActiveCell == null) return;
            OperationResult objOperationResult = new OperationResult();
            switch (grdData.ActiveCell.Column.Key)
            {
                case "v_NroDocumento":
                    if (grdData.ActiveRow.Cells["v_NroDocumento"].Value != null)
                    {

                        var nroGuia = grdData.ActiveRow.Cells["v_NroDocumento"].Value.ToString();

                        if (grdData.ActiveRow.Cells["i_IdTipoDocumento"].Value.ToString() != "309")
                        {
                            if (nroGuia.Count(x => x.ToString() == "-") == 1)
                            {
                                try
                                {
                                    var serieCorrelativo = nroGuia.Split('-');
                                    if (serieCorrelativo.Count(p => !string.IsNullOrEmpty(p)) == 2)
                                    {
                                       
                                        var ser =  Utils.Windows.DevuelveSerieFormateada4Digitos ( ref objOperationResult, serieCorrelativo[0]);
                                        var corr =Utils.Windows.DevuelveCorrelativoFormateada8Digitos (ref objOperationResult, serieCorrelativo[1]);
                                        grdData.ActiveRow.Cells["v_NroDocumento"].Value =ser + "-" + corr;
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                    break;
                case "i_IdConceptosCajaChica":
                    var ConceptoElegido = ListaConceptosCajaChica.Where(o => o.i_IdConceptosCajaChica == int.Parse(grdData.ActiveRow.Cells["i_IdConceptosCajaChica"].Value.ToString())).FirstOrDefault();
                    grdData.ActiveRow.Cells["i_RequiereAnexo"].Value = ConceptoElegido == null ? 0 : ConceptoElegido.i_RequiereAnexo == 1 ? 1 : 0;
                    grdData.ActiveRow.Cells["i_RequiereTipoDocumento"].Value = ConceptoElegido == null ? 0 : ConceptoElegido.i_RequiereTipoDocumento == 1 ? 1 : 0;
                    grdData.ActiveRow.Cells["i_RequiereNumeroDocumento"].Value = ConceptoElegido == null ? 0 : ConceptoElegido.i_RequiereNumeroDocumento == 1 ? 1 : 0;
                    grdData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";
                    grdData.ActiveRow.Cells["v_IdCliente"].Value = grdData.ActiveRow.Cells["i_RequiereAnexo"].Value.ToString () == "0" ? null : grdData.ActiveRow.Cells["v_IdCliente"].Value ==null ?null  :  grdData.ActiveRow.Cells["v_IdCliente"].Value.ToString();
                    grdData.ActiveRow.Cells["CodigoAnexo"].Value = grdData.ActiveRow.Cells["i_RequiereAnexo"].Value.ToString() == "0" ? "" : grdData.ActiveRow.Cells["CodigoAnexo"].Value ==null ?"": grdData.ActiveRow.Cells["CodigoAnexo"].Value.ToString();
                    grdData.ActiveRow.Cells["i_IdTipoDocumento"].Value = grdData.ActiveRow.Cells["i_RequiereTipoDocumento"].Value.ToString() == "0" ? "-1" : grdData.ActiveRow.Cells["i_IdTipoDocumento"].Value.ToString();
                    grdData.ActiveRow.Cells["v_NroDocumento"].Value = grdData.ActiveRow.Cells["i_RequiereNumeroDocumento"].Value.ToString() == "0" ? "" : grdData.ActiveRow.Cells["v_NroDocumento"].Value ==null ?"": grdData.ActiveRow.Cells["v_NroDocumento"].Value.ToString();
                    break;
            }
        }





        public void CalcularTotales()
        {
            try
            {
                if (!grdData.Rows.Any())
                {
                    txtTotalGastos.Text = "0.00";
                    txtTotalIngresos.Text = "0.00";
                    txtSaldoCaja.Text = "0.00";

                }

                else
                {



                    var filas = grdData.Rows.GetRowEnumerator(GridRowType.DataRow, null, null).Cast<UltraGridRow>().ToList();
                    var filasIngresos = filas.Where(f => f.Cells["d_Importe"].Value != null && f.Cells["i_Motivo"].Value.ToString() == "1").ToList();
                    txtTotalIngresos.Text = filasIngresos.Sum(f => decimal.Parse(f.Cells["d_Importe"].Value.ToString())).ToString();
                    var filaSalida = filas.Where(f => f.Cells["d_Importe"].Value != null && f.Cells["i_Motivo"].Value.ToString() == "0").ToList();
                    txtTotalGastos.Text = filaSalida.Sum(f => decimal.Parse(f.Cells["d_Importe"].Value.ToString())).ToString("0.00");
                    txtSaldoCaja.Text = (decimal.Parse(txtTotalIngresos.Text) - decimal.Parse(txtTotalGastos.Text)).ToString("0.00");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nLinea: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));

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

            cboTipoDocumento.Value = UserConfig.Default.TipoDocumentoCajaChica;
            object s1 = new object();
            EventArgs e1 = new EventArgs();
            cboTipoDocumento_Leave(s1, e1);
            OperationResult objOperationResult = new OperationResult();
            txtMes.Text = DateTime.Now.Date.Month.ToString("00");
            txtPeriodo.Text = Globals.ClientSession.i_Periodo.ToString();
            dtpFechaRegistro.Value = DateTime.Now;
            // txtCorrelativo.Text = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult, ListaProcesos.Tesoreria, null, DateTime.Parse(dtpFechaRegistro.Value.ToString()), objCajaChica.v_Correlativo == null ? "" : objCajaChica.v_Correlativo, 0);
            txtCorrelativo.Text = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult, ListaProcesos.CajaChica, objCajaChica.t_FechaRegistro, dtpFechaRegistro.Value, objCajaChica.v_Correlativo, int.Parse(cboTipoDocumento.Value.ToString()));
            cboEstado.Value = "1";
            txtResponsable.Text = Globals.ClientSession.v_UserName;
        }

        private void CargarDetalle(string IdCajaChica)
        {
            OperationResult objOperationResult = new OperationResult();
            grdData.DataSource = _objCajaChicaBL.ObtenerDetallesCajaChica(ref objOperationResult, IdCajaChica);
            if (objOperationResult.Success == 0)
            {
                UltraMessageBox.Show("Hubo un error al cargar detalles caja chica", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            for (int i = 0; i < grdData.Rows.Count(); i++)
            {
                grdData.Rows[i].Cells["i_RegistroTipo"].Value = "NoTemporal";

            }
        }
        private void CargarCabecera(string IdCajaChica)
        {
            _Mode = "Edit";
            objCajaChica = new cajachicaDto();
            OperationResult objOperationResult = new OperationResult();
            objCajaChica = _objCajaChicaBL.ObtenerCabeceraCajaChica(ref objOperationResult, IdCajaChica);
            if (objCajaChica != null)
            {

                dtpFechaRegistro.Value = objCajaChica.t_FechaRegistro.Value;
                txtSaldoCaja.Text = objCajaChica.d_CajaSaldo.ToString();
                txtTotalIngresos.Text = objCajaChica.d_TotalIngresos.ToString();
                txtTotalGastos.Text = objCajaChica.d_TotalGastos.ToString();
                txtPeriodo.Text = objCajaChica.v_Periodo.Trim();
                txtMes.Text = objCajaChica.v_Mes.Trim();
                txtCorrelativo.Text = objCajaChica.v_Correlativo.Trim();
                cboEstado.Value = objCajaChica.i_IdEstado.Value.ToString();
                cboTipoDocumento.Value = objCajaChica.i_IdTipoDocumento.Value.ToString();

                txtResponsable.Text = _objCajaChicaBL.BuscarResponsable(objCajaChica.i_InsertaIdUsuario.Value);
                txtTipoCambio.Text = objCajaChica.d_TipoCambio.Value.ToString();
                cboMoneda.Value = objCajaChica.i_IdMoneda.Value.ToString();
                CargarDetalle(objCajaChica.v_IdCajaChica);
            }
            else
            {
                UltraMessageBox.Show("Hubo un error al cargar caja chica", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void LlenarTemporales()
        {
            try
            {


                if (!grdData.Rows.Any()) return;
                foreach (var fila in grdData.Rows)
                {
                    switch (fila.Cells["i_RegistroTipo"].Value.ToString())
                    {
                        case "Temporal":
                            if (fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {
                                var _objOrdenTrabajoDetalle = (cajachicadetalleDto)fila.ListObject;

                                _TempDetalle_AgregarDto.Add(_objOrdenTrabajoDetalle);
                            }
                            break;

                        case "NoTemporal":
                            if (fila.Cells["i_RegistroEstado"].Value != null && fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {
                                var _objOrdenTrabajoDetalle = (cajachicadetalleDto)fila.ListObject;
                                _TempDetalle_ModificarDto.Add(_objOrdenTrabajoDetalle);

                            }
                            break;
                    }
                }
            }

            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message + "Linea: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
            }

        }
        private void ValidarFechas()
        {

            if (DateTime.Now.Year.ToString().Trim() == txtPeriodo.Text.Trim())
            {
                if (strModo == "Nuevo")
                {
                    dtpFechaRegistro.Value = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                    dtpFechaRegistro.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                    dtpFechaRegistro.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(txtMes.Text.Trim()))).ToString()).ToString());
                    dtpFechaRegistro.Value = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                    //dtpFechaRegistro.MinDate = dtpFechaEmision.Value;
                    dtpFechaRegistro.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                    dtpFechaRegistro.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(txtMes.Text.Trim()))).ToString()).ToString());


                }
                else
                {
                    if (int.Parse(objCajaChica.v_Mes.Trim()) <= int.Parse(DateTime.Now.Month.ToString()))
                    {
                        dtpFechaRegistro.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(DateTime.Now.Month.ToString()))).ToString()).ToString());
                        dtpFechaRegistro.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(DateTime.Now.Month.ToString()))).ToString()).ToString());

                    }

                    dtpFechaRegistro.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                    dtpFechaRegistro.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());

                }


            }
            else
            {
                if (strModo == "Nuevo")
                {
                    dtpFechaRegistro.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                    dtpFechaRegistro.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                }

                dtpFechaRegistro.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                dtpFechaRegistro.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), 12)).ToString()).ToString());
                //dtpFechaRegistro.MinDate = dtpFechaEmision.Value;
                dtpFechaRegistro.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                dtpFechaRegistro.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), 12)).ToString()).ToString());

            }
        }

        #endregion


        #region Comportamiento-Controles

        private void dtpFechaRegistro_ValueChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            txtPeriodo.Text = dtpFechaRegistro.Value.Year.ToString();
            txtMes.Text = dtpFechaRegistro.Value.Month.ToString("00");
            txtCorrelativo.Text = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult, ListaProcesos.CajaChica, objCajaChica.t_FechaRegistro, dtpFechaRegistro.Value, objCajaChica.v_Correlativo, int.Parse(cboTipoDocumento.Value.ToString()));
            txtTipoCambio.Text = _objTesoreriaBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaRegistro.Value.Date);
            #region Controla Cierre de Mes
            if (new CierreMensualBL().VerificarMesCerrado(txtPeriodo.Text, txtMes.Text, (int)ModulosSistema.Tesoreria))
            {
                btnGuardar.Visible = false;
                this.Text = "Caja Chica [MES CERRADO]";
            }
            else
            {
                btnGuardar.Visible = true;
                this.Text = "Caja Chica";
            }
            #endregion
        }



        private void dtpFechaRegistro_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)

                dtpFechaRegistro.Focus();
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
                        ObtenerListadoCajaChica();
                    }
                    else if (strModo == "Guardado")
                    {
                        strModo = "Consulta";
                        ObtenerListadoCajaChica();
                    }
                    else
                    {
                        ObtenerListadoCajaChica();
                    }

                    // txtSerieDoc.Focus();
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
                            ObtenerListadoCajaChica();
                        }
                        else if (strModo == "Guardado")
                        {
                            strModo = "Consulta";
                            ObtenerListadoCajaChica();
                        }
                        else
                        {
                            ObtenerListadoCajaChica();
                        }

                        //txtSerieDoc.Focus();
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

        #endregion

        #region CRUD
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();

            if (uvCajaChica.Validate(true, false).IsValid)
            {
                if (!grdData.Rows.Any())
                {
                    UltraMessageBox.Show("No se permite guardar mientras el detalle está vacío", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (String.IsNullOrEmpty(txtTipoCambio.Text) || decimal.Parse(txtTipoCambio.Text) <= 0)
                {
                    UltraMessageBox.Show("Ingrese un tipo de cambio correcto", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (objCajaChica.v_IdCajaChica == null)
                {
                    if (_objCajaChicaBL.VerificarSiUsuarioTieneCajaAbiertaFecha(Globals.ClientSession.GetAsList(), dtpFechaRegistro.Value.Date))
                    {
                        UltraMessageBox.Show("Ya existe una caja aperturada para este dia  por este responsable.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;

                    }
                }
                else
                {
                    if (_objCajaChicaBL.VerificarSiUsuarioTieneCajaAbiertaFechaEditado(Globals.ClientSession.GetAsList(), dtpFechaRegistro.Value.Date, objCajaChica.v_IdCajaChica))
                    {
                        UltraMessageBox.Show("Ya existe una caja aperturada para este dia  por este responsable.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (cboEstado.Value != null && cboEstado.Value.ToString() == "1")
                    {
                        if (_objCajaChicaBL.VerificarSiUsuarioTieneCajaAbiertaFechaPosterior(Globals.ClientSession.GetAsList(), dtpFechaRegistro.Value.Date, objCajaChica.v_IdCajaChica))
                        {
                            UltraMessageBox.Show("Ya existe una caja aperturada  con fechas posteriores , que utilizaron el saldo de ésta caja", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                }


                if (ValidaCamposNulosVacios())
                {

                    if (cboEstado.Value.ToString() == "0")
                    {
                        if (UltraMessageBox.Show("Al cerrar la caja ya no se podra realizar ningún movimiento.¿Desea Continuar?", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                        {
                            return;

                        }

                    }

                    if (_Mode == "New")
                    {

                        while (!_objCajaChicaBL.ExisteNroRegistro(int.Parse(cboTipoDocumento.Value.ToString()), txtPeriodo.Text, txtMes.Text.Trim(), txtCorrelativo.Text))
                        {
                            txtCorrelativo.Text = (int.Parse(txtCorrelativo.Text) + 1).ToString("00000000");
                        }


                        objCajaChica = new cajachicaDto
                        {
                            v_Periodo = txtPeriodo.Text.Trim(),
                            v_Mes = txtMes.Text.Trim(),
                            i_IdTipoDocumento = int.Parse(cboTipoDocumento.Value.ToString()),
                            t_FechaRegistro = dtpFechaRegistro.Value.Date,
                            i_IdEstado = int.Parse(cboEstado.Value.ToString()),
                            v_Correlativo = txtCorrelativo.Text.Trim(),
                            d_TotalGastos = string.IsNullOrEmpty(txtTotalGastos.Text) ? 0 : decimal.Parse(txtTotalGastos.Text),
                            d_TotalIngresos = string.IsNullOrEmpty(txtTotalIngresos.Text) ? 0 : decimal.Parse(txtTotalIngresos.Text),
                            d_CajaSaldo = string.IsNullOrEmpty(txtSaldoCaja.Text) ? 0 : decimal.Parse(txtSaldoCaja.Text),
                            d_TipoCambio = decimal.Parse(txtTipoCambio.Text),
                            i_IdMoneda = int.Parse(cboMoneda.Value.ToString()),


                        };
                        LlenarTemporales();
                        newIdCajaChica = _objCajaChicaBL.InsertarCajaChica(ref objOperationResult, objCajaChica, Globals.ClientSession.GetAsList(), _TempDetalle_AgregarDto);


                    }
                    else if (_Mode == "Edit")
                    {

                        objCajaChica.v_Periodo = txtPeriodo.Text;
                        objCajaChica.v_Mes = txtMes.Text;
                        objCajaChica.v_Correlativo = txtCorrelativo.Text;
                        objCajaChica.i_IdTipoDocumento = int.Parse(cboTipoDocumento.Value.ToString());
                        objCajaChica.t_FechaRegistro = dtpFechaRegistro.Value.Date;
                        objCajaChica.i_IdEstado = int.Parse(cboEstado.Value.ToString());
                        objCajaChica.d_TotalGastos = decimal.Parse(txtTotalGastos.Text);
                        objCajaChica.d_TotalIngresos = decimal.Parse(txtTotalIngresos.Text);
                        objCajaChica.d_CajaSaldo = decimal.Parse(txtSaldoCaja.Text);
                        objCajaChica.d_TipoCambio = decimal.Parse(txtTipoCambio.Text);
                        objCajaChica.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());
                        LlenarTemporales();
                        _objCajaChicaBL.ActualizarCajaChica(ref objOperationResult, objCajaChica, Globals.ClientSession.GetAsList(), _TempDetalle_AgregarDto, _TempDetalle_ModificarDto, _TempDetalle_EliminarDto);
                    }

                    if (objOperationResult.Success == 1)
                    {
                        _Mode = "Edit";
                        CargarCabecera(newIdCajaChica);
                        _pstrIdCajaChica_Nuevo = newIdCajaChica;
                        //strModo = "Guardado";
                        //EdicionBarraNavegacion(true);
                        //strIdReciboHonorario = newIdRecibo;
                        //ObtenerListadoReciboHonorarios(txtPeriodo.Text, txtMes.Text);
                        //_pstrIdMovimiento_Nuevo = newIdRecibo;
                        linkAsiento.Visible = true;
                        UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema", MessageBoxButtons.OK);
                        //if (UltraMessageBox.Show("El registro se ha guardado correctamente.¿Desea Generar  Nueva Caja Chica?", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
                        //{
                        //    NuevaCajaChica();

                        //}
                    }
                    else
                    {
                        UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    _TempDetalle_AgregarDto = new List<cajachicadetalleDto>();
                    _TempDetalle_ModificarDto = new List<cajachicadetalleDto>();
                    _TempDetalle_EliminarDto = new List<cajachicadetalleDto>();
                }
            }
        }

        private bool ValidaCamposNulosVacios()
        {
            if (grdData.Rows.Count(p => p.Cells["v_Usuario"].Value == null || p.Cells["v_Usuario"].Value.ToString().Trim() == string.Empty) != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente los usuarios", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // UltraStatusbarManager.MarcarError(ultraStatusBar1, "Por favor ingrese correctamente todas las cuentas al detalle.", timer1);
                UltraGridRow Row = grdData.Rows.FirstOrDefault(x => x.Cells["v_Usuario"].Value == null || x.Cells["v_Usuario"].Value.ToString().Trim() == string.Empty);
                grdData.Selected.Cells.Add(Row.Cells["v_Usuario"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["v_Usuario"];
                this.grdData.ActiveCell = aCell;
                return false;
            }

            if (grdData.Rows.Count(p => p.Cells["i_IdConceptosCajaChica"].Value == null || p.Cells["i_IdConceptosCajaChica"].Value.ToString().Trim() == "-1") != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente la descripción de los Motivos", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // UltraStatusbarManager.MarcarError(ultraStatusBar1, "Por favor ingrese correctamente todas las cuentas al detalle.", timer1);
                UltraGridRow Row = grdData.Rows.FirstOrDefault(x => x.Cells["i_IdConceptosCajaChica"].Value == null || x.Cells["i_IdConceptosCajaChica"].Value.ToString().Trim() == "-1");
                grdData.Selected.Cells.Add(Row.Cells["i_IdConceptosCajaChica"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["i_IdConceptosCajaChica"];
                this.grdData.ActiveCell = aCell;
                return false;
            }


            if (grdData.Rows.Count(p => p.Cells["d_Importe"].Value == null || p.Cells["d_Importe"].Value.ToString().Trim() == "" || decimal.Parse(p.Cells["d_Importe"].Value.ToString().Trim()) == 0) != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente los importes", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // UltraStatusbarManager.MarcarError(ultraStatusBar1, "Por favor ingrese correctamente todas las cuentas al detalle.", timer1);
                UltraGridRow Row = grdData.Rows.FirstOrDefault(x => x.Cells["d_Importe"].Value == null || x.Cells["d_Importe"].Value.ToString().Trim() == "" || decimal.Parse(x.Cells["d_Importe"].Value.ToString().Trim()) == 0);
                grdData.Selected.Cells.Add(Row.Cells["d_Importe"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["d_Importe"];
                this.grdData.ActiveCell = aCell;
                return false;
            }

            if (grdData.Rows.Count(p => p.Cells["i_Motivo"].Value == null || p.Cells["i_Motivo"].Value.ToString().Trim() == "" || p.Cells["i_Motivo"].Value.ToString().Trim() == "-1") != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente el Motivo", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // UltraStatusbarManager.MarcarError(ultraStatusBar1, "Por favor ingrese correctamente todas las cuentas al detalle.", timer1);
                UltraGridRow Row = grdData.Rows.FirstOrDefault(x => x.Cells["i_Motivo"].Value == null || x.Cells["i_Motivo"].Value.ToString().Trim() == "" || x.Cells["i_Motivo"].Value.ToString().Trim() == "-1");
                grdData.Selected.Cells.Add(Row.Cells["i_Motivo"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["i_Motivo"];
                this.grdData.ActiveCell = aCell;
                return false;
            }




            if (grdData.Rows.Count(p => p.Cells["i_RequiereTipoDocumento"].Value.ToString ()=="0" &&  p.Cells["i_IdTipoDocumento"].Value != null && p.Cells["i_IdTipoDocumento"].Value.ToString() != "-1")!=0)  // p.Cells["i_Motivo"].Value.ToString().Trim() == "" || p.Cells["i_Motivo"].Value.ToString().Trim() == "-1") != 0)
            {
                UltraMessageBox.Show("Este Motivo no necesita Tipo Documento asociado", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);

                UltraGridRow Row = grdData.Rows.FirstOrDefault(x => x.Cells["i_RequiereTipoDocumento"].Value.ToString() == "0" && x.Cells["i_IdTipoDocumento"].Value != null && x.Cells["i_IdTipoDocumento"].Value.ToString()!= "-1");
                grdData.Selected.Cells.Add(Row.Cells["i_IdConceptosCajaChica"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["i_IdConceptosCajaChica"];
                this.grdData.ActiveCell = aCell;
                return false;
            }


            if (grdData.Rows.Count(p => p.Cells["i_RequiereTipoDocumento"].Value.ToString() == "1" && p.Cells["i_IdTipoDocumento"].Value.ToString ()=="-1") != 0)  // p.Cells["i_Motivo"].Value.ToString().Trim() == "" || p.Cells["i_Motivo"].Value.ToString().Trim() == "-1") != 0)
            {
                UltraMessageBox.Show("Debe asociar Tipo Documento para este motivo", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);

                UltraGridRow Row = grdData.Rows.FirstOrDefault(x => x.Cells["i_RequiereTipoDocumento"].Value.ToString() == "1" && x.Cells["i_IdTipoDocumento"].Value.ToString() == "-1");
                grdData.Selected.Cells.Add(Row.Cells["i_IdConceptosCajaChica"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["i_IdConceptosCajaChica"];
                this.grdData.ActiveCell = aCell;
                return false;
            }


            if (grdData.Rows.Count(p => p.Cells["i_RequiereAnexo"].Value.ToString() == "0" && p.Cells["v_IdCliente"].Value != null && p.Cells["v_IdCliente"].Value.ToString() != "") != 0)  // p.Cells["i_Motivo"].Value.ToString().Trim() == "" || p.Cells["i_Motivo"].Value.ToString().Trim() == "-1") != 0)
            {
                UltraMessageBox.Show("Este Motivo no necesita Anexo", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);

                UltraGridRow Row = grdData.Rows.FirstOrDefault(x => x.Cells["i_RequiereAnexo"].Value.ToString() == "0" && x.Cells["v_IdCliente"].Value != null &&  x.Cells["v_IdCliente"].Value.ToString() != "");
                grdData.Selected.Cells.Add(Row.Cells["i_IdConceptosCajaChica"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["i_IdConceptosCajaChica"];
                this.grdData.ActiveCell = aCell;
                return false;
            }


            if (grdData.Rows.Count(p => p.Cells["i_RequiereAnexo"].Value.ToString() == "1" && p.Cells["v_IdCliente"].Value==null) != 0)  // p.Cells["i_Motivo"].Value.ToString().Trim() == "" || p.Cells["i_Motivo"].Value.ToString().Trim() == "-1") != 0)
            {
                UltraMessageBox.Show("Debe asociar Tipo Anexo para este motivo", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);

                UltraGridRow Row = grdData.Rows.FirstOrDefault(x => x.Cells["i_RequiereAnexo"].Value.ToString() == "1" && x.Cells["v_IdCliente"].Value==null );
                grdData.Selected.Cells.Add(Row.Cells["i_IdConceptosCajaChica"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["i_IdConceptosCajaChica"];
                this.grdData.ActiveCell = aCell;
                return false;
            }



            if (grdData.Rows.Count(p => p.Cells["i_RequiereNumeroDocumento"].Value.ToString() == "0" && p.Cells["v_NroDocumento"].Value != null && p.Cells["v_NroDocumento"].Value.ToString() != "") != 0)  // p.Cells["i_Motivo"].Value.ToString().Trim() == "" || p.Cells["i_Motivo"].Value.ToString().Trim() == "-1") != 0)
            {
                UltraMessageBox.Show("Este Motivo no necesita Número Documento", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);

                UltraGridRow Row = grdData.Rows.FirstOrDefault(x => x.Cells["i_RequiereNumeroDocumento"].Value.ToString() == "0" && x.Cells["v_NroDocumento"].Value != null && x.Cells["v_NroDocumento"].Value.ToString() != "");
                grdData.Selected.Cells.Add(Row.Cells["i_IdConceptosCajaChica"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["i_IdConceptosCajaChica"];
                this.grdData.ActiveCell = aCell;
                return false;
            }


            if (grdData.Rows.Count(p => p.Cells["i_RequiereNumeroDocumento"].Value.ToString() == "1" && p.Cells["v_NroDocumento"].Value == null) != 0)  // p.Cells["i_Motivo"].Value.ToString().Trim() == "" || p.Cells["i_Motivo"].Value.ToString().Trim() == "-1") != 0)
            {
                UltraMessageBox.Show("Debe asociar Número Documento para este motivo", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);

                UltraGridRow Row = grdData.Rows.FirstOrDefault(x => x.Cells["i_RequiereNumeroDocumento"].Value.ToString() == "1" && x.Cells["v_NroDocumento"].Value == null);
                grdData.Selected.Cells.Add(Row.Cells["i_IdConceptosCajaChica"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["i_IdConceptosCajaChica"];
                this.grdData.ActiveCell = aCell;
                return false;
            }



            foreach (var item in grdData.Rows)
            {
                if (item.Cells["i_Motivo"].Value.ToString() == "0" && item.Cells["i_IdConceptosCajaChica"].Value.ToString() == "1")
                {
                    UltraMessageBox.Show("No se puede asignar a una salida un motivo de apertura", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UltraGridRow Row = grdData.Rows.FirstOrDefault (x=>x.Index == item.Index);
                    grdData.Selected.Cells.Add(Row.Cells["i_Motivo"]);
                    grdData.Focus();
                    Row.Activate();
                    grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                    UltraGridCell aCell = this.grdData.ActiveRow.Cells["i_Motivo"];
                    this.grdData.ActiveCell = aCell;
                    return false;
                }

                if ((item.Cells["i_IdTipoDocumento"].Value != null && item.Cells["i_IdTipoDocumento"].Value.ToString() != "-1" && (item.Cells["v_NroDocumento"].Value == null || item.Cells["v_NroDocumento"].Value.ToString().Trim() == "")) || (item.Cells["i_IdTipoDocumento"].Value == null && item.Cells["i_IdTipoDocumento"].Value.ToString() == "-1" && (item.Cells["v_NroDocumento"].Value != null || item.Cells["v_NroDocumento"].Value.ToString().Trim() != "")))
                {
                    UltraMessageBox.Show("Los tipos de documentos estan ingresados de forma incorrecta", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    UltraGridRow Row = grdData.Rows.FirstOrDefault(x => x.Index == item.Index);
                    grdData.Selected.Cells.Add(Row.Cells["v_NroDocumento"]);
                    grdData.Focus();
                    Row.Activate();
                    grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                    UltraGridCell aCell = this.grdData.ActiveRow.Cells["v_NroDocumento"];
                    this.grdData.ActiveCell = aCell;
                    return false;
                }
              

                if (item.Cells["i_RequiereNumeroDocumento"].Value.ToString() == "0")
                {

                    if (item.Cells["v_NroDocumento"].Value != null && item.Cells["v_NroDocumento"].Value.ToString().Trim () != "")
                    {
                        UltraMessageBox.Show("Este Motivo no necesita Numero Documento", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        UltraGridRow Row = grdData.Rows.FirstOrDefault(x => x.Index == item.Index);
                        grdData.Selected.Cells.Add(Row.Cells["v_NroDocumento"]);
                        grdData.Focus();
                        Row.Activate();
                        grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                        UltraGridCell aCell = this.grdData.ActiveRow.Cells["v_NroDocumento"];
                        this.grdData.ActiveCell = aCell;
                        
                        return false;
                    }
                }
                else
                {
                    if (item.Cells["v_NroDocumento"].Value == null || item.Cells["v_NroDocumento"].Value.ToString().Trim() == "")
                    {
                        UltraMessageBox.Show("Debe asociar  Numero Documento para este motivo", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        UltraGridRow Row = grdData.Rows.FirstOrDefault(x => x.Index == item.Index);
                        grdData.Selected.Cells.Add(Row.Cells["v_NroDocumento"]);
                        grdData.Focus();
                        Row.Activate();
                        grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                        UltraGridCell aCell = this.grdData.ActiveRow.Cells["v_NroDocumento"];
                        this.grdData.ActiveCell = aCell;
                        return false;
                    }

                }

            }
            return true;
        }

        private void NuevaCajaChica()
        {
            strModo = "Nuevo";
            _Mode = "New";
            LimpiarCabecera();
            cboTipoDocumento.Enabled = true;
            cboTipoDocumento.PerformAction(UltraComboAction.Dropdown);
            ObtenerListadoCajaChica();
        }


        #endregion


        #region Clases/Validaciones
        //private bool ValidaCamposNulosVacios()
        //{
        //foreach (var Fila in grdData.Rows)
        //{
        //    string Cuenta = Fila.Cells["v_NroCuenta"].Value.ToString().Trim();

        //    //if (!_objReciboHonorariosBL.ExistenciaCuentaImputable(Cuenta))
        //    //{

        //    //    UltraMessageBox.Show("Por favor ingrese Nro. de Cuenta válida ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    //    grdData.Selected.Cells.Add(Fila.Cells["v_NroCuenta"]);
        //    //    grdData.Focus();
        //    //    Fila.Activate();
        //    //    grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
        //    //    UltraGridCell aCell = this.grdData.ActiveRow.Cells["v_NroCuenta"];
        //    //    this.grdData.ActiveCell = aCell;
        //    //    return false;
        //    //}

        //    string CentroCosto = Fila.Cells["i_CCosto"].Value.ToString().Trim();
        //    if (Fila.Cells["i_ValidarCentroCosto"].Value != null && Fila.Cells["i_ValidarCentroCosto"].Value.ToString() == "1" && !_objDatahierarchyBL.EsValidoDtahierarchy(CentroCosto, 31))
        //    {
        //        UltraMessageBox.Show("Por favor ingrese Centro Costo Válido ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        grdData.Selected.Cells.Add(Fila.Cells["i_CCosto"]);
        //        grdData.Focus();
        //        Fila.Activate();
        //        grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
        //        UltraGridCell aCell = this.grdData.ActiveRow.Cells["i_CCosto"];
        //        this.grdData.ActiveCell = aCell;
        //        return false;
        //    }

        //}

        //if (grdData.Rows.Where(p => p.Cells["v_NroCuenta"].Value == null || p.Cells["v_NroCuenta"].Value.ToString().Trim() == string.Empty).Count() != 0)
        //{
        //    UltraMessageBox.Show("Por favor ingrese correctamente las Cuentas", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    UltraGridRow Row = grdData.Rows.Where(x => x.Cells["v_NroCuenta"].Value == null || x.Cells["v_NroCuenta"].Value.ToString().Trim() == string.Empty).FirstOrDefault();
        //    grdData.Selected.Cells.Add(Row.Cells["v_NroCuenta"]);
        //    grdData.Focus();
        //    Row.Activate();
        //    grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
        //    UltraGridCell aCell = this.grdData.ActiveRow.Cells["v_NroCuenta"];
        //    this.grdData.ActiveCell = aCell;
        //    return false;
        //}

        //if (grdData.Rows.Where((p => (p.Cells["i_CCosto"].Value == null || p.Cells["i_CCosto"].Value.ToString().Trim() == "-1" || p.Cells["i_CCosto"].Value.ToString().Trim() == string.Empty) && (p.Cells["i_ValidarCentroCosto"].Value != null && p.Cells["i_ValidarCentroCosto"].Value.ToString() == "1"))).Count() != 0)
        //{
        //    UltraMessageBox.Show("Por favor ingrese correctamente los Centros de Costo", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    UltraGridRow Row = grdData.Rows.Where(x => (x.Cells["i_CCosto"].Value == null || x.Cells["i_CCosto"].Value.ToString().Trim() == "-1" || x.Cells["i_CCosto"].Value.ToString().Trim() == string.Empty) && (x.Cells["i_ValidarCentroCosto"].Value != null && x.Cells["i_ValidarCentroCosto"].Value.ToString() == "1")).FirstOrDefault();
        //    grdData.Selected.Cells.Add(Row.Cells["i_CCosto"]);
        //    grdData.Focus();
        //    Row.Activate();
        //    grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
        //    UltraGridCell aCell = this.grdData.ActiveRow.Cells["i_CCosto"];
        //    this.grdData.ActiveCell = aCell;
        //    return false;
        //}
        //if (grdData.Rows.Where((p => (p.Cells["i_CCosto"].Value != null && p.Cells["i_CCosto"].Value.ToString().Trim() != string.Empty) && (p.Cells["i_ValidarCentroCosto"].Value != null && p.Cells["i_ValidarCentroCosto"].Value.ToString() == "0"))).Count() != 0)
        //{
        //    UltraMessageBox.Show("La configuración de la  cuenta no tiene centro de costo", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    UltraGridRow Row = grdData.Rows.Where(x => (x.Cells["i_CCosto"].Value != null || x.Cells["i_CCosto"].Value.ToString().Trim() != "-1" || x.Cells["i_CCosto"].Value.ToString().Trim() != string.Empty) && (x.Cells["i_ValidarCentroCosto"].Value != null && x.Cells["i_ValidarCentroCosto"].Value.ToString() == "0")).FirstOrDefault();
        //    grdData.Selected.Cells.Add(Row.Cells["i_CCosto"]);
        //    grdData.Focus();
        //    Row.Activate();
        //    grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
        //    UltraGridCell aCell = this.grdData.ActiveRow.Cells["i_CCosto"];
        //    this.grdData.ActiveCell = aCell;
        //    return false;
        //}

        //return true;
        // }

        private bool ValidarCuentasGeneracionLibro()
        {

            string CuartaCategoria = int.Parse(((int)Concepto.CuartaCategoria).ToString()) < 10 ? ("0" + ((int)Concepto.CuartaCategoria).ToString()).Trim() : (((int)Concepto.CuartaCategoria).ToString()).Trim();
            string PorPagarSoles = int.Parse(((int)Concepto.PorPagarSoles).ToString()) < 10 ? ("0" + ((int)Concepto.PorPagarSoles).ToString()).Trim() : (((int)Concepto.PorPagarSoles).ToString()).Trim();
            string PorPagarDolares = int.Parse(((int)Concepto.PorPagarDolares).ToString()) < 10 ? ("0" + ((int)Concepto.PorPagarDolares).ToString()).Trim() : (((int)Concepto.PorPagarDolares).ToString()).Trim();
            return true;

        }




        #endregion


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


           
            }
        }

        private void CargarReciboHonorario(string pIdReciboHonorario)
        {
            CargarCabecera(pIdReciboHonorario);
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

        private void cboTipoDocumento_KeyUp(object sender, KeyEventArgs e)
        {
            foreach (var row in cboTipoDocumento.Rows)
            {
                if ((string)cboTipoDocumento.Value == "-1") cboTipoDocumento.Text = string.Empty;
                var filterRow =
                    cboTipoDocumento.DisplayLayout.Bands[0].Columns.Cast<UltraGridColumn>()
                        .Where(column => column.IsVisibleInLayout)
                        .All(column => !row.Cells[column].Text.Contains(cboTipoDocumento.Text.ToUpper()));
                row.Hidden = filterRow;
            }
        }

        private void cboTipoDocumento_Leave(object sender, EventArgs e)
        {
            if (strModo == "Edicion") return;
            if (cboTipoDocumento.Text.Trim() == "")
            {
                cboTipoDocumento.Value = "-1";
            }
            else
            {
                var x = _ListadoComboDocumentos.Find(p => p.Id == cboTipoDocumento.Value.ToString() || p.Id == cboTipoDocumento.Text);
                if (x == null)
                {
                    cboTipoDocumento.Value = "-1";
                }
            }
            if (cboTipoDocumento.Value != null && cboTipoDocumento.Value.ToString() != "-1")
                ObtenerCorrelativoCobranza(int.Parse(cboTipoDocumento.Value.ToString()));
            txtCorrelativo.Enabled = true;
        }
        private void ObtenerCorrelativoCobranza(int IdDocumento)
        {
            OperationResult objOperationResult = new OperationResult();
            txtCorrelativo.Text = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult, ListaProcesos.CajaChica, objCajaChica.t_FechaRegistro, dtpFechaRegistro.Value, objCajaChica.v_Correlativo, int.Parse(cboTipoDocumento.Value.ToString()));
            if (strModo == "Nuevo") DevuelveCuentaCaja(IdDocumento);
        }

        private void DevuelveCuentaCaja(int IdDocumento)
        {
            try
            {
                var objOperationResult = new OperationResult();
                var cadena = _objTesoreriaBL.DevuelveCuentaCajaBanco(ref objOperationResult, IdDocumento);
                if (objOperationResult.Success == 0)
                {
                    btnGuardar.Enabled = false;
                    if (objOperationResult.ErrorMessage != null || objOperationResult.ExceptionMessage != null)
                    {
                        UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\n" + objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show(objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
                else
                {
                    btnGuardar.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message + "\nLinea: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
            }

        }




        private bool EsCuentaApertura(int Motivo, int DescripcionMotivo, int Index)
        {

            if (Motivo == 1 && DescripcionMotivo == 1 && Index == 1)
                return true;
            else return false;


        }

        private void grdData_ClickCell(object sender, ClickCellEventArgs e)
        {
            if (grdData.ActiveRow == null) return;

            switch (e.Cell.Column.Key)
            {

                case "i_Motivo":

                    if (EsCuentaApertura(grdData.Rows[grdData.ActiveRow.Index].Cells["i_Motivo"].Value == null ? -1 : int.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["i_Motivo"].Value.ToString()), grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdConceptosCajaChica"].Value == null ? 0 : int.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdConceptosCajaChica"].Value.ToString()), grdData.Rows[grdData.ActiveRow.Index].Cells["Index"].Value == null ? -5 : int.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["Index"].Value.ToString())))
                    {
                        e.Cell.CancelUpdate();
                    }
                    break;
                case "i_IdConceptosCajaChica":
                    if (EsCuentaApertura(grdData.Rows[grdData.ActiveRow.Index].Cells["i_Motivo"].Value == null ? -1 : int.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["i_Motivo"].Value.ToString()), grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdConceptosCajaChica"].Value == null ? 0 : int.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdConceptosCajaChica"].Value.ToString()), grdData.Rows[grdData.ActiveRow.Index].Cells["Index"].Value == null ? -5 : int.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["Index"].Value.ToString())))
                    {

                        e.Cell.CancelUpdate();
                    }
                    
                    break;

                case "i_IdTipoDocumento":
                    if ( grdData.Rows[grdData.ActiveRow.Index].Cells["i_RequiereTipoDocumento"].Value.ToString() == "0" )
                    {
                        e.Cell.CancelUpdate();
                    }

                    break; 
                case "CodigoAnexo":
                    OperationResult objOperationResult = new OperationResult();
                    string IdCliente = grdData.ActiveRow.Cells["v_IdCliente"].Value != null ? grdData.ActiveRow.Cells["v_IdCliente"].Value.ToString() : string.Empty;
                    if (!string.IsNullOrEmpty(IdCliente))
                    {
                        string[] Cadena = new string[4];
                        Cadena = new VentaBL().DevolverClientePorIdCliente(ref objOperationResult, IdCliente);
                        txtDescripcion.Text = Cadena != null ? Cadena[2] : string.Empty;
                    }
                    else
                    {
                        txtDescripcion.Clear();
                    }
                    break;
                   
                default:
                    txtDescripcion.Clear();
                    break;
            }
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {

            Reportes.Ventas.frmDocumentoCajChica frm = new Reportes.Ventas.frmDocumentoCajChica(newIdCajaChica);
             frm.ShowDialog();
        }






    }
}
