namespace SAMBHS.Windows.WinClient.UI.Reportes.Almacen
{
    partial class frmProducto
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
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton("btnBuscarArticulo");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmProducto));
            this.uvDatos = new Infragistics.Win.Misc.UltraValidator(this.components);
            this.cboOrden = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.label12 = new Infragistics.Win.Misc.UltraLabel();
            this.groupBox5 = new Infragistics.Win.Misc.UltraGroupBox();
            this.cboAgrupamiento = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.cboLinea = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.BtnVuisualizar = new Infragistics.Win.Misc.UltraButton();
            this.TxtProducto = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.chkHoraimpresion = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.label7 = new Infragistics.Win.Misc.UltraLabel();
            this.label8 = new Infragistics.Win.Misc.UltraLabel();
            this.label6 = new Infragistics.Win.Misc.UltraLabel();
            this.groupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.pBuscando = new System.Windows.Forms.Panel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.crystalReportViewer1 = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this.frmProducto_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this._frmProducto_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmProducto_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmProducto_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmProducto_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            ((System.ComponentModel.ISupportInitialize)(this.uvDatos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboOrden)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox5)).BeginInit();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboAgrupamiento)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboLinea)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TxtProducto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkHoraimpresion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.pBuscando.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            this.frmProducto_Fill_Panel.ClientArea.SuspendLayout();
            this.frmProducto_Fill_Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // cboOrden
            // 
            this.cboOrden.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboOrden.Location = new System.Drawing.Point(510, 22);
            this.cboOrden.Name = "cboOrden";
            this.cboOrden.Size = new System.Drawing.Size(149, 21);
            this.cboOrden.TabIndex = 2;
            this.uvDatos.GetValidationSettings(this.cboOrden).Condition = new Infragistics.Win.OperatorCondition(Infragistics.Win.ConditionOperator.NotEquals, "-1", true, typeof(string));
            this.uvDatos.GetValidationSettings(this.cboOrden).Enabled = false;
            this.uvDatos.GetValidationSettings(this.cboOrden).IsRequired = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.BackColorInternal = System.Drawing.Color.Transparent;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(6, 26);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(76, 14);
            this.label12.TabIndex = 245;
            this.label12.Text = "Agrupado por:";
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.BackColorInternal = System.Drawing.Color.Transparent;
            this.groupBox5.Controls.Add(this.cboAgrupamiento);
            this.groupBox5.Controls.Add(this.cboLinea);
            this.groupBox5.Controls.Add(this.cboOrden);
            this.groupBox5.Controls.Add(this.BtnVuisualizar);
            this.groupBox5.Controls.Add(this.TxtProducto);
            this.groupBox5.Controls.Add(this.label12);
            this.groupBox5.Controls.Add(this.chkHoraimpresion);
            this.groupBox5.Controls.Add(this.label7);
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Controls.Add(this.label6);
            this.groupBox5.Location = new System.Drawing.Point(13, 5);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(1030, 55);
            this.groupBox5.TabIndex = 0;
            this.groupBox5.Text = "Filtro de Búsqueda";
            // 
            // cboAgrupamiento
            // 
            this.cboAgrupamiento.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboAgrupamiento.Location = new System.Drawing.Point(88, 22);
            this.cboAgrupamiento.Name = "cboAgrupamiento";
            this.cboAgrupamiento.Size = new System.Drawing.Size(144, 21);
            this.cboAgrupamiento.TabIndex = 246;
            // 
            // cboLinea
            // 
            this.cboLinea.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboLinea.Location = new System.Drawing.Point(280, 22);
            this.cboLinea.Name = "cboLinea";
            this.cboLinea.Size = new System.Drawing.Size(149, 21);
            this.cboLinea.TabIndex = 1;
            // 
            // BtnVuisualizar
            // 
            this.BtnVuisualizar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.Image = global::SAMBHS.Windows.WinClient.UI.Resource.eye;
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance1.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance1.TextHAlignAsString = "Right";
            appearance1.TextVAlignAsString = "Middle";
            this.BtnVuisualizar.Appearance = appearance1;
            this.BtnVuisualizar.Location = new System.Drawing.Point(902, 15);
            this.BtnVuisualizar.Margin = new System.Windows.Forms.Padding(2);
            this.BtnVuisualizar.Name = "BtnVuisualizar";
            this.BtnVuisualizar.Size = new System.Drawing.Size(126, 30);
            this.BtnVuisualizar.TabIndex = 5;
            this.BtnVuisualizar.Text = "Visualizar Reporte";
            this.BtnVuisualizar.Click += new System.EventHandler(this.BtnVuisualizar_Click);
            // 
            // TxtProducto
            // 
            appearance2.Image = ((object)(resources.GetObject("appearance2.Image")));
            editorButton1.Appearance = appearance2;
            editorButton1.Key = "btnBuscarArticulo";
            this.TxtProducto.ButtonsLeft.Add(editorButton1);
            this.TxtProducto.Location = new System.Drawing.Point(730, 22);
            this.TxtProducto.Name = "TxtProducto";
            this.TxtProducto.Size = new System.Drawing.Size(109, 21);
            this.TxtProducto.TabIndex = 3;
            this.TxtProducto.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.TxtProducto_EditorButtonClick);
            // 
            // chkHoraimpresion
            // 
            this.chkHoraimpresion.AutoSize = true;
            this.chkHoraimpresion.BackColor = System.Drawing.Color.Transparent;
            this.chkHoraimpresion.BackColorInternal = System.Drawing.Color.Transparent;
            this.chkHoraimpresion.Checked = true;
            this.chkHoraimpresion.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkHoraimpresion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkHoraimpresion.Location = new System.Drawing.Point(867, 23);
            this.chkHoraimpresion.Name = "chkHoraimpresion";
            this.chkHoraimpresion.Size = new System.Drawing.Size(157, 17);
            this.chkHoraimpresion.TabIndex = 4;
            this.chkHoraimpresion.Text = "Fecha y Hora de Impresión ";
            this.chkHoraimpresion.UseFlatMode = Infragistics.Win.DefaultableBoolean.True;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColorInternal = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label7.Location = new System.Drawing.Point(242, 26);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 14);
            this.label7.TabIndex = 240;
            this.label7.Text = "Linea:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColorInternal = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label8.Location = new System.Drawing.Point(673, 26);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 14);
            this.label8.TabIndex = 242;
            this.label8.Text = "Producto:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColorInternal = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(442, 26);
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
            this.groupBox1.Location = new System.Drawing.Point(12, 66);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1030, 467);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.Text = "Reporte de Producto";
            // 
            // pBuscando
            // 
            this.pBuscando.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pBuscando.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pBuscando.Controls.Add(this.ultraLabel2);
            this.pBuscando.Controls.Add(this.pictureBox1);
            this.pBuscando.Location = new System.Drawing.Point(419, 199);
            this.pBuscando.Name = "pBuscando";
            this.pBuscando.Size = new System.Drawing.Size(193, 42);
            this.pBuscando.TabIndex = 5;
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
            this.crystalReportViewer1.Size = new System.Drawing.Size(1024, 448);
            this.crystalReportViewer1.TabIndex = 0;
            this.crystalReportViewer1.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // frmProducto_Fill_Panel
            // 
            // 
            // frmProducto_Fill_Panel.ClientArea
            // 
            this.frmProducto_Fill_Panel.ClientArea.Controls.Add(this.groupBox5);
            this.frmProducto_Fill_Panel.ClientArea.Controls.Add(this.groupBox1);
            this.frmProducto_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.frmProducto_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmProducto_Fill_Panel.Location = new System.Drawing.Point(8, 32);
            this.frmProducto_Fill_Panel.Name = "frmProducto_Fill_Panel";
            this.frmProducto_Fill_Panel.Size = new System.Drawing.Size(1054, 546);
            this.frmProducto_Fill_Panel.TabIndex = 0;
            // 
            // _frmProducto_UltraFormManager_Dock_Area_Left
            // 
            this._frmProducto_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmProducto_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmProducto_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._frmProducto_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmProducto_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._frmProducto_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmProducto_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 32);
            this._frmProducto_UltraFormManager_Dock_Area_Left.Name = "_frmProducto_UltraFormManager_Dock_Area_Left";
            this._frmProducto_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 546);
            // 
            // _frmProducto_UltraFormManager_Dock_Area_Right
            // 
            this._frmProducto_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmProducto_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmProducto_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._frmProducto_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmProducto_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._frmProducto_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmProducto_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(1062, 32);
            this._frmProducto_UltraFormManager_Dock_Area_Right.Name = "_frmProducto_UltraFormManager_Dock_Area_Right";
            this._frmProducto_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 546);
            // 
            // _frmProducto_UltraFormManager_Dock_Area_Top
            // 
            this._frmProducto_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmProducto_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmProducto_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._frmProducto_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmProducto_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._frmProducto_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmProducto_UltraFormManager_Dock_Area_Top.Name = "_frmProducto_UltraFormManager_Dock_Area_Top";
            this._frmProducto_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(1070, 32);
            // 
            // _frmProducto_UltraFormManager_Dock_Area_Bottom
            // 
            this._frmProducto_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmProducto_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmProducto_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._frmProducto_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmProducto_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._frmProducto_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmProducto_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 578);
            this._frmProducto_UltraFormManager_Dock_Area_Bottom.Name = "_frmProducto_UltraFormManager_Dock_Area_Bottom";
            this._frmProducto_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(1070, 8);
            // 
            // frmProducto
            // 
            this.AcceptButton = this.BtnVuisualizar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(1070, 586);
            this.Controls.Add(this.frmProducto_Fill_Panel);
            this.Controls.Add(this._frmProducto_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._frmProducto_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._frmProducto_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._frmProducto_UltraFormManager_Dock_Area_Bottom);
            this.Name = "frmProducto";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reporte de Producto";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmProducto_FormClosing);
            this.Load += new System.EventHandler(this.frmProducto_Load);
            ((System.ComponentModel.ISupportInitialize)(this.uvDatos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboOrden)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox5)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboAgrupamiento)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboLinea)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TxtProducto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkHoraimpresion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.pBuscando.ResumeLayout(false);
            this.pBuscando.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            this.frmProducto_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmProducto_Fill_Panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraValidator uvDatos;
        private Infragistics.Win.Misc.UltraLabel label12;
        private Infragistics.Win.Misc.UltraGroupBox groupBox5;
        private Infragistics.Win.Misc.UltraButton BtnVuisualizar;
        private Infragistics.Win.Misc.UltraLabel label7;
        private Infragistics.Win.Misc.UltraLabel label8;
        private Infragistics.Win.Misc.UltraLabel label6;
        private Infragistics.Win.Misc.UltraGroupBox groupBox1;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkHoraimpresion;
        private CrystalDecisions.Windows.Forms.CrystalReportViewer crystalReportViewer1;
        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.Misc.UltraPanel frmProducto_Fill_Panel;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmProducto_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmProducto_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmProducto_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmProducto_UltraFormManager_Dock_Area_Bottom;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor TxtProducto;
        private System.Windows.Forms.Panel pBuscando;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboLinea;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboOrden;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboAgrupamiento;
    }
}