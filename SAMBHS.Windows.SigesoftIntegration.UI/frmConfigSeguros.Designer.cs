namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    partial class frmConfigSeguros
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
            this.txtPrecioBase = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFactor = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtnuevoPrecio = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbCoaseguro = new System.Windows.Forms.CheckBox();
            this.rbDeducible = new System.Windows.Forms.CheckBox();
            this.txtCoaseguro = new System.Windows.Forms.TextBox();
            this.lblMontoCoaseguro = new System.Windows.Forms.Label();
            this.txtImporte = new System.Windows.Forms.TextBox();
            this.lblmontoDeducible = new System.Windows.Forms.Label();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnSalir = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.txtPagoPaciente = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtPagoAseguradora = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Precio base";
            // 
            // txtPrecioBase
            // 
            this.txtPrecioBase.Location = new System.Drawing.Point(82, 13);
            this.txtPrecioBase.Name = "txtPrecioBase";
            this.txtPrecioBase.Size = new System.Drawing.Size(70, 20);
            this.txtPrecioBase.TabIndex = 1;
            this.txtPrecioBase.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPrecioBase_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(154, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Factor";
            // 
            // txtFactor
            // 
            this.txtFactor.Location = new System.Drawing.Point(218, 13);
            this.txtFactor.Name = "txtFactor";
            this.txtFactor.Size = new System.Drawing.Size(70, 20);
            this.txtFactor.TabIndex = 1;
            this.txtFactor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFactor_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(302, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Nuevo precio";
            // 
            // txtnuevoPrecio
            // 
            this.txtnuevoPrecio.Enabled = false;
            this.txtnuevoPrecio.Location = new System.Drawing.Point(387, 13);
            this.txtnuevoPrecio.Name = "txtnuevoPrecio";
            this.txtnuevoPrecio.Size = new System.Drawing.Size(70, 20);
            this.txtnuevoPrecio.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbCoaseguro);
            this.groupBox1.Controls.Add(this.rbDeducible);
            this.groupBox1.Controls.Add(this.txtCoaseguro);
            this.groupBox1.Controls.Add(this.lblMontoCoaseguro);
            this.groupBox1.Controls.Add(this.txtImporte);
            this.groupBox1.Controls.Add(this.lblmontoDeducible);
            this.groupBox1.Location = new System.Drawing.Point(12, 51);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(458, 48);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tipo de pago";
            // 
            // rbCoaseguro
            // 
            this.rbCoaseguro.AutoSize = true;
            this.rbCoaseguro.Location = new System.Drawing.Point(252, 22);
            this.rbCoaseguro.Name = "rbCoaseguro";
            this.rbCoaseguro.Size = new System.Drawing.Size(77, 17);
            this.rbCoaseguro.TabIndex = 2;
            this.rbCoaseguro.Text = "Coaseguro";
            this.rbCoaseguro.UseVisualStyleBackColor = true;
            // 
            // rbDeducible
            // 
            this.rbDeducible.AutoSize = true;
            this.rbDeducible.Location = new System.Drawing.Point(6, 22);
            this.rbDeducible.Name = "rbDeducible";
            this.rbDeducible.Size = new System.Drawing.Size(74, 17);
            this.rbDeducible.TabIndex = 2;
            this.rbDeducible.Text = "Deducible";
            this.rbDeducible.UseVisualStyleBackColor = true;
            // 
            // txtCoaseguro
            // 
            this.txtCoaseguro.Location = new System.Drawing.Point(375, 20);
            this.txtCoaseguro.Name = "txtCoaseguro";
            this.txtCoaseguro.Size = new System.Drawing.Size(70, 20);
            this.txtCoaseguro.TabIndex = 1;
            this.txtCoaseguro.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtImporte_KeyPress);
            // 
            // lblMontoCoaseguro
            // 
            this.lblMontoCoaseguro.AutoSize = true;
            this.lblMontoCoaseguro.Location = new System.Drawing.Point(332, 24);
            this.lblMontoCoaseguro.Name = "lblMontoCoaseguro";
            this.lblMontoCoaseguro.Size = new System.Drawing.Size(37, 13);
            this.lblMontoCoaseguro.TabIndex = 0;
            this.lblMontoCoaseguro.Text = "Monto";
            // 
            // txtImporte
            // 
            this.txtImporte.Location = new System.Drawing.Point(124, 20);
            this.txtImporte.Name = "txtImporte";
            this.txtImporte.Size = new System.Drawing.Size(70, 20);
            this.txtImporte.TabIndex = 1;
            this.txtImporte.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtImporte_KeyPress);
            // 
            // lblmontoDeducible
            // 
            this.lblmontoDeducible.AutoSize = true;
            this.lblmontoDeducible.Location = new System.Drawing.Point(81, 24);
            this.lblmontoDeducible.Name = "lblmontoDeducible";
            this.lblmontoDeducible.Size = new System.Drawing.Size(37, 13);
            this.lblmontoDeducible.TabIndex = 0;
            this.lblmontoDeducible.Text = "Monto";
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(288, 145);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(75, 23);
            this.btnGuardar.TabIndex = 3;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // btnSalir
            // 
            this.btnSalir.Location = new System.Drawing.Point(395, 145);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(75, 23);
            this.btnSalir.TabIndex = 3;
            this.btnSalir.Text = "Salir";
            this.btnSalir.UseVisualStyleBackColor = true;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 123);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Paga Paciente";
            // 
            // txtPagoPaciente
            // 
            this.txtPagoPaciente.Enabled = false;
            this.txtPagoPaciente.Location = new System.Drawing.Point(82, 119);
            this.txtPagoPaciente.Name = "txtPagoPaciente";
            this.txtPagoPaciente.Size = new System.Drawing.Size(70, 20);
            this.txtPagoPaciente.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(154, 123);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Aseguradora";
            // 
            // txtPagoAseguradora
            // 
            this.txtPagoAseguradora.Enabled = false;
            this.txtPagoAseguradora.Location = new System.Drawing.Point(218, 119);
            this.txtPagoAseguradora.Name = "txtPagoAseguradora";
            this.txtPagoAseguradora.Size = new System.Drawing.Size(70, 20);
            this.txtPagoAseguradora.TabIndex = 1;
            // 
            // frmConfigSeguros
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 177);
            this.Controls.Add(this.btnSalir);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtnuevoPrecio);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtPagoAseguradora);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtFactor);
            this.Controls.Add(this.txtPagoPaciente);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtPrecioBase);
            this.Controls.Add(this.label1);
            this.Name = "frmConfigSeguros";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmConfigSeguros_FormClosing);
            this.Load += new System.EventHandler(this.frmConfigSeguros_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPrecioBase;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFactor;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtnuevoPrecio;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblmontoDeducible;
        private System.Windows.Forms.TextBox txtImporte;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnSalir;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtPagoPaciente;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtPagoAseguradora;
        private System.Windows.Forms.CheckBox rbDeducible;
        private System.Windows.Forms.TextBox txtCoaseguro;
        private System.Windows.Forms.Label lblMontoCoaseguro;
        private System.Windows.Forms.CheckBox rbCoaseguro;
    }
}