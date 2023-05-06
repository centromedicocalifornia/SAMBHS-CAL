using System;
using System.Collections.Generic;
using SAMBHS.Common.Resource;
using System.Linq;
using SAMBHS.CommonWIN.BL;

namespace SAMBHS.Contabilidad.BL
{
    public class VentaDetraccionesBl
    {
        public List<ReporteVentaDetracciones> ObtenerVentasDetraccion(ref OperationResult pobjOperationResult, DateTime fInicio, DateTime fFinal)
        {
            try
            {
                var bancoRetencion = DocumentoBL.ObtenerDocumentoBancoDetraccion();
                if (bancoRetencion == -1) throw new Exception("Documento banco retención no identificado en el sistema.");

                var data = new AsientosContablesBL().ReporteAnalisisCuentasCteAnalitico(ref pobjOperationResult, fInicio,
                    fFinal, null, null, true, null, TipoReporteAnalisis.AnalisisCuentaCorrienteAnalitico, 0, null, "12");

                var result = (from agrupado in data.GroupBy(g => new {anexo = g.IdAnexo, doc = g.DocumentoKey})
                    where agrupado.Any(p => p.TasaDetraccionVenta > 0)
                    let provicion = agrupado.FirstOrDefault(p => p.TasaDetraccionVenta > 0)
                    where provicion != null
                    select new ReporteVentaDetracciones
                    {
                        Cliente = provicion.NombreCliente, 
                        Importe = provicion.ImporteSoles ?? 0, 
                        DepositoBancoDetraccion = agrupado.Where(o => o.IdTipoDocumentoProvicion == bancoRetencion && o != provicion).Sum(p => p.ImporteSoles ?? 0), 
                        MontoCobrado = agrupado.Where(o => o.IdTipoDocumentoProvicion != bancoRetencion && o != provicion).Sum(p => p.ImporteSoles ?? 0), 
                        NroComprobante = provicion.NroDocumento, 
                        TasaDetraccion = provicion.TasaDetraccionVenta, 
                        FechaVenta = provicion.FechaDetalle,
                        TipoDocumentoCliente =provicion.Detalle_TipoIdentificacion ,
                        NroDocumentoCliente =provicion.Detalle_Ruc ,
                        NroCuentaDetraccion =provicion.CuentaDetraccion ,
                        TipoDocumento =provicion.TipoDocumento.ToString ("00") ,
                        NroDocumento =provicion.DocumentoRaw ,
                        TipoOperacionDetraccion =provicion.TipoOperacionDetraccion.ToString ("00"),
                        CodigoDetraccion =provicion.i_CodigoDetraccion.ToString ("000"),
                        
                    }).ToList();

                pobjOperationResult.Success = 1;
                return result;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "VentaDetraccionesBl.ObtenerVentasDetraccion()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }
    }

    public class ReporteVentaDetracciones
    {
        public DateTime FechaVenta { get; set; }
        public string NroComprobante { get; set; }
        public string Cliente { get; set; }
        public decimal Importe { get; set; }
        public decimal TasaDetraccion { get; set; }
        public decimal DepositoBancoDetraccion { get; set; }
        public decimal MontoCobrado { get; set; }
        public string TipoDocumentoCliente { get; set; }
        public string NroDocumentoCliente { get; set; }
        public string NroCuentaDetraccion { get; set; }
        public string TipoDocumento { get; set; }
        public string NroDocumento { get; set; }
        public string TipoOperacionDetraccion { get; set; }
        public string CodigoDetraccion { get; set; }
       
    }
}
