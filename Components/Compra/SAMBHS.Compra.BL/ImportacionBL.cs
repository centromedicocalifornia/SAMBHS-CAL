using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Almacen.BL;
using System.Data.Objects;
using SAMBHS.Tesoreria.BL;
using SAMBHS.Common.BL;
using System.ComponentModel;
using System.Transactions;

namespace SAMBHS.Compra.BL
{
    public class ImportacionBL
    {
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        private static string periodo = Globals.ClientSession != null ? (Globals.ClientSession.i_Periodo ?? DateTime.Now.Year).ToString() : DateTime.Now.Year.ToString();
        #region Formulario-Importación
        public List<KeyValueDTO> ObtenerListadoImportaciones(ref OperationResult pobjOperationResult, string pstringPeriodo, string pstringMes)
        {
            try
            {

                var almacenpredeterminado = Globals.ClientSession.i_IdAlmacenPredeterminado.Value.ToString("00");
                string replicationID = Globals.ClientSession.ReplicationNodeID;
                SAMBHSEntitiesModelWin dbcontext = new SAMBHSEntitiesModelWin();
                var query = (from n in dbcontext.importacion
                             where n.i_Eliminado == 0 && n.v_Periodo == pstringPeriodo && n.v_Mes == pstringMes && n.v_IdImportacion.Substring(2, 2) == almacenpredeterminado && n.v_IdImportacion.Substring(0, 1) == replicationID

                             orderby n.v_Correlativo ascending
                             select new
                             {
                                 v_Correlativo = n.v_Correlativo,
                                 v_IdPedido = n.v_IdImportacion


                             }
                             );
                if (query.Any())
                {
                    var query2 = query.AsEnumerable()
                                .Select(x => new KeyValueDTO
                                {
                                    Value1 = x.v_Correlativo,
                                    Value2 = x.v_IdPedido
                                }).ToList();

                    return query2;
                }
                else
                {
                    return new List<KeyValueDTO> { new KeyValueDTO { Value1 = almacenpredeterminado + "000000" } };
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string DevolverTipoCambioPorFecha(ref OperationResult pobjOperationResult, DateTime Fecha)
        {
            try
            {
                var query = new TipoCambioBL().DevolverTipoCambioPorFechaCompra(ref pobjOperationResult, Fecha);
                if (pobjOperationResult.Success == 0) return "0";
                return query;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        /*public IQueryable ObtenerImportacionDetallesFob(ref OperationResult pobjOperationResult, string pstrIdImportacion)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = (from n in dbContext.importaciondetallefob
                             where n.v_IdImportacion == pstrIdImportacion
                             where n.i_Eliminado == 0 && n.v_IdImportacion == pstrIdImportacion
                             orderby n.t_InsertaFecha ascending
                             select n
                             );


                pobjOperationResult.Success = 1;

                return query.AsQueryable();
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        */



        public BindingList<importaciondetallefobDto> ObtenerImportacionDetallesFob(ref OperationResult pobjOperationResult, string pstrIdImportacion)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query1 = (from n in dbContext.importaciondetallefob
                              join a in dbContext.cliente on new { c = n.v_IdCliente, eliminado = 0 } equals new { c = a.v_IdCliente, eliminado = a.i_Eliminado.Value } into a_join
                              from a in a_join.DefaultIfEmpty()
                              where n.v_IdImportacion == pstrIdImportacion
                              where n.i_Eliminado == 0 && n.v_IdImportacion == pstrIdImportacion

                              select new importaciondetallefobDto
                 {
                     v_IdImportacionDetalleFob = n.v_IdImportacionDetalleFob,
                     v_IdImportacion = n.v_IdImportacion,
                     v_IdCliente = n.v_IdCliente,
                     i_IdTipoDocumento = n.i_IdTipoDocumento ?? -1,
                     v_SerieDocumento = n.v_SerieDocumento,
                     v_CorrelativoDocumento = n.v_CorrelativoDocumento,
                     t_FechaEmision = n.t_FechaEmision,
                     d_TipoCambio = n.d_TipoCambio,
                     d_ValorFob = n.d_ValorFob,
                     v_NroPedido = n.v_NroPedido,
                     i_Eliminado = n.i_Eliminado,
                     i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                     t_InsertaFecha = n.t_InsertaFecha,
                     i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                     t_ActualizaFecha = n.t_ActualizaFecha,
                     v_CodCliente = a != null ? a.v_CodCliente.Trim() : "",
                     v_RazonSocial = a != null ? (a.v_ApePaterno + " " + a.v_ApeMaterno + " " + a.v_PrimerNombre + " " + a.v_SegundoNombre + " " + a.v_RazonSocial).Trim() : "",


                 }
                             ).ToList();


                pobjOperationResult.Success = 1;
                var query = new BindingList<importaciondetallefobDto>(query1);
                return query;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }


        //public IQueryable ObtenerImportacionDetallesProducto(ref OperationResult pobjOperationResult, string pstrIdImportacion)
        //{
        //    try
        //    {
        //        SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

        //        var query = (from n in dbContext.importaciondetalleproducto
        //                     where n.i_Eliminado == 0 && n.v_IdImportacion == pstrIdImportacion
        //                     //orderby n.v_IdImportacionDetalleProducto 
        //                     select n
        //                     );


        //        pobjOperationResult.Success = 1;

        //        return query.AsQueryable();
        //    }
        //    catch (Exception ex)
        //    {
        //        pobjOperationResult.Success = 0;
        //        pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
        //        return null;
        //    }
        //}

        public BindingList<importaciondetalleproductoDto> ObtenerImportacionDetallesProducto(ref OperationResult pobjOperationResult, string pstrIdImportacion)
        {

            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = (from n in dbContext.importaciondetalleproducto

                             join b in dbContext.productodetalle on new { pd = n.v_IdProductoDetalle, eliminado = 0 } equals new { pd = b.v_IdProductoDetalle, eliminado = b.i_Eliminado.Value } into b_join
                             from b in b_join.DefaultIfEmpty()

                             join e in dbContext.producto on new { p = b.v_IdProducto, eliminado = 0 } equals new { p = e.v_IdProducto, eliminado = e.i_Eliminado.Value } into e_join
                             from e in e_join.DefaultIfEmpty()

                             join c in dbContext.cliente on new { c = n.v_IdCliente, eliminado = 0 } equals new { c = c.v_IdCliente, eliminado = c.i_Eliminado.Value } into c_join
                             from c in c_join.DefaultIfEmpty()

                             join d in dbContext.datahierarchy on new { Grupo = 17, eliminado = 0, Unid = e.i_IdUnidadMedida ?? -1 } equals new { Grupo = d.i_GroupId, eliminado = d.i_IsDeleted.Value, Unid = d.i_ItemId } into d_join
                             from d in d_join.DefaultIfEmpty()
                             where n.i_Eliminado == 0 && n.v_IdImportacion == pstrIdImportacion

                             select new
                             {

                                 v_IdImportacionDetalleProducto = n.v_IdImportacionDetalleProducto,
                                 v_IdImportacion = n.v_IdImportacion,
                                 v_IdProductoDetalle = n.v_IdProductoDetalle,
                                 v_IdMovimientoDetalle = n.v_IdMovimientoDetalle,
                                 d_Cantidad = n.d_Cantidad,
                                 d_CantidadEmpaque = n.d_CantidadEmpaque,
                                 i_IdUnidadMedida = n.i_IdUnidadMedida,
                                 d_Precio = n.d_Precio,
                                 d_ValorFob = n.d_ValorFob,
                                 d_Flete = n.d_Flete,
                                 d_Seguro = n.d_Seguro,
                                 d_AdValorem = n.d_AdValorem,
                                 d_Igv = n.d_Igv,
                                 d_OtrosGastos = n.d_OtrosGastos,
                                 d_Total = n.d_Total,
                                 d_CostoUnitario = n.d_CostoUnitario,
                                 d_PrecioVenta = n.d_PrecioVenta,
                                 v_NroFactura = n.v_NroFactura,
                                 v_IdCliente = n.v_IdCliente,
                                 v_NroPedido = n.v_NroPedido,
                                 i_Eliminado = n.i_Eliminado,
                                 i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                 t_InsertaFecha = n.t_InsertaFecha,
                                 i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                 t_ActualizaFecha = n.t_ActualizaFecha,
                                 d_CostoUnitarioCambio = n.d_CostoUnitarioCambio,
                                 v_NombreProducto = e != null ? e.v_Descripcion : "",
                                 v_CodCliente = c == null ? "" : c.v_CodCliente,
                                 v_CodigoInterno = e != null ? e.v_CodInterno : "",
                                 EsServicio = e != null ? e.i_EsServicio ?? 0 : 0,
                                 Empaque = e != null ? e.d_Empaque : 0,
                                 UMEmpaque = d != null ? d.v_Value1 : "0",
                                 i_IdUnidadMedidaProducto = e != null ? e.i_IdUnidadMedida.Value : -1,


                             }
                             ).ToList().Select(o => new importaciondetalleproductoDto
                             {
                                 v_IdImportacionDetalleProducto = o.v_IdImportacionDetalleProducto,
                                 v_IdImportacion = o.v_IdImportacion,
                                 v_IdProductoDetalle = o.v_IdProductoDetalle,
                                 v_IdMovimientoDetalle = o.v_IdMovimientoDetalle,
                                 d_Cantidad = o.d_Cantidad,
                                 d_CantidadEmpaque = o.d_CantidadEmpaque,
                                 i_IdUnidadMedida = o.i_IdUnidadMedida,
                                 d_Precio = o.d_Precio,
                                 d_ValorFob = o.d_ValorFob,
                                 d_Flete = o.d_Flete,
                                 d_Seguro = o.d_Seguro,
                                 d_AdValorem = o.d_AdValorem,
                                 d_Igv = o.d_Igv,
                                 d_OtrosGastos = o.d_OtrosGastos,
                                 d_Total = o.d_Total,
                                 d_CostoUnitario = o.d_CostoUnitario,
                                 d_PrecioVenta = o.d_PrecioVenta,
                                 v_NroFactura = o.v_NroFactura,
                                 v_IdCliente = o.v_IdCliente,
                                 v_NroPedido = o.v_NroPedido,
                                 i_Eliminado = o.i_Eliminado,
                                 i_InsertaIdUsuario = o.i_InsertaIdUsuario,
                                 t_InsertaFecha = o.t_InsertaFecha,
                                 i_ActualizaIdUsuario = o.i_ActualizaIdUsuario,
                                 t_ActualizaFecha = o.t_ActualizaFecha,
                                 d_CostoUnitarioCambio = o.d_CostoUnitarioCambio,
                                 v_NombreProducto = o.v_NombreProducto.Trim(),
                                 v_CodCliente = o.v_CodCliente,
                                 v_CodigoInterno = o.v_CodigoInterno,
                                 EsServicio = o.EsServicio,
                                 Empaque = o.Empaque.ToString(),
                                 UMEmpaque = o.UMEmpaque,
                                 i_IdUnidadMedidaProducto = o.i_IdUnidadMedidaProducto,

                             }).ToList();
                pobjOperationResult.Success = 1;
                var query1 = new BindingList<importaciondetalleproductoDto>(query);
                return query1;




            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }

        }

        public BindingList<importaciondetallegastosDto> ObtenerImportacionDetallesGastos(ref OperationResult pobjOperationResult, string pstrIdImportacion)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = (from n in dbContext.importaciondetallegastos
                             where n.v_IdImportacion == pstrIdImportacion
                             where n.i_Eliminado == 0 && n.v_IdImportacion == pstrIdImportacion
                             orderby n.t_InsertaFecha ascending
                             select new importaciondetallegastosDto
                             {
                                 v_IdImportacionDetalleGastos = n.v_IdImportacionDetalleGastos,
                                 v_IdImportacion = n.v_IdImportacion,
                                 v_GastoImportacion = n.v_GastoImportacion,
                                 v_IdAsientoContable = n.v_IdAsientoContable,
                                 i_IdTipoDocumento = n.i_IdTipoDocumento,
                                 v_SerieDocumento = n.v_SerieDocumento,
                                 v_CorrelativoDocumento = n.v_CorrelativoDocumento,
                                 t_FechaEmision = n.t_FechaEmision,
                                 v_Detalle = n.v_Detalle,
                                 d_TipoCambio = n.d_TipoCambio.Value,
                                 i_IdMoneda = n.i_IdMoneda.Value,
                                 d_ValorVenta = n.d_ValorVenta.Value,
                                 d_NAfectoDetraccion = n.d_NAfectoDetraccion.Value,
                                 d_Igv = n.d_Igv.Value,
                                 d_ImporteSoles = n.d_ImporteSoles.Value,
                                 d_ImporteDolares = n.d_ImporteDolares,
                                 i_CCosto = n.i_CCosto,
                                 v_Glosa = n.v_Glosa == null ? "" : n.v_Glosa,
                                 i_IdTipoDocRef = n.i_IdTipoDocRef.Value,
                                 v_SerieDocRef = n.v_SerieDocRef,
                                 v_CorrelativoDocRef = n.v_CorrelativoDocRef,
                                 i_Eliminado = n.i_Eliminado.Value,
                                 i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                 t_InsertaFecha = n.t_InsertaFecha,
                                 i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                 t_ActualizaFecha = n.t_ActualizaFecha,
                                 t_FechaRegistro = n.t_FechaRegistro,
                                 i_EsDetraccion = n.i_EsDetraccion ?? 0,
                                 i_CodigoDetraccion = n.i_CodigoDetraccion ?? 0,
                                 d_PorcentajeDetraccion = n.d_PorcentajeDetraccion ?? 0,
                                 v_NroDetraccion = n.v_NroDetraccion ?? "",
                                 t_FechaDetraccion = n.t_FechaDetraccion == null ? DateTime.Now : n.t_FechaDetraccion,
                                 d_ValorSolesDetraccion = n.d_ValorSolesDetraccion ?? 0,
                                 d_ValorDolaresDetraccion = n.d_ValorDolaresDetraccion ?? 0,
                                 d_ValorSolesDetraccionNoAfecto = n.d_ValorSolesDetraccionNoAfecto ?? 0,
                                 d_ValorDolaresDetraccionNoAfecto = n.d_ValorSolesDetraccionNoAfecto ?? 0,

                             }
                             ).ToList();


                //var queryFinal = (from n in query
                //                  select new importaciondetallegastosDto
                //                  {

                //                      v_IdImportacionDetalleGastos = n.v_IdImportacionDetalleGastos,
                //                      v_IdImportacion = n.v_IdImportacion,
                //                      v_GastoImportación = n.v_GastoImportación,
                //                      v_IdAsientoContable = n.v_IdAsientoContable,
                //                      i_IdTipoDocumento = n.i_IdTipoDocumento,
                //                      v_SerieDocumento = n.v_SerieDocumento,
                //                      v_CorrelativoDocumento = n.v_CorrelativoDocumento,
                //                      t_FechaEmision = n.t_FechaEmision,
                //                      v_Detalle = n.v_Detalle,
                //                      d_TipoCambio = n.d_TipoCambio.Value,
                //                      i_IdMoneda = n.i_IdMoneda.Value,
                //                      d_ValorVenta = n.d_ValorVenta.Value,
                //                      d_NAfectoDetraccion = n.d_NAfectoDetraccion.Value,
                //                      d_Igv = n.d_Igv.Value,
                //                      d_ImporteSoles = n.d_ImporteSoles.Value,
                //                      d_ImporteDolares = n.d_ImporteDolares.Value,
                //                      i_CCosto =  n.i_CCosto,
                //                      v_Glosa = n.v_Glosa == null ? "" : n.v_Glosa,
                //                      i_IdTipoDocRef = n.i_IdTipoDocRef.Value,
                //                      v_SerieDocRef = n.v_SerieDocRef,
                //                      v_CorrelativoDocRef = n.v_CorrelativoDocRef,
                //                      i_Eliminado = n.i_Eliminado.Value,
                //                      i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                //                      t_InsertaFecha = n.t_InsertaFecha,
                //                      i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                //                      t_ActualizaFecha = n.t_ActualizaFecha,



                //                  }).ToList();
                pobjOperationResult.Success = 1;

                return new BindingList<importaciondetallegastosDto>(query);
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public importacionDto ObtenerImportacionCabecera(ref OperationResult objOperationResult, string pstrIdImportacion)
        {

            try
            {

                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var ObjEntityImportacion = (from a in dbContext.importacion

                                            join b in dbContext.datahierarchy on new { y = a.i_IdTipoVia.Value, x = 49 } //Via
                                           equals new { y = b.i_ItemId, x = b.i_GroupId } into b_join
                                            from b in b_join.DefaultIfEmpty()

                                            join c in dbContext.datahierarchy on new { y = a.i_IdDestino.Value, x = 24 } //Destino
                                            equals new { y = c.i_ItemId, x = c.i_GroupId } into c_join
                                            from c in c_join.DefaultIfEmpty()

                                            join d in dbContext.datahierarchy on new { y = a.i_IdEstablecimiento.Value, x = 25 } // Establecimiento
                                            equals new { y = d.i_ItemId, x = d.i_GroupId } into d_join
                                            from d in d_join.DefaultIfEmpty()

                                            join e in dbContext.datahierarchy on new { y = a.i_IdAgencia.Value, x = 50 }
                                            equals new { y = e.i_ItemId, x = e.i_GroupId } into e_join
                                            from e in e_join.DefaultIfEmpty()

                                            join f in dbContext.cliente on a.v_IdClienteDoc1 equals f.v_IdCliente into f_join
                                            from f in f_join.DefaultIfEmpty()

                                            join g in dbContext.cliente on a.v_IdClienteDoc2 equals g.v_IdCliente into g_join
                                            from g in g_join.DefaultIfEmpty()

                                            join h in dbContext.cliente on a.v_IdClienteDoc3 equals h.v_IdCliente into h_join
                                            from h in h_join.DefaultIfEmpty()

                                            join i in dbContext.cliente on a.v_IdClienteDoc4 equals i.v_IdCliente into i_join
                                            from i in i_join.DefaultIfEmpty()

                                            join j in dbContext.datahierarchy on new { y = a.i_IdSerieDocumento.Value, x = 53 }
                                            equals new { y = j.i_ItemId, x = j.i_GroupId } into j_join
                                            from j in j_join

                                            where a.v_IdImportacion == pstrIdImportacion

                                            select new importacionDto
                                            {
                                                v_Periodo = a.v_Periodo,
                                                v_Mes = a.v_Mes,
                                                v_Correlativo = a.v_Correlativo,
                                                i_Igv = a.i_Igv,
                                                i_IdTipoDocumento = a.i_IdTipoDocumento,
                                                i_IdSerieDocumento = a.i_IdSerieDocumento,
                                                v_CorrelativoDocumento = a.v_CorrelativoDocumento,
                                                i_IdTipoVia = a.i_IdTipoVia,
                                                i_IdDestino = a.i_IdDestino,
                                                t_FechaRegistro = a.t_FechaRegistro,
                                                t_FechaEmision = a.t_FechaEmision,
                                                t_FechaPagoVencimiento = a.t_FechaPagoVencimiento,
                                                d_TipoCambio = a.d_TipoCambio,
                                                i_IdEstablecimiento = a.i_IdEstablecimiento,
                                                v_NroOrden = a.v_NroOrden,
                                                v_Bl = a.v_Bl,
                                                t_FechaArrivo = a.t_FechaArrivo,
                                                t_FechaLLegada = a.t_FechaLLegada,
                                                i_IdAgencia = a.i_IdAgencia,
                                                v_Terminal = a.v_Terminal,
                                                v_Ent1Serie = a.v_Ent1Serie,
                                                v_Ent1Correlativo = a.v_Ent1Correlativo,
                                                v_Ent2Serie = a.v_Ent2Serie,
                                                v_Ent2Correlativo = a.v_Ent2Correlativo,
                                                i_IdAlmacen = a.i_IdAlmacen,
                                                d_Utilidad = a.d_Utilidad,
                                                d_TotalValorFob = a.d_TotalValorFob,
                                                d_Flete = a.d_Flete,
                                                i_IdTipoDocumento1 = a.i_IdTipoDocumento1,
                                                v_SerieDocumento1 = a.v_SerieDocumento1,
                                                v_CorrelativoDocumento1 = a.v_CorrelativoDocumento1,
                                                t_FechaEmisionDoc1 = a.t_FechaEmisionDoc1,
                                                d_TipoCambioDoc1 = a.d_TipoCambioDoc1,
                                                RucProveedor1 = f.v_NroDocIdentificacion,
                                                NombreProveedor1 = (f.v_ApePaterno.Trim() + " " + f.v_ApeMaterno.Trim() + " " + f.v_PrimerNombre.Trim() + " " + f.v_SegundoNombre.Trim() + " " + f.v_RazonSocial.Trim()).Trim(),
                                                i_PagaSeguro = a.i_PagaSeguro,
                                                d_PagoSeguro = a.d_PagoSeguro,
                                                i_IdTipoDocumento2 = a.i_IdTipoDocumento2,
                                                v_SerieDocumento2 = a.v_SerieDocumento2,
                                                v_CorrelativoDocumento2 = a.v_CorrelativoDocumento2,
                                                t_FechaEmisionDoc2 = a.t_FechaEmisionDoc2,
                                                d_TipoCambioDoc2 = a.d_TipoCambioDoc2,
                                                RucProveedor2 = g.v_NroDocIdentificacion,
                                                NombreProveedor2 = (g.v_ApePaterno.Trim() + " " + g.v_ApeMaterno.Trim() + g.v_PrimerNombre.Trim() + " " + g.v_SegundoNombre.Trim() + " " + g.v_RazonSocial.Trim()).Trim(),
                                                i_AdValorem = a.i_AdValorem,
                                                d_AdValorem = a.d_AdValorem,
                                                i_IdTipoDocumento3 = a.i_IdTipoDocumento3,
                                                v_SerieDocumento3 = a.v_SerieDocumento3,
                                                v_CorrelativoDocumento3 = a.v_CorrelativoDocumento3,
                                                t_FechaEmisionDoc3 = a.t_FechaEmisionDoc3,
                                                d_TipoCambioDoc3 = a.d_TipoCambioDoc3,
                                                RucProveedor3 = h.v_NroDocIdentificacion,
                                                NombreProveedor3 = (h.v_ApePaterno.Trim() + " " + h.v_ApeMaterno.Trim() + " " + h.v_PrimerNombre.Trim() + " " + h.v_SegundoNombre.Trim() + " " + h.v_RazonSocial.Trim()).Trim(),
                                                d_SubTotal = a.d_SubTotal,
                                                d_Tax = a.d_Tax,
                                                i_IdTipoDocumento4 = a.i_IdTipoDocumento4,
                                                v_SerieDocumento4 = a.v_SerieDocumento4,
                                                v_CorrelativoDoc4 = a.v_CorrelativoDoc4,
                                                t_FechaEmisionDoc4 = a.t_FechaEmisionDoc4,
                                                d_TipoCambioDoc4 = a.d_TipoCambioDoc4,
                                                RucProveedor4 = i.v_NroDocIdentificacion,
                                                NombreProveedor4 = (i.v_PrimerNombre + " " + i.v_ApeMaterno + " " + i.v_PrimerNombre + " " + i.v_SegundoNombre + " " + i.v_RazonSocial).Trim(),
                                                d_Prom = a.d_Prom,
                                                d_TasaDespacho = a.d_TasaDespacho,
                                                d_Percepcion = a.d_Percepcion,
                                                i_IdMoneda = a.i_IdMoneda,
                                                d_TotalIgv = a.d_TotalIgv,
                                                d_Intereses = a.d_Intereses,
                                                i_IdEstado = a.i_IdEstado,
                                                d_ValorFob = a.d_ValorFob,
                                                d_TotalSeguro = a.d_TotalSeguro,
                                                d_Igv = a.d_Igv,
                                                d_TotalFlete = a.d_TotalFlete,
                                                d_TotalAdValorem = a.d_TotalAdValorem,
                                                d_TotalOtrosGastos = a.d_TotalOtrosGastos,
                                                d_OtrosGastos = a.d_OtrosGastos,
                                                v_IdImportacion = a.v_IdImportacion,
                                                i_InsertaIdUsuario = a.i_InsertaIdUsuario,
                                                t_InsertaFecha = a.t_InsertaFecha,
                                                v_IdClienteDoc1 = a.v_IdClienteDoc1,
                                                v_IdClienteDoc2 = a.v_IdClienteDoc2,
                                                v_IdClienteDoc3 = a.v_IdClienteDoc3,
                                                v_IdClienteDoc4 = a.v_IdClienteDoc4,
                                                i_IdTipoDocRerefencia = a.i_IdTipoDocRerefencia ?? -1,
                                                v_NumeroDocRerefencia = a.v_NumeroDocRerefencia,
                                                v_IdDocumentoReferencia = a.v_IdDocumentoReferencia,

                                            }


                                          ).FirstOrDefault();

                objOperationResult.Success = 1;
                return ObjEntityImportacion;


            }
            catch (Exception e)
            {

                objOperationResult.Success = 0;
                objOperationResult.ExceptionMessage = Utils.ExceptionFormatter(e);
                return null;

            }


        }
        public bool ExisteNroRegistro(string Periodo, string Mes, string Correlativo)
        {
            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
            string replicationID = Globals.ClientSession.ReplicationNodeID;
            var Registro = (from n in dbContext.importacion
                            where n.i_Eliminado == 0 && n.v_Periodo == Periodo && n.v_Mes == Mes && n.v_Correlativo == Correlativo && n.v_IdImportacion.Substring(0, 1) == replicationID

                            select n).FirstOrDefault();

            if (Registro == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string InsertarImportacion(ref OperationResult pobjOperationResult, importacionDto pobjDtoEntity, List<string> ClientSession, List<importaciondetallefobDto> pTemp_InsertarDetalleFob, List<importaciondetalleproductoDto> pTemp_InsertarDetalleProducto, List<importaciondetallegastosDto> pTemp_InsertarDetalleGastos)
        {
            try
            {


                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        SecuentialBL objSecuentialBL = new SecuentialBL();
                        importacion objEntityImportacion = importacionAssembler.ToEntity(pobjDtoEntity);
                        importaciondetallefobDto pobjImportacionDetalleFob = new importaciondetallefobDto();
                        importaciondetalleproductoDto pobjImportacionDetalleProd = new importaciondetalleproductoDto();
                        importaciondetallegastosDto pobjImportacionDetalleGastos = new importaciondetallegastosDto();

                        int SecuentialId = 0;
                        string newIdImportacion = string.Empty;
                        string newIdImportacionDetalleFob = string.Empty;
                        string newIdImportacionDetalleProducto = string.Empty;
                        string newIdImportacionDetalleGastos = string.Empty;
                        int intNodeId;
                        # region Inserta Nota Ingreso

                        if (pTemp_InsertarDetalleProducto.Count() > 0 & pobjDtoEntity.i_IdEstado == 1)
                        {
                            movimientoDto _movimientoDto = new movimientoDto();
                            movimientodetalleDto _movimientodetalleDto = new movimientodetalleDto();
                            MovimientoBL _objMovimientoBL = new MovimientoBL();

                            List<movimientodetalleDto> _movimientodetalleDtos = new List<movimientodetalleDto>();


                            var IdProveedorFOB = pTemp_InsertarDetalleFob.Count() == 1 ? pTemp_InsertarDetalleFob.FirstOrDefault().v_IdCliente : null;


                            if ((pTemp_InsertarDetalleProducto.Find(p => p.EsServicio == 0) != null))
                            {

                                List<KeyValueDTO> ListaMovimientos = new List<KeyValueDTO>();
                                ListaMovimientos = _objMovimientoBL.ObtenerListadoMovimientos(ref pobjOperationResult, pobjDtoEntity.v_Periodo, pobjDtoEntity.v_Mes, (int)TipoDeMovimiento.NotadeIngreso);

                                int MaxMovimiento;
                                MaxMovimiento = ListaMovimientos.Count() > 0 ? int.Parse(ListaMovimientos[ListaMovimientos.Count() - 1].Value1.ToString()) : 0;

                                _movimientoDto = new movimientoDto();
                                MaxMovimiento++;
                                _movimientoDto.d_TipoCambio = pobjDtoEntity.d_TipoCambio;
                                _movimientoDto.i_IdAlmacenOrigen = pobjDtoEntity.i_IdAlmacen;
                                _movimientoDto.i_IdMoneda = pobjDtoEntity.i_IdMoneda;
                                _movimientoDto.i_IdTipoMotivo = 6;//Importación
                                _movimientoDto.t_Fecha = pobjDtoEntity.t_FechaRegistro;
                                _movimientoDto.v_Mes = pobjDtoEntity.v_Mes.Trim();
                                _movimientoDto.v_Periodo = pobjDtoEntity.v_Periodo.Trim();
                                _movimientoDto.i_IdTipoMovimiento = (int)TipoDeMovimiento.NotadeIngreso;
                                _movimientoDto.v_Correlativo = MaxMovimiento.ToString("00000000");
                                _movimientoDto.v_IdCliente = IdProveedorFOB;
                                _movimientoDto.v_OrigenTipo = "I";
                                _movimientoDto.i_EsDevolucion = 0;
                                _movimientoDto.i_IdEstablecimiento = pobjDtoEntity.i_IdEstablecimiento.Value;
                                _movimientoDto.v_OrigenRegCorrelativo = pobjDtoEntity.v_Correlativo;
                                _movimientoDto.v_OrigenRegMes = pobjDtoEntity.v_Mes;
                                _movimientoDto.v_OrigenRegPeriodo = pobjDtoEntity.v_Periodo;
                                foreach (importaciondetalleproductoDto _importaciondetalleproductoDto in pTemp_InsertarDetalleProducto.Where(p => p.EsServicio == 0).ToList())
                                {
                                    _movimientodetalleDto = new movimientodetalleDto();
                                    _movimientodetalleDto.v_IdProductoDetalle = _importaciondetalleproductoDto.v_IdProductoDetalle.Trim();
                                    _movimientodetalleDto.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento;

                                    var SerieDocumento = (from n in dbContext.datahierarchy
                                                          where n.i_GroupId == 53 && n.i_ItemId == pobjDtoEntity.i_IdSerieDocumento
                                                          select new { v_SerieDocumento = n.v_Value2 }).First();

                                    _movimientodetalleDto.v_NumeroDocumento = int.Parse(SerieDocumento.v_SerieDocumento).ToString("0000") + "-" + pobjDtoEntity.v_CorrelativoDocumento.Trim();
                                    _movimientodetalleDto.d_Cantidad = _importaciondetalleproductoDto.d_Cantidad;
                                    _movimientodetalleDto.i_IdUnidad = _importaciondetalleproductoDto.i_IdUnidadMedida;
                                    _movimientodetalleDto.d_Precio = Utils.Windows.DevuelveValorRedondeado(_importaciondetalleproductoDto.d_CostoUnitario.Value, 6);
                                    _movimientodetalleDto.d_Total = Utils.Windows.DevuelveValorRedondeado((_movimientodetalleDto.d_Cantidad.Value * _movimientodetalleDto.d_Precio.Value), 2);

                                    _movimientodetalleDto.d_PrecioCambio = Utils.Windows.DevuelveValorRedondeado(_importaciondetalleproductoDto.d_CostoUnitarioCambio.Value, 6);
                                    _movimientodetalleDto.d_TotalCambio = Utils.Windows.DevuelveValorRedondeado((_movimientodetalleDto.d_Cantidad.Value * _movimientodetalleDto.d_PrecioCambio.Value), 2);

                                    _movimientodetalleDto.v_NroPedido = _importaciondetalleproductoDto.v_NroPedido == null ? null : _importaciondetalleproductoDto.v_NroPedido.Trim();
                                    _movimientodetalleDto.d_CantidadEmpaque = _importaciondetalleproductoDto.d_CantidadEmpaque;
                                    _movimientodetalleDtos.Add(_movimientodetalleDto);


                                }
                                _movimientoDto.d_TotalCantidad = Utils.Windows.DevuelveValorRedondeado(_movimientodetalleDtos.Sum(p => p.d_Cantidad.Value), 2);
                                _movimientoDto.d_TotalPrecio = _movimientodetalleDtos.Sum(p => p.d_Total.Value);
                                _objMovimientoBL.InsertarMovimiento(ref pobjOperationResult, _movimientoDto, Globals.ClientSession.GetAsList(), _movimientodetalleDtos);
                                if (pobjOperationResult.Success == 0) return string.Empty;
                                _movimientodetalleDtos = new List<movimientodetalleDto>();

                            }
                            dbContext.SaveChanges();

                        }

                        #endregion

                        #region Inserta Cabecera

                        objEntityImportacion.t_InsertaFecha = DateTime.Now;
                        objEntityImportacion.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntityImportacion.i_Eliminado = 0;


                        // Autogeneramos el Pk de la tabla
                        intNodeId = int.Parse(ClientSession[0]);
                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 51);
                        newIdImportacion = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "XA");
                        objEntityImportacion.v_IdImportacion = newIdImportacion;
                        dbContext.AddToimportacion(objEntityImportacion);
                        #endregion

                        #region Inserta ImportacionDetalleFob
                        foreach (importaciondetallefobDto importaciondetallefobDto in pTemp_InsertarDetalleFob)
                        {
                            importaciondetallefob objEntityImportacionDetalleFob = importaciondetallefobAssembler.ToEntity(importaciondetallefobDto);
                            SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 52);
                            newIdImportacionDetalleFob = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "XB");
                            objEntityImportacionDetalleFob.v_IdImportacionDetalleFob = newIdImportacionDetalleFob;
                            objEntityImportacionDetalleFob.v_IdImportacion = newIdImportacion;
                            objEntityImportacionDetalleFob.t_InsertaFecha = DateTime.Now;
                            objEntityImportacionDetalleFob.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                            objEntityImportacionDetalleFob.i_Eliminado = 0;
                            dbContext.AddToimportaciondetallefob(objEntityImportacionDetalleFob);
                            Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "importacionDetalleFob", importaciondetallefobDto.v_IdImportacionDetalleFob);

                        }
                        dbContext.SaveChanges();
                        #endregion

                        #region Inserta ImportacionDetalleProducto

                        foreach (importaciondetalleproductoDto importaciondetalleproductoDto in pTemp_InsertarDetalleProducto)
                        {

                            movimientodetalleDto NotadeIngresoDetalle = new movimientodetalleDto();
                            if (Globals.ClientSession.i_IncluirPedidoExportacionCompraVenta == 1)
                            {
                                NotadeIngresoDetalle = (from n in dbContext.movimientodetalle

                                                        join J1 in dbContext.movimiento on new { idMovimiento = n.v_IdMovimiento }
                                                                                        equals new { idMovimiento = J1.v_IdMovimiento } into J1_join
                                                        from J1 in J1_join.DefaultIfEmpty()

                                                        where n.i_Eliminado == 0 && n.v_IdProductoDetalle == importaciondetalleproductoDto.v_IdProductoDetalle && J1.v_OrigenTipo == "I" && J1.v_OrigenRegPeriodo == objEntityImportacion.v_Periodo
                                                              && J1.v_OrigenRegMes == objEntityImportacion.v_Mes && J1.v_OrigenRegCorrelativo == objEntityImportacion.v_Correlativo
                                                             && n.v_NroPedido == importaciondetalleproductoDto.v_NroPedido

                                                        select new movimientodetalleDto { v_IdMovimientoDetalle = n.v_IdMovimientoDetalle }).FirstOrDefault();


                            }
                            else
                            {
                                NotadeIngresoDetalle = (from n in dbContext.movimientodetalle

                                                        join J1 in dbContext.movimiento on new { idMovimiento = n.v_IdMovimiento }
                                                                                        equals new { idMovimiento = J1.v_IdMovimiento } into J1_join
                                                        from J1 in J1_join.DefaultIfEmpty()

                                                        where n.i_Eliminado == 0 && n.v_IdProductoDetalle == importaciondetalleproductoDto.v_IdProductoDetalle && J1.v_OrigenTipo == "I" && J1.v_OrigenRegPeriodo == objEntityImportacion.v_Periodo
                                                              && J1.v_OrigenRegMes == objEntityImportacion.v_Mes && J1.v_OrigenRegCorrelativo == objEntityImportacion.v_Correlativo

                                                        select new movimientodetalleDto { v_IdMovimientoDetalle = n.v_IdMovimientoDetalle }).FirstOrDefault();
                            }



                            importaciondetalleproducto objEntityImportacionDetalleProducto = importaciondetalleproductoAssembler.ToEntity(importaciondetalleproductoDto);
                            SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 54);
                            newIdImportacionDetalleProducto = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "XD");

                            objEntityImportacionDetalleProducto.v_IdImportacionDetalleProducto = newIdImportacionDetalleProducto;
                            objEntityImportacionDetalleProducto.v_IdImportacion = newIdImportacion;
                            objEntityImportacionDetalleProducto.t_InsertaFecha = DateTime.Now;
                            objEntityImportacionDetalleProducto.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                            objEntityImportacionDetalleProducto.i_Eliminado = 0;
                            objEntityImportacionDetalleProducto.v_IdMovimientoDetalle = NotadeIngresoDetalle.v_IdMovimientoDetalle;
                            dbContext.AddToimportaciondetalleproducto(objEntityImportacionDetalleProducto);
                            Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "importacionDetalleProducto", newIdImportacionDetalleProducto);

                        }




                        #endregion

                        #region Inserta ImportacionDetalleGastos
                        foreach (importaciondetallegastosDto importaciondetallegastosDto in pTemp_InsertarDetalleGastos)
                        {
                            importaciondetallegastos objEntityImportacionDetalleGastos = importaciondetallegastosAssembler.ToEntity(importaciondetallegastosDto);

                            SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 53);
                            newIdImportacionDetalleGastos = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "XC");

                            objEntityImportacionDetalleGastos.v_IdImportacionDetalleGastos = newIdImportacionDetalleGastos;
                            objEntityImportacionDetalleGastos.v_IdImportacion = newIdImportacion;
                            objEntityImportacionDetalleGastos.t_InsertaFecha = DateTime.Now;
                            objEntityImportacionDetalleGastos.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                            objEntityImportacionDetalleGastos.i_Eliminado = 0;
                            dbContext.AddToimportaciondetallegastos(objEntityImportacionDetalleGastos);
                            Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "importacionDetalleGastos", newIdImportacionDetalleGastos);
                            //Genera Compra
                            if (pobjDtoEntity.i_IdEstado == 1)
                            {

                                List<KeyValueDTO> _ListadoCompras = new List<KeyValueDTO>();
                                ComprasBL _objComprasBL = new ComprasBL();
                                compraDto _compraDto = new compraDto();
                                compradetalleDto _compradetalleDto = new compradetalleDto();
                                OperationResult objOperationResult = new OperationResult();
                                List<compradetalleDto> _TempDetalle_AgregarCompraDto = new List<compradetalleDto>();

                                if (!ExistenciaDocumentoenCompras(importaciondetallegastosDto.i_IdTipoDocumento.Value, importaciondetallegastosDto.v_SerieDocumento, importaciondetallegastosDto.v_CorrelativoDocumento, importaciondetallegastosDto.v_Detalle, importaciondetallegastosDto.v_IdImportacionDetalleGastos))
                                {
                                    string Mes = string.Empty;
                                    string Periodo = "";
                                    if (Globals.ClientSession.i_FechaEmision.Value.ToString() == "1") // FechaRegistroOtrosGastos-Desde 4 junio
                                    {
                                        //Mes = importaciondetallegastosDto.t_FechaEmision.Value.Month.ToString("00").Trim(); //Antes del  4 de junio
                                        Mes = importaciondetallegastosDto.t_FechaRegistro.Value.Month.ToString("00").Trim();
                                        Periodo = importaciondetallegastosDto.t_FechaRegistro.Value.Year.ToString().Trim();
                                        _ListadoCompras = _objComprasBL.ObtenerListadoCompras(ref objOperationResult, importaciondetallegastosDto.t_FechaRegistro.Value.Year.ToString(), Mes);
                                        _compraDto.t_FechaEmision = importaciondetallegastosDto.t_FechaEmision;
                                        _compraDto.t_FechaRegistro = importaciondetallegastosDto.t_FechaRegistro;
                                        _compraDto.t_FechaVencimiento = importaciondetallegastosDto.t_FechaEmision;

                                    }
                                    else
                                    {
                                        Mes = pobjDtoEntity.t_FechaRegistro.Value.Month.ToString("00").Trim(); //FechaRegistroImportacion
                                        Periodo = pobjDtoEntity.t_FechaRegistro.Value.Year.ToString();
                                        _ListadoCompras = _objComprasBL.ObtenerListadoCompras(ref objOperationResult, pobjDtoEntity.t_FechaRegistro.Value.Year.ToString(), Mes);
                                        _compraDto.t_FechaEmision = importaciondetallegastosDto.t_FechaEmision;
                                        _compraDto.t_FechaRegistro = objEntityImportacion.t_FechaRegistro;
                                        _compraDto.t_FechaVencimiento = importaciondetallegastosDto.t_FechaEmision;

                                    }

                                    int _MaxMovimiento;
                                    _MaxMovimiento = _ListadoCompras.Count() > 0 ? int.Parse(_ListadoCompras[_ListadoCompras.Count() - 1].Value1.ToString()) : 0;
                                    _MaxMovimiento++;
                                    _compraDto.v_Periodo = Periodo;
                                    _compraDto.v_Mes = Mes;
                                    _compraDto.v_Correlativo = _MaxMovimiento.ToString("00000000");
                                    //_compraDto.v_Correlativo = _MaxMovimiento.ToString("00000000");
                                    _compraDto.v_IdDocumentoReferencia = newIdImportacionDetalleGastos;
                                    _compraDto.i_IdIgv = Globals.ClientSession.i_IdIgv;
                                    _compraDto.i_IdTipoDocumento = importaciondetallegastosDto.i_IdTipoDocumento;
                                    _compraDto.v_SerieDocumento = importaciondetallegastosDto.v_SerieDocumento;
                                    _compraDto.v_CorrelativoDocumento = importaciondetallegastosDto.v_CorrelativoDocumento;
                                    _compraDto.i_IdCondicionPago = 1; //Por Confirmar

                                    _compraDto.t_FechaRef = importaciondetallegastosDto.t_FechaEmision;
                                    _compraDto.i_IdEstado = 1;
                                    _compraDto.i_IdTipoDocumentoRef = importaciondetallegastosDto.i_IdTipoDocRef;
                                    _compraDto.v_CorrelativoDocumentoRef = importaciondetallegastosDto.v_CorrelativoDocRef;
                                    _compraDto.v_SerieDocumentoRef = importaciondetallegastosDto.v_SerieDocRef;
                                    _compraDto.v_IdProveedor = importaciondetallegastosDto.v_Detalle;
                                    _compraDto.v_GuiaRemisionSerie = string.Empty;
                                    _compraDto.v_GuiaRemisionCorrelativo = string.Empty;
                                    _compraDto.d_TipoCambio = importaciondetallegastosDto.d_TipoCambio;
                                    _compraDto.v_Glosa = importaciondetallegastosDto.v_Glosa;
                                    _compraDto.i_IdMoneda = importaciondetallegastosDto.i_IdMoneda;
                                    _compraDto.i_IdTipoCompra = importaciondetallegastosDto.i_IdMoneda == 1 ? (int)TipoCompra.MonedaNacional : (int)TipoCompra.MonedaExtranjera;
                                    _compraDto.i_EsAfectoIgv = importaciondetallegastosDto.d_Igv == 0 ? 1 : 1;
                                    _compraDto.i_IdEstablecimiento = pobjDtoEntity.i_IdEstablecimiento;
                                    _compraDto.i_EsDetraccion = importaciondetallegastosDto.i_EsDetraccion;
                                    _compraDto.i_CodigoDetraccion = importaciondetallegastosDto.i_CodigoDetraccion;
                                    _compraDto.v_NroDetraccion = importaciondetallegastosDto.v_NroDetraccion;
                                    _compraDto.d_PorcentajeDetraccion = importaciondetallegastosDto.d_PorcentajeDetraccion;
                                    _compraDto.t_FechaDetraccion = importaciondetallegastosDto.t_FechaDetraccion;
                                    _compraDto.i_PreciosIncluyenIgv = 0;
                                    _compraDto.i_DeduccionAnticipio = 0;
                                    var Proveedor = (from n in dbContext.cliente
                                                     where n.i_Eliminado == 0 && n.v_FlagPantalla == "V" & n.v_IdCliente == importaciondetallegastosDto.v_Detalle
                                                     select n).FirstOrDefault();
                                    _compraDto.NombreProveedor = Proveedor == null ? null : (Proveedor.v_PrimerNombre + " " + Proveedor.v_ApePaterno + " " + Proveedor.v_ApeMaterno + " " + Proveedor.v_RazonSocial).Trim();

                                    if (importaciondetallegastosDto.d_ValorVenta != 0 && importaciondetallegastosDto.d_NAfectoDetraccion != 0) // Se generan dos registros en la grilla , Uno para el Valor Venta y otro para el No Afecto Igv
                                    {
                                        //Fila Valor Venta
                                        _compradetalleDto.v_NroCuenta = importaciondetallegastosDto.v_IdAsientoContable;
                                        _compradetalleDto.i_Anticipio = 0;
                                        _compradetalleDto.i_IdAlmacen = pobjDtoEntity.i_IdAlmacen;
                                        _compradetalleDto.v_IdProductoDetalle = null;
                                        _compradetalleDto.d_Cantidad = 0;
                                        _compradetalleDto.d_Precio = 0;
                                        _compradetalleDto.i_IdUnidadMedida = -1;
                                        _compradetalleDto.d_ValorVenta = importaciondetallegastosDto.d_ValorVenta;
                                        _compradetalleDto.d_PrecioVenta = importaciondetallegastosDto.i_IdMoneda == 1 ? importaciondetallegastosDto.d_ImporteSoles - importaciondetallegastosDto.d_NAfectoDetraccion : importaciondetallegastosDto.d_ImporteDolares - importaciondetallegastosDto.d_NAfectoDetraccion;
                                        _compradetalleDto.d_Igv = importaciondetallegastosDto.d_Igv;
                                        _compradetalleDto.i_IdDestino = 1;
                                        _compradetalleDto.i_IdCentroCostos = importaciondetallegastosDto.i_CCosto;
                                        _compradetalleDto.v_Glosa = string.Empty;
                                        _compradetalleDto.v_NroGuiaRemision = string.Empty;
                                        _compradetalleDto.d_ValorDolaresDetraccion = importaciondetallegastosDto.d_ValorDolaresDetraccion;
                                        _compradetalleDto.d_ValorSolesDetraccion = importaciondetallegastosDto.d_ValorSolesDetraccion;
                                        _compradetalleDto.v_NroPedido = string.Empty;

                                        _TempDetalle_AgregarCompraDto.Add(_compradetalleDto);
                                        _compradetalleDto = new compradetalleDto();
                                        //Fila Valor No Afecto
                                        _compradetalleDto.v_NroCuenta = importaciondetallegastosDto.v_IdAsientoContable;
                                        _compradetalleDto.i_Anticipio = 0;
                                        _compradetalleDto.i_IdAlmacen = pobjDtoEntity.i_IdAlmacen;
                                        _compradetalleDto.v_IdProductoDetalle = null;
                                        _compradetalleDto.d_Cantidad = 0;
                                        _compradetalleDto.d_Precio = 0;
                                        _compradetalleDto.i_IdUnidadMedida = -1;
                                        _compradetalleDto.d_ValorVenta = importaciondetallegastosDto.d_NAfectoDetraccion;
                                        _compradetalleDto.d_PrecioVenta = importaciondetallegastosDto.d_NAfectoDetraccion;
                                        _compradetalleDto.d_Igv = 0;
                                        _compradetalleDto.i_IdDestino = 4;
                                        _compradetalleDto.i_IdCentroCostos = importaciondetallegastosDto.i_CCosto;
                                        _compradetalleDto.v_Glosa = string.Empty;
                                        _compradetalleDto.v_NroGuiaRemision = string.Empty;
                                        _compradetalleDto.d_ValorDolaresDetraccion = importaciondetallegastosDto.d_ValorDolaresDetraccionNoAfecto;
                                        _compradetalleDto.d_ValorSolesDetraccion = importaciondetallegastosDto.d_ValorSolesDetraccionNoAfecto;
                                        _compradetalleDto.v_NroPedido = string.Empty;
                                        _TempDetalle_AgregarCompraDto.Add(_compradetalleDto);
                                        _compraDto.i_IdDestino = 5;
                                        _compraDto.d_Anticipio = 0;
                                        _compraDto.d_ValorVenta = _TempDetalle_AgregarCompraDto.Sum(x => x.d_ValorVenta);
                                        _compraDto.d_IGV = _TempDetalle_AgregarCompraDto.Sum(y => y.d_Igv);
                                        _compraDto.d_Total = _TempDetalle_AgregarCompraDto.Sum(x => x.d_PrecioVenta);
                                        _compradetalleDto = new compradetalleDto();
                                    }
                                    else
                                    {
                                        _compraDto.i_IdDestino = importaciondetallegastosDto.d_ValorVenta != 0 ? 1 : 4;
                                        _compradetalleDto.v_NroCuenta = importaciondetallegastosDto.v_IdAsientoContable;
                                        _compradetalleDto.i_Anticipio = 0;
                                        _compradetalleDto.i_IdAlmacen = pobjDtoEntity.i_IdAlmacen;
                                        _compradetalleDto.v_IdProductoDetalle = null;
                                        _compradetalleDto.d_Cantidad = 0;
                                        _compradetalleDto.d_Precio = 0;
                                        _compradetalleDto.i_IdUnidadMedida = -1;
                                        _compradetalleDto.d_PrecioVenta = importaciondetallegastosDto.i_IdMoneda == 1 ? importaciondetallegastosDto.d_ImporteSoles : importaciondetallegastosDto.d_ImporteDolares;
                                        _compradetalleDto.d_ValorVenta = importaciondetallegastosDto.d_NAfectoDetraccion == 0 ? importaciondetallegastosDto.d_ValorVenta : importaciondetallegastosDto.d_NAfectoDetraccion;
                                        _compradetalleDto.d_Igv = importaciondetallegastosDto.d_ValorVenta != 0 ? importaciondetallegastosDto.d_Igv : 0;
                                        _compradetalleDto.i_IdDestino = _compraDto.i_IdDestino;
                                        _compradetalleDto.i_IdCentroCostos = importaciondetallegastosDto.i_CCosto;
                                        _compradetalleDto.v_Glosa = string.Empty;
                                        _compradetalleDto.v_NroGuiaRemision = string.Empty;

                                        _compradetalleDto.d_ValorDolaresDetraccion = _compraDto.i_IdDestino == 1 ? importaciondetallegastosDto.d_ValorDolaresDetraccion : importaciondetallegastosDto.d_ValorSolesDetraccionNoAfecto;
                                        _compradetalleDto.d_ValorSolesDetraccion = _compraDto.i_IdDestino == 1 ? importaciondetallegastosDto.d_ValorSolesDetraccion : importaciondetallegastosDto.d_ValorSolesDetraccionNoAfecto;
                                        _compradetalleDto.v_NroPedido = string.Empty;
                                        _TempDetalle_AgregarCompraDto.Add(_compradetalleDto);
                                        _compradetalleDto = new compradetalleDto();
                                        _compraDto.d_Anticipio = 0;
                                        _compraDto.d_ValorVenta = _TempDetalle_AgregarCompraDto.Sum(x => x.d_ValorVenta);
                                        _compraDto.d_IGV = _TempDetalle_AgregarCompraDto.Sum(y => y.d_Igv);
                                        _compraDto.d_Total = _TempDetalle_AgregarCompraDto.Sum(x => x.d_PrecioVenta);
                                    }
                                    _objComprasBL.InsertarCompra(ref objOperationResult, _compraDto, Globals.ClientSession.GetAsList(), _TempDetalle_AgregarCompraDto, null);
                                    if (objOperationResult.Success == 0) return string.Empty;
                                    _compraDto = new compraDto();
                                    _TempDetalle_AgregarCompraDto = new List<compradetalleDto>();
                                    dbContext.SaveChanges();
                                }
                            }
                        }
                        #endregion
                        #region Genera Libro Diario


                        if (pobjDtoEntity.i_IdEstado == 1)
                        {

                            //if (decimal.Parse(pobjDtoEntity.d_TotalValorFob.ToString()) > 0)
                            //{
                            DiarioBL _objDiarioBL = new DiarioBL();
                            diarioDto _diarioDto = new diarioDto();
                            List<KeyValueDTO> _ListadoDiarios = new List<KeyValueDTO>();
                            List<diariodetalleDto> TempDiarioInsertarImportaciones = new List<diariodetalleDto>();
                            diariodetalleDto D_Totales = new diariodetalleDto();
                            diariodetalleDto H_Totales = new diariodetalleDto();
                            OperationResult objOperationResult = new OperationResult();
                            var DetalleImportacionesFOB = (from d in dbContext.importaciondetallefob
                                                           where d.v_IdImportacion == newIdImportacion && d.i_Eliminado == 0
                                                           select d).ToList();

                            _ListadoDiarios = _objDiarioBL.ObtenerListadoDiario(ref objOperationResult, pobjDtoEntity.v_Periodo, pobjDtoEntity.v_Mes, (int)LibroDiarios.Importaciones);

                            int _MaxMovimiento;
                            _MaxMovimiento = _ListadoDiarios.Count() > 0 ? int.Parse(_ListadoDiarios[_ListadoDiarios.Count() - 1].Value1.ToString()) : 0;
                            _MaxMovimiento++;
                            _diarioDto.v_IdDocumentoReferencia = newIdImportacion;
                            _diarioDto.v_Periodo = pobjDtoEntity.v_Periodo;
                            _diarioDto.v_Mes = pobjDtoEntity.v_Mes;
                            _diarioDto.v_Nombre = "IMPORTACIÓN" + " " + pobjDtoEntity.v_Mes.Trim() + "-" + pobjDtoEntity.v_Correlativo.Trim();
                            _diarioDto.v_Glosa = "IMPORTACIÓN DE MERCADERÍA";
                            _diarioDto.v_Correlativo = _MaxMovimiento.ToString("00000000");
                            _diarioDto.d_TipoCambio = pobjDtoEntity.d_TipoCambio.Value;
                            _diarioDto.i_IdMoneda = pobjDtoEntity.i_IdMoneda.Value;
                            _diarioDto.i_IdTipoDocumento = (int)LibroDiarios.Importaciones;
                            _diarioDto.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                            _diarioDto.i_IdTipoComprobante = 2;

                            #region FOB
                            string ConceptoFob = ((int)Concepto.ValorFob).ToString();

                            var CuentaDetalleFob = (from n in dbContext.administracionconceptos

                                                    where n.v_Codigo == ConceptoFob && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)

                                                    select new { n.v_CuentaPVenta }).FirstOrDefault();

                            if (CuentaDetalleFob.v_CuentaPVenta != string.Empty & CuentaDetalleFob.v_CuentaPVenta != null)
                            {

                                foreach (var Proveedor in DetalleImportacionesFOB)
                                {

                                    var Cliente = (from n in dbContext.cliente
                                                   where n.v_IdCliente == Proveedor.v_IdCliente && n.i_Eliminado == 0 && n.v_FlagPantalla == "V"
                                                   select new { n.v_CodCliente }).FirstOrDefault();


                                    D_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(Proveedor.d_ValorFob.Value, 2);
                                    D_Totales.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(Proveedor.d_ValorFob.Value / Proveedor.d_TipoCambio.Value, 2) : Utils.Windows.DevuelveValorRedondeado(Proveedor.d_ValorFob.Value * Proveedor.d_TipoCambio.Value, 2);
                                    D_Totales.i_IdTipoDocumento = (int)LibroDiarios.Importaciones;
                                    D_Totales.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                    //D_Totales.v_CodAnexo = Cliente.v_CodCliente;
                                    D_Totales.v_IdCliente = Proveedor.v_IdCliente;
                                    //  D_Totales.v_Naturaleza = Proveedor.d_ValorFob > 0 ?   pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H" : pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D";
                                    D_Totales.v_Naturaleza = Proveedor.d_ValorFob > 0 ? !_objDocumentoBL.DocumentoEsInverso(int.Parse(pobjDtoEntity.i_IdTipoDocumento.Value.ToString())) ? "D" : "H" : !_objDocumentoBL.DocumentoEsInverso(int.Parse(pobjDtoEntity.i_IdTipoDocumento.Value.ToString())) ? "H" : "D";

                                    D_Totales.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                                    D_Totales.v_NroCuenta = CuentaDetalleFob.v_CuentaPVenta;
                                    D_Totales.v_Pedido = Proveedor.v_NroPedido;
                                    D_Totales.i_IdTipoDocumentoRef = -1;
                                    D_Totales.i_IdCentroCostos = "";
                                    TempDiarioInsertarImportaciones.Add(D_Totales);
                                    D_Totales = new diariodetalleDto();

                                }
                            }
                            #endregion

                            #region Flete
                            string ConceptoFlete = ((int)Concepto.Flete).ToString();
                            var CuentaDetalleFlete = (from n in dbContext.administracionconceptos

                                                      where n.v_Codigo == ConceptoFlete && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)

                                                      select new { n.v_CuentaPVenta }).FirstOrDefault();

                            if (pobjDtoEntity.d_Flete > 0 && CuentaDetalleFlete.v_CuentaPVenta != string.Empty && CuentaDetalleFlete.v_CuentaPVenta != null)
                            {
                                var Cliente = (from n in dbContext.cliente
                                               where n.v_IdCliente == pobjDtoEntity.v_IdClienteDoc1 && n.i_Eliminado == 0 && n.v_FlagPantalla == "V"
                                               select new { n.v_CodCliente }).FirstOrDefault();

                                D_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Flete.Value, 2);
                                D_Totales.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Flete.Value / pobjDtoEntity.d_TipoCambioDoc1.Value, 2) : Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Flete.Value * pobjDtoEntity.d_TipoCambioDoc1.Value, 2);
                                D_Totales.i_IdTipoDocumento = (int)LibroDiarios.Importaciones;
                                D_Totales.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                //D_Totales.v_CodAnexo = Cliente.v_CodCliente;
                                D_Totales.v_IdCliente = pobjDtoEntity.v_IdClienteDoc1;
                                //  D_Totales.v_Naturaleza = pobjDtoEntity.d_Flete > 0 ? pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H" : pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D";

                                D_Totales.v_Naturaleza = pobjDtoEntity.d_Flete > 0 ? !_objDocumentoBL.DocumentoEsInverso(int.Parse(pobjDtoEntity.i_IdTipoDocumento.Value.ToString())) ? "D" : "H" : !_objDocumentoBL.DocumentoEsInverso(int.Parse(pobjDtoEntity.i_IdTipoDocumento.Value.ToString())) ? "H" : "D";

                                D_Totales.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                                D_Totales.v_NroCuenta = CuentaDetalleFlete.v_CuentaPVenta;
                                D_Totales.i_IdTipoDocumentoRef = -1;
                                D_Totales.i_IdCentroCostos = "";
                                TempDiarioInsertarImportaciones.Add(D_Totales);
                                D_Totales = new diariodetalleDto();
                            }
                            #endregion

                            #region PagaSeguro
                            string ConceptoSeguro = ((int)Concepto.Seguro).ToString();
                            var CuentaDetalleSeguro = (from n in dbContext.administracionconceptos

                                                       where n.v_Codigo == ConceptoSeguro && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)

                                                       select new { n.v_CuentaPVenta }).FirstOrDefault();

                            if (pobjDtoEntity.i_PagaSeguro == 1 && CuentaDetalleSeguro.v_CuentaPVenta != string.Empty && CuentaDetalleSeguro.v_CuentaPVenta != null && decimal.Parse(pobjDtoEntity.d_PagoSeguro.ToString()) > 0)
                            {

                                var Cliente = (from n in dbContext.cliente
                                               where n.v_IdCliente == pobjDtoEntity.v_IdClienteDoc2 && n.i_Eliminado == 0 && n.v_FlagPantalla == "V"
                                               select new { n.v_CodCliente }).FirstOrDefault();

                                D_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_PagoSeguro.Value, 2);
                                D_Totales.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_PagoSeguro.Value / pobjDtoEntity.d_TipoCambioDoc2.Value, 2) : Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_PagoSeguro.Value * pobjDtoEntity.d_TipoCambioDoc2.Value, 2);
                                D_Totales.i_IdTipoDocumento = (int)LibroDiarios.Importaciones;
                                D_Totales.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                //D_Totales.v_CodAnexo = Cliente.v_CodCliente;
                                D_Totales.v_IdCliente = pobjDtoEntity.v_IdClienteDoc2;
                                // D_Totales.v_Naturaleza = pobjDtoEntity.d_PagoSeguro > 0 ? pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H" : pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D";

                                D_Totales.v_Naturaleza = pobjDtoEntity.d_PagoSeguro > 0 ? !_objDocumentoBL.DocumentoEsInverso(int.Parse(pobjDtoEntity.i_IdTipoDocumento.Value.ToString())) ? "D" : "H" : !_objDocumentoBL.DocumentoEsInverso(int.Parse(pobjDtoEntity.i_IdTipoDocumento.Value.ToString())) ? "H" : "D";

                                D_Totales.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                                D_Totales.v_NroCuenta = CuentaDetalleSeguro.v_CuentaPVenta;
                                D_Totales.i_IdCentroCostos = "";
                                D_Totales.i_IdTipoDocumentoRef = -1;
                                TempDiarioInsertarImportaciones.Add(D_Totales);
                                D_Totales = new diariodetalleDto();

                            }
                            #endregion

                            #region Advalorem
                            string ConceptoAdValorem = ((int)Concepto.AdValorem).ToString();
                            var CuentaDetalleAdValorem = (from n in dbContext.administracionconceptos
                                                          where n.v_Codigo == ConceptoAdValorem && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)
                                                          select new { n.v_CuentaPVenta }).FirstOrDefault();


                            if (pobjDtoEntity.i_AdValorem == 1 && CuentaDetalleAdValorem.v_CuentaPVenta != string.Empty && CuentaDetalleAdValorem.v_CuentaPVenta != null && decimal.Parse(pobjDtoEntity.d_AdValorem.ToString()) > 0)
                            {
                                var Cliente = (from n in dbContext.cliente
                                               where n.v_IdCliente == pobjDtoEntity.v_IdClienteDoc3 && n.i_Eliminado == 0 && n.v_FlagPantalla == "V"
                                               select new { n.v_CodCliente }).FirstOrDefault();

                                D_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_AdValorem.Value, 2);
                                D_Totales.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_AdValorem.Value / pobjDtoEntity.d_TipoCambioDoc3.Value, 2) : Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_AdValorem.Value * pobjDtoEntity.d_TipoCambioDoc3.Value, 2);
                                D_Totales.i_IdTipoDocumento = (int)LibroDiarios.Importaciones;
                                D_Totales.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                //D_Totales.v_CodAnexo = Cliente.v_CodCliente;
                                D_Totales.v_IdCliente = pobjDtoEntity.v_IdClienteDoc3;
                                //D_Totales.v_Naturaleza = pobjDtoEntity.d_AdValorem > 0 ? pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H" : pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D";

                                D_Totales.v_Naturaleza = pobjDtoEntity.d_AdValorem > 0 ? !_objDocumentoBL.DocumentoEsInverso(int.Parse(pobjDtoEntity.i_IdTipoDocumento.Value.ToString())) ? "D" : "H" : !_objDocumentoBL.DocumentoEsInverso(int.Parse(pobjDtoEntity.i_IdTipoDocumento.Value.ToString())) ? "H" : "D";

                                D_Totales.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                                D_Totales.v_NroCuenta = CuentaDetalleAdValorem.v_CuentaPVenta;
                                D_Totales.i_IdCentroCostos = "";
                                D_Totales.i_IdTipoDocumentoRef = -1;
                                TempDiarioInsertarImportaciones.Add(D_Totales);
                                D_Totales = new diariodetalleDto();


                            }
                            #endregion

                            #region Igv
                            string ConceptoIgv = ((int)Concepto.Igv).ToString();
                            var CuentaDetalleIgv = (from n in dbContext.administracionconceptos
                                                    where n.v_Codigo == ConceptoIgv && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)
                                                    select new { n.v_CuentaPVenta }).FirstOrDefault();


                            if (decimal.Parse(pobjDtoEntity.d_Igv.ToString()) > 0 && CuentaDetalleIgv.v_CuentaPVenta != null & CuentaDetalleIgv.v_CuentaPVenta != string.Empty)
                            {
                                var Cliente = (from n in dbContext.cliente
                                               where n.v_IdCliente == pobjDtoEntity.v_IdClienteDoc4 && n.i_Eliminado == 0 && n.v_FlagPantalla == "V"
                                               select new { n.v_CodCliente }).FirstOrDefault();

                                D_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Igv.Value, 2);
                                D_Totales.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Igv.Value / pobjDtoEntity.d_TipoCambioDoc4.Value, 2) : Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Igv.Value * pobjDtoEntity.d_TipoCambioDoc4.Value, 2);
                                D_Totales.i_IdTipoDocumento = (int)LibroDiarios.Importaciones;
                                D_Totales.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                //D_Totales.v_CodAnexo = Cliente.v_CodCliente;
                                D_Totales.v_IdCliente = pobjDtoEntity.v_IdClienteDoc4;
                                // D_Totales.v_Naturaleza = pobjDtoEntity.d_Igv > 0 ? pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H" : pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D";

                                D_Totales.v_Naturaleza = pobjDtoEntity.d_Igv > 0 ? !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "D" : "H" : !_objDocumentoBL.DocumentoEsInverso(int.Parse(pobjDtoEntity.i_IdTipoDocumento.Value.ToString())) ? "H" : "D";
                                D_Totales.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                                D_Totales.v_NroCuenta = CuentaDetalleIgv.v_CuentaPVenta;
                                D_Totales.i_IdCentroCostos = "";
                                D_Totales.i_IdTipoDocumentoRef = -1;
                                TempDiarioInsertarImportaciones.Add(D_Totales);
                                D_Totales = new diariodetalleDto();
                            }
                            #endregion

                            #region Percepcion
                            string ConceptoPercepcion = ((int)Concepto.Percepcion).ToString();

                            var CuentaDetallePercepcion = (from n in dbContext.administracionconceptos

                                                           where n.v_Codigo == ConceptoPercepcion && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)

                                                           select new { n.v_CuentaPVenta }).FirstOrDefault();



                            if (decimal.Parse(pobjDtoEntity.d_Percepcion.ToString()) > 0 && CuentaDetallePercepcion.v_CuentaPVenta != null && CuentaDetallePercepcion.v_CuentaPVenta != string.Empty)
                            {
                                var Cliente = (from n in dbContext.cliente
                                               where n.v_IdCliente == pobjDtoEntity.v_IdClienteDoc4 && n.i_Eliminado == 0 && n.v_FlagPantalla == "V"
                                               select new { n.v_CodCliente }).FirstOrDefault();

                                D_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Percepcion.Value, 2);
                                D_Totales.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Percepcion.Value / pobjDtoEntity.d_TipoCambioDoc4.Value, 2) : Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Percepcion.Value * pobjDtoEntity.d_TipoCambioDoc4.Value, 2);
                                D_Totales.i_IdTipoDocumento = (int)LibroDiarios.Importaciones;
                                D_Totales.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;

                                D_Totales.v_IdCliente = pobjDtoEntity.v_IdClienteDoc4;

                                D_Totales.v_Naturaleza = pobjDtoEntity.d_Percepcion > 0 ? !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "D" : "H" : !_objDocumentoBL.DocumentoEsInverso(int.Parse(pobjDtoEntity.i_IdTipoDocumento.Value.ToString())) ? "H" : "D";
                                D_Totales.v_NroDocumento = pobjDtoEntity.v_SerieDocumento4 + "-" + pobjDtoEntity.v_CorrelativoDoc4;
                                D_Totales.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                                D_Totales.v_NroCuenta = CuentaDetallePercepcion.v_CuentaPVenta;
                                D_Totales.i_IdCentroCostos = "";
                                D_Totales.i_IdTipoDocumentoRef = -1;
                                TempDiarioInsertarImportaciones.Add(D_Totales);
                                D_Totales = new diariodetalleDto();
                            }

                            #endregion

                            #region ProveedoresFob
                            string ConceptoProveedoresFob = ((int)Concepto.ProveedoresFob).ToString();
                            var CuentaDetalleProveedorFob = (from n in dbContext.administracionconceptos

                                                             where n.v_Codigo == ConceptoProveedoresFob && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)

                                                             select new { n.v_CuentaPVenta }).FirstOrDefault();

                            if (CuentaDetalleProveedorFob.v_CuentaPVenta != string.Empty & CuentaDetalleProveedorFob.v_CuentaPVenta != null)
                            {

                                foreach (var Proveedor in DetalleImportacionesFOB)
                                {

                                    var Cliente = (from n in dbContext.cliente
                                                   where n.v_IdCliente == Proveedor.v_IdCliente && n.i_Eliminado == 0 && n.v_FlagPantalla == "V"
                                                   select new { n.v_CodCliente }).FirstOrDefault();

                                    H_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(Proveedor.d_ValorFob.Value, 2);
                                    H_Totales.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(Proveedor.d_ValorFob.Value / Proveedor.d_TipoCambio.Value, 2) : Utils.Windows.DevuelveValorRedondeado(Proveedor.d_ValorFob.Value * Proveedor.d_TipoCambio.Value, 2);
                                    H_Totales.i_IdTipoDocumento = Proveedor.i_IdTipoDocumento.Value;
                                    H_Totales.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;

                                    H_Totales.v_IdCliente = Proveedor.v_IdCliente;

                                    H_Totales.v_Naturaleza = Proveedor.d_ValorFob > 0 ? !_objDocumentoBL.DocumentoEsInverso(int.Parse(pobjDtoEntity.i_IdTipoDocumento.Value.ToString())) ? "H" : "D" : !_objDocumentoBL.DocumentoEsInverso(int.Parse(pobjDtoEntity.i_IdTipoDocumento.Value.ToString())) ? "D" : "H";
                                    H_Totales.v_NroDocumento = Proveedor.v_SerieDocumento + "-" + Proveedor.v_CorrelativoDocumento;
                                    H_Totales.v_NroCuenta = CuentaDetalleProveedorFob.v_CuentaPVenta;
                                    H_Totales.v_Pedido = Proveedor.v_NroPedido;
                                    H_Totales.i_IdCentroCostos = "";
                                    D_Totales.i_IdTipoDocumentoRef = -1;
                                    TempDiarioInsertarImportaciones.Add(H_Totales);
                                    H_Totales = new diariodetalleDto();

                                }
                            #endregion

                                #region ProveedoresFlete
                                string ConceptoProveedoresFlete = ((int)Concepto.ProveedoresFlete).ToString();
                                var CuentaDetalleProveedoresFlete = (from n in dbContext.administracionconceptos

                                                                     where n.v_Codigo == ConceptoProveedoresFlete && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)

                                                                     select new { n.v_CuentaPVenta }).FirstOrDefault();

                                if (pobjDtoEntity.d_Flete > 0 && CuentaDetalleProveedoresFlete.v_CuentaPVenta != string.Empty && CuentaDetalleProveedoresFlete.v_CuentaPVenta != null)
                                {

                                    var Cliente = (from n in dbContext.cliente
                                                   where n.i_Eliminado == 0 && n.v_FlagPantalla == "V" && n.v_IdCliente == pobjDtoEntity.v_IdClienteDoc1
                                                   select new { n.v_CodCliente }).FirstOrDefault();


                                    H_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Flete.Value, 2);
                                    H_Totales.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Flete.Value / pobjDtoEntity.d_TipoCambioDoc1.Value, 2) : Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Flete.Value * pobjDtoEntity.d_TipoCambioDoc1.Value, 2);
                                    H_Totales.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento1.Value;
                                    H_Totales.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                    H_Totales.v_IdCliente = pobjDtoEntity.v_IdClienteDoc1;
                                    H_Totales.v_Naturaleza = pobjDtoEntity.d_Flete > 0 ? !_objDocumentoBL.DocumentoEsInverso(int.Parse(pobjDtoEntity.i_IdTipoDocumento.Value.ToString())) ? "H" : "D" : !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "D" : "H";
                                    H_Totales.v_NroDocumento = pobjDtoEntity.v_SerieDocumento1 + "-" + pobjDtoEntity.v_CorrelativoDocumento1;
                                    H_Totales.v_NroCuenta = CuentaDetalleProveedoresFlete.v_CuentaPVenta;
                                    H_Totales.i_IdCentroCostos = "";
                                    D_Totales.i_IdTipoDocumentoRef = -1;
                                    TempDiarioInsertarImportaciones.Add(H_Totales);
                                    H_Totales = new diariodetalleDto();
                                }

                                #endregion

                                #region ProveedoresSeguro



                                string ConceptoProveedoresSeguro = ((int)Concepto.ProveedoresSeguro).ToString();
                                var CuentaDetalleProveedorSeguro = (from n in dbContext.administracionconceptos

                                                                    where n.v_Codigo == ConceptoProveedoresSeguro && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)

                                                                    select new { n.v_CuentaPVenta }).FirstOrDefault();



                                if (pobjDtoEntity.i_PagaSeguro == 1 && decimal.Parse(pobjDtoEntity.d_PagoSeguro.ToString()) > 0 && CuentaDetalleProveedorSeguro.v_CuentaPVenta != string.Empty && CuentaDetalleProveedorSeguro.v_CuentaPVenta != null)
                                {

                                    var Cliente = (from n in dbContext.cliente
                                                   where n.i_Eliminado == 0 && n.v_FlagPantalla == "V" && n.v_IdCliente == pobjDtoEntity.v_IdClienteDoc2
                                                   select new { n.v_CodCliente }).FirstOrDefault();

                                    H_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_PagoSeguro.Value, 2);
                                    H_Totales.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_PagoSeguro.Value / pobjDtoEntity.d_TipoCambioDoc2.Value, 2) : Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_PagoSeguro.Value * pobjDtoEntity.d_TipoCambioDoc2.Value, 2);
                                    H_Totales.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento2.Value;
                                    H_Totales.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                    H_Totales.v_IdCliente = pobjDtoEntity.v_IdClienteDoc2;
                                    H_Totales.v_Naturaleza = pobjDtoEntity.d_PagoSeguro > 0 ? !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "H" : "D" : !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "D" : "H";
                                    H_Totales.v_NroDocumento = pobjDtoEntity.v_SerieDocumento2 + "-" + pobjDtoEntity.v_CorrelativoDocumento2;
                                    H_Totales.v_NroCuenta = CuentaDetalleProveedorSeguro.v_CuentaPVenta;
                                    H_Totales.i_IdCentroCostos = "";
                                    D_Totales.i_IdTipoDocumentoRef = -1;
                                    TempDiarioInsertarImportaciones.Add(H_Totales);
                                    H_Totales = new diariodetalleDto();

                                }
                                #endregion

                                #region ProveedorAdvalorem

                                string ConceptoProveedoresAdvalorem = ((int)Concepto.ProveedoresAdvalorem).ToString();
                                var CuentaDetalleProveedorAdvalorem = (from n in dbContext.administracionconceptos

                                                                       where n.v_Codigo == ConceptoProveedoresAdvalorem && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)

                                                                       select new { n.v_CuentaPVenta }).FirstOrDefault();


                                if (pobjDtoEntity.i_AdValorem == 1 && decimal.Parse(pobjDtoEntity.d_AdValorem.ToString()) > 0 && CuentaDetalleProveedorAdvalorem.v_CuentaPVenta != string.Empty && CuentaDetalleProveedorAdvalorem.v_CuentaPVenta != null)
                                {
                                    var Cliente = (from n in dbContext.cliente
                                                   where n.i_Eliminado == 0 && n.v_FlagPantalla == "V" && n.v_IdCliente == pobjDtoEntity.v_IdClienteDoc3
                                                   select new { n.v_CodCliente }).FirstOrDefault();

                                    H_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_AdValorem.Value, 2);
                                    H_Totales.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_AdValorem.Value / pobjDtoEntity.d_TipoCambioDoc3.Value, 2) : Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_AdValorem.Value * pobjDtoEntity.d_TipoCambioDoc3.Value, 2);
                                    H_Totales.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento3.Value;
                                    H_Totales.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                    H_Totales.v_IdCliente = pobjDtoEntity.v_IdClienteDoc3;
                                    H_Totales.v_Naturaleza = pobjDtoEntity.d_AdValorem > 0 ? !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "H" : "D" : !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "D" : "H";
                                    H_Totales.v_NroDocumento = pobjDtoEntity.v_SerieDocumento3 + "-" + pobjDtoEntity.v_CorrelativoDocumento3;
                                    H_Totales.v_NroCuenta = CuentaDetalleProveedorAdvalorem.v_CuentaPVenta;
                                    H_Totales.i_IdCentroCostos = "";
                                    D_Totales.i_IdTipoDocumentoRef = -1;
                                    TempDiarioInsertarImportaciones.Add(H_Totales);
                                    H_Totales = new diariodetalleDto();

                                }

                                #endregion

                                #region Proveedor IGV

                                string ConceptoProveedoresIgv = ((int)Concepto.ProveedorIgv).ToString();
                                var CuentaDetalleProveedorIgv = (from n in dbContext.administracionconceptos

                                                                 where n.v_Codigo == ConceptoProveedoresIgv && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)

                                                                 select new { n.v_CuentaPVenta }).FirstOrDefault();



                                if (decimal.Parse(pobjDtoEntity.d_Igv.ToString()) > 0 && CuentaDetalleProveedorIgv.v_CuentaPVenta != string.Empty && CuentaDetalleProveedorIgv.v_CuentaPVenta != null)
                                {

                                    var Cliente = (from n in dbContext.cliente
                                                   where n.i_Eliminado == 0 && n.v_FlagPantalla == "V" && n.v_IdCliente == pobjDtoEntity.v_IdClienteDoc4
                                                   select new { n.v_CodCliente }).FirstOrDefault();

                                    H_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Igv.Value, 2);
                                    H_Totales.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Igv.Value / pobjDtoEntity.d_TipoCambioDoc4.Value, 2) : Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Igv.Value * pobjDtoEntity.d_TipoCambioDoc4.Value, 2);
                                    H_Totales.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento4.Value;
                                    H_Totales.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                    H_Totales.v_IdCliente = pobjDtoEntity.v_IdClienteDoc4;
                                    H_Totales.v_Naturaleza = pobjDtoEntity.d_Igv > 0 ? !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "H" : "D" : !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "D" : "H";
                                    H_Totales.v_NroDocumento = pobjDtoEntity.v_SerieDocumento4 + "-" + pobjDtoEntity.v_CorrelativoDoc4;
                                    H_Totales.v_NroCuenta = CuentaDetalleProveedorIgv.v_CuentaPVenta;
                                    H_Totales.i_IdCentroCostos = "";
                                    D_Totales.i_IdTipoDocumentoRef = -1;
                                    TempDiarioInsertarImportaciones.Add(H_Totales);
                                    H_Totales = new diariodetalleDto();
                                }
                                #endregion ProveedorIgv

                                #region Proveedor Percepcion
                                string ConceptoProveedoresPercepcion = ((int)Concepto.ProveedorPercepcion).ToString();

                                var CuentaDetalleProveedoresPercepcion = (from n in dbContext.administracionconceptos

                                                                          where n.v_Codigo == ConceptoProveedoresPercepcion && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)

                                                                          select new { n.v_CuentaPVenta }).FirstOrDefault();



                                if (decimal.Parse(pobjDtoEntity.d_Percepcion.ToString()) > 0 && CuentaDetalleProveedoresPercepcion.v_CuentaPVenta != string.Empty && CuentaDetalleProveedoresPercepcion.v_CuentaPVenta != null)
                                {

                                    var Cliente = (from n in dbContext.cliente
                                                   where n.i_Eliminado == 0 && n.v_FlagPantalla == "V" && n.v_IdCliente == pobjDtoEntity.v_IdClienteDoc4
                                                   select new { n.v_CodCliente }).FirstOrDefault();

                                    H_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Percepcion.Value, 2);
                                    H_Totales.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Percepcion.Value / pobjDtoEntity.d_TipoCambioDoc4.Value, 2) : Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Percepcion.Value * pobjDtoEntity.d_TipoCambioDoc4.Value, 2);
                                    H_Totales.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento4.Value;
                                    H_Totales.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                    H_Totales.v_IdCliente = pobjDtoEntity.v_IdClienteDoc4;
                                    H_Totales.v_Naturaleza = pobjDtoEntity.d_Percepcion > 0 ? !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "H" : "D" : !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "D" : "H";
                                    // H_Totales.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                                    H_Totales.v_NroDocumento = pobjDtoEntity.v_SerieDocumento4 + "-" + pobjDtoEntity.v_CorrelativoDoc4;
                                    H_Totales.v_NroCuenta = CuentaDetalleProveedoresPercepcion.v_CuentaPVenta;
                                    H_Totales.i_IdCentroCostos = "";
                                    D_Totales.i_IdTipoDocumentoRef = -1;
                                    TempDiarioInsertarImportaciones.Add(H_Totales);
                                    H_Totales = new diariodetalleDto();
                                }

                                #endregion

                            }



                            decimal TotDebe, TotHaber, TotDebeC, TotHaberC;
                            TotDebe = TempDiarioInsertarImportaciones.Where(p => p.v_Naturaleza == "D" && p.v_NroCuenta != string.Empty).Sum(o => o.d_Importe.Value);
                            TotHaber = TempDiarioInsertarImportaciones.Where(p => p.v_Naturaleza == "H" && p.v_NroCuenta != string.Empty).Sum(o => o.d_Importe.Value);
                            TotDebeC = TempDiarioInsertarImportaciones.Where(p => p.v_Naturaleza == "D" && p.v_NroCuenta != string.Empty).Sum(o => o.d_Cambio.Value);
                            TotHaberC = TempDiarioInsertarImportaciones.Where(p => p.v_Naturaleza == "H" && p.v_NroCuenta != string.Empty).Sum(o => o.d_Cambio.Value);
                            _diarioDto.d_TotalDebe = Utils.Windows.DevuelveValorRedondeado(TotDebe, 2);
                            _diarioDto.d_TotalHaber = Utils.Windows.DevuelveValorRedondeado(TotHaber, 2);
                            _diarioDto.d_TotalDebeCambio = Utils.Windows.DevuelveValorRedondeado(TotDebeC, 2);
                            _diarioDto.d_TotalHaberCambio = Utils.Windows.DevuelveValorRedondeado(TotHaberC, 2);
                            _diarioDto.d_DiferenciaDebe = Utils.Windows.DevuelveValorRedondeado(TotDebe - TotHaber, 2);
                            _diarioDto.d_DiferenciaHaber = Utils.Windows.DevuelveValorRedondeado(TotDebeC - TotHaberC, 2);

                            _objDiarioBL.InsertarDiario(ref objOperationResult, _diarioDto, Globals.ClientSession.GetAsList(), TempDiarioInsertarImportaciones.Where(x => x.v_NroCuenta != String.Empty).ToList(), (int)TipoMovimientoTesoreria.Egreso);
                            if (objOperationResult.Success == 0) return string.Empty;

                            //}
                        }
                        #endregion

                        #region Actualiza OrdenCompra

                        if (pobjDtoEntity.v_IdDocumentoReferencia != null)
                        {


                            var oc = (from a in dbContext.ordendecompra
                                      where a.v_IdOrdenCompra == pobjDtoEntity.v_IdDocumentoReferencia
                                      select a).FirstOrDefault();

                            if (oc != null)
                            {

                                oc.i_IdEstado = 2;
                                dbContext.ordendecompra.ApplyCurrentValues(oc);
                            }

                        }
                        #endregion
                        dbContext.SaveChanges();
                        pobjOperationResult.Success = 1;
                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "importacion", pobjDtoEntity.v_IdImportacion);
                        ts.Complete();
                        return newIdImportacion;
                    }
                }

            }
            catch (Exception ex)
            {


                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ImportacionBL.InsertarImportacion()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;

            }

        }

        public bool ValidarNroCuentaGeneracionLibro(string Codigo)
        {
            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
            var AdministracionConcepto = (from n in dbContext.administracionconceptos

                                          where n.v_Codigo == Codigo && n.i_Eliminado == 0 && n.v_CuentaPVenta != null && n.v_CuentaPVenta != string.Empty && n.v_Periodo.Equals(periodo)
                                          select n).FirstOrDefault();


            if (AdministracionConcepto != null)
            {

                var PlanCuenta = (from a in dbContext.asientocontable
                                  where a.v_NroCuenta == AdministracionConcepto.v_CuentaPVenta && a.i_Imputable == 1 && a.i_Eliminado == 0 && a.v_Periodo == periodo
                                  select a).FirstOrDefault();
                if (PlanCuenta != null)
                {
                    return true;

                }
                else
                {
                    return false;
                }

            }
            else
            {

                return false;
            }

        }
        public bool ExistenciaDocumentoenCompras(int iTipoDoc, string pstrSerieDoc, string pstrCorrelativoDoc, string pstrIdProveedor, string DocumentoReferencia)
        {
            //SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
            //compra Compras = new compra();
            //if (string.IsNullOrEmpty(DocumentoReferencia))
            //{
            //    Compras = (from n in dbContext.compra
            //               where n.v_SerieDocumento == pstrSerieDoc && n.v_CorrelativoDocumento == pstrCorrelativoDoc && n.i_IdTipoDocumento == iTipoDoc && n.v_IdProveedor == pstrIdProveedor && n.i_Eliminado == 0
            //               select n).FirstOrDefault();
            //}
            //else
            //{
            //    Compras = (from n in dbContext.compra
            //               where n.v_SerieDocumento == pstrSerieDoc && n.v_CorrelativoDocumento == pstrCorrelativoDoc && n.i_IdTipoDocumento == iTipoDoc && n.v_IdProveedor == pstrIdProveedor && n.i_Eliminado == 0 && n.v_IdDocumentoReferencia == DocumentoReferencia
            //               select n).FirstOrDefault();
            //}
            //if (Compras != null)
            //    return true;
            //else
            //    return false;

            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
            compra Compras = new compra();
            if (string.IsNullOrEmpty(DocumentoReferencia))
            {
                Compras = (from n in dbContext.compra
                           where n.v_SerieDocumento == pstrSerieDoc && n.v_CorrelativoDocumento == pstrCorrelativoDoc && n.i_IdTipoDocumento == iTipoDoc && n.v_IdProveedor == pstrIdProveedor && n.i_Eliminado == 0
                           select n).FirstOrDefault();
                if (Compras != null)
                    return true;
                else
                    return false;
            }
            else
            {
                Compras = (from n in dbContext.compra
                           where n.i_Eliminado == 0 && n.v_IdDocumentoReferencia == DocumentoReferencia
                           select n).FirstOrDefault();
                if (Compras != null)
                {

                    var ExisteDocumento = (from n in dbContext.compra
                                           where n.v_SerieDocumento == pstrSerieDoc && n.v_CorrelativoDocumento == pstrCorrelativoDoc && n.i_IdTipoDocumento == iTipoDoc && n.v_IdProveedor == pstrIdProveedor && n.i_Eliminado == 0
                                           select n).FirstOrDefault();
                    if (ExisteDocumento != null && ExisteDocumento.v_IdDocumentoReferencia != DocumentoReferencia)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    var ExisteDocumento = (from n in dbContext.compra
                                           where n.v_SerieDocumento == pstrSerieDoc && n.v_CorrelativoDocumento == pstrCorrelativoDoc && n.i_IdTipoDocumento == iTipoDoc && n.v_IdProveedor == pstrIdProveedor && n.i_Eliminado == 0
                                           select n).FirstOrDefault();

                    if (ExisteDocumento == null)
                    {

                        return true;
                    }
                    else
                    {

                        return false;
                    }
                }
            }




        }


        public string ActualizarImportacion(ref OperationResult pobjOperationResult, importacionDto pobjDtoEntity, List<string> ClientSession, List<importaciondetallefobDto> pTemp_InsertarDetalleFob, List<importaciondetallefobDto> pTemp_EditarDetalleFob, List<importaciondetallefobDto> pTemp_EliminarDetalleFob, List<importaciondetalleproductoDto> pTemp_AgregarDetalleProd, List<importaciondetalleproductoDto> pTemp_EditarDetalleProd, List<importaciondetalleproductoDto> pTemp_EliminarDetalleProd, List<importaciondetallegastosDto> pTemp_AgregarDetalleGastos, List<importaciondetallegastosDto> pTemp_ModificarDetalleGastos, List<importaciondetallegastosDto> pTemp_EliminarDetalleGastos)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    SecuentialBL objSecuentialBL = new SecuentialBL();
                    importaciondetallefobDto pobjImportacionDetalleFobDto = new importaciondetallefobDto();
                    importaciondetalleproductoDto pobjImportacionDetalleProdDto = new importaciondetalleproductoDto();
                    ComprasBL _ComprasBL = new ComprasBL();
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                    int SecuentialId = 0;
                    string newIdImportacionDetalleFob = string.Empty;
                    string newIdImportacionDetalleProd = string.Empty;
                    string newIdImportacionDetalleGastos = string.Empty;
                    int intNodeId;
                    List<string> ListaProveedores = new List<string>();
                    List<KeyValueDTO> _ListadoCompras = new List<KeyValueDTO>();
                    ComprasBL _objComprasBL = new ComprasBL();
                    compraDto _compraDto = new compraDto();
                    compradetalleDto _compradetalleDto = new compradetalleDto();
                    List<compradetalleDto> _TempDetalle_AgregarCompraDto = new List<compradetalleDto>();


                    var x = (from n in dbContext.importaciondetalleproducto
                             where n.v_IdImportacion == pobjDtoEntity.v_IdImportacion
                             select new
                             {
                                 n.v_IdCliente
                             }
                                        );
                    ListaProveedores = x.Select(p => p.v_IdCliente).ToList();

                    #region Actualiza Cabecera

                    intNodeId = int.Parse(ClientSession[0]);
                    var objEntity = (from a in dbContext.importacion
                                     where a.v_IdImportacion == pobjDtoEntity.v_IdImportacion
                                     select a).FirstOrDefault();
                    pobjDtoEntity.i_Eliminado = 0;
                    pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                    pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                    objEntity = importacionAssembler.ToEntity(pobjDtoEntity);

                    dbContext.importacion.ApplyCurrentValues(objEntity);
                    #endregion

                    #region Actualiza ImportacionDetalleFob

                    foreach (importaciondetallefobDto importaciondetalleFobDto in pTemp_InsertarDetalleFob)
                    {
                        importaciondetallefob objEntityImportacionDetalleFob = importaciondetallefobAssembler.ToEntity(importaciondetalleFobDto);
                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 52);
                        newIdImportacionDetalleFob = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "XB");
                        objEntityImportacionDetalleFob.v_IdImportacionDetalleFob = newIdImportacionDetalleFob;
                        objEntityImportacionDetalleFob.t_InsertaFecha = DateTime.Now;
                        objEntityImportacionDetalleFob.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntityImportacionDetalleFob.i_Eliminado = 0;
                        dbContext.AddToimportaciondetallefob(objEntityImportacionDetalleFob);
                        dbContext.SaveChanges();
                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "importacionDetalleFob", newIdImportacionDetalleFob);

                    }


                    foreach (importaciondetallefobDto importaciondetalleFobDto in pTemp_EditarDetalleFob)
                    {
                        importaciondetallefob objEntityImportacionDetalleFob = importaciondetallefobAssembler.ToEntity(importaciondetalleFobDto);

                        var query = (from n in dbContext.importaciondetallefob
                                     where n.v_IdImportacionDetalleFob == importaciondetalleFobDto.v_IdImportacionDetalleFob
                                     select n).FirstOrDefault();

                        objEntityImportacionDetalleFob.t_ActualizaFecha = DateTime.Now;
                        objEntityImportacionDetalleFob.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        dbContext.importaciondetallefob.ApplyCurrentValues(objEntityImportacionDetalleFob);
                        dbContext.SaveChanges();
                        Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "importacionDetalleFob", query.v_IdImportacionDetalleFob);
                    }

                    foreach (importaciondetallefobDto importaciondetalleFobDto in pTemp_EliminarDetalleFob)
                    {
                        importaciondetallefob objEntityImportacionDetalleFob = importaciondetallefobAssembler.ToEntity(importaciondetalleFobDto);

                        var query = (from n in dbContext.importaciondetallefob
                                     where n.v_IdImportacionDetalleFob == importaciondetalleFobDto.v_IdImportacionDetalleFob
                                     select n).FirstOrDefault();

                        if (query != null)
                        {
                            query.t_ActualizaFecha = DateTime.Now;
                            query.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            query.i_Eliminado = 1;
                        }

                        dbContext.importaciondetallefob.ApplyCurrentValues(query);
                        dbContext.SaveChanges();
                        Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "importacionDetalleFob", query.v_IdImportacionDetalleFob);
                    }
                    #endregion

                    #region Actualiza ImportacionDetalleProducto

                    foreach (importaciondetalleproductoDto importacionDetalleProdDto in pTemp_AgregarDetalleProd)
                    {
                        movimientodetalleDto NotadeIngresoDetalle = new movimientodetalleDto();
                        if (Globals.ClientSession.i_IncluirPedidoExportacionCompraVenta == 1)
                        {

                            NotadeIngresoDetalle = (from n in dbContext.movimientodetalle

                                                    join J1 in dbContext.movimiento on new { idMovimiento = n.v_IdMovimiento }
                                                                                    equals new { idMovimiento = J1.v_IdMovimiento } into J1_join
                                                    from J1 in J1_join.DefaultIfEmpty()

                                                    where n.i_Eliminado == 0 && n.v_IdProductoDetalle == importacionDetalleProdDto.v_IdProductoDetalle && J1.v_OrigenTipo == "I" && J1.v_OrigenRegPeriodo == pobjDtoEntity.v_Periodo
                                                          && J1.v_OrigenRegMes == pobjDtoEntity.v_Mes && J1.v_OrigenRegCorrelativo == pobjDtoEntity.v_Correlativo
                                                          && n.v_NroPedido == importacionDetalleProdDto.v_NroPedido
                                                    select new movimientodetalleDto { v_IdMovimientoDetalle = n.v_IdMovimientoDetalle }).FirstOrDefault();
                        }
                        else
                        {

                            NotadeIngresoDetalle = (from n in dbContext.movimientodetalle

                                                    join J1 in dbContext.movimiento on new { idMovimiento = n.v_IdMovimiento }
                                                                                    equals new { idMovimiento = J1.v_IdMovimiento } into J1_join
                                                    from J1 in J1_join.DefaultIfEmpty()

                                                    where n.i_Eliminado == 0 && n.v_IdProductoDetalle == importacionDetalleProdDto.v_IdProductoDetalle && J1.v_OrigenTipo == "I" && J1.v_OrigenRegPeriodo == pobjDtoEntity.v_Periodo
                                                          && J1.v_OrigenRegMes == pobjDtoEntity.v_Mes && J1.v_OrigenRegCorrelativo == pobjDtoEntity.v_Correlativo

                                                    select new movimientodetalleDto { v_IdMovimientoDetalle = n.v_IdMovimientoDetalle }).FirstOrDefault();

                        }



                        importaciondetalleproducto objEntityImportacionDetalleProd = importaciondetalleproductoAssembler.ToEntity(importacionDetalleProdDto);
                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 54);
                        newIdImportacionDetalleProd = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "XD");
                        objEntityImportacionDetalleProd.v_IdImportacionDetalleProducto = newIdImportacionDetalleProd;
                        objEntityImportacionDetalleProd.t_InsertaFecha = DateTime.Now;
                        objEntityImportacionDetalleProd.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntityImportacionDetalleProd.v_IdMovimientoDetalle = NotadeIngresoDetalle.v_IdMovimientoDetalle;
                        objEntityImportacionDetalleProd.i_Eliminado = 0;
                        dbContext.AddToimportaciondetalleproducto(objEntityImportacionDetalleProd);
                        dbContext.SaveChanges();
                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "importacionDetalleProducto", newIdImportacionDetalleProd);


                    }

                    foreach (importaciondetalleproductoDto importacionDetalleProdDto in pTemp_EditarDetalleProd)
                    {
                        importaciondetalleproducto objEntityImportacionDetalleProd = importaciondetalleproductoAssembler.ToEntity(importacionDetalleProdDto);

                        var query = (from n in dbContext.importaciondetalleproducto
                                     where n.v_IdImportacionDetalleProducto == importacionDetalleProdDto.v_IdImportacionDetalleProducto
                                     select n).FirstOrDefault();

                        objEntityImportacionDetalleProd.t_ActualizaFecha = DateTime.Now;
                        objEntityImportacionDetalleProd.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        dbContext.importaciondetalleproducto.ApplyCurrentValues(objEntityImportacionDetalleProd);
                        dbContext.SaveChanges();
                        Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "importacionDetalleProducto", query.v_IdImportacionDetalleProducto);
                    }

                    foreach (importaciondetalleproductoDto importaciondetalleProdDto in pTemp_EliminarDetalleProd)
                    {

                        importaciondetalleproducto objEntityImportacionDetalleProd = importaciondetalleproductoAssembler.ToEntity(importaciondetalleProdDto);

                        var query = (from n in dbContext.importaciondetalleproducto
                                     where n.v_IdImportacionDetalleProducto == importaciondetalleProdDto.v_IdImportacionDetalleProducto
                                     select n).FirstOrDefault();

                        if (query != null)
                        {
                            query.t_ActualizaFecha = DateTime.Now;
                            query.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            query.i_Eliminado = 1;
                        }

                        dbContext.importaciondetalleproducto.ApplyCurrentValues(query);
                        dbContext.SaveChanges();
                        Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "importacionDetalleProducto", query.v_IdImportacionDetalleProducto);
                    }

                    #endregion

                    #region Inserta ImportacionDetalleGastos

                    foreach (importaciondetallegastosDto importacionDetalleGastosDto in pTemp_AgregarDetalleGastos)
                    {
                        importaciondetallegastos objEntityImportacionDetalleGastos = importaciondetallegastosAssembler.ToEntity(importacionDetalleGastosDto);
                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 53);
                        newIdImportacionDetalleGastos = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "XC");
                        objEntityImportacionDetalleGastos.v_IdImportacionDetalleGastos = newIdImportacionDetalleGastos;
                        objEntityImportacionDetalleGastos.t_InsertaFecha = DateTime.Now;
                        objEntityImportacionDetalleGastos.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntityImportacionDetalleGastos.i_Eliminado = 0;
                        dbContext.AddToimportaciondetallegastos(objEntityImportacionDetalleGastos);

                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "importacionDetalleGastos", newIdImportacionDetalleGastos);

                        if (pobjDtoEntity.i_IdEstado == 1)
                        {

                            if (!ExistenciaDocumentoenCompras(importacionDetalleGastosDto.i_IdTipoDocumento.Value, importacionDetalleGastosDto.v_SerieDocumento, importacionDetalleGastosDto.v_CorrelativoDocumento, importacionDetalleGastosDto.v_Detalle, importacionDetalleGastosDto.v_IdImportacionDetalleGastos))
                            {
                                string Mes = string.Empty;
                                string Periodo = "";
                                if (Globals.ClientSession.i_FechaEmision.Value.ToString() == "1") //Fecha Registro Otros Gastos
                                {
                                    //Mes = int.Parse(importacionDetalleGastosDto.t_FechaEmision.Value.Month.ToString()) <= 9 ? ("0" + importacionDetalleGastosDto.t_FechaEmision.Value.Month.ToString()).Trim() : importacionDetalleGastosDto.t_FechaEmision.Value.Month.ToString();
                                    Mes = importacionDetalleGastosDto.t_FechaRegistro.Value.Month.ToString("00").Trim();
                                    Periodo = importacionDetalleGastosDto.t_FechaRegistro.Value.Year.ToString();
                                    _ListadoCompras = _objComprasBL.ObtenerListadoCompras(ref pobjOperationResult, importacionDetalleGastosDto.t_FechaRegistro.Value.Year.ToString(), Mes);
                                    _compraDto.t_FechaEmision = importacionDetalleGastosDto.t_FechaEmision;
                                    _compraDto.t_FechaRegistro = importacionDetalleGastosDto.t_FechaRegistro;
                                    _compraDto.t_FechaVencimiento = importacionDetalleGastosDto.t_FechaEmision;

                                }
                                else //FechaRegistro Importacion
                                {
                                    //  Mes = int.Parse(pobjDtoEntity.t_FechaRegistro.Value.Month.ToString()) <= 9 ? ("0" + pobjDtoEntity.t_FechaRegistro.Value.Month.ToString()).Trim() : pobjDtoEntity.t_FechaRegistro.Value.Month.ToString();
                                    Mes = pobjDtoEntity.t_FechaRegistro.Value.Month.ToString("00").Trim();
                                    Periodo = pobjDtoEntity.t_FechaRegistro.Value.Year.ToString();
                                    _ListadoCompras = _objComprasBL.ObtenerListadoCompras(ref pobjOperationResult, pobjDtoEntity.t_FechaRegistro.Value.Year.ToString(), Mes);
                                    _compraDto.t_FechaEmision = importacionDetalleGastosDto.t_FechaEmision;
                                    _compraDto.t_FechaRegistro = objEntity.t_FechaRegistro;
                                    _compraDto.t_FechaVencimiento = importacionDetalleGastosDto.t_FechaEmision;
                                }

                                int _MaxMovimiento;
                                _MaxMovimiento = _ListadoCompras.Count() > 0 ? int.Parse(_ListadoCompras[_ListadoCompras.Count() - 1].Value1.ToString()) : 0;
                                _MaxMovimiento++;
                                _compraDto.v_Periodo = Periodo;
                                _compraDto.v_Mes = Mes;
                                _compraDto.v_Correlativo = _MaxMovimiento.ToString("00000000");
                                _compraDto.v_IdDocumentoReferencia = newIdImportacionDetalleGastos;
                                _compraDto.i_IdIgv = Globals.ClientSession.i_IdIgv;
                                _compraDto.i_IdTipoDocumento = importacionDetalleGastosDto.i_IdTipoDocumento;
                                _compraDto.v_SerieDocumento = importacionDetalleGastosDto.v_SerieDocumento;
                                _compraDto.v_CorrelativoDocumento = importacionDetalleGastosDto.v_CorrelativoDocumento;

                                _compraDto.i_IdCondicionPago = 1; //Por Confirmar

                                _compraDto.t_FechaRef = importacionDetalleGastosDto.t_FechaEmision;
                                _compraDto.i_IdEstado = 1;
                                _compraDto.i_IdTipoDocumentoRef = importacionDetalleGastosDto.i_IdTipoDocRef;
                                _compraDto.v_CorrelativoDocumentoRef = importacionDetalleGastosDto.v_CorrelativoDocRef;
                                _compraDto.v_SerieDocumentoRef = importacionDetalleGastosDto.v_SerieDocRef;
                                _compraDto.v_IdProveedor = importacionDetalleGastosDto.v_Detalle;
                                _compraDto.v_GuiaRemisionSerie = string.Empty;
                                _compraDto.v_GuiaRemisionCorrelativo = string.Empty;
                                _compraDto.d_TipoCambio = importacionDetalleGastosDto.d_TipoCambio;
                                _compraDto.v_Glosa = importacionDetalleGastosDto.v_Glosa;
                                _compraDto.i_IdMoneda = importacionDetalleGastosDto.i_IdMoneda;
                                _compraDto.i_IdTipoCompra = importacionDetalleGastosDto.i_IdMoneda == 1 ? (int)TipoCompra.MonedaNacional : (int)TipoCompra.MonedaExtranjera;
                                _compraDto.i_EsAfectoIgv = importacionDetalleGastosDto.d_Igv == 0 ? 1 : 1;
                                _compraDto.i_IdEstablecimiento = pobjDtoEntity.i_IdEstablecimiento;

                                _compraDto.i_EsDetraccion = importacionDetalleGastosDto.i_EsDetraccion;
                                _compraDto.i_CodigoDetraccion = importacionDetalleGastosDto.i_CodigoDetraccion;
                                _compraDto.v_NroDetraccion = importacionDetalleGastosDto.v_NroDetraccion;
                                _compraDto.d_PorcentajeDetraccion = importacionDetalleGastosDto.d_PorcentajeDetraccion;
                                _compraDto.t_FechaDetraccion = importacionDetalleGastosDto.t_FechaDetraccion;

                                _compraDto.d_PorcentajeDetraccion = 0;
                                _compraDto.i_PreciosIncluyenIgv = 0;
                                _compraDto.i_DeduccionAnticipio = 0;

                                var Proveedor = (from n in dbContext.cliente
                                                 where n.i_Eliminado == 0 && n.v_FlagPantalla == "V" & n.v_IdCliente == importacionDetalleGastosDto.v_Detalle
                                                 select n).FirstOrDefault();
                                _compraDto.NombreProveedor = (Proveedor.v_PrimerNombre + " " + Proveedor.v_ApePaterno + " " + Proveedor.v_ApeMaterno + " " + Proveedor.v_RazonSocial).Trim();

                                if (importacionDetalleGastosDto.d_ValorVenta != 0 && importacionDetalleGastosDto.d_NAfectoDetraccion != 0) // Se generan dos registros en la grilla , Uno para el Valor Venta y otro para el No Afecto Igv
                                {
                                    //Fila Valor Venta
                                    _compradetalleDto.v_NroCuenta = importacionDetalleGastosDto.v_IdAsientoContable;
                                    _compradetalleDto.i_Anticipio = 0;
                                    _compradetalleDto.i_IdAlmacen = pobjDtoEntity.i_IdAlmacen;
                                    _compradetalleDto.v_IdProductoDetalle = null;
                                    _compradetalleDto.d_Cantidad = 0;
                                    _compradetalleDto.d_Precio = 0;
                                    _compradetalleDto.i_IdUnidadMedida = -1;
                                    _compradetalleDto.d_ValorVenta = importacionDetalleGastosDto.d_ValorVenta;
                                    _compradetalleDto.d_PrecioVenta = importacionDetalleGastosDto.i_IdMoneda == 1 ? importacionDetalleGastosDto.d_ImporteSoles - importacionDetalleGastosDto.d_NAfectoDetraccion : importacionDetalleGastosDto.d_ImporteDolares - importacionDetalleGastosDto.d_NAfectoDetraccion;
                                    _compradetalleDto.d_Igv = importacionDetalleGastosDto.d_Igv;
                                    _compradetalleDto.i_IdDestino = 1;
                                    _compradetalleDto.i_IdCentroCostos = importacionDetalleGastosDto.i_CCosto;
                                    _compradetalleDto.v_Glosa = string.Empty;
                                    _compradetalleDto.v_NroGuiaRemision = string.Empty;
                                    _compradetalleDto.d_ValorDolaresDetraccion = importacionDetalleGastosDto.d_ValorDolaresDetraccion;
                                    _compradetalleDto.d_ValorSolesDetraccion = importacionDetalleGastosDto.d_ValorSolesDetraccion;
                                    _compradetalleDto.v_NroPedido = string.Empty;
                                    _TempDetalle_AgregarCompraDto.Add(_compradetalleDto);
                                    _compradetalleDto = new compradetalleDto();

                                    //Fila Valor No Afecto

                                    _compradetalleDto.v_NroCuenta = importacionDetalleGastosDto.v_IdAsientoContable;
                                    _compradetalleDto.i_Anticipio = 0;
                                    _compradetalleDto.i_IdAlmacen = pobjDtoEntity.i_IdAlmacen;
                                    _compradetalleDto.v_IdProductoDetalle = null;
                                    _compradetalleDto.d_Cantidad = 0;
                                    _compradetalleDto.d_Precio = 0;
                                    _compradetalleDto.i_IdUnidadMedida = -1;
                                    _compradetalleDto.d_ValorVenta = importacionDetalleGastosDto.d_NAfectoDetraccion;
                                    _compradetalleDto.d_PrecioVenta = importacionDetalleGastosDto.d_NAfectoDetraccion;
                                    _compradetalleDto.d_Igv = 0;
                                    _compradetalleDto.i_IdDestino = 4;
                                    _compradetalleDto.i_IdCentroCostos = importacionDetalleGastosDto.i_CCosto;
                                    _compradetalleDto.v_Glosa = string.Empty;
                                    _compradetalleDto.v_NroGuiaRemision = string.Empty;
                                    _compradetalleDto.d_ValorDolaresDetraccion = importacionDetalleGastosDto.d_ValorDolaresDetraccionNoAfecto;
                                    _compradetalleDto.d_ValorSolesDetraccion = importacionDetalleGastosDto.d_ValorSolesDetraccionNoAfecto;
                                    _compradetalleDto.v_NroPedido = string.Empty;
                                    _TempDetalle_AgregarCompraDto.Add(_compradetalleDto);
                                    _compraDto.i_IdDestino = 5;
                                    _compraDto.d_Anticipio = 0;
                                    _compraDto.d_ValorVenta = _TempDetalle_AgregarCompraDto.Sum(A => A.d_ValorVenta);
                                    _compraDto.d_IGV = _TempDetalle_AgregarCompraDto.Sum(y => y.d_Igv);
                                    _compraDto.d_Total = _TempDetalle_AgregarCompraDto.Sum(b => b.d_PrecioVenta);
                                    _compradetalleDto = new compradetalleDto();


                                }
                                else
                                {

                                    _compraDto.i_IdDestino = importacionDetalleGastosDto.d_ValorVenta != 0 ? 1 : 4;
                                    _compradetalleDto.v_NroCuenta = importacionDetalleGastosDto.v_IdAsientoContable;
                                    _compradetalleDto.i_Anticipio = 0;
                                    _compradetalleDto.i_IdAlmacen = pobjDtoEntity.i_IdAlmacen;
                                    _compradetalleDto.v_IdProductoDetalle = null;
                                    _compradetalleDto.d_Cantidad = 0;
                                    _compradetalleDto.d_Precio = 0;
                                    _compradetalleDto.i_IdUnidadMedida = -1;
                                    _compradetalleDto.d_PrecioVenta = importacionDetalleGastosDto.i_IdMoneda == 1 ? importacionDetalleGastosDto.d_ImporteSoles : importacionDetalleGastosDto.d_ImporteDolares;
                                    _compradetalleDto.d_ValorVenta = importacionDetalleGastosDto.d_NAfectoDetraccion == 0 ? importacionDetalleGastosDto.d_ValorVenta : importacionDetalleGastosDto.d_NAfectoDetraccion;
                                    _compradetalleDto.d_Igv = importacionDetalleGastosDto.d_ValorVenta != 0 ? importacionDetalleGastosDto.d_Igv : 0;
                                    //_compradetalleDto.i_IdDestino = -1;
                                    _compradetalleDto.i_IdDestino = _compraDto.i_IdDestino;
                                    _compradetalleDto.i_IdCentroCostos = importacionDetalleGastosDto.i_CCosto;
                                    _compradetalleDto.v_Glosa = string.Empty;
                                    _compradetalleDto.v_NroGuiaRemision = string.Empty;
                                    _compradetalleDto.d_ValorDolaresDetraccion = _compraDto.i_IdDestino == 1 ? importacionDetalleGastosDto.d_ValorDolaresDetraccion : importacionDetalleGastosDto.d_ValorDolaresDetraccionNoAfecto;
                                    _compradetalleDto.d_ValorSolesDetraccion = _compraDto.i_IdDestino == 1 ? importacionDetalleGastosDto.d_ValorSolesDetraccion : importacionDetalleGastosDto.d_ValorSolesDetraccionNoAfecto;
                                    _compradetalleDto.v_NroPedido = string.Empty;
                                    _TempDetalle_AgregarCompraDto.Add(_compradetalleDto);
                                    _compradetalleDto = new compradetalleDto();
                                    _compraDto.d_Anticipio = 0;
                                    _compraDto.d_ValorVenta = _TempDetalle_AgregarCompraDto.Sum(a => a.d_ValorVenta);
                                    _compraDto.d_IGV = _TempDetalle_AgregarCompraDto.Sum(y => y.d_Igv);
                                    _compraDto.d_Total = _TempDetalle_AgregarCompraDto.Sum(c => c.d_PrecioVenta);

                                }
                                _objComprasBL.InsertarCompra(ref pobjOperationResult, _compraDto, Globals.ClientSession.GetAsList(), _TempDetalle_AgregarCompraDto, null);

                                if (pobjOperationResult.Success == 0) return string.Empty;
                                _compraDto = new compraDto();
                                _TempDetalle_AgregarCompraDto = new List<compradetalleDto>();
                                dbContext.SaveChanges();
                            }
                        }

                    }

                    foreach (importaciondetallegastosDto importacionDetalleGastosDto in pTemp_ModificarDetalleGastos)
                    {
                        importaciondetallegastos objEntityImportacionDetalleGastos = importaciondetallegastosAssembler.ToEntity(importacionDetalleGastosDto);

                        var query = (from n in dbContext.importaciondetallegastos
                                     where n.v_IdImportacionDetalleGastos == importacionDetalleGastosDto.v_IdImportacionDetalleGastos
                                     select n).FirstOrDefault();

                        objEntityImportacionDetalleGastos.t_ActualizaFecha = DateTime.Now;
                        objEntityImportacionDetalleGastos.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        dbContext.importaciondetallegastos.ApplyCurrentValues(objEntityImportacionDetalleGastos);
                        Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "importacionDetalleGastos", query.v_IdImportacionDetalleGastos);


                        if (ExistenciaDocumentoenCompras(importacionDetalleGastosDto.i_IdTipoDocumento.Value, importacionDetalleGastosDto.v_SerieDocumento, importacionDetalleGastosDto.v_CorrelativoDocumento, importacionDetalleGastosDto.v_Detalle, importacionDetalleGastosDto.v_IdImportacionDetalleGastos))
                        {
                            string Mes = string.Empty;
                            string Periodo = "";
                            if (Globals.ClientSession.i_FechaEmision.Value.ToString() == "1")//FechaRegistroGastos
                            {
                                Mes = importacionDetalleGastosDto.t_FechaRegistro.Value.Month.ToString("00").Trim();
                                Periodo = importacionDetalleGastosDto.t_FechaRegistro.Value.Year.ToString();
                                _ListadoCompras = _objComprasBL.ObtenerListadoCompras(ref pobjOperationResult, importacionDetalleGastosDto.t_FechaRegistro.Value.Year.ToString(), Mes);
                                _compraDto.t_FechaEmision = importacionDetalleGastosDto.t_FechaEmision;
                                _compraDto.t_FechaRegistro = importacionDetalleGastosDto.t_FechaRegistro;
                                _compraDto.t_FechaVencimiento = importacionDetalleGastosDto.t_FechaEmision;

                            }
                            else
                            {
                                Mes = pobjDtoEntity.t_FechaRegistro.Value.Month.ToString("00").Trim();//Fecha Registro Importacion
                                Periodo = pobjDtoEntity.t_FechaRegistro.Value.Year.ToString();
                                _ListadoCompras = _objComprasBL.ObtenerListadoCompras(ref pobjOperationResult, pobjDtoEntity.t_FechaRegistro.Value.Year.ToString(), Mes);

                                _compraDto.t_FechaEmision = importacionDetalleGastosDto.t_FechaEmision;
                                _compraDto.t_FechaRegistro = objEntity.t_FechaRegistro;
                                _compraDto.t_FechaVencimiento = importacionDetalleGastosDto.t_FechaEmision;
                            }

                            int _MaxMovimiento;
                            _MaxMovimiento = _ListadoCompras.Count() > 0 ? int.Parse(_ListadoCompras[_ListadoCompras.Count() - 1].Value1.ToString()) : 0;
                            _MaxMovimiento++;
                            _compraDto.v_Periodo = Periodo;
                            _compraDto.v_Mes = Mes;
                            _compraDto.v_Correlativo = _MaxMovimiento.ToString("00000000");
                            _compraDto.v_IdDocumentoReferencia = importacionDetalleGastosDto.v_IdImportacionDetalleGastos;
                            _compraDto.i_IdIgv = Globals.ClientSession.i_IdIgv;
                            _compraDto.i_IdTipoDocumento = importacionDetalleGastosDto.i_IdTipoDocumento;
                            _compraDto.v_SerieDocumento = importacionDetalleGastosDto.v_SerieDocumento;
                            _compraDto.v_CorrelativoDocumento = importacionDetalleGastosDto.v_CorrelativoDocumento;

                            _compraDto.i_IdCondicionPago = 1; //Por Confirmar
                            _compraDto.t_FechaDetraccion = importacionDetalleGastosDto.t_FechaEmision;
                            _compraDto.t_FechaRef = importacionDetalleGastosDto.t_FechaEmision;
                            _compraDto.i_IdEstado = 1;
                            _compraDto.i_IdTipoDocumentoRef = importacionDetalleGastosDto.i_IdTipoDocRef;
                            _compraDto.v_CorrelativoDocumentoRef = importacionDetalleGastosDto.v_CorrelativoDocRef;
                            _compraDto.v_SerieDocumentoRef = importacionDetalleGastosDto.v_SerieDocRef;
                            _compraDto.v_IdProveedor = importacionDetalleGastosDto.v_Detalle;
                            _compraDto.v_GuiaRemisionSerie = string.Empty;
                            _compraDto.v_GuiaRemisionCorrelativo = string.Empty;
                            _compraDto.d_TipoCambio = importacionDetalleGastosDto.d_TipoCambio;
                            _compraDto.v_Glosa = importacionDetalleGastosDto.v_Glosa;
                            _compraDto.i_IdMoneda = importacionDetalleGastosDto.i_IdMoneda;
                            _compraDto.i_IdTipoCompra = importacionDetalleGastosDto.i_IdMoneda == 1 ? (int)TipoCompra.MonedaNacional : (int)TipoCompra.MonedaExtranjera;
                            _compraDto.i_EsAfectoIgv = importacionDetalleGastosDto.d_Igv == 0 ? 1 : 1;
                            _compraDto.i_IdEstablecimiento = pobjDtoEntity.i_IdEstablecimiento;



                            _compraDto.i_EsDetraccion = importacionDetalleGastosDto.i_EsDetraccion;
                            _compraDto.i_CodigoDetraccion = importacionDetalleGastosDto.i_CodigoDetraccion;
                            _compraDto.v_NroDetraccion = importacionDetalleGastosDto.v_NroDetraccion;
                            _compraDto.d_PorcentajeDetraccion = importacionDetalleGastosDto.d_PorcentajeDetraccion;
                            _compraDto.t_FechaDetraccion = importacionDetalleGastosDto.t_FechaDetraccion;



                            _compraDto.i_PreciosIncluyenIgv = 0;
                            _compraDto.i_DeduccionAnticipio = 0;


                            var Proveedor = (from n in dbContext.cliente
                                             where n.i_Eliminado == 0 && n.v_FlagPantalla == "V" & n.v_IdCliente == importacionDetalleGastosDto.v_Detalle
                                             select n).FirstOrDefault();
                            _compraDto.NombreProveedor = (Proveedor.v_PrimerNombre + " " + Proveedor.v_ApePaterno + " " + Proveedor.v_ApeMaterno + " " + Proveedor.v_RazonSocial).Trim();

                            if (importacionDetalleGastosDto.d_ValorVenta != 0 && importacionDetalleGastosDto.d_NAfectoDetraccion != 0) // Se generan dos registros en la grilla , Uno para el Valor Venta y otro para el No Afecto Igv
                            {
                                //Fila Valor Venta
                                _compradetalleDto.v_NroCuenta = importacionDetalleGastosDto.v_IdAsientoContable;
                                _compradetalleDto.i_Anticipio = 0;
                                _compradetalleDto.i_IdAlmacen = pobjDtoEntity.i_IdAlmacen;
                                _compradetalleDto.v_IdProductoDetalle = null;
                                _compradetalleDto.d_Cantidad = 0;
                                _compradetalleDto.d_Precio = 0;
                                _compradetalleDto.i_IdUnidadMedida = -1;
                                _compradetalleDto.d_ValorVenta = importacionDetalleGastosDto.d_ValorVenta;
                                _compradetalleDto.d_PrecioVenta = importacionDetalleGastosDto.i_IdMoneda == 1 ? importacionDetalleGastosDto.d_ImporteSoles - importacionDetalleGastosDto.d_NAfectoDetraccion : importacionDetalleGastosDto.d_ImporteDolares - importacionDetalleGastosDto.d_NAfectoDetraccion;
                                _compradetalleDto.d_Igv = importacionDetalleGastosDto.d_Igv;
                                _compradetalleDto.i_IdDestino = 1;
                                _compradetalleDto.i_IdCentroCostos = importacionDetalleGastosDto.i_CCosto;
                                _compradetalleDto.v_Glosa = string.Empty;
                                _compradetalleDto.v_NroGuiaRemision = string.Empty;

                                _compradetalleDto.d_ValorDolaresDetraccion = importacionDetalleGastosDto.d_ValorDolaresDetraccion;
                                _compradetalleDto.d_ValorSolesDetraccion = importacionDetalleGastosDto.d_ValorSolesDetraccion;
                                _compradetalleDto.v_NroPedido = string.Empty;
                                _TempDetalle_AgregarCompraDto.Add(_compradetalleDto);
                                _compradetalleDto = new compradetalleDto();

                                //Fila Valor No Afecto

                                _compradetalleDto.v_NroCuenta = importacionDetalleGastosDto.v_IdAsientoContable;
                                _compradetalleDto.i_Anticipio = 0;
                                _compradetalleDto.i_IdAlmacen = pobjDtoEntity.i_IdAlmacen;
                                _compradetalleDto.v_IdProductoDetalle = null;
                                _compradetalleDto.d_Cantidad = 0;
                                _compradetalleDto.d_Precio = 0;
                                _compradetalleDto.i_IdUnidadMedida = -1;
                                _compradetalleDto.d_ValorVenta = importacionDetalleGastosDto.d_NAfectoDetraccion;
                                _compradetalleDto.d_PrecioVenta = importacionDetalleGastosDto.d_NAfectoDetraccion;
                                _compradetalleDto.d_Igv = 0;
                                _compradetalleDto.i_IdDestino = 4;
                                _compradetalleDto.i_IdCentroCostos = importacionDetalleGastosDto.i_CCosto;
                                _compradetalleDto.v_Glosa = string.Empty;
                                _compradetalleDto.v_NroGuiaRemision = string.Empty;
                                _compradetalleDto.d_ValorDolaresDetraccion = importacionDetalleGastosDto.d_ValorDolaresDetraccionNoAfecto;
                                _compradetalleDto.d_ValorSolesDetraccion = importacionDetalleGastosDto.d_ValorSolesDetraccionNoAfecto;
                                _compradetalleDto.v_NroPedido = string.Empty;
                                _TempDetalle_AgregarCompraDto.Add(_compradetalleDto);
                                _compraDto.i_IdDestino = 5;
                                _compraDto.d_Anticipio = 0;
                                _compraDto.d_ValorVenta = _TempDetalle_AgregarCompraDto.Sum(w => w.d_ValorVenta);
                                _compraDto.d_IGV = _TempDetalle_AgregarCompraDto.Sum(y => y.d_Igv);
                                _compraDto.d_Total = _TempDetalle_AgregarCompraDto.Sum(c => c.d_PrecioVenta);
                                _compradetalleDto = new compradetalleDto();


                            }
                            else
                            {

                                _compraDto.i_IdDestino = importacionDetalleGastosDto.d_ValorVenta != 0 ? 1 : 4;
                                _compradetalleDto.v_NroCuenta = importacionDetalleGastosDto.v_IdAsientoContable;
                                _compradetalleDto.i_Anticipio = 0;
                                _compradetalleDto.i_IdAlmacen = pobjDtoEntity.i_IdAlmacen;
                                _compradetalleDto.v_IdProductoDetalle = null;
                                _compradetalleDto.d_Cantidad = 0;
                                _compradetalleDto.d_Precio = 0;
                                _compradetalleDto.i_IdUnidadMedida = -1;
                                _compradetalleDto.d_PrecioVenta = importacionDetalleGastosDto.i_IdMoneda == 1 ? importacionDetalleGastosDto.d_ImporteSoles : importacionDetalleGastosDto.d_ImporteDolares;
                                _compradetalleDto.d_ValorVenta = importacionDetalleGastosDto.d_NAfectoDetraccion == 0 ? importacionDetalleGastosDto.d_ValorVenta : importacionDetalleGastosDto.d_NAfectoDetraccion;
                                _compradetalleDto.d_Igv = importacionDetalleGastosDto.d_ValorVenta != 0 ? importacionDetalleGastosDto.d_Igv : 0;
                                //_compradetalleDto.i_IdDestino = -1;
                                _compradetalleDto.i_IdDestino = _compraDto.i_IdDestino;
                                _compradetalleDto.i_IdCentroCostos = importacionDetalleGastosDto.i_CCosto;
                                _compradetalleDto.v_Glosa = string.Empty;
                                _compradetalleDto.v_NroGuiaRemision = string.Empty;

                                _compradetalleDto.d_ValorDolaresDetraccion = _compraDto.i_IdDestino == 1 ? importacionDetalleGastosDto.d_ValorDolaresDetraccion : importacionDetalleGastosDto.d_ValorDolaresDetraccionNoAfecto;
                                _compradetalleDto.d_ValorSolesDetraccion = _compraDto.i_IdDestino == 1 ? importacionDetalleGastosDto.d_ValorSolesDetraccion : importacionDetalleGastosDto.d_ValorSolesDetraccionNoAfecto;

                                _compradetalleDto.v_NroPedido = string.Empty;
                                _TempDetalle_AgregarCompraDto.Add(_compradetalleDto);
                                _compradetalleDto = new compradetalleDto();
                                _compraDto.d_Anticipio = 0;
                                _compraDto.d_ValorVenta = _TempDetalle_AgregarCompraDto.Sum(a => a.d_ValorVenta);
                                _compraDto.d_IGV = _TempDetalle_AgregarCompraDto.Sum(y => y.d_Igv);
                                _compraDto.d_Total = _TempDetalle_AgregarCompraDto.Sum(c => c.d_PrecioVenta);

                            }
                            if (pobjDtoEntity.i_IdEstado == 1)
                            {
                                _objComprasBL.RegenerarCompras(ref pobjOperationResult, _compraDto, Globals.ClientSession.GetAsList(), _TempDetalle_AgregarCompraDto);
                                if (pobjOperationResult.Success == 0) return string.Empty;
                                _compraDto = new compraDto();
                                _TempDetalle_AgregarCompraDto = new List<compradetalleDto>();
                                dbContext.SaveChanges();
                            }
                            else
                            {
                                var Compritas = (from n in dbContext.compra
                                                 where n.v_SerieDocumento.Trim() == importacionDetalleGastosDto.v_SerieDocumento.Trim() && n.v_CorrelativoDocumento.Trim() == importacionDetalleGastosDto.v_CorrelativoDocumento.Trim() && n.i_IdTipoDocumento == importacionDetalleGastosDto.i_IdTipoDocumento.Value && n.v_IdProveedor == importacionDetalleGastosDto.v_Detalle && n.i_Eliminado == 0 && n.v_IdDocumentoReferencia == importacionDetalleGastosDto.v_IdImportacionDetalleGastos
                                                 select n).FirstOrDefault();


                                _compraDto.i_IdEstado = 0;
                                _compraDto.v_IdCompra = Compritas.v_IdCompra;
                                List<compradetalleDto> ListaAgregar = new List<compradetalleDto>();
                                List<compradetalleDto> ListaEliminar = new List<compradetalleDto>();
                                List<compradetalleDto> ListaModificar = new List<compradetalleDto>();
                                _objComprasBL.ActualizarCompra(ref pobjOperationResult, _compraDto, Globals.ClientSession.GetAsList(), ListaAgregar, ListaModificar, ListaEliminar);
                                if (pobjOperationResult.Success == 0) return string.Empty;
                                _compraDto = new compraDto();
                                _TempDetalle_AgregarCompraDto = new List<compradetalleDto>();
                                dbContext.SaveChanges();


                            }
                        }
                    }

                    foreach (importaciondetallegastosDto importaciondetalleGastosDto in pTemp_EliminarDetalleGastos)
                    {

                        importaciondetallegastos objEntityImportacionDetalleProd = importaciondetallegastosAssembler.ToEntity(importaciondetalleGastosDto);

                        var query = (from n in dbContext.importaciondetallegastos
                                     where n.v_IdImportacionDetalleGastos == importaciondetalleGastosDto.v_IdImportacionDetalleGastos
                                     select n).FirstOrDefault();

                        if (query != null)
                        {
                            query.t_ActualizaFecha = DateTime.Now;
                            query.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            query.i_Eliminado = 1;
                        }

                        dbContext.importaciondetallegastos.ApplyCurrentValues(query);
                        _objComprasBL.EliminarComprasXDocRef(ref pobjOperationResult, importaciondetalleGastosDto.v_IdImportacionDetalleGastos, Globals.ClientSession.GetAsList(), false);
                        if (pobjOperationResult.Success == 0) return string.Empty;

                    }

                    dbContext.SaveChanges();

                    #endregion

                    #region ActualizaCorrelativos()
                    /*
                    var Compras = (from n in dbContext.importaciondetallegastos
                                   join a in dbContext.compra on new { A = n.v_IdImportacionDetalleGastos, eliminado = 0 } equals new { A = a.v_IdDocumentoReferencia, eliminado = a.i_Eliminado.Value } into a_join
                                   from a in a_join
                                   where n.i_Eliminado.Value == 0
                                   select a).ToList();
                    var ComprasDocReferencia = Compras.Select(w => w.v_IdDocumentoReferencia).ToList();


                    var Gastos = (from n in dbContext.importaciondetallegastos
                                  where n.v_IdImportacion == pobjDtoEntity.v_IdImportacion && n.i_Eliminado == 0
                                  select n).ToList();

                    var GastosGeneranCompras = Gastos.Select(y => y.v_IdImportacionDetalleGastos).ToList();


                    foreach (var gasto in Gastos)
                    {
                        foreach (var compra in Compras.Where(t => t.v_IdDocumentoReferencia == gasto.v_IdImportacionDetalleGastos))
                        {
                            //if (compra.v_IdDocumentoReferencia == gasto.v_IdImportacionDetalleGastos)
                            //{
                                switch (Globals.ClientSession.i_FechaRegistro.Value) //Fecha Registro Importacion
                                {

                                    case 1: 
                                        string MesFechaRegistro = int.Parse(pobjDtoEntity.t_FechaRegistro.Value.Month.ToString()).ToString("00");

                                        if (MesFechaRegistro == compra.v_Mes.Trim())
                                        {
                                        }
                                        else
                                        {
                                            // Eliminanos la compra con la serie 
                                            _objComprasBL.EliminarComprasXDocRef(ref pobjOperationResult, gasto.v_IdImportacionDetalleGastos, ClientSession, false);
                                            //Creamos nueva Compra con Serie del Mes de F.Registro
                                            if (pobjDtoEntity.i_IdEstado == 1)
                                            {

                                                if (!ExistenciaDocumentoenCompras(gasto.i_IdTipoDocumento.Value, gasto.v_SerieDocumento, gasto.v_CorrelativoDocumento, gasto.v_Detalle, gasto.v_IdImportacionDetalleGastos))
                                                {
                                                    string Mes = string.Empty;
                                                    string Periodo = string.Empty;
                                                    Mes = pobjDtoEntity.t_FechaRegistro.Value.Month.ToString("00");
                                                    Periodo = pobjDtoEntity.t_FechaRegistro.Value.Year.ToString();
                                                    _ListadoCompras = _objComprasBL.ObtenerListadoCompras(ref pobjOperationResult, pobjDtoEntity.t_FechaRegistro.Value.Year.ToString(), Mes);
                                                    int _MaxMovimiento;
                                                    _MaxMovimiento = _ListadoCompras.Count() > 0 ? int.Parse(_ListadoCompras[_ListadoCompras.Count() - 1].Value1.ToString()) : 0;
                                                    _MaxMovimiento++;
                                                    _compraDto.v_Periodo = Periodo;
                                                    _compraDto.v_Mes = Mes;
                                                    _compraDto.v_Correlativo = _MaxMovimiento.ToString("00000000");
                                                    _compraDto.v_IdDocumentoReferencia = gasto.v_IdImportacionDetalleGastos;
                                                    _compraDto.i_IdIgv = Globals.ClientSession.i_IdIgv;
                                                    _compraDto.i_IdTipoDocumento = gasto.i_IdTipoDocumento;
                                                    _compraDto.v_SerieDocumento = gasto.v_SerieDocumento;
                                                    _compraDto.v_CorrelativoDocumento = gasto.v_CorrelativoDocumento;
                                                    _compraDto.t_FechaEmision = gasto.t_FechaEmision;
                                                    _compraDto.t_FechaRegistro = objEntity.t_FechaRegistro; //Por Confirmar
                                                    _compraDto.t_FechaVencimiento = gasto.t_FechaEmision; //Por Confirmar
                                                    _compraDto.i_IdCondicionPago = 1; //Por Confirmar
                                                    _compraDto.t_FechaDetraccion = gasto.t_FechaEmision;
                                                    _compraDto.t_FechaRef = gasto.t_FechaEmision;
                                                    _compraDto.i_IdEstado = 1;
                                                    _compraDto.i_IdTipoDocumentoRef = gasto.i_IdTipoDocRef;
                                                    _compraDto.v_CorrelativoDocumentoRef = gasto.v_CorrelativoDocRef;
                                                    _compraDto.v_SerieDocumentoRef = gasto.v_SerieDocRef;
                                                    _compraDto.v_IdProveedor = gasto.v_Detalle;
                                                    _compraDto.v_GuiaRemisionSerie = string.Empty;
                                                    _compraDto.v_GuiaRemisionCorrelativo = string.Empty;
                                                    _compraDto.d_TipoCambio = gasto.d_TipoCambio;
                                                    _compraDto.v_Glosa = gasto.v_Glosa;
                                                    _compraDto.i_IdMoneda = gasto.i_IdMoneda;
                                                    _compraDto.i_IdTipoCompra = gasto.i_IdMoneda == 1 ? (int)TipoCompra.MonedaNacional : (int)TipoCompra.MonedaExtranjera;
                                                    _compraDto.i_EsAfectoIgv = gasto.d_Igv == 0 ? 1 : 1;
                                                    _compraDto.i_IdEstablecimiento = pobjDtoEntity.i_IdEstablecimiento;
                                                    _compraDto.i_EsDetraccion = 0;
                                                    _compraDto.i_CodigoDetraccion = 0;
                                                    _compraDto.v_NroDetraccion = string.Empty;
                                                    _compraDto.d_PorcentajeDetraccion = 0;
                                                    _compraDto.i_PreciosIncluyenIgv = 0;
                                                    _compraDto.i_DeduccionAnticipio = 0;

                                                    var Proveedor = (from n in dbContext.cliente
                                                                     where n.i_Eliminado == 0 && n.v_FlagPantalla == "V" & n.v_IdCliente == gasto.v_Detalle
                                                                     select n).FirstOrDefault();
                                                    _compraDto.NombreProveedor = (Proveedor.v_PrimerNombre + " " + Proveedor.v_ApePaterno + " " + Proveedor.v_ApeMaterno + " " + Proveedor.v_RazonSocial).Trim();

                                                    if (gasto.d_ValorVenta != 0 && gasto.d_NAfectoDetraccion != 0) // Se generan dos registros en la grilla , Uno para el Valor Venta y otro para el No Afecto Igv
                                                    {
                                                        //Fila Valor Venta
                                                        _compradetalleDto.v_NroCuenta = gasto.v_IdAsientoContable;
                                                        _compradetalleDto.i_Anticipio = 0;
                                                        _compradetalleDto.i_IdAlmacen = pobjDtoEntity.i_IdAlmacen;
                                                        _compradetalleDto.v_IdProductoDetalle = null;
                                                        _compradetalleDto.d_Cantidad = 0;
                                                        _compradetalleDto.d_Precio = 0;
                                                        _compradetalleDto.i_IdUnidadMedida = -1;
                                                        _compradetalleDto.d_ValorVenta = gasto.d_ValorVenta;
                                                        _compradetalleDto.d_PrecioVenta = gasto.i_IdMoneda == 1 ? gasto.d_ImporteSoles - gasto.d_NAfectoDetraccion : gasto.d_ImporteDolares - gasto.d_NAfectoDetraccion;
                                                        _compradetalleDto.d_Igv = gasto.d_Igv;
                                                        _compradetalleDto.i_IdDestino = 1;
                                                        _compradetalleDto.i_IdCentroCostos = gasto.i_CCosto;
                                                        _compradetalleDto.v_Glosa = string.Empty;
                                                        _compradetalleDto.v_NroGuiaRemision = string.Empty;
                                                        _compradetalleDto.d_ValorDolaresDetraccion = 0;
                                                        _compradetalleDto.d_ValorSolesDetraccion = 0;
                                                        _compradetalleDto.v_NroPedido = string.Empty;
                                                        _TempDetalle_AgregarCompraDto.Add(_compradetalleDto);
                                                        _compradetalleDto = new compradetalleDto();

                                                        //Fila Valor No Afecto

                                                        _compradetalleDto.v_NroCuenta = gasto.v_IdAsientoContable;
                                                        _compradetalleDto.i_Anticipio = 0;
                                                        _compradetalleDto.i_IdAlmacen = pobjDtoEntity.i_IdAlmacen;
                                                        _compradetalleDto.v_IdProductoDetalle = null;
                                                        _compradetalleDto.d_Cantidad = 0;
                                                        _compradetalleDto.d_Precio = 0;
                                                        _compradetalleDto.i_IdUnidadMedida = -1;
                                                        _compradetalleDto.d_ValorVenta = gasto.d_NAfectoDetraccion;
                                                        _compradetalleDto.d_PrecioVenta = gasto.d_NAfectoDetraccion;
                                                        _compradetalleDto.d_Igv = 0;
                                                        _compradetalleDto.i_IdDestino = 4;
                                                        _compradetalleDto.i_IdCentroCostos = gasto.i_CCosto;
                                                        _compradetalleDto.v_Glosa = string.Empty;
                                                        _compradetalleDto.v_NroGuiaRemision = string.Empty;
                                                        _compradetalleDto.d_ValorDolaresDetraccion = 0;
                                                        _compradetalleDto.d_ValorSolesDetraccion = 0;
                                                        _compradetalleDto.v_NroPedido = string.Empty;
                                                        _TempDetalle_AgregarCompraDto.Add(_compradetalleDto);
                                                        _compraDto.i_IdDestino = 999;
                                                        _compraDto.d_Anticipio = 0;
                                                        _compraDto.d_ValorVenta = _TempDetalle_AgregarCompraDto.Sum(A => A.d_ValorVenta);
                                                        _compraDto.d_IGV = _TempDetalle_AgregarCompraDto.Sum(y => y.d_Igv);
                                                        _compraDto.d_Total = _TempDetalle_AgregarCompraDto.Sum(b => b.d_PrecioVenta);
                                                        _compradetalleDto = new compradetalleDto();


                                                    }
                                                    else
                                                    {

                                                        _compraDto.i_IdDestino = gasto.d_ValorVenta != 0 ? 1 : 4;
                                                        _compradetalleDto.v_NroCuenta = gasto.v_IdAsientoContable;
                                                        _compradetalleDto.i_Anticipio = 0;
                                                        _compradetalleDto.i_IdAlmacen = pobjDtoEntity.i_IdAlmacen;
                                                        _compradetalleDto.v_IdProductoDetalle = null;
                                                        _compradetalleDto.d_Cantidad = 0;
                                                        _compradetalleDto.d_Precio = 0;
                                                        _compradetalleDto.i_IdUnidadMedida = -1;
                                                        _compradetalleDto.d_PrecioVenta = gasto.i_IdMoneda == 1 ? gasto.d_ImporteSoles : gasto.d_ImporteDolares;
                                                        _compradetalleDto.d_ValorVenta = gasto.d_NAfectoDetraccion == 0 ? gasto.d_ValorVenta : gasto.d_NAfectoDetraccion;
                                                        _compradetalleDto.d_Igv = gasto.d_ValorVenta != 0 ? gasto.d_Igv : 0;
                                                        _compradetalleDto.i_IdDestino = -1;
                                                        _compradetalleDto.i_IdCentroCostos = "";
                                                        _compradetalleDto.v_Glosa = string.Empty;
                                                        _compradetalleDto.v_NroGuiaRemision = string.Empty;
                                                        _compradetalleDto.d_ValorDolaresDetraccion = 0;
                                                        _compradetalleDto.d_ValorSolesDetraccion = 0;
                                                        _compradetalleDto.v_NroPedido = string.Empty;
                                                        _TempDetalle_AgregarCompraDto.Add(_compradetalleDto);
                                                        _compradetalleDto = new compradetalleDto();
                                                        _compraDto.d_Anticipio = 0;
                                                        _compraDto.d_ValorVenta = _TempDetalle_AgregarCompraDto.Sum(a => a.d_ValorVenta);
                                                        _compraDto.d_IGV = _TempDetalle_AgregarCompraDto.Sum(y => y.d_Igv);
                                                        _compraDto.d_Total = _TempDetalle_AgregarCompraDto.Sum(c => c.d_PrecioVenta);

                                                    }
                                                    _objComprasBL.InsertarCompra(ref pobjOperationResult, _compraDto, Globals.ClientSession.GetAsList(), _TempDetalle_AgregarCompraDto, null);
                                                    if (pobjOperationResult.Success == 0) return string.Empty;
                                                    _compraDto = new compraDto();
                                                    _TempDetalle_AgregarCompraDto = new List<compradetalleDto>();
                                                    dbContext.SaveChanges();
                                                }
                                            }
                                        }
                                        break;

                                    case 0:  //Es porque documento Compra debe ser generado con el mes de la Fecha de Registro de los Gastos
                                        string MesFechaRegistroG = int.Parse(gasto.t_FechaRegistro.Value.Month.ToString()).ToString("00");
                                        if (MesFechaRegistroG == compra.v_Mes.Trim())
                                        { }
                                        else
                                        {
                                            _objComprasBL.EliminarComprasXDocRef(ref pobjOperationResult, gasto.v_IdImportacionDetalleGastos, ClientSession, false);// Eliminanos la compra con la serie    
                                            //Creamos nueva Compra con Serie del Mes de F.Registro
                                            if (pobjDtoEntity.i_IdEstado == 1)
                                            {

                                                if (!ExistenciaDocumentoenCompras(gasto.i_IdTipoDocumento.Value, gasto.v_SerieDocumento, gasto.v_CorrelativoDocumento, gasto.v_Detalle, gasto.v_IdImportacionDetalleGastos))
                                                {
                                                    string Mes = string.Empty;
                                                    string Periodo = string.Empty;
                                                    Mes = gasto.t_FechaRegistro .Value.Month.ToString("00");
                                                    Periodo = gasto.t_FechaRegistro.Value.Year.ToString();
                                                    _ListadoCompras = _objComprasBL.ObtenerListadoCompras(ref pobjOperationResult, gasto.t_FechaRegistro.Value.Year.ToString(), Mes);
                                                    int _MaxMovimiento;
                                                    _MaxMovimiento = _ListadoCompras.Count() > 0 ? int.Parse(_ListadoCompras[_ListadoCompras.Count() - 1].Value1.ToString()) : 0;
                                                    _MaxMovimiento++;
                                                    _compraDto.v_Periodo = Periodo;
                                                    _compraDto.v_Mes = Mes;
                                                    _compraDto.v_Correlativo = _MaxMovimiento.ToString("00000000");
                                                    _compraDto.v_IdDocumentoReferencia = gasto.v_IdImportacionDetalleGastos;
                                                    _compraDto.i_IdIgv = Globals.ClientSession.i_IdIgv;
                                                    _compraDto.i_IdTipoDocumento = gasto.i_IdTipoDocumento;
                                                    _compraDto.v_SerieDocumento = gasto.v_SerieDocumento;
                                                    _compraDto.v_CorrelativoDocumento = gasto.v_CorrelativoDocumento;
                                                    _compraDto.t_FechaEmision = gasto.t_FechaEmision;
                                                    _compraDto.t_FechaRegistro = gasto.t_FechaEmision; //Por Confirmar
                                                    _compraDto.t_FechaVencimiento = gasto.t_FechaEmision; //Por Confirmar
                                                    _compraDto.i_IdCondicionPago = 1; //Por Confirmar
                                                    _compraDto.t_FechaDetraccion = gasto.t_FechaEmision;
                                                    _compraDto.t_FechaRef = gasto.t_FechaEmision;
                                                    _compraDto.i_IdEstado = 1;
                                                    _compraDto.i_IdTipoDocumentoRef = gasto.i_IdTipoDocRef;
                                                    _compraDto.v_CorrelativoDocumentoRef = gasto.v_CorrelativoDocRef;
                                                    _compraDto.v_SerieDocumentoRef = gasto.v_SerieDocRef;
                                                    _compraDto.v_IdProveedor = gasto.v_Detalle;
                                                    _compraDto.v_GuiaRemisionSerie = string.Empty;
                                                    _compraDto.v_GuiaRemisionCorrelativo = string.Empty;
                                                    _compraDto.d_TipoCambio = gasto.d_TipoCambio;
                                                    _compraDto.v_Glosa = gasto.v_Glosa;
                                                    _compraDto.i_IdMoneda = gasto.i_IdMoneda;
                                                    _compraDto.i_IdTipoCompra = gasto.i_IdMoneda == 1 ? (int)TipoCompra.MonedaNacional : (int)TipoCompra.MonedaExtranjera;
                                                    _compraDto.i_EsAfectoIgv = gasto.d_Igv == 0 ? 1 : 1;
                                                    _compraDto.i_IdEstablecimiento = pobjDtoEntity.i_IdEstablecimiento;
                                                    _compraDto.i_EsDetraccion = 0;
                                                    _compraDto.i_CodigoDetraccion = 0;
                                                    _compraDto.v_NroDetraccion = string.Empty;
                                                    _compraDto.d_PorcentajeDetraccion = 0;
                                                    _compraDto.i_PreciosIncluyenIgv = 0;
                                                    _compraDto.i_DeduccionAnticipio = 0;

                                                    var Proveedor = (from n in dbContext.cliente
                                                                     where n.i_Eliminado == 0 && n.v_FlagPantalla == "V" & n.v_IdCliente == gasto.v_Detalle
                                                                     select n).FirstOrDefault();
                                                    _compraDto.NombreProveedor = (Proveedor.v_PrimerNombre + " " + Proveedor.v_ApePaterno + " " + Proveedor.v_ApeMaterno + " " + Proveedor.v_RazonSocial).Trim();

                                                    if (gasto.d_ValorVenta != 0 && gasto.d_NAfectoDetraccion != 0) // Se generan dos registros en la grilla , Uno para el Valor Venta y otro para el No Afecto Igv
                                                    {
                                                        //Fila Valor Venta
                                                        _compradetalleDto.v_NroCuenta = gasto.v_IdAsientoContable;
                                                        _compradetalleDto.i_Anticipio = 0;
                                                        _compradetalleDto.i_IdAlmacen = pobjDtoEntity.i_IdAlmacen;
                                                        _compradetalleDto.v_IdProductoDetalle = null;
                                                        _compradetalleDto.d_Cantidad = 0;
                                                        _compradetalleDto.d_Precio = 0;
                                                        _compradetalleDto.i_IdUnidadMedida = -1;
                                                        _compradetalleDto.d_ValorVenta = gasto.d_ValorVenta;
                                                        _compradetalleDto.d_PrecioVenta = gasto.i_IdMoneda == 1 ? gasto.d_ImporteSoles - gasto.d_NAfectoDetraccion : gasto.d_ImporteDolares - gasto.d_NAfectoDetraccion;
                                                        _compradetalleDto.d_Igv = gasto.d_Igv;
                                                        _compradetalleDto.i_IdDestino = 1;
                                                        _compradetalleDto.i_IdCentroCostos = gasto.i_CCosto;
                                                        _compradetalleDto.v_Glosa = string.Empty;
                                                        _compradetalleDto.v_NroGuiaRemision = string.Empty;
                                                        _compradetalleDto.d_ValorDolaresDetraccion = 0;
                                                        _compradetalleDto.d_ValorSolesDetraccion = 0;
                                                        _compradetalleDto.v_NroPedido = string.Empty;
                                                        _TempDetalle_AgregarCompraDto.Add(_compradetalleDto);
                                                        _compradetalleDto = new compradetalleDto();

                                                        //Fila Valor No Afecto

                                                        _compradetalleDto.v_NroCuenta = gasto.v_IdAsientoContable;
                                                        _compradetalleDto.i_Anticipio = 0;
                                                        _compradetalleDto.i_IdAlmacen = pobjDtoEntity.i_IdAlmacen;
                                                        _compradetalleDto.v_IdProductoDetalle = null;
                                                        _compradetalleDto.d_Cantidad = 0;
                                                        _compradetalleDto.d_Precio = 0;
                                                        _compradetalleDto.i_IdUnidadMedida = -1;
                                                        _compradetalleDto.d_ValorVenta = gasto.d_NAfectoDetraccion;
                                                        _compradetalleDto.d_PrecioVenta = gasto.d_NAfectoDetraccion;
                                                        _compradetalleDto.d_Igv = 0;
                                                        _compradetalleDto.i_IdDestino = 4;
                                                        _compradetalleDto.i_IdCentroCostos = gasto.i_CCosto;
                                                        _compradetalleDto.v_Glosa = string.Empty;
                                                        _compradetalleDto.v_NroGuiaRemision = string.Empty;
                                                        _compradetalleDto.d_ValorDolaresDetraccion = 0;
                                                        _compradetalleDto.d_ValorSolesDetraccion = 0;
                                                        _compradetalleDto.v_NroPedido = string.Empty;
                                                        _TempDetalle_AgregarCompraDto.Add(_compradetalleDto);
                                                        _compraDto.i_IdDestino = 999;
                                                        _compraDto.d_Anticipio = 0;
                                                        _compraDto.d_ValorVenta = _TempDetalle_AgregarCompraDto.Sum(A => A.d_ValorVenta);
                                                        _compraDto.d_IGV = _TempDetalle_AgregarCompraDto.Sum(y => y.d_Igv);
                                                        _compraDto.d_Total = _TempDetalle_AgregarCompraDto.Sum(b => b.d_PrecioVenta);
                                                        _compradetalleDto = new compradetalleDto();


                                                    }
                                                    else
                                                    {

                                                        _compraDto.i_IdDestino = gasto.d_ValorVenta != 0 ? 1 : 4;
                                                        _compradetalleDto.v_NroCuenta = gasto.v_IdAsientoContable;
                                                        _compradetalleDto.i_Anticipio = 0;
                                                        _compradetalleDto.i_IdAlmacen = pobjDtoEntity.i_IdAlmacen;
                                                        _compradetalleDto.v_IdProductoDetalle = null;
                                                        _compradetalleDto.d_Cantidad = 0;
                                                        _compradetalleDto.d_Precio = 0;
                                                        _compradetalleDto.i_IdUnidadMedida = -1;
                                                        _compradetalleDto.d_PrecioVenta = gasto.i_IdMoneda == 1 ? gasto.d_ImporteSoles : gasto.d_ImporteDolares;
                                                        _compradetalleDto.d_ValorVenta = gasto.d_NAfectoDetraccion == 0 ? gasto.d_ValorVenta : gasto.d_NAfectoDetraccion;
                                                        _compradetalleDto.d_Igv = gasto.d_ValorVenta != 0 ? gasto.d_Igv : 0;
                                                        _compradetalleDto.i_IdDestino = -1;
                                                        _compradetalleDto.i_IdCentroCostos = "";
                                                        _compradetalleDto.v_Glosa = string.Empty;
                                                        _compradetalleDto.v_NroGuiaRemision = string.Empty;
                                                        _compradetalleDto.d_ValorDolaresDetraccion = 0;
                                                        _compradetalleDto.d_ValorSolesDetraccion = 0;
                                                        _compradetalleDto.v_NroPedido = string.Empty;
                                                        _TempDetalle_AgregarCompraDto.Add(_compradetalleDto);
                                                        _compradetalleDto = new compradetalleDto();
                                                        _compraDto.d_Anticipio = 0;
                                                        _compraDto.d_ValorVenta = _TempDetalle_AgregarCompraDto.Sum(a => a.d_ValorVenta);
                                                        _compraDto.d_IGV = _TempDetalle_AgregarCompraDto.Sum(y => y.d_Igv);
                                                        _compraDto.d_Total = _TempDetalle_AgregarCompraDto.Sum(c => c.d_PrecioVenta);

                                                    }
                                                    _objComprasBL.InsertarCompra(ref pobjOperationResult, _compraDto, Globals.ClientSession.GetAsList(), _TempDetalle_AgregarCompraDto, null);
                                                    if (pobjOperationResult.Success == 0) return string.Empty;
                                                    _compraDto = new compraDto();
                                                    _TempDetalle_AgregarCompraDto = new List<compradetalleDto>();
                                                    dbContext.SaveChanges();
                                                }
                                            }
                                        }
                                        break;
                                }
                            //}

                        }

                    }
                      */
                    #endregion

                    #region Genera Libro Diario

                    //if (decimal.Parse(pobjDtoEntity.d_TotalValorFob.ToString()) > 0)
                    //{
                    DiarioBL _objDiarioBL = new DiarioBL();
                    diarioDto _diarioDto = new diarioDto();
                    List<KeyValueDTO> _ListadoDiarios = new List<KeyValueDTO>();
                    List<diariodetalleDto> TempDiarioInsertarImportaciones = new List<diariodetalleDto>();
                    diariodetalleDto D_Totales = new diariodetalleDto();
                    diariodetalleDto H_Totales = new diariodetalleDto();
                    string[] IdRegistroEliminado = new string[3];
                    int _MaxMovimientoDiario;
                    var DetalleImportacionesFOB = (from d in dbContext.importaciondetallefob
                                                   where d.v_IdImportacion == pobjDtoEntity.v_IdImportacion && d.i_Eliminado == 0
                                                   select d).ToList();

                    var queryExisteDiario = (from n in dbContext.diario
                                             where n.v_IdDocumentoReferencia == pobjDtoEntity.v_IdImportacion
                                             select n).FirstOrDefault();
                    if (queryExisteDiario != null)
                    {
                        IdRegistroEliminado = _objDiarioBL.EliminarDiarioXDocRef(ref pobjOperationResult, pobjDtoEntity.v_IdImportacion, ClientSession, false);
                        if (IdRegistroEliminado == null || (IdRegistroEliminado != null && (IdRegistroEliminado[1].Trim() != pobjDtoEntity.t_FechaRegistro.Value.Month.ToString("00") || IdRegistroEliminado[0] != pobjDtoEntity.t_FechaRegistro.Value.Year.ToString())))
                        {
                            _ListadoDiarios = _objDiarioBL.ObtenerListadoDiario(ref pobjOperationResult, pobjDtoEntity.v_Periodo, pobjDtoEntity.v_Mes, (int)LibroDiarios.Importaciones);
                            _MaxMovimientoDiario = _ListadoDiarios.Count() > 0 ? int.Parse(_ListadoDiarios[_ListadoDiarios.Count() - 1].Value1.ToString()) : 0;
                            _MaxMovimientoDiario++;
                            _diarioDto.v_Periodo = pobjDtoEntity.v_Periodo;
                            _diarioDto.v_Mes = pobjDtoEntity.v_Mes;
                            _diarioDto.v_Correlativo = _MaxMovimientoDiario.ToString("00000000");

                        }
                        else
                        {
                            _diarioDto.v_Periodo = IdRegistroEliminado[0];
                            _diarioDto.v_Mes = IdRegistroEliminado[1];
                            _diarioDto.v_Correlativo = IdRegistroEliminado[2];

                        }
                    }
                    else
                    {
                        _ListadoDiarios = _objDiarioBL.ObtenerListadoDiario(ref pobjOperationResult, pobjDtoEntity.v_Periodo, pobjDtoEntity.v_Mes, (int)LibroDiarios.Importaciones);
                        _MaxMovimientoDiario = _ListadoDiarios.Count() > 0 ? int.Parse(_ListadoDiarios[_ListadoDiarios.Count() - 1].Value1.ToString()) : 0;
                        _MaxMovimientoDiario++;
                        _diarioDto.v_Periodo = pobjDtoEntity.v_Periodo;
                        _diarioDto.v_Mes = pobjDtoEntity.v_Mes;
                        _diarioDto.v_Correlativo = _MaxMovimientoDiario.ToString("00000000");
                    }

                    if (pobjDtoEntity.i_IdEstado == 1)
                    {
                        _diarioDto.v_IdDocumentoReferencia = pobjDtoEntity.v_IdImportacion;
                        _diarioDto.v_Nombre = "IMPORTACIÓN" + " " + pobjDtoEntity.v_Mes.Trim() + "-" + pobjDtoEntity.v_Correlativo.Trim();
                        _diarioDto.v_Glosa = "IMPORTACIÓN DE MERCADERÍA";
                        _diarioDto.d_TipoCambio = pobjDtoEntity.d_TipoCambio.Value;
                        _diarioDto.i_IdMoneda = pobjDtoEntity.i_IdMoneda.Value;
                        _diarioDto.i_IdTipoDocumento = (int)LibroDiarios.Importaciones;
                        _diarioDto.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                        _diarioDto.i_IdTipoComprobante = 2;
                        #region Fob

                        string ConceptoFob = ((int)Concepto.ValorFob).ToString();

                        var CuentaDetalleFob = (from n in dbContext.administracionconceptos

                                                where n.v_Codigo == ConceptoFob && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)

                                                select new { n.v_CuentaPVenta }).FirstOrDefault();

                        if (CuentaDetalleFob.v_CuentaPVenta != string.Empty & CuentaDetalleFob.v_CuentaPVenta != null)
                        {

                            foreach (var Proveedor in DetalleImportacionesFOB)
                            {

                                var Cliente = (from n in dbContext.cliente
                                               where n.v_IdCliente == Proveedor.v_IdCliente && n.i_Eliminado == 0 && n.v_FlagPantalla == "V"
                                               select new { n.v_CodCliente }).FirstOrDefault();


                                D_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(Proveedor.d_ValorFob.Value, 2);
                                D_Totales.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(Proveedor.d_ValorFob.Value / Proveedor.d_TipoCambio.Value, 2) : Utils.Windows.DevuelveValorRedondeado(Proveedor.d_ValorFob.Value * Proveedor.d_TipoCambio.Value, 2);
                                D_Totales.i_IdTipoDocumento = (int)LibroDiarios.Importaciones;
                                D_Totales.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                //D_Totales.v_CodAnexo = Cliente.v_CodCliente;
                                D_Totales.v_IdCliente = Proveedor.v_IdCliente;
                                // D_Totales.v_Naturaleza = Proveedor.d_ValorFob > 0 ? pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H" : pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D";
                                D_Totales.v_Naturaleza = Proveedor.d_ValorFob > 0 ? !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "D" : "H" : !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "H" : "D";
                                D_Totales.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                                D_Totales.v_NroCuenta = CuentaDetalleFob.v_CuentaPVenta;
                                D_Totales.i_IdCentroCostos = "";
                                D_Totales.i_IdTipoDocumentoRef = -1;
                                TempDiarioInsertarImportaciones.Add(D_Totales);
                                D_Totales = new diariodetalleDto();

                            }
                        }

                        #endregion

                        #region Flete
                        string ConceptoFlete = ((int)Concepto.Flete).ToString();
                        var CuentaDetalleFlete = (from n in dbContext.administracionconceptos

                                                  where n.v_Codigo == ConceptoFlete && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)

                                                  select new { n.v_CuentaPVenta }).FirstOrDefault();

                        if (pobjDtoEntity.d_Flete > 0 && CuentaDetalleFlete.v_CuentaPVenta != string.Empty && CuentaDetalleFlete.v_CuentaPVenta != null)
                        {
                            var Cliente = (from n in dbContext.cliente
                                           where n.v_IdCliente == pobjDtoEntity.v_IdClienteDoc1 && n.i_Eliminado == 0 && n.v_FlagPantalla == "V"
                                           select new { n.v_CodCliente }).FirstOrDefault();

                            D_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Flete.Value, 2);
                            D_Totales.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Flete.Value / pobjDtoEntity.d_TipoCambioDoc1.Value, 2) : Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Flete.Value * pobjDtoEntity.d_TipoCambioDoc1.Value, 2);
                            D_Totales.i_IdTipoDocumento = (int)LibroDiarios.Importaciones;
                            D_Totales.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                            //D_Totales.v_CodAnexo = Cliente.v_CodCliente;
                            D_Totales.v_IdCliente = pobjDtoEntity.v_IdClienteDoc1;
                            //D_Totales.v_Naturaleza = pobjDtoEntity.d_Flete > 0 ? pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H" : pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D";
                            D_Totales.v_Naturaleza = pobjDtoEntity.d_Flete > 0 ? !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "D" : "H" : !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "H" : "D";
                            D_Totales.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                            D_Totales.v_NroCuenta = CuentaDetalleFlete.v_CuentaPVenta;
                            D_Totales.i_IdCentroCostos = "";
                            D_Totales.i_IdTipoDocumentoRef = -1;
                            TempDiarioInsertarImportaciones.Add(D_Totales);
                            D_Totales = new diariodetalleDto();
                        }
                        #endregion

                        #region Seguro
                        string ConceptoSeguro = ((int)Concepto.Seguro).ToString();
                        var CuentaDetalleSeguro = (from n in dbContext.administracionconceptos

                                                   where n.v_Codigo == ConceptoSeguro && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)

                                                   select new { n.v_CuentaPVenta }).FirstOrDefault();

                        if (pobjDtoEntity.i_PagaSeguro == 1 && CuentaDetalleSeguro.v_CuentaPVenta != string.Empty && CuentaDetalleSeguro.v_CuentaPVenta != null && decimal.Parse(pobjDtoEntity.d_PagoSeguro.ToString()) > 0)
                        {

                            var Cliente = (from n in dbContext.cliente
                                           where n.v_IdCliente == pobjDtoEntity.v_IdClienteDoc2 && n.i_Eliminado == 0 && n.v_FlagPantalla == "V"
                                           select new { n.v_CodCliente }).FirstOrDefault();

                            D_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_PagoSeguro.Value, 2);
                            D_Totales.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_PagoSeguro.Value / pobjDtoEntity.d_TipoCambioDoc2.Value, 2) : Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_PagoSeguro.Value * pobjDtoEntity.d_TipoCambioDoc2.Value, 2);
                            D_Totales.i_IdTipoDocumento = (int)LibroDiarios.Importaciones;
                            D_Totales.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                            //D_Totales.v_CodAnexo = Cliente.v_CodCliente;
                            D_Totales.v_IdCliente = pobjDtoEntity.v_IdClienteDoc2;
                            // D_Totales.v_Naturaleza = pobjDtoEntity.d_PagoSeguro > 0 ? pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H" : pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D";
                            D_Totales.v_Naturaleza = pobjDtoEntity.d_PagoSeguro > 0 ? !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "D" : "H" : !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "H" : "D";
                            D_Totales.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                            D_Totales.v_NroCuenta = CuentaDetalleSeguro.v_CuentaPVenta;
                            D_Totales.i_IdCentroCostos = "";
                            D_Totales.i_IdTipoDocumentoRef = -1;
                            TempDiarioInsertarImportaciones.Add(D_Totales);
                            D_Totales = new diariodetalleDto();

                        }
                        #endregion

                        #region Advaloren
                        string ConceptoAdValorem = ((int)Concepto.AdValorem).ToString();
                        var CuentaDetalleAdValorem = (from n in dbContext.administracionconceptos
                                                      where n.v_Codigo == ConceptoAdValorem && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)
                                                      select new { n.v_CuentaPVenta }).FirstOrDefault();


                        if (pobjDtoEntity.i_AdValorem == 1 && CuentaDetalleAdValorem.v_CuentaPVenta != string.Empty && CuentaDetalleAdValorem.v_CuentaPVenta != null && decimal.Parse(pobjDtoEntity.d_AdValorem.ToString()) > 0)
                        {
                            var Cliente = (from n in dbContext.cliente
                                           where n.v_IdCliente == pobjDtoEntity.v_IdClienteDoc3 && n.i_Eliminado == 0 && n.v_FlagPantalla == "V"
                                           select new { n.v_CodCliente }).FirstOrDefault();

                            D_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_AdValorem.Value, 2);
                            D_Totales.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_AdValorem.Value / pobjDtoEntity.d_TipoCambioDoc3.Value, 2) : Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_AdValorem.Value * pobjDtoEntity.d_TipoCambioDoc3.Value, 2);
                            D_Totales.i_IdTipoDocumento = (int)LibroDiarios.Importaciones;
                            D_Totales.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                            //D_Totales.v_CodAnexo = Cliente.v_CodCliente;
                            D_Totales.v_IdCliente = pobjDtoEntity.v_IdClienteDoc3;
                            // D_Totales.v_Naturaleza = pobjDtoEntity.d_AdValorem > 0 ? pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H" : pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D";
                            D_Totales.v_Naturaleza = pobjDtoEntity.d_AdValorem > 0 ? !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "D" : "H" : !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "H" : "D";
                            D_Totales.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                            D_Totales.v_NroCuenta = CuentaDetalleAdValorem.v_CuentaPVenta;
                            D_Totales.i_IdCentroCostos = "";
                            D_Totales.i_IdTipoDocumentoRef = -1;
                            TempDiarioInsertarImportaciones.Add(D_Totales);
                            D_Totales = new diariodetalleDto();


                        }
                        #endregion
                        #region Igv
                        string ConceptoIgv = ((int)Concepto.Igv).ToString();
                        var CuentaDetalleIgv = (from n in dbContext.administracionconceptos
                                                where n.v_Codigo == ConceptoIgv && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)
                                                select new { n.v_CuentaPVenta }).FirstOrDefault();


                        if (decimal.Parse(pobjDtoEntity.d_Igv.ToString()) > 0 && CuentaDetalleIgv.v_CuentaPVenta != null & CuentaDetalleIgv.v_CuentaPVenta != string.Empty)
                        {
                            var Cliente = (from n in dbContext.cliente
                                           where n.v_IdCliente == pobjDtoEntity.v_IdClienteDoc4 && n.i_Eliminado == 0 && n.v_FlagPantalla == "V"
                                           select new { n.v_CodCliente }).FirstOrDefault();

                            D_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Igv.Value, 2);
                            D_Totales.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Igv.Value / pobjDtoEntity.d_TipoCambioDoc4.Value, 2) : Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Igv.Value * pobjDtoEntity.d_TipoCambioDoc4.Value, 2);
                            D_Totales.i_IdTipoDocumento = (int)LibroDiarios.Importaciones;
                            D_Totales.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                            // D_Totales.v_CodAnexo = Cliente.v_CodCliente;
                            D_Totales.v_IdCliente = pobjDtoEntity.v_IdClienteDoc4;
                            //  D_Totales.v_Naturaleza = pobjDtoEntity.d_Igv > 0 ? pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H" : pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D";
                            D_Totales.v_Naturaleza = pobjDtoEntity.d_Igv > 0 ? !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "D" : "H" : _objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "H" : "D";

                            D_Totales.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                            D_Totales.v_NroCuenta = CuentaDetalleIgv.v_CuentaPVenta;
                            D_Totales.i_IdCentroCostos = "";
                            D_Totales.i_IdTipoDocumentoRef = -1;
                            TempDiarioInsertarImportaciones.Add(D_Totales);
                            D_Totales = new diariodetalleDto();
                        }
                        #endregion

                        #region Percepcion
                        string ConceptoPercepcion = ((int)Concepto.Percepcion).ToString();

                        var CuentaDetallePercepcion = (from n in dbContext.administracionconceptos

                                                       where n.v_Codigo == ConceptoPercepcion && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)

                                                       select new { n.v_CuentaPVenta }).FirstOrDefault();



                        if (decimal.Parse(pobjDtoEntity.d_Percepcion.ToString()) > 0 && CuentaDetallePercepcion.v_CuentaPVenta != null && CuentaDetallePercepcion.v_CuentaPVenta != string.Empty)
                        {
                            var Cliente = (from n in dbContext.cliente
                                           where n.v_IdCliente == pobjDtoEntity.v_IdClienteDoc4 && n.i_Eliminado == 0 && n.v_FlagPantalla == "V"
                                           select new { n.v_CodCliente }).FirstOrDefault();

                            D_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Percepcion.Value, 2);
                            D_Totales.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Percepcion.Value / pobjDtoEntity.d_TipoCambioDoc4.Value, 2) : Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Percepcion.Value * pobjDtoEntity.d_TipoCambioDoc4.Value, 2);
                            D_Totales.i_IdTipoDocumento = (int)LibroDiarios.Importaciones;
                            D_Totales.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;

                            D_Totales.v_IdCliente = pobjDtoEntity.v_IdClienteDoc4;

                            D_Totales.v_Naturaleza = pobjDtoEntity.d_Percepcion > 0 ? !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "D" : "H" : !_objDocumentoBL.DocumentoEsInverso(int.Parse(pobjDtoEntity.i_IdTipoDocumento.Value.ToString())) ? "H" : "D";
                            D_Totales.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;


                            D_Totales.v_NroCuenta = CuentaDetallePercepcion.v_CuentaPVenta;
                            D_Totales.i_IdCentroCostos = "";
                            D_Totales.i_IdTipoDocumentoRef = -1;
                            TempDiarioInsertarImportaciones.Add(D_Totales);
                            D_Totales = new diariodetalleDto();
                        }

                        #endregion
                        #region ProveedoresFob
                        string ConceptoProveedoresFob = ((int)Concepto.ProveedoresFob).ToString();
                        var CuentaDetalleProveedorFob = (from n in dbContext.administracionconceptos

                                                         where n.v_Codigo == ConceptoProveedoresFob && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)

                                                         select new { n.v_CuentaPVenta }).FirstOrDefault();

                        if (CuentaDetalleProveedorFob.v_CuentaPVenta != string.Empty & CuentaDetalleProveedorFob.v_CuentaPVenta != null)
                        {

                            foreach (var Proveedor in DetalleImportacionesFOB)
                            {

                                var Cliente = (from n in dbContext.cliente
                                               where n.v_IdCliente == Proveedor.v_IdCliente && n.i_Eliminado == 0 && n.v_FlagPantalla == "V"
                                               select new { n.v_CodCliente }).FirstOrDefault();

                                H_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(Proveedor.d_ValorFob.Value, 2);
                                H_Totales.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(Proveedor.d_ValorFob.Value / Proveedor.d_TipoCambio.Value, 2) : Utils.Windows.DevuelveValorRedondeado(Proveedor.d_ValorFob.Value * Proveedor.d_TipoCambio.Value, 2);
                                //H_Totales.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;

                                H_Totales.i_IdTipoDocumento = Proveedor.i_IdTipoDocumento.Value;
                                H_Totales.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                //H_Totales.v_CodAnexo = Cliente.v_CodCliente;
                                H_Totales.v_IdCliente = Proveedor.v_IdCliente;
                                //H_Totales.v_Naturaleza = Proveedor.d_ValorFob > 0 ? pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D" : pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H";

                                H_Totales.v_Naturaleza = Proveedor.d_ValorFob > 0 ? !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "H" : "D" : !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "D" : "H";

                                H_Totales.v_NroDocumento = Proveedor.v_SerieDocumento + "-" + Proveedor.v_CorrelativoDocumento;
                                H_Totales.v_NroCuenta = CuentaDetalleProveedorFob.v_CuentaPVenta;
                                H_Totales.i_IdCentroCostos = "";
                                D_Totales.i_IdTipoDocumentoRef = -1;
                                TempDiarioInsertarImportaciones.Add(H_Totales);
                                H_Totales = new diariodetalleDto();

                            }

                        #endregion


                            #region ProveedoresFlete
                            string ConceptoProveedoresFlete = ((int)Concepto.ProveedoresFlete).ToString();
                            var CuentaDetalleProveedoresFlete = (from n in dbContext.administracionconceptos

                                                                 where n.v_Codigo == ConceptoProveedoresFlete && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)

                                                                 select new { n.v_CuentaPVenta }).FirstOrDefault();


                            if (pobjDtoEntity.d_Flete > 0 && CuentaDetalleProveedoresFlete.v_CuentaPVenta != null && CuentaDetalleProveedoresFlete.v_CuentaPVenta != string.Empty)
                            {

                                var Cliente = (from n in dbContext.cliente
                                               where n.i_Eliminado == 0 && n.v_FlagPantalla == "V" && n.v_IdCliente == pobjDtoEntity.v_IdClienteDoc1
                                               select new { n.v_CodCliente }).FirstOrDefault();


                                H_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Flete.Value, 2);
                                H_Totales.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Flete.Value / pobjDtoEntity.d_TipoCambioDoc1.Value, 2) : Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Flete.Value * pobjDtoEntity.d_TipoCambioDoc1.Value, 2);
                                // H_Totales.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                                H_Totales.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento1.Value;
                                H_Totales.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                //H_Totales.v_CodAnexo = Cliente.v_CodCliente;
                                H_Totales.v_IdCliente = pobjDtoEntity.v_IdClienteDoc1;
                                // H_Totales.v_Naturaleza = pobjDtoEntity.d_Flete > 0 ? pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D" : pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H";

                                H_Totales.v_Naturaleza = pobjDtoEntity.d_Flete > 0 ? !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "H" : "D" : !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "D" : "H";
                                H_Totales.v_NroDocumento = pobjDtoEntity.v_SerieDocumento1 + "-" + pobjDtoEntity.v_CorrelativoDocumento1;
                                H_Totales.v_NroCuenta = CuentaDetalleProveedoresFlete.v_CuentaPVenta;
                                H_Totales.i_IdCentroCostos = "";
                                D_Totales.i_IdTipoDocumentoRef = -1;
                                TempDiarioInsertarImportaciones.Add(H_Totales);
                                H_Totales = new diariodetalleDto();
                            }

                            #endregion

                            #region Proveedores Seguro

                            string ConceptoProveedoresSeguro = ((int)Concepto.ProveedoresSeguro).ToString();
                            var CuentaDetalleProveedorSeguro = (from n in dbContext.administracionconceptos

                                                                where n.v_Codigo == ConceptoProveedoresSeguro && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)

                                                                select new { n.v_CuentaPVenta }).FirstOrDefault();
                            if (pobjDtoEntity.i_PagaSeguro == 1 && decimal.Parse(pobjDtoEntity.d_PagoSeguro.ToString()) > 0 && CuentaDetalleProveedorSeguro.v_CuentaPVenta != null && CuentaDetalleProveedorSeguro.v_CuentaPVenta != string.Empty)
                            {

                                var Cliente = (from n in dbContext.cliente
                                               where n.i_Eliminado == 0 && n.v_FlagPantalla == "V" && n.v_IdCliente == pobjDtoEntity.v_IdClienteDoc2
                                               select new { n.v_CodCliente }).FirstOrDefault();

                                H_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_PagoSeguro.Value, 2);
                                H_Totales.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_PagoSeguro.Value / pobjDtoEntity.d_TipoCambioDoc2.Value, 2) : Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_PagoSeguro.Value * pobjDtoEntity.d_TipoCambioDoc2.Value, 2);
                                H_Totales.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento2.Value;
                                // H_Totales.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                                H_Totales.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                //H_Totales.v_CodAnexo = Cliente.v_CodCliente;
                                H_Totales.v_IdCliente = pobjDtoEntity.v_IdClienteDoc2;
                                // H_Totales.v_Naturaleza = pobjDtoEntity.d_PagoSeguro > 0 ? pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D" : pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H";

                                H_Totales.v_Naturaleza = pobjDtoEntity.d_PagoSeguro > 0 ? !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "H" : "D" : !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "D" : "H";

                                H_Totales.v_NroDocumento = pobjDtoEntity.v_SerieDocumento2 + "-" + pobjDtoEntity.v_CorrelativoDocumento2;
                                H_Totales.v_NroCuenta = CuentaDetalleProveedorSeguro.v_CuentaPVenta;
                                H_Totales.i_IdCentroCostos = "";
                                D_Totales.i_IdTipoDocumentoRef = -1;
                                TempDiarioInsertarImportaciones.Add(H_Totales);
                                H_Totales = new diariodetalleDto();

                            }

                            #endregion

                            #region ProveedoresAdvalorem

                            string ConceptoProveedoresAdvalorem = ((int)Concepto.ProveedoresAdvalorem).ToString();
                            var CuentaDetalleProveedorAdvalorem = (from n in dbContext.administracionconceptos

                                                                   where n.v_Codigo == ConceptoProveedoresAdvalorem && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)
                                                                   select new { n.v_CuentaPVenta }).FirstOrDefault();

                            if (pobjDtoEntity.i_AdValorem == 1 && decimal.Parse(pobjDtoEntity.d_AdValorem.ToString()) > 0 && CuentaDetalleProveedorAdvalorem.v_CuentaPVenta != null && CuentaDetalleProveedorAdvalorem.v_CuentaPVenta != string.Empty)
                            {
                                var Cliente = (from n in dbContext.cliente
                                               where n.i_Eliminado == 0 && n.v_FlagPantalla == "V" && n.v_IdCliente == pobjDtoEntity.v_IdClienteDoc3
                                               select new { n.v_CodCliente }).FirstOrDefault();

                                H_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_AdValorem.Value, 2);
                                H_Totales.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_AdValorem.Value / pobjDtoEntity.d_TipoCambioDoc3.Value, 2) : Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_AdValorem.Value * pobjDtoEntity.d_TipoCambioDoc3.Value, 2);
                                //H_Totales.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                                H_Totales.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento3.Value;
                                H_Totales.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                //H_Totales.v_CodAnexo = Cliente.v_CodCliente;
                                H_Totales.v_IdCliente = pobjDtoEntity.v_IdClienteDoc3;
                                //H_Totales.v_Naturaleza = pobjDtoEntity.d_AdValorem > 0 ? pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D" : pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H";
                                H_Totales.v_Naturaleza = pobjDtoEntity.d_AdValorem > 0 ? !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "H" : "D" : !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "D" : "H";
                                H_Totales.v_NroDocumento = pobjDtoEntity.v_SerieDocumento3 + "-" + pobjDtoEntity.v_CorrelativoDocumento3;
                                H_Totales.v_NroCuenta = CuentaDetalleProveedorAdvalorem.v_CuentaPVenta;
                                H_Totales.i_IdCentroCostos = "";
                                D_Totales.i_IdTipoDocumentoRef = -1;
                                TempDiarioInsertarImportaciones.Add(H_Totales);
                                H_Totales = new diariodetalleDto();

                            }

                            #endregion

                            #region Proveedores Igv
                            string ConceptoProveedoresIgv = ((int)Concepto.ProveedorIgv).ToString();
                            var CuentaDetalleProveedorIgv = (from n in dbContext.administracionconceptos

                                                             where n.v_Codigo == ConceptoProveedoresIgv && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)
                                                             select new { n.v_CuentaPVenta }).FirstOrDefault();

                            if (decimal.Parse(pobjDtoEntity.d_Igv.ToString()) > 0 && CuentaDetalleProveedorIgv.v_CuentaPVenta != null && CuentaDetalleProveedorIgv.v_CuentaPVenta != string.Empty)
                            {

                                var Cliente = (from n in dbContext.cliente
                                               where n.i_Eliminado == 0 && n.v_FlagPantalla == "V" && n.v_IdCliente == pobjDtoEntity.v_IdClienteDoc4
                                               select new { n.v_CodCliente }).FirstOrDefault();

                                H_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Igv.Value, 2);
                                H_Totales.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Igv.Value / pobjDtoEntity.d_TipoCambioDoc4.Value, 2) : Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Igv.Value * pobjDtoEntity.d_TipoCambioDoc4.Value, 2);
                                // H_Totales.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                                H_Totales.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento4.Value;
                                H_Totales.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                //H_Totales.v_CodAnexo = Cliente.v_CodCliente;
                                H_Totales.v_IdCliente = pobjDtoEntity.v_IdClienteDoc4;
                                // H_Totales.v_Naturaleza = pobjDtoEntity.d_Igv > 0 ? pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D" : pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H";
                                H_Totales.v_Naturaleza = pobjDtoEntity.d_Igv > 0 ? !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "H" : "D" : !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "D" : "H";
                                H_Totales.v_NroDocumento = pobjDtoEntity.v_SerieDocumento4 + "-" + pobjDtoEntity.v_CorrelativoDoc4;
                                H_Totales.v_NroCuenta = CuentaDetalleProveedorIgv.v_CuentaPVenta;
                                H_Totales.i_IdCentroCostos = "";
                                D_Totales.i_IdTipoDocumentoRef = -1;
                                TempDiarioInsertarImportaciones.Add(H_Totales);
                                H_Totales = new diariodetalleDto();
                            }
                            #endregion

                            #region ProveedorPercepcion
                            string ConceptoProveedoresPercepcion = ((int)Concepto.ProveedorPercepcion).ToString();

                            var CuentaDetalleProveedoresPercepcion = (from n in dbContext.administracionconceptos

                                                                      where n.v_Codigo == ConceptoProveedoresPercepcion && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)

                                                                      select new { n.v_CuentaPVenta }).FirstOrDefault();



                            if (decimal.Parse(pobjDtoEntity.d_Percepcion.ToString()) > 0 && CuentaDetalleProveedoresPercepcion.v_CuentaPVenta != string.Empty && CuentaDetalleProveedoresPercepcion.v_CuentaPVenta != null)
                            {

                                var Cliente = (from n in dbContext.cliente
                                               where n.i_Eliminado == 0 && n.v_FlagPantalla == "V" && n.v_IdCliente == pobjDtoEntity.v_IdClienteDoc4
                                               select new { n.v_CodCliente }).FirstOrDefault();

                                H_Totales.d_Importe = Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Percepcion.Value, 2);
                                H_Totales.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Percepcion.Value / pobjDtoEntity.d_TipoCambioDoc4.Value, 2) : Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.d_Percepcion.Value * pobjDtoEntity.d_TipoCambioDoc4.Value, 2);
                                H_Totales.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento4.Value;
                                H_Totales.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                H_Totales.v_IdCliente = pobjDtoEntity.v_IdClienteDoc4;
                                H_Totales.v_Naturaleza = pobjDtoEntity.d_Percepcion > 0 ? !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "H" : "D" : !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "D" : "H";
                                // H_Totales.v_NroDocumento = _diarioDto.v_Mes + "-" + _diarioDto.v_Correlativo;
                                H_Totales.v_NroDocumento = pobjDtoEntity.v_SerieDocumento4 + "-" + pobjDtoEntity.v_CorrelativoDoc4;
                                H_Totales.v_NroCuenta = CuentaDetalleProveedoresPercepcion.v_CuentaPVenta;
                                H_Totales.i_IdCentroCostos = "";
                                D_Totales.i_IdTipoDocumentoRef = -1;
                                TempDiarioInsertarImportaciones.Add(H_Totales);
                                H_Totales = new diariodetalleDto();
                            }

                            #endregion
                        }


                        decimal TotDebe, TotHaber, TotDebeC, TotHaberC;
                        TotDebe = TempDiarioInsertarImportaciones.Where(p => p.v_Naturaleza == "D" && p.v_NroCuenta != string.Empty).Sum(o => o.d_Importe.Value);
                        TotHaber = TempDiarioInsertarImportaciones.Where(p => p.v_Naturaleza == "H" && p.v_NroCuenta != string.Empty).Sum(o => o.d_Importe.Value);
                        TotDebeC = TempDiarioInsertarImportaciones.Where(p => p.v_Naturaleza == "D" && p.v_NroCuenta != string.Empty).Sum(o => o.d_Cambio.Value);
                        TotHaberC = TempDiarioInsertarImportaciones.Where(p => p.v_Naturaleza == "H" && p.v_NroCuenta != string.Empty).Sum(o => o.d_Cambio.Value);
                        _diarioDto.d_TotalDebe = Utils.Windows.DevuelveValorRedondeado(TotDebe, 2);
                        _diarioDto.d_TotalHaber = Utils.Windows.DevuelveValorRedondeado(TotHaber, 2);
                        _diarioDto.d_TotalDebeCambio = Utils.Windows.DevuelveValorRedondeado(TotDebeC, 2);
                        _diarioDto.d_TotalHaberCambio = Utils.Windows.DevuelveValorRedondeado(TotHaberC, 2);
                        _diarioDto.d_DiferenciaDebe = Utils.Windows.DevuelveValorRedondeado(TotDebe - TotHaber, 2);
                        _diarioDto.d_DiferenciaHaber = Utils.Windows.DevuelveValorRedondeado(TotDebeC - TotHaberC, 2);
                        if (TempDiarioInsertarImportaciones.Where(a => a.v_NroCuenta != String.Empty).ToList().Any())
                        {
                            _objDiarioBL.InsertarDiario(ref pobjOperationResult, _diarioDto, Globals.ClientSession.GetAsList(), TempDiarioInsertarImportaciones.Where(a => a.v_NroCuenta != String.Empty).ToList(), (int)TipoMovimientoTesoreria.Egreso);
                        }
                        if (pobjOperationResult.Success == 0) return string.Empty;
                    }

                    //}
                    #endregion

                    #region AnuladaCompras   // AnulaCompras , si la Importaciòn fue anulada
                    if (pobjDtoEntity.i_IdEstado == 0 && !pTemp_ModificarDetalleGastos.Any())
                    {
                        var ListaDetalleGastos = dbContext.importaciondetallegastos
                            .Where(a => a.i_Eliminado == 0 && a.v_IdImportacion == pobjDtoEntity.v_IdImportacion).ToList();
                        foreach (var item in ListaDetalleGastos)
                        {
                            _compraDto = new compraDto();
                            var Compritas = (from n in dbContext.compra
                                             where n.v_IdDocumentoReferencia == item.v_IdImportacionDetalleGastos
                                             select n).FirstOrDefault().ToDTO();

                            _compraDto = Compritas;
                            _compraDto.i_IdEstado = 0;

                            List<compradetalleDto> ListaAgregar = new List<compradetalleDto>();
                            List<compradetalleDto> ListaEliminar = new List<compradetalleDto>();
                            List<compradetalleDto> ListaModificar = new List<compradetalleDto>();
                            _objComprasBL.ActualizarCompra(ref pobjOperationResult, _compraDto, Globals.ClientSession.GetAsList(), ListaAgregar, ListaModificar, ListaEliminar);
                            if (pobjOperationResult.Success == 0) return string.Empty;
                        }
                    }
                    #endregion


                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                    Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "importacion", pobjDtoEntity.v_IdImportacion);
                    ts.Complete();
                    return pobjDtoEntity.v_IdImportacion;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ImportacionBL.ActualizarImportacion()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public bool ConsultaSiTieneTesoreriaDetalleGastos(string pstrIdImportacion)
        {
            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
            bool TieneTesoreria = false;
            var DetalleGastos = (from n in dbContext.importaciondetallegastos
                                 join a in dbContext.compra on n.v_IdImportacionDetalleGastos equals a.v_IdDocumentoReferencia
                                 where n.v_IdImportacion == pstrIdImportacion && n.i_Eliminado == 0

                                 select a).ToList();
            foreach (var Fila in DetalleGastos)
            {
                if (ConsultarSiTieneTesorerias(NroDiario(Fila.v_IdCompra)))
                {
                    TieneTesoreria = true;
                }

            }

            return TieneTesoreria;
        }

        public string NroDiario(string pstrDocumentoReferencia)
        {
            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();


            var NroDiario = (from n in dbContext.diario
                             where n.i_Eliminado == 0 && n.v_IdDocumentoReferencia == pstrDocumentoReferencia
                             select n).FirstOrDefault();
            if (NroDiario != null)
            {
                return NroDiario.v_IdDiario;
            }
            else
            {
                return string.Empty;
            }

        }
        public bool ConsultarSiTieneTesorerias(string psrtIdDiario)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    if (psrtIdDiario != string.Empty)
                    {


                        var Pendiente = (from v in dbContext.pendientecobrardetalle
                                         where v.v_IdDiario == psrtIdDiario
                                         select new { v.v_IdPendienteCobrar }).FirstOrDefault();

                        //string IdPendienteCobrar = (from v in dbContext.pendientecobrardetalle
                        //                            where v.v_IdDiario == psrtIdDiario
                        //                            select new { v.v_IdPendienteCobrar }).FirstOrDefault().v_IdPendienteCobrar;

                        if (Pendiente != null)
                        {
                            string IdPendienteCobrar = Pendiente.v_IdPendienteCobrar;

                            pendientecobrar Entidad = (from e in dbContext.pendientecobrar
                                                       where e.v_IdPendienteCobrar == IdPendienteCobrar
                                                       select e).FirstOrDefault();

                            if (Entidad.pendientecobrardetalle.ToList().Where(p => p.v_IdTesoreria != null).Count() != 0)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {

                            return false;
                        }

                    }
                    else
                    {

                        return false;

                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void EliminarImportacion(ref OperationResult pobjOperationResult, string pstrIdImportacion, List<string> ClientSession)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {


                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                    DiarioBL _objDiarioBL = new DiarioBL();
                    ComprasBL _objComprasBL = new ComprasBL();
                    #region Elimina Cabecera
                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.importacion
                                           where a.v_IdImportacion == pstrIdImportacion
                                           select a).FirstOrDefault();

                    // Crear la entidad con los datos actualizados
                    objEntitySource.t_ActualizaFecha = DateTime.Now;
                    objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntitySource.i_Eliminado = 1;
                    #endregion

                    #region Elimina Detalles
                    //Eliminar detalles de la importación
                    var objEntitySourceImportacionDetalleFob = (from a in dbContext.importaciondetallefob
                                                                where a.v_IdImportacion == pstrIdImportacion & a.i_Eliminado == 0
                                                                select a).ToList();

                    foreach (var Importaciondetallefob in objEntitySourceImportacionDetalleFob)
                    {
                        Importaciondetallefob.t_ActualizaFecha = DateTime.Now;
                        Importaciondetallefob.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        Importaciondetallefob.i_Eliminado = 1;
                        Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "importacionDetalleFob", Importaciondetallefob.v_IdImportacionDetalleFob);
                    }

                    var objEntitySourceImportacionDetalleProductos = (from a in dbContext.importaciondetalleproducto
                                                                      where a.v_IdImportacion == pstrIdImportacion & a.i_Eliminado == 0
                                                                      select a).ToList();

                    foreach (var Importaciondetalleproductos in objEntitySourceImportacionDetalleProductos)
                    {
                        Importaciondetalleproductos.t_ActualizaFecha = DateTime.Now;
                        Importaciondetalleproductos.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        Importaciondetalleproductos.i_Eliminado = 1;
                        Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "importacionDetalleProducto", Importaciondetalleproductos.v_IdImportacionDetalleProducto);
                    }

                    var objEntitySourceImportacionDetalleGastos = (from a in dbContext.importaciondetallegastos
                                                                   where a.v_IdImportacion == pstrIdImportacion && a.i_Eliminado == 0
                                                                   select a).ToList();

                    foreach (var Importaciondetallegastos in objEntitySourceImportacionDetalleGastos)
                    {

                        Importaciondetallegastos.t_ActualizaFecha = DateTime.Now;
                        Importaciondetallegastos.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        Importaciondetallegastos.i_Eliminado = 1;
                        Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "importacionDetalleGastos", Importaciondetallegastos.v_IdImportacionDetalleGastos);
                        _objComprasBL.EliminarComprasXDocRef(ref pobjOperationResult, Importaciondetallegastos.v_IdImportacionDetalleGastos, ClientSession, true);
                        if (pobjOperationResult.Success == 0) return;
                    }


                    #endregion

                    #region Elimina Notas de Ingresos Relacionadas
                    MovimientoBL objMovimientoBL = new MovimientoBL();

                    var NotasDeIngresos = (from a in dbContext.movimiento
                                           where a.v_OrigenTipo == "I" && a.v_OrigenRegPeriodo == objEntitySource.v_Periodo && a.v_OrigenRegMes == objEntitySource.v_Mes
                                           && a.v_OrigenRegCorrelativo == objEntitySource.v_Correlativo
                                           select new { a.v_IdMovimiento }).ToList();

                    foreach (var NotadeIngreso in NotasDeIngresos)
                    {
                        objMovimientoBL.EliminarMovimiento(ref pobjOperationResult, NotadeIngreso.v_IdMovimiento, ClientSession);
                        if (pobjOperationResult.Success == 0) return;
                    }
                    #endregion

                    #region Elimina Libro Diario

                    //_objDiarioBL.EliminarDiarioXDocRef(ref objOperationResult, pstrIdImportacion, ClientSession);
                    _objDiarioBL.EliminarDiarioXDocRef(ref pobjOperationResult, pstrIdImportacion, ClientSession, true);
                    if (pobjOperationResult.Success == 0) return;
                    #endregion

                    #region Actualiza Orden Compra

                    if (objEntitySource.v_IdDocumentoReferencia != null)
                    {
                        var oc = (from a in dbContext.ordendecompra
                                  where a.v_IdOrdenCompra == objEntitySource.v_IdDocumentoReferencia
                                  select a).FirstOrDefault();
                        if (oc != null)
                        {
                            oc.i_IdEstado = 1;
                            dbContext.ordendecompra.ApplyCurrentValues(oc);
                        }
                         
                    
                    
                    }
                    #endregion
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                    Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "importacion", pstrIdImportacion);
                    ts.Complete();
                    return;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return;
            }
        }
        public List<GridKeyValueDTO> ObtenGastosImportacionParaComboGrid(ref OperationResult pobjOperationResult)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                var query = (from a in dbContext.gastosimportacion

                             where a.i_Eliminado == 0

                             select a);

                query = query.OrderBy("v_Codigo");

                var query2 = query.AsEnumerable()
                            .Select(x => new GridKeyValueDTO
                            {
                                Id = x.v_Codigo.ToString(),
                                Value1 = x.v_Nombre,
                                Value2 = x.v_Cuenta,

                            }).ToList();

                pobjOperationResult.Success = 1;
                return query2;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }
        public bool ValidarCuenta(ref OperationResult pobjOperationResult, string pstrNumeroCuenta)
        {

            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = from A in dbContext.asientocontable
                            where A.v_NroCuenta == pstrNumeroCuenta && A.i_Eliminado == 0 & A.i_Imputable == 1 && A.v_Periodo == periodo
                            select new asientocontableDto
                            {
                                v_NroCuenta = A.v_NroCuenta
                            };

                if (query.Count() == 0)
                {
                    return false;
                }
                else
                {
                    return true;

                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return false;
            }
        }
        public List<string[]> DevolverNombreProveedores(string pstrIdImportacion)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                var query = DevuelveNombresProveedores(dbContext, pstrIdImportacion);

                if (query != null)
                {
                    List<string[]> Lista = new List<string[]>();
                    foreach (var Fila in query)
                    {
                        string[] Cadena = new string[3];
                        Cadena[0] = Fila.NombreProveedor;
                        Cadena[1] = Fila.CodigoProveedor;

                        Lista.Add(Cadena);
                    }
                    return Lista;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<string[]> DevolverNombreProveedoresGastos(string pstrIdImportacion)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                var query = DevuelveNombresProveedoresGastos(dbContext, pstrIdImportacion);

                if (query != null)
                {
                    List<string[]> Lista = new List<string[]>();
                    foreach (var Fila in query)
                    {
                        string[] Cadena = new string[3];
                        Cadena[0] = Fila.NombreProveedor;
                        Cadena[1] = Fila.CodigoProveedor;

                        Lista.Add(Cadena);
                    }
                    return Lista;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<string[]> DevolverNombreGrillaProductos(string pstrIdImportacion)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                var query = DevuelveNombresGrillaProductos(dbContext, pstrIdImportacion);

                if (query != null)
                {
                    List<string[]> Lista = new List<string[]>();
                    foreach (var Fila in query)
                    {
                        string[] Cadena = new string[7];
                        Cadena[0] = Fila.CodigoProducto;
                        Cadena[1] = Fila.NombreProducto;
                        Cadena[2] = Fila.CodigoProveedor;
                        Cadena[3] = Fila.EsServicio.Value.ToString();
                        Cadena[4] = Fila.d_Empaque.Value.ToString();
                        Cadena[5] = Fila.UmEmpaque;
                        Cadena[6] = Fila.i_IdUnidadMedidaProducto.Value.ToString();

                        Lista.Add(Cadena);
                    }
                    return Lista;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public string[] DevolverProveedorPorNroDocumento(ref OperationResult pobjOperationResult, string NroDocumento)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = (from n in dbContext.cliente
                             where n.i_Eliminado == 0 && n.v_FlagPantalla == "V" && n.v_NroDocIdentificacion == NroDocumento
                             select n
                             ).FirstOrDefault();

                pobjOperationResult.Success = 1;

                if (query != null)
                {
                    string[] Cadena = new string[4];
                    Cadena[0] = query.v_IdCliente;
                    Cadena[1] = query.v_CodCliente;
                    Cadena[2] = (query.v_PrimerNombre + " " + query.v_ApePaterno + " " + query.v_ApeMaterno + " " + query.v_RazonSocial).Trim();
                    Cadena[3] = query.v_NroDocIdentificacion.Trim();
                    return Cadena;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public string[] DevolverProveedorPorCodigo(ref OperationResult pobjOperationResult, string pstrCodigo)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = (from n in dbContext.cliente
                             where n.i_Eliminado == 0 && n.v_FlagPantalla == "V" && n.v_CodCliente == pstrCodigo
                             select n
                             ).FirstOrDefault();

                pobjOperationResult.Success = 1;

                if (query != null)
                {
                    string[] Cadena = new string[4];
                    Cadena[0] = query.v_IdCliente;
                    Cadena[1] = query.v_CodCliente;
                    Cadena[2] = (query.v_PrimerNombre + " " + query.v_ApePaterno + " " + query.v_ApeMaterno + " " + query.v_RazonSocial).Trim();
                    Cadena[3] = query.v_NroDocIdentificacion.Trim();
                    return Cadena;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }


        public string ObtenerSiglasDocumento(int Codigo)
        {
            string codigo;
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                var query = (from a in dbContext.documento
                             where a.i_Eliminado == 0 && a.i_CodigoDocumento == Codigo && (a.i_UsadoCompras == 1 || a.i_UsadoVentas == 1)
                             select a).Distinct().FirstOrDefault();

                if (query != null)
                {
                    codigo = query.v_Siglas;
                    return codigo;

                }
                else
                {

                    return "--Seleccionar--";

                }
            }
            catch (Exception e)
            {

                throw e;
            }

        }

        public bool ValidarNumeroRegistro(int TipoDoc, int serie, string correlativodoc, string IdImportacion)
        {

            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                importacionDto registro = new importacionDto();

                if (IdImportacion == null)
                {
                    var registroT = (from a in dbContext.importacion
                                     where a.i_Eliminado == 0 && a.i_IdTipoDocumento == TipoDoc && a.i_IdSerieDocumento == serie && a.v_CorrelativoDocumento == correlativodoc

                                     select a).FirstOrDefault();
                    registro = importacionAssembler.ToDTO(registroT);


                }
                else
                {
                    var registroT = (from a in dbContext.importacion
                                     where a.i_Eliminado == 0 && a.i_IdTipoDocumento == TipoDoc && a.i_IdSerieDocumento == serie && a.v_CorrelativoDocumento == correlativodoc
                                     && a.v_IdImportacion != IdImportacion
                                     select a).FirstOrDefault();
                    registro = importacionAssembler.ToDTO(registroT);

                }



                if (registro != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }


        #region QueryCompilados
        public static Func<SAMBHSEntitiesModelWin, string, IQueryable<SAMBHS.Common.BE.importacionDto.CompiladoResultImportacion>>
                    DevuelveNombresProveedores = CompiledQuery.Compile((SAMBHSEntitiesModelWin db, string ID) =>
                    from n in db.importaciondetallefob
                    join A in db.cliente on n.v_IdCliente equals A.v_IdCliente into A_join
                    from A in A_join.DefaultIfEmpty()



                    where n.v_IdImportacion == ID && n.i_Eliminado == 0 && A.v_FlagPantalla == "V"

                    select new SAMBHS.Common.BE.importacionDto.CompiladoResultImportacion
                    {
                        NombreProveedor = (A.v_PrimerNombre + " " + A.v_ApePaterno + " " + A.v_ApeMaterno + " " + A.v_RazonSocial).Trim(),
                        CodigoProveedor = A.v_CodCliente,
                    }
                    );

        public static Func<SAMBHSEntitiesModelWin, string, IQueryable<SAMBHS.Common.BE.importacionDto.CompiladoResultImportacion>>
                    DevuelveNombresProveedoresGastos = CompiledQuery.Compile((SAMBHSEntitiesModelWin db, string ID) =>
                    from n in db.importaciondetallegastos
                    join A in db.cliente on n.v_Detalle equals A.v_IdCliente into A_join
                    from A in A_join.DefaultIfEmpty()
                    where n.v_IdImportacion == ID && n.i_Eliminado == 0 && A.v_FlagPantalla == "V"
                    select new SAMBHS.Common.BE.importacionDto.CompiladoResultImportacion
                    {
                        NombreProveedor = (A.v_PrimerNombre + " " + A.v_ApePaterno + " " + A.v_ApeMaterno + " " + A.v_RazonSocial).Trim(),
                        CodigoProveedor = A.v_CodCliente,
                    }
                    );

        public static Func<SAMBHSEntitiesModelWin, string, IQueryable<SAMBHS.Common.BE.importacionDto.CompiladoResultImportacion>>
                   DevuelveNombresGrillaProductos = CompiledQuery.Compile((SAMBHSEntitiesModelWin db, string ID) =>
                   from n in db.importaciondetalleproducto
                   join A in db.productodetalle on n.v_IdProductoDetalle equals A.v_IdProductoDetalle into A_join
                   from A in A_join.DefaultIfEmpty()
                   join B in db.producto on A.v_IdProducto equals B.v_IdProducto into B_join
                   from B in B_join.DefaultIfEmpty()
                   join C in db.cliente on n.v_IdCliente equals C.v_IdCliente into C_join
                   from C in C_join.DefaultIfEmpty()

                   join D in db.datahierarchy on new { a = B.i_IdUnidadMedida.Value, b = 17 }
                                                      equals new { a = D.i_ItemId, b = D.i_GroupId } into D_join
                   from D in D_join.DefaultIfEmpty()


                   where n.i_Eliminado == 0 && n.v_IdImportacion == ID
                   //orderby n.v_IdImportacionDetalleProducto 
                   select new SAMBHS.Common.BE.importacionDto.CompiladoResultImportacion
                   {

                       CodigoProducto = B.v_CodInterno,
                       NombreProducto = B.v_Descripcion,
                       CodigoProveedor = C.v_CodCliente,
                       EsServicio = B.i_EsServicio,
                       d_Empaque = B.d_Empaque,
                       UmEmpaque = D.v_Value1,
                       i_IdUnidadMedidaProducto = n.i_IdUnidadMedida,

                   }
                   );



        #endregion

        #endregion

        #region Bandeja


        public List<importacionDto> ListarBusquedaImportacion(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression, DateTime F_Inicio, DateTime F_Fin)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();



                var Fob = (from a in dbContext.importaciondetallefob
                           join b in dbContext.cliente on new { cl = a.v_IdCliente, eliminado = 0 } equals new { cl = b.v_IdCliente, eliminado = b.i_Eliminado.Value } into b_join
                           from b in b_join.DefaultIfEmpty()
                           where a.i_Eliminado == 0
                           select new importaciondetallefobDto
                           {
                               v_IdImportacion = a.v_IdImportacion,
                               v_IdCliente = b.v_IdCliente,
                               NombreProveedorFOB = (b.v_ApePaterno + " " + b.v_PrimerNombre + " " + b.v_SegundoNombre + b.v_RazonSocial).Trim()
                           }).ToList();
                var query = (from n in dbContext.importacion

                             join A in dbContext.documento on n.i_IdTipoDocumento equals A.i_CodigoDocumento into A_join
                             from A in A_join.DefaultIfEmpty()


                             join J2 in dbContext.systemuser on new { i_UpdateUserId = n.i_ActualizaIdUsuario.Value }
                                                            equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                             from J2 in J2_join.DefaultIfEmpty()

                             join J3 in dbContext.systemuser on new { i_InsertUserId = n.i_InsertaIdUsuario.Value }
                                                            equals new { i_InsertUserId = J3.i_SystemUserId } into J3_join
                             from J3 in J3_join.DefaultIfEmpty()

                             join J4 in dbContext.datahierarchy on new { i_IdTipoVia = n.i_IdTipoVia.Value, b = 49 }
                                                                equals new { i_IdTipoVia = J4.i_ItemId, b = J4.i_GroupId } into J4_join
                             from J4 in J4_join.DefaultIfEmpty()

                             join J5 in dbContext.almacen on n.i_IdAlmacen equals J5.i_IdAlmacen into J5_join
                             from J5 in J5_join.DefaultIfEmpty()

                             join J6 in dbContext.datahierarchy on new { i_idMoneda = n.i_IdMoneda.Value, b = 17 }
                                                    equals new { i_idMoneda = J6.i_ItemId, b = J6.i_GroupId } into J6_join
                             from J6 in J6_join.DefaultIfEmpty()

                             join J7 in dbContext.datahierarchy on new { i_idSerieDoc = n.i_IdSerieDocumento.Value, b = 53 }
                                                    equals new { i_idSerieDoc = J7.i_ItemId, b = J7.i_GroupId } into J7_join
                             from J7 in J7_join

                             join J8 in dbContext.documento on new { doc = n.i_IdTipoDocRerefencia ?? -1, eliminado = 0 }
                                                              equals new { doc = J8.i_CodigoDocumento, eliminado = J8.i_Eliminado.Value } into J8_join
                             from J8 in J8_join.DefaultIfEmpty()
                             where n.i_Eliminado == 0 && n.t_FechaRegistro >= F_Inicio && n.t_FechaRegistro <= F_Fin

                             select new importacionDto
                             {
                                 NroRegistro = n.v_Mes.Trim() + "-" + n.v_Correlativo.Trim(),
                                 i_IdEstado = n.i_IdEstado,
                                 t_FechaEmision = n.t_FechaEmision,
                                 t_FechaLLegada = n.t_FechaLLegada,
                                 t_FechaPagoVencimiento = n.t_FechaPagoVencimiento,
                                 i_IdTipoDocumento = n.i_IdTipoDocumento,
                                 TipoDocumento = A.v_Siglas,
                                 TipoVia = J4.v_Value1,
                                 i_IdTipoVia = n.i_IdTipoVia,
                                 i_IdSerieDocumento = n.i_IdSerieDocumento,
                                 v_CorrelativoDocumento = n.v_CorrelativoDocumento,
                                 i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                 i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                 v_UsuarioModificacion = J2.v_UserName,
                                 v_UsuarioCreacion = J3.v_UserName,
                                 t_ActualizaFecha = n.t_ActualizaFecha,
                                 t_InsertaFecha = n.t_InsertaFecha,
                                 v_IdImportacion = n.v_IdImportacion,
                                 Documento = J7.v_Value2.Trim() + "-" + n.v_CorrelativoDocumento.Trim(),
                                 t_FechaRegistro = n.t_FechaRegistro,
                                 Almacen = J5.v_Nombre,
                                 Moneda = n.i_IdMoneda == 1 ? "S/." : "US$.",
                                 i_IdMoneda = n.i_IdMoneda,
                                 i_IdAlmacen = n.i_IdAlmacen,
                                 v_Mes = n.v_Mes,
                                 v_Correlativo = n.v_Correlativo,
                                 v_Periodo = n.v_Periodo,
                                 i_IdEstablecimiento = n.i_IdEstablecimiento.Value,
                                 NroDocumentoReferencia = J8 == null ? "" : J8.v_Siglas + " " + n.v_NumeroDocRerefencia,


                             });

                if (!string.IsNullOrEmpty(pstrFilterExpression))
                {
                    query = query.Where(pstrFilterExpression);
                }
                if (!string.IsNullOrEmpty(pstrSortExpression))
                {
                    query = query.OrderBy(pstrSortExpression);
                }

                List<importacionDto> objData = query.ToList();



                objData.AsParallel().ToList().ForEach(importacion =>
                {
                    IEnumerable<string> ListaProv = Fob.Where(o => o.v_IdImportacion == importacion.v_IdImportacion).Select(o => o.NombreProveedorFOB).AsEnumerable();
                    importacion.ProveedoresVistaRapida = ListaProv.Any() && ListaProv != null ? string.Join(", ", ListaProv) : "";

                });




                pobjOperationResult.Success = 1;
                return objData;


            }
            catch (Exception e)
            {

                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(e);
                return null;

            }




        }


        #endregion

        #region Reportes

        public List<ReporteImportaciones> ReporteDocumentoImportaciones(ref OperationResult objOperationResult, string pstrIdImportaciones, int MonedaImpresion)
        {



            try
            {
                objOperationResult.Success = 1;
                using (SAMBHSEntitiesModelWin dbcontext = new SAMBHSEntitiesModelWin())
                {
                    List<ReporteImportaciones> objData = new List<ReporteImportaciones>();
                    if (dbcontext.importacion.Where(o => o.v_IdImportacion == pstrIdImportaciones).FirstOrDefault().i_IdMoneda == MonedaImpresion)
                    {

                        var Importacion = (from n in dbcontext.importacion

                                           join e in dbcontext.importaciondetalleproducto on new { i = n.v_IdImportacion, eliminado = 0 } equals new { i = e.v_IdImportacion, eliminado = e.i_Eliminado.Value } into e_join

                                           from e in e_join.DefaultIfEmpty()

                                           join a in dbcontext.documento on new { td = n.i_IdTipoDocumento.Value, eliminado = 0 } equals new { td = a.i_CodigoDocumento, eliminado = a.i_Eliminado.Value } into a_join
                                           from a in a_join.DefaultIfEmpty()

                                           join b in dbcontext.datahierarchy on new { y = n.i_IdMoneda.Value, x = 18, eliminado = 0 } //Moneda
                                                             equals new { y = b.i_ItemId, x = b.i_GroupId, eliminado = b.i_IsDeleted.Value } into b_join
                                           from b in b_join.DefaultIfEmpty()

                                           join c in dbcontext.datahierarchy on new { y = n.i_IdTipoVia.Value, x = 49, eliminado = 0 } //Via
                                                             equals new { y = c.i_ItemId, x = c.i_GroupId, eliminado = c.i_IsDeleted.Value } into c_join
                                           from c in c_join.DefaultIfEmpty()

                                           join d in dbcontext.datahierarchy on new { y = n.i_IdEstado.Value, x = 30, eliminado = 0 }
                                                           equals new { y = d.i_ItemId, x = d.i_GroupId, eliminado = d.i_IsDeleted.Value } into d_join
                                           from d in d_join.DefaultIfEmpty()

                                           join f in dbcontext.cliente on new { c = e.v_IdCliente, eliminado = 0 } equals new { c = f.v_IdCliente, eliminado = f.i_Eliminado.Value } into f_join

                                           from f in f_join.DefaultIfEmpty()

                                           join g in dbcontext.productodetalle on new { pd = e.v_IdProductoDetalle, eliminado = 0 } equals new { pd = g.v_IdProductoDetalle, eliminado = g.i_Eliminado.Value } into g_join

                                           from g in g_join.DefaultIfEmpty()

                                           join h in dbcontext.producto on new { p = g.v_IdProducto, eliminado = 0 } equals new { p = h.v_IdProducto, eliminado = h.i_Eliminado.Value } into h_join

                                           from h in h_join.DefaultIfEmpty()

                                           join i in dbcontext.datahierarchy on new { y = e.i_IdUnidadMedida.Value, x = 17, eliminado = 0 }
                                                                          equals new { y = i.i_ItemId, x = i.i_GroupId, eliminado = i.i_IsDeleted.Value }
                                           join j in dbcontext.datahierarchy on new { y = n.i_IdSerieDocumento.Value, x = 53, eliminado = 0 }
                                                                          equals new { y = j.i_ItemId, x = j.i_GroupId, eliminado = j.i_IsDeleted.Value } into j_join
                                           from j in j_join.DefaultIfEmpty()

                                           join k in dbcontext.datahierarchy on new { Grupo = 27, eliminado = 0, item = n.i_Igv.Value } equals new { Grupo = k.i_GroupId, eliminado = k.i_IsDeleted.Value, item = k.i_ItemId } into k_join
                                           from k in k_join.DefaultIfEmpty()
                                           where n.v_IdImportacion == pstrIdImportaciones && n.i_Eliminado == 0

                                           select new ReporteImportaciones
                                           {
                                               NumeroRegistro = "IMPORTACIÓN N° " + (n.v_Mes.Trim() + "-" + n.v_Correlativo.Trim()).Trim(),
                                               NumeroDocumento = a == null || j == null || n == null ? "" : a.v_Siglas.Trim() + " " + j.v_Value2.Trim() + "-" + n.v_CorrelativoDocumento.Trim(),
                                               Moneda = b == null ? "" : b.v_Value1,
                                               TipoCambio = n.d_TipoCambio.Value,
                                               Via = c == null ? "" : c.v_Value1,
                                               ValorFob = n.d_TotalValorFob,
                                               Flete = n.d_Flete,
                                               PagaSeguro = n.i_PagaSeguro == 1 ? n.d_PagoSeguro : 0,
                                               ImpuestoAdValoren = n.d_AdValorem,
                                               SubTotal = n.d_SubTotal,
                                               Igv = n.d_Igv,
                                               OtrosGastos = n.d_OtrosGastos,
                                               Estado = d.v_Value1,
                                               CodigProducto = h.v_CodInterno,
                                               NombreProducto = h.v_Descripcion,
                                               UM = i.v_Value1,
                                               d_Cantidad = e == null ? 0 : e.d_Cantidad,
                                               d_ValorFob = e == null ? 0 : e.d_ValorFob,
                                               d_Flete = e == null ? 0 : e.d_Flete,
                                               d_Advalorem = e == null ? 0 : e.d_AdValorem,
                                               d_Seguro = e == null ? 0 : e.d_Seguro,
                                               d_Igv = e == null ? 0 : e.d_Igv,
                                               d_OtrosGastos = e == null ? 0 : e.d_OtrosGastos,
                                               d_TotalConIgv = e == null ? 0 : e.d_Total,
                                               d_CostoUnitarioSinIgv = e == null ? 0 : e.d_CostoUnitario,
                                               NroPedido = e == null ? "" : e.v_NroPedido,
                                               Proveedor = e == null ? "" : f.v_CodCliente,
                                               NombreEmpresaPropietaria = "",
                                               RucEmpresaPropietaria = "",
                                               FechaEmision = n.t_FechaEmision.Value,
                                               FechaPagoVencimiento = n.t_FechaPagoVencimiento.Value,
                                               AdValorem = n == null ? 0 : n.d_AdValorem.Value == null ? 0 : n.d_AdValorem.Value,
                                               i_Igv = n.i_Igv.Value,
                                               MonedaOperacion = n.i_IdMoneda.Value,
                                               Igv18 = k.v_Value1,
                                               v_IdProductoDetalle = e.v_IdProductoDetalle,
                                               PrecioUnitario = e.d_Precio,
                                           });


                        if (Importacion.Count() == 0)
                        {
                            var query2 = (from n in dbcontext.importacion

                                          join a in dbcontext.documento on n.i_IdTipoDocumento equals a.i_CodigoDocumento into a_join
                                          from a in a_join.DefaultIfEmpty()

                                          join b in dbcontext.datahierarchy on new { y = n.i_IdMoneda.Value, x = 18 } //Moneda
                                                            equals new { y = b.i_ItemId, x = b.i_GroupId } into b_join
                                          from b in b_join.DefaultIfEmpty()

                                          join c in dbcontext.datahierarchy on new { y = n.i_IdTipoVia.Value, x = 49 } //Via
                                                            equals new { y = c.i_ItemId, x = c.i_GroupId } into c_join
                                          from c in c_join.DefaultIfEmpty()

                                          join d in dbcontext.datahierarchy on new { y = n.i_IdEstado.Value, x = 30 }
                                                          equals new { y = d.i_ItemId, x = d.i_GroupId } into d_join
                                          from d in d_join.DefaultIfEmpty()

                                          join j in dbcontext.datahierarchy on new { y = n.i_IdSerieDocumento.Value, x = 53 }
                                                                    equals new { y = j.i_ItemId, x = j.i_GroupId } into j_join
                                          from j in j_join.DefaultIfEmpty()

                                          where n.v_IdImportacion == pstrIdImportaciones && n.i_Eliminado == 0

                                          select new ReporteImportaciones
                                          {
                                              NumeroRegistro = (n.v_Mes.Trim() + "-" + n.v_Correlativo.Trim()).Trim(),
                                              NumeroDocumento = a.v_Siglas.Trim() + " " + j.v_Value2.Trim() + "-" + n.v_CorrelativoDocumento.Trim(),
                                              Moneda = b.v_Value1,
                                              TipoCambio = n.d_TipoCambio,
                                              Via = c.v_Value1,
                                              ValorFob = n.d_TotalValorFob,
                                              Flete = n.d_Flete,
                                              PagaSeguro = n.i_PagaSeguro == 1 ? n.d_PagoSeguro : 0,
                                              ImpuestoAdValoren = n.d_AdValorem,
                                              SubTotal = n.d_SubTotal,
                                              Igv = n.d_Igv,
                                              OtrosGastos = n.d_OtrosGastos,
                                              Estado = d.v_Value1,
                                              CodigProducto = string.Empty,
                                              NombreProducto = string.Empty,
                                              UM = string.Empty,
                                              d_Cantidad = 0,

                                              d_ValorFob = 0,
                                              d_Flete = 0,
                                              d_Advalorem = 0,
                                              d_Seguro = 0,
                                              d_Igv = 0,
                                              d_OtrosGastos = 0,
                                              d_TotalConIgv = 0,
                                              d_CostoUnitarioSinIgv = 0,
                                              NroPedido = string.Empty,
                                              Proveedor = string.Empty,
                                              NombreEmpresaPropietaria = "",
                                              RucEmpresaPropietaria = "",
                                              FechaEmision = n.t_FechaEmision.Value,
                                              FechaPagoVencimiento = n.t_FechaPagoVencimiento.Value,
                                              AdValorem = n == null ? 0 : n.d_AdValorem.Value == null ? 0 : n.d_AdValorem.Value,
                                              i_Igv = n.i_Igv.Value,
                                              MonedaOperacion = n.i_IdMoneda.Value,
                                              PrecioUnitario = 0,
                                          });

                            objData = query2.ToList();
                        }
                        else
                        {

                            objData = Importacion.ToList();
                        }


                        return objData;
                    }

                    else
                    {


                        var Importacion = (from n in dbcontext.importacion

                                           join e in dbcontext.importaciondetalleproducto on new { i = n.v_IdImportacion, eliminado = 0 } equals new { i = e.v_IdImportacion, eliminado = e.i_Eliminado.Value } into e_join

                                           from e in e_join.DefaultIfEmpty()

                                           join a in dbcontext.documento on new { td = n.i_IdTipoDocumento.Value, eliminado = 0 } equals new { td = a.i_CodigoDocumento, eliminado = a.i_Eliminado.Value } into a_join
                                           from a in a_join.DefaultIfEmpty()

                                           join b in dbcontext.datahierarchy on new { y = n.i_IdMoneda.Value, x = 18, eliminado = 0 } //Moneda
                                                             equals new { y = b.i_ItemId, x = b.i_GroupId, eliminado = b.i_IsDeleted.Value } into b_join
                                           from b in b_join.DefaultIfEmpty()

                                           join c in dbcontext.datahierarchy on new { y = n.i_IdTipoVia.Value, x = 49, eliminado = 0 } //Via
                                                             equals new { y = c.i_ItemId, x = c.i_GroupId, eliminado = c.i_IsDeleted.Value } into c_join
                                           from c in c_join.DefaultIfEmpty()

                                           join d in dbcontext.datahierarchy on new { y = n.i_IdEstado.Value, x = 30, eliminado = 0 }
                                                           equals new { y = d.i_ItemId, x = d.i_GroupId, eliminado = d.i_IsDeleted.Value } into d_join
                                           from d in d_join.DefaultIfEmpty()

                                           join f in dbcontext.cliente on new { c = e.v_IdCliente, eliminado = 0 } equals new { c = f.v_IdCliente, eliminado = f.i_Eliminado.Value } into f_join

                                           from f in f_join.DefaultIfEmpty()

                                           join g in dbcontext.productodetalle on new { pd = e.v_IdProductoDetalle, eliminado = 0 } equals new { pd = g.v_IdProductoDetalle, eliminado = g.i_Eliminado.Value } into g_join

                                           from g in g_join.DefaultIfEmpty()

                                           join h in dbcontext.producto on new { p = g.v_IdProducto, eliminado = 0 } equals new { p = h.v_IdProducto, eliminado = h.i_Eliminado.Value } into h_join

                                           from h in h_join.DefaultIfEmpty()

                                           join i in dbcontext.datahierarchy on new { y = e.i_IdUnidadMedida.Value, x = 17, eliminado = 0 }
                                                                          equals new { y = i.i_ItemId, x = i.i_GroupId, eliminado = i.i_IsDeleted.Value }
                                           join j in dbcontext.datahierarchy on new { y = n.i_IdSerieDocumento.Value, x = 53, eliminado = 0 }
                                                                          equals new { y = j.i_ItemId, x = j.i_GroupId, eliminado = j.i_IsDeleted.Value } into j_join
                                           from j in j_join.DefaultIfEmpty()
                                           join k in dbcontext.datahierarchy on new { Grupo = 27, eliminado = 0, item = n.i_Igv.Value } equals new { Grupo = k.i_GroupId, eliminado = k.i_IsDeleted.Value, item = k.i_ItemId } into k_join
                                           from k in k_join.DefaultIfEmpty()

                                           where n.v_IdImportacion == pstrIdImportaciones && n.i_Eliminado == 0

                                           select new ReporteImportaciones
                                           {
                                               NumeroRegistro = "IMPORTACIÓN N° " + (n.v_Mes.Trim() + "-" + n.v_Correlativo.Trim()).Trim(),
                                               NumeroDocumento = a == null || j == null || n == null ? "" : a.v_Siglas.Trim() + " " + j.v_Value2.Trim() + "-" + n.v_CorrelativoDocumento.Trim(),
                                               Moneda = b == null ? "" : b.v_Value1,
                                               TipoCambio = n.d_TipoCambio.Value,
                                               Via = c == null ? "" : c.v_Value1,

                                               Estado = d.v_Value1,
                                               CodigProducto = h.v_CodInterno,
                                               NombreProducto = h.v_Descripcion,
                                               UM = i.v_Value1,
                                               d_Cantidad = e.d_Cantidad.Value,

                                               NroPedido = e == null ? "" : e.v_NroPedido,
                                               Proveedor = e == null ? "" : f.v_CodCliente,
                                               NombreEmpresaPropietaria = "",
                                               RucEmpresaPropietaria = "",
                                               FechaEmision = n.t_FechaEmision.Value,
                                               FechaPagoVencimiento = n.t_FechaPagoVencimiento.Value,
                                               v_IdImportacion = n.v_IdImportacion,
                                               i_Igv = n.i_Igv.Value,
                                               MonedaOperacion = n.i_IdMoneda.Value,
                                               Igv18 = k.v_Value1,
                                               v_IdProductoDetalle = e.v_IdProductoDetalle,
                                           });


                        if (Importacion.Count() == 0)
                        {

                            var query2 = (from n in dbcontext.importacion

                                          join a in dbcontext.documento on n.i_IdTipoDocumento equals a.i_CodigoDocumento into a_join
                                          from a in a_join.DefaultIfEmpty()

                                          join b in dbcontext.datahierarchy on new { y = n.i_IdMoneda.Value, x = 18 } //Moneda
                                                            equals new { y = b.i_ItemId, x = b.i_GroupId } into b_join
                                          from b in b_join.DefaultIfEmpty()

                                          join c in dbcontext.datahierarchy on new { y = n.i_IdTipoVia.Value, x = 49 } //Via
                                                            equals new { y = c.i_ItemId, x = c.i_GroupId } into c_join
                                          from c in c_join.DefaultIfEmpty()

                                          join d in dbcontext.datahierarchy on new { y = n.i_IdEstado.Value, x = 30 }
                                                          equals new { y = d.i_ItemId, x = d.i_GroupId } into d_join
                                          from d in d_join.DefaultIfEmpty()

                                          join j in dbcontext.datahierarchy on new { y = n.i_IdSerieDocumento.Value, x = 53 }
                                                                    equals new { y = j.i_ItemId, x = j.i_GroupId } into j_join
                                          from j in j_join.DefaultIfEmpty()

                                          where n.v_IdImportacion == pstrIdImportaciones && n.i_Eliminado == 0

                                          select new ReporteImportaciones
                                          {
                                              NumeroRegistro = (n.v_Mes.Trim() + "-" + n.v_Correlativo.Trim()).Trim(),
                                              NumeroDocumento = a.v_Siglas.Trim() + " " + j.v_Value2.Trim() + "-" + n.v_CorrelativoDocumento.Trim(),
                                              Moneda = b.v_Value1,
                                              TipoCambio = n.d_TipoCambio,
                                              Via = c.v_Value1,
                                              ValorFob = n.d_TotalValorFob,
                                              Flete = n.d_Flete,
                                              PagaSeguro = n.i_PagaSeguro == 1 ? n.d_PagoSeguro : 0,
                                              ImpuestoAdValoren = n.d_AdValorem,
                                              SubTotal = n.d_SubTotal,
                                              Igv = n.d_Igv,
                                              OtrosGastos = n.d_OtrosGastos,
                                              Estado = d.v_Value1,
                                              CodigProducto = string.Empty,
                                              NombreProducto = string.Empty,
                                              UM = string.Empty,
                                              PrecioUnitario = 0,
                                              d_Cantidad = 0,
                                              d_ValorFob = 0,
                                              d_Flete = 0,
                                              d_Advalorem = 0,
                                              d_Seguro = 0,
                                              d_Igv = 0,
                                              d_OtrosGastos = 0,
                                              d_TotalConIgv = 0,
                                              d_CostoUnitarioSinIgv = 0,
                                              NroPedido = string.Empty,
                                              Proveedor = string.Empty,
                                              NombreEmpresaPropietaria = "",
                                              RucEmpresaPropietaria = "",
                                              FechaEmision = n.t_FechaEmision.Value,
                                              FechaPagoVencimiento = n.t_FechaPagoVencimiento.Value,
                                              AdValorem = n == null ? 0 : n.d_AdValorem.Value == null ? 0 : n.d_AdValorem.Value,
                                              i_Igv = n.i_Igv.Value,
                                              MonedaOperacion = n.i_IdMoneda.Value,

                                          });

                            objData = query2.ToList();
                        }
                        else
                        {

                            foreach (var item in Importacion)
                            {
                                ReporteImportaciones _objImportacion = new ReporteImportaciones();
                                _objImportacion = item;
                                _objImportacion.Moneda = MonedaImpresion == (int)Currency.Soles ? "SOLES" : "DÓLARES";
                                var ValoresCambio = CalculoValoresImportacionCambio(item.v_IdImportacion, item.MonedaOperacion, decimal.Parse(item.Igv18), MonedaImpresion, item.v_IdProductoDetalle);
                                _objImportacion.ValorFob = ValoresCambio.Fob;
                                _objImportacion.Flete = ValoresCambio.Flete;
                                _objImportacion.PagaSeguro = ValoresCambio.Seguro;
                                _objImportacion.AdValorem = ValoresCambio.Advalorem;
                                _objImportacion.Igv = ValoresCambio.Igv;
                                _objImportacion.OtrosGastos = ValoresCambio.OtrosGastos;
                                _objImportacion.PrecioUnitario = ValoresCambio.PrecioUnitarioProducto;
                                _objImportacion.d_ValorFob = ValoresCambio.FobProducto;
                                _objImportacion.d_Flete = ValoresCambio.FleteProducto;
                                _objImportacion.d_Seguro = ValoresCambio.SeguroProducto;
                                _objImportacion.d_Advalorem = ValoresCambio.AdvaloremProducto;
                                _objImportacion.d_Igv = ValoresCambio.IgvProducto;
                                _objImportacion.d_OtrosGastos = ValoresCambio.OtrosGastosProducto;
                                _objImportacion.d_TotalConIgv = ValoresCambio.TotalProducto;
                                _objImportacion.d_CostoUnitarioSinIgv = ValoresCambio.CostoUnitarioProducto;

                                objData.Add(_objImportacion);
                            }

                        }

                        return objData;
                    }
                }

            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                return null;
            }


        }


        public ValoresImportacionCambio CalculoValoresImportacionCambio(string IdImportacion, int MonedaOperacion, decimal Igv, int MonedaReporte, string IdProductoDetalle)
        {

            using (SAMBHSEntitiesModelWin dbcontext = new SAMBHSEntitiesModelWin())
            {
                //var NotaIngreso = dbContext.movimiento.Where(i => i.i_IdTipoMovimiento == 1 && i.v_OrigenRegPeriodo.Trim() == import.v_Periodo.Trim() && i.v_OrigenRegCorrelativo.Trim() == import.v_Correlativo.Trim() && i.v_OrigenRegMes.Trim() == import.v_Mes.Trim() && i.i_Eliminado == 0 && i.v_OrigenTipo == "I").FirstOrDefault();
                //decimal Igv = 18;  
                ValoresImportacionCambio _objReporteValoresCambio = new ValoresImportacionCambio();
                var Importaciones = dbcontext.importacion.Where(h => h.v_IdImportacion == IdImportacion).FirstOrDefault();
                var ImportacionProducto = dbcontext.importaciondetalleproducto.Where(c => c.i_Eliminado == 0 && c.v_IdImportacion == IdImportacion && c.v_IdProductoDetalle == IdProductoDetalle).ToList().OrderBy(x => x.v_IdImportacionDetalleProducto).ToList();
                var Gastos = dbcontext.importaciondetallegastos.Where(x => x.v_IdImportacion == IdImportacion && x.i_Eliminado == 0).ToList();
                decimal otrosGastosDolares = 0, otrosGastosSoles = 0, TotalFobSoles = 0, TotalFobDolares = 0, FleteSoles = 0, FleteDolares = 0, SeguroSoles = 0;
                decimal SeguroDolares = 0, AdvaloremSoles = 0, AdvaloremDolares = 0, IgvSoles = 0, IgvDolares = 0;
                #region Fletes-Igv
                switch (MonedaOperacion)
                {

                    case 1:
                        FleteSoles = Importaciones.d_Flete.Value;
                        FleteDolares = Importaciones.d_TipoCambioDoc1 == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado(Importaciones.d_Flete.Value / Importaciones.d_TipoCambioDoc1.Value, 4);
                        SeguroSoles = Importaciones.d_PagoSeguro.Value;
                        SeguroDolares = Utils.Windows.DevuelveValorRedondeado(Importaciones.d_PagoSeguro.Value / Importaciones.d_TipoCambioDoc2.Value, 4);
                        AdvaloremSoles = Importaciones.d_AdValorem.Value;
                        AdvaloremDolares = Utils.Windows.DevuelveValorRedondeado(Importaciones.d_AdValorem.Value / Importaciones.d_TipoCambioDoc3.Value, 4);
                        IgvSoles = Importaciones.d_Igv.Value;
                        IgvDolares = Utils.Windows.DevuelveValorRedondeado(Importaciones.d_Igv.Value / Importaciones.d_TipoCambioDoc4.Value, 4);
                        break;
                    case 2:
                        FleteDolares = Importaciones.d_Flete.Value;
                        FleteSoles = Utils.Windows.DevuelveValorRedondeado(Importaciones.d_Flete.Value * Importaciones.d_TipoCambioDoc1.Value, 4);
                        SeguroDolares = Importaciones.d_PagoSeguro.Value;
                        SeguroSoles = Utils.Windows.DevuelveValorRedondeado(Importaciones.d_PagoSeguro.Value * Importaciones.d_TipoCambioDoc2.Value, 4);
                        AdvaloremDolares = Importaciones.d_AdValorem.Value;
                        AdvaloremSoles = Utils.Windows.DevuelveValorRedondeado(Importaciones.d_AdValorem.Value * Importaciones.d_TipoCambioDoc3.Value, 4);
                        IgvDolares = Importaciones.d_Igv.Value;
                        IgvSoles = Utils.Windows.DevuelveValorRedondeado(Importaciones.d_Igv.Value * Importaciones.d_TipoCambioDoc4.Value, 4);
                        break;
                }
                #endregion
                #region Fob
                var Fob = dbcontext.importaciondetallefob.Where(x => x.i_Eliminado == 0 && x.v_IdImportacion == Importaciones.v_IdImportacion).ToList();
                if (Fob.Any())
                {
                    foreach (var Fila in Fob)
                    {
                        switch (Importaciones.i_IdMoneda)
                        {
                            case 1:
                                TotalFobSoles = TotalFobSoles + Fila.d_ValorFob.Value;
                                TotalFobDolares = Fila.d_TipoCambio == 0 ? TotalFobDolares : TotalFobDolares + Utils.Windows.DevuelveValorRedondeado((Fila.d_ValorFob.Value / Fila.d_TipoCambio.Value), 2);
                                TotalFobDolares = Utils.Windows.DevuelveValorRedondeado(TotalFobDolares, 2);
                                break;
                            case 2:
                                TotalFobDolares = TotalFobDolares + Fila.d_ValorFob.Value;
                                TotalFobSoles = TotalFobSoles + Utils.Windows.DevuelveValorRedondeado((Fila.d_ValorFob.Value * Fila.d_TipoCambio.Value), 2);
                                TotalFobSoles = Utils.Windows.DevuelveValorRedondeado(TotalFobSoles, 2);
                                break;
                        }
                    }

                }

                #endregion
                #region Gastos
                foreach (var gastito in Gastos)
                {

                    if (Igv != 0)
                    {
                        decimal d_Igv = Utils.Windows.DevuelveValorRedondeado((gastito.d_ValorVenta.Value) * (Igv / 100), 2);
                    }

                    if (gastito.d_TipoCambio.Value != 0)
                    {
                        decimal d_ImporteSoles = 0, d_ImporteDolares = 0;
                        decimal VaVNoAfectSoles = 0, VaVNoAfectDolares = 0;
                        switch (gastito.i_IdMoneda)
                        {


                            case 1:   //Soles
                                d_ImporteSoles = d_ImporteSoles + Utils.Windows.DevuelveValorRedondeado(gastito.d_ValorVenta.Value + gastito.d_NAfectoDetraccion.Value + gastito.d_Igv.Value, 2);
                                d_ImporteDolares = d_ImporteDolares + Utils.Windows.DevuelveValorRedondeado(gastito.d_ImporteSoles.Value / gastito.d_TipoCambio.Value, 2);


                                VaVNoAfectSoles = gastito.d_ValorVenta.Value + gastito.d_NAfectoDetraccion.Value;
                                VaVNoAfectDolares = Utils.Windows.DevuelveValorRedondeado((gastito.d_ValorVenta.Value + gastito.d_NAfectoDetraccion.Value) / gastito.d_TipoCambio.Value, 2);

                                otrosGastosSoles = VaVNoAfectSoles + otrosGastosSoles;
                                otrosGastosDolares = gastito.d_TipoCambio.Value == 0 ? otrosGastosDolares : VaVNoAfectDolares + otrosGastosDolares;
                                otrosGastosSoles = Utils.Windows.DevuelveValorRedondeado(otrosGastosSoles, 2);

                                break;

                            case 2: //Dolares


                                d_ImporteDolares = d_ImporteDolares + Utils.Windows.DevuelveValorRedondeado(gastito.d_ValorVenta.Value + gastito.d_NAfectoDetraccion.Value + gastito.d_Igv.Value, 2);
                                d_ImporteSoles = d_ImporteSoles + Utils.Windows.DevuelveValorRedondeado((gastito.d_ImporteDolares.Value * gastito.d_TipoCambio.Value), 2);


                                VaVNoAfectSoles = Utils.Windows.DevuelveValorRedondeado((gastito.d_ValorVenta.Value + gastito.d_NAfectoDetraccion.Value) * gastito.d_TipoCambio.Value, 2);
                                VaVNoAfectDolares = gastito.d_ValorVenta.Value + gastito.d_NAfectoDetraccion.Value;


                                otrosGastosDolares = gastito.d_ValorVenta.Value + gastito.d_NAfectoDetraccion.Value + otrosGastosDolares;
                                otrosGastosSoles = gastito.d_TipoCambio == 0 ? otrosGastosSoles : Utils.Windows.DevuelveValorRedondeado((gastito.d_ValorVenta.Value + gastito.d_NAfectoDetraccion.Value) * gastito.d_TipoCambio.Value, 2) + otrosGastosSoles;
                                otrosGastosDolares = Utils.Windows.DevuelveValorRedondeado(otrosGastosDolares, 2);



                                break;
                        }
                    }


                }
                #endregion
                #region ProductoDetalles
                foreach (var producto in ImportacionProducto)
                {
                    //if (NotaIngreso != null)
                    //{
                    decimal d_FobProductoSoles = 0, d_FobProductoDolares = 0;
                    decimal d_SeguroProductoSoles = 0, d_SeguroProductoDolares = 0;
                    decimal d_AdvaloremProductoSoles = 0, d_AdvaloremProductoDolares = 0;
                    decimal d_IgvProductoSoles = 0, d_IgvProductoDolares = 0;
                    decimal d_FleteProductoSoles = 0, d_FleteProductoDolares = 0;
                    decimal d_OtrosGastosProductoSoles = 0, d_OtrosGastosProductoDolares = 0;
                    decimal d_CostoUnitarioSoles = 0, d_CostoUnitarioDolares = 0, PrecioUnitarioCambio = 0;
                    // var NotaIngresoDetalle = dbContext.movimientodetalle.Where(x => x.v_IdMovimiento == NotaIngreso.v_IdMovimiento && x.v_IdProductoDetalle == producto.v_IdProductoDetalle && x.i_Eliminado == 0).FirstOrDefault();

                    #region CalculoSoles


                    if (producto.d_Cantidad != 0 && producto.d_Precio != 0)
                    {

                        d_FobProductoDolares = Utils.Windows.DevuelveValorRedondeado((producto.d_Cantidad.Value * producto.d_Precio.Value), 6);
                        PrecioUnitarioCambio = PrecioCambio(producto.v_IdCliente, producto.v_NroFactura, producto.v_IdImportacion, MonedaOperacion, producto.d_Precio.Value);
                        d_FobProductoSoles = Utils.Windows.DevuelveValorRedondeado(producto.d_Cantidad.Value * PrecioCambio(producto.v_IdCliente, producto.v_NroFactura, producto.v_IdImportacion, MonedaOperacion, producto.d_Precio.Value), 6);

                        d_FleteProductoDolares = TotalFobDolares == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((d_FobProductoDolares / TotalFobDolares) * FleteDolares, 6);
                        d_FleteProductoSoles = TotalFobSoles == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((d_FobProductoSoles / TotalFobSoles) * FleteSoles, 6);
                        // Fila.Cells["d_Flete"].Value = ValorFob == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado(((decimal.Parse(Fila.Cells["d_ValorFob"].Text.ToString()) / ValorFob) * flete), 6);

                        d_SeguroProductoDolares = TotalFobDolares == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((d_FobProductoDolares / TotalFobDolares) * SeguroDolares, 6);
                        d_SeguroProductoSoles = TotalFobSoles == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((d_FobProductoSoles / TotalFobSoles) * SeguroSoles, 6);
                        //Fila.Cells["d_Seguro"].Value = ValorFob == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((decimal.Parse(Fila.Cells["d_ValorFob"].Text.ToString()) / ValorFob) * Seguro, 6);

                        d_AdvaloremProductoDolares = TotalFobDolares == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((d_FobProductoDolares / TotalFobDolares) * AdvaloremDolares, 6);
                        d_AdvaloremProductoSoles = TotalFobSoles == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((d_FobProductoSoles / TotalFobSoles) * AdvaloremSoles, 6);
                        //Fila.Cells["d_AdValorem"].Value = ValorFob == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((decimal.Parse(Fila.Cells["d_ValorFob"].Text.ToString()) / ValorFob) * AdValorem, 6);


                        d_IgvProductoDolares = TotalFobDolares == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((d_FobProductoDolares / TotalFobDolares) * IgvDolares, 6);
                        d_IgvProductoSoles = TotalFobSoles == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((d_FobProductoSoles / TotalFobSoles) * IgvSoles, 6);
                        //Fila.Cells["d_Igv"].Value = ValorFob == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((decimal.Parse(Fila.Cells["d_ValorFob"].Text.ToString()) / ValorFob) * Igv, 6);

                        //Fila.Cells["d_OtrosGastos"].Value = ValorFob == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((decimal.Parse(Fila.Cells["d_ValorFob"].Text.ToString()) / ValorFob) * OtrosGastos, 6);

                        d_OtrosGastosProductoDolares = TotalFobDolares == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((d_FobProductoDolares / TotalFobDolares) * otrosGastosDolares, 6);
                        d_OtrosGastosProductoSoles = TotalFobSoles == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((d_FobProductoSoles / TotalFobSoles) * otrosGastosSoles, 6);

                        d_CostoUnitarioDolares = producto.d_Cantidad == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado(((d_FobProductoDolares + d_FleteProductoDolares + d_SeguroProductoDolares + d_AdvaloremProductoDolares + d_OtrosGastosProductoDolares) / producto.d_Cantidad.Value), 6);
                        d_CostoUnitarioSoles = producto.d_Cantidad == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado(((d_FobProductoSoles + d_FleteProductoSoles + d_SeguroProductoSoles + d_AdvaloremProductoSoles + d_OtrosGastosProductoSoles) / producto.d_Cantidad.Value), 6);
                    }

                    #endregion


                    switch (MonedaReporte)
                    {


                        case 1:
                            _objReporteValoresCambio.Fob = TotalFobSoles;
                            _objReporteValoresCambio.Flete = FleteSoles;
                            _objReporteValoresCambio.Seguro = SeguroSoles;
                            _objReporteValoresCambio.Advalorem = AdvaloremSoles;
                            _objReporteValoresCambio.Igv = IgvSoles;
                            _objReporteValoresCambio.OtrosGastos = otrosGastosSoles;
                            _objReporteValoresCambio.PrecioUnitarioProducto = PrecioUnitarioCambio;
                            _objReporteValoresCambio.FobProducto = d_FobProductoSoles;
                            _objReporteValoresCambio.FleteProducto = d_FleteProductoSoles;
                            _objReporteValoresCambio.SeguroProducto = d_SeguroProductoSoles;
                            _objReporteValoresCambio.AdvaloremProducto = d_AdvaloremProductoSoles;
                            _objReporteValoresCambio.IgvProducto = d_IgvProductoSoles;
                            _objReporteValoresCambio.OtrosGastosProducto = d_OtrosGastosProductoSoles;
                            _objReporteValoresCambio.TotalProducto = Utils.Windows.DevuelveValorRedondeado(d_FobProductoSoles + d_FleteProductoSoles + d_SeguroProductoSoles + d_AdvaloremProductoSoles + d_OtrosGastosProductoSoles + d_IgvProductoSoles, 6);
                            _objReporteValoresCambio.CostoUnitarioProducto = d_CostoUnitarioSoles;
                            break;

                        case 2:

                            break;
                    }




                }

                return _objReporteValoresCambio;

                #endregion
            }




        }
        public List<ReporteImportacionesProveeedorAnalitico> ReporteImportacionesProveedorAnalitico(ref  OperationResult objOperationResult, DateTime FechaInicio, DateTime FechaFin, string Orden, string Filtro, string pstr_grupollave, string pstr_NombreGrupoLlave, int IdMoneda)
        {
            try
            {
                SAMBHSEntitiesModelWin dbcontext = new SAMBHSEntitiesModelWin();
                var query = (from a in dbcontext.importacion
                             join b in dbcontext.importaciondetalleproducto on new { i = a.v_IdImportacion, eliminado = 0 } equals new { i = b.v_IdImportacion, eliminado = b.i_Eliminado.Value } into b_join
                             from b in b_join.DefaultIfEmpty()
                             join c in dbcontext.datahierarchy on new { x = a.i_IdMoneda.Value, y = 18, eliminado = 0 }
                                                              equals new { x = c.i_ItemId, y = c.i_GroupId, eliminado = c.i_IsDeleted.Value } into c_join
                             from c in c_join.DefaultIfEmpty()

                             join d in dbcontext.documento on new { d = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { d = d.i_CodigoDocumento, eliminado = d.i_Eliminado.Value } into d_join
                             from d in d_join.DefaultIfEmpty()

                             join e in dbcontext.productodetalle on new { pd = b.v_IdProductoDetalle, eliminado = 0 } equals new { pd = e.v_IdProductoDetalle, eliminado = e.i_Eliminado.Value } into e_join
                             from e in e_join.DefaultIfEmpty()

                             join f in dbcontext.producto on new { p = e.v_IdProducto, eliminado = 0 } equals new { p = f.v_IdProducto, eliminado = f.i_Eliminado.Value } into f_join
                             from f in f_join.DefaultIfEmpty()

                             join g in dbcontext.cliente on new { c = b.v_IdCliente, eliminado = 0 } equals new { c = g.v_IdCliente, eliminado = g.i_Eliminado.Value } into g_join
                             from g in g_join.DefaultIfEmpty()

                             join h in dbcontext.almacen on new { a = a.i_IdAlmacen.Value, eliminado = 0 } equals new { a = h.i_IdAlmacen, eliminado = h.i_Eliminado.Value } into h_join
                             from h in h_join.DefaultIfEmpty()

                             join i in dbcontext.datahierarchy on new { x = a.i_IdSerieDocumento.Value, y = 53, eliminado = 0 }
                                                             equals new { x = i.i_ItemId, y = i.i_GroupId, eliminado = i.i_IsDeleted.Value } into i_join
                             from i in i_join.DefaultIfEmpty()


                             where a.i_Eliminado == 0 && a.t_FechaRegistro >= FechaInicio && a.t_FechaRegistro <= FechaFin


                             select new ReporteImportacionesProveeedorAnalitico
                             {
                                 Moneda = c.v_Value1,
                                 Almacen = h.v_Nombre,
                                 Proveedor = g.v_CodCliente.Trim().ToUpper() + " / " + (g.v_PrimerNombre.Trim().ToUpper() + " " + g.v_ApePaterno.Trim().ToUpper() + " " + g.v_ApeMaterno.Trim().ToUpper() + " " + g.v_RazonSocial.Trim().ToUpper()).Trim() + " / " + g.v_NroDocIdentificacion.Trim(),
                                 NroRegistro = a.v_Mes.Trim() + "-" + a.v_Correlativo.Trim(),
                                 Fecha = a.t_FechaRegistro.Value,
                                 Documento = d.v_Siglas.Trim() + ".  " + i.v_Value2.Trim() + "-" + a.v_CorrelativoDocumento.Trim(),
                                 NroPedido = b.v_NroPedido,
                                 CodProducto = f.v_CodInterno,
                                 DescripcionProducto = f.v_Descripcion,
                                 Cantidad = b.d_Cantidad,
                                 CostoSinIgv = a.i_IdMoneda.Value == IdMoneda ? b.d_CostoUnitario : IdMoneda == (int)Currency.Soles ? b.d_CostoUnitario.Value * a.d_TipoCambio.Value : a.d_TipoCambio.Value == 0 ? 0 : b.d_CostoUnitario.Value / a.d_TipoCambio.Value,
                                 TotalCosto = a.i_IdMoneda.Value == IdMoneda ? b.d_Cantidad * b.d_CostoUnitario : IdMoneda == (int)Currency.Soles ? (b.d_Cantidad.Value * b.d_CostoUnitario.Value) * a.d_TipoCambio.Value : a.d_TipoCambio.Value == 0 ? 0 : (b.d_Cantidad.Value * b.d_CostoUnitario.Value) / a.d_TipoCambio.Value,
                                 //TotalCosto =  b.d_Cantidad.Value  * b.d_CostoUnitario.Value ,
                                 GrupoLLave = pstr_grupollave == "PRODUCTO" ? f.v_Descripcion == null ? "** " + pstr_grupollave + " NO EXISTE **" : pstr_grupollave + " : " + f.v_CodInterno.Trim().ToUpper() + " / " + f.v_Descripcion.Trim().ToUpper() : "",
                                 i_IdAlmacen = h.i_IdAlmacen,
                                 i_IdMoneda = c.i_ItemId,
                                 CodProveedor = g.v_CodCliente,
                             }).ToList().AsQueryable();


                if (!string.IsNullOrEmpty(Filtro))
                {
                    query = query.Where(Filtro);
                }

                if (!string.IsNullOrEmpty(Orden))
                {
                    query = query.OrderBy(Orden);
                }
                objOperationResult.Success = 1;
                List<ReporteImportacionesProveeedorAnalitico> objData = query.ToList();
                return objData;
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;

                return null;
            }

        }

        public List<ReporteImportacionesProveeedorResumen> ReporteImportacionesProveedorResumen(ref OperationResult objOperationResult, DateTime FechaInicio, DateTime FechaFin, string pstrCodigoProveedor, int Almacen, string NroPedido)
        {

            try
            {

                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                List<ReporteImportacionesProveeedorResumen> Reporte = new List<ReporteImportacionesProveeedorResumen>();
                List<ReporteImportacionesProveeedorResumen> query = (from a in dbContext.importacion

                                                                     join b in dbContext.importaciondetalleproducto on new { i = a.v_IdImportacion, eliminado = 0 } equals new { i = b.v_IdImportacion, eliminado = b.i_Eliminado.Value } into b_join
                                                                     from b in b_join.DefaultIfEmpty()

                                                                     join c in dbContext.almacen on new { a = a.i_IdAlmacen.Value, eliminado = 0 } equals new { a = c.i_IdAlmacen, eliminado = c.i_Eliminado.Value } into c_join
                                                                     from c in c_join.DefaultIfEmpty()

                                                                     join d in dbContext.cliente on new { c = b.v_IdCliente, eliminado = 0 } equals new { c = d.v_IdCliente, eliminado = d.i_Eliminado.Value } into d_join
                                                                     from d in d_join.DefaultIfEmpty()
                                                                     join e in dbContext.datahierarchy on new { y = a.i_IdMoneda.Value, x = 18, eliminado = 0 }
                                                                                                        equals new { y = e.i_ItemId, x = e.i_GroupId, eliminado = e.i_IsDeleted.Value }

                                                                     where a.t_FechaRegistro >= FechaInicio && a.t_FechaRegistro <= FechaFin &&

                                                                     (d.v_CodCliente == pstrCodigoProveedor || pstrCodigoProveedor == "")
                                                                     && a.i_IdAlmacen == Almacen && (b.v_NroPedido == NroPedido || NroPedido == "")
                                                                     && a.i_Eliminado == 0
                                                                     select new ReporteImportacionesProveeedorResumen
                                                                     {
                                                                         Almacen = c.v_Nombre,
                                                                         CodigoProveedor = d.v_CodCliente,
                                                                         NombreProveedor = (d.v_PrimerNombre.Trim() + " " + d.v_ApePaterno.Trim() + " " + d.v_ApeMaterno.Trim() + " " + d.v_RazonSocial.Trim()).Trim(),
                                                                         ValorVentaSoles = a.i_IdMoneda == 1 ? (b.d_CostoUnitario.Value * b.d_Cantidad) : a.d_TipoCambio.Value * (b.d_CostoUnitario.Value * b.d_Cantidad),
                                                                         ValorVentaDolares = a.i_IdMoneda == 1 ? (b.d_CostoUnitario.Value * b.d_Cantidad) / a.d_TipoCambio : b.d_CostoUnitario.Value * b.d_Cantidad,
                                                                         v_IdCliente = d.v_IdCliente,
                                                                         v_NroDocumento = d.v_NroDocIdentificacion,
                                                                         v_IdImportacionDetalleProducto = b.v_IdImportacionDetalleProducto,
                                                                     }).ToList();


                var xx = query.GroupBy(o => o.CodigoProveedor).ToList();

                foreach (var Fila in xx)
                {
                    var Resultado = (from A in xx
                                     //let CalcularTotales = MetodoCalcularTotalesCobranza(Fila.Select())
                                     select new ReporteImportacionesProveeedorResumen
                                     {


                                         Almacen = Fila.Select(p => p.Almacen).First(),
                                         CodigoProveedor = Fila.Select(q => q.CodigoProveedor).First(),
                                         NombreProveedor = Fila.Select(q => q.NombreProveedor).First(),
                                         ValorVentaSoles = Fila.Sum(P => P.ValorVentaSoles),
                                         ValorVentaDolares = Fila.Sum(Q => Q.ValorVentaDolares),
                                         v_IdCliente = Fila.Select(p => p.v_IdCliente).First(),
                                         v_NroDocumento = Fila.Select(p => p.v_NroDocumento).First(),
                                     }).First();
                    Reporte.Add(Resultado);

                }



                List<ReporteImportacionesProveeedorResumen> objData = Reporte.ToList();
                objOperationResult.Success = 1;
                return objData;
            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                return null;
            }
        }

        private List<decimal> MetodoCalcularTotalesCobranza(List<ReporteImportacionesProveeedorResumen> query1)
        {
            List<decimal> x = new List<decimal>();


            List<string> IdClientes = new List<string>();
            IdClientes = query1.Select(p => p.CodigoProveedor).Distinct().ToList();

            IdClientes.ForEach(o => x.Add(query1.Where(p => p.CodigoProveedor == o).Sum(e => e.ValorVentaSoles.Value)));
            IdClientes.ForEach(o => x.Add(query1.Where(p => p.CodigoProveedor == o).Sum(e => e.ValorVentaDolares.Value)));
            return x;
        }

        public List<ReporteImportacionesProductoAnalitico> ReporteImportacionesProductoAnalitico(ref OperationResult objOperationResult, DateTime FechaInicio, DateTime FechaFin, string Orden, string Filtro, string pstr_grupollave, string pstr_NombreGrupoLlave, int IdMoneda)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbcontext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from a in dbcontext.importacion

                                 join b in dbcontext.importaciondetalleproducto on new { IdImportacion = a.v_IdImportacion, eliminado = 0 } equals new { IdImportacion = b.v_IdImportacion, eliminado = b.i_Eliminado.Value } into b_join
                                 from b in b_join.DefaultIfEmpty()
                                 join c in dbcontext.datahierarchy on new { x = a.i_IdMoneda.Value, y = 18, eliminado = 0 }
                                                                  equals new { x = c.i_ItemId, y = c.i_GroupId, eliminado = c.i_IsDeleted.Value } into c_join
                                 from c in c_join.DefaultIfEmpty()

                                 join d in dbcontext.documento on new { TipoDoc = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { TipoDoc = d.i_CodigoDocumento, eliminado = d.i_Eliminado.Value } into d_join
                                 from d in d_join.DefaultIfEmpty()

                                 join e in dbcontext.productodetalle on new { ProductoDetalle = b.v_IdProductoDetalle, eliminado = 0 } equals new { ProductoDetalle = e.v_IdProductoDetalle, eliminado = e.i_Eliminado.Value } into e_join
                                 from e in e_join.DefaultIfEmpty()

                                 join f in dbcontext.producto on new { IdProducto = e.v_IdProducto, eliminado = 0 } equals new { IdProducto = f.v_IdProducto, eliminado = f.i_Eliminado.Value } into f_join
                                 from f in f_join.DefaultIfEmpty()

                                 join g in dbcontext.cliente on new { IdCliente = b.v_IdCliente, eliminado = 0, Flag = "V" } equals new { IdCliente = g.v_IdCliente, eliminado = g.i_Eliminado.Value, Flag = g.v_FlagPantalla } into g_join
                                 from g in g_join.DefaultIfEmpty()

                                 join h in dbcontext.almacen on a.i_IdAlmacen equals h.i_IdAlmacen into h_join
                                 from h in h_join.DefaultIfEmpty()

                                 join i in dbcontext.datahierarchy on new { x = a.i_IdSerieDocumento.Value, y = 53, eliminado = 0 }
                                                                 equals new { x = i.i_ItemId, y = i.i_GroupId, eliminado = i.i_IsDeleted.Value } into i_join
                                 from i in i_join.DefaultIfEmpty()


                                 where a.i_Eliminado == 0 && a.t_FechaRegistro >= FechaInicio && a.t_FechaRegistro <= FechaFin


                                 select new ReporteImportacionesProductoAnalitico
                                 {
                                     Moneda = c.v_Value1,
                                     Almacen = h.v_Nombre.ToUpper(),
                                     Proveedor = g == null ? "**PROVEEDOR NO EXISTE**" : g.v_CodCliente.Trim().ToUpper() + " / " + (g.v_ApePaterno.Trim().ToUpper() + " " + g.v_ApeMaterno.Trim().ToUpper() + " " + g.v_PrimerNombre.Trim().ToUpper() + " " + g.v_RazonSocial.Trim().ToUpper()).Trim() + " / " + g.v_NroDocIdentificacion.Trim(),
                                     NroRegistro = a.v_Mes.Trim() + "-" + a.v_Correlativo.Trim(),
                                     Fecha = a.t_FechaRegistro.Value,
                                     Documento = d.v_Siglas.Trim() + ".  " + i.v_Value2.Trim() + "-" + a.v_CorrelativoDocumento.Trim(),
                                     NroPedido = b.v_NroPedido,
                                     Producto = f == null ? "**PRODUCTO NO EXISTE**" : "PRODUCTO : " + f.v_CodInterno.Trim().ToUpper() + " " + f.v_Descripcion.Trim().ToUpper(),
                                     Cantidad = b.d_Cantidad,
                                     //CostoSinIgv = b.d_CostoUnitario,
                                     //TotalCosto = b.d_Cantidad * b.d_CostoUnitario,
                                     CostoSinIgv = a.i_IdMoneda.Value == IdMoneda ? b.d_CostoUnitario : IdMoneda == (int)Currency.Soles ? b.d_CostoUnitario.Value * a.d_TipoCambio.Value : a.d_TipoCambio.Value == 0 ? 0 : b.d_CostoUnitario.Value / a.d_TipoCambio.Value,
                                     TotalCosto = a.i_IdMoneda.Value == IdMoneda ? b.d_Cantidad * b.d_CostoUnitario : IdMoneda == (int)Currency.Soles ? (b.d_Cantidad.Value * b.d_CostoUnitario.Value) * a.d_TipoCambio.Value : a.d_TipoCambio.Value == 0 ? 0 : (b.d_Cantidad.Value * b.d_CostoUnitario.Value) / a.d_TipoCambio.Value,
                                     GrupoLLave = pstr_grupollave == "PROVEEDOR" ? g == null ? "** " + pstr_grupollave + " NO EXISTE **" : pstr_grupollave + " : " + g.v_CodCliente.Trim().ToUpper() + " / " + (g.v_ApePaterno.Trim().ToUpper() + " " + g.v_ApeMaterno.Trim().ToUpper() + " " + g.v_PrimerNombre.Trim().ToUpper() + " " + g.v_RazonSocial.Trim().ToUpper()).Trim() : "",
                                     i_IdAlmacen = h.i_IdAlmacen,
                                     i_IdMoneda = c.i_ItemId,
                                     CodProveedor = g == null ? "" : g.v_CodCliente.Trim(),
                                     CodProducto = f == null ? "" : f.v_CodInterno.Trim(),
                                 });


                    if (!string.IsNullOrEmpty(Filtro))
                    {
                        query = query.Where(Filtro);
                    }

                    if (!string.IsNullOrEmpty(Orden))
                    {
                        query = query.OrderBy(Orden);
                    }

                    List<ReporteImportacionesProductoAnalitico> objData = query.ToList();
                    objOperationResult.Success = 1;
                    return objData;
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;
            }

        }
        #endregion


        #region GenerarNotaIngresos


        public void ActualizaNotadeIngresoDesdeImportacion(ref OperationResult objOperationResult)
        {
            string NroImportacion = "";
            try
            {

                objOperationResult.Success = 1;
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {

                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {


                        var Importaciones = dbContext.importacion.Where(x => x.i_Eliminado == 0).ToList().OrderBy(x => x.v_IdImportacion).ToList();
                        if (Importaciones.Any())
                        {
                            Globals.ProgressbarStatus.i_Progress = 1;
                            Globals.ProgressbarStatus.i_TotalProgress = 1;
                            Globals.ProgressbarStatus.b_Cancelado = false;
                            Globals.ProgressbarStatus.i_TotalProgress = Importaciones.Count;

                        }

                        foreach (var import in Importaciones)
                        {
                            NroImportacion = import.v_Mes.Trim() + import.v_Correlativo.Trim();
                            if (NroImportacion == "0401000001")
                            {
                                string g = "";
                            }
                            var NotaIngreso = dbContext.movimiento.Where(i => i.i_IdTipoMovimiento == 1 && i.v_OrigenRegPeriodo.Trim() == import.v_Periodo.Trim() && i.v_OrigenRegCorrelativo.Trim() == import.v_Correlativo.Trim() && i.v_OrigenRegMes.Trim() == import.v_Mes.Trim() && i.i_Eliminado == 0 && i.v_OrigenTipo == "I").FirstOrDefault();
                            var ImportacionProducto = dbContext.importaciondetalleproducto.Where(c => c.i_Eliminado == 0 && c.v_IdImportacion == import.v_IdImportacion).ToList().OrderBy(x => x.v_IdImportacionDetalleProducto).ToList();
                            var Gastos = dbContext.importaciondetallegastos.Where(x => x.v_IdImportacion == import.v_IdImportacion && x.i_Eliminado == 0).ToList();
                            decimal Igv = decimal.Parse(dbContext.datahierarchy.Where(h => h.i_ItemId == import.i_Igv.Value && h.i_GroupId == 27 && h.i_IsDeleted.Value == 0).FirstOrDefault().v_Value1), otrosGastosDolares = 0, otrosGastosSoles = 0, TotalFobSoles = 0, TotalFobDolares = 0, FleteSoles = 0, FleteDolares = 0, SeguroSoles = 0;
                            decimal SeguroDolares = 0, AdvaloremSoles = 0, AdvaloremDolares = 0, IgvSoles = 0, IgvDolares = 0;
                            #region Fletes-Igv
                            switch (import.i_IdMoneda)
                            {

                                case 1:
                                    FleteSoles = import.d_Flete.Value;
                                    FleteDolares = import.d_TipoCambioDoc1.Value == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado(import.d_Flete.Value / import.d_TipoCambioDoc1.Value, 4);
                                    SeguroSoles = import.d_PagoSeguro.Value;
                                    SeguroDolares = import.d_TipoCambioDoc2 == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado(import.d_PagoSeguro.Value / import.d_TipoCambioDoc2.Value, 4);
                                    AdvaloremSoles = import.d_AdValorem.Value;
                                    AdvaloremDolares = import.d_TipoCambioDoc3 == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado(import.d_AdValorem.Value / import.d_TipoCambioDoc3.Value, 4);
                                    IgvSoles = import.d_Igv.Value;
                                    IgvDolares = import.d_TipoCambioDoc4 == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado(import.d_Igv.Value / import.d_TipoCambioDoc4.Value, 4);
                                    break;
                                case 2:
                                    FleteDolares = import.d_Flete.Value;
                                    FleteSoles = Utils.Windows.DevuelveValorRedondeado(import.d_Flete.Value * import.d_TipoCambioDoc1.Value, 4);
                                    SeguroDolares = import.d_PagoSeguro.Value;
                                    SeguroSoles = Utils.Windows.DevuelveValorRedondeado(import.d_PagoSeguro.Value * import.d_TipoCambioDoc2.Value, 4);
                                    AdvaloremDolares = import.d_AdValorem.Value;
                                    AdvaloremSoles = Utils.Windows.DevuelveValorRedondeado(import.d_AdValorem.Value * import.d_TipoCambioDoc3.Value, 4);
                                    IgvDolares = import.d_Igv.Value;
                                    IgvSoles = Utils.Windows.DevuelveValorRedondeado(import.d_Igv.Value * import.d_TipoCambioDoc4.Value, 4);
                                    break;
                            }
                            #endregion
                            #region Fob
                            var Fob = dbContext.importaciondetallefob.Where(x => x.i_Eliminado == 0 && x.v_IdImportacion == import.v_IdImportacion).ToList();
                            if (Fob.Any())
                            {
                                foreach (var Fila in Fob)
                                {
                                    switch (import.i_IdMoneda)
                                    {
                                        case 1:
                                            TotalFobSoles = TotalFobSoles + Fila.d_ValorFob.Value;
                                            TotalFobDolares = Fila.d_TipoCambio == 0 ? TotalFobDolares : TotalFobDolares + Utils.Windows.DevuelveValorRedondeado((Fila.d_ValorFob.Value / Fila.d_TipoCambio.Value), 2);
                                            TotalFobDolares = Utils.Windows.DevuelveValorRedondeado(TotalFobDolares, 2);
                                            break;
                                        case 2:
                                            TotalFobDolares = TotalFobDolares + Fila.d_ValorFob.Value;
                                            TotalFobSoles = TotalFobSoles + Utils.Windows.DevuelveValorRedondeado((Fila.d_ValorFob.Value * Fila.d_TipoCambio.Value), 2);
                                            TotalFobSoles = Utils.Windows.DevuelveValorRedondeado(TotalFobSoles, 2);
                                            break;
                                    }
                                }

                            }

                            #endregion
                            #region Gastos
                            foreach (var gastito in Gastos)
                            {

                                if (Igv != 0)
                                {
                                    decimal d_Igv = Utils.Windows.DevuelveValorRedondeado((gastito.d_ValorVenta.Value) * (Igv / 100), 2);
                                }

                                if (gastito.d_TipoCambio.Value != 0)
                                {
                                    decimal d_ImporteSoles = 0, d_ImporteDolares = 0;
                                    decimal VaVNoAfectSoles = 0, VaVNoAfectDolares = 0;
                                    switch (gastito.i_IdMoneda)
                                    {


                                        case 1:   //Soles
                                            d_ImporteSoles = d_ImporteSoles + Utils.Windows.DevuelveValorRedondeado(gastito.d_ValorVenta.Value + gastito.d_NAfectoDetraccion.Value + gastito.d_Igv.Value, 2);
                                            d_ImporteDolares = d_ImporteDolares + Utils.Windows.DevuelveValorRedondeado(gastito.d_ImporteSoles.Value / gastito.d_TipoCambio.Value, 2);


                                            VaVNoAfectSoles = gastito.d_ValorVenta.Value + gastito.d_NAfectoDetraccion.Value;
                                            VaVNoAfectDolares = Utils.Windows.DevuelveValorRedondeado((gastito.d_ValorVenta.Value + gastito.d_NAfectoDetraccion.Value) / gastito.d_TipoCambio.Value, 2);

                                            otrosGastosSoles = VaVNoAfectSoles + otrosGastosSoles;
                                            otrosGastosDolares = gastito.d_TipoCambio.Value == 0 ? otrosGastosDolares : VaVNoAfectDolares + otrosGastosDolares;
                                            otrosGastosSoles = Utils.Windows.DevuelveValorRedondeado(otrosGastosSoles, 2);

                                            break;

                                        case 2: //Dolares


                                            d_ImporteDolares = d_ImporteDolares + Utils.Windows.DevuelveValorRedondeado(gastito.d_ValorVenta.Value + gastito.d_NAfectoDetraccion.Value + gastito.d_Igv.Value, 2);
                                            d_ImporteSoles = d_ImporteSoles + Utils.Windows.DevuelveValorRedondeado((gastito.d_ImporteDolares.Value * gastito.d_TipoCambio.Value), 2);


                                            VaVNoAfectSoles = Utils.Windows.DevuelveValorRedondeado((gastito.d_ValorVenta.Value + gastito.d_NAfectoDetraccion.Value) * gastito.d_TipoCambio.Value, 2);
                                            VaVNoAfectDolares = gastito.d_ValorVenta.Value + gastito.d_NAfectoDetraccion.Value;


                                            otrosGastosDolares = gastito.d_ValorVenta.Value + gastito.d_NAfectoDetraccion.Value + otrosGastosDolares;
                                            otrosGastosSoles = gastito.d_TipoCambio == 0 ? otrosGastosSoles : Utils.Windows.DevuelveValorRedondeado((gastito.d_ValorVenta.Value + gastito.d_NAfectoDetraccion.Value) * gastito.d_TipoCambio.Value, 2) + otrosGastosSoles;
                                            otrosGastosDolares = Utils.Windows.DevuelveValorRedondeado(otrosGastosDolares, 2);



                                            break;
                                    }
                                }


                            }
                            #endregion
                            #region ProductoDetalles
                            foreach (var producto in ImportacionProducto)
                            {
                                if (NotaIngreso != null)
                                {
                                    decimal d_FobProductoSoles = 0, d_FobProductoDolares = 0;
                                    decimal d_SeguroProductoSoles = 0, d_SeguroProductoDolares = 0;
                                    decimal d_AdvaloremProductoSoles = 0, d_AdvaloremProductoDolares = 0;
                                    decimal d_IgvProductoSoles = 0, d_IgvProductoDolares = 0;
                                    decimal d_FleteProductoSoles = 0, d_FleteProductoDolares = 0;
                                    decimal d_OtrosGastosProductoSoles = 0, d_OtrosGastosProductoDolares = 0;
                                    decimal d_CostoUnitarioSoles = 0, d_CostoUnitarioDolares = 0;
                                    var NotaIngresoDetalle = dbContext.movimientodetalle.Where(x => x.v_IdMovimiento == NotaIngreso.v_IdMovimiento && x.v_IdProductoDetalle == producto.v_IdProductoDetalle && x.i_Eliminado == 0 && x.v_IdMovimientoDetalle == producto.v_IdMovimientoDetalle).FirstOrDefault();

                                    #region CalculoSoles


                                    if (producto.d_Cantidad != 0 && producto.d_Precio != 0)
                                    {

                                        d_FobProductoDolares = Utils.Windows.DevuelveValorRedondeado((producto.d_Cantidad.Value * producto.d_Precio.Value), 6);
                                        d_FobProductoSoles = Utils.Windows.DevuelveValorRedondeado(producto.d_Cantidad.Value * PrecioCambio(producto.v_IdCliente, producto.v_NroFactura, producto.v_IdImportacion, import.i_IdMoneda.Value, producto.d_Precio.Value), 6);

                                        d_FleteProductoDolares = TotalFobDolares == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((d_FobProductoDolares / TotalFobDolares) * FleteDolares, 6);
                                        d_FleteProductoSoles = TotalFobSoles == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((d_FobProductoSoles / TotalFobSoles) * FleteSoles, 6);
                                        // Fila.Cells["d_Flete"].Value = ValorFob == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado(((decimal.Parse(Fila.Cells["d_ValorFob"].Text.ToString()) / ValorFob) * flete), 6);

                                        d_SeguroProductoDolares = TotalFobDolares == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((d_FobProductoDolares / TotalFobDolares) * SeguroDolares, 6);
                                        d_SeguroProductoSoles = TotalFobSoles == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((d_FobProductoSoles / TotalFobSoles) * SeguroSoles, 6);
                                        //Fila.Cells["d_Seguro"].Value = ValorFob == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((decimal.Parse(Fila.Cells["d_ValorFob"].Text.ToString()) / ValorFob) * Seguro, 6);

                                        d_AdvaloremProductoDolares = TotalFobDolares == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((d_FobProductoDolares / TotalFobDolares) * AdvaloremDolares, 6);
                                        d_AdvaloremProductoSoles = TotalFobSoles == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((d_FobProductoSoles / TotalFobSoles) * AdvaloremSoles, 6);
                                        //Fila.Cells["d_AdValorem"].Value = ValorFob == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((decimal.Parse(Fila.Cells["d_ValorFob"].Text.ToString()) / ValorFob) * AdValorem, 6);


                                        d_IgvProductoDolares = TotalFobDolares == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((d_FobProductoDolares / TotalFobDolares) * IgvDolares, 6);
                                        d_IgvProductoSoles = TotalFobSoles == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((d_FobProductoSoles / TotalFobSoles) * IgvSoles, 6);
                                        //Fila.Cells["d_Igv"].Value = ValorFob == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((decimal.Parse(Fila.Cells["d_ValorFob"].Text.ToString()) / ValorFob) * Igv, 6);

                                        //Fila.Cells["d_OtrosGastos"].Value = ValorFob == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((decimal.Parse(Fila.Cells["d_ValorFob"].Text.ToString()) / ValorFob) * OtrosGastos, 6);

                                        d_OtrosGastosProductoDolares = TotalFobDolares == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((d_FobProductoDolares / TotalFobDolares) * otrosGastosDolares, 6);
                                        d_OtrosGastosProductoSoles = TotalFobSoles == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((d_FobProductoSoles / TotalFobSoles) * otrosGastosSoles, 6);

                                        d_CostoUnitarioDolares = producto.d_Cantidad == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado(((d_FobProductoDolares + d_FleteProductoDolares + d_SeguroProductoDolares + d_AdvaloremProductoDolares + d_OtrosGastosProductoDolares) / producto.d_Cantidad.Value), 6);
                                        d_CostoUnitarioSoles = producto.d_Cantidad == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado(((d_FobProductoSoles + d_FleteProductoSoles + d_SeguroProductoSoles + d_AdvaloremProductoSoles + d_OtrosGastosProductoSoles) / producto.d_Cantidad.Value), 6);



                                    }









                                    #endregion


                                    switch (import.i_IdMoneda.Value)
                                    {
                                        case 1:

                                            var costoguardado = producto.d_CostoUnitario;
                                            // NotaIngresoDetalle.d_Precio = d_CostoUnitarioSoles; //Utils.Windows.DevuelveValorRedondeado(producto.d_CostoUnitario.Value, 6); //Soles
                                            NotaIngresoDetalle.d_PrecioCambio = d_CostoUnitarioDolares;
                                            NotaIngresoDetalle.d_TotalCambio = Utils.Windows.DevuelveValorRedondeado((producto.d_Cantidad.Value * NotaIngresoDetalle.d_PrecioCambio.Value), 2);
                                            // NotaIngresoDetalle.d_Total = Utils.Windows.DevuelveValorRedondeado((NotaIngresoDetalle.d_Cantidad.Value * NotaIngresoDetalle.d_Precio.Value), 2);
                                            producto.d_CostoUnitarioCambio = NotaIngresoDetalle.d_PrecioCambio;
                                            break;
                                        case 2:
                                            var costoguardado1 = producto.d_CostoUnitario;
                                            // NotaIngresoDetalle.d_Precio = d_CostoUnitarioDolares;// Utils.Windows.DevuelveValorRedondeado(producto.d_CostoUnitario.Value, 6);//Dolares
                                            NotaIngresoDetalle.d_PrecioCambio = d_CostoUnitarioSoles;
                                            NotaIngresoDetalle.d_TotalCambio = Utils.Windows.DevuelveValorRedondeado((producto.d_Cantidad.Value * NotaIngresoDetalle.d_PrecioCambio.Value), 2);
                                            // NotaIngresoDetalle.d_Total = Utils.Windows.DevuelveValorRedondeado((NotaIngresoDetalle.d_Cantidad.Value * NotaIngresoDetalle.d_Precio.Value), 2);
                                            producto.d_CostoUnitarioCambio = NotaIngresoDetalle.d_PrecioCambio;
                                            break;
                                    }



                                    dbContext.movimientodetalle.ApplyCurrentValues(NotaIngresoDetalle);
                                    dbContext.importaciondetalleproducto.ApplyCurrentValues(producto);

                                }

                            }
                            Globals.ProgressbarStatus.i_Progress++;


                            #endregion



                        }
                        dbContext.SaveChanges();

                    }

                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;



                objOperationResult.AdditionalInformation = "ImportacionBL.ActualizaNotadeIngresoDesdeImportacion(): " + NroImportacion + "\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);


            }
        }




        public decimal PrecioCambio(string IdProveedor, string NroFactura, string IdImportacion, int Moneda, decimal Precio)
        {

            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {



                string[] TipoDocumento;
                string[] SerieCorrelativo;
                TipoDocumento = NroFactura.Split(new Char[] { ' ' });
                string TipoDocumentoSiglas = TipoDocumento[0].Trim();
                SerieCorrelativo = TipoDocumento[1].Trim().Split(new Char[] { '-' });
                string SerieDocumento = SerieCorrelativo[0].Trim();
                string CorrelativoDocumento = SerieCorrelativo[1].Trim();
                var TipoCambioFob = (from a in dbContext.importaciondetallefob

                                     join b in dbContext.documento on new { tipoDoc = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { tipoDoc = b.i_CodigoDocumento, eliminado = b.i_Eliminado.Value } into b_join

                                     from b in b_join.DefaultIfEmpty()
                                     where a.v_IdImportacion == IdImportacion && a.i_Eliminado == 0 && b.v_Siglas == TipoDocumentoSiglas && a.v_IdCliente == IdProveedor

                                     select new
                                     {

                                         d_TipoCambio = a.d_TipoCambio
                                     }).FirstOrDefault();

                decimal PrecioCambio = TipoCambioFob != null ? Moneda == (int)Currency.Dolares ? Utils.Windows.DevuelveValorRedondeado(Precio * TipoCambioFob.d_TipoCambio.Value, 6) : Utils.Windows.DevuelveValorRedondeado(Precio / TipoCambioFob.d_TipoCambio.Value, 6) : 0;

                return PrecioCambio;
            }


        }


        #endregion


    }
}
