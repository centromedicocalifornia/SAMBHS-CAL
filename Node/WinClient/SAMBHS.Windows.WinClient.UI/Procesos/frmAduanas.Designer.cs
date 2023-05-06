namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    partial class frmAduanas
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
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Id", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Value1", 1);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            this.groupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.grdData = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this.frmAduanas_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this._frmAduanas_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmAduanas_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmAduanas_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmAduanas_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            this.frmAduanas_Fill_Panel.ClientArea.SuspendLayout();
            this.frmAduanas_Fill_Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.grdData);
            this.groupBox1.Location = new System.Drawing.Point(6, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(288, 305);
            this.groupBox1.TabIndex = 124;
            // 
            // grdData
            // 
            this.grdData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdData.CausesValidation = false;
            appearance1.BackColor = System.Drawing.Color.White;
            this.grdData.DisplayLayout.Appearance = appearance1;
            ultraGridColumn4.Header.Caption = "Código";
            ultraGridColumn4.Header.VisiblePosition = 0;
            ultraGridColumn4.Width = 119;
            ultraGridColumn6.Header.Caption = "Nombre";
            ultraGridColumn6.Header.VisiblePosition = 1;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn4,
            ultraGridColumn6});
            this.grdData.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdData.DisplayLayout.MaxColScrollRegions = 1;
            this.grdData.DisplayLayout.MaxRowScrollRegions = 1;
            this.grdData.DisplayLayout.NewColumnLoadStyle = Infragistics.Win.UltraWinGrid.NewColumnLoadStyle.Hide;
            appearance2.BackColor = System.Drawing.Color.AliceBlue;
            appearance2.ForeColor = System.Drawing.Color.Black;
            this.grdData.DisplayLayout.Override.ActiveRowAppearance = appearance2;
            this.grdData.DisplayLayout.Override.AllowColSwapping = Infragistics.Win.UltraWinGrid.AllowColSwapping.NotAllowed;
            this.grdData.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            this.grdData.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance3.BackColor = System.Drawing.Color.Transparent;
            this.grdData.DisplayLayout.Override.CardAreaAppearance = appearance3;
            appearance4.BorderColor = System.Drawing.SystemColors.ControlLight;
            this.grdData.DisplayLayout.Override.CellAppearance = appearance4;
            this.grdData.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.grdData.DisplayLayout.Override.CellPadding = 3;
            appearance5.TextHAlignAsString = "Left";
            this.grdData.DisplayLayout.Override.HeaderAppearance = appearance5;
            this.grdData.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.Select;
            appearance6.BorderColor = System.Drawing.Color.White;
            appearance6.TextVAlignAsString = "Middle";
            this.grdData.DisplayLayout.Override.RowAppearance = appearance6;
            this.grdData.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.grdData.DisplayLayout.RowConnectorStyle = Infragistics.Win.UltraWinGrid.RowConnectorStyle.None;
            this.grdData.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdData.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdData.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.grdData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdData.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.grdData.Location = new System.Drawing.Point(5, 5);
            this.grdData.Margin = new System.Windows.Forms.Padding(2);
            this.grdData.Name = "grdData";
            this.grdData.Size = new System.Drawing.Size(278, 284);
            this.grdData.TabIndex = 124;
            this.grdData.DoubleClick += new System.EventHandler(this.grdData_DoubleClick);
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // frmAduanas_Fill_Panel
            // 
            // 
            // frmAduanas_Fill_Panel.ClientArea
            // 
            this.frmAduanas_Fill_Panel.ClientArea.Controls.Add(this.groupBox1);
            this.frmAduanas_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.frmAduanas_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmAduanas_Fill_Panel.Location = new System.Drawing.Point(8, 32);
            this.frmAduanas_Fill_Panel.Name = "frmAduanas_Fill_Panel";
            this.frmAduanas_Fill_Panel.Size = new System.Drawing.Size(298, 316);
            this.frmAduanas_Fill_Panel.TabIndex = 0;
            // 
            // _frmAduanas_UltraFormManager_Dock_Area_Left
            // 
            this._frmAduanas_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmAduanas_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmAduanas_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._frmAduanas_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmAduanas_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._frmAduanas_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmAduanas_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 32);
            this._frmAduanas_UltraFormManager_Dock_Area_Left.Name = "_frmAduanas_UltraFormManager_Dock_Area_Left";
            this._frmAduanas_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 316);
            // 
            // _frmAduanas_UltraFormManager_Dock_Area_Right
            // 
            this._frmAduanas_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmAduanas_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmAduanas_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._frmAduanas_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmAduanas_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._frmAduanas_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmAduanas_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(306, 32);
            this._frmAduanas_UltraFormManager_Dock_Area_Right.Name = "_frmAduanas_UltraFormManager_Dock_Area_Right";
            this._frmAduanas_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 316);
            // 
            // _frmAduanas_UltraFormManager_Dock_Area_Top
            // 
            this._frmAduanas_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmAduanas_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmAduanas_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._frmAduanas_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmAduanas_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._frmAduanas_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmAduanas_UltraFormManager_Dock_Area_Top.Name = "_frmAduanas_UltraFormManager_Dock_Area_Top";
            this._frmAduanas_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(314, 32);
            // 
            // _frmAduanas_UltraFormManager_Dock_Area_Bottom
            // 
            this._frmAduanas_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmAduanas_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmAduanas_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._frmAduanas_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmAduanas_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._frmAduanas_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmAduanas_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 348);
            this._frmAduanas_UltraFormManager_Dock_Area_Bottom.Name = "_frmAduanas_UltraFormManager_Dock_Area_Bottom";
            this._frmAduanas_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(314, 8);
            // 
            // frmAduanas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(314, 356);
            this.Controls.Add(this.frmAduanas_Fill_Panel);
            this.Controls.Add(this._frmAduanas_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._frmAduanas_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._frmAduanas_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._frmAduanas_UltraFormManager_Dock_Area_Bottom);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAduanas";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Códigos de Aduanas";
            this.Load += new System.EventHandler(this.frmAduanas_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmAduanas_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            this.frmAduanas_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmAduanas_Fill_Panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox groupBox1;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdData;
        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.Misc.UltraPanel frmAduanas_Fill_Panel;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmAduanas_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmAduanas_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmAduanas_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmAduanas_UltraFormManager_Dock_Area_Bottom;
    }
}