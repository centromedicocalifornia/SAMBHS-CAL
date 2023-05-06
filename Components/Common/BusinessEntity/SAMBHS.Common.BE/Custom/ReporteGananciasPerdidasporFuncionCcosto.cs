using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public class ReporteGananciasPerdidasporFuncionCcosto : ICloneable
    {
        public string pstrCuenta { get; set; }
        public string pstrDenominacion { get; set; }
        public decimal MovimientoDebe { get; set; }
        public decimal MovimientoHaber { get; set; }
        public decimal perdida { get; set; }
        public decimal gananacia { get; set; }
        public string mes { get; set; }
        public decimal DebeSoles { get; set; }
        public decimal DebeDolares { get; set; }
        public decimal HaberDolares { get; set; }
        public decimal HaberSoles { get; set; }
        public int imputable { get; set; }
        public string Ccosto { get; set; }
        public string IdCcosto { get; set; }
        public string ValueCcosto { get; set; }
        public string Grupo { get; set; }
        public int iGrupo { get; set; }
        public decimal Acumulado { get; set; }
        public decimal Mensual { get; set; }
        public decimal Mes1 { get; set; }
        public decimal Mes2 { get; set; }

        public decimal Mes3 { get; set; }
        public decimal Mes4 { get; set; }
        public decimal Mes5 { get; set; }
        public decimal Mes6 { get; set; }
        public decimal Mes7 { get; set; }
        public decimal Mes8 { get; set; }
        public decimal Mes9 { get; set; }
        public decimal Mes10 { get; set; }
        public decimal Mes11 { get; set; }
        public decimal Mes12 { get; set; }
        public string NombreMes1 { get; set; }
        public string NombreMes2 { get; set; }

        public string NombreMes3 { get; set; }
        public string NombreMes4 { get; set; }
        public string NombreMes5 { get; set; }
        public string NombreMes6 { get; set; }
        public string NombreMes7 { get; set; }
        public string NombreMes8 { get; set; }
        public string NombreMes9 { get; set; }
        public string NombreMes10 { get; set; }
        public string NombreMes11 { get; set; }
        public string NombreMes12 { get; set; }

        public string Naturaleza { get; set; }
        public string IdTesoreriaDetalle { get; set; }
        public bool EsCuentaPerdidaFuncion { get; set; }
        public string GrupoCuenta { get; set; }
        public string NroDiario { get; set; }
        public string Analisis { get; set; }
        public decimal MensualCuenta { get; set; }
        public decimal AcumuladoCuenta { get; set; }


        public object Clone()
        {
            return MemberwiseClone();
        }


    }
}
