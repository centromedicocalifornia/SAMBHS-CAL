namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    partial class frmNuevoRegistro
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmNuevoRegistro));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnBuscarTrabajador = new System.Windows.Forms.Button();
            this.label15 = new System.Windows.Forms.Label();
            this.txtSearchNroDocument = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtNombres = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtApellidoMaterno = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtApellidoPaterno = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.dtpBirthdate = new System.Windows.Forms.DateTimePicker();
            this.label11 = new System.Windows.Forms.Label();
            this.txtNroDocumento = new System.Windows.Forms.TextBox();
            this.cboGenero = new System.Windows.Forms.ComboBox();
            this.cboTipoDocumento = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnCapturedFingerPrintAndRubric = new System.Windows.Forms.Button();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.btnWebCam = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.btnArchivo1 = new System.Windows.Forms.Button();
            this.pbPersonImage = new System.Windows.Forms.PictureBox();
            this.btnSavePacient = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPersonImage)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnBuscarTrabajador);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.txtSearchNroDocument);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.MediumBlue;
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(490, 49);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Búsqueda";
            // 
            // btnBuscarTrabajador
            // 
            this.btnBuscarTrabajador.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBuscarTrabajador.BackColor = System.Drawing.SystemColors.Control;
            this.btnBuscarTrabajador.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnBuscarTrabajador.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.btnBuscarTrabajador.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.btnBuscarTrabajador.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBuscarTrabajador.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBuscarTrabajador.ForeColor = System.Drawing.Color.Black;
            this.btnBuscarTrabajador.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBuscarTrabajador.Location = new System.Drawing.Point(330, 16);
            this.btnBuscarTrabajador.Margin = new System.Windows.Forms.Padding(2);
            this.btnBuscarTrabajador.Name = "btnBuscarTrabajador";
            this.btnBuscarTrabajador.Size = new System.Drawing.Size(147, 21);
            this.btnBuscarTrabajador.TabIndex = 24;
            this.btnBuscarTrabajador.Text = "Buscar Trabajador";
            this.btnBuscarTrabajador.UseVisualStyleBackColor = false;
            this.btnBuscarTrabajador.Click += new System.EventHandler(this.btnBuscarTrabajador_Click);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.ForeColor = System.Drawing.Color.Black;
            this.label15.Location = new System.Drawing.Point(5, 22);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(95, 13);
            this.label15.TabIndex = 0;
            this.label15.Text = "Nro Documento";
            // 
            // txtSearchNroDocument
            // 
            this.txtSearchNroDocument.ForeColor = System.Drawing.Color.Black;
            this.txtSearchNroDocument.Location = new System.Drawing.Point(102, 16);
            this.txtSearchNroDocument.Margin = new System.Windows.Forms.Padding(2);
            this.txtSearchNroDocument.Name = "txtSearchNroDocument";
            this.txtSearchNroDocument.Size = new System.Drawing.Size(130, 20);
            this.txtSearchNroDocument.TabIndex = 1;
            this.txtSearchNroDocument.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSearchNroDocument_KeyPress);
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.SystemColors.Info;
            this.groupBox3.Controls.Add(this.txtNombres);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.txtApellidoMaterno);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.txtApellidoPaterno);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.dtpBirthdate);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.txtNroDocumento);
            this.groupBox3.Controls.Add(this.cboGenero);
            this.groupBox3.Controls.Add(this.cboTipoDocumento);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.ForeColor = System.Drawing.Color.MediumBlue;
            this.groupBox3.Location = new System.Drawing.Point(12, 67);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(490, 166);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Datos Generales";
            // 
            // txtNombres
            // 
            this.txtNombres.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNombres.Location = new System.Drawing.Point(94, 18);
            this.txtNombres.Margin = new System.Windows.Forms.Padding(2);
            this.txtNombres.MaxLength = 50;
            this.txtNombres.Name = "txtNombres";
            this.txtNombres.Size = new System.Drawing.Size(172, 20);
            this.txtNombres.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(37, 22);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Nombres";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(15, 92);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Ape. Materno";
            // 
            // txtApellidoMaterno
            // 
            this.txtApellidoMaterno.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtApellidoMaterno.Location = new System.Drawing.Point(94, 88);
            this.txtApellidoMaterno.Margin = new System.Windows.Forms.Padding(2);
            this.txtApellidoMaterno.MaxLength = 50;
            this.txtApellidoMaterno.Name = "txtApellidoMaterno";
            this.txtApellidoMaterno.Size = new System.Drawing.Size(172, 20);
            this.txtApellidoMaterno.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(17, 55);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Ape. Paterno";
            // 
            // txtApellidoPaterno
            // 
            this.txtApellidoPaterno.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtApellidoPaterno.Location = new System.Drawing.Point(94, 51);
            this.txtApellidoPaterno.Margin = new System.Windows.Forms.Padding(2);
            this.txtApellidoPaterno.MaxLength = 50;
            this.txtApellidoPaterno.Name = "txtApellidoPaterno";
            this.txtApellidoPaterno.Size = new System.Drawing.Size(172, 20);
            this.txtApellidoPaterno.TabIndex = 2;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.Color.Black;
            this.label12.Location = new System.Drawing.Point(276, 55);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(50, 13);
            this.label12.TabIndex = 4;
            this.label12.Text = "Nro Doc.";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dtpBirthdate
            // 
            this.dtpBirthdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpBirthdate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpBirthdate.Location = new System.Drawing.Point(94, 125);
            this.dtpBirthdate.Margin = new System.Windows.Forms.Padding(2);
            this.dtpBirthdate.Name = "dtpBirthdate";
            this.dtpBirthdate.ShowCheckBox = true;
            this.dtpBirthdate.Size = new System.Drawing.Size(96, 20);
            this.dtpBirthdate.TabIndex = 25;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.Black;
            this.label11.Location = new System.Drawing.Point(284, 92);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(42, 13);
            this.label11.TabIndex = 15;
            this.label11.Text = "Género";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtNroDocumento
            // 
            this.txtNroDocumento.Location = new System.Drawing.Point(330, 51);
            this.txtNroDocumento.Margin = new System.Windows.Forms.Padding(2);
            this.txtNroDocumento.MaxLength = 20;
            this.txtNroDocumento.Name = "txtNroDocumento";
            this.txtNroDocumento.Size = new System.Drawing.Size(147, 20);
            this.txtNroDocumento.TabIndex = 5;
            // 
            // cboGenero
            // 
            this.cboGenero.DisplayMember = "Nombre";
            this.cboGenero.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGenero.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboGenero.FormattingEnabled = true;
            this.cboGenero.Location = new System.Drawing.Point(330, 88);
            this.cboGenero.Margin = new System.Windows.Forms.Padding(2);
            this.cboGenero.Name = "cboGenero";
            this.cboGenero.Size = new System.Drawing.Size(147, 21);
            this.cboGenero.TabIndex = 6;
            this.cboGenero.ValueMember = "EsoId";
            // 
            // cboTipoDocumento
            // 
            this.cboTipoDocumento.DisplayMember = "Nombre";
            this.cboTipoDocumento.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTipoDocumento.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboTipoDocumento.FormattingEnabled = true;
            this.cboTipoDocumento.Items.AddRange(new object[] {
            "\"1\", \"Carnet de Extranjería\"",
            "\"2\", \"DNI\"",
            "\"3\", \"Licencia de Conducir\"",
            "\"4\", \"Pasaporte\""});
            this.cboTipoDocumento.Location = new System.Drawing.Point(330, 18);
            this.cboTipoDocumento.Margin = new System.Windows.Forms.Padding(2);
            this.cboTipoDocumento.Name = "cboTipoDocumento";
            this.cboTipoDocumento.Size = new System.Drawing.Size(147, 21);
            this.cboTipoDocumento.TabIndex = 4;
            this.cboTipoDocumento.ValueMember = "EsoId";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.Color.Black;
            this.label13.Location = new System.Drawing.Point(272, 22);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(54, 13);
            this.label13.TabIndex = 13;
            this.label13.Text = "Tipo Doc.";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.Black;
            this.label9.Location = new System.Drawing.Point(2, 129);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(84, 13);
            this.label9.TabIndex = 17;
            this.label9.Text = "Fec. Nacimiento";
            // 
            // btnClear
            // 
            this.btnClear.Image = ((System.Drawing.Image)(resources.GetObject("btnClear.Image")));
            this.btnClear.Location = new System.Drawing.Point(805, 207);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(37, 31);
            this.btnClear.TabIndex = 156;
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnCapturedFingerPrintAndRubric
            // 
            this.btnCapturedFingerPrintAndRubric.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCapturedFingerPrintAndRubric.ForeColor = System.Drawing.Color.Black;
            this.btnCapturedFingerPrintAndRubric.Image = global::SAMBHS.Windows.SigesoftIntegration.UI.Properties.Resources.tag_blue_edit;
            this.btnCapturedFingerPrintAndRubric.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCapturedFingerPrintAndRubric.Location = new System.Drawing.Point(665, 239);
            this.btnCapturedFingerPrintAndRubric.Name = "btnCapturedFingerPrintAndRubric";
            this.btnCapturedFingerPrintAndRubric.Size = new System.Drawing.Size(177, 31);
            this.btnCapturedFingerPrintAndRubric.TabIndex = 158;
            this.btnCapturedFingerPrintAndRubric.Text = "Capturar Huella Digital y Firma";
            this.btnCapturedFingerPrintAndRubric.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCapturedFingerPrintAndRubric.UseVisualStyleBackColor = true;
            this.btnCapturedFingerPrintAndRubric.Click += new System.EventHandler(this.btnCapturedFingerPrintAndRubric_Click);
            // 
            // txtFileName
            // 
            this.txtFileName.BackColor = System.Drawing.SystemColors.Control;
            this.txtFileName.Location = new System.Drawing.Point(573, 212);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.ReadOnly = true;
            this.txtFileName.Size = new System.Drawing.Size(184, 20);
            this.txtFileName.TabIndex = 155;
            // 
            // btnWebCam
            // 
            this.btnWebCam.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnWebCam.ForeColor = System.Drawing.Color.Black;
            this.btnWebCam.Image = global::SAMBHS.Windows.SigesoftIntegration.UI.Properties.Resources.user_suit_black;
            this.btnWebCam.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnWebCam.Location = new System.Drawing.Point(534, 239);
            this.btnWebCam.Name = "btnWebCam";
            this.btnWebCam.Size = new System.Drawing.Size(86, 31);
            this.btnWebCam.TabIndex = 157;
            this.btnWebCam.Text = "Tomar foto";
            this.btnWebCam.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnWebCam.UseVisualStyleBackColor = true;
            this.btnWebCam.Click += new System.EventHandler(this.btnWebCam_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(537, 216);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 154;
            this.label4.Text = "Ruta";
            // 
            // btnArchivo1
            // 
            this.btnArchivo1.Image = ((System.Drawing.Image)(resources.GetObject("btnArchivo1.Image")));
            this.btnArchivo1.Location = new System.Drawing.Point(763, 207);
            this.btnArchivo1.Name = "btnArchivo1";
            this.btnArchivo1.Size = new System.Drawing.Size(37, 31);
            this.btnArchivo1.TabIndex = 153;
            this.btnArchivo1.UseVisualStyleBackColor = true;
            this.btnArchivo1.Click += new System.EventHandler(this.btnArchivo1_Click);
            // 
            // pbPersonImage
            // 
            this.pbPersonImage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pbPersonImage.Image = global::SAMBHS.Windows.SigesoftIntegration.UI.Properties.Resources.usuario;
            this.pbPersonImage.Location = new System.Drawing.Point(534, 12);
            this.pbPersonImage.Margin = new System.Windows.Forms.Padding(2);
            this.pbPersonImage.Name = "pbPersonImage";
            this.pbPersonImage.Size = new System.Drawing.Size(308, 190);
            this.pbPersonImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbPersonImage.TabIndex = 152;
            this.pbPersonImage.TabStop = false;
            // 
            // btnSavePacient
            // 
            this.btnSavePacient.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSavePacient.BackColor = System.Drawing.SystemColors.Control;
            this.btnSavePacient.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnSavePacient.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.btnSavePacient.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.btnSavePacient.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSavePacient.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSavePacient.ForeColor = System.Drawing.Color.Black;
            this.btnSavePacient.Image = global::SAMBHS.Windows.SigesoftIntegration.UI.Properties.Resources.disk;
            this.btnSavePacient.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSavePacient.Location = new System.Drawing.Point(342, 239);
            this.btnSavePacient.Margin = new System.Windows.Forms.Padding(2);
            this.btnSavePacient.Name = "btnSavePacient";
            this.btnSavePacient.Size = new System.Drawing.Size(156, 31);
            this.btnSavePacient.TabIndex = 151;
            this.btnSavePacient.Text = "Guardar Nuevo Paciente";
            this.btnSavePacient.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSavePacient.UseVisualStyleBackColor = false;
            this.btnSavePacient.Click += new System.EventHandler(this.btnSavePacient_Click);
            // 
            // frmNuevoRegistro
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(852, 281);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnCapturedFingerPrintAndRubric);
            this.Controls.Add(this.txtFileName);
            this.Controls.Add(this.btnWebCam);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnArchivo1);
            this.Controls.Add(this.pbPersonImage);
            this.Controls.Add(this.btnSavePacient);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Name = "frmNuevoRegistro";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Nuevo Registro";
            this.Load += new System.EventHandler(this.frmNuevoRegistro_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPersonImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnBuscarTrabajador;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtSearchNroDocument;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtNombres;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtApellidoMaterno;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtApellidoPaterno;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.DateTimePicker dtpBirthdate;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtNroDocumento;
        private System.Windows.Forms.ComboBox cboGenero;
        private System.Windows.Forms.ComboBox cboTipoDocumento;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnCapturedFingerPrintAndRubric;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.Button btnWebCam;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnArchivo1;
        private System.Windows.Forms.PictureBox pbPersonImage;
        private System.Windows.Forms.Button btnSavePacient;
    }
}