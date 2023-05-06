using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Almacen.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos.Migraciones
{
    public partial class frmMigrarLineas : Form
    {
        public frmMigrarLineas(IEnumerable<lineaDto> pLineas)
        {
            InitializeComponent();
            ultraGrid1.DataSource = pLineas.ToList();
        }

        private void frmMigrarLineas_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
            if (ultraGrid1.Rows.Any())
            {
                ultraGrid1.Rows.ToList()
                    .ForEach(r =>
                        r.Cells["v_CodLinea"]
                        .SetValue(string.Format("L{0}", (r.Index + 1).ToString("000")), false)
                        );
            }
        }

        private void ultraGrid1_KeyDown(object sender, KeyEventArgs e)
        {
            if (ultraGrid1.ActiveCell != null && (ultraGrid1.ActiveCell.Column.Key.Equals("v_NroCuentaVenta") ||
                           ultraGrid1.ActiveCell.Column.Key.Equals("v_NroCuentaCompra")))
            {
                switch (e.KeyCode)
                {
                    case Keys.Up:
                        ultraGrid1.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        ultraGrid1.PerformAction(UltraGridAction.AboveCell, false, false);
                        e.Handled = true;
                        ultraGrid1.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                    case Keys.Down:
                        ultraGrid1.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        ultraGrid1.PerformAction(UltraGridAction.BelowCell, false, false);
                        e.Handled = true;
                        ultraGrid1.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                }
            }
        }

        private void ultraGrid1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void ultraTextEditor1_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var f = new frmPlanCuentasConsulta("70");
            f.ShowDialog();

            if (f._NroSubCuenta != null)
            {
                ultraTextEditor1.Text = f._NroSubCuenta;
            }
        }

        private void ultraTextEditor2_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var f = new frmPlanCuentasConsulta("60");
            f.ShowDialog();

            if (f._NroSubCuenta != null)
            {
                ultraTextEditor2.Text = f._NroSubCuenta;
            }
        }

        private void ultraTextEditor1_ValueChanged(object sender, EventArgs e)
        {
            ultraGrid1.Rows.ToList().ForEach(f => f.Cells["v_NroCuentaVenta"].SetValue(ultraTextEditor1.Text, false));
        }

        private void ultraTextEditor2_ValueChanged(object sender, EventArgs e)
        {
            ultraGrid1.Rows.ToList().ForEach(f => f.Cells["v_NroCuentaCompra"].SetValue(ultraTextEditor2.Text, false));
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            var msj = MessageBox.Show(@"¿Seguro de continuar?", @"Sistema", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (msj == DialogResult.No) return;
            
            var objOperationRsult = new OperationResult();
            new LineaBL().MigrarLineas(ref objOperationRsult, (List<lineaDto>) ultraGrid1.DataSource);
            if (objOperationRsult.Success == 0)
            {
                UltraMessageBox.Show(objOperationRsult.ErrorMessage + "\n\n" + objOperationRsult.ExceptionMessage + "\n\nTARGET: " + objOperationRsult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show(@"Guardado exitoso!");
        }
    }
}
