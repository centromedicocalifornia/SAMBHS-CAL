namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    partial class frmEditMedic
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
            this.cboMedicoIndica = new System.Windows.Forms.ComboBox();
            this.cboMedicoTratante = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(12, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 13);
            this.label1.TabIndex = 138;
            this.label1.Text = "Médico Solicitante";
            // 
            // cboMedicoIndica
            // 
            this.cboMedicoIndica.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cboMedicoIndica.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboMedicoIndica.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboMedicoIndica.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.cboMedicoIndica.DisplayMember = "Value1";
            this.cboMedicoIndica.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMedicoIndica.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboMedicoIndica.FormattingEnabled = true;
            this.cboMedicoIndica.Location = new System.Drawing.Point(11, 29);
            this.cboMedicoIndica.Margin = new System.Windows.Forms.Padding(2);
            this.cboMedicoIndica.Name = "cboMedicoIndica";
            this.cboMedicoIndica.Size = new System.Drawing.Size(422, 21);
            this.cboMedicoIndica.TabIndex = 1;
            this.cboMedicoIndica.ValueMember = "Id";
            // 
            // cboMedicoTratante
            // 
            this.cboMedicoTratante.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cboMedicoTratante.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboMedicoTratante.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboMedicoTratante.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.cboMedicoTratante.DisplayMember = "Value1";
            this.cboMedicoTratante.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMedicoTratante.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboMedicoTratante.FormattingEnabled = true;
            this.cboMedicoTratante.Location = new System.Drawing.Point(11, 82);
            this.cboMedicoTratante.Margin = new System.Windows.Forms.Padding(2);
            this.cboMedicoTratante.Name = "cboMedicoTratante";
            this.cboMedicoTratante.Size = new System.Drawing.Size(422, 21);
            this.cboMedicoTratante.TabIndex = 2;
            this.cboMedicoTratante.ValueMember = "Id";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(12, 63);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 13);
            this.label3.TabIndex = 135;
            this.label3.Text = "Médico Tratante";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.BackColor = System.Drawing.SystemColors.Control;
            this.btnOK.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnOK.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.btnOK.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Image = global::SAMBHS.Windows.SigesoftIntegration.UI.Properties.Resources.system_save;
            this.btnOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOK.Location = new System.Drawing.Point(358, 116);
            this.btnOK.Margin = new System.Windows.Forms.Padding(2);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 24);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "      Guardar";
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // frmEditMedic
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(446, 149);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboMedicoIndica);
            this.Controls.Add(this.cboMedicoTratante);
            this.Controls.Add(this.label3);
            this.Name = "frmEditMedic";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmEditMedic";
            this.Load += new System.EventHandler(this.frmEditMedic_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboMedicoIndica;
        private System.Windows.Forms.ComboBox cboMedicoTratante;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnOK;
    }
}