namespace SAMBHS.Windows.WinClient.UI.Mantenimientos.Migraciones
{
    partial class frmMigrarVendedores
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
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Nuevo", 0);
            this.ultraExpandableGroupBox1 = new Infragistics.Win.Misc.UltraExpandableGroupBox();
            this.ultraExpandableGroupBoxPanel1 = new Infragistics.Win.Misc.UltraExpandableGroupBoxPanel();
            this.ultraGroupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            this.ultraLabel7 = new Infragistics.Win.Misc.UltraLabel();
            this.cboAnalisis = new System.Windows.Forms.ComboBox();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.cboNaturaleza = new System.Windows.Forms.ComboBox();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.cboMoneda = new System.Windows.Forms.ComboBox();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.cboDetalle = new System.Windows.Forms.ComboBox();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.cboNivel = new System.Windows.Forms.ComboBox();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.cboNombre = new System.Windows.Forms.ComboBox();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.cboNroCuenta = new System.Windows.Forms.ComboBox();
            this.btnImportar = new Infragistics.Win.Misc.UltraButton();
            this.ultraButton1 = new Infragistics.Win.Misc.UltraButton();
            this.gpDetalle = new Infragistics.Win.Misc.UltraGroupBox();
            this.lblContadorFilas = new Infragistics.Win.Misc.UltraLabel();
            this.grdData = new Infragistics.Win.UltraWinGrid.UltraGrid();
            ((System.ComponentModel.ISupportInitialize)(this.ultraExpandableGroupBox1)).BeginInit();
            this.ultraExpandableGroupBox1.SuspendLayout();
            this.ultraExpandableGroupBoxPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).BeginInit();
            this.ultraGroupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gpDetalle)).BeginInit();
            this.gpDetalle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraExpandableGroupBox1
            // 
            this.ultraExpandableGroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ultraExpandableGroupBox1.Controls.Add(this.ultraExpandableGroupBoxPanel1);
            this.ultraExpandableGroupBox1.ExpandedSize = new System.Drawing.Size(280, 482);
            this.ultraExpandableGroupBox1.ExpansionIndicator = Infragistics.Win.Misc.GroupBoxExpansionIndicator.None;
            this.ultraExpandableGroupBox1.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.LeftOutsideBorder;
            this.ultraExpandableGroupBox1.Location = new System.Drawing.Point(12, 12);
            this.ultraExpandableGroupBox1.Name = "ultraExpandableGroupBox1";
            this.ultraExpandableGroupBox1.Size = new System.Drawing.Size(280, 482);
            this.ultraExpandableGroupBox1.TabIndex = 29;
            this.ultraExpandableGroupBox1.Text = "Opciones";
            this.ultraExpandableGroupBox1.UseAppStyling = false;
            // 
            // ultraExpandableGroupBoxPanel1
            // 
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.ultraGroupBox2);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.btnImportar);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.ultraButton1);
            this.ultraExpandableGroupBoxPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraExpandableGroupBoxPanel1.Location = new System.Drawing.Point(21, 3);
            this.ultraExpandableGroupBoxPanel1.Name = "ultraExpandableGroupBoxPanel1";
            this.ultraExpandableGroupBoxPanel1.Size = new System.Drawing.Size(256, 476);
            this.ultraExpandableGroupBoxPanel1.TabIndex = 0;
            // 
            // ultraGroupBox2
            // 
            this.ultraGroupBox2.Controls.Add(this.ultraLabel7);
            this.ultraGroupBox2.Controls.Add(this.cboAnalisis);
            this.ultraGroupBox2.Controls.Add(this.ultraLabel6);
            this.ultraGroupBox2.Controls.Add(this.cboNaturaleza);
            this.ultraGroupBox2.Controls.Add(this.ultraLabel5);
            this.ultraGroupBox2.Controls.Add(this.cboMoneda);
            this.ultraGroupBox2.Controls.Add(this.ultraLabel3);
            this.ultraGroupBox2.Controls.Add(this.cboDetalle);
            this.ultraGroupBox2.Controls.Add(this.ultraLabel4);
            this.ultraGroupBox2.Controls.Add(this.cboNivel);
            this.ultraGroupBox2.Controls.Add(this.ultraLabel2);
            this.ultraGroupBox2.Controls.Add(this.cboNombre);
            this.ultraGroupBox2.Controls.Add(this.ultraLabel1);
            this.ultraGroupBox2.Controls.Add(this.cboNroCuenta);
            this.ultraGroupBox2.Location = new System.Drawing.Point(3, 3);
            this.ultraGroupBox2.Name = "ultraGroupBox2";
            this.ultraGroupBox2.Size = new System.Drawing.Size(250, 288);
            this.ultraGroupBox2.TabIndex = 12;
            this.ultraGroupBox2.Text = "Columnas Equivalentes";
            this.ultraGroupBox2.UseAppStyling = false;
            // 
            // ultraLabel7
            // 
            this.ultraLabel7.AutoSize = true;
            this.ultraLabel7.Location = new System.Drawing.Point(6, 180);
            this.ultraLabel7.Name = "ultraLabel7";
            this.ultraLabel7.Size = new System.Drawing.Size(47, 14);
            this.ultraLabel7.TabIndex = 26;
            this.ultraLabel7.Text = "Análisis:";
            // 
            // cboAnalisis
            // 
            this.cboAnalisis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAnalisis.FormattingEnabled = true;
            this.cboAnalisis.Items.AddRange(new object[] {
            "--Seleccionar--"});
            this.cboAnalisis.Location = new System.Drawing.Point(89, 177);
            this.cboAnalisis.Name = "cboAnalisis";
            this.cboAnalisis.Size = new System.Drawing.Size(154, 21);
            this.cboAnalisis.TabIndex = 25;
            // 
            // ultraLabel6
            // 
            this.ultraLabel6.AutoSize = true;
            this.ultraLabel6.Location = new System.Drawing.Point(6, 256);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(62, 14);
            this.ultraLabel6.TabIndex = 24;
            this.ultraLabel6.Text = "Naturaleza:";
            // 
            // cboNaturaleza
            // 
            this.cboNaturaleza.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNaturaleza.FormattingEnabled = true;
            this.cboNaturaleza.Items.AddRange(new object[] {
            "--Seleccionar--"});
            this.cboNaturaleza.Location = new System.Drawing.Point(89, 253);
            this.cboNaturaleza.Name = "cboNaturaleza";
            this.cboNaturaleza.Size = new System.Drawing.Size(154, 21);
            this.cboNaturaleza.TabIndex = 23;
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(6, 218);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(48, 14);
            this.ultraLabel5.TabIndex = 22;
            this.ultraLabel5.Text = "Moneda:";
            // 
            // cboMoneda
            // 
            this.cboMoneda.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMoneda.FormattingEnabled = true;
            this.cboMoneda.Items.AddRange(new object[] {
            "--Seleccionar--"});
            this.cboMoneda.Location = new System.Drawing.Point(89, 215);
            this.cboMoneda.Name = "cboMoneda";
            this.cboMoneda.Size = new System.Drawing.Size(154, 21);
            this.cboMoneda.TabIndex = 21;
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(6, 145);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(43, 14);
            this.ultraLabel3.TabIndex = 20;
            this.ultraLabel3.Text = "Detalle:";
            // 
            // cboDetalle
            // 
            this.cboDetalle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDetalle.FormattingEnabled = true;
            this.cboDetalle.Items.AddRange(new object[] {
            "--Seleccionar--"});
            this.cboDetalle.Location = new System.Drawing.Point(89, 142);
            this.cboDetalle.Name = "cboDetalle";
            this.cboDetalle.Size = new System.Drawing.Size(154, 21);
            this.cboDetalle.TabIndex = 19;
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(6, 109);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(33, 14);
            this.ultraLabel4.TabIndex = 18;
            this.ultraLabel4.Text = "Nivel:";
            // 
            // cboNivel
            // 
            this.cboNivel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNivel.FormattingEnabled = true;
            this.cboNivel.Items.AddRange(new object[] {
            "--Seleccionar--"});
            this.cboNivel.Location = new System.Drawing.Point(89, 106);
            this.cboNivel.Name = "cboNivel";
            this.cboNivel.Size = new System.Drawing.Size(154, 21);
            this.cboNivel.TabIndex = 17;
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(6, 72);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(48, 14);
            this.ultraLabel2.TabIndex = 16;
            this.ultraLabel2.Text = "Nombre:";
            // 
            // cboNombre
            // 
            this.cboNombre.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNombre.FormattingEnabled = true;
            this.cboNombre.Items.AddRange(new object[] {
            "--Seleccionar--"});
            this.cboNombre.Location = new System.Drawing.Point(89, 69);
            this.cboNombre.Name = "cboNombre";
            this.cboNombre.Size = new System.Drawing.Size(154, 21);
            this.cboNombre.TabIndex = 15;
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(6, 33);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(83, 14);
            this.ultraLabel1.TabIndex = 14;
            this.ultraLabel1.Text = "Cod. Vendedor:";
            // 
            // cboNroCuenta
            // 
            this.cboNroCuenta.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNroCuenta.FormattingEnabled = true;
            this.cboNroCuenta.Items.AddRange(new object[] {
            "--Seleccionar--"});
            this.cboNroCuenta.Location = new System.Drawing.Point(89, 30);
            this.cboNroCuenta.Name = "cboNroCuenta";
            this.cboNroCuenta.Size = new System.Drawing.Size(154, 21);
            this.cboNroCuenta.TabIndex = 13;
            // 
            // btnImportar
            // 
            appearance1.Image = global::SAMBHS.Windows.WinClient.UI.Resource.page_excel;
            this.btnImportar.Appearance = appearance1;
            this.btnImportar.Location = new System.Drawing.Point(33, 403);
            this.btnImportar.Name = "btnImportar";
            this.btnImportar.Size = new System.Drawing.Size(190, 29);
            this.btnImportar.TabIndex = 9;
            this.btnImportar.Text = "Importar Excel";
            // 
            // ultraButton1
            // 
            appearance2.Image = global::SAMBHS.Windows.WinClient.UI.Resource.database_save1;
            this.ultraButton1.Appearance = appearance2;
            this.ultraButton1.Location = new System.Drawing.Point(33, 438);
            this.ultraButton1.Name = "ultraButton1";
            this.ultraButton1.Size = new System.Drawing.Size(190, 29);
            this.ultraButton1.TabIndex = 13;
            this.ultraButton1.Text = "Guardar y Procesar";
            // 
            // gpDetalle
            // 
            this.gpDetalle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gpDetalle.Controls.Add(this.lblContadorFilas);
            this.gpDetalle.Controls.Add(this.grdData);
            this.gpDetalle.Location = new System.Drawing.Point(298, 12);
            this.gpDetalle.Name = "gpDetalle";
            this.gpDetalle.Size = new System.Drawing.Size(730, 482);
            this.gpDetalle.TabIndex = 28;
            this.gpDetalle.Text = "Importación desde Excel";
            this.gpDetalle.UseAppStyling = false;
            // 
            // lblContadorFilas
            // 
            this.lblContadorFilas.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblContadorFilas.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.2F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContadorFilas.Location = new System.Drawing.Point(498, 13);
            this.lblContadorFilas.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblContadorFilas.Name = "lblContadorFilas";
            this.lblContadorFilas.Size = new System.Drawing.Size(226, 19);
            this.lblContadorFilas.TabIndex = 144;
            this.lblContadorFilas.Text = "No se ha realizado la importación aún.";
            this.lblContadorFilas.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;
            this.lblContadorFilas.Appearance.TextVAlign = Infragistics.Win.VAlign.Middle;
            // 
            // grdData
            // 
            this.grdData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            ultraGridColumn2.Header.VisiblePosition = 0;
            ultraGridColumn2.Hidden = true;
            ultraGridColumn2.Width = 61;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn2});
            this.grdData.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdData.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdData.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdData.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.grdData.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.grdData.DisplayLayout.Override.ColumnAutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.AllRowsInBand;
            this.grdData.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.grdData.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.grdData.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.grdData.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdData.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdData.DisplayLayout.TabNavigation = Infragistics.Win.UltraWinGrid.TabNavigation.NextControl;
            this.grdData.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.grdData.Location = new System.Drawing.Point(6, 35);
            this.grdData.Name = "grdData";
            this.grdData.Size = new System.Drawing.Size(718, 441);
            this.grdData.TabIndex = 8;
            // 
            // frmMigrarVendedores
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1040, 506);
            this.Controls.Add(this.ultraExpandableGroupBox1);
            this.Controls.Add(this.gpDetalle);
            this.Name = "frmMigrarVendedores";
            this.Text = "frmMigrarVendedores";
            ((System.ComponentModel.ISupportInitialize)(this.ultraExpandableGroupBox1)).EndInit();
            this.ultraExpandableGroupBox1.ResumeLayout(false);
            this.ultraExpandableGroupBoxPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).EndInit();
            this.ultraGroupBox2.ResumeLayout(false);
            this.ultraGroupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gpDetalle)).EndInit();
            this.gpDetalle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraExpandableGroupBox ultraExpandableGroupBox1;
        private Infragistics.Win.Misc.UltraExpandableGroupBoxPanel ultraExpandableGroupBoxPanel1;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox2;
        private Infragistics.Win.Misc.UltraLabel ultraLabel7;
        private System.Windows.Forms.ComboBox cboAnalisis;
        private Infragistics.Win.Misc.UltraLabel ultraLabel6;
        private System.Windows.Forms.ComboBox cboNaturaleza;
        private Infragistics.Win.Misc.UltraLabel ultraLabel5;
        private System.Windows.Forms.ComboBox cboMoneda;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private System.Windows.Forms.ComboBox cboDetalle;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private System.Windows.Forms.ComboBox cboNivel;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private System.Windows.Forms.ComboBox cboNombre;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private System.Windows.Forms.ComboBox cboNroCuenta;
        private Infragistics.Win.Misc.UltraButton btnImportar;
        private Infragistics.Win.Misc.UltraButton ultraButton1;
        private Infragistics.Win.Misc.UltraGroupBox gpDetalle;
        private Infragistics.Win.Misc.UltraLabel lblContadorFilas;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdData;
    }
}