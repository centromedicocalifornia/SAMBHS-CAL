using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    public class EnumsSigesoft
    {
        public enum ComponenteProcedencia
        {
            Interno = 1,
            Externo = 2
        }

        public enum SiNo
        {
            NO = 0,
            SI = 1,
            NONE = 2
        }

        public enum ServiceStatus
        {
            PorIniciar = 1,
            Iniciado = 2,
            Culminado = 3,
            Incompleto = 4,
            Cancelado = 5,
            EsperandoAptitud = 6
        }

        public enum QueueStatusId
        {
            LIBRE = 1,
            LLAMANDO = 2,
            OCUPADO = 3
        }

        public enum Flag_Call
        {
            NoseLlamo = 0,
            Sellamo = 1
        }

        public enum ModoCargaImagen
        {
            DesdeArchivo = 1,
            DesdeDispositivo = 2

        }
    }
}
