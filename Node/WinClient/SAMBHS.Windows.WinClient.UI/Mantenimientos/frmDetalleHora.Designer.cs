namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    partial class frmDetalleHora
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtIntervalo = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDescrpcion = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.Hasta = new System.Windows.Forms.Label();
            this.dtTimeHasta = new System.Windows.Forms.DateTimePicker();
            this.dtTimeDesde = new System.Windows.Forms.DateTimePicker();
            this.label19 = new System.Windows.Forms.Label();
            this.btnFilter = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescrpcion)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.txtIntervalo);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.txtDescrpcion);
            this.groupBox2.Controls.Add(this.Hasta);
            this.groupBox2.Controls.Add(this.dtTimeHasta);
            this.groupBox2.Controls.Add(this.dtTimeDesde);
            this.groupBox2.Controls.Add(this.btnFilter);
            this.groupBox2.Controls.Add(this.label19);
            this.groupBox2.ForeColor = System.Drawing.Color.MediumBlue;
            this.groupBox2.Location = new System.Drawing.Point(6, 10);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(449, 103);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "DETALLE TURNO / HORARIO";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(282, 18);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 19);
            this.label1.TabIndex = 196;
            this.label1.Text = "Intervalo";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtIntervalo
            // 
            this.txtIntervalo.Location = new System.Drawing.Point(338, 20);
            this.txtIntervalo.MaxLength = 250;
            this.txtIntervalo.Name = "txtIntervalo";
            this.txtIntervalo.Size = new System.Drawing.Size(45, 20);
            this.txtIntervalo.TabIndex = 195;
            this.txtIntervalo.Text = "0";
            this.txtIntervalo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(10, 59);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 194;
            this.label3.Text = "Comentarios";
            // 
            // txtDescrpcion
            // 
            this.txtDescrpcion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescrpcion.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtDescrpcion.Location = new System.Drawing.Point(80, 56);
            this.txtDescrpcion.MaxLength = 200;
            this.txtDescrpcion.Multiline = true;
            this.txtDescrpcion.Name = "txtDescrpcion";
            this.txtDescrpcion.Scrollbars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDescrpcion.Size = new System.Drawing.Size(283, 43);
            this.txtDescrpcion.TabIndex = 193;
            // 
            // Hasta
            // 
            this.Hasta.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Hasta.ForeColor = System.Drawing.Color.Black;
            this.Hasta.Location = new System.Drawing.Point(137, 20);
            this.Hasta.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Hasta.Name = "Hasta";
            this.Hasta.Size = new System.Drawing.Size(37, 19);
            this.Hasta.TabIndex = 143;
            this.Hasta.Text = "Hasta";
            this.Hasta.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dtTimeHasta
            // 
            this.dtTimeHasta.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtTimeHasta.Location = new System.Drawing.Point(179, 17);
            this.dtTimeHasta.Name = "dtTimeHasta";
            this.dtTimeHasta.Size = new System.Drawing.Size(85, 20);
            this.dtTimeHasta.TabIndex = 142;
            // 
            // dtTimeDesde
            // 
            this.dtTimeDesde.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtTimeDesde.Location = new System.Drawing.Point(47, 17);
            this.dtTimeDesde.Name = "dtTimeDesde";
            this.dtTimeDesde.Size = new System.Drawing.Size(85, 20);
            this.dtTimeDesde.TabIndex = 141;
            // 
            // label19
            // 
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.ForeColor = System.Drawing.Color.Black;
            this.label19.Location = new System.Drawing.Point(10, 17);
            this.label19.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(64, 19);
            this.label19.TabIndex = 10;
            this.label19.Text = "Desde";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnFilter
            // 
            this.btnFilter.BackColor = System.Drawing.SystemColors.Control;
            this.btnFilter.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnFilter.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.btnFilter.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.btnFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFilter.ForeColor = System.Drawing.Color.Black;
            this.btnFilter.Image = global::SAMBHS.Windows.WinClient.UI.Resource.next;
            this.btnFilter.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFilter.Location = new System.Drawing.Point(368, 55);
            this.btnFilter.Margin = new System.Windows.Forms.Padding(2);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(75, 44);
            this.btnFilter.TabIndex = 140;
            this.btnFilter.Text = "OK";
            this.btnFilter.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnFilter.UseVisualStyleBackColor = false;
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // frmDetalleHora
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(461, 123);
            this.Controls.Add(this.groupBox2);
            this.Name = "frmDetalleHora";
            this.Text = "frmDetalleHora";
            this.Load += new System.EventHandler(this.frmDetalleHora_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescrpcion)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtIntervalo;
        private System.Windows.Forms.Label label3;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtDescrpcion;
        private System.Windows.Forms.Label Hasta;
        private System.Windows.Forms.DateTimePicker dtTimeHasta;
        private System.Windows.Forms.DateTimePicker dtTimeDesde;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.Label label19;
    }
}