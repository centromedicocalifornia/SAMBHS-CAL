using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Almacen.BL;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using SAMBHS.Common.Resource;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.CommonWIN.BL;
using System.Threading;
using System.Threading.Tasks;
namespace SAMBHS.Windows.WinClient.UI.Reportes.Almacen
{
    public partial class frmProducto : Form
    {
        #region Declaraciones / Referencias
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        EstablecimientoBL objEstablecimientoBL = new EstablecimientoBL();
        NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL();
        CancellationTokenSource _cts = new CancellationTokenSource();
        LineaBL _objLineaBL = new LineaBL();
     
        List<string> Grupollave = new List<string>();
        List<string> NombreGrupollave = new List<string>();
        #endregion
        #region Carga de inicializacion
        public frmProducto(string _IdProducto)
        {
            InitializeComponent();

        }
        #endregion
        #region Cargar Load
        private void frmProducto_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            OperationResult objOperationResult = new OperationResult();

            #region Cargar Combos
            Utils.Windows.LoadUltraComboEditorList(cboOrden, "Value1", "Value2", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 61, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAgrupamiento, "Value1", "Value2", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 62, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboLinea, "Value1", "Id", _objLineaBL.LlenarComboLinea(ref objOperationResult, "v_CodLinea"), DropDownListAction.Select);

            cboAgrupamiento.SelectedIndex = 2;
            cboOrden.SelectedIndex = 1;
            cboLinea.Value = "-1";
            #endregion
        }
        #endregion
        #region Cargar Reporte
        private void CargarReporte(string _IdProducto, string _IdLinea)
        {
            OperationResult objOperationResult = new OperationResult();
            List<ReporteProducto> aptitudeCertificate = new List<ReporteProducto>();
            datahierarchyDto __datahierarchyDto = new datahierarchyDto();
            List<KeyValueDTO> _ListadoGrupos = new List<KeyValueDTO>();
            List<datahierarchyDto> _datahierarchyDto = new List<datahierarchyDto>();
            List<string> Retonar = new List<string>();
            List<string> Retonar2 = new List<string>();
            string strOrderExpression = "", strGrupollave="";

            var rp = new Reportes.Almacen.crProducto();
            strOrderExpression = "";
            _ListadoGrupos = _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 62, null);
            strGrupollave = cboAgrupamiento.Text.Trim();
            strOrderExpression += strOrderExpression != "" ? strOrderExpression != cboOrden.Value.ToString().Trim() ? "," + cboOrden.Value.ToString().Trim() : "" : cboOrden.Value.ToString().Trim();

            OcultarMostrarBuscar(true);
            Cursor.Current = Cursors.WaitCursor;

            Task.Factory.StartNew(() =>
            {
                aptitudeCertificate = new ProductoBL().ReporteProducto(_IdProducto, _IdLinea, "" + strOrderExpression + " ASC", strGrupollave);
            }, _cts.Token)
            .ContinueWith(t =>
            {
                if (_cts.IsCancellationRequested) return;
                OcultarMostrarBuscar(false);
                Cursor.Current = Cursors.Default;
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

                var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();

                DataSet ds1 = new DataSet();
                DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
                DataTable dt2 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate2);


                ds1.Tables.Add(dt);
                ds1.Tables.Add(dt2);
                ds1.Tables[0].TableName = "dsProducto";
                ds1.Tables[1].TableName = "dsEmpresa";

                rp.SetDataSource(ds1);


                rp.SetParameterValue("FechaHoraImpresion", chkHoraimpresion.Checked == true ? "1" : "0");
                rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count());

                crystalReportViewer1.ReportSource = rp;
                crystalReportViewer1.Show();
                crystalReportViewer1.Zoom(110);

            }, TaskScheduler.FromCurrentSynchronizationContext());


        }


        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Reporte de Producto"
                : @"Reporte de Producto";
            pBuscando.Visible = estado;
            BtnVuisualizar.Enabled = !estado;

        }
        #endregion
        #region Controles Botones
        private void BtnVuisualizar_Click(object sender, EventArgs e)
        {
            try{

                if (uvDatos.Validate(true, false).IsValid)
                {
                    if (cboAgrupamiento.Text == "" || cboAgrupamiento.Text == "--Seleccionar--")
                    {
                        UltraMessageBox.Show("Por favor llene los campos requeridos para visualizar el reporte", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        cboAgrupamiento.Focus();
                        return;
                    }
                    
                  CargarReporte(TxtProducto.Text.Trim(), cboLinea.Value.ToString());

                }
                else
                {

                    UltraMessageBox.Show("Por favor llene los campos requeridos para visualizar el reporte", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

       
            
            }catch{
                UltraMessageBox.Show("Se produjo un error  al mostrar el reporte ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

         
        }
        private void btnBuscarProducto_Click(object sender, EventArgs e)
        {
            //OperationResult objOperationResult = new OperationResult();
            //Mantenimientos.frmBuscarProducto frm = new Mantenimientos.frmBuscarProducto(1, "PedidoVenta", "", "");
            //frm.ShowDialog();

            //if (frm._IdProducto != null)
            //{
            //    TxtProducto.Text = frm._CodigoInternoProducto;

            //}
            //else
            //{

            //}


        }
        private void TxtProducto_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            Mantenimientos.frmBuscarProducto frm = new Mantenimientos.frmBuscarProducto(-1, "REPORTE", null, null);
            frm.ShowDialog();

            if (frm._IdProducto != null)
            {
                TxtProducto.Text = frm._CodigoInternoProducto.Trim();

            }
            else
            {
                TxtProducto.Text = string.Empty;
            }
        }
        #endregion

        private void frmProducto_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }

        

    }
}
