//-------------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by EntitiesToDTOs.v3.2 (entitiestodtos.codeplex.com).
//     Timestamp: 2017/07/13 - 15:14:02
//
//     Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//-------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Linq;
using SAMBHS.Common.DataModel;

namespace SAMBHS.Common.BE
{

    /// <summary>
    /// Assembler for <see cref="planillarelacioningresos"/> and <see cref="planillarelacioningresosDto"/>.
    /// </summary>
    public static partial class planillarelacioningresosAssembler
    {
        /// <summary>
        /// Invoked when <see cref="ToDTO"/> operation is about to return.
        /// </summary>
        /// <param name="dto"><see cref="planillarelacioningresosDto"/> converted from <see cref="planillarelacioningresos"/>.</param>
        static partial void OnDTO(this planillarelacioningresos entity, planillarelacioningresosDto dto);

        /// <summary>
        /// Invoked when <see cref="ToEntity"/> operation is about to return.
        /// </summary>
        /// <param name="entity"><see cref="planillarelacioningresos"/> converted from <see cref="planillarelacioningresosDto"/>.</param>
        static partial void OnEntity(this planillarelacioningresosDto dto, planillarelacioningresos entity);

        /// <summary>
        /// Converts this instance of <see cref="planillarelacioningresosDto"/> to an instance of <see cref="planillarelacioningresos"/>.
        /// </summary>
        /// <param name="dto"><see cref="planillarelacioningresosDto"/> to convert.</param>
        public static planillarelacioningresos ToEntity(this planillarelacioningresosDto dto)
        {
            if (dto == null) return null;

            var entity = new planillarelacioningresos();

            entity.i_Id = dto.i_Id;
            entity.v_Periodo = dto.v_Periodo;
            entity.i_IdTipoPlanilla = dto.i_IdTipoPlanilla;
            entity.v_IdConceptoPlanilla = dto.v_IdConceptoPlanilla;
            entity.i_IdCentroCosto = dto.i_IdCentroCosto;
            entity.v_NroCuenta = dto.v_NroCuenta;
            entity.i_Eliminado = dto.i_Eliminado;
            entity.i_InsertaIdUsuario = dto.i_InsertaIdUsuario;
            entity.t_InsertaFecha = dto.t_InsertaFecha;
            entity.i_ActualizaIdUsuario = dto.i_ActualizaIdUsuario;
            entity.t_ActualizaFecha = dto.t_ActualizaFecha;

            dto.OnEntity(entity);

            return entity;
        }

        /// <summary>
        /// Converts this instance of <see cref="planillarelacioningresos"/> to an instance of <see cref="planillarelacioningresosDto"/>.
        /// </summary>
        /// <param name="entity"><see cref="planillarelacioningresos"/> to convert.</param>
        public static planillarelacioningresosDto ToDTO(this planillarelacioningresos entity)
        {
            if (entity == null) return null;

            var dto = new planillarelacioningresosDto();

            dto.i_Id = entity.i_Id;
            dto.v_Periodo = entity.v_Periodo;
            dto.i_IdTipoPlanilla = entity.i_IdTipoPlanilla;
            dto.v_IdConceptoPlanilla = entity.v_IdConceptoPlanilla;
            dto.i_IdCentroCosto = entity.i_IdCentroCosto;
            dto.v_NroCuenta = entity.v_NroCuenta;
            dto.i_Eliminado = entity.i_Eliminado;
            dto.i_InsertaIdUsuario = entity.i_InsertaIdUsuario;
            dto.t_InsertaFecha = entity.t_InsertaFecha;
            dto.i_ActualizaIdUsuario = entity.i_ActualizaIdUsuario;
            dto.t_ActualizaFecha = entity.t_ActualizaFecha;

            entity.OnDTO(dto);

            return dto;
        }

        /// <summary>
        /// Converts each instance of <see cref="planillarelacioningresosDto"/> to an instance of <see cref="planillarelacioningresos"/>.
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        public static List<planillarelacioningresos> ToEntities(this IEnumerable<planillarelacioningresosDto> dtos)
        {
            if (dtos == null) return null;

            return dtos.Select(e => e.ToEntity()).ToList();
        }

        /// <summary>
        /// Converts each instance of <see cref="planillarelacioningresos"/> to an instance of <see cref="planillarelacioningresosDto"/>.
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static List<planillarelacioningresosDto> ToDTOs(this IEnumerable<planillarelacioningresos> entities)
        {
            if (entities == null) return null;

            return entities.Select(e => e.ToDTO()).ToList();
        }

    }
}