namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    partial class frmVentaProdEstablecimiento
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
            System.Windows.Forms.Panel pnTop;
            Infragistics.Win.Misc.UltraGroupBox gbFilter;
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton("btnBuscarArticulo");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmVentaProdEstablecimiento));
            this.btnExcel = new Infragistics.Win.Misc.UltraButton();
            this.label3 = new Infragistics.Win.Misc.UltraLabel();
            this.cboMoneda = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.cboConsiderar = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.label9 = new Infragistics.Win.Misc.UltraLabel();
            this.BtnVuisualizar = new Infragistics.Win.Misc.UltraButton();
            this.txtCodigoProducto = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label7 = new Infragistics.Win.Misc.UltraLabel();
            this.dtpFechaRegistroDe = new System.Windows.Forms.DateTimePicker();
            this.dtpFechaRegistroAl = new System.Windows.Forms.DateTimePicker();
            this.label1 = new Infragistics.Win.Misc.UltraLabel();
            this.label2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraSplitter1 = new Infragistics.Win.Misc.UltraSplitter();
            this.groupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.pBuscando = new System.Windows.Forms.Panel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.crystalReportViewer1 = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.uvDatos = new Infragistics.Win.Misc.UltraValidator(this.components);
            pnTop = new System.Windows.Forms.Panel();
            gbFilter = new Infragistics.Win.Misc.UltraGroupBox();
            pnTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(gbFilter)).BeginInit();
            gbFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboMoneda)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboConsiderar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCodigoProducto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.pBuscando.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uvDatos)).BeginInit();
            this.SuspendLayout();
            // 
            // pnTop
            // 
            pnTop.Controls.Add(gbFilter);
            pnTop.Dock = System.Windows.Forms.DockStyle.Top;
            pnTop.Location = new System.Drawing.Point(0, 0);
            pnTop.Name = "pnTop";
            pnTop.Size = new System.Drawing.Size(1016, 98);
            pnTop.TabIndex = 0;
            // 
            // gbFilter
            // 
            gbFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            gbFilter.Controls.Add(this.btnExcel);
            gbFilter.Controls.Add(this.label3);
            gbFilter.Controls.Add(this.cboMoneda);
            gbFilter.Controls.Add(this.cboConsiderar);
            gbFilter.Controls.Add(this.label9);
            gbFilter.Controls.Add(this.BtnVuisualizar);
            gbFilter.Controls.Add(this.txtCodigoProducto);
            gbFilter.Controls.Add(this.label7);
            gbFilter.Controls.Add(this.dtpFechaRegistroDe);
            gbFilter.Controls.Add(this.dtpFechaRegistroAl);
            gbFilter.Controls.Add(this.label1);
            gbFilter.Controls.Add(this.label2);
            gbFilter.Location = new System.Drawing.Point(3, 4);
            gbFilter.Name = "gbFilter";
            gbFilter.Size = new System.Drawing.Size(1010, 88);
            gbFilter.TabIndex = 0;
            gbFilter.Text = "Filtros";
            // 
            // btnExcel
            // 
            this.btnExcel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.Image = global::SAMBHS.Windows.WinClient.UI.Resource.page_excel1;
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Center;
            this.btnExcel.Appearance = appearance1;
            this.btnExcel.Location = new System.Drawing.Point(885, 40);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(29, 30);
            this.btnExcel.TabIndex = 251;
            this.btnExcel.Click += new System.EventHandler(this.BtnVuisualizar_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColorInternal = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label3.Location = new System.Drawing.Point(28, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 14);
            this.label3.TabIndex = 249;
            this.label3.Text = "Moneda :";
            // 
            // cboMoneda
            // 
            this.cboMoneda.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboMoneda.Location = new System.Drawing.Point(82, 54);
            this.cboMoneda.Name = "cboMoneda";
            this.cboMoneda.Size = new System.Drawing.Size(128, 21);
            this.cboMoneda.TabIndex = 250;
            this.uvDatos.GetValidationSettings(this.cboMoneda).Condition = new Infragistics.Win.OperatorCondition(Infragistics.Win.ConditionOperator.NotEquals, "-1", true, typeof(string));
            this.uvDatos.GetValidationSettings(this.cboMoneda).IsRequired = true;
            // 
            // cboConsiderar
            // 
            this.cboConsiderar.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboConsiderar.Location = new System.Drawing.Point(285, 52);
            this.cboConsiderar.Name = "cboConsiderar";
            this.cboConsiderar.Size = new System.Drawing.Size(128, 21);
            this.cboConsiderar.TabIndex = 247;
            this.uvDatos.GetValidationSettings(this.cboConsiderar).Condition = new Infragistics.Win.OperatorCondition(Infragistics.Win.ConditionOperator.NotEquals, "-1", true, typeof(string));
            this.uvDatos.GetValidationSettings(this.cboConsiderar).IsRequired = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColorInternal = System.Drawing.Color.Transparent;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label9.Location = new System.Drawing.Point(216, 56);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(63, 14);
            this.label9.TabIndex = 248;
            this.label9.Text = "Considerar:";
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
            this.BtnVuisualizar.Location = new System.Drawing.Point(919, 40);
            this.BtnVuisualizar.Margin = new System.Windows.Forms.Padding(2);
            this.BtnVuisualizar.Name = "BtnVuisualizar";
            this.BtnVuisualizar.Size = new System.Drawing.Size(83, 30);
            this.BtnVuisualizar.TabIndex = 214;
            this.BtnVuisualizar.Text = "Visualizar";
            this.BtnVuisualizar.Click += new System.EventHandler(this.BtnVuisualizar_Click);
            // 
            // txtCodigoProducto
            // 
            appearance3.Image = ((object)(resources.GetObject("appearance3.Image")));
            editorButton1.Appearance = appearance3;
            editorButton1.Key = "btnBuscarArticulo";
            this.txtCodigoProducto.ButtonsLeft.Add(editorButton1);
            this.txtCodigoProducto.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCodigoProducto.Location = new System.Drawing.Point(494, 25);
            this.txtCodigoProducto.Name = "txtCodigoProducto";
            this.txtCodigoProducto.NullText = "Código Producto";
            this.txtCodigoProducto.Size = new System.Drawing.Size(128, 21);
            this.txtCodigoProducto.TabIndex = 212;
            this.txtCodigoProducto.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.txtCodigoProducto_EditorButtonClick);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColorInternal = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label7.Location = new System.Drawing.Point(431, 28);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 14);
            this.label7.TabIndex = 213;
            this.label7.Text = "Producto :";
            // 
            // dtpFechaRegistroDe
            // 
            this.dtpFechaRegistroDe.AllowDrop = true;
            this.dtpFechaRegistroDe.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaRegistroDe.Location = new System.Drawing.Point(82, 26);
            this.dtpFechaRegistroDe.Name = "dtpFechaRegistroDe";
            this.dtpFechaRegistroDe.Size = new System.Drawing.Size(128, 20);
            this.dtpFechaRegistroDe.TabIndex = 3;
            // 
            // dtpFechaRegistroAl
            // 
            this.dtpFechaRegistroAl.AllowDrop = true;
            this.dtpFechaRegistroAl.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaRegistroAl.Location = new System.Drawing.Point(285, 26);
            this.dtpFechaRegistroAl.Name = "dtpFechaRegistroAl";
            this.dtpFechaRegistroAl.Size = new System.Drawing.Size(128, 20);
            this.dtpFechaRegistroAl.TabIndex = 5;
            this.dtpFechaRegistroAl.ValueChanged += new System.EventHandler(this.dtpFechaRegistroAl_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColorInternal = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label1.Location = new System.Drawing.Point(19, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 14);
            this.label1.TabIndex = 4;
            this.label1.Text = "Fecha  del:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColorInternal = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label2.Location = new System.Drawing.Point(262, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 14);
            this.label2.TabIndex = 6;
            this.label2.Text = "Al:";
            // 
            // ultraSplitter1
            // 
            this.ultraSplitter1.BackColor = System.Drawing.SystemColors.Control;
            this.ultraSplitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.ultraSplitter1.Location = new System.Drawing.Point(0, 98);
            this.ultraSplitter1.Name = "ultraSplitter1";
            this.ultraSplitter1.RestoreExtent = 116;
            this.ultraSplitter1.Size = new System.Drawing.Size(1016, 6);
            this.ultraSplitter1.TabIndex = 12;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColorInternal = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.pBuscando);
            this.groupBox1.Controls.Add(this.crystalReportViewer1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 104);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1016, 357);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.Text = "Reporte";
            // 
            // pBuscando
            // 
            this.pBuscando.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pBuscando.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pBuscando.Controls.Add(this.ultraLabel2);
            this.pBuscando.Controls.Add(this.pictureBox1);
            this.pBuscando.Location = new System.Drawing.Point(416, 198);
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
            // crystalReportViewer1
            // 
            this.crystalReportViewer1.ActiveViewIndex = -1;
            this.crystalReportViewer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crystalReportViewer1.Cursor = System.Windows.Forms.Cursors.Default;
            this.crystalReportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crystalReportViewer1.Location = new System.Drawing.Point(3, 16);
            this.crystalReportViewer1.Name = "crystalReportViewer1";
            this.crystalReportViewer1.Size = new System.Drawing.Size(1010, 338);
            this.crystalReportViewer1.TabIndex = 2;
            this.crystalReportViewer1.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            // 
            // frmVentaProdEstablecimiento
            // 
            this.AcceptButton = this.BtnVuisualizar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1016, 461);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ultraSplitter1);
            this.Controls.Add(pnTop);
            this.Name = "frmVentaProdEstablecimiento";
            this.Text = "Venta Producto-Establecimiento";
            pnTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(gbFilter)).EndInit();
            gbFilter.ResumeLayout(false);
            gbFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboMoneda)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboConsiderar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCodigoProducto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.pBuscando.ResumeLayout(false);
            this.pBuscando.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uvDatos)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraSplitter ultraSplitter1;
        private System.Windows.Forms.DateTimePicker dtpFechaRegistroDe;
        private System.Windows.Forms.DateTimePicker dtpFechaRegistroAl;
        private Infragistics.Win.Misc.UltraLabel label1;
        private Infragistics.Win.Misc.UltraLabel label2;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtCodigoProducto;
        private Infragistics.Win.Misc.UltraLabel label7;
        private Infragistics.Win.Misc.UltraGroupBox groupBox1;
        private System.Windows.Forms.Panel pBuscando;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private CrystalDecisions.Windows.Forms.CrystalReportViewer crystalReportViewer1;
        private Infragistics.Win.Misc.UltraButton BtnVuisualizar;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboConsiderar;
        private Infragistics.Win.Misc.UltraLabel label9;
        private Infragistics.Win.Misc.UltraLabel label3;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboMoneda;
        private Infragistics.Win.Misc.UltraButton btnExcel;
        private Infragistics.Win.Misc.UltraValidator uvDatos;
    }
}