namespace SAMBHS.Windows.WinServer.UI.Administracion
{
    partial class frmMantenimientoUsuarioEmpresa
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
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_RUC");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_RazonSocial");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("_Eliminar", 0);
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMantenimientoUsuarioEmpresa));
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this.frmMantenimientoUsuarioEmpresa_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this.ultraGroupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            this.grdData = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.btnAceptar = new Infragistics.Win.Misc.UltraButton();
            this.cboEmpresas = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this.ultraValidator1 = new Infragistics.Win.Misc.UltraValidator(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            this.frmMantenimientoUsuarioEmpresa_Fill_Panel.ClientArea.SuspendLayout();
            this.frmMantenimientoUsuarioEmpresa_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).BeginInit();
            this.ultraGroupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboEmpresas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraValidator1)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // frmMantenimientoUsuarioEmpresa_Fill_Panel
            // 
            // 
            // frmMantenimientoUsuarioEmpresa_Fill_Panel.ClientArea
            // 
            this.frmMantenimientoUsuarioEmpresa_Fill_Panel.ClientArea.Controls.Add(this.ultraGroupBox2);
            this.frmMantenimientoUsuarioEmpresa_Fill_Panel.ClientArea.Controls.Add(this.ultraGroupBox1);
            this.frmMantenimientoUsuarioEmpresa_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.frmMantenimientoUsuarioEmpresa_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmMantenimientoUsuarioEmpresa_Fill_Panel.Location = new System.Drawing.Point(8, 31);
            this.frmMantenimientoUsuarioEmpresa_Fill_Panel.Name = "frmMantenimientoUsuarioEmpresa_Fill_Panel";
            this.frmMantenimientoUsuarioEmpresa_Fill_Panel.Size = new System.Drawing.Size(529, 274);
            this.frmMantenimientoUsuarioEmpresa_Fill_Panel.TabIndex = 0;
            // 
            // ultraGroupBox2
            // 
            this.ultraGroupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraGroupBox2.Controls.Add(this.grdData);
            this.ultraGroupBox2.Location = new System.Drawing.Point(6, 84);
            this.ultraGroupBox2.Name = "ultraGroupBox2";
            this.ultraGroupBox2.Size = new System.Drawing.Size(517, 184);
            this.ultraGroupBox2.TabIndex = 1;
            this.ultraGroupBox2.Text = "Empresas Asignadas:";
            // 
            // grdData
            // 
            ultraGridColumn1.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
            ultraGridColumn1.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            ultraGridColumn1.Header.Caption = "RUC";
            ultraGridColumn1.Header.VisiblePosition = 1;
            ultraGridColumn1.Width = 129;
            ultraGridColumn2.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
            ultraGridColumn2.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            ultraGridColumn2.Header.Caption = "Razón Social";
            ultraGridColumn2.Header.VisiblePosition = 2;
            ultraGridColumn2.Width = 303;
            ultraGridColumn3.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
            ultraGridColumn3.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
            appearance1.Image = ((object)(resources.GetObject("appearance1.Image")));
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance1.ImageVAlign = Infragistics.Win.VAlign.Middle;
            ultraGridColumn3.CellButtonAppearance = appearance1;
            ultraGridColumn3.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            ultraGridColumn3.Header.Caption = "";
            ultraGridColumn3.Header.VisiblePosition = 0;
            ultraGridColumn3.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
            ultraGridColumn3.Width = 26;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3});
            this.grdData.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdData.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            this.grdData.DisplayLayout.NewColumnLoadStyle = Infragistics.Win.UltraWinGrid.NewColumnLoadStyle.Hide;
            this.grdData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdData.Location = new System.Drawing.Point(6, 19);
            this.grdData.Name = "grdData";
            this.grdData.Size = new System.Drawing.Size(505, 159);
            this.grdData.TabIndex = 0;
            this.grdData.Text = "ultraGrid1";
            this.grdData.ClickCellButton += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.grdData_ClickCellButton);
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraGroupBox1.Controls.Add(this.btnAceptar);
            this.ultraGroupBox1.Controls.Add(this.cboEmpresas);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel1);
            this.ultraGroupBox1.Location = new System.Drawing.Point(6, 6);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(517, 72);
            this.ultraGroupBox1.TabIndex = 0;
            this.ultraGroupBox1.Text = "Empresas Disponibles:";
            // 
            // btnAceptar
            // 
            this.btnAceptar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance2.Image = global::SAMBHS.Windows.WinServer.UI.Properties.Resources.add;
            this.btnAceptar.Appearance = appearance2;
            this.btnAceptar.Location = new System.Drawing.Point(406, 24);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(85, 31);
            this.btnAceptar.TabIndex = 2;
            this.btnAceptar.Text = "Aceptar";
            this.btnAceptar.Click += new System.EventHandler(this.btnAceptar_Click);
            // 
            // cboEmpresas
            // 
            this.cboEmpresas.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboEmpresas.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboEmpresas.Location = new System.Drawing.Point(82, 29);
            this.cboEmpresas.Name = "cboEmpresas";
            this.cboEmpresas.Size = new System.Drawing.Size(307, 21);
            this.cboEmpresas.TabIndex = 1;
            this.ultraValidator1.GetValidationSettings(this.cboEmpresas).Condition = new Infragistics.Win.OperatorCondition(Infragistics.Win.ConditionOperator.NotEquals, "-1", true, typeof(string));
            this.ultraValidator1.GetValidationSettings(this.cboEmpresas).DataType = typeof(string);
            this.ultraValidator1.GetValidationSettings(this.cboEmpresas).IsRequired = true;
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(23, 33);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(53, 14);
            this.ultraLabel1.TabIndex = 0;
            this.ultraLabel1.Text = "Empresa:";
            // 
            // _frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Left
            // 
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 31);
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Left.Name = "_frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Left";
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 274);
            // 
            // _frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Right
            // 
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(537, 31);
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Right.Name = "_frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Right";
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 274);
            // 
            // _frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Top
            // 
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Top.Name = "_frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Top";
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(545, 31);
            // 
            // _frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Bottom
            // 
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 305);
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Bottom.Name = "_frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Bottom";
            this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(545, 8);
            // 
            // frmMantenimientoUsuarioEmpresa
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(545, 313);
            this.Controls.Add(this.frmMantenimientoUsuarioEmpresa_Fill_Panel);
            this.Controls.Add(this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Bottom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMantenimientoUsuarioEmpresa";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Empresas Asignadas al Usuario: ";
            this.Load += new System.EventHandler(this.frmMantenimientoUsuarioEmpresa_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            this.frmMantenimientoUsuarioEmpresa_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmMantenimientoUsuarioEmpresa_Fill_Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).EndInit();
            this.ultraGroupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboEmpresas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraValidator1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.Misc.UltraPanel frmMantenimientoUsuarioEmpresa_Fill_Panel;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox2;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdData;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboEmpresas;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmMantenimientoUsuarioEmpresa_UltraFormManager_Dock_Area_Bottom;
        private Infragistics.Win.Misc.UltraButton btnAceptar;
        private Infragistics.Win.Misc.UltraValidator ultraValidator1;
    }
}