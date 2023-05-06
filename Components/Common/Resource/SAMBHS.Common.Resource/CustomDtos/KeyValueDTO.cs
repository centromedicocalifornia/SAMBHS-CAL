﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace SAMBHS.Common.Resource
{

   public class KeyValueDTO
    {
        public string Id { get; set; }

        public string Value1 { get; set; }

        public string Value2 { get; set; }

        public string Value3 { get; set; }

        public Single Value4 { get; set; }

        public string Value5 { get; set; }

        public byte[] Value5_ { get; set; }

       public decimal Value6 { get; set; }

       public int? Estado { get; set; }
       public string Value7 { get; set; }

    }

    public class KeyValueDtoImage : KeyValueDTO
    {
        public byte[] Imagen { get; set; }
    }

    public class KeyValueDetalleCompraVentaDTO
    {
       public string v_IdProductoDetalle { get; set; }
       public string v_CodInterno { get; set; }
       public string v_Descripcion { get; set; }
       public int? i_IdUnidadMedida { get; set; }
       public decimal? d_Empaque { get; set; }
       public string EmpaqueUM { get; set; }
       public decimal? d_Cantidad { get; set; }
       public decimal? d_PrecioUnitario { get; set; }
       public int? i_EsServicio { get; set; }
    }
}
