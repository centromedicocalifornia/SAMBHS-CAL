using CrystalDecisions.Shared;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Venta.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    public partial class frmDocumentoGuiaOtroFormato : Form
    {
          #region Declaraciones / Referencias
      //  string TotalesenLetras, TotalEnNumero;
         List<string> IdVentas =  new List<string> ();
        bool ImpresionVistaPrevia = true;
        bool ProcedenciaFormulario = false;
        CrystalDecisions.CrystalReports.Engine.ReportDocument rp;
        #endregion
        
        
        public frmDocumentoGuiaOtroFormato(List<string> _IdVentas, bool IVistaPrevia, bool ProcedenciaVenta)
        {
            InitializeComponent();
            IdVentas = _IdVentas;
            ImpresionVistaPrevia = IVistaPrevia;
            ProcedenciaFormulario = ProcedenciaVenta;
            ImpresionDirecto();
        }
   

        #region Prodecimientos/Funciones
        private void ImpresionDirecto()
        {
            try
            {
                
                if (IdVentas != null)
                {
                    foreach (string IdVenta in IdVentas)
                    {
                        var Ruc = new NodeBL().ReporteEmpresa().FirstOrDefault().RucEmpresaPropietaria;
                        
                         rp = ReportesUtils.DevolverReporte(Ruc, TiposReportes.OtroFormatoGuia);

                        if (rp == null)
                        {
                            UltraMessageBox.Show("Reporte no realizado para esta Empresa,contactar con el administrador de Sistema ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        OperationResult objOperationResult = new OperationResult();
                        VentaBL _objVentasBL = new VentaBL();
                        ventaDto _ventaDto = new ventaDto();

                        var aptitudeCertificate = new VentaBL().ReporteGuiaRemisionOtroFormato(IdVenta, ProcedenciaFormulario);
                        DataSet ds1 = new DataSet();

                        DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);

                        dt.TableName = "dsDocumentoGuiaRemisionOtroFormato";

                        ds1.Tables.Add(dt);

                        rp.SetDataSource(ds1);

                      //  rp.SetParameterValue("TotalLetras", TotalesenLetras);
                        rp.SetParameterValue("CantidadDecimal", (int)Globals.ClientSession.i_CantidadDecimales);
                        rp.SetParameterValue("CantidadDeciamlPrecio", (int)Globals.ClientSession.i_PrecioDecimales);

                        var FieldsName = new List<string>();
                        foreach (ParameterField item in rp.ParameterFields)
                            FieldsName.Add(item.Name);
                        if (FieldsName.Contains("Mes"))
                            rp.SetParameterValue("Mes", aptitudeCertificate.Count() > 0 ? new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(int.Parse(aptitudeCertificate.FirstOrDefault().DFecha.Month.ToString("00"))).ToUpper() : "");
                        try
                        {
                            if (!ImpresionVistaPrevia)
                            {

                                var Impresora = Globals.ListaEstablecimientoDetalle.Where(x => x.i_IdTipoDocumento == (int)TiposDocumentos.GuiaInterna && x.v_NombreImpresora != null && x.v_NombreImpresora != string.Empty && x.v_Serie.Trim () ==aptitudeCertificate.FirstOrDefault ().SerieDocumento );
                                if (Impresora != null)
                                {
                                  //  rp.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.DefaultPaperSize;
                                    var nombreImpresora = Impresora.FirstOrDefault().v_NombreImpresora.Trim ();
                                    rp.PrintOptions.PrinterName = nombreImpresora;
                                    rp.PrintToPrinter(1, false, 1, 1);
                                }
                                else
                                {
                                    
                                    rp.PrintToPrinter(1, true, 1, 1);
                                }

                            }
                            
                            crystalReportViewer1.ReportSource = rp;
                            crystalReportViewer1.Show();
                            
                        }
                        catch
                        {
                            UltraMessageBox.Show("El nombre de impresora no existe ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            //crystalReportViewer1.ReportSource = rp;
                            //crystalReportViewer1.Show();
                            return;
                        }

                    }

                    //this.Close();
                }
                else
                {
                    UltraMessageBox.Show("El documento no se puede imprimir ", "Error de Validacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
            catch (Exception f)
            {
                UltraMessageBox.Show("Se produjo un error  al mostrar el reporte ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }


        }
        #endregion

        
    }
}
