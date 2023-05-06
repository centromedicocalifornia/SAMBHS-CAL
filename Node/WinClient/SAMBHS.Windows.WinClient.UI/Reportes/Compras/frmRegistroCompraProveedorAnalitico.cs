#region  Name Space
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
using SAMBHS.Compra.BL;
using SAMBHS.Venta.BL;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Almacen.BL;
using System.Text.RegularExpressions;
using Infragistics.Win.UltraWinMaskedEdit;
using CrystalDecisions.CrystalReports.Engine;
using System.Data.Sql;
using System.Linq.Dynamic;
using System.Data.SqlClient;
using System.Configuration;
using SAMBHS.Security.BL;
using CrystalDecisions.Shared;
using System.Reflection;
using System.Threading;
#endregion
namespace SAMBHS.Windows.WinClient.UI.Reportes.Compras
{
    public partial class frmRegistroCompraProveedorAnalitico : Form
    {

        #region Declaraciones / Referencias
        VendedorBL _objVendedorBL = new VendedorBL();
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        string _strFilterExpression;
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        EstablecimientoBL objEstablecimientoBL = new EstablecimientoBL();
        NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL();
        LineaBL _objLineaBL = new LineaBL();
        MarcaBL _objMarcaBL = new MarcaBL();
        CancellationTokenSource _cts = new CancellationTokenSource();
        string strOrderExpression;
        string strGrupollave, strGrupollave2;
        string strNombreGrupollave, strNombreGrupollave2;
        List<string> Grupollave = new List<string>();
        List<string> NombreGrupollave = new List<string>();
        public int ConsideraDocInternos = -1;
        #endregion
        #region Carga Load
        public frmRegistroCompraProveedorAnalitico(string Modalidad)
        {
            InitializeComponent();
            ConsideraDocInternos = Modalidad == Constants.ModuloContable ? 0 : 1;

        }
        #endregion
        #region Carga Reporte
        private void CargarReporte(DateTime FechaRegistroIni, DateTime FechaRegistroFin, int IdTipoDocumento, string FechaHoraImpresion, string IdProveedor, string NroCuenta, string Orden)
        {


            OperationResult objOperationResult = new OperationResult();

            datahierarchyDto __datahierarchyDto = new datahierarchyDto();
            List<KeyValueDTO> _ListadoGrupos = new List<KeyValueDTO>();
            List<datahierarchyDto> _datahierarchyDto = new List<datahierarchyDto>();
            List<string> Retonar = new List<string>();
            List<string> Retonar2 = new List<string>();
            var rp = new Reportes.Compras.crRegistroCompraProveedorAnalitico();
            strOrderExpression = "";
            _ListadoGrupos = _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 76, null);
            string Seccion;
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


            strOrderExpression = cboOrden.Value.ToString().Trim();
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
            List<ReporteRegistroCompraProveedorAnalitico> aptitudeCertificate = new List<ReporteRegistroCompraProveedorAnalitico>();
            OcultarMostrarBuscar(true);
            Cursor.Current = Cursors.WaitCursor;

            Task.Factory.StartNew(() =>
            {

                aptitudeCertificate = new ComprasBL().ReporteRegistroCompraProveedorAnalitico(ref objOperationResult, 0, FechaRegistroIni, FechaRegistroFin, IdTipoDocumento, IdProveedor, NroCuenta, strOrderExpression, strGrupollave, strNombreGrupollave, ConsideraDocInternos, cboMarca.Value.ToString(), cboLinea.Value.ToString(),TxtProducto.Text.Trim ());
            }
                , _cts.Token)
            .ContinueWith(t =>
            {
                if (_cts.IsCancellationRequested) return;
                OcultarMostrarBuscar(false);
                Cursor.Current = Cursors.Default;
                if (objOperationResult.Success == 0)
                {
                    if (!string.IsNullOrEmpty(objOperationResult.ExceptionMessage))
                    {
                        UltraMessageBox.Show("Ocurrió un error al realizar Reporte de Compras por Proveedor Analitico", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show("Ocurrió un error al realizar Reporte de Compras por Proveedor Analitico", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }


                var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();
                DataSet ds1 = new DataSet();

                DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
                DataTable dt2 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate2);

                ds1.Tables.Add(dt2);
                ds1.Tables.Add(dt);

                ds1.Tables[0].TableName = "dsEmpresa";
                ds1.Tables[1].TableName = "dsRegistroCompraProveedorAnalitico";
                rp.SetDataSource(ds1);

                rp.SetParameterValue("FechaHoraImpresion", FechaHoraImpresion);
                rp.SetParameterValue("Fecha", "DEL " + FechaRegistroIni.Date.Day.ToString("00") + "/" + FechaRegistroIni.Date.Month.ToString("00") + "/" + FechaRegistroIni.Date.Year.ToString() + " AL " + FechaRegistroFin.Date.Day.ToString("00") + "/" + FechaRegistroFin.Date.Month.ToString("00") + "/" + FechaRegistroFin.Date.Year.ToString());
                rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count());
                rp.SetParameterValue("NombreEmpresa", aptitudeCertificate2.FirstOrDefault().NombreEmpresaPropietaria.Trim());
                rp.SetParameterValue("RucEmpresa", "R.U.C. : " + aptitudeCertificate2.FirstOrDefault().RucEmpresaPropietaria.Trim());

                crystalReportViewer1.ReportSource = rp;
                crystalReportViewer1.Show();

                crystalReportViewer1.Zoom(110);
            }
                , TaskScheduler.FromCurrentSynchronizationContext());
        }
        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Reporte de Compras por Proveedor Analitico"
                : @"Reporte de Compras por Proveedor Analitico";
            pBuscando.Visible = estado;
            BtnReporte.Enabled = !estado;
        }
        #endregion
        #region Carga Load
        private void frmRegistroCompraProveedorAnalitico_Load(object sender, EventArgs e)
        {

            this.BackColor = new GlobalFormColors().FormColor;
            CargarCombos();

        }
        private void CargarCombos()
        {

            OperationResult objOperationResult = new OperationResult();
            dtpFechaRegistroDe.MinDate = DateTime.Parse((Globals.ClientSession.i_Periodo.ToString() + "/" + 01 + "/01").ToString());
            dtpFechaRegistroDe.MaxDate = DateTime.Parse((Globals.ClientSession.i_Periodo.ToString() + "/" + 12 + "/" + (DateTime.DaysInMonth(2015, 1)).ToString()).ToString());
            dtpFechaRegistroAl.MinDate = DateTime.Parse((Globals.ClientSession.i_Periodo.ToString() + "/" + 01 + "/01").ToString());
            dtpFechaRegistroAl.MaxDate = DateTime.Parse((Globals.ClientSession.i_Periodo.ToString() + "/" + 12 + "/" + (DateTime.DaysInMonth(2015, 1)).ToString()).ToString());
            Utils.Windows.LoadUltraComboEditorList(cboDetalleDocumento, "Value1", "Id", _objDocumentoBL.ObtenDocumentosParaCombo(ref objOperationResult, null, 1, 0), DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboOrden, "Value1", "Value2", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 75, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAgrupar, "Value1", "Value2", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 76, null).Where (x=>x.Id!="2").ToList (), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboLinea, "Value1", "Id", _objLineaBL.LlenarComboLinea(ref objOperationResult, "v_CodLinea"), DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboMarca, "Value1", "Id", _objMarcaBL.LlenarComboMarca(ref objOperationResult, "v_CodMarca", "-1"), DropDownListAction.All);

            cboAgrupar.SelectedIndex = 2;
            cboOrden.SelectedIndex = 2;
            cboDetalleDocumento.Value = "-1";
            cboLinea.Value = "-1";
            cboMarca.Value = "-1";
        }
        #endregion
        #region Controles Botones
        private void BtnReporte_Click(object sender, EventArgs e)
        {

            try
            {
                if (uvDatos.Validate(true, false).IsValid)
                {

                    if (cboAgrupar.SelectedIndex  == 0)
                    {
                        UltraMessageBox.Show("Seleccione los campos Obligatorios", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        cboAgrupar.Focus();
                        return;

                    }
                    string ValorIdTipoDocumento, strOrderExpression;
                    List<string> Filters = new List<string>();
                    ValorIdTipoDocumento = cboDetalleDocumento.Value.ToString() == Constants.SelectValue ? cboDetalleDocumento.Value.ToString() : cboDetalleDocumento.Value.ToString();

                    _strFilterExpression = null;
                    strOrderExpression = cboOrden.Value.ToString();

                    if (Filters.Count > 0)
                    {
                        foreach (string item in Filters)
                        {
                            _strFilterExpression = _strFilterExpression + item + " && ";
                        }
                        _strFilterExpression = _strFilterExpression.Substring(0, _strFilterExpression.Length - 4);
                    }

                    CargarReporte(DateTime.Parse(dtpFechaRegistroDe.Text + " 00:00"), DateTime.Parse(dtpFechaRegistroAl.Text + " 23:59"), int.Parse(cboDetalleDocumento.Value.ToString()), chkHoraimpresion.Checked == true ? "1" : "0", txtProveedor.Text.Trim(), TxtNrocuenta.Text, strOrderExpression);

                }
                else
                {
                    UltraMessageBox.Show("Por favor llene los campos requeridos para visualizar el reporte", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

            }
            catch
            {
                UltraMessageBox.Show("Se produjo un error  al mostrar el reporte ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }
        private void btnBuscarCuenta_Click(object sender, EventArgs e)
        {
            if (TxtNrocuenta.Text == "") return;
            Mantenimientos.frmPlanCuentasConsulta frm = new Mantenimientos.frmPlanCuentasConsulta(TxtNrocuenta.Text.Trim());
            frm.ShowDialog();
            if (frm._NroSubCuenta != null)
            {
                TxtNrocuenta.Text = frm._NroSubCuenta;

            }
        }
        private void BtnBuscarProducto_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            Mantenimientos.frmBuscarProducto frm = new Mantenimientos.frmBuscarProducto(0, null, null, "");
            frm.ShowDialog();

            if (frm._IdProducto != null)
            {
                TxtProducto.Text = frm._CodigoInternoProducto;
            }
        }


        #endregion
        #region Comportamiento Controles


        private void ChckProveedor_CheckedChanged(object sender, EventArgs e)
        {

            //if (ChckProveedor.Checked == true)
            //{
            //    groupBox3.Enabled = true;
            //    TxtProveedor.Enabled = true;
            //    TxtProveedor.Text = "";
            //    RbtnRuc.Checked = true;
            //    TxtCodigo.Enabled = false;
            //    TxtCodigo.Text = "";
            //}
            //else
            //{
            //    groupBox3.Enabled = false;
            //    TxtProveedor.Enabled = false;
            //    TxtProveedor.Text = "";
            //    RbtnRuc.Checked = true;
            //    TxtCodigo.Enabled = false;
            //    TxtCodigo.Text = "";

            //}
        }


        #endregion


        private void frmRegistroCompraProveedorAnalitico_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
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

        private void txtProveedor_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            Mantenimientos.frmBuscarProveedor frm = new Mantenimientos.frmBuscarProveedor("", "");
            frm.ShowDialog();
            if (frm._IdProveedor != null)
            {
                txtProveedor.Text = frm._CodigoProveedor.Trim ();
            }
        }



    }
}
