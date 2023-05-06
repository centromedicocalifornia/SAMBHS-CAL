namespace SAMBHS.Windows.WinServer.UI.Administracion
{
    partial class frmBuscarBinPostgres
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
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            this.ultraTextEditor1 = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.R = new Infragistics.Win.Misc.UltraLabel();
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this.frmBuscarBinPostgres_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTextEditor1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            this.frmBuscarBinPostgres_Fill_Panel.ClientArea.SuspendLayout();
            this.frmBuscarBinPostgres_Fill_Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraTextEditor1
            // 
            this.ultraTextEditor1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            appearance1.Image = global::SAMBHS.Windows.WinServer.UI.Properties.Resources.folder_explore;
            editorButton1.Appearance = appearance1;
            this.ultraTextEditor1.ButtonsRight.Add(editorButton1);
            this.ultraTextEditor1.Location = new System.Drawing.Point(49, 14);
            this.ultraTextEditor1.MaxLength = 100;
            this.ultraTextEditor1.Name = "ultraTextEditor1";
            this.ultraTextEditor1.ReadOnly = true;
            this.ultraTextEditor1.Size = new System.Drawing.Size(315, 21);
            this.ultraTextEditor1.TabIndex = 12;
            this.ultraTextEditor1.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.ultraTextEditor1_EditorButtonClick);
            // 
            // R
            // 
            this.R.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.R.AutoSize = true;
            this.R.Location = new System.Drawing.Point(12, 18);
            this.R.Name = "R";
            this.R.Size = new System.Drawing.Size(31, 14);
            this.R.TabIndex = 11;
            this.R.Text = "Ruta:";
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // frmBuscarBinPostgres_Fill_Panel
            // 
            // 
            // frmBuscarBinPostgres_Fill_Panel.ClientArea
            // 
            this.frmBuscarBinPostgres_Fill_Panel.ClientArea.Controls.Add(this.ultraTextEditor1);
            this.frmBuscarBinPostgres_Fill_Panel.ClientArea.Controls.Add(this.R);
            this.frmBuscarBinPostgres_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.frmBuscarBinPostgres_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmBuscarBinPostgres_Fill_Panel.Location = new System.Drawing.Point(8, 31);
            this.frmBuscarBinPostgres_Fill_Panel.Name = "frmBuscarBinPostgres_Fill_Panel";
            this.frmBuscarBinPostgres_Fill_Panel.Size = new System.Drawing.Size(377, 51);
            this.frmBuscarBinPostgres_Fill_Panel.TabIndex = 0;
            // 
            // _frmBuscarBinPostgres_UltraFormManager_Dock_Area_Left
            // 
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 31);
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Left.Name = "_frmBuscarBinPostgres_UltraFormManager_Dock_Area_Left";
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 51);
            // 
            // _frmBuscarBinPostgres_UltraFormManager_Dock_Area_Right
            // 
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(385, 31);
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Right.Name = "_frmBuscarBinPostgres_UltraFormManager_Dock_Area_Right";
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 51);
            // 
            // _frmBuscarBinPostgres_UltraFormManager_Dock_Area_Top
            // 
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Top.Name = "_frmBuscarBinPostgres_UltraFormManager_Dock_Area_Top";
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(393, 31);
            // 
            // _frmBuscarBinPostgres_UltraFormManager_Dock_Area_Bottom
            // 
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 82);
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Bottom.Name = "_frmBuscarBinPostgres_UltraFormManager_Dock_Area_Bottom";
            this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(393, 8);
            // 
            // frmBuscarBinPostgres
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(393, 90);
            this.Controls.Add(this.frmBuscarBinPostgres_Fill_Panel);
            this.Controls.Add(this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._frmBuscarBinPostgres_UltraFormManager_Dock_Area_Bottom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmBuscarBinPostgres";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Buscar Carpeta Bin Postgres";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmBuscarBinPostgres_FormClosing);
            this.Load += new System.EventHandler(this.frmBuscarBinPostgres_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ultraTextEditor1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            this.frmBuscarBinPostgres_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmBuscarBinPostgres_Fill_Panel.ClientArea.PerformLayout();
            this.frmBuscarBinPostgres_Fill_Panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraTextEditor ultraTextEditor1;
        private Infragistics.Win.Misc.UltraLabel R;
        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.Misc.UltraPanel frmBuscarBinPostgres_Fill_Panel;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmBuscarBinPostgres_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmBuscarBinPostgres_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmBuscarBinPostgres_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmBuscarBinPostgres_UltraFormManager_Dock_Area_Bottom;
    }
}