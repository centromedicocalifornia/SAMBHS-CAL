using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Almacen.BL;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using SAMBHS.Common.BL;
using SAMBHS.Common.BE;
using System.Threading.Tasks;
using System.Threading;
using SAMBHS.Contabilidad.BL;
using System.Globalization;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Almacen
{
    public partial class frmReporteAsientoConsumoArticulo : Form
    {
        EstablecimientoBL objEstablecimientoBL = new EstablecimientoBL();
        LineaBL _objLineaBL = new LineaBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        ProductoBL _objProductoBL = new ProductoBL();
        List<KeyValueDTO> _ListadoGruposOrdenar = new List<KeyValueDTO>();
        CancellationTokenSource _cts = new CancellationTokenSource();
        MarcaBL _objMarcaBL = new MarcaBL();
        public string v_CodigoInterno = string.Empty;
        public string v_Pedido = string.Empty;
        public frmReporteAsientoConsumoArticulo(string pstrParametro)
        {
            InitializeComponent();
        }

        private void frmResumenAlmacen_Load(object sender, EventArgs e)
        {

            this.BackColor = new GlobalFormColors().FormColor;
            ValidarFechas();
            cboTipo.Value  = "1";
            chkIncluirNroPedido.Checked = Globals.ClientSession.v_RucEmpresa == Constants.RucWortec;
        }

        private void ValidarFechas()
        {


            lblPeriodo.Text = "Periodo : " + Globals.ClientSession.i_Periodo.ToString();
            nupMes.Minimum = 1;
            nupMes.Maximum = 12;
            nupMes.Value = decimal.Parse(DateTime.Now.Month.ToString());
        }

        private void btnVisualizar_Click(object sender, EventArgs e)
        {
            List<string> Filters = new List<string>();
            string _strFilterExpression = string.Empty, strOrderExpression = string.Empty, pstrCodigoProducto = string.Empty;
            OperationResult objOperationResult = new OperationResult();

            v_Pedido = string.Empty; v_CodigoInterno = string.Empty;
            _ListadoGruposOrdenar = _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 96, null);
            if (uvValidar.Validate(true, false).IsValid)
            {
                CargarReporte();
            }

        }
        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Reporte de Asiento Consumo por Artículo"
                : @"Reporte de Asiento Consumo por Artículo";
            pBuscando.Visible = estado;


            btnVisualizar.Enabled = !estado;
        }
        private void CargarReporte()
        {


            DateTime FechaInicio = DateTime.Parse(Globals.ClientSession.i_Periodo.ToString() + "/" + nupMes.Value.ToString() + "/" + 01);
            DateTime FechaFin = DateTime.Parse((Globals.ClientSession.i_Periodo.ToString() + "/" + nupMes.Value.ToString() + "/" + (DateTime.DaysInMonth(int.Parse(Globals.ClientSession.i_Periodo.ToString()), int.Parse(nupMes.Value.ToString())) + " 23:59").ToString()).ToString());
            OperationResult objOperationResult = new OperationResult();
           // List<ReporteAsientoConsumo> aptitudeCertificate = new List<ReporteAsientoConsumo>();
             List<ReporteListadoSalidaAlmacenAnalitico>  aptitudeCertificate = new List<ReporteListadoSalidaAlmacenAnalitico> (); 
            List<KeyValueDTO> Almacenes = new List<KeyValueDTO>();
            List<string> Filters = new List<string>();

            string _whereAlmacenesConcatenados = string.Empty, _AlmacenesConcatenados = string.Empty, _strFilterExpression = null;
            // Filters.Add ("i_IdTipoMovimiento==1");

            _AlmacenesConcatenados = string.Empty;

            DateTime FechaInicioValorizar = DateTime.Parse("01/01/" + Globals.ClientSession.i_Periodo.ToString());
            Almacenes = objEstablecimientoBL.GetAlmacenesXEstablecimiento(Globals.ClientSession.i_IdEstablecimiento.Value);

            if (Almacenes.Count > 0)
            {
                foreach (var item in Almacenes)
                {
                    _whereAlmacenesConcatenados = _whereAlmacenesConcatenados + "IdAlmacen==" + item.Id + " || ";
                    _AlmacenesConcatenados = _AlmacenesConcatenados + item.Value1 + "/ ";
                }
                _whereAlmacenesConcatenados = _whereAlmacenesConcatenados.Substring(0, _whereAlmacenesConcatenados.Length - 4);
                //_AlmacenesConcatenados = cboEstablecimiento.Text + " / " + _AlmacenesConcatenados.Substring(0, _AlmacenesConcatenados.Length - 2);
                _AlmacenesConcatenados = _AlmacenesConcatenados.Substring(0, _AlmacenesConcatenados.Length - 2);
            }

            Filters.Add("(" + _whereAlmacenesConcatenados + ")");

            if (Filters.Count > 0)
            {
                foreach (string item in Filters)
                {
                    _strFilterExpression = _strFilterExpression + item + " && ";
                }
                _strFilterExpression = _strFilterExpression.Substring(0, _strFilterExpression.Length - 4);
            }


           
            var Empresa = new NodeBL().ReporteEmpresa();
            OcultarMostrarBuscar(true);

            Cursor.Current = Cursors.WaitCursor;
            Task.Factory.StartNew(() =>
            {
              
                aptitudeCertificate = aptitudeCertificate = new AlmacenBL().ReporteAsientoConsumoNuevo(ref  objOperationResult, FechaInicio, FechaFin, _strFilterExpression, cboTipo.Value.ToString() == "1" ? true : false,txtArtIni.Text.Trim (),chkIncluirNroPedido.Checked);
               
            
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
                        UltraMessageBox.Show("Ocurrió un error al realizar Reporte de Asiento Consumo por Artículo", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show("Ocurrió un error al realizar Reporte de Asiento Consumo por Artículo", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }

                ReportDocument rp = new ReportDocument();
                if (cboTipo.Value ==null )
            
                    {
                        UltraMessageBox.Show("Ocurrió un error al realizar Reporte de Asiento Consumo por Artículo, Como Null", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                if (int.Parse(cboTipo.Value.ToString()) == 1)
                {
                    rp = new Reportes.Almacen.crReporteAsientoConsumoArticulo_();

                }
                else
                {
                    rp = new Reportes.Almacen.crReporteAsientoConsumoArticuloResumen_();
                }


                var aptitudeCertificate1 = new NodeBL().ReporteEmpresa();

                DataSet ds1 = new DataSet();
                DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
             
                ds1.Tables.Add(dt);
              
                //ds1.Tables[0].TableName = "dsReporteAsientoConsumo";
                ds1.Tables[0].TableName = "dsListadoSalidaAlmacenAnalitico";
                rp.SetDataSource(ds1);

                rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count());
                rp.SetParameterValue("NombreEmpresa", Empresa.FirstOrDefault().NombreEmpresaPropietaria);
                rp.SetParameterValue("RucEmpresa", "R.U.C. : " + Empresa.FirstOrDefault().RucEmpresaPropietaria.Trim());

                rp.SetParameterValue("Mes", "MES : " + new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(int.Parse(nupMes.Value.ToString())).ToUpper() + " -  PERIODO : " + Globals.ClientSession.i_Periodo.ToString());


                   
                crystalReportViewer1.ReportSource = rp;
                crystalReportViewer1.Show();
            }
                , TaskScheduler.FromCurrentSynchronizationContext());

        }




        private void txtCodigoProducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
            {
                v_CodigoInterno = string.Empty;

            }
        }



        private void frmResumenAlmacen_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }

        private void txtArtIni_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscarArticulo")
            {
                OperationResult objOperationResult = new OperationResult();
                Mantenimientos.frmBuscarProducto frm = new Mantenimientos.frmBuscarProducto(-1, "REPORTE", null, null);
                frm.ShowDialog();

                if (frm._IdProducto != null)
                {

                    txtArtIni.Text = frm._CodigoInternoProducto.Trim();
                }
                else
                {
                    txtArtIni.Text = string.Empty;
                }
            }
        }




    }
}
