using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE.Custom
{
    public class ConsultaFlujoProcesosNBS
    {
        public string NroOrdenTrabajo { get; set; }
        public DateTime? FechaOrdenTrabajo { get; set; }
        public string NroFormatoUnicoFacturacion { get; set; }
        public DateTime? FechaFormatoUnicoFacturacion { get; set; }
        public decimal? MontoFormatoUnicoFacturacion { get; set; }
        public string FormatoFacturado { get; set; }
        public string SerieCorrelativoVenta { get; set; }
        public DateTime? FechaVenta { get; set; }
        public string NroKardexs { get; set; }
        public decimal? MontoCobrado { get; set; }
        public string NombreCliente { get; set; }
        public string Responsable { get; set; }
    }
    public class KardexDesdeFacturacion
    {
        public string SerieCorrelativoVenta { get; set; }
        public DateTime FechaVenta { get; set; }
        public string NroKardexs { get; set; }
        public decimal MontoCobrado { get; set; }
        public string v_IdTipoKardex { get; set; }
        public string Cliente { get; set; }

    }
}
