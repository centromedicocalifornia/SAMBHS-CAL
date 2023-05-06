namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    partial class frmMigrarCobranzasAdministrativas
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
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.cboCabCorrelativoC = new System.Windows.Forms.ComboBox();
            this.ultraLabel8 = new Infragistics.Win.Misc.UltraLabel();
            this.cboCabTipoDocumentoC = new System.Windows.Forms.ComboBox();
            this.ultraLabel9 = new Infragistics.Win.Misc.UltraLabel();
            this.cboCabGlosa = new System.Windows.Forms.ComboBox();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.cboCabTipoCambio = new System.Windows.Forms.ComboBox();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.cboCabFechaReg = new System.Windows.Forms.ComboBox();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.btnCabecera = new Infragistics.Win.Misc.UltraButton();
            this.ultraGroupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            this.cboDetCorrelativoC = new System.Windows.Forms.ComboBox();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.cboDetTipoDocumentoC = new System.Windows.Forms.ComboBox();
            this.ultraLabel7 = new Infragistics.Win.Misc.UltraLabel();
            this.cboDetTDventa = new System.Windows.Forms.ComboBox();
            this.ultraLabel10 = new Infragistics.Win.Misc.UltraLabel();
            this.cboDetMontoCobranza = new System.Windows.Forms.ComboBox();
            this.ultraLabel11 = new Infragistics.Win.Misc.UltraLabel();
            this.cboDetNetoXCobrar = new System.Windows.Forms.ComboBox();
            this.ultraLabel13 = new Infragistics.Win.Misc.UltraLabel();
            this.cboDetCorrelativoVenta = new System.Windows.Forms.ComboBox();
            this.ultraLabel14 = new Infragistics.Win.Misc.UltraLabel();
            this.btnDetalle = new Infragistics.Win.Misc.UltraButton();
            this.grdDataCabecera = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.grdDataDetalle = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraDataSource1 = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
            this.cboImportarDaTA = new Infragistics.Win.Misc.UltraButton();
            this.ultraDataSource2 = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
            this.lblEstadoMigracion = new Infragistics.Win.Misc.UltraLabel();
            this.btnEliminar = new Infragistics.Win.Misc.UltraButton();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).BeginInit();
            this.ultraGroupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdDataCabecera)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdDataDetalle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraDataSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraDataSource2)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraGroupBox1.Controls.Add(this.cboCabCorrelativoC);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel8);
            this.ultraGroupBox1.Controls.Add(this.cboCabTipoDocumentoC);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel9);
            this.ultraGroupBox1.Controls.Add(this.cboCabGlosa);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel5);
            this.ultraGroupBox1.Controls.Add(this.cboCabTipoCambio);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel6);
            this.ultraGroupBox1.Controls.Add(this.cboCabFechaReg);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel1);
            this.ultraGroupBox1.Controls.Add(this.btnCabecera);
            this.ultraGroupBox1.Location = new System.Drawing.Point(12, 12);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(1168, 52);
            this.ultraGroupBox1.TabIndex = 0;
            this.ultraGroupBox1.Text = "Cabecera";
            this.ultraGroupBox1.Click += new System.EventHandler(this.ultraGroupBox1_Click);
            // 
            // cboCabCorrelativoC
            // 
            this.cboCabCorrelativoC.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCabCorrelativoC.FormattingEnabled = true;
            this.cboCabCorrelativoC.Location = new System.Drawing.Point(436, 20);
            this.cboCabCorrelativoC.Name = "cboCabCorrelativoC";
            this.cboCabCorrelativoC.Size = new System.Drawing.Size(97, 21);
            this.cboCabCorrelativoC.TabIndex = 16;
            // 
            // ultraLabel8
            // 
            appearance1.FontData.BoldAsString = "True";
            this.ultraLabel8.Appearance = appearance1;
            this.ultraLabel8.AutoSize = true;
            this.ultraLabel8.Location = new System.Drawing.Point(343, 23);
            this.ultraLabel8.Name = "ultraLabel8";
            this.ultraLabel8.Size = new System.Drawing.Size(88, 14);
            this.ultraLabel8.TabIndex = 15;
            this.ultraLabel8.Text = "Corr. Cabecera:";
            // 
            // cboCabTipoDocumentoC
            // 
            this.cboCabTipoDocumentoC.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCabTipoDocumentoC.FormattingEnabled = true;
            this.cboCabTipoDocumentoC.Location = new System.Drawing.Point(233, 20);
            this.cboCabTipoDocumentoC.Name = "cboCabTipoDocumentoC";
            this.cboCabTipoDocumentoC.Size = new System.Drawing.Size(97, 21);
            this.cboCabTipoDocumentoC.TabIndex = 14;
            // 
            // ultraLabel9
            // 
            appearance2.FontData.BoldAsString = "True";
            this.ultraLabel9.Appearance = appearance2;
            this.ultraLabel9.AutoSize = true;
            this.ultraLabel9.Location = new System.Drawing.Point(140, 23);
            this.ultraLabel9.Name = "ultraLabel9";
            this.ultraLabel9.Size = new System.Drawing.Size(77, 14);
            this.ultraLabel9.TabIndex = 13;
            this.ultraLabel9.Text = "TD. Cabecera";
            // 
            // cboCabGlosa
            // 
            this.cboCabGlosa.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCabGlosa.FormattingEnabled = true;
            this.cboCabGlosa.Location = new System.Drawing.Point(1041, 20);
            this.cboCabGlosa.Name = "cboCabGlosa";
            this.cboCabGlosa.Size = new System.Drawing.Size(121, 21);
            this.cboCabGlosa.TabIndex = 10;
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(979, 23);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(37, 14);
            this.ultraLabel5.TabIndex = 9;
            this.ultraLabel5.Text = "Glosa:";
            // 
            // cboCabTipoCambio
            // 
            this.cboCabTipoCambio.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCabTipoCambio.FormattingEnabled = true;
            this.cboCabTipoCambio.Location = new System.Drawing.Point(835, 20);
            this.cboCabTipoCambio.Name = "cboCabTipoCambio";
            this.cboCabTipoCambio.Size = new System.Drawing.Size(121, 21);
            this.cboCabTipoCambio.TabIndex = 8;
            // 
            // ultraLabel6
            // 
            this.ultraLabel6.AutoSize = true;
            this.ultraLabel6.Location = new System.Drawing.Point(757, 23);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(72, 14);
            this.ultraLabel6.TabIndex = 7;
            this.ultraLabel6.Text = "Tipo Cambio:";
            // 
            // cboCabFechaReg
            // 
            this.cboCabFechaReg.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCabFechaReg.FormattingEnabled = true;
            this.cboCabFechaReg.Location = new System.Drawing.Point(625, 20);
            this.cboCabFechaReg.Name = "cboCabFechaReg";
            this.cboCabFechaReg.Size = new System.Drawing.Size(121, 21);
            this.cboCabFechaReg.TabIndex = 2;
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(556, 23);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(63, 14);
            this.ultraLabel1.TabIndex = 1;
            this.ultraLabel1.Text = "Fecha Reg.";
            // 
            // btnCabecera
            // 
            this.btnCabecera.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCabecera.Location = new System.Drawing.Point(6, 19);
            this.btnCabecera.Name = "btnCabecera";
            this.btnCabecera.Size = new System.Drawing.Size(128, 27);
            this.btnCabecera.TabIndex = 0;
            this.btnCabecera.Text = "Cabecera";
            this.btnCabecera.Click += new System.EventHandler(this.btnCabecera_Click);
            // 
            // ultraGroupBox2
            // 
            this.ultraGroupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraGroupBox2.Controls.Add(this.cboDetCorrelativoC);
            this.ultraGroupBox2.Controls.Add(this.ultraLabel3);
            this.ultraGroupBox2.Controls.Add(this.cboDetTipoDocumentoC);
            this.ultraGroupBox2.Controls.Add(this.ultraLabel7);
            this.ultraGroupBox2.Controls.Add(this.cboDetTDventa);
            this.ultraGroupBox2.Controls.Add(this.ultraLabel10);
            this.ultraGroupBox2.Controls.Add(this.cboDetMontoCobranza);
            this.ultraGroupBox2.Controls.Add(this.ultraLabel11);
            this.ultraGroupBox2.Controls.Add(this.cboDetNetoXCobrar);
            this.ultraGroupBox2.Controls.Add(this.ultraLabel13);
            this.ultraGroupBox2.Controls.Add(this.cboDetCorrelativoVenta);
            this.ultraGroupBox2.Controls.Add(this.ultraLabel14);
            this.ultraGroupBox2.Controls.Add(this.btnDetalle);
            this.ultraGroupBox2.Location = new System.Drawing.Point(13, 70);
            this.ultraGroupBox2.Name = "ultraGroupBox2";
            this.ultraGroupBox2.Size = new System.Drawing.Size(1168, 82);
            this.ultraGroupBox2.TabIndex = 17;
            this.ultraGroupBox2.Text = "Cabecera";
            // 
            // cboDetCorrelativoC
            // 
            this.cboDetCorrelativoC.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDetCorrelativoC.FormattingEnabled = true;
            this.cboDetCorrelativoC.Location = new System.Drawing.Point(435, 20);
            this.cboDetCorrelativoC.Name = "cboDetCorrelativoC";
            this.cboDetCorrelativoC.Size = new System.Drawing.Size(97, 21);
            this.cboDetCorrelativoC.TabIndex = 16;
            // 
            // ultraLabel3
            // 
            appearance3.FontData.BoldAsString = "True";
            this.ultraLabel3.Appearance = appearance3;
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(341, 23);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(88, 14);
            this.ultraLabel3.TabIndex = 15;
            this.ultraLabel3.Text = "Corr. Cabecera:";
            // 
            // cboDetTipoDocumentoC
            // 
            this.cboDetTipoDocumentoC.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDetTipoDocumentoC.FormattingEnabled = true;
            this.cboDetTipoDocumentoC.Location = new System.Drawing.Point(233, 20);
            this.cboDetTipoDocumentoC.Name = "cboDetTipoDocumentoC";
            this.cboDetTipoDocumentoC.Size = new System.Drawing.Size(97, 21);
            this.cboDetTipoDocumentoC.TabIndex = 14;
            // 
            // ultraLabel7
            // 
            appearance4.FontData.BoldAsString = "True";
            this.ultraLabel7.Appearance = appearance4;
            this.ultraLabel7.AutoSize = true;
            this.ultraLabel7.Location = new System.Drawing.Point(140, 23);
            this.ultraLabel7.Name = "ultraLabel7";
            this.ultraLabel7.Size = new System.Drawing.Size(77, 14);
            this.ultraLabel7.TabIndex = 13;
            this.ultraLabel7.Text = "TD Cabecera:";
            // 
            // cboDetTDventa
            // 
            this.cboDetTDventa.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDetTDventa.FormattingEnabled = true;
            this.cboDetTDventa.Location = new System.Drawing.Point(834, 20);
            this.cboDetTDventa.Name = "cboDetTDventa";
            this.cboDetTDventa.Size = new System.Drawing.Size(121, 21);
            this.cboDetTDventa.TabIndex = 12;
            // 
            // ultraLabel10
            // 
            this.ultraLabel10.AutoSize = true;
            this.ultraLabel10.Location = new System.Drawing.Point(770, 23);
            this.ultraLabel10.Name = "ultraLabel10";
            this.ultraLabel10.Size = new System.Drawing.Size(58, 14);
            this.ultraLabel10.TabIndex = 11;
            this.ultraLabel10.Text = "TD. Venta:";
            // 
            // cboDetMontoCobranza
            // 
            this.cboDetMontoCobranza.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDetMontoCobranza.FormattingEnabled = true;
            this.cboDetMontoCobranza.Location = new System.Drawing.Point(1040, 20);
            this.cboDetMontoCobranza.Name = "cboDetMontoCobranza";
            this.cboDetMontoCobranza.Size = new System.Drawing.Size(121, 21);
            this.cboDetMontoCobranza.TabIndex = 10;
            // 
            // ultraLabel11
            // 
            this.ultraLabel11.AutoSize = true;
            this.ultraLabel11.Location = new System.Drawing.Point(978, 23);
            this.ultraLabel11.Name = "ultraLabel11";
            this.ultraLabel11.Size = new System.Drawing.Size(56, 14);
            this.ultraLabel11.TabIndex = 9;
            this.ultraLabel11.Text = "Cobranza:";
            // 
            // cboDetNetoXCobrar
            // 
            this.cboDetNetoXCobrar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDetNetoXCobrar.FormattingEnabled = true;
            this.cboDetNetoXCobrar.Location = new System.Drawing.Point(233, 52);
            this.cboDetNetoXCobrar.Name = "cboDetNetoXCobrar";
            this.cboDetNetoXCobrar.Size = new System.Drawing.Size(97, 21);
            this.cboDetNetoXCobrar.TabIndex = 4;
            // 
            // ultraLabel13
            // 
            this.ultraLabel13.AutoSize = true;
            this.ultraLabel13.Location = new System.Drawing.Point(140, 55);
            this.ultraLabel13.Name = "ultraLabel13";
            this.ultraLabel13.Size = new System.Drawing.Size(79, 14);
            this.ultraLabel13.TabIndex = 3;
            this.ultraLabel13.Text = "Neto X Cobrar:";
            // 
            // cboDetCorrelativoVenta
            // 
            this.cboDetCorrelativoVenta.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDetCorrelativoVenta.FormattingEnabled = true;
            this.cboDetCorrelativoVenta.Location = new System.Drawing.Point(624, 20);
            this.cboDetCorrelativoVenta.Name = "cboDetCorrelativoVenta";
            this.cboDetCorrelativoVenta.Size = new System.Drawing.Size(121, 21);
            this.cboDetCorrelativoVenta.TabIndex = 2;
            // 
            // ultraLabel14
            // 
            this.ultraLabel14.AutoSize = true;
            this.ultraLabel14.Location = new System.Drawing.Point(555, 23);
            this.ultraLabel14.Name = "ultraLabel14";
            this.ultraLabel14.Size = new System.Drawing.Size(65, 14);
            this.ultraLabel14.TabIndex = 1;
            this.ultraLabel14.Text = "Corr. Venta:";
            // 
            // btnDetalle
            // 
            this.btnDetalle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDetalle.Location = new System.Drawing.Point(6, 19);
            this.btnDetalle.Name = "btnDetalle";
            this.btnDetalle.Size = new System.Drawing.Size(128, 56);
            this.btnDetalle.TabIndex = 0;
            this.btnDetalle.Text = "Detalle";
            this.btnDetalle.Click += new System.EventHandler(this.btnDetalle_Click);
            // 
            // grdDataCabecera
            // 
            this.grdDataCabecera.Location = new System.Drawing.Point(12, 158);
            this.grdDataCabecera.Name = "grdDataCabecera";
            this.grdDataCabecera.Size = new System.Drawing.Size(615, 328);
            this.grdDataCabecera.TabIndex = 18;
            this.grdDataCabecera.Text = "ultraGrid1";
            this.grdDataCabecera.AfterRowActivate += new System.EventHandler(this.grdDataCabecera_AfterRowActivate);
            // 
            // grdDataDetalle
            // 
            this.grdDataDetalle.Location = new System.Drawing.Point(633, 158);
            this.grdDataDetalle.Name = "grdDataDetalle";
            this.grdDataDetalle.Size = new System.Drawing.Size(547, 328);
            this.grdDataDetalle.TabIndex = 19;
            this.grdDataDetalle.Text = "ultraGrid1";
            // 
            // cboImportarDaTA
            // 
            this.cboImportarDaTA.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.cboImportarDaTA.Location = new System.Drawing.Point(1052, 489);
            this.cboImportarDaTA.Name = "cboImportarDaTA";
            this.cboImportarDaTA.Size = new System.Drawing.Size(128, 27);
            this.cboImportarDaTA.TabIndex = 20;
            this.cboImportarDaTA.Text = "Importar Data";
            this.cboImportarDaTA.Click += new System.EventHandler(this.cboImportarDaTA_Click);
            // 
            // lblEstadoMigracion
            // 
            this.lblEstadoMigracion.Location = new System.Drawing.Point(12, 495);
            this.lblEstadoMigracion.Name = "lblEstadoMigracion";
            this.lblEstadoMigracion.Size = new System.Drawing.Size(400, 23);
            this.lblEstadoMigracion.TabIndex = 21;
            this.lblEstadoMigracion.Text = "ultraLabel2";
            this.lblEstadoMigracion.Visible = false;
            // 
            // btnEliminar
            // 
            this.btnEliminar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEliminar.Location = new System.Drawing.Point(884, 489);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(163, 27);
            this.btnEliminar.TabIndex = 22;
            this.btnEliminar.Text = "Eliminar Cobranzas Por Id";
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // frmMigrarCobranzasAdministrativas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1193, 522);
            this.Controls.Add(this.btnEliminar);
            this.Controls.Add(this.lblEstadoMigracion);
            this.Controls.Add(this.cboImportarDaTA);
            this.Controls.Add(this.grdDataDetalle);
            this.Controls.Add(this.grdDataCabecera);
            this.Controls.Add(this.ultraGroupBox2);
            this.Controls.Add(this.ultraGroupBox1);
            this.Name = "frmMigrarCobranzasAdministrativas";
            this.Text = "frmMigrarCobranzasAdministrativas";
            this.Load += new System.EventHandler(this.frmMigrarCobranzasAdministrativas_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).EndInit();
            this.ultraGroupBox2.ResumeLayout(false);
            this.ultraGroupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdDataCabecera)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdDataDetalle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraDataSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraDataSource2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private System.Windows.Forms.ComboBox cboCabFechaReg;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraButton btnCabecera;
        private System.Windows.Forms.ComboBox cboCabCorrelativoC;
        private Infragistics.Win.Misc.UltraLabel ultraLabel8;
        private System.Windows.Forms.ComboBox cboCabTipoDocumentoC;
        private Infragistics.Win.Misc.UltraLabel ultraLabel9;
        private System.Windows.Forms.ComboBox cboCabGlosa;
        private Infragistics.Win.Misc.UltraLabel ultraLabel5;
        private System.Windows.Forms.ComboBox cboCabTipoCambio;
        private Infragistics.Win.Misc.UltraLabel ultraLabel6;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox2;
        private System.Windows.Forms.ComboBox cboDetCorrelativoC;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private System.Windows.Forms.ComboBox cboDetTipoDocumentoC;
        private Infragistics.Win.Misc.UltraLabel ultraLabel7;
        private System.Windows.Forms.ComboBox cboDetTDventa;
        private Infragistics.Win.Misc.UltraLabel ultraLabel10;
        private System.Windows.Forms.ComboBox cboDetMontoCobranza;
        private Infragistics.Win.Misc.UltraLabel ultraLabel11;
        private System.Windows.Forms.ComboBox cboDetNetoXCobrar;
        private Infragistics.Win.Misc.UltraLabel ultraLabel13;
        private System.Windows.Forms.ComboBox cboDetCorrelativoVenta;
        private Infragistics.Win.Misc.UltraLabel ultraLabel14;
        private Infragistics.Win.Misc.UltraButton btnDetalle;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdDataCabecera;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdDataDetalle;
        private Infragistics.Win.UltraWinDataSource.UltraDataSource ultraDataSource1;
        private Infragistics.Win.Misc.UltraButton cboImportarDaTA;
        private Infragistics.Win.UltraWinDataSource.UltraDataSource ultraDataSource2;
        private Infragistics.Win.Misc.UltraLabel lblEstadoMigracion;
        private Infragistics.Win.Misc.UltraButton btnEliminar;
    }
}