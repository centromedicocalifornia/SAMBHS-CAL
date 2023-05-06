namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    partial class frmCondicionPago
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
            Infragistics.Win.Misc.UltraGroupBox groupBox2;
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_NombreCondicion", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Descending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("b_CreditoLetras");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("i_Dias");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_UsuarioCreacion");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("t_InsertaFecha");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_UsuarioModificacion");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("t_ActualizaFecha");
            Infragistics.Win.UltraWinGrid.SummarySettings summarySettings1 = new Infragistics.Win.UltraWinGrid.SummarySettings("", Infragistics.Win.UltraWinGrid.SummaryType.Count, null, "v_NombreCondicion", 0, true, "Band 0", 0, Infragistics.Win.UltraWinGrid.SummaryPosition.Right, null, -1, false);
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            System.Windows.Forms.Panel panelTop;
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.Misc.UltraPanel ultraPanel1;
            this.btnEliminar = new Infragistics.Win.Misc.UltraButton();
            this.btnAgregar = new Infragistics.Win.Misc.UltraButton();
            this.grdData = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.gbDatos = new Infragistics.Win.Misc.UltraGroupBox();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnGrabar = new Infragistics.Win.Misc.UltraButton();
            this.chkCredito = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.txtDias = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label4 = new Infragistics.Win.Misc.UltraLabel();
            this.txtNombre = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label3 = new Infragistics.Win.Misc.UltraLabel();
            this.gbSearch = new Infragistics.Win.Misc.UltraGroupBox();
            this.btnBuscar = new Infragistics.Win.Misc.UltraButton();
            this.txtBuscarNombre = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label1 = new Infragistics.Win.Misc.UltraLabel();
            this.uvCondicionPago = new Infragistics.Win.Misc.UltraValidator(this.components);
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this._frmCondicionPago_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmCondicionPago_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmCondicionPago_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmCondicionPago_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this.ultraFlowLayoutManager1 = new Infragistics.Win.Misc.UltraFlowLayoutManager(this.components);
            groupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            panelTop = new System.Windows.Forms.Panel();
            ultraPanel1 = new Infragistics.Win.Misc.UltraPanel();
            ((System.ComponentModel.ISupportInitialize)(groupBox2)).BeginInit();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).BeginInit();
            panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gbDatos)).BeginInit();
            this.gbDatos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkCredito)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDias)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNombre)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gbSearch)).BeginInit();
            this.gbSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuscarNombre)).BeginInit();
            ultraPanel1.ClientArea.SuspendLayout();
            ultraPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uvCondicionPago)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFlowLayoutManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(this.btnEliminar);
            groupBox2.Controls.Add(this.btnAgregar);
            groupBox2.Controls.Add(this.grdData);
            groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox2.Location = new System.Drawing.Point(0, 0);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(802, 343);
            groupBox2.TabIndex = 3;
            groupBox2.Text = "Filtro de Búsquedas";
            // 
            // btnEliminar
            // 
            this.btnEliminar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.Image = global::SAMBHS.Windows.WinClient.UI.Resource.delete;
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance1.ImageVAlign = Infragistics.Win.VAlign.Bottom;
            appearance1.TextHAlignAsString = "Right";
            appearance1.TextVAlignAsString = "Middle";
            this.btnEliminar.Appearance = appearance1;
            this.btnEliminar.Location = new System.Drawing.Point(635, 314);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(158, 23);
            this.btnEliminar.TabIndex = 5;
            this.btnEliminar.Text = "&Eliminar Condición Pago";
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // btnAgregar
            // 
            this.btnAgregar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance2.Image = global::SAMBHS.Windows.WinClient.UI.Resource.add;
            appearance2.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance2.ImageVAlign = Infragistics.Win.VAlign.Bottom;
            appearance2.TextHAlignAsString = "Right";
            appearance2.TextVAlignAsString = "Middle";
            this.btnAgregar.Appearance = appearance2;
            this.btnAgregar.Location = new System.Drawing.Point(483, 314);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(147, 23);
            this.btnAgregar.TabIndex = 3;
            this.btnAgregar.Text = "&Nueva Condición Pago";
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
            // 
            // grdData
            // 
            this.grdData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdData.CausesValidation = false;
            appearance3.BackColor = System.Drawing.SystemColors.Window;
            appearance3.BackColor2 = System.Drawing.Color.LightSteelBlue;
            appearance3.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            this.grdData.DisplayLayout.Appearance = appearance3;
            this.grdData.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn1.Header.Caption = "Nombre";
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Width = 200;
            ultraGridColumn4.Header.Caption = "Crédito / Letras";
            ultraGridColumn4.Header.VisiblePosition = 1;
            ultraGridColumn4.Width = 101;
            ultraGridColumn3.Header.Caption = "Días";
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn3.Width = 94;
            ultraGridColumn6.Header.Caption = "Usuario Crea.";
            ultraGridColumn6.Header.VisiblePosition = 3;
            ultraGridColumn6.Width = 94;
            ultraGridColumn5.Header.Caption = "Fecha Crea.";
            ultraGridColumn5.Header.VisiblePosition = 4;
            ultraGridColumn5.Width = 94;
            ultraGridColumn7.Header.Caption = "Usuario Act.";
            ultraGridColumn7.Header.VisiblePosition = 5;
            ultraGridColumn7.Width = 94;
            ultraGridColumn14.Header.Caption = "Fecha Act.";
            ultraGridColumn14.Header.VisiblePosition = 6;
            ultraGridColumn14.Width = 94;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn4,
            ultraGridColumn3,
            ultraGridColumn6,
            ultraGridColumn5,
            ultraGridColumn7,
            ultraGridColumn14});
            ultraGridBand1.Override.SummaryFooterCaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance4.Image = global::SAMBHS.Windows.WinClient.UI.Resource.book_magnify;
            summarySettings1.Appearance = appearance4;
            summarySettings1.DisplayFormat = "{0} Registros";
            summarySettings1.GroupBySummaryValueAppearance = appearance5;
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
            appearance6.BackColor = System.Drawing.Color.Transparent;
            this.grdData.DisplayLayout.Override.CardAreaAppearance = appearance6;
            appearance7.BackColor = System.Drawing.SystemColors.Control;
            appearance7.BackColor2 = System.Drawing.SystemColors.ControlLightLight;
            appearance7.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            this.grdData.DisplayLayout.Override.CellAppearance = appearance7;
            this.grdData.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            appearance8.BackColor = System.Drawing.SystemColors.Control;
            appearance8.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance8.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            appearance8.ThemedElementAlpha = Infragistics.Win.Alpha.Transparent;
            this.grdData.DisplayLayout.Override.HeaderAppearance = appearance8;
            this.grdData.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance9.AlphaLevel = ((short)(187));
            this.grdData.DisplayLayout.Override.RowAlternateAppearance = appearance9;
            appearance10.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.grdData.DisplayLayout.Override.RowSelectorAppearance = appearance10;
            this.grdData.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            appearance11.BackColor = System.Drawing.SystemColors.Highlight;
            appearance11.BackColor2 = System.Drawing.SystemColors.ActiveCaption;
            appearance11.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            appearance11.FontData.BoldAsString = "True";
            this.grdData.DisplayLayout.Override.SelectedRowAppearance = appearance11;
            this.grdData.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            this.grdData.DisplayLayout.RowConnectorColor = System.Drawing.SystemColors.ControlDarkDark;
            this.grdData.DisplayLayout.RowConnectorStyle = Infragistics.Win.UltraWinGrid.RowConnectorStyle.Dashed;
            this.grdData.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdData.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdData.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.grdData.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdData.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.grdData.Location = new System.Drawing.Point(5, 18);
            this.grdData.Margin = new System.Windows.Forms.Padding(2);
            this.grdData.Name = "grdData";
            this.grdData.Size = new System.Drawing.Size(792, 291);
            this.grdData.TabIndex = 45;
            this.grdData.AfterRowActivate += new System.EventHandler(this.grdData_AfterRowActivate);
            this.grdData.DoubleClickRow += new Infragistics.Win.UltraWinGrid.DoubleClickRowEventHandler(this.grdData_DoubleClickRow);
            this.grdData.MouseDown += new System.Windows.Forms.MouseEventHandler(this.grdData_MouseDown_1);
            // 
            // panelTop
            // 
            panelTop.AutoSize = true;
            panelTop.Controls.Add(this.gbDatos);
            panelTop.Controls.Add(this.gbSearch);
            panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            panelTop.Location = new System.Drawing.Point(8, 31);
            panelTop.Name = "panelTop";
            panelTop.Size = new System.Drawing.Size(802, 162);
            panelTop.TabIndex = 5;
            // 
            // gbDatos
            // 
            this.gbDatos.Controls.Add(this.btnCancel);
            this.gbDatos.Controls.Add(this.btnGrabar);
            this.gbDatos.Controls.Add(this.chkCredito);
            this.gbDatos.Controls.Add(this.txtDias);
            this.gbDatos.Controls.Add(this.label4);
            this.gbDatos.Controls.Add(this.txtNombre);
            this.gbDatos.Controls.Add(this.label3);
            this.gbDatos.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbDatos.Location = new System.Drawing.Point(0, 0);
            this.gbDatos.Name = "gbDatos";
            this.gbDatos.Size = new System.Drawing.Size(802, 97);
            this.gbDatos.TabIndex = 1;
            this.gbDatos.Text = "Datos Condición de Pago";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance12.Image = global::SAMBHS.Windows.WinClient.UI.Resource.cross;
            this.btnCancel.Appearance = appearance12;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(609, 67);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(88, 23);
            this.btnCancel.TabIndex = 158;
            this.btnCancel.Text = "Cancelar";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnGrabar
            // 
            this.btnGrabar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance13.Image = global::SAMBHS.Windows.WinClient.UI.Resource.system_save;
            appearance13.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance13.ImageVAlign = Infragistics.Win.VAlign.Bottom;
            appearance13.TextHAlignAsString = "Center";
            appearance13.TextVAlignAsString = "Middle";
            this.btnGrabar.Appearance = appearance13;
            this.btnGrabar.Location = new System.Drawing.Point(703, 67);
            this.btnGrabar.Name = "btnGrabar";
            this.btnGrabar.Size = new System.Drawing.Size(88, 23);
            this.btnGrabar.TabIndex = 7;
            this.btnGrabar.Text = "&Guadar";
            this.btnGrabar.Click += new System.EventHandler(this.btnGrabar_Click);
            // 
            // chkCredito
            // 
            this.chkCredito.AutoSize = true;
            this.chkCredito.Location = new System.Drawing.Point(70, 49);
            this.chkCredito.Name = "chkCredito";
            this.chkCredito.Size = new System.Drawing.Size(92, 17);
            this.chkCredito.TabIndex = 4;
            this.chkCredito.Text = "Crédito/Letras";
            // 
            // txtDias
            // 
            this.txtDias.Location = new System.Drawing.Point(277, 48);
            this.txtDias.Name = "txtDias";
            this.txtDias.Size = new System.Drawing.Size(64, 21);
            this.txtDias.TabIndex = 3;
            this.txtDias.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDias_KeyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(238, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 14);
            this.label4.TabIndex = 2;
            this.label4.Text = "Días:";
            // 
            // txtNombre
            // 
            this.txtNombre.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtNombre.Location = new System.Drawing.Point(70, 19);
            this.txtNombre.MaxLength = 120;
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(271, 21);
            this.txtNombre.TabIndex = 1;
            this.uvCondicionPago.GetValidationSettings(this.txtNombre).DataType = typeof(string);
            this.uvCondicionPago.GetValidationSettings(this.txtNombre).EmptyValueCriteria = Infragistics.Win.Misc.EmptyValueCriteria.NullOrEmptyString;
            this.uvCondicionPago.GetValidationSettings(this.txtNombre).IsRequired = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 14);
            this.label3.TabIndex = 0;
            this.label3.Text = "Nombre:";
            // 
            // gbSearch
            // 
            this.gbSearch.Controls.Add(this.btnBuscar);
            this.gbSearch.Controls.Add(this.txtBuscarNombre);
            this.gbSearch.Controls.Add(this.label1);
            this.gbSearch.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gbSearch.Location = new System.Drawing.Point(0, 97);
            this.gbSearch.Name = "gbSearch";
            this.gbSearch.Size = new System.Drawing.Size(802, 65);
            this.gbSearch.TabIndex = 0;
            this.gbSearch.Text = "Filtro de Búsquedas";
            // 
            // btnBuscar
            // 
            this.btnBuscar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance14.Image = global::SAMBHS.Windows.WinClient.UI.Resource.system_search;
            appearance14.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance14.ImageVAlign = Infragistics.Win.VAlign.Bottom;
            appearance14.TextHAlignAsString = "Center";
            appearance14.TextVAlignAsString = "Middle";
            this.btnBuscar.Appearance = appearance14;
            this.btnBuscar.Location = new System.Drawing.Point(716, 23);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(75, 24);
            this.btnBuscar.TabIndex = 2;
            this.btnBuscar.Text = "&Buscar";
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // txtBuscarNombre
            // 
            this.txtBuscarNombre.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBuscarNombre.Location = new System.Drawing.Point(76, 26);
            this.txtBuscarNombre.Name = "txtBuscarNombre";
            this.txtBuscarNombre.Size = new System.Drawing.Size(618, 21);
            this.txtBuscarNombre.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "Nombre:";
            // 
            // ultraPanel1
            // 
            // 
            // ultraPanel1.ClientArea
            // 
            ultraPanel1.ClientArea.Controls.Add(groupBox2);
            ultraPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            ultraPanel1.Location = new System.Drawing.Point(8, 193);
            ultraPanel1.Name = "ultraPanel1";
            ultraPanel1.Size = new System.Drawing.Size(802, 343);
            ultraPanel1.TabIndex = 6;
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // _frmCondicionPago_UltraFormManager_Dock_Area_Left
            // 
            this._frmCondicionPago_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmCondicionPago_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmCondicionPago_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._frmCondicionPago_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmCondicionPago_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._frmCondicionPago_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmCondicionPago_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 31);
            this._frmCondicionPago_UltraFormManager_Dock_Area_Left.Name = "_frmCondicionPago_UltraFormManager_Dock_Area_Left";
            this._frmCondicionPago_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 505);
            // 
            // _frmCondicionPago_UltraFormManager_Dock_Area_Right
            // 
            this._frmCondicionPago_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmCondicionPago_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmCondicionPago_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._frmCondicionPago_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmCondicionPago_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._frmCondicionPago_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmCondicionPago_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(810, 31);
            this._frmCondicionPago_UltraFormManager_Dock_Area_Right.Name = "_frmCondicionPago_UltraFormManager_Dock_Area_Right";
            this._frmCondicionPago_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 505);
            // 
            // _frmCondicionPago_UltraFormManager_Dock_Area_Top
            // 
            this._frmCondicionPago_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmCondicionPago_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmCondicionPago_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._frmCondicionPago_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmCondicionPago_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._frmCondicionPago_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmCondicionPago_UltraFormManager_Dock_Area_Top.Name = "_frmCondicionPago_UltraFormManager_Dock_Area_Top";
            this._frmCondicionPago_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(818, 31);
            // 
            // _frmCondicionPago_UltraFormManager_Dock_Area_Bottom
            // 
            this._frmCondicionPago_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmCondicionPago_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmCondicionPago_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._frmCondicionPago_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmCondicionPago_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._frmCondicionPago_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmCondicionPago_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 536);
            this._frmCondicionPago_UltraFormManager_Dock_Area_Bottom.Name = "_frmCondicionPago_UltraFormManager_Dock_Area_Bottom";
            this._frmCondicionPago_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(818, 8);
            // 
            // ultraFlowLayoutManager1
            // 
            this.ultraFlowLayoutManager1.ContainerControl = panelTop;
            // 
            // frmCondicionPago
            // 
            this.AcceptButton = this.btnBuscar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(818, 544);
            this.Controls.Add(ultraPanel1);
            this.Controls.Add(panelTop);
            this.Controls.Add(this._frmCondicionPago_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._frmCondicionPago_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._frmCondicionPago_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._frmCondicionPago_UltraFormManager_Dock_Area_Bottom);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCondicionPago";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Condición Pago";
            this.Load += new System.EventHandler(this.frmCondicionPago_Load);
            ((System.ComponentModel.ISupportInitialize)(groupBox2)).EndInit();
            groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).EndInit();
            panelTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gbDatos)).EndInit();
            this.gbDatos.ResumeLayout(false);
            this.gbDatos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkCredito)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDias)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNombre)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gbSearch)).EndInit();
            this.gbSearch.ResumeLayout(false);
            this.gbSearch.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuscarNombre)).EndInit();
            ultraPanel1.ClientArea.ResumeLayout(false);
            ultraPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.uvCondicionPago)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFlowLayoutManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraButton btnBuscar;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtBuscarNombre;
        private Infragistics.Win.Misc.UltraLabel label1;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdData;
        private Infragistics.Win.Misc.UltraButton btnEliminar;
        private Infragistics.Win.Misc.UltraButton btnAgregar;
        private Infragistics.Win.Misc.UltraValidator uvCondicionPago;
        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmCondicionPago_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmCondicionPago_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmCondicionPago_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmCondicionPago_UltraFormManager_Dock_Area_Bottom;
        private Infragistics.Win.Misc.UltraGroupBox gbSearch;
        private Infragistics.Win.Misc.UltraFlowLayoutManager ultraFlowLayoutManager1;
        private Infragistics.Win.Misc.UltraGroupBox gbDatos;
        private Infragistics.Win.Misc.UltraButton btnGrabar;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkCredito;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtDias;
        private Infragistics.Win.Misc.UltraLabel label4;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtNombre;
        private Infragistics.Win.Misc.UltraLabel label3;
        private Infragistics.Win.Misc.UltraButton btnCancel;
    }
}