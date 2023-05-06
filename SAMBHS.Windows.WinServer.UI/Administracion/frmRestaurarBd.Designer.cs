namespace SAMBHS.Windows.WinServer.UI.Administracion
{
    partial class frmRestaurarBd
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
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this.frmRestaurarBd_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.ultraButton1 = new Infragistics.Win.Misc.UltraButton();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraTextEditor1 = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            this.frmRestaurarBd_Fill_Panel.ClientArea.SuspendLayout();
            this.frmRestaurarBd_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTextEditor1)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // frmRestaurarBd_Fill_Panel
            // 
            // 
            // frmRestaurarBd_Fill_Panel.ClientArea
            // 
            this.frmRestaurarBd_Fill_Panel.ClientArea.Controls.Add(this.ultraGroupBox1);
            this.frmRestaurarBd_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.frmRestaurarBd_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmRestaurarBd_Fill_Panel.Location = new System.Drawing.Point(8, 31);
            this.frmRestaurarBd_Fill_Panel.Name = "frmRestaurarBd_Fill_Panel";
            this.frmRestaurarBd_Fill_Panel.Size = new System.Drawing.Size(486, 28);
            this.frmRestaurarBd_Fill_Panel.TabIndex = 0;
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Controls.Add(this.ultraButton1);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel1);
            this.ultraGroupBox1.Controls.Add(this.ultraTextEditor1);
            this.ultraGroupBox1.Location = new System.Drawing.Point(6, 6);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(490, 53);
            this.ultraGroupBox1.TabIndex = 5;
            this.ultraGroupBox1.Text = "Elegir backup para restaurar";
            // 
            // ultraButton1
            // 
            this.ultraButton1.Location = new System.Drawing.Point(406, 20);
            this.ultraButton1.Name = "ultraButton1";
            this.ultraButton1.Size = new System.Drawing.Size(75, 23);
            this.ultraButton1.TabIndex = 2;
            this.ultraButton1.Text = "Restaurar";
            this.ultraButton1.Click += new System.EventHandler(this.ultraButton1_Click);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(15, 25);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(31, 14);
            this.ultraLabel1.TabIndex = 1;
            this.ultraLabel1.Text = "Ruta:";
            // 
            // ultraTextEditor1
            // 
            appearance1.Image = global::SAMBHS.Windows.WinServer.UI.Properties.Resources.folder_database;
            editorButton1.Appearance = appearance1;
            this.ultraTextEditor1.ButtonsRight.Add(editorButton1);
            this.ultraTextEditor1.Location = new System.Drawing.Point(52, 21);
            this.ultraTextEditor1.Name = "ultraTextEditor1";
            this.ultraTextEditor1.Size = new System.Drawing.Size(348, 21);
            this.ultraTextEditor1.TabIndex = 0;
            this.ultraTextEditor1.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.ultraTextEditor1_EditorButtonClick);
            // 
            // _frmRestaurarBd_UltraFormManager_Dock_Area_Left
            // 
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 31);
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Left.Name = "_frmRestaurarBd_UltraFormManager_Dock_Area_Left";
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 28);
            // 
            // _frmRestaurarBd_UltraFormManager_Dock_Area_Right
            // 
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(494, 31);
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Right.Name = "_frmRestaurarBd_UltraFormManager_Dock_Area_Right";
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 28);
            // 
            // _frmRestaurarBd_UltraFormManager_Dock_Area_Top
            // 
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Top.Name = "_frmRestaurarBd_UltraFormManager_Dock_Area_Top";
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(502, 31);
            // 
            // _frmRestaurarBd_UltraFormManager_Dock_Area_Bottom
            // 
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 59);
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Bottom.Name = "_frmRestaurarBd_UltraFormManager_Dock_Area_Bottom";
            this._frmRestaurarBd_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(502, 8);
            // 
            // frmRestaurarBd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(518, 105);
            this.Controls.Add(this.frmRestaurarBd_Fill_Panel);
            this.Controls.Add(this._frmRestaurarBd_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._frmRestaurarBd_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._frmRestaurarBd_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._frmRestaurarBd_UltraFormManager_Dock_Area_Bottom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmRestaurarBd";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Restaurar Empresa";
            this.Load += new System.EventHandler(this.frmRestaurarBd_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            this.frmRestaurarBd_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmRestaurarBd_Fill_Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTextEditor1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.Misc.UltraPanel frmRestaurarBd_Fill_Panel;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmRestaurarBd_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmRestaurarBd_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmRestaurarBd_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmRestaurarBd_UltraFormManager_Dock_Area_Bottom;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private Infragistics.Win.Misc.UltraButton ultraButton1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor ultraTextEditor1;
    }
}