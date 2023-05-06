namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    partial class frmVacunasCovid19
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtLote1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cboMarca1 = new System.Windows.Forms.ComboBox();
            this.lblLote = new System.Windows.Forms.Label();
            this.dtpFecha1 = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtLugar1 = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtLugar2 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.dtpFecha2 = new System.Windows.Forms.DateTimePicker();
            this.txtLote2 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cboMarca2 = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtLugar3 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.dtpFecha3 = new System.Windows.Forms.DateTimePicker();
            this.txtLote3 = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.cboMarca3 = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.btnSavePacient = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.LightSkyBlue;
            this.groupBox3.Controls.Add(this.txtLugar1);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.dtpFecha1);
            this.groupBox3.Controls.Add(this.txtLote1);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.cboMarca1);
            this.groupBox3.Controls.Add(this.lblLote);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.ForeColor = System.Drawing.Color.MediumBlue;
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(722, 58);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "1° DOSIS";
            // 
            // txtLote1
            // 
            this.txtLote1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLote1.Location = new System.Drawing.Point(274, 19);
            this.txtLote1.Margin = new System.Windows.Forms.Padding(2);
            this.txtLote1.MaxLength = 50;
            this.txtLote1.Name = "txtLote1";
            this.txtLote1.Size = new System.Drawing.Size(101, 20);
            this.txtLote1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(7, 21);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Marca";
            // 
            // cboMarca1
            // 
            this.cboMarca1.DisplayMember = "Nombre";
            this.cboMarca1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMarca1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboMarca1.FormattingEnabled = true;
            this.cboMarca1.Items.AddRange(new object[] {
            "\"1\", \"Carnet de Extranjería\"",
            "\"2\", \"DNI\"",
            "\"3\", \"Licencia de Conducir\"",
            "\"4\", \"Pasaporte\""});
            this.cboMarca1.Location = new System.Drawing.Point(64, 18);
            this.cboMarca1.Margin = new System.Windows.Forms.Padding(2);
            this.cboMarca1.Name = "cboMarca1";
            this.cboMarca1.Size = new System.Drawing.Size(165, 21);
            this.cboMarca1.TabIndex = 5;
            this.cboMarca1.ValueMember = "EsoId";
            // 
            // lblLote
            // 
            this.lblLote.AutoSize = true;
            this.lblLote.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLote.ForeColor = System.Drawing.Color.Black;
            this.lblLote.Location = new System.Drawing.Point(242, 21);
            this.lblLote.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblLote.Name = "lblLote";
            this.lblLote.Size = new System.Drawing.Size(28, 13);
            this.lblLote.TabIndex = 13;
            this.lblLote.Text = "Lote";
            // 
            // dtpFecha1
            // 
            this.dtpFecha1.Checked = false;
            this.dtpFecha1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpFecha1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFecha1.Location = new System.Drawing.Point(444, 19);
            this.dtpFecha1.Margin = new System.Windows.Forms.Padding(2);
            this.dtpFecha1.Name = "dtpFecha1";
            this.dtpFecha1.ShowCheckBox = true;
            this.dtpFecha1.Size = new System.Drawing.Size(96, 20);
            this.dtpFecha1.TabIndex = 14;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(403, 21);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Fecha";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(568, 22);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Lugar";
            // 
            // txtLugar1
            // 
            this.txtLugar1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLugar1.Location = new System.Drawing.Point(606, 18);
            this.txtLugar1.Margin = new System.Windows.Forms.Padding(2);
            this.txtLugar1.MaxLength = 50;
            this.txtLugar1.Name = "txtLugar1";
            this.txtLugar1.Size = new System.Drawing.Size(101, 20);
            this.txtLugar1.TabIndex = 17;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtLugar2);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.dtpFecha2);
            this.groupBox1.Controls.Add(this.txtLote2);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.cboMarca2);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.Lavender;
            this.groupBox1.Location = new System.Drawing.Point(12, 76);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(722, 58);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "2° DOSIS";
            // 
            // txtLugar2
            // 
            this.txtLugar2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLugar2.Location = new System.Drawing.Point(606, 18);
            this.txtLugar2.Margin = new System.Windows.Forms.Padding(2);
            this.txtLugar2.MaxLength = 50;
            this.txtLugar2.Name = "txtLugar2";
            this.txtLugar2.Size = new System.Drawing.Size(101, 20);
            this.txtLugar2.TabIndex = 17;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(403, 21);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Fecha";
            // 
            // dtpFecha2
            // 
            this.dtpFecha2.Checked = false;
            this.dtpFecha2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpFecha2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFecha2.Location = new System.Drawing.Point(444, 19);
            this.dtpFecha2.Margin = new System.Windows.Forms.Padding(2);
            this.dtpFecha2.Name = "dtpFecha2";
            this.dtpFecha2.ShowCheckBox = true;
            this.dtpFecha2.Size = new System.Drawing.Size(96, 20);
            this.dtpFecha2.TabIndex = 14;
            // 
            // txtLote2
            // 
            this.txtLote2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLote2.Location = new System.Drawing.Point(274, 19);
            this.txtLote2.Margin = new System.Windows.Forms.Padding(2);
            this.txtLote2.MaxLength = 50;
            this.txtLote2.Name = "txtLote2";
            this.txtLote2.Size = new System.Drawing.Size(101, 20);
            this.txtLote2.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(7, 21);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Marca";
            // 
            // cboMarca2
            // 
            this.cboMarca2.DisplayMember = "Nombre";
            this.cboMarca2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMarca2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboMarca2.FormattingEnabled = true;
            this.cboMarca2.Items.AddRange(new object[] {
            "\"1\", \"Carnet de Extranjería\"",
            "\"2\", \"DNI\"",
            "\"3\", \"Licencia de Conducir\"",
            "\"4\", \"Pasaporte\""});
            this.cboMarca2.Location = new System.Drawing.Point(64, 18);
            this.cboMarca2.Margin = new System.Windows.Forms.Padding(2);
            this.cboMarca2.Name = "cboMarca2";
            this.cboMarca2.Size = new System.Drawing.Size(165, 21);
            this.cboMarca2.TabIndex = 5;
            this.cboMarca2.ValueMember = "EsoId";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(242, 21);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(28, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "Lote";
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Navy;
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.txtLugar3);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.dtpFecha3);
            this.groupBox2.Controls.Add(this.txtLote3);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.cboMarca3);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.Color.Lavender;
            this.groupBox2.Location = new System.Drawing.Point(12, 140);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(722, 58);
            this.groupBox2.TabIndex = 18;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "3° DOSIS";
            // 
            // txtLugar3
            // 
            this.txtLugar3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLugar3.Location = new System.Drawing.Point(606, 18);
            this.txtLugar3.Margin = new System.Windows.Forms.Padding(2);
            this.txtLugar3.MaxLength = 50;
            this.txtLugar3.Name = "txtLugar3";
            this.txtLugar3.Size = new System.Drawing.Size(101, 20);
            this.txtLugar3.TabIndex = 17;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(403, 21);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(37, 13);
            this.label9.TabIndex = 15;
            this.label9.Text = "Fecha";
            // 
            // dtpFecha3
            // 
            this.dtpFecha3.Checked = false;
            this.dtpFecha3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpFecha3.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFecha3.Location = new System.Drawing.Point(444, 19);
            this.dtpFecha3.Margin = new System.Windows.Forms.Padding(2);
            this.dtpFecha3.Name = "dtpFecha3";
            this.dtpFecha3.ShowCheckBox = true;
            this.dtpFecha3.Size = new System.Drawing.Size(96, 20);
            this.dtpFecha3.TabIndex = 14;
            // 
            // txtLote3
            // 
            this.txtLote3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLote3.Location = new System.Drawing.Point(274, 19);
            this.txtLote3.Margin = new System.Windows.Forms.Padding(2);
            this.txtLote3.MaxLength = 50;
            this.txtLote3.Name = "txtLote3";
            this.txtLote3.Size = new System.Drawing.Size(101, 20);
            this.txtLote3.TabIndex = 1;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.White;
            this.label10.Location = new System.Drawing.Point(7, 21);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(37, 13);
            this.label10.TabIndex = 0;
            this.label10.Text = "Marca";
            // 
            // cboMarca3
            // 
            this.cboMarca3.DisplayMember = "Nombre";
            this.cboMarca3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMarca3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboMarca3.FormattingEnabled = true;
            this.cboMarca3.Items.AddRange(new object[] {
            "\"1\", \"Carnet de Extranjería\"",
            "\"2\", \"DNI\"",
            "\"3\", \"Licencia de Conducir\"",
            "\"4\", \"Pasaporte\""});
            this.cboMarca3.Location = new System.Drawing.Point(64, 18);
            this.cboMarca3.Margin = new System.Windows.Forms.Padding(2);
            this.cboMarca3.Name = "cboMarca3";
            this.cboMarca3.Size = new System.Drawing.Size(165, 21);
            this.cboMarca3.TabIndex = 5;
            this.cboMarca3.ValueMember = "EsoId";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.White;
            this.label11.Location = new System.Drawing.Point(242, 21);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(28, 13);
            this.label11.TabIndex = 13;
            this.label11.Text = "Lote";
            // 
            // btnSavePacient
            // 
            this.btnSavePacient.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSavePacient.BackColor = System.Drawing.SystemColors.Control;
            this.btnSavePacient.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnSavePacient.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.btnSavePacient.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.btnSavePacient.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSavePacient.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSavePacient.ForeColor = System.Drawing.Color.Black;
            this.btnSavePacient.Image = global::SAMBHS.Windows.SigesoftIntegration.UI.Properties.Resources.disk;
            this.btnSavePacient.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSavePacient.Location = new System.Drawing.Point(657, 203);
            this.btnSavePacient.Margin = new System.Windows.Forms.Padding(2);
            this.btnSavePacient.Name = "btnSavePacient";
            this.btnSavePacient.Size = new System.Drawing.Size(77, 31);
            this.btnSavePacient.TabIndex = 35;
            this.btnSavePacient.Text = "Guardar";
            this.btnSavePacient.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSavePacient.UseVisualStyleBackColor = false;
            this.btnSavePacient.Click += new System.EventHandler(this.btnSavePacient_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(568, 25);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "Lugar";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(568, 21);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(34, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = "Lugar";
            // 
            // frmVacunasCovid19
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(746, 238);
            this.Controls.Add(this.btnSavePacient);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Name = "frmVacunasCovid19";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "VACUNACION COVID 19";
            this.Load += new System.EventHandler(this.frmVacunasCovid19_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtLote1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboMarca1;
        private System.Windows.Forms.Label lblLote;
        private System.Windows.Forms.DateTimePicker dtpFecha1;
        private System.Windows.Forms.TextBox txtLugar1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtLugar2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker dtpFecha2;
        private System.Windows.Forms.TextBox txtLote2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cboMarca2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtLugar3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.DateTimePicker dtpFecha3;
        private System.Windows.Forms.TextBox txtLote3;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox cboMarca3;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnSavePacient;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label8;
    }
}