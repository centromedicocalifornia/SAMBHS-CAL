using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public partial class productoalmacenDto
    {
        #region Por corregir
        public decimal? Costo;
        public decimal? d_Descuento;
        public decimal? d_Precio;
        public decimal? d_PrecioMinDolares;
        public decimal? d_PrecioMinSoles;
        public decimal? d_Utilidad;
        public string v_IdProductoDetalle { get; set; } 
        #endregion

        /// <summary>
        /// Llave usada para recalcular los stocks y separaciones
        /// </summary>
        public string Key
        {
            get { return string.Format("{0}-{1}-{2}", v_ProductoDetalleId, Serie, (NroLote?? string.Empty).Trim()); }
        }

        public string Serie
        {
            get { return v_NroSerie; }
            set { v_NroSerie = value ?? string.Empty; }
        }

        public string NroLote
        {
            get { return v_NroLote; }
            set { v_NroLote = value ?? string.Empty; }
        }

        public DateTime? FechaCaducidad
        {
            get { return t_FechaCaducidad ?? DateTime.MinValue; }
            set { t_FechaCaducidad = value ?? DateTime.MinValue; }
        }
    }
}
