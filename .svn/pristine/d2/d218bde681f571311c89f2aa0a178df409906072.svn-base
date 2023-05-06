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
using Infragistics.Win.UltraWinMaskedEdit;
using LoadingClass;


namespace SAMBHS.Windows.WinClient.UI.Requerimientos.Manguifajas
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
            FormatoDecimalesGrilla((int)Globals.ClientSession.i_CantidadDecimales, (int)Globals.ClientSession.i_PrecioDecimales);
            RecalcularStock();
        }

        private void RecalcularStock()
        {
            if (Globals.ActualizadoStocks) return;
            var op = new OperationResult();
            pbRecalculandoStock.Visible = true;
            var objStockBl = new StockSeparacionBl();
            objStockBl.Terminado += delegate { pbRecalculandoStock.Visible = false; };
            objStockBl.IniciarProceso(ref op, Globals.ClientSession.i_Periodo.ToString(), Globals.ClientSession.i_IdAlmacenPredeterminado ?? -1);
        }

        private void FormatoDecimalesGrilla(int DecimalesCantidad, int DecimalesPrecio)
        {
            string FormatoCantidad, FormatoPrecio;
            UltraGridColumn _CantidadActual= this.grdData.DisplayLayout.Bands[0].Columns["stockActual"];
            _CantidadActual.MaskDataMode = MaskMode.IncludeLiterals;
            _CantidadActual.MaskDisplayMode = MaskMode.IncludeLiterals;



            UltraGridColumn _CantidadDisponible= this.grdData.DisplayLayout.Bands[0].Columns["StockDisponible"];
            _CantidadDisponible.MaskDataMode = MaskMode.IncludeLiterals;
            _CantidadDisponible.MaskDisplayMode = MaskMode.IncludeLiterals;

            UltraGridColumn _CantidadSeparacion= this.grdData.DisplayLayout.Bands[0].Columns["d_separacion"];
            _CantidadSeparacion.MaskDataMode = MaskMode.IncludeLiterals;
            _CantidadSeparacion.MaskDisplayMode = MaskMode.IncludeLiterals;

            UltraGridColumn _Costo= this.grdData.DisplayLayout.Bands[0].Columns["d_Costo"];
            _Costo.MaskDataMode = MaskMode.IncludeLiterals;
            _Costo.MaskDisplayMode = MaskMode.IncludeLiterals;

            UltraGridColumn _PrecioVenta = this.grdData.DisplayLayout.Bands[0].Columns["PrecioVenta"];
            _PrecioVenta.MaskDataMode = MaskMode.IncludeLiterals;
            _PrecioVenta.MaskDisplayMode = MaskMode.IncludeLiterals;



            if (DecimalesCantidad > 0)
            {
                string sharp = "n";
                FormatoCantidad = "-nnnnnnnn.";
                for (int i = 0; i < DecimalesCantidad; i++)
                {
                    FormatoCantidad = FormatoCantidad + sharp;
                }
            }
            else
            {
                FormatoCantidad = "-nnnnnnnn";
            }

            if (DecimalesPrecio > 0)
            {
                string sharp = "n";
                FormatoPrecio = "-nnnnnnnn.";
                for (int i = 0; i < DecimalesPrecio; i++)
                {
                    FormatoPrecio = FormatoPrecio + sharp;
                }
            }
            else
            {
                FormatoPrecio = "-nnnnnnnn";
            }

            _CantidadActual.MaskInput = FormatoCantidad;
            _CantidadDisponible.MaskInput = FormatoCantidad;
            _CantidadSeparacion.MaskInput = FormatoCantidad;
            _Costo.MaskInput = FormatoPrecio;
            _PrecioVenta.MaskInput = FormatoPrecio;
            
        }
        #region Busqueda
        private void btnBuscar_Click(object sender, EventArgs e)
        {

            OperationResult objOperationResult = new OperationResult();
            if (Validar.Validate(true, false).IsValid)
            {

                idAlmacen = int.Parse(cboAlmacen.Value.ToString());
                FormatoC = int.Parse(cboFormato.Value.ToString()); 
                
                
                    if (idAlmacen == -1)
                    {
                        BindGrid(chkSoloStockMayor0.Checked, chkSoloStockDiferente0.Checked, int.Parse(cboFormato.Value.ToString()),chkTodosProductos.Checked);
                        grdData.Focus();
                        
                        
                    }
                    else
                    {
                        BindGrid(chkSoloStockMayor0.Checked, chkSoloStockDiferente0.Checked, int.Parse(cboFormato.Value.ToString()), chkTodosProductos.Checked);
                        grdData.Focus();
                        
                    }

                    var filas = grdData.Rows.Count();
                   

                   
            }

        }
       
        private void BindGrid ( bool SoloStockMayor0 ,bool SoloStockDiferente0,int FormatoCantidad, bool TodosProductos)
        {
            List<productoshortDto> objData = new List<productoshortDto> ();
            OperationResult  OperationRes = new OperationResult ();
            OcultarMostrarBuscar(true);
            Cursor.Current = Cursors.WaitCursor;
            Task.Factory.StartNew(() =>
            {

                objData = GetData(ref OperationRes, "v_CodInterno ASC", SoloStockMayor0, SoloStockDiferente0, FormatoCantidad, TodosProductos);
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
                grdData.DataSource = objData;
                if (grdData.Rows.Count() > 0)
                {
                    txtBuscarProducto.Enabled = true;
                    btnKardex.Enabled = true;
                    lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", objData.Count);
                    InicializarIndex();
                    btnExportar.Enabled = true;
                }
                else
                {
                    txtBuscarProducto.Enabled = false;
                    btnKardex.Enabled = false;
                    lblContadorFilas.Text = string.Format("Se encontraron {0} registros.",0);
                    btnExportar.Enabled = false;
                }
            
                //UltraMessageBox.Show("Ocurrió un error al Consulta", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //txtBuscarProducto.Enabled = false;
                //btnKardex.Enabled = false;
                //return;
            }
                 , TaskScheduler.FromCurrentSynchronizationContext());
       
            
        }


        private List<productoshortDto> GetData(ref OperationResult objOperationResult,     string pstrSortExpression,bool SoloStockMayor0, bool SoloStockDiferente0,int Formato,bool TodosProductos)
        {


            var _objData = _objMovimientoBL.ListarBusquedaConsultaProductoStockManguifajas(ref objOperationResult, pstrSortExpression, idAlmacen, SoloStockMayor0, SoloStockDiferente0, Formato, TodosProductos);
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
        
        #endregion
        #region Grilla
      
        private void grdData_DoubleClick(object sender, EventArgs e)
        {

            if (grdData.ActiveRow == null || grdData.ActiveRow.Index < 0) return;

            if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoAlmacen"].Value != null)
            {
                string Producto =grdData.Rows[grdData.ActiveRow.Index].Cells["v_Descripcion"].Value.ToString ().Trim ();
                string CodigoProducto = grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodInterno"].Value.ToString().Trim();
                string idProductoDetalle = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value.ToString();
                frmKardexProducto frmKardex = new frmKardexProducto(CodigoProducto , Producto, idProductoDetalle, idAlmacen, FormatoC, decimal.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["stockActual"].Value.ToString()),"");
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
            Index = 0;
            foreach (UltraGridRow Fila in grdData.Rows )
            {

                Fila.Cells["Index"].Value = (Index + 1).ToString("00");
                Index = Index + 1;
            }
        }
        #endregion

        private void grdData_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.Index < 0) return;
            
        }

        private void btnKardex_Click(object sender, EventArgs e)
        {
            
           
                new LoadingClass.PleaseWait(this.Location, "Por favor espere...");
                string Producto =grdData.Rows[grdData.ActiveRow.Index].Cells["v_Descripcion"].Value.ToString().Trim();
                string CodigoProducto = grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodInterno"].Value.ToString().Trim();
                string idProductoDetalle = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value.ToString();
                SAMBHS.Windows.WinClient.UI.Requerimientos.Manguifajas.frmKardexProducto frmKardex = new SAMBHS.Windows.WinClient.UI.Requerimientos.Manguifajas.frmKardexProducto(CodigoProducto, Producto, idProductoDetalle, idAlmacen, FormatoC, grdData.Rows[grdData.ActiveRow.Index].Cells["stockActual"].Value == null ? 0 : decimal.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["stockActual"].Value.ToString()), grdData.Rows[grdData.ActiveRow.Index].Cells["EmpaqueUnidadMedidaFinal"].Value.ToString ());
                frmKardex.ShowDialog();
           
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



        private void btnExportar_Click(object sender, EventArgs e)
        {
            try
            {
                // Prepare a dummy string, thos would appear in the dialog
                string dummyFileName = "KARDEX";
                SaveFileDialog sf = new SaveFileDialog();
                sf.DefaultExt = "xlsx";
                sf.Filter = "xlsx files (*.xlsx)|*.xlsx";
                // Feed the dummy name to the save dialog
                sf.FileName = dummyFileName;


                if (sf.ShowDialog() == DialogResult.OK)
                {
                    using (new PleaseWait(this.Location, "Exportando excel..."))
                    {
                        ultraGridExcelExporter.Export(grdData, sf.FileName);

                    }
                    UltraMessageBox.Show("Exportación Finalizada", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message);
            }
        }


        #region Busqueda KeyPress

        private void OnTimedEvent(Object myObject, EventArgs myEventArgs)
        {
            myTimer.Stop();
            LabelContador(Utils.Windows.FiltrarGrillaPorColumnas(grdData, txtBuscarProducto.Text, new List<string> { "v_CodInterno", "v_Descripcion" }));
            var x = grdData.Rows.Count();
            btnExportar.Enabled = grdData.Rows.Count() > 0 ? true : false;
        }
        void LabelContador(int Cantidad)
        {
            lblContadorFilas.Text = String.Format("Se encontraron {0} registros", Cantidad);
        }

        private void txtBuscarProducto_KeyPress(object sender, KeyPressEventArgs e)
        {
            myTimer.Stop();
            myTimer.Start();
        }

        #endregion
    }
}
