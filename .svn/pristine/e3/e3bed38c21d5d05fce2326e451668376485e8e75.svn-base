using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class FrmDocumentoRol : Form
    {
        private readonly UltraCombo _ucRolDocumento = new UltraCombo();

        public FrmDocumentoRol()
        {
            _ucRolDocumento.DataSource = Enum.GetValues(typeof(DocumentoRolBl.RolDocumento));
            InitializeComponent();
        }

        private void frmDocumentoRol_Load(object sender, EventArgs e)
        {
            var objOperationResult = new OperationResult();
            var ds = DocumentoRolBl.ObtenerRolesDocumento(ref objOperationResult);
            if (objOperationResult.Success == 1)
            {
                ultraGrid1.DataSource = ds;
            }
        }

        private void ultraGrid1_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["RolEnum"].EditorComponent = _ucRolDocumento;
            e.Layout.Bands[0].Columns["RolEnum"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            e.Layout.Bands[0].Columns["RolEnum"].CellActivation = Activation.AllowEdit;
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            UltraGridRow row = ultraGrid1.DisplayLayout.Bands[0].AddNew();
            ultraGrid1.Rows.Move(row, ultraGrid1.Rows.Count() - 1);
            this.ultraGrid1.ActiveRowScrollRegion.ScrollRowIntoView(row);
            row.Cells["IdRol"].Value = -1;
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            var objOperationResult = new OperationResult();
            DocumentoRolBl.ActualizarDocumentosRol(ref objOperationResult,
                new List<DocumentoRolBl.RelacionDocumentoRol>(
                    (BindingList<DocumentoRolBl.RelacionDocumentoRol>) ultraGrid1.DataSource));

            if (objOperationResult.Success == 0)
            {
                MessageBox.Show(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage, @"Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show(@"Datos Actualizados", @"Sistema", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }
    }
}
