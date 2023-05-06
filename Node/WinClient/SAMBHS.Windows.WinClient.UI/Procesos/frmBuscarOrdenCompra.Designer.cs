namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    partial class frmBuscarOrdenCompra
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
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("NroOrdenCompra");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Estado");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FechaRegistro");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("RazonSocialProveedor");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DocInterno");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton("btnBuscarProveedor");
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this.frmBuscarOrdenCompra_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this.gbBusqueda = new Infragistics.Win.Misc.UltraGroupBox();
            this.grdData = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.gbFiltro = new Infragistics.Win.Misc.UltraGroupBox();
            this.btnBuscar = new Infragistics.Win.Misc.UltraButton();
            this.txtProveedor = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.lbl1 = new Infragistics.Win.Misc.UltraLabel();
            this.dtpEnd = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.dtpInit = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            this.frmBuscarOrdenCompra_Fill_Panel.ClientArea.SuspendLayout();
            this.frmBuscarOrdenCompra_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gbBusqueda)).BeginInit();
            this.gbBusqueda.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gbFiltro)).BeginInit();
            this.gbFiltro.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtProveedor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpEnd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpInit)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // frmBuscarOrdenCompra_Fill_Panel
            // 
            // 
            // frmBuscarOrdenCompra_Fill_Panel.ClientArea
            // 
            this.frmBuscarOrdenCompra_Fill_Panel.ClientArea.Controls.Add(this.gbBusqueda);
            this.frmBuscarOrdenCompra_Fill_Panel.ClientArea.Controls.Add(this.gbFiltro);
            this.frmBuscarOrdenCompra_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.frmBuscarOrdenCompra_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmBuscarOrdenCompra_Fill_Panel.Location = new System.Drawing.Point(8, 31);
            this.frmBuscarOrdenCompra_Fill_Panel.Name = "frmBuscarOrdenCompra_Fill_Panel";
            this.frmBuscarOrdenCompra_Fill_Panel.Size = new System.Drawing.Size(731, 375);
            this.frmBuscarOrdenCompra_Fill_Panel.TabIndex = 0;
            // 
            // gbBusqueda
            // 
            this.gbBusqueda.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbBusqueda.Controls.Add(this.grdData);
            this.gbBusqueda.Location = new System.Drawing.Point(7, 74);
            this.gbBusqueda.Name = "gbBusqueda";
            this.gbBusqueda.Size = new System.Drawing.Size(718, 297);
            this.gbBusqueda.TabIndex = 1;
            this.gbBusqueda.Text = "Resultado";
            // 
            // grdData
            // 
            this.grdData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdData.CausesValidation = false;
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BackColor2 = System.Drawing.Color.LightSteelBlue;
            appearance1.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            this.grdData.DisplayLayout.Appearance = appearance1;
            this.grdData.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn4.Header.Caption = "Nro. Orden Compra";
            ultraGridColumn4.Header.VisiblePosition = 0;
            ultraGridColumn4.Width = 108;
            ultraGridColumn2.Header.VisiblePosition = 3;
            ultraGridColumn2.Width = 99;
            ultraGridColumn9.Header.VisiblePosition = 2;
            ultraGridColumn9.Width = 90;
            ultraGridColumn11.Header.VisiblePosition = 1;
            ultraGridColumn11.Width = 269;
            ultraGridColumn7.Header.Caption = "Doc. Interno";
            ultraGridColumn7.Header.VisiblePosition = 4;
            ultraGridColumn7.Width = 120;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn4,
            ultraGridColumn2,
            ultraGridColumn9,
            ultraGridColumn11,
            ultraGridColumn7});
            this.grdData.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdData.DisplayLayout.InterBandSpacing = 10;
            this.grdData.DisplayLayout.MaxColScrollRegions = 1;
            this.grdData.DisplayLayout.MaxRowScrollRegions = 1;
            this.grdData.DisplayLayout.NewColumnLoadStyle = Infragistics.Win.UltraWinGrid.NewColumnLoadStyle.Hide;
            appearance2.BackColor = System.Drawing.SystemColors.Highlight;
            appearance2.BackColor2 = System.Drawing.SystemColors.ActiveCaption;
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            appearance2.FontData.BoldAsString = "True";
            appearance2.ForeColor = System.Drawing.Color.White;
            this.grdData.DisplayLayout.Override.ActiveRowAppearance = appearance2;
            this.grdData.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdData.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdData.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.grdData.DisplayLayout.Override.AllowRowSummaries = Infragistics.Win.UltraWinGrid.AllowRowSummaries.False;
            this.grdData.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            appearance3.BackColor = System.Drawing.Color.Transparent;
            this.grdData.DisplayLayout.Override.CardAreaAppearance = appearance3;
            appearance4.BackColor = System.Drawing.SystemColors.Control;
            appearance4.BackColor2 = System.Drawing.SystemColors.ControlLightLight;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            this.grdData.DisplayLayout.Override.CellAppearance = appearance4;
            this.grdData.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            appearance5.BackColor = System.Drawing.SystemColors.Control;
            appearance5.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance5.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            appearance5.ThemedElementAlpha = Infragistics.Win.Alpha.Transparent;
            this.grdData.DisplayLayout.Override.HeaderAppearance = appearance5;
            this.grdData.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance6.AlphaLevel = ((short)(187));
            this.grdData.DisplayLayout.Override.RowAlternateAppearance = appearance6;
            appearance7.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.grdData.DisplayLayout.Override.RowSelectorAppearance = appearance7;
            this.grdData.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            this.grdData.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.grdData.DisplayLayout.RowConnectorColor = System.Drawing.SystemColors.ControlDarkDark;
            this.grdData.DisplayLayout.RowConnectorStyle = Infragistics.Win.UltraWinGrid.RowConnectorStyle.Dashed;
            this.grdData.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdData.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdData.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.grdData.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdData.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.grdData.Location = new System.Drawing.Point(6, 18);
            this.grdData.Margin = new System.Windows.Forms.Padding(2);
            this.grdData.Name = "grdData";
            this.grdData.Size = new System.Drawing.Size(707, 274);
            this.grdData.TabIndex = 149;
            this.grdData.DoubleClickRow += new Infragistics.Win.UltraWinGrid.DoubleClickRowEventHandler(this.grdData_DoubleClickRow);
            this.grdData.KeyDown += new System.Windows.Forms.KeyEventHandler(this.grdData_KeyDown);
            // 
            // gbFiltro
            // 
            this.gbFiltro.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbFiltro.Controls.Add(this.btnBuscar);
            this.gbFiltro.Controls.Add(this.txtProveedor);
            this.gbFiltro.Controls.Add(this.label2);
            this.gbFiltro.Controls.Add(this.ultraLabel1);
            this.gbFiltro.Controls.Add(this.lbl1);
            this.gbFiltro.Controls.Add(this.dtpEnd);
            this.gbFiltro.Controls.Add(this.dtpInit);
            this.gbFiltro.Location = new System.Drawing.Point(7, 7);
            this.gbFiltro.Name = "gbFiltro";
            this.gbFiltro.Size = new System.Drawing.Size(718, 61);
            this.gbFiltro.TabIndex = 0;
            this.gbFiltro.Text = "Filtro";
            // 
            // btnBuscar
            // 
            this.btnBuscar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance8.Image = global::SAMBHS.Windows.WinClient.UI.Resource.system_search;
            appearance8.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance8.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance8.TextHAlignAsString = "Center";
            appearance8.TextVAlignAsString = "Middle";
            this.btnBuscar.Appearance = appearance8;
            this.btnBuscar.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBuscar.Location = new System.Drawing.Point(617, 23);
            this.btnBuscar.Margin = new System.Windows.Forms.Padding(2);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(85, 24);
            this.btnBuscar.TabIndex = 103;
            this.btnBuscar.Text = "&Buscar";
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // txtProveedor
            // 
            this.txtProveedor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance9.Image = global::SAMBHS.Windows.WinClient.UI.Resource.system_search;
            editorButton1.Appearance = appearance9;
            editorButton1.Key = "btnBuscarProveedor";
            this.txtProveedor.ButtonsLeft.Add(editorButton1);
            this.txtProveedor.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.WindowsVista;
            this.txtProveedor.Location = new System.Drawing.Point(404, 24);
            this.txtProveedor.Name = "txtProveedor";
            this.txtProveedor.Size = new System.Drawing.Size(184, 21);
            this.txtProveedor.TabIndex = 2;
            this.txtProveedor.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.txtProveedor_EditorButtonClick);
            this.txtProveedor.Validated += new System.EventHandler(this.txtProveedor_Validated);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(338, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 14);
            this.label2.TabIndex = 102;
            this.label2.Text = "Proveedor:";
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ultraLabel1.Location = new System.Drawing.Point(179, 28);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(58, 14);
            this.ultraLabel1.TabIndex = 1;
            this.ultraLabel1.Text = "Fecha Fin:";
            // 
            // lbl1
            // 
            this.lbl1.AutoSize = true;
            this.lbl1.Location = new System.Drawing.Point(6, 28);
            this.lbl1.Name = "lbl1";
            this.lbl1.Size = new System.Drawing.Size(68, 14);
            this.lbl1.TabIndex = 1;
            this.lbl1.Text = "Fecha Inicio:";
            // 
            // dtpEnd
            // 
            this.dtpEnd.Location = new System.Drawing.Point(243, 24);
            this.dtpEnd.Name = "dtpEnd";
            this.dtpEnd.Size = new System.Drawing.Size(89, 21);
            this.dtpEnd.TabIndex = 1;
            this.dtpEnd.ValueChanged += new System.EventHandler(this.dtpEnd_ValueChanged);
            // 
            // dtpInit
            // 
            this.dtpInit.Location = new System.Drawing.Point(84, 24);
            this.dtpInit.Name = "dtpInit";
            this.dtpInit.Size = new System.Drawing.Size(89, 21);
            this.dtpInit.TabIndex = 0;
            // 
            // _frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Left
            // 
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 31);
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Left.Name = "_frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Left";
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 375);
            // 
            // _frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Right
            // 
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(739, 31);
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Right.Name = "_frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Right";
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 375);
            // 
            // _frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Top
            // 
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Top.Name = "_frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Top";
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(747, 31);
            // 
            // _frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Bottom
            // 
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 406);
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Bottom.Name = "_frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Bottom";
            this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(747, 8);
            // 
            // frmBuscarOrdenCompra
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(747, 414);
            this.Controls.Add(this.frmBuscarOrdenCompra_Fill_Panel);
            this.Controls.Add(this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Bottom);
            this.Name = "frmBuscarOrdenCompra";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Buscar Orden de Compra";
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            this.frmBuscarOrdenCompra_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmBuscarOrdenCompra_Fill_Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gbBusqueda)).EndInit();
            this.gbBusqueda.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gbFiltro)).EndInit();
            this.gbFiltro.ResumeLayout(false);
            this.gbFiltro.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtProveedor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpEnd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpInit)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.Misc.UltraPanel frmBuscarOrdenCompra_Fill_Panel;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmBuscarOrdenCompra_UltraFormManager_Dock_Area_Bottom;
        private Infragistics.Win.Misc.UltraGroupBox gbFiltro;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraLabel lbl1;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dtpEnd;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor dtpInit;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtProveedor;
        private Infragistics.Win.Misc.UltraLabel label2;
        private Infragistics.Win.Misc.UltraButton btnBuscar;
        private Infragistics.Win.Misc.UltraGroupBox gbBusqueda;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdData;
    }
}