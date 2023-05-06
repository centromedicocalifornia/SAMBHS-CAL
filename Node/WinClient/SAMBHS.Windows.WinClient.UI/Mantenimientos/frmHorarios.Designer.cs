namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    partial class frmHorarios
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            this.btnGrabar = new Infragistics.Win.Misc.UltraButton();
            this.Turno = new System.Windows.Forms.GroupBox();
            this.uckActivo = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDescrpcion = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.dtFin = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.dtInicio = new System.Windows.Forms.DateTimePicker();
            this.label9 = new System.Windows.Forms.Label();
            this.Turno.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uckActivo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescrpcion)).BeginInit();
            this.SuspendLayout();
            // 
            // btnGrabar
            // 
            this.btnGrabar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.Image = global::SAMBHS.Windows.WinClient.UI.Resource.system_save;
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance1.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance1.TextHAlignAsString = "Right";
            appearance1.TextVAlignAsString = "Middle";
            this.btnGrabar.Appearance = appearance1;
            this.btnGrabar.Location = new System.Drawing.Point(275, 226);
            this.btnGrabar.Name = "btnGrabar";
            this.btnGrabar.Size = new System.Drawing.Size(77, 23);
            this.btnGrabar.TabIndex = 159;
            this.btnGrabar.Text = "&Guardar";
            this.btnGrabar.Click += new System.EventHandler(this.btnGrabar_Click);
            // 
            // Turno
            // 
            this.Turno.BackColor = System.Drawing.Color.LightBlue;
            this.Turno.Controls.Add(this.uckActivo);
            this.Turno.Controls.Add(this.label4);
            this.Turno.Controls.Add(this.label3);
            this.Turno.Controls.Add(this.txtDescrpcion);
            this.Turno.Controls.Add(this.dtFin);
            this.Turno.Controls.Add(this.label2);
            this.Turno.Controls.Add(this.dtInicio);
            this.Turno.Controls.Add(this.label9);
            this.Turno.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Turno.ForeColor = System.Drawing.Color.MediumBlue;
            this.Turno.Location = new System.Drawing.Point(12, 10);
            this.Turno.Name = "Turno";
            this.Turno.Size = new System.Drawing.Size(340, 210);
            this.Turno.TabIndex = 158;
            this.Turno.TabStop = false;
            this.Turno.Text = "Definir Horario";
            // 
            // uckActivo
            // 
            this.uckActivo.Enabled = false;
            this.uckActivo.Location = new System.Drawing.Point(109, 88);
            this.uckActivo.Name = "uckActivo";
            this.uckActivo.Size = new System.Drawing.Size(54, 20);
            this.uckActivo.TabIndex = 195;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(23, 91);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 13);
            this.label4.TabIndex = 194;
            this.label4.Text = "VIGENTE";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(23, 125);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 192;
            this.label3.Text = "Comentarios";
            // 
            // txtDescrpcion
            // 
            this.txtDescrpcion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescrpcion.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtDescrpcion.Location = new System.Drawing.Point(109, 122);
            this.txtDescrpcion.MaxLength = 200;
            this.txtDescrpcion.Multiline = true;
            this.txtDescrpcion.Name = "txtDescrpcion";
            this.txtDescrpcion.Scrollbars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDescrpcion.Size = new System.Drawing.Size(225, 82);
            this.txtDescrpcion.TabIndex = 191;
            // 
            // dtFin
            // 
            this.dtFin.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtFin.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtFin.Location = new System.Drawing.Point(109, 56);
            this.dtFin.Margin = new System.Windows.Forms.Padding(2);
            this.dtFin.Name = "dtFin";
            this.dtFin.ShowCheckBox = true;
            this.dtFin.Size = new System.Drawing.Size(96, 20);
            this.dtFin.TabIndex = 27;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(21, 59);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 26;
            this.label2.Text = "Fec. Fin";
            // 
            // dtInicio
            // 
            this.dtInicio.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtInicio.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtInicio.Location = new System.Drawing.Point(109, 23);
            this.dtInicio.Margin = new System.Windows.Forms.Padding(2);
            this.dtInicio.Name = "dtInicio";
            this.dtInicio.ShowCheckBox = true;
            this.dtInicio.Size = new System.Drawing.Size(96, 20);
            this.dtInicio.TabIndex = 25;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.Black;
            this.label9.Location = new System.Drawing.Point(21, 26);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 13);
            this.label9.TabIndex = 17;
            this.label9.Text = "Fec. Inicio";
            // 
            // frmHorarios
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 259);
            this.Controls.Add(this.btnGrabar);
            this.Controls.Add(this.Turno);
            this.Name = "frmHorarios";
            this.Text = "frmHorarios";
            this.Load += new System.EventHandler(this.frmHorarios_Load);
            this.Turno.ResumeLayout(false);
            this.Turno.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uckActivo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescrpcion)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraButton btnGrabar;
        private System.Windows.Forms.GroupBox Turno;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor uckActivo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtDescrpcion;
        private System.Windows.Forms.DateTimePicker dtFin;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtInicio;
        private System.Windows.Forms.Label label9;
    }
}