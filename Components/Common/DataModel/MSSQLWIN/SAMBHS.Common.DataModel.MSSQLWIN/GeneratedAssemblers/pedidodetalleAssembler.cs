//-------------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by EntitiesToDTOs.v3.1 (entitiestodtos.codeplex.com).
//     Timestamp: 2017/09/27 - 17:30:00
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
    /// Assembler for <see cref="pedidodetalle"/> and <see cref="pedidodetalleDto"/>.
    /// </summary>
    public static partial class pedidodetalleAssembler
    {
        /// <summary>
        /// Invoked when <see cref="ToDTO"/> operation is about to return.
        /// </summary>
        /// <param name="dto"><see cref="pedidodetalleDto"/> converted from <see cref="pedidodetalle"/>.</param>
        static partial void OnDTO(this pedidodetalle entity, pedidodetalleDto dto);

        /// <summary>
        /// Invoked when <see cref="ToEntity"/> operation is about to return.
        /// </summary>
        /// <param name="entity"><see cref="pedidodetalle"/> converted from <see cref="pedidodetalleDto"/>.</param>
        static partial void OnEntity(this pedidodetalleDto dto, pedidodetalle entity);

        /// <summary>
        /// Converts this instance of <see cref="pedidodetalleDto"/> to an instance of <see cref="pedidodetalle"/>.
        /// </summary>
        /// <param name="dto"><see cref="pedidodetalleDto"/> to convert.</param>
        public static pedidodetalle ToEntity(this pedidodetalleDto dto)
        {
            if (dto == null) return null;

            var entity = new pedidodetalle();

            entity.v_IdPedidoDetalle = dto.v_IdPedidoDetalle;
            entity.v_IdPedido = dto.v_IdPedido;
            entity.i_IdAlmacen = dto.i_IdAlmacen;
            entity.v_IdProductoDetalle = dto.v_IdProductoDetalle;
            entity.v_NombreProducto = dto.v_NombreProducto;
            entity.d_Cantidad = dto.d_Cantidad;
            entity.d_CantidadEmpaque = dto.d_CantidadEmpaque;
            entity.i_IdUnidadMedida = dto.i_IdUnidadMedida;
            entity.d_PrecioUnitario = dto.d_PrecioUnitario;
            entity.d_Valor = dto.d_Valor;
            entity.d_Descuento = dto.d_Descuento;
            entity.d_ValorVenta = dto.d_ValorVenta;
            entity.d_Igv = dto.d_Igv;
            entity.d_PrecioVenta = dto.d_PrecioVenta;
            entity.i_NroUnidades = dto.i_NroUnidades;
            entity.v_Observacion = dto.v_Observacion;
            entity.i_Eliminado = dto.i_Eliminado;
            entity.i_InsertaIdUsuario = dto.i_InsertaIdUsuario;
            entity.t_InsertaFecha = dto.t_InsertaFecha;
            entity.i_ActualizaIdUsuario = dto.i_ActualizaIdUsuario;
            entity.t_ActualizaFecha = dto.t_ActualizaFecha;
            entity.v_Descuento = dto.v_Descuento;
            entity.i_IdTipoOperacion = dto.i_IdTipoOperacion;
            entity.t_FechaLiberacion = dto.t_FechaLiberacion;
            entity.i_LiberacionUsuario = dto.i_LiberacionUsuario;
            entity.v_NroSerie = dto.v_NroSerie;
            entity.v_NroLote = dto.v_NroLote;
            entity.t_FechaCaducidad = dto.t_FechaCaducidad;
            entity.v_NroPedido = dto.v_NroPedido;

            dto.OnEntity(entity);

            return entity;
        }

        /// <summary>
        /// Converts this instance of <see cref="pedidodetalle"/> to an instance of <see cref="pedidodetalleDto"/>.
        /// </summary>
        /// <param name="entity"><see cref="pedidodetalle"/> to convert.</param>
        public static pedidodetalleDto ToDTO(this pedidodetalle entity)
        {
            if (entity == null) return null;

            var dto = new pedidodetalleDto();

            dto.v_IdPedidoDetalle = entity.v_IdPedidoDetalle;
            dto.v_IdPedido = entity.v_IdPedido;
            dto.i_IdAlmacen = entity.i_IdAlmacen;
            dto.v_IdProductoDetalle = entity.v_IdProductoDetalle;
            dto.v_NombreProducto = entity.v_NombreProducto;
            dto.d_Cantidad = entity.d_Cantidad;
            dto.d_CantidadEmpaque = entity.d_CantidadEmpaque;
            dto.i_IdUnidadMedida = entity.i_IdUnidadMedida;
            dto.d_PrecioUnitario = entity.d_PrecioUnitario;
            dto.d_Valor = entity.d_Valor;
            dto.d_Descuento = entity.d_Descuento;
            dto.d_ValorVenta = entity.d_ValorVenta;
            dto.d_Igv = entity.d_Igv;
            dto.d_PrecioVenta = entity.d_PrecioVenta;
            dto.i_NroUnidades = entity.i_NroUnidades;
            dto.v_Observacion = entity.v_Observacion;
            dto.i_Eliminado = entity.i_Eliminado;
            dto.i_InsertaIdUsuario = entity.i_InsertaIdUsuario;
            dto.t_InsertaFecha = entity.t_InsertaFecha;
            dto.i_ActualizaIdUsuario = entity.i_ActualizaIdUsuario;
            dto.t_ActualizaFecha = entity.t_ActualizaFecha;
            dto.v_Descuento = entity.v_Descuento;
            dto.i_IdTipoOperacion = entity.i_IdTipoOperacion;
            dto.t_FechaLiberacion = entity.t_FechaLiberacion;
            dto.i_LiberacionUsuario = entity.i_LiberacionUsuario;
            dto.v_NroSerie = entity.v_NroSerie;
            dto.v_NroLote = entity.v_NroLote;
            dto.t_FechaCaducidad = entity.t_FechaCaducidad;
            dto.v_NroPedido = entity.v_NroPedido;

            entity.OnDTO(dto);

            return dto;
        }

        /// <summary>
        /// Converts each instance of <see cref="pedidodetalleDto"/> to an instance of <see cref="pedidodetalle"/>.
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        public static List<pedidodetalle> ToEntities(this IEnumerable<pedidodetalleDto> dtos)
        {
            if (dtos == null) return null;

            return dtos.Select(e => e.ToEntity()).ToList();
        }

        /// <summary>
        /// Converts each instance of <see cref="pedidodetalle"/> to an instance of <see cref="pedidodetalleDto"/>.
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static List<pedidodetalleDto> ToDTOs(this IEnumerable<pedidodetalle> entities)
        {
            if (entities == null) return null;

            return entities.Select(e => e.ToDTO()).ToList();
        }

    }
}