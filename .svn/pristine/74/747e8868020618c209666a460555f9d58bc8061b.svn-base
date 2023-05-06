using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.Venta.BL;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using SAMBHS.Common.BL;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    public partial class frmDocumentoNotaDebito : Form
    {
        #region Declaraciones / Referencias
        List<string> IdVentas = new List<string>();
        bool ImpresionVistaPrevia = true;
        CrystalDecisions.CrystalReports.Engine.ReportDocument rp;
        #endregion
        #region CargaInicializacion
        public frmDocumentoNotaDebito(List<string> IdVenta, bool IVistaPrevia)
        {

            InitializeComponent();
            IdVentas = IdVenta;
            ImpresionVistaPrevia = IVistaPrevia;
            ImpresionDirecto();
            
        }

        private void frmDocumentoNotaDebito_Load(object sender, EventArgs e)
        {

        }
        private void ImpresionDirecto()
        {
            string TotalEnNumero = "", TotalesenLetras = "";
            if (IdVentas != null)
            {

                foreach (string IdVenta in IdVentas)
                {
                    var Ruc = new NodeBL().ReporteEmpresa().FirstOrDefault().RucEmpresaPropietaria;
                   
                    rp = ReportesUtils.DevolverReporte(Ruc, TiposReportes.NotaDebidto);
                    if (rp == null)
                    {
                        UltraMessageBox.Show("Reporte no realizado para esta Empresa,contactar con el administrador de Sistema ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    
                    OperationResult objOperationResult = new OperationResult();
                    var aptitudeCertificate = new VentaBL().ReporteDocumentoNotaDebito(IdVenta);
                    TotalEnNumero = aptitudeCertificate.FirstOrDefault().Total.ToString("0.00");

                    if (aptitudeCertificate.FirstOrDefault().Moneda == "S/.")
                    {

                        TotalesenLetras = Globals.ClientSession.i_IncluirSEUOImpresionDocumentos == 0 ? "SON :" + SAMBHS.Common.Resource.Utils.ConvertirenLetras(TotalEnNumero) + " SOLES " : "SON :" + SAMBHS.Common.Resource.Utils.ConvertirenLetras(TotalEnNumero) + " SOLES " + "S.E.U.O.";
                    }
                    if (aptitudeCertificate.FirstOrDefault().Moneda == "US$.")
                    {

                        TotalesenLetras = Globals.ClientSession.i_IncluirSEUOImpresionDocumentos == 0 ? SAMBHS.Common.Resource.Utils.ConvertirenLetras(TotalEnNumero) + " DOLARES AMERICANOS " : "SON :" + SAMBHS.Common.Resource.Utils.ConvertirenLetras(TotalEnNumero) + " DOLARES AMERICANOS " + "S.E.U.O.";
                    }
                    
                    DataSet ds1 = new DataSet();

                    DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);

                    dt.TableName = "dsDocumentoNotaDebito";

                    ds1.Tables.Add(dt);
                    rp.SetDataSource(ds1);

                    ParameterFieldDefinitions crParameterFieldDefinitions;
                    ParameterFieldDefinition crParameterFieldDefinition;
                    ParameterValues crParameterValues;
                    ParameterDiscreteValue crParameterDiscreteValue;

                    crParameterValues = new ParameterValues();
                    crParameterDiscreteValue = new ParameterDiscreteValue();
                    crParameterDiscreteValue.Value = (int)Globals.ClientSession.i_CantidadDecimales;  // TextBox con el valor del Parametro
                    crParameterFieldDefinitions = rp.DataDefinition.ParameterFields;
                    crParameterFieldDefinition = crParameterFieldDefinitions["CantidadDecimal"];

                    crParameterValues = crParameterFieldDefinition.CurrentValues;
                    crParameterValues.Clear();
                    crParameterValues.Add(crParameterDiscreteValue);
                    crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);
                    crParameterValues = new ParameterValues();

                    crParameterDiscreteValue = new ParameterDiscreteValue();
                    crParameterDiscreteValue.Value = (int)Globals.ClientSession.i_PrecioDecimales;  // TextBox con el valor del Parametro
                    crParameterFieldDefinitions = rp.DataDefinition.ParameterFields;
                    crParameterFieldDefinition = crParameterFieldDefinitions["CantidadDeciamlPrecio"];
                    crParameterValues = crParameterFieldDefinition.CurrentValues;
                    crParameterValues.Clear();
                    crParameterValues.Add(crParameterDiscreteValue);
                    crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);


                    crParameterDiscreteValue = new ParameterDiscreteValue();
                    crParameterDiscreteValue.Value = aptitudeCertificate.Count() > 0 ? aptitudeCertificate.FirstOrDefault().FechaRegistro.Day.ToString("00") : "";  // TextBox con el valor del Parametro  ;  // TextBox con el valor del Parametro
                    crParameterFieldDefinitions = rp.DataDefinition.ParameterFields;
                    crParameterFieldDefinition = crParameterFieldDefinitions["Dia"];
                    crParameterValues = crParameterFieldDefinition.CurrentValues;
                    crParameterValues.Clear();
                    crParameterValues.Add(crParameterDiscreteValue);
                    crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);


                    crParameterDiscreteValue = new ParameterDiscreteValue();
                    crParameterDiscreteValue.Value = aptitudeCertificate.Count() > 0 ? aptitudeCertificate.FirstOrDefault().FechaRegistro.Month.ToString("00") : ""; ;  // TextBox con el valor del Parametro
                    crParameterFieldDefinitions = rp.DataDefinition.ParameterFields;
                    crParameterFieldDefinition = crParameterFieldDefinitions["Mes"];
                    crParameterValues = crParameterFieldDefinition.CurrentValues;
                    crParameterValues.Clear();
                    crParameterValues.Add(crParameterDiscreteValue);
                    crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);

                    crParameterDiscreteValue = new ParameterDiscreteValue();
                    crParameterDiscreteValue.Value = aptitudeCertificate.Count() > 0 ? aptitudeCertificate.FirstOrDefault().FechaRegistro.Year.ToString() : ""; ;  // TextBox con el valor del Parametro
                    crParameterFieldDefinitions = rp.DataDefinition.ParameterFields;
                    crParameterFieldDefinition = crParameterFieldDefinitions["Anio"];
                    crParameterValues = crParameterFieldDefinition.CurrentValues;
                    crParameterValues.Clear();
                    crParameterValues.Add(crParameterDiscreteValue);
                    crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);


                    crParameterValues = new ParameterValues();
                    crParameterDiscreteValue = new ParameterDiscreteValue();
                    crParameterDiscreteValue.Value = TotalesenLetras;
                    crParameterFieldDefinitions = rp.DataDefinition.ParameterFields;
                    crParameterFieldDefinition = crParameterFieldDefinitions["TotalesenLetras"];
                    crParameterValues = crParameterFieldDefinition.CurrentValues;
                    crParameterValues.Clear();
                    crParameterValues.Add(crParameterDiscreteValue);
                    crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);


                    try
                    {
                        


                        if (!ImpresionVistaPrevia)
                        {

                            var Impresora = Globals.ListaEstablecimientoDetalle.Where(x => x.i_IdTipoDocumento == (int)TiposDocumentos.NotaDebito && x.v_NombreImpresora != null && x.v_NombreImpresora != string.Empty && x.v_Serie.Trim () ==aptitudeCertificate.FirstOrDefault ().SerieDocumento );
                            if (Impresora != null)
                            {
                                var nombreImpresora = Impresora.FirstOrDefault().v_NombreImpresora.Trim ();
                                rp.PrintOptions.PrinterName = nombreImpresora;
                                rp.PrintToPrinter(1, false, 1, 1);
                            }
                            else
                            {
                               // rp.PrintOptions.PrinterName = "NotaDebito_tisn";
                                rp.PrintToPrinter(1, true, 1, 1);
                            }
                        }
                        crystalReportViewer1.ReportSource = rp;
                        crystalReportViewer1.Show();
                        //rp.PrintOptions.PrinterName = "Notacredito_tisn";
                        //rp.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.DefaultPaperSize;
                        //crystalReportViewer1.ReportSource = rp;
                        //crystalReportViewer1.Show();
                        //rp.PrintToPrinter(1, false, 1, 1);
                    }
                    catch
                    {
                        UltraMessageBox.Show("El nombre de impresora no existe ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }





                    // this.Close();
                }
            }
            else
            {
                UltraMessageBox.Show("El documento no se puede imprimir ", "Error de Validacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }
        #endregion

     
    }
}
