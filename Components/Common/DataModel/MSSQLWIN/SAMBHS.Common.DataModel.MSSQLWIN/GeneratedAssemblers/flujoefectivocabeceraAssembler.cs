//-------------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by EntitiesToDTOs.v3.2 (entitiestodtos.codeplex.com).
//     Timestamp: 2017/07/13 - 15:13:22
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
    /// Assembler for <see cref="flujoefectivocabecera"/> and <see cref="flujoefectivocabeceraDto"/>.
    /// </summary>
    public static partial class flujoefectivocabeceraAssembler
    {
        /// <summary>
        /// Invoked when <see cref="ToDTO"/> operation is about to return.
        /// </summary>
        /// <param name="dto"><see cref="flujoefectivocabeceraDto"/> converted from <see cref="flujoefectivocabecera"/>.</param>
        static partial void OnDTO(this flujoefectivocabecera entity, flujoefectivocabeceraDto dto);

        /// <summary>
        /// Invoked when <see cref="ToEntity"/> operation is about to return.
        /// </summary>
        /// <param name="entity"><see cref="flujoefectivocabecera"/> converted from <see cref="flujoefectivocabeceraDto"/>.</param>
        static partial void OnEntity(this flujoefectivocabeceraDto dto, flujoefectivocabecera entity);

        /// <summary>
        /// Converts this instance of <see cref="flujoefectivocabeceraDto"/> to an instance of <see cref="flujoefectivocabecera"/>.
        /// </summary>
        /// <param name="dto"><see cref="flujoefectivocabeceraDto"/> to convert.</param>
        public static flujoefectivocabecera ToEntity(this flujoefectivocabeceraDto dto)
        {
            if (dto == null) return null;

            var entity = new flujoefectivocabecera();

            entity.i_IdFlujoEfectivoCabecera = dto.i_IdFlujoEfectivoCabecera;
            entity.v_PeriodoProceso = dto.v_PeriodoProceso;

            dto.OnEntity(entity);

            return entity;
        }

        /// <summary>
        /// Converts this instance of <see cref="flujoefectivocabecera"/> to an instance of <see cref="flujoefectivocabeceraDto"/>.
        /// </summary>
        /// <param name="entity"><see cref="flujoefectivocabecera"/> to convert.</param>
        public static flujoefectivocabeceraDto ToDTO(this flujoefectivocabecera entity)
        {
            if (entity == null) return null;

            var dto = new flujoefectivocabeceraDto();

            dto.i_IdFlujoEfectivoCabecera = entity.i_IdFlujoEfectivoCabecera;
            dto.v_PeriodoProceso = entity.v_PeriodoProceso;

            entity.OnDTO(dto);

            return dto;
        }

        /// <summary>
        /// Converts each instance of <see cref="flujoefectivocabeceraDto"/> to an instance of <see cref="flujoefectivocabecera"/>.
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        public static List<flujoefectivocabecera> ToEntities(this IEnumerable<flujoefectivocabeceraDto> dtos)
        {
            if (dtos == null) return null;

            return dtos.Select(e => e.ToEntity()).ToList();
        }

        /// <summary>
        /// Converts each instance of <see cref="flujoefectivocabecera"/> to an instance of <see cref="flujoefectivocabeceraDto"/>.
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static List<flujoefectivocabeceraDto> ToDTOs(this IEnumerable<flujoefectivocabecera> entities)
        {
            if (entities == null) return null;

            return entities.Select(e => e.ToDTO()).ToList();
        }

    }
}