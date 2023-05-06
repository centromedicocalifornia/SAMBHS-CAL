using System;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.Requerimientos.NBS;
using System.Linq;
using System.Drawing;
using SAMBHS.Windows.WinClient.UI.Mantenimientos;
using Infragistics.Win.UltraWinGrid;


namespace SAMBHS.Windows.WinClient.UI.Requerimientos.NotariaBecerraSosaya
{
    public partial class frmBandejaFormatoUnicoFacturacion : Form
    {

        public string v_IdFormatoUnicoFacturacion = null;
        public string IdCliente = string.Empty;
        public string TipoDocumento = string.Empty;
        string IdTipoDocumento = string.Empty;
        public frmBandejaFormatoUnicoFacturacion(string TipoDocumento)
        {
            InitializeComponent();

            IdTipoDocumento = TipoDocumento;

            if (IdTipoDocumento != "N")
            {
                this.Text = "Formato Único de Facturación";
                btnEliminar.Visible = false;
                btnNuevo.Visible = false;
                btnEditar.Visible = false;
                this.Size = new System.Drawing.Size(1010, 650);
                this.StartPosition = FormStartPosition.CenterParent;
             
            }
        }

        private void frmBandejaFormatoUnicoFacturacion_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
        }

        private void ultraButton3_Click(object sender, EventArgs e)
        {
            //frmFormatoUnicoFacturacion f = new frmFormatoUnicoFacturacion(frmFormatoUnicoFacturacion.TipoAccion.Nuevo);
            //f.Show();
            //btnBuscar_Click(sender, e);
            //MantenerSeleccion(f.IdFormatoUnico);
            SAMBHS.Windows.WinClient.UI.Requerimientos.NotariaBecerraSosaya.frmFormatoUnicoFacturacion frm = new frmFormatoUnicoFacturacion(frmFormatoUnicoFacturacion.TipoAccion.Nuevo);
            frm.FormClosed += (_, ev) =>
            {
                btnBuscar_Click(sender, e);
                MantenerSeleccion(frm.IdFormatoUnico);
            };

            ((frmMaster)MdiParent).RegistrarForm(this, frm);
        }
        private void MantenerSeleccion(string ValorSeleccionado)
        {
            if (string.IsNullOrEmpty(ValorSeleccionado)) return;
            var filas = grdData.Rows.GetRowEnumerator(GridRowType.DataRow, null, null).Cast<UltraGridRow>().ToList();
            var fila = filas.FirstOrDefault(f => f.Cells["v_IdFormatoUnicoFacturacion"].Value.ToString().Contains(ValorSeleccionado));
            if (fila != null) fila.Activate();
        }
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow) return;
            var id = grdData.ActiveRow.Cells["v_IdFormatoUnicoFacturacion"].Value.ToString();
            
            if (grdData.ActiveRow.Cells["NroDocumentoVenta"].Value != null &&  grdData.ActiveRow.Cells["NroDocumentoVenta"].Value.ToString ()!= string.Empty)

            {
                UltraMessageBox.Show("No se puede Eliminar,FUF  está siendo utilizado en los sgtes. doc. de Facturación :  " + grdData.ActiveRow.Cells["NroDocumentoVenta"].Value.ToString(), "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var resp = MessageBox.Show("¿Seguro de Eliminar este Registro?", "Confirmación", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (resp == DialogResult.No) return;
            FormatoUnicoFacturacionBl.EliminarFormatoUnicoFacturacion(ref objOperationResult, id, Globals.ClientSession.GetAsList());
            if (objOperationResult.Success == 0)
            {
                MessageBox.Show(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage, @"Error en la transacción.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            btnBuscar_Click(sender, e);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            var objOperationResult = new OperationResult();
            var ds = FormatoUnicoFacturacionBl.ObtenerFormatosUnicosBandeja(ref objOperationResult, dtpFechaInicio.Value.Date, dtpFechaFin.Value, IdCliente, IdTipoDocumento);
            if (objOperationResult.Success == 0)
            {
                MessageBox.Show(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage, @"Error en la transacción.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            grdData.DataSource = ds;
            lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", ds.Count);
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow) return;
            var id = grdData.ActiveRow.Cells["v_IdFormatoUnicoFacturacion"].Value.ToString();
            frmFormatoUnicoFacturacion f = new frmFormatoUnicoFacturacion(frmFormatoUnicoFacturacion.TipoAccion.Editar, id);
            f.Show();
            btnBuscar_Click(sender,e);

        }

        private void grdData_DoubleClick(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow) return;
            if (IdTipoDocumento == String.Empty)
            {
                return;
            }
            else if (IdTipoDocumento == "N")
            {
                var id = grdData.ActiveRow.Cells["v_IdFormatoUnicoFacturacion"].Value.ToString();
                frmFormatoUnicoFacturacion f = new frmFormatoUnicoFacturacion(frmFormatoUnicoFacturacion.TipoAccion.Editar, id);
                f.ShowDialog();
            }
            else
            {
                v_IdFormatoUnicoFacturacion = grdData.ActiveRow.Cells["v_IdFormatoUnicoFacturacion"].Value.ToString();
                Close();
            }
        }

        private void txtCliente_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            frmBuscarCliente f = new frmBuscarCliente("V", "");
            f.ShowDialog();

            if (f._IdCliente != null)
            {

                txtCliente.Text = f._RazonSocial;
                IdCliente = f._IdCliente;

            }
        }

        private void grdData_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow) return;

            if (grdData.ActiveRow != null && IdTipoDocumento == "N")
            {
                if (btnEditar.Enabled) btnEditar_Click(sender, e);
            }
        }

        private void grdData_KeyDown(object sender, KeyEventArgs e)
        {

            object sender1 = new object();
            EventArgs e1 = new EventArgs();
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    grdData_DoubleClick(sender1, e1);
                    break;

            }
        }
    }
}
