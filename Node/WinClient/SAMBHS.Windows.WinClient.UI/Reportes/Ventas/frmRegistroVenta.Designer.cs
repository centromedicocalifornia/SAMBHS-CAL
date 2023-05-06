namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    partial class frmRegistroVenta
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
            Infragistics.Win.Misc.UltraLabel lbl1;
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            this.dtpFechaRegistroAl = new System.Windows.Forms.DateTimePicker();
            this.dtpFechaRegistroDe = new System.Windows.Forms.DateTimePicker();
            this.chkHoraimpresion = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.BtnImprimir = new Infragistics.Win.Misc.UltraButton();
            this.label6 = new Infragistics.Win.Misc.UltraLabel();
            this.groupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.pBuscando = new System.Windows.Forms.Panel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.crystalReportViewer1 = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.label2 = new Infragistics.Win.Misc.UltraLabel();
            this.label1 = new Infragistics.Win.Misc.UltraLabel();
            this.label5 = new Infragistics.Win.Misc.UltraLabel();
            this.label4 = new Infragistics.Win.Misc.UltraLabel();
            this.cboEstablecimiento = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.btnExcel = new Infragistics.Win.Misc.UltraButton();
            this.cboOrden = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.cboVendedor = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.cboDetalleDocumento = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.cboAlmacen = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.cboMoneda = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.label3 = new Infragistics.Win.Misc.UltraLabel();
            this.label7 = new Infragistics.Win.Misc.UltraLabel();
            this.uvDatos = new Infragistics.Win.Misc.UltraValidator(this.components);
            this.cboAgrupar = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this.frmRegistroVenta_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this.ultraExpandableGroupBox1 = new Infragistics.Win.Misc.UltraExpandableGroupBox();
            this.ultraExpandableGroupBoxPanel1 = new Infragistics.Win.Misc.UltraExpandableGroupBoxPanel();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            lbl1 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.chkHoraimpresion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.pBuscando.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboEstablecimiento)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboOrden)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboVendedor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDetalleDocumento)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAlmacen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboMoneda)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uvDatos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAgrupar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            this.frmRegistroVenta_Fill_Panel.ClientArea.SuspendLayout();
            this.frmRegistroVenta_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraExpandableGroupBox1)).BeginInit();
            this.ultraExpandableGroupBox1.SuspendLayout();
            this.ultraExpandableGroupBoxPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl1
            // 
            lbl1.AutoSize = true;
            lbl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            lbl1.Location = new System.Drawing.Point(12, 1);
            lbl1.Name = "lbl1";
            lbl1.Size = new System.Drawing.Size(87, 14);
            lbl1.TabIndex = 265;
            lbl1.Text = "Establecimiento:";
            // 
            // dtpFechaRegistroAl
            // 
            this.dtpFechaRegistroAl.AllowDrop = true;
            this.dtpFechaRegistroAl.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaRegistroAl.Location = new System.Drawing.Point(449, -2);
            this.dtpFechaRegistroAl.Name = "dtpFechaRegistroAl";
            this.dtpFechaRegistroAl.Size = new System.Drawing.Size(100, 20);
            this.dtpFechaRegistroAl.TabIndex = 1;
            this.dtpFechaRegistroAl.ValueChanged += new System.EventHandler(this.dtpFechaRegistroAl_ValueChanged);
            // 
            // dtpFechaRegistroDe
            // 
            this.dtpFechaRegistroDe.AllowDrop = true;
            this.dtpFechaRegistroDe.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaRegistroDe.Location = new System.Drawing.Point(323, -2);
            this.dtpFechaRegistroDe.Name = "dtpFechaRegistroDe";
            this.dtpFechaRegistroDe.Size = new System.Drawing.Size(98, 20);
            this.dtpFechaRegistroDe.TabIndex = 0;
            // 
            // chkHoraimpresion
            // 
            this.chkHoraimpresion.AutoSize = true;
            this.chkHoraimpresion.BackColor = System.Drawing.Color.Transparent;
            this.chkHoraimpresion.BackColorInternal = System.Drawing.Color.Transparent;
            this.chkHoraimpresion.Checked = true;
            this.chkHoraimpresion.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkHoraimpresion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkHoraimpresion.Location = new System.Drawing.Point(801, 1);
            this.chkHoraimpresion.Name = "chkHoraimpresion";
            this.chkHoraimpresion.Size = new System.Drawing.Size(157, 17);
            this.chkHoraimpresion.TabIndex = 4;
            this.chkHoraimpresion.Text = "Fecha y Hora de Impresión ";
            // 
            // BtnImprimir
            // 
            this.BtnImprimir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.Image = global::SAMBHS.Windows.WinClient.UI.Resource.eye;
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance1.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance1.TextHAlignAsString = "Right";
            appearance1.TextVAlignAsString = "Middle";
            this.BtnImprimir.Appearance = appearance1;
            this.BtnImprimir.Location = new System.Drawing.Point(896, 48);
            this.BtnImprimir.Margin = new System.Windows.Forms.Padding(2);
            this.BtnImprimir.Name = "BtnImprimir";
            this.BtnImprimir.Size = new System.Drawing.Size(130, 30);
            this.BtnImprimir.TabIndex = 7;
            this.BtnImprimir.Text = "Visualizar Reporte";
            this.BtnImprimir.Click += new System.EventHandler(this.BtnImprimir_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColorInternal = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(253, 29);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 14);
            this.label6.TabIndex = 24;
            this.label6.Text = "Ordena por:";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.BackColorInternal = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.pBuscando);
            this.groupBox1.Controls.Add(this.crystalReportViewer1);
            this.groupBox1.Location = new System.Drawing.Point(6, 115);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1040, 458);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.Text = "Reporte de Ventas";
            // 
            // pBuscando
            // 
            this.pBuscando.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pBuscando.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pBuscando.Controls.Add(this.ultraLabel2);
            this.pBuscando.Controls.Add(this.pictureBox1);
            this.pBuscando.Location = new System.Drawing.Point(455, 207);
            this.pBuscando.Name = "pBuscando";
            this.pBuscando.Size = new System.Drawing.Size(185, 42);
            this.pBuscando.TabIndex = 7;
            this.pBuscando.Visible = false;
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.ultraLabel2.Location = new System.Drawing.Point(53, 13);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(134, 17);
            this.ultraLabel2.TabIndex = 1;
            this.ultraLabel2.Text = "Generando Reporte...";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::SAMBHS.Windows.WinClient.UI.Resource.loadingfinal1;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(35, 35);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // crystalReportViewer1
            // 
            this.crystalReportViewer1.ActiveViewIndex = -1;
            this.crystalReportViewer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crystalReportViewer1.Cursor = System.Windows.Forms.Cursors.Default;
            this.crystalReportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crystalReportViewer1.Location = new System.Drawing.Point(3, 16);
            this.crystalReportViewer1.Name = "crystalReportViewer1";
            this.crystalReportViewer1.ShowRefreshButton = false;
            this.crystalReportViewer1.Size = new System.Drawing.Size(1034, 439);
            this.crystalReportViewer1.TabIndex = 4;
            this.crystalReportViewer1.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColorInternal = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label2.Location = new System.Drawing.Point(427, 1);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 14);
            this.label2.TabIndex = 2;
            this.label2.Text = "Al:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColorInternal = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label1.Location = new System.Drawing.Point(257, 1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "Fecha  del:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColorInternal = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label5.Location = new System.Drawing.Point(563, 29);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 14);
            this.label5.TabIndex = 23;
            this.label5.Text = "Vendedor:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColorInternal = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label4.Location = new System.Drawing.Point(34, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 14);
            this.label4.TabIndex = 12;
            this.label4.Text = "Documento:";
            // 
            // cboEstablecimiento
            // 
            this.cboEstablecimiento.DropDownListWidth = 350;
            this.cboEstablecimiento.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboEstablecimiento.Location = new System.Drawing.Point(99, 0);
            this.cboEstablecimiento.Name = "cboEstablecimiento";
            this.cboEstablecimiento.Size = new System.Drawing.Size(138, 21);
            this.cboEstablecimiento.TabIndex = 263;
            this.cboEstablecimiento.ValueChanged += new System.EventHandler(this.cboEstablecimiento_ValueChanged);
            // 
            // btnExcel
            // 
            this.btnExcel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance2.Image = global::SAMBHS.Windows.WinClient.UI.Resource.page_excel;
            appearance2.ImageHAlign = Infragistics.Win.HAlign.Center;
            this.btnExcel.Appearance = appearance2;
            this.btnExcel.Location = new System.Drawing.Point(867, 49);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(29, 29);
            this.btnExcel.TabIndex = 262;
            this.btnExcel.Click += new System.EventHandler(this.BtnImprimir_Click);
            // 
            // cboOrden
            // 
            this.cboOrden.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboOrden.Location = new System.Drawing.Point(322, 25);
            this.cboOrden.Name = "cboOrden";
            this.cboOrden.Size = new System.Drawing.Size(227, 21);
            this.cboOrden.TabIndex = 5;
            this.uvDatos.GetValidationSettings(this.cboOrden).Condition = new Infragistics.Win.OperatorCondition(Infragistics.Win.ConditionOperator.NotEquals, "-1", true, typeof(string));
            this.uvDatos.GetValidationSettings(this.cboOrden).Enabled = false;
            this.uvDatos.GetValidationSettings(this.cboOrden).IsRequired = true;
            // 
            // cboVendedor
            // 
            this.cboVendedor.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboVendedor.Location = new System.Drawing.Point(623, 25);
            this.cboVendedor.Name = "cboVendedor";
            this.cboVendedor.Size = new System.Drawing.Size(147, 21);
            this.cboVendedor.TabIndex = 7;
            // 
            // cboDetalleDocumento
            // 
            this.cboDetalleDocumento.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboDetalleDocumento.Location = new System.Drawing.Point(99, 52);
            this.cboDetalleDocumento.Name = "cboDetalleDocumento";
            this.cboDetalleDocumento.Size = new System.Drawing.Size(138, 21);
            this.cboDetalleDocumento.TabIndex = 3;
            // 
            // cboAlmacen
            // 
            this.cboAlmacen.DropDownListWidth = 350;
            this.cboAlmacen.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboAlmacen.Location = new System.Drawing.Point(99, 25);
            this.cboAlmacen.Name = "cboAlmacen";
            this.cboAlmacen.Size = new System.Drawing.Size(138, 21);
            this.cboAlmacen.TabIndex = 6;
            // 
            // cboMoneda
            // 
            this.cboMoneda.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboMoneda.Location = new System.Drawing.Point(623, 0);
            this.cboMoneda.Name = "cboMoneda";
            this.cboMoneda.Size = new System.Drawing.Size(147, 21);
            this.cboMoneda.TabIndex = 2;
            this.uvDatos.GetValidationSettings(this.cboMoneda).Condition = new Infragistics.Win.OperatorCondition(Infragistics.Win.ConditionOperator.NotEquals, "-1", true, typeof(string));
            this.uvDatos.GetValidationSettings(this.cboMoneda).Enabled = false;
            this.uvDatos.GetValidationSettings(this.cboMoneda).IsRequired = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColorInternal = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label3.Location = new System.Drawing.Point(48, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 14);
            this.label3.TabIndex = 211;
            this.label3.Text = "Almacén:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColorInternal = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label7.Location = new System.Drawing.Point(571, 1);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(48, 14);
            this.label7.TabIndex = 210;
            this.label7.Text = "Moneda:";
            // 
            // cboAgrupar
            // 
            this.cboAgrupar.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            valueListItem1.DataValue = "ValueListItem0";
            valueListItem1.DisplayText = "SIN AGRUPAR";
            valueListItem2.DataValue = "ValueListItem1";
            valueListItem2.DisplayText = "MONEDA";
            this.cboAgrupar.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2});
            this.cboAgrupar.Location = new System.Drawing.Point(322, 52);
            this.cboAgrupar.Name = "cboAgrupar";
            this.cboAgrupar.Size = new System.Drawing.Size(227, 21);
            this.cboAgrupar.TabIndex = 266;
            this.uvDatos.GetValidationSettings(this.cboAgrupar).Condition = new Infragistics.Win.OperatorCondition(Infragistics.Win.ConditionOperator.NotEquals, "-1", true, typeof(string));
            this.uvDatos.GetValidationSettings(this.cboAgrupar).Enabled = false;
            this.uvDatos.GetValidationSettings(this.cboAgrupar).IsRequired = true;
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // frmRegistroVenta_Fill_Panel
            // 
            // 
            // frmRegistroVenta_Fill_Panel.ClientArea
            // 
            this.frmRegistroVenta_Fill_Panel.ClientArea.Controls.Add(this.ultraExpandableGroupBox1);
            this.frmRegistroVenta_Fill_Panel.ClientArea.Controls.Add(this.groupBox1);
            this.frmRegistroVenta_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.frmRegistroVenta_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmRegistroVenta_Fill_Panel.Location = new System.Drawing.Point(8, 32);
            this.frmRegistroVenta_Fill_Panel.Name = "frmRegistroVenta_Fill_Panel";
            this.frmRegistroVenta_Fill_Panel.Size = new System.Drawing.Size(1052, 573);
            this.frmRegistroVenta_Fill_Panel.TabIndex = 0;
            // 
            // ultraExpandableGroupBox1
            // 
            this.ultraExpandableGroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraExpandableGroupBox1.Controls.Add(this.ultraExpandableGroupBoxPanel1);
            this.ultraExpandableGroupBox1.ExpandedSize = new System.Drawing.Size(1034, 105);
            this.ultraExpandableGroupBox1.Location = new System.Drawing.Point(9, 7);
            this.ultraExpandableGroupBox1.Name = "ultraExpandableGroupBox1";
            this.ultraExpandableGroupBox1.Size = new System.Drawing.Size(1034, 105);
            this.ultraExpandableGroupBox1.TabIndex = 8;
            this.ultraExpandableGroupBox1.Text = "Filtro de Búsqueda";
            this.ultraExpandableGroupBox1.ExpandedStateChanged += new System.EventHandler(this.ultraExpandableGroupBox1_ExpandedStateChanged);
            // 
            // ultraExpandableGroupBoxPanel1
            // 
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.cboAgrupar);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.ultraLabel1);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.btnExcel);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(lbl1);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.BtnImprimir);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.cboDetalleDocumento);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.cboOrden);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.label4);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.cboEstablecimiento);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.cboVendedor);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.chkHoraimpresion);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.label3);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.cboAlmacen);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.cboMoneda);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.label7);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.dtpFechaRegistroDe);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.label5);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.dtpFechaRegistroAl);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.label6);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.label1);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.label2);
            this.ultraExpandableGroupBoxPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraExpandableGroupBoxPanel1.Location = new System.Drawing.Point(3, 19);
            this.ultraExpandableGroupBoxPanel1.Name = "ultraExpandableGroupBoxPanel1";
            this.ultraExpandableGroupBoxPanel1.Size = new System.Drawing.Size(1028, 83);
            this.ultraExpandableGroupBoxPanel1.TabIndex = 0;
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.BackColorInternal = System.Drawing.Color.Transparent;
            this.ultraLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraLabel1.Location = new System.Drawing.Point(269, 55);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(48, 14);
            this.ultraLabel1.TabIndex = 267;
            this.ultraLabel1.Text = "Agrupar:";
            // 
            // _frmRegistroVenta_UltraFormManager_Dock_Area_Left
            // 
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 32);
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Left.Name = "_frmRegistroVenta_UltraFormManager_Dock_Area_Left";
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 573);
            // 
            // _frmRegistroVenta_UltraFormManager_Dock_Area_Right
            // 
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(1060, 32);
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Right.Name = "_frmRegistroVenta_UltraFormManager_Dock_Area_Right";
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 573);
            // 
            // _frmRegistroVenta_UltraFormManager_Dock_Area_Top
            // 
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Top.Name = "_frmRegistroVenta_UltraFormManager_Dock_Area_Top";
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(1068, 32);
            // 
            // _frmRegistroVenta_UltraFormManager_Dock_Area_Bottom
            // 
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 605);
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Bottom.Name = "_frmRegistroVenta_UltraFormManager_Dock_Area_Bottom";
            this._frmRegistroVenta_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(1068, 8);
            // 
            // frmRegistroVenta
            // 
            this.AcceptButton = this.BtnImprimir;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(1068, 613);
            this.Controls.Add(this.frmRegistroVenta_Fill_Panel);
            this.Controls.Add(this._frmRegistroVenta_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._frmRegistroVenta_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._frmRegistroVenta_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._frmRegistroVenta_UltraFormManager_Dock_Area_Bottom);
            this.Name = "frmRegistroVenta";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reporte de Ventas";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmRegistroVenta_FormClosing);
            this.Load += new System.EventHandler(this.frmRegistroVenta_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chkHoraimpresion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.pBuscando.ResumeLayout(false);
            this.pBuscando.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboEstablecimiento)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboOrden)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboVendedor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboDetalleDocumento)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAlmacen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboMoneda)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uvDatos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAgrupar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            this.frmRegistroVenta_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmRegistroVenta_Fill_Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraExpandableGroupBox1)).EndInit();
            this.ultraExpandableGroupBox1.ResumeLayout(false);
            this.ultraExpandableGroupBoxPanel1.ResumeLayout(false);
            this.ultraExpandableGroupBoxPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dtpFechaRegistroDe;
        private System.Windows.Forms.DateTimePicker dtpFechaRegistroAl;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkHoraimpresion;
        private Infragistics.Win.Misc.UltraButton BtnImprimir;
        private Infragistics.Win.Misc.UltraLabel label6;
        private Infragistics.Win.Misc.UltraGroupBox groupBox1;
        private Infragistics.Win.Misc.UltraLabel label5;
        private Infragistics.Win.Misc.UltraLabel label4;
        private Infragistics.Win.Misc.UltraLabel label2;
        private Infragistics.Win.Misc.UltraLabel label1;
        private Infragistics.Win.Misc.UltraLabel label7;
        private Infragistics.Win.Misc.UltraValidator uvDatos;
        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.Misc.UltraPanel frmRegistroVenta_Fill_Panel;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmRegistroVenta_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmRegistroVenta_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmRegistroVenta_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmRegistroVenta_UltraFormManager_Dock_Area_Bottom;
        private Infragistics.Win.Misc.UltraLabel label3;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboMoneda;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboAlmacen;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboOrden;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboVendedor;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboDetalleDocumento;
        private CrystalDecisions.Windows.Forms.CrystalReportViewer crystalReportViewer1;
        private System.Windows.Forms.Panel pBuscando;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private Infragistics.Win.Misc.UltraButton btnExcel;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboEstablecimiento;
        private Infragistics.Win.Misc.UltraExpandableGroupBox ultraExpandableGroupBox1;
        private Infragistics.Win.Misc.UltraExpandableGroupBoxPanel ultraExpandableGroupBoxPanel1;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboAgrupar;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;

    }
}