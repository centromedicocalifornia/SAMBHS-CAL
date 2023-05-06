namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    partial class frmBusquedaFacturasImportacion
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
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("i_RegistroTipo", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("i_RegistroEstado", 1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("i_IdTipoDocumento", 2);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_SerieDocumento", 3);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_NroFactura", 4);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn21 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_NroPedido", 5);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_CodigoCliente", 6);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_IdCliente", 7);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_RazonSocial", 8);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            this.grdDataFacturas = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this.frmBusquedaFacturasImportacion_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            ((System.ComponentModel.ISupportInitialize)(this.grdDataFacturas)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            this.frmBusquedaFacturasImportacion_Fill_Panel.ClientArea.SuspendLayout();
            this.frmBusquedaFacturasImportacion_Fill_Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // grdDataFacturas
            // 
            this.grdDataFacturas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdDataFacturas.CausesValidation = false;
            appearance1.BackColor = System.Drawing.Color.White;
            this.grdDataFacturas.DisplayLayout.Appearance = appearance1;
            this.grdDataFacturas.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn4.Header.VisiblePosition = 0;
            ultraGridColumn4.Hidden = true;
            ultraGridColumn6.Header.VisiblePosition = 1;
            ultraGridColumn6.Hidden = true;
            ultraGridColumn5.Header.Caption = "Tipo Doc.";
            ultraGridColumn5.Header.VisiblePosition = 2;
            ultraGridColumn5.MaskInput = "{double:9.4}";
            ultraGridColumn5.Width = 82;
            ultraGridColumn7.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            ultraGridColumn7.Header.Caption = "Serie";
            ultraGridColumn7.Header.VisiblePosition = 3;
            ultraGridColumn7.Width = 76;
            ultraGridColumn3.Header.Caption = "Nro. Factura";
            ultraGridColumn3.Header.VisiblePosition = 4;
            ultraGridColumn3.Width = 94;
            ultraGridColumn21.Header.Caption = "Nro. Pedido";
            ultraGridColumn21.Header.VisiblePosition = 5;
            ultraGridColumn21.Width = 101;
            ultraGridColumn1.Header.Caption = "Código Proveedor";
            ultraGridColumn1.Header.VisiblePosition = 6;
            ultraGridColumn1.Width = 132;
            ultraGridColumn2.Header.VisiblePosition = 7;
            ultraGridColumn2.Hidden = true;
            ultraGridColumn9.Header.Caption = "Proveedor";
            ultraGridColumn9.Header.VisiblePosition = 8;
            ultraGridColumn9.Width = 127;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn4,
            ultraGridColumn6,
            ultraGridColumn5,
            ultraGridColumn7,
            ultraGridColumn3,
            ultraGridColumn21,
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn9});
            this.grdDataFacturas.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdDataFacturas.DisplayLayout.MaxColScrollRegions = 1;
            this.grdDataFacturas.DisplayLayout.MaxRowScrollRegions = 1;
            this.grdDataFacturas.DisplayLayout.NewColumnLoadStyle = Infragistics.Win.UltraWinGrid.NewColumnLoadStyle.Hide;
            appearance2.BackColor = System.Drawing.Color.AliceBlue;
            appearance2.ForeColor = System.Drawing.Color.Black;
            this.grdDataFacturas.DisplayLayout.Override.ActiveRowAppearance = appearance2;
            this.grdDataFacturas.DisplayLayout.Override.AllowColSwapping = Infragistics.Win.UltraWinGrid.AllowColSwapping.NotAllowed;
            this.grdDataFacturas.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            this.grdDataFacturas.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance3.BackColor = System.Drawing.Color.Transparent;
            this.grdDataFacturas.DisplayLayout.Override.CardAreaAppearance = appearance3;
            appearance4.BorderColor = System.Drawing.SystemColors.ControlLight;
            this.grdDataFacturas.DisplayLayout.Override.CellAppearance = appearance4;
            this.grdDataFacturas.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.grdDataFacturas.DisplayLayout.Override.CellPadding = 3;
            appearance5.TextHAlignAsString = "Left";
            this.grdDataFacturas.DisplayLayout.Override.HeaderAppearance = appearance5;
            this.grdDataFacturas.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.Select;
            appearance6.BorderColor = System.Drawing.Color.White;
            appearance6.TextVAlignAsString = "Middle";
            this.grdDataFacturas.DisplayLayout.Override.RowAppearance = appearance6;
            this.grdDataFacturas.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.grdDataFacturas.DisplayLayout.RowConnectorStyle = Infragistics.Win.UltraWinGrid.RowConnectorStyle.None;
            this.grdDataFacturas.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdDataFacturas.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdDataFacturas.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.grdDataFacturas.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdDataFacturas.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.grdDataFacturas.Location = new System.Drawing.Point(5, 18);
            this.grdDataFacturas.Margin = new System.Windows.Forms.Padding(2);
            this.grdDataFacturas.Name = "grdDataFacturas";
            this.grdDataFacturas.Size = new System.Drawing.Size(614, 182);
            this.grdDataFacturas.TabIndex = 123;
            this.grdDataFacturas.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grdDataFacturas_InitializeLayout);
            this.grdDataFacturas.DoubleClickCell += new Infragistics.Win.UltraWinGrid.DoubleClickCellEventHandler(this.grdDataFacturas_DoubleClickCell);
            this.grdDataFacturas.KeyDown += new System.Windows.Forms.KeyEventHandler(this.grdDataFacturas_KeyDown);
            this.grdDataFacturas.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.grdDataFacturas_KeyPress);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.grdDataFacturas);
            this.groupBox4.Location = new System.Drawing.Point(12, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(639, 207);
            this.groupBox4.TabIndex = 294;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Facturas Registradas";
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // frmBusquedaFacturasImportacion_Fill_Panel
            // 
            // 
            // frmBusquedaFacturasImportacion_Fill_Panel.ClientArea
            // 
            this.frmBusquedaFacturasImportacion_Fill_Panel.ClientArea.Controls.Add(this.groupBox4);
            this.frmBusquedaFacturasImportacion_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.frmBusquedaFacturasImportacion_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmBusquedaFacturasImportacion_Fill_Panel.Location = new System.Drawing.Point(8, 31);
            this.frmBusquedaFacturasImportacion_Fill_Panel.Name = "frmBusquedaFacturasImportacion_Fill_Panel";
            this.frmBusquedaFacturasImportacion_Fill_Panel.Size = new System.Drawing.Size(666, 234);
            this.frmBusquedaFacturasImportacion_Fill_Panel.TabIndex = 0;
            // 
            // _frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Left
            // 
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 31);
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Left.Name = "_frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Left";
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 234);
            // 
            // _frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Right
            // 
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(674, 31);
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Right.Name = "_frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Right";
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 234);
            // 
            // _frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Top
            // 
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Top.Name = "_frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Top";
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(682, 31);
            // 
            // _frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Bottom
            // 
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 265);
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Bottom.Name = "_frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Bottom";
            this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(682, 8);
            // 
            // frmBusquedaFacturasImportacion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(682, 273);
            this.Controls.Add(this.frmBusquedaFacturasImportacion_Fill_Panel);
            this.Controls.Add(this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Bottom);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmBusquedaFacturasImportacion";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Relación de Facturas Registradas";
            this.Load += new System.EventHandler(this.frmBusquedaFacturasImportacion_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grdDataFacturas)).EndInit();
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            this.frmBusquedaFacturasImportacion_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmBusquedaFacturasImportacion_Fill_Panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinGrid.UltraGrid grdDataFacturas;
        private System.Windows.Forms.GroupBox groupBox4;
        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.Misc.UltraPanel frmBusquedaFacturasImportacion_Fill_Panel;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmBusquedaFacturasImportacion_UltraFormManager_Dock_Area_Bottom;
    }
}