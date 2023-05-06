namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    partial class frmBuscarTicket
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
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.txtNroTicket = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.btnBuscar = new Infragistics.Win.Misc.UltraButton();
            ((System.ComponentModel.ISupportInitialize)(this.txtNroTicket)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.ultraLabel1.Location = new System.Drawing.Point(12, 32);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(59, 14);
            this.ultraLabel1.TabIndex = 183;
            this.ultraLabel1.Text = "Nro. Ticket";
            // 
            // txtNroTicket
            // 
            this.txtNroTicket.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtNroTicket.Location = new System.Drawing.Point(77, 27);
            this.txtNroTicket.MaxLength = 16;
            this.txtNroTicket.Name = "txtNroTicket";
            this.txtNroTicket.Size = new System.Drawing.Size(98, 21);
            this.txtNroTicket.TabIndex = 182;
            // 
            // btnBuscar
            // 
            this.btnBuscar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            appearance2.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance2.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance2.TextHAlignAsString = "Right";
            appearance2.TextVAlignAsString = "Middle";
            this.btnBuscar.Appearance = appearance2;
            this.btnBuscar.Location = new System.Drawing.Point(190, 23);
            this.btnBuscar.Margin = new System.Windows.Forms.Padding(2);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(61, 30);
            this.btnBuscar.TabIndex = 184;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // frmBuscarTicket
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 68);
            this.Controls.Add(this.btnBuscar);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.txtNroTicket);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmBuscarTicket";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Buscar Ticket";
            ((System.ComponentModel.ISupportInitialize)(this.txtNroTicket)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtNroTicket;
        private Infragistics.Win.Misc.UltraButton btnBuscar;
    }
}