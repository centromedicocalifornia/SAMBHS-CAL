//-------------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by EntitiesToDTOs.v3.2 (entitiestodtos.codeplex.com).
//     Timestamp: 2017/07/13 - 15:12:37
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
    /// Assembler for <see cref="linea"/> and <see cref="lineaDto"/>.
    /// </summary>
    public static partial class lineaAssembler
    {
        /// <summary>
        /// Invoked when <see cref="ToDTO"/> operation is about to return.
        /// </summary>
        /// <param name="dto"><see cref="lineaDto"/> converted from <see cref="linea"/>.</param>
        static partial void OnDTO(this linea entity, lineaDto dto);

        /// <summary>
        /// Invoked when <see cref="ToEntity"/> operation is about to return.
        /// </summary>
        /// <param name="entity"><see cref="linea"/> converted from <see cref="lineaDto"/>.</param>
        static partial void OnEntity(this lineaDto dto, linea entity);

        /// <summary>
        /// Converts this instance of <see cref="lineaDto"/> to an instance of <see cref="linea"/>.
        /// </summary>
        /// <param name="dto"><see cref="lineaDto"/> to convert.</param>
        public static linea ToEntity(this lineaDto dto)
        {
            if (dto == null) return null;

            var entity = new linea();

            entity.v_IdLinea = dto.v_IdLinea;
            entity.v_Periodo = dto.v_Periodo;
            entity.v_CodLinea = dto.v_CodLinea;
            entity.v_Nombre = dto.v_Nombre;
            entity.v_NroCuentaVenta = dto.v_NroCuentaVenta;
            entity.v_NroCuentaCompra = dto.v_NroCuentaCompra;
            entity.v_NroCuentaDConsumo = dto.v_NroCuentaDConsumo;
            entity.v_NroCuentaHConsumo = dto.v_NroCuentaHConsumo;
            entity.i_Eliminado = dto.i_Eliminado;
            entity.i_InsertaIdUsuario = dto.i_InsertaIdUsuario;
            entity.t_InsertaFecha = dto.t_InsertaFecha;
            entity.i_ActualizaIdUsuario = dto.i_ActualizaIdUsuario;
            entity.t_ActualizaFecha = dto.t_ActualizaFecha;
            entity.b_Foto = dto.b_Foto;
            entity.i_Header = dto.i_Header;

            dto.OnEntity(entity);

            return entity;
        }

        /// <summary>
        /// Converts this instance of <see cref="linea"/> to an instance of <see cref="lineaDto"/>.
        /// </summary>
        /// <param name="entity"><see cref="linea"/> to convert.</param>
        public static lineaDto ToDTO(this linea entity)
        {
            if (entity == null) return null;

            var dto = new lineaDto();

            dto.v_IdLinea = entity.v_IdLinea;
            dto.v_Periodo = entity.v_Periodo;
            dto.v_CodLinea = entity.v_CodLinea;
            dto.v_Nombre = entity.v_Nombre;
            dto.v_NroCuentaVenta = entity.v_NroCuentaVenta;
            dto.v_NroCuentaCompra = entity.v_NroCuentaCompra;
            dto.v_NroCuentaDConsumo = entity.v_NroCuentaDConsumo;
            dto.v_NroCuentaHConsumo = entity.v_NroCuentaHConsumo;
            dto.i_Eliminado = entity.i_Eliminado;
            dto.i_InsertaIdUsuario = entity.i_InsertaIdUsuario;
            dto.t_InsertaFecha = entity.t_InsertaFecha;
            dto.i_ActualizaIdUsuario = entity.i_ActualizaIdUsuario;
            dto.t_ActualizaFecha = entity.t_ActualizaFecha;
            dto.b_Foto = entity.b_Foto;
            dto.i_Header = entity.i_Header;

            entity.OnDTO(dto);

            return dto;
        }

        /// <summary>
        /// Converts each instance of <see cref="lineaDto"/> to an instance of <see cref="linea"/>.
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        public static List<linea> ToEntities(this IEnumerable<lineaDto> dtos)
        {
            if (dtos == null) return null;

            return dtos.Select(e => e.ToEntity()).ToList();
        }

        /// <summary>
        /// Converts each instance of <see cref="linea"/> to an instance of <see cref="lineaDto"/>.
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static List<lineaDto> ToDTOs(this IEnumerable<linea> entities)
        {
            if (entities == null) return null;

            return entities.Select(e => e.ToDTO()).ToList();
        }

    }
}