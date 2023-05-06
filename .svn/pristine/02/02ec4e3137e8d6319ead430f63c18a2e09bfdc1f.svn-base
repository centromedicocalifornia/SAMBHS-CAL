using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public class compradetalleDtoShort
    {
        public string Nombre { get; set; }
        public string CodigoInterno { get; set; }
        public decimal? Empaque { get; set; }
        public string UMEmpaque { get; set; }
        public string NombreCuenta { get; set; }
        public string NombreDestino { get; set; }
        public string NombreCentroCostos { get; set; }
        public string v_Nombre { get; set; }
        
    }
    public partial class compradetalleDto
    {
        public bool _DesdeODC { get; set; }
        public string SerieDocumento { get; set; }
        public string CorrelativoDocumento { get; set; }
        public int TipoDocumento { get; set; }
        public string RegistroKey { get; set; }
        public decimal TotalDolares { get; set; }
        public int EsActivoFijo { get; set; }
        public string IdAnexo { get; set; }
        public int i_SolicitarNroLoteIngreso { get; set; }
        public int i_SolicitarNroSerieIngreso { get; set; }
        public int i_SolicitaOrdenProduccionIngreso { get; set; }

        #region Campos para el cambio de moneda

        public decimal tipoCambio_ { get; set; }
        public bool incluyeIgv_ { get; set; }
        public int idMoneda_ { get; set; }

        public decimal d_Precio_
        {
            get
            {
                try
                {
                    if (tipoCambio_ == 0) return 0;
                    var precio = d_Precio ?? 0;
                    return idMoneda_ == 1 ? precio / tipoCambio_ : precio * tipoCambio_;
                }
                catch (Exception)
                {
                    return 0;
                }

            }
        }

        public decimal d_ValorVenta_
        {
            get
            {
                try
                {
                    if (tipoCambio_ == 0) return 0;
                    d_ValorVenta = d_ValorVenta ?? 0;
                    var result = idMoneda_ == 1 ? d_ValorVenta / tipoCambio_ : d_ValorVenta * tipoCambio_;
                    return decimal.Round(result ?? 0, 2, MidpointRounding.AwayFromZero);
                }
                catch (Exception)
                {
                    return 0;
                }

            }
        }

        public decimal d_Igv_
        {
            get
            {
                try
                {
                    if (tipoCambio_ == 0) return 0;
                    decimal? result;
                    if (incluyeIgv_)
                        result = decimal.Round((idMoneda_ == 1 ? d_PrecioVenta / tipoCambio_ : d_PrecioVenta * tipoCambio_) ?? 0, 2, MidpointRounding.AwayFromZero) -
                                 decimal.Round((idMoneda_ == 1 ? d_ValorVenta / tipoCambio_ : d_ValorVenta * tipoCambio_) ?? 0, 2, MidpointRounding.AwayFromZero);
                    else result = decimal.Round((idMoneda_ == 1 ? d_Igv / tipoCambio_ : d_Igv * tipoCambio_) ?? 0, 2, MidpointRounding.AwayFromZero);

                    return result ?? 0;
                }
                catch (Exception)
                {
                    return 0;
                }

            }
        }

        public decimal d_PrecioVenta_
        {
            get
            {
                try
                {
                    if (tipoCambio_ == 0) return 0;
                    var result = incluyeIgv_
                        ? decimal.Round((idMoneda_ == 1 ? d_PrecioVenta / tipoCambio_ : d_PrecioVenta * tipoCambio_) ?? 0, 2, MidpointRounding.AwayFromZero)
                        : decimal.Round((idMoneda_ == 1 ? d_ValorVenta / tipoCambio_ : d_ValorVenta * tipoCambio_) ?? 0, 2, MidpointRounding.AwayFromZero) +
                        decimal.Round((idMoneda_ == 1 ? d_Igv / tipoCambio_ : d_Igv * tipoCambio_) ?? 0, 2, MidpointRounding.AwayFromZero);

                    return result;
                }
                catch (Exception)
                {
                    return 0;
                }

            }
        }

        #endregion
    }
}
