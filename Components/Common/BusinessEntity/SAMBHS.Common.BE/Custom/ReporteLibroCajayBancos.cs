using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public class ReporteLibroCajayBancos : ICloneable
    {

        public DateTime fecha { get; set; }
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
        public int tipoComprobante { get; set; }
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
        public string medioPago { get; set; }
        public string  entidadFinanciera { get; set; }
        public string  MaxNumeroCuenta { get; set; }
        public string IdTesoreria { get; set; }
        public int Apertura { get; set; }
        public string Grupo { get; set; }
        public string cuentaGrupo { get; set; }
        public string FechaS { get; set; }
        public string v_IdTesoreriaDetalle { get; set; }
        public string OrdenAdicional { get; set; }
       


        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
