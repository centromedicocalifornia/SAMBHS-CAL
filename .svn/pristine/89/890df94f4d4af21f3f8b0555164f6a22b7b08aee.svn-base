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
using System.Threading.Tasks;
using System.Threading;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Almacen
{
    public partial class frmListadoIngresoAlmacenAnalitico : Form
    {
        #region Declaraciones / Referencias
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        EstablecimientoBL objEstablecimientoBL = new EstablecimientoBL();
        NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL();
        LineaBL _objLineaBL = new LineaBL();
        string _whereAlmacenesConcatenados;
        string _AlmacenesConcatenados;
        string strOrderExpression;
        string strGrupollave, strGrupollave2;
        string strNombreGrupollave, strNombreGrupollave2;
        List<string> Grupollave = new List<string>();
        List<string> NombreGrupollave = new List<string>();
        CancellationTokenSource _cts = new CancellationTokenSource();
        #endregion
        #region Carga de inicializacion
        public frmListadoIngresoAlmacenAnalitico(string _IdMovimiento)
        {
            InitializeComponent();
        }
        #endregion
        #region Cargar Load
        private void frmListadoIngresoAlmacenAnalitico_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            CargarCombos();
            ValidarFechas();
            chkIncluirNroPedido.Checked = Globals.ClientSession.v_RucEmpresa == Constants.RucWortec;
        }


        private void CargarCombos()
        {
            OperationResult objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboOrden, "Value1", "Value2", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 59, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAgrupar, "Value1", "Value2", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 60, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboMotivo, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 19, null), DropDownListAction.Select);//20 numero del id Del motivo
            Utils.Windows.LoadUltraComboEditorList(cboEstablecimiento, "Value1", "Id", objEstablecimientoBL.ObtenerEstablecimientosValueDto(ref objOperationResult, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", objEstablecimientoBL.GetAlmacenesXEstablecimiento(-1), DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboLinea, "Value1", "Id", _objLineaBL.LlenarComboLinea(ref objOperationResult, "v_CodLinea"), DropDownListAction.Select);
            cboEstablecimiento.Value = Globals.ClientSession.i_IdEstablecimiento.ToString();
            cboAlmacen.Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
            cboMoneda.Value = Globals.ClientSession.i_IdMonedaCompra.ToString();
            cboOrden.SelectedIndex = 3;
            cboAgrupar.SelectedIndex = 8;
            //cboEstablecimiento.Enabled = false;
            cboLinea.Value = "-1";
            cboMotivo.Value = "-1";

        }

        private void ValidarFechas()
        {
            string Periodo = Globals.ClientSession.i_Periodo.ToString();
            string Mes = DateTime.Today.Month.ToString();
            if (DateTime.Now.Year.ToString().Trim() == Periodo)
            {

                dtpFechaRegistroDe.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/" + "01").ToString());
                dtpFechaRegistroDe.MaxDate = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                dtpFechaRegistroDe.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());

                dtpFechaRegistroAl.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/" + "01").ToString());
                dtpFechaRegistroAl.MaxDate = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                dtpFechaRegistroAl.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());


            }
            else
            {
                dtpFechaRegistroDe.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/01").ToString());
                dtpFechaRegistroDe.MaxDate = DateTime.Parse((Periodo + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(Periodo), 12)).ToString()).ToString());
                dtpFechaRegistroDe.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());

                dtpFechaRegistroAl.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/01").ToString());
                dtpFechaRegistroAl.MaxDate = DateTime.Parse((Periodo + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(Periodo), 12)).ToString()).ToString());
                dtpFechaRegistroAl.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());


            }
        }
        #endregion
        #region Cargar Reporte
        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Reporte Listado de Ingreso Almacén Analítico"
                : @"Reporte Listado de Ingreso Almacén Analítico";
            pBuscando.Visible = estado;

            BtnVuisualizar.Enabled = !estado;

        }
        private void CargarReporte(int pintTipoMovimiento, DateTime? pstrt_FechaRegistroIni, DateTime? pstrt_FechaRegistroFin, string pstrIdAlmacen, string pstr_CodInterno, string pstr_CodLinea, int pintIdTipoMotivo, string pstrIdCliente, string pstrNumPedido, string pstrt_Orden, int pstrMoneda)
        {
            OperationResult objOperationResult = new OperationResult();

            datahierarchyDto __datahierarchyDto = new datahierarchyDto();
            List<KeyValueDTO> _ListadoGrupos = new List<KeyValueDTO>();
            List<datahierarchyDto> _datahierarchyDto = new List<datahierarchyDto>();
            List<string> Retonar = new List<string>();
            List<string> Retonar2 = new List<string>();
            strOrderExpression = "";
            _ListadoGrupos = _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 60, null);
            var rp = new Reportes.Almacen.crListadoIngresoAlmacenAnalitico();
            string Seccion;
            List<ReporteListadoIngresoAlmacenAnalitico> aptitudeCertificate = new List<ReporteListadoIngresoAlmacenAnalitico>();
            if (cboAgrupar.Value == null)
            {
                Seccion = "";
            }
            else
            {
                Seccion = cboAgrupar.Value.ToString().ToLower();
            }

            Seccion = Seccion.Replace("g", "G");
            Seccion = Seccion.Replace("s", "S");
            Seccion = Seccion.Replace("h", "H");

            strGrupollave = "";
            strNombreGrupollave = "";
            strGrupollave2 = null;
            strNombreGrupollave2 = null;
            Grupollave = new List<string>();
            NombreGrupollave = new List<string>();

            for (int i = 0; i <= _ListadoGrupos.Count - 1; i++)
            {

                if (cboAgrupar.Value != null && _ListadoGrupos[i].Value2 != null && _ListadoGrupos[i].Value3 != null)
                {
                    if (cboAgrupar.Value.ToString().Trim() == _ListadoGrupos[i].Value2.ToString().Trim() && _ListadoGrupos[i].Value3.ToString().Trim() != "")
                    {
                        if (cboAgrupar.Text.Trim() == _ListadoGrupos[i].Value1.ToString())
                        {

                            strNombreGrupollave = _ListadoGrupos[i].Value1.ToString();
                            string[] splitNombreGrupollave = strNombreGrupollave.Split(new Char[] { '/' });
                            foreach (string s in splitNombreGrupollave)
                            {
                                if (s.Trim() != "")
                                    NombreGrupollave.Add(s);
                            }

                            if (NombreGrupollave.Count == 2)
                            {
                                strNombreGrupollave = NombreGrupollave[0];
                                strNombreGrupollave2 = NombreGrupollave[1];
                            }
                            else
                            {
                                strNombreGrupollave = NombreGrupollave[0];
                            }

                            strOrderExpression = _ListadoGrupos[i].Value3.ToString();
                            strGrupollave = _ListadoGrupos[i].Value3.ToString();
                        }

                    }
                    string[] split = _ListadoGrupos[i].Value2.Split(new Char[] { ',' });
                    foreach (string s in split)
                    {
                        if (s.Trim() != "")
                            Retonar.Add(s);

                    }
                }
            }
            Retonar = Retonar.Distinct().ToList();
            for (int i = 0; i <= Retonar.Count() - 1; i++)
            {

                Retonar2.Add(Retonar[0]);
                Seccion = Retonar[i].ToLower();
                Seccion = Seccion.Replace("g", "G");
                Seccion = Seccion.Replace("s", "S");
                Seccion = Seccion.Replace("h", "H");
                Seccion = Seccion.Replace("f", "F");

            }

            if (cboAgrupar.Value != null && cboAgrupar.Value.ToString().Trim() != "")
            {
                string[] split_ = cboAgrupar.Value.ToString().Split(new Char[] { ',' });
                foreach (string s in split_)
                {
                    if (s.Trim() != "")
                        Seccion = s.ToLower();
                    Seccion = Seccion.Replace("g", "G");
                    Seccion = Seccion.Replace("s", "S");
                    Seccion = Seccion.Replace("h", "H");
                    Seccion = Seccion.Replace("f", "F");
                    if (cboAgrupar.Text.ToString() == "SIN AGRUPAR")
                    {

                        rp.ReportDefinition.Sections[Seccion].SectionFormat.EnableSuppress = true;
                    }
                    else
                    {
                        rp.ReportDefinition.Sections[Seccion].SectionFormat.EnableSuppress = false;
                    }

                }
            }
            // NodeBL _objVentasBL = new NodeBL();
            strOrderExpression += strOrderExpression != "" ? strOrderExpression != cboOrden.Value.ToString().Trim() ? "," + cboOrden.Value.ToString().Trim() : "" : cboOrden.Value.ToString().Trim();
            strOrderExpression = strOrderExpression != "" ? "Establecimiento,Almacen," + strOrderExpression : "Establecimiento,Almacen";
            string[] splitGrupollave = strGrupollave.Split(new Char[] { ',' });
            foreach (string s in splitGrupollave)
            {
                if (s.Trim() != "")
                    Grupollave.Add(s);

            }
            if (Grupollave.Count > 0)
            {
                if (Grupollave.Count == 2)
                {
                    strGrupollave = Grupollave[0];
                    strGrupollave2 = Grupollave[1];

                }
                else
                {
                    strGrupollave = Grupollave[0];
                }
            }

            int Proveedor = TxtProveedor.Text.Trim() == string.Empty ? 0 : 1;
            var Empresa = new NodeBL().ReporteEmpresa();

            OcultarMostrarBuscar(true);
            Cursor.Current = Cursors.WaitCursor;

            Task.Factory.StartNew(() =>
            {
                aptitudeCertificate = new AlmacenBL().ReporteListadoIngresoAlmacenAnalitico(ref objOperationResult, pintTipoMovimiento, pstrt_FechaRegistroIni, pstrt_FechaRegistroFin, pstrIdAlmacen, pstr_CodInterno, pstr_CodLinea, pintIdTipoMotivo, pstrIdCliente, pstrNumPedido, "" + strOrderExpression + " ASC", strGrupollave, strNombreGrupollave, strGrupollave2, strNombreGrupollave2, pstrMoneda, Empresa[0].NombreEmpresaPropietaria, Empresa[0].RucEmpresaPropietaria,chkIncluirNroPedido.Checked);
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
                        UltraMessageBox.Show("Ocurrió un error al realizar Reporte Listado de Ingresos(Analítico)", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show("Ocurrió un error al realizar Reporte Listado de Ingresos(Analítico)", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }

                DataSet ds1 = new DataSet();

                DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);

                dt.TableName = "dsListadoIngresoAlmacenAnalitico";

                ds1.Tables.Add(dt);
                rp.SetDataSource(ds1);



                rp.SetParameterValue("FechaHoraImpresion", chkHoraimpresion.Checked == true ? "1" : "0");
                rp.SetParameterValue("IdMoneda", cboMoneda.Value);
                rp.SetParameterValue("CantidadDecimal", (int)Globals.ClientSession.i_CantidadDecimales);
                rp.SetParameterValue("CantidadDecimalPrecio", (int)Globals.ClientSession.i_PrecioDecimales);
                rp.SetParameterValue("Proveedor", Proveedor);
                rp.SetParameterValue("Fecha", "DEL " + dtpFechaRegistroDe.Value.Date.Day.ToString("00") + "/" + dtpFechaRegistroDe.Value.Date.Month.ToString("00") + "/" + dtpFechaRegistroDe.Value.Date.Year.ToString() + " AL " + dtpFechaRegistroAl.Value.Date.Day.ToString("00") + "/" + dtpFechaRegistroAl.Value.Date.Month.ToString("00") + "/" + dtpFechaRegistroAl.Value.Date.Year.ToString());
                rp.SetParameterValue("Empresa", Empresa.FirstOrDefault().NombreEmpresaPropietaria);
                rp.SetParameterValue("Ruc","R.U.C. : "+ Empresa.FirstOrDefault().RucEmpresaPropietaria);
                rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count());




                crystalReportViewer1.ReportSource = rp;
                crystalReportViewer1.Show();
            }
                , TaskScheduler.FromCurrentSynchronizationContext());
        }

        #endregion
        #region Controles Botones
        private void BtnVuisualizar_Click(object sender, EventArgs e)
        {
            List<string> Filters = new List<string>();
            string _strFilterExpression = string.Empty;

            try
            {
                if (uvDatos.Validate(true, false).IsValid)
                {

                    var H = cboAgrupar.Text;
                    if (cboEstablecimiento.Value.ToString() == "-1")
                    {
                        _strFilterExpression = string.Empty;
                    }
                    else
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

                    }

                    CargarReporte((int)Common.Resource.TipoDeMovimiento.NotadeIngreso, DateTime.Parse(dtpFechaRegistroDe.Text + " 00:00"), DateTime.Parse(dtpFechaRegistroAl.Text + " 23:59"), _strFilterExpression, TxtProducto.Text.Trim(), cboLinea.Value.ToString(), int.Parse(cboMotivo.Value.ToString()), TxtProveedor.Text.Trim(), TxtPedido.Text.Trim(), cboOrden.Value.ToString(), int.Parse(cboMoneda.Value.ToString()));
                }
                else
                {
                    UltraMessageBox.Show("Por favor llene los campos requeridos para visualizar el reporte", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch
            {

                UltraMessageBox.Show("Ocurrió un error al realizar Reporte Listado de Ingresos(Analítico) ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }
        private void btnBuscarProducto_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            Mantenimientos.frmBuscarProducto frm = new Mantenimientos.frmBuscarProducto(-1, "REPORTE", null, null);
            frm.ShowDialog();

            if (frm._IdProducto != null)
            {
                TxtProducto.Text = frm._CodigoInternoProducto;
            }
            else
            {
                TxtProducto.Text = string.Empty;
            }
        }
        private void btnBuscarProveedor_Click(object sender, EventArgs e)
        {

            Mantenimientos.frmBuscarProveedor frm = new Mantenimientos.frmBuscarProveedor("", "");
            frm.ShowDialog();
            if (frm._IdProveedor != null)
            {
                TxtProveedor.Text = frm._CodigoProveedor;
            }
            else
            {
                TxtProveedor.Text = string.Empty;
            }
        }
        #endregion
        #region Comportamiento de Controles
        private void cboEstablecimiento_SelectedIndexChanged(object sender, EventArgs e)
        {
            _whereAlmacenesConcatenados = string.Empty;
            _AlmacenesConcatenados = string.Empty;
            EstablecimientoBL objEstablecimientoBL = new EstablecimientoBL();
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
        #endregion
        #region Prodecimientos/Funciones
        private void LimpiarFiltros()
        {
            cboEstablecimiento.Value = "-1";
        }
        #endregion

        private void frmListadoIngresoAlmacenAnalitico_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }

        private void dtpFechaRegistroAl_ValueChanged(object sender, EventArgs e)
        {

            dtpFechaRegistroDe.MaxDate = dtpFechaRegistroAl.Value;
        }
    }
}
