using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public partial class guiaremisioncompradetalleDto
    {
        public string v_NombreProducto { get; set; }
        public string v_CodigoInterno { get; set; }
        public string UMEmpaque { get; set; }
        public int i_EsServicio { get; set; }
        public decimal? d_Empaque { get; set; }
        public int? i_IdUnidadMedidaProducto { get; set; }
        public int i_SolicitarNroLoteIngreso { get; set; }
        public int i_SolicitarNroSerieIngreso { get; set; }
        public int i_SolicitaOrdenProduccionIngreso { get; set; }
    }
}
