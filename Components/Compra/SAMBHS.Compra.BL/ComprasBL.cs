using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using SAMBHS.ActivoFijo.BL;
using SAMBHS.Almacen.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Tesoreria.BL;
using SAMBHS.Venta.BL;
using SAMBHS.Common.BE.Custom;

namespace SAMBHS.Compra.BL
{
    public class ComprasBL
    {
        private readonly DocumentoBL _objDocumentoBL = new DocumentoBL();
        private string periodo = Globals.ClientSession.i_Periodo.Value.ToString();
        public List<KeyValueDTO> ObtenerListadoCompras(ref OperationResult pobjOperationResult, string pstringPeriodo,
            string pstringMes)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var idAlmacen = Globals.ClientSession.i_IdAlmacenPredeterminado.Value.ToString("00");

                var replicationID = Globals.ClientSession.ReplicationNodeID;
                var query = from n in dbContext.compra
                            where
                            n.i_Eliminado == 0 && n.v_Periodo == pstringPeriodo && n.v_Mes == pstringMes &&
                            n.v_IdCompra.Substring(2, 2) == idAlmacen
                            && n.v_IdCompra.Substring(0, 1) == replicationID
                            orderby n.v_Correlativo ascending
                            select new
                            {
                                n.v_Correlativo,
                                n.v_IdCompra
                            };

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
                return new List<KeyValueDTO> { new KeyValueDTO { Value1 = idAlmacen + "000000" } };
            }
        }

        public compraDto ObtenerCompraCabecera(ref OperationResult pobjOperationResult, string pstrIdCompra)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var objEntity = (from a in dbContext.compra

                                     join A in dbContext.cliente on a.v_IdProveedor equals A.v_IdCliente into A_join
                                     from A in A_join.DefaultIfEmpty()

                                     join B in dbContext.datahierarchy on new { a = a.i_CodigoDetraccion.Value, b = 29 }
                                         equals new { a = B.i_ItemId, b = B.i_GroupId } into B_join
                                     from B in B_join.DefaultIfEmpty()


                                     join c in dbContext.pagopendiente on
                                             new { idCompra = a.v_IdCompra, eliminado = 0 } equals
                                             new { idCompra = c.v_IdCompra, eliminado = c.i_Eliminado.Value } into c_join
                                     from c in c_join.DefaultIfEmpty()



                                     where a.v_IdCompra == pstrIdCompra
                                     select new compraDto
                                     {
                                         v_IdCompra = a.v_IdCompra,
                                         v_Periodo = a.v_Periodo,
                                         v_Mes = a.v_Mes,
                                         v_Correlativo = a.v_Correlativo,
                                         i_IdIgv = a.i_IdIgv,
                                         i_IdTipoDocumento = a.i_IdTipoDocumento,
                                         v_SerieDocumento = a.v_SerieDocumento,
                                         v_CorrelativoDocumento = a.v_CorrelativoDocumento,
                                         i_IdTipoDocumentoRef = a.i_IdTipoDocumentoRef,
                                         v_SerieDocumentoRef = a.v_SerieDocumentoRef,
                                         v_CorrelativoDocumentoRef = a.v_CorrelativoDocumentoRef,
                                         t_FechaRef = a.t_FechaRef,
                                         v_IdProveedor = a.v_IdProveedor,
                                         t_FechaRegistro = a.t_FechaRegistro,
                                         t_FechaEmision = a.t_FechaEmision,
                                         d_TipoCambio = a.d_TipoCambio,
                                         t_FechaVencimiento = a.t_FechaVencimiento,
                                         i_IdCondicionPago = a.i_IdCondicionPago,
                                         v_Glosa = a.v_Glosa,
                                         i_EsDetraccion = a.i_EsDetraccion,
                                         i_CodigoDetraccion = a.i_CodigoDetraccion,
                                         d_PorcentajeDetraccion = a.d_PorcentajeDetraccion,
                                         v_NroDetraccion = a.v_NroDetraccion,
                                         t_FechaDetraccion = a.t_FechaDetraccion,
                                         i_EsAfectoIgv = a.i_EsAfectoIgv,
                                         i_PreciosIncluyenIgv = a.i_PreciosIncluyenIgv,
                                         i_DeduccionAnticipio = a.i_DeduccionAnticipio,
                                         i_IdMoneda = a.i_IdMoneda,
                                         i_IdEstado = a.i_IdEstado,
                                         i_IdTipoCompra = a.i_IdTipoCompra,
                                         i_IdDestino = a.i_IdDestino,
                                         i_IdEstablecimiento = a.i_IdEstablecimiento,
                                         d_Anticipio = a.d_Anticipio,
                                         d_ValorVenta = a.d_ValorVenta,
                                         d_IGV = a.d_IGV,
                                         d_Total = a.d_Total,
                                         i_Eliminado = a.i_Eliminado,
                                         i_InsertaIdUsuario = a.i_InsertaIdUsuario,
                                         t_InsertaFecha = a.t_InsertaFecha,
                                         i_ActualizaIdUsuario = a.i_ActualizaIdUsuario,
                                         t_ActualizaFecha = a.t_ActualizaFecha,
                                         NombreProveedor =
                                             (A.v_ApePaterno + " " + A.v_ApeMaterno + " " + A.v_PrimerNombre + " " + A.v_SegundoNombre + " " + A.v_RazonSocial)
                                                 .Trim(),
                                         CodigoProveedor = A.v_CodCliente,
                                         RUCProveedor = A.v_NroDocIdentificacion,
                                         NombreDetraccion = B.v_Value1,
                                         v_GuiaRemisionCorrelativo = a.v_GuiaRemisionCorrelativo,
                                         v_GuiaRemisionSerie = a.v_GuiaRemisionSerie,
                                         v_IdDocumentoReferencia = a.v_IdDocumentoReferencia,
                                         v_ODCCorrelativo = a.v_ODCCorrelativo,
                                         v_ODCSerie = a.v_ODCSerie,
                                         Saldo = c != null ? c.d_Saldo ?? 0 : 0,
                                         v_SeriePercepcion = a.v_SeriePercepcion,
                                         v_CorrelativoPercepcion = a.v_CorrelativoPercepcion,
                                         v_DocumentoPercepcion = a.v_DocumentoPercepcion,
                                         i_AplicaRetencion = a.i_AplicaRetencion,
                                         t_FechaPercepcion = a.t_FechaPercepcion,
                                         d_PorcentajePercepcion = a.d_PorcentajePercepcion,
                                         d_ImporteCalculoPercepcion = a.d_ImporteCalculoPercepcion,
                                         d_Percepcion = a.d_Percepcion,
                                         i_IdTipoDocumentoPercepcion = a.i_IdTipoDocumentoPercepcion,
                                         i_AplicaRectificacion = a.i_AplicaRectificacion ?? 0,
                                         t_FechaCorreccionPle = a.t_FechaCorreccionPle,

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

        public BindingList<compradetalleDto> ObtenerCompraDetalles(ref OperationResult pobjOperationResult, string pstrIdCompra)
        {
            try
            {
                var dbContext = new SAMBHSEntitiesModelWin();

                var query = (from n in dbContext.compradetalle
                             join a in dbContext.cliente on n.v_IdAnexo equals a.v_IdCliente into aJoin
                             from a in aJoin.DefaultIfEmpty()
                             join b in dbContext.productodetalle on new { pd = n.v_IdProductoDetalle, eliminado = 0 } equals new { pd = b.v_IdProductoDetalle, eliminado = b.i_Eliminado.Value } into b_join
                             from b in b_join.DefaultIfEmpty()
                             join c in dbContext.producto on new { pd = b.v_IdProducto, eliminado = 0 } equals new { pd = c.v_IdProducto, eliminado = c.i_Eliminado.Value } into c_join
                             from c in c_join.DefaultIfEmpty()
                             where n.i_Eliminado == 0 && n.v_IdCompra == pstrIdCompra
                             orderby n.t_InsertaFecha ascending
                             select new compradetalleDto
                             {
                                 v_IdCompraDetalle = n.v_IdCompraDetalle,
                                 v_IdCompra = n.v_IdCompra,
                                 v_IdMovimientoDetalle = n.v_IdMovimientoDetalle,
                                 v_NroCuenta = n.v_NroCuenta,
                                 i_Anticipio = n.i_Anticipio,
                                 v_IdProductoDetalle = n.v_IdProductoDetalle,
                                 i_IdAlmacen = n.i_IdAlmacen,
                                 d_Cantidad = n.d_Cantidad,
                                 d_CantidadEmpaque = n.d_CantidadEmpaque,
                                 i_IdUnidadMedida = n.i_IdUnidadMedida,
                                 d_Precio = n.d_Precio,
                                 d_ValorVenta = n.d_ValorVenta,
                                 d_Igv = n.d_Igv,
                                 d_PrecioVenta = n.d_PrecioVenta,
                                 d_isc = n.d_isc,
                                 d_otrostributos = n.d_otrostributos,
                                 i_IdDestino = n.i_IdDestino,
                                 i_IdCentroCostos = n.i_IdCentroCostos,
                                 v_NroGuiaRemision = n.v_NroGuiaRemision,
                                 d_ValorSolesDetraccion = n.d_ValorSolesDetraccion,
                                 d_ValorDolaresDetraccion = n.d_ValorDolaresDetraccion,
                                 v_NroPedido = n.v_NroPedido,
                                 v_Glosa = n.v_Glosa,
                                 i_Eliminado = n.i_Eliminado,
                                 i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                 t_InsertaFecha = n.t_InsertaFecha,
                                 i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                 t_ActualizaFecha = n.t_ActualizaFecha,
                                 d_DescuentoItem = n.d_DescuentoItem,
                                 v_DescuentoItem = n.v_DescuentoItem,
                                 v_IdAnexo = n.v_IdAnexo,
                                 IdAnexo = a != null ? (a.v_ApePaterno + " " + a.v_ApeMaterno + " " + a.v_PrimerNombre + " " + a.v_RazonSocial).Trim() : string.Empty,
                                 v_NroLote = n.v_NroLote,
                                 v_NroSerie = n.v_NroSerie,
                                 i_SolicitarNroLoteIngreso = c.i_SolicitarNroLoteIngreso ?? 0,
                                 i_SolicitarNroSerieIngreso = c.i_SolicitarNroSerieIngreso ?? 0,
                                 i_SolicitaOrdenProduccionIngreso = c.i_SolicitaOrdenProduccionIngreso ?? 0,
                                 t_FechaCaducidad = n.t_FechaCaducidad,
                                 t_FechaFabricacion = n.t_FechaFabricacion,

                             }).ToList();

                pobjOperationResult.Success = 1;

                return new BindingList<compradetalleDto>(query);
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public List<compradetalleDtoShort> DevolverNombresAMC(string pstrIdCompra)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var query = (from n in dbContext.compradetalle
                             join A in dbContext.productodetalle on n.v_IdProductoDetalle equals A.v_IdProductoDetalle
                             join B in dbContext.producto on A.v_IdProducto equals B.v_IdProducto
                             join C in dbContext.asientocontable on new { c = n.v_NroCuenta, p = periodo, eliminado = 0 } equals new { c = C.v_NroCuenta, p = C.v_Periodo, eliminado = C.i_Eliminado.Value }
                             join D in dbContext.almacen on n.i_IdAlmacen equals D.i_IdAlmacen
                             join J1 in dbContext.datahierarchy on new { a = B.i_IdUnidadMedida.Value, b = 17 }
                             equals new { a = J1.i_ItemId, b = J1.i_GroupId } into J1_join
                             from J1 in J1_join.DefaultIfEmpty()
                             join J2 in dbContext.datahierarchy on new { a = n.i_IdDestino.Value, b = 24 }
                             equals new { a = J2.i_ItemId, b = J2.i_GroupId } into J2_join
                             from J2 in J2_join.DefaultIfEmpty()
                             join J3 in dbContext.datahierarchy on new { a = n.i_IdCentroCostos, b = 31 }
                             equals new { a = J3.v_Value2, b = J3.i_GroupId } into J3_join
                             from J3 in J3_join.DefaultIfEmpty()
                             where n.v_IdCompra == pstrIdCompra
                             select new compradetalleDtoShort
                             {
                                 Nombre = B.v_Descripcion,
                                 CodigoInterno = B.v_CodInterno,
                                 Empaque = B.d_Empaque,
                                 UMEmpaque = J1.v_Value1,
                                 NombreCuenta = C.v_NombreCuenta,
                                 NombreDestino = J2.v_Value1,
                                 NombreCentroCostos = J3.v_Value1,
                                 v_Nombre = D.v_Nombre
                             }).ToList();

                return query;
            }
        }

        public string[] DevolverNombres(string pstringIdProductoDetalle, string pstringNroCuenta, int pintIdDestino,
            string pintIdCentroCosto, int pintIdAlmacen)
        {
            var dbContext = new SAMBHSEntitiesModelWin();
            var EntityProducto = (from n in dbContext.productodetalle
                                  join A in dbContext.producto on n.v_IdProducto equals A.v_IdProducto into A_join
                                  from A in A_join.DefaultIfEmpty()
                                  join B in dbContext.producto on n.v_IdProducto equals B.v_IdProducto into B_join
                                  from B in B_join.DefaultIfEmpty()
                                  join J1 in dbContext.datahierarchy on new { a = B.i_IdUnidadMedida.Value, b = 17 }
                                  equals new { a = J1.i_ItemId, b = J1.i_GroupId } into J1_join
                                  from J1 in J1_join.DefaultIfEmpty()
                                  where n.v_IdProductoDetalle == pstringIdProductoDetalle
                                  select new
                                  {
                                      Nombre = A.v_Descripcion,
                                      CodigoInterno = A.v_CodInterno,
                                      Empaque = B.d_Empaque,
                                      UMEmpaque = J1.v_Value1,
                                      EsServicio = A.i_EsServicio,
                                      UnidadMedida = A.i_IdUnidadMedida,
                                      EsActivoFijo = A.i_EsActivoFijo,
                                      i_SolicitarNroSerie = A.i_SolicitarNroSerieIngreso ?? 0,
                                      i_SolicitarNroLote = A.i_SolicitarNroLoteIngreso ?? 0,
                                  }
            ).FirstOrDefault();

            var EntityCuenta = (from n in dbContext.asientocontable
                                join A in dbContext.compradetalle on n.v_NroCuenta equals A.v_NroCuenta into A_join
                                from A in A_join.DefaultIfEmpty()
                                where n.v_NroCuenta == pstringNroCuenta && n.v_Periodo == periodo && n.i_Eliminado == 0
                                select new
                                {
                                    NombreCuenta = n.v_NombreCuenta
                                }).FirstOrDefault();

            var EntityDestino = (from n in dbContext.datahierarchy
                                 join A in dbContext.compradetalle on n.i_ItemId equals A.i_IdDestino into A_join
                                 from A in A_join.DefaultIfEmpty()
                                 where n.i_GroupId == 24 && n.i_ItemId == pintIdDestino
                                 select new
                                 {
                                     NombreDestino = n.v_Value1
                                 }).FirstOrDefault();

            var EntityCentroCostos = (from n in dbContext.datahierarchy
                                      join A in dbContext.compradetalle on n.v_Value2 equals A.i_IdCentroCostos into A_join
                                      from A in A_join.DefaultIfEmpty()
                                      where n.i_GroupId == 31 && n.v_Value2 == pintIdCentroCosto
                                      select new
                                      {
                                          NombreCentroCostos = n.v_Value1
                                      }).FirstOrDefault();

            var EntityAlmacen = (from a in dbContext.nodewarehouse
                                 join J1 in dbContext.almacen on new { AlmacenID = a.i_NodeWarehouseId }
                                 equals new { AlmacenID = J1.i_IdAlmacen } into J1_join
                                 from J1 in J1_join.DefaultIfEmpty()
                                 where a.i_IsDeleted == 0 && a.i_NodeWarehouseId == pintIdAlmacen
                                 select new
                                 {
                                     J1.v_Nombre
                                 }).FirstOrDefault();

            var Cadena = new string[13];
            if (EntityProducto != null)
            {
                Cadena[0] = EntityProducto.CodigoInterno;
                Cadena[1] = EntityProducto.Nombre;
                Cadena[2] = EntityProducto.Empaque.ToString();
                Cadena[3] = EntityProducto.UMEmpaque;
                Cadena[8] = EntityProducto.EsServicio.Value.ToString();
                Cadena[9] = EntityProducto.UnidadMedida.ToString();
                Cadena[10] = EntityProducto.EsActivoFijo.ToString();
                Cadena[11] = EntityProducto.i_SolicitarNroLote.ToString();
                Cadena[12] = EntityProducto.i_SolicitarNroSerie.ToString();
            }

            Cadena[4] = EntityCuenta != null ? EntityCuenta.NombreCuenta : string.Empty;

            Cadena[5] = EntityDestino != null ? EntityDestino.NombreDestino : string.Empty;

            Cadena[6] = EntityCentroCostos != null ? EntityCentroCostos.NombreCentroCostos : string.Empty;

            Cadena[7] = EntityAlmacen != null ? EntityAlmacen.v_Nombre : string.Empty;

            return Cadena;
        }

        public string InsertarCompra(ref OperationResult pobjOperationResult, compraDto pobjDtoEntity,
            List<string> ClientSession, List<compradetalleDto> pTemp_Insertar, string[] RegistroDiario)
        {
            var xx = new string[2];
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    var dbContext = new SAMBHSEntitiesModelWin();
                    var objSecuentialBL = new SecuentialBL();
                    var objEntityCompra = pobjDtoEntity.ToEntity();
                    var SecuentialId = 0;
                    var newIdCompra = string.Empty;
                    var newIdCompraDetalle = string.Empty;

                    #region Inserta Cabecera

                    var idTipoDoc = pobjDtoEntity.i_IdTipoDocumento ?? -1;
                    var serie = pobjDtoEntity.v_SerieDocumento.Trim();
                    var corr = pobjDtoEntity.v_CorrelativoDocumento.Trim();
                    var prov = pobjDtoEntity.v_IdProveedor;

                    var yaExiste = dbContext.compra.Any(p =>
                                p.i_Eliminado == 0 && p.i_IdTipoDocumento == idTipoDoc && p.v_SerieDocumento.Trim().Equals(serie) &&
                                p.v_CorrelativoDocumento.Trim().Equals(corr) && p.v_IdProveedor.Equals(prov));

                    if (yaExiste) throw new Exception("Documento ingresado anteriormente!");

                    objEntityCompra.t_InsertaFecha = DateTime.Now;
                    objEntityCompra.i_InsertaIdUsuario = int.Parse(ClientSession[2]);
                    objEntityCompra.i_Eliminado = 0;
                    xx = new string[2];
                    xx[0] = objEntityCompra.v_SerieDocumentoRef;
                    xx[1] = objEntityCompra.v_CorrelativoDocumentoRef;
                    var intNodeId = int.Parse(ClientSession[0]);
                    SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 31);
                    newIdCompra = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "ZZ");
                    objEntityCompra.v_IdCompra = newIdCompra;
                    dbContext.AddTocompra(objEntityCompra);

                    #endregion

                    #region Inserta Detalle

                    foreach (var compradetalleDto in pTemp_Insertar)
                    {
                        var NotadeIngresoDetalle = (from n in dbContext.movimientodetalle
                                                    join J1 in dbContext.movimiento on
                                                        new { idMovimiento = n.v_IdMovimiento, IdAlmacen = compradetalleDto.i_IdAlmacen }
                                                        equals new { idMovimiento = J1.v_IdMovimiento, IdAlmacen = J1.i_IdAlmacenOrigen } into
                                                        J1_join
                                                    from J1 in J1_join.DefaultIfEmpty()
                                                    where
                                                        n.i_Eliminado == 0 && n.v_IdProductoDetalle == compradetalleDto.v_IdProductoDetalle &&
                                                        J1.v_OrigenTipo == "C" && J1.v_OrigenRegPeriodo == objEntityCompra.v_Periodo
                                                        && J1.v_OrigenRegMes == objEntityCompra.v_Mes &&
                                                        J1.v_OrigenRegCorrelativo == objEntityCompra.v_Correlativo
                                                    select new { n.v_IdMovimientoDetalle }).FirstOrDefault();

                        var objEntityCompraDetalle = compradetalleDto.ToEntity();
                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 32);
                        newIdCompraDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "ZY");
                        objEntityCompraDetalle.v_IdCompraDetalle = newIdCompraDetalle;
                        objEntityCompraDetalle.v_IdCompra = newIdCompra;
                        objEntityCompraDetalle.v_IdMovimientoDetalle = NotadeIngresoDetalle != null
                            ? NotadeIngresoDetalle.v_IdMovimientoDetalle
                            : null;
                        objEntityCompraDetalle.t_InsertaFecha = DateTime.Now;
                        objEntityCompraDetalle.i_InsertaIdUsuario = int.Parse(ClientSession[2]);
                        objEntityCompraDetalle.i_Eliminado = 0;
                        dbContext.AddTocompradetalle(objEntityCompraDetalle);
                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName,
                            "compradetalle", newIdCompraDetalle);
                        if (compradetalleDto.EsActivoFijo == 1)
                        {
                            ActualizarActivoFijo(pobjDtoEntity, compradetalleDto, ClientSession);
                        }
                    }

                    dbContext.SaveChanges();
                    Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "compra",
                        newIdCompra);

                    #endregion

                    #region Genera Pagos Pendientes

                    if (newIdCompra != null)
                    {
                        if (pobjDtoEntity.i_IdEstado == 1)
                        {
                            InsertaPagosPendiente(ref pobjOperationResult, newIdCompra, pobjDtoEntity.d_Total.Value,
                                Globals.ClientSession.GetAsList());
                            if (pobjOperationResult.Success == 0) return null;
                        }
                    }

                    #endregion

                    #region Asiento Contable
                    GenerarAsientoContable(ref pobjOperationResult, newIdCompra);
                    if (pobjOperationResult.Success == 0) return null;
                    #endregion

                    #region Cambia Estado de la Orden de Compra si es Necesario

                    if (pTemp_Insertar.Any(p => p._DesdeODC))
                    {
                        var SerieODC = pobjDtoEntity.v_ODCSerie;
                        var CorrelativoODC = pobjDtoEntity.v_ODCCorrelativo;

                        var ODC = (from n in dbContext.ordendecompra
                                   where n.v_SerieDocumento == SerieODC && n.v_CorrelativoDocumento == CorrelativoODC
                                   select n).FirstOrDefault();

                        if (ODC != null)
                        {
                            var OrdenCompraDetalles =
                                ODC.ordendecompradetalle.Where(p => p.v_IdOrdenCompra == ODC.v_IdOrdenCompra).ToList();
                            var Counter = 0;
                            foreach (var CompraDetalle in pTemp_Insertar.Where(p => p._DesdeODC))
                            {
                                var ODCDetalle =
                                    OrdenCompraDetalles.Where(
                                        p => p.v_IdProductoDetalle == CompraDetalle.v_IdProductoDetalle)
                                        .FirstOrDefault();
                                ODCDetalle.i_UsadoEnCompra = 1;
                                dbContext.ordendecompradetalle.ApplyCurrentValues(ODCDetalle);
                                Counter++;
                            }

                            if (OrdenCompraDetalles.Where(p => p.i_UsadoEnCompra != 1).Count() == 0)
                            {
                                ODC.i_IdEstado = 2; //Se le cambia el estado de la orden de compra a Procesado. 
                                dbContext.ordendecompra.ApplyCurrentValues(ODC);
                            }

                            dbContext.SaveChanges();
                        }
                        else
                        {
                            pobjOperationResult.Success = 0;
                            pobjOperationResult.ErrorMessage = "La orden de compra ya no existe!";
                            return null;
                        }
                    }

                    #endregion

                    #region Genera el ingreso a almacén

                    GenerarIngresoAlmacen(ref pobjOperationResult, newIdCompra, "", "", "");
                    if (pobjOperationResult.Success == 0) return null;

                    #endregion

                    pobjOperationResult.Success = 1;
                    ts.Complete();
                    return newIdCompra;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ComprasBL.InsertarCompra()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        private void ActualizarActivoFijo(compraDto objCompra, compradetalleDto objCompraDetalle,
            List<string> ClientSession)
        {
            var pTemp_Insertar = new List<activofijodetalleDto>();
            var pTemp_Editar = new List<activofijodetalleDto>();
            var pTemp_Eliminar = new List<activofijodetalleDto>();
            var pTemp_InsertarAnexo = new List<activofijoanexoDto>();
            var pTemp_EditarAnexo = new List<activofijoanexoDto>();
            var pTemp_EliminarAnexo = new List<activofijoanexoDto>();
            ActivoFijoBL _objActivoFijoBL = new ActivoFijoBL();
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var pobjOperationResult = new OperationResult();
                var activoDto = (from n in dbContext.activofijo
                                 where n.i_Eliminado == 0 && n.v_IdProducto == objCompraDetalle.v_IdProductoDetalle
                                 select n).FirstOrDefault();


                var objActivoDto = activoDto.ToDTO();
                if (objActivoDto != null)
                {

                    while (_objActivoFijoBL.ExisteNroCorrelativo(objActivoDto.v_CodigoActivoFijo.Trim()))
                    {
                        objActivoDto.v_CodigoActivoFijo = (int.Parse(objActivoDto.v_CodigoActivoFijo) + 1).ToString("00000000");

                    }

                    var Producto = (from a in dbContext.productodetalle
                                    where a.v_IdProductoDetalle == objCompraDetalle.v_IdProductoDetalle

                                    select a.producto).FirstOrDefault();
                    producto ProductoCambiar = Producto;
                    ProductoCambiar.v_CodInterno = objActivoDto.v_CodigoActivoFijo;
                    dbContext.producto.ApplyCurrentValues(ProductoCambiar);
                    dbContext.SaveChanges();
                    objActivoDto.v_IdCliente = objCompra.v_IdProveedor;
                    objActivoDto.i_IdTipoDocumento = objCompra.i_IdTipoDocumento;
                    objActivoDto.t_FechaOrdenCompra = objCompra.t_FechaRegistro.Value;
                    objActivoDto.v_NumeroFactura = objCompra.v_SerieDocumento.Trim() + "-" +
                                                   objCompra.v_CorrelativoDocumento.Trim();
                    objActivoDto.t_FechaFactura = objCompra.t_FechaEmision.Value;
                    objActivoDto.d_MonedaNacional = objCompra.i_IdMoneda == (int)Currency.Soles
                        ? objCompraDetalle.d_ValorVenta
                        : objCompraDetalle.d_ValorVenta * objCompra.d_TipoCambio;
                    objActivoDto.d_MonedaExtranjera = objCompra.i_IdMoneda == (int)Currency.Dolares
                        ? objCompraDetalle.d_ValorVenta
                        : objCompra.d_TipoCambio == 0 ? 0 : objCompraDetalle.d_ValorVenta / objCompra.d_TipoCambio;
                    objActivoDto.t_FechaUso = objCompra.t_FechaRegistro.Value;
                    objActivoDto.i_EsTemporal = 0;
                    new ActivoFijoBL().ActualizarActivoFijo(ref pobjOperationResult, objActivoDto, ClientSession,
                        pTemp_Insertar, pTemp_Editar, pTemp_Eliminar, pTemp_InsertarAnexo, pTemp_EditarAnexo, pTemp_EliminarAnexo);

                }
            }
        }

        public void ActualizarCompra(ref OperationResult pobjOperationResult, compraDto pobjDtoEntity,
            List<string> ClientSession, List<compradetalleDto> pTemp_Insertar, List<compradetalleDto> pTemp_Editar,
            List<compradetalleDto> pTemp_Eliminar)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    var objOperationResult = new OperationResult();
                    var objSecuentialBL = new SecuentialBL();
                    var objEntityCompra = pobjDtoEntity.ToEntity();
                    var pobjDtoCompraDetalle = new compradetalleDto();
                    var dbContext = new SAMBHSEntitiesModelWin();
                    var _objDocumentoBL = new DocumentoBL();
                    var SecuentialId = 0;
                    var newIdCompraDetalle = string.Empty;
                    int intNodeId;

                    #region Actualiza Cabecera

                    var idTipoDoc = pobjDtoEntity.i_IdTipoDocumento ?? -1;
                    var serie = pobjDtoEntity.v_SerieDocumento.Trim();
                    var corr = pobjDtoEntity.v_CorrelativoDocumento.Trim();
                    var prov = pobjDtoEntity.v_IdProveedor;
                    var idCompra = pobjDtoEntity.v_IdCompra;

                    var yaExiste = dbContext.compra.Any(p => p.v_IdCompra != idCompra && p.i_IdTipoDocumento == idTipoDoc &&
                                p.i_Eliminado == 0 && p.v_SerieDocumento.Trim().Equals(serie) &&
                                p.v_CorrelativoDocumento.Trim().Equals(corr) && p.v_IdProveedor.Equals(prov));

                    if (yaExiste) throw new Exception("Documento ingresado anteriormente!");

                    intNodeId = int.Parse(ClientSession[0]);
                    var objEntitySource = (dbContext.compra.Where(a => a.v_IdCompra == pobjDtoEntity.v_IdCompra)).FirstOrDefault();

                    pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                    pobjDtoEntity.i_ActualizaIdUsuario = int.Parse(ClientSession[2]);

                    var objEntity = pobjDtoEntity.ToEntity();
                    dbContext.compra.ApplyCurrentValues(objEntity);

                    #endregion

                    #region Actualiza Detalle

                    foreach (var compradetalleDto in pTemp_Insertar)
                    {
                        var NotadeIngresoDetalle = (from n in dbContext.movimientodetalle
                                                    join J1 in dbContext.movimiento on
                                                        new { idMovimiento = n.v_IdMovimiento, IdAlmacen = compradetalleDto.i_IdAlmacen }
                                                        equals new { idMovimiento = J1.v_IdMovimiento, IdAlmacen = J1.i_IdAlmacenOrigen } into
                                                        J1_join
                                                    from J1 in J1_join.DefaultIfEmpty()
                                                    where
                                                        n.i_Eliminado == 0 && n.v_IdProductoDetalle == compradetalleDto.v_IdProductoDetalle &&
                                                        J1.v_OrigenTipo == "C" && J1.v_OrigenRegPeriodo == objEntityCompra.v_Periodo
                                                        && J1.v_OrigenRegMes == objEntityCompra.v_Mes &&
                                                        J1.v_OrigenRegCorrelativo == objEntityCompra.v_Correlativo
                                                    select new { n.v_IdMovimientoDetalle }).FirstOrDefault();

                        var objEntityCompraDetalle = compradetalleDto.ToEntity();
                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 32);
                        newIdCompraDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "ZY");
                        objEntityCompraDetalle.v_IdCompraDetalle = newIdCompraDetalle;
                        objEntityCompraDetalle.v_IdMovimientoDetalle = NotadeIngresoDetalle != null
                            ? NotadeIngresoDetalle.v_IdMovimientoDetalle
                            : null;
                        objEntityCompraDetalle.t_InsertaFecha = DateTime.Now;
                        objEntityCompraDetalle.i_InsertaIdUsuario = int.Parse(ClientSession[2]);
                        objEntityCompraDetalle.i_Eliminado = 0;
                        dbContext.AddTocompradetalle(objEntityCompraDetalle);
                        dbContext.SaveChanges();
                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName,
                            "compradetalle", newIdCompraDetalle);
                        if (compradetalleDto.EsActivoFijo == 1)
                        {
                            ActualizarActivoFijo(pobjDtoEntity, compradetalleDto, ClientSession);
                        }
                    }

                    foreach (var compradetalleDto in pTemp_Editar)
                    {
                        var _objEntity = compradetalleDto.ToEntity();
                        var query = (from n in dbContext.compradetalle
                                     where n.v_IdCompraDetalle == compradetalleDto.v_IdCompraDetalle
                                     select n).FirstOrDefault();

                        _objEntity.t_ActualizaFecha = DateTime.Now;
                        _objEntity.i_ActualizaIdUsuario = int.Parse(ClientSession[2]);

                        dbContext.compradetalle.ApplyCurrentValues(_objEntity);
                        Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName,
                            "compradetalle", query.v_IdCompraDetalle);
                        if (compradetalleDto.EsActivoFijo == 1)
                        {
                            ActualizarActivoFijo(pobjDtoEntity, compradetalleDto, ClientSession);
                        }
                    }

                    if (pTemp_Editar.Count == 0)
                    {
                        var ComprasDetalles = (from a in dbContext.producto
                                               join b in dbContext.productodetalle on new { a = a.v_IdProducto, eliminado = 0 } equals
                                                   new { a = b.v_IdProducto, eliminado = b.i_Eliminado.Value } into b_join
                                               from b in b_join.DefaultIfEmpty()
                                               join c in dbContext.compradetalle on new { b = b.v_IdProductoDetalle, eliminado = 0 } equals
                                                   new { b = c.v_IdProductoDetalle, eliminado = c.i_Eliminado.Value } into c_join
                                               from c in c_join.DefaultIfEmpty()
                                               where a.i_Eliminado.Value == 0 && c.v_IdCompra == pobjDtoEntity.v_IdCompra
                                               select new compradetalleDto
                                               {
                                                   v_IdCompra = c.v_IdCompra,
                                                   v_IdCompraDetalle = c.v_IdCompraDetalle,
                                                   d_ValorVenta = c.d_ValorVenta,
                                                   EsActivoFijo = a.i_EsActivoFijo.Value,
                                                   v_IdProductoDetalle = c.v_IdProductoDetalle
                                               }).ToList();
                        foreach (var objCompraDetalle in ComprasDetalles)
                        {
                            if (objCompraDetalle.EsActivoFijo == 1)
                            {
                                ActualizarActivoFijo(pobjDtoEntity, objCompraDetalle, ClientSession);
                            }
                        }
                    }

                    foreach (var compradetalleDto in pTemp_Eliminar)
                    {
                        var _objEntity = compradetalleDto.ToEntity();
                        var query = (from n in dbContext.compradetalle
                                     where n.v_IdCompraDetalle == compradetalleDto.v_IdCompraDetalle
                                     select n).FirstOrDefault();

                        if (query != null)
                        {
                            query.t_ActualizaFecha = DateTime.Now;
                            query.i_ActualizaIdUsuario = int.Parse(ClientSession[2]);
                            query.i_Eliminado = 1;
                            if (compradetalleDto.EsActivoFijo == 1)
                            {
                                new ActivoFijoBL().EliminarActivoFijoDesdeCompras(ref objOperationResult, ClientSession,
                                    query.v_IdProductoDetalle);
                            }
                        }

                        dbContext.compradetalle.ApplyCurrentValues(query);
                        Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName,
                            "compradetalle", query.v_IdCompraDetalle);
                    }

                    #endregion

                    #region Genera Pago Pendiente

                    if (pobjDtoEntity.v_IdCompra != null)
                    {
                        EliminarPagoPendiente(ref pobjOperationResult, pobjDtoEntity.v_IdCompra,
                            Globals.ClientSession.GetAsList(), pobjDtoEntity.i_IdEstado.Value, 0);
                        if (pobjDtoEntity.i_IdEstado == 1)
                            InsertaPagosPendiente(ref pobjOperationResult, pobjDtoEntity.v_IdCompra,
                                pobjDtoEntity.d_Total.Value, Globals.ClientSession.GetAsList());
                    }
                    if (pobjOperationResult.Success == 0) return;

                    #endregion

                    #region Regenera asiento contable
                    dbContext.SaveChanges();
                    if (_objDocumentoBL.DocumentoEsContable(pobjDtoEntity.i_IdTipoDocumento.Value) &&
                        pobjDtoEntity.i_IdEstado == 1)
                    {
                        RegenerarAsientoContable(ref pobjOperationResult, pobjDtoEntity.v_IdCompra);
                        if (pobjOperationResult.Success == 0) return;
                    }
                    else if (pobjDtoEntity.i_IdEstado == 0)
                    {
                        EliminarAsientoContable(ref pobjOperationResult, pobjDtoEntity.v_IdCompra);
                        if (pobjOperationResult.Success == 0) return;
                    }
                    #endregion

                    #region Actualiza Detalles de Orden de Compra Si es Necesario

                    if (!string.IsNullOrEmpty(objEntitySource.v_ODCSerie) &&
                        !string.IsNullOrEmpty(objEntitySource.v_ODCCorrelativo))
                    {
                        var Flag = false;

                        var ODC = (from n in dbContext.ordendecompra
                                   where
                                       n.v_SerieDocumento == objEntitySource.v_ODCSerie &&
                                       n.v_CorrelativoDocumento == objEntitySource.v_ODCCorrelativo
                                   select n).FirstOrDefault();

                        if (ODC != null)
                        {
                            foreach (var DetalleCompra in pTemp_Eliminar)
                            {
                                var DetalleCompraEntity = (from n in dbContext.compradetalle
                                                           where n.v_IdCompraDetalle == DetalleCompra.v_IdCompraDetalle
                                                           select n).FirstOrDefault();

                                var _ordendecompradetalle =
                                    ODC.ordendecompradetalle.Where(
                                        p => p.v_IdProductoDetalle == DetalleCompraEntity.v_IdProductoDetalle)
                                        .FirstOrDefault();

                                if (_ordendecompradetalle != null)
                                {
                                    _ordendecompradetalle.i_UsadoEnCompra = 0;
                                    dbContext.ordendecompradetalle.ApplyCurrentValues(_ordendecompradetalle);
                                    Flag = true;
                                }
                            }

                            if (Flag)
                            {
                                ODC.i_IdEstado = (int)OrdenCompraEstados.Pendiente;
                            }

                            dbContext.ordendecompra.ApplyCurrentValues(ODC);
                        }
                    }

                    #endregion

                    #region Regenera el ingreso a almacén

                    RegenerarIngresoAlmacen(ref pobjOperationResult, objEntitySource.v_IdCompra);
                    // EliminarIngresoAlmacenyRegenera(ref pobjOperationResult, objEntitySource.v_IdCompra);
                    if (pobjOperationResult.Success == 0) return;

                    #endregion

                    Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "compra",
                        objEntitySource.v_IdCompra);
                    pobjOperationResult.Success = 1;
                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ComprasBL.ActualizarCompra()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public void EliminarPagoPendiente(ref OperationResult pobjOperationResult, string pstrIdCompra,
            List<string> ClientSession, int estadoDocumento, int eliminado)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    var dbContext = new SAMBHSEntitiesModelWin();
                    var objEntidadCompra = dbContext.compra.FirstOrDefault(p => p.v_IdCompra == pstrIdCompra);

                    var _pagopendienteEntity = (from _PagoPendienteEntity in dbContext.pagopendiente
                                                where
                                                    _PagoPendienteEntity.v_IdCompra == pstrIdCompra &&
                                                    _PagoPendienteEntity.i_Eliminado == 0
                                                select _PagoPendienteEntity).FirstOrDefault();

                    if (_pagopendienteEntity != null)
                    {
                        _pagopendienteEntity.t_ActualizaFecha = DateTime.Now;
                        _pagopendienteEntity.i_ActualizaIdUsuario = int.Parse(ClientSession[2]);
                        _pagopendienteEntity.i_Eliminado = 1;
                        dbContext.pagopendiente.ApplyCurrentValues(_pagopendienteEntity);
                    }

                    #region Procesa casos en que la compra sea nota de crédito

                    if (objEntidadCompra.i_IdTipoDocumento == 7 &&
                        (eliminado == 1 || objEntidadCompra.i_IdEstado.Value != estadoDocumento))

                        if (_objDocumentoBL.DocumentoEsInverso(objEntidadCompra.i_IdTipoDocumento.Value))
                        {
                            var IdCompraReferencia = DevolverIdCompra(ref pobjOperationResult,
                                objEntidadCompra.v_IdProveedor, objEntidadCompra.i_IdTipoDocumentoRef.Value,
                                objEntidadCompra.v_SerieDocumentoRef, objEntidadCompra.v_CorrelativoDocumentoRef);

                            if (!string.IsNullOrEmpty(IdCompraReferencia))
                            {
                                var TCambio = objEntidadCompra.d_TipoCambio.Value;
                                var _PagoPendienteEntity = (from n in dbContext.pagopendiente
                                                            where n.v_IdCompra == IdCompraReferencia && n.i_Eliminado == 0
                                                            select n).FirstOrDefault();

                                if (_PagoPendienteEntity != null)
                                {
                                    var Moneda = (from m in dbContext.compra
                                                  where m.v_IdCompra == IdCompraReferencia && m.i_Eliminado == 0
                                                  select new { m.i_IdMoneda }).FirstOrDefault().i_IdMoneda.Value;


                                    switch (objEntidadCompra.i_IdMoneda)
                                    {
                                        case 1:
                                            switch (Moneda)
                                            {
                                                case 1:
                                                    _PagoPendienteEntity.d_Acuenta =
                                                        _PagoPendienteEntity.d_Acuenta.Value -
                                                        objEntidadCompra.d_Total.Value;
                                                    _PagoPendienteEntity.d_Saldo = _PagoPendienteEntity.d_Saldo.Value +
                                                                                   objEntidadCompra.d_Total.Value >= 0
                                                        ? _PagoPendienteEntity.d_Saldo.Value +
                                                          objEntidadCompra.d_Total.Value
                                                        : 0;

                                                    break;

                                                case 2:
                                                    _PagoPendienteEntity.d_Acuenta =
                                                        _PagoPendienteEntity.d_Acuenta.Value -
                                                        objEntidadCompra.d_Total.Value / TCambio;
                                                    _PagoPendienteEntity.d_Saldo = _PagoPendienteEntity.d_Saldo.Value +
                                                                                   objEntidadCompra.d_Total.Value /
                                                                                   TCambio >= 0
                                                        ? _PagoPendienteEntity.d_Saldo.Value +
                                                          objEntidadCompra.d_Total.Value / TCambio
                                                        : 0;

                                                    break;
                                            }
                                            break;

                                        case 2:
                                            switch (Moneda)
                                            {
                                                case 1:
                                                    _PagoPendienteEntity.d_Acuenta =
                                                        _PagoPendienteEntity.d_Acuenta.Value -
                                                        objEntidadCompra.d_Total.Value * TCambio;
                                                    _PagoPendienteEntity.d_Saldo = _PagoPendienteEntity.d_Saldo.Value +
                                                                                   objEntidadCompra.d_Total.Value *
                                                                                   TCambio >= 0
                                                        ? _PagoPendienteEntity.d_Saldo.Value +
                                                          objEntidadCompra.d_Total.Value * TCambio
                                                        : 0;

                                                    break;

                                                case 2:
                                                    _PagoPendienteEntity.d_Acuenta =
                                                        _PagoPendienteEntity.d_Acuenta.Value -
                                                        objEntidadCompra.d_Total.Value;
                                                    _PagoPendienteEntity.d_Saldo = _PagoPendienteEntity.d_Saldo.Value +
                                                                                   objEntidadCompra.d_Total.Value >= 0
                                                        ? _PagoPendienteEntity.d_Saldo.Value +
                                                          objEntidadCompra.d_Total.Value
                                                        : 0;

                                                    break;
                                            }
                                            break;
                                    }

                                    _PagoPendienteEntity.t_ActualizaFecha = DateTime.Now;
                                    _PagoPendienteEntity.i_ActualizaIdUsuario = int.Parse(ClientSession[2]);

                                    dbContext.pagopendiente.ApplyCurrentValues(_PagoPendienteEntity);
                                }
                                else
                                {
                                    pobjOperationResult.Success = 0;
                                    pobjOperationResult.ErrorMessage = "No se encontró Pago Pendiente";
                                    pobjOperationResult.AdditionalInformation = "ComprasBL.EliminarPagoPendiente()";
                                    return;
                                }
                            }
                        }

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
                pobjOperationResult.AdditionalInformation = "CompraBL.EliminarCobranzaPendiente()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public void EliminarCompra(ref OperationResult pobjOperationResult, string pstrIdCompra,
            List<string> ClientSession)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {

                        #region Elimina Cabecera

                        // Obtener la entidad fuente
                        var objEntitySource = (from a in dbContext.compra
                                               where a.v_IdCompra == pstrIdCompra
                                               select a).FirstOrDefault();

                        // Crear la entidad con los datos actualizados
                        objEntitySource.t_ActualizaFecha = DateTime.Now;
                        objEntitySource.i_ActualizaIdUsuario = int.Parse(ClientSession[2]);
                        objEntitySource.i_Eliminado = 1;

                        #endregion

                        #region Elimina Detalles

                        //Eliminar detalles del movimiento eliminado.
                        var objEntitySourceDetallesCompra = (from a in dbContext.compradetalle
                                                             where a.v_IdCompra == pstrIdCompra
                                                             select a).ToList();

                        foreach (var RegistroCompraDetalle in objEntitySourceDetallesCompra)
                        {
                            RegistroCompraDetalle.t_ActualizaFecha = DateTime.Now;
                            RegistroCompraDetalle.i_ActualizaIdUsuario = int.Parse(ClientSession[2]);
                            RegistroCompraDetalle.i_Eliminado = 1;
                            Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName,
                                "compradetalle", RegistroCompraDetalle.v_IdCompraDetalle);

                            if (VerificarSiEsActivoFijo(RegistroCompraDetalle.v_IdProductoDetalle))
                            {
                                new ActivoFijoBL().EliminarActivoFijoDesdeCompras(ref pobjOperationResult, ClientSession,
                                    RegistroCompraDetalle.v_IdProductoDetalle);
                            }
                        }

                        #endregion

                        #region Elimina Notas de Ingresos Relacionadas

                        EliminarIngresoAlmacen(ref pobjOperationResult, objEntitySource.v_IdCompra);
                        if (pobjOperationResult.Success == 0) return;

                        #endregion

                        #region Elimina Cobranza Pendiente

                        if (objEntitySource.i_IdEstado != 0)
                        {
                            EliminarPagoPendiente(ref pobjOperationResult, pstrIdCompra, ClientSession,
                                objEntitySource.i_IdEstado.Value, 1);
                            if (pobjOperationResult.Success == 0) return;
                        }

                        #endregion

                        #region Elimina Libro Diario

                        var _objDiarioBL = new DiarioBL();
                        var objOperationResult = new OperationResult();
                        _objDiarioBL.EliminarDiarioXDocRef(ref objOperationResult, pstrIdCompra, ClientSession, true);
                        if (objOperationResult.Success == 0) return;

                        #endregion

                        #region Actualiza Detalles de Orden de Compra Si es Necesario

                        if (!string.IsNullOrEmpty(objEntitySource.v_ODCSerie) &&
                            !string.IsNullOrEmpty(objEntitySource.v_ODCCorrelativo))
                        {
                            var Flag = false;

                            var ODC = (from n in dbContext.ordendecompra
                                       where
                                           n.v_SerieDocumento == objEntitySource.v_ODCSerie &&
                                           n.v_CorrelativoDocumento == objEntitySource.v_ODCCorrelativo
                                       select n).FirstOrDefault();

                            if (ODC != null)
                            {
                                foreach (var DetalleCompra in objEntitySourceDetallesCompra)
                                {
                                    var _ordendecompradetalle =
                                        ODC.ordendecompradetalle.Where(
                                            p => p.v_IdProductoDetalle == DetalleCompra.v_IdProductoDetalle)
                                            .FirstOrDefault();

                                    if (_ordendecompradetalle != null)
                                    {
                                        _ordendecompradetalle.i_UsadoEnCompra = 0;
                                        dbContext.ordendecompradetalle.ApplyCurrentValues(_ordendecompradetalle);
                                        Flag = true;
                                    }
                                }

                                if (Flag)
                                {
                                    ODC.i_IdEstado = (int)OrdenCompraEstados.Pendiente;
                                }

                                dbContext.ordendecompra.ApplyCurrentValues(ODC);
                            }
                        }

                        #endregion

                        #region Elimina las guias de remision de compras relacionadas.

                        if (!string.IsNullOrWhiteSpace(objEntitySource.v_GuiaRemisionSerie) &&
                            !string.IsNullOrWhiteSpace(objEntitySource.v_GuiaRemisionCorrelativo))
                        {
                            var serie = objEntitySource.v_GuiaRemisionSerie.Trim();
                            var correlativos = objEntitySource.v_GuiaRemisionCorrelativo.Contains(',')
                                ? objEntitySource.v_GuiaRemisionCorrelativo.Split(',').ToList()
                                : new List<string> { objEntitySource.v_GuiaRemisionCorrelativo };

                            foreach (var correlativo in correlativos)
                            {
                                var guiaRemisionCompra =
                                    dbContext.guiaremisioncompra.FirstOrDefault(
                                        p =>
                                            p.v_SerieDocumento.Trim().Equals(serie.Trim()) &&
                                            p.v_CorrelativoDocumento.Trim().Equals(correlativo.Trim()) &&
                                            p.i_Eliminado == 0 &&
                                            p.v_IdProveedor.Trim().Equals(objEntitySource.v_IdProveedor.Trim()));
                                if (guiaRemisionCompra != null)
                                {
                                    new GuiaRemisionCompraBL().EliminarGuiaRemisionCompra(ref pobjOperationResult,
                                        guiaRemisionCompra.v_IdGuiaCompra, ClientSession);
                                    if (pobjOperationResult.Success == 0) return;
                                }
                            }
                        }

                        #endregion

                        dbContext.SaveChanges();
                        Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "compra",
                            pstrIdCompra);
                        pobjOperationResult.Success = 1;
                        ts.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ComprasBL.EliminarCompra()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        private bool VerificarSiEsActivoFijo(string IdProductoDetalle)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var ActivoFijo = (from n in dbContext.activofijo
                                  where n.v_IdProducto == IdProductoDetalle
                                  select n).FirstOrDefault();

                if (ActivoFijo != null)
                {
                    return true;
                }
                return false;
            }
        }

        public string[] DevolverProveedorPorNroDocumento(ref OperationResult pobjOperationResult, string NroDocumento)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from n in dbContext.cliente
                                 where n.i_Eliminado == 0 && n.v_FlagPantalla == "V" && n.v_NroDocIdentificacion == NroDocumento
                                 select n
                                ).FirstOrDefault();

                    pobjOperationResult.Success = 1;

                    if (query != null)
                    {
                        var Cadena = new string[3];
                        Cadena[0] = query.v_IdCliente;
                        Cadena[1] = query.v_CodCliente;
                        Cadena[2] =
                            (query.v_ApePaterno + " " + query.v_ApeMaterno + " " + query.v_PrimerNombre + " " +
                             query.v_RazonSocial).Trim();
                        return Cadena;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ComprasBL.DevolverProveedorPorNroDocumento()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public string[] DevolverProveedorPorID(ref OperationResult pobjOperationResult, string pstrIdProveedor)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from n in dbContext.cliente
                                 where n.i_Eliminado == 0 && n.v_FlagPantalla == "V" && n.v_IdCliente == pstrIdProveedor
                                 select n
                                ).FirstOrDefault();

                    pobjOperationResult.Success = 1;

                    if (query != null)
                    {
                        var Cadena = new string[3];
                        Cadena[0] = query.v_NroDocIdentificacion;
                        Cadena[1] = query.v_CodCliente;
                        Cadena[2] =
                            (query.v_ApePaterno + " " + query.v_ApeMaterno + " " + query.v_PrimerNombre + " " +
                             query.v_RazonSocial).Trim();
                        return Cadena;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ComprasBL.DevolverProveedorPorID()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
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
                pobjOperationResult.AdditionalInformation = "ComprasBL.DevolverTipoCambioPorFecha()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return "0";
            }
        }

        public List<KeyValueDTO> ObtenerConceptosParaCombo(ref OperationResult pobjOperationResult, int pintIdArea,
            string pstrSortExpression)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = from a in dbContext.administracionconceptos
                                join A in dbContext.concepto on new { cod = a.v_Codigo, eliminado = 0 }
                                    equals new { cod = A.v_Codigo, eliminado = A.i_Eliminado.Value } into A_join
                                from A in A_join.DefaultIfEmpty()
                                where A.i_IdArea == pintIdArea && a.i_Eliminado == 0 && a.v_Periodo.Equals(periodo)
                                select new
                                {
                                    a.v_Codigo,
                                    a.v_Nombre,
                                    CuentaVenta = a.v_CuentaPVenta
                                };
                    query = query.OrderBy(!string.IsNullOrEmpty(pstrSortExpression) ? pstrSortExpression : "v_Nombre");

                    var query2 = query.AsEnumerable().GroupBy(g => g.v_Codigo)
                        .Select(x => new KeyValueDTO
                            {
                                Id = x.Key,
                                Value1 = x.Key + " | " + x.FirstOrDefault().v_Nombre,
                                Value2 = x.FirstOrDefault().CuentaVenta
                            }
                        ).ToList();

                    pobjOperationResult.Success = 1;
                    return query2;

                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ComprasBL.ObtenerConceptosParaCombo()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public bool ComprobarRelacionDocumentoProveedor(ref OperationResult pobjOperationResult, string pstrIdProveedor,
            int pintIdTipoDocumento, string pstrSerieDoc, string pstrCorrelativoDoc, string pstrIdCompra)
        {
            try
            {
                pobjOperationResult.Success = 1;
                if (pintIdTipoDocumento != -1 && pstrIdProveedor != null && pstrSerieDoc != null &&
                    pstrCorrelativoDoc != null)
                // comprueba que tenga los datos necesarios para no consultar a la Bd en vano.
                {
                    var dbContext = new SAMBHSEntitiesModelWin();
                    if (pstrIdCompra == null)
                    {
                        var query = (from n in dbContext.compra
                                     where n.i_Eliminado == 0 && n.v_IdProveedor == pstrIdProveedor
                                           && n.i_IdTipoDocumento == pintIdTipoDocumento
                                           && n.v_SerieDocumento == pstrSerieDoc &&
                                           n.v_CorrelativoDocumento == pstrCorrelativoDoc
                                     select n
                            ).FirstOrDefault();
                        return query != null;
                    }
                    else
                    {
                        var query = (from n in dbContext.compra
                                     where n.i_Eliminado == 0 && n.v_IdProveedor == pstrIdProveedor
                                           && n.i_IdTipoDocumento == pintIdTipoDocumento
                                           && n.v_SerieDocumento == pstrSerieDoc &&
                                           n.v_CorrelativoDocumento == pstrCorrelativoDoc
                                           && n.v_IdCompra != pstrIdCompra
                                     select n
                            ).FirstOrDefault();
                        return query != null;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ComprasBL.ComprobarRelacionDocumentoProveedor()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return false;
            }
        }

        public string DevolverIdCompra(ref OperationResult pobjOperationResult, string pstrIdProveedor,
            int pintIdTipoDocumento, string pstrSerieDoc, string pstrCorrelativoDoc)
        {
            try
            {
                if (pintIdTipoDocumento != -1 && pstrIdProveedor != null && pstrSerieDoc != null &&
                    pstrCorrelativoDoc != null)
                // comprueba que tenga los datos necesarios para no consultar a la Bd en vano.
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var query = (from n in dbContext.compra
                                     where n.i_Eliminado == 0 && n.v_IdProveedor == pstrIdProveedor
                                           && n.i_IdTipoDocumento == pintIdTipoDocumento
                                           && n.v_SerieDocumento == pstrSerieDoc && n.v_CorrelativoDocumento == pstrCorrelativoDoc
                                     select new { n.v_IdCompra }).FirstOrDefault();
                        if (query != null)
                        {
                            return query.v_IdCompra;
                        }
                        return string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ComprasBL.DevolverIdCompra()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
            return "";
        }

        public string[] DevolverPeriodoMesCorrelativoCompra(ref OperationResult pobjOperationResult,
            string pstrIdProveedor, int pintIdTipoDocumento, string pstrSerieDoc, string pstrCorrelativoDoc)
        {
            try
            {
                var Cadena = new string[3];
                if (pintIdTipoDocumento != -1 && pstrSerieDoc != null && pstrCorrelativoDoc != null)
                // comprueba que tenga los datos necesarios para no consultar a la Bd en vano.
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var query = (from n in dbContext.compra
                                     where n.i_Eliminado == 0 && n.v_IdProveedor == pstrIdProveedor
                                           && n.i_IdTipoDocumento == pintIdTipoDocumento
                                           && n.v_SerieDocumento == pstrSerieDoc && n.v_CorrelativoDocumento == pstrCorrelativoDoc
                                     select new { n.v_Periodo, n.v_Mes, n.v_Correlativo }).FirstOrDefault();
                        if (query != null)
                        {
                            Cadena[0] = query.v_Periodo;
                            Cadena[1] = query.v_Mes;
                            Cadena[2] = query.v_Correlativo;
                        }
                    }
                }
                return Cadena;
            }
            catch (Exception ex)
            {
                return null;
                throw ex;
            }
        }

        public bool ExisteDocumento(string pstrSerie, string pstrCorrelativo)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var Registro = (from n in dbContext.compra
                                where
                                    n.i_Eliminado == 0 && n.v_SerieDocumento == pstrSerie && n.v_CorrelativoDocumento == pstrCorrelativo
                                select n).FirstOrDefault();

                if (Registro == null)
                {
                    return true;
                }
                return false;
            }
        }

        public guiaremisioncompraDto DevolverGuiaRemisionCompraCabecera(ref OperationResult pobjOperationResult,
            string serie, string correlativo, string pstrIdProveedor)
        {
            try
            {
                using (var dbcontext = new SAMBHSEntitiesModelWin())
                {
                    var _guiaremisioncompra = (from g in dbcontext.guiaremisioncompra
                                               where g.v_SerieDocumento == serie && g.v_CorrelativoDocumento == correlativo
                                                     && g.v_IdProveedor == pstrIdProveedor
                                                     && g.i_Eliminado == 0
                                               select g).FirstOrDefault();

                    var Dto = new guiaremisioncompraDto();

                    pobjOperationResult.Success = 1;
                    return _guiaremisioncompra.ToDTO();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ComprasBL.DevolverGuiaRemisionCompraCabecera()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public List<guiaremisioncompradetalleDto> DevolverGuiaRemisionCompraDetalle(
            ref OperationResult pobjOperationResult, string pstrIdGuiaRemisionCompra)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var _guiaremisioncompradetalle = (from a in dbContext.guiaremisioncompradetalle
                                                      where a.v_IdGuiaCompra == pstrIdGuiaRemisionCompra
                                                            && a.i_Eliminado == 0
                                                      select a).ToList();
                    pobjOperationResult.Success = 1;

                    return _guiaremisioncompradetalle.ToDTOs();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ComprasBL.DevolverGuiaRemisionCompraDetalle()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public bool ComprobarSiFueLlamadaEnCompras(ref OperationResult pobjOperationResult, string pstrSerie,
            string pstrCorrelativo, string pstrIdProveedor)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var Result = (from c in dbContext.compra
                                  where
                                      c.v_GuiaRemisionSerie == pstrSerie && c.v_GuiaRemisionCorrelativo == pstrCorrelativo &&
                                      c.v_IdProveedor == pstrIdProveedor
                                      && c.i_Eliminado == 0
                                  select c).Count();

                    if (Result > 0)
                    {
                        pobjOperationResult.Success = 1;
                        return true;
                    }
                    pobjOperationResult.Success = 1;
                    return false;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ComprasBL.ComprobarSiFueLlamadaEnCompras()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return false;
            }
        }

        public string[] EliminarComprasXDocRef(ref OperationResult pobjOperationResult, string pstrIdDocRef,
            List<string> ClientSession, bool Origen)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    var dbContext = new SAMBHSEntitiesModelWin();
                    var IdRegistroEliminado = new string[6];


                    if (Origen) // Bandeja -Eliminado Logico
                    {
                        #region Elimina Cabecera

                        // Obtener la entidad fuente
                        var objEntitySource = (from a in dbContext.compra
                                               where a.v_IdDocumentoReferencia == pstrIdDocRef && a.i_Eliminado == 0
                                               select a).FirstOrDefault();


                        if (objEntitySource != null)
                        {
                            var PagosPendientes = (from a in dbContext.pagopendiente
                                                   where a.i_Eliminado == 0 && a.v_IdCompra == objEntitySource.v_IdCompra
                                                   select a).First();

                            PagosPendientes.t_ActualizaFecha = DateTime.Now;
                            PagosPendientes.i_ActualizaIdUsuario = int.Parse(ClientSession[2]);
                            PagosPendientes.i_Eliminado = 1;

                            IdRegistroEliminado[0] = objEntitySource.v_Periodo.Trim();
                            IdRegistroEliminado[1] = objEntitySource.v_Mes.Trim();
                            IdRegistroEliminado[2] = objEntitySource.v_Correlativo.Trim();

                            #region Elimina Detalles

                            //Eliminar detalles del movimiento eliminado.
                            var objEntitySourceDetallesCompra = (from a in dbContext.compradetalle
                                                                 where a.v_IdCompra == objEntitySource.v_IdCompra && a.i_Eliminado == 0
                                                                 select a).ToList();

                            objEntitySource.t_ActualizaFecha = DateTime.Now;
                            objEntitySource.i_ActualizaIdUsuario = int.Parse(ClientSession[2]);
                            objEntitySource.i_Eliminado = 1;

                            #endregion
                        }
                        else
                        {
                            IdRegistroEliminado[0] = string.Empty;
                        }

                        #region Elimina Libro Diario

                        var IdRegistroDiarioEliminado = new string[3];
                        var _objDiarioBL = new DiarioBL();
                        var objOperationResult = new OperationResult();

                        if (objEntitySource != null)
                        {
                            IdRegistroDiarioEliminado = _objDiarioBL.EliminarDiarioXDocRef(ref objOperationResult,
                                objEntitySource.v_IdCompra, ClientSession, Origen);
                            IdRegistroEliminado[3] = IdRegistroDiarioEliminado[0];
                            IdRegistroEliminado[4] = IdRegistroDiarioEliminado[1];
                            IdRegistroEliminado[5] = IdRegistroDiarioEliminado[2];
                        }

                        #endregion
                    }
                    else //Eliminado Fisico, cuando Eliminado es de procesos internos
                    {
                        #region Elimina Cabecera

                        // Obtener la entidad fuente

                        var objEntitySource = (from a in dbContext.compra
                                               where a.v_IdDocumentoReferencia == pstrIdDocRef && a.i_Eliminado == 0
                                               select a).FirstOrDefault();


                        if (objEntitySource != null)
                        {
                            var PagoPendientes = (from a in dbContext.pagopendiente
                                                  where a.v_IdCompra == objEntitySource.v_IdCompra
                                                  select a).ToList();

                            foreach (var pagopendient in PagoPendientes)
                            {
                                dbContext.pagopendiente.DeleteObject(pagopendient);
                            }
                            IdRegistroEliminado[0] = objEntitySource.v_Periodo.Trim();
                            IdRegistroEliminado[1] = objEntitySource.v_Mes.Trim();
                            IdRegistroEliminado[2] = objEntitySource.v_Correlativo.Trim();

                            #region Elimina Detalles

                            //Eliminar detalles del movimiento eliminado.
                            var objEntitySourceDetallesCompra = (from a in dbContext.compradetalle
                                                                 where a.v_IdCompra == objEntitySource.v_IdCompra && a.i_Eliminado == 0
                                                                 select a).ToList();

                            foreach (var RegistroCompraDetalle in objEntitySourceDetallesCompra)
                            {
                                dbContext.compradetalle.DeleteObject(RegistroCompraDetalle);
                            }

                            #endregion

                            dbContext.compra.DeleteObject(objEntitySource);

                        #endregion
                        }
                        else
                        {
                            IdRegistroEliminado[0] = string.Empty;
                        }

                        #region Elimina Libro Diario

                        var IdRegistroDiarioEliminado = new string[3];
                        var _objDiarioBL = new DiarioBL();
                        var objOperationResult = new OperationResult();

                        if (objEntitySource != null)
                        {
                            IdRegistroDiarioEliminado = _objDiarioBL.EliminarDiarioXDocRef(ref objOperationResult,
                                objEntitySource.v_IdCompra, ClientSession, true);
                            IdRegistroEliminado[3] = IdRegistroDiarioEliminado[0];
                            IdRegistroEliminado[4] = IdRegistroDiarioEliminado[1];
                            IdRegistroEliminado[5] = IdRegistroDiarioEliminado[2];
                        }

                        #endregion
                    }

                    dbContext.SaveChanges();

                        #endregion

                    ts.Complete();
                    pobjOperationResult.Success = 1;
                    return IdRegistroEliminado;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public string[] EliminarComprasXDocRefImportaciones(ref OperationResult pobjOperationResult, string pstrIdDocRef,
            List<string> ClientSession, bool Origen)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    var dbContext = new SAMBHSEntitiesModelWin();
                    var IdRegistroEliminado = new string[6];


                    if (Origen) // Bandeja -Eliminado Logico
                    {
                        #region Elimina Cabecera

                        // Obtener la entidad fuente
                        var objEntitySource = (from a in dbContext.compra
                                               where a.v_IdDocumentoReferencia == pstrIdDocRef && a.i_Eliminado == 0
                                               select a).FirstOrDefault();


                        if (objEntitySource != null)
                        {
                            var PagosPendientes = (from a in dbContext.pagopendiente
                                                   where a.i_Eliminado == 0 && a.v_IdCompra == objEntitySource.v_IdCompra
                                                   select a).First();

                            PagosPendientes.t_ActualizaFecha = DateTime.Now;
                            PagosPendientes.i_ActualizaIdUsuario = int.Parse(ClientSession[2]);
                            PagosPendientes.i_Eliminado = 1;

                            IdRegistroEliminado[0] = objEntitySource.v_Periodo.Trim();
                            IdRegistroEliminado[1] = objEntitySource.v_Mes.Trim();
                            IdRegistroEliminado[2] = objEntitySource.v_Correlativo.Trim();

                            #region Elimina Detalles

                            //Eliminar detalles del movimiento eliminado.
                            var objEntitySourceDetallesCompra = (from a in dbContext.compradetalle
                                                                 where a.v_IdCompra == objEntitySource.v_IdCompra && a.i_Eliminado == 0
                                                                 select a).ToList();

                            objEntitySource.t_ActualizaFecha = DateTime.Now;
                            objEntitySource.i_ActualizaIdUsuario = int.Parse(ClientSession[2]);
                            objEntitySource.i_Eliminado = 1;

                            #endregion
                        }
                        else
                        {
                            IdRegistroEliminado[0] = string.Empty;
                        }

                        #region Elimina Libro Diario

                        var IdRegistroDiarioEliminado = new string[3];
                        var _objDiarioBL = new DiarioBL();
                        var objOperationResult = new OperationResult();
                        IdRegistroDiarioEliminado = _objDiarioBL.EliminarDiarioXDocRef(ref objOperationResult,
                            objEntitySource.v_IdCompra, ClientSession, Origen);
                        IdRegistroEliminado[3] = IdRegistroDiarioEliminado[0];
                        IdRegistroEliminado[4] = IdRegistroDiarioEliminado[1];
                        IdRegistroEliminado[5] = IdRegistroDiarioEliminado[2];

                        #endregion
                    }
                    else //Eliminado Fisico, cuando Eliminado es de procesos internos
                    {
                        #region Elimina Cabecera

                        // Obtener la entidad fuente

                        var objEntitySource = (from a in dbContext.compra
                                               where a.v_IdDocumentoReferencia == pstrIdDocRef && a.i_Eliminado == 0
                                               select a).FirstOrDefault();


                        if (objEntitySource != null)
                        {
                            var PagoPendientes = (from a in dbContext.pagopendiente
                                                  where a.v_IdCompra == objEntitySource.v_IdCompra
                                                  select a).ToList();

                            foreach (var pagopendient in PagoPendientes)
                            {
                                dbContext.pagopendiente.DeleteObject(pagopendient);
                            }
                            IdRegistroEliminado[0] = objEntitySource.v_Periodo.Trim();
                            IdRegistroEliminado[1] = objEntitySource.v_Mes.Trim();
                            IdRegistroEliminado[2] = objEntitySource.v_Correlativo.Trim();

                            #region Elimina Detalles

                            //Eliminar detalles del movimiento eliminado.
                            var objEntitySourceDetallesCompra = (from a in dbContext.compradetalle
                                                                 where a.v_IdCompra == objEntitySource.v_IdCompra && a.i_Eliminado == 0
                                                                 select a).ToList();

                            foreach (var RegistroCompraDetalle in objEntitySourceDetallesCompra)
                            {
                                dbContext.compradetalle.DeleteObject(RegistroCompraDetalle);
                            }

                            #endregion

                            dbContext.compra.DeleteObject(objEntitySource);

                        #endregion
                        }
                        else
                        {
                            IdRegistroEliminado[0] = string.Empty;
                        }

                        #region Elimina Libro Diario

                        var IdRegistroDiarioEliminado = new string[3];
                        var _objDiarioBL = new DiarioBL();
                        var objOperationResult = new OperationResult();
                        IdRegistroDiarioEliminado = _objDiarioBL.EliminarDiarioXDocRef(ref objOperationResult,
                            objEntitySource.v_IdCompra, ClientSession, true);
                        IdRegistroEliminado[3] = IdRegistroDiarioEliminado[0];
                        IdRegistroEliminado[4] = IdRegistroDiarioEliminado[1];
                        IdRegistroEliminado[5] = IdRegistroDiarioEliminado[2];

                        #endregion
                    }

                    dbContext.SaveChanges();

                        #endregion

                    ts.Complete();
                    pobjOperationResult.Success = 1;
                    return IdRegistroEliminado;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public static bool ExisteDocumento2(int pintIdTipoDocumento, string pstrIdProveedor, string pstrSerie,
            string pstrCorrelativo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var result =
                        dbContext.compra.Any(
                            c =>
                                c.i_Eliminado == 0 && c.i_IdTipoDocumento.HasValue &&
                                c.i_IdTipoDocumento.Value.Equals(pintIdTipoDocumento) &&
                                c.v_IdProveedor.Trim().Equals(pstrIdProveedor.Trim())
                                && c.v_SerieDocumento.ToUpper().Trim().Equals(pstrSerie.ToUpper().Trim()) &&
                                c.v_CorrelativoDocumento.Trim().Equals(pstrCorrelativo.Trim()));

                    return result;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool RegenerarCompras(ref OperationResult pobjOperationResult, compraDto pobjDtoEntity,
            List<string> ClientSession, List<compradetalleDto> pTemp_Insertar)
        {
            using (var ts = TransactionUtils.CreateTransactionScope())
            {
                var IdRegistroEliminado = new string[6];
                var _ListadoDiarios = new List<KeyValueDTO>();
                var _objDiarioBL = new DiarioBL();
                var objOperationResult = new OperationResult();
                IdRegistroEliminado = EliminarComprasXDocRef(ref pobjOperationResult,
                    pobjDtoEntity.v_IdDocumentoReferencia, ClientSession, false);
                if (pobjOperationResult.Success == 0) return false;

                pobjDtoEntity.v_Periodo = pobjDtoEntity.v_Periodo != null
                    ? IdRegistroEliminado[0] == pobjDtoEntity.v_Periodo.Trim()
                        ? IdRegistroEliminado[0]
                        : pobjDtoEntity.v_Periodo
                    : IdRegistroEliminado[0];
                pobjDtoEntity.v_Mes = pobjDtoEntity.v_Mes != null
                    ? IdRegistroEliminado[1] == pobjDtoEntity.v_Mes.Trim()
                        ? IdRegistroEliminado[1]
                        : pobjDtoEntity.v_Mes
                    : IdRegistroEliminado[1];
                pobjDtoEntity.v_Correlativo = pobjDtoEntity.v_Correlativo != null
                    ? IdRegistroEliminado[1] == pobjDtoEntity.v_Mes.Trim()
                        ? IdRegistroEliminado[2]
                        : pobjDtoEntity.v_Correlativo
                    : IdRegistroEliminado[2];


                _ListadoDiarios = _objDiarioBL.ObtenerListadoDiario(ref objOperationResult,
                    pobjDtoEntity.v_Periodo, pobjDtoEntity.v_Mes, (int)LibroDiarios.Compra);

                int _MaxMovimiento;
                _MaxMovimiento = _ListadoDiarios.Count() > 0
                    ? int.Parse(_ListadoDiarios[_ListadoDiarios.Count() - 1].Value1)
                    : 0;
                _MaxMovimiento++;


                var IdRegistroDiarioEliminado = new string[3];
                IdRegistroDiarioEliminado[0] = pobjDtoEntity.v_Periodo != null
                    ? pobjDtoEntity.v_Periodo == IdRegistroEliminado[3]
                        ? IdRegistroEliminado[3]
                        : pobjDtoEntity.v_Periodo
                    : IdRegistroEliminado[3];
                IdRegistroDiarioEliminado[1] = pobjDtoEntity.v_Mes != null
                    ? IdRegistroEliminado[4] == pobjDtoEntity.v_Mes
                        ? IdRegistroEliminado[4]
                        : pobjDtoEntity.v_Mes
                    : IdRegistroEliminado[4];
                IdRegistroDiarioEliminado[2] = pobjDtoEntity.v_Mes != null
                    ? pobjDtoEntity.v_Mes == IdRegistroEliminado[4]
                        ? IdRegistroEliminado[5]
                        : _MaxMovimiento.ToString("00000000")
                    : IdRegistroEliminado[5];

                InsertarCompra(ref pobjOperationResult, pobjDtoEntity, ClientSession, pTemp_Insertar,
                    IdRegistroDiarioEliminado);

                if (pobjOperationResult.Success == 1)
                {
                    ts.Complete();
                    return true;
                }
                return false;
            }
        }

        public bool RegenerarComprasImportacion(ref OperationResult pobjOperationResult, compraDto pobjDtoEntity,
            List<string> ClientSession, List<compradetalleDto> pTemp_Insertar)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    pobjOperationResult.Success = 1;


                    var IdRegistroEliminado = new string[6];


                    IdRegistroEliminado = EliminarComprasXDocRefImportaciones(ref pobjOperationResult,
                        pobjDtoEntity.v_IdDocumentoReferencia, ClientSession, false);
                    if (pobjOperationResult.Success == 0) return false;

                    pobjDtoEntity.v_Periodo = IdRegistroEliminado[0];
                    pobjDtoEntity.v_Mes = IdRegistroEliminado[1];
                    pobjDtoEntity.v_Correlativo = IdRegistroEliminado[2];


                    var IdRegistroDiarioEliminado = new string[3];
                    IdRegistroDiarioEliminado[0] = IdRegistroEliminado[3];
                    IdRegistroDiarioEliminado[1] = IdRegistroEliminado[4];
                    IdRegistroDiarioEliminado[2] = IdRegistroEliminado[5];

                    InsertarCompra(ref pobjOperationResult, pobjDtoEntity, ClientSession, pTemp_Insertar,
                        IdRegistroDiarioEliminado);

                    if (pobjOperationResult.Success == 1)
                    {
                        ts.Complete();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception)
            {
                pobjOperationResult.Success = 0;
                return false;
            }
        }

        public bool ComprobarSiFueLlamadoEnTesoreria(string IDCompra, out string NroTesoreria)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                NroTesoreria = string.Empty;

                var Datos = (from c in dbContext.compra
                             where c.v_IdCompra == IDCompra
                             select c).FirstOrDefault();

                var IdProveedor = Datos.v_IdProveedor;
                var TipoDoc = Datos.i_IdTipoDocumento.Value;
                var NroDocumento = Datos.v_SerieDocumento.Trim() + "-" + Datos.v_CorrelativoDocumento.Trim();

                var Pendientecobrar = from t in dbContext.tesoreriadetalle
                                      where
                                      t.i_IdTipoDocumento == TipoDoc && t.v_IdCliente == IdProveedor &&
                                      t.v_NroDocumento == NroDocumento
                                      && t.v_Naturaleza == "D" && t.i_Eliminado == 0
                                      select t;

                if (Pendientecobrar != null && Pendientecobrar.Count() != 0)
                {
                    var Tesoreria = Pendientecobrar.FirstOrDefault().tesoreria;
                    var IdTipoDocTesoreria = Tesoreria.i_IdTipoDocumento.Value;
                    var Documento =
                        dbContext.documento.FirstOrDefault(p => p.i_CodigoDocumento == IdTipoDocTesoreria);
                    var SiglasBanco = Documento != null ? Documento.v_Siglas : string.Empty;
                    NroTesoreria = SiglasBanco.Trim() + "-" + Tesoreria.v_Mes.Trim() + "-" +
                                   Tesoreria.v_Correlativo.Trim();
                    return true;
                }
                return false;
            }
        }

        public productoshortDto DevolverArticuloPorCodInterno(string pstrCodInterno)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var query2 = (from n in dbContext.producto
                              join D in dbContext.productodetalle on n.v_IdProducto equals D.v_IdProducto into D_join
                              from D in D_join.DefaultIfEmpty()
                              join J4 in dbContext.datahierarchy on new { a = n.i_IdUnidadMedida.Value, b = 17 }
                              equals new { a = J4.i_ItemId, b = J4.i_GroupId } into J4_join
                              from J4 in J4_join.DefaultIfEmpty()
                              join B in dbContext.linea on n.v_IdLinea equals B.v_IdLinea into B_join
                              from B in B_join.DefaultIfEmpty()
                              where n.i_Eliminado == 0 && n.v_CodInterno == pstrCodInterno
                              select new productoshortDto
                              {
                                  v_IdProducto = n.v_IdProducto,
                                  v_IdProductoDetalle = D.v_IdProductoDetalle,
                                  v_Descripcion = n.v_Descripcion,
                                  v_CodInterno = n.v_CodInterno,
                                  i_EsServicio = n.i_EsServicio,
                                  i_EsLote = n.i_EsLote,
                                  i_IdTipoProducto = n.i_IdTipoProducto,
                                  i_IdUnidadMedida = n.i_IdUnidadMedida,
                                  EmpaqueUnidadMedida = J4.v_Value1,
                                  d_Empaque = n.d_Empaque,
                                  i_EsAfectoDetraccion = n.i_EsAfectoDetraccion,
                                  i_NombreEditable = n.i_NombreEditable,
                                  i_ValidarStock = n.i_ValidarStock,
                                  i_EsAfectoPercepcion = n.i_EsAfectoPercepcion,
                                  d_TasaPercepcion = n.d_TasaPercepcion,
                                  d_Precio = n.d_PrecioCosto ?? 0,
                                  PrecioVenta = n.d_PrecioVenta ?? 0,
                                  NroCuentaCompra = B != null ? B.v_NroCuentaCompra : string.Empty,
                                  NroCuentaVenta = B != null ? B.v_NroCuentaVenta : string.Empty,
                                  i_SolicitarNroLoteIngreso = n.i_SolicitarNroLoteIngreso ?? 0,
                                  i_SolicitarNroSerieIngreso = n.i_SolicitarNroSerieIngreso ?? 0,
                                  i_SolicitaOrdenProduccionIngreso = n.i_SolicitaOrdenProduccionIngreso ?? 0,

                              }
                ).FirstOrDefault();

                return query2;
            }
        }

        public bool ExisteNroRegistro(string Periodo, string Mes, string Correlativo)
        {
            try
            {
                var dbContext = new SAMBHSEntitiesModelWin();
                var replicationID = Globals.ClientSession.ReplicationNodeID;
                var Registro = (from n in dbContext.compra
                                where
                                    n.i_Eliminado == 0 && n.v_Periodo == Periodo && n.v_Mes == Mes && n.v_Correlativo == Correlativo &&
                                    n.v_IdCompra.Substring(0, 1) == replicationID
                                select n).FirstOrDefault();

                return Registro == null;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public void InsertaPagosPendiente(ref OperationResult pobjOperationResult, string IdCompra, decimal TotalCompra,
            List<string> ClientSession)
        {
            var SecuentialId = 0;
            var newId = string.Empty;
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var objSecuentialBL = new SecuentialBL();
                        var objPagoPendienteEntity = new pagopendienteDto();
                        var objEntity = objPagoPendienteEntity.ToEntity();
                        var objEntidadCompra = dbContext.compra.Where(x => x.v_IdCompra == IdCompra).FirstOrDefault();
                        decimal SaldoProcesadoDocumentoRefrerencia = 0;
                        #region Procesa casos en que la venta sea nota de crédito
                        if (_objDocumentoBL.DocumentoEsInverso(objEntidadCompra.i_IdTipoDocumento.Value))
                        {
                            var IdCompraReferencia = DevolverIdCompraPagosPendientes(ref pobjOperationResult,
                                objEntidadCompra.i_IdTipoDocumentoRef.Value, objEntidadCompra.v_SerieDocumentoRef,
                                objEntidadCompra.v_CorrelativoDocumentoRef);

                            if (!string.IsNullOrEmpty(IdCompraReferencia))
                            {
                                var TCambio = objEntidadCompra.d_TipoCambio.Value;

                                var _PagoPendienteEntity = (from n in dbContext.pagopendiente
                                                            where n.v_IdCompra == IdCompraReferencia && n.i_Eliminado == 0
                                                            select n).FirstOrDefault();


                                if (_PagoPendienteEntity != null)
                                {
                                    if (_PagoPendienteEntity.d_Saldo >= objEntidadCompra.d_Total.Value)
                                    {
                                        var Moneda = (from m in dbContext.compra
                                                      where m.v_IdCompra == IdCompraReferencia && m.i_Eliminado == 0
                                                      select new { m.i_IdMoneda }).FirstOrDefault().i_IdMoneda.Value;

                                        switch (objEntidadCompra.i_IdMoneda)
                                        {
                                            case 1:
                                                switch (Moneda)
                                                {
                                                    case 1:
                                                        _PagoPendienteEntity.d_Acuenta =
                                                            _PagoPendienteEntity.d_Acuenta.Value +
                                                            objEntidadCompra.d_Total.Value;
                                                        _PagoPendienteEntity.d_Saldo = _PagoPendienteEntity.d_Saldo.Value -
                                                                                       objEntidadCompra.d_Total.Value >= 0
                                                            ? _PagoPendienteEntity.d_Saldo.Value -
                                                              objEntidadCompra.d_Total.Value
                                                            : 0;
                                                        break;

                                                    case 2:
                                                        decimal Monto;
                                                        Monto =
                                                            decimal.Parse(
                                                                Math.Round(
                                                                    _PagoPendienteEntity.d_Saldo.Value -
                                                                    objEntidadCompra.d_Total.Value / TCambio, 1,
                                                                    MidpointRounding.AwayFromZero).ToString("0.00"));
                                                        _PagoPendienteEntity.d_Acuenta =
                                                            _PagoPendienteEntity.d_Acuenta.Value +
                                                            objEntidadCompra.d_Total.Value / TCambio;
                                                        _PagoPendienteEntity.d_Saldo = Monto >= 0 ? Monto : 0;
                                                        break;
                                                }
                                                break;

                                            case 2:
                                                switch (Moneda)
                                                {
                                                    case 1:
                                                        decimal Monto;
                                                        Monto =
                                                            decimal.Parse(
                                                                Math.Round(
                                                                    _PagoPendienteEntity.d_Saldo.Value -
                                                                    objEntidadCompra.d_Total.Value * TCambio, 1,
                                                                    MidpointRounding.AwayFromZero).ToString("0.00"));
                                                        _PagoPendienteEntity.d_Acuenta =
                                                            _PagoPendienteEntity.d_Acuenta.Value +
                                                            objEntidadCompra.d_Total.Value * TCambio;
                                                        _PagoPendienteEntity.d_Saldo = Monto >= 0 ? Monto : 0;
                                                        break;

                                                    case 2:
                                                        _PagoPendienteEntity.d_Acuenta =
                                                            _PagoPendienteEntity.d_Acuenta.Value +
                                                            objEntidadCompra.d_Total.Value;
                                                        _PagoPendienteEntity.d_Saldo = _PagoPendienteEntity.d_Saldo.Value -
                                                                                       objEntidadCompra.d_Total.Value >= 0
                                                            ? _PagoPendienteEntity.d_Saldo.Value - objEntidadCompra.d_Total
                                                            : 0;
                                                        break;
                                                }
                                                break;
                                        }

                                        _PagoPendienteEntity.t_ActualizaFecha = DateTime.Now;
                                        _PagoPendienteEntity.i_ActualizaIdUsuario = int.Parse(ClientSession[2]);

                                        dbContext.pagopendiente.ApplyCurrentValues(_PagoPendienteEntity);
                                        SaldoProcesadoDocumentoRefrerencia = objEntidadCompra.d_Total.Value;
                                    }
                                }
                                else
                                {
                                    pobjOperationResult.Success = 0;
                                    pobjOperationResult.ErrorMessage = "No se encontró la Cobranza Pendiente";
                                    pobjOperationResult.AdditionalInformation = "CobranzaBL.InsertarCobranza()";
                                    return;
                                }
                            }
                        }

                        #endregion

                        objEntity.v_IdCompra = IdCompra;
                        objEntity.d_Acuenta = 0 + SaldoProcesadoDocumentoRefrerencia;
                        objEntity.d_Saldo =
                            decimal.Parse(Math.Round(TotalCompra, 2, MidpointRounding.AwayFromZero).ToString("0.00")) -
                            SaldoProcesadoDocumentoRefrerencia;
                        objEntity.t_InsertaFecha = DateTime.Now;
                        objEntity.i_InsertaIdUsuario = int.Parse(ClientSession[2]);
                        objEntity.i_Eliminado = 0;
                        // Autogeneramos el Pk de la tabla
                        var intNodeId = int.Parse(ClientSession[0]);
                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 80);
                        newId = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "PC");
                        objEntity.v_IdPagoPendiente = newId;
                        dbContext.AddTopagopendiente(objEntity);
                        dbContext.SaveChanges();
                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName,
                            "pagopendiente", newId);
                        pobjOperationResult.Success = 1;
                        ts.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "CompraBL.InsertaPagosPendiente()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public List<historialpagoscompraDto> BuscarHistorialPagos(ref OperationResult pobjOperationResult,
           string pstrIdCompra)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region Query Buscar Pagos
                    var queryPagos = (from n in dbContext.pagodetalle
                                      join J1 in dbContext.pago on n.v_IdPago equals J1.v_IdPago into J1_join
                                      from J1 in J1_join.DefaultIfEmpty()
                                      join J4 in dbContext.documento on new { i_IdTipoDocumento = J1.i_IdTipoDocumento.Value }
                                          equals new { i_IdTipoDocumento = J4.i_CodigoDocumento } into J4_join
                                      from J4 in J4_join.DefaultIfEmpty()
                                      where n.v_IdCompra == pstrIdCompra && J1.i_Eliminado == 0 && n.i_Eliminado == 0
                                      select new historialpagoscompraDto
                                      {
                                          TipoDocumento = J4.v_Siglas,
                                          NroDocumento = J1.v_Mes.Trim() + "-" + J1.v_Correlativo,
                                          IdDocumento = J1.v_IdPago,
                                          Glosa = J1.v_Glosa,
                                          TipoCambio = J1.d_TipoCambio.Value,
                                          Fecha = J1.t_FechaRegistro.Value,
                                          Moneda = J1.i_IdMoneda == 1 ? "Soles" : "Dólares",
                                          EsLetra = false,
                                          Pago = n.d_ImporteSoles.Value,
                                          SaldoLetra = 0,
                                          Estado = J1.i_IdEstado ?? 0,
                                      }).ToList();
                    #endregion

                    #region Query Buscar Canjes a Letras


                    var queryLetras = (from n in dbContext.letraspagardetalle
                                       join J1 in dbContext.letraspagarcanje on n.v_IdLetrasPagar equals J1.v_IdLetrasPagar into J1_join
                                       from J1 in J1_join.DefaultIfEmpty()
                                       join J2 in dbContext.letraspagar on J1.v_IdLetrasPagar equals J2.v_IdLetrasPagar into J2_join
                                       from J2 in J2_join.DefaultIfEmpty()
                                       join J3 in dbContext.cobranzaletraspagarpendiente on n.v_IdLetrasPagarDetalle equals J3.v_IdLetrasPagarDetalle into J3_join
                                       from J3 in J3_join.DefaultIfEmpty()
                                       join J4 in dbContext.documento on new { i_IdTipoDocumento = n.i_IdTipoDocumento.Value }
                                           equals new { i_IdTipoDocumento = J4.i_CodigoDocumento } into J4_join
                                       from J4 in J4_join.DefaultIfEmpty()
                                       where J1.v_IdCompra == pstrIdCompra && J1.i_Eliminado == 0 && n.i_Eliminado == 0
                                       select new historialpagoscompraDto
                                       {
                                           TipoDocumento = J4.v_Siglas,
                                           NroDocumento = J2.v_Mes.Trim() + "-" + J2.v_Correlativo,
                                           IdDocumento = n.v_IdLetrasPagarDetalle,
                                           TipoCambio = J2.d_TipoCambio.Value,
                                           Glosa = "*C A N J E  A  L E T R A S*",
                                           Fecha = J2.t_FechaRegistro.Value,
                                           Moneda = J2.i_IdMoneda == 1 ? "Soles" : "Dólares",
                                           EsLetra = true,
                                           Pago = n.d_Importe.Value,
                                           SaldoLetra = J3.d_Saldo.Value,
                                           Estado = 1,
                                           FechaVencimiento = n.t_FechaVencimiento == null ? DateTime.Now : n.t_FechaVencimiento.Value,
                                       }).ToList();
                    #endregion

                    pobjOperationResult.Success = 1;
                    return queryPagos.Concat(queryLetras).ToList();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "CompraBL.BuscarHistorialPagos()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                return null;
            }
        }

        public string DevolverIdCompraPagosPendientes(ref OperationResult pobjOperationResult, int pintIdTipoDocumento,
            string pstrSerieDoc, string pstrCorrelativoDoc)
        {
            try
            {
                if (pintIdTipoDocumento != -1 && pstrSerieDoc != null && pstrCorrelativoDoc != null)
                // comprueba que tenga los datos necesarios para no consultar a la Bd en vano.
                {
                    var dbContext = new SAMBHSEntitiesModelWin();
                    var query = (from n in dbContext.compra
                                 where n.i_Eliminado == 0
                                       && n.i_IdTipoDocumento == pintIdTipoDocumento
                                       && n.v_SerieDocumento == pstrSerieDoc && n.v_CorrelativoDocumento == pstrCorrelativoDoc
                                 select new { n.v_IdCompra }).FirstOrDefault();
                    if (query != null)
                    {
                        return query.v_IdCompra;
                    }
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return "";
        }

        public bool TienePagosRealizados(string pstrIdCompra)
        {
            var dbContext = new SAMBHSEntitiesModelWin();

            var cpEntity = new List<pagodetalle>();
            var Compra = dbContext.compra.FirstOrDefault(p => p.v_IdCompra == pstrIdCompra);

            if (Compra != null)
            {
                //if (Compra.i_IdTipoDocumento != 7)
                if (!_objDocumentoBL.DocumentoEsInverso(Compra.i_IdTipoDocumento.Value))
                {
                    cpEntity =
                        dbContext.pagodetalle.Where(p => p.v_IdCompra == pstrIdCompra && p.i_Eliminado == 0)
                            .ToList();
                }
                else
                {
                    var NroDocumentoRef = Compra.v_SerieDocumento + "-" + Compra.v_CorrelativoDocumento;
                    // cpEntity = dbContext.pagodetalle.Where(p => p.i_IdTipoDocumentoRef == 7 && p.v_DocumentoRef == NroDocumentoRef && p.i_Eliminado == 0).ToList();
                    cpEntity =
                        dbContext.pagodetalle.Where(p => p.v_DocumentoRef == NroDocumentoRef && p.i_Eliminado == 0)
                            .ToList()
                            .Where(x => _objDocumentoBL.DocumentoEsInverso(x.i_IdTipoDocumentoRef.Value))
                            .ToList();
                }
            }

            if (cpEntity != null && cpEntity.Count > 0)
            {
                if (cpEntity.Sum(p => p.d_ImporteSoles.Value) > 0)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public bool TienePagoPendiente(string pstrIdCompra)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var Registro = (from n in dbContext.compra
                                where n.i_Eliminado == 0 && n.v_IdCompra == pstrIdCompra
                                select new { n.v_IdCompra }).FirstOrDefault();

                var Cobranza = (from c in dbContext.pagopendiente
                                where c.v_IdCompra == Registro.v_IdCompra
                                select new { c.d_Saldo }
                    ).FirstOrDefault();

                if (Cobranza.d_Saldo > 0)
                {
                    return true;
                }
                return false;
            }
        }

        public List<Tuple<string, string, string, string, string, decimal>> ObtenerComprasPercepcionPDT(ref OperationResult pobjOperationResult, DateTime fInicio, DateTime fFin)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var resp = new List<Tuple<string, string, string, string, string, decimal>>();
                    var comprasAfectas = (from p in dbContext.compra
                                          join c in dbContext.cliente on p.v_IdProveedor equals c.v_IdCliente into cJoin
                                          from c in cJoin.DefaultIfEmpty()
                                          where p.t_FechaEmision >= fInicio && p.t_FechaEmision <= fFin && (p.d_Percepcion ?? 0) > 0 &&
                                                p.i_Eliminado == 0
                                          orderby p.t_FechaEmision
                                          select new
                                          {
                                              c.v_NroDocIdentificacion,
                                              p.i_IdTipoDocumentoPercepcion,
                                              p.v_SeriePercepcion,
                                              p.v_CorrelativoPercepcion,
                                              p.t_FechaEmision,
                                              p.d_Percepcion
                                          }).ToList();
                    foreach (var c in comprasAfectas)
                    {
                        var nroDocCliente = c.v_NroDocIdentificacion;
                        var tipoComprobante = (c.i_IdTipoDocumentoPercepcion ?? 1).ToString("00");
                        int i;
                        var serie = (int.TryParse(c.v_SeriePercepcion, out i) ? i : 0).ToString("0000");
                        var correlativo = (int.TryParse(c.v_CorrelativoPercepcion, out i) ? i : 0).ToString("00000000");
                        var fechaEmision = c.t_FechaEmision.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        var monto = Utils.Windows.DevuelveValorRedondeado((c.d_Percepcion ?? 0m), 2);
                        resp.Add(Tuple.Create(nroDocCliente, tipoComprobante, serie, correlativo, fechaEmision, monto));
                    }

                    pobjOperationResult.Success = 1;
                    return resp;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ComprasBL.ObtenerComprasPercepcion()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        /// <summary>
        ///     Proceso que se creo especialmente para chayna a pedido de Evaristo ya que a inicios de su uso los ingresos se
        ///     generaron de manera erronea.
        ///     Solo procesa ingresos desde el 01/01/2016 al 31/05/2016.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        public void RegenerarIngresosPorCompras(ref OperationResult pobjOperationResult)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var objMovimientoBl = new MovimientoBL();

                        #region Se recopilan los ingresos a eliminar

                        var ingresosPorComprasGuias =
                            dbContext.movimiento.Where(
                                p =>
                                    p.i_Eliminado == 0 && p.i_IdTipoMovimiento == 1 &&
                                    (p.v_OrigenTipo.Equals("C") || p.v_OrigenTipo.Equals("G")) &&
                                    p.t_Fecha.HasValue &&
                                    p.t_Fecha.Value >= new DateTime(2016, 01, 01) &&
                                    p.t_Fecha.Value <= new DateTime(2016, 05, 31));

                        #endregion

                        #region Se recopilan las compras a procesar.

                        var comprasPorProcesar =
                            dbContext.compra.Where(
                                p =>
                                    p.i_Eliminado == 0 && p.t_FechaRegistro.HasValue &&
                                    p.t_FechaRegistro.Value >= new DateTime(2016, 01, 01) &&
                                    p.t_FechaRegistro.Value <= new DateTime(2016, 05, 31));

                        #endregion

                        #region Se recorren los ingresos y se eliminan.

                        foreach (var ingreso in ingresosPorComprasGuias)
                        {
                            objMovimientoBl.EliminarMovimiento(ref pobjOperationResult, ingreso.v_IdMovimiento,
                                Globals.ClientSession.GetAsList());
                            if (pobjOperationResult.Success == 0) return;
                        }

                        #endregion

                        #region Se recorren las compras y se les genera su nota de ingreso.

                        foreach (var compra in comprasPorProcesar)
                        {
                            var compraDetalles =
                                compra.compradetalle.Where(p => p.i_Eliminado == 0 && p.v_IdProductoDetalle != null &&
                                                                !p.v_IdProductoDetalle.Trim().Equals("")).ToList();

                            if (compraDetalles.Any())
                            {
                                var listaMovimientos = objMovimientoBl.ObtenerListadoMovimientos(
                                    ref pobjOperationResult, compra.v_Periodo, compra.v_Mes,
                                    (int)TipoDeMovimiento.NotadeIngreso);
                                var maxMovimiento = listaMovimientos.Any()
                                    ? int.Parse(listaMovimientos[listaMovimientos.Count - 1].Value1)
                                    : 0;

                                foreach (var detalle in compraDetalles.GroupBy(p => p.i_IdAlmacen))
                                {
                                    var movimientoDto = new movimientoDto();
                                    maxMovimiento++;
                                    movimientoDto.d_TipoCambio = compra.d_TipoCambio;
                                    movimientoDto.i_IdAlmacenOrigen = detalle.Key;
                                    movimientoDto.i_IdMoneda = compra.i_IdMoneda;
                                    movimientoDto.i_IdTipoMotivo = 1;
                                    movimientoDto.t_Fecha = compra.t_FechaRegistro;
                                    movimientoDto.v_Mes = compra.v_Mes.Trim();
                                    movimientoDto.v_Periodo = compra.v_Periodo.Trim();
                                    movimientoDto.i_IdTipoMovimiento = (int)TipoDeMovimiento.NotadeIngreso;
                                    movimientoDto.v_Correlativo = maxMovimiento.ToString("00000000");
                                    movimientoDto.v_IdCliente = compra.v_IdProveedor;
                                    movimientoDto.v_OrigenTipo = "C";
                                    movimientoDto.i_EsDevolucion = compra.i_IdTipoDocumento.Value == 7 ? 1 : 0;
                                    movimientoDto.v_OrigenRegCorrelativo = compra.v_Correlativo;
                                    movimientoDto.v_OrigenRegMes = compra.v_Mes;
                                    movimientoDto.v_OrigenRegPeriodo = compra.v_Periodo;
                                    movimientoDto.d_TotalPrecio = compra.d_Total;
                                    movimientoDto.i_IdEstablecimiento = compra.i_IdEstablecimiento ?? 1;

                                    var movimientosDetalleDto = detalle.ToList()
                                        .Select(d => new movimientodetalleDto
                                        {
                                            v_IdProductoDetalle = d.v_IdProductoDetalle,
                                            i_IdTipoDocumento = compra.i_IdTipoDocumento ?? -1,
                                            v_NumeroDocumento =
                                                string.Format("{0}-{1}", compra.v_SerieDocumento,
                                                    compra.v_CorrelativoDocumento),
                                            v_NroGuiaRemision = d.v_NroGuiaRemision,
                                            d_Cantidad = d.d_Cantidad ?? 0,
                                            i_IdUnidad = d.i_IdUnidadMedida ?? -1,
                                            d_CantidadEmpaque = d.d_CantidadEmpaque ?? 0,
                                            d_Precio = d.d_Precio ?? 0,
                                            d_Total =
                                                Utils.Windows.DevuelveValorRedondeado(
                                                    (d.d_Precio ?? 0) * (d.d_Cantidad ?? 0), 2),
                                            d_CantidadAdministrativa = d.d_Cantidad,
                                            d_CantidadEmpaqueAdministrativa = d.d_CantidadEmpaque
                                        }).ToList();

                                    movimientoDto.d_TotalCantidad = movimientosDetalleDto.Sum(p => p.d_Cantidad ?? 0);
                                    movimientoDto.d_TotalPrecio = movimientosDetalleDto.Sum(p => p.d_Total ?? 0);
                                    objMovimientoBl.InsertarMovimiento(ref pobjOperationResult, movimientoDto,
                                        Globals.ClientSession.GetAsList(), movimientosDetalleDto);
                                    if (pobjOperationResult.Success == 0) return;
                                }
                            }
                        }

                        #endregion

                        pobjOperationResult.Success = 1;
                        ts.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "MovimientoBL.RegeneraIngresosSegunCompras()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public bool CompraTieneGuiasRemisionEnlazadas(ref OperationResult pobjOperationResult, string pstrIdCompra)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var compra = dbContext.compra.FirstOrDefault(p => p.v_IdCompra.Equals(pstrIdCompra));
                    if (compra != null)
                        return !string.IsNullOrWhiteSpace(compra.v_GuiaRemisionCorrelativo) &&
                               !string.IsNullOrWhiteSpace(compra.v_GuiaRemisionSerie);

                    return false;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "CompraBL.CompraTieneGuiasRemisionEnlazadas()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return false;
            }
        }

        public string ObtenerCuentaContableProducto(string pstrIdProductoDetalle)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var linea = (from n in dbContext.productodetalle
                                 join p in dbContext.producto on n.v_IdProducto equals p.v_IdProducto into pJoin
                                 from p in pJoin.DefaultIfEmpty()

                                 join l in dbContext.linea on p.v_IdLinea equals l.v_IdLinea into lJoin
                                 from l in lJoin.DefaultIfEmpty()

                                 where n.v_IdProductoDetalle.Equals(pstrIdProductoDetalle) && l.i_Eliminado == 0

                                 select l).FirstOrDefault();
                    return linea != null ? linea.v_NroCuentaCompra : string.Empty;
                }
            }
            catch (Exception ex)
            {
                var pobjOperationResult = new OperationResult
                {
                    Success = 0,
                    AdditionalInformation = "CompraBL.ObtenerCuentaContableProducto()",
                    ErrorMessage = ex.Message,
                    ExceptionMessage = ex.InnerException != null
                        ? ex.InnerException.Message
                        : string.Empty
                };
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return string.Empty;
            }
        }

        #region Bandeja

        public List<compraDto> ListarBusquedaCompras(ref OperationResult pobjOperationResult, string pstrSortExpression,
            string pstrFilterExpression, DateTime F_Ini, DateTime F_Fin, bool SoloCompraDesdeImportacion, int idEstablecimiento = -1)
        {
            try
            {
                F_Fin = DateTime.Parse(F_Fin.Day + "/" + F_Fin.Month + "/" + F_Fin.Year + " 23:59");
                var dbContext = new SAMBHSEntitiesModelWin();

                var query = (from n in dbContext.compra
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

                             join J5 in dbContext.pagopendiente on new { idCompra = n.v_IdCompra, eliminado = 0 }
                                 equals new { idCompra = J5.v_IdCompra, eliminado = J5.i_Eliminado.Value } into J5_join
                             from J5 in J5_join.DefaultIfEmpty()
                             where
                                 n.i_Eliminado == 0 && n.t_FechaRegistro >= F_Ini && n.t_FechaRegistro <= F_Fin &&
                                 (idEstablecimiento == -1 || n.i_IdEstablecimiento == idEstablecimiento)
                             select new compraDto
                             {
                                 v_IdCompra = n.v_IdCompra,
                                 v_Mes = n.v_Mes,
                                 v_Correlativo = n.v_Correlativo,
                                 v_SerieDocumento = n.v_SerieDocumento,
                                 v_CorrelativoDocumento = n.v_CorrelativoDocumento,
                                 NroRegistro = n.v_Mes.Trim() + "-" + n.v_Correlativo,
                                 Documento = n.v_SerieDocumento + " - " + n.v_CorrelativoDocumento,
                                 i_IdTipoDocumento = n.i_IdTipoDocumento,
                                 TipoDocumento = J4.v_Siglas,
                                 t_FechaRegistro = n.t_FechaRegistro,
                                 t_FechaEmision = n.t_FechaEmision,
                                 v_IdProveedor = n.v_IdProveedor,
                                 CodigoProveedor = A.v_CodCliente,
                                 NombreProveedor =
                                     (A.v_ApePaterno + " " + A.v_ApeMaterno + " " + A.v_PrimerNombre + " " + A.v_SegundoNombre + " " + A.v_RazonSocial)
                                         .Trim(),
                                 d_Total = n.d_Total,
                                 i_IdEstado = n.i_IdEstado,
                                 t_InsertaFecha = n.t_InsertaFecha,
                                 t_ActualizaFecha = n.t_ActualizaFecha,
                                 v_UsuarioModificacion = J2.v_UserName,
                                 v_UsuarioCreacion = J3.v_UserName,
                                 v_Periodo = n.v_Periodo,
                                 RegistroOrigen = n.v_IdDocumentoReferencia,
                                 Moneda = n.i_IdMoneda == 1 ? "S" : "D",
                                 ListaCuentasUsadas = (from compradetalle cd in dbContext.compradetalle
                                                       where cd.v_IdCompra == n.v_IdCompra && cd.i_Eliminado == 0
                                                       select cd.v_NroCuenta).AsEnumerable(),
                                 NroRegistroOrigen = (from importaciondetallegastos b in dbContext.importaciondetallegastos
                                                      where b.v_IdImportacionDetalleGastos == n.v_IdDocumentoReferencia && b.i_Eliminado == 0
                                                      select b.v_IdImportacion).AsEnumerable(),
                                 Saldo = J5 != null ? J5.d_Saldo.Value : n.d_Total.Value,
                                 d_TipoCambio = n.d_TipoCambio,
                                 v_IdDocumentoReferencia = n.v_IdDocumentoReferencia,
                                 i_AplicaRectificacion = n.i_AplicaRectificacion ?? 0,

                             }
                    ).AsQueryable();

                if (!string.IsNullOrEmpty(pstrFilterExpression))
                {
                    query = query.Where(pstrFilterExpression);
                }
                if (!string.IsNullOrEmpty(pstrSortExpression))
                {
                    query = query.OrderBy(pstrSortExpression);
                }

                var objData = query.ToList();
                objData.AsParallel().ToList().ForEach(compra =>
                {
                    var RegOrigen = compra.NroRegistroOrigen.Any() ? compra.NroRegistroOrigen.FirstOrDefault() : null;
                    var Importacion = compra.NroRegistroOrigen.Any()
                        ? dbContext.importacion.Where(i => i.v_IdImportacion == RegOrigen).FirstOrDefault()
                        : null;
                    compra.CtasVistaRapida = string.Join(", ", compra.ListaCuentasUsadas.Distinct());
                    compra.Origen = Importacion != null
                        ? "I" + " " + Importacion.v_Mes + " " + Importacion.v_Correlativo
                        : "";
                    compra.v_Rectificacion = compra.i_AplicaRectificacion == 1 ? "SI" : "NO";
                });


                pobjOperationResult.Success = 1;
                if (!SoloCompraDesdeImportacion)
                {
                    return objData;
                }
                else
                {
                    return objData.Where(l => l.v_IdDocumentoReferencia != null).ToList();
                };
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }


        public List<KeyValueDTO> BuscarProveedoresParaCombo(ref OperationResult pobjOperationResult,
            string pstrRucRazonSocial, string Flag)
        {
            try
            {
                var dbcontext = new SAMBHSEntitiesModelWin();
                var query = from n in dbcontext.cliente
                            where n.i_Eliminado == 0 &&
                                  n.v_PrimerNombre.Contains(pstrRucRazonSocial) | n.v_SegundoNombre.Contains(pstrRucRazonSocial) |
                                  n.v_ApeMaterno.Contains(pstrRucRazonSocial)
                                  | n.v_ApePaterno.Contains(pstrRucRazonSocial) | n.v_RazonSocial.Contains(pstrRucRazonSocial)
                                  && n.v_FlagPantalla == Flag && pstrRucRazonSocial.Trim() != string.Empty
                            orderby n.v_RazonSocial ascending
                            select new
                            {
                                n.v_IdCliente,
                                v_RazonSocial =
                                    (n.v_ApePaterno + " " + n.v_ApeMaterno + " " + n.v_PrimerNombre + " " + n.v_RazonSocial)
                                        .Trim(),
                                n.v_NroDocIdentificacion
                            };

                var query2 = query.AsEnumerable()
                    .Select(x => new KeyValueDTO
                    {
                        Id = x.v_IdCliente,
                        Value1 = x.v_NroDocIdentificacion + " | " + x.v_RazonSocial
                    }).ToList();

                return query2;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Reporte

        private List<string> DevolverNombreEmpresaPropietaria()
        {
            var objOperationResult = new OperationResult();
            var Retonar = new List<string>();
            var objNodeBL = new NodeBL();
            var _intNodeId = int.Parse(Utils.Windows.GetApplicationConfigValue("NodeId"));

            var x = objNodeBL.GetNodeByNodeId(ref objOperationResult, _intNodeId);
            Retonar.Add(x.v_RazonSocial);
            Retonar.Add(x.v_RUC);
            return Retonar;
        }

        public List<ReporteRegistroCompraProveedorResumen> ReporteRegistroCompraProveedorResumen(
            ref OperationResult objOperationResult, int pstri_IdEstablecimiento, DateTime? pstrt_FechaRegistroIni,
            DateTime? pstrt_FechaRegistroFin, int pintTipoDocumentoId, string pstrt_IdProveedor, string pstrt_NroCuenta,
            string pstrt_Orden, string pstr_grupollave, string pstr_Nombregrupollave, int ConsiderarDocInternos, string CodigoProducto)
        {
            //mon.IsActive = true;
            try
            {
                var dbContext = new SAMBHSEntitiesModelWin();
                //1. query para obtener toda la data filtrada por los parametros

                #region Query

                var query =
                    (from A in dbContext.compra
                     join B in dbContext.cliente on new { IdCliente = A.v_IdProveedor, eliminado = 0 } equals
                         new { IdCliente = B.v_IdCliente, eliminado = B.i_Eliminado.Value } into B_join
                     from B in B_join.DefaultIfEmpty()
                     join C in dbContext.compradetalle on new { IdCompra = A.v_IdCompra, eliminado = 0 } equals
                         new { IdCompra = C.v_IdCompra, eliminado = C.i_Eliminado.Value } into C_join
                     from C in C_join.DefaultIfEmpty()
                     join D in dbContext.productodetalle on
                         new { IdProductoDetalle = C.v_IdProductoDetalle, eliminado = 0 } equals
                         new { IdProductoDetalle = D.v_IdProductoDetalle, eliminado = D.i_Eliminado.Value } into D_join
                     from D in D_join.DefaultIfEmpty()
                     join E in dbContext.producto on new { IdProducto = D.v_IdProducto, eliminado = 0 } equals
                         new { IdProducto = E.v_IdProducto, eliminado = E.i_Eliminado.Value } into E_join
                     from E in E_join.DefaultIfEmpty()
                     join F in dbContext.asientocontable on new { NroCuenta = C.v_NroCuenta, eliminado = 0, p = periodo } equals
                         new { NroCuenta = F.v_NroCuenta, eliminado = F.i_Eliminado.Value, p = F.v_Periodo } into F_join
                     from F in F_join.DefaultIfEmpty()

                     join G in dbContext.documento on new { doc = A.i_IdTipoDocumento.Value, eliminado = 0 } equals new { doc = G.i_CodigoDocumento, eliminado = G.i_Eliminado.Value } into G_join
                     from G in G_join.DefaultIfEmpty()
                     where
                         A.i_Eliminado == 0 && A.t_FechaRegistro >= pstrt_FechaRegistroIni &&
                         A.t_FechaRegistro <= pstrt_FechaRegistroFin && A.i_IdEstado == 1 &&
                         (A.i_IdTipoDocumento == pintTipoDocumentoId || pintTipoDocumentoId == -1) &&
                         (B.v_CodCliente == pstrt_IdProveedor || pstrt_IdProveedor == "") &&
                         (C.v_NroCuenta == pstrt_NroCuenta || pstrt_NroCuenta == "") &&
                         (E.v_CodInterno == CodigoProducto || CodigoProducto == "")

                     //orderby pstrt_Orden ascending
                     select new ReporteRegistroCompraProveedorResumen
                     {
                         NombreProveedor =
                             pstr_grupollave == ""
                                 ? (B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " +
                                    B.v_SegundoNombre + " " + B.v_RazonSocial).Trim()
                                 : B.v_CodCliente.Trim() + " / " +
                                   (B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " +
                                    B.v_SegundoNombre + " " + B.v_RazonSocial).Trim() + " / " +
                                   B.v_NroDocIdentificacion.Trim(),
                         NroDocProveedor = B.v_NroDocIdentificacion,
                         IdProveedor = B.v_CodCliente,

                         IdTipoDocumento = A.i_IdTipoDocumento.Value,
                         TipoCambio = A.d_TipoCambio.Value,
                         IdMoneda = A.i_IdMoneda.Value,
                         DetalleCodigoLlave =
                             pstr_grupollave == "NROCUENTA"
                                 ? C.v_NroCuenta == null
                                     ? "** NO EXISTE " + pstr_Nombregrupollave + " **"
                                     : C.v_NroCuenta + B.v_CodCliente
                                 : "" + pstr_grupollave == "NOMBREPRODUCTO"
                                     ? E.v_CodInterno == null
                                         ? "** NO EXISTE " + pstr_Nombregrupollave + " **"
                                         : E.v_CodInterno + B.v_CodCliente
                                     : "" + pstr_grupollave == ""
                                         ? B.v_CodCliente == null
                                             ? "** NO EXISTE " + pstr_Nombregrupollave + " **"
                                             : pstr_Nombregrupollave + " " + B.v_CodCliente
                                         : "",
                         DetalleNombreLlave =
                             pstr_grupollave == "NROCUENTA"
                                 ? F.v_NombreCuenta == null ? "" : F.v_NombreCuenta
                                 : "" + pstr_grupollave == "NOMBREPRODUCTO"
                                     ? E.v_Descripcion == null ? "" : E.v_Descripcion
                                     : "" + pstr_grupollave == ""
                                         ? B.v_PrimerNombre == null
                                             ? ""
                                             : (B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " +
                                                B.v_SegundoNombre + " " + B.v_RazonSocial).Trim()
                                         : "",
                         DetalleCodigoLlaveAux =
                             pstr_grupollave == "NROCUENTA"
                                 ? C.v_NroCuenta == null
                                     ? "** NO EXISTE " + pstr_Nombregrupollave + " **"
                                     : C.v_NroCuenta
                                 : "" + pstr_grupollave == "NOMBREPRODUCTO"
                                     ? E.v_CodInterno == null
                                         ? "** NO EXISTE " + pstr_Nombregrupollave + " **"
                                         : E.v_CodInterno
                                     : "" + pstr_grupollave == ""
                                         ? B.v_CodCliente == null
                                             ? "** NO EXISTE " + pstr_Nombregrupollave + " **"
                                             : pstr_Nombregrupollave + " " + B.v_CodCliente
                                         : "",
                         NombreGrupo = pstr_Nombregrupollave == "" ? "PROVEEDOR" : pstr_Nombregrupollave,
                         NumeroDocumento = G.v_Siglas + " " + A.v_SerieDocumento + " " + A.v_CorrelativoDocumento,
                         // ValorVenta = pstr_grupollave == "" ? C.d_ValorVenta.Value : C.d_ValorVenta.Value,
                         ValorVenta = pstr_grupollave == "" ? A.i_IdMoneda == (int)Currency.Soles ? G.i_UsadoDocumentoInverso == 1 ? A.d_ValorVenta.Value * -1 : A.d_ValorVenta.Value : G.i_UsadoDocumentoInverso == 1 ? (A.d_ValorVenta.Value * A.d_TipoCambio.Value) * -1 : A.d_ValorVenta.Value * A.d_TipoCambio.Value : A.i_IdMoneda == (int)Currency.Soles ? G.i_UsadoDocumentoInverso == 1 ? C.d_ValorVenta.Value * -1 : C.d_ValorVenta.Value : G.i_UsadoDocumentoInverso == 1 ? (C.d_ValorVenta.Value * A.d_TipoCambio.Value) * -1 : C.d_ValorVenta.Value * A.d_TipoCambio.Value,
                         ValorVentaD = pstr_grupollave == "" ? A.i_IdMoneda == (int)Currency.Dolares ? G.i_UsadoDocumentoInverso == 1 ? A.d_ValorVenta.Value * -1 : A.d_ValorVenta.Value : G.i_UsadoDocumentoInverso == 1 ? (A.d_ValorVenta.Value / A.d_TipoCambio.Value) * -1 : A.d_ValorVenta.Value / A.d_TipoCambio.Value : A.i_IdMoneda == (int)Currency.Dolares ? G.i_UsadoDocumentoInverso == 1 ? C.d_ValorVenta.Value * -1 : C.d_ValorVenta.Value : G.i_UsadoDocumentoInverso == 1 ? (C.d_ValorVenta.Value / A.d_TipoCambio.Value) * -1 : C.d_ValorVenta.Value / A.d_TipoCambio.Value,
                         AgruparPor = pstr_grupollave == "" ? G.v_Siglas + " " + A.v_SerieDocumento + " " + A.v_CorrelativoDocumento : pstr_grupollave == "NROCUENTA" ? C.v_NroCuenta : E.v_CodInterno,
                     }).ToList();


                //2.- Query para hacer càculos en memoria
                var query1 = new List<ReporteRegistroCompraProveedorResumen>();
                List<ReporteRegistroCompraProveedorResumen> query10 = new List<ReporteRegistroCompraProveedorResumen>();

                if (pstr_grupollave == "")
                {
                    query10 = query.GroupBy(l => new { prov = l.IdProveedor, doc = l.NumeroDocumento }).Select(o => o.FirstOrDefault()).ToList();

                }
                else
                {
                    query10 = query;

                }

                if (ConsiderarDocInternos == 1)
                {
                    if (pstr_grupollave == "")
                    {
                        query1 = query10.ToList().GroupBy(o => new { prov = o.IdProveedor }).ToList().Select(p =>
                            {

                                var h = p.FirstOrDefault();
                                h.ValorVenta = p.Sum(l => l.ValorVenta);
                                h.ValorVentaD = p.Sum(l => l.ValorVentaD);
                                return h;

                            }).ToList();
                    }
                    else
                    {
                        query1 = query10.ToList().GroupBy(o => new { prov = o.IdProveedor, prod = o.AgruparPor }).ToList().Select(p =>
                        {

                            var h = p.FirstOrDefault();
                            h.ValorVenta = p.Sum(l => l.ValorVenta);
                            h.ValorVentaD = p.Sum(l => l.ValorVentaD);
                            return h;

                        }).ToList();


                    }
                }
                else
                {
                    query = query10.ToList().Where(x => _objDocumentoBL.DocumentoEsContable(x.IdTipoDocumento)).ToList();

                    if (pstr_grupollave == "")
                    {
                        query1 = query.ToList().GroupBy(o => new { prov = o.IdProveedor }).ToList().Select(p =>
                        {

                            var h = p.FirstOrDefault();
                            h.ValorVenta = p.Sum(l => l.ValorVenta);
                            h.ValorVentaD = p.Sum(l => l.ValorVentaD);
                            return h;

                        }).ToList();
                    }
                    else
                    {
                        query1 = query.ToList().GroupBy(o => new { prov = o.IdProveedor, prod = o.AgruparPor }).ToList().Select(p =>
                        {

                            var h = p.FirstOrDefault();
                            h.ValorVenta = p.Sum(l => l.ValorVenta);
                            h.ValorVentaD = p.Sum(l => l.ValorVentaD);
                            return h;

                        }).ToList();


                    }


                }
                var objData = new List<ReporteRegistroCompraProveedorResumen>();
                // 3.- Query para agrupary ordenar 
                if (pstr_grupollave == "")
                {
                    objData = query1.GroupBy(x => new { x.IdProveedor })
                        .Select(group => group.First())
                        //.OrderByDescending(o => pstrt_Orden == "ValorVenta" ? o.ValorVentaDetalle : o.ValorVenta).ToList();
                        .OrderByDescending(o => pstrt_Orden).ToList();
                }
                else
                {
                    objData = query1.GroupBy(x => new { x.DetalleCodigoLlave })
                        .Select(group => group.First())
                        .OrderByDescending(o => pstrt_Orden).ToList();
                }

                #endregion

                objOperationResult.Success = 1;
                return objData.ToList();
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;
            }
        }

        public List<ReporteRegistroCompraProveedorAnalitico> ReporteRegistroCompraProveedorAnalitico(
            ref OperationResult objOperationResult, int pstri_IdEstablecimiento, DateTime? pstrt_FechaRegistroIni,
            DateTime? pstrt_FechaRegistroFin, int pintTipoDocumentoId, string pstrt_IdProveedor, string pstrt_NroCuenta,
            string pstrt_Orden, string pstr_grupollave, string pstr_Nombregrupollave, int ConsideraDocInternos,
            string IdMarca, string IdLinea, string CodProducto)
        {
            //mon.IsActive = true;


            try
            {
                objOperationResult.Success = 1;
                var dbContext = new SAMBHSEntitiesModelWin();

                #region Query

                var query =
                    from A in dbContext.compra
                    join B in dbContext.cliente on new { IdProveedor = A.v_IdProveedor, eliminado = 0, Flag = "V" } equals
                        new { IdProveedor = B.v_IdCliente, eliminado = B.i_Eliminado.Value, Flag = B.v_FlagPantalla } into
                        B_join
                    from B in B_join.DefaultIfEmpty()
                    join C in dbContext.compradetalle on new { IdCompra = A.v_IdCompra, eliminado = 0 } equals
                        new { IdCompra = C.v_IdCompra, eliminado = C.i_Eliminado.Value } into C_join
                    from C in C_join.DefaultIfEmpty()
                    join D in dbContext.datahierarchy on new { NroCuenta = C.v_NroCuenta, eliminado = 0, Grupo = 52 }
                        equals new { NroCuenta = D.v_Value2, eliminado = D.i_IsDeleted.Value, Grupo = D.i_GroupId } into
                        D_join
                    from D in D_join.DefaultIfEmpty()
                    join E in dbContext.documento on new { IdTipoDocumento = A.i_IdTipoDocumento.Value } equals
                        new { IdTipoDocumento = E.i_CodigoDocumento } into E_join
                    from E in E_join.DefaultIfEmpty()
                    join F in dbContext.datahierarchy on new { IdMoneda = A.i_IdMoneda.Value, Grupo = 18 } equals
                        new { IdMoneda = F.i_ItemId, Grupo = F.i_GroupId } into F_join
                    from F in F_join.DefaultIfEmpty()
                    join J in dbContext.productodetalle on new { C.v_IdProductoDetalle } equals
                        new { J.v_IdProductoDetalle } into J_join
                    from J in J_join.DefaultIfEmpty()
                    join K in dbContext.producto on new { IdProducto = J.v_IdProducto } equals
                        new { IdProducto = K.v_IdProducto } into K_join
                    from K in K_join.DefaultIfEmpty()
                    join G in dbContext.datahierarchy on new { IdUnidadMedida = C.i_IdUnidadMedida.Value, Grupo = 17 }
                        equals new { IdUnidadMedida = G.i_ItemId, Grupo = G.i_GroupId } into G_join
                    from G in G_join.DefaultIfEmpty()
                    where
                        A.i_Eliminado == 0 && A.t_FechaRegistro >= pstrt_FechaRegistroIni &&
                        A.t_FechaRegistro <= pstrt_FechaRegistroFin && A.i_IdEstado == 1 &&
                        (A.i_IdTipoDocumento == pintTipoDocumentoId || pintTipoDocumentoId == -1) &&
                        (B.v_CodCliente == pstrt_IdProveedor || pstrt_IdProveedor == "") &&
                        (C.v_NroCuenta == pstrt_NroCuenta || pstrt_NroCuenta == "") &&
                        (K.v_IdLinea == IdLinea || IdLinea == "-1") && (K.v_IdMarca == IdMarca || IdMarca == "-1") &&
                        (K.v_CodInterno == CodProducto || CodProducto == "")
                    orderby A.d_Total descending
                    select new ReporteRegistroCompraProveedorAnalitico
                    {
                        IdProveedor = B.v_CodCliente,
                        NombreProveedor =
                            B.v_CodCliente.Trim() + " " +
                            (B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " + B.v_SegundoNombre +
                             " " + B.v_RazonSocial).Trim(),
                        NroDocProveedor = B.v_NroDocIdentificacion,
                        FechaRegistro = A.t_FechaEmision.Value,
                        IdTipoDocumento = A.i_IdTipoDocumento.Value,
                        TipoDocumento = E.v_Siglas,
                        Documento = E.v_Siglas + " " + A.v_SerieDocumento.Trim() + "-" + A.v_CorrelativoDocumento.Trim(),
                        IdProducto = K.v_CodInterno,
                        NombreProducto = K.v_Descripcion,
                        NroCuenta = C.v_NroCuenta,
                        CantidadDetalle = C.d_Cantidad.Value,
                        PrecioDetalle = C.d_Precio.Value,
                        ValorVentaDetalle = C.d_ValorVenta.Value,
                        TipoCambio = A.d_TipoCambio.Value,
                        IdMoneda = A.i_IdMoneda.Value,
                        CorrelativoDocumento = A.v_CorrelativoDocumento,
                        Grupollave =
                            pstr_grupollave == "NROCUENTA"
                                ? C.v_NroCuenta == null
                                    ? "** " + pstr_Nombregrupollave + " NO EXISTE **"
                                    : "CUENTA : " + C.v_NroCuenta.Trim()
                                : "" + pstr_grupollave == "NOMBREPROVEEDOR"
                                    ? B.v_CodCliente == null
                                        ? "** " + pstr_Nombregrupollave + " NO EXISTE **"
                                        : pstr_Nombregrupollave + " : " +
                                          (B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " +
                                           B.v_SegundoNombre + " " + B.v_RazonSocial).Trim()
                                    : ""
                    };


                if (ConsideraDocInternos == 1)
                {
                    var query1 = (from A in query.ToList()
                                  let ValorCompra =
                                      CalcularCompraDetalle(A.IdTipoDocumento, A.PrecioDetalle, A.ValorVentaDetalle, 1,
                                          A.PrecioDetalle, A.TipoCambio, A.IdMoneda)
                                  select new ReporteRegistroCompraProveedorAnalitico
                                  {
                                      IdProveedor = A.IdProveedor,
                                      NombreProveedor = A.NombreProveedor,
                                      NroDocProveedor = A.NroDocProveedor,
                                      FechaRegistro = A.FechaRegistro,
                                      IdTipoDocumento = A.IdTipoDocumento,
                                      TipoDocumento = A.TipoDocumento,
                                      Documento = A.Documento,
                                      IdProducto = A.IdProducto,
                                      NombreProducto = A.NombreProducto,
                                      NroCuenta = A.NroCuenta,
                                      CantidadDetalle = A.CantidadDetalle,
                                      PrecioDetalle = ValorCompra.PrecioDetalleSoles,
                                      // ValorVentaDetalle = ValorCompra.ValorCompraSoles,
                                      ValorVentaDetalle = ValorCompra.ValorCompraDetalleSoles,
                                      PrecioDetalleD = ValorCompra.PrecioDetalleDolares,
                                      ValorVentaDetalleD = ValorCompra.ValorCompraDetalleDolares,
                                      TipoCambio = A.TipoCambio,
                                      IdMoneda = A.IdMoneda,
                                      CorrelativoDocumento = A.CorrelativoDocumento,
                                      Grupollave = A.Grupollave
                                  }).ToList().AsQueryable();


                    if (!string.IsNullOrEmpty(pstrt_Orden))
                    {
                        query1 = query1.OrderBy(pstrt_Orden);
                    }


                    return query1.ToList();
                }
                else
                {
                    var query1 =
                        (from A in query.ToList().Where(x => _objDocumentoBL.DocumentoEsContable(x.IdTipoDocumento))
                         let ValorCompra =
                             CalcularCompraDetalle(A.IdTipoDocumento, A.PrecioDetalle, A.ValorVentaDetalle, 1,
                                 A.PrecioDetalle, A.TipoCambio, A.IdMoneda)
                         select new ReporteRegistroCompraProveedorAnalitico
                         {
                             IdProveedor = A.IdProveedor,
                             NombreProveedor = A.NombreProveedor,
                             NroDocProveedor = A.NroDocProveedor,
                             FechaRegistro = A.FechaRegistro,
                             IdTipoDocumento = A.IdTipoDocumento,
                             TipoDocumento = A.TipoDocumento,
                             Documento = A.Documento,
                             IdProducto = A.IdProducto,
                             NombreProducto = A.NombreProducto,
                             NroCuenta = A.NroCuenta,
                             CantidadDetalle = A.CantidadDetalle,
                             PrecioDetalle = ValorCompra.PrecioDetalleSoles,
                             // ValorVentaDetalle = ValorCompra.ValorCompraSoles,
                             ValorVentaDetalle = ValorCompra.ValorCompraDetalleSoles,
                             PrecioDetalleD = ValorCompra.PrecioDetalleDolares,
                             ValorVentaDetalleD = ValorCompra.ValorCompraDetalleDolares,
                             TipoCambio = A.TipoCambio,
                             IdMoneda = A.IdMoneda,
                             CorrelativoDocumento = A.CorrelativoDocumento,
                             Grupollave = A.Grupollave
                         }).ToList().AsQueryable();

                    if (!string.IsNullOrEmpty(pstrt_Orden))
                    {
                        query1 = query1.OrderBy(pstrt_Orden);
                    }
                    return query1.ToList();
                }

                #endregion
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;
            }
        }

        private valoresRegistroCompra CalcularCompra(int pintTipoDocumento, decimal pdecValorCompra, decimal pdecIgv,
            decimal pdecTotal, decimal pdecAnticipio, decimal pdecTipoCambio, int pintMonedaId)
        {
            var objvaloresRegistroCompra = new valoresRegistroCompra();
            if (pdecTipoCambio == 0 || pdecTipoCambio == null)
            {
                pdecTipoCambio = 1;
            }
            if (_objDocumentoBL.DocumentoEsInverso(pintTipoDocumento))
            {
                if (pintMonedaId == (int)Currency.Soles)
                {
                    objvaloresRegistroCompra.ValorCompraSoles = pdecValorCompra * -1;
                    objvaloresRegistroCompra.ValorCompraDolares = pdecValorCompra * -1 / pdecTipoCambio;

                    objvaloresRegistroCompra.IgvSoles = pdecIgv * -1;
                    objvaloresRegistroCompra.IgvDolares = pdecIgv * -1 / pdecTipoCambio;

                    objvaloresRegistroCompra.TotalSoles = pdecTotal * -1;
                    objvaloresRegistroCompra.TotalDolares = pdecTotal * -1 / pdecTipoCambio;

                    objvaloresRegistroCompra.AnticipioSoles = pdecAnticipio * -1;
                    objvaloresRegistroCompra.AnticipioDolares = pdecAnticipio * -1 / pdecTipoCambio;
                }
                else if (pintMonedaId == (int)Currency.Dolares)
                {
                    objvaloresRegistroCompra.ValorCompraSoles = pdecValorCompra * -1 * pdecTipoCambio;
                    objvaloresRegistroCompra.ValorCompraDolares = pdecValorCompra * -1;

                    objvaloresRegistroCompra.IgvSoles = pdecIgv * -1 * pdecTipoCambio;
                    objvaloresRegistroCompra.IgvDolares = pdecIgv * -1;

                    objvaloresRegistroCompra.TotalSoles = pdecTotal * -1 * pdecTipoCambio;
                    objvaloresRegistroCompra.TotalDolares = pdecTotal * -1;

                    objvaloresRegistroCompra.AnticipioSoles = pdecAnticipio * -1 * pdecTipoCambio;
                    objvaloresRegistroCompra.AnticipioDolares = pdecAnticipio * -1;
                }
            }
            else
            {
                if (pintMonedaId == (int)Currency.Soles)
                {
                    objvaloresRegistroCompra.ValorCompraSoles = pdecValorCompra;
                    objvaloresRegistroCompra.ValorCompraDolares = pdecValorCompra / pdecTipoCambio;

                    objvaloresRegistroCompra.IgvSoles = pdecIgv;
                    objvaloresRegistroCompra.IgvDolares = pdecIgv / pdecTipoCambio;

                    objvaloresRegistroCompra.TotalSoles = pdecTotal;
                    objvaloresRegistroCompra.TotalDolares = pdecTotal / pdecTipoCambio;

                    objvaloresRegistroCompra.AnticipioSoles = pdecAnticipio;
                    objvaloresRegistroCompra.AnticipioDolares = pdecAnticipio / pdecTipoCambio;
                }
                else if (pintMonedaId == (int)Currency.Dolares)
                {
                    objvaloresRegistroCompra.ValorCompraSoles = pdecValorCompra * pdecTipoCambio;
                    objvaloresRegistroCompra.ValorCompraDolares = pdecValorCompra;

                    objvaloresRegistroCompra.IgvSoles = pdecIgv * pdecTipoCambio;
                    objvaloresRegistroCompra.IgvDolares = pdecIgv;

                    objvaloresRegistroCompra.TotalSoles = pdecTotal * pdecTipoCambio;
                    objvaloresRegistroCompra.TotalDolares = pdecTotal;

                    objvaloresRegistroCompra.AnticipioSoles = pdecAnticipio * pdecTipoCambio;
                    objvaloresRegistroCompra.AnticipioDolares = pdecAnticipio;
                }
            }
            return objvaloresRegistroCompra;
        }

        private valoresRegistroCompra CalcularCompraDetalle(int pintTipoDocumento, decimal pdecPrecioDetalle,
            decimal pdecValorVentaDetalle, decimal pdecIgvDetalle, decimal pdecPrecioVentaDetalle,
            decimal pdecTipoCambio, int pintMonedaId)
        {
            var objvaloresRegistroCompra = new valoresRegistroCompra();
            if (pdecTipoCambio == 0 || pdecTipoCambio == null)
            {
                pdecTipoCambio = 1;
            }
            if (_objDocumentoBL.DocumentoEsInverso(pintTipoDocumento))
            {
                if (pintMonedaId == (int)Currency.Soles)
                {
                    objvaloresRegistroCompra.PrecioDetalleSoles = pdecPrecioDetalle * -1;
                    objvaloresRegistroCompra.PrecioDetalleDolares = pdecPrecioDetalle * -1 / pdecTipoCambio;


                    objvaloresRegistroCompra.ValorCompraDetalleSoles = pdecValorVentaDetalle * -1;
                    objvaloresRegistroCompra.ValorCompraDetalleDolares = pdecValorVentaDetalle * -1 / pdecTipoCambio;


                    objvaloresRegistroCompra.PrecioCompraDetalleSoles = pdecPrecioVentaDetalle * -1;
                    objvaloresRegistroCompra.PrecioCompraDetalleDolares = pdecPrecioVentaDetalle * -1 / pdecTipoCambio;

                    objvaloresRegistroCompra.IgvDetalleSoles = pdecIgvDetalle * -1;
                    objvaloresRegistroCompra.IgvDetalleDolares = pdecIgvDetalle * -1 / pdecTipoCambio;
                }
                else if (pintMonedaId == (int)Currency.Dolares)
                {
                    objvaloresRegistroCompra.PrecioDetalleSoles = pdecPrecioDetalle * -1 * pdecTipoCambio;
                    objvaloresRegistroCompra.PrecioDetalleDolares = pdecPrecioDetalle * -1;
                    objvaloresRegistroCompra.ValorCompraDetalleSoles = pdecValorVentaDetalle * -1 * pdecTipoCambio;
                    objvaloresRegistroCompra.ValorCompraDetalleDolares = pdecValorVentaDetalle * -1;

                    objvaloresRegistroCompra.PrecioCompraDetalleSoles = pdecPrecioVentaDetalle * -1 * pdecTipoCambio;
                    objvaloresRegistroCompra.PrecioCompraDetalleDolares = pdecPrecioVentaDetalle * -1;

                    objvaloresRegistroCompra.IgvDetalleSoles = pdecIgvDetalle * -1 * pdecTipoCambio;
                    objvaloresRegistroCompra.IgvDetalleDolares = pdecIgvDetalle * -1;
                }
            }
            else
            {
                if (pintMonedaId == (int)Currency.Soles)
                {
                    objvaloresRegistroCompra.PrecioDetalleSoles = pdecPrecioDetalle;
                    objvaloresRegistroCompra.PrecioDetalleDolares = pdecPrecioDetalle / pdecTipoCambio;


                    objvaloresRegistroCompra.ValorCompraDetalleSoles = pdecValorVentaDetalle;
                    objvaloresRegistroCompra.ValorCompraDetalleDolares = pdecValorVentaDetalle / pdecTipoCambio;


                    objvaloresRegistroCompra.PrecioCompraDetalleSoles = pdecPrecioVentaDetalle;
                    objvaloresRegistroCompra.PrecioCompraDetalleDolares = pdecPrecioVentaDetalle / pdecTipoCambio;

                    objvaloresRegistroCompra.IgvDetalleSoles = pdecIgvDetalle;
                    objvaloresRegistroCompra.IgvDetalleDolares = pdecIgvDetalle / pdecTipoCambio;
                }
                else if (pintMonedaId == (int)Currency.Dolares)
                {
                    objvaloresRegistroCompra.PrecioDetalleSoles = pdecPrecioDetalle * pdecTipoCambio;
                    objvaloresRegistroCompra.PrecioDetalleDolares = pdecPrecioDetalle;


                    objvaloresRegistroCompra.ValorCompraDetalleSoles = pdecValorVentaDetalle * pdecTipoCambio;
                    objvaloresRegistroCompra.ValorCompraDetalleDolares = pdecValorVentaDetalle;


                    objvaloresRegistroCompra.PrecioCompraDetalleSoles = pdecPrecioVentaDetalle * pdecTipoCambio;
                    objvaloresRegistroCompra.PrecioCompraDetalleDolares = pdecPrecioVentaDetalle;

                    objvaloresRegistroCompra.IgvDetalleSoles = pdecIgvDetalle * pdecTipoCambio;
                    objvaloresRegistroCompra.IgvDetalleDolares = pdecIgvDetalle;
                }
            }
            return objvaloresRegistroCompra;
        }

        private List<decimal> CalcularSumaCabecera(List<ReporteRegistroCompraProveedorResumen> Lista,
            string pstrIdProveedor)
        {
            var ValorCompra = new valoresRegistroCompra();

            var Retonar = new List<decimal>();
            var xx = Lista.Where(p => p.IdProveedor == pstrIdProveedor);

            decimal ValorVenta;
            ValorVenta = xx.Sum(x => x.ValorVenta);
            //(A.IdTipoDocumento, A.ValorVenta, 1, 1, 1, A.TipoCambio, A.IdMoneda)
            ValorCompra = CalcularCompra(Lista.FirstOrDefault().IdTipoDocumento, ValorVenta, 1, 1, 1,
                Lista.FirstOrDefault().TipoCambio, Lista.FirstOrDefault().IdMoneda);
            Retonar.Add(ValorCompra.ValorCompraSoles);
            Retonar.Add(ValorCompra.ValorCompraDolares);
            return Retonar;
        }

        private List<decimal> CalcularSumaProducto(List<ReporteRegistroCompraProveedorResumen> Lista,
            string DetalleCodigoLlave)
        {
            var ValorCompra = new valoresRegistroCompra();

            var Retonar = new List<decimal>();
            var xx = Lista.Where(p => p.DetalleCodigoLlave == DetalleCodigoLlave);

            decimal ValorVenta;
            ValorVenta =
                xx.ToList().Where(x => !_objDocumentoBL.DocumentoEsInverso(x.IdTipoDocumento)).Sum(x => x.ValorVenta) -
                xx.ToList().Where(x => _objDocumentoBL.DocumentoEsInverso(x.IdTipoDocumento)).Sum(x => x.ValorVenta);
            ValorCompra = CalcularCompra(Lista.FirstOrDefault().IdTipoDocumento, ValorVenta, 1, 1, 1,
                Lista.FirstOrDefault().TipoCambio, Lista.FirstOrDefault().IdMoneda);
            Retonar.Add(ValorCompra.ValorCompraSoles);
            Retonar.Add(ValorCompra.ValorCompraDolares);
            return Retonar;
        }

        public List<ReporteRegistroCompraProductoAnalitico> ReporteRegistroCompraProductoAnalitico(
            ref OperationResult objOperationResult, int pstri_IdEstablecimiento, DateTime? pstrt_FechaRegistroIni,
            DateTime? pstrt_FechaRegistroFin, int pintTipoDocumentoId, string pstrt_IdProveedor, string pstrt_NroCuenta,
            string pstrt_Orden, string pstr_grupollave, string pstr_Nombregrupollave, int ConsideraDocInternos,
            string IdLinea, string IdMarca, string CodigoProducto)
        {
            //mon.IsActive = true;
            try
            {
                var dbContext = new SAMBHSEntitiesModelWin();
                objOperationResult.Success = 1;

                #region Query

                var query =
                    from A in dbContext.compra
                    join B in dbContext.cliente on new { IdProveedor = A.v_IdProveedor, eliminado = 0 } equals
                        new { IdProveedor = B.v_IdCliente, eliminado = B.i_Eliminado.Value } into B_join
                    from B in B_join.DefaultIfEmpty()
                    join C in dbContext.compradetalle on new { IdCompra = A.v_IdCompra, eliminado = 0 } equals
                        new { IdCompra = C.v_IdCompra, eliminado = C.i_Eliminado.Value } into C_join
                    from C in C_join.DefaultIfEmpty()
                    join D in dbContext.datahierarchy on new { NroCuenta = C.v_NroCuenta, Grupo = 48 } equals
                        new { NroCuenta = D.v_Value2, Grupo = D.i_GroupId } into D_join
                    from D in D_join.DefaultIfEmpty()
                    join E in dbContext.documento on new { TipoDoc = A.i_IdTipoDocumento.Value, eliminado = 0 } equals
                        new { TipoDoc = E.i_CodigoDocumento, eliminado = E.i_Eliminado.Value } into E_join
                    from E in E_join.DefaultIfEmpty()
                    join F in dbContext.datahierarchy on new { IdMoneda = A.i_IdMoneda.Value, Grupo = 18 } equals
                        new { IdMoneda = F.i_ItemId, Grupo = F.i_GroupId } into F_join
                    from F in F_join.DefaultIfEmpty()
                    join J in dbContext.productodetalle on new { C.v_IdProductoDetalle, eliminado = 0 }
                        equals new { J.v_IdProductoDetalle, eliminado = J.i_Eliminado.Value } into J_join
                    from J in J_join.DefaultIfEmpty()
                    join K in dbContext.producto on new { J.v_IdProducto, eliminado = 0 }
                        equals new { K.v_IdProducto, eliminado = K.i_Eliminado.Value } into K_join
                    from K in K_join.DefaultIfEmpty()
                    join G in dbContext.datahierarchy on new { IdUnidadMedida = C.i_IdUnidadMedida.Value, Grupo = 17 }
                        equals new { IdUnidadMedida = G.i_ItemId, Grupo = G.i_GroupId } into G_join
                    from G in G_join.DefaultIfEmpty()

                    join H in dbContext.datahierarchy on new { igv = A.i_IdIgv.Value, eliminado = 0, Grupo = 27 } equals new { igv = H.i_ItemId, eliminado = H.i_IsDeleted.Value, Grupo = H.i_GroupId } into H_join

                    from H in H_join.DefaultIfEmpty()
                    where
                        A.i_Eliminado == 0 && A.t_FechaRegistro >= pstrt_FechaRegistroIni &&
                        A.t_FechaRegistro <= pstrt_FechaRegistroFin && A.i_IdEstado == 1 &&
                        (A.i_IdTipoDocumento == pintTipoDocumentoId || pintTipoDocumentoId == -1) &&
                        (B.v_CodCliente == pstrt_IdProveedor || pstrt_IdProveedor == "") &&
                        (C.v_NroCuenta == pstrt_NroCuenta || pstrt_NroCuenta == "") &&
                        (K.v_IdLinea == IdLinea || IdLinea == "-1") && (K.v_IdMarca == IdMarca || IdMarca == "-1")
                        && (K.v_CodInterno == CodigoProducto || CodigoProducto == "")
                        && C.v_IdProductoDetalle != null
                        && A.i_IdEstablecimiento == Globals.ClientSession.i_IdEstablecimiento.Value && C.i_IdAlmacen == Globals.ClientSession.i_IdAlmacenPredeterminado.Value
                    orderby A.d_Total descending
                    select new ReporteRegistroCompraProductoAnalitico
                    {
                        FechaRegistro = A.t_FechaRegistro.Value,
                        IdTipoDocumento = A.i_IdTipoDocumento.Value,
                        TipoDocumento = E == null ? "" : E.v_Siglas,
                        Documento = A.v_SerieDocumento.Trim() + "-" + A.v_CorrelativoDocumento.Trim(),
                        IdProducto =
                            K == null
                                ? "** PRODUCTO NO EXISTE **"
                                : " PRODUCTO : " + K.v_CodInterno.Trim() + " " + K.v_Descripcion.Trim(),
                        NombreProducto = K == null ? "" : K.v_Descripcion,
                        NroCuenta = C == null ? "" : C.v_NroCuenta,
                        CantidadDetalle = C == null ? 0 : C.d_Cantidad.Value,
                        PrecioDetalle = C == null ? 0 : C.d_Precio.Value,
                        ValorVentaDetalle = C == null ? 0 : C.d_ValorVenta.Value,
                        TipoCambio = A.d_TipoCambio.Value,
                        IdMoneda = A.i_IdMoneda.Value,
                        CorrelativoDocumento = A.v_CorrelativoDocumento,
                        Grupollave =
                            pstr_grupollave == "NROCUENTA"
                                ? C == null
                                    ? "** " + pstr_Nombregrupollave + " NO EXISTE **"
                                    : pstr_Nombregrupollave + " : " + C.v_NroCuenta
                                : "" + pstr_grupollave == "NOMBREPROVEEDOR"
                                    ? B == null
                                        ? "** " + pstr_Nombregrupollave + " NO EXISTE **"
                                        : pstr_Nombregrupollave + " : " +
                                          (B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " +
                                           B.v_SegundoNombre + " " + B.v_RazonSocial).Trim()
                                    : "",
                        IdProveedor = B == null ? "" : B.v_CodCliente,
                        NombreProveedor =
                            B == null
                                ? "**CLIENTE ELIMINADO**"
                                : (B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " +
                                   B.v_SegundoNombre + " " + B.v_RazonSocial).Trim(),
                        NroDocProveedor = B == null ? "**CLIENTE ELIMINADO**" : B.v_NroDocIdentificacion,
                        Igv = H.v_Value1,
                        i_PreciosIncluyenIgv = A.i_PreciosIncluyenIgv.Value,

                        PrecioDetalleIncIgvSoles = A.i_IdMoneda == (int)Currency.Soles ? C.d_Precio.Value : C.d_Precio.Value * A.d_TipoCambio.Value,
                        NroRegistro = A.v_Mes + "-" + A.v_Correlativo,
                    };


                query = query.OrderBy(pstrt_Orden);
                if (ConsideraDocInternos == 1)
                {
                    var query1 = from A in query.ToList()
                                 let ValorCompra =
                                     CalcularCompraDetalle(A.IdTipoDocumento, A.i_PreciosIncluyenIgv == 1 ? A.PrecioDetalle / (1 + decimal.Parse(A.Igv) / 100) : A.PrecioDetalle, A.ValorVentaDetalle, 1,
                                         A.PrecioDetalle, A.TipoCambio, A.IdMoneda)
                                 select new ReporteRegistroCompraProductoAnalitico
                                 {
                                     FechaRegistro = A.FechaRegistro,
                                     IdTipoDocumento = A.IdTipoDocumento,
                                     TipoDocumento = A.TipoDocumento,
                                     Documento = A.Documento,
                                     IdProducto = A.IdProducto,
                                     NombreProducto = A.NombreProducto,
                                     NroCuenta = A.NroCuenta,
                                     CantidadDetalle = A.CantidadDetalle,
                                     //PrecioDetalle = ValorCompra.PrecioDetalleDolares,
                                     PrecioDetalle = ValorCompra.PrecioDetalleSoles,
                                     ValorVentaDetalle = ValorCompra.ValorCompraDetalleSoles,
                                     PrecioDetalleD = ValorCompra.PrecioDetalleDolares,
                                     ValorVentaDetalleD = ValorCompra.ValorCompraDetalleDolares,
                                     TipoCambio = A.TipoCambio,
                                     IdMoneda = A.IdMoneda,
                                     CorrelativoDocumento = A.CorrelativoDocumento,
                                     Grupollave = A.Grupollave,
                                     IdProveedor = A.IdProveedor,
                                     NombreProveedor = A.NombreProveedor,
                                     NroDocProveedor = A.NroDocProveedor,
                                     PrecioDetalleIncIgvSoles = A.i_PreciosIncluyenIgv == 1 ? A.PrecioDetalleIncIgvSoles : A.PrecioDetalleIncIgvSoles + (A.PrecioDetalleIncIgvSoles * (decimal.Parse(A.Igv) / 100)),

                                     NroRegistro = A.NroRegistro,
                                 };

                    return query1.ToList();
                }
                else
                {
                    var query1 =
                        from A in query.ToList().Where(x => _objDocumentoBL.DocumentoEsContable(x.IdTipoDocumento))
                        let ValorCompra =
                            CalcularCompraDetalle(A.IdTipoDocumento, A.i_PreciosIncluyenIgv == 1 ? A.PrecioDetalle / (1 + decimal.Parse(A.Igv) / 100) : A.PrecioDetalle, A.ValorVentaDetalle, 1,
                                A.PrecioDetalle, A.TipoCambio, A.IdMoneda)
                        select new ReporteRegistroCompraProductoAnalitico
                        {
                            FechaRegistro = A.FechaRegistro,
                            IdTipoDocumento = A.IdTipoDocumento,
                            TipoDocumento = A.TipoDocumento,
                            Documento = A.Documento,
                            IdProducto = A.IdProducto,
                            NombreProducto = A.NombreProducto,
                            NroCuenta = A.NroCuenta,
                            CantidadDetalle = A.CantidadDetalle,
                            PrecioDetalle = ValorCompra.PrecioDetalleSoles,
                            ValorVentaDetalle = ValorCompra.ValorCompraDetalleSoles,
                            PrecioDetalleD = ValorCompra.PrecioDetalleDolares,
                            ValorVentaDetalleD = ValorCompra.ValorCompraDetalleDolares,
                            TipoCambio = A.TipoCambio,
                            IdMoneda = A.IdMoneda,
                            CorrelativoDocumento = A.CorrelativoDocumento,
                            Grupollave = A.Grupollave,
                            IdProveedor = A.IdProveedor,
                            NombreProveedor = A.NombreProveedor,
                            NroDocProveedor = A.NroDocProveedor,
                            PrecioDetalleIncIgvSoles = A.i_PreciosIncluyenIgv == 1 ? A.PrecioDetalleIncIgvSoles : A.PrecioDetalleIncIgvSoles + (A.PrecioDetalleIncIgvSoles * (decimal.Parse(A.Igv) / 100)),
                            NroRegistro = A.NroRegistro,
                        };
                    return query1.ToList();
                }

                #endregion
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;
            }
        }

        public List<ReporteRegistroCompraProductoResumen> ReporteRegistroCompraProductoResumen(
            ref OperationResult objOperationResult, int pstri_IdEstablecimiento, DateTime? pstrt_FechaRegistroIni,
            DateTime? pstrt_FechaRegistroFin, int pintTipoDocumentoId, string pstrt_IdProveedor,
            string pstrt_NroDocIdentificacion, string pstrt_NroCuenta, string pst_IdProducto, string pstrt_Orden,
            string pstr_grupollave, string pstr_Nombregrupollave, int ConsideraDocInternos, string IdLinea,
            string IdMarca)
        {
            //mon.IsActive = true;
            try
            {
                var dbContext = new SAMBHSEntitiesModelWin();
                objOperationResult.Success = 1;

                #region Query

                var query =
                    (from A in dbContext.compra
                     join B in dbContext.cliente on new { IdProveedor = A.v_IdProveedor, eliminado = 0 } equals
                         new { IdProveedor = B.v_IdCliente, eliminado = B.i_Eliminado.Value } into B_join
                     from B in B_join.DefaultIfEmpty()
                     join C in dbContext.compradetalle on new { IdCompra = A.v_IdCompra, eliminado = 0 } equals
                         new { IdCompra = C.v_IdCompra, eliminado = C.i_Eliminado.Value } into C_join
                     from C in C_join.DefaultIfEmpty()
                     join F in dbContext.productodetalle on new { C.v_IdProductoDetalle, eliminado = 0 }
                         equals new { F.v_IdProductoDetalle, eliminado = F.i_Eliminado.Value } into F_join
                     from F in F_join.DefaultIfEmpty()
                     join G in dbContext.producto on new { F.v_IdProducto, eliminado = 0 }
                         equals new { G.v_IdProducto, eliminado = G.i_Eliminado.Value } into G_join
                     from G in G_join.DefaultIfEmpty()
                     join H in dbContext.asientocontable on new { NroCuenta = C.v_NroCuenta, eliminado = 0, p = periodo } equals
                         new { NroCuenta = H.v_NroCuenta, eliminado = H.i_Eliminado.Value, p = H.v_Periodo } into H_join
                     from H in H_join.DefaultIfEmpty()
                     join I in dbContext.datahierarchy on
                         new { MedioPago = A.i_IdCondicionPago.Value, eliminado = 0, Grupo = 23 } equals
                         new { MedioPago = I.i_ItemId, eliminado = I.i_IsDeleted.Value, Grupo = I.i_GroupId } into
                         I_join
                     from I in I_join.DefaultIfEmpty()
                     join J in dbContext.documento on new { TipoDoc = A.i_IdTipoDocumento.Value, eliminado = 0 } equals
                         new { TipoDoc = J.i_CodigoDocumento, eliminado = J.i_Eliminado.Value } into J_join
                     from J in J_join.DefaultIfEmpty()
                     where
                         A.i_Eliminado == 0 && A.t_FechaRegistro >= pstrt_FechaRegistroIni &&
                         A.t_FechaRegistro <= pstrt_FechaRegistroFin && A.i_IdEstado == 1 &&
                         (A.i_IdTipoDocumento == pintTipoDocumentoId || pintTipoDocumentoId == -1) &&
                         (B.v_NroDocIdentificacion == pstrt_NroDocIdentificacion || pstrt_NroDocIdentificacion == "") &&
                         (B.v_CodCliente == pstrt_IdProveedor || pstrt_IdProveedor == "") &&
                         (C.v_NroCuenta == pstrt_NroCuenta || pstrt_NroCuenta == "") &&
                         (G.v_CodInterno == pst_IdProducto || pst_IdProducto == "") &&
                         (G.v_IdLinea == IdLinea || IdLinea == "-1") && (G.v_IdMarca == IdMarca || IdMarca == "-1")

                         && C.v_IdProductoDetalle != null
                     orderby G.v_CodInterno descending
                     select new ReporteRegistroCompraProductoResumen
                     {
                         IdTipoDocumento = A.i_IdTipoDocumento.Value,
                         IdProducto = G == null ? "" : G.v_CodInterno,
                         NombreProducto =
                             G == null
                                 ? "** PRODUCTO NO EXISTE **"
                                 : "PRODUCTO :" + G.v_CodInterno + " " + G.v_Descripcion,
                         CantidadDetalle = C == null ? 0 : C.d_Cantidad.Value,
                         ValorVentaDetalle = C == null ? 0 : C.d_ValorVenta.Value,
                         TipoCambio = A.d_TipoCambio.Value,
                         IdMoneda = A.i_IdMoneda.Value,
                         DetalleCodigoLlave =
                             pstr_grupollave == "NROCUENTA"
                                 ? H == null
                                     ? "** NO EXISTE " + pstr_Nombregrupollave + " **"
                                     : "CUENTA :" + H.v_NroCuenta + " " + H.v_NombreCuenta
                                 : "" + pstr_grupollave == "MEDIOPAGO"
                                     ? I == null
                                         ? "** NO EXISTE " + pstr_Nombregrupollave + " **"
                                         : "CONDICIÓN DE PAGO :" + I.v_Value1
                                     : pstr_grupollave == "NRODOCUMENTOCOBRANZA"
                                         ? J == null
                                             ? "** TIPO DE COMPROBANTE NO EXISTE **"
                                             : "TIPO DE COMPROBANTE :" + J.v_Nombre
                                         : "",
                         DetalleNombreLlave = "",
                         DetalleCodigoLlaveAux =
                             pstr_grupollave == "NROCUENTA"
                                 ? H == null
                                     ? "** NO EXISTE " + pstr_Nombregrupollave + " **"
                                     : "CUENTA :" + H.v_NroCuenta + " " + H.v_NombreCuenta
                                 : "" + pstr_grupollave == "MEDIOPAGO"
                                     ? I == null
                                         ? "** NO EXISTE " + pstr_Nombregrupollave + " **"
                                         : "CONDICIÓN DE PAGO :" + I.v_Value1
                                     : pstr_grupollave == "NRODOCUMENTOCOBRANZA"
                                         ? J == null
                                             ? "** TIPO DE COMPROBANTE NO EXISTE **"
                                             : "TIPO DE COMPROBANTE :" + J.v_Nombre
                                         : "",
                         //DetalleNombreLlave = pstr_grupollave == "NROCUENTA" ? H == null ? "** NO EXISTE " + pstr_Nombregrupollave + " **" : "CUENTA :" + H.v_NombreCuenta + " " + H.v_NombreCuenta : "" + pstr_grupollave == "NOMBREPROVEEDOR" ? B == null ? "**NO EXISTE " + pstr_Nombregrupollave + " **" : "PROVEEDOR :" + (B.v_CodCliente + " " + B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " + B.v_RazonSocial).Trim() : "" + pstr_grupollave == "" ? G == null ? "** NO EXISTE PRODUCTO" + pstr_Nombregrupollave + " **" : "PRODUCTO :" + G.v_CodInterno + " " + G.v_Descripcion : "",
                         //DetalleCodigoLlaveAux = pstr_grupollave == "NROCUENTA" ? H == null ? "** NO EXISTE " + pstr_Nombregrupollave + " **" : "CUENTA :" + H.v_NroCuenta + " " + H.v_NombreCuenta : "" + pstr_grupollave == "NOMBREPROVEEDOR" ? B == null ? "** NO EXISTE " + pstr_Nombregrupollave + " **" : "PROVEEDOR :" + (B.v_CodCliente + " " + B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " + B.v_RazonSocial).Trim() : "" + pstr_grupollave == "" ? B == null ? "** NO EXISTE " + pstr_Nombregrupollave + " **" : pstr_Nombregrupollave + ":" + G.v_CodInterno + " " + G.v_Descripcion : "",
                         NombreGrupo = pstr_Nombregrupollave == "" ? "PRODUCTO" : pstr_Nombregrupollave
                     }).ToList();

                if (ConsideraDocInternos == 1)
                {
                    var query1 = from A in query.ToList()
                                 //let ValorCompra = CalcularCompraDetalle(A.IdTipoDocumento, 1, A.ValorVentaDetalle, 1, A.PrecioVentaDetalle, A.TipoCambio, A.IdMoneda)
                                 let CalcularSumaTotal =
                                     CalcularSumaDetalleResumen(query, A.DetalleCodigoLlave, A.IdTipoDocumento).ToList()
                                 select new ReporteRegistroCompraProductoResumen
                                 {
                                     IdTipoDocumento = A.IdTipoDocumento,
                                     IdProducto = A.IdProducto,
                                     NombreProducto = A.NombreProducto,
                                     CantidadDetalle = CalcularSumaTotal[0],
                                     ValorVentaDetalle = CalcularSumaTotal[1],
                                     ValorVentaDetalleD = CalcularSumaTotal[2],
                                     PrecioVentaDetalle = CalcularSumaTotal[3],
                                     PrecioVentaDetalleD = CalcularSumaTotal[4],
                                     TipoCambio = A.TipoCambio,
                                     IdMoneda = A.IdMoneda,
                                     DetalleCodigoLlave = A.DetalleCodigoLlave,
                                     DetalleNombreLlave = A.DetalleNombreLlave,
                                     DetalleCodigoLlaveAux = A.DetalleCodigoLlaveAux,
                                     NombreGrupo = A.NombreGrupo
                                 };

                #endregion

                    // 3.- Query para agrupary ordenar 

                    var objData = new List<ReporteRegistroCompraProductoResumen>();
                    //objData = query1.GroupBy(x => new { x.DetalleCodigoLlave })
                    //                        .Select(group => group.First())

                    //                        .OrderByDescending(o => o.NombreProducto).ToList();
                    objData = query1.GroupBy(x => new { x.NombreProducto })
                        .Select(group => group.First())
                        .OrderByDescending(o => o.NombreProducto).ToList();

                    return objData.ToList();
                }
                else
                {
                    query = query.ToList().Where(x => _objDocumentoBL.DocumentoEsContable(x.IdTipoDocumento)).ToList();

                    var query1 = from A in query.ToList()
                                 //let ValorCompra = CalcularCompraDetalle(A.IdTipoDocumento, 1, A.ValorVentaDetalle, 1, A.PrecioVentaDetalle, A.TipoCambio, A.IdMoneda)
                                 let CalcularSumaTotal =
                                     CalcularSumaDetalleResumen(query, A.DetalleCodigoLlave, A.IdTipoDocumento).ToList()
                                 select new ReporteRegistroCompraProductoResumen
                                 {
                                     IdTipoDocumento = A.IdTipoDocumento,
                                     IdProducto = A.IdProducto,
                                     NombreProducto = A.NombreProducto,
                                     CantidadDetalle = CalcularSumaTotal[0],
                                     ValorVentaDetalle = CalcularSumaTotal[1],
                                     ValorVentaDetalleD = CalcularSumaTotal[2],
                                     PrecioVentaDetalle = CalcularSumaTotal[3],
                                     PrecioVentaDetalleD = CalcularSumaTotal[4],
                                     TipoCambio = A.TipoCambio,
                                     IdMoneda = A.IdMoneda,
                                     DetalleCodigoLlave = A.DetalleCodigoLlave,
                                     DetalleNombreLlave = A.DetalleNombreLlave,
                                     DetalleCodigoLlaveAux = A.DetalleCodigoLlaveAux,
                                     NombreGrupo = A.NombreGrupo
                                 };


                    // 3.- Query para agrupary ordenar 

                    var objData = new List<ReporteRegistroCompraProductoResumen>();
                    objData = query1.GroupBy(x => new { x.NombreProducto })
                        .Select(group => group.First())
                        .OrderByDescending(o => o.NombreProducto).ToList();

                    return objData.ToList();
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;
            }
        }

        private List<decimal> CalcularSumaDetalle(List<ReporteRegistroCompraProductoResumen> Lista,
            string pstrDetalleCodigoLlave)
        {
            var objData = new valoresRegistroCompra();

            //List<ReporteRegistroVentaProductoResumen> RetonaRLISTA = new List<ReporteRegistroVentaProductoResumen>();
            //RetonaRLISTA=Lista;
            var Retonar = new List<decimal>();
            var xx = Lista.Where(p => p.DetalleCodigoLlave == pstrDetalleCodigoLlave);

            decimal CantidadDetalle, ValorVentaDetalle, PrecioVentaDetalle;
            CantidadDetalle = xx.Sum(x => x.CantidadDetalle);

            ValorVentaDetalle = xx.Sum(x => x.ValorVentaDetalle);
            PrecioVentaDetalle = xx.Sum(x => x.PrecioVentaDetalle);
            objData = CalcularCompraDetalle(Lista.FirstOrDefault().IdTipoDocumento, 0, ValorVentaDetalle, 0,
                PrecioVentaDetalle, Lista.FirstOrDefault().TipoCambio, Lista.FirstOrDefault().IdMoneda);

            Retonar.Add(xx.Sum(x => x.CantidadDetalle));
            Retonar.Add(objData.ValorCompraDetalleSoles);
            Retonar.Add(objData.ValorCompraDetalleDolares);
            Retonar.Add(objData.PrecioCompraDetalleSoles);
            Retonar.Add(objData.PrecioCompraDetalleDolares);
            return Retonar;
        }

        private List<decimal> CalcularSumaDetalleResumen(List<ReporteRegistroCompraProductoResumen> Lista,
            string pstrDetalleCodigoLlave, int? TipoDocumento)
        {
            var objData = new valoresRegistroCompra();
            var Retonar = new List<decimal>();
            var xx = Lista.Where(p => p.DetalleCodigoLlave == pstrDetalleCodigoLlave);

            decimal CantidadDetalle, ValorVentaDetalle, PrecioVentaDetalle;
            CantidadDetalle = xx.Sum(x => x.CantidadDetalle);
            ValorVentaDetalle =
                xx.ToList()
                    .Where(x => !_objDocumentoBL.DocumentoEsInverso(x.IdTipoDocumento))
                    .Sum(x => x.ValorVentaDetalle) -
                xx.ToList()
                    .Where(x => _objDocumentoBL.DocumentoEsInverso(x.IdTipoDocumento))
                    .Sum(x => x.ValorVentaDetalle);
            PrecioVentaDetalle = xx.Sum(x => x.PrecioVentaDetalle);
            objData = CalcularCompraDetalle(TipoDocumento.Value, 0, ValorVentaDetalle, 0, PrecioVentaDetalle,
                Lista.FirstOrDefault().TipoCambio, Lista.FirstOrDefault().IdMoneda);

            Retonar.Add(xx.Sum(x => x.CantidadDetalle));
            Retonar.Add(objData.ValorCompraDetalleSoles);
            Retonar.Add(objData.ValorCompraDetalleDolares);
            Retonar.Add(objData.PrecioCompraDetalleSoles);
            Retonar.Add(objData.PrecioCompraDetalleDolares);
            return Retonar;
        }

        public List<ReportePdtLiquidacionCompras> ReportepdtLiquidacionCompras(ref OperationResult objOperationResult,
            DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var liquidacioncompra = (from a in dbContext.liquidacioncompra
                                             join b in dbContext.cliente on new { proveedor = a.v_IdProveedor, eliminado = 0 } equals
                                                 new { proveedor = b.v_IdCliente, eliminado = b.i_Eliminado.Value } into b_join
                                             from b in b_join.DefaultIfEmpty()
                                             where
                                                 a.t_FechaRegistro >= fechaInicio
                                                 && a.t_FechaRegistro <= fechaFin &&
                                                 a.i_IdEstablecimiento == Globals.ClientSession.i_IdEstablecimiento.Value &&
                                                 a.i_Eliminado == 0
                                                 && a.i_IdEstado == 1
                                             select new ReportePdtLiquidacionCompras
                                             {
                                                 TipoDocumentoProv = b == null ? -1 : b.i_IdTipoIdentificacion.Value,
                                                 NumeroDocumentoProv = b == null ? "" : b.v_NroDocIdentificacion.Trim(),
                                                 ApePaternoProv = b == null ? "" : b.v_ApePaterno.Trim(),
                                                 ApeMaternoProv = b == null ? "" : b.v_ApeMaterno.Trim(),
                                                 Nombre = b == null ? "" : b.v_PrimerNombre.Trim(),
                                                 Serie = a.v_SerieDocumento.Trim(),
                                                 Numero = a.v_CorrelativoDocumento.Trim(),
                                                 FechaEmision = a.t_FechaEmision.Value,
                                                 FechaRetencion = a.t_FechaEmision.Value,
                                                 TotalOperacion = a.d_ValorVenta.Value,
                                                 TipoOperacion = "06",
                                                 NumeroRegistro = a.v_Periodo + a.v_Mes + a.v_Correlativo,
                                                 CodigoProv = b.v_CodCliente.Trim()
                                             }).ToList().OrderBy(x => x.NumeroRegistro).ToList();


                    objOperationResult.Success = 1;
                    return liquidacioncompra;
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;
            }
        }

        public List<ReportePdtRetencionIgv> ReportepdtRetencionIgv(ref OperationResult objOperationResult,
            DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var liquidacioncompra = (from a in dbContext.liquidacioncompra
                                             join b in dbContext.cliente on new { proveedor = a.v_IdProveedor, eliminado = 0 } equals
                                                 new { proveedor = b.v_IdCliente, eliminado = b.i_Eliminado.Value } into b_join
                                             from b in b_join.DefaultIfEmpty()
                                             where
                                                 a.t_FechaRegistro >= fechaInicio
                                                 && a.t_FechaRegistro <= fechaFin &&
                                                 a.i_IdEstablecimiento == Globals.ClientSession.i_IdEstablecimiento.Value &&
                                                 a.i_Eliminado == 0
                                                 && a.i_IdEstado == 1
                                             select new ReportePdtRetencionIgv
                                             {
                                                 TipoDocumentoProv = b == null ? -1 : b.i_IdTipoIdentificacion.Value,
                                                 NumeroDocumentoProv = b == null ? "" : b.v_NroDocIdentificacion.Trim(),
                                                 TipoPersona = b.i_IdTipoPersona.Value,
                                                 ApePaternoProv = b == null ? "" : b.v_ApePaterno.Trim(),
                                                 ApeMaternoProv = b == null ? "" : b.v_ApeMaterno.Trim(),
                                                 Nombre = b == null ? "" : b.v_PrimerNombre.Trim(),
                                                 RazonSocial = b == null ? "" : b.i_IdTipoPersona == 2 ? b.v_RazonSocial.Trim() : "",
                                                 TipoDocumento = a == null ? -1 : a.i_IdTipoDocumento.Value,
                                                 Serie = a.v_SerieDocumento.Trim(),
                                                 Numero = a.v_CorrelativoDocumento.Trim(),
                                                 FechaEmision = a.t_FechaEmision.Value,
                                                 RentaNeta = a.d_ValorVenta.Value,
                                                 NumeroRegistro = a.v_Periodo + a.v_Mes + a.v_Correlativo,
                                                 CodigoProv = b.v_CodCliente.Trim()
                                             }).ToList().OrderBy(x => x.NumeroRegistro).ToList();


                    objOperationResult.Success = 1;
                    return liquidacioncompra;
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;
            }
        }

        public List<ReporteRegistroCompraSunat> ReporteRegistroCompraSunat(ref OperationResult objOperationResult,
            int pstri_IdEstablecimiento, DateTime? pstrt_FechaRegistroIni, DateTime? pstrt_FechaRegistroFin,
            int pintIdMoneda, string pstrt_Orden, int pintIdTipoCompra, string pstrt_NroCuenta, int pintIdTipoDocumento,
            string pstr_grupollave = null, string pstr_Nombregrupollave = null, int? ConsideraDocInternos = null,
            int? LibroElectronico = null, bool UsadoPDB = false, int Establecimiento = -1)
        {
            //mon.IsActive = true;
            try
            {
                var _clienteDto = new clienteDto();
                var _objClienteBL = new ClienteBL();
                var dbContext = new SAMBHSEntitiesModelWin();
                var query4 = new List<ReporteRegistroCompraSunat>();
                var ListaFinal = new List<ReporteRegistroCompraSunat>();
                var objReporte = new ReporteRegistroCompraSunat();
                var ListaReporteFinalizado = new List<ReporteRegistroCompraSunat>();
                var ListaFinal1 = new List<ReporteRegistroCompraSunat>();

                #region Query

                //string MesReporte = pstrt_FechaRegistroIni.Value.Date.Month.ToString();
                //1. query para obtener toda la data filtrada por los parametros




                var queryComprasRectificaciones =
                   (from A in dbContext.compra
                    join B in dbContext.cliente on new { IdProveedor = A.v_IdProveedor, Flag = "V", eliminado = 0 }
                        equals
                        new { IdProveedor = B.v_IdCliente, Flag = B.v_FlagPantalla, eliminado = B.i_Eliminado.Value }
                        into B_join
                    from B in B_join.DefaultIfEmpty()
                    join F in dbContext.datahierarchy on
                        new { IdMoneda = A.i_IdMoneda.Value, Grupo = 18, eliminado = 0 } equals
                        new { IdMoneda = F.i_ItemId, Grupo = F.i_GroupId, eliminado = F.i_IsDeleted.Value } into
                        F_join
                    from F in F_join.DefaultIfEmpty()
                    join I in dbContext.compradetalle on new { idCompra = A.v_IdCompra, eliminado = 0 } equals
                        new { idCompra = I.v_IdCompra, eliminado = I.i_Eliminado.Value } into I_join
                    from I in I_join.DefaultIfEmpty()
                    join J in dbContext.productodetalle on
                        new { IdProductoDetalle = (I == null ? "" : I.v_IdProductoDetalle), eliminado = 0 } equals
                        new { IdProductoDetalle = J.v_IdProductoDetalle, eliminado = J.i_Eliminado.Value } into J_join
                    from J in J_join.DefaultIfEmpty()
                    join K in dbContext.producto on new { idProducto = (J == null ? "" : J.v_IdProducto), eliminado = 0 } equals
                        new { idProducto = K.v_IdProducto, eliminado = K.i_Eliminado.Value } into K_join
                    from K in K_join.DefaultIfEmpty()
                    join G in dbContext.datahierarchy on new { Igv = (A == null || A.i_IdIgv == null ? 0 : A.i_IdIgv.Value), eliminado = 0, Grupo = 27 }
                        equals new { Igv = G.i_ItemId, eliminado = G.i_IsDeleted.Value, Grupo = G.i_GroupId } into
                        G_join
                    from G in G_join.DefaultIfEmpty()
                    join H in dbContext.documento on new { TipoDoc = A.i_IdTipoDocumento.Value, eliminado = 0 } equals
                        new { TipoDoc = H.i_CodigoDocumento, eliminado = H.i_Eliminado.Value } into H_join
                    from H in H_join.DefaultIfEmpty()
                    join L in dbContext.asientocontable on new { eliminado = 0, c = (I == null ? "" : I.v_NroCuenta), p = periodo } equals
                        new { eliminado = L.i_Eliminado.Value, c = L.v_NroCuenta, p = L.v_Periodo } into L_join
                    from L in L_join.DefaultIfEmpty()
                    join M in dbContext.datahierarchy on
                        new { eliminado = 0, Grupo = 146, Tribut = (B == null || B.i_IdConvenioDobleTributacion == null ? 0 : B.i_IdConvenioDobleTributacion.Value) } equals
                        new { eliminado = M.i_IsDeleted.Value, Grupo = M.i_GroupId, Tribut = M.i_ItemId } into M_join
                    from M in M_join.DefaultIfEmpty()
                    join N in dbContext.datahierarchy on
                        new { Grupo = 29, eliminado = 0, codigo = A.i_CodigoDetraccion.Value } equals
                        new { Grupo = N.i_GroupId, eliminado = N.i_IsDeleted.Value, codigo = N.i_ItemId } into N_join
                    from N in N_join.DefaultIfEmpty()

                    where
                        (A.i_Eliminado == 0) && A.t_FechaCorreccionPle >= pstrt_FechaRegistroIni &&
                        A.t_FechaCorreccionPle <= pstrt_FechaRegistroFin &&
                        (I.v_NroCuenta.Trim() == pstrt_NroCuenta || pstrt_NroCuenta == "") &&
                        (A.i_IdTipoDocumento == pintIdTipoDocumento || pintIdTipoDocumento == -1) &&
                        (A.i_IdTipoCompra == pintIdTipoCompra || pintIdTipoCompra == -1) && A.i_AplicaRectificacion == 1

                        && (A.i_IdEstablecimiento == Establecimiento || Establecimiento == -1)


                    select new ReporteRegistroCompraSunat
                    {
                        Correlativo = A.v_Mes.Trim() + A.v_Correlativo.Trim(),
                        FechaVencimiento = A.i_IdTipoDocumento == 14 ? A.t_FechaVencimiento : null,
                        IdTipoDocumento = A.i_IdTipoDocumento ?? 0,
                        TipoDocumento = "",
                        SerieDocumento = A.v_SerieDocumento,
                        aniovencimiento = "",
                        CorrelativoDocumento = A.v_CorrelativoDocumento.Trim(),
                        NombreProveedor =
                            A.i_IdEstado == 1
                                ? B == null ? "** CLIENTE NO EXISTE **" : (B.v_PrimerNombre + " " + B.v_ApePaterno + " " + B.v_ApeMaterno + " " +
                                   B.v_RazonSocial).Trim()
                                : "**** ANULADO ****",
                        DocIdentidad = A.i_IdEstado == 1 ? B == null ? 0 : B.i_IdTipoIdentificacion ?? 0 : 0,
                        NroDocProveedor = A.i_IdEstado == 1 ? B == null ? "" : B.v_NroDocIdentificacion : "0",
                        TipoCambio = A.i_IdEstado == 1 ? A.d_TipoCambio ?? 0 : 0,
                        LlaveOrdenar = "",
                        FechaRegistroRef =
                            A.i_IdTipoDocumento == 6 || A.i_IdTipoDocumento == 7 || A.i_IdTipoDocumento == 8 ||
                            A.i_IdTipoDocumento == 87 || A.i_IdTipoDocumento == 88 || A.i_IdTipoDocumento == 97 ||
                            A.i_IdTipoDocumento == 98
                                ? A.t_FechaRef
                                : null,
                        IdTipoDocumentoRef = A.i_IdTipoDocumentoRef ?? 0,
                        TipoDocumentoRef = "",
                        SerieDocumentoRef = A.v_SerieDocumentoRef,
                        CorrelativoDocumentoRef = A.v_CorrelativoDocumentoRef,
                        NroCPagoNoDomiciliario =
                            A.i_IdTipoDocumento == 91 ? A.v_SerieDocumento + "-" + A.v_CorrelativoDocumento : "",

                        FechaEmision = A.t_FechaEmision.Value,
                        IgvNombre = G == null ? "" : G.v_Value1,
                        TipoCompra = "COMPRAS NACIONALES",
                        CompraGeneradaImportacion =
                            A.v_IdDocumentoReferencia == null ? "" : A.v_IdDocumentoReferencia,
                        NumeroRegistro = "C " + A.v_Mes + A.v_Correlativo + A.v_Periodo,
                        AnioAduanero = A.t_FechaEmision.Value,
                        Estado = A.i_IdEstado.Value,
                        DestinoCabecera = A.i_IdDestino == null || A.i_IdDestino == null ? 0 : A.i_IdDestino.Value,
                        DestinoGrilla = I == null || I.i_IdDestino == null ? 0 : I.i_IdDestino.Value,
                        idMoneda = A.i_IdMoneda ?? 0,
                        d_ValorVenta = A.d_ValorVenta == null ? 0 : A.d_ValorVenta.Value,
                        idCabecera = A.v_IdCompra == null ? "" : A.v_IdCompra,

                        anticipoDetalle = I == null ? 0 : I.i_Anticipio == null ? 0 : I.i_Anticipio.Value,
                        d_Igv = A.d_IGV == null ? 0 : A.d_IGV.Value,
                        TipoDocGenerado = A.i_IdTipoDocumento ?? 0,
                        serieDocumentoGenerado = A.v_SerieDocumento,
                        correlativoDocumentoGenerado = A.v_CorrelativoDocumento,
                        porcentajeIgv = G == null ? "0" : G.v_Value1,
                        d_ValorVentaDetalle = I == null ? 0 : I.d_ValorVenta == null ? 0 : I.d_ValorVenta.Value,
                        d_IgvDetalle = I == null ? 0 : I.d_Igv ?? 0,
                        Isc = A == null ? 0 : A.d_total_isc == null ? 0 : A.d_total_isc.Value,
                        OtroTributos = A == null ? 0 : A.d_total_otrostributos == null ? 0 : A.d_total_otrostributos.Value,
                        TipoPersona = B == null ? 0 : B.i_IdTipoPersona ?? 0,
                        FechaDetraccion = A.i_EsDetraccion == 1 ? A.t_FechaDetraccion : null,
                        CorrelativoPle =
                            A.v_Mes.Trim() + A.v_Correlativo.Trim().Substring(0, 2) +
                            A.v_Correlativo.Trim().Substring(4, 8),
                        GrupoLlave =
                            pstr_grupollave == "SERIE DOCUMENTO" ? "SERIE DOCUMENTO : " + A.v_SerieDocumento : "",
                        GrupoLlave2 = pstr_grupollave == "SERIE DOCUMENTO" ? "TIPO DOCUMENTO : " + H.v_Nombre : "",
                        TotalDocumentos = H.v_Nombre == null ? "" : "TOTAL " + H.v_Nombre + " : ",
                        SiglasMoneda = A.i_IdMoneda != -1 ? F.v_Field.Trim() : "",
                        TipoBien = L == null ? "" : L.v_TipoExistencia == null ? "" : L.v_TipoExistencia,
                        IdPais = B == null ? 0 : B.i_IdPais ?? 0,
                        DobleTributacion = M == null ? "" : M.v_Value2 == null ? "" : M.v_Value2,
                        ApePaterno = B == null ? "" : B.v_ApePaterno ?? "",
                        ApeMaterno = B == null ? "" : B.v_ApeMaterno ?? "",
                        PrimerNombre = B == null ? "" : B.v_PrimerNombre ?? "",
                        SegundoNombre = B == null ? "" : B.v_SegundoNombre ?? "",
                        EsDetraccion = A.i_EsDetraccion ?? 0,
                        NumeroDetraccion = A.v_NroDetraccion ?? "",
                        Glosa = A.v_Glosa,
                        CentroCosto = I.i_IdCentroCostos,
                        Total = A.i_IdEstado == 1 ? A.d_Total ?? 0 : 0,
                        d_PrecioVentaDetalle = I == null ? 0 : I.d_PrecioVenta,

                        NroCuenta = I == null ? "" : I.v_NroCuenta,
                        AfectoIgv = A.i_EsAfectoIgv == 1 ? true : false,
                        PrecioIncluyeIgv = A.i_PreciosIncluyenIgv == 1 ? true : false,

                        CodigoDetraccion = N == null ? "" : N.v_Field,
                        i_AplicarRectificacion = A.i_AplicaRectificacion ?? 0,
                        t_FechaRegistro = A.t_FechaRegistro.Value,




                    }).ToList();


                var queryCompras =
                    (from A in dbContext.compra
                     join B in dbContext.cliente on new { IdProveedor = A.v_IdProveedor, Flag = "V", eliminado = 0 }
                         equals
                         new { IdProveedor = B.v_IdCliente, Flag = B.v_FlagPantalla, eliminado = B.i_Eliminado.Value }
                         into B_join
                     from B in B_join.DefaultIfEmpty()
                     join F in dbContext.datahierarchy on
                         new { IdMoneda = A.i_IdMoneda.Value, Grupo = 18, eliminado = 0 } equals
                         new { IdMoneda = F.i_ItemId, Grupo = F.i_GroupId, eliminado = F.i_IsDeleted.Value } into
                         F_join
                     from F in F_join.DefaultIfEmpty()
                     join I in dbContext.compradetalle on new { idCompra = A.v_IdCompra, eliminado = 0 } equals
                         new { idCompra = I.v_IdCompra, eliminado = I.i_Eliminado.Value } into I_join
                     from I in I_join.DefaultIfEmpty()
                     join J in dbContext.productodetalle on
                         new { IdProductoDetalle = (I == null ? "" : I.v_IdProductoDetalle), eliminado = 0 } equals
                         new { IdProductoDetalle = J.v_IdProductoDetalle, eliminado = J.i_Eliminado.Value } into J_join
                     from J in J_join.DefaultIfEmpty()
                     join K in dbContext.producto on new { idProducto = (J == null ? "" : J.v_IdProducto), eliminado = 0 } equals
                         new { idProducto = K.v_IdProducto, eliminado = K.i_Eliminado.Value } into K_join
                     from K in K_join.DefaultIfEmpty()
                     join G in dbContext.datahierarchy on new { Igv = (A == null || A.i_IdIgv == null ? 0 : A.i_IdIgv.Value), eliminado = 0, Grupo = 27 }
                         equals new { Igv = G.i_ItemId, eliminado = G.i_IsDeleted.Value, Grupo = G.i_GroupId } into
                         G_join
                     from G in G_join.DefaultIfEmpty()
                     join H in dbContext.documento on new { TipoDoc = A.i_IdTipoDocumento.Value, eliminado = 0 } equals
                         new { TipoDoc = H.i_CodigoDocumento, eliminado = H.i_Eliminado.Value } into H_join
                     from H in H_join.DefaultIfEmpty()
                     join L in dbContext.asientocontable on new { eliminado = 0, c = (I == null ? "" : I.v_NroCuenta), p = periodo } equals
                         new { eliminado = L.i_Eliminado.Value, c = L.v_NroCuenta, p = L.v_Periodo } into L_join
                     from L in L_join.DefaultIfEmpty()
                     join M in dbContext.datahierarchy on
                         new { eliminado = 0, Grupo = 146, Tribut = (B == null || B.i_IdConvenioDobleTributacion == null ? 0 : B.i_IdConvenioDobleTributacion.Value) } equals
                         new { eliminado = M.i_IsDeleted.Value, Grupo = M.i_GroupId, Tribut = M.i_ItemId } into M_join
                     from M in M_join.DefaultIfEmpty()
                     join N in dbContext.datahierarchy on
                         new { Grupo = 29, eliminado = 0, codigo = A.i_CodigoDetraccion.Value } equals
                         new { Grupo = N.i_GroupId, eliminado = N.i_IsDeleted.Value, codigo = N.i_ItemId } into N_join
                     from N in N_join.DefaultIfEmpty()

                     where
                         (A.i_Eliminado == 0) && A.t_FechaRegistro >= pstrt_FechaRegistroIni &&
                         A.t_FechaRegistro <= pstrt_FechaRegistroFin &&
                         (I.v_NroCuenta.Trim() == pstrt_NroCuenta || pstrt_NroCuenta == "") &&
                         (A.i_IdTipoDocumento == pintIdTipoDocumento || pintIdTipoDocumento == -1) &&
                         (A.i_IdTipoCompra == pintIdTipoCompra || pintIdTipoCompra == -1)
                          && (A.i_IdEstablecimiento == Establecimiento || Establecimiento == -1)


                     select new ReporteRegistroCompraSunat
                     {
                         Correlativo = A.v_Mes.Trim() + A.v_Correlativo.Trim(),
                         FechaVencimiento = A.i_IdTipoDocumento == 14 ? A.t_FechaVencimiento : null,
                         IdTipoDocumento = A.i_IdTipoDocumento ?? 0,
                         TipoDocumento = "",
                         SerieDocumento = A.v_SerieDocumento,
                         aniovencimiento = "",
                         CorrelativoDocumento = A.v_CorrelativoDocumento.Trim(),
                         NombreProveedor =
                             A.i_IdEstado == 1
                                 ? B == null ? "** CLIENTE NO EXISTE **" : (B.v_PrimerNombre + " " + B.v_ApePaterno + " " + B.v_ApeMaterno + " " +
                                    B.v_RazonSocial).Trim()
                                 : "**** ANULADO ****",
                         DocIdentidad = A.i_IdEstado == 1 ? B == null ? 0 : B.i_IdTipoIdentificacion ?? 0 : 0,
                         NroDocProveedor = A.i_IdEstado == 1 ? B == null ? "" : B.v_NroDocIdentificacion : "0",
                         TipoCambio = A.i_IdEstado == 1 ? A.d_TipoCambio ?? 0 : 0,
                         LlaveOrdenar = "",
                         FechaRegistroRef =
                             A.i_IdTipoDocumento == 6 || A.i_IdTipoDocumento == 7 || A.i_IdTipoDocumento == 8 ||
                             A.i_IdTipoDocumento == 87 || A.i_IdTipoDocumento == 88 || A.i_IdTipoDocumento == 97 ||
                             A.i_IdTipoDocumento == 98
                                 ? A.t_FechaRef
                                 : null,
                         IdTipoDocumentoRef = A.i_IdTipoDocumentoRef ?? 0,
                         TipoDocumentoRef = "",
                         SerieDocumentoRef = A.v_SerieDocumentoRef,
                         CorrelativoDocumentoRef = A.v_CorrelativoDocumentoRef,
                         NroCPagoNoDomiciliario =
                             A.i_IdTipoDocumento == 91 ? A.v_SerieDocumento + "-" + A.v_CorrelativoDocumento : "",

                         FechaEmision = A.t_FechaEmision.Value,
                         IgvNombre = G == null ? "" : G.v_Value1,
                         TipoCompra = "COMPRAS NACIONALES",
                         CompraGeneradaImportacion =
                             A.v_IdDocumentoReferencia == null ? "" : A.v_IdDocumentoReferencia,
                         NumeroRegistro = "C " + A.v_Mes + A.v_Correlativo + A.v_Periodo,
                         AnioAduanero = A.t_FechaEmision.Value,
                         Estado = A.i_IdEstado.Value,
                         DestinoCabecera = A.i_IdDestino == null || A.i_IdDestino == null ? 0 : A.i_IdDestino.Value,
                         DestinoGrilla = I == null || I.i_IdDestino == null ? 0 : I.i_IdDestino.Value,
                         idMoneda = A.i_IdMoneda ?? 0,
                         d_ValorVenta = A.d_ValorVenta == null ? 0 : A.d_ValorVenta.Value,
                         idCabecera = A.v_IdCompra == null ? "" : A.v_IdCompra,

                         anticipoDetalle = I == null ? 0 : I.i_Anticipio == null ? 0 : I.i_Anticipio.Value,
                         d_Igv = A.d_IGV == null ? 0 : A.d_IGV.Value,
                         TipoDocGenerado = A.i_IdTipoDocumento ?? 0,
                         serieDocumentoGenerado = A.v_SerieDocumento,
                         correlativoDocumentoGenerado = A.v_CorrelativoDocumento,
                         porcentajeIgv = G == null ? "0" : G.v_Value1,
                         d_ValorVentaDetalle = I == null ? 0 : I.d_ValorVenta == null ? 0 : I.d_ValorVenta.Value,
                         d_IgvDetalle = I == null ? 0 : I.d_Igv ?? 0,
                         Isc = A == null ? 0 : A.d_total_isc == null ? 0 : A.d_total_isc.Value,
                         OtroTributos = A == null ? 0 : A.d_total_otrostributos == null ? 0 : A.d_total_otrostributos.Value,
                         TipoPersona = B == null ? 0 : B.i_IdTipoPersona ?? 0,
                         FechaDetraccion = A.i_EsDetraccion == 1 ? A.t_FechaDetraccion : null,
                         CorrelativoPle =
                             A.v_Mes.Trim() + A.v_Correlativo.Trim().Substring(0, 2) +
                             A.v_Correlativo.Trim().Substring(4, 8),
                         GrupoLlave =
                             pstr_grupollave == "SERIE DOCUMENTO" ? "SERIE DOCUMENTO : " + A.v_SerieDocumento : "",
                         GrupoLlave2 = pstr_grupollave == "SERIE DOCUMENTO" ? "TIPO DOCUMENTO : " + H.v_Nombre : "",
                         TotalDocumentos = H.v_Nombre == null ? "" : "TOTAL " + H.v_Nombre + " : ",
                         SiglasMoneda = A.i_IdMoneda != -1 ? F.v_Field.Trim() : "",
                         TipoBien = L == null ? "" : L.v_TipoExistencia == null ? "" : L.v_TipoExistencia,
                         IdPais = B == null ? 0 : B.i_IdPais ?? 0,
                         DobleTributacion = M == null ? "" : M.v_Value2 == null ? "" : M.v_Value2,
                         ApePaterno = B == null ? "" : B.v_ApePaterno ?? "",
                         ApeMaterno = B == null ? "" : B.v_ApeMaterno ?? "",
                         PrimerNombre = B == null ? "" : B.v_PrimerNombre ?? "",
                         SegundoNombre = B == null ? "" : B.v_SegundoNombre ?? "",
                         EsDetraccion = A.i_EsDetraccion ?? 0,
                         NumeroDetraccion = A.v_NroDetraccion ?? "",
                         Glosa = A.v_Glosa,
                         CentroCosto = I.i_IdCentroCostos,
                         Total = A.i_IdEstado == 1 ? A.d_Total ?? 0 : 0,
                         d_PrecioVentaDetalle = I == null ? 0 : I.d_PrecioVenta,

                         NroCuenta = I == null ? "" : I.v_NroCuenta,
                         AfectoIgv = A.i_EsAfectoIgv == 1 ? true : false,
                         PrecioIncluyeIgv = A.i_PreciosIncluyenIgv == 1 ? true : false,

                         CodigoDetraccion = N == null ? "" : N.v_Field,
                         i_AplicarRectificacion = A.i_AplicaRectificacion ?? 0,
                         t_FechaRegistro = A.t_FechaRegistro.Value,




                     }).ToList();

                queryCompras = queryCompras.Concat(queryComprasRectificaciones).ToList();


                var queryLiquidacionCompra = (from A in dbContext.liquidacioncompra
                                              join B in dbContext.cliente on new { IdProveedor = A.v_IdProveedor, Flag = "V", eliminado = 0 } equals
                                                  new { IdProveedor = B.v_IdCliente, Flag = B.v_FlagPantalla, eliminado = B.i_Eliminado.Value } into
                                                  B_join
                                              from B in B_join.DefaultIfEmpty()
                                              join F in dbContext.datahierarchy on new { IdMoneda = A.i_IdMoneda.Value, Grupo = 18, eliminado = 0 }
                                                  equals new { IdMoneda = F.i_ItemId, Grupo = F.i_GroupId, eliminado = F.i_IsDeleted.Value } into
                                                  F_join
                                              from F in F_join.DefaultIfEmpty()
                                              join I in dbContext.liquidacioncompradetalle on
                                                  new { IdLiquidacionCompra = A.v_IdLiquidacionCompra, eliminado = 0 } equals
                                                  new { IdLiquidacionCompra = I.v_IdLiquidacionCompra, eliminado = I.i_Eliminado.Value } into I_join
                                              from I in I_join.DefaultIfEmpty()
                                              join J in dbContext.productodetalle on
                                                  new { IdProductoDetalle = I.v_IdProductoDetalle, eliminado = 0 } equals
                                                  new { IdProductoDetalle = J.v_IdProductoDetalle, eliminado = J.i_Eliminado.Value } into J_join
                                              from J in J_join.DefaultIfEmpty()
                                              join K in dbContext.producto on new { idProducto = J.v_IdProducto, eliminado = 0 } equals
                                                  new { idProducto = K.v_IdProducto, eliminado = K.i_Eliminado.Value } into K_join
                                              from K in K_join.DefaultIfEmpty()
                                              join G in dbContext.datahierarchy on new { Igv = A.i_IdIgv.Value, eliminado = 0, Grupo = 27 } equals
                                                  new { Igv = G.i_ItemId, eliminado = G.i_IsDeleted.Value, Grupo = G.i_GroupId } into G_join
                                              from G in G_join.DefaultIfEmpty()
                                              join H in dbContext.documento on new { TipoDoc = A.i_IdTipoDocumento.Value, eliminado = 0 } equals
                                                  new { TipoDoc = H.i_CodigoDocumento, eliminado = H.i_Eliminado.Value } into H_join
                                              from H in H_join.DefaultIfEmpty()
                                              join L in dbContext.asientocontable on new { eliminado = 0, c = I.v_NroCuenta, p = periodo } equals
                                                  new { eliminado = L.i_Eliminado.Value, c = L.v_NroCuenta, p = L.v_Periodo } into L_join
                                              from L in L_join.DefaultIfEmpty()
                                              join M in dbContext.datahierarchy on
                                                  new { eliminado = 0, Grupo = 146, Tribut = B.i_IdConvenioDobleTributacion.Value } equals
                                                  new { eliminado = M.i_IsDeleted.Value, Grupo = M.i_GroupId, Tribut = M.i_ItemId } into M_join
                                              from M in M_join.DefaultIfEmpty()
                                              where
                                                  (A.i_Eliminado == 0) && A.t_FechaRegistro >= pstrt_FechaRegistroIni &&
                                                  A.t_FechaRegistro <= pstrt_FechaRegistroFin &&
                                                  (I.v_NroCuenta.Trim() == pstrt_NroCuenta || pstrt_NroCuenta == "") &&
                                                  (A.i_IdTipoDocumento == pintIdTipoDocumento || pintIdTipoDocumento == -1) &&
                                                  (pintIdTipoCompra == -1)
                                                   && (A.i_IdEstablecimiento == Establecimiento || Establecimiento == -1)

                                              select new ReporteRegistroCompraSunat
                                              {
                                                  Correlativo = A.v_Mes.Trim() + A.v_Correlativo.Trim(),
                                                  FechaVencimiento = null,
                                                  IdTipoDocumento = A.i_IdTipoDocumento.Value,
                                                  TipoDocumento = "",
                                                  SerieDocumento = A.v_SerieDocumento,
                                                  aniovencimiento = "",
                                                  CorrelativoDocumento = A.v_CorrelativoDocumento.Trim(),
                                                  NombreProveedor =
                                                      A.i_IdEstado == 1
                                                          ? (B.v_PrimerNombre + " " + B.v_ApePaterno + " " + B.v_ApeMaterno + " " +
                                                             B.v_RazonSocial).Trim()
                                                          : "**** ANULADO ****",
                                                  DocIdentidad = A.i_IdEstado == 1 ? B.i_IdTipoIdentificacion.Value : 0,
                                                  NroDocProveedor = A.i_IdEstado == 1 ? B.v_NroDocIdentificacion : "0",
                                                  TipoCambio = A.i_IdEstado == 1 ? A.d_TipoCambio.Value : 0,
                                                  LlaveOrdenar = "",
                                                  FechaRegistroRef = null,
                                                  IdTipoDocumentoRef = -1,
                                                  TipoDocumentoRef = "",
                                                  SerieDocumentoRef = "",
                                                  CorrelativoDocumentoRef = "",
                                                  NroCPagoNoDomiciliario =
                                                      A.i_IdTipoDocumento == 91 ? A.v_SerieDocumento + "-" + A.v_CorrelativoDocumento : "",

                                                  FechaEmision = A.t_FechaEmision.Value,
                                                  IgvNombre = G == null ? "" : G.v_Value1,
                                                  TipoCompra = "COMPRAS NACIONALES",
                                                  CompraGeneradaImportacion = "",
                                                  NumeroRegistro = "LC " + A.v_Mes + A.v_Correlativo + A.v_Periodo,
                                                  AnioAduanero = A.t_FechaEmision.Value,
                                                  Estado = A.i_IdEstado.Value,
                                                  DestinoCabecera = A.i_IdDestino == null ? 0 : A.i_IdDestino.Value,
                                                  DestinoGrilla = I == null ? 0 : I.i_IdDestino.Value,
                                                  idMoneda = A.i_IdMoneda.Value,
                                                  d_ValorVenta = A.d_ValorVenta == null ? 0 : A.d_ValorVenta.Value,
                                                  idCabecera = A.v_IdLiquidacionCompra == null ? "" : A.v_IdLiquidacionCompra,
                                                  anticipoDetalle = 0,
                                                  d_Igv = A.d_IGV == null ? 0 : A.d_IGV.Value,
                                                  TipoDocGenerado = A.i_IdTipoDocumento.Value,
                                                  serieDocumentoGenerado = A.v_SerieDocumento,
                                                  correlativoDocumentoGenerado = A.v_CorrelativoDocumento,
                                                  porcentajeIgv = G == null ? "0" : G.v_Value1,
                                                  d_ValorVentaDetalle = I == null ? 0 : I.d_ValorVenta == null ? 0 : I.d_ValorVenta.Value,
                                                  d_IgvDetalle = I == null ? 0 : I.d_Igv.Value,
                                                  Isc = 0,
                                                  OtroTributos = 0,
                                                  TipoPersona = B == null ? 0 : B.i_IdTipoPersona.Value,
                                                  FechaDetraccion = null,
                                                  CorrelativoPle =
                                                      A.v_Mes.Trim() + A.v_Correlativo.Trim().Substring(0, 2) +
                                                      A.v_Correlativo.Trim().Substring(4, 8),
                                                  GrupoLlave =
                                                      pstr_grupollave == "SERIE DOCUMENTO" ? "SERIE DOCUMENTO : " + A.v_SerieDocumento : "",
                                                  GrupoLlave2 = pstr_grupollave == "SERIE DOCUMENTO" ? "TIPO DOCUMENTO : " + H.v_Nombre : "",
                                                  TotalDocumentos = H.v_Nombre == null ? "" : "TOTAL " + H.v_Nombre + " : ",
                                                  SiglasMoneda = A.i_IdMoneda != -1 ? F.v_Field.Trim() : "",
                                                  TipoBien = L == null ? "" : L.v_TipoExistencia == null ? "" : L.v_TipoExistencia,

                                                  IdPais = B == null ? 0 : B.i_IdPais ?? 0,
                                                  DobleTributacion = M == null ? "" : M.v_Value2 == null ? "" : M.v_Value2,
                                                  ApePaterno = B == null ? "" : B.v_ApePaterno ?? "",
                                                  ApeMaterno = B == null ? "" : B.v_ApeMaterno ?? "",
                                                  PrimerNombre = B == null ? "" : B.v_PrimerNombre ?? "",
                                                  SegundoNombre = B == null ? "" : B.v_SegundoNombre ?? "",
                                                  EsDetraccion = 0,
                                                  NumeroDetraccion = "",
                                                  Glosa = A.v_Glosa,
                                                  CentroCosto = "",
                                                  Total = A.i_IdEstado == 1 ? A.d_Total ?? 0 : 0,
                                                  NroCuenta = I == null ? "" : I.v_NroCuenta,
                                                  AfectoIgv = A.i_EsAfectoIgv == 1 ? true : false,
                                                  PrecioIncluyeIgv = A.i_PreciosIncluyenIgv == 1 ? true : false,
                                                  d_PrecioVentaDetalle = I == null ? 0 : I.d_PrecioVenta,
                                                  CodigoDetraccion = "",
                                                  i_AplicarRectificacion = 0,
                                                  t_FechaRegistro = A.t_FechaRegistro.Value,


                                              }).ToList();


                if (LibroElectronico == 1 &&
                    dbContext.importacion.Where(
                        x => x.t_FechaRegistro >= pstrt_FechaRegistroIni && x.t_FechaRegistro <= pstrt_FechaRegistroFin)
                        .Any())
                {
                    if (
                        !dbContext.cliente.Where(
                            x => x.v_NroDocIdentificacion == Constants.RucSunat && x.i_Eliminado == 0 && x.i_Activo == 1 && x.v_FlagPantalla == "V")
                            .Any())
                    {
                        _clienteDto = new clienteDto();
                        _clienteDto.v_RazonSocial = "SUPERINTENDENCIA NACIONAL DE ADUANAS Y DE ADMINISTRACIÓN TRIBUTARIA";
                        _clienteDto.v_NroDocIdentificacion = Constants.RucSunat;
                        _clienteDto.v_CodCliente = Constants.RucSunat;
                        _clienteDto.i_IdTipoPersona = 2;
                        _clienteDto.i_IdTipoIdentificacion = 6;
                        _clienteDto.v_FlagPantalla = "V";
                        _clienteDto.i_Activo = 1;
                        _clienteDto.i_IdSexo = 2;
                        _clienteDto.v_DirecPrincipal = "LIMA";
                        _clienteDto.i_IdPais = 1;
                        _clienteDto.i_IdDepartamento = 1391;
                        _clienteDto.i_IdProvincia = 1392;
                        _clienteDto.i_IdDistrito = 1393;
                        _objClienteBL.InsertarCliente(ref objOperationResult, _clienteDto,
                            Globals.ClientSession.GetAsList(), null, null, null, null, null, null);
                    }
                }
                List<ReporteRegistroCompraSunat> queryImportaciones = new List<ReporteRegistroCompraSunat>();
                if (!UsadoPDB)
                {
                    queryImportaciones =
                         (from A in dbContext.importacion
                          join F in dbContext.datahierarchy on
                              new { idMoneda = A.i_IdMoneda.Value, eliminado = 0, Grupo = 18 } equals
                              new { idMoneda = F.i_ItemId, eliminado = F.i_IsDeleted.Value, Grupo = F.i_GroupId } into
                              F_join
                          from F in F_join.DefaultIfEmpty()
                          join I in dbContext.importaciondetalleproducto on
                              new { importacion = A.v_IdImportacion, eliminado = 0 } equals
                              new { importacion = I.v_IdImportacion, eliminado = I.i_Eliminado.Value } into I_join
                          from I in I_join.DefaultIfEmpty()
                          join J in dbContext.productodetalle on
                              new { idProductoDetalle = I.v_IdProductoDetalle, eliminado = 0 } equals
                              new { idProductoDetalle = J.v_IdProductoDetalle, eliminado = J.i_Eliminado.Value } into J_join
                          from J in J_join.DefaultIfEmpty()
                          join K in dbContext.producto on new { idProducto = J.v_IdProducto, eliminado = 0 } equals
                              new { idProducto = K.v_IdProducto, eliminado = K.i_Eliminado.Value } into K_join
                          from K in K_join.DefaultIfEmpty()
                          join G in dbContext.datahierarchy on new { igv = A.i_Igv.Value, eliminado = 0, Grupo = 27 } equals
                              new { igv = G.i_ItemId, eliminado = G.i_IsDeleted.Value, Grupo = G.i_GroupId } into G_join
                          from G in G_join.DefaultIfEmpty()
                          join H in dbContext.documento on new { TipoDoc = A.i_IdTipoDocumento.Value, eliminado = 0 } equals
                              new { TipoDoc = H.i_CodigoDocumento, eliminado = H.i_Eliminado.Value } into H_join
                          from H in H_join.DefaultIfEmpty()
                          join L in dbContext.importaciondetallegastos on new { A.v_IdImportacion, eliminado = 0 } equals
                              new { L.v_IdImportacion, eliminado = L.i_Eliminado.Value } into L_join
                          from L in L_join.DefaultIfEmpty()
                          join M in dbContext.datahierarchy on new { item = A.i_Igv.Value, eliminado = 0, Grupo = 27 }
                              equals new { item = M.i_ItemId, eliminado = M.i_IsDeleted.Value, Grupo = M.i_GroupId } into
                              M_join
                          from M in M_join.DefaultIfEmpty()
                          join N in dbContext.datahierarchy on
                              new { item = A.i_IdSerieDocumento.Value, eliminado = 0, Grupo = 53 } equals
                              new { item = N.i_ItemId, eliminado = N.i_IsDeleted.Value, Grupo = N.i_GroupId } into N_join
                          from N in N_join.DefaultIfEmpty()
                          join O in dbContext.cliente on new { DocCliente = H.v_provimp_3i, Flag = "V", eliminado = 0 }
                              equals
                              new { DocCliente = O.v_CodCliente, Flag = O.v_FlagPantalla, eliminado = O.i_Eliminado.Value }
                              into O_join
                          from O in O_join.DefaultIfEmpty()
                          join P in dbContext.datahierarchy on
                              new { eliminado = 0, Grupo = 146, Tribut = O.i_IdConvenioDobleTributacion.Value } equals
                              new { eliminado = P.i_IsDeleted.Value, Grupo = P.i_GroupId, Tribut = P.i_ItemId } into P_join
                          from P in P_join.DefaultIfEmpty()
                          where
                              (A.i_Eliminado == 0) && A.t_FechaRegistro >= pstrt_FechaRegistroIni &&
                              A.t_FechaRegistro <= pstrt_FechaRegistroFin &&
                              (A.i_IdTipoDocumento == pintIdTipoDocumento || pintIdTipoDocumento == -1)
                               && (A.i_IdEstablecimiento == Establecimiento || Establecimiento == -1)

                          select new ReporteRegistroCompraSunat
                          {
                              Correlativo = A.v_Mes.Trim() + A.v_Correlativo.Trim(),
                              FechaVencimiento = null,
                              IdTipoDocumento = A.i_IdTipoDocumento ?? -1,
                              TipoDocumento = "",
                              SerieDocumento = N == null ? "" : N.v_Value2,
                              aniovencimiento = "",
                              CorrelativoDocumento = A.v_CorrelativoDocumento.Trim(),
                              NombreProveedor =
                                  A.i_IdEstado == 0
                                      ? "****ANULADO***"
                                      : A.i_IdTipoDocumento == 50 || A.i_IdTipoDocumento == 52 ? O.v_RazonSocial : "",
                              DocIdentidad =
                                  A.i_IdEstado == 0
                                      ? 0
                                      : A.i_IdTipoDocumento == 50 || A.i_IdTipoDocumento == 52
                                          ? O.i_IdTipoIdentificacion
                                          : 0,
                              NroDocProveedor =
                                  A.i_IdEstado == 0
                                      ? "0"
                                      : A.i_IdTipoDocumento == 50 || A.i_IdTipoDocumento.Value == 52
                                          ? O.v_NroDocIdentificacion
                                          : "",
                              TipoCambio = A.i_IdEstado == 1 ? A.d_TipoCambio == null ? 0 : A.d_TipoCambio.Value : 0,
                              BaseImponible1 = 0,
                              Igv1 = 0,
                              BaseImponible2 = 0,
                              Igv2 = 0,
                              BaseImponible3 = 0,
                              Igv3 = 0,
                              ValorAdquisiciones = 0,
                              Total = 0,
                              LlaveOrdenar = "",
                              FechaRegistroRef = null,
                              IdTipoDocumentoRef = A.i_IdTipoDocumento ?? -1,
                              TipoDocumentoRef = "",
                              SerieDocumentoRef = "",
                              CorrelativoDocumentoRef = "",
                              NroCPagoNoDomiciliario =
                                  A.i_IdTipoDocumento == 91 ? N.v_Value2 + "-" + A.v_CorrelativoDocumento : "",

                              FechaEmision = A.t_FechaEmision.Value,
                              IgvNombre = G.v_Value1,
                              TipoCompra = "IMPORTACIONES",
                              CompraGeneradaImportacion = L == null ? "" : L.v_IdImportacionDetalleGastos,
                              NumeroRegistro = "I " + A.v_Mes + A.v_Correlativo + A.v_Periodo,
                              AnioAduanero = A.t_FechaEmision.Value,
                              Estado = A.i_IdEstado.Value,
                              DestinoCabecera = A.i_IdDestino ?? -1,
                              DestinoGrilla = -1,
                              idMoneda = A.i_IdMoneda ?? -1,
                              d_ValorVenta = 0,
                              idCabecera = A.v_IdImportacion,
                              anticipoDetalle = 0,
                              d_Igv = A == null ? 0 : A.d_Igv == null ? 0 : A.d_Igv.Value,
                              TipoDocGenerado = L == null ? -1 : L.i_IdTipoDocumento.Value,
                              serieDocumentoGenerado = L == null ? "" : L.v_SerieDocumento,
                              correlativoDocumentoGenerado = L == null ? "" : L.v_CorrelativoDocumento,
                              porcentajeIgv = M == null ? "" : M.v_Value1,
                              monedaGenerado = L == null ? -1 : L.i_IdMoneda == null ? 0 : L.i_IdMoneda.Value,
                              TotalSolesGenerado = L == null ? 0 : L.d_ImporteSoles == null ? 0 : L.d_ImporteSoles.Value,
                              TotalDolaresGenerado = L == null ? 0 : L.d_ImporteDolares == null ? 0 : L.d_ImporteDolares,
                              d_ValorVentaDetalle = 0,
                              d_IgvDetalle = 0,
                              Isc = 0,
                              OtroTributos = 0,
                              TipoPersona = A.i_IdEstado.Value == 1 ? -1 : -1,
                              FechaDetraccion = null,
                              CorrelativoPle =
                                A.v_Mes.Trim(),
                              GrupoLlave =
                                  pstr_grupollave == "SERIE DOCUMENTO"
                                      ? N.v_Value2 == null ? "SERIE DOCUMENTO : " + "" : "SERIE DOCUMENTO : " + N.v_Value2
                                      : "",
                              TotalDocumentos = H.v_Nombre == null ? "" : "SUB-TOTAL " + H.v_Nombre + " : ",
                              GrupoLlave2 = "",
                              SiglasMoneda = A.i_IdMoneda != -1 ? F.v_Field.Trim() : "",
                              TipoBien = "1",
                              IdPais = O.i_IdPais ?? -1,
                              DobleTributacion = P == null ? "" : P.v_Value2 == null ? "" : P.v_Value2,
                              ApePaterno = "",
                              ApeMaterno = "",
                              PrimerNombre = "",
                              SegundoNombre = "",
                              EsDetraccion = 0,
                              NumeroDetraccion = "",
                              Glosa = "",
                              CentroCosto = "",
                              CodigoDetraccion = "",
                              i_AplicarRectificacion = 0,
                              t_FechaRegistro = A.t_FechaRegistro.Value,


                          }).ToList();
                }

                #endregion
                if (Globals.ClientSession.i_Periodo != 2016)
                {
                    var CabecernasNulas = queryCompras.Concat(queryLiquidacionCompra).Where(l => (l.DestinoCabecera == null || l.DestinoCabecera.ToString() == "-1" || l.DestinoCabecera.ToString() == "0") && l.Estado == 1).ToList();
                    var DetallesNulas = queryCompras.Concat(queryLiquidacionCompra).Where(l => ((l.DestinoGrilla == null || l.DestinoGrilla.ToString() == "0" || l.DestinoGrilla.ToString() == "5") && (l.DestinoCabecera == 5 || l.DestinoCabecera == 999)) && l.Estado == 1).ToList();
                    if (CabecernasNulas.Any())
                    {

                        objOperationResult.AdditionalInformation = "El tipo de Operación no puede ser nulo : " + CabecernasNulas.FirstOrDefault().Correlativo;
                        objOperationResult.Success = 0;
                        return null;
                    }
                    if (DetallesNulas.Any())
                    {
                        objOperationResult.AdditionalInformation = "El tipo de Operación de los detalles no puede ser nulo ni mixto : " + DetallesNulas.FirstOrDefault().Correlativo;
                        objOperationResult.Success = 0;
                        return null;
                    }
                }

                var ComprasImportacionesConcatenadas =
                    queryCompras.Concat(queryImportaciones).ToList().Concat(queryLiquidacionCompra).ToList();

                #region Verficar que compras han sigo generadas por Importacion

                foreach (var item in ComprasImportacionesConcatenadas.ToList().AsParallel())
                {
                    if (item.TipoCompra == "COMPRAS NACIONALES")
                    {
                        var queryExistencia =
                            queryImportaciones.Where(
                                x =>
                                    x.CompraGeneradaImportacion != "" &&
                                    x.CompraGeneradaImportacion == item.CompraGeneradaImportacion).ToList();
                        if (queryExistencia.Count() > 0)
                        {
                            if (queryExistencia.FirstOrDefault().IdTipoDocumento == item.TipoDocGenerado &&
                                queryExistencia.FirstOrDefault().SerieDocumento == item.serieDocumentoGenerado &&
                                queryExistencia.FirstOrDefault().CorrelativoDocumento ==
                                item.correlativoDocumentoGenerado && item.DestinoCabecera != 4)
                            {
                                ListaFinal.Add(item);
                            }
                            else if (queryExistencia.FirstOrDefault().IdTipoDocumento != item.TipoDocGenerado &&
                                     queryExistencia.FirstOrDefault().SerieDocumento != item.serieDocumentoGenerado &&
                                     queryExistencia.FirstOrDefault().CorrelativoDocumento !=
                                     item.correlativoDocumentoGenerado)
                            {
                                ListaFinal.Add(item);
                            }


                        }
                        else
                        {
                            ListaFinal.Add(item);
                        }
                    }
                    else if (item.TipoCompra == "IMPORTACIONES")
                    {
                        ListaFinal.Add(item);
                    }
                }

                #endregion


                if (UsadoPDB)
                {
                    var Temporal1 =
                        ComprasImportacionesConcatenadas.ToList()
                            .GroupBy(x => x.idCabecera)
                            .Where(y => y.Any(x => x.DestinoCabecera != 5 && x.DestinoCabecera != 999))
                            .Select(x => x.First())
                            .OrderBy(o => o.TipoCompra)
                            .ThenBy(p => p.Correlativo)
                            .ToList();
                    //var Temporal2 =
                    //    ComprasImportacionesConcatenadas.ToList()
                    //        .Where(y => y.DestinoCabecera == 5 || y.DestinoCabecera == 999)
                    //        .OrderBy(o => o.TipoCompra)
                    //        .ThenBy(p => p.Correlativo)
                    //        .ToList();
                    var Temporal2 =
                      ComprasImportacionesConcatenadas.ToList()
                          .Where(y => y.DestinoCabecera == 5 || y.DestinoCabecera == 999)
                          .OrderBy(o => o.TipoCompra)
                          .ThenBy(p => p.Correlativo)
                          .ToList();
                    Temporal2 = Temporal2.GroupBy(o => new { o.idCabecera, o.DestinoGrilla }).ToList().Select(x =>
                        {
                            var k = x.FirstOrDefault();
                            k.d_ValorVenta = x.Sum(h => h.d_ValorVenta);
                            k.d_ValorVentaDetalle = x.Sum(h => h.d_ValorVentaDetalle);
                            k.Isc = x.Sum(h => h.Isc);
                            k.d_Igv = x.Sum(h => h.d_Igv);
                            k.d_IgvDetalle = x.Sum(h => h.d_IgvDetalle);

                            return k;

                        }).ToList();

                    ListaFinal1 = Temporal1.Concat(Temporal2).ToList();
                }
                else
                {

                    if (pstrt_Orden == "FECHAREGISTRO")
                    {
                        ListaFinal1 =
                            ListaFinal.ToList()
                            //.GroupBy(x => x.NumeroRegistro)
                                 .GroupBy(x => x.idCabecera)
                                .Select(x => x.First())
                                .OrderBy(o => o.TipoCompra)
                                .ThenBy(p => p.FechaEmision)
                                .ThenBy(p => p.Correlativo)
                                .ToList();
                    }
                    else if (pstrt_Orden == "CORRELATIVO")
                    {
                        ListaFinal1 =
                            ListaFinal.ToList()
                            //.GroupBy(x => x.NumeroRegistro)
                                 .GroupBy(x => x.idCabecera)
                                .Select(x => x.First())
                                .OrderBy(o => o.TipoCompra)
                                .ThenBy(p => p.Correlativo)
                                .ToList();
                    }
                    else if (pstrt_Orden == "FECHAEMISION")
                    {
                        ListaFinal1 =
                            ListaFinal.ToList()
                            //.GroupBy(x => x.NumeroRegistro)
                                  .GroupBy(x => x.idCabecera)
                                .Select(x => x.First())
                                .OrderBy(o => o.TipoCompra)
                                .ThenBy(p => p.FechaEmision)
                                .ToList();
                    }
                    else if (pstrt_Orden == "TIPODOCUMENTO")
                    {
                        ListaFinal1 =
                            ListaFinal.ToList()
                            //.GroupBy(x => x.NumeroRegistro)
                                .GroupBy(x => x.idCabecera)
                                .Select(x => x.First())
                                .OrderBy(o => o.TipoCompra)
                                .ThenBy(p => p.IdTipoDocumento)
                                .ThenBy(p => p.SerieDocumento)
                                .ThenBy(p => p.CorrelativoDocumento)
                                .ToList();
                    }
                    else if (pstrt_Orden == "NOMBREPROVEEDOR")
                    {
                        ListaFinal1 =
                            ListaFinal.ToList()
                            //.GroupBy(x => x.NumeroRegistro)
                                .GroupBy(x => x.idCabecera)
                                .Select(x => x.First())
                                .OrderBy(o => o.TipoCompra)
                                .ThenBy(p => p.NombreProveedor)
                                .ToList();
                    }
                }

                var index = 1;
                int MaximoCorrelativoCompras = queryCompras.Count > 0 ? int.Parse(queryCompras.Select(l => l.Correlativo).Max()) : 0;

                var QueryComprasLiquidacionesDictionary = queryCompras.Concat(queryLiquidacionCompra).GroupBy(g => g.idCabecera)
                    .ToDictionary(o => o.Key, hg => hg);

                IGrouping<string, ReporteRegistroCompraSunat> xz;

                #region Calculos
                foreach (var item in ListaFinal1)
                {
                    objOperationResult.Success = 1;
                    var detalles = QueryComprasLiquidacionesDictionary.TryGetValue(item.idCabecera, out xz) ? xz.ToList() : null;

                    if (ConsideraDocInternos == 0 && _objDocumentoBL.DocumentoEsContable(item.IdTipoDocumento))
                    {
                        if (!string.IsNullOrEmpty(item.NumeroRegistro) && item.NumeroRegistro.Length > 1)
                        {
                            string TipoOperacion = item.NumeroRegistro.Substring(0, 1);
                            if (TipoOperacion == "L")
                            {
                                MaximoCorrelativoCompras = MaximoCorrelativoCompras + 1;
                                item.Correlativo = MaximoCorrelativoCompras.ToString("0000000000");
                                item.CorrelativoPle = item.Correlativo.Substring(0, 4) + item.Correlativo.Trim().Substring(6, 4);
                            }
                        }

                        objReporte = new ReporteRegistroCompraSunat();

                        objReporte = item;
                        objReporte.sFechaEmision = item.FechaEmision.Date.ToShortDateString();
                        objReporte.TipoDocumento = item.IdTipoDocumento.ToString("000");

                        if (UsadoPDB)
                        {



                            objReporte.BaseImponible1 = item.Estado == 1 ? item.DestinoCabecera <= 4 ? item.d_ValorVenta : item.d_ValorVentaDetalle : 0;
                            objReporte.Isc = item.Estado == 1 ? item.DestinoCabecera <= 4 ? item.Isc : item.Isc : 0;
                            objReporte.Igv1 = item.Estado == 1 ? item.DestinoCabecera <= 4 ? item.d_Igv : item.d_IgvDetalle
                                : 0;


                        }
                        else
                        {

                            if (item.TipoCompra == "COMPRAS NACIONALES")
                            {

                                objReporte.BaseImponible1 = item.DestinoCabecera == 1
                                    ? item.Estado == 1
                                    ? Globals.ClientSession.i_Periodo == 2016 ? CalcularBaseImponibleIgv2016(ref objOperationResult, "B1", pintIdMoneda, item, detalles.ToList(), "",
                                    queryImportaciones.ToList()) : CalcularBaseImponibleIgv2017(ref objOperationResult, "B1", pintIdMoneda, item, detalles.ToList(), "",
                                    queryImportaciones.ToList())
                                        : 0
                                    : item.DestinoCabecera == 5 || item.DestinoCabecera == 999
                                        ? item.Estado == 1
                                            ? Globals.ClientSession.i_Periodo == 2016 ? CalcularBaseImponibleIgv2016(ref objOperationResult, "B51", pintIdMoneda, item, detalles.ToList(), "",
                                                queryImportaciones.ToList()) : CalcularBaseImponibleIgv2017(ref objOperationResult, "B51", pintIdMoneda, item, detalles.ToList(), "",
                                                queryImportaciones.ToList())
                                            : 0
                                        : 0;
                                objReporte.Igv1 = item.DestinoCabecera == 1
                                    ? item.Estado == 1
                                        ? Globals.ClientSession.i_Periodo == 2016 ? CalcularBaseImponibleIgv2016(ref objOperationResult, "I1", pintIdMoneda, item, detalles.ToList(), "",
                                        queryImportaciones.ToList()) : CalcularBaseImponibleIgv2017(ref objOperationResult, "I1", pintIdMoneda, item, detalles.ToList(), "",
                                        queryImportaciones.ToList())
                                        : 0
                                    : item.DestinoCabecera == 5 || item.DestinoCabecera == 999
                                        ? item.Estado == 1
                                        ? Globals.ClientSession.i_Periodo == 2016 ? CalcularBaseImponibleIgv2016(ref objOperationResult, "I51", pintIdMoneda, item, detalles.ToList(), "",
                                        queryImportaciones.ToList()) : CalcularBaseImponibleIgv2017(ref objOperationResult, "I51", pintIdMoneda, item, detalles.ToList(), "",
                                                queryImportaciones.ToList())
                                            : 0
                                        : 0;
                                objReporte.BaseImponible2 = item.DestinoCabecera == 2
                                    ? item.Estado == 1
                                        ? Globals.ClientSession.i_Periodo == 2016 ? CalcularBaseImponibleIgv2016(ref objOperationResult, "B2", pintIdMoneda, item, detalles.ToList(), "",
                                        queryImportaciones.ToList()) : CalcularBaseImponibleIgv2017(ref objOperationResult, "B2", pintIdMoneda, item, detalles.ToList(), "",
                                        queryImportaciones.ToList())
                                        : 0
                                    : item.DestinoCabecera == 5 || item.DestinoCabecera == 999
                                        ? item.Estado == 1
                                            ? Globals.ClientSession.i_Periodo == 2016 ? CalcularBaseImponibleIgv2016(ref objOperationResult, "B52", pintIdMoneda, item, detalles.ToList(), "",
                                            queryImportaciones.ToList()) : CalcularBaseImponibleIgv2017(ref objOperationResult, "B52", pintIdMoneda, item, detalles.ToList(), "",
                                            queryImportaciones.ToList())
                                            : 0
                                        : 0;
                                objReporte.Igv2 = item.DestinoCabecera == 2
                                    ? item.Estado == 1
                                        ? Globals.ClientSession.i_Periodo == 2016 ? CalcularBaseImponibleIgv2016(ref objOperationResult, "I2", pintIdMoneda, item, detalles.ToList(), "",
                                        queryImportaciones.ToList()) : CalcularBaseImponibleIgv2017(ref objOperationResult, "I2", pintIdMoneda, item, detalles.ToList(), "",
                                        queryImportaciones.ToList())
                                        : 0
                                    : item.DestinoCabecera == 5 || item.DestinoCabecera == 999
                                        ? item.Estado == 1
                                            ? Globals.ClientSession.i_Periodo == 2016 ? CalcularBaseImponibleIgv2016(ref objOperationResult, "I52", pintIdMoneda, item, detalles.ToList(), "",
                                            queryImportaciones.ToList()) : CalcularBaseImponibleIgv2017(ref objOperationResult, "I52", pintIdMoneda, item, detalles.ToList(), "",
                                                queryImportaciones.ToList())
                                            : 0
                                        : 0;
                                objReporte.BaseImponible3 = item.DestinoCabecera == 3
                                    ? item.Estado == 1
                                        ? Globals.ClientSession.i_Periodo == 2016 ? CalcularBaseImponibleIgv2016(ref objOperationResult, "B3", pintIdMoneda, item, detalles.ToList(), "",
                                        queryImportaciones.ToList()) : CalcularBaseImponibleIgv2017(ref objOperationResult, "B3", pintIdMoneda, item, detalles.ToList(), "",
                                        queryImportaciones.ToList())
                                        : 0
                                    : item.DestinoCabecera == 5 || item.DestinoCabecera == 999
                                        ? item.Estado == 1
                                            ? Globals.ClientSession.i_Periodo == 2016 ? CalcularBaseImponibleIgv2016(ref objOperationResult, "B53", pintIdMoneda, item, detalles.ToList(), "",
                                            queryImportaciones.ToList()) : CalcularBaseImponibleIgv2017(ref objOperationResult, "B53", pintIdMoneda, item, detalles.ToList(), "",
                                            queryImportaciones.ToList())
                                            : 0
                                        : 0;
                                objReporte.Igv3 = item.DestinoCabecera == 3
                                    ? item.Estado == 1
                                        ? Globals.ClientSession.i_Periodo == 2016 ? CalcularBaseImponibleIgv2016(ref objOperationResult, "I3", pintIdMoneda, item, detalles.ToList(), "",
                                        queryImportaciones.ToList()) : CalcularBaseImponibleIgv2017(ref objOperationResult, "I3", pintIdMoneda, item, detalles.ToList(), "",
                                        queryImportaciones.ToList())
                                        : 0
                                    : item.DestinoCabecera == 5 || item.DestinoCabecera == 999
                                        ? item.Estado == 1
                                            ? Globals.ClientSession.i_Periodo == 2016 ? CalcularBaseImponibleIgv2016(ref objOperationResult, "I53", pintIdMoneda, item, detalles.ToList(), "",
                                            queryImportaciones.ToList()) : CalcularBaseImponibleIgv2017(ref objOperationResult, "I53", pintIdMoneda, item, detalles.ToList(), "",
                                            queryImportaciones.ToList())
                                            : 0
                                        : 0;
                                objReporte.ValorAdquisiciones = item.DestinoCabecera == 4
                                    ? item.Estado == 1
                                        ? Globals.ClientSession.i_Periodo == 2016 ? CalcularBaseImponibleIgv2016(ref objOperationResult, "B4", pintIdMoneda, item, detalles.ToList(), "",
                                        queryImportaciones.ToList()) : CalcularBaseImponibleIgv2017(ref objOperationResult, "B4", pintIdMoneda, item, detalles.ToList(), "",
                                        queryImportaciones.ToList())
                                        : 0
                                    : item.DestinoCabecera == 5 || item.DestinoCabecera == 999
                                        ? item.Estado == 1
                                            ? Globals.ClientSession.i_Periodo == 2016 ? CalcularBaseImponibleIgv2016(ref objOperationResult, "B54", pintIdMoneda, item, detalles.ToList(), "",
                                            queryImportaciones.ToList()) : CalcularBaseImponibleIgv2017(ref objOperationResult, "B54", pintIdMoneda, item, detalles.ToList(), "",
                                                queryImportaciones.ToList())
                                            : 0
                                        : 0;
                                // Compras 2016
                                if (Globals.ClientSession.i_Periodo == 2016)
                                {
                                    objReporte.Total = item.Estado == 0 ? 0 : _objDocumentoBL.DocumentoEsInverso(item.IdTipoDocumento) ? pintIdMoneda == item.idMoneda ? item.Total * -1 : pintIdMoneda == (int)Currency.Soles ? Utils.Windows.DevuelveValorRedondeado((item.Total * item.TipoCambio * -1).Value, 2) : Utils.Windows.DevuelveValorRedondeado((item.Total / item.TipoCambio * -1).Value, 2) :
                                        pintIdMoneda == item.idMoneda ? item.Total : pintIdMoneda == (int)Currency.Soles ? Utils.Windows.DevuelveValorRedondeado((item.Total * item.TipoCambio).Value, 2) : Utils.Windows.DevuelveValorRedondeado((item.Total / item.TipoCambio).Value, 2);

                                }
                                else
                                {
                                    // Compras2017
                                    objReporte.Total = item.Estado == 0 ? 0 : _objDocumentoBL.DocumentoEsInverso(item.IdTipoDocumento) ? pintIdMoneda == item.idMoneda ? item.Total * -1 : CalcularBaseImponibleIgv2017(ref objOperationResult, "T", pintIdMoneda, item, detalles.ToList(), "", queryImportaciones.ToList()) * -1 :
                                       pintIdMoneda == item.idMoneda ? item.Total : CalcularBaseImponibleIgv2017(ref objOperationResult, "T", pintIdMoneda, item, detalles.ToList(), "", queryImportaciones.ToList());
                                }




                            }
                            else
                            {
                                objReporte.BaseImponible1 = item.Estado == 1
                                    ? Globals.ClientSession.i_Periodo == 2016 ? CalcularBaseImponibleIgv2016(ref objOperationResult, "B1", pintIdMoneda, item, queryCompras.Concat(queryLiquidacionCompra).ToList(), "1",
                                    queryImportaciones.ToList()) : CalcularBaseImponibleIgv2017(ref objOperationResult, "B1", pintIdMoneda, item, queryCompras.Concat(queryLiquidacionCompra).ToList(), "1",
                                        queryImportaciones.ToList())
                                    : 0;
                                objReporte.Igv1 = item.Estado == 1
                                    ? Globals.ClientSession.i_Periodo == 2016 ? CalcularBaseImponibleIgv2016(ref objOperationResult, "I1", pintIdMoneda, item, queryCompras.Concat(queryLiquidacionCompra).ToList(), "1",
                                    queryImportaciones.ToList()) : CalcularBaseImponibleIgv2017(ref objOperationResult, "I1", pintIdMoneda, item, queryCompras.Concat(queryLiquidacionCompra).ToList(), "1",
                                        queryImportaciones.ToList())
                                    : 0;
                                objReporte.BaseImponible2 = 0;
                                objReporte.Igv2 = 0;
                                objReporte.BaseImponible3 = 0;
                                objReporte.Igv3 = 0;
                                objReporte.ValorAdquisiciones = item.Estado == 1
                                    ? Globals.ClientSession.i_Periodo == 2016 ? CalcularBaseImponibleIgv2016(ref objOperationResult, "A", pintIdMoneda, item, queryCompras.ToList(), "4",
                                    queryImportaciones.ToList()) : CalcularBaseImponibleIgv2017(ref objOperationResult, "A", pintIdMoneda, item, queryCompras.ToList(), "4",
                                    queryImportaciones.ToList())
                                    : 0;
                                objReporte.Total = item.Estado == 1 ? objReporte.BaseImponible1 + objReporte.Igv1 + objReporte.BaseImponible2 +
                                             objReporte.Igv2 + objReporte.BaseImponible3 + objReporte.Igv3 +
                                             objReporte.ValorAdquisiciones : 0;
                            }
                            if (objOperationResult.Success == 0)
                            {

                                return null;
                            }
                        }

                        objReporte.LlaveOrdenar = pstrt_Orden == "CORRELATIVO"
                            ? item.Correlativo.Trim()
                            : "" + pstrt_Orden == "FECHAREGISTRO"
                                ? item.FechaEmision.ToString()
                                : "" + pstrt_Orden == "FECHAVENCIMIENTO"
                                    ? item.FechaVencimiento.ToString()
                                    : "" + pstrt_Orden == "TIPODOCUMENTO"
                                        ? item.TipoDocumento.Trim() + item.SerieDocumento.Trim() +
                                          item.CorrelativoDocumento.Trim()
                                        : "" + pstrt_Orden == "NOMBREPROVEEDOR" ? item.NombreProveedor : "";

                        objReporte.TipoDocumentoRef = item.IdTipoDocumento == 6 || item.IdTipoDocumento == 7 ||
                                                      item.IdTipoDocumento == 8 || item.IdTipoDocumento == 87 ||
                                                      item.IdTipoDocumento == 88 || item.IdTipoDocumento == 97 ||
                                                      item.IdTipoDocumento == 98
                            ? item.IdTipoDocumentoRef.Value.ToString("000")
                            : "";

                        objReporte.IgvNombre = "IGV AL " + item.IgvNombre + " :";

                        objReporte.Index = index;

                        objReporte.OtroTributos = objReporte.TipoCompra == "COMPRAS NACIONALES"
                            ? item.Estado == 1
                                ? Globals.ClientSession.i_Periodo == 2016 ? CalcularBaseImponibleIgv2016(ref objOperationResult, "OT", pintIdMoneda, item, queryCompras.ToList(), "1",
                                queryImportaciones.ToList()) : CalcularBaseImponibleIgv2017(ref objOperationResult, "OT", pintIdMoneda, item, queryCompras.ToList(), "1",
                                    queryImportaciones.ToList())
                                : 0
                            : 0;
                        objReporte.Isc = objReporte.TipoCompra == "COMPRAS NACIONALES"
                            ? item.Estado == 1
                                ? Globals.ClientSession.i_Periodo == 2016 ? CalcularBaseImponibleIgv2016(ref objOperationResult, "ISC", pintIdMoneda, item, queryCompras.ToList(), "1",
                                queryImportaciones.ToList()) : CalcularBaseImponibleIgv2017(ref objOperationResult, "ISC", pintIdMoneda, item, queryCompras.ToList(), "1",
                                    queryImportaciones.ToList())
                                : 0
                            : 0;

                        index = index + 1;
                        ListaReporteFinalizado.Add(objReporte);
                    }
                }

                if (pstrt_Orden == "FECHAREGISTRO")
                {
                    ListaReporteFinalizado =
                        ListaReporteFinalizado.ToList()
                            .OrderBy(o => o.TipoCompra)
                            .ThenBy(p => p.FechaEmision)
                            .ThenBy(p => p.Correlativo)
                            .ToList();
                }
                else if (pstrt_Orden == "CORRELATIVO")
                {
                    ListaReporteFinalizado =
                        ListaReporteFinalizado.ToList()
                            .OrderBy(o => o.TipoCompra)
                            .ThenBy(p => p.Correlativo)
                            .ToList();
                }
                else if (pstrt_Orden == "FECHAEMISION")
                {
                    ListaReporteFinalizado =
                        ListaReporteFinalizado.ToList()
                            .OrderBy(o => o.TipoCompra)
                            .ThenBy(p => p.FechaEmision)
                            .ToList();
                }
                else if (pstrt_Orden == "TIPODOCUMENTO")
                {
                    ListaReporteFinalizado =
                        ListaReporteFinalizado.ToList()
                            .OrderBy(o => o.TipoCompra)
                            .ThenBy(p => p.IdTipoDocumento)
                            .ThenBy(p => p.SerieDocumento)
                            .ThenBy(p => p.CorrelativoDocumento)
                            .ToList();
                }
                else if (pstrt_Orden == "NOMBREPROVEEDOR")
                {
                    ListaReporteFinalizado =
                        ListaReporteFinalizado.ToList()
                            .OrderBy(o => o.TipoCompra)
                            .ThenBy(p => p.NombreProveedor)
                            .ToList();
                }


                #endregion

                objOperationResult.Success = 1;
                return ListaReporteFinalizado;
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                // objOperationResult.AdditionalInformation = "ComprasBL.()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }
        }





        public bool BuscarConfiguracionAntesdeGenerarPle(DateTime FechaInicio, DateTime FechaFin)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var ExisteProveedor = false;

                var Documento = (from a in dbContext.documento
                                 join b in dbContext.cliente on new { cod = a.v_provimp_3i.Trim(), eliminado = 0, Flag = "V" } equals
                                     new { cod = b.v_CodCliente.Trim(), eliminado = b.i_Eliminado.Value, Flag = b.v_FlagPantalla } into
                                     b_join
                                 from b in b_join.DefaultIfEmpty()
                                 where
                                     b.v_NroDocIdentificacion == Constants.RucSunat &&
                                     a.i_CodigoDocumento == (int)TiposDocumentos.Importacion
                                 select b).FirstOrDefault();


                ExisteProveedor = Documento != null ? true : false;


                if (
                    !dbContext.importacion.Where(x => x.t_FechaRegistro >= FechaInicio && x.t_FechaRegistro <= FechaFin)
                        .Any())
                {
                    return true;
                }
                if (
                    dbContext.importacion.Where(x => x.t_FechaRegistro >= FechaInicio && x.t_FechaRegistro <= FechaFin)
                        .Any() && !ExisteProveedor)
                // if (dbContext.importacion.Where(x => x.t_FechaRegistro >= FechaInicio && x.t_FechaRegistro <= FechaFin).Any() && dbContext.documento.Where(x => x.i_CodigoDocumento == (int) TiposDocumentos.Importacion).FirstOrDefault().v_provimp_3i != Constants.RucSunat)
                {
                    return false;
                }

                return true;
            }
        }



        private decimal CalcularBaseImponibleIgv2016(ref  OperationResult objOperationResult, string TipoCalculo, int pintIdMonedaReporte,
            ReporteRegistroCompraSunat objReporte, List<ReporteRegistroCompraSunat> Compras, string Fila,
            List<ReporteRegistroCompraSunat> Importaciones)
        {

            decimal? ValorVenta = 0;


            try
            {
                objOperationResult.Success = 1;

                if (objReporte.TipoCompra == "COMPRAS NACIONALES")
                {
                    switch (objReporte.DestinoCabecera)
                    {
                        case 1:

                            if (_objDocumentoBL.DocumentoEsInverso(objReporte.IdTipoDocumento))
                            {
                                if (TipoCalculo == "B1")
                                {
                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? objReporte.d_ValorVenta * -1
                                        : pintIdMonedaReporte == (int)Currency.Soles
                                            ? objReporte.idMoneda == (int)Currency.Soles
                                                ? objReporte.d_ValorVenta * -1
                                                : objReporte.d_ValorVenta * objReporte.TipoCambio * -1
                                            : objReporte.idMoneda == (int)Currency.Dolares
                                                ? objReporte.d_ValorVenta * -1
                                                : objReporte.d_ValorVenta / objReporte.TipoCambio * -1;
                                }
                                else if (TipoCalculo == "I1")
                                {
                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? objReporte.d_Igv * -1
                                        : pintIdMonedaReporte == (int)Currency.Soles
                                            ? objReporte.idMoneda == (int)Currency.Soles
                                                ? objReporte.d_Igv * -1
                                                : objReporte.d_Igv * objReporte.TipoCambio * -1
                                            : objReporte.idMoneda == (int)Currency.Dolares
                                                ? objReporte.d_Igv * -1
                                                : objReporte.d_Igv / objReporte.TipoCambio * -1;
                                }
                            }
                            else
                            {
                                if (TipoCalculo == "B1")
                                {


                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? objReporte.d_ValorVenta
                                        : pintIdMonedaReporte == (int)Currency.Soles
                                            ? objReporte.idMoneda == (int)Currency.Soles
                                                ? objReporte.d_ValorVenta
                                                : objReporte.d_ValorVenta * objReporte.TipoCambio
                                            : objReporte.idMoneda == (int)Currency.Dolares
                                                ? objReporte.d_ValorVenta
                                                : objReporte.d_ValorVenta / objReporte.TipoCambio;
                                }
                                else if (TipoCalculo == "I1")
                                {
                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? objReporte.d_Igv
                                        : pintIdMonedaReporte == (int)Currency.Soles
                                            ? objReporte.idMoneda == (int)Currency.Soles
                                                ? objReporte.d_Igv
                                                : objReporte.d_Igv * objReporte.TipoCambio
                                            : objReporte.idMoneda == (int)Currency.Dolares
                                                ? objReporte.d_Igv
                                                : objReporte.d_Igv / objReporte.TipoCambio;
                                }
                            }

                            if (TipoCalculo == "OT")
                            {
                                ValorVenta = pintIdMonedaReporte == -1
                                    ? objReporte.OtroTributos
                                    : pintIdMonedaReporte == (int)Currency.Soles
                                        ? objReporte.idMoneda == (int)Currency.Soles
                                            ? objReporte.OtroTributos
                                            : objReporte.OtroTributos * objReporte.TipoCambio
                                        : objReporte.idMoneda == (int)Currency.Dolares
                                            ? objReporte.OtroTributos
                                            : objReporte.OtroTributos / objReporte.TipoCambio;
                            }

                            else if (TipoCalculo == "ISC")
                            {
                                ValorVenta = pintIdMonedaReporte == -1
                                    ? objReporte.Isc
                                    : pintIdMonedaReporte == (int)Currency.Soles
                                        ? objReporte.idMoneda == (int)Currency.Soles
                                            ? objReporte.Isc
                                            : objReporte.Isc * objReporte.TipoCambio
                                        : objReporte.idMoneda == (int)Currency.Dolares
                                            ? objReporte.Isc
                                            : objReporte.Isc / objReporte.TipoCambio;
                            }
                            break;

                        case 2:
                            if (_objDocumentoBL.DocumentoEsInverso(objReporte.IdTipoDocumento))
                            {
                                if (TipoCalculo == "B2")
                                {
                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? Compras
                                            .Sum(x => x.d_ValorVentaDetalle) * -1
                                        : pintIdMonedaReporte == (int)Currency.Soles ? objReporte.idMoneda == (int)Currency.Soles ? objReporte.d_ValorVenta * -1
                                               : objReporte.d_ValorVenta * objReporte.TipoCambio * -1
                                                : objReporte.idMoneda == (int)Currency.Soles
                                                ? objReporte.d_ValorVenta / objReporte.TipoCambio * -1
                                                : objReporte.d_ValorVenta * -1;
                                }
                                else if (TipoCalculo == "I2")
                                {
                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? Compras.Sum(x => x.d_IgvDetalle) *
                                          -1
                                        : pintIdMonedaReporte == (int)Currency.Soles
                                            ? objReporte.idMoneda == (int)Currency.Soles
                                                ? objReporte.d_Igv * -1
                                                : objReporte.d_Igv * objReporte.TipoCambio * -1
                                            : objReporte.idMoneda == (int)Currency.Soles
                                                ? objReporte.d_Igv / objReporte.TipoCambio * -1
                                                : objReporte.d_Igv * -1;
                                }


                            }
                            else
                            {
                                if (TipoCalculo == "B2")
                                {
                                    var bi2 = pintIdMonedaReporte == -1 ? objReporte.d_ValorVenta : pintIdMonedaReporte == objReporte.idMoneda ? objReporte.d_ValorVenta : pintIdMonedaReporte == (int)Currency.Soles ? objReporte.d_ValorVenta * objReporte.TipoCambio : objReporte.d_ValorVenta / objReporte.TipoCambio;
                                    //var bi2 = pintIdMonedaReporte == -1 ? objReporte.d_ValorVenta : pintIdMonedaReporte == objReporte.idMoneda ? objReporte.d_ValorVenta : CalcularTotalesOtraMoneda(ref  objOperationResult, Compras.Where(x => x.idCabecera == objReporte.idCabecera).ToList(), pintIdMonedaReporte, TipoCalculo);   // pintIdMonedaReporte == (int)Currency.Soles ? objReporte.d_ValorVenta * objReporte.TipoCambio : objReporte.d_ValorVenta / objReporte.TipoCambio;
                                    ValorVenta = bi2;
                                    if (objOperationResult.Success == 0)
                                    {
                                        return 0;
                                    }

                                }
                                else if (TipoCalculo == "I2")
                                {

                                    var i2 = pintIdMonedaReporte == -1 ? objReporte.d_Igv : pintIdMonedaReporte == objReporte.idMoneda ? objReporte.d_Igv : pintIdMonedaReporte == (int)Currency.Soles ? objReporte.d_Igv * objReporte.TipoCambio : objReporte.d_Igv / objReporte.TipoCambio;
                                    // var i2 = pintIdMonedaReporte == -1 ? objReporte.d_Igv : pintIdMonedaReporte == objReporte.idMoneda ? objReporte.d_Igv : CalcularTotalesOtraMoneda(ref objOperationResult, Compras.Where(x => x.idCabecera == objReporte.idCabecera).ToList(), pintIdMonedaReporte, TipoCalculo); //objReporte.d_Igv : pintIdMonedaReporte == (int)Currency.Soles ? objReporte.d_Igv * objReporte.TipoCambio : objReporte.d_Igv / objReporte.TipoCambio;
                                    ValorVenta = i2;
                                    if (objOperationResult.Success == 0)
                                    {
                                        return 0;
                                    }

                                }
                            }
                            if (TipoCalculo == "OT")
                            {
                                ValorVenta = pintIdMonedaReporte == -1
                                    ? objReporte.OtroTributos
                                    : pintIdMonedaReporte == (int)Currency.Soles
                                        ? objReporte.idMoneda == (int)Currency.Soles
                                            ? objReporte.OtroTributos
                                            : objReporte.OtroTributos * objReporte.TipoCambio
                                        : objReporte.idMoneda == (int)Currency.Dolares
                                            ? objReporte.OtroTributos
                                            : objReporte.OtroTributos / objReporte.TipoCambio;
                            }

                            else if (TipoCalculo == "ISC")
                            {
                                ValorVenta = pintIdMonedaReporte == -1
                                    ? objReporte.Isc
                                    : pintIdMonedaReporte == (int)Currency.Soles
                                        ? objReporte.idMoneda == (int)Currency.Soles
                                            ? objReporte.Isc
                                            : objReporte.Isc * objReporte.TipoCambio
                                        : objReporte.idMoneda == (int)Currency.Dolares
                                            ? objReporte.Isc
                                            : objReporte.Isc / objReporte.TipoCambio;
                            }
                            break;

                        case 3:

                            if (_objDocumentoBL.DocumentoEsInverso(objReporte.IdTipoDocumento))
                            {
                                if (TipoCalculo == "B3")
                                {
                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? objReporte.d_ValorVenta * -1
                                        : pintIdMonedaReporte == (int)Currency.Soles
                                            ? objReporte.idMoneda == (int)Currency.Soles
                                                ? objReporte.d_ValorVenta * -1
                                                : objReporte.d_ValorVenta * objReporte.TipoCambio * -1
                                            : objReporte.idMoneda == (int)Currency.Dolares
                                                ? objReporte.d_ValorVenta * -1
                                                : objReporte.d_ValorVenta / objReporte.TipoCambio * -1;
                                }
                                else if (TipoCalculo == "I3")
                                {
                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? objReporte.d_Igv * -1
                                        : pintIdMonedaReporte == (int)Currency.Soles
                                            ? objReporte.idMoneda == (int)Currency.Soles
                                                ? objReporte.d_Igv * -1
                                                : objReporte.d_Igv * objReporte.TipoCambio * -1
                                            : objReporte.idMoneda == (int)Currency.Dolares
                                                ? objReporte.d_Igv * -1
                                                : objReporte.d_Igv / objReporte.TipoCambio * -1;
                                }
                            }
                            else
                            {
                                if (TipoCalculo == "B3")
                                {
                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? objReporte.d_ValorVenta
                                        : pintIdMonedaReporte == (int)Currency.Soles
                                            ? objReporte.idMoneda == (int)Currency.Soles
                                                ? objReporte.d_ValorVenta
                                                : objReporte.d_ValorVenta * objReporte.TipoCambio
                                            : objReporte.idMoneda == (int)Currency.Dolares
                                                ? objReporte.d_ValorVenta
                                                : objReporte.d_ValorVenta / objReporte.TipoCambio;
                                }
                                else if (TipoCalculo == "I3")
                                {
                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? objReporte.d_Igv
                                        : pintIdMonedaReporte == (int)Currency.Soles
                                            ? objReporte.idMoneda == (int)Currency.Soles
                                                ? objReporte.d_Igv
                                                : objReporte.d_Igv * objReporte.TipoCambio
                                            : objReporte.idMoneda == (int)Currency.Dolares
                                                ? objReporte.d_Igv
                                                : objReporte.d_Igv / objReporte.TipoCambio;
                                }
                            }
                            if (TipoCalculo == "OT")
                            {
                                ValorVenta = pintIdMonedaReporte == -1
                                    ? objReporte.OtroTributos
                                    : pintIdMonedaReporte == (int)Currency.Soles
                                        ? objReporte.idMoneda == (int)Currency.Soles
                                            ? objReporte.OtroTributos
                                            : objReporte.OtroTributos * objReporte.TipoCambio
                                        : objReporte.idMoneda == (int)Currency.Dolares
                                            ? objReporte.OtroTributos
                                            : objReporte.OtroTributos / objReporte.TipoCambio;
                            }

                            else if (TipoCalculo == "ISC")
                            {
                                ValorVenta = pintIdMonedaReporte == -1
                                    ? objReporte.Isc
                                    : pintIdMonedaReporte == (int)Currency.Soles
                                        ? objReporte.idMoneda == (int)Currency.Soles
                                            ? objReporte.Isc
                                            : objReporte.Isc * objReporte.TipoCambio
                                        : objReporte.idMoneda == (int)Currency.Dolares
                                            ? objReporte.Isc
                                            : objReporte.Isc / objReporte.TipoCambio;
                            }
                            break;
                        case 4:

                            if (_objDocumentoBL.DocumentoEsInverso(objReporte.IdTipoDocumento))
                            {
                                if (TipoCalculo == "B4")
                                {
                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? objReporte.d_ValorVenta * -1
                                        : pintIdMonedaReporte == (int)Currency.Soles
                                            ? objReporte.idMoneda == (int)Currency.Soles
                                                ? objReporte.d_ValorVenta * -1
                                                : objReporte.d_ValorVenta * objReporte.TipoCambio * -1
                                            : objReporte.idMoneda == (int)Currency.Dolares
                                                ? objReporte.d_ValorVenta * -1
                                                : objReporte.d_ValorVenta / objReporte.TipoCambio * -1;
                                }
                            }
                            else
                            {
                                if (TipoCalculo == "B4")
                                {
                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? objReporte.d_ValorVenta
                                        : pintIdMonedaReporte == (int)Currency.Soles
                                            ? objReporte.idMoneda == (int)Currency.Soles
                                                ? objReporte.d_ValorVenta
                                                : objReporte.d_ValorVenta * objReporte.TipoCambio
                                            : objReporte.idMoneda == (int)Currency.Dolares
                                                ? objReporte.d_ValorVenta
                                                : objReporte.d_ValorVenta / objReporte.TipoCambio;
                                }
                            }
                            if (TipoCalculo == "OT")
                            {
                                ValorVenta = pintIdMonedaReporte == -1
                                    ? objReporte.OtroTributos
                                    : pintIdMonedaReporte == (int)Currency.Soles
                                        ? objReporte.idMoneda == (int)Currency.Soles
                                            ? objReporte.OtroTributos
                                            : objReporte.OtroTributos * objReporte.TipoCambio
                                        : objReporte.idMoneda == (int)Currency.Dolares
                                            ? objReporte.OtroTributos
                                            : objReporte.OtroTributos / objReporte.TipoCambio;
                            }

                            else if (TipoCalculo == "ISC")
                            {
                                ValorVenta = pintIdMonedaReporte == -1
                                    ? objReporte.Isc
                                    : pintIdMonedaReporte == (int)Currency.Soles
                                        ? objReporte.idMoneda == (int)Currency.Soles
                                            ? objReporte.Isc
                                            : objReporte.Isc * objReporte.TipoCambio
                                        : objReporte.idMoneda == (int)Currency.Dolares
                                            ? objReporte.Isc
                                            : objReporte.Isc / objReporte.TipoCambio;
                            }
                            break;

                        case 5:

                            if (_objDocumentoBL.DocumentoEsInverso(objReporte.IdTipoDocumento))
                            {
                                if (TipoCalculo == "B51")
                                {
                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? Compras.Where(x => x.DestinoGrilla == 1)
                                            .Sum(x => x.d_ValorVentaDetalle) * -1
                                        : pintIdMonedaReporte == (int)Currency.Soles
                                            ? objReporte.idMoneda == (int)Currency.Soles
                                                ? Compras.Where(
                                                    x => x.DestinoGrilla == 1)
                                                    .Sum(x => x.d_ValorVentaDetalle) * -1
                                                : Compras.Where(
                                                    x => x.DestinoGrilla == 1)
                                                    .Sum(x => x.d_ValorVentaDetalle) * objReporte.TipoCambio * -1
                                            : objReporte.idMoneda == (int)Currency.Soles
                                                ? Compras.Where(
                                                    x => x.DestinoGrilla == 1)
                                                    .Sum(x => x.d_ValorVentaDetalle) / objReporte.TipoCambio * -1
                                                : Compras.Where(
                                                    x => x.DestinoGrilla == 1)
                                                    .Sum(x => x.d_ValorVentaDetalle) * -1;
                                }
                                else if (TipoCalculo == "B52")
                                {
                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? Compras.Where(x => x.DestinoGrilla == 2)
                                            .Sum(x => x.d_ValorVentaDetalle) * -1
                                        : pintIdMonedaReporte == (int)Currency.Soles
                                            ? objReporte.idMoneda == (int)Currency.Soles
                                                ? Compras.Where(
                                                    x => x.DestinoGrilla == 2)
                                                    .Sum(x => x.d_ValorVentaDetalle) * -1
                                                : Compras.Where(
                                                    x => x.DestinoGrilla == 2)
                                                    .Sum(x => x.d_ValorVentaDetalle) * objReporte.TipoCambio * -1
                                            : objReporte.idMoneda == (int)Currency.Soles
                                                ? Compras.Where(
                                                    x => x.DestinoGrilla == 2)
                                                    .Sum(x => x.d_ValorVentaDetalle) / objReporte.TipoCambio * -1
                                                : Compras.Where(
                                                    x => x.DestinoGrilla == 2)
                                                    .Sum(x => x.d_ValorVentaDetalle) * -1;
                                }
                                else if (TipoCalculo == "B53")
                                {
                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? Compras.Where(x => x.idCabecera == objReporte.idCabecera && x.DestinoGrilla == 3)
                                            .Sum(x => x.d_ValorVentaDetalle) * -1
                                        : pintIdMonedaReporte == (int)Currency.Soles
                                            ? objReporte.idMoneda == (int)Currency.Soles
                                                ? Compras.Where(
                                                    x => x.DestinoGrilla == 3)
                                                    .Sum(x => x.d_ValorVentaDetalle) * -1
                                                : Compras.Where(
                                                    x => x.DestinoGrilla == 3)
                                                    .Sum(x => x.d_ValorVentaDetalle) * objReporte.TipoCambio * -1
                                            : objReporte.idMoneda == (int)Currency.Soles
                                                ? Compras.Where(
                                                    x => x.DestinoGrilla == 3)
                                                    .Sum(x => x.d_ValorVentaDetalle) / objReporte.TipoCambio * -1
                                                : Compras.Where(
                                                    x => x.DestinoGrilla == 3)
                                                    .Sum(x => x.d_ValorVentaDetalle) * -1;
                                }
                                else if (TipoCalculo == "B54")
                                {
                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? Compras.Where(x => x.DestinoGrilla == 4)
                                            .Sum(x => x.d_ValorVentaDetalle) * -1
                                        : pintIdMonedaReporte == (int)Currency.Soles
                                            ? objReporte.idMoneda == (int)Currency.Soles
                                                ? Compras.Where(
                                                    x => x.DestinoGrilla == 4)
                                                    .Sum(x => x.d_ValorVentaDetalle) * -1
                                                : Compras.Where(
                                                    x => x.DestinoGrilla == 4)
                                                    .Sum(x => x.d_ValorVentaDetalle) * objReporte.TipoCambio * -1
                                            : objReporte.idMoneda == (int)Currency.Soles
                                                ? Compras.Where(
                                                    x => x.DestinoGrilla == 4)
                                                    .Sum(x => x.d_ValorVentaDetalle) / objReporte.TipoCambio * -1
                                                : Compras.Where(
                                                    x => x.DestinoGrilla == 4)
                                                    .Sum(x => x.d_ValorVentaDetalle) * -1;
                                }
                                else if (TipoCalculo == "I51")
                                {
                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? Compras.Where(x => x.DestinoGrilla == 1)
                                            .Sum(x => x.d_IgvDetalle) * -1
                                        : pintIdMonedaReporte == (int)Currency.Soles
                                            ? objReporte.idMoneda == (int)Currency.Soles
                                                ? Compras.Where(
                                                    x => x.DestinoGrilla == 1)
                                                    .Sum(x => x.d_IgvDetalle) * -1
                                                : Compras.Where(
                                                    x => x.DestinoGrilla == 1)
                                                    .Sum(x => x.d_IgvDetalle) * objReporte.TipoCambio * -1
                                            : objReporte.idMoneda == (int)Currency.Soles
                                                ? Compras.Where(
                                                    x => x.DestinoGrilla == 1)
                                                    .Sum(x => x.d_IgvDetalle) / objReporte.TipoCambio * -1
                                                : Compras.Where(
                                                    x => x.DestinoGrilla == 1)
                                                    .Sum(x => x.d_IgvDetalle) * -1;
                                }
                                else if (TipoCalculo == "I52")
                                {
                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? Compras.Where(x => x.DestinoGrilla == 2)
                                            .Sum(x => x.d_IgvDetalle) * -1
                                        : pintIdMonedaReporte == (int)Currency.Soles
                                            ? objReporte.idMoneda == (int)Currency.Soles
                                                ? Compras.Where(
                                                    x => x.DestinoGrilla == 2)
                                                    .Sum(x => x.d_IgvDetalle) * -1
                                                : Compras.Where(
                                                    x => x.DestinoGrilla == 2)
                                                    .Sum(x => x.d_IgvDetalle) * objReporte.TipoCambio * -1
                                            : objReporte.idMoneda == (int)Currency.Soles
                                                ? Compras.Where(
                                                    x => x.DestinoGrilla == 2)
                                                    .Sum(x => x.d_IgvDetalle) / objReporte.TipoCambio * -1
                                                : Compras.Where(
                                                    x => x.DestinoGrilla == 2)
                                                    .Sum(x => x.d_IgvDetalle) * -1;
                                }
                                else if (TipoCalculo == "I53")
                                {
                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? Compras.Where(x => x.DestinoGrilla == 3)
                                            .Sum(x => x.d_IgvDetalle) * -1
                                        : pintIdMonedaReporte == (int)Currency.Soles
                                            ? objReporte.idMoneda == (int)Currency.Soles
                                                ? Compras.Where(
                                                    x => x.DestinoGrilla == 3)
                                                    .Sum(x => x.d_IgvDetalle) * -1
                                                : Compras.Where(
                                                    x => x.DestinoGrilla == 3)
                                                    .Sum(x => x.d_IgvDetalle) * objReporte.TipoCambio * 1
                                            : objReporte.idMoneda == (int)Currency.Soles
                                                ? Compras.Where(
                                                    x => x.DestinoGrilla == 3)
                                                    .Sum(x => x.d_IgvDetalle) / objReporte.TipoCambio * -1
                                                : Compras.Where(
                                                    x => x.DestinoGrilla == 3)
                                                    .Sum(x => x.d_IgvDetalle) * -1;
                                }
                                else if (TipoCalculo == "I54")
                                {
                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? Compras.Where(x => x.DestinoGrilla == 4)
                                            .Sum(x => x.d_IgvDetalle) * -1
                                        : pintIdMonedaReporte == (int)Currency.Soles
                                            ? objReporte.idMoneda == (int)Currency.Soles
                                                ? Compras.Where(
                                                    x => x.DestinoGrilla == 4)
                                                    .Sum(x => x.d_IgvDetalle) * -1
                                                : Compras.Where(
                                                    x => x.DestinoGrilla == 4)
                                                    .Sum(x => x.d_IgvDetalle) * objReporte.TipoCambio * -1
                                            : objReporte.idMoneda == (int)Currency.Soles
                                                ? Compras.Where(
                                                    x => x.DestinoGrilla == 4)
                                                    .Sum(x => x.d_IgvDetalle) / objReporte.TipoCambio * -1
                                                : Compras.Where(
                                                    x => x.DestinoGrilla == 4)
                                                    .Sum(x => x.d_IgvDetalle) * -1;
                                }
                            }
                            else // Solo para este caso agregar anticipo , ya que son por TipoDestino
                            {
                                if (TipoCalculo == "B51")
                                {
                                    var Anticipio = Compras.Where(x => x.DestinoGrilla == 1 && x.anticipoDetalle == 0).Sum(x => x.d_ValorVentaDetalle)
                                                  - Compras.Where(x => x.DestinoGrilla == 1 && x.anticipoDetalle == 1).Sum(x => x.d_ValorVentaDetalle);
                                    ValorVenta = pintIdMonedaReporte == -1 ? Anticipio : pintIdMonedaReporte == objReporte.idMoneda ? Anticipio : pintIdMonedaReporte == (int)Currency.Soles ? Anticipio * objReporte.TipoCambio : Anticipio / objReporte.TipoCambio;

                                }
                                else if (TipoCalculo == "B52")
                                {
                                    var Anticipio = Compras.Where(x => x.DestinoGrilla == 2 && x.anticipoDetalle == 0).Sum(x => x.d_ValorVentaDetalle)
                                                  - Compras.Where(x => x.DestinoGrilla == 2 && x.anticipoDetalle == 1)
                                            .Sum(x => x.d_ValorVentaDetalle);

                                    ValorVenta = pintIdMonedaReporte == -1 ? Anticipio : pintIdMonedaReporte == objReporte.idMoneda ? Anticipio : pintIdMonedaReporte == (int)Currency.Soles ? Anticipio * objReporte.TipoCambio : Anticipio / objReporte.TipoCambio;


                                }
                                else if (TipoCalculo == "B53")
                                {
                                    var Anticipio = Compras.Where(x => x.DestinoGrilla == 3 && x.anticipoDetalle == 0).Sum(x => x.d_ValorVentaDetalle)
                                                - Compras.Where(x => x.DestinoGrilla == 3 && x.anticipoDetalle == 1).Sum(x => x.d_ValorVentaDetalle);

                                    ValorVenta = pintIdMonedaReporte == -1 ? Anticipio : pintIdMonedaReporte == objReporte.idMoneda ? Anticipio : pintIdMonedaReporte == (int)Currency.Soles ? Anticipio * objReporte.TipoCambio : Anticipio / objReporte.TipoCambio;



                                }
                                else if (TipoCalculo == "B54")
                                {

                                    var Anticipio = Compras.Where(x => x.DestinoGrilla == 4 && x.anticipoDetalle == 0).Sum(x => x.d_ValorVentaDetalle)
                                                    - Compras.Where(x => x.DestinoGrilla == 4 && x.anticipoDetalle == 1).Sum(x => x.d_ValorVentaDetalle);
                                    ValorVenta = pintIdMonedaReporte == -1 ? Anticipio : pintIdMonedaReporte == objReporte.idMoneda ? Anticipio : pintIdMonedaReporte == (int)Currency.Soles ? Anticipio * objReporte.TipoCambio : Anticipio / objReporte.TipoCambio;

                                }
                                else if (TipoCalculo == "I51")
                                {
                                    var Anticipio = Compras.Where(x => x.DestinoGrilla == 1 && x.anticipoDetalle == 0).Sum(x => x.d_IgvDetalle)
                                                   - Compras.Where(x => x.DestinoGrilla == 1 && x.anticipoDetalle == 1).Sum(x => x.d_IgvDetalle);

                                    ValorVenta = pintIdMonedaReporte == -1 ? Anticipio : pintIdMonedaReporte == objReporte.idMoneda ? Anticipio : pintIdMonedaReporte == (int)Currency.Soles ? Anticipio * objReporte.TipoCambio : Anticipio / objReporte.TipoCambio;


                                }
                                else if (TipoCalculo == "I52")
                                {
                                    var Anticipio = Compras.Where(x => x.DestinoGrilla == 2 && x.anticipoDetalle == 0).Sum(x => x.d_IgvDetalle)
                                                   - Compras.Where(x => x.DestinoGrilla == 2 && x.anticipoDetalle == 1).Sum(x => x.d_IgvDetalle);
                                    ValorVenta = pintIdMonedaReporte == -1 ? Anticipio : pintIdMonedaReporte == objReporte.idMoneda ? Anticipio : pintIdMonedaReporte == (int)Currency.Soles ? Anticipio * objReporte.TipoCambio : Anticipio / objReporte.TipoCambio;


                                }
                                else if (TipoCalculo == "I53")
                                {


                                    var Anticipio = Compras.Where(x => x.DestinoGrilla == 3 && x.anticipoDetalle == 0).Sum(x => x.d_IgvDetalle)
                                                    - Compras.Where(x => x.DestinoGrilla == 3 && x.anticipoDetalle == 1).Sum(x => x.d_IgvDetalle);

                                    ValorVenta = pintIdMonedaReporte == -1 ? Anticipio : pintIdMonedaReporte == objReporte.idMoneda ? Anticipio : pintIdMonedaReporte == (int)Currency.Soles ? Anticipio * objReporte.TipoCambio : Anticipio / objReporte.TipoCambio;


                                }
                                else if (TipoCalculo == "I54")
                                {


                                    var Anticipio = Compras.Where(x => x.DestinoGrilla == 4 && x.anticipoDetalle == 0).Sum(x => x.d_IgvDetalle)
                                                - Compras.Where(x => x.DestinoGrilla == 4 && x.anticipoDetalle == 1).Sum(x => x.d_IgvDetalle);
                                    ValorVenta = pintIdMonedaReporte == -1 ? Anticipio : pintIdMonedaReporte == objReporte.idMoneda ? Anticipio : pintIdMonedaReporte == (int)Currency.Soles ? Anticipio * objReporte.TipoCambio : Anticipio / objReporte.TipoCambio;

                                }
                            }
                            if (TipoCalculo == "OT")
                            {
                                ValorVenta = pintIdMonedaReporte == -1
                                    ? objReporte.OtroTributos
                                    : pintIdMonedaReporte == (int)Currency.Soles
                                        ? objReporte.idMoneda == (int)Currency.Soles
                                            ? objReporte.OtroTributos
                                            : objReporte.OtroTributos * objReporte.TipoCambio
                                        : objReporte.idMoneda == (int)Currency.Dolares
                                            ? objReporte.OtroTributos
                                            : objReporte.OtroTributos / objReporte.TipoCambio;
                            }

                            else if (TipoCalculo == "ISC")
                            {
                                ValorVenta = pintIdMonedaReporte == -1
                                    ? objReporte.Isc
                                    : pintIdMonedaReporte == (int)Currency.Soles
                                        ? objReporte.idMoneda == (int)Currency.Soles
                                            ? objReporte.Isc
                                            : objReporte.Isc * objReporte.TipoCambio
                                        : objReporte.idMoneda == (int)Currency.Dolares
                                            ? objReporte.Isc
                                            : objReporte.Isc / objReporte.TipoCambio;
                            }
                            break;

                        case 999:


                            if (_objDocumentoBL.DocumentoEsInverso(objReporte.IdTipoDocumento))
                            //Solo para este caso agregar anticipo , ya que son por TipoDestino
                            {
                                if (TipoCalculo == "B51")
                                {
                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? Compras.Where(x => x.DestinoGrilla == 1)
                                            .Sum(x => x.d_ValorVentaDetalle) * -1
                                        : pintIdMonedaReporte == (int)Currency.Soles
                                            ? objReporte.idMoneda == (int)Currency.Soles
                                                ? Compras.Where(
                                                    x => x.DestinoGrilla == 1)
                                                    .Sum(x => x.d_ValorVentaDetalle) * -1
                                                : Compras.Where(
                                                    x => x.DestinoGrilla == 1)
                                                    .Sum(x => x.d_ValorVentaDetalle) * objReporte.TipoCambio * -1
                                            : objReporte.idMoneda == (int)Currency.Soles
                                                ? Compras.Where(
                                                    x => x.DestinoGrilla == 1)
                                                    .Sum(x => x.d_ValorVentaDetalle) / objReporte.TipoCambio * -1
                                                : Compras.Where(
                                                    x => x.DestinoGrilla == 1)
                                                    .Sum(x => x.d_ValorVentaDetalle) * -1;
                                }
                                else if (TipoCalculo == "B52")
                                {
                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? Compras.Where(x => x.DestinoGrilla == 2)
                                            .Sum(x => x.d_ValorVentaDetalle) * -1
                                        : pintIdMonedaReporte == (int)Currency.Soles
                                            ? objReporte.idMoneda == (int)Currency.Soles
                                                ? Compras.Where(
                                                    x => x.DestinoGrilla == 2)
                                                    .Sum(x => x.d_ValorVentaDetalle) * -1
                                                : Compras.Where(
                                                    x => x.DestinoGrilla == 2)
                                                    .Sum(x => x.d_ValorVentaDetalle) * objReporte.TipoCambio * -1
                                            : objReporte.idMoneda == (int)Currency.Soles
                                                ? Compras.Where(
                                                    x => x.DestinoGrilla == 2)
                                                    .Sum(x => x.d_ValorVentaDetalle) / objReporte.TipoCambio * -1
                                                : Compras.Where(
                                                    x => x.DestinoGrilla == 2)
                                                    .Sum(x => x.d_ValorVentaDetalle) * -1;
                                }
                                else if (TipoCalculo == "B53")
                                {
                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? Compras.Where(x => x.DestinoGrilla == 3)
                                            .Sum(x => x.d_ValorVentaDetalle) * -1
                                        : pintIdMonedaReporte == (int)Currency.Soles
                                            ? objReporte.idMoneda == (int)Currency.Soles
                                                ? Compras.Where(
                                                    x => x.DestinoGrilla == 3)
                                                    .Sum(x => x.d_ValorVentaDetalle) * -1
                                                : Compras.Where(
                                                    x => x.DestinoGrilla == 3)
                                                    .Sum(x => x.d_ValorVentaDetalle) * objReporte.TipoCambio * -1
                                            : objReporte.idMoneda == (int)Currency.Soles
                                                ? Compras.Where(
                                                    x => x.DestinoGrilla == 3)
                                                    .Sum(x => x.d_ValorVentaDetalle) / objReporte.TipoCambio * -1
                                                : Compras.Where(
                                                    x => x.DestinoGrilla == 3)
                                                    .Sum(x => x.d_ValorVentaDetalle) * -1;
                                }
                                else if (TipoCalculo == "B54")
                                {
                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? Compras.Where(x => x.DestinoGrilla == 4)
                                            .Sum(x => x.d_ValorVentaDetalle) * -1
                                        : pintIdMonedaReporte == (int)Currency.Soles
                                            ? objReporte.idMoneda == (int)Currency.Soles
                                                ? Compras.Where(
                                                    x => x.DestinoGrilla == 4)
                                                    .Sum(x => x.d_ValorVentaDetalle) * -1
                                                : Compras.Where(
                                                    x => x.DestinoGrilla == 4)
                                                    .Sum(x => x.d_ValorVentaDetalle) * objReporte.TipoCambio * -1
                                            : objReporte.idMoneda == (int)Currency.Soles
                                                ? Compras.Where(
                                                    x => x.DestinoGrilla == 4)
                                                    .Sum(x => x.d_ValorVentaDetalle) / objReporte.TipoCambio * -1
                                                : Compras.Where(
                                                    x => x.DestinoGrilla == 4)
                                                    .Sum(x => x.d_ValorVentaDetalle) * -1;
                                }
                                else if (TipoCalculo == "I51")
                                {
                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? Compras.Where(x => x.DestinoGrilla == 1)
                                            .Sum(x => x.d_IgvDetalle) * -1
                                        : pintIdMonedaReporte == (int)Currency.Soles
                                            ? objReporte.idMoneda == (int)Currency.Soles
                                                ? Compras.Where(
                                                    x => x.DestinoGrilla == 1)
                                                    .Sum(x => x.d_IgvDetalle) * -1
                                                : Compras.Where(
                                                    x => x.DestinoGrilla == 1)
                                                    .Sum(x => x.d_IgvDetalle) * objReporte.TipoCambio * -1
                                            : objReporte.idMoneda == (int)Currency.Soles
                                                ? Compras.Where(
                                                    x => x.DestinoGrilla == 1)
                                                    .Sum(x => x.d_IgvDetalle) / objReporte.TipoCambio * -1
                                                : Compras.Where(
                                                    x => x.DestinoGrilla == 1)
                                                    .Sum(x => x.d_IgvDetalle) * -1;
                                }
                                else if (TipoCalculo == "I52")
                                {
                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? Compras.Where(x => x.DestinoGrilla == 2)
                                            .Sum(x => x.d_IgvDetalle) * -1
                                        : pintIdMonedaReporte == (int)Currency.Soles
                                            ? objReporte.idMoneda == (int)Currency.Soles
                                                ? Compras.Where(
                                                    x => x.DestinoGrilla == 2)
                                                    .Sum(x => x.d_IgvDetalle) * -1
                                                : Compras.Where(
                                                    x => x.DestinoGrilla == 2)
                                                    .Sum(x => x.d_IgvDetalle) * objReporte.TipoCambio * -1
                                            : objReporte.idMoneda == (int)Currency.Soles
                                                ? Compras.Where(
                                                    x => x.DestinoGrilla == 2)
                                                    .Sum(x => x.d_IgvDetalle) / objReporte.TipoCambio * -1
                                                : Compras.Where(
                                                    x => x.DestinoGrilla == 2)
                                                    .Sum(x => x.d_IgvDetalle) * -1;
                                }
                                else if (TipoCalculo == "I53")
                                {
                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? Compras.Where(x => x.DestinoGrilla == 3)
                                            .Sum(x => x.d_IgvDetalle) * -1
                                        : pintIdMonedaReporte == (int)Currency.Soles
                                            ? objReporte.idMoneda == (int)Currency.Soles
                                                ? Compras.Where(
                                                    x => x.DestinoGrilla == 3)
                                                    .Sum(x => x.d_IgvDetalle) * -1
                                                : Compras.Where(
                                                    x => x.DestinoGrilla == 3)
                                                    .Sum(x => x.d_IgvDetalle) * objReporte.TipoCambio * 1
                                            : objReporte.idMoneda == (int)Currency.Soles
                                                ? Compras.Where(
                                                    x => x.DestinoGrilla == 3)
                                                    .Sum(x => x.d_IgvDetalle) / objReporte.TipoCambio * -1
                                                : Compras.Where(
                                                    x => x.DestinoGrilla == 3)
                                                    .Sum(x => x.d_IgvDetalle) * -1;
                                }
                                else if (TipoCalculo == "I54")
                                {
                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? Compras.Where(x => x.DestinoGrilla == 4)
                                            .Sum(x => x.d_IgvDetalle) * -1
                                        : pintIdMonedaReporte == (int)Currency.Soles
                                            ? objReporte.idMoneda == (int)Currency.Soles
                                                ? Compras.Where(
                                                    x => x.DestinoGrilla == 4)
                                                    .Sum(x => x.d_IgvDetalle) * -1
                                                : Compras.Where(
                                                    x => x.DestinoGrilla == 4)
                                                    .Sum(x => x.d_IgvDetalle) * objReporte.TipoCambio * -1
                                            : objReporte.idMoneda == (int)Currency.Soles
                                                ? Compras.Where(
                                                    x => x.DestinoGrilla == 4)
                                                    .Sum(x => x.d_IgvDetalle) / objReporte.TipoCambio * -1
                                                : Compras.Where(
                                                    x => x.DestinoGrilla == 4)
                                                    .Sum(x => x.d_IgvDetalle) * -1;
                                }
                            }
                            else // Solo para este caso agregar anticipo , ya que son por TipoDestino
                            {
                                if (TipoCalculo == "B51")
                                {


                                    var Anticipio = Compras.Where(x => x.DestinoGrilla == 1 && x.anticipoDetalle == 0).Sum(x => x.d_ValorVentaDetalle)
                                                  - Compras.Where(x => x.DestinoGrilla == 1 && x.anticipoDetalle == 1).Sum(x => x.d_ValorVentaDetalle);
                                    ValorVenta = pintIdMonedaReporte == -1 ? Anticipio : pintIdMonedaReporte == objReporte.idMoneda ? Anticipio : pintIdMonedaReporte == (int)Currency.Soles ? Anticipio * objReporte.TipoCambio : Anticipio / objReporte.TipoCambio;
                                }
                                else if (TipoCalculo == "B52")
                                {
                                    var Anticipio = Compras.Where(x => x.DestinoGrilla == 2 && x.anticipoDetalle == 0).Sum(x => x.d_ValorVentaDetalle)
                                                   - Compras.Where(x => x.DestinoGrilla == 2 && x.anticipoDetalle == 1).Sum(x => x.d_ValorVentaDetalle);

                                    ValorVenta = pintIdMonedaReporte == -1 ? Anticipio : pintIdMonedaReporte == objReporte.idMoneda ? Anticipio : pintIdMonedaReporte == (int)Currency.Soles ? Anticipio * objReporte.TipoCambio : Anticipio / objReporte.TipoCambio;
                                }
                                else if (TipoCalculo == "B53")
                                {
                                    var Anticipio = Compras.Where(x => x.DestinoGrilla == 3 && x.anticipoDetalle == 0).Sum(x => x.d_ValorVentaDetalle)
                                                  - Compras.Where(x => x.DestinoGrilla == 3 && x.anticipoDetalle == 1).Sum(x => x.d_ValorVentaDetalle);
                                    ValorVenta = pintIdMonedaReporte == -1 ? Anticipio : pintIdMonedaReporte == objReporte.idMoneda ? Anticipio : pintIdMonedaReporte == (int)Currency.Soles ? Anticipio * objReporte.TipoCambio : Anticipio / objReporte.TipoCambio;
                                }
                                else if (TipoCalculo == "B54")
                                {


                                    var Anticipio = Compras.Where(x => x.DestinoGrilla == 4 && x.anticipoDetalle == 0).Sum(x => x.d_ValorVentaDetalle)
                                                 - Compras.Where(x => x.DestinoGrilla == 4 && x.anticipoDetalle == 1).Sum(x => x.d_ValorVentaDetalle);
                                    ValorVenta = pintIdMonedaReporte == -1 ? Anticipio : pintIdMonedaReporte == objReporte.idMoneda ? Anticipio : pintIdMonedaReporte == (int)Currency.Soles ? Anticipio * objReporte.TipoCambio : Anticipio / objReporte.TipoCambio;

                                }
                                else if (TipoCalculo == "I51")
                                {



                                    var Anticipio = Compras.Where(x => x.DestinoGrilla == 1 && x.anticipoDetalle == 0).Sum(x => x.d_IgvDetalle)
                                                   - Compras.Where(x => x.DestinoGrilla == 1 && x.anticipoDetalle == 1).Sum(x => x.d_IgvDetalle);

                                    ValorVenta = pintIdMonedaReporte == -1 ? Anticipio : pintIdMonedaReporte == objReporte.idMoneda ? Anticipio : pintIdMonedaReporte == (int)Currency.Soles ? Anticipio * objReporte.TipoCambio : Anticipio / objReporte.TipoCambio;

                                }
                                else if (TipoCalculo == "I52")
                                {
                                    var Anticipio = Compras.Where(x => x.DestinoGrilla == 2 && x.anticipoDetalle == 0).Sum(x => x.d_IgvDetalle)
                                                    - Compras.Where(x => x.DestinoGrilla == 2 && x.anticipoDetalle == 1).Sum(x => x.d_IgvDetalle);
                                    ValorVenta = pintIdMonedaReporte == -1 ? Anticipio : pintIdMonedaReporte == objReporte.idMoneda ? Anticipio : pintIdMonedaReporte == (int)Currency.Soles ? Anticipio * objReporte.TipoCambio : Anticipio / objReporte.TipoCambio;


                                }
                                else if (TipoCalculo == "I53")
                                {

                                    var Anticipio = Compras.Where(x => x.DestinoGrilla == 3 && x.anticipoDetalle == 0).Sum(x => x.d_IgvDetalle)
                                                - Compras.Where(x => x.DestinoGrilla == 3 && x.anticipoDetalle == 1).Sum(x => x.d_IgvDetalle);

                                    ValorVenta = pintIdMonedaReporte == -1 ? Anticipio : pintIdMonedaReporte == objReporte.idMoneda ? Anticipio : pintIdMonedaReporte == (int)Currency.Soles ? Anticipio * objReporte.TipoCambio : Anticipio / objReporte.TipoCambio;
                                }
                                else if (TipoCalculo == "I54")
                                {

                                    var Anticipio = Compras.Where(x => x.DestinoGrilla == 4 && x.anticipoDetalle == 0).Sum(x => x.d_IgvDetalle)
                                        - Compras.Where(x => x.DestinoGrilla == 4 && x.anticipoDetalle == 1).Sum(x => x.d_IgvDetalle);

                                    ValorVenta = pintIdMonedaReporte == -1 ? Anticipio : pintIdMonedaReporte == objReporte.idMoneda ? Anticipio : pintIdMonedaReporte == (int)Currency.Soles ? Anticipio * objReporte.TipoCambio : Anticipio / objReporte.TipoCambio;
                                }
                            }
                            if (TipoCalculo == "OT")
                            {
                                ValorVenta = pintIdMonedaReporte == -1
                                    ? objReporte.OtroTributos
                                    : pintIdMonedaReporte == (int)Currency.Soles
                                        ? objReporte.idMoneda == (int)Currency.Soles
                                            ? objReporte.OtroTributos
                                            : objReporte.OtroTributos * objReporte.TipoCambio
                                        : objReporte.idMoneda == (int)Currency.Dolares
                                            ? objReporte.OtroTributos
                                            : objReporte.OtroTributos / objReporte.TipoCambio;
                            }

                            else if (TipoCalculo == "ISC")
                            {
                                ValorVenta = pintIdMonedaReporte == -1
                                    ? objReporte.Isc
                                    : pintIdMonedaReporte == (int)Currency.Soles
                                        ? objReporte.idMoneda == (int)Currency.Soles
                                            ? objReporte.Isc
                                            : objReporte.Isc * objReporte.TipoCambio
                                        : objReporte.idMoneda == (int)Currency.Dolares
                                            ? objReporte.Isc
                                            : objReporte.Isc / objReporte.TipoCambio;
                            }
                            break;
                    }
                }
                else
                {
                    if (TipoCalculo == "B1" && Fila == "1")
                    {
                        var IgvPorcentaje = decimal.Parse(objReporte.porcentajeIgv) / 100;
                        ValorVenta = pintIdMonedaReporte == -1
                            ? objReporte.d_Igv / IgvPorcentaje
                            : pintIdMonedaReporte == (int)Currency.Soles
                                ? objReporte.idMoneda == (int)Currency.Soles
                                    ? objReporte.d_Igv / IgvPorcentaje
                                    : objReporte.d_Igv / IgvPorcentaje * objReporte.TipoCambio
                                : objReporte.idMoneda == (int)Currency.Dolares
                                    ? objReporte.d_Igv / IgvPorcentaje
                                    : objReporte.d_Igv / IgvPorcentaje / objReporte.TipoCambio;
                    }

                    if (TipoCalculo == "I1" && Fila == "1")
                    {
                        var IgvPorcentaje = decimal.Parse(objReporte.porcentajeIgv);
                        ValorVenta = pintIdMonedaReporte == -1
                            ? objReporte.d_Igv
                            : pintIdMonedaReporte == (int)Currency.Soles
                                ? objReporte.idMoneda == (int)Currency.Soles
                                    ? objReporte.d_Igv
                                    : objReporte.d_Igv * objReporte.TipoCambio
                                : objReporte.idMoneda == (int)Currency.Dolares
                                    ? objReporte.d_Igv
                                    : objReporte.d_Igv / objReporte.TipoCambio;
                    }
                    if (TipoCalculo == "A" && Fila == "4")
                    {
                        var queryExistencia =
                            Importaciones.Where(
                                x =>
                                    x.CompraGeneradaImportacion != "" &&
                                    x.CompraGeneradaImportacion == objReporte.CompraGeneradaImportacion).ToList();
                        if (queryExistencia.Count() > 0)
                        {
                            var CompraGenerada =
                                Compras.Where(
                                    x =>
                                        x.CompraGeneradaImportacion ==
                                        queryExistencia.FirstOrDefault().CompraGeneradaImportacion);
                            if (queryExistencia.FirstOrDefault().TipoDocGenerado == objReporte.IdTipoDocumento &&
                                queryExistencia.FirstOrDefault().serieDocumentoGenerado == objReporte.SerieDocumento &&
                                queryExistencia.FirstOrDefault().correlativoDocumentoGenerado ==
                                objReporte.CorrelativoDocumento && CompraGenerada.FirstOrDefault().DestinoCabecera == 4)
                            {
                                ValorVenta = pintIdMonedaReporte == -1
                                    ? queryExistencia.FirstOrDefault().TotalSolesGenerado
                                    : pintIdMonedaReporte == 1
                                        ? queryExistencia.FirstOrDefault().TotalSolesGenerado
                                        : queryExistencia.FirstOrDefault().TotalDolaresGenerado;
                            }
                            else
                            {
                                ValorVenta = 0;
                            }
                        }
                        else
                        {
                            ValorVenta = 0;
                        }
                    }
                }


                return Utils.Windows.DevuelveValorRedondeado((decimal)ValorVenta, 2);
            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                return 0;

            }
        }


        /// <summary>
        /// Nuevo Calculo de Base Imponible
        /// </summary>
        /// <param name="objOperationResult"></param>
        /// <param name="TipoCalculo"></param>
        /// <param name="pintIdMonedaReporte"></param>
        /// <param name="objReporte"></param>
        /// <param name="Compras"></param>
        /// <param name="Fila"></param>
        /// <param name="Importaciones"></param>
        /// <returns></returns>

        private decimal CalcularBaseImponibleIgv2017(ref  OperationResult objOperationResult, string TipoCalculo, int pintIdMonedaReporte,
                    ReporteRegistroCompraSunat objReporte, List<ReporteRegistroCompraSunat> Compras, string Fila,
                    List<ReporteRegistroCompraSunat> Importaciones)
        {

            decimal? ValorVenta = 0;
            try
            {
                objOperationResult.Success = 1;
                if (TipoCalculo == "T")
                {

                    ValorVenta = CalcularTotalesOtraMoneda(ref objOperationResult, Compras.ToList(), pintIdMonedaReporte, TipoCalculo); //objReporte.d_Igv : pintIdMonedaReporte == (int)Currency.Soles ? objReporte.d_Igv * objReporte.TipoCambio : objReporte.d_Igv / objReporte.TipoCambio;  CalcularTotalesOtraMoneda(ref objOperationResult, Compras.Where(x => x.idCabecera == objReporte.idCabecera).ToList(), pintIdMonedaReporte, TipoCalculo); //objReporte.d_Igv : pintIdMonedaReporte == (int)Currency.Soles ? objReporte.d_Igv * objReporte.TipoCambio : objReporte.d_Igv / objReporte.TipoCambio;
                }
                else
                {
                    if (objReporte.TipoCompra == "COMPRAS NACIONALES")
                    {
                        switch (objReporte.DestinoCabecera)
                        {
                            case 1:

                                #region caso 1
                                if (_objDocumentoBL.DocumentoEsInverso(objReporte.IdTipoDocumento))
                                {
                                    if (TipoCalculo == "B1")
                                    {



                                        ValorVenta = pintIdMonedaReporte == -1
                                           ? objReporte.d_ValorVenta * -1
                                           : pintIdMonedaReporte == objReporte.idMoneda ?
                                                   objReporte.d_ValorVenta * -1
                                                   : CalcularTotalesOtraMoneda(ref  objOperationResult, Compras, pintIdMonedaReporte, TipoCalculo) * -1;


                                    }
                                    else if (TipoCalculo == "I1")
                                    {


                                        ValorVenta = pintIdMonedaReporte == -1
                                            ? objReporte.d_Igv * -1
                                            : pintIdMonedaReporte == objReporte.idMoneda ?
                                         objReporte.d_Igv * -1 : CalcularTotalesOtraMoneda(ref  objOperationResult, Compras, pintIdMonedaReporte, TipoCalculo) * -1;



                                    }
                                }
                                else
                                {
                                    if (TipoCalculo == "B1")
                                    {


                                        ValorVenta = pintIdMonedaReporte == -1 ? objReporte.d_ValorVenta : pintIdMonedaReporte == objReporte.idMoneda ? objReporte.d_ValorVenta : CalcularTotalesOtraMoneda(ref  objOperationResult, Compras, pintIdMonedaReporte, TipoCalculo);


                                    }
                                    else if (TipoCalculo == "I1")
                                    {


                                        ValorVenta = pintIdMonedaReporte == -1 ? objReporte.d_Igv : pintIdMonedaReporte == objReporte.idMoneda ? objReporte.d_Igv : CalcularTotalesOtraMoneda(ref  objOperationResult, Compras, pintIdMonedaReporte, TipoCalculo);



                                    }
                                }

                                if (TipoCalculo == "OT")
                                {


                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? objReporte.OtroTributos
                                        : pintIdMonedaReporte == objReporte.idMoneda ?
                                                 objReporte.OtroTributos
                                                : CalcularTotalesOtraMoneda(ref  objOperationResult, Compras, pintIdMonedaReporte, TipoCalculo);


                                }

                                else if (TipoCalculo == "ISC")
                                {

                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? objReporte.Isc
                                        : pintIdMonedaReporte == objReporte.idMoneda ?
                                     objReporte.Isc : CalcularTotalesOtraMoneda(ref  objOperationResult, Compras, pintIdMonedaReporte, TipoCalculo);



                                }
                                break;
                                #endregion
                            case 2:
                                #region caso 2
                                if (_objDocumentoBL.DocumentoEsInverso(objReporte.IdTipoDocumento))
                                {
                                    if (TipoCalculo == "B2")
                                    {


                                        ValorVenta = pintIdMonedaReporte == -1
                                            ? objReporte.d_ValorVenta * -1 :
                                             pintIdMonedaReporte == objReporte.idMoneda ? objReporte.d_ValorVenta * -1
                                                   : CalcularTotalesOtraMoneda(ref  objOperationResult, Compras, pintIdMonedaReporte, TipoCalculo) * -1;

                                    }
                                    else if (TipoCalculo == "I2")
                                    {
                                        ValorVenta = pintIdMonedaReporte == -1
                                            ? objReporte.d_Igv * -1 : pintIdMonedaReporte == objReporte.idMoneda ? objReporte.d_Igv * -1
                                            : CalcularTotalesOtraMoneda(ref  objOperationResult, Compras, pintIdMonedaReporte, TipoCalculo) * -1;

                                    }


                                }
                                else
                                {
                                    if (TipoCalculo == "B2")
                                    {

                                        var bi2 = pintIdMonedaReporte == -1 ? objReporte.d_ValorVenta : pintIdMonedaReporte == objReporte.idMoneda ? objReporte.d_ValorVenta : CalcularTotalesOtraMoneda(ref  objOperationResult, Compras, pintIdMonedaReporte, TipoCalculo);   // pintIdMonedaReporte == (int)Currency.Soles ? objReporte.d_ValorVenta * objReporte.TipoCambio : objReporte.d_ValorVenta / objReporte.TipoCambio;
                                        ValorVenta = bi2;
                                        if (objOperationResult.Success == 0)
                                        {
                                            return 0;
                                        }

                                    }
                                    else if (TipoCalculo == "I2")
                                    {


                                        var i2 = pintIdMonedaReporte == -1 ? objReporte.d_Igv : pintIdMonedaReporte == objReporte.idMoneda ? objReporte.d_Igv : CalcularTotalesOtraMoneda(ref objOperationResult, Compras, pintIdMonedaReporte, TipoCalculo); //objReporte.d_Igv : pintIdMonedaReporte == (int)Currency.Soles ? objReporte.d_Igv * objReporte.TipoCambio : objReporte.d_Igv / objReporte.TipoCambio;
                                        ValorVenta = i2;
                                        if (objOperationResult.Success == 0)
                                        {
                                            return 0;
                                        }

                                    }
                                }
                                if (TipoCalculo == "OT")
                                {

                                    ValorVenta = pintIdMonedaReporte == -1
                                       ? objReporte.OtroTributos
                                       : pintIdMonedaReporte == objReporte.idMoneda ?
                                       objReporte.OtroTributos : CalcularTotalesOtraMoneda(ref  objOperationResult, Compras, pintIdMonedaReporte, TipoCalculo);

                                }

                                else if (TipoCalculo == "ISC")
                                {


                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? objReporte.Isc
                                        : pintIdMonedaReporte == objReporte.idMoneda ?
                                                 objReporte.Isc
                                                : CalcularTotalesOtraMoneda(ref  objOperationResult, Compras, pintIdMonedaReporte, TipoCalculo);



                                }
                                break;
                                #endregion
                            case 3:
                                #region caso 3
                                if (_objDocumentoBL.DocumentoEsInverso(objReporte.IdTipoDocumento))
                                {
                                    if (TipoCalculo == "B3")
                                    {

                                        ValorVenta = pintIdMonedaReporte == -1
                                            ? objReporte.d_ValorVenta * -1
                                            : pintIdMonedaReporte == objReporte.idMoneda
                                                    ? objReporte.d_ValorVenta * -1
                                                    : CalcularTotalesOtraMoneda(ref  objOperationResult, Compras, pintIdMonedaReporte, TipoCalculo) * -1;


                                    }
                                    else if (TipoCalculo == "I3")
                                    {


                                        ValorVenta = pintIdMonedaReporte == -1
                                            ? objReporte.d_Igv * -1
                                            : pintIdMonedaReporte == objReporte.idMoneda
                                                    ? objReporte.d_Igv * -1
                                                    : CalcularTotalesOtraMoneda(ref  objOperationResult, Compras, pintIdMonedaReporte, TipoCalculo) * -1;

                                    }
                                }
                                else
                                {
                                    if (TipoCalculo == "B3")
                                    {



                                        ValorVenta = pintIdMonedaReporte == -1
                                           ? objReporte.d_ValorVenta
                                           : pintIdMonedaReporte == objReporte.idMoneda
                                                   ? objReporte.d_ValorVenta
                                                   : CalcularTotalesOtraMoneda(ref  objOperationResult, Compras, pintIdMonedaReporte, TipoCalculo);

                                    }
                                    else if (TipoCalculo == "I3")
                                    {


                                        ValorVenta = pintIdMonedaReporte == -1
                                           ? objReporte.d_Igv
                                           : pintIdMonedaReporte == objReporte.idMoneda
                                                   ? objReporte.d_Igv
                                                   : CalcularTotalesOtraMoneda(ref  objOperationResult, Compras, pintIdMonedaReporte, TipoCalculo);
                                    }
                                }
                                if (TipoCalculo == "OT")
                                {

                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? objReporte.OtroTributos
                                        : pintIdMonedaReporte == objReporte.idMoneda ? objReporte.OtroTributos
                                                : CalcularTotalesOtraMoneda(ref  objOperationResult, Compras, pintIdMonedaReporte, TipoCalculo);
                                }

                                else if (TipoCalculo == "ISC")
                                {

                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? objReporte.Isc
                                        : pintIdMonedaReporte == objReporte.idMoneda ? objReporte.Isc
                                                : CalcularTotalesOtraMoneda(ref  objOperationResult, Compras, pintIdMonedaReporte, TipoCalculo);



                                }
                                break;
                                #endregion
                            case 4:
                                #region caso4
                                if (_objDocumentoBL.DocumentoEsInverso(objReporte.IdTipoDocumento))
                                {
                                    if (TipoCalculo == "B4")
                                    {


                                        ValorVenta = pintIdMonedaReporte == -1
                                            ? objReporte.d_ValorVenta * -1
                                            : pintIdMonedaReporte == objReporte.idMoneda ? objReporte.d_ValorVenta * -1
                                                    : CalcularTotalesOtraMoneda(ref  objOperationResult, Compras, pintIdMonedaReporte, TipoCalculo) * -1;
                                    }
                                }
                                else
                                {
                                    if (TipoCalculo == "B4")
                                    {
                                        ValorVenta = pintIdMonedaReporte == -1
                                            ? objReporte.d_ValorVenta
                                            : pintIdMonedaReporte == objReporte.idMoneda ?
                                                     objReporte.d_ValorVenta
                                                    : CalcularTotalesOtraMoneda(ref  objOperationResult, Compras, pintIdMonedaReporte, TipoCalculo);
                                    }
                                }
                                if (TipoCalculo == "OT")
                                {


                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? objReporte.OtroTributos
                                        : pintIdMonedaReporte == objReporte.idMoneda
                                                ? objReporte.OtroTributos
                                                : CalcularTotalesOtraMoneda(ref  objOperationResult, Compras, pintIdMonedaReporte, TipoCalculo);



                                }

                                else if (TipoCalculo == "ISC")
                                {


                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? objReporte.Isc
                                        : pintIdMonedaReporte == objReporte.idMoneda
                                                ? objReporte.Isc
                                                : CalcularTotalesOtraMoneda(ref  objOperationResult, Compras, pintIdMonedaReporte, TipoCalculo);
                                }
                                break;
                                #endregion
                            case 5:
                                #region caso 5:
                                if (_objDocumentoBL.DocumentoEsInverso(objReporte.IdTipoDocumento))
                                {
                                    if (TipoCalculo == "B51")
                                    {


                                        if (pintIdMonedaReporte == objReporte.idMoneda || pintIdMonedaReporte == -1)
                                        {
                                            ValorVenta = Compras.Where(x => x.DestinoGrilla == 1).Sum(x => x.d_ValorVentaDetalle) * -1;
                                        }
                                        else
                                        {
                                            var Detalles = ListaValoresOtraMoneda(ref objOperationResult, Compras, pintIdMonedaReporte);
                                            ValorVenta = Detalles.Where(x => x.DestinoGrillaDetalle == 1).Sum(x => x.ValorVentaOM) * -1;
                                        }

                                    }
                                    else if (TipoCalculo == "B52")
                                    {


                                        if (pintIdMonedaReporte == objReporte.idMoneda || pintIdMonedaReporte == -1)
                                        {
                                            ValorVenta = Compras.Where(x => x.DestinoGrilla == 2).Sum(x => x.d_ValorVentaDetalle) * -1;
                                        }
                                        else
                                        {
                                            var Detalles = ListaValoresOtraMoneda(ref objOperationResult, Compras, pintIdMonedaReporte);
                                            ValorVenta = Detalles.Where(o => o.DestinoGrillaDetalle == 2).Sum(o => o.ValorVentaOM) * -1;

                                        }


                                    }
                                    else if (TipoCalculo == "B53")
                                    {


                                        if (pintIdMonedaReporte == objReporte.idMoneda || pintIdMonedaReporte == -1)
                                        {
                                            ValorVenta = Compras.Where(o => o.DestinoGrilla == 3).Sum(o => o.d_ValorVentaDetalle) * -1;
                                        }
                                        else
                                        {
                                            var Detalles = ListaValoresOtraMoneda(ref objOperationResult, Compras, pintIdMonedaReporte);
                                            ValorVenta = Detalles.Where(o => o.DestinoGrillaDetalle == 3).Sum(o => o.ValorVentaOM) * -1;
                                        }



                                    }
                                    else if (TipoCalculo == "B54")
                                    {


                                        if (pintIdMonedaReporte == objReporte.idMoneda || pintIdMonedaReporte == -1)
                                        {
                                            ValorVenta = Compras.Where(o => o.DestinoGrilla == 4).Sum(o => o.d_ValorVentaDetalle) * -1;
                                        }
                                        else
                                        {
                                            var Detalles = ListaValoresOtraMoneda(ref objOperationResult, Compras, pintIdMonedaReporte);
                                            ValorVenta = Detalles.Where(o => o.DestinoGrillaDetalle == 4).Sum(o => o.ValorVentaOM) * -1;
                                        }



                                    }
                                    else if (TipoCalculo == "I51")
                                    {

                                        if (pintIdMonedaReporte == objReporte.idMoneda || pintIdMonedaReporte == -1)
                                        {
                                            ValorVenta = Compras.Where(o => o.DestinoGrilla == 1).Sum(o => o.d_IgvDetalle) * -1;
                                        }
                                        else
                                        {
                                            var Detalles = ListaValoresOtraMoneda(ref objOperationResult, Compras, pintIdMonedaReporte);
                                            ValorVenta = Detalles.Where(o => o.DestinoGrillaDetalle == 1).Sum(o => o.IgvOM) * -1;
                                        }



                                    }
                                    else if (TipoCalculo == "I52")
                                    {


                                        if (pintIdMonedaReporte == objReporte.idMoneda || pintIdMonedaReporte == -1)
                                        {
                                            ValorVenta = Compras.Where(o => o.DestinoGrilla == 2).Sum(o => o.d_IgvDetalle) * -1;
                                        }
                                        else
                                        {
                                            var Detalles = ListaValoresOtraMoneda(ref objOperationResult, Compras, pintIdMonedaReporte);
                                            ValorVenta = Detalles.Where(o => o.DestinoGrillaDetalle == 2).Sum(o => o.IgvOM) * -1;
                                        }


                                    }
                                    else if (TipoCalculo == "I53")
                                    {


                                        if (pintIdMonedaReporte == objReporte.idMoneda || pintIdMonedaReporte == -1)
                                        {
                                            ValorVenta = Compras.Where(o => o.DestinoGrilla == 3).Sum(o => o.d_IgvDetalle) * -1;
                                        }
                                        else
                                        {
                                            var Detalles = ListaValoresOtraMoneda(ref objOperationResult, Compras, pintIdMonedaReporte);
                                            ValorVenta = Detalles.Where(o => o.DestinoGrillaDetalle == 3).Sum(o => o.IgvOM) * -1;
                                        }


                                    }
                                    else if (TipoCalculo == "I54")
                                    {




                                        if (pintIdMonedaReporte == objReporte.idMoneda || pintIdMonedaReporte == -1)
                                        {
                                            ValorVenta = Compras.Where(o => o.DestinoGrilla == 4).Sum(o => o.d_IgvDetalle) * -1;
                                        }
                                        else
                                        {
                                            var Detalles = ListaValoresOtraMoneda(ref objOperationResult, Compras, pintIdMonedaReporte);
                                            ValorVenta = Detalles.Where(o => o.DestinoGrillaDetalle == 4).Sum(o => o.IgvOM) * -1;
                                        }
                                    }
                                }
                                else // Solo para este caso agregar anticipo , ya que son por TipoDestino
                                {
                                    if (TipoCalculo == "B51")
                                    {

                                        //var Anticipio = Compras.Where(x => x.DestinoGrilla == 1 && x.anticipo == 0).Sum(x => x.d_ValorVentaDetalle)
                                        //              - Compras.Where(x => x.DestinoGrilla == 1 && x.anticipo == 1).Sum(x => x.d_ValorVentaDetalle);
                                        //ValorVenta = pintIdMonedaReporte == -1 ? Anticipio : pintIdMonedaReporte == objReporte.idMoneda ? Anticipio : pintIdMonedaReporte == (int)Currency.Soles ? Anticipio * objReporte.TipoCambio : Anticipio / objReporte.TipoCambio;

                                        if (pintIdMonedaReporte == objReporte.idMoneda || pintIdMonedaReporte == -1)
                                        {
                                            var Anticipio = Compras.Where(x => x.DestinoGrilla == 1 && x.anticipoDetalle == 0).Sum(x => x.d_ValorVentaDetalle)
                                                          - Compras.Where(x => x.DestinoGrilla == 1 && x.anticipoDetalle == 1).Sum(x => x.d_ValorVentaDetalle);
                                            ValorVenta = Anticipio;
                                        }
                                        else
                                        {
                                            var Detalles = ListaValoresOtraMoneda(ref objOperationResult, Compras, pintIdMonedaReporte);
                                            var Anticipio = Detalles.Where(x => x.DestinoGrillaDetalle == 1 && x.Anticipio == 0).Sum(x => x.ValorVentaOM)
                                                             - Detalles.Where(x => x.DestinoGrillaDetalle == 1 && x.Anticipio == 1).Sum(x => x.ValorVentaOM);
                                            ValorVenta = Anticipio;
                                        }

                                    }
                                    else if (TipoCalculo == "B52")
                                    {

                                        if (pintIdMonedaReporte == objReporte.idMoneda || pintIdMonedaReporte == -1)
                                        {
                                            var Anticipio = Compras.Where(x => x.DestinoGrilla == 2 && x.anticipoDetalle == 0).Sum(x => x.d_ValorVentaDetalle)
                                                          - Compras.Where(x => x.DestinoGrilla == 2 && x.anticipoDetalle == 1)
                                                    .Sum(x => x.d_ValorVentaDetalle);

                                            ValorVenta = Anticipio;

                                        }
                                        else
                                        {
                                            var Detalles = ListaValoresOtraMoneda(ref objOperationResult, Compras, pintIdMonedaReporte);
                                            var Anticipio = Detalles.Where(x => x.DestinoGrillaDetalle == 2 && x.Anticipio == 0).Sum(x => x.ValorVentaOM)
                                                    - Detalles.Where(x => x.DestinoGrillaDetalle == 2 && x.Anticipio == 1)
                                              .Sum(x => x.ValorVentaOM);
                                            ValorVenta = Anticipio;
                                        }

                                    }
                                    else if (TipoCalculo == "B53")
                                    {

                                        if (pintIdMonedaReporte == objReporte.idMoneda || pintIdMonedaReporte == -1)
                                        {

                                            var Anticipio = Compras.Where(x => x.DestinoGrilla == 3 && x.anticipoDetalle == 0).Sum(x => x.d_ValorVentaDetalle)
                                                        - Compras.Where(x => x.DestinoGrilla == 3 && x.anticipoDetalle == 1).Sum(x => x.d_ValorVentaDetalle);

                                            ValorVenta = Anticipio;

                                        }
                                        else
                                        {
                                            var Detalles = ListaValoresOtraMoneda(ref objOperationResult, Compras, pintIdMonedaReporte);
                                            var Anticipio = Detalles.Where(x => x.DestinoGrillaDetalle == 3 && x.Anticipio == 0).Sum(x => x.ValorVentaOM)
                                                        - Detalles.Where(x => x.DestinoGrillaDetalle == 3 && x.Anticipio == 1).Sum(x => x.ValorVentaOM);

                                            ValorVenta = Anticipio;

                                        }

                                    }
                                    else if (TipoCalculo == "B54")
                                    {


                                        if (pintIdMonedaReporte == objReporte.idMoneda || pintIdMonedaReporte == -1)
                                        {
                                            var Anticipio = Compras.Where(x => x.DestinoGrilla == 4 && x.anticipoDetalle == 0).Sum(x => x.d_ValorVentaDetalle)
                                                            - Compras.Where(x => x.DestinoGrilla == 4 && x.anticipoDetalle == 1).Sum(x => x.d_ValorVentaDetalle);
                                            ValorVenta = Anticipio;
                                        }
                                        else
                                        {
                                            var Detalles = ListaValoresOtraMoneda(ref objOperationResult, Compras, pintIdMonedaReporte);
                                            var Anticipio = Detalles.Where(x => x.DestinoGrillaDetalle == 4 && x.Anticipio == 0).Sum(x => x.ValorVentaOM)
                                                            - Detalles.Where(x => x.DestinoGrillaDetalle == 4 && x.Anticipio == 1).Sum(x => x.ValorVentaOM);
                                            ValorVenta = Anticipio;

                                        }


                                    }
                                    else if (TipoCalculo == "I51")
                                    {

                                        if (pintIdMonedaReporte == objReporte.idMoneda || pintIdMonedaReporte == -1)
                                        {
                                            var Anticipio = Compras.Where(x => x.DestinoGrilla == 1 && x.anticipoDetalle == 0).Sum(x => x.d_IgvDetalle)
                                                           - Compras.Where(x => x.DestinoGrilla == 1 && x.anticipoDetalle == 1).Sum(x => x.d_IgvDetalle);

                                            ValorVenta = Anticipio;
                                        }
                                        else
                                        {
                                            var Detalles = ListaValoresOtraMoneda(ref objOperationResult, Compras, pintIdMonedaReporte);
                                            var Anticipio = Detalles.Where(x => x.DestinoGrillaDetalle == 1 && x.Anticipio == 0).Sum(x => x.IgvOM)
                                                               - Detalles.Where(x => x.DestinoGrillaDetalle == 1 && x.Anticipio == 1).Sum(x => x.IgvOM);

                                            ValorVenta = Anticipio;



                                        }

                                    }
                                    else if (TipoCalculo == "I52")
                                    {

                                        if (pintIdMonedaReporte == objReporte.idMoneda || pintIdMonedaReporte == -1)
                                        {
                                            var Anticipio = Compras.Where(x => x.DestinoGrilla == 2 && x.anticipoDetalle == 0).Sum(x => x.d_IgvDetalle)
                                                          - Compras.Where(x => x.DestinoGrilla == 2 && x.anticipoDetalle == 1).Sum(x => x.d_IgvDetalle);
                                            ValorVenta = Anticipio;

                                        }
                                        else
                                        {
                                            var Detalles = ListaValoresOtraMoneda(ref objOperationResult, Compras, pintIdMonedaReporte);
                                            var Anticipio = Detalles.Where(x => x.DestinoGrillaDetalle == 2 && x.Anticipio == 0).Sum(x => x.IgvOM)
                                                             - Detalles.Where(x => x.DestinoGrillaDetalle == 2 && x.Anticipio == 1).Sum(x => x.IgvOM);
                                            ValorVenta = Anticipio;

                                        }




                                    }
                                    else if (TipoCalculo == "I53")
                                    {

                                        if (pintIdMonedaReporte == objReporte.idMoneda || pintIdMonedaReporte == -1)
                                        {
                                            var Anticipio = Compras.Where(x => x.DestinoGrilla == 3 && x.anticipoDetalle == 0).Sum(x => x.d_IgvDetalle)
                                                       - Compras.Where(x => x.DestinoGrilla == 3 && x.anticipoDetalle == 1).Sum(x => x.d_IgvDetalle);

                                            ValorVenta = Anticipio;

                                        }
                                        else
                                        {
                                            var Detalles = ListaValoresOtraMoneda(ref objOperationResult, Compras, pintIdMonedaReporte);
                                            var Anticipio = Detalles.Where(x => x.DestinoGrillaDetalle == 3 && x.Anticipio == 0).Sum(x => x.IgvOM)
                                                        - Detalles.Where(x => x.DestinoGrillaDetalle == 3 && x.Anticipio == 1).Sum(x => x.IgvOM);

                                            ValorVenta = Anticipio;

                                        }
                                    }
                                    else if (TipoCalculo == "I54")
                                    {



                                        if (pintIdMonedaReporte == objReporte.idMoneda || pintIdMonedaReporte == -1)
                                        {
                                            var Anticipio = Compras.Where(x => x.DestinoGrilla == 4 && x.anticipoDetalle == 0).Sum(x => x.d_IgvDetalle)
                                                   - Compras.Where(x => x.DestinoGrilla == 4 && x.anticipoDetalle == 1).Sum(x => x.d_IgvDetalle);
                                            ValorVenta = Anticipio;
                                        }
                                        else
                                        {
                                            var Detalles = ListaValoresOtraMoneda(ref objOperationResult, Compras, pintIdMonedaReporte);
                                            var Anticipio = Detalles.Where(x => x.DestinoGrillaDetalle == 4 && x.Anticipio == 0).Sum(x => x.IgvOM)
                                                                                               - Detalles.Where(x => x.DestinoGrillaDetalle == 4 && x.Anticipio == 1).Sum(x => x.IgvOM);
                                            ValorVenta = Anticipio;


                                        }
                                    }
                                }
                                if (TipoCalculo == "OT")
                                {
                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? objReporte.OtroTributos
                                        : pintIdMonedaReporte == objReporte.idMoneda
                                                ? objReporte.OtroTributos
                                                : CalcularTotalesOtraMoneda(ref  objOperationResult, Compras, pintIdMonedaReporte, TipoCalculo);
                                }

                                else if (TipoCalculo == "ISC")
                                {
                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? objReporte.Isc
                                        : pintIdMonedaReporte == objReporte.idMoneda
                                                ? objReporte.Isc
                                                : CalcularTotalesOtraMoneda(ref  objOperationResult, Compras, pintIdMonedaReporte, TipoCalculo);
                                }
                                break;
                                #endregion
                            case 999:


                                #region caso 5:
                                if (_objDocumentoBL.DocumentoEsInverso(objReporte.IdTipoDocumento))
                                {
                                    if (TipoCalculo == "B51")
                                    {


                                        if (pintIdMonedaReporte == objReporte.idMoneda || pintIdMonedaReporte == -1)
                                        {
                                            ValorVenta = Compras.Where(x => x.DestinoGrilla == 1).Sum(x => x.d_ValorVentaDetalle) * -1;
                                        }
                                        else
                                        {
                                            var Detalles = ListaValoresOtraMoneda(ref objOperationResult, Compras, pintIdMonedaReporte);
                                            ValorVenta = Detalles.Where(x => x.DestinoGrillaDetalle == 1).Sum(x => x.ValorVentaOM) * -1;
                                        }

                                    }
                                    else if (TipoCalculo == "B52")
                                    {


                                        if (pintIdMonedaReporte == objReporte.idMoneda || pintIdMonedaReporte == -1)
                                        {
                                            ValorVenta = Compras.Where(x => x.DestinoGrilla == 2).Sum(x => x.d_ValorVentaDetalle) * -1;
                                        }
                                        else
                                        {
                                            var Detalles = ListaValoresOtraMoneda(ref objOperationResult, Compras, pintIdMonedaReporte);
                                            ValorVenta = Detalles.Where(o => o.DestinoGrillaDetalle == 2).Sum(o => o.ValorVentaOM) * -1;

                                        }


                                    }
                                    else if (TipoCalculo == "B53")
                                    {


                                        if (pintIdMonedaReporte == objReporte.idMoneda || pintIdMonedaReporte == -1)
                                        {
                                            ValorVenta = Compras.Where(o => o.DestinoGrilla == 3).Sum(o => o.d_ValorVentaDetalle) * -1;
                                        }
                                        else
                                        {
                                            var Detalles = ListaValoresOtraMoneda(ref objOperationResult, Compras, pintIdMonedaReporte);
                                            ValorVenta = Detalles.Where(o => o.DestinoGrillaDetalle == 3).Sum(o => o.ValorVentaOM) * -1;
                                        }



                                    }
                                    else if (TipoCalculo == "B54")
                                    {


                                        if (pintIdMonedaReporte == objReporte.idMoneda || pintIdMonedaReporte == -1)
                                        {
                                            ValorVenta = Compras.Where(o => o.DestinoGrilla == 4).Sum(o => o.d_ValorVentaDetalle) * -1;
                                        }
                                        else
                                        {
                                            var Detalles = ListaValoresOtraMoneda(ref objOperationResult, Compras, pintIdMonedaReporte);
                                            ValorVenta = Detalles.Where(o => o.DestinoGrillaDetalle == 4).Sum(o => o.ValorVentaOM) * -1;
                                        }



                                    }
                                    else if (TipoCalculo == "I51")
                                    {

                                        if (pintIdMonedaReporte == objReporte.idMoneda || pintIdMonedaReporte == -1)
                                        {
                                            ValorVenta = Compras.Where(o => o.DestinoGrilla == 1).Sum(o => o.d_IgvDetalle) * -1;
                                        }
                                        else
                                        {
                                            var Detalles = ListaValoresOtraMoneda(ref objOperationResult, Compras, pintIdMonedaReporte);
                                            ValorVenta = Detalles.Where(o => o.DestinoGrillaDetalle == 1).Sum(o => o.IgvOM) * -1;
                                        }



                                    }
                                    else if (TipoCalculo == "I52")
                                    {


                                        if (pintIdMonedaReporte == objReporte.idMoneda || pintIdMonedaReporte == -1)
                                        {
                                            ValorVenta = Compras.Where(o => o.DestinoGrilla == 2).Sum(o => o.d_IgvDetalle) * -1;
                                        }
                                        else
                                        {
                                            var Detalles = ListaValoresOtraMoneda(ref objOperationResult, Compras, pintIdMonedaReporte);
                                            ValorVenta = Detalles.Where(o => o.DestinoGrillaDetalle == 2).Sum(o => o.IgvOM) * -1;
                                        }


                                    }
                                    else if (TipoCalculo == "I53")
                                    {


                                        if (pintIdMonedaReporte == objReporte.idMoneda || pintIdMonedaReporte == -1)
                                        {
                                            ValorVenta = Compras.Where(o => o.DestinoGrilla == 3).Sum(o => o.d_IgvDetalle) * -1;
                                        }
                                        else
                                        {
                                            var Detalles = ListaValoresOtraMoneda(ref objOperationResult, Compras, pintIdMonedaReporte);
                                            ValorVenta = Detalles.Where(o => o.DestinoGrillaDetalle == 3).Sum(o => o.IgvOM) * -1;
                                        }


                                    }
                                    else if (TipoCalculo == "I54")
                                    {




                                        if (pintIdMonedaReporte == objReporte.idMoneda || pintIdMonedaReporte == -1)
                                        {
                                            ValorVenta = Compras.Where(o => o.DestinoGrilla == 4).Sum(o => o.d_IgvDetalle) * -1;
                                        }
                                        else
                                        {
                                            var Detalles = ListaValoresOtraMoneda(ref objOperationResult, Compras, pintIdMonedaReporte);
                                            ValorVenta = Detalles.Where(o => o.DestinoGrillaDetalle == 4).Sum(o => o.IgvOM) * -1;
                                        }
                                    }
                                }
                                else // Solo para este caso agregar anticipo , ya que son por TipoDestino
                                {
                                    if (TipoCalculo == "B51")
                                    {

                                        //var Anticipio = Compras.Where(x => x.DestinoGrilla == 1 && x.anticipo == 0).Sum(x => x.d_ValorVentaDetalle)
                                        //              - Compras.Where(x => x.DestinoGrilla == 1 && x.anticipo == 1).Sum(x => x.d_ValorVentaDetalle);
                                        //ValorVenta = pintIdMonedaReporte == -1 ? Anticipio : pintIdMonedaReporte == objReporte.idMoneda ? Anticipio : pintIdMonedaReporte == (int)Currency.Soles ? Anticipio * objReporte.TipoCambio : Anticipio / objReporte.TipoCambio;

                                        if (pintIdMonedaReporte == objReporte.idMoneda || pintIdMonedaReporte == -1)
                                        {
                                            var Anticipio = Compras.Where(x => x.DestinoGrilla == 1 && x.anticipoDetalle == 0).Sum(x => x.d_ValorVentaDetalle)
                                                          - Compras.Where(x => x.DestinoGrilla == 1 && x.anticipoDetalle == 1).Sum(x => x.d_ValorVentaDetalle);
                                            ValorVenta = Anticipio;
                                        }
                                        else
                                        {
                                            var Detalles = ListaValoresOtraMoneda(ref objOperationResult, Compras, pintIdMonedaReporte);
                                            var Anticipio = Detalles.Where(x => x.DestinoGrillaDetalle == 1 && x.Anticipio == 0).Sum(x => x.ValorVentaOM)
                                                             - Detalles.Where(x => x.DestinoGrillaDetalle == 1 && x.Anticipio == 1).Sum(x => x.ValorVentaOM);
                                            ValorVenta = Anticipio;
                                        }

                                    }
                                    else if (TipoCalculo == "B52")
                                    {

                                        if (pintIdMonedaReporte == objReporte.idMoneda || pintIdMonedaReporte == -1)
                                        {
                                            var Anticipio = Compras.Where(x => x.DestinoGrilla == 2 && x.anticipoDetalle == 0).Sum(x => x.d_ValorVentaDetalle)
                                                          - Compras.Where(x => x.DestinoGrilla == 2 && x.anticipoDetalle == 1)
                                                    .Sum(x => x.d_ValorVentaDetalle);

                                            ValorVenta = Anticipio;

                                        }
                                        else
                                        {
                                            var Detalles = ListaValoresOtraMoneda(ref objOperationResult, Compras, pintIdMonedaReporte);
                                            var Anticipio = Detalles.Where(x => x.DestinoGrillaDetalle == 2 && x.Anticipio == 0).Sum(x => x.ValorVentaOM)
                                                    - Detalles.Where(x => x.DestinoGrillaDetalle == 2 && x.Anticipio == 1)
                                              .Sum(x => x.ValorVentaOM);
                                            ValorVenta = Anticipio;
                                        }

                                    }
                                    else if (TipoCalculo == "B53")
                                    {

                                        if (pintIdMonedaReporte == objReporte.idMoneda || pintIdMonedaReporte == -1)
                                        {

                                            var Anticipio = Compras.Where(x => x.DestinoGrilla == 3 && x.anticipoDetalle == 0).Sum(x => x.d_ValorVentaDetalle)
                                                        - Compras.Where(x => x.DestinoGrilla == 3 && x.anticipoDetalle == 1).Sum(x => x.d_ValorVentaDetalle);

                                            ValorVenta = Anticipio;

                                        }
                                        else
                                        {
                                            var Detalles = ListaValoresOtraMoneda(ref objOperationResult, Compras, pintIdMonedaReporte);
                                            var Anticipio = Detalles.Where(x => x.DestinoGrillaDetalle == 3 && x.Anticipio == 0).Sum(x => x.ValorVentaOM)
                                                        - Detalles.Where(x => x.DestinoGrillaDetalle == 3 && x.Anticipio == 1).Sum(x => x.ValorVentaOM);

                                            ValorVenta = Anticipio;

                                        }

                                    }
                                    else if (TipoCalculo == "B54")
                                    {


                                        if (pintIdMonedaReporte == objReporte.idMoneda || pintIdMonedaReporte == -1)
                                        {
                                            var Anticipio = Compras.Where(x => x.DestinoGrilla == 4 && x.anticipoDetalle == 0).Sum(x => x.d_ValorVentaDetalle)
                                                            - Compras.Where(x => x.DestinoGrilla == 4 && x.anticipoDetalle == 1).Sum(x => x.d_ValorVentaDetalle);
                                            ValorVenta = Anticipio;
                                        }
                                        else
                                        {
                                            var Detalles = ListaValoresOtraMoneda(ref objOperationResult, Compras, pintIdMonedaReporte);
                                            var Anticipio = Detalles.Where(x => x.DestinoGrillaDetalle == 4 && x.Anticipio == 0).Sum(x => x.ValorVentaOM)
                                                            - Detalles.Where(x => x.DestinoGrillaDetalle == 4 && x.Anticipio == 1).Sum(x => x.ValorVentaOM);
                                            ValorVenta = Anticipio;

                                        }


                                    }
                                    else if (TipoCalculo == "I51")
                                    {

                                        if (pintIdMonedaReporte == objReporte.idMoneda || pintIdMonedaReporte == -1)
                                        {
                                            var Anticipio = Compras.Where(x => x.DestinoGrilla == 1 && x.anticipoDetalle == 0).Sum(x => x.d_IgvDetalle)
                                                           - Compras.Where(x => x.DestinoGrilla == 1 && x.anticipoDetalle == 1).Sum(x => x.d_IgvDetalle);

                                            ValorVenta = Anticipio;
                                        }
                                        else
                                        {
                                            var Detalles = ListaValoresOtraMoneda(ref objOperationResult, Compras, pintIdMonedaReporte);
                                            var Anticipio = Detalles.Where(x => x.DestinoGrillaDetalle == 1 && x.Anticipio == 0).Sum(x => x.IgvOM)
                                                               - Detalles.Where(x => x.DestinoGrillaDetalle == 1 && x.Anticipio == 1).Sum(x => x.IgvOM);

                                            ValorVenta = Anticipio;



                                        }

                                    }
                                    else if (TipoCalculo == "I52")
                                    {

                                        if (pintIdMonedaReporte == objReporte.idMoneda || pintIdMonedaReporte == -1)
                                        {
                                            var Anticipio = Compras.Where(x => x.DestinoGrilla == 2 && x.anticipoDetalle == 0).Sum(x => x.d_IgvDetalle)
                                                          - Compras.Where(x => x.DestinoGrilla == 2 && x.anticipoDetalle == 1).Sum(x => x.d_IgvDetalle);
                                            ValorVenta = Anticipio;

                                        }
                                        else
                                        {
                                            var Detalles = ListaValoresOtraMoneda(ref objOperationResult, Compras, pintIdMonedaReporte);
                                            var Anticipio = Detalles.Where(x => x.DestinoGrillaDetalle == 2 && x.Anticipio == 0).Sum(x => x.IgvOM)
                                                             - Detalles.Where(x => x.DestinoGrillaDetalle == 2 && x.Anticipio == 1).Sum(x => x.IgvOM);
                                            ValorVenta = Anticipio;

                                        }




                                    }
                                    else if (TipoCalculo == "I53")
                                    {

                                        if (pintIdMonedaReporte == objReporte.idMoneda || pintIdMonedaReporte == -1)
                                        {
                                            var Anticipio = Compras.Where(x => x.DestinoGrilla == 3 && x.anticipoDetalle == 0).Sum(x => x.d_IgvDetalle)
                                                       - Compras.Where(x => x.DestinoGrilla == 3 && x.anticipoDetalle == 1).Sum(x => x.d_IgvDetalle);

                                            ValorVenta = Anticipio;

                                        }
                                        else
                                        {
                                            var Detalles = ListaValoresOtraMoneda(ref objOperationResult, Compras, pintIdMonedaReporte);
                                            var Anticipio = Detalles.Where(x => x.DestinoGrillaDetalle == 3 && x.Anticipio == 0).Sum(x => x.IgvOM)
                                                        - Detalles.Where(x => x.DestinoGrillaDetalle == 3 && x.Anticipio == 1).Sum(x => x.IgvOM);

                                            ValorVenta = Anticipio;

                                        }
                                    }
                                    else if (TipoCalculo == "I54")
                                    {



                                        if (pintIdMonedaReporte == objReporte.idMoneda || pintIdMonedaReporte == -1)
                                        {
                                            var Anticipio = Compras.Where(x => x.DestinoGrilla == 4 && x.anticipoDetalle == 0).Sum(x => x.d_IgvDetalle)
                                                   - Compras.Where(x => x.DestinoGrilla == 4 && x.anticipoDetalle == 1).Sum(x => x.d_IgvDetalle);
                                            ValorVenta = Anticipio;
                                        }
                                        else
                                        {
                                            var Detalles = ListaValoresOtraMoneda(ref objOperationResult, Compras, pintIdMonedaReporte);
                                            var Anticipio = Detalles.Where(x => x.DestinoGrillaDetalle == 4 && x.Anticipio == 0).Sum(x => x.IgvOM)
                                                                                               - Detalles.Where(x => x.DestinoGrillaDetalle == 4 && x.Anticipio == 1).Sum(x => x.IgvOM);
                                            ValorVenta = Anticipio;


                                        }
                                    }
                                }
                                if (TipoCalculo == "OT")
                                {
                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? objReporte.OtroTributos
                                        : pintIdMonedaReporte == objReporte.idMoneda
                                                ? objReporte.OtroTributos
                                                : CalcularTotalesOtraMoneda(ref  objOperationResult, Compras, pintIdMonedaReporte, TipoCalculo);
                                }

                                else if (TipoCalculo == "ISC")
                                {
                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? objReporte.Isc
                                        : pintIdMonedaReporte == objReporte.idMoneda
                                                ? objReporte.Isc
                                                : CalcularTotalesOtraMoneda(ref  objOperationResult, Compras, pintIdMonedaReporte, TipoCalculo);
                                }
                                break;
                                #endregion

                        }
                    }
                    else
                    {
                        if (TipoCalculo == "B1" && Fila == "1")
                        {
                            var IgvPorcentaje = decimal.Parse(objReporte.porcentajeIgv) / 100;
                            ValorVenta = pintIdMonedaReporte == -1
                                ? objReporte.d_Igv / IgvPorcentaje
                                : pintIdMonedaReporte == (int)Currency.Soles
                                    ? objReporte.idMoneda == (int)Currency.Soles
                                        ? objReporte.d_Igv / IgvPorcentaje
                                        : objReporte.d_Igv / IgvPorcentaje * objReporte.TipoCambio
                                    : objReporte.idMoneda == (int)Currency.Dolares
                                        ? objReporte.d_Igv / IgvPorcentaje
                                        : objReporte.d_Igv / IgvPorcentaje / objReporte.TipoCambio;
                        }

                        if (TipoCalculo == "I1" && Fila == "1")
                        {
                            var IgvPorcentaje = decimal.Parse(objReporte.porcentajeIgv);
                            ValorVenta = pintIdMonedaReporte == -1
                                ? objReporte.d_Igv
                                : pintIdMonedaReporte == (int)Currency.Soles
                                    ? objReporte.idMoneda == (int)Currency.Soles
                                        ? objReporte.d_Igv
                                        : objReporte.d_Igv * objReporte.TipoCambio
                                    : objReporte.idMoneda == (int)Currency.Dolares
                                        ? objReporte.d_Igv
                                        : objReporte.d_Igv / objReporte.TipoCambio;
                        }
                        if (TipoCalculo == "A" && Fila == "4")
                        {
                            var queryExistencia =
                                Importaciones.Where(
                                    x =>
                                        x.CompraGeneradaImportacion != "" &&
                                        x.CompraGeneradaImportacion == objReporte.CompraGeneradaImportacion).ToList();
                            if (queryExistencia.Count() > 0)
                            {
                                var CompraGenerada =
                                    Compras.Where(
                                        x =>
                                            x.CompraGeneradaImportacion ==
                                            queryExistencia.FirstOrDefault().CompraGeneradaImportacion);
                                if (queryExistencia.FirstOrDefault().TipoDocGenerado == objReporte.IdTipoDocumento &&
                                    queryExistencia.FirstOrDefault().serieDocumentoGenerado == objReporte.SerieDocumento &&
                                    queryExistencia.FirstOrDefault().correlativoDocumentoGenerado ==
                                    objReporte.CorrelativoDocumento && CompraGenerada.FirstOrDefault().DestinoCabecera == 4)
                                {
                                    ValorVenta = pintIdMonedaReporte == -1
                                        ? queryExistencia.FirstOrDefault().TotalSolesGenerado
                                        : pintIdMonedaReporte == 1
                                            ? queryExistencia.FirstOrDefault().TotalSolesGenerado
                                            : queryExistencia.FirstOrDefault().TotalDolaresGenerado;
                                }
                                else
                                {
                                    ValorVenta = 0;
                                }
                            }
                            else
                            {
                                ValorVenta = 0;
                            }
                        }
                    }
                }

                return Utils.Windows.DevuelveValorRedondeado((decimal)ValorVenta, 2);
            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                return 0;

            }
        }

        private Calculos CalcularValoresFilaOtraMoneda(ref OperationResult objOperationResult, int MonedaReporte, bool AfectoIgv, bool PrecioIncluyeIgv, decimal TipoCambio, ReporteRegistroCompraSunat DetalleCompra)
        {
            try
            {

                if (DetalleCompra.Correlativo == "0101000121")
                {
                    string h = "";
                }
                objOperationResult.Success = 1;
                Calculos objCalculados = new Calculos();
                decimal d_ValorVentaOM = 0;
                decimal d_IgvOM = 0;
                decimal d_PrecioVentaOM = 0;
                decimal d_OtrosTributosOM = MonedaReporte == (int)Currency.Soles ? Utils.Windows.DevuelveValorRedondeado(DetalleCompra.OtroTributos.Value * TipoCambio, 2) : Utils.Windows.DevuelveValorRedondeado(DetalleCompra.OtroTributos.Value / TipoCambio, 2);
                decimal d_IscOM = MonedaReporte == (int)Currency.Soles ? Utils.Windows.DevuelveValorRedondeado(DetalleCompra.Isc.Value * TipoCambio, 2) : Utils.Windows.DevuelveValorRedondeado(DetalleCompra.Isc.Value / TipoCambio, 2);

                if (!AfectoIgv)
                {
                    d_ValorVentaOM = MonedaReporte == (int)Currency.Soles ? Utils.Windows.DevuelveValorRedondeado(DetalleCompra.d_ValorVentaDetalle.Value * TipoCambio, 2) : Utils.Windows.DevuelveValorRedondeado(DetalleCompra.d_ValorVenta.Value / TipoCambio, 2);
                    d_IgvOM = 0;
                    d_PrecioVentaOM = MonedaReporte == (int)Currency.Soles ? Utils.Windows.DevuelveValorRedondeado(DetalleCompra.d_PrecioVentaDetalle.Value * TipoCambio, 2) : Utils.Windows.DevuelveValorRedondeado(DetalleCompra.d_PrecioVentaDetalle.Value / TipoCambio, 2);

                }
                else if (AfectoIgv && !PrecioIncluyeIgv)
                {
                    if (MonedaReporte == (int)Currency.Soles)
                    {
                        d_ValorVentaOM = Utils.Windows.DevuelveValorRedondeado(DetalleCompra.d_ValorVentaDetalle.Value * TipoCambio, 2);
                        d_IgvOM = Utils.Windows.DevuelveValorRedondeado(DetalleCompra.d_IgvDetalle.Value * TipoCambio, 2);
                        d_PrecioVentaOM = d_ValorVentaOM + d_IgvOM;
                    }
                    else
                    {
                        d_ValorVentaOM = Utils.Windows.DevuelveValorRedondeado(DetalleCompra.d_ValorVentaDetalle.Value / TipoCambio, 2);
                        d_IgvOM = Utils.Windows.DevuelveValorRedondeado(DetalleCompra.d_IgvDetalle.Value / TipoCambio, 2);
                        d_PrecioVentaOM = d_ValorVentaOM + d_IgvOM;
                    }


                }
                else if (AfectoIgv && PrecioIncluyeIgv)
                {
                    d_ValorVentaOM = MonedaReporte == (int)Currency.Soles ? Utils.Windows.DevuelveValorRedondeado(DetalleCompra.d_ValorVentaDetalle.Value * TipoCambio, 2) : Utils.Windows.DevuelveValorRedondeado(DetalleCompra.d_ValorVentaDetalle.Value / TipoCambio, 2);
                    d_PrecioVentaOM = MonedaReporte == (int)Currency.Soles ? Utils.Windows.DevuelveValorRedondeado(DetalleCompra.d_PrecioVentaDetalle.Value * TipoCambio, 2) : Utils.Windows.DevuelveValorRedondeado(DetalleCompra.d_PrecioVentaDetalle.Value / TipoCambio, 2);
                    d_IgvOM = d_PrecioVentaOM - d_ValorVentaOM;
                }


                objCalculados.Anticipio = DetalleCompra.anticipoDetalle;
                objCalculados.ValorVentaOM = d_ValorVentaOM;
                objCalculados.PrecioVentaOM = d_PrecioVentaOM;
                objCalculados.IscOM = d_IscOM;
                objCalculados.OtrosTributosOM = d_OtrosTributosOM;
                objCalculados.IgvOM = d_IgvOM;
                objCalculados.DestinoGrillaDetalle = DetalleCompra.DestinoGrilla.Value;
                return objCalculados;


            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;
            }
        }



        private decimal CalcularTotalesOtraMoneda(ref  OperationResult objOperationResult, List<ReporteRegistroCompraSunat> ListaDetalles, int MonedaReporte, string TipoCalculo)
        {
            try
            {
                List<Calculos> ListaCalculos = new List<Calculos>();
                Calculos objCalculos = new Calculos();
                objOperationResult.Success = 1;
                foreach (var item in ListaDetalles)
                {
                    objCalculos = new Calculos();
                    objCalculos = CalcularValoresFilaOtraMoneda(ref objOperationResult, MonedaReporte, item.AfectoIgv, item.PrecioIncluyeIgv, item.TipoCambio.Value, item);
                    if (objOperationResult.Success == 0)
                    {
                        return 0;
                    }
                    ListaCalculos.Add(objCalculos);
                }
                if (ListaCalculos.Any())
                {
                    decimal SumAntValVenta = 0, SumAntIgv = 0, SumAntTotal = 0, SumAntISC = 0, SumAntOT = 0;
                    decimal SumValVenta = 0, SumIgv = 0, SumTotal = 0, SumISC = 0, SumOT = 0;

                    foreach (Calculos Fila in ListaCalculos)
                    {

                        switch (Fila.Anticipio)
                        {
                            case 1:
                                SumAntValVenta = SumAntValVenta + Fila.ValorVentaOM;//  Fila.VA decimal.Parse(Fila.Cells["d_ValorVenta"].Value.ToString());
                                SumAntIgv = SumAntIgv + Fila.IgvOM;//  decimal.Parse(Fila.Cells["d_Igv"].Value.ToString());
                                SumAntTotal = SumAntTotal + Fila.PrecioVentaOM;//  decimal.Parse(Fila.Cells["d_PrecioVenta"].Value.ToString());
                                SumAntISC = SumAntISC + Fila.IscOM;//  decimal.Parse(Fila.Cells["d_isc"].Value.ToString());
                                SumAntOT = SumAntOT + Fila.OtrosTributosOM;//  decimal.Parse(Fila.Cells["d_otrostributos"].Value.ToString());
                                break;
                            case 0:
                                SumValVenta = SumValVenta + Fila.ValorVentaOM;   // decimal.Parse(Fila.Cells["d_ValorVenta"].Value.ToString());
                                SumIgv = SumIgv + Fila.IgvOM;   // decimal.Parse(Fila.Cells["d_Igv"].Value.ToString());
                                SumTotal = SumTotal + Fila.PrecioVentaOM;//  // decimal.Parse(Fila.Cells["d_PrecioVenta"].Value.ToString());
                                SumISC = SumISC + Fila.IscOM;// // decimal.Parse(Fila.Cells["d_isc"].Value.ToString());
                                SumOT = SumOT + Fila.OtrosTributosOM;//    decimal.Parse(Fila.Cells["d_otrostributos"].Value.ToString());
                                break;
                        }


                    }

                    if (TipoCalculo.StartsWith("ISC"))
                    {
                        return Utils.Windows.DevuelveValorRedondeado(SumISC - SumAntISC, 2);
                    }
                    else if (TipoCalculo.StartsWith("B"))
                    {
                        return Utils.Windows.DevuelveValorRedondeado(SumValVenta - SumAntValVenta, 2);
                    }
                    else if (TipoCalculo.StartsWith("T"))
                    {

                        return Utils.Windows.DevuelveValorRedondeado(SumTotal - SumAntTotal, 2);
                    }
                    else if (TipoCalculo.StartsWith("OT"))
                    {
                        return Utils.Windows.DevuelveValorRedondeado(SumOT - SumAntOT, 2);
                    }
                    else if (TipoCalculo.StartsWith("I"))
                    {
                        return Utils.Windows.DevuelveValorRedondeado(SumIgv - SumAntIgv, 2);
                    }

                    return 0;
                }

                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return 0;

            }

        }



        private List<Calculos> ListaValoresOtraMoneda(ref  OperationResult objOperationResult, List<ReporteRegistroCompraSunat> ListaDetalles, int MonedaReporte)
        {
            List<Calculos> ListaCalculos = new List<Calculos>();
            Calculos objCalculos = new Calculos();
            objOperationResult.Success = 1;
            foreach (var item in ListaDetalles)
            {
                objCalculos = new Calculos();
                objCalculos = CalcularValoresFilaOtraMoneda(ref objOperationResult, MonedaReporte, item.AfectoIgv, item.PrecioIncluyeIgv, item.TipoCambio.Value, item);
                if (objOperationResult.Success == 0)
                {
                    return null;
                }
                ListaCalculos.Add(objCalculos);
            }
            return ListaCalculos;


        }





        public List<ReporteRegistroCompraSunat> ReportePDBCompras(ref OperationResult objOperationResult,
         int pstri_IdEstablecimiento, DateTime? pstrt_FechaRegistroIni, DateTime? pstrt_FechaRegistroFin,
         int pintIdMoneda, string pstrt_Orden, int pintIdTipoCompra, string pstrt_NroCuenta, int pintIdTipoDocumento,
         string pstr_grupollave = null, string pstr_Nombregrupollave = null, int? ConsideraDocInternos = null,
         int? LibroElectronico = null)
        {
            try
            {
                var _clienteDto = new clienteDto();
                var _objClienteBL = new ClienteBL();
                var dbContext = new SAMBHSEntitiesModelWin();
                var query4 = new List<ReporteRegistroCompraSunat>();
                var ListaFinal = new List<ReporteRegistroCompraSunat>();
                var objReporte = new ReporteRegistroCompraSunat();
                var queryComprasx = new List<ReporteRegistroCompraSunat>();
                var ListaReporteFinalizado = new List<ReporteRegistroCompraSunat>();
                var ListaFinal1 = new List<ReporteRegistroCompraSunat>();

                #region Query

                //1. query para obtener toda la data filtrada por los parametros

                var queryCompras =
                    (from A in dbContext.compra
                     join B in dbContext.cliente on new { IdProveedor = A.v_IdProveedor, Flag = "V", eliminado = 0 }
                         equals
                         new { IdProveedor = B.v_IdCliente, Flag = B.v_FlagPantalla, eliminado = B.i_Eliminado.Value }
                         into B_join
                     from B in B_join.DefaultIfEmpty()
                     join F in dbContext.datahierarchy on
                         new { IdMoneda = A.i_IdMoneda.Value, Grupo = 18, eliminado = 0 } equals
                         new { IdMoneda = F.i_ItemId, Grupo = F.i_GroupId, eliminado = F.i_IsDeleted.Value } into
                         F_join
                     from F in F_join.DefaultIfEmpty()
                     join I in dbContext.compradetalle on new { idCompra = A.v_IdCompra, eliminado = 0 } equals
                         new { idCompra = I.v_IdCompra, eliminado = I.i_Eliminado.Value } into I_join
                     from I in I_join.DefaultIfEmpty()
                     join J in dbContext.productodetalle on
                         new { IdProductoDetalle = I.v_IdProductoDetalle, eliminado = 0 } equals
                         new { IdProductoDetalle = J.v_IdProductoDetalle, eliminado = J.i_Eliminado.Value } into J_join
                     from J in J_join.DefaultIfEmpty()
                     join K in dbContext.producto on new { idProducto = J.v_IdProducto, eliminado = 0 } equals
                         new { idProducto = K.v_IdProducto, eliminado = K.i_Eliminado.Value } into K_join
                     from K in K_join.DefaultIfEmpty()
                     join G in dbContext.datahierarchy on new { Igv = A.i_IdIgv.Value, eliminado = 0, Grupo = 27 }
                         equals new { Igv = G.i_ItemId, eliminado = G.i_IsDeleted.Value, Grupo = G.i_GroupId } into
                         G_join
                     from G in G_join.DefaultIfEmpty()
                     join H in dbContext.documento on new { TipoDoc = A.i_IdTipoDocumento.Value, eliminado = 0 } equals
                         new { TipoDoc = H.i_CodigoDocumento, eliminado = H.i_Eliminado.Value } into H_join
                     from H in H_join.DefaultIfEmpty()
                     join L in dbContext.asientocontable on new { eliminado = 0, c = I.v_NroCuenta, p = periodo } equals
                         new { eliminado = L.i_Eliminado.Value, c = L.v_NroCuenta, p = L.v_Periodo } into L_join
                     from L in L_join.DefaultIfEmpty()
                     join M in dbContext.datahierarchy on
                         new { eliminado = 0, Grupo = 146, Tribut = B.i_IdConvenioDobleTributacion.Value } equals
                         new { eliminado = M.i_IsDeleted.Value, Grupo = M.i_GroupId, Tribut = M.i_ItemId } into M_join
                     from M in M_join.DefaultIfEmpty()
                     join N in dbContext.datahierarchy on
                         new { Grupo = 29, eliminado = 0, codigo = A.i_CodigoDetraccion.Value } equals
                         new { Grupo = N.i_GroupId, eliminado = N.i_IsDeleted.Value, codigo = N.i_ItemId } into N_join
                     from N in N_join.DefaultIfEmpty()
                     where
                         (A.i_Eliminado == 0) && A.t_FechaRegistro >= pstrt_FechaRegistroIni &&
                         A.t_FechaRegistro <= pstrt_FechaRegistroFin &&
                         (I.v_NroCuenta == pstrt_NroCuenta || pstrt_NroCuenta == "") &&
                         (A.i_IdTipoDocumento == pintIdTipoDocumento || pintIdTipoDocumento == -1) &&
                         (A.i_IdTipoCompra == pintIdTipoCompra || pintIdTipoCompra == -1) &&
                         (I.v_NroCuenta.Trim() == pstrt_NroCuenta.Trim() || pstrt_NroCuenta == "")
                         && A.i_IdEstado == 1
                     //orderby pstrt_Orden
                     select new ReporteRegistroCompraSunat
                     {
                         Correlativo = A.v_Mes.Trim() + A.v_Correlativo.Trim(),
                         //FechaRegistro = A.t_FechaEmision.Value,
                         FechaVencimiento = A.t_FechaVencimiento.Value,
                         IdTipoDocumento = A.i_IdTipoDocumento.Value,
                         TipoDocumento = "",
                         SerieDocumento = A.v_SerieDocumento,
                         aniovencimiento = "",
                         CorrelativoDocumento = A.v_CorrelativoDocumento.Trim(),
                         NombreProveedor =
                             A.i_IdEstado == 1
                                 ? (B.v_PrimerNombre + " " + B.v_ApePaterno + " " + B.v_ApeMaterno + " " +
                                    B.v_RazonSocial).Trim()
                                 : "**** ANULADO ****",
                         DocIdentidad = A.i_IdEstado == 1 ? B.i_IdTipoIdentificacion.Value : 0,
                         NroDocProveedor = A.i_IdEstado == 1 ? B.v_NroDocIdentificacion : "0",
                         TipoCambio = A.i_IdEstado == 1 ? A.d_TipoCambio.Value : 0,
                         LlaveOrdenar = "",
                         FechaRegistroRef = A.t_FechaRef.Value,
                         IdTipoDocumentoRef = A.i_IdTipoDocumentoRef.Value,
                         TipoDocumentoRef = "",
                         SerieDocumentoRef = A.v_SerieDocumentoRef,
                         CorrelativoDocumentoRef = A.v_CorrelativoDocumentoRef,
                         NroCPagoNoDomiciliario =
                             A.i_IdTipoDocumento == 91 ? A.v_SerieDocumento + "-" + A.v_CorrelativoDocumento : "",

                         FechaEmision = A.t_FechaEmision.Value,
                         IgvNombre = G == null ? "" : G.v_Value1,
                         TipoCompra = "COMPRAS NACIONALES",
                         CompraGeneradaImportacion =
                             A.v_IdDocumentoReferencia == null ? "" : A.v_IdDocumentoReferencia,
                         NumeroRegistro = "C " + A.v_Mes + A.v_Correlativo + A.v_Periodo,
                         AnioAduanero = A.t_FechaEmision.Value,
                         Estado = A.i_IdEstado.Value,
                         DestinoCabecera = A.i_IdDestino == null ? 0 : A.i_IdDestino.Value,
                         DestinoGrilla = I == null ? 0 : I.i_IdDestino.Value,
                         idMoneda = A.i_IdMoneda.Value,
                         d_ValorVenta = A.d_ValorVenta == null ? 0 : A.d_ValorVenta.Value,
                         idCabecera = A.v_IdCompra == null ? "" : A.v_IdCompra,
                         anticipoDetalle = I == null ? 0 : I.i_Anticipio == null ? 0 : I.i_Anticipio.Value,
                         d_Igv = A.d_IGV == null ? 0 : A.d_IGV.Value,
                         TipoDocGenerado = A.i_IdTipoDocumento.Value,
                         serieDocumentoGenerado = A.v_SerieDocumento,
                         correlativoDocumentoGenerado = A.v_CorrelativoDocumento,
                         porcentajeIgv = "0",
                         d_ValorVentaDetalle = I == null ? 0 : I.d_ValorVenta == null ? 0 : I.d_ValorVenta.Value,
                         d_IgvDetalle = I == null ? 0 : I.d_Igv.Value,
                         Isc = A == null ? 0 : A.d_total_isc == null ? 0 : A.d_total_isc.Value,
                         OtroTributos =
                             A == null ? 0 : A.d_total_otrostributos == null ? 0 : A.d_total_otrostributos.Value,
                         TipoPersona = B == null ? 0 : B.i_IdTipoPersona.Value,
                         FechaDetraccion = A.t_FechaDetraccion.Value,
                         CorrelativoPle =
                             A.v_Mes.Trim() + A.v_Correlativo.Trim().Substring(0, 2) +
                             A.v_Correlativo.Trim().Substring(4, 8),
                         GrupoLlave =
                             pstr_grupollave == "SERIE DOCUMENTO" ? "SERIE DOCUMENTO : " + A.v_SerieDocumento : "",
                         GrupoLlave2 = pstr_grupollave == "SERIE DOCUMENTO" ? "TIPO DOCUMENTO : " + H.v_Nombre : "",
                         TotalDocumentos = H.v_Nombre == null ? "" : "TOTAL " + H.v_Nombre + " : ",
                         SiglasMoneda = A.i_IdMoneda != -1 ? F.v_Field.Trim() : "",
                         TipoBien = L == null ? "" : L.v_TipoExistencia == null ? "" : L.v_TipoExistencia,
                         IdPais = B.i_IdPais.Value,
                         DobleTributacion = M == null ? "" : M.v_Value2 == null ? "" : M.v_Value2,
                         ApePaterno = B.v_ApePaterno ?? "",
                         ApeMaterno = B.v_ApeMaterno ?? "",
                         PrimerNombre = B.v_PrimerNombre ?? "",
                         SegundoNombre = B.v_SegundoNombre ?? "",
                         EsDetraccion = A.i_EsDetraccion ?? 0,
                         //iCodigoDetraccion = N == null ? -1 : N.v_Field ,
                         CodigoDetraccion = N == null || N.v_Field == null || N.v_Field == "" ? "" : N.v_Field,
                         NumeroDetraccion = A.v_NroDetraccion ?? "",
                         DocumentoInverso = H.i_UsadoDocumentoInverso ?? 0,
                         Total = A.d_Total ?? 0,

                     }).ToList();


                var queryLiquidacionCompra = (from A in dbContext.liquidacioncompra
                                              join B in dbContext.cliente on new { IdProveedor = A.v_IdProveedor, Flag = "V", eliminado = 0 } equals
                                                  new { IdProveedor = B.v_IdCliente, Flag = B.v_FlagPantalla, eliminado = B.i_Eliminado.Value } into
                                                  B_join
                                              from B in B_join.DefaultIfEmpty()
                                              join F in dbContext.datahierarchy on new { IdMoneda = A.i_IdMoneda.Value, Grupo = 18, eliminado = 0 }
                                                  equals new { IdMoneda = F.i_ItemId, Grupo = F.i_GroupId, eliminado = F.i_IsDeleted.Value } into
                                                  F_join
                                              from F in F_join.DefaultIfEmpty()
                                              join I in dbContext.liquidacioncompradetalle on
                                                  new { IdLiquidacionCompra = A.v_IdLiquidacionCompra, eliminado = 0 } equals
                                                  new { IdLiquidacionCompra = I.v_IdLiquidacionCompra, eliminado = I.i_Eliminado.Value } into I_join
                                              from I in I_join.DefaultIfEmpty()
                                              join J in dbContext.productodetalle on
                                                  new { IdProductoDetalle = I.v_IdProductoDetalle, eliminado = 0 } equals
                                                  new { IdProductoDetalle = J.v_IdProductoDetalle, eliminado = J.i_Eliminado.Value } into J_join
                                              from J in J_join.DefaultIfEmpty()
                                              join K in dbContext.producto on new { idProducto = J.v_IdProducto, eliminado = 0 } equals
                                                  new { idProducto = K.v_IdProducto, eliminado = K.i_Eliminado.Value } into K_join
                                              from K in K_join.DefaultIfEmpty()
                                              join G in dbContext.datahierarchy on new { Igv = A.i_IdIgv.Value, eliminado = 0, Grupo = 27 } equals
                                                  new { Igv = G.i_ItemId, eliminado = G.i_IsDeleted.Value, Grupo = G.i_GroupId } into G_join
                                              from G in G_join.DefaultIfEmpty()
                                              join H in dbContext.documento on new { TipoDoc = A.i_IdTipoDocumento.Value, eliminado = 0 } equals
                                                  new { TipoDoc = H.i_CodigoDocumento, eliminado = H.i_Eliminado.Value } into H_join
                                              from H in H_join.DefaultIfEmpty()
                                              join L in dbContext.asientocontable on new { eliminado = 0, c = I.v_NroCuenta, p = periodo } equals
                                                  new { eliminado = L.i_Eliminado.Value, c = L.v_NroCuenta, p = L.v_Periodo } into L_join
                                              from L in L_join.DefaultIfEmpty()
                                              join M in dbContext.datahierarchy on
                                                  new { eliminado = 0, Grupo = 146, Tribut = B.i_IdConvenioDobleTributacion.Value } equals
                                                  new { eliminado = M.i_IsDeleted.Value, Grupo = M.i_GroupId, Tribut = M.i_ItemId } into M_join
                                              from M in M_join.DefaultIfEmpty()
                                              where
                                                  (A.i_Eliminado == 0) && A.t_FechaRegistro >= pstrt_FechaRegistroIni &&
                                                  A.t_FechaRegistro <= pstrt_FechaRegistroFin &&
                                                  (I.v_NroCuenta == pstrt_NroCuenta || pstrt_NroCuenta == "") &&
                                                  (A.i_IdTipoDocumento == pintIdTipoDocumento || pintIdTipoDocumento == -1) &&
                                                  (pintIdTipoCompra == -1) &&
                                                  (I.v_NroCuenta.Trim() == pstrt_NroCuenta.Trim() || pstrt_NroCuenta == "")

                                                  && A.i_IdEstado == 1
                                              //orderby pstrt_Orden
                                              select new ReporteRegistroCompraSunat
                                              {
                                                  Correlativo = A.v_Mes.Trim() + A.v_Correlativo.Trim(),

                                                  FechaVencimiento = A.t_FechaEmision.Value,
                                                  IdTipoDocumento = A.i_IdTipoDocumento.Value,
                                                  TipoDocumento = "",
                                                  SerieDocumento = A.v_SerieDocumento,
                                                  aniovencimiento = "",
                                                  CorrelativoDocumento = A.v_CorrelativoDocumento.Trim(),
                                                  NombreProveedor =
                                                      A.i_IdEstado == 1
                                                          ? (B.v_PrimerNombre + " " + B.v_ApePaterno + " " + B.v_ApeMaterno + " " +
                                                             B.v_RazonSocial).Trim()
                                                          : "**** ANULADO ****",
                                                  DocIdentidad = A.i_IdEstado == 1 ? B.i_IdTipoIdentificacion.Value : 0,
                                                  NroDocProveedor = A.i_IdEstado == 1 ? B.v_NroDocIdentificacion : "0",
                                                  TipoCambio = A.i_IdEstado == 1 ? A.d_TipoCambio.Value : 0,
                                                  LlaveOrdenar = "",
                                                  FechaRegistroRef = A.t_FechaEmision.Value,
                                                  IdTipoDocumentoRef = -1,
                                                  TipoDocumentoRef = "",
                                                  SerieDocumentoRef = "",
                                                  CorrelativoDocumentoRef = "",
                                                  NroCPagoNoDomiciliario =
                                                      A.i_IdTipoDocumento == 91 ? A.v_SerieDocumento + "-" + A.v_CorrelativoDocumento : "",

                                                  FechaEmision = A.t_FechaEmision.Value,
                                                  IgvNombre = G == null ? "" : G.v_Value1,
                                                  TipoCompra = "COMPRAS NACIONALES",
                                                  CompraGeneradaImportacion = "",
                                                  NumeroRegistro = "LC " + A.v_Mes + A.v_Correlativo + A.v_Periodo,
                                                  AnioAduanero = A.t_FechaEmision.Value,
                                                  Estado = A.i_IdEstado.Value,
                                                  DestinoCabecera = A.i_IdDestino == null ? 0 : A.i_IdDestino.Value,
                                                  DestinoGrilla = I == null ? 0 : I.i_IdDestino.Value,
                                                  idMoneda = A.i_IdMoneda.Value,
                                                  d_ValorVenta = A.d_ValorVenta == null ? 0 : A.d_ValorVenta.Value,
                                                  idCabecera = A.v_IdLiquidacionCompra == null ? "" : A.v_IdLiquidacionCompra,
                                                  anticipoDetalle = 0,
                                                  d_Igv = A.d_IGV == null ? 0 : A.d_IGV.Value,
                                                  TipoDocGenerado = A.i_IdTipoDocumento.Value,
                                                  serieDocumentoGenerado = A.v_SerieDocumento,
                                                  correlativoDocumentoGenerado = A.v_CorrelativoDocumento,
                                                  porcentajeIgv = "0",
                                                  d_ValorVentaDetalle = I == null ? 0 : I.d_ValorVenta == null ? 0 : I.d_ValorVenta.Value,
                                                  d_IgvDetalle = I == null ? 0 : I.d_Igv.Value,
                                                  Isc = 0,
                                                  OtroTributos = 0,
                                                  TipoPersona = B == null ? 0 : B.i_IdTipoPersona.Value,
                                                  FechaDetraccion = A.t_FechaEmision.Value,
                                                  CorrelativoPle =
                                                      A.v_Mes.Trim() + A.v_Correlativo.Trim().Substring(0, 2) +
                                                      A.v_Correlativo.Trim().Substring(4, 8),
                                                  GrupoLlave =
                                                      pstr_grupollave == "SERIE DOCUMENTO" ? "SERIE DOCUMENTO : " + A.v_SerieDocumento : "",
                                                  GrupoLlave2 = pstr_grupollave == "SERIE DOCUMENTO" ? "TIPO DOCUMENTO : " + H.v_Nombre : "",
                                                  TotalDocumentos = H.v_Nombre == null ? "" : "TOTAL " + H.v_Nombre + " : ",
                                                  SiglasMoneda = A.i_IdMoneda != -1 ? F.v_Field.Trim() : "",
                                                  TipoBien = L == null ? "" : L.v_TipoExistencia == null ? "" : L.v_TipoExistencia,
                                                  IdPais = B.i_IdPais.Value,
                                                  DobleTributacion = M == null ? "" : M.v_Value2 == null ? "" : M.v_Value2,
                                                  ApePaterno = B.v_ApePaterno ?? "",
                                                  ApeMaterno = B.v_ApeMaterno ?? "",
                                                  PrimerNombre = B.v_PrimerNombre ?? "",
                                                  SegundoNombre = B.v_SegundoNombre ?? "",
                                                  EsDetraccion = 0,
                                                  CodigoDetraccion = "",
                                                  NumeroDetraccion = "",
                                                  DocumentoInverso = H.i_UsadoDocumentoInverso ?? 0,
                                                  Total = A.d_Total ?? 0,

                                              }).ToList();


                if (LibroElectronico == 1 &&
                    dbContext.importacion.Where(
                        x => x.t_FechaRegistro >= pstrt_FechaRegistroIni && x.t_FechaRegistro <= pstrt_FechaRegistroFin)
                        .Any())
                {
                    if (
                        !dbContext.cliente.Where(
                            x => x.v_NroDocIdentificacion == Constants.RucSunat && x.i_Eliminado == 0 && x.i_Activo == 1)
                            .Any())
                    {
                        _clienteDto = new clienteDto();
                        _clienteDto.v_RazonSocial = "SUNAT";
                        _clienteDto.v_NroDocIdentificacion = Constants.RucSunat;
                        _clienteDto.v_CodCliente = Constants.RucSunat;
                        _clienteDto.i_IdTipoPersona = 2;
                        _clienteDto.i_IdTipoIdentificacion = 6;
                        _clienteDto.i_Activo = 1;
                        _clienteDto.i_IdSexo = 2;
                        _clienteDto.v_DirecPrincipal = "LIMA";
                        _clienteDto.i_IdPais = 1;
                        _clienteDto.i_IdDepartamento = 1391;
                        _clienteDto.i_IdProvincia = 1392;
                        _clienteDto.i_IdDistrito = 1393;
                        _objClienteBL.InsertarCliente(ref objOperationResult, _clienteDto,
                            Globals.ClientSession.GetAsList(), null, null, null, null, null, null);
                    }
                }





                var ComprasImportacionesConcatenadas =
                    queryCompras.ToList().Concat(queryLiquidacionCompra).ToList();

                if (pstrt_Orden == "CORRELATIVO")
                {
                    var Temporal1 =
                        ComprasImportacionesConcatenadas.ToList()
                            .GroupBy(x => x.NumeroRegistro)
                            .Where(y => y.Any(x => x.DestinoCabecera != 5 && x.DestinoCabecera != 999))
                            .Select(x => x.First())
                            .OrderBy(o => o.TipoCompra)
                            .ThenBy(p => p.Correlativo)
                            .ToList();
                    var Temporal2 =
                        ComprasImportacionesConcatenadas.ToList()
                            .Where(y => y.DestinoCabecera == 5 || y.DestinoCabecera == 999)
                            .OrderBy(o => o.TipoCompra)
                            .ThenBy(p => p.Correlativo)
                            .ToList();
                    ListaFinal1 = Temporal1.Concat(Temporal2).ToList();
                }

                var index = 1;

                foreach (var item in ListaFinal1)
                {
                    if (ConsideraDocInternos == 0 && _objDocumentoBL.DocumentoEsContable(item.IdTipoDocumento))
                    {
                        objReporte = new ReporteRegistroCompraSunat();
                        objReporte = item;
                        //objReporte.CodigoDetraccion = item.iCodigoDetraccion != -1 ? item.iCodigoDetraccion.ToString("000") : "-1";
                        objReporte.TipoDocumento = item.IdTipoDocumento.ToString("000");


                        if (item.TipoCompra == "COMPRAS NACIONALES")
                        {
                            if (item.DocumentoInverso == 1)
                            {
                                objReporte.BaseImponible1 = item.Estado == 1
                                    ? item.DestinoCabecera <= 4 ? item.d_ValorVenta * -1 : item.d_ValorVentaDetalle * -1
                                    : 0;
                                objReporte.Isc = item.Estado == 1 ? item.DestinoCabecera <= 4 ? item.Isc * -1 : item.Isc * -1 : 0;
                                objReporte.Igv1 = item.Estado == 1
                                    ? item.DestinoCabecera <= 4 ? item.d_Igv * -1 : item.d_IgvDetalle * -1
                                    : 0;
                            }
                            else
                            {
                                objReporte.BaseImponible1 = item.Estado == 1
                                 ? item.DestinoCabecera <= 4 ? item.d_ValorVenta : item.d_ValorVentaDetalle
                                 : 0;
                                objReporte.Isc = item.Estado == 1 ? item.DestinoCabecera <= 4 ? item.Isc : item.Isc : 0;
                                objReporte.Igv1 = item.Estado == 1
                                    ? item.DestinoCabecera <= 4 ? item.d_Igv : item.d_IgvDetalle
                                    : 0;

                            }


                        }
                        List<ReporteRegistroCompraSunat> queryImportaciones = new List<ReporteRegistroCompraSunat>();

                        var QueryComprasLiquidacionesDictionary = queryCompras.Concat(queryLiquidacionCompra).GroupBy(g => g.idCabecera)
                   .ToDictionary(o => o.Key, hg => hg);

                        IGrouping<string, ReporteRegistroCompraSunat> xz;
                        var detalles = QueryComprasLiquidacionesDictionary.TryGetValue(item.idCabecera, out xz) ? xz.ToList() : null;

                        // objReporte.Total = objReporte.Total;
                        if (Globals.ClientSession.i_Periodo == 2016)
                        {
                            objReporte.Total = _objDocumentoBL.DocumentoEsInverso(item.IdTipoDocumento) ? objReporte.Total * -1 : objReporte.Total;
                        }
                        else
                        {
                            // Compras2017
                            objReporte.Total = _objDocumentoBL.DocumentoEsInverso(item.IdTipoDocumento) ? pintIdMoneda == item.idMoneda ? item.Total * -1 : CalcularBaseImponibleIgv2017(ref objOperationResult, "T", pintIdMoneda, item, detalles, "", queryImportaciones.ToList()) * -1 :
                               pintIdMoneda == item.idMoneda ? item.Total : CalcularBaseImponibleIgv2017(ref objOperationResult, "T", pintIdMoneda, item, detalles, "", queryImportaciones.ToList());
                        }


                        objReporte.LlaveOrdenar = pstrt_Orden == "CORRELATIVO"
                            ? item.Correlativo.Trim()
                            : "" + pstrt_Orden == "FECHAREGISTRO"
                                ? item.FechaEmision.ToString()
                                : "" + pstrt_Orden == "FECHAVENCIMIENTO"
                                    ? item.FechaVencimiento.ToString()
                                    : "" + pstrt_Orden == "TIPODOCUMENTO"
                                        ? item.TipoDocumento.Trim() + item.SerieDocumento.Trim() +
                                          item.CorrelativoDocumento.Trim()
                                        : "" + pstrt_Orden == "NOMBREPROVEEDOR" ? item.NombreProveedor : "";
                        objReporte.TipoDocumentoRef = item.IdTipoDocumentoRef.Value.ToString("000");

                        objReporte.IgvNombre = "IGV AL " + item.IgvNombre + " :";
                        objReporte.OtroTributos = objReporte.TipoCompra == "COMPRAS NACIONALES"
                            ? item.Estado == 1
                                ? Globals.ClientSession.i_Periodo == 2016 ? CalcularBaseImponibleIgv2016(ref objOperationResult, "OT", pintIdMoneda, item, queryCompras.ToList(), "1",
                                queryImportaciones.ToList()) : CalcularBaseImponibleIgv2017(ref objOperationResult, "OT", pintIdMoneda, item, detalles, "1",
                                queryImportaciones.ToList())
                                : 0
                            : 0;
                        objReporte.Isc = objReporte.TipoCompra == "COMPRAS NACIONALES"
                            ? item.Estado == 1
                                ? Globals.ClientSession.i_Periodo == 2016 ? CalcularBaseImponibleIgv2016(ref objOperationResult, "ISC", pintIdMoneda, item, queryCompras.ToList(), "1",
                                queryImportaciones.ToList()) : CalcularBaseImponibleIgv2017(ref objOperationResult, "ISC", pintIdMoneda, item, detalles, "1",
                                queryImportaciones.ToList())
                                : 0
                            : 0;

                        index = index + 1;
                        ListaReporteFinalizado.Add(objReporte);
                    }
                }

                #endregion

                objOperationResult.Success = 1;
                return ListaReporteFinalizado.OrderBy(l => l.Correlativo).ToList();
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                // objOperationResult.AdditionalInformation = "ComprasBL.()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }
        }











        public List<ReporteComprasLineaAnalitico> ReporteComprasLinea(ref OperationResult objOperationResult,
            DateTime FechaInicio, DateTime FechaFin, string Filtro, string Agrupar, string Ordenar,
            string pstr_grupollave, string TipoReporte, int ConsideraDocInternos, bool UsadoReporteComprasGasto)
        {
            try
            {
                objOperationResult.Success = 1;

                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var Compras = from compras in dbContext.compra
                                  join compradetalles in dbContext.compradetalle on new { IdCompra = compras.v_IdCompra, eliminado = 0 } equals new { IdCompra = compradetalles.v_IdCompra, eliminado = compradetalles.i_Eliminado.Value } into compradetalles_join
                                  from compradetalles in compradetalles_join.DefaultIfEmpty()
                                  join productDetalle in dbContext.productodetalle on new { IdProductoDet = compradetalles.v_IdProductoDetalle, eliminado = 0 } equals new { IdProductoDet = productDetalle.v_IdProductoDetalle, eliminado = productDetalle.i_Eliminado.Value } into productDetalle_join
                                  from productDetalle in productDetalle_join.DefaultIfEmpty()
                                  join product in dbContext.producto on new { IdProducto = productDetalle.v_IdProducto, eliminado = 0 } equals new { IdProducto = product.v_IdProducto, eliminado = product.i_Eliminado.Value } into producto_join
                                  from product in producto_join.DefaultIfEmpty()
                                  join document in dbContext.documento on new { IdDocumento = compras.i_IdTipoDocumento.Value, eliminado = 0 } equals new { IdDocumento = document.i_CodigoDocumento, eliminado = document.i_Eliminado.Value } into document_join
                                  from document in document_join.DefaultIfEmpty()
                                  join UnidadM in dbContext.datahierarchy on new { Item = compradetalles.i_IdUnidadMedida.Value, eliminado = 0, Grupo = 17 } equals new { Item = UnidadM.i_ItemId, eliminado = UnidadM.i_IsDeleted.Value, Grupo = UnidadM.i_GroupId } into UnidadM_join
                                  from UnidadM in UnidadM_join.DefaultIfEmpty()
                                  join Proveedor in dbContext.cliente on new { Flag = "V", eliminado = 0, IdCliente = compras.v_IdProveedor } equals new { Flag = Proveedor.v_FlagPantalla, eliminado = Proveedor.i_Eliminado.Value, IdCliente = Proveedor.v_IdCliente } into Proveedor_join
                                  from Proveedor in Proveedor_join.DefaultIfEmpty()
                                  join almacens in dbContext.almacen on new { IdAlmacen = compradetalles.i_IdAlmacen.Value, eliminado = 0 } equals new { IdAlmacen = almacens.i_IdAlmacen, eliminado = almacens.i_Eliminado.Value } into almacens_join
                                  from almacens in almacens_join.DefaultIfEmpty()
                                  join AlmacenEst in dbContext.establecimientoalmacen on new { Id = almacens.i_IdAlmacen, eliminado = 0 } equals new { Id = AlmacenEst.i_IdAlmacen.Value, eliminado = AlmacenEst.i_Eliminado.Value } into AlmacenEst_join
                                  from AlmacenEst in AlmacenEst_join.DefaultIfEmpty()
                                  join lineas in dbContext.linea on new { IdLinea = product.v_IdLinea, eliminado = 0 } equals new { IdLinea = lineas.v_IdLinea, eliminado = lineas.i_Eliminado.Value } into lineas_join
                                  from lineas in lineas_join.DefaultIfEmpty()
                                  join marcas in dbContext.marca on new { IdMarca = product.v_IdMarca, eliminado = 0 } equals new { IdMarca = marcas.v_IdMarca, eliminado = marcas.i_Eliminado.Value } into marcas_join
                                  from marcas in marcas_join.DefaultIfEmpty()
                                  join asientocont in dbContext.asientocontable on new { p = periodo, eliminado = 0, cuenta = compradetalles.v_NroCuenta } equals new { p = asientocont.v_Periodo, eliminado = asientocont.i_Eliminado.Value, cuenta = asientocont.v_NroCuenta } into asientocont_join
                                  from asientocont in asientocont_join.DefaultIfEmpty()
                                  where
                                      compras.i_Eliminado == 0 && compras.t_FechaRegistro >= FechaInicio &&
                                      compras.t_FechaRegistro <= FechaFin && compras.i_IdEstado == 1
                                  select new ReporteComprasLineaAnalitico
                                  {
                                      NumeroRegistro =
                                          compras == null ? "**SIN NÚMERO**" : compras.v_Mes + " " + compras.v_Correlativo,
                                      Producto =
                                          product == null
                                              ? "**SIN PRODUCTO**"
                                              : product.v_CodInterno.Trim() + " " + product.v_Descripcion.Trim(),
                                      Fecha = compras.t_FechaRegistro.Value,
                                      NroDocumento =
                                          compras == null
                                              ? "**SIN NÚMERO DOC.**"
                                              : document == null
                                                  ? compras.v_SerieDocumento.Trim() + "-" + compras.v_CorrelativoDocumento.Trim()
                                                  : document.v_Siglas + " " + compras.v_SerieDocumento + "-" +
                                                    compras.v_CorrelativoDocumento,
                                      Cuenta = compradetalles == null ? "**SIN DETALLE**" : compradetalles.v_NroCuenta,
                                      Cantidad = compradetalles == null ? 0 : compradetalles.d_Cantidad.Value,
                                      UnidadMedida = UnidadM == null ? "** U.M. NO EXISTE **" : UnidadM.v_Value1,
                                      PrecioUnitario = compradetalles == null ? 0 : compradetalles.d_Cantidad.Value,
                                      VentaSoles =
                                          compras == null
                                              ? 0
                                              : compras.i_IdMoneda == (int)Currency.Soles
                                                  ? document.i_UsadoDocumentoInverso.Value == 1
                                                      ? compradetalles.d_ValorVenta.Value * -1
                                                      : compradetalles.d_ValorVenta.Value
                                                  : document.i_UsadoDocumentoInverso.Value == 1
                                                      ? compradetalles.d_ValorVenta.Value * compras.d_TipoCambio.Value * -1
                                                      : compradetalles.d_ValorVenta.Value * compras.d_TipoCambio.Value,
                                      VentasDolares =
                                          compras == null
                                              ? 0
                                              : compras.i_IdMoneda == (int)Currency.Dolares
                                                  ? document.i_UsadoDocumentoInverso.Value == 1
                                                      ? compradetalles.d_ValorVenta.Value * -1
                                                      : compradetalles.d_ValorVenta.Value
                                                  : document.i_UsadoDocumentoInverso.Value == 1
                                                      ? compradetalles.d_ValorVenta.Value / compras.d_TipoCambio.Value * -1
                                                      : compradetalles.d_ValorVenta.Value / compras.d_TipoCambio.Value,
                                      NroPedido = compradetalles == null ? "**DETALLE NO EXISTE **" : compradetalles.v_NroPedido,
                                      GrupoLLave =
                                          pstr_grupollave == "PROVEEDOR" ? Proveedor == null
                                                  ? "** " + pstr_grupollave + " NO EXISTE **"
                                                  : pstr_grupollave + " : " + Proveedor.v_CodCliente.Trim().ToUpper() + " / " +
                                                    (Proveedor.v_ApePaterno.Trim().ToUpper() + " " +
                                                     Proveedor.v_ApeMaterno.Trim().ToUpper() + " " +
                                                     Proveedor.v_PrimerNombre.Trim().ToUpper() + " " +
                                                     Proveedor.v_RazonSocial.Trim().ToUpper()).Trim()
                                              : pstr_grupollave == "PRODUCTO"
                                                  ? product == null
                                                      ? "** " + pstr_grupollave + " NO EXISTE **"
                                                      : pstr_grupollave + " : " + product.v_CodInterno.Trim().ToUpper() + " / " +
                                                        product.v_Descripcion.Trim().ToUpper()
                                                  : "",
                                      IdEstablecimiento = AlmacenEst == null ? -1 : AlmacenEst.i_IdEstablecimiento.Value,
                                      IdAlmacen = compradetalles == null ? -1 : compradetalles.i_IdAlmacen.Value,
                                      pIntTipoCompra = compras == null ? -1 : compras.i_IdTipoCompra.Value,
                                      pstrAlmacen = UsadoReporteComprasGasto ? "" : almacens == null ? "**ALMACÉN NO EXISTE **" : almacens.v_Nombre,
                                      pIntLinea = lineas == null ? "**LINEA NO EXISTE**" : lineas.v_IdLinea,
                                      Linea = UsadoReporteComprasGasto ? "CUENTA : " + compradetalles.v_NroCuenta + " " + asientocont.v_NombreCuenta : lineas == null ? "**LINEA NO EXISTE**" : lineas.v_CodLinea + " " + lineas.v_Nombre,
                                      CodigoProducto = product == null ? "**PRODUCTO NO EXISTE **" : product.v_CodInterno.Trim(),
                                      CodigoProveedor =
                                          Proveedor == null ? "**PROVEEDOR NO EXISTE **" : Proveedor.v_CodCliente.Trim(),
                                      Marca = UsadoReporteComprasGasto ? "" : marcas == null ? "**MARCA NO EXISTE**" : marcas.v_CodMarca + " " + marcas.v_Nombre,
                                      v_Marca = marcas == null ? "-1" : marcas.v_IdMarca,
                                      TipoDocumento = compras.i_IdTipoDocumento.Value,
                                      EsDocInverso = document.i_UsadoDocumentoInverso == 1 ? true : false
                                  };

                    if (!string.IsNullOrEmpty(Filtro))
                    {
                        Compras = Compras.Where(Filtro);
                    }

                    if (!string.IsNullOrEmpty(Ordenar))
                    {
                        Compras = Compras.Where(Ordenar);
                    }

                    if (TipoReporte == "ANALITICO")
                    {
                        return ConsideraDocInternos == 1
                            ? Compras.ToList().OrderBy(x => x.NumeroRegistro).ToList()
                            : Compras.ToList()
                                .Where(x => _objDocumentoBL.DocumentoEsContable(x.TipoDocumento))
                                .OrderBy(x => x.NumeroRegistro)
                                .ToList();
                    }
                    var ComprasFinal = new List<ReporteComprasLineaAnalitico>();
                    var ListaCompras =
                        Compras.ToList()
                            .GroupBy(x => new { x.pstrAlmacen, x.Linea, x.Marca, x.GrupoLLave })
                            .Select(group => group.First())
                            .OrderBy(o => o.NumeroRegistro)
                            .ToList();

                    foreach (var item in ListaCompras)
                    {
                        if (ConsideraDocInternos == 1)
                        {
                            var objReporte = new ReporteComprasLineaAnalitico();


                            objReporte = item;
                            objReporte.VentaSoles =
                                Compras.ToList()
                                    .Where(
                                        x =>
                                            x.GrupoLLave == item.GrupoLLave && x.pstrAlmacen == item.pstrAlmacen &&
                                            x.Linea == item.Linea && x.Marca == item.Marca && !x.EsDocInverso)
                                    .Sum(x => x.VentaSoles) -
                                Compras.ToList()
                                    .Where(
                                        x =>
                                            x.GrupoLLave == item.GrupoLLave && x.pstrAlmacen == item.pstrAlmacen &&
                                            x.Linea == item.Linea && x.Marca == item.Marca && x.EsDocInverso)
                                    .Sum(x => x.VentaSoles);
                            objReporte.VentasDolares =
                                Compras.ToList()
                                    .Where(
                                        x =>
                                            x.GrupoLLave == item.GrupoLLave && x.pstrAlmacen == item.pstrAlmacen &&
                                            x.Linea == item.Linea && x.Marca == item.Marca && !x.EsDocInverso)
                                    .Sum(x => x.VentasDolares) -
                                Compras.ToList()
                                    .Where(
                                        x =>
                                            x.GrupoLLave == item.GrupoLLave && x.pstrAlmacen == item.pstrAlmacen &&
                                            x.Linea == item.Linea && x.Marca == item.Marca && x.EsDocInverso)
                                    .Sum(x => x.VentasDolares);
                            ComprasFinal.Add(objReporte);
                        }
                        else if (_objDocumentoBL.DocumentoEsContable(item.TipoDocumento))
                        {
                            var objReporte = new ReporteComprasLineaAnalitico();
                            objReporte = item;
                            var Compritas =
                                Compras.ToList()
                                    .Where(x => _objDocumentoBL.DocumentoEsContable(x.TipoDocumento))
                                    .ToList();
                            objReporte.VentaSoles =
                                Compritas.ToList()
                                    .Where(
                                        x =>
                                            x.GrupoLLave == item.GrupoLLave && x.pstrAlmacen == item.pstrAlmacen &&
                                            x.Linea == item.Linea && x.Marca == item.Marca && !x.EsDocInverso)
                                    .Sum(x => x.VentaSoles) +
                                Compritas.ToList()
                                    .Where(
                                        x =>
                                            x.GrupoLLave == item.GrupoLLave && x.pstrAlmacen == item.pstrAlmacen &&
                                            x.Linea == item.Linea && x.Marca == item.Marca && x.EsDocInverso)
                                    .Sum(x => x.VentaSoles);
                            objReporte.VentasDolares =
                                Compritas.ToList()
                                    .Where(
                                        x =>
                                            x.GrupoLLave == item.GrupoLLave && x.pstrAlmacen == item.pstrAlmacen &&
                                            x.Linea == item.Linea && x.Marca == item.Marca && !x.EsDocInverso)
                                    .Sum(x => x.VentasDolares) +
                                Compritas.ToList()
                                    .Where(
                                        x =>
                                            x.GrupoLLave == item.GrupoLLave && x.pstrAlmacen == item.pstrAlmacen &&
                                            x.Linea == item.Linea && x.Marca == item.Marca && x.EsDocInverso)
                                    .Sum(x => x.VentasDolares);
                            ComprasFinal.Add(objReporte);
                        }
                    }

                    return ComprasFinal;
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;
            }
        }



        public List<ReporteComprasGastoAnalitico> ReporteComprasGastosAnalitico(ref OperationResult objOperationResult,
           DateTime FechaInicio, DateTime FechaFin, string Filtro, string Agrupar, string Ordenar,
           string pstr_grupollave, string TipoReporte, int ConsideraDocInternos)
        {
            try
            {
                objOperationResult.Success = 1;

                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var Compras = (from compras in dbContext.compra
                                   join compradetalles in dbContext.compradetalle on new { IdCompra = compras.v_IdCompra, eliminado = 0 } equals new { IdCompra = compradetalles.v_IdCompra, eliminado = compradetalles.i_Eliminado.Value } into compradetalles_join
                                   from compradetalles in compradetalles_join.DefaultIfEmpty()
                                   join productDetalle in dbContext.productodetalle on new { IdProductoDet = compradetalles.v_IdProductoDetalle, eliminado = 0 } equals new { IdProductoDet = productDetalle.v_IdProductoDetalle, eliminado = productDetalle.i_Eliminado.Value } into productDetalle_join
                                   from productDetalle in productDetalle_join.DefaultIfEmpty()
                                   join product in dbContext.producto on new { IdProducto = productDetalle.v_IdProducto, eliminado = 0 } equals new { IdProducto = product.v_IdProducto, eliminado = product.i_Eliminado.Value } into producto_join
                                   from product in producto_join.DefaultIfEmpty()
                                   join document in dbContext.documento on new { IdDocumento = compras.i_IdTipoDocumento.Value, eliminado = 0 } equals new { IdDocumento = document.i_CodigoDocumento, eliminado = document.i_Eliminado.Value } into document_join
                                   from document in document_join.DefaultIfEmpty()
                                   join UnidadM in dbContext.datahierarchy on new { Item = compradetalles.i_IdUnidadMedida.Value, eliminado = 0, Grupo = 17 } equals new { Item = UnidadM.i_ItemId, eliminado = UnidadM.i_IsDeleted.Value, Grupo = UnidadM.i_GroupId } into UnidadM_join
                                   from UnidadM in UnidadM_join.DefaultIfEmpty()
                                   join Proveedor in dbContext.cliente on new { Flag = "V", eliminado = 0, IdCliente = compras.v_IdProveedor } equals new { Flag = Proveedor.v_FlagPantalla, eliminado = Proveedor.i_Eliminado.Value, IdCliente = Proveedor.v_IdCliente } into Proveedor_join
                                   from Proveedor in Proveedor_join.DefaultIfEmpty()
                                   join asiento in dbContext.asientocontable on new { cuenta = compradetalles.v_NroCuenta, eliminado = 0, p = periodo } equals new { cuenta = asiento.v_NroCuenta, eliminado = asiento.i_Eliminado.Value, p = asiento.v_Periodo } into asiento_join
                                   from asiento in asiento_join.DefaultIfEmpty()

                                   where
                                       compras.i_Eliminado == 0 && compras.t_FechaRegistro >= FechaInicio &&
                                       compras.t_FechaRegistro <= FechaFin && compras.i_IdEstado == 1
                                   select new
                                   {
                                       NroRegistro = compras == null ? "**SIN NÚMERO**" : compras.v_Mes + " " + compras.v_Correlativo,
                                       Fecha = compras.t_FechaRegistro.Value,
                                       Documento = document.v_Siglas + " " + compras.v_SerieDocumento + " " + compras.v_CorrelativoDocumento,
                                       Articulo =
                                           product == null
                                               ? "**SIN PRODUCTO**"
                                               : product.v_CodInterno.Trim() + " " + product.v_Descripcion.Trim(),
                                       Cuenta = compradetalles == null ? "**SIN DETALLE**" : compradetalles.v_NroCuenta,
                                       Cantidad = compradetalles == null ? 0 : compradetalles.d_Cantidad.Value,

                                       PrecioUnitario = compradetalles == null ? 0 : compradetalles.d_Cantidad.Value,
                                       VentaSoles =
                                           compras == null
                                               ? 0
                                               : compras.i_IdMoneda == (int)Currency.Soles
                                                   ? document.i_UsadoDocumentoInverso.Value == 1
                                                       ? compradetalles.d_ValorVenta.Value * -1
                                                       : compradetalles.d_ValorVenta.Value
                                                   : document.i_UsadoDocumentoInverso.Value == 1
                                                       ? compradetalles.d_ValorVenta.Value * compras.d_TipoCambio.Value * -1
                                                       : compradetalles.d_ValorVenta.Value * compras.d_TipoCambio.Value,
                                       VentasDolares =
                                           compras == null
                                               ? 0
                                               : compras.i_IdMoneda == (int)Currency.Dolares
                                                   ? document.i_UsadoDocumentoInverso.Value == 1
                                                       ? compradetalles.d_ValorVenta.Value * -1
                                                       : compradetalles.d_ValorVenta.Value
                                                   : document.i_UsadoDocumentoInverso.Value == 1
                                                       ? compradetalles.d_ValorVenta.Value / compras.d_TipoCambio.Value * -1
                                                       : compradetalles.d_ValorVenta.Value / compras.d_TipoCambio.Value,
                                       NroPedido = compradetalles == null ? "**DETALLE NO EXISTE **" : compradetalles.v_NroPedido,
                                       GrupoLLave1 = "NRO. CUENTA : " + compradetalles.v_NroCuenta + " " + asiento.v_NombreCuenta,
                                       GrupoLlave2 = "2",
                                       IdCompra = compras.v_IdCompra,
                                       TipoDocumento = compras.i_IdTipoDocumento.Value,
                                       //EsDocInverso = document.i_UsadoDocumentoInverso == 1 ? true : false
                                   }).Select(o => new ReporteComprasGastoAnalitico
                                  {

                                      NroRegistro = o.NroRegistro,
                                      sFecha = o.Fecha.ToShortDateString(),
                                      Documento = o.Documento,
                                      Articulo = o.Articulo,
                                      Cuenta = o.Cuenta,
                                      Cantidad = o.Cantidad,
                                      PrecioUnitario = o.PrecioUnitario,
                                      VentaSoles = o.VentaSoles,
                                      VentasDolares = o.VentasDolares,
                                      NroPedido = o.NroPedido,
                                      GrupoLLave1 = o.GrupoLLave1,
                                      GrupoLLave2 = o.GrupoLlave2,
                                      IdCompra = o.IdCompra,
                                      TipoDocumento = o.TipoDocumento,

                                  }).ToList().AsQueryable();

                    if (!string.IsNullOrEmpty(Filtro))
                    {
                        Compras = Compras.Where(Filtro);
                    }


                    if (TipoReporte == "ANALITICO")
                    {
                        return ConsideraDocInternos == 1
                            ? Compras.ToList().OrderBy(x => x.NroRegistro).ToList()
                            : Compras.ToList()
                                .Where(x => _objDocumentoBL.DocumentoEsContable(x.TipoDocumento))
                                .OrderBy(x => x.NroRegistro)
                                .ToList();
                    }
                    var ComprasFinal = new List<ReporteComprasGastoAnalitico>();
                    //var ListaCompras =
                    //    Compras.ToList()
                    //        .GroupBy(x => new { x.pstrAlmacen, x.Linea, x.Marca, x.GrupoLLave })
                    //        .Select(group => group.First())
                    //        .OrderBy(o => o.NumeroRegistro)
                    //        .ToList();

                    //foreach (var item in ListaCompras)
                    //{
                    //    if (ConsideraDocInternos == 1)
                    //    {
                    //        var objReporte = new ReporteComprasLineaAnalitico();
                    //        objReporte = item;
                    //        objReporte.VentaSoles =
                    //            Compras.ToList()
                    //                .Where(
                    //                    x =>
                    //                        x.GrupoLLave == item.GrupoLLave && x.pstrAlmacen == item.pstrAlmacen &&
                    //                        x.Linea == item.Linea && x.Marca == item.Marca && !x.EsDocInverso)
                    //                .Sum(x => x.VentaSoles) -
                    //            Compras.ToList()
                    //                .Where(
                    //                    x =>
                    //                        x.GrupoLLave == item.GrupoLLave && x.pstrAlmacen == item.pstrAlmacen &&
                    //                        x.Linea == item.Linea && x.Marca == item.Marca && x.EsDocInverso)
                    //                .Sum(x => x.VentaSoles);
                    //        objReporte.VentasDolares =
                    //            Compras.ToList()
                    //                .Where(
                    //                    x =>
                    //                        x.GrupoLLave == item.GrupoLLave && x.pstrAlmacen == item.pstrAlmacen &&
                    //                        x.Linea == item.Linea && x.Marca == item.Marca && !x.EsDocInverso)
                    //                .Sum(x => x.VentasDolares) -
                    //            Compras.ToList()
                    //                .Where(
                    //                    x =>
                    //                        x.GrupoLLave == item.GrupoLLave && x.pstrAlmacen == item.pstrAlmacen &&
                    //                        x.Linea == item.Linea && x.Marca == item.Marca && x.EsDocInverso)
                    //                .Sum(x => x.VentasDolares);
                    //        ComprasFinal.Add(objReporte);
                    //    }
                    //    else if (_objDocumentoBL.DocumentoEsContable(item.TipoDocumento))
                    //    {
                    //        var objReporte = new ReporteComprasLineaAnalitico();
                    //        objReporte = item;
                    //        var Compritas =
                    //            Compras.ToList()
                    //                .Where(x => _objDocumentoBL.DocumentoEsContable(x.TipoDocumento))
                    //                .ToList();
                    //        objReporte.VentaSoles =
                    //            Compritas.ToList()
                    //                .Where(
                    //                    x =>
                    //                        x.GrupoLLave == item.GrupoLLave && x.pstrAlmacen == item.pstrAlmacen &&
                    //                        x.Linea == item.Linea && x.Marca == item.Marca && !x.EsDocInverso)
                    //                .Sum(x => x.VentaSoles) +
                    //            Compritas.ToList()
                    //                .Where(
                    //                    x =>
                    //                        x.GrupoLLave == item.GrupoLLave && x.pstrAlmacen == item.pstrAlmacen &&
                    //                        x.Linea == item.Linea && x.Marca == item.Marca && x.EsDocInverso)
                    //                .Sum(x => x.VentaSoles);
                    //        objReporte.VentasDolares =
                    //            Compritas.ToList()
                    //                .Where(
                    //                    x =>
                    //                        x.GrupoLLave == item.GrupoLLave && x.pstrAlmacen == item.pstrAlmacen &&
                    //                        x.Linea == item.Linea && x.Marca == item.Marca && !x.EsDocInverso)
                    //                .Sum(x => x.VentasDolares) +
                    //            Compritas.ToList()
                    //                .Where(
                    //                    x =>
                    //                        x.GrupoLLave == item.GrupoLLave && x.pstrAlmacen == item.pstrAlmacen &&
                    //                        x.Linea == item.Linea && x.Marca == item.Marca && x.EsDocInverso)
                    //                .Sum(x => x.VentasDolares);
                    //        ComprasFinal.Add(objReporte);
                    //    }
                    //}

                    return ComprasFinal;
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;
            }
        }


















        private List<decimal> CalcularSumaDetalleCompraSunat(List<ReporteRegistroCompraSunat> Lista,
            string CorrelativoDocumento)
        {
            var objData = new valoresRegistroVenta();
            var Retonar = new List<decimal>();
            var xx = Lista.Where(p => p.CorrelativoDocumento == CorrelativoDocumento);


            decimal BaseImponible1, Igv1, BaseImponible2, Igv2, BaseImponible3, Igv3, ValorAdquisiciones, Total;
            BaseImponible1 = xx.Sum(x => x.BaseImponible1.Value);
            Igv1 = xx.Sum(x => x.Igv1.Value);
            BaseImponible2 = xx.Sum(x => x.BaseImponible2.Value);
            Igv2 = xx.Sum(x => x.Igv2.Value);
            BaseImponible3 = xx.Sum(x => x.BaseImponible3.Value);
            Igv3 = xx.Sum(x => x.Igv3.Value);
            ValorAdquisiciones = xx.Sum(x => x.ValorAdquisiciones.Value);
            Total = xx.Sum(x => x.Total.Value);


            Retonar.Add(xx.Sum(x => x.BaseImponible1.Value));
            Retonar.Add(Igv1);
            Retonar.Add(BaseImponible2);
            Retonar.Add(Igv2);
            Retonar.Add(BaseImponible3);
            Retonar.Add(Igv3);
            Retonar.Add(ValorAdquisiciones);
            Retonar.Add(Total);
            return Retonar;
        }

        public List<RegistroRetenciones> ReporteRegistroDetracciones(ref OperationResult objOperationResult,
            DateTime FechaInicio, DateTime FechaFin, int TipoCompra, string NroCuenta, string Orden, int IdMoneda)
        {
            try
            {
                objOperationResult.Success = 1;
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var CompraDetalle =
                        dbContext.compradetalle.Where(
                            x => x.i_Eliminado == 0 && (x.v_NroCuenta == NroCuenta || NroCuenta == "")).ToList();

                    var compras = (from a in dbContext.compra
                                   join b in dbContext.documento on new { TipoDoc = a.i_IdTipoDocumento.Value, eliminado = 0 } equals
                                       new { TipoDoc = b.i_CodigoDocumento, eliminado = b.i_Eliminado.Value } into b_join
                                   from b in b_join.DefaultIfEmpty()
                                   join c in dbContext.cliente on new { c = a.v_IdProveedor } equals new { c = c.v_IdCliente } into
                                       c_join
                                   from c in c_join.DefaultIfEmpty()
                                   join d in dbContext.compradetalle on new { c = a.v_IdCompra, eliminado = 0 } equals
                                       new { c = d.v_IdCompra, eliminado = d.i_Eliminado.Value } into d_join
                                   from d in d_join.DefaultIfEmpty()
                                   where a.i_Eliminado == 0 && a.i_IdEstado == 1 && a.i_EsDetraccion == 1
                                         && (a.i_IdTipoCompra == TipoCompra || TipoCompra == -1)
                                         && (d.v_NroCuenta == NroCuenta || NroCuenta == "") && a.t_FechaRegistro >= FechaInicio &&
                                         a.t_FechaRegistro <= FechaFin
                                   select new RegistroRetenciones
                                   {
                                       NroRegistro = a.v_Mes + " " + a.v_Correlativo,
                                       FechaEmision = a.t_FechaEmision.Value,
                                       TipoNumeroDocumento =
                                           b.v_Siglas + "   " + a.v_SerieDocumento + " " + a.v_CorrelativoDocumento,
                                       RucProveedor = c == null ? "" : c.v_NroDocIdentificacion.Trim(),
                                       RazonSocialProveedor =
                                           c == null
                                               ? ""
                                               : (c.v_ApePaterno + " " + c.v_ApeMaterno + " " + c.v_PrimerNombre + " " +
                                                  c.v_RazonSocial).Trim(),
                                       NroDetraccion = a.v_NroDetraccion,
                                       FechaDetraccion = a.t_FechaDetraccion.Value,
                                       IdCompra = a.v_IdCompra,
                                       Moneda = a.i_IdMoneda == 1 ? "S" : "D"
                                   }).ToList().Select(x =>
                        {
                            var DetraccionSoles =
                                CompraDetalle.Where(a => a.v_IdCompra == x.IdCompra)
                                    .Sum(a => a.d_ValorSolesDetraccion)
                                    .Value;
                            var DetraccionDolares =
                                CompraDetalle.Where(a => a.v_IdCompra == x.IdCompra)
                                    .Sum(a => a.d_ValorDolaresDetraccion)
                                    .Value;
                            return new RegistroRetenciones
                            {
                                NroRegistro = x.NroRegistro,
                                FechaEmision = x.FechaEmision,
                                TipoNumeroDocumento = x.TipoNumeroDocumento,
                                RucProveedor = x.RucProveedor,
                                RazonSocialProveedor = x.RazonSocialProveedor,
                                NroDetraccion = x.NroDetraccion,
                                FechaDetraccion = x.FechaDetraccion,
                                ImporteDetraccionSoles = DetraccionSoles != null ? DetraccionSoles : 0,
                                ImporteDetraccionDolares = DetraccionDolares != null ? DetraccionDolares : 0,
                                Moneda = x.Moneda,
                                IdCompra = x.IdCompra
                            };
                        }).ToList().GroupBy(x => new { x.IdCompra })
                        .Select(group => group.Last())
                        .OrderBy(o => o.IdCompra).ToList();

                    return compras.ToList().AsQueryable().OrderBy(Orden).ToList();
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;
            }
        }

        public List<ReporteCompraDaotAnalitico> ReporteProveedorCompraDaotAnalitico(
            ref OperationResult objOperationResult, DateTime fechaInicio, DateTime fechaFin, decimal Tope,
            int TipoCompra, string NroCuenta, string Producto, string Proveedor, string Ordenar)
        {
            try
            {
                objOperationResult.Success = 1;
                var _objDocumentoBL = new DocumentoBL();
                var Compras = new List<ReporteCompraDaotAnalitico>();
                var ListaComprasTope = new List<ReporteCompraDaotAnalitico>();
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    if (NroCuenta != string.Empty || Producto != string.Empty)
                    {
                        var Compritas = (from a in dbContext.compradetalle
                                         join b in dbContext.compra on new { c = a.v_IdCompra, eliminado = 0 } equals
                                             new { c = b.v_IdCompra, eliminado = 0 } into b_join
                                         from b in b_join.DefaultIfEmpty()
                                         join c in dbContext.cliente on new { c = b.v_IdProveedor, eliminado = 0 } equals
                                             new { c = c.v_IdCliente, eliminado = c.i_Eliminado.Value } into c_join
                                         from c in c_join.DefaultIfEmpty()
                                         join d in dbContext.documento on new { d = b.i_IdTipoDocumento.Value, eliminado = 0 } equals
                                             new { d = d.i_CodigoDocumento, eliminado = d.i_Eliminado.Value } into d_join
                                         from d in d_join.DefaultIfEmpty()
                                         join e in dbContext.productodetalle on new { pd = a.v_IdProductoDetalle, eliminado = 0 }
                                             equals new { pd = e.v_IdProductoDetalle, eliminado = 0 } into e_join
                                         from e in e_join.DefaultIfEmpty()
                                         where a.i_Eliminado == 0 && b.i_IdEstado == 1 &&
                                               (b.i_IdTipoCompra == TipoCompra || TipoCompra == -1)
                                               && (c.v_CodCliente.Trim() == Proveedor || Proveedor == string.Empty)
                                               && (e.producto.v_CodInterno == Producto || Producto == string.Empty)
                                               && (a.v_NroCuenta == NroCuenta || NroCuenta == string.Empty) &&
                                               b.t_FechaRegistro >= fechaInicio && b.t_FechaRegistro <= fechaFin &&
                                               b.i_IdTipoDocumento == (int)TiposDocumentos.Factura
                                         select new
                                         {
                                             proveedor =
                                                 c == null
                                                     ? "** CLIENTE NO EXISTE **"
                                                     : "PROVEEDOR : " + c.v_CodCliente.Trim() + " " +
                                                       (c.v_ApePaterno + " " + c.v_ApeMaterno + " " + c.v_PrimerNombre + " " +
                                                        c.v_RazonSocial).Trim(),
                                             documento = d.v_Siglas + " " + b.v_SerieDocumento + " " + b.v_CorrelativoDocumento,
                                             fecha = b.t_FechaRegistro.Value,
                                             FechaEmision = b.t_FechaEmision.Value,
                                             nroRegistro = b.v_Mes.Trim() + " " + b.v_Correlativo,
                                             b.v_IdCompra,
                                             ValorVentaSoles = b.i_IdMoneda == (int)Currency.Soles ? a.d_ValorVenta.Value : a.d_ValorVenta * b.d_TipoCambio,
                                             IgvSoles = b.i_IdMoneda == (int)Currency.Soles ? a.d_Igv : a.d_Igv * b.d_TipoCambio,
                                             TotalSoles = b.i_IdMoneda == (int)Currency.Soles ? a.d_PrecioVenta : a.d_PrecioVenta * b.d_TipoCambio,
                                             ValorVentaDolares = b.i_IdMoneda == (int)Currency.Dolares ? a.d_ValorVenta : a.d_ValorVenta / b.d_TipoCambio,
                                             IgvDolares = b.i_IdMoneda == (int)Currency.Dolares ? a.d_Igv : a.d_Igv / b.d_TipoCambio,
                                             TotalDolares = b.i_IdMoneda == (int)Currency.Dolares ? a.d_PrecioVenta : a.d_PrecioVenta / b.d_TipoCambio,
                                             v_IdProveedor = c.v_IdCliente,
                                             rucProveedor = c.v_NroDocIdentificacion,
                                             IdTipoDocumento = b.i_IdTipoDocumento,
                                             NroDocumentoDeclarado = b.v_SerieDocumento.Trim() + b.v_CorrelativoDocumento.Trim(),


                                         }).ToList().Select(x =>
                                {
                                    return new ReporteCompraDaotAnalitico
                                    {
                                        proveedor = x.proveedor,
                                        documento = x.documento,
                                        dFecha = x.FechaEmision,
                                        fecha =
                                            x.fecha.Date.Day.ToString("00") + "/" + x.fecha.Date.Month.ToString("00") + "/" +
                                            x.fecha.Date.Year.ToString(),
                                        nroRegistro = x.nroRegistro,
                                        ValorVentaSoles =
                                            _objDocumentoBL.DocumentoEsInverso(x.IdTipoDocumento.Value)
                                                ? x.ValorVentaSoles.Value * -1
                                                : x.ValorVentaSoles.Value,
                                        IgvSoles =
                                            _objDocumentoBL.DocumentoEsInverso(x.IdTipoDocumento.Value)
                                                ? x.IgvSoles.Value * -1
                                                : x.IgvSoles.Value,
                                        TotalSoles =
                                            _objDocumentoBL.DocumentoEsInverso(x.IdTipoDocumento.Value)
                                                ? x.TotalSoles.Value * -1
                                                : x.TotalSoles.Value,
                                        ValorVentaDolares =
                                            _objDocumentoBL.DocumentoEsInverso(x.IdTipoDocumento.Value)
                                                ? x.ValorVentaDolares.Value * -1
                                                : x.ValorVentaDolares.Value,
                                        IgvDolares =
                                            _objDocumentoBL.DocumentoEsInverso(x.IdTipoDocumento.Value)
                                                ? x.IgvDolares.Value * -1
                                                : x.IgvDolares.Value,
                                        TotalDolares =
                                            _objDocumentoBL.DocumentoEsInverso(x.IdTipoDocumento.Value)
                                                ? x.TotalDolares.Value * -1
                                                : x.TotalDolares.Value,
                                        Grupo = x.proveedor,
                                        v_IdCompra = x.v_IdCompra,
                                        v_IdProveedor = x.v_IdProveedor,
                                        RucProveedor = x.rucProveedor,
                                        //NroDocDeclarado = x.NroDocumentoDeclarado
                                    };
                                }).ToList().AsQueryable();
                        if (!string.IsNullOrEmpty(Ordenar))
                        {
                            Compritas.OrderBy(Ordenar).ToList();

                        }
                        Compras = Compritas.GroupBy(a => new { a.v_IdCompra }).Select(g => new ReporteCompraDaotAnalitico
                        {
                            proveedor = g.FirstOrDefault().proveedor,
                            documento = g.FirstOrDefault().documento,
                            fecha = g.FirstOrDefault().fecha,
                            nroRegistro = g.FirstOrDefault().nroRegistro,
                            ValorVentaSoles = g.Sum(x => x.ValorVentaSoles),
                            IgvSoles = g.Sum(c => c.IgvSoles),
                            TotalSoles = g.Sum(c => c.TotalSoles),
                            ValorVentaDolares = g.Sum(c => c.ValorVentaDolares),
                            IgvDolares = g.Sum(c => c.IgvDolares),
                            TotalDolares = g.Sum(c => c.TotalDolares),
                            v_IdProveedor = g.FirstOrDefault().v_IdProveedor,
                            //NroDocDeclarado = g.FirstOrDefault().NroDocDeclarado,
                            RucProveedor = g.FirstOrDefault().RucProveedor,
                        }).ToList();
                    }
                    else
                    {
                        var Compritas = (from a in dbContext.compra
                                         join b in dbContext.cliente on new { c = a.v_IdProveedor, eliminado = 0 } equals
                                             new { c = b.v_IdCliente, eliminado = b.i_Eliminado.Value } into b_join
                                         from b in b_join.DefaultIfEmpty()
                                         join c in dbContext.documento on new { d = a.i_IdTipoDocumento.Value, eliminado = 0 } equals
                                             new { d = c.i_CodigoDocumento, eliminado = c.i_Eliminado.Value } into c_join
                                         from c in c_join.DefaultIfEmpty()
                                         where a.i_Eliminado == 0 && a.i_IdEstado == 1 &&
                                               (a.i_IdTipoCompra == TipoCompra || TipoCompra == -1)
                                               && (b.v_CodCliente.Trim() == Proveedor || Proveedor == string.Empty) &&
                                               a.t_FechaRegistro >= fechaInicio && a.t_FechaRegistro <= fechaFin
                                         select new
                                         {
                                             proveedor =
                                                 b == null
                                                     ? "** PROVEEDOR NO EXISTE **"
                                                     : "PROVEEDOR : " + b.v_CodCliente.Trim() + " " +
                                                       (b.v_ApePaterno + " " + b.v_ApeMaterno + " " + b.v_PrimerNombre + " " +
                                                        b.v_RazonSocial).Trim(),
                                             documento = c.v_Siglas + " " + a.v_SerieDocumento + " " + a.v_CorrelativoDocumento,
                                             fecha = a.t_FechaRegistro.Value,

                                             nroRegistro = a.v_Mes.Trim() + " " + a.v_Correlativo,
                                             ValorVentaSoles =
                                                 a.i_IdMoneda == (int)Currency.Soles
                                                     ? a.d_ValorVenta.Value
                                                     : a.d_ValorVenta * a.d_TipoCambio,
                                             IgvSoles =
                                                 a.i_IdMoneda == (int)Currency.Soles ? a.d_IGV : a.d_IGV * a.d_TipoCambio,
                                             TotalSoles =
                                                 a.i_IdMoneda == (int)Currency.Soles ? a.d_Total : a.d_Total * a.d_TipoCambio,
                                             ValorVentaDolares =
                                                 a.i_IdMoneda == (int)Currency.Dolares
                                                     ? a.d_ValorVenta
                                                     : a.d_ValorVenta / a.d_TipoCambio,
                                             IgvDolares = a.i_IdMoneda == (int)Currency.Dolares ? a.d_IGV : a.d_IGV / a.d_TipoCambio,
                                             TotalDolares =
                                                 a.i_IdMoneda == (int)Currency.Dolares ? a.d_Total : a.d_Total / a.d_TipoCambio,
                                             v_IdProveedor = b.v_IdCliente,
                                             RucProveedor = b == null ? "" : b.v_NroDocIdentificacion,
                                             IdTipoDocumento = a.i_IdTipoDocumento,
                                             FechaEmision = a.t_FechaEmision.Value,

                                             TipoPersonaProveedor = b.i_IdTipoPersona,
                                             TipoDocumentoProveedor = b.i_IdTipoIdentificacion.Value,

                                             PrimerNombre = b.v_PrimerNombre,
                                             SegundoNombre = b.v_SegundoNombre,
                                             ApePaterno = b.v_ApePaterno,
                                             ApeMaterno = b.v_ApeMaterno,
                                             RazonSocial = b.v_RazonSocial,

                                         }).ToList().Select(x => new ReporteCompraDaotAnalitico
                              {
                                  proveedor = x.proveedor,
                                  documento = x.documento,
                                  fecha =
                                      x.fecha.Date.Day.ToString("00") + "/" + x.fecha.Date.Month.ToString("00") + "/" +
                                      x.fecha.Date.Year.ToString(),
                                  nroRegistro = x.nroRegistro,
                                  ValorVentaSoles =
                                      _objDocumentoBL.DocumentoEsInverso(x.IdTipoDocumento.Value)
                                          ? x.ValorVentaSoles.Value * -1
                                          : x.ValorVentaSoles.Value,
                                  IgvSoles =
                                      _objDocumentoBL.DocumentoEsInverso(x.IdTipoDocumento.Value)
                                          ? x.IgvSoles.Value * -1
                                          : x.IgvSoles.Value,
                                  TotalSoles =
                                      _objDocumentoBL.DocumentoEsInverso(x.IdTipoDocumento.Value)
                                          ? x.TotalSoles.Value * -1
                                          : x.TotalSoles.Value,
                                  ValorVentaDolares =
                                      _objDocumentoBL.DocumentoEsInverso(x.IdTipoDocumento.Value)
                                          ? x.ValorVentaDolares.Value * -1
                                          : x.ValorVentaDolares.Value,
                                  IgvDolares =
                                      _objDocumentoBL.DocumentoEsInverso(x.IdTipoDocumento.Value)
                                          ? x.IgvDolares.Value * -1
                                          : x.IgvDolares.Value,
                                  TotalDolares =
                                      _objDocumentoBL.DocumentoEsInverso(x.IdTipoDocumento.Value)
                                          ? x.TotalDolares.Value * -1
                                          : x.TotalDolares.Value,
                                  Grupo = x.proveedor,
                                  v_IdProveedor = x.v_IdProveedor,
                                  RucProveedor = x.RucProveedor,
                                  TipoPersona = x.TipoPersonaProveedor.Value,
                                  TipoDocumentoProveedor = x.TipoDocumentoProveedor,
                                  ApeMaterno = x.ApeMaterno,
                                  ApePaterno = x.ApePaterno,
                                  PrimerNombre = x.PrimerNombre,
                                  SegundoNombre = x.SegundoNombre,
                                  RazonSocial = x.RazonSocial,

                                  dFecha = x.FechaEmision,
                              }).ToList().AsQueryable();

                        var FFF = Compritas.Where(o => o.proveedor.Contains("GAEDU"));
                        if (!string.IsNullOrEmpty(Ordenar))
                        {
                            Compritas = Compritas.OrderBy(Ordenar);
                        }


                        Compras = Compritas.ToList();
                    }

                    if (Tope > 0)
                    {
                        var ComprasProveedorMayorTope =
                            Compras.GroupBy(x => x.v_IdProveedor).Select(g => new ReporteCompraDaotAnalitico
                            {
                                ValorVentaSoles = g.Sum(x => x.ValorVentaSoles),
                                v_IdProveedor = g.FirstOrDefault().v_IdProveedor,
                                proveedor = g.FirstOrDefault().proveedor
                            }).ToList().Where(x => x.ValorVentaSoles >= Tope).ToList();

                        foreach (var item in ComprasProveedorMayorTope)
                        {
                            var objReporte = new List<ReporteCompraDaotAnalitico>();
                            objReporte = Compras.Where(x => x.v_IdProveedor == item.v_IdProveedor).ToList();
                            ListaComprasTope.AddRange(objReporte);
                        }
                        return ListaComprasTope;
                    }
                    return Compras;
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;
            }
        }

        public List<ReporteCompraDaotAnalitico> ReporteProveedorCompraDaotResumen(
            ref OperationResult objOperationResult, DateTime fechaInicio, DateTime fechaFin, decimal Tope,
            int TipoCompra, string NroCuenta, string Producto, string Proveedor, string Ordenar)
        {
            var ListaAnalitico = ReporteProveedorCompraDaotAnalitico(ref objOperationResult, fechaInicio, fechaFin, Tope,
                TipoCompra, NroCuenta, Producto, Proveedor, Ordenar);

            if (objOperationResult.Success == 1)
            {
                ListaAnalitico =
                    ListaAnalitico.GroupBy(x => new { x.v_IdProveedor }).Select(x => new ReporteCompraDaotAnalitico
                    {
                        proveedor = x.FirstOrDefault().proveedor,
                        ValorVentaSoles = x.Sum(c => c.ValorVentaSoles),
                        IgvSoles = x.Sum(c => c.IgvSoles),
                        TotalSoles = x.Sum(c => c.TotalSoles),
                        ValorVentaDolares = x.Sum(c => c.ValorVentaDolares),
                        IgvDolares = x.Sum(c => c.IgvDolares),
                        TotalDolares = x.Sum(c => c.TotalDolares),
                        RucProveedor = x.FirstOrDefault().RucProveedor,
                        ApePaterno = x.FirstOrDefault().ApePaterno,
                        ApeMaterno = x.FirstOrDefault().ApeMaterno,
                        PrimerNombre = x.FirstOrDefault().PrimerNombre,
                        SegundoNombre = x.FirstOrDefault().SegundoNombre,
                        RazonSocial = x.FirstOrDefault().RazonSocial,
                        TipoDocumentoProveedor = x.FirstOrDefault().TipoDocumentoProveedor,
                        TipoPersona = x.FirstOrDefault().TipoPersona,

                    }).ToList();
                if (!string.IsNullOrEmpty(Ordenar))
                {
                    ListaAnalitico = ListaAnalitico.AsQueryable().OrderBy(Ordenar).ToList();

                }


                return Tope > 0
                    ? ListaAnalitico.Where(x => x.ValorVentaSoles >= Tope).ToList()
                    : ListaAnalitico.ToList();
            }
            objOperationResult.Success = 0;
            return null;
        }





















        #endregion

        #region KeyValuesDTOs

        public List<GridKeyValueDTO> ObtenRubrosParaComboGridCompra(ref OperationResult pobjOperationResult,
            string pstrSortExpression)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var consulta =
                        dbContext.linea.Where(p => p.i_Eliminado == 0 && !string.IsNullOrEmpty(p.v_NroCuentaCompra))
                            .ToList()
                            .Select(l => new GridKeyValueDTO
                            {
                                Id = l.v_NroCuentaCompra,
                                Value1 = l.v_Nombre
                            }).ToList();

                    return consulta.Any() ? consulta : new List<GridKeyValueDTO>();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public ordendecompraDto ObtenerOrdenDeCompraEnKeyValuesDTO(ref OperationResult pobjOperationResult,
            string pstrSerieODC, string pstrCorrelativoODC)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var Cabecera = (from n in dbContext.ordendecompra
                                    join J1 in dbContext.cliente on n.v_IdProveedor equals J1.v_IdCliente into J1_join
                                    from J1 in J1_join.DefaultIfEmpty()
                                    where n.v_SerieDocumento == pstrSerieODC && n.v_CorrelativoDocumento == pstrCorrelativoODC
                                    select new ordendecompraDto
                                    {
                                        RazonSocialProveedor =
                                            (J1.v_ApePaterno + " " + J1.v_ApeMaterno + " " + J1.v_PrimerNombre + " " +
                                             J1.v_RazonSocial).Trim(),
                                        CodProveedor = J1.v_CodCliente,
                                        RUCProveedor = J1.v_NroDocIdentificacion,
                                        i_PreciosIncluyeIgv = n.i_PreciosIncluyeIgv,
                                        i_PreciosAfectosIgv = n.i_PreciosAfectosIgv,
                                        i_IdMoneda = n.i_IdMoneda,
                                        i_IdEstado = n.i_IdEstado,
                                        v_IdOrdenCompra = n.v_IdOrdenCompra,
                                        v_IdProveedor = n.v_IdProveedor,
                                        v_DocumentoInterno = n.v_DocumentoInterno
                                    }).FirstOrDefault();

                    if (Cabecera != null)
                    {
                        switch ((OrdenCompraEstados)Cabecera.i_IdEstado)
                        {
                            case OrdenCompraEstados.Cancelado:
                                pobjOperationResult.Success = (int)OrdenCompraEstados.Cancelado;
                                return null;

                            case OrdenCompraEstados.Procesado:
                                pobjOperationResult.Success = (int)OrdenCompraEstados.Procesado;
                                return null;

                            case OrdenCompraEstados.Terminado:
                                pobjOperationResult.Success = (int)OrdenCompraEstados.Terminado;
                                return null;

                            default:
                                pobjOperationResult.Success = (int)OrdenCompraEstados.Pendiente;
                                return Cabecera;
                        }
                    }
                    pobjOperationResult.Success = (int)OrdenCompraEstados.NoEncontrada;
                    return null;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = (int)OrdenCompraEstados.Error;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                pobjOperationResult.AdditionalInformation = "ComprasBL.ObtenerOrdenDeCompraEnKeyValuesDTO()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                return null;
            }
        }

        public List<KeyValueDetalleCompraVentaDTO> ObtenerOrdenDeCompraDetallesEnKeyValuesDTO(
            ref OperationResult pobjOperationResult, string pstrIdOrdenCompra)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var ListaDetalles = (from n in dbContext.ordendecompradetalle
                                         join J1 in dbContext.productodetalle on n.v_IdProductoDetalle equals J1.v_IdProductoDetalle into
                                             J1_join
                                         from J1 in J1_join.DefaultIfEmpty()
                                         join J2 in dbContext.producto on J1.v_IdProducto equals J2.v_IdProducto into J2_join
                                         from J2 in J2_join.DefaultIfEmpty()
                                         join J3 in dbContext.datahierarchy on new { ItemId = J2.i_IdUnidadMedida.Value, Grupo = 17 }
                                             equals new { ItemId = J3.i_ItemId, Grupo = J3.i_GroupId } into J3_join
                                         from J3 in J3_join.DefaultIfEmpty()
                                         where n.v_IdOrdenCompra == pstrIdOrdenCompra && n.i_UsadoEnCompra == 0
                                         select new KeyValueDetalleCompraVentaDTO
                                         {
                                             v_IdProductoDetalle = n.v_IdProductoDetalle,
                                             v_CodInterno = J2.v_CodInterno,
                                             v_Descripcion = J2.v_Descripcion,
                                             i_IdUnidadMedida = n.i_IdUnidadMedida,
                                             d_Empaque = J2.d_Empaque,
                                             d_Cantidad = n.d_Cantidad,
                                             d_PrecioUnitario = n.d_PrecioUnitario,
                                             EmpaqueUM = J3.v_Value1,
                                             i_EsServicio = J2.i_EsServicio
                                         }).ToList();
                    pobjOperationResult.Success = 1;
                    return ListaDetalles;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                pobjOperationResult.AdditionalInformation =
                    "ComprasBL.ObtenerOrdenDeCompraDetallesEnKeyValuesDTO()\nLinea:" +
                    ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                return null;
            }
        }

        #endregion

        /// <summary>
        ///     Se realizo este proceso porque jhonatan en chayna encontro que sus correlativos no cuadraban con los declarados
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="listaCompras"></param>
        public static void RegularRegistrosCompras(ref OperationResult pobjOperationResult, List<compraDto> listaCompras)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var compras = dbContext.compra.Where(c => c.i_Eliminado == 0);
                    foreach (var compraComparable in listaCompras)
                    {
                        var compraEditar = compras.FirstOrDefault(p => p.v_IdCompra.Equals(compraComparable.v_IdCompra));
                        if (compraEditar == null) continue;
                        var Almacenes = compraEditar.compradetalle.Select(p => p.i_IdAlmacen).Distinct().ToList();
                        foreach (var Almacen in Almacenes)
                        {
                            var movimientoRelacionado =
                                new MovimientoBL().ObtenerMovimientoCabeceraDesdeCompras(ref pobjOperationResult,
                                    Almacen ?? 1, "C", compraEditar.v_Periodo, compraEditar.v_Mes.Trim(),
                                    compraEditar.v_Correlativo.Trim());
                            if (movimientoRelacionado != null && pobjOperationResult.Success == 1)
                            {
                                movimientoRelacionado.v_OrigenRegCorrelativo = compraComparable.v_Correlativo;
                                movimientoRelacionado.v_Mes = compraComparable.v_Mes;
                                var movimientoOriginal =
                                    dbContext.movimiento.Single(
                                        p => p.v_IdMovimiento.Equals(movimientoRelacionado.v_IdMovimiento));
                                dbContext.movimiento.ApplyCurrentValues(movimientoRelacionado.ToEntity());
                            }
                        }

                        compraEditar.v_Mes = compraComparable.v_Mes;
                        compraEditar.v_Correlativo = compraComparable.v_Correlativo;
                        dbContext.compra.ApplyCurrentValues(compraEditar);
                    }

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "CompraBL.RegularRegistrosCompras()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static void RegularRegistrosComprasRollavel(ref OperationResult pobjOperationResult,
            List<compraDto> listaCompras)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var clientesDs = dbContext.cliente.Where(p => p.i_Eliminado == 0).ToList();
                        foreach (var compraToCompare in listaCompras)
                        {
                            var mes = compraToCompare.NroRegistro.Substring(0, 2);
                            var correlativo = int.Parse(compraToCompare.NroRegistro.Substring(2, 6)).ToString("00000000");

                            var serieCompra = compraToCompare.v_SerieDocumento.Trim();
                            var tipoDocCompra = compraToCompare.i_IdTipoDocumento ?? -1;
                            var correlativoCompra = int.Parse(compraToCompare.v_CorrelativoDocumento).ToString("00000000");
                            var proveedor = clientesDs.FirstOrDefault(p => p.v_CodCliente.Trim().Equals(compraToCompare.CodigoProveedor.Trim()));

                            if (proveedor == null) continue;
                            var compraToModify =
                                dbContext.compra.FirstOrDefault(
                                    p => p.v_SerieDocumento.Equals(serieCompra) && p.i_IdTipoDocumento == tipoDocCompra
                                         && p.v_CorrelativoDocumento.Equals(correlativoCompra) &&
                                         p.v_IdProveedor.Equals(proveedor.v_IdCliente) && p.i_Eliminado == 0);

                            if (compraToModify == null) continue;
                            compraToModify.v_Mes = mes;
                            compraToModify.v_Correlativo = correlativo;
                            dbContext.compra.ApplyCurrentValues(compraToModify);
                        }

                        dbContext.SaveChanges();
                        ts.Complete();
                        pobjOperationResult.Success = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "CompraBL.RegularRegistrosComprasRollavel()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        /// <summary>
        ///     Devuelve el asiento por el id de la compra seleccionada.
        /// </summary>
        /// <param name="pstrIdCompra"></param>
        /// <returns></returns>
        public static string DevolverAsientoContable(string pstrIdCompra)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var asientoRef =
                        dbContext.diario.FirstOrDefault(
                            p => p.v_IdDocumentoReferencia.Equals(pstrIdCompra) && p.i_Eliminado == 0);
                    return asientoRef != null ? asientoRef.v_IdDiario : null;
                }
            }
            catch
            {
                return null;
            }
        }

        public static void ModificarNroRegistros(ref OperationResult pobjOperationResult,
            IDictionary<string, string> lista)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    foreach (var item in lista)
                    {
                        var compra = dbContext.compra.FirstOrDefault(p => p.v_IdCompra.Equals(item.Value));
                        if (compra == null) continue;
                        var diario = dbContext.diario.FirstOrDefault(p => p.v_IdDocumentoReferencia.Equals(compra.v_IdCompra));
                        var reg = item.Key.Split('-')[1];
                        compra.v_Correlativo = reg.Trim();
                        dbContext.compra.ApplyCurrentValues(compra);

                        if (diario == null) continue;
                        diario.v_Correlativo = reg.Trim();
                        dbContext.diario.ApplyCurrentValues(diario);
                    }

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "CompraBL.ModificarNroRegistros()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        #region Asiento Contable
        public void GenerarAsientoContable(ref OperationResult pobjOperationResult, string pstridCompra)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var pobjDtoEntity = dbContext.compra.FirstOrDefault(p => p.v_IdCompra.Equals(pstridCompra));
                    if (pobjDtoEntity == null) return;
                    var clienteDto =
                        dbContext.cliente.FirstOrDefault(p => p.v_IdCliente.Equals(pobjDtoEntity.v_IdProveedor)).ToDTO();


                    if (_objDocumentoBL.DocumentoEsContable(pobjDtoEntity.i_IdTipoDocumento ?? 1) &&
                        pobjDtoEntity.i_IdEstado == 1)
                    {
                        #region Genera Libro Diario

                        var IDConcepto = pobjDtoEntity.i_IdTipoCompra.Value.ToString("00");

                        var aa = (from a in dbContext.administracionconceptos
                                  where a.v_Codigo == IDConcepto && a.i_Eliminado == 0 && a.v_Periodo.Equals(periodo)
                                  select new { a.v_CuentaIGV, a.v_CuentaPVenta, a.v_CuentaDetraccion }).FirstOrDefault();

                        if (aa != null && aa.v_CuentaIGV.Trim() != string.Empty &&
                            aa.v_CuentaPVenta.Trim() != string.Empty)
                        {
                            var _objDiarioBL = new DiarioBL();
                            var _diarioDto = new diarioDto();
                            var _ListadoDiarios = new List<KeyValueDTO>();
                            var TempXInsertar = new List<diariodetalleDto>();

                            var DetalleCompra = (dbContext.compradetalle.Where(
                                d => d.v_IdCompra == pobjDtoEntity.v_IdCompra && d.i_Eliminado == 0)).ToDTOs();

                            DetalleCompra.ForEach(d =>
                            {
                                d.tipoCambio_ = pobjDtoEntity.d_TipoCambio.Value;
                                d.incluyeIgv_ = pobjDtoEntity.i_PreciosIncluyenIgv == 1;
                                d.idMoneda_ = pobjDtoEntity.i_IdMoneda.Value;
                            });

                            var H_IGV = new diariodetalleDto();
                            var D_PrecioVenta = new diariodetalleDto();
                            var H_Percepcion = new diariodetalleDto();
                            var _Detraccion = new diariodetalleDto();
                            var montoPercepcion = pobjDtoEntity.d_Percepcion ?? 0m;

                            _diarioDto.v_Periodo = pobjDtoEntity.v_Periodo;
                            _diarioDto.t_FechaVencimiento = pobjDtoEntity.t_FechaVencimiento;
                            _diarioDto.t_FechaEmision = pobjDtoEntity.t_FechaEmision;
                            _diarioDto.v_Mes = pobjDtoEntity.v_Mes;
                            _diarioDto.v_Correlativo = pobjDtoEntity.v_Correlativo;
                            _diarioDto.v_IdDocumentoReferencia = pobjDtoEntity.v_IdCompra;
                            _diarioDto.v_Nombre = clienteDto.NombreRazonSocial;
                            _diarioDto.v_Glosa = pobjDtoEntity.v_Glosa;
                            _diarioDto.d_TipoCambio = pobjDtoEntity.d_TipoCambio.Value;
                            _diarioDto.i_IdMoneda = pobjDtoEntity.i_IdMoneda.Value;
                            _diarioDto.i_IdTipoDocumento = 336; // D/V = Diario de venta
                            _diarioDto.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                            _diarioDto.i_IdTipoComprobante = 2;

                            #region SubTotalVenta

                            var agrupado = DetalleCompra.GroupBy(g => new { cuenta = g.v_NroCuenta, cc = g.i_IdCentroCostos, anticipo = g.i_Anticipio, analisis = g.v_Glosa, anexo = g.v_IdAnexo });

                            foreach (var detalle in agrupado)
                            {
                                var H_SubTotalVenta = new diariodetalleDto();
                                var subTotal = detalle.Sum(o => o.d_ValorVenta ?? 0);
                                var subTotalCambio = detalle.Sum(o => o.d_ValorVenta_);
                                H_SubTotalVenta.d_Importe = subTotal > 0 ? subTotal : subTotal * -1;
                                H_SubTotalVenta.d_Cambio = subTotalCambio;
                                H_SubTotalVenta.i_IdCentroCostos = detalle.Key.cc;
                                H_SubTotalVenta.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento ?? -1;
                                H_SubTotalVenta.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                H_SubTotalVenta.v_IdCliente = string.IsNullOrEmpty(detalle.Key.anexo) ? pobjDtoEntity.v_IdProveedor : detalle.Key.anexo;
                                H_SubTotalVenta.v_Naturaleza = subTotal > 0
                                    ? !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento ?? -1)
                                        ? "D"
                                        : "H"
                                    : !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento ?? -1)
                                        ? "H"
                                        : "D";
                                if (detalle.Key.anticipo == 1)
                                {
                                    H_SubTotalVenta.v_Naturaleza = H_SubTotalVenta.v_Naturaleza == "D" ? "H" : "D";
                                }
                                H_SubTotalVenta.v_NroDocumento = pobjDtoEntity.v_SerieDocumento.Trim() + "-" +
                                                                 pobjDtoEntity.v_CorrelativoDocumento.Trim();
                                H_SubTotalVenta.v_NroCuenta = detalle.Key.cuenta;

                                if (_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento ?? -1) ||
                                    (pobjDtoEntity.i_IdTipoDocumento ?? -1).ToString() == "8")
                                {
                                    H_SubTotalVenta.i_IdTipoDocumentoRef = pobjDtoEntity.i_IdTipoDocumentoRef ?? -1;
                                    H_SubTotalVenta.v_NroDocumentoRef = pobjDtoEntity.v_SerieDocumentoRef + "-" +
                                                                        pobjDtoEntity.v_CorrelativoDocumentoRef;
                                }

                                H_SubTotalVenta.v_Analisis = detalle.Key.analisis;

                                TempXInsertar.Add(H_SubTotalVenta);
                            }

                            #endregion

                            #region IGV

                            var importe = DetalleCompra.Where(o => o.i_Anticipio != 1).Sum(p => p.d_Igv ?? 0) -
                                          DetalleCompra.Where(o => o.i_Anticipio == 1).Sum(p => p.d_Igv ?? 0);

                            var cambio = DetalleCompra.Where(o => o.i_Anticipio != 1).Sum(p => p.d_Igv_) -
                                          DetalleCompra.Where(o => o.i_Anticipio == 1).Sum(p => p.d_Igv_);

                            H_IGV.d_Importe = importe;
                            H_IGV.d_Cambio = cambio;
                            H_IGV.i_IdCentroCostos = "0";
                            H_IGV.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento ?? -1;
                            H_IGV.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                            H_IGV.v_IdCliente = pobjDtoEntity.v_IdProveedor;
                            H_IGV.v_Naturaleza =
                                !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento ?? -1) ? "D" : "H";
                            H_IGV.v_NroDocumento = pobjDtoEntity.v_SerieDocumento.Trim() + "-" +
                                                   pobjDtoEntity.v_CorrelativoDocumento.Trim();
                            H_IGV.v_NroCuenta = (from a in dbContext.administracionconceptos
                                                 where a.v_Codigo == IDConcepto && a.i_Eliminado == 0 && a.v_Periodo.Equals(periodo)
                                                 select new { a.v_CuentaIGV }).First().v_CuentaIGV;
                            if (_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento ?? 0) ||
                                pobjDtoEntity.i_IdTipoDocumento.Value.ToString() == "8")
                            {
                                H_IGV.i_IdTipoDocumentoRef = pobjDtoEntity.i_IdTipoDocumentoRef ?? -1;
                                H_IGV.v_NroDocumentoRef = pobjDtoEntity.v_SerieDocumentoRef + "-" +
                                                          pobjDtoEntity.v_CorrelativoDocumentoRef;
                            }
                            TempXInsertar.Add(H_IGV);

                            #endregion

                            #region Percepcion

                            if (montoPercepcion > 0)
                            {
                                var docPercepcion = dbContext.documento.FirstOrDefault(p => p.i_CodigoDocumento == 40);
                                if (docPercepcion == null)
                                    throw new Exception("No está registrado el documento de percepciones (40)");

                                var ctaPercepciones = docPercepcion.v_NroCuenta;

                                if (string.IsNullOrWhiteSpace(ctaPercepciones))
                                    throw new Exception("No está especificada la cuenta de percepciones en el documento 40.");

                                var D_SubTotalVenta = new diariodetalleDto();
                                var subTotal = montoPercepcion;

                                D_SubTotalVenta.d_Importe = pobjDtoEntity.i_IdMoneda.Value == 1 ? subTotal : subTotal / (pobjDtoEntity.d_TipoCambio ?? 1);
                                D_SubTotalVenta.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1
                                    ? Utils.Windows.DevuelveValorRedondeado(subTotal / pobjDtoEntity.d_TipoCambio.Value, 2)
                                    : subTotal;
                                D_SubTotalVenta.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumentoPercepcion ?? -1;
                                D_SubTotalVenta.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                D_SubTotalVenta.v_IdCliente = pobjDtoEntity.v_IdProveedor;
                                D_SubTotalVenta.v_Naturaleza = subTotal > 0
                                    ? !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento ?? -1)
                                        ? "D"
                                        : "H"
                                    : !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento ?? -1)
                                        ? "H"
                                        : "D";

                                D_SubTotalVenta.v_Naturaleza = "D";

                                D_SubTotalVenta.v_NroDocumento = pobjDtoEntity.v_SeriePercepcion.Trim() + "-" +
                                                                 pobjDtoEntity.v_CorrelativoPercepcion;
                                D_SubTotalVenta.v_NroCuenta = ctaPercepciones;

                                TempXInsertar.Add(D_SubTotalVenta);
                            }
                            #endregion

                            #region PrecioVenta


                            decimal montoDetraccion = 0;
                            var ctaCompra42 = (from a in dbContext.administracionconceptos
                                               where a.v_Codigo == IDConcepto && a.i_Eliminado == 0 && a.v_Periodo.Equals(periodo)
                                               select new { a.v_CuentaPVenta }).First().v_CuentaPVenta;
                            var nroCuentaDetraccion = aa.v_CuentaDetraccion ?? string.Empty;

                            if (DetalleCompra.Sum(p => p.d_ValorSolesDetraccion) > 0)
                            {
                                if (string.IsNullOrEmpty(nroCuentaDetraccion))
                                    throw new Exception(
                                        "No existe cuenta de detracción especificada en administracion de conceptos.!");
                                montoDetraccion = pobjDtoEntity.i_IdMoneda.Value == 1
                                   ? DetalleCompra.Sum(p => p.d_ValorSolesDetraccion ?? 0)
                                   : DetalleCompra.Sum(p => p.d_ValorDolaresDetraccion ?? 0);

                                if (ctaCompra42 != nroCuentaDetraccion)
                                {
                                    _Detraccion.d_Importe = Utils.Windows.DevuelveValorRedondeado(montoDetraccion, 2);
                                    _Detraccion.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1
                                        ? Utils.Windows.DevuelveValorRedondeado(
                                            _Detraccion.d_Importe.Value / pobjDtoEntity.d_TipoCambio.Value, 2)
                                        : Utils.Windows.DevuelveValorRedondeado(
                                            _Detraccion.d_Importe.Value * pobjDtoEntity.d_TipoCambio.Value, 2);
                                    _Detraccion.i_IdCentroCostos = "0";
                                    _Detraccion.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                                    _Detraccion.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                    _Detraccion.v_IdCliente = pobjDtoEntity.v_IdProveedor;
                                    _Detraccion.v_Naturaleza =
                                        !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value)
                                            ? "H"
                                            : "D";
                                    _Detraccion.v_NroDocumento = pobjDtoEntity.v_SerieDocumento.Trim() + "-" +
                                                                 pobjDtoEntity.v_CorrelativoDocumento;
                                    _Detraccion.v_NroCuenta = nroCuentaDetraccion;
                                    if (_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ||
                                        pobjDtoEntity.i_IdTipoDocumento.Value.ToString() == "8")
                                    {
                                        _Detraccion.i_IdTipoDocumentoRef = pobjDtoEntity.i_IdTipoDocumentoRef.Value;
                                        _Detraccion.v_NroDocumentoRef = pobjDtoEntity.v_SerieDocumentoRef + "-" +
                                                                        pobjDtoEntity.v_CorrelativoDocumentoRef;
                                    }
                                    TempXInsertar.Add(_Detraccion);
                                }
                            }

                            montoPercepcion = pobjDtoEntity.i_IdMoneda == 2
                                ? Utils.Windows.DevuelveValorRedondeado((montoPercepcion / pobjDtoEntity.d_TipoCambio.Value), 2)
                                : montoPercepcion;

                            var montoAnticipio = DetalleCompra.Where(p => p.i_Anticipio == 1).Sum(p => p.d_PrecioVenta ?? 0);

                            D_PrecioVenta.d_Importe =
                                decimal.Parse(Math.Round(DetalleCompra.Where(o => o.i_Anticipio != 1).Sum(p => p.d_PrecioVenta ?? 0) -
                                        montoAnticipio - montoDetraccion + montoPercepcion + (H_Percepcion.d_Importe ?? 0), 2,
                                        MidpointRounding.AwayFromZero).ToString("0.00"));

                            if (ctaCompra42 == nroCuentaDetraccion) D_PrecioVenta.d_Importe += montoDetraccion;

                            //D_PrecioVenta.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1
                            //    ? Utils.Windows.DevuelveValorRedondeado(
                            //        D_PrecioVenta.d_Importe.Value / pobjDtoEntity.d_TipoCambio.Value, 2)
                            //    : Utils.Windows.DevuelveValorRedondeado(
                            //        D_PrecioVenta.d_Importe.Value * pobjDtoEntity.d_TipoCambio.Value, 2);

                            D_PrecioVenta.d_Cambio = TempXInsertar.Sum(p => p.d_Cambio ?? 0);

                            D_PrecioVenta.i_IdCentroCostos = "0";
                            D_PrecioVenta.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                            D_PrecioVenta.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                            D_PrecioVenta.v_IdCliente = pobjDtoEntity.v_IdProveedor;
                            D_PrecioVenta.v_Naturaleza =
                                !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value)
                                    ? "H"
                                    : "D";
                            D_PrecioVenta.v_NroDocumento = pobjDtoEntity.v_SerieDocumento + "-" +
                                                           pobjDtoEntity.v_CorrelativoDocumento;
                            D_PrecioVenta.v_NroCuenta = ctaCompra42;
                            if (_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ||
                                pobjDtoEntity.i_IdTipoDocumento.Value.ToString() == "8")
                            {
                                D_PrecioVenta.i_IdTipoDocumentoRef = pobjDtoEntity.i_IdTipoDocumentoRef.Value;
                                D_PrecioVenta.v_NroDocumentoRef = pobjDtoEntity.v_SerieDocumentoRef + "-" +
                                                                  pobjDtoEntity.v_CorrelativoDocumentoRef;
                            }
                            TempXInsertar.Add(D_PrecioVenta);

                            #endregion

                            decimal TotDebe, TotHaber, TotDebeC, TotHaberC;
                            TotDebe =
                                TempXInsertar.Where(p => p.v_Naturaleza == "D" && p.v_NroCuenta != string.Empty)
                                    .Sum(o => o.d_Importe.Value);
                            TotHaber =
                                TempXInsertar.Where(p => p.v_Naturaleza == "H" && p.v_NroCuenta != string.Empty)
                                    .Sum(o => o.d_Importe.Value);
                            TotDebeC =
                                TempXInsertar.Where(p => p.v_Naturaleza == "D" && p.v_NroCuenta != string.Empty)
                                    .Sum(o => o.d_Cambio.Value);
                            TotHaberC =
                                TempXInsertar.Where(p => p.v_Naturaleza == "H" && p.v_NroCuenta != string.Empty)
                                    .Sum(o => o.d_Cambio.Value);
                            _diarioDto.d_TotalDebe = TotDebe;
                            _diarioDto.d_TotalHaber = TotHaber;
                            _diarioDto.d_TotalDebeCambio = TotDebeC;
                            _diarioDto.d_TotalHaberCambio = TotHaberC;
                            _diarioDto.d_DiferenciaDebe = TotDebe - TotHaber;
                            _diarioDto.d_DiferenciaHaber = TotDebeC - TotHaberC;

                            _objDiarioBL.InsertarDiario(ref pobjOperationResult, _diarioDto,
                                Globals.ClientSession.GetAsList(),
                                TempXInsertar.Where(x => x.v_NroCuenta != string.Empty).ToList(),
                                (int)TipoMovimientoTesoreria.Egreso);
                            if (pobjOperationResult.Success == 0) return;
                        }

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "MovimientoBL.GenerarAsientoContable()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public void EliminarAsientoContable(ref OperationResult pobjOperationResult, string pstridCompra)
        {
            try
            {
                new DiarioBL().EliminarDiarioXDocRef(ref pobjOperationResult, pstridCompra, Globals.ClientSession.GetAsList(), false);
                if (pobjOperationResult.Success == 0) return;
                pobjOperationResult.Success = 1;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "MovimientoBL.EliminarAsientoContable()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public void RegenerarAsientoContable(ref OperationResult pobjOperationResult, string pstridCompra)
        {
            try
            {
                EliminarAsientoContable(ref pobjOperationResult, pstridCompra);
                if (pobjOperationResult.Success == 0) return;
                GenerarAsientoContable(ref pobjOperationResult, pstridCompra);
                if (pobjOperationResult.Success == 0) return;

                pobjOperationResult.Success = 1;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "MovimientoBL.RegenerarAsientoContable()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }
        #endregion

        #region Almacén

        public static void GenerarIngresoAlmacen(ref OperationResult pobjOperationResult, string pstrIdCompra, string Correlativo, string Periodo, string Mes)
        {
            pobjOperationResult.Success = 1;
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        int igv;
                        var compra = dbContext.compra.FirstOrDefault(p => p.v_IdCompra.Equals(pstrIdCompra));
                        if (compra == null) return;
                        var i = compra.i_IdIgv ?? 1;
                        var igvCompra = int.TryParse(dbContext.datahierarchy.FirstOrDefault(
                            p => p.i_GroupId == 27 && p.i_ItemId.Equals(i)).v_Value1, out igv) ? igv : 18;
                        var porIgv = 1 + igvCompra / 100M;

                        EliminarIngresosDeGuiaRemision(ref pobjOperationResult, compra);
                        if (pobjOperationResult.Success == 0) return;

                        #region recopila los detalles de la compra

                        var compraDetalles = (from p in dbContext.compradetalle
                                              join J1 in dbContext.productodetalle on p.v_IdProductoDetalle equals J1.v_IdProductoDetalle
                                                  into J1_join
                                              from J1 in J1_join.DefaultIfEmpty()
                                              join J2 in dbContext.producto on J1.v_IdProducto equals J2.v_IdProducto into J2_join
                                              from J2 in J2_join.DefaultIfEmpty()
                                              where
                                                  p.i_Eliminado == 0 && p.v_IdProductoDetalle != null &&
                                                  !p.v_IdProductoDetalle.Trim().Equals("") && J2.i_EsServicio == 0 && J2.i_EsActivoFijo == 0
                                                  && p.v_IdCompra.Equals(pstrIdCompra)
                                              select p).ToList();

                        #endregion

                        if (compraDetalles.Any())
                        {
                            var listaMovimientos = new MovimientoBL().ObtenerListadoMovimientos(
                                ref pobjOperationResult, compra.v_Periodo, compra.v_Mes,
                                (int)TipoDeMovimiento.NotadeIngreso);
                            var maxMovimiento = listaMovimientos.Any()
                                ? int.Parse(listaMovimientos[listaMovimientos.Count - 1].Value1)
                                : 0;

                            foreach (var detalle in compraDetalles.GroupBy(p => p.i_IdAlmacen))
                            {
                                var movimientoDto = new movimientoDto();
                                maxMovimiento++;
                                movimientoDto.d_TipoCambio = compra.d_TipoCambio;
                                movimientoDto.i_IdAlmacenOrigen = detalle.Key;
                                movimientoDto.i_IdMoneda = compra.i_IdMoneda;
                                movimientoDto.i_IdTipoMotivo = compra.i_IdTipoDocumento.Value == 7 ? 4 : 1;
                                movimientoDto.t_Fecha = compra.t_FechaRegistro;
                                movimientoDto.v_Mes = !string.IsNullOrEmpty(Mes) ? Mes : compra.v_Mes.Trim();
                                movimientoDto.v_Periodo = !string.IsNullOrEmpty(Periodo) ? Periodo : compra.v_Periodo.Trim();
                                movimientoDto.i_IdTipoMovimiento = (int)TipoDeMovimiento.NotadeIngreso;
                                movimientoDto.v_Correlativo = !string.IsNullOrEmpty(Correlativo) ? Correlativo : maxMovimiento.ToString("00000000");
                                movimientoDto.v_IdCliente = compra.v_IdProveedor;
                                movimientoDto.v_OrigenTipo = "C";
                                movimientoDto.i_EsDevolucion = compra.i_IdTipoDocumento.Value == 7 ? 1 : 0;
                                movimientoDto.v_OrigenRegCorrelativo = compra.v_Correlativo;
                                movimientoDto.v_OrigenRegMes = compra.v_Mes;
                                movimientoDto.v_OrigenRegPeriodo = compra.v_Periodo;
                                movimientoDto.d_TotalPrecio = compra.d_Total;
                                movimientoDto.i_IdEstablecimiento = compra.i_IdEstablecimiento ?? 1;
                                movimientoDto.v_IdMovimientoOrigen = compra.v_IdCompra;
                                movimientoDto.v_Glosa = compra.v_Glosa;
                                movimientoDto.v_NroOrdenCompra = compra.v_ODCSerie + "-" + compra.v_ODCCorrelativo;
                                var movimientosDetalleDto = detalle.ToList()
                                    .Select(d => new movimientodetalleDto
                                    {
                                        v_IdProductoDetalle = d.v_IdProductoDetalle,
                                        i_IdTipoDocumento = compra.i_IdTipoDocumento ?? -1,
                                        v_NumeroDocumento =
                                            string.Format("{0}-{1}", compra.v_SerieDocumento,
                                                compra.v_CorrelativoDocumento),
                                        v_NroGuiaRemision = d.v_NroGuiaRemision,
                                        d_Cantidad = d.d_Cantidad ?? 0,
                                        i_IdUnidad = d.i_IdUnidadMedida ?? -1,
                                        d_CantidadEmpaque = d.d_CantidadEmpaque ?? 0,
                                        d_Precio = compra.i_PreciosIncluyenIgv == 1 ? Utils.Windows.DevuelveValorRedondeado(d.d_Precio.Value / porIgv, 6) : d.d_Precio ?? 0,
                                        d_Total = d.d_ValorVenta ?? 0,
                                        d_CantidadAdministrativa = d.d_Cantidad,
                                        d_CantidadEmpaqueAdministrativa = d.d_CantidadEmpaque,
                                        v_NroPedido = d.v_NroPedido,
                                        i_IdCentroCosto = d.i_IdCentroCostos,
                                        t_FechaCaducidad = d.t_FechaCaducidad,
                                        t_FechaFabricacion = d.t_FechaFabricacion,
                                        v_NroSerie = d.v_NroSerie,
                                        v_NroLote = d.v_NroLote,
                                    }).ToList();

                                movimientoDto.d_TotalCantidad = movimientosDetalleDto.Sum(p => p.d_Cantidad ?? 0);
                                movimientoDto.d_TotalPrecio = movimientosDetalleDto.Sum(p => p.d_Total ?? 0);
                                new MovimientoBL().InsertarMovimiento(ref pobjOperationResult, movimientoDto,
                                    Globals.ClientSession.GetAsList(), movimientosDetalleDto);
                                if (pobjOperationResult.Success == 0) return;
                            }
                        }
                    }
                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ComprasBL.GenerarSalidaAlmacen()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }







        private static void EliminarIngresoAlmacen(ref OperationResult pobjOperationResult, string pstrIdCompra)
        {
            pobjOperationResult.Success = 1;
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var movimientosRelacionado =
                            dbContext.movimiento.Where(
                                p => p.v_IdMovimientoOrigen.Equals(pstrIdCompra) && p.i_Eliminado == 0).ToList();

                        if (movimientosRelacionado.Any())
                        {
                            foreach (var movimientoRef in movimientosRelacionado)
                            {
                                new MovimientoBL().EliminarMovimiento(ref pobjOperationResult,
                                    movimientoRef.v_IdMovimiento, Globals.ClientSession.GetAsList());
                                if (pobjOperationResult.Success == 0) return;
                            }
                        }
                        ts.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ComprasBL.EliminarIngresoAlmacen()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        private static void RegenerarIngresoAlmacen(ref OperationResult pobjOperationResult, string pstrIdCompra)
        {
            pobjOperationResult.Success = 1;
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    EliminarIngresoAlmacen(ref pobjOperationResult, pstrIdCompra);
                    if (pobjOperationResult.Success == 0) return;
                    GenerarIngresoAlmacen(ref pobjOperationResult, pstrIdCompra, "", "", "");
                    if (pobjOperationResult.Success == 0) return;
                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ComprasBL.RegenerarIngresoAlmacen()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }









        private static void EliminarIngresoAlmacenyRegenera(ref OperationResult pobjOperationResult, string pstrIdCompra)
        {
            pobjOperationResult.Success = 1;
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var movimientosRelacionado =
                            dbContext.movimiento.Where(
                                p => p.v_IdMovimientoOrigen.Equals(pstrIdCompra) && p.i_Eliminado == 0).ToList();

                        if (movimientosRelacionado.Any())
                        {
                            foreach (var movimientoRef in movimientosRelacionado)
                            {
                                new MovimientoBL().EliminarMovimiento(ref pobjOperationResult,
                                    movimientoRef.v_IdMovimiento, Globals.ClientSession.GetAsList());
                                if (pobjOperationResult.Success == 0) return;

                                GenerarIngresoAlmacen(ref pobjOperationResult, pstrIdCompra, movimientoRef.v_Correlativo.Trim(), movimientoRef.v_Periodo.Trim(), movimientoRef.v_Mes.Trim());
                                if (pobjOperationResult.Success == 0) return;


                            }
                        }
                        ts.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ComprasBL.EliminarIngresoAlmacen()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }










        private static void EliminarIngresosDeGuiaRemision(ref OperationResult pobjOperationResult, compra objCompra)
        {
            pobjOperationResult.Success = 1;
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    if (string.IsNullOrWhiteSpace(objCompra.v_GuiaRemisionCorrelativo)) return;
                    var idProveedor = objCompra.v_IdProveedor;
                    var serieGrm = objCompra.v_GuiaRemisionSerie.Trim();
                    var correlativosGrm = objCompra.v_GuiaRemisionCorrelativo.Contains(',')
                        ? objCompra.v_GuiaRemisionCorrelativo.Split(',').ToList().Select(p => p.Trim())
                        : new List<string> { objCompra.v_GuiaRemisionCorrelativo.Trim() };

                    foreach (var correlativoGrm in correlativosGrm)
                    {
                        var grm =
                            dbContext.guiaremisioncompra.FirstOrDefault(
                                p =>
                                    p.v_IdProveedor.Equals(idProveedor) && p.v_SerieDocumento.Equals(serieGrm) &&
                                    p.v_CorrelativoDocumento.Equals(correlativoGrm) && p.i_Eliminado == 0);

                        if (grm == null) continue;
                        GuiaRemisionCompraBL.EliminarIngresoAlmacen(ref pobjOperationResult, grm.v_IdGuiaCompra);
                        if (pobjOperationResult.Success == 0) return;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ComprasBL.EliminarIngresosDeGuiaRemision()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        #endregion
    }

    /// <summary>
    ///     Proceso que ejecuta un recálculo completo de las notas de Ingresos de las Guias de remision de compras y Compras
    ///     del sistema por periodo.
    /// </summary>
    public class ReprocesarIngresosWorker
    {
        private readonly BackgroundWorker bw = new BackgroundWorker();

        public delegate void OnProcesar(string pstrEstadoProceso);

        public delegate void OnError(OperationResult pobjOperationResult);

        public delegate void OnFinalizado();

        public event OnProcesar OnProcesarEvent;
        public event OnError OnErrorEvent;
        public event OnFinalizado OnFinalizadoEvent;
        private string _periodo;
        public List<string> FiltroProductos { get; set; }

        public bool IsBusy
        {
            get { return bw.IsBusy; }
        }

        /// <summary>
        ///     Indica el periodo que se va a reprocesar.
        /// </summary>
        public string Periodo
        {
            set { _periodo = string.IsNullOrWhiteSpace(value) ? "2016" : value; }
        }

        /// <summary>
        ///     Da inicio al proceso.
        /// </summary>
        public void Comenzar()
        {
            bw.DoWork += bw_DoWork;
            bw.RunWorkerAsync();
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            lock (this)
            {
                var pobjOperationResult = new OperationResult();
                try
                {
                    using (var ts = TransactionUtils.CreateTransactionScope())
                    {
                        using (var dbContext = new SAMBHSEntitiesModelWin())
                        {
                            if (OnProcesarEvent != null)
                                OnProcesarEvent("Recopilando Data de Compras...");

                            var movimientoIngresos =
                                dbContext.movimiento.Where(
                                    p =>
                                        p.v_OrigenTipo != null && (p.v_OrigenTipo.Equals("C") || p.v_OrigenTipo.Equals("G")) &&
                                        p.v_Periodo.Equals(_periodo) && p.i_Eliminado == 0)
                                    .ToList()
                                    .OrderBy(o => o.v_Mes)
                                    .ThenBy(t => t.v_Correlativo);

                            var guiasRemisionPorProcesar =
                                dbContext.guiaremisioncompra.Where(p => p.v_Periodo.Equals(_periodo) && p.i_Eliminado == 0)
                                    .ToList()
                                    .OrderBy(o => o.v_Mes)
                                    .ThenBy(t => t.v_Correlativo);

                            var comprasPorProcesar =
                                dbContext.compra.Where(p => p.v_Periodo.Equals(_periodo) && p.i_Eliminado == 0)
                                    .ToList()
                                    .OrderBy(o => o.v_Mes)
                                    .ThenBy(t => t.v_Correlativo);

                            //if (FiltroProductos != null && FiltroProductos.Any())
                            //{
                            //    if (OnProcesarEvent != null)
                            //        OnProcesarEvent("Filtrando datos...");

                            //    var movimientosFiltrados =
                            //  dbContext.movimientodetalle.Where(m => FiltroProductos.Contains(m.v_IdProductoDetalle) && m.i_Eliminado == 0)
                            //                                  .Select(n => n.v_IdMovimiento).Distinct().ToList();

                            //    var guiasFiltrados =
                            //  dbContext.guiaremisioncompradetalle.Where(m => FiltroProductos.Contains(m.v_IdProductoDetalle) && m.i_Eliminado == 0)
                            //                                  .Select(n => n.v_IdGuiaCompra).Distinct().ToList();

                            //    var comprasFiltrados =
                            //       dbContext.compradetalle.Where(m => FiltroProductos.Contains(m.v_IdProductoDetalle) && m.i_Eliminado == 0)
                            //                                       .Select(n => n.v_IdCompra).Distinct().ToList();         

                            //    movimientoIngresos = movimientoIngresos.Where(m => movimientosFiltrados.Contains(m.v_IdMovimiento)).ToList().OrderBy(o => o.v_Mes)
                            //        .ThenBy(t => t.v_Correlativo);

                            //    guiasRemisionPorProcesar = guiasRemisionPorProcesar.Where(m => guiasFiltrados.Contains(m.v_IdGuiaCompra)).ToList().OrderBy(o => o.v_Mes)
                            //        .ThenBy(t => t.v_Correlativo);

                            //    comprasPorProcesar = comprasPorProcesar.Where(m => comprasFiltrados.Contains(m.v_IdCompra)).ToList().OrderBy(o => o.v_Mes)
                            //        .ThenBy(t => t.v_Correlativo); 
                            //}

                            if (movimientoIngresos.Any())
                            {
                                foreach (var movimientoSalida in movimientoIngresos.AsParallel())
                                {
                                    var nroSalida = string.Format("{0}-{1}", movimientoSalida.v_Mes,
                                        movimientoSalida.v_Correlativo);
                                    if (OnProcesarEvent != null)
                                        OnProcesarEvent(string.Format("Eliminado Ingreso: {0}", nroSalida));

                                    new MovimientoBL().EliminarMovimiento(ref pobjOperationResult,
                                        movimientoSalida.v_IdMovimiento, Globals.ClientSession.GetAsList());
                                    if (pobjOperationResult.Success == 0)
                                        throw new Exception("Error al Eliminar Nota de Ingreso: " +
                                                            movimientoSalida.v_IdMovimiento);
                                }
                            }

                            if (guiasRemisionPorProcesar.Any())
                            {
                                foreach (var guia in guiasRemisionPorProcesar.AsParallel())
                                {
                                    var nroGuia = string.Format("{0}-{1}", guia.v_Mes.Trim(), guia.v_Correlativo);
                                    if (OnProcesarEvent != null)
                                        OnProcesarEvent(string.Format("Generando Ingreso de GRM: {0}", nroGuia));

                                    GuiaRemisionCompraBL.GenerarIngresoAlmacen(ref pobjOperationResult, guia.v_IdGuiaCompra);
                                    if (pobjOperationResult.Success == 0)
                                        throw new Exception("Error al Generar Ingreso de: " + guia.v_IdGuiaCompra);
                                }
                            }

                            if (comprasPorProcesar.Any())
                            {
                                foreach (var compra in comprasPorProcesar.AsParallel())
                                {
                                    var nroVenta = string.Format("{0}-{1}", compra.v_Mes.Trim(), compra.v_Correlativo);
                                    if (OnProcesarEvent != null)
                                        OnProcesarEvent(string.Format("Generando Ingreso de Compra: {0}", nroVenta));

                                    ComprasBL.GenerarIngresoAlmacen(ref pobjOperationResult, compra.v_IdCompra, "", "", "");
                                    if (pobjOperationResult.Success == 0)
                                        throw new Exception("Error al Generar Salida de: " + compra.v_IdCompra);
                                }
                            }
                        }

                        ts.Complete();
                        if (OnFinalizadoEvent != null)
                            OnFinalizadoEvent();
                    }
                }
                catch (Exception ex)
                {
                    pobjOperationResult.Success = 0;
                    pobjOperationResult.AdditionalInformation = "ReprocesarIngresosWorker.Comenzar()";
                    pobjOperationResult.ErrorMessage = ex.Message;
                    pobjOperationResult.ExceptionMessage = ex.InnerException != null
                        ? ex.InnerException.Message
                        : string.Empty;
                    Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                    if (OnErrorEvent != null)
                        OnErrorEvent(pobjOperationResult);
                }
            }

        }

        public void Detener()
        {
            bw.CancelAsync();
        }
    }

    public class RegenerarAsientosCompraWorker
    {
        public delegate void Proceso(int porcentaje);
        public delegate void Error(OperationResult pobjOperationResult);

        public event Proceso ProcesoEvent;
        public event Error ErrorEvent;
        public event EventHandler FinalizadoEvent;

        public void ComenzarAsync(int pintMes, int pintPeriodo)
        {
            var pobjOperationResult = new OperationResult();
            try
            {
                Task.Factory.StartNew(() =>
                {
                    lock (this)
                    {
                        using (var ts = TransactionUtils.CreateTransactionScope())
                        {
                            using (var dbContext = new SAMBHSEntitiesModelWin())
                            {
                                var pobjDtoEntities =
                                    dbContext.compra.Where(
                                        p =>
                                            p.t_FechaEmision.Value.Month == pintMes && p.t_FechaEmision.Value.Year == pintPeriodo &&
                                            p.i_Eliminado == 0).ToDTOs();

                                var total = pobjDtoEntities.Count;
                                var pos = 0;

                                foreach (var pobjDtoEntity in pobjDtoEntities)
                                {
                                    if (new DocumentoBL().DocumentoEsContable(pobjDtoEntity.i_IdTipoDocumento ?? -1) && pobjDtoEntity.i_IdEstado == 1)
                                    {
                                        pos++;
                                        new ComprasBL().RegenerarAsientoContable(ref pobjOperationResult, pobjDtoEntity.v_IdCompra);
                                        if (pobjOperationResult.Success == 0) return;

                                        if (ProcesoEvent != null)
                                            ProcesoEvent((pos * 100) / total);
                                    }
                                }

                                ts.Complete();
                                pobjOperationResult.Success = 1;

                                if (ProcesoEvent != null)
                                    ProcesoEvent(100);

                                if (FinalizadoEvent != null)
                                    FinalizadoEvent(this, new EventArgs());
                            }
                        }
                    }

                }, TaskCreationOptions.LongRunning);
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "RegenerarAsientosDiarioWorker.ComenzarAsync()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                if (ErrorEvent != null)
                    ErrorEvent(pobjOperationResult);
            }
        }

        protected virtual void OnProcesoEvent(int mensaje)
        {
            var handler = ProcesoEvent;
            if (handler != null) handler(mensaje);
        }
    }
}



