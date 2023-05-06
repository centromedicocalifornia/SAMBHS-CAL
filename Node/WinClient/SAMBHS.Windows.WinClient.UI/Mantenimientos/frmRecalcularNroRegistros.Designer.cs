namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    partial class frmRecalcularNroRegistros
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
            this.ultraButton1 = new Infragistics.Win.Misc.UltraButton();
            this.txtPeriodo = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.label1 = new System.Windows.Forms.Label();
            this.cboMeses = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.chkAplicarMes = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this.frmRecalcularNroRegistros_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this.chkSoloVentas = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.btnEmparejarLiquidacionCompra = new System.Windows.Forms.Button();
            this.btnEmparejar = new System.Windows.Forms.Button();
            this.chkSoloCompras = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this.btnActualizarNombre = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.txtPeriodo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboMeses)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAplicarMes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            this.frmRecalcularNroRegistros_Fill_Panel.ClientArea.SuspendLayout();
            this.frmRecalcularNroRegistros_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkSoloVentas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSoloCompras)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraButton1
            // 
            this.ultraButton1.Location = new System.Drawing.Point(216, 54);
            this.ultraButton1.Name = "ultraButton1";
            this.ultraButton1.Size = new System.Drawing.Size(144, 23);
            this.ultraButton1.TabIndex = 0;
            this.ultraButton1.Text = "Recalcular Nro. Registros";
            this.ultraButton1.Click += new System.EventHandler(this.ultraButton1_Click);
            // 
            // txtPeriodo
            // 
            this.txtPeriodo.Enabled = false;
            this.txtPeriodo.Location = new System.Drawing.Point(67, 19);
            this.txtPeriodo.Name = "txtPeriodo";
            this.txtPeriodo.Size = new System.Drawing.Size(72, 21);
            this.txtPeriodo.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Periodo";
            // 
            // cboMeses
            // 
            this.cboMeses.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboMeses.Enabled = false;
            this.cboMeses.Location = new System.Drawing.Point(216, 19);
            this.cboMeses.Name = "cboMeses";
            this.cboMeses.Size = new System.Drawing.Size(144, 21);
            this.cboMeses.TabIndex = 5;
            // 
            // chkAplicarMes
            // 
            this.chkAplicarMes.Location = new System.Drawing.Point(163, 20);
            this.chkAplicarMes.Name = "chkAplicarMes";
            this.chkAplicarMes.Size = new System.Drawing.Size(47, 20);
            this.chkAplicarMes.TabIndex = 6;
            this.chkAplicarMes.Text = "Mes:";
            this.chkAplicarMes.CheckedChanged += new System.EventHandler(this.chkAplicarMes_CheckedChanged);
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // frmRecalcularNroRegistros_Fill_Panel
            // 
            // 
            // frmRecalcularNroRegistros_Fill_Panel.ClientArea
            // 
            this.frmRecalcularNroRegistros_Fill_Panel.ClientArea.Controls.Add(this.btnActualizarNombre);
            this.frmRecalcularNroRegistros_Fill_Panel.ClientArea.Controls.Add(this.chkSoloVentas);
            this.frmRecalcularNroRegistros_Fill_Panel.ClientArea.Controls.Add(this.btnEmparejarLiquidacionCompra);
            this.frmRecalcularNroRegistros_Fill_Panel.ClientArea.Controls.Add(this.btnEmparejar);
            this.frmRecalcularNroRegistros_Fill_Panel.ClientArea.Controls.Add(this.chkSoloCompras);
            this.frmRecalcularNroRegistros_Fill_Panel.ClientArea.Controls.Add(this.chkAplicarMes);
            this.frmRecalcularNroRegistros_Fill_Panel.ClientArea.Controls.Add(this.cboMeses);
            this.frmRecalcularNroRegistros_Fill_Panel.ClientArea.Controls.Add(this.label1);
            this.frmRecalcularNroRegistros_Fill_Panel.ClientArea.Controls.Add(this.txtPeriodo);
            this.frmRecalcularNroRegistros_Fill_Panel.ClientArea.Controls.Add(this.ultraButton1);
            this.frmRecalcularNroRegistros_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.frmRecalcularNroRegistros_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmRecalcularNroRegistros_Fill_Panel.Location = new System.Drawing.Point(8, 31);
            this.frmRecalcularNroRegistros_Fill_Panel.Name = "frmRecalcularNroRegistros_Fill_Panel";
            this.frmRecalcularNroRegistros_Fill_Panel.Size = new System.Drawing.Size(550, 113);
            this.frmRecalcularNroRegistros_Fill_Panel.TabIndex = 0;
            // 
            // chkSoloVentas
            // 
            this.chkSoloVentas.Location = new System.Drawing.Point(21, 82);
            this.chkSoloVentas.Name = "chkSoloVentas";
            this.chkSoloVentas.Size = new System.Drawing.Size(105, 20);
            this.chkSoloVentas.TabIndex = 10;
            this.chkSoloVentas.Text = "Sólo Ventas";
            // 
            // btnEmparejarLiquidacionCompra
            // 
            this.btnEmparejarLiquidacionCompra.Location = new System.Drawing.Point(366, 53);
            this.btnEmparejarLiquidacionCompra.Name = "btnEmparejarLiquidacionCompra";
            this.btnEmparejarLiquidacionCompra.Size = new System.Drawing.Size(173, 23);
            this.btnEmparejarLiquidacionCompra.TabIndex = 9;
            this.btnEmparejarLiquidacionCompra.Text = "Emparejar Liquidación Compra";
            this.btnEmparejarLiquidacionCompra.UseVisualStyleBackColor = true;
            this.btnEmparejarLiquidacionCompra.Click += new System.EventHandler(this.btnEmparejarLiquidacionCompra_Click);
            // 
            // btnEmparejar
            // 
            this.btnEmparejar.Location = new System.Drawing.Point(135, 54);
            this.btnEmparejar.Name = "btnEmparejar";
            this.btnEmparejar.Size = new System.Drawing.Size(75, 23);
            this.btnEmparejar.TabIndex = 8;
            this.btnEmparejar.Text = "Emparejar";
            this.btnEmparejar.UseVisualStyleBackColor = true;
            this.btnEmparejar.Click += new System.EventHandler(this.btnEmparejar_Click);
            // 
            // chkSoloCompras
            // 
            this.chkSoloCompras.Location = new System.Drawing.Point(21, 56);
            this.chkSoloCompras.Name = "chkSoloCompras";
            this.chkSoloCompras.Size = new System.Drawing.Size(105, 20);
            this.chkSoloCompras.TabIndex = 7;
            this.chkSoloCompras.Text = "Sólo Compras";
            // 
            // _frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Left
            // 
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 31);
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Left.Name = "_frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Left";
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 113);
            // 
            // _frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Right
            // 
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(558, 31);
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Right.Name = "_frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Right";
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 113);
            // 
            // _frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Top
            // 
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Top.Name = "_frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Top";
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(566, 31);
            // 
            // _frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Bottom
            // 
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 144);
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Bottom.Name = "_frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Bottom";
            this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(566, 8);
            // 
            // btnActualizarNombre
            // 
            this.btnActualizarNombre.Location = new System.Drawing.Point(366, 82);
            this.btnActualizarNombre.Name = "btnActualizarNombre";
            this.btnActualizarNombre.Size = new System.Drawing.Size(173, 23);
            this.btnActualizarNombre.TabIndex = 10;
            this.btnActualizarNombre.Text = "Actualizar Nombre Diario";
            this.btnActualizarNombre.UseVisualStyleBackColor = true;
            this.btnActualizarNombre.Click += new System.EventHandler(this.btnActualizarNombre_Click);
            // 
            // frmRecalcularNroRegistros
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(566, 152);
            this.Controls.Add(this.frmRecalcularNroRegistros_Fill_Panel);
            this.Controls.Add(this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Bottom);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmRecalcularNroRegistros";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Recalcular Nro. Registros";
            this.Load += new System.EventHandler(this.frmRecalcularNroRegistros_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtPeriodo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboMeses)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkAplicarMes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            this.frmRecalcularNroRegistros_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmRecalcularNroRegistros_Fill_Panel.ClientArea.PerformLayout();
            this.frmRecalcularNroRegistros_Fill_Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chkSoloVentas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkSoloCompras)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraButton ultraButton1;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor txtPeriodo;
        private System.Windows.Forms.Label label1;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboMeses;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkAplicarMes;
        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.Misc.UltraPanel frmRecalcularNroRegistros_Fill_Panel;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmRecalcularNroRegistros_UltraFormManager_Dock_Area_Bottom;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkSoloCompras;
        private System.Windows.Forms.Button btnEmparejar;
        private System.Windows.Forms.Button btnEmparejarLiquidacionCompra;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor chkSoloVentas;
        private System.Windows.Forms.Button btnActualizarNombre;
    }
}