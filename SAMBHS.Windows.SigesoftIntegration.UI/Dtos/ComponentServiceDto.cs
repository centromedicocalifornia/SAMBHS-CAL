using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    public class ComponentServiceDto
    {
        public ComponentServiceDto()
        {
            TipoCalculo = TipoFacturacion.Ocupacional;
            Cantidad = 1;
        }

        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string IdMedicina { get; set; }
        public decimal Cantidad { get; set; }
        public decimal d_SaldoPaciente { get; set; }
        public decimal d_SaldoAseguradora { get; set; }
        public decimal PrecioRedondeadoPaciente
        {
            get
            {
                return Math.Round(d_SaldoPaciente, 2, MidpointRounding.AwayFromZero);
            }
        }
        public decimal PrecioRedondeadoAseguradora
        {
            get
            {
                return Math.Round(d_SaldoAseguradora, 2, MidpointRounding.AwayFromZero);
            }
        }
        public decimal PrecioRedondeado
        {
            get
            {
                return Math.Round(Precio, 2, MidpointRounding.AwayFromZero);
            }
        }
        public int EsCoaseguro { get; set; }
        public int EsDeducible { get; set; }
        public decimal ImporteDescontado { get; set; }
        public string IdUnidadProductiva { get; set; }
        public TipoFacturacion TipoCalculo { get; set; }
        public decimal PrecioCalculado
        {
            get
            {
                decimal impAseguradora;
                decimal impCliente;
                if (EsCoaseguro == 1)
                {
                    var porc = Math.Round(ImporteDescontado / 100, 2, MidpointRounding.AwayFromZero);
                    impCliente = Math.Round(PrecioRedondeado * porc, 2, MidpointRounding.AwayFromZero);
                    impAseguradora = Math.Round(PrecioRedondeado - impCliente, 2, MidpointRounding.AwayFromZero);                    
                }
                else
                {
                    impCliente = Math.Round(ImporteDescontado, 2, MidpointRounding.AwayFromZero);
                    impAseguradora = Math.Round(PrecioRedondeado - impCliente, 2, MidpointRounding.AwayFromZero);                    
                }

                if (TipoCalculo == TipoFacturacion.Aseguradora)
                {
                    PrecioContraparte = impCliente;
                    return impAseguradora;
                }
                PrecioContraparte = impAseguradora;
                return impCliente > 0 ? impCliente : 0;
            }
        }
        public decimal PrecioContraparte { get; private set; }
    }
}
