namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    partial class frmBandejaCovid19
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
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SERVICIO");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PACIENTE");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("EXAMEN");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PRECIO");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FECHA");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("COMPROBANTE");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PROTOCOLO");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("RESULTADO");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ATENCION");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbEmpresarial = new System.Windows.Forms.RadioButton();
            this.rbSeguro = new System.Windows.Forms.RadioButton();
            this.rbParticular = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dtFin = new System.Windows.Forms.DateTimePicker();
            this.dtInicio = new System.Windows.Forms.DateTimePicker();
            this.btnFiltrar = new System.Windows.Forms.Button();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grdServices = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.btnExportarBandeja = new Infragistics.Win.Misc.UltraButton();
            this.lblContadorFilas = new Infragistics.Win.Misc.UltraLabel();
            this.checkPruebaRapida = new System.Windows.Forms.CheckBox();
            this.checkPruebaAntigeno = new System.Windows.Forms.CheckBox();
            this.checkPruebaMolecular = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdServices)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkPruebaMolecular);
            this.groupBox1.Controls.Add(this.checkPruebaAntigeno);
            this.groupBox1.Controls.Add(this.checkPruebaRapida);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.dtFin);
            this.groupBox1.Controls.Add(this.dtInicio);
            this.groupBox1.Controls.Add(this.btnFiltrar);
            this.groupBox1.Controls.Add(this.txtValue);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(11, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(968, 79);
            this.groupBox1.TabIndex = 57;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Filtro";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbEmpresarial);
            this.groupBox2.Controls.Add(this.rbSeguro);
            this.groupBox2.Controls.Add(this.rbParticular);
            this.groupBox2.Location = new System.Drawing.Point(657, 7);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 66);
            this.groupBox2.TabIndex = 62;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Tipo";
            this.groupBox2.Visible = false;
            // 
            // rbEmpresarial
            // 
            this.rbEmpresarial.AutoSize = true;
            this.rbEmpresarial.Location = new System.Drawing.Point(93, 13);
            this.rbEmpresarial.Name = "rbEmpresarial";
            this.rbEmpresarial.Size = new System.Drawing.Size(79, 17);
            this.rbEmpresarial.TabIndex = 2;
            this.rbEmpresarial.Text = "Empresarial";
            this.rbEmpresarial.UseVisualStyleBackColor = true;
            // 
            // rbSeguro
            // 
            this.rbSeguro.AutoSize = true;
            this.rbSeguro.Location = new System.Drawing.Point(6, 36);
            this.rbSeguro.Name = "rbSeguro";
            this.rbSeguro.Size = new System.Drawing.Size(59, 17);
            this.rbSeguro.TabIndex = 1;
            this.rbSeguro.Text = "Seguro";
            this.rbSeguro.UseVisualStyleBackColor = true;
            // 
            // rbParticular
            // 
            this.rbParticular.AutoSize = true;
            this.rbParticular.Checked = true;
            this.rbParticular.Location = new System.Drawing.Point(6, 13);
            this.rbParticular.Name = "rbParticular";
            this.rbParticular.Size = new System.Drawing.Size(69, 17);
            this.rbParticular.TabIndex = 0;
            this.rbParticular.Text = "Particular";
            this.rbParticular.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(166, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(21, 13);
            this.label3.TabIndex = 61;
            this.label3.Text = "Fin";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 60;
            this.label2.Text = "Inicio";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // dtFin
            // 
            this.dtFin.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtFin.Location = new System.Drawing.Point(193, 19);
            this.dtFin.Name = "dtFin";
            this.dtFin.Size = new System.Drawing.Size(102, 20);
            this.dtFin.TabIndex = 59;
            this.dtFin.ValueChanged += new System.EventHandler(this.dtFin_ValueChanged);
            // 
            // dtInicio
            // 
            this.dtInicio.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtInicio.Location = new System.Drawing.Point(41, 19);
            this.dtInicio.Name = "dtInicio";
            this.dtInicio.Size = new System.Drawing.Size(102, 20);
            this.dtInicio.TabIndex = 58;
            this.dtInicio.ValueChanged += new System.EventHandler(this.dtInicio_ValueChanged);
            // 
            // btnFiltrar
            // 
            this.btnFiltrar.Location = new System.Drawing.Point(875, 16);
            this.btnFiltrar.Name = "btnFiltrar";
            this.btnFiltrar.Size = new System.Drawing.Size(75, 57);
            this.btnFiltrar.TabIndex = 57;
            this.btnFiltrar.Text = "Buscar";
            this.btnFiltrar.UseVisualStyleBackColor = true;
            this.btnFiltrar.Click += new System.EventHandler(this.btnFiltrar_Click);
            // 
            // txtValue
            // 
            this.txtValue.Location = new System.Drawing.Point(412, 19);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(222, 20);
            this.txtValue.TabIndex = 56;
            this.txtValue.TextChanged += new System.EventHandler(this.txtValue_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(324, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 26);
            this.label1.TabIndex = 55;
            this.label1.Text = "Nombre/\r\nNro Documento";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // grdServices
            // 
            this.grdServices.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdServices.CausesValidation = false;
            appearance1.BackColor = System.Drawing.Color.White;
            appearance1.BackColor2 = System.Drawing.Color.Silver;
            appearance1.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            this.grdServices.DisplayLayout.Appearance = appearance1;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Width = 130;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Width = 226;
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn3.Width = 251;
            ultraGridColumn4.Header.VisiblePosition = 3;
            ultraGridColumn4.Hidden = true;
            ultraGridColumn5.Header.VisiblePosition = 4;
            ultraGridColumn6.Header.VisiblePosition = 5;
            ultraGridColumn6.Hidden = true;
            ultraGridColumn7.Header.VisiblePosition = 6;
            ultraGridColumn7.Width = 238;
            ultraGridColumn8.Header.VisiblePosition = 7;
            ultraGridColumn8.Hidden = true;
            ultraGridColumn9.Header.VisiblePosition = 8;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn8,
            ultraGridColumn9});
            this.grdServices.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdServices.DisplayLayout.InterBandSpacing = 10;
            this.grdServices.DisplayLayout.MaxColScrollRegions = 1;
            this.grdServices.DisplayLayout.MaxRowScrollRegions = 1;
            this.grdServices.DisplayLayout.NewColumnLoadStyle = Infragistics.Win.UltraWinGrid.NewColumnLoadStyle.Hide;
            this.grdServices.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdServices.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdServices.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.grdServices.DisplayLayout.Override.AllowRowSummaries = Infragistics.Win.UltraWinGrid.AllowRowSummaries.False;
            this.grdServices.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.grdServices.DisplayLayout.Override.BorderStyleHeader = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance2.BackColor = System.Drawing.Color.Transparent;
            this.grdServices.DisplayLayout.Override.CardAreaAppearance = appearance2;
            appearance3.BackColor = System.Drawing.Color.White;
            appearance3.BackColor2 = System.Drawing.Color.White;
            appearance3.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            this.grdServices.DisplayLayout.Override.CellAppearance = appearance3;
            this.grdServices.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            appearance4.BackColor = System.Drawing.Color.White;
            appearance4.BackColor2 = System.Drawing.Color.LightGray;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance4.BorderColor = System.Drawing.Color.DarkGray;
            appearance4.ThemedElementAlpha = Infragistics.Win.Alpha.Transparent;
            this.grdServices.DisplayLayout.Override.HeaderAppearance = appearance4;
            this.grdServices.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance5.AlphaLevel = ((short)(187));
            appearance5.BackColor = System.Drawing.Color.Gainsboro;
            appearance5.BackColor2 = System.Drawing.Color.Gainsboro;
            appearance5.ForeColor = System.Drawing.Color.Black;
            appearance5.ForegroundAlpha = Infragistics.Win.Alpha.Opaque;
            this.grdServices.DisplayLayout.Override.RowAlternateAppearance = appearance5;
            appearance6.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.grdServices.DisplayLayout.Override.RowSelectorAppearance = appearance6;
            this.grdServices.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            appearance7.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            appearance7.BackColor2 = System.Drawing.SystemColors.GradientInactiveCaption;
            appearance7.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            appearance7.BorderColor = System.Drawing.SystemColors.GradientActiveCaption;
            appearance7.BorderColor2 = System.Drawing.SystemColors.GradientActiveCaption;
            appearance7.FontData.BoldAsString = "False";
            appearance7.ForeColor = System.Drawing.Color.Black;
            this.grdServices.DisplayLayout.Override.SelectedRowAppearance = appearance7;
            this.grdServices.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            this.grdServices.DisplayLayout.RowConnectorColor = System.Drawing.SystemColors.ControlDarkDark;
            this.grdServices.DisplayLayout.RowConnectorStyle = Infragistics.Win.UltraWinGrid.RowConnectorStyle.Dashed;
            this.grdServices.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdServices.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdServices.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.grdServices.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdServices.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.grdServices.Location = new System.Drawing.Point(11, 117);
            this.grdServices.Margin = new System.Windows.Forms.Padding(2);
            this.grdServices.Name = "grdServices";
            this.grdServices.Size = new System.Drawing.Size(968, 381);
            this.grdServices.TabIndex = 56;
            // 
            // btnExportarBandeja
            // 
            this.btnExportarBandeja.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            appearance13.Image = global::SAMBHS.Windows.WinClient.UI.Resource.page_excel1;
            appearance13.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance13.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance13.TextHAlignAsString = "Right";
            appearance13.TextVAlignAsString = "Middle";
            this.btnExportarBandeja.Appearance = appearance13;
            this.btnExportarBandeja.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExportarBandeja.Location = new System.Drawing.Point(11, 502);
            this.btnExportarBandeja.Margin = new System.Windows.Forms.Padding(2);
            this.btnExportarBandeja.Name = "btnExportarBandeja";
            this.btnExportarBandeja.Size = new System.Drawing.Size(146, 31);
            this.btnExportarBandeja.TabIndex = 161;
            this.btnExportarBandeja.Text = "EXPORTAR A EXCEL";
            this.btnExportarBandeja.Click += new System.EventHandler(this.btnExportarBandeja_Click);
            // 
            // lblContadorFilas
            // 
            this.lblContadorFilas.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblContadorFilas.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContadorFilas.Location = new System.Drawing.Point(668, 96);
            this.lblContadorFilas.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblContadorFilas.Name = "lblContadorFilas";
            this.lblContadorFilas.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblContadorFilas.Size = new System.Drawing.Size(311, 19);
            this.lblContadorFilas.TabIndex = 162;
            this.lblContadorFilas.Text = "No se ha realizado la búsqueda aún.";
            // 
            // checkPruebaRapida
            // 
            this.checkPruebaRapida.AutoSize = true;
            this.checkPruebaRapida.Checked = true;
            this.checkPruebaRapida.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkPruebaRapida.Location = new System.Drawing.Point(6, 56);
            this.checkPruebaRapida.Name = "checkPruebaRapida";
            this.checkPruebaRapida.Size = new System.Drawing.Size(97, 17);
            this.checkPruebaRapida.TabIndex = 63;
            this.checkPruebaRapida.Text = "Prueba Rápida";
            this.checkPruebaRapida.UseVisualStyleBackColor = true;
            this.checkPruebaRapida.CheckedChanged += new System.EventHandler(this.checkPruebaRapida_CheckedChanged);
            // 
            // checkPruebaAntigeno
            // 
            this.checkPruebaAntigeno.AutoSize = true;
            this.checkPruebaAntigeno.Location = new System.Drawing.Point(148, 56);
            this.checkPruebaAntigeno.Name = "checkPruebaAntigeno";
            this.checkPruebaAntigeno.Size = new System.Drawing.Size(122, 17);
            this.checkPruebaAntigeno.TabIndex = 64;
            this.checkPruebaAntigeno.Text = "Prueba de Antígeno";
            this.checkPruebaAntigeno.UseVisualStyleBackColor = true;
            this.checkPruebaAntigeno.CheckedChanged += new System.EventHandler(this.checkPruebaAntigeno_CheckedChanged);
            // 
            // checkPruebaMolecular
            // 
            this.checkPruebaMolecular.AutoSize = true;
            this.checkPruebaMolecular.Location = new System.Drawing.Point(327, 56);
            this.checkPruebaMolecular.Name = "checkPruebaMolecular";
            this.checkPruebaMolecular.Size = new System.Drawing.Size(109, 17);
            this.checkPruebaMolecular.TabIndex = 65;
            this.checkPruebaMolecular.Text = "Prueba Molecular";
            this.checkPruebaMolecular.UseVisualStyleBackColor = true;
            this.checkPruebaMolecular.CheckedChanged += new System.EventHandler(this.checkPruebaMolecular_CheckedChanged);
            // 
            // frmBandejaCovid19
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(997, 542);
            this.Controls.Add(this.lblContadorFilas);
            this.Controls.Add(this.btnExportarBandeja);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grdServices);
            this.Name = "frmBandejaCovid19";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmBandejaCovid19";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdServices)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbEmpresarial;
        private System.Windows.Forms.RadioButton rbSeguro;
        private System.Windows.Forms.RadioButton rbParticular;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtFin;
        private System.Windows.Forms.DateTimePicker dtInicio;
        private System.Windows.Forms.Button btnFiltrar;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.Label label1;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdServices;
        private Infragistics.Win.Misc.UltraButton btnExportarBandeja;
        private Infragistics.Win.Misc.UltraLabel lblContadorFilas;
        private System.Windows.Forms.CheckBox checkPruebaAntigeno;
        private System.Windows.Forms.CheckBox checkPruebaRapida;
        private System.Windows.Forms.CheckBox checkPruebaMolecular;
    }
}