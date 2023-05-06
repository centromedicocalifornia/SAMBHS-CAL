namespace SAMBHS.Common.BE.Custom
{
    public class CalculoFlujoEfectivoDto
    {
        public string CtaMayor { get; set; }
        public int? TipoActividad { get; set; }
        public string DescripcionCuenta { get; set; }
        public decimal BalancePeriodoActual { get; set; }
        public decimal BalancePeriodoAnterior { get; set; }
        public decimal Aumento
        {
            get
            {
                if (CtaMayor == null || CtaMayor.Equals("*N") || int.Parse(CtaMayor) >= 60) return 0;
                if (NaturalezaCuenta > 0 && BalancePeriodoActual > BalancePeriodoAnterior)
                    return BalancePeriodoActual - BalancePeriodoAnterior;
                return 0;
            }
        }

        public decimal Disminucion
        {
            get
            {
                if (CtaMayor == null ||  CtaMayor.Equals("*N") || int.Parse(CtaMayor) >= 60) return 0;
                if (NaturalezaCuenta > 0 && BalancePeriodoAnterior > BalancePeriodoActual)
                    return BalancePeriodoAnterior - BalancePeriodoActual;
                return 0;
            }
        }

        public decimal AjusteDebe { get; set; }
        public decimal AjusteHaber { get; set; }

        public decimal Operacion
        {
            get
            {
                if ((TipoActividad ?? 0) < 0 || TipoActividad != 1) return 0;
                var importe = Aplicacion > 0 ? Aplicacion * -1 : Origen;
                return importe;
            }
        }

        public decimal Inversion
        {
            get
            {
                if ((TipoActividad ?? 0) < 0 || TipoActividad != 2) return 0;
                var importe = Aplicacion > 0 ? Aplicacion * -1 : Origen;
                return importe;
            }
        }

        public decimal Financiamiento
        {
            get
            {
                if ((TipoActividad ?? 0) < 0 || TipoActividad != 3) return 0;
                var importe = Aplicacion > 0 ? Aplicacion * -1 : Origen;
                return importe;
            }
        }

        public decimal MetodoDirecto
        {
            get
            {
                if (CtaMayor == null || CtaMayor.Equals("10")) return 0;
                if (CtaMayor.Equals("89")) return BalancePeriodoActual;
                return Disminucion - Aumento;
            }
        }

        public int NaturalezaCuenta { get; set; }
        public int NroAsientoD { get; set; }
        public int NroAsientoH { get; set; }
        public decimal Aplicacion
        {
            get
            {
                var debe = Aumento + AjusteDebe;
                var haber = Disminucion + AjusteHaber;
                var resultado = debe - haber;
                return resultado > 0 ? resultado : 0;
            }
        }

        public decimal Origen
        {
            get
            {
                var debe = Aumento + AjusteDebe;
                var haber = Disminucion + AjusteHaber;
                var resultado = debe - haber;
                return resultado < 0 ? resultado * -1 : 0;
            }
        }
    }
}
