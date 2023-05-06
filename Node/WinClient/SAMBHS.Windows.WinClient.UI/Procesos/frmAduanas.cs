using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Almacen.BL;



namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmAduanas : Form
       
    {
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        public string _codigoAduana ;
        
        
        public frmAduanas()
        {
            InitializeComponent();
        }

        private void frmAduanas_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            
            BindGrid();
        }

        private void BindGrid()
        {
            var objData = GetData("v_Value1 ASC", "");
            grdData.DataSource = objData;

        }

        private List<GridKeyValueDTO> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();

            var _objData = _objDatahierarchyBL.GetDataHierarchiesForComboGrid(ref objOperationResult,53,"");


            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

             return _objData;
        }

        private void grdData_DoubleClick(object sender, EventArgs e)
        {
            if (grdData.ActiveRow != null)
            {
                _codigoAduana = grdData.Rows[grdData.ActiveRow.Index].Cells["Id"].Value.ToString();
                this.Close();
            }
        }

        private void frmAduanas_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {

                case Keys.Escape: this.Close();
                    break;

            }
        }

    }
}
