namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    partial class frmAddSolicitudCarta
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
            this.txtNroCartaSolicitud = new System.Windows.Forms.TextBox();
            this.lblRegistrado = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnGrabar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtNroCartaSolicitud
            // 
            this.txtNroCartaSolicitud.Location = new System.Drawing.Point(171, 35);
            this.txtNroCartaSolicitud.Name = "txtNroCartaSolicitud";
            this.txtNroCartaSolicitud.Size = new System.Drawing.Size(192, 20);
            this.txtNroCartaSolicitud.TabIndex = 5;
            // 
            // lblRegistrado
            // 
            this.lblRegistrado.AutoSize = true;
            this.lblRegistrado.ForeColor = System.Drawing.Color.Blue;
            this.lblRegistrado.Location = new System.Drawing.Point(12, 11);
            this.lblRegistrado.Name = "lblRegistrado";
            this.lblRegistrado.Size = new System.Drawing.Size(0, 13);
            this.lblRegistrado.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(152, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "SOLICITUD/CART. GARANT:";
            // 
            // btnCancel
            // 
            this.btnCancel.ForeColor = System.Drawing.SystemColors.Highlight;
            this.btnCancel.Image = global::SAMBHS.Windows.SigesoftIntegration.UI.Properties.Resources.system_close1;
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(288, 75);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Salir";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnGrabar
            // 
            this.btnGrabar.ForeColor = System.Drawing.SystemColors.Highlight;
            this.btnGrabar.Image = global::SAMBHS.Windows.SigesoftIntegration.UI.Properties.Resources.accept;
            this.btnGrabar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGrabar.Location = new System.Drawing.Point(171, 75);
            this.btnGrabar.Name = "btnGrabar";
            this.btnGrabar.Size = new System.Drawing.Size(75, 23);
            this.btnGrabar.TabIndex = 7;
            this.btnGrabar.Text = "Guardar";
            this.btnGrabar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGrabar.UseVisualStyleBackColor = true;
            this.btnGrabar.Click += new System.EventHandler(this.btnGrabar_Click);
            // 
            // frmAddSolicitudCarta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(375, 108);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnGrabar);
            this.Controls.Add(this.txtNroCartaSolicitud);
            this.Controls.Add(this.lblRegistrado);
            this.Controls.Add(this.label1);
            this.Name = "frmAddSolicitudCarta";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Registrar";
            this.Load += new System.EventHandler(this.frmAddSolicitudCarta_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnGrabar;
        private System.Windows.Forms.TextBox txtNroCartaSolicitud;
        private System.Windows.Forms.Label lblRegistrado;
        private System.Windows.Forms.Label label1;

    }
}