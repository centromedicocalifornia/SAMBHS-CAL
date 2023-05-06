
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
using SAMBHS.Venta.BL;
using SAMBHS.Cobranza.BL;
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

namespace SAMBHS.Windows.WinClient.UI.Reportes.Cobranza
{
    public partial class frmReporteVentasCobranzas : Form
    {
        public DocumentoBL _objDocumentoBL = new DocumentoBL();
        List<GridKeyValueDTO> _ListadoComboDocumentos = new List<GridKeyValueDTO>();
        PedidoBL _objPedidoBL = new PedidoBL();
        ClienteBL _objClienteBL = new ClienteBL();
        public string v_IdCliente = string.Empty;
        CancellationTokenSource _cts = new CancellationTokenSource();
        public frmReporteVentasCobranzas(string parametro)
        {
            InitializeComponent();
        }

        public void frmReporteVentasCobranzas_Load(object sender, EventArgs e)
        {
            List<ComboAgrupado> CombitoAgrupado = new List<ComboAgrupado>();

            this.BackColor = new GlobalFormColors().FormColor;
            OperationResult objOperationResult = new OperationResult();
            _ListadoComboDocumentos = _objDocumentoBL.ObtenDocumentosCobranzaParaComboGrid(ref objOperationResult, null);
            Utils.Windows.LoadUltraComboList(cboTipoDocumento, "Value2", "Id", _ListadoComboDocumentos, DropDownListAction.Select);
            cboTipoDocumento.Value = "-1";



        }

        private void frmReporteVentasCobranzas_Load()
        {

        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {

            try
            {

                string ValorIdCliente, strOrderExpression;
                int idTipoDocumentoCobranzaDetalle;
                var rp = new Reportes.Cobranza.crReporteVentaCobranza_();
                List<string> Filters = new List<string>();
                ValorIdCliente = txtCodigo.Text.Trim() == "" ? "" : txtCodigo.Text.Trim();
                strOrderExpression = RbtnNumeroDoc.Checked == true ? "NroDocumento" : "";
                strOrderExpression += RbtnTipoDocumento.Checked == true ? "TipoDocumento" : "";
                idTipoDocumentoCobranzaDetalle = int.Parse(cboTipoDocumento.Value.ToString());
               // using (new LoadingClass.PleaseWait(this.Location, "Generando Reporte..."))
                    CargarReporte(dtpFecha.Value.Date, idTipoDocumentoCobranzaDetalle, strOrderExpression, v_IdCliente);
            }
            catch
            {

                UltraMessageBox.Show("Ocurrio un error", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void CargarReporte(DateTime fechacobranza, int? idTipoDocumentoCobranzaDetalle, string orden, string IdCliente)
        {

            int NroRegistros = 0;
            string parametroDocumentos = "", parametroTiposPago = "", parametroFormaPago = "";
            if (int.Parse(cboTipoDocumento.Value.ToString()) != -1)
            {
                parametroFormaPago = "TOTAL DE " + cboTipoDocumento.Text + ":";
            }

           
            var rp = new Reportes.Cobranza.crReporteVentaCobranza_();
            List<ReporteVentaCobranza> aptitudeCertificate2 = new List<ReporteVentaCobranza>();
            List<TotalCobranzaDto> aptitudeCertificate3 = new List<TotalCobranzaDto>();
            List<CobranzaTipoDocumentoDto> aptitudeCertificate4 = new List<CobranzaTipoDocumentoDto>();
            OperationResult objOperationResult = new OperationResult();
            var aptitudeCertificate1 = new NodeBL().ReporteEmpresa();
            OcultarMostrarBuscar(true);
            Cursor.Current = Cursors.WaitCursor;

            Task.Factory.StartNew(() =>
            {



                aptitudeCertificate2 = new CobranzaBL().ReporteVentasCobranzas(ref objOperationResult, fechacobranza, idTipoDocumentoCobranzaDetalle, orden, IdCliente);
                aptitudeCertificate3 = new CobranzaBL().ReporteVentasCobranzasResumen(ref objOperationResult, fechacobranza, idTipoDocumentoCobranzaDetalle, orden, IdCliente);
                aptitudeCertificate4 = new CobranzaBL().ReporteVentasCobranzasResumenDocumentos(ref objOperationResult, fechacobranza, idTipoDocumentoCobranzaDetalle, orden, IdCliente);

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
                        UltraMessageBox.Show("Ocurrió un Error al realizar Reporte Ventas y Cobranzas" , "Sistema " + " Estado de Cuenta Cliente", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show("Ocurrió un Error al realizar Reporte Ventas y Cobranzas", "Sistema" + " Estado de Cuenta Cliente", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }


                NroRegistros = aptitudeCertificate2.Count() == 0 && aptitudeCertificate3.Count() == 0 && aptitudeCertificate4.Count() == 0 && aptitudeCertificate2 == null && aptitudeCertificate3 == null && aptitudeCertificate4 ==null ? 0 : aptitudeCertificate2.Count();
                DataSet ds1 = new DataSet();

                DataTable dt1 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate1);
                DataTable dt2 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate2);
                DataTable dt3 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate3);
                DataTable dt4 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate4);

                DataRow workRow1;
                workRow1 = dt2.NewRow();
                workRow1["LineaResumen"] = "_______________________________________________________________________________________________________________________________________________"; //LineaResumen
                dt2.Rows.Add(workRow1);



                DataRow workRow2;
                workRow2 = dt2.NewRow();
                workRow2["FormatoDias"] = "                 DEL DIA                                      OTROS DIAS                                                DEL DIA                                  OTROS DIAS";
                dt2.Rows.Add(workRow2);


                DataRow workRowa;
                workRowa = dt2.NewRow();
                workRowa["LineaResumen"] = "_______________________________________________________________________________________________________________________________________________";//lineaResumen
                dt2.Rows.Add(workRowa);


                DataRow workRow3;
                workRow3 = dt2.NewRow();
                workRow3["FormatoMonedas"] = "              S/                      US$.                       S/                     US$.                                   S/                   US$.                   S/              US$.";
                dt2.Rows.Add(workRow3);

                DataRow workRowb;
                workRowb = dt2.NewRow();
                workRowb["LineaResumen"] = "_______________________________________________________________________________________________________________________________________________"; //LineaResumen
                dt2.Rows.Add(workRowb);

                int numeroFilasdt4 = dt4.Rows.Count;
                int i = 0;
                for (i = 0; i <= dt3.Rows.Count - 1; i++)
                {
                    DataRow workRow;
                    workRow = dt2.NewRow();
                    workRow["NroDocumento"] = dt3.Rows[i][22].ToString();
                    workRow["MontoSolesResumen"] = decimal.Parse(dt3.Rows[i][16].ToString()); //SolesDia
                    workRow["MontoDolaresResumen"] = decimal.Parse(dt3.Rows[i][18].ToString());    //DolaresDia
                    workRow["MontoSolesOtroDiaResumen"] = decimal.Parse(dt3.Rows[i][17].ToString());
                    workRow["MontoDolaresOtroDiaResumen"] = decimal.Parse(dt3.Rows[i][19].ToString());

                    if (i < numeroFilasdt4)
                    {
                        workRow["Cheq"] = dt4.Rows[i][8].ToString();
                        workRow["MontoSolesDocumentoResumen"] = decimal.Parse(dt4.Rows[i][0].ToString());
                        workRow["MontoDolaresDocumentoResumen"] = decimal.Parse(dt4.Rows[i][1].ToString());
                        workRow["MontoSolesOtroDiaDocumentoResumen"] = decimal.Parse(dt4.Rows[i][2].ToString());
                        workRow["MontoDolaresOtroDiaDocumentoResumen"] = decimal.Parse(dt4.Rows[i][3].ToString());

                    }
                    dt2.Rows.Add(workRow);
                    parametroTiposPago = "Apacerer";
                    parametroDocumentos = "Aparecer";

                }

                for (int j = i; j <= dt4.Rows.Count - 1; j++)
                {
                    DataRow workRow;
                    workRow = dt2.NewRow();
                    workRow["Cheq"] = dt4.Rows[j][8].ToString();
                    workRow["MontoSolesDocumentoResumen"] = dt4.Rows[j][0].ToString();
                    workRow["MontoDolaresDocumentoResumen"] = dt4.Rows[j][1].ToString();
                    workRow["MontoSolesOtroDiaDocumentoResumen"] = dt4.Rows[j][2].ToString();
                    workRow["MontoDolaresOtroDiaDocumentoResumen"] = dt4.Rows[j][3].ToString();
                    dt2.Rows.Add(workRow);
                    parametroDocumentos = "Aparecer";


                }

                DataRow workRowc;
                workRowc = dt2.NewRow();
                workRowc["LineaResumen"] = "_______________________________________________________________________________________________________________________________________________"; //LineaResumen
                dt2.Rows.Add(workRowc);





                ds1.Tables.Add(dt1);
                ds1.Tables.Add(dt2);

                ds1.Tables[0].TableName = "dsEmpresa";
                ds1.Tables[1].TableName = "dsReporteVentaCobranza";
                rp.SetDataSource(ds1);
                rp.SetParameterValue("FechaBusqueda", "VENTAS Y COBRANZAS DEL " + fechacobranza.Date.Day.ToString("00") + "/" + fechacobranza.Date.Month.ToString("00") + "/" + fechacobranza.Date.Year.ToString());
                rp.SetParameterValue("CantidadDecimalPrecio", (int)Globals.ClientSession.i_PrecioDecimales);
                rp.SetParameterValue("parametroDocumentos", parametroDocumentos);
                rp.SetParameterValue("parametroTiposPago", parametroTiposPago);
                rp.SetParameterValue("parametroFormaPago", parametroFormaPago);
                rp.SetParameterValue("NroRegistros", NroRegistros);
                crystalReportViewer1.ReportSource = rp;
                crystalReportViewer1.Show();
            }
                , TaskScheduler.FromCurrentSynchronizationContext());

        }


        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Reporte Ventas y Cobranzas"
                : @"Reporte Ventas y Cobranzas";
            pBuscando.Visible = estado;

            BtnVuisualizar.Enabled = !estado;
            BtnVuisualizar.Enabled = !estado;

        }

        private void cboTipoDocumento_Leave(object sender, EventArgs e)
        {

            if (cboTipoDocumento.Text.Trim() == "")
            {
                cboTipoDocumento.Value = "-1";
            }
            else
            {
                var x = _ListadoComboDocumentos.Find(p => p.Id == cboTipoDocumento.Value | p.Id == cboTipoDocumento.Text);
                if (x == null)
                {
                    cboTipoDocumento.Value = "-1";
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



        //private void TxtCodigo_KeyDown(object sender, KeyEventArgs e)
        //{
        //    OperationResult objOperationResult = new OperationResult();
        //    if (e.KeyCode == Keys.Enter)
        //    {
        //        if (TxtCodigo.Text.Trim() != "" & TxtCodigo.TextLength <= 7)
        //        {

        //            Mantenimientos.frmBuscarCliente frm = new Mantenimientos.frmBuscarCliente("VV", TxtCodigo.Text.Trim());
        //            frm.ShowDialog();

        //            if (frm._IdCliente != null)
        //            {

        //                v_IdCliente = frm._IdCliente;
        //                TxtCodigo.Text = frm._NroDocumento;
        //                txtRazonSocial.Text = frm._RazonSocial;

        //            }
        //            else
        //            {
        //                txtRazonSocial.Clear();

        //            }
        //        }

        //        else
        //        {

        //            if (TxtCodigo.TextLength == 8 | TxtCodigo.TextLength == 11)
        //            {
        //                string[] DatosCliente = new string[3];
        //                DatosCliente = _objPedidoBL.DevolverClientePorNroDocumento(ref objOperationResult, TxtCodigo.Text.Trim());
        //                if (DatosCliente != null)
        //                {
        //                    v_IdCliente = DatosCliente[0];
        //                    txtRazonSocial.Text = DatosCliente[2];

        //                }
        //            }
        //            else
        //            {

        //                Mantenimientos.frmBuscarCliente frm = new Mantenimientos.frmBuscarCliente("VV", TxtCodigo.Text.Trim());
        //                frm.ShowDialog();

        //                if (frm._IdCliente != null)
        //                {

        //                    v_IdCliente = frm._IdCliente;
        //                    TxtCodigo.Text = frm._NroDocumento;
        //                    txtRazonSocial.Text = frm._RazonSocial;

        //                }
        //                else
        //                {
        //                    txtRazonSocial.Clear();
        //                    v_IdCliente = string.Empty;

        //                }

        //            }
        //        }


        //    }
        //}

        //private void TxtCodigo_TextChanged(object sender, EventArgs e)
        //{
        //    if (TxtCodigo.Text == "")
        //    {
        //        OperationResult objOperationResult = new OperationResult();
        //        // Utils.Windows.LoadDropDownList(cboCliente, "Value1", "Id", objMovimientoBL.BuscarProveedoresParaCombo(ref objOperationResult, txtCliente.Text, "C"), DropDownListAction.All);
        //        v_IdCliente = string.Empty;
        //        txtRazonSocial.Clear();

        //    }
        //}

        //private void btnBuscarCliente_Click(object sender, EventArgs e)
        //{
        //    OperationResult objOperationResult = new OperationResult();
        //    Mantenimientos.frmBuscarCliente frm = new Mantenimientos.frmBuscarCliente("VV", TxtCodigo.Text.Trim());
        //    frm.ShowDialog();
        //    if (frm._IdCliente != null)
        //    {
        //        txtCodigo.Text = frm._NroDocumento;
        //        txtRazonSocial.Text = frm._RazonSocial;
        //        v_IdCliente = frm._IdCliente;

        //    }

        //    else
        //    {
        //        v_IdCliente = string.Empty;
        //        txtRazonSocial.Clear();
        //        txtCodigo.Clear();

        //    }
        //}

        private void txtCodigo_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscarCliente")
            {
                OperationResult objOperationResult = new OperationResult();
                Mantenimientos.frmBuscarCliente frm = new Mantenimientos.frmBuscarCliente("VV", txtCodigo.Text.Trim());
                frm.ShowDialog();

                if (frm._IdCliente != null)
                {
                    txtCodigo.Text = frm._NroDocumento.Trim().ToUpper();
                    txtRazonSocial.Text = frm._RazonSocial.Trim().ToUpper();
                    v_IdCliente = frm._IdCliente.Trim();
                }
                else
                {

                }
            }
        }

        private void txtCodigo_Validated(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (txtCodigo.Text.Trim() != string.Empty)
            {
                var Cliente = _objClienteBL.ObtenerClienteCodigoBandejas(ref objOperationResult, txtCodigo.Text.Trim(), "C");
                if (Cliente != null)
                {
                    txtRazonSocial.Text = (Cliente.v_ApePaterno + " " + Cliente.v_ApeMaterno.Trim() + " " + Cliente.v_PrimerNombre.Trim() + " " + Cliente.v_SegundoNombre.Trim() + " " + Cliente.v_RazonSocial.Trim()).Trim().ToUpper();
                    v_IdCliente = Cliente.v_IdCliente.Trim();
                }
                else
                {
                    txtRazonSocial.Clear();
                    v_IdCliente = string.Empty;
                }

            }
            else
            {
                txtRazonSocial.Clear();
                v_IdCliente = string.Empty;
            }
        }


    }
}
