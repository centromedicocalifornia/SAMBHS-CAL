namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    partial class frmRecetaMedica
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
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn24 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_DiagnosticRepositoryId");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn25 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_DiseasesId");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn26 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_ComponentName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn27 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_DiseasesName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn69 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_UpdateUser");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn28 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_AutoManualName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn29 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_RestrictionsName", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn30 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_RecomendationsName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn31 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_PreQualificationName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn32 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_FinalQualificationName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn33 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_DiagnosticTypeName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn34 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_IsSentToAntecedentName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn35 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("d_ExpirationDateDiagnostic");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn36 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("i_GenerateMedicalBreak");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn37 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("i_RecordStatus");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn38 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("i_RecordType");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn39 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_ComponentId");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("RecipeDetail");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("_AddRecipe", 0);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRecetaMedica));
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand2 = new Infragistics.Win.UltraWinGrid.UltraGridBand("RecipeDetail", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("NombreMedicamento", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("d_Cantidad");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_Posologia");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_Duracion");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("t_FechaFin");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("llevo");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("_EditRecipe", 0);
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("_DeleteRecipe", 1);
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridGroup ultraGridGroup1 = new Infragistics.Win.UltraWinGrid.UltraGridGroup("NewGroup0", 849942719);
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinDataSource.UltraDataBand ultraDataBand1 = new Infragistics.Win.UltraWinDataSource.UltraDataBand("RecipeDetail");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn1 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("NombreMedicamento");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn2 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("d_Cantidad");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn3 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("v_Posologia");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn4 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("v_Duracion");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn5 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("t_FechaFin");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn6 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("v_DiagnosticRepositoryId");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn7 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("v_DiseasesId");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn8 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("v_ComponentName");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn9 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("v_DiseasesName");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn10 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("v_UpdateUser");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn11 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("v_AutoManualName");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn12 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("v_RestrictionsName");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn13 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("v_RecomendationsName");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn14 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("v_PreQualificationName");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn15 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("v_FinalQualificationName");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn16 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("v_DiagnosticTypeName");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn17 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("v_IsSentToAntecedentName");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn18 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("d_ExpirationDateDiagnostic");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn19 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("i_GenerateMedicalBreak");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn20 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("i_RecordStatus");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn21 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("i_RecordType");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn22 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("v_ComponentId");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn23 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("llevo");
            this.frmRecetaMedica_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this.btnDespachado = new Infragistics.Win.Misc.UltraButton();
            this.ultraButton2 = new Infragistics.Win.Misc.UltraButton();
            this.ultraButton1 = new Infragistics.Win.Misc.UltraButton();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.lblRecetamEd = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.cboMedicoTratante = new System.Windows.Forms.ComboBox();
            this.grdTotalDiagnosticos = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraDataSource1 = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
            this.frmRecetaMedica_Fill_Panel.ClientArea.SuspendLayout();
            this.frmRecetaMedica_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdTotalDiagnosticos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraDataSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // frmRecetaMedica_Fill_Panel
            // 
            // 
            // frmRecetaMedica_Fill_Panel.ClientArea
            // 
            this.frmRecetaMedica_Fill_Panel.ClientArea.Controls.Add(this.btnDespachado);
            this.frmRecetaMedica_Fill_Panel.ClientArea.Controls.Add(this.ultraButton2);
            this.frmRecetaMedica_Fill_Panel.ClientArea.Controls.Add(this.ultraButton1);
            this.frmRecetaMedica_Fill_Panel.ClientArea.Controls.Add(this.ultraGroupBox1);
            this.frmRecetaMedica_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.frmRecetaMedica_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmRecetaMedica_Fill_Panel.Location = new System.Drawing.Point(0, 0);
            this.frmRecetaMedica_Fill_Panel.Name = "frmRecetaMedica_Fill_Panel";
            this.frmRecetaMedica_Fill_Panel.Size = new System.Drawing.Size(1136, 420);
            this.frmRecetaMedica_Fill_Panel.TabIndex = 1;
            // 
            // btnDespachado
            // 
            this.btnDespachado.Location = new System.Drawing.Point(1033, 382);
            this.btnDespachado.Name = "btnDespachado";
            this.btnDespachado.Size = new System.Drawing.Size(89, 26);
            this.btnDespachado.TabIndex = 55;
            this.btnDespachado.Text = "Despachar";
            this.btnDespachado.Click += new System.EventHandler(this.btnDespachado_Click);
            // 
            // ultraButton2
            // 
            this.ultraButton2.Location = new System.Drawing.Point(152, 382);
            this.ultraButton2.Name = "ultraButton2";
            this.ultraButton2.Size = new System.Drawing.Size(134, 26);
            this.ultraButton2.TabIndex = 54;
            this.ultraButton2.Text = "Confirmar Despacho";
            this.ultraButton2.Visible = false;
            this.ultraButton2.Click += new System.EventHandler(this.ultraButton2_Click);
            // 
            // ultraButton1
            // 
            this.ultraButton1.Location = new System.Drawing.Point(12, 382);
            this.ultraButton1.Name = "ultraButton1";
            this.ultraButton1.Size = new System.Drawing.Size(134, 26);
            this.ultraButton1.TabIndex = 53;
            this.ultraButton1.Text = "Imprimir Receta";
            this.ultraButton1.Click += new System.EventHandler(this.ultraButton1_Click);
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraGroupBox1.Controls.Add(this.lblRecetamEd);
            this.ultraGroupBox1.Controls.Add(this.label19);
            this.ultraGroupBox1.Controls.Add(this.cboMedicoTratante);
            this.ultraGroupBox1.Controls.Add(this.grdTotalDiagnosticos);
            this.ultraGroupBox1.Location = new System.Drawing.Point(12, 12);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(1110, 365);
            this.ultraGroupBox1.TabIndex = 52;
            this.ultraGroupBox1.Text = "Receta por Diagnóstico";
            // 
            // lblRecetamEd
            // 
            this.lblRecetamEd.AutoSize = true;
            this.lblRecetamEd.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRecetamEd.ForeColor = System.Drawing.Color.Blue;
            this.lblRecetamEd.Location = new System.Drawing.Point(7, 23);
            this.lblRecetamEd.Name = "lblRecetamEd";
            this.lblRecetamEd.Size = new System.Drawing.Size(51, 16);
            this.lblRecetamEd.TabIndex = 163;
            this.lblRecetamEd.Text = "label1";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.ForeColor = System.Drawing.Color.Black;
            this.label19.Location = new System.Drawing.Point(581, 21);
            this.label19.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(85, 13);
            this.label19.TabIndex = 161;
            this.label19.Text = "Médico Tratante";
            // 
            // cboMedicoTratante
            // 
            this.cboMedicoTratante.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboMedicoTratante.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboMedicoTratante.DisplayMember = "Value1";
            this.cboMedicoTratante.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboMedicoTratante.FormattingEnabled = true;
            this.cboMedicoTratante.Location = new System.Drawing.Point(692, 18);
            this.cboMedicoTratante.Margin = new System.Windows.Forms.Padding(2);
            this.cboMedicoTratante.Name = "cboMedicoTratante";
            this.cboMedicoTratante.Size = new System.Drawing.Size(407, 21);
            this.cboMedicoTratante.TabIndex = 162;
            this.cboMedicoTratante.ValueMember = "ID";
            // 
            // grdTotalDiagnosticos
            // 
            this.grdTotalDiagnosticos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdTotalDiagnosticos.CausesValidation = false;
            appearance1.BackColor = System.Drawing.Color.White;
            appearance1.BackColor2 = System.Drawing.Color.Silver;
            appearance1.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            this.grdTotalDiagnosticos.DisplayLayout.Appearance = appearance1;
            ultraGridColumn24.Header.VisiblePosition = 1;
            ultraGridColumn24.Hidden = true;
            ultraGridColumn25.Header.VisiblePosition = 2;
            ultraGridColumn25.Hidden = true;
            ultraGridColumn26.Header.Caption = "Consultorio";
            ultraGridColumn26.Header.VisiblePosition = 3;
            ultraGridColumn26.Width = 95;
            ultraGridColumn27.Header.Caption = "Diagnóstico";
            ultraGridColumn27.Header.VisiblePosition = 4;
            ultraGridColumn27.Width = 189;
            ultraGridColumn69.Header.Caption = "Usuario Act.";
            ultraGridColumn69.Header.VisiblePosition = 5;
            ultraGridColumn69.Width = 95;
            ultraGridColumn28.Header.Caption = "Automatico?";
            ultraGridColumn28.Header.VisiblePosition = 6;
            ultraGridColumn28.Width = 95;
            ultraGridColumn29.Header.Caption = "Restricciones";
            ultraGridColumn29.Header.VisiblePosition = 7;
            ultraGridColumn29.Width = 146;
            ultraGridColumn30.Header.Caption = "Recomendaciones";
            ultraGridColumn30.Header.VisiblePosition = 8;
            ultraGridColumn30.Width = 143;
            ultraGridColumn31.Header.Caption = "Pre-Calificación";
            ultraGridColumn31.Header.VisiblePosition = 9;
            ultraGridColumn31.Width = 49;
            ultraGridColumn32.Header.Caption = "Calificación Final";
            ultraGridColumn32.Header.VisiblePosition = 10;
            ultraGridColumn32.Width = 52;
            ultraGridColumn33.Header.Caption = "Tipo DX";
            ultraGridColumn33.Header.VisiblePosition = 11;
            ultraGridColumn33.Width = 58;
            ultraGridColumn34.Header.Caption = "Enviar a Ant";
            ultraGridColumn34.Header.VisiblePosition = 12;
            ultraGridColumn34.Width = 38;
            ultraGridColumn35.Header.Caption = "Fec Vcto";
            ultraGridColumn35.Header.VisiblePosition = 13;
            ultraGridColumn35.Width = 45;
            ultraGridColumn36.Header.VisiblePosition = 14;
            ultraGridColumn36.Hidden = true;
            ultraGridColumn37.Header.VisiblePosition = 15;
            ultraGridColumn37.Hidden = true;
            ultraGridColumn38.Header.VisiblePosition = 16;
            ultraGridColumn38.Hidden = true;
            ultraGridColumn39.Header.VisiblePosition = 17;
            ultraGridColumn39.Hidden = true;
            ultraGridColumn1.Header.VisiblePosition = 18;
            ultraGridColumn7.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
            appearance2.Image = ((object)(resources.GetObject("appearance2.Image")));
            appearance2.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance2.ImageVAlign = Infragistics.Win.VAlign.Middle;
            ultraGridColumn7.CellButtonAppearance = appearance2;
            ultraGridColumn7.Header.VisiblePosition = 0;
            ultraGridColumn7.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
            ultraGridColumn7.Width = 28;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn24,
            ultraGridColumn25,
            ultraGridColumn26,
            ultraGridColumn27,
            ultraGridColumn69,
            ultraGridColumn28,
            ultraGridColumn29,
            ultraGridColumn30,
            ultraGridColumn31,
            ultraGridColumn32,
            ultraGridColumn33,
            ultraGridColumn34,
            ultraGridColumn35,
            ultraGridColumn36,
            ultraGridColumn37,
            ultraGridColumn38,
            ultraGridColumn39,
            ultraGridColumn1,
            ultraGridColumn7});
            ultraGridColumn2.Header.Caption = "Medicamento";
            ultraGridColumn2.Header.VisiblePosition = 2;
            ultraGridColumn2.RowLayoutColumnInfo.ParentGroupIndex = 0;
            ultraGridColumn2.RowLayoutColumnInfo.ParentGroupKey = "NewGroup0";
            ultraGridColumn2.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(234, 0);
            ultraGridColumn2.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn2.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn2.Width = 106;
            appearance3.TextHAlignAsString = "Right";
            ultraGridColumn3.CellAppearance = appearance3;
            ultraGridColumn3.Header.Caption = "Cantidad";
            ultraGridColumn3.Header.VisiblePosition = 3;
            ultraGridColumn3.RowLayoutColumnInfo.ParentGroupIndex = 0;
            ultraGridColumn3.RowLayoutColumnInfo.ParentGroupKey = "NewGroup0";
            ultraGridColumn3.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn3.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn4.Header.Caption = "Posología";
            ultraGridColumn4.Header.VisiblePosition = 4;
            ultraGridColumn4.RowLayoutColumnInfo.ParentGroupIndex = 0;
            ultraGridColumn4.RowLayoutColumnInfo.ParentGroupKey = "NewGroup0";
            ultraGridColumn4.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn4.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn5.Header.Caption = "Duración";
            ultraGridColumn5.Header.VisiblePosition = 5;
            ultraGridColumn5.RowLayoutColumnInfo.ParentGroupIndex = 0;
            ultraGridColumn5.RowLayoutColumnInfo.ParentGroupKey = "NewGroup0";
            ultraGridColumn5.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn5.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn6.Header.Caption = "Fecha Fin.";
            ultraGridColumn6.Header.VisiblePosition = 7;
            ultraGridColumn6.RowLayoutColumnInfo.ParentGroupIndex = 0;
            ultraGridColumn6.RowLayoutColumnInfo.ParentGroupKey = "NewGroup0";
            ultraGridColumn6.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(188, 0);
            ultraGridColumn6.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn6.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn11.Header.VisiblePosition = 6;
            ultraGridColumn11.RowLayoutColumnInfo.ParentGroupIndex = 0;
            ultraGridColumn11.RowLayoutColumnInfo.ParentGroupKey = "NewGroup0";
            ultraGridColumn11.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn11.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn8.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
            appearance4.Image = ((object)(resources.GetObject("appearance4.Image")));
            appearance4.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance4.ImageVAlign = Infragistics.Win.VAlign.Middle;
            ultraGridColumn8.CellButtonAppearance = appearance4;
            ultraGridColumn8.Header.Caption = "";
            ultraGridColumn8.Header.VisiblePosition = 0;
            ultraGridColumn8.RowLayoutColumnInfo.ParentGroupIndex = 0;
            ultraGridColumn8.RowLayoutColumnInfo.ParentGroupKey = "NewGroup0";
            ultraGridColumn8.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn8.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn8.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
            ultraGridColumn8.Width = 29;
            ultraGridColumn9.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
            appearance5.Image = ((object)(resources.GetObject("appearance5.Image")));
            appearance5.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance5.ImageVAlign = Infragistics.Win.VAlign.Middle;
            ultraGridColumn9.CellButtonAppearance = appearance5;
            ultraGridColumn9.Header.Caption = "";
            ultraGridColumn9.Header.VisiblePosition = 1;
            ultraGridColumn9.RowLayoutColumnInfo.ParentGroupIndex = 0;
            ultraGridColumn9.RowLayoutColumnInfo.ParentGroupKey = "NewGroup0";
            ultraGridColumn9.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn9.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn9.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
            ultraGridColumn9.Width = 27;
            ultraGridBand2.Columns.AddRange(new object[] {
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn11,
            ultraGridColumn8,
            ultraGridColumn9});
            ultraGridGroup1.Header.Caption = "Receta";
            ultraGridGroup1.Key = "NewGroup0";
            ultraGridGroup1.RowLayoutGroupInfo.LabelSpan = 1;
            ultraGridGroup1.RowLayoutGroupInfo.OriginX = 0;
            ultraGridGroup1.RowLayoutGroupInfo.OriginY = 0;
            ultraGridGroup1.RowLayoutGroupInfo.SpanX = 16;
            ultraGridGroup1.RowLayoutGroupInfo.SpanY = 3;
            ultraGridBand2.Groups.AddRange(new Infragistics.Win.UltraWinGrid.UltraGridGroup[] {
            ultraGridGroup1});
            ultraGridBand2.RowLayoutStyle = Infragistics.Win.UltraWinGrid.RowLayoutStyle.GroupLayout;
            this.grdTotalDiagnosticos.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdTotalDiagnosticos.DisplayLayout.BandsSerializer.Add(ultraGridBand2);
            this.grdTotalDiagnosticos.DisplayLayout.InterBandSpacing = 10;
            this.grdTotalDiagnosticos.DisplayLayout.MaxColScrollRegions = 1;
            this.grdTotalDiagnosticos.DisplayLayout.MaxRowScrollRegions = 1;
            this.grdTotalDiagnosticos.DisplayLayout.NewColumnLoadStyle = Infragistics.Win.UltraWinGrid.NewColumnLoadStyle.Hide;
            this.grdTotalDiagnosticos.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdTotalDiagnosticos.DisplayLayout.Override.AllowColSizing = Infragistics.Win.UltraWinGrid.AllowColSizing.Free;
            this.grdTotalDiagnosticos.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdTotalDiagnosticos.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.grdTotalDiagnosticos.DisplayLayout.Override.AllowRowSummaries = Infragistics.Win.UltraWinGrid.AllowRowSummaries.False;
            this.grdTotalDiagnosticos.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.grdTotalDiagnosticos.DisplayLayout.Override.BorderStyleHeader = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance6.BackColor = System.Drawing.Color.Transparent;
            this.grdTotalDiagnosticos.DisplayLayout.Override.CardAreaAppearance = appearance6;
            appearance7.BackColor = System.Drawing.Color.White;
            appearance7.BackColor2 = System.Drawing.Color.White;
            appearance7.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            this.grdTotalDiagnosticos.DisplayLayout.Override.CellAppearance = appearance7;
            this.grdTotalDiagnosticos.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.grdTotalDiagnosticos.DisplayLayout.Override.ColumnSizingArea = Infragistics.Win.UltraWinGrid.ColumnSizingArea.HeadersOnly;
            this.grdTotalDiagnosticos.DisplayLayout.Override.ExpansionIndicator = Infragistics.Win.UltraWinGrid.ShowExpansionIndicator.CheckOnDisplay;
            appearance8.BackColor = System.Drawing.Color.White;
            appearance8.BackColor2 = System.Drawing.Color.LightGray;
            appearance8.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            appearance8.BorderColor = System.Drawing.Color.DarkGray;
            appearance8.ThemedElementAlpha = Infragistics.Win.Alpha.Transparent;
            this.grdTotalDiagnosticos.DisplayLayout.Override.HeaderAppearance = appearance8;
            this.grdTotalDiagnosticos.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance9.AlphaLevel = ((short)(187));
            appearance9.BackColor = System.Drawing.Color.Gainsboro;
            appearance9.BackColor2 = System.Drawing.Color.Gainsboro;
            appearance9.ForeColor = System.Drawing.Color.Black;
            appearance9.ForegroundAlpha = Infragistics.Win.Alpha.Opaque;
            this.grdTotalDiagnosticos.DisplayLayout.Override.RowAlternateAppearance = appearance9;
            appearance10.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.grdTotalDiagnosticos.DisplayLayout.Override.RowSelectorAppearance = appearance10;
            this.grdTotalDiagnosticos.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            this.grdTotalDiagnosticos.DisplayLayout.Override.RowSizing = Infragistics.Win.UltraWinGrid.RowSizing.AutoFree;
            appearance11.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            appearance11.BackColor2 = System.Drawing.SystemColors.GradientInactiveCaption;
            appearance11.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            appearance11.BorderColor = System.Drawing.SystemColors.GradientActiveCaption;
            appearance11.BorderColor2 = System.Drawing.SystemColors.GradientActiveCaption;
            appearance11.FontData.BoldAsString = "False";
            appearance11.ForeColor = System.Drawing.Color.Black;
            this.grdTotalDiagnosticos.DisplayLayout.Override.SelectedRowAppearance = appearance11;
            this.grdTotalDiagnosticos.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            this.grdTotalDiagnosticos.DisplayLayout.RowConnectorColor = System.Drawing.SystemColors.ControlDarkDark;
            this.grdTotalDiagnosticos.DisplayLayout.RowConnectorStyle = Infragistics.Win.UltraWinGrid.RowConnectorStyle.Dashed;
            this.grdTotalDiagnosticos.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdTotalDiagnosticos.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdTotalDiagnosticos.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdTotalDiagnosticos.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.grdTotalDiagnosticos.Location = new System.Drawing.Point(10, 49);
            this.grdTotalDiagnosticos.Margin = new System.Windows.Forms.Padding(2);
            this.grdTotalDiagnosticos.Name = "grdTotalDiagnosticos";
            this.grdTotalDiagnosticos.Size = new System.Drawing.Size(1089, 308);
            this.grdTotalDiagnosticos.TabIndex = 52;
            this.grdTotalDiagnosticos.ClickCellButton += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.grdTotalDiagnosticos_ClickCellButton);
            // 
            // ultraDataSource1
            // 
            ultraDataBand1.Columns.AddRange(new object[] {
            ultraDataColumn1,
            ultraDataColumn2,
            ultraDataColumn3,
            ultraDataColumn4,
            ultraDataColumn5});
            this.ultraDataSource1.Band.ChildBands.AddRange(new object[] {
            ultraDataBand1});
            this.ultraDataSource1.Band.Columns.AddRange(new object[] {
            ultraDataColumn6,
            ultraDataColumn7,
            ultraDataColumn8,
            ultraDataColumn9,
            ultraDataColumn10,
            ultraDataColumn11,
            ultraDataColumn12,
            ultraDataColumn13,
            ultraDataColumn14,
            ultraDataColumn15,
            ultraDataColumn16,
            ultraDataColumn17,
            ultraDataColumn18,
            ultraDataColumn19,
            ultraDataColumn20,
            ultraDataColumn21,
            ultraDataColumn22,
            ultraDataColumn23});
            // 
            // frmRecetaMedica
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1136, 420);
            this.Controls.Add(this.frmRecetaMedica_Fill_Panel);
            this.Name = "frmRecetaMedica";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Receta Medica";
            this.Load += new System.EventHandler(this.frmRecetaMedica_Load);
            this.frmRecetaMedica_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmRecetaMedica_Fill_Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdTotalDiagnosticos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraDataSource1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraPanel frmRecetaMedica_Fill_Panel;
        private Infragistics.Win.Misc.UltraButton ultraButton2;
        private Infragistics.Win.Misc.UltraButton ultraButton1;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdTotalDiagnosticos;
        private Infragistics.Win.Misc.UltraButton btnDespachado;
        private Infragistics.Win.UltraWinDataSource.UltraDataSource ultraDataSource1;
        private System.Windows.Forms.ComboBox cboMedicoTratante;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label lblRecetamEd;
    }
}