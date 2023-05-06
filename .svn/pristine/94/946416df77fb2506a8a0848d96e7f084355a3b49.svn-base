using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL ;
using SAMBHS.Compra.BL ;
using SAMBHS.Common.BL;
using SAMBHS.Venta.BL;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Venta.BL;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Pago
{
    public partial class frmReportePlanillaPagos : Form
    {

        List<KeyValueDTO> _ListadoCobranzas = new List<KeyValueDTO>();
        List<GridKeyValueDTO> _ListadoComboDocumentos = new List<GridKeyValueDTO>();
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        List<GridKeyValueDTO> _ListadoComboDocumentosCobranza = new List<GridKeyValueDTO>();
        ClienteBL _objClienteBL = new ClienteBL();
        public frmReportePlanillaPagos(string pstrParametro)
        {
            InitializeComponent();
        }

        private void ultraExpandableGroupBox1_ExpandedStateChanged(object sender, EventArgs e)
        {
            if (ultraExpandableGroupBox1.Expanded == true)
            {
                ultraGroupBox1.Location = new Point(ultraGroupBox1.Location.X, ultraExpandableGroupBox1.Location.Y + ultraExpandableGroupBox1.Height + 5);
                ultraGroupBox1.Height = this.Height - ultraGroupBox1.Location.Y - 7;
            }
            else
            {
                ultraGroupBox1.Location = new Point(ultraGroupBox1.Location.X, ultraExpandableGroupBox1.Location.Y + ultraExpandableGroupBox1.Height + 5);
                ultraGroupBox1.Height = this.Height - ultraGroupBox1.Location.Y - 7;
            }
        }

        private void frmReportePlanillaPagos_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            ValidarFechas();
            CargarCombos();
        }

        private void ValidarFechas()
        {
            string Periodo = Globals.ClientSession.i_Periodo.ToString();
            string Mes = DateTime.Today.Month.ToString();
            if (DateTime.Now.Year.ToString().Trim() == Periodo)
            {
                dtpFecha.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/" + "01").ToString());
                dtpFecha.MaxDate = DateTime.Parse((Periodo + "/" + Mes + "/" + (DateTime.DaysInMonth(int.Parse(Periodo), int.Parse(Mes))).ToString()).ToString());
                dtpFecha.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());

                dtpFechaAl.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/" + "01").ToString());
                dtpFechaAl.MaxDate = DateTime.Parse((Periodo + "/" + Mes + "/" + (DateTime.DaysInMonth(int.Parse(Periodo), int.Parse(Mes))).ToString()).ToString());
                dtpFechaAl.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());

            }
            else
            {
                dtpFecha.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/01").ToString());
                dtpFecha.MaxDate = DateTime.Parse((Periodo + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(Periodo), 12)).ToString()).ToString());
                dtpFecha.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                dtpFechaAl.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/01").ToString());
                dtpFechaAl.MaxDate = DateTime.Parse((Periodo + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(Periodo), 12)).ToString()).ToString());
                dtpFechaAl.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());

            }
        }

        private void CargarCombos()
        {

            OperationResult objOperationResult = new OperationResult();
            _ListadoComboDocumentosCobranza = _objDocumentoBL.ObtenDocumentosCobranzaParaComboGrid(ref objOperationResult, null);
            _ListadoComboDocumentos = _objDocumentoBL.ObtenDocumentosParaComboGrid(ref objOperationResult, null, 0, 1);
            Utils.Windows.LoadUltraComboList(cboComprobante, "Value2", "Id", _ListadoComboDocumentosCobranza, DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(cboTipoDocumento, "Value2", "Id", _ListadoComboDocumentos, DropDownListAction.Select);
            cboAgrupar.Text = "SIN AGRUPAR";
            cboTipoDocumento.Value = "-1";
            cboOrden.Text = "FECHA";
            cboComprobante.Value = "-1";

        }

        private void BtnVuisualizar_Click(object sender, EventArgs e)
        {
            try
            {

                if (uvValidar.Validate(true, false).IsValid)
                {
                    string ValorIdCliente;
                    int idTipoDocumentoCobranzaDetalle;
                    var rp = new Reportes.Cobranza.crReportePlanillaCobranza();
                    List<string> Filters = new List<string>();
                    ValorIdCliente = txtCodigoProveedor.Text.Trim() == "" ? "" : txtCodigoProveedor.Text.Trim();
                    idTipoDocumentoCobranzaDetalle = int.Parse(cboComprobante.Value.ToString());
                    string Orden= cboOrden.Text =="PROVEEDOR"?"NombreProveedor" :"FechaEmision";
                    using (new LoadingClass.PleaseWait(this.Location, "Generando Reporte..."))

                        CargarReporte(dtpFecha.Value.Date, dtpFechaAl.Value.Date, idTipoDocumentoCobranzaDetalle,Orden, txtCodigoProveedor.Text.Trim(), txtSerie.Text.Trim());
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



        private void CargarReporte(DateTime fechaDesde, DateTime fechaHasta, int? idTipoDocumentoCobranzaDetalle, string orden, string IdCliente, string Serie)
        {

            OperationResult objOperationResult = new OperationResult();
            var rp = new Reportes.Pago.crReportePlanillaPagos();
            var ReportePlanillasPagos = new PagoBL().ReportePlanillaPagos(fechaDesde, fechaHasta, idTipoDocumentoCobranzaDetalle, orden, IdCliente, Serie, cboAgrupar.Text, txtCorrelativo.Text.Trim () , int.Parse (cboTipoDocumento.Value.ToString( ))); 
            var Empresa = new NodeBL().ReporteEmpresa();
            DataSet ds1 = new DataSet();
            DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(ReportePlanillasPagos);
            ds1.Tables.Add(dt);
            ds1.Tables[0].TableName = "dsReportePlanillaPagos";
            rp.SetDataSource(ds1);
            rp.SetParameterValue("NombreEmpresa", Empresa.FirstOrDefault().NombreEmpresaPropietaria.Trim());
            rp.SetParameterValue("RucEmpresa", Empresa.FirstOrDefault().RucEmpresaPropietaria.Trim());
            rp.SetParameterValue("NroRegistros", ReportePlanillasPagos.Count());
            rp.SetParameterValue("FechaReporte", "DEL " + dtpFecha.Value.Date.Day.ToString() + "/" + dtpFecha.Value.Date.Month.ToString() + "/" + dtpFecha.Value.Date.Year.ToString() + " AL " + dtpFechaAl.Value.Date.Day.ToString() + "/" + dtpFechaAl.Value.Date.Month.ToString() + "/" + dtpFechaAl.Value.Date.Year.ToString());
             rp.SetParameterValue ("SubTotalGrupo","SUB-TOTAL " + cboAgrupar.Text.Trim() +" :  ");
            
            crystalReportViewer1.ReportSource = rp;
            crystalReportViewer1.Show();
        }

        private void txtCodigo_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscarProveedor")
            {
                OperationResult objOperationResult = new OperationResult();
                Mantenimientos.frmBuscarProveedor frm = new Mantenimientos.frmBuscarProveedor("", "RUC");
                frm.ShowDialog();
                if (frm._IdProveedor != null)
                {
                    txtRazonSocial.Text = frm._RazonSocial.Trim().ToUpper();
                    txtCodigoProveedor.Text = frm._CodigoProveedor.ToUpper();
                }
                else
                {

                }
            }
        }

        private void txtCodigoProveedor_TextChanged(object sender, EventArgs e)
        {
            if (txtCodigoProveedor.Text == string.Empty)
            {
                txtRazonSocial.Clear();
            }
        }

        private void txtCodigoProveedor_Validated(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (txtCodigoProveedor.Text.Trim() != string.Empty)
            {
                var Proveedor = _objClienteBL.ObtenerClienteCodigoBandejasCodigo(ref objOperationResult, txtCodigoProveedor.Text.Trim(), "V");
                if (Proveedor != null)
                {
                    txtRazonSocial.Text = (Proveedor.v_ApePaterno + " " + Proveedor.v_ApeMaterno.Trim() + " " + Proveedor.v_PrimerNombre.Trim() + " " + Proveedor.v_SegundoNombre.Trim() + " " + Proveedor.v_RazonSocial.Trim()).Trim().ToUpper();
                    txtCodigoProveedor.Text = Proveedor.v_CodCliente.Trim();
                }
                else
                {
                    txtRazonSocial.Clear();
                    txtCodigoProveedor.Clear();
                }

            }
            else
            {
                txtRazonSocial.Clear();
                txtCodigoProveedor.Clear();
            }
        }

        private void cboTipoDocumento_AfterDropDown(object sender, EventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboTipoDocumento.Rows)
            {
                if (cboTipoDocumento.Value == "-1") cboTipoDocumento.Text = string.Empty;
                bool filterRow = true;
                foreach (UltraGridColumn column in cboTipoDocumento.DisplayLayout.Bands[0].Columns)
                {
                    if (column.IsVisibleInLayout)
                    {
                        if (row.Cells[column].Text.Contains(cboTipoDocumento.Text.ToUpper()))
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

        private void cboTipoDocumento_KeyUp(object sender, KeyEventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboTipoDocumento.Rows)
            {
                if (cboTipoDocumento.Value == "-1") cboTipoDocumento.Text = string.Empty;
                bool filterRow = true;
                foreach (UltraGridColumn column in cboTipoDocumento.DisplayLayout.Bands[0].Columns)
                {
                    if (column.IsVisibleInLayout)
                    {
                        if (row.Cells[column].Text.Contains(cboTipoDocumento.Text.ToUpper()))
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

        private void cboTipoDocumento_Leave(object sender, EventArgs e)
        {
            if (cboTipoDocumento.Text.Trim() == "")
            {
                cboTipoDocumento.Value = "-1";
            }
            else
            {
                var x = _ListadoComboDocumentos.Find(p => p.Id == cboTipoDocumento.Value.ToString() | p.Id == cboTipoDocumento.Text.Trim());
                if (x == null)
                {
                    cboTipoDocumento.Value = "-1";
                }
            }
        }

        private void cboComprobante_AfterDropDown(object sender, EventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboComprobante.Rows)
            {
                if (cboComprobante.Value == "-1") cboComprobante.Text = string.Empty;
                bool filterRow = true;
                foreach (UltraGridColumn column in cboComprobante.DisplayLayout.Bands[0].Columns)
                {
                    if (column.IsVisibleInLayout)
                    {
                        if (row.Cells[column].Text.Contains(cboComprobante.Text.ToUpper()))
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

        private void cboComprobante_KeyUp(object sender, KeyEventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboComprobante.Rows)
            {
                if (cboComprobante.Value == "-1") cboComprobante.Text = string.Empty;
                bool filterRow = true;
                foreach (UltraGridColumn column in cboComprobante.DisplayLayout.Bands[0].Columns)
                {
                    if (column.IsVisibleInLayout)
                    {
                        if (row.Cells[column].Text.Contains(cboComprobante.Text.ToUpper()))
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

        private void cboComprobante_Leave(object sender, EventArgs e)
        {
            if (cboComprobante.Text.Trim() == "")
            {
                cboComprobante.Value = "-1";
            }
            else
            {
                var x = _ListadoComboDocumentosCobranza.Find(p => p.Id == cboComprobante.Value.ToString() | p.Id == cboComprobante.Text.Trim());
                if (x == null)
                {
                    cboComprobante.Value = "-1";
                }
            }
        }
    }
}
