using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace SAMBHS.Common.BE.Custom
{
    public class Medicina_Tickets
    {
        public string Servicio { get; set; }
        public string Comprobante { get; set; }
        public string Paciente { get; set; }
        public string Protocolo { get; set; }
        public string Titular { get; set; }
        public string DNI { get; set; }
        public string Empresa { get; set; }
        public string Medico { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public DateTime? FECHA_SERVICIO { get; set; }
        public string Usuario { get; set; }

        public string Medicina { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public decimal PagoPaciente { get; set; }
        public decimal PagoSeguro { get; set; }

        public string Plan { get; set; }
        public decimal Factor { get; set; }
        public string Descuento_PPS { get; set; }
        public string Deducible { get; set; }
        public string Coaseguro { get; set; }
        public string Ticket { get; set; }
        public string TicketDetalle { get; set; }
        public string Habitacion { get; set; }

    }
}
