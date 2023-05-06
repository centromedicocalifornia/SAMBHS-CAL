using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.Venta.BL;
using SAMBHS.Common.BE;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmBuscarVendedor : Form
    {
        #region Fields
        private System.Timers.Timer timerConsult;
        private vendedorDto _vendedor;
        #endregion

        #region Properties

        public vendedorDto Vendedor
        {
            get { return _vendedor; }
        }
        
        #endregion

        public frmBuscarVendedor()
        {
            InitializeComponent();
        }

        private void frmBuscarVendedor_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            timerConsult = new System.Timers.Timer(500);
            timerConsult.Elapsed += timerConsult_Elapsed;
            this.BindGrid();
            this.Load -= frmBuscarVendedor_Load;
        }

        void timerConsult_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timerConsult.Stop();
            var count = Utils.Windows.FiltrarGrilla(grdData, txtBuscar.Text.Trim());
            this.Invoke((MethodInvoker)delegate
            {
                lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", count);
            });
        }

        #region Events UI
        private void txtBuscar_ValueChanged(object sender, EventArgs e)
        {
            timerConsult.Stop();
            timerConsult.Start();
        }

        private void grdData_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                ReturnVendedor();
        }

        private void grdData_DoubleClick(object sender, EventArgs e)
        {
            ReturnVendedor();
        }
        #endregion

        #region Methods
        private void BindGrid()
        {
            var objOperationResult = new OperationResult();
            var _objData = new VendedorBL().ObtenerListadoVendedor(ref objOperationResult, null, null);
            if (objOperationResult.Success == 0)
            {
                MessageBox.Show(objOperationResult.ExceptionMessage, "Error en la consulta", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                this.grdData.DataSource = _objData;
                this.grdData.DisplayLayout.Bands[0].Columns["v_CodVendedor"].PerformAutoResize(Infragistics.Win.UltraWinGrid.PerformAutoSizeType.AllRowsInBand);
            }
        }

        private void ReturnVendedor()
        {
            if (grdData.ActiveRow != null)
            {
                this._vendedor = grdData.ActiveRow.ListObject as vendedorDto;
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }
        #endregion
    }
}
