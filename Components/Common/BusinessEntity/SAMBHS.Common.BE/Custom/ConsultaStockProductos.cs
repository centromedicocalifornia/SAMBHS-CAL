
using System;

namespace SAMBHS.Common.BE
{
    public class ConsultaStockProductos
    {
        /// <summary>
        /// Llave usada para recalcular los stocks y separaciones
        /// </summary>
        public string Key
        {
            get { return string.Format("{0}-{1}-{2}", IdProductoDetalle, Serie, (NroLote ?? string.Empty).Trim()).Trim(); }
        }

        private string _serie;
        private string _nroLote;
        private DateTime? _fechaCaducidad;

        public string Serie
        {
            get { return _serie; }
            set { _serie = value ?? string.Empty; }
        }
        
        public string NroLote
        {
            get { return _nroLote; }
            set { _nroLote = value ?? string.Empty; }
        }
        
        public DateTime? FechaCaducidad
        {
            get { return _fechaCaducidad ?? DateTime.MinValue; }
            set { _fechaCaducidad = value ?? DateTime.MinValue; }
        }

        public string IdProductoDetalle { get; set; }
        public decimal Cantidad { get; set; }
        public int Almacen { get; set; }
        public string TipoMovimientoReal { get; set; }

    }
}
