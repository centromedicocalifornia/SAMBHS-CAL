﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public partial class movimientodetalleDto : ICloneable
    {
        public string v_NombreProducto { get; set; }
        public string i_IdAlmacen { get; set; }
        public int EsServicio { get; set; }
        public string UnidadMIngreso { get; set; }
        public string UnidadMSalida { get; set; }
        public string UnidadMTransferenia { get; set; }
        public string UnidadMSaldo { get; set; }
        public decimal IngresosCantidadEmpaque { get; set; }
        public decimal SalidasCantidadEmpaque { get; set; }
        public string KeyRegistro { get; set; }
 
        #region kardex
        public string v_NombreClienteProveedor { get; set; }
        public decimal? Ingresos { get; set; }
        public decimal? Salidas { get; set; }
        public decimal? TipoMovimiento { get; set; }
        public decimal? Transferencias { get; set; }
        public string moneda { get; set; }
        public DateTime? Fecha { get; set; }
        public string TipoDocumento { get; set; }
        public int? EsDevolucion { get; set; }
        public string AlmacenDestino { get; set; }
        public string Origen { get; set; }
        public string ValorUm { get; set; }
        public string CodigoProducto { get; set; }
        public string DescripcionProducto { get; set; }
        public int i_TipoDocumento { get; set; }
        public decimal? IngresosTotal { get; set; }
        public decimal? SalidasTotal { get; set; }
        public DateTime t_Fecha { get; set; }
        public int TipoMotivo { get; set; }
        public string v_IdMovimientoOrigen { get; set; }
        #endregion

        public object Clone()
        {
            return MemberwiseClone();
        }
    }


    public partial class separacionproductoDto
    {

        public int i_IdAlmacen { get; set; }
        public string v_IdProductoDetalle { get; set; }

    }

    public partial class GridmovimientodetalleDto
    {
        public string v_IdProductoDetalle { get; set; }
        public string v_NroGuiaRemision { get; set; }
        public int? i_IdTipoDocumento { get; set; }
        public string v_NumeroDocumento { get; set; }
        public decimal? d_Cantidad { get; set; }
        public int? i_IdUnidad { get; set; }
        public decimal? d_Precio { get; set; }
        public decimal? d_Total { get; set; }
        public string v_NroPedido { get; set; }
        public decimal? d_CantidadEmpaque { get; set; }
        public string v_CodigoInterno { get; set; }
        public string v_NombreProducto { get; set; }
        public decimal? Empaque { get; set; }
        public string UMEmpaque { get; set; }
        public int? i_IdUnidadMedidaProducto { get; set; }
        public string v_IdMovimientoDetalle { get; set; }
        public string v_IdMovimientoDetalleTransferencia { get; set; }
        public string v_IdMovimiento { get; set; }
        public int? i_Eliminado { get; set; }
        public int? i_InsertaIdUsuario { get; set; }
        public DateTime t_InsertaFecha { get; set; }
        public decimal? StockActual { get; set; }
        public int? EsServicio { get; set; }
        public int? i_EsProductoFinal { get; set; }
        public string i_RegistroTipo { get; set; }
        public int i_ValidarStock { get; set; }
        public string i_IdCentroCosto { get; set; }
        public string v_NroSerie { get; set; }
        public string v_NroLote { get; set; }


        public int i_SolicitarNroSerie { get; set; }
        public int i_SolicitarNroLote { get; set; }
        public int i_SolicitaOrdenProduccion { get; set; }


        public int i_SolicitarNroLoteIngreso { get; set; }
        public int i_SolicitarNroSerieIngreso { get; set; }
        public int i_SolicitaOrdenProduccionIngreso { get; set; }
        public int i_SolicitarNroSerieSalida { get; set; }
        public int i_SolicitarNroLoteSalida { get; set; }
        public int i_SolicitaOrdenProduccionSalida { get; set; }



        public DateTime? t_FechaCaducidad { get; set; }
        public DateTime? t_FechaFabricacion { get; set; }
        public string v_NroOrdenProduccion { get; set; }

        public string v_TicketDetalleId { get; set; }
    }

    public class TicketDetalleDto
    {
        public string v_TicketDetalleId { get; set; }
        public string v_TicketId { get; set; }
        public string v_Descripcion { get; set; }
        public string v_CodInterno { get; set; }
        public string v_IdProductoDetalle { get; set; }
        public decimal d_Cantidad { get; set; }
        public int i_EsDespachado { get; set; }
    }
}
