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
using SAMBHS.Almacen.BL;
namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    public partial class frmDocumentoPedido : Form
    {
        string _idPedido;
        string _idVendedor;
        bool ImpresionVistaPrevia = true;
        public frmDocumentoPedido(int Modo, string idPedido, string idVendedor, bool IVistaPrevia)
        {
            InitializeComponent();
            _idPedido = idPedido;
            _idVendedor = idVendedor;
            ImpresionVistaPrevia = IVistaPrevia;
        }

        private void frmDocumentoPedido_Load(object sender, EventArgs e)
        {

            string TotalLetras = string.Empty, TotalEnNumero,TextoCotizacion="";
             var Empresa = new NodeBL().ReporteEmpresa();
            string DireccionEmpresa="";
            string TelefonoEmpresa="";
            string OtrosDatosEmpresa="";
            ReportDocument rp = new ReportDocument();
            PedidoBL objPedidoBL = new PedidoBL();
            OperationResult objOperationResult = new OperationResult();
            DataSet ds = new DataSet();
            TextoCotizacion = Empresa.FirstOrDefault().RucEmpresaPropietaria.Trim () == Constants.RucManguifajas   ?"SIN OTRO PARTICULAR Y A LA ESPERA DE SUS GRATAS ORDENES, NOS DESPEDIMOS COORDIALMENTE .\n ATENTAMENTE."  : "CONFIRMAR SU PEDIDO CON ORDEN DE COMPRA \n NO SE ACEPTAN DEVOLUCIONES" ;

            var DatosAlmacen = new AlmacenBL().ObtenerDatosEmpresa(Globals.ClientSession.i_IdAlmacenPredeterminado.Value, Globals.ClientSession.i_IdEstablecimiento.Value);
            DireccionEmpresa = DatosAlmacen.Count() > 0 ? "Dirección : " + DatosAlmacen[0] :"";
            TelefonoEmpresa = DatosAlmacen.Count() > 0 ? "Teléfonos : " + DatosAlmacen[1]:"";
            OtrosDatosEmpresa = DatosAlmacen.Count() > 0 ? "E-Mail :  " + DatosAlmacen[2] : ""; //atemultimangueras@manguifajas.com	Web: www.manguifajas.com" ;
            	
            var Ruc = new NodeBL().ReporteEmpresa().FirstOrDefault().RucEmpresaPropietaria;
            rp = ReportesUtils.DevolverReporte(Ruc, TiposReportes.Pedido);
            if (rp == null)
            {
                UltraMessageBox.Show("Reporte no realizado para esta Empresa,contactar con el administrador de Sistema ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var aptitudeCertificate = objPedidoBL.ReporteDocumentoPedido(ref objOperationResult , _idPedido, _idVendedor);

            if (objOperationResult.Success == 0)
            {
                UltraMessageBox.Show("Ocurrió un Error al realizar Reporte ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            int IdMoneda = aptitudeCertificate.FirstOrDefault().i_idMoneda;
          
            TotalEnNumero = aptitudeCertificate.FirstOrDefault().d_PrecioVenta.ToString("0.00");

            if (IdMoneda == (int)Currency.Soles)
            {
             
                TotalLetras =  Globals.ClientSession.i_IncluirSEUOImpresionDocumentos ==0 ?   "SON : "+ SAMBHS.Common.Resource.Utils.ConvertirenLetras(TotalEnNumero) + " SOLES ":   "SON : "+ SAMBHS.Common.Resource.Utils.ConvertirenLetras(TotalEnNumero) + " SOLES " + "S.E.U.O.";
            }
            if (IdMoneda == (int)Currency.Dolares)
            {
             
                TotalLetras = Globals.ClientSession.i_IncluirSEUOImpresionDocumentos ==0? SAMBHS.Common.Resource.Utils.ConvertirenLetras(TotalEnNumero) + " DOLARES AMERICANOS ": "SON : "+SAMBHS.Common.Resource.Utils.ConvertirenLetras(TotalEnNumero) + " DOLARES AMERICANOS " + "S.E.U.O.";
            }

           
            DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);

            dt.TableName = "dsDocumentoPedido";
            ds.Tables.Add(dt);
            rp.SetDataSource(ds);

            rp.SetParameterValue("CantidadDecimal", (int)Globals.ClientSession.i_CantidadDecimales);
            rp.SetParameterValue("CantidadDeciamlPrecio", (int)Globals.ClientSession.i_PrecioDecimales);
            rp.SetParameterValue("NombreEmpresaPropietaria", Empresa.FirstOrDefault().NombreEmpresaPropietaria.Trim());
            rp.SetParameterValue("TotalLetras", TotalLetras);
            rp.SetParameterValue("TextoCotizacion", TextoCotizacion);
            rp.SetParameterValue("DatosEmpresa", DireccionEmpresa);
            rp.SetParameterValue("TelefonoEmpresa", TelefonoEmpresa);
            rp.SetParameterValue("OtrosDatosEmpresa", OtrosDatosEmpresa);
            rp.SetParameterValue("RucEmpresa", Empresa.FirstOrDefault().RucEmpresaPropietaria.Trim());
            if (!ImpresionVistaPrevia)
            {

                var Impresora = Globals.ListaEstablecimientoDetalle.Where(x => x.i_IdTipoDocumento == (int)TiposDocumentos.Pedido && x.v_NombreImpresora != null && x.v_NombreImpresora != string.Empty && x.v_Serie.Trim () ==aptitudeCertificate.FirstOrDefault ().v_SerieDocumento.Trim () );
                if (Impresora != null)
                {
                    var nombreImpresora = Impresora.FirstOrDefault().v_NombreImpresora.Trim ();
                    rp.PrintOptions.PrinterName = nombreImpresora;
                    rp.PrintToPrinter(1,false, 1, 1);
                }
                else
                {
                  
                    rp.PrintToPrinter(1, true, 1, 1);
                }
            }
            crystalReportViewer1.ReportSource = rp;
            crystalReportViewer1.Show();
        }


    }
}
