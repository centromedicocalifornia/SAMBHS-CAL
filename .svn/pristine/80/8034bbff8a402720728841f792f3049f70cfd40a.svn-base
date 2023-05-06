using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public class ImportacionRegistroVentaCobranzaDto
    {
        private readonly int[] _documentosSoportados = { 1, 3, 7, 8, 12 };
        /// <summary>
        /// Id del tipo de documento de venta.
        /// </summary>
        public int IdTipoDoc { get; set; }
        /// <summary>
        /// Serie del documento de venta.
        /// </summary>
        public string Serie { get; set; }
        /// <summary>
        /// Correlativo del documento de venta.
        /// </summary>
        public string Correlativo { get; set; }

        public decimal TipoCambio { get; set; }

        public string Moneda { get; set; }

        public decimal MontoCobrado { get; set; }

        public string FechaCobranza { get; set; }

        #region Tipo Cobranza
        public int IdTipoCobranza { get; set; }

        public string TipoCobranza
        {
            get { return IdTipoCobranza == 1 ? "CONTADO" : IdTipoCobranza == 2 ? "TARJETA" : "??"; }
        }
        #endregion

        #region Tipo Tarjeta
        private int? _idTipoTarjeta;

        public int? IdTipoTarjeta
        {
            get { return _idTipoTarjeta ?? 0; }
            set { _idTipoTarjeta = value ?? 0; }
        }

        public Tarjeta TarjetaEnum
        {
            get { return (Tarjeta)(IdTipoTarjeta ?? 0); }
        }

        public string TipoTarjeta
        {
            get
            {
                return !IdTipoTarjeta.HasValue
                    ? string.Empty
                    : IdTipoTarjeta.Value == 1
                        ? "VISA"
                        : IdTipoTarjeta.Value == 2 ? "MASTERCARD" : IdTipoTarjeta.Value == 3 ? "AMERICAN EXPRESS" : "??";
            }
        } 
        #endregion

        public bool Valido
        {
            get { return string.IsNullOrWhiteSpace(Errores); }
        }

        public string Errores
        {
            get
            {
                int i;
                var errores = new StringBuilder();
                if (IdTipoCobranza == 2 && IdTipoTarjeta.HasValue && IdTipoTarjeta.Value != 1 && IdTipoTarjeta.Value != 2 &&
                    IdTipoTarjeta.Value != 3)
                    errores.AppendLine("IdTipoTarjeta -> Tipo de tarjeta no reconocido");

                if (IdTipoCobranza != 1 && IdTipoCobranza != 2)
                    errores.AppendLine("IdTipoCobranza -> Tipo de Pago no reconocido");

                if (string.IsNullOrWhiteSpace(FechaCobranza))
                    errores.AppendLine("FechaCobranza -> Fecha de cobranza requerida");

                if (string.IsNullOrWhiteSpace(Moneda) || (Moneda != "S" && Moneda != "D"))
                    errores.AppendLine("Moneda -> Formato de moneda no reconocido.");

                if (TipoCambio <= 0)
                    errores.AppendLine("TipoCambio -> Tipo de cambio inválido");

                if (MontoCobrado <= 0)
                    errores.AppendLine("MontoCobrado -> Importe inválido");

                if (!((IList)_documentosSoportados).Contains(IdTipoDoc))
                    errores.AppendLine(string.Format("T. Doc. -> Documento no sportado ({0})", IdTipoDoc));

                if (!int.TryParse(Correlativo, out i) || i <= 0)
                    errores.AppendLine(string.Format("Correlativo -> Número de Correlativo incorrecto ({0})", Correlativo));

                return errores.ToString();
            }
        }

        public string NroDocumentoVenta
        {
            get
            {
                int i;
                var doc = IdTipoDoc == 1
                    ? "FAC"
                    : IdTipoDoc == 3 ? "BOL" : IdTipoDoc == 7 ? "NCR" : IdTipoDoc == 8 ? "NDB" : IdTipoDoc == 12 ? "TCK" : "??";
                var serie = int.TryParse(Serie, out i) ? i.ToString("0000") : Serie;
                var correlativo = int.TryParse(Correlativo, out i) ? i.ToString("00000000") : "??";
                return string.Format("{0} {1}-{2}", doc, serie, correlativo);
            }
        }

        public string EstadoRegistro { get { return string.IsNullOrWhiteSpace(Errores) ? "CORRECTO" : "CON ERRORES"; } }

        public string KeyVenta
        {
            get
            {
                int i;
                var serie = int.TryParse(Serie.Trim(), out i) ? i.ToString("0000") : Serie;
                var correlativo = int.TryParse(Correlativo.Trim(), out i) ? i.ToString("00000000") : "??";
                return IdTipoDoc + serie.Trim() + correlativo.Trim();
            }
        }

        public enum Tarjeta
        {
            Ninguna = 0,
            Visa = 1,
            Mastercard = 2,
            AmericanExpress = 3
        }
    }
}
