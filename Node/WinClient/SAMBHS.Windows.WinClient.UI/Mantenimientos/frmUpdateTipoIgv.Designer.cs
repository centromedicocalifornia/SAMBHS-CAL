namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    partial class frmUpdateTipoIgv
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
            this.btnExecute = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.bar = new System.Windows.Forms.ProgressBar();
            this.btnUpdatePedido = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnExecute
            // 
            this.btnExecute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExecute.Location = new System.Drawing.Point(355, 143);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(75, 23);
            this.btnExecute.TabIndex = 0;
            this.btnExecute.Text = "Actualizar";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(13, 152);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(13, 13);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "_";
            // 
            // bar
            // 
            this.bar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bar.Location = new System.Drawing.Point(12, 172);
            this.bar.Name = "bar";
            this.bar.Size = new System.Drawing.Size(418, 23);
            this.bar.TabIndex = 1;
            // 
            // btnUpdatePedido
            // 
            this.btnUpdatePedido.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdatePedido.Location = new System.Drawing.Point(355, 96);
            this.btnUpdatePedido.Name = "btnUpdatePedido";
            this.btnUpdatePedido.Size = new System.Drawing.Size(75, 41);
            this.btnUpdatePedido.TabIndex = 0;
            this.btnUpdatePedido.Text = "Actualizar Pedidos";
            this.btnUpdatePedido.UseVisualStyleBackColor = true;
            this.btnUpdatePedido.Click += new System.EventHandler(this.btnUpdatePedido_Click);
            // 
            // frmUpdateTipoIgv
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(442, 207);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.bar);
            this.Controls.Add(this.btnUpdatePedido);
            this.Controls.Add(this.btnExecute);
            this.Name = "frmUpdateTipoIgv";
            this.Text = "UPDATE IGV";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ProgressBar bar;
        private System.Windows.Forms.Button btnUpdatePedido;
    }
}