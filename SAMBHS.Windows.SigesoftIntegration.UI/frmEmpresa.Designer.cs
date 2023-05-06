namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    partial class frmEmpresa
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            this.btnConsultaInternet = new Infragistics.Win.Misc.UltraButton();
            this.txtNroDocumento = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label6 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraGroupBox4 = new Infragistics.Win.Misc.UltraGroupBox();
            this.ddlPais = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.label17 = new Infragistics.Win.Misc.UltraLabel();
            this.ddlDistrito = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ddlProvincia = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ddlDepartamento = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.label16 = new Infragistics.Win.Misc.UltraLabel();
            this.label15 = new Infragistics.Win.Misc.UltraLabel();
            this.label14 = new Infragistics.Win.Misc.UltraLabel();
            this.txtDireccion = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label32 = new Infragistics.Win.Misc.UltraLabel();
            this.txtEMail = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label12 = new Infragistics.Win.Misc.UltraLabel();
            this.label20 = new Infragistics.Win.Misc.UltraLabel();
            this.txtCelular = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.button1 = new Infragistics.Win.Misc.UltraButton();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.txtRazonSocial = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.txtNroDocumento)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox4)).BeginInit();
            this.ultraGroupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ddlPais)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ddlDistrito)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ddlProvincia)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ddlDepartamento)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDireccion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEMail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCelular)).BeginInit();
            this.SuspendLayout();
            // 
            // btnConsultaInternet
            // 
            this.btnConsultaInternet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Center;
            this.btnConsultaInternet.Appearance = appearance1;
            this.btnConsultaInternet.Location = new System.Drawing.Point(183, 7);
            this.btnConsultaInternet.Name = "btnConsultaInternet";
            this.btnConsultaInternet.Size = new System.Drawing.Size(104, 27);
            this.btnConsultaInternet.TabIndex = 19;
            this.btnConsultaInternet.Text = "Buscar en Sunat";
            this.btnConsultaInternet.Click += new System.EventHandler(this.btnConsultaInternet_Click);
            // 
            // txtNroDocumento
            // 
            this.txtNroDocumento.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNroDocumento.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtNroDocumento.Location = new System.Drawing.Point(50, 9);
            this.txtNroDocumento.MaxLength = 11;
            this.txtNroDocumento.Name = "txtNroDocumento";
            this.txtNroDocumento.Size = new System.Drawing.Size(127, 21);
            this.txtNroDocumento.TabIndex = 18;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(27, 14);
            this.label6.TabIndex = 20;
            this.label6.Text = "Ruc:";
            // 
            // ultraGroupBox4
            // 
            this.ultraGroupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraGroupBox4.Controls.Add(this.ddlPais);
            this.ultraGroupBox4.Controls.Add(this.label17);
            this.ultraGroupBox4.Controls.Add(this.ddlDistrito);
            this.ultraGroupBox4.Controls.Add(this.ddlProvincia);
            this.ultraGroupBox4.Controls.Add(this.ddlDepartamento);
            this.ultraGroupBox4.Controls.Add(this.label16);
            this.ultraGroupBox4.Controls.Add(this.label15);
            this.ultraGroupBox4.Controls.Add(this.label14);
            this.ultraGroupBox4.Location = new System.Drawing.Point(12, 110);
            this.ultraGroupBox4.Name = "ultraGroupBox4";
            this.ultraGroupBox4.Size = new System.Drawing.Size(222, 186);
            this.ultraGroupBox4.TabIndex = 21;
            this.ultraGroupBox4.Text = "Ubicación";
            this.ultraGroupBox4.UseAppStyling = false;
            // 
            // ddlPais
            // 
            this.ddlPais.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ddlPais.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.ddlPais.Location = new System.Drawing.Point(38, 33);
            this.ddlPais.Name = "ddlPais";
            this.ddlPais.Size = new System.Drawing.Size(169, 21);
            this.ddlPais.TabIndex = 0;
            this.ddlPais.ValueChanged += new System.EventHandler(this.ddlPais_ValueChanged);
            // 
            // label17
            // 
            this.label17.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label17.AutoSize = true;
            this.label17.BackColorInternal = System.Drawing.Color.Transparent;
            this.label17.Location = new System.Drawing.Point(15, 17);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(29, 14);
            this.label17.TabIndex = 6;
            this.label17.Text = "País:";
            // 
            // ddlDistrito
            // 
            this.ddlDistrito.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ddlDistrito.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.ddlDistrito.Location = new System.Drawing.Point(38, 151);
            this.ddlDistrito.Name = "ddlDistrito";
            this.ddlDistrito.Size = new System.Drawing.Size(169, 21);
            this.ddlDistrito.TabIndex = 3;
            // 
            // ddlProvincia
            // 
            this.ddlProvincia.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ddlProvincia.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.ddlProvincia.Location = new System.Drawing.Point(38, 112);
            this.ddlProvincia.Name = "ddlProvincia";
            this.ddlProvincia.Size = new System.Drawing.Size(169, 21);
            this.ddlProvincia.TabIndex = 2;
            this.ddlProvincia.ValueChanged += new System.EventHandler(this.ddlProvincia_ValueChanged);
            // 
            // ddlDepartamento
            // 
            this.ddlDepartamento.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ddlDepartamento.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.ddlDepartamento.Location = new System.Drawing.Point(38, 72);
            this.ddlDepartamento.Name = "ddlDepartamento";
            this.ddlDepartamento.Size = new System.Drawing.Size(169, 21);
            this.ddlDepartamento.TabIndex = 1;
            this.ddlDepartamento.ValueChanged += new System.EventHandler(this.ddlDepartamento_ValueChanged);
            // 
            // label16
            // 
            this.label16.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label16.AutoSize = true;
            this.label16.BackColorInternal = System.Drawing.Color.Transparent;
            this.label16.Location = new System.Drawing.Point(15, 136);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(43, 14);
            this.label16.TabIndex = 2;
            this.label16.Text = "Distrito:";
            // 
            // label15
            // 
            this.label15.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label15.AutoSize = true;
            this.label15.BackColorInternal = System.Drawing.Color.Transparent;
            this.label15.Location = new System.Drawing.Point(15, 96);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(54, 14);
            this.label15.TabIndex = 1;
            this.label15.Text = "Provincia:";
            // 
            // label14
            // 
            this.label14.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label14.AutoSize = true;
            this.label14.BackColorInternal = System.Drawing.Color.Transparent;
            this.label14.Location = new System.Drawing.Point(15, 57);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(79, 14);
            this.label14.TabIndex = 0;
            this.label14.Text = "Departamento:";
            // 
            // txtDireccion
            // 
            this.txtDireccion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDireccion.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtDireccion.Location = new System.Drawing.Point(240, 143);
            this.txtDireccion.MaxLength = 200;
            this.txtDireccion.Multiline = true;
            this.txtDireccion.Name = "txtDireccion";
            this.txtDireccion.Scrollbars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDireccion.Size = new System.Drawing.Size(202, 57);
            this.txtDireccion.TabIndex = 147;
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(240, 127);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(55, 14);
            this.label32.TabIndex = 148;
            this.label32.Text = "Dirección:";
            // 
            // txtEMail
            // 
            this.txtEMail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEMail.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtEMail.Location = new System.Drawing.Point(240, 226);
            this.txtEMail.MaxLength = 150;
            this.txtEMail.Name = "txtEMail";
            this.txtEMail.Size = new System.Drawing.Size(202, 21);
            this.txtEMail.TabIndex = 149;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(240, 206);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(40, 14);
            this.label12.TabIndex = 150;
            this.label12.Text = "E-Mail:";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(240, 253);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(43, 14);
            this.label20.TabIndex = 152;
            this.label20.Text = "Celular:";
            // 
            // txtCelular
            // 
            this.txtCelular.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCelular.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCelular.Location = new System.Drawing.Point(240, 273);
            this.txtCelular.MaxLength = 30;
            this.txtCelular.Name = "txtCelular";
            this.txtCelular.Size = new System.Drawing.Size(202, 21);
            this.txtCelular.TabIndex = 151;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance2.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance2.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance2.TextHAlignAsString = "Right";
            appearance2.TextVAlignAsString = "Middle";
            this.button1.Appearance = appearance2;
            this.button1.Location = new System.Drawing.Point(387, 301);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(55, 27);
            this.button1.TabIndex = 153;
            this.button1.Text = "&Grabar";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(16, 67);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(74, 14);
            this.ultraLabel1.TabIndex = 155;
            this.ultraLabel1.Text = "Razón Social: ";
            // 
            // txtRazonSocial
            // 
            this.txtRazonSocial.Location = new System.Drawing.Point(96, 64);
            this.txtRazonSocial.Name = "txtRazonSocial";
            this.txtRazonSocial.Size = new System.Drawing.Size(346, 20);
            this.txtRazonSocial.TabIndex = 156;
            // 
            // frmEmpresa
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 340);
            this.Controls.Add(this.txtRazonSocial);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.txtCelular);
            this.Controls.Add(this.txtEMail);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.txtDireccion);
            this.Controls.Add(this.label32);
            this.Controls.Add(this.ultraGroupBox4);
            this.Controls.Add(this.btnConsultaInternet);
            this.Controls.Add(this.txtNroDocumento);
            this.Controls.Add(this.label6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmEmpresa";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Empresa";
            this.Load += new System.EventHandler(this.frmEmpresa_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtNroDocumento)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox4)).EndInit();
            this.ultraGroupBox4.ResumeLayout(false);
            this.ultraGroupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ddlPais)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ddlDistrito)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ddlProvincia)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ddlDepartamento)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDireccion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEMail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCelular)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraButton btnConsultaInternet;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtNroDocumento;
        private Infragistics.Win.Misc.UltraLabel label6;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox4;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor ddlPais;
        private Infragistics.Win.Misc.UltraLabel label17;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor ddlDistrito;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor ddlProvincia;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor ddlDepartamento;
        private Infragistics.Win.Misc.UltraLabel label16;
        private Infragistics.Win.Misc.UltraLabel label15;
        private Infragistics.Win.Misc.UltraLabel label14;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtDireccion;
        private Infragistics.Win.Misc.UltraLabel label32;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtEMail;
        private Infragistics.Win.Misc.UltraLabel label12;
        private Infragistics.Win.Misc.UltraLabel label20;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtCelular;
        private Infragistics.Win.Misc.UltraButton button1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private System.Windows.Forms.TextBox txtRazonSocial;
    }
}