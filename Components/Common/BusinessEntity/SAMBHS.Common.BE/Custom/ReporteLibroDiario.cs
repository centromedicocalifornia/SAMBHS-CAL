using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public class ReporteLibroDiario :ICloneable 
    {
        public string monedaTransaccion { get; set; }
       public string codigoUnicoOperacion { get; set; }
        public  string codigoUnico {get;set;}
        public DateTime fecha { get; set; }
        public string descripcionOperacion { get; set; }
        public string cuenta { get; set; }
        public string denominacion { get; set; }
        public decimal debeSoles { get; set; }
        public decimal haberSoles { get; set; }
        public int tipoComprobante { get; set; }
        public string PrimerElemento { get; set; }
        //public string DebeSolesString { get; set; }
        //public string HaberSolesString { get; set; }
        public int  Index { get; set; }
        public string NroRegistro { get; set; }
        public string SiglasMoneda { get; set; }
        public int  TipoDocEmisor { get; set; }
        public string NroDocEmisor { get; set; }
        public int  TipoComprobanteDetalle { get; set; }
        public string SerieCorrelativoDetalle { get; set; }
        public string Glosa { get; set; }
        public string NroRegistroSiglas { get; set; }
        public int Ordenamiento { get; set; }
        public string IdDiaro { get; set; }
        public string Pivot { get; set; }
        public string IdDiarioDetalle { get; set; }
        public string Naturaleza { get; set; }
        public int TipoDocumento { get; set; }
        public double   TipoDocumentoNroRegistro { get; set; }

     
        

        public object Clone()
        {
            return MemberwiseClone();

        }
       
    }


    public class ReporteLibroDiarioSimplificado : ICloneable
    {

        public string NroRegistro { get; set; }
        public string Celda1 { get; set; }
        public string CeldaValor1 { get; set; }
        public string Celda2 { get; set; }
        public string CeldaValor2 { get; set; }
        public string Pivot { get; set; }
        public object Clone()
        {
            return MemberwiseClone();

        }
    
    }

    public class DocumentosSerieCorrelativo
    {

        public int TipoDocumento { get; set; }
        public int MaxSerie { get; set; }
        public int MaxCorrelativo { get; set; }
    
    }
    public class ReporteLibroInventarioValorizado
    { 
    
    
    }
}
