using Infragistics.Win;
using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinEditors;
namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    partial class frmVendedor
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
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_CodVendedor");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_NombreCompleto");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_NroDocIdentificacion");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("TipoDocumento");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_UsuarioCreacion", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("t_InsertaFecha", 1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_UsuarioModificacion", 2);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("t_ActualizaFecha", 3);
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance28 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance29 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand2 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_CodigoCliente");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_ClienteNombreRazonSocial");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_TipoDocumento");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_NroDocumento");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn18 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_UsuarioCreacion", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn19 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("t_InsertaFecha", 1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn26 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_UsuarioModificacion", 2);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn27 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("t_ActualizaFecha", 3);
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance30 = new Infragistics.Win.Appearance();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.label1 = new Infragistics.Win.Misc.UltraLabel();
            this.txtBuscarCodigo = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.btnBuscar = new Infragistics.Win.Misc.UltraButton();
            this.txtBuscarDoc = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label2 = new Infragistics.Win.Misc.UltraLabel();
            this.label3 = new Infragistics.Win.Misc.UltraLabel();
            this.txtBuscarNombre = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.groupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            this.btnEliminar = new Infragistics.Win.Misc.UltraButton();
            this.btnAgregar = new Infragistics.Win.Misc.UltraButton();
            this.grdData = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.lblContadorFilas = new Infragistics.Win.Misc.UltraLabel();
            this.tcDetalle = new System.Windows.Forms.TabControl();
            this.tpDatos = new System.Windows.Forms.TabPage();
            this.btnGrabar = new Infragistics.Win.Misc.UltraButton();
            this.groupBox4 = new Infragistics.Win.Misc.UltraGroupBox();
            this.cboEstablecimiento = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.txtEmail = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label19 = new Infragistics.Win.Misc.UltraLabel();
            this.txtFax = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label13 = new Infragistics.Win.Misc.UltraLabel();
            this.label18 = new Infragistics.Win.Misc.UltraLabel();
            this.txtTelefono = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.groupBox3 = new Infragistics.Win.Misc.UltraGroupBox();
            this.cboUsuario = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.label10 = new Infragistics.Win.Misc.UltraLabel();
            this.txtContacto = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label8 = new Infragistics.Win.Misc.UltraLabel();
            this.cboTipoDocumento = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.cboTipoPersona = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.txtDireccionSec = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label12 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraGroupBox4 = new Infragistics.Win.Misc.UltraGroupBox();
            this.ddlPais = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.label17 = new Infragistics.Win.Misc.UltraLabel();
            this.ddlDistrito = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ddlProvincia = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ddlDepartamento = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.label16 = new Infragistics.Win.Misc.UltraLabel();
            this.label15 = new Infragistics.Win.Misc.UltraLabel();
            this.label14 = new Infragistics.Win.Misc.UltraLabel();
            this.txtNombres = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.lblNombreRS = new Infragistics.Win.Misc.UltraLabel();
            this.txtNroDocumento = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label6 = new Infragistics.Win.Misc.UltraLabel();
            this.label5 = new Infragistics.Win.Misc.UltraLabel();
            this.label4 = new Infragistics.Win.Misc.UltraLabel();
            this.txtCodigoAnexo = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label7 = new Infragistics.Win.Misc.UltraLabel();
            this.tpAdicionales = new System.Windows.Forms.TabPage();
            this.groupBox6 = new Infragistics.Win.Misc.UltraGroupBox();
            this.txtBuscarCliente = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label9 = new Infragistics.Win.Misc.UltraLabel();
            this.btnAgregarAval = new Infragistics.Win.Misc.UltraButton();
            this.groupBox5 = new Infragistics.Win.Misc.UltraGroupBox();
            this.btnClienteEliminar = new Infragistics.Win.Misc.UltraButton();
            this.grdClientes = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.lblClienteContadorFilas = new Infragistics.Win.Misc.UltraLabel();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.chkPermisoEliminar = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.chkPermisoAnular = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.uvDatos = new Infragistics.Win.Misc.UltraValidator(this.components);
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this._frmVendedor_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmVendedor_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmVendedor_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmVendedor_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this.uckActivo = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuscarCodigo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuscarDoc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuscarNombre)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox2)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).BeginInit();
            this.tcDetalle.SuspendLayout();
            this.tpDatos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox4)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboEstablecimiento)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEmail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTelefono)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox3)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboUsuario)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtContacto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTipoDocumento)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTipoPersona)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDireccionSec)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox4)).BeginInit();
            this.ultraGroupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ddlPais)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ddlDistrito)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ddlProvincia)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ddlDepartamento)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNombres)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNroDocumento)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCodigoAnexo)).BeginInit();
            this.tpAdicionales.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox6)).BeginInit();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuscarCliente)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox5)).BeginInit();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdClientes)).BeginInit();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkPermisoEliminar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPermisoAnular)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uvDatos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uckActivo)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(8, 31);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tcDetalle);
            this.splitContainer1.Size = new System.Drawing.Size(1062, 483);
            this.splitContainer1.SplitterDistance = 506;
            this.splitContainer1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtBuscarCodigo);
            this.groupBox1.Controls.Add(this.btnBuscar);
            this.groupBox1.Controls.Add(this.txtBuscarDoc);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtBuscarNombre);
            this.groupBox1.Location = new System.Drawing.Point(10, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(489, 86);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.Text = "Filtro de Búsqueda";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 14);
            this.label1.TabIndex = 12;
            this.label1.Text = "Código:";
            // 
            // txtBuscarCodigo
            // 
            this.txtBuscarCodigo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBuscarCodigo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBuscarCodigo.Location = new System.Drawing.Point(55, 22);
            this.txtBuscarCodigo.MaxLength = 120;
            this.txtBuscarCodigo.Name = "txtBuscarCodigo";
            this.txtBuscarCodigo.Size = new System.Drawing.Size(185, 21);
            this.txtBuscarCodigo.TabIndex = 13;
            // 
            // btnBuscar
            // 
            this.btnBuscar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance24.Image = global::SAMBHS.Windows.WinClient.UI.Resource.system_search;
            appearance24.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance24.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance24.TextHAlignAsString = "Center";
            appearance24.TextVAlignAsString = "Middle";
            this.btnBuscar.Appearance = appearance24;
            this.btnBuscar.BackColorInternal = System.Drawing.Color.Transparent;
            this.btnBuscar.Location = new System.Drawing.Point(394, 49);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(78, 23);
            this.btnBuscar.TabIndex = 11;
            this.btnBuscar.Text = "&Buscar";
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // txtBuscarDoc
            // 
            this.txtBuscarDoc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBuscarDoc.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBuscarDoc.Location = new System.Drawing.Point(310, 22);
            this.txtBuscarDoc.MaxLength = 20;
            this.txtBuscarDoc.Name = "txtBuscarDoc";
            this.txtBuscarDoc.Size = new System.Drawing.Size(162, 21);
            this.txtBuscarDoc.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 14);
            this.label2.TabIndex = 7;
            this.label2.Text = "Nombre:";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(248, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 14);
            this.label3.TabIndex = 9;
            this.label3.Text = "RUC/DNI:";
            // 
            // txtBuscarNombre
            // 
            this.txtBuscarNombre.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBuscarNombre.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBuscarNombre.Location = new System.Drawing.Point(55, 51);
            this.txtBuscarNombre.MaxLength = 120;
            this.txtBuscarNombre.Name = "txtBuscarNombre";
            this.txtBuscarNombre.Size = new System.Drawing.Size(333, 21);
            this.txtBuscarNombre.TabIndex = 8;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.btnEliminar);
            this.groupBox2.Controls.Add(this.btnAgregar);
            this.groupBox2.Controls.Add(this.grdData);
            this.groupBox2.Controls.Add(this.lblContadorFilas);
            this.groupBox2.Location = new System.Drawing.Point(10, 105);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(489, 372);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.Text = "Resultado de Búsqueda";
            // 
            // btnEliminar
            // 
            this.btnEliminar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance25.Image = global::SAMBHS.Windows.WinClient.UI.Resource.delete;
            appearance25.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance25.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance25.TextHAlignAsString = "Right";
            appearance25.TextVAlignAsString = "Middle";
            this.btnEliminar.Appearance = appearance25;
            this.btnEliminar.Enabled = false;
            this.btnEliminar.Location = new System.Drawing.Point(356, 343);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(127, 23);
            this.btnEliminar.TabIndex = 147;
            this.btnEliminar.Text = "&Eliminar Vendedor";
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // btnAgregar
            // 
            this.btnAgregar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance26.Image = global::SAMBHS.Windows.WinClient.UI.Resource.add;
            appearance26.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance26.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance26.TextHAlignAsString = "Right";
            appearance26.TextVAlignAsString = "Middle";
            this.btnAgregar.Appearance = appearance26;
            this.btnAgregar.Location = new System.Drawing.Point(232, 343);
            this.btnAgregar.Name = "btnAgregar";
            this.btnAgregar.Size = new System.Drawing.Size(118, 23);
            this.btnAgregar.TabIndex = 146;
            this.btnAgregar.Text = "&Nuevo Vendedor";
            this.btnAgregar.Click += new System.EventHandler(this.btnAgregar_Click);
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
            ultraGridColumn3.Header.Caption = "Cod. Vendedor";
            ultraGridColumn3.Header.VisiblePosition = 0;
            ultraGridColumn3.Width = 96;
            ultraGridColumn1.Header.Caption = "Nombre Apellidos";
            ultraGridColumn1.Header.VisiblePosition = 1;
            ultraGridColumn1.Width = 200;
            ultraGridColumn6.Header.Caption = "Nro. Documento";
            ultraGridColumn6.Header.VisiblePosition = 2;
            ultraGridColumn6.Width = 111;
            ultraGridColumn2.Header.Caption = "Tipo Documento";
            ultraGridColumn2.Header.VisiblePosition = 3;
            ultraGridColumn2.Width = 93;
            ultraGridColumn7.Header.Caption = "Usuario Crea.";
            ultraGridColumn7.Header.VisiblePosition = 4;
            ultraGridColumn9.Format = "dd/MM/yyyy hh:mm tt";
            ultraGridColumn9.Header.Caption = "Fecha Crea. ";
            ultraGridColumn9.Header.VisiblePosition = 5;
            ultraGridColumn11.Header.Caption = "Usuario Act.";
            ultraGridColumn11.Header.VisiblePosition = 6;
            ultraGridColumn12.Format = "dd/MM/yyyy hh:mm tt";
            ultraGridColumn12.Header.Caption = "Fecha Act.";
            ultraGridColumn12.Header.VisiblePosition = 7;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn3,
            ultraGridColumn1,
            ultraGridColumn6,
            ultraGridColumn2,
            ultraGridColumn7,
            ultraGridColumn9,
            ultraGridColumn11,
            ultraGridColumn12});
            this.grdData.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdData.DisplayLayout.InterBandSpacing = 10;
            this.grdData.DisplayLayout.MaxColScrollRegions = 1;
            this.grdData.DisplayLayout.MaxRowScrollRegions = 1;
            this.grdData.DisplayLayout.NewColumnLoadStyle = Infragistics.Win.UltraWinGrid.NewColumnLoadStyle.Hide;
            appearance5.BackColor = System.Drawing.SystemColors.Highlight;
            appearance5.BackColor2 = System.Drawing.SystemColors.ActiveCaption;
            appearance5.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            appearance5.FontData.BoldAsString = "True";
            appearance5.ForeColor = System.Drawing.Color.White;
            this.grdData.DisplayLayout.Override.ActiveRowAppearance = appearance5;
            this.grdData.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdData.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdData.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.grdData.DisplayLayout.Override.AllowRowSummaries = Infragistics.Win.UltraWinGrid.AllowRowSummaries.False;
            this.grdData.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            appearance6.BackColor = System.Drawing.Color.Transparent;
            this.grdData.DisplayLayout.Override.CardAreaAppearance = appearance6;
            appearance7.BackColor = System.Drawing.SystemColors.Control;
            appearance7.BackColor2 = System.Drawing.SystemColors.ControlLightLight;
            appearance7.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            this.grdData.DisplayLayout.Override.CellAppearance = appearance7;
            this.grdData.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            appearance8.BackColor = System.Drawing.SystemColors.Control;
            appearance8.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance8.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            appearance8.ThemedElementAlpha = Infragistics.Win.Alpha.Transparent;
            this.grdData.DisplayLayout.Override.HeaderAppearance = appearance8;
            this.grdData.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance9.AlphaLevel = ((short)(187));
            this.grdData.DisplayLayout.Override.RowAlternateAppearance = appearance9;
            appearance10.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.grdData.DisplayLayout.Override.RowSelectorAppearance = appearance10;
            this.grdData.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            this.grdData.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.grdData.DisplayLayout.RowConnectorColor = System.Drawing.SystemColors.ControlDarkDark;
            this.grdData.DisplayLayout.RowConnectorStyle = Infragistics.Win.UltraWinGrid.RowConnectorStyle.Dashed;
            this.grdData.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdData.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdData.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.grdData.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdData.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.grdData.Location = new System.Drawing.Point(5, 37);
            this.grdData.Margin = new System.Windows.Forms.Padding(2);
            this.grdData.Name = "grdData";
            this.grdData.Size = new System.Drawing.Size(478, 301);
            this.grdData.TabIndex = 142;
            this.grdData.AfterRowActivate += new System.EventHandler(this.grdData_AfterRowActivate);
            this.grdData.MouseDown += new System.Windows.Forms.MouseEventHandler(this.grdData_MouseDown);
            // 
            // lblContadorFilas
            // 
            this.lblContadorFilas.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance27.TextHAlignAsString = "Right";
            appearance27.TextVAlignAsString = "Middle";
            this.lblContadorFilas.Appearance = appearance27;
            this.lblContadorFilas.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.2F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContadorFilas.Location = new System.Drawing.Point(258, 16);
            this.lblContadorFilas.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblContadorFilas.Name = "lblContadorFilas";
            this.lblContadorFilas.Size = new System.Drawing.Size(226, 19);
            this.lblContadorFilas.TabIndex = 143;
            this.lblContadorFilas.Text = "No se ha realizado la búsqueda aún.";
            // 
            // tcDetalle
            // 
            this.tcDetalle.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcDetalle.Controls.Add(this.tpDatos);
            this.tcDetalle.Controls.Add(this.tpAdicionales);
            this.tcDetalle.Controls.Add(this.tabPage1);
            this.tcDetalle.Location = new System.Drawing.Point(3, 3);
            this.tcDetalle.Name = "tcDetalle";
            this.tcDetalle.SelectedIndex = 0;
            this.tcDetalle.Size = new System.Drawing.Size(542, 474);
            this.tcDetalle.TabIndex = 1;
            // 
            // tpDatos
            // 
            this.tpDatos.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.tpDatos.Controls.Add(this.btnGrabar);
            this.tpDatos.Controls.Add(this.groupBox4);
            this.tpDatos.Controls.Add(this.groupBox3);
            this.tpDatos.Location = new System.Drawing.Point(4, 22);
            this.tpDatos.Name = "tpDatos";
            this.tpDatos.Padding = new System.Windows.Forms.Padding(3);
            this.tpDatos.Size = new System.Drawing.Size(534, 448);
            this.tpDatos.TabIndex = 0;
            this.tpDatos.Text = "Datos";
            // 
            // btnGrabar
            // 
            this.btnGrabar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance23.Image = global::SAMBHS.Windows.WinClient.UI.Resource.system_save;
            appearance23.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance23.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance23.TextHAlignAsString = "Right";
            appearance23.TextVAlignAsString = "Middle";
            this.btnGrabar.Appearance = appearance23;
            this.btnGrabar.Location = new System.Drawing.Point(405, 419);
            this.btnGrabar.Name = "btnGrabar";
            this.btnGrabar.Size = new System.Drawing.Size(120, 23);
            this.btnGrabar.TabIndex = 7;
            this.btnGrabar.Text = "&Grabar Vendedor";
            this.btnGrabar.Click += new System.EventHandler(this.btnGrabar_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.cboEstablecimiento);
            this.groupBox4.Controls.Add(this.ultraLabel1);
            this.groupBox4.Controls.Add(this.txtEmail);
            this.groupBox4.Controls.Add(this.label19);
            this.groupBox4.Controls.Add(this.txtFax);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Controls.Add(this.label18);
            this.groupBox4.Controls.Add(this.txtTelefono);
            this.groupBox4.Location = new System.Drawing.Point(6, 300);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(523, 113);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.Text = "Opcionales";
            // 
            // cboEstablecimiento
            // 
            this.cboEstablecimiento.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboEstablecimiento.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboEstablecimiento.Location = new System.Drawing.Point(344, 51);
            this.cboEstablecimiento.Name = "cboEstablecimiento";
            this.cboEstablecimiento.Size = new System.Drawing.Size(127, 21);
            this.cboEstablecimiento.TabIndex = 160;
            this.uvDatos.GetValidationSettings(this.cboEstablecimiento).Condition = new Infragistics.Win.OperatorCondition(Infragistics.Win.ConditionOperator.NotEquals, "--Seleccionar--", true, typeof(string));
            this.uvDatos.GetValidationSettings(this.cboEstablecimiento).DataType = typeof(string);
            this.uvDatos.GetValidationSettings(this.cboEstablecimiento).EmptyValueCriteria = Infragistics.Win.Misc.EmptyValueCriteria.NullOrEmptyString;
            this.uvDatos.GetValidationSettings(this.cboEstablecimiento).IsRequired = true;
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(248, 55);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(90, 14);
            this.ultraLabel1.TabIndex = 159;
            this.ultraLabel1.Text = "Establecimiento :";
            // 
            // txtEmail
            // 
            this.txtEmail.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtEmail.Location = new System.Drawing.Point(310, 25);
            this.txtEmail.MaxLength = 50;
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(161, 21);
            this.txtEmail.TabIndex = 158;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(252, 28);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(40, 14);
            this.label19.TabIndex = 157;
            this.label19.Text = "E-Mail:";
            // 
            // txtFax
            // 
            this.txtFax.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtFax.Location = new System.Drawing.Point(64, 51);
            this.txtFax.MaxLength = 30;
            this.txtFax.Name = "txtFax";
            this.txtFax.Size = new System.Drawing.Size(178, 21);
            this.txtFax.TabIndex = 154;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 55);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(26, 14);
            this.label13.TabIndex = 153;
            this.label13.Text = "Fax:";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(6, 28);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(51, 14);
            this.label18.TabIndex = 151;
            this.label18.Text = "Teléfono:";
            // 
            // txtTelefono
            // 
            this.txtTelefono.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtTelefono.Location = new System.Drawing.Point(64, 25);
            this.txtTelefono.MaxLength = 30;
            this.txtTelefono.Name = "txtTelefono";
            this.txtTelefono.Size = new System.Drawing.Size(178, 21);
            this.txtTelefono.TabIndex = 152;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.uckActivo);
            this.groupBox3.Controls.Add(this.cboUsuario);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.txtContacto);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.cboTipoDocumento);
            this.groupBox3.Controls.Add(this.cboTipoPersona);
            this.groupBox3.Controls.Add(this.txtDireccionSec);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.ultraGroupBox4);
            this.groupBox3.Controls.Add(this.txtNombres);
            this.groupBox3.Controls.Add(this.lblNombreRS);
            this.groupBox3.Controls.Add(this.txtNroDocumento);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.txtCodigoAnexo);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Location = new System.Drawing.Point(6, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(523, 288);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.Text = "Principal";
            // 
            // cboUsuario
            // 
            this.cboUsuario.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboUsuario.Location = new System.Drawing.Point(104, 256);
            this.cboUsuario.Name = "cboUsuario";
            this.cboUsuario.Size = new System.Drawing.Size(185, 21);
            this.cboUsuario.TabIndex = 150;
            this.uvDatos.GetValidationSettings(this.cboUsuario).Condition = new Infragistics.Win.OperatorCondition(Infragistics.Win.ConditionOperator.NotEquals, "--Seleccionar--", true, typeof(string));
            this.uvDatos.GetValidationSettings(this.cboUsuario).DataType = typeof(string);
            this.uvDatos.GetValidationSettings(this.cboUsuario).EmptyValueCriteria = Infragistics.Win.Misc.EmptyValueCriteria.NullOrEmptyString;
            this.uvDatos.GetValidationSettings(this.cboUsuario).IsRequired = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 261);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(46, 14);
            this.label10.TabIndex = 149;
            this.label10.Text = "Usuario:";
            // 
            // txtContacto
            // 
            this.txtContacto.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtContacto.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtContacto.Location = new System.Drawing.Point(104, 191);
            this.txtContacto.MaxLength = 200;
            this.txtContacto.Name = "txtContacto";
            this.txtContacto.Size = new System.Drawing.Size(185, 21);
            this.txtContacto.TabIndex = 148;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 194);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 14);
            this.label8.TabIndex = 147;
            this.label8.Text = "Contacto:";
            // 
            // cboTipoDocumento
            // 
            this.cboTipoDocumento.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboTipoDocumento.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboTipoDocumento.Location = new System.Drawing.Point(104, 91);
            this.cboTipoDocumento.Name = "cboTipoDocumento";
            this.cboTipoDocumento.Size = new System.Drawing.Size(185, 21);
            this.cboTipoDocumento.TabIndex = 144;
            this.uvDatos.GetValidationSettings(this.cboTipoDocumento).Condition = new Infragistics.Win.OperatorCondition(Infragistics.Win.ConditionOperator.NotEquals, "--Seleccionar--", true, typeof(string));
            this.uvDatos.GetValidationSettings(this.cboTipoDocumento).DataType = typeof(string);
            this.uvDatos.GetValidationSettings(this.cboTipoDocumento).EmptyValueCriteria = Infragistics.Win.Misc.EmptyValueCriteria.NullOrEmptyString;
            this.uvDatos.GetValidationSettings(this.cboTipoDocumento).IsRequired = true;
            this.cboTipoDocumento.ValueChanged += new System.EventHandler(this.cboTipoDocumento_SelectedIndexChanged);
            // 
            // cboTipoPersona
            // 
            this.cboTipoPersona.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboTipoPersona.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboTipoPersona.Location = new System.Drawing.Point(104, 57);
            this.cboTipoPersona.Name = "cboTipoPersona";
            this.cboTipoPersona.Size = new System.Drawing.Size(185, 21);
            this.cboTipoPersona.TabIndex = 143;
            this.uvDatos.GetValidationSettings(this.cboTipoPersona).Condition = new Infragistics.Win.OperatorCondition(Infragistics.Win.ConditionOperator.NotEquals, "--Seleccionar--", true, typeof(string));
            this.uvDatos.GetValidationSettings(this.cboTipoPersona).DataType = typeof(string);
            this.uvDatos.GetValidationSettings(this.cboTipoPersona).EmptyValueCriteria = Infragistics.Win.Misc.EmptyValueCriteria.NullOrEmptyString;
            this.uvDatos.GetValidationSettings(this.cboTipoPersona).IsRequired = true;
            this.cboTipoPersona.ValueChanged += new System.EventHandler(this.cboTipoPersona_SelectedIndexChanged);
            // 
            // txtDireccionSec
            // 
            this.txtDireccionSec.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDireccionSec.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtDireccionSec.Location = new System.Drawing.Point(104, 224);
            this.txtDireccionSec.MaxLength = 200;
            this.txtDireccionSec.Name = "txtDireccionSec";
            this.txtDireccionSec.Size = new System.Drawing.Size(391, 21);
            this.txtDireccionSec.TabIndex = 142;
            this.uvDatos.GetValidationSettings(this.txtDireccionSec).DataType = typeof(string);
            this.uvDatos.GetValidationSettings(this.txtDireccionSec).EmptyValueCriteria = Infragistics.Win.Misc.EmptyValueCriteria.NullOrEmptyString;
            this.uvDatos.GetValidationSettings(this.txtDireccionSec).IsRequired = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 227);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(55, 14);
            this.label12.TabIndex = 141;
            this.label12.Text = "Dirección:";
            // 
            // ultraGroupBox4
            // 
            this.ultraGroupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraGroupBox4.Controls.Add(this.ddlPais);
            this.ultraGroupBox4.Controls.Add(this.label17);
            this.ultraGroupBox4.Controls.Add(this.ddlDistrito);
            this.ultraGroupBox4.Controls.Add(this.ddlProvincia);
            this.ultraGroupBox4.Controls.Add(this.ddlDepartamento);
            this.ultraGroupBox4.Controls.Add(this.label16);
            this.ultraGroupBox4.Controls.Add(this.label15);
            this.ultraGroupBox4.Controls.Add(this.label14);
            this.ultraGroupBox4.Location = new System.Drawing.Point(295, 17);
            this.ultraGroupBox4.Name = "ultraGroupBox4";
            this.ultraGroupBox4.Size = new System.Drawing.Size(222, 201);
            this.ultraGroupBox4.TabIndex = 140;
            this.ultraGroupBox4.Text = "Ubicación";
            this.ultraGroupBox4.UseAppStyling = false;
            // 
            // ddlPais
            // 
            this.ddlPais.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ddlPais.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.ddlPais.Location = new System.Drawing.Point(31, 35);
            this.ddlPais.Name = "ddlPais";
            this.ddlPais.Size = new System.Drawing.Size(169, 21);
            this.ddlPais.TabIndex = 0;
            this.ddlPais.ValueChanged += new System.EventHandler(this.ddlPais_SelectedIndexChanged);
            // 
            // label17
            // 
            this.label17.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label17.AutoSize = true;
            this.label17.BackColorInternal = System.Drawing.Color.Transparent;
            this.label17.Location = new System.Drawing.Point(8, 19);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(29, 14);
            this.label17.TabIndex = 6;
            this.label17.Text = "País:";
            // 
            // ddlDistrito
            // 
            this.ddlDistrito.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ddlDistrito.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.ddlDistrito.Location = new System.Drawing.Point(31, 161);
            this.ddlDistrito.Name = "ddlDistrito";
            this.ddlDistrito.Size = new System.Drawing.Size(169, 21);
            this.ddlDistrito.TabIndex = 3;
            // 
            // ddlProvincia
            // 
            this.ddlProvincia.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ddlProvincia.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.ddlProvincia.Location = new System.Drawing.Point(31, 119);
            this.ddlProvincia.Name = "ddlProvincia";
            this.ddlProvincia.Size = new System.Drawing.Size(169, 21);
            this.ddlProvincia.TabIndex = 2;
            this.ddlProvincia.ValueChanged += new System.EventHandler(this.ddlProvincia_SelectedIndexChanged);
            // 
            // ddlDepartamento
            // 
            this.ddlDepartamento.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ddlDepartamento.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.ddlDepartamento.Location = new System.Drawing.Point(31, 78);
            this.ddlDepartamento.Name = "ddlDepartamento";
            this.ddlDepartamento.Size = new System.Drawing.Size(169, 21);
            this.ddlDepartamento.TabIndex = 1;
            this.ddlDepartamento.ValueChanged += new System.EventHandler(this.ddlDepartamento_SelectedIndexChanged);
            // 
            // label16
            // 
            this.label16.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label16.AutoSize = true;
            this.label16.BackColorInternal = System.Drawing.Color.Transparent;
            this.label16.Location = new System.Drawing.Point(8, 146);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(43, 14);
            this.label16.TabIndex = 2;
            this.label16.Text = "Distrito:";
            // 
            // label15
            // 
            this.label15.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label15.AutoSize = true;
            this.label15.BackColorInternal = System.Drawing.Color.Transparent;
            this.label15.Location = new System.Drawing.Point(8, 103);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(54, 14);
            this.label15.TabIndex = 1;
            this.label15.Text = "Provincia:";
            // 
            // label14
            // 
            this.label14.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label14.AutoSize = true;
            this.label14.BackColorInternal = System.Drawing.Color.Transparent;
            this.label14.Location = new System.Drawing.Point(8, 63);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(79, 14);
            this.label14.TabIndex = 0;
            this.label14.Text = "Departamento:";
            // 
            // txtNombres
            // 
            this.txtNombres.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNombres.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtNombres.Location = new System.Drawing.Point(104, 158);
            this.txtNombres.MaxLength = 120;
            this.txtNombres.Name = "txtNombres";
            this.txtNombres.Size = new System.Drawing.Size(185, 21);
            this.txtNombres.TabIndex = 20;
            this.uvDatos.GetValidationSettings(this.txtNombres).DataType = typeof(string);
            this.uvDatos.GetValidationSettings(this.txtNombres).EmptyValueCriteria = Infragistics.Win.Misc.EmptyValueCriteria.NullOrEmptyString;
            this.uvDatos.GetValidationSettings(this.txtNombres).IsRequired = true;
            // 
            // lblNombreRS
            // 
            this.lblNombreRS.AutoSize = true;
            this.lblNombreRS.Location = new System.Drawing.Point(8, 161);
            this.lblNombreRS.Name = "lblNombreRS";
            this.lblNombreRS.Size = new System.Drawing.Size(97, 14);
            this.lblNombreRS.TabIndex = 19;
            this.lblNombreRS.Text = "Nombre Apellidos:";
            // 
            // txtNroDocumento
            // 
            this.txtNroDocumento.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNroDocumento.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtNroDocumento.Location = new System.Drawing.Point(104, 125);
            this.txtNroDocumento.MaxLength = 50;
            this.txtNroDocumento.Name = "txtNroDocumento";
            this.txtNroDocumento.Size = new System.Drawing.Size(185, 21);
            this.txtNroDocumento.TabIndex = 18;
            this.uvDatos.GetValidationSettings(this.txtNroDocumento).DataType = typeof(string);
            this.uvDatos.GetValidationSettings(this.txtNroDocumento).EmptyValueCriteria = Infragistics.Win.Misc.EmptyValueCriteria.NullOrEmptyString;
            this.uvDatos.GetValidationSettings(this.txtNroDocumento).IsRequired = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 128);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(90, 14);
            this.label6.TabIndex = 17;
            this.label6.Text = "Nro. Documento:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 96);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(90, 14);
            this.label5.TabIndex = 15;
            this.label5.Text = "Tipo Documento:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 60);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 14);
            this.label4.TabIndex = 13;
            this.label4.Text = "Tipo Persona:";
            // 
            // txtCodigoAnexo
            // 
            this.txtCodigoAnexo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCodigoAnexo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCodigoAnexo.Location = new System.Drawing.Point(104, 24);
            this.txtCodigoAnexo.MaxLength = 20;
            this.txtCodigoAnexo.Name = "txtCodigoAnexo";
            this.txtCodigoAnexo.Size = new System.Drawing.Size(185, 21);
            this.txtCodigoAnexo.TabIndex = 12;
            this.uvDatos.GetValidationSettings(this.txtCodigoAnexo).DataType = typeof(string);
            this.uvDatos.GetValidationSettings(this.txtCodigoAnexo).EmptyValueCriteria = Infragistics.Win.Misc.EmptyValueCriteria.NullOrEmptyString;
            this.uvDatos.GetValidationSettings(this.txtCodigoAnexo).IsRequired = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 28);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(78, 14);
            this.label7.TabIndex = 1;
            this.label7.Text = "Código Anexo:";
            // 
            // tpAdicionales
            // 
            this.tpAdicionales.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.tpAdicionales.Controls.Add(this.groupBox6);
            this.tpAdicionales.Controls.Add(this.groupBox5);
            this.tpAdicionales.Location = new System.Drawing.Point(4, 22);
            this.tpAdicionales.Name = "tpAdicionales";
            this.tpAdicionales.Padding = new System.Windows.Forms.Padding(3);
            this.tpAdicionales.Size = new System.Drawing.Size(534, 448);
            this.tpAdicionales.TabIndex = 1;
            this.tpAdicionales.Text = "Cartera de Clientes";
            // 
            // groupBox6
            // 
            this.groupBox6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox6.Controls.Add(this.txtBuscarCliente);
            this.groupBox6.Controls.Add(this.label9);
            this.groupBox6.Controls.Add(this.btnAgregarAval);
            this.groupBox6.Location = new System.Drawing.Point(6, 10);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(526, 61);
            this.groupBox6.TabIndex = 150;
            this.groupBox6.Text = "Buscar Cliente";
            // 
            // txtBuscarCliente
            // 
            this.txtBuscarCliente.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBuscarCliente.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBuscarCliente.Location = new System.Drawing.Point(117, 26);
            this.txtBuscarCliente.MaxLength = 120;
            this.txtBuscarCliente.Name = "txtBuscarCliente";
            this.txtBuscarCliente.Size = new System.Drawing.Size(260, 21);
            this.txtBuscarCliente.TabIndex = 148;
            this.txtBuscarCliente.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBuscarCliente_KeyPress);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(21, 29);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(97, 14);
            this.label9.TabIndex = 147;
            this.label9.Text = "Nombre Apellidos:";
            // 
            // btnAgregarAval
            // 
            this.btnAgregarAval.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance28.Image = global::SAMBHS.Windows.WinClient.UI.Resource.system_search;
            appearance28.TextHAlignAsString = "Right";
            appearance28.TextVAlignAsString = "Middle";
            this.btnAgregarAval.Appearance = appearance28;
            this.btnAgregarAval.Location = new System.Drawing.Point(383, 24);
            this.btnAgregarAval.Name = "btnAgregarAval";
            this.btnAgregarAval.Size = new System.Drawing.Size(121, 22);
            this.btnAgregarAval.TabIndex = 146;
            this.btnAgregarAval.Text = "B&uscar y Agregar";
            this.btnAgregarAval.Click += new System.EventHandler(this.btnAgregarAval_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.btnClienteEliminar);
            this.groupBox5.Controls.Add(this.grdClientes);
            this.groupBox5.Controls.Add(this.lblClienteContadorFilas);
            this.groupBox5.Location = new System.Drawing.Point(6, 82);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(526, 378);
            this.groupBox5.TabIndex = 148;
            this.groupBox5.Text = "Lista de Clientes";
            // 
            // btnClienteEliminar
            // 
            this.btnClienteEliminar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance29.Image = global::SAMBHS.Windows.WinClient.UI.Resource.delete;
            appearance29.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance29.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance29.TextHAlignAsString = "Right";
            appearance29.TextVAlignAsString = "Middle";
            this.btnClienteEliminar.Appearance = appearance29;
            this.btnClienteEliminar.Location = new System.Drawing.Point(410, 347);
            this.btnClienteEliminar.Name = "btnClienteEliminar";
            this.btnClienteEliminar.Size = new System.Drawing.Size(110, 23);
            this.btnClienteEliminar.TabIndex = 145;
            this.btnClienteEliminar.Text = "Eliminar &Cliente";
            this.btnClienteEliminar.Click += new System.EventHandler(this.btnChoferEliminar_Click);
            // 
            // grdClientes
            // 
            this.grdClientes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdClientes.CausesValidation = false;
            appearance15.BackColor = System.Drawing.SystemColors.Window;
            appearance15.BackColor2 = System.Drawing.Color.LightSteelBlue;
            appearance15.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            this.grdClientes.DisplayLayout.Appearance = appearance15;
            ultraGridColumn5.Header.Caption = "Cod. Cliente";
            ultraGridColumn5.Header.VisiblePosition = 0;
            ultraGridColumn4.Header.Caption = "Nombre / Razón Social";
            ultraGridColumn4.Header.VisiblePosition = 1;
            ultraGridColumn4.Width = 203;
            ultraGridColumn8.Header.Caption = "Tipo Documento";
            ultraGridColumn8.Header.VisiblePosition = 2;
            ultraGridColumn14.Header.Caption = "Nro. Documento";
            ultraGridColumn14.Header.VisiblePosition = 3;
            ultraGridColumn18.Header.Caption = "Usuario Crea.";
            ultraGridColumn18.Header.VisiblePosition = 4;
            ultraGridColumn19.Format = "dd/MM/yyyy hh:mm tt";
            ultraGridColumn19.Header.Caption = "Fecha Crea.";
            ultraGridColumn19.Header.VisiblePosition = 5;
            ultraGridColumn26.Header.Caption = "Usuario Act.";
            ultraGridColumn26.Header.VisiblePosition = 6;
            ultraGridColumn27.Format = "dd/MM/yyyy hh:mm tt";
            ultraGridColumn27.Header.Caption = "Fecha Act.";
            ultraGridColumn27.Header.VisiblePosition = 7;
            ultraGridBand2.Columns.AddRange(new object[] {
            ultraGridColumn5,
            ultraGridColumn4,
            ultraGridColumn8,
            ultraGridColumn14,
            ultraGridColumn18,
            ultraGridColumn19,
            ultraGridColumn26,
            ultraGridColumn27});
            this.grdClientes.DisplayLayout.BandsSerializer.Add(ultraGridBand2);
            this.grdClientes.DisplayLayout.InterBandSpacing = 10;
            this.grdClientes.DisplayLayout.MaxColScrollRegions = 1;
            this.grdClientes.DisplayLayout.MaxRowScrollRegions = 1;
            this.grdClientes.DisplayLayout.NewColumnLoadStyle = Infragistics.Win.UltraWinGrid.NewColumnLoadStyle.Hide;
            appearance16.BackColor = System.Drawing.SystemColors.Highlight;
            appearance16.BackColor2 = System.Drawing.SystemColors.ActiveCaption;
            appearance16.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            appearance16.FontData.BoldAsString = "True";
            appearance16.ForeColor = System.Drawing.Color.White;
            this.grdClientes.DisplayLayout.Override.ActiveRowAppearance = appearance16;
            this.grdClientes.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdClientes.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdClientes.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.grdClientes.DisplayLayout.Override.AllowRowSummaries = Infragistics.Win.UltraWinGrid.AllowRowSummaries.False;
            this.grdClientes.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            appearance17.BackColor = System.Drawing.Color.Transparent;
            this.grdClientes.DisplayLayout.Override.CardAreaAppearance = appearance17;
            appearance18.BackColor = System.Drawing.SystemColors.Control;
            appearance18.BackColor2 = System.Drawing.SystemColors.ControlLightLight;
            appearance18.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            this.grdClientes.DisplayLayout.Override.CellAppearance = appearance18;
            this.grdClientes.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            appearance19.BackColor = System.Drawing.SystemColors.Control;
            appearance19.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance19.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            appearance19.ThemedElementAlpha = Infragistics.Win.Alpha.Transparent;
            this.grdClientes.DisplayLayout.Override.HeaderAppearance = appearance19;
            this.grdClientes.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance20.AlphaLevel = ((short)(187));
            appearance20.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            appearance20.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.grdClientes.DisplayLayout.Override.RowAlternateAppearance = appearance20;
            appearance21.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.grdClientes.DisplayLayout.Override.RowSelectorAppearance = appearance21;
            this.grdClientes.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            this.grdClientes.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.grdClientes.DisplayLayout.RowConnectorColor = System.Drawing.SystemColors.ControlDarkDark;
            this.grdClientes.DisplayLayout.RowConnectorStyle = Infragistics.Win.UltraWinGrid.RowConnectorStyle.Dashed;
            this.grdClientes.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdClientes.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdClientes.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.grdClientes.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdClientes.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.grdClientes.Location = new System.Drawing.Point(6, 35);
            this.grdClientes.Margin = new System.Windows.Forms.Padding(2);
            this.grdClientes.Name = "grdClientes";
            this.grdClientes.Size = new System.Drawing.Size(515, 307);
            this.grdClientes.TabIndex = 142;
            // 
            // lblClienteContadorFilas
            // 
            this.lblClienteContadorFilas.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance30.TextHAlignAsString = "Right";
            appearance30.TextVAlignAsString = "Middle";
            this.lblClienteContadorFilas.Appearance = appearance30;
            this.lblClienteContadorFilas.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.2F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblClienteContadorFilas.Location = new System.Drawing.Point(295, 16);
            this.lblClienteContadorFilas.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblClienteContadorFilas.Name = "lblClienteContadorFilas";
            this.lblClienteContadorFilas.Size = new System.Drawing.Size(226, 19);
            this.lblClienteContadorFilas.TabIndex = 143;
            this.lblClienteContadorFilas.Text = "No se ha realizado la búsqueda aún.";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.chkPermisoEliminar);
            this.tabPage1.Controls.Add(this.chkPermisoAnular);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(534, 448);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Permisos";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // chkPermisoEliminar
            // 
            this.chkPermisoEliminar.Checked = true;
            this.chkPermisoEliminar.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPermisoEliminar.Location = new System.Drawing.Point(16, 41);
            this.chkPermisoEliminar.Name = "chkPermisoEliminar";
            this.chkPermisoEliminar.Size = new System.Drawing.Size(148, 20);
            this.chkPermisoEliminar.TabIndex = 1;
            this.chkPermisoEliminar.Text = "Permitir Eliminar Ventas";
            // 
            // chkPermisoAnular
            // 
            this.chkPermisoAnular.Checked = true;
            this.chkPermisoAnular.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPermisoAnular.Location = new System.Drawing.Point(16, 13);
            this.chkPermisoAnular.Name = "chkPermisoAnular";
            this.chkPermisoAnular.Size = new System.Drawing.Size(148, 20);
            this.chkPermisoAnular.TabIndex = 0;
            this.chkPermisoAnular.Text = "Permitir Anular Ventas";
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // _frmVendedor_UltraFormManager_Dock_Area_Left
            // 
            this._frmVendedor_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmVendedor_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmVendedor_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._frmVendedor_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmVendedor_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._frmVendedor_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmVendedor_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 31);
            this._frmVendedor_UltraFormManager_Dock_Area_Left.Name = "_frmVendedor_UltraFormManager_Dock_Area_Left";
            this._frmVendedor_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 483);
            // 
            // _frmVendedor_UltraFormManager_Dock_Area_Right
            // 
            this._frmVendedor_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmVendedor_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmVendedor_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._frmVendedor_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmVendedor_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._frmVendedor_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmVendedor_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(1070, 31);
            this._frmVendedor_UltraFormManager_Dock_Area_Right.Name = "_frmVendedor_UltraFormManager_Dock_Area_Right";
            this._frmVendedor_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 483);
            // 
            // _frmVendedor_UltraFormManager_Dock_Area_Top
            // 
            this._frmVendedor_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmVendedor_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmVendedor_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._frmVendedor_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmVendedor_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._frmVendedor_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmVendedor_UltraFormManager_Dock_Area_Top.Name = "_frmVendedor_UltraFormManager_Dock_Area_Top";
            this._frmVendedor_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(1078, 31);
            // 
            // _frmVendedor_UltraFormManager_Dock_Area_Bottom
            // 
            this._frmVendedor_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmVendedor_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmVendedor_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._frmVendedor_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmVendedor_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._frmVendedor_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmVendedor_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 514);
            this._frmVendedor_UltraFormManager_Dock_Area_Bottom.Name = "_frmVendedor_UltraFormManager_Dock_Area_Bottom";
            this._frmVendedor_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(1078, 8);
            // 
            // uckActivo
            // 
            this.uckActivo.Location = new System.Drawing.Point(303, 257);
            this.uckActivo.Name = "uckActivo";
            this.uckActivo.Size = new System.Drawing.Size(54, 20);
            this.uckActivo.TabIndex = 151;
            this.uckActivo.Text = "Activo";
            // 
            // frmVendedor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(1078, 522);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this._frmVendedor_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._frmVendedor_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._frmVendedor_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._frmVendedor_UltraFormManager_Dock_Area_Bottom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmVendedor";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Administración de Vendedor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmVendedor_FormClosing);
            this.Load += new System.EventHandler(this.frmVendedor_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmVendedor_KeyDown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuscarCodigo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuscarDoc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuscarNombre)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox2)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).EndInit();
            this.tcDetalle.ResumeLayout(false);
            this.tpDatos.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupBox4)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboEstablecimiento)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEmail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTelefono)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox3)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboUsuario)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtContacto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTipoDocumento)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboTipoPersona)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDireccionSec)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox4)).EndInit();
            this.ultraGroupBox4.ResumeLayout(false);
            this.ultraGroupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ddlPais)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ddlDistrito)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ddlProvincia)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ddlDepartamento)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNombres)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNroDocumento)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCodigoAnexo)).EndInit();
            this.tpAdicionales.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.groupBox6)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuscarCliente)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox5)).EndInit();
            this.groupBox5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdClientes)).EndInit();
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkPermisoEliminar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkPermisoAnular)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uvDatos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uckActivo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private Infragistics.Win.Misc.UltraGroupBox groupBox1;
        private Infragistics.Win.Misc.UltraButton btnBuscar;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtBuscarDoc;
        private Infragistics.Win.Misc.UltraLabel label2;
        private Infragistics.Win.Misc.UltraLabel label3;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtBuscarNombre;
        private Infragistics.Win.Misc.UltraGroupBox groupBox2;
        private Infragistics.Win.Misc.UltraButton btnEliminar;
        private Infragistics.Win.Misc.UltraButton btnAgregar;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdData;
        private Infragistics.Win.Misc.UltraLabel lblContadorFilas;
        private Infragistics.Win.Misc.UltraLabel label1;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtBuscarCodigo;
        private System.Windows.Forms.TabControl tcDetalle;
        private System.Windows.Forms.TabPage tpDatos;
        private Infragistics.Win.Misc.UltraGroupBox groupBox4;
        private Infragistics.Win.Misc.UltraGroupBox groupBox3;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtContacto;
        private Infragistics.Win.Misc.UltraLabel label8;
        private UltraComboEditor cboTipoDocumento;
        private UltraComboEditor cboTipoPersona;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtDireccionSec;
        private Infragistics.Win.Misc.UltraLabel label12;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox4;
        private UltraComboEditor ddlPais;
        private Infragistics.Win.Misc.UltraLabel label17;
        private UltraComboEditor ddlDistrito;
        private UltraComboEditor ddlProvincia;
        private UltraComboEditor ddlDepartamento;
        private Infragistics.Win.Misc.UltraLabel label16;
        private Infragistics.Win.Misc.UltraLabel label15;
        private Infragistics.Win.Misc.UltraLabel label14;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtNombres;
        private Infragistics.Win.Misc.UltraLabel lblNombreRS;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtNroDocumento;
        private Infragistics.Win.Misc.UltraLabel label6;
        private Infragistics.Win.Misc.UltraLabel label5;
        private Infragistics.Win.Misc.UltraLabel label4;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtCodigoAnexo;
        private Infragistics.Win.Misc.UltraLabel label7;
        private System.Windows.Forms.TabPage tpAdicionales;
        private Infragistics.Win.Misc.UltraGroupBox groupBox6;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtBuscarCliente;
        private Infragistics.Win.Misc.UltraLabel label9;
        private Infragistics.Win.Misc.UltraButton btnAgregarAval;
        private Infragistics.Win.Misc.UltraGroupBox groupBox5;
        private Infragistics.Win.Misc.UltraButton btnClienteEliminar;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdClientes;
        private Infragistics.Win.Misc.UltraLabel lblClienteContadorFilas;
        private Infragistics.Win.Misc.UltraButton btnGrabar;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtEmail;
        private Infragistics.Win.Misc.UltraLabel label19;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtFax;
        private Infragistics.Win.Misc.UltraLabel label13;
        private Infragistics.Win.Misc.UltraLabel label18;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtTelefono;
        private Infragistics.Win.Misc.UltraValidator uvDatos;
        private UltraComboEditor cboUsuario;
        private Infragistics.Win.Misc.UltraLabel label10;
        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmVendedor_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmVendedor_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmVendedor_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmVendedor_UltraFormManager_Dock_Area_Bottom;
        private System.Windows.Forms.TabPage tabPage1;
        private UltraCheckEditor chkPermisoEliminar;
        private UltraCheckEditor chkPermisoAnular;
        private UltraComboEditor cboEstablecimiento;
        private UltraLabel ultraLabel1;
        private UltraCheckEditor uckActivo;
    }
}