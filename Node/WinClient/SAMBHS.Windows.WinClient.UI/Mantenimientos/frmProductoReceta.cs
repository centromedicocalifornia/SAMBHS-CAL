using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.Almacen.BL;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmProductoReceta : Form
    {
        #region Fields
        private System.Timers.Timer _timerConsult;
        private  string  _intTipoReceta ;
        #endregion

        #region Properties
        public List<Infragistics.Win.UltraWinGrid.UltraGridRow> Resultado { private set; get; }
        #endregion

        public frmProductoReceta(string  TipoReceta="I")
        {
            InitializeComponent();
            _intTipoReceta = TipoReceta;
            this.Text = _intTipoReceta =="I"?"Buscar Producto - Insumo":"Buscar Producto - Tributo";
        }

        private void frmProductoReceta_Load(object sender, EventArgs e)
        {
            _timerConsult = new System.Timers.Timer(500);
            _timerConsult.Elapsed += timerConsult_Elapsed;
            BindGrid();
            #region Cancel
            var btnCancel = new Button();
            btnCancel.Click += delegate { this.Close(); };
            this.CancelButton = btnCancel;
            #endregion
        }

        void timerConsult_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _timerConsult.Stop();
            var count = Utils.Windows.FiltrarGrilla(grdData, txtBuscarNombre.Text.Trim());
            Invoke((MethodInvoker)delegate
            {
                lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", count);
            });
        }

        private void txtBuscarNombre_TextChanged(object sender, EventArgs e)
        {
            _timerConsult.Stop();
            _timerConsult.Start();
        }

        private void BindGrid()
        {
            var objOperationResult = new OperationResult();
            List<KeyValueDtoImage> objData = new List<KeyValueDtoImage>();
            if (_intTipoReceta == "I")
            {
                objData = new ProductoBL().GetProductsReceta(ref objOperationResult);
            }
            else
            {
                objData = new ProductoBL().GetProductsRecetaSalida(ref objOperationResult);
            }
            if (objOperationResult.Success == 0)
            {
                MessageBox.Show(objOperationResult.ExceptionMessage, @"Error en la consulta", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                grdData.DataSource = objData;
                grdData.DisplayLayout.Bands[0].Columns["Value1"].PerformAutoResize(Infragistics.Win.UltraWinGrid.PerformAutoSizeType.AllRowsInBand);
            }
        }

        private void grdData_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (grdData.Selected.Rows.Count > 0)
                {
                    Resultado = new List<Infragistics.Win.UltraWinGrid.UltraGridRow>();
                    Resultado.AddRange(grdData.Selected.Rows.Cast<Infragistics.Win.UltraWinGrid.UltraGridRow>());
                    Close();
                }
            }
        }

        private void grdData_DoubleClick(object sender, EventArgs e)
        {
            if (grdData.ActiveRow != null)
            {
                Resultado = new List<Infragistics.Win.UltraWinGrid.UltraGridRow>()
                {
                    grdData.ActiveRow
                };
                Close();
            }
        }
    }
}
