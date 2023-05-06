using SAMBHS.Almacen.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmConfigurarEstablecimientoAlmacen : Form
    {
        bool IsOK = false;

        public frmConfigurarEstablecimientoAlmacen()
        {
            InitializeComponent();
        }

        private void frmConfigurarEstablecimientoAlmacen_Load(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            try
            {
                this.BackColor = new GlobalFormColors().FormColor;
                NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL();
                EstablecimientoBL _objEstablecimientoBL = new EstablecimientoBL();
              //  Utils.Windows.LoadDropDownList(cboAlmacen, "Value1", "Id", _objNodeWarehouseBL.ObtenerAlmacenesParaCombo(ref objOperationResult, null, Globals.ClientSession.i_CurrentExecutionNodeId), DropDownListAction.Select);
                Utils.Windows.LoadDropDownList(cboEstablecimientoPredet, "Value1", "Id", _objEstablecimientoBL.ObtenerEstablecimientosValueDto(ref objOperationResult, null), DropDownListAction.Select);
            }
            catch (Exception ex)
            {
                if (objOperationResult.Success == 0)
                {
                    if (!string.IsNullOrEmpty(objOperationResult.ExceptionMessage))
                    {
                        UltraMessageBox.Show(objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }
                UltraMessageBox.Show(ex.Message);
                this.Close();
            }

            try
            {
                List<string> ColumnasAlmacen = cboAlmacen.Items.Cast<KeyValueDTO>().Select(p => p.Value1).ToList();
                List<string> ColumnasEstablecimiento = cboEstablecimientoPredet.Items.Cast<KeyValueDTO>().Select(p => p.Value1).ToList();

                if (ColumnasAlmacen.Count(p => !p.Contains("--Seleccionar--")) == 1 && ColumnasEstablecimiento.Count(p => !p.Contains("--Seleccionar--")) == 1)
                {
                    int AlmacenPredeterminado = ColumnasAlmacen.FindIndex(p => p != "--Seleccionar--");
                    int EstablecimientoPredeterminado = ColumnasEstablecimiento.FindIndex(p => p != "--Seleccionar--");

                    if (AlmacenPredeterminado != -1) cboAlmacen.SelectedIndex = AlmacenPredeterminado;
                    if (EstablecimientoPredeterminado != -1) cboEstablecimientoPredet.SelectedIndex = AlmacenPredeterminado;

                    if (AlmacenPredeterminado != -1 && EstablecimientoPredeterminado != -1) ultraButton1_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message + '\n' + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
            }
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            if (uvDatos.Validate(true, false).IsValid)
            {
                //Utils.ConfiguraEstablecimientoAlmacenPredeterminado(Globals.ClientSession.i_CurrentExecutionNodeId.ToString(), cboAlmacen.SelectedValue.ToString(), cboEstablecimientoPredet.SelectedValue.ToString());
                int IdAlmacen = int.Parse(cboAlmacen.SelectedValue.ToString());
                int IdEstablecimiento = int.Parse(cboEstablecimientoPredet.SelectedValue.ToString());
                UserConfig.Default.appAlmacenPredeterminado = IdAlmacen;
                UserConfig.Default.appEstablecimientoPredeterminado = IdEstablecimiento;
                UserConfig.Default.Save();

                Globals.ClientSession.i_IdAlmacenPredeterminado = IdAlmacen;
                Globals.ClientSession.i_IdEstablecimiento = IdEstablecimiento;
                IsOK = true;
                this.Close();
            }
        }

        private void frmConfigurarEstablecimientoAlmacen_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!IsOK)
            {
                e.Cancel = true;
            }
        }

        private void ultraButton2_Click(object sender, EventArgs e)
        {
           
        }

        private void cboEstablecimientoPredet_SelectedIndexChanged(object sender, EventArgs e)
        {
            EstablecimientoBL objEstablecimientoBL = new EstablecimientoBL();
            List<KeyValueDTO> x = new List<KeyValueDTO>();
            if (cboEstablecimientoPredet.SelectedValue == null) return;
            x = objEstablecimientoBL.GetAlmacenesXEstablecimiento(int.Parse(cboEstablecimientoPredet.SelectedValue.ToString()));
            Utils.Windows.LoadDropDownList(cboAlmacen, "Value1", "Id", x, DropDownListAction.Select);
        }
    }
}
