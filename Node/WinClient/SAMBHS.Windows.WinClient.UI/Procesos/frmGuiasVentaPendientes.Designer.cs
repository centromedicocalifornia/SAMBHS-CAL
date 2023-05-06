namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    partial class frmGuiasVentaPendientes
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
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("AlmacenDestino");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Detalles");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.lblContadorFilas = new Infragistics.Win.Misc.UltraLabel();
            this.grdData = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraGroupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            this.btnBuscar = new Infragistics.Win.Misc.UltraButton();
            this.cboEstado = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.dtpFechaRegistroDe = new System.Windows.Forms.DateTimePicker();
            this.dtpFechaRegistroAl = new System.Windows.Forms.DateTimePicker();
            this.label1 = new Infragistics.Win.Misc.UltraLabel();
            this.label2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this.frmGuiasVentaPendientes_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).BeginInit();
            this.ultraGroupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboEstado)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            this.frmGuiasVentaPendientes_Fill_Panel.ClientArea.SuspendLayout();
            this.frmGuiasVentaPendientes_Fill_Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraGroupBox1.Controls.Add(this.lblContadorFilas);
            this.ultraGroupBox1.Controls.Add(this.grdData);
            this.ultraGroupBox1.Location = new System.Drawing.Point(10, 74);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(1039, 385);
            this.ultraGroupBox1.TabIndex = 1;
            this.ultraGroupBox1.Text = "Guias Pendientes";
            // 
            // lblContadorFilas
            // 
            this.lblContadorFilas.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.TextHAlignAsString = "Right";
            appearance1.TextVAlignAsString = "Middle";
            this.lblContadorFilas.Appearance = appearance1;
            this.lblContadorFilas.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.2F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContadorFilas.Location = new System.Drawing.Point(819, 12);
            this.lblContadorFilas.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblContadorFilas.Name = "lblContadorFilas";
            this.lblContadorFilas.Size = new System.Drawing.Size(213, 19);
            this.lblContadorFilas.TabIndex = 146;
            this.lblContadorFilas.Text = "No se ha realizado la búsqueda aún.";
            // 
            // grdData
            // 
            this.grdData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdData.CausesValidation = false;
            appearance2.BackColor = System.Drawing.SystemColors.Window;
            appearance2.BackColor2 = System.Drawing.Color.LightSteelBlue;
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            this.grdData.DisplayLayout.Appearance = appearance2;
            this.grdData.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn2.Header.Caption = "Almacén Destino";
            ultraGridColumn2.Header.VisiblePosition = 0;
            ultraGridColumn2.Width = 927;
            ultraGridColumn4.Header.VisiblePosition = 1;
            ultraGridColumn4.Width = 81;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn2,
            ultraGridColumn4});
            appearance3.FontData.BoldAsString = "True";
            ultraGridBand1.Override.GroupByRowAppearance = appearance3;
            this.grdData.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdData.DisplayLayout.GroupByBox.Hidden = true;
            this.grdData.DisplayLayout.InterBandSpacing = 10;
            this.grdData.DisplayLayout.MaxColScrollRegions = 1;
            this.grdData.DisplayLayout.MaxRowScrollRegions = 1;
            this.grdData.DisplayLayout.NewColumnLoadStyle = Infragistics.Win.UltraWinGrid.NewColumnLoadStyle.Hide;
            appearance4.BackColor = System.Drawing.SystemColors.Highlight;
            appearance4.BackColor2 = System.Drawing.SystemColors.ActiveCaption;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            appearance4.FontData.BoldAsString = "True";
            appearance4.ForeColor = System.Drawing.Color.White;
            this.grdData.DisplayLayout.Override.ActiveRowAppearance = appearance4;
            this.grdData.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdData.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdData.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.grdData.DisplayLayout.Override.AllowRowSummaries = Infragistics.Win.UltraWinGrid.AllowRowSummaries.False;
            this.grdData.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            appearance5.BackColor = System.Drawing.Color.Transparent;
            this.grdData.DisplayLayout.Override.CardAreaAppearance = appearance5;
            appearance6.BackColor = System.Drawing.SystemColors.Control;
            appearance6.BackColor2 = System.Drawing.SystemColors.ControlLightLight;
            appearance6.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            this.grdData.DisplayLayout.Override.CellAppearance = appearance6;
            this.grdData.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            appearance7.BackColor = System.Drawing.SystemColors.Control;
            appearance7.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance7.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            appearance7.ThemedElementAlpha = Infragistics.Win.Alpha.Transparent;
            this.grdData.DisplayLayout.Override.HeaderAppearance = appearance7;
            this.grdData.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance8.AlphaLevel = ((short)(187));
            this.grdData.DisplayLayout.Override.RowAlternateAppearance = appearance8;
            appearance9.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.grdData.DisplayLayout.Override.RowSelectorAppearance = appearance9;
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
            this.grdData.Location = new System.Drawing.Point(5, 33);
            this.grdData.Margin = new System.Windows.Forms.Padding(2);
            this.grdData.Name = "grdData";
            this.grdData.Size = new System.Drawing.Size(1029, 347);
            this.grdData.TabIndex = 5;
            // 
            // ultraGroupBox2
            // 
            this.ultraGroupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraGroupBox2.Controls.Add(this.btnBuscar);
            this.ultraGroupBox2.Controls.Add(this.cboEstado);
            this.ultraGroupBox2.Controls.Add(this.ultraLabel1);
            this.ultraGroupBox2.Controls.Add(this.dtpFechaRegistroDe);
            this.ultraGroupBox2.Controls.Add(this.dtpFechaRegistroAl);
            this.ultraGroupBox2.Controls.Add(this.label1);
            this.ultraGroupBox2.Controls.Add(this.label2);
            this.ultraGroupBox2.Location = new System.Drawing.Point(10, 20);
            this.ultraGroupBox2.Name = "ultraGroupBox2";
            this.ultraGroupBox2.Size = new System.Drawing.Size(1037, 48);
            this.ultraGroupBox2.TabIndex = 0;
            this.ultraGroupBox2.Text = "Filtros de Búsqueda";
            // 
            // btnBuscar
            // 
            this.btnBuscar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance10.Image = global::SAMBHS.Windows.WinClient.UI.Resource.system_search;
            appearance10.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance10.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance10.TextHAlignAsString = "Center";
            appearance10.TextVAlignAsString = "Middle";
            this.btnBuscar.Appearance = appearance10;
            this.btnBuscar.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBuscar.Location = new System.Drawing.Point(947, 11);
            this.btnBuscar.Margin = new System.Windows.Forms.Padding(2);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(85, 35);
            this.btnBuscar.TabIndex = 13;
            this.btnBuscar.Text = "&Buscar";
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // cboEstado
            // 
            this.cboEstado.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboEstado.Location = new System.Drawing.Point(476, 18);
            this.cboEstado.Name = "cboEstado";
            this.cboEstado.Size = new System.Drawing.Size(183, 21);
            this.cboEstado.TabIndex = 2;
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.BackColorInternal = System.Drawing.Color.Transparent;
            this.ultraLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraLabel1.ForeColor = System.Drawing.SystemColors.MenuText;
            this.ultraLabel1.Location = new System.Drawing.Point(424, 20);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(46, 14);
            this.ultraLabel1.TabIndex = 7;
            this.ultraLabel1.Text = "Estado :";
            // 
            // dtpFechaRegistroDe
            // 
            this.dtpFechaRegistroDe.AllowDrop = true;
            this.dtpFechaRegistroDe.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaRegistroDe.Location = new System.Drawing.Point(75, 18);
            this.dtpFechaRegistroDe.Name = "dtpFechaRegistroDe";
            this.dtpFechaRegistroDe.Size = new System.Drawing.Size(116, 20);
            this.dtpFechaRegistroDe.TabIndex = 0;
            // 
            // dtpFechaRegistroAl
            // 
            this.dtpFechaRegistroAl.AllowDrop = true;
            this.dtpFechaRegistroAl.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaRegistroAl.Location = new System.Drawing.Point(265, 18);
            this.dtpFechaRegistroAl.Name = "dtpFechaRegistroAl";
            this.dtpFechaRegistroAl.Size = new System.Drawing.Size(113, 20);
            this.dtpFechaRegistroAl.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColorInternal = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 14);
            this.label1.TabIndex = 4;
            this.label1.Text = "Fecha  del:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColorInternal = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label2.Location = new System.Drawing.Point(242, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 14);
            this.label2.TabIndex = 6;
            this.label2.Text = "Al:";
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // frmGuiasVentaPendientes_Fill_Panel
            // 
            // 
            // frmGuiasVentaPendientes_Fill_Panel.ClientArea
            // 
            this.frmGuiasVentaPendientes_Fill_Panel.ClientArea.Controls.Add(this.ultraGroupBox2);
            this.frmGuiasVentaPendientes_Fill_Panel.ClientArea.Controls.Add(this.ultraGroupBox1);
            this.frmGuiasVentaPendientes_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.frmGuiasVentaPendientes_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmGuiasVentaPendientes_Fill_Panel.Location = new System.Drawing.Point(8, 31);
            this.frmGuiasVentaPendientes_Fill_Panel.Name = "frmGuiasVentaPendientes_Fill_Panel";
            this.frmGuiasVentaPendientes_Fill_Panel.Size = new System.Drawing.Size(1055, 465);
            this.frmGuiasVentaPendientes_Fill_Panel.TabIndex = 0;
            // 
            // _frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Left
            // 
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 31);
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Left.Name = "_frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Left";
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 465);
            // 
            // _frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Right
            // 
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(1063, 31);
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Right.Name = "_frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Right";
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 465);
            // 
            // _frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Top
            // 
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Top.Name = "_frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Top";
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(1071, 31);
            // 
            // _frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Bottom
            // 
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 496);
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Bottom.Name = "_frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Bottom";
            this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(1071, 8);
            // 
            // frmGuiasVentaPendientes
            // 
            this.AcceptButton = this.btnBuscar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1071, 504);
            this.Controls.Add(this.frmGuiasVentaPendientes_Fill_Panel);
            this.Controls.Add(this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Bottom);
            this.Name = "frmGuiasVentaPendientes";
            this.ShowIcon = false;
            this.Text = "Guias Pendientes";
            this.Load += new System.EventHandler(this.frmGuiasVentaPendientes_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).EndInit();
            this.ultraGroupBox2.ResumeLayout(false);
            this.ultraGroupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboEstado)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            this.frmGuiasVentaPendientes_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmGuiasVentaPendientes_Fill_Panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdData;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox2;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private System.Windows.Forms.DateTimePicker dtpFechaRegistroDe;
        private System.Windows.Forms.DateTimePicker dtpFechaRegistroAl;
        private Infragistics.Win.Misc.UltraLabel label1;
        private Infragistics.Win.Misc.UltraLabel label2;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboEstado;
        private Infragistics.Win.Misc.UltraButton btnBuscar;
        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.Misc.UltraPanel frmGuiasVentaPendientes_Fill_Panel;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmGuiasVentaPendientes_UltraFormManager_Dock_Area_Bottom;
        private Infragistics.Win.Misc.UltraLabel lblContadorFilas;
    }
}