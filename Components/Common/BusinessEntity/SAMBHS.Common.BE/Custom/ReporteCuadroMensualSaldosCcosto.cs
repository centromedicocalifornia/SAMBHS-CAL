using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public class ReporteCuadroMensualSaldosCcosto
    {
        public string Cuenta { get; set; }
        public string NombreCuenta { get; set; }
        public decimal DebeSoles { get; set; }
        public decimal HaberSoles { get; set; }
        public decimal DebeDolares { get; set; }
        public decimal HaberDolares { get; set; }
        public string Mes { get; set; }
        public int imputable { get; set; }
        public decimal ValorEnero { get; set; }
        public decimal ValorFebrero { get; set; }
        public decimal ValorMarzo { get; set; }
        public decimal ValorAbril { get; set; }
        public decimal ValorMayo { get; set; }
        public decimal ValorJunio { get; set; }
        public decimal ValorJulio { get; set; }
        public decimal ValorAgosto { get; set; }
        public decimal ValorSetiembre { get; set; }
        public decimal ValorOctubre { get; set; }
        public decimal ValorNoviembre { get; set; }
        public decimal ValorDiciembre { get; set; }
        public decimal Total { get; set; }
        public string Ccosto { get; set; }
        public string    IdCcosto { get; set; }
        public string ValueCcosto { get; set; }
    }
}
