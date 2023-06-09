﻿using SAMBHS.Common.BE.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Windows.SigesoftIntegration.UI.Dtos
{
    public class ServiceCustom
    {
        public string v_ServiceId { get; set; }
        public string v_Paciente { get; set; }
        public string v_FirstName { get; set; }
        public string v_FirstLastName { get; set; }
        public string v_SecondLastName { get; set; }
        public string v_Empresa { get; set; }
        public string v_ProtocolId { get; set; }
        public DateTime d_FechaAtencion { get; set; }
        public string Prococolo { get; set; }
        public string Factor { get; set; }
        public string Descuento_PPS { get; set; }
        public string Documento { get; set; }
        public string Deducible { get; set; }
        public string Coaseguro { get; set; }
    }

    public class ListadoCovid
    {
        public string SERVICIO { get; set; }
        public string PACIENTE { get; set; }
        public string EXAMEN { get; set; }
        public decimal PRECIO { get; set; }
        public DateTime? FECHA { get; set; }
        public string COMPROBANTE { get; set; }
        public string PROTOCOLO { get; set; }
        public string RESULTADO { get; set; }
        public string ATENCION { get; set; }
    }
    public class DatosSeguro
    {
        public string Deducible { get; set; }
        public string Coaseguro { get; set; }

    }

    public class ServiceComponentList
    {
        public string v_PersonId { get; set; }
        public string v_CategoryName { get; set; }
        public string v_ServiceComponentId { get; set; }
        public string v_ServiceId { get; set; }
        public string v_ComponentId { get; set; }
        public string v_ComponentName { get; set; }
        public int? i_ServiceComponentStatusId { get; set; }
        public string v_ServiceComponentStatusName { get; set; }
        public int? i_ExternalInternalId { get; set; }
        public int? i_ServiceComponentTypeId { get; set; }
        public int? i_IsVisibleId { get; set; }
        public int? i_IsInheritedId { get; set; }
        public DateTime? d_StartDate { get; set; }
        public DateTime? d_EndDate { get; set; }
        public int? i_index { get; set; }
        public Single? r_Price { get; set; }
        public int? i_IsInvoicedId { get; set; }
        public int? i_IsRequiredId { get; set; }
        public int? i_IsManuallyAddedId { get; set; }
        public int? i_IsDeleted { get; set; }
        public string v_CreationUser { get; set; }
        public string v_UpdateUser { get; set; }
        public DateTime? d_CreationDate { get; set; }
        public DateTime? d_UpdateDate { get; set; }
        public string v_IdUnidadProductiva { get; set; }
        public int i_QueueStatusId { get; set; }
        public string v_QueueStatusName { get; set; }
        public int i_MedicoTratanteId { get; set; }
        public int i_MedicoRealizaId { get; set; }
        public string v_Comment { get; set; }
        public decimal i_Cantidad { get; set; }
        public decimal d_Descuento { get; set; }

        //public List<ServiceComponentFieldsList> ServiceComponentFields { get; set; }

        public int ServiceStatusId { get; set; }
        public string v_Motive { get; set; }
        public int? i_ControlId { get; set; }

        public int? i_CategoryId { get; set; }


        public int? i_IsApprovedId { get; set; }

        public List<DiagnosticRepositoryList> DiagnosticRepository { get; set; }

        //public List<RecomendationList> Recomendation { get; set; }

        public string v_InternalGroup { get; set; }
        public string v_InternalCode { get; set; }

        public int? i_StatusLiquidation { get; set; }

        public byte[] FirmaMedico { get; set; }

        public string v_ServiceComponentConcatId { get; set; }

        public string v_ComponentConcatId { get; set; }

        public string v_Paciente { get; set; }
        public DateTime? d_ServiceDate { get; set; }
        public List<ServiceComponentList> ComponenteDetalle { get; set; }

        public int Orden { get; set; }

        public string v_ComponentId_ { get; set; }


        public string v_NameOfice { get; set; }

        public DateTime? d_ApprovedInsertDate { get; set; }
        public DateTime? d_ApprovedUpdateDate { get; set; }
        public DateTime? d_CalledDate { get; set; }
        public DateTime? d_InsertDateMedicalAnalyst { get; set; }
        public DateTime? d_InsertDateTechnicalDataRegister { get; set; }
        public DateTime? d_UpdateDateMedicalAnalyst { get; set; }
        public DateTime? d_UpdateDateTechnicalDataRegister { get; set; }

        public DateTime? d_InsertDate { get; set; }

        public int? i_ApprovedInsertUserId { get; set; }
        public int? i_ApprovedUpdateUserId { get; set; }
        public string ApprovedUpdateUser { get; set; }
        public int? i_InsertUserMedicalAnalystId { get; set; }
        public int? i_InsertUserTechnicalDataRegisterId { get; set; }
        public int? i_Iscalling { get; set; }
        public int? i_Iscalling_1 { get; set; }
        public int? i_UpdateUserMedicalAnalystId { get; set; }
        public int? i_UpdateUserTechnicalDataRegisterId { get; set; }
        public string MedicoTratante { get; set; }

        public decimal? d_SaldoPaciente { get; set; }
        public decimal? d_SaldoAseguradora { get; set; }

        public int i_EsDeducible { get; set; }
        public int i_EsCoaseguro { get; set; }

        public decimal? d_Importe { get; set; }
        public int? i_ConCargoA { get; set; }
        public int? i_TipoDesc { get; set; }
        public int? i_InsertUserId { get; set; }
        public int i_PayMedic { get; set; }
    }
}
