using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SAMBHS.Common.BE
{


   public class ReporteRegistroVentaAnaliticoDAOT
    {

       public string Correlativo { get; set; }


        public string NombreEmpresaPropietaria { get; set; }
        public string RucEmpresaPropietaria { get; set; }
        public string NombreCliente { get; set; }
        public string NroDocCliente { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string TipoDocumento { get; set; }
        public string Documento { get; set; }
        public int IdTipoDocumento { get; set; }
        public string CorrelativoDocumento { get; set; }
        public string Grupollave { get; set; }

        public decimal ValorVentaS { get; set; }
        public decimal ValorVentaD { get; set; }
        public decimal IGVS { get; set; }
        public decimal IGVD { get; set; }
        public decimal TOTALS { get; set; }
        public decimal TOTALD { get; set; }
        public string NombreMoneda { get; set; }
        public int IdMoneda { get; set; }
        public decimal TipoCambio { get; set; }
        public string NombreClienteAux { get; set; }
        public string NroDocDeclarado { get; set; }
        public string v_IdVenta { get; set; }
    }
}
