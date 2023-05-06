using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
public    class valoresRegistroPedido
    {
        public decimal ValorVentaSoles { get; set; }
        public decimal IgvSoles { get; set; }
        public decimal TotalSoles { get; set; }
        public decimal DescuentoSoles { get; set; }

        public decimal ValorVentaDolares { get; set; }
        public decimal IgvDolares { get; set; }
        public decimal TotalDolares { get; set; }
        public decimal DescuentoDolares { get; set; }

        public decimal PrecioDetalleSoles { get; set; }
        public decimal ValorDetalleSoles { get; set; }
        public decimal ValorVentaDetalleSoles { get; set; }
        public decimal DescuentoDetalleSoles { get; set; }
        public decimal PrecioVentaDetalleSoles { get; set; }
        public decimal IgvDetalleSoles { get; set; }

        public decimal PrecioDetalleDolares { get; set; }
        public decimal ValorDetalleDolares { get; set; }
        public decimal ValorVentaDetalleDolares { get; set; }
        public decimal DescuentoDetalleDolares { get; set; }
        public decimal PrecioVentaDetalleDolares { get; set; }
        public decimal IgvDetalleDolares { get; set; }



    }
}
