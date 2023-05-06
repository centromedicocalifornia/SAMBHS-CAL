using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using SAMBHS.Venta.BL;
using CrystalDecisions.Shared;
using SAMBHS.Common.Resource;
using SAMBHS.Common.BL;
using System.Globalization;
using SAMBHS.Common.BE;


namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    public partial class frmDocumentoProforma : Form
    {
        #region Fields
        private readonly List<string> _idVentas;
        private readonly bool _impresionVistaPrevia;
        CrystalDecisions.CrystalReports.Engine.ReportDocument rp;
        Idioma _Idioma = new Idioma();
        #endregion

        #region Constructor
        public frmDocumentoProforma(List<string> idVenta, bool vistaPrevia, Idioma Idi = Idioma.Espaniol)
        {
            InitializeComponent();
            _idVentas = idVenta;
            _impresionVistaPrevia = vistaPrevia;
             _Idioma = Idi;
            ImpresionDirecto();
        }
        #endregion

        #region Method
        private void ImpresionDirecto()
        {
            try
            {
                //this.Visible = false;
                OperationResult objOperationResult = new OperationResult();
                if (_idVentas != null)
                {
                    foreach (var idVenta in _idVentas)
                    {
                        var empresa1 = new NodeBL().ReporteEmpresa().FirstOrDefault();
                        if (empresa1 == null) continue;
                        var ruc = empresa1.RucEmpresaPropietaria;
                        //ruc = Constants.RucFamilia;
                        rp = ReportesUtils.DevolverReporte(ruc, TiposReportes.Proforma);

                        if (rp == null)
                        {
                            UltraMessageBox.Show("Reporte no realizado para esta Empresa,contactar con el administrador de Sistema ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        var aptitudeCertificate = new VentaBL().ReporteDocumentoVenta(ref objOperationResult,idVenta);
                        var firstaptitude = aptitudeCertificate.FirstOrDefault();
                        if(firstaptitude == null) continue;
                        var totalEnNumero = firstaptitude.Total.ToString("0.00");

                        var totalesenLetras = "";
                        if (firstaptitude.Moneda == "S/")
                            totalesenLetras = Globals.ClientSession.i_IncluirSEUOImpresionDocumentos ==0 ? "SON :" + Utils.ConvertirenLetras(totalEnNumero) + " SOLES " : "SON :" + Utils.ConvertirenLetras(totalEnNumero) + " SOLES " + "S.E.U.O.";
                        else if (firstaptitude.Moneda == "US$")
                            totalesenLetras = Globals.ClientSession.i_IncluirSEUOImpresionDocumentos == 0 ? "SON :" + Utils.ConvertirenLetras(totalEnNumero) + " DOLARES AMERICANOS " : "SON :" + Utils.ConvertirenLetras(totalEnNumero) + " DOLARES AMERICANOS " + "S.E.U.O.";
                        var ds1 = new DataSet();

                        var dt = Utils.ConvertToDatatable(aptitudeCertificate);

                        //var fieldsName = new List<string>();
                        var fieldsName = (from ParameterField item in rp.ParameterFields select item.Name).ToList();
                        List<ReporteDocumentoFactura> ServicioFlete = new List<ReporteDocumentoFactura>();
                        List<ReporteDocumentoFactura> ServicioSeguro = new List<ReporteDocumentoFactura>();
                        string AnexoDetalle = "";
                        List<ReporteDocumentoFactura> Reporte = new List<ReporteDocumentoFactura>();
                        if (Globals.ClientSession.v_RucEmpresa == Constants.RucAgrofergic)
                        {

                            Reporte = aptitudeCertificate;
                            aptitudeCertificate = Reporte.Where(o => o.FormaParteOtrosTributos == 0).ToList();
                            ServicioFlete = Reporte.Where(o => o.FormaParteOtrosTributos == 1 && o.Descripcion.ToUpper().Contains("FLETE")).ToList();
                            ServicioSeguro = Reporte.Where(o => o.FormaParteOtrosTributos == 1 && o.Descripcion.ToUpper().Contains("SEGURO")).ToList();
                            var anexdetalle = Reporte.Where(o => !string.IsNullOrEmpty(o.ObservacionDetalle)).FirstOrDefault();
                            AnexoDetalle = anexdetalle != null ? anexdetalle.ObservacionDetalle : "";

                        }

                        if (fieldsName.Contains("ServicioFlete"))

                            if (Reporte.Any() && (Reporte.FirstOrDefault().iTipoCfr == (int)TiposMediosPagoVenta.Cfr || Reporte.FirstOrDefault().iTipoCfr == (int)TiposMediosPagoVenta.Cif))
                            {
                                rp.SetParameterValue("ServicioFlete", ServicioFlete == null ? "" : "FLETE : ");
                            }
                            else
                            {
                                rp.SetParameterValue("ServicioFlete", "");
                            }
                        if (fieldsName.Contains("TotalFlete"))

                            if (Reporte.Any() && (Reporte.FirstOrDefault().iTipoCfr == (int)TiposMediosPagoVenta.Cfr || Reporte.FirstOrDefault().iTipoCfr == (int)TiposMediosPagoVenta.Cif))
                            {
                                rp.SetParameterValue("TotalFlete", !ServicioFlete.Any() ? 0 : ServicioFlete.Sum(o => o.PrecioVenta));
                            }
                            else
                            {
                                rp.SetParameterValue("TotalFlete", 0);
                            }


                        if (fieldsName.Contains("ServicioSeguro"))
                            if (Reporte.Any() && Reporte.FirstOrDefault().iTipoCfr == (int)TiposMediosPagoVenta.Cif)
                            {
                                rp.SetParameterValue("ServicioSeguro", ServicioSeguro == null ? "" : "SEGURO :");
                            }
                            else
                            {
                                rp.SetParameterValue("ServicioSeguro", "");
                            }

                        if (fieldsName.Contains("TotalSeguro"))
                            if (Reporte.Any() && Reporte.FirstOrDefault().iTipoCfr == (int)TiposMediosPagoVenta.Cif)
                            {
                                rp.SetParameterValue("TotalSeguro", !ServicioSeguro.Any() ? 0 : ServicioSeguro.Sum(o => o.PrecioVenta));
                            }
                            else
                            {
                                rp.SetParameterValue("TotalSeguro", 0);
                            }

                        if (fieldsName.Contains("Idioma"))
                            rp.SetParameterValue("Idioma", _Idioma);

                        if (fieldsName.Contains("Anexo"))
                            rp.SetParameterValue("Anexo", AnexoDetalle);








                        dt.TableName = "dsDocumentoBoleta";
                        ds1.Tables.Add(dt);
                        rp.SetDataSource(ds1);

                        rp.SetParameterValue("CantidadDecimal", (int)Globals.ClientSession.i_CantidadDecimales);
                        rp.SetParameterValue("CantidadDeciamlPrecio", (int)Globals.ClientSession.i_PrecioDecimales);
                        rp.SetParameterValue("Dia", firstaptitude.FechaRegistro.Day.ToString("00"));
                        rp.SetParameterValue("Mes", new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(int.Parse(firstaptitude.FechaRegistro.Month.ToString("00"))).ToUpper());  // TextBox con el valor del Parametro);
                        rp.SetParameterValue("Anio", ruc == Constants.RucRollavel || ruc == Constants.RucJorplast || ruc== Constants.RucFamilia ? firstaptitude.FechaRegistro.Year.ToString().Substring(2, 2) : firstaptitude.FechaRegistro.Year.ToString().Substring(3, 1));
                        rp.SetParameterValue("TotalesenLetras", totalesenLetras);

                       

                        if (fieldsName.Contains("MesNum"))
                            rp.SetParameterValue("MesNum", firstaptitude.FechaRegistro.Month.ToString("00"));
                        if(fieldsName.Contains("YearFull"))
                            rp.SetParameterValue("YearFull", firstaptitude.FechaRegistro.Year.ToString());
                        try
                        {
                            if (!_impresionVistaPrevia)
                            {

                                var impresora =
                                    Globals.ListaEstablecimientoDetalle.Where(
                                        x =>
                                            x.i_IdTipoDocumento == (int) TiposDocumentos.BoletaVenta &&
                                            !string.IsNullOrEmpty(x.v_NombreImpresora)
                                            && x.v_Serie.Trim() == firstaptitude.SerieDocumento.Trim()).ToList();
                                if (impresora.Any())
                                {
                                    var nombreImpresora = impresora.FirstOrDefault().v_NombreImpresora.Trim();
                                    rp.PrintOptions.PrinterName = nombreImpresora;
                                    try
                                    {
                                        rp.PrintToPrinter(1, false, 1, 1);
                                    }
                                    catch
                                    {
                                        // ignored
                                    }
                                }
                                else
                                {
                                    // rp.PrintOptions.PrinterName = "Boletas_tisn";
                                    rp.PrintToPrinter(1, false, 1, 1);
                                }

                            }
                           
                        }
                        catch
                        {
                            UltraMessageBox.Show("El nombre de impresora no existe ", "SISTEMA", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            return;
                        }
                        finally
                        {
                            crystalReportViewer1.ReportSource = rp;
                            crystalReportViewer1.Show();
                        }
                        //rp.PrintOptions.PrinterName = @"\\192.168.1.41\Boleta";

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
                        //rp.PrintToPrinter(1, false, 1, 1);
                    }

                    //this.Close();
                }
                else
                {
                    UltraMessageBox.Show("El documento no se puede imprimir por falta de datos  ", "Error de Validacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                }
            }
            catch
            {
                UltraMessageBox.Show("Se produjo un error  al mostrar el reporte ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        
        #endregion

       

    }
}
