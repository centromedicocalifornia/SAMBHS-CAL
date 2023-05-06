namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    partial class frmUbigueo
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
            this.btnImport = new Infragistics.Win.Misc.UltraButton();
            this.btnUM = new Infragistics.Win.Misc.UltraButton();
            this.btnPDT = new Infragistics.Win.Misc.UltraButton();
            this.SuspendLayout();
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(141, 12);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(103, 23);
            this.btnImport.TabIndex = 0;
            this.btnImport.Text = "Ubigueo";
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnUM
            // 
            this.btnUM.Location = new System.Drawing.Point(141, 41);
            this.btnUM.Name = "btnUM";
            this.btnUM.Size = new System.Drawing.Size(110, 23);
            this.btnUM.TabIndex = 0;
            this.btnUM.Text = "Unidad de Medida";
            this.btnUM.Click += new System.EventHandler(this.btnImportUM_Click);
            // 
            // btnPDT
            // 
            this.btnPDT.Location = new System.Drawing.Point(141, 70);
            this.btnPDT.Name = "btnPDT";
            this.btnPDT.Size = new System.Drawing.Size(103, 23);
            this.btnPDT.TabIndex = 1;
            this.btnPDT.Text = "PDT";
            this.btnPDT.Click += new System.EventHandler(this.btnPDT_Click);
            // 
            // frmUbigueo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(396, 239);
            this.Controls.Add(this.btnPDT);
            this.Controls.Add(this.btnUM);
            this.Controls.Add(this.btnImport);
            this.Name = "frmUbigueo";
            this.ShowIcon = false;
            this.Text = "Load Data";
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraButton btnImport;
        private Infragistics.Win.Misc.UltraButton btnUM;
        private Infragistics.Win.Misc.UltraButton btnPDT;
    }
}