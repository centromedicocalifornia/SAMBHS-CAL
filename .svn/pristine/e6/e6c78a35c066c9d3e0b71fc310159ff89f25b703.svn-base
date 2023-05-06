using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Compra.BL;
using SAMBHS.Contabilidad.BL;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmBuscarConcepto : Form
    {
        string _strFilterExpression;
        ConceptoBL _objConceptoBL = new ConceptoBL();
        public string _IdConcepto,_CodigoConcepto,_NombreConcepto,_pstrCadena;
        
        public frmBuscarConcepto(string cadena)
        {
            InitializeComponent();
            _pstrCadena = cadena;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            List<string> Filters = new List<string>();

            if (!string.IsNullOrEmpty(txtCodigo.Text)) Filters.Add("v_Codigo.Contains(\"" + txtCodigo.Text.Trim().ToUpper() + "\")");
            if (!string.IsNullOrEmpty(txtNombre.Text)) Filters.Add("v_Nombre.Contains(\"" + txtNombre.Text.Trim().ToUpper() + "\")");
            _strFilterExpression = null;

            if (Filters.Count > 0)
            {
                foreach (string item in Filters)
                {
                    _strFilterExpression = _strFilterExpression + item + " && ";
                }
                _strFilterExpression = _strFilterExpression.Substring(0, _strFilterExpression.Length - 4);
            }

            this.BindGrid();
            grdData.Focus();
        }


        private void BindGrid()
        {
            var objData = GetData("v_Nombre ASC", _strFilterExpression);

            grdData.DataSource = objData;
            lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", objData.Count);
        }


        private List<conceptoDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _objConceptoBL.ObtenerListadoConcepto(ref objOperationResult, pstrSortExpression, pstrFilterExpression);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }

        private void frmBuscarConcepto_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            txtNombre.Enabled = false;

            if (_pstrCadena != "")
            {
                
                    txtCodigo.Text = _pstrCadena;
                    btnBuscar_Click(sender, e);
            }

        }

        private void grdData_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (grdData.Rows.Count == 0) return;
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                _IdConcepto = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdConcepto"].Value.ToString();
                _CodigoConcepto = grdData.Rows[grdData.ActiveRow.Index].Cells["v_Codigo"].Value.ToString();
                _NombreConcepto = grdData.Rows[grdData.ActiveRow.Index].Cells["v_Nombre"].Value.ToString();
                this.Close();
            }
        }

        private void grdData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

        }

        private void grdData_DoubleClick(object sender, EventArgs e)
        {
            if (grdData.Rows.Count == 0) return;
            _IdConcepto = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdConcepto"].Value.ToString();
            _CodigoConcepto = grdData.Rows[grdData.ActiveRow.Index].Cells["v_Codigo"].Value.ToString();
            _NombreConcepto = grdData.Rows[grdData.ActiveRow.Index].Cells["v_Nombre"].Value.ToString();
            this.Close();
         
        }

        private void frmBuscarConcepto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

     
    }
}
