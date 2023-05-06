using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
 public class ReporteRegistroVentaProductoMensual
    {
        public int IdMoneda { get; set; }
        public string IdProducto { get; set; }
        public string IdCliente { get; set; }
        public string NombreProducto { get; set; }
        public decimal CantidadDetalle { get; set; }
     
     
        public decimal PrecioVentaDetalle { get; set; }
        public decimal PrecioTotal { get; set; }

        public decimal ValorVentaDetalle { get; set; }
        public decimal ValorVentaTotal { get; set; }

  
        public decimal PrecioVentaDetalleD { get; set; }
        public decimal ValorVentaDetalleD { get; set; }
        public int IdTipoDocumento { get; set; }
        public decimal TipoCambio { get; set; }
        public string Mes { get; set; }
        public decimal T_Enero { get; set; }
        public decimal T_Febrero { get; set; }
        public decimal T_Marzo { get; set; }
        public decimal T_Abril { get; set; }
        public decimal T_Mayo { get; set; }
        public decimal T_Junio { get; set; }
        public decimal T_Julio { get; set; }
        public decimal T_Agosto { get; set; }
        public decimal T_Setiembre { get; set; }
        public decimal T_Octubre { get; set; }
        public decimal T_Noviembre { get; set; }
        public decimal T_Diciembre { get; set; }
        public decimal C_Enero { get; set; }
        public decimal C_Febrero { get; set; }
        public decimal C_Marzo { get; set; }
        public decimal C_Abril { get; set; }
        public decimal C_Mayo { get; set; }
        public decimal C_Junio { get; set; }
        public decimal C_Julio { get; set; }
        public decimal C_Agosto { get; set; }
        public decimal C_Setiembre { get; set; }
        public decimal C_Octubre { get; set; }
        public decimal C_Noviembre { get; set; }
        public decimal C_Diciembre { get; set; }
        public decimal T_Cantidad { get; set; }
        public decimal T_Total { get; set; }
        public string GrupoLlave { get; set; }
        public string TotalGrupoLlave { get; set; }
        public string v_IdVendedor { get; set; }
        public int DocContable { get; set; }
        public string IdProductoDetalle { get; set; }
        public string IdProductoAux { get; set; }
        
      


    }
}
