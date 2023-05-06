﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    public class ConsultaServicioDto
    {
        public ConsultaServicioDto()
        {
            Seleccionar = false;
        }
        
        public bool Seleccionar { get; set; }
        public string ServiceId { get; set; }
        public string CalendarId { get; set; }
        public string ProtocolId { get; set; }
        public string PersonId { get; set; }
        public string MasterServiceId { get; set; }
        public string TipoServicio { get; set; }
        public string ServiceStatusId { get; set; }
        public string Estado { get; set; }
        public string AptitudeStatusId { get; set; }
        public string Organizacion { get; set; }
        public string Establecimiento { get; set; }
        public string MasterServiceType { get; set; }
        public string MasterServiceTypeId { get; set; }
        public string EsoId { get; set; }
        public string Protocolo { get; set; }
        public string Aptitud { get; set; }
        public string TipoDocumento { get; set; }
        public string NroDocumento { get; set; }
        public string Eso { get; set; }
        public string GrupoOcupacion { get; set; }
        /// <summary>
        /// Nombre del paciente
        /// </summary>
        public string C7 { get; set; }
        /// <summary>
        /// Fecha del servicio
        /// </summary>
        public DateTime C3 { get; set; }
        public string UsuarioCrea  { get; set; }
        public string UsuarioActualiza { get; set; }
        public DateTime FechaCrea { get; set; }
        public DateTime FechaActualiza { get; set; }

        public decimal d_PagoMedico { get; set; }
        public decimal d_PagoPaciente { get; set; }

        public decimal Total { get; set; }
    }
}
