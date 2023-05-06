using Infragistics.Win.UltraWinGrid;
using SAMBHS.Almacen.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.Compra.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinGrid.ExcelExport;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmBandejaGuiaRemisionCompras : Form
    {
        private Task _tarea;
        private CancellationTokenSource cts = new CancellationTokenSource();
        GuiaRemisionCompraBL _ObjGuiaRemisionCompraBL = new GuiaRemisionCompraBL();
        MovimientoBL _objMovimientoBL = new MovimientoBL();
        string _strFilterExpression = "", v_IdProveedor = string.Empty;

        public frmBandejaGuiaRemisionCompras(string Parametro)
        {
            InitializeComponent();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (Utils.Windows.HaveFormChild(this, typeof(frmGuiaRemisionCompra))) return;
            frmGuiaRemisionCompra frm = new frmGuiaRemisionCompra("Nuevo", "");
            frm.FormClosed += (_, ev) =>
            {
                BindGrid();
            };
            (this.MdiParent as frmMaster).RegistrarForm(this, frm);
        }

        private void BindGrid()
        {
            var objData = GetData("NroRegistro ASC", _strFilterExpression);

            if (objData != null)
            {
                grdData.DataSource = objData;
            }
        }

        private List<guiaremisioncompraShortDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _ObjGuiaRemisionCompraBL.ListarGuiasRemisionCompra(ref objOperationResult, dtpFechaInicio.Value.Date, DateTime.Parse(dtpFechaFin.Value.Day.ToString() + "/" + dtpFechaFin.Value.Month.ToString() + "/" + dtpFechaFin.Value.Year.ToString() + " 23:59"), _strFilterExpression);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            List<string> Filters = new List<string>();
            OperationResult objOperationResult = new OperationResult();
            _strFilterExpression = null;

            if (v_IdProveedor != string.Empty) Filters.Add("v_IdProveedor==\"" + v_IdProveedor + "\"");
            if (cboMoneda.Value.ToString() != "-1") Filters.Add("Moneda==\"" + (cboMoneda.Value.ToString().Equals("1") ? "S" : "D") + "\"");
            if (Filters.Count > 0)
            {
                foreach (string item in Filters)
                {
                    _strFilterExpression = _strFilterExpression + item + " && ";
                }
                _strFilterExpression = _strFilterExpression.Substring(0, _strFilterExpression.Length - 4);
            }

            this.BindGrid();
        }

        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            btnEditar.Enabled = grdData.ActiveRow != null;
            btnEliminar.Enabled = grdData.ActiveRow != null;
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow) return;

            if (Utils.Windows.HaveFormChild(this, typeof(frmGuiaRemisionCompra), true)) return;
            frmGuiaRemisionCompra frm = new frmGuiaRemisionCompra("Edicion", grdData.ActiveRow.Cells["v_IdGuiaCompra"].Value.ToString());
            (this.MdiParent as frmMaster).RegistrarForm(this, frm);

        }

        private void frmBandejaGuiaRemisionCompras_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            var fecha = DateTime.Parse(string.Format("{0}/{1}/{2}", DateTime.Today.Day, DateTime.Today.Month, Globals.ClientSession.i_Periodo));
            dtpFechaInicio.Value = fecha;
            dtpFechaFin.Value = fecha;
            dtpFechaInicio.MaxDate = dtpFechaFin.Value;
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", Globals.CacheCombosVentaDto.cboMoneda, DropDownListAction.Select);
            cboMoneda.Value = "-1";
            ultraButton1.Visible = Globals.ClientSession.i_SystemUserId == 1;
        }

        private void dtpFechaFin_ValueChanged(object sender, EventArgs e)
        {
            dtpFechaInicio.MaxDate = dtpFechaFin.Value;
        }

        private void txtCliente_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtCliente_Validated(object sender, EventArgs e)
        {
            txtCliente_KeyDown(sender, new KeyEventArgs(Keys.Enter));
        }

        private void frmBandejaGuiaRemisionCompras_Fill_Panel_PaintClient(object sender, PaintEventArgs e)
        {

        }

        private void grdData_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            if (grdData.ActiveRow != null)
            {
                if (btnEditar.Enabled) btnEditar_Click(sender, e);
            }
        }

        private void grdData_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F)
            {
                if (grdData.DisplayLayout.Override.FilterUIType == FilterUIType.Default)
                {
                    grdData.DisplayLayout.Override.FilterUIType = FilterUIType.FilterRow;
                }
                else
                {
                    grdData.DisplayLayout.Override.FilterUIType = FilterUIType.Default;
                }
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsGroupByRow || grdData.ActiveRow.IsFilterRow) return;

            var resp = MessageBox.Show("¿Seguro de Eliminar el Registro?", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resp == DialogResult.No) return;

            var objOperationResult = new OperationResult();
            var id = grdData.ActiveRow.Cells["v_IdGuiaCompra"].Value.ToString();

            _ObjGuiaRemisionCompraBL.EliminarGuiaRemisionCompra(ref objOperationResult, id, Globals.ClientSession.GetAsList());
            if (objOperationResult.Success == 0)
            {
                MessageBox.Show(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage, @"Error en la consulta.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            btnBuscar_Click(sender, e);
        }

        private void txtCliente_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscar")
            {
                OperationResult objOperationResult = new OperationResult();
                Mantenimientos.frmBuscarProveedor frm = new Mantenimientos.frmBuscarProveedor(txtRucProveedor.Text.Trim(), "RUC");
                frm.ShowDialog();
                if (frm._IdProveedor != null)
                {
                    txtRucProveedor.Text = string.Concat(frm._NroDocumento, " - ", frm._RazonSocial);
                    v_IdProveedor = frm._IdProveedor;
                    txtRucProveedor.ButtonsRight["btnEliminar"].Enabled = true;
                }
            }
            else
            {
                txtRucProveedor.Clear();
                v_IdProveedor = string.Empty;
                txtRucProveedor.ButtonsRight["btnEliminar"].Enabled = false;
            }
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            var resp = MessageBox.Show(@"¿Seguro de Proseguir?", @"Sistema", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resp == DialogResult.No) return;
            var objOperationResult = new OperationResult();

            Task.Factory.StartNew(() =>
            {
                Invoke((MethodInvoker)delegate
                {
                    ultraButton1.Enabled = false;
                });
                new ComprasBL().RegenerarIngresosPorCompras(ref objOperationResult);
            }, TaskCreationOptions.LongRunning)
            .ContinueWith(t =>
            {
                if (!IsDisposed)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        ultraButton1.Enabled = true;
                    });
                }

                if (objOperationResult.Success == 0)
                {
                    MessageBox.Show(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage,
                        @"Error en la operación.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                    MessageBox.Show("operación completada!");

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void chkFiltroPersonalizado_CheckedChanged(object sender, EventArgs e)
        {
            Utils.Windows.MostrarOcultarFiltrosGrilla(grdData);
        }

        private void chkBandejaAgrupable_CheckedChanged(object sender, EventArgs e)
        {
            Utils.Windows.HacerGrillaAgrupable(grdData, chkBandejaAgrupable.Checked);
        }

        private void btnExportarBandeja_Click(object sender, EventArgs e)
        {
            if (!grdData.Rows.Any() || (_tarea != null && !_tarea.IsCompleted)) return;

            const string dummyFileName = "Bandeja Ventas";
            UltraGridExcelExporter ultraGridExcelExporter1 = new UltraGridExcelExporter();
            SaveFileDialog sf = new SaveFileDialog
            {
                DefaultExt = "xlsx",
                Filter = @"xlsx files (*.xlsx)|*.xlsx",
                FileName = dummyFileName
            };

            if (sf.ShowDialog() != DialogResult.OK) return;
            btnExportarBandeja.Appearance.Image = Resource.loadingfinal1;

            _tarea = Task.Factory.StartNew(() => { ultraGridExcelExporter1.Export(grdData, sf.FileName); }, cts.Token)
                                 .ContinueWith(t => ActualizarLabel("Bandeja Exportada a Excel."), TaskScheduler.FromCurrentSynchronizationContext());
        }
        private void ActualizarLabel(string Texto)
        {
            lblDocumentoExportado.Text = Texto;
            btnExportarBandeja.Enabled = false;
            btnExportarBandeja.Appearance.Image = Resource.accept;
        }
    }
}
