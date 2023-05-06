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
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using SAMBHS.Cobranza.BL;
using SAMBHS.Common.BL;
using SAMBHS.Venta.BL;
using SAMBHS.Common.BE;
using System.Threading.Tasks;
using System.Threading;
using SAMBHS.Letras.BL;
using System.IO;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Letras.LetrasCobrar
{
    public partial class frmLetrasCanceladasRenovadas : Form
    {

        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        ClienteBL _objClienteBL = new ClienteBL();
        CancellationTokenSource _cts = new CancellationTokenSource();
        public frmLetrasCanceladasRenovadas(string pstrParametro)
        {
            InitializeComponent();
        }

        private void txtCliente_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscarCliente")
            {
                OperationResult objOperationResult = new OperationResult();
                Mantenimientos.frmBuscarCliente frm = new Mantenimientos.frmBuscarCliente("VV", txtCliente.Text.Trim());
                frm.ShowDialog();

                if (frm._IdCliente != null)
                {
                    txtCliente.Text = frm._CodigoCliente.Trim().ToUpper();
                   
                }
             
            }
        }

        private void frmReporteEstadoCuentaCliente_Load(object sender, EventArgs e)
        {

            this.BackColor = new GlobalFormColors().FormColor;
            CargarCombos();

            uckRangoFecha.Checked = false;
            object senderFechas = new object();
            EventArgs eFechas = new EventArgs();
            uckRangoFecha_CheckedChanged(senderFechas, eFechas);
           // ValidarFechas();

        }




        private void ValidarFechas()
        {
            string Periodo = Globals.ClientSession.i_Periodo.ToString();
            string Mes = DateTime.Today.Month.ToString();
            if (DateTime.Now.Year.ToString().Trim() == Periodo)
            {
                dtpFechaDesde.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/" + "01").ToString());
                
                dtpFechaDesde.MaxDate = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());
           
            }
            else
            {
                dtpFechaDesde.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/01").ToString());
                dtpFechaDesde.MaxDate = DateTime.Parse((Periodo + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(Periodo), 12)).ToString()).ToString());
                //dtpFecha.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());
            }

            dtpFechaDesde.Value = DateTime.Parse((Periodo + "/" + " 01/01").ToString());
        }

        private void CargarCombos()
        {
            OperationResult objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList(cboEstadoLetra, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 110, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(cboUbicacionLetra, "Value1", "Id", new DocumentoBL().ObtenDocumentosCobranzaParaComboGrid(ref objOperationResult, null), DropDownListAction.Select);
            cboEstadoLetra.Value = "-1";
            cboUbicacionLetra.Value = "-1";
            cboAgrupar.SelectedIndex = 1;
            cboOrdenar.SelectedIndex = 1;
        }

        private void btnVisualizar_Click(object sender, EventArgs e)
        {
            Reporte(false);
        }


        private void Reporte(bool  Excel)
        {
            try
            {
                if (uvReporte.Validate(true, false).IsValid)
                {
                    CargarReporte(Excel);
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

        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Letras canceladas y renovadas"
                : @"Letras canceladas y renovadas";
            pBuscando.Visible = estado;
            btnVisualizar.Enabled = !estado;

        }

        private void CargarReporte(bool Export)
        {
            OperationResult objOperationResult = new OperationResult();
            DateTime FechaDesde = dtpFechaDesde.Value.Date;
            DateTime FechaHasta = DateTime.Parse(dtpFechaHasta.Text + " 23:59"); 
            List<string> Retonar = new List<string>();
            List<string> Retonar2 = new List<string>();
            var rp = new Reportes.Letras.LetrasCobrar.crReporteLetrasCobrarEmitidas();
            List<ReporteLetrasCobrarEmitidas> aptitudeCertificate = new List<ReporteLetrasCobrarEmitidas>();


            OcultarMostrarBuscar(true);
            Cursor.Current = Cursors.WaitCursor;

            Task.Factory.StartNew(() =>
            {

                aptitudeCertificate = new LetrasBL().ReporteLetrasCobrarEmitidas(ref objOperationResult, txtCliente.Text.Trim(), FechaDesde, FechaHasta, cboAgrupar.Value.ToString(), uckMostrarSoloCanceladas.Checked ? true : false, uckRangoFecha.Checked ? true : false, cboEstadoLetra.Value.ToString() != "-1" ? cboEstadoLetra.Text : "-1", cboUbicacionLetra.Value.ToString() != "-1" ? cboUbicacionLetra.Text : "-1",cboOrdenar.Value.ToString ());
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
                        UltraMessageBox.Show("Ocurrió un error al realizar reporte \n. Información adicional : " + objOperationResult.ErrorMessage + "\n" + objOperationResult.ExceptionMessage + "\n" + objOperationResult.AdditionalInformation, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    
                   
                    }
                    else
                    {
                        UltraMessageBox.Show("Ocurrió un error al realizar reporte \n. Información adicional : " + objOperationResult.ErrorMessage + "\n" + objOperationResult.ExceptionMessage + "\n" + objOperationResult.AdditionalInformation, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }

                var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();
                DataSet ds1 = new DataSet();
                DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
                DataTable dt2 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate2);


                if (Export)
                {

                    #region Headers
                    var columnas = new[]
                    {
                        "NumeroLetra", "NroRegistro", "Estado", "Cliente", "sFechaEmision",
                        "TotalDias", "sFechaVencimiento", "ImporteSoles", "Moneda","FechaPago","NroCobranza","MontoCobrado","MonedaCobranza","Ubicacion","NroUnico"
                    };
                    var heads = new ExcelHeader[]
                    {
                        "NRO. LETRA", "NUM. REGISTRO", "ESTADO", "CLIENTE", "FECHA EMISIÓN", "TOTAL DIAS", "FECHA VENC.", "IMPORTE", "M", "FECHA PAGO","NRO. COBRANZA","IMPORTE COBRADO","M","UBICACIÓN","NRO. ÚNICO"
                    };
                    #endregion

                    var objexcel = new ExcelReport(dt) { Headers = heads };
                    objexcel.AutoSizeColumns(1, 20,20, 20, 50, 15, 15, 15, 15, 15, 15,15,15,15,30,15);
                    objexcel.SetTitle("REPORTE LETRAS CANCELADAS Y RENOVADAS");
                    objexcel.SetHeaders();
                    //objexcel.EndSection += (_, e) =>
                    //{
                    //    var obj = (ExcelReport)_;
                    //    obj.SetFormulas(6, "SUB TOTAL:", Enumerable.Range(0, 3).Select(i => string.Format("=SUM(${2}{0}:${2}{1})", e.StartPosition + 1, e.EndPosition, (char)('H' + i))).ToArray());
                    //    obj.CurrentPosition++;
                    //};
                    objexcel.SetData( ref objOperationResult ,columnas);
                    var path = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + @".xlsx");
                    objexcel.Generate(path);
                    System.Diagnostics.Process.Start(path);

                }
                else
                {

                    ds1.Tables.Add(dt);
                    ds1.Tables.Add(dt2);
                    ds1.Tables[0].TableName = "dsReporteLetrasCobrarEmitidas";
                    rp.SetDataSource(ds1);
                    rp.SetParameterValue("FechaReporte", "DEL " + FechaDesde.Date.Day.ToString("00") + "/" + FechaDesde.Date.Month.ToString("00") + "/" + FechaDesde.Date.Year.ToString() + "   AL   " + FechaHasta.Date.Day.ToString("00") + "/" + FechaHasta.Date.Month.ToString("00") + "/" + FechaHasta.Date.Year.ToString());
                    rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count());
                    rp.SetParameterValue("NombreEmpresa", aptitudeCertificate2.FirstOrDefault().NombreEmpresaPropietaria.Trim());
                    rp.SetParameterValue("RucEmpresa", "R.U.C. : " + aptitudeCertificate2.FirstOrDefault().RucEmpresaPropietaria.Trim());
                    rp.SetParameterValue("SubTotalLetras", "SUB-TOTAL : ");
                    rp.SetParameterValue("TotalLetras", "TOTAL :  ");
                    rp.SetParameterValue("Agrupamiento", cboAgrupar.Text.Trim());
                    rp.SetParameterValue("RangoFecha", uckRangoFecha.Checked ? true : false);
                    #region Redimensionando las columas deacuerdo al agrupamiento
                    ReportObject ColumnaOculta;
                    ReportObject ColumnaOculta1;
                    ReportObject ColumnaRedimensionada;
                    switch (cboAgrupar.Text)
                    {
                        case "CLIENTE":
                            var Header = rp.ReportDefinition.ReportObjects["Text4"];
                            // ColumnaOculta = rp.ReportDefinition.ReportObjects["CodigoCliente1"];
                            ColumnaOculta1 = rp.ReportDefinition.ReportObjects["Cliente1"];
                            ColumnaRedimensionada = rp.ReportDefinition.ReportObjects["Estado1"];
                            ColumnaRedimensionada.Width = ColumnaRedimensionada.Width + ColumnaOculta1.Width;
                            Header.Width = ColumnaRedimensionada.Width;
                            break;

                        case "UBICACIÓN":
                            var Header1 = rp.ReportDefinition.ReportObjects["Text12"];
                            ColumnaOculta = rp.ReportDefinition.ReportObjects["Ubicacion1"];
                            ColumnaRedimensionada = rp.ReportDefinition.ReportObjects["ImporteDolares1"];
                            //ColumnaRedimensionada.Width = ColumnaRedimensionada.Width + ColumnaOculta.Width;
                            //Header1.Width = ColumnaRedimensionada.Width;
                            break;

                        case "FEC. VENCIMIENTO":
                            var Header2 = rp.ReportDefinition.ReportObjects["Text7"];
                            ColumnaOculta = rp.ReportDefinition.ReportObjects["sFechaVencimiento1"];
                            ColumnaRedimensionada = rp.ReportDefinition.ReportObjects["sFechaEmision1"];
                            ColumnaRedimensionada.Width = ColumnaRedimensionada.Width + ColumnaOculta.Width;
                            Header2.Width = ColumnaRedimensionada.Width;
                            break;
                    }


                    #endregion
                    crystalReportViewer1.ReportSource = rp;
                    crystalReportViewer1.Show();
                }
            }
                , TaskScheduler.FromCurrentSynchronizationContext());



        }

        private void txtCliente_Validated(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            //if (txtCliente.Text.Trim() != string.Empty)
            //{
            //    var Cliente = _objClienteBL.ObtenerClienteCodigoBandejasCodigo(ref objOperationResult, txtCliente.Text.Trim(), "C");
            //    if (Cliente != null)
            //    {
            //        txtRazonSocial.Text = (Cliente.v_ApePaterno + " " + Cliente.v_ApeMaterno.Trim() + " " + Cliente.v_PrimerNombre.Trim() + " " + Cliente.v_SegundoNombre.Trim() + " " + Cliente.v_RazonSocial.Trim()).Trim().ToUpper();
            //    }
            //    else
            //    {
            //        txtRazonSocial.Clear();
            //    }

            //}
            //else
            //{
            //    txtRazonSocial.Clear();
            //}
        }

        private void frmReporteEstadoCuentaCliente_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }

        private void uckRangoFecha_CheckedChanged(object sender, EventArgs e)
        {
            
            dtpFechaDesde.Enabled = uckRangoFecha.Checked;
            dtpFechaHasta.Enabled = uckRangoFecha.Checked;
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            Reporte(true);
        }

       
    }
}
