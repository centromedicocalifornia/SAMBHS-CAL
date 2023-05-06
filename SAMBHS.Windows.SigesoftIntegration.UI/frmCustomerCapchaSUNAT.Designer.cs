namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    partial class frmCustomerCapchaSUNAT
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
            Infragistics.Win.UltraWinStatusBar.UltraStatusPanel ultraStatusPanel1 = new Infragistics.Win.UltraWinStatusBar.UltraStatusPanel();
            Infragistics.Win.UltraWinStatusBar.UltraStatusPanel ultraStatusPanel2 = new Infragistics.Win.UltraWinStatusBar.UltraStatusPanel();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.btnReload = new Infragistics.Win.Misc.UltraButton();
            this.txtCapcha = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.pbCapcha = new System.Windows.Forms.PictureBox();
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this.frmCustomerCapchaSUNAT_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this.uStatusBar = new Infragistics.Win.UltraWinStatusBar.UltraStatusBar();
            this.pbLoadingCapcha = new System.Windows.Forms.PictureBox();
            this.lblObteniendoCapcha = new Infragistics.Win.Misc.UltraLabel();
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            ((System.ComponentModel.ISupportInitialize)(this.txtCapcha)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCapcha)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            this.frmCustomerCapchaSUNAT_Fill_Panel.ClientArea.SuspendLayout();
            this.frmCustomerCapchaSUNAT_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uStatusBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLoadingCapcha)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(183, 74);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(62, 23);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnReload
            // 
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance1.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.btnReload.Appearance = appearance1;
            this.btnReload.Enabled = false;
            this.btnReload.ImageSize = new System.Drawing.Size(50, 50);
            this.btnReload.Location = new System.Drawing.Point(183, 9);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(62, 59);
            this.btnReload.TabIndex = 6;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // txtCapcha
            // 
            this.txtCapcha.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCapcha.Location = new System.Drawing.Point(9, 76);
            this.txtCapcha.MaxLength = 5;
            this.txtCapcha.Name = "txtCapcha";
            this.txtCapcha.Size = new System.Drawing.Size(168, 21);
            this.txtCapcha.TabIndex = 5;
            // 
            // pbCapcha
            // 
            this.pbCapcha.Location = new System.Drawing.Point(9, 9);
            this.pbCapcha.Margin = new System.Windows.Forms.Padding(0);
            this.pbCapcha.Name = "pbCapcha";
            this.pbCapcha.Size = new System.Drawing.Size(168, 62);
            this.pbCapcha.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbCapcha.TabIndex = 4;
            this.pbCapcha.TabStop = false;
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // frmCustomerCapchaSUNAT_Fill_Panel
            // 
            // 
            // frmCustomerCapchaSUNAT_Fill_Panel.ClientArea
            // 
            this.frmCustomerCapchaSUNAT_Fill_Panel.ClientArea.Controls.Add(this.uStatusBar);
            this.frmCustomerCapchaSUNAT_Fill_Panel.ClientArea.Controls.Add(this.pbLoadingCapcha);
            this.frmCustomerCapchaSUNAT_Fill_Panel.ClientArea.Controls.Add(this.lblObteniendoCapcha);
            this.frmCustomerCapchaSUNAT_Fill_Panel.ClientArea.Controls.Add(this.btnOK);
            this.frmCustomerCapchaSUNAT_Fill_Panel.ClientArea.Controls.Add(this.btnReload);
            this.frmCustomerCapchaSUNAT_Fill_Panel.ClientArea.Controls.Add(this.txtCapcha);
            this.frmCustomerCapchaSUNAT_Fill_Panel.ClientArea.Controls.Add(this.pbCapcha);
            this.frmCustomerCapchaSUNAT_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.frmCustomerCapchaSUNAT_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmCustomerCapchaSUNAT_Fill_Panel.Location = new System.Drawing.Point(8, 32);
            this.frmCustomerCapchaSUNAT_Fill_Panel.Name = "frmCustomerCapchaSUNAT_Fill_Panel";
            this.frmCustomerCapchaSUNAT_Fill_Panel.Size = new System.Drawing.Size(245, 126);
            this.frmCustomerCapchaSUNAT_Fill_Panel.TabIndex = 0;
            // 
            // uStatusBar
            // 
            this.uStatusBar.Location = new System.Drawing.Point(0, 103);
            this.uStatusBar.Name = "uStatusBar";
            ultraStatusPanel1.Key = "pMensaje";
            ultraStatusPanel1.SizingMode = Infragistics.Win.UltraWinStatusBar.PanelSizingMode.Automatic;
            ultraStatusPanel1.Text = "Ingrese el Capcha...";
            ultraStatusPanel1.WrapText = Infragistics.Win.DefaultableBoolean.False;
            ultraStatusPanel2.Key = "pMarquee";
            ultraStatusPanel2.Text = "Obteniendo datos...";
            ultraStatusPanel2.Visible = false;
            ultraStatusPanel2.Width = 183;
            this.uStatusBar.Panels.AddRange(new Infragistics.Win.UltraWinStatusBar.UltraStatusPanel[] {
            ultraStatusPanel1,
            ultraStatusPanel2});
            this.uStatusBar.Size = new System.Drawing.Size(245, 23);
            this.uStatusBar.TabIndex = 10;
            this.uStatusBar.Text = "ultraStatusBar1";
            // 
            // pbLoadingCapcha
            // 
            this.pbLoadingCapcha.Location = new System.Drawing.Point(24, 27);
            this.pbLoadingCapcha.Name = "pbLoadingCapcha";
            this.pbLoadingCapcha.Size = new System.Drawing.Size(25, 26);
            this.pbLoadingCapcha.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbLoadingCapcha.TabIndex = 9;
            this.pbLoadingCapcha.TabStop = false;
            // 
            // lblObteniendoCapcha
            // 
            this.lblObteniendoCapcha.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblObteniendoCapcha.AutoSize = true;
            this.lblObteniendoCapcha.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObteniendoCapcha.Location = new System.Drawing.Point(51, 34);
            this.lblObteniendoCapcha.Name = "lblObteniendoCapcha";
            this.lblObteniendoCapcha.Size = new System.Drawing.Size(108, 14);
            this.lblObteniendoCapcha.TabIndex = 8;
            this.lblObteniendoCapcha.Text = "Obteniendo Capcha";
            // 
            // _frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Left
            // 
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 32);
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Left.Name = "_frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Left";
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 126);
            // 
            // _frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Right
            // 
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(253, 32);
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Right.Name = "_frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Right";
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 126);
            // 
            // _frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Top
            // 
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Top.Name = "_frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Top";
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(261, 32);
            // 
            // _frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Bottom
            // 
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 158);
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Bottom.Name = "_frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Bottom";
            this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(261, 8);
            // 
            // frmCustomerCapchaSUNAT
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(261, 166);
            this.Controls.Add(this.frmCustomerCapchaSUNAT_Fill_Panel);
            this.Controls.Add(this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Bottom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCustomerCapchaSUNAT";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Capcha SUNAT";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmCustomerCapchaSUNAT_FormClosing);
            this.Load += new System.EventHandler(this.frmVenta_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmCustomerCapchaSUNAT_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.txtCapcha)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCapcha)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            this.frmCustomerCapchaSUNAT_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmCustomerCapchaSUNAT_Fill_Panel.ClientArea.PerformLayout();
            this.frmCustomerCapchaSUNAT_Fill_Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.uStatusBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbLoadingCapcha)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraButton btnOK;
        private Infragistics.Win.Misc.UltraButton btnReload;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtCapcha;
        private System.Windows.Forms.PictureBox pbCapcha;
        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.Misc.UltraPanel frmCustomerCapchaSUNAT_Fill_Panel;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmCustomerCapchaSUNAT_UltraFormManager_Dock_Area_Bottom;
        private System.Windows.Forms.PictureBox pbLoadingCapcha;
        private Infragistics.Win.Misc.UltraLabel lblObteniendoCapcha;
        private Infragistics.Win.UltraWinStatusBar.UltraStatusBar uStatusBar;
    }
}