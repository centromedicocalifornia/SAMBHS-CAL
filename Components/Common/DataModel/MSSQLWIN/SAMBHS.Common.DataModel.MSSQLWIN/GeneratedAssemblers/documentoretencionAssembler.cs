//-------------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by EntitiesToDTOs.v3.2 (entitiestodtos.codeplex.com).
//     Timestamp: 2017/07/13 - 15:13:17
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
    /// Assembler for <see cref="documentoretencion"/> and <see cref="documentoretencionDto"/>.
    /// </summary>
    public static partial class documentoretencionAssembler
    {
        /// <summary>
        /// Invoked when <see cref="ToDTO"/> operation is about to return.
        /// </summary>
        /// <param name="dto"><see cref="documentoretencionDto"/> converted from <see cref="documentoretencion"/>.</param>
        static partial void OnDTO(this documentoretencion entity, documentoretencionDto dto);

        /// <summary>
        /// Invoked when <see cref="ToEntity"/> operation is about to return.
        /// </summary>
        /// <param name="entity"><see cref="documentoretencion"/> converted from <see cref="documentoretencionDto"/>.</param>
        static partial void OnEntity(this documentoretencionDto dto, documentoretencion entity);

        /// <summary>
        /// Converts this instance of <see cref="documentoretencionDto"/> to an instance of <see cref="documentoretencion"/>.
        /// </summary>
        /// <param name="dto"><see cref="documentoretencionDto"/> to convert.</param>
        public static documentoretencion ToEntity(this documentoretencionDto dto)
        {
            if (dto == null) return null;

            var entity = new documentoretencion();

            entity.v_IdDocumentoRetencion = dto.v_IdDocumentoRetencion;
            entity.v_IdTesoreria = dto.v_IdTesoreria;
            entity.v_IdCliente = dto.v_IdCliente;
            entity.t_FechaRegistro = dto.t_FechaRegistro;
            entity.v_Periodo = dto.v_Periodo;
            entity.v_Mes = dto.v_Mes;
            entity.v_Correlativo = dto.v_Correlativo;
            entity.v_SerieDocumento = dto.v_SerieDocumento;
            entity.v_CorrelativoDocumento = dto.v_CorrelativoDocumento;
            entity.d_TotalRetenido = dto.d_TotalRetenido;
            entity.i_Estado = dto.i_Estado;
            entity.i_InsertaIdUsuario = dto.i_InsertaIdUsuario;
            entity.t_InsertaFecha = dto.t_InsertaFecha;
            entity.i_EstadoSunat = dto.i_EstadoSunat;
            entity.i_Eliminado = dto.i_Eliminado;
            entity.i_ActualizaUsuario = dto.i_ActualizaUsuario;
            entity.t_ActualizaFecha = dto.t_ActualizaFecha;
            entity.i_IdMoneda = dto.i_IdMoneda;
            entity.d_TipoCambio = dto.d_TipoCambio;
            entity.v_MotivoEliminacion = dto.v_MotivoEliminacion;

            dto.OnEntity(entity);

            return entity;
        }

        /// <summary>
        /// Converts this instance of <see cref="documentoretencion"/> to an instance of <see cref="documentoretencionDto"/>.
        /// </summary>
        /// <param name="entity"><see cref="documentoretencion"/> to convert.</param>
        public static documentoretencionDto ToDTO(this documentoretencion entity)
        {
            if (entity == null) return null;

            var dto = new documentoretencionDto();

            dto.v_IdDocumentoRetencion = entity.v_IdDocumentoRetencion;
            dto.v_IdTesoreria = entity.v_IdTesoreria;
            dto.v_IdCliente = entity.v_IdCliente;
            dto.t_FechaRegistro = entity.t_FechaRegistro;
            dto.v_Periodo = entity.v_Periodo;
            dto.v_Mes = entity.v_Mes;
            dto.v_Correlativo = entity.v_Correlativo;
            dto.v_SerieDocumento = entity.v_SerieDocumento;
            dto.v_CorrelativoDocumento = entity.v_CorrelativoDocumento;
            dto.d_TotalRetenido = entity.d_TotalRetenido;
            dto.i_Estado = entity.i_Estado;
            dto.i_InsertaIdUsuario = entity.i_InsertaIdUsuario;
            dto.t_InsertaFecha = entity.t_InsertaFecha;
            dto.i_EstadoSunat = entity.i_EstadoSunat;
            dto.i_Eliminado = entity.i_Eliminado;
            dto.i_ActualizaUsuario = entity.i_ActualizaUsuario;
            dto.t_ActualizaFecha = entity.t_ActualizaFecha;
            dto.i_IdMoneda = entity.i_IdMoneda;
            dto.d_TipoCambio = entity.d_TipoCambio;
            dto.v_MotivoEliminacion = entity.v_MotivoEliminacion;

            entity.OnDTO(dto);

            return dto;
        }

        /// <summary>
        /// Converts each instance of <see cref="documentoretencionDto"/> to an instance of <see cref="documentoretencion"/>.
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        public static List<documentoretencion> ToEntities(this IEnumerable<documentoretencionDto> dtos)
        {
            if (dtos == null) return null;

            return dtos.Select(e => e.ToEntity()).ToList();
        }

        /// <summary>
        /// Converts each instance of <see cref="documentoretencion"/> to an instance of <see cref="documentoretencionDto"/>.
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static List<documentoretencionDto> ToDTOs(this IEnumerable<documentoretencion> entities)
        {
            if (entities == null) return null;

            return entities.Select(e => e.ToDTO()).ToList();
        }

    }
}