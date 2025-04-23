using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.Compra.BL;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    public partial class frmDocumentoGuiaRemision : Form
    {
        #region Declaraciones / Referencias
        string idGuiaRemision;
        bool impresionVistaPrevia = true;
        #endregion

        #region Carga de inicializacion
        public frmDocumentoGuiaRemision(string idGuiaRemision, bool iVistaPrevia, TiposReportes tipoRep)
        {
            this.idGuiaRemision = idGuiaRemision;
            InitializeComponent();
            impresionVistaPrevia = iVistaPrevia;
            ImpresionDirecto(tipoRep);
        }
        #endregion
       
        #region Prodecimientos/Funciones
        private void ImpresionDirecto(TiposReportes iTipoReporte)
        {
            
            var ruc = Globals.ClientSession.v_RucEmpresa;
       

            var aptitudeCertificate = new GuiaRemisionBL().ReporteDocumentoGuiaRemision(idGuiaRemision);
            var rp = ReportesUtils.DevolverReporte(ruc, iTipoReporte, aptitudeCertificate.FirstOrDefault ().SerieGuia);
            if (rp == null)
            {
                UltraMessageBox.Show("Reporte no realizado para esta Empresa,contactar con el administrador de Sistema ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DataSet ds1 = new DataSet();

            var dt = Utils.ConvertToDatatable(aptitudeCertificate);

            dt.TableName = "dtDocumentoGuiaRemision";

            ds1.Tables.Add(dt);
            rp.SetDataSource(ds1);
            //rp.SetParameterValue("CantidadDecimal", Globals.ClientSession.i_CantidadDecimales ?? 2);

            try
            {

                if (!impresionVistaPrevia)
                {
                 // rp.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.DefaultPaperSize;
                  var impresora = Globals.ListaEstablecimientoDetalle.Where(x => x.i_IdTipoDocumento == (int)TiposDocumentos.GuiaRemision && x.v_NombreImpresora != null && x.v_NombreImpresora != string.Empty && x.v_Serie.Trim () ==aptitudeCertificate.FirstOrDefault().SerieGuia);
                     if (impresora.Any())
                     {
                         var nombreImpresora = impresora.FirstOrDefault().v_NombreImpresora.Trim ();
                         rp.PrintOptions.PrinterName = nombreImpresora;
                         rp.PrintToPrinter(1, false, 1, 1);
                     }
                     else
                     {
                        // rp.PrintOptions.PrinterName = "GuiaRemision_chayna";
                         rp.PrintToPrinter(1, true, 1, 1);
                     }
                     
                }
                crystalReportViewer1.ReportSource = rp;
                //crystalReportViewer1.Show();
            }
            catch
            {
                UltraMessageBox.Show("El nombre de impresora no existe ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           // this.Close();

        }
        #endregion
    }
}
