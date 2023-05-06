using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SAMBHS.Common.BE
{
    public partial class planillacalculoDto
    {
        public string NombreTrabajador { get; set; }
        public string NombreConcepto { get; set; }
    }

    public class ReportePlanillaBoleta  : ICloneable 
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public decimal? ImporteIngreso { get; set; }
        public string NombreConceptoIngreso { get; set; }
        public decimal? ImporteDescuentos { get; set; }
        public string NombreConceptoDescuentos { get; set; }
        public decimal? ImporteAportaciones { get; set; }
        public string NombreConceptoAportaciones { get; set; }
        public string NombreTrabajador { get; set; }
        public string CodigoTrabajador { get; set; }
        public string CargoTrabajador { get; set; }
        public string CondicionLaboral { get; set; }
        public string Vacaciones { get; set; }
        public int DiasLaborados { get; set; }
        public string AutogeneradoEsSalud { get; set; }
        public DateTime FechaIngreso { get; set; }
        public string FechaCese { get; set; }
        public string NroDocumento { get; set; }
        public string RegimenPensionario { get; set; }
        public int DiasInasistencia { get; set; }
        public string CarnetAfp { get; set; }
        public decimal Tardanza { get; set; }
        public string Periodo { get; set; }
        public string Mes { get; set; }
        public decimal HorasTrabajadas { get; set; }
        public decimal  HorasExtrasTrabajadas { get; set; }
        public decimal HE { get; set; }
        public string IdTrabajador { get; set; }
        public Byte[] LogoEmpresa { get; set; }
        public Byte[] FirmaEmpleador{ get; set; }
        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public class PlanillaAsientoConsulta
    {
        public int IdPlanilla { get; set; }
        public string CodigoPlanilla { get; set; }
        public string NombrePlanilla { get; set; }
        public int NroTrabajadores { get; set; }
        public bool _Check { get; set; }
    }

    
}
