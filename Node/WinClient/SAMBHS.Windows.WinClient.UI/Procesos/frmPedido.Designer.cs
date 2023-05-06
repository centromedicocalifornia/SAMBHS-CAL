using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    partial class frmPedido
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_NroSerie");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_NroLote");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("i_SolicitarNroSerie");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("i_SolicitarNroLote");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("t_FechaCaducidad");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_NroPedido");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("i_RegistroTipo", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("i_RegistroEstado", 1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_NombreProducto", 2);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn15 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Empaque", 3);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn16 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("UMEmpaque", 4);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn17 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("i_IdAlmacen", 5);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn18 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("d_Cantidad", 6);
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn19 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("i_IdUnidadMedida", 7);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn20 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("d_PrecioUnitario", 8);
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPedido));
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn21 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("i_NroUnidades", 9);
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn22 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_Observacion", 10);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn23 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_IdProductoAlmacen", 11);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn24 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_IdPedido", 12);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn25 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("d_SeparacionTotal", 13);
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn26 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("d_StockActual", 14);
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn27 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("i_EsAfectoDetraccion", 15);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn28 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("i_EsServicio", 16);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn29 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_CodigoInterno", 17);
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn30 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("d_DescuentoLP", 18);
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn31 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Index", 19);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn32 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("EsNombreEditable", 20);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn33 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("EsRedondeo", 21);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn70 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("d_Valor", 22);
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn71 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("d_Descuento", 23);
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn72 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("d_ValorVenta", 24);
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn73 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("d_Igv", 25);
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn74 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("d_PrecioVenta", 26);
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn75 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("i_ValidarStock", 27);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn76 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("i_IdUnidadMedidaProducto", 28);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn77 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("d_CantidadEmpaque", 29);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn78 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("i_PrecioEditable", 30);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn79 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_Descuento", 31);
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn80 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("i_IdTipoOperacion", 32);
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance28 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance29 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance30 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance31 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance32 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance33 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand2 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn67 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Id");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn68 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Value1");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn69 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Value2");
            Infragistics.Win.Appearance appearance34 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance35 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance36 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance37 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance38 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance39 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance40 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance41 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance42 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance43 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance44 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance45 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance46 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance47 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance48 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance49 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance50 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance51 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance52 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance53 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance54 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance55 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance56 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance57 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance58 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance59 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance60 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance61 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance62 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance63 = new Infragistics.Win.Appearance();
            this.ucTipoOperacion = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.panel1 = new Infragistics.Win.Misc.UltraPanel();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.cboIGV = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.label21 = new Infragistics.Win.Misc.UltraLabel();
            this.btnNuevoMovimiento = new Infragistics.Win.Misc.UltraButton();
            this.cboEstablecimiento = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.label44 = new Infragistics.Win.Misc.UltraLabel();
            this.btnSiguiente = new Infragistics.Win.Misc.UltraButton();
            this.btnAnterior = new Infragistics.Win.Misc.UltraButton();
            this.txtCorrelativo = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label16 = new Infragistics.Win.Misc.UltraLabel();
            this.txtMes = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label15 = new Infragistics.Win.Misc.UltraLabel();
            this.txtPeriodo = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label4 = new Infragistics.Win.Misc.UltraLabel();
            this.groupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            this.btnLiberarSeparacion = new Infragistics.Win.Misc.UltraButton();
            this.panelListaPrecios = new Infragistics.Win.Misc.UltraPanel();
            this.grdData = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.btnRedondear = new Infragistics.Win.Misc.UltraButton();
            this.btnEliminar = new Infragistics.Win.Misc.UltraButton();
            this.btnAgregar = new Infragistics.Win.Misc.UltraButton();
            this.txtCantidadTotal = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label18 = new Infragistics.Win.Misc.UltraLabel();
            this.label2 = new Infragistics.Win.Misc.UltraLabel();
            this.txtSerieDoc = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtNumeroDoc = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.dtpFechaEmision = new System.Windows.Forms.DateTimePicker();
            this.label25 = new Infragistics.Win.Misc.UltraLabel();
            this.label24 = new Infragistics.Win.Misc.UltraLabel();
            this.label3 = new Infragistics.Win.Misc.UltraLabel();
            this.txtDiasVigencia = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.dtpFechaVencimiento = new System.Windows.Forms.DateTimePicker();
            this.cboEstado = new Infragistics.Win.Misc.UltraLabel();
            this.label6 = new Infragistics.Win.Misc.UltraLabel();
            this.cboCondicionPago = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.label8 = new Infragistics.Win.Misc.UltraLabel();
            this.label10 = new Infragistics.Win.Misc.UltraLabel();
            this.cboMoneda = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.label12 = new Infragistics.Win.Misc.UltraLabel();
            this.cboVendedor = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.txtGlosa = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.cboEstados = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.label1 = new Infragistics.Win.Misc.UltraLabel();
            this.cboVendedorRef = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.label5 = new Infragistics.Win.Misc.UltraLabel();
            this.chkPrecInIGV = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkAfectoIGV = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.label19 = new Infragistics.Win.Misc.UltraLabel();
            this.txtDescuento = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label20 = new Infragistics.Win.Misc.UltraLabel();
            this.uvPedido = new Infragistics.Win.Misc.UltraValidator(this.components);
            this.cboDocumento = new Infragistics.Win.UltraWinGrid.UltraCombo();
            this.cboVerificacion = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.label22 = new Infragistics.Win.Misc.UltraLabel();
            this.groupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.btnBuscarDirecciones = new Infragistics.Win.Misc.UltraButton();
            this.txtRucCliente = new SAMBHS.Common.Resource.RucEditor();
            this.txtDireccion = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label23 = new Infragistics.Win.Misc.UltraLabel();
            this.label7 = new Infragistics.Win.Misc.UltraLabel();
            this.btnBuscarCliente = new Infragistics.Win.Misc.UltraButton();
            this.txtRazonCliente = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtCodigoCliente = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label9 = new Infragistics.Win.Misc.UltraLabel();
            this.label11 = new Infragistics.Win.Misc.UltraLabel();
            this.txtTipoCambio = new Infragistics.Win.UltraWinMaskedEdit.UltraMaskedEdit();
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this.frmPedido_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this.lblAccion = new Infragistics.Win.Misc.UltraLabel();
            this.cboTipoOperacion = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.cboAgenciaTransporte = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.label40 = new Infragistics.Win.Misc.UltraLabel();
            this.lblAgenciaTransporte = new Infragistics.Win.Misc.UltraLabel();
            this.dtpFechaDespacho = new System.Windows.Forms.DateTimePicker();
            this.ultraLabel8 = new Infragistics.Win.Misc.UltraLabel();
            this.panel3 = new Infragistics.Win.Misc.UltraPanel();
            this.btnSalir = new Infragistics.Win.Misc.UltraButton();
            this.btnGuardar = new Infragistics.Win.Misc.UltraButton();
            this.BtnImprimir = new Infragistics.Win.Misc.UltraButton();
            this.btnGenerar = new Infragistics.Win.Misc.UltraButton();
            this.txtPrecioVenta = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtValorVenta = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label28 = new Infragistics.Win.Misc.UltraLabel();
            this.txtValor = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label29 = new Infragistics.Win.Misc.UltraLabel();
            this.txtDscto = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label13 = new Infragistics.Win.Misc.UltraLabel();
            this.label14 = new Infragistics.Win.Misc.UltraLabel();
            this.txtIgv = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label17 = new Infragistics.Win.Misc.UltraLabel();
            this.panel2 = new Infragistics.Win.Misc.UltraPanel();
            this.lblNotificacion = new Infragistics.Win.Misc.UltraLabel();
            this._frmPedido_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmPedido_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmPedido_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmPedido_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this.ultraPopupControlContainer1 = new Infragistics.Win.Misc.UltraPopupControlContainer(this.components);
            this.ValidarAgregarDetalle = new Infragistics.Win.Misc.UltraValidator(this.components);
            this.ValidarVendedor = new Infragistics.Win.Misc.UltraValidator(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ucTipoOperacion)).BeginInit();
            this.panel1.ClientArea.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboIGV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboEstablecimiento)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCorrelativo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPeriodo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox2)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.panelListaPrecios.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCantidadTotal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSerieDoc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNumeroDoc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDiasVigencia)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCondicionPago)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboMoneda)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboVendedor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGlosa)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboEstados)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboVendedorRef)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrecInIGV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAfectoIGV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescuento)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uvPedido)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDocumento)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboVerificacion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRucCliente)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDireccion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRazonCliente)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCodigoCliente)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            this.frmPedido_Fill_Panel.ClientArea.SuspendLayout();
            this.frmPedido_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboTipoOperacion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAgenciaTransporte)).BeginInit();
            this.panel3.ClientArea.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtPrecioVenta)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtValorVenta)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtValor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDscto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIgv)).BeginInit();
            this.panel2.ClientArea.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ValidarAgregarDetalle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ValidarVendedor)).BeginInit();
            this.SuspendLayout();
            // 
            // ucTipoOperacion
            // 
            this.ucTipoOperacion.DropDownListWidth = 270;
            this.ucTipoOperacion.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.ucTipoOperacion.Location = new System.Drawing.Point(9, 270);
            this.ucTipoOperacion.Name = "ucTipoOperacion";
            this.ucTipoOperacion.Size = new System.Drawing.Size(156, 21);
            this.ucTipoOperacion.TabIndex = 188;
            this.ucTipoOperacion.Visible = false;
            // 
            // panel1
            // 
            this.panel1.BackColorInternal = System.Drawing.Color.SlateGray;
            // 
            // panel1.ClientArea
            // 
            this.panel1.ClientArea.Controls.Add(this.ultraLabel1);
            this.panel1.ClientArea.Controls.Add(this.cboIGV);
            this.panel1.ClientArea.Controls.Add(this.label21);
            this.panel1.ClientArea.Controls.Add(this.btnNuevoMovimiento);
            this.panel1.ClientArea.Controls.Add(this.cboEstablecimiento);
            this.panel1.ClientArea.Controls.Add(this.label44);
            this.panel1.ClientArea.Controls.Add(this.btnSiguiente);
            this.panel1.ClientArea.Controls.Add(this.btnAnterior);
            this.panel1.ClientArea.Controls.Add(this.txtCorrelativo);
            this.panel1.ClientArea.Controls.Add(this.label16);
            this.panel1.ClientArea.Controls.Add(this.txtMes);
            this.panel1.ClientArea.Controls.Add(this.label15);
            this.panel1.ClientArea.Controls.Add(this.txtPeriodo);
            this.panel1.ClientArea.Controls.Add(this.label4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(989, 41);
            this.panel1.TabIndex = 86;
            this.panel1.UseAppStyling = false;
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.ForeColor = System.Drawing.Color.Black;
            this.ultraLabel1.Location = new System.Drawing.Point(822, 15);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(27, 14);
            this.ultraLabel1.TabIndex = 29;
            this.ultraLabel1.Text = "IGV:";
            // 
            // cboIGV
            // 
            this.cboIGV.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboIGV.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboIGV.Location = new System.Drawing.Point(856, 11);
            this.cboIGV.Name = "cboIGV";
            this.cboIGV.Size = new System.Drawing.Size(121, 21);
            this.cboIGV.TabIndex = 0;
            this.uvPedido.GetValidationSettings(this.cboIGV).Condition = new Infragistics.Win.OperatorCondition(Infragistics.Win.ConditionOperator.NotEquals, "--Seleccionar--", true, typeof(string));
            this.uvPedido.GetValidationSettings(this.cboIGV).Enabled = false;
            this.cboIGV.ValueChanged += new System.EventHandler(this.cboIGV_ValueChanged);
            // 
            // label21
            // 
            this.label21.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label21.AutoSize = true;
            this.label21.ForeColor = System.Drawing.Color.Black;
            this.label21.Location = new System.Drawing.Point(1680, 15);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(33, 14);
            this.label21.TabIndex = 28;
            this.label21.Text = "I.G.V.";
            // 
            // btnNuevoMovimiento
            // 
            appearance1.Image = global::SAMBHS.Windows.WinClient.UI.Resource.add;
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Center;
            this.btnNuevoMovimiento.Appearance = appearance1;
            this.btnNuevoMovimiento.Location = new System.Drawing.Point(618, 9);
            this.btnNuevoMovimiento.Name = "btnNuevoMovimiento";
            this.btnNuevoMovimiento.Size = new System.Drawing.Size(26, 25);
            this.btnNuevoMovimiento.TabIndex = 26;
            this.btnNuevoMovimiento.Visible = false;
            this.btnNuevoMovimiento.Click += new System.EventHandler(this.btnNuevoMovimiento_Click);
            // 
            // cboEstablecimiento
            // 
            this.cboEstablecimiento.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboEstablecimiento.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboEstablecimiento.Location = new System.Drawing.Point(775, 8);
            this.cboEstablecimiento.Name = "cboEstablecimiento";
            this.cboEstablecimiento.Size = new System.Drawing.Size(41, 21);
            this.cboEstablecimiento.TabIndex = 18;
            this.cboEstablecimiento.Visible = false;
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.ForeColor = System.Drawing.Color.Black;
            this.label44.Location = new System.Drawing.Point(684, 12);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(87, 14);
            this.label44.TabIndex = 206;
            this.label44.Text = "Establecimiento:";
            this.label44.Visible = false;
            // 
            // btnSiguiente
            // 
            this.btnSiguiente.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance2.Image = global::SAMBHS.Windows.WinClient.UI.Resource.resultset_next;
            appearance2.ImageHAlign = Infragistics.Win.HAlign.Center;
            this.btnSiguiente.Appearance = appearance2;
            this.btnSiguiente.Location = new System.Drawing.Point(1578, 2);
            this.btnSiguiente.Name = "btnSiguiente";
            this.btnSiguiente.Size = new System.Drawing.Size(26, 25);
            this.btnSiguiente.TabIndex = 21;
            this.btnSiguiente.Visible = false;
            this.btnSiguiente.Click += new System.EventHandler(this.btnSiguiente_Click);
            // 
            // btnAnterior
            // 
            this.btnAnterior.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance3.Image = global::SAMBHS.Windows.WinClient.UI.Resource.resultset_previous;
            appearance3.ImageHAlign = Infragistics.Win.HAlign.Center;
            this.btnAnterior.Appearance = appearance3;
            this.btnAnterior.Location = new System.Drawing.Point(1550, 2);
            this.btnAnterior.Name = "btnAnterior";
            this.btnAnterior.Size = new System.Drawing.Size(26, 25);
            this.btnAnterior.TabIndex = 20;
            this.btnAnterior.Visible = false;
            this.btnAnterior.Click += new System.EventHandler(this.btnAnterior_Click);
            // 
            // txtCorrelativo
            // 
            this.txtCorrelativo.Anchor = System.Windows.Forms.AnchorStyles.Top;
            appearance4.TextHAlignAsString = "Center";
            this.txtCorrelativo.Appearance = appearance4;
            this.txtCorrelativo.Location = new System.Drawing.Point(499, 11);
            this.txtCorrelativo.Margin = new System.Windows.Forms.Padding(2);
            this.txtCorrelativo.MaxLength = 8;
            this.txtCorrelativo.Name = "txtCorrelativo";
            this.txtCorrelativo.Size = new System.Drawing.Size(91, 21);
            this.txtCorrelativo.TabIndex = 0;
            // 
            // label16
            // 
            this.label16.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label16.AutoSize = true;
            this.label16.ForeColor = System.Drawing.Color.Black;
            this.label16.Location = new System.Drawing.Point(484, 15);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(8, 14);
            this.label16.TabIndex = 24;
            this.label16.Text = "-";
            // 
            // txtMes
            // 
            this.txtMes.Anchor = System.Windows.Forms.AnchorStyles.Top;
            appearance5.TextHAlignAsString = "Center";
            this.txtMes.Appearance = appearance5;
            this.txtMes.Location = new System.Drawing.Point(447, 11);
            this.txtMes.Margin = new System.Windows.Forms.Padding(2);
            this.txtMes.MaxLength = 2;
            this.txtMes.Name = "txtMes";
            this.txtMes.Size = new System.Drawing.Size(34, 21);
            this.txtMes.TabIndex = 18;
            // 
            // label15
            // 
            this.label15.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label15.AutoSize = true;
            this.label15.ForeColor = System.Drawing.Color.Black;
            this.label15.Location = new System.Drawing.Point(366, 15);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(74, 14);
            this.label15.TabIndex = 23;
            this.label15.Text = "Nro: Registro:";
            // 
            // txtPeriodo
            // 
            appearance6.TextHAlignAsString = "Center";
            this.txtPeriodo.Appearance = appearance6;
            this.txtPeriodo.Enabled = false;
            this.txtPeriodo.Location = new System.Drawing.Point(88, 11);
            this.txtPeriodo.Margin = new System.Windows.Forms.Padding(2);
            this.txtPeriodo.Name = "txtPeriodo";
            this.txtPeriodo.ReadOnly = true;
            this.txtPeriodo.Size = new System.Drawing.Size(85, 21);
            this.txtPeriodo.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(37, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 14);
            this.label4.TabIndex = 22;
            this.label4.Text = "Periodo:";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.btnLiberarSeparacion);
            this.groupBox2.Controls.Add(this.ucTipoOperacion);
            this.groupBox2.Controls.Add(this.panelListaPrecios);
            this.groupBox2.Controls.Add(this.grdData);
            this.groupBox2.Controls.Add(this.btnRedondear);
            this.groupBox2.Controls.Add(this.btnEliminar);
            this.groupBox2.Controls.Add(this.btnAgregar);
            this.groupBox2.Controls.Add(this.txtCantidadTotal);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Location = new System.Drawing.Point(8, 242);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(975, 256);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.Text = "Detalle de Pedido/Cotización";
            // 
            // btnLiberarSeparacion
            // 
            this.btnLiberarSeparacion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance7.Image = global::SAMBHS.Windows.WinClient.UI.Resource.arrow_undo;
            appearance7.ImageHAlign = Infragistics.Win.HAlign.Right;
            appearance7.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.btnLiberarSeparacion.Appearance = appearance7;
            this.btnLiberarSeparacion.Enabled = false;
            this.btnLiberarSeparacion.Location = new System.Drawing.Point(9, 224);
            this.btnLiberarSeparacion.Margin = new System.Windows.Forms.Padding(2);
            this.btnLiberarSeparacion.Name = "btnLiberarSeparacion";
            this.btnLiberarSeparacion.Size = new System.Drawing.Size(129, 24);
            this.btnLiberarSeparacion.TabIndex = 189;
            this.btnLiberarSeparacion.Text = "&Liberar Separación";
            this.btnLiberarSeparacion.Click += new System.EventHandler(this.btnLiberarSeparacion_Click);
            // 
            // panelListaPrecios
            // 
            this.panelListaPrecios.Location = new System.Drawing.Point(780, 18);
            this.panelListaPrecios.Name = "panelListaPrecios";
            this.panelListaPrecios.Size = new System.Drawing.Size(245, 200);
            this.panelListaPrecios.TabIndex = 122;
            this.panelListaPrecios.Visible = false;
            // 
            // grdData
            // 
            this.grdData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdData.CausesValidation = false;
            appearance8.BackColor = System.Drawing.Color.White;
            this.grdData.DisplayLayout.Appearance = appearance8;
            ultraGridColumn6.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn6.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.CellSelect;
            ultraGridColumn6.Header.Caption = "Nro. Serie";
            ultraGridColumn6.Header.VisiblePosition = 24;
            ultraGridColumn7.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn7.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.CellSelect;
            ultraGridColumn7.Header.Caption = "Nro. Lote";
            ultraGridColumn7.Header.VisiblePosition = 22;
            ultraGridColumn8.Header.VisiblePosition = 2;
            ultraGridColumn8.Hidden = true;
            ultraGridColumn9.Header.VisiblePosition = 4;
            ultraGridColumn9.Hidden = true;
            ultraGridColumn10.Header.Caption = "Fec. Vencimiento";
            ultraGridColumn10.Header.VisiblePosition = 23;
            ultraGridColumn1.Header.VisiblePosition = 5;
            ultraGridColumn1.Hidden = true;
            ultraGridColumn12.Header.VisiblePosition = 9;
            ultraGridColumn12.Hidden = true;
            ultraGridColumn13.Header.VisiblePosition = 10;
            ultraGridColumn13.Hidden = true;
            ultraGridColumn14.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.Edit;
            ultraGridColumn14.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            ultraGridColumn14.Header.Caption = "Descripción Producto";
            ultraGridColumn14.Header.VisiblePosition = 6;
            ultraGridColumn14.Width = 403;
            ultraGridColumn15.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn15.Header.VisiblePosition = 7;
            ultraGridColumn15.MaskInput = "{double:9.3}";
            ultraGridColumn15.Width = 59;
            ultraGridColumn16.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn16.Header.Caption = "UM. Empaque";
            ultraGridColumn16.Header.VisiblePosition = 8;
            ultraGridColumn16.TabStop = false;
            ultraGridColumn16.Width = 71;
            ultraGridColumn17.Header.Caption = "Almacén";
            ultraGridColumn17.Header.VisiblePosition = 1;
            ultraGridColumn17.Width = 56;
            appearance9.TextHAlignAsString = "Right";
            ultraGridColumn18.CellAppearance = appearance9;
            ultraGridColumn18.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            ultraGridColumn18.Header.Caption = "Cantidad";
            ultraGridColumn18.Header.VisiblePosition = 11;
            ultraGridColumn18.Width = 56;
            ultraGridColumn19.Header.Caption = "U.M.";
            ultraGridColumn19.Header.VisiblePosition = 12;
            ultraGridColumn19.TabStop = false;
            ultraGridColumn19.Width = 70;
            appearance10.TextHAlignAsString = "Right";
            ultraGridColumn20.CellAppearance = appearance10;
            appearance11.Image = ((object)(resources.GetObject("appearance11.Image")));
            ultraGridColumn20.CellButtonAppearance = appearance11;
            ultraGridColumn20.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.Edit;
            ultraGridColumn20.Header.Caption = "Precio Unitario";
            ultraGridColumn20.Header.VisiblePosition = 13;
            ultraGridColumn20.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.EditButton;
            ultraGridColumn20.Width = 107;
            appearance12.TextHAlignAsString = "Right";
            ultraGridColumn21.CellAppearance = appearance12;
            ultraGridColumn21.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.Edit;
            ultraGridColumn21.Header.Caption = "Nro. Unidades";
            ultraGridColumn21.Header.VisiblePosition = 21;
            ultraGridColumn21.MaskInput = "{LOC}nn,nnn,nnn";
            ultraGridColumn22.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.Edit;
            ultraGridColumn22.Header.Caption = "Observacion";
            ultraGridColumn22.Header.VisiblePosition = 25;
            ultraGridColumn22.MaxLength = 200;
            ultraGridColumn23.Header.VisiblePosition = 26;
            ultraGridColumn23.Hidden = true;
            ultraGridColumn24.Header.VisiblePosition = 27;
            ultraGridColumn24.Hidden = true;
            appearance13.TextHAlignAsString = "Right";
            ultraGridColumn25.CellAppearance = appearance13;
            ultraGridColumn25.Header.VisiblePosition = 28;
            ultraGridColumn25.Hidden = true;
            appearance14.TextHAlignAsString = "Right";
            ultraGridColumn26.CellAppearance = appearance14;
            ultraGridColumn26.Header.VisiblePosition = 29;
            ultraGridColumn26.Hidden = true;
            ultraGridColumn27.Header.VisiblePosition = 30;
            ultraGridColumn27.Hidden = true;
            ultraGridColumn28.Header.VisiblePosition = 31;
            ultraGridColumn28.Hidden = true;
            appearance15.Image = global::SAMBHS.Windows.WinClient.UI.Resource.system_search;
            appearance15.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance15.ImageVAlign = Infragistics.Win.VAlign.Middle;
            ultraGridColumn29.CellButtonAppearance = appearance15;
            ultraGridColumn29.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            ultraGridColumn29.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            ultraGridColumn29.Header.Caption = "Código";
            ultraGridColumn29.Header.VisiblePosition = 3;
            ultraGridColumn29.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.EditButton;
            ultraGridColumn29.Width = 88;
            appearance16.TextHAlignAsString = "Right";
            ultraGridColumn30.CellAppearance = appearance16;
            ultraGridColumn30.Header.VisiblePosition = 32;
            ultraGridColumn30.Hidden = true;
            ultraGridColumn31.Header.Caption = "Item";
            ultraGridColumn31.Header.VisiblePosition = 0;
            ultraGridColumn31.Width = 37;
            ultraGridColumn32.Header.VisiblePosition = 33;
            ultraGridColumn32.Hidden = true;
            ultraGridColumn33.Header.VisiblePosition = 34;
            ultraGridColumn33.Hidden = true;
            ultraGridColumn70.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            appearance17.TextHAlignAsString = "Right";
            ultraGridColumn70.CellAppearance = appearance17;
            ultraGridColumn70.Format = "0.00";
            ultraGridColumn70.Header.Caption = "Valor";
            ultraGridColumn70.Header.VisiblePosition = 14;
            ultraGridColumn70.MaskInput = "{double:9.2}";
            ultraGridColumn70.Width = 51;
            ultraGridColumn71.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            appearance18.TextHAlignAsString = "Right";
            ultraGridColumn71.CellAppearance = appearance18;
            ultraGridColumn71.Format = "";
            ultraGridColumn71.Header.Caption = "Descuento";
            ultraGridColumn71.Header.VisiblePosition = 16;
            ultraGridColumn71.MaskInput = "{double:9.2}";
            ultraGridColumn71.Width = 85;
            ultraGridColumn72.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            appearance19.TextHAlignAsString = "Right";
            ultraGridColumn72.CellAppearance = appearance19;
            ultraGridColumn72.Format = "0.00";
            ultraGridColumn72.Header.Caption = "Valor Venta";
            ultraGridColumn72.Header.VisiblePosition = 17;
            ultraGridColumn72.MaskInput = "{double:9.2}";
            ultraGridColumn72.Width = 55;
            ultraGridColumn73.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            appearance20.TextHAlignAsString = "Right";
            ultraGridColumn73.CellAppearance = appearance20;
            ultraGridColumn73.Format = "0.00";
            ultraGridColumn73.Header.Caption = "Igv";
            ultraGridColumn73.Header.VisiblePosition = 18;
            ultraGridColumn73.MaskInput = "{double:9.2}";
            ultraGridColumn73.Width = 46;
            ultraGridColumn74.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            appearance21.TextHAlignAsString = "Right";
            ultraGridColumn74.CellAppearance = appearance21;
            ultraGridColumn74.Format = "0.00";
            ultraGridColumn74.Header.Caption = "Precio Venta";
            ultraGridColumn74.Header.VisiblePosition = 19;
            ultraGridColumn74.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals;
            ultraGridColumn74.MaskInput = "{double:9.2}";
            ultraGridColumn74.Width = 77;
            ultraGridColumn75.Header.VisiblePosition = 35;
            ultraGridColumn75.Hidden = true;
            ultraGridColumn76.Header.VisiblePosition = 36;
            ultraGridColumn76.Hidden = true;
            ultraGridColumn77.Header.VisiblePosition = 37;
            ultraGridColumn77.Hidden = true;
            ultraGridColumn78.Header.VisiblePosition = 38;
            ultraGridColumn78.Hidden = true;
            appearance22.TextHAlignAsString = "Right";
            ultraGridColumn79.CellAppearance = appearance22;
            ultraGridColumn79.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            ultraGridColumn79.Header.Caption = "Descuentos(%)";
            ultraGridColumn79.Header.VisiblePosition = 15;
            ultraGridColumn79.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            ultraGridColumn80.EditorComponent = this.ucTipoOperacion;
            ultraGridColumn80.Header.Caption = "Tipo Operacion";
            ultraGridColumn80.Header.VisiblePosition = 20;
            ultraGridColumn80.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn8,
            ultraGridColumn9,
            ultraGridColumn10,
            ultraGridColumn1,
            ultraGridColumn12,
            ultraGridColumn13,
            ultraGridColumn14,
            ultraGridColumn15,
            ultraGridColumn16,
            ultraGridColumn17,
            ultraGridColumn18,
            ultraGridColumn19,
            ultraGridColumn20,
            ultraGridColumn21,
            ultraGridColumn22,
            ultraGridColumn23,
            ultraGridColumn24,
            ultraGridColumn25,
            ultraGridColumn26,
            ultraGridColumn27,
            ultraGridColumn28,
            ultraGridColumn29,
            ultraGridColumn30,
            ultraGridColumn31,
            ultraGridColumn32,
            ultraGridColumn33,
            ultraGridColumn70,
            ultraGridColumn71,
            ultraGridColumn72,
            ultraGridColumn73,
            ultraGridColumn74,
            ultraGridColumn75,
            ultraGridColumn76,
            ultraGridColumn77,
            ultraGridColumn78,
            ultraGridColumn79,
            ultraGridColumn80});
            this.grdData.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdData.DisplayLayout.MaxColScrollRegions = 1;
            this.grdData.DisplayLayout.MaxRowScrollRegions = 1;
            this.grdData.DisplayLayout.NewColumnLoadStyle = Infragistics.Win.UltraWinGrid.NewColumnLoadStyle.Hide;
            this.grdData.DisplayLayout.Override.ActiveAppearancesEnabled = Infragistics.Win.DefaultableBoolean.False;
            appearance23.BackColor = System.Drawing.Color.AliceBlue;
            appearance23.ForeColor = System.Drawing.Color.Black;
            this.grdData.DisplayLayout.Override.ActiveRowAppearance = appearance23;
            this.grdData.DisplayLayout.Override.AllowColSwapping = Infragistics.Win.UltraWinGrid.AllowColSwapping.NotAllowed;
            this.grdData.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            this.grdData.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance24.BackColor = System.Drawing.Color.Transparent;
            this.grdData.DisplayLayout.Override.CardAreaAppearance = appearance24;
            appearance25.BorderColor = System.Drawing.SystemColors.ControlLight;
            this.grdData.DisplayLayout.Override.CellAppearance = appearance25;
            this.grdData.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.grdData.DisplayLayout.Override.CellPadding = 3;
            appearance26.TextHAlignAsString = "Left";
            this.grdData.DisplayLayout.Override.HeaderAppearance = appearance26;
            this.grdData.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.Select;
            appearance27.BorderColor = System.Drawing.Color.White;
            appearance27.TextVAlignAsString = "Middle";
            this.grdData.DisplayLayout.Override.RowAppearance = appearance27;
            this.grdData.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.grdData.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Extended;
            this.grdData.DisplayLayout.RowConnectorStyle = Infragistics.Win.UltraWinGrid.RowConnectorStyle.None;
            this.grdData.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdData.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdData.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.grdData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdData.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.grdData.Location = new System.Drawing.Point(9, 18);
            this.grdData.Margin = new System.Windows.Forms.Padding(2);
            this.grdData.Name = "grdData";
            this.grdData.Size = new System.Drawing.Size(959, 201);
            this.grdData.TabIndex = 121;
            this.grdData.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdData_InitializeLayout);
            this.grdData.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.grdData_InitializeRow);
            this.grdData.AfterEnterEditMode += new System.EventHandler(this.grdData_AfterEnterEditMode);
            this.grdData.AfterExitEditMode += new System.EventHandler(this.grdData_AfterExitEditMode);
            this.grdData.CellChange += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.grdData_CellChange);
            this.grdData.ClickCellButton += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.grdData_ClickCellButton);
            this.grdData.BeforeRowInsert += new Infragistics.Win.UltraWinGrid.BeforeRowInsertEventHandler(this.grdData_BeforeRowInsert);
            this.grdData.ClickCell += new Infragistics.Win.UltraWinGrid.ClickCellEventHandler(this.grdData_ClickCell);
            this.grdData.DoubleClickCell += new Infragistics.Win.UltraWinGrid.DoubleClickCellEventHandler(this.grdData_DoubleClickCell);
            this.grdData.KeyDown += new System.Windows.Forms.KeyEventHandler(this.grdData_KeyDown);
            this.grdData.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.grdData_KeyPress);
            this.grdData.KeyUp += new System.Windows.Forms.KeyEventHandler(this.grdData_KeyUp);
            this.grdData.MouseDown += new System.Windows.Forms.MouseEventHandler(this.grdData_MouseDown);
            // 
            // btnRedondear
            // 
            this.btnRedondear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance28.Image = global::SAMBHS.Windows.WinClient.UI.Resource.table_gear;
            appearance28.ImageHAlign = Infragistics.Win.HAlign.Right;
            appearance28.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.btnRedondear.Appearance = appearance28;
            this.btnRedondear.Enabled = false;
            this.btnRedondear.Location = new System.Drawing.Point(467, 225);
            this.btnRedondear.Margin = new System.Windows.Forms.Padding(2);
            this.btnRedondear.Name = "btnRedondear";
            this.btnRedondear.Size = new System.Drawing.Size(100, 24);
            this.btnRedondear.TabIndex = 120;
            this.btnRedondear.Text = "&Redondear";
            this.btnRedondear.Visible = false;
            this.btnRedondear.Click += new System.EventHandler(this.btnRedondear_Click);
            // 
            // btnEliminar
            // 
            this.btnEliminar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance29.Image = global::SAMBHS.Windows.WinClient.UI.Resource.table_row_delete;
            appearance29.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance29.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance29.TextHAlignAsString = "Right";
            appearance29.TextVAlignAsString = "Middle";
            this.btnEliminar.Appearance = appearance29;
            this.btnEliminar.Enabled = false;
            this.btnEliminar.Location = new System.Drawing.Point(737, 225);
            this.btnEliminar.Margin = new System.Windows.Forms.Padding(2);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(110, 24);
            this.btnEliminar.TabIndex = 1;
            this.btnEliminar.Text = "&Eliminar Detalle";
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // btnAgregar
            // 
            this.btnAgregar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance30.Image = global::SAMBHS.Windows.WinClient.UI.Resource.table_row_insert;
            appearance30.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance30.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance30.TextHAlignAsString = "Right";
            appearance30.TextVAlignAsString = "Middle";
            this.btnAgregar.Appearance = appearance30;
            this.btnAgregar.Location = new System.Drawing.Point(849, 225);
            this.btnAgregar.Margin = new System.Windows.Forms.Padding(2);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(116, 24);
            this.btnAgregar.TabIndex = 0;
            this.btnAgregar.Text = "&Agregar Detalle";
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
            // 
            // txtCantidadTotal
            // 
            this.txtCantidadTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance31.TextHAlignAsString = "Right";
            this.txtCantidadTotal.Appearance = appearance31;
            this.txtCantidadTotal.Location = new System.Drawing.Point(648, 227);
            this.txtCantidadTotal.Name = "txtCantidadTotal";
            this.txtCantidadTotal.ReadOnly = true;
            this.txtCantidadTotal.Size = new System.Drawing.Size(84, 21);
            this.txtCantidadTotal.TabIndex = 186;
            this.txtCantidadTotal.Text = "0.00";
            // 
            // label18
            // 
            this.label18.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(571, 229);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(63, 14);
            this.label18.TabIndex = 187;
            this.label18.Text = "Cant. Total:";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 14);
            this.label2.TabIndex = 89;
            this.label2.Text = "Documento:";
            // 
            // txtSerieDoc
            // 
            this.txtSerieDoc.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txtSerieDoc.Enabled = false;
            this.txtSerieDoc.Location = new System.Drawing.Point(150, 51);
            this.txtSerieDoc.Margin = new System.Windows.Forms.Padding(2);
            this.txtSerieDoc.MaxLength = 4;
            this.txtSerieDoc.Name = "txtSerieDoc";
            this.txtSerieDoc.Size = new System.Drawing.Size(34, 21);
            this.txtSerieDoc.TabIndex = 2;
            this.uvPedido.GetValidationSettings(this.txtSerieDoc).EmptyValueCriteria = Infragistics.Win.Misc.EmptyValueCriteria.NullOrEmptyString;
            this.uvPedido.GetValidationSettings(this.txtSerieDoc).Enabled = false;
            this.uvPedido.GetValidationSettings(this.txtSerieDoc).IsRequired = true;
            this.txtSerieDoc.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSerieDoc_KeyPress);
            this.txtSerieDoc.Validated += new System.EventHandler(this.txtSerieDoc_Validated);
            // 
            // txtNumeroDoc
            // 
            this.txtNumeroDoc.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txtNumeroDoc.Enabled = false;
            this.txtNumeroDoc.Location = new System.Drawing.Point(201, 51);
            this.txtNumeroDoc.Margin = new System.Windows.Forms.Padding(2);
            this.txtNumeroDoc.MaxLength = 8;
            this.txtNumeroDoc.Name = "txtNumeroDoc";
            this.txtNumeroDoc.Size = new System.Drawing.Size(88, 21);
            this.txtNumeroDoc.TabIndex = 3;
            this.uvPedido.GetValidationSettings(this.txtNumeroDoc).EmptyValueCriteria = Infragistics.Win.Misc.EmptyValueCriteria.NullOrEmptyString;
            this.uvPedido.GetValidationSettings(this.txtNumeroDoc).Enabled = false;
            this.uvPedido.GetValidationSettings(this.txtNumeroDoc).IsRequired = true;
            this.txtNumeroDoc.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNumeroDoc_KeyPress);
            this.txtNumeroDoc.Validated += new System.EventHandler(this.txtNumeroDoc_Validated);
            // 
            // dtpFechaEmision
            // 
            this.dtpFechaEmision.AllowDrop = true;
            this.dtpFechaEmision.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.dtpFechaEmision.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaEmision.Location = new System.Drawing.Point(340, 51);
            this.dtpFechaEmision.Name = "dtpFechaEmision";
            this.dtpFechaEmision.Size = new System.Drawing.Size(83, 20);
            this.dtpFechaEmision.TabIndex = 4;
            this.dtpFechaEmision.ValueChanged += new System.EventHandler(this.dtpFechaEmision_ValueChanged);
            // 
            // label25
            // 
            this.label25.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(296, 55);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(39, 14);
            this.label25.TabIndex = 129;
            this.label25.Text = "Fecha:";
            // 
            // label24
            // 
            this.label24.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(433, 55);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(72, 14);
            this.label24.TabIndex = 131;
            this.label24.Text = "Tipo Cambio:";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label3.Location = new System.Drawing.Point(566, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 34);
            this.label3.TabIndex = 132;
            this.label3.Text = "N° Días Vigencia:";
            // 
            // txtDiasVigencia
            // 
            this.txtDiasVigencia.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.txtDiasVigencia.Location = new System.Drawing.Point(624, 51);
            this.txtDiasVigencia.MaxLength = 6;
            this.txtDiasVigencia.Name = "txtDiasVigencia";
            this.txtDiasVigencia.Size = new System.Drawing.Size(37, 21);
            this.txtDiasVigencia.TabIndex = 6;
            this.txtDiasVigencia.TextChanged += new System.EventHandler(this.txtDiasVigencia_TextChanged);
            this.txtDiasVigencia.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDiasVigencia_KeyPress);
            // 
            // dtpFechaVencimiento
            // 
            this.dtpFechaVencimiento.AllowDrop = true;
            this.dtpFechaVencimiento.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.dtpFechaVencimiento.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaVencimiento.Location = new System.Drawing.Point(742, 51);
            this.dtpFechaVencimiento.Name = "dtpFechaVencimiento";
            this.dtpFechaVencimiento.Size = new System.Drawing.Size(81, 20);
            this.dtpFechaVencimiento.TabIndex = 7;
            // 
            // cboEstado
            // 
            this.cboEstado.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.cboEstado.AutoSize = true;
            this.cboEstado.Location = new System.Drawing.Point(670, 53);
            this.cboEstado.Name = "cboEstado";
            this.cboEstado.Size = new System.Drawing.Size(71, 14);
            this.cboEstado.TabIndex = 135;
            this.cboEstado.Text = "Fecha Venc.:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 159);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(103, 14);
            this.label6.TabIndex = 168;
            this.label6.Text = "Condición de Pago:";
            // 
            // cboCondicionPago
            // 
            this.cboCondicionPago.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboCondicionPago.Location = new System.Drawing.Point(123, 156);
            this.cboCondicionPago.Name = "cboCondicionPago";
            this.cboCondicionPago.Size = new System.Drawing.Size(144, 21);
            this.cboCondicionPago.TabIndex = 10;
            this.uvPedido.GetValidationSettings(this.cboCondicionPago).Condition = new Infragistics.Win.OperatorCondition(Infragistics.Win.ConditionOperator.NotEquals, "-1", true, typeof(string));
            this.uvPedido.GetValidationSettings(this.cboCondicionPago).Enabled = false;
            this.uvPedido.GetValidationSettings(this.cboCondicionPago).IsRequired = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(273, 159);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(37, 14);
            this.label8.TabIndex = 170;
            this.label8.Text = "Glosa:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(14, 188);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(48, 14);
            this.label10.TabIndex = 172;
            this.label10.Text = "Moneda:";
            // 
            // cboMoneda
            // 
            this.cboMoneda.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboMoneda.Location = new System.Drawing.Point(123, 185);
            this.cboMoneda.Name = "cboMoneda";
            this.cboMoneda.Size = new System.Drawing.Size(144, 21);
            this.cboMoneda.TabIndex = 12;
            this.uvPedido.GetValidationSettings(this.cboMoneda).Condition = new Infragistics.Win.OperatorCondition(Infragistics.Win.ConditionOperator.NotEquals, "-1", true, typeof(string));
            this.uvPedido.GetValidationSettings(this.cboMoneda).Enabled = false;
            this.uvPedido.GetValidationSettings(this.cboMoneda).IsRequired = true;
            this.ValidarAgregarDetalle.GetValidationSettings(this.cboMoneda).Condition = new Infragistics.Win.OperatorCondition(Infragistics.Win.ConditionOperator.NotEquals, "-1", true, typeof(string));
            this.ValidarAgregarDetalle.GetValidationSettings(this.cboMoneda).Enabled = false;
            this.ValidarAgregarDetalle.GetValidationSettings(this.cboMoneda).IsRequired = true;
            this.cboMoneda.ValueChanged += new System.EventHandler(this.cboMoneda_ValueChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(14, 217);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(56, 14);
            this.label12.TabIndex = 174;
            this.label12.Text = "Vendedor:";
            // 
            // cboVendedor
            // 
            this.cboVendedor.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboVendedor.Location = new System.Drawing.Point(123, 213);
            this.cboVendedor.Name = "cboVendedor";
            this.cboVendedor.Size = new System.Drawing.Size(276, 21);
            this.cboVendedor.TabIndex = 16;
            this.uvPedido.GetValidationSettings(this.cboVendedor).Enabled = false;
            this.uvPedido.GetValidationSettings(this.cboVendedor).IsRequired = true;
            this.ValidarVendedor.GetValidationSettings(this.cboVendedor).Condition = new Infragistics.Win.OperatorCondition(Infragistics.Win.ConditionOperator.NotEquals, "-1", true, typeof(string));
            this.ValidarVendedor.GetValidationSettings(this.cboVendedor).Enabled = false;
            this.ValidarVendedor.GetValidationSettings(this.cboVendedor).IsRequired = true;
            this.cboVendedor.ValueChanged += new System.EventHandler(this.cboVendedor_ValueChanged);
            // 
            // txtGlosa
            // 
            this.txtGlosa.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtGlosa.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtGlosa.Location = new System.Drawing.Point(342, 156);
            this.txtGlosa.MaxLength = 200;
            this.txtGlosa.Name = "txtGlosa";
            this.txtGlosa.Scrollbars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtGlosa.Size = new System.Drawing.Size(632, 21);
            this.txtGlosa.TabIndex = 11;
            // 
            // cboEstados
            // 
            this.cboEstados.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboEstados.Location = new System.Drawing.Point(475, 185);
            this.cboEstados.Name = "cboEstados";
            this.cboEstados.Size = new System.Drawing.Size(130, 21);
            this.cboEstados.TabIndex = 14;
            this.uvPedido.GetValidationSettings(this.cboEstados).Condition = new Infragistics.Win.OperatorCondition(Infragistics.Win.ConditionOperator.NotEquals, "--Seleccionar--", true, typeof(string));
            this.uvPedido.GetValidationSettings(this.cboEstados).Enabled = false;
            this.uvPedido.GetValidationSettings(this.cboEstados).IsRequired = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(428, 188);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 14);
            this.label1.TabIndex = 191;
            this.label1.Text = "Estado:";
            // 
            // cboVendedorRef
            // 
            this.cboVendedorRef.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboVendedorRef.Location = new System.Drawing.Point(649, 213);
            this.cboVendedorRef.Name = "cboVendedorRef";
            this.cboVendedorRef.Size = new System.Drawing.Size(97, 21);
            this.cboVendedorRef.TabIndex = 18;
            this.cboVendedorRef.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(611, 217);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 14);
            this.label5.TabIndex = 193;
            this.label5.Text = "Vendedor Ref.:";
            this.label5.Visible = false;
            // 
            // chkPrecInIGV
            // 
            this.chkPrecInIGV.AutoSize = true;
            this.chkPrecInIGV.Enabled = false;
            this.chkPrecInIGV.Location = new System.Drawing.Point(644, 187);
            this.chkPrecInIGV.Name = "chkPrecInIGV";
            this.chkPrecInIGV.Size = new System.Drawing.Size(115, 17);
            this.chkPrecInIGV.TabIndex = 16;
            this.chkPrecInIGV.Text = "Precio Incluye IGV";
            this.chkPrecInIGV.CheckedChanged += new System.EventHandler(this.chkPrecInIGV_CheckedChanged);
            // 
            // chkAfectoIGV
            // 
            this.chkAfectoIGV.AutoSize = true;
            this.chkAfectoIGV.Location = new System.Drawing.Point(630, 187);
            this.chkAfectoIGV.Name = "chkAfectoIGV";
            this.chkAfectoIGV.Size = new System.Drawing.Size(85, 17);
            this.chkAfectoIGV.TabIndex = 15;
            this.chkAfectoIGV.Text = "Afecto a IGV";
            this.chkAfectoIGV.Visible = false;
            this.chkAfectoIGV.CheckedChanged += new System.EventHandler(this.chkAfectoIGV_CheckedChanged);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(273, 188);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(64, 14);
            this.label19.TabIndex = 197;
            this.label19.Text = "Descuento :";
            // 
            // txtDescuento
            // 
            appearance32.TextHAlignAsString = "Right";
            this.txtDescuento.Appearance = appearance32;
            this.txtDescuento.Location = new System.Drawing.Point(342, 185);
            this.txtDescuento.Margin = new System.Windows.Forms.Padding(2);
            this.txtDescuento.MaxLength = 8;
            this.txtDescuento.Name = "txtDescuento";
            this.txtDescuento.Size = new System.Drawing.Size(56, 21);
            this.txtDescuento.TabIndex = 13;
            this.txtDescuento.Text = "0";
            this.txtDescuento.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDescuento_KeyPress);
            this.txtDescuento.Validating += new System.ComponentModel.CancelEventHandler(this.txtDescuento_Validating);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(399, 190);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(14, 14);
            this.label20.TabIndex = 14;
            this.label20.Text = "%";
            // 
            // cboDocumento
            // 
            this.cboDocumento.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.cboDocumento.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.None;
            appearance33.BackColor = System.Drawing.Color.White;
            this.cboDocumento.DisplayLayout.Appearance = appearance33;
            ultraGridColumn67.Format = "000";
            ultraGridColumn67.Header.Caption = "Cod.";
            ultraGridColumn67.Header.VisiblePosition = 0;
            ultraGridColumn67.RowLayoutColumnInfo.OriginX = 0;
            ultraGridColumn67.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn67.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(50, 0);
            ultraGridColumn67.RowLayoutColumnInfo.SpanX = 1;
            ultraGridColumn67.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn68.Header.Caption = "Nombre";
            ultraGridColumn68.Header.VisiblePosition = 1;
            ultraGridColumn68.RowLayoutColumnInfo.OriginX = 1;
            ultraGridColumn68.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn68.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(248, 0);
            ultraGridColumn68.RowLayoutColumnInfo.SpanX = 3;
            ultraGridColumn68.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn69.Header.Caption = "Siglas";
            ultraGridColumn69.Header.VisiblePosition = 2;
            ultraGridColumn69.RowLayoutColumnInfo.OriginX = 4;
            ultraGridColumn69.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn69.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(63, 0);
            ultraGridColumn69.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn69.RowLayoutColumnInfo.SpanY = 2;
            ultraGridBand2.Columns.AddRange(new object[] {
            ultraGridColumn67,
            ultraGridColumn68,
            ultraGridColumn69});
            ultraGridBand2.RowLayoutStyle = Infragistics.Win.UltraWinGrid.RowLayoutStyle.GroupLayout;
            this.cboDocumento.DisplayLayout.BandsSerializer.Add(ultraGridBand2);
            this.cboDocumento.DisplayLayout.NewColumnLoadStyle = Infragistics.Win.UltraWinGrid.NewColumnLoadStyle.Hide;
            this.cboDocumento.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.None;
            appearance34.BackColor = System.Drawing.Color.Transparent;
            this.cboDocumento.DisplayLayout.Override.CardAreaAppearance = appearance34;
            this.cboDocumento.DisplayLayout.Override.CellPadding = 3;
            appearance35.TextHAlignAsString = "Left";
            this.cboDocumento.DisplayLayout.Override.HeaderAppearance = appearance35;
            appearance36.BorderColor = System.Drawing.Color.LightGray;
            appearance36.TextVAlignAsString = "Middle";
            this.cboDocumento.DisplayLayout.Override.RowAppearance = appearance36;
            this.cboDocumento.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance37.BackColor = System.Drawing.Color.LightSteelBlue;
            appearance37.BorderColor = System.Drawing.Color.Black;
            appearance37.ForeColor = System.Drawing.Color.Black;
            this.cboDocumento.DisplayLayout.Override.SelectedRowAppearance = appearance37;
            this.cboDocumento.DisplayLayout.RowConnectorStyle = Infragistics.Win.UltraWinGrid.RowConnectorStyle.None;
            this.cboDocumento.Location = new System.Drawing.Point(76, 50);
            this.cboDocumento.Name = "cboDocumento";
            this.cboDocumento.Size = new System.Drawing.Size(70, 22);
            this.cboDocumento.TabIndex = 1;
            this.uvPedido.GetValidationSettings(this.cboDocumento).Condition = new Infragistics.Win.OperatorCondition(Infragistics.Win.ConditionOperator.NotEquals, "-1", true, typeof(string));
            this.uvPedido.GetValidationSettings(this.cboDocumento).Enabled = false;
            this.uvPedido.GetValidationSettings(this.cboDocumento).IsRequired = true;
            this.cboDocumento.AfterDropDown += new System.EventHandler(this.cboDocumento_AfterDropDown);
            this.cboDocumento.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboDocumento_KeyUp);
            this.cboDocumento.Leave += new System.EventHandler(this.cboDocumento_Leave);
            // 
            // cboVerificacion
            // 
            this.cboVerificacion.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboVerificacion.Location = new System.Drawing.Point(475, 213);
            this.cboVerificacion.Name = "cboVerificacion";
            this.cboVerificacion.Size = new System.Drawing.Size(130, 21);
            this.cboVerificacion.TabIndex = 17;
            this.uvPedido.GetValidationSettings(this.cboVerificacion).Enabled = false;
            // 
            // label22
            // 
            this.label22.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label22.AutoSize = true;
            this.label22.ForeColor = System.Drawing.Color.Black;
            this.label22.Location = new System.Drawing.Point(187, 55);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(8, 14);
            this.label22.TabIndex = 200;
            this.label22.Text = "-";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnBuscarDirecciones);
            this.groupBox1.Controls.Add(this.txtRucCliente);
            this.groupBox1.Controls.Add(this.txtDireccion);
            this.groupBox1.Controls.Add(this.label23);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.btnBuscarCliente);
            this.groupBox1.Controls.Add(this.txtRazonCliente);
            this.groupBox1.Controls.Add(this.txtCodigoCliente);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Location = new System.Drawing.Point(8, 78);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(975, 71);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.Text = "Datos Cliente";
            // 
            // btnBuscarDirecciones
            // 
            this.btnBuscarDirecciones.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance38.Image = global::SAMBHS.Windows.WinClient.UI.Resource.arrow_undo;
            appearance38.ImageHAlign = Infragistics.Win.HAlign.Center;
            this.btnBuscarDirecciones.Appearance = appearance38;
            this.btnBuscarDirecciones.Location = new System.Drawing.Point(942, 39);
            this.btnBuscarDirecciones.Name = "btnBuscarDirecciones";
            this.btnBuscarDirecciones.Size = new System.Drawing.Size(24, 26);
            this.btnBuscarDirecciones.TabIndex = 174;
            this.btnBuscarDirecciones.Click += new System.EventHandler(this.btnBuscarDirecciones_Click);
            // 
            // txtRucCliente
            // 
            appearance39.BackColor = System.Drawing.SystemColors.Window;
            appearance39.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.txtRucCliente.DisplayLayout.Appearance = appearance39;
            this.txtRucCliente.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.txtRucCliente.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance40.BackColor = System.Drawing.SystemColors.ActiveBorder;
            appearance40.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance40.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance40.BorderColor = System.Drawing.SystemColors.Window;
            this.txtRucCliente.DisplayLayout.GroupByBox.Appearance = appearance40;
            appearance41.ForeColor = System.Drawing.SystemColors.GrayText;
            this.txtRucCliente.DisplayLayout.GroupByBox.BandLabelAppearance = appearance41;
            this.txtRucCliente.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance42.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance42.BackColor2 = System.Drawing.SystemColors.Control;
            appearance42.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance42.ForeColor = System.Drawing.SystemColors.GrayText;
            this.txtRucCliente.DisplayLayout.GroupByBox.PromptAppearance = appearance42;
            this.txtRucCliente.DisplayLayout.MaxColScrollRegions = 1;
            this.txtRucCliente.DisplayLayout.MaxRowScrollRegions = 1;
            this.txtRucCliente.DisplayLayout.NewColumnLoadStyle = Infragistics.Win.UltraWinGrid.NewColumnLoadStyle.Hide;
            appearance43.BackColor = System.Drawing.SystemColors.Window;
            appearance43.ForeColor = System.Drawing.SystemColors.ControlText;
            this.txtRucCliente.DisplayLayout.Override.ActiveCellAppearance = appearance43;
            appearance44.BackColor = System.Drawing.SystemColors.Highlight;
            appearance44.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.txtRucCliente.DisplayLayout.Override.ActiveRowAppearance = appearance44;
            this.txtRucCliente.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Dotted;
            this.txtRucCliente.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Dotted;
            appearance45.BackColor = System.Drawing.SystemColors.Window;
            this.txtRucCliente.DisplayLayout.Override.CardAreaAppearance = appearance45;
            appearance46.BorderColor = System.Drawing.Color.Silver;
            appearance46.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.txtRucCliente.DisplayLayout.Override.CellAppearance = appearance46;
            this.txtRucCliente.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.txtRucCliente.DisplayLayout.Override.CellPadding = 0;
            appearance47.BackColor = System.Drawing.SystemColors.Control;
            appearance47.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance47.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance47.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance47.BorderColor = System.Drawing.SystemColors.Window;
            this.txtRucCliente.DisplayLayout.Override.GroupByRowAppearance = appearance47;
            appearance48.TextHAlignAsString = "Left";
            this.txtRucCliente.DisplayLayout.Override.HeaderAppearance = appearance48;
            this.txtRucCliente.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.txtRucCliente.DisplayLayout.Override.HeaderStyle = Infragistics.Win.HeaderStyle.WindowsXPCommand;
            appearance49.BackColor = System.Drawing.SystemColors.Window;
            appearance49.BorderColor = System.Drawing.Color.Silver;
            this.txtRucCliente.DisplayLayout.Override.RowAppearance = appearance49;
            this.txtRucCliente.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance50.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtRucCliente.DisplayLayout.Override.TemplateAddRowAppearance = appearance50;
            this.txtRucCliente.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.txtRucCliente.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.txtRucCliente.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.txtRucCliente.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.txtRucCliente.Location = new System.Drawing.Point(130, 16);
            this.txtRucCliente.Name = "txtRucCliente";
            this.txtRucCliente.Size = new System.Drawing.Size(159, 22);
            this.txtRucCliente.TabIndex = 173;
            this.txtRucCliente.TextChanged += new System.EventHandler(this.txtRucCliente_TextChanged);
            this.txtRucCliente.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtRucCliente_KeyDown);
            this.txtRucCliente.Validating += new System.ComponentModel.CancelEventHandler(this.txtRucCliente_Validating);
            // 
            // txtDireccion
            // 
            this.txtDireccion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDireccion.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtDireccion.Enabled = false;
            this.txtDireccion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDireccion.Location = new System.Drawing.Point(422, 42);
            this.txtDireccion.MaxLength = 200;
            this.txtDireccion.Name = "txtDireccion";
            this.txtDireccion.Size = new System.Drawing.Size(520, 21);
            this.txtDireccion.TabIndex = 172;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(340, 45);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(55, 14);
            this.label23.TabIndex = 171;
            this.label23.Text = "Dirección:";
            // 
            // label7
            // 
            appearance51.TextHAlignAsString = "Right";
            appearance51.TextVAlignAsString = "Top";
            this.label7.Appearance = appearance51;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(339, 20);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(74, 14);
            this.label7.TabIndex = 170;
            this.label7.Text = "Razón Social:";
            // 
            // btnBuscarCliente
            // 
            appearance52.Image = ((object)(resources.GetObject("appearance52.Image")));
            appearance52.ImageHAlign = Infragistics.Win.HAlign.Center;
            this.btnBuscarCliente.Appearance = appearance52;
            this.btnBuscarCliente.Location = new System.Drawing.Point(292, 14);
            this.btnBuscarCliente.Name = "btnBuscarCliente";
            this.btnBuscarCliente.Size = new System.Drawing.Size(26, 26);
            this.btnBuscarCliente.TabIndex = 1;
            this.btnBuscarCliente.Click += new System.EventHandler(this.btnBuscarCliente_Click);
            // 
            // txtRazonCliente
            // 
            this.txtRazonCliente.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRazonCliente.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtRazonCliente.Enabled = false;
            this.txtRazonCliente.Location = new System.Drawing.Point(422, 16);
            this.txtRazonCliente.Margin = new System.Windows.Forms.Padding(2);
            this.txtRazonCliente.Name = "txtRazonCliente";
            this.txtRazonCliente.Size = new System.Drawing.Size(543, 21);
            this.txtRazonCliente.TabIndex = 3;
            // 
            // txtCodigoCliente
            // 
            this.txtCodigoCliente.Enabled = false;
            this.txtCodigoCliente.Location = new System.Drawing.Point(130, 42);
            this.txtCodigoCliente.Margin = new System.Windows.Forms.Padding(2);
            this.txtCodigoCliente.MaxLength = 20;
            this.txtCodigoCliente.Name = "txtCodigoCliente";
            this.txtCodigoCliente.Size = new System.Drawing.Size(188, 21);
            this.txtCodigoCliente.TabIndex = 2;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(34, 23);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(32, 14);
            this.label9.TabIndex = 168;
            this.label9.Text = "RUC:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(34, 45);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(82, 14);
            this.label11.TabIndex = 167;
            this.label11.Text = "Código Cliente:";
            // 
            // txtTipoCambio
            // 
            this.txtTipoCambio.Anchor = System.Windows.Forms.AnchorStyles.Top;
            appearance53.TextHAlignAsString = "Right";
            this.txtTipoCambio.Appearance = appearance53;
            this.txtTipoCambio.EditAs = Infragistics.Win.UltraWinMaskedEdit.EditAsType.UseSpecifiedMask;
            this.txtTipoCambio.InputMask = "{double:1.4}";
            this.txtTipoCambio.Location = new System.Drawing.Point(506, 51);
            this.txtTipoCambio.Name = "txtTipoCambio";
            this.txtTipoCambio.NonAutoSizeHeight = 20;
            this.txtTipoCambio.Size = new System.Drawing.Size(52, 20);
            this.txtTipoCambio.TabIndex = 5;
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // frmPedido_Fill_Panel
            // 
            // 
            // frmPedido_Fill_Panel.ClientArea
            // 
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.cboVerificacion);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.lblAccion);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.cboTipoOperacion);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.cboAgenciaTransporte);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.label40);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.lblAgenciaTransporte);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.dtpFechaDespacho);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.ultraLabel8);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.panel3);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.panel2);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.txtTipoCambio);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.groupBox1);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.cboDocumento);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.label22);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.label20);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.txtDescuento);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.label19);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.chkPrecInIGV);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.chkAfectoIGV);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.cboVendedorRef);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.label5);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.cboEstados);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.label1);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.txtGlosa);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.cboVendedor);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.label12);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.cboMoneda);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.label10);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.label8);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.cboCondicionPago);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.label6);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.groupBox2);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.dtpFechaVencimiento);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.cboEstado);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.txtDiasVigencia);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.label3);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.label24);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.dtpFechaEmision);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.label25);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.txtNumeroDoc);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.txtSerieDoc);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.label2);
            this.frmPedido_Fill_Panel.ClientArea.Controls.Add(this.panel1);
            this.frmPedido_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.frmPedido_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmPedido_Fill_Panel.Location = new System.Drawing.Point(8, 31);
            this.frmPedido_Fill_Panel.Name = "frmPedido_Fill_Panel";
            this.frmPedido_Fill_Panel.Size = new System.Drawing.Size(989, 581);
            this.frmPedido_Fill_Panel.TabIndex = 0;
            // 
            // lblAccion
            // 
            this.lblAccion.AutoSize = true;
            this.lblAccion.Location = new System.Drawing.Point(427, 217);
            this.lblAccion.Name = "lblAccion";
            this.lblAccion.Size = new System.Drawing.Size(44, 14);
            this.lblAccion.TabIndex = 210;
            this.lblAccion.Text = "Acción :";
            // 
            // cboTipoOperacion
            // 
            this.cboTipoOperacion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboTipoOperacion.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboTipoOperacion.Location = new System.Drawing.Point(861, 185);
            this.cboTipoOperacion.Name = "cboTipoOperacion";
            this.cboTipoOperacion.Size = new System.Drawing.Size(113, 21);
            this.cboTipoOperacion.TabIndex = 15;
            this.cboTipoOperacion.ValueChanged += new System.EventHandler(this.cboTipoOperacion_ValueChanged);
            // 
            // cboAgenciaTransporte
            // 
            this.cboAgenciaTransporte.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboAgenciaTransporte.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.SuggestAppend;
            this.cboAgenciaTransporte.Location = new System.Drawing.Point(861, 213);
            this.cboAgenciaTransporte.Name = "cboAgenciaTransporte";
            this.cboAgenciaTransporte.Size = new System.Drawing.Size(113, 21);
            this.cboAgenciaTransporte.TabIndex = 19;
            this.cboAgenciaTransporte.ValueChanged += new System.EventHandler(this.cboAgenciaTransporte_ValueChanged);
            this.cboAgenciaTransporte.Leave += new System.EventHandler(this.cboAgenciaTransporte_Leave);
            this.cboAgenciaTransporte.Validated += new System.EventHandler(this.cboAgenciaTransporte_Validated);
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.ForeColor = System.Drawing.Color.Black;
            this.label40.Location = new System.Drawing.Point(775, 188);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(85, 14);
            this.label40.TabIndex = 189;
            this.label40.Text = "Tipo Operación:";
            // 
            // lblAgenciaTransporte
            // 
            this.lblAgenciaTransporte.AutoSize = true;
            this.lblAgenciaTransporte.Location = new System.Drawing.Point(751, 217);
            this.lblAgenciaTransporte.Name = "lblAgenciaTransporte";
            this.lblAgenciaTransporte.Size = new System.Drawing.Size(109, 14);
            this.lblAgenciaTransporte.TabIndex = 209;
            this.lblAgenciaTransporte.Text = "Agencia Transporte :";
            // 
            // dtpFechaDespacho
            // 
            this.dtpFechaDespacho.AllowDrop = true;
            this.dtpFechaDespacho.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.dtpFechaDespacho.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaDespacho.Location = new System.Drawing.Point(896, 52);
            this.dtpFechaDespacho.Name = "dtpFechaDespacho";
            this.dtpFechaDespacho.Size = new System.Drawing.Size(84, 20);
            this.dtpFechaDespacho.TabIndex = 8;
            // 
            // ultraLabel8
            // 
            this.ultraLabel8.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.ultraLabel8.Location = new System.Drawing.Point(831, 49);
            this.ultraLabel8.Name = "ultraLabel8";
            this.ultraLabel8.Size = new System.Drawing.Size(63, 31);
            this.ultraLabel8.TabIndex = 174;
            this.ultraLabel8.Text = "Fecha Despacho :";
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // panel3.ClientArea
            // 
            this.panel3.ClientArea.Controls.Add(this.btnSalir);
            this.panel3.ClientArea.Controls.Add(this.btnGuardar);
            this.panel3.ClientArea.Controls.Add(this.BtnImprimir);
            this.panel3.ClientArea.Controls.Add(this.btnGenerar);
            this.panel3.ClientArea.Controls.Add(this.txtPrecioVenta);
            this.panel3.ClientArea.Controls.Add(this.txtValorVenta);
            this.panel3.ClientArea.Controls.Add(this.label28);
            this.panel3.ClientArea.Controls.Add(this.txtValor);
            this.panel3.ClientArea.Controls.Add(this.label29);
            this.panel3.ClientArea.Controls.Add(this.txtDscto);
            this.panel3.ClientArea.Controls.Add(this.label13);
            this.panel3.ClientArea.Controls.Add(this.label14);
            this.panel3.ClientArea.Controls.Add(this.txtIgv);
            this.panel3.ClientArea.Controls.Add(this.label17);
            this.panel3.Location = new System.Drawing.Point(3, 504);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(982, 39);
            this.panel3.TabIndex = 208;
            // 
            // btnSalir
            // 
            this.btnSalir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            appearance54.Image = global::SAMBHS.Windows.WinClient.UI.Resource.door_out;
            appearance54.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance54.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance54.TextHAlignAsString = "Center";
            appearance54.TextVAlignAsString = "Middle";
            this.btnSalir.Appearance = appearance54;
            this.btnSalir.Location = new System.Drawing.Point(4, 4);
            this.btnSalir.Margin = new System.Windows.Forms.Padding(2);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(64, 33);
            this.btnSalir.TabIndex = 24;
            this.btnSalir.Text = "&Salir";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            appearance55.Image = global::SAMBHS.Windows.WinClient.UI.Resource.system_save;
            appearance55.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance55.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance55.TextHAlignAsString = "Center";
            appearance55.TextVAlignAsString = "Middle";
            this.btnGuardar.Appearance = appearance55;
            this.btnGuardar.Location = new System.Drawing.Point(244, 4);
            this.btnGuardar.Margin = new System.Windows.Forms.Padding(2);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(82, 33);
            this.btnGuardar.TabIndex = 21;
            this.btnGuardar.Text = "&Guardar";
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // BtnImprimir
            // 
            this.BtnImprimir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            appearance56.Image = global::SAMBHS.Windows.WinClient.UI.Resource.printer;
            appearance56.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance56.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance56.TextHAlignAsString = "Center";
            appearance56.TextVAlignAsString = "Middle";
            this.BtnImprimir.Appearance = appearance56;
            this.BtnImprimir.Location = new System.Drawing.Point(69, 4);
            this.BtnImprimir.Margin = new System.Windows.Forms.Padding(2);
            this.BtnImprimir.Name = "BtnImprimir";
            this.BtnImprimir.Size = new System.Drawing.Size(73, 33);
            this.BtnImprimir.TabIndex = 23;
            this.BtnImprimir.Text = "&Imprimir ";
            this.BtnImprimir.Click += new System.EventHandler(this.BtnImprimir_Click);
            // 
            // btnGenerar
            // 
            this.btnGenerar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            appearance57.Image = global::SAMBHS.Windows.WinClient.UI.Resource.email_transfer;
            appearance57.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance57.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance57.TextHAlignAsString = "Center";
            appearance57.TextVAlignAsString = "Middle";
            this.btnGenerar.Appearance = appearance57;
            this.btnGenerar.Enabled = false;
            this.btnGenerar.Location = new System.Drawing.Point(143, 4);
            this.btnGenerar.Margin = new System.Windows.Forms.Padding(2);
            this.btnGenerar.Name = "btnGenerar";
            this.btnGenerar.Size = new System.Drawing.Size(100, 33);
            this.btnGenerar.TabIndex = 22;
            this.btnGenerar.Text = "&Generar Comprobante";
            this.btnGenerar.Click += new System.EventHandler(this.btnGenerar_Click);
            // 
            // txtPrecioVenta
            // 
            this.txtPrecioVenta.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance58.TextHAlignAsString = "Right";
            this.txtPrecioVenta.Appearance = appearance58;
            this.txtPrecioVenta.Location = new System.Drawing.Point(890, 11);
            this.txtPrecioVenta.Name = "txtPrecioVenta";
            this.txtPrecioVenta.ReadOnly = true;
            this.txtPrecioVenta.Size = new System.Drawing.Size(84, 21);
            this.txtPrecioVenta.TabIndex = 188;
            this.txtPrecioVenta.Text = "0.00";
            // 
            // txtValorVenta
            // 
            this.txtValorVenta.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance59.TextHAlignAsString = "Right";
            this.txtValorVenta.Appearance = appearance59;
            this.txtValorVenta.Location = new System.Drawing.Point(632, 11);
            this.txtValorVenta.Name = "txtValorVenta";
            this.txtValorVenta.ReadOnly = true;
            this.txtValorVenta.Size = new System.Drawing.Size(84, 21);
            this.txtValorVenta.TabIndex = 183;
            this.txtValorVenta.Text = "0.00";
            // 
            // label28
            // 
            this.label28.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(433, 14);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(34, 14);
            this.label28.TabIndex = 179;
            this.label28.Text = "Valor:";
            // 
            // txtValor
            // 
            this.txtValor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance60.TextHAlignAsString = "Right";
            this.txtValor.Appearance = appearance60;
            this.txtValor.Location = new System.Drawing.Point(469, 11);
            this.txtValor.Name = "txtValor";
            this.txtValor.ReadOnly = true;
            this.txtValor.Size = new System.Drawing.Size(84, 21);
            this.txtValor.TabIndex = 177;
            this.txtValor.Text = "0.00";
            // 
            // label29
            // 
            this.label29.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(329, 14);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(42, 14);
            this.label29.TabIndex = 180;
            this.label29.Text = "Dscto. :";
            // 
            // txtDscto
            // 
            this.txtDscto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance61.TextHAlignAsString = "Right";
            this.txtDscto.Appearance = appearance61;
            this.txtDscto.Location = new System.Drawing.Point(378, 11);
            this.txtDscto.Name = "txtDscto";
            this.txtDscto.ReadOnly = true;
            this.txtDscto.Size = new System.Drawing.Size(52, 21);
            this.txtDscto.TabIndex = 178;
            this.txtDscto.Text = "0.00";
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(565, 14);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(66, 14);
            this.label13.TabIndex = 182;
            this.label13.Text = "Valor Venta:";
            // 
            // label14
            // 
            this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(716, 14);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(39, 14);
            this.label14.TabIndex = 185;
            this.label14.Text = "I.G.V. :";
            // 
            // txtIgv
            // 
            this.txtIgv.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance62.TextHAlignAsString = "Right";
            this.txtIgv.Appearance = appearance62;
            this.txtIgv.Location = new System.Drawing.Point(756, 11);
            this.txtIgv.Name = "txtIgv";
            this.txtIgv.ReadOnly = true;
            this.txtIgv.Size = new System.Drawing.Size(64, 21);
            this.txtIgv.TabIndex = 184;
            this.txtIgv.Text = "0.00";
            // 
            // label17
            // 
            this.label17.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(820, 14);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(72, 14);
            this.label17.TabIndex = 189;
            this.label17.Text = "Precio Venta:";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColorInternal = System.Drawing.Color.Gray;
            // 
            // panel2.ClientArea
            // 
            this.panel2.ClientArea.Controls.Add(this.lblNotificacion);
            this.panel2.ForeColor = System.Drawing.Color.White;
            this.panel2.Location = new System.Drawing.Point(3, 549);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1005, 26);
            this.panel2.TabIndex = 204;
            this.panel2.UseAppStyling = false;
            // 
            // lblNotificacion
            // 
            appearance63.ForeColor = System.Drawing.Color.White;
            this.lblNotificacion.Appearance = appearance63;
            this.lblNotificacion.AutoSize = true;
            this.lblNotificacion.BackColorInternal = System.Drawing.Color.Gray;
            this.lblNotificacion.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNotificacion.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblNotificacion.Location = new System.Drawing.Point(1, 3);
            this.lblNotificacion.Name = "lblNotificacion";
            this.lblNotificacion.Size = new System.Drawing.Size(417, 17);
            this.lblNotificacion.TabIndex = 0;
            this.lblNotificacion.Text = "El comprobante se generará con el correlativo:  0000-000000000";
            this.lblNotificacion.UseAppStyling = false;
            // 
            // _frmPedido_UltraFormManager_Dock_Area_Left
            // 
            this._frmPedido_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmPedido_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmPedido_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._frmPedido_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmPedido_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._frmPedido_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmPedido_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 31);
            this._frmPedido_UltraFormManager_Dock_Area_Left.Name = "_frmPedido_UltraFormManager_Dock_Area_Left";
            this._frmPedido_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 581);
            // 
            // _frmPedido_UltraFormManager_Dock_Area_Right
            // 
            this._frmPedido_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmPedido_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmPedido_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._frmPedido_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmPedido_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._frmPedido_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmPedido_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(997, 31);
            this._frmPedido_UltraFormManager_Dock_Area_Right.Name = "_frmPedido_UltraFormManager_Dock_Area_Right";
            this._frmPedido_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 581);
            // 
            // _frmPedido_UltraFormManager_Dock_Area_Top
            // 
            this._frmPedido_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmPedido_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmPedido_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._frmPedido_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmPedido_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._frmPedido_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmPedido_UltraFormManager_Dock_Area_Top.Name = "_frmPedido_UltraFormManager_Dock_Area_Top";
            this._frmPedido_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(1005, 31);
            // 
            // _frmPedido_UltraFormManager_Dock_Area_Bottom
            // 
            this._frmPedido_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmPedido_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmPedido_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._frmPedido_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmPedido_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._frmPedido_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmPedido_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 612);
            this._frmPedido_UltraFormManager_Dock_Area_Bottom.Name = "_frmPedido_UltraFormManager_Dock_Area_Bottom";
            this._frmPedido_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(1005, 8);
            // 
            // ultraPopupControlContainer1
            // 
            this.ultraPopupControlContainer1.PopupControl = this.panelListaPrecios;
            // 
            // frmPedido
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(1005, 620);
            this.Controls.Add(this.frmPedido_Fill_Panel);
            this.Controls.Add(this._frmPedido_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._frmPedido_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._frmPedido_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._frmPedido_UltraFormManager_Dock_Area_Bottom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPedido";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Pedido/Cotización";
            this.Load += new System.EventHandler(this.frmPedido_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ucTipoOperacion)).EndInit();
            this.panel1.ClientArea.ResumeLayout(false);
            this.panel1.ClientArea.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboIGV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboEstablecimiento)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCorrelativo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPeriodo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox2)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panelListaPrecios.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCantidadTotal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSerieDoc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNumeroDoc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDiasVigencia)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboCondicionPago)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboMoneda)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboVendedor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGlosa)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboEstados)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboVendedorRef)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPrecInIGV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAfectoIGV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescuento)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uvPedido)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDocumento)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboVerificacion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRucCliente)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDireccion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRazonCliente)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCodigoCliente)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            this.frmPedido_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmPedido_Fill_Panel.ClientArea.PerformLayout();
            this.frmPedido_Fill_Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboTipoOperacion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAgenciaTransporte)).EndInit();
            this.panel3.ClientArea.ResumeLayout(false);
            this.panel3.ClientArea.PerformLayout();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtPrecioVenta)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtValorVenta)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtValor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDscto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIgv)).EndInit();
            this.panel2.ClientArea.ResumeLayout(false);
            this.panel2.ClientArea.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ValidarAgregarDetalle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ValidarVendedor)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraPanel panel1;
        private Infragistics.Win.Misc.UltraButton btnNuevoMovimiento;
        private Infragistics.Win.Misc.UltraButton btnSiguiente;
        private Infragistics.Win.Misc.UltraButton btnAnterior;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtCorrelativo;
        private Infragistics.Win.Misc.UltraLabel label16;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtMes;
        private Infragistics.Win.Misc.UltraLabel label15;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtPeriodo;
        private Infragistics.Win.Misc.UltraLabel label4;
        private Infragistics.Win.Misc.UltraGroupBox groupBox2;
        private Infragistics.Win.Misc.UltraLabel label2;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtSerieDoc;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtNumeroDoc;
        private System.Windows.Forms.DateTimePicker dtpFechaEmision;
        private Infragistics.Win.Misc.UltraLabel label25;
        private Infragistics.Win.Misc.UltraLabel label24;
        private Infragistics.Win.Misc.UltraLabel label3;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtDiasVigencia;
        private System.Windows.Forms.DateTimePicker dtpFechaVencimiento;
        private Infragistics.Win.Misc.UltraLabel cboEstado;
        private Infragistics.Win.Misc.UltraLabel label6;
        private UltraComboEditor cboCondicionPago;
        private Infragistics.Win.Misc.UltraLabel label8;
        private Infragistics.Win.Misc.UltraLabel label10;
        private UltraComboEditor cboMoneda;
        private Infragistics.Win.Misc.UltraLabel label12;
        private Infragistics.Win.Misc.UltraButton btnEliminar;
        private Infragistics.Win.Misc.UltraButton btnAgregar;
        private UltraComboEditor cboVendedor;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtGlosa;
        private UltraComboEditor cboEstados;
        private Infragistics.Win.Misc.UltraLabel label1;
        private UltraComboEditor cboVendedorRef;
        private Infragistics.Win.Misc.UltraLabel label5;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkPrecInIGV;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkAfectoIGV;
        private Infragistics.Win.Misc.UltraLabel label19;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtDescuento;
        private Infragistics.Win.Misc.UltraLabel label20;
        private UltraComboEditor cboIGV;
        private Infragistics.Win.Misc.UltraLabel label21;
        private Infragistics.Win.Misc.UltraValidator uvPedido;
        private Infragistics.Win.Misc.UltraLabel label22;
        private Infragistics.Win.UltraWinGrid.UltraCombo cboDocumento;
        private Infragistics.Win.Misc.UltraButton btnRedondear;
        private Infragistics.Win.Misc.UltraGroupBox groupBox1;
        private Infragistics.Win.Misc.UltraLabel label7;
        private Infragistics.Win.Misc.UltraButton btnBuscarCliente;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtRazonCliente;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtCodigoCliente;
        private Infragistics.Win.Misc.UltraLabel label9;
        private Infragistics.Win.Misc.UltraLabel label11;
        private Infragistics.Win.Misc.UltraLabel label23;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtDireccion;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdData;
        private Infragistics.Win.UltraWinMaskedEdit.UltraMaskedEdit txtTipoCambio;
        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.Misc.UltraPanel frmPedido_Fill_Panel;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmPedido_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmPedido_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmPedido_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmPedido_UltraFormManager_Dock_Area_Bottom;
        private UltraComboEditor cboEstablecimiento;
        private Infragistics.Win.Misc.UltraLabel label44;
        private Infragistics.Win.Misc.UltraPanel panel3;
        private Infragistics.Win.Misc.UltraButton btnSalir;
        private Infragistics.Win.Misc.UltraButton btnGuardar;
        private Infragistics.Win.Misc.UltraButton BtnImprimir;
        private Infragistics.Win.Misc.UltraButton btnGenerar;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtPrecioVenta;
        private Infragistics.Win.Misc.UltraLabel label28;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtValor;
        private Infragistics.Win.Misc.UltraLabel label29;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtDscto;
        private Infragistics.Win.Misc.UltraLabel label13;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtValorVenta;
        private Infragistics.Win.Misc.UltraLabel label14;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtIgv;
        private Infragistics.Win.Misc.UltraLabel label18;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtCantidadTotal;
        private Infragistics.Win.Misc.UltraLabel label17;
        private Infragistics.Win.Misc.UltraPanel panel2;
        private Infragistics.Win.Misc.UltraLabel lblNotificacion;
        private Infragistics.Win.Misc.UltraPanel panelListaPrecios;
        private Infragistics.Win.Misc.UltraPopupControlContainer ultraPopupControlContainer1;
        private Infragistics.Win.Misc.UltraValidator ValidarAgregarDetalle;
        private Common.Resource.RucEditor txtRucCliente;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private System.Windows.Forms.DateTimePicker dtpFechaDespacho;
        private Infragistics.Win.Misc.UltraLabel ultraLabel8;
        private UltraComboEditor cboAgenciaTransporte;
        private Infragistics.Win.Misc.UltraLabel lblAgenciaTransporte;
        private UltraComboEditor cboTipoOperacion;
        private Infragistics.Win.Misc.UltraLabel label40;
        private UltraComboEditor ucTipoOperacion;
        private Infragistics.Win.Misc.UltraButton btnBuscarDirecciones;
        private Infragistics.Win.Misc.UltraValidator ValidarVendedor;
        private UltraComboEditor cboVerificacion;
        private Infragistics.Win.Misc.UltraLabel lblAccion;
        private Infragistics.Win.Misc.UltraButton btnLiberarSeparacion;
        //private Reportes.Ventas.Rpt_Pedido rpt_Pedido1;
    }
}