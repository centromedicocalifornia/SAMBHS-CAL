//-------------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by EntitiesToDTOs.v3.2 (entitiestodtos.codeplex.com).
//     Timestamp: 2017/07/13 - 15:12:50
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
    /// Assembler for <see cref="planillaporcleyesempleador"/> and <see cref="planillaporcleyesempleadorDto"/>.
    /// </summary>
    public static partial class planillaporcleyesempleadorAssembler
    {
        /// <summary>
        /// Invoked when <see cref="ToDTO"/> operation is about to return.
        /// </summary>
        /// <param name="dto"><see cref="planillaporcleyesempleadorDto"/> converted from <see cref="planillaporcleyesempleador"/>.</param>
        static partial void OnDTO(this planillaporcleyesempleador entity, planillaporcleyesempleadorDto dto);

        /// <summary>
        /// Invoked when <see cref="ToEntity"/> operation is about to return.
        /// </summary>
        /// <param name="entity"><see cref="planillaporcleyesempleador"/> converted from <see cref="planillaporcleyesempleadorDto"/>.</param>
        static partial void OnEntity(this planillaporcleyesempleadorDto dto, planillaporcleyesempleador entity);

        /// <summary>
        /// Converts this instance of <see cref="planillaporcleyesempleadorDto"/> to an instance of <see cref="planillaporcleyesempleador"/>.
        /// </summary>
        /// <param name="dto"><see cref="planillaporcleyesempleadorDto"/> to convert.</param>
        public static planillaporcleyesempleador ToEntity(this planillaporcleyesempleadorDto dto)
        {
            if (dto == null) return null;

            var entity = new planillaporcleyesempleador();

            entity.i_Id = dto.i_Id;
            entity.v_Periodo = dto.v_Periodo;
            entity.v_Mes = dto.v_Mes;
            entity.d_EsSalud = dto.d_EsSalud;
            entity.d_Senati = dto.d_Senati;
            entity.d_SCTR = dto.d_SCTR;
            entity.d_SCTRPen = dto.d_SCTRPen;
            entity.i_Eliminado = dto.i_Eliminado;
            entity.i_InsertaIdUsuario = dto.i_InsertaIdUsuario;
            entity.t_InsertaFecha = dto.t_InsertaFecha;
            entity.i_ActualizaIdUsuario = dto.i_ActualizaIdUsuario;
            entity.t_ActualizaFecha = dto.t_ActualizaFecha;

            dto.OnEntity(entity);

            return entity;
        }

        /// <summary>
        /// Converts this instance of <see cref="planillaporcleyesempleador"/> to an instance of <see cref="planillaporcleyesempleadorDto"/>.
        /// </summary>
        /// <param name="entity"><see cref="planillaporcleyesempleador"/> to convert.</param>
        public static planillaporcleyesempleadorDto ToDTO(this planillaporcleyesempleador entity)
        {
            if (entity == null) return null;

            var dto = new planillaporcleyesempleadorDto();

            dto.i_Id = entity.i_Id;
            dto.v_Periodo = entity.v_Periodo;
            dto.v_Mes = entity.v_Mes;
            dto.d_EsSalud = entity.d_EsSalud;
            dto.d_Senati = entity.d_Senati;
            dto.d_SCTR = entity.d_SCTR;
            dto.d_SCTRPen = entity.d_SCTRPen;
            dto.i_Eliminado = entity.i_Eliminado;
            dto.i_InsertaIdUsuario = entity.i_InsertaIdUsuario;
            dto.t_InsertaFecha = entity.t_InsertaFecha;
            dto.i_ActualizaIdUsuario = entity.i_ActualizaIdUsuario;
            dto.t_ActualizaFecha = entity.t_ActualizaFecha;

            entity.OnDTO(dto);

            return dto;
        }

        /// <summary>
        /// Converts each instance of <see cref="planillaporcleyesempleadorDto"/> to an instance of <see cref="planillaporcleyesempleador"/>.
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        public static List<planillaporcleyesempleador> ToEntities(this IEnumerable<planillaporcleyesempleadorDto> dtos)
        {
            if (dtos == null) return null;

            return dtos.Select(e => e.ToEntity()).ToList();
        }

        /// <summary>
        /// Converts each instance of <see cref="planillaporcleyesempleador"/> to an instance of <see cref="planillaporcleyesempleadorDto"/>.
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static List<planillaporcleyesempleadorDto> ToDTOs(this IEnumerable<planillaporcleyesempleador> entities)
        {
            if (entities == null) return null;

            return entities.Select(e => e.ToDTO()).ToList();
        }

    }
}