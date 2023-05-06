namespace SAMBHS.Windows.WinClient.UI
{
    partial class frmLogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLogin));
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            this.upbLogin = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this.frmLogin_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this.btnConfigurarConexión = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.txtPassword = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtUserName = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._frmLogin_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmLogin_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmLogin_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmLogin_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this.uvDatos = new Infragistics.Win.Misc.UltraValidator(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            this.frmLogin_Fill_Panel.ClientArea.SuspendLayout();
            this.frmLogin_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUserName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uvDatos)).BeginInit();
            this.SuspendLayout();
            // 
            // upbLogin
            // 
            this.upbLogin.BorderShadowColor = System.Drawing.Color.Empty;
            this.upbLogin.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.upbLogin.Image = ((object)(resources.GetObject("upbLogin.Image")));
            this.upbLogin.ImageTransparentColor = System.Drawing.SystemColors.ActiveCaption;
            this.upbLogin.Location = new System.Drawing.Point(6, 6);
            this.upbLogin.Name = "upbLogin";
            this.upbLogin.Size = new System.Drawing.Size(146, 138);
            this.upbLogin.TabIndex = 4;
            this.upbLogin.UseAppStyling = false;
            this.upbLogin.UseFlatMode = Infragistics.Win.DefaultableBoolean.True;
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // frmLogin_Fill_Panel
            // 
            // 
            // frmLogin_Fill_Panel.ClientArea
            // 
            this.frmLogin_Fill_Panel.ClientArea.Controls.Add(this.upbLogin);
            this.frmLogin_Fill_Panel.ClientArea.Controls.Add(this.btnConfigurarConexión);
            this.frmLogin_Fill_Panel.ClientArea.Controls.Add(this.btnOK);
            this.frmLogin_Fill_Panel.ClientArea.Controls.Add(this.txtPassword);
            this.frmLogin_Fill_Panel.ClientArea.Controls.Add(this.txtUserName);
            this.frmLogin_Fill_Panel.ClientArea.Controls.Add(this.label1);
            this.frmLogin_Fill_Panel.ClientArea.Controls.Add(this.label2);
            this.frmLogin_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.frmLogin_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmLogin_Fill_Panel.Location = new System.Drawing.Point(8, 32);
            this.frmLogin_Fill_Panel.Name = "frmLogin_Fill_Panel";
            this.frmLogin_Fill_Panel.Size = new System.Drawing.Size(448, 162);
            this.frmLogin_Fill_Panel.TabIndex = 0;
            // 
            // btnConfigurarConexión
            // 
            appearance1.Image = global::SAMBHS.Windows.WinServer.UI.Properties.Resources.database_connect;
            this.btnConfigurarConexión.Appearance = appearance1;
            this.btnConfigurarConexión.Location = new System.Drawing.Point(399, 95);
            this.btnConfigurarConexión.Name = "btnConfigurarConexión";
            this.btnConfigurarConexión.Size = new System.Drawing.Size(26, 31);
            this.btnConfigurarConexión.TabIndex = 9;
            this.btnConfigurarConexión.Visible = false;
            this.btnConfigurarConexión.Click += new System.EventHandler(this.btnConfigurarConexión_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(208, 95);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(174, 31);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "Iniciar Sesión";
            this.btnOK.Click += new System.EventHandler(this.ultraButton1_Click);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(208, 55);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(174, 21);
            this.txtPassword.TabIndex = 1;
            this.uvDatos.GetValidationSettings(this.txtPassword).EmptyValueCriteria = Infragistics.Win.Misc.EmptyValueCriteria.NullOrEmptyString;
            this.uvDatos.GetValidationSettings(this.txtPassword).IsRequired = true;
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(208, 23);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(174, 21);
            this.txtUserName.TabIndex = 0;
            this.uvDatos.GetValidationSettings(this.txtUserName).EmptyValueCriteria = Infragistics.Win.Misc.EmptyValueCriteria.NullOrEmptyString;
            this.uvDatos.GetValidationSettings(this.txtUserName).IsRequired = true;
            this.txtUserName.Validated += new System.EventHandler(this.txtUserName_Validated);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(153, 25);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 19);
            this.label1.TabIndex = 2;
            this.label1.Text = "Usuario";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(150, 57);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 19);
            this.label2.TabIndex = 4;
            this.label2.Text = "Password";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _frmLogin_UltraFormManager_Dock_Area_Left
            // 
            this._frmLogin_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmLogin_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmLogin_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._frmLogin_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmLogin_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._frmLogin_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmLogin_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 32);
            this._frmLogin_UltraFormManager_Dock_Area_Left.Name = "_frmLogin_UltraFormManager_Dock_Area_Left";
            this._frmLogin_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 162);
            // 
            // _frmLogin_UltraFormManager_Dock_Area_Right
            // 
            this._frmLogin_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmLogin_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmLogin_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._frmLogin_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmLogin_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._frmLogin_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmLogin_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(456, 32);
            this._frmLogin_UltraFormManager_Dock_Area_Right.Name = "_frmLogin_UltraFormManager_Dock_Area_Right";
            this._frmLogin_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 162);
            // 
            // _frmLogin_UltraFormManager_Dock_Area_Top
            // 
            this._frmLogin_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmLogin_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmLogin_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._frmLogin_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmLogin_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._frmLogin_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmLogin_UltraFormManager_Dock_Area_Top.Name = "_frmLogin_UltraFormManager_Dock_Area_Top";
            this._frmLogin_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(464, 32);
            // 
            // _frmLogin_UltraFormManager_Dock_Area_Bottom
            // 
            this._frmLogin_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmLogin_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmLogin_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._frmLogin_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmLogin_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._frmLogin_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmLogin_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 194);
            this._frmLogin_UltraFormManager_Dock_Area_Bottom.Name = "_frmLogin_UltraFormManager_Dock_Area_Bottom";
            this._frmLogin_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(464, 8);
            // 
            // frmLogin
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(464, 202);
            this.Controls.Add(this.frmLogin_Fill_Panel);
            this.Controls.Add(this._frmLogin_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._frmLogin_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._frmLogin_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._frmLogin_UltraFormManager_Dock_Area_Bottom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Acceso al Módulo de Seguridad";
            this.Load += new System.EventHandler(this.frmLogin_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            this.frmLogin_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmLogin_Fill_Panel.ClientArea.PerformLayout();
            this.frmLogin_Fill_Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUserName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uvDatos)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraPictureBox upbLogin;
        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.Misc.UltraPanel frmLogin_Fill_Panel;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmLogin_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmLogin_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmLogin_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmLogin_UltraFormManager_Dock_Area_Bottom;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtPassword;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtUserName;
        private Infragistics.Win.Misc.UltraValidator uvDatos;
        private Infragistics.Win.Misc.UltraButton btnOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private Infragistics.Win.Misc.UltraButton btnConfigurarConexión;
    }
}