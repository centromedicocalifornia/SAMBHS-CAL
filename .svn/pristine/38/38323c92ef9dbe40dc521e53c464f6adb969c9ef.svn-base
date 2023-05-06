using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Security.BL;
using SAMBHS.Contabilidad.BL;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{

    public partial class frmPlanCuentasConsulta : Form
    {
        AsientosContablesBL _objBL = new AsientosContablesBL();
        public asientocontableDto _objAsientosDto = new asientocontableDto();

        asientocontableDto _asientosDto = new asientocontableDto();
        string strFilterExpression, _Cuenta;
        public string _NroSubCuenta, _NombreCuenta;
        public int  _ValidarCentroCosto;

        public frmPlanCuentasConsulta(string Cuenta)
        {
            InitializeComponent();
            _Cuenta = Cuenta;
        }

        private void frmPlanCuentasConsulta_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            if (_Cuenta != "")
            {
                txtNroCuentaMayor.Text = _Cuenta;
                txtNroCuentaMayor_Leave(sender, e);
            }
        }

        private void BindGrid(string _NroCuenta)
        {
            var objData = GetData(0, null, "v_NroCuenta ASC", strFilterExpression, _NroCuenta);

            grdData.DataSource = objData;
            grdData.DisplayLayout.Bands[0].Columns["v_NroCuenta"].PerformAutoResize(Infragistics.Win.UltraWinGrid.PerformAutoSizeType.AllRowsInBand, Infragistics.Win.UltraWinGrid.AutoResizeColumnWidthOptions.All);
        }

        private List<asientocontableDto> GetData(int pintPageIndex, int? pintPageSize, string pstrSortExpression, string pstrFilterExpression, string _NroCuenta)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _objBL.ObtenPlanDeCuentasPaginadoFiltrado(ref objOperationResult, pintPageIndex, pintPageSize, pstrSortExpression, pstrFilterExpression, _NroCuenta);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }

        private void txtNroCuentaMayor_KeyPress(object sender, KeyPressEventArgs e)
        {


            Utils.Windows.NumeroEnteroUltraTextBox(txtNroCuentaMayor, e);

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                int numero;
                string _NroCuenta;
                if (!string.IsNullOrEmpty(txtNroCuentaMayor.Text))
                {
                    if (IsNumeric(txtNroCuentaMayor.Text) == true)
                    {
                        numero = Convert.ToInt32(txtNroCuentaMayor.Text);
                        txtNroCuentaMayor.Text = string.Format("{0:00}", numero);
                    }
                    else
                    {
                        txtNroCuentaMayor.Text = "";
                    }
                }
                if (txtNroCuentaMayor.TextLength >= 2)
                {

                    if (RevisaNroCuentaMayor() == 0)
                    {
                        _NroCuenta = txtNroCuentaMayor.Text.Substring(0, 2);
                        OperationResult objOperationResult = new OperationResult();
                        _asientosDto = _objBL.ObtenAsientosPorNro(ref objOperationResult, _NroCuenta);
                        if (_asientosDto == null) return;
                        lblNombreCuentaM.Text = _asientosDto.v_NombreCuenta;
                        BindGrid(_NroCuenta);
                        grdData.Enabled = true;
                        grdData.Focus();

                        if (txtNroCuentaMayor.TextLength > 2)
                        {
                            var Fila = grdData.Rows.Where(p => p.Cells["v_NroCuenta"].Value != null && p.Cells["v_NroCuenta"].Value.ToString().Contains(txtNroCuentaMayor.Text)).FirstOrDefault();

                            if (Fila != null)
                            {
                                Fila.Activate();
                            }
                        }
                    }
                    else
                    {
                        UltraMessageBox.Show("El Número de cuenta ingresado no existe en los registros", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                }
            }
        }

        private void grdData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            grdData.DisplayLayout.Bands[0].SortedColumns.Add("v_NroCuenta", false, true);
        }

        private void grdData_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            if (e.Row.Cells["i_Imputable"].Value.ToString() == "0")
            {
                e.Row.Appearance.BackColor = Color.Cornsilk;
                e.Row.Appearance.BackColor2 = Color.Cornsilk;
                e.Row.Appearance.ForeColor = Color.Black;
            }
        }

        private void grdData_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (grdData.ActiveRow == null)
            {
                return;
            }

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_Imputable"].Value.ToString() != "0")
                {
                    _NroSubCuenta = grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroCuenta"].Value.ToString();
                    _NombreCuenta = grdData.Rows[grdData.ActiveRow.Index].Cells["v_NombreCuenta"].Value.ToString();
                    _ValidarCentroCosto = int.Parse ( grdData.Rows[grdData.ActiveRow.Index].Cells["i_CentroCosto"].Value.ToString());
                    this.Close();
                }
            }
        }

        private void grdData_DoubleClick(object sender, EventArgs e)
        {

        }

        private void txtNroCuentaMayor_Leave(object sender, EventArgs e)
        {
            int numero;
            string _NroCuenta;
            if (!string.IsNullOrEmpty(txtNroCuentaMayor.Text))
            {
                if (IsNumeric(txtNroCuentaMayor.Text) == true)
                {
                    numero = Convert.ToInt32(txtNroCuentaMayor.Text);
                    txtNroCuentaMayor.Text = string.Format("{0:00}", numero);
                }
                else
                {
                    txtNroCuentaMayor.Text = "";
                }
            }
            if (txtNroCuentaMayor.TextLength >= 2)
            {

                if (RevisaNroCuentaMayor() == 0)
                {
                    _NroCuenta = txtNroCuentaMayor.Text.Substring(0, 2);

                    OperationResult objOperationResult = new OperationResult();
                    _asientosDto = _objBL.ObtenAsientosPorNro(ref objOperationResult, _NroCuenta);
                    lblNombreCuentaM.Text =_asientosDto ==null ?"":  _asientosDto.v_NombreCuenta;
                    BindGrid(_NroCuenta);
                    grdData.Enabled = true;
                    grdData.Focus();

                    if (txtNroCuentaMayor.TextLength > 2)
                    {
                        var Fila = grdData.Rows.Where(p => p.Cells["v_NroCuenta"].Value != null && p.Cells["v_NroCuenta"].Value.ToString().Contains(txtNroCuentaMayor.Text)).FirstOrDefault();

                        if (Fila != null)
                        {
                            Fila.Activate();
                        }
                    }
                }
                else
                {
                    UltraMessageBox.Show("El Número de cuenta ingresado no existe en los registros", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
        }

        #region Clases/Validaciones
        private void Entrada_Numerica(KeyPressEventArgs e)
        {
            if (e.KeyChar == 8)
            {
                e.Handled = false;
                return;
            }
            bool IsDec = false;
            if (e.KeyChar >= 48 && e.KeyChar <= 57)
                e.Handled = false;
            else if (e.KeyChar == 46)
                e.Handled = (IsDec) ? true : false;
            else
                e.Handled = true;
        }

        private int RevisaNroCuentaMayor()
        {
            OperationResult objOperationResult = new OperationResult();
            var _objCheck = _objBL.CheckByNroCuenta(ref objOperationResult, txtNroCuentaMayor.Text.Trim());
            if (_objCheck.Count == 0)
            {
                txtNroCuentaMayor.Focus();
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public bool IsNumeric(object Expression)
        {
            bool isNum;
            double retNum;
            isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        #endregion

        private void frmPlanCuentasConsulta_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {


                this.Close();
            }
        }

        private void grdData_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            if (grdData.ActiveRow == null) return;
            if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_Imputable"].Value.ToString() != "0")
            {
                _NroSubCuenta = grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroCuenta"].Value.ToString();
                _NombreCuenta = grdData.Rows[grdData.ActiveRow.Index].Cells["v_NombreCuenta"].Value.ToString();
                _ValidarCentroCosto = int.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["i_CentroCosto"].Value.ToString());
                this.Close();
            }
        }

    }
}
