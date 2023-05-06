namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    partial class frmReporteCuadresCajasChicas
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
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this.frmReporteVentasDetraccion_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this.ultraExpandableGroupBox1 = new Infragistics.Win.Misc.UltraExpandableGroupBox();
            this.ultraExpandableGroupBoxPanel1 = new Infragistics.Win.Misc.UltraExpandableGroupBoxPanel();
            this.btnVisualizar = new Infragistics.Win.Misc.UltraButton();
            this.dtpFechaRegistroDe = new System.Windows.Forms.DateTimePicker();
            this.dtpFechaRegistroAl = new System.Windows.Forms.DateTimePicker();
            this.label1 = new Infragistics.Win.Misc.UltraLabel();
            this.label2 = new Infragistics.Win.Misc.UltraLabel();
            this.groupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.pBuscando = new System.Windows.Forms.Panel();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.crystalReportViewer1 = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            this.frmReporteVentasDetraccion_Fill_Panel.ClientArea.SuspendLayout();
            this.frmReporteVentasDetraccion_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraExpandableGroupBox1)).BeginInit();
            this.ultraExpandableGroupBox1.SuspendLayout();
            this.ultraExpandableGroupBoxPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.pBuscando.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // frmReporteVentasDetraccion_Fill_Panel
            // 
            // 
            // frmReporteVentasDetraccion_Fill_Panel.ClientArea
            // 
            this.frmReporteVentasDetraccion_Fill_Panel.ClientArea.Controls.Add(this.ultraExpandableGroupBox1);
            this.frmReporteVentasDetraccion_Fill_Panel.ClientArea.Controls.Add(this.groupBox1);
            this.frmReporteVentasDetraccion_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmReporteVentasDetraccion_Fill_Panel.Location = new System.Drawing.Point(8, 32);
            this.frmReporteVentasDetraccion_Fill_Panel.Name = "frmReporteVentasDetraccion_Fill_Panel";
            this.frmReporteVentasDetraccion_Fill_Panel.Size = new System.Drawing.Size(1052, 573);
            this.frmReporteVentasDetraccion_Fill_Panel.TabIndex = 0;
            // 
            // ultraExpandableGroupBox1
            // 
            this.ultraExpandableGroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraExpandableGroupBox1.Controls.Add(this.ultraExpandableGroupBoxPanel1);
            this.ultraExpandableGroupBox1.ExpandedSize = new System.Drawing.Size(1037, 55);
            this.ultraExpandableGroupBox1.Location = new System.Drawing.Point(6, 6);
            this.ultraExpandableGroupBox1.Name = "ultraExpandableGroupBox1";
            this.ultraExpandableGroupBox1.Size = new System.Drawing.Size(1037, 55);
            this.ultraExpandableGroupBox1.TabIndex = 10;
            this.ultraExpandableGroupBox1.Text = "Filtros de Búsqueda";
            // 
            // ultraExpandableGroupBoxPanel1
            // 
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.btnVisualizar);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.dtpFechaRegistroDe);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.dtpFechaRegistroAl);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.label1);
            this.ultraExpandableGroupBoxPanel1.Controls.Add(this.label2);
            this.ultraExpandableGroupBoxPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraExpandableGroupBoxPanel1.Location = new System.Drawing.Point(3, 19);
            this.ultraExpandableGroupBoxPanel1.Name = "ultraExpandableGroupBoxPanel1";
            this.ultraExpandableGroupBoxPanel1.Size = new System.Drawing.Size(1031, 33);
            this.ultraExpandableGroupBoxPanel1.TabIndex = 0;
            // 
            // btnVisualizar
            // 
            this.btnVisualizar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.Image = global::SAMBHS.Windows.WinClient.UI.Resource.eye;
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance1.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance1.TextHAlignAsString = "Right";
            appearance1.TextVAlignAsString = "Middle";
            this.btnVisualizar.Appearance = appearance1;
            this.btnVisualizar.Location = new System.Drawing.Point(904, -1);
            this.btnVisualizar.Margin = new System.Windows.Forms.Padding(2);
            this.btnVisualizar.Name = "btnVisualizar";
            this.btnVisualizar.Size = new System.Drawing.Size(125, 30);
            this.btnVisualizar.TabIndex = 7;
            this.btnVisualizar.Text = "Visualizar Reporte";
            this.btnVisualizar.Click += new System.EventHandler(this.btnVisualizar_Click);
            // 
            // dtpFechaRegistroDe
            // 
            this.dtpFechaRegistroDe.AllowDrop = true;
            this.dtpFechaRegistroDe.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaRegistroDe.Location = new System.Drawing.Point(65, 7);
            this.dtpFechaRegistroDe.Name = "dtpFechaRegistroDe";
            this.dtpFechaRegistroDe.Size = new System.Drawing.Size(98, 20);
            this.dtpFechaRegistroDe.TabIndex = 0;
            // 
            // dtpFechaRegistroAl
            // 
            this.dtpFechaRegistroAl.AllowDrop = true;
            this.dtpFechaRegistroAl.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaRegistroAl.Location = new System.Drawing.Point(192, 7);
            this.dtpFechaRegistroAl.Name = "dtpFechaRegistroAl";
            this.dtpFechaRegistroAl.Size = new System.Drawing.Size(100, 20);
            this.dtpFechaRegistroAl.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColorInternal = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label1.Location = new System.Drawing.Point(2, 9);
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
            this.label2.Location = new System.Drawing.Point(169, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 14);
            this.label2.TabIndex = 2;
            this.label2.Text = "Al:";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.BackColorInternal = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.pBuscando);
            this.groupBox1.Controls.Add(this.crystalReportViewer1);
            this.groupBox1.Location = new System.Drawing.Point(6, 67);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1040, 495);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.Text = "Reportes Cuadres de Caja Chica";
            // 
            // pBuscando
            // 
            this.pBuscando.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pBuscando.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pBuscando.Controls.Add(this.ultraLabel1);
            this.pBuscando.Controls.Add(this.pictureBox1);
            this.pBuscando.Location = new System.Drawing.Point(428, 201);
            this.pBuscando.Name = "pBuscando";
            this.pBuscando.Size = new System.Drawing.Size(185, 42);
            this.pBuscando.TabIndex = 9;
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
            // crystalReportViewer1
            // 
            this.crystalReportViewer1.ActiveViewIndex = -1;
            this.crystalReportViewer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crystalReportViewer1.Cursor = System.Windows.Forms.Cursors.Default;
            this.crystalReportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crystalReportViewer1.Location = new System.Drawing.Point(3, 16);
            this.crystalReportViewer1.Name = "crystalReportViewer1";
            this.crystalReportViewer1.Size = new System.Drawing.Size(1034, 476);
            this.crystalReportViewer1.TabIndex = 5;
            this.crystalReportViewer1.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            // 
            // _frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Left
            // 
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 32);
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Left.Name = "_frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Left";
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 573);
            // 
            // _frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Right
            // 
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(1060, 32);
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Right.Name = "_frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Right";
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 573);
            // 
            // _frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Top
            // 
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Top.Name = "_frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Top";
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(1068, 32);
            // 
            // _frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Bottom
            // 
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 605);
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Bottom.Name = "_frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Bottom";
            this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(1068, 8);
            // 
            // frmReporteCuadresCajasChicas
            // 
            this.AcceptButton = this.btnVisualizar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1068, 613);
            this.Controls.Add(this.frmReporteVentasDetraccion_Fill_Panel);
            this.Controls.Add(this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Bottom);
            this.Name = "frmReporteCuadresCajasChicas";
            this.ShowIcon = false;
            this.Text = "Reportes Cuadres de Caja Chica";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmReporteCuadresCajasChicas_FormClosing);
            this.Load += new System.EventHandler(this.frmReporteCuadresCajasChicas_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            this.frmReporteVentasDetraccion_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmReporteVentasDetraccion_Fill_Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraExpandableGroupBox1)).EndInit();
            this.ultraExpandableGroupBox1.ResumeLayout(false);
            this.ultraExpandableGroupBoxPanel1.ResumeLayout(false);
            this.ultraExpandableGroupBoxPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.pBuscando.ResumeLayout(false);
            this.pBuscando.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.Misc.UltraPanel frmReporteVentasDetraccion_Fill_Panel;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmReporteVentasDetraccion_UltraFormManager_Dock_Area_Bottom;
        private Infragistics.Win.Misc.UltraButton btnVisualizar;
        private Infragistics.Win.Misc.UltraLabel label2;
        private Infragistics.Win.Misc.UltraLabel label1;
        private System.Windows.Forms.DateTimePicker dtpFechaRegistroAl;
        private System.Windows.Forms.DateTimePicker dtpFechaRegistroDe;
        private Infragistics.Win.Misc.UltraGroupBox groupBox1;
        private System.Windows.Forms.Panel pBuscando;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private CrystalDecisions.Windows.Forms.CrystalReportViewer crystalReportViewer1;
        private Infragistics.Win.Misc.UltraExpandableGroupBox ultraExpandableGroupBox1;
        private Infragistics.Win.Misc.UltraExpandableGroupBoxPanel ultraExpandableGroupBoxPanel1;

    }
}