//-------------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by EntitiesToDTOs.v3.2 (entitiestodtos.codeplex.com).
//     Timestamp: 2017/07/13 - 15:13:54
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
    /// Assembler for <see cref="pagodetalle"/> and <see cref="pagodetalleDto"/>.
    /// </summary>
    public static partial class pagodetalleAssembler
    {
        /// <summary>
        /// Invoked when <see cref="ToDTO"/> operation is about to return.
        /// </summary>
        /// <param name="dto"><see cref="pagodetalleDto"/> converted from <see cref="pagodetalle"/>.</param>
        static partial void OnDTO(this pagodetalle entity, pagodetalleDto dto);

        /// <summary>
        /// Invoked when <see cref="ToEntity"/> operation is about to return.
        /// </summary>
        /// <param name="entity"><see cref="pagodetalle"/> converted from <see cref="pagodetalleDto"/>.</param>
        static partial void OnEntity(this pagodetalleDto dto, pagodetalle entity);

        /// <summary>
        /// Converts this instance of <see cref="pagodetalleDto"/> to an instance of <see cref="pagodetalle"/>.
        /// </summary>
        /// <param name="dto"><see cref="pagodetalleDto"/> to convert.</param>
        public static pagodetalle ToEntity(this pagodetalleDto dto)
        {
            if (dto == null) return null;

            var entity = new pagodetalle();

            entity.v_IdPagoDetalle = dto.v_IdPagoDetalle;
            entity.v_IdPago = dto.v_IdPago;
            entity.v_IdCompra = dto.v_IdCompra;
            entity.i_IdFormaPago = dto.i_IdFormaPago;
            entity.i_IdTipoDocumentoRef = dto.i_IdTipoDocumentoRef;
            entity.v_DocumentoRef = dto.v_DocumentoRef;
            entity.i_IdMoneda = dto.i_IdMoneda;
            entity.d_NetoXCobrar = dto.d_NetoXCobrar;
            entity.d_ImporteSoles = dto.d_ImporteSoles;
            entity.d_ImporteDolares = dto.d_ImporteDolares;
            entity.v_Observacion = dto.v_Observacion;
            entity.i_EsLetra = dto.i_EsLetra;
            entity.i_Eliminado = dto.i_Eliminado;
            entity.i_InsertaIdUsuario = dto.i_InsertaIdUsuario;
            entity.t_InsertaFecha = dto.t_InsertaFecha;
            entity.i_ActualizaIdUsuario = dto.i_ActualizaIdUsuario;
            entity.t_ActualizaFecha = dto.t_ActualizaFecha;

            dto.OnEntity(entity);

            return entity;
        }

        /// <summary>
        /// Converts this instance of <see cref="pagodetalle"/> to an instance of <see cref="pagodetalleDto"/>.
        /// </summary>
        /// <param name="entity"><see cref="pagodetalle"/> to convert.</param>
        public static pagodetalleDto ToDTO(this pagodetalle entity)
        {
            if (entity == null) return null;

            var dto = new pagodetalleDto();

            dto.v_IdPagoDetalle = entity.v_IdPagoDetalle;
            dto.v_IdPago = entity.v_IdPago;
            dto.v_IdCompra = entity.v_IdCompra;
            dto.i_IdFormaPago = entity.i_IdFormaPago;
            dto.i_IdTipoDocumentoRef = entity.i_IdTipoDocumentoRef;
            dto.v_DocumentoRef = entity.v_DocumentoRef;
            dto.i_IdMoneda = entity.i_IdMoneda;
            dto.d_NetoXCobrar = entity.d_NetoXCobrar;
            dto.d_ImporteSoles = entity.d_ImporteSoles;
            dto.d_ImporteDolares = entity.d_ImporteDolares;
            dto.v_Observacion = entity.v_Observacion;
            dto.i_EsLetra = entity.i_EsLetra;
            dto.i_Eliminado = entity.i_Eliminado;
            dto.i_InsertaIdUsuario = entity.i_InsertaIdUsuario;
            dto.t_InsertaFecha = entity.t_InsertaFecha;
            dto.i_ActualizaIdUsuario = entity.i_ActualizaIdUsuario;
            dto.t_ActualizaFecha = entity.t_ActualizaFecha;

            entity.OnDTO(dto);

            return dto;
        }

        /// <summary>
        /// Converts each instance of <see cref="pagodetalleDto"/> to an instance of <see cref="pagodetalle"/>.
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        public static List<pagodetalle> ToEntities(this IEnumerable<pagodetalleDto> dtos)
        {
            if (dtos == null) return null;

            return dtos.Select(e => e.ToEntity()).ToList();
        }

        /// <summary>
        /// Converts each instance of <see cref="pagodetalle"/> to an instance of <see cref="pagodetalleDto"/>.
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static List<pagodetalleDto> ToDTOs(this IEnumerable<pagodetalle> entities)
        {
            if (entities == null) return null;

            return entities.Select(e => e.ToDTO()).ToList();
        }

    }
}