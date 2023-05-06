using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Venta.BL;
using Timer = System.Threading.Timer;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmBuscarAnexo : Form
    {
        public Anexo Anexo
        {
            get { return _anexo; }
        }
        private Anexo _anexo = new Anexo();
        private Task _taskTarea;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly ClienteBL _objClienteBl = new ClienteBL();
        readonly System.Windows.Forms.Timer _myTimer = new System.Windows.Forms.Timer();
        public frmBuscarAnexo()
        {
            InitializeComponent();
        }

        private void frmBuscarAnexo_Load(object sender, EventArgs e)
        {
            var objOperationResult = new OperationResult();
            var ds = new DatahierarchyBL().GetDataHierarchiesForCombo(ref objOperationResult, 170, null);
            _myTimer.Tick += OnTimedEvent;
            _myTimer.Interval = 300;
            BackColor = new GlobalFormColors().FormColor;
            Utils.Windows.LoadUltraComboEditorList(cboTipoAnexo, "Value1", "Id", ds, DropDownListAction.Select);
            cboTipoAnexo.Value = "-1";
        }

        private void Buscar(string tipo)
        {
            try
            {
                _taskTarea = Task.Factory.StartNew(() => { GetData(tipo); }, _cts.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GetData(string tipo)
        {
            try
            {
                if (Application.OpenForms["frmBuscarAnexo"] != null)
                {
                    Invoke(new ActivarDesactivarBuscandoDelegado(ActivarDesactivarBuscando), true);
                    var objOperationResult = new OperationResult();

                    var ds = _objClienteBl.BuscarAnexo(ref objOperationResult, tipo);

                    if (objOperationResult.Success == 0)
                    {
                        MessageBox.Show(
                            objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
                            objOperationResult.AdditionalInformation, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (Application.OpenForms["frmBuscarAnexo"] != null)
                    {
                        Invoke(new SetDatasetDelegado(SetDataset), ds);
                    }
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (Application.OpenForms["frmBuscarAnexo"] != null)
                {
                    Invoke(new ActivarDesactivarBuscandoDelegado(ActivarDesactivarBuscando), false);
                }
            }
        }

        private delegate void ActivarDesactivarBuscandoDelegado(bool estado = true);
        private void ActivarDesactivarBuscando(bool estado = true)
        {
            pBuscando.Visible = estado;
            cboTipoAnexo.Enabled = !estado;
        }

        private delegate void SetDatasetDelegado(List<Anexo> ds);
        private void SetDataset(List<Anexo> ds)
        {
            try
            {
                grdData.DataSource = ds;
                lblContador.Text = string.Format("Se encontraron {0} registros",
                    Utils.Windows.FiltrarGrilla(grdData,
                        string.IsNullOrEmpty(txtBuscarNombre.Text.Trim()) ? " " : txtBuscarNombre.Text.Trim()));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }


        private void frmBuscarAnexo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }

        private void grdData_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            if (grdData.ActiveRow == null) return;
            _anexo = (Anexo)grdData.ActiveRow.ListObject;
            DialogResult = DialogResult.OK;
            Close();
        }

        #region EVentos del timer para la busqueda.
        private void OnTimedEvent(object myObject, EventArgs myEventArgs)
        {
            try
            {
                _myTimer.Stop();
                lblContador.Text = string.Format("Se encontraron {0} registros",
                    Utils.Windows.FiltrarGrilla(grdData,
                        string.IsNullOrEmpty(txtBuscarNombre.Text.Trim()) ? " " : txtBuscarNombre.Text.Trim()));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
        
        private void txtBuscarNombre_KeyUp(object sender, KeyEventArgs e)
        {
            _myTimer.Stop();
            _myTimer.Start();
        }
        #endregion

        private void ultraStatusBar1_Click(object sender, EventArgs e)
        {

        }
        
        private void grdData_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter || grdData.ActiveRow == null) return;
            _anexo = (Anexo)grdData.ActiveRow.ListObject;
            DialogResult = DialogResult.OK;
            Close();
        }
        
        private void cboTipoAnexo_ValueChanged(object sender, EventArgs e)
        {
            if (cboTipoAnexo.Value == null || cboTipoAnexo.Value.ToString() == "-1") return;
            if (_taskTarea == null || _taskTarea.IsCompleted)
            {
                var flag = ((KeyValueDTO)cboTipoAnexo.SelectedItem.ListObject).Value2.Trim();
                Buscar(flag);
                txtBuscarNombre.Focus();
            }
        }
    }
}
