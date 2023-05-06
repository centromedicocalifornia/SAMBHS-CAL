using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.Compra.BL;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmBuscarOrdenCompra : Form
    {
        #region Fields
        private readonly OrdenCompraBL _objOrdenCompraBL = new OrdenCompraBL(); 
        #endregion

        #region Properties
        /// <summary>
        /// Gets the result de la Orden de Compra seleccionada.
        /// </summary>
        /// <value>The result.</value>
        public string Result { get; private set; }
        #endregion

        #region Init
        public frmBuscarOrdenCompra()
        {
            InitializeComponent();
            Load += frmBuscarOrdenCompra_Load;
        }

        void frmBuscarOrdenCompra_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
            dtpInit.MaxDate = dtpEnd.DateTime;
        }
        #endregion

        #region Events
        private void txtProveedor_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var frm = new Mantenimientos.frmBuscarProveedor(txtProveedor.Text.Trim(), "Nombre");
            frm.ShowDialog();
            if (!string.IsNullOrEmpty(frm._RazonSocial))
            {
                txtProveedor.Text = frm._RazonSocial;
                txtProveedor.Tag = frm._IdProveedor;
            }
        }


        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string filtro = null;
            if (txtProveedor.Tag != null) filtro = "v_IdProveedor==\"" + txtProveedor.Tag.ToString() + "\"";
            filtro = string.IsNullOrEmpty (filtro) ?  "i_IdEstado==1": filtro + "&&"+ "i_IdEstado==1" ;
            Bind(filtro);
        }

        private void dtpEnd_ValueChanged(object sender, EventArgs e)
        {
            dtpInit.MaxDate = dtpEnd.DateTime;
            if (dtpInit.DateTime > dtpInit.MaxDate)
                dtpInit.DateTime = dtpInit.MaxDate;
        }

        private void grdData_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            End();
        }

        private void grdData_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            End();
        }

        private void txtProveedor_Validated(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtProveedor.Text))
            {
                txtProveedor.Tag = null;
            }
        }
        #endregion

        #region Methods
        private void Bind(string filter)
        {
            OperationResult objOperationResult = new OperationResult();
            var objData = _objOrdenCompraBL.ListarOrdenCompras(ref objOperationResult, dtpInit.DateTime.Date, dtpEnd.DateTime.Date, dtpInit.DateTime.Date, dtpEnd.DateTime.Date, filter);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            grdData.DataSource = objData;
        }
        private void End()
        {
            if (grdData.ActiveRow != null)
            {
                Result = grdData.ActiveRow.GetCellValue("NroOrdenCompra").ToString();
                if (Modal) DialogResult = System.Windows.Forms.DialogResult.OK;
                else Close();
            }
        }
        #endregion

    }
}
