using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinEditors;
namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    partial class frmBandejaRegistroVentaElectronica
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
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                if (_tarea != null)
                {
                    _tarea.Dispose();
                    _tarea = null;
                }
                if (_cts != null)
                    _cts.Dispose();
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
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn20 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("NroRegistro");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn21 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Documento");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("TipoDocumento");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn22 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("t_FechaRegistro");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn23 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("t_FechaEmision");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn24 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CodigoCliente");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn25 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("NombreCliente");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn16 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("d_Total");
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn17 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("i_IdEstado");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("t_InsertaFecha", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("t_ActualizaFecha");
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn15 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Saldo");
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("TieneGRM");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn26 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_UsuarioCreacion", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn27 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_UsuarioModificacion", 1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn28 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Moneda", 2);
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("_btnEnviar", 3);
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmBandejaRegistroVentaElectronica));
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("EstadoSunat", 4);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("_btnPdf", 5);
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("_btnXml", 6);
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("_btnCdr", 7);
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("_Procesando", 8);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("_Opciones", 9);
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("_btnPdfBaja", 10);
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("_btnXmlBaja", 11);
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("_btnCdrBaja", 12);
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridGroup ultraGridGroup1 = new Infragistics.Win.UltraWinGrid.UltraGridGroup("NewGroup0", 689049001);
            Infragistics.Win.UltraWinGrid.UltraGridGroup ultraGridGroup2 = new Infragistics.Win.UltraWinGrid.UltraGridGroup("NewGroup1", 689049002);
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            this.groupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            this.pConfig = new Infragistics.Win.Misc.UltraPanel();
            this.btnConfig = new Infragistics.Win.Misc.UltraButton();
            this.pbRecalculandoStock = new System.Windows.Forms.PictureBox();
            this.pBuscando = new System.Windows.Forms.Panel();
            this.lblState = new Infragistics.Win.Misc.UltraLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblDocumentoExportado = new Infragistics.Win.Misc.UltraLabel();
            this.btnExportarBandeja = new Infragistics.Win.Misc.UltraButton();
            this.chkFiltroPersonalizado = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.grdData = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.lblContadorFilas = new Infragistics.Win.Misc.UltraLabel();
            this.groupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.btnEnviarPendientes = new Infragistics.Win.Misc.UltraButton();
            this.btnBuscar = new Infragistics.Win.Misc.UltraButton();
            this.dtpFechaFin = new System.Windows.Forms.DateTimePicker();
            this.label4 = new Infragistics.Win.Misc.UltraLabel();
            this.dtpFechaInicio = new System.Windows.Forms.DateTimePicker();
            this.label3 = new Infragistics.Win.Misc.UltraLabel();
            this.uvDatos = new Infragistics.Win.Misc.UltraValidator(this.components);
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this.frmBandejaRegistroVenta_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this.pPopUp = new Infragistics.Win.Misc.UltraPopupControlContainer(this.components);
            this.contextMenu1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.groupBox2)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.pConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbRecalculandoStock)).BeginInit();
            this.pBuscando.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkFiltroPersonalizado)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uvDatos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            this.frmBandejaRegistroVenta_Fill_Panel.ClientArea.SuspendLayout();
            this.frmBandejaRegistroVenta_Fill_Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.pConfig);
            this.groupBox2.Controls.Add(this.btnConfig);
            this.groupBox2.Controls.Add(this.pbRecalculandoStock);
            this.groupBox2.Controls.Add(this.pBuscando);
            this.groupBox2.Controls.Add(this.lblDocumentoExportado);
            this.groupBox2.Controls.Add(this.btnExportarBandeja);
            this.groupBox2.Controls.Add(this.chkFiltroPersonalizado);
            this.groupBox2.Controls.Add(this.grdData);
            this.groupBox2.Controls.Add(this.lblContadorFilas);
            this.groupBox2.Location = new System.Drawing.Point(12, 72);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1109, 386);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.Text = "Resultado de Búsqueda";
            // 
            // pConfig
            // 
            this.pConfig.Location = new System.Drawing.Point(19, 186);
            this.pConfig.Name = "pConfig";
            this.pConfig.Size = new System.Drawing.Size(158, 87);
            this.pConfig.TabIndex = 174;
            this.pConfig.Visible = false;
            // 
            // btnConfig
            // 
            this.btnConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            appearance1.Image = global::SAMBHS.Windows.WinClient.UI.Properties.Resources.settings;
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance1.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance1.TextHAlignAsString = "Right";
            appearance1.TextVAlignAsString = "Middle";
            this.btnConfig.Appearance = appearance1;
            this.btnConfig.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfig.Location = new System.Drawing.Point(1075, 356);
            this.btnConfig.Margin = new System.Windows.Forms.Padding(2);
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.Size = new System.Drawing.Size(25, 24);
            this.btnConfig.TabIndex = 173;
            this.btnConfig.Click += new System.EventHandler(this.btnConfig_Click);
            // 
            // pbRecalculandoStock
            // 
            this.pbRecalculandoStock.Image = global::SAMBHS.Windows.WinClient.UI.Resource.loadingfinal1;
            this.pbRecalculandoStock.Location = new System.Drawing.Point(7, 15);
            this.pbRecalculandoStock.Name = "pbRecalculandoStock";
            this.pbRecalculandoStock.Size = new System.Drawing.Size(22, 19);
            this.pbRecalculandoStock.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbRecalculandoStock.TabIndex = 172;
            this.pbRecalculandoStock.TabStop = false;
            this.pbRecalculandoStock.Visible = false;
            // 
            // pBuscando
            // 
            this.pBuscando.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pBuscando.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pBuscando.Controls.Add(this.lblState);
            this.pBuscando.Controls.Add(this.pictureBox1);
            this.pBuscando.Location = new System.Drawing.Point(476, 172);
            this.pBuscando.Name = "pBuscando";
            this.pBuscando.Size = new System.Drawing.Size(203, 42);
            this.pBuscando.TabIndex = 171;
            this.pBuscando.Visible = false;
            // 
            // lblState
            // 
            this.lblState.AutoSize = true;
            this.lblState.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.lblState.Location = new System.Drawing.Point(51, 13);
            this.lblState.Name = "lblState";
            this.lblState.Size = new System.Drawing.Size(87, 17);
            this.lblState.TabIndex = 1;
            this.lblState.Text = "Procesando...";
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
            // lblDocumentoExportado
            // 
            this.lblDocumentoExportado.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            appearance2.FontData.BoldAsString = "True";
            this.lblDocumentoExportado.Appearance = appearance2;
            this.lblDocumentoExportado.AutoSize = true;
            this.lblDocumentoExportado.Location = new System.Drawing.Point(232, 364);
            this.lblDocumentoExportado.Name = "lblDocumentoExportado";
            this.lblDocumentoExportado.Size = new System.Drawing.Size(0, 0);
            this.lblDocumentoExportado.TabIndex = 161;
            // 
            // btnExportarBandeja
            // 
            this.btnExportarBandeja.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            appearance3.Image = global::SAMBHS.Windows.WinClient.UI.Resource.page_excel1;
            appearance3.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance3.ImageVAlign = Infragistics.Win.VAlign.Middle;
            appearance3.TextHAlignAsString = "Right";
            appearance3.TextVAlignAsString = "Middle";
            this.btnExportarBandeja.Appearance = appearance3;
            this.btnExportarBandeja.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExportarBandeja.Location = new System.Drawing.Point(7, 359);
            this.btnExportarBandeja.Margin = new System.Windows.Forms.Padding(2);
            this.btnExportarBandeja.Name = "btnExportarBandeja";
            this.btnExportarBandeja.Size = new System.Drawing.Size(22, 24);
            this.btnExportarBandeja.TabIndex = 160;
            this.btnExportarBandeja.Click += new System.EventHandler(this.btnExportarBandeja_Click);
            // 
            // chkFiltroPersonalizado
            // 
            this.chkFiltroPersonalizado.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkFiltroPersonalizado.Location = new System.Drawing.Point(37, 360);
            this.chkFiltroPersonalizado.Name = "chkFiltroPersonalizado";
            this.chkFiltroPersonalizado.Size = new System.Drawing.Size(108, 20);
            this.chkFiltroPersonalizado.TabIndex = 157;
            this.chkFiltroPersonalizado.Text = "Filtro Avanzado";
            this.chkFiltroPersonalizado.CheckedChanged += new System.EventHandler(this.chkFiltroPersonalizado_CheckedChanged);
            // 
            // grdData
            // 
            this.grdData.AccessibleName = " ";
            this.grdData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdData.CausesValidation = false;
            appearance4.BackColor = System.Drawing.SystemColors.Window;
            appearance4.BackColor2 = System.Drawing.Color.LightSteelBlue;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            this.grdData.DisplayLayout.Appearance = appearance4;
            ultraGridBand1.CardSettings.Width = 241;
            ultraGridColumn20.Header.Caption = "Nº Registro";
            ultraGridColumn20.Header.VisiblePosition = 0;
            ultraGridColumn20.RowLayoutColumnInfo.OriginX = 0;
            ultraGridColumn20.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn20.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(0, 29);
            ultraGridColumn20.RowLayoutColumnInfo.PreferredLabelSize = new System.Drawing.Size(0, 40);
            ultraGridColumn20.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn20.RowLayoutColumnInfo.SpanY = 4;
            ultraGridColumn21.Header.VisiblePosition = 2;
            ultraGridColumn21.RowLayoutColumnInfo.OriginX = 4;
            ultraGridColumn21.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn21.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(0, 29);
            ultraGridColumn21.RowLayoutColumnInfo.PreferredLabelSize = new System.Drawing.Size(0, 40);
            ultraGridColumn21.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn21.RowLayoutColumnInfo.SpanY = 4;
            ultraGridColumn21.Width = 159;
            ultraGridColumn12.Header.Caption = "Tipo Doc.";
            ultraGridColumn12.Header.VisiblePosition = 1;
            ultraGridColumn12.RowLayoutColumnInfo.OriginX = 2;
            ultraGridColumn12.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn12.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(0, 29);
            ultraGridColumn12.RowLayoutColumnInfo.PreferredLabelSize = new System.Drawing.Size(0, 40);
            ultraGridColumn12.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn12.RowLayoutColumnInfo.SpanY = 4;
            ultraGridColumn12.Width = 70;
            ultraGridColumn22.Header.Caption = "Fecha Registro";
            ultraGridColumn22.Header.VisiblePosition = 3;
            ultraGridColumn22.RowLayoutColumnInfo.OriginX = 6;
            ultraGridColumn22.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn22.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(0, 29);
            ultraGridColumn22.RowLayoutColumnInfo.PreferredLabelSize = new System.Drawing.Size(0, 40);
            ultraGridColumn22.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn22.RowLayoutColumnInfo.SpanY = 4;
            ultraGridColumn23.Header.Caption = "Fecha Emisión";
            ultraGridColumn23.Header.VisiblePosition = 4;
            ultraGridColumn23.RowLayoutColumnInfo.OriginX = 8;
            ultraGridColumn23.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn23.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(0, 29);
            ultraGridColumn23.RowLayoutColumnInfo.PreferredLabelSize = new System.Drawing.Size(0, 40);
            ultraGridColumn23.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn23.RowLayoutColumnInfo.SpanY = 4;
            ultraGridColumn24.Header.VisiblePosition = 5;
            ultraGridColumn24.RowLayoutColumnInfo.OriginX = 10;
            ultraGridColumn24.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn24.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(0, 29);
            ultraGridColumn24.RowLayoutColumnInfo.PreferredLabelSize = new System.Drawing.Size(0, 40);
            ultraGridColumn24.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn24.RowLayoutColumnInfo.SpanY = 4;
            ultraGridColumn24.Width = 133;
            ultraGridColumn25.Header.VisiblePosition = 6;
            ultraGridColumn25.RowLayoutColumnInfo.OriginX = 12;
            ultraGridColumn25.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn25.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(239, 29);
            ultraGridColumn25.RowLayoutColumnInfo.PreferredLabelSize = new System.Drawing.Size(0, 40);
            ultraGridColumn25.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn25.RowLayoutColumnInfo.SpanY = 4;
            ultraGridColumn25.Width = 252;
            appearance5.TextHAlignAsString = "Right";
            ultraGridColumn16.CellAppearance = appearance5;
            ultraGridColumn16.Format = "0.00";
            ultraGridColumn16.Header.Caption = "Total";
            ultraGridColumn16.Header.VisiblePosition = 8;
            ultraGridColumn16.MaxWidth = 150;
            ultraGridColumn16.RowLayoutColumnInfo.OriginX = 16;
            ultraGridColumn16.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn16.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(0, 29);
            ultraGridColumn16.RowLayoutColumnInfo.PreferredLabelSize = new System.Drawing.Size(0, 40);
            ultraGridColumn16.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn16.RowLayoutColumnInfo.SpanY = 4;
            ultraGridColumn16.Width = 134;
            ultraGridColumn17.Header.Caption = "E";
            ultraGridColumn17.Header.VisiblePosition = 10;
            ultraGridColumn17.RowLayoutColumnInfo.OriginX = 18;
            ultraGridColumn17.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn17.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(0, 29);
            ultraGridColumn17.RowLayoutColumnInfo.PreferredLabelSize = new System.Drawing.Size(0, 40);
            ultraGridColumn17.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn17.RowLayoutColumnInfo.SpanY = 4;
            ultraGridColumn17.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
            ultraGridColumn17.Width = 30;
            appearance6.TextHAlignAsString = "Left";
            ultraGridColumn13.CellAppearance = appearance6;
            ultraGridColumn13.Format = "dd/MM/yyyy hh:mm tt";
            ultraGridColumn13.Header.Caption = "Fecha Crea.";
            ultraGridColumn13.Header.VisiblePosition = 12;
            ultraGridColumn13.Hidden = true;
            ultraGridColumn13.Width = 128;
            appearance7.TextHAlignAsString = "Left";
            ultraGridColumn14.CellAppearance = appearance7;
            ultraGridColumn14.Format = "dd/MM/yyyy hh:mm tt";
            ultraGridColumn14.Header.Caption = "Fecha Act.";
            ultraGridColumn14.Header.VisiblePosition = 14;
            ultraGridColumn14.Hidden = true;
            ultraGridColumn14.Width = 118;
            appearance8.TextHAlignAsString = "Right";
            ultraGridColumn15.CellAppearance = appearance8;
            ultraGridColumn15.Header.VisiblePosition = 9;
            ultraGridColumn15.Hidden = true;
            ultraGridColumn1.Header.Caption = "Con Guía";
            ultraGridColumn1.Header.VisiblePosition = 11;
            ultraGridColumn1.Hidden = true;
            ultraGridColumn26.Header.Caption = "Usuario Crea.";
            ultraGridColumn26.Header.VisiblePosition = 13;
            ultraGridColumn26.Hidden = true;
            ultraGridColumn27.Header.Caption = "Usuario Act.";
            ultraGridColumn27.Header.VisiblePosition = 15;
            ultraGridColumn27.Hidden = true;
            ultraGridColumn27.Width = 88;
            appearance9.TextHAlignAsString = "Center";
            ultraGridColumn28.CellAppearance = appearance9;
            ultraGridColumn28.Header.VisiblePosition = 7;
            ultraGridColumn28.RowLayoutColumnInfo.OriginX = 14;
            ultraGridColumn28.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn28.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(0, 29);
            ultraGridColumn28.RowLayoutColumnInfo.PreferredLabelSize = new System.Drawing.Size(0, 40);
            ultraGridColumn28.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn28.RowLayoutColumnInfo.SpanY = 4;
            ultraGridColumn28.Width = 50;
            ultraGridColumn2.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
            appearance10.Image = ((object)(resources.GetObject("appearance10.Image")));
            appearance10.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance10.ImageVAlign = Infragistics.Win.VAlign.Middle;
            ultraGridColumn2.CellButtonAppearance = appearance10;
            ultraGridColumn2.Header.Caption = "Enviar";
            ultraGridColumn2.Header.VisiblePosition = 16;
            ultraGridColumn2.RowLayoutColumnInfo.OriginX = 20;
            ultraGridColumn2.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn2.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(0, 29);
            ultraGridColumn2.RowLayoutColumnInfo.PreferredLabelSize = new System.Drawing.Size(0, 40);
            ultraGridColumn2.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn2.RowLayoutColumnInfo.SpanY = 4;
            ultraGridColumn2.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
            ultraGridColumn2.Width = 52;
            ultraGridColumn3.Header.VisiblePosition = 17;
            ultraGridColumn3.RowLayoutColumnInfo.OriginX = 22;
            ultraGridColumn3.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn3.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(0, 29);
            ultraGridColumn3.RowLayoutColumnInfo.PreferredLabelSize = new System.Drawing.Size(0, 40);
            ultraGridColumn3.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn3.RowLayoutColumnInfo.SpanY = 4;
            ultraGridColumn3.Width = 156;
            ultraGridColumn4.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
            appearance11.Image = ((object)(resources.GetObject("appearance11.Image")));
            appearance11.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance11.ImageVAlign = Infragistics.Win.VAlign.Middle;
            ultraGridColumn4.CellButtonAppearance = appearance11;
            ultraGridColumn4.Header.Caption = "PDF";
            ultraGridColumn4.Header.VisiblePosition = 18;
            ultraGridColumn4.RowLayoutColumnInfo.OriginX = 0;
            ultraGridColumn4.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn4.RowLayoutColumnInfo.ParentGroupIndex = 0;
            ultraGridColumn4.RowLayoutColumnInfo.ParentGroupKey = "NewGroup0";
            ultraGridColumn4.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(48, 28);
            ultraGridColumn4.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn4.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn4.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
            ultraGridColumn4.Width = 39;
            ultraGridColumn5.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
            appearance12.Image = ((object)(resources.GetObject("appearance12.Image")));
            appearance12.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance12.ImageVAlign = Infragistics.Win.VAlign.Middle;
            ultraGridColumn5.CellButtonAppearance = appearance12;
            ultraGridColumn5.Header.Caption = "XML";
            ultraGridColumn5.Header.VisiblePosition = 19;
            ultraGridColumn5.RowLayoutColumnInfo.OriginX = 2;
            ultraGridColumn5.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn5.RowLayoutColumnInfo.ParentGroupIndex = 0;
            ultraGridColumn5.RowLayoutColumnInfo.ParentGroupKey = "NewGroup0";
            ultraGridColumn5.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(48, 28);
            ultraGridColumn5.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn5.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn5.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
            ultraGridColumn5.Width = 39;
            ultraGridColumn6.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
            appearance13.Image = ((object)(resources.GetObject("appearance13.Image")));
            appearance13.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance13.ImageVAlign = Infragistics.Win.VAlign.Middle;
            ultraGridColumn6.CellButtonAppearance = appearance13;
            ultraGridColumn6.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            ultraGridColumn6.Header.Caption = "CDR";
            ultraGridColumn6.Header.VisiblePosition = 20;
            ultraGridColumn6.RowLayoutColumnInfo.OriginX = 4;
            ultraGridColumn6.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn6.RowLayoutColumnInfo.ParentGroupIndex = 0;
            ultraGridColumn6.RowLayoutColumnInfo.ParentGroupKey = "NewGroup0";
            ultraGridColumn6.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(53, 28);
            ultraGridColumn6.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn6.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn6.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
            ultraGridColumn6.Width = 36;
            ultraGridColumn7.DataType = typeof(bool);
            ultraGridColumn7.Header.VisiblePosition = 21;
            ultraGridColumn7.Hidden = true;
            ultraGridColumn7.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
            ultraGridColumn8.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
            appearance14.Image = ((object)(resources.GetObject("appearance14.Image")));
            appearance14.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance14.ImageVAlign = Infragistics.Win.VAlign.Middle;
            ultraGridColumn8.CellButtonAppearance = appearance14;
            ultraGridColumn8.Header.Caption = "Op.";
            ultraGridColumn8.Header.VisiblePosition = 22;
            ultraGridColumn8.RowLayoutColumnInfo.OriginX = 36;
            ultraGridColumn8.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn8.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(0, 29);
            ultraGridColumn8.RowLayoutColumnInfo.PreferredLabelSize = new System.Drawing.Size(0, 40);
            ultraGridColumn8.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn8.RowLayoutColumnInfo.SpanY = 4;
            ultraGridColumn8.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
            ultraGridColumn8.Width = 38;
            ultraGridColumn9.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
            appearance15.Image = ((object)(resources.GetObject("appearance15.Image")));
            appearance15.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance15.ImageVAlign = Infragistics.Win.VAlign.Middle;
            ultraGridColumn9.CellButtonAppearance = appearance15;
            ultraGridColumn9.Header.Caption = "PDF";
            ultraGridColumn9.Header.VisiblePosition = 23;
            ultraGridColumn9.RowLayoutColumnInfo.OriginX = 0;
            ultraGridColumn9.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn9.RowLayoutColumnInfo.ParentGroupIndex = 1;
            ultraGridColumn9.RowLayoutColumnInfo.ParentGroupKey = "NewGroup1";
            ultraGridColumn9.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(47, 28);
            ultraGridColumn9.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn9.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn9.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
            ultraGridColumn10.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
            appearance16.Image = ((object)(resources.GetObject("appearance16.Image")));
            appearance16.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance16.ImageVAlign = Infragistics.Win.VAlign.Middle;
            ultraGridColumn10.CellButtonAppearance = appearance16;
            ultraGridColumn10.Header.Caption = "XML";
            ultraGridColumn10.Header.VisiblePosition = 24;
            ultraGridColumn10.RowLayoutColumnInfo.OriginX = 2;
            ultraGridColumn10.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn10.RowLayoutColumnInfo.ParentGroupIndex = 1;
            ultraGridColumn10.RowLayoutColumnInfo.ParentGroupKey = "NewGroup1";
            ultraGridColumn10.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(51, 28);
            ultraGridColumn10.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn10.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn10.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
            ultraGridColumn11.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
            appearance17.Image = ((object)(resources.GetObject("appearance17.Image")));
            appearance17.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance17.ImageVAlign = Infragistics.Win.VAlign.Middle;
            ultraGridColumn11.CellButtonAppearance = appearance17;
            ultraGridColumn11.Header.Caption = "CDR";
            ultraGridColumn11.Header.VisiblePosition = 25;
            ultraGridColumn11.RowLayoutColumnInfo.OriginX = 4;
            ultraGridColumn11.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn11.RowLayoutColumnInfo.ParentGroupIndex = 1;
            ultraGridColumn11.RowLayoutColumnInfo.ParentGroupKey = "NewGroup1";
            ultraGridColumn11.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(49, 28);
            ultraGridColumn11.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn11.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn11.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn20,
            ultraGridColumn21,
            ultraGridColumn12,
            ultraGridColumn22,
            ultraGridColumn23,
            ultraGridColumn24,
            ultraGridColumn25,
            ultraGridColumn16,
            ultraGridColumn17,
            ultraGridColumn13,
            ultraGridColumn14,
            ultraGridColumn15,
            ultraGridColumn1,
            ultraGridColumn26,
            ultraGridColumn27,
            ultraGridColumn28,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn8,
            ultraGridColumn9,
            ultraGridColumn10,
            ultraGridColumn11});
            ultraGridGroup1.Header.Caption = "Envío";
            ultraGridGroup1.Key = "NewGroup0";
            ultraGridGroup1.RowLayoutGroupInfo.LabelSpan = 1;
            ultraGridGroup1.RowLayoutGroupInfo.OriginX = 24;
            ultraGridGroup1.RowLayoutGroupInfo.OriginY = 0;
            ultraGridGroup1.RowLayoutGroupInfo.SpanX = 6;
            ultraGridGroup1.RowLayoutGroupInfo.SpanY = 4;
            ultraGridGroup2.Header.Caption = "Baja";
            ultraGridGroup2.Key = "NewGroup1";
            ultraGridGroup2.RowLayoutGroupInfo.LabelSpan = 1;
            ultraGridGroup2.RowLayoutGroupInfo.OriginX = 30;
            ultraGridGroup2.RowLayoutGroupInfo.OriginY = 0;
            ultraGridGroup2.RowLayoutGroupInfo.SpanX = 6;
            ultraGridGroup2.RowLayoutGroupInfo.SpanY = 4;
            ultraGridBand1.Groups.AddRange(new Infragistics.Win.UltraWinGrid.UltraGridGroup[] {
            ultraGridGroup1,
            ultraGridGroup2});
            ultraGridBand1.RowLayoutStyle = Infragistics.Win.UltraWinGrid.RowLayoutStyle.GroupLayout;
            ultraGridBand1.SummaryFooterCaption = "Total:";
            this.grdData.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdData.DisplayLayout.GroupByBox.Hidden = true;
            this.grdData.DisplayLayout.InterBandSpacing = 10;
            this.grdData.DisplayLayout.MaxColScrollRegions = 1;
            this.grdData.DisplayLayout.MaxRowScrollRegions = 1;
            this.grdData.DisplayLayout.NewColumnLoadStyle = Infragistics.Win.UltraWinGrid.NewColumnLoadStyle.Hide;
            appearance18.BackColor = System.Drawing.SystemColors.Highlight;
            appearance18.BackColor2 = System.Drawing.SystemColors.ActiveCaption;
            appearance18.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            appearance18.FontData.BoldAsString = "True";
            appearance18.ForeColor = System.Drawing.Color.White;
            this.grdData.DisplayLayout.Override.ActiveRowAppearance = appearance18;
            this.grdData.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdData.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdData.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.grdData.DisplayLayout.Override.AllowRowSummaries = Infragistics.Win.UltraWinGrid.AllowRowSummaries.False;
            this.grdData.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            appearance19.BackColor = System.Drawing.Color.Transparent;
            this.grdData.DisplayLayout.Override.CardAreaAppearance = appearance19;
            appearance20.BackColor = System.Drawing.SystemColors.Control;
            appearance20.BackColor2 = System.Drawing.SystemColors.ControlLightLight;
            appearance20.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            this.grdData.DisplayLayout.Override.CellAppearance = appearance20;
            this.grdData.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.grdData.DisplayLayout.Override.GroupBySummaryDisplayStyle = Infragistics.Win.UltraWinGrid.GroupBySummaryDisplayStyle.SummaryCells;
            appearance21.BackColor = System.Drawing.SystemColors.Control;
            appearance21.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance21.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            appearance21.ThemedElementAlpha = Infragistics.Win.Alpha.Transparent;
            this.grdData.DisplayLayout.Override.HeaderAppearance = appearance21;
            this.grdData.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance22.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.grdData.DisplayLayout.Override.RowSelectorAppearance = appearance22;
            this.grdData.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            this.grdData.DisplayLayout.Override.RowSizing = Infragistics.Win.UltraWinGrid.RowSizing.Fixed;
            this.grdData.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.grdData.DisplayLayout.Override.SummaryDisplayArea = Infragistics.Win.UltraWinGrid.SummaryDisplayAreas.BottomFixed;
            this.grdData.DisplayLayout.Override.SummaryFooterCaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance23.BackColor = System.Drawing.SystemColors.Window;
            appearance23.FontData.BoldAsString = "True";
            appearance23.ForeColor = System.Drawing.SystemColors.WindowText;
            appearance23.Image = ((object)(resources.GetObject("appearance23.Image")));
            appearance23.ImageHAlign = Infragistics.Win.HAlign.Left;
            this.grdData.DisplayLayout.Override.SummaryValueAppearance = appearance23;
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
            this.grdData.Size = new System.Drawing.Size(1097, 317);
            this.grdData.TabIndex = 148;
            this.grdData.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(this.grdData_InitializeRow);
            this.grdData.AfterRowActivate += new System.EventHandler(this.grdData_AfterRowActivate);
            this.grdData.ClickCellButton += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.grdData_ClickCellButton);
            this.grdData.DoubleClick += new System.EventHandler(this.grdData_DoubleClick);
            this.grdData.KeyDown += new System.Windows.Forms.KeyEventHandler(this.grdData_KeyDown);
            this.grdData.MouseDown += new System.Windows.Forms.MouseEventHandler(this.grdData_MouseDown);
            // 
            // lblContadorFilas
            // 
            this.lblContadorFilas.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblContadorFilas.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.2F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContadorFilas.Location = new System.Drawing.Point(888, 16);
            this.lblContadorFilas.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblContadorFilas.Name = "lblContadorFilas";
            this.lblContadorFilas.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblContadorFilas.Size = new System.Drawing.Size(213, 19);
            this.lblContadorFilas.TabIndex = 143;
            this.lblContadorFilas.Text = "No se ha realizado la búsqueda aún.";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance24.FontData.BoldAsString = "False";
            this.groupBox1.Appearance = appearance24;
            this.groupBox1.Controls.Add(this.btnEnviarPendientes);
            this.groupBox1.Controls.Add(this.btnBuscar);
            this.groupBox1.Controls.Add(this.dtpFechaFin);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.dtpFechaInicio);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1109, 54);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.Text = "Filtro de Búsqueda:";
            // 
            // btnEnviarPendientes
            // 
            appearance25.Image = global::SAMBHS.Windows.WinClient.UI.Resource.application_start;
            appearance25.ImageHAlign = Infragistics.Win.HAlign.Left;
            this.btnEnviarPendientes.Appearance = appearance25;
            this.btnEnviarPendientes.Location = new System.Drawing.Point(374, 17);
            this.btnEnviarPendientes.Name = "btnEnviarPendientes";
            this.btnEnviarPendientes.Size = new System.Drawing.Size(127, 26);
            this.btnEnviarPendientes.TabIndex = 17;
            this.btnEnviarPendientes.Text = "Enviar Pendientes";
            this.btnEnviarPendientes.Click += new System.EventHandler(this.btnEnviarPendientes_Click);
            // 
            // btnBuscar
            // 
            this.btnBuscar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance26.Image = global::SAMBHS.Windows.WinClient.UI.Resource.system_search;
            appearance26.ImageHAlign = Infragistics.Win.HAlign.Left;
            this.btnBuscar.Appearance = appearance26;
            this.btnBuscar.Location = new System.Drawing.Point(991, 17);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(103, 26);
            this.btnBuscar.TabIndex = 11;
            this.btnBuscar.Text = "&Buscar";
            this.btnBuscar.Click += new System.EventHandler(this.ultraButton1_Click);
            // 
            // dtpFechaFin
            // 
            this.dtpFechaFin.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFechaFin.Location = new System.Drawing.Point(253, 21);
            this.dtpFechaFin.Margin = new System.Windows.Forms.Padding(2);
            this.dtpFechaFin.Name = "dtpFechaFin";
            this.dtpFechaFin.Size = new System.Drawing.Size(102, 20);
            this.dtpFechaFin.TabIndex = 9;
            this.dtpFechaFin.ValueChanged += new System.EventHandler(this.dtpFechaFin_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(229, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 14);
            this.label4.TabIndex = 16;
            this.label4.Text = "Al:";
            // 
            // dtpFechaInicio
            // 
            this.dtpFechaInicio.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFechaInicio.Location = new System.Drawing.Point(127, 21);
            this.dtpFechaInicio.Margin = new System.Windows.Forms.Padding(2);
            this.dtpFechaInicio.Name = "dtpFechaInicio";
            this.dtpFechaInicio.Size = new System.Drawing.Size(96, 20);
            this.dtpFechaInicio.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 14);
            this.label3.TabIndex = 2;
            this.label3.Text = "Fecha Registro del: ";
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // frmBandejaRegistroVenta_Fill_Panel
            // 
            // 
            // frmBandejaRegistroVenta_Fill_Panel.ClientArea
            // 
            this.frmBandejaRegistroVenta_Fill_Panel.ClientArea.Controls.Add(this.groupBox2);
            this.frmBandejaRegistroVenta_Fill_Panel.ClientArea.Controls.Add(this.groupBox1);
            this.frmBandejaRegistroVenta_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.frmBandejaRegistroVenta_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmBandejaRegistroVenta_Fill_Panel.Location = new System.Drawing.Point(8, 32);
            this.frmBandejaRegistroVenta_Fill_Panel.Name = "frmBandejaRegistroVenta_Fill_Panel";
            this.frmBandejaRegistroVenta_Fill_Panel.Size = new System.Drawing.Size(1133, 471);
            this.frmBandejaRegistroVenta_Fill_Panel.TabIndex = 0;
            // 
            // _frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Left
            // 
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 32);
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Left.Name = "_frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Left";
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 471);
            // 
            // _frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Right
            // 
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(1141, 32);
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Right.Name = "_frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Right";
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 471);
            // 
            // _frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Top
            // 
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Top.Name = "_frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Top";
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(1149, 32);
            // 
            // _frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Bottom
            // 
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 503);
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Bottom.Name = "_frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Bottom";
            this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(1149, 8);
            // 
            // pPopUp
            // 
            this.pPopUp.DropDownResizeHandleStyle = Infragistics.Win.DropDownResizeHandleStyle.VerticalResize;
            this.pPopUp.PopupControl = this.pConfig;
            // 
            // contextMenu1
            // 
            this.contextMenu1.Name = "contextMenu1";
            this.contextMenu1.Size = new System.Drawing.Size(61, 4);
            this.contextMenu1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // frmBandejaRegistroVentaElectronica
            // 
            this.AcceptButton = this.btnBuscar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(1149, 511);
            this.Controls.Add(this.frmBandejaRegistroVenta_Fill_Panel);
            this.Controls.Add(this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Bottom);
            this.Name = "frmBandejaRegistroVentaElectronica";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bandeja Registro Venta";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmBandejaRegistroVenta_FormClosing);
            this.Load += new System.EventHandler(this.frmBandejaRegistroVenta_Load);
            ((System.ComponentModel.ISupportInitialize)(this.groupBox2)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.pConfig.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbRecalculandoStock)).EndInit();
            this.pBuscando.ResumeLayout(false);
            this.pBuscando.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkFiltroPersonalizado)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uvDatos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            this.frmBandejaRegistroVenta_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmBandejaRegistroVenta_Fill_Panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private UltraGroupBox groupBox2;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdData;
        private UltraLabel lblContadorFilas;
        private UltraGroupBox groupBox1;
        private System.Windows.Forms.DateTimePicker dtpFechaFin;
        private UltraLabel label4;
        private System.Windows.Forms.DateTimePicker dtpFechaInicio;
        private UltraLabel label3;
        private Infragistics.Win.Misc.UltraValidator uvDatos;
        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.Misc.UltraPanel frmBandejaRegistroVenta_Fill_Panel;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmBandejaRegistroVenta_UltraFormManager_Dock_Area_Bottom;
        private UltraButton btnBuscar;
        private UltraCheckEditor chkFiltroPersonalizado;
        private UltraLabel lblDocumentoExportado;
        private UltraButton btnExportarBandeja;
        private System.Windows.Forms.Panel pBuscando;
        private UltraLabel lblState;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pbRecalculandoStock;
        private UltraPanel pConfig;
        private UltraButton btnConfig;
        private UltraPopupControlContainer pPopUp;
        private System.Windows.Forms.ContextMenuStrip contextMenu1;
        private UltraButton btnEnviarPendientes;

    }
}