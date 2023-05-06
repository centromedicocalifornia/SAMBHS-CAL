using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Windows.SigesoftIntegration.UI.Dtos
{
    public class ServiceDto
    {
        public string ServiceId { get; set; }
        public string OrganizationId { get; set; }
        public string ProtocolId { get; set; }
        public string PersonId { get; set; }
        public int MasterServiceId { get; set; }
        public int ServiceStatusId { get; set; }
        public int AptitudeStatusId { get; set; }
        public DateTime? ServiceDate { get; set; }
        public DateTime? GlobalExpirationDate { get; set; }
        public DateTime? ObsExpirationDate { get; set; }
        public int FlagAgentId { get; set; }
        public string Motive { get; set; }
        public int IsFac { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public int GeneroId { get; set; }
        public int MedicoTratanteId { get; set; }
        public int MedicoRealizaId { get; set; }
        public string v_centrocosto { get; set; }
        public string CommentaryUpdate { get; set; }
        public string Area { get; set; }
        public string CCosto { get; set; }
        public int  Plan { get; set; }
        public int ServiceTypeId { get; set; }
        public string v_LicenciaConducir { get; set; }
        public string ObservacionesAtencion { get; set; }
        public string PacienteHospSala { get; set; }
        public int i_ModTrabajo { get; set; }
        public int PasoSop { get; set; }
        public int PasoHosp { get; set; }
        public int i_ProcedenciaPac_Mkt { get; set; }

        public int Establecimiento { get; set; }
        public int VendedorExterno { get; set; }

        public int MedicoSolicitanteExterno { get; set; }

        public string _idccEditarNew { get; set; }

        public int i_MedicoAtencion { get; set; }
        public int i_CodigoAtencion { get; set; }
        public int i_GrupoAtencion { get; set; }

    }

    public class Liquidacion
    {
        public string v_PersonId { get; set; }
        public string v_ServiceId { get; set; }
        public int? i_EsoTypeId { get; set; }
        public string Esotype { get; set; }
        public string v_CustomerOrganizationId { get; set; }
        public string v_EmployerOrganizationId { get; set; }
        public string v_WorkingOrganizationId { get; set; }
        public string v_NroLiquidacion { get; set; }

        public string Trabajador { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public int? Edad { get; set; }
        public DateTime? FechaExamen { get; set; }
        public string NroDocumemto { get; set; }
        public string Cargo { get; set; }
        public string Perfil { get; set; }
        public decimal Precio { get; set; }
        public string CCosto { get; set; }
        public string v_ProtocolId { get; set; }

        public string v_CustomerLocationId { get; set; }
        public string v_EmployerLocationId { get; set; }
        public string v_WorkingLocationId { get; set; }
    }

    public class ServiceComponentDto
    {
        public string ServiceId { get; set; }
        public int ExternalInternalId { get; set; }
        public int ServiceComponentTypeId { get; set; }
        public int IsVisibleId { get; set; }
        public int IsInheritedId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Index { get; set; }
        public float Price { get; set; }
        public string ComponentId { get; set; }
        public string ComponentName { get; set; }
        public int IsInvoicedId { get; set; }
        public int ServiceComponentStatusId { get; set; }
        public int QueueStatusId { get; set; }
        public int IsRequiredId { get; set; }
        public string ProtocolId { get; set; }
        public int Iscalling { get; set; }
        public int Iscalling1 { get; set; }
        public string IdUnidadProductiva { get; set; }
        public int IsManuallyAddedId { get; set; }
        public int i_MedicoTratanteId { get; set; }
        public int i_MedicoRealizaId { get; set; }
         public DateTime? d_InsertDate { get; set; }
         public int? i_InsertUserId { get; set; }
         public int i_ConCargoA { get; set; }
         public string v_OrganizationId { get; set; }
         public decimal i_Cantidad { get; set; }
         public decimal d_Descuento { get; set; }

         public decimal  d_SaldoPaciente { get; set; }
         public decimal d_SaldoAseguradora { get; set; }
         public int i_PayMedic { get; set; }
    }


}
