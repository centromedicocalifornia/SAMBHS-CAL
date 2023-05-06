using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Windows.SigesoftIntegration.UI.Dtos
{
    public class Professional
    {
        public string Nombre { get; set; }
        public string Usuario { get; set; }
        public int IdUsuario { get; set; }
        public string PersonId { get; set; }
        public string ProfessionalCode { get; set; }
        public string ProfessionalInformation { get; set; }
        public int i_GrupoHorario { get; set; }
        public string v_GrupoHorario { get; set; }
        public int ProfessionalId { get; set; }
        public int i_CodigoProfesion { get; set; }
        public string v_CodigoProfesion { get; set; }
    }

    public class SystemParameter_Horario
    {
        public int i_GroupId { get; set; }
        public int i_ParameterId { get; set; }
        public string v_Value1 { get; set; }
        public string v_Value2 { get; set; }
        public string v_Field { get; set; }
        public int i_ParentParameterId { get; set; }
        public int i_IsDeleted { get; set; }
        public string v_ComentaryUpdate { get; set; }
    }
    public class HorarioClonar
    {
        public int i_ParameterId { get; set; }
        public string v_Value1 { get; set; }
        public int i_ParentParameterId { get; set; }
        public string v_Field { get; set; }
        public int i_IsDeleted { get; set; }
    }
    public class Horarios
    {
        public int IdHorario { get; set; }
        public int i_SystemUserId { get; set; }
        public DateTime d_FechaInicio { get; set; }
        public DateTime d_FechaFin { get; set; }
        public int i_01 { get; set; }
        public int i_02 { get; set; }
        public int i_03 { get; set; }
        public int i_04 { get; set; }
        public int i_05 { get; set; }
        public int i_06 { get; set; }
        public int i_07 { get; set; }
        public int i_08 { get; set; }
        public int i_09 { get; set; }
        public int i_10 { get; set; }
        public int i_11 { get; set; }
        public int i_12 { get; set; }
        public int i_13 { get; set; }
        public int i_14 { get; set; }
        public int i_15 { get; set; }
        public int i_16 { get; set; }
        public int i_17 { get; set; }
        public int i_18 { get; set; }
        public int i_19 { get; set; }
        public int i_20 { get; set; }
        public int i_21 { get; set; }
        public int i_22 { get; set; }
        public int i_23 { get; set; }
        public int i_24 { get; set; }
        public int i_25 { get; set; }
        public int i_26 { get; set; }
        public int i_27 { get; set; }
        public int i_28 { get; set; }
        public int i_29 { get; set; }
        public int i_30 { get; set; }
        public int i_31 { get; set; }
        public string v_Comentary { get; set; }
        public int i_IsDeleted { get; set; }
        public int i_InsertUserId { get; set; }
        public DateTime d_InsertDate { get; set; }
        public int i_UpdateUserId { get; set; }
        public DateTime d_UpdateDate { get; set; }
    }

    public class HorariosListados
    {
        public int IdHorario { get; set; }
        public int i_SystemUserId { get; set; }
        public string PROFESIONAL { get; set; }
        public DateTime d_FechaInicio { get; set; }
        public DateTime d_FechaFin { get; set; }
        public string i_01 { get; set; }
        public string i_02 { get; set; }
        public string i_03 { get; set; }
        public string i_04 { get; set; }
        public string i_05 { get; set; }
        public string i_06 { get; set; }
        public string i_07 { get; set; }
        public string i_08 { get; set; }
        public string i_09 { get; set; }
        public string i_10 { get; set; }
        public string i_11 { get; set; }
        public string i_12 { get; set; }
        public string i_13 { get; set; }
        public string i_14 { get; set; }
        public string i_15 { get; set; }
        public string i_16 { get; set; }
        public string i_17 { get; set; }
        public string i_18 { get; set; }
        public string i_19 { get; set; }
        public string i_20 { get; set; }
        public string i_21 { get; set; }
        public string i_22 { get; set; }
        public string i_23 { get; set; }
        public string i_24 { get; set; }
        public string i_25 { get; set; }
        public string i_26 { get; set; }
        public string i_27 { get; set; }
        public string i_28 { get; set; }
        public string i_29 { get; set; }
        public string i_30 { get; set; }
        public string i_31 { get; set; }
        public string v_Comentary { get; set; }

    }
    public class Dias
    {
        public string Dia { get; set; }
        public string ESTADO { get; set; }
        public int Grupo { get; set; }
        public string Orden { get; set; }
    }

    public class GrupoDia
    {
        public string Dia { get; set; }
        public int Grupo { get; set; }
    }
}
