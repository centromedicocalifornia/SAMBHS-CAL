//-------------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by EntitiesToDTOs.v3.1 (entitiestodtos.codeplex.com).
//     Timestamp: 2015/11/24 - 15:06:57
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
    /// Assembler for <see cref="nodewarehouse"/> and <see cref="nodewarehouseDto"/>.
    /// </summary>
    public static partial class nodewarehouseAssembler
    {
        /// <summary>
        /// Invoked when <see cref="ToDTO"/> operation is about to return.
        /// </summary>
        /// <param name="dto"><see cref="nodewarehouseDto"/> converted from <see cref="nodewarehouse"/>.</param>
        static partial void OnDTO(this nodewarehouse entity, nodewarehouseDto dto);

        /// <summary>
        /// Invoked when <see cref="ToEntity"/> operation is about to return.
        /// </summary>
        /// <param name="entity"><see cref="nodewarehouse"/> converted from <see cref="nodewarehouseDto"/>.</param>
        static partial void OnEntity(this nodewarehouseDto dto, nodewarehouse entity);

        /// <summary>
        /// Converts this instance of <see cref="nodewarehouseDto"/> to an instance of <see cref="nodewarehouse"/>.
        /// </summary>
        /// <param name="dto"><see cref="nodewarehouseDto"/> to convert.</param>
        public static nodewarehouse ToEntity(this nodewarehouseDto dto)
        {
            if (dto == null) return null;

            var entity = new nodewarehouse();

            entity.i_NodeWarehouseId = dto.i_NodeWarehouseId;
            entity.i_NodeId = dto.i_NodeId;
            entity.i_WarehouseId = dto.i_WarehouseId;
            entity.i_IsDeleted = dto.i_IsDeleted;
            entity.i_InsertUserId = dto.i_InsertUserId;
            entity.d_InsertDate = dto.d_InsertDate;
            entity.i_UpdateUserId = dto.i_UpdateUserId;
            entity.d_UpdateDate = dto.d_UpdateDate;

            dto.OnEntity(entity);

            return entity;
        }

        /// <summary>
        /// Converts this instance of <see cref="nodewarehouse"/> to an instance of <see cref="nodewarehouseDto"/>.
        /// </summary>
        /// <param name="entity"><see cref="nodewarehouse"/> to convert.</param>
        public static nodewarehouseDto ToDTO(this nodewarehouse entity)
        {
            if (entity == null) return null;

            var dto = new nodewarehouseDto();

            dto.i_NodeWarehouseId = entity.i_NodeWarehouseId;
            dto.i_NodeId = entity.i_NodeId;
            dto.i_WarehouseId = entity.i_WarehouseId;
            dto.i_IsDeleted = entity.i_IsDeleted;
            dto.i_InsertUserId = entity.i_InsertUserId;
            dto.d_InsertDate = entity.d_InsertDate;
            dto.i_UpdateUserId = entity.i_UpdateUserId;
            dto.d_UpdateDate = entity.d_UpdateDate;

            entity.OnDTO(dto);

            return dto;
        }

        /// <summary>
        /// Converts each instance of <see cref="nodewarehouseDto"/> to an instance of <see cref="nodewarehouse"/>.
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        public static List<nodewarehouse> ToEntities(this IEnumerable<nodewarehouseDto> dtos)
        {
            if (dtos == null) return null;

            return dtos.Select(e => e.ToEntity()).ToList();
        }

        /// <summary>
        /// Converts each instance of <see cref="nodewarehouse"/> to an instance of <see cref="nodewarehouseDto"/>.
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static List<nodewarehouseDto> ToDTOs(this IEnumerable<nodewarehouse> entities)
        {
            if (entities == null) return null;

            return entities.Select(e => e.ToDTO()).ToList();
        }

    }
}