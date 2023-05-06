using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Compra.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmOrdenCompraEstado : Form
    {
        #region Fields
        private readonly OrdenCompraBL _objOrdenCompraBL = new OrdenCompraBL();
        private readonly string _idOrdenCompra;
        #endregion

        #region Init
        public frmOrdenCompraEstado(string idOrdenCompra)
        {
            _idOrdenCompra = idOrdenCompra;
            InitializeComponent();
            Load += frmOrdenCompraEstado_Load;
        }

        void frmOrdenCompraEstado_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
            var objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboList(cboDocumento, "Value2", "Id", new DocumentoBL().ObtenDocumentosParaComboGridCompras(ref objOperationResult), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(cboUnidadMedida, "Value1", "Id", new DatahierarchyBL().GetDataHierarchiesForComboGrid(ref objOperationResult, 17, null), DropDownListAction.Select);
            CargarDetalle(_idOrdenCompra);
        }
        #endregion

        #region Methods
        private void CargarDetalle(string pstringIdCompra)
        {
            var objOperationResult = new OperationResult();
            var ordeDto = _objOrdenCompraBL.ObtenerOrdenCompra(ref objOperationResult, pstringIdCompra);
            if (objOperationResult.Success != 0 && ordeDto != null)
            {
                txtDocumento.Text = string.Concat(ordeDto.v_SerieDocumento, "-", ordeDto.v_CorrelativoDocumento);
                txtProveedor.Text = ordeDto.RazonSocialProveedor;
                txtRUCProveedor.Text = ordeDto.RUCProveedor;
            }
            grdData.DataSource = _objOrdenCompraBL.CargarDetalleWithCompras(ref objOperationResult, _idOrdenCompra);
            foreach (var item in grdData.Rows)
            {
                if (item.GetCellValue("d_CantidadCancelada") == null) item.Cells["d_CantidadCancelada"].Value = 0M;
                var cell = item.Cells["d_TotalFact"];

                if (item.HasChild(true))
                    cell.Value = item.ChildBands[grdData.DisplayLayout.Bands["listaComprasDto"]].Rows.Sum(r => (decimal)r.GetCellValue("d_Cantidad"));
                else cell.Value = 0M;
                item.Cells["d_Resto"].Value = (decimal)item.GetCellValue("d_Cantidad") - ((decimal)cell.Value + (decimal)item.GetCellValue("d_CantidadCancelada"));
                item.Cells["i_RegistroEstado"].Value = false;
            }
            if (objOperationResult.Success == 0)
            {
                UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\n" + objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Events

        private void grdData_AfterExitEditMode(object sender, EventArgs e)
        {
            if (grdData.ActiveCell == null || grdData.ActiveCell.Column.Key != "d_CantidadCancelada") return;
            if (grdData.ActiveRow != null)
            {
                var row = grdData.ActiveRow;
                if (row.GetCellValue("d_CantidadCancelada") == null) row.Cells["d_CantidadCancelada"].Value = 0M;
                row.Cells["d_Resto"].Value = (decimal)row.GetCellValue("d_Cantidad") - ((decimal)row.GetCellValue("d_TotalFact") + (decimal)row.GetCellValue("d_CantidadCancelada"));
                row.Cells["i_RegistroEstado"].Value = true;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            var upDatesItem = grdData.Rows
                .Where(r => r.Cells["i_RegistroEstado"].Value != null && (bool)r.Cells["i_RegistroEstado"].Value)
                .Select(r => new Tuple<string, decimal>(r.Cells["v_IdOrdenCompraDetalle"].Value.ToString(), (decimal)r.Cells["d_CantidadCancelada"].Value));
                
            if(upDatesItem.Any()){
                var opResult = new OperationResult();
                _objOrdenCompraBL.UpdateCantidadCancelada(ref opResult, upDatesItem);
                if(opResult.Success == 0){
                    UltraMessageBox.Show(opResult.ExceptionMessage, "Error", Icono: MessageBoxIcon.Error);
                    return;
                }
                CargarDetalle(_idOrdenCompra);
            }
            UltraMessageBox.Show("Guardado!", "Aviso");
        }

        #endregion
    }
}
