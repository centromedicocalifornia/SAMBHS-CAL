using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.Common.BL;
namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmBuscarDatahierarchy : Form
    {
        DatahierarchyBL _DatahierarchyBL = new DatahierarchyBL();
        datahierarchyDto _datahierarchyDto = new datahierarchyDto();
        SystemParameterBL _systemParameterBL = new SystemParameterBL();
        string _strFilterExpression;
        public string _itemId, _value1, _value2, TipoBase = String.Empty, Orden = null;
        int _IdGrupo;
        public frmBuscarDatahierarchy(int IdGrupo, string Titulo, string pTipoBase = null, string pOrden = null, string pFiltro = null)
        {
            _IdGrupo = IdGrupo;
            InitializeComponent();
            this.Text = Titulo;
            TipoBase = pTipoBase;
            Orden = pOrden;
            _strFilterExpression = pFiltro;

        }

        private void frmBuscarDatahierarchy_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            BindGrid();
            if (_IdGrupo == 29)
            {
                grdData.DisplayLayout.Bands[0].Columns["v_Field"].Header.Caption = "Código Sunat";
                grdData.DisplayLayout.Bands[0].Columns["v_Value2"].Header.Caption = "% Detracción";
                grdData.DisplayLayout.Bands[0].Columns["v_Field"].Hidden = false;
            }
            


        }

        private void BindGrid()
        {
            if (TipoBase == "TISN")
            {
                string OrdenarPor = string.Empty;
                if (Orden == null)
                {
                    OrdenarPor = "i_ItemId";
                }
                else
                {
                    OrdenarPor = Orden;
                }


                var objData = GetDataSystem(OrdenarPor, _strFilterExpression);
                grdData.DataSource = objData;
                lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", objData.Count);

            }
            else
            {
                string OrdenarPor = string.Empty;
                if (Orden == null)
                {
                    OrdenarPor = "i_ItemId";
                }
                else
                {
                    OrdenarPor = Orden;
                }

                var objData = GetDataDataHierarChy(OrdenarPor, _strFilterExpression);
                grdData.DataSource = objData;
                lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", objData.Count);
            }
        }

        private List<datahierarchyDto> GetDataDataHierarChy(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _DatahierarchyBL.GetDataHierarchiesPagedAndFiltered(ref objOperationResult, pstrSortExpression, pstrFilterExpression, _IdGrupo);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return _objData;

        }

        private List<systemparameterDto> GetDataSystem(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _systemParameterBL.GetSystemParametersxGrupo(ref objOperationResult, pstrSortExpression, pstrFilterExpression, 152);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return _objData;

        }


        private void grdDetalle_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            if (_IdGrupo != 29)
            {
                grdData.DisplayLayout.Bands[0].Columns["v_Value2"].Hidden = true;
                grdData.DisplayLayout.Bands[0].Columns["v_Value1"].Width = grdData.DisplayLayout.Bands[0].Columns["v_Value1"].Width + 80;
            }
            if (_IdGrupo == 111)
            {
                grdData.DisplayLayout.Bands[0].Columns["v_Value2"].Hidden = false;
                // grdData.DisplayLayout.Bands[0].Columns["v_Value1"].Width = grdData.DisplayLayout.Bands[0].Columns["v_Value1"].Width + 80;
            }
            if (_IdGrupo == 29)
            {
                grdData.DisplayLayout.Bands[0].Columns["v_Field"].Header.Caption = "Código Sunat";
            }

            if ((TipoBase == "TISN" && _IdGrupo == 152) || (TipoBase == null && _IdGrupo == 103) || (TipoBase == null && _IdGrupo == 31) || (TipoBase == null && _IdGrupo == 111))
            {
                grdData.DisplayLayout.Bands[0].Columns["v_Value2"].Hidden = false;
                grdData.DisplayLayout.Bands[0].Columns["v_Value2"].Header.Caption = "Código";
                grdData.DisplayLayout.Bands[0].Columns["v_Value1"].Hidden = false;

            }
            if ((TipoBase == null && _IdGrupo == 103) || (TipoBase == null && _IdGrupo == 31) || (TipoBase == null && _IdGrupo == 111))
            {

                grdData.DisplayLayout.Bands[0].Columns["i_ItemId"].Hidden = true;

            }

        }

        private void grdData_KeyDown(object sender, KeyEventArgs e)
        {
            if (grdData.Rows.Count == 0) return;



            if (e.KeyCode == Keys.Enter)
            {
                if (TipoBase == "TISN" && _IdGrupo == 152)
                {
                    if (grdData.ActiveRow.Cells["v_Value3"].Value == null || grdData.ActiveRow.Cells["v_Value3"].Value.ToString() == "0")
                    {
                        _itemId = grdData.Rows[grdData.ActiveRow.Index].Cells["i_ParameterId"].Value.ToString();
                        _value1 = grdData.Rows[grdData.ActiveRow.Index].Cells["v_Value1"].Value.ToString();
                        _value2 = grdData.Rows[grdData.ActiveRow.Index].Cells["v_SValue2"].Value.ToString();
                        this.Close();
                    }

                }
                else
                    if (grdData.ActiveRow.Cells["i_Header"].Value == null || grdData.ActiveRow.Cells["i_Header"].Value.ToString() == "0")
                    {
                        _itemId = grdData.Rows[grdData.ActiveRow.Index].Cells["i_ItemId"].Value.ToString();
                        _value1 = grdData.Rows[grdData.ActiveRow.Index].Cells["v_Value1"].Value.ToString();
                        _value2 = grdData.Rows[grdData.ActiveRow.Index].Cells["v_Value2"].Value.ToString();
                        this.Close();
                    }
            }
        }

        private void grdData_DoubleClick(object sender, EventArgs e)
        {
            if (grdData.Rows.Count == 0) return;
            if (_IdGrupo == 103)
            {
                if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_Value2"].Value.ToString().Substring((grdData.Rows[grdData.ActiveRow.Index].Cells["v_Value2"].Value.ToString().Length) - 1, 1) == "0")
                {
                    UltraMessageBox.Show("No se retorna Códigos Genéricos  ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (TipoBase == "TISN" && _IdGrupo == 152)
                {
                    if (grdData.ActiveRow.Cells["v_Value3"].Value.ToString() == "1")
                    {
                        UltraMessageBox.Show("No se retorna Códigos Genéricos  ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                }
            }

            if (TipoBase == "TISN" && _IdGrupo == 152)
            {
                if (grdData.ActiveRow.Cells["v_Value3"].Value == null || grdData.ActiveRow.Cells["v_Value3"].Value.ToString() == "0")
                {
                    _itemId = grdData.Rows[grdData.ActiveRow.Index].Cells["i_ParameterId"].Value.ToString();
                    _value1 = grdData.Rows[grdData.ActiveRow.Index].Cells["v_Value1"].Value.ToString();
                    _value2 = grdData.Rows[grdData.ActiveRow.Index].Cells["v_Value2"].Value.ToString();
                    this.Close();
                }

            }
            else
            {
                if (grdData.ActiveRow.Cells["i_Header"].Value == null || grdData.ActiveRow.Cells["i_Header"].Value.ToString() == "0")
                {
                    _itemId = grdData.Rows[grdData.ActiveRow.Index].Cells["i_ItemId"].Value.ToString();
                    _value1 = grdData.Rows[grdData.ActiveRow.Index].Cells["v_Value1"].Value.ToString();
                    _value2 = grdData.Rows[grdData.ActiveRow.Index].Cells["v_Value2"].Value == null ? null : grdData.Rows[grdData.ActiveRow.Index].Cells["v_Value2"].Value.ToString();
                    this.Close();
                }
            }
        }

        private void frmBuscarDatahierarchy_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            List<string> Filters = new List<string>();

            if (!string.IsNullOrEmpty(txtBuscarNombre.Text)) Filters.Add("v_Value1.Contains(\"" + txtBuscarNombre.Text.Trim().ToUpper() + "\")");
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

        private void txtBuscarNombre_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnBuscar_Click(sender, e);
            }
        }

        private void grdData_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            if (TipoBase == "TISN")
            {
                if (e.Row.Cells["v_Value3"].Value != null && e.Row.Cells["v_Value3"].Value.ToString() == "1")
                {
                    e.Row.Appearance.ForeColor = Color.Black;
                    e.Row.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                    e.Row.Appearance.BackColor = Color.Bisque;
                    e.Row.Appearance.BackColor2 = Color.Bisque;
                }
                else if (e.Row.Cells["v_Value3"].Value == null || e.Row.Cells["v_Value3"].Value.ToString() == "0")
                {
                    e.Row.Appearance.ForeColor = Color.Black;
                    e.Row.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.False;
                    e.Row.Appearance.BackColor = Color.White;
                }

            }
            else
            {


                if (e.Row.Cells["i_Header"].Value != null && e.Row.Cells["i_Header"].Value.ToString() == "1")
                {
                    e.Row.Appearance.ForeColor = Color.Black;
                    e.Row.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
                    e.Row.Appearance.BackColor = Color.Bisque;
                    e.Row.Appearance.BackColor2 = Color.Bisque;
                }
                else if (e.Row.Cells["i_Header"].Value == null || e.Row.Cells["i_Header"].Value.ToString() == "0")
                {
                    e.Row.Appearance.ForeColor = Color.Black;
                    e.Row.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.False;
                    e.Row.Appearance.BackColor = Color.White;
                }
            }
        }

        private void txtBuscarNombre_KeyUp(object sender, KeyEventArgs e)
        {
            LabelContador(Utils.Windows.FiltrarGrilla(grdData, txtBuscarNombre.Text.Trim()));
        }

        void LabelContador(int Cantidad)
        {
            lblContadorFilas.Text = String.Format("Se encontraron {0} registros", Cantidad);
        }
    }
}
