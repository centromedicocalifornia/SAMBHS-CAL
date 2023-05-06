using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.Cobranza.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Security.BL;
using SAMBHS.Windows.WinClient.UI.Reportes.Ventas;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmBandejaAdelanto : Form
    {
        #region Fields
        string Modo;
        string v_IdCliente;

        #region PermisosBotones
        bool _btnAgregar = false;
        bool _btnEditar = false;
        bool _btnEliminar = false;
        #endregion

        #endregion

        #region Construct and Load
        public frmBandejaAdelanto(string _Modo)
        {
            InitializeComponent();
            Modo = _Modo;
        }

        private void frmBandejaAdelanto_Load(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();

            #region ControlAcciones
            var _formActions = new SecurityBL().GetFormAction(ref objOperationResult, Globals.ClientSession.i_CurrentExecutionNodeId, Globals.ClientSession.i_SystemUserId, "frmBandejaAdelanto", Globals.ClientSession.i_RoleId);

            _btnAgregar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaAdelanto_ADD", _formActions);
            _btnEditar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaAdelanto_EDIT", _formActions);
            _btnEliminar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaAdelanto_DELETE", _formActions);

            btnAgregar.Enabled = _btnAgregar;
            #endregion

            this.BackColor = new GlobalFormColors().FormColor;
            dtpFechaInicio.Value = DateTime.Parse("01/" + DateTime.Today.Month.ToString() + "/" + DateTime.Today.Year.ToString());
            dtpFechaInicio.MaxDate = dtpFechaFin.Value;
        }
        #endregion

        #region Methods
        private void BindGrid(string filter)
        {
            var objData = GetData("v_IdAdelanto ASC", filter);

            grdData.DataSource = objData;
            lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", objData.Count);
        }

        private List<adelantoDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = new AdelantoBL().DevuelveListadoAdelantos(ref objOperationResult, pstrSortExpression, pstrFilterExpression, dtpFechaInicio.Value.Date, DateTime.Parse(dtpFechaFin.Value.Day.ToString() + "/" + dtpFechaFin.Value.Month.ToString() + "/" + dtpFechaFin.Value.Year.ToString() + " 23:59"));

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }
        #endregion

        #region Events UI

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (Utils.Windows.HaveFormChild(this, typeof(frmRegistroAdelanto))) return;
            frmRegistroAdelanto frm = new frmRegistroAdelanto("Nuevo","");
            frm.FormClosed += (_, ev) =>
            {
                this.InvokeOnClick(btnBuscar, e);
                Utils.Windows.SelectRowForKeyValue(grdData, "v_IdAdelanto", (_ as frmRegistroAdelanto)._pstrIdAdelanto_Nuevo);
            };
            (this.MdiParent as frmMaster).RegistrarForm(this, frm);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            List<string> Filters = new List<string>();
            if (!string.IsNullOrEmpty(v_IdCliente)) Filters.Add("v_IdCliente==\"" + v_IdCliente + "\"");
            var _strFilterExpression = string.Empty;
            if (Filters.Count > 0)
                _strFilterExpression = string.Join(" && ", Filters);

            this.BindGrid(_strFilterExpression);
            if (!grdData.Rows.Any())
            {
                btnEliminar.Enabled = false;
                btnEditar.Enabled = false;
            }
        }

        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            if (grdData.ActiveRow != null)
            {
                btnEditar.Enabled = _btnEditar == true ? true : false;
                btnEliminar.Enabled = _btnEliminar == true ? true : false; ;
            }
            else
            {
                btnEditar.Enabled = false;
                btnEliminar.Enabled = false;
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow != null)
            {
                if (Utils.Windows.HaveFormChild(this, typeof(frmRegistroAdelanto), true)) return;
                var pstrIdAdelanto = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdAdelanto"].Value.ToString();
                frmRegistroAdelanto frm = new frmRegistroAdelanto("Edicion", pstrIdAdelanto);
                frm.FormClosed += delegate
                {
                    this.InvokeOnClick(btnBuscar, null);
                    Utils.Windows.SelectRowForKeyValue(grdData, "v_IdAdelanto", pstrIdAdelanto);
                };
                (this.MdiParent as frmMaster).RegistrarForm(this, frm);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            OperationResult _objOperationResult = new OperationResult();
            if (UltraMessageBox.Show("¿Está seguro de Eliminar este Adelanto de los registros?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var pstrIdAdelanto = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdAdelanto"].Value.ToString();
                new AdelantoBL().EliminarAdelanto(ref _objOperationResult, pstrIdAdelanto, Globals.ClientSession.GetAsList());
                this.InvokeOnClick(btnBuscar, e);
            }
        }

        private void dtpFechaFin_ValueChanged(object sender, EventArgs e)
        {
            dtpFechaInicio.MaxDate = dtpFechaFin.Value;
        }

        private void txtCliente_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key.Equals("btnEliminar"))
            {
                v_IdCliente = null;
                txtCliente.Clear();
                txtCliente.ButtonsRight[0].Enabled = false;
            }
            else
            {
                OperationResult objOperationResult = new OperationResult();
                Mantenimientos.frmBuscarCliente frm = new Mantenimientos.frmBuscarCliente("VV", txtCliente.Text.Trim());
                frm.ShowDialog();
                if (frm._IdCliente != null)
                {
                    txtCliente.Text = frm._NroDocumento + " - " + frm._RazonSocial;
                    v_IdCliente = frm._IdCliente;
                    txtCliente.ButtonsRight[0].Enabled = true;
                }
            }
        }
        #endregion

    }
}
