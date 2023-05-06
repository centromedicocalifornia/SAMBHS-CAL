namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    partial class frmRecalcularSeparacionPedido
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
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this.frmRecalcularSeparacionPedido_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.btnRecalcularSeparacion = new Infragistics.Win.Misc.UltraButton();
            this.panel1 = new Infragistics.Win.Misc.UltraPanel();
            this.cboAlmacen = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.label2 = new Infragistics.Win.Misc.UltraLabel();
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this.bwkProcesoBL = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            this.frmRecalcularSeparacionPedido_Fill_Panel.ClientArea.SuspendLayout();
            this.frmRecalcularSeparacionPedido_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            this.panel1.ClientArea.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboAlmacen)).BeginInit();
            this.SuspendLayout();
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // frmRecalcularSeparacionPedido_Fill_Panel
            // 
            // 
            // frmRecalcularSeparacionPedido_Fill_Panel.ClientArea
            // 
            this.frmRecalcularSeparacionPedido_Fill_Panel.ClientArea.Controls.Add(this.ultraGroupBox1);
            this.frmRecalcularSeparacionPedido_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.frmRecalcularSeparacionPedido_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frmRecalcularSeparacionPedido_Fill_Panel.Location = new System.Drawing.Point(8, 31);
            this.frmRecalcularSeparacionPedido_Fill_Panel.Name = "frmRecalcularSeparacionPedido_Fill_Panel";
            this.frmRecalcularSeparacionPedido_Fill_Panel.Size = new System.Drawing.Size(1185, 460);
            this.frmRecalcularSeparacionPedido_Fill_Panel.TabIndex = 0;
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.ultraGroupBox1.BorderStyle = Infragistics.Win.Misc.GroupBoxBorderStyle.RectangularDoubleSolid;
            this.ultraGroupBox1.Controls.Add(this.btnRecalcularSeparacion);
            this.ultraGroupBox1.Controls.Add(this.panel1);
            this.ultraGroupBox1.Location = new System.Drawing.Point(366, 193);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(464, 111);
            this.ultraGroupBox1.TabIndex = 2;
            this.ultraGroupBox1.Text = "Re-calcular separación stock";
            this.ultraGroupBox1.UseAppStyling = false;
            // 
            // btnRecalcularSeparacion
            // 
            appearance1.Image = global::SAMBHS.Windows.WinClient.UI.Resource.database_gear;
            this.btnRecalcularSeparacion.Appearance = appearance1;
            this.btnRecalcularSeparacion.Location = new System.Drawing.Point(183, 64);
            this.btnRecalcularSeparacion.Name = "btnRecalcularSeparacion";
            this.btnRecalcularSeparacion.Size = new System.Drawing.Size(115, 41);
            this.btnRecalcularSeparacion.TabIndex = 1;
            this.btnRecalcularSeparacion.Text = "Re-calcular";
            this.btnRecalcularSeparacion.UseAppStyling = false;
            this.btnRecalcularSeparacion.Click += new System.EventHandler(this.btnRecalcularSeparacion_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // panel1.ClientArea
            // 
            this.panel1.ClientArea.Controls.Add(this.cboAlmacen);
            this.panel1.ClientArea.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(7, 15);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(450, 44);
            this.panel1.TabIndex = 0;
            // 
            // cboAlmacen
            // 
            this.cboAlmacen.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.cboAlmacen.Location = new System.Drawing.Point(166, 13);
            this.cboAlmacen.Name = "cboAlmacen";
            this.cboAlmacen.Size = new System.Drawing.Size(125, 21);
            this.cboAlmacen.TabIndex = 3;
            this.cboAlmacen.ValueChanged += new System.EventHandler(this.cboAlmacen_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(109, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 14);
            this.label2.TabIndex = 6;
            this.label2.Text = "Almacén:";
            // 
            // _frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Left
            // 
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 31);
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Left.Name = "_frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Left";
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 460);
            // 
            // _frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Right
            // 
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(1193, 31);
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Right.Name = "_frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Right";
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 460);
            // 
            // _frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Top
            // 
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Top.Name = "_frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Top";
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(1201, 31);
            // 
            // _frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Bottom
            // 
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 491);
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Bottom.Name = "_frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Bottom";
            this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(1201, 8);
            // 
            // bwkProcesoBL
            // 
            this.bwkProcesoBL.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwkProcesoBL_DoWork);
            // 
            // frmRecalcularSeparacionPedido
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1201, 499);
            this.Controls.Add(this.frmRecalcularSeparacionPedido_Fill_Panel);
            this.Controls.Add(this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Bottom);
            this.Name = "frmRecalcularSeparacionPedido";
            this.ShowIcon = false;
            this.Text = "Re-calcular separación stock";
            this.Load += new System.EventHandler(this.frmRecalcularSeparacionPedido_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            this.frmRecalcularSeparacionPedido_Fill_Panel.ClientArea.ResumeLayout(false);
            this.frmRecalcularSeparacionPedido_Fill_Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.panel1.ClientArea.ResumeLayout(false);
            this.panel1.ClientArea.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboAlmacen)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.Misc.UltraPanel frmRecalcularSeparacionPedido_Fill_Panel;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _frmRecalcularSeparacionPedido_UltraFormManager_Dock_Area_Bottom;
        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private Infragistics.Win.Misc.UltraButton btnRecalcularSeparacion;
        private Infragistics.Win.Misc.UltraPanel panel1;
        private Infragistics.Win.Misc.UltraLabel label2;
        private System.ComponentModel.BackgroundWorker bwkProcesoBL;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor cboAlmacen;
    }
}