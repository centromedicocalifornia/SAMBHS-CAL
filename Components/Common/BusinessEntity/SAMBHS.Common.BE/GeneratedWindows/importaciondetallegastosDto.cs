//-------------------------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by EntitiesToDTOs.v3.2 (entitiestodtos.codeplex.com).
//     Timestamp: 2017/07/13 - 15:09:48
//
//     Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//-------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SAMBHS.Common.BE
{
    [DataContract()]
    public partial class importaciondetallegastosDto
    {
        [DataMember()]
        public String v_IdImportacionDetalleGastos { get; set; }

        [DataMember()]
        public String v_IdImportacion { get; set; }

        [DataMember()]
        public String v_GastoImportacion { get; set; }

        [DataMember()]
        public String v_IdAsientoContable { get; set; }

        [DataMember()]
        public Nullable<Int32> i_IdTipoDocumento { get; set; }

        [DataMember()]
        public String v_SerieDocumento { get; set; }

        [DataMember()]
        public String v_CorrelativoDocumento { get; set; }

        [DataMember()]
        public Nullable<DateTime> t_FechaEmision { get; set; }

        [DataMember()]
        public String v_Detalle { get; set; }

        [DataMember()]
        public Nullable<Decimal> d_TipoCambio { get; set; }

        [DataMember()]
        public Nullable<Int32> i_IdMoneda { get; set; }

        [DataMember()]
        public Nullable<Decimal> d_ValorVenta { get; set; }

        [DataMember()]
        public Nullable<Decimal> d_NAfectoDetraccion { get; set; }

        [DataMember()]
        public Nullable<Decimal> d_Igv { get; set; }

        [DataMember()]
        public Nullable<Decimal> d_ImporteSoles { get; set; }

        [DataMember()]
        public Nullable<Decimal> d_ImporteDolares { get; set; }

        [DataMember()]
        public String i_CCosto { get; set; }

        [DataMember()]
        public String v_Glosa { get; set; }

        [DataMember()]
        public Nullable<Int32> i_IdTipoDocRef { get; set; }

        [DataMember()]
        public String v_SerieDocRef { get; set; }

        [DataMember()]
        public String v_CorrelativoDocRef { get; set; }

        [DataMember()]
        public Nullable<Int32> i_Eliminado { get; set; }

        [DataMember()]
        public Nullable<Int32> i_InsertaIdUsuario { get; set; }

        [DataMember()]
        public Nullable<DateTime> t_InsertaFecha { get; set; }

        [DataMember()]
        public Nullable<Int32> i_ActualizaIdUsuario { get; set; }

        [DataMember()]
        public Nullable<DateTime> t_ActualizaFecha { get; set; }

        [DataMember()]
        public Nullable<DateTime> t_FechaRegistro { get; set; }

        [DataMember()]
        public Nullable<Int32> i_EsDetraccion { get; set; }

        [DataMember()]
        public Nullable<Int32> i_CodigoDetraccion { get; set; }

        [DataMember()]
        public String v_NroDetraccion { get; set; }

        [DataMember()]
        public Nullable<Decimal> d_PorcentajeDetraccion { get; set; }

        [DataMember()]
        public Nullable<DateTime> t_FechaDetraccion { get; set; }

        [DataMember()]
        public Nullable<Decimal> d_ValorSolesDetraccion { get; set; }

        [DataMember()]
        public Nullable<Decimal> d_ValorDolaresDetraccion { get; set; }

        [DataMember()]
        public Nullable<Decimal> d_ValorSolesDetraccionNoAfecto { get; set; }

        [DataMember()]
        public Nullable<Decimal> d_ValorDolaresDetraccionNoAfecto { get; set; }

        [DataMember()]
        public importacionDto importacion { get; set; }

        public importaciondetallegastosDto()
        {
        }

        public importaciondetallegastosDto(String v_IdImportacionDetalleGastos, String v_IdImportacion, String v_GastoImportacion, String v_IdAsientoContable, Nullable<Int32> i_IdTipoDocumento, String v_SerieDocumento, String v_CorrelativoDocumento, Nullable<DateTime> t_FechaEmision, String v_Detalle, Nullable<Decimal> d_TipoCambio, Nullable<Int32> i_IdMoneda, Nullable<Decimal> d_ValorVenta, Nullable<Decimal> d_NAfectoDetraccion, Nullable<Decimal> d_Igv, Nullable<Decimal> d_ImporteSoles, Nullable<Decimal> d_ImporteDolares, String i_CCosto, String v_Glosa, Nullable<Int32> i_IdTipoDocRef, String v_SerieDocRef, String v_CorrelativoDocRef, Nullable<Int32> i_Eliminado, Nullable<Int32> i_InsertaIdUsuario, Nullable<DateTime> t_InsertaFecha, Nullable<Int32> i_ActualizaIdUsuario, Nullable<DateTime> t_ActualizaFecha, Nullable<DateTime> t_FechaRegistro, Nullable<Int32> i_EsDetraccion, Nullable<Int32> i_CodigoDetraccion, String v_NroDetraccion, Nullable<Decimal> d_PorcentajeDetraccion, Nullable<DateTime> t_FechaDetraccion, Nullable<Decimal> d_ValorSolesDetraccion, Nullable<Decimal> d_ValorDolaresDetraccion, Nullable<Decimal> d_ValorSolesDetraccionNoAfecto, Nullable<Decimal> d_ValorDolaresDetraccionNoAfecto, importacionDto importacion)
        {
			this.v_IdImportacionDetalleGastos = v_IdImportacionDetalleGastos;
			this.v_IdImportacion = v_IdImportacion;
			this.v_GastoImportacion = v_GastoImportacion;
			this.v_IdAsientoContable = v_IdAsientoContable;
			this.i_IdTipoDocumento = i_IdTipoDocumento;
			this.v_SerieDocumento = v_SerieDocumento;
			this.v_CorrelativoDocumento = v_CorrelativoDocumento;
			this.t_FechaEmision = t_FechaEmision;
			this.v_Detalle = v_Detalle;
			this.d_TipoCambio = d_TipoCambio;
			this.i_IdMoneda = i_IdMoneda;
			this.d_ValorVenta = d_ValorVenta;
			this.d_NAfectoDetraccion = d_NAfectoDetraccion;
			this.d_Igv = d_Igv;
			this.d_ImporteSoles = d_ImporteSoles;
			this.d_ImporteDolares = d_ImporteDolares;
			this.i_CCosto = i_CCosto;
			this.v_Glosa = v_Glosa;
			this.i_IdTipoDocRef = i_IdTipoDocRef;
			this.v_SerieDocRef = v_SerieDocRef;
			this.v_CorrelativoDocRef = v_CorrelativoDocRef;
			this.i_Eliminado = i_Eliminado;
			this.i_InsertaIdUsuario = i_InsertaIdUsuario;
			this.t_InsertaFecha = t_InsertaFecha;
			this.i_ActualizaIdUsuario = i_ActualizaIdUsuario;
			this.t_ActualizaFecha = t_ActualizaFecha;
			this.t_FechaRegistro = t_FechaRegistro;
			this.i_EsDetraccion = i_EsDetraccion;
			this.i_CodigoDetraccion = i_CodigoDetraccion;
			this.v_NroDetraccion = v_NroDetraccion;
			this.d_PorcentajeDetraccion = d_PorcentajeDetraccion;
			this.t_FechaDetraccion = t_FechaDetraccion;
			this.d_ValorSolesDetraccion = d_ValorSolesDetraccion;
			this.d_ValorDolaresDetraccion = d_ValorDolaresDetraccion;
			this.d_ValorSolesDetraccionNoAfecto = d_ValorSolesDetraccionNoAfecto;
			this.d_ValorDolaresDetraccionNoAfecto = d_ValorDolaresDetraccionNoAfecto;
			this.importacion = importacion;
        }
    }
}