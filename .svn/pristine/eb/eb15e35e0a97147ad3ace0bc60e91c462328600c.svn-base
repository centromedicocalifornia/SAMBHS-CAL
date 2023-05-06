using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using SAMBHS.Venta.BL;
using CrystalDecisions.Shared;
using SAMBHS.Common.Resource;
using SAMBHS.Common.BL;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    // ReSharper disable once InconsistentNaming
    public partial class frmDocumentoNotaCredito : Form
    {
        #region Declaraciones
        readonly List<string> _idVentas;
        readonly bool _impresionVistaPrevia;
        CrystalDecisions.CrystalReports.Engine.ReportDocument rp;
        #endregion

        #region Carga de inicializacion
        public frmDocumentoNotaCredito(List<string> idVenta, bool vistaPrevia)
        {
            InitializeComponent();
            _idVentas = idVenta;
            _impresionVistaPrevia = vistaPrevia;
            ImpresionDirecto();
        }
        #endregion

        #region Prodecimientos/Funciones
        private void ImpresionDirecto()
        {
            // this.Visible = false;
            OperationResult objOperationResult = new OperationResult();
            var totalesenLetras = "";
            if (_idVentas != null)
            {
                foreach (var idVenta in _idVentas)
                {
                    //  ReportDocument rp = new ReportDocument();
                    var objEmpresa = new NodeBL().ReporteEmpresa().FirstOrDefault();
                    if (objEmpresa == null) continue;
                    var ruc = objEmpresa.RucEmpresaPropietaria;
                    rp = ReportesUtils.DevolverReporte(ruc, TiposReportes.NotaCredito);
                    if (rp == null)
                    {
                        UltraMessageBox.Show("Reporte no realizado para esta Empresa,contactar con el administrador de Sistema ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    var aptitudeCertificate = new VentaBL().ReporteDocumentoNotaCredito( idVenta);
                    var firstItem = aptitudeCertificate.FirstOrDefault();
                    if(firstItem == null) return;
                    var totalEnNumero = firstItem.Total.ToString("0.00");
                    if (firstItem.Moneda == "S/")
                    {

                        totalesenLetras = Globals.ClientSession.i_IncluirSEUOImpresionDocumentos == 0 ? "SON :" + Utils.ConvertirenLetras(totalEnNumero) + " SOLES " : "SON :" + Utils.ConvertirenLetras(totalEnNumero) + " SOLES " + "S.E.U.O.";
                    }
                    if (firstItem.Moneda == "US$")
                    {

                        totalesenLetras = Globals.ClientSession.i_IncluirSEUOImpresionDocumentos == 0 ? "SON :" + Utils.ConvertirenLetras(totalEnNumero) + " DOLARES AMERICANOS " : "SON :" + Utils.ConvertirenLetras(totalEnNumero) + " DOLARES AMERICANOS " + "S.E.U.O.";
                    }



                    var ds1 = new DataSet();

                    var dt = Utils.ConvertToDatatable(aptitudeCertificate);

                    dt.TableName = "dsDocumentonNotaCredito";

                    ds1.Tables.Add(dt);
                    rp.SetDataSource(ds1);

                    var crParameterDiscreteValue = new ParameterDiscreteValue
                    {
                        Value = Globals.ClientSession.i_CantidadDecimales ?? 2
                    };
                    // TextBox con el valor del Parametro
                    var crParameterFieldDefinitions = rp.DataDefinition.ParameterFields;
                    var crParameterFieldDefinition = crParameterFieldDefinitions["CantidadDecimal"];

                    var crParameterValues = crParameterFieldDefinition.CurrentValues;
                    crParameterValues.Clear();
                    crParameterValues.Add(crParameterDiscreteValue);
                    crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);

                    crParameterDiscreteValue = new ParameterDiscreteValue
                    {
                        Value = Globals.ClientSession.i_PrecioDecimales ?? 2
                    };
                    // TextBox con el valor del Parametro
                    crParameterFieldDefinitions = rp.DataDefinition.ParameterFields;
                    crParameterFieldDefinition = crParameterFieldDefinitions["CantidadDeciamlPrecio"];
                    crParameterValues = crParameterFieldDefinition.CurrentValues;
                    crParameterValues.Clear();
                    crParameterValues.Add(crParameterDiscreteValue);
                    crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);


                    crParameterDiscreteValue = new ParameterDiscreteValue
                    {
                        Value = firstItem.FechaRegistro.Day.ToString("00")
                    };
                    // TextBox con el valor del Parametro  ;  // TextBox con el valor del Parametro
                    crParameterFieldDefinitions = rp.DataDefinition.ParameterFields;
                    crParameterFieldDefinition = crParameterFieldDefinitions["Dia"];
                    crParameterValues = crParameterFieldDefinition.CurrentValues;
                    crParameterValues.Clear();
                    crParameterValues.Add(crParameterDiscreteValue);
                    crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);


                    crParameterDiscreteValue = new ParameterDiscreteValue
                    {
                        Value = firstItem.FechaRegistro.Month.ToString("00")
                    };
                    crParameterFieldDefinitions = rp.DataDefinition.ParameterFields;
                    crParameterFieldDefinition = crParameterFieldDefinitions["Mes"];
                    crParameterValues = crParameterFieldDefinition.CurrentValues;
                    crParameterValues.Clear();
                    crParameterValues.Add(crParameterDiscreteValue);
                    crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);

                    crParameterDiscreteValue = new ParameterDiscreteValue
                    {
                        Value = firstItem.FechaRegistro.Year.ToString()
                    };
                    crParameterFieldDefinitions = rp.DataDefinition.ParameterFields;
                    crParameterFieldDefinition = crParameterFieldDefinitions["Anio"];
                    crParameterValues = crParameterFieldDefinition.CurrentValues;
                    crParameterValues.Clear();
                    crParameterValues.Add(crParameterDiscreteValue);
                    crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);

                    crParameterDiscreteValue = new ParameterDiscreteValue {Value = totalesenLetras};
                    crParameterFieldDefinitions = rp.DataDefinition.ParameterFields;
                    crParameterFieldDefinition = crParameterFieldDefinitions["TotalesenLetras"];
                    crParameterValues = crParameterFieldDefinition.CurrentValues;
                    crParameterValues.Clear();
                    crParameterValues.Add(crParameterDiscreteValue);
                    crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);


                    try
                    {
                       
                        if (!_impresionVistaPrevia)
                        {
                           // rp.PrintOptions.PaperSize = PaperSize.DefaultPaperSize;
                            var impresora = Globals.ListaEstablecimientoDetalle.Where(x => x.i_IdTipoDocumento == (int)TiposDocumentos.NotaCredito && !string.IsNullOrEmpty(x.v_NombreImpresora) && x.v_Serie.Trim() == firstItem.SerieDocumento).ToList();
                            if (impresora.Any())
                            {
                                var nombreImpresora = impresora.First().v_NombreImpresora.Trim ();
                                rp.PrintOptions.PrinterName = nombreImpresora;
                                
                                try 
                                {
                                    rp.PrintToPrinter(1, false, 1, 1);
                                }catch
                                    {
                                    }
                                
                            }
                            else
                            {
                                rp.PrintToPrinter(1, true, 1, 1);
                            }
                            //crystalReportViewer1.ReportSource = rp;
                            //rp.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.DefaultPaperSize;                        
                            //crystalReportViewer1.Show();
                        }
                        crystalReportViewer1.ReportSource = rp;
                        crystalReportViewer1.Show();
                        
                    }
                    catch
                    {
                        UltraMessageBox.Show("El nombre de impresora no existe ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                // this.Close();
            }
            else
            {
                UltraMessageBox.Show("El documento no se puede imprimir ", "Error de Validacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }
        #endregion

    }
}
