using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAMBHS.Common.BE
{
    public class KardexList :ICloneable
    {
        public string v_IdMovimientoDetalle { get; set; }
        public string v_IdProducto { get; set; }
        public string v_NombreProducto { get; set; }
        public string v_NombreProductoAuxiliar { get; set; }
        public string v_NombreProductoInventarioSunat { get; set; }
        public string TipoOperacionInventarioSunat { get; set; }
        public DateTime? t_Fecha { get; set; }
        public string v_Fecha { get; set; }
        public int? i_IdTipoMovimiento { get; set; }
        public string v_NombreTipoMovimiento { get; set; }
        public decimal? d_Cantidad { get; set; }
        public decimal? d_Precio { get; set; }
        public decimal? d_Total { get; set; }
        public int? i_EsDevolucion { get; set; }
        public int? i_IdTipoMotivo { get; set; }
        public decimal? Ingreso_Cantidad { get; set; }
        public decimal? IngresoCantidadCalculo { get; set; }
       
        public decimal? IngresoTotalCalculo { get; set; }
        public decimal? SalidaTotalCalculo { get; set; }
        public decimal? Ingreso_CantidadInicial { get; set; }
        public decimal? Ingreso_Precio { get; set; }
        public decimal? Ingreso_Total { get; set; }
        public decimal? Ingreso_PrecioInicial { get; set; }
        public decimal? Ingreso_TotalInicial { get; set; }

        public decimal? Salida_Cantidad { get; set; }
        public decimal? Salida_CantidadCalculos { get; set; }
        public decimal? Salida_Precio { get; set; }
        public decimal? Salida_Total { get; set; }

        public decimal? Saldo_Cantidad { get; set; }
        public decimal? Saldo_Precio { get; set; }
        public decimal? Saldo_Total { get; set; }

        public decimal? Saldo_CantidadExcel { get; set; }
        public decimal? Saldo_PrecioExcel { get; set; }
        public decimal? Saldo_TotalExcel { get; set; }


        public string Ruc { get; set; }
        public int IdMoneda { get; set; }
        public string Moneda { get; set; }
        public string Almacen { get; set; }
        public string Mes { get; set; }
        public string Al { get; set; }
        public string Empresa { get; set; }
        public string Guia { get; set; }
        public string Documento { get; set; }
        public string DocumentoInventarioSunat { get; set; }
        public string NroPedido { get; set; }
        public string v_IdLinea { get; set; }
        public decimal TipoCambio { get; set; }
        public string ClienteProveedor { get; set; }
        public string CodProducto { get; set; }
        public string UnidadMedidaCompleto { get; set; }
        public string UnidadMedida { get; set; }
        public string UnidadMedidaCodigoSunat { get; set; }
        public int IdAlmacen { get; set; }
        public int i_IdUnidad { get; set; }
        public decimal? d_Empaque { get; set; }
        public int TipoMotivo { get; set; }
        public int IdAlmacenOrigen { get; set; }
        public string NroRegistro { get; set; }
        public string TipoExistencia { get; set; }
        public string NombreLinea { get; set; }
        public string GrupoLlave { get; set; }
        public int i_IdTipoProducto { get; set; }
        public int NumeroElemento { get; set; }
        public int i_IdTipoDocumentoDetalle { get; set; }
        public int EsDocumentoInterno { get; set; }
        public int EsDocInverso { get; set; }
        public string ValorUM { get; set; }
        public string v_IdMarca { get; set; }
        public string GuiasNoseTomaronCuenta { get; set; }
        public string Origen { get; set; }
        public decimal StockFisico { get; set; }
       public string CorrelativoPle { get; set; }
        public string SoloNumeroDocumentoDetalle { get; set; }
        public string DescripcionProducto { get; set; }
        public string TipoeExistenciaCompleto { get; set; }
        public decimal StockMinimo { get; set; }
        public string UnidadMedidaProducto { get; set; }
        public string v_IdProductoDetalle { get; set; }
        public int i_IdUnidadMedidaProducto { get; set; }
        public int i_Activo { get; set; }
        public decimal UltimoRegistro { get; set; }
        public string Modelo { get; set; }
        public string NroParte { get; set; }
        public string Ubicacion { get; set; }
        public string Marca { get; set; }
        public decimal SumatoriaIngresos { get; set; }
        public decimal SumatoriaSalidas { get; set; }
        public decimal SumatoriaTotalIngresos { get; set; }
        public decimal SumatoriaTotalSalidas { get; set; }
        public decimal SumatoriaPreciosIngresos { get; set; }
        public decimal SumatoriaPreciosSalidas { get; set; }
        public string GrupoLoteSerie { get; set; }
        public string v_NroLote { get; set; }
        public string v_NroSerie { get; set; }
        public DateTime   t_FechaCaducidad { get; set; }
       
        
       
        public object Clone()
        {
            return MemberwiseClone();
        }


    }

    public class CargaInicial
    {

        public string v_IdMovimientoDetalle { get; set; }
        public string v_IdProducto { get; set; }
        public string v_NombreProducto { get; set; }
        public DateTime? t_Fecha { get; set; }
        public int? i_IdTipoMovimiento { get; set; }
        public decimal? d_CantidadEmpaque { get; set; }
        public decimal? d_Cantidad { get; set; }
        public decimal? d_Precio { get; set; }
        public int? i_EsDevolucion { get; set; }
        public int? i_IdTipoMotivo { get; set; }
        public string NroPedido { get; set; }
        public string v_IdLinea { get; set; }
        public int IdMoneda { get; set; }
        public decimal TipoCambio { get; set; }
        public string ClienteProveedor { get; set; }
        public string Guia { get; set; }
        public string Documento { get; set; }
        public int IdAlmacen { get; set; }
        public int i_IdUnidad { get; set; }
        public string ValorUM { get; set; }
        public decimal DValorUM { get; set; }
        public string v_Fecha { get; set; }
        public decimal? Ingreso_CantidadEmpaque { get; set; }
        public decimal? Ingreso_PrecioEmpaque { get; set; }
        public decimal? Ingreso_TotalEmpaque { get; set; }
        public decimal? Salida_CantidadEmpaque { get; set; }
        public decimal? Salida_PrecioEmpaque { get; set; }
        public decimal? Salida_TotalEmpaque { get; set; }
        public decimal? Saldo_CantidadEmpaque { get; set; }
        public decimal? Saldo_PrecioEmpaque { get; set; }
        public decimal? Saldo_TotalEmpaque { get; set; }
        public string v_NombreTipoMovimiento { get; set; }
        public string Moneda { get; set; }
        public string Almacen { get; set; }
        public string CodigoProducto { get; set; }
        public string Origen { get; set; }


        public decimal? Ingreso_Cantidad { get; set; }
        public decimal? Ingreso_Precio { get; set; }
        public decimal? Ingreso_Total { get; set; }
        public decimal? Salida_Cantidad { get; set; }
        public decimal? Salida_Precio { get; set; }
        public decimal? Salida_Total { get; set; }
        public decimal? Saldo_Cantidad { get; set; }
        public decimal? Saldo_Precio { get; set; }
        public decimal? Saldo_Total { get; set; }

    }

    public class GuiasComprasNoConsideradas
    {
        public string GuiasNoseTomaronCuenta { get; set; }
        public int IdAlmacen { get; set; }
        public string v_IdLinea { get; set; }

    }

    public class CantidadValorizadas
    {
        public decimal? Ingreso_Cantidad { get; set; }
        public decimal? Ingreso_Precio { get; set; }
        public decimal? Ingreso_Total { get; set; }

        public decimal? Ingreso_CantidadDolares { get; set; }
        public decimal? Ingreso_PrecioDolares { get; set; }
        public decimal? Ingreso_TotalDolares { get; set; }



        public decimal? Saldo_Cantidad { get; set; }
        public decimal? Saldo_Total { get; set; }
        public decimal? Saldo_Precio { get; set; }



        public decimal? Saldo_CantidadDolares { get; set; }
        public decimal? Saldo_TotalDolares { get; set; }
        public decimal? Saldo_PrecioDolares { get; set; }




        public decimal? Salida_Cantidad { get; set; }
        public decimal? Salida_Precio { get; set; }
        public decimal? Salida_Total { get; set; }

        public decimal? Salida_CantidadDolares { get; set; }
        public decimal? Salida_PrecioDolares { get; set; }
        public decimal? Salida_TotalDolares { get; set; }

        public int NumeroElemento { get; set; }

    }

}
