using System;
using System.Linq;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    using Common.Resource;
    using Almacen.BL;
    using Common.BE;
    using CommonWIN.BL;

    public partial class frmProductoInventario : Form
    {
        #region Fields
        private readonly ProductoInventarioBL _inventarioBl;
        private readonly ProductoBL _productoBl;
        #endregion

        #region Init
        public frmProductoInventario(string arg)
        {
            InitializeComponent();
            _inventarioBl = new ProductoInventarioBL();
            _productoBl = new ProductoBL();
            Load += frmProductoInventario_Load;
            dtpFechaFin.ValueChanged += dtpFechaFin_ValueChanged;
            btnBuscar.Click += btnBuscar_Click;
            btnExcel.Click += btnExcel_Click;
            txtCodigo.KeyDown += txtCodigo_KeyDown;
        }

        private void frmProductoInventario_Load(object sender, EventArgs e)
        {
            var objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList(cboEstablecimiento, "Value1", "Id", new EstablecimientoBL().ObtenerEstablecimientosValueDto(ref objOperationResult, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", new NodeWarehouseBL().ObtenerAlmacenesParaCombo(ref objOperationResult, null, Globals.ClientSession.i_IdEstablecimiento ?? 1), DropDownListAction.Select);
            cboEstablecimiento.Value = Globals.ClientSession.i_IdEstablecimiento.ToString();
            cboAlmacen.Value = Globals.ClientSession.i_IdAlmacenPredeterminado.HasValue ? Globals.ClientSession.i_IdAlmacenPredeterminado.Value.ToString() : "-1";
            cboEstablecimiento.Enabled = false;
            dtpFechaInicio.MaxDate = dtpFechaInicio.DateTime;
        }
        #endregion

        #region Events
        private void dtpFechaFin_ValueChanged(object sender, EventArgs e)
        {
            dtpFechaInicio.MaxDate = dtpFechaFin.DateTime;
            if(dtpFechaInicio.DateTime > dtpFechaInicio.MaxDate)
                dtpFechaInicio.DateTime = dtpFechaInicio.MaxDate;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            var objResult = new OperationResult();
            var items = _inventarioBl.GetData(ref objResult, 
                            dtpFechaInicio.DateTime.Date,
                            dtpFechaFin.DateTime.Date, 
                            int.Parse(cboEstablecimiento.Value.ToString()), 
                            int.Parse(cboAlmacen.Value.ToString()), null);
            if (objResult.Success == 0)
            {
                UltraMessageBox.Show(objResult.ExceptionMessage, Icono: MessageBoxIcon.Error);
            }
            grdData.DataSource = items;
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            try
            {
                var dialog = new SaveFileDialog
                {
                    Filter = @"Excel Files (*.xlsx)|*.xlsx|Todos los Archivos(*.*)|*.*",
                    FileName = "Inventario"
                };
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    gridExcelExport.Export(grdData, dialog.FileName);
                    System.Diagnostics.Process.Start(dialog.FileName);
                    UltraMessageBox.Show("Exportación Finalizada", "Sistema");
                }
            }
            catch (Exception er)
            {
                UltraMessageBox.Show(er.Message, "Error", Icono: MessageBoxIcon.Error);
            }
        }

        private void txtCodigo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter || string.IsNullOrEmpty(txtCodigo.Text)) return;
            var item = grdData.Rows.FirstOrDefault(r => (string)r.GetCellValue("Codigo") == txtCodigo.Text.Trim());
            decimal cantSum;
            if(!decimal.TryParse(txtCantidad.Value.ToString(), out cantSum) || cantSum == 0)
            {
                UltraMessageBox.Show("Cantidad Invalida!", Icono: MessageBoxIcon.Error);
                return;
            }
            var objResult = new OperationResult();
            productoinventarioDto dto;
            if (item != null)
            {
                //item.Cells["d_Cantidad"].Value = (decimal)item.GetCellValue("d_Cantidad") + cantSum;
                dto = (productoinventarioDto)item.ListObject;
                dto.d_Cantidad = cantSum;
                //_inventarioBl.Update(ref objResult, dto);
            }
            else
            {
                var prod = _productoBl.ObtenerProductoCodigo(ref objResult, txtCodigo.Text);
                if(objResult.Success == 0 || prod == null || prod.i_Eliminado == 1)
                {
                    UltraMessageBox.Show("No se pudo encontrar el producto" + Environment.NewLine + objResult.ExceptionMessage, Icono: MessageBoxIcon.Error);
                    return;
                }
                dto = new productoinventarioDto
                {
                    v_IdProducto = prod.v_IdProducto,
                    i_IdEstablecimiento = int.Parse(cboEstablecimiento.Value.ToString()),
                    i_IdAlmacen = int.Parse(cboAlmacen.Value.ToString()),
                    d_Cantidad = cantSum,
                    //t_Fecha = DateTime.Now
                };
                //item.Cells["d_Cantidad"].Value = cantSum;

            }
            dto.t_Fecha =DateTime.Parse ( dtpFechaFin.Value.ToString()); //comente 2017
            _inventarioBl.Add(ref objResult, dto);
            if (objResult.Success == 0)
                UltraMessageBox.Show("Ocurrio un Error" + Environment.NewLine + objResult.ExceptionMessage, Icono: MessageBoxIcon.Error);
            else
            {
                dtpFechaFin.DateTime = DateTime.Parse(dtpFechaFin.Value.ToString());  // DateTime.Now; // comente 2017
                InvokeOnClick(btnBuscar, e);    
            }
            txtCodigo.Clear();
        }
        #endregion

        private void btnBuscar_Click_1(object sender, EventArgs e)
        {

        }
    }
}
