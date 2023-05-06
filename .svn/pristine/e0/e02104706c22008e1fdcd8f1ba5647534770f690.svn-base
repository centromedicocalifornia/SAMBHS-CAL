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

namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    public partial class frmDocumentoFacturaRapida : Form
    {
        public bool ImpresionExitosa {
            get { return impresionExitosa; }
        }

        private bool impresionExitosa;

        #region Declaraciones / Referencias
        string TotalesenLetras, TotalEnNumero;
        List<string> IdVentas = new List<string>();
        CrystalDecisions.CrystalReports.Engine.ReportDocument rp;
        #endregion
        #region Carga de inicializacion
        public frmDocumentoFacturaRapida(List<string> _IdVentas)
        {
            InitializeComponent();
            IdVentas = _IdVentas;
            ImpresionDirecto();
            FormClosing += frmDocumentoFacturaRapida_FormClosing;
        }

        void frmDocumentoFacturaRapida_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (rp != null)
                rp.Dispose();
        }
        #endregion
        #region Cargar Load
        private void frmDocumentoFactura_Load(object sender, EventArgs e)
        {

        }
        #endregion
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

                      rp = ReportesUtils.DevolverReporte(Ruc, TiposReportes.Factura);

                      if (rp== null)
                      {
                          UltraMessageBox.Show("Reporte no realizado para esta Empresa,contactar con el administrador de Sistema ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                          return;
                      }

                        OperationResult objOperationResult = new OperationResult();
                        VentaBL _objVentasBL = new VentaBL();

                        var _ventaDto = _objVentasBL.ObtenerVentaCabecera(ref objOperationResult, IdVenta);
                       
                        TotalEnNumero = _ventaDto.d_Total.Value.ToString("0.00");

                        if (_ventaDto.i_IdMoneda == (int)Currency.Soles)
                        {

                            TotalesenLetras = Globals.ClientSession.i_IncluirSEUOImpresionDocumentos ==0 ? "SON :" + SAMBHS.Common.Resource.Utils.ConvertirenLetras(TotalEnNumero) + " SOLES " : "SON :" + SAMBHS.Common.Resource.Utils.ConvertirenLetras(TotalEnNumero) + " SOLES " + "S.E.U.O.";
                            
                        }
                        if (_ventaDto.i_IdMoneda == (int)Currency.Dolares)
                        {

                            TotalesenLetras = Globals.ClientSession.i_IncluirSEUOImpresionDocumentos == 0 ? "SON :" + SAMBHS.Common.Resource.Utils.ConvertirenLetras(TotalEnNumero) + " DOLARES AMERICANOS " : "SON :" + SAMBHS.Common.Resource.Utils.ConvertirenLetras(TotalEnNumero) + " DOLARES AMERICANOS " + "S.E.U.O.";
                        }

                        var aptitudeCertificate = _objVentasBL.ReporteDocumentoVenta(ref objOperationResult, IdVenta);
                        using (DataSet ds1 = new DataSet())
                        {
                            using (DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate))
                            {
                                dt.TableName = "dsDocumentoFactura";
                                ds1.Tables.Add(dt);
                                rp.SetDataSource(ds1);
                                rp.SetParameterValue("TotalLetras", TotalesenLetras);
                                rp.SetParameterValue("CantidadDecimal", (int)Globals.ClientSession.i_CantidadDecimales);
                                rp.SetParameterValue("CantidadDeciamlPrecio", (int)Globals.ClientSession.i_PrecioDecimales);

                                try
                                {
                                    rp.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.DefaultPaperSize;
                                    var Impresora = Globals.ListaEstablecimientoDetalle.Where(x => x.i_IdTipoDocumento == (int)TiposDocumentos.Factura && x.v_NombreImpresora != null && x.v_NombreImpresora != string.Empty && x.v_Serie.Trim() == aptitudeCertificate.FirstOrDefault().SerieDocumento.Trim());
                                    if (Impresora.Any())
                                    {
                                        var nombreImpresora = Impresora.First().v_NombreImpresora.Trim();
                                        rp.PrintOptions.PrinterName = nombreImpresora;
                                        rp.PrintToPrinter(1, false, 1, 1);
                                    }
                                    else
                                    {
                                        //rp.PrintOptions.PrinterName = "Facturas_tisn";
                                        rp.PrintToPrinter(1, true, 1, 1);
                                    }

                                    //rp.PrintOptions.PrinterName = "Facturas_chayna";
                                    // rp.PrintOptions.PrinterName = "OneNote2010"; 
                                    //rp.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.DefaultPaperSize;
                                    //crystalReportViewer1.ReportSource = rp;
                                    //crystalReportViewer1.Show();
                                    // rp.PrintToPrinter(1, false, 1, 1);
                                    crystalReportViewer1.ReportSource = rp;
                                    //crystalReportViewer1.Show();
                                }
                                catch
                                {
                                    UltraMessageBox.Show("El nombre de impresora no existe ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
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
            catch(Exception f)
            {
                UltraMessageBox.Show("Se produjo un error  al mostrar el reporte ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            impresionExitosa = true;
        }
        #endregion

        

    }
}
