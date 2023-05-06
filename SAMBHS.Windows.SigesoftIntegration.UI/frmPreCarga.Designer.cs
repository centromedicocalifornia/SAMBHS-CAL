namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    partial class frmPreCarga
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtDocNumber = new System.Windows.Forms.TextBox();
            this.cboEmpresa = new System.Windows.Forms.ComboBox();
            this.lblEmpresa = new System.Windows.Forms.Label();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtIdorganization = new System.Windows.Forms.TextBox();
            this.gbServiceType = new System.Windows.Forms.GroupBox();
            this.rbseguros = new System.Windows.Forms.RadioButton();
            this.rbparticular = new System.Windows.Forms.RadioButton();
            this.rbocupacional = new System.Windows.Forms.RadioButton();
            this.cboContrata = new System.Windows.Forms.ComboBox();
            this.lblContrata = new System.Windows.Forms.Label();
            this.txtContrata = new System.Windows.Forms.TextBox();
            this.lblSeguros = new System.Windows.Forms.Label();
            this.gbServiceType.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "DNI";
            // 
            // txtDocNumber
            // 
            this.txtDocNumber.Location = new System.Drawing.Point(8, 67);
            this.txtDocNumber.Name = "txtDocNumber";
            this.txtDocNumber.Size = new System.Drawing.Size(123, 20);
            this.txtDocNumber.TabIndex = 4;
            this.txtDocNumber.HideSelectionChanged += new System.EventHandler(this.txtDocNumber_HideSelectionChanged);
            this.txtDocNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDocNumber_KeyPress);
            // 
            // cboEmpresa
            // 
            this.cboEmpresa.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboEmpresa.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboEmpresa.DisplayMember = "Value1";
            this.cboEmpresa.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboEmpresa.FormattingEnabled = true;
            this.cboEmpresa.Location = new System.Drawing.Point(8, 112);
            this.cboEmpresa.Margin = new System.Windows.Forms.Padding(2);
            this.cboEmpresa.Name = "cboEmpresa";
            this.cboEmpresa.Size = new System.Drawing.Size(356, 21);
            this.cboEmpresa.TabIndex = 5;
            this.cboEmpresa.ValueMember = "Id";
            this.cboEmpresa.SelectedIndexChanged += new System.EventHandler(this.cboEmpresa_SelectedIndexChanged);
            this.cboEmpresa.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cboEmpresa_KeyPress);
            // 
            // lblEmpresa
            // 
            this.lblEmpresa.AutoSize = true;
            this.lblEmpresa.Location = new System.Drawing.Point(8, 93);
            this.lblEmpresa.Name = "lblEmpresa";
            this.lblEmpresa.Size = new System.Drawing.Size(0, 13);
            this.lblEmpresa.TabIndex = 0;
            // 
            // btnBuscar
            // 
            this.btnBuscar.Image = global::SAMBHS.Windows.SigesoftIntegration.UI.Properties.Resources.accept;
            this.btnBuscar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBuscar.Location = new System.Drawing.Point(8, 176);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(114, 23);
            this.btnBuscar.TabIndex = 7;
            this.btnBuscar.Text = "Buscar y cargar";
            this.btnBuscar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnBuscar.UseVisualStyleBackColor = true;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Image = global::SAMBHS.Windows.SigesoftIntegration.UI.Properties.Resources.information;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(221, 176);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(143, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Continuar sin búsqueda";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtIdorganization
            // 
            this.txtIdorganization.Location = new System.Drawing.Point(241, 67);
            this.txtIdorganization.Name = "txtIdorganization";
            this.txtIdorganization.Size = new System.Drawing.Size(123, 20);
            this.txtIdorganization.TabIndex = 1;
            this.txtIdorganization.Visible = false;
            // 
            // gbServiceType
            // 
            this.gbServiceType.Controls.Add(this.rbseguros);
            this.gbServiceType.Controls.Add(this.rbparticular);
            this.gbServiceType.Controls.Add(this.rbocupacional);
            this.gbServiceType.Location = new System.Drawing.Point(8, 1);
            this.gbServiceType.Name = "gbServiceType";
            this.gbServiceType.Size = new System.Drawing.Size(356, 48);
            this.gbServiceType.TabIndex = 162;
            this.gbServiceType.TabStop = false;
            this.gbServiceType.Text = "Tipo de servicio";
            // 
            // rbseguros
            // 
            this.rbseguros.AutoSize = true;
            this.rbseguros.Location = new System.Drawing.Point(272, 19);
            this.rbseguros.Name = "rbseguros";
            this.rbseguros.Size = new System.Drawing.Size(78, 17);
            this.rbseguros.TabIndex = 3;
            this.rbseguros.Text = "SEGUROS";
            this.rbseguros.UseVisualStyleBackColor = true;
            this.rbseguros.CheckedChanged += new System.EventHandler(this.rbseguros_CheckedChanged);
            // 
            // rbparticular
            // 
            this.rbparticular.AutoSize = true;
            this.rbparticular.Location = new System.Drawing.Point(143, 19);
            this.rbparticular.Name = "rbparticular";
            this.rbparticular.Size = new System.Drawing.Size(93, 17);
            this.rbparticular.TabIndex = 2;
            this.rbparticular.Text = "PARTICULAR";
            this.rbparticular.UseVisualStyleBackColor = true;
            this.rbparticular.CheckedChanged += new System.EventHandler(this.rbparticular_CheckedChanged);
            // 
            // rbocupacional
            // 
            this.rbocupacional.AutoSize = true;
            this.rbocupacional.Checked = true;
            this.rbocupacional.Location = new System.Drawing.Point(6, 19);
            this.rbocupacional.Name = "rbocupacional";
            this.rbocupacional.Size = new System.Drawing.Size(101, 17);
            this.rbocupacional.TabIndex = 1;
            this.rbocupacional.TabStop = true;
            this.rbocupacional.Text = "OCUPACIONAL";
            this.rbocupacional.UseVisualStyleBackColor = true;
            this.rbocupacional.CheckedChanged += new System.EventHandler(this.rbocupacional_CheckedChanged);
            // 
            // cboContrata
            // 
            this.cboContrata.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboContrata.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboContrata.DisplayMember = "Value1";
            this.cboContrata.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboContrata.FormattingEnabled = true;
            this.cboContrata.Location = new System.Drawing.Point(8, 150);
            this.cboContrata.Margin = new System.Windows.Forms.Padding(2);
            this.cboContrata.Name = "cboContrata";
            this.cboContrata.Size = new System.Drawing.Size(356, 21);
            this.cboContrata.TabIndex = 6;
            this.cboContrata.ValueMember = "Id";
            this.cboContrata.SelectedIndexChanged += new System.EventHandler(this.cboContrata_SelectedIndexChanged);
            this.cboContrata.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cboContrata_KeyPress);
            // 
            // lblContrata
            // 
            this.lblContrata.AutoSize = true;
            this.lblContrata.Location = new System.Drawing.Point(8, 136);
            this.lblContrata.Name = "lblContrata";
            this.lblContrata.Size = new System.Drawing.Size(0, 13);
            this.lblContrata.TabIndex = 163;
            // 
            // txtContrata
            // 
            this.txtContrata.Location = new System.Drawing.Point(241, 90);
            this.txtContrata.Name = "txtContrata";
            this.txtContrata.Size = new System.Drawing.Size(123, 20);
            this.txtContrata.TabIndex = 1;
            this.txtContrata.Visible = false;
            // 
            // lblSeguros
            // 
            this.lblSeguros.AutoSize = true;
            this.lblSeguros.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSeguros.ForeColor = System.Drawing.Color.Red;
            this.lblSeguros.Location = new System.Drawing.Point(68, 18);
            this.lblSeguros.Name = "lblSeguros";
            this.lblSeguros.Size = new System.Drawing.Size(234, 20);
            this.lblSeguros.TabIndex = 4;
            this.lblSeguros.Text = "ATENCIÓN POR SEGUROS";
            this.lblSeguros.Visible = false;
            // 
            // frmPreCarga
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(376, 207);
            this.Controls.Add(this.lblSeguros);
            this.Controls.Add(this.lblContrata);
            this.Controls.Add(this.gbServiceType);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnBuscar);
            this.Controls.Add(this.cboContrata);
            this.Controls.Add(this.cboEmpresa);
            this.Controls.Add(this.txtContrata);
            this.Controls.Add(this.txtIdorganization);
            this.Controls.Add(this.txtDocNumber);
            this.Controls.Add(this.lblEmpresa);
            this.Controls.Add(this.label1);
            this.Name = "frmPreCarga";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Datos básicos de agenda";
            this.Load += new System.EventHandler(this.frmPreCarga_Load);
            this.gbServiceType.ResumeLayout(false);
            this.gbServiceType.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtDocNumber;
        private System.Windows.Forms.ComboBox cboEmpresa;
        private System.Windows.Forms.Label lblEmpresa;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtIdorganization;
        private System.Windows.Forms.GroupBox gbServiceType;
        private System.Windows.Forms.RadioButton rbseguros;
        private System.Windows.Forms.RadioButton rbparticular;
        private System.Windows.Forms.RadioButton rbocupacional;
        private System.Windows.Forms.ComboBox cboContrata;
        private System.Windows.Forms.Label lblContrata;
        private System.Windows.Forms.TextBox txtContrata;
        private System.Windows.Forms.Label lblSeguros;
    }
}