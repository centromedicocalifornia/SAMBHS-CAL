namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    partial class frmMovimientosImportacionExcel
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
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this.frmMovimientosImportacionExcel_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this.ultraGrid1 = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.groupBox2 = new Infragistics.Win.Misc.UltraGroupBox();
            this.cboColumnaPedido = new System.Windows.Forms.ComboBox();
            this.label11 = new Infragistics.Win.Misc.UltraLabel();
            this.cboColumnaPrecio = new System.Windows.Forms.ComboBox();
            this.label10 = new Infragistics.Win.Misc.UltraLabel();
            this.btnGuardar = new Infragistics.Win.Misc.UltraButton();
            this.cboColumnaCantidad = new System.Windows.Forms.ComboBox();
            this.label9 = new Infragistics.Win.Misc.UltraLabel();
            this.cboColumnaNombre = new System.Windows.Forms.ComboBox();
            this.label8 = new Infragistics.Win.Misc.UltraLabel();
            this.cboColumnaCodigo = new System.Windows.Forms.ComboBox();
            this.label7 = new Infragistics.Win.Misc.UltraLabel();
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this.ultraDataSource1 = new Infragistics.Win.UltraWinDataSource.UltraDataSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            this.frmMovimientosImportacionExcel_Fill_Panel.ClientArea.SuspendLayout();
            this.frmMovimientosImportacionExcel_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGrid1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox2)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraDataSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // frmMovimientosImportacionExcel_Fill_Panel
            // 
            // 
            // frmMovimientosImportacionExcel_Fill_Panel.ClientArea
            // 
            this.frmMovimientosImportacionExcel_Fill_Panel.ClientArea.Controls.Add(this.ultraGrid1);
            this.frmMovimientosImportacionExcel_Fill_Panel.ClientArea.Controls.Add(this.groupBox2);
            this.frmMovimientosImportacionExcel_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.frmMovimientosImportacionExcel_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmMovimientosImportacionExcel_Fill_Panel.Location = new System.Drawing.Point(8, 31);
            this.frmMovimientosImportacionExcel_Fill_Panel.Name = "frmMovimientosImportacionExcel_Fill_Panel";
            this.frmMovimientosImportacionExcel_Fill_Panel.Size = new System.Drawing.Size(1012, 496);
            this.frmMovimientosImportacionExcel_Fill_Panel.TabIndex = 0;
            // 
            // ultraGrid1
            // 
            this.ultraGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraGrid1.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            this.ultraGrid1.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.ultraGrid1.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.ultraGrid1.DisplayLayout.Override.AllowRowFiltering = Infragistics.Win.DefaultableBoolean.True;
            this.ultraGrid1.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.ultraGrid1.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            appearance1.FontData.SizeInPoints = 7F;
            this.ultraGrid1.DisplayLayout.Override.RowAppearance = appearance1;
            this.ultraGrid1.DisplayLayout.Override.RowFilterAction = Infragistics.Win.UltraWinGrid.RowFilterAction.AppearancesOnly;
            this.ultraGrid1.Location = new System.Drawing.Point(6, 66);
            this.ultraGrid1.Name = "ultraGrid1";
            this.ultraGrid1.Size = new System.Drawing.Size(1000, 424);
            this.ultraGrid1.TabIndex = 23;
            this.ultraGrid1.Text = "Visualizacion";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.cboColumnaPedido);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.cboColumnaPrecio);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.btnGuardar);
            this.groupBox2.Controls.Add(this.cboColumnaCantidad);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.cboColumnaNombre);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.cboColumnaCodigo);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Location = new System.Drawing.Point(6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1000, 51);
            this.groupBox2.TabIndex = 22;
            this.groupBox2.Text = "Columnas Equivalentes";
            // 
            // cboColumnaPedido
            // 
            this.cboColumnaPedido.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboColumnaPedido.FormattingEnabled = true;
            this.cboColumnaPedido.Items.AddRange(new object[] {
            "--Seleccionar--"});
            this.cboColumnaPedido.Location = new System.Drawing.Point(778, 21);
            this.cboColumnaPedido.Name = "cboColumnaPedido";
            this.cboColumnaPedido.Size = new System.Drawing.Size(93, 21);
            this.cboColumnaPedido.TabIndex = 29;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(732, 24);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(43, 14);
            this.label11.TabIndex = 30;
            this.label11.Text = "Pedido:";
            // 
            // cboColumnaPrecio
            // 
            this.cboColumnaPrecio.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboColumnaPrecio.FormattingEnabled = true;
            this.cboColumnaPrecio.Items.AddRange(new object[] {
            "--Seleccionar--"});
            this.cboColumnaPrecio.Location = new System.Drawing.Point(619, 21);
            this.cboColumnaPrecio.Name = "cboColumnaPrecio";
            this.cboColumnaPrecio.Size = new System.Drawing.Size(94, 21);
            this.cboColumnaPrecio.TabIndex = 27;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(573, 24);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(39, 14);
            this.label10.TabIndex = 28;
            this.label10.Text = "Precio:";
            // 
            // btnGuardar
            // 
            appearance2.Image = global::SAMBHS.Windows.WinClient.UI.Resource.tick;
            this.btnGuardar.Appearance = appearance2;
            this.btnGuardar.Location = new System.Drawing.Point(887, 13);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(100, 34);
            this.btnGuardar.TabIndex = 26;
            this.btnGuardar.Text = "Importar";
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // cboColumnaCantidad
            // 
            this.cboColumnaCantidad.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboColumnaCantidad.FormattingEnabled = true;
            this.cboColumnaCantidad.Items.AddRange(new object[] {
            "--Seleccionar--"});
            this.cboColumnaCantidad.Location = new System.Drawing.Point(457, 21);
            this.cboColumnaCantidad.Name = "cboColumnaCantidad";
            this.cboColumnaCantidad.Size = new System.Drawing.Size(103, 21);
            this.cboColumnaCantidad.TabIndex = 24;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(399, 24);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 14);
            this.label9.TabIndex = 25;
            this.label9.Text = "Cantidad:";
            // 
            // cboColumnaNombre
            // 
            this.cboColumnaNombre.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboColumnaNombre.FormattingEnabled = true;
            this.cboColumnaNombre.Items.AddRange(new object[] {
            "--Seleccionar--"});
            this.cboColumnaNombre.Location = new System.Drawing.Point(273, 21);
            this.cboColumnaNombre.Name = "cboColumnaNombre";
            this.cboColumnaNombre.Size = new System.Drawing.Size(105, 21);
            this.cboColumnaNombre.TabIndex = 22;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(200, 24);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(67, 14);
            this.label8.TabIndex = 23;
            this.label8.Text = "Descripción:";
            // 
            // cboColumnaCodigo
            // 
            this.cboColumnaCodigo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboColumnaCodigo.FormattingEnabled = true;
            this.cboColumnaCodigo.Items.AddRange(new object[] {
            "--Seleccionar--"});
            this.cboColumnaCodigo.Location = new System.Drawing.Point(98, 21);
            this.cboColumnaCodigo.Name = "cboColumnaCodigo";
            this.cboColumnaCodigo.Size = new System.Drawing.Size(87, 21);
            this.cboColumnaCodigo.TabIndex = 20;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(14, 24);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(80, 14);
            this.label7.TabIndex = 21;
            this.label7.Text = "Cod. Producto:";
            // 
            // _frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Left
            // 
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 31);
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Left.Name = "_frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Left";
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 496);
            // 
            // _frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Right
            // 
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(1020, 31);
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Right.Name = "_frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Right";
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 496);
            // 
            // _frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Top
            // 
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Top.Name = "_frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Top";
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(1028, 31);
            // 
            // _frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Bottom
            // 
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 527);
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Bottom.Name = "_frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Bottom";
            this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(1028, 8);
            // 
            // frmMovimientosImportacionExcel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1028, 535);
            this.Controls.Add(this.frmMovimientosImportacionExcel_Fill_Panel);
            this.Controls.Add(this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Bottom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMovimientosImportacionExcel";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Importacion Excel";
            this.Load += new System.EventHandler(this.frmMovimientosImportacionExcel_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            this.frmMovimientosImportacionExcel_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmMovimientosImportacionExcel_Fill_Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGrid1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupBox2)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraDataSource1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.Misc.UltraPanel frmMovimientosImportacionExcel_Fill_Panel;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmMovimientosImportacionExcel_UltraFormManager_Dock_Area_Bottom;
        private Infragistics.Win.Misc.UltraGroupBox groupBox2;
        private System.Windows.Forms.ComboBox cboColumnaPedido;
        private Infragistics.Win.Misc.UltraLabel label11;
        private System.Windows.Forms.ComboBox cboColumnaPrecio;
        private Infragistics.Win.Misc.UltraLabel label10;
        private Infragistics.Win.Misc.UltraButton btnGuardar;
        private System.Windows.Forms.ComboBox cboColumnaCantidad;
        private Infragistics.Win.Misc.UltraLabel label9;
        private System.Windows.Forms.ComboBox cboColumnaNombre;
        private Infragistics.Win.Misc.UltraLabel label8;
        private System.Windows.Forms.ComboBox cboColumnaCodigo;
        private Infragistics.Win.Misc.UltraLabel label7;
        private Infragistics.Win.UltraWinGrid.UltraGrid ultraGrid1;
        private Infragistics.Win.UltraWinDataSource.UltraDataSource ultraDataSource1;
    }
}