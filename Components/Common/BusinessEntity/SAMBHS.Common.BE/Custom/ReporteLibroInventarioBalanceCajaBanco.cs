using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public class ReporteLibroInventarioBalanceCajaBanco
    {
        public string CuentaMayor { get; set; }
        public string CuentaImputable { get; set; }
        public string NombreCuenta { get; set; }
        public string Detalle { get; set; }
        public string Documento { get; set; }
        public string DocumentoRef { get; set; }
        public DateTime? FechaDocumento { get; set; }
        public string DocumentoKey { get; set; }
        public int IdMoneda { get; set; }
        public int IdMonedaCuenta { get; set; }
        public string NaturalezaCuenta { get; set; }
        public string NaturalezaRegistro { get; set; }
        public decimal? ImporteSolesD { get; set; }
        public decimal? ImporteSolesH { get; set; }
        public decimal? ImporteDolaresD { get; set; }
        public decimal? ImporteDolaresH { get; set; }
        public bool CuentaConDetalle { get; set; }
        public decimal TipoCambio { get; set; }
        public string  IdAnexo { get; set; }
        public int TipoDocumento { get; set; }
        public string NroDocumento { get; set; }
        public string EntidadFinanceriaCuenta { get; set; }
        public string TipoMonedaCuenta { get; set; }
       // public string NombreCliente { get; set; }
        public string Cliente { get; set; }
        public DateTime? FechaComprobante { get; set; }
        public string Concepto { get; set; }
        public string NroCuentaEntidadFinanciera { get; set; }

    }

    public class ReporteBalanceCapital

    {

        public string Grupo { get; set; }
        public string Detalle { get; set; }
        public decimal DetalleValor { get;set;}
    
    }
    public class ReporteSocios
    {
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string RazonSocial { get; set; }
        public string TipoAccionSocio { get; set; }
        public decimal NumeroAcciones { get; set; }
        public string  Porcentaje { get; set; }
    
    }
}
