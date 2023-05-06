namespace SAMBHS.Windows.WinClient.UI.Mantenimientos.Migraciones
{
    partial class frmMigrarPlanCuentas
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
            Infragistics.Win.Misc.UltraExpandableGroupBox ultraExpandableGroupBox1;
            Infragistics.Win.Misc.UltraExpandableGroupBoxPanel ultraExpandableGroupBoxPanel1;
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Misc.UltraPanel ultraPanel1;
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Nuevo", 0);
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbDestinos = new System.Windows.Forms.RadioButton();
            this.rbPlanContable = new System.Windows.Forms.RadioButton();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.ultraLabel8 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel9 = new Infragistics.Win.Misc.UltraLabel();
            this.cboCuentaDestino = new System.Windows.Forms.ComboBox();
            this.cboPorcentaje = new System.Windows.Forms.ComboBox();
            this.ultraLabel10 = new Infragistics.Win.Misc.UltraLabel();
            this.cboCuentaOrigen = new System.Windows.Forms.ComboBox();
            this.cboCuentaTransferencia = new System.Windows.Forms.ComboBox();
            this.ultraLabel11 = new Infragistics.Win.Misc.UltraLabel();
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
            this.ultraDataSource1 = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
            this.uvDatos = new Infragistics.Win.Misc.UltraValidator(this.components);
            this.ultraPanel2 = new Infragistics.Win.Misc.UltraPanel();
            ultraExpandableGroupBox1 = new Infragistics.Win.Misc.UltraExpandableGroupBox();
            ultraExpandableGroupBoxPanel1 = new Infragistics.Win.Misc.UltraExpandableGroupBoxPanel();
            ultraPanel1 = new Infragistics.Win.Misc.UltraPanel();
            ((System.ComponentModel.ISupportInitialize)(ultraExpandableGroupBox1)).BeginInit();
            ultraExpandableGroupBox1.SuspendLayout();
            ultraExpandableGroupBoxPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).BeginInit();
            this.ultraGroupBox2.SuspendLayout();
            ultraPanel1.ClientArea.SuspendLayout();
            ultraPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gpDetalle)).BeginInit();
            this.gpDetalle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraDataSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uvDatos)).BeginInit();
            this.ultraPanel2.ClientArea.SuspendLayout();
            this.ultraPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraExpandableGroupBox1
            // 
            ultraExpandableGroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            ultraExpandableGroupBox1.Controls.Add(ultraExpandableGroupBoxPanel1);
            ultraExpandableGroupBox1.ExpandedSize = new System.Drawing.Size(295, 542);
            ultraExpandableGroupBox1.ExpansionIndicator = Infragistics.Win.Misc.GroupBoxExpansionIndicator.None;
            ultraExpandableGroupBox1.HeaderPosition = Infragistics.Win.Misc.GroupBoxHeaderPosition.LeftOutsideBorder;
            ultraExpandableGroupBox1.Location = new System.Drawing.Point(3, 6);
            ultraExpandableGroupBox1.Name = "ultraExpandableGroupBox1";
            ultraExpandableGroupBox1.Size = new System.Drawing.Size(295, 542);
            ultraExpandableGroupBox1.TabIndex = 27;
            ultraExpandableGroupBox1.Text = "Opciones";
            ultraExpandableGroupBox1.UseAppStyling = false;
            // 
            // ultraExpandableGroupBoxPanel1
            // 
            ultraExpandableGroupBoxPanel1.Controls.Add(this.panel1);
            ultraExpandableGroupBoxPanel1.Controls.Add(this.ultraGroupBox1);
            ultraExpandableGroupBoxPanel1.Controls.Add(this.ultraGroupBox2);
            ultraExpandableGroupBoxPanel1.Controls.Add(this.btnImportar);
            ultraExpandableGroupBoxPanel1.Controls.Add(this.ultraButton1);
            ultraExpandableGroupBoxPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            ultraExpandableGroupBoxPanel1.Location = new System.Drawing.Point(21, 3);
            ultraExpandableGroupBoxPanel1.Name = "ultraExpandableGroupBoxPanel1";
            ultraExpandableGroupBoxPanel1.Size = new System.Drawing.Size(271, 536);
            ultraExpandableGroupBoxPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Gainsboro;
            this.panel1.Controls.Add(this.rbDestinos);
            this.panel1.Controls.Add(this.rbPlanContable);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(250, 38);
            this.panel1.TabIndex = 28;
            // 
            // rbDestinos
            // 
            this.rbDestinos.AutoSize = true;
            this.rbDestinos.Location = new System.Drawing.Point(154, 10);
            this.rbDestinos.Name = "rbDestinos";
            this.rbDestinos.Size = new System.Drawing.Size(66, 17);
            this.rbDestinos.TabIndex = 15;
            this.rbDestinos.TabStop = true;
            this.rbDestinos.Text = "Destinos";
            this.rbDestinos.UseVisualStyleBackColor = true;
            this.rbDestinos.CheckedChanged += new System.EventHandler(this.rbDestinos_CheckedChanged);
            // 
            // rbPlanContable
            // 
            this.rbPlanContable.AutoSize = true;
            this.rbPlanContable.Location = new System.Drawing.Point(31, 10);
            this.rbPlanContable.Name = "rbPlanContable";
            this.rbPlanContable.Size = new System.Drawing.Size(91, 17);
            this.rbPlanContable.TabIndex = 14;
            this.rbPlanContable.TabStop = true;
            this.rbPlanContable.Text = "Plan Contable";
            this.rbPlanContable.UseVisualStyleBackColor = true;
            this.rbPlanContable.CheckedChanged += new System.EventHandler(this.rbPlanContable_CheckedChanged);
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Controls.Add(this.ultraLabel8);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel9);
            this.ultraGroupBox1.Controls.Add(this.cboCuentaDestino);
            this.ultraGroupBox1.Controls.Add(this.cboPorcentaje);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel10);
            this.ultraGroupBox1.Controls.Add(this.cboCuentaOrigen);
            this.ultraGroupBox1.Controls.Add(this.cboCuentaTransferencia);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel11);
            this.ultraGroupBox1.Location = new System.Drawing.Point(3, 305);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(250, 150);
            this.ultraGroupBox1.TabIndex = 27;
            this.ultraGroupBox1.Text = "Columnas Equivalentes";
            this.ultraGroupBox1.UseAppStyling = false;
            // 
            // ultraLabel8
            // 
            this.ultraLabel8.AutoSize = true;
            this.ultraLabel8.Location = new System.Drawing.Point(12, 57);
            this.ultraLabel8.Name = "ultraLabel8";
            this.ultraLabel8.Size = new System.Drawing.Size(55, 14);
            this.ultraLabel8.TabIndex = 34;
            this.ultraLabel8.Text = "Cuenta D:";
            // 
            // ultraLabel9
            // 
            this.ultraLabel9.AutoSize = true;
            this.ultraLabel9.Location = new System.Drawing.Point(12, 121);
            this.ultraLabel9.Name = "ultraLabel9";
            this.ultraLabel9.Size = new System.Drawing.Size(62, 14);
            this.ultraLabel9.TabIndex = 32;
            this.ultraLabel9.Text = "Porcentaje:";
            // 
            // cboCuentaDestino
            // 
            this.cboCuentaDestino.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCuentaDestino.FormattingEnabled = true;
            this.cboCuentaDestino.Items.AddRange(new object[] {
            "--Seleccionar--"});
            this.cboCuentaDestino.Location = new System.Drawing.Point(89, 54);
            this.cboCuentaDestino.Name = "cboCuentaDestino";
            this.cboCuentaDestino.Size = new System.Drawing.Size(154, 21);
            this.cboCuentaDestino.TabIndex = 33;
            // 
            // cboPorcentaje
            // 
            this.cboPorcentaje.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPorcentaje.FormattingEnabled = true;
            this.cboPorcentaje.Items.AddRange(new object[] {
            "--Seleccionar--"});
            this.cboPorcentaje.Location = new System.Drawing.Point(89, 118);
            this.cboPorcentaje.Name = "cboPorcentaje";
            this.cboPorcentaje.Size = new System.Drawing.Size(154, 21);
            this.cboPorcentaje.TabIndex = 31;
            // 
            // ultraLabel10
            // 
            this.ultraLabel10.AutoSize = true;
            this.ultraLabel10.Location = new System.Drawing.Point(12, 89);
            this.ultraLabel10.Name = "ultraLabel10";
            this.ultraLabel10.Size = new System.Drawing.Size(54, 14);
            this.ultraLabel10.TabIndex = 30;
            this.ultraLabel10.Text = "Cuenta T:";
            // 
            // cboCuentaOrigen
            // 
            this.cboCuentaOrigen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCuentaOrigen.FormattingEnabled = true;
            this.cboCuentaOrigen.Items.AddRange(new object[] {
            "--Seleccionar--"});
            this.cboCuentaOrigen.Location = new System.Drawing.Point(89, 25);
            this.cboCuentaOrigen.Name = "cboCuentaOrigen";
            this.cboCuentaOrigen.Size = new System.Drawing.Size(154, 21);
            this.cboCuentaOrigen.TabIndex = 27;
            // 
            // cboCuentaTransferencia
            // 
            this.cboCuentaTransferencia.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCuentaTransferencia.FormattingEnabled = true;
            this.cboCuentaTransferencia.Items.AddRange(new object[] {
            "--Seleccionar--"});
            this.cboCuentaTransferencia.Location = new System.Drawing.Point(89, 86);
            this.cboCuentaTransferencia.Name = "cboCuentaTransferencia";
            this.cboCuentaTransferencia.Size = new System.Drawing.Size(154, 21);
            this.cboCuentaTransferencia.TabIndex = 29;
            // 
            // ultraLabel11
            // 
            this.ultraLabel11.AutoSize = true;
            this.ultraLabel11.Location = new System.Drawing.Point(12, 28);
            this.ultraLabel11.Name = "ultraLabel11";
            this.ultraLabel11.Size = new System.Drawing.Size(59, 14);
            this.ultraLabel11.TabIndex = 28;
            this.ultraLabel11.Text = "Cuenta O.:";
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
            this.ultraGroupBox2.Location = new System.Drawing.Point(3, 47);
            this.ultraGroupBox2.Name = "ultraGroupBox2";
            this.ultraGroupBox2.Size = new System.Drawing.Size(250, 252);
            this.ultraGroupBox2.TabIndex = 12;
            this.ultraGroupBox2.Text = "Columnas Equivalentes";
            this.ultraGroupBox2.UseAppStyling = false;
            // 
            // ultraLabel7
            // 
            this.ultraLabel7.AutoSize = true;
            this.ultraLabel7.Location = new System.Drawing.Point(6, 157);
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
            this.cboAnalisis.Location = new System.Drawing.Point(89, 154);
            this.cboAnalisis.Name = "cboAnalisis";
            this.cboAnalisis.Size = new System.Drawing.Size(154, 21);
            this.cboAnalisis.TabIndex = 25;
            // 
            // ultraLabel6
            // 
            this.ultraLabel6.AutoSize = true;
            this.ultraLabel6.Location = new System.Drawing.Point(6, 221);
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
            this.cboNaturaleza.Location = new System.Drawing.Point(89, 218);
            this.cboNaturaleza.Name = "cboNaturaleza";
            this.cboNaturaleza.Size = new System.Drawing.Size(154, 21);
            this.cboNaturaleza.TabIndex = 23;
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.Location = new System.Drawing.Point(6, 189);
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
            this.cboMoneda.Location = new System.Drawing.Point(89, 186);
            this.cboMoneda.Name = "cboMoneda";
            this.cboMoneda.Size = new System.Drawing.Size(154, 21);
            this.cboMoneda.TabIndex = 21;
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.Location = new System.Drawing.Point(6, 128);
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
            this.cboDetalle.Location = new System.Drawing.Point(89, 125);
            this.cboDetalle.Name = "cboDetalle";
            this.cboDetalle.Size = new System.Drawing.Size(154, 21);
            this.cboDetalle.TabIndex = 19;
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.Location = new System.Drawing.Point(6, 97);
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
            this.cboNivel.Location = new System.Drawing.Point(89, 94);
            this.cboNivel.Name = "cboNivel";
            this.cboNivel.Size = new System.Drawing.Size(154, 21);
            this.cboNivel.TabIndex = 17;
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(6, 66);
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
            this.cboNombre.Location = new System.Drawing.Point(89, 63);
            this.cboNombre.Name = "cboNombre";
            this.cboNombre.Size = new System.Drawing.Size(154, 21);
            this.cboNombre.TabIndex = 15;
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(6, 33);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(68, 14);
            this.ultraLabel1.TabIndex = 14;
            this.ultraLabel1.Text = "Nro. Cuenta:";
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
            this.btnImportar.Location = new System.Drawing.Point(34, 461);
            this.btnImportar.Name = "btnImportar";
            this.btnImportar.Size = new System.Drawing.Size(190, 29);
            this.btnImportar.TabIndex = 9;
            this.btnImportar.Text = "Importar Excel";
            this.btnImportar.Click += new System.EventHandler(this.btnImportar_Click);
            // 
            // ultraButton1
            // 
            appearance2.Image = global::SAMBHS.Windows.WinClient.UI.Resource.database_save1;
            this.ultraButton1.Appearance = appearance2;
            this.ultraButton1.Location = new System.Drawing.Point(34, 492);
            this.ultraButton1.Name = "ultraButton1";
            this.ultraButton1.Size = new System.Drawing.Size(190, 29);
            this.ultraButton1.TabIndex = 13;
            this.ultraButton1.Text = "Guardar y Procesar";
            this.ultraButton1.Click += new System.EventHandler(this.ultraButton1_Click);
            // 
            // ultraPanel1
            // 
            ultraPanel1.AutoScroll = true;
            // 
            // ultraPanel1.ClientArea
            // 
            ultraPanel1.ClientArea.Controls.Add(ultraExpandableGroupBox1);
            ultraPanel1.Dock = System.Windows.Forms.DockStyle.Left;
            ultraPanel1.Location = new System.Drawing.Point(0, 0);
            ultraPanel1.Name = "ultraPanel1";
            ultraPanel1.Size = new System.Drawing.Size(306, 551);
            ultraPanel1.TabIndex = 28;
            // 
            // gpDetalle
            // 
            this.gpDetalle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gpDetalle.Controls.Add(this.lblContadorFilas);
            this.gpDetalle.Controls.Add(this.grdData);
            this.gpDetalle.Location = new System.Drawing.Point(6, 6);
            this.gpDetalle.Name = "gpDetalle";
            this.gpDetalle.Size = new System.Drawing.Size(748, 542);
            this.gpDetalle.TabIndex = 26;
            this.gpDetalle.Text = "Importación desde Excel";
            this.gpDetalle.UseAppStyling = false;
            // 
            // lblContadorFilas
            // 
            this.lblContadorFilas.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance3.TextHAlignAsString = "Right";
            appearance3.TextVAlignAsString = "Middle";
            this.lblContadorFilas.Appearance = appearance3;
            this.lblContadorFilas.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.2F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContadorFilas.Location = new System.Drawing.Point(516, 13);
            this.lblContadorFilas.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblContadorFilas.Name = "lblContadorFilas";
            this.lblContadorFilas.Size = new System.Drawing.Size(226, 19);
            this.lblContadorFilas.TabIndex = 144;
            this.lblContadorFilas.Text = "No se ha realizado la importación aún.";
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
            this.grdData.Size = new System.Drawing.Size(736, 501);
            this.grdData.TabIndex = 8;
            // 
            // ultraPanel2
            // 
            // 
            // ultraPanel2.ClientArea
            // 
            this.ultraPanel2.ClientArea.Controls.Add(this.gpDetalle);
            this.ultraPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraPanel2.Location = new System.Drawing.Point(306, 0);
            this.ultraPanel2.Name = "ultraPanel2";
            this.ultraPanel2.Size = new System.Drawing.Size(759, 551);
            this.ultraPanel2.TabIndex = 29;
            // 
            // frmMigrarPlanCuentas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1065, 551);
            this.Controls.Add(this.ultraPanel2);
            this.Controls.Add(ultraPanel1);
            this.Name = "frmMigrarPlanCuentas";
            this.Text = "Migración Plan de Cuentas";
            this.Load += new System.EventHandler(this.frmMigrarPlanCuentas_Load);
            ((System.ComponentModel.ISupportInitialize)(ultraExpandableGroupBox1)).EndInit();
            ultraExpandableGroupBox1.ResumeLayout(false);
            ultraExpandableGroupBoxPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).EndInit();
            this.ultraGroupBox2.ResumeLayout(false);
            this.ultraGroupBox2.PerformLayout();
            ultraPanel1.ClientArea.ResumeLayout(false);
            ultraPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gpDetalle)).EndInit();
            this.gpDetalle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraDataSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uvDatos)).EndInit();
            this.ultraPanel2.ClientArea.ResumeLayout(false);
            this.ultraPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox2;
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
        private Infragistics.Win.Misc.UltraLabel ultraLabel6;
        private System.Windows.Forms.ComboBox cboNaturaleza;
        private Infragistics.Win.Misc.UltraLabel ultraLabel5;
        private System.Windows.Forms.ComboBox cboMoneda;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private System.Windows.Forms.ComboBox cboDetalle;
        private Infragistics.Win.UltraWinDataSource.UltraDataSource ultraDataSource1;
        private Infragistics.Win.Misc.UltraValidator uvDatos;
        private Infragistics.Win.Misc.UltraLabel ultraLabel7;
        private System.Windows.Forms.ComboBox cboAnalisis;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbDestinos;
        private System.Windows.Forms.RadioButton rbPlanContable;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel8;
        private Infragistics.Win.Misc.UltraLabel ultraLabel9;
        private System.Windows.Forms.ComboBox cboCuentaDestino;
        private System.Windows.Forms.ComboBox cboPorcentaje;
        private Infragistics.Win.Misc.UltraLabel ultraLabel10;
        private System.Windows.Forms.ComboBox cboCuentaOrigen;
        private System.Windows.Forms.ComboBox cboCuentaTransferencia;
        private Infragistics.Win.Misc.UltraLabel ultraLabel11;
        private Infragistics.Win.Misc.UltraPanel ultraPanel2;
    }
}