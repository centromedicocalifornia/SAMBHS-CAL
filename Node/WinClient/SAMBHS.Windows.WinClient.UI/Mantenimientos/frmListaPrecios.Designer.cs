namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    partial class frmListaPrecios
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
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("i_RegistroTipo", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("i_RegistroEstado", 1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_Descripcion", 2);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_CodInterno", 3, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("d_Descuento", 4);
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("d_Precio", 5);
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("d_PrecioMinSoles", 6);
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("d_PrecioMinDolares", 7);
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_IdProductoAlmacen", 8);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_idListaPrecioDetalle", 9);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Index", 10);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Costo", 11);
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_IdProductoDetalle", 12);
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            this.groupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.cboAlmacen = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.cboListaPrecios = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.cboMoneda = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.label2 = new Infragistics.Win.Misc.UltraLabel();
            this.label1 = new Infragistics.Win.Misc.UltraLabel();
            this.groupBox3 = new Infragistics.Win.Misc.UltraGroupBox();
            this.lblContadorFilas = new Infragistics.Win.Misc.UltraLabel();
            this.txtBuscarProducto = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label3 = new Infragistics.Win.Misc.UltraLabel();
            this.btnAgregar = new Infragistics.Win.Misc.UltraButton();
            this.grdData = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.uvListaPrecios = new Infragistics.Win.Misc.UltraValidator(this.components);
            this.btnGuardar = new Infragistics.Win.Misc.UltraButton();
            this.btnMigracion = new Infragistics.Win.Misc.UltraButton();
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this.frmListaPrecios_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this.BtnImprimir = new Infragistics.Win.Misc.UltraButton();
            this._frmListaPrecios_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmListaPrecios_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmListaPrecios_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmListaPrecios_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboAlmacen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboListaPrecios)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboMoneda)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox3)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuscarProducto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uvListaPrecios)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            this.frmListaPrecios_Fill_Panel.ClientArea.SuspendLayout();
            this.frmListaPrecios_Fill_Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.cboAlmacen);
            this.groupBox1.Controls.Add(this.cboListaPrecios);
            this.groupBox1.Controls.Add(this.cboMoneda);
            this.groupBox1.Controls.Add(this.ultraLabel1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(4, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1040, 46);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.Text = "Filtro de Búsqueda:";
            // 
            // cboAlmacen
            // 
            this.cboAlmacen.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboAlmacen.Enabled = false;
            this.cboAlmacen.Location = new System.Drawing.Point(65, 16);
            this.cboAlmacen.Name = "cboAlmacen";
            this.cboAlmacen.Size = new System.Drawing.Size(227, 21);
            this.cboAlmacen.TabIndex = 23;
            // 
            // cboListaPrecios
            // 
            this.cboListaPrecios.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboListaPrecios.Enabled = false;
            this.cboListaPrecios.Location = new System.Drawing.Point(383, 16);
            this.cboListaPrecios.Name = "cboListaPrecios";
            this.cboListaPrecios.Size = new System.Drawing.Size(126, 21);
            this.cboListaPrecios.TabIndex = 22;
            // 
            // cboMoneda
            // 
            this.cboMoneda.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboMoneda.Enabled = false;
            this.cboMoneda.Location = new System.Drawing.Point(585, 16);
            this.cboMoneda.Name = "cboMoneda";
            this.cboMoneda.Size = new System.Drawing.Size(118, 21);
            this.cboMoneda.TabIndex = 21;
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(528, 19);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(51, 14);
            this.ultraLabel1.TabIndex = 20;
            this.ultraLabel1.Text = "Moneda : ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(302, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 14);
            this.label2.TabIndex = 16;
            this.label2.Text = "Lista Precios :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "Almacén:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lblContadorFilas);
            this.groupBox3.Controls.Add(this.txtBuscarProducto);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.btnAgregar);
            this.groupBox3.Controls.Add(this.grdData);
            this.groupBox3.Location = new System.Drawing.Point(4, 64);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(1040, 402);
            this.groupBox3.TabIndex = 24;
            this.groupBox3.Text = "Productos";
            // 
            // lblContadorFilas
            // 
            this.lblContadorFilas.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.TextHAlignAsString = "Right";
            appearance1.TextVAlignAsString = "Middle";
            this.lblContadorFilas.Appearance = appearance1;
            this.lblContadorFilas.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.2F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContadorFilas.Location = new System.Drawing.Point(810, 17);
            this.lblContadorFilas.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblContadorFilas.Name = "lblContadorFilas";
            this.lblContadorFilas.Size = new System.Drawing.Size(213, 19);
            this.lblContadorFilas.TabIndex = 144;
            this.lblContadorFilas.Text = "No se ha realizado la búsqueda aún.";
            // 
            // txtBuscarProducto
            // 
            this.txtBuscarProducto.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBuscarProducto.Location = new System.Drawing.Point(133, 20);
            this.txtBuscarProducto.Name = "txtBuscarProducto";
            this.txtBuscarProducto.Size = new System.Drawing.Size(539, 21);
            this.txtBuscarProducto.TabIndex = 4;
            this.txtBuscarProducto.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBuscarProducto_KeyDown);
            this.txtBuscarProducto.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBuscarProducto_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 14);
            this.label3.TabIndex = 3;
            this.label3.Text = "Código / Descripción :";
            // 
            // btnAgregar
            // 
            appearance2.Image = global::SAMBHS.Windows.WinClient.UI.Resource.table_row_insert;
            appearance2.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance2.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance2.TextHAlignAsString = "Right";
            appearance2.TextVAlignAsString = "Middle";
            this.btnAgregar.Appearance = appearance2;
            this.btnAgregar.Location = new System.Drawing.Point(860, 369);
            this.btnAgregar.Margin = new System.Windows.Forms.Padding(2);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(175, 28);
            this.btnAgregar.TabIndex = 2;
            this.btnAgregar.Text = "&Agregar Productos Faltantes";
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
            // 
            // grdData
            // 
            this.grdData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdData.CausesValidation = false;
            appearance3.BackColor = System.Drawing.Color.White;
            this.grdData.DisplayLayout.Appearance = appearance3;
            this.grdData.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn1.Header.VisiblePosition = 4;
            ultraGridColumn1.Hidden = true;
            ultraGridColumn2.Header.VisiblePosition = 3;
            ultraGridColumn2.Hidden = true;
            ultraGridColumn2.Width = 95;
            ultraGridColumn3.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn3.Header.Caption = "Descripción Producto";
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn3.Width = 319;
            ultraGridColumn4.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn4.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.CellSelect;
            ultraGridColumn4.Header.Caption = "Cod. Producto";
            ultraGridColumn4.Header.VisiblePosition = 1;
            ultraGridColumn4.MaskInput = "";
            ultraGridColumn4.Width = 101;
            appearance4.TextHAlignAsString = "Right";
            ultraGridColumn5.CellAppearance = appearance4;
            ultraGridColumn5.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            ultraGridColumn5.Header.Caption = "Descuento %";
            ultraGridColumn5.Header.VisiblePosition = 5;
            ultraGridColumn5.MaskInput = "{double:9.4}";
            ultraGridColumn5.Width = 124;
            ultraGridColumn6.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            appearance5.TextHAlignAsString = "Right";
            ultraGridColumn6.CellAppearance = appearance5;
            ultraGridColumn6.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            ultraGridColumn6.Header.Caption = "Precio Venta";
            ultraGridColumn6.Header.VisiblePosition = 7;
            ultraGridColumn6.Width = 152;
            ultraGridColumn7.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            appearance6.TextHAlignAsString = "Right";
            ultraGridColumn7.CellAppearance = appearance6;
            ultraGridColumn7.Header.Caption = "Precio Min. Soles";
            ultraGridColumn7.Header.VisiblePosition = 8;
            ultraGridColumn7.Width = 168;
            ultraGridColumn8.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            appearance7.TextHAlignAsString = "Right";
            ultraGridColumn8.CellAppearance = appearance7;
            ultraGridColumn8.Header.Caption = "Precio Min. Dolares";
            ultraGridColumn8.Header.VisiblePosition = 9;
            ultraGridColumn8.Width = 74;
            ultraGridColumn9.Header.VisiblePosition = 10;
            ultraGridColumn9.Hidden = true;
            ultraGridColumn9.Width = 110;
            ultraGridColumn10.Header.VisiblePosition = 11;
            ultraGridColumn10.Hidden = true;
            ultraGridColumn11.Header.Caption = "Item";
            ultraGridColumn11.Header.VisiblePosition = 0;
            ultraGridColumn11.Hidden = true;
            ultraGridColumn11.Width = 44;
            ultraGridColumn12.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            appearance8.TextHAlignAsString = "Right";
            ultraGridColumn12.CellAppearance = appearance8;
            ultraGridColumn12.Header.VisiblePosition = 6;
            ultraGridColumn12.Width = 90;
            ultraGridColumn13.Header.VisiblePosition = 12;
            ultraGridColumn13.Hidden = true;
            ultraGridColumn13.Width = 98;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn8,
            ultraGridColumn9,
            ultraGridColumn10,
            ultraGridColumn11,
            ultraGridColumn12,
            ultraGridColumn13});
            this.grdData.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdData.DisplayLayout.MaxColScrollRegions = 1;
            this.grdData.DisplayLayout.MaxRowScrollRegions = 1;
            this.grdData.DisplayLayout.NewColumnLoadStyle = Infragistics.Win.UltraWinGrid.NewColumnLoadStyle.Hide;
            appearance9.BackColor = System.Drawing.Color.DarkTurquoise;
            appearance9.ForeColor = System.Drawing.Color.Black;
            this.grdData.DisplayLayout.Override.ActiveRowAppearance = appearance9;
            this.grdData.DisplayLayout.Override.AllowColSwapping = Infragistics.Win.UltraWinGrid.AllowColSwapping.NotAllowed;
            this.grdData.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.False;
            this.grdData.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance10.BackColor = System.Drawing.Color.Transparent;
            this.grdData.DisplayLayout.Override.CardAreaAppearance = appearance10;
            appearance11.BorderColor = System.Drawing.SystemColors.ControlLight;
            this.grdData.DisplayLayout.Override.CellAppearance = appearance11;
            this.grdData.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.grdData.DisplayLayout.Override.CellPadding = 3;
            appearance12.TextHAlignAsString = "Left";
            this.grdData.DisplayLayout.Override.HeaderAppearance = appearance12;
            this.grdData.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance13.BorderColor = System.Drawing.Color.White;
            appearance13.TextVAlignAsString = "Middle";
            this.grdData.DisplayLayout.Override.RowAppearance = appearance13;
            this.grdData.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.grdData.DisplayLayout.RowConnectorStyle = Infragistics.Win.UltraWinGrid.RowConnectorStyle.None;
            this.grdData.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdData.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdData.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.grdData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdData.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.grdData.Location = new System.Drawing.Point(5, 49);
            this.grdData.Margin = new System.Windows.Forms.Padding(2);
            this.grdData.Name = "grdData";
            this.grdData.Size = new System.Drawing.Size(1030, 316);
            this.grdData.TabIndex = 1;
            this.grdData.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.grdData_InitializeRow);
            this.grdData.AfterExitEditMode += new System.EventHandler(this.grdData_AfterExitEditMode);
            this.grdData.CellChange += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.grdData_CellChange);
            this.grdData.DoubleClickCell += new Infragistics.Win.UltraWinGrid.DoubleClickCellEventHandler(this.grdData_DoubleClickCell);
            this.grdData.KeyDown += new System.Windows.Forms.KeyEventHandler(this.grdData_KeyDown);
            this.grdData.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.grdData_KeyPress);
            this.grdData.MouseDown += new System.Windows.Forms.MouseEventHandler(this.grdData_MouseDown);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Anchor = System.Windows.Forms.AnchorStyles.Left;
            appearance14.Image = global::SAMBHS.Windows.WinClient.UI.Resource.system_save;
            appearance14.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance14.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance14.TextHAlignAsString = "Right";
            appearance14.TextVAlignAsString = "Middle";
            this.btnGuardar.Appearance = appearance14;
            this.btnGuardar.Location = new System.Drawing.Point(4, 466);
            this.btnGuardar.Margin = new System.Windows.Forms.Padding(2);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(80, 30);
            this.btnGuardar.TabIndex = 25;
            this.btnGuardar.Text = "&Guardar";
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // btnMigracion
            // 
            this.btnMigracion.Anchor = System.Windows.Forms.AnchorStyles.Left;
            appearance15.Image = global::SAMBHS.Windows.WinClient.UI.Resource.table_gear;
            appearance15.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance15.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance15.TextHAlignAsString = "Right";
            appearance15.TextVAlignAsString = "Middle";
            this.btnMigracion.Appearance = appearance15;
            this.btnMigracion.Location = new System.Drawing.Point(172, 466);
            this.btnMigracion.Margin = new System.Windows.Forms.Padding(2);
            this.btnMigracion.Name = "btnMigracion";
            this.btnMigracion.Size = new System.Drawing.Size(134, 30);
            this.btnMigracion.TabIndex = 30;
            this.btnMigracion.Text = "&Recalcular Precios";
            this.btnMigracion.Click += new System.EventHandler(this.btnMigracion_Click);
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // frmListaPrecios_Fill_Panel
            // 
            // 
            // frmListaPrecios_Fill_Panel.ClientArea
            // 
            this.frmListaPrecios_Fill_Panel.ClientArea.Controls.Add(this.BtnImprimir);
            this.frmListaPrecios_Fill_Panel.ClientArea.Controls.Add(this.btnMigracion);
            this.frmListaPrecios_Fill_Panel.ClientArea.Controls.Add(this.btnGuardar);
            this.frmListaPrecios_Fill_Panel.ClientArea.Controls.Add(this.groupBox3);
            this.frmListaPrecios_Fill_Panel.ClientArea.Controls.Add(this.groupBox1);
            this.frmListaPrecios_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.frmListaPrecios_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmListaPrecios_Fill_Panel.Location = new System.Drawing.Point(8, 32);
            this.frmListaPrecios_Fill_Panel.Name = "frmListaPrecios_Fill_Panel";
            this.frmListaPrecios_Fill_Panel.Size = new System.Drawing.Size(1050, 498);
            this.frmListaPrecios_Fill_Panel.TabIndex = 0;
            this.frmListaPrecios_Fill_Panel.PaintClient += new System.Windows.Forms.PaintEventHandler(this.frmListaPrecios_Fill_Panel_PaintClient);
            // 
            // BtnImprimir
            // 
            this.BtnImprimir.Anchor = System.Windows.Forms.AnchorStyles.Left;
            appearance16.Image = global::SAMBHS.Windows.WinClient.UI.Resource.printer;
            appearance16.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance16.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance16.TextHAlignAsString = "Center";
            appearance16.TextVAlignAsString = "Middle";
            this.BtnImprimir.Appearance = appearance16;
            this.BtnImprimir.Location = new System.Drawing.Point(88, 466);
            this.BtnImprimir.Margin = new System.Windows.Forms.Padding(2);
            this.BtnImprimir.Name = "BtnImprimir";
            this.BtnImprimir.Size = new System.Drawing.Size(80, 30);
            this.BtnImprimir.TabIndex = 31;
            this.BtnImprimir.Text = "&Imprimir ";
            this.BtnImprimir.Click += new System.EventHandler(this.BtnImprimir_Click);
            // 
            // _frmListaPrecios_UltraFormManager_Dock_Area_Left
            // 
            this._frmListaPrecios_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmListaPrecios_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmListaPrecios_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._frmListaPrecios_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmListaPrecios_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._frmListaPrecios_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmListaPrecios_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 32);
            this._frmListaPrecios_UltraFormManager_Dock_Area_Left.Name = "_frmListaPrecios_UltraFormManager_Dock_Area_Left";
            this._frmListaPrecios_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 498);
            // 
            // _frmListaPrecios_UltraFormManager_Dock_Area_Right
            // 
            this._frmListaPrecios_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmListaPrecios_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmListaPrecios_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._frmListaPrecios_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmListaPrecios_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._frmListaPrecios_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmListaPrecios_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(1058, 32);
            this._frmListaPrecios_UltraFormManager_Dock_Area_Right.Name = "_frmListaPrecios_UltraFormManager_Dock_Area_Right";
            this._frmListaPrecios_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 498);
            // 
            // _frmListaPrecios_UltraFormManager_Dock_Area_Top
            // 
            this._frmListaPrecios_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmListaPrecios_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmListaPrecios_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._frmListaPrecios_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmListaPrecios_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._frmListaPrecios_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmListaPrecios_UltraFormManager_Dock_Area_Top.Name = "_frmListaPrecios_UltraFormManager_Dock_Area_Top";
            this._frmListaPrecios_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(1066, 32);
            // 
            // _frmListaPrecios_UltraFormManager_Dock_Area_Bottom
            // 
            this._frmListaPrecios_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmListaPrecios_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmListaPrecios_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._frmListaPrecios_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmListaPrecios_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._frmListaPrecios_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmListaPrecios_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 530);
            this._frmListaPrecios_UltraFormManager_Dock_Area_Bottom.Name = "_frmListaPrecios_UltraFormManager_Dock_Area_Bottom";
            this._frmListaPrecios_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(1066, 8);
            // 
            // frmListaPrecios
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(1066, 538);
            this.Controls.Add(this.frmListaPrecios_Fill_Panel);
            this.Controls.Add(this._frmListaPrecios_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._frmListaPrecios_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._frmListaPrecios_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._frmListaPrecios_UltraFormManager_Dock_Area_Bottom);
            this.Name = "frmListaPrecios";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Manteniminento de ListaPrecios x Precio/Dscto.";
            this.Load += new System.EventHandler(this.frmListaPrecios_Load);
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboAlmacen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboListaPrecios)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboMoneda)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox3)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuscarProducto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uvListaPrecios)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            this.frmListaPrecios_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmListaPrecios_Fill_Panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox groupBox1;
        private Infragistics.Win.Misc.UltraLabel label1;
        private Infragistics.Win.Misc.UltraGroupBox groupBox3;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdData;
        private Infragistics.Win.Misc.UltraValidator uvListaPrecios;
        private Infragistics.Win.Misc.UltraButton btnGuardar;
        private Infragistics.Win.Misc.UltraButton btnAgregar;
        private Infragistics.Win.Misc.UltraLabel label2;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtBuscarProducto;
        private Infragistics.Win.Misc.UltraLabel label3;
        private Infragistics.Win.Misc.UltraButton btnMigracion;
        private Infragistics.Win.Misc.UltraLabel lblContadorFilas;
        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.Misc.UltraPanel frmListaPrecios_Fill_Panel;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmListaPrecios_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmListaPrecios_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmListaPrecios_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmListaPrecios_UltraFormManager_Dock_Area_Bottom;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboMoneda;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboListaPrecios;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboAlmacen;
        private Infragistics.Win.Misc.UltraButton BtnImprimir;
    }
}