//-------------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by EntitiesToDTOs.v3.2 (entitiestodtos.codeplex.com).
//     Timestamp: 2017/07/13 - 15:12:36
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
    /// Assembler for <see cref="modelo"/> and <see cref="modeloDto"/>.
    /// </summary>
    public static partial class modeloAssembler
    {
        /// <summary>
        /// Invoked when <see cref="ToDTO"/> operation is about to return.
        /// </summary>
        /// <param name="dto"><see cref="modeloDto"/> converted from <see cref="modelo"/>.</param>
        static partial void OnDTO(this modelo entity, modeloDto dto);

        /// <summary>
        /// Invoked when <see cref="ToEntity"/> operation is about to return.
        /// </summary>
        /// <param name="entity"><see cref="modelo"/> converted from <see cref="modeloDto"/>.</param>
        static partial void OnEntity(this modeloDto dto, modelo entity);

        /// <summary>
        /// Converts this instance of <see cref="modeloDto"/> to an instance of <see cref="modelo"/>.
        /// </summary>
        /// <param name="dto"><see cref="modeloDto"/> to convert.</param>
        public static modelo ToEntity(this modeloDto dto)
        {
            if (dto == null) return null;

            var entity = new modelo();

            entity.v_IdModelo = dto.v_IdModelo;
            entity.v_IdMarca = dto.v_IdMarca;
            entity.v_CodModelo = dto.v_CodModelo;
            entity.v_Nombre = dto.v_Nombre;
            entity.i_Eliminado = dto.i_Eliminado;
            entity.i_InsertaIdUsuario = dto.i_InsertaIdUsuario;
            entity.t_InsertaFecha = dto.t_InsertaFecha;
            entity.i_ActualizaIdUsuario = dto.i_ActualizaIdUsuario;
            entity.t_ActualizaFecha = dto.t_ActualizaFecha;

            dto.OnEntity(entity);

            return entity;
        }

        /// <summary>
        /// Converts this instance of <see cref="modelo"/> to an instance of <see cref="modeloDto"/>.
        /// </summary>
        /// <param name="entity"><see cref="modelo"/> to convert.</param>
        public static modeloDto ToDTO(this modelo entity)
        {
            if (entity == null) return null;

            var dto = new modeloDto();

            dto.v_IdModelo = entity.v_IdModelo;
            dto.v_IdMarca = entity.v_IdMarca;
            dto.v_CodModelo = entity.v_CodModelo;
            dto.v_Nombre = entity.v_Nombre;
            dto.i_Eliminado = entity.i_Eliminado;
            dto.i_InsertaIdUsuario = entity.i_InsertaIdUsuario;
            dto.t_InsertaFecha = entity.t_InsertaFecha;
            dto.i_ActualizaIdUsuario = entity.i_ActualizaIdUsuario;
            dto.t_ActualizaFecha = entity.t_ActualizaFecha;

            entity.OnDTO(dto);

            return dto;
        }

        /// <summary>
        /// Converts each instance of <see cref="modeloDto"/> to an instance of <see cref="modelo"/>.
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        public static List<modelo> ToEntities(this IEnumerable<modeloDto> dtos)
        {
            if (dtos == null) return null;

            return dtos.Select(e => e.ToEntity()).ToList();
        }

        /// <summary>
        /// Converts each instance of <see cref="modelo"/> to an instance of <see cref="modeloDto"/>.
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static List<modeloDto> ToDTOs(this IEnumerable<modelo> entities)
        {
            if (entities == null) return null;

            return entities.Select(e => e.ToDTO()).ToList();
        }

    }
}