using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Compra.BL;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Venta.BL;
using SAMBHS.Almacen.BL ;
using Infragistics.Win.UltraWinGrid.ExcelExport;
using System.Threading;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmBuscarPedido : Form
    {
        public string _strFilterExpression, pstrIdPedido;
        private CancellationTokenSource cts = new CancellationTokenSource();
        private Task _tarea;
        PedidoBL objPedidoBL = new PedidoBL();
        MovimientoBL objMovimientoBL = new MovimientoBL();
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        List<GridKeyValueDTO> _ListadoComboDocumentos = new List<GridKeyValueDTO>();
        ClienteBL _objClienteBL = new ClienteBL();
        string v_IdCliente = string.Empty;

        public frmBuscarPedido(string cadena)
        {
            InitializeComponent();
        }
        private void frmBandejaPedido_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            OperationResult objOperationResult = new OperationResult();
            #region CargarCombos
            _ListadoComboDocumentos = _objDocumentoBL.ObtenDocumentosPedidosParaComboGrid(ref objOperationResult);
            Utils.Windows.LoadUltraComboList(cboDocumento, "Value2", "Id", _ListadoComboDocumentos, DropDownListAction.Select);
            cboDocumento.Value= "430";
            #endregion
           // dtpFechaInicio.Value = DateTime.Parse("01/" + DateTime.Today.Month.ToString() + "/" + DateTime.Today.Year.ToString());
            dtpFechaInicio.Value = DateTime.Now;
            dtpFechaInicio.MaxDate = dtpFechaFin.Value;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            List<string> Filters = new List<string>();
            OperationResult objOperationResult = new OperationResult();
            if (txtSeriePedido.Text != "") Filters.Add("v_SerieDocumento==\"" + txtSeriePedido.Text + "\"");
            if (txtCorrelativoPedido.Text != "") Filters.Add("v_CorrelativoDocumento==\"" + txtCorrelativoPedido.Text + "\"");
            if (v_IdCliente != string.Empty)
            {
                Filters.Add("v_IdCliente==\"" + v_IdCliente + "\"");
            }
            
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
        }

        private void BindGrid()
        {
            var objData = GetData("NumeroPedido ASC", _strFilterExpression);
            grdData.DataSource = objData;
            lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", objData.Count);
        }
       

        private List<pedidoDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();

            var _objData = objPedidoBL.ListarPedidosPendientes(ref objOperationResult, pstrSortExpression, pstrFilterExpression, dtpFechaInicio.Value.Date, DateTime.Parse(dtpFechaFin.Value.Day.ToString() + "/" + dtpFechaFin.Value.Month.ToString() + "/" + dtpFechaFin.Value.Year.ToString() + " 23:59"));


            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }

        private void grdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            grdData.DisplayLayout.Bands[0].Columns["i_IdEstado"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            frmPedido frmPedido= new frmPedido("Nuevo", "");
            frmPedido.ShowDialog();
            BindGrid();
            MantenerSeleccion(frmPedido._pstrIdMovimiento_Nuevo);
        }
        private void MantenerSeleccion(string ValorSeleccionado)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in grdData.Rows)
            {
                if (row.Cells["v_IdPedido"].Text == ValorSeleccionado)
                {
                    grdData.ActiveRow = row;
                    grdData.ActiveRow.Selected = true;
                    break;
                }
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow != null)
            {
                pstrIdPedido = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdPedido"].Value.ToString();
                frmPedido frmPedido = new frmPedido("Edicion", pstrIdPedido);
                frmPedido.ShowDialog();
                BindGrid();
                MantenerSeleccion(pstrIdPedido);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            OperationResult _objOperationResult = new OperationResult();
            if (UltraMessageBox.Show("¿Está seguro de Eliminar esta guia de remisión de los registros?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                pstrIdPedido = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdPedido"].Value.ToString();
                objPedidoBL.EliminarPedido (ref _objOperationResult, pstrIdPedido, Globals.ClientSession.GetAsList());
                btnBuscar_Click(sender, e);
            }
        }

        private void grdData_MouseDown(object sender, MouseEventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow) return;
            
            Point point = new System.Drawing.Point(e.X, e.Y);
            Infragistics.Win.UIElement uiElement = ((Infragistics.Win.UltraWinGrid.UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

            if (uiElement == null || uiElement.Parent == null) return;

            Infragistics.Win.UltraWinGrid.UltraGridRow row = (Infragistics.Win.UltraWinGrid.UltraGridRow)uiElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridRow));

            if (row == null)
            {
                //btnEditar.Enabled = false;
                //btnEliminar.Enabled = false;
                btnGenerarComprobante.Enabled = false;

            }
            else
            {
                //btnEditar.Enabled = true;
                //btnEliminar.Enabled = true;
                btnGenerarComprobante.Enabled = true;
            }
        }

        private void btnGenerarComprobante_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow != null)
            {
                //string IdPedido = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdPedido"].Value.ToString();
                //frmRegistroVentaRapida frm = new frmRegistroVentaRapida("Nuevo", IdPedido);
                //frm.ShowDialog();
            }
        }

       
        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }
        
        private void txtSeriePedido_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtSeriePedido, e);
        }

        private void txtCorrelativoPedido_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCorrelativoPedido, e);
        }

        private void txtSeriePedido_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtSeriePedido, "{0:0000}");
        }

        private void txtCorrelativoPedido_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtCorrelativoPedido, "{0:00000000}");
        }
        
        private void cboDocumento_AfterDropDown(object sender, EventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboDocumento.Rows)
            {
                if (cboDocumento.Value.ToString () == "-1") cboDocumento.Text = string.Empty;
                bool filterRow = true;
                foreach (UltraGridColumn column in cboDocumento.DisplayLayout.Bands[0].Columns)
                {
                    if (column.IsVisibleInLayout)
                    {
                        if (row.Cells[column].Text.Contains(cboDocumento.Text.ToUpper()))
                        {
                            filterRow = false;
                            break;
                        }
                    }
                }
                if (filterRow)
                {
                    row.Hidden = true;
                }
                else
                {
                    row.Hidden = false;
                }

            }
        }

        private void cboDocumento_Leave(object sender, EventArgs e)
        {
            if (cboDocumento.Text.Trim() == "")
            {
                cboDocumento.Value = "-1";
            }
            else
            {
                var x = _ListadoComboDocumentos.Find(p => p.Id == cboDocumento.Value | p.Id == cboDocumento.Text);
                if (x == null)
                {
                    cboDocumento.Value = "-1";
                }
            }
        }

        private void btnGenerarComprobante_Click_1(object sender, EventArgs e)
        {
            if (grdData.ActiveRow != null)
            {
                string mensaje;
                pstrIdPedido = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdPedido"].Value.ToString();
                if (PedidoBL.PedidoAunExiste(pstrIdPedido, out mensaje))
                {
                    frmPedido frmPedido = new frmPedido("Cobranza", pstrIdPedido);
                    frmPedido.ShowDialog();
                    BindGrid();
                    MantenerSeleccion(pstrIdPedido);
                }
                else
                    MessageBox.Show(mensaje, @"Error.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
               
            }
        }

        private void dtpFechaFin_ValueChanged(object sender, EventArgs e)
        {
            dtpFechaInicio.MaxDate = dtpFechaFin.Value;
        }

        private void ultraTextEditor1_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

        }

        void BuscarCliente(string Nombre)
        {
            if (!string.IsNullOrEmpty(Nombre))
            {
                UltraGridRow Fila = grdData.Rows.Where(p => p.Cells["Cliente"].Value.ToString().ToLower().Contains(Nombre.Trim().ToLower())).FirstOrDefault();
                if (Fila != null) Fila.Activate();
            }
        }

        private void frmBuscarPedido_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnBuscar_Click(sender, e);
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

        private void txtCliente_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscarCliente")
            {
                OperationResult objOperationResult = new OperationResult();
                Mantenimientos.frmBuscarCliente frm = new Mantenimientos.frmBuscarCliente("VV", txtCliente.Text.Trim());
                frm.ShowDialog();

                if (frm._IdCliente != null)
                {

                   
                    txtCliente.Text = frm._NroDocumento;
                    txtRazonSocial.Text = frm._RazonSocial;
                    v_IdCliente = frm._IdCliente;
                }
                else
                {
                  
                }
            }
        }

        private void txtCliente_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCliente, e);
        }

        private void txtCliente_Validated(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (txtCliente.Text.Trim() != string.Empty)
            {
                var Cliente = _objClienteBL.ObtenerClienteCodigoBandejas(ref objOperationResult, txtCliente.Text.Trim(), "C");
                if (Cliente != null)
                {
                    txtRazonSocial.Text = (Cliente.v_ApePaterno + " " + Cliente.v_ApeMaterno.Trim() + " " + Cliente.v_PrimerNombre.Trim() + " " + Cliente.v_SegundoNombre.Trim() + " " + Cliente.v_RazonSocial.Trim()).Trim();
                    v_IdCliente = Cliente.v_IdCliente;
                }
                else
                {
                    txtRazonSocial.Clear();
                    v_IdCliente = "-1";
                }

            }
            else
            {

                txtRazonSocial.Clear();
                v_IdCliente = string.Empty;
            }
        }

        private void grdData_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            

            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow) return;

            btnGenerarComprobante_Click_1(sender, e);
        }

        private void btnExportarBandeja_Click(object sender, EventArgs e)
        {
            if (!grdData.Rows.Any() || (_tarea != null && !_tarea.IsCompleted)) return;

            const string dummyFileName = "Consulta de Pedidos";
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

        private void chkFiltroPersonalizado_CheckedChanged(object sender, EventArgs e)
        {
            Utils.Windows.MostrarOcultarFiltrosGrilla(grdData);
        }

        private void chkBandejaAgrupable_CheckedChanged(object sender, EventArgs e)
        {
            Utils.Windows.HacerGrillaAgrupable(grdData, chkBandejaAgrupable.Checked);
        }

       
    }
}
