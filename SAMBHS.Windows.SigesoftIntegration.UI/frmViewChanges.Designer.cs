﻿namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    partial class frmViewChanges
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
            this.richComentarios = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // richComentarios
            // 
            this.richComentarios.Location = new System.Drawing.Point(88, 12);
            this.richComentarios.Name = "richComentarios";
            this.richComentarios.Size = new System.Drawing.Size(338, 253);
            this.richComentarios.TabIndex = 4;
            this.richComentarios.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Cambios";
            // 
            // frmViewChanges
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(438, 277);
            this.Controls.Add(this.richComentarios);
            this.Controls.Add(this.label1);
            this.Name = "frmViewChanges";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CambiosRealizados";
            this.Load += new System.EventHandler(this.frmViewChanges_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richComentarios;
        private System.Windows.Forms.Label label1;
    }
}