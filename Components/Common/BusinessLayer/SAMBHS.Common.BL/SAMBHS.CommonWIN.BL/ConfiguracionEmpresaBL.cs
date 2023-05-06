using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Linq.Dynamic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace SAMBHS.CommonWIN.BL
{
    public class ConfiguracionEmpresaBL
    {
        public configuracionempresaDto ObtenerConfiguracionEmpresa(ref OperationResult objOperationResult,
            int IdSystemUser)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbcontext = new SAMBHSEntitiesModelWin())
                {
                    #region Consulta

                    var query = (from n in dbcontext.configuracionempresa

                                 select new configuracionempresaDto
                                 {
                                     i_IdConfiguracionEmpresa = n.i_IdConfiguracionEmpresa,
                                     i_IdMoneda = n.i_IdMoneda,
                                     d_ValorMaximoBoletas = n.d_ValorMaximoBoletas,
                                     i_AfectoIgvCompras = n.i_AfectoIgvCompras,
                                     i_AfectoIgvVentas = n.i_AfectoIgvVentas,
                                     i_CantidadDecimales = n.i_CantidadDecimales,
                                     i_IdDestinoCompras = n.i_IdDestinoCompras,
                                     i_IdPermiteStockNegativo = n.i_IdPermiteStockNegativo,
                                     i_IdTipoOperacionVentas = n.i_IdTipoOperacionVentas,
                                     i_PrecioDecimales = n.i_PrecioDecimales,
                                     i_PrecioIncluyeIgvCompras = n.i_PrecioIncluyeIgvCompras,
                                     i_PrecioIncluyeIgvVentas = n.i_PrecioIncluyeIgvVentas,
                                     v_IdCuentaContableDolares = n.v_IdCuentaContableDolares,
                                     v_IdCuentaContableSoles = n.v_IdCuentaContableSoles,
                                     d_ComisionVendedor = n.d_ComisionVendedor,
                                     i_IdIgv = n.i_IdIgv,
                                     i_Eliminado = n.i_Eliminado,
                                     i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                     t_InsertaFecha = n.t_InsertaFecha,
                                     i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                     t_ActualizaFecha = n.t_ActualizaFecha,
                                     i_IdAlmacenPredeterminado = n.i_IdAlmacenPredeterminado,
                                     i_IncluirPreciosGuiaRemision = n.i_IncluirPreciosGuiaRemision,
                                     i_FechaRegistro = n.i_FechaRegistro,
                                     i_FechaEmision = n.i_FechaEmision,
                                     i_EmpresaAfectaRetencion = n.i_EmpresaAfectaRetencion,
                                     d_TasaRetencion = n.d_TasaRetencion,
                                     v_NroCuentaRetencion = n.v_NroCuentaRetencion,
                                     i_RedondearVentas = n.i_RedondearVentas,
                                     i_ComprasMostrarColumasEmpaque = n.i_ComprasMostrarColumasEmpaque,
                                     i_VentasMostrarColumasEmpaque = n.i_VentasMostrarColumasEmpaque,
                                     i_IdMonedaCompra = n.i_IdMonedaCompra,
                                     i_EditarPrecioPedido = n.i_EditarPrecioPedido,
                                     i_ComprasMostrarIscyOtrosTributos = n.i_ComprasMostrarIscyOtrosTributos,
                                     i_VentasMostrarIscyOtrosTributos = n.i_VentasMostrarIscyOtrosTributos,
                                     v_NroCuentaRedondeoGanancia = n.v_NroCuentaRedondeoGanancia,
                                     v_NroCuentaRedondeoPerdida = n.v_NroCuentaRedondeoPerdida,
                                     v_DiferenciaCambioCtaGanancia = n.v_DiferenciaCambioCtaGanancia,
                                     v_DiferenciaCambioCtaPerdida = n.v_DiferenciaCambioCtaPerdida,
                                     i_EditarPrecioVenta = n.i_EditarPrecioVenta,
                                     i_PermiteIntercambiarListasPrecios = n.i_PermiteIntercambiarListasPrecios,
                                     i_EmpresaUsaCentroCostos = n.i_EmpresaUsaCentroCostos,
                                     i_GenerarActDesdeCompras = n.i_GenerarActDesdeCompras ?? 0,
                                     v_NroCuentaUtilidadCierre = n.v_NroCuentaUtilidadCierre ?? "",
                                     v_NroCuentaPerdidaCierre = n.v_NroCuentaPerdidaCierre ?? "",
                                     v_NroCuentaResul891Cierre = n.v_NroCuentaResul891Cierre ?? "",
                                     v_NroCuentaResul892Cierre = n.v_NroCuentaResul892Cierre ?? "",
                                     v_ImpresionDirectoVentas = n.v_ImpresionDirectoVentas,
                                     v_NroCuentaAjuste = n.v_NroCuentaAjuste ?? "",
                                     i_IncluirServicioGuiaVenta = n.i_IncluirServicioGuiaVenta ?? 0,
                                     i_IncluirTransportistaGuiaRemision = n.i_IncluirTransportistaGuiaRemision ?? 0,
                                     i_ActualizarCostoProductos = n.i_ActualizarCostoProductos ?? 0,
                                    // i_IncluirLotesCompraVenta = n.i_IncluirLotesCompraVenta ?? 0,
                                     i_IncluirNingunoCompraVenta = n.i_IncluirNingunoCompraVenta ?? 0,
                                     i_IncluirPedidoExportacionCompraVenta = n.i_IncluirPedidoExportacionCompraVenta ?? 0,
                                     i_IdMonedaImportacion = n.i_IdMonedaImportacion ?? -1,
                                     v_NroCuentaISC = n.v_NroCuentaISC,
                                     v_NroCuentaPercepcion = n.v_NroCuentaPercepcion,
                                     v_NroCuentaOtrosConsumos = n.v_NroCuentaOtrosConsumos,
                                     v_NroCuentaGastosFinancierosCobranza = n.v_NroCuentaGastosFinancierosCobranza,
                                     v_NroCuentaIngresosFinancierosCobranza = n.v_NroCuentaIngresosFinancierosCobranza,
                                     i_CambiarUnidadMedidaVentaPedido = n.i_CambiarUnidadMedidaVentaPedido ?? 0,
                                     i_IncluirSEUOImpresionDocumentos = n.i_IncluirSEUOImpresionDocumentos ?? 0,
                                     i_IdDocumentoContableLEC = n.i_IdDocumentoContableLEC ?? 335,
                                     i_IdDocumentoContableLEP = n.i_IdDocumentoContableLEP ?? 335,
                                     i_TipoRegimenEmpresa = n.i_TipoRegimenEmpresa ?? -1,
                                     v_NroCuentaObligacionesFinancierosCobranza = n.v_NroCuentaObligacionesFinancierosCobranza,
                                     i_ImprimirDniPNaturalesLetras = n.i_ImprimirDniPNaturalesLetras ?? 0,
                                     i_UsaListaPrecios = n.i_UsaListaPrecios,
                                     i_IncluirAgenciaTransportePedido = n.i_IncluirAgenciaTransportePedido ?? 0,
                                     i_EditarPrecioVentaPedido = n.i_EditarPrecioVentaPedido ?? 0,
                                     v_GlosaTicket = n.v_GlosaTicket,
                                     v_NroCuentaAdelanto = n.v_NroCuentaAdelanto,
                                     i_CodigoPlanContable = n.i_CodigoPlanContable ?? -1,
                                     i_TckUseInfo = n.i_TckUseInfo,
                                     v_TckRuc = n.v_TckRuc,
                                     v_TckRzs = n.v_TckRzs,
                                     v_TckDireccion = n.v_TckDireccion,
                                     v_TckExt = n.v_TckExt,
                                     i_ValidarStockMinimoProducto = n.i_ValidarStockMinimoProducto ?? 0,
                                     v_NroCtaRetenciones = n.v_NroCtaRetenciones,
                                     i_CostoListaPreciosDiferentesxAlmacen = n.i_CostoListaPreciosDiferentesxAlmacen ?? 0,
                                     i_IncluirAlmacenDestinoGuiaRemision = n.i_IncluirAlmacenDestinoGuiaRemision ?? 0,
                                     i_VisualizarColumnasBasicasPedido = n.i_VisualizarColumnasBasicasPedido ?? 0,
                                     v_IdPlanillaConceptoFaltas = n.v_IdPlanillaConceptoFaltas,
                                     v_IdPlanillaConceptoTardanzas = n.v_IdPlanillaConceptoTardanzas,
                                     v_IdRepresentanteLegal = n.v_IdRepresentanteLegal,
                                     v_NroLeyCuartaCategoria = n.v_NroLeyCuartaCategoria ?? "",
                                     i_UsaDominicalCalculoDescuento = n.i_UsaDominicalCalculoDescuento,
                                     v_IdProductoDetalleFlete = n.v_IdProductoDetalleFlete,
                                     v_IdProductoDetalleSeguro = n.v_IdProductoDetalleSeguro,
                                     i_GenerarNotaSalidaDesdeVentaUltimoDiaMes = n.i_GenerarNotaSalidaDesdeVentaUltimoDiaMes ?? 0,
                                     i_TipoVentaVentas = n.i_TipoVentaVentas ?? -1,
                                     v_GlosaPedido = n.v_GlosaPedido,
                                     i_IdCondicionPagoPedido = n.i_IdCondicionPagoPedido ?? -1,
                                     i_SeUsaraSoloUnaListaPrecioEmpresa = n.i_SeUsaraSoloUnaListaPrecioEmpresa ?? 0,
                                     i_VisualizarBusquedaProductos = n.i_VisualizarBusquedaProductos ?? 0,
                                     i_PermiteIncluirPreciosCeroPedido = n.i_PermiteIncluirPreciosCeroPedido ?? 0,
                                     i_IdCondicionPagoVenta = n.i_IdCondicionPagoVenta ?? -1,
                                     v_NroCuentaCobranzaRedondeoGanancia = n.v_NroCuentaCobranzaRedondeoGanancia,
                                     v_NroCuentaCobranzaRedondeoPerdida = n.v_NroCuentaCobranzaRedondeoPerdida,
                                     i_CambiarAlmacenVentasDesdeVendedor = n.i_CambiarAlmacenVentasDesdeVendedor,
                                     i_EmpresaAfectaPercepcionVenta = n.i_EmpresaAfectaPercepcionVenta ?? 0,
                                     i_TipoDepreciacionActivoFijo =n.i_TipoDepreciacionActivoFijo ==null ? 1 : n.i_TipoDepreciacionActivoFijo.Value ,
                                     i_PermiteEditarPedidoFacturado =n.i_PermiteEditarPedidoFacturado ??0,
                                     b_LogoEmpresa =n.b_LogoEmpresa ,
                                     b_FirmaDigitalEmpresa =n.b_FirmaDigitalEmpresa ,
                                 }
                        ).FirstOrDefault();

                    #endregion

                    objOperationResult.Success = 1;

                    if (query != null)
                    {
                        var objData = query;
                        //se saco a una consulta a parte el id del vendedor porque hacia conflicto con la consulta en postgres.
                        var objVendedor =
                            dbcontext.vendedor
                                .FirstOrDefault(p => p.i_SystemUser == IdSystemUser && p.i_Eliminado == 0);

                        objData.v_IdVendedor = objVendedor != null ? objVendedor.v_IdVendedor : "-1";

                        if (!string.IsNullOrWhiteSpace(objData.v_IdPlanillaConceptoFaltas))
                        {
                            var conceptof = dbcontext.planillaconceptos.FirstOrDefault(p =>
                                p.v_IdConceptoPlanilla.Equals(objData.v_IdPlanillaConceptoFaltas));
                            if (conceptof != null)
                                objData.CodConceptoPlanillaFaltas = conceptof.v_Codigo;

                            var conceptot = dbcontext.planillaconceptos.FirstOrDefault(p =>
                                p.v_IdConceptoPlanilla.Equals(objData.v_IdPlanillaConceptoTardanzas));
                            if (conceptot != null)
                                objData.CodConceptoPlanillaTardanzas = conceptot.v_Codigo;
                        }


                        if (!string.IsNullOrWhiteSpace(objData.v_IdProductoDetalleFlete))
                        {
                            var codFlete = (from pd in dbcontext.productodetalle
                                            join p in dbcontext.producto on pd.v_IdProducto equals p.v_IdProducto into pJoin
                                            from p in pJoin.DefaultIfEmpty()
                                            where pd.v_IdProductoDetalle.Equals(objData.v_IdProductoDetalleFlete)
                                            select p).FirstOrDefault();

                            if (codFlete != null)
                                objData.CodArticuloFlete = codFlete.v_CodInterno;
                        }

                        if (!string.IsNullOrWhiteSpace(objData.v_IdProductoDetalleSeguro))
                        {
                            var codSeguro = (from pd in dbcontext.productodetalle
                                             join p in dbcontext.producto on pd.v_IdProducto equals p.v_IdProducto into pJoin
                                             from p in pJoin.DefaultIfEmpty()
                                             where pd.v_IdProductoDetalle.Equals(objData.v_IdProductoDetalleSeguro)
                                             select p).FirstOrDefault();

                            if (codSeguro != null)
                                objData.CodArticuloSeguroMaritimo = codSeguro.v_CodInterno;
                        }

                        return objData;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                objOperationResult.AdditionalInformation = "ConfiguracionEmpresaBL.ObtenerConfiguracionEmpresa()";
                return null;
            }
        }

        public void InsertarConfiguracionEmpresa(ref OperationResult pobjOperationResult,
            configuracionempresaDto pobjDtoEntity, List<string> ClientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    configuracionempresa objEntity = pobjDtoEntity.ToEntity();

                    if (!string.IsNullOrEmpty(pobjDtoEntity.v_NroCuentaRedondeoGanancia))
                    {
                        ActualizarRubosRedondeoVentas(ref pobjOperationResult, pobjDtoEntity.v_NroCuentaRedondeoGanancia,
                            pobjDtoEntity.v_NroCuentaRedondeoPerdida);
                    }

                    objEntity.t_InsertaFecha = DateTime.Now;
                    objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntity.i_Eliminado = 0;
                    dbContext.AddToconfiguracionempresa(objEntity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
            }
        }

        public void ActualizarConfiguracionEmpresa(ref OperationResult pobjOperationResult,
            configuracionempresaDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;
            try
            {

                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.configuracionempresa
                                           where a.i_IdConfiguracionEmpresa == pobjDtoEntity.i_IdConfiguracionEmpresa
                                           select a).FirstOrDefault();

                    // Crear la entidad con los datos actualizados
                    pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                    pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                    configuracionempresa objEntity = configuracionempresaAssembler.ToEntity(pobjDtoEntity);

                    dbContext.configuracionempresa.ApplyCurrentValues(objEntity);

                    // Guardar los cambios
                    dbContext.SaveChanges();

                    if (!string.IsNullOrEmpty(pobjDtoEntity.v_NroCuentaRedondeoGanancia))
                    {
                        ActualizarRubosRedondeoVentas(ref pobjOperationResult, pobjDtoEntity.v_NroCuentaRedondeoGanancia,
                            pobjDtoEntity.v_NroCuentaRedondeoPerdida);
                    }

                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
            }
        }

        public void EliminarConcepto(ref OperationResult pobjOperationResult, int pintIdConfiguracionEmpresa,
            List<string> ClientSession)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.configuracionempresa
                                       where a.i_IdConfiguracionEmpresa == pintIdConfiguracionEmpresa
                                       select a).FirstOrDefault();

                // Crear la entidad con los datos actualizados
                objEntitySource.t_ActualizaFecha = DateTime.Now;
                objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                objEntitySource.i_Eliminado = 1;
                dbContext.SaveChanges();

                pobjOperationResult.Success = 1;
                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return;
            }
        }

        public void ActualizarRubosRedondeoVentas(ref OperationResult pobjOperationResult, string RedondeoGanancia,
            string RedondeoPerdida)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var RuboRedondeoGanancia =
                        dbContext.datahierarchy.FirstOrDefault(p => p.i_GroupId == 51 && p.i_ItemId == 4);
                    var RuboRedondeoPerdida =
                        dbContext.datahierarchy.FirstOrDefault(p => p.i_GroupId == 51 && p.i_ItemId == 1);

                    RuboRedondeoPerdida.v_Value2 = RedondeoPerdida;
                    RuboRedondeoGanancia.v_Value2 = RedondeoGanancia;

                    dbContext.datahierarchy.ApplyCurrentValues(RuboRedondeoPerdida);
                    dbContext.datahierarchy.ApplyCurrentValues(RuboRedondeoGanancia);

                    dbContext.SaveChanges();

                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception)
            {
                pobjOperationResult.Success = 0;
                throw;
            }
        }

        public void ActualizarUsuariosDesdeTIS_Integrado(ref OperationResult pobjOperationResult)
        {
            try
            {
                using (var dbContextSeg = new SAMBHSEntitiesModel())
                {
                    var usuariosBdExterna = dbContextSeg.systemuser.Where(user => user.i_IsDeleted == 0).ToList();

                    using (var dbContextWin = new SAMBHSEntitiesModelWin())
                    {
                        if (usuariosBdExterna.Any())
                        {
                            dbContextWin.systemuser.ToList()
                                .ForEach(user => dbContextWin.systemuser.DeleteObject(user));

                            usuariosBdExterna.ForEach(usuario =>
                            {
                                var usuarioWin = dbContextWin.systemuser.CreateObject();
                                usuarioWin.d_InsertDate = usuario.d_InsertDate;
                                usuarioWin.d_UpdateDate = usuario.d_UpdateDate;
                                usuarioWin.i_InsertUserId = usuario.i_InsertUserId;
                                usuarioWin.i_IsDeleted = usuario.i_IsDeleted;
                                usuarioWin.i_PersonId = usuario.i_PersonId;
                                usuarioWin.i_RoleId = usuario.i_RoleId;
                                usuarioWin.i_SystemUserId = usuario.i_SystemUserId;
                                usuarioWin.i_UpdateUserId = usuario.i_UpdateUserId;
                                usuarioWin.v_Password = usuario.v_Password;
                                usuarioWin.v_UserName = usuario.v_UserName;

                                dbContextWin.systemuser.AddObject(usuarioWin);
                            });
                        }

                        dbContextWin.SaveChanges();
                        pobjOperationResult.Success = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation =
                    "ConfiguracionEmpresaBL.ActualizarUsuariosDesdeTIS_Integrado()\nLinea:" +
                    ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        private T GetInstance<T>(string type)
        {
            return (T)Activator.CreateInstance(Type.GetType(type));
        }
    }


}
