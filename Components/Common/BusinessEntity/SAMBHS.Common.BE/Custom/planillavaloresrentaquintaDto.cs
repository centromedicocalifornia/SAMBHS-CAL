using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public partial class planillavaloresrentaquintaDto
    {
        public string ConceptoRenta5T { get; set; }
        public string ConceptoGratificacion { get; set; }

        public PlanillavaloresrentaquintaUnidad this[int posicion]
        {
            get
            {
                switch (posicion)
                {
                    case 1:
                        return new PlanillavaloresrentaquintaUnidad { Porcentaje = d_Porcentaje1 ?? 0, Tope = i_Tope1 ?? 0, TopeAnterior = 0 };
                    case 2:
                        return new PlanillavaloresrentaquintaUnidad { Porcentaje = d_Porcentaje2 ?? 0, Tope = i_Tope2 ?? 0, TopeAnterior = i_Tope1 ?? 0 };
                    case 3:
                        return new PlanillavaloresrentaquintaUnidad { Porcentaje = d_Porcentaje3 ?? 0, Tope = i_Tope3 ?? 0, TopeAnterior = i_Tope2 ?? 0 };
                    case 4:
                        return new PlanillavaloresrentaquintaUnidad { Porcentaje = d_Porcentaje4 ?? 0, Tope = i_Tope4 ?? 0, TopeAnterior = i_Tope3 ?? 0 };
                    case 5:
                        return new PlanillavaloresrentaquintaUnidad { Porcentaje = d_Porcentaje4Superior ?? 0, Tope = -1, TopeAnterior = i_Tope4 ?? 0 };
                    default:
                        return new PlanillavaloresrentaquintaUnidad();
                }
            }
        }
    }

    public struct PlanillavaloresrentaquintaUnidad
    {
        public int Tope { get; set; }
        public int TopeAnterior { get; set; }
        public decimal Porcentaje { get; set; }
        public decimal Multiplo { get { return Tope - TopeAnterior; } }
    }
}
