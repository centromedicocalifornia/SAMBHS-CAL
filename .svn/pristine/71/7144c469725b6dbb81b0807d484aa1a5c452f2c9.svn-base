using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Venta.BL;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using SAMBHS.Common.Resource;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.CommonWIN.BL;
using CrystalDecisions.Windows;
using System.Globalization;


namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    public partial class frmDocumentoBoletaRapida : Form
    {
        #region Properties
        public bool ImpresionExitosa
        {
            get { return impresionExitosa; }
        }
        #endregion

        #region Fields
        private bool impresionExitosa;
        List<string> IdVentas = new List<string>();
        CrystalDecisions.CrystalReports.Engine.ReportDocument rp;
        #endregion

        #region Init
        public frmDocumentoBoletaRapida(List<string> IdVenta)
        {
            InitializeComponent();
            IdVentas = IdVenta;
            ImpresionDirecto();
            FormClosing += frmDocumentoBoletaRapida_FormClosing;
        }

        void frmDocumentoBoletaRapida_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (rp != null)
                rp.Dispose();
        }
        #endregion

        #region Prodecimientos/Funciones
        private void ImpresionDirecto()
        {
            string TotalEnNumero, TotalesenLetras="";
            try
            {

                //this.Visible = false;          
                if (IdVentas != null)
                {
                    foreach (string IdVenta in IdVentas)
                    {
                        var Ruc = new NodeBL().ReporteEmpresa().FirstOrDefault().RucEmpresaPropietaria;
                        rp = ReportesUtils.DevolverReporte(Ruc, TiposReportes.Boleta);
                        if (rp == null)
                        {
                            UltraMessageBox.Show("Reporte no realizado para esta Empresa,contactar con el administrador de Sistema ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        OperationResult objOperationResult = new OperationResult();
                        VentaBL _objVentasBL = new VentaBL();

                        var aptitudeCertificate = new VentaBL().ReporteDocumentoVenta(ref  objOperationResult ,IdVenta);
                        var head = aptitudeCertificate.FirstOrDefault();
                        TotalEnNumero = head.Total.ToString("0.00");

                        if (head.Moneda == "S/")
                        {

                            TotalesenLetras = Globals.ClientSession.i_IncluirSEUOImpresionDocumentos == 0 ? "SON :" + SAMBHS.Common.Resource.Utils.ConvertirenLetras(TotalEnNumero) + " SOLES " : "SON :" + SAMBHS.Common.Resource.Utils.ConvertirenLetras(TotalEnNumero) + " SOLES " + "S.E.U.O.";
                        }
                        if (head.Moneda == "US$")
                        {

                            TotalesenLetras = Globals.ClientSession.i_IncluirSEUOImpresionDocumentos == 0 ? "SON :" + SAMBHS.Common.Resource.Utils.ConvertirenLetras(TotalEnNumero) + " DOLARES AMERICANOS " : "SON :" + SAMBHS.Common.Resource.Utils.ConvertirenLetras(TotalEnNumero) + " DOLARES AMERICANOS " + "S.E.U.O.";
                        }
                        using (DataSet ds1 = new DataSet())
                        {
                            using (DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate))
                            {
                                dt.TableName = "dsDocumentoBoleta";
                                ds1.Tables.Add(dt);
                                rp.SetDataSource(ds1);
                                rp.SetParameterValue("CantidadDecimal", (int)Globals.ClientSession.i_CantidadDecimales);
                                rp.SetParameterValue("CantidadDeciamlPrecio", (int)Globals.ClientSession.i_PrecioDecimales);
                                rp.SetParameterValue("Dia", aptitudeCertificate.Count > 0 ? head.FechaRegistro.Day.ToString("00") : "");
                                rp.SetParameterValue("Mes", aptitudeCertificate.Count > 0 ? new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(int.Parse(head.FechaRegistro.Month.ToString("00"))).ToUpper() : "");  // TextBox con el valor del Parametro);
                                rp.SetParameterValue("Anio", aptitudeCertificate.Count > 0 ? head.FechaRegistro.Year.ToString().Substring(3, 1) : "");
                                rp.SetParameterValue("TotalesenLetras", TotalesenLetras);
                                try
                                {
                                    rp.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.DefaultPaperSize;
                                    var Impresora = Globals.ListaEstablecimientoDetalle.Where(x => x.i_IdTipoDocumento == (int)TiposDocumentos.BoletaVenta && x.v_NombreImpresora != null && x.v_NombreImpresora != string.Empty && x.v_Serie == aptitudeCertificate.FirstOrDefault().SerieDocumento.Trim());
                                    if (Impresora.Any())
                                    {
                                        var nombreImpresora = Impresora.First().v_NombreImpresora.Trim();

                                        //rp.PrintOptions.PrinterName = "Boletas_chayna";
                                        rp.PrintOptions.PrinterName = nombreImpresora;
                                        rp.PrintToPrinter(1, false, 1, 1);
                                    }
                                    else
                                    {
                                        rp.PrintToPrinter(1, true, 1, 1);
                                    }

                                    crystalReportViewer1.ReportSource = rp;
                                    //crystalReportViewer1.Show();
                                    //rp.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.DefaultPaperSize;
                                    //crystalReportViewer1.ReportSource = rp;
                                    //crystalReportViewer1.Show();
                                    //rp.PrintToPrinter(1, false, 1, 1);
                                    //rp.PrintOptions.PrinterName = @"\\192.168.1.50\Boleta";
                                    //----------
                                    //CrystalDecisions.ReportAppServer.ClientDoc.ISCDReportClientDocument rptClientDoc;
                                    //rptClientDoc = rp.ReportClientDocument;
                                    //System.Drawing.Printing.PrintDocument pDoc = new System.Drawing.Printing.PrintDocument();
                                    //pDoc.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("PapelBoleta", 210, 117);
                                    //CrystalDecisions.ReportAppServer.Controllers.PrintReportOptions rasPROpts = new CrystalDecisions.ReportAppServer.Controllers.PrintReportOptions();
                                    //CrystalDecisions.ReportAppServer.ReportDefModel.PrintOptions MYPRTOpts = new CrystalDecisions.ReportAppServer.ReportDefModel.PrintOptions();
                                    //MYPRTOpts.PrinterName = "Facturas";
                                    //MYPRTOpts.DissociatePageSizeAndPrinterPaperSize = true;
                                    //MYPRTOpts.PaperSize = (CrPaperSizeEnum)pDoc.DefaultPageSettings.PaperSize.Width;
                                    //MYPRTOpts.PaperSize = (CrPaperSizeEnum)pDoc.DefaultPageSettings.PaperSize.Height;
                                    //MYPRTOpts.PaperOrientation = CrPaperOrientationEnum.crPaperOrientationPortrait;
                                    //MYPRTOpts.PaperSource = CrPaperSourceEnum.crPaperSourceTractor;
                                    //rptClientDoc.PrintOutputController.ModifyPrintOptions(MYPRTOpts);
                                    //rptClientDoc.PrintOutputController.PrintReport(rasPROpts);
                                    //---------


                                }
                                catch
                                {
                                    UltraMessageBox.Show("El nombre de impresora no existe ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
                        }
                    }

                  // this.Close();
                }
                else
                {
                    UltraMessageBox.Show("El documento no se puede imprimir por falta de datos  ", "Error de Validacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
            catch
            {
                UltraMessageBox.Show("Se produjo un error  al mostrar el reporte ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            impresionExitosa = true;
        }
        #endregion
    }
}
