using System;

namespace SAMBHS.Common.BE
{
    public partial class planillaasistenciaDto
    {
        public DateTime FechaInicioAreaLaboral { get; set; }
        public string Trabajador { get; set; }
        public string NroDocIdentidad { get; set; }
        public int IdTurno { get; set; }
        /// <summary>
        /// Horas que le corresponde trabajar al mes.
        /// </summary>
        public TimeSpan HorasMensuales { get; set; }
        /// <summary>
        /// Horas que le corresponde trabajar al día.
        /// </summary>
        public TimeSpan HorasCorrespondidas { get; set; }
        /// <summary>
        /// Horas que trabajó en el día según la asistencia.
        /// </summary>
        public TimeSpan HorasNormales
        {
            get
            {
                var primerTurno = t_Salida_I - t_Ingreso_I;
                var segundoTurno = t_Salida_II - t_Ingreso_II;
                return (primerTurno ?? TimeSpan.Zero) + (segundoTurno ?? TimeSpan.Zero);
            }
        }
        /// <summary>
        /// Retorna el tiempo de horas extras al 25%
        /// </summary>
        public TimeSpan Minutos25
        {
            get
            {
                if (!HorasExtrasAutorizadas) return TimeSpan.Zero;
                if (HorasNormales <= HorasCorrespondidas) return TimeSpan.Zero;
                var tiempoExtra = HorasNormales - HorasCorrespondidas;
                if (tiempoExtra > TimeSpan.FromHours(1))
                    return tiempoExtra < TimeSpan.FromHours(2) ? tiempoExtra : TimeSpan.FromHours(2);
                
                return TimeSpan.Zero;
            }
        }
        /// <summary>
        /// Retorna el tiempo de horas extras al 35%
        /// </summary>
        public TimeSpan Minutos35
        {
            get
            {
                if (!HorasExtrasAutorizadas) return TimeSpan.Zero;
                if (HorasNormales <= HorasCorrespondidas) return TimeSpan.Zero;
                var tiempoExtra = HorasNormales - HorasCorrespondidas;
                if (tiempoExtra > TimeSpan.FromHours(2))
                    return tiempoExtra - Minutos25;
                return TimeSpan.Zero;
            }
        }
        /// <summary>
        /// Retorna el tiempo de tardanza.
        /// </summary>
        public TimeSpan MinutosTardanza {
            get
            {
                var horaIngreso = t_Ingreso_I_Turno;
                if( horaIngreso == DateTime.MinValue) return TimeSpan.Zero;
                if (t_Ingreso_I > horaIngreso)
                    return (t_Ingreso_I ?? DateTime.MinValue) - horaIngreso.Value;
                return TimeSpan.Zero;
            }
        }

        public string xls_IngresoI {
            get
            {
                var fecha = t_Ingreso_I ?? DateTime.Now;
                return string.Format("{0:00}:{1:00}", fecha.Hour, fecha.Minute);
            }
        }
        public string xls_SalidaI
        {
            get
            {
                var fecha = t_Salida_I ?? DateTime.Now;
                return string.Format("{0:00}:{1:00}", fecha.Hour, fecha.Minute);
            }
        }
        public string xls_IngresoII
        {
            get
            {
                var fecha = t_Ingreso_II ?? DateTime.Now;
                return string.Format("{0:00}:{1:00}", fecha.Hour, fecha.Minute);
            }
        }
        public string xls_SalidaII
        {
            get
            {
                var fecha = t_Salida_II ?? DateTime.Now;
                return string.Format("{0:00}:{1:00}", fecha.Hour, fecha.Minute);
            }
        }

        public bool RequiereHorasExtras {
            get { return HorasNormales > HorasCorrespondidas; }
        }

        public bool HorasExtrasAutorizadas { get; set; }      
  
    }
}
