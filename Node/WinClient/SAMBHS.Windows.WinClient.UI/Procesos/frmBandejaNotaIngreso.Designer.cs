﻿using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    partial class frmBandejaNotaIngreso
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
            Infragistics.Win.Misc.UltraLabel label25;
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton("btnBuscarCliente");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmBandejaNotaIngreso));
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_MesCorrelativo", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_AlmacenOrigen");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("t_Fecha");
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_NombreProveedor");
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_DescripcionMotivo");
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("t_InsertaFecha");
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("t_ActualizaFecha");
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("RegistroOrigen");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_UsuarioCreacion", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_UsuarioModificacion", 1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Moneda", 2);
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            this.groupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.cboMoneda = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.txtCliente = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.txtRazonSocial = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.btnBuscar = new Infragistics.Win.Misc.UltraButton();
            this.dtpFechaFin = new System.Windows.Forms.DateTimePicker();
            this.label4 = new Infragistics.Win.Misc.UltraLabel();
            this.dtpFechaInicio = new System.Windows.Forms.DateTimePicker();
            this.label3 = new Infragistics.Win.Misc.UltraLabel();
            this.cboMotivo = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.label13 = new Infragistics.Win.Misc.UltraLabel();
            this.cboAlmacen = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.label2 = new Infragistics.Win.Misc.UltraLabel();
            this.label1 = new Infragistics.Win.Misc.UltraLabel();
            this.groupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            this.lblDocumentoExportado = new Infragistics.Win.Misc.UltraLabel();
            this.btnExportarBandeja = new Infragistics.Win.Misc.UltraButton();
            this.chkFiltroPersonalizado = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkBandejaAgrupable = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.btnEditar = new Infragistics.Win.Misc.UltraButton();
            this.grdData = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.btnEliminar = new Infragistics.Win.Misc.UltraButton();
            this.btnAgregar = new Infragistics.Win.Misc.UltraButton();
            this.lblContadorFilas = new Infragistics.Win.Misc.UltraLabel();
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this.frmBandejaNotaIngreso_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            label25 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboMoneda)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCliente)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRazonSocial)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboMotivo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAlmacen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox2)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkFiltroPersonalizado)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkBandejaAgrupable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            this.frmBandejaNotaIngreso_Fill_Panel.ClientArea.SuspendLayout();
            this.frmBandejaNotaIngreso_Fill_Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label25
            // 
            label25.AutoSize = true;
            label25.Location = new System.Drawing.Point(408, 51);
            label25.Name = "label25";
            label25.Size = new System.Drawing.Size(48, 14);
            label25.TabIndex = 5010;
            label25.Text = "Moneda:";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.cboMoneda);
            this.groupBox1.Controls.Add(label25);
            this.groupBox1.Controls.Add(this.txtCliente);
            this.groupBox1.Controls.Add(this.txtRazonSocial);
            this.groupBox1.Controls.Add(this.btnBuscar);
            this.groupBox1.Controls.Add(this.dtpFechaFin);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.dtpFechaInicio);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cboMotivo);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.cboAlmacen);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1021, 78);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.Text = "Filtro de Búsqueda:";
            // 
            // cboMoneda
            // 
            appearance1.Image = global::SAMBHS.Windows.WinClient.UI.Resource.money_dollar;
            this.cboMoneda.Appearance = appearance1;
            this.cboMoneda.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboMoneda.Location = new System.Drawing.Point(455, 47);
            this.cboMoneda.Name = "cboMoneda";
            this.cboMoneda.Size = new System.Drawing.Size(96, 22);
            this.cboMoneda.TabIndex = 6;
            // 
            // txtCliente
            // 
            appearance2.Image = ((object)(resources.GetObject("appearance2.Image")));
            editorButton1.Appearance = appearance2;
            editorButton1.Key = "btnBuscarCliente";
            this.txtCliente.ButtonsLeft.Add(editorButton1);
            this.txtCliente.Location = new System.Drawing.Point(623, 21);
            this.txtCliente.Name = "txtCliente";
            this.txtCliente.NullText = "RUC/DNI";
            appearance3.FontData.ItalicAsString = "True";
            appearance3.ForeColor = System.Drawing.Color.Silver;
            this.txtCliente.NullTextAppearance = appearance3;
            this.txtCliente.Size = new System.Drawing.Size(113, 21);
            this.txtCliente.TabIndex = 5008;
            this.txtCliente.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.txtCliente_EditorButtonClick);
            this.txtCliente.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCliente_KeyPress);
            this.txtCliente.Validated += new System.EventHandler(this.txtCliente_Validated);
            // 
            // txtRazonSocial
            // 
            this.txtRazonSocial.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRazonSocial.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtRazonSocial.Enabled = false;
            this.txtRazonSocial.Location = new System.Drawing.Point(739, 21);
            this.txtRazonSocial.Margin = new System.Windows.Forms.Padding(2);
            this.txtRazonSocial.MaxLength = 50;
            this.txtRazonSocial.Name = "txtRazonSocial";
            this.txtRazonSocial.ReadOnly = true;
            this.txtRazonSocial.Size = new System.Drawing.Size(270, 21);
            this.txtRazonSocial.TabIndex = 5007;
            // 
            // btnBuscar
            // 
            this.btnBuscar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance4.Image = global::SAMBHS.Windows.WinClient.UI.Resource.system_search;
            appearance4.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance4.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance4.TextHAlignAsString = "Center";
            appearance4.TextVAlignAsString = "Middle";
            this.btnBuscar.Appearance = appearance4;
            this.btnBuscar.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBuscar.Location = new System.Drawing.Point(924, 47);
            this.btnBuscar.Margin = new System.Windows.Forms.Padding(2);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(85, 24);
            this.btnBuscar.TabIndex = 7;
            this.btnBuscar.Text = "&Buscar";
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // dtpFechaFin
            // 
            this.dtpFechaFin.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFechaFin.Location = new System.Drawing.Point(302, 47);
            this.dtpFechaFin.Margin = new System.Windows.Forms.Padding(2);
            this.dtpFechaFin.Name = "dtpFechaFin";
            this.dtpFechaFin.Size = new System.Drawing.Size(102, 20);
            this.dtpFechaFin.TabIndex = 5;
            this.dtpFechaFin.ValueChanged += new System.EventHandler(this.dtpFechaFin_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(239, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 14);
            this.label4.TabIndex = 16;
            this.label4.Text = "Fecha Fin:";
            // 
            // dtpFechaInicio
            // 
            this.dtpFechaInicio.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFechaInicio.Location = new System.Drawing.Point(77, 47);
            this.dtpFechaInicio.Margin = new System.Windows.Forms.Padding(2);
            this.dtpFechaInicio.Name = "dtpFechaInicio";
            this.dtpFechaInicio.Size = new System.Drawing.Size(96, 20);
            this.dtpFechaInicio.TabIndex = 4;
            this.dtpFechaInicio.ValueChanged += new System.EventHandler(this.dtpFechaInicio_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 51);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 14);
            this.label3.TabIndex = 2;
            this.label3.Text = "Fecha Inicio:";
            // 
            // cboMotivo
            // 
            this.cboMotivo.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboMotivo.Location = new System.Drawing.Point(302, 21);
            this.cboMotivo.Name = "cboMotivo";
            this.cboMotivo.Size = new System.Drawing.Size(249, 21);
            this.cboMotivo.TabIndex = 1;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(255, 24);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(41, 14);
            this.label13.TabIndex = 14;
            this.label13.Text = "Motivo:";
            // 
            // cboAlmacen
            // 
            this.cboAlmacen.DropDownListWidth = 200;
            this.cboAlmacen.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboAlmacen.Location = new System.Drawing.Point(77, 21);
            this.cboAlmacen.Name = "cboAlmacen";
            this.cboAlmacen.Size = new System.Drawing.Size(151, 21);
            this.cboAlmacen.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(566, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 14);
            this.label2.TabIndex = 1;
            this.label2.Text = "Proveedor:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "Almacén:";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.lblDocumentoExportado);
            this.groupBox2.Controls.Add(this.btnExportarBandeja);
            this.groupBox2.Controls.Add(this.chkFiltroPersonalizado);
            this.groupBox2.Controls.Add(this.chkBandejaAgrupable);
            this.groupBox2.Controls.Add(this.btnEditar);
            this.groupBox2.Controls.Add(this.grdData);
            this.groupBox2.Controls.Add(this.btnEliminar);
            this.groupBox2.Controls.Add(this.btnAgregar);
            this.groupBox2.Controls.Add(this.lblContadorFilas);
            this.groupBox2.Location = new System.Drawing.Point(12, 96);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1021, 368);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.Text = "Resultado de Búsqueda";
            // 
            // lblDocumentoExportado
            // 
            this.lblDocumentoExportado.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblDocumentoExportado.Location = new System.Drawing.Point(222, 339);
            this.lblDocumentoExportado.Name = "lblDocumentoExportado";
            this.lblDocumentoExportado.Size = new System.Drawing.Size(291, 21);
            this.lblDocumentoExportado.TabIndex = 168;
            // 
            // btnExportarBandeja
            // 
            this.btnExportarBandeja.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            appearance5.Image = global::SAMBHS.Windows.WinClient.UI.Resource.page_excel1;
            appearance5.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance5.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance5.TextHAlignAsString = "Right";
            appearance5.TextVAlignAsString = "Middle";
            this.btnExportarBandeja.Appearance = appearance5;
            this.btnExportarBandeja.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExportarBandeja.Location = new System.Drawing.Point(4, 339);
            this.btnExportarBandeja.Margin = new System.Windows.Forms.Padding(2);
            this.btnExportarBandeja.Name = "btnExportarBandeja";
            this.btnExportarBandeja.Size = new System.Drawing.Size(22, 24);
            this.btnExportarBandeja.TabIndex = 167;
            this.btnExportarBandeja.Click += new System.EventHandler(this.btnExportarBandeja_Click);
            // 
            // chkFiltroPersonalizado
            // 
            this.chkFiltroPersonalizado.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkFiltroPersonalizado.Location = new System.Drawing.Point(34, 341);
            this.chkFiltroPersonalizado.Name = "chkFiltroPersonalizado";
            this.chkFiltroPersonalizado.Size = new System.Drawing.Size(108, 20);
            this.chkFiltroPersonalizado.TabIndex = 166;
            this.chkFiltroPersonalizado.Text = "Filtro Avanzado";
            this.chkFiltroPersonalizado.CheckedChanged += new System.EventHandler(this.chkFiltroPersonalizado_CheckedChanged);
            // 
            // chkBandejaAgrupable
            // 
            this.chkBandejaAgrupable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkBandejaAgrupable.Location = new System.Drawing.Point(143, 341);
            this.chkBandejaAgrupable.Name = "chkBandejaAgrupable";
            this.chkBandejaAgrupable.Size = new System.Drawing.Size(77, 20);
            this.chkBandejaAgrupable.TabIndex = 165;
            this.chkBandejaAgrupable.Text = "Agrupable";
            this.chkBandejaAgrupable.Visible = false;
            // 
            // btnEditar
            // 
            this.btnEditar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance6.Image = global::SAMBHS.Windows.WinClient.UI.Resource.pencil;
            appearance6.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance6.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance6.TextHAlignAsString = "Right";
            appearance6.TextVAlignAsString = "Middle";
            this.btnEditar.Appearance = appearance6;
            this.btnEditar.Enabled = false;
            this.btnEditar.Location = new System.Drawing.Point(706, 339);
            this.btnEditar.Name = "btnEditar";
            this.btnEditar.Size = new System.Drawing.Size(147, 25);
            this.btnEditar.TabIndex = 1;
            this.btnEditar.Text = "E&ditar Nota de Ingreso";
            this.btnEditar.Click += new System.EventHandler(this.btnEditar_Click);
            // 
            // grdData
            // 
            this.grdData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdData.CausesValidation = false;
            appearance7.BackColor = System.Drawing.SystemColors.Window;
            appearance7.BackColor2 = System.Drawing.Color.LightSteelBlue;
            appearance7.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            this.grdData.DisplayLayout.Appearance = appearance7;
            appearance8.TextHAlignAsString = "Center";
            ultraGridColumn1.CellAppearance = appearance8;
            ultraGridColumn1.Header.Caption = "Nro. Registro";
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Width = 108;
            ultraGridColumn2.Header.Caption = "Almacén";
            ultraGridColumn2.Header.VisiblePosition = 2;
            ultraGridColumn2.Width = 189;
            appearance9.TextHAlignAsString = "Center";
            ultraGridColumn10.CellAppearance = appearance9;
            ultraGridColumn10.Header.Caption = "Fecha";
            ultraGridColumn10.Header.VisiblePosition = 3;
            ultraGridColumn10.Width = 100;
            appearance10.TextHAlignAsString = "Left";
            ultraGridColumn11.CellAppearance = appearance10;
            ultraGridColumn11.Header.Caption = "Proveedor";
            ultraGridColumn11.Header.VisiblePosition = 4;
            ultraGridColumn11.Width = 185;
            appearance11.TextHAlignAsString = "Left";
            ultraGridColumn12.CellAppearance = appearance11;
            ultraGridColumn12.Header.Caption = "Motivo";
            ultraGridColumn12.Header.VisiblePosition = 5;
            ultraGridColumn12.Width = 185;
            appearance12.TextHAlignAsString = "Left";
            ultraGridColumn13.CellAppearance = appearance12;
            ultraGridColumn13.Format = "dd/MM/yyyy hh:mm tt";
            ultraGridColumn13.Header.Caption = "Fecha Crea.";
            ultraGridColumn13.Header.VisiblePosition = 8;
            ultraGridColumn13.Width = 128;
            appearance13.TextHAlignAsString = "Left";
            ultraGridColumn14.CellAppearance = appearance13;
            ultraGridColumn14.Format = "dd/MM/yyyy hh:mm tt";
            ultraGridColumn14.Header.Caption = "Fecha Act.";
            ultraGridColumn14.Header.VisiblePosition = 10;
            ultraGridColumn14.Width = 118;
            ultraGridColumn6.Header.Caption = "Nº Registro Origen";
            ultraGridColumn6.Header.VisiblePosition = 1;
            ultraGridColumn3.Header.Caption = "Usuario Crea.";
            ultraGridColumn3.Header.VisiblePosition = 7;
            ultraGridColumn4.Header.Caption = "Usuario Act.";
            ultraGridColumn4.Header.VisiblePosition = 9;
            ultraGridColumn4.Width = 88;
            ultraGridColumn5.Header.VisiblePosition = 6;
            ultraGridColumn5.Width = 50;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn10,
            ultraGridColumn11,
            ultraGridColumn12,
            ultraGridColumn13,
            ultraGridColumn14,
            ultraGridColumn6,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5});
            this.grdData.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdData.DisplayLayout.InterBandSpacing = 10;
            this.grdData.DisplayLayout.MaxColScrollRegions = 1;
            this.grdData.DisplayLayout.MaxRowScrollRegions = 1;
            this.grdData.DisplayLayout.NewColumnLoadStyle = Infragistics.Win.UltraWinGrid.NewColumnLoadStyle.Hide;
            appearance14.BackColor = System.Drawing.SystemColors.Highlight;
            appearance14.BackColor2 = System.Drawing.SystemColors.ActiveCaption;
            appearance14.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            appearance14.FontData.BoldAsString = "True";
            appearance14.ForeColor = System.Drawing.Color.White;
            this.grdData.DisplayLayout.Override.ActiveRowAppearance = appearance14;
            this.grdData.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdData.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdData.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.grdData.DisplayLayout.Override.AllowRowSummaries = Infragistics.Win.UltraWinGrid.AllowRowSummaries.False;
            this.grdData.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            appearance15.BackColor = System.Drawing.Color.Transparent;
            this.grdData.DisplayLayout.Override.CardAreaAppearance = appearance15;
            appearance16.BackColor = System.Drawing.SystemColors.Control;
            appearance16.BackColor2 = System.Drawing.SystemColors.ControlLightLight;
            appearance16.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            this.grdData.DisplayLayout.Override.CellAppearance = appearance16;
            this.grdData.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            appearance17.BackColor = System.Drawing.SystemColors.Control;
            appearance17.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance17.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            appearance17.ThemedElementAlpha = Infragistics.Win.Alpha.Transparent;
            this.grdData.DisplayLayout.Override.HeaderAppearance = appearance17;
            this.grdData.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance18.AlphaLevel = ((short)(187));
            this.grdData.DisplayLayout.Override.RowAlternateAppearance = appearance18;
            appearance19.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.grdData.DisplayLayout.Override.RowSelectorAppearance = appearance19;
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
            this.grdData.Size = new System.Drawing.Size(1010, 299);
            this.grdData.TabIndex = 3;
            this.grdData.AfterRowActivate += new System.EventHandler(this.grdData_AfterRowActivate);
            this.grdData.DoubleClickRow += new Infragistics.Win.UltraWinGrid.DoubleClickRowEventHandler(this.grdData_DoubleClickRow);
            this.grdData.KeyDown += new System.Windows.Forms.KeyEventHandler(this.grdData_KeyDown);
            this.grdData.MouseDown += new System.Windows.Forms.MouseEventHandler(this.grdData_MouseDown);
            // 
            // btnEliminar
            // 
            this.btnEliminar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance20.Image = global::SAMBHS.Windows.WinClient.UI.Resource.delete;
            appearance20.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance20.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance20.TextHAlignAsString = "Right";
            appearance20.TextVAlignAsString = "Middle";
            this.btnEliminar.Appearance = appearance20;
            this.btnEliminar.Enabled = false;
            this.btnEliminar.Location = new System.Drawing.Point(859, 339);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(156, 25);
            this.btnEliminar.TabIndex = 2;
            this.btnEliminar.Text = "&Eliminar Nota de Ingreso";
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // btnAgregar
            // 
            this.btnAgregar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance21.Image = global::SAMBHS.Windows.WinClient.UI.Resource.add;
            appearance21.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance21.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance21.TextHAlignAsString = "Right";
            appearance21.TextVAlignAsString = "Middle";
            this.btnAgregar.Appearance = appearance21;
            this.btnAgregar.Location = new System.Drawing.Point(553, 339);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(147, 25);
            this.btnAgregar.TabIndex = 0;
            this.btnAgregar.Text = "&Nueva Nota de Ingreso";
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
            // 
            // lblContadorFilas
            // 
            this.lblContadorFilas.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance22.TextHAlignAsString = "Right";
            appearance22.TextVAlignAsString = "Middle";
            this.lblContadorFilas.Appearance = appearance22;
            this.lblContadorFilas.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.2F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContadorFilas.Location = new System.Drawing.Point(803, 16);
            this.lblContadorFilas.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblContadorFilas.Name = "lblContadorFilas";
            this.lblContadorFilas.Size = new System.Drawing.Size(213, 19);
            this.lblContadorFilas.TabIndex = 143;
            this.lblContadorFilas.Text = "No se ha realizado la búsqueda aún.";
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // frmBandejaNotaIngreso_Fill_Panel
            // 
            // 
            // frmBandejaNotaIngreso_Fill_Panel.ClientArea
            // 
            this.frmBandejaNotaIngreso_Fill_Panel.ClientArea.Controls.Add(this.groupBox2);
            this.frmBandejaNotaIngreso_Fill_Panel.ClientArea.Controls.Add(this.groupBox1);
            this.frmBandejaNotaIngreso_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.frmBandejaNotaIngreso_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmBandejaNotaIngreso_Fill_Panel.Location = new System.Drawing.Point(8, 32);
            this.frmBandejaNotaIngreso_Fill_Panel.Name = "frmBandejaNotaIngreso_Fill_Panel";
            this.frmBandejaNotaIngreso_Fill_Panel.Size = new System.Drawing.Size(1045, 472);
            this.frmBandejaNotaIngreso_Fill_Panel.TabIndex = 0;
            // 
            // _frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Left
            // 
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 32);
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Left.Name = "_frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Left";
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 472);
            // 
            // _frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Right
            // 
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(1053, 32);
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Right.Name = "_frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Right";
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 472);
            // 
            // _frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Top
            // 
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Top.Name = "_frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Top";
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(1061, 32);
            // 
            // _frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Bottom
            // 
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 504);
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Bottom.Name = "_frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Bottom";
            this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(1061, 8);
            // 
            // frmBandejaNotaIngreso
            // 
            this.AcceptButton = this.btnBuscar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(1061, 512);
            this.Controls.Add(this.frmBandejaNotaIngreso_Fill_Panel);
            this.Controls.Add(this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Bottom);
            this.Name = "frmBandejaNotaIngreso";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bandeja Notas de Ingresos";
            this.Load += new System.EventHandler(this.frmBandejaNotaIngreso_Load);
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboMoneda)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCliente)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRazonSocial)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboMotivo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboAlmacen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox2)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkFiltroPersonalizado)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkBandejaAgrupable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            this.frmBandejaNotaIngreso_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmBandejaNotaIngreso_Fill_Panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox groupBox1;
        private UltraComboEditor cboMotivo;
        private Infragistics.Win.Misc.UltraLabel label13;
        private System.Windows.Forms.DateTimePicker dtpFechaInicio;
        private UltraComboEditor cboAlmacen;
        private Infragistics.Win.Misc.UltraLabel label3;
        private Infragistics.Win.Misc.UltraLabel label2;
        private Infragistics.Win.Misc.UltraLabel label1;
        private Infragistics.Win.Misc.UltraButton btnBuscar;
        private System.Windows.Forms.DateTimePicker dtpFechaFin;
        private Infragistics.Win.Misc.UltraLabel label4;
        private Infragistics.Win.Misc.UltraGroupBox groupBox2;
        private Infragistics.Win.Misc.UltraButton btnEditar;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdData;
        private Infragistics.Win.Misc.UltraButton btnEliminar;
        private Infragistics.Win.Misc.UltraButton btnAgregar;
        private Infragistics.Win.Misc.UltraLabel lblContadorFilas;
        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.Misc.UltraPanel frmBandejaNotaIngreso_Fill_Panel;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmBandejaNotaIngreso_UltraFormManager_Dock_Area_Bottom;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtCliente;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtRazonSocial;
        private UltraComboEditor cboMoneda;
        private Infragistics.Win.Misc.UltraLabel lblDocumentoExportado;
        private Infragistics.Win.Misc.UltraButton btnExportarBandeja;
        private UltraCheckEditor chkFiltroPersonalizado;
        private UltraCheckEditor chkBandejaAgrupable;
    }
}