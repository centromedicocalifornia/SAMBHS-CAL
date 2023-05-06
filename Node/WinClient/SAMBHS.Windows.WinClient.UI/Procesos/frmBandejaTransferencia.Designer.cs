namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    partial class frmBandejaTransferencia
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
            Infragistics.Win.Misc.UltraLabel label25;
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_MesCorrelativo");
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_AlmacenOrigen");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("t_Fecha");
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_AlmacenDestino");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_DescripcionMotivo");
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("t_InsertaFecha");
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("t_ActualizaFecha");
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_UsuarioCreacion", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_UsuarioModificacion", 1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Moneda", 2);
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            this.groupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            this.btnRegenerarIDs = new Infragistics.Win.Misc.UltraButton();
            this.btnActualizarGuiasRemision = new Infragistics.Win.Misc.UltraButton();
            this.btnRegenerarT = new Infragistics.Win.Misc.UltraButton();
            this.lblDocumentoExportado = new Infragistics.Win.Misc.UltraLabel();
            this.btnExportarBandeja = new Infragistics.Win.Misc.UltraButton();
            this.btnEditar = new Infragistics.Win.Misc.UltraButton();
            this.chkFiltroPersonalizado = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkBandejaAgrupable = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.grdData = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.btnEliminar = new Infragistics.Win.Misc.UltraButton();
            this.btnAgregar = new Infragistics.Win.Misc.UltraButton();
            this.lblContadorFilas = new Infragistics.Win.Misc.UltraLabel();
            this.groupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.cboMoneda = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.cboMotivo = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.cboAlmacenDestino = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.cboAlmacenOrigen = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.label5 = new Infragistics.Win.Misc.UltraLabel();
            this.label2 = new Infragistics.Win.Misc.UltraLabel();
            this.btnBuscar = new Infragistics.Win.Misc.UltraButton();
            this.dtpFechaFin = new System.Windows.Forms.DateTimePicker();
            this.dtpFechaInicio = new System.Windows.Forms.DateTimePicker();
            this.label3 = new Infragistics.Win.Misc.UltraLabel();
            this.label13 = new Infragistics.Win.Misc.UltraLabel();
            this.label1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this.frmBandejaTransferencia_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this.bwkProcesoBL = new System.ComponentModel.BackgroundWorker();
            label25 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox2)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkFiltroPersonalizado)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkBandejaAgrupable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboMoneda)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboMotivo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAlmacenDestino)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAlmacenOrigen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            this.frmBandejaTransferencia_Fill_Panel.ClientArea.SuspendLayout();
            this.frmBandejaTransferencia_Fill_Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label25
            // 
            label25.AutoSize = true;
            label25.Location = new System.Drawing.Point(410, 45);
            label25.Name = "label25";
            label25.Size = new System.Drawing.Size(48, 14);
            label25.TabIndex = 5009;
            label25.Text = "Moneda:";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.btnRegenerarIDs);
            this.groupBox2.Controls.Add(this.btnActualizarGuiasRemision);
            this.groupBox2.Controls.Add(this.btnRegenerarT);
            this.groupBox2.Controls.Add(this.lblDocumentoExportado);
            this.groupBox2.Controls.Add(this.btnExportarBandeja);
            this.groupBox2.Controls.Add(this.btnEditar);
            this.groupBox2.Controls.Add(this.chkFiltroPersonalizado);
            this.groupBox2.Controls.Add(this.chkBandejaAgrupable);
            this.groupBox2.Controls.Add(this.grdData);
            this.groupBox2.Controls.Add(this.btnEliminar);
            this.groupBox2.Controls.Add(this.btnAgregar);
            this.groupBox2.Controls.Add(this.lblContadorFilas);
            this.groupBox2.Location = new System.Drawing.Point(12, 91);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1021, 349);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.Text = "Resultado de Búsqueda";
            // 
            // btnRegenerarIDs
            // 
            this.btnRegenerarIDs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.Image = global::SAMBHS.Windows.WinClient.UI.Properties.Resources.system_savenew;
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance1.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance1.TextHAlignAsString = "Right";
            appearance1.TextVAlignAsString = "Middle";
            this.btnRegenerarIDs.Appearance = appearance1;
            this.btnRegenerarIDs.Location = new System.Drawing.Point(261, 320);
            this.btnRegenerarIDs.Name = "btnRegenerarIDs";
            this.btnRegenerarIDs.Size = new System.Drawing.Size(45, 25);
            this.btnRegenerarIDs.TabIndex = 170;
            this.btnRegenerarIDs.Text = "&Ids";
            this.btnRegenerarIDs.Click += new System.EventHandler(this.btnRegenerarIDs_Click);
            // 
            // btnActualizarGuiasRemision
            // 
            this.btnActualizarGuiasRemision.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance2.Image = global::SAMBHS.Windows.WinClient.UI.Properties.Resources.system_savenew;
            appearance2.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance2.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance2.TextHAlignAsString = "Right";
            appearance2.TextVAlignAsString = "Middle";
            this.btnActualizarGuiasRemision.Appearance = appearance2;
            this.btnActualizarGuiasRemision.Location = new System.Drawing.Point(309, 321);
            this.btnActualizarGuiasRemision.Name = "btnActualizarGuiasRemision";
            this.btnActualizarGuiasRemision.Size = new System.Drawing.Size(105, 25);
            this.btnActualizarGuiasRemision.TabIndex = 169;
            this.btnActualizarGuiasRemision.Text = "&Actualizar GM";
            this.btnActualizarGuiasRemision.Click += new System.EventHandler(this.btnActualizarGuiasRemision_Click);
            // 
            // btnRegenerarT
            // 
            this.btnRegenerarT.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance3.Image = global::SAMBHS.Windows.WinClient.UI.Properties.Resources.system_savenew;
            appearance3.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance3.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance3.TextHAlignAsString = "Right";
            appearance3.TextVAlignAsString = "Middle";
            this.btnRegenerarT.Appearance = appearance3;
            this.btnRegenerarT.Location = new System.Drawing.Point(420, 321);
            this.btnRegenerarT.Name = "btnRegenerarT";
            this.btnRegenerarT.Size = new System.Drawing.Size(160, 25);
            this.btnRegenerarT.TabIndex = 168;
            this.btnRegenerarT.Text = "&Regenerar Transferencias";
            this.btnRegenerarT.Click += new System.EventHandler(this.btnRegenerarT_Click);
            // 
            // lblDocumentoExportado
            // 
            this.lblDocumentoExportado.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblDocumentoExportado.Location = new System.Drawing.Point(222, 321);
            this.lblDocumentoExportado.Name = "lblDocumentoExportado";
            this.lblDocumentoExportado.Size = new System.Drawing.Size(50, 21);
            this.lblDocumentoExportado.TabIndex = 167;
            // 
            // btnExportarBandeja
            // 
            this.btnExportarBandeja.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            appearance4.Image = global::SAMBHS.Windows.WinClient.UI.Resource.page_excel1;
            appearance4.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance4.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance4.TextHAlignAsString = "Right";
            appearance4.TextVAlignAsString = "Middle";
            this.btnExportarBandeja.Appearance = appearance4;
            this.btnExportarBandeja.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExportarBandeja.Location = new System.Drawing.Point(7, 318);
            this.btnExportarBandeja.Margin = new System.Windows.Forms.Padding(2);
            this.btnExportarBandeja.Name = "btnExportarBandeja";
            this.btnExportarBandeja.Size = new System.Drawing.Size(22, 24);
            this.btnExportarBandeja.TabIndex = 166;
            this.btnExportarBandeja.Click += new System.EventHandler(this.btnExportarBandeja_Click);
            // 
            // btnEditar
            // 
            this.btnEditar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance5.Image = global::SAMBHS.Windows.WinClient.UI.Resource.pencil;
            appearance5.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance5.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance5.TextHAlignAsString = "Right";
            appearance5.TextVAlignAsString = "Middle";
            this.btnEditar.Appearance = appearance5;
            this.btnEditar.Enabled = false;
            this.btnEditar.Location = new System.Drawing.Point(731, 320);
            this.btnEditar.Name = "btnEditar";
            this.btnEditar.Size = new System.Drawing.Size(135, 25);
            this.btnEditar.TabIndex = 2;
            this.btnEditar.Text = "E&ditar Transferencia";
            this.btnEditar.Click += new System.EventHandler(this.btnEditar_Click);
            // 
            // chkFiltroPersonalizado
            // 
            this.chkFiltroPersonalizado.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkFiltroPersonalizado.Location = new System.Drawing.Point(37, 320);
            this.chkFiltroPersonalizado.Name = "chkFiltroPersonalizado";
            this.chkFiltroPersonalizado.Size = new System.Drawing.Size(108, 20);
            this.chkFiltroPersonalizado.TabIndex = 165;
            this.chkFiltroPersonalizado.Text = "Filtro Avanzado";
            this.chkFiltroPersonalizado.CheckedChanged += new System.EventHandler(this.chkFiltroPersonalizado_CheckedChanged);
            // 
            // chkBandejaAgrupable
            // 
            this.chkBandejaAgrupable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkBandejaAgrupable.Location = new System.Drawing.Point(146, 320);
            this.chkBandejaAgrupable.Name = "chkBandejaAgrupable";
            this.chkBandejaAgrupable.Size = new System.Drawing.Size(77, 20);
            this.chkBandejaAgrupable.TabIndex = 164;
            this.chkBandejaAgrupable.Text = "Agrupable";
            this.chkBandejaAgrupable.CheckedChanged += new System.EventHandler(this.chkBandejaAgrupable_CheckedChanged);
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
            appearance7.TextHAlignAsString = "Center";
            ultraGridColumn1.CellAppearance = appearance7;
            ultraGridColumn1.Header.Caption = "Nro. Registro";
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Width = 70;
            ultraGridColumn2.Header.Caption = "Almacén Origen";
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Width = 163;
            appearance8.TextHAlignAsString = "Center";
            ultraGridColumn3.CellAppearance = appearance8;
            ultraGridColumn3.Header.Caption = "Fecha";
            ultraGridColumn3.Header.VisiblePosition = 4;
            ultraGridColumn3.Width = 76;
            ultraGridColumn4.Header.Caption = "Almacén Destino";
            ultraGridColumn4.Header.VisiblePosition = 2;
            ultraGridColumn4.Width = 161;
            appearance9.TextHAlignAsString = "Left";
            ultraGridColumn5.CellAppearance = appearance9;
            ultraGridColumn5.Header.Caption = "Motivo";
            ultraGridColumn5.Header.VisiblePosition = 5;
            ultraGridColumn5.Width = 140;
            appearance10.TextHAlignAsString = "Left";
            ultraGridColumn6.CellAppearance = appearance10;
            ultraGridColumn6.Format = "dd/MM/yyyy hh:mm tt";
            ultraGridColumn6.Header.Caption = "Fecha Crea.";
            ultraGridColumn6.Header.VisiblePosition = 7;
            ultraGridColumn6.Width = 98;
            appearance11.TextHAlignAsString = "Left";
            ultraGridColumn7.CellAppearance = appearance11;
            ultraGridColumn7.Format = "dd/MM/yyyy hh:mm tt";
            ultraGridColumn7.Header.Caption = "Fecha Act.";
            ultraGridColumn7.Header.VisiblePosition = 9;
            ultraGridColumn7.Width = 90;
            ultraGridColumn8.Header.Caption = "Usuario Crea.";
            ultraGridColumn8.Header.VisiblePosition = 6;
            ultraGridColumn8.Width = 72;
            ultraGridColumn9.Header.Caption = "Usuario Act.";
            ultraGridColumn9.Header.VisiblePosition = 8;
            ultraGridColumn9.Width = 67;
            ultraGridColumn10.Header.VisiblePosition = 3;
            ultraGridColumn10.Width = 52;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn8,
            ultraGridColumn9,
            ultraGridColumn10});
            this.grdData.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdData.DisplayLayout.GroupByBox.Hidden = true;
            this.grdData.DisplayLayout.InterBandSpacing = 10;
            this.grdData.DisplayLayout.MaxColScrollRegions = 1;
            this.grdData.DisplayLayout.MaxRowScrollRegions = 1;
            this.grdData.DisplayLayout.NewColumnLoadStyle = Infragistics.Win.UltraWinGrid.NewColumnLoadStyle.Hide;
            appearance12.BackColor = System.Drawing.SystemColors.Highlight;
            appearance12.BackColor2 = System.Drawing.SystemColors.ActiveCaption;
            appearance12.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            appearance12.FontData.BoldAsString = "True";
            appearance12.ForeColor = System.Drawing.Color.White;
            this.grdData.DisplayLayout.Override.ActiveRowAppearance = appearance12;
            this.grdData.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdData.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdData.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.grdData.DisplayLayout.Override.AllowRowSummaries = Infragistics.Win.UltraWinGrid.AllowRowSummaries.False;
            this.grdData.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            appearance13.BackColor = System.Drawing.Color.Transparent;
            this.grdData.DisplayLayout.Override.CardAreaAppearance = appearance13;
            appearance14.BackColor = System.Drawing.SystemColors.Control;
            appearance14.BackColor2 = System.Drawing.SystemColors.ControlLightLight;
            appearance14.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            this.grdData.DisplayLayout.Override.CellAppearance = appearance14;
            this.grdData.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            appearance15.BackColor = System.Drawing.SystemColors.Control;
            appearance15.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance15.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            appearance15.ThemedElementAlpha = Infragistics.Win.Alpha.Transparent;
            this.grdData.DisplayLayout.Override.HeaderAppearance = appearance15;
            this.grdData.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance16.AlphaLevel = ((short)(187));
            this.grdData.DisplayLayout.Override.RowAlternateAppearance = appearance16;
            appearance17.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.grdData.DisplayLayout.Override.RowSelectorAppearance = appearance17;
            this.grdData.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            this.grdData.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.grdData.DisplayLayout.RowConnectorColor = System.Drawing.SystemColors.ControlDarkDark;
            this.grdData.DisplayLayout.RowConnectorStyle = Infragistics.Win.UltraWinGrid.RowConnectorStyle.Dashed;
            this.grdData.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdData.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdData.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.grdData.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.grdData.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdData.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.grdData.Location = new System.Drawing.Point(5, 36);
            this.grdData.Margin = new System.Windows.Forms.Padding(2);
            this.grdData.Name = "grdData";
            this.grdData.Size = new System.Drawing.Size(1010, 280);
            this.grdData.TabIndex = 0;
            this.grdData.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdData_InitializeLayout);
            this.grdData.AfterRowActivate += new System.EventHandler(this.grdData_AfterRowActivate);
            this.grdData.DoubleClickRow += new Infragistics.Win.UltraWinGrid.DoubleClickRowEventHandler(this.grdData_DoubleClickRow);
            this.grdData.MouseDown += new System.Windows.Forms.MouseEventHandler(this.grdData_MouseDown);
            // 
            // btnEliminar
            // 
            this.btnEliminar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance18.Image = global::SAMBHS.Windows.WinClient.UI.Resource.delete;
            appearance18.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance18.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance18.TextHAlignAsString = "Right";
            appearance18.TextVAlignAsString = "Middle";
            this.btnEliminar.Appearance = appearance18;
            this.btnEliminar.Enabled = false;
            this.btnEliminar.Location = new System.Drawing.Point(872, 320);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(144, 25);
            this.btnEliminar.TabIndex = 3;
            this.btnEliminar.Text = "&Eliminar Transferencia";
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // btnAgregar
            // 
            this.btnAgregar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance19.Image = global::SAMBHS.Windows.WinClient.UI.Resource.add;
            appearance19.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance19.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance19.TextHAlignAsString = "Right";
            appearance19.TextVAlignAsString = "Middle";
            this.btnAgregar.Appearance = appearance19;
            this.btnAgregar.Location = new System.Drawing.Point(586, 320);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(139, 25);
            this.btnAgregar.TabIndex = 1;
            this.btnAgregar.Text = "&Nueva Transferencia";
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
            // 
            // lblContadorFilas
            // 
            this.lblContadorFilas.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance20.TextHAlignAsString = "Right";
            appearance20.TextVAlignAsString = "Middle";
            this.lblContadorFilas.Appearance = appearance20;
            this.lblContadorFilas.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.2F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContadorFilas.Location = new System.Drawing.Point(803, 16);
            this.lblContadorFilas.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblContadorFilas.Name = "lblContadorFilas";
            this.lblContadorFilas.Size = new System.Drawing.Size(213, 19);
            this.lblContadorFilas.TabIndex = 143;
            this.lblContadorFilas.Text = "No se ha realizado la búsqueda aún.";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.cboMoneda);
            this.groupBox1.Controls.Add(label25);
            this.groupBox1.Controls.Add(this.cboMotivo);
            this.groupBox1.Controls.Add(this.cboAlmacenDestino);
            this.groupBox1.Controls.Add(this.cboAlmacenOrigen);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btnBuscar);
            this.groupBox1.Controls.Add(this.dtpFechaFin);
            this.groupBox1.Controls.Add(this.dtpFechaInicio);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1021, 75);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.Text = "Filtro de Búsqueda:";
            // 
            // cboMoneda
            // 
            this.cboMoneda.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboMoneda.Location = new System.Drawing.Point(464, 43);
            this.cboMoneda.Name = "cboMoneda";
            this.cboMoneda.Size = new System.Drawing.Size(132, 21);
            this.cboMoneda.TabIndex = 5;
            // 
            // cboMotivo
            // 
            this.cboMotivo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboMotivo.Location = new System.Drawing.Point(789, 18);
            this.cboMotivo.Name = "cboMotivo";
            this.cboMotivo.Size = new System.Drawing.Size(220, 21);
            this.cboMotivo.TabIndex = 2;
            // 
            // cboAlmacenDestino
            // 
            this.cboAlmacenDestino.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboAlmacenDestino.Location = new System.Drawing.Point(464, 18);
            this.cboAlmacenDestino.Name = "cboAlmacenDestino";
            this.cboAlmacenDestino.Size = new System.Drawing.Size(233, 21);
            this.cboAlmacenDestino.TabIndex = 1;
            // 
            // cboAlmacenOrigen
            // 
            this.cboAlmacenOrigen.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboAlmacenOrigen.Location = new System.Drawing.Point(100, 18);
            this.cboAlmacenOrigen.Name = "cboAlmacenOrigen";
            this.cboAlmacenOrigen.Size = new System.Drawing.Size(245, 21);
            this.cboAlmacenOrigen.TabIndex = 0;
            this.cboAlmacenOrigen.ValueChanged += new System.EventHandler(this.cboAlmacenOrigen_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(209, 49);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(20, 14);
            this.label5.TabIndex = 21;
            this.label5.Text = "Al :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(371, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 14);
            this.label2.TabIndex = 19;
            this.label2.Text = "Almacén Destino:";
            // 
            // btnBuscar
            // 
            this.btnBuscar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance21.Image = global::SAMBHS.Windows.WinClient.UI.Resource.system_search;
            appearance21.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance21.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance21.TextHAlignAsString = "Center";
            appearance21.TextVAlignAsString = "Middle";
            this.btnBuscar.Appearance = appearance21;
            this.btnBuscar.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBuscar.Location = new System.Drawing.Point(937, 44);
            this.btnBuscar.Margin = new System.Windows.Forms.Padding(2);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(72, 24);
            this.btnBuscar.TabIndex = 6;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // dtpFechaFin
            // 
            this.dtpFechaFin.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFechaFin.Location = new System.Drawing.Point(243, 44);
            this.dtpFechaFin.Margin = new System.Windows.Forms.Padding(2);
            this.dtpFechaFin.Name = "dtpFechaFin";
            this.dtpFechaFin.Size = new System.Drawing.Size(102, 20);
            this.dtpFechaFin.TabIndex = 4;
            this.dtpFechaFin.ValueChanged += new System.EventHandler(this.dtpFechaFin_ValueChanged);
            // 
            // dtpFechaInicio
            // 
            this.dtpFechaInicio.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFechaInicio.Location = new System.Drawing.Point(100, 44);
            this.dtpFechaInicio.Margin = new System.Windows.Forms.Padding(2);
            this.dtpFechaInicio.Name = "dtpFechaInicio";
            this.dtpFechaInicio.Size = new System.Drawing.Size(96, 20);
            this.dtpFechaInicio.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(37, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 14);
            this.label3.TabIndex = 2;
            this.label3.Text = "Fecha del :";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(742, 22);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(41, 14);
            this.label13.TabIndex = 14;
            this.label13.Text = "Motivo:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "Almacén Origen:";
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // frmBandejaTransferencia_Fill_Panel
            // 
            // 
            // frmBandejaTransferencia_Fill_Panel.ClientArea
            // 
            this.frmBandejaTransferencia_Fill_Panel.ClientArea.Controls.Add(this.groupBox2);
            this.frmBandejaTransferencia_Fill_Panel.ClientArea.Controls.Add(this.groupBox1);
            this.frmBandejaTransferencia_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.frmBandejaTransferencia_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmBandejaTransferencia_Fill_Panel.Location = new System.Drawing.Point(8, 32);
            this.frmBandejaTransferencia_Fill_Panel.Name = "frmBandejaTransferencia_Fill_Panel";
            this.frmBandejaTransferencia_Fill_Panel.Size = new System.Drawing.Size(1045, 450);
            this.frmBandejaTransferencia_Fill_Panel.TabIndex = 0;
            // 
            // _frmBandejaTransferencia_UltraFormManager_Dock_Area_Left
            // 
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 32);
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Left.Name = "_frmBandejaTransferencia_UltraFormManager_Dock_Area_Left";
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 450);
            // 
            // _frmBandejaTransferencia_UltraFormManager_Dock_Area_Right
            // 
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(1053, 32);
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Right.Name = "_frmBandejaTransferencia_UltraFormManager_Dock_Area_Right";
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 450);
            // 
            // _frmBandejaTransferencia_UltraFormManager_Dock_Area_Top
            // 
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Top.Name = "_frmBandejaTransferencia_UltraFormManager_Dock_Area_Top";
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(1061, 32);
            // 
            // _frmBandejaTransferencia_UltraFormManager_Dock_Area_Bottom
            // 
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 482);
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Bottom.Name = "_frmBandejaTransferencia_UltraFormManager_Dock_Area_Bottom";
            this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(1061, 8);
            // 
            // bwkProcesoBL
            // 
            this.bwkProcesoBL.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwkProcesoBL_DoWork);
            // 
            // frmBandejaTransferencia
            // 
            this.AcceptButton = this.btnBuscar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(1061, 490);
            this.Controls.Add(this.frmBandejaTransferencia_Fill_Panel);
            this.Controls.Add(this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._frmBandejaTransferencia_UltraFormManager_Dock_Area_Bottom);
            this.Name = "frmBandejaTransferencia";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bandeja Transferencia entre Almacenes";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmBandejaTransferencia_FormClosing);
            this.Load += new System.EventHandler(this.frmBandejaTransferencia_Load);
            ((System.ComponentModel.ISupportInitialize)(this.groupBox2)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkFiltroPersonalizado)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkBandejaAgrupable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboMoneda)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboMotivo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAlmacenDestino)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAlmacenOrigen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            this.frmBandejaTransferencia_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmBandejaTransferencia_Fill_Panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox groupBox2;
        private Infragistics.Win.Misc.UltraButton btnEditar;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdData;
        private Infragistics.Win.Misc.UltraButton btnEliminar;
        private Infragistics.Win.Misc.UltraButton btnAgregar;
        private Infragistics.Win.Misc.UltraLabel lblContadorFilas;
        private Infragistics.Win.Misc.UltraGroupBox groupBox1;
        private Infragistics.Win.Misc.UltraButton btnBuscar;
        private System.Windows.Forms.DateTimePicker dtpFechaFin;
        private System.Windows.Forms.DateTimePicker dtpFechaInicio;
        private Infragistics.Win.Misc.UltraLabel label3;
        private Infragistics.Win.Misc.UltraLabel label13;
        private Infragistics.Win.Misc.UltraLabel label1;
        private Infragistics.Win.Misc.UltraLabel label2;
        private Infragistics.Win.Misc.UltraLabel label5;
        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.Misc.UltraPanel frmBandejaTransferencia_Fill_Panel;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmBandejaTransferencia_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmBandejaTransferencia_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmBandejaTransferencia_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmBandejaTransferencia_UltraFormManager_Dock_Area_Bottom;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboAlmacenOrigen;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboMotivo;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboAlmacenDestino;
        private Infragistics.Win.Misc.UltraButton btnExportarBandeja;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkFiltroPersonalizado;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkBandejaAgrupable;
        private Infragistics.Win.Misc.UltraLabel lblDocumentoExportado;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboMoneda;
        private Infragistics.Win.Misc.UltraButton btnRegenerarT;
        private System.ComponentModel.BackgroundWorker bwkProcesoBL;
        private Infragistics.Win.Misc.UltraButton btnActualizarGuiasRemision;
        private Infragistics.Win.Misc.UltraButton btnRegenerarIDs;
    }
}