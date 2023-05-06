namespace SAMBHS.Windows.WinClient.UI.Reportes.Almacen
{
    partial class frmProductoReposicion
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
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            this.groupBox5 = new Infragistics.Win.Misc.UltraGroupBox();
            this.btnExcel = new Infragistics.Win.Misc.UltraButton();
            this.cboLinea = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.cboEstablecimiento = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.label11 = new Infragistics.Win.Misc.UltraLabel();
            this.dtpFechaRegistroDe = new System.Windows.Forms.DateTimePicker();
            this.txtCodigoProducto = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.BtnVuisualizar = new Infragistics.Win.Misc.UltraButton();
            this.dtpFechaRegistroAl = new System.Windows.Forms.DateTimePicker();
            this.label7 = new Infragistics.Win.Misc.UltraLabel();
            this.label1 = new Infragistics.Win.Misc.UltraLabel();
            this.label2 = new Infragistics.Win.Misc.UltraLabel();
            this.label8 = new Infragistics.Win.Misc.UltraLabel();
            this.groupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            this.pBuscando = new System.Windows.Forms.Panel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.crystalReportViewer1 = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox5)).BeginInit();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboLinea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboEstablecimiento)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCodigoProducto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox2)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.pBuscando.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.BackColorInternal = System.Drawing.Color.Transparent;
            this.groupBox5.Controls.Add(this.btnExcel);
            this.groupBox5.Controls.Add(this.cboLinea);
            this.groupBox5.Controls.Add(this.cboEstablecimiento);
            this.groupBox5.Controls.Add(this.label11);
            this.groupBox5.Controls.Add(this.dtpFechaRegistroDe);
            this.groupBox5.Controls.Add(this.txtCodigoProducto);
            this.groupBox5.Controls.Add(this.BtnVuisualizar);
            this.groupBox5.Controls.Add(this.dtpFechaRegistroAl);
            this.groupBox5.Controls.Add(this.label7);
            this.groupBox5.Controls.Add(this.label1);
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Location = new System.Drawing.Point(12, 12);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(1086, 83);
            this.groupBox5.TabIndex = 0;
            this.groupBox5.Text = "Filtro de Búsqueda";
            // 
            // btnExcel
            // 
            this.btnExcel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance4.Image = global::SAMBHS.Windows.WinClient.UI.Resource.page_excel;
            appearance4.ImageHAlign = Infragistics.Win.HAlign.Center;
            this.btnExcel.Appearance = appearance4;
            this.btnExcel.Location = new System.Drawing.Point(943, 45);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(29, 29);
            this.btnExcel.TabIndex = 7;
            this.btnExcel.Click += new System.EventHandler(this.BtnVuisualizar_Click);
            // 
            // cboLinea
            // 
            this.cboLinea.DropDownListWidth = 250;
            this.cboLinea.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboLinea.Location = new System.Drawing.Point(272, 49);
            this.cboLinea.Name = "cboLinea";
            this.cboLinea.Size = new System.Drawing.Size(150, 21);
            this.cboLinea.TabIndex = 5;
            // 
            // cboEstablecimiento
            // 
            this.cboEstablecimiento.DropDownListWidth = 300;
            this.cboEstablecimiento.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboEstablecimiento.Location = new System.Drawing.Point(529, 18);
            this.cboEstablecimiento.Name = "cboEstablecimiento";
            this.cboEstablecimiento.Size = new System.Drawing.Size(152, 21);
            this.cboEstablecimiento.TabIndex = 2;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColorInternal = System.Drawing.Color.Transparent;
            this.label11.Location = new System.Drawing.Point(442, 21);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(87, 14);
            this.label11.TabIndex = 241;
            this.label11.Text = "Establecimiento:";
            // 
            // dtpFechaRegistroDe
            // 
            this.dtpFechaRegistroDe.AllowDrop = true;
            this.dtpFechaRegistroDe.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaRegistroDe.Location = new System.Drawing.Point(63, 16);
            this.dtpFechaRegistroDe.Name = "dtpFechaRegistroDe";
            this.dtpFechaRegistroDe.Size = new System.Drawing.Size(150, 20);
            this.dtpFechaRegistroDe.TabIndex = 0;
            // 
            // txtCodigoProducto
            // 
            appearance2.Image = global::SAMBHS.Windows.WinClient.UI.Resource.system_search;
            editorButton1.Appearance = appearance2;
            this.txtCodigoProducto.ButtonsRight.Add(editorButton1);
            this.txtCodigoProducto.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCodigoProducto.Location = new System.Drawing.Point(63, 49);
            this.txtCodigoProducto.Name = "txtCodigoProducto";
            this.txtCodigoProducto.Size = new System.Drawing.Size(150, 21);
            this.txtCodigoProducto.TabIndex = 3;
            this.txtCodigoProducto.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.TxtProducto_EditorButtonClick);
            // 
            // BtnVuisualizar
            // 
            this.BtnVuisualizar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance5.Image = global::SAMBHS.Windows.WinClient.UI.Resource.eye;
            appearance5.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance5.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance5.TextHAlignAsString = "Center";
            appearance5.TextVAlignAsString = "Middle";
            this.BtnVuisualizar.Appearance = appearance5;
            this.BtnVuisualizar.Location = new System.Drawing.Point(977, 44);
            this.BtnVuisualizar.Margin = new System.Windows.Forms.Padding(2);
            this.BtnVuisualizar.Name = "BtnVuisualizar";
            this.BtnVuisualizar.ShowOutline = false;
            this.BtnVuisualizar.Size = new System.Drawing.Size(104, 30);
            this.BtnVuisualizar.TabIndex = 8;
            this.BtnVuisualizar.Text = "Visualizar";
            this.BtnVuisualizar.Click += new System.EventHandler(this.BtnVuisualizar_Click);
            // 
            // dtpFechaRegistroAl
            // 
            this.dtpFechaRegistroAl.AllowDrop = true;
            this.dtpFechaRegistroAl.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaRegistroAl.Location = new System.Drawing.Point(270, 18);
            this.dtpFechaRegistroAl.Name = "dtpFechaRegistroAl";
            this.dtpFechaRegistroAl.Size = new System.Drawing.Size(152, 20);
            this.dtpFechaRegistroAl.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColorInternal = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label7.Location = new System.Drawing.Point(231, 53);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 14);
            this.label7.TabIndex = 240;
            this.label7.Text = "Linea:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColorInternal = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label1.Location = new System.Drawing.Point(4, 22);
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
            this.label2.Location = new System.Drawing.Point(248, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 14);
            this.label2.TabIndex = 2;
            this.label2.Text = "Al:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColorInternal = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label8.Location = new System.Drawing.Point(6, 52);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 14);
            this.label8.TabIndex = 242;
            this.label8.Text = "Producto:";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.pBuscando);
            this.groupBox2.Controls.Add(this.crystalReportViewer1);
            this.groupBox2.Location = new System.Drawing.Point(12, 101);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1086, 375);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.Text = "Reporte de Stock Consolidado";
            // 
            // pBuscando
            // 
            this.pBuscando.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pBuscando.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pBuscando.Controls.Add(this.ultraLabel2);
            this.pBuscando.Controls.Add(this.pictureBox1);
            this.pBuscando.Location = new System.Drawing.Point(453, 177);
            this.pBuscando.Name = "pBuscando";
            this.pBuscando.Size = new System.Drawing.Size(181, 42);
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
            this.crystalReportViewer1.ShowRefreshButton = false;
            this.crystalReportViewer1.Size = new System.Drawing.Size(1080, 356);
            this.crystalReportViewer1.TabIndex = 5;
            this.crystalReportViewer1.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            // 
            // frmProductoReposicion
            // 
            this.AcceptButton = this.BtnVuisualizar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1110, 488);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox5);
            this.Name = "frmProductoReposicion";
            this.Text = "Reposicion de Mercaderia";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmProductoReposicion_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.groupBox5)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboLinea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboEstablecimiento)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCodigoProducto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox2)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.pBuscando.ResumeLayout(false);
            this.pBuscando.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox groupBox5;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboLinea;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboEstablecimiento;
        private Infragistics.Win.Misc.UltraLabel label11;
        private System.Windows.Forms.DateTimePicker dtpFechaRegistroDe;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtCodigoProducto;
        private Infragistics.Win.Misc.UltraButton BtnVuisualizar;
        private System.Windows.Forms.DateTimePicker dtpFechaRegistroAl;
        private Infragistics.Win.Misc.UltraLabel label7;
        private Infragistics.Win.Misc.UltraLabel label1;
        private Infragistics.Win.Misc.UltraLabel label2;
        private Infragistics.Win.Misc.UltraLabel label8;
        private Infragistics.Win.Misc.UltraButton btnExcel;
        private Infragistics.Win.Misc.UltraGroupBox groupBox2;
        private System.Windows.Forms.Panel pBuscando;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private CrystalDecisions.Windows.Forms.CrystalReportViewer crystalReportViewer1;
    }
}