using Infragistics.Win.Misc;
namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    partial class frmBuscarTransportista
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
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_Codigo");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_NombreRazonSocial");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_NombreContacto", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_Direccion");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_NumeroDocumento");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_Telefono", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_CorreoElectronico", 1);
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            this.groupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            this.grdData = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.lblContadorFilas = new Infragistics.Win.Misc.UltraLabel();
            this.groupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.txtBuscarNombre = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this.frmBuscarTransportista_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox2)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuscarNombre)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            this.frmBuscarTransportista_Fill_Panel.ClientArea.SuspendLayout();
            this.frmBuscarTransportista_Fill_Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.grdData);
            this.groupBox2.Controls.Add(this.lblContadorFilas);
            this.groupBox2.Location = new System.Drawing.Point(4, 81);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(560, 325);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.Text = "Resultado de Búsqueda";
            // 
            // grdData
            // 
            this.grdData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdData.CausesValidation = false;
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BackColor2 = System.Drawing.Color.LightSteelBlue;
            appearance1.BackGradientStyle = Infragistics.Win.GradientStyle.Vertical;
            this.grdData.DisplayLayout.Appearance = appearance1;
            ultraGridColumn1.Header.Caption = "Cod. Transportista";
            ultraGridColumn1.Header.VisiblePosition = 1;
            ultraGridColumn2.Header.Caption = "Razón Social";
            ultraGridColumn2.Header.VisiblePosition = 2;
            ultraGridColumn2.Width = 127;
            ultraGridColumn7.Header.Caption = "Nombre Contacto";
            ultraGridColumn7.Header.VisiblePosition = 4;
            ultraGridColumn7.Width = 119;
            ultraGridColumn3.Header.Caption = "Dirección";
            ultraGridColumn3.Header.VisiblePosition = 3;
            ultraGridColumn3.Width = 124;
            ultraGridColumn4.Header.Caption = "RUC";
            ultraGridColumn4.Header.VisiblePosition = 0;
            ultraGridColumn5.Header.Caption = "Teléfono";
            ultraGridColumn5.Header.VisiblePosition = 5;
            ultraGridColumn8.Header.Caption = "Correo Electronico";
            ultraGridColumn8.Header.VisiblePosition = 6;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn7,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn8});
            this.grdData.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdData.DisplayLayout.InterBandSpacing = 10;
            this.grdData.DisplayLayout.MaxColScrollRegions = 1;
            this.grdData.DisplayLayout.MaxRowScrollRegions = 1;
            this.grdData.DisplayLayout.NewColumnLoadStyle = Infragistics.Win.UltraWinGrid.NewColumnLoadStyle.Hide;
            this.grdData.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.grdData.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.grdData.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.grdData.DisplayLayout.Override.AllowRowSummaries = Infragistics.Win.UltraWinGrid.AllowRowSummaries.False;
            this.grdData.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            appearance2.BackColor = System.Drawing.Color.Transparent;
            this.grdData.DisplayLayout.Override.CardAreaAppearance = appearance2;
            appearance3.BackColor = System.Drawing.SystemColors.Control;
            appearance3.BackColor2 = System.Drawing.SystemColors.ControlLightLight;
            appearance3.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            this.grdData.DisplayLayout.Override.CellAppearance = appearance3;
            this.grdData.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            appearance4.BackColor = System.Drawing.SystemColors.Control;
            appearance4.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            appearance4.ThemedElementAlpha = Infragistics.Win.Alpha.Transparent;
            this.grdData.DisplayLayout.Override.HeaderAppearance = appearance4;
            this.grdData.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance5.AlphaLevel = ((short)(187));
            this.grdData.DisplayLayout.Override.RowAlternateAppearance = appearance5;
            appearance6.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.grdData.DisplayLayout.Override.RowSelectorAppearance = appearance6;
            this.grdData.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            appearance7.BackColor = System.Drawing.SystemColors.Highlight;
            appearance7.BackColor2 = System.Drawing.SystemColors.ActiveCaption;
            appearance7.BackGradientStyle = Infragistics.Win.GradientStyle.VerticalBump;
            appearance7.FontData.BoldAsString = "True";
            this.grdData.DisplayLayout.Override.SelectedRowAppearance = appearance7;
            this.grdData.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            this.grdData.DisplayLayout.RowConnectorColor = System.Drawing.SystemColors.ControlDarkDark;
            this.grdData.DisplayLayout.RowConnectorStyle = Infragistics.Win.UltraWinGrid.RowConnectorStyle.Dashed;
            this.grdData.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdData.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdData.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.grdData.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdData.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.grdData.Location = new System.Drawing.Point(10, 32);
            this.grdData.Margin = new System.Windows.Forms.Padding(2);
            this.grdData.Name = "grdData";
            this.grdData.Size = new System.Drawing.Size(545, 288);
            this.grdData.TabIndex = 1;
            this.grdData.DoubleClickRow += new Infragistics.Win.UltraWinGrid.DoubleClickRowEventHandler(this.grdData_DoubleClickRow);
            this.grdData.KeyDown += new System.Windows.Forms.KeyEventHandler(this.grdData_KeyDown);
            // 
            // lblContadorFilas
            // 
            this.lblContadorFilas.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance9.TextHAlignAsString = "Right";
            appearance9.TextVAlignAsString = "Middle";
            this.lblContadorFilas.Appearance = appearance9;
            this.lblContadorFilas.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.2F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContadorFilas.Location = new System.Drawing.Point(299, 11);
            this.lblContadorFilas.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblContadorFilas.Name = "lblContadorFilas";
            this.lblContadorFilas.Size = new System.Drawing.Size(226, 19);
            this.lblContadorFilas.TabIndex = 143;
            this.lblContadorFilas.Text = "No se ha realizado la búsqueda aún.";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.txtBuscarNombre);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(4, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(560, 63);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.Text = "Filtro de Búsqueda";
            // 
            // txtBuscarNombre
            // 
            this.txtBuscarNombre.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBuscarNombre.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtBuscarNombre.Location = new System.Drawing.Point(59, 27);
            this.txtBuscarNombre.MaxLength = 120;
            this.txtBuscarNombre.Name = "txtBuscarNombre";
            this.txtBuscarNombre.Size = new System.Drawing.Size(495, 21);
            this.txtBuscarNombre.TabIndex = 9;
            this.txtBuscarNombre.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtBuscarNombre_KeyUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 14);
            this.label1.TabIndex = 8;
            this.label1.Text = "Buscar:";
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // frmBuscarTransportista_Fill_Panel
            // 
            // 
            // frmBuscarTransportista_Fill_Panel.ClientArea
            // 
            this.frmBuscarTransportista_Fill_Panel.ClientArea.Controls.Add(this.groupBox2);
            this.frmBuscarTransportista_Fill_Panel.ClientArea.Controls.Add(this.groupBox1);
            this.frmBuscarTransportista_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.frmBuscarTransportista_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmBuscarTransportista_Fill_Panel.Location = new System.Drawing.Point(8, 31);
            this.frmBuscarTransportista_Fill_Panel.Name = "frmBuscarTransportista_Fill_Panel";
            this.frmBuscarTransportista_Fill_Panel.Size = new System.Drawing.Size(570, 418);
            this.frmBuscarTransportista_Fill_Panel.TabIndex = 0;
            // 
            // _frmBuscarTransportista_UltraFormManager_Dock_Area_Left
            // 
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 31);
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Left.Name = "_frmBuscarTransportista_UltraFormManager_Dock_Area_Left";
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 418);
            // 
            // _frmBuscarTransportista_UltraFormManager_Dock_Area_Right
            // 
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(578, 31);
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Right.Name = "_frmBuscarTransportista_UltraFormManager_Dock_Area_Right";
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 418);
            // 
            // _frmBuscarTransportista_UltraFormManager_Dock_Area_Top
            // 
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Top.Name = "_frmBuscarTransportista_UltraFormManager_Dock_Area_Top";
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(586, 31);
            // 
            // _frmBuscarTransportista_UltraFormManager_Dock_Area_Bottom
            // 
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 449);
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Bottom.Name = "_frmBuscarTransportista_UltraFormManager_Dock_Area_Bottom";
            this._frmBuscarTransportista_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(586, 8);
            // 
            // frmBuscarTransportista
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(586, 457);
            this.Controls.Add(this.frmBuscarTransportista_Fill_Panel);
            this.Controls.Add(this._frmBuscarTransportista_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._frmBuscarTransportista_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._frmBuscarTransportista_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._frmBuscarTransportista_UltraFormManager_Dock_Area_Bottom);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmBuscarTransportista";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Buscar Transportista";
            this.Load += new System.EventHandler(this.frmBuscarTransportista_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmBuscarTransportista_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.groupBox2)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtBuscarNombre)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            this.frmBuscarTransportista_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmBuscarTransportista_Fill_Panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox groupBox2;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdData;
        private Infragistics.Win.Misc.UltraLabel lblContadorFilas;
        private Infragistics.Win.Misc.UltraGroupBox groupBox1;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtBuscarNombre;
        private Infragistics.Win.Misc.UltraLabel label1;
        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.Misc.UltraPanel frmBuscarTransportista_Fill_Panel;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmBuscarTransportista_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmBuscarTransportista_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmBuscarTransportista_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmBuscarTransportista_UltraFormManager_Dock_Area_Bottom;
    }
}