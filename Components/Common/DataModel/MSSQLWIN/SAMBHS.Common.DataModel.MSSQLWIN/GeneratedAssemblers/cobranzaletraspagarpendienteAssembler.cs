//-------------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by EntitiesToDTOs.v3.2 (entitiestodtos.codeplex.com).
//     Timestamp: 2017/07/13 - 15:13:12
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
    /// Assembler for <see cref="cobranzaletraspagarpendiente"/> and <see cref="cobranzaletraspagarpendienteDto"/>.
    /// </summary>
    public static partial class cobranzaletraspagarpendienteAssembler
    {
        /// <summary>
        /// Invoked when <see cref="ToDTO"/> operation is about to return.
        /// </summary>
        /// <param name="dto"><see cref="cobranzaletraspagarpendienteDto"/> converted from <see cref="cobranzaletraspagarpendiente"/>.</param>
        static partial void OnDTO(this cobranzaletraspagarpendiente entity, cobranzaletraspagarpendienteDto dto);

        /// <summary>
        /// Invoked when <see cref="ToEntity"/> operation is about to return.
        /// </summary>
        /// <param name="entity"><see cref="cobranzaletraspagarpendiente"/> converted from <see cref="cobranzaletraspagarpendienteDto"/>.</param>
        static partial void OnEntity(this cobranzaletraspagarpendienteDto dto, cobranzaletraspagarpendiente entity);

        /// <summary>
        /// Converts this instance of <see cref="cobranzaletraspagarpendienteDto"/> to an instance of <see cref="cobranzaletraspagarpendiente"/>.
        /// </summary>
        /// <param name="dto"><see cref="cobranzaletraspagarpendienteDto"/> to convert.</param>
        public static cobranzaletraspagarpendiente ToEntity(this cobranzaletraspagarpendienteDto dto)
        {
            if (dto == null) return null;

            var entity = new cobranzaletraspagarpendiente();

            entity.v_IdCobranzaLetrasPagarPendiente = dto.v_IdCobranzaLetrasPagarPendiente;
            entity.v_IdLetrasPagarDetalle = dto.v_IdLetrasPagarDetalle;
            entity.d_Acuenta = dto.d_Acuenta;
            entity.d_Saldo = dto.d_Saldo;
            entity.i_Eliminado = dto.i_Eliminado;
            entity.i_InsertaIdUsuario = dto.i_InsertaIdUsuario;
            entity.t_InsertaFecha = dto.t_InsertaFecha;
            entity.i_ActualizaIdUsuario = dto.i_ActualizaIdUsuario;
            entity.t_ActualizaFecha = dto.t_ActualizaFecha;

            dto.OnEntity(entity);

            return entity;
        }

        /// <summary>
        /// Converts this instance of <see cref="cobranzaletraspagarpendiente"/> to an instance of <see cref="cobranzaletraspagarpendienteDto"/>.
        /// </summary>
        /// <param name="entity"><see cref="cobranzaletraspagarpendiente"/> to convert.</param>
        public static cobranzaletraspagarpendienteDto ToDTO(this cobranzaletraspagarpendiente entity)
        {
            if (entity == null) return null;

            var dto = new cobranzaletraspagarpendienteDto();

            dto.v_IdCobranzaLetrasPagarPendiente = entity.v_IdCobranzaLetrasPagarPendiente;
            dto.v_IdLetrasPagarDetalle = entity.v_IdLetrasPagarDetalle;
            dto.d_Acuenta = entity.d_Acuenta;
            dto.d_Saldo = entity.d_Saldo;
            dto.i_Eliminado = entity.i_Eliminado;
            dto.i_InsertaIdUsuario = entity.i_InsertaIdUsuario;
            dto.t_InsertaFecha = entity.t_InsertaFecha;
            dto.i_ActualizaIdUsuario = entity.i_ActualizaIdUsuario;
            dto.t_ActualizaFecha = entity.t_ActualizaFecha;

            entity.OnDTO(dto);

            return dto;
        }

        /// <summary>
        /// Converts each instance of <see cref="cobranzaletraspagarpendienteDto"/> to an instance of <see cref="cobranzaletraspagarpendiente"/>.
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        public static List<cobranzaletraspagarpendiente> ToEntities(this IEnumerable<cobranzaletraspagarpendienteDto> dtos)
        {
            if (dtos == null) return null;

            return dtos.Select(e => e.ToEntity()).ToList();
        }

        /// <summary>
        /// Converts each instance of <see cref="cobranzaletraspagarpendiente"/> to an instance of <see cref="cobranzaletraspagarpendienteDto"/>.
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static List<cobranzaletraspagarpendienteDto> ToDTOs(this IEnumerable<cobranzaletraspagarpendiente> entities)
        {
            if (entities == null) return null;

            return entities.Select(e => e.ToDTO()).ToList();
        }

    }
}