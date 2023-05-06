using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.BL;
using System.Transactions;
using System.ComponentModel;
using SAMBHS.Almacen.BL;

namespace SAMBHS.Compra.BL
{
    public class GuiaRemisionBL
    {
        # region Formulario
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
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public List<KeyValueDTO> ObtenerListadoGuiaRemision(ref OperationResult pobjOperationResult, string pstringPeriodo, string pstringMes)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var almacenpredeterminado = Globals.ClientSession.i_IdAlmacenPredeterminado.Value.ToString("00");
                string replicationID = Globals.ClientSession.ReplicationNodeID;
                var query = (from n in dbContext.guiaremision
                             where n.i_Eliminado == 0 && n.v_Periodo == pstringPeriodo && n.v_Mes == pstringMes && n.v_IdGuiaRemision.Substring(2, 2) == almacenpredeterminado && n.v_IdGuiaRemision.Substring(0, 1) == replicationID

                             orderby n.v_Correlativo ascending
                             select new
                             {
                                 n.v_Correlativo,
                                 n.v_IdGuiaRemision,
                             }
                );
                if (query.Any())
                {
                    var query2 = query.AsEnumerable()
                        .Select(x => new KeyValueDTO
                        {
                            Value1 = x.v_Correlativo,
                            Value2 = x.v_IdGuiaRemision
                        }).ToList();

                    return query2;
                }
                return new List<KeyValueDTO> { new KeyValueDTO { Value1 = almacenpredeterminado + "000000" } };
            }
        }

        public bool ExisteNroRegistro(string Periodo, string Mes, string Correlativo)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                string replicationID = Globals.ClientSession.ReplicationNodeID;
                var Registro = (from n in dbContext.guiaremision
                                where n.i_Eliminado == 0 && n.v_Periodo == Periodo && n.v_Mes == Mes && n.v_Correlativo == Correlativo && n.v_IdGuiaRemision.Substring(0, 1) == replicationID

                                select n).FirstOrDefault();

                return Registro == null;
            }
        }

        public guiaremisionDto ObtenerGuiaRemisionCabecera(ref OperationResult pobjOperationResult, string pstrIdGuiaRemision)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var objEntity = (from a in dbContext.guiaremision

                                     join A in dbContext.transportista on a.v_IdTransportista equals A.v_IdTransportista into A_join
                                     from A in A_join.DefaultIfEmpty()

                                     join B in dbContext.transportistaunidadtransporte on a.v_IdUnidadTransporte equals B.v_IdUnidadTransporte into B_join
                                     from B in B_join.DefaultIfEmpty()

                                     join C in dbContext.transportistachofer on a.v_IdChofer equals C.v_IdChofer into C_join
                                     from C in C_join.DefaultIfEmpty()

                                     join D in dbContext.datahierarchy on new { a = a.i_IdEstado.Value, b = 30 } //Estado
                                     equals new { a = D.i_ItemId, b = D.i_GroupId } into D_join
                                     from D in D_join.DefaultIfEmpty()

                                     join E in dbContext.agenciatransporte on a.v_IdAgenciaTransporte equals E.v_IdAgenciaTransporte into E_join
                                     from E in E_join.DefaultIfEmpty()

                                     join F in dbContext.cliente on a.v_IdCliente equals F.v_IdCliente into F_join
                                     from F in F_join.DefaultIfEmpty()


                                     where a.v_IdGuiaRemision == pstrIdGuiaRemision

                                     select new guiaremisionDto
                                     {

                                         v_IdCliente = a.v_IdCliente,
                                         v_IdGuiaRemision = a.v_IdGuiaRemision,
                                         v_IdChofer = a.v_IdChofer,
                                         v_IdUnidadTransporte = a.v_IdUnidadTransporte,
                                         v_IdAgenciaTransporte = a.v_IdAgenciaTransporte,
                                         v_Periodo = a.v_Periodo,
                                         v_Mes = a.v_Mes,
                                         v_Correlativo = a.v_Correlativo,
                                         v_SerieGuiaRemision = a.v_SerieGuiaRemision,
                                         v_NumeroGuiaRemision = a.v_NumeroGuiaRemision,
                                         t_FechaEmision = a.t_FechaEmision,
                                         d_TipoCambio = a.d_TipoCambio,
                                         t_FechaTraslado = a.t_FechaTraslado,
                                         i_IdTipoDocumento = a.i_IdTipoDocumento,
                                         v_SerieDocumentoRef = a.v_SerieDocumentoRef,
                                         v_NumeroDocumentoRef = a.v_NumeroDocumentoRef,
                                         v_NumeroPedidoCotizacion = a.v_NumeroPedidoCotizacion,
                                         i_IdEstado = a.i_IdEstado,
                                         v_PuntoPartida = a.v_PuntoPartida,
                                         v_PuntoLLegada = a.v_PuntoLLegada,
                                         i_IdMotivoTraslado = a.i_IdMotivoTraslado,
                                         i_IdMoneda = a.i_IdMoneda,
                                         v_IdTransportista = A.v_IdTransportista,
                                         CodTransportista = A.v_Codigo,
                                         NombreTransportista = A.v_NombreRazonSocial,
                                         RucTransportista = A.v_NumeroDocumento,
                                         //CODIGOTRACTO NombreChofer HAY EN TRANSPORTEUNIDADTRANSPORTE
                                         MarcaTracto = B.v_TractoMarca,
                                         PlacaTracto = B.v_TractoPlaca,
                                         ConstInscripcion = B.v_TractoCertificado,
                                         NombreChofer = C.v_NombreCompleto,
                                         NumLicencia = C.v_Brevete,
                                         NombreAgencia = E.v_NombreRazonSocial,
                                         RucAgencia = E.v_NumeroDocumento,
                                         DireccionAgencia = E.v_Direccion,
                                         NombreCliente = (F.v_ApePaterno + " " + F.v_ApeMaterno + " " + F.v_PrimerNombre + " " + F.v_SegundoNombre + " " + F.v_RazonSocial).Trim(),
                                         CodigoCliente = F.v_CodCliente.Trim(),
                                         NroDocCliente = F.v_NroDocIdentificacion,
                                         d_Redondeo = a.d_Redondeo,
                                         i_Eliminado = a.i_Eliminado,
                                         i_InsertaIdUsuario = a.i_InsertaIdUsuario,
                                         t_InsertaFecha = a.t_InsertaFecha,
                                         i_ActualizaIdUsuario = a.i_ActualizaIdUsuario,
                                         t_ActualizaFecha = a.t_ActualizaFecha,
                                         i_IdEstablecimiento = a.i_IdEstablecimiento,
                                         i_IdTipoGuia = a.i_IdTipoGuia ?? -1,
                                         i_IdIgv = a.i_IdIgv ?? -1,
                                         i_AfectoIgv = a.i_AfectoIgv ?? 0,
                                         i_PrecionInclIgv = a.i_PrecionInclIgv ?? 0,
                                         d_Total = a.d_Total ?? 0,
                                         i_IdAlmacenDestino = a.i_IdAlmacenDestino ?? -1,
                                         i_IdDireccionCliente = a.i_IdDireccionCliente ?? -1,
                                         v_UbigueoLlegada = a.v_UbigueoLlegada,
                                         v_UbigueoPartida = a.v_UbigueoPartida,
                                         d_TotalPeso = a.d_TotalPeso,
                                         i_Modalidad = a.i_Modalidad,
                                         i_EstadoSunat = a.i_EstadoSunat
                                     }
                                             ).FirstOrDefault();

                    pobjOperationResult.Success = 1;

                    return objEntity;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "GuiaRemisionBL.ObtenerGuiaRemisionCabecera()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public BindingList<guiaremisiondetalle> ObtenerGuiaRemisionDetalles(ref OperationResult pobjOperationResult, string pstrIdGuiaRemision)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = from n in dbContext.guiaremisiondetalle
                                where n.i_Eliminado == 0 && n.v_IdGuiaRemision == pstrIdGuiaRemision
                                orderby n.t_InsertaFecha ascending
                                select n;

                    pobjOperationResult.Success = 1;
                    return new BindingList<guiaremisiondetalle>(query.ToList());
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public List<GridKeyValueDTO> ObtenerAlmacenesGuiaRemision(ref OperationResult pobjOperationResult)
        {
            string pstrSortExpression = "v_Nombre";

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = from a in dbContext.almacen

                                select a;

                    query = query.OrderBy(!string.IsNullOrEmpty(pstrSortExpression) ? pstrSortExpression : "v_Nombre");

                    var query2 = query.AsEnumerable()
                                .Select(x => new GridKeyValueDTO
                                {
                                    //Id = x.i_ItemId.ToString(),
                                    Id = x.i_IdAlmacen.ToString(),
                                    Value1 = x.v_Nombre,
                                }).ToList();

                    pobjOperationResult.Success = 1;
                    return query2;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }


        public List<guiaremisiondetalleDto> ObtenerDetalleGuiaRemisionporDocumentoRef(int TipoDoc, string SerieDoc, string CorrelativoDoc)
        {

            var dbContext = new SAMBHSEntitiesModelWin();
            using (dbContext)
            {
                var DetallesGuia = (from a in dbContext.guiaremision

                                    join b in dbContext.guiaremisiondetalle on new { IdGuiaRemision = a.v_IdGuiaRemision, eliminado = 0 } equals new { IdGuiaRemision = b.v_IdGuiaRemision, eliminado = b.i_Eliminado.Value } into b_join

                                    from b in b_join.DefaultIfEmpty()

                                    where a.i_Eliminado == 0 && a.i_IdTipoDocumento == TipoDoc && a.v_SerieDocumentoRef.Trim() == SerieDoc && a.v_NumeroDocumentoRef.Trim() == CorrelativoDoc
                                    && a.i_IdEstado == 1
                                    select new guiaremisiondetalleDto
                                    {
                                        v_IdProductoDetalle = b.v_IdProductoDetalle,
                                        d_Cantidad = b.d_Cantidad,
                                        v_IdGuiaRemision = a.v_IdGuiaRemision,
                                        d_CantidadEmpaque = b.d_CantidadEmpaque,
                                        v_IdGuiaRemisionDetalle = b.v_IdGuiaRemisionDetalle,
                                    }).ToList();
                return DetallesGuia;
            }

        }

        public List<guiaremisiondetalleDto> ObtenerDetalleGuiaRemisionParaExtraccion(
            ref OperationResult OperationResult, string pstrIdGuiaRemision)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var query =
                    dbContext.guiaremisiondetalle.Where(
                        p => p.v_IdGuiaRemision == pstrIdGuiaRemision && p.i_Eliminado == 0).ToList();

                var productosDetalle = dbContext.productodetalle.Where(p => p.i_Eliminado == 0).ToList();

                var result = query.ToDTOs();

                result.ForEach(p =>
                {
                    var productoDetalle = productosDetalle.FirstOrDefault(x => x.v_IdProductoDetalle == p.v_IdProductoDetalle);
                    var linea = dbContext.linea.FirstOrDefault(x => x.v_IdLinea == productoDetalle.producto.v_IdLinea);
                    p.v_Descripcion = productoDetalle != null ? productoDetalle.producto.v_Descripcion : string.Empty;
                    p.v_CodInterno = productoDetalle != null ? productoDetalle.producto.v_CodInterno : string.Empty;
                    p.NroCuenta = linea != null ? linea.v_NroCuentaVenta : "-1";
                    p.i_IdUnidadEmpaque = productoDetalle != null ? productoDetalle.producto.i_IdUnidadMedida ?? 5 : 5;
                  
                });


                return result;
            }
        }

        public string[] DevolverProductos(string pstringIdProductoDetalle)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var entityProducto = (from n in dbContext.productodetalle

                                      join A in dbContext.producto on n.v_IdProducto equals A.v_IdProducto into A_join
                                      from A in A_join.DefaultIfEmpty()



                                      join J1 in dbContext.datahierarchy on new { a = A.i_IdUnidadMedida.Value, b = 17 }
                                      equals new { a = J1.i_ItemId, b = J1.i_GroupId } into J1_join
                                      from J1 in J1_join.DefaultIfEmpty()

                                      where n.v_IdProductoDetalle == pstringIdProductoDetalle

                                      select new
                                      {
                                          Nombre = A.v_Descripcion,
                                          CodigoInterno = A.v_CodInterno,
                                          Empaque = A.d_Empaque,
                                          //UMEmpaque = J1.i_ItemId,
                                          UMEmpaque = J1.v_Value1,
                                          //UnidadMedidasss=J1.i_ItemId,
                                          i_IdUnidadMedida = A.i_IdUnidadMedida

                                      }
                ).FirstOrDefault();


                string[] Cadena = new string[8];
                if (entityProducto != null)
                {
                    Cadena[0] = entityProducto.CodigoInterno;
                    Cadena[1] = entityProducto.Nombre;
                    Cadena[2] = entityProducto.Empaque.ToString();
                    Cadena[3] = entityProducto.UMEmpaque.ToString();
                    Cadena[4] = entityProducto.i_IdUnidadMedida.ToString();
                    //Cadena[3] = EntityProducto.UnidadMedidasss.ToString ();
                }

                return Cadena;
            }
        }

        public string[] DevolverProductosTemporal(string pstringIdProductoDetalle)
        {
            string success;
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {

                var EntityProducto = (from n in dbContext.productodetalle

                                      join A in dbContext.producto on n.v_IdProducto equals A.v_IdProducto into A_join
                                      from A in A_join.DefaultIfEmpty()



                                      join J1 in dbContext.datahierarchy on new { a = A.i_IdUnidadMedida.Value, b = 17 }
                                      equals new { a = J1.i_ItemId, b = J1.i_GroupId } into J1_join
                                      from J1 in J1_join.DefaultIfEmpty()

                                      where n.v_IdProductoDetalle == pstringIdProductoDetalle

                                      select new
                                      {
                                          Nombre = A.v_Descripcion,
                                          CodigoInterno = A.v_CodInterno,
                                          Empaque = A.d_Empaque,
                                          UMEmpaque = J1.i_ItemId,
                                          //UnidadMedidasss=J1.i_ItemId,
                                          i_idUnidaMedida = J1.i_ItemId,


                                      }
                ).FirstOrDefault();


                var cadena = new string[8];
                if (EntityProducto != null)
                {
                    cadena[0] = EntityProducto.CodigoInterno;
                    cadena[1] = EntityProducto.Nombre;
                    cadena[2] = EntityProducto.Empaque.ToString();
                    //Cadena[3] = EntityProducto.UMEmpaque.ToString();
                    cadena[3] = EntityProducto.i_idUnidaMedida.ToString();
                    //Cadena[3] = EntityProducto.UnidadMedidasss.ToString ();
                }

                return cadena;
            }
        }

        public int EsServicio(string IdProductoDetalle)
        {

            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var producto = (from a in dbContext.productodetalle
                                join b in dbContext.producto on new { p = a.v_IdProducto, eliminado = 0 } equals new { p = b.v_IdProducto, eliminado = b.i_Eliminado.Value } into b_join
                                from b in b_join.DefaultIfEmpty()
                                where a.i_Eliminado == 0 && a.v_IdProductoDetalle == IdProductoDetalle
                                select new
                                {
                                    i_EsServicio = b.i_EsServicio ?? 0,
                                }).FirstOrDefault();
                return producto != null ? producto.i_EsServicio : 0;
            }


        }
        public string InsertarGuiaRemision(ref OperationResult pobjOperationResult, guiaremisionDto pobjDtoEntity, List<string> ClientSession, List<guiaremisiondetalleDto> pTemp_Insertar)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        SecuentialBL objSecuentialBL = new SecuentialBL();
                        guiaremision objEntityGuiaRemision = guiaremisionAssembler.ToEntity(pobjDtoEntity);
                        guiaremisiondetalleDto pobjDtoGuiaRemisionDetalle = new guiaremisiondetalleDto();
                        movimientodetalleDto objMovimientoDetalleDto = new movimientodetalleDto();
                        DocumentoBL _objDocumentoBL = new DocumentoBL();
                        OperationResult objOperationResult = new OperationResult();
                        int SecuentialId = 0;
                        string newIdGuiaRemision = string.Empty;
                        string newIdGuiaRemisionDetalle = string.Empty;
                        int intNodeId;

                        #region Inserta Cabecera

                        objEntityGuiaRemision.t_InsertaFecha = DateTime.Now;
                        objEntityGuiaRemision.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntityGuiaRemision.i_Eliminado = 0;

                        // Autogeneramos el Pk de la tabla
                        intNodeId = int.Parse(ClientSession[0]);
                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 38);
                        newIdGuiaRemision = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "ZR");
                        objEntityGuiaRemision.v_IdGuiaRemision = newIdGuiaRemision;
                        dbContext.AddToguiaremision(objEntityGuiaRemision);

                        #endregion

                        #region Inserta Detalle

                        foreach (guiaremisiondetalleDto guiaremisiondetalleDto in pTemp_Insertar)
                        {
                            guiaremisiondetalle objEntityGuiaRemisionDetalle = guiaremisiondetalleAssembler.ToEntity(guiaremisiondetalleDto);
                            SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 39);
                            newIdGuiaRemisionDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "ZS");
                            objEntityGuiaRemisionDetalle.v_IdGuiaRemisionDetalle = newIdGuiaRemisionDetalle;
                            objEntityGuiaRemisionDetalle.v_IdGuiaRemision = newIdGuiaRemision;
                            objEntityGuiaRemisionDetalle.t_InsertaFecha = DateTime.Now;
                            objEntityGuiaRemisionDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                            objEntityGuiaRemisionDetalle.i_Eliminado = 0;
                            dbContext.AddToguiaremisiondetalle(objEntityGuiaRemisionDetalle);
                            Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "guiaremisiondetalle", newIdGuiaRemisionDetalle);
                            //#region ActualizaNotaSalida

                            //if (guiaremisiondetalleDto.v_IdMovimientoDetalle != null)
                            //{
                            //    //ActualizaNotaSalida- CantidadAdministrativa
                            //    var GuiasAnteriores = CalcularGuiasAnteriores(IdVenta, guiaremisiondetalleDto.v_IdMovimientoDetalle );
                            //    var nsd = (from a in dbContext.movimientodetalle
                            //               where a.i_Eliminado == 0 && a.v_IdMovimientoDetalle == guiaremisiondetalleDto.v_IdMovimientoDetalle
                            //               select a).FirstOrDefault();

                            //    if (nsd != null)
                            //    {
                            //        movimientodetalleDto objMD = movimientodetalleAssembler.ToDTO(nsd);
                            //        objMD.d_CantidadAdministrativa = guiaremisiondetalleDto.d_Cantidad + GuiasAnteriores.CantidadAdministrativa;
                            //        objMD.d_CantidadEmpaqueAdministrativa = guiaremisiondetalleDto.d_CantidadEmpaque + GuiasAnteriores.CantidadEmpaqueAdministrativa;
                            //        movimientodetalle _objMovimientoDetalle = movimientodetalleAssembler.ToEntity(objMD);
                            //        var query = (from n in dbContext.movimientodetalle
                            //                     where n.v_IdMovimientoDetalle == guiaremisiondetalleDto.v_IdMovimientoDetalle
                            //                     select n).FirstOrDefault();

                            //        _objMovimientoDetalle.t_ActualizaFecha = DateTime.Now;
                            //        _objMovimientoDetalle.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            //        dbContext.movimientodetalle.ApplyCurrentValues(_objMovimientoDetalle);
                            //        Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "movimientodetalle", query.v_IdMovimientoDetalle);
                            //    }
                            //}
                            //#endregion
                        }

                        #endregion

                        #region Actualiza Correlativo EmpresaDetalle
                        _objDocumentoBL.ActualizarCorrelativoPorSerie(ref objOperationResult, Globals.ClientSession.i_IdEstablecimiento, int.Parse(pobjDtoEntity.i_IdTipoGuia.Value.ToString()), pobjDtoEntity.v_SerieGuiaRemision, int.Parse(pobjDtoEntity.v_NumeroGuiaRemision) + 1);
                        #endregion

                        #region Actualiza Cabecera Venta y/o Pedido
                        var venta =
                            dbContext.venta.FirstOrDefault(
                                p =>
                                    p.i_IdTipoDocumento == pobjDtoEntity.i_IdTipoDocumento.Value &&
                                    p.v_SerieDocumento == pobjDtoEntity.v_SerieDocumentoRef.Trim() &&
                                    p.v_CorrelativoDocumento == pobjDtoEntity.v_NumeroDocumentoRef.Trim() && p.i_Eliminado == 0);

                        if (venta != null)
                        {
                            venta.v_NroGuiaRemisionSerie = pobjDtoEntity.v_SerieGuiaRemision;
                            venta.v_NroGuiaRemisionCorrelativo = pobjDtoEntity.v_NumeroGuiaRemision;
                            dbContext.venta.ApplyCurrentValues(venta);
                        }


                        
                        #endregion

                        #region GeneraNotaSalida

                        if (_objDocumentoBL.DocumentoGeneraStock(pobjDtoEntity.i_IdTipoGuia.Value))
                        {

                            if (pobjDtoEntity.i_IdEstado == 1)
                            {
                                #region Genera Nota de Salida

                                movimientoDto _movimientoDto = new movimientoDto();
                                movimientodetalleDto _movimientodetalleDto = new movimientodetalleDto();
                                MovimientoBL _objMovimientoBL = new MovimientoBL();
                                List<string> Almacenes = new List<string>();
                                List<movimientodetalleDto> _movimientodetalleDtos = new List<movimientodetalleDto>();

                                if ((pTemp_Insertar.Find(p => p.i_EsServicio == 0) != null) && pobjDtoEntity.i_IdEstado != 0)
                                {
                                    List<int?> pAlmacenes = pTemp_Insertar.Select(p => p.i_IdAlmacen).Distinct().ToList();

                                    List<KeyValueDTO> ListaMovimientos = new List<KeyValueDTO>();
                                    ListaMovimientos = _objMovimientoBL.ObtenerListadoMovimientos(ref pobjOperationResult,
                                        pobjDtoEntity.v_Periodo, pobjDtoEntity.v_Mes, (int)TipoDeMovimiento.NotadeSalida);

                                    int MaxMovimiento;
                                    MaxMovimiento = ListaMovimientos.Count() > 0
                                        ? int.Parse(ListaMovimientos[ListaMovimientos.Count() - 1].Value1.ToString())
                                        : 0;

                                    foreach (int? pAlmacen in pAlmacenes)
                                    {
                                        _movimientoDto = new movimientoDto();
                                        MaxMovimiento++;
                                        _movimientoDto.d_TipoCambio = pobjDtoEntity.d_TipoCambio;
                                        _movimientoDto.i_IdAlmacenOrigen = pAlmacen;
                                        _movimientoDto.i_IdMoneda = pobjDtoEntity.i_IdMoneda;
                                        _movimientoDto.i_IdTipoMotivo = 1; //Compra nacional
                                        _movimientoDto.t_Fecha = pobjDtoEntity.t_FechaEmision;
                                        _movimientoDto.v_Mes = pobjDtoEntity.v_Mes.Trim();
                                        _movimientoDto.v_Periodo = pobjDtoEntity.v_Periodo.Trim();
                                        _movimientoDto.i_IdTipoMovimiento = (int)TipoDeMovimiento.NotadeSalida;
                                        _movimientoDto.v_Correlativo = MaxMovimiento.ToString("00000000");
                                        _movimientoDto.v_IdCliente = pobjDtoEntity.v_IdCliente;
                                        // _movimientoDto.v_OrigenTipo = "G";
                                        _movimientoDto.v_OrigenTipo = Constants.OrigenGuiaInterna;
                                        _movimientoDto.i_EsDevolucion = _objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? 1 : 0;
                                        _movimientoDto.v_OrigenRegCorrelativo = pobjDtoEntity.v_Correlativo;
                                        _movimientoDto.v_OrigenRegMes = pobjDtoEntity.v_Mes;
                                        _movimientoDto.v_OrigenRegPeriodo = pobjDtoEntity.v_Periodo;

                                        //  _movimientoDto.d_TotalPrecio =pobjDtoEntity.d_Total;
                                        //  _movimientoDto.d_TotalPrecio = pTemp_Insertar.Sum(x => x.d_Cantidad * x.d_Precio).Value;
                                        _movimientoDto.i_IdEstablecimiento = pobjDtoEntity.i_IdEstablecimiento.Value;

                                        foreach (
                                            guiaremisiondetalleDto _guiaremisiondetalleDto in
                                                pTemp_Insertar.Where(
                                                    p =>
                                                        p.i_EsServicio == 0 && p.i_IdAlmacen == pAlmacen &&
                                                        p.v_IdProductoDetalle != null).ToList())
                                        {
                                            _movimientodetalleDto = new movimientodetalleDto();
                                            _movimientodetalleDto.v_IdProductoDetalle = _guiaremisiondetalleDto.v_IdProductoDetalle.Trim();
                                            _movimientodetalleDto.i_IdTipoDocumento = int.Parse(pobjDtoEntity.i_IdTipoDocumento.Value.ToString());
                                            _movimientodetalleDto.v_NumeroDocumento = pobjDtoEntity.v_SerieDocumentoRef.Trim() == string.Empty || pobjDtoEntity.v_NumeroDocumentoRef == string.Empty ? "" : pobjDtoEntity.v_SerieDocumentoRef + "-" +
                                                                                      pobjDtoEntity.v_NumeroDocumentoRef.Trim();
                                            _movimientodetalleDto.v_NroGuiaRemision = pobjDtoEntity.v_SerieGuiaRemision == string.Empty || pobjDtoEntity.v_NumeroGuiaRemision == string.Empty ? "" : pobjDtoEntity.v_SerieGuiaRemision.Trim() + "-" +
                                                                                     pobjDtoEntity.v_NumeroGuiaRemision.Trim();
                                            _movimientodetalleDto.d_Cantidad = _guiaremisiondetalleDto.d_Cantidad;
                                            _movimientodetalleDto.i_IdUnidad = _guiaremisiondetalleDto.i_IdUnidadMedida;
                                            _movimientodetalleDto.d_Precio = _guiaremisiondetalleDto.d_Precio;
                                            _movimientodetalleDto.d_Total = _guiaremisiondetalleDto.d_Cantidad * _guiaremisiondetalleDto.d_Precio;
                                            _movimientodetalleDto.d_CantidadEmpaque = _guiaremisiondetalleDto.d_CantidadEmpaque;
                                            _movimientodetalleDto.i_IdTipoDocumento = int.Parse(pobjDtoEntity.i_IdTipoGuia.Value.ToString());
                                            _movimientodetalleDto.v_NumeroDocumento = _movimientodetalleDto.v_NroGuiaRemision;

                                            _movimientodetalleDto.v_NroPedido = "";
                                            _movimientodetalleDtos.Add(_movimientodetalleDto);
                                        }

                                        if (_movimientodetalleDtos.Count() > 0)
                                        {
                                            _movimientoDto.d_TotalCantidad = _movimientodetalleDtos.Sum(p => p.d_Cantidad.Value);
                                            _movimientoDto.d_TotalPrecio = _movimientodetalleDtos.Sum(p => p.d_Total.Value);
                                            _objMovimientoBL.InsertarMovimiento(ref pobjOperationResult, _movimientoDto,
                                                Globals.ClientSession.GetAsList(), _movimientodetalleDtos);
                                            if (pobjOperationResult.Success == 0) return string.Empty;
                                        }
                                        _movimientodetalleDtos = new List<movimientodetalleDto>();
                                    }
                                }


                                #endregion
                            }
                        }

                        #endregion
                        dbContext.SaveChanges();
                        pobjOperationResult.Success = 1;
                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "guiaremision", newIdGuiaRemision);
                        ts.Complete();
                        return newIdGuiaRemision;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "GuiaRemisionBL.InsertarGuiaRemision()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return "";
            }
        }

        public CantidadesGuiaRemision CalcularGuiasAnteriores(string pstrIdVenta, string IdMovimientoDetalle)
        {
            CantidadesGuiaRemision objCantidad = new CantidadesGuiaRemision();
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {

                var ProductosGuiasAnteriores = (from a in dbContext.guiaremision

                                                join b in dbContext.venta on new { eliminado = 0, TipoDocRef = a.i_IdTipoDocumento, Serie = a.v_SerieDocumentoRef, Correlativo = a.v_NumeroDocumentoRef } equals new { eliminado = b.i_Eliminado.Value, TipoDocRef = b.i_IdTipoDocumento, Serie = b.v_SerieDocumento, Correlativo = b.v_CorrelativoDocumento } into b_join

                                                from b in b_join.DefaultIfEmpty()

                                                join c in dbContext.guiaremisiondetalle on new { IdGuia = a.v_IdGuiaRemision, eliminado = 0 } equals new { IdGuia = c.v_IdGuiaRemision, eliminado = c.i_Eliminado.Value } into c_join

                                                from c in c_join.DefaultIfEmpty()

                                                where b.v_IdVenta == pstrIdVenta && b.i_Eliminado.Value == 0 && a.i_Eliminado == 0 && a.i_IdEstado == 1 && c.v_IdMovimientoDetalle == IdMovimientoDetalle
                                                select new CantidadesGuiaRemision
                                                {

                                                    CantidadAdministrativa = c.d_Cantidad.Value,
                                                    CantidadEmpaqueAdministrativa = c.d_CantidadEmpaque.Value,
                                                }).ToList();

                objCantidad.CantidadAdministrativa = ProductosGuiasAnteriores.Any() ? ProductosGuiasAnteriores.Sum(x => x.CantidadAdministrativa) : 0;
                objCantidad.CantidadEmpaqueAdministrativa = ProductosGuiasAnteriores.Any() ? ProductosGuiasAnteriores.Sum(x => x.CantidadEmpaqueAdministrativa) : 0;

                return objCantidad;


            }
        }
        public CantidadesGuiaRemision CalcularGuiasAnterioresModificar(string pstrIdVenta, string IdMovimientoDetalle, string GuiaRemisionDetalle)
        {
            CantidadesGuiaRemision objCantidad = new CantidadesGuiaRemision();
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {

                var ProductosGuiasAnteriores = (from a in dbContext.guiaremision

                                                join b in dbContext.venta on new { eliminado = 0, TipoDocRef = a.i_IdTipoDocumento, Serie = a.v_SerieDocumentoRef, Correlativo = a.v_NumeroDocumentoRef } equals new { eliminado = b.i_Eliminado.Value, TipoDocRef = b.i_IdTipoDocumento, Serie = b.v_SerieDocumento, Correlativo = b.v_CorrelativoDocumento } into b_join

                                                from b in b_join.DefaultIfEmpty()

                                                join c in dbContext.guiaremisiondetalle on new { IdGuia = a.v_IdGuiaRemision, eliminado = 0 } equals new { IdGuia = c.v_IdGuiaRemision, eliminado = c.i_Eliminado.Value } into c_join

                                                from c in c_join.DefaultIfEmpty()

                                                where b.v_IdVenta == pstrIdVenta && b.i_Eliminado.Value == 0 && a.i_Eliminado == 0 && a.i_IdEstado == 1 && c.v_IdMovimientoDetalle == IdMovimientoDetalle

                                                && c.v_IdGuiaRemisionDetalle != GuiaRemisionDetalle
                                                select new CantidadesGuiaRemision
                                                {

                                                    CantidadAdministrativa = c.d_Cantidad.Value,
                                                    CantidadEmpaqueAdministrativa = c.d_CantidadEmpaque.Value,
                                                }).ToList();

                objCantidad.CantidadAdministrativa = ProductosGuiasAnteriores.Any() ? ProductosGuiasAnteriores.Sum(x => x.CantidadAdministrativa) : 0;
                objCantidad.CantidadEmpaqueAdministrativa = ProductosGuiasAnteriores.Any() ? ProductosGuiasAnteriores.Sum(x => x.CantidadEmpaqueAdministrativa) : 0;

                return objCantidad;


            }
        }
        public string ActualizarGuiaRemision(ref OperationResult pobjOperationResult, guiaremisionDto pobjDtoEntity, List<string> ClientSession, List<guiaremisiondetalleDto> pTemp_Insertar, List<guiaremisiondetalleDto> pTemp_Editar, List<guiaremisiondetalleDto> pTemp_Eliminar, string IdVenta)
        {
            try
            {

                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    var objSecuentialBL = new SecuentialBL();

                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        string newIdGuiaRemisionDetalle = string.Empty;
                        #region Actualiza Cabecera
                        var intNodeId = int.Parse(ClientSession[0]);
                        var objEntitySource = (from a in dbContext.guiaremision
                                               where a.v_IdGuiaRemision == pobjDtoEntity.v_IdGuiaRemision
                                               select a).FirstOrDefault();

                        pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                        pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                        guiaremision objEntity = guiaremisionAssembler.ToEntity(pobjDtoEntity);

                        dbContext.guiaremision.ApplyCurrentValues(objEntity);
                        #endregion
                        #region Actualiza Detalle
                        foreach (guiaremisiondetalleDto guiaremisiondetalleDto in pTemp_Insertar)
                        {
                            guiaremisiondetalle objEntityGuiaRemisionDetalle = guiaremisiondetalleAssembler.ToEntity(guiaremisiondetalleDto);
                            var SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 39);
                            newIdGuiaRemisionDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "ZS");
                            objEntityGuiaRemisionDetalle.v_IdGuiaRemisionDetalle = newIdGuiaRemisionDetalle;
                            objEntityGuiaRemisionDetalle.t_InsertaFecha = DateTime.Now;
                            objEntityGuiaRemisionDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                            objEntityGuiaRemisionDetalle.i_Eliminado = 0;
                            dbContext.AddToguiaremisiondetalle(objEntityGuiaRemisionDetalle);
                            Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "guiaremisiondetalle", newIdGuiaRemisionDetalle);
                            #region ActualizaNotaSalida

                            if (guiaremisiondetalleDto.v_IdMovimientoDetalle != null)
                            {
                                //ActualizaNotaSalida- CantidadAdministrativa
                                var GuiasAnteriores = CalcularGuiasAnteriores(IdVenta, guiaremisiondetalleDto.v_IdMovimientoDetalle);
                                var nsd = (from a in dbContext.movimientodetalle
                                           where a.i_Eliminado == 0 && a.v_IdMovimientoDetalle == guiaremisiondetalleDto.v_IdMovimientoDetalle
                                           select a).FirstOrDefault();

                                if (nsd != null)
                                {
                                    movimientodetalleDto objMD = movimientodetalleAssembler.ToDTO(nsd);
                                    objMD.d_CantidadAdministrativa = guiaremisiondetalleDto.d_Cantidad + GuiasAnteriores.CantidadAdministrativa;
                                    objMD.d_CantidadEmpaqueAdministrativa = guiaremisiondetalleDto.d_CantidadEmpaque + GuiasAnteriores.CantidadEmpaqueAdministrativa;
                                    movimientodetalle _objMovimientoDetalle = movimientodetalleAssembler.ToEntity(objMD);
                                    var query = (from n in dbContext.movimientodetalle
                                                 where n.v_IdMovimientoDetalle == guiaremisiondetalleDto.v_IdMovimientoDetalle
                                                 select n).FirstOrDefault();

                                    _objMovimientoDetalle.t_ActualizaFecha = DateTime.Now;
                                    _objMovimientoDetalle.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                                    // dbContext.movimientodetalle.ApplyCurrentValues(_objMovimientoDetalle);
                                    dbContext.AddTomovimientodetalle(_objMovimientoDetalle);

                                    Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "movimientodetalle", query.v_IdMovimientoDetalle);
                                }
                            }




                            #endregion

                        }


                        foreach (guiaremisiondetalleDto guiaremisiondetalleDto in pTemp_Editar)
                        {
                            guiaremisiondetalle _objEntity = guiaremisiondetalleAssembler.ToEntity(guiaremisiondetalleDto);

                            var query = (from n in dbContext.guiaremisiondetalle
                                         where n.v_IdGuiaRemisionDetalle == guiaremisiondetalleDto.v_IdGuiaRemisionDetalle
                                         select n).FirstOrDefault();

                            _objEntity.t_ActualizaFecha = DateTime.Now;
                            _objEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                            dbContext.guiaremisiondetalle.ApplyCurrentValues(_objEntity);
                            Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "guiaremisiondetalle", query.v_IdGuiaRemisionDetalle);
                            #region ActualizaNotaSalida

                            if (guiaremisiondetalleDto.v_IdMovimientoDetalle != null)
                            {
                                //ActualizaNotaSalida- CantidadAdministrativa
                                var GuiasAnteriores = CalcularGuiasAnterioresModificar(IdVenta, guiaremisiondetalleDto.v_IdMovimientoDetalle, guiaremisiondetalleDto.v_IdGuiaRemisionDetalle);
                                var nsd = (from a in dbContext.movimientodetalle
                                           where a.i_Eliminado == 0 && a.v_IdMovimientoDetalle == guiaremisiondetalleDto.v_IdMovimientoDetalle
                                           select a).FirstOrDefault();

                                if (nsd != null)
                                {
                                    movimientodetalleDto objMD = movimientodetalleAssembler.ToDTO(nsd);
                                    objMD.d_CantidadAdministrativa = guiaremisiondetalleDto.d_Cantidad + GuiasAnteriores.CantidadAdministrativa;
                                    objMD.d_CantidadEmpaqueAdministrativa = guiaremisiondetalleDto.d_CantidadEmpaque + GuiasAnteriores.CantidadEmpaqueAdministrativa;
                                    movimientodetalle _objMovimientoDetalle = movimientodetalleAssembler.ToEntity(objMD);
                                    var queryns = (from n in dbContext.movimientodetalle
                                                   where n.v_IdMovimientoDetalle == guiaremisiondetalleDto.v_IdMovimientoDetalle
                                                   select n).FirstOrDefault();

                                    _objMovimientoDetalle.t_ActualizaFecha = DateTime.Now;
                                    _objMovimientoDetalle.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                                    dbContext.movimientodetalle.ApplyCurrentValues(_objMovimientoDetalle);
                                    Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "movimientodetalle", query.v_IdMovimientoDetalle);
                                }
                            }

                            #endregion
                        }

                        foreach (guiaremisiondetalleDto guiaremisiondetalleDto in pTemp_Eliminar)
                        {
                            guiaremisiondetalle _objEntity = guiaremisiondetalleAssembler.ToEntity(guiaremisiondetalleDto);
                            var query = (from n in dbContext.guiaremisiondetalle
                                         where n.v_IdGuiaRemisionDetalle == guiaremisiondetalleDto.v_IdGuiaRemisionDetalle
                                         select n).FirstOrDefault();

                            if (query != null)
                            {
                                query.t_ActualizaFecha = DateTime.Now;
                                query.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                                query.i_Eliminado = 1;
                            }

                            dbContext.guiaremisiondetalle.ApplyCurrentValues(query);
                            Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "guiaremisiondetalle", query.v_IdGuiaRemisionDetalle);
                            dbContext.SaveChanges();
                            #region ActualizaNotaSalida

                            if (guiaremisiondetalleDto.v_IdMovimientoDetalle != null)
                            {
                                //ActualizaNotaSalida- CantidadAdministrativa

                                if (IdVenta != null)
                                {
                                    var GuiasAnteriores = CalcularGuiasAnteriores(IdVenta, guiaremisiondetalleDto.v_IdMovimientoDetalle);
                                    var nsd = (from a in dbContext.movimientodetalle
                                               where a.i_Eliminado == 0 && a.v_IdMovimientoDetalle == guiaremisiondetalleDto.v_IdMovimientoDetalle
                                               select a).FirstOrDefault();


                                    if (nsd != null)
                                    {
                                        movimientodetalleDto objMD = movimientodetalleAssembler.ToDTO(nsd);
                                        objMD.d_CantidadAdministrativa = GuiasAnteriores.CantidadAdministrativa;
                                        objMD.d_CantidadEmpaqueAdministrativa = GuiasAnteriores.CantidadEmpaqueAdministrativa;
                                        movimientodetalle _objMovimientoDetalle = movimientodetalleAssembler.ToEntity(objMD);
                                        var queryns = (from n in dbContext.movimientodetalle
                                                       where n.v_IdMovimientoDetalle == guiaremisiondetalleDto.v_IdMovimientoDetalle
                                                       select n).FirstOrDefault();

                                        _objMovimientoDetalle.t_ActualizaFecha = DateTime.Now;
                                        _objMovimientoDetalle.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                                        dbContext.movimientodetalle.ApplyCurrentValues(_objMovimientoDetalle);
                                        Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "movimientodetalle", query.v_IdMovimientoDetalle);
                                    }
                                }
                            }


                            #endregion
                        }



                        #endregion

                        #region Actualiza Cabecera Venta - Pedido
                        var venta =
                            dbContext.venta.FirstOrDefault(
                                p =>
                                    p.i_IdTipoDocumento == pobjDtoEntity.i_IdTipoDocumento.Value &&
                                    p.v_SerieDocumento == pobjDtoEntity.v_SerieDocumentoRef.Trim() &&
                                    p.v_CorrelativoDocumento == pobjDtoEntity.v_NumeroDocumentoRef.Trim() && p.i_Eliminado == 0);

                        if (venta != null)
                        {
                            venta.v_NroGuiaRemisionSerie = pobjDtoEntity.v_SerieGuiaRemision;
                            venta.v_NroGuiaRemisionCorrelativo = pobjDtoEntity.v_NumeroGuiaRemision;
                            dbContext.venta.ApplyCurrentValues(venta);
                        }



                       



                        #endregion
                        dbContext.SaveChanges();
                        pobjOperationResult.Success = 1;
                        Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "guiaremision", pobjDtoEntity.v_IdGuiaRemision);
                        ts.Complete();
                        return pobjDtoEntity.v_IdGuiaRemision;
                    }
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "GuiaRemisionBL.ActualizarGuiaRemision()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return "";
            }
        }


        public void ActualizarNotasSalidas(ref OperationResult objOperationResult)
        {
            try
            {
                objOperationResult.Success = 1;
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {

                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {

                        string Fecha = "26/03/2016 23:59";
                        List<movimientodetalleDto> __TempDetalle_AgregarDto = new List<movimientodetalleDto>();
                        List<movimientodetalleDto> __TempDetalle_ModificarDto = new List<movimientodetalleDto>();
                        List<movimientodetalleDto> __TempDetalle_EliminarDto = new List<movimientodetalleDto>();
                        movimientodetalleDto _movimientodetalleDto = new movimientodetalleDto();
                        movimientoDto _movimientoDto = new movimientoDto();
                        MovimientoBL _objMovimientoBL = new MovimientoBL();
                        var guiasremision = (from a in dbContext.guiaremision
                                             where a.i_Eliminado == 0 && a.i_IdTipoGuia == (int)TiposDocumentos.GuiaInterna
                                            && a.i_IdEstado == 1
                                             select a).ToList().Where(z => z.t_FechaEmision <= DateTime.Parse(Fecha)).ToList();
                        foreach (var itemGuia in guiasremision)
                        {


                            __TempDetalle_AgregarDto = new List<movimientodetalleDto>();
                            __TempDetalle_ModificarDto = new List<movimientodetalleDto>();
                            __TempDetalle_EliminarDto = new List<movimientodetalleDto>();
                            _movimientodetalleDto = new movimientodetalleDto();
                            List<movimientodetalleDto> ListaNs = new List<movimientodetalleDto>();
                            List<movimientodetalleDto> ListaNsEliminar = new List<movimientodetalleDto>();


                            var guiadetalles = (from a in dbContext.guiaremisiondetalle
                                                where a.i_Eliminado == 0 && a.v_IdGuiaRemision == itemGuia.v_IdGuiaRemision
                                                select a).ToList();
                            _movimientoDto = _objMovimientoBL.ObtenerMovimientoCabeceraDesdeCompras(ref objOperationResult, 1, Constants.OrigenGuiaInterna, itemGuia.v_Periodo, itemGuia.v_Mes.Trim(), itemGuia.v_Correlativo.Trim());

                            decimal Cantidad = 0, Total = 0, _Cantidad, _Total;
                            foreach (var detalleGuia in guiadetalles)
                            {
                                _movimientodetalleDto = new movimientodetalleDto();
                                _movimientodetalleDto.v_IdMovimiento = _movimientoDto.v_IdMovimiento;
                                _movimientodetalleDto.v_IdProductoDetalle = detalleGuia.v_IdProductoDetalle; // Fila.Cells["v_IdProductoDetalle"].Value == null ? null : Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                                _movimientodetalleDto.v_NroGuiaRemision = itemGuia.v_SerieGuiaRemision == string.Empty || itemGuia.v_NumeroGuiaRemision == string.Empty ? "" : itemGuia.v_SerieGuiaRemision + "-" + itemGuia.v_NumeroGuiaRemision; // txtSerieGuia.Text.Trim() == string.Empty || txtNumeroGuia.Text.Trim() == string.Empty ? "" : txtSerieGuia.Text.Trim() + "-" + txtNumeroGuia.Text.Trim();
                                _movimientodetalleDto.d_Cantidad = detalleGuia.d_Cantidad;
                                _movimientodetalleDto.i_IdUnidad = detalleGuia.i_IdUnidadMedida;
                                _movimientodetalleDto.d_Precio = detalleGuia.d_Precio;
                                _movimientodetalleDto.d_Total = detalleGuia.d_Precio * detalleGuia.d_Cantidad;
                                _movimientodetalleDto.v_NroPedido = "";
                                _movimientodetalleDto.d_CantidadEmpaque = detalleGuia.d_CantidadEmpaque;// decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());
                                _movimientodetalleDto.i_IdTipoDocumento = itemGuia.i_IdTipoGuia; //  int.Parse(cboTipoGuia.Value.ToString());
                                _movimientodetalleDto.v_NumeroDocumento = _movimientodetalleDto.v_NroGuiaRemision.Trim();
                                __TempDetalle_AgregarDto.Add(_movimientodetalleDto);
                                _Cantidad = _movimientodetalleDto.d_Cantidad.Value;
                                _Total = _movimientodetalleDto.d_Total.Value;
                                Cantidad = Cantidad + _Cantidad;
                                Total = Total + _Total;

                            }


                            _movimientoDto.d_TotalCantidad = Cantidad;
                            _movimientoDto.d_TotalPrecio = Total;

                            var DetallesNotaSalida = (from a in dbContext.movimientodetalle
                                                      where a.i_Eliminado == 0 && a.v_IdMovimiento == _movimientoDto.v_IdMovimiento
                                                      select a).ToList();
                            ListaNs = movimientodetalleAssembler.ToDTOs(DetallesNotaSalida);

                            foreach (var item in ListaNs)
                            {
                                movimientodetalleDto detalleEliminar = new movimientodetalleDto();
                                detalleEliminar = item;
                                detalleEliminar.i_IdAlmacen = "1";
                                ListaNsEliminar.Add(detalleEliminar);

                            }

                            _objMovimientoBL.ActualizarMovimiento(ref objOperationResult, _movimientoDto, Globals.ClientSession.GetAsList(), __TempDetalle_AgregarDto, __TempDetalle_ModificarDto, ListaNsEliminar);

                        }
                        dbContext.SaveChanges();


                    }

                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;

            }
        }
        public BindingList<Gridtemporalventadetalle> ObtenerGuiaRemisionDetallesTemporal(ref OperationResult pobjOperationResult)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    var query = (from n in dbContext.temporalventadetalle
                                 where n.v_IdProductoDetalle != "N002-PE000000000"
                                 select new Gridtemporalventadetalle
                                 {

                                     v_IdTemporalVentaD = n.v_IdTemporalVentaD,
                                     i_IdAlmacen = n.i_IdAlmacen.Value,
                                     v_IdProductoDetalle = n.v_IdProductoDetalle,
                                     i_IdUnidadEmpaque = n.i_IdUnidadEmpaque == null ? -1 : n.i_IdUnidadEmpaque.Value,
                                     d_Cantidad = n.d_Cantidad == null ? 0 : n.d_Cantidad.Value,
                                     i_IdUnidadMedida = n.i_IdUnidadMedida.Value,
                                     d_CantidadEmpaque = n.d_CantidadEmpaque == null ? 0 : n.d_CantidadEmpaque.Value,
                                     d_Precio = n.d_Precio == null ? 0 : n.d_Precio.Value,
                                     d_Total = n.d_Total == null ? 0 : n.d_Total.Value,
                                     d_CantidadBulto = n.d_CantidadBulto == null ? 0 : n.d_CantidadBulto.Value,
                                     i_IdTipoBulto = n.i_IdTipoBulto == null ? -1 : n.i_IdTipoBulto.Value,
                                     v_Observacion = n.v_Observacion,
                                     v_IdMovimientoDetalle = n.v_IdMovimientoDetalle,
                                     d_Valor = 0,
                                     v_Descuento = "0",
                                     d_Descuento = 0,
                                     d_ValorVenta = 0,
                                     d_Igv = 0,
                                     v_IdGuiaRemisionDetalle = null,

                                 }
                    ).ToList();


                    pobjOperationResult.Success = 1;

                    //return query.AsQueryable();

                    var Result = new BindingList<Gridtemporalventadetalle>(query);
                    return Result;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }




        public clienteDto ObtenerClienteporId(ref OperationResult pobjOperationResult, string strCliente, string IdVenta)
        {
            try
            {
                var _objSystemParameterBL = new SystemParameterBL();
                var objOperationResult = new OperationResult();
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {


                    var ClienteVenta = (from a in dbContext.venta
                                        where a.v_IdVenta == IdVenta
                                        select a).FirstOrDefault().ToDTO();

                    var objEntity = (from a in dbContext.cliente
                                     where a.v_IdCliente == strCliente
                                     select new
                                     {
                                         v_IdCliente = a.v_IdCliente,
                                         v_PrimerNombre = a.v_PrimerNombre,
                                         v_SegundoNombre = a.v_SegundoNombre,
                                         v_ApePaterno = a.v_ApePaterno,
                                         v_ApeMaterno = a.v_ApeMaterno,
                                         v_RazonSocial = a.v_RazonSocial.Trim(),
                                         v_NroDocIdentificacion = a.v_NroDocIdentificacion,
                                         v_CodCliente = a.v_CodCliente,
                                         v_DirecPrincipal = a.v_DirecPrincipal ?? "",

                                         i_IdDepartamento = a.i_IdDepartamento ?? -1,
                                         i_IdProvincia = a.i_IdProvincia ?? -1,
                                         i_IdDistrito = a.i_IdDistrito ?? -1,


                                     }).ToList().Select(a => new clienteDto
                                     {


                                         v_IdCliente = a.v_IdCliente,
                                         v_PrimerNombre = a.v_PrimerNombre,
                                         v_SegundoNombre = a.v_SegundoNombre,
                                         v_ApePaterno = a.v_ApePaterno,
                                         v_ApeMaterno = a.v_ApeMaterno,
                                         v_RazonSocial = a.v_RazonSocial.Trim(),
                                         v_NroDocIdentificacion = a.v_NroDocIdentificacion,
                                         v_CodCliente = a.v_CodCliente,
                                         //v_DirecPrincipal = a.v_DirecPrincipal ?? "",
                                         v_DirecPrincipal = ClienteVenta != null ? string.IsNullOrEmpty(ClienteVenta.v_DireccionClienteTemporal) ? a.v_DirecPrincipal : ClienteVenta.v_DireccionClienteTemporal : a.v_DirecPrincipal,
                                         i_IdDepartamento = a.i_IdDepartamento,
                                         i_IdProvincia = a.i_IdProvincia,
                                         i_IdDistrito = a.i_IdDistrito,
                                         i_IdDireccionCliente = ClienteVenta != null ? ClienteVenta.i_IdDireccionCliente ?? -1 : -1



                                     }).FirstOrDefault();


                    if (objEntity != null)
                    {

                        var departamento = objEntity.i_IdDepartamento.Value != -1
                            ? _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult,
                                    1, 112, "")
                                .ToList()
                                .Where(x => x.Id == objEntity.i_IdDepartamento.Value.ToString())
                                .FirstOrDefault()
                                .Value1
                            : "";
                        var provincia = objEntity.i_IdProvincia.Value != -1
                            ? _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult,
                                    objEntity.i_IdDepartamento, 112, "")
                                .Where(x => x.Id == objEntity.i_IdProvincia.Value.ToString())
                                .FirstOrDefault()
                                .Value1
                            : "";
                        var distrito = objEntity.i_IdDistrito.Value != -1
                            ? _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult,
                                    objEntity.i_IdProvincia, 112, "")
                                .Where(x => x.Id == objEntity.i_IdDistrito.Value.ToString())
                                .FirstOrDefault()
                                .Value1
                            : "";
                        objEntity.Ubigeo = departamento + " " + provincia + " " + distrito;

                    }
                    pobjOperationResult.Success = 1;

                    return objEntity;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "GuiaRemisionBL.ObtenerClienteporId()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }



        }

        public ventaDto ObtenerIdVenta(ref OperationResult pobjOperationResult, int tipoDocumento, string SerieDoc, string CorrelativoDoc)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var objEntity = (from a in dbContext.venta

                                     where a.v_SerieDocumento == SerieDoc && a.v_CorrelativoDocumento == CorrelativoDoc && a.i_IdTipoDocumento == tipoDocumento
                                     && a.i_Eliminado == 0

                                     select new ventaDto
                                     {

                                         v_IdVenta = a.v_IdVenta,
                                         v_IdCliente = a.v_IdCliente,
                                         i_IdMoneda = a.i_IdMoneda,
                                         v_NroPedido = a.v_NroPedido,


                                     }
                                             ).FirstOrDefault();

                    pobjOperationResult.Success = 1;

                    return objEntity;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "GuiaRemisionBL.ObtenerIdVenta()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public bool ObtenerFacturaenGuiaRemisionPrueba(ref OperationResult pobjOperationResult, int tipoDocumento, string strSerieDoc, string strNumeroDoc, string idGuiaRemision)
        {
            try
            {
                if (tipoDocumento != -1 && strSerieDoc != null & strNumeroDoc != null)
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        if (string.IsNullOrEmpty(idGuiaRemision)) // si el idGuia es nulo se consulta de un  nuevo no guardado
                        {
                            var query = (from a in dbContext.guiaremision
                                         where a.i_IdTipoDocumento == tipoDocumento & a.v_SerieDocumentoRef == strSerieDoc & a.v_NumeroDocumentoRef == strNumeroDoc && a.i_Eliminado == 0 && a.i_IdEstado == 1
                                         select a).FirstOrDefault();

                            return query != null;
                        }
                        else // Comprueba de una guia que no esta siendo modificada
                        {
                            var query = (from a in dbContext.guiaremision
                                         where a.i_IdTipoDocumento == tipoDocumento & a.v_SerieDocumentoRef == strSerieDoc & a.v_NumeroDocumentoRef == strNumeroDoc & a.v_IdGuiaRemision != idGuiaRemision && a.i_Eliminado == 0 && a.i_IdEstado == 1

                                         select a).FirstOrDefault();
                            return query != null;
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
                pobjOperationResult.AdditionalInformation = "GuiaRemisionBL.ObtenerFacturaenGuiaRemisionPrueba()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return false;
            }
        }

        public guiaremisionDto ObtenerFacturaenGuiaRemision(ref OperationResult pobjOperationResult, int tipoDocumento, string strSerieDoc, string strNumeroDoc)
        {

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var objEntity = (from a in dbContext.guiaremision
                                     where a.i_IdTipoDocumento == tipoDocumento & a.v_SerieDocumentoRef == strSerieDoc & a.v_NumeroDocumentoRef == strNumeroDoc
                                     select new guiaremisionDto
                                     {
                                         v_IdGuiaRemision = a.v_IdGuiaRemision

                                     }).FirstOrDefault();

                    return objEntity;
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "GuiaRemisionBL.ObtenerFacturaenGuiaRemision()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }

        }

        public bool ComprobarExistenciaCorrelativoDocumento(ref OperationResult pobjOperationResult, int pintIdTipoDocumento, string pstrSerieDoc, string pstrCorrelativoDoc, string pstrIdGuiaRemision)
        {
            try
            {
                if (pintIdTipoDocumento != -1 && pstrSerieDoc != null && pstrCorrelativoDoc != null) // comprueba que tenga los datos necesarios para no consultar a la Bd en vano.
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        if (pstrIdGuiaRemision == null) // SI el idVenta es nulo se esta consultado de una venta nueva que no ha sido guardada
                        {
                            var query = (from n in dbContext.guiaremision
                                         where n.i_Eliminado == 0
                                         && n.i_IdTipoDocumento == pintIdTipoDocumento
                                         && n.v_SerieGuiaRemision == pstrSerieDoc && n.v_NumeroGuiaRemision == pstrCorrelativoDoc
                                         select n
                                     ).FirstOrDefault();
                            return query != null;
                        }
                        else // si no es nulo se comprueba de una venta que esta siendo modificada
                        {
                            var query = (from n in dbContext.guiaremision
                                         where n.i_Eliminado == 0
                                         && n.i_IdTipoDocumento == pintIdTipoDocumento
                                         && n.v_SerieGuiaRemision == pstrSerieDoc && n.v_NumeroGuiaRemision == pstrCorrelativoDoc
                                         && n.v_IdGuiaRemision != pstrIdGuiaRemision
                                         select n
                                         ).FirstOrDefault();
                            return query != null;
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
                pobjOperationResult.AdditionalInformation = "GuiaRemisionBL.ComprobarExistenciaCorrelativoDocumento()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return false;
            }
        }

        public bool ExisteNroDocumento(int TipoDoc, string pstrSerie, string pstrCorrelativo)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var registro = (from n in dbContext.guiaremision
                                where n.i_Eliminado == 0 && n.v_SerieGuiaRemision == pstrSerie && n.v_NumeroGuiaRemision == pstrCorrelativo
                                select n).FirstOrDefault();

                return registro == null;
            }
        }

        public bool ObtenerExistenciaGuiaRemision(OperationResult pobjOperationResult, string strSerieGuia, string strNumeroGuia, string strIdGuiaRemision, int TipoGuia)
        {

            try
            {
                if (strSerieGuia != null && strNumeroGuia != null) // comprueba que tenga los datos necesarios para no consultar a la Bd en vano.
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        if (strIdGuiaRemision == null) // SI el idGuia es nula, se esta consultado de una guia  nueva que no ha sido guardada
                        {
                            var query = (from n in dbContext.guiaremision
                                         where n.i_Eliminado == 0 && n.i_IdTipoGuia == TipoGuia

                                         && n.v_SerieGuiaRemision == strSerieGuia && n.v_NumeroGuiaRemision == strNumeroGuia
                                         select n
                                     ).FirstOrDefault();
                            return query != null;
                        }
                        else // si no es nulo se comprueba de una guiaremision que esta siendo modificada
                        {
                            var query = (from n in dbContext.guiaremision
                                         where n.i_Eliminado == 0
                                         && n.v_SerieGuiaRemision == strSerieGuia && n.v_NumeroGuiaRemision == strNumeroGuia && n.i_IdTipoGuia == TipoGuia
                                         && n.v_IdGuiaRemision != strIdGuiaRemision
                                         select n
                                         ).FirstOrDefault();
                            return query != null;
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
                pobjOperationResult.AdditionalInformation = "GuiaRemisionBL.ObtenerExistenciaGuiaRemision()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return false;
            }

        }

        /// <summary>
        /// Obtiene los detalles de la venta para la guia de remision, exceptuando los detalles que pasaron a guias anteriores.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstrIdVenta"></param>
        public BindingList<guiaremisiondetalleDto> ObtenerDetallesVentaParaGuiaRemision(ref OperationResult pobjOperationResult, string pstrIdVenta)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var objProductoBL = new ProductoBL();
                    var listaResultante = new List<guiaremisiondetalleDto>();
                    var listaVentaDetalle = dbContext.ventadetalle.Where(p => p.v_IdVenta == pstrIdVenta && p.i_Eliminado == 0).ToList().ToDTOs().
                        Where(o => !objProductoBL.ProductoDetalleEsServicio(o.v_IdProductoDetalle)).ToList();

                    if (listaVentaDetalle.Any())
                    {
                        #region Obtiene Los detalles de las guias anteriores
                        var guiasDetalleAnteriores = (from a in dbContext.guiaremisiondetalle

                                                      join g in dbContext.guiaremision on a.v_IdGuiaRemision equals g.v_IdGuiaRemision into g_join
                                                      from g in g_join.DefaultIfEmpty()

                                                      join b in dbContext.venta on new
                                                      {
                                                          eliminado = 0,
                                                          TipoDocRef = g.i_IdTipoDocumento,
                                                          Serie = g.v_SerieDocumentoRef,
                                                          Correlativo = g.v_NumeroDocumentoRef
                                                      } equals new
                                                          {
                                                              eliminado = b.i_Eliminado.Value,
                                                              TipoDocRef = b.i_IdTipoDocumento,
                                                              Serie = b.v_SerieDocumento,
                                                              Correlativo = b.v_CorrelativoDocumento
                                                          } into b_join
                                                      from b in b_join.DefaultIfEmpty()
                                                      where a.i_Eliminado == 0 && b.v_IdVenta == pstrIdVenta
                                                      select a).ToList();
                        #endregion

                        #region Obtiene el agrupado de las guias anteriores y la suma de sus cantidades
                        var agrupado = guiasDetalleAnteriores.GroupBy(g => new { g.v_IdProductoDetalle })
                                            .Select(p => new
                                            {
                                                IdProductoDetalle = p.FirstOrDefault().v_IdProductoDetalle,
                                                CantidadUsada = p.Sum(o => o.d_Cantidad ?? 0),
                                                CantidadEmpaqueUsada = p.Sum(o => o.d_CantidadEmpaque ?? 0) // como usar ?
                                            });
                        #endregion

                        #region Recorre cada detalle de la venta y revisa si se ha usado ya la cantidad en otras guias, si es asi se les descuenta
                        foreach (var ventaDetalle in listaVentaDetalle)
                        {
                            var recolectado = agrupado.FirstOrDefault(p => p.IdProductoDetalle == ventaDetalle.v_IdProductoDetalle);

                            var cantidadSobrante = ventaDetalle.d_Cantidad.Value - (recolectado != null ? recolectado.CantidadUsada : 0);
                            var cantidadEmpaqueSobrante = ventaDetalle.d_CantidadEmpaque.Value - (recolectado != null ? recolectado.CantidadEmpaqueUsada : 0);

                            if (cantidadSobrante > 0)
                            {
                                var result = new guiaremisiondetalleDto
                                {
                                    v_IdProductoDetalle = ventaDetalle.v_IdProductoDetalle,
                                    v_Observacion = ventaDetalle.v_Observaciones,
                                    i_IdAlmacen = ventaDetalle.i_IdAlmacen,
                                    d_Cantidad = cantidadSobrante,
                                    d_CantidadBulto = ventaDetalle.d_CantidaBulto,
                                    d_Precio = ventaDetalle.d_Precio,
                                    d_CantidadEmpaque = cantidadEmpaqueSobrante
                                };
                                result.d_Total = result.d_CantidadBulto * result.d_Precio;
                                var productoRelacionado = dbContext.productodetalle.FirstOrDefault(p => p.v_IdProductoDetalle == ventaDetalle.v_IdProductoDetalle);
                                if (productoRelacionado != null && productoRelacionado.producto != null)
                                {
                                    var i_IdUnidadMedidaProducto = productoRelacionado.producto.i_IdUnidadMedida.Value;
                                    result.v_CodInterno = productoRelacionado.producto.v_CodInterno;
                                    result.v_Descripcion = productoRelacionado.producto.v_Descripcion;
                                    result.i_IdUnidadMedidaProducto = productoRelacionado.producto.i_IdUnidadMedida.Value.ToString();
                                    result.Empaque = productoRelacionado.producto.d_Empaque.Value.ToString();
                                    result.UMEmpaque = dbContext.datahierarchy.FirstOrDefault(p => p.i_GroupId == 17 && p.i_ItemId == i_IdUnidadMedidaProducto).v_Value1;
                                }
                                listaResultante.Add(result);
                            }
                        }
                        #endregion
                    }

                    pobjOperationResult.Success = 1;
                    return new BindingList<guiaremisiondetalleDto>(listaResultante);
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "GuiaRemisionBL.ObtenerDetallesVentaParaGuiaRemision()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public void InsertarDetalleVentaADetalleGuiaRemision(ref OperationResult pobjOperationResult, string pstrIdVenta)
        {
            SecuentialBL objSecuentialBL = new SecuentialBL();
            try
            {
                bool TieneGuiasAnteriores = false;
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    List<temporalventadetalleDto> ProductosInsertar = new List<temporalventadetalleDto>();


                    var Observacion = (from n in dbContext.ventadetalle

                                       where n.v_IdVenta == pstrIdVenta && n.i_Eliminado == 0

                                       select new
                                       {
                                           v_Observacion = n.v_Observaciones,

                                       }).FirstOrDefault();

                    List<temporalventadetalleDto> ProductosGuiasAnteriores = (from a in dbContext.guiaremision

                                                                              join b in dbContext.venta on new { eliminado = 0, TipoDocRef = a.i_IdTipoDocumento, Serie = a.v_SerieDocumentoRef, Correlativo = a.v_NumeroDocumentoRef } equals new { eliminado = b.i_Eliminado.Value, TipoDocRef = b.i_IdTipoDocumento, Serie = b.v_SerieDocumento, Correlativo = b.v_CorrelativoDocumento } into b_join

                                                                              from b in b_join.DefaultIfEmpty()

                                                                              join c in dbContext.guiaremisiondetalle on new { IdGuia = a.v_IdGuiaRemision, eliminado = 0 } equals new { IdGuia = c.v_IdGuiaRemision, eliminado = c.i_Eliminado.Value } into c_join

                                                                              from c in c_join.DefaultIfEmpty()

                                                                              where b.v_IdVenta == pstrIdVenta && b.i_Eliminado.Value == 0 && a.i_Eliminado == 0 && a.i_IdEstado == 1
                                                                              select new
                                                                              {

                                                                                  i_IdAlmacen = c.i_IdAlmacen,
                                                                                  v_Observacion = "",
                                                                                  v_IdProductoDetalle = c.v_IdProductoDetalle,
                                                                                  d_Cantidad = c.d_Cantidad,
                                                                                  d_Precio = c.d_Precio,
                                                                                  d_CantidadEmpaque = c.d_CantidadEmpaque,
                                                                                  v_IdMovimientoDetalle = c.v_IdMovimientoDetalle,
                                                                                  d_Valor = c.d_Valor,
                                                                                  v_Descuento = "",



                                                                              }).ToList().Select
                                                                              (p => new temporalventadetalleDto
                                                                              {

                                                                                  i_IdAlmacen = p.i_IdAlmacen,
                                                                                  v_Observacion = Observacion.v_Observacion,
                                                                                  v_IdProductoDetalle = p.v_IdProductoDetalle,
                                                                                  d_Cantidad = p.d_Cantidad,
                                                                                  d_Precio = p.d_Precio,
                                                                                  d_CantidadEmpaque = p.d_CantidadEmpaque,
                                                                                  v_IdMovimientoDetalle = p.v_IdMovimientoDetalle,
                                                                                  d_Valor = null,
                                                                                  v_Descuento = p.v_Descuento,

                                                                              }).ToList();

                    List<temporalventadetalleDto> ListaDetallesVenta = (from n in dbContext.ventadetalle
                                                                        join a in dbContext.productodetalle on new { ProdDetalle = n.v_IdProductoDetalle, eliminado = 0 } equals new { ProdDetalle = a.v_IdProductoDetalle, eliminado = a.i_Eliminado.Value } into a_join
                                                                        from a in a_join.DefaultIfEmpty()
                                                                        join b in dbContext.producto on new { Prod = a.v_IdProducto, eliminado = 0 } equals new { Prod = b.v_IdProducto, eliminado = b.i_Eliminado.Value } into b_join
                                                                        from b in b_join.DefaultIfEmpty()

                                                                        //where n.v_IdVenta == pstrIdVenta && n.i_Eliminado == 0 && n.v_IdProductoDetalle != null && b.i_EsServicio == 0
                                                                        where n.v_IdVenta == pstrIdVenta && n.i_Eliminado == 0 && n.v_IdProductoDetalle != null
                                                                        select new temporalventadetalleDto
                                                                        {
                                                                            i_IdAlmacen = n.i_IdAlmacen,
                                                                            v_Observacion = n.v_Observaciones,
                                                                            v_IdProductoDetalle = n.v_IdProductoDetalle,
                                                                            d_Cantidad = n.d_Cantidad,
                                                                            d_Precio = n.d_Precio,
                                                                            d_CantidadEmpaque = n.d_CantidadEmpaque,
                                                                            i_IdUnidadMedida = n.i_IdUnidadMedida,
                                                                            v_IdMovimientoDetalle = n.v_IdMovimientoDetalle,
                                                                            i_EsServicio = b.i_EsServicio == null ? 0 : b.i_EsServicio.Value,
                                                                            d_Valor = 0,
                                                                            v_Descuento = n.v_FacturaRef,

                                                                        }).ToList();

                    ListaDetallesVenta = Globals.ClientSession.i_IncluirServicioGuiaVenta == 1 ? ListaDetallesVenta : ListaDetallesVenta.Where(x => x.i_EsServicio == 0).ToList();
                    foreach (temporalventadetalleDto registro in ListaDetallesVenta)
                    {

                        List<string> ProductoInsertado = ProductosInsertar.Select(x => x.v_IdProductoDetalle).ToList();
                        if (!ListaDetallesVenta.FindAll(x => x.v_IdProductoDetalle == registro.v_IdProductoDetalle).Any())
                        {
                            if (!ProductoInsertado.Contains(registro.v_IdProductoDetalle))
                            {
                                ProductosInsertar.Add(registro);
                            }
                        }
                        else
                        {
                            if (!ProductoInsertado.Contains(registro.v_IdProductoDetalle))
                            {
                                //registro.d_Cantidad = ListaDetallesVenta.FindAll(x => x.v_IdProductoDetalle == registro.v_IdProductoDetalle).FirstOrDefault().d_Cantidad - ProductosGuiasAnteriores.FindAll(y => y.v_IdProductoDetalle == registro.v_IdProductoDetalle).Sum(x => x.d_Cantidad);
                                registro.d_Cantidad = ListaDetallesVenta.FindAll(x => x.v_IdProductoDetalle == registro.v_IdProductoDetalle).Sum (o=>o.d_Cantidad) - ProductosGuiasAnteriores.FindAll(y => y.v_IdProductoDetalle == registro.v_IdProductoDetalle).Sum(x => x.d_Cantidad);
                                ProductosInsertar.Add(registro);
                            }
                            
                        }
                    }


                    foreach (temporalventadetalleDto registro in ProductosGuiasAnteriores)
                    {
                        TieneGuiasAnteriores = true;
                        List<string> ProductoInsertado = ProductosInsertar.Select(x => x.v_IdProductoDetalle).ToList();
                        if (!ListaDetallesVenta.FindAll(x => x.v_IdProductoDetalle == registro.v_IdProductoDetalle).Any())
                        {
                            if (!ProductoInsertado.Contains(registro.v_IdProductoDetalle))
                            {
                                ProductosInsertar.Add(registro);
                            }
                        }
                        else
                        {
                            if (!ProductoInsertado.Contains(registro.v_IdProductoDetalle))
                            {
                                registro.d_Cantidad = ListaDetallesVenta.FindAll(x => x.v_IdProductoDetalle == registro.v_IdProductoDetalle).FirstOrDefault().d_Cantidad - ProductosGuiasAnteriores.FindAll(y => y.v_IdProductoDetalle == registro.v_IdProductoDetalle).Sum(x => x.d_Cantidad);
                                ProductosInsertar.Add(registro);
                            }
                        }
                    }
                    ProductosInsertar = ProductosInsertar.Where(x => x.d_Cantidad > 0).ToList();
                    foreach (var Fila in ProductosInsertar)
                    {

                        temporalventadetalle objEntityTemporalVentaDetalle = Fila.ToEntity();
                        dbContext.AddTotemporalventadetalle(objEntityTemporalVentaDetalle);
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "GuiaRemisionBL.InsertarDetalleVentaADetalleGuiaRemision()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }
        public decimal CantidadExcedentePorGuiaDetalle(ref OperationResult objOperationResult, int pIdAlmacen, string pIdProductoDetalle, decimal pCantidad,
            string IdDetalleGuia, int IdUnidadVenta)
        {
            try
            {
                var periodo = Globals.ClientSession.i_Periodo.ToString();
                objOperationResult.Success = 1;
                var dbContext = new SAMBHSEntitiesModelWin();
                var _productoalmacen = (from n in dbContext.productoalmacen
                                        where
                                            n.i_IdAlmacen == pIdAlmacen && n.v_ProductoDetalleId == pIdProductoDetalle && n.i_Eliminado == 0 &&
                                            n.v_Periodo == periodo
                                        select n).FirstOrDefault();
                if (_productoalmacen != null)
                {
                    decimal saldoAlmacen = 0;
                    if (IdDetalleGuia != null)
                    {
                        var _ventadetalleCantidad = (from n in dbContext.guiaremisiondetalle
                                                     where n.v_IdGuiaRemisionDetalle == IdDetalleGuia && n.i_Eliminado == 0
                                                     select new { n.d_Cantidad }).FirstOrDefault();

                        decimal Separacion = _productoalmacen.d_SeparacionTotal ?? 0;
                        if (_productoalmacen.d_StockActual != null)
                            if (_ventadetalleCantidad != null)
                                saldoAlmacen = (_productoalmacen.d_StockActual.Value +
                                                _ventadetalleCantidad.d_Cantidad.Value) - (Separacion);
                    }
                    else
                    {
                        var Stock = _productoalmacen.d_StockActual ?? 0;
                        var Separacion = _productoalmacen.d_SeparacionTotal ?? 0;
                        saldoAlmacen = Stock - Separacion;
                    }

                    var Excedente = saldoAlmacen -
                                    Utils.Windows.DevolverTotalUnidades(pIdProductoDetalle, pCantidad, IdUnidadVenta);

                    return Excedente;
                }
                else
                {
                    return -1;
                }

            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "GuiaRemisionBL.InsertarDetalleVentaADetalleGuiaRemision()";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return 0;
            }
        }
        public transportistaDto ObtenerTransportistaPorNroDocumento(ref OperationResult pobjOperationResult, string pstrRucTransportista)
        {

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    transportistaDto objDtoEntity = null;

                    var objEntity = (from a in dbContext.transportista
                                     where a.v_NumeroDocumento == pstrRucTransportista && a.i_Eliminado == 0
                                     select a).FirstOrDefault();

                    if (objEntity != null)
                        objDtoEntity = transportistaAssembler.ToDTO(objEntity);

                    pobjOperationResult.Success = 1;
                    return objDtoEntity;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "GuiaRemisionBL.ObtenerTransportistaPorNroDocumento()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }

        }

        public string[] DevolverClientePorNroDocumento(ref OperationResult pobjOperationResult, string NroDocumento)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from n in dbContext.cliente
                                 join o in dbContext.clientedirecciones on new { client = n.v_IdCliente, eliminado = 0, pred = 1 } equals new { client = o.v_IdCliente, eliminado = o.i_Eliminado.Value, pred = o.i_EsDireccionPredeterminada.Value } into o_jojn
                                 from o in o_jojn.DefaultIfEmpty()
                                 where n.i_Eliminado == 0 && n.v_FlagPantalla == "C" && n.v_NroDocIdentificacion == NroDocumento
                                 select new
                                 {

                                     v_IdCliente = n.v_IdCliente,
                                     v_CodCliente = n.v_CodCliente,
                                     v_Nombres = n.v_PrimerNombre + " " + n.v_ApePaterno + " " + n.v_ApeMaterno + " " + n.v_RazonSocial,
                                     v_DirecPrincipal = n.v_DirecPrincipal,
                                     i_IdDireccionCliente = o.i_IdDireccionCliente,

                                 }).FirstOrDefault();

                    pobjOperationResult.Success = 1;

                    if (query != null)
                    {
                        string[] Cadena = new string[5];
                        Cadena[0] = query.v_IdCliente;
                        Cadena[1] = query.v_CodCliente;
                        Cadena[2] = (query.v_Nombres).Trim();
                        Cadena[3] = query.v_DirecPrincipal;
                        Cadena[4] = query.i_IdDireccionCliente.ToString();
                        return Cadena;
                    }
                    else
                    {
                        return null;
                    }
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public Tuple<string, string> ObtenerDatosEmpresa()
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                //var Direccion = (from n in dbContext.establecimientoalmacen

                //                 join a in dbContext.establecimiento on n.i_IdEstablecimiento equals a.i_IdEstablecimiento

                //                 where n.i_Eliminado == 0 && a.i_Eliminado == 0 && n.i_IdAlmacen == Globals.ClientSession.i_IdAlmacenPredeterminado

                //                 select new
                //                 {
                //                     a.v_Direccion
                //                 }).FirstOrDefault();

                var direccion = (from a in dbContext.almacen
                                 where a.i_Eliminado == 0 && a.i_IdAlmacen == Globals.ClientSession.i_IdAlmacenPredeterminado
                                 select new
                                 {
                                     a.v_Direccion,
                                     a.v_Ubigueo
                                 }).FirstOrDefault();
                return direccion == null ? new Tuple<string, string>("", "")
                    : new Tuple<string, string>(direccion.v_Direccion, direccion.v_Ubigueo);
            }
        }
        public decimal ObtenerRedondeo(string pstrIdVenta)
        {
            decimal redondeo;
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var Redondeo = (from n in dbContext.venta

                                join a in dbContext.ventadetalle on n.v_IdVenta equals a.v_IdVenta into a_join

                                from a in a_join.DefaultIfEmpty()

                                where n.v_IdVenta == pstrIdVenta && a.v_IdProductoDetalle == "N002-PE000000000"
                                select a).FirstOrDefault();

                redondeo = Redondeo != null ? Redondeo.d_PrecioVenta.Value : 0;
            }

            return redondeo;
        }

        #endregion
        #region Bandeja

        public List<guiaremisionDto> ListarBusquedaGuiaRemision(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression, DateTime F_Inicio, DateTime F_Fin)
        {
            try
            {
                IQueryable<guiaremisionDto> query = null;

                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    query = (from n in dbContext.guiaremision

                             join J2 in dbContext.systemuser on new { i_UpdateUserId = n.i_ActualizaIdUsuario.Value, eliminado = 0 }
                                                            equals new { i_UpdateUserId = J2.i_SystemUserId, eliminado = J2.i_IsDeleted.Value } into J2_join
                             from J2 in J2_join.DefaultIfEmpty()

                             join J3 in dbContext.systemuser on new { i_InsertUserId = n.i_InsertaIdUsuario.Value, eliminado = 0 }
                                                            equals new { i_InsertUserId = J3.i_SystemUserId, eliminado = J3.i_IsDeleted.Value } into J3_join
                             from J3 in J3_join.DefaultIfEmpty()

                             join J4 in dbContext.documento on new { i_IdTipoDocumento = n.i_IdTipoDocumento.Value, eliminado = 0 }
                                                            equals new { i_IdTipoDocumento = J4.i_CodigoDocumento, eliminado = J4.i_Eliminado.Value } into J4_join
                             from J4 in J4_join.DefaultIfEmpty()

                             join J5 in dbContext.documento on new { i_IdTipoGuia = n.i_IdTipoGuia.Value, eliminado = 0 }
                                                            equals new { i_IdTipoGuia = J5.i_CodigoDocumento, eliminado = J5.i_Eliminado.Value } into J5_join
                             from J5 in J5_join.DefaultIfEmpty()

                             join J6 in dbContext.almacen on new { almacenDestino = n.i_IdAlmacenDestino ?? -1, eliminado = 0 } equals new { almacenDestino = J6.i_IdAlmacen, eliminado = J6.i_Eliminado.Value } into J6_join
                             from J6 in J6_join.DefaultIfEmpty()

                             join J7 in dbContext.cliente on new { cliente = n.v_IdCliente } equals new { cliente = J7.v_IdCliente } into J7_join
                             from J7 in J7_join.DefaultIfEmpty()
                             where n.i_Eliminado == 0 && n.t_FechaEmision >= F_Inicio && n.t_FechaEmision <= F_Fin


                             select new guiaremisionDto
                             {
                                 v_IdGuiaRemision = n.v_IdGuiaRemision,
                                 v_Periodo = n.v_Periodo,
                                 v_Mes = n.v_Mes,
                                 v_Correlativo = n.v_Correlativo,
                                 v_NumeroRegistro = (n.v_Mes.Trim() + "-" + n.v_Correlativo.Trim()).Trim(),
                                 v_SerieDocumentoRef = n.v_SerieDocumentoRef,
                                 v_NumeroDocumentoRef = n.v_NumeroDocumentoRef,
                                 v_Documento = n.v_SerieDocumentoRef == string.Empty || n.v_NumeroDocumentoRef == string.Empty ? "" : (n.v_SerieDocumentoRef.Trim() + "-" + n.v_NumeroDocumentoRef.Trim()).Trim(),
                                 t_FechaEmision = n.t_FechaEmision.Value,
                                 t_FechaTraslado = n.t_FechaTraslado.Value,
                                 i_IdEstado = n.i_IdEstado ?? 0,

                                 AgenciaTransportes = n.agenciatransporte != null ? n.agenciatransporte.v_NombreRazonSocial : "",
                                 t_InsertaFecha = n.t_InsertaFecha,
                                 i_ActualizaIdUsuario = n.i_ActualizaIdUsuario.Value,
                                 t_ActualizaFecha = n.t_ActualizaFecha,
                                 v_UsuarioModificacion = J2 == null ? "" : J2.v_UserName,
                                 v_UsuarioCreacion = J3 == null ? "" : J3.v_UserName,
                                 v_SerieGuiaRemision = n.v_SerieGuiaRemision,
                                 v_NumeroGuiaRemision = n.v_NumeroGuiaRemision,
                                 v_Guia = (n.v_SerieGuiaRemision.Trim() + "-" + n.v_NumeroGuiaRemision.Trim()).Trim(),

                                 NombreTransportista = n.transportista != null ? n.transportista.v_NombreRazonSocial : "",
                                 TipoDocumentoReferencia = J4 == null ? "" : J4.v_Siglas,
                                 TipoDocumento = J5 == null ? "" : J5.v_Siglas,

                                 NombreCliente = J7 != null ? (J7.v_ApePaterno + " " + J7.v_ApeMaterno + " " + J7.v_PrimerNombre + " " + J7.v_SegundoNombre + " " + J7.v_RazonSocial).Trim() : "",   // n.cliente != null ? (n.cliente.v_ApePaterno + " " + n.cliente.v_ApeMaterno + " " + n.cliente.v_PrimerNombre + " " + n.cliente.v_SegundoNombre + " " + n.cliente.v_RazonSocial).Trim() : "",
                                 v_IdCliente = n.v_IdCliente,
                                 i_IdEstablecimiento = n.i_IdEstablecimiento.Value,
                                 v_IdAgenciaTransporte = n.agenciatransporte != null ? n.agenciatransporte.v_IdAgenciaTransporte : "",
                                 v_IdTransportista = n.v_IdTransportista,
                                 i_IdMoneda = n.i_IdMoneda,
                                 Moneda = n.i_IdMoneda == 1 ? "S" : "D",
                                 d_Total = n.d_Total ?? 0,
                                 i_IdTipoDocumento = n.i_IdTipoDocumento,
                                 Origen = Constants.OrigenGuiaInterna,
                                 NroDocCliente = J7.v_NroDocIdentificacion,
                                 i_IdAlmacenDestino = n.i_IdAlmacenDestino ?? -1,
                                 AlmacenDestino = J6 == null ? "" : J6.v_Nombre,
                                 i_EstadoSunat = n.i_EstadoSunat
                             }
                                ).ToList().AsQueryable();
                }

                if (!string.IsNullOrEmpty(pstrFilterExpression))
                {
                    query = query.Where(pstrFilterExpression);
                }
                if (!string.IsNullOrEmpty(pstrSortExpression))
                {
                    query = query.OrderBy(pstrSortExpression);
                }

                List<guiaremisionDto> objData = query.ToList();
                //objData = Globals.ClientSession.v_RucEmpresa == Constants.RucCMR ? Globals.ClientSession.i_SystemUserId == 74 ? objData.Where(k => k.v_SerieGuiaRemision.StartsWith("0006")).ToList() : objData.Where(k => !k.v_SerieGuiaRemision.StartsWith("0006")).ToList() : objData;
                pobjOperationResult.Success = 1;
                return objData;


            }
            catch (Exception ex)
            {
                dynamic error = ex;
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = error.InnerException.ErrorSql;
                return null;
            }
        }


        public void Except(ref OperationResult objOperationResult)
        {
            try
            {
                objOperationResult.Success = 1;
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {

                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var query1 = (from a in dbContext.guiaremisiondetalle
                                      join b in dbContext.movimientodetalle on new { id = a.v_IdMovimientoDetalle, eliminado = 0 } equals new { id = b.v_IdMovimientoDetalle, eliminado = b.i_Eliminado.Value } into b_join
                                      from b in b_join.DefaultIfEmpty()
                                      where b.d_Cantidad != a.d_Cantidad
                                      select a).ToList();

                        var guiaremisiondetalle = dbContext.guiaremisiondetalle.ToList();
                        var movimientodetalle = dbContext.movimientodetalle.ToList();
                        // var movimiento = dbContext.movimiento.Where (o=>o.i_IdTipoMovimiento ==2 && o.v_Periodo =="2017").ToList();
                        var guiaremision = dbContext.guiaremision.ToList();
                        var productos = (from a in dbContext.productodetalle
                                         join b in dbContext.producto on new { p = a.v_IdProducto, eliminado = 0 } equals new { p = b.v_IdProducto, eliminado = b.i_Eliminado.Value } into b_join
                                         from b in b_join.DefaultIfEmpty()
                                         select new
                                         {
                                             v_IdProductoDetalle = a.v_IdProductoDetalle,
                                             CodProducto = b.v_CodInterno,
                                         }).ToList();


                        string rutaGuardar1 = "D:" + "\\" + "Observadas" + ".txt";

                        //if (!System.IO.File.Exists(rutaGuardar1))
                        //{
                        //    System.IO.Directory.CreateDirectory(rutaGuardar1);
                        //}
                        using (System.IO.StreamWriter escribir = new System.IO.StreamWriter(rutaGuardar1))
                        {
                            string Fila = "";
                            foreach (var item in query1)
                            {
                                var gd = guiaremisiondetalle.Where(o => o.v_IdMovimientoDetalle == item.v_IdMovimientoDetalle).ToList();
                                if (gd.Count() == 1)
                                {

                                    var nsd = (from a in movimientodetalle
                                               where a.v_IdMovimientoDetalle == item.v_IdMovimientoDetalle && a.i_Eliminado == 0
                                               select a).FirstOrDefault();
                                    var cantidadantigua = nsd.d_Cantidad;
                                    var ns = dbContext.movimiento.Where(o => o.v_IdMovimiento == nsd.v_IdMovimiento).FirstOrDefault();
                                    var g = guiaremision.Where(o => o.v_IdGuiaRemision == gd.FirstOrDefault().v_IdGuiaRemision).FirstOrDefault();
                                    var prod = productos.Where(o => o.v_IdProductoDetalle == nsd.v_IdProductoDetalle).FirstOrDefault();
                                    nsd.d_Cantidad = gd.FirstOrDefault().d_Cantidad;
                                    nsd.d_CantidadEmpaque = gd.FirstOrDefault().d_CantidadEmpaque;
                                    dbContext.movimientodetalle.ApplyCurrentValues(nsd);
                                    string NroNotaSalida = ns != null ? ns.v_Mes + " " + ns.v_Correlativo : "";
                                    string NroGuia = g != null ? g.v_Mes + " " + g.v_Correlativo : "";
                                    string CodProducto = prod != null ? prod.CodProducto : "";

                                    Fila = "Nro Nota Salida :" + NroNotaSalida + " Cantidad ns: " + cantidadantigua + " - Producto  :" + CodProducto + " - Nro Guia :" + NroGuia + " - Cantidad guia : " + gd.FirstOrDefault().d_Cantidad;
                                    escribir.WriteLine(Fila);
                                }
                                else
                                {
                                    gd = gd.Where(o => o.i_Eliminado == 0).ToList();
                                    if (gd.Count() == 1)
                                    {
                                        var nds = movimientodetalle.Where(o => o.v_IdMovimientoDetalle == item.v_IdMovimientoDetalle && o.i_Eliminado == 0).FirstOrDefault();
                                        var cantidadantigua = nds.d_Cantidad;
                                        var ns = dbContext.movimiento.Where(o => o.v_IdMovimiento == nds.v_IdMovimiento).FirstOrDefault();
                                        var g = guiaremision.Where(o => o.v_IdGuiaRemision == gd.FirstOrDefault().v_IdGuiaRemision).FirstOrDefault();
                                        var prod = productos.Where(o => o.v_IdProductoDetalle == nds.v_IdProductoDetalle).FirstOrDefault();
                                        nds.d_Cantidad = gd.FirstOrDefault().d_Cantidad;
                                        nds.d_CantidadEmpaque = gd.FirstOrDefault().d_CantidadEmpaque;
                                        dbContext.movimientodetalle.ApplyCurrentValues(nds);
                                        string NroNotaSalida = ns != null ? ns.v_Mes + " " + ns.v_Correlativo : "";
                                        string NroGuia = g != null ? g.v_Mes + " " + g.v_Correlativo : "";
                                        string CodProducto = prod != null ? prod.CodProducto : "";
                                        Fila = "Nro Nota Salida :" + NroNotaSalida + " Cantidad ns: " + cantidadantigua + " - Producto :" + CodProducto + " - Nro Guia :" + NroGuia + " - Cantidad guia : " + gd.FirstOrDefault().d_Cantidad;
                                        escribir.WriteLine(Fila);

                                    }
                                }
                            }
                        }

                        dbContext.SaveChanges();
                        ts.Complete();
                    }

                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "GuiaRemisionBL.Except()";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return;
            }


        }

        public List<guiaremisionDto> ListarBusquedaGuiaRemisionPendientes(ref OperationResult pobjOperationResult, int IdAlmacenDestino, DateTime F_Inicio, DateTime F_Fin, int Estado)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();



                var query = (from n in dbContext.guiaremision

                             //join J2 in dbContext.systemuser on new { i_UpdateUserId = n.i_ActualizaIdUsuario.Value, eliminado = 0 }
                             //                               equals new { i_UpdateUserId = J2.i_SystemUserId, eliminado = J2.i_IsDeleted.Value } into J2_join
                             //from J2 in J2_join.DefaultIfEmpty()

                             join J3 in dbContext.systemuser on new { i_InsertUserId = n.i_InsertaIdUsuario.Value, eliminado = 0 }
                                                            equals new { i_InsertUserId = J3.i_SystemUserId, eliminado = J3.i_IsDeleted.Value } into J3_join
                             from J3 in J3_join.DefaultIfEmpty()

                             join J4 in dbContext.documento on new { i_IdTipoDocumento = n.i_IdTipoDocumento.Value, eliminado = 0 }
                                                            equals new { i_IdTipoDocumento = J4.i_CodigoDocumento, eliminado = J4.i_Eliminado.Value } into J4_join
                             from J4 in J4_join.DefaultIfEmpty()

                             join J5 in dbContext.documento on new { i_IdTipoGuia = n.i_IdTipoGuia.Value, eliminado = 0 }
                                                            equals new { i_IdTipoGuia = J5.i_CodigoDocumento, eliminado = J5.i_Eliminado.Value } into J5_join
                             from J5 in J5_join.DefaultIfEmpty()

                             join J6 in dbContext.almacen on new { almacenDestino = n.i_IdAlmacenDestino ?? -1, eliminado = 0 } equals new { almacenDestino = J6.i_IdAlmacen, eliminado = J6.i_Eliminado.Value } into J6_join
                             from J6 in J6_join.DefaultIfEmpty()

                             join J7 in dbContext.establecimiento on new { est = n.i_IdEstablecimiento.Value, eliminado = 0 } equals new { est = J7.i_IdEstablecimiento, eliminado = J7.i_Eliminado.Value } into J7_join
                             from J7 in J7_join.DefaultIfEmpty()
                             join J8 in dbContext.datahierarchy on new { Grupo = 158, eliminado = 0, estado = n.i_IdEstado.Value } equals new { Grupo = J8.i_GroupId, eliminado = J8.i_IsDeleted.Value, estado = J8.i_ItemId } into J8_join
                             from J8 in J8_join.DefaultIfEmpty()
                             where n.i_Eliminado == 0 && n.t_FechaEmision >= F_Inicio && n.t_FechaEmision <= F_Fin
                              && (n.i_IdAlmacenDestino == IdAlmacenDestino || IdAlmacenDestino == -1)

                              && (n.i_IdEstado == Estado || Estado == -1)

                             select new
                             {

                                 v_NumeroRegistro = (n.v_Mes.Trim() + "-" + n.v_Correlativo.Trim()).Trim(),
                                 v_UsuarioCreacion = J3 == null ? "" : J3.v_UserName,
                                 SerieGuia = string.IsNullOrEmpty(n.v_SerieGuiaRemision) ? "" : n.v_SerieGuiaRemision.Trim(),
                                 CorrelativoGuia = string.IsNullOrEmpty(n.v_NumeroGuiaRemision) ? "" : n.v_NumeroGuiaRemision.Trim(),
                                 SerieDocReferencia = string.IsNullOrEmpty(n.v_SerieDocumentoRef) ? "" : n.v_SerieDocumentoRef.Trim(),
                                 CorrelativoDocRef = string.IsNullOrEmpty(n.v_NumeroDocumentoRef) ? "" : n.v_NumeroDocumentoRef.Trim(),
                                 TipoDocumentoReferencia = J4 == null ? "" : J4.v_Siglas,
                                 TipoDocumento = J5 == null ? "" : J5.v_Siglas,
                                 Estado = J8 == null ? "" : J8.v_Value1,
                                 AlmacenDestino = J6 == null ? "" : J6.v_Nombre,
                                 Establecimiento = J7 == null ? "" : J7.v_Nombre,
                                 FechaEmision = n.t_InsertaFecha.Value,


                             }
                            ).ToList().Select(o => new guiaremisionDto
                            {

                                AlmacenDestino = o.AlmacenDestino,
                                Detalles = "ORIGEN : " + o.Establecimiento + " , DOCUMENTO: " + o.TipoDocumento + o.SerieGuia + "-" + o.CorrelativoGuia + " , DOCUMENTO REF. :" + o.TipoDocumentoReferencia + " " + o.SerieDocReferencia + "-" + o.CorrelativoDocRef + " , ESTADO : " + o.Estado + " , FECHA EMISIÓN :" + o.FechaEmision.ToShortDateString() + " , USUARIO CREACION : " + o.v_UsuarioCreacion,
                            }).ToList();


                List<guiaremisionDto> objData = query.ToList();


                pobjOperationResult.Success = 1;
                return objData;
            }
            catch (Exception ex)
            {
                dynamic error = ex;
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = error.InnerException.ErrorSql;
                return null;
            }
        }


        public void EliminarTemporal()
        {
            using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                var query = (from a in dbContext.temporalventadetalle
                             select a);
                foreach (var a in query)
                {
                    dbContext.temporalventadetalle.DeleteObject(a);
                }
                dbContext.SaveChanges();
                ts.Complete();
            }
        }

        public List<GridKeyValueDTO> ObtenerDocumentosParaComboGridGuiaRemision(ref OperationResult pobjOperationResult, string pstrSortExpression, string Filter)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = from a in dbContext.documento
                            where a.i_Eliminado == 0
                             && a.i_UsadoVentas == 1
                            select a;

                if (!string.IsNullOrEmpty(Filter))
                {

                    query = query.Where(Filter);
                }

                if (!string.IsNullOrEmpty(pstrSortExpression))
                {
                    query = query.OrderBy(pstrSortExpression);
                }
                else
                {
                    query = query.OrderBy("i_CodigoDocumento");
                }

                var query2 = query.AsEnumerable()
                            .Select(x => new GridKeyValueDTO
                            {
                                Id = x.i_CodigoDocumento.ToString(),
                                Value1 = x.v_Nombre,
                                Value2 = x.v_Siglas
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




        public void EliminarGuiaRemision(ref OperationResult pobjOperationResult, string pstrIdGuiaRemision, List<string> ClientSession)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                    DocumentoBL _objDocumentoBL = new DocumentoBL();
                    MovimientoBL _objMovimientoBL = new MovimientoBL();
                    OperationResult objOperationResult = new OperationResult();
                    #region Elimina Cabecera
                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.guiaremision
                                           where a.v_IdGuiaRemision == pstrIdGuiaRemision
                                           select a).FirstOrDefault();

                    // Crear la entidad con los datos actualizados
                    objEntitySource.t_ActualizaFecha = DateTime.Now;
                    objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntitySource.i_Eliminado = 1;
                    #endregion

                    #region Elimina Detalles
                    //Eliminar detalles del movimiento eliminado.
                    var objEntitySourceDetallesGuiaRemision = (from a in dbContext.guiaremisiondetalle
                                                               where a.v_IdGuiaRemision == pstrIdGuiaRemision
                                                               select a).ToList();

                    foreach (var GuiaRemisionDetalle in objEntitySourceDetallesGuiaRemision)
                    {
                        GuiaRemisionDetalle.t_ActualizaFecha = DateTime.Now;
                        GuiaRemisionDetalle.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        GuiaRemisionDetalle.i_Eliminado = 1;
                        Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "guiaremisiondetalle", GuiaRemisionDetalle.v_IdGuiaRemisionDetalle);
                    }
                    #endregion

                    #region EliminaNotaSalida
                    if (_objDocumentoBL.DocumentoGeneraStock(objEntitySource.i_IdTipoGuia.Value))
                    {

                        var NotaSalida = (from a in dbContext.movimiento

                                          where a.v_OrigenTipo == Constants.OrigenGuiaInterna && a.v_OrigenRegPeriodo == objEntitySource.v_Periodo && a.v_OrigenRegMes == objEntitySource.v_Mes.Trim() && a.v_OrigenRegCorrelativo == objEntitySource.v_Correlativo.Trim()
                                          && a.i_Eliminado == 0

                                          select new

                                          {
                                              IdMovimiento = a.v_IdMovimiento
                                          }).ToList();
                        foreach (var item in NotaSalida)
                        {
                            if (item != null && item.IdMovimiento != null)
                            {

                                _objMovimientoBL.EliminarMovimiento(ref objOperationResult, item.IdMovimiento, Globals.ClientSession.GetAsList());

                            }
                        }


                    }

                    #endregion



                    #region Actualiza Cabecera Venta
                    var venta =
                        dbContext.venta.FirstOrDefault(
                            p =>
                                p.i_IdTipoDocumento == objEntitySource.i_IdTipoDocumento.Value &&
                                p.v_SerieDocumento == objEntitySource.v_SerieDocumentoRef &&
                                p.v_CorrelativoDocumento == objEntitySource.v_NumeroDocumentoRef.Trim() && p.i_Eliminado == 0);

                    if (venta != null)
                    {
                        venta.v_NroGuiaRemisionSerie = "";
                        venta.v_NroGuiaRemisionCorrelativo = "";
                        dbContext.venta.ApplyCurrentValues(venta);
                    }





                   

                    #endregion


                    dbContext.SaveChanges();
                    Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "guiaremision", objEntitySource.v_IdGuiaRemision);

                    pobjOperationResult.Success = 1;
                    ts.Complete();
                    return;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "GuiaRemisionBL.EliminarGuiaRemision()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }

        #endregion
        #region REPORTE
        public List<ReporteDocumentoGuiaRemision> ReporteDocumentoGuiaRemision(string pstrIdGuiaRemision)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    string FechaReferencia = "";
                    List<ReporteDocumentoGuiaRemision> ReporteFinal = new List<ReporteDocumentoGuiaRemision>();
                    var objEntity = (from a in dbContext.guiaremision
                                     join a1 in dbContext.guiaremisiondetalle on new { IdGuia = a.v_IdGuiaRemision, eliminado = 0 }
                                     equals new { IdGuia = a1.v_IdGuiaRemision, eliminado = a1.i_Eliminado.Value }

                                     join a2 in dbContext.productodetalle on new { ProdDet = a1.v_IdProductoDetalle, eliminado = 0 }
                                     equals new { ProdDet = a2.v_IdProductoDetalle, eliminado = a2.i_Eliminado.Value }

                                     join a3 in dbContext.producto on new { Prod = a2.v_IdProducto, eliminado = 0 } equals
                                     new { Prod = a3.v_IdProducto, eliminado = a3.i_Eliminado.Value }

                                     join A in dbContext.transportista on a.v_IdTransportista equals A.v_IdTransportista into A_join
                                     from A in A_join.DefaultIfEmpty()

                                     join B in dbContext.transportistaunidadtransporte on a.v_IdUnidadTransporte equals
                                     B.v_IdUnidadTransporte into B_join
                                     from B in B_join.DefaultIfEmpty()

                                     join C in dbContext.transportistachofer on a.v_IdChofer equals C.v_IdChofer into C_join
                                     from C in C_join.DefaultIfEmpty()

                                     join E in dbContext.agenciatransporte on a.v_IdAgenciaTransporte equals E.v_IdAgenciaTransporte
                                     into E_join
                                     from E in E_join.DefaultIfEmpty()

                                     join F in dbContext.cliente on a.v_IdCliente equals F.v_IdCliente into F_join
                                     from F in F_join.DefaultIfEmpty()

                                     join G in dbContext.datahierarchy on new { a = a1.i_IdUnidadMedida.Value, b = 17 } //Unidad Medida
                                     equals new { a = G.i_ItemId, b = G.i_GroupId } into G_join
                                     from G in G_join.DefaultIfEmpty()

                                     join H in dbContext.datahierarchy on
                                     new { a = a.i_IdMotivoTraslado.Value, eliminado = 0, Grupo = 33 }

                                     equals new { a = H.i_ItemId, eliminado = H.i_IsDeleted.Value, Grupo = H.i_GroupId } into H_join
                                     from H in H_join.DefaultIfEmpty()

                                     join I in dbContext.documento on new { Referencia = a.i_IdTipoDocumento.Value, eliminado = 0 }
                                     equals new { Referencia = I.i_CodigoDocumento, eliminado = I.i_Eliminado.Value } into I_join

                                     from I in I_join.DefaultIfEmpty()


                                     join J in dbContext.venta on new { TipoDoc = a.i_IdTipoDocumento.Value, eliminado = 0, serie = a.v_SerieDocumentoRef.Trim(), correlativo = a.v_NumeroDocumentoRef.Trim() } equals new
                                     {
                                         TipoDoc = J.i_IdTipoDocumento.Value,
                                         eliminado = J.i_Eliminado.Value,
                                         serie = J.v_SerieDocumento.Trim(),
                                         correlativo = J.v_CorrelativoDocumento.Trim()
                                     } into J_join

                                     from J in J_join.DefaultIfEmpty()

                                     join K in dbContext.datahierarchy on
                                     new { Grupo = 23, eliminado = 0, condicVenta = J.i_IdCondicionPago.Value } equals
                                     new { Grupo = K.i_GroupId, eliminado = K.i_IsDeleted.Value, condicVenta = K.i_ItemId } into K_join

                                     from K in K_join.DefaultIfEmpty()

                                     join M in dbContext.documento on new { td = a.i_IdTipoGuia.Value, eliminado = 0 } equals
                                     new { td = M.i_CodigoDocumento, eliminado = M.i_Eliminado.Value } into M_join

                                     from M in M_join.DefaultIfEmpty()

                                     where a.v_IdGuiaRemision == pstrIdGuiaRemision
                                     /*&& a.i_Eliminado == 0 && a1.i_Eliminado == 0 && a2.i_Eliminado == 0 && A.i_Eliminado == 0 && B.i_Eliminado == 0 && C.i_Eliminado == 0 && E.i_Eliminado == 0 && F.i_Eliminado == 0 && G.i_IsDeleted == 0*/
                                     orderby a3.v_CodInterno ascending
                                     select new ReporteDocumentoGuiaRemision
                                     {
                                         Fecha = a.t_FechaEmision.Value,
                                         RazonSocialCliente =
                                             F != null
                                                 ? (F.v_ApePaterno + " " + F.v_ApeMaterno + " " + F.v_PrimerNombre + " " +
                                                    F.v_SegundoNombre + " " + F.v_RazonSocial).Trim()
                                                 : "",
                                         //  n.cliente != null ? (n.cliente.v_ApePaterno + " " + n.cliente.v_ApeMaterno + " " + n.cliente.v_PrimerNombre + " " + n.cliente.v_SegundoNombre + " " + n.cliente.v_RazonSocial).Trim() : "",
                                         DireccionCliente = F.v_DirecPrincipal,
                                         RucCliente = F.v_NroDocIdentificacion,
                                         ChoferTransportista = C.v_NombreCompleto,
                                         PesoTransportista = "",
                                         CIMTCTransportista = B.v_TractoCertificado,
                                         PlacaTransportista = B.v_TractoPlaca,
                                         RazonSocialTransportista = A.v_NombreRazonSocial,
                                         RucTransportista = A.v_NumeroDocumento,
                                         MarcaTransportista = B.v_TractoMarca,
                                         IdMotivoTraslado = a.i_IdMotivoTraslado.Value,
                                         Cantidad = a1.d_Cantidad.Value,
                                         UnidadMedida = G == null ? "" : G.v_Value1,
                                         Codigo = a3.v_CodInterno,
                                         Descripcion = a3.v_Descripcion,
                                         PuntoPartida = a.v_PuntoPartida,
                                         PuntoLlegada = a.v_PuntoLLegada,
                                         DocumentoReferencia =
                                             I != null ? I.v_Siglas + " " + a.v_SerieDocumentoRef + "-" + a.v_NumeroDocumentoRef : "",
                                         Licencia = C.v_Brevete,
                                         FechaTraslado = a.t_FechaTraslado.Value,
                                         MotivoTraslado = H.v_Value1,
                                         TipoDocReferencia = a.i_IdTipoDocumento.Value,
                                         NumeroGuiaRemision = a.v_SerieGuiaRemision.Trim() + "-" + a.v_NumeroGuiaRemision.Trim(),
                                         SerieGuia = a.v_SerieGuiaRemision.Trim(),
                                         CodigoCliente = F == null ? "" : F.v_CodCliente.Trim(),
                                         CondicionVentas = K == null ? "" : K.v_Value1.Trim(),
                                         DireccionTransportista = A == null ? "" : A.v_Direccion.Trim(),
                                         UnidadMedidaSiglas = G == null ? "" : G.v_Field == null ? "" : G.v_Field,
                                         TipoNumeroGuia =
                                             M == null
                                                 ? ""
                                                 : M.v_Siglas + " " + a.v_SerieGuiaRemision.Trim() + "-" +
                                                   a.v_NumeroGuiaRemision.Trim(),

                                         Precio = a1.d_Precio ?? 0,
                                         SubTotal = a1.d_Total ?? 0,
                                         v_Descuento = a1.v_Descuento,
                                         AgenciaTransportes = E.v_NombreRazonSocial,
                                         SerieReferencia = a.v_SerieDocumentoRef.Trim(),
                                         CorrelativoReferencia = a.v_NumeroDocumentoRef.Trim(),
                                         CantidadBulto = a1.d_CantidadBulto ?? 0,
                                         DocumentoInternoReferencia = I.i_UsadoDocumentoInterno ?? 0,
                                         ObservacionDetalle = a1.v_Observacion,
                                         DireccionAgenciaTransporte = E.v_Direccion,
                                         DocumentoAgenciaTransporte = E.v_NumeroDocumento,
                                         v_IdGuiaRemisionDetalle = a1.v_IdGuiaRemisionDetalle,
                                         Modalidad = a.i_Modalidad ?? 1
                                     }
                    ).ToList();

                    try
                    {
                        if (objEntity.Any())
                        {
                            int TipoDoc = objEntity.First().TipoDocReferencia;
                            string serieRef = objEntity.First().SerieReferencia.Trim();
                            string correlativoRef = objEntity.First().CorrelativoReferencia.Trim();
                            var FechaRef =
                                dbContext.venta.Where(
                                        k =>
                                            k.i_IdTipoDocumento == TipoDoc && k.v_CorrelativoDocumento == correlativoRef &&
                                            k.v_SerieDocumento.Trim() == serieRef && k.i_Eliminado == 0 && k.i_IdEstado == 1)
                                    .FirstOrDefault();

                            FechaReferencia = FechaRef != null ? FechaRef.t_FechaRegistro.Value.ToShortDateString() : "";

                        }
                    }
                    catch (Exception ex)
                    {
                        FechaReferencia = "";
                    }
                    var Final = (from n in objEntity
                                 select new ReporteDocumentoGuiaRemision
                                 {

                                     Fecha = n.Fecha,
                                     RazonSocialCliente = n.RazonSocialCliente,
                                     DireccionCliente = n.DireccionCliente,
                                     RucCliente = n.RucCliente,
                                     ChoferTransportista = n.ChoferTransportista,
                                     PesoTransportista = n.PesoTransportista,
                                     CIMTCTransportista = n.CIMTCTransportista,
                                     PlacaTransportista = n.PlacaTransportista,
                                     RazonSocialTransportista = n.RazonSocialTransportista,
                                     RucTransportista = n.RucTransportista,
                                     MarcaTransportista = n.MarcaTransportista,
                                     IdMotivoTraslado = n.IdMotivoTraslado,
                                     Cantidad = n.Cantidad,
                                     UnidadMedida = n.UnidadMedida,
                                     Codigo = n.Codigo,
                                     Descripcion = n.Descripcion,
                                     PuntoPartida = n.PuntoPartida,
                                     PuntoLlegada = n.PuntoLlegada,
                                     DocumentoReferencia = n.DocumentoReferencia,
                                     Licencia = n.Licencia,
                                     DiaFechaEmision = n.Fecha.Date.Day.ToString("00"),
                                     MesFechaEmision = n.Fecha.Date.Month.ToString("00"),
                                     AnioFechaEmision = n.Fecha.Date.Year.ToString(),
                                     DiaFechaTraslado = n.FechaTraslado.Date.Day.ToString("00"),
                                     MesFechaTraslado = n.FechaTraslado.Date.Month.ToString("00"),
                                     AnioFechaTraslado = n.FechaTraslado.Date.Year.ToString(),
                                     MotivoTraslado = n.MotivoTraslado,
                                     TipoDocReferencia = n.TipoDocReferencia,
                                     NumeroGuiaRemision = n.NumeroGuiaRemision,
                                     Traslado =
                                         n.FechaTraslado.Date.Day.ToString("00") + "/" +
                                         n.FechaTraslado.Date.Month.ToString("00") + "/" + n.FechaTraslado.Date.Year.ToString(),
                                     SerieGuia = n.SerieGuia.Trim(),
                                     CodigoCliente = n.CodigoCliente.Trim(),
                                     CondicionVentas = n.CondicionVentas,
                                     DireccionTransportista = n.DireccionTransportista,
                                     UnidadMedidaSiglas = n.UnidadMedidaSiglas,
                                     TipoNumeroGuia = n.TipoNumeroGuia,
                                     Precio = n.Precio,
                                     SubTotal = n.SubTotal,
                                     v_Descuento = n.v_Descuento,
                                     AgenciaTransportes = n.AgenciaTransportes,
                                     FechaReferencia = FechaReferencia,
                                     CantidadBulto = n.CantidadBulto,
                                     DocumentoInternoReferencia = n.DocumentoInternoReferencia,
                                     ObservacionDetalle = n.ObservacionDetalle,
                                     DireccionAgenciaTransporte = n.DireccionAgenciaTransporte,
                                     DocumentoAgenciaTransporte = n.DocumentoAgenciaTransporte,
                                     v_IdGuiaRemisionDetalle = n.v_IdGuiaRemisionDetalle,
                                     Modalidad = n.Modalidad
                                 }).ToList().OrderBy(o => o.v_IdGuiaRemisionDetalle);

                    int i = 0;
                    foreach (var item in Final)
                    {

                        item.Item = (i + 1).ToString();
                        i = i + 1;
                        ReporteFinal.Add(item);
                    }



                    return ReporteFinal;
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //return null;
            }
        }

        #endregion

        #region Retencion Electronica

        public List<ReporteDocumentoRetencion> ObtenerReporte(ref OperationResult pobjOperationResult, string pstrIdDocumento)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var rep = from r in dbContext.documentoretencion
                              where r.v_IdDocumentoRetencion == pstrIdDocumento
                              join j1 in dbContext.cliente on r.v_IdCliente equals j1.v_IdCliente into j1Join
                              from j1 in j1Join.DefaultIfEmpty()
                              join j2 in dbContext.documentoretenciondetalle on r.v_IdDocumentoRetencion equals j2.v_IdDocumentoRetencion
                              into j2Join
                              from j2 in j2Join.DefaultIfEmpty()
                              join j3 in dbContext.compra on new { j2.i_IdTipoDocumento, j2.v_SerieDocumento, j2.v_CorrelativoDocumento } equals
                              new { j3.i_IdTipoDocumento, j3.v_SerieDocumento, j3.v_CorrelativoDocumento } into j3Join
                              from j3 in j3Join.DefaultIfEmpty()
                              join j4 in dbContext.documento on j3.i_IdTipoDocumento equals j4.i_CodigoDocumento into j4Join
                              from j4 in j4Join.DefaultIfEmpty()
                              select new ReporteDocumentoRetencion
                              {
                                  Documento = r.v_SerieDocumento + "-" + r.v_CorrelativoDocumento,
                                  FechaRegistro = r.t_FechaRegistro ?? DateTime.Now,
                                  NombreCliente = j1.v_PrimerNombre + " " + j1.v_ApePaterno + " " + j1.v_ApeMaterno + " " + j1.v_RazonSocial,
                                  NroDocCliente = j1.v_NroDocIdentificacion,
                                  TipoCambio = r.tesoreria.d_TipoCambio ?? 1,
                                  DocTipo = j4 == null ? "" : j4.v_Siglas,
                                  DocSerie = j2.v_SerieDocumento,
                                  DocCorrelativo = j2.v_CorrelativoDocumento,
                                  DocFecha = j3.t_FechaRegistro ?? DateTime.Now,
                                  DocMoneda = j3.i_IdMoneda == 1 ? "PEN" : "USD",
                                  DocTotal = j3.d_Total ?? 0,
                                  DocRetenido = j2.d_MontoRetenido ?? 0,
                              };
                    var res = rep.ToList();
                    pobjOperationResult.Success = 1;
                    return res;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }

        }

        /// <summary>
        /// Obtiene el Archivo Zip segun la Operacion.
        /// </summary>
        /// <param name="pobjOperationResult">Resultado de la Operacion</param>
        /// <param name="pstrIdGuia">Id de la guia</param>
        public byte[] GetZipContent(ref OperationResult pobjOperationResult, string pstrIdGuia)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var bFile = (from n in dbContext.guiaremisionhomologacion
                                 where n.v_IdGuiaRemision == pstrIdGuia
                                 select n.b_FileXml).First();

                    pobjOperationResult.Success = 1;
                    return bFile;
                }
            }
            catch (Exception er)
            {
                pobjOperationResult.ExceptionMessage = er.Message;
                pobjOperationResult.AdditionalInformation = "DocumentoRetencionBL.GetZipFile";
                return null;
            }
        }
        #endregion
    }
}
