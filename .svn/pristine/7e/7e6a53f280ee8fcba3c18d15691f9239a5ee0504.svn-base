using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic;
using System.ComponentModel;
using System.Transactions;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Tesoreria.BL;
using SAMBHS.Almacen.BL;
using SAMBHS.Common.BL;
using SAMBHS.Venta.BL;

namespace SAMBHS.Compra.BL
{
    public class LiquidacionCompraBL
    {
        private static string periodo = Globals.ClientSession != null ? (Globals.ClientSession.i_Periodo ?? DateTime.Now.Year).ToString() : DateTime.Now.Year.ToString();
        public List<KeyValueDTO> ObtenerListadoCompras(ref OperationResult pobjOperationResult, string pstrPeriodo, string pstrMes)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var almacenpredeterminado = Globals.ClientSession.i_IdAlmacenPredeterminado.Value.ToString("00");
                var query = (from n in dbContext.liquidacioncompra
                    where n.i_Eliminado == 0 && n.v_Periodo == pstrPeriodo && n.v_Mes == pstrMes && n.v_IdLiquidacionCompra.Substring(2, 2) == almacenpredeterminado
                    orderby n.v_Correlativo ascending
                    select new
                    {
                        v_Correlativo = n.v_Correlativo,
                        v_IdCompra = n.v_IdLiquidacionCompra
                    }
                );

                if (query.Any())
                {

                    var query2 = query.AsEnumerable()
                        .Select(x => new KeyValueDTO
                        {
                            Value1 = x.v_Correlativo,
                            Value2 = x.v_IdCompra
                        }).ToList();

                    return query2;
                }
                return new List<KeyValueDTO> { new KeyValueDTO { Value1 = almacenpredeterminado + "000000" } };
            }
        }

        public liquidacioncompraDto ObtenerCompraCabecera(ref OperationResult pobjOperationResult, string pstrIdCompra)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var objEntity = (from a in dbContext.liquidacioncompra

                                     join A in dbContext.cliente on a.v_IdProveedor equals A.v_IdCliente into A_join
                                     from A in A_join.DefaultIfEmpty()

                                     where a.v_IdLiquidacionCompra == pstrIdCompra
                                     select new liquidacioncompraDto
                                     {
                                         v_IdLiquidacionCompra = a.v_IdLiquidacionCompra,
                                         v_Periodo = a.v_Periodo,
                                         v_Mes = a.v_Mes,
                                         v_Correlativo = a.v_Correlativo,
                                         i_IdIgv = a.i_IdIgv,
                                         i_IdTipoDocumento = a.i_IdTipoDocumento,
                                         v_SerieDocumento = a.v_SerieDocumento,
                                         v_CorrelativoDocumento = a.v_CorrelativoDocumento,
                                         v_IdProveedor = a.v_IdProveedor,
                                         t_FechaRegistro = a.t_FechaRegistro,
                                         t_FechaEmision = a.t_FechaEmision,
                                         d_TipoCambio = a.d_TipoCambio,
                                         v_Glosa = a.v_Glosa,
                                         i_EsAfectoIgv = a.i_EsAfectoIgv,
                                         i_PreciosIncluyenIgv = a.i_PreciosIncluyenIgv,
                                         i_IdMoneda = a.i_IdMoneda,
                                         i_IdEstado = a.i_IdEstado,
                                         i_IdDestino = a.i_IdDestino,
                                         i_IdEstablecimiento = a.i_IdEstablecimiento,
                                         d_ValorVenta = a.d_ValorVenta,
                                         d_IGV = a.d_IGV,
                                         d_Total = a.d_Total,
                                         i_Eliminado = a.i_Eliminado,
                                         i_InsertaIdUsuario = a.i_InsertaIdUsuario,
                                         t_InsertaFecha = a.t_InsertaFecha,
                                         i_ActualizaIdUsuario = a.i_ActualizaIdUsuario,
                                         t_ActualizaFecha = a.t_ActualizaFecha,
                                         NombreProveedor = (A.v_ApePaterno + " " + A.v_ApeMaterno + " " + A.v_PrimerNombre + " " + A.v_RazonSocial).Trim(),
                                         CodigoProveedor = A.v_CodCliente,
                                         DNIProveedor = A.v_NroDocIdentificacion
                                     }
                                             ).FirstOrDefault();

                    pobjOperationResult.Success = 1;

                    return objEntity; 
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public BindingList<liquidacioncompradetalleDto> ObtenerCompraDetalles(ref OperationResult pobjOperationResult, string pstrIdCompra)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = (from n in dbContext.liquidacioncompradetalle

                             join A in dbContext.productodetalle on n.v_IdProductoDetalle equals A.v_IdProductoDetalle into A_join
                             from A in A_join.DefaultIfEmpty()

                             join B in dbContext.producto on A.v_IdProducto equals B.v_IdProducto into B_join
                             from B in B_join.DefaultIfEmpty()

                             join J1 in dbContext.datahierarchy on new { a = B.i_IdUnidadMedida.Value, b = 17 }
                                                            equals new { a = J1.i_ItemId, b = J1.i_GroupId } into J1_join
                             from J1 in J1_join.DefaultIfEmpty()

                             join J2 in dbContext.asientocontable on new {c= n.v_NroCuenta,eliminado =0 ,p=periodo } equals new {c= J2.v_NroCuenta ,eliminado=J2.i_Eliminado.Value ,p=J2.v_Periodo } into J2_join
                             from J2 in J2_join.DefaultIfEmpty()

                             join J3 in dbContext.datahierarchy on new { a = n.i_IdDestino.Value, b = 24 }
                                                             equals new { a = J3.i_ItemId, b = J3.i_GroupId } into J3_join
                             from J3 in J3_join.DefaultIfEmpty()

                             join C in dbContext.nodewarehouse on new { a = n.i_IdAlmacen.Value, b = 0 }
                                                             equals new { a = C.i_NodeWarehouseId, b = C.i_IsDeleted.Value } into C_join
                             from C in C_join.DefaultIfEmpty()

                             join J4 in dbContext.almacen on C.i_NodeWarehouseId equals J4.i_IdAlmacen into J4_join
                             from J4 in J4_join.DefaultIfEmpty()

                             where n.i_Eliminado == 0 && n.v_IdLiquidacionCompra.Equals(pstrIdCompra)
                             orderby n.t_InsertaFecha ascending

                             select new
                             {
                                 v_IdLiquidacionCompraDetalle = n.v_IdLiquidacionCompraDetalle,
                                 v_IdLiquidacionCompra = n.v_IdLiquidacionCompra,
                                 v_IdMovimientoDetalle = n.v_IdMovimientoDetalle,
                                 v_NroCuenta = n.v_NroCuenta,
                                 v_IdProductoDetalle = n.v_IdProductoDetalle,
                                 i_IdAlmacen = n.i_IdAlmacen,
                                 d_Cantidad = n.d_Cantidad,
                                 d_CantidadEmpaque = n.d_CantidadEmpaque,
                                 i_IdUnidadMedida = n.i_IdUnidadMedida,
                                 d_Precio = n.d_Precio,
                                 d_ValorVenta = n.d_ValorVenta,
                                 d_Igv = n.d_Igv,
                                 d_PrecioVenta = n.d_PrecioVenta,
                                 i_IdDestino = n.i_IdDestino,
                                 v_NroPedido = n.v_NroPedido,
                                 v_Glosa = n.v_Glosa,
                                 i_Eliminado = n.i_Eliminado,
                                 i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                 t_InsertaFecha = n.t_InsertaFecha,
                                 i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                 t_ActualizaFecha = n.t_ActualizaFecha,
                                 v_NombreProducto = B.v_Descripcion,
                                 v_CodigoInterno = B.v_CodInterno,
                                 Empaque = B.d_Empaque,// != null ? B.d_Empaque.ToString() : String.Empty,
                                 UMEmpaque = J1.v_Value1,
                                 i_EsServicio = B.i_EsServicio,// != null ? B.i_EsServicio.ToString() : String.Empty,
                                 i_IdUnidadMedidaProducto = B.i_IdUnidadMedida,// != null ? B.i_IdUnidadMedida.ToString() : String.Empty,
                                 i_ActivoFijo = B.i_EsActivoFijo,// == 1 ? true : false,
                                 _NombreCuenta = J2.v_NombreCuenta,
                                 _Destino = J3.v_Value1,
                                 _Almacen = J4.v_Nombre
                             }
                             ).ToList().Select(p => new liquidacioncompradetalleDto
                             {
                                 v_IdLiquidacionCompraDetalle = p.v_IdLiquidacionCompraDetalle,
                                 v_IdLiquidacionCompra = p.v_IdLiquidacionCompra,
                                 v_IdMovimientoDetalle = p.v_IdMovimientoDetalle,
                                 v_NroCuenta = p.v_NroCuenta,
                                 v_IdProductoDetalle = p.v_IdProductoDetalle,
                                 i_IdAlmacen = p.i_IdAlmacen,
                                 d_Cantidad = p.d_Cantidad,
                                 d_CantidadEmpaque = p.d_CantidadEmpaque,
                                 i_IdUnidadMedida = p.i_IdUnidadMedida,
                                 d_Precio = p.d_Precio,
                                 d_ValorVenta = p.d_ValorVenta,
                                 d_Igv = p.d_Igv,
                                 d_PrecioVenta = p.d_PrecioVenta,
                                 i_IdDestino = p.i_IdDestino,
                                 v_NroPedido = p.v_NroPedido,
                                 v_Glosa = p.v_Glosa,
                                 i_Eliminado = p.i_Eliminado,
                                 i_InsertaIdUsuario = p.i_InsertaIdUsuario,
                                 t_InsertaFecha = p.t_InsertaFecha,
                                 i_ActualizaIdUsuario = p.i_ActualizaIdUsuario,
                                 t_ActualizaFecha = p.t_ActualizaFecha,
                                 v_NombreProducto = p.v_NombreProducto,
                                 v_CodigoInterno = p.v_CodigoInterno,
                                 Empaque = p.Empaque != null ? p.Empaque.ToString() : String.Empty,
                                 UMEmpaque = p.UMEmpaque,
                                 i_EsServicio = p.i_EsServicio != null ? p.i_EsServicio.ToString() : String.Empty,
                                 i_IdUnidadMedidaProducto = p.i_IdUnidadMedida != null ? p.i_IdUnidadMedida.ToString() : String.Empty,
                                 i_ActivoFijo = p.i_ActivoFijo == 1 ? true : false,
                                 _NombreCuenta = p._NombreCuenta,
                                 _Destino = p._Destino,
                                 _Almacen = p._Almacen

                             }
                             ).ToList();
                return new BindingList<liquidacioncompradetalleDto>(query);
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LiquidacionCompraBL.ObtenerListadoCompras()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public void InsertarLiqCompra(ref OperationResult pobjOperationResult, liquidacioncompraDto pobjDtoEntity, List<string> ClientSession, List<liquidacioncompradetalleDto> pTemp_Insertar, string[] RegistroDiario)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        SecuentialBL objSecuentialBL = new SecuentialBL();
                        liquidacioncompra objEntityLiqCompra = liquidacioncompraAssembler.ToEntity(pobjDtoEntity);
                        liquidacioncompradetalleDto pobjDtoLiqCompraDetalle = new liquidacioncompradetalleDto();
                        MovimientoBL _objMovimientoBL = new MovimientoBL();
                        movimientoDto _movimientoDto = new movimientoDto();
                        movimientodetalleDto _movimientodetalleDto = new movimientodetalleDto();
                        DocumentoBL _objDocumentoBL = new DocumentoBL();
                        int SecuentialId = 0;
                        string newIdLiqCompra = string.Empty;
                        string newIdLiqCompraDetalle = string.Empty;
                        int intNodeId;
                        # region Inserta Nota Ingreso
                        if (pTemp_Insertar.Count() > 0 & pobjDtoEntity.i_IdEstado == 1)
                        {
                            _movimientoDto = new movimientoDto();
                            _movimientodetalleDto = new movimientodetalleDto();
                            List<movimientodetalleDto> _movimientodetalleDtos = new List<movimientodetalleDto>();
                            List<string> Almacenes = new List<string>();
                            string pstrAlmacen;
                            foreach (var Fila in pTemp_Insertar)
                            {
                                if (Fila.v_IdProductoDetalle != null)
                                {
                                    pstrAlmacen = Fila.i_IdAlmacen.Value.ToString();
                                    if (!Almacenes.Contains(pstrAlmacen))
                                    {
                                        Almacenes.Add(pstrAlmacen);
                                    }
                                }
                            }

                            if ((pTemp_Insertar.Find(p => p.i_EsServicio == "0") != null))
                            {

                                List<KeyValueDTO> ListaMovimientos = new List<KeyValueDTO>();
                                ListaMovimientos = _objMovimientoBL.ObtenerListadoMovimientos(ref pobjOperationResult, pobjDtoEntity.v_Periodo, pobjDtoEntity.v_Mes, (int)TipoDeMovimiento.NotadeIngreso);

                                int MaxMovimiento;
                                MaxMovimiento = ListaMovimientos.Count() > 0 ? int.Parse(ListaMovimientos[ListaMovimientos.Count() - 1].Value1.ToString()) : 0;
                                foreach (String Almacen in Almacenes)
                                {
                                    _movimientoDto = new movimientoDto();
                                    MaxMovimiento++;
                                    _movimientoDto.d_TipoCambio = pobjDtoEntity.d_TipoCambio;
                                    _movimientoDto.i_IdAlmacenOrigen = int.Parse(Almacen);
                                    _movimientoDto.i_IdMoneda = pobjDtoEntity.i_IdMoneda;
                                    _movimientoDto.i_IdTipoMotivo = 1;//COMPRA
                                    _movimientoDto.t_Fecha = pobjDtoEntity.t_FechaRegistro;
                                    _movimientoDto.v_Mes = pobjDtoEntity.v_Mes.Trim();
                                    _movimientoDto.v_Periodo = pobjDtoEntity.v_Periodo.Trim();
                                    _movimientoDto.i_IdTipoMovimiento = (int)TipoDeMovimiento.NotadeIngreso;
                                    _movimientoDto.v_Correlativo = MaxMovimiento.ToString("00000000");
                                    _movimientoDto.v_IdCliente = pobjDtoEntity.v_IdProveedor;
                                    _movimientoDto.v_OrigenTipo = "LC";
                                    _movimientoDto.i_EsDevolucion = 0;
                                    _movimientoDto.i_IdEstablecimiento = pobjDtoEntity.i_IdEstablecimiento.Value;
                                    _movimientoDto.v_OrigenRegCorrelativo = pobjDtoEntity.v_Correlativo;
                                    _movimientoDto.v_OrigenRegMes = pobjDtoEntity.v_Mes;
                                    _movimientoDto.v_OrigenRegPeriodo = pobjDtoEntity.v_Periodo;
                                    foreach (liquidacioncompradetalleDto _liquidacioncompradetalleproductoDto in pTemp_Insertar.Where(p => p.i_EsServicio == "0" && p.i_IdAlmacen == _movimientoDto.i_IdAlmacenOrigen).ToList())
                                    {
                                        _movimientodetalleDto = new movimientodetalleDto();
                                        _movimientodetalleDto.v_IdProductoDetalle = _liquidacioncompradetalleproductoDto.v_IdProductoDetalle.Trim();
                                        _movimientodetalleDto.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento;

                                        //var SerieDocumento = (from n in dbContext.datahierarchy
                                        //                      where n.i_GroupId == 53 && n.i_ItemId == pobjDtoEntity.i_IdSerieDocumento
                                        //                      select new { v_SerieDocumento = n.v_Value2 }).First();

                                        _movimientodetalleDto.v_NumeroDocumento = pobjDtoEntity.v_SerieDocumento.Trim() + "-" + pobjDtoEntity.v_CorrelativoDocumento.Trim();  //.ToString("0000") + "-" + pobjDtoEntity.v_CorrelativoDocumento.Trim();
                                        _movimientodetalleDto.d_Cantidad = _liquidacioncompradetalleproductoDto.d_Cantidad;
                                        _movimientodetalleDto.i_IdUnidad = _liquidacioncompradetalleproductoDto.i_IdUnidadMedida;
                                        _movimientodetalleDto.d_Precio = _liquidacioncompradetalleproductoDto.d_Precio;
                                        _movimientodetalleDto.d_Total = _liquidacioncompradetalleproductoDto.d_Cantidad * _liquidacioncompradetalleproductoDto.d_Precio;
                                        _movimientodetalleDto.v_NroPedido = _liquidacioncompradetalleproductoDto.v_NroPedido;
                                        _movimientodetalleDto.d_CantidadEmpaque = _liquidacioncompradetalleproductoDto.d_CantidadEmpaque;
                                        _movimientodetalleDtos.Add(_movimientodetalleDto);


                                    }
                                    _movimientoDto.d_TotalCantidad = _movimientodetalleDtos.Sum(p => p.d_Cantidad.Value);
                                    _movimientoDto.d_TotalPrecio = _movimientodetalleDtos.Sum(p => p.d_Total.Value);
                                    if (_movimientodetalleDtos.Any())
                                    {
                                        _objMovimientoBL.InsertarMovimiento(ref pobjOperationResult, _movimientoDto, Globals.ClientSession.GetAsList(), _movimientodetalleDtos);
                                    }

                                    _movimientodetalleDtos = new List<movimientodetalleDto>();

                                }

                            }

                        }

                        #endregion


                        #region Inserta Cabecera

                        objEntityLiqCompra.t_InsertaFecha = DateTime.Now;
                        objEntityLiqCompra.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntityLiqCompra.i_Eliminado = 0;

                        intNodeId = int.Parse(ClientSession[0]);
                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 96);
                        newIdLiqCompra = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "CQ");
                        objEntityLiqCompra.v_IdLiquidacionCompra = newIdLiqCompra;
                        dbContext.AddToliquidacioncompra(objEntityLiqCompra);
                        #endregion

                        #region Inserta Detalle
                        foreach (liquidacioncompradetalleDto LiqcompradetalleDto in pTemp_Insertar)
                        {
                            var NotadeIngresoDetalle = (from n in dbContext.movimientodetalle

                                                        join J1 in dbContext.movimiento on new { idMovimiento = n.v_IdMovimiento }
                                                                                        equals new { idMovimiento = J1.v_IdMovimiento } into J1_join
                                                        from J1 in J1_join.DefaultIfEmpty()

                                                        where n.i_Eliminado == 0 && n.v_IdProductoDetalle == LiqcompradetalleDto.v_IdProductoDetalle && J1.v_OrigenTipo == "LC" && J1.v_OrigenRegPeriodo == pobjDtoEntity.v_Periodo
                                                              && J1.v_OrigenRegMes == pobjDtoEntity.v_Mes && J1.v_OrigenRegCorrelativo == pobjDtoEntity.v_Correlativo

                                                        select new { n.v_IdMovimientoDetalle }).FirstOrDefault();


                            liquidacioncompradetalle objEntityLiqCompraDetalle = liquidacioncompradetalleAssembler.ToEntity(LiqcompradetalleDto);
                            SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 97);
                            newIdLiqCompraDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "CX");
                            objEntityLiqCompraDetalle.v_IdLiquidacionCompraDetalle = newIdLiqCompraDetalle;
                            objEntityLiqCompraDetalle.v_IdLiquidacionCompra = newIdLiqCompra;
                            if (NotadeIngresoDetalle != null)
                                objEntityLiqCompraDetalle.v_IdMovimientoDetalle = NotadeIngresoDetalle.v_IdMovimientoDetalle; //NotadeIngresoDetalle != null ? NotadeIngresoDetalle.v_IdMovimientoDetalle : null;
                            objEntityLiqCompraDetalle.t_InsertaFecha = DateTime.Now;
                            objEntityLiqCompraDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                            objEntityLiqCompraDetalle.i_Eliminado = 0;
                            dbContext.AddToliquidacioncompradetalle(objEntityLiqCompraDetalle);
                            Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "liquidacioncompradetalle", newIdLiqCompraDetalle);

                        }
                        dbContext.SaveChanges();
                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "liquidacioncompra", newIdLiqCompra);
                        #endregion

                        #region Genera Pagos Pendientes
                        if (newIdLiqCompra != null)
                        {
                            if (pobjDtoEntity.i_IdEstado == 1)
                            {
                                InsertaPagosPendiente(ref pobjOperationResult, newIdLiqCompra, pobjDtoEntity.d_Total.Value, Globals.ClientSession.GetAsList());
                                if (pobjOperationResult.Success == 0) return;
                            }
                        }

                        #endregion

                        #region Genera Libro Diario

                        if (pobjDtoEntity.i_IdEstado == 1)
                        {
                            string IDConcepto = pobjDtoEntity.i_IdMoneda.Value.ToString("00");

                            var aa = (from a in dbContext.administracionconceptos
                                      where a.v_Codigo == IDConcepto && a.i_Eliminado == 0 && a.v_Periodo.Equals(periodo)
                                      select new { a.v_CuentaIGV, a.v_CuentaPVenta }).FirstOrDefault();

                            if (aa != null && aa.v_CuentaIGV.Trim() != string.Empty && aa.v_CuentaPVenta.Trim() != string.Empty)
                            {
                                DiarioBL _objDiarioBL = new DiarioBL();
                                diarioDto _diarioDto = new diarioDto();
                                List<KeyValueDTO> _ListadoDiarios = new List<KeyValueDTO>();
                                List<diariodetalleDto> TempXInsertar = new List<diariodetalleDto>();

                                var DetalleCompra = (from d in dbContext.liquidacioncompradetalle
                                                     where d.v_IdLiquidacionCompra == newIdLiqCompra && d.i_Eliminado == 0
                                                     select d).ToList();

                                diariodetalleDto H_SubTotalVenta = new diariodetalleDto();
                                diariodetalleDto H_IGV = new diariodetalleDto();
                                diariodetalleDto D_PrecioVenta = new diariodetalleDto();
                                diariodetalleDto H_Percepcion = new diariodetalleDto();

                                OperationResult objOperationResult = new OperationResult();

                                if (RegistroDiario == null)
                                {
                                    _ListadoDiarios = _objDiarioBL.ObtenerListadoDiario(ref objOperationResult, pobjDtoEntity.t_FechaRegistro.Value.Year.ToString(), pobjDtoEntity.t_FechaRegistro.Value.Month.ToString("00"), 341);
                                    int _MaxMovimiento;
                                    _MaxMovimiento = _ListadoDiarios.Count() > 0 ? int.Parse(_ListadoDiarios[_ListadoDiarios.Count() - 1].Value1.ToString()) : 0;
                                    _MaxMovimiento++;
                                    _diarioDto.v_Periodo = pobjDtoEntity.t_FechaRegistro.Value.Year.ToString();
                                    _diarioDto.v_Mes = int.Parse(pobjDtoEntity.t_FechaRegistro.Value.Month.ToString()).ToString("00");
                                    _diarioDto.v_Correlativo = _MaxMovimiento.ToString("00000000");
                                }
                                else
                                {
                                    _diarioDto.v_Periodo = RegistroDiario[0];
                                    _diarioDto.v_Mes = RegistroDiario[1];
                                    _diarioDto.v_Correlativo = RegistroDiario[2];
                                }

                                _diarioDto.v_IdDocumentoReferencia = newIdLiqCompra;
                                _diarioDto.v_Nombre = pobjDtoEntity.NombreProveedor;
                                _diarioDto.v_Glosa = pobjDtoEntity.v_Glosa;
                                _diarioDto.d_TipoCambio = pobjDtoEntity.d_TipoCambio.Value;
                                _diarioDto.i_IdMoneda = pobjDtoEntity.i_IdMoneda.Value;
                                _diarioDto.i_IdTipoDocumento = 341; // D/V = Diario de Liquidación Compra
                                _diarioDto.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                _diarioDto.i_IdTipoComprobante = 2;

                                List<String> CuentasDetalle = new List<string>();

                                CuentasDetalle = DetalleCompra.Select(p => p.v_NroCuenta).Distinct().ToList();

                                #region SubTotalCompra
                                foreach (String CuentaDetalle in CuentasDetalle)
                                {
                                    decimal SubTotal = DetalleCompra.Where(p => p.v_NroCuenta == CuentaDetalle).Sum(o => o.d_ValorVenta.Value);
                                    H_SubTotalVenta.d_Importe = SubTotal > 0 ? SubTotal : SubTotal * -1;
                                    H_SubTotalVenta.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(SubTotal / pobjDtoEntity.d_TipoCambio.Value, 2) : Utils.Windows.DevuelveValorRedondeado(SubTotal * pobjDtoEntity.d_TipoCambio.Value, 2);
                                    H_SubTotalVenta.i_IdCentroCostos = "";
                                    H_SubTotalVenta.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                                    H_SubTotalVenta.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                    H_SubTotalVenta.v_IdCliente = pobjDtoEntity.v_IdProveedor;
                                    H_SubTotalVenta.v_Naturaleza = SubTotal > 0 ? pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H" : pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D";
                                    H_SubTotalVenta.v_NroDocumento = pobjDtoEntity.v_SerieDocumento.Trim() + "-" + pobjDtoEntity.v_CorrelativoDocumento.Trim();
                                    H_SubTotalVenta.v_NroCuenta = CuentaDetalle;

                                    TempXInsertar.Add(H_SubTotalVenta);
                                    H_SubTotalVenta = new diariodetalleDto();
                                }
                                #endregion

                                #region IGV
                                H_IGV.d_Importe = DetalleCompra.Sum(p => p.d_Igv.Value);
                                H_IGV.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(DetalleCompra.Sum(p => p.d_Igv.Value) / pobjDtoEntity.d_TipoCambio.Value, 2) : Utils.Windows.DevuelveValorRedondeado(DetalleCompra.Sum(p => p.d_Igv.Value) * pobjDtoEntity.d_TipoCambio.Value, 2);
                                H_IGV.i_IdCentroCostos = "";
                                H_IGV.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                                H_IGV.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                H_IGV.v_IdCliente = pobjDtoEntity.v_IdProveedor;
                                H_IGV.v_Naturaleza = pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H";
                                H_IGV.v_NroDocumento = pobjDtoEntity.v_SerieDocumento.Trim() + "-" + pobjDtoEntity.v_CorrelativoDocumento.Trim();
                                H_IGV.v_NroCuenta = (from a in dbContext.administracionconceptos
                                                     where a.v_Codigo == IDConcepto && a.i_Eliminado == 0 && a.v_Periodo.Equals(periodo)
                                                     select new { a.v_CuentaIGV }).First().v_CuentaIGV;
                                TempXInsertar.Add(H_IGV);
                                #endregion

                                #region PrecioVenta
                                D_PrecioVenta.d_Importe = DetalleCompra.Sum(p => p.d_PrecioVenta);
                                D_PrecioVenta.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(DetalleCompra.Sum(p => p.d_PrecioVenta.Value) / pobjDtoEntity.d_TipoCambio.Value, 2) : Utils.Windows.DevuelveValorRedondeado(DetalleCompra.Sum(p => p.d_PrecioVenta.Value) * pobjDtoEntity.d_TipoCambio.Value, 2);
                                D_PrecioVenta.i_IdCentroCostos = "";
                                D_PrecioVenta.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                                D_PrecioVenta.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                D_PrecioVenta.v_IdCliente = pobjDtoEntity.v_IdProveedor;
                                D_PrecioVenta.v_Naturaleza = pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D";
                                D_PrecioVenta.v_NroDocumento = pobjDtoEntity.v_SerieDocumento.Trim() + "-" + pobjDtoEntity.v_CorrelativoDocumento.Trim();
                                D_PrecioVenta.v_NroCuenta = (from a in dbContext.administracionconceptos
                                                             where a.v_Codigo == IDConcepto && a.i_Eliminado == 0 && a.v_Periodo.Equals(periodo)
                                                             select new { a.v_CuentaPVenta }).First().v_CuentaPVenta;
                                TempXInsertar.Add(D_PrecioVenta);
                                #endregion

                                decimal TotDebe, TotHaber, TotDebeC, TotHaberC;
                                TotDebe = TempXInsertar.Where(p => p.v_Naturaleza == "D" && p.v_NroCuenta != String.Empty).Sum(o => o.d_Importe.Value);
                                TotHaber = TempXInsertar.Where(p => p.v_Naturaleza == "H" && p.v_NroCuenta != String.Empty).Sum(o => o.d_Importe.Value);
                                TotDebeC = TempXInsertar.Where(p => p.v_Naturaleza == "D" && p.v_NroCuenta != String.Empty).Sum(o => o.d_Cambio.Value);
                                TotHaberC = TempXInsertar.Where(p => p.v_Naturaleza == "H" && p.v_NroCuenta != String.Empty).Sum(o => o.d_Cambio.Value);
                                _diarioDto.d_TotalDebe = TotDebe;
                                _diarioDto.d_TotalHaber = TotHaber;
                                _diarioDto.d_TotalDebeCambio = TotDebeC;
                                _diarioDto.d_TotalHaberCambio = TotHaberC;
                                _diarioDto.d_DiferenciaDebe = TotDebe - TotHaber;
                                _diarioDto.d_DiferenciaHaber = TotDebeC - TotHaberC;

                                _objDiarioBL.InsertarDiario(ref objOperationResult, _diarioDto, Globals.ClientSession.GetAsList(), TempXInsertar.Where(x => x.v_NroCuenta != string.Empty).ToList(), (int)TipoMovimientoTesoreria.Egreso);
                                if (objOperationResult.Success == 0) return;
                            }
                        }
                        #endregion


                        #region Actualiza Correlativo EmpresaDetalle
                        _objDocumentoBL.ActualizarCorrelativoPorSerie(ref pobjOperationResult, pobjDtoEntity.i_IdEstablecimiento, int.Parse(pobjDtoEntity.i_IdTipoDocumento.Value.ToString()), pobjDtoEntity.v_SerieDocumento, int.Parse(pobjDtoEntity.v_CorrelativoDocumento) + 1);
                        #endregion

                        dbContext.SaveChanges();
                        pobjOperationResult.Success = 1;
                        ts.Complete(); 
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LiquidacionCompraBL.InsertarLiqCompra()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public void ActualizarLiqCompra(ref OperationResult pobjOperationResult, liquidacioncompraDto pobjDtoEntity, List<string> ClientSession, List<liquidacioncompradetalleDto> pTemp_Insertar, List<liquidacioncompradetalleDto> pTemp_Editar, List<liquidacioncompradetalleDto> pTemp_Eliminar, List<movimientodetalleDto> _TempDetalleMovimiento_AgregarDto, List<movimientodetalleDto> _TempDetalleMovimientoDetalle_ModificarDto, List<movimientodetalleDto> _TempDetalleMovimientoDetalle_EliminarDto, List<string> Almacenes, decimal CantidadNotaIngreso)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        OperationResult objOperationResult = new OperationResult();
                        SecuentialBL objSecuentialBL = new SecuentialBL();
                        liquidacioncompra objEntityCompra = liquidacioncompraAssembler.ToEntity(pobjDtoEntity);
                        liquidacioncompradetalleDto pobjDtoCompraDetalle = new liquidacioncompradetalleDto();

                        MovimientoBL _objMovimientoBL = new MovimientoBL();
                        DocumentoBL _objDocumentoBL = new DocumentoBL();
                        int SecuentialId = 0;
                        string newIdCompraDetalle = string.Empty;
                        int intNodeId;
                        movimientoDto _movimientoDto = new movimientoDto();
                        List<movimientodetalleDto> _TempDetalle_AgregarFinal = new List<movimientodetalleDto>();
                        List<movimientodetalleDto> _TempDetalle_EliminarFinal = new List<movimientodetalleDto>();
                        List<movimientodetalleDto> _TempDetalle_ModificarFinal = new List<movimientodetalleDto>();


                        #region Actualiza Nota de Ingeso
                        foreach (String Almacen in Almacenes)
                        {
                            _movimientoDto = _objMovimientoBL.ObtenerMovimientoCabeceraDesdeCompras(ref objOperationResult, int.Parse(Almacen), "LC", pobjDtoEntity.v_Periodo, pobjDtoEntity.v_Mes.Trim(), pobjDtoEntity.v_Correlativo.Trim());
                            if (pobjDtoEntity.i_IdEstado != 0)
                            {
                                if (_movimientoDto != null)
                                {
                                    _movimientoDto.d_TipoCambio = pobjDtoEntity.d_TipoCambio == null ? 0 : pobjDtoEntity.d_TipoCambio.Value;
                                    _movimientoDto.i_IdAlmacenOrigen = int.Parse(Almacen);
                                    _movimientoDto.i_IdMoneda = pobjDtoEntity.i_IdMoneda;
                                    _movimientoDto.t_Fecha = pobjDtoEntity.t_FechaRegistro;
                                    _movimientoDto.v_Glosa = pobjDtoEntity.v_Glosa;
                                    _movimientoDto.v_Mes = pobjDtoEntity.v_Mes;
                                    _movimientoDto.v_Periodo = pobjDtoEntity.v_Periodo;
                                    _movimientoDto.i_EsDevolucion = _objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? 1 : 0; // _EsNotadeCredito == true ? 1 : 0;
                                    _movimientoDto.v_IdCliente = pobjDtoEntity.v_IdProveedor;    // _compraDto.v_IdProveedor;
                                    _movimientoDto.d_TotalPrecio = pobjDtoEntity.d_Total;
                                    _movimientoDto.i_IdEstablecimiento = pobjDtoEntity.i_IdEstablecimiento.Value;
                                    //if (_EsNotadeCredito == true)
                                    //{
                                    //    for (int i = 0; i < grdData.Rows.Count(); i++)
                                    //    {
                                    //        grdData.Rows[i].Cells["i_RegistroEstado"].Value = "Modificado";
                                    //    }
                                    //}
                                    //  LlenarTemporalesMovimiento(Almacen);
                                    // decimal Cantidad = 0, Total = 0, _Cantidad, _Total;
                                    //  pTemp_Insertar 


                                    var _TempDetalleMovimiento_AgregarDto1 = _TempDetalleMovimiento_AgregarDto.Where(x => x.i_IdAlmacen == Almacen && x.v_IdProductoDetalle != null && x.EsServicio == 0).ToList(); //No servicios

                                    foreach (var item in _TempDetalleMovimiento_AgregarDto1)
                                    {
                                        movimientodetalleDto obj = new movimientodetalleDto();
                                        obj = item;
                                        obj.v_IdMovimiento = _movimientoDto.v_IdMovimiento;
                                        _TempDetalle_AgregarFinal.Add(obj);
                                    }

                                    var _TempDetalleMovimientoDetalle_ModificarDto1 = _TempDetalleMovimientoDetalle_ModificarDto.Where(x => x.i_IdAlmacen == Almacen && x.v_IdProductoDetalle != null && x.EsServicio == 0).ToList(); //No servicios

                                    foreach (var item in _TempDetalleMovimientoDetalle_ModificarDto1)
                                    {
                                        movimientodetalleDto obj = new movimientodetalleDto();
                                        obj = item;
                                        obj.v_IdMovimiento = _movimientoDto.v_IdMovimiento;
                                        _TempDetalle_ModificarFinal.Add(obj);

                                    }
                                    var _TempDetalleMovimientoDetalle_EliminarDto1 = _TempDetalleMovimientoDetalle_EliminarDto.Where(x => x.i_IdAlmacen == Almacen && x.v_IdProductoDetalle != null && x.EsServicio == 0).ToList(); //No servicios

                                    foreach (var item in _TempDetalleMovimientoDetalle_EliminarDto1)
                                    {
                                        movimientodetalleDto obj = new movimientodetalleDto();
                                        obj = item;
                                        obj.v_IdMovimiento = _movimientoDto.v_IdMovimiento;
                                        _TempDetalle_EliminarFinal.Add(obj);
                                    }
                                    _movimientoDto.d_TotalCantidad = CantidadNotaIngreso;
                                    _objMovimientoBL.ActualizarMovimiento(ref objOperationResult, _movimientoDto, Globals.ClientSession.GetAsList(), _TempDetalle_AgregarFinal, _TempDetalle_ModificarFinal, _TempDetalle_EliminarFinal);
                                    _TempDetalle_AgregarFinal = new List<movimientodetalleDto>();
                                    _TempDetalle_ModificarFinal = new List<movimientodetalleDto>();
                                    _TempDetalle_EliminarFinal = new List<movimientodetalleDto>();
                                }
                                else
                                {
                                    _movimientoDto = new movimientoDto();
                                    List<KeyValueDTO> ListaMovimientos = new List<KeyValueDTO>();
                                    ListaMovimientos = _objMovimientoBL.ObtenerListadoMovimientos(ref objOperationResult, pobjDtoEntity.v_Periodo, pobjDtoEntity.v_Mes, (int)TipoDeMovimiento.NotadeIngreso);

                                    int MaxMovimiento;
                                    MaxMovimiento = ListaMovimientos.Count() > 0 ? int.Parse(ListaMovimientos[ListaMovimientos.Count() - 1].Value1.ToString()) : 0;
                                    MaxMovimiento++;

                                    _movimientoDto.d_TipoCambio = pobjDtoEntity.d_TipoCambio == null ? 0 : pobjDtoEntity.d_TipoCambio.Value;
                                    _movimientoDto.i_IdAlmacenOrigen = int.Parse(Almacen);
                                    _movimientoDto.i_IdMoneda = pobjDtoEntity.i_IdMoneda.Value;
                                    _movimientoDto.i_IdTipoMotivo = 1;//Compra nacional
                                    _movimientoDto.t_Fecha = pobjDtoEntity.t_FechaRegistro;
                                    _movimientoDto.v_Glosa = pobjDtoEntity.v_Glosa;
                                    _movimientoDto.v_Mes = pobjDtoEntity.v_Mes;
                                    _movimientoDto.v_Periodo = pobjDtoEntity.v_Periodo;
                                    _movimientoDto.i_IdTipoMovimiento = (int)TipoDeMovimiento.NotadeIngreso;
                                    _movimientoDto.v_Correlativo = MaxMovimiento.ToString("00000000");
                                    _movimientoDto.v_IdCliente = pobjDtoEntity.v_IdProveedor;
                                    _movimientoDto.v_OrigenTipo = "LC";
                                    _movimientoDto.v_OrigenRegCorrelativo = pobjDtoEntity.v_Correlativo;//  txtCorrelativo.Text;
                                    _movimientoDto.v_OrigenRegMes = pobjDtoEntity.v_Mes;
                                    _movimientoDto.v_OrigenRegPeriodo = pobjDtoEntity.v_Periodo;
                                    _movimientoDto.d_TotalPrecio = pobjDtoEntity.d_Total;
                                    _movimientoDto.i_IdEstablecimiento = pobjDtoEntity.i_IdEstablecimiento;
                                    _movimientoDto.d_TotalCantidad = CantidadNotaIngreso;
                                    _TempDetalle_AgregarFinal = _TempDetalleMovimiento_AgregarDto.Where(x => x.i_IdAlmacen == Almacen && x.v_IdProductoDetalle != null && x.EsServicio == 0).ToList(); //No servicios
                                    if (_TempDetalle_AgregarFinal.Count() > 0)
                                    {
                                        _objMovimientoBL.InsertarMovimiento(ref objOperationResult, _movimientoDto, Globals.ClientSession.GetAsList(), _TempDetalleMovimiento_AgregarDto);
                                        _TempDetalle_AgregarFinal = new List<movimientodetalleDto>();
                                    }
                                }
                            }
                            else
                            {
                                if (_movimientoDto != null && _movimientoDto.v_IdMovimiento != null)
                                    _objMovimientoBL.EliminarMovimiento(ref objOperationResult, _movimientoDto.v_IdMovimiento, Globals.ClientSession.GetAsList());
                            }
                        }
                        // dbContext.SaveChanges();
                        #endregion

                        #region Actualiza Cabecera
                        intNodeId = int.Parse(ClientSession[0]);
                        var objEntitySource = (from a in dbContext.liquidacioncompra
                                               where a.v_IdLiquidacionCompra == pobjDtoEntity.v_IdLiquidacionCompra
                                               select a).FirstOrDefault();

                        pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                        pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                        liquidacioncompra objEntity = liquidacioncompraAssembler.ToEntity(pobjDtoEntity);
                        dbContext.liquidacioncompra.ApplyCurrentValues(objEntity);
                        #endregion

                        #region Actualiza Detalle
                        foreach (liquidacioncompradetalleDto LiqcompradetalleDto in pTemp_Insertar)
                        {

                            var NotadeIngresoDetalle = (from n in dbContext.movimientodetalle

                                                        join J1 in dbContext.movimiento on new { idMovimiento = n.v_IdMovimiento }
                                                                                        equals new { idMovimiento = J1.v_IdMovimiento } into J1_join
                                                        from J1 in J1_join.DefaultIfEmpty()

                                                        where n.i_Eliminado == 0 && n.v_IdProductoDetalle == LiqcompradetalleDto.v_IdProductoDetalle && J1.v_OrigenTipo == "LC" && J1.v_OrigenRegPeriodo == pobjDtoEntity.v_Periodo
                                                              && J1.v_OrigenRegMes == pobjDtoEntity.v_Mes && J1.v_OrigenRegCorrelativo == pobjDtoEntity.v_Correlativo

                                                        select new { n.v_IdMovimientoDetalle }).FirstOrDefault();


                            liquidacioncompradetalle objEntityCompraDetalle = liquidacioncompradetalleAssembler.ToEntity(LiqcompradetalleDto);
                            SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 97);
                            newIdCompraDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "CX");
                            objEntityCompraDetalle.v_IdLiquidacionCompraDetalle = newIdCompraDetalle;
                            if (NotadeIngresoDetalle != null)
                                objEntityCompraDetalle.v_IdMovimientoDetalle = NotadeIngresoDetalle.v_IdMovimientoDetalle;//NotadeIngresoDetalle != null ? NotadeIngresoDetalle.v_IdMovimientoDetalle : null;
                            objEntityCompraDetalle.v_IdLiquidacionCompra = objEntityCompra.v_IdLiquidacionCompra;
                            objEntityCompraDetalle.t_InsertaFecha = DateTime.Now;
                            objEntityCompraDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                            objEntityCompraDetalle.i_Eliminado = 0;
                            dbContext.AddToliquidacioncompradetalle(objEntityCompraDetalle);
                            dbContext.SaveChanges();
                            Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "liquidacioncompradetalle", newIdCompraDetalle);
                            if (LiqcompradetalleDto.i_ActivoFijo)
                            {
                                //ActualizarActivoFijo(pobjDtoEntity, compradetalleDto, ClientSession);
                            }
                        }

                        foreach (liquidacioncompradetalleDto compradetalleDto in pTemp_Editar)
                        {
                            liquidacioncompradetalle _objEntity = liquidacioncompradetalleAssembler.ToEntity(compradetalleDto);
                            var query = (from n in dbContext.liquidacioncompradetalle
                                         where n.v_IdLiquidacionCompraDetalle == compradetalleDto.v_IdLiquidacionCompraDetalle
                                         select n).FirstOrDefault();

                            _objEntity.t_ActualizaFecha = DateTime.Now;
                            _objEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                            dbContext.liquidacioncompradetalle.ApplyCurrentValues(_objEntity);
                            Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "liquidacioncompradetalle", query.v_IdLiquidacionCompraDetalle);
                            dbContext.SaveChanges();
                            //if (compradetalleDto.EsActivoFijo == 1)
                            //{
                            //    ActualizarActivoFijo(pobjDtoEntity, compradetalleDto, ClientSession);
                            //}
                        }

                        if (pTemp_Editar.Count() == 0)
                        {
                            var ComprasDetalles = (from a in dbContext.producto
                                                   join b in dbContext.productodetalle on new { a = a.v_IdProducto, eliminado = 0 } equals new { a = b.v_IdProducto, eliminado = b.i_Eliminado.Value } into b_join
                                                   from b in b_join.DefaultIfEmpty()
                                                   join c in dbContext.liquidacioncompradetalle on new { b = b.v_IdProductoDetalle, eliminado = 0 } equals new { b = c.v_IdProductoDetalle, eliminado = c.i_Eliminado.Value } into c_join
                                                   from c in c_join.DefaultIfEmpty()
                                                   where a.i_Eliminado.Value == 0 && c.v_IdLiquidacionCompra == pobjDtoEntity.v_IdLiquidacionCompra

                                                   select new liquidacioncompradetalleDto
                                                   {
                                                       v_IdLiquidacionCompra = c.v_IdLiquidacionCompra,
                                                       v_IdLiquidacionCompraDetalle = c.v_IdLiquidacionCompraDetalle,
                                                       d_PrecioVenta = c.d_PrecioVenta,
                                                       i_ActivoFijo = a.i_EsActivoFijo == 1 ? true : false,
                                                       v_IdProductoDetalle = c.v_IdProductoDetalle,
                                                   }).ToList();
                            //foreach (var objCompraDetalle in ComprasDetalles)
                            //{

                            //    if (objCompraDetalle.EsActivoFijo == 1)
                            //    {
                            //        ActualizarActivoFijo(pobjDtoEntity, objCompraDetalle, ClientSession);
                            //    }

                            //}
                        }

                        foreach (liquidacioncompradetalleDto compradetalleDto in pTemp_Eliminar)
                        {
                            liquidacioncompradetalle _objEntity = liquidacioncompradetalleAssembler.ToEntity(compradetalleDto);
                            var query = (from n in dbContext.liquidacioncompradetalle
                                         where n.v_IdLiquidacionCompraDetalle == compradetalleDto.v_IdLiquidacionCompraDetalle
                                         select n).FirstOrDefault();

                            if (query != null)
                            {
                                query.t_ActualizaFecha = DateTime.Now;
                                query.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                                query.i_Eliminado = 1;
                                //if (compradetalleDto.i_ActivoFijo)
                                //{
                                //    new ActivoFijoBL().EliminarActivoFijoDesdeCompras(ref objOperationResult, ClientSession, query.v_IdProductoDetalle);
                                //}

                            }
                            dbContext.liquidacioncompradetalle.ApplyCurrentValues(query);
                            Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "liquidacioncompradetalle", query.v_IdLiquidacionCompraDetalle);
                            dbContext.SaveChanges();
                        }
                        #endregion

                        #region Genera Pago Pendiente
                        if (pobjDtoEntity.v_IdLiquidacionCompra != null)
                        {
                            EliminarPagoPendiente(ref pobjOperationResult, pobjDtoEntity.v_IdLiquidacionCompra, Globals.ClientSession.GetAsList());
                            if (pobjDtoEntity.i_IdEstado == 1) InsertaPagosPendiente(ref pobjOperationResult, pobjDtoEntity.v_IdLiquidacionCompra, pobjDtoEntity.d_Total.Value, Globals.ClientSession.GetAsList());
                        }
                        if (pobjOperationResult.Success == 0) return;
                        #endregion

                        #region Regenera Libro Diario
                        string IDConcepto = pobjDtoEntity.i_IdMoneda.Value.ToString("00");

                        var aa = (from a in dbContext.administracionconceptos
                                  where a.v_Codigo == IDConcepto && a.i_Eliminado == 0 && a.v_Periodo.Equals(periodo)
                                  select new { a.v_CuentaIGV, a.v_CuentaPVenta }).FirstOrDefault();

                        if (aa != null && aa.v_CuentaIGV.Trim() != string.Empty && aa.v_CuentaPVenta.Trim() != string.Empty)
                        {
                            DiarioBL _objDiarioBL = new DiarioBL();
                            diarioDto _diarioDto = new diarioDto();
                            List<KeyValueDTO> _ListadoDiarios = new List<KeyValueDTO>();
                            List<diariodetalleDto> TempXInsertar = new List<diariodetalleDto>();

                            var DetalleCompra = (from d in dbContext.liquidacioncompradetalle
                                                 where d.v_IdLiquidacionCompra == pobjDtoEntity.v_IdLiquidacionCompra && d.i_Eliminado == 0
                                                 select d).ToList();

                            diariodetalleDto H_SubTotalVenta = new diariodetalleDto();
                            diariodetalleDto H_IGV = new diariodetalleDto();
                            diariodetalleDto D_PrecioVenta = new diariodetalleDto();

                            string[] IdDiarioEliminado = new string[3];

                            try
                            {
                                IdDiarioEliminado = _objDiarioBL.EliminarDiarioXDocRef(ref pobjOperationResult, pobjDtoEntity.v_IdLiquidacionCompra, ClientSession, false);
                                if (pobjOperationResult.Success == 0) return;
                            }
                            finally
                            {

                                if (pobjDtoEntity.i_IdEstado == 1)
                                {

                                    if (IdDiarioEliminado[0] == null || ( IdDiarioEliminado !=null && (IdDiarioEliminado[1].Trim () != pobjDtoEntity.t_FechaRegistro.Value.Month.ToString("00") || IdDiarioEliminado[0] != pobjDtoEntity.t_FechaRegistro.Value.Year.ToString())))
                                    {
                                        int _MaxMovimiento;
                                        _ListadoDiarios = _objDiarioBL.ObtenerListadoDiario(ref pobjOperationResult, pobjDtoEntity.t_FechaRegistro.Value.Year.ToString(), pobjDtoEntity.t_FechaRegistro.Value.Month.ToString("00"), 341);
                                        _MaxMovimiento = _ListadoDiarios.Count() > 0 ? int.Parse(_ListadoDiarios[_ListadoDiarios.Count() - 1].Value1.ToString()) : 0;
                                        _MaxMovimiento++;
                                        _diarioDto.v_Periodo = pobjDtoEntity.t_FechaRegistro.Value.Year.ToString();
                                        _diarioDto.v_Mes = int.Parse(pobjDtoEntity.t_FechaRegistro.Value.Month.ToString()).ToString("00");
                                        _diarioDto.v_Correlativo = _MaxMovimiento.ToString("00000000");
                                    }
                                    else
                                    {
                                        _diarioDto.v_Periodo = IdDiarioEliminado[0];
                                        _diarioDto.v_Mes = IdDiarioEliminado[1];
                                        _diarioDto.v_Correlativo = IdDiarioEliminado[2];
                                    }

                                    _diarioDto.v_IdDocumentoReferencia = pobjDtoEntity.v_IdLiquidacionCompra;
                                    _diarioDto.v_Nombre = pobjDtoEntity.NombreProveedor;
                                    _diarioDto.v_Glosa = pobjDtoEntity.v_Glosa;
                                    _diarioDto.d_TipoCambio = pobjDtoEntity.d_TipoCambio.Value;
                                    _diarioDto.i_IdMoneda = pobjDtoEntity.i_IdMoneda.Value;
                                    _diarioDto.i_IdTipoDocumento = 341;
                                    _diarioDto.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                    _diarioDto.i_IdTipoComprobante = 2;

                                    List<String> CuentasDetalle = new List<string>();

                                    CuentasDetalle = DetalleCompra.Select(p => p.v_NroCuenta).Distinct().ToList();

                                    foreach (String CuentaDetalle in CuentasDetalle)
                                    {
                                        decimal SubTotal = DetalleCompra.Where(p => p.v_NroCuenta == CuentaDetalle).Sum(o => o.d_ValorVenta.Value);
                                        H_SubTotalVenta.d_Importe = SubTotal > 0 ? SubTotal : SubTotal * -1;
                                        H_SubTotalVenta.d_Cambio = Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.i_IdMoneda.Value == 1 ? SubTotal / pobjDtoEntity.d_TipoCambio.Value : SubTotal * pobjDtoEntity.d_TipoCambio.Value, 2);
                                        H_SubTotalVenta.i_IdCentroCostos = "";
                                        H_SubTotalVenta.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                                        H_SubTotalVenta.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                        H_SubTotalVenta.v_IdCliente = pobjDtoEntity.v_IdProveedor;
                                        H_SubTotalVenta.v_Naturaleza = SubTotal > 0 ? pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H" : pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D";
                                        H_SubTotalVenta.v_NroDocumento = pobjDtoEntity.v_SerieDocumento.Trim() + "-" + pobjDtoEntity.v_CorrelativoDocumento.Trim();
                                        H_SubTotalVenta.v_NroCuenta = CuentaDetalle;

                                        TempXInsertar.Add(H_SubTotalVenta);
                                        H_SubTotalVenta = new diariodetalleDto();
                                    }

                                    H_IGV.d_Importe = DetalleCompra.Sum(p => p.d_Igv.Value);
                                    H_IGV.d_Cambio = Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.i_IdMoneda.Value == 1 ? DetalleCompra.Sum(p => p.d_Igv.Value) / pobjDtoEntity.d_TipoCambio.Value : DetalleCompra.Sum(p => p.d_Igv.Value) * pobjDtoEntity.d_TipoCambio.Value, 2);
                                    H_IGV.i_IdCentroCostos = "";
                                    H_IGV.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                                    H_IGV.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                    H_IGV.v_IdCliente = pobjDtoEntity.v_IdProveedor;
                                    H_IGV.v_Naturaleza = pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H";
                                    H_IGV.v_NroDocumento = pobjDtoEntity.v_SerieDocumento.Trim() + "-" + pobjDtoEntity.v_CorrelativoDocumento.Trim();
                                    H_IGV.v_NroCuenta = (from a in dbContext.administracionconceptos
                                                         where a.v_Codigo == IDConcepto && a.i_Eliminado == 0 && a.v_Periodo.Equals(periodo)
                                                         select new { a.v_CuentaIGV }).First().v_CuentaIGV;
                                    TempXInsertar.Add(H_IGV);

                                    D_PrecioVenta.d_Importe = DetalleCompra.Sum(p => p.d_PrecioVenta);
                                    D_PrecioVenta.d_Cambio = Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.i_IdMoneda.Value == 1 ? DetalleCompra.Sum(p => p.d_PrecioVenta.Value) / pobjDtoEntity.d_TipoCambio.Value : DetalleCompra.Sum(p => p.d_PrecioVenta.Value) * pobjDtoEntity.d_TipoCambio.Value, 2);
                                    D_PrecioVenta.i_IdCentroCostos = "";
                                    D_PrecioVenta.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                                    D_PrecioVenta.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                    D_PrecioVenta.v_IdCliente = pobjDtoEntity.v_IdProveedor;
                                    D_PrecioVenta.v_Naturaleza = pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D";
                                    D_PrecioVenta.v_NroDocumento = pobjDtoEntity.v_SerieDocumento.Trim() + "-" + pobjDtoEntity.v_CorrelativoDocumento.Trim();
                                    D_PrecioVenta.v_NroCuenta = (from a in dbContext.administracionconceptos
                                                                 where a.v_Codigo == IDConcepto && a.i_Eliminado == 0 && a.v_Periodo.Equals(periodo)
                                                                 select new { a.v_CuentaPVenta }).First().v_CuentaPVenta;
                                    TempXInsertar.Add(D_PrecioVenta);

                                    decimal TotDebe, TotHaber, TotDebeC, TotHaberC;
                                    TotDebe = TempXInsertar.Where(p => p.v_Naturaleza == "D" && p.v_NroCuenta != String.Empty).Sum(o => o.d_Importe.Value);
                                    TotHaber = TempXInsertar.Where(p => p.v_Naturaleza == "H" && p.v_NroCuenta != String.Empty).Sum(o => o.d_Importe.Value);
                                    TotDebeC = TempXInsertar.Where(p => p.v_Naturaleza == "D" && p.v_NroCuenta != String.Empty).Sum(o => o.d_Cambio.Value);
                                    TotHaberC = TempXInsertar.Where(p => p.v_Naturaleza == "H" && p.v_NroCuenta != String.Empty).Sum(o => o.d_Cambio.Value);
                                    _diarioDto.d_TotalDebe = TotDebe;
                                    _diarioDto.d_TotalHaber = TotHaber;
                                    _diarioDto.d_TotalDebeCambio = TotDebeC;
                                    _diarioDto.d_TotalHaberCambio = TotHaberC;
                                    _diarioDto.d_DiferenciaDebe = TotDebe - TotHaber;
                                    _diarioDto.d_DiferenciaHaber = TotDebeC - TotHaberC;

                                    _objDiarioBL.InsertarDiario(ref pobjOperationResult, _diarioDto, Globals.ClientSession.GetAsList(), TempXInsertar.Where(x => x.v_NroCuenta != string.Empty).ToList(), (int)TipoMovimientoTesoreria.Egreso);
                                  
                                }
                               
                            }
                            if (pobjOperationResult.Success == 0) return;

                        }
                        #endregion

                        Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "liquidacioncompra", objEntitySource.v_IdLiquidacionCompra);
                        dbContext.SaveChanges();
                        pobjOperationResult.Success = 1;
                        ts.Complete();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LiquidacionComprasBL.ActualizarCompra()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }

        public void EliminarLiqCompra(ref OperationResult pobjOperationResult, string pstrIdCompra, List<string> ClientSession)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                    #region Elimina Cabecera
                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.liquidacioncompra
                                           where a.v_IdLiquidacionCompra == pstrIdCompra
                                           select a).FirstOrDefault();

                    // Crear la entidad con los datos actualizados
                    objEntitySource.t_ActualizaFecha = DateTime.Now;
                    objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntitySource.i_Eliminado = 1;
                    #endregion

                    #region Elimina Detalles
                    //Eliminar detalles del movimiento eliminado.
                    var objEntitySourceDetallesCompra = (from a in dbContext.liquidacioncompradetalle
                                                         where a.v_IdLiquidacionCompra == pstrIdCompra
                                                         select a).ToList();

                    foreach (var RegistroCompraDetalle in objEntitySourceDetallesCompra)
                    {
                        RegistroCompraDetalle.t_ActualizaFecha = DateTime.Now;
                        RegistroCompraDetalle.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        RegistroCompraDetalle.i_Eliminado = 1;
                        Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "liquidacioncompradetalle", RegistroCompraDetalle.v_IdLiquidacionCompraDetalle);

                        //if (VerificarSiEsActivoFijo(RegistroCompraDetalle.v_IdProductoDetalle))
                        //{
                        //    new ActivoFijoBL().EliminarActivoFijoDesdeCompras(ref pobjOperationResult, ClientSession, RegistroCompraDetalle.v_IdProductoDetalle);

                        //}
                    }
                    #endregion

                    #region Elimina Notas de Ingresos Relacionadas
                    MovimientoBL objMovimientoBL = new MovimientoBL();

                    var NotasDeIngresos = (from a in dbContext.movimiento
                                           where a.v_OrigenTipo == "LC" && a.v_OrigenRegPeriodo == objEntitySource.v_Periodo && a.v_OrigenRegMes == objEntitySource.v_Mes
                                           && a.v_OrigenRegCorrelativo == objEntitySource.v_Correlativo
                                           select new { a.v_IdMovimiento }).ToList();

                    foreach (var NotadeIngreso in NotasDeIngresos)
                    {
                        objMovimientoBL.EliminarMovimiento(ref pobjOperationResult, NotadeIngreso.v_IdMovimiento, ClientSession);
                        if (pobjOperationResult.Success == 0) return;
                    }
                    #endregion

                    #region Elimina Cobranza Pendiente
                    if (objEntitySource.i_IdEstado != 0)
                    {
                        EliminarPagoPendiente(ref pobjOperationResult, pstrIdCompra, ClientSession);
                        if (pobjOperationResult.Success == 0) return;
                    }
                    #endregion

                    #region Elimina Libro Diario
                    DiarioBL _objDiarioBL = new DiarioBL();
                    OperationResult objOperationResult = new OperationResult();
                    _objDiarioBL.EliminarDiarioXDocRef(ref objOperationResult, pstrIdCompra, ClientSession, true);
                    if (objOperationResult.Success == 0) return;
                    #endregion

                    #region Actualiza Detalles de Orden de Compra Si es Necesario
                    //if (!string.IsNullOrEmpty(objEntitySource.v_ODCSerie) && !string.IsNullOrEmpty(objEntitySource.v_ODCCorrelativo))
                    //{
                    //    bool Flag = false;

                    //    ordendecompra ODC = (from n in dbContext.ordendecompra
                    //                         where n.v_SerieDocumento == objEntitySource.v_ODCSerie && n.v_CorrelativoDocumento == objEntitySource.v_ODCCorrelativo
                    //                         select n).FirstOrDefault();

                    //    if (ODC != null)
                    //    {
                    //        foreach (compradetalle DetalleCompra in objEntitySourceDetallesCompra)
                    //        {
                    //            ordendecompradetalle _ordendecompradetalle = ODC.ordendecompradetalle.Where(p => p.v_IdProductoDetalle == DetalleCompra.v_IdProductoDetalle).FirstOrDefault();

                    //            if (_ordendecompradetalle != null)
                    //            {
                    //                _ordendecompradetalle.i_UsadoEnCompra = 0;
                    //                dbContext.ordendecompradetalle.ApplyCurrentValues(_ordendecompradetalle);
                    //                Flag = true;
                    //            }
                    //        }

                    //        if (Flag)
                    //        {
                    //            ODC.i_IdEstado = (int)OrdenCompraEstados.Pendiente;
                    //        }

                    //        dbContext.ordendecompra.ApplyCurrentValues(ODC);
                    //    }
                    //}
                    #endregion

                    dbContext.SaveChanges();
                    Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "liquidacioncompra", pstrIdCompra);
                    pobjOperationResult.Success = 1;
                    ts.Complete();
                    return;
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LiquidacionComprasBL.EliminarCompra()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }

        public bool ComprobarSiFueLlamadoEnTesoreria(string IDCompra, out string NroTesoreria)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    NroTesoreria = string.Empty;
                    string IdProveedor;
                    int TipoDoc;
                    string NroDocumento;

                    var Datos = (from c in dbContext.liquidacioncompra
                                 where c.v_IdLiquidacionCompra == IDCompra
                                 select c).FirstOrDefault();

                    IdProveedor = Datos.v_IdProveedor;
                    TipoDoc = Datos.i_IdTipoDocumento.Value;
                    NroDocumento = Datos.v_SerieDocumento.Trim() + "-" + Datos.v_CorrelativoDocumento.Trim();

                    var Pendientecobrar = (from t in dbContext.tesoreriadetalle
                                           where t.i_IdTipoDocumento == TipoDoc && t.v_IdCliente == IdProveedor && t.v_NroDocumento == NroDocumento
                                           && t.v_Naturaleza == "D" && t.i_Eliminado == 0
                                           select t);

                    if (Pendientecobrar != null && Pendientecobrar.Count() != 0)
                    {
                        var Tesoreria = Pendientecobrar.FirstOrDefault().tesoreria;
                        int IdTipoDocTesoreria = Tesoreria.i_IdTipoDocumento.Value;
                        var Documento = dbContext.documento.Where(p => p.i_CodigoDocumento == IdTipoDocTesoreria).FirstOrDefault();
                        string SiglasBanco = Documento != null ? Documento.v_Siglas : string.Empty;
                        NroTesoreria = SiglasBanco.Trim() + "-" + Tesoreria.v_Mes.Trim() + "-" + Tesoreria.v_Correlativo.Trim();
                        return true;
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

        public bool ExisteNroRegistro(string Periodo, string Mes, string Correlativo)
        {
            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
            var Registro = (from n in dbContext.liquidacioncompra
                            where n.i_Eliminado == 0 && n.v_Periodo == Periodo && n.v_Mes == Mes && n.v_Correlativo == Correlativo
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

        public void InsertaPagosPendiente(ref OperationResult pobjOperationResult, string IdCompra, decimal TotalVenta, List<string> ClientSession)
        {
            int SecuentialId = 0;
            string newId = string.Empty;
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                    SecuentialBL objSecuentialBL = new SecuentialBL();
                   
                    pagopendienteDto objPagoPendienteEntity = new pagopendienteDto();
                    pagopendiente objEntity = pagopendienteAssembler.ToEntity(objPagoPendienteEntity);
                    liquidacioncompra objEntidadCompra = dbContext.liquidacioncompra.Where(x => x.v_IdLiquidacionCompra == IdCompra).FirstOrDefault();
                    // venta objEntidadVenta = dbContext.venta.Where(p => p.v_IdVenta == IdVenta).FirstOrDefault();
                    decimal SaldoProcesadoDocumentoRefrerencia = 0;

                    #region Procesa casos en que la venta sea nota de crédito
                    //if (objEntidadCompra.i_IdTipoDocumento == 7)
                    //{
                    //    string IdCompraReferencia = DevolverIdCompraPagosPendientes(ref pobjOperationResult, objEntidadCompra.i_IdTipoDocumentoRef.Value, objEntidadCompra.v_SerieDocumentoRef, objEntidadCompra.v_CorrelativoDocumentoRef);

                    //    if (!string.IsNullOrEmpty(IdCompraReferencia))
                    //    {
                    //        decimal TCambio = objEntidadCompra.d_TipoCambio.Value;
                    //        //cobranzapendiente _CobranzaPendienteEntity = (from n in dbContext.cobranzapendiente
                    //        //                                              where n.v_IdVenta == IdVentaReferencia && n.i_Eliminado == 0
                    //        //                                              select n).FirstOrDefault();

                    //        pagopendiente _PagoPendienteEntity = (from n in dbContext.pagopendiente
                    //                                              where n.v_IdCompra == IdCompraReferencia && n.i_Eliminado == 0
                    //                                              select n).FirstOrDefault();


                    //        if (_PagoPendienteEntity != null)
                    //        {
                    //            if (_PagoPendienteEntity.d_Saldo >= objEntidadCompra.d_Total.Value)
                    //            {
                    //                int Moneda = (from m in dbContext.compra
                    //                              where m.v_IdCompra == IdCompraReferencia && m.i_Eliminado == 0
                    //                              select new { m.i_IdMoneda }).FirstOrDefault().i_IdMoneda.Value;

                    //                switch (objEntidadCompra.i_IdMoneda)
                    //                {
                    //                    case 1:
                    //                        switch (Moneda)
                    //                        {
                    //                            case 1:
                    //                                //_CobranzaPendienteEntity.d_Acuenta = _CobranzaPendienteEntity.d_Acuenta.Value + objEntidadVenta.d_Total.Value;
                    //                                // _CobranzaPendienteEntity.d_Saldo = _CobranzaPendienteEntity.d_Saldo.Value - objEntidadVenta.d_Total.Value >= 0 ? _CobranzaPendienteEntity.d_Saldo.Value - objEntidadVenta.d_Total.Value : 0;

                    //                                _PagoPendienteEntity.d_Acuenta = _PagoPendienteEntity.d_Acuenta.Value + objEntidadCompra.d_Total.Value;
                    //                                _PagoPendienteEntity.d_Saldo = _PagoPendienteEntity.d_Saldo.Value - objEntidadCompra.d_Total.Value >= 0 ? _PagoPendienteEntity.d_Saldo.Value - objEntidadCompra.d_Total.Value : 0;
                    //                                break;

                    //                            case 2:
                    //                                decimal Monto;
                    //                                Monto = decimal.Parse(Math.Round(_PagoPendienteEntity.d_Saldo.Value - (objEntidadCompra.d_Total.Value / TCambio), 1, MidpointRounding.AwayFromZero).ToString("0.00"));
                    //                                _PagoPendienteEntity.d_Acuenta = _PagoPendienteEntity.d_Acuenta.Value + (objEntidadCompra.d_Total.Value / TCambio);
                    //                                _PagoPendienteEntity.d_Saldo = Monto >= 0 ? Monto : 0;
                    //                                break;
                    //                        }
                    //                        break;

                    //                    case 2:
                    //                        switch (Moneda)
                    //                        {
                    //                            case 1:
                    //                                decimal Monto;
                    //                                Monto = decimal.Parse(Math.Round(_PagoPendienteEntity.d_Saldo.Value - (objEntidadCompra.d_Total.Value * TCambio), 1, MidpointRounding.AwayFromZero).ToString("0.00"));
                    //                                _PagoPendienteEntity.d_Acuenta = _PagoPendienteEntity.d_Acuenta.Value + (objEntidadCompra.d_Total.Value * TCambio);
                    //                                _PagoPendienteEntity.d_Saldo = Monto >= 0 ? Monto : 0;
                    //                                break;

                    //                            case 2:
                    //                                _PagoPendienteEntity.d_Acuenta = _PagoPendienteEntity.d_Acuenta.Value + objEntidadCompra.d_Total.Value;
                    //                                _PagoPendienteEntity.d_Saldo = _PagoPendienteEntity.d_Saldo.Value - objEntidadCompra.d_Total.Value >= 0 ? _PagoPendienteEntity.d_Saldo.Value - objEntidadCompra.d_Total : 0;
                    //                                break;
                    //                        }
                    //                        break;
                    //                }

                    //                _PagoPendienteEntity.t_ActualizaFecha = DateTime.Now;
                    //                _PagoPendienteEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                    //                dbContext.pagopendiente.ApplyCurrentValues(_PagoPendienteEntity);
                    //                SaldoProcesadoDocumentoRefrerencia = objEntidadCompra.d_Total.Value;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            pobjOperationResult.Success = 0;
                    //            pobjOperationResult.ErrorMessage = "No se encontró la Cobranza Pendiente";
                    //            pobjOperationResult.AdditionalInformation = "CobranzaBL.InsertarCobranza()";
                    //            return;
                    //        }
                    //    }

                    //}
                    #endregion

                    objEntity.v_IdLiqCompra = IdCompra;
                    objEntity.d_Acuenta = 0 + SaldoProcesadoDocumentoRefrerencia;
                    objEntity.d_Saldo = decimal.Parse(Math.Round(TotalVenta, 2, MidpointRounding.AwayFromZero).ToString("0.00")) - SaldoProcesadoDocumentoRefrerencia;
                    objEntity.t_InsertaFecha = DateTime.Now;
                    objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntity.i_Eliminado = 0;
                    // Autogeneramos el Pk de la tabla
                    int intNodeId = int.Parse(ClientSession[0]);
                    SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 80);
                    newId = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "PC");
                    objEntity.v_IdPagoPendiente = newId;
                    #region Actualiza Linea de Crédito del Cliente
                    /*  
                    var Cliente = (from n in dbContext.venta
                                   join c in dbContext.cliente on n.v_IdCliente equals c.v_IdCliente into c_join
                                   from c in c_join.DefaultIfEmpty()
                                   where n.v_IdVenta == IdVenta
                                   select c).FirstOrDefault();


                    var Venta = dbContext.venta.Where(p => p.v_IdVenta == IdVenta).FirstOrDefault();

                    if (Cliente != null)
                    {
                        if (Cliente.i_UsaLineaCredito != null && Cliente.i_UsaLineaCredito == 1)
                        {
                            var LineaCredito = dbContext.lineacreditoempresa.Where(p => p.v_IdCliente == Cliente.v_IdCliente).FirstOrDefault();

                            if (LineaCredito != null)
                            {
                                switch (LineaCredito.i_IdMoneda)
                                {
                                    case 1:
                                        switch (Venta.i_IdMoneda)
                                        {
                                            case 1:
                                                LineaCredito.d_Acuenta = LineaCredito.d_Acuenta.Value + TotalVenta;
                                                LineaCredito.d_Saldo = LineaCredito.d_Credito.Value - LineaCredito.d_Acuenta.Value;
                                                break;

                                            case 2:
                                                LineaCredito.d_Acuenta = LineaCredito.d_Acuenta.Value + (TotalVenta * Venta.d_TipoCambio.Value);
                                                LineaCredito.d_Saldo = LineaCredito.d_Credito.Value - LineaCredito.d_Acuenta.Value;
                                                break;
                                        }
                                        break;

                                    case 2:
                                        switch (Venta.i_IdMoneda)
                                        {
                                            case 1:
                                                LineaCredito.d_Acuenta = LineaCredito.d_Acuenta.Value + (TotalVenta / Venta.d_TipoCambio.Value);
                                                LineaCredito.d_Saldo = LineaCredito.d_Credito.Value - LineaCredito.d_Acuenta.Value;
                                                break;

                                            case 2:
                                                LineaCredito.d_Acuenta = LineaCredito.d_Acuenta.Value + TotalVenta;
                                                LineaCredito.d_Saldo = LineaCredito.d_Credito.Value - LineaCredito.d_Acuenta.Value;
                                                break;
                                        }
                                        break;
                                }

                                dbContext.lineacreditoempresa.ApplyCurrentValues(LineaCredito);
                            }
                        }
                    }
                   
                    */
                    #endregion
                    dbContext.AddTopagopendiente(objEntity);
                    // dbContext.AddTocobranzapendiente(objEntity);
                    dbContext.SaveChanges();
                    Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "pagopendiente", newId);
                    pobjOperationResult.Success = 1;
                    ts.Complete();
                    return;
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LiquidacionCompraBL.InsertaPagosPendiente()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }

        public void EliminarPagoPendiente(ref OperationResult pobjOperationResult, string pstrIdCompra, List<string> ClientSession)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                    liquidacioncompra objEntidadCompra = dbContext.liquidacioncompra.Where(p => p.v_IdLiquidacionCompra == pstrIdCompra).FirstOrDefault();

                    pagopendiente _pagopendienteEntity = (from _PagoPendienteEntity in dbContext.pagopendiente
                                                          where _PagoPendienteEntity.v_IdLiqCompra == pstrIdCompra && _PagoPendienteEntity.i_Eliminado == 0
                                                          select _PagoPendienteEntity).FirstOrDefault();

                    if (_pagopendienteEntity != null)
                    {
                        _pagopendienteEntity.t_ActualizaFecha = DateTime.Now;
                        _pagopendienteEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        _pagopendienteEntity.i_Eliminado = 1;
                        dbContext.pagopendiente.ApplyCurrentValues(_pagopendienteEntity);
                    }

                    #region Procesa casos en que la compra sea nota de crédito
                    //if (objEntidadCompra.i_IdTipoDocumento == 7)
                    //{
                    //    string IdCompraReferencia = DevolverIdCompra(ref pobjOperationResult, objEntidadCompra.v_IdProveedor, objEntidadCompra.i_IdTipoDocumentoRef.Value, objEntidadCompra.v_SerieDocumentoRef, objEntidadCompra.v_CorrelativoDocumentoRef);

                    //    if (!string.IsNullOrEmpty(IdCompraReferencia))
                    //    {
                    //        decimal TCambio = objEntidadCompra.d_TipoCambio.Value;
                    //        pagopendiente _PagoPendienteEntity = (from n in dbContext.pagopendiente
                    //                                              where n.v_IdCompra == IdCompraReferencia && n.i_Eliminado == 0
                    //                                              select n).FirstOrDefault();

                    //        if (_PagoPendienteEntity != null)
                    //        {
                    //            int Moneda = (from m in dbContext.compra
                    //                          where m.v_IdCompra == IdCompraReferencia && m.i_Eliminado == 0
                    //                          select new { m.i_IdMoneda }).FirstOrDefault().i_IdMoneda.Value;


                    //            switch (objEntidadCompra.i_IdMoneda)
                    //            {
                    //                case 1:
                    //                    switch (Moneda)
                    //                    {
                    //                        case 1:
                    //                            _PagoPendienteEntity.d_Acuenta = _PagoPendienteEntity.d_Acuenta.Value - objEntidadCompra.d_Total.Value;
                    //                            _PagoPendienteEntity.d_Saldo = _PagoPendienteEntity.d_Saldo.Value + objEntidadCompra.d_Total.Value >= 0 ? _PagoPendienteEntity.d_Saldo.Value + objEntidadCompra.d_Total.Value : 0;

                    //                            break;

                    //                        case 2:
                    //                            _PagoPendienteEntity.d_Acuenta = _PagoPendienteEntity.d_Acuenta.Value - (objEntidadCompra.d_Total.Value / TCambio);
                    //                            _PagoPendienteEntity.d_Saldo = _PagoPendienteEntity.d_Saldo.Value + (objEntidadCompra.d_Total.Value / TCambio) >= 0 ? _PagoPendienteEntity.d_Saldo.Value + (objEntidadCompra.d_Total.Value / TCambio) : 0;

                    //                            break;
                    //                    }
                    //                    break;

                    //                case 2:
                    //                    switch (Moneda)
                    //                    {
                    //                        case 1:
                    //                            _PagoPendienteEntity.d_Acuenta = _PagoPendienteEntity.d_Acuenta.Value - (objEntidadCompra.d_Total.Value * TCambio);
                    //                            _PagoPendienteEntity.d_Saldo = _PagoPendienteEntity.d_Saldo.Value + (objEntidadCompra.d_Total.Value * TCambio) >= 0 ? _PagoPendienteEntity.d_Saldo.Value + (objEntidadCompra.d_Total.Value * TCambio) : 0;

                    //                            break;

                    //                        case 2:
                    //                            _PagoPendienteEntity.d_Acuenta = _PagoPendienteEntity.d_Acuenta.Value - objEntidadCompra.d_Total.Value;
                    //                            _PagoPendienteEntity.d_Saldo = _PagoPendienteEntity.d_Saldo.Value + objEntidadCompra.d_Total.Value >= 0 ? _PagoPendienteEntity.d_Saldo.Value + objEntidadCompra.d_Total.Value : 0;

                    //                            break;
                    //                    }
                    //                    break;
                    //            }

                    //            _PagoPendienteEntity.t_ActualizaFecha = DateTime.Now;
                    //            _PagoPendienteEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                    //            dbContext.pagopendiente.ApplyCurrentValues(_PagoPendienteEntity);
                    //        }
                    //        else
                    //        {
                    //            pobjOperationResult.Success = 0;
                    //            pobjOperationResult.ErrorMessage = "No se encontró Pago Pendiente";
                    //            pobjOperationResult.AdditionalInformation = "ComprasBL.EliminarPagoPendiente()";
                    //            return;
                    //        }
                    //    }

                    //}
                    #endregion

                    #region Actualiza Linea de Crédito del Cliente
                    /* var Cliente = (from n in dbContext.venta
                                   join c in dbContext.cliente on n.v_IdCliente equals c.v_IdCliente into c_join
                                   from c in c_join.DefaultIfEmpty()
                                   where n.v_IdVenta == pstrIdCompra
                                   select c).FirstOrDefault();

                    var Venta = dbContext.venta.Where(p => p.v_IdVenta == pstrIdCompra).FirstOrDefault();

                    if (Cliente != null && Venta != null)
                    {
                        if (Cliente.i_UsaLineaCredito != null && Cliente.i_UsaLineaCredito == 1)
                        {
                            var LineaCredito = dbContext.lineacreditoempresa.Where(p => p.v_IdCliente == Cliente.v_IdCliente).FirstOrDefault();

                            if (LineaCredito != null)
                            {
                                switch (LineaCredito.i_IdMoneda)
                                {
                                    case 1:
                                        switch (Venta.i_IdMoneda)
                                        {
                                            case 1:
                                                LineaCredito.d_Acuenta = LineaCredito.d_Acuenta.Value - Venta.d_Total.Value;
                                                LineaCredito.d_Saldo = LineaCredito.d_Credito.Value - LineaCredito.d_Acuenta.Value;
                                                break;

                                            case 2:
                                                LineaCredito.d_Acuenta = LineaCredito.d_Acuenta.Value - (Venta.d_Total.Value * Venta.d_TipoCambio.Value);
                                                LineaCredito.d_Saldo = LineaCredito.d_Credito.Value - LineaCredito.d_Acuenta.Value;
                                                break;
                                        }
                                        break;

                                    case 2:
                                        switch (Venta.i_IdMoneda)
                                        {
                                            case 1:
                                                LineaCredito.d_Acuenta = LineaCredito.d_Acuenta.Value - (Venta.d_Total.Value / Venta.d_TipoCambio.Value);
                                                LineaCredito.d_Saldo = LineaCredito.d_Credito.Value - LineaCredito.d_Acuenta.Value;
                                                break;

                                            case 2:
                                                LineaCredito.d_Acuenta = LineaCredito.d_Acuenta.Value - Venta.d_Total.Value;
                                                LineaCredito.d_Saldo = LineaCredito.d_Credito.Value - LineaCredito.d_Acuenta.Value;
                                                break;
                                        }
                                        break;
                                }

                                dbContext.lineacreditoempresa.ApplyCurrentValues(LineaCredito);
                            }
                        }
                    }
                    * */
                    #endregion

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                    ts.Complete();
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LiquidacionCompraBL.EliminarCobranzaPendiente()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }

        public string[] DevolverProveedorPorNroDocumento(ref OperationResult pobjOperationResult, string pstrNroDocumento)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = (from n in dbContext.cliente
                             where n.i_Eliminado == 0 && n.v_FlagPantalla == "V" && n.v_NroDocIdentificacion == pstrNroDocumento && n.i_IdTipoIdentificacion == 1
                             select n
                             ).FirstOrDefault();

                pobjOperationResult.Success = 1;

                if (query != null)
                {
                    string[] Cadena = new string[3];
                    Cadena[0] = query.v_IdCliente;
                    Cadena[1] = query.v_CodCliente;
                    Cadena[2] = (query.v_ApePaterno + " " + query.v_ApeMaterno + " " + query.v_PrimerNombre + " " + query.v_RazonSocial).Trim();
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
                pobjOperationResult.AdditionalInformation = "LiquidacionComprasBL.DevolverProveedorPorNroDocumento()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public string DevolverTipoCambioPorFecha(ref OperationResult pobjOperationResult, DateTime Fecha)
        {
            try
            {
                var query = new TipoCambioBL().DevolverTipoCambioPorFechaVenta(ref pobjOperationResult, Fecha);
                if (pobjOperationResult.Success == 0) return "0";
                return query;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LiquidacionComprasBL.DevolverTipoCambioPorFecha()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public bool ComprobarRelacionDocumentoProveedor(ref OperationResult pobjOperationResult, string pstrIdProveedor, int pintIdTipoDocumento, string pstrSerieDoc, string pstrCorrelativoDoc, string pstrIdCompra)
        {
            try
            {
                if (pintIdTipoDocumento != -1  && pstrSerieDoc != null && pstrCorrelativoDoc != null)  // && pstrIdProveedor != null// comprueba que tenga los datos necesarios para no consultar a la Bd en vano.
                {
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                    if (pstrIdCompra == null) // SI el idCompra es nulo se esta consultado de una compra nueva que no ha sido guardada
                    {
                        var query = (from n in dbContext.liquidacioncompra
                                     where n.i_Eliminado == 0 //&& n.v_IdProveedor == pstrIdProveedor
                                     && n.i_IdTipoDocumento == pintIdTipoDocumento
                                     && n.v_SerieDocumento == pstrSerieDoc && n.v_CorrelativoDocumento == pstrCorrelativoDoc
                                     select n
                                 ).FirstOrDefault();
                        if (query == null)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else // si no es nulo se comprueba de una compra que esta siendo modificada
                    {
                        var query = (from n in dbContext.liquidacioncompra
                                     where n.i_Eliminado == 0 //&& n.v_IdProveedor == pstrIdProveedor
                                     && n.i_IdTipoDocumento == pintIdTipoDocumento
                                     && n.v_SerieDocumento == pstrSerieDoc && n.v_CorrelativoDocumento == pstrCorrelativoDoc
                                     && n.v_IdLiquidacionCompra != pstrIdCompra
                                     select n
                                     ).FirstOrDefault();
                        if (query == null)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LiquidacionComprasBL.ComprobarRelacionDocumentoProveedor()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return false;
            }
        }

        public void RegenerarDiariosLiquidacionCompras(ref OperationResult objOperationResult,string Periodo ,DateTime FechaHasta,List<string> ClientSession)
        {
            ClienteBL _objClienteBL = new ClienteBL();
            using (TransactionScope  ts = TransactionUtils.CreateTransactionScope())
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    List <liquidacioncompra> LiquidacionCompra= (from a in dbContext.liquidacioncompra

                                             where a.i_Eliminado.Value == 0 && a.v_Periodo == Periodo && a.t_FechaRegistro<= FechaHasta

                                             select a).ToList();


                    List<liquidacioncompraDto> LiquidacioncompraDto = liquidacioncompraAssembler.ToDTOs (LiquidacionCompra);

                    foreach (var pobjDtoEntity in LiquidacioncompraDto)
                    {

                        OperationResult pobjOperationResult = new OperationResult();

                        #region Genera Pago Pendiente
                        if (pobjDtoEntity.v_IdLiquidacionCompra != null)
                        {
                            EliminarPagoPendiente(ref pobjOperationResult, pobjDtoEntity.v_IdLiquidacionCompra, Globals.ClientSession.GetAsList());
                            if (pobjDtoEntity.i_IdEstado == 1) InsertaPagosPendiente(ref pobjOperationResult, pobjDtoEntity.v_IdLiquidacionCompra, pobjDtoEntity.d_Total.Value, Globals.ClientSession.GetAsList());
                        }
                        if (pobjOperationResult.Success == 0) return;
                        #endregion

                        #region Regenera Libro Diario
                        string IDConcepto = pobjDtoEntity.i_IdMoneda.Value.ToString("00");

                        var aa = (from a in dbContext.administracionconceptos
                                  where a.v_Codigo == IDConcepto && a.i_Eliminado == 0 && a.v_Periodo.Equals(periodo)
                                  select new { a.v_CuentaIGV, a.v_CuentaPVenta }).FirstOrDefault();

                        if (aa != null && aa.v_CuentaIGV.Trim() != string.Empty && aa.v_CuentaPVenta.Trim() != string.Empty)
                        {
                            DiarioBL _objDiarioBL = new DiarioBL();
                            diarioDto _diarioDto = new diarioDto();
                            List<KeyValueDTO> _ListadoDiarios = new List<KeyValueDTO>();
                            List<diariodetalleDto> TempXInsertar = new List<diariodetalleDto>();

                            var DetalleCompra = (from d in dbContext.liquidacioncompradetalle
                                                 where d.v_IdLiquidacionCompra == pobjDtoEntity.v_IdLiquidacionCompra && d.i_Eliminado == 0
                                                 select d).ToList();

                            diariodetalleDto H_SubTotalVenta = new diariodetalleDto();
                            diariodetalleDto H_IGV = new diariodetalleDto();
                            diariodetalleDto D_PrecioVenta = new diariodetalleDto();

                            string[] IdDiarioEliminado = new string[3];

                            try
                            {
                                IdDiarioEliminado = _objDiarioBL.EliminarDiarioXDocRef(ref pobjOperationResult, pobjDtoEntity.v_IdLiquidacionCompra, ClientSession, false);
                                if (pobjOperationResult.Success == 0) return;
                            }
                            finally
                            {

                                if (pobjDtoEntity.i_IdEstado == 1)
                                {

                                    //if (IdDiarioEliminado[0] == null)
                                    if (IdDiarioEliminado[0] == null || (IdDiarioEliminado != null && (IdDiarioEliminado[1].Trim() != pobjDtoEntity.t_FechaRegistro.Value.Month.ToString("00") || IdDiarioEliminado[0] != pobjDtoEntity.t_FechaRegistro.Value.Year.ToString())))

                                    {
                                        int _MaxMovimiento;
                                        _ListadoDiarios = _objDiarioBL.ObtenerListadoDiario(ref pobjOperationResult, pobjDtoEntity.t_FechaRegistro.Value.Year.ToString(), pobjDtoEntity.t_FechaRegistro.Value.Month.ToString("00"), 341);
                                        _MaxMovimiento = _ListadoDiarios.Count() > 0 ? int.Parse(_ListadoDiarios[_ListadoDiarios.Count() - 1].Value1.ToString()) : 0;
                                        _MaxMovimiento++;
                                        _diarioDto.v_Periodo = pobjDtoEntity.t_FechaRegistro.Value.Year.ToString();
                                        _diarioDto.v_Mes = int.Parse(pobjDtoEntity.t_FechaRegistro.Value.Month.ToString()).ToString("00");
                                        _diarioDto.v_Correlativo = _MaxMovimiento.ToString("00000000");
                                    }
                                    else
                                    {
                                        _diarioDto.v_Periodo = IdDiarioEliminado[0];
                                        _diarioDto.v_Mes = IdDiarioEliminado[1];
                                        _diarioDto.v_Correlativo = IdDiarioEliminado[2];
                                    }

                                    _diarioDto.v_IdDocumentoReferencia = pobjDtoEntity.v_IdLiquidacionCompra;

                                    var cliente = _objClienteBL.ObtenerClientePorID(ref  objOperationResult , pobjDtoEntity.v_IdProveedor);
                                    _diarioDto.v_Nombre = cliente == null ? "" : (cliente.v_ApePaterno + " " + cliente.v_ApeMaterno.Trim () + " " + cliente.v_PrimerNombre.Trim() + cliente.v_RazonSocial.Trim ()).Trim();
                                    _diarioDto.v_Glosa = pobjDtoEntity.v_Glosa;
                                    _diarioDto.d_TipoCambio = pobjDtoEntity.d_TipoCambio.Value;
                                    _diarioDto.i_IdMoneda = pobjDtoEntity.i_IdMoneda.Value;
                                    _diarioDto.i_IdTipoDocumento = 341;
                                    _diarioDto.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                    _diarioDto.i_IdTipoComprobante = 2;

                                    List<String> CuentasDetalle = new List<string>();

                                    CuentasDetalle = DetalleCompra.Select(p => p.v_NroCuenta).Distinct().ToList();

                                    foreach (String CuentaDetalle in CuentasDetalle)
                                    {
                                        decimal SubTotal = DetalleCompra.Where(p => p.v_NroCuenta == CuentaDetalle).Sum(o => o.d_ValorVenta.Value);
                                        H_SubTotalVenta.d_Importe = SubTotal > 0 ? SubTotal : SubTotal * -1;
                                        H_SubTotalVenta.d_Cambio = Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.i_IdMoneda.Value == 1 ? SubTotal / pobjDtoEntity.d_TipoCambio.Value : SubTotal * pobjDtoEntity.d_TipoCambio.Value, 2);
                                        H_SubTotalVenta.i_IdCentroCostos = "";
                                        H_SubTotalVenta.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                                        H_SubTotalVenta.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                        H_SubTotalVenta.v_IdCliente = pobjDtoEntity.v_IdProveedor;
                                        H_SubTotalVenta.v_Naturaleza = SubTotal > 0 ? pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H" : pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D";
                                        H_SubTotalVenta.v_NroDocumento = pobjDtoEntity.v_SerieDocumento.Trim() + "-" + pobjDtoEntity.v_CorrelativoDocumento.Trim();
                                        H_SubTotalVenta.v_NroCuenta = CuentaDetalle;

                                        TempXInsertar.Add(H_SubTotalVenta);
                                        H_SubTotalVenta = new diariodetalleDto();
                                    }

                                    H_IGV.d_Importe = DetalleCompra.Sum(p => p.d_Igv.Value);
                                    H_IGV.d_Cambio = Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.i_IdMoneda.Value == 1 ? DetalleCompra.Sum(p => p.d_Igv.Value) / pobjDtoEntity.d_TipoCambio.Value : DetalleCompra.Sum(p => p.d_Igv.Value) * pobjDtoEntity.d_TipoCambio.Value, 2);
                                    H_IGV.i_IdCentroCostos = "";
                                    H_IGV.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                                    H_IGV.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                    H_IGV.v_IdCliente = pobjDtoEntity.v_IdProveedor;
                                    H_IGV.v_Naturaleza = pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H";
                                    H_IGV.v_NroDocumento = pobjDtoEntity.v_SerieDocumento.Trim() + "-" + pobjDtoEntity.v_CorrelativoDocumento.Trim();
                                    H_IGV.v_NroCuenta = (from a in dbContext.administracionconceptos
                                                         where a.v_Codigo == IDConcepto && a.i_Eliminado == 0 && a.v_Periodo.Equals(periodo)
                                                         select new { a.v_CuentaIGV }).First().v_CuentaIGV;
                                    TempXInsertar.Add(H_IGV);

                                    D_PrecioVenta.d_Importe = DetalleCompra.Sum(p => p.d_PrecioVenta);
                                    D_PrecioVenta.d_Cambio = Utils.Windows.DevuelveValorRedondeado(pobjDtoEntity.i_IdMoneda.Value == 1 ? DetalleCompra.Sum(p => p.d_PrecioVenta.Value) / pobjDtoEntity.d_TipoCambio.Value : DetalleCompra.Sum(p => p.d_PrecioVenta.Value) * pobjDtoEntity.d_TipoCambio.Value, 2);
                                    D_PrecioVenta.i_IdCentroCostos = "";
                                    D_PrecioVenta.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                                    D_PrecioVenta.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                    D_PrecioVenta.v_IdCliente = pobjDtoEntity.v_IdProveedor;
                                    D_PrecioVenta.v_Naturaleza = pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D";
                                    D_PrecioVenta.v_NroDocumento = pobjDtoEntity.v_SerieDocumento.Trim() + "-" + pobjDtoEntity.v_CorrelativoDocumento.Trim();
                                    D_PrecioVenta.v_NroCuenta = (from a in dbContext.administracionconceptos
                                                                 where a.v_Codigo == IDConcepto && a.i_Eliminado == 0 && a.v_Periodo.Equals(periodo)
                                                                 select new { a.v_CuentaPVenta }).First().v_CuentaPVenta;
                                    TempXInsertar.Add(D_PrecioVenta);

                                    decimal TotDebe, TotHaber, TotDebeC, TotHaberC;
                                    TotDebe = TempXInsertar.Where(p => p.v_Naturaleza == "D" && p.v_NroCuenta != String.Empty).Sum(o => o.d_Importe.Value);
                                    TotHaber = TempXInsertar.Where(p => p.v_Naturaleza == "H" && p.v_NroCuenta != String.Empty).Sum(o => o.d_Importe.Value);
                                    TotDebeC = TempXInsertar.Where(p => p.v_Naturaleza == "D" && p.v_NroCuenta != String.Empty).Sum(o => o.d_Cambio.Value);
                                    TotHaberC = TempXInsertar.Where(p => p.v_Naturaleza == "H" && p.v_NroCuenta != String.Empty).Sum(o => o.d_Cambio.Value);
                                    _diarioDto.d_TotalDebe = TotDebe;
                                    _diarioDto.d_TotalHaber = TotHaber;
                                    _diarioDto.d_TotalDebeCambio = TotDebeC;
                                    _diarioDto.d_TotalHaberCambio = TotHaberC;
                                    _diarioDto.d_DiferenciaDebe = TotDebe - TotHaber;
                                    _diarioDto.d_DiferenciaHaber = TotDebeC - TotHaberC;

                                    _objDiarioBL.InsertarDiario(ref pobjOperationResult, _diarioDto, Globals.ClientSession.GetAsList(), TempXInsertar.Where(x => x.v_NroCuenta != string.Empty).ToList(), (int)TipoMovimientoTesoreria.Egreso);

                                    dbContext.SaveChanges();
                                }

                            }
                            if (pobjOperationResult.Success == 0) return;

                        }
                        #endregion

                        Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "liquidacioncompra", pobjDtoEntity.v_IdLiquidacionCompra);
                    

                    }

                    dbContext.SaveChanges();
                    objOperationResult.Success = 1;
                    ts.Complete();
                    return;
                
                
                
                }
            }
        
        }



        #region Bandeja
        public List<liquidacioncompraDto> ListarBusquedaCompras(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression, DateTime F_Ini, DateTime F_Fin)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = (from n in dbContext.liquidacioncompra

                             join A in dbContext.cliente on n.v_IdProveedor equals A.v_IdCliente into A_join
                             from A in A_join.DefaultIfEmpty()

                             join J2 in dbContext.systemuser on new { i_UpdateUserId = n.i_ActualizaIdUsuario.Value }
                                                            equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                             from J2 in J2_join.DefaultIfEmpty()

                             join J3 in dbContext.systemuser on new { i_InsertUserId = n.i_InsertaIdUsuario.Value }
                                                            equals new { i_InsertUserId = J3.i_SystemUserId } into J3_join
                             from J3 in J3_join.DefaultIfEmpty()

                             join J4 in dbContext.documento on new { i_IdTipoDocumento = n.i_IdTipoDocumento.Value }
                                                            equals new { i_IdTipoDocumento = J4.i_CodigoDocumento } into J4_join
                             from J4 in J4_join.DefaultIfEmpty()

                             where n.i_Eliminado == 0 && n.t_FechaRegistro >= F_Ini && n.t_FechaRegistro <= F_Fin && n.i_IdEstablecimiento == Globals.ClientSession.i_IdEstablecimiento.Value
                             select new liquidacioncompraDto
                             {
                                 v_IdLiquidacionCompra = n.v_IdLiquidacionCompra,
                                 v_Periodo = n.v_Periodo,
                                 v_Mes = n.v_Mes,
                                 v_Correlativo = n.v_Correlativo,
                                 NroRegistro = n.v_Mes.Trim() + "-" + n.v_Correlativo,
                                 v_SerieDocumento = n.v_SerieDocumento,
                                 v_CorrelativoDocumento = n.v_CorrelativoDocumento,
                                 Documento = n.v_SerieDocumento + "-" + n.v_CorrelativoDocumento,
                                 i_IdTipoDocumento = n.i_IdTipoDocumento,
                                 TipoDocumento = J4.v_Siglas,
                                 t_FechaRegistro = n.t_FechaRegistro,
                                 t_FechaEmision = n.t_FechaEmision,
                                 v_IdProveedor = n.v_IdProveedor,
                                 CodigoProveedor = A.v_CodCliente,
                                 NombreProveedor = (A.v_ApePaterno + " " + A.v_ApeMaterno + " " + A.v_PrimerNombre + " " + A.v_RazonSocial).Trim(),
                                 i_IdEstado = n.i_IdEstado,
                                 d_Total = n.d_Total,
                                 t_InsertaFecha = n.t_InsertaFecha,
                                 t_ActualizaFecha = n.t_ActualizaFecha,
                                 v_UsuarioCreacion = J3.v_UserName,
                                 v_UsuarioModificacion = J2.v_UserName,

                             }
                             );
                if (!string.IsNullOrEmpty(pstrFilterExpression))
                {
                    query = query.Where(pstrFilterExpression);
                }
                if (!string.IsNullOrEmpty(pstrSortExpression))
                {
                    query = query.OrderBy(pstrSortExpression);
                }
                List<liquidacioncompraDto> objData = query.ToList();
                pobjOperationResult.Success = 1;
                return objData;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }
        #endregion
    }
}
