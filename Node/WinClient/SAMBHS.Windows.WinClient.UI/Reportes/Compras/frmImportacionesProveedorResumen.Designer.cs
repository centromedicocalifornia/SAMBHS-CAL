namespace SAMBHS.Windows.WinClient.UI.Reportes.Compras
{
    partial class frmImportacionesProveedorResumen
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
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton("btnBuscarProveedor");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmImportacionesProveedorResumen));
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            this.groupBox5 = new Infragistics.Win.Misc.UltraGroupBox();
            this.cboAlmacen = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.txtProveedor = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtNroPedido = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label9 = new Infragistics.Win.Misc.UltraLabel();
            this.txtRazonSocial = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label4 = new Infragistics.Win.Misc.UltraLabel();
            this.label5 = new Infragistics.Win.Misc.UltraLabel();
            this.dtpFechaInicio = new System.Windows.Forms.DateTimePicker();
            this.btnVuisualizar = new Infragistics.Win.Misc.UltraButton();
            this.dtpFechaFin = new System.Windows.Forms.DateTimePicker();
            this.label1 = new Infragistics.Win.Misc.UltraLabel();
            this.label2 = new Infragistics.Win.Misc.UltraLabel();
            this.groupBox4 = new Infragistics.Win.Misc.UltraGroupBox();
            this.crystalReportViewer1 = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.uvReporteIR = new Infragistics.Win.Misc.UltraValidator(this.components);
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this.frmImportacionesProveedorResumen_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this.pBuscando = new System.Windows.Forms.Panel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox5)).BeginInit();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboAlmacen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProveedor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNroPedido)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRazonSocial)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox4)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uvReporteIR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            this.frmImportacionesProveedorResumen_Fill_Panel.ClientArea.SuspendLayout();
            this.frmImportacionesProveedorResumen_Fill_Panel.SuspendLayout();
            this.pBuscando.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.BackColorInternal = System.Drawing.Color.Transparent;
            this.groupBox5.Controls.Add(this.cboAlmacen);
            this.groupBox5.Controls.Add(this.txtProveedor);
            this.groupBox5.Controls.Add(this.txtNroPedido);
            this.groupBox5.Controls.Add(this.label9);
            this.groupBox5.Controls.Add(this.txtRazonSocial);
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Controls.Add(this.label5);
            this.groupBox5.Controls.Add(this.dtpFechaInicio);
            this.groupBox5.Controls.Add(this.btnVuisualizar);
            this.groupBox5.Controls.Add(this.dtpFechaFin);
            this.groupBox5.Controls.Add(this.label1);
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Location = new System.Drawing.Point(3, 1);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(1124, 81);
            this.groupBox5.TabIndex = 0;
            this.groupBox5.Text = "Filtro de Búsqueda";
            // 
            // cboAlmacen
            // 
            this.cboAlmacen.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboAlmacen.Location = new System.Drawing.Point(375, 46);
            this.cboAlmacen.Name = "cboAlmacen";
            this.cboAlmacen.Size = new System.Drawing.Size(210, 21);
            this.cboAlmacen.TabIndex = 5;
            this.uvReporteIR.GetValidationSettings(this.cboAlmacen).Condition = new Infragistics.Win.OperatorCondition(Infragistics.Win.ConditionOperator.NotEquals, "-1", true, typeof(string));
            this.uvReporteIR.GetValidationSettings(this.cboAlmacen).Enabled = false;
            this.uvReporteIR.GetValidationSettings(this.cboAlmacen).IsRequired = true;
            // 
            // txtProveedor
            // 
            appearance1.Image = ((object)(resources.GetObject("appearance1.Image")));
            editorButton1.Appearance = appearance1;
            editorButton1.Key = "btnBuscarProveedor";
            this.txtProveedor.ButtonsLeft.Add(editorButton1);
            this.txtProveedor.Location = new System.Drawing.Point(375, 22);
            this.txtProveedor.Name = "txtProveedor";
            this.txtProveedor.NullText = "Código Proveedor";
            this.txtProveedor.Size = new System.Drawing.Size(98, 21);
            this.txtProveedor.TabIndex = 2;
            this.txtProveedor.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.txtProveedor_EditorButtonClick);
            this.txtProveedor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtProveedor_KeyPress);
            this.txtProveedor.Validated += new System.EventHandler(this.txtProveedor_Validated);
            // 
            // txtNroPedido
            // 
            this.txtNroPedido.Location = new System.Drawing.Point(82, 46);
            this.txtNroPedido.Name = "txtNroPedido";
            this.txtNroPedido.Size = new System.Drawing.Size(212, 21);
            this.txtNroPedido.TabIndex = 4;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColorInternal = System.Drawing.Color.Transparent;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label9.Location = new System.Drawing.Point(11, 50);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(67, 14);
            this.label9.TabIndex = 257;
            this.label9.Text = "Nro. Pedido:";
            // 
            // txtRazonSocial
            // 
            this.txtRazonSocial.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRazonSocial.Enabled = false;
            this.txtRazonSocial.Location = new System.Drawing.Point(479, 22);
            this.txtRazonSocial.Name = "txtRazonSocial";
            this.txtRazonSocial.ReadOnly = true;
            this.txtRazonSocial.Size = new System.Drawing.Size(527, 21);
            this.txtRazonSocial.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColorInternal = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label4.Location = new System.Drawing.Point(315, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 14);
            this.label4.TabIndex = 247;
            this.label4.Text = "Proveedor:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColorInternal = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label5.Location = new System.Drawing.Point(324, 50);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 14);
            this.label5.TabIndex = 242;
            this.label5.Text = "Almacén:";
            // 
            // dtpFechaInicio
            // 
            this.dtpFechaInicio.AllowDrop = true;
            this.dtpFechaInicio.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaInicio.Location = new System.Drawing.Point(82, 22);
            this.dtpFechaInicio.Name = "dtpFechaInicio";
            this.dtpFechaInicio.Size = new System.Drawing.Size(98, 20);
            this.dtpFechaInicio.TabIndex = 0;
            // 
            // btnVuisualizar
            // 
            this.btnVuisualizar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance3.Image = global::SAMBHS.Windows.WinClient.UI.Resource.eye;
            appearance3.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance3.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance3.TextHAlignAsString = "Right";
            appearance3.TextVAlignAsString = "Middle";
            this.btnVuisualizar.Appearance = appearance3;
            this.btnVuisualizar.Location = new System.Drawing.Point(991, 46);
            this.btnVuisualizar.Margin = new System.Windows.Forms.Padding(2);
            this.btnVuisualizar.Name = "btnVuisualizar";
            this.btnVuisualizar.Size = new System.Drawing.Size(125, 30);
            this.btnVuisualizar.TabIndex = 6;
            this.btnVuisualizar.Text = "Visualizar Reporte";
            this.btnVuisualizar.Click += new System.EventHandler(this.btnVuisualizar_Click);
            // 
            // dtpFechaFin
            // 
            this.dtpFechaFin.AllowDrop = true;
            this.dtpFechaFin.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaFin.Location = new System.Drawing.Point(199, 22);
            this.dtpFechaFin.Name = "dtpFechaFin";
            this.dtpFechaFin.Size = new System.Drawing.Size(95, 20);
            this.dtpFechaFin.TabIndex = 1;
            this.dtpFechaFin.ValueChanged += new System.EventHandler(this.dtpFechaFin_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColorInternal = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label1.Location = new System.Drawing.Point(18, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "Fecha  del:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColorInternal = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label2.Location = new System.Drawing.Point(182, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(13, 14);
            this.label2.TabIndex = 2;
            this.label2.Text = "al";
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.BackColorInternal = System.Drawing.Color.Transparent;
            this.groupBox4.Controls.Add(this.pBuscando);
            this.groupBox4.Controls.Add(this.crystalReportViewer1);
            this.groupBox4.Location = new System.Drawing.Point(3, 88);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(1122, 467);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.Text = "Reporte de  Importaciones por Proveedor Resumen";
            // 
            // crystalReportViewer1
            // 
            this.crystalReportViewer1.ActiveViewIndex = -1;
            this.crystalReportViewer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.crystalReportViewer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crystalReportViewer1.Cursor = System.Windows.Forms.Cursors.Default;
            this.crystalReportViewer1.ForeColor = System.Drawing.Color.Coral;
            this.crystalReportViewer1.Location = new System.Drawing.Point(6, 19);
            this.crystalReportViewer1.Name = "crystalReportViewer1";
            this.crystalReportViewer1.ShowCopyButton = false;
            this.crystalReportViewer1.ShowGroupTreeButton = false;
            this.crystalReportViewer1.ShowParameterPanelButton = false;
            this.crystalReportViewer1.ShowRefreshButton = false;
            this.crystalReportViewer1.Size = new System.Drawing.Size(1110, 442);
            this.crystalReportViewer1.TabIndex = 1;
            this.crystalReportViewer1.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // _frmReporteExtractoBancario_UltraFormManager_Dock_Area_Top
            // 
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Top.Name = "_frmReporteExtractoBancario_UltraFormManager_Dock_Area_Top";
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(1149, 31);
            // 
            // _frmReporteExtractoBancario_UltraFormManager_Dock_Area_Bottom
            // 
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 594);
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Bottom.Name = "_frmReporteExtractoBancario_UltraFormManager_Dock_Area_Bottom";
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(1149, 8);
            // 
            // _frmReporteExtractoBancario_UltraFormManager_Dock_Area_Left
            // 
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 31);
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Left.Name = "_frmReporteExtractoBancario_UltraFormManager_Dock_Area_Left";
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 563);
            // 
            // _frmReporteExtractoBancario_UltraFormManager_Dock_Area_Right
            // 
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(1141, 31);
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Right.Name = "_frmReporteExtractoBancario_UltraFormManager_Dock_Area_Right";
            this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 563);
            // 
            // frmImportacionesProveedorResumen_Fill_Panel
            // 
            // 
            // frmImportacionesProveedorResumen_Fill_Panel.ClientArea
            // 
            this.frmImportacionesProveedorResumen_Fill_Panel.ClientArea.Controls.Add(this.groupBox4);
            this.frmImportacionesProveedorResumen_Fill_Panel.ClientArea.Controls.Add(this.groupBox5);
            this.frmImportacionesProveedorResumen_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.frmImportacionesProveedorResumen_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmImportacionesProveedorResumen_Fill_Panel.Location = new System.Drawing.Point(8, 31);
            this.frmImportacionesProveedorResumen_Fill_Panel.Name = "frmImportacionesProveedorResumen_Fill_Panel";
            this.frmImportacionesProveedorResumen_Fill_Panel.Size = new System.Drawing.Size(1133, 563);
            this.frmImportacionesProveedorResumen_Fill_Panel.TabIndex = 0;
            // 
            // pBuscando
            // 
            this.pBuscando.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pBuscando.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pBuscando.Controls.Add(this.ultraLabel2);
            this.pBuscando.Controls.Add(this.pictureBox1);
            this.pBuscando.Location = new System.Drawing.Point(469, 212);
            this.pBuscando.Name = "pBuscando";
            this.pBuscando.Size = new System.Drawing.Size(185, 42);
            this.pBuscando.TabIndex = 8;
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
            // frmImportacionesProveedorResumen
            // 
            this.AcceptButton = this.btnVuisualizar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(1149, 602);
            this.Controls.Add(this.frmImportacionesProveedorResumen_Fill_Panel);
            this.Controls.Add(this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._frmReporteExtractoBancario_UltraFormManager_Dock_Area_Bottom);
            this.Name = "frmImportacionesProveedorResumen";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reporte de Importaciones por Proveedor-Resumen";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmImportacionesProveedorResumen_FormClosing);
            this.Load += new System.EventHandler(this.frmImportacionesProveedorResumen_Load);
            ((System.ComponentModel.ISupportInitialize)(this.groupBox5)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboAlmacen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProveedor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNroPedido)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRazonSocial)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox4)).EndInit();
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.uvReporteIR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            this.frmImportacionesProveedorResumen_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmImportacionesProveedorResumen_Fill_Panel.ResumeLayout(false);
            this.pBuscando.ResumeLayout(false);
            this.pBuscando.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox groupBox5;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtNroPedido;
        private Infragistics.Win.Misc.UltraLabel label9;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtRazonSocial;
        private Infragistics.Win.Misc.UltraLabel label4;
        private Infragistics.Win.Misc.UltraLabel label5;
        private System.Windows.Forms.DateTimePicker dtpFechaInicio;
        private Infragistics.Win.Misc.UltraButton btnVuisualizar;
        private System.Windows.Forms.DateTimePicker dtpFechaFin;
        private Infragistics.Win.Misc.UltraLabel label1;
        private Infragistics.Win.Misc.UltraLabel label2;
        private Infragistics.Win.Misc.UltraGroupBox groupBox4;
        private CrystalDecisions.Windows.Forms.CrystalReportViewer crystalReportViewer1;
        private Infragistics.Win.Misc.UltraValidator uvReporteIR;
        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.Misc.UltraPanel frmImportacionesProveedorResumen_Fill_Panel;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmReporteExtractoBancario_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmReporteExtractoBancario_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmReporteExtractoBancario_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmReporteExtractoBancario_UltraFormManager_Dock_Area_Bottom;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtProveedor;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboAlmacen;
        private System.Windows.Forms.Panel pBuscando;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}