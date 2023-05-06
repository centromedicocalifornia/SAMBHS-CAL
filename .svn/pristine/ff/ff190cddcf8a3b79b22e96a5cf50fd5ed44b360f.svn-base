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
using SAMBHS.Venta.BL;
using Infragistics.Win.UltraWinGrid.ExcelExport;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmConsultaVentasExportacion : Form
    {
        //public int idAlmacen,FormatoC;
        //MovimientoBL _objMovimientoBL = new MovimientoBL();
        //NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL(); 
        private Task _tarea;
        SecurityBL _objSecurityBL = new SecurityBL();
        CancellationTokenSource _cts = new CancellationTokenSource();
       
        public frmConsultaVentasExportacion(string cadena)
        {
            InitializeComponent();
        }
        private void frmConsultaVentasExportacion_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            
            OperationResult objOperationResult = new OperationResult();

            //#region ControlAcciones
            //var _formActions = _objSecurityBL.GetFormAction(ref objOperationResult, Globals.ClientSession.i_CurrentExecutionNodeId, Globals.ClientSession.i_SystemUserId, "frmConsultaProductostock", Globals.ClientSession.i_RoleId);

            //_btnBuscar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmConsultaProductostock_Buscar", _formActions);
            //btnBuscar.Enabled = _btnBuscar;

            //#endregion
           
           
            
           
        }

        #region Busqueda
        private void btnBuscar_Click(object sender, EventArgs e)
        {

            OperationResult objOperationResult = new OperationResult();
            if (Validar.Validate(true, false).IsValid)
            {
                        BindGrid();
                        grdData.Focus();
                      
            }

        }
       
        private void BindGrid( )
        {
            List<ConsultaVentasExportacion> objData = new List<ConsultaVentasExportacion>();
            OperationResult  OperationRes = new OperationResult ();
            OcultarMostrarBuscar(true);
            Cursor.Current = Cursors.WaitCursor;
            Task.Factory.StartNew(() =>
            {

                objData = GetData(ref OperationRes);
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

                    //txtBuscarProducto.Enabled = false;
                    
                    return;
                }
                

            
                grdData.DataSource = objData;
                if (grdData.Rows.Count() > 0)
                {
                    //txtBuscarProducto.Enabled = true;
                   
                    lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", objData.Count);
                    InicializarIndex();
                }
                else
                {
                    //txtBuscarProducto.Enabled = false;
                   
                    lblContadorFilas.Text = string.Format("Se encontraron {0} registros.",0);
                }
            
               
            }
                 , TaskScheduler.FromCurrentSynchronizationContext());
       
            
        }


        private List<ConsultaVentasExportacion> GetData(ref OperationResult OperationRes)
        {


            var _objData = new VentaBL().ConsultaVentaExportacion(ref OperationRes, DateTime.Parse(dtpFechaInicio.Text), DateTime.Parse(dtpFechaFin.Text + " 23:59"));
            return _objData;
           
            
        }

     private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Buscando... " + "Ventas Exportación"
                : @"Consulta de Ventas Exportación";
            pBuscando.Visible = estado;
            btnBuscar.Enabled = !estado;
        }
        #endregion
       
     
        #region Grilla
  

        private void grdData_DoubleClick(object sender, EventArgs e)
        {

            if (grdData.ActiveRow == null || grdData.ActiveRow.Index < 0) return;

            //if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoAlmacen"].Value != null)
            //{
               
            //}
            //else
            //{
                
            //}

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
        }

        private void btnKardex_Click(object sender, EventArgs e)
        {
            
            
            
            
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

        private void btnExportarBandeja_Click(object sender, EventArgs e)
        {

            try
            {

                if (!grdData.Rows.Any() || (_tarea != null && !_tarea.IsCompleted)) return;

                const string dummyFileName = "Ventas Exportación";

                using (var sf = new SaveFileDialog
                {
                    DefaultExt = "xlsx",
                    Filter = @"xlsx files (*.xlsx)|*.xlsx",
                    FileName = dummyFileName
                })
                {
                    if (sf.ShowDialog() != DialogResult.OK) return;
                    btnExportarBandeja.Appearance.Image = Resource.loadingfinal1;
                    var filename = sf.FileName;
                    _tarea = Task.Factory.StartNew(() =>
                    {
                        using (var ultraGridExcelExporter1 = new UltraGridExcelExporter())
                            ultraGridExcelExporter1.Export(grdData, filename);
                    }, _cts.Token)
                        .ContinueWith(t => ActualizarLabel("Ventas Exportación"), TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show("Ocurrió un error al Exportar, quizás el documento esta siendo utilizado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }

        private void ActualizarLabel(string texto)
        {
            lblDocumentoExportado.Text = texto;
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
