namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    partial class RecalcularDiarios
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
            this.ultraGroupBox1 = new Infragistics.Win.Misc.UltraGroupBox();
            this.lblPeriodo = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.btnRecalcular = new Infragistics.Win.Misc.UltraButton();
            this.dtpFechaRegistroDe = new System.Windows.Forms.DateTimePicker();
            this.label1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraFormManager1 = new Infragistics.Win.UltraWinForm.UltraFormManager(this.components);
            this.RecalcularDiarios_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Left = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Right = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Top = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Bottom = new Infragistics.Win.UltraWinForm.UltraFormDockArea();
            this.bwkProcesoBL = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).BeginInit();
            this.ultraGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).BeginInit();
            this.RecalcularDiarios_Fill_Panel.ClientArea.SuspendLayout();
            this.RecalcularDiarios_Fill_Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ultraGroupBox1
            // 
            this.ultraGroupBox1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.ultraGroupBox1.BorderStyle = Infragistics.Win.Misc.GroupBoxBorderStyle.RectangularDoubleSolid;
            this.ultraGroupBox1.Controls.Add(this.lblPeriodo);
            this.ultraGroupBox1.Controls.Add(this.ultraLabel1);
            this.ultraGroupBox1.Controls.Add(this.btnRecalcular);
            this.ultraGroupBox1.Controls.Add(this.dtpFechaRegistroDe);
            this.ultraGroupBox1.Controls.Add(this.label1);
            this.ultraGroupBox1.Location = new System.Drawing.Point(5, 26);
            this.ultraGroupBox1.Name = "ultraGroupBox1";
            this.ultraGroupBox1.Size = new System.Drawing.Size(442, 95);
            this.ultraGroupBox1.TabIndex = 6;
            this.ultraGroupBox1.Text = "Iniciar Proceso";
            // 
            // lblPeriodo
            // 
            this.lblPeriodo.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblPeriodo.Location = new System.Drawing.Point(75, 43);
            this.lblPeriodo.Name = "lblPeriodo";
            this.lblPeriodo.Size = new System.Drawing.Size(75, 23);
            this.lblPeriodo.TabIndex = 266;
            this.lblPeriodo.Text = "ultraLabel2";
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(23, 46);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(46, 14);
            this.ultraLabel1.TabIndex = 265;
            this.ultraLabel1.Text = "Periodo:";
            // 
            // btnRecalcular
            // 
            appearance1.Image = global::SAMBHS.Windows.WinClient.UI.Resource.database_gear;
            this.btnRecalcular.Appearance = appearance1;
            this.btnRecalcular.Location = new System.Drawing.Point(343, 36);
            this.btnRecalcular.Name = "btnRecalcular";
            this.btnRecalcular.Size = new System.Drawing.Size(92, 35);
            this.btnRecalcular.TabIndex = 264;
            this.btnRecalcular.Text = "Recalcular";
            this.btnRecalcular.Click += new System.EventHandler(this.btnRecalcular_Click);
            // 
            // dtpFechaRegistroDe
            // 
            this.dtpFechaRegistroDe.AllowDrop = true;
            this.dtpFechaRegistroDe.CustomFormat = "MMMM-YY";
            this.dtpFechaRegistroDe.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaRegistroDe.Location = new System.Drawing.Point(235, 43);
            this.dtpFechaRegistroDe.Name = "dtpFechaRegistroDe";
            this.dtpFechaRegistroDe.Size = new System.Drawing.Size(82, 20);
            this.dtpFechaRegistroDe.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(156, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 14);
            this.label1.TabIndex = 4;
            this.label1.Text = "Hasta el Mes:";
            // 
            // ultraFormManager1
            // 
            this.ultraFormManager1.Form = this;
            // 
            // RecalcularDiarios_Fill_Panel
            // 
            // 
            // RecalcularDiarios_Fill_Panel.ClientArea
            // 
            this.RecalcularDiarios_Fill_Panel.ClientArea.Controls.Add(this.ultraGroupBox1);
            this.RecalcularDiarios_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RecalcularDiarios_Fill_Panel.Location = new System.Drawing.Point(8, 31);
            this.RecalcularDiarios_Fill_Panel.Name = "RecalcularDiarios_Fill_Panel";
            this.RecalcularDiarios_Fill_Panel.Size = new System.Drawing.Size(465, 153);
            this.RecalcularDiarios_Fill_Panel.TabIndex = 0;
            // 
            // _RecalcularDiarios_UltraFormManager_Dock_Area_Left
            // 
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Left;
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Left.FormManager = this.ultraFormManager1;
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Left.InitialResizeAreaExtent = 8;
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Left.Location = new System.Drawing.Point(0, 31);
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Left.Name = "_RecalcularDiarios_UltraFormManager_Dock_Area_Left";
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Left.Size = new System.Drawing.Size(8, 153);
            // 
            // _RecalcularDiarios_UltraFormManager_Dock_Area_Right
            // 
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Right;
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Right.FormManager = this.ultraFormManager1;
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Right.InitialResizeAreaExtent = 8;
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Right.Location = new System.Drawing.Point(473, 31);
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Right.Name = "_RecalcularDiarios_UltraFormManager_Dock_Area_Right";
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Right.Size = new System.Drawing.Size(8, 153);
            // 
            // _RecalcularDiarios_UltraFormManager_Dock_Area_Top
            // 
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Top;
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Top.FormManager = this.ultraFormManager1;
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Top.Name = "_RecalcularDiarios_UltraFormManager_Dock_Area_Top";
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Top.Size = new System.Drawing.Size(481, 31);
            // 
            // _RecalcularDiarios_UltraFormManager_Dock_Area_Bottom
            // 
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinForm.DockedPosition.Bottom;
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Bottom.FormManager = this.ultraFormManager1;
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Bottom.InitialResizeAreaExtent = 8;
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 184);
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Bottom.Name = "_RecalcularDiarios_UltraFormManager_Dock_Area_Bottom";
            this._RecalcularDiarios_UltraFormManager_Dock_Area_Bottom.Size = new System.Drawing.Size(481, 8);
            // 
            // RecalcularDiarios
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(481, 192);
            this.Controls.Add(this.RecalcularDiarios_Fill_Panel);
            this.Controls.Add(this._RecalcularDiarios_UltraFormManager_Dock_Area_Left);
            this.Controls.Add(this._RecalcularDiarios_UltraFormManager_Dock_Area_Right);
            this.Controls.Add(this._RecalcularDiarios_UltraFormManager_Dock_Area_Top);
            this.Controls.Add(this._RecalcularDiarios_UltraFormManager_Dock_Area_Bottom);
            this.Name = "RecalcularDiarios";
            this.ShowIcon = false;
            this.Text = "Regenerar Diarios Liquidación Compra";
            this.Load += new System.EventHandler(this.RecalcularDiarios_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGroupBox1)).EndInit();
            this.ultraGroupBox1.ResumeLayout(false);
            this.ultraGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraFormManager1)).EndInit();
            this.RecalcularDiarios_Fill_Panel.ClientArea.ResumeLayout(false);
            this.RecalcularDiarios_Fill_Panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraGroupBox ultraGroupBox1;
        private Infragistics.Win.Misc.UltraLabel lblPeriodo;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraButton btnRecalcular;
        private System.Windows.Forms.DateTimePicker dtpFechaRegistroDe;
        private Infragistics.Win.Misc.UltraLabel label1;
        private Infragistics.Win.UltraWinForm.UltraFormManager ultraFormManager1;
        private Infragistics.Win.Misc.UltraPanel RecalcularDiarios_Fill_Panel;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _RecalcularDiarios_UltraFormManager_Dock_Area_Left;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _RecalcularDiarios_UltraFormManager_Dock_Area_Right;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _RecalcularDiarios_UltraFormManager_Dock_Area_Top;
        private Infragistics.Win.UltraWinForm.UltraFormDockArea _RecalcularDiarios_UltraFormManager_Dock_Area_Bottom;
        private System.ComponentModel.BackgroundWorker bwkProcesoBL;
    }
}