using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE.Custom
{
    public class Rentecion5TReporte
    {
        public int Mes { get; set; }
        public string CodTrabajador { get; set; }
        public string ApellidoNombres { get; set; }
        public string Direccion { get; set; }
        public string v_IdTrabajador { get; set; }
        public string Provincia { get; set; }
        public string Distrito { get; set; }
        public string TipoVia { get; set; }
        public string Ocupacion { get; set; }
        public string NroDocIdentidad { get; set; }
        public decimal SueldoBasico { get; set; }
        public decimal RemuneracionesAnteriores { get; set; }
        public decimal GratificacionesProyectadas { get; set; }
        public decimal RemuneracionBruta { get; set; }
        public decimal RemuneracionProyectada { get; set; }
        public decimal TotalRemuneracion { get; set; }
        public decimal DeduccionUit { get; set; }
        public decimal RentaNeta { get; set; }
        public decimal ImporteResultante { get; set; }
        public decimal RetencionMesesAnteriores { get; set; }
        public decimal ImportePorRetener { get; set; }
        public decimal Retencion { get; set; }
        public decimal Tope1 { get; set; }
        public decimal Tope2 { get; set; }
        public decimal Tope3 { get; set; }
        public decimal Tope4 { get; set; }
        public decimal Tope5 { get; set; }
    }

    public class Certificado5taCategoria
    {
        public string Titulo { get; set; }
        public string FechaEmision { get; set; }
        public string Texto1 { get; set; }
        public string Texto2 { get; set; }
        public string Grupo { get; set; }
        public string Conceptos { get; set; }
        public string Total { get; set; }
        public string v_IdTrabajador { get; set; }
        public decimal RentaBruta { get; set; }
        public decimal Deducciones { get; set; }
        public decimal RentaNeta { get; set; }
        public decimal ImpuestoRenta { get; set; }
        public decimal ImpuestoTotalRetenido { get; set; }
        public decimal Tope1 { get; set; }
        public decimal Tope2 { get; set; }
        public decimal Tope3 { get; set; }
        public decimal Tope4 { get; set; }
        public decimal Tope5 { get; set; }
        
    }
    public class Certificado5taCategoriaDetalles
    {
        public string Grupo { get; set; }
        public string Conceptos { get; set; }
        public string Total { get; set; }
        public string v_IdTrabajador { get; set; }
        public string Porcentajes { get; set; }
    
    }
}
