namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    partial class FrmExtraerDetallesMovimientosConsultaPedidos
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
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_NombreProducto");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_CodigoInterno");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("d_Cantidad");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("UMEmpaque");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CantidadOriginal");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CantidadFacturada");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this.frmExtraerDetallesMovimientosConsultaPedidos_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this.ultraGrid1 = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            this.frmExtraerDetallesMovimientosConsultaPedidos_Fill_Panel.ClientArea.SuspendLayout();
            this.frmExtraerDetallesMovimientosConsultaPedidos_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGrid1)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // frmExtraerDetallesMovimientosConsultaPedidos_Fill_Panel
            // 
            // 
            // frmExtraerDetallesMovimientosConsultaPedidos_Fill_Panel.ClientArea
            // 
            this.frmExtraerDetallesMovimientosConsultaPedidos_Fill_Panel.ClientArea.Controls.Add(this.ultraGrid1);
            this.frmExtraerDetallesMovimientosConsultaPedidos_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.frmExtraerDetallesMovimientosConsultaPedidos_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmExtraerDetallesMovimientosConsultaPedidos_Fill_Panel.Location = new System.Drawing.Point(8, 31);
            this.frmExtraerDetallesMovimientosConsultaPedidos_Fill_Panel.Name = "frmExtraerDetallesMovimientosConsultaPedidos_Fill_Panel";
            this.frmExtraerDetallesMovimientosConsultaPedidos_Fill_Panel.Size = new System.Drawing.Size(832, 253);
            this.frmExtraerDetallesMovimientosConsultaPedidos_Fill_Panel.TabIndex = 0;
            // 
            // ultraGrid1
            // 
            this.ultraGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            ultraGridColumn1.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
            ultraGridColumn1.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
            ultraGridColumn1.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            ultraGridColumn1.Header.Caption = "Producto";
            ultraGridColumn1.Header.VisiblePosition = 1;
            ultraGridColumn1.Width = 319;
            ultraGridColumn2.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
            ultraGridColumn2.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
            ultraGridColumn2.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            ultraGridColumn2.Header.Caption = "Cod. Producto";
            ultraGridColumn2.Header.VisiblePosition = 0;
            ultraGridColumn2.Width = 108;
            ultraGridColumn3.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
            ultraGridColumn3.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
            appearance1.TextHAlignAsString = "Right";
            ultraGridColumn3.CellAppearance = appearance1;
            ultraGridColumn3.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            ultraGridColumn3.Header.Caption = "Saldo";
            ultraGridColumn3.Header.VisiblePosition = 5;
            ultraGridColumn3.MaskDataMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals;
            ultraGridColumn3.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals;
            ultraGridColumn3.MaskInput = "{double:9.2:c}";
            ultraGridColumn4.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
            ultraGridColumn4.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
            ultraGridColumn4.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            ultraGridColumn4.Header.Caption = "U.M";
            ultraGridColumn4.Header.VisiblePosition = 2;
            ultraGridColumn5.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
            ultraGridColumn5.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
            appearance2.TextHAlignAsString = "Right";
            ultraGridColumn5.CellAppearance = appearance2;
            ultraGridColumn5.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            ultraGridColumn5.Header.Caption = "Cantidad";
            ultraGridColumn5.Header.VisiblePosition = 3;
            ultraGridColumn5.MaskDataMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals;
            ultraGridColumn5.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals;
            ultraGridColumn5.MaskInput = "{double:9.2:c}";
            ultraGridColumn6.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
            ultraGridColumn6.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
            appearance3.TextHAlignAsString = "Right";
            ultraGridColumn6.CellAppearance = appearance3;
            ultraGridColumn6.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            ultraGridColumn6.Format = "";
            ultraGridColumn6.Header.Caption = "Facturado";
            ultraGridColumn6.Header.VisiblePosition = 4;
            ultraGridColumn6.MaskDataMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals;
            ultraGridColumn6.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals;
            ultraGridColumn6.MaskInput = "{double:9.2:c}";
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6});
            this.ultraGrid1.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.ultraGrid1.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            this.ultraGrid1.DisplayLayout.NewColumnLoadStyle = Infragistics.Win.UltraWinGrid.NewColumnLoadStyle.Hide;
            this.ultraGrid1.DisplayLayout.Override.ActiveAppearancesEnabled = Infragistics.Win.DefaultableBoolean.False;
            this.ultraGrid1.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.ultraGrid1.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.ultraGrid1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraGrid1.Location = new System.Drawing.Point(6, 6);
            this.ultraGrid1.Name = "ultraGrid1";
            this.ultraGrid1.Size = new System.Drawing.Size(820, 241);
            this.ultraGrid1.TabIndex = 0;
            this.ultraGrid1.Text = "ultraGrid1";
            // 
            // _frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Left
            // 
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 31);
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Left.Name = "_frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Left";
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 253);
            // 
            // _frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Right
            // 
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(840, 31);
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Right.Name = "_frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Right";
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 253);
            // 
            // _frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Top
            // 
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Top.Name = "_frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Top";
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(848, 31);
            // 
            // _frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Bottom
            // 
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 284);
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Bottom.Name = "_frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Bottom";
            this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(848, 8);
            // 
            // FrmExtraerDetallesMovimientosConsultaPedidos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(848, 292);
            this.Controls.Add(this.frmExtraerDetallesMovimientosConsultaPedidos_Fill_Panel);
            this.Controls.Add(this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Bottom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmExtraerDetallesMovimientosConsultaPedidos";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Faltantes de facturar";
            this.Load += new System.EventHandler(this.frmExtraerDetallesMovimientosConsultaPedidos_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            this.frmExtraerDetallesMovimientosConsultaPedidos_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmExtraerDetallesMovimientosConsultaPedidos_Fill_Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGrid1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.Misc.UltraPanel frmExtraerDetallesMovimientosConsultaPedidos_Fill_Panel;
        private Infragistics.Win.UltraWinGrid.UltraGrid ultraGrid1;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmExtraerDetallesMovimientosConsultaPedidos_UltraFormManager_Dock_Area_Bottom;
    }
}