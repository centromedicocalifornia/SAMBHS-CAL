using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Windows.SigesoftIntegration.UI.Dtos
{
    public class TicketDto
    {
        public string v_TicketId { get; set; }
        public string v_ServiceId { get; set; }
        public string v_TicketDetalleId { get; set; }
        public string v_Descripcion { get; set; }
        public string v_IdProductoDetalle { get; set; }
        public string v_CodInterno { get; set; }
        public decimal d_Cantidad { get; set; }
        public decimal d_PrecioVenta { get; set; }
        public decimal d_Total { get; set; }

    }
}
