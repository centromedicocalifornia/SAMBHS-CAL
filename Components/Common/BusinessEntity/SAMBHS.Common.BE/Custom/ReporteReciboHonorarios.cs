using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public class ReporteReciboHonorarios :ICloneable 
    {
        public DateTime Fecha { get; set; }
        public string Documento { get; set; }
        public string Proveedor{ get; set; }
        public string NroComprobante { get; set; }
        public string RucProveedor { get; set; }
        public string Moneda { get; set; }
        public decimal? Monto { get; set; }
        public decimal? MontoNacional { get; set; }
        public decimal? ImpuestoCuartaCategoria { get; set; }
        public decimal? Total { get; set; }
        public decimal? TipoCambio{get;set;}
        public string NombreEmpresaPropietaria { get; set; }
        public string RucEmpresaPropietaria { get; set; }
        public string MesParametro { get; set; }
        public decimal? ImpuestoExtSolidaridad { get; set; }
        public string v_Mes { get; set; }
        public int? i_IdMoneda{get;set ;}
        public string v_IdProveedor { get; set; }
        public string Glosa { get; set; }
        public string Servicio { get; set; }
       
        public string NombreProveedor { get; set; }
        public string DireccionProveedor { get; set; }
        public object Clone()
        {
            return MemberwiseClone();
        }
        
    }
    public class ReporteCuartaCategoria
    {
        public string Texto1 { get; set; }
        public string Texto3 { get; set; }
        public string Servicio { get; set; }
        public string   RentasBrutas { get; set; }
        public string ImporteRetenido { get; set; }
        public string Texto2 { get;set;}
        public string Ejercicio { get; set; }
        public string v_IdTrabajador { get; set; }
        public string NroLey { get; set; }
        public string NombreRepresentante { get; set; }
        public string FechaHoraImprimir { get; set; }
    
    }
}
