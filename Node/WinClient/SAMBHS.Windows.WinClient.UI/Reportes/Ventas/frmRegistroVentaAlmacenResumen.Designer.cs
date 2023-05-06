namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    partial class frmRegistroVentaAlmacenResumen
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
            this.groupBox5 = new Infragistics.Win.Misc.UltraGroupBox();
            this.cboAlmacen = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.label4 = new Infragistics.Win.Misc.UltraLabel();
            this.BtnVuisualizar = new Infragistics.Win.Misc.UltraButton();
            this.label2 = new Infragistics.Win.Misc.UltraLabel();
            this.label1 = new Infragistics.Win.Misc.UltraLabel();
            this.chkHoraimpresion = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.dtpFechaRegistroAl = new System.Windows.Forms.DateTimePicker();
            this.dtpFechaRegistroDe = new System.Windows.Forms.DateTimePicker();
            this.groupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.TxtCodigoProducto = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.uvDatos = new Infragistics.Win.Misc.UltraValidator(this.components);
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this.frmRegistroVentaAlmacenResumen_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox5)).BeginInit();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboAlmacen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkHoraimpresion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uvDatos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            this.frmRegistroVentaAlmacenResumen_Fill_Panel.ClientArea.SuspendLayout();
            this.frmRegistroVentaAlmacenResumen_Fill_Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.BackColorInternal = System.Drawing.Color.Transparent;
            this.groupBox5.Controls.Add(this.cboAlmacen);
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Controls.Add(this.BtnVuisualizar);
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Controls.Add(this.label1);
            this.groupBox5.Controls.Add(this.chkHoraimpresion);
            this.groupBox5.Controls.Add(this.dtpFechaRegistroAl);
            this.groupBox5.Controls.Add(this.dtpFechaRegistroDe);
            this.groupBox5.Location = new System.Drawing.Point(12, 2);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(1017, 66);
            this.groupBox5.TabIndex = 23;
            this.groupBox5.Text = "Filtro de Búsqueda";
            // 
            // cboAlmacen
            // 
            this.cboAlmacen.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboAlmacen.Location = new System.Drawing.Point(391, 18);
            this.cboAlmacen.Name = "cboAlmacen";
            this.cboAlmacen.Size = new System.Drawing.Size(150, 21);
            this.cboAlmacen.TabIndex = 2;
            this.uvDatos.GetValidationSettings(this.cboAlmacen).Condition = new Infragistics.Win.OperatorCondition(Infragistics.Win.ConditionOperator.NotEquals, "-1", true, typeof(string));
            this.uvDatos.GetValidationSettings(this.cboAlmacen).Enabled = false;
            this.uvDatos.GetValidationSettings(this.cboAlmacen).IsRequired = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColorInternal = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label4.Location = new System.Drawing.Point(336, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 14);
            this.label4.TabIndex = 242;
            this.label4.Text = "Almacén:";
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
            this.BtnVuisualizar.Location = new System.Drawing.Point(883, 16);
            this.BtnVuisualizar.Margin = new System.Windows.Forms.Padding(2);
            this.BtnVuisualizar.Name = "BtnVuisualizar";
            this.BtnVuisualizar.Size = new System.Drawing.Size(128, 30);
            this.BtnVuisualizar.TabIndex = 4;
            this.BtnVuisualizar.Text = "Visualizar Reporte";
            this.BtnVuisualizar.Click += new System.EventHandler(this.BtnVuisualizar_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColorInternal = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.MenuText;
            this.label2.Location = new System.Drawing.Point(190, 22);
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
            this.label1.Location = new System.Drawing.Point(23, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "Fecha  del:";
            // 
            // chkHoraimpresion
            // 
            this.chkHoraimpresion.AutoSize = true;
            this.chkHoraimpresion.BackColor = System.Drawing.Color.Transparent;
            this.chkHoraimpresion.BackColorInternal = System.Drawing.Color.Transparent;
            this.chkHoraimpresion.Checked = true;
            this.chkHoraimpresion.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkHoraimpresion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkHoraimpresion.Location = new System.Drawing.Point(561, 19);
            this.chkHoraimpresion.Name = "chkHoraimpresion";
            this.chkHoraimpresion.Size = new System.Drawing.Size(157, 17);
            this.chkHoraimpresion.TabIndex = 3;
            this.chkHoraimpresion.Text = "Fecha y Hora de Impresión ";
            // 
            // dtpFechaRegistroAl
            // 
            this.dtpFechaRegistroAl.AllowDrop = true;
            this.dtpFechaRegistroAl.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaRegistroAl.Location = new System.Drawing.Point(215, 19);
            this.dtpFechaRegistroAl.Name = "dtpFechaRegistroAl";
            this.dtpFechaRegistroAl.Size = new System.Drawing.Size(99, 20);
            this.dtpFechaRegistroAl.TabIndex = 1;
            // 
            // dtpFechaRegistroDe
            // 
            this.dtpFechaRegistroDe.AllowDrop = true;
            this.dtpFechaRegistroDe.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaRegistroDe.Location = new System.Drawing.Point(86, 19);
            this.dtpFechaRegistroDe.Name = "dtpFechaRegistroDe";
            this.dtpFechaRegistroDe.Size = new System.Drawing.Size(98, 20);
            this.dtpFechaRegistroDe.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.BackColorInternal = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.TxtCodigoProducto);
            this.groupBox1.Location = new System.Drawing.Point(12, 74);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1017, 483);
            this.groupBox1.TabIndex = 24;
            this.groupBox1.Text = "Reporte de Ventas";
            // 
            // TxtCodigoProducto
            // 
            this.TxtCodigoProducto.ActiveViewIndex = -1;
            this.TxtCodigoProducto.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtCodigoProducto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TxtCodigoProducto.Cursor = System.Windows.Forms.Cursors.Default;
            this.TxtCodigoProducto.ForeColor = System.Drawing.Color.Coral;
            this.TxtCodigoProducto.Location = new System.Drawing.Point(3, 27);
            this.TxtCodigoProducto.Name = "TxtCodigoProducto";
            this.TxtCodigoProducto.ShowCopyButton = false;
            this.TxtCodigoProducto.ShowGroupTreeButton = false;
            this.TxtCodigoProducto.ShowParameterPanelButton = false;
            this.TxtCodigoProducto.ShowRefreshButton = false;
            this.TxtCodigoProducto.Size = new System.Drawing.Size(1008, 456);
            this.TxtCodigoProducto.TabIndex = 1;
            this.TxtCodigoProducto.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // frmRegistroVentaAlmacenResumen_Fill_Panel
            // 
            // 
            // frmRegistroVentaAlmacenResumen_Fill_Panel.ClientArea
            // 
            this.frmRegistroVentaAlmacenResumen_Fill_Panel.ClientArea.Controls.Add(this.groupBox5);
            this.frmRegistroVentaAlmacenResumen_Fill_Panel.ClientArea.Controls.Add(this.groupBox1);
            this.frmRegistroVentaAlmacenResumen_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.frmRegistroVentaAlmacenResumen_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmRegistroVentaAlmacenResumen_Fill_Panel.Location = new System.Drawing.Point(8, 31);
            this.frmRegistroVentaAlmacenResumen_Fill_Panel.Name = "frmRegistroVentaAlmacenResumen_Fill_Panel";
            this.frmRegistroVentaAlmacenResumen_Fill_Panel.Size = new System.Drawing.Size(1041, 547);
            this.frmRegistroVentaAlmacenResumen_Fill_Panel.TabIndex = 0;
            // 
            // _frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Left
            // 
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 31);
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Left.Name = "_frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Left";
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 547);
            // 
            // _frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Right
            // 
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(1049, 31);
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Right.Name = "_frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Right";
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 547);
            // 
            // _frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Top
            // 
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Top.Name = "_frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Top";
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(1057, 31);
            // 
            // _frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Bottom
            // 
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 578);
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Bottom.Name = "_frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Bottom";
            this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(1057, 8);
            // 
            // frmRegistroVentaAlmacenResumen
            // 
            this.AcceptButton = this.BtnVuisualizar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(1057, 586);
            this.Controls.Add(this.frmRegistroVentaAlmacenResumen_Fill_Panel);
            this.Controls.Add(this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Bottom);
            this.Name = "frmRegistroVentaAlmacenResumen";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reporte de Ventas por Almacen Resumen";
            this.Load += new System.EventHandler(this.frmRegistroVentaAlmacenResumen_Load);
            ((System.ComponentModel.ISupportInitialize)(this.groupBox5)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboAlmacen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkHoraimpresion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.uvDatos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            this.frmRegistroVentaAlmacenResumen_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmRegistroVentaAlmacenResumen_Fill_Panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox groupBox5;
        private Infragistics.Win.Misc.UltraLabel label4;
        private Infragistics.Win.Misc.UltraButton BtnVuisualizar;
        private Infragistics.Win.Misc.UltraLabel label2;
        private Infragistics.Win.Misc.UltraLabel label1;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkHoraimpresion;
        private System.Windows.Forms.DateTimePicker dtpFechaRegistroAl;
        private System.Windows.Forms.DateTimePicker dtpFechaRegistroDe;
        private Infragistics.Win.Misc.UltraGroupBox groupBox1;
        private CrystalDecisions.Windows.Forms.CrystalReportViewer TxtCodigoProducto;
        private Infragistics.Win.Misc.UltraValidator uvDatos;
        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.Misc.UltraPanel frmRegistroVentaAlmacenResumen_Fill_Panel;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmRegistroVentaAlmacenResumen_UltraFormManager_Dock_Area_Bottom;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboAlmacen;
    }
}