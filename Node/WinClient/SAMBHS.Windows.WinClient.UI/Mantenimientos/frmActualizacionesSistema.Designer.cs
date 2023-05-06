namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    partial class frmActualizacionesSistema
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.downloadStatus = new Infragistics.Win.Misc.UltraLabel();
            this.label1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraButton1 = new Infragistics.Win.Misc.UltraButton();
            this.ultraTextEditor1 = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this.frmActualizacionesSistema_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTextEditor1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            this.frmActualizacionesSistema_Fill_Panel.ClientArea.SuspendLayout();
            this.frmActualizacionesSistema_Fill_Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::SAMBHS.Windows.WinClient.UI.Properties.Resources.system_software_update;
            this.pictureBox1.Location = new System.Drawing.Point(20, 36);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(148, 153);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ultraGroupBox1.Controls.Add(this.panel1);
            this.ultraGroupBox1.Controls.Add(this.ultraButton1);
            this.ultraGroupBox1.Controls.Add(this.pictureBox1);
            this.ultraGroupBox1.Location = new System.Drawing.Point(287, 103);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(533, 206);
            this.ultraGroupBox1.TabIndex = 1;
            this.ultraGroupBox1.Text = "Actualizaciones";
            this.ultraGroupBox1.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.XP;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.downloadStatus);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(174, 95);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(343, 94);
            this.panel1.TabIndex = 2;
            // 
            // downloadStatus
            // 
            this.downloadStatus.AutoSize = true;
            this.downloadStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.downloadStatus.Location = new System.Drawing.Point(16, 60);
            this.downloadStatus.Name = "downloadStatus";
            this.downloadStatus.Size = new System.Drawing.Size(0, 0);
            this.downloadStatus.TabIndex = 8;
            this.downloadStatus.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(14, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(257, 26);
            this.label1.TabIndex = 7;
            this.label1.Text = "Descargando Actualización";
            this.label1.Visible = false;
            // 
            // ultraButton1
            // 
            this.ultraButton1.ButtonStyle = Infragistics.Win.UIElementButtonStyle.Windows8Button;
            this.ultraButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraButton1.Location = new System.Drawing.Point(174, 36);
            this.ultraButton1.Name = "ultraButton1";
            this.ultraButton1.Size = new System.Drawing.Size(343, 53);
            this.ultraButton1.TabIndex = 1;
            this.ultraButton1.Text = "Buscar Actualizaciones";
            this.ultraButton1.UseOsThemes = Infragistics.Win.DefaultableBoolean.False;
            this.ultraButton1.Click += new System.EventHandler(this.ultraButton1_Click);
            // 
            // ultraTextEditor1
            // 
            this.ultraTextEditor1.Anchor = System.Windows.Forms.AnchorStyles.None;
            appearance1.BackColor = System.Drawing.Color.Silver;
            appearance1.BackColor2 = System.Drawing.Color.Silver;
            appearance1.ForeColor = System.Drawing.Color.Black;
            appearance1.TextHAlignAsString = "Center";
            appearance1.TextVAlignAsString = "Middle";
            this.ultraTextEditor1.Appearance = appearance1;
            this.ultraTextEditor1.BackColor = System.Drawing.Color.Silver;
            this.ultraTextEditor1.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.ScenicRibbon;
            this.ultraTextEditor1.Location = new System.Drawing.Point(287, 336);
            this.ultraTextEditor1.Multiline = true;
            this.ultraTextEditor1.Name = "ultraTextEditor1";
            this.ultraTextEditor1.ReadOnly = true;
            this.ultraTextEditor1.ShowOverflowIndicator = true;
            this.ultraTextEditor1.Size = new System.Drawing.Size(533, 63);
            this.ultraTextEditor1.TabIndex = 2;
            this.ultraTextEditor1.UseAppStyling = false;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.BackColorInternal = System.Drawing.Color.DarkRed;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label2.Location = new System.Drawing.Point(287, 315);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(533, 18);
            this.label2.TabIndex = 9;
            this.label2.Text = "Error";
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // frmActualizacionesSistema_Fill_Panel
            // 
            // 
            // frmActualizacionesSistema_Fill_Panel.ClientArea
            // 
            this.frmActualizacionesSistema_Fill_Panel.ClientArea.Controls.Add(this.label2);
            this.frmActualizacionesSistema_Fill_Panel.ClientArea.Controls.Add(this.ultraTextEditor1);
            this.frmActualizacionesSistema_Fill_Panel.ClientArea.Controls.Add(this.ultraGroupBox1);
            this.frmActualizacionesSistema_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.frmActualizacionesSistema_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmActualizacionesSistema_Fill_Panel.Location = new System.Drawing.Point(8, 31);
            this.frmActualizacionesSistema_Fill_Panel.Name = "frmActualizacionesSistema_Fill_Panel";
            this.frmActualizacionesSistema_Fill_Panel.Size = new System.Drawing.Size(1051, 453);
            this.frmActualizacionesSistema_Fill_Panel.TabIndex = 0;
            // 
            // _frmActualizacionesSistema_UltraFormManager_Dock_Area_Left
            // 
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 31);
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Left.Name = "_frmActualizacionesSistema_UltraFormManager_Dock_Area_Left";
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 453);
            // 
            // _frmActualizacionesSistema_UltraFormManager_Dock_Area_Right
            // 
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(1059, 31);
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Right.Name = "_frmActualizacionesSistema_UltraFormManager_Dock_Area_Right";
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 453);
            // 
            // _frmActualizacionesSistema_UltraFormManager_Dock_Area_Top
            // 
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Top.Name = "_frmActualizacionesSistema_UltraFormManager_Dock_Area_Top";
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(1067, 31);
            // 
            // _frmActualizacionesSistema_UltraFormManager_Dock_Area_Bottom
            // 
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 484);
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Bottom.Name = "_frmActualizacionesSistema_UltraFormManager_Dock_Area_Bottom";
            this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(1067, 8);
            // 
            // frmActualizacionesSistema
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(1067, 492);
            this.Controls.Add(this.frmActualizacionesSistema_Fill_Panel);
            this.Controls.Add(this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._frmActualizacionesSistema_UltraFormManager_Dock_Area_Bottom);
            this.Name = "frmActualizacionesSistema";
            this.Text = "Actualizaciones del Sistema";
            this.Load += new System.EventHandler(this.frmActualizacionesSistema_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTextEditor1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            this.frmActualizacionesSistema_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmActualizacionesSistema_Fill_Panel.ClientArea.PerformLayout();
            this.frmActualizacionesSistema_Fill_Panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private System.Windows.Forms.Panel panel1;
        private Infragistics.Win.Misc.UltraButton ultraButton1;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor ultraTextEditor1;
        private Infragistics.Win.Misc.UltraLabel downloadStatus;
        private Infragistics.Win.Misc.UltraLabel label1;
        private Infragistics.Win.Misc.UltraLabel label2;
        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.Misc.UltraPanel frmActualizacionesSistema_Fill_Panel;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmActualizacionesSistema_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmActualizacionesSistema_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmActualizacionesSistema_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmActualizacionesSistema_UltraFormManager_Dock_Area_Bottom;
    }
}