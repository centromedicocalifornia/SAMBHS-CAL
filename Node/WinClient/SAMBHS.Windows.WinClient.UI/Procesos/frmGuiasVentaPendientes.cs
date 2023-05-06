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
    public partial class frmGuiasVentaPendientes : Form
    {
        private readonly GuiaRemisionBL _objGuiaRemisionBl = new GuiaRemisionBL();
        private readonly DatahierarchyBL objDatahierarchyBL= new DatahierarchyBL();
        public frmGuiasVentaPendientes(string p)
        {
            InitializeComponent();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
           
            var GuiasPendientesVenta =GetData ();
            if (GuiasPendientesVenta ==null ) 
            {
                return;
            }
            else 
            {

                //GuiasPendientesVenta = GuiasPendientesVenta.GroupBy(x => new { x.AlmacenDestino , x.Detalles}).Select(group => group.First()).ToList();
                grdData.DataSource = GuiasPendientesVenta;
                grdData.DisplayLayout.Bands[0].SortedColumns.Add("AlmacenDestino", true, true);
            }
            lblContadorFilas.Text = @"Se encontraron " + GuiasPendientesVenta.Count + @" registros.";
        }

        private List<guiaremisionDto> GetData()
        {
            OperationResult objOperationResult = new OperationResult();
            var GuiasPendientes = _objGuiaRemisionBl.ListarBusquedaGuiaRemisionPendientes(ref objOperationResult,Globals.ClientSession.i_IdAlmacenPredeterminado  ==1 ? -1: Globals.ClientSession.i_IdAlmacenPredeterminado.Value   , DateTime.Parse ( dtpFechaRegistroDe.Text ), DateTime.Parse(dtpFechaRegistroAl.Text + " 23:59") , int.Parse ( cboEstado.Value.ToString ()));
            if (objOperationResult.Success ==1)
            {
                return GuiasPendientes ;
            }else 
            {
              UltraMessageBox.Show("Error en operación:" + Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
              return null;
            }
        }

        private void frmGuiasVentaPendientes_Load(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList(cboEstado, "Value1", "Id", objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 158, null), DropDownListAction.All);// UTILIZO EL MISMO ESTADO QUE PARA RECIBO HONORARIO DEL DATAHIE
            cboEstado.Value = "-1";
        }

    }
}
