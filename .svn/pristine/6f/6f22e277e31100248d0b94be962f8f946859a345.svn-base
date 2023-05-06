using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.Common.BL;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Almacen.BL;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using System.Globalization;
using SAMBHS.Common.BE;
using System.Threading.Tasks;
using System.Threading;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Almacen
{
    public partial class frmResumenMovimientos : Form
    {
        public string v_CodigoInterno = string.Empty, _whereAlmacenesConcatenados, _AlmacenesConcatenados;
        EstablecimientoBL objEstablecimientoBL = new EstablecimientoBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        LineaBL _objLineaBL = new LineaBL();
        MarcaBL _objMarcaBL = new MarcaBL();
        CancellationTokenSource _cts = new CancellationTokenSource();
        public frmResumenMovimientos(string parametro)
        {
            InitializeComponent();
        }
        private void frmResumenMovimientos_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            ValidarMes();
            lblperiodo.Text = Globals.ClientSession.i_Periodo.ToString();
            CargarCombos();
            
            
            
        }
        private void CargarCombos()
        {
            OperationResult objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList (cboEstablecimiento, "Value1", "Id", objEstablecimientoBL.ObtenerEstablecimientosValueDto(ref objOperationResult, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", objEstablecimientoBL.GetAlmacenesXEstablecimiento(-1), DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboLinea, "Value1", "Id", _objLineaBL.LlenarComboLinea(ref objOperationResult, "v_CodLinea"), DropDownListAction.All );
            Utils.Windows.LoadUltraComboEditorList(cboMarca, "Value1", "Id", _objMarcaBL.LlenarComboMarca(ref objOperationResult, "v_CodMarca", "-1"), DropDownListAction.All );

            
            cboEstablecimiento.Value = Globals.ClientSession.i_IdEstablecimiento.ToString();
            cboMoneda.Value = Globals.ClientSession.i_IdMonedaCompra.ToString();
            cboEstablecimiento.Enabled = false;
            cboLinea.Value = "-1";
            cboAlmacen.Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
            cboMarca.Value = "-1";
            cboFormato.Value = "1";
        
        }
        private void ValidarMes()
        {
            if (Globals.ClientSession.i_Periodo == DateTime.Now.Year)
            {
                nupMes.Value = DateTime.Now.Month;
                nupMes.Maximum = DateTime.Now.Month;
                nupMes.Minimum = 1;
                

            }
            else
            {
                nupMes.Value = DateTime.Now.Month;
                nupMes.Maximum = 12;
                nupMes.Minimum = 1;

            }
        
        
        
        }
        private void btnBuscarProducto_Click(object sender, EventArgs e)
        {
          
        }
        private void cboEstablecimiento_SelectedIndexChanged(object sender, EventArgs e)
        {
            _whereAlmacenesConcatenados = string.Empty;
            _AlmacenesConcatenados = string.Empty;
            List<KeyValueDTO> x = new List<KeyValueDTO>();
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
                _AlmacenesConcatenados = cboEstablecimiento.Text + " / " + _AlmacenesConcatenados.Substring(0, _AlmacenesConcatenados.Length - 2);
            }
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", x, DropDownListAction.All);
            cboAlmacen.Value = "-1";
        }
        private void btnVisualizar_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> Filters = new List<string>();
                string _strFilterExpression = string.Empty;
                DateTime FechaInicio = DateTime.Parse("01/" + nupMes.Value.ToString() + "/" + Globals.ClientSession.i_Periodo.ToString() + " 00:00");
                DateTime FechaFin = DateTime.Parse(DateTime.DaysInMonth(int.Parse(Globals.ClientSession.i_Periodo.ToString()), int.Parse(nupMes.Value.ToString())).ToString() + "/" + nupMes.Value + "/" + Globals.ClientSession.i_Periodo.ToString() + " 23:59");

                if (uvValidar.Validate(true, false).IsValid)
                {
                    if (cboAlmacen.Value.ToString() != "-1")
                    {
                        Filters.Add("IdAlmacen==" + cboAlmacen.Value.ToString());
                    }
                    else
                    {
                        Filters.Add(_whereAlmacenesConcatenados);
                    }


                    if (Filters.Count > 0)
                    {
                        foreach (string item in Filters)
                        {
                            _strFilterExpression = _strFilterExpression + item + " && ";
                        }
                        _strFilterExpression = _strFilterExpression.Substring(0, _strFilterExpression.Length - 4);
                    }
                   
                        CargarReporte(FechaInicio, FechaFin, cboLinea.Value.ToString(), txtCodigoProducto.Text.Trim(), int.Parse(cboMoneda.Value.ToString()), _strFilterExpression, txtPedido.Text.Trim(), int.Parse(cboEstablecimiento.Value.ToString()));

                }
            }
            catch (Exception f)
            {
                UltraMessageBox.Show("Ocurrió un error al realizar Reporte Resumen Movimiento", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            }

        }
        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Reporte de Resumen de Movimientos"
                : @"Reporte de Resumen de Movimientos";
            pBuscando.Visible = estado;
            btnVisualizar.Enabled =!estado ;
        }
        private void CargarReporte(DateTime FechaInicio, DateTime FechaFin, string pstrLinea, string pstrCodigoProducto, int pIntMoneda, string pstrAlmacenes, string pstrPedido, int pIntEstablecimiento)
        {
            OperationResult objOperationResult = new OperationResult();
            List<KardexList> aptitudeCertificate = new List<KardexList>();
            string  Mes = new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(int.Parse (nupMes.Value.ToString ()));
            var rp = new Reportes.Almacen.crReporteMovimientosAlmacen();
            var Empresa = new NodeBL().ReporteEmpresa();
            Cursor.Current = Cursors.WaitCursor;
            DateTime fecha1 = Convert.ToDateTime("01/01/" + Globals.ClientSession.i_Periodo.ToString() + " 00:00");
            DateTime fecha2 = DateTime.Parse(DateTime.DaysInMonth (Globals.ClientSession.i_Periodo.Value , int.Parse ( nupMes.Value.ToString ()))+"/"+int.Parse ( nupMes.Value.ToString ())+ "/"+ Globals.ClientSession.i_Periodo.ToString ()+" 23:59");
            OcultarMostrarBuscar(true);
            Task.Factory.StartNew(() =>
            {

               // aptitudeCertificate = new AlmacenBL().ReporteStock(ref objOperationResult, int.Parse(cboEstablecimiento.Value.ToString()), fecha1, fecha2,"", pIntMoneda, pstrCodigoProducto, pstrPedido,pstrLinea, 0, 0, 0, 0, 0, int.Parse (cboFormato.Value.ToString ()) ,"", fecha2,0,null,false,-1,false,true);
                aptitudeCertificate = new AlmacenBL().ResumenMovimientos(ref objOperationResult, int.Parse(cboEstablecimiento.Value.ToString()), fecha1, fecha2, "", pIntMoneda, pstrCodigoProducto, pstrPedido, pstrLinea, 0, 0, 0, 0, 0, int.Parse(cboFormato.Value.ToString()), "", fecha2, 0, null, false, -1, false, true);
                
                },_cts.Token)
            .ContinueWith(t =>
            {
                if (_cts.IsCancellationRequested) return;
                OcultarMostrarBuscar(false);
                Cursor.Current = Cursors.Default;
                if (objOperationResult.Success == 0)
                {
                    if (!string.IsNullOrEmpty(objOperationResult.ExceptionMessage))
                    {
                        UltraMessageBox.Show("Ocurrió un error al realizar Reporte Resumen Movimiento", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show("Ocurrió un error al realizar Reporte Resumen Movimiento", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }

            
                var aptitudeCertificate1 = new NodeBL().ReporteEmpresa();

                DataSet ds1 = new DataSet();
                DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
                DataTable dt2 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate1);
                ds1.Tables.Add(dt);
                ds1.Tables.Add(dt2);
                ds1.Tables[0].TableName = "dtKardex";
                ds1.Tables[1].TableName = "dsEmpresa";


                rp.SetDataSource(ds1);
                rp.SetParameterValue ("Fecha","MES :  " + Mes.ToUpper () + " " + Globals.ClientSession.i_Periodo.ToString());
                rp.SetParameterValue ("Moneda","MONEDA REPORTE : "+cboMoneda.Text);
                rp.SetParameterValue ("DecimalesCantidad",(int)Globals.ClientSession.i_CantidadDecimales);
                rp.SetParameterValue ("DecimalesPrecio",(int)Globals.ClientSession.i_PrecioDecimales);
                rp.SetParameterValue ("Establecimiento","ESTABLECIMIENTO : "+ cboEstablecimiento.Text.Trim().ToUpper ());
                rp.SetParameterValue("NombreEmpresa",Empresa.FirstOrDefault ().NombreEmpresaPropietaria );
                rp.SetParameterValue("RucEmpresa", "R.U.C. : "+Empresa.FirstOrDefault ().RucEmpresaPropietaria);
                rp.SetParameterValue ("NroRegistros",aptitudeCertificate.Count ());

                crystalReportViewer1.ReportSource = rp;
                crystalReportViewer1.Show();
                 }
                , TaskScheduler.FromCurrentSynchronizationContext());
            }
            
        private void txtCodigoProducto_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            Mantenimientos.frmBuscarProducto frm = new Mantenimientos.frmBuscarProducto(-1, "REPORTE", null, null);
            frm.ShowDialog();

            if (frm._IdProducto != null)
            {
                txtCodigoProducto.Text = frm._CodigoInternoProducto.Trim();
            
                v_CodigoInterno = frm._CodigoInternoProducto.Trim();

            }
            else
            {
                txtCodigoProducto.Text = string.Empty;
              
                v_CodigoInterno = string.Empty;
            }
        }
        private void txtCodigoProducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
            {
                v_CodigoInterno = string.Empty;
               
            }
        }

        private void frmResumenMovimientos_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }

        private void cboLinea_ValueChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (cboLinea.Value == null || cboLinea.Value.ToString() == "-1") return;

            Utils.Windows.LoadUltraComboEditorList(cboMarca, "Value1", "Id", _objMarcaBL.LlenarComboMarca(ref objOperationResult, "v_CodMarca", cboLinea.Value.ToString()), DropDownListAction.Select);
            cboMarca.Value = "-1";
        }
    }
}
