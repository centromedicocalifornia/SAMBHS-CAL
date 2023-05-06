namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    partial class frmPlanCuentasConsulta
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
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_NroCuenta");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_NombreCuenta");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("i_Naturaleza", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("i_Imputable", 0);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            this.txtNroCuentaMayor = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label1 = new Infragistics.Win.Misc.UltraLabel();
            this.grdData = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.lblNombreCuentaM = new Infragistics.Win.Misc.UltraLabel();
            this.groupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this.frmPlanCuentasConsulta_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            ((System.ComponentModel.ISupportInitialize)(this.txtNroCuentaMayor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            this.frmPlanCuentasConsulta_Fill_Panel.ClientArea.SuspendLayout();
            this.frmPlanCuentasConsulta_Fill_Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtNroCuentaMayor
            // 
            this.txtNroCuentaMayor.Location = new System.Drawing.Point(92, 19);
            this.txtNroCuentaMayor.Name = "txtNroCuentaMayor";
            this.txtNroCuentaMayor.Size = new System.Drawing.Size(177, 21);
            this.txtNroCuentaMayor.TabIndex = 0;
            this.txtNroCuentaMayor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNroCuentaMayor_KeyPress);
            this.txtNroCuentaMayor.Leave += new System.EventHandler(this.txtNroCuentaMayor_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 14);
            this.label1.TabIndex = 1;
            this.label1.Text = "Cuenta Mayor:";
            // 
            // grdData
            // 
            this.grdData.AlphaBlendMode = Infragistics.Win.AlphaBlendMode.Disabled;
            this.grdData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdData.CausesValidation = false;
            appearance1.BackColor = System.Drawing.SystemColors.ControlLight;
            appearance1.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance1.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            this.grdData.DisplayLayout.Appearance = appearance1;
            this.grdData.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn4.Header.Caption = "Número";
            ultraGridColumn4.Header.VisiblePosition = 0;
            ultraGridColumn4.Width = 55;
            ultraGridColumn5.Header.Caption = "Nombre de Cuenta";
            ultraGridColumn5.Header.VisiblePosition = 1;
            ultraGridColumn5.Width = 295;
            ultraGridColumn6.Header.Caption = "Naturaleza";
            ultraGridColumn6.Header.VisiblePosition = 2;
            ultraGridColumn6.Width = 93;
            ultraGridColumn1.Header.VisiblePosition = 3;
            ultraGridColumn1.Hidden = true;
            ultraGridColumn1.Width = 83;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn1});
            this.grdData.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdData.DisplayLayout.EmptyRowSettings.ShowEmptyRows = true;
            this.grdData.DisplayLayout.EmptyRowSettings.Style = Infragistics.Win.UltraWinGrid.EmptyRowStyle.AlignWithDataRows;
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
            this.grdData.DisplayLayout.Override.AllowColSwapping = Infragistics.Win.UltraWinGrid.AllowColSwapping.NotAllowed;
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
            appearance6.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.grdData.DisplayLayout.Override.RowSelectorAppearance = appearance6;
            appearance7.BackColor = System.Drawing.SystemColors.InactiveCaption;
            appearance7.BackColor2 = System.Drawing.SystemColors.ActiveCaption;
            appearance7.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            this.grdData.DisplayLayout.Override.SelectedRowAppearance = appearance7;
            this.grdData.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.grdData.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.grdData.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.grdData.DisplayLayout.Override.SummaryDisplayArea = Infragistics.Win.UltraWinGrid.SummaryDisplayAreas.None;
            this.grdData.DisplayLayout.RowConnectorColor = System.Drawing.SystemColors.ControlDarkDark;
            this.grdData.DisplayLayout.RowConnectorStyle = Infragistics.Win.UltraWinGrid.RowConnectorStyle.Dashed;
            this.grdData.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdData.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdData.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.grdData.ExitEditModeOnLeave = false;
            this.grdData.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.grdData.Location = new System.Drawing.Point(11, 71);
            this.grdData.Margin = new System.Windows.Forms.Padding(2);
            this.grdData.Name = "grdData";
            this.grdData.Size = new System.Drawing.Size(464, 286);
            this.grdData.TabIndex = 27;
            this.grdData.TextRenderingMode = Infragistics.Win.TextRenderingMode.GDI;
            this.grdData.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdData_InitializeLayout);
            this.grdData.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.grdData_InitializeRow);
            this.grdData.DoubleClickRow += new Infragistics.Win.UltraWinGrid.DoubleClickRowEventHandler(this.grdData_DoubleClickRow);
            this.grdData.DoubleClick += new System.EventHandler(this.grdData_DoubleClick);
            this.grdData.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.grdData_KeyPress);
            // 
            // lblNombreCuentaM
            // 
            this.lblNombreCuentaM.AutoSize = true;
            this.lblNombreCuentaM.Location = new System.Drawing.Point(199, 19);
            this.lblNombreCuentaM.Name = "lblNombreCuentaM";
            this.lblNombreCuentaM.Size = new System.Drawing.Size(0, 0);
            this.lblNombreCuentaM.TabIndex = 28;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.txtNroCuentaMayor);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(11, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(464, 54);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.Text = "Búsqueda";
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // frmPlanCuentasConsulta_Fill_Panel
            // 
            // 
            // frmPlanCuentasConsulta_Fill_Panel.ClientArea
            // 
            this.frmPlanCuentasConsulta_Fill_Panel.ClientArea.Controls.Add(this.groupBox1);
            this.frmPlanCuentasConsulta_Fill_Panel.ClientArea.Controls.Add(this.lblNombreCuentaM);
            this.frmPlanCuentasConsulta_Fill_Panel.ClientArea.Controls.Add(this.grdData);
            this.frmPlanCuentasConsulta_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.frmPlanCuentasConsulta_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmPlanCuentasConsulta_Fill_Panel.Location = new System.Drawing.Point(8, 31);
            this.frmPlanCuentasConsulta_Fill_Panel.Name = "frmPlanCuentasConsulta_Fill_Panel";
            this.frmPlanCuentasConsulta_Fill_Panel.Size = new System.Drawing.Size(486, 368);
            this.frmPlanCuentasConsulta_Fill_Panel.TabIndex = 0;
            // 
            // _frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Left
            // 
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 31);
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Left.Name = "_frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Left";
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 368);
            // 
            // _frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Right
            // 
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(494, 31);
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Right.Name = "_frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Right";
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 368);
            // 
            // _frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Top
            // 
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Top.Name = "_frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Top";
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(502, 31);
            // 
            // _frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Bottom
            // 
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 399);
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Bottom.Name = "_frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Bottom";
            this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(502, 8);
            // 
            // frmPlanCuentasConsulta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(502, 407);
            this.Controls.Add(this.frmPlanCuentasConsulta_Fill_Panel);
            this.Controls.Add(this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Bottom);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPlanCuentasConsulta";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Plan de Cuentas";
            this.Load += new System.EventHandler(this.frmPlanCuentasConsulta_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmPlanCuentasConsulta_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.txtNroCuentaMayor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            this.frmPlanCuentasConsulta_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmPlanCuentasConsulta_Fill_Panel.ClientArea.PerformLayout();
            this.frmPlanCuentasConsulta_Fill_Panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtNroCuentaMayor;
        private Infragistics.Win.Misc.UltraLabel label1;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdData;
        private Infragistics.Win.Misc.UltraLabel lblNombreCuentaM;
        private Infragistics.Win.Misc.UltraGroupBox groupBox1;
        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.Misc.UltraPanel frmPlanCuentasConsulta_Fill_Panel;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmPlanCuentasConsulta_UltraFormManager_Dock_Area_Bottom;
    }
}