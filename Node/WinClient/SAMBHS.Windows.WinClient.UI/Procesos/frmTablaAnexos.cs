using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using SAMBHS.Venta.BL;
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
    public partial class frmTablaAnexos : Form
    {
        string _Mode;
        ventadetalleanexoDto _objventadetalleanexo = new ventadetalleanexoDto();
        public frmTablaAnexos()
        {
            InitializeComponent();
        }
        public int _IdAnexo =-1;
        public string _Anexo=null;
        private void frmTablaAnexos_Load(object sender, EventArgs e)
        {

            BindGrid();
           
        }

        private void grdData_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
             if (grdData.Rows.Count == 0) return; //se cambio
             if (grdData.ActiveRow != null)
             {
                 
                 if (grdData.ActiveRow.Cells["i_IdVentaDetalleAnexo"].Value != null)
                 {
                     _IdAnexo = int.Parse(grdData.ActiveRow.Cells["i_IdVentaDetalleAnexo"].Value.ToString());
                     _Anexo = grdData.ActiveRow.Cells["v_Anexo"].Value.ToString().Trim();
                 }
                 Close();
             }
        }

      
     

       
        private void BindGrid()
        {
            var objData = GetData();

            grdData.DataSource = objData;
        }

        private BindingList<ventadetalleanexoDto> GetData()
        {
            BindingList<ventadetalleanexoDto> _objData = new BindingList<ventadetalleanexoDto> ();
            OperationResult objOperationResult = new OperationResult();
            _objData =new VentaBL().ObtenerVentaDetallesAnexo(ref  objOperationResult);
            
            if (objOperationResult.Success == 0)
            {
                UltraMessageBox.Show("Hubo un error al cargar Anexos", "Sistema", Icono: MessageBoxIcon.Error);
            }
            
           
            return _objData;
        }

        

      

       
    }
}
