using System;
using System.Collections.Generic;
namespace SAMBHS.Common.BE
{
    public partial class asientocontableDto
    {
        public NaturalezaCta NaturalezaCuenta
        {
            get { return i_Naturaleza == 2 ? NaturalezaCta.Acreedora : NaturalezaCta.Deudora; }
        }

    }

    public partial class configuracionbalancesDto :ICloneable
    {

        public IEnumerable<string> ListaCuentasUsadas { get; set; }
        public string CtasVistaRapida { get; set; }
        public string v_UsuarioModificacion { get; set; }
        public string v_UsuarioCreacion { get; set; }
        public string NombreNota { get; set; }
        public string DescripcionNota { get; set; }
        public string Mesecito { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }

    }

    public enum NaturalezaCta
    {
        Deudora = 1,
        Acreedora = 2
    }
}