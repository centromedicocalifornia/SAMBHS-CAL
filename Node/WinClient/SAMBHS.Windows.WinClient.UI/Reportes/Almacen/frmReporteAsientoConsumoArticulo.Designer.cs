namespace SAMBHS.Windows.WinClient.UI.Reportes.Almacen
{
    partial class frmReporteAsientoConsumoArticulo
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
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton("btnBuscarArticulo");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmReporteAsientoConsumoArticulo));
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem3 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this.frmResumenAlmacen_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this.groupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            this.pBuscando = new System.Windows.Forms.Panel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.crystalReportViewer1 = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.groupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.txtArtIni = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label8 = new Infragistics.Win.Misc.UltraLabel();
            this.lblPeriodo = new Infragistics.Win.Misc.UltraLabel();
            this.nupMes = new System.Windows.Forms.NumericUpDown();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.cboTipo = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.label2 = new Infragistics.Win.Misc.UltraLabel();
            this.btnVisualizar = new Infragistics.Win.Misc.UltraButton();
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this.uvValidar = new Infragistics.Win.Misc.UltraValidator(this.components);
            this.chkIncluirNroPedido = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            this.frmResumenAlmacen_Fill_Panel.ClientArea.SuspendLayout();
            this.frmResumenAlmacen_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox2)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.pBuscando.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtArtIni)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupMes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTipo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uvValidar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIncluirNroPedido)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // frmResumenAlmacen_Fill_Panel
            // 
            // 
            // frmResumenAlmacen_Fill_Panel.ClientArea
            // 
            this.frmResumenAlmacen_Fill_Panel.ClientArea.Controls.Add(this.groupBox2);
            this.frmResumenAlmacen_Fill_Panel.ClientArea.Controls.Add(this.groupBox1);
            this.frmResumenAlmacen_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.frmResumenAlmacen_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmResumenAlmacen_Fill_Panel.Location = new System.Drawing.Point(8, 31);
            this.frmResumenAlmacen_Fill_Panel.Name = "frmResumenAlmacen_Fill_Panel";
            this.frmResumenAlmacen_Fill_Panel.Size = new System.Drawing.Size(1270, 574);
            this.frmResumenAlmacen_Fill_Panel.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.pBuscando);
            this.groupBox2.Controls.Add(this.crystalReportViewer1);
            this.groupBox2.Location = new System.Drawing.Point(6, 58);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1254, 508);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.Text = "Reporte de Asiento Consumo por Producto";
            // 
            // pBuscando
            // 
            this.pBuscando.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pBuscando.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pBuscando.Controls.Add(this.ultraLabel2);
            this.pBuscando.Controls.Add(this.pictureBox1);
            this.pBuscando.Location = new System.Drawing.Point(515, 209);
            this.pBuscando.Name = "pBuscando";
            this.pBuscando.Size = new System.Drawing.Size(183, 42);
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
            this.crystalReportViewer1.Size = new System.Drawing.Size(1248, 489);
            this.crystalReportViewer1.TabIndex = 4;
            this.crystalReportViewer1.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.chkIncluirNroPedido);
            this.groupBox1.Controls.Add(this.txtArtIni);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.lblPeriodo);
            this.groupBox1.Controls.Add(this.nupMes);
            this.groupBox1.Controls.Add(this.ultraLabel1);
            this.groupBox1.Controls.Add(this.cboTipo);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btnVisualizar);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1258, 46);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.Text = "Filtro de Búsqueda";
            // 
            // txtArtIni
            // 
            appearance2.Image = ((object)(resources.GetObject("appearance2.Image")));
            editorButton1.Appearance = appearance2;
            editorButton1.Key = "btnBuscarArticulo";
            this.txtArtIni.ButtonsLeft.Add(editorButton1);
            this.txtArtIni.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtArtIni.Location = new System.Drawing.Point(446, 13);
            this.txtArtIni.Name = "txtArtIni";
            this.txtArtIni.NullText = "Código Producto";
            this.txtArtIni.Size = new System.Drawing.Size(145, 21);
            this.txtArtIni.TabIndex = 216;
            this.txtArtIni.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.txtArtIni_EditorButtonClick);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColorInternal = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label8.Location = new System.Drawing.Point(384, 17);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(56, 14);
            this.label8.TabIndex = 215;
            this.label8.Text = "Producto :";
            // 
            // lblPeriodo
            // 
            this.lblPeriodo.AutoSize = true;
            this.lblPeriodo.Location = new System.Drawing.Point(98, 20);
            this.lblPeriodo.Name = "lblPeriodo";
            this.lblPeriodo.Size = new System.Drawing.Size(78, 14);
            this.lblPeriodo.TabIndex = 214;
            this.lblPeriodo.Text = "Periodo : 2016";
            // 
            // nupMes
            // 
            this.nupMes.Location = new System.Drawing.Point(49, 18);
            this.nupMes.Name = "nupMes";
            this.nupMes.Size = new System.Drawing.Size(36, 20);
            this.nupMes.TabIndex = 213;
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(16, 20);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(32, 14);
            this.ultraLabel1.TabIndex = 212;
            this.ultraLabel1.Text = "Mes :";
            // 
            // cboTipo
            // 
            this.cboTipo.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            valueListItem1.DataValue = "-1";
            valueListItem1.DisplayText = "--Seleccionar--";
            valueListItem2.DataValue = "1";
            valueListItem2.DisplayText = "DETALLADO";
            valueListItem3.DataValue = "2";
            valueListItem3.DisplayText = "RESUMEN";
            this.cboTipo.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2,
            valueListItem3});
            this.cboTipo.Location = new System.Drawing.Point(224, 16);
            this.cboTipo.Name = "cboTipo";
            this.cboTipo.Size = new System.Drawing.Size(144, 21);
            this.cboTipo.TabIndex = 2;
            this.uvValidar.GetValidationSettings(this.cboTipo).Condition = new Infragistics.Win.OperatorCondition(Infragistics.Win.ConditionOperator.NotEquals, "-1", true, typeof(string));
            this.uvValidar.GetValidationSettings(this.cboTipo).Enabled = false;
            this.uvValidar.GetValidationSettings(this.cboTipo).IsRequired = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(186, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 14);
            this.label2.TabIndex = 11;
            this.label2.Text = "Tipo :";
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
            this.btnVisualizar.Location = new System.Drawing.Point(1121, 12);
            this.btnVisualizar.Margin = new System.Windows.Forms.Padding(2);
            this.btnVisualizar.Name = "btnVisualizar";
            this.btnVisualizar.Size = new System.Drawing.Size(133, 32);
            this.btnVisualizar.TabIndex = 10;
            this.btnVisualizar.Text = "Visualizar Reporte";
            this.btnVisualizar.Click += new System.EventHandler(this.btnVisualizar_Click);
            // 
            // _frmResumenAlmacen_UltraFormManager_Dock_Area_Left
            // 
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 31);
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Left.Name = "_frmResumenAlmacen_UltraFormManager_Dock_Area_Left";
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 574);
            // 
            // _frmResumenAlmacen_UltraFormManager_Dock_Area_Right
            // 
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(1278, 31);
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Right.Name = "_frmResumenAlmacen_UltraFormManager_Dock_Area_Right";
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 574);
            // 
            // _frmResumenAlmacen_UltraFormManager_Dock_Area_Top
            // 
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Top.Name = "_frmResumenAlmacen_UltraFormManager_Dock_Area_Top";
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(1286, 31);
            // 
            // _frmResumenAlmacen_UltraFormManager_Dock_Area_Bottom
            // 
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 605);
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Bottom.Name = "_frmResumenAlmacen_UltraFormManager_Dock_Area_Bottom";
            this._frmResumenAlmacen_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(1286, 8);
            // 
            // chkIncluirNroPedido
            // 
            this.chkIncluirNroPedido.AutoSize = true;
            this.chkIncluirNroPedido.Location = new System.Drawing.Point(613, 18);
            this.chkIncluirNroPedido.Name = "chkIncluirNroPedido";
            this.chkIncluirNroPedido.Size = new System.Drawing.Size(172, 17);
            this.chkIncluirNroPedido.TabIndex = 269;
            this.chkIncluirNroPedido.Text = "Tomar en cuenta  Nro. Pedido";
            // 
            // frmReporteAsientoConsumoArticulo
            // 
            this.AcceptButton = this.btnVisualizar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(1286, 613);
            this.Controls.Add(this.frmResumenAlmacen_Fill_Panel);
            this.Controls.Add(this._frmResumenAlmacen_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._frmResumenAlmacen_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._frmResumenAlmacen_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._frmResumenAlmacen_UltraFormManager_Dock_Area_Bottom);
            this.Name = "frmReporteAsientoConsumoArticulo";
            this.ShowIcon = false;
            this.Text = "Reporte de Asiento Consumo por Producto";
            this.Load += new System.EventHandler(this.frmResumenAlmacen_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            this.frmResumenAlmacen_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmResumenAlmacen_Fill_Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupBox2)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.pBuscando.ResumeLayout(false);
            this.pBuscando.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtArtIni)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupMes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTipo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uvValidar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIncluirNroPedido)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.Misc.UltraPanel frmResumenAlmacen_Fill_Panel;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmResumenAlmacen_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmResumenAlmacen_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmResumenAlmacen_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmResumenAlmacen_UltraFormManager_Dock_Area_Bottom;
        private Infragistics.Win.Misc.UltraGroupBox groupBox1;
        private Infragistics.Win.Misc.UltraLabel label2;
        private Infragistics.Win.Misc.UltraButton btnVisualizar;
        private Infragistics.Win.Misc.UltraGroupBox groupBox2;
        private Infragistics.Win.Misc.UltraValidator uvValidar;
        private CrystalDecisions.Windows.Forms.CrystalReportViewer crystalReportViewer1;
        private System.Windows.Forms.Panel pBuscando;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private Infragistics.Win.Misc.UltraLabel lblPeriodo;
        private System.Windows.Forms.NumericUpDown nupMes;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboTipo;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtArtIni;
        private Infragistics.Win.Misc.UltraLabel label8;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkIncluirNroPedido;
    }
}