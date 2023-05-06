using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    partial class frmBandejaOrdenCompra
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
            Infragistics.Win.Misc.UltraLabel label25;
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("NroOrdenCompra");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("AreaSolicita");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Estado");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FechaActualiza");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FechaCrea", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FechaEntrega");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FechaRegistro");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Importe");
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("RazonSocialProveedor");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("UsuarioActualiza");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("UsuarioCrea");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DocInterno");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Moneda", 0);
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton("btnBuscarProveedor");
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            this.groupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.button1 = new Infragistics.Win.Misc.UltraButton();
            this.btnEstado = new Infragistics.Win.Misc.UltraButton();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.btnEditar = new Infragistics.Win.Misc.UltraButton();
            this.grdData = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.btnEliminar = new Infragistics.Win.Misc.UltraButton();
            this.btnAgregar = new Infragistics.Win.Misc.UltraButton();
            this.lblContadorFilas = new Infragistics.Win.Misc.UltraLabel();
            this.groupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.cboMoneda = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.cboEstado = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.label1 = new Infragistics.Win.Misc.UltraLabel();
            this.cboAreaSolicita = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.label9 = new Infragistics.Win.Misc.UltraLabel();
            this.dtpFechaRegistroFin = new System.Windows.Forms.DateTimePicker();
            this.label6 = new Infragistics.Win.Misc.UltraLabel();
            this.dtpFechaRegistroIni = new System.Windows.Forms.DateTimePicker();
            this.label7 = new Infragistics.Win.Misc.UltraLabel();
            this.txtProveedor = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label2 = new Infragistics.Win.Misc.UltraLabel();
            this.btnBuscar = new Infragistics.Win.Misc.UltraButton();
            this.dtpFechaEntregaFin = new System.Windows.Forms.DateTimePicker();
            this.label4 = new Infragistics.Win.Misc.UltraLabel();
            this.dtpFechaEntregaIni = new System.Windows.Forms.DateTimePicker();
            this.label3 = new Infragistics.Win.Misc.UltraLabel();
            label25 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox2)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboMoneda)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboEstado)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAreaSolicita)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProveedor)).BeginInit();
            this.SuspendLayout();
            // 
            // label25
            // 
            label25.AutoSize = true;
            label25.Location = new System.Drawing.Point(583, 57);
            label25.Name = "label25";
            label25.Size = new System.Drawing.Size(48, 14);
            label25.TabIndex = 5009;
            label25.Text = "Moneda:";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.flowLayoutPanel1);
            this.groupBox2.Controls.Add(this.ultraLabel1);
            this.groupBox2.Controls.Add(this.btnEditar);
            this.groupBox2.Controls.Add(this.grdData);
            this.groupBox2.Controls.Add(this.btnEliminar);
            this.groupBox2.Controls.Add(this.btnAgregar);
            this.groupBox2.Controls.Add(this.lblContadorFilas);
            this.groupBox2.Location = new System.Drawing.Point(8, 100);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1117, 364);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.Text = "Resultado de Búsqueda";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.button1);
            this.flowLayoutPanel1.Controls.Add(this.btnEstado);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(343, 331);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(271, 29);
            this.flowLayoutPanel1.TabIndex = 152;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.Image = global::SAMBHS.Windows.WinClient.UI.Resource.brick_add;
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance1.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance1.TextHAlignAsString = "Right";
            appearance1.TextVAlignAsString = "Middle";
            this.button1.Appearance = appearance1;
            this.button1.Location = new System.Drawing.Point(108, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(160, 23);
            this.button1.TabIndex = 149;
            this.button1.Text = "&Clonar Orden de Compra";
            this.button1.Visible = false;
            // 
            // btnEstado
            // 
            this.btnEstado.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance2.Image = global::SAMBHS.Windows.WinClient.UI.Properties.Resources.folder_table;
            this.btnEstado.Appearance = appearance2;
            this.btnEstado.Location = new System.Drawing.Point(28, 3);
            this.btnEstado.Name = "btnEstado";
            this.btnEstado.Size = new System.Drawing.Size(74, 23);
            this.btnEstado.TabIndex = 1;
            this.btnEstado.Text = "Estado";
            this.btnEstado.Click += new System.EventHandler(this.btnEstado_Click);
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(6, 340);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(154, 14);
            this.ultraLabel1.TabIndex = 151;
            this.ultraLabel1.Text = "Mostrar / Ocultar Filtro: Ctrl+F";
            // 
            // btnEditar
            // 
            this.btnEditar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance3.Image = global::SAMBHS.Windows.WinClient.UI.Resource.pencil;
            appearance3.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance3.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance3.TextHAlignAsString = "Right";
            appearance3.TextVAlignAsString = "Middle";
            this.btnEditar.Appearance = appearance3;
            this.btnEditar.Enabled = false;
            this.btnEditar.Location = new System.Drawing.Point(783, 335);
            this.btnEditar.Name = "btnEditar";
            this.btnEditar.Size = new System.Drawing.Size(155, 23);
            this.btnEditar.TabIndex = 0;
            this.btnEditar.Text = "E&ditar Orden de Compra";
            this.btnEditar.Click += new System.EventHandler(this.btnEditar_Click);
            // 
            // grdData
            // 
            this.grdData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdData.CausesValidation = false;
            appearance4.BackColor = System.Drawing.SystemColors.Window;
            appearance4.BackColor2 = System.Drawing.Color.LightSteelBlue;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            this.grdData.DisplayLayout.Appearance = appearance4;
            this.grdData.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn4.Header.Caption = "Nro. Orden Compra";
            ultraGridColumn4.Header.VisiblePosition = 0;
            ultraGridColumn4.Width = 82;
            ultraGridColumn1.Header.VisiblePosition = 4;
            ultraGridColumn1.Width = 95;
            ultraGridColumn2.Header.VisiblePosition = 7;
            ultraGridColumn2.Width = 69;
            ultraGridColumn3.Header.Caption = "FechaAct";
            ultraGridColumn3.Header.VisiblePosition = 12;
            ultraGridColumn3.Width = 73;
            ultraGridColumn5.Header.VisiblePosition = 10;
            ultraGridColumn5.Width = 69;
            ultraGridColumn8.Header.VisiblePosition = 3;
            ultraGridColumn8.Width = 69;
            ultraGridColumn9.Header.VisiblePosition = 2;
            ultraGridColumn9.Width = 71;
            appearance5.TextHAlignAsString = "Right";
            ultraGridColumn10.CellAppearance = appearance5;
            ultraGridColumn10.Header.VisiblePosition = 5;
            ultraGridColumn10.Width = 69;
            ultraGridColumn11.Header.VisiblePosition = 1;
            ultraGridColumn11.Width = 203;
            ultraGridColumn12.Header.Caption = "UsuarioAct";
            ultraGridColumn12.Header.VisiblePosition = 11;
            ultraGridColumn12.Width = 80;
            ultraGridColumn13.Header.VisiblePosition = 9;
            ultraGridColumn13.Width = 69;
            ultraGridColumn7.Header.Caption = "Doc. Interno";
            ultraGridColumn7.Header.VisiblePosition = 8;
            ultraGridColumn7.Width = 88;
            ultraGridColumn6.Header.VisiblePosition = 6;
            ultraGridColumn6.Width = 48;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn4,
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn5,
            ultraGridColumn8,
            ultraGridColumn9,
            ultraGridColumn10,
            ultraGridColumn11,
            ultraGridColumn12,
            ultraGridColumn13,
            ultraGridColumn7,
            ultraGridColumn6});
            this.grdData.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdData.DisplayLayout.InterBandSpacing = 10;
            this.grdData.DisplayLayout.MaxColScrollRegions = 1;
            this.grdData.DisplayLayout.MaxRowScrollRegions = 1;
            this.grdData.DisplayLayout.NewColumnLoadStyle = Infragistics.Win.UltraWinGrid.NewColumnLoadStyle.Hide;
            appearance6.BackColor = System.Drawing.SystemColors.Highlight;
            appearance6.BackColor2 = System.Drawing.SystemColors.ActiveCaption;
            appearance6.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            appearance6.FontData.BoldAsString = "True";
            appearance6.ForeColor = System.Drawing.Color.White;
            this.grdData.DisplayLayout.Override.ActiveRowAppearance = appearance6;
            this.grdData.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdData.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdData.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.grdData.DisplayLayout.Override.AllowRowSummaries = Infragistics.Win.UltraWinGrid.AllowRowSummaries.False;
            this.grdData.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            appearance7.BackColor = System.Drawing.Color.Transparent;
            this.grdData.DisplayLayout.Override.CardAreaAppearance = appearance7;
            appearance8.BackColor = System.Drawing.SystemColors.Control;
            appearance8.BackColor2 = System.Drawing.SystemColors.ControlLightLight;
            appearance8.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            this.grdData.DisplayLayout.Override.CellAppearance = appearance8;
            this.grdData.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            appearance9.BackColor = System.Drawing.SystemColors.Control;
            appearance9.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance9.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            appearance9.ThemedElementAlpha = Infragistics.Win.Alpha.Transparent;
            this.grdData.DisplayLayout.Override.HeaderAppearance = appearance9;
            this.grdData.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance10.AlphaLevel = ((short)(187));
            this.grdData.DisplayLayout.Override.RowAlternateAppearance = appearance10;
            appearance11.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.grdData.DisplayLayout.Override.RowSelectorAppearance = appearance11;
            this.grdData.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            this.grdData.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.grdData.DisplayLayout.RowConnectorColor = System.Drawing.SystemColors.ControlDarkDark;
            this.grdData.DisplayLayout.RowConnectorStyle = Infragistics.Win.UltraWinGrid.RowConnectorStyle.Dashed;
            this.grdData.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdData.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdData.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.grdData.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdData.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.grdData.Location = new System.Drawing.Point(5, 36);
            this.grdData.Margin = new System.Windows.Forms.Padding(2);
            this.grdData.Name = "grdData";
            this.grdData.Size = new System.Drawing.Size(1106, 295);
            this.grdData.TabIndex = 148;
            this.grdData.DoubleClickRow += new Infragistics.Win.UltraWinGrid.DoubleClickRowEventHandler(this.grdData_DoubleClickRow);
            this.grdData.KeyDown += new System.Windows.Forms.KeyEventHandler(this.grdData_KeyDown);
            this.grdData.MouseDown += new System.Windows.Forms.MouseEventHandler(this.grdData_MouseDown);
            // 
            // btnEliminar
            // 
            this.btnEliminar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance12.Image = global::SAMBHS.Windows.WinClient.UI.Resource.delete;
            appearance12.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance12.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance12.TextHAlignAsString = "Right";
            appearance12.TextVAlignAsString = "Middle";
            this.btnEliminar.Appearance = appearance12;
            this.btnEliminar.Enabled = false;
            this.btnEliminar.Location = new System.Drawing.Point(944, 335);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(167, 23);
            this.btnEliminar.TabIndex = 2;
            this.btnEliminar.Text = "&Eliminar Orden de Compra";
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // btnAgregar
            // 
            this.btnAgregar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance13.Image = global::SAMBHS.Windows.WinClient.UI.Resource.add;
            appearance13.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance13.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance13.TextHAlignAsString = "Right";
            appearance13.TextVAlignAsString = "Middle";
            this.btnAgregar.Appearance = appearance13;
            this.btnAgregar.Location = new System.Drawing.Point(617, 335);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(160, 23);
            this.btnAgregar.TabIndex = 1;
            this.btnAgregar.Text = "&Nueva Orden de Compra";
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
            // 
            // lblContadorFilas
            // 
            this.lblContadorFilas.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance14.TextHAlignAsString = "Right";
            appearance14.TextVAlignAsString = "Middle";
            this.lblContadorFilas.Appearance = appearance14;
            this.lblContadorFilas.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.2F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContadorFilas.Location = new System.Drawing.Point(899, 16);
            this.lblContadorFilas.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblContadorFilas.Name = "lblContadorFilas";
            this.lblContadorFilas.Size = new System.Drawing.Size(213, 19);
            this.lblContadorFilas.TabIndex = 143;
            this.lblContadorFilas.Text = "No se ha realizado la búsqueda aún.";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.cboMoneda);
            this.groupBox1.Controls.Add(label25);
            this.groupBox1.Controls.Add(this.cboEstado);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cboAreaSolicita);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.dtpFechaRegistroFin);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.dtpFechaRegistroIni);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txtProveedor);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btnBuscar);
            this.groupBox1.Controls.Add(this.dtpFechaEntregaFin);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.dtpFechaEntregaIni);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(8, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1117, 86);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.Text = "Filtro de Búsqueda:";
            // 
            // cboMoneda
            // 
            this.cboMoneda.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboMoneda.Location = new System.Drawing.Point(634, 53);
            this.cboMoneda.Name = "cboMoneda";
            this.cboMoneda.Size = new System.Drawing.Size(91, 21);
            this.cboMoneda.TabIndex = 8;
            // 
            // cboEstado
            // 
            this.cboEstado.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboEstado.Location = new System.Drawing.Point(436, 53);
            this.cboEstado.Name = "cboEstado";
            this.cboEstado.Size = new System.Drawing.Size(140, 21);
            this.cboEstado.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(371, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 14);
            this.label1.TabIndex = 109;
            this.label1.Text = "Estado:";
            // 
            // cboAreaSolicita
            // 
            this.cboAreaSolicita.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboAreaSolicita.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboAreaSolicita.Location = new System.Drawing.Point(842, 22);
            this.cboAreaSolicita.Name = "cboAreaSolicita";
            this.cboAreaSolicita.Size = new System.Drawing.Size(140, 21);
            this.cboAreaSolicita.TabIndex = 6;
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.Black;
            this.label9.Location = new System.Drawing.Point(743, 27);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(93, 14);
            this.label9.TabIndex = 107;
            this.label9.Text = "Área que Solicita:";
            // 
            // dtpFechaRegistroFin
            // 
            this.dtpFechaRegistroFin.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFechaRegistroFin.Location = new System.Drawing.Point(240, 23);
            this.dtpFechaRegistroFin.Margin = new System.Windows.Forms.Padding(2);
            this.dtpFechaRegistroFin.Name = "dtpFechaRegistroFin";
            this.dtpFechaRegistroFin.Size = new System.Drawing.Size(102, 20);
            this.dtpFechaRegistroFin.TabIndex = 2;
            this.dtpFechaRegistroFin.ValueChanged += new System.EventHandler(this.dtpFechaRegistroFin_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(216, 27);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 14);
            this.label6.TabIndex = 105;
            this.label6.Text = "Al:";
            // 
            // dtpFechaRegistroIni
            // 
            this.dtpFechaRegistroIni.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFechaRegistroIni.Location = new System.Drawing.Point(114, 23);
            this.dtpFechaRegistroIni.Margin = new System.Windows.Forms.Padding(2);
            this.dtpFechaRegistroIni.Name = "dtpFechaRegistroIni";
            this.dtpFechaRegistroIni.Size = new System.Drawing.Size(96, 20);
            this.dtpFechaRegistroIni.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(11, 27);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(102, 14);
            this.label7.TabIndex = 102;
            this.label7.Text = "Fecha Registro del: ";
            // 
            // txtProveedor
            // 
            this.txtProveedor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance15.Image = global::SAMBHS.Windows.WinClient.UI.Resource.system_search;
            editorButton1.Appearance = appearance15;
            editorButton1.Key = "btnBuscarProveedor";
            this.txtProveedor.ButtonsLeft.Add(editorButton1);
            this.txtProveedor.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.WindowsVista;
            this.txtProveedor.Location = new System.Drawing.Point(436, 22);
            this.txtProveedor.Name = "txtProveedor";
            this.txtProveedor.Size = new System.Drawing.Size(289, 21);
            this.txtProveedor.TabIndex = 5;
            this.txtProveedor.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.txtProveedor_EditorButtonClick);
            this.txtProveedor.Validated += new System.EventHandler(this.txtProveedor_Validated);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(371, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 14);
            this.label2.TabIndex = 100;
            this.label2.Text = "Proveedor:";
            // 
            // btnBuscar
            // 
            this.btnBuscar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance16.Image = global::SAMBHS.Windows.WinClient.UI.Resource.system_search;
            appearance16.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance16.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance16.TextHAlignAsString = "Center";
            appearance16.TextVAlignAsString = "Middle";
            this.btnBuscar.Appearance = appearance16;
            this.btnBuscar.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBuscar.Location = new System.Drawing.Point(897, 53);
            this.btnBuscar.Margin = new System.Windows.Forms.Padding(2);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(85, 24);
            this.btnBuscar.TabIndex = 9;
            this.btnBuscar.Text = "&Buscar";
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // dtpFechaEntregaFin
            // 
            this.dtpFechaEntregaFin.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFechaEntregaFin.Location = new System.Drawing.Point(240, 56);
            this.dtpFechaEntregaFin.Margin = new System.Windows.Forms.Padding(2);
            this.dtpFechaEntregaFin.Name = "dtpFechaEntregaFin";
            this.dtpFechaEntregaFin.Size = new System.Drawing.Size(102, 20);
            this.dtpFechaEntregaFin.TabIndex = 4;
            this.dtpFechaEntregaFin.ValueChanged += new System.EventHandler(this.dtpFechaEntregaFin_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(216, 60);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 14);
            this.label4.TabIndex = 16;
            this.label4.Text = "Al:";
            // 
            // dtpFechaEntregaIni
            // 
            this.dtpFechaEntregaIni.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFechaEntregaIni.Location = new System.Drawing.Point(114, 56);
            this.dtpFechaEntregaIni.Margin = new System.Windows.Forms.Padding(2);
            this.dtpFechaEntregaIni.Name = "dtpFechaEntregaIni";
            this.dtpFechaEntregaIni.Size = new System.Drawing.Size(96, 20);
            this.dtpFechaEntregaIni.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 14);
            this.label3.TabIndex = 2;
            this.label3.Text = "Fecha Entrega del: ";
            // 
            // frmBandejaOrdenCompra
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1133, 473);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmBandejaOrdenCompra";
            this.ShowIcon = false;
            this.Text = "Bandeja de Órdenes de Compras";
            this.Load += new System.EventHandler(this.frmBandejaOrdenCompra_Load);
            ((System.ComponentModel.ISupportInitialize)(this.groupBox2)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboMoneda)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboEstado)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAreaSolicita)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProveedor)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox groupBox2;
        private Infragistics.Win.Misc.UltraButton btnEditar;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdData;
        private Infragistics.Win.Misc.UltraButton btnEliminar;
        private Infragistics.Win.Misc.UltraButton btnAgregar;
        private Infragistics.Win.Misc.UltraLabel lblContadorFilas;
        private Infragistics.Win.Misc.UltraGroupBox groupBox1;
        private Infragistics.Win.Misc.UltraButton btnBuscar;
        private System.Windows.Forms.DateTimePicker dtpFechaEntregaFin;
        private Infragistics.Win.Misc.UltraLabel label4;
        private System.Windows.Forms.DateTimePicker dtpFechaEntregaIni;
        private Infragistics.Win.Misc.UltraLabel label3;
        private System.Windows.Forms.DateTimePicker dtpFechaRegistroFin;
        private Infragistics.Win.Misc.UltraLabel label6;
        private System.Windows.Forms.DateTimePicker dtpFechaRegistroIni;
        private Infragistics.Win.Misc.UltraLabel label7;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtProveedor;
        private Infragistics.Win.Misc.UltraLabel label2;
        private UltraComboEditor cboAreaSolicita;
        private Infragistics.Win.Misc.UltraLabel label9;
        private Infragistics.Win.Misc.UltraButton button1;
        private UltraComboEditor cboEstado;
        private Infragistics.Win.Misc.UltraLabel label1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private UltraComboEditor cboMoneda;
        private Infragistics.Win.Misc.UltraButton btnEstado;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}