namespace SAMBHS.Windows.WinServer.UI.Administracion
{
    partial class frmBandejaRoles
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
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("d_InsertDate");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("d_UpdateDate");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("btnPermisos", 0);
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmBandejaRoles));
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_InsertUser", 1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("v_UpdateUser", 2);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("_Eliminar", 3);
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.ultraButton1 = new Infragistics.Win.Misc.UltraButton();
            this.txtNombre = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraGroupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            this.grdData = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this.frmBandejaRoles_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this.ultraGroupBox3 = new Infragistics.Win.Misc.UltraGroupBox();
            this.ultraButton2 = new Infragistics.Win.Misc.UltraButton();
            this.ultraTextEditor1 = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtNombre)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).BeginInit();
            this.ultraGroupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            this.frmBandejaRoles_Fill_Panel.ClientArea.SuspendLayout();
            this.frmBandejaRoles_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox3)).BeginInit();
            this.ultraGroupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTextEditor1)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Controls.Add(this.ultraButton1);
            this.ultraGroupBox1.Controls.Add(this.txtNombre);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel1);
            this.ultraGroupBox1.Location = new System.Drawing.Point(12, 12);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(431, 65);
            this.ultraGroupBox1.TabIndex = 0;
            this.ultraGroupBox1.Text = "Filtro de Búsqueda";
            // 
            // ultraButton1
            // 
            appearance1.Image = global::SAMBHS.Windows.WinServer.UI.Properties.Resources.system_search;
            this.ultraButton1.Appearance = appearance1;
            this.ultraButton1.Location = new System.Drawing.Point(329, 21);
            this.ultraButton1.Name = "ultraButton1";
            this.ultraButton1.Size = new System.Drawing.Size(75, 30);
            this.ultraButton1.TabIndex = 2;
            this.ultraButton1.Text = "Buscar";
            this.ultraButton1.Click += new System.EventHandler(this.ultraButton1_Click);
            // 
            // txtNombre
            // 
            this.txtNombre.Location = new System.Drawing.Point(87, 27);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(236, 21);
            this.txtNombre.TabIndex = 1;
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(18, 30);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(63, 14);
            this.ultraLabel1.TabIndex = 0;
            this.ultraLabel1.Text = "Buscar Rol:";
            // 
            // ultraGroupBox2
            // 
            this.ultraGroupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraGroupBox2.Controls.Add(this.grdData);
            this.ultraGroupBox2.Location = new System.Drawing.Point(12, 83);
            this.ultraGroupBox2.Name = "ultraGroupBox2";
            this.ultraGroupBox2.Size = new System.Drawing.Size(936, 337);
            this.ultraGroupBox2.TabIndex = 1;
            this.ultraGroupBox2.Text = "Resultado Búsqueda";
            this.ultraGroupBox2.ViewStyle = Infragistics.Win.Misc.GroupBoxViewStyle.Office2000;
            // 
            // grdData
            // 
            this.grdData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            appearance2.BackColor = System.Drawing.Color.White;
            this.grdData.DisplayLayout.Appearance = appearance2;
            ultraGridColumn1.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
            ultraGridColumn1.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            ultraGridColumn1.Header.Caption = "Nombre del Rol";
            ultraGridColumn1.Header.VisiblePosition = 2;
            ultraGridColumn1.Width = 318;
            ultraGridColumn2.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
            ultraGridColumn2.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            ultraGridColumn2.Header.Caption = "Fecha Crea.";
            ultraGridColumn2.Header.VisiblePosition = 4;
            ultraGridColumn3.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
            ultraGridColumn3.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            ultraGridColumn3.Header.Caption = "Fecha Act.";
            ultraGridColumn3.Header.VisiblePosition = 6;
            ultraGridColumn4.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
            appearance3.Image = ((object)(resources.GetObject("appearance3.Image")));
            appearance3.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance3.ImageVAlign = Infragistics.Win.VAlign.Middle;
            ultraGridColumn4.CellButtonAppearance = appearance3;
            ultraGridColumn4.Header.Caption = "";
            ultraGridColumn4.Header.VisiblePosition = 1;
            ultraGridColumn4.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
            ultraGridColumn4.Width = 24;
            ultraGridColumn5.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
            ultraGridColumn5.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            ultraGridColumn5.Header.Caption = "Usuario Crea.";
            ultraGridColumn5.Header.VisiblePosition = 3;
            ultraGridColumn6.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
            ultraGridColumn6.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            ultraGridColumn6.Header.Caption = "Usuario Act.";
            ultraGridColumn6.Header.VisiblePosition = 5;
            ultraGridColumn7.ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle.Always;
            appearance4.Image = ((object)(resources.GetObject("appearance4.Image")));
            appearance4.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance4.ImageVAlign = Infragistics.Win.VAlign.Middle;
            ultraGridColumn7.CellButtonAppearance = appearance4;
            ultraGridColumn7.Header.Caption = "";
            ultraGridColumn7.Header.VisiblePosition = 0;
            ultraGridColumn7.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Button;
            ultraGridColumn7.Width = 25;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7});
            this.grdData.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grdData.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            this.grdData.DisplayLayout.MaxColScrollRegions = 1;
            this.grdData.DisplayLayout.MaxRowScrollRegions = 1;
            this.grdData.DisplayLayout.NewColumnLoadStyle = Infragistics.Win.UltraWinGrid.NewColumnLoadStyle.Hide;
            this.grdData.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.None;
            appearance5.BackColor = System.Drawing.Color.Transparent;
            this.grdData.DisplayLayout.Override.CardAreaAppearance = appearance5;
            this.grdData.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.EditAndSelectText;
            this.grdData.DisplayLayout.Override.CellPadding = 3;
            appearance6.TextHAlignAsString = "Left";
            this.grdData.DisplayLayout.Override.HeaderAppearance = appearance6;
            this.grdData.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance7.BorderColor = System.Drawing.Color.LightGray;
            appearance7.TextVAlignAsString = "Middle";
            this.grdData.DisplayLayout.Override.RowAppearance = appearance7;
            this.grdData.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            appearance8.BackColor = System.Drawing.Color.LightSteelBlue;
            appearance8.BorderColor = System.Drawing.Color.Black;
            appearance8.ForeColor = System.Drawing.Color.Black;
            this.grdData.DisplayLayout.Override.SelectedRowAppearance = appearance8;
            this.grdData.DisplayLayout.RowConnectorStyle = Infragistics.Win.UltraWinGrid.RowConnectorStyle.None;
            this.grdData.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.grdData.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.grdData.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.grdData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdData.Location = new System.Drawing.Point(6, 19);
            this.grdData.Name = "grdData";
            this.grdData.Size = new System.Drawing.Size(924, 312);
            this.grdData.TabIndex = 0;
            this.grdData.Text = "Crear Rol:";
            this.grdData.ClickCellButton += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.grdData_ClickCellButton);
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // frmBandejaRoles_Fill_Panel
            // 
            // 
            // frmBandejaRoles_Fill_Panel.ClientArea
            // 
            this.frmBandejaRoles_Fill_Panel.ClientArea.Controls.Add(this.ultraGroupBox3);
            this.frmBandejaRoles_Fill_Panel.ClientArea.Controls.Add(this.ultraGroupBox2);
            this.frmBandejaRoles_Fill_Panel.ClientArea.Controls.Add(this.ultraGroupBox1);
            this.frmBandejaRoles_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.frmBandejaRoles_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmBandejaRoles_Fill_Panel.Location = new System.Drawing.Point(8, 31);
            this.frmBandejaRoles_Fill_Panel.Name = "frmBandejaRoles_Fill_Panel";
            this.frmBandejaRoles_Fill_Panel.Size = new System.Drawing.Size(960, 432);
            this.frmBandejaRoles_Fill_Panel.TabIndex = 0;
            // 
            // ultraGroupBox3
            // 
            this.ultraGroupBox3.Controls.Add(this.ultraButton2);
            this.ultraGroupBox3.Controls.Add(this.ultraTextEditor1);
            this.ultraGroupBox3.Controls.Add(this.ultraLabel2);
            this.ultraGroupBox3.Location = new System.Drawing.Point(449, 12);
            this.ultraGroupBox3.Name = "ultraGroupBox3";
            this.ultraGroupBox3.Size = new System.Drawing.Size(479, 65);
            this.ultraGroupBox3.TabIndex = 11;
            this.ultraGroupBox3.Text = "Crear Rol:";
            // 
            // ultraButton2
            // 
            this.ultraButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance9.Image = global::SAMBHS.Windows.WinServer.UI.Properties.Resources.add;
            this.ultraButton2.Appearance = appearance9;
            this.ultraButton2.Location = new System.Drawing.Point(380, 21);
            this.ultraButton2.Name = "ultraButton2";
            this.ultraButton2.Size = new System.Drawing.Size(75, 30);
            this.ultraButton2.TabIndex = 10;
            this.ultraButton2.Text = "Agregar";
            this.ultraButton2.Click += new System.EventHandler(this.ultraButton2_Click);
            // 
            // ultraTextEditor1
            // 
            this.ultraTextEditor1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraTextEditor1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.ultraTextEditor1.Location = new System.Drawing.Point(114, 27);
            this.ultraTextEditor1.Name = "ultraTextEditor1";
            this.ultraTextEditor1.Size = new System.Drawing.Size(260, 21);
            this.ultraTextEditor1.TabIndex = 9;
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(22, 30);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(86, 14);
            this.ultraLabel2.TabIndex = 8;
            this.ultraLabel2.Text = "Nombre del Rol:";
            // 
            // _frmBandejaRoles_UltraFormManager_Dock_Area_Left
            // 
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 31);
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Left.Name = "_frmBandejaRoles_UltraFormManager_Dock_Area_Left";
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 432);
            // 
            // _frmBandejaRoles_UltraFormManager_Dock_Area_Right
            // 
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(968, 31);
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Right.Name = "_frmBandejaRoles_UltraFormManager_Dock_Area_Right";
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 432);
            // 
            // _frmBandejaRoles_UltraFormManager_Dock_Area_Top
            // 
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Top.Name = "_frmBandejaRoles_UltraFormManager_Dock_Area_Top";
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(976, 31);
            // 
            // _frmBandejaRoles_UltraFormManager_Dock_Area_Bottom
            // 
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 463);
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Bottom.Name = "_frmBandejaRoles_UltraFormManager_Dock_Area_Bottom";
            this._frmBandejaRoles_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(976, 8);
            // 
            // frmBandejaRoles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(976, 471);
            this.Controls.Add(this.frmBandejaRoles_Fill_Panel);
            this.Controls.Add(this._frmBandejaRoles_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._frmBandejaRoles_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._frmBandejaRoles_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._frmBandejaRoles_UltraFormManager_Dock_Area_Bottom);
            this.Name = "frmBandejaRoles";
            this.Text = "Bandeja de Roles";
            this.Load += new System.EventHandler(this.frmBandejaRoles_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtNombre)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox2)).EndInit();
            this.ultraGroupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            this.frmBandejaRoles_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmBandejaRoles_Fill_Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox3)).EndInit();
            this.ultraGroupBox3.ResumeLayout(false);
            this.ultraGroupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTextEditor1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private Infragistics.Win.Misc.UltraButton ultraButton1;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtNombre;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox2;
        private Infragistics.Win.UltraWinGrid.UltraGrid grdData;
        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.Misc.UltraPanel frmBandejaRoles_Fill_Panel;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmBandejaRoles_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmBandejaRoles_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmBandejaRoles_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmBandejaRoles_UltraFormManager_Dock_Area_Bottom;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox3;
        private Infragistics.Win.Misc.UltraButton ultraButton2;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor ultraTextEditor1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
    }
}