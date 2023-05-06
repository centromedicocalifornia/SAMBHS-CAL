using SAMBHS.Common.DataModel;
using System.Linq;
using System.Collections.Generic;

namespace SAMBHS.Common.BE
{

    /// <summary>
    /// Assembler for <see cref="venta"/> and <see cref="ventaDto"/>.
    /// </summary>
    public static partial class ventaAssembler
    {
        /// <summary>
        /// Invoked when <see cref="ToDTO"/> operation is about to return.
        /// </summary>
        /// <param name="dto"><see cref="ventaDto"/> converted from <see cref="venta"/>.</param>
        static partial void OnDTO(this venta entity, ventaDto dto);

        /// <summary>
        /// Invoked when <see cref="ToEntity"/> operation is about to return.
        /// </summary>
        /// <param name="entity"><see cref="venta"/> converted from <see cref="ventaDto"/>.</param>
        static partial void OnEntity(this ventaDto dto, venta entity);

        /// <summary>
        /// Converts this instance of <see cref="ventaDto"/> to an instance of <see cref="venta"/>.
        /// </summary>
        /// <param name="dto"><see cref="ventaDto"/> to convert.</param>
        public static venta ToEntity(this ventaDto dto)
        {
            if (dto == null) return null;

            var entity = new venta();

            entity.v_IdVenta = dto.v_IdVenta;
            entity.v_IdFormatoUnicoFacturacion = dto.v_IdFormatoUnicoFacturacion;
            entity.v_Periodo = dto.v_Periodo;
            entity.v_Mes = dto.v_Mes;
            entity.v_Correlativo = dto.v_Correlativo;
            entity.i_IdIgv = dto.i_IdIgv;
            entity.i_IdTipoDocumento = dto.i_IdTipoDocumento;
            entity.v_SerieDocumento = dto.v_SerieDocumento;
            entity.v_CorrelativoDocumento = dto.v_CorrelativoDocumento;
            entity.v_CorrelativoDocumentoFin = dto.v_CorrelativoDocumentoFin;
            entity.i_IdTipoDocumentoRef = dto.i_IdTipoDocumentoRef;
            entity.v_SerieDocumentoRef = dto.v_SerieDocumentoRef;
            entity.v_CorrelativoDocumentoRef = dto.v_CorrelativoDocumentoRef;
            entity.t_FechaRef = dto.t_FechaRef;
            entity.v_IdCliente = dto.v_IdCliente;
            entity.v_NombreClienteTemporal = dto.v_NombreClienteTemporal;
            entity.v_DireccionClienteTemporal = dto.v_DireccionClienteTemporal;
            entity.t_FechaRegistro = dto.t_FechaRegistro;
            entity.d_TipoCambio = dto.d_TipoCambio;
            entity.i_NroDias = dto.i_NroDias;
            entity.t_FechaVencimiento = dto.t_FechaVencimiento;
            entity.i_IdCondicionPago = dto.i_IdCondicionPago;
            entity.v_Concepto = dto.v_Concepto;
            entity.i_IdMoneda = dto.i_IdMoneda;
            entity.i_IdEstado = dto.i_IdEstado;
            entity.i_EsAfectoIgv = dto.i_EsAfectoIgv;
            entity.i_PreciosIncluyenIgv = dto.i_PreciosIncluyenIgv;
            entity.v_IdVendedor = dto.v_IdVendedor;
            entity.v_IdVendedorRef = dto.v_IdVendedorRef;
            entity.d_PorcDescuento = dto.d_PorcDescuento;
            entity.d_PocComision = dto.d_PocComision;
            entity.d_ValorFOB = dto.d_ValorFOB;
            entity.i_DeduccionAnticipio = dto.i_DeduccionAnticipio;
            entity.v_NroPedido = dto.v_NroPedido;
            entity.v_NroGuiaRemisionSerie = dto.v_NroGuiaRemisionSerie;
            entity.v_NroGuiaRemisionCorrelativo = dto.v_NroGuiaRemisionCorrelativo;
            entity.v_OrdenCompra = dto.v_OrdenCompra;
            entity.t_FechaOrdenCompra = dto.t_FechaOrdenCompra;
            entity.i_IdTipoVenta = dto.i_IdTipoVenta;
            entity.i_IdTipoOperacion = dto.i_IdTipoOperacion;
            entity.i_IdEstablecimiento = dto.i_IdEstablecimiento;
            entity.i_IdPuntoEmbarque = dto.i_IdPuntoEmbarque;
            entity.i_IdPuntoDestino = dto.i_IdPuntoDestino;
            entity.i_IdTipoEmbarque = dto.i_IdTipoEmbarque;
            entity.i_IdMedioPagoVenta = dto.i_IdMedioPagoVenta;
            entity.v_Marca = dto.v_Marca;
            entity.i_DrawBack = dto.i_DrawBack;
            entity.v_NroBulto = dto.v_NroBulto;
            entity.v_BultoDimensiones = dto.v_BultoDimensiones;
            entity.d_PesoBrutoKG = dto.d_PesoBrutoKG;
            entity.d_PesoNetoKG = dto.d_PesoNetoKG;
            entity.d_Valor = dto.d_Valor;
            entity.d_ValorVenta = dto.d_ValorVenta;
            entity.d_Descuento = dto.d_Descuento;
            entity.d_Percepcion = dto.d_Percepcion;
            entity.d_Anticipio = dto.d_Anticipio;
            entity.d_IGV = dto.d_IGV;
            entity.d_Total = dto.d_Total;
            entity.d_total_isc = dto.d_total_isc;
            entity.d_total_otrostributos = dto.d_total_otrostributos;
            entity.v_PlacaVehiculo = dto.v_PlacaVehiculo;
            entity.v_IdTipoKardex = dto.v_IdTipoKardex;
            entity.i_Impresion = dto.i_Impresion;
            entity.i_Eliminado = dto.i_Eliminado;
            entity.i_InsertaIdUsuario = dto.i_InsertaIdUsuario;
            entity.t_InsertaFecha = dto.t_InsertaFecha;
            entity.i_ActualizaIdUsuario = dto.i_ActualizaIdUsuario;
            entity.t_ActualizaFecha = dto.t_ActualizaFecha;
            entity.i_EstadoSunat = dto.i_EstadoSunat;
            entity.i_IdTipoNota = dto.i_IdTipoNota;
            entity.i_EsGratuito = dto.i_EsGratuito;
            entity.i_IdTipoBulto = dto.i_IdTipoBulto;
            entity.i_IdDireccionCliente = dto.i_IdDireccionCliente;
            entity.d_SeguroTotal = dto.d_SeguroTotal;
            entity.d_FleteTotal = dto.d_FleteTotal;
            entity.d_CantidaTotal = dto.d_CantidaTotal;
            entity.v_NroBL = dto.v_NroBL;
            entity.t_FechaPagoBL = dto.t_FechaPagoBL;
            entity.v_Contenedor = dto.v_Contenedor;
            entity.v_Banco = dto.v_Banco;
            entity.v_Naviera = dto.v_Naviera;
            entity.i_AplicaPercepcion = dto.i_AplicaPercepcion;
            entity.i_ClienteEsAgente = dto.i_ClienteEsAgente;
            entity.d_PorcentajePercepcion = dto.d_PorcentajePercepcion;
            entity.i_ItemsAfectosPercepcion = dto.i_ItemsAfectosPercepcion;
            entity.v_MotivoEliminacion = dto.v_MotivoEliminacion;
            entity.v_NroBultoIngles = dto.v_NroBultoIngles;
            entity.i_AfectaDetraccion = dto.i_AfectaDetraccion;
            entity.d_TasaDetraccion = dto.d_TasaDetraccion;
            entity.i_IdCodigoDetraccion = dto.i_IdCodigoDetraccion;
            entity.i_IdTipoOperacionDetraccion = dto.i_IdTipoOperacionDetraccion;
            entity.i_Publish = dto.i_Publish;
            entity.v_IdDocAnticipo = dto.v_IdDocAnticipo;
            entity.i_EsAnticipo = dto.i_EsAnticipo;
            entity.v_SigesoftServiceId = dto.v_SigesoftServiceId;
            entity.i_FacturacionCliente = dto.i_FacturacionCliente;
            entity.v_SunatResponseCode = dto.v_SunatResponseCode;
            entity.v_CadenaCodigoQr = dto.v_CadenaCodigoQr;
            entity.v_Hash = dto.v_Hash;
            entity.v_CodigoBarras = dto.v_CodigoBarras;
            entity.v_KeySunat = dto.v_KeySunat;
            entity.SunatDescription = dto.SunatDescription;
            entity.SunatNote = dto.SunatNote;
            entity.v_EnlaceBaja = dto.v_EnlaceBaja;
            entity.v_EnlaceEnvio = dto.v_EnlaceEnvio;

            dto.OnEntity(entity);

            return entity;
        }

        /// <summary>
        /// Converts this instance of <see cref="venta"/> to an instance of <see cref="ventaDto"/>.
        /// </summary>
        /// <param name="entity"><see cref="venta"/> to convert.</param>
        public static ventaDto ToDTO(this venta entity)
        {
            if (entity == null) return null;

            var dto = new ventaDto();

            dto.v_IdVenta = entity.v_IdVenta;
            dto.v_IdFormatoUnicoFacturacion = entity.v_IdFormatoUnicoFacturacion;
            dto.v_Periodo = entity.v_Periodo;
            dto.v_Mes = entity.v_Mes;
            dto.v_Correlativo = entity.v_Correlativo;
            dto.i_IdIgv = entity.i_IdIgv;
            dto.i_IdTipoDocumento = entity.i_IdTipoDocumento;
            dto.v_SerieDocumento = entity.v_SerieDocumento;
            dto.v_CorrelativoDocumento = entity.v_CorrelativoDocumento;
            dto.v_CorrelativoDocumentoFin = entity.v_CorrelativoDocumentoFin;
            dto.i_IdTipoDocumentoRef = entity.i_IdTipoDocumentoRef;
            dto.v_SerieDocumentoRef = entity.v_SerieDocumentoRef;
            dto.v_CorrelativoDocumentoRef = entity.v_CorrelativoDocumentoRef;
            dto.t_FechaRef = entity.t_FechaRef;
            dto.v_IdCliente = entity.v_IdCliente;
            dto.v_NombreClienteTemporal = entity.v_NombreClienteTemporal;
            dto.v_DireccionClienteTemporal = entity.v_DireccionClienteTemporal;
            dto.t_FechaRegistro = entity.t_FechaRegistro;
            dto.d_TipoCambio = entity.d_TipoCambio;
            dto.i_NroDias = entity.i_NroDias;
            dto.t_FechaVencimiento = entity.t_FechaVencimiento;
            dto.i_IdCondicionPago = entity.i_IdCondicionPago;
            dto.v_Concepto = entity.v_Concepto;
            dto.i_IdMoneda = entity.i_IdMoneda;
            dto.i_IdEstado = entity.i_IdEstado;
            dto.i_EsAfectoIgv = entity.i_EsAfectoIgv;
            dto.i_PreciosIncluyenIgv = entity.i_PreciosIncluyenIgv;
            dto.v_IdVendedor = entity.v_IdVendedor;
            dto.v_IdVendedorRef = entity.v_IdVendedorRef;
            dto.d_PorcDescuento = entity.d_PorcDescuento;
            dto.d_PocComision = entity.d_PocComision;
            dto.d_ValorFOB = entity.d_ValorFOB;
            dto.i_DeduccionAnticipio = entity.i_DeduccionAnticipio;
            dto.v_NroPedido = entity.v_NroPedido;
            dto.v_NroGuiaRemisionSerie = entity.v_NroGuiaRemisionSerie;
            dto.v_NroGuiaRemisionCorrelativo = entity.v_NroGuiaRemisionCorrelativo;
            dto.v_OrdenCompra = entity.v_OrdenCompra;
            dto.t_FechaOrdenCompra = entity.t_FechaOrdenCompra;
            dto.i_IdTipoVenta = entity.i_IdTipoVenta;
            dto.i_IdTipoOperacion = entity.i_IdTipoOperacion;
            dto.i_IdEstablecimiento = entity.i_IdEstablecimiento;
            dto.i_IdPuntoEmbarque = entity.i_IdPuntoEmbarque;
            dto.i_IdPuntoDestino = entity.i_IdPuntoDestino;
            dto.i_IdTipoEmbarque = entity.i_IdTipoEmbarque;
            dto.i_IdMedioPagoVenta = entity.i_IdMedioPagoVenta;
            dto.v_Marca = entity.v_Marca;
            dto.i_DrawBack = entity.i_DrawBack;
            dto.v_NroBulto = entity.v_NroBulto;
            dto.v_BultoDimensiones = entity.v_BultoDimensiones;
            dto.d_PesoBrutoKG = entity.d_PesoBrutoKG;
            dto.d_PesoNetoKG = entity.d_PesoNetoKG;
            dto.d_Valor = entity.d_Valor;
            dto.d_ValorVenta = entity.d_ValorVenta;
            dto.d_Descuento = entity.d_Descuento;
            dto.d_Percepcion = entity.d_Percepcion;
            dto.d_Anticipio = entity.d_Anticipio;
            dto.d_IGV = entity.d_IGV;
            dto.d_Total = entity.d_Total;
            dto.d_total_isc = entity.d_total_isc;
            dto.d_total_otrostributos = entity.d_total_otrostributos;
            dto.v_PlacaVehiculo = entity.v_PlacaVehiculo;
            dto.v_IdTipoKardex = entity.v_IdTipoKardex;
            dto.i_Impresion = entity.i_Impresion;
            dto.i_Eliminado = entity.i_Eliminado;
            dto.i_InsertaIdUsuario = entity.i_InsertaIdUsuario;
            dto.t_InsertaFecha = entity.t_InsertaFecha;
            dto.i_ActualizaIdUsuario = entity.i_ActualizaIdUsuario;
            dto.t_ActualizaFecha = entity.t_ActualizaFecha;
            dto.i_EstadoSunat = entity.i_EstadoSunat;
            dto.i_IdTipoNota = entity.i_IdTipoNota;
            dto.i_EsGratuito = entity.i_EsGratuito;
            dto.i_IdTipoBulto = entity.i_IdTipoBulto;
            dto.i_IdDireccionCliente = entity.i_IdDireccionCliente;
            dto.d_SeguroTotal = entity.d_SeguroTotal;
            dto.d_FleteTotal = entity.d_FleteTotal;
            dto.d_CantidaTotal = entity.d_CantidaTotal;
            dto.v_NroBL = entity.v_NroBL;
            dto.t_FechaPagoBL = entity.t_FechaPagoBL;
            dto.v_Contenedor = entity.v_Contenedor;
            dto.v_Banco = entity.v_Banco;
            dto.v_Naviera = entity.v_Naviera;
            dto.i_AplicaPercepcion = entity.i_AplicaPercepcion;
            dto.i_ClienteEsAgente = entity.i_ClienteEsAgente;
            dto.d_PorcentajePercepcion = entity.d_PorcentajePercepcion;
            dto.i_ItemsAfectosPercepcion = entity.i_ItemsAfectosPercepcion;
            dto.v_MotivoEliminacion = entity.v_MotivoEliminacion;
            dto.v_NroBultoIngles = entity.v_NroBultoIngles;
            dto.i_AfectaDetraccion = entity.i_AfectaDetraccion;
            dto.d_TasaDetraccion = entity.d_TasaDetraccion;
            dto.i_IdCodigoDetraccion = entity.i_IdCodigoDetraccion;
            dto.i_IdTipoOperacionDetraccion = entity.i_IdTipoOperacionDetraccion;
            dto.i_Publish = entity.i_Publish;
            dto.v_IdDocAnticipo = entity.v_IdDocAnticipo;
            dto.i_EsAnticipo = entity.i_EsAnticipo;
            dto.v_SigesoftServiceId = entity.v_SigesoftServiceId;
            dto.i_FacturacionCliente = entity.i_FacturacionCliente ?? 0;
            dto.v_SunatResponseCode = entity.v_SunatResponseCode;
            dto.v_CadenaCodigoQr = entity.v_CadenaCodigoQr;
            dto.v_Hash = entity.v_Hash;
            dto.v_CodigoBarras = entity.v_CodigoBarras;
            dto.v_KeySunat = entity.v_KeySunat;
            dto.SunatNote = entity.SunatNote;
            dto.SunatDescription = entity.SunatDescription;
            dto.v_EnlaceEnvio = entity.v_EnlaceEnvio;
            dto.v_EnlaceBaja = entity.v_EnlaceBaja;
            entity.OnDTO(dto);

            return dto;
        }

        /// <summary>
        /// Converts each instance of <see cref="ventaDto"/> to an instance of <see cref="venta"/>.
        /// </summary>
        /// <param name="dtos"></param>
        /// <returns></returns>
        public static List<venta> ToEntities(this IEnumerable<ventaDto> dtos)
        {
            if (dtos == null) return null;

            return dtos.Select(e => e.ToEntity()).ToList();
        }

        /// <summary>
        /// Converts each instance of <see cref="venta"/> to an instance of <see cref="ventaDto"/>.
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static List<ventaDto> ToDTOs(this IEnumerable<venta> entities)
        {
            if (entities == null) return null;

            return entities.Select(e => e.ToDTO()).ToList();
        }

    }
}