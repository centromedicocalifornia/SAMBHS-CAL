using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinEditors;
namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    partial class frmEstablecimiento
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
            Infragistics.Win.Misc.UltraGroupBox groupBox1;
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Misc.UltraGroupBox groupBox2;
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_Nombre");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_Direccion");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("i_IdCentroCosto");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_UsuarioCreacion");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("t_InsertaFecha");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_UsuarioModificacion");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("t_ActualizaFecha");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("i_IdEstablecimiento", 0);
            Infragistics.Win.UltraWinGrid.UltraGridGroup ultraGridGroup1 = new Infragistics.Win.UltraWinGrid.UltraGridGroup("NewGroup0", 24699327);
            Infragistics.Win.UltraWinGrid.UltraGridGroup ultraGridGroup2 = new Infragistics.Win.UltraWinGrid.UltraGridGroup("NewGroup1", 24699328);
            Infragistics.Win.UltraWinGrid.UltraGridGroup ultraGridGroup3 = new Infragistics.Win.UltraWinGrid.UltraGridGroup("NewGroup2", 24699329);
            Infragistics.Win.UltraWinGrid.SummarySettings summarySettings1 = new Infragistics.Win.UltraWinGrid.SummarySettings("", Infragistics.Win.UltraWinGrid.SummaryType.Count, null, "v_Nombre", 0, true, "Band 0", 0, Infragistics.Win.UltraWinGrid.SummaryPosition.Left, null, -1, false);
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmEstablecimiento));
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.Misc.UltraPanel ultraPanel1;
            Infragistics.Win.Misc.UltraFlowLayoutManager ultraFlowLayoutManager1;
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton("btnBuscar");
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Se especifica cuál será el centro de costo predeterminado para las ventas rápidas" +
        ".", Infragistics.Win.ToolTipImage.Info, "Información", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand2 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_AlmacenNombre");
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo2 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Con ésta serie se generará la Venta  para este Tipo Documento", Infragistics.Win.ToolTipImage.Default, null, Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance28 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance29 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance30 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance31 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance32 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance33 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand3 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("i_IdEstablecimientoDetalle");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_WarehouseName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("i_ImpresionVistaPrevia");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_NombreImpresora");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("i_DocumentoPredeterminado");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn15 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("i_NumeroItems");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn16 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_Documento", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn17 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_Serie", 1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn18 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("i_Correlativo", 2);
            Infragistics.Win.Appearance appearance34 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance35 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance36 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance37 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance38 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance39 = new Infragistics.Win.Appearance();
            Infragistics.Win.Misc.ValidationGroup validationGroup1 = new Infragistics.Win.Misc.ValidationGroup("Grupo");
            Infragistics.Win.Misc.ValidationGroup validationGroup2 = new Infragistics.Win.Misc.ValidationGroup("Detalle");
            Infragistics.Win.Misc.ValidationGroup validationGroup3 = new Infragistics.Win.Misc.ValidationGroup("Almacen");
            this.btnBuscar = new Infragistics.Win.Misc.UltraButton();
            this.txtEstablecimiento = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label1 = new Infragistics.Win.Misc.UltraLabel();
            this.btnAsignarAlmacen = new Infragistics.Win.Misc.UltraButton();
            this.btnEliminarGrupo = new Infragistics.Win.Misc.UltraButton();
            this.btnCrearDetalle = new Infragistics.Win.Misc.UltraButton();
            this.btnCrearGrupo = new Infragistics.Win.Misc.UltraButton();
            this.grdData = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.pnRight = new System.Windows.Forms.Panel();
            this.gbGrupo = new Infragistics.Win.Misc.UltraGroupBox();
            this.txtCentroCosto = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label4 = new Infragistics.Win.Misc.UltraLabel();
            this.btnGrabar = new Infragistics.Win.Misc.UltraButton();
            this.txtGrupoDireccion = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtGrupoNombre = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label7 = new Infragistics.Win.Misc.UltraLabel();
            this.label6 = new Infragistics.Win.Misc.UltraLabel();
            this.gbAlmacenes = new Infragistics.Win.Misc.UltraGroupBox();
            this.cboEstablecimientoAlmacen = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.btnEliminarAlmacen = new Infragistics.Win.Misc.UltraButton();
            this.btnEditarAlmacen = new Infragistics.Win.Misc.UltraButton();
            this.groupBox4 = new Infragistics.Win.Misc.UltraGroupBox();
            this.grdAlmacen = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.btnAgregarEstablecimientoAlmacen = new Infragistics.Win.Misc.UltraButton();
            this.label3 = new Infragistics.Win.Misc.UltraLabel();
            this.gbDetalle = new Infragistics.Win.Misc.UltraGroupBox();
            this.txtNombreImpresora = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.cboDetalleAlmacen = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.cboDetalleDocumento = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.txtMaxNumeroItems = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.uckDocumentoPredeterminado = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.uckImpresionPrevia = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.label2 = new Infragistics.Win.Misc.UltraLabel();
            this.txtDetalleCorrelativoDocIni = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtDetalleSerie = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.btnEliminar = new Infragistics.Win.Misc.UltraButton();
            this.btnEditar = new Infragistics.Win.Misc.UltraButton();
            this.btnAgregar = new Infragistics.Win.Misc.UltraButton();
            this.groupBox5 = new Infragistics.Win.Misc.UltraGroupBox();
            this.grdDetalle = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.label14 = new Infragistics.Win.Misc.UltraLabel();
            this.label11 = new Infragistics.Win.Misc.UltraLabel();
            this.label10 = new Infragistics.Win.Misc.UltraLabel();
            this.uvGrupo = new Infragistics.Win.Misc.UltraValidator(this.components);
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.ultraSplitter1 = new Infragistics.Win.Misc.UltraSplitter();
            groupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            groupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            ultraPanel1 = new Infragistics.Win.Misc.UltraPanel();
            ultraFlowLayoutManager1 = new Infragistics.Win.Misc.UltraFlowLayoutManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(groupBox1)).BeginInit();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtEstablecimiento)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(groupBox2)).BeginInit();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).BeginInit();
            ultraPanel1.ClientArea.SuspendLayout();
            ultraPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(ultraFlowLayoutManager1)).BeginInit();
            this.pnRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gbGrupo)).BeginInit();
            this.gbGrupo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCentroCosto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGrupoDireccion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGrupoNombre)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gbAlmacenes)).BeginInit();
            this.gbAlmacenes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboEstablecimientoAlmacen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox4)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdAlmacen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gbDetalle)).BeginInit();
            this.gbDetalle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtNombreImpresora)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDetalleAlmacen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDetalleDocumento)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxNumeroItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uckDocumentoPredeterminado)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uckImpresionPrevia)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDetalleCorrelativoDocIni)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDetalleSerie)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox5)).BeginInit();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdDetalle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uvGrupo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            groupBox1.Controls.Add(this.btnBuscar);
            groupBox1.Controls.Add(this.txtEstablecimiento);
            groupBox1.Controls.Add(this.label1);
            groupBox1.Location = new System.Drawing.Point(6, 6);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(468, 51);
            groupBox1.TabIndex = 0;
            groupBox1.Text = "Filtro de Búsqueda";
            // 
            // btnBuscar
            // 
            this.btnBuscar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.Image = global::SAMBHS.Windows.WinClient.UI.Resource.system_search;
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance1.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance1.TextHAlignAsString = "Center";
            appearance1.TextVAlignAsString = "Middle";
            this.btnBuscar.Appearance = appearance1;
            this.btnBuscar.BackColorInternal = System.Drawing.Color.Transparent;
            this.btnBuscar.Location = new System.Drawing.Point(385, 19);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(75, 23);
            this.btnBuscar.TabIndex = 1;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // txtEstablecimiento
            // 
            this.txtEstablecimiento.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEstablecimiento.Location = new System.Drawing.Point(108, 19);
            this.txtEstablecimiento.Name = "txtEstablecimiento";
            this.txtEstablecimiento.Size = new System.Drawing.Size(265, 21);
            this.txtEstablecimiento.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "Establecimiento";
            // 
            // groupBox2
            // 
            groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            groupBox2.Controls.Add(this.btnAsignarAlmacen);
            groupBox2.Controls.Add(this.btnEliminarGrupo);
            groupBox2.Controls.Add(this.btnCrearDetalle);
            groupBox2.Controls.Add(this.btnCrearGrupo);
            groupBox2.Controls.Add(this.grdData);
            groupBox2.Location = new System.Drawing.Point(6, 65);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(468, 450);
            groupBox2.TabIndex = 1;
            groupBox2.Text = "Resultado de Búsqueda";
            // 
            // btnAsignarAlmacen
            // 
            this.btnAsignarAlmacen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            appearance2.Image = global::SAMBHS.Windows.WinClient.UI.Resource.bricks;
            appearance2.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance2.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance2.TextHAlignAsString = "Right";
            appearance2.TextVAlignAsString = "Middle";
            this.btnAsignarAlmacen.Appearance = appearance2;
            this.btnAsignarAlmacen.Enabled = false;
            this.btnAsignarAlmacen.Location = new System.Drawing.Point(190, 415);
            this.btnAsignarAlmacen.Name = "btnAsignarAlmacen";
            this.btnAsignarAlmacen.Size = new System.Drawing.Size(119, 25);
            this.btnAsignarAlmacen.TabIndex = 5;
            this.btnAsignarAlmacen.Text = "Asignar Almacén";
            this.btnAsignarAlmacen.Click += new System.EventHandler(this.btnAsignarAlmacen_Click);
            // 
            // btnEliminarGrupo
            // 
            this.btnEliminarGrupo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            appearance3.Image = global::SAMBHS.Windows.WinClient.UI.Resource.delete;
            appearance3.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance3.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance3.TextHAlignAsString = "Right";
            appearance3.TextVAlignAsString = "Middle";
            this.btnEliminarGrupo.Appearance = appearance3;
            this.btnEliminarGrupo.Enabled = false;
            this.btnEliminarGrupo.Location = new System.Drawing.Point(315, 415);
            this.btnEliminarGrupo.Name = "btnEliminarGrupo";
            this.btnEliminarGrupo.Size = new System.Drawing.Size(117, 25);
            this.btnEliminarGrupo.TabIndex = 3;
            this.btnEliminarGrupo.Text = "Eliminar Grupo";
            this.btnEliminarGrupo.Click += new System.EventHandler(this.btnEliminarGrupo_Click);
            // 
            // btnCrearDetalle
            // 
            this.btnCrearDetalle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            appearance4.Image = global::SAMBHS.Windows.WinClient.UI.Resource.bricks;
            appearance4.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance4.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance4.TextHAlignAsString = "Right";
            appearance4.TextVAlignAsString = "Middle";
            this.btnCrearDetalle.Appearance = appearance4;
            this.btnCrearDetalle.Enabled = false;
            this.btnCrearDetalle.Location = new System.Drawing.Point(109, 415);
            this.btnCrearDetalle.Name = "btnCrearDetalle";
            this.btnCrearDetalle.Size = new System.Drawing.Size(75, 25);
            this.btnCrearDetalle.TabIndex = 2;
            this.btnCrearDetalle.Text = "Detalle";
            this.btnCrearDetalle.Click += new System.EventHandler(this.btnCrearDetalle_Click);
            // 
            // btnCrearGrupo
            // 
            this.btnCrearGrupo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            appearance5.Image = global::SAMBHS.Windows.WinClient.UI.Resource.brick_add;
            appearance5.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance5.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance5.TextHAlignAsString = "Right";
            appearance5.TextVAlignAsString = "Middle";
            this.btnCrearGrupo.Appearance = appearance5;
            this.btnCrearGrupo.Location = new System.Drawing.Point(6, 415);
            this.btnCrearGrupo.Name = "btnCrearGrupo";
            this.btnCrearGrupo.Size = new System.Drawing.Size(102, 25);
            this.btnCrearGrupo.TabIndex = 4;
            this.btnCrearGrupo.Text = "Nuevo Grupo";
            this.btnCrearGrupo.Click += new System.EventHandler(this.btnCrearGrupo_Click);
            // 
            // grdData
            // 
            this.grdData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdData.CausesValidation = false;
            appearance6.BackColor = System.Drawing.SystemColors.Window;
            appearance6.BackColor2 = System.Drawing.Color.LightSteelBlue;
            appearance6.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            this.grdData.DisplayLayout.Appearance = appearance6;
            this.grdData.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn1.Header.Caption = "Nombre";
            ultraGridColumn1.Header.VisiblePosition = 1;
            ultraGridColumn1.RowLayoutColumnInfo.OriginX = 0;
            ultraGridColumn1.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn1.RowLayoutColumnInfo.ParentGroupIndex = 0;
            ultraGridColumn1.RowLayoutColumnInfo.ParentGroupKey = "NewGroup0";
            ultraGridColumn1.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn1.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn1.Width = 142;
            ultraGridColumn2.Header.Caption = "Dirección";
            ultraGridColumn2.Header.VisiblePosition = 2;
            ultraGridColumn2.RowLayoutColumnInfo.OriginX = 2;
            ultraGridColumn2.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn2.RowLayoutColumnInfo.ParentGroupIndex = 0;
            ultraGridColumn2.RowLayoutColumnInfo.ParentGroupKey = "NewGroup0";
            ultraGridColumn2.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn2.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn2.Width = 99;
            ultraGridColumn3.Header.Caption = "Centro Costo";
            ultraGridColumn3.Header.VisiblePosition = 3;
            ultraGridColumn3.RowLayoutColumnInfo.OriginX = 4;
            ultraGridColumn3.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn3.RowLayoutColumnInfo.ParentGroupIndex = 0;
            ultraGridColumn3.RowLayoutColumnInfo.ParentGroupKey = "NewGroup0";
            ultraGridColumn3.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(53, 0);
            ultraGridColumn3.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn3.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn3.Width = 79;
            ultraGridColumn4.Header.Caption = "Usuario";
            ultraGridColumn4.Header.VisiblePosition = 4;
            ultraGridColumn4.RowLayoutColumnInfo.OriginX = 0;
            ultraGridColumn4.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn4.RowLayoutColumnInfo.ParentGroupIndex = 1;
            ultraGridColumn4.RowLayoutColumnInfo.ParentGroupKey = "NewGroup1";
            ultraGridColumn4.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(40, 0);
            ultraGridColumn4.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn4.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn4.Width = 70;
            ultraGridColumn5.Header.Caption = "Fecha";
            ultraGridColumn5.Header.VisiblePosition = 5;
            ultraGridColumn5.RowLayoutColumnInfo.OriginX = 2;
            ultraGridColumn5.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn5.RowLayoutColumnInfo.ParentGroupIndex = 1;
            ultraGridColumn5.RowLayoutColumnInfo.ParentGroupKey = "NewGroup1";
            ultraGridColumn5.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(49, 0);
            ultraGridColumn5.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn5.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn5.Width = 24;
            ultraGridColumn6.Header.Caption = "Usuario";
            ultraGridColumn6.Header.VisiblePosition = 6;
            ultraGridColumn6.RowLayoutColumnInfo.OriginX = 0;
            ultraGridColumn6.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn6.RowLayoutColumnInfo.ParentGroupIndex = 2;
            ultraGridColumn6.RowLayoutColumnInfo.ParentGroupKey = "NewGroup2";
            ultraGridColumn6.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(45, 0);
            ultraGridColumn6.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn6.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn6.Width = 13;
            ultraGridColumn7.Header.Caption = "Fecha";
            ultraGridColumn7.Header.VisiblePosition = 7;
            ultraGridColumn7.RowLayoutColumnInfo.OriginX = 2;
            ultraGridColumn7.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn7.RowLayoutColumnInfo.ParentGroupIndex = 2;
            ultraGridColumn7.RowLayoutColumnInfo.ParentGroupKey = "NewGroup2";
            ultraGridColumn7.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(48, 0);
            ultraGridColumn7.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn7.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn7.Width = 14;
            ultraGridColumn8.Header.VisiblePosition = 0;
            ultraGridColumn8.Hidden = true;
            ultraGridColumn8.RowLayoutColumnInfo.OriginX = 14;
            ultraGridColumn8.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn8.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn8.RowLayoutColumnInfo.SpanY = 2;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn8});
            ultraGridGroup1.Header.Caption = "Establecimiento";
            ultraGridGroup1.Key = "NewGroup0";
            ultraGridGroup1.RowLayoutGroupInfo.LabelSpan = 1;
            ultraGridGroup1.RowLayoutGroupInfo.OriginX = 0;
            ultraGridGroup1.RowLayoutGroupInfo.OriginY = 0;
            ultraGridGroup1.RowLayoutGroupInfo.SpanX = 6;
            ultraGridGroup1.RowLayoutGroupInfo.SpanY = 3;
            ultraGridGroup2.Header.Caption = "Creación";
            ultraGridGroup2.Key = "NewGroup1";
            ultraGridGroup2.RowLayoutGroupInfo.LabelSpan = 1;
            ultraGridGroup2.RowLayoutGroupInfo.OriginX = 6;
            ultraGridGroup2.RowLayoutGroupInfo.OriginY = 0;
            ultraGridGroup2.RowLayoutGroupInfo.SpanX = 4;
            ultraGridGroup2.RowLayoutGroupInfo.SpanY = 3;
            ultraGridGroup3.Header.Caption = "Actualización";
            ultraGridGroup3.Key = "NewGroup2";
            ultraGridGroup3.RowLayoutGroupInfo.LabelSpan = 1;
            ultraGridGroup3.RowLayoutGroupInfo.OriginX = 10;
            ultraGridGroup3.RowLayoutGroupInfo.OriginY = 0;
            ultraGridGroup3.RowLayoutGroupInfo.SpanX = 4;
            ultraGridGroup3.RowLayoutGroupInfo.SpanY = 3;
            ultraGridBand1.Groups.AddRange(new Infragistics.Win.UltraWinGrid.UltraGridGroup[] {
            ultraGridGroup1,
            ultraGridGroup2,
            ultraGridGroup3});
            ultraGridBand1.Override.SummaryFooterCaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            ultraGridBand1.RowLayoutStyle = Infragistics.Win.UltraWinGrid.RowLayoutStyle.GroupLayout;
            appearance7.Image = ((object)(resources.GetObject("appearance7.Image")));
            summarySettings1.Appearance = appearance7;
            summarySettings1.DisplayFormat = "{0} Registros";
            summarySettings1.GroupBySummaryValueAppearance = appearance8;
            summarySettings1.SummaryDisplayArea = Infragistics.Win.UltraWinGrid.SummaryDisplayAreas.BottomFixed;
            ultraGridBand1.Summaries.AddRange(new Infragistics.Win.UltraWinGrid.SummarySettings[] {
            summarySettings1});
            this.grdData.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdData.DisplayLayout.InterBandSpacing = 10;
            this.grdData.DisplayLayout.MaxColScrollRegions = 1;
            this.grdData.DisplayLayout.MaxRowScrollRegions = 1;
            this.grdData.DisplayLayout.NewColumnLoadStyle = Infragistics.Win.UltraWinGrid.NewColumnLoadStyle.Hide;
            this.grdData.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdData.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdData.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.grdData.DisplayLayout.Override.AllowRowSummaries = Infragistics.Win.UltraWinGrid.AllowRowSummaries.False;
            this.grdData.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            appearance9.BackColor = System.Drawing.Color.Transparent;
            this.grdData.DisplayLayout.Override.CardAreaAppearance = appearance9;
            appearance10.BackColor = System.Drawing.SystemColors.Control;
            appearance10.BackColor2 = System.Drawing.SystemColors.ControlLightLight;
            appearance10.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            this.grdData.DisplayLayout.Override.CellAppearance = appearance10;
            this.grdData.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.grdData.DisplayLayout.Override.ColumnSizingArea = Infragistics.Win.UltraWinGrid.ColumnSizingArea.HeadersOnly;
            appearance11.BackColor = System.Drawing.SystemColors.Control;
            appearance11.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance11.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            appearance11.ThemedElementAlpha = Infragistics.Win.Alpha.Transparent;
            this.grdData.DisplayLayout.Override.HeaderAppearance = appearance11;
            this.grdData.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance12.AlphaLevel = ((short)(187));
            this.grdData.DisplayLayout.Override.RowAlternateAppearance = appearance12;
            appearance13.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.grdData.DisplayLayout.Override.RowSelectorAppearance = appearance13;
            this.grdData.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            this.grdData.DisplayLayout.Override.RowSizing = Infragistics.Win.UltraWinGrid.RowSizing.Fixed;
            appearance14.BackColor = System.Drawing.SystemColors.Highlight;
            appearance14.BackColor2 = System.Drawing.SystemColors.ActiveCaption;
            appearance14.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            appearance14.FontData.BoldAsString = "True";
            this.grdData.DisplayLayout.Override.SelectedRowAppearance = appearance14;
            this.grdData.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            this.grdData.DisplayLayout.RowConnectorColor = System.Drawing.SystemColors.ControlDarkDark;
            this.grdData.DisplayLayout.RowConnectorStyle = Infragistics.Win.UltraWinGrid.RowConnectorStyle.Dashed;
            this.grdData.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdData.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdData.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.grdData.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdData.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.grdData.Location = new System.Drawing.Point(3, 30);
            this.grdData.Margin = new System.Windows.Forms.Padding(2);
            this.grdData.Name = "grdData";
            this.grdData.Size = new System.Drawing.Size(462, 377);
            this.grdData.TabIndex = 0;
            this.grdData.DoubleClickCell += new Infragistics.Win.UltraWinGrid.DoubleClickCellEventHandler(this.grdData_DoubleClickCell);
            this.grdData.MouseDown += new System.Windows.Forms.MouseEventHandler(this.grdData_MouseDown);
            // 
            // ultraPanel1
            // 
            // 
            // ultraPanel1.ClientArea
            // 
            ultraPanel1.ClientArea.Controls.Add(groupBox1);
            ultraPanel1.ClientArea.Controls.Add(groupBox2);
            ultraPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            ultraPanel1.Location = new System.Drawing.Point(8, 31);
            ultraPanel1.Name = "ultraPanel1";
            ultraPanel1.Size = new System.Drawing.Size(480, 521);
            ultraPanel1.TabIndex = 8;
            // 
            // ultraFlowLayoutManager1
            // 
            ultraFlowLayoutManager1.ContainerControl = this.pnRight;
            ultraFlowLayoutManager1.HorizontalAlignment = Infragistics.Win.Layout.DefaultableFlowLayoutAlignment.Center;
            ultraFlowLayoutManager1.Orientation = System.Windows.Forms.Orientation.Vertical;
            ultraFlowLayoutManager1.VerticalAlignment = Infragistics.Win.Layout.DefaultableFlowLayoutAlignment.Near;
            // 
            // pnRight
            // 
            this.pnRight.Controls.Add(this.gbGrupo);
            this.pnRight.Controls.Add(this.gbAlmacenes);
            this.pnRight.Controls.Add(this.gbDetalle);
            this.pnRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnRight.Location = new System.Drawing.Point(494, 31);
            this.pnRight.Name = "pnRight";
            this.pnRight.Size = new System.Drawing.Size(707, 521);
            this.pnRight.TabIndex = 6;
            // 
            // gbGrupo
            // 
            this.gbGrupo.Controls.Add(this.txtCentroCosto);
            this.gbGrupo.Controls.Add(this.label4);
            this.gbGrupo.Controls.Add(this.btnGrabar);
            this.gbGrupo.Controls.Add(this.txtGrupoDireccion);
            this.gbGrupo.Controls.Add(this.txtGrupoNombre);
            this.gbGrupo.Controls.Add(this.label7);
            this.gbGrupo.Controls.Add(this.label6);
            this.gbGrupo.Location = new System.Drawing.Point(5, 5);
            this.gbGrupo.Name = "gbGrupo";
            this.gbGrupo.Size = new System.Drawing.Size(692, 118);
            this.gbGrupo.TabIndex = 0;
            this.gbGrupo.Text = "Datos de Establecimiento";
            // 
            // txtCentroCosto
            // 
            appearance15.Image = global::SAMBHS.Windows.WinClient.UI.Properties.Resources.find;
            editorButton1.Appearance = appearance15;
            editorButton1.Key = "btnBuscar";
            this.txtCentroCosto.ButtonsLeft.Add(editorButton1);
            this.txtCentroCosto.Location = new System.Drawing.Point(106, 77);
            this.txtCentroCosto.Name = "txtCentroCosto";
            this.txtCentroCosto.ReadOnly = true;
            this.txtCentroCosto.Size = new System.Drawing.Size(308, 21);
            this.txtCentroCosto.TabIndex = 6;
            ultraToolTipInfo1.ToolTipImage = Infragistics.Win.ToolTipImage.Info;
            ultraToolTipInfo1.ToolTipText = "Se especifica cuál será el centro de costo predeterminado para las ventas rápidas" +
    ".";
            ultraToolTipInfo1.ToolTipTitle = "Información";
            this.ultraToolTipManager1.SetUltraToolTip(this.txtCentroCosto, ultraToolTipInfo1);
            this.txtCentroCosto.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.ultraTextEditor1_EditorButtonClick);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 82);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 14);
            this.label4.TabIndex = 5;
            this.label4.Text = "Centro de Costo:";
            // 
            // btnGrabar
            // 
            this.btnGrabar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance16.Image = global::SAMBHS.Windows.WinClient.UI.Resource.system_save;
            appearance16.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance16.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance16.TextHAlignAsString = "Right";
            appearance16.TextVAlignAsString = "Middle";
            this.btnGrabar.Appearance = appearance16;
            this.btnGrabar.Enabled = false;
            this.btnGrabar.Location = new System.Drawing.Point(611, 84);
            this.btnGrabar.Name = "btnGrabar";
            this.btnGrabar.Size = new System.Drawing.Size(75, 25);
            this.btnGrabar.TabIndex = 2;
            this.btnGrabar.Text = "Grabar";
            this.btnGrabar.Click += new System.EventHandler(this.btnGrabar_Click);
            // 
            // txtGrupoDireccion
            // 
            this.txtGrupoDireccion.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtGrupoDireccion.Location = new System.Drawing.Point(106, 51);
            this.txtGrupoDireccion.Name = "txtGrupoDireccion";
            this.txtGrupoDireccion.Size = new System.Drawing.Size(356, 21);
            this.txtGrupoDireccion.TabIndex = 1;
            // 
            // txtGrupoNombre
            // 
            this.txtGrupoNombre.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtGrupoNombre.Location = new System.Drawing.Point(106, 25);
            this.txtGrupoNombre.Name = "txtGrupoNombre";
            this.txtGrupoNombre.Size = new System.Drawing.Size(356, 21);
            this.txtGrupoNombre.TabIndex = 0;
            this.uvGrupo.GetValidationSettings(this.txtGrupoNombre).DataType = typeof(string);
            this.uvGrupo.GetValidationSettings(this.txtGrupoNombre).EmptyValueCriteria = Infragistics.Win.Misc.EmptyValueCriteria.NullOrEmptyString;
            this.uvGrupo.GetValidationSettings(this.txtGrupoNombre).IsRequired = true;
            this.uvGrupo.GetValidationSettings(this.txtGrupoNombre).ValidationGroupKey = "Grupo";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(14, 54);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 14);
            this.label7.TabIndex = 4;
            this.label7.Text = "Dirección:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 28);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(48, 14);
            this.label6.TabIndex = 3;
            this.label6.Text = "Nombre:";
            // 
            // gbAlmacenes
            // 
            this.gbAlmacenes.Controls.Add(this.cboEstablecimientoAlmacen);
            this.gbAlmacenes.Controls.Add(this.btnEliminarAlmacen);
            this.gbAlmacenes.Controls.Add(this.btnEditarAlmacen);
            this.gbAlmacenes.Controls.Add(this.groupBox4);
            this.gbAlmacenes.Controls.Add(this.btnAgregarEstablecimientoAlmacen);
            this.gbAlmacenes.Controls.Add(this.label3);
            this.gbAlmacenes.Location = new System.Drawing.Point(7, 128);
            this.gbAlmacenes.Name = "gbAlmacenes";
            this.gbAlmacenes.Size = new System.Drawing.Size(688, 154);
            this.gbAlmacenes.TabIndex = 5;
            this.gbAlmacenes.Text = "Asignar Almacenes";
            // 
            // cboEstablecimientoAlmacen
            // 
            this.cboEstablecimientoAlmacen.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboEstablecimientoAlmacen.Location = new System.Drawing.Point(104, 20);
            this.cboEstablecimientoAlmacen.Name = "cboEstablecimientoAlmacen";
            this.cboEstablecimientoAlmacen.Size = new System.Drawing.Size(178, 21);
            this.cboEstablecimientoAlmacen.TabIndex = 31;
            this.uvGrupo.GetValidationSettings(this.cboEstablecimientoAlmacen).Condition = new Infragistics.Win.OperatorCondition(Infragistics.Win.ConditionOperator.NotEquals, "-1", true, typeof(string));
            this.uvGrupo.GetValidationSettings(this.cboEstablecimientoAlmacen).IsRequired = true;
            this.uvGrupo.GetValidationSettings(this.cboEstablecimientoAlmacen).ValidationGroupKey = "Almacen";
            // 
            // btnEliminarAlmacen
            // 
            this.btnEliminarAlmacen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance17.Image = global::SAMBHS.Windows.WinClient.UI.Resource.delete;
            appearance17.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance17.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance17.TextHAlignAsString = "Right";
            appearance17.TextVAlignAsString = "Middle";
            this.btnEliminarAlmacen.Appearance = appearance17;
            this.btnEliminarAlmacen.Enabled = false;
            this.btnEliminarAlmacen.Location = new System.Drawing.Point(605, 117);
            this.btnEliminarAlmacen.Name = "btnEliminarAlmacen";
            this.btnEliminarAlmacen.Size = new System.Drawing.Size(75, 25);
            this.btnEliminarAlmacen.TabIndex = 32;
            this.btnEliminarAlmacen.Text = "Eliminar";
            this.btnEliminarAlmacen.Click += new System.EventHandler(this.btnEliminarAlmacen_Click);
            // 
            // btnEditarAlmacen
            // 
            this.btnEditarAlmacen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance18.Image = global::SAMBHS.Windows.WinClient.UI.Resource.pencil;
            appearance18.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance18.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance18.TextHAlignAsString = "Right";
            appearance18.TextVAlignAsString = "Middle";
            this.btnEditarAlmacen.Appearance = appearance18;
            this.btnEditarAlmacen.Enabled = false;
            this.btnEditarAlmacen.Location = new System.Drawing.Point(524, 117);
            this.btnEditarAlmacen.Name = "btnEditarAlmacen";
            this.btnEditarAlmacen.Size = new System.Drawing.Size(75, 25);
            this.btnEditarAlmacen.TabIndex = 31;
            this.btnEditarAlmacen.Text = "Editar";
            this.btnEditarAlmacen.Visible = false;
            this.btnEditarAlmacen.Click += new System.EventHandler(this.btnEditarAlmacen_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.grdAlmacen);
            this.groupBox4.Location = new System.Drawing.Point(6, 43);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(673, 68);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.Text = "Detalle";
            // 
            // grdAlmacen
            // 
            this.grdAlmacen.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdAlmacen.CausesValidation = false;
            appearance19.BackColor = System.Drawing.SystemColors.Window;
            appearance19.BackColor2 = System.Drawing.Color.LightSteelBlue;
            appearance19.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            this.grdAlmacen.DisplayLayout.Appearance = appearance19;
            this.grdAlmacen.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn9.Header.Caption = "Almacén";
            ultraGridColumn9.Header.VisiblePosition = 0;
            ultraGridColumn9.Width = 625;
            ultraGridBand2.Columns.AddRange(new object[] {
            ultraGridColumn9});
            this.grdAlmacen.DisplayLayout.BandsSerializer.Add(ultraGridBand2);
            this.grdAlmacen.DisplayLayout.InterBandSpacing = 10;
            this.grdAlmacen.DisplayLayout.MaxColScrollRegions = 1;
            this.grdAlmacen.DisplayLayout.MaxRowScrollRegions = 1;
            this.grdAlmacen.DisplayLayout.NewColumnLoadStyle = Infragistics.Win.UltraWinGrid.NewColumnLoadStyle.Hide;
            this.grdAlmacen.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdAlmacen.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdAlmacen.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.grdAlmacen.DisplayLayout.Override.AllowRowSummaries = Infragistics.Win.UltraWinGrid.AllowRowSummaries.False;
            this.grdAlmacen.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            appearance20.BackColor = System.Drawing.Color.Transparent;
            this.grdAlmacen.DisplayLayout.Override.CardAreaAppearance = appearance20;
            appearance21.BackColor = System.Drawing.SystemColors.Control;
            appearance21.BackColor2 = System.Drawing.SystemColors.ControlLightLight;
            appearance21.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            this.grdAlmacen.DisplayLayout.Override.CellAppearance = appearance21;
            this.grdAlmacen.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            appearance22.BackColor = System.Drawing.SystemColors.Control;
            appearance22.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance22.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            appearance22.ThemedElementAlpha = Infragistics.Win.Alpha.Transparent;
            this.grdAlmacen.DisplayLayout.Override.HeaderAppearance = appearance22;
            this.grdAlmacen.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance23.AlphaLevel = ((short)(187));
            appearance23.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            appearance23.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.grdAlmacen.DisplayLayout.Override.RowAlternateAppearance = appearance23;
            appearance24.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.grdAlmacen.DisplayLayout.Override.RowSelectorAppearance = appearance24;
            this.grdAlmacen.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            appearance25.BackColor = System.Drawing.SystemColors.Highlight;
            appearance25.BackColor2 = System.Drawing.SystemColors.ActiveCaption;
            appearance25.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            appearance25.FontData.BoldAsString = "True";
            this.grdAlmacen.DisplayLayout.Override.SelectedRowAppearance = appearance25;
            this.grdAlmacen.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            this.grdAlmacen.DisplayLayout.RowConnectorColor = System.Drawing.SystemColors.ControlDarkDark;
            this.grdAlmacen.DisplayLayout.RowConnectorStyle = Infragistics.Win.UltraWinGrid.RowConnectorStyle.Dashed;
            this.grdAlmacen.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdAlmacen.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdAlmacen.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.grdAlmacen.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdAlmacen.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.grdAlmacen.Location = new System.Drawing.Point(5, 18);
            this.grdAlmacen.Margin = new System.Windows.Forms.Padding(2);
            this.grdAlmacen.Name = "grdAlmacen";
            this.grdAlmacen.Size = new System.Drawing.Size(663, 45);
            this.grdAlmacen.TabIndex = 0;
            this.grdAlmacen.MouseDown += new System.Windows.Forms.MouseEventHandler(this.grdAlmacen_MouseDown);
            // 
            // btnAgregarEstablecimientoAlmacen
            // 
            appearance26.Image = global::SAMBHS.Windows.WinClient.UI.Resource.add;
            appearance26.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance26.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance26.TextHAlignAsString = "Right";
            appearance26.TextVAlignAsString = "Middle";
            this.btnAgregarEstablecimientoAlmacen.Appearance = appearance26;
            this.btnAgregarEstablecimientoAlmacen.Enabled = false;
            this.btnAgregarEstablecimientoAlmacen.Location = new System.Drawing.Point(349, 20);
            this.btnAgregarEstablecimientoAlmacen.Name = "btnAgregarEstablecimientoAlmacen";
            this.btnAgregarEstablecimientoAlmacen.Size = new System.Drawing.Size(133, 23);
            this.btnAgregarEstablecimientoAlmacen.TabIndex = 31;
            this.btnAgregarEstablecimientoAlmacen.Text = "Agregar y Guardar";
            this.btnAgregarEstablecimientoAlmacen.Click += new System.EventHandler(this.btnAgregarEstablecimientoAlmacen_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 14);
            this.label3.TabIndex = 32;
            this.label3.Text = "Almacén :";
            // 
            // gbDetalle
            // 
            this.gbDetalle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbDetalle.Controls.Add(this.txtNombreImpresora);
            this.gbDetalle.Controls.Add(this.cboDetalleAlmacen);
            this.gbDetalle.Controls.Add(this.cboDetalleDocumento);
            this.gbDetalle.Controls.Add(this.txtMaxNumeroItems);
            this.gbDetalle.Controls.Add(this.ultraLabel2);
            this.gbDetalle.Controls.Add(this.uckDocumentoPredeterminado);
            this.gbDetalle.Controls.Add(this.ultraLabel1);
            this.gbDetalle.Controls.Add(this.uckImpresionPrevia);
            this.gbDetalle.Controls.Add(this.label2);
            this.gbDetalle.Controls.Add(this.txtDetalleCorrelativoDocIni);
            this.gbDetalle.Controls.Add(this.txtDetalleSerie);
            this.gbDetalle.Controls.Add(this.btnEliminar);
            this.gbDetalle.Controls.Add(this.btnEditar);
            this.gbDetalle.Controls.Add(this.btnAgregar);
            this.gbDetalle.Controls.Add(this.groupBox5);
            this.gbDetalle.Controls.Add(this.label14);
            this.gbDetalle.Controls.Add(this.label11);
            this.gbDetalle.Controls.Add(this.label10);
            this.gbDetalle.Location = new System.Drawing.Point(7, 287);
            this.gbDetalle.Name = "gbDetalle";
            this.gbDetalle.Size = new System.Drawing.Size(688, 184);
            this.gbDetalle.TabIndex = 1;
            this.gbDetalle.Text = "Detalle de Establecimiento";
            // 
            // txtNombreImpresora
            // 
            this.txtNombreImpresora.DropDownListWidth = 300;
            this.txtNombreImpresora.Location = new System.Drawing.Point(116, 68);
            this.txtNombreImpresora.Name = "txtNombreImpresora";
            this.txtNombreImpresora.Size = new System.Drawing.Size(184, 21);
            this.txtNombreImpresora.TabIndex = 4;
            this.txtNombreImpresora.BeforeDropDown += new System.ComponentModel.CancelEventHandler(this.txtNombreImpresora_BeforeDropDown);
            // 
            // cboDetalleAlmacen
            // 
            this.cboDetalleAlmacen.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboDetalleAlmacen.Location = new System.Drawing.Point(116, 44);
            this.cboDetalleAlmacen.Name = "cboDetalleAlmacen";
            this.cboDetalleAlmacen.Size = new System.Drawing.Size(184, 21);
            this.cboDetalleAlmacen.TabIndex = 2;
            this.uvGrupo.GetValidationSettings(this.cboDetalleAlmacen).Condition = new Infragistics.Win.OperatorCondition(Infragistics.Win.ConditionOperator.NotEquals, "-1", true, typeof(string));
            this.uvGrupo.GetValidationSettings(this.cboDetalleAlmacen).IsRequired = true;
            this.uvGrupo.GetValidationSettings(this.cboDetalleAlmacen).ValidationGroupKey = "Detalle";
            // 
            // cboDetalleDocumento
            // 
            this.cboDetalleDocumento.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboDetalleDocumento.Location = new System.Drawing.Point(116, 20);
            this.cboDetalleDocumento.Name = "cboDetalleDocumento";
            this.cboDetalleDocumento.Size = new System.Drawing.Size(184, 21);
            this.cboDetalleDocumento.TabIndex = 0;
            this.uvGrupo.GetValidationSettings(this.cboDetalleDocumento).Condition = new Infragistics.Win.OperatorCondition(Infragistics.Win.ConditionOperator.NotEquals, "-1", true, typeof(string));
            this.uvGrupo.GetValidationSettings(this.cboDetalleDocumento).IsRequired = true;
            this.uvGrupo.GetValidationSettings(this.cboDetalleDocumento).ValidationGroupKey = "Detalle";
            this.cboDetalleDocumento.ValueChanged += new System.EventHandler(this.cboDetalleDocumento_ValueChanged);
            // 
            // txtMaxNumeroItems
            // 
            appearance27.TextHAlignAsString = "Center";
            this.txtMaxNumeroItems.Appearance = appearance27;
            this.txtMaxNumeroItems.Location = new System.Drawing.Point(402, 69);
            this.txtMaxNumeroItems.Margin = new System.Windows.Forms.Padding(2);
            this.txtMaxNumeroItems.MaxLength = 8;
            this.txtMaxNumeroItems.Name = "txtMaxNumeroItems";
            this.txtMaxNumeroItems.Size = new System.Drawing.Size(114, 21);
            this.txtMaxNumeroItems.TabIndex = 5;
            this.txtMaxNumeroItems.Text = "0";
            this.uvGrupo.GetValidationSettings(this.txtMaxNumeroItems).EmptyValueCriteria = Infragistics.Win.Misc.EmptyValueCriteria.NullOrEmptyString;
            this.uvGrupo.GetValidationSettings(this.txtMaxNumeroItems).Enabled = false;
            this.uvGrupo.GetValidationSettings(this.txtMaxNumeroItems).IsRequired = true;
            this.uvGrupo.GetValidationSettings(this.txtMaxNumeroItems).ValidationGroupKey = "Detalle";
            this.txtMaxNumeroItems.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMaxNumeroItems_KeyPress);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(309, 71);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(94, 14);
            this.ultraLabel2.TabIndex = 36;
            this.ultraLabel2.Text = "Max. num. Items :";
            // 
            // uckDocumentoPredeterminado
            // 
            this.uckDocumentoPredeterminado.Location = new System.Drawing.Point(521, 44);
            this.uckDocumentoPredeterminado.Name = "uckDocumentoPredeterminado";
            this.uckDocumentoPredeterminado.Size = new System.Drawing.Size(163, 20);
            this.uckDocumentoPredeterminado.TabIndex = 7;
            this.uckDocumentoPredeterminado.Text = "Documento Predeterminado";
            ultraToolTipInfo2.ToolTipText = "Con ésta serie se generará la Venta  para este Tipo Documento";
            this.ultraToolTipManager1.SetUltraToolTip(this.uckDocumentoPredeterminado, ultraToolTipInfo2);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(11, 71);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(105, 14);
            this.ultraLabel1.TabIndex = 32;
            this.ultraLabel1.Text = "Nombre Impresora :";
            // 
            // uckImpresionPrevia
            // 
            this.uckImpresionPrevia.Location = new System.Drawing.Point(521, 21);
            this.uckImpresionPrevia.Name = "uckImpresionPrevia";
            this.uckImpresionPrevia.Size = new System.Drawing.Size(160, 20);
            this.uckImpresionPrevia.TabIndex = 6;
            this.uckImpresionPrevia.Text = "Impresión  con Vista Previa";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 14);
            this.label2.TabIndex = 30;
            this.label2.Text = "Almacén :";
            // 
            // txtDetalleCorrelativoDocIni
            // 
            appearance28.TextHAlignAsString = "Center";
            this.txtDetalleCorrelativoDocIni.Appearance = appearance28;
            this.txtDetalleCorrelativoDocIni.Location = new System.Drawing.Point(402, 44);
            this.txtDetalleCorrelativoDocIni.Margin = new System.Windows.Forms.Padding(2);
            this.txtDetalleCorrelativoDocIni.MaxLength = 8;
            this.txtDetalleCorrelativoDocIni.Name = "txtDetalleCorrelativoDocIni";
            this.txtDetalleCorrelativoDocIni.Size = new System.Drawing.Size(114, 21);
            this.txtDetalleCorrelativoDocIni.TabIndex = 3;
            this.uvGrupo.GetValidationSettings(this.txtDetalleCorrelativoDocIni).EmptyValueCriteria = Infragistics.Win.Misc.EmptyValueCriteria.NullOrEmptyString;
            this.uvGrupo.GetValidationSettings(this.txtDetalleCorrelativoDocIni).Enabled = false;
            this.uvGrupo.GetValidationSettings(this.txtDetalleCorrelativoDocIni).IsRequired = true;
            this.uvGrupo.GetValidationSettings(this.txtDetalleCorrelativoDocIni).ValidationGroupKey = "Detalle";
            this.txtDetalleCorrelativoDocIni.TextChanged += new System.EventHandler(this.txtDetalleCorrelativoDocIni_TextChanged);
            this.txtDetalleCorrelativoDocIni.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDetalleCorrelativoDocIni_KeyPress);
            this.txtDetalleCorrelativoDocIni.Validated += new System.EventHandler(this.txtDetalleCorrelativoDocIni_Validated);
            // 
            // txtDetalleSerie
            // 
            appearance29.TextHAlignAsString = "Center";
            this.txtDetalleSerie.Appearance = appearance29;
            this.txtDetalleSerie.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtDetalleSerie.Location = new System.Drawing.Point(402, 20);
            this.txtDetalleSerie.Margin = new System.Windows.Forms.Padding(2);
            this.txtDetalleSerie.MaxLength = 4;
            this.txtDetalleSerie.Name = "txtDetalleSerie";
            this.txtDetalleSerie.Size = new System.Drawing.Size(114, 21);
            this.txtDetalleSerie.TabIndex = 1;
            this.uvGrupo.GetValidationSettings(this.txtDetalleSerie).EmptyValueCriteria = Infragistics.Win.Misc.EmptyValueCriteria.NullOrEmptyString;
            this.uvGrupo.GetValidationSettings(this.txtDetalleSerie).Enabled = false;
            this.uvGrupo.GetValidationSettings(this.txtDetalleSerie).IsRequired = true;
            this.uvGrupo.GetValidationSettings(this.txtDetalleSerie).ValidationGroupKey = "Detalle";
            this.txtDetalleSerie.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDetalleSerie_KeyPress);
            this.txtDetalleSerie.Validated += new System.EventHandler(this.txtDetalleSerie_Validated);
            // 
            // btnEliminar
            // 
            this.btnEliminar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance30.Image = global::SAMBHS.Windows.WinClient.UI.Resource.delete;
            appearance30.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance30.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance30.TextHAlignAsString = "Right";
            appearance30.TextVAlignAsString = "Middle";
            this.btnEliminar.Appearance = appearance30;
            this.btnEliminar.Enabled = false;
            this.btnEliminar.Location = new System.Drawing.Point(607, 155);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(75, 25);
            this.btnEliminar.TabIndex = 10;
            this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // btnEditar
            // 
            this.btnEditar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance31.Image = global::SAMBHS.Windows.WinClient.UI.Resource.pencil;
            appearance31.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance31.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance31.TextHAlignAsString = "Right";
            appearance31.TextVAlignAsString = "Middle";
            this.btnEditar.Appearance = appearance31;
            this.btnEditar.Enabled = false;
            this.btnEditar.Location = new System.Drawing.Point(526, 155);
            this.btnEditar.Name = "btnEditar";
            this.btnEditar.Size = new System.Drawing.Size(75, 25);
            this.btnEditar.TabIndex = 9;
            this.btnEditar.Text = "Editar";
            this.btnEditar.Click += new System.EventHandler(this.btnEditar_Click);
            // 
            // btnAgregar
            // 
            this.btnAgregar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance32.Image = global::SAMBHS.Windows.WinClient.UI.Resource.add;
            appearance32.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance32.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance32.TextHAlignAsString = "Right";
            appearance32.TextVAlignAsString = "Middle";
            this.btnAgregar.Appearance = appearance32;
            this.btnAgregar.Enabled = false;
            this.btnAgregar.Location = new System.Drawing.Point(547, 79);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(133, 25);
            this.btnAgregar.TabIndex = 8;
            this.btnAgregar.Text = "Agregar y Guardar";
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.grdDetalle);
            this.groupBox5.Location = new System.Drawing.Point(7, 98);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(673, 51);
            this.groupBox5.TabIndex = 5;
            this.groupBox5.Text = "Detalle";
            // 
            // grdDetalle
            // 
            this.grdDetalle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdDetalle.CausesValidation = false;
            appearance33.BackColor = System.Drawing.SystemColors.Window;
            appearance33.BackColor2 = System.Drawing.Color.LightSteelBlue;
            appearance33.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            this.grdDetalle.DisplayLayout.Appearance = appearance33;
            this.grdDetalle.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn10.Header.VisiblePosition = 0;
            ultraGridColumn10.Hidden = true;
            ultraGridColumn10.Width = 346;
            ultraGridColumn11.Header.Caption = "Almacén";
            ultraGridColumn11.Header.VisiblePosition = 1;
            ultraGridColumn11.Width = 84;
            ultraGridColumn12.Header.Caption = "VistaPrevia";
            ultraGridColumn12.Header.VisiblePosition = 5;
            ultraGridColumn12.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
            ultraGridColumn12.Width = 72;
            ultraGridColumn13.Header.Caption = "Nombre Impresora";
            ultraGridColumn13.Header.VisiblePosition = 6;
            ultraGridColumn13.Width = 84;
            ultraGridColumn14.Header.Caption = "Doc. Predeterminado";
            ultraGridColumn14.Header.VisiblePosition = 7;
            ultraGridColumn14.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
            ultraGridColumn14.Width = 120;
            ultraGridColumn15.Header.Caption = "Número  Items";
            ultraGridColumn15.Header.VisiblePosition = 8;
            ultraGridColumn15.Width = 84;
            ultraGridColumn16.Header.Caption = "Documento";
            ultraGridColumn16.Header.VisiblePosition = 2;
            ultraGridColumn16.Width = 71;
            ultraGridColumn17.Header.Caption = "Nro Serie";
            ultraGridColumn17.Header.VisiblePosition = 3;
            ultraGridColumn17.Width = 57;
            ultraGridColumn18.Header.Caption = "Correlativo";
            ultraGridColumn18.Header.VisiblePosition = 4;
            ultraGridColumn18.Width = 53;
            ultraGridBand3.Columns.AddRange(new object[] {
            ultraGridColumn10,
            ultraGridColumn11,
            ultraGridColumn12,
            ultraGridColumn13,
            ultraGridColumn14,
            ultraGridColumn15,
            ultraGridColumn16,
            ultraGridColumn17,
            ultraGridColumn18});
            this.grdDetalle.DisplayLayout.BandsSerializer.Add(ultraGridBand3);
            this.grdDetalle.DisplayLayout.InterBandSpacing = 10;
            this.grdDetalle.DisplayLayout.MaxColScrollRegions = 1;
            this.grdDetalle.DisplayLayout.MaxRowScrollRegions = 1;
            this.grdDetalle.DisplayLayout.NewColumnLoadStyle = Infragistics.Win.UltraWinGrid.NewColumnLoadStyle.Hide;
            this.grdDetalle.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdDetalle.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdDetalle.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.grdDetalle.DisplayLayout.Override.AllowRowSummaries = Infragistics.Win.UltraWinGrid.AllowRowSummaries.False;
            this.grdDetalle.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            appearance34.BackColor = System.Drawing.Color.Transparent;
            this.grdDetalle.DisplayLayout.Override.CardAreaAppearance = appearance34;
            appearance35.BackColor = System.Drawing.SystemColors.Control;
            appearance35.BackColor2 = System.Drawing.SystemColors.ControlLightLight;
            appearance35.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            this.grdDetalle.DisplayLayout.Override.CellAppearance = appearance35;
            this.grdDetalle.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            appearance36.BackColor = System.Drawing.SystemColors.Control;
            appearance36.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance36.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            appearance36.ThemedElementAlpha = Infragistics.Win.Alpha.Transparent;
            this.grdDetalle.DisplayLayout.Override.HeaderAppearance = appearance36;
            this.grdDetalle.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance37.AlphaLevel = ((short)(187));
            appearance37.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            appearance37.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.grdDetalle.DisplayLayout.Override.RowAlternateAppearance = appearance37;
            appearance38.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.grdDetalle.DisplayLayout.Override.RowSelectorAppearance = appearance38;
            this.grdDetalle.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            appearance39.BackColor = System.Drawing.SystemColors.Highlight;
            appearance39.BackColor2 = System.Drawing.SystemColors.ActiveCaption;
            appearance39.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            appearance39.FontData.BoldAsString = "True";
            this.grdDetalle.DisplayLayout.Override.SelectedRowAppearance = appearance39;
            this.grdDetalle.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            this.grdDetalle.DisplayLayout.RowConnectorColor = System.Drawing.SystemColors.ControlDarkDark;
            this.grdDetalle.DisplayLayout.RowConnectorStyle = Infragistics.Win.UltraWinGrid.RowConnectorStyle.Dashed;
            this.grdDetalle.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdDetalle.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdDetalle.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.grdDetalle.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdDetalle.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.grdDetalle.Location = new System.Drawing.Point(5, 18);
            this.grdDetalle.Margin = new System.Windows.Forms.Padding(2);
            this.grdDetalle.Name = "grdDetalle";
            this.grdDetalle.Size = new System.Drawing.Size(663, 28);
            this.grdDetalle.TabIndex = 11;
            this.grdDetalle.AfterExitEditMode += new System.EventHandler(this.grdDetalle_AfterExitEditMode);
            this.grdDetalle.AfterRowActivate += new System.EventHandler(this.grdDetalle_AfterRowActivate);
            this.grdDetalle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.grdDetalle_MouseDown);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(14, 22);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(93, 14);
            this.label14.TabIndex = 12;
            this.label14.Text = "Tipo Documento :";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(309, 46);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(65, 14);
            this.label11.TabIndex = 15;
            this.label11.Text = "Correlativo :";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(316, 22);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(58, 14);
            this.label10.TabIndex = 16;
            this.label10.Text = "Nro Serie :";
            // 
            // uvGrupo
            // 
            validationGroup1.Key = "Grupo";
            validationGroup2.Key = "Detalle";
            validationGroup3.Key = "Almacen";
            this.uvGrupo.ValidationGroups.Add(validationGroup1);
            this.uvGrupo.ValidationGroups.Add(validationGroup2);
            this.uvGrupo.ValidationGroups.Add(validationGroup3);
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // _frmEstablecimiento_UltraFormManager_Dock_Area_Left
            // 
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 31);
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Left.Name = "_frmEstablecimiento_UltraFormManager_Dock_Area_Left";
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 521);
            // 
            // _frmEstablecimiento_UltraFormManager_Dock_Area_Right
            // 
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(1201, 31);
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Right.Name = "_frmEstablecimiento_UltraFormManager_Dock_Area_Right";
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 521);
            // 
            // _frmEstablecimiento_UltraFormManager_Dock_Area_Top
            // 
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Top.Name = "_frmEstablecimiento_UltraFormManager_Dock_Area_Top";
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(1209, 31);
            // 
            // _frmEstablecimiento_UltraFormManager_Dock_Area_Bottom
            // 
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 552);
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Bottom.Name = "_frmEstablecimiento_UltraFormManager_Dock_Area_Bottom";
            this._frmEstablecimiento_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(1209, 8);
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // ultraSplitter1
            // 
            this.ultraSplitter1.BackColor = System.Drawing.SystemColors.Window;
            this.ultraSplitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.ultraSplitter1.Location = new System.Drawing.Point(488, 31);
            this.ultraSplitter1.Name = "ultraSplitter1";
            this.ultraSplitter1.RestoreExtent = 713;
            this.ultraSplitter1.Size = new System.Drawing.Size(6, 521);
            this.ultraSplitter1.TabIndex = 7;
            // 
            // frmEstablecimiento
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(1209, 560);
            this.Controls.Add(ultraPanel1);
            this.Controls.Add(this.ultraSplitter1);
            this.Controls.Add(this.pnRight);
            this.Controls.Add(this._frmEstablecimiento_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._frmEstablecimiento_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._frmEstablecimiento_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._frmEstablecimiento_UltraFormManager_Dock_Area_Bottom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmEstablecimiento";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Administración de Establecimiento";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmEstablecimiento_FormClosing);
            this.Load += new System.EventHandler(this.frmEstablecimiento_Load);
            ((System.ComponentModel.ISupportInitialize)(groupBox1)).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtEstablecimiento)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(groupBox2)).EndInit();
            groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).EndInit();
            ultraPanel1.ClientArea.ResumeLayout(false);
            ultraPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(ultraFlowLayoutManager1)).EndInit();
            this.pnRight.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gbGrupo)).EndInit();
            this.gbGrupo.ResumeLayout(false);
            this.gbGrupo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCentroCosto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGrupoDireccion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGrupoNombre)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gbAlmacenes)).EndInit();
            this.gbAlmacenes.ResumeLayout(false);
            this.gbAlmacenes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboEstablecimientoAlmacen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox4)).EndInit();
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdAlmacen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gbDetalle)).EndInit();
            this.gbDetalle.ResumeLayout(false);
            this.gbDetalle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtNombreImpresora)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDetalleAlmacen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDetalleDocumento)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxNumeroItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uckDocumentoPredeterminado)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uckImpresionPrevia)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDetalleCorrelativoDocIni)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDetalleSerie)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox5)).EndInit();
            this.groupBox5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdDetalle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uvGrupo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox groupBox5;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdDetalle;
        private Infragistics.Win.Misc.UltraLabel label14;
        private Infragistics.Win.Misc.UltraLabel label11;
        private Infragistics.Win.Misc.UltraValidator uvGrupo;
        private Infragistics.Win.Misc.UltraLabel label10;
        private Infragistics.Win.Misc.UltraButton btnEliminarGrupo;
        private Infragistics.Win.Misc.UltraButton btnCrearDetalle;
        private Infragistics.Win.Misc.UltraButton btnCrearGrupo;
        private Infragistics.Win.Misc.UltraButton btnBuscar;
        private Infragistics.Win.Misc.UltraButton btnEliminar;
        private Infragistics.Win.Misc.UltraButton btnAgregar;
        private Infragistics.Win.Misc.UltraGroupBox gbDetalle;
        private Infragistics.Win.Misc.UltraButton btnEditar;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtEstablecimiento;
        private Infragistics.Win.Misc.UltraLabel label1;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdData;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtDetalleSerie;
        private Infragistics.Win.Misc.UltraLabel label2;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtDetalleCorrelativoDocIni;
        private Infragistics.Win.Misc.UltraGroupBox gbAlmacenes;
        private Infragistics.Win.Misc.UltraGroupBox groupBox4;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdAlmacen;
        private Infragistics.Win.Misc.UltraButton btnAgregarEstablecimientoAlmacen;
        private Infragistics.Win.Misc.UltraLabel label3;
        private Infragistics.Win.Misc.UltraButton btnEliminarAlmacen;
        private Infragistics.Win.Misc.UltraButton btnEditarAlmacen;
        private Infragistics.Win.Misc.UltraButton btnAsignarAlmacen;
        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmEstablecimiento_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmEstablecimiento_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmEstablecimiento_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmEstablecimiento_UltraFormManager_Dock_Area_Bottom;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
        private UltraCheckEditor uckImpresionPrevia;
        private UltraLabel ultraLabel1;
        private UltraCheckEditor uckDocumentoPredeterminado;
        private UltraTextEditor txtMaxNumeroItems;
        private UltraLabel ultraLabel2;
        private UltraSplitter ultraSplitter1;
        private System.Windows.Forms.Panel pnRight;
        private UltraGroupBox gbGrupo;
        private UltraTextEditor txtCentroCosto;
        private UltraLabel label4;
        private UltraButton btnGrabar;
        private UltraTextEditor txtGrupoDireccion;
        private UltraTextEditor txtGrupoNombre;
        private UltraLabel label7;
        private UltraLabel label6;
        private UltraComboEditor cboEstablecimientoAlmacen;
        private UltraComboEditor cboDetalleDocumento;
        private UltraComboEditor cboDetalleAlmacen;
        private UltraComboEditor txtNombreImpresora;
    }
}