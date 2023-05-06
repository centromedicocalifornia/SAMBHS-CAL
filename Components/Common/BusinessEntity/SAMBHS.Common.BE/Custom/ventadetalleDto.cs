using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public partial class ventadetalleDto
    {
      public decimal d_Total {get;set;}
      public decimal d_CantidaBulto {get;set;}
      public int i_IdTipoBulto {get;set;}
      public int i_IdUnidadEmpaque { get; set; }
      public string ProductoNombre { get; set; }
      public string EmpaqueUM { get; set; }
      public string UnidadMedida { get; set; }
      public int EsServicio { get; set; }
      public string SerieDocumento { get; set; }
      public string CorrelativoDocumento { get; set; }
      public int TipoDocumento { get; set; }
      public string RegistroKey { get; set; }
      public decimal TotalDolares { get; set; }

      public string v_CodigoInterno { get; set; }
      public decimal? Empaque { get; set; }
      public string UMEmpaque { get; set; }
      public int? i_EsServicio { get; set; }
      public int? i_EsAfectoDetraccion { get; set; }
      public int? i_EsNombreEditable { get; set; }
      public string NombreCuenta { get; set; }
      public string Descripcion { get; set; }
      public int? i_EsAfectoPercepcion { get; set; }
      public decimal? d_TasaPercepcion { get; set; }
      public string EsRedondeo { get; set; }
      public int? i_IdUnidadMedidaProducto { get; set; }
      public int i_ValidarStock { get; set; }
      public string v_IdFormatoUnicoFacturacionDetalle { get; set; }
      public string DetalleAnexo { get; set; }
      public int i_SolicitarNroSerieSalida { get; set; }
      public int i_SolicitarNroLoteSalida { get; set; }
      public int i_SolicitaOrdenProduccionSalida { get; set; }
        public string v_ServiceId { get; set; }

        public string EmpresaFacturacion { get; set; }
        public string RucEmpFacturacion { get; set; }

      public string v_IdUnidadProductiva { get; set; }
      public string v_ComponentId { get; set; }

      public string v_HopitalizacionId { get; set; }
      public string ACargo { get; set; }
    }
}
