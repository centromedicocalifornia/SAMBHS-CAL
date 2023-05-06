using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Windows.SigesoftIntegration.UI.Dtos
{
   public class TicketCobranza
    {
        public string v_CodInterno { get; set; }       
        public string v_Descripcion { get; set; }
        public decimal? d_PrecioVenta { get; set; }
        public decimal? d_Cantidad { get; set; }
        public int i_EsCoaseguro { get; set; }
        public int i_EsDeducible { get; set; }
        public string v_IdLinea { get; set; }
        public decimal? d_Importe { get; set; }
        public decimal? TotalPagarPaciente { get; set; }
        public decimal? TotalPagarAseguradora { get; set; }

        public decimal d_SaldoPaciente { get; set; }
        public decimal d_SaldoAseguradora { get; set; }

        public string v_IdProductoDetalle { get; set; }

        public int i_IdUnidadMedida { get; set; }
       
    }
}
