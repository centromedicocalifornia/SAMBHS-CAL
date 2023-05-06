namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    partial class frmComprobante
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
            this.rbFacturado = new System.Windows.Forms.RadioButton();
            this.rbNoFacturado = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.txtComprobante = new System.Windows.Forms.TextBox();
            this.txtLiquidacion = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblPaciente
            // 
            this.lblPaciente.AutoSize = true;
            this.lblPaciente.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPaciente.ForeColor = System.Drawing.Color.Red;
            this.lblPaciente.Location = new System.Drawing.Point(21, 18);
            this.lblPaciente.Name = "lblPaciente";
            this.lblPaciente.Size = new System.Drawing.Size(68, 13);
            this.lblPaciente.TabIndex = 0;
            this.lblPaciente.Text = "PACIENTE";
            // 
            // rbFacturado
            // 
            this.rbFacturado.AutoSize = true;
            this.rbFacturado.Location = new System.Drawing.Point(117, 63);
            this.rbFacturado.Name = "rbFacturado";
            this.rbFacturado.Size = new System.Drawing.Size(73, 17);
            this.rbFacturado.TabIndex = 1;
            this.rbFacturado.TabStop = true;
            this.rbFacturado.Text = "Facturado";
            this.rbFacturado.UseVisualStyleBackColor = true;
            // 
            // rbNoFacturado
            // 
            this.rbNoFacturado.AutoSize = true;
            this.rbNoFacturado.Location = new System.Drawing.Point(261, 63);
            this.rbNoFacturado.Name = "rbNoFacturado";
            this.rbNoFacturado.Size = new System.Drawing.Size(90, 17);
            this.rbNoFacturado.TabIndex = 2;
            this.rbNoFacturado.TabStop = true;
            this.rbNoFacturado.Text = "No Facturado";
            this.rbNoFacturado.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(21, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Facturacion";
            // 
            // txtComprobante
            // 
            this.txtComprobante.Location = new System.Drawing.Point(117, 100);
            this.txtComprobante.Name = "txtComprobante";
            this.txtComprobante.Size = new System.Drawing.Size(229, 20);
            this.txtComprobante.TabIndex = 4;
            // 
            // txtLiquidacion
            // 
            this.txtLiquidacion.Location = new System.Drawing.Point(117, 140);
            this.txtLiquidacion.Name = "txtLiquidacion";
            this.txtLiquidacion.Size = new System.Drawing.Size(229, 20);
            this.txtLiquidacion.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(21, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Comprobante";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(21, 143);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Liquidación";
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(271, 166);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(75, 23);
            this.btnGuardar.TabIndex = 8;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // frmComprobante
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(363, 195);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtLiquidacion);
            this.Controls.Add(this.txtComprobante);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.rbNoFacturado);
            this.Controls.Add(this.rbFacturado);
            this.Controls.Add(this.lblPaciente);
            this.Name = "frmComprobante";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Comprobante";
            this.Load += new System.EventHandler(this.frmComprobante_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPaciente;
        private System.Windows.Forms.RadioButton rbFacturado;
        private System.Windows.Forms.RadioButton rbNoFacturado;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtComprobante;
        private System.Windows.Forms.TextBox txtLiquidacion;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnGuardar;
    }
}