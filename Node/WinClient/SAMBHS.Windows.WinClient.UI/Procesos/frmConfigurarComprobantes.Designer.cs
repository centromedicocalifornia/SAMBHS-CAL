namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    partial class frmConfigurarComprobantes
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
            this.lblPaciente = new System.Windows.Forms.Label();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtComprobante = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.rbNoFacturado = new System.Windows.Forms.RadioButton();
            this.rbFacturado = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // lblPaciente
            // 
            this.lblPaciente.AutoSize = true;
            this.lblPaciente.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPaciente.ForeColor = System.Drawing.Color.Red;
            this.lblPaciente.Location = new System.Drawing.Point(19, 10);
            this.lblPaciente.Name = "lblPaciente";
            this.lblPaciente.Size = new System.Drawing.Size(68, 13);
            this.lblPaciente.TabIndex = 24;
            this.lblPaciente.Text = "PACIENTE";
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(472, 115);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(75, 23);
            this.btnGuardar.TabIndex = 23;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click_1);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(19, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "Comprobante";
            // 
            // txtComprobante
            // 
            this.txtComprobante.Location = new System.Drawing.Point(115, 82);
            this.txtComprobante.Name = "txtComprobante";
            this.txtComprobante.Size = new System.Drawing.Size(432, 20);
            this.txtComprobante.TabIndex = 21;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(18, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "Facturacion";
            // 
            // rbNoFacturado
            // 
            this.rbNoFacturado.AutoSize = true;
            this.rbNoFacturado.Location = new System.Drawing.Point(258, 41);
            this.rbNoFacturado.Name = "rbNoFacturado";
            this.rbNoFacturado.Size = new System.Drawing.Size(81, 17);
            this.rbNoFacturado.TabIndex = 19;
            this.rbNoFacturado.TabStop = true;
            this.rbNoFacturado.Text = "No Enviado";
            this.rbNoFacturado.UseVisualStyleBackColor = true;
            // 
            // rbFacturado
            // 
            this.rbFacturado.AutoSize = true;
            this.rbFacturado.Location = new System.Drawing.Point(114, 41);
            this.rbFacturado.Name = "rbFacturado";
            this.rbFacturado.Size = new System.Drawing.Size(64, 17);
            this.rbFacturado.TabIndex = 18;
            this.rbFacturado.TabStop = true;
            this.rbFacturado.Text = "Enviado";
            this.rbFacturado.UseVisualStyleBackColor = true;
            // 
            // frmConfigurarComprobantes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(565, 149);
            this.Controls.Add(this.lblPaciente);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtComprobante);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.rbNoFacturado);
            this.Controls.Add(this.rbFacturado);
            this.Name = "frmConfigurarComprobantes";
            this.Text = "frmConfigurarComprobantes";
            this.Load += new System.EventHandler(this.frmConfigurarComprobantes_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPaciente;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtComprobante;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton rbNoFacturado;
        private System.Windows.Forms.RadioButton rbFacturado;
    }
}