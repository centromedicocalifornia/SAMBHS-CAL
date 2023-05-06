namespace SAMBHS.Windows.WinClient.UI.Reportes.Compras
{
    partial class frmProveedor
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
            this.cboOrden = new System.Windows.Forms.ComboBox();
            this.cboTipoPersona = new System.Windows.Forms.ComboBox();
            this.label4 = new Infragistics.Win.Misc.UltraLabel();
            this.label6 = new Infragistics.Win.Misc.UltraLabel();
            this.BtnVuisualizar = new Infragistics.Win.Misc.UltraButton();
            this.chkHoraimpresion = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.crystalReportViewer1 = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.groupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.uvDatos = new Infragistics.Win.Misc.UltraValidator(this.components);
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this.frmProveedor_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this._frmProveedor_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmProveedor_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmProveedor_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmProveedor_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox5)).BeginInit();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkHoraimpresion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uvDatos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            this.frmProveedor_Fill_Panel.ClientArea.SuspendLayout();
            this.frmProveedor_Fill_Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.BackColorInternal = System.Drawing.Color.Transparent;
            this.groupBox5.Controls.Add(this.cboOrden);
            this.groupBox5.Controls.Add(this.cboTipoPersona);
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Controls.Add(this.label6);
            this.groupBox5.Controls.Add(this.BtnVuisualizar);
            this.groupBox5.Controls.Add(this.chkHoraimpresion);
            this.groupBox5.Location = new System.Drawing.Point(2, 12);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(1033, 51);
            this.groupBox5.TabIndex = 0;
            this.groupBox5.Text = "Filtro de Búsqueda";
            // 
            // cboOrden
            // 
            this.cboOrden.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOrden.FormattingEnabled = true;
            this.cboOrden.Location = new System.Drawing.Point(341, 19);
            this.cboOrden.Name = "cboOrden";
            this.cboOrden.Size = new System.Drawing.Size(168, 21);
            this.cboOrden.TabIndex = 1;
            this.uvDatos.GetValidationSettings(this.cboOrden).Condition = new Infragistics.Win.OperatorCondition(Infragistics.Win.ConditionOperator.NotEquals, "--Seleccionar--", true, typeof(string));
            this.uvDatos.GetValidationSettings(this.cboOrden).IsRequired = true;
            // 
            // cboTipoPersona
            // 
            this.cboTipoPersona.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTipoPersona.FormattingEnabled = true;
            this.cboTipoPersona.Location = new System.Drawing.Point(74, 19);
            this.cboTipoPersona.Name = "cboTipoPersona";
            this.cboTipoPersona.Size = new System.Drawing.Size(201, 21);
            this.cboTipoPersona.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColorInternal = System.Drawing.Color.Transparent;
            this.label4.Location = new System.Drawing.Point(4, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 14);
            this.label4.TabIndex = 209;
            this.label4.Text = "Tipo Persona:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColorInternal = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(281, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 14);
            this.label6.TabIndex = 24;
            this.label6.Text = "Ordena por:";
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
            this.BtnVuisualizar.Location = new System.Drawing.Point(902, 12);
            this.BtnVuisualizar.Margin = new System.Windows.Forms.Padding(2);
            this.BtnVuisualizar.Name = "BtnVuisualizar";
            this.BtnVuisualizar.Size = new System.Drawing.Size(126, 30);
            this.BtnVuisualizar.TabIndex = 3;
            this.BtnVuisualizar.Text = "Visualizar Reporte";
            this.BtnVuisualizar.Click += new System.EventHandler(this.BtnVuisualizar_Click);
            // 
            // chkHoraimpresion
            // 
            this.chkHoraimpresion.AutoSize = true;
            this.chkHoraimpresion.BackColor = System.Drawing.Color.Transparent;
            this.chkHoraimpresion.BackColorInternal = System.Drawing.Color.Transparent;
            this.chkHoraimpresion.Checked = true;
            this.chkHoraimpresion.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkHoraimpresion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkHoraimpresion.Location = new System.Drawing.Point(535, 19);
            this.chkHoraimpresion.Name = "chkHoraimpresion";
            this.chkHoraimpresion.Size = new System.Drawing.Size(157, 17);
            this.chkHoraimpresion.TabIndex = 2;
            this.chkHoraimpresion.Text = "Fecha y Hora de Impresión ";
            this.chkHoraimpresion.UseFlatMode = Infragistics.Win.DefaultableBoolean.True;
            // 
            // crystalReportViewer1
            // 
            this.crystalReportViewer1.ActiveViewIndex = -1;
            this.crystalReportViewer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crystalReportViewer1.Cursor = System.Windows.Forms.Cursors.Default;
            this.crystalReportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crystalReportViewer1.Location = new System.Drawing.Point(3, 16);
            this.crystalReportViewer1.Name = "crystalReportViewer1";
            this.crystalReportViewer1.Size = new System.Drawing.Size(1027, 442);
            this.crystalReportViewer1.TabIndex = 0;
            this.crystalReportViewer1.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.BackColorInternal = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.crystalReportViewer1);
            this.groupBox1.Location = new System.Drawing.Point(2, 79);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1033, 461);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.Text = "Reporte de Proveedores";
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // frmProveedor_Fill_Panel
            // 
            // 
            // frmProveedor_Fill_Panel.ClientArea
            // 
            this.frmProveedor_Fill_Panel.ClientArea.Controls.Add(this.groupBox5);
            this.frmProveedor_Fill_Panel.ClientArea.Controls.Add(this.groupBox1);
            this.frmProveedor_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.frmProveedor_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmProveedor_Fill_Panel.Location = new System.Drawing.Point(8, 31);
            this.frmProveedor_Fill_Panel.Name = "frmProveedor_Fill_Panel";
            this.frmProveedor_Fill_Panel.Size = new System.Drawing.Size(1041, 547);
            this.frmProveedor_Fill_Panel.TabIndex = 0;
            // 
            // _frmProveedor_UltraFormManager_Dock_Area_Left
            // 
            this._frmProveedor_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmProveedor_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmProveedor_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._frmProveedor_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmProveedor_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._frmProveedor_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmProveedor_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 31);
            this._frmProveedor_UltraFormManager_Dock_Area_Left.Name = "_frmProveedor_UltraFormManager_Dock_Area_Left";
            this._frmProveedor_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 547);
            // 
            // _frmProveedor_UltraFormManager_Dock_Area_Right
            // 
            this._frmProveedor_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmProveedor_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmProveedor_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._frmProveedor_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmProveedor_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._frmProveedor_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmProveedor_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(1049, 31);
            this._frmProveedor_UltraFormManager_Dock_Area_Right.Name = "_frmProveedor_UltraFormManager_Dock_Area_Right";
            this._frmProveedor_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 547);
            // 
            // _frmProveedor_UltraFormManager_Dock_Area_Top
            // 
            this._frmProveedor_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmProveedor_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmProveedor_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._frmProveedor_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmProveedor_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._frmProveedor_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmProveedor_UltraFormManager_Dock_Area_Top.Name = "_frmProveedor_UltraFormManager_Dock_Area_Top";
            this._frmProveedor_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(1057, 31);
            // 
            // _frmProveedor_UltraFormManager_Dock_Area_Bottom
            // 
            this._frmProveedor_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmProveedor_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmProveedor_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._frmProveedor_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmProveedor_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._frmProveedor_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmProveedor_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 578);
            this._frmProveedor_UltraFormManager_Dock_Area_Bottom.Name = "_frmProveedor_UltraFormManager_Dock_Area_Bottom";
            this._frmProveedor_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(1057, 8);
            // 
            // frmProveedor
            // 
            this.AcceptButton = this.BtnVuisualizar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(1057, 586);
            this.Controls.Add(this.frmProveedor_Fill_Panel);
            this.Controls.Add(this._frmProveedor_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._frmProveedor_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._frmProveedor_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._frmProveedor_UltraFormManager_Dock_Area_Bottom);
            this.Name = "frmProveedor";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reporte Proveedor";
            this.Load += new System.EventHandler(this.frmProveedor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.groupBox5)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkHoraimpresion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.uvDatos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            this.frmProveedor_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmProveedor_Fill_Panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox groupBox5;
        private System.Windows.Forms.ComboBox cboOrden;
        private Infragistics.Win.Misc.UltraValidator uvDatos;
        private System.Windows.Forms.ComboBox cboTipoPersona;
        private Infragistics.Win.Misc.UltraLabel label4;
        private Infragistics.Win.Misc.UltraLabel label6;
        private Infragistics.Win.Misc.UltraButton BtnVuisualizar;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkHoraimpresion;
        private CrystalDecisions.Windows.Forms.CrystalReportViewer crystalReportViewer1;
        private Infragistics.Win.Misc.UltraGroupBox groupBox1;
        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.Misc.UltraPanel frmProveedor_Fill_Panel;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmProveedor_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmProveedor_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmProveedor_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmProveedor_UltraFormManager_Dock_Area_Bottom;
    }
}