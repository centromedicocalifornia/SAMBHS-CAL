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
    public partial class frmDocumentoCajChica : Form
    {
        public string pstrIdCajaChica = "";
        public frmDocumentoCajChica( string idCajaChica)
        {
            InitializeComponent();
            pstrIdCajaChica = idCajaChica;
            //_idPedido = idPedido;
            //_idVendedor = idVendedor;
            //ImpresionVistaPrevia = IVistaPrevia;
        }

        private void frmDocumentoCajChica_Load(object sender, EventArgs e)
        {

             var Empresa = new NodeBL().ReporteEmpresa();
             OperationResult objOperationResult = new OperationResult();
            ReportDocument rp = new ReportDocument();
            DataSet ds = new DataSet();
            
            rp = new Ventas.crDocumentoCajaChica();
            if (rp == null)
            {
                UltraMessageBox.Show("Reporte no realizado para esta Empresa,contactar con el administrador de Sistema ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var aptitudeCertificate = new CajaChicaBL().RepoorteDocumentoCajaChica(ref objOperationResult, pstrIdCajaChica,DateTime.Now.Date  ,DateTime.Now.Date ,false);

            if (objOperationResult.Success == 0)
            {
                UltraMessageBox.Show("Ocurrió un Error al realizar Reporte ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

           
            DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);

            dt.TableName = "dsDocumentoCajaChica";
            ds.Tables.Add(dt);
            rp.SetDataSource(ds);
            rp.SetParameterValue("NombreEmpresa",Empresa.FirstOrDefault ().NombreEmpresaPropietaria.Trim ());
            rp.SetParameterValue("RucEmpresa","R.U.C. : "+ Empresa.FirstOrDefault().RucEmpresaPropietaria.Trim());
            //if (!ImpresionVistaPrevia)
            //{

            //    var Impresora = Globals.ListaEstablecimientoDetalle.Where(x => x.i_IdTipoDocumento == (int)TiposDocumentos.Pedido && x.v_NombreImpresora != null && x.v_NombreImpresora != string.Empty && x.v_Serie.Trim () ==aptitudeCertificate.FirstOrDefault ().v_SerieDocumento.Trim () );
            //    if (Impresora != null)
            //    {
            //        var nombreImpresora = Impresora.FirstOrDefault().v_NombreImpresora.Trim ();
            //        rp.PrintOptions.PrinterName = nombreImpresora;
            //        rp.PrintToPrinter(1,false, 1, 1);
            //    }
            //    else
            //    {
                  
            //        rp.PrintToPrinter(1, true, 1, 1);
            //    }
            //}
            crystalReportViewer1.ReportSource = rp;
            crystalReportViewer1.Show();
        }


    }
}
