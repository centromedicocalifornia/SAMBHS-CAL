using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.BL;
using SAMBHS.Compra.BL;
using SAMBHS.Almacen.BL;
using System.Threading.Tasks;
using System.Threading;
using SAMBHS.Common.BE;
namespace SAMBHS.Windows.WinClient.UI.Reportes.Compras
{
    public partial class frmComprasLineaAnalitico : Form
    {
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL();
        ComprasBL _objComprasBL = new ComprasBL();
        EstablecimientoBL _objEstablecimientoBL = new EstablecimientoBL();
        List<KeyValueDTO> _ListadoGruposOrdenar = new List<KeyValueDTO>();
        LineaBL _objLineaBL = new LineaBL();
        MarcaBL _objMarcaBL= new MarcaBL ();
         CancellationTokenSource _cts = new CancellationTokenSource();
        public string _whereAlmacenesConcatenados = string.Empty, _AlmacenesConcatenados = String.Empty;
        int ConsideraDocumentosInternos = -1;
        
        public frmComprasLineaAnalitico(string Modalidad)
        {
            InitializeComponent();
            ConsideraDocumentosInternos = Modalidad == Constants.ModuloContable ? 0 : 1;
        }
        private void frmComprasLineaAnalitico_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            ValidarFechas();
            CargarCombos();
        }

        private void CargarCombos()
        {
            OperationResult objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList (cboAlmacen, "Value1", "Id", _objNodeWarehouseBL.ObtenerAlmacenesParaCombo(ref objOperationResult, null, Globals.ClientSession.i_IdEstablecimiento.Value), DropDownListAction.Select);
           // Utils.Windows.LoadDropDownList(cboOrdenar, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 65, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboTipoCompra, "Value1", "Id", _objComprasBL.ObtenerConceptosParaCombo(ref objOperationResult, 1, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboEstablecimiento, "Value1", "Id", _objEstablecimientoBL.ObtenerEstablecimientosValueDto(ref objOperationResult, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboLinea, "Value1", "Id", _objLineaBL.LlenarComboLinea(ref objOperationResult, "v_CodLinea"), DropDownListAction.Select);
            cboAlmacen.Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
            Utils.Windows.LoadUltraComboEditorList(cboMarca, "Value1", "Id", _objMarcaBL.LlenarComboMarca(ref objOperationResult, "v_CodMarca", "-1"), DropDownListAction.All);
            //cboOrdenar.SelectedValue = "1";
            cboAgrupar.Text = "SIN AGRUPAR";
            cboEstablecimiento.Value = Globals.ClientSession.i_IdEstablecimiento.ToString();
            cboTipoCompra.Value = "-1";
            cboEstablecimiento.Enabled = false;
            cboLinea.Value = "-1";
            cboMarca.Value = "-1";

        }
        private void ValidarFechas()
        {
            string Periodo = Globals.ClientSession.i_Periodo.ToString();
            string Mes = DateTime.Today.Month.ToString();
            if (DateTime.Now.Year.ToString().Trim() == Periodo)
            {
                dtpFechaInicio.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/" + "01").ToString());
                dtpFechaInicio.MaxDate = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                dtpFechaInicio.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                dtpFechaFin.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/" + "01").ToString());
                dtpFechaFin.MaxDate = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                dtpFechaFin.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());

            }
            else
            {
                dtpFechaInicio.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/01").ToString());
                dtpFechaInicio.MaxDate = DateTime.Parse((Periodo + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(Periodo), 12)).ToString()).ToString());
                dtpFechaInicio.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                dtpFechaFin.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/01").ToString());
                dtpFechaFin.MaxDate = DateTime.Parse((Periodo + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(Periodo), 12)).ToString()).ToString());
                dtpFechaFin.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());

            }
        }

        private void btnVisualizar_Click(object sender, EventArgs e)
        {
            try
            {
                if (uvValidaciones.Validate(true, false).IsValid)
                {
                    OperationResult objOperationResult = new OperationResult();
                    string strOrderExpression = string.Empty;
                    List<string> Filters = new List<string>();
                    string _strFilterExpression = null, Seccion = string.Empty, strNombreGrupollave = string.Empty, strGrupollave = string.Empty;
                    _ListadoGruposOrdenar = _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 65, null);
                    if (txtCodigoProducto.Text != string.Empty) Filters.Add("CodigoProducto==\"" + txtCodigoProducto.Text.Trim() + "\"");
                    if (txtProveedor.Text != string.Empty) Filters.Add("CodigoProveedor==\"" + txtProveedor.Text.Trim() + "\"");
                    if (txtPedido.Text != string.Empty) Filters.Add("NroPedido==\"" + txtPedido.Text.Trim() + "\"");
                    if (cboEstablecimiento.Value.ToString() != "-1") Filters.Add("IdEstablecimiento==" + cboEstablecimiento.Value.ToString());
                    if (cboAlmacen.Value.ToString() != "-1")
                    {
                        Filters.Add("IdAlmacen==" + cboAlmacen.Value.ToString());
                    }
                    else
                    {
                        Filters.Add(_whereAlmacenesConcatenados);
                    }
                    if (cboTipoCompra.Value.ToString() != "-1") Filters.Add("pIntTipoCompra==" + cboTipoCompra.Value.ToString());
                    if (cboLinea.Value.ToString() != "-1") Filters.Add("pIntLinea==\"" + cboLinea.Value.ToString() + "\"");
                    if (cboMarca.Value.ToString() != "-1") Filters.Add("v_Marca==\"" + cboMarca.Value.ToString() + "\"");
                    if (Filters.Count > 0)
                    {
                        foreach (string item in Filters)
                        {
                            _strFilterExpression = _strFilterExpression + item + " && ";
                        }
                        _strFilterExpression = _strFilterExpression.Substring(0, _strFilterExpression.Length - 4);
                    }
                   
                        CargarReporte(dtpFechaInicio.Value.Date,DateTime.Parse ( dtpFechaFin.Text +" 23:59"), _strFilterExpression, cboAgrupar.Text.ToString(), strOrderExpression);
                }
            }
            catch
            {
                UltraMessageBox.Show("Se produjo un error  al mostrar el reporte ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }
        }
        
        
        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Reporte de Compras por Linea - Analítico"
                : @"Reporte de  Compras por Linea - Analítico";
            pBuscando.Visible = estado;
           
            btnVisualizar.Enabled =!estado ;
        }

        private void CargarReporte(DateTime FechaInicio, DateTime FechaFin, string Filtro, string Agrupar, string Ordenar)
        {

          
            string strNombreGrupollave = string.Empty, strGrupollave = string.Empty;
            var rp = new Reportes.Compras.crComprasLineaAnalitico();
            strGrupollave = cboAgrupar.Text.Trim();
            OperationResult objOperationResult= new OperationResult ();

            List<ReporteComprasLineaAnalitico> aptitudeCertificate= new List<ReporteComprasLineaAnalitico> ();
             OcultarMostrarBuscar(true);
            Cursor.Current = Cursors.WaitCursor;
            Task.Factory.StartNew(() =>
            {
      aptitudeCertificate = new ComprasBL().ReporteComprasLinea( ref objOperationResult ,   FechaInicio, FechaFin, Filtro, Agrupar, Ordenar, strGrupollave, "ANALITICO", ConsideraDocumentosInternos,false);
           
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
                        UltraMessageBox.Show("Ocurrió un Error al realizar Reporte de Compras por Linea - Analítico","Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show("Ocurrió un Error al realizar Reporte de Compras por Linea - Analítico","Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }
                var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();

            DataSet ds1 = new DataSet();
            DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
            DataTable dt2 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate2);


            ds1.Tables.Add(dt);
            ds1.Tables.Add(dt2);
            ds1.Tables[0].TableName = "dsReporteComprasLineaAnalitico";
            ds1.Tables[1].TableName = "dsEmpresa";

            rp.SetDataSource(ds1);

            rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count());
            rp.SetParameterValue("DecimalesCantidad", (int)Globals.ClientSession.i_CantidadDecimales);
            rp.SetParameterValue("Fecha", "DEL  " + FechaInicio.Date.Day.ToString("00") + "/" + FechaInicio.Date.Month.ToString("00") + "/" + FechaInicio.Date.Year + " AL " + FechaFin.Date.Day.ToString("00") + "/" + FechaFin.Date.Month.ToString("00") + "/" + FechaFin.Date.Year); 
            rp.SetParameterValue ("Establecimiento",cboEstablecimiento.Text.Trim ()  );
            rp.SetParameterValue("TipoAgrupamiento", cboAgrupar.Text.Trim());
            rp.SetParameterValue("Tproveedor", txtProveedor.Text.Trim());
            rp.SetParameterValue("ReporteGastos", true);
            rp.SetParameterValue("Tproducto",txtCodigoProducto.Text.Trim ());
            rp.SetParameterValue("NombreEmpresa", aptitudeCertificate2.FirstOrDefault().NombreEmpresaPropietaria.Trim());
            rp.SetParameterValue("RucEmpresa", "R.U.C. : " + aptitudeCertificate2.FirstOrDefault().RucEmpresaPropietaria.Trim());

            crystalReportViewer1.ReportSource = rp;
            crystalReportViewer1.Show();
            crystalReportViewer1.Zoom(110);
            }
                , TaskScheduler.FromCurrentSynchronizationContext());
        
        }

        private void cboEstablecimiento_SelectedIndexChanged(object sender, EventArgs e)
        {

            EstablecimientoBL objEstablecimientoBL = new EstablecimientoBL();
            List<KeyValueDTO> x = new List<KeyValueDTO>();
            _whereAlmacenesConcatenados = string.Empty;
            _AlmacenesConcatenados = string.Empty;

            if (cboEstablecimiento.Value == null) return;

            x = objEstablecimientoBL.GetAlmacenesXEstablecimiento(int.Parse(cboEstablecimiento.Value.ToString()));

            if (x.Count > 0)
            {
                foreach (var item in x)
                {
                    _whereAlmacenesConcatenados = _whereAlmacenesConcatenados + "IdAlmacen==" + item.Id + " || ";
                    _AlmacenesConcatenados = _AlmacenesConcatenados + item.Value1 + ", ";
                }
                _whereAlmacenesConcatenados = _whereAlmacenesConcatenados.Substring(0, _whereAlmacenesConcatenados.Length - 4);
                _AlmacenesConcatenados = _AlmacenesConcatenados.Substring(0, _AlmacenesConcatenados.Length - 2);
            }
            Utils.Windows.LoadUltraComboEditorList (cboAlmacen, "Value1", "Id", x, DropDownListAction.All);
            cboAlmacen.Value = "-1";
        }

        private void txtCodigoProducto_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscarProducto")
            {
                OperationResult objOperationResult = new OperationResult();
                Mantenimientos.frmBuscarProducto frm = new Mantenimientos.frmBuscarProducto(-1, "REPORTE", null, null);
                frm.ShowDialog();

                if (frm._IdProducto != null)
                {

                    txtCodigoProducto.Text = frm._CodigoInternoProducto.Trim();

                }
                else
                {
                    txtCodigoProducto.Text = string.Empty;
                }
            }
        }

        private void txtProveedor_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            Mantenimientos.frmBuscarProveedor frm = new Mantenimientos.frmBuscarProveedor("REPORTE", "RUC");
            frm.ShowDialog();
            if (frm._IdProveedor != null)
            {

                txtProveedor.Text = frm._CodigoProveedor.Trim().ToUpper();
               

            }
        }

        private void cboLinea_SelectedIndexChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (cboLinea.Value == null || cboLinea.Value.ToString ()=="-1" ) return;
            Utils.Windows.LoadUltraComboEditorList(cboMarca, "Value1", "Id", _objMarcaBL.LlenarComboMarca(ref objOperationResult, "v_CodMarca", cboLinea.Value.ToString()), DropDownListAction.Select);
            cboLinea.Value = "-1";
     
        }

        private void frmComprasLineaAnalitico_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }

        private void cboLinea_ValueChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (cboLinea.Value == null || cboLinea.Value.ToString() == "-1")
            {
                Utils.Windows.LoadUltraComboEditorList(cboMarca, "Value1", "Id", _objMarcaBL.LlenarComboMarca(ref objOperationResult, "v_CodMarca", "-1"), DropDownListAction.All);
                cboMarca.Value = "-1";
            }
            else
            {
                Utils.Windows.LoadUltraComboEditorList(cboMarca, "Value1", "Id", _objMarcaBL.LlenarComboMarca(ref objOperationResult, "v_CodMarca", cboLinea.Value.ToString()), DropDownListAction.Select);
                cboMarca.Value = "-1";
            }
        }
    }
}
