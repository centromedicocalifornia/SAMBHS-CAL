//-------------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by EntitiesToDTOs.v3.2 (entitiestodtos.codeplex.com).
//     Timestamp: 2017/07/13 - 15:13:18
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
    /// Assembler for <see cref="contratodetalletrabajador"/> and <see cref="contratodetalletrabajadorDto"/>.
    /// </summary>
    public static partial class contratodetalletrabajadorAssembler
    {
        /// <summary>
        /// Invoked when <see cref="ToDTO"/> operation is about to return.
        /// </summary>
        /// <param name="dto"><see cref="contratodetalletrabajadorDto"/> converted from <see cref="contratodetalletrabajador"/>.</param>
        static partial void OnDTO(this contratodetalletrabajador entity, contratodetalletrabajadorDto dto);

        /// <summary>
        /// Invoked when <see cref="ToEntity"/> operation is about to return.
        /// </summary>
        /// <param name="entity"><see cref="contratodetalletrabajador"/> converted from <see cref="contratodetalletrabajadorDto"/>.</param>
        static partial void OnEntity(this contratodetalletrabajadorDto dto, contratodetalletrabajador entity);

        /// <summary>
        /// Converts this instance of <see cref="contratodetalletrabajadorDto"/> to an instance of <see cref="contratodetalletrabajador"/>.
        /// </summary>
        /// <param name="dto"><see cref="contratodetalletrabajadorDto"/> to convert.</param>
        public static contratodetalletrabajador ToEntity(this contratodetalletrabajadorDto dto)
        {
            if (dto == null) return null;

            var entity = new contratodetalletrabajador();

            entity.v_IdContratoDetalle = dto.v_IdContratoDetalle;
            entity.v_IdContrato = dto.v_IdContrato;
            entity.v_IdConcepto = dto.v_IdConcepto;
            entity.i_Fijo = dto.i_Fijo;
            entity.d_ImporteMensual = dto.d_ImporteMensual;
            entity.d_SubTotal = dto.d_SubTotal;
            entity.i_Eliminado = dto.i_Eliminado;
            entity.i_InsertaIdUsuario = dto.i_InsertaIdUsuario;
            entity.t_InsertaFecha = dto.t_InsertaFecha;
            entity.i_ActualizaIdUsuario = dto.i_ActualizaIdUsuario;
            entity.t_ActualizaFecha = dto.t_ActualizaFecha;

            dto.OnEntity(entity);

            return entity;
        }

        /// <summary>
        /// Converts this instance of <see cref="contratodetalletrabajador"/> to an instance of <see cref="contratodetalletrabajadorDto"/>.
        /// </summary>
        /// <param name="entity"><see cref="contratodetalletrabajador"/> to convert.</param>
        public static contratodetalletrabajadorDto ToDTO(this contratodetalletrabajador entity)
        {
            if (entity == null) return null;

            var dto = new contratodetalletrabajadorDto();

            dto.v_IdContratoDetalle = entity.v_IdContratoDetalle;
            dto.v_IdContrato = entity.v_IdContrato;
            dto.v_IdConcepto = entity.v_IdConcepto;
            dto.i_Fijo = entity.i_Fijo;
            dto.d_ImporteMensual = entity.d_ImporteMensual;
            dto.d_SubTotal = entity.d_SubTotal;
            dto.i_Eliminado = entity.i_Eliminado;
            dto.i_InsertaIdUsuario = entity.i_InsertaIdUsuario;
            dto.t_InsertaFecha = entity.t_InsertaFecha;
            dto.i_ActualizaIdUsuario = entity.i_ActualizaIdUsuario;
            dto.t_ActualizaFecha = entity.t_ActualizaFecha;

            entity.OnDTO(dto);

            return dto;
        }

        /// <summary>
        /// Converts each instance of <see cref="contratodetalletrabajadorDto"/> to an instance of <see cref="contratodetalletrabajador"/>.
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        public static List<contratodetalletrabajador> ToEntities(this IEnumerable<contratodetalletrabajadorDto> dtos)
        {
            if (dtos == null) return null;

            return dtos.Select(e => e.ToEntity()).ToList();
        }

        /// <summary>
        /// Converts each instance of <see cref="contratodetalletrabajador"/> to an instance of <see cref="contratodetalletrabajadorDto"/>.
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static List<contratodetalletrabajadorDto> ToDTOs(this IEnumerable<contratodetalletrabajador> entities)
        {
            if (entities == null) return null;

            return entities.Select(e => e.ToDTO()).ToList();
        }

    }
}