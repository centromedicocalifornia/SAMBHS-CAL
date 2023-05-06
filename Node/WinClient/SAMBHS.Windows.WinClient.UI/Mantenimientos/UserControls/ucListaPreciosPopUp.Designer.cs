namespace SAMBHS.Windows.WinClient.UI.Mantenimientos.UserControls
{
    partial class ucListaPreciosPopUp
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("NombreLista");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Moneda");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("d_Precio", 0);
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            this.grListaPrecios = new Infragistics.Win.UltraWinGrid.UltraGrid();
            ((System.ComponentModel.ISupportInitialize)(this.grListaPrecios)).BeginInit();
            this.SuspendLayout();
            // 
            // grListaPrecios
            // 
            this.grListaPrecios.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grListaPrecios.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn1.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn1.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.CellSelect;
            ultraGridColumn1.Header.Caption = "Lista";
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Width = 75;
            ultraGridColumn3.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn3.Header.VisiblePosition = 1;
            ultraGridColumn3.Width = 71;
            ultraGridColumn2.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            appearance1.TextHAlignAsString = "Right";
            ultraGridColumn2.CellAppearance = appearance1;
            ultraGridColumn2.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.CellSelect;
            ultraGridColumn2.Header.Caption = "Precio";
            ultraGridColumn2.Header.VisiblePosition = 2;
            ultraGridColumn2.Width = 78;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn3,
            ultraGridColumn2});
            this.grListaPrecios.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.grListaPrecios.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.True;
            this.grListaPrecios.DisplayLayout.NewColumnLoadStyle = Infragistics.Win.UltraWinGrid.NewColumnLoadStyle.Hide;
            this.grListaPrecios.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.grListaPrecios.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grListaPrecios.Location = new System.Drawing.Point(0, 0);
            this.grListaPrecios.Name = "grListaPrecios";
            this.grListaPrecios.Size = new System.Drawing.Size(245, 184);
            this.grListaPrecios.TabIndex = 0;
            this.grListaPrecios.Text = "Tarifario";
            this.grListaPrecios.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.grListaPrecios_InitializeLayout);
            this.grListaPrecios.DoubleClickCell += new Infragistics.Win.UltraWinGrid.DoubleClickCellEventHandler(this.grListaPrecios_DoubleClickCell);
            // 
            // ucListaPreciosPopUp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grListaPrecios);
            this.Name = "ucListaPreciosPopUp";
            this.Size = new System.Drawing.Size(245, 187);
            this.Load += new System.EventHandler(this.ucListaPreciosPopUp_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grListaPrecios)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinGrid.UltraGrid grListaPrecios;

    }
}
