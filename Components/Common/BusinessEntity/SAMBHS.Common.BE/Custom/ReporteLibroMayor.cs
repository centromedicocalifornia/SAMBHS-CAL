using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace SAMBHS.Common.BE
{
    public class ReporteLibroMayor
    {
        public DateTime  fecha { get; set; }
        public string IdAnexo { get; set; }
        public string nroComprobante { get; set; }
        public string docReferencia { get; set; }
        public string nombre { get; set; }
        public string descripcionOperacion { get; set; }
        public decimal debeSoles { get; set; }
        public decimal debeDolares { get; set; }
        public decimal haberSoles { get; set; }
        public decimal haberDolares { get; set; }
        public string cuentaMayor { get; set; }
        public string cuenta { get; set; }
        //public int tipoComprobante { get; set; }
        public int cuentaInt { get; set; }
        public string numeroCuenta { get; set; }
        public decimal saldoAnteriorSoles { get; set; }
        public decimal saldoAnteriorDolares { get; set; }
        public decimal acumuladoAnteriorDebeSoles { get; set; }
        public decimal acumuladoAnteriorDebeDolares { get; set; }
        public decimal acumuladoAnteriorHaberSoles { get; set; }
        public decimal acumuladoAnteriorHaberDolares { get; set; }
        public decimal movimientoMesDebeSoles { get; set; }
        public decimal movimientoMesDebeDolares { get; set; }
        public decimal movimientoMesHaberSoles { get; set; }
        public decimal movimientoMesHaberDolares { get; set; }
        public decimal acumuladoActualDebeSoles { get; set; }
        public decimal acumuladoActualDebeDolares { get; set; }
        public decimal acumuladoActualHaberSoles { get; set; }
        public decimal acumuladoActualHaberDolares { get; set; }
        public string monedaTransaccion { get; set; }
        //public DateTime FechaInsercion { get; set; }
        public string FechaMostrar { get; set; }
        public string NroRegistro { get; set; }
        public int i_CodigoDocumento { get; set; }
        public string MonedaSiglas { get; set; }
        public int TipoDocumentoEmisor { get; set; }
        public string NroDocumentoEmisor { get; set; }
        public string TipoCliente { get; set; }
        public int TipoComprobanteDetalle { get; set; }
        public string SerieCorrelativoDetalle { get; set; }
       // public DateTime FechaDetalle { get; set; }
        public string IdCabecera { get; set; }
        public string Naturaleza { get; set; }
        public string NaturalezaCuenta { get; set; }
        public string SiglasComprobante { get; set; }
        public string NombreComprobante { get; set; }
        public string NaturalezaReal {
            get { return NaturalezaMayor == 1 ? "D" : "H"; }
        }
        public decimal Importe { get; set; }
        public decimal Cambio { get; set; }
        public int TipoComprobante { get; set; }
        public int NaturalezaMayor { get; set; }
    }
    public partial class saldoscontablesDto
    {
        public string Naturaleza { get; set; }
        public string CuentaMayor { get; set; }
        public string IdCentroCosto { get; set; }
        public int  i_IdPatrimonioNeto { get; set; }
        public bool CuentaConDetalle { get; set; }
    }

    public class AsientoConsumo
    {
        public string CuentaDebe { get; set; }
        public string CuentaHaber { get; set; }
        public decimal Debe { get; set; }
        public decimal Haber { get; set; }
        public decimal Importe { get; set; }
        public decimal Cambio { get; set; }
        public string Linea { get; set; }
    }
    public class ReporteAsientoConsumo {

        public decimal  DebeSoles { get; set; }
        public decimal  HaberSoles{ get; set; }
        public decimal DebeDolares { get; set; }
        public decimal HaberDolares { get; set; }
        public string Cuenta { get; set; }
        public string Linea { get; set; }
        public string Producto { get; set; }
        public string Movimiento { get; set; }
        public string LetrasSubTotalLinea { get; set; }
        public string CodigoProducto { get; set; }
        public string NroPedido { get; set; }
        public int i_EsDevolucion { get; set; }
        public decimal Cantidad { get; set; }
        public decimal TipoCambio { get; set; }
        public int IdMoneda { get; set; }
        public DateTime Fecha { get; set; }
        public string v_IdMovimientoDetalle { get; set; }
        public string CuentaDebe { get; set; }
        public string CuentaHaber { get; set; }
        public string sFecha { get; set; }
    
    }


    public class ProgramacionFabricacion
    {

        public string CodigoProducto { get; set; }
        public string DescripcionProducto { get; set; }
        public decimal Stock { get; set; }
        public decimal CantidadPedido { get; set; }
        public decimal SaldoUnidades { get; set; }
        public int  CantidadFabricacion { get; set; }
        public decimal SaldoDias { get; set; }
        public DateTime FechaFabricacion { get; set; }
        public string sFechaFabricacion { get; set; }
        public string Situacion { get; set; }
        public string IdProducto { get; set; }
    
    }

    public class ReporteTrazabilidad
    {
        
        public decimal Cantidad { get; set; }
        public string Lote { get; set; }
        public string FechaVencimiento { get; set; }
        public string Key { get; set; }
        public string KeyOrdenProduccion { get; set; }
        public string NroLote { get; set; }
        public string FechaCaducidad { get; set; }
        public string Almacen { get; set; }
        public string NroRegistro { get; set; }
        public string Fecha { get; set; }
    
    }
}
