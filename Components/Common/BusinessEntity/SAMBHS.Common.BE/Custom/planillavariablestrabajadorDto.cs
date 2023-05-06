using System;
using System.Collections.Generic;

namespace SAMBHS.Common.BE
{
    public partial class planillavariablestrabajadorDto
    {
        public string NumeroPlanilla { get; set; }
        public string NombreTrabajador { get; set; }
        public string NumeroContrato { get; set; }

        public List<planillavariablesingresosDto> ListaIngresos
        {
            get;
            set;
        }

        public List<planillavariablesaportacionesDto> ListaAportaciones
        {
            get;
            set;
        }
    }

    public class PlanillaVariablesBandejaDto
    {
        public string IdVariableTrabajador { get; set; }
        public string NombreTrabajador { get; set; }
        public string Contrato { get; set; }
        public string IdTrabajador { get; set; }
        public string UsuarioCrea { get; set; }
        public string UsuarioActualiza { get; set; }
        public DateTime? t_ActualizaFecha { get; set; }
        public DateTime? t_InsertaFecha { get; set; }
        public string NroDocumentoTrabajador { get; set; }
    }
}
