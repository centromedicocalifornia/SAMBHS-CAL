using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Windows.SigesoftIntegration.UI.Dtos
{
    public class DigitalContactCenterDto
    {
        public string ID_DCC { get; set; }

        public string ID_Person { get; set; }

        public int? TIPO_DOC_ID { get; set; }

        public string TIPO_DOC { get; set; }

        public string DOC { get; set; }

        public string NOMBRES { get; set; }

        public string AP_PATERNO { get; set; }
        
        public string AP_MATERNO { get; set; }

        public string CELULAR { get; set; }

        public string EMAIL { get; set; }

        public string DIRECCION { get; set; }

        public int? MEDIO_MKT_ID { get; set; }

        public string MEDIO_MKT { get; set; }

        public DateTime? FECHA_CITA { get; set; }

        public string PROTOCOL_ID { get; set; }

        public string PROTOCOL_NAME { get; set; }

        public string MOTIVO { get; set; }

        public int? METODO_PAGO_ID { get; set; }

        public string METODO_PAGO { get; set; }

        public int? ESTADO_DCC_ID { get; set; }

        public string ESTADO_DCC { get; set; }

        public Byte[] COMPROBANTE_ADJUNTO { get; set; }

        public string SERVICIO_ENLAZADO { get; set; }

        public string MOTIVO_ELIMINACION { get; set; }

        public int? ELIMINADO { get; set; }

        public int? ID_INSERT_USER { get; set; }

        public string INSERT_USER { get; set; }

        public DateTime? FECHA_INGRESO { get; set; }

        public int? ID_UPDATE_USER { get; set; }

        public string UPDATE_USER { get; set; }

        public DateTime? FECHA_ACTUALIZACION { get; set; }

        public DateTime? F_NACIMIENTO { get; set; }

        public int? SEXO { get; set; }

        public string SEXO_ { get; set; }

        public string EDAD { get; set; }

        public string DCCIdReAgenda { get; set; }

        public string COMENTARIOS { get; set; }
    }

    public class DigitalContactCenterDtoNew
    {
        //public string v_DigitalContactCenterId { get; set; }
        public string v_ServiceId { get; set; }

    }
}
