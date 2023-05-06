namespace SAMBHS.Windows.WinClient.UI.Reportes.Letras.LetrasCobrar
{
    partial class frmLetrasPendientesCobranza
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
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton("btnBuscar");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            this.txtDetalleEspecifico = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label5 = new Infragistics.Win.Misc.UltraLabel();
            this.BtnVuisualizar = new Infragistics.Win.Misc.UltraButton();
            this.dtpFechaRegistroDe = new System.Windows.Forms.DateTimePicker();
            this.dtpFechaRegistroAl = new System.Windows.Forms.DateTimePicker();
            this.label1 = new Infragistics.Win.Misc.UltraLabel();
            this.label2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraExpandableGroupBox1 = new Infragistics.Win.Misc.UltraExpandableGroupBox();
            this.ultraExpandableGroupBoxPanel1 = new Infragistics.Win.Misc.UltraExpandableGroupBoxPanel();
            this.cboVendedor = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.cboUbicacionLetra = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.cboEstadoLetra = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.btnExcel = new Infragistics.Win.Misc.UltraButton();
            this.label3 = new Infragistics.Win.Misc.UltraLabel();
            this.cboAgrupar = new System.Windows.Forms.ComboBox();
            this.chkFechaImpresion = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.groupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            this.pBuscando = new System.Windows.Forms.Panel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.crystalReportViewer1 = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this.frmLetrasPendientesCobranza_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            ((System.ComponentModel.ISupportInitialize)(this.txtDetalleEspecifico)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraExpandableGroupBox1)).BeginInit();
            this.ultraExpandableGroupBox1.SuspendLayout();
            this.ultraExpandableGroupBoxPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboVendedor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboUbicacionLetra)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboEstadoLetra)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkFechaImpresion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox2)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.pBuscando.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            this.frmLetrasPendientesCobranza_Fill_Panel.ClientArea.SuspendLayout();
            this.frmLetrasPendientesCobranza_Fill_Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtDetalleEspecifico
            // 
            appearance1.Image = global::SAMBHS.Windows.WinClient.UI.Resource.system_search;
            editorButton1.Appearance = appearance1;
            editorButton1.Key = "btnBuscar";
            this.txtDetalleEspecifico.ButtonsLeft.Add(editorButton1);
            this.txtDetalleEspecifico.Location = new System.Drawing.Point(604, 0);
            this.txtDetalleEspecifico.Name = "txtDetalleEspecifico";
            this.txtDetalleEspecifico.Size = new System.Drawing.Size(143, 21);
            this.txtDetalleEspecifico.TabIndex = 3;
            this.txtDetalleEspecifico.ValueChanged += new System.EventHandler(this.txtDetalleEspecifico_ValueChanged);
            this.txtDetalleEspecifico.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.txtDetalleEspecifico_EditorButtonClick);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColorInternal = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label5.Location = new System.Drawing.Point(556, 3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 14);
            this.label5.TabIndex = 266;
            this.label5.Text = "Cliente:";
            // 
            // BtnVuisualizar
            // 
            this.BtnVuisualizar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance2.Image = global::SAMBHS.Windows.WinClient.UI.Resource.eye;
            appearance2.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance2.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance2.TextHAlignAsString = "Right";
            appearance2.TextVAlignAsString = "Middle";
            this.BtnVuisualizar.Appearance = appearance2;
            this.BtnVuisualizar.Location = new System.Drawing.Point(905, 21);
            this.BtnVuisualizar.Margin = new System.Windows.Forms.Padding(2);
            this.BtnVuisualizar.Name = "BtnVuisualizar";
            this.BtnVuisualizar.Size = new System.Drawing.Size(127, 34);
            this.BtnVuisualizar.TabIndex = 5;
            this.BtnVuisualizar.Text = "Visualizar Reporte";
            this.BtnVuisualizar.Click += new System.EventHandler(this.BtnVuisualizar_Click);
            // 
            // dtpFechaRegistroDe
            // 
            this.dtpFechaRegistroDe.AllowDrop = true;
            this.dtpFechaRegistroDe.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaRegistroDe.Location = new System.Drawing.Point(96, 0);
            this.dtpFechaRegistroDe.Name = "dtpFechaRegistroDe";
            this.dtpFechaRegistroDe.Size = new System.Drawing.Size(98, 20);
            this.dtpFechaRegistroDe.TabIndex = 0;
            // 
            // dtpFechaRegistroAl
            // 
            this.dtpFechaRegistroAl.AllowDrop = true;
            this.dtpFechaRegistroAl.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaRegistroAl.Location = new System.Drawing.Point(222, 0);
            this.dtpFechaRegistroAl.Name = "dtpFechaRegistroAl";
            this.dtpFechaRegistroAl.Size = new System.Drawing.Size(96, 20);
            this.dtpFechaRegistroAl.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColorInternal = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label1.Location = new System.Drawing.Point(7, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "Fecha Venc.  del:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColorInternal = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label2.Location = new System.Drawing.Point(201, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 14);
            this.label2.TabIndex = 2;
            this.label2.Text = "Al:";
            // 
            // ultraExpandableGroupBox1
            // 
            this.ultraExpandableGroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance3.BackColor = System.Drawing.Color.Transparent;
            this.ultraExpandableGroupBox1.Appearance = appearance3;
            this.ultraExpandableGroupBox1.Controls.Add(this.ultraExpandableGroupBoxPanel1);
            this.ultraExpandableGroupBox1.ExpandedSize = new System.Drawing.Size(1040, 79);
            this.ultraExpandableGroupBox1.Location = new System.Drawing.Point(6, 6);
            this.ultraExpandableGroupBox1.Name = "ultraExpandableGroupBox1";
            this.ultraExpandableGroupBox1.Size = new System.Drawing.Size(1040, 79);
            this.ultraExpandableGroupBox1.TabIndex = 209;
            this.ultraExpandableGroupBox1.Text = "Búsqueda";
            this.ultraExpandableGroupBox1.UseAppStyling = false;
            this.ultraExpandableGroupBox1.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.XP;
            this.ultraExpandableGroupBox1.ExpandedStateChanged += new System.EventHandler(this.ultraExpandableGroupBox1_ExpandedStateChanged);
            // 
            // ultraExpandableGroupBoxPanel1
            // 
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.cboVendedor);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.ultraLabel4);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.cboUbicacionLetra);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.ultraLabel3);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.ultraLabel1);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.cboEstadoLetra);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.btnExcel);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.label3);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.cboAgrupar);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.chkFechaImpresion);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.txtDetalleEspecifico);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.label5);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.BtnVuisualizar);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.dtpFechaRegistroDe);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.dtpFechaRegistroAl);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.label1);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.label2);
            this.ultraExpandableGroupBoxPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraExpandableGroupBoxPanel1.Location = new System.Drawing.Point(3, 19);
            this.ultraExpandableGroupBoxPanel1.Name = "ultraExpandableGroupBoxPanel1";
            this.ultraExpandableGroupBoxPanel1.Size = new System.Drawing.Size(1034, 57);
            this.ultraExpandableGroupBoxPanel1.TabIndex = 0;
            // 
            // cboVendedor
            // 
            this.cboVendedor.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboVendedor.Location = new System.Drawing.Point(811, 0);
            this.cboVendedor.Name = "cboVendedor";
            this.cboVendedor.Size = new System.Drawing.Size(128, 21);
            this.cboVendedor.TabIndex = 277;
            this.cboVendedor.Visible = false;
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.AutoSize = true;
            this.ultraLabel4.BackColorInternal = System.Drawing.Color.Transparent;
            this.ultraLabel4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraLabel4.ForeColor = System.Drawing.SystemColors.MenuText;
            this.ultraLabel4.Location = new System.Drawing.Point(750, 3);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(59, 14);
            this.ultraLabel4.TabIndex = 276;
            this.ultraLabel4.Text = "Vendedor :";
            this.ultraLabel4.Visible = false;
            // 
            // cboUbicacionLetra
            // 
            this.cboUbicacionLetra.DropDownListWidth = 200;
            this.cboUbicacionLetra.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboUbicacionLetra.Location = new System.Drawing.Point(604, 28);
            this.cboUbicacionLetra.Name = "cboUbicacionLetra";
            this.cboUbicacionLetra.Size = new System.Drawing.Size(140, 21);
            this.cboUbicacionLetra.TabIndex = 275;
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.AutoSize = true;
            this.ultraLabel3.BackColorInternal = System.Drawing.Color.Transparent;
            this.ultraLabel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraLabel3.ForeColor = System.Drawing.SystemColors.MenuText;
            this.ultraLabel3.Location = new System.Drawing.Point(539, 31);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(60, 14);
            this.ultraLabel3.TabIndex = 274;
            this.ultraLabel3.Text = "Ubicación :";
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.BackColorInternal = System.Drawing.Color.Transparent;
            this.ultraLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraLabel1.ForeColor = System.Drawing.SystemColors.MenuText;
            this.ultraLabel1.Location = new System.Drawing.Point(328, 31);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(46, 14);
            this.ultraLabel1.TabIndex = 273;
            this.ultraLabel1.Text = "Estado :";
            // 
            // cboEstadoLetra
            // 
            this.cboEstadoLetra.DropDownListWidth = 200;
            this.cboEstadoLetra.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboEstadoLetra.Location = new System.Drawing.Point(402, 28);
            this.cboEstadoLetra.Name = "cboEstadoLetra";
            this.cboEstadoLetra.Size = new System.Drawing.Size(128, 21);
            this.cboEstadoLetra.TabIndex = 272;
            // 
            // btnExcel
            // 
            this.btnExcel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance4.Image = global::SAMBHS.Windows.WinClient.UI.Resource.page_excel1;
            appearance4.ImageHAlign = Infragistics.Win.HAlign.Center;
            this.btnExcel.Appearance = appearance4;
            this.btnExcel.Location = new System.Drawing.Point(871, 21);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(34, 34);
            this.btnExcel.TabIndex = 271;
            this.btnExcel.Click += new System.EventHandler(this.BtnVuisualizar_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColorInternal = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label3.Location = new System.Drawing.Point(328, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 14);
            this.label3.TabIndex = 270;
            this.label3.Text = "Agrupar Por:";
            // 
            // cboAgrupar
            // 
            this.cboAgrupar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAgrupar.FormattingEnabled = true;
            this.cboAgrupar.Items.AddRange(new object[] {
            "SIN AGRUPAR",
            "CLIENTE",
            "ESTADO",
            "UBICACIÓN"});
            this.cboAgrupar.Location = new System.Drawing.Point(402, 0);
            this.cboAgrupar.Name = "cboAgrupar";
            this.cboAgrupar.Size = new System.Drawing.Size(128, 21);
            this.cboAgrupar.TabIndex = 2;
            // 
            // chkFechaImpresion
            // 
            this.chkFechaImpresion.AutoSize = true;
            this.chkFechaImpresion.Location = new System.Drawing.Point(7, 31);
            this.chkFechaImpresion.Name = "chkFechaImpresion";
            this.chkFechaImpresion.Size = new System.Drawing.Size(199, 17);
            this.chkFechaImpresion.TabIndex = 4;
            this.chkFechaImpresion.Text = "Mostrar Fecha y Hora de Impresión";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.pBuscando);
            this.groupBox2.Controls.Add(this.crystalReportViewer1);
            this.groupBox2.Location = new System.Drawing.Point(4, 88);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1045, 474);
            this.groupBox2.TabIndex = 208;
            this.groupBox2.Text = "Reporte";
            this.groupBox2.UseAppStyling = false;
            this.groupBox2.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.XP;
            // 
            // pBuscando
            // 
            this.pBuscando.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pBuscando.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pBuscando.Controls.Add(this.ultraLabel2);
            this.pBuscando.Controls.Add(this.pictureBox1);
            this.pBuscando.Location = new System.Drawing.Point(430, 161);
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
            this.crystalReportViewer1.Size = new System.Drawing.Size(1039, 455);
            this.crystalReportViewer1.TabIndex = 1;
            this.crystalReportViewer1.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // frmLetrasPendientesCobranza_Fill_Panel
            // 
            // 
            // frmLetrasPendientesCobranza_Fill_Panel.ClientArea
            // 
            this.frmLetrasPendientesCobranza_Fill_Panel.ClientArea.Controls.Add(this.ultraExpandableGroupBox1);
            this.frmLetrasPendientesCobranza_Fill_Panel.ClientArea.Controls.Add(this.groupBox2);
            this.frmLetrasPendientesCobranza_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.frmLetrasPendientesCobranza_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmLetrasPendientesCobranza_Fill_Panel.Location = new System.Drawing.Point(8, 31);
            this.frmLetrasPendientesCobranza_Fill_Panel.Name = "frmLetrasPendientesCobranza_Fill_Panel";
            this.frmLetrasPendientesCobranza_Fill_Panel.Size = new System.Drawing.Size(1052, 574);
            this.frmLetrasPendientesCobranza_Fill_Panel.TabIndex = 0;
            // 
            // _frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Left
            // 
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 31);
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Left.Name = "_frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Left";
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 574);
            // 
            // _frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Right
            // 
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(1060, 31);
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Right.Name = "_frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Right";
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 574);
            // 
            // _frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Top
            // 
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Top.Name = "_frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Top";
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(1068, 31);
            // 
            // _frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Bottom
            // 
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 605);
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Bottom.Name = "_frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Bottom";
            this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(1068, 8);
            // 
            // frmLetrasPendientesCobranza
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1068, 613);
            this.Controls.Add(this.frmLetrasPendientesCobranza_Fill_Panel);
            this.Controls.Add(this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Bottom);
            this.Name = "frmLetrasPendientesCobranza";
            this.Text = "Letras Pendientes de Cobranza";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmLetrasPendientesCobranza_FormClosing);
            this.Load += new System.EventHandler(this.frmLetrasPendientesCobranza_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtDetalleEspecifico)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraExpandableGroupBox1)).EndInit();
            this.ultraExpandableGroupBox1.ResumeLayout(false);
            this.ultraExpandableGroupBoxPanel1.ResumeLayout(false);
            this.ultraExpandableGroupBoxPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboVendedor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboUbicacionLetra)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboEstadoLetra)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkFechaImpresion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox2)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.pBuscando.ResumeLayout(false);
            this.pBuscando.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            this.frmLetrasPendientesCobranza_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmLetrasPendientesCobranza_Fill_Panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtDetalleEspecifico;
        private Infragistics.Win.Misc.UltraLabel label5;
        private Infragistics.Win.Misc.UltraButton BtnVuisualizar;
        private System.Windows.Forms.DateTimePicker dtpFechaRegistroDe;
        private System.Windows.Forms.DateTimePicker dtpFechaRegistroAl;
        private Infragistics.Win.Misc.UltraLabel label1;
        private Infragistics.Win.Misc.UltraLabel label2;
        private Infragistics.Win.Misc.UltraExpandableGroupBox ultraExpandableGroupBox1;
        private Infragistics.Win.Misc.UltraExpandableGroupBoxPanel ultraExpandableGroupBoxPanel1;
        private Infragistics.Win.Misc.UltraGroupBox groupBox2;
        private CrystalDecisions.Windows.Forms.CrystalReportViewer crystalReportViewer1;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkFechaImpresion;
        private Infragistics.Win.Misc.UltraLabel label3;
        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.Misc.UltraPanel frmLetrasPendientesCobranza_Fill_Panel;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmLetrasPendientesCobranza_UltraFormManager_Dock_Area_Bottom;
        private System.Windows.Forms.Panel pBuscando;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private Infragistics.Win.Misc.UltraButton btnExcel;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboEstadoLetra;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboUbicacionLetra;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboVendedor;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private System.Windows.Forms.ComboBox cboAgrupar;

    }
}