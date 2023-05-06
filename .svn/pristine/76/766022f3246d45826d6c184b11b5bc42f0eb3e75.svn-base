using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE.Custom
{
    public class ReporteAnalisisCuentas
    {
        public string DocumentoKey { get; set; }
        public string CuentaMayor { get; set; }
        public string CuentaImputable { get; set; }
        public string Detalle { get; set; }
        public string Documento { get; set; }
        public string DocumentoRef { get; set; }
        public decimal? ImporteSoles { get; set; }
        public decimal? ImporteDolares { get; set; }
        public string NombreCuenta { get; set; }
        public string NombreCuentaMayor { get; set; }
        public DateTime? FechaDocumento { get; set; }
        public string NaturalezaCuenta { get; set; }
        public string NaturalezaRegistro { get; set; }
        public int? IdMonedaCuenta { get; set; }
        public int IdMoneda { get; set; }
        public string NombreRazonSocial { get; set; }
        public decimal? ImporteSolesD { get; set; }
        public decimal? ImporteDolaresD { get; set; }
        public decimal? ImporteSolesH { get; set; }
        public decimal? ImporteDolaresH { get; set; }
        public bool CuentaConDetalle { get; set; }
        public string IdAnexo { get; set; }
        public int TipoDocumento { get; set; }
        public string NroDocumento { get; set; }
        public string Doc { get; set; }
        public DateTime? DocFecha { get; set; }
        public string DocTipo { get; set; }
        public string CodAnexo { get; set; }
        public bool VentaDetraccion { get; set; }
        public decimal VentaTasaDetraccion { get; set; }
    }

    public class ReporteAnalisisCuentasCteAnalitico :ICloneable 
    {
        public ReporteAnalisisCuentasCteAnalitico()
        {
            Sospechoso = false;
        }
        public string DocumentoKey { get; set; }
        public string CuentaMayor { get; set; }
        public string CuentaImputable { get; set; }
        public string Detalle { get; set; }
        public string Documento { get; set; }
        public string DocumentoRef { get; set; }
        public decimal? ImporteSolesD { get; set; }
        public decimal? ImporteDolaresD { get; set; }
        public decimal? ImporteSolesH { get; set; }
        public decimal? ImporteDolaresH { get; set; }
        public string NombreCuenta { get; set; }
        public string NombreCuentaMayor { get; set; }
        public DateTime? FechaDocumento { get; set; }
        public string NaturalezaCuenta { get; set; }
        public string NaturalezaRegistro { get; set; }
        public int? IdMonedaCuenta { get; set; }
        public int IdMoneda { get; set; }
        public bool CuentaConDetalle { get; set; }
        public decimal TipoCambio { get; set; }
        public string IdAnexo { get; set; }
        public int TipoDocumento { get; set; }
        public string NroDocumento { get; set; }
        public int TipoDocumentoRef { get; set; }
        public string EntidadFinanceriaCuenta { get; set; }
        public string Analisis { get; set; }
        public string Detalle_TipoIdentificacion { get; set; }
        public string Detalle_Ruc { get; set; }
        public int IdTipoDocumentoProvicion { get; set; }
        public decimal? ImporteSoles
        {
            get {
                return ImporteSolesD > 0 ? ImporteSolesD : ImporteSolesH;
            }
        }

        public decimal? ImporteDolares
        {
            get {
                return ImporteDolaresD > 0 ? ImporteDolaresD : ImporteDolaresH;
            }
        }

        public string Moneda
        {
            get { return IdMoneda == 1 ? "S" : "D"; }
        }

        public string DocumentoProvicion { get; set; }
        public string IdDocumentoProvicion { get; set; }
        public string IdDocumentoProvicionDetalle { get; set; }
        public bool EsAjusteDiferenciaCambio { get; set; }
        public bool Sospechoso { get; set; }
        public string DocumentoRaw { get; set; }
        public string DocumentoRefRaw { get; set; }
        public string IdCentroCostos { get; set; }
        public string Mes { get; set; }
        public string CentroCosto { get; set; }
        public int i_IdTipoComprobante { get; set; }
        public string NroCuentaEntidadFinanciera { get; set; }
        public string NombreCliente { get; set; }
        public string NroRegistro { get; set; }
        public string FlagCliente {get;set;}
        public DateTime FechaDetalle { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimieto { get; set; }
        public string CodigoCliente { get; set; }
        public int i_IdPatrimonioNeto { get; set; }
        public string Grupo1 { get; set; }
        public string Grupo2 { get; set; }
        public decimal TasaDetraccionVenta { get; set; }
        public string CuentaDetraccion { get; set; }
        public int TipoOperacionDetraccion { get; set; }
        public int i_CodigoDetraccion { get; set; }

         public object Clone()
        {
            return MemberwiseClone();
        } 
    }
}
