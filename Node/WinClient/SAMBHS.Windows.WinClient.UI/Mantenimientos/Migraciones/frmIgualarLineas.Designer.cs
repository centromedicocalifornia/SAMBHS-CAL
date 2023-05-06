namespace SAMBHS.Windows.WinClient.UI.Mantenimientos.Migraciones
{
    partial class frmIgualarLineas
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
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_UM");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("i_UMdb");
            this.ultraButton1 = new Infragistics.Win.Misc.UltraButton();
            this.grdData = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this.frmIgualarLineas_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this.uDataSource = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            this.frmIgualarLineas_Fill_Panel.ClientArea.SuspendLayout();
            this.frmIgualarLineas_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uDataSource)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraButton1
            // 
            this.ultraButton1.Location = new System.Drawing.Point(100, 380);
            this.ultraButton1.Name = "ultraButton1";
            this.ultraButton1.Size = new System.Drawing.Size(101, 23);
            this.ultraButton1.TabIndex = 3;
            this.ultraButton1.Text = "Aceptar";
            this.ultraButton1.Click += new System.EventHandler(this.ultraButton1_Click);
            // 
            // grdData
            // 
            this.grdData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            ultraGridColumn1.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
            ultraGridColumn1.Header.Caption = "U.M Importada";
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Width = 139;
            ultraGridColumn2.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.Edit;
            ultraGridColumn2.Header.Caption = "U.M Equivalente";
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Width = 139;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2});
            this.grdData.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdData.DisplayLayout.Override.AllowColSwapping = Infragistics.Win.UltraWinGrid.AllowColSwapping.NotAllowed;
            this.grdData.DisplayLayout.Override.SupportDataErrorInfo = Infragistics.Win.UltraWinGrid.SupportDataErrorInfo.CellsOnly;
            this.grdData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdData.Location = new System.Drawing.Point(11, 6);
            this.grdData.Name = "grdData";
            this.grdData.Size = new System.Drawing.Size(277, 368);
            this.grdData.TabIndex = 2;
            this.grdData.Text = "ultraGrid1";
            this.grdData.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdData_InitializeLayout);
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // frmIgualarLineas_Fill_Panel
            // 
            // 
            // frmIgualarLineas_Fill_Panel.ClientArea
            // 
            this.frmIgualarLineas_Fill_Panel.ClientArea.Controls.Add(this.ultraButton1);
            this.frmIgualarLineas_Fill_Panel.ClientArea.Controls.Add(this.grdData);
            this.frmIgualarLineas_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.frmIgualarLineas_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmIgualarLineas_Fill_Panel.Location = new System.Drawing.Point(8, 31);
            this.frmIgualarLineas_Fill_Panel.Name = "frmIgualarLineas_Fill_Panel";
            this.frmIgualarLineas_Fill_Panel.Size = new System.Drawing.Size(294, 410);
            this.frmIgualarLineas_Fill_Panel.TabIndex = 0;
            // 
            // _frmIgualarLineas_UltraFormManager_Dock_Area_Left
            // 
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 31);
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Left.Name = "_frmIgualarLineas_UltraFormManager_Dock_Area_Left";
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 410);
            // 
            // _frmIgualarLineas_UltraFormManager_Dock_Area_Right
            // 
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(302, 31);
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Right.Name = "_frmIgualarLineas_UltraFormManager_Dock_Area_Right";
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 410);
            // 
            // _frmIgualarLineas_UltraFormManager_Dock_Area_Top
            // 
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Top.Name = "_frmIgualarLineas_UltraFormManager_Dock_Area_Top";
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(310, 31);
            // 
            // _frmIgualarLineas_UltraFormManager_Dock_Area_Bottom
            // 
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 441);
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Bottom.Name = "_frmIgualarLineas_UltraFormManager_Dock_Area_Bottom";
            this._frmIgualarLineas_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(310, 8);
            // 
            // frmIgualarLineas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(310, 449);
            this.Controls.Add(this.frmIgualarLineas_Fill_Panel);
            this.Controls.Add(this._frmIgualarLineas_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._frmIgualarLineas_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._frmIgualarLineas_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._frmIgualarLineas_UltraFormManager_Dock_Area_Bottom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmIgualarLineas";
            this.ShowIcon = false;
            this.Text = "frmIgualarLineas";
            this.Load += new System.EventHandler(this.frmIgualarLineas_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            this.frmIgualarLineas_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmIgualarLineas_Fill_Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.uDataSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraButton ultraButton1;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdData;
        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.Misc.UltraPanel frmIgualarLineas_Fill_Panel;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmIgualarLineas_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmIgualarLineas_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmIgualarLineas_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmIgualarLineas_UltraFormManager_Dock_Area_Bottom;
        private Infragistics.Win.UltraWinDataSource.UltraDataSource uDataSource;
    }
}