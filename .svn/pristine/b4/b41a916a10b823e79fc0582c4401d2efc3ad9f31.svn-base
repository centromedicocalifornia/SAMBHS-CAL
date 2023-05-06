using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public partial class pagopendienteDto
    {
        public string v_IdProveedor { get; set; }
        public string NombreProveedor { get; set; }
        public string TipoDocumento { get; set; }
        public string NroDocumento { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Moneda { get; set; }
        public decimal ValorCompra { get; set; }
        public decimal Importe { get; set; }
        public int? i_IdTipoDocumento { get; set; }
        public decimal TipoCambio { get; set; }
        public bool EsLetra { get; set; }
        public string Ubicacion { get; set; }
        public string UbicacionNombreCompleto { get; set; }
        public int EsDocInverso { get; set; }
    }


    public class GrillaPagoDetalleDto
    {
        public string v_IdPagoDetalle { get; set; }
        public string v_IdPago { get; set; }
        public string v_IdCompra { get; set; }
        public string v_Observacion { get; set; }
        public string v_DocumentoRef { get; set; }
        public int? i_IdFormaPago { get; set; }
        public int? i_IdTipoDocumentoRef { get; set; }
        public int? i_IdMoneda { get; set; }
        public string TipoDocumento { get; set; }
        public string NroDocumento { get; set; }
        public string NombreProveedor { get; set; }
        public string MonedaCompra { get; set; }
        public int? IdMonedaCompra { get; set; }
        public string MonedaOriginal { get; set; }
        public int? i_EsLetra { get; set; }
        public decimal? d_ImporteSoles { get; set; }
        public decimal? d_ImporteDolares { get; set; }
        public decimal? d_NetoXCobrar { get; set; }
        public int i_Eliminado { get; set; }
        public int i_InsertaIdUsuario { get; set; }
        public DateTime t_InsertaFecha { get; set; }
    }
    public partial class pagodetalleDto
    {
        public string Moneda { get; set; }

    }

    public partial class pagoDto
    {
        public string NroRegistro { get; set; }
        public string TipoDocumento { get; set; }
        public string v_UsuarioModificacion { get; set; }
        public string v_UsuarioCreacion { get; set; }
        public string MedioPago { get; set; }
        public string Moneda { get; set; }
        public string IdCompra { get; set; }
    
    }

    public class ReporteCuentasPagarDto
    {

        public string Correlativo { get; set; }
        public string NombreCliente { get; set; }
        public string MedioPago { get; set; }
        public DateTime FechaEmision { get; set; }
        public string TipoDocumento { get; set; }
        public string NroDocumento { get; set; }
     //   public string GuiaRemision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string Vendedor { get; set; }
        public string Moneda { get; set; }
        public decimal? TotalFacturado { get; set; }
        public decimal? Acuenta { get; set; }
        public decimal? Saldo { get; set; }
        public decimal SaldoDolares { get; set; }
        public string MonedaCobranza { get; set; }
        public string Grupollave { get; set; }
        public string NombreGrupo { get; set; }
        public string v_CodigoProveedor { get; set; }
        public int idTipoDocumento { get; set; }
        public string v_IdVendedor { get; set; }
        public string v_SerieDocumento { get; set; }
        public string v_CorrelativoDocumento { get; set; }
        public string Glosa { get; set; }
        public decimal? Soles { get; set; }
        public decimal? Dolares { get; set; }
        public string sFechaVencimiento { get; set; }
        
    }

    public class ReportePlanillaPagos
    {

        public string TipoDocumento { get; set; }
        public string NroDocumento { get; set; }
        public DateTime? FechaEmision { get; set; }
        public string NombreProveedor { get; set; }
        public string Glosa { get; set; }
        public string Cheq { get; set; }
        public string Moneda { get; set; }
        public decimal? TotalFacturado { get; set; }
        public decimal? TotalFacturadoDolares { get; set; }
        public decimal? MontoPagado { get; set; }
        public decimal? MontoPagadoDolares { get; set; }
        public decimal? Saldo { get; set; }
        public decimal? SaldoDolares { get; set; }
        public int? idTipoDocumento { get; set; }
        public string MedioPago { get; set; }
        public int? MedioPagoCobranza { get; set; }
        public string v_IdCobranzaPendiente { get; set; }
        public decimal? MontoCobrar { get; set; }
        public decimal? MontoCobrarDolares { get; set; }
        public decimal Valor { get; set; }
        public decimal? IdTipoDocumentoCobranzaDetalle { get; set; }
        public string MonedaCobranza { get; set; }
        public string Grupollave { get; set; }
        public string NombreGrupo { get; set; }
        public string TipoDocumentoCobranza { get; set; }
        public string NroDocumentoCobranza { get; set; }
        public int IdTipoDocumentoReferenciaCobranzaDetalle { get; set; }
        public string NumeroDocumentoReferenciaCobranzaDetalle { get; set; }
        public string FormaPago { get; set; }
    
    
    }


    public class ReporteEstadoCuentaProveedor
    {
        public string Proveedor { get; set; }
        public DateTime? Fecha { get; set; }
        public int pIntTipoDocumento { get; set; }
        public string Concepto { get; set; }
        public string Referencia { get; set; }
        public decimal  DebeSoles { get; set; }
        public decimal  DebeDolares { get; set; }
        public decimal HaberSoles { get; set; }
        public decimal HaberDolares { get; set; }
        public string IdPagoDetalle { get; set; }
        public DateTime FechaInsercion { get; set; }
        public string DocumentoReferencia {get;set ;}
        public string  Naturaleza { get; set; }
        public string Movimiento { get; set; }
        public string IdLetras { get; set; }
        public string IdLetrasDetalle { get; set; }
    }
}
