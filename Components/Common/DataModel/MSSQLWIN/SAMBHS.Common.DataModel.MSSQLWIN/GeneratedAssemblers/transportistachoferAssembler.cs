using SAMBHS.Common.DataModel;
using System.Linq;
//-------------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by EntitiesToDTOs.v3.3.0.0 (entitiestodtos.codeplex.com).
//     Timestamp: 2017/07/19 - 12:50:57
//
//     Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//-------------------------------------------------------------------------------------------------------
using System.Text;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System;

namespace SAMBHS.Common.BE
{

    /// <summary>
    /// Assembler for <see cref="transportistachofer"/> and <see cref="transportistachoferDto"/>.
    /// </summary>
    public static partial class transportistachoferAssembler
    {
        /// <summary>
        /// Invoked when <see cref="ToDTO"/> operation is about to return.
        /// </summary>
        /// <param name="dto"><see cref="transportistachoferDto"/> converted from <see cref="transportistachofer"/>.</param>
        static partial void OnDTO(this transportistachofer entity, transportistachoferDto dto);

        /// <summary>
        /// Invoked when <see cref="ToEntity"/> operation is about to return.
        /// </summary>
        /// <param name="entity"><see cref="transportistachofer"/> converted from <see cref="transportistachoferDto"/>.</param>
        static partial void OnEntity(this transportistachoferDto dto, transportistachofer entity);

        /// <summary>
        /// Converts this instance of <see cref="transportistachoferDto"/> to an instance of <see cref="transportistachofer"/>.
        /// </summary>
        /// <param name="dto"><see cref="transportistachoferDto"/> to convert.</param>
        public static transportistachofer ToEntity(this transportistachoferDto dto)
        {
            if (dto == null) return null;

            var entity = new transportistachofer();

            entity.v_IdChofer = dto.v_IdChofer;
            entity.v_IdTransportista = dto.v_IdTransportista;
            entity.v_NombreCompleto = dto.v_NombreCompleto;
            entity.v_Brevete = dto.v_Brevete;
            entity.i_Eliminado = dto.i_Eliminado;
            entity.i_InsertaIdUsuario = dto.i_InsertaIdUsuario;
            entity.t_InsertaFecha = dto.t_InsertaFecha;
            entity.i_ActualizaIdUsuario = dto.i_ActualizaIdUsuario;
            entity.t_ActualizaFecha = dto.t_ActualizaFecha;
            entity.i_IdTipoIdentificacion = dto.i_IdTipoIdentificacion;
            entity.v_NroDocIdentificacion = dto.v_NroDocIdentificacion;

            dto.OnEntity(entity);

            return entity;
        }

        /// <summary>
        /// Converts this instance of <see cref="transportistachofer"/> to an instance of <see cref="transportistachoferDto"/>.
        /// </summary>
        /// <param name="entity"><see cref="transportistachofer"/> to convert.</param>
        public static transportistachoferDto ToDTO(this transportistachofer entity)
        {
            if (entity == null) return null;

            var dto = new transportistachoferDto();

            dto.v_IdChofer = entity.v_IdChofer;
            dto.v_IdTransportista = entity.v_IdTransportista;
            dto.v_NombreCompleto = entity.v_NombreCompleto;
            dto.v_Brevete = entity.v_Brevete;
            dto.i_Eliminado = entity.i_Eliminado;
            dto.i_InsertaIdUsuario = entity.i_InsertaIdUsuario;
            dto.t_InsertaFecha = entity.t_InsertaFecha;
            dto.i_ActualizaIdUsuario = entity.i_ActualizaIdUsuario;
            dto.t_ActualizaFecha = entity.t_ActualizaFecha;
            dto.i_IdTipoIdentificacion = entity.i_IdTipoIdentificacion;
            dto.v_NroDocIdentificacion = entity.v_NroDocIdentificacion;

            entity.OnDTO(dto);

            return dto;
        }

        /// <summary>
        /// Converts each instance of <see cref="transportistachoferDto"/> to an instance of <see cref="transportistachofer"/>.
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        public static List<transportistachofer> ToEntities(this IEnumerable<transportistachoferDto> dtos)
        {
            if (dtos == null) return null;

            return dtos.Select(e => e.ToEntity()).ToList();
        }

        /// <summary>
        /// Converts each instance of <see cref="transportistachofer"/> to an instance of <see cref="transportistachoferDto"/>.
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static List<transportistachoferDto> ToDTOs(this IEnumerable<transportistachofer> entities)
        {
            if (entities == null) return null;

            return entities.Select(e => e.ToDTO()).ToList();
        }

    }
}