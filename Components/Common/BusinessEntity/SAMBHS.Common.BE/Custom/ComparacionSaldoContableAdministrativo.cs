using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE.Custom
{
    /// <summary>
    /// Clase que sirve para la consulta de saldos contables y administrativos para su posterior regularización.
    /// </summary>
    public class ComparacionSaldoContableAdministrativo
    {
        /// <summary>
        /// Id De la venta de la que se consulta su saldo.
        /// </summary>
        public string IdVenta { get; set; }
        /// <summary>
        /// Tipo de documento de la venta, indica si es factura, boleta, etc..
        /// </summary>
        public int IdTipoDocumento { get; set; }
        /// <summary>
        /// ID de la tabla de cobranzas pendientes donde se almacena el saldo administrativo
        /// </summary>
        public string IdCobranzaPendiente { get; set; }
        /// <summary>
        /// Serie concadenada al correlativo del documento de venta
        /// </summary>
        public string SerieCorrelativo { get; set; }
        /// <summary>
        /// Saldo contable de la venta
        /// </summary>
        public decimal SaldoContable { get; set; }
        /// <summary>
        /// Saldo Administrativo de la venta
        /// </summary>
        public decimal SaldoAdministrativo { get; set; }
        /// <summary>
        /// Anexo del registro, indica el nombre del cliente o proveedor.
        /// </summary>
        public string Anexo { get; set; }
    }
}
