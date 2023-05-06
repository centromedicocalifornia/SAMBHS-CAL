using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public partial class ordendecompradetalleDto
    {
        public string CodProducto { get; set; }
        public string NombreProducto { get; set; }
        public string EmpaqueUM { get; set; }
        public decimal? Empaque { get; set; }
        public int UMProducto { get; set; }
    }

    public class ordendeCompraDetailDto : ordendecompradetalleDto
    {
        public List<CompraDetail> listaComprasDto { get; set; }
    }

    public class CompraDetail
	{
        public int i_IdTipoDocumento { get; set; }
        public string v_SerieDocumento { get; set; }
        public string v_CorrelativoDocumento { get; set; }
        public decimal d_Cantidad { get; set; }
	}
    public class ReporteOrdenCompraEstado
    {
        public string OrdenCompraDoc { get; set; }
        public string v_Documento { get; set; }
        public string v_SerieDocumento { get; set; }
        public string v_CorrelativoDocumento { get; set; }
        public decimal d_CantidadFact { get; set; }
        public decimal d_Cantidad { get; set; }
        public string CodProd { get; set; }
        public string NombreProd { get; set; }
        public decimal d_CantidadCancelada { get; set; }
    }
}
