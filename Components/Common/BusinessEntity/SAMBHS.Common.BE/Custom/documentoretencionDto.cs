using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public partial class documentoretencionDto
    {
        public string NombreProveedor { get; set; }
        public string RucProveedor { get; set; }
        public string NroRegistro { get; set; }
        public string UsuarioCreacion { get; set; }
        public string FechaCreacion { get; set; }
        public bool Contabilizado { get; set; }
        public string NroRetencion { get; set; }
        //public string Tesoreria { get; set; } 
    }

    public partial class ReporteRetencion
    {

        public string NumeroDocumentoRetencion { get; set; }
        public string sFechaRegistro { get; set; }
        public string NombreProveedor { get; set; }
        public string RucProveedor { get; set; }
        public decimal MontoPagado { get; set; }
        public decimal MontoRetenido { get; set; }
        public string TotalLetras { get; set; }
        public string v_IdProveedor { get; set; }
        public string sFechaEmisionCompra { get; set; }
        //Detalles

       
        public string v_IdDocumentoRetencion { get; set; }
        public string v_SerieDocumentoDetalle { get; set; }
        public string v_CorrelativoDocumentoDetalle { get; set; }
        public int i_IdTipoDocumentoDetalle { get; set; }
        public string  TipoDocumentoDetalle { get; set; }
        public string MonedaRetencion { get; set; }
       






    }
}
