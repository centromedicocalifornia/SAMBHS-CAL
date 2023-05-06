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
using SAMBHS.Almacen.BL;
using SAMBHS.Security.BL;
using Infragistics.Win.UltraWinGrid;
using System.Threading;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmConsultaProductostock : Form
    {
        public int idAlmacen,FormatoC;
        MovimientoBL _objMovimientoBL = new MovimientoBL();
        NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL(); 
        SecurityBL _objSecurityBL = new SecurityBL();
        CancellationTokenSource _cts = new CancellationTokenSource();
        bool _btnBuscar;
        System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
        public frmConsultaProductostock(string cadena)
        {
            InitializeComponent();
        }
        private void frmConsultaProductostock_Load(object sender, EventArgs e)
        {
            myTimer.Tick += OnTimedEvent;
            myTimer.Interval = 500;
            this.BackColor = new GlobalFormColors().FormColor;
            OperationResult objOperationResult = new OperationResult();

            #region ControlAcciones
            var _formActions = _objSecurityBL.GetFormAction(ref objOperationResult, Globals.ClientSession.i_CurrentExecutionNodeId, Globals.ClientSession.i_SystemUserId, "frmConsultaProductostock", Globals.ClientSession.i_RoleId);

            _btnBuscar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmConsultaProductostock_Buscar", _formActions);
            btnBuscar.Enabled = _btnBuscar;

            #endregion
            #region Cargar Combos
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", _objNodeWarehouseBL.ObtenerAlmacenesParaComboAll(ref objOperationResult, ""), DropDownListAction.Select);
            cboAlmacen.Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
            cboFormato.Value = ((int)FormatoCantidad.UnidadMedidaProducto).ToString();
            #endregion

            txtBuscarProducto.Enabled = false;
            btnKardex.Enabled = false;
           
        }

        #region Busqueda

        private void OnTimedEvent(Object myObject, EventArgs myEventArgs)
        {
            myTimer.Stop();
            LabelContador(Utils.Windows.FiltrarGrillaPorColumnas(grdData, txtBuscarProducto.Text, new List<string> { "v_CodInterno", "v_Descripcion" }));
            var x = grdData.Rows.Count();
            //btnExportar.Enabled = grdData.Rows.Count() > 0 ? true : false;
        
         }

        void LabelContador(int Cantidad)
        {
            lblContadorFilas.Text = String.Format("Se encontraron {0} registros", Cantidad);
        }
        private void btnBuscar_Click(object sender, EventArgs e)
        {

            OperationResult objOperationResult = new OperationResult();
            if (Validar.Validate(true, false).IsValid)
            {

                idAlmacen = int.Parse(cboAlmacen.Value.ToString());
                FormatoC = int.Parse(cboFormato.Value.ToString()); 
                
                
                    if (idAlmacen == -1)
                    {
                        BindGrid(chkSoloStockMayor0.Checked, chkSoloStockDiferente0.Checked, int.Parse(cboFormato.Value.ToString()));
                        grdData.Focus();
                        
                    }
                    else
                    {
                        BindGrid(chkSoloStockMayor0.Checked, chkSoloStockDiferente0.Checked, int.Parse(cboFormato.Value.ToString()));
                        grdData.Focus();
                        
                    }

                   
            }

        }
       
        private void BindGrid( bool SoloStockMayor0 ,bool SoloStockDiferente0,int FormatoCantidad)
        {
            List<productoshortDto> objData = new List<productoshortDto> ();
            OperationResult  OperationRes = new OperationResult ();
            OcultarMostrarBuscar(true);
            Cursor.Current = Cursors.WaitCursor;
            Task.Factory.StartNew(() =>
            {

              objData = GetData(ref OperationRes,"v_CodInterno ASC", SoloStockMayor0, SoloStockDiferente0, FormatoCantidad);
            },_cts.Token).ContinueWith(t =>
            {
                 if (_cts.IsCancellationRequested) return;
                OcultarMostrarBuscar(false);
                Cursor.Current = Cursors.Default;
                if (OperationRes.Success == 0)
                {
                    if (!string.IsNullOrEmpty(OperationRes.ExceptionMessage))
                    {
                        UltraMessageBox.Show( OperationRes.ExceptionMessage + "\n\nTARGET: " + OperationRes.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show(OperationRes.ErrorMessage + "\n\nTARGET: " + OperationRes.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    txtBuscarProducto.Enabled = false;
                    btnKardex.Enabled = false;
                    return;
                }
                

            
                grdData.DataSource = objData;
                if (grdData.Rows.Count() > 0)
                {
                    txtBuscarProducto.Enabled = true;
                    btnKardex.Enabled = true;
                    lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", objData.Count);
                    InicializarIndex();
                }
                else
                {
                    txtBuscarProducto.Enabled = false;
                    btnKardex.Enabled = false;
                    lblContadorFilas.Text = string.Format("Se encontraron {0} registros.",0);
                }
            
                //UltraMessageBox.Show("Ocurrió un error al Consulta", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //txtBuscarProducto.Enabled = false;
                //btnKardex.Enabled = false;
                //return;
            }
                 , TaskScheduler.FromCurrentSynchronizationContext());
       
            
        }


        private List<productoshortDto> GetData(ref OperationResult objOperationResult,     string pstrSortExpression,bool SoloStockMayor0, bool SoloStockDiferente0,int Formato)
        {
           
          
            var  _objData = _objMovimientoBL.ListarBusquedaConsultaProductoStock(ref objOperationResult, pstrSortExpression, null, idAlmacen, SoloStockMayor0, SoloStockDiferente0, Formato);
            return _objData;
           
            
        }

     private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Buscando... " + "Stock x Producto"
                : @"Consulta de Stock x Producto";
            pBuscando.Visible = estado;
            btnBuscar.Enabled = !estado;
        }
        #endregion
       
        #region Comportamiento Controles
        private void txtBuscarProducto_KeyDown(object sender, KeyEventArgs e)
        {
            //List<string> Filters = new List<string>();
            //OperationResult objOperationResult = new OperationResult();
            //string celda;
            //bool Encontrado = false;
            //if (e.KeyCode == Keys.Enter)
            //{
            //    if (grdData.Rows.Count() <= 0) return;

            //    string CodBuscar = txtBuscarProducto.Text.Trim().ToUpper();
            //    foreach (UltraGridRow row in grdData.Rows)
            //    {

            //        if (row.Cells["v_CodInterno"].Value != null) // Solo busca código Interno
            //        {
            //            celda = row.Cells["v_CodInterno"].Value.ToString().ToUpper();
            //            if (celda.Contains(CodBuscar))
            //            {
                            
            //                //row.Appearance.ForeColor = Color.Red;
            //                grdData.PerformAction(UltraGridAction.EnterEditMode);
            //                grdData.DisplayLayout.RowScrollRegions[0].FirstRow = row;
            //                row.Activate();
            //                Encontrado = true;
            //                return;
            //            }
            //        }

            //        if (row.Cells["v_Descripcion"].Value != null) // Solo busca Descripcion
            //        {
            //            celda = row.Cells["v_Descripcion"].Value.ToString().ToUpper();
            //            if (celda.Contains(CodBuscar))
            //            {
                          
            //               // row.Appearance.ForeColor = Color.Red;
            //                grdData.PerformAction(UltraGridAction.EnterEditMode);
            //                grdData.DisplayLayout.RowScrollRegions[0].FirstRow = row;
            //                row.Activate();
            //                Encontrado = true;
            //                return;
            //            }
            //        }


            //    }

            //    if (Encontrado == false)
            //    {
            //        grdData.ActiveRow.Selected = false;
            //    }
            //}
        }
        #endregion
        #region Grilla
  

        private void grdData_DoubleClick(object sender, EventArgs e)
        {

            if (grdData.ActiveRow == null || grdData.ActiveRow.Index < 0) return;

            if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoAlmacen"].Value != null)
            {
                string Producto =grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodInterno"].Value.ToString ().Trim () +":"+grdData.Rows[grdData.ActiveRow.Index].Cells["v_Descripcion"].Value.ToString ().Trim ();
                string idProductoDetalle = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value.ToString();
                frmKardexProducto frmKardex = new frmKardexProducto(Producto, idProductoDetalle, idAlmacen,FormatoC );
                frmKardex.ShowDialog();
            }
            else
            {
                return;
            }

        }

        private void InicializarIndex()
        {
            int Index = 0;
            foreach (UltraGridRow Fila in grdData.Rows)
            {

                Fila.Cells["Index"].Value = (Index + 1).ToString("00");
                Index = Index + 1;
            }
        }
        #endregion

        private void grdData_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.Index < 0) return;
            
            if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoAlmacen"].Value != null)
            {
                btnKardex.Enabled = true;
            }
            else
            {
                btnKardex.Enabled = false;
            }
        }

        private void btnKardex_Click(object sender, EventArgs e)
        {
            
            
            
            if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoAlmacen"].Value != null)
            {
                new LoadingClass.PleaseWait(this.Location, "Por favor espere...");
                string Producto = grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodInterno"].Value.ToString().Trim() + ":" + grdData.Rows[grdData.ActiveRow.Index].Cells["v_Descripcion"].Value.ToString().Trim();
                string idProductoDetalle = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value.ToString();
                frmKardexProducto frmKardex = new frmKardexProducto(Producto, idProductoDetalle, idAlmacen, FormatoC);
                frmKardex.ShowDialog();
                btnKardex.Enabled = false;
            }
            else
            {
                return;
            }
        }

        private void frmConsultaProductostock_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {

                case Keys.Escape: this.Close();
                    break;

            }
        }

        private void grdData_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F)
            {
                if (grdData.DisplayLayout.Override.FilterUIType != FilterUIType.FilterRow)
                {
                    grdData.DisplayLayout.Override.FilterUIType = FilterUIType.FilterRow;
                }
                else
                {
                    grdData.DisplayLayout.Override.FilterUIType = FilterUIType.Default;
                }
            }
        }

        private void frmConsultaProductostock_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }

        private void txtBuscarProducto_KeyPress(object sender, KeyPressEventArgs e)
        {
            myTimer.Stop();
            myTimer.Start();
        }
    }
}
