using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public class ReporteAsientoDiario
    {
        public string Cuenta { get; set; }
        public string Anexo { get; set; }
        public decimal MonedaExtranjera { get; set; }
        public decimal TipoCambio { get; set; }
        public decimal Debe { get; set; }
        public decimal Haber { get; set; }
        public string FechaVencimiento { get; set; }
        public string Documento { get; set; }
        public string NombreAsiento { get; set; }
        public string GlosaAsiento { get; set; }
        public string FechaAsiento { get; set; }
        public string NroAsiento { get; set; }
        public string Naturaleza { get; set; }
        public decimal d_Importe { get; set; }
        public decimal d_Cambio { get; set; }
        public string TipoDiario { get; set; }
        public string KeyCuenta3 { get; set; }
        public string   CentroCosto { get; set; }
        public int CorrelativoInt { get; set; }
        public string Grupo { get; set; }
        public string CodigoAnexo { get; set; }
        public string i_IdCentroCosto { get; set; }
        public string SiglasDocumentoDetalle { get; set; }
        public string SiglasDocumentoRefDetalle { get; set; }
        public string NroDocRef { get; set; }
        public string Analisis { get; set; }
        public string MonedaOperacion { get; set; }
        public string v_IdDiarioDetalle { get; set; }
       
    }
}
