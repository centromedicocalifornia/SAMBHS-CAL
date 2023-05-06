namespace SAMBHS.Windows.WinServer.UI.Administracion
{
    partial class frmMantenimientoLicenciasRegistrar
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
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton("_File");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance("_File");
            Infragistics.Win.UltraWinEditors.EditorButton editorButton2 = new Infragistics.Win.UltraWinEditors.EditorButton("_Input");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance("_Input");
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this.frmMantenimientoLicenciasRegistrar_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this.btnRegistrar = new Infragistics.Win.Misc.UltraButton();
            this.txtDescripcion = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.txtLicenciaRuta = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.txtUID = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this.ultraValidator1 = new Infragistics.Win.Misc.UltraValidator(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            this.frmMantenimientoLicenciasRegistrar_Fill_Panel.ClientArea.SuspendLayout();
            this.frmMantenimientoLicenciasRegistrar_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescripcion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLicenciaRuta)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraValidator1)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // frmMantenimientoLicenciasRegistrar_Fill_Panel
            // 
            // 
            // frmMantenimientoLicenciasRegistrar_Fill_Panel.ClientArea
            // 
            this.frmMantenimientoLicenciasRegistrar_Fill_Panel.ClientArea.Controls.Add(this.btnRegistrar);
            this.frmMantenimientoLicenciasRegistrar_Fill_Panel.ClientArea.Controls.Add(this.txtDescripcion);
            this.frmMantenimientoLicenciasRegistrar_Fill_Panel.ClientArea.Controls.Add(this.ultraLabel3);
            this.frmMantenimientoLicenciasRegistrar_Fill_Panel.ClientArea.Controls.Add(this.txtLicenciaRuta);
            this.frmMantenimientoLicenciasRegistrar_Fill_Panel.ClientArea.Controls.Add(this.ultraLabel2);
            this.frmMantenimientoLicenciasRegistrar_Fill_Panel.ClientArea.Controls.Add(this.txtUID);
            this.frmMantenimientoLicenciasRegistrar_Fill_Panel.ClientArea.Controls.Add(this.ultraLabel1);
            this.frmMantenimientoLicenciasRegistrar_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.frmMantenimientoLicenciasRegistrar_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmMantenimientoLicenciasRegistrar_Fill_Panel.Location = new System.Drawing.Point(8, 32);
            this.frmMantenimientoLicenciasRegistrar_Fill_Panel.Name = "frmMantenimientoLicenciasRegistrar_Fill_Panel";
            this.frmMantenimientoLicenciasRegistrar_Fill_Panel.Size = new System.Drawing.Size(345, 195);
            this.frmMantenimientoLicenciasRegistrar_Fill_Panel.TabIndex = 0;
            // 
            // btnRegistrar
            // 
            this.btnRegistrar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.Image = global::SAMBHS.Windows.WinServer.UI.Properties.Resources.system_save;
            this.btnRegistrar.Appearance = appearance1;
            this.btnRegistrar.Location = new System.Drawing.Point(236, 155);
            this.btnRegistrar.Name = "btnRegistrar";
            this.btnRegistrar.Size = new System.Drawing.Size(88, 29);
            this.btnRegistrar.TabIndex = 3;
            this.btnRegistrar.Text = "Registrar";
            this.btnRegistrar.Click += new System.EventHandler(this.btnRegistrar_Click);
            // 
            // txtDescripcion
            // 
            this.txtDescripcion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescripcion.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtDescripcion.Location = new System.Drawing.Point(18, 77);
            this.txtDescripcion.MaxLength = 100;
            this.txtDescripcion.Name = "txtDescripcion";
            this.txtDescripcion.Size = new System.Drawing.Size(306, 21);
            this.txtDescripcion.TabIndex = 1;
            this.ultraValidator1.GetValidationSettings(this.txtDescripcion).EmptyValueCriteria = Infragistics.Win.Misc.EmptyValueCriteria.NullOrEmptyString;
            this.ultraValidator1.GetValidationSettings(this.txtDescripcion).IsRequired = true;
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(18, 57);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(67, 14);
            this.ultraLabel3.TabIndex = 6;
            this.ultraLabel3.Text = "Descripción:";
            // 
            // txtLicenciaRuta
            // 
            this.txtLicenciaRuta.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance2.Image = global::SAMBHS.Windows.WinServer.UI.Properties.Resources.folder_explore;
            editorButton1.Appearance = appearance2;
            editorButton1.Key = "_File";
            appearance3.Image = global::SAMBHS.Windows.WinServer.UI.Properties.Resources.textfield_add;
            editorButton2.Appearance = appearance3;
            editorButton2.Key = "_Input";
            this.txtLicenciaRuta.ButtonsRight.Add(editorButton1);
            this.txtLicenciaRuta.ButtonsRight.Add(editorButton2);
            this.txtLicenciaRuta.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtLicenciaRuta.Location = new System.Drawing.Point(18, 124);
            this.txtLicenciaRuta.MaxLength = 5000;
            this.txtLicenciaRuta.Name = "txtLicenciaRuta";
            this.txtLicenciaRuta.ReadOnly = true;
            this.txtLicenciaRuta.Size = new System.Drawing.Size(306, 21);
            this.txtLicenciaRuta.TabIndex = 2;
            this.ultraValidator1.GetValidationSettings(this.txtLicenciaRuta).EmptyValueCriteria = Infragistics.Win.Misc.EmptyValueCriteria.NullOrEmptyString;
            this.ultraValidator1.GetValidationSettings(this.txtLicenciaRuta).IsRequired = true;
            this.txtLicenciaRuta.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.txtLicenciaRuta_EditorButtonClick);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(18, 104);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(99, 14);
            this.ultraLabel2.TabIndex = 4;
            this.ultraLabel2.Text = "Licencia Otorgada:";
            // 
            // txtUID
            // 
            this.txtUID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance4.TextHAlignAsString = "Center";
            this.txtUID.Appearance = appearance4;
            this.txtUID.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtUID.Location = new System.Drawing.Point(18, 30);
            this.txtUID.MaxLength = 30;
            this.txtUID.Name = "txtUID";
            this.txtUID.Size = new System.Drawing.Size(306, 21);
            this.txtUID.TabIndex = 0;
            this.ultraValidator1.GetValidationSettings(this.txtUID).EmptyValueCriteria = Infragistics.Win.Misc.EmptyValueCriteria.NullOrEmptyString;
            this.ultraValidator1.GetValidationSettings(this.txtUID).IsRequired = true;
            this.txtUID.Validating += new System.ComponentModel.CancelEventHandler(this.txtUID_Validating);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(18, 10);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(27, 14);
            this.ultraLabel1.TabIndex = 2;
            this.ultraLabel1.Text = "UID:";
            // 
            // _frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Left
            // 
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 32);
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Left.Name = "_frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Left";
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 195);
            // 
            // _frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Right
            // 
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(353, 32);
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Right.Name = "_frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Right";
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 195);
            // 
            // _frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Top
            // 
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Top.Name = "_frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Top";
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(361, 32);
            // 
            // _frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Bottom
            // 
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 227);
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Bottom.Name = "_frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Bottom";
            this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(361, 8);
            // 
            // frmMantenimientoLicenciasRegistrar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(361, 235);
            this.Controls.Add(this.frmMantenimientoLicenciasRegistrar_Fill_Panel);
            this.Controls.Add(this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Bottom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMantenimientoLicenciasRegistrar";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Registrar Licencia";
            this.Load += new System.EventHandler(this.frmMantenimientoLicenciasRegistrar_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            this.frmMantenimientoLicenciasRegistrar_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmMantenimientoLicenciasRegistrar_Fill_Panel.ClientArea.PerformLayout();
            this.frmMantenimientoLicenciasRegistrar_Fill_Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtDescripcion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLicenciaRuta)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraValidator1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.Misc.UltraPanel frmMantenimientoLicenciasRegistrar_Fill_Panel;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmMantenimientoLicenciasRegistrar_UltraFormManager_Dock_Area_Bottom;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtDescripcion;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtLicenciaRuta;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtUID;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraButton btnRegistrar;
        private Infragistics.Win.Misc.UltraValidator ultraValidator1;
    }
}