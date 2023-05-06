namespace SAMBHS.Windows.WinClient.UI.Reportes.Compras
{
    partial class frmReporteOrdenCompraEstado
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
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton("btnBuscarArticulo");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmReporteOrdenCompraEstado));
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            this.pnTop = new System.Windows.Forms.Panel();
            this.gbFilter = new Infragistics.Win.Misc.UltraGroupBox();
            this.txtProveedor = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.btnVisualizar = new Infragistics.Win.Misc.UltraButton();
            this.label2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.dtpFechaRegistroAl = new System.Windows.Forms.DateTimePicker();
            this.dtpFechaRegistroDe = new System.Windows.Forms.DateTimePicker();
            this.ultraSplitter1 = new Infragistics.Win.Misc.UltraSplitter();
            this.crystalReportViewer1 = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.pBuscando = new System.Windows.Forms.Panel();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pnTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gbFilter)).BeginInit();
            this.gbFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtProveedor)).BeginInit();
            this.pBuscando.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pnTop
            // 
            this.pnTop.Controls.Add(this.gbFilter);
            this.pnTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnTop.Location = new System.Drawing.Point(0, 0);
            this.pnTop.Name = "pnTop";
            this.pnTop.Size = new System.Drawing.Size(901, 60);
            this.pnTop.TabIndex = 0;
            // 
            // gbFilter
            // 
            this.gbFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbFilter.Controls.Add(this.txtProveedor);
            this.gbFilter.Controls.Add(this.ultraLabel2);
            this.gbFilter.Controls.Add(this.btnVisualizar);
            this.gbFilter.Controls.Add(this.label2);
            this.gbFilter.Controls.Add(this.ultraLabel5);
            this.gbFilter.Controls.Add(this.dtpFechaRegistroAl);
            this.gbFilter.Controls.Add(this.dtpFechaRegistroDe);
            this.gbFilter.Location = new System.Drawing.Point(3, 4);
            this.gbFilter.Name = "gbFilter";
            this.gbFilter.Size = new System.Drawing.Size(895, 50);
            this.gbFilter.TabIndex = 0;
            this.gbFilter.Text = "Busqueda";
            // 
            // txtProveedor
            // 
            appearance1.Image = ((object)(resources.GetObject("appearance1.Image")));
            editorButton1.Appearance = appearance1;
            editorButton1.Key = "btnBuscarArticulo";
            this.txtProveedor.ButtonsLeft.Add(editorButton1);
            this.txtProveedor.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtProveedor.Location = new System.Drawing.Point(393, 18);
            this.txtProveedor.Name = "txtProveedor";
            this.txtProveedor.Size = new System.Drawing.Size(122, 21);
            this.txtProveedor.TabIndex = 263;
            this.txtProveedor.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.txtProveedor_EditorButtonClick);
            this.txtProveedor.Validated += new System.EventHandler(this.txtProveedor_Validated);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.BackColorInternal = System.Drawing.Color.Transparent;
            this.ultraLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraLabel2.ForeColor = System.Drawing.SystemColors.MenuText;
            this.ultraLabel2.Location = new System.Drawing.Point(324, 22);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(63, 14);
            this.ultraLabel2.TabIndex = 264;
            this.ultraLabel2.Text = "Proveedor :";
            // 
            // btnVisualizar
            // 
            this.btnVisualizar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance2.Image = global::SAMBHS.Windows.WinClient.UI.Resource.eye;
            appearance2.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance2.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance2.TextHAlignAsString = "Right";
            appearance2.TextVAlignAsString = "Middle";
            this.btnVisualizar.Appearance = appearance2;
            this.btnVisualizar.Location = new System.Drawing.Point(762, 14);
            this.btnVisualizar.Margin = new System.Windows.Forms.Padding(2);
            this.btnVisualizar.Name = "btnVisualizar";
            this.btnVisualizar.Size = new System.Drawing.Size(125, 30);
            this.btnVisualizar.TabIndex = 8;
            this.btnVisualizar.Text = "Visualizar Reporte";
            this.btnVisualizar.Click += new System.EventHandler(this.btnVisualizar_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColorInternal = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label2.Location = new System.Drawing.Point(188, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 14);
            this.label2.TabIndex = 6;
            this.label2.Text = "Al:";
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.AutoSize = true;
            this.ultraLabel5.BackColorInternal = System.Drawing.Color.Transparent;
            this.ultraLabel5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraLabel5.ForeColor = System.Drawing.SystemColors.MenuText;
            this.ultraLabel5.Location = new System.Drawing.Point(18, 22);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(60, 14);
            this.ultraLabel5.TabIndex = 3;
            this.ultraLabel5.Text = "Fecha  del:";
            // 
            // dtpFechaRegistroAl
            // 
            this.dtpFechaRegistroAl.AllowDrop = true;
            this.dtpFechaRegistroAl.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaRegistroAl.Location = new System.Drawing.Point(213, 19);
            this.dtpFechaRegistroAl.Name = "dtpFechaRegistroAl";
            this.dtpFechaRegistroAl.Size = new System.Drawing.Size(100, 20);
            this.dtpFechaRegistroAl.TabIndex = 5;
            // 
            // dtpFechaRegistroDe
            // 
            this.dtpFechaRegistroDe.AllowDrop = true;
            this.dtpFechaRegistroDe.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaRegistroDe.Location = new System.Drawing.Point(84, 19);
            this.dtpFechaRegistroDe.Name = "dtpFechaRegistroDe";
            this.dtpFechaRegistroDe.Size = new System.Drawing.Size(98, 20);
            this.dtpFechaRegistroDe.TabIndex = 4;
            // 
            // ultraSplitter1
            // 
            this.ultraSplitter1.BackColor = System.Drawing.SystemColors.Control;
            this.ultraSplitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.ultraSplitter1.Location = new System.Drawing.Point(0, 60);
            this.ultraSplitter1.Name = "ultraSplitter1";
            this.ultraSplitter1.RestoreExtent = 88;
            this.ultraSplitter1.Size = new System.Drawing.Size(901, 6);
            this.ultraSplitter1.TabIndex = 1;
            // 
            // crystalReportViewer1
            // 
            this.crystalReportViewer1.ActiveViewIndex = -1;
            this.crystalReportViewer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crystalReportViewer1.Cursor = System.Windows.Forms.Cursors.Default;
            this.crystalReportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crystalReportViewer1.Location = new System.Drawing.Point(0, 66);
            this.crystalReportViewer1.Name = "crystalReportViewer1";
            this.crystalReportViewer1.Size = new System.Drawing.Size(901, 388);
            this.crystalReportViewer1.TabIndex = 6;
            this.crystalReportViewer1.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            // 
            // pBuscando
            // 
            this.pBuscando.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pBuscando.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pBuscando.Controls.Add(this.ultraLabel1);
            this.pBuscando.Controls.Add(this.pictureBox1);
            this.pBuscando.Location = new System.Drawing.Point(358, 230);
            this.pBuscando.Name = "pBuscando";
            this.pBuscando.Size = new System.Drawing.Size(185, 42);
            this.pBuscando.TabIndex = 10;
            this.pBuscando.Visible = false;
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.ultraLabel1.Location = new System.Drawing.Point(53, 13);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(134, 17);
            this.ultraLabel1.TabIndex = 1;
            this.ultraLabel1.Text = "Generando Reporte...";
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
            // frmReporteOrdenCompraEstado
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(901, 454);
            this.Controls.Add(this.pBuscando);
            this.Controls.Add(this.crystalReportViewer1);
            this.Controls.Add(this.ultraSplitter1);
            this.Controls.Add(this.pnTop);
            this.Name = "frmReporteOrdenCompraEstado";
            this.Text = "Reporte Orden Compra Estado";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmReporteOrdenCompraEstado_FormClosing);
            this.pnTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gbFilter)).EndInit();
            this.gbFilter.ResumeLayout(false);
            this.gbFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtProveedor)).EndInit();
            this.pBuscando.ResumeLayout(false);
            this.pBuscando.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnTop;
        private Infragistics.Win.Misc.UltraSplitter ultraSplitter1;
        private CrystalDecisions.Windows.Forms.CrystalReportViewer crystalReportViewer1;
        private System.Windows.Forms.Panel pBuscando;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private Infragistics.Win.Misc.UltraGroupBox gbFilter;
        private Infragistics.Win.Misc.UltraLabel label2;
        private Infragistics.Win.Misc.UltraLabel ultraLabel5;
        private System.Windows.Forms.DateTimePicker dtpFechaRegistroAl;
        private System.Windows.Forms.DateTimePicker dtpFechaRegistroDe;
        private Infragistics.Win.Misc.UltraButton btnVisualizar;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtProveedor;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
    }
}