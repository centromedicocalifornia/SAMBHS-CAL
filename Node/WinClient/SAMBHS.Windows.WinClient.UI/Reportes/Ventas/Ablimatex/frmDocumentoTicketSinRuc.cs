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
using SAMBHS.Almacen.BL;


namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas.Ablimatex
{
    public partial class frmDocumentoTicketSinRuc : Form
    {

        #region Fields
        List<string> IdVentas = new List<string>();
        public int _IdIdentificacion = 0;
        bool ImpresionVistaPrevia = true;
        #endregion

        #region Init
        public frmDocumentoTicketSinRuc(List<string> IdVenta, int IdIdentificacion, bool IVistaPrevia)
        {
            InitializeComponent();
            IdVentas = IdVenta;
            _IdIdentificacion = IdIdentificacion;
            ImpresionVistaPrevia = IVistaPrevia;
            ImpresionDirecto();


        }
        #endregion

        #region Methods
        private void ImpresionDirecto()
        {
            try
            {
                string Impresion = Globals.ClientSession.v_ImpresionDirectoVentas;
                this.Visible = Impresion == "1" ? false : true;

                //this.Visible = false;          
                if (IdVentas != null)
                {
                    var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();
                    var aptitudeCertificat3 = new AlmacenBL().ReporteAlmacen(Globals.ClientSession.i_IdAlmacenPredeterminado.Value);
                    foreach (string IdVenta in IdVentas)
                    {
                        var Ruc = new NodeBL().ReporteEmpresa().FirstOrDefault().RucEmpresaPropietaria;
                        var rp = _IdIdentificacion != 1 ? ReportesUtils.DevolverReporte(Ruc, TiposReportes.Ticket) : ReportesUtils.DevolverReporte(Ruc, TiposReportes.TicketRuc);
                        if (rp == null)
                        {
                            UltraMessageBox.Show("Reporte no realizado para esta Empresa,contactar con el administrador de Sistema ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        OperationResult objOperationResult = new OperationResult();
                        VentaBL _objVentasBL = new VentaBL();

                        var aptitudeCertificate = new VentaBL().ReporteDocumentoTicket(IdVenta);
                        DataSet ds1 = new DataSet();

                        DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
                        DataTable dt2 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate2);
                        DataTable dt3 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificat3);
                        ds1.Tables.Add(dt);
                        ds1.Tables[0].TableName = "dsDocumentoTicketSinRuc";
                        ds1.Tables.Add(dt2);
                        ds1.Tables[1].TableName = "dsEmpresa";
                        ds1.Tables.Add(dt3);
                        ds1.Tables[2].TableName = "dsAlmacen";      
                        rp.SetDataSource(ds1);    
                        try
                        {
                            ////rp.PrintOptions.PrinterName = "Boletas_tisn";
                            ////rp.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.DefaultPaperSize;
                            ////crystalReportViewer1.ReportSource = rp;
                            ////crystalReportViewer1.Show();
                            ////rp.PrintToPrinter(1, false, 1, 1);

                            crystalReportViewer1.ReportSource = rp;
                            crystalReportViewer1.Show();
                            if (Impresion == "1") {
                                //rp.PrintToPrinter(1, false, 1, 1);
                            }
                        }
                        catch
                        {
                            UltraMessageBox.Show("El nombre de impresora no existe ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    if (Impresion == "1")
                    {
                        this.Close();
                    }             
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
        }
        #endregion
    }
}
