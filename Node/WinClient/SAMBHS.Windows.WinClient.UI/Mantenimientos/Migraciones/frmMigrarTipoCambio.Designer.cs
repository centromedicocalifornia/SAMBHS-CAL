namespace SAMBHS.Windows.WinClient.UI.Mantenimientos.Migraciones
{
    partial class frmMigrarTipoCambio
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
            Infragistics.Win.UltraWinToolTip.UltraToolTipInfo ultraToolTipInfo1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipInfo("Convierte los valores con fecha de excel a fecha estándar", Infragistics.Win.ToolTipImage.Info, "Información", Infragistics.Win.DefaultableBoolean.Default);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Nuevo", 0);
            this.ultraExpandableGroupBox1 = new Infragistics.Win.Misc.UltraExpandableGroupBox();
            this.ultraExpandableGroupBoxPanel1 = new Infragistics.Win.Misc.UltraExpandableGroupBoxPanel();
            this.ultraGroupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            this.ultraButton2 = new Infragistics.Win.Misc.UltraButton();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.cboValorVenta = new System.Windows.Forms.ComboBox();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.cboValorCompra = new System.Windows.Forms.ComboBox();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.cboFecha = new System.Windows.Forms.ComboBox();
            this.btnImportar = new Infragistics.Win.Misc.UltraButton();
            this.ultraButton1 = new Infragistics.Win.Misc.UltraButton();
            this.gpDetalle = new Infragistics.Win.Misc.UltraGroupBox();
            this.lblContadorFilas = new Infragistics.Win.Misc.UltraLabel();
            this.grdData = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraDataSource1 = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
            this.uvDatos = new Infragistics.Win.Misc.UltraValidator(this.components);
            this.ultraToolTipManager1 = new Infragistics.Win.UltraWinToolTip.UltraToolTipManager(this.components);
            this.frmMigrarTipoCambio_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            ((System.ComponentModel.ISupportInitialize)(this.ultraExpandableGroupBox1)).BeginInit();
            this.ultraExpandableGroupBox1.SuspendLayout();
            this.ultraExpandableGroupBoxPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).BeginInit();
            this.ultraGroupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gpDetalle)).BeginInit();
            this.gpDetalle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraDataSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uvDatos)).BeginInit();
            this.frmMigrarTipoCambio_Fill_Panel.ClientArea.SuspendLayout();
            this.frmMigrarTipoCambio_Fill_Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraExpandableGroupBox1
            // 
            this.ultraExpandableGroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ultraExpandableGroupBox1.Controls.Add(this.ultraExpandableGroupBoxPanel1);
            this.ultraExpandableGroupBox1.ExpandedSize = new System.Drawing.Size(280, 499);
            this.ultraExpandableGroupBox1.ExpansionIndicator = Infragistics.Win.Misc.GroupBoxExpansionIndicator.None;
            this.ultraExpandableGroupBox1.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.LeftOutsideBorder;
            this.ultraExpandableGroupBox1.Location = new System.Drawing.Point(3, 7);
            this.ultraExpandableGroupBox1.Name = "ultraExpandableGroupBox1";
            this.ultraExpandableGroupBox1.Size = new System.Drawing.Size(280, 499);
            this.ultraExpandableGroupBox1.TabIndex = 25;
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
            this.ultraExpandableGroupBoxPanel1.Size = new System.Drawing.Size(256, 493);
            this.ultraExpandableGroupBoxPanel1.TabIndex = 0;
            // 
            // ultraGroupBox2
            // 
            this.ultraGroupBox2.Controls.Add(this.ultraButton2);
            this.ultraGroupBox2.Controls.Add(this.ultraLabel4);
            this.ultraGroupBox2.Controls.Add(this.cboValorVenta);
            this.ultraGroupBox2.Controls.Add(this.ultraLabel2);
            this.ultraGroupBox2.Controls.Add(this.cboValorCompra);
            this.ultraGroupBox2.Controls.Add(this.ultraLabel1);
            this.ultraGroupBox2.Controls.Add(this.cboFecha);
            this.ultraGroupBox2.Location = new System.Drawing.Point(3, 3);
            this.ultraGroupBox2.Name = "ultraGroupBox2";
            this.ultraGroupBox2.Size = new System.Drawing.Size(250, 142);
            this.ultraGroupBox2.TabIndex = 12;
            this.ultraGroupBox2.Text = "Columnas Equivalentes";
            this.ultraGroupBox2.UseAppStyling = false;
            // 
            // ultraButton2
            // 
            appearance1.Image = global::SAMBHS.Windows.WinClient.UI.Resource.table_refresh;
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Center;
            this.ultraButton2.Appearance = appearance1;
            this.ultraButton2.ButtonStyle = Infragistics.Win.UIElementButtonStyle.WindowsVistaButton;
            this.ultraButton2.Location = new System.Drawing.Point(210, 28);
            this.ultraButton2.Name = "ultraButton2";
            this.ultraButton2.Size = new System.Drawing.Size(33, 25);
            this.ultraButton2.TabIndex = 14;
            ultraToolTipInfo1.ToolTipImage = Infragistics.Win.ToolTipImage.Info;
            ultraToolTipInfo1.ToolTipText = "Convierte los valores con fecha de excel a fecha estándar";
            ultraToolTipInfo1.ToolTipTitle = "Información";
            this.ultraToolTipManager1.SetUltraToolTip(this.ultraButton2, ultraToolTipInfo1);
            this.ultraButton2.Click += new System.EventHandler(this.ultraButton2_Click);
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(6, 109);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(66, 14);
            this.ultraLabel4.TabIndex = 18;
            this.ultraLabel4.Text = "Valor Venta:";
            // 
            // cboValorVenta
            // 
            this.cboValorVenta.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboValorVenta.FormattingEnabled = true;
            this.cboValorVenta.Items.AddRange(new object[] {
            "--Seleccionar--"});
            this.cboValorVenta.Location = new System.Drawing.Point(89, 106);
            this.cboValorVenta.Name = "cboValorVenta";
            this.cboValorVenta.Size = new System.Drawing.Size(154, 21);
            this.cboValorVenta.TabIndex = 17;
            this.uvDatos.GetValidationSettings(this.cboValorVenta).Condition = new Infragistics.Win.OperatorCondition(Infragistics.Win.ConditionOperator.NotEquals, "--Seleccionar--", true, typeof(string));
            this.uvDatos.GetValidationSettings(this.cboValorVenta).IsRequired = true;
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(6, 72);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(77, 14);
            this.ultraLabel2.TabIndex = 16;
            this.ultraLabel2.Text = "Valor Compra:";
            // 
            // cboValorCompra
            // 
            this.cboValorCompra.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboValorCompra.FormattingEnabled = true;
            this.cboValorCompra.Items.AddRange(new object[] {
            "--Seleccionar--"});
            this.cboValorCompra.Location = new System.Drawing.Point(89, 69);
            this.cboValorCompra.Name = "cboValorCompra";
            this.cboValorCompra.Size = new System.Drawing.Size(154, 21);
            this.cboValorCompra.TabIndex = 15;
            this.uvDatos.GetValidationSettings(this.cboValorCompra).Condition = new Infragistics.Win.OperatorCondition(Infragistics.Win.ConditionOperator.NotEquals, "--Seleccionar--", true, typeof(string));
            this.uvDatos.GetValidationSettings(this.cboValorCompra).IsRequired = true;
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(6, 33);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(39, 14);
            this.ultraLabel1.TabIndex = 14;
            this.ultraLabel1.Text = "Fecha:";
            // 
            // cboFecha
            // 
            this.cboFecha.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFecha.FormattingEnabled = true;
            this.cboFecha.Items.AddRange(new object[] {
            "--Seleccionar--"});
            this.cboFecha.Location = new System.Drawing.Point(89, 30);
            this.cboFecha.Name = "cboFecha";
            this.cboFecha.Size = new System.Drawing.Size(115, 21);
            this.cboFecha.TabIndex = 13;
            this.uvDatos.GetValidationSettings(this.cboFecha).Condition = new Infragistics.Win.OperatorCondition(Infragistics.Win.ConditionOperator.NotEquals, "--Seleccionar--", true, typeof(string));
            this.uvDatos.GetValidationSettings(this.cboFecha).IsRequired = true;
            // 
            // btnImportar
            // 
            appearance2.Image = global::SAMBHS.Windows.WinClient.UI.Resource.page_excel;
            this.btnImportar.Appearance = appearance2;
            this.btnImportar.Location = new System.Drawing.Point(33, 403);
            this.btnImportar.Name = "btnImportar";
            this.btnImportar.Size = new System.Drawing.Size(190, 29);
            this.btnImportar.TabIndex = 9;
            this.btnImportar.Text = "Importar Excel";
            this.btnImportar.Click += new System.EventHandler(this.btnImportar_Click);
            // 
            // ultraButton1
            // 
            appearance3.Image = global::SAMBHS.Windows.WinClient.UI.Resource.database_save1;
            this.ultraButton1.Appearance = appearance3;
            this.ultraButton1.Location = new System.Drawing.Point(33, 438);
            this.ultraButton1.Name = "ultraButton1";
            this.ultraButton1.Size = new System.Drawing.Size(190, 29);
            this.ultraButton1.TabIndex = 13;
            this.ultraButton1.Text = "Guardar y Procesar";
            this.ultraButton1.Click += new System.EventHandler(this.ultraButton1_Click);
            // 
            // gpDetalle
            // 
            this.gpDetalle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gpDetalle.Controls.Add(this.lblContadorFilas);
            this.gpDetalle.Controls.Add(this.grdData);
            this.gpDetalle.Location = new System.Drawing.Point(289, 7);
            this.gpDetalle.Name = "gpDetalle";
            this.gpDetalle.Size = new System.Drawing.Size(826, 499);
            this.gpDetalle.TabIndex = 24;
            this.gpDetalle.Text = "Importación desde Excel";
            this.gpDetalle.UseAppStyling = false;
            // 
            // lblContadorFilas
            // 
            this.lblContadorFilas.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblContadorFilas.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.2F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContadorFilas.Location = new System.Drawing.Point(594, 13);
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
            this.grdData.Size = new System.Drawing.Size(814, 458);
            this.grdData.TabIndex = 8;
            // 
            // ultraToolTipManager1
            // 
            this.ultraToolTipManager1.ContainingControl = this;
            // 
            // frmMigrarTipoCambio_Fill_Panel
            // 
            // 
            // frmMigrarTipoCambio_Fill_Panel.ClientArea
            // 
            this.frmMigrarTipoCambio_Fill_Panel.ClientArea.Controls.Add(this.ultraExpandableGroupBox1);
            this.frmMigrarTipoCambio_Fill_Panel.ClientArea.Controls.Add(this.gpDetalle);
            this.frmMigrarTipoCambio_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.frmMigrarTipoCambio_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmMigrarTipoCambio_Fill_Panel.Location = new System.Drawing.Point(0, 0);
            this.frmMigrarTipoCambio_Fill_Panel.Name = "frmMigrarTipoCambio_Fill_Panel";
            this.frmMigrarTipoCambio_Fill_Panel.Size = new System.Drawing.Size(1127, 513);
            this.frmMigrarTipoCambio_Fill_Panel.TabIndex = 0;
            // 
            // frmMigrarTipoCambio
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1127, 513);
            this.Controls.Add(this.frmMigrarTipoCambio_Fill_Panel);
            this.Name = "frmMigrarTipoCambio";
            this.Text = "Migrar Tipo de Cambio";
            this.Load += new System.EventHandler(this.frmMigrarTipoCambio_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ultraExpandableGroupBox1)).EndInit();
            this.ultraExpandableGroupBox1.ResumeLayout(false);
            this.ultraExpandableGroupBoxPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).EndInit();
            this.ultraGroupBox2.ResumeLayout(false);
            this.ultraGroupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gpDetalle)).EndInit();
            this.gpDetalle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraDataSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uvDatos)).EndInit();
            this.frmMigrarTipoCambio_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmMigrarTipoCambio_Fill_Panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraExpandableGroupBox ultraExpandableGroupBox1;
        private Infragistics.Win.Misc.UltraExpandableGroupBoxPanel ultraExpandableGroupBoxPanel1;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox2;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private System.Windows.Forms.ComboBox cboValorVenta;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private System.Windows.Forms.ComboBox cboValorCompra;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private System.Windows.Forms.ComboBox cboFecha;
        private Infragistics.Win.Misc.UltraButton btnImportar;
        private Infragistics.Win.Misc.UltraButton ultraButton1;
        private Infragistics.Win.Misc.UltraGroupBox gpDetalle;
        private Infragistics.Win.Misc.UltraLabel lblContadorFilas;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdData;
        private Infragistics.Win.UltraWinDataSource.UltraDataSource ultraDataSource1;
        private Infragistics.Win.Misc.UltraValidator uvDatos;
        private Infragistics.Win.Misc.UltraButton ultraButton2;
        private Infragistics.Win.UltraWinToolTip.UltraToolTipManager ultraToolTipManager1;
        private Infragistics.Win.Misc.UltraPanel frmMigrarTipoCambio_Fill_Panel;
    }
}