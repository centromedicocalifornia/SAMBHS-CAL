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
using SAMBHS.Almacen.BL;
using SAMBHS.Common.BE.Custom;
using SAMBHS.Common.BL;
using System.Transactions;
using System.Diagnostics;
using System.Data.SqlClient;
using Devart.Data.PostgreSql;
using Dapper;

namespace SAMBHS.Almacen.BL
{
    public class AlmacenBL
    {

        string Periodo = Globals.ClientSession.i_Periodo.ToString();
        string Error = ""; 
        DateTime Fechanull = DateTime.Parse("01/01/1753");

        public List<string> ObtenerDatosEmpresa(int pIntAlmacen, int pIntEstablecimiento)
        {

            List<string> Lista = new List<string>();
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {

                var Direccion = (from a in dbContext.establecimientoalmacen

                                 join b in dbContext.establecimiento on
                                     new { IdEstablecimiento = a.i_IdEstablecimiento.Value, eliminado = 0 } equals
                                     new { IdEstablecimiento = b.i_IdEstablecimiento, eliminado = 0 } into b_join

                                 from b in b_join.DefaultIfEmpty()

                                 join c in dbContext.almacen on new { IdAlmacen = a.i_IdAlmacen.Value, eliminado = 0 } equals
                                     new { IdAlmacen = c.i_IdAlmacen, eliminado = c.i_Eliminado.Value } into c_join
                                 from c in c_join.DefaultIfEmpty()


                                 where b.i_IdEstablecimiento == pIntEstablecimiento && a.i_IdAlmacen == pIntAlmacen

                                 select new
                                 {
                                     Direccion = c.v_Direccion == null ? "" : c.v_Direccion,
                                     Telefono = c.v_Telefono == null ? "" : c.v_Telefono,
                                     Email = c.v_Observacion == null ? "" : c.v_Observacion,

                                 }).FirstOrDefault();

                if (Direccion != null)
                {

                    Lista.Add(Direccion.Direccion);
                    Lista.Add(Direccion.Telefono);
                    Lista.Add(Direccion.Email);
                }
                //else
                //{
                //    return string.Empty;
                //}
                return Lista;
            }

        }

        public List<ReporteAlmacen> ReporteAlmacen(int pintIdAlmacen)
        {
            try
            {

                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    //int _intNodeId = 1;

                    #region Query

                    var query = (from A in dbContext.almacen
                                 where (A.i_Eliminado == 0 && A.i_IdAlmacen == pintIdAlmacen)

                                 select new ReporteAlmacen
                                 {
                                     v_Nombre = A.v_Nombre,
                                     v_Direccion = A.v_Direccion,
                                     v_Telefono = A.v_Telefono,
                                     v_NombreComercial = A.v_NombreComercial,
                                     v_NumeroSerieTicket = A.v_NumeroSerieTicket,
                                     v_Observacion = A.v_Observacion,


                                 });

                    #endregion

                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                //pobjOperationResult.Success = 0;
                //pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public List<AlmacenConsulta> ObtenerListadoAlmacen(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = from A in dbContext.almacen

                                join J1 in dbContext.systemuser on new { i_InsertUserId = A.i_InsertaIdUsuario.Value, eliminado = 0 }
                                                            equals new { i_InsertUserId = J1.i_SystemUserId, eliminado = J1.i_IsDeleted.Value } into J1_join
                                from J1 in J1_join.DefaultIfEmpty()

                                join J2 in dbContext.systemuser on new { i_UpdateUserId = A.i_ActualizaIdUsuario.Value, eliminado = 0 }
                                                            equals new { i_UpdateUserId = J2.i_SystemUserId, eliminado = J2.i_IsDeleted.Value } into J2_join
                                from J2 in J2_join.DefaultIfEmpty()

                                where A.i_Eliminado == 0

                                select new AlmacenConsulta
                                {
                                    i_IdAlmacen = A.i_IdAlmacen,
                                    v_Nombre = A.v_Nombre,
                                    v_Direccion = A.v_Direccion,
                                    v_Telefono = A.v_Telefono,
                                    v_NombreComercial = A.v_NombreComercial,
                                    v_NumeroSerieTicket = A.v_NumeroSerieTicket,
                                    v_Observacion = A.v_Observacion,
                                };

                    if (!string.IsNullOrEmpty(pstrFilterExpression))
                    {
                        query = query.Where(pstrFilterExpression);
                    }
                    if (!string.IsNullOrEmpty(pstrSortExpression))
                    {
                        query = query.OrderBy(pstrSortExpression);
                    }

                    List<AlmacenConsulta> objData = query.ToList();
                    pobjOperationResult.Success = 1;
                    return objData;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public bool ExistenciaAlmacenDiversosProcesos(int pintIdAlmacen)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var ProductoAlmacen = (from a in dbContext.productoalmacen
                                       where a.i_IdAlmacen == pintIdAlmacen
                                       select a.i_IdAlmacen).ToList();
                return ProductoAlmacen.Any();
            }
        }

        #region CRUD
        public int InsertarAlmacen(ref OperationResult pobjOperationResult, almacenDto pObjDtoEntity, List<string> ClientSession)
        {

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    almacen objEntity = pObjDtoEntity.ToEntity();

                    int newId = (dbContext.almacen.Count() == 0) ? (1) : (dbContext.almacen.Max(item => item.i_IdAlmacen) + 1);
                    objEntity.i_IdAlmacen = newId;
                    objEntity.t_InsertaFecha = DateTime.Now;
                    objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntity.i_Eliminado = 0;

                    dbContext.AddToalmacen(objEntity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                    Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "almacen", newId.ToString());
                    return newId;
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AlmacenBL.InsertarAlmacen()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return 0;
            }
        }

        public int ActualizarAlmacen(ref OperationResult pobjOperationResult, almacenDto pobjDtoEntity, List<string> ClientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var objEntitySource = (from a in dbContext.almacen
                                           where a.i_IdAlmacen == pobjDtoEntity.i_IdAlmacen
                                           select a).FirstOrDefault();
                    pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                    pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                    almacen objEntity = pobjDtoEntity.ToEntity();

                    dbContext.almacen.ApplyCurrentValues(objEntity);
                    dbContext.SaveChanges();

                    Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "almacen", objEntitySource.i_IdAlmacen.ToString());
                    pobjOperationResult.Success = 1;
                    return pobjDtoEntity.i_IdAlmacen;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AlmacenBL.ActualizarAlmacen()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return 0;
            }
        }

        public almacenDto ObtenerAlmacen(ref OperationResult pobjOperationResult, int pintIdAlmacen)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    almacenDto objDtoEntity = null;

                    var objEntity = (from a in dbContext.almacen
                                     where a.i_IdAlmacen == pintIdAlmacen && a.i_Eliminado == 0
                                     select a).FirstOrDefault();

                    if (objEntity != null)
                        objDtoEntity = objEntity.ToDTO();

                    pobjOperationResult.Success = 1;
                    return objDtoEntity;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public void EliminarAlmacen(ref OperationResult pobjOperationResult, int pintIdAlmacen, List<string> ClientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var objEntitySource = (from a in dbContext.almacen
                                           where a.i_IdAlmacen == pintIdAlmacen
                                           select a).FirstOrDefault();

                    objEntitySource.t_ActualizaFecha = DateTime.Now;
                    objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntitySource.i_Eliminado = 1;
                    dbContext.SaveChanges();
                    Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "almacen", objEntitySource.i_IdAlmacen.ToString());
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "AlmacenBL.EliminarAlmacen()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }
        #endregion

        #region Reporte


        public decimal GetCantidadEmpaque(KardexList objKardex)
        {
            decimal totalEmpaque = 0;
            if (objKardex.i_IdUnidadMedidaProducto == null) return 0M;
            if (objKardex.d_Empaque == null) totalEmpaque = 0M;

            if (objKardex.v_IdProducto != null && objKardex.i_IdUnidad != -1)
            {

                var empaque = objKardex.d_Empaque.Value;
                var cantidad = objKardex.Saldo_Cantidad;

                var umProducto = objKardex.i_IdUnidadMedidaProducto.ToString();
                var um = objKardex.i_IdUnidad.ToString();

                if (um != null)
                {
                    switch (objKardex.UnidadMedida)
                    {
                        case "CAJA":
                            var caja = empaque * (!string.IsNullOrEmpty(objKardex.ValorUM) ? decimal.Parse(objKardex.ValorUM) : 0);
                            totalEmpaque = cantidad.Value * caja;
                            break;

                        default:
                            totalEmpaque = cantidad.Value * (!string.IsNullOrEmpty(objKardex.ValorUM) ? decimal.Parse(objKardex.ValorUM) : 0);
                            break;
                    }
                }
                return totalEmpaque;
            }
            else
                return 0M;
        }

        public List<ReporteNotaSalidaAlmacen> ReporteNotaSalidaAlmacen(string pstrIdMovimiento, int pintTipoMovimiento)
        {
            try
            {

                int _b = pintTipoMovimiento == 1 ? 19 : 20;

                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from n in dbContext.movimiento

                                 join A in dbContext.almacen on n.i_IdAlmacenOrigen equals A.i_IdAlmacen into A_join
                                 from A in A_join.DefaultIfEmpty()

                                 join B in dbContext.movimientodetalle on n.v_IdMovimiento
                                                                        equals B.v_IdMovimiento into B_join
                                 from B in B_join.DefaultIfEmpty()
                                 join C in dbContext.productodetalle on B.v_IdProductoDetalle equals C.v_IdProductoDetalle
                                 join D in dbContext.producto on C.v_IdProducto
                                                                equals D.v_IdProducto into D_join
                                 from D in D_join.DefaultIfEmpty()

                                 //join E in dbContext.cliente on n.v_IdCliente equals E.v_IdCliente into E_join
                                 //from E in E_join.DefaultIfEmpty()

                                 join F in dbContext.datahierarchy on new { i_TipoMotivo = n.i_IdTipoMotivo.Value, b = _b }
                                                                equals new { i_TipoMotivo = F.i_ItemId, b = F.i_GroupId } into J1_join
                                 from F in J1_join.DefaultIfEmpty()
                                 join G in dbContext.datahierarchy on B.i_IdUnidad equals G.i_ItemId into G_join
                                 from G in G_join.DefaultIfEmpty()

                                 where (G.i_GroupId == 17 && G.i_IsDeleted == 0) && (F.i_GroupId == 20 && F.i_IsDeleted == 0)

                                 where n.i_Eliminado == 0 && n.v_IdMovimiento == pstrIdMovimiento && A.i_Eliminado == 0 && B.i_Eliminado == 0 && C.i_Eliminado == 0 && D.i_Eliminado == 0
                                 orderby D.v_CodInterno
                                 select new ReporteNotaSalidaAlmacen
                                 {
                                     NombreEmpresaPropietaria = "",
                                     RucEmpresaPropietaria = "",
                                     IdMovimiento = n.v_IdMovimiento,
                                     Fecha = n.t_Fecha.Value,
                                     TipoCambio = n.d_TipoCambio.Value,
                                     Pedido = "",
                                     DescripcionMotivo = F.v_Value1,
                                     NombreCliente = "",
                                     NroDocCliente = "",
                                     Concepto = n.v_Glosa,
                                     Periodo = n.v_Periodo,
                                     Mes = n.v_Mes,
                                     Correlativo = "SALIDA DEL ALMACÉN N° " + n.v_Mes + "-" + n.v_Correlativo,
                                     IdProducto = D.v_CodInterno,
                                     NombreProducto = D.v_Descripcion,
                                     UnidadMedida = G.v_Value1,
                                     CantidadDetalle = B.d_Cantidad.Value,
                                     PrecioDetalle = B.d_Precio.Value,
                                     TotalDetalle = B.d_Total.Value,
                                     NombreMoneda = n.i_IdMoneda.Value == (int)Currency.Soles ? "S/." : "$",
                                     IdMoneda = n.i_IdMoneda.Value,
                                     Guia = B.v_NroGuiaRemision,
                                     Documento = B.i_IdTipoDocumento,
                                     NumeroDocumento = B.v_NumeroDocumento,
                                     v_IdCliente = n.v_IdCliente,
                                 });


                    var query1 = (from A in query.ToList()

                                  let NombreCliente = ObtenerNombreCliente(A.v_IdCliente)
                                  let DocumentoSiglas = ObtenerSiglasDocumento(A.Documento.Value)
                                  select new ReporteNotaSalidaAlmacen
                                  {
                                      NombreEmpresaPropietaria = "",//DevolverNombreEmpresaPropietaria()[0],
                                      RucEmpresaPropietaria = "",//DevolverNombreEmpresaPropietaria()[1],
                                      IdMovimiento = A.IdMovimiento,
                                      Fecha = A.Fecha,
                                      TipoCambio = A.TipoCambio,
                                      Pedido = A.Pedido,
                                      DescripcionMotivo = A.DescripcionMotivo,
                                      NombreCliente = NombreCliente,
                                      NroDocCliente = A.NroDocCliente,
                                      Concepto = A.Concepto,
                                      Periodo = A.Periodo,
                                      Mes = A.Mes,
                                      Correlativo = A.Correlativo,
                                      IdProducto = A.IdProducto,
                                      NombreProducto = A.NombreProducto,
                                      UnidadMedida = A.UnidadMedida,
                                      CantidadDetalle = A.CantidadDetalle,
                                      PrecioDetalle = A.PrecioDetalle,
                                      TotalDetalle = A.TotalDetalle,
                                      NombreMoneda = A.NombreMoneda,
                                      IdMoneda = A.IdMoneda,
                                      Guia = A.Guia,
                                      Documento = A.Documento,
                                      NumeroDocumento = DocumentoSiglas + " " + A.NumeroDocumento,
                                  });


                    return query1.ToList();
                }
            }
            catch (Exception)
            {
                //pobjOperationResult.Success = 0;
                //pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public List<ReporteNotaIngresoAlmacen> ReporteNotaIngresoAlmacen(string pstrIdMovimiento, int pintTipoMovimiento)
        {
            try
            {

                int _b = pintTipoMovimiento == 1 ? 19 : 20;
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from n in dbContext.movimiento

                                 join A in dbContext.almacen on n.i_IdAlmacenOrigen equals A.i_IdAlmacen into A_join
                                 from A in A_join.DefaultIfEmpty()

                                 join B in dbContext.movimientodetalle on n.v_IdMovimiento
                                                                        equals B.v_IdMovimiento into B_join
                                 from B in B_join.DefaultIfEmpty()
                                 join C in dbContext.productodetalle on B.v_IdProductoDetalle equals C.v_IdProductoDetalle
                                 join D in dbContext.producto on C.v_IdProducto
                                                                equals D.v_IdProducto into D_join
                                 from D in D_join.DefaultIfEmpty()

                                 join F in dbContext.datahierarchy on new { i_TipoMotivo = n.i_IdTipoMotivo.Value, b = _b }
                                                                equals new { i_TipoMotivo = F.i_ItemId, b = F.i_GroupId } into J1_join
                                 from F in J1_join.DefaultIfEmpty()
                                 join G in dbContext.datahierarchy on B.i_IdUnidad equals G.i_ItemId into G_join
                                 from G in G_join.DefaultIfEmpty()

                                 where (G.i_GroupId == 17 && G.i_IsDeleted == 0) && (F.i_GroupId == 19 && F.i_IsDeleted == 0)



                                 where n.i_Eliminado == 0 && n.v_IdMovimiento == pstrIdMovimiento && A.i_Eliminado == 0 && B.i_Eliminado == 0 && C.i_Eliminado == 0 && D.i_Eliminado == 0

                                 select new ReporteNotaIngresoAlmacen
                                 {
                                     NombreEmpresaPropietaria = "",
                                     RucEmpresaPropietaria = "",
                                     IdMovimiento = n.v_IdMovimiento,
                                     Fecha = n.t_Fecha.Value,
                                     TipoCambio = n.d_TipoCambio.Value,
                                     OrdeCompra = "",
                                     DescripcionMotivo = F.v_Value1,
                                     Detalle = "",
                                     Concepto = n.v_Glosa,
                                     Periodo = n.v_Periodo,
                                     Mes = n.v_Mes,
                                     Correlativo = "NOTA DE INGRESO DEL ALMACÉN N° " + n.v_Mes + "-" + n.v_Correlativo,
                                     IdProducto = D.v_CodInterno,
                                     NombreProducto = D.v_Descripcion,
                                     UnidadMedida = G.v_Value1,
                                     CantidadDetalle = B.d_Cantidad.Value,
                                     PrecioDetalle = B.d_Precio.Value,
                                     TotalDetalle = B.d_Total.Value,
                                     NombreMoneda = n.i_IdMoneda.Value == (int)Currency.Soles ? "S/." : "$",
                                     IdMoneda = n.i_IdMoneda.Value,
                                     Guia = B.v_NroGuiaRemision,
                                     Documento = B.i_IdTipoDocumento.Value,
                                     NumeroDocumento = B.v_NumeroDocumento,
                                     v_IdCliente = n.v_IdCliente,
                                     v_NroOrdenCompra = n.v_NroOrdenCompra,


                                 }
                        );


                    var query1 = (from A in query.ToList()

                                  let DocumentoSiglas = ObtenerSiglasDocumento(A.Documento.Value)
                                  let NombreCliente = ObtenerNombreCliente(A.v_IdCliente)
                                  select new ReporteNotaIngresoAlmacen
                                  {
                                      NombreEmpresaPropietaria = "",// DevolverNombreEmpresaPropietaria()[0],
                                      RucEmpresaPropietaria = "", //DevolverNombreEmpresaPropietaria()[1],
                                      IdMovimiento = A.IdMovimiento,
                                      Fecha = A.Fecha,
                                      TipoCambio = A.TipoCambio,
                                      OrdeCompra = A.OrdeCompra,
                                      DescripcionMotivo = A.DescripcionMotivo,
                                      Detalle = NombreCliente,
                                      Concepto = A.Concepto,
                                      Periodo = A.Periodo,
                                      Mes = A.Mes,
                                      Correlativo = A.Correlativo,
                                      IdProducto = A.IdProducto,
                                      NombreProducto = A.NombreProducto,
                                      UnidadMedida = A.UnidadMedida,
                                      CantidadDetalle = A.CantidadDetalle,
                                      PrecioDetalle = A.PrecioDetalle,
                                      TotalDetalle = A.TotalDetalle,
                                      NombreMoneda = A.NombreMoneda,
                                      IdMoneda = A.IdMoneda,
                                      Guia = A.Guia,
                                      Documento = A.Documento,
                                      NumeroDocumento = DocumentoSiglas + " " + A.NumeroDocumento,
                                  });


                    return query1.ToList();
                }
            }
            catch (Exception ex)
            {

                return null;
                throw ex;
            }
        }
        public string ObtenerNombreCliente(string pstrIdCliente)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var Cliente = (from n in dbContext.cliente
                               where n.i_Eliminado == 0 && n.v_IdCliente == pstrIdCliente
                               select n).FirstOrDefault();

                if (Cliente != null)
                    return string.Join(" ", Cliente.v_PrimerNombre, Cliente.v_ApePaterno, Cliente.v_ApeMaterno, Cliente.v_RazonSocial).Trim();
                return string.Empty;
            }
        }

        public List<ReporteListadoSalidaAlmacenAnalitico> ReporteAsientoConsumoNuevo(ref OperationResult objOperationResult, DateTime FechaInicio, DateTime FechaFin, string _strFilterExpression, bool EsDetallado, string CodProducto, bool TomarCuentaNroPedido)
        {
            List<ReporteListadoSalidaAlmacenAnalitico> ListaAsientoConsumo = new List<ReporteListadoSalidaAlmacenAnalitico>();
            string Orden = "Establecimiento,Almacen,Correlativo,FECHA ASC";
            var HaberNotaSalida = ReporteListadoSalidaAlmacenAnalitico(ref objOperationResult, FechaInicio, FechaFin, _strFilterExpression, CodProducto, "", 1, "", "", Orden, "", "", "", "", "", "", (int)Currency.Todas, Globals.ClientSession.i_IdEstablecimiento.Value, TomarCuentaNroPedido, false, true);
            var DebeNotaSalida = ReporteListadoSalidaAlmacenAnalitico(ref objOperationResult, FechaInicio, FechaFin, _strFilterExpression, CodProducto, "", 1, "", "", Orden, "", "", "", "", "", "", (int)Currency.Todas, Globals.ClientSession.i_IdEstablecimiento.Value, TomarCuentaNroPedido, true, true);

            ListaAsientoConsumo = HaberNotaSalida.Concat(DebeNotaSalida).ToList();

            if (EsDetallado)
            {
                //ListaResumen = ListaResumen.OrderBy(z => z.CodigoProducto).ToList();
                //return ListaResumen;
                return ListaAsientoConsumo;
            }
            else
            {
                ListaAsientoConsumo = ListaAsientoConsumo.GroupBy(x => new { x.Cuenta })
                       .SelectMany(y => y.Select(u => new ReporteListadoSalidaAlmacenAnalitico
                       {

                           Cuenta = u.Cuenta,
                           CuentaDebe = u.CuentaDebe,
                           CuentaHaber = u.CuentaHaber,
                           DebeSoles = Utils.Windows.DevuelveValorRedondeado(y.Sum(i => i.DebeSoles.Value), 2),
                           DebeDolares = Utils.Windows.DevuelveValorRedondeado(y.Sum(i => i.DebeDolares.Value), 2),
                           HaberSoles = Utils.Windows.DevuelveValorRedondeado(y.Sum(i => i.HaberSoles.Value), 2),
                           HaberDolares = Utils.Windows.DevuelveValorRedondeado(y.Sum(i => i.HaberDolares.Value), 2),

                       })).ToList().GroupBy(x => x.Cuenta).Select(x => x.FirstOrDefault()).ToList();

                return ListaAsientoConsumo;
            }
        }


        //En este procedimiento no se toma en cuenta el artificio cuando no tiene Saldo Inicial --- se modifico 12 abril del 2017
        /* public List<ReporteListadoSalidaAlmacenAnalitico> ReporteListadoSalidaAlmacenAnalitico(ref OperationResult objOperationResult, DateTime? pstrt_FechaRegistroIni, DateTime? pstrt_FechaRegistroFin, string pstrFilterExpression, string pstr_CodInterno, string pstr_CodLinea, int pintIdTipoMotivo, string pstrIdCliente, string pstrNumPedido, string pstrt_Orden, string pstr_grupollave, string pstr_Nombregrupollave, string pstr_grupollave2, string pstr_Nombregrupollave2, string NombreEmpresa, string RucEmpresa, int pintMoneda, int IdEstablecimento,bool IncluirNroPedido, bool Debe = false, bool Consumo = false)
         {
             try
             {
                 int FormatoCant = (int)FormatoCantidad.Unidades;
                 using (var dbContext = new SAMBHSEntitiesModelWin())
                 {
                     DateTime FechaInicioValorizar = DateTime.Parse("01/01/" + Globals.ClientSession.i_Periodo);
                     int ConsideraDocInternos = 1;

                     string IdProductoOld = "";
                     var query = (from A in dbContext.movimientodetalle
                                  join B in dbContext.productodetalle on
                                  new { IdProductoDetalle = A.v_IdProductoDetalle, eliminado = 0 } equals
                                  new { IdProductoDetalle = B.v_IdProductoDetalle, eliminado = B.i_Eliminado.Value } into B_join
                                  from B in B_join.DefaultIfEmpty()
                                  join C in dbContext.producto on new { IdProducto = B.v_IdProducto, eliminado = 0 } equals
                                  new { IdProducto = C.v_IdProducto, eliminado = C.i_Eliminado.Value } into C_join
                                  from C in C_join.DefaultIfEmpty()
                                  join D in dbContext.movimiento on new { IdMovimiento = A.v_IdMovimiento, eliminado = 0 } equals
                                  new { IdMovimiento = D.v_IdMovimiento, eliminado = D.i_Eliminado.Value } into D_join
                                  from D in D_join.DefaultIfEmpty()
                                  join E in dbContext.cliente on new { IdCliente = D.v_IdCliente, eliminado = 0 } equals
                                  new { IdCliente = E.v_IdCliente, eliminado = E.i_Eliminado.Value } into E_join
                                  from E in E_join.DefaultIfEmpty()
                                  join F in dbContext.establecimientoalmacen on
                                  new { IdAlmacenOrigen = D.i_IdAlmacenOrigen, eliminado = 0 } equals
                                  new { IdAlmacenOrigen = F.i_IdAlmacen, eliminado = F.i_Eliminado.Value } into F_join
                                  from F in F_join.DefaultIfEmpty()
                                  join G in dbContext.almacen on new { IdAlmacen = F.i_IdAlmacen.Value, eliminado = 0 } equals
                                  new { IdAlmacen = G.i_IdAlmacen, eliminado = G.i_Eliminado.Value } into G_join
                                  from G in G_join.DefaultIfEmpty()
                                  join H in dbContext.documento on new { IdTipoDoc = A.i_IdTipoDocumento.Value, eliminado = 0 }
                                  equals new { IdTipoDoc = H.i_CodigoDocumento, eliminado = H.i_Eliminado.Value } into H_join
                                  from H in H_join.DefaultIfEmpty()

                                  join I in dbContext.datahierarchy on
                                  new { Grupo = 17, eliminado = 0, UnidadMedida = C.i_IdUnidadMedida.Value } equals
                                  new { Grupo = I.i_GroupId, eliminado = I.i_IsDeleted.Value, UnidadMedida = I.i_ItemId } into
                                  I_join
                                  from I in I_join.DefaultIfEmpty()

                                  join M in dbContext.datahierarchy on
                                  new { Motivo = D.i_IdTipoMotivo.Value, Grupo = 20, eliminado = 0 }
                                  equals new { Motivo = M.i_ItemId, Grupo = M.i_GroupId, eliminado = M.i_IsDeleted.Value } into
                                  M_join
                                  from M in M_join.DefaultIfEmpty()

                                  join N in dbContext.establecimiento on new { estab = F.i_IdEstablecimiento.Value, eliminado = 0 }
                                  equals new { estab = N.i_IdEstablecimiento, eliminado = N.i_Eliminado.Value } into N_join
                                  from N in N_join.DefaultIfEmpty()

                                  join O in dbContext.linea on new { l = C.v_IdLinea, eliminado = 0 } equals
                                  new { l = O.v_IdLinea, eliminado = O.i_Eliminado.Value } into O_join
                                  from O in O_join.DefaultIfEmpty()

                                  where A.i_Eliminado == 0 && D.i_IdTipoMovimiento != (int)TipoDeMovimiento.Transferencia
                                        && (F.i_IdEstablecimiento == IdEstablecimento || IdEstablecimento == -1) &&
                                        (C.v_CodInterno == pstr_CodInterno || pstr_CodInterno == "") &&
                                        (A.v_NroPedido == pstrNumPedido || pstrNumPedido == "")
                                        &&
                                        (D.t_Fecha.Value >= FechaInicioValorizar &&
                                         D.t_Fecha.Value <= pstrt_FechaRegistroFin.Value)
                                        && C.i_EsServicio == 0
                                        && C.i_EsActivoFijo == 0
                                        &&
                                        (D.v_OrigenTipo == null || D.v_OrigenTipo == "" ||
                                         D.v_OrigenTipo == Constants.OrigenCompra || D.v_OrigenTipo == Constants.OrigenImportacion ||
                                         D.v_OrigenTipo == Constants.OrigenTransferencia ||
                                         D.v_OrigenTipo == Constants.OrigenVenta || D.v_OrigenTipo == Constants.OrigenGuiaInterna ||
                                         (!string.IsNullOrEmpty(D.v_OrigenTipo) && D.v_OrigenTipo != Constants.OrigenGuiaCompra))
                                  // No toma en cuentas las GUIAS DE COMPRA
                                  select new ReporteListadoSalidaAlmacenAnalitico
                                  {
                                      v_IdMovimientoDetalle = A.v_IdMovimientoDetalle,
                                      v_IdProducto = C == null ? "" : C.v_IdProducto,
                                      // NombreProducto = C == null ? "" : C.v_CodInterno + "      " + C.v_Descripcion,
                                      NombreProducto = C == null ? "" : C.v_CodInterno + " " + C.v_Descripcion.Trim(),
                                      v_NombreProducto = IncluirNroPedido ? A.v_NroPedido == null ? "PRODUCTO :" + C.v_CodInterno.Trim() + " " + C.v_Descripcion.Trim() + "  PEDIDO :" : "PRODUCTO :" + C.v_CodInterno.Trim() + " " + C.v_Descripcion.Trim() + "  PEDIDO :" + A.v_NroPedido.Trim() : "PRODUCTO :" + C.v_CodInterno.Trim() + " " + C.v_Descripcion.Trim(),
                                      i_IdTipoMovimiento = D.i_IdTipoMovimiento.Value,
                                      i_IdTipoMotivo = D.i_IdTipoMotivo.Value,
                                      v_IdLinea = C.v_IdLinea,
                                      Almacen = G.v_Nombre,
                                      Guia = A.v_NroGuiaRemision,
                                      Documento = H == null ? A.v_NumeroDocumento : H.v_Siglas.Trim() + " " + A.v_NumeroDocumento.Trim(),
                                      IdAlmacen = D.i_IdAlmacenOrigen.Value,
                                      TipoMotivo = D.i_IdTipoMotivo == 5 ? 1 : 2,
                                      IdAlmacenOrigen = D.i_IdAlmacenOrigen.Value,
                                      i_IdTipoDocumentoDetalle = A.i_IdTipoDocumento == null ? -1 : A.i_IdTipoDocumento.Value,
                                      EsDocumentoInterno = H.i_UsadoDocumentoInterno == null ? 0 : H.i_UsadoDocumentoInterno.Value,
                                      EsDocInverso = H.i_UsadoDocumentoInverso == null ? 0 : H.i_UsadoDocumentoInverso.Value,
                                      ValorUM = I.v_Value2,
                                      v_IdMarca = C.v_IdMarca,
                                      Origen = D.v_OrigenTipo,
                                      IdMovimiento = A.v_IdMovimiento,
                                      t_Fecha = D.t_Fecha.Value,
                                      Movimiento = M == null ? "" : M.v_Value2,
                                      Periodo = D.v_Periodo,
                                      Mes = D.v_Mes,
                                      Correlativo = D.v_Mes + "-" + D.v_Correlativo,
                                      IdProducto = G == null ? "" : C.v_CodInterno,
                                      UnidadMedida = I == null ? "**U.M. NO EXISTE**" : I.v_Value1,
                                      TipoCambio = D.d_TipoCambio.Value,
                                      NombreMoneda = "",
                                      IdMoneda = D.i_IdMoneda.Value,
                                      Establecimiento = "ESTABLECIMIENTO :" + N.v_Nombre,
                                      NombreLinea = C == null ? "**LINEA NO EXISTE**" : O.v_Nombre ?? "",
                                      MotivoSalida = M.v_Value1 ?? "",
                                      ClienteProveedor = D.v_OrigenTipo == "T" ? D.v_Glosa == null || D.v_Glosa == string.Empty ? "TRANSFERENCIA " : D.v_Glosa : E == null ? "**CLIENTE/PROVEEDOR  NO EXISTE**" : (E.v_ApePaterno + " " + E.v_ApeMaterno + " " + E.v_PrimerNombre + " " + E.v_RazonSocial).Trim(),
                                      NumeroGuiaRemision = A.v_NroGuiaRemision,
                                      Numeropedido = IncluirNroPedido ? A.v_NroPedido ?? "" :"",
                                      Grupollave = pstr_grupollave == "NOMBREPRODUCTO" ? G == null ? "** " + pstr_Nombregrupollave + " NO EXISTE **" : pstr_Nombregrupollave + " : " + C.v_Descripcion : "" + pstr_grupollave == "NOMBRECLIENTE" ? I == null ? "** " + pstr_Nombregrupollave + " NO EXISTE **" : pstr_Nombregrupollave + " : " + (E.v_CodCliente.Trim() + " " + E.v_PrimerNombre + " " + E.v_ApePaterno + " " + E.v_ApeMaterno + " " + E.v_RazonSocial).Trim() : "" + pstr_grupollave == "NOMBRELINEA" ? H == null ? "** " + pstr_Nombregrupollave + " NO EXISTE **" : pstr_Nombregrupollave + " : " + H.v_Nombre : "" + pstr_grupollave == "MOVIMIENTO" ? M == null ? "** " + pstr_Nombregrupollave + " NO EXISTE **" : "MOVIMIENTO :" + M.v_Value2 : "" + pstr_grupollave == "NUMEROPEDIDO" ? E == null ? "** " + pstr_Nombregrupollave + " NO EXISTE **" : pstr_Nombregrupollave + " : " + A.v_NroPedido : "" + pstr_grupollave == "MOTIVOSALIDA" ? A == null ? "** " + pstr_Nombregrupollave + " NO EXISTE **" : pstr_Nombregrupollave + " : " + M.v_Value1 : "" + pstr_grupollave == "NOMBRELINEA/NOMBREPRODUCTO" ? H == null ? "** " + "LÍNEA" + " NO EXISTE **" : "LINEA" + " : " + H.v_Nombre.Trim() : "",
                                      Grupollave2 = pstr_grupollave2 == "NOMBREPRODUCTO" ? G == null ? "** " + pstr_Nombregrupollave2 + " NO EXISTE **" : pstr_Nombregrupollave2 + " : " + C.v_Descripcion : "" + pstr_grupollave2 == "NOMBRECLIENTE" ? I == null ? "** " + pstr_Nombregrupollave2 + " NO EXISTE **" : pstr_Nombregrupollave2 + " : " + (E.v_CodCliente.Trim() + " " + E.v_PrimerNombre + " " + E.v_ApePaterno + " " + E.v_ApeMaterno + " " + E.v_RazonSocial).Trim() : "" + pstr_grupollave2 == "NOMBRELINEA" ? H == null ? "** " + pstr_Nombregrupollave2 + " NO EXISTE **" : pstr_Nombregrupollave2 + " : " + H.v_Nombre : "" + pstr_grupollave2 == "MOVIMIENTO" ? M == null ? "** " + pstr_Nombregrupollave2 + " NO EXISTE **" : "MOVIMIENTO :" + M.v_Value2 : "" + pstr_grupollave2 == "NUMEROPEDIDO" ? E == null ? "** " + pstr_Nombregrupollave2 + " NO EXISTE **" : pstr_Nombregrupollave2 + " : " + A.v_NroPedido : "" + pstr_grupollave2 == "MOTIVOSALIDA" ? A == null ? "** " + pstr_Nombregrupollave2 + " NO EXISTE **" : pstr_Nombregrupollave2 + " : " + M.v_Value1 : "" + pstr_grupollave2 == "NOMBRELINEA/NOMBREPRODUCTO" ? G == null ? "** " + "DESCRIPCIÓN" + " NO EXISTE **" : "DESCRIPCICIÓN" + " : " + C.v_Descripcion.Trim() : "",
                                      i_EsDevolucion = D.i_EsDevolucion ?? 0,
                                      d_Total = D.v_OrigenTipo == "I" ? pintMoneda == -1 ? D.i_IdMoneda == (int)Currency.Soles ? A.d_Total : A.d_TotalCambio : pintMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : D.i_IdMoneda == (int)Currency.Soles ? A.d_Total : A.d_Total,
                                      d_TotalDolares = D.v_OrigenTipo == "I" ? pintMoneda == -1 ? D.i_IdMoneda == (int)Currency.Dolares ? A.d_Total : A.d_TotalCambio : pintMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : D.i_IdMoneda == (int)Currency.Dolares ? A.d_Total : (D.d_TipoCambio == 0 ? 0 : A.d_Total / D.d_TipoCambio),
                                      d_Cantidad = A.d_CantidadEmpaque == null ? A.d_Cantidad == null ? 0 : A.d_Cantidad : A.d_CantidadEmpaque,
                                      d_Precio = D.v_OrigenTipo == "I" ? pintMoneda == -1 ? D.i_IdMoneda == (int)Currency.Soles ? A.d_Total : A.d_TotalCambio : pintMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : D.i_IdMoneda == (int)Currency.Soles ? A.d_Total : A.d_Total,
                                      d_PrecioDolares = D.v_OrigenTipo == "I" ? pintMoneda == -1 ? D.i_IdMoneda == (int)Currency.Dolares ? A.d_Total : A.d_TotalCambio : pintMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : D.i_IdMoneda == (int)Currency.Dolares ? A.d_Total : (D.d_TipoCambio == 0 ? 0 : A.d_Total / D.d_TipoCambio),
                                      CodigoProducto = C.v_CodInterno,
                                      Cuenta = Debe ? D.i_EsDevolucion == 1 ? "CUENTA :" + O.v_NroCuentaHConsumo : "CUENTA :" + O.v_NroCuentaDConsumo : D.i_EsDevolucion == 1 ? "CUENTA :" + O.v_NroCuentaDConsumo : "CUENTA :" + O.v_NroCuentaHConsumo,
                                      SoloCuenta = Debe ? D.i_EsDevolucion == 1 ? O.v_NroCuentaHConsumo : O.v_NroCuentaDConsumo : D.i_EsDevolucion == 1 ? O.v_NroCuentaDConsumo : O.v_NroCuentaHConsumo,
                                      LetrasSubTotalLinea = "SUB- TOTAL CUENTA : " + O.v_NroCuentaDConsumo + " : ",
                                  }).ToList().AsQueryable();



                     if (!string.IsNullOrEmpty(pstrFilterExpression))
                     {
                         query = query.Where(pstrFilterExpression);
                     }

                     var queryFinal =
                         query.ToList()
                             .OrderBy(x => x.IdAlmacenOrigen)
                             .ThenBy(x => x.v_NombreProducto)
                             .ThenBy(x => x.t_Fecha)
                             .ThenBy(x => x.i_IdTipoMovimiento)
                             .ThenBy(x => x.TipoMotivo)
                             .ThenBy(x => x.NroPedido)
                             .ToList();


                     queryFinal = queryFinal.ToList().Select(n => new ReporteListadoSalidaAlmacenAnalitico
                     {
                         v_IdMovimientoDetalle = n.v_IdMovimientoDetalle,
                         v_IdProducto = n.v_IdProducto,
                         v_NombreProducto = n.v_NombreProducto,
                         t_Fecha = n.t_Fecha,
                         i_IdTipoMovimiento = n.i_IdTipoMovimiento,
                         d_Cantidad = n.d_Cantidad == 0 || int.Parse(n.ValorUM) == 0 ? 0 : FormatoCant == (int)FormatoCantidad.UnidadMedidaProducto ? n.d_Cantidad / int.Parse(n.ValorUM) : n.d_Cantidad,
                         d_Precio = n.d_Cantidad == 0 || int.Parse(n.ValorUM) == 0 || n.d_Precio == 0 ? 0 : FormatoCant == (int)FormatoCantidad.UnidadMedidaProducto ? n.d_Cantidad == 0 ? 0 : n.d_Precio / (n.d_Cantidad / (int.Parse(n.ValorUM))) : n.d_Cantidad == 0 ? 0 : n.d_Precio / n.d_Cantidad,
                         d_PrecioDolares = n.d_Cantidad == 0 || int.Parse(n.ValorUM) == 0 || n.d_PrecioDolares == 0 ? 0 : FormatoCant == (int)FormatoCantidad.UnidadMedidaProducto ? n.d_Cantidad == 0 ? 0 : n.d_PrecioDolares / (n.d_Cantidad / (int.Parse(n.ValorUM))) : n.d_Cantidad == 0 ? 0 : n.d_PrecioDolares / n.d_Cantidad,
                         i_EsDevolucion = n.i_EsDevolucion,
                         i_IdTipoMotivo = n.i_IdTipoMotivo,
                         v_IdLinea = n.v_IdLinea,
                         IdMoneda = n.IdMoneda,
                         TipoCambio = n.TipoCambio,
                         Almacen = n.Almacen,
                         ClienteProveedor = n.ClienteProveedor,
                         Guia = n.Guia,
                         Documento = n.Documento,
                         IdAlmacen = n.IdAlmacen,
                         TipoMotivo = n.TipoMotivo,
                         IdAlmacenOrigen = n.IdAlmacenOrigen,
                         i_IdTipoDocumentoDetalle = n.i_IdTipoDocumentoDetalle,
                         EsDocumentoInterno = n.EsDocumentoInterno,
                         EsDocInverso = n.EsDocInverso,
                         Origen = n.Origen,
                         d_Total = n.d_Total,
                         d_TotalDolares = n.d_TotalDolares,
                         NombreProducto = n.NombreProducto,
                         Movimiento = n.Movimiento,
                         Correlativo = n.Correlativo,
                         Numeropedido = n.Numeropedido,
                         UnidadMedida = n.UnidadMedida,
                         Establecimiento = n.Establecimiento,
                         Grupollave = n.Grupollave,
                         Grupollave2 = n.Grupollave2,
                         Cuenta = n.Cuenta,
                         LetrasSubTotalLinea = n.LetrasSubTotalLinea,
                         NombreLinea = n.NombreLinea,
                         SoloCuenta = n.SoloCuenta,

                     }).ToList();

                     List<ReporteListadoSalidaAlmacenAnalitico> Lista = new List<ReporteListadoSalidaAlmacenAnalitico>();

                     int Contador = queryFinal.Count();
                     int NumeroElmento = 0;
                     int PosicionProducto = 0;
                     decimal UltimoPrecioSaldo = 0;
                     decimal UltimoPrecioSaldoDolares = 0;
                     int i = 0;
                     foreach (var item in queryFinal)
                     {
                         if (ConsideraDocInternos == 1 ||
                             (ConsideraDocInternos == 0 && queryFinal[i].EsDocumentoInterno == 0))
                         {

                             var oKardexList = new ReporteListadoSalidaAlmacenAnalitico();
                             oKardexList = (ReporteListadoSalidaAlmacenAnalitico)item.Clone();
                             oKardexList.Fecha = item.t_Fecha.ToShortDateString();

                             oKardexList.Correlativo = item.i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso &&
                                                       queryFinal[i].i_IdTipoMotivo == 5
                                 ? "INICIAL" + " " + item.Correlativo
                                 : queryFinal[i].i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso &&
                                   queryFinal[i].i_IdTipoMotivo != 5
                                     ? "N/I" + " " + item.Correlativo
                                     : "N/S" + " " + item.Correlativo;
                             CantidadValorizadas objProceso = new CantidadValorizadas();

                             if (!Consumo)
                             {
                                 objProceso = ProcesoValorizacionProductoAlmacenPedidoNotasSalida(
                                     ref objOperationResult, item.i_IdTipoMovimiento, IdProductoOld,
                                     item.v_NombreProducto, item.IdAlmacen.ToString(), pintMoneda, item.Origen,
                                     item.i_EsDevolucion, item.d_Cantidad.Value, item.IdMoneda, item.d_Precio ?? 0,
                                     PosicionProducto, item.TipoCambio, ref UltimoPrecioSaldo, NumeroElmento, Lista,
                                     item.d_Total ?? 0, item.d_TotalDolares ?? 0, item.d_PrecioDolares ?? 0,
                                     ref UltimoPrecioSaldoDolares);
                             }
                             else
                             {

                                 objProceso = ProcesoValorizacionProductoAlmacenPedidoNotasSalida(
                                     ref objOperationResult, item.i_IdTipoMovimiento, IdProductoOld,
                                     item.v_NombreProducto, item.IdAlmacen.ToString(), pintMoneda, item.Origen,
                                     item.i_EsDevolucion, item.d_Cantidad.Value, item.IdMoneda, item.d_Precio ?? 0,
                                     PosicionProducto, item.TipoCambio, ref UltimoPrecioSaldo, NumeroElmento, Lista,
                                     item.d_Total ?? 0, item.d_TotalDolares ?? 0, item.d_PrecioDolares ?? 0,
                                     ref UltimoPrecioSaldoDolares);

                             }
                             oKardexList.Ingreso_Cantidad = objProceso.Ingreso_Cantidad;
                             oKardexList.Ingreso_Precio = objProceso.Ingreso_Precio;
                             oKardexList.Ingreso_Total = objProceso.Ingreso_Total;

                             oKardexList.Ingreso_CantidadDolares = objProceso.Ingreso_CantidadDolares;
                             oKardexList.Ingreso_PrecioDolares = objProceso.Ingreso_PrecioDolares;
                             oKardexList.Ingreso_TotalDolares = objProceso.Ingreso_TotalDolares;


                             oKardexList.Saldo_Cantidad = objProceso.Saldo_Cantidad;
                             oKardexList.Saldo_Precio = objProceso.Saldo_Precio;
                             oKardexList.Saldo_Total = objProceso.Saldo_Total;

                             oKardexList.Saldo_CantidadDolares = objProceso.Saldo_CantidadDolares;
                             oKardexList.Saldo_PrecioDolares = objProceso.Saldo_PrecioDolares;
                             oKardexList.Saldo_TotalDolares = objProceso.Saldo_TotalDolares;


                             oKardexList.Salida_Cantidad = objProceso.Salida_Cantidad;
                             oKardexList.Salida_Precio = objProceso.Salida_Precio;
                             oKardexList.Salida_Total = objProceso.Salida_Total;

                             oKardexList.Saldo_CantidadDolares = objProceso.Saldo_CantidadDolares;
                             oKardexList.Salida_PrecioDolares = objProceso.Salida_PrecioDolares;
                             oKardexList.Salida_TotalDolares = objProceso.Salida_TotalDolares;



                             oKardexList.NumeroElemento = objProceso.NumeroElemento;
                             IdProductoOld = item.v_NombreProducto + " " + item.IdAlmacen;

                             if (Debe)
                             {
                                 oKardexList.DebeSoles = item.i_EsDevolucion == 0
                                     ? oKardexList.Salida_Total == null
                                         ? 0
                                         : Utils.Windows.DevuelveValorRedondeado(oKardexList.Salida_Total.Value, 2)
                                     : 0;
                                 oKardexList.CuentaDebe = item.SoloCuenta;
                                 oKardexList.DebeDolares = item.i_EsDevolucion == 0
                                     ? oKardexList.Salida_TotalDolares == null
                                         ? 0
                                         : Utils.Windows.DevuelveValorRedondeado(oKardexList.Salida_TotalDolares.Value, 2)
                                     : 0;
                                 oKardexList.HaberSoles = item.i_EsDevolucion == 1
                                     ? oKardexList.Salida_Total == null
                                         ? 0
                                         : Utils.Windows.DevuelveValorRedondeado(oKardexList.Salida_Total.Value, 2) * -1
                                     : 0;
                                 oKardexList.HaberDolares = item.i_EsDevolucion == 1
                                     ? oKardexList.Salida_TotalDolares == null
                                         ? 0
                                         : Utils.Windows.DevuelveValorRedondeado(oKardexList.Salida_TotalDolares.Value, 2) *
                                           -1
                                     : 0;
                             }
                             else
                             {
                                 oKardexList.DebeSoles = item.i_EsDevolucion == 1
                                     ? oKardexList.Salida_Total == null
                                         ? 0
                                         : Utils.Windows.DevuelveValorRedondeado(oKardexList.Salida_Total.Value, 2) * -1
                                     : 0;
                                 oKardexList.CuentaHaber = item.SoloCuenta;
                                 oKardexList.DebeDolares = item.i_EsDevolucion == 1
                                     ? oKardexList.Salida_TotalDolares == null
                                         ? 0
                                         : Utils.Windows.DevuelveValorRedondeado(oKardexList.Salida_TotalDolares.Value, 2) *
                                           -1
                                     : 0;
                                 oKardexList.HaberSoles = item.i_EsDevolucion == 0
                                     ? oKardexList.Salida_Total == null
                                         ? 0
                                         : Utils.Windows.DevuelveValorRedondeado(oKardexList.Salida_Total.Value, 2)
                                     : 0;
                                 oKardexList.HaberDolares = item.i_EsDevolucion == 0
                                     ? oKardexList.Salida_TotalDolares == null
                                         ? 0
                                         : Utils.Windows.DevuelveValorRedondeado(oKardexList.Salida_TotalDolares.Value, 2)
                                     : 0;

                             }


                             if (i + 1 < Contador &&
                                 IdProductoOld == queryFinal[i + 1].v_NombreProducto + " " + queryFinal[i + 1].IdAlmacen)
                             {
                                 PosicionProducto = PosicionProducto + 1;
                             }
                             else
                             {
                                 PosicionProducto = 0;
                                 UltimoPrecioSaldo = 0;
                                 UltimoPrecioSaldoDolares = 0;
                             }
                             Lista.Add(oKardexList);
                         }

                         i = i + 1;
                     }
                     objOperationResult.Success = 1;
                     var ReporteFinal =
                         Lista.Where(k => k.i_IdTipoMovimiento == 2)
                             .ToList()
                             .Where(k => k.t_Fecha >= pstrt_FechaRegistroIni && k.t_Fecha <= pstrt_FechaRegistroFin.Value)
                             .ToList()
                             .AsQueryable();

                     return ReporteFinal.ToList();
                 }
             }
             catch (Exception ex)
             {
                 objOperationResult.Success = 0;
                 objOperationResult.ExceptionMessage = ex.Message;
                 return null;
             }
         }

         */


        public List<ReporteListadoSalidaAlmacenAnalitico> ReporteListadoSalidaAlmacenAnalitico(ref OperationResult objOperationResult, DateTime? pstrt_FechaRegistroIni, DateTime? pstrt_FechaRegistroFin, string pstrFilterExpression, string pstr_CodInterno, string pstr_CodLinea, int pintIdTipoMotivo, string pstrIdCliente, string pstrNumPedido, string pstrt_Orden, string pstr_grupollave, string pstr_Nombregrupollave, string pstr_grupollave2, string pstr_Nombregrupollave2, string NombreEmpresa, string RucEmpresa, int pintMoneda, int IdEstablecimento, bool IncluirNroPedido, bool Debe = false, bool Consumo = false, bool ReporteCostoComercializacion = false)
        {
            try
            {
                int FormatoCant = (int)FormatoCantidad.Unidades;
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    DateTime FechaInicioValorizar = DateTime.Parse("01/01/" + Globals.ClientSession.i_Periodo);
                    int ConsideraDocInternos = 0;

                    string IdProductoOld = "";
                    var query = (from A in dbContext.movimientodetalle
                                 join B in dbContext.productodetalle on
                                 new { IdProductoDetalle = A.v_IdProductoDetalle, eliminado = 0 } equals
                                 new { IdProductoDetalle = B.v_IdProductoDetalle, eliminado = B.i_Eliminado.Value } into B_join
                                 from B in B_join.DefaultIfEmpty()
                                 join C in dbContext.producto on new { IdProducto = B.v_IdProducto, eliminado = 0 } equals
                                 new { IdProducto = C.v_IdProducto, eliminado = C.i_Eliminado.Value } into C_join
                                 from C in C_join.DefaultIfEmpty()
                                 join D in dbContext.movimiento on new { IdMovimiento = A.v_IdMovimiento, eliminado = 0 } equals
                                 new { IdMovimiento = D.v_IdMovimiento, eliminado = D.i_Eliminado.Value } into D_join
                                 from D in D_join.DefaultIfEmpty()
                                 join E in dbContext.cliente on new { IdCliente = D.v_IdCliente, eliminado = 0 } equals
                                 new { IdCliente = E.v_IdCliente, eliminado = E.i_Eliminado.Value } into E_join
                                 from E in E_join.DefaultIfEmpty()
                                 join F in dbContext.establecimientoalmacen on
                                 new { IdAlmacenOrigen = D.i_IdAlmacenOrigen, eliminado = 0 } equals
                                 new { IdAlmacenOrigen = F.i_IdAlmacen, eliminado = F.i_Eliminado.Value } into F_join
                                 from F in F_join.DefaultIfEmpty()
                                 join G in dbContext.almacen on new { IdAlmacen = F.i_IdAlmacen.Value, eliminado = 0 } equals
                                 new { IdAlmacen = G.i_IdAlmacen, eliminado = G.i_Eliminado.Value } into G_join
                                 from G in G_join.DefaultIfEmpty()
                                 join H in dbContext.documento on new { IdTipoDoc = A.i_IdTipoDocumento.Value, eliminado = 0 }
                                 equals new { IdTipoDoc = H.i_CodigoDocumento, eliminado = H.i_Eliminado.Value } into H_join
                                 from H in H_join.DefaultIfEmpty()

                                 join I in dbContext.datahierarchy on
                                 new { Grupo = 17, eliminado = 0, UnidadMedida = C.i_IdUnidadMedida.Value } equals
                                 new { Grupo = I.i_GroupId, eliminado = I.i_IsDeleted.Value, UnidadMedida = I.i_ItemId } into
                                 I_join
                                 from I in I_join.DefaultIfEmpty()

                                 join M in dbContext.datahierarchy on
                                 new { Motivo = D.i_IdTipoMotivo.Value, Grupo = 20, eliminado = 0 }
                                 equals new { Motivo = M.i_ItemId, Grupo = M.i_GroupId, eliminado = M.i_IsDeleted.Value } into
                                 M_join
                                 from M in M_join.DefaultIfEmpty()

                                 join N in dbContext.establecimiento on new { estab = F.i_IdEstablecimiento.Value, eliminado = 0 }
                                 equals new { estab = N.i_IdEstablecimiento, eliminado = N.i_Eliminado.Value } into N_join
                                 from N in N_join.DefaultIfEmpty()

                                 join O in dbContext.linea on new { l = C.v_IdLinea, eliminado = 0 } equals
                                 new { l = O.v_IdLinea, eliminado = O.i_Eliminado.Value } into O_join
                                 from O in O_join.DefaultIfEmpty()

                                 where A.i_Eliminado == 0 && D.i_IdTipoMovimiento != (int)TipoDeMovimiento.Transferencia
                                       && (F.i_IdEstablecimiento == IdEstablecimento || IdEstablecimento == -1) &&
                                       (C.v_CodInterno == pstr_CodInterno || pstr_CodInterno == "") &&
                                       (A.v_NroPedido == pstrNumPedido || pstrNumPedido == "")
                                       && (D.t_Fecha.Value >= FechaInicioValorizar && D.t_Fecha.Value <= pstrt_FechaRegistroFin.Value) && C.i_EsServicio == 0
                                       && C.i_EsActivoFijo == 0 &&
                                       (D.v_OrigenTipo == null || D.v_OrigenTipo == "" ||
                                        D.v_OrigenTipo == Constants.OrigenCompra || D.v_OrigenTipo == Constants.OrigenImportacion ||
                                        D.v_OrigenTipo == Constants.OrigenTransferencia ||
                                        D.v_OrigenTipo == Constants.OrigenVenta || D.v_OrigenTipo == Constants.OrigenGuiaInterna ||
                                        (!string.IsNullOrEmpty(D.v_OrigenTipo) && D.v_OrigenTipo != Constants.OrigenGuiaCompra))
                                 // No toma en cuentas las GUIAS DE COMPRA
                                 select new ReporteListadoSalidaAlmacenAnalitico
                                 {
                                     v_IdMovimientoDetalle = A.v_IdMovimientoDetalle,
                                     v_IdProducto = C == null ? "" : C.v_IdProducto,
                                     // NombreProducto = C == null ? "" : C.v_CodInterno + "      " + C.v_Descripcion,
                                     NombreProducto = C == null ? "" : C.v_CodInterno + " " + C.v_Descripcion.Trim(),
                                     v_NombreProducto = IncluirNroPedido ? string.IsNullOrEmpty(A.v_NroPedido) ? "PRODUCTO :" + C.v_CodInterno.Trim() + " " + C.v_Descripcion.Trim() + "  PEDIDO :" : "PRODUCTO :" + C.v_CodInterno.Trim() + " " + C.v_Descripcion.Trim() + "  PEDIDO :" + A.v_NroPedido.Trim() : "PRODUCTO :" + C.v_CodInterno.Trim() + " " + C.v_Descripcion.Trim(),
                                     i_IdTipoMovimiento = D.i_IdTipoMovimiento.Value,
                                     i_IdTipoMotivo = D.i_IdTipoMotivo.Value,
                                     v_IdLinea = C.v_IdLinea,
                                     Almacen = G.v_Nombre,
                                     Guia = A.v_NroGuiaRemision,
                                     Documento = H == null ? A.v_NumeroDocumento : H.v_Siglas.Trim() + " " + A.v_NumeroDocumento.Trim(),
                                     IdAlmacen = D.i_IdAlmacenOrigen.Value,
                                     TipoMotivo = D.i_IdTipoMotivo == 5 ? 1 : 2,
                                     IdAlmacenOrigen = D.i_IdAlmacenOrigen.Value,
                                     i_IdTipoDocumentoDetalle = A.i_IdTipoDocumento == null ? -1 : A.i_IdTipoDocumento.Value,
                                     EsDocumentoInterno = H.i_UsadoDocumentoInterno == null ? 0 : H.i_UsadoDocumentoInterno.Value,
                                     EsDocInverso = H.i_UsadoDocumentoInverso == null ? 0 : H.i_UsadoDocumentoInverso.Value,
                                     ValorUM = I.v_Value2,
                                     v_IdMarca = C.v_IdMarca,
                                     Origen = D.v_OrigenTipo,
                                     IdMovimiento = A.v_IdMovimiento,
                                     t_Fecha = D.t_Fecha.Value,
                                     Movimiento = M == null ? "" : M.v_Value2,
                                     Periodo = D.v_Periodo,
                                     Mes = D.v_Mes,
                                     Correlativo = D.v_Mes + "-" + D.v_Correlativo,
                                     IdProducto = G == null ? "" : C.v_CodInterno,
                                     UnidadMedida = I == null ? "**U.M. NO EXISTE**" : I.v_Value1,
                                     TipoCambio = D.d_TipoCambio.Value,
                                     NombreMoneda = "",
                                     IdMoneda = D.i_IdMoneda.Value,
                                     Establecimiento = "ESTABLECIMIENTO :" + N.v_Nombre,
                                     NombreLinea = C == null ? "**LINEA NO EXISTE**" : O.v_Nombre ?? "",
                                     MotivoSalida = M.v_Value1 ?? "",
                                     ClienteProveedor = D.v_OrigenTipo == "T" ? D.v_Glosa == null || D.v_Glosa == string.Empty ? "TRANSFERENCIA " : D.v_Glosa : E == null ? "**CLIENTE/PROVEEDOR  NO EXISTE**" : (E.v_ApePaterno + " " + E.v_ApeMaterno + " " + E.v_PrimerNombre + " " + E.v_RazonSocial).Trim(),
                                     NumeroGuiaRemision = A.v_NroGuiaRemision,
                                     Numeropedido = IncluirNroPedido ? A.v_NroPedido ?? "" : "",
                                     Grupollave = pstr_grupollave == "NOMBREPRODUCTO" ? G == null ? "** " + pstr_Nombregrupollave + " NO EXISTE **" : pstr_Nombregrupollave + " : " + C.v_Descripcion : "" + pstr_grupollave == "NOMBRECLIENTE" ? I == null ? "** " + pstr_Nombregrupollave + " NO EXISTE **" : pstr_Nombregrupollave + " : " + (E.v_CodCliente.Trim() + " " + E.v_PrimerNombre + " " + E.v_ApePaterno + " " + E.v_ApeMaterno + " " + E.v_RazonSocial).Trim() : "" + pstr_grupollave == "NOMBRELINEA" ? H == null ? "** " + pstr_Nombregrupollave + " NO EXISTE **" : pstr_Nombregrupollave + " : " + H.v_Nombre : "" + pstr_grupollave == "MOVIMIENTO" ? M == null ? "** " + pstr_Nombregrupollave + " NO EXISTE **" : "MOVIMIENTO :" + M.v_Value2 : "" + pstr_grupollave == "NUMEROPEDIDO" ? E == null ? "** " + pstr_Nombregrupollave + " NO EXISTE **" : pstr_Nombregrupollave + " : " + A.v_NroPedido : "" + pstr_grupollave == "MOTIVOSALIDA" ? A == null ? "** " + pstr_Nombregrupollave + " NO EXISTE **" : pstr_Nombregrupollave + " : " + M.v_Value1 : "" + pstr_grupollave == "NOMBRELINEA/NOMBREPRODUCTO" ? H == null ? "** " + "LÍNEA" + " NO EXISTE **" : "LINEA" + " : " + H.v_Nombre.Trim() : "",
                                     Grupollave2 = pstr_grupollave2 == "NOMBREPRODUCTO" ? G == null ? "** " + pstr_Nombregrupollave2 + " NO EXISTE **" : pstr_Nombregrupollave2 + " : " + C.v_Descripcion : "" + pstr_grupollave2 == "NOMBRECLIENTE" ? I == null ? "** " + pstr_Nombregrupollave2 + " NO EXISTE **" : pstr_Nombregrupollave2 + " : " + (E.v_CodCliente.Trim() + " " + E.v_PrimerNombre + " " + E.v_ApePaterno + " " + E.v_ApeMaterno + " " + E.v_RazonSocial).Trim() : "" + pstr_grupollave2 == "NOMBRELINEA" ? H == null ? "** " + pstr_Nombregrupollave2 + " NO EXISTE **" : pstr_Nombregrupollave2 + " : " + H.v_Nombre : "" + pstr_grupollave2 == "MOVIMIENTO" ? M == null ? "** " + pstr_Nombregrupollave2 + " NO EXISTE **" : "MOVIMIENTO :" + M.v_Value2 : "" + pstr_grupollave2 == "NUMEROPEDIDO" ? E == null ? "** " + pstr_Nombregrupollave2 + " NO EXISTE **" : pstr_Nombregrupollave2 + " : " + A.v_NroPedido : "" + pstr_grupollave2 == "MOTIVOSALIDA" ? A == null ? "** " + pstr_Nombregrupollave2 + " NO EXISTE **" : pstr_Nombregrupollave2 + " : " + M.v_Value1 : "" + pstr_grupollave2 == "NOMBRELINEA/NOMBREPRODUCTO" ? G == null ? "** " + "DESCRIPCIÓN" + " NO EXISTE **" : "DESCRIPCICIÓN" + " : " + C.v_Descripcion.Trim() : "",
                                     i_EsDevolucion = D.i_EsDevolucion ?? 0,
                                     d_Total = D.v_OrigenTipo == "I" ? pintMoneda == -1 ? D.i_IdMoneda == (int)Currency.Soles ? A.d_Total : A.d_TotalCambio : pintMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : D.i_IdMoneda == (int)Currency.Soles ? A.d_Total : A.d_Total,
                                     d_TotalDolares = D.v_OrigenTipo == "I" ? pintMoneda == -1 ? D.i_IdMoneda == (int)Currency.Dolares ? A.d_Total : A.d_TotalCambio : pintMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : D.i_IdMoneda == (int)Currency.Dolares ? A.d_Total : (D.d_TipoCambio == 0 ? 0 : A.d_Total / D.d_TipoCambio),
                                     d_Cantidad = A.d_CantidadEmpaque == null ? A.d_Cantidad == null ? 0 : A.d_Cantidad : A.d_CantidadEmpaque,
                                     d_Precio = D.v_OrigenTipo == "I" ? pintMoneda == -1 ? D.i_IdMoneda == (int)Currency.Soles ? A.d_Total : A.d_TotalCambio : pintMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : D.i_IdMoneda == (int)Currency.Soles ? A.d_Total : A.d_Total,
                                     d_PrecioDolares = D.v_OrigenTipo == "I" ? pintMoneda == -1 ? D.i_IdMoneda == (int)Currency.Dolares ? A.d_Total : A.d_TotalCambio : pintMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : D.i_IdMoneda == (int)Currency.Dolares ? A.d_Total : (D.d_TipoCambio == 0 ? 0 : A.d_Total / D.d_TipoCambio),
                                     CodigoProducto = C.v_CodInterno,
                                     Cuenta = Debe ? D.i_EsDevolucion == 1 ? "CUENTA :" + O.v_NroCuentaHConsumo : "CUENTA :" + O.v_NroCuentaDConsumo : D.i_EsDevolucion == 1 ? "CUENTA :" + O.v_NroCuentaDConsumo : "CUENTA :" + O.v_NroCuentaHConsumo,
                                     SoloCuenta = Debe ? D.i_EsDevolucion == 1 ? O.v_NroCuentaHConsumo : O.v_NroCuentaDConsumo : D.i_EsDevolucion == 1 ? O.v_NroCuentaDConsumo : O.v_NroCuentaHConsumo,
                                     LetrasSubTotalLinea = "SUB- TOTAL CUENTA : " + O.v_NroCuentaDConsumo + " : ",
                                     AuxiliarNroPedido = string.IsNullOrEmpty(A.v_NroPedido) ? "" : A.v_NroPedido.Trim(),

                                 }).ToList().AsQueryable();



                    if (!string.IsNullOrEmpty(pstrFilterExpression))
                    {
                        query = query.Where(pstrFilterExpression);
                    }
                    if (ReporteCostoComercializacion)
                    {
                        query = query.Where(o => !string.IsNullOrEmpty(o.AuxiliarNroPedido));
                        query = query.Where(o => !o.NombreProducto.Contains("FLETE"));
                    }
                    var queryFinal =
                        query.ToList()
                            .OrderBy(x => x.IdAlmacenOrigen)
                            .ThenBy(x => x.v_NombreProducto)
                            .ThenBy(x => x.t_Fecha)
                            .ThenBy(x => x.i_IdTipoMovimiento)
                            .ThenBy(x => x.TipoMotivo)
                            .ThenBy(x => x.NroPedido)
                            .ToList();


                    queryFinal = queryFinal.ToList().Select(n => new ReporteListadoSalidaAlmacenAnalitico
                    {
                        v_IdMovimientoDetalle = n.v_IdMovimientoDetalle,
                        v_IdProducto = n.v_IdProducto,
                        v_NombreProducto = n.v_NombreProducto,
                        t_Fecha = n.t_Fecha,
                        i_IdTipoMovimiento = n.i_IdTipoMovimiento,
                        d_Cantidad = n.d_Cantidad == 0 || int.Parse(n.ValorUM) == 0 ? 0 : FormatoCant == (int)FormatoCantidad.UnidadMedidaProducto ? n.d_Cantidad / int.Parse(n.ValorUM) : n.d_Cantidad,
                        d_Precio = n.d_Cantidad == 0 || int.Parse(n.ValorUM) == 0 || n.d_Precio == 0 ? 0 : FormatoCant == (int)FormatoCantidad.UnidadMedidaProducto ? n.d_Cantidad == 0 ? 0 : n.d_Precio / (n.d_Cantidad / (int.Parse(n.ValorUM))) : n.d_Cantidad == 0 ? 0 : n.d_Precio / n.d_Cantidad,
                        d_PrecioDolares = n.d_Cantidad == 0 || int.Parse(n.ValorUM) == 0 || n.d_PrecioDolares == 0 ? 0 : FormatoCant == (int)FormatoCantidad.UnidadMedidaProducto ? n.d_Cantidad == 0 ? 0 : n.d_PrecioDolares / (n.d_Cantidad / (int.Parse(n.ValorUM))) : n.d_Cantidad == 0 ? 0 : n.d_PrecioDolares / n.d_Cantidad,
                        i_EsDevolucion = n.i_EsDevolucion,
                        i_IdTipoMotivo = n.i_IdTipoMotivo,
                        v_IdLinea = n.v_IdLinea,
                        IdMoneda = n.IdMoneda,
                        TipoCambio = n.TipoCambio,
                        Almacen = n.Almacen,
                        ClienteProveedor = n.ClienteProveedor,
                        Guia = n.Guia,
                        Documento = n.Documento,
                        IdAlmacen = n.IdAlmacen,
                        TipoMotivo = n.TipoMotivo,
                        IdAlmacenOrigen = n.IdAlmacenOrigen,
                        i_IdTipoDocumentoDetalle = n.i_IdTipoDocumentoDetalle,
                        EsDocumentoInterno = n.EsDocumentoInterno,
                        EsDocInverso = n.EsDocInverso,
                        Origen = n.Origen,
                        d_Total = n.d_Total,
                        d_TotalDolares = n.d_TotalDolares,
                        NombreProducto = n.NombreProducto,
                        Movimiento = n.Movimiento,
                        Correlativo = n.Correlativo,
                        Numeropedido = n.Numeropedido,
                        UnidadMedida = n.UnidadMedida,
                        Establecimiento = n.Establecimiento,
                        Grupollave = n.Grupollave,
                        Grupollave2 = n.Grupollave2,
                        Cuenta = n.Cuenta,
                        LetrasSubTotalLinea = n.LetrasSubTotalLinea,
                        NombreLinea = n.NombreLinea,
                        SoloCuenta = n.SoloCuenta,
                        AuxiliarNroPedido = n.AuxiliarNroPedido,

                    }).ToList();

                    List<ReporteListadoSalidaAlmacenAnalitico> Lista = new List<ReporteListadoSalidaAlmacenAnalitico>();

                    int Contador = queryFinal.Count();
                    int NumeroElmento = 0;
                    int PosicionProducto = 0;
                    decimal UltimoPrecioSaldo = 0;
                    decimal UltimoPrecioSaldoDolares = 0;
                    int i = 0;
                    foreach (var item in queryFinal)
                    {
                        if (ConsideraDocInternos == 1 ||
                            (ConsideraDocInternos == 0 && queryFinal[i].EsDocumentoInterno == 0))
                        {

                            var oKardexList = new ReporteListadoSalidaAlmacenAnalitico();
                            oKardexList = (ReporteListadoSalidaAlmacenAnalitico)item.Clone();
                            oKardexList.Fecha = item.t_Fecha.ToShortDateString();

                            oKardexList.Correlativo = item.i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso &&
                                                      queryFinal[i].i_IdTipoMotivo == 5
                                ? "INICIAL" + " " + item.Correlativo
                                : queryFinal[i].i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso &&
                                  queryFinal[i].i_IdTipoMotivo != 5
                                    ? "N/I" + " " + item.Correlativo
                                    : "N/S" + " " + item.Correlativo;
                            if (i == 0)
                            {

                                if ((oKardexList.i_IdTipoMotivo != 5 && oKardexList.i_IdTipoMovimiento == 1) || oKardexList.i_IdTipoMovimiento == 2)
                                {
                                    ReporteListadoSalidaAlmacenAnalitico oKardexList1 = new ReporteListadoSalidaAlmacenAnalitico();
                                    oKardexList1 = (ReporteListadoSalidaAlmacenAnalitico)oKardexList.Clone();
                                    //oKardexList1.v_NombreTipoMovimiento = "INICIAL";
                                    DateTime date = DateTime.Parse("01/" + pstrt_FechaRegistroIni.Value.Month.ToString("00") + "/" + pstrt_FechaRegistroIni.Value.Year.ToString());
                                    oKardexList1.t_Fecha = date;
                                    //oKardexList1.DocumentoInventarioSunat = "";
                                    //oKardexList1.v_Fecha = date.ToString("dd-MMM");
                                    //oKardexList1.DocumentoInventarioSunat = "";
                                    oKardexList1.Guia = "";
                                    oKardexList1.Documento = "";
                                    oKardexList1.ClienteProveedor = "";
                                    oKardexList1.Ingreso_Cantidad = 0;
                                    oKardexList1.Ingreso_Precio = 0;
                                    oKardexList1.Ingreso_Total = 0;
                                    oKardexList1.Salida_Cantidad = null;
                                    oKardexList1.i_IdTipoMotivo = 5;
                                    oKardexList1.i_IdTipoMovimiento = 1;
                                    oKardexList1.Salida_Precio = 0;
                                    oKardexList1.Salida_Total = 0;
                                    oKardexList1.Saldo_Cantidad = 0;
                                    oKardexList1.Saldo_Precio = 0;
                                    oKardexList1.Saldo_Total = 0;
                                    oKardexList1.Ingreso_CantidadDolares = 0;
                                    oKardexList1.Ingreso_PrecioDolares = 0;
                                    oKardexList1.Ingreso_TotalDolares = 0;
                                    oKardexList1.Saldo_CantidadDolares = 0;
                                    oKardexList1.Saldo_PrecioDolares = 0;
                                    oKardexList1.Saldo_TotalDolares = 0;
                                    oKardexList1.Salida_PrecioDolares = 0;
                                    oKardexList1.Salida_TotalDolares = 0;
                                    oKardexList1.NumeroElemento = 0;
                                    Lista.Add(oKardexList1);
                                    PosicionProducto = PosicionProducto + 1;
                                    IdProductoOld = oKardexList1.v_NombreProducto + " " + oKardexList1.IdAlmacen;

                                }

                            }

                            CantidadValorizadas objProceso = new CantidadValorizadas();

                            if (!Consumo)
                            {
                                objProceso = Globals.ClientSession.i_Periodo == 2016 ? ProcesoValorizacionProductoAlmacenPedidoNotasSalida2016(
                                    ref objOperationResult, item.i_IdTipoMovimiento, IdProductoOld,
                                    item.v_NombreProducto, item.IdAlmacen.ToString(), pintMoneda, item.Origen,
                                    item.i_EsDevolucion, item.d_Cantidad.Value, item.IdMoneda, item.d_Precio ?? 0,
                                    PosicionProducto, item.TipoCambio, ref UltimoPrecioSaldo, NumeroElmento, Lista,
                                    item.d_Total ?? 0, item.d_TotalDolares ?? 0, item.d_PrecioDolares ?? 0,
                                    ref UltimoPrecioSaldoDolares) : ProcesoValorizacionProductoAlmacenPedidoNotasSalida2017(
                                    ref objOperationResult, item.i_IdTipoMovimiento, IdProductoOld,
                                    item.v_NombreProducto, item.IdAlmacen.ToString(), pintMoneda, item.Origen,
                                    item.i_EsDevolucion, item.d_Cantidad.Value, item.IdMoneda, item.d_Precio ?? 0,
                                    PosicionProducto, item.TipoCambio, ref UltimoPrecioSaldo, NumeroElmento, Lista,
                                    item.d_Total ?? 0, item.d_TotalDolares ?? 0, item.d_PrecioDolares ?? 0,
                                    ref UltimoPrecioSaldoDolares);
                                if (objOperationResult.Success == 0)
                                {
                                    return null;
                                }
                            }
                            else
                            {

                                objProceso = Globals.ClientSession.i_Periodo == 2016 ? ProcesoValorizacionProductoAlmacenPedidoNotasSalida2016(
                                    ref objOperationResult, item.i_IdTipoMovimiento, IdProductoOld,
                                    item.v_NombreProducto, item.IdAlmacen.ToString(), pintMoneda, item.Origen,
                                    item.i_EsDevolucion, item.d_Cantidad.Value, item.IdMoneda, item.d_Precio ?? 0,
                                    PosicionProducto, item.TipoCambio, ref UltimoPrecioSaldo, NumeroElmento, Lista,
                                    item.d_Total ?? 0, item.d_TotalDolares ?? 0, item.d_PrecioDolares ?? 0,
                                    ref UltimoPrecioSaldoDolares) : ProcesoValorizacionProductoAlmacenPedidoNotasSalida2017(
                                    ref objOperationResult, item.i_IdTipoMovimiento, IdProductoOld,
                                    item.v_NombreProducto, item.IdAlmacen.ToString(), pintMoneda, item.Origen,
                                    item.i_EsDevolucion, item.d_Cantidad.Value, item.IdMoneda, item.d_Precio ?? 0,
                                    PosicionProducto, item.TipoCambio, ref UltimoPrecioSaldo, NumeroElmento, Lista,
                                    item.d_Total ?? 0, item.d_TotalDolares ?? 0, item.d_PrecioDolares ?? 0,
                                    ref UltimoPrecioSaldoDolares);
                                if (objOperationResult.Success == 0)
                                {
                                    return null;
                                }

                            }
                            oKardexList.Ingreso_Cantidad = objProceso.Ingreso_Cantidad;
                            oKardexList.Ingreso_Precio = objProceso.Ingreso_Precio;
                            oKardexList.Ingreso_Total = objProceso.Ingreso_Total;

                            oKardexList.Ingreso_CantidadDolares = objProceso.Ingreso_CantidadDolares;
                            oKardexList.Ingreso_PrecioDolares = objProceso.Ingreso_PrecioDolares;
                            oKardexList.Ingreso_TotalDolares = objProceso.Ingreso_TotalDolares;


                            oKardexList.Saldo_Cantidad = objProceso.Saldo_Cantidad;
                            oKardexList.Saldo_Precio = objProceso.Saldo_Precio;
                            oKardexList.Saldo_Total = objProceso.Saldo_Total;

                            oKardexList.Saldo_CantidadDolares = objProceso.Saldo_CantidadDolares;
                            oKardexList.Saldo_PrecioDolares = objProceso.Saldo_PrecioDolares;
                            oKardexList.Saldo_TotalDolares = objProceso.Saldo_TotalDolares;


                            oKardexList.Salida_Cantidad = objProceso.Salida_Cantidad;
                            oKardexList.Salida_Precio = objProceso.Salida_Precio;
                            oKardexList.Salida_Total = objProceso.Salida_Total;


                            oKardexList.Salida_PrecioDolares = objProceso.Salida_PrecioDolares;
                            oKardexList.Salida_TotalDolares = objProceso.Salida_TotalDolares;



                            oKardexList.NumeroElemento = objProceso.NumeroElemento;
                            IdProductoOld = item.v_NombreProducto + " " + item.IdAlmacen;

                            if (Debe)
                            {
                                oKardexList.DebeSoles = item.i_EsDevolucion == 0
                                    ? oKardexList.Salida_Total == null
                                        ? 0
                                        : Utils.Windows.DevuelveValorRedondeado(oKardexList.Salida_Total.Value, 2)
                                    : 0;
                                oKardexList.CuentaDebe = item.SoloCuenta;
                                oKardexList.DebeDolares = item.i_EsDevolucion == 0
                                    ? oKardexList.Salida_TotalDolares == null
                                        ? 0
                                        : Utils.Windows.DevuelveValorRedondeado(oKardexList.Salida_TotalDolares.Value, 2)
                                    : 0;
                                oKardexList.HaberSoles = item.i_EsDevolucion == 1
                                    ? oKardexList.Salida_Total == null
                                        ? 0
                                        : Utils.Windows.DevuelveValorRedondeado(oKardexList.Salida_Total.Value, 2) * -1
                                    : 0;
                                oKardexList.HaberDolares = item.i_EsDevolucion == 1
                                    ? oKardexList.Salida_TotalDolares == null
                                        ? 0
                                        : Utils.Windows.DevuelveValorRedondeado(oKardexList.Salida_TotalDolares.Value, 2) *
                                          -1
                                    : 0;
                            }
                            else
                            {
                                oKardexList.DebeSoles = item.i_EsDevolucion == 1
                                    ? oKardexList.Salida_Total == null
                                        ? 0
                                        : Utils.Windows.DevuelveValorRedondeado(oKardexList.Salida_Total.Value, 2) * -1
                                    : 0;
                                oKardexList.CuentaHaber = item.SoloCuenta;
                                oKardexList.DebeDolares = item.i_EsDevolucion == 1
                                    ? oKardexList.Salida_TotalDolares == null
                                        ? 0
                                        : Utils.Windows.DevuelveValorRedondeado(oKardexList.Salida_TotalDolares.Value, 2) *
                                          -1
                                    : 0;
                                oKardexList.HaberSoles = item.i_EsDevolucion == 0
                                    ? oKardexList.Salida_Total == null
                                        ? 0
                                        : Utils.Windows.DevuelveValorRedondeado(oKardexList.Salida_Total.Value, 2)
                                    : 0;
                                oKardexList.HaberDolares = item.i_EsDevolucion == 0
                                    ? oKardexList.Salida_TotalDolares == null
                                        ? 0
                                        : Utils.Windows.DevuelveValorRedondeado(oKardexList.Salida_TotalDolares.Value, 2)
                                    : 0;

                            }


                            if (i + 1 < Contador && IdProductoOld == queryFinal[i + 1].v_NombreProducto + " " + queryFinal[i + 1].IdAlmacen)
                            {
                                ///Se agrego para valorizacion sunat
                                int Reg = Lista.Count();
                                //Se agrego para los saldos iniciales 
                                if (PosicionProducto == 0 && Reg > 0 && Lista[Reg - 1].v_NombreProducto + " " + Lista[Reg - 1].IdAlmacen != oKardexList.v_NombreProducto + " " + oKardexList.IdAlmacen)
                                {
                                    if (int.Parse(pstrt_FechaRegistroIni.Value.Month.ToString()) > 1)
                                    {
                                        ReporteListadoSalidaAlmacenAnalitico oKardexList1 = new ReporteListadoSalidaAlmacenAnalitico();
                                        oKardexList1 = (ReporteListadoSalidaAlmacenAnalitico)queryFinal[i + 1].Clone();
                                        if (oKardexList1.v_IdProducto == "N001-PD000000527")
                                        {
                                            string h = "";
                                        }
                                        // oKardexList1.v_NombreTipoMovimiento = "INICIAL";
                                        DateTime date = DateTime.Parse("01/" + pstrt_FechaRegistroIni.Value.Month.ToString("00") + "/" + pstrt_FechaRegistroIni.Value.Year.ToString());
                                        oKardexList1.t_Fecha = date;
                                        // oKardexList1.DocumentoInventarioSunat = "";
                                        // oKardexList1.v_Fecha = date.ToString("dd-MMM");
                                        // oKardexList1.DocumentoInventarioSunat = "";
                                        oKardexList1.Guia = "";
                                        oKardexList1.Documento = "";
                                        oKardexList1.ClienteProveedor = "";
                                        oKardexList1.Ingreso_Cantidad = 0;
                                        oKardexList1.Ingreso_Precio = 0;
                                        oKardexList1.Ingreso_Total = 0;
                                        oKardexList1.Salida_Cantidad = null;
                                        oKardexList1.i_IdTipoMotivo = 5;
                                        oKardexList1.i_IdTipoMovimiento = 1;
                                        oKardexList1.Salida_Precio = 0;
                                        oKardexList1.Salida_Total = 0;
                                        oKardexList1.Saldo_Cantidad = 0;
                                        oKardexList1.Saldo_Precio = 0;
                                        oKardexList1.Saldo_Total = 0;
                                        oKardexList1.NumeroElemento = 0;

                                        oKardexList1.Ingreso_CantidadDolares = 0;
                                        oKardexList1.Ingreso_PrecioDolares = 0;
                                        oKardexList1.Ingreso_TotalDolares = 0;
                                        oKardexList1.Saldo_CantidadDolares = 0;
                                        oKardexList1.Saldo_PrecioDolares = 0;
                                        oKardexList1.Saldo_TotalDolares = 0;
                                        oKardexList1.Salida_PrecioDolares = 0;
                                        oKardexList1.Salida_TotalDolares = 0;


                                        Lista.Add(oKardexList1);
                                        PosicionProducto = PosicionProducto + 1;

                                        IdProductoOld = oKardexList1.v_NombreProducto + " " + oKardexList1.IdAlmacen;
                                    }

                                }

                                // Hasta Acá;

                                PosicionProducto = PosicionProducto + 1;
                                Lista.Add(oKardexList);

                            }
                            else
                            {
                                PosicionProducto = 0;
                                UltimoPrecioSaldo = 0;
                                UltimoPrecioSaldoDolares = 0;
                                Lista.Add(oKardexList);
                                if (i + 1 < Contador)// !SeInsertoLista
                                {
                                    ReporteListadoSalidaAlmacenAnalitico oKardexList1 = new ReporteListadoSalidaAlmacenAnalitico();
                                    oKardexList1 = (ReporteListadoSalidaAlmacenAnalitico)queryFinal[i + 1].Clone();

                                    if (((queryFinal[i + 1].i_IdTipoMotivo != 5 && queryFinal[i + 1].i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso)) || queryFinal[i + 1].i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeSalida)
                                    {
                                        // oKardexList1.v_NombreTipoMovimiento = "INICIAL";
                                        DateTime date = DateTime.Parse("01/" + pstrt_FechaRegistroIni.Value.Month.ToString("00") + "/" + pstrt_FechaRegistroIni.Value.Year.ToString());
                                        // oKardexList1.v_Fecha = date.ToString("dd-MMM");
                                        oKardexList1.t_Fecha = date;
                                        // oKardexList1.DocumentoInventarioSunat = "";
                                        oKardexList1.Guia = "";
                                        oKardexList1.Documento = "";
                                        oKardexList1.ClienteProveedor = "";
                                        oKardexList1.Ingreso_Cantidad = 0;
                                        oKardexList1.Ingreso_Precio = 0;
                                        oKardexList1.Ingreso_Total = 0;
                                        oKardexList1.Salida_Cantidad = null;
                                        oKardexList1.i_IdTipoMotivo = 5;
                                        oKardexList1.Salida_Precio = 0;
                                        oKardexList1.Salida_Total = 0;
                                        oKardexList1.Saldo_Cantidad = 0;
                                        oKardexList1.Saldo_Precio = 0;
                                        oKardexList1.Saldo_Total = 0;
                                        oKardexList1.NumeroElemento = 0;
                                        // oKardexList1.TipoOperacionInventarioSunat = int.Parse(TipoMovimientos.Where(l => l.i_ItemId == 5).FirstOrDefault().v_Value2).ToString("00");
                                        oKardexList1.i_IdTipoMovimiento = 1;

                                        oKardexList1.Ingreso_CantidadDolares = 0;
                                        oKardexList1.Ingreso_PrecioDolares = 0;
                                        oKardexList1.Ingreso_TotalDolares = 0;
                                        oKardexList1.Saldo_CantidadDolares = 0;
                                        oKardexList1.Saldo_PrecioDolares = 0;
                                        oKardexList1.Saldo_TotalDolares = 0;
                                        oKardexList1.Salida_PrecioDolares = 0;
                                        oKardexList1.Salida_TotalDolares = 0;
                                        Lista.Add(oKardexList1);
                                        PosicionProducto = PosicionProducto + 1;
                                        IdProductoOld = oKardexList1.v_NombreProducto + " " + oKardexList1.IdAlmacen;
                                    }
                                }
                            }
                        }

                        i = i + 1;
                    }
                    objOperationResult.Success = 1;
                    var ReporteFinal =
                        Lista.Where(k => k.i_IdTipoMovimiento == 2)
                            .ToList()
                            .Where(k => k.t_Fecha >= pstrt_FechaRegistroIni && k.t_Fecha <= pstrt_FechaRegistroFin.Value)
                            .ToList()
                            .AsQueryable();

                    return ReporteFinal.ToList();
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public List<ReporteListadoIngresoAlmacenAnalitico> ReporteListadoIngresoAlmacenAnalitico_(int pstrIdMovimiento, int pintTipoMovimiento, DateTime? pstrt_FechaRegistroIni, DateTime? pstrt_FechaRegistroFin, int pstrIdAlmacen, string pstr_CodInterno, string pstr_CodLinea, int pintIdTipoMotivo, string pstrIdCliente, string pstrNumPedido, string pstrt_Orden, string pstr_grupollave, string pstr_Nombregrupollave, string pstr_grupollave2, string pstr_Nombregrupollave2)
        {
            try
            {

                int _b = pintTipoMovimiento == 1 ? 19 : 20;

                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    var query = (from n in dbContext.movimiento

                                 join A in dbContext.almacen on n.i_IdAlmacenOrigen equals A.i_IdAlmacen into A_join
                                 from A in A_join.DefaultIfEmpty()

                                 join B in dbContext.movimientodetalle on new { v_IdMovimiento = n.v_IdMovimiento }
                                 equals new { v_IdMovimiento = B.v_IdMovimiento } into B_join
                                 from B in B_join.DefaultIfEmpty()
                                 join C in dbContext.productodetalle on B.v_IdProductoDetalle equals C.v_IdProductoDetalle
                                 join D in dbContext.producto on new { v_IdProducto = C.v_IdProducto }
                                 equals new { v_IdProducto = D.v_IdProducto } into D_join
                                 from D in D_join.DefaultIfEmpty()
                                 join L in dbContext.linea on new { v_IdLinea = D.v_IdLinea }
                                 equals new { v_IdLinea = L.v_IdLinea } into L_join
                                 from L in L_join.DefaultIfEmpty()
                                 join E in dbContext.cliente on n.v_IdCliente equals E.v_IdCliente into E_join
                                 from E in E_join.DefaultIfEmpty()

                                 join G in dbContext.datahierarchy on B.i_IdUnidad equals G.i_ItemId into G_join
                                 from G in G_join.DefaultIfEmpty()
                                 join M in dbContext.datahierarchy on n.i_IdTipoMovimiento equals M.i_ItemId into M_join
                                 from M in M_join.DefaultIfEmpty()
                                 join H in dbContext.documento on B.i_IdTipoDocumento equals H.i_CodigoDocumento into H_join
                                 from H in H_join.DefaultIfEmpty()
                                 join P in dbContext.pedido on B.v_NroPedido equals P.v_IdPedido into P_join
                                 from P in P_join.DefaultIfEmpty()
                                 where (G.i_GroupId == 17 && G.i_IsDeleted == 1) && (M.i_GroupId == _b && M.i_IsDeleted == 1)
                                 where
                                 n.i_Eliminado == 0 && A.i_Eliminado == 0 && B.i_Eliminado == 0 && C.i_Eliminado == 0 &&
                                 D.i_Eliminado == 0 &&


                                 (n.t_Fecha >= pstrt_FechaRegistroIni && n.t_Fecha <= pstrt_FechaRegistroFin) &&
                                 (n.i_IdAlmacenOrigen == pstrIdAlmacen || pstrIdAlmacen == -1) &&
                                 (D.v_CodInterno == pstr_CodInterno || pstr_CodInterno == "") &&
                                 (L.v_CodLinea == pstr_CodLinea || pstr_CodLinea == "") &&
                                 (n.i_IdTipoMotivo == pintIdTipoMotivo || pintIdTipoMotivo == -1) &&
                                 (n.v_IdCliente == pstrIdCliente || pstrIdCliente == "") &&
                                 (n.v_Correlativo == pstrNumPedido || pstrNumPedido == "")

                                 select new ReporteListadoIngresoAlmacenAnalitico
                                 {
                                     NombreEmpresaPropietaria = "",
                                     RucEmpresaPropietaria = "",
                                     IdAlmacen = 0,
                                     Almacen = "ALMACEN PRINCIPAL",
                                     IdMovimiento = n.v_IdMovimiento,
                                     Fecha = n.t_Fecha.Value,
                                     Movimiento = M.v_Value2,
                                     Periodo = n.v_Periodo,
                                     Mes = n.v_Mes,
                                     Correlativo = n.v_Mes + "-" + n.v_Correlativo,
                                     IdProducto = D.v_CodInterno,
                                     NombreProducto = D.v_Descripcion,
                                     UnidadMedida = G.v_Value1,
                                     CantidadDetalle = n.i_EsDevolucion == 1 ? B.d_Cantidad.Value * -1 : B.d_Cantidad.Value,
                                     PrecioDetalle =
                                         n.i_EsDevolucion == 1
                                             ? n.i_IdMoneda.Value == (int)Currency.Soles
                                                 ? B.d_Precio.Value * -1
                                                 : (B.d_Precio.Value * n.d_TipoCambio.Value) * -1
                                             : n.i_IdMoneda.Value == (int)Currency.Soles
                                                 ? B.d_Precio.Value
                                                 : (B.d_Precio.Value * n.d_TipoCambio.Value),
                                     TotalDetalle =
                                         n.i_EsDevolucion == 1
                                             ? n.i_IdMoneda.Value == (int)Currency.Soles
                                                 ? B.d_Total.Value * -1
                                                 : (B.d_Total.Value * n.d_TipoCambio.Value) * -1
                                             : n.i_IdMoneda.Value == (int)Currency.Soles
                                                 ? B.d_Total.Value
                                                 : (B.d_Total.Value * n.d_TipoCambio.Value),
                                     TipoCambio = n.d_TipoCambio.Value,
                                     NombreMoneda =
                                         pstrIdMovimiento == (int)Currency.Soles
                                             ? pstrIdMovimiento == (int)Currency.Soles ? "S/." : "$"
                                             : pstrIdMovimiento == (int)Currency.Soles ? "S/." : "$",
                                     IdMoneda = n.i_IdMoneda.Value,
                                     PrecioDetalleD =
                                         n.i_EsDevolucion == 1
                                             ? n.i_IdMoneda.Value == (int)Currency.Dolares
                                                 ? B.d_Precio.Value * -1
                                                 : (B.d_Precio.Value / n.d_TipoCambio.Value) * -1
                                             : n.i_IdMoneda.Value == (int)Currency.Dolares
                                                 ? B.d_Precio.Value
                                                 : (B.d_Precio.Value / n.d_TipoCambio.Value),
                                     TotalDetalleD =
                                         n.i_EsDevolucion == 1
                                             ? n.i_IdMoneda.Value == (int)Currency.Dolares
                                                 ? B.d_Total.Value * -1
                                                 : (B.d_Total.Value / n.d_TipoCambio.Value) * -1
                                             : n.i_IdMoneda.Value == (int)Currency.Dolares
                                                 ? B.d_Total.Value
                                                 : (B.d_Total.Value / n.d_TipoCambio.Value),
                                     NumeroGuiaRemision = B.v_NroGuiaRemision,
                                     Numeropedido = B.v_NroPedido,

                                 }
                    );
                    query = query.OrderBy("" + pstrt_Orden + "ASC");

                    var query1 = (from A in query.ToList()
                                  select new ReporteListadoIngresoAlmacenAnalitico
                                  {
                                      NombreEmpresaPropietaria = DevolverNombreEmpresaPropietaria()[0],
                                      RucEmpresaPropietaria = DevolverNombreEmpresaPropietaria()[1],
                                      IdAlmacen = A.IdAlmacen,
                                      Almacen = A.Almacen,
                                      IdMovimiento = A.IdMovimiento,
                                      Fecha = A.Fecha,
                                      Movimiento = A.Movimiento,
                                      Periodo = A.Periodo,
                                      Mes = A.Mes,
                                      Correlativo = A.Correlativo,
                                      IdProducto = A.IdProducto,
                                      NombreProducto = A.NombreProducto,
                                      UnidadMedida = A.UnidadMedida,
                                      CantidadDetalle = A.CantidadDetalle,
                                      PrecioDetalle = A.PrecioDetalle,
                                      TotalDetalle = A.TotalDetalle,
                                      TipoCambio = A.TipoCambio,
                                      NombreMoneda = A.NombreMoneda,
                                      IdMoneda = A.IdMoneda,
                                      PrecioDetalleD = A.PrecioDetalleD,
                                      TotalDetalleD = A.TotalDetalleD,
                                      NumeroGuiaRemision = A.NumeroGuiaRemision,
                                      Numeropedido = A.Numeropedido,
                                  });


                    return query1.ToList();
                }
            }
            catch (Exception ex)
            {
                //pobjOperationResult.Success = 0;
                //pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }
        public List<ReporteListadoIngresoAlmacenAnalitico> ReporteListadoIngresoAlmacenAnalitico(ref OperationResult objOperationResult, int pintTipoMovimiento, DateTime? pstrt_FechaRegistroIni, DateTime? pstrt_FechaRegistroFin, string pstrFilterExpression, string pstr_CodInterno, string pstr_CodLinea, int pintIdTipoMotivo, string pstrIdCliente, string pstrNumPedido, string pstrt_Orden, string pstr_grupollave, string pstr_Nombregrupollave, string pstr_grupollave2, string pstr_Nombregrupollave2, int pstrMoneda, string Empresa, string pstrRuc, bool TomarCuentaNroPedido)
        {
            try
            {

                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from A in dbContext.movimiento

                                 join E in dbContext.movimientodetalle on new { v_IdMovimiento = A.v_IdMovimiento, eliminado = 0 }
                                                                      equals new { v_IdMovimiento = E.v_IdMovimiento, eliminado = E.i_Eliminado.Value } into E_join
                                 from E in E_join.DefaultIfEmpty()

                                 join F in dbContext.productodetalle on new { IdProductoDetalle = E.v_IdProductoDetalle, eliminado = 0 } equals new { IdProductoDetalle = F.v_IdProductoDetalle, eliminado = F.i_Eliminado.Value } into F_join
                                 from F in F_join.DefaultIfEmpty()

                                 join G in dbContext.producto on new { v_IdProducto = F.v_IdProducto, eliminado = 0 }
                                                                equals new { v_IdProducto = G.v_IdProducto, eliminado = G.i_Eliminado.Value } into G_join
                                 from G in G_join.DefaultIfEmpty()

                                 join H in dbContext.linea on new { IdLinea = G.v_IdLinea, eliminado = 0 } equals new { IdLinea = H.v_IdLinea, eliminado = H.i_Eliminado.Value } into H_join

                                 from H in H_join.DefaultIfEmpty()

                                 join I in dbContext.cliente on new { idCliente = A.v_IdCliente, Flag = "V", eliminado = 0 }
                                                            equals new { idCliente = I.v_IdCliente, Flag = I.v_FlagPantalla, eliminado = I.i_Eliminado.Value } into I_join
                                 from I in I_join.DefaultIfEmpty()

                                 join J in dbContext.datahierarchy on new { idUnidad = E.i_IdUnidad.Value, Grupo = 17, eliminado = 0 }
                                                                  equals new { idUnidad = J.i_ItemId, Grupo = J.i_GroupId, eliminado = J.i_IsDeleted.Value } into J_join
                                 from J in J_join.DefaultIfEmpty()


                                 join M in dbContext.datahierarchy on new { Motivo = A.i_IdTipoMotivo.Value, Grupo = 19, eliminado = 0 }
                                                            equals new { Motivo = M.i_ItemId, Grupo = M.i_GroupId, eliminado = M.i_IsDeleted.Value } into M_join
                                 from M in M_join.DefaultIfEmpty()


                                 join C in dbContext.establecimientoalmacen on new { IdAlmacen = A.i_IdAlmacenOrigen.Value, eliminado = 0 } equals new { IdAlmacen = C.i_IdAlmacen.Value, eliminado = C.i_Eliminado.Value } into C_join
                                 from C in C_join.DefaultIfEmpty()


                                 join B in dbContext.almacen on new { IdAlmacen = C.i_IdAlmacen.Value, eliminado = 0 } equals new { IdAlmacen = B.i_IdAlmacen, eliminado = B.i_Eliminado.Value } into B_join
                                 from B in B_join.DefaultIfEmpty()


                                 join O in dbContext.establecimiento on new { IdEstablecimiento = C.i_IdEstablecimiento.Value, eliminado = 0 } equals new { IdEstablecimiento = O.i_IdEstablecimiento, eliminado = O.i_Eliminado.Value } into O_join

                                 from O in O_join.DefaultIfEmpty()
                                 where A.i_Eliminado == 0 &&

                                 (A.t_Fecha >= pstrt_FechaRegistroIni && A.t_Fecha <= pstrt_FechaRegistroFin) &&
                                 (G.v_CodInterno.Trim() == pstr_CodInterno.Trim() || pstr_CodInterno == "") &&
                                 (H.v_IdLinea == pstr_CodLinea || pstr_CodLinea == "-1") &&
                                 (A.i_IdTipoMotivo == pintIdTipoMotivo || pintIdTipoMotivo == -1) &&
                                 (I.v_CodCliente == pstrIdCliente || pstrIdCliente == "") &&
                                 (E.v_NroPedido == pstrNumPedido || pstrNumPedido == "") && A.i_IdTipoMovimiento == pintTipoMovimiento
                                 && G.i_EsActivoFijo == 0

                                 select new ReporteListadoIngresoAlmacenAnalitico
                                 {
                                     NombreEmpresaPropietaria = "",
                                     RucEmpresaPropietaria = "",
                                     IdAlmacen = A.i_IdAlmacenOrigen.Value,
                                     Almacen = B == null ? "ALMACÉN NO EXISTE" : B.v_Nombre,
                                     IdMovimiento = A.v_IdMovimiento,
                                     Fecha = A.t_Fecha.Value,
                                     Movimiento = M == null ? "" : M.v_Value2,
                                     Periodo = A.v_Periodo,
                                     Mes = A.v_Mes,
                                     Correlativo = A.v_Mes + "-" + A.v_Correlativo,
                                     IdProducto = G == null ? "" : G.v_CodInterno,
                                     NombreProducto = G == null ? "** PRODUCTO NO EXISTE **" : G.v_Descripcion,
                                     UnidadMedida = J == null ? "**U.M. NO EXISTE**" : J.v_Value1,
                                     CantidadDetalle = A == null ? 0 : A.i_EsDevolucion == 1 ? E.d_CantidadEmpaque == null ? 0 : E.d_CantidadEmpaque.Value * -1 : E.d_CantidadEmpaque.Value,
                                     PrecioDetalle = A == null ? 0 : A.v_OrigenTipo == "I" ? pstrMoneda == A.i_IdMoneda.Value ? E.d_Precio : E.d_PrecioCambio : A.i_IdMoneda.Value == pstrMoneda ? E.d_Precio.Value : pstrMoneda == (int)Currency.Soles ? (E.d_Precio.Value * A.d_TipoCambio.Value) : E.d_Precio / A.d_TipoCambio,
                                     TotalDetalle = A == null ? 0 : A.v_OrigenTipo == "I" ? pstrMoneda == A.i_IdMoneda ? A.i_EsDevolucion == 1 ? E.d_Total * -1 : E.d_Total : A.i_EsDevolucion == 1 ? E.d_TotalCambio * -1 : E.d_TotalCambio : A.i_EsDevolucion == 1 ? A.i_IdMoneda.Value == pstrMoneda ? E.d_Total.Value * -1 : pstrMoneda == (int)Currency.Soles ? (E.d_Total.Value * A.d_TipoCambio.Value) * -1 : (E.d_Total / A.d_TipoCambio.Value) * -1 :
                                     A.i_IdMoneda.Value == pstrMoneda ? E.d_Total.Value : pstrMoneda == (int)Currency.Soles ? (E.d_Total.Value * A.d_TipoCambio.Value) : (E.d_Total / A.d_TipoCambio.Value),
                                     TipoCambio = A.d_TipoCambio.Value,
                                     NombreMoneda = "",
                                     IdMoneda = A.i_IdMoneda.Value,
                                     PrecioDetalleD = A == null ? 0 : A.i_EsDevolucion == 1 ? A.i_IdMoneda.Value == (int)Currency.Dolares ? E.d_Precio.Value * -1 : (E.d_Precio.Value / A.d_TipoCambio.Value) * -1 : A.i_IdMoneda.Value == (int)Currency.Dolares ? E.d_Precio.Value : (E.d_Precio.Value / A.d_TipoCambio.Value),
                                     TotalDetalleD = A == null ? 0 : A.i_EsDevolucion == 1 ? A.i_IdMoneda.Value == (int)Currency.Dolares ? E.d_Total.Value * -1 : (E.d_Total.Value / A.d_TipoCambio.Value) * -1 : A.i_IdMoneda.Value == (int)Currency.Dolares ? E.d_Total.Value : (E.d_Total.Value / A.d_TipoCambio.Value),
                                     Establecimiento = O == null ? "** NO EXISTE ESTAB.**" : O.v_Nombre,
                                     NombreLinea = H == null ? "** NO EXISTE LINEA ** " : H.v_Nombre == null ? "" : H.v_Nombre,
                                     MotivoIngreso = M == null ? "" : M.v_Value1 == null ? "" : M.v_Value1,
                                     NombreProveedor = I == null ? "** NO EXISTE PROVEEDOR **" : I.v_IdCliente == null ? "" : (I.v_CodCliente.Trim() + " " + I.v_PrimerNombre + " " + I.v_ApePaterno + " " + I.v_ApeMaterno + " " + I.v_RazonSocial).Trim(),
                                     NumeroGuiaRemision = E == null ? "" : E.v_NroGuiaRemision,
                                     Numeropedido = TomarCuentaNroPedido == true ? E == null ? "" : E.v_NroPedido : "",
                                     Grupollave = pstr_grupollave == "NOMBREPRODUCTO" ? G == null ? "** PRODUCTO NO EXISTE **" : " PRODUCTO : " + G.v_Descripcion : "" + pstr_grupollave == "NOMBREPROVEEDOR" ? I == null ? "** PROVEEDOR NO EXISTE **" : " PROVEEDOR : " + (I.v_CodCliente.Trim() + " " + I.v_PrimerNombre + " " + I.v_ApePaterno + " " + I.v_ApeMaterno + " " + I.v_RazonSocial).Trim() : "" + pstr_grupollave == "NOMBRELINEA" ? H == null ? "** LINEA NO EXISTE **" : "LINEA : " + H.v_Nombre : "" + pstr_grupollave == "MOVIMIENTO" ? M == null ? "** MOVIMIENTO NO EXISTE **" : " MOVIMIENTO :" + M.v_Value2 : "" + pstr_grupollave == "NUMEROPEDIDO" ? E == null ? "** PEDIDO NO EXISTE **" : " PEDIDO : " + E.v_NroPedido : "" + pstr_grupollave == "MOTIVOINGRESO" ? A == null ? "** MOTIVO INGRESO NO EXISTE **" : " MOTIVO INGRESO : " + M.v_Value1 : "" + pstr_grupollave == "NOMBRELINEA/NOMBREPRODUCTO" ? H == null ? "** LÍNEA NO EXISTE **" : "LINEA" + " : " + H.v_Nombre.Trim() : "",
                                     Grupollave2 = pstr_grupollave2 == "NOMBREPRODUCTO" ? G == null ? "** PRODUCTO NO EXISTE **" : " PRODUCTO : " + G.v_Descripcion : "" + pstr_grupollave2 == "NOMBREPROVEEDOR" ? I == null ? "** PROVEEDOR NO EXISTE **" : " PROVEEDOR : " + (I.v_CodCliente.Trim() + " " + I.v_PrimerNombre + " " + I.v_ApePaterno + " " + I.v_ApeMaterno + " " + I.v_RazonSocial).Trim() : "" + pstr_grupollave2 == "NOMBRELINEA" ? H == null ? "** LINEA NO EXISTE **" : " LINEA : " + H.v_Nombre : "" + pstr_grupollave == "MOVIMIENTO" ? M == null ? "** MOVIMIENTO NO EXISTE **" : " MOVIMIENTO :" + M.v_Value2 : "" + pstr_grupollave2 == "NUMEROPEDIDO" ? E == null ? "** PEDIDO NO EXISTE **" : " PEDIDO : " + E.v_NroPedido : "" + pstr_grupollave2 == "MOTIVOINGRESO" ? A == null ? "** MOTIVO INGRESO  NO EXISTE **" : " MOTIVO INGRESO : " + M.v_Value1 : "" + pstr_grupollave2 == "NOMBRELINEA/NOMBREPRODUCTO" ? G == null ? "** DESCRIPCIÓN NO EXISTE **" : "DESCRIPCICIÓN" + " : " + G.v_Descripcion.Trim() : "",

                                 }
                                        );

                    if (!string.IsNullOrEmpty(pstrFilterExpression))
                    {
                        query = query.Where(pstrFilterExpression);
                    }

                    query = query.OrderBy(pstrt_Orden);

                    var query1 = (from A in query.ToList()

                                  select new ReporteListadoIngresoAlmacenAnalitico
                                  {
                                      NombreEmpresaPropietaria = A.NombreEmpresaPropietaria,
                                      RucEmpresaPropietaria = A.RucEmpresaPropietaria,
                                      IdAlmacen = A.IdAlmacen,
                                      Almacen = "ALMACÉN : " + A.Almacen.ToUpper(),
                                      IdMovimiento = A.IdMovimiento,
                                      Fecha = A.Fecha,
                                      Movimiento = A.Movimiento,
                                      Periodo = A.Periodo,
                                      Mes = A.Mes,
                                      Correlativo = A.Correlativo,
                                      IdProducto = A.IdProducto,
                                      NombreProducto = A.NombreProducto,
                                      UnidadMedida = A.UnidadMedida,
                                      CantidadDetalle = A.CantidadDetalle,
                                      PrecioDetalle = A.PrecioDetalle,
                                      TotalDetalle = Globals.ClientSession.i_Periodo == 2016 ? A.TotalDetalle.Value : Utils.Windows.DevuelveValorRedondeado(A.TotalDetalle.Value, 2),
                                      //TotalDetalle = A.TotalDetalle.Value,
                                      TipoCambio = A.TipoCambio,
                                      NombreMoneda = A.NombreMoneda,
                                      IdMoneda = A.IdMoneda,
                                      PrecioDetalleD = A.PrecioDetalleD,
                                      TotalDetalleD = A.TotalDetalleD,
                                      Establecimiento = "ESTABLECIMIENTO : " + A.Establecimiento.ToUpper(),
                                      NombreLinea = A.NombreLinea,
                                      MotivoIngreso = A.MotivoIngreso,
                                      NombreProveedor = A.NombreProveedor,
                                      NumeroGuiaRemision = A.NumeroGuiaRemision,
                                      Numeropedido = A.Numeropedido,
                                      Grupollave = A.Grupollave,
                                      Grupollave2 = A.Grupollave2,
                                  });

                    objOperationResult.Success = 1;

                    return query1.ToList();
                }

            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;

            }
        }
        public List<string> DevolverNombreEmpresaPropietaria()
        {
            OperationResult objOperationResult = new OperationResult();
            List<string> Retonar = new List<string>();
            NodeBL objNodeBL = new NodeBL();
            int _intNodeId = int.Parse(Utils.Windows.GetApplicationConfigValue("NodeId"));

            var x = objNodeBL.GetNodeByNodeId(ref objOperationResult, _intNodeId);
            Retonar.Add(x.v_RazonSocial);
            Retonar.Add(x.v_RUC);
            return Retonar;

        }
        public List<ReporteTransferenciaAlmacen> ReporteTransferenciaAlmacen(string pstrIdMovimiento)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var Reporte = (from a in dbContext.movimiento

                                   join b in dbContext.movimientodetalle on a.v_IdMovimiento equals b.v_IdMovimiento into b_join

                                   from b in b_join.DefaultIfEmpty()

                                   join c in dbContext.productodetalle on b.v_IdProductoDetalle equals c.v_IdProductoDetalle into c_join

                                   from c in c_join.DefaultIfEmpty()

                                   join d in dbContext.producto on c.v_IdProducto equals d.v_IdProducto into d_join

                                   from d in d_join.DefaultIfEmpty()

                                   join e in dbContext.almacen on a.i_IdAlmacenOrigen equals e.i_IdAlmacen into e_join

                                   from e in e_join.DefaultIfEmpty()

                                   join f in dbContext.almacen on a.i_IdAlmacenDestino equals f.i_IdAlmacen into f_join

                                   from f in f_join.DefaultIfEmpty()


                                   join h in dbContext.datahierarchy on new { i_idUnidaMedidad = b.i_IdUnidad.Value, grupoMedida = 17 }
                                                                    equals new { i_idUnidaMedidad = h.i_ItemId, grupoMedida = h.i_GroupId } into h_join

                                   from h in h_join.DefaultIfEmpty()


                                   join i in dbContext.datahierarchy on new { i_idMotivo = a.i_IdTipoMotivo.Value, grupo = 22 }
                                                                    equals new { i_idMotivo = i.i_ItemId, grupo = i.i_GroupId } into i_join

                                   from i in i_join.DefaultIfEmpty()


                                   where a.v_IdMovimiento == pstrIdMovimiento && a.i_Eliminado == 0 && b.i_Eliminado == 0 && c.i_Eliminado == 0 && d.i_Eliminado == 0 && e.i_Eliminado == 0 && f.i_Eliminado == 0 && h.i_IsDeleted == 0

                                   select new ReporteTransferenciaAlmacen
                                   {

                                       AlmacenOrigen = e.v_Nombre,
                                       AlmacenDestino = f.v_Nombre,
                                       Fecha = a.t_Fecha.Value,
                                       TipoCambio = a.d_TipoCambio.Value,
                                       DescripcionMotivo = i.v_Value1,
                                       NombreMoneda = a.i_IdMoneda.Value == 1 ? "SOLES" : "DOLARES",
                                       Glosa = a.v_Glosa,
                                       CodProducto = d.v_CodInterno,
                                       DescripcionProducto = d.v_Descripcion,
                                       UnidadMedida = h.v_Value1,
                                       Guia = b.v_NroGuiaRemision,
                                       Documento = b.i_IdTipoDocumento.Value,
                                       CantidadDetalle = b.d_Cantidad.Value,
                                       PrecioDetalle = b.d_Precio.Value,
                                       TotalDetalle = b.d_Total.Value,
                                       Pedido = b.v_NroPedido,
                                       Correlativo = "N°" + " " + a.v_Mes + "-" + a.v_Correlativo,
                                       NumeroDocumento = b.v_NumeroDocumento,
                                   }).ToList();



                    var ReporteFinal = (from a in Reporte
                                        let DocumentoSiglas = ObtenerSiglasDocumento(a.Documento)
                                        select new ReporteTransferenciaAlmacen
                                        {

                                            AlmacenOrigen = a.AlmacenOrigen,
                                            AlmacenDestino = a.AlmacenDestino,
                                            Fecha = a.Fecha,
                                            TipoCambio = a.TipoCambio,
                                            DescripcionMotivo = a.DescripcionMotivo,
                                            NombreMoneda = a.NombreMoneda,
                                            Glosa = a.Glosa,
                                            CodProducto = a.CodProducto,
                                            DescripcionProducto = a.DescripcionProducto,
                                            UnidadMedida = a.UnidadMedida,
                                            Guia = a.Guia,
                                            Documento = a.Documento,
                                            CantidadDetalle = a.CantidadDetalle,
                                            PrecioDetalle = a.PrecioDetalle,
                                            TotalDetalle = a.TotalDetalle,
                                            Pedido = a.Pedido,
                                            Correlativo = a.Correlativo,
                                            NumeroDocumento = DocumentoSiglas + " " + a.NumeroDocumento,
                                        }
                                );

                    return ReporteFinal.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        public string ObtenerSiglasDocumento(int pstrCodigo)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var Codigo = (from n in dbContext.documento

                              where n.i_Eliminado == 0 && n.i_CodigoDocumento == pstrCodigo

                              select n).FirstOrDefault();

                if (Codigo != null)
                    return Codigo.v_Siglas;
                return string.Empty;
            }
        }

        public List<ReporteStockConsolidado> ReporteStockConsolidado(ref OperationResult objOperationResult, DateTime FechaInicial, DateTime FechaFinal, string Orden, string pstrLinea, int idEstablecimiento, string CodInternoProducto, string IdMarca, bool StockNegativo, bool StockPositivo, bool StockCero, bool CeroEstablecimiento, int EstadoProducto, int IdAlmacen = -1, int TipoUnidades = (int)FormatoCantidad.UnidadMedidaProducto)
        {
            try
            {
                IQueryable<ReporteStockConsolidado> query;
                objOperationResult.Success = 1;
                ReporteStockConsolidado objError = new ReporteStockConsolidado();
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    query = (from b in dbContext.movimientodetalle

                             join c in dbContext.productodetalle on new { IdProductoDetalle = b.v_IdProductoDetalle, eliminado = 0 } equals new { IdProductoDetalle = c.v_IdProductoDetalle, eliminado = c.i_Eliminado.Value } into c_join

                             from c in c_join.DefaultIfEmpty()

                             join d in dbContext.producto on new { IdProducto = c.v_IdProducto, eliminado = 0 } equals new { IdProducto = d.v_IdProducto, eliminado = d.i_Eliminado.Value } into d_join

                             from d in d_join.DefaultIfEmpty()

                             join g in dbContext.datahierarchy on new { g = 17, i = b.i_IdUnidad.Value, eliminado = 0 }
                                                             equals new { g = g.i_GroupId, i = g.i_ItemId, eliminado = g.i_IsDeleted.Value } into g_join
                             from g in g_join.DefaultIfEmpty()

                             join h in dbContext.datahierarchy on new { g = 17, i = d.i_IdUnidadMedida.Value, eliminado = 0 }
                                                             equals new { g = h.i_GroupId, i = h.i_ItemId, eliminado = h.i_IsDeleted.Value } into h_join
                             from h in h_join.DefaultIfEmpty()


                             join a in dbContext.movimiento on new { m = b.v_IdMovimiento, eliminado = 0 } equals new { m = a.v_IdMovimiento, eliminado = a.i_Eliminado.Value } into a_join
                             from a in a_join.DefaultIfEmpty()

                             where a.t_Fecha >= FechaInicial
                             && a.t_Fecha <= FechaFinal && a.i_Eliminado == 0 && (d.v_IdLinea == pstrLinea || pstrLinea == "-1")
                             && (a.i_IdEstablecimiento == idEstablecimiento || idEstablecimiento == -1)
                             && (d.v_CodInterno == CodInternoProducto || CodInternoProducto == "") && a.i_IdTipoMovimiento != (int)TipoDeMovimiento.Transferencia
                             && d.i_EsServicio == 0 && d.i_EsActivoFijo == 0 && (d.v_IdMarca == IdMarca || IdMarca == "-1")
                             && (d.i_EsActivo == EstadoProducto || EstadoProducto == -1)
                             && c_join.Any(o => o.v_IdProducto == d.v_IdProducto)
                             && c_join.Any(o => o.v_IdProductoDetalle == b.v_IdProductoDetalle)
                             && a_join.Any(o => o.v_IdMovimiento == b.v_IdMovimiento)
                             && b.i_Eliminado == 0
                             select new
                             {
                                 codigoProducto = d.v_CodInterno,
                                 IdProducto = d.v_IdProducto,
                                 descripcionProducto = d.v_Descripcion.Trim(),
                                 tipoMovimiento = a.i_IdTipoMovimiento.Value,
                                 devolucion = a.i_EsDevolucion ?? 0,
                                 idAlmacen = a.i_IdAlmacenOrigen.Value,
                                 cantidad = b == null ? 0 : b.d_CantidadEmpaque == null ? b.d_Cantidad == null ? 0 : b.d_Cantidad : b.d_CantidadEmpaque,
                                 unidad = d.d_Empaque.Value == 1 && g.v_Value2 == "1" ? g.v_Value1 : "UNIDAD",
                                 v_IdProductoDetalle = b.v_IdProductoDetalle,
                                 Fecha = a.t_Fecha.Value,
                                 IdMovimiento = a.v_IdMovimiento,
                                 ValorUM = string.IsNullOrEmpty(h.v_Value2) ? "0" : h.v_Value2,

                             }).ToList().AsQueryable().Select(o => new ReporteStockConsolidado
                             {
                                 codigoProducto = o.codigoProducto,
                                 IdProducto = o.IdProducto,
                                 descripcionProducto = o.descripcionProducto,
                                 tipoMovimiento = o.tipoMovimiento,
                                 devolucion = o.devolucion,
                                 idAlmacen = o.idAlmacen,
                                 // cantidad =o.cantidad ,
                                 cantidad = o.cantidad == 0 || decimal.Parse(o.ValorUM) == 0 ? 0 : TipoUnidades == (int)FormatoCantidad.UnidadMedidaProducto ? o.cantidad / decimal.Parse(o.ValorUM) : o.cantidad,
                                 unidad = o.unidad,
                                 v_IdProductoDetalle = o.v_IdProductoDetalle,
                                 Fecha = o.Fecha,
                                 IdMovimiento = o.IdMovimiento,
                                 ValorUM = o.ValorUM,
                             }).ToList().AsQueryable();
                    if (!string.IsNullOrEmpty(Orden))
                    {
                        query = query.OrderBy(Orden);
                    }
                    query = query.AsQueryable().OrderBy(o => o.Fecha);

                }
                var listReport = GetConsolidado(ref objOperationResult, query, idEstablecimiento, IdAlmacen);
                if (objOperationResult.Success == 1)
                {
                    if (StockNegativo)
                    {
                        listReport = listReport.Where(o => o.cantidad <= 0).ToList();
                    }
                    else if (StockPositivo)
                    {
                        listReport = listReport.Where(o => o.cantidad > 0).ToList();
                    }

                    else if (StockCero)
                    {
                        listReport = listReport.Where(o => o.cantidad == 0).ToList();
                    }

                    else if (CeroEstablecimiento)
                    {

                        listReport = listReport.Where(o => o.EsCantidadCeroAlmacen).ToList();


                    }

                    objOperationResult.Success = 1;
                    return listReport;
                }
                else
                {
                    objOperationResult.Success = 0;
                    return null;

                }
            }
            catch (Exception ex)
            {


                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "AlmacenBL.ReporteStockConsolidado()";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }

        }

        public List<ReporteStockConsolidado> GetReporteReposicion(ref OperationResult pobjOperationResult,
        DateTime fechaInicial, DateTime fechaFinal, string pstrLinea, int idEstablecimiento, string codigoProducto)
        {
            try
            {
                ReporteStockConsolidado[] query;
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    query = (from a in dbContext.movimiento

                             join b in dbContext.movimientodetalle on new { IdMovimiento = a.v_IdMovimiento, eliminado = 0 } equals new { IdMovimiento = b.v_IdMovimiento, eliminado = b.i_Eliminado.Value } into b_join

                             from b in b_join.DefaultIfEmpty()

                             join c in dbContext.productodetalle on new { IdProductoDetalle = b.v_IdProductoDetalle, eliminado = 0 } equals new { IdProductoDetalle = c.v_IdProductoDetalle, eliminado = c.i_Eliminado.Value } into c_join

                             from c in c_join.DefaultIfEmpty()

                             join d in dbContext.producto on new { IdProducto = c.v_IdProducto, eliminado = 0 } equals new { IdProducto = d.v_IdProducto, eliminado = d.i_Eliminado.Value } into d_join

                             from d in d_join.DefaultIfEmpty()

                             join g in dbContext.datahierarchy on new { g = 17, i = b.i_IdUnidad.Value, eliminado = 0 }
                                                             equals new { g = g.i_GroupId, i = g.i_ItemId, eliminado = g.i_IsDeleted.Value } into g_join
                             from g in g_join.DefaultIfEmpty()

                             where a.t_Fecha >= fechaInicial
                             && a.t_Fecha <= fechaFinal && a.i_Eliminado == 0 && (d.v_IdLinea == pstrLinea || pstrLinea == "-1")
                             && (a.i_IdEstablecimiento == idEstablecimiento || idEstablecimiento == -1)
                             && b.v_NumeroDocumento != null && b.v_NumeroDocumento != ""
                             && (d.v_CodInterno == codigoProducto || codigoProducto == "") && a.i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeSalida
                             && a.i_EsDevolucion.Value == 0
                             && d.i_EsServicio == 0 && d.i_EsActivoFijo == 0
                             select new ReporteStockConsolidado
                             {
                                 codigoProducto = d.v_CodInterno,
                                 descripcionProducto = d.v_Descripcion.Trim(),
                                 cantidad = d.d_Empaque.Value == 1 && g.v_Value2 == "1" ? b.d_Cantidad : b.d_CantidadEmpaque ?? 0,
                                 unidad = d.d_Empaque.Value == 1 && g.v_Value2 == "1" ? g.v_Value1 : "UNIDAD",
                             }).ToArray();
                }

                var rep2 = new AlmacenBL().ReporteStockConsolidado(ref pobjOperationResult, new DateTime(Globals.ClientSession.i_Periodo ?? DateTime.Now.Year, 1, 1),
                                            fechaFinal, null, pstrLinea, idEstablecimiento, codigoProducto, "-1", false, false, false, false, -1).ToDictionary(r => r.codigoProducto, r => r.cantidad);
                var repItalia = new AlmacenBL().ReporteStockConsolidado(ref pobjOperationResult, new DateTime(Globals.ClientSession.i_Periodo ?? DateTime.Now.Year, 1, 1),
                            fechaFinal, null, pstrLinea, Globals.ClientSession.i_IdEstablecimiento ?? 1, codigoProducto, "-1", false, false, false, false, -1).ToDictionary(r => r.codigoProducto, r => r.cantidad);
                var queryList = query.GroupBy(x => x.codigoProducto);
                var listReport = new List<ReporteStockConsolidado>();

                foreach (var prod in queryList)
                {
                    var first = prod.FirstOrDefault();
                    if (first == null) continue;
                    first.cantidad = prod.Sum(r => r.cantidad);
                    if (!rep2.ContainsKey(first.codigoProducto)) continue;
                    decimal? val;
                    rep2.TryGetValue(first.codigoProducto, out val);
                    first.cantidadAlmacen1 = val;
                    if (repItalia.TryGetValue(first.codigoProducto, out val))
                        first.cantidadAlmacen2 = val;
                    else first.cantidadAlmacen2 = 0;
                    listReport.Add(first);
                }
                pobjOperationResult.Success = 1;
                return listReport;
            }
            catch (Exception ex)
            {
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public List<ReporteStockConsolidado> ReporteStockConsolidado2(ref OperationResult objOperationResult, DateTime FechaInicial, DateTime FechaFinal, string pstrLinea, int idEstablecimiento, string CodInternoProducto, string IdMarca, bool StockNegativo, bool StockPositivo, bool StockCero, bool StockCeroEsstablecimiento, int EstadoProducto = 1)
        {
            try
            {
                IQueryable<ReporteStockConsolidado> query;
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    query = (from a in dbContext.movimiento

                             join b in dbContext.movimientodetalle on new { IdMovimiento = a.v_IdMovimiento, eliminado = 0 } equals new { IdMovimiento = b.v_IdMovimiento, eliminado = b.i_Eliminado.Value } into b_join

                             from b in b_join.DefaultIfEmpty()

                             join c in dbContext.productodetalle on new { IdProductoDetalle = b.v_IdProductoDetalle, eliminado = 0 } equals new { IdProductoDetalle = c.v_IdProductoDetalle, eliminado = c.i_Eliminado.Value } into c_join

                             from c in c_join.DefaultIfEmpty()

                             join d in dbContext.producto on new { IdProducto = c.v_IdProducto, eliminado = 0 } equals new { IdProducto = d.v_IdProducto, eliminado = d.i_Eliminado.Value } into d_join

                             from d in d_join.DefaultIfEmpty()

                             join g in dbContext.datahierarchy on new { g = 17, i = b.i_IdUnidad.Value, eliminado = 0 }
                                                             equals new { g = g.i_GroupId, i = g.i_ItemId, eliminado = g.i_IsDeleted.Value } into g_join
                             from g in g_join.DefaultIfEmpty()

                             join h in dbContext.color on d.v_IdColor equals h.v_IdColor into h_join
                             from h in h_join.DefaultIfEmpty()

                             join i in dbContext.talla on d.v_IdTalla equals i.v_IdTalla into i_join
                             from i in i_join.DefaultIfEmpty()

                             join j in dbContext.marca on d.v_IdMarca equals j.v_IdMarca into j_join
                             from j in j_join.DefaultIfEmpty()

                             join k in dbContext.datahierarchy on new { Id = d.i_IdTemporada.Value, gr = 16 } equals new { Id = k.i_ItemId, gr = k.i_GroupId } into k_join
                             from k in k_join.DefaultIfEmpty()

                             join l in dbContext.datahierarchy on new { Id = d.i_IdUsuario.Value, gr = 9 } equals new { Id = l.i_ItemId, gr = l.i_GroupId } into l_join
                             from l in l_join.DefaultIfEmpty()

                             join m in dbContext.datahierarchy on new { Id = d.i_IdColeccion.Value, gr = 15 } equals new { Id = m.i_ItemId, gr = m.i_GroupId } into m_join
                             from m in m_join.DefaultIfEmpty()

                             join n in dbContext.datahierarchy on new { Id = d.i_IdArte.Value, gr = 14 } equals new { Id = n.i_ItemId, gr = n.i_GroupId } into n_join
                             from n in n_join.DefaultIfEmpty()

                             where a.t_Fecha >= FechaInicial
                             && a.t_Fecha <= FechaFinal && a.i_Eliminado == 0 && (d.v_IdLinea == pstrLinea || pstrLinea == "-1")
                             && (a.i_IdEstablecimiento == idEstablecimiento || idEstablecimiento == -1)
                             && (d.v_CodInterno == CodInternoProducto || CodInternoProducto == "") && a.i_IdTipoMovimiento != (int)TipoDeMovimiento.Transferencia
                             && d.i_EsServicio == 0 && d.i_EsActivoFijo == 0 && (d.v_IdMarca == IdMarca || IdMarca == "-1")
                             && (d.i_EsActivo == EstadoProducto || EstadoProducto == -1)
                             && b_join.Any(p => p.v_IdProductoDetalle == c.v_IdProductoDetalle)
                             orderby d.v_CodInterno
                             select new ReporteStockConsolidado
                             {
                                 codigoProducto = d.v_CodInterno,
                                 descripcionProducto = d.v_Descripcion.Trim(),
                                 tipoMovimiento = a.i_IdTipoMovimiento.Value,
                                 devolucion = a.i_EsDevolucion.Value,
                                 idAlmacen = a.i_IdAlmacenOrigen.Value,
                                 cantidad = b == null ? 0 : b.d_CantidadEmpaque == null ? b.d_Cantidad == null ? 0 : b.d_Cantidad : b.d_CantidadEmpaque,
                                 unidad = d.d_Empaque.Value == 1 && g.v_Value2 == "1" ? g.v_Value1 : "UNIDAD",
                                 Color = h.v_Nombre,
                                 Talla = i.v_Nombre,
                                 Marca = j.v_Nombre,
                                 Temporada = k != null ? k.v_Value1 : "-",
                                 Genero = l != null ? l.v_Value1 : "-",
                                 Coleccion = m != null ? m.v_Value1 : "-",
                                 Material = n != null ? n.v_Value1 : "-",
                                 Op = d.v_NroOrdenProduccion,
                                 PVenta = d.d_PrecioVenta.Value,
                                 PCosto = d.d_PrecioCosto.Value,
                                 Anio = d.i_Anio ?? 0,
                                 PrecioMayorista = d.d_PrecioMayorista ?? 0
                             }).ToList().AsQueryable();

                }
                var listReport = GetConsolidado(ref objOperationResult, query, idEstablecimiento);
                if (StockNegativo)
                {
                    listReport = listReport.Where(o => o.cantidad <= 0).ToList();
                }
                else if (StockPositivo)
                {
                    listReport = listReport.Where(o => o.cantidad > 0).ToList();
                }
                else if (StockCero)
                {
                    listReport = listReport.Where(o => o.cantidad == 0).ToList();
                }
                else if (StockCeroEsstablecimiento)
                {
                    listReport = listReport.Where(o => o.EsCantidadCeroAlmacen).ToList();
                }
                objOperationResult.Success = 1;
                return listReport;
            }
            catch (Exception ex)
            {
                objOperationResult.ExceptionMessage = ex.Message;
                return null;
            }

        }

        private List<ReporteStockConsolidado> GetConsolidado(ref OperationResult objOperationResult, IQueryable<ReporteStockConsolidado> q, int idestabl, int IdAlmacen = -1)
        {
            try
            {
                objOperationResult.Success = 1;

                var queryList = q.GroupBy(x => x.IdProducto);
                var almacenes = ObtenerAlmacenesxEstablecimiento(idestabl, IdAlmacen);
                var listReport = new List<ReporteStockConsolidado>();
                var ty = typeof(ReporteStockConsolidado);
                foreach (var prod in queryList)
                {
                    Error = prod.FirstOrDefault().codigoProducto;
                    var first = prod.FirstOrDefault();
                    if (first == null) continue;
                    decimal? total = 0;
                    bool CantidadCeroAlmacen = true;
                    for (var i = 0; i < almacenes.Count; i++)
                    {
                        var al = almacenes[i];
                        var prop = ty.GetProperty("almacen" + (i + 1));

                        prop.SetValue(first, al.v_Nombre, null);
                        var sum = prod.Where(r => r.idAlmacen == al.i_IdAlmacen)
                            .Sum(r =>
                            {
                                var val = r.cantidad;
                                if ((r.tipoMovimiento == 1 && r.devolucion == 1) ||
                                    (r.tipoMovimiento == 2 && r.devolucion == 0))
                                    val *= -1;
                                return val;
                            });
                        total += sum;
                        if (sum == 0 && CantidadCeroAlmacen == true)
                        {
                            CantidadCeroAlmacen = true;
                        }
                        else
                        {
                            CantidadCeroAlmacen = false;
                        }
                        prop = ty.GetProperty("cantidadAlmacen" + (i + 1));
                        prop.SetValue(first, sum, null);

                    }
                    first.EsCantidadCeroAlmacen = CantidadCeroAlmacen;
                    first.cantidad = total;
                    listReport.Add(first);
                }
                return listReport;
            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "AlmacenBL.GetConsolidado()" + Error;
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }
        }

        public void DesactivarProductosAlmacen(ref OperationResult objOperationResult, DateTime fechaFinal, List<string> ClientSession)
        {
            producto objProductoDto = new producto();

            using (var transaction = TransactionUtils.CreateTransactionScope())
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    var ListaProductos = dbContext.producto.Where(o => o.i_Eliminado.Value == 0).ToList();
                    var ProductosSinStock = ReporteStockConsolidado(ref objOperationResult, DateTime.Parse("01/01/" + Globals.ClientSession.i_Periodo.ToString()), fechaFinal, "", "-1", -1, "", "-1", false, false, false, true, 1);
                    if (objOperationResult.Success == 0)
                    {
                        return;
                    }

                    Globals.ProgressbarStatus.i_TotalProgress = ProductosSinStock.Count;
                    foreach (var item in ProductosSinStock)
                    {
                        Globals.ProgressbarStatus.i_Progress++;
                        var Producto = ListaProductos.Where(o => o.v_IdProducto == item.IdProducto).FirstOrDefault();
                        Producto.i_EsActivo = 0;
                        Producto.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        Producto.t_ActualizaFecha = DateTime.Now;
                        dbContext.producto.ApplyCurrentValues(Producto);
                    }
                    dbContext.SaveChanges();

                }
                transaction.Complete();
            }

        }
        public List<ReporteStockConsolidadoCajasUnidades> ReporteStockConsolidadoCajasUnidades(ref OperationResult objOperationResult, DateTime FechaInicial, DateTime FechaFinal, string pstrAlmacenes, string Orden, int stock, string pstrLinea, int idEstablecimiento, int StockCero, string CodInternoProducto, int Modelo, int EstadoProducto)
        {

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = from a in dbContext.movimiento


                                join b in dbContext.movimientodetalle on new { IdMovimiento = a.v_IdMovimiento, eliminado = 0 } equals new { IdMovimiento = b.v_IdMovimiento, eliminado = b.i_Eliminado.Value } into b_join

                                from b in b_join.DefaultIfEmpty()

                                join c in dbContext.productodetalle on new { IdProductoDetalle = b.v_IdProductoDetalle, eliminado = 0 } equals new { IdProductoDetalle = c.v_IdProductoDetalle, eliminado = c.i_Eliminado.Value } into c_join

                                from c in c_join.DefaultIfEmpty()

                                join d in dbContext.producto on new { IdProducto = c.v_IdProducto, eliminado = 0 } equals new { IdProducto = d.v_IdProducto, eliminado = d.i_Eliminado.Value } into d_join

                                from d in d_join.DefaultIfEmpty()

                                join e in dbContext.linea on new { IdLinea = d.v_IdLinea, eliminado = 0 } equals new { IdLinea = e.v_IdLinea, eliminado = e.i_Eliminado.Value } into e_join

                                from e in e_join.DefaultIfEmpty()

                                join g in dbContext.datahierarchy on new { i_idUnidadMedidad = d.i_IdUnidadMedida.Value, b = 17, eliminado = 0 }
                                equals new { i_idUnidadMedidad = g.i_ItemId, b = g.i_GroupId, eliminado = g.i_IsDeleted.Value } into g_join
                                from g in g_join.DefaultIfEmpty()


                                join h in dbContext.datahierarchy on new { i_idUnidadElejido = b.i_IdUnidad.Value, b = 17, eliminado = 0 }
                                equals new { i_idUnidadElejido = h.i_ItemId, b = h.i_GroupId, eliminado = h.i_IsDeleted.Value } into h_join
                                from h in h_join.DefaultIfEmpty()

                                where a.t_Fecha >= FechaInicial && a.t_Fecha <= FechaFinal && (d.v_IdLinea == pstrLinea || pstrLinea == "-1") &&
                                      a.i_IdEstablecimiento == idEstablecimiento
                                      && a.i_Eliminado.Value == 0 && (d.v_CodInterno == CodInternoProducto || CodInternoProducto == "")
                                      && a.i_IdTipoMovimiento != (int)TipoDeMovimiento.Transferencia
                                      && d.i_EsServicio == 0 && (d.i_EsActivo == EstadoProducto || EstadoProducto == -1)
                                select new ReporteStockConsolidadoCajasUnidades
                                {
                                    linea = e == null ? "NO EXISTE LINEA" : e.v_Nombre,
                                    codigoProducto = d == null ? "NO EXISTE PRODUCTO" : d.v_CodInterno,
                                    descripcionProducto = d == null ? "NO EXISTE PRODUCTO" : d.v_Descripcion,
                                    tipoMovimiento = a.i_IdTipoMovimiento.Value,
                                    devolucion = a.i_EsDevolucion.Value,
                                    idAlmacen = a.i_IdAlmacenOrigen.Value,
                                    cantidad = b == null ? 0 : b.d_CantidadEmpaque.Value == null ? 0 : b.d_CantidadEmpaque,
                                    v_IdProductoDetalle = b == null ? "" : b.v_IdProductoDetalle,
                                    d_Empaque = d == null ? 0 : d.d_Empaque,
                                    d_CantidadEmpaque = b == null ? 0 : b.d_CantidadEmpaque.Value == null ? 0 : b.d_CantidadEmpaque.Value,
                                    v_idUnidadMedidaEmpaque = g.v_Value2,
                                    v_IdUnidadMedidaElejido = h.v_Value2,
                                    d_Cantidad = b.d_Cantidad,
                                    Fecha = a.t_Fecha.Value,
                                    idMovimiento = a.v_IdMovimiento,
                                    idMovimientoDetalle = b.v_IdMovimientoDetalle,
                                };
                    List<ReporteStockConsolidadoCajasUnidades> queryFinal = new List<ReporteStockConsolidadoCajasUnidades>();
                    if (!string.IsNullOrEmpty(Orden))
                    {
                        var queryOrdenado = query.OrderBy(Orden);
                        queryFinal = queryOrdenado.ToList();
                    }


                    var queryConversion = (from A in queryFinal
                                           select new ReporteStockConsolidadoCajasUnidades
                                           {
                                               linea = A.linea,
                                               codigoProducto = A.codigoProducto,
                                               descripcionProducto = A.descripcionProducto,
                                               tipoMovimiento = A.tipoMovimiento,
                                               devolucion = A.devolucion,
                                               idAlmacen = A.idAlmacen,
                                               cantidad = A.cantidad,// d_CantidaEmpaque
                                               v_IdProductoDetalle = A.v_IdProductoDetalle,
                                               d_Empaque = A.d_Empaque,
                                               d_CantidadEmpaque = A.d_CantidadEmpaque,
                                               i_idUnidadMedidaEmpaque = A.v_idUnidadMedidaEmpaque == null ? 1 : int.Parse(A.v_idUnidadMedidaEmpaque),
                                               i_IdUnidadMedidaElejido = A.v_IdUnidadMedidaElejido == null ? 1 : int.Parse(A.v_IdUnidadMedidaElejido),
                                               d_Cantidad = A.d_Cantidad,
                                               cajas = 0,
                                               unidad = 0,
                                           }).ToList();


                    var queryConversion2 = (from n in queryConversion
                                            select new ReporteStockConsolidadoCajasUnidades
                                            {
                                                linea = n.linea,
                                                codigoProducto = n.codigoProducto,
                                                descripcionProducto = n.descripcionProducto,
                                                tipoMovimiento = n.tipoMovimiento,
                                                devolucion = n.devolucion,
                                                idAlmacen = n.idAlmacen,
                                                cantidad = n.cantidad,
                                                v_IdProductoDetalle = n.v_IdProductoDetalle,
                                                d_Empaque = n.d_Empaque,
                                                d_CantidadEmpaque = n.d_CantidadEmpaque,
                                                //cajas = n.d_CantidadEmpaque == 0 ? n.d_Cantidad == 0 ? 0 : n.d_CantidadEmpaque.Value / (n.d_Cantidad.Value * n.i_idUnidadMedidaEmpaque.Value) : n.d_CantidadEmpaque.Value / (n.d_CantidadEmpaque.Value * n.i_idUnidadMedidaEmpaque.Value),
                                                cajas = n.d_CantidadEmpaque.Value / (n.d_Empaque.Value * n.i_idUnidadMedidaEmpaque.Value),
                                                unidad = n.unidad,
                                                d_Cantidad = n.d_Cantidad,
                                                i_idUnidadMedidaEmpaque = n.i_idUnidadMedidaEmpaque,
                                            });


                    var queryCalculos = (from A in queryConversion2
                                         select new ReporteStockConsolidadoCajasUnidades
                                         {

                                             linea = A.linea,
                                             codigoProducto = A.codigoProducto,
                                             descripcionProducto = A.descripcionProducto,
                                             tipoMovimiento = A.tipoMovimiento,
                                             devolucion = A.devolucion,
                                             idAlmacen = A.idAlmacen,
                                             cantidad = A.cantidad,// d_CantidaEmpaque
                                             v_IdProductoDetalle = A.v_IdProductoDetalle,
                                             d_Empaque = A.d_Empaque,
                                             d_CantidadEmpaque = A.d_CantidadEmpaque,
                                             i_idUnidadMedidaEmpaque = A.i_idUnidadMedidaEmpaque,
                                             i_IdUnidadMedidaElejido = A.i_IdUnidadMedidaElejido,
                                             cajas = decimal.Parse(Math.Round(A.cajas.Value, 0, MidpointRounding.AwayFromZero).ToString()),
                                             // unidad = A.d_CantidadEmpaque == 0 ? A.d_Cantidad == 0 ? 0 : (A.d_Cantidad.Value/ (A.d_Empaque.Value * A.i_idUnidadMedidaEmpaque.Value) - A.cajas.Value) * (A.d_Empaque.Value * A.i_idUnidadMedidaEmpaque.Value) : (A.d_CantidadEmpaque.Value  / (A.d_Empaque.Value  * A.i_idUnidadMedidaEmpaque.Value ) - A.cajas.Value ) * (A.d_Empaque.Value  * A.i_idUnidadMedidaEmpaque.Value), 
                                             // unidad = (A.d_CantidadEmpaque / (A.d_Empaque.Value * A.i_idUnidadMedidaEmpaque.Value)) - decimal.Parse(Math.Round(A.cajas.Value, 0, MidpointRounding.AwayFromZero).ToString()) * (A.d_Empaque.Value * A.i_idUnidadMedidaEmpaque.Value) ,
                                             unidad = ((A.d_CantidadEmpaque / (A.d_Empaque * A.i_idUnidadMedidaEmpaque)) - decimal.Parse(Math.Round(A.cajas.Value, 0, MidpointRounding.AwayFromZero).ToString())) * (A.d_Empaque.Value * A.i_idUnidadMedidaEmpaque.Value),

                                         });



                    var queryList = queryCalculos.ToList();
                    int Contador = queryList.Count;
                    List<ReporteStockConsolidadoCajasUnidades> ListaReporteStockConsolidadoCajasUnidades = new List<ReporteStockConsolidadoCajasUnidades>();
                    List<string> ProductosReporte = new List<string>();

                    for (int i = 0; i < Contador; i++)
                    {

                        if (!ProductosReporte.Contains(queryList[i].codigoProducto))
                        {
                            var Total = ObtenerTotalesCajasUnidades(queryList[i].codigoProducto, queryList, idEstablecimiento);
                            var objReporteStock = new ReporteStockConsolidadoCajasUnidades();
                            objReporteStock.linea = queryList[i].linea;
                            objReporteStock.codigoProducto = queryList[i].codigoProducto.Trim();
                            objReporteStock.descripcionProducto = queryList[i].descripcionProducto.Trim();
                            objReporteStock.almacen1 = Total.almacen1;
                            objReporteStock.almacen2 = Total.almacen2;
                            objReporteStock.almacen3 = Total.almacen3;
                            objReporteStock.almacen4 = Total.almacen4;
                            objReporteStock.almacen5 = Total.almacen5;
                            objReporteStock.almacen6 = Total.almacen6;
                            objReporteStock.almacen7 = Total.almacen7;
                            objReporteStock.almacen8 = Total.almacen8;
                            objReporteStock.almacen9 = Total.almacen9;
                            objReporteStock.almacen10 = Total.almacen10;
                            objReporteStock.cantidadAlmacen1 = Total.cantidadAlmacen1;
                            objReporteStock.cantidadAlmacen2 = Total.cantidadAlmacen2;
                            objReporteStock.cantidadAlmacen3 = Total.cantidadAlmacen3;
                            objReporteStock.cantidadAlmacen4 = Total.cantidadAlmacen4;
                            objReporteStock.cantidadAlmacen5 = Total.cantidadAlmacen5;
                            objReporteStock.cantidadAlmacen6 = Total.cantidadAlmacen6;
                            objReporteStock.cantidadAlmacen7 = Total.cantidadAlmacen7;
                            objReporteStock.cantidadAlmacen8 = Total.cantidadAlmacen8;
                            objReporteStock.cantidadAlmacen9 = Total.cantidadAlmacen9;
                            objReporteStock.cantidadAlmacen10 = Total.cantidadAlmacen10;
                            objReporteStock.cajasAlmacen1 = Total.cajasAlmacen1;
                            objReporteStock.cajasAlmacen2 = Total.cajasAlmacen2;
                            objReporteStock.cajasAlmacen3 = Total.cajasAlmacen3;
                            objReporteStock.cajasAlmacen4 = Total.cajasAlmacen4;
                            objReporteStock.cajasAlmacen5 = Total.cajasAlmacen5;
                            objReporteStock.cajasAlmacen6 = Total.cajasAlmacen6;
                            objReporteStock.cajasAlmacen7 = Total.cajasAlmacen7;
                            objReporteStock.cajasAlmacen8 = Total.cajasAlmacen8;
                            objReporteStock.cajasAlmacen9 = Total.cajasAlmacen9;
                            objReporteStock.cajasAlmacen10 = Total.cajasAlmacen10;

                            ProductosReporte.Add(queryList[i].codigoProducto);
                            ListaReporteStockConsolidadoCajasUnidades.Add(objReporteStock);


                        }
                    }
                    objOperationResult.Success = 1;
                    return ListaReporteStockConsolidadoCajasUnidades;
                }
            }
            catch (Exception e)
            {
                objOperationResult.Success = 0;
                return null;
            }
        }

        public ReporteStockConsolidadoCajasUnidades ObtenerTotalesCajasUnidades(string pstrCodigoProducto, List<ReporteStockConsolidadoCajasUnidades> Lista, int idEstablecimiento)
        {
            ReporteStockConsolidadoCajasUnidades objReporteStock = new ReporteStockConsolidadoCajasUnidades();
            var AlmacenesEstablecimiento = ObtenerAlmacenesxEstablecimiento(idEstablecimiento);



            objReporteStock = new ReporteStockConsolidadoCajasUnidades();
            int Contalmacenestablecimiento = AlmacenesEstablecimiento.Count();
            objReporteStock.almacen1 = 0 < Contalmacenestablecimiento ? AlmacenesEstablecimiento[0].v_Nombre : string.Empty;
            objReporteStock.almacen2 = 1 < Contalmacenestablecimiento ? AlmacenesEstablecimiento[1].v_Nombre : string.Empty;
            objReporteStock.almacen3 = 2 < Contalmacenestablecimiento ? AlmacenesEstablecimiento[2].v_Nombre : string.Empty;
            objReporteStock.almacen4 = 3 < Contalmacenestablecimiento ? AlmacenesEstablecimiento[3].v_Nombre : string.Empty;
            objReporteStock.almacen5 = 4 < Contalmacenestablecimiento ? AlmacenesEstablecimiento[4].v_Nombre : string.Empty;
            objReporteStock.almacen6 = 5 < Contalmacenestablecimiento ? AlmacenesEstablecimiento[5].v_Nombre : string.Empty;
            objReporteStock.almacen7 = 6 < Contalmacenestablecimiento ? AlmacenesEstablecimiento[6].v_Nombre : string.Empty;
            objReporteStock.almacen8 = 7 < Contalmacenestablecimiento ? AlmacenesEstablecimiento[7].v_Nombre : string.Empty;
            objReporteStock.almacen9 = 8 < Contalmacenestablecimiento ? AlmacenesEstablecimiento[8].v_Nombre : string.Empty;
            objReporteStock.almacen10 = 9 < Contalmacenestablecimiento ? AlmacenesEstablecimiento[9].v_Nombre : string.Empty;

            objReporteStock.cantidadAlmacen1 = 0 < Contalmacenestablecimiento ? (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[0].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 0).Sum(x => x.unidad).Value - Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[0].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 1).Sum(x => x.unidad).Value) - (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[0].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 0).Sum(x => x.unidad).Value + Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[0].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 1).Sum(x => x.unidad).Value) : 0;
            objReporteStock.cantidadAlmacen2 = 1 < Contalmacenestablecimiento ? (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[1].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 0).Sum(x => x.unidad).Value - Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[1].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 1).Sum(x => x.unidad).Value) - (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[1].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 0).Sum(x => x.unidad).Value + Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[1].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 1).Sum(x => x.unidad).Value) : 0;
            objReporteStock.cantidadAlmacen3 = 2 < Contalmacenestablecimiento ? (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[2].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 0).Sum(x => x.unidad).Value - Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[2].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 1).Sum(x => x.unidad).Value) - (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[2].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 0).Sum(x => x.unidad).Value + Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[2].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 1).Sum(x => x.unidad).Value) : 0;
            objReporteStock.cantidadAlmacen4 = 3 < Contalmacenestablecimiento ? (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[3].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 0).Sum(x => x.unidad).Value - Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[3].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 1).Sum(x => x.unidad).Value) - (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[3].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 0).Sum(x => x.unidad).Value + Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[3].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 1).Sum(x => x.unidad).Value) : 0;
            objReporteStock.cantidadAlmacen5 = 4 < Contalmacenestablecimiento ? (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[4].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 0).Sum(x => x.unidad).Value - Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[4].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 1).Sum(x => x.unidad).Value) - (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[4].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 0).Sum(x => x.unidad).Value + Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[4].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 1).Sum(x => x.unidad).Value) : 0;
            objReporteStock.cantidadAlmacen6 = 5 < Contalmacenestablecimiento ? (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[5].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 0).Sum(x => x.unidad).Value - Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[5].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 1).Sum(x => x.unidad).Value) - (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[5].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 0).Sum(x => x.unidad).Value + Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[5].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 1).Sum(x => x.unidad).Value) : 0;
            objReporteStock.cantidadAlmacen7 = 6 < Contalmacenestablecimiento ? (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[6].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 0).Sum(x => x.unidad).Value - Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[6].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 1).Sum(x => x.unidad).Value) - (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[6].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 0).Sum(x => x.unidad).Value + Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[6].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 1).Sum(x => x.unidad).Value) : 0;
            objReporteStock.cantidadAlmacen8 = 7 < Contalmacenestablecimiento ? (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[7].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 0).Sum(x => x.unidad).Value - Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[7].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 1).Sum(x => x.unidad).Value) - (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[7].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 0).Sum(x => x.unidad).Value + Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[7].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 1).Sum(x => x.unidad).Value) : 0;
            objReporteStock.cantidadAlmacen9 = 8 < Contalmacenestablecimiento ? (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[8].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 0).Sum(x => x.unidad).Value - Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[8].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 1).Sum(x => x.unidad).Value) - (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[8].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 0).Sum(x => x.unidad).Value + Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[8].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 1).Sum(x => x.unidad).Value) : 0;
            objReporteStock.cantidadAlmacen10 = 9 < Contalmacenestablecimiento ? (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[9].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 0).Sum(x => x.unidad).Value - Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[9].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 1).Sum(x => x.unidad).Value) - (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[9].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 0).Sum(x => x.unidad).Value + Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[9].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 1).Sum(x => x.unidad).Value) : 0;


            objReporteStock.cajasAlmacen1 = 0 < Contalmacenestablecimiento ? (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[0].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 0).Sum(x => x.cajas).Value - Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[0].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 1).Sum(x => x.cajas).Value) - (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[0].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 0).Sum(x => x.cajas).Value + Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[0].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 1).Sum(x => x.cajas).Value) : 0;
            objReporteStock.cajasAlmacen2 = 1 < Contalmacenestablecimiento ? (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[1].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 0).Sum(x => x.cajas).Value - Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[1].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 1).Sum(x => x.cajas).Value) - (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[1].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 0).Sum(x => x.cajas).Value + Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[1].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 1).Sum(x => x.cajas).Value) : 0;
            objReporteStock.cajasAlmacen3 = 2 < Contalmacenestablecimiento ? (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[2].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 0).Sum(x => x.cajas).Value - Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[2].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 1).Sum(x => x.cajas).Value) - (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[2].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 0).Sum(x => x.cajas).Value + Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[2].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 1).Sum(x => x.cajas).Value) : 0;
            objReporteStock.cajasAlmacen4 = 3 < Contalmacenestablecimiento ? (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[3].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 0).Sum(x => x.cajas).Value - Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[3].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 1).Sum(x => x.cajas).Value) - (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[3].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 0).Sum(x => x.cajas).Value + Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[3].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 1).Sum(x => x.cajas).Value) : 0;
            objReporteStock.cajasAlmacen5 = 4 < Contalmacenestablecimiento ? (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[4].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 0).Sum(x => x.cajas).Value - Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[4].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 1).Sum(x => x.cajas).Value) - (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[4].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 0).Sum(x => x.cajas).Value + Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[4].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 1).Sum(x => x.cajas).Value) : 0;
            objReporteStock.cajasAlmacen6 = 5 < Contalmacenestablecimiento ? (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[5].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 0).Sum(x => x.cajas).Value - Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[5].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 1).Sum(x => x.cajas).Value) - (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[5].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 0).Sum(x => x.cajas).Value + Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[5].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 1).Sum(x => x.cajas).Value) : 0;
            objReporteStock.cajasAlmacen7 = 6 < Contalmacenestablecimiento ? (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[6].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 0).Sum(x => x.cajas).Value - Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[6].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 1).Sum(x => x.cajas).Value) - (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[6].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 0).Sum(x => x.cajas).Value + Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[6].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 1).Sum(x => x.cajas).Value) : 0;
            objReporteStock.cajasAlmacen8 = 7 < Contalmacenestablecimiento ? (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[7].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 0).Sum(x => x.cajas).Value - Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[7].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 1).Sum(x => x.cajas).Value) - (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[7].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 0).Sum(x => x.cajas).Value + Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[7].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 1).Sum(x => x.cajas).Value) : 0;
            objReporteStock.cajasAlmacen9 = 8 < Contalmacenestablecimiento ? (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[8].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 0).Sum(x => x.cajas).Value - Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[8].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 1).Sum(x => x.cajas).Value) - (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[8].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 0).Sum(x => x.cajas).Value + Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[8].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 1).Sum(x => x.cajas).Value) : 0;
            objReporteStock.cajasAlmacen10 = 9 < Contalmacenestablecimiento ? (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[9].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 0).Sum(x => x.cajas).Value - Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen.Value == AlmacenesEstablecimiento[9].i_IdAlmacen && x.tipoMovimiento.Value == 1 && x.devolucion.Value == 1).Sum(x => x.cajas).Value) - (Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[9].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 0).Sum(x => x.cajas).Value + Lista.Where(x => x.codigoProducto == pstrCodigoProducto && x.idAlmacen == AlmacenesEstablecimiento[9].i_IdAlmacen && x.tipoMovimiento.Value == 2 && x.devolucion.Value == 1).Sum(x => x.cajas).Value) : 0;
            return objReporteStock;
        }

        public List<ReporteResumenAlmacen> ReporteResumenAlmacen(ref OperationResult objOperationResult, DateTime FechaInicioAnterior, DateTime FechaFinAnterior, DateTime FechaInicio, DateTime FechaFin, string almacenes, int pIntEstablecimiento, string pstrOrdenar, int pIntMoneda, string pstridLinea, string pstrCodigoInterno, string pstrPedido, string IdMarca, int FormatoCantidad, int IncluirPedido, int MostrarSoloMovimiento)
        {

            try
            {

                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    List<ReporteResumenAlmacen> ReporteResumenMovimientosFiltrado = new List<ReporteResumenAlmacen>();
                    List<ReporteResumenAlmacen> ReporteResumenAnteriorFiltrado = new List<ReporteResumenAlmacen>();
                    List<ReporteResumenAlmacen> ReporteResumenTotalFiltrado = new List<ReporteResumenAlmacen>();
                    DateTime Fecha = DateTime.Parse("01/01/" + FechaInicio.Year.ToString());

                    var ReporteTotal = (from a in dbContext.movimiento

                                        join b in dbContext.movimientodetalle on new { IdMovimiento = a.v_IdMovimiento, eliminado = 0 }
                                        equals new { IdMovimiento = b.v_IdMovimiento, eliminado = b.i_Eliminado.Value } into b_join

                                        from b in b_join.DefaultIfEmpty()

                                        join c in dbContext.productodetalle on
                                        new { ProductoDetalle = b.v_IdProductoDetalle, eliminado = 0 } equals
                                        new { ProductoDetalle = c.v_IdProductoDetalle, eliminado = c.i_Eliminado.Value } into c_join

                                        from c in c_join.DefaultIfEmpty()

                                        join d in dbContext.producto on new { IdProducto = c.v_IdProducto, eliminado = 0 } equals
                                        new { IdProducto = d.v_IdProducto, eliminado = d.i_Eliminado.Value } into d_join

                                        from d in d_join.DefaultIfEmpty()

                                        join e in dbContext.linea on new { IdLinea = d.v_IdLinea, eliminado = 0 } equals
                                        new { IdLinea = e.v_IdLinea, eliminado = e.i_Eliminado.Value } into e_join

                                        from e in e_join.DefaultIfEmpty()

                                        join f in dbContext.datahierarchy on
                                        new { i_idUnidadMedida = d.i_IdUnidadMedida.Value, b = 17, eliminado = 0 }
                                        equals new { i_idUnidadMedida = f.i_ItemId, b = f.i_GroupId, eliminado = f.i_IsDeleted.Value }

                                        join g in dbContext.establecimientoalmacen on
                                        new { AlmacenOrigen = a.i_IdAlmacenOrigen, eliminado = 0 } equals
                                        new { AlmacenOrigen = g.i_IdAlmacen, eliminado = g.i_Eliminado.Value } into g_join

                                        from g in g_join.DefaultIfEmpty()

                                        join h in dbContext.almacen on new { IdAlmacen = g.i_IdAlmacen.Value, eliminado = 0 } equals
                                        new { IdAlmacen = h.i_IdAlmacen, eliminado = h.i_Eliminado.Value } into h_join

                                        from h in h_join.DefaultIfEmpty()

                                        join j in dbContext.documento on new { doc = b.i_IdTipoDocumento.Value, eliminado = 0 } equals new { doc = j.i_CodigoDocumento, eliminado = j.i_Eliminado.Value } into j_join
                                        from j in j_join.DefaultIfEmpty()

                                        where a.i_Eliminado == 0 && g.i_IdEstablecimiento == pIntEstablecimiento
                                              && (a.t_Fecha.Value >= Fecha && a.t_Fecha.Value <= FechaFin) &&
                                              (d.v_CodInterno == pstrCodigoInterno || pstrCodigoInterno == "") &&
                                              (b.v_NroPedido.Trim() == pstrPedido || pstrPedido == "")
                                              && a.i_IdTipoMovimiento != (int)TipoDeMovimiento.Transferencia && d.i_EsServicio == 0
                                              && (d.v_IdMarca == IdMarca || IdMarca == "-1") && (d.v_IdLinea == pstridLinea || pstridLinea == "-1")
                                              && d.i_EsActivoFijo == 0
                                        //&& // j.i_UsadoDocumentoContable ==1
                                        orderby a.i_IdAlmacenOrigen, d.v_CodInterno, b.v_NroPedido, a.t_Fecha
                                        select new ReporteResumenAlmacen
                                        {
                                            codigoProducto = d == null ? "PRODUCTO NO EXISTE" : d.v_CodInterno,
                                            descripcionProducto = d == null ? "PRODUCTO NO EXISTE" : d.v_Descripcion.Trim(),
                                            unidadMedida =
                                                d == null
                                                    ? "PRODUCTO NO EXISTE"
                                                    : d.d_Empaque.Value == 1 && f.v_Value2 == "1" ? f.v_Value1 : "UNIDAD",
                                            tipoMovimiento = a.i_IdTipoMovimiento.Value,
                                            cantidad = b.d_CantidadEmpaque == null ? 0 : b.d_CantidadEmpaque.Value,
                                            precio =
                                                d == null
                                                    ? 0
                                                    : d.d_Empaque.Value == 1 && f.v_Value2 == "1"
                                                        ? b.d_Precio
                                                        : b.d_CantidadEmpaque == null
                                                            ? 0
                                                            : b.d_CantidadEmpaque == 0 ? 0 : b.d_Total / b.d_CantidadEmpaque,
                                            valor =
                                                d == null
                                                    ? 0
                                                    : d.d_Empaque.Value == 1 && f.v_Value2 == "1"
                                                        ? b.d_Cantidad.Value * b.d_Precio.Value
                                                        : b.d_CantidadEmpaque == null
                                                            ? 0
                                                            : b.d_CantidadEmpaque == 0
                                                                ? 0
                                                                : (b.d_Total / b.d_CantidadEmpaque) * b.d_CantidadEmpaque,
                                            v_almacen = h == null ? "ALMACÉN NO EXISTE" : h.v_Nombre,
                                            IdAlmacen = a.i_IdAlmacenOrigen.Value,
                                            linea = e == null ? "LINEA NO EXISTE" : e.v_Nombre,
                                            pedido = IncluirPedido == 1 ? b == null ? "" : string.IsNullOrEmpty(b.v_NroPedido) ? "" : b.v_NroPedido.Trim() : "",
                                            pIntMoneda = a.i_IdMoneda.Value,
                                            Devolucion = a.i_EsDevolucion == null ? 0 : a.i_EsDevolucion.Value,
                                            tipoCambio = a.d_TipoCambio,
                                            Fecha = a.t_Fecha.Value,
                                            v_IdProducto = IncluirPedido == 1 ? b.v_NroPedido == null || b.v_NroPedido.Trim() == string.Empty ? d.v_IdProducto : d.v_IdProducto + " " + b.v_NroPedido.Trim() : d.v_IdProducto,

                                        }).ToList().AsQueryable();
                    if (!string.IsNullOrEmpty(almacenes))
                    {
                        ReporteTotal = ReporteTotal.Where(almacenes);
                    }
                    List<KardexList> ReporteResumenAnterior = new List<KardexList>();
                    bool SeUsaCargaInicial = false;
                    if (int.Parse(FechaInicio.Date.Day.ToString()) == 1 &&
                        int.Parse(FechaInicio.Date.Month.ToString()) == 1) //a todos los que tienen Motivo=CargaInicial
                    {
                        ReporteResumenAnterior = ReporteStock(ref objOperationResult, pIntEstablecimiento, DateTime.Parse("01/01/" + Globals.ClientSession.i_Periodo.ToString()), DateTime.Parse("01/01/" + Globals.ClientSession.i_Periodo.ToString()), almacenes, pIntMoneda, pstrCodigoInterno, pstrPedido, pstridLinea, 0, 0, 0, 0, 0, FormatoCantidad, "", FechaInicio, IncluirPedido, null, false, (int)TipoDeMovimiento.NotadeIngreso, false, false);
                        SeUsaCargaInicial = true;
                    }
                    else
                    {

                        ReporteResumenAnterior = ReporteStock(ref objOperationResult, pIntEstablecimiento, FechaInicioAnterior, FechaFinAnterior, almacenes, pIntMoneda, pstrCodigoInterno, pstrPedido, pstridLinea, 0, 0, 0, 0, 0, FormatoCantidad, "", FechaInicio, IncluirPedido, null, false, -1, false);
                        SeUsaCargaInicial = false;
                        if (!ReporteResumenAnterior.Any())
                        {
                            ReporteResumenAnterior = ReporteStock(ref objOperationResult, pIntEstablecimiento, DateTime.Parse("01/01/" + Globals.ClientSession.i_Periodo.ToString()), DateTime.Parse("01/01/" + Globals.ClientSession.i_Periodo.ToString()), almacenes, pIntMoneda, pstrCodigoInterno, pstrPedido, pstridLinea, 0, 0, 0, 0, 0, FormatoCantidad, "", FechaInicio, IncluirPedido, null, false, (int)TipoDeMovimiento.NotadeIngreso, false, false);
                            SeUsaCargaInicial = true;
                        }
                    }
                    List<KardexList> MovimientoMes = new List<KardexList>();

                    // MovimientoMes = ReporteStock(ref objOperationResult, pIntEstablecimiento, DateTime.Parse("01/01/" + Globals.ClientSession.i_Periodo.ToString()), FechaFin, almacenes, pIntMoneda, pstrCodigoInterno, pstrPedido, pstridLinea, 0, 0, 0, 0, 0, FormatoCantidad, "", FechaInicio,IncluirPedido , null, false, -1, true);
                    MovimientoMes = ReporteStock(ref objOperationResult, pIntEstablecimiento, DateTime.Parse("01/01/" + Globals.ClientSession.i_Periodo.ToString()), FechaFin, almacenes, pIntMoneda, pstrCodigoInterno, pstrPedido, pstridLinea, 0, 0, 0, 0, 0, FormatoCantidad, "", FechaInicio, IncluirPedido, null, false, -1, true, false, SeUsaCargaInicial);
                    ReporteResumenAlmacen _objreporteresumenAlmacen = new ReporteResumenAlmacen();
                    List<ReporteResumenAlmacen> ListaProductosReporte = new List<ReporteResumenAlmacen>();
                    ReporteResumenAlmacen objProductoReporte = new ReporteResumenAlmacen();
                    List<ReporteResumenAlmacen> ListaFinal = new List<ReporteResumenAlmacen>();
                    int i = -1;

                    var ReporteResumenAnteriorDictionary = ReporteResumenAnterior.ToDictionary(o => o.v_IdProducto, o => o);
                    var MovimientoMesDictionary = MovimientoMes.ToDictionary(o => o.v_IdProducto, o => o);
                    ReporteTotal = ReporteTotal.GroupBy(o => o.v_IdProducto).Select(o => o.LastOrDefault()).OrderBy(o => o.v_IdProducto);

                    foreach (ReporteResumenAlmacen objTotal in ReporteTotal)
                    {


                        objProductoReporte = new ReporteResumenAlmacen();
                        objProductoReporte = objTotal;
                        KardexList prodBuscarAnterior;
                        prodBuscarAnterior = ReporteResumenAnteriorDictionary.TryGetValue(objTotal.v_IdProducto, out prodBuscarAnterior) ? prodBuscarAnterior : new KardexList();
                        objProductoReporte.cantidadAnterior = prodBuscarAnterior == null ? 0 : prodBuscarAnterior.Saldo_Cantidad ?? 0;
                        objProductoReporte.precioAnterior = prodBuscarAnterior == null ? 0 : prodBuscarAnterior.Saldo_Precio ?? 0;
                        objProductoReporte.valorAnterior = prodBuscarAnterior == null ? 0 : prodBuscarAnterior.Saldo_Total ?? 0;
                        KardexList prodMovimientoMes;
                        prodMovimientoMes = MovimientoMesDictionary.TryGetValue(objTotal.v_IdProducto, out prodBuscarAnterior) ? prodBuscarAnterior : new KardexList();


                        objProductoReporte.cantidadEntrada = prodMovimientoMes == null ? 0 : prodMovimientoMes.SumatoriaIngresos;
                        objProductoReporte.valorEntrada = prodMovimientoMes == null ? 0 : prodMovimientoMes.SumatoriaTotalIngresos;

                        objProductoReporte.cantidadSalida = prodMovimientoMes == null ? 0 : prodMovimientoMes.SumatoriaSalidas;
                        objProductoReporte.valorSalida = prodMovimientoMes == null ? 0 : prodMovimientoMes.SumatoriaTotalSalidas;
                        objProductoReporte.precioEntrada = prodMovimientoMes == null ? 0 : prodMovimientoMes.SumatoriaPreciosIngresos;
                        objProductoReporte.precioSalida = prodMovimientoMes == null ? 0 : prodMovimientoMes.SumatoriaPreciosSalidas;

                        objProductoReporte.cantidadActual = (objProductoReporte.cantidadAnterior + objProductoReporte.cantidadEntrada) - objProductoReporte.cantidadSalida;
                        objProductoReporte.valorActual = (objProductoReporte.valorAnterior + objProductoReporte.valorEntrada) - objProductoReporte.valorSalida;
                        objProductoReporte.precioActual = objProductoReporte.cantidadActual == 0 ? 0 : objProductoReporte.valorActual / objProductoReporte.cantidadActual;  /// (objProductoReporte.precioAnterior + objProductoReporte.precioEntrada) - objProductoReporte.precioSalida;
                        ListaFinal.Add(objProductoReporte);
                    }
                    if (MostrarSoloMovimiento == 1)
                    {
                        //ListaFinal = ListaFinal.Where(o => o.cantidadAnterior != 0 && o.cantidadEntrada != 0 && o.cantidadSalida != 0 && o.cantidadActual != 0).ToList();
                        ListaFinal = ListaFinal.Where(o => o.cantidadAnterior != 0 && o.cantidadActual != 0).ToList();
                    }
                    objOperationResult.Success = 1;
                    if (pstrOrdenar == "CODIGOPRODUCTO")
                    {
                        return ListaFinal.OrderBy(x => x.codigoProducto).ToList();
                    }
                    else
                    {

                        return ListaFinal.OrderBy(x => x.descripcionProducto).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;
            }
        }

        public List<KardexList> ResumenMovimientos(ref OperationResult pobjOperationResult, int Establecimiento, DateTime? pdtFechaInicio, DateTime? pdtFechaFin, string pstrFilterExpression, int pintIdMoneda, string pstrCodigoProducto, string Pedido, string Linea, int SoloStock0, int SoloStock, int SoloStockNegativo, int SoloStockMinimo, int ConsideraDocInternos, int FormatoCant, string Modelo, DateTime FechaRepResumenAlmaMov, int IncluirNroPedido, string IdMovimientoDetalleConsumo = null, bool AsientoConsumo = false, int TipoMovimiento = -1, bool RepResumenAlma = false, bool RepResumenMov = false)
        {

            var aptitudeCertificate = new AlmacenBL().ReporteStock(ref pobjOperationResult, Establecimiento, pdtFechaInicio, pdtFechaFin, "", pintIdMoneda, pstrCodigoProducto, Pedido, Linea, 0, 0, 0, 0, 0, FormatoCant, "", FechaRepResumenAlmaMov, 0, null, false, -1, RepResumenAlma, RepResumenMov);
            var ListaProductos = aptitudeCertificate.Select(o => o.v_IdProducto).Distinct().ToList();
            List<KardexList> ListaResumen = new List<KardexList>();
            foreach (var prod in ListaProductos)
            {
                KardexList objReporte = new KardexList();
                var Ingresos = aptitudeCertificate.Where(o => o.i_IdTipoMovimiento == 1 && o.v_IdProducto == prod).LastOrDefault();
                var Salidas = aptitudeCertificate.Where(o => o.i_IdTipoMovimiento == 2 && o.v_IdProducto == prod).LastOrDefault();
                objReporte = aptitudeCertificate.Where(o => o.v_IdProducto == prod).FirstOrDefault();
                objReporte.Ingreso_Cantidad = Ingresos != null ? Ingresos.SumatoriaIngresos : 0;
                objReporte.Ingreso_Precio = Ingresos != null ? Ingresos.Saldo_Precio : 0;
                objReporte.Ingreso_Total = Ingresos != null ? Ingresos.Saldo_Total : 0;
                objReporte.Salida_Cantidad = Salidas != null ? Salidas.SumatoriaSalidas : 0;
                objReporte.Salida_Precio = Salidas != null ? Salidas.Saldo_Precio : 0;
                objReporte.Salida_Total = Salidas != null ? Salidas.Saldo_Total : 0;

                ListaResumen.Add(objReporte);
            }

            return ListaResumen;

        }

        public List<almacenDto> ObtenerAlmacenesxEstablecimiento(int idEstablecimiento, int IdAlmacen = -1)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {

                if (idEstablecimiento == -1)
                {
                    var almacenes = (from a in dbContext.almacen
                                     where a.i_Eliminado == 0
                                     orderby a.v_Nombre
                                     select new almacenDto
                                     {
                                         i_IdAlmacen = a.i_IdAlmacen,
                                         v_Nombre = a.v_Nombre,
                                     }).ToList();
                    return almacenes;
                }
                else
                {
                    if (IdAlmacen != -1)
                    {

                        var almacenes = (from n in dbContext.establecimientoalmacen

                                         join m in dbContext.almacen on new { a = n.i_IdAlmacen.Value, eliminado = 0 } equals new { a = m.i_IdAlmacen, eliminado = m.i_Eliminado.Value } into m_join

                                         from m in m_join
                                         where n.i_IdEstablecimiento == idEstablecimiento && n.i_Eliminado == 0 && m.i_IdAlmacen == IdAlmacen
                                         select new almacenDto
                                         {
                                             i_IdAlmacen = n.i_IdAlmacen.Value,
                                             v_Nombre = m.v_Nombre,

                                         }).ToList();
                        return almacenes;

                    }
                    else
                    {
                        var almacenes = (from n in dbContext.establecimientoalmacen

                                         join m in dbContext.almacen on new { a = n.i_IdAlmacen.Value, eliminado = 0 } equals new { a = m.i_IdAlmacen, eliminado = m.i_Eliminado.Value } into m_join

                                         from m in m_join
                                         where n.i_IdEstablecimiento == idEstablecimiento && n.i_Eliminado == 0
                                         select new almacenDto
                                         {
                                             i_IdAlmacen = n.i_IdAlmacen.Value,
                                             v_Nombre = m.v_Nombre,

                                         }).ToList();
                        return almacenes;
                    }
                }
            }
        }

        public string NombreEstablecimiento(int pinEstablecimiento)
        {

            using (var dbContext = new SAMBHSEntitiesModelWin())
            {

                var est = (from a in dbContext.establecimiento

                           where a.i_IdEstablecimiento == pinEstablecimiento

                           select a).FirstOrDefault();

                return est.v_Nombre;
            }
        }

        // Kardex Antiguo demoraba mucho en ingresar a la consulta, se cambio  Kardex Ultima Version  12 abril 2017
        /*public List<KardexList> ReporteKardexValorizadoSunat_(ref OperationResult pobjOperationResult, DateTime? pdtFechaInicio, DateTime? pdtFechaFin, string pstrFilterExpression, int pintIdMoneda, string pstrEmpresa, string pstrRUC, string pstrMoneda, int idEstablecimiento, int IncluirPedido, string pstrCodigoProducto, string pstrNumeroPedido, int ArticulosStockMayor0, int SoloArticulosMovimiento, int ConsideraDocInternos, int FormatoCant, string IdMarca, bool ConsiderarGuiasCompras, int ArticuloStockNegativo, int ArticuloStockCero, string IdLinea, DateTime FechaInicioReport, bool LibroInventario, string OrdenLibroInventario, string Modelo, string pstrGrupoLlave = "", int IdZona = -1)// List<KardexList> ListaSaldosAnteriores = null)
        {

            //Inicialmente en cantidad se toma todas las cantidadempaque , si son nulos (que no deberia pasar) se toma las cantidades
            //En precio estoy capturando los totales de las entradas y/o salidas , luego el precio propiamente dicho se calculara dividiendo el total /cantidad empaque 

            pobjOperationResult.Success = 1;
            string IdProductoOld = "";
            int ContadorError = 1;


            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    List<KardexList> Lista = new List<KardexList>();
                    List<KardexList> ListaUltimosRegistros = new List<KardexList>();
                    List<KardexList> queryFinal = new List<KardexList>();
                    List<KardexList> Consultas = new List<KardexList>();
                    KardexList oKardexList;
                    string Periodo = Globals.ClientSession.i_Periodo.ToString();
                    var LineaDictionary = dbContext.linea.ToList().ToDictionary(o => o.v_IdLinea, o => o);
                    var TipoMovimientos = dbContext.datahierarchy.Where(o => o.i_IsDeleted == 0 && o.i_GroupId == 19).ToList();
                    if (!ConsiderarGuiasCompras)
                    {
                        #region NoConsideraGuiaCompras
                     
                        if (IncluirPedido == 1 && (ArticulosStockMayor0 == 1 || ArticuloStockCero == 1 || ArticuloStockNegativo == 1))
                        
                        {

                            var query = (from A in dbContext.movimientodetalle
                                         join B in dbContext.productodetalle on new { IdProductoDetalle = A.v_IdProductoDetalle, eliminado = 0 } equals new { IdProductoDetalle = B.v_IdProductoDetalle, eliminado = B.i_Eliminado.Value } into B_join
                                         from B in B_join.DefaultIfEmpty()
                                         join C in dbContext.producto on new { IdProducto = B.v_IdProducto, eliminado = 0 } equals new { IdProducto = C.v_IdProducto, eliminado = C.i_Eliminado.Value } into C_join
                                         from C in C_join.DefaultIfEmpty()
                                         join D in dbContext.movimiento on new { A.v_IdMovimiento, eliminado = 0 } equals new { D.v_IdMovimiento, eliminado = D.i_Eliminado.Value } into D_join
                                         from D in D_join.DefaultIfEmpty()
                                         join E in dbContext.cliente on new { IdCliente = D.v_IdCliente, eliminado = 0 } equals new { IdCliente = E.v_IdCliente, eliminado = E.i_Eliminado.Value } into E_join
                                         from E in E_join.DefaultIfEmpty()

                                         join G in dbContext.almacen on new { IdAlmacen = D.i_IdAlmacenOrigen.Value, eliminado = 0 } equals new { IdAlmacen = G.i_IdAlmacen, eliminado = G.i_Eliminado.Value } into G_join
                                         from G in G_join.DefaultIfEmpty()

                                         join H in dbContext.documento on new { IdTipoDoc = A.i_IdTipoDocumento.Value, eliminado = 0 } equals new { IdTipoDoc = H.i_CodigoDocumento, eliminado = H.i_Eliminado.Value } into H_join
                                         from H in H_join.DefaultIfEmpty()

                                         join K in dbContext.datahierarchy on new { Grupo = 17, UnidadMedida = C.i_IdUnidadMedida.Value, eliminado = 0 } equals new { Grupo = K.i_GroupId, UnidadMedida = K.i_ItemId, eliminado = K.i_IsDeleted.Value } into K_join
                                         from K in K_join.DefaultIfEmpty()

                                         join I in dbContext.datahierarchy on new { Grupo = 6, TipoProd = C.i_IdTipoProducto ?? -1, eliminado = 0 } equals new { Grupo = I.i_GroupId, TipoProd = I.i_ItemId, eliminado = I.i_IsDeleted.Value } into I_join
                                         from I in I_join.DefaultIfEmpty()

                                         join J4 in dbContext.marca on new { marc = C.v_IdMarca, eliminado = 0 } equals new { marc = J4.v_IdMarca, eliminado = J4.i_Eliminado.Value } into J4_join
                                         from J4 in J4_join.DefaultIfEmpty()
                                         where A.i_Eliminado == 0 && D.i_IdTipoMovimiento != (int)TipoDeMovimiento.Transferencia
                                         && (D.t_Fecha.Value >= pdtFechaInicio && D.t_Fecha.Value <= pdtFechaFin)
                                         && D.i_IdEstablecimiento == idEstablecimiento
                                         && (C.v_CodInterno.Trim() == pstrCodigoProducto.Trim() || pstrCodigoProducto == "")
                                         && (A.v_NroPedido == pstrNumeroPedido || pstrNumeroPedido == "")
                                         && C.i_EsServicio == 0
                                         && (C.v_IdMarca == IdMarca || IdMarca == "-1")
                                         && C.i_EsActivoFijo == 0
                                         && (C.v_IdLinea == IdLinea || IdLinea == "-1")
                                         && (D.v_OrigenTipo == null || D.v_OrigenTipo == "" || D.v_OrigenTipo == Constants.OrigenCompra || D.v_OrigenTipo == Constants.OrigenImportacion || D.v_OrigenTipo == Constants.OrigenTransferencia || D.v_OrigenTipo == Constants.OrigenVenta || D.v_OrigenTipo == Constants.OrigenGuiaInterna || (!string.IsNullOrEmpty(D.v_OrigenTipo) && D.v_OrigenTipo != Constants.OrigenGuiaCompra))
                                         && (C.v_Modelo == Modelo || Modelo == "")
                                         select new KardexList
                                         {
                                             v_IdMovimientoDetalle = A.v_IdMovimientoDetalle,
                                             v_IdProducto = C.v_IdProducto,
                                             v_NombreProducto = A.v_NroPedido == null ? "PRODUCTO :" + C.v_CodInterno.Trim() + " " + C.v_Descripcion.Trim() + "  PEDIDO :" : "PRODUCTO :" + C.v_CodInterno.Trim() + " " + C.v_Descripcion.Trim() + "  PEDIDO :" + A.v_NroPedido.Trim()  ,
                                             //v_NombreProducto = IncluirPedido ==1 ? A.v_NroPedido == null ? "PRODUCTO :" + C.v_CodInterno.Trim() + " " + C.v_Descripcion.Trim() + "  PEDIDO :" : "PRODUCTO :" + C.v_CodInterno.Trim() + " " + C.v_Descripcion.Trim() + "  PEDIDO :" + A.v_NroPedido.Trim() :  "PRODUCTO :" + C.v_CodInterno.Trim() + " " + C.v_Descripcion.Trim()
                                             v_NombreProductoInventarioSunat = FormatoCant == (int)FormatoCantidad.UnidadMedidaProducto ? "CODIGO EXISTENCIA : " + C.v_CodInterno.Trim() + " \n" + "TIPO : " + I.v_Value2 + " " + I.v_Value1 + "\n" + "DESCRIPCIÓN : " + C.v_Descripcion + "\n" + "UNIDAD MEDIDA : " + K.v_Value4 + " " + K.v_Value1 : "CODIGO EXISTENCIA : " + C.v_CodInterno.Trim() + " \n" + "TIPO : " + I.v_Value2 + " " + I.v_Value1 + "\n" + "DESCRIPCIÓN : " + C.v_Descripcion + "\n" + "UNIDAD MEDIDA : 07 UNIDAD",
                                             t_Fecha = D.t_Fecha,
                                             i_IdTipoMovimiento = D.i_IdTipoMovimiento,
                                             d_Cantidad = A.d_CantidadEmpaque == null ? A.d_Cantidad == null ? 0 : A.d_Cantidad : A.d_CantidadEmpaque,
                                             d_Precio = D.v_OrigenTipo == "I" ? pintIdMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : A.d_Total,
                                             i_EsDevolucion = D.i_EsDevolucion,
                                             i_IdTipoMotivo = D.i_IdTipoMotivo,
                                             NroPedido = A.v_NroPedido == null ? "" : A.v_NroPedido,
                                             //NroPedido =IncluirPedido ==1 ? A.v_NroPedido == null ? "" : A.v_NroPedido:"",
                                             v_IdLinea = C.v_IdLinea,
                                             IdMoneda = D.i_IdMoneda.Value,
                                             TipoCambio = D.d_TipoCambio.Value,
                                             Almacen = G.v_Nombre,
                                             ClienteProveedor = D.v_OrigenTipo == "T" ? D.v_Glosa == null || D.v_Glosa == string.Empty ? "TRANSFERENCIA " : D.v_Glosa : E == null ? "**CLIENTE/PROVEEDOR  NO EXISTE**" : (E.v_ApePaterno + " " + E.v_ApeMaterno + " " + E.v_PrimerNombre + " " + E.v_RazonSocial).Trim(),
                                             Guia = A.v_NroGuiaRemision,
                                             Documento = H == null ? A.v_NumeroDocumento : H.v_Siglas.Trim() + " " + A.v_NumeroDocumento.Trim(),
                                             DocumentoInventarioSunat = string.IsNullOrEmpty(A.v_NumeroDocumento) ? "" : A.v_NumeroDocumento,
                                             IdAlmacen = D.i_IdAlmacenOrigen.Value,

                                             TipoMotivo = D.i_IdTipoMotivo == 5 ? 1 : D.v_OrigenTipo == "T" ? 3 : 2,
                                             IdAlmacenOrigen = D.i_IdAlmacenOrigen.Value,
                                             NroRegistro = D.v_Mes + "-" + D.v_Correlativo,
                                             i_IdTipoDocumentoDetalle = A.i_IdTipoDocumento == null ? -1 : A.i_IdTipoDocumento.Value,
                                             EsDocumentoInterno = H.i_UsadoDocumentoInterno == null ? 0 : H.i_UsadoDocumentoInterno.Value,
                                             EsDocInverso = H.i_UsadoDocumentoInverso == null ? 0 : H.i_UsadoDocumentoInverso.Value,
                                             ValorUM = K.v_Value2,
                                             v_IdMarca = C.v_IdMarca,
                                             Origen = D.v_OrigenTipo,
                                             d_Total = D.v_OrigenTipo == "I" ? pintIdMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : A.d_Total,
                                             i_IdTipoProducto = C == null ? -1 : C.i_IdTipoProducto ?? 0,
                                             CodProducto = C == null ? "" : C.v_CodInterno.Trim(),
                                             TipoExistencia = I == null ? "" : I.v_Value2,
                                             UnidadMedidaProducto = K.v_Value1,
                                             Modelo = C == null ? "" : string.IsNullOrEmpty(C.v_Modelo) ? "" : C.v_Modelo,
                                             Marca = J4 == null || string.IsNullOrEmpty(J4.v_Nombre) ? "" : J4.v_Nombre,
                                             Ubicacion = C == null || string.IsNullOrEmpty(C.v_Ubicacion) ? "" : C.v_Ubicacion,
                                             NroParte = C == null || string.IsNullOrEmpty(C.v_NroParte) ? "" : C.v_NroParte,


                                         }).ToList().AsQueryable();



                            if (!string.IsNullOrEmpty(pstrFilterExpression))
                            {
                                query = query.Where(pstrFilterExpression);
                            }

                            queryFinal = query.ToList().OrderBy(x => x.IdAlmacenOrigen).ThenBy(x => x.v_NombreProducto).ThenBy(x => x.t_Fecha).ThenBy(x => x.i_IdTipoMovimiento).ThenBy(x => x.TipoMotivo).ThenBy(x => x.NroPedido).ThenBy(o => o.v_IdMovimientoDetalle).ToList();


                        }
                        else if (IncluirPedido == 1) // Sunat
                        {

                            var query = (from A in dbContext.movimientodetalle
                                         join B in dbContext.productodetalle on new { IdProductoDetalle = A.v_IdProductoDetalle, eliminado = 0 } equals new { IdProductoDetalle = B.v_IdProductoDetalle, eliminado = B.i_Eliminado.Value } into B_join
                                         from B in B_join.DefaultIfEmpty()
                                         join C in dbContext.producto on new { IdProducto = B.v_IdProducto, eliminado = 0 } equals new { IdProducto = C.v_IdProducto, eliminado = C.i_Eliminado.Value } into C_join
                                         from C in C_join.DefaultIfEmpty()
                                         join D in dbContext.movimiento on new { IdMovimiento = A.v_IdMovimiento, eliminado = 0 } equals new { IdMovimiento = D.v_IdMovimiento, eliminado = D.i_Eliminado.Value } into D_join
                                         from D in D_join.DefaultIfEmpty()
                                         join E in dbContext.cliente on new { IdCliente = D.v_IdCliente, eliminado = 0 } equals new { IdCliente = E.v_IdCliente, eliminado = E.i_Eliminado.Value } into E_join
                                         from E in E_join.DefaultIfEmpty()
                                         join G in dbContext.almacen on new { IdAlmacen = D.i_IdAlmacenOrigen.Value, eliminado = 0 } equals new { IdAlmacen = G.i_IdAlmacen, eliminado = G.i_Eliminado.Value } into G_join
                                         from G in G_join.DefaultIfEmpty()

                                         join H in dbContext.documento on new { IdTipoDoc = A.i_IdTipoDocumento.Value, eliminado = 0 } equals new { IdTipoDoc = H.i_CodigoDocumento, eliminado = H.i_Eliminado.Value } into H_join
                                         from H in H_join.DefaultIfEmpty()

                                         join I in dbContext.datahierarchy on new { Grupo = 17, eliminado = 0, UnidadMedida = C.i_IdUnidadMedida.Value } equals new { Grupo = I.i_GroupId, eliminado = I.i_IsDeleted.Value, UnidadMedida = I.i_ItemId } into I_join
                                         from I in I_join.DefaultIfEmpty()
                                         join K in dbContext.datahierarchy on new { Grupo = 6, TipoProd = C.i_IdTipoProducto ?? -1, eliminado = 0 } equals new { Grupo = K.i_GroupId, TipoProd = K.i_ItemId, eliminado = K.i_IsDeleted.Value } into K_join
                                         from K in K_join.DefaultIfEmpty()
                                         join J4 in dbContext.marca on new { marc = C.v_IdMarca, eliminado = 0 } equals new { marc = J4.v_IdMarca, eliminado = J4.i_Eliminado.Value } into J4_join
                                         from J4 in J4_join.DefaultIfEmpty()
                                         where A.i_Eliminado == 0 && D.i_IdTipoMovimiento != (int)TipoDeMovimiento.Transferencia
                                         && (D.t_Fecha.Value >= pdtFechaInicio && D.t_Fecha.Value <= pdtFechaFin.Value)
                                         && D.i_IdEstablecimiento == idEstablecimiento
                                         && (C.v_CodInterno == pstrCodigoProducto || pstrCodigoProducto == "")
                                         && (A.v_NroPedido.Trim() == pstrNumeroPedido || pstrNumeroPedido == "")
                                         && C.i_EsServicio == 0
                                         && (C.v_IdMarca == IdMarca || IdMarca == "-1")
                                         && C.i_EsActivoFijo == 0
                                         && (C.v_IdLinea == IdLinea || IdLinea == "-1")
                                         && B_join.Any(p => p.v_IdProductoDetalle == A.v_IdProductoDetalle)
                                         && (D.v_OrigenTipo == null || D.v_OrigenTipo == "" || D.v_OrigenTipo == Constants.OrigenCompra || D.v_OrigenTipo == Constants.OrigenImportacion || D.v_OrigenTipo == Constants.OrigenTransferencia || D.v_OrigenTipo == Constants.OrigenVenta || D.v_OrigenTipo == Constants.OrigenGuiaInterna || (!string.IsNullOrEmpty(D.v_OrigenTipo) && D.v_OrigenTipo != Constants.OrigenGuiaCompra))// No toma en cuentas las GUIAS DE COMPRA
                                         && (C.v_Modelo == Modelo || Modelo == "")

                                         select new KardexList
                                         {
                                             v_IdMovimientoDetalle = A.v_IdMovimientoDetalle,
                                             v_IdProducto = C == null ? "" : C.v_IdProducto,
                                             v_NombreProducto = A.v_NroPedido == null ? "PRODUCTO :" + C.v_CodInterno.Trim() + " " + C.v_Descripcion.Trim() + "  PEDIDO :" : "PRODUCTO :" + C.v_CodInterno.Trim() + " " + C.v_Descripcion.Trim() + "  PEDIDO :" + A.v_NroPedido.Trim(),
                                             v_NombreProductoInventarioSunat = FormatoCant == (int)FormatoCantidad.UnidadMedidaProducto ? "CODIGO EXISTENCIA : " + C.v_CodInterno.Trim() + " \n" + "TIPO : " + K.v_Value2 + " " + K.v_Value1 + "\n" + "DESCRIPCIÓN : " + C.v_Descripcion + "\n" + "UNIDAD MEDIDA : " + I.v_Value4 + " " + I.v_Value1 : "CODIGO EXISTENCIA : " + C.v_CodInterno.Trim() + " \n" + "TIPO : " + K.v_Value2 + " " + K.v_Value1 + "\n" + "DESCRIPCIÓN : " + C.v_Descripcion + "\n" + "UNIDAD MEDIDA : 07 UNIDAD",
                                             t_Fecha = D.t_Fecha,
                                             i_IdTipoMovimiento = D.i_IdTipoMovimiento,
                                             d_Cantidad = A.d_CantidadEmpaque == null ? A.d_Cantidad == null ? 0 : A.d_Cantidad : A.d_CantidadEmpaque,
                                             d_Precio = D.v_OrigenTipo == "I" ? pintIdMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : A.d_Total,
                                             i_EsDevolucion = D.i_EsDevolucion,
                                             i_IdTipoMotivo = D.i_IdTipoMotivo,
                                             NroPedido = A.v_NroPedido == null ? "" : A.v_NroPedido,
                                             v_IdLinea = C.v_IdLinea,
                                             IdMoneda = D.i_IdMoneda.Value,
                                             TipoCambio = D.d_TipoCambio.Value,
                                             Almacen = G.v_Nombre,
                                             ClienteProveedor = D.v_OrigenTipo == "T" ? D.v_Glosa == null || D.v_Glosa == string.Empty ? "TRANSFERENCIA " : D.v_Glosa : E == null ? "**CLIENTE/PROVEEDOR  NO EXISTE**" : (E.v_ApePaterno + " " + E.v_ApeMaterno + " " + E.v_PrimerNombre + " " + E.v_RazonSocial).Trim(),
                                             Guia = A.v_NroGuiaRemision,
                                             Documento = H == null ? A.v_NumeroDocumento : H.v_Siglas.Trim() + " " + A.v_NumeroDocumento.Trim(),
                                             DocumentoInventarioSunat = string.IsNullOrEmpty(A.v_NumeroDocumento) ? "" : A.v_NumeroDocumento,
                                             IdAlmacen = D.i_IdAlmacenOrigen.Value,
                                             TipoMotivo = D.i_IdTipoMotivo == 5 ? 1 : D.v_OrigenTipo == "T" ? 3 : 2,
                                             IdAlmacenOrigen = D.i_IdAlmacenOrigen.Value,
                                             NroRegistro = D.v_Mes + "-" + D.v_Correlativo,
                                             i_IdTipoDocumentoDetalle = A.i_IdTipoDocumento == null ? -1 : A.i_IdTipoDocumento.Value,
                                             EsDocumentoInterno = H.i_UsadoDocumentoInterno == null ? 0 : H.i_UsadoDocumentoInterno.Value,
                                             EsDocInverso = H.i_UsadoDocumentoInverso == null ? 0 : H.i_UsadoDocumentoInverso.Value,
                                             ValorUM = I.v_Value2,
                                             v_IdMarca = C.v_IdMarca,
                                             Origen = D.v_OrigenTipo,
                                             d_Total = D.v_OrigenTipo == "I" ? pintIdMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : A.d_Total,
                                             i_IdTipoProducto = C == null ? -1 : C.i_IdTipoProducto ?? 0,
                                             CodProducto = C == null ? "" : C.v_CodInterno.Trim(),
                                             TipoExistencia = K == null ? "" : K.v_Value2,
                                             DescripcionProducto = C == null ? "" : C.v_Descripcion,
                                             UnidadMedidaProducto = I.v_Value1,
                                             Modelo = C == null || string.IsNullOrEmpty(C.v_Modelo) ? "" : C.v_Modelo,
                                             Marca = J4 == null || string.IsNullOrEmpty(J4.v_Nombre) ? "" : J4.v_Nombre,
                                             Ubicacion = C == null || string.IsNullOrEmpty(C.v_Ubicacion) ? "" : C.v_Ubicacion,
                                             NroParte = C == null || string.IsNullOrEmpty(C.v_NroParte) ? "" : C.v_NroParte,
                                             UnidadMedida = I == null || I.v_Value4.Trim() == null || I.v_Value4.Trim() == "" ? "07" : I.v_Value4,

                                         }).ToList().AsQueryable();

                            if (!string.IsNullOrEmpty(pstrFilterExpression))
                            {
                                query = query.Where(pstrFilterExpression);
                            }
                            queryFinal = query.ToList().OrderBy(x => x.IdAlmacenOrigen).ThenBy(x => x.v_NombreProducto).ThenBy(x => x.t_Fecha).ThenBy(x => x.i_IdTipoMovimiento).ThenBy(x => x.TipoMotivo).ThenBy(x => x.NroPedido).ThenBy(o => o.v_IdMovimientoDetalle).ToList();

                        }
                        else if (IncluirPedido == 0)
                        {

                            var query = (from A in dbContext.movimientodetalle
                                         join B in dbContext.productodetalle on new { IdProductoDetalle = A.v_IdProductoDetalle, eliminado = 0 } equals new { IdProductoDetalle = B.v_IdProductoDetalle, eliminado = B.i_Eliminado.Value } into B_join
                                         from B in B_join.DefaultIfEmpty()
                                         join C in dbContext.producto on new { IdProducto = B.v_IdProducto, eliminado = 0 } equals new { IdProducto = C.v_IdProducto, eliminado = C.i_Eliminado.Value } into C_join
                                         from C in C_join.DefaultIfEmpty()
                                         join D in dbContext.movimiento on new { IdMovimiento = A.v_IdMovimiento, eliminado = 0 } equals new { IdMovimiento = D.v_IdMovimiento, eliminado = D.i_Eliminado.Value } into D_join
                                         from D in D_join.DefaultIfEmpty()
                                         join E in dbContext.cliente on new { IdCliente = D.v_IdCliente, eliminado = 0 } equals new { IdCliente = E.v_IdCliente, eliminado = E.i_Eliminado.Value } into E_join
                                         from E in E_join.DefaultIfEmpty()
                                         join G in dbContext.almacen on new { pIntAlmacen = D.i_IdAlmacenOrigen.Value, eliminado = 0 } equals new { pIntAlmacen = G.i_IdAlmacen, eliminado = G.i_Eliminado.Value } into G_join
                                         from G in G_join.DefaultIfEmpty()

                                         join H in dbContext.documento on new { IdTipoDoc = A.i_IdTipoDocumento.Value, eliminado = 0 } equals new { IdTipoDoc = H.i_CodigoDocumento, eliminado = H.i_Eliminado.Value } into H_join
                                         from H in H_join.DefaultIfEmpty()
                                         join I in dbContext.datahierarchy on new { Grupo = 17, eliminado = 0, Um = C.i_IdUnidadMedida.Value } equals new { Grupo = I.i_GroupId, eliminado = I.i_IsDeleted.Value, Um = I.i_ItemId } into I_join
                                         from I in I_join.DefaultIfEmpty()
                                         join K in dbContext.datahierarchy on new { Grupo = 6, TipoProd = C.i_IdTipoProducto ?? -1, eliminado = 0 } equals new { Grupo = K.i_GroupId, TipoProd = K.i_ItemId, eliminado = K.i_IsDeleted.Value } into K_join
                                         from K in K_join.DefaultIfEmpty()
                                         join L in dbContext.datahierarchy on new { MotivosIngreso = D.i_IdTipoMovimiento == 1 ? 19 : 20, mot = D.i_IdTipoMotivo.Value, eliminado = 0 } equals new { MotivosIngreso = L.i_GroupId, mot = L.i_ItemId, eliminado = L.i_IsDeleted.Value } into L_join
                                         from L in L_join.DefaultIfEmpty()
                                         join J4 in dbContext.marca on new { marc = C.v_IdMarca, eliminado = 0 } equals new { marc = J4.v_IdMarca, eliminado = J4.i_Eliminado.Value } into J4_join
                                         from J4 in J4_join.DefaultIfEmpty()

                                         where A.i_Eliminado == 0 && D.i_IdTipoMovimiento != (int)TipoDeMovimiento.Transferencia
                                         && (D.t_Fecha.Value >= pdtFechaInicio && D.t_Fecha.Value <= pdtFechaFin)

                                           && D.i_IdEstablecimiento == idEstablecimiento
                                         && (C.v_CodInterno == pstrCodigoProducto || pstrCodigoProducto == "") && (A.v_NroPedido == pstrNumeroPedido || pstrNumeroPedido == "")
                                         && C.i_EsServicio == 0
                                         && (C.v_IdMarca == IdMarca || IdMarca == "-1")
                                         && C.i_EsActivoFijo == 0
                                         && (C.v_IdLinea == IdLinea || IdLinea == "-1")
                                         && (D.v_OrigenTipo == null || D.v_OrigenTipo == "" || D.v_OrigenTipo == Constants.OrigenCompra || D.v_OrigenTipo == Constants.OrigenImportacion || D.v_OrigenTipo == Constants.OrigenTransferencia || D.v_OrigenTipo == Constants.OrigenVenta || D.v_OrigenTipo == Constants.OrigenGuiaInterna || (!string.IsNullOrEmpty(D.v_OrigenTipo) && D.v_OrigenTipo != Constants.OrigenGuiaCompra))
                                         && D.i_IdEstablecimiento == idEstablecimiento
                                           && (C.v_Modelo == Modelo || Modelo == "")
                                         select new KardexList
                                         {
                                             v_IdMovimientoDetalle = A.v_IdMovimientoDetalle,
                                             v_IdProducto = C.v_IdProducto,
                                             v_NombreProducto = "PRODUCTO :" + C.v_CodInterno.Trim() + " " + C.v_Descripcion.Trim(),

                                             t_Fecha = D.t_Fecha,
                                             i_IdTipoMovimiento = D.i_IdTipoMovimiento,
                                             d_Cantidad = A.d_CantidadEmpaque == null ? A.d_Cantidad == null ? 0 : A.d_Cantidad : A.d_CantidadEmpaque,

                                             d_Precio = D.v_OrigenTipo == "I" ? pintIdMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : A.d_Total,
                                             i_EsDevolucion = D.i_EsDevolucion,
                                             i_IdTipoMotivo = D.i_IdTipoMotivo,
                                             NroPedido = A.v_NroPedido == null ? "" : A.v_NroPedido,
                                             v_IdLinea = C.v_IdLinea,
                                             IdMoneda = D.i_IdMoneda.Value,
                                             TipoCambio = D.d_TipoCambio.Value,
                                             Almacen = G.v_Nombre,
                                             ClienteProveedor = D.v_OrigenTipo == "T" ? D.v_Glosa == null || D.v_Glosa == string.Empty ? "TRANSFERENCIA " : D.v_Glosa : E == null ? "**CLIENTE/PROVEEDOR  NO EXISTE**" : (E.v_ApePaterno + " " + E.v_ApeMaterno + " " + E.v_PrimerNombre + " " + E.v_RazonSocial).Trim(),
                                             Guia = A.v_NroGuiaRemision,
                                             Documento = H == null ? A.v_NumeroDocumento : H.v_Siglas + " " + A.v_NumeroDocumento,
                                             DocumentoInventarioSunat = string.IsNullOrEmpty(A.v_NumeroDocumento) ? "" : A.v_NumeroDocumento,
                                             IdAlmacen = D.i_IdAlmacenOrigen.Value,

                                             TipoMotivo = D.i_IdTipoMotivo == 5 ? 1 : D.v_OrigenTipo == "T" ? 3 : 2,
                                             IdAlmacenOrigen = D.i_IdAlmacenOrigen.Value,
                                             NroRegistro = D.v_Mes + "-" + D.v_Correlativo,
                                             i_IdTipoDocumentoDetalle = A.i_IdTipoDocumento == null ? -1 : A.i_IdTipoDocumento.Value,
                                             EsDocumentoInterno = H.i_UsadoDocumentoInterno == null ? 0 : H.i_UsadoDocumentoInterno.Value,
                                             EsDocInverso = H.i_UsadoDocumentoInverso == null ? 0 : H.i_UsadoDocumentoInverso.Value,
                                             ValorUM = I.v_Value2,
                                             v_IdMarca = C.v_IdMarca,
                                             Origen = D.v_OrigenTipo,
                                             d_Total = D.v_OrigenTipo == "I" ? pintIdMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : A.d_Total,
                                             CorrelativoPle = D.v_Mes.Trim() + D.v_Correlativo.Trim().Substring(0, 2) + D.v_Correlativo.Trim().Substring(3, 8),
                                             TipoExistencia = K == null ? "" : K.v_Value2,
                                             SoloNumeroDocumentoDetalle = string.IsNullOrEmpty(A.v_NumeroDocumento.Trim()) ? "" : A.v_NumeroDocumento.Trim(),
                                             TipoOperacionInventarioSunat = L == null ? "" : L.v_Value2,
                                             CodProducto = C.v_CodInterno.Trim(),
                                             DescripcionProducto = C.v_Descripcion,
                                             TipoeExistenciaCompleto = K == null ? "TIPO : " + "NO EXISTE TIPO PRODUCTO " : "TIPO : " + K.v_Value4 + " - " + K.v_Value1,
                                             UnidadMedidaCompleto = I == null ? "UNIDAD MEDIDA : NO EXISTE UNIDAD ASIGNADA" : I.v_Value4 == null ? "UNIDAD MEDIDA : 07 - UNIDAD" : "UNIDAD MEDIDA : " + I.v_Value4 + " - " + I.v_Value1,
                                             UnidadMedida = I == null || I.v_Value4.Trim() == null || I.v_Value4.Trim() == "" ? "07" : I.v_Value4,
                                             i_IdTipoProducto = C == null ? -1 : C.i_IdTipoProducto ?? 0,
                                             UnidadMedidaProducto = I.v_Value1,
                                             Modelo = C == null ? "" : string.IsNullOrEmpty(C.v_Modelo) ? "" : C.v_Modelo,
                                             Marca = J4 == null || string.IsNullOrEmpty(J4.v_Nombre) ? "" : J4.v_Nombre,
                                             Ubicacion = C == null || string.IsNullOrEmpty(C.v_Ubicacion) ? "" : C.v_Ubicacion,
                                             NroParte = C == null || string.IsNullOrEmpty(C.v_NroParte) ? "" : C.v_NroParte,

                                         }).ToList().AsQueryable();


                            if (!string.IsNullOrEmpty(pstrFilterExpression))
                            {
                                query = query.Where(pstrFilterExpression);
                            }

                            queryFinal = query.ToList().OrderBy(x => x.IdAlmacenOrigen).ThenBy(x => x.v_IdProducto).ThenBy(x => x.t_Fecha).ThenBy(x => x.i_IdTipoMovimiento).ThenBy(x => x.TipoMotivo).ThenBy(x => x.NroPedido).ThenBy(o => o.v_IdMovimientoDetalle).ToList();


                        }
                        else if (IncluirPedido == 0 && (ArticulosStockMayor0 == 1 || ArticuloStockCero == 1 || ArticuloStockNegativo == 1))
                        {
                            var query = (from A in dbContext.movimientodetalle
                                         join B in dbContext.productodetalle on new { IdProductoDetalle = A.v_IdProductoDetalle, eliminado = 0 } equals new { IdProductoDetalle = B.v_IdProductoDetalle, eliminado = B.i_Eliminado.Value } into B_join
                                         from B in B_join.DefaultIfEmpty()
                                         join C in dbContext.producto on new { IdProducto = B.v_IdProducto, eliminado = 0 } equals new { IdProducto = C.v_IdProducto, eliminado = C.i_Eliminado.Value } into C_join
                                         from C in C_join.DefaultIfEmpty()
                                         join D in dbContext.movimiento on new { IdMovimiento = A.v_IdMovimiento, eliminado = 0 } equals new { IdMovimiento = D.v_IdMovimiento, eliminado = D.i_Eliminado.Value } into D_join
                                         from D in D_join.DefaultIfEmpty()
                                         join E in dbContext.cliente on new { IdCliente = D.v_IdCliente, eliminado = 0 } equals new { IdCliente = E.v_IdCliente, eliminado = E.i_Eliminado.Value } into E_join
                                         from E in E_join.DefaultIfEmpty()
                                         join G in dbContext.almacen on new { IdAlmacen = D.i_IdAlmacenOrigen.Value, eliminado = 0 } equals new { IdAlmacen = G.i_IdAlmacen, eliminado = G.i_Eliminado.Value } into G_join
                                         from G in G_join.DefaultIfEmpty()

                                         join H in dbContext.documento on new { IdTipoDoc = A.i_IdTipoDocumento.Value, eliminado = 0 } equals new { IdTipoDoc = H.i_CodigoDocumento, eliminado = H.i_Eliminado.Value } into H_join
                                         from H in H_join.DefaultIfEmpty()
                                         join K in dbContext.datahierarchy on new { Grupo = 17, eliminado = 0, Um = C.i_IdUnidadMedida.Value } equals new { Grupo = K.i_GroupId, eliminado = K.i_IsDeleted.Value, Um = K.i_ItemId } into K_join

                                         from K in K_join.DefaultIfEmpty()
                                         join I in dbContext.datahierarchy on new { Grupo = 6, TipoProd = C.i_IdTipoProducto ?? -1, eliminado = 0 } equals new { Grupo = I.i_GroupId, TipoProd = I.i_ItemId, eliminado = I.i_IsDeleted.Value } into I_join
                                         from I in I_join.DefaultIfEmpty()
                                         join J4 in dbContext.marca on new { marc = C.v_IdMarca, eliminado = 0 } equals new { marc = J4.v_IdMarca, eliminado = J4.i_Eliminado.Value } into J4_join
                                         from J4 in J4_join.DefaultIfEmpty()

                                         where A.i_Eliminado == 0 && D.i_IdTipoMovimiento != (int)TipoDeMovimiento.Transferencia
                                         && (D.t_Fecha.Value >= pdtFechaInicio && D.t_Fecha.Value <= pdtFechaFin)

                                         && D.i_IdEstablecimiento == idEstablecimiento
                                         && (C.v_CodInterno == pstrCodigoProducto || pstrCodigoProducto == "") && (A.v_NroPedido == pstrNumeroPedido || pstrNumeroPedido == "")

                                         && C.i_EsServicio == 0
                                          && C.i_EsActivoFijo == 0
                                           && (C.v_IdLinea == IdLinea || IdLinea == "-1")
                                          && (C.v_IdMarca == IdMarca || IdMarca == "-1")
                                        && (D.v_OrigenTipo == null || D.v_OrigenTipo == "" || D.v_OrigenTipo == Constants.OrigenCompra || D.v_OrigenTipo == Constants.OrigenImportacion || D.v_OrigenTipo == Constants.OrigenTransferencia || D.v_OrigenTipo == Constants.OrigenVenta || D.v_OrigenTipo == Constants.OrigenGuiaInterna || (!string.IsNullOrEmpty(D.v_OrigenTipo) && D.v_OrigenTipo != Constants.OrigenGuiaCompra))
                                           && (C.v_Modelo == Modelo || Modelo == "")
                                         select new KardexList
                                         {
                                             v_IdMovimientoDetalle = A.v_IdMovimientoDetalle,
                                             v_IdProducto = C.v_IdProducto,
                                             v_NombreProducto = "PRODUCTO :" + C.v_CodInterno.Trim() + " " + C.v_Descripcion.Trim(),

                                             v_NombreProductoInventarioSunat = FormatoCant == (int)FormatoCantidad.UnidadMedidaProducto ? "CODIGO EXISTENCIA : " + C.v_CodInterno.Trim() + " \n" + "TIPO : " + I.v_Value2 + " " + I.v_Value1 + "\n" + "DESCRIPCIÓN : " + C.v_Descripcion + "\n" + "UNIDAD MEDIDA : " + K.v_Value4 + " " + K.v_Value1 : "CODIGO EXISTENCIA : " + C.v_CodInterno.Trim() + " \n" + "TIPO : " + I.v_Value2 + " " + I.v_Value1 + "\n" + "DESCRIPCIÓN : " + C.v_Descripcion + "\n" + "UNIDAD MEDIDA : 07 UNIDAD",
                                             t_Fecha = D.t_Fecha,
                                             i_IdTipoMovimiento = D.i_IdTipoMovimiento,
                                             d_Cantidad = A.d_CantidadEmpaque == null ? A.d_Cantidad == null ? 0 : A.d_Cantidad : A.d_CantidadEmpaque,
                                             d_Precio = D.v_OrigenTipo == "I" ? pintIdMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : A.d_Total,
                                             i_EsDevolucion = D.i_EsDevolucion,
                                             i_IdTipoMotivo = D.i_IdTipoMotivo,
                                             NroPedido = A.v_NroPedido == null ? "" : A.v_NroPedido,
                                             v_IdLinea = C.v_IdLinea,
                                             IdMoneda = D.i_IdMoneda.Value,
                                             TipoCambio = D.d_TipoCambio.Value,
                                             Almacen = G.v_Nombre,
                                             ClienteProveedor = D.v_OrigenTipo == "T" ? D.v_Glosa == null || D.v_Glosa == string.Empty ? "TRANSFERENCIA " : D.v_Glosa : E == null ? "**CLIENTE/PROVEEDOR  NO EXISTE**" : (E.v_ApePaterno + " " + E.v_ApeMaterno + " " + E.v_PrimerNombre + " " + E.v_RazonSocial).Trim(),
                                             Guia = A.v_NroGuiaRemision,
                                             Documento = H == null ? A.v_NumeroDocumento : H.v_Siglas + " " + A.v_NumeroDocumento,
                                             DocumentoInventarioSunat = string.IsNullOrEmpty(A.v_NumeroDocumento) ? "" : A.v_NumeroDocumento,
                                             IdAlmacen = D.i_IdAlmacenOrigen.Value,
                                             TipoMotivo = D.i_IdTipoMotivo == 5 ? 1 : D.v_OrigenTipo == "T" ? 3 : 2,
                                             IdAlmacenOrigen = D.i_IdAlmacenOrigen.Value,
                                             NroRegistro = D.v_Mes + "-" + D.v_Correlativo,
                                             i_IdTipoDocumentoDetalle = A.i_IdTipoDocumento == null ? -1 : A.i_IdTipoDocumento.Value,
                                             EsDocumentoInterno = H.i_UsadoDocumentoInterno == null ? 0 : H.i_UsadoDocumentoInterno.Value,
                                             EsDocInverso = H.i_UsadoDocumentoInverso == null ? 0 : H.i_UsadoDocumentoInverso.Value,
                                             ValorUM = K.v_Value2,
                                             v_IdMarca = C.v_IdMarca,
                                             Origen = D.v_OrigenTipo,
                                             d_Total = D.v_OrigenTipo == "I" ? pintIdMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : A.d_Total,
                                             i_IdTipoProducto = C == null ? -1 : C.i_IdTipoProducto ?? 0,
                                             CodProducto = C == null ? "" : C.v_CodInterno.Trim(),
                                             TipoExistencia = I == null ? "" : I.v_Value2,
                                             UnidadMedidaProducto = K.v_Value1,

                                             Modelo = C == null ? "" : string.IsNullOrEmpty(C.v_Modelo) ? "" : C.v_Modelo,
                                             Marca = J4 == null || string.IsNullOrEmpty(J4.v_Nombre) ? "" : J4.v_Nombre,
                                             Ubicacion = C == null || string.IsNullOrEmpty(C.v_Ubicacion) ? "" : C.v_Ubicacion,
                                             NroParte = C == null || string.IsNullOrEmpty(C.v_NroParte) ? "" : C.v_NroParte,

                                         }).ToList().AsQueryable();


                            if (!string.IsNullOrEmpty(pstrFilterExpression))
                            {
                                query = query.Where(pstrFilterExpression);
                            }


                            queryFinal = query.ToList().OrderBy(x => x.IdAlmacenOrigen).ThenBy(x => x.v_IdProducto).ThenBy(x => x.t_Fecha).ThenBy(x => x.i_IdTipoMovimiento).ThenBy(x => x.TipoMotivo).ThenBy(x => x.NroPedido).ThenBy(o => o.v_IdMovimientoDetalle).ToList();
                        }
                        

                        #endregion
                        
                    }
                    else
                    {
                        #region ConsideraGuiaCompra
                        
                        if (IncluirPedido == 1 && (ArticulosStockMayor0 == 1 || ArticuloStockCero == 1 || ArticuloStockNegativo == 1))
                        {

                            var query = (from A in dbContext.movimientodetalle
                                         join B in dbContext.productodetalle on new { IdProductoDetalle = A.v_IdProductoDetalle, eliminado = 0 } equals new { IdProductoDetalle = B.v_IdProductoDetalle, eliminado = B.i_Eliminado.Value } into B_join
                                         from B in B_join.DefaultIfEmpty()
                                         join C in dbContext.producto on new { IdProducto = B.v_IdProducto, eliminado = 0 } equals new { IdProducto = C.v_IdProducto, eliminado = C.i_Eliminado.Value } into C_join
                                         from C in C_join.DefaultIfEmpty()
                                         join D in dbContext.movimiento on new { A.v_IdMovimiento, eliminado = 0 } equals new { D.v_IdMovimiento, eliminado = D.i_Eliminado.Value } into D_join
                                         from D in D_join.DefaultIfEmpty()
                                         join E in dbContext.cliente on new { IdCliente = D.v_IdCliente, eliminado = 0 } equals new { IdCliente = E.v_IdCliente, eliminado = E.i_Eliminado.Value } into E_join
                                         from E in E_join.DefaultIfEmpty()
                                         join G in dbContext.almacen on new { IdAlmacen = D.i_IdAlmacenOrigen.Value, eliminado = 0 } equals new { IdAlmacen = G.i_IdAlmacen, eliminado = G.i_Eliminado.Value } into G_join
                                         from G in G_join.DefaultIfEmpty()

                                         join H in dbContext.documento on new { IdTipoDoc = A.i_IdTipoDocumento.Value, eliminado = 0 } equals new { IdTipoDoc = H.i_CodigoDocumento, eliminado = H.i_Eliminado.Value } into H_join
                                         from H in H_join.DefaultIfEmpty()

                                         join K in dbContext.datahierarchy on new { Grupo = 17, UnidadMedida = C.i_IdUnidadMedida.Value, eliminado = 0 } equals new { Grupo = K.i_GroupId, UnidadMedida = K.i_ItemId, eliminado = K.i_IsDeleted.Value } into K_join
                                         from K in K_join.DefaultIfEmpty()

                                         join I in dbContext.datahierarchy on new { Grupo = 6, TipoProd = C.i_IdTipoProducto ?? -1, eliminado = 0 } equals new { Grupo = I.i_GroupId, TipoProd = I.i_ItemId, eliminado = I.i_IsDeleted.Value } into I_join
                                         from I in I_join.DefaultIfEmpty()
                                         join J4 in dbContext.marca on new { marc = C.v_IdMarca, eliminado = 0 } equals new { marc = J4.v_IdMarca, eliminado = J4.i_Eliminado.Value } into J4_join
                                         from J4 in J4_join.DefaultIfEmpty()


                                         where A.i_Eliminado == 0 && D.i_IdTipoMovimiento != (int)TipoDeMovimiento.Transferencia
                                         && (D.t_Fecha.Value >= pdtFechaInicio && D.t_Fecha.Value <= pdtFechaFin)
                                         && D.i_IdEstablecimiento == idEstablecimiento
                                         && (C.v_CodInterno.Trim() == pstrCodigoProducto.Trim() || pstrCodigoProducto == "")
                                         && (A.v_NroPedido == pstrNumeroPedido || pstrNumeroPedido == "")

                                         && C.i_EsServicio == 0
                                          && (C.v_IdLinea == IdLinea || IdLinea == "-1")
                                          && C.i_EsActivoFijo == 0
                                         && (C.v_IdMarca == IdMarca || IdMarca == "-1")
                                           && (C.v_Modelo == Modelo || Modelo == "")
                                         select new KardexList
                                         {
                                             v_IdMovimientoDetalle = A.v_IdMovimientoDetalle,
                                             v_IdProducto = C.v_IdProducto,
                                             v_NombreProducto = A.v_NroPedido == null ? "PRODUCTO :" + C.v_CodInterno.Trim() + " " + C.v_Descripcion.Trim() + "  PEDIDO :" : "PRODUCTO :" + C.v_CodInterno.Trim() + " " + C.v_Descripcion.Trim() + "  PEDIDO :" + A.v_NroPedido.Trim(),

                                             v_NombreProductoInventarioSunat = FormatoCant == (int)FormatoCantidad.UnidadMedidaProducto ? "CODIGO EXISTENCIA : " + C.v_CodInterno.Trim() + " \n" + "TIPO : " + I.v_Value2 + " " + I.v_Value1 + "\n" + "DESCRIPCIÓN : " + C.v_Descripcion + "\n" + "UNIDAD MEDIDA : " + K.v_Value4 + " " + K.v_Value1 : "CODIGO EXISTENCIA : " + C.v_CodInterno.Trim() + " \n" + "TIPO : " + I.v_Value2 + " " + I.v_Value1 + "\n" + "DESCRIPCIÓN : " + C.v_Descripcion + "\n" + "UNIDAD MEDIDA : 07 UNIDAD",
                                             t_Fecha = D.t_Fecha,
                                             i_IdTipoMovimiento = D.i_IdTipoMovimiento,
                                             d_Cantidad = A.d_CantidadEmpaque == null ? A.d_Cantidad == null ? 0 : A.d_Cantidad : A.d_CantidadEmpaque,
                                             d_Precio = D.v_OrigenTipo == "I" ? pintIdMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : A.d_Total,
                                             i_EsDevolucion = D.i_EsDevolucion,
                                             i_IdTipoMotivo = D.i_IdTipoMotivo,
                                             NroPedido = A.v_NroPedido == null ? "" : A.v_NroPedido,
                                             v_IdLinea = C.v_IdLinea,
                                             IdMoneda = D.i_IdMoneda.Value,
                                             TipoCambio = D.d_TipoCambio.Value,
                                             Almacen = G.v_Nombre,
                                             ClienteProveedor = D.v_OrigenTipo == "T" ? D.v_Glosa == null || D.v_Glosa == string.Empty ? "TRANSFERENCIA " : D.v_Glosa : E == null ? "**CLIENTE/PROVEEDOR  NO EXISTE**" : (E.v_ApePaterno + " " + E.v_ApeMaterno + " " + E.v_PrimerNombre + " " + E.v_RazonSocial).Trim(),
                                             Guia = A.v_NroGuiaRemision,
                                             Documento = H == null ? A.v_NumeroDocumento : H.v_Siglas.Trim() + " " + A.v_NumeroDocumento.Trim(),
                                             DocumentoInventarioSunat = string.IsNullOrEmpty(A.v_NumeroDocumento) ? "" : A.v_NumeroDocumento,
                                             IdAlmacen = D.i_IdAlmacenOrigen.Value,
                                             //TipoMotivo = D.i_IdTipoMotivo == 5 ? 1 : 2,
                                             TipoMotivo = D.i_IdTipoMotivo == 5 ? 1 : D.v_OrigenTipo == "T" ? 3 : 2,
                                             IdAlmacenOrigen = D.i_IdAlmacenOrigen.Value,
                                             NroRegistro = D.v_Mes + "-" + D.v_Correlativo,
                                             i_IdTipoDocumentoDetalle = A.i_IdTipoDocumento == null ? -1 : A.i_IdTipoDocumento.Value,
                                             EsDocumentoInterno = H.i_UsadoDocumentoInterno == null ? 0 : H.i_UsadoDocumentoInterno.Value,
                                             EsDocInverso = H.i_UsadoDocumentoInverso == null ? 0 : H.i_UsadoDocumentoInverso.Value,
                                             ValorUM = K.v_Value2,
                                             v_IdMarca = C.v_IdMarca,
                                             Origen = D.v_OrigenTipo,
                                             d_Total = D.v_OrigenTipo == "I" ? pintIdMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : A.d_Total,
                                             i_IdTipoProducto = C == null ? -1 : C.i_IdTipoProducto ?? 0,
                                             CodProducto = C == null ? "" : C.v_CodInterno.Trim(),
                                             TipoExistencia = I == null ? "" : I.v_Value2,
                                             UnidadMedidaProducto = K.v_Value1,
                                             Modelo = C == null ? "" : string.IsNullOrEmpty(C.v_Modelo) ? "" : C.v_Modelo,
                                             Marca = J4 == null || string.IsNullOrEmpty(J4.v_Nombre) ? "" : J4.v_Nombre,
                                             Ubicacion = C == null || string.IsNullOrEmpty(C.v_Ubicacion) ? "" : C.v_Ubicacion,
                                             NroParte = C == null || string.IsNullOrEmpty(C.v_NroParte) ? "" : C.v_NroParte,
                                         }).ToList().AsQueryable();



                            if (!string.IsNullOrEmpty(pstrFilterExpression))
                            {
                                query = query.Where(pstrFilterExpression);
                            }

                            queryFinal = query.ToList().OrderBy(x => x.IdAlmacenOrigen).ThenBy(x => x.v_NombreProducto).ThenBy(x => x.t_Fecha).ThenBy(x => x.i_IdTipoMovimiento).ThenBy(x => x.TipoMotivo).ThenBy(x => x.NroPedido).ThenBy(o => o.v_IdMovimientoDetalle).ToList();


                        }
                        else if (IncluirPedido == 1)
                        {

                            var query = (from A in dbContext.movimientodetalle
                                         join B in dbContext.productodetalle on new { IdProductoDetalle = A.v_IdProductoDetalle, eliminado = 0 } equals new { IdProductoDetalle = B.v_IdProductoDetalle, eliminado = B.i_Eliminado.Value } into B_join
                                         from B in B_join.DefaultIfEmpty()
                                         join C in dbContext.producto on new { IdProducto = B.v_IdProducto, eliminado = 0 } equals new { IdProducto = C.v_IdProducto, eliminado = C.i_Eliminado.Value } into C_join
                                         from C in C_join.DefaultIfEmpty()
                                         join D in dbContext.movimiento on new { IdMovimiento = A.v_IdMovimiento, eliminado = 0 } equals new { IdMovimiento = D.v_IdMovimiento, eliminado = D.i_Eliminado.Value } into D_join
                                         from D in D_join.DefaultIfEmpty()
                                         join E in dbContext.cliente on new { IdCliente = D.v_IdCliente, eliminado = 0 } equals new { IdCliente = E.v_IdCliente, eliminado = E.i_Eliminado.Value } into E_join
                                         from E in E_join.DefaultIfEmpty()
                                         join G in dbContext.almacen on new { IdAlmacen = D.i_IdEstablecimiento.Value, eliminado = 0 } equals new { IdAlmacen = G.i_IdAlmacen, eliminado = G.i_Eliminado.Value } into G_join
                                         from G in G_join.DefaultIfEmpty()

                                         join H in dbContext.documento on new { IdTipoDoc = A.i_IdTipoDocumento.Value, eliminado = 0 } equals new { IdTipoDoc = H.i_CodigoDocumento, eliminado = H.i_Eliminado.Value } into H_join
                                         from H in H_join.DefaultIfEmpty()

                                         join I in dbContext.datahierarchy on new { Grupo = 17, eliminado = 0, UnidadMedida = C.i_IdUnidadMedida.Value } equals new { Grupo = I.i_GroupId, eliminado = I.i_IsDeleted.Value, UnidadMedida = I.i_ItemId } into I_join
                                         from I in I_join.DefaultIfEmpty()

                                         join K in dbContext.datahierarchy on new { Grupo = 6, TipoProd = C.i_IdTipoProducto ?? -1, eliminado = 0 } equals new { Grupo = K.i_GroupId, TipoProd = K.i_ItemId, eliminado = K.i_IsDeleted.Value } into K_join
                                         from K in K_join.DefaultIfEmpty()
                                         join J4 in dbContext.marca on new { marc = C.v_IdMarca, eliminado = 0 } equals new { marc = J4.v_IdMarca, eliminado = J4.i_Eliminado.Value } into J4_join
                                         from J4 in J4_join.DefaultIfEmpty()

                                         where A.i_Eliminado == 0 && D.i_IdTipoMovimiento != (int)TipoDeMovimiento.Transferencia
                                         && (D.t_Fecha.Value >= pdtFechaInicio && D.t_Fecha.Value <= pdtFechaFin.Value)
                                         && D.i_IdEstablecimiento == idEstablecimiento
                                         && (C.v_CodInterno == pstrCodigoProducto || pstrCodigoProducto == "") && (A.v_NroPedido == pstrNumeroPedido || pstrNumeroPedido == "")
                                         && C.i_EsServicio == 0
                                          && C.i_EsActivoFijo == 0
                                           && (C.v_IdLinea == IdLinea || IdLinea == "-1")
                                          && (C.v_IdMarca == IdMarca || IdMarca == "-1")
                                            && (C.v_Modelo == Modelo || Modelo == "")
                                         select new KardexList
                                         {
                                             v_IdMovimientoDetalle = A.v_IdMovimientoDetalle,
                                             v_IdProducto = C == null ? "" : C.v_IdProducto,
                                             v_NombreProducto = A.v_NroPedido == null ? "PRODUCTO :" + C.v_CodInterno.Trim() + " " + C.v_Descripcion.Trim() + "  PEDIDO :" : "PRODUCTO :" + C.v_CodInterno.Trim() + " " + C.v_Descripcion.Trim() + "  PEDIDO :" + A.v_NroPedido.Trim(),
                                             v_NombreProductoInventarioSunat = FormatoCant == (int)FormatoCantidad.UnidadMedidaProducto ? "CODIGO EXISTENCIA : " + C.v_CodInterno.Trim() + " \n" + "TIPO : " + K.v_Value2 + " " + K.v_Value1 + "\n" + "DESCRIPCIÓN : " + C.v_Descripcion + "\n" + "UNIDAD MEDIDA : " + I.v_Value4 + " " + I.v_Value1 : "CODIGO EXISTENCIA : " + C.v_CodInterno.Trim() + " \n" + "TIPO : " + K.v_Value2 + " " + K.v_Value1 + "\n" + "DESCRIPCIÓN : " + C.v_Descripcion + "\n" + "UNIDAD MEDIDA : 07 UNIDAD",
                                             t_Fecha = D.t_Fecha,
                                             i_IdTipoMovimiento = D.i_IdTipoMovimiento,
                                             d_Cantidad = A.d_CantidadEmpaque == null ? A.d_Cantidad == null ? 0 : A.d_Cantidad : A.d_CantidadEmpaque,
                                             d_Precio = D.v_OrigenTipo == "I" ? pintIdMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : A.d_Total,
                                             i_EsDevolucion = D.i_EsDevolucion,
                                             i_IdTipoMotivo = D.i_IdTipoMotivo,
                                             NroPedido = A.v_NroPedido == null ? "" : A.v_NroPedido,
                                             v_IdLinea = C.v_IdLinea,
                                             IdMoneda = D.i_IdMoneda.Value,
                                             TipoCambio = D.d_TipoCambio.Value,
                                             Almacen = G.v_Nombre,
                                             ClienteProveedor = D.v_OrigenTipo == "T" ? D.v_Glosa == null || D.v_Glosa == string.Empty ? "TRANSFERENCIA " : D.v_Glosa : E == null ? "**CLIENTE/PROVEEDOR  NO EXISTE**" : (E.v_ApePaterno + " " + E.v_ApeMaterno + " " + E.v_PrimerNombre + " " + E.v_RazonSocial).Trim(),
                                             Guia = A.v_NroGuiaRemision,
                                             Documento = H == null ? A.v_NumeroDocumento : H.v_Siglas.Trim() + " " + A.v_NumeroDocumento.Trim(),
                                             DocumentoInventarioSunat = string.IsNullOrEmpty(A.v_NumeroDocumento) ? "" : A.v_NumeroDocumento,
                                             IdAlmacen = D.i_IdAlmacenOrigen.Value,

                                             TipoMotivo = D.i_IdTipoMotivo == 5 ? 1 : D.v_OrigenTipo == "T" ? 3 : 2,
                                             IdAlmacenOrigen = D.i_IdAlmacenOrigen.Value,
                                             NroRegistro = D.v_Mes + "-" + D.v_Correlativo,
                                             i_IdTipoDocumentoDetalle = A.i_IdTipoDocumento == null ? -1 : A.i_IdTipoDocumento.Value,
                                             EsDocumentoInterno = H.i_UsadoDocumentoInterno == null ? 0 : H.i_UsadoDocumentoInterno.Value,
                                             EsDocInverso = H.i_UsadoDocumentoInverso == null ? 0 : H.i_UsadoDocumentoInverso.Value,
                                             ValorUM = I.v_Value2,
                                             v_IdMarca = C.v_IdMarca,
                                             Origen = D.v_OrigenTipo,
                                             d_Total = D.v_OrigenTipo == "I" ? pintIdMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : A.d_Total,
                                             i_IdTipoProducto = C == null ? -1 : C.i_IdTipoProducto ?? 0,
                                             CodProducto = C == null ? "" : C.v_CodInterno.Trim(),
                                             TipoExistencia = K == null ? "" : K.v_Value2,
                                             UnidadMedidaProducto = I.v_Value1,
                                             Modelo = C == null ? "" : string.IsNullOrEmpty(C.v_Modelo) ? "" : C.v_Modelo,
                                             Marca = J4 == null || string.IsNullOrEmpty(J4.v_Nombre) ? "" : J4.v_Nombre,
                                             Ubicacion = C == null || string.IsNullOrEmpty(C.v_Ubicacion) ? "" : C.v_Ubicacion,
                                             NroParte = C == null || string.IsNullOrEmpty(C.v_NroParte) ? "" : C.v_NroParte,
                                         }).ToList().AsQueryable();

                            if (!string.IsNullOrEmpty(pstrFilterExpression))
                            {
                                query = query.Where(pstrFilterExpression);
                            }
                            queryFinal = query.ToList().OrderBy(x => x.IdAlmacenOrigen).ThenBy(x => x.v_NombreProducto).ThenBy(x => x.t_Fecha).ThenBy(x => x.i_IdTipoMovimiento).ThenBy(x => x.TipoMotivo).ThenBy(x => x.NroPedido).ThenBy(o => o.v_IdMovimientoDetalle).ToList();

                        }
                        else if (IncluirPedido == 0)
                        {

                            var query = (from A in dbContext.movimientodetalle
                                         join B in dbContext.productodetalle on new { IdProductoDetalle = A.v_IdProductoDetalle, eliminado = 0 } equals new { IdProductoDetalle = B.v_IdProductoDetalle, eliminado = B.i_Eliminado.Value } into B_join
                                         from B in B_join.DefaultIfEmpty()
                                         join C in dbContext.producto on new { IdProducto = B.v_IdProducto, eliminado = 0 } equals new { IdProducto = C.v_IdProducto, eliminado = C.i_Eliminado.Value } into C_join
                                         from C in C_join.DefaultIfEmpty()
                                         join D in dbContext.movimiento on new { IdMovimiento = A.v_IdMovimiento, eliminado = 0 } equals new { IdMovimiento = D.v_IdMovimiento, eliminado = D.i_Eliminado.Value } into D_join
                                         from D in D_join.DefaultIfEmpty()
                                         join E in dbContext.cliente on new { IdCliente = D.v_IdCliente, eliminado = 0 } equals new { IdCliente = E.v_IdCliente, eliminado = E.i_Eliminado.Value } into E_join
                                         from E in E_join.DefaultIfEmpty()

                                         join G in dbContext.almacen on new { pIntAlmacen = D.i_IdAlmacenOrigen.Value, eliminado = 0 } equals new { pIntAlmacen = G.i_IdAlmacen, eliminado = G.i_Eliminado.Value } into G_join
                                         from G in G_join.DefaultIfEmpty()
                                         join H in dbContext.documento on new { IdTipoDoc = A.i_IdTipoDocumento.Value, eliminado = 0 } equals new { IdTipoDoc = H.i_CodigoDocumento, eliminado = H.i_Eliminado.Value } into H_join
                                         from H in H_join.DefaultIfEmpty()
                                         join I in dbContext.datahierarchy on new { Grupo = 17, eliminado = 0, Um = C.i_IdUnidadMedida.Value } equals new { Grupo = I.i_GroupId, eliminado = I.i_IsDeleted.Value, Um = I.i_ItemId } into I_join
                                         from I in I_join.DefaultIfEmpty()
                                         join K in dbContext.datahierarchy on new { Grupo = 6, TipoProd = C.i_IdTipoProducto ?? -1, eliminado = 0 } equals new { Grupo = K.i_GroupId, TipoProd = K.i_ItemId, eliminado = K.i_IsDeleted.Value } into K_join
                                         from K in K_join.DefaultIfEmpty()
                                         join J4 in dbContext.marca on new { marc = C.v_IdMarca, eliminado = 0 } equals new { marc = J4.v_IdMarca, eliminado = J4.i_Eliminado.Value } into J4_join
                                         from J4 in J4_join.DefaultIfEmpty()

                                         where A.i_Eliminado == 0 && D.i_IdTipoMovimiento != (int)TipoDeMovimiento.Transferencia
                                         && (D.t_Fecha.Value >= pdtFechaInicio && D.t_Fecha.Value <= pdtFechaFin)
                                         && D.i_IdEstablecimiento == idEstablecimiento
                                         && (C.v_CodInterno == pstrCodigoProducto || pstrCodigoProducto == "") && (A.v_NroPedido == pstrNumeroPedido || pstrNumeroPedido == "")
                                         && C.i_EsServicio == 0
                                          && C.i_EsActivoFijo == 0
                                           && (C.v_IdLinea == IdLinea || IdLinea == "-1")
                                          && (C.v_IdMarca == IdMarca || IdMarca == "-1")
                                            && (C.v_Modelo == Modelo || Modelo == "")

                                         select new KardexList
                                         {
                                             v_IdMovimientoDetalle = A.v_IdMovimientoDetalle,
                                             v_IdProducto = C.v_IdProducto,
                                             v_NombreProducto = "PRODUCTO :" + C.v_CodInterno.Trim() + " " + C.v_Descripcion.Trim(),

                                             v_NombreProductoInventarioSunat = FormatoCant == (int)FormatoCantidad.UnidadMedidaProducto ? "CODIGO EXISTENCIA : " + C.v_CodInterno.Trim() + " \n" + "TIPO : " + K.v_Value2 + " " + K.v_Value1 + "\n" + "DESCRIPCIÓN : " + C.v_Descripcion + "\n" + "UNIDAD MEDIDA : " + I.v_Value4 + " " + I.v_Value1 : "CODIGO EXISTENCIA : " + C.v_CodInterno.Trim() + " \n" + "TIPO : " + K.v_Value2 + " " + K.v_Value1 + "\n" + "DESCRIPCIÓN : " + C.v_Descripcion + "\n" + "UNIDAD MEDIDA : 07 UNIDAD",
                                             t_Fecha = D.t_Fecha,
                                             i_IdTipoMovimiento = D.i_IdTipoMovimiento,
                                             d_Cantidad = A.d_CantidadEmpaque == null ? A.d_Cantidad == null ? 0 : A.d_Cantidad : A.d_CantidadEmpaque,

                                             d_Precio = D.v_OrigenTipo == "I" ? pintIdMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : A.d_Total,
                                             i_EsDevolucion = D.i_EsDevolucion,
                                             i_IdTipoMotivo = D.i_IdTipoMotivo,
                                             NroPedido = A.v_NroPedido == null ? "" : A.v_NroPedido,
                                             v_IdLinea = C.v_IdLinea,
                                             IdMoneda = D.i_IdMoneda.Value,
                                             TipoCambio = D.d_TipoCambio.Value,
                                             Almacen = G.v_Nombre,
                                             ClienteProveedor = D.v_OrigenTipo == "T" ? D.v_Glosa == null || D.v_Glosa == string.Empty ? "TRANSFERENCIA " : D.v_Glosa : E == null ? "**CLIENTE/PROVEEDOR  NO EXISTE**" : (E.v_ApePaterno + " " + E.v_ApeMaterno + " " + E.v_PrimerNombre + " " + E.v_RazonSocial).Trim(),
                                             Guia = A.v_NroGuiaRemision,
                                             Documento = H == null ? A.v_NumeroDocumento : H.v_Siglas + " " + A.v_NumeroDocumento,
                                             DocumentoInventarioSunat = string.IsNullOrEmpty(A.v_NumeroDocumento) ? "" : A.v_NumeroDocumento,
                                             IdAlmacen = D.i_IdAlmacenOrigen.Value,
                                             //TipoMotivo = D.i_IdTipoMotivo == 5 ? 1 : 2,
                                             TipoMotivo = D.i_IdTipoMotivo == 5 ? 1 : D.v_OrigenTipo == "T" ? 3 : 2,
                                             IdAlmacenOrigen = D.i_IdAlmacenOrigen.Value,
                                             NroRegistro = D.v_Mes + "-" + D.v_Correlativo,
                                             i_IdTipoDocumentoDetalle = A.i_IdTipoDocumento == null ? -1 : A.i_IdTipoDocumento.Value,
                                             EsDocumentoInterno = H.i_UsadoDocumentoInterno == null ? 0 : H.i_UsadoDocumentoInterno.Value,
                                             EsDocInverso = H.i_UsadoDocumentoInverso == null ? 0 : H.i_UsadoDocumentoInverso.Value,
                                             ValorUM = I.v_Value2,
                                             v_IdMarca = C.v_IdMarca,
                                             Origen = D.v_OrigenTipo,
                                             d_Total = D.v_OrigenTipo == "I" ? pintIdMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : A.d_Total,
                                             i_IdTipoProducto = C == null ? -1 : C.i_IdTipoProducto ?? 0,
                                             CodProducto = C == null ? "" : C.v_CodInterno.Trim(),
                                             TipoExistencia = K == null ? "" : K.v_Value2,
                                             UnidadMedidaProducto = I.v_Value1,
                                             Modelo = C == null ? "" : string.IsNullOrEmpty(C.v_Modelo) ? "" : C.v_Modelo,
                                             Marca = J4 == null || string.IsNullOrEmpty(J4.v_Nombre) ? "" : J4.v_Nombre,
                                             Ubicacion = C == null || string.IsNullOrEmpty(C.v_Ubicacion) ? "" : C.v_Ubicacion,
                                             NroParte = C == null || string.IsNullOrEmpty(C.v_NroParte) ? "" : C.v_NroParte,
                                         }).ToList().AsQueryable();


                            if (!string.IsNullOrEmpty(pstrFilterExpression))
                            {
                                query = query.Where(pstrFilterExpression);
                            }

                            queryFinal = query.ToList().OrderBy(x => x.IdAlmacenOrigen).ThenBy(x => x.v_IdProducto).ThenBy(x => x.i_IdTipoMovimiento).ThenBy(x => x.TipoMotivo).ThenBy(x => x.t_Fecha).ThenBy(x => x.NroPedido).ThenBy(o => o.v_IdMovimientoDetalle).ToList();

                        }
                        else if (IncluirPedido == 0 && (ArticulosStockMayor0 == 1 || ArticuloStockCero == 1 || ArticuloStockNegativo == 1))
                        {
                            var query = (from A in dbContext.movimientodetalle
                                         join B in dbContext.productodetalle on new { IdProductoDetalle = A.v_IdProductoDetalle, eliminado = 0 } equals new { IdProductoDetalle = B.v_IdProductoDetalle, eliminado = B.i_Eliminado.Value } into B_join
                                         from B in B_join.DefaultIfEmpty()
                                         join C in dbContext.producto on new { IdProducto = B.v_IdProducto, eliminado = 0 } equals new { IdProducto = C.v_IdProducto, eliminado = C.i_Eliminado.Value } into C_join
                                         from C in C_join.DefaultIfEmpty()
                                         join D in dbContext.movimiento on new { IdMovimiento = A.v_IdMovimiento, eliminado = 0 } equals new { IdMovimiento = D.v_IdMovimiento, eliminado = D.i_Eliminado.Value } into D_join
                                         from D in D_join.DefaultIfEmpty()
                                         join E in dbContext.cliente on new { IdCliente = D.v_IdCliente, eliminado = 0 } equals new { IdCliente = E.v_IdCliente, eliminado = E.i_Eliminado.Value } into E_join
                                         from E in E_join.DefaultIfEmpty()
                                         join G in dbContext.almacen on new { IdAlmacen = D.i_IdAlmacenOrigen.Value, eliminado = 0 } equals new { IdAlmacen = G.i_IdAlmacen, eliminado = G.i_Eliminado.Value } into G_join
                                         from G in G_join.DefaultIfEmpty()

                                         join H in dbContext.documento on new { IdTipoDoc = A.i_IdTipoDocumento.Value, eliminado = 0 } equals new { IdTipoDoc = H.i_CodigoDocumento, eliminado = H.i_Eliminado.Value } into H_join
                                         from H in H_join.DefaultIfEmpty()

                                         join K in dbContext.datahierarchy on new { Grupo = 17, eliminado = 0, Um = C.i_IdUnidadMedida.Value } equals new { Grupo = K.i_GroupId, eliminado = K.i_IsDeleted.Value, Um = K.i_ItemId } into K_join

                                         from K in K_join.DefaultIfEmpty()
                                         join I in dbContext.datahierarchy on new { Grupo = 6, TipoProd = C.i_IdTipoProducto ?? -1, eliminado = 0 } equals new { Grupo = I.i_GroupId, TipoProd = I.i_ItemId, eliminado = I.i_IsDeleted.Value } into I_join
                                         from I in I_join.DefaultIfEmpty()
                                         join J4 in dbContext.marca on new { marc = C.v_IdMarca, eliminado = 0 } equals new { marc = J4.v_IdMarca, eliminado = J4.i_Eliminado.Value } into J4_join
                                         from J4 in J4_join.DefaultIfEmpty()

                                         where A.i_Eliminado == 0 && D.i_IdTipoMovimiento != (int)TipoDeMovimiento.Transferencia
                                         && (D.t_Fecha.Value >= pdtFechaInicio && D.t_Fecha.Value <= pdtFechaFin)

                                          && D.i_IdEstablecimiento == idEstablecimiento
                                         && (C.v_CodInterno == pstrCodigoProducto || pstrCodigoProducto == "") && (A.v_NroPedido == pstrNumeroPedido || pstrNumeroPedido == "")

                                         && C.i_EsServicio == 0
                                          && C.i_EsActivoFijo == 0
                                           && (C.v_IdLinea == IdLinea || IdLinea == "-1")
                                          && (C.v_IdMarca == IdMarca || IdMarca == "-1")

                                           && (C.v_Modelo == Modelo || Modelo == "")
                                         select new KardexList
                                         {
                                             v_IdMovimientoDetalle = A.v_IdMovimientoDetalle,
                                             v_IdProducto = C.v_IdProducto,
                                             v_NombreProducto = "PRODUCTO :" + C.v_CodInterno.Trim() + " " + C.v_Descripcion.Trim(),

                                             v_NombreProductoInventarioSunat = FormatoCant == (int)FormatoCantidad.UnidadMedidaProducto ? "CODIGO EXISTENCIA : " + C.v_CodInterno.Trim() + " \n" + "TIPO : " + I.v_Value2 + " " + I.v_Value1 + "\n" + "DESCRIPCIÓN : " + C.v_Descripcion + "\n" + "UNIDAD MEDIDA : " + K.v_Value4 + " " + K.v_Value1 : "CODIGO EXISTENCIA : " + C.v_CodInterno.Trim() + " \n" + "TIPO : " + I.v_Value2 + " " + I.v_Value1 + "\n" + "DESCRIPCIÓN : " + C.v_Descripcion + "\n" + "UNIDAD MEDIDA : 07 UNIDAD",
                                             t_Fecha = D.t_Fecha,
                                             i_IdTipoMovimiento = D.i_IdTipoMovimiento,
                                             d_Cantidad = A.d_CantidadEmpaque == null ? A.d_Cantidad == null ? 0 : A.d_Cantidad : A.d_CantidadEmpaque,
                                             d_Precio = D.v_OrigenTipo == "I" ? pintIdMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : A.d_Total,
                                             i_EsDevolucion = D.i_EsDevolucion,
                                             i_IdTipoMotivo = D.i_IdTipoMotivo,
                                             NroPedido = A.v_NroPedido == null ? "" : A.v_NroPedido,
                                             v_IdLinea = C.v_IdLinea,
                                             IdMoneda = D.i_IdMoneda.Value,
                                             TipoCambio = D.d_TipoCambio.Value,
                                             Almacen = G.v_Nombre,
                                             ClienteProveedor = D.v_OrigenTipo == "T" ? D.v_Glosa == null || D.v_Glosa == string.Empty ? "TRANSFERENCIA " : D.v_Glosa : E == null ? "**CLIENTE/PROVEEDOR  NO EXISTE**" : (E.v_ApePaterno + " " + E.v_ApeMaterno + " " + E.v_PrimerNombre + " " + E.v_RazonSocial).Trim(),
                                             Guia = A.v_NroGuiaRemision,
                                             Documento = H == null ? A.v_NumeroDocumento : H.v_Siglas + " " + A.v_NumeroDocumento,
                                             DocumentoInventarioSunat = string.IsNullOrEmpty(A.v_NumeroDocumento) ? "" : A.v_NumeroDocumento,
                                             IdAlmacen = D.i_IdAlmacenOrigen.Value,
                                             TipoMotivo = D.i_IdTipoMotivo == 5 ? 1 : D.v_OrigenTipo == "T" ? 3 : 2,
                                             IdAlmacenOrigen = D.i_IdAlmacenOrigen.Value,
                                             NroRegistro = D.v_Mes + "-" + D.v_Correlativo,
                                             i_IdTipoDocumentoDetalle = A.i_IdTipoDocumento == null ? -1 : A.i_IdTipoDocumento.Value,
                                             EsDocumentoInterno = H.i_UsadoDocumentoInterno == null ? 0 : H.i_UsadoDocumentoInterno.Value,
                                             EsDocInverso = H.i_UsadoDocumentoInverso == null ? 0 : H.i_UsadoDocumentoInverso.Value,
                                             ValorUM = K.v_Value2,
                                             v_IdMarca = C.v_IdMarca,
                                             Origen = D.v_OrigenTipo,
                                             d_Total = D.v_OrigenTipo == "I" ? pintIdMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : A.d_Total,
                                             i_IdTipoProducto = C == null ? -1 : C.i_IdTipoProducto ?? 0,
                                             CodProducto = C == null ? "" : C.v_CodInterno.Trim(),
                                             TipoExistencia = I == null ? "" : I.v_Value2,
                                             UnidadMedidaProducto = K.v_Value1,
                                             Modelo = C == null ? "" : string.IsNullOrEmpty(C.v_Modelo) ? "" : C.v_Modelo,
                                             Marca = J4 == null || string.IsNullOrEmpty(J4.v_Nombre) ? "" : J4.v_Nombre,
                                             Ubicacion = C == null || string.IsNullOrEmpty(C.v_Ubicacion) ? "" : C.v_Ubicacion,
                                             NroParte = C == null || string.IsNullOrEmpty(C.v_NroParte) ? "" : C.v_NroParte,
                                         }).ToList().AsQueryable();


                            if (!string.IsNullOrEmpty(pstrFilterExpression))
                            {
                                query = query.Where(pstrFilterExpression);
                            }


                            queryFinal = query.ToList().OrderBy(x => x.IdAlmacenOrigen).ThenBy(x => x.v_IdProducto).ThenBy(x => x.t_Fecha).ThenBy(x => x.i_IdTipoMovimiento).ThenBy(x => x.TipoMotivo).ThenBy(x => x.NroPedido).ThenBy(o => o.v_IdMovimientoDetalle).ToList();
                        }
                   
                        #endregion

                    }


                    if (string.IsNullOrEmpty(pstrFilterExpression)) // Cuando el formato es de la Sunat
                    {
                        var Iniciales = queryFinal.Where(l => l.i_IdTipoMotivo == (int)TipoDeMovimiento.Inicial && l.i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso).ToList().GroupBy(p => new { p.v_IdProducto }).Select(d =>
                        // var Iniciales = queryFinal.Where(l => l.i_IdTipoMotivo == (int)TipoDeMovimiento.Inicial).ToList().GroupBy(p => new { p.v_IdProducto }).Select(d =>
                        {
                            var k = d.FirstOrDefault();
                            k.d_Cantidad = d.Sum(h => h.d_Cantidad);
                            k.d_Precio = d.Sum(l => l.d_Precio);
                            k.d_Total = d.Sum(l => l.d_Total);
                            return k;
                        }).AsQueryable().ToList(); ;
                        var Otros = queryFinal.Where(l => l.i_IdTipoMotivo != (int)TipoDeMovimiento.Inicial).ToList();
                        queryFinal = Iniciales.Concat(Otros).ToList();
                        queryFinal = queryFinal.ToList().OrderBy(x => x.v_IdProducto).ThenBy(x => x.t_Fecha).ThenBy(x => x.i_IdTipoMovimiento).ThenBy(x => x.TipoMotivo).ThenBy(x => x.NroPedido).ToList();

                    }

                    queryFinal = queryFinal.ToList().Select(n => new KardexList
                    {


                        v_IdMovimientoDetalle = n.v_IdMovimientoDetalle,
                        v_IdProducto = n.v_IdProducto,
                        v_NombreProducto = n.v_NombreProducto,
                        v_NombreProductoInventarioSunat =
                        "CODIGO EXISTENCIA : " + n.CodProducto + " \n" + n.TipoeExistenciaCompleto + "\n" + "DESCRIPCIÓN : " + n.DescripcionProducto + "\n" + n.UnidadMedidaCompleto,
                        t_Fecha = n.t_Fecha,
                        i_IdTipoMovimiento = n.i_IdTipoMovimiento,
                        d_Cantidad = n.d_Cantidad == 0 || decimal.Parse(n.ValorUM) == 0 ? 0 : FormatoCant == (int)FormatoCantidad.UnidadMedidaProducto ? n.d_Cantidad / decimal.Parse(n.ValorUM) : n.d_Cantidad,
                        d_Precio = n.d_Cantidad == 0 || decimal.Parse(n.ValorUM) == 0 || n.d_Precio == 0 ? 0 : FormatoCant == (int)FormatoCantidad.UnidadMedidaProducto ? n.d_Cantidad == 0 ? 0 : n.d_Precio / (n.d_Cantidad / (decimal.Parse(n.ValorUM))) : n.d_Cantidad == 0 ? 0 : n.d_Precio / n.d_Cantidad,
                        i_EsDevolucion = n.i_EsDevolucion,
                        i_IdTipoMotivo = n.i_IdTipoMotivo,
                        NroPedido = n.NroPedido,
                        v_IdLinea = n.v_IdLinea,
                        IdMoneda = n.IdMoneda,
                        TipoCambio = n.TipoCambio,
                        ClienteProveedor = n.ClienteProveedor,
                        Guia = n.Guia,
                        Documento = n.Documento,
                        TipoMotivo = n.TipoMotivo,
                        NroRegistro = n.NroRegistro,
                        i_IdTipoDocumentoDetalle = n.i_IdTipoDocumentoDetalle,
                        EsDocumentoInterno = n.EsDocumentoInterno,
                        EsDocInverso = n.EsDocInverso,
                        Origen = n.Origen,
                        d_Total = n.d_Total,
                        Almacen = string.IsNullOrEmpty(pstrFilterExpression) ? "" : n.Almacen.ToUpper (),
                        IdAlmacen = string.IsNullOrEmpty(pstrFilterExpression) ? -1 : n.IdAlmacen,
                        IdAlmacenOrigen = string.IsNullOrEmpty(pstrFilterExpression) ? -1 : n.IdAlmacenOrigen,
                        DocumentoInventarioSunat = n.i_IdTipoDocumentoDetalle.ToString("00") + "-" + n.DocumentoInventarioSunat,
                        CorrelativoPle = n.CorrelativoPle,
                        TipoExistencia = n.TipoExistencia,
                        SoloNumeroDocumentoDetalle = n.SoloNumeroDocumentoDetalle,
                        TipoOperacionInventarioSunat = n.TipoOperacionInventarioSunat,
                        CodProducto = n.CodProducto,
                        DescripcionProducto = n.DescripcionProducto,
                        UnidadMedida = n.UnidadMedida,
                        UnidadMedidaCompleto = n.UnidadMedidaCompleto,
                        UnidadMedidaProducto = n.UnidadMedidaProducto,
                        GrupoLlave = n.GrupoLlave,
                        UnidadMedidaCodigoSunat = n.UnidadMedida,
                        Marca = n.Marca,
                        Modelo = n.Modelo,
                        Ubicacion = n.Ubicacion,
                        NroParte = n.NroParte,

                    }).ToList();


                    int Contador = queryFinal.Count();
                    int NumeroElmento = 0;
                    int PosicionProducto = 0;
                    decimal UltimoPrecioSaldo = 0;
                    for (int i = 0; i < Contador; i++)
                    {
                        if (ConsideraDocInternos == 1 || (ConsideraDocInternos == 0 && queryFinal[i].EsDocumentoInterno == 0))
                        {

                            if (ContadorError == 1103)
                            {
                                string h = "";
                            }
                            if (queryFinal[i].CodProducto == "130001655")
                            {
                                string h = "";
                            }
                            oKardexList = new KardexList();
                            oKardexList.v_NombreProductoInventarioSunat = queryFinal[i].v_NombreProductoInventarioSunat;
                            oKardexList.Marca = queryFinal[i].Marca;

                            oKardexList.v_NombreProductoAuxiliar = queryFinal[i].v_NombreProducto + " - MODELO : " + queryFinal[i].Modelo + " - UBICACIÓN : " + queryFinal[i].Ubicacion + " - NRO. PARTE : " + queryFinal[i].NroParte;

                            oKardexList.Modelo = queryFinal[i].Modelo;
                            oKardexList.Ubicacion = queryFinal[i].Ubicacion;
                            oKardexList.TipoOperacionInventarioSunat = queryFinal[i].TipoOperacionInventarioSunat == null ? "" : int.Parse(queryFinal[i].TipoOperacionInventarioSunat).ToString("00"); //!string.IsNullOrEmpty ( TipoOperacion) ?  int.Parse ( TipoOperacion).ToString ("00")  : "00 NO EXISTE CÓDIGO ";
                            oKardexList.v_IdLinea = queryFinal[i].v_IdLinea;
                            oKardexList.v_IdProducto = queryFinal[i].v_IdProducto;
                            oKardexList.NroPedido = queryFinal[i].NroPedido;
                            oKardexList.CorrelativoPle = queryFinal[i].CorrelativoPle;
                            oKardexList.CodProducto = queryFinal[i].CodProducto;
                            oKardexList.DescripcionProducto = queryFinal[i].DescripcionProducto;
                            oKardexList.SoloNumeroDocumentoDetalle = queryFinal[i].SoloNumeroDocumentoDetalle;
                            oKardexList.i_IdTipoDocumentoDetalle = queryFinal[i].i_IdTipoDocumentoDetalle;
                            // oKardexList.UnidadMedida = queryFinal[i].UnidadMedida;
                            oKardexList.UnidadMedidaCodigoSunat = queryFinal[i].UnidadMedida;
                            oKardexList.UnidadMedida = FormatoCant == (int)FormatoCantidad.Unidades ? "UNIDAD" : queryFinal.Where(x => x.CodProducto == oKardexList.CodProducto && x.UnidadMedidaProducto == "UNIDAD").Count() != 0 ? "UNIDAD" : queryFinal[i].UnidadMedidaProducto;
                            oKardexList.TipoExistencia = queryFinal[i].TipoExistencia;
                            oKardexList.v_NombreProducto = queryFinal[i].v_NombreProducto;
                            oKardexList.t_Fecha = queryFinal[i].t_Fecha;
                            oKardexList.v_Fecha = queryFinal[i].t_Fecha.Value.ToString("dd-MMM");
                            oKardexList.v_NombreTipoMovimiento = queryFinal[i].i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso && queryFinal[i].i_IdTipoMotivo == 5 ? "INICIAL" + " " + queryFinal[i].NroRegistro : queryFinal[i].i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso && queryFinal[i].i_IdTipoMotivo != 5 ? "N/I" + " " + queryFinal[i].NroRegistro : "N/S" + " " + queryFinal[i].NroRegistro;
                            oKardexList.i_IdTipoMotivo = queryFinal[i].i_IdTipoMotivo;
                            oKardexList.Documento = queryFinal[i].Documento;
                            oKardexList.IdMoneda = queryFinal[i].IdMoneda;
                            oKardexList.TipoCambio = queryFinal[i].TipoCambio;
                            oKardexList.i_EsDevolucion = queryFinal[i].i_EsDevolucion;
                            oKardexList.ClienteProveedor = queryFinal[i].ClienteProveedor;
                            oKardexList.Empresa = "";
                            oKardexList.Ruc = "";
                            oKardexList.Moneda = "MONEDA REPORTE : " + pstrMoneda.ToUpper();
                            oKardexList.Almacen = queryFinal[i].Almacen.ToUpper();
                            oKardexList.Mes = "";
                            oKardexList.Al = "";
                            oKardexList.IdAlmacen = queryFinal[i].IdAlmacen;
                            oKardexList.Guia = queryFinal[i].Guia;
                            oKardexList.Documento = queryFinal[i].Documento;
                            oKardexList.DocumentoInventarioSunat = queryFinal[i].DocumentoInventarioSunat;
                            oKardexList.i_IdTipoMovimiento = queryFinal[i].i_IdTipoMovimiento;

                            if (LibroInventario && pstrGrupoLlave == "LINEA")
                            {
                                linea l = LineaDictionary.TryGetValue(oKardexList.v_IdLinea, out l) ? l : new linea();
                                oKardexList.GrupoLlave = l != null ? l.v_Nombre : "";
                            }


                            if (i == 0)
                            {


                                if ((oKardexList.i_IdTipoMotivo != 5 && oKardexList.i_IdTipoMovimiento == 1) || oKardexList.i_IdTipoMovimiento == 2)
                                {
                                    KardexList oKardexList1 = new KardexList();
                                    oKardexList1 = (KardexList)oKardexList.Clone();
                                    oKardexList1.v_NombreTipoMovimiento = "INICIAL";
                                    DateTime date = DateTime.Parse("01/" + pdtFechaInicio.Value.Month.ToString("00") + "/" + pdtFechaInicio.Value.Year.ToString());
                                    oKardexList1.t_Fecha = date;
                                    oKardexList1.DocumentoInventarioSunat = "";
                                    oKardexList1.v_Fecha = date.ToString("dd-MMM");
                                    oKardexList1.DocumentoInventarioSunat = "";
                                    oKardexList1.Guia = "";
                                    oKardexList1.Documento = "";
                                    oKardexList1.ClienteProveedor = "";
                                    oKardexList1.Ingreso_Cantidad = 0;
                                    oKardexList1.Ingreso_Precio = 0;
                                    oKardexList1.Ingreso_Total = 0;
                                    oKardexList1.Salida_Cantidad = null;
                                    oKardexList1.i_IdTipoMotivo = 5;
                                    oKardexList1.Salida_Precio = null;
                                    oKardexList1.Salida_Total = null;
                                    oKardexList1.Saldo_Cantidad = 0;
                                    oKardexList1.Saldo_Precio = 0;
                                    oKardexList1.Saldo_Total = 0;
                                    oKardexList1.NumeroElemento = 0;
                                    oKardexList1.i_IdTipoMovimiento = 1;
                                    oKardexList1.IngresoCantidadCalculo = oKardexList1.Ingreso_Cantidad == null ? 0 : oKardexList1.Ingreso_Cantidad;
                                    oKardexList1.Salida_CantidadCalculos = oKardexList1.Salida_Cantidad == null ? 0 : oKardexList1.Salida_Cantidad;
                                    oKardexList1.IngresoTotalCalculo = oKardexList1.Ingreso_Total == null ? 0 : oKardexList1.Ingreso_Total;
                                    oKardexList1.SalidaTotalCalculo = oKardexList1.Salida_Total == null ? 0 : oKardexList1.Salida_Total;

                                    oKardexList1.TipoOperacionInventarioSunat = int.Parse(TipoMovimientos.Where(l => l.i_ItemId == 5).FirstOrDefault().v_Value2).ToString("00");

                                    Lista.Add(oKardexList1);
                                    PosicionProducto = PosicionProducto + 1;
                                    IdProductoOld = oKardexList1.v_NombreProducto + " " + oKardexList1.IdAlmacen;// Se cambio 7 marzo 2017 , porque ahora el nombre producto tiene marca modelo


                                }

                            }

                            CantidadValorizadas objProceso = new CantidadValorizadas();
                            objProceso = ProcesoValorizacionProductoAlmacenPedido(ref  pobjOperationResult, queryFinal[i].i_IdTipoMovimiento.Value, IdProductoOld, queryFinal[i].v_NombreProducto, queryFinal[i].IdAlmacen.ToString(), pintIdMoneda, queryFinal[i].Origen, queryFinal[i].i_EsDevolucion ?? 0, queryFinal[i].d_Cantidad.Value, queryFinal[i].IdMoneda, queryFinal[i].d_Precio ?? 0, PosicionProducto, queryFinal[i].TipoCambio, ref UltimoPrecioSaldo, NumeroElmento, Lista, queryFinal[i].d_Total ?? 0);

                            oKardexList.Ingreso_Cantidad = objProceso.Ingreso_Cantidad;
                            oKardexList.Ingreso_Precio = objProceso.Ingreso_Precio;
                            oKardexList.Ingreso_Total = objProceso.Ingreso_Total;
                            oKardexList.Saldo_Cantidad = objProceso.Saldo_Cantidad;
                            oKardexList.Saldo_Precio = objProceso.Saldo_Precio;
                            oKardexList.Saldo_Total = objProceso.Saldo_Total;
                            oKardexList.Salida_Cantidad = objProceso.Salida_Cantidad;
                            oKardexList.Salida_Precio = objProceso.Salida_Precio;
                            oKardexList.Salida_Total = objProceso.Salida_Total;
                            oKardexList.Ingreso_CantidadInicial = queryFinal[i].i_IdTipoMotivo == 5 ? null : oKardexList.Ingreso_Cantidad;
                            oKardexList.Ingreso_PrecioInicial = queryFinal[i].i_IdTipoMotivo == 5 ? null : oKardexList.Ingreso_Precio;
                            oKardexList.Ingreso_TotalInicial = queryFinal[i].i_IdTipoMotivo == 5 ? null : oKardexList.Ingreso_Total;


                            oKardexList.IngresoCantidadCalculo = oKardexList.Ingreso_Cantidad == null ? 0 : oKardexList.Ingreso_Cantidad;
                            oKardexList.Salida_CantidadCalculos = oKardexList.Salida_Cantidad == null ? 0 : oKardexList.Salida_Cantidad;
                            oKardexList.IngresoTotalCalculo = oKardexList.Ingreso_Total == null ? 0 : oKardexList.Ingreso_Total;
                            oKardexList.SalidaTotalCalculo = oKardexList.Salida_Total == null ? 0 : oKardexList.Salida_Total;


                            oKardexList.NumeroElemento = objProceso.NumeroElemento;
                            IdProductoOld = queryFinal[i].v_NombreProducto + " " + queryFinal[i].IdAlmacen;

                            if (i + 1 < Contador && IdProductoOld == queryFinal[i + 1].v_NombreProducto + " " + queryFinal[i + 1].IdAlmacen)
                            {
                                ///Se agrego para valorizacion sunat
                                int Reg = Lista.Count();
                                //Se agrego para los saldos iniciales 
                                if (PosicionProducto == 0 && Reg > 0 && Lista[Reg - 1].v_NombreProducto + " " + Lista[Reg - 1].IdAlmacen != oKardexList.v_NombreProducto + " " + oKardexList.IdAlmacen)
                                {
                                    if (int.Parse(pdtFechaInicio.Value.Month.ToString()) > 1)
                                    {
                                        KardexList oKardexList1 = new KardexList();
                                        oKardexList1 = (KardexList)queryFinal[i + 1].Clone();

                                        if (oKardexList1.CodProducto == "130005875")
                                        {
                                            string h = "";
                                        }
                                        oKardexList1.v_NombreTipoMovimiento = "INICIAL";
                                        DateTime date = DateTime.Parse("01/" + pdtFechaInicio.Value.Month.ToString("00") + "/" + pdtFechaInicio.Value.Year.ToString());
                                        oKardexList1.t_Fecha = date;
                                        oKardexList1.DocumentoInventarioSunat = "";
                                        oKardexList1.v_Fecha = date.ToString("dd-MMM");
                                        oKardexList1.DocumentoInventarioSunat = "";
                                        oKardexList1.Guia = "";
                                        oKardexList1.Documento = "";
                                        oKardexList1.ClienteProveedor = "";
                                        oKardexList1.Ingreso_Cantidad = 0;
                                        oKardexList1.Ingreso_Precio = 0;
                                        oKardexList1.Ingreso_Total = 0;
                                        oKardexList1.Salida_Cantidad = null;
                                        oKardexList1.i_IdTipoMotivo = 5;
                                        oKardexList1.Salida_Precio = null;
                                        oKardexList1.Salida_Total = null;
                                        oKardexList1.Saldo_Cantidad = 0;
                                        oKardexList1.Saldo_Precio = 0;
                                        oKardexList1.Saldo_Total = 0;
                                        oKardexList1.NumeroElemento = 0;
                                        oKardexList1.IngresoCantidadCalculo = oKardexList1.Ingreso_Cantidad == null ? 0 : oKardexList1.Ingreso_Cantidad;
                                        oKardexList1.Salida_CantidadCalculos = oKardexList1.Salida_Cantidad == null ? 0 : oKardexList1.Salida_Cantidad;
                                        oKardexList1.IngresoTotalCalculo = oKardexList1.Ingreso_Total == null ? 0 : oKardexList1.Ingreso_Total;
                                        oKardexList1.SalidaTotalCalculo = oKardexList1.Salida_Total == null ? 0 : oKardexList1.Salida_Total;
                                        oKardexList1.TipoOperacionInventarioSunat = int.Parse(TipoMovimientos.Where(l => l.i_ItemId == 5).FirstOrDefault().v_Value2).ToString("00");

                                        oKardexList1.i_IdTipoMovimiento = 1;
                                        oKardexList1.v_NombreProductoAuxiliar = queryFinal[i + 1].v_NombreProducto + " - MODELO : " + queryFinal[i + 1].Modelo + " - UBICACIÓN : " + queryFinal[i + 1].Ubicacion + " - NRO. PARTE : " + queryFinal[i + 1].NroParte;

                                        Lista.Add(oKardexList1);
                                        PosicionProducto = PosicionProducto + 1;
                                        IdProductoOld = oKardexList1.v_NombreProducto + " " + oKardexList1.IdAlmacen;
                                    }

                                }

                                // Hasta Acá;

                                PosicionProducto = PosicionProducto + 1;
                                Lista.Add(oKardexList);

                            }
                            else
                            {
                                PosicionProducto = 0;
                                UltimoPrecioSaldo = 0;
                                Lista.Add(oKardexList);
                                if (i + 1 < Contador)// !SeInsertoLista
                                {
                                    KardexList oKardexList1 = new KardexList();
                                    oKardexList1 = (KardexList)queryFinal[i + 1].Clone();
                                    var ExisteListaReporte = Lista.Where(l => l.v_IdProducto + " " + l.IdAlmacen == oKardexList1.v_IdProducto + " " + oKardexList.IdAlmacen && l.NroPedido == oKardexList1.NroPedido).ToList();
                                    if (((queryFinal[i + 1].i_IdTipoMotivo != 5 && queryFinal[i + 1].i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso)) || queryFinal[i + 1].i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeSalida)
                                    {
                                        oKardexList1.v_NombreTipoMovimiento = "INICIAL";
                                        DateTime date = DateTime.Parse("01/" + pdtFechaInicio.Value.Month.ToString("00") + "/" + pdtFechaInicio.Value.Year.ToString());
                                        //DateTime date = DateTime.Parse("01/" + FechaInicioReport.Month.ToString("00") + "/" + FechaInicioReport.Year.ToString());
                                        oKardexList1.v_Fecha = date.ToString("dd-MMM");
                                        oKardexList1.t_Fecha = date;
                                        oKardexList1.DocumentoInventarioSunat = "";
                                        oKardexList1.Guia = "";
                                        oKardexList1.Documento = "";
                                        oKardexList1.ClienteProveedor = "";
                                        oKardexList1.Ingreso_Cantidad = 0;
                                        oKardexList1.Ingreso_Precio = 0;
                                        oKardexList1.Ingreso_Total = 0;
                                       
                                        oKardexList1.Salida_Cantidad = null;
                                        oKardexList1.i_IdTipoMotivo = 5;
                                        oKardexList1.Salida_Precio = null;
                                        oKardexList1.Salida_Total = null;
                                        oKardexList1.Saldo_Cantidad = 0;
                                        oKardexList1.Saldo_Precio = 0;
                                        oKardexList1.Saldo_Total = 0;
                                        oKardexList1.NumeroElemento = 0;
                                        oKardexList1.IngresoCantidadCalculo = oKardexList1.Ingreso_Cantidad == null ? 0 : oKardexList1.Ingreso_Cantidad;
                                        oKardexList1.Salida_CantidadCalculos = oKardexList1.Salida_Cantidad == null ? 0 : oKardexList1.Salida_Cantidad;
                                        oKardexList1.IngresoTotalCalculo = oKardexList1.Ingreso_Total == null ? 0 : oKardexList1.Ingreso_Total;
                                        oKardexList1.SalidaTotalCalculo = oKardexList1.Salida_Total == null ? 0 : oKardexList1.Salida_Total;

                                        oKardexList1.TipoOperacionInventarioSunat = int.Parse(TipoMovimientos.Where(l => l.i_ItemId == 5).FirstOrDefault().v_Value2).ToString("00");
                                        oKardexList1.i_IdTipoMovimiento = 1;
                                        oKardexList1.v_NombreProductoAuxiliar = queryFinal[i + 1].v_NombreProducto + " - MODELO : " + queryFinal[i + 1].Modelo + " - UBICACIÓN : " + queryFinal[i + 1].Ubicacion + " - NRO. PARTE : " + queryFinal[i + 1].NroParte;
                                        Lista.Add(oKardexList1);
                                        PosicionProducto = PosicionProducto + 1;
                                        IdProductoOld = oKardexList1.v_NombreProducto + " " + oKardexList1.IdAlmacen;
                                    }
                                }
                            }

                            ContadorError = ContadorError + 1;
                        }//Cierre If Documentos Internos 
                    }
                    if (FechaInicioReport.Month.ToString("00") != "01")
                    {

                        //var MesAnterior = int.Parse(pdtFechaFin.Value.Month.ToString()) - 1;
                        var MesAnterior = int.Parse(FechaInicioReport.Month.ToString()) - 1;
                        var FechaAnt = DateTime.Parse(DateTime.DaysInMonth(Globals.ClientSession.i_Periodo.Value, MesAnterior).ToString() + "/" + MesAnterior + "/" + Globals.ClientSession.i_Periodo.ToString() + " 23:59");

                        List<KardexList> Sald = new List<KardexList>();
                        Sald = Lista.Select(item => (KardexList)item.Clone()).ToList();

                        var ListaSaldosAnt = Sald.ToList().Where(l => l.t_Fecha <= FechaAnt).GroupBy(l => l.v_NombreProducto).ToList().Select(d =>
                        {

                            if (d.LastOrDefault().CodProducto == "130005875")
                            {
                                string h = "";
                            }
                            var k = d.LastOrDefault();
                            k.v_NombreTipoMovimiento = "INICIAL";
                            DateTime date = DateTime.Parse("01/" + FechaInicioReport.Month.ToString("00") + "/" + FechaInicioReport.Year.ToString()); // DateTime.Parse("01/" + pdtFechaInicio.Value.Month.ToString("00") + "/" + pdtFechaInicio.Value.Year.ToString());
                            k.v_Fecha = date.ToString("dd-MMM");
                            k.t_Fecha = date;
                            k.Guia = "";
                            k.Documento = "";
                            k.ClienteProveedor = "";
                            k.Salida_Cantidad = null;
                            k.Ingreso_Cantidad = null;
                            k.Ingreso_Precio = null;
                            k.Ingreso_Total = null;
                            k.Salida_Cantidad = null;
                            k.Salida_Precio = null;
                            k.Salida_Total = null;
                            k.i_IdTipoMotivo = 5;
                            k.IngresoCantidadCalculo = k.Ingreso_Cantidad == null ? 0 : k.Ingreso_Cantidad;
                            k.Salida_CantidadCalculos = k.Salida_Cantidad == null ? 0 : k.Salida_Cantidad;
                            k.IngresoTotalCalculo = k.Ingreso_Total == null ? 0 : k.Ingreso_Total;
                            k.SalidaTotalCalculo = k.Salida_Total == null ? 0 : k.Salida_Total;
                            k.DocumentoInventarioSunat = "";
                            k.TipoOperacionInventarioSunat = "16";
                            k.UnidadMedidaCodigoSunat = k.UnidadMedidaCodigoSunat;
                            k.v_NombreProductoAuxiliar = k.v_NombreProductoAuxiliar;
                            return k;
                        }).ToList();

                        var jj = Lista.Where(l => l.v_IdProducto == "N001-PD000000527").ToList();
                        var Lista2 = Lista.Where(l => l.t_Fecha > FechaAnt).ToList();
                        Lista = new List<KardexList>();
                        Lista = ListaSaldosAnt.Concat(Lista2).ToList();
                    }
                    List<KardexList> ListaReporte = new List<KardexList>();
                    if (SoloArticulosMovimiento == 1)
                    {
                        var UltimoCadaGrupo = Lista.GroupBy(x => new { x.Almacen, x.v_NombreProducto }).ToList().Select(y => y.LastOrDefault());
                        foreach (var item in UltimoCadaGrupo)
                        {
                            if (!item.v_NombreTipoMovimiento.Contains("INICIAL"))
                            {

                                ListaUltimosRegistros.Add(item);
                            }
                        }

                        foreach (var item in ListaUltimosRegistros)
                        {
                            var Registros = Lista.Where(x => x.v_IdProducto == item.v_IdProducto && x.Almacen == item.Almacen).ToList();
                            {
                                foreach (var item1 in Registros)
                                {
                                    ListaReporte.Add(item1);
                                }
                            }

                        }

                        ListaReporte = ListaReporte.ToList().OrderBy(x => x.IdAlmacenOrigen).ThenBy(x => x.v_NombreProducto).ThenBy(x => x.t_Fecha).ThenBy(x => x.i_IdTipoMovimiento).ThenBy(x => x.TipoMotivo).ThenBy(x => x.NroPedido).ToList();

                        return ListaReporte;
                    }
                    else
                    {
                        if (ArticulosStockMayor0 == 1)
                        {
                            var UltimoCadaGrupo = Lista.GroupBy(x => new { x.Almacen, x.v_NombreProducto }).ToList().Select(y => y.LastOrDefault()).Where(l => l.Saldo_Cantidad > 0).ToList();
                            foreach (var itemUltimo in UltimoCadaGrupo)
                            {
                                var detalles = Lista.Where(l => l.Almacen == itemUltimo.Almacen && l.v_NombreProducto == itemUltimo.v_NombreProducto).ToList();
                                foreach (var itemDetalles in detalles)
                                {
                                    ListaReporte.Add(itemDetalles);
                                }
                            }

                            return ListaReporte;

                        }
                        else if (ArticuloStockCero == 1)
                        {

                            var UltimoCadaGrupo = Lista.GroupBy(x => new { x.Almacen, x.v_NombreProducto }).ToList().Select(y => y.LastOrDefault()).Where(l => l.Saldo_Cantidad == 0).ToList();
                            foreach (var itemUltimo in UltimoCadaGrupo)
                            {
                                var detalles = Lista.Where(l => l.Almacen == itemUltimo.Almacen && l.v_NombreProducto == itemUltimo.v_NombreProducto).ToList();
                                foreach (var itemDetalles in detalles)
                                {
                                    ListaReporte.Add(itemDetalles);
                                }
                            }
                            return ListaReporte;
                        }

                        else if (ArticuloStockNegativo == 1)
                        {
                            var UltimoCadaGrupo = Lista.GroupBy(x => new { x.Almacen, x.v_NombreProducto }).ToList().Select(y => y.LastOrDefault()).Where(l => l.Saldo_Cantidad < 0).ToList();
                            foreach (var itemUltimo in UltimoCadaGrupo)
                            {
                                var detalles = Lista.Where(l => l.Almacen == itemUltimo.Almacen && l.v_NombreProducto == itemUltimo.v_NombreProducto).ToList();
                                foreach (var itemDetalles in detalles)
                                {
                                    ListaReporte.Add(itemDetalles);
                                }
                            }
                            return ListaReporte;
                        }
                        else
                        {
                            if (!LibroInventario)
                            {

                                return Lista;
                            }
                            else
                            {

                                if (OrdenLibroInventario == "CÓDIGO PRODUCTO")
                                {
                                    Lista = Lista.GroupBy(x => new { x.v_IdProducto, x.NroPedido, x.Almacen })
                                                .Select(group => group.Last())
                                                .OrderBy(o => o.CodProducto).ToList();
                                }
                                else
                                {

                                    var hh = Lista.Where(o => o.CodProducto == "130001655").ToList();
                                    Lista = Lista.GroupBy(x => new { x.v_IdProducto, x.NroPedido, x.Almacen })
                                                  .Select(group => group.Last())
                                                  .OrderBy(o => o.v_NombreProducto).ToList();

                                }
                                return Lista;

                            }
                        }


                    }
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ProductoBL.ReporteKardex(), ,Producto :" + IdProductoOld + "Contador Error " + ContadorError;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }


        }
        */

        // Kardex Ultima Version  12 abril 2017







        //public string ObtenerConsultaAnaliticoMovimientos(int tipo, DateTime fechaIni, DateTime fechaFin)
        //{
        //    //idAnexo = string.IsNullOrWhiteSpace(idAnexo) ? "" : idAnexo;
        //    var strFIni = string.Format("{0}-{1}-{2}", fechaIni.Year, fechaIni.Month, fechaIni.Day);
        //    var strFFin = string.Format("{0}-{1}-{2}", fechaFin.Year, fechaFin.Month, fechaFin.Day);
        //    //var ctas = cuentas.Count > 0 ? string.Join(", ", cuentas.Select(p => "'" + p.Trim() + "'")) : "''";
        //    //nroDoc = string.IsNullOrWhiteSpace(nroDoc) ? "null" : "'" + nroDoc + "'";
        //    string query;
        //    //nroCuentaMayor = string.IsNullOrWhiteSpace(nroCuentaMayor) ? string.Empty : nroCuentaMayor.Trim();

        //    if (tipo == 1)
        //        #region query para diariodetalle

        //        query = @"SELECT " +
        //                "\"Extent1\".\"v_IdMovimientoDetalle\", " +
        //                "\"Extent1\".\"i_EsDestino\"," +
        //                "\"Extent1\".\"v_IdDiario\"," +
        //                "\"Extent1\".\"v_IdCliente\"," +
        //                "\"Extent1\".\"v_NroCuenta\"," +
        //                "\"Extent1\".\"v_Naturaleza\"," +
        //                "\"Extent1\".\"d_Importe\"," +
        //                "\"Extent1\".\"d_Cambio\"," +
        //                "\"Extent1\".\"i_IdCentroCostos\"," +
        //                 "\"Extent1\".\"i_IdPatrimonioNeto\"," +
        //                "\"Extent1\".\"i_IdTipoDocumento\"," +
        //                "\"Extent1\".\"v_NroDocumento\"," +
        //                "\"Extent1\".\"t_Fecha\"," +
        //                "\"Extent1\".\"i_IdTipoDocumentoRef\"," +
        //                "\"Extent1\".\"v_NroDocumentoRef\"," +
        //                "\"Extent1\".\"v_Analisis\"," +
        //                "\"Extent1\".\"v_OrigenDestino\"," +
        //                "\"Extent1\".\"v_Pedido\"," +
        //                "\"Extent1\".\"i_Eliminado\"," +
        //                "\"Extent1\".\"i_InsertaIdUsuario\"," +
        //                "\"Extent1\".\"t_InsertaFecha\"," +
        //                "\"Extent1\".\"i_ActualizaIdUsuario\"," +
        //                "\"Extent1\".\"t_ActualizaFecha\" " +
        //                "FROM  diariodetalle AS \"Extent1\" " +
        //                "INNER JOIN diario AS \"Extent2\" ON \"Extent1\".\"v_IdDiario\" = \"Extent2\".\"v_IdDiario\" " +
        //                "WHERE (((((\"Extent1\".\"i_Eliminado\" = 0) AND (\"Extent2\".\"t_Fecha\" >= '" + strFIni +
        //                "')) " +
        //                "AND (\"Extent2\".\"t_Fecha\" <= '" + strFFin + " 23:59')) AND (('' = '" + idAnexo + "') " +
        //                "OR (\"Extent1\".\"v_IdCliente\" = '" + idAnexo + "'))) AND ((0 = " + cuentas.Count +
        //                ") OR (\"Extent1\".\"v_NroCuenta\" IN (" + ctas + "))) AND (\"Extent1\".\"v_NroCuenta\" like '" + nroCuentaMayor + "%')) " +
        //                "AND (((" + idtipoDoc + " < 1) AND ( CAST(" + nroDoc + " AS varchar) IS NULL)) " +
        //                "OR (((CASE WHEN \"Extent1\".\"i_IdTipoDocumento\" IS NULL THEN -1 ELSE \"Extent1\".\"i_IdTipoDocumento\" END) = " +
        //                idtipoDoc + ") " +
        //                "AND (\"Extent1\".\"v_NroDocumento\" = " + nroDoc + "))) ";
        //        #endregion

        //    else
        //        #region query para tesoreriadetalle

        //        query = @"SELECT " +
        //                "\"Extent1\".\"v_IdTesoreriaDetalle\", " +
        //                "\"Extent1\".\"i_EsDestino\", " +
        //                "\"Extent1\".\"v_IdTesoreria\", " +
        //                "\"Extent1\".\"v_IdCliente\", " +
        //                "\"Extent1\".\"v_NroCuenta\", " +
        //                "\"Extent1\".\"v_Naturaleza\", " +
        //                "\"Extent1\".\"d_Importe\", " +
        //                "\"Extent1\".\"d_Cambio\", " +
        //                "\"Extent1\".\"i_IdCentroCostos\", " +
        //                "\"Extent1\".\"i_IdPatrimonioNeto\"," +
        //                "\"Extent1\".\"i_IdCaja\", " +
        //                "\"Extent1\".\"i_IdTipoDocumento\", " +
        //                "\"Extent1\".\"v_NroDocumento\", " +
        //                "\"Extent1\".\"t_Fecha\", " +
        //                "\"Extent1\".\"i_IdTipoDocumentoRef\", " +
        //                "\"Extent1\".\"v_NroDocumentoRef\", " +
        //                "\"Extent1\".\"v_Analisis\", " +
        //                "\"Extent1\".\"v_OrigenDestino\", " +
        //                "\"Extent1\".\"v_Pedido\", " +
        //                "\"Extent1\".\"i_Eliminado\", " +
        //                "\"Extent1\".\"i_InsertaIdUsuario\", " +
        //                "\"Extent1\".\"t_InsertaFecha\", " +
        //                "\"Extent1\".\"i_ActualizaIdUsuario\", " +
        //                "\"Extent1\".\"t_ActualizaFecha\" " +
        //                "FROM  tesoreriadetalle AS \"Extent1\" " +
        //                "INNER JOIN tesoreria AS \"Extent2\" ON \"Extent1\".\"v_IdTesoreria\" = \"Extent2\".\"v_IdTesoreria\" " +
        //                "WHERE (((((\"Extent1\".\"i_Eliminado\" = 0) AND (\"Extent2\".\"t_FechaRegistro\" >= '" +
        //                strFIni + "')) " +
        //                "AND (\"Extent2\".\"t_FechaRegistro\" <= '" + strFFin + " 23:59')) AND (('' = '" + idAnexo +
        //                "') " +
        //                "OR (\"Extent1\".\"v_IdCliente\" = '" + idAnexo + "'))) AND ((0 = " + cuentas.Count +
        //                ") OR (\"Extent1\".\"v_NroCuenta\" IN (" + ctas + "))) AND (\"Extent1\".\"v_NroCuenta\" like '" + nroCuentaMayor + "%')) " +
        //                "AND (((" + idtipoDoc + " < 1) AND ( CAST(" + nroDoc + " AS varchar) IS NULL)) " +
        //                "OR (((CASE WHEN \"Extent1\".\"i_IdTipoDocumento\" IS NULL THEN -1 ELSE \"Extent1\".\"i_IdTipoDocumento\" END) = " +
        //                idtipoDoc + ") " +
        //                "AND (\"Extent1\".\"v_NroDocumento\" = " + nroDoc + "))) ";

        //        #endregion

        //    return query;

        //}


        public string ObtenerConsultaAnaliticoMovimientos(DateTime fechaIni, DateTime fechaFin, string IdProducto)
        {
            IdProducto = string.IsNullOrWhiteSpace(IdProducto) ? "" : IdProducto;
            var strFIni = string.Format("{0}-{1}-{2}", fechaIni.Year, fechaIni.Month, fechaIni.Day);
            var strFFin = string.Format("{0}-{2}-{1}", fechaFin.Year, fechaFin.Month, fechaFin.Day);
            //var ctas = cuentas.Count > 0 ? string.Join(", ", cuentas.Select(p => "'" + p.Trim() + "'")) : "''";
            //nroDoc = string.IsNullOrWhiteSpace(nroDoc) ? "null" : "'" + nroDoc + "'";
            string query;
            //nroCuentaMayor = string.IsNullOrWhiteSpace(nroCuentaMayor) ? string.Empty : nroCuentaMayor.Trim();


            #region query

            query = @"SELECT " +
                    "\"Extent1\".\"v_IdMovimientoDetalle\", " +
                    "\"Extent1\".\"d_CantidadEmpaque\"," +
                     "\"Extent1\".\"v_IdProductoDetalle\"," +
                     "\"Extent1\".\"i_Eliminado\"," +
                     "\"Extent1\".\"v_IdMovimiento\"," +
                    "\"Extent1\".\"d_Cantidad\"," +
                    "\"Extent1\".\"v_NroPedido\"," +
                    "\"Extent1\".\"d_TotalCambio\"," +
                    "\"Extent1\".\"d_Total\"," +
                    "\"Extent1\".\"v_NroGuiaRemision\"," +
                    "\"Extent1\".\"v_NumeroDocumento\"," +
                     "\"Extent1\".\"v_NroSerie\"," +
                    "\"Extent1\".\"v_NroLote\"," +
                    "\"Extent1\".\"t_FechaCaducidad\"," +
                    "\"Extent1\".\"i_IdTipoDocumento\" " +
                    "FROM  movimientodetalle AS \"Extent1\" " +
                    "INNER JOIN movimiento AS \"Extent2\" ON \"Extent1\".\"v_IdMovimiento\" = \"Extent2\".\"v_IdMovimiento\" " +
                    "WHERE ((\"Extent1\".\"i_Eliminado\" = 0) AND  (\"Extent2\".\"i_Eliminado\" = 0) AND ((\"Extent2\".\"t_Fecha\" >= '" + strFIni +
                    "')) " +
                    "AND (\"Extent2\".\"t_Fecha\" <= '" + strFFin + " 23:59')) AND d_Cantidad != 0.00";
            #endregion



            return query;

        }

        public string ObtenerConsultaAnaliticoMovimientosCargaInicial(string IdProducto)
        {
            IdProducto = string.IsNullOrWhiteSpace(IdProducto) ? "" : IdProducto;
            //var strFIni = string.Format("{0}-{1}-{2}", fechaIni.Year, fechaIni.Month, fechaIni.Day);
            //var strFFin = string.Format("{0}-{1}-{2}", fechaFin.Year, fechaFin.Month, fechaFin.Day);
            //var ctas = cuentas.Count > 0 ? string.Join(", ", cuentas.Select(p => "'" + p.Trim() + "'")) : "''";
            //nroDoc = string.IsNullOrWhiteSpace(nroDoc) ? "null" : "'" + nroDoc + "'";
            string query;
            //nroCuentaMayor = string.IsNullOrWhiteSpace(nroCuentaMayor) ? string.Empty : nroCuentaMayor.Trim();
            #region query

            query = @"SELECT " +
                    "\"Extent1\".\"v_IdMovimientoDetalle\", " +
                    "\"Extent1\".\"d_CantidadEmpaque\"," +
                     "\"Extent1\".\"v_IdProductoDetalle\"," +
                     "\"Extent1\".\"i_Eliminado\"," +
                     "\"Extent1\".\"v_IdMovimiento\"," +
                    "\"Extent1\".\"d_Cantidad\"," +
                    "\"Extent1\".\"v_NroPedido\"," +
                    "\"Extent1\".\"d_TotalCambio\"," +
                    "\"Extent1\".\"d_Total\"," +
                    "\"Extent1\".\"v_NroGuiaRemision\"," +
                    "\"Extent1\".\"v_NumeroDocumento\"," +
                    "\"Extent1\".\"v_NroSerie\"," +
                    "\"Extent1\".\"v_NroLote\"," +
                    "\"Extent1\".\"t_FechaCaducidad\"," +
                    "\"Extent1\".\"i_IdTipoDocumento\" " +
                    "FROM  movimientodetalle AS \"Extent1\" " +
                    "INNER JOIN movimiento AS \"Extent2\" ON \"Extent1\".\"v_IdMovimiento\" = \"Extent2\".\"v_IdMovimiento\" " +
                    "WHERE ((\"Extent1\".\"i_Eliminado\" = 0) AND  (\"Extent2\".\"i_Eliminado\" = 0))  ";
            #endregion



            return query;

        }

        public List<KardexList> ReporteKardexValorizadoSunat(ref OperationResult pobjOperationResult, DateTime? pdtFechaInicio, DateTime? pdtFechaFin, string pstrFilterExpression, int pintIdMoneda, string pstrEmpresa, string pstrRUC, string pstrMoneda, int idEstablecimiento, int IncluirPedido, string pstrCodigoProducto, string pstrNumeroPedido, int ArticulosStockMayor0, int SoloArticulosMovimiento, int ConsideraDocInternos, int FormatoCant, string IdMarca, bool ConsiderarGuiasCompras, int ArticuloStockNegativo, int ArticuloStockCero, string IdLinea, DateTime FechaInicioReport, bool LibroInventario, string OrdenLibroInventario, string Modelo,string Agrupar, string pstrGrupoLlave = "", int IdZona = -1)// List<KardexList> ListaSaldosAnteriores = null)
        {
            // Kardex Ultima Version  12 abril 2017
            //Inicialmente en cantidad se toma todas las cantidadempaque , si son nulos (que no deberia pasar) se toma las cantidades
            //En precio estoy capturando los totales de las entradas y/o salidas , luego el precio propiamente dicho se calculara dividiendo el total /cantidad empaque 

            pobjOperationResult.Success = 1;
            string IdProductoOld = "";
            int ContadorError = 1;
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    dbContext.CommandTimeout = 0;
                    List<KardexList> Lista = new List<KardexList>();
                    List<KardexList> ListaUltimosRegistros = new List<KardexList>();
                    List<KardexList> queryFinal = new List<KardexList>();
                    List<KardexList> Consultas = new List<KardexList>();
                    KardexList oKardexList;
                    string Periodo = Globals.ClientSession.i_Periodo.ToString();
                    var LineaDictionary = dbContext.linea.ToList().ToDictionary(o => o.v_IdLinea, o => o);
                    var TipoMovimientos = dbContext.datahierarchy.Where(o => o.i_IsDeleted == 0 && o.i_GroupId == 19).ToList();
                    List<movimientodetalle> movimientodetalle;
                    var queryToExecute = ObtenerConsultaAnaliticoMovimientos(pdtFechaInicio.Value, pdtFechaFin.Value, pstrCodigoProducto);

                    switch (Globals.TipoMotor)
                    {
                        case TipoMotorBD.PostgreSQL:
                            using (var cnx = new PgSqlConnection(Globals.CadenaConexion))
                            {
                                cnx.Execute("set dateformat ymd;");
                                movimientodetalle = cnx.Query<movimientodetalle>(queryToExecute).ToList();
                            }
                            break;

                        default:
                            using (var cnx = new SqlConnection(Globals.CadenaConexion))
                            {
                                var dateFormat = "set dateformat ymd;";
                                var q = dateFormat += queryToExecute;
                                movimientodetalle = cnx.Query<movimientodetalle>(q).ToList();
                            }
                            break;
                    }
                    if (!ConsiderarGuiasCompras)
                    {
                        #region NoConsideraGuiaCompras


                        if (IncluirPedido == 0 || IncluirPedido == 1) // De aca se realizan todos los Reportes
                        {




                            var query = (from A in movimientodetalle
                                         join B in dbContext.productodetalle on new { IdProductoDetalle = A.v_IdProductoDetalle, eliminado = 0 } equals new { IdProductoDetalle = B.v_IdProductoDetalle, eliminado = B.i_Eliminado.Value } into B_join
                                         from B in B_join.DefaultIfEmpty()
                                         join C in dbContext.producto on new { IdProducto = B.v_IdProducto, eliminado = 0 } equals new { IdProducto = C == null ? null : C.v_IdProducto, eliminado = C == null ? 0 : C.i_Eliminado.Value } into C_join
                                         from C in C_join.DefaultIfEmpty()
                                         join D in dbContext.movimiento on new { IdMovimiento = A.v_IdMovimiento, eliminado = 0 } equals new { IdMovimiento = D.v_IdMovimiento, eliminado = D.i_Eliminado.Value } into D_join
                                         from D in D_join.DefaultIfEmpty()
                                         join E in dbContext.cliente on new { IdCliente = D.v_IdCliente, eliminado = 0 } equals new { IdCliente = E.v_IdCliente, eliminado = E.i_Eliminado.Value } into E_join
                                         from E in E_join.DefaultIfEmpty()
                                         join G in dbContext.almacen on new { pIntAlmacen = D.i_IdAlmacenOrigen.Value, eliminado = 0 } equals new { pIntAlmacen = G.i_IdAlmacen, eliminado = G.i_Eliminado.Value } into G_join
                                         from G in G_join.DefaultIfEmpty()
                                         join H in dbContext.documento on new { IdTipoDoc = A.i_IdTipoDocumento.Value, eliminado = 0 } equals new { IdTipoDoc = H == null ? -1 : H.i_CodigoDocumento, eliminado = H == null ? 0 : H.i_Eliminado.Value } into H_join
                                         from H in H_join.DefaultIfEmpty()
                                         join I in dbContext.datahierarchy on new { Grupo = 17, eliminado = 0, Um = C == null ? -1 : C.i_IdUnidadMedida ?? -1 } equals new { Grupo = I == null ? -1 : I.i_GroupId, eliminado = I == null ? 0 : I.i_IsDeleted.Value, Um = I == null ? -1 : I.i_ItemId } into I_join
                                         from I in I_join.DefaultIfEmpty()
                                         join K in dbContext.datahierarchy on new { Grupo = 6, TipoProd = C == null ? -1 : C.i_IdTipoProducto ?? -1, eliminado = 0 } equals new { Grupo = K == null ? -1 : K.i_GroupId, TipoProd = K == null ? -1 : K.i_ItemId, eliminado = K == null ? 0 : K.i_IsDeleted.Value } into K_join
                                         from K in K_join.DefaultIfEmpty()
                                         join L in dbContext.datahierarchy on new { MotivosIngreso = D.i_IdTipoMovimiento == 1 ? 19 : 20, mot = D.i_IdTipoMotivo.Value, eliminado = 0 } equals new { MotivosIngreso = L.i_GroupId, mot = L.i_ItemId, eliminado = L.i_IsDeleted.Value } into L_join
                                         from L in L_join.DefaultIfEmpty()
                                         join J4 in dbContext.marca on new { marc = C == null ? null : C.v_IdMarca, eliminado = 0 } equals new { marc = J4 == null ? null : J4.v_IdMarca, eliminado = J4 == null ? 0 : J4.i_Eliminado.Value } into J4_join
                                         from J4 in J4_join.DefaultIfEmpty()
                                         where A.i_Eliminado == 0 && D.i_IdTipoMovimiento != (int)TipoDeMovimiento.Transferencia
                                         && C != null && D != null
                                         && (D.t_Fecha.Value >= pdtFechaInicio && D.t_Fecha.Value <= pdtFechaFin)
                                         && D.i_IdEstablecimiento == idEstablecimiento
                                         && (C.v_CodInterno == pstrCodigoProducto || pstrCodigoProducto == "") && (A.v_NroPedido == pstrNumeroPedido || pstrNumeroPedido == "")
                                         && C.i_EsServicio == 0
                                         && (C.v_IdMarca == IdMarca || IdMarca == "-1")
                                         && C.i_EsActivoFijo == 0
                                         && (C.v_IdLinea == IdLinea || IdLinea == "-1")
                                         && (D.v_OrigenTipo == null || D.v_OrigenTipo == "" || D.v_OrigenTipo == Constants.OrigenCompra || D.v_OrigenTipo == Constants.OrigenImportacion || D.v_OrigenTipo == Constants.OrigenTransferencia || D.v_OrigenTipo == Constants.OrigenVenta || D.v_OrigenTipo == Constants.OrigenGuiaInterna || (!string.IsNullOrEmpty(D.v_OrigenTipo) && D.v_OrigenTipo != Constants.OrigenGuiaCompra))
                                         && D.i_IdEstablecimiento == idEstablecimiento
                                         && (C.v_Modelo == Modelo || Modelo == "")
                                         select new KardexList
                                         {
                                             v_IdMovimientoDetalle = A.v_IdMovimientoDetalle,
                                             v_IdProducto = C.v_IdProducto,
                                             v_NombreProducto = IncluirPedido == 0 ? "PRODUCTO :" + C.v_CodInterno.Trim() + " " + C.v_Descripcion.Trim() : A.v_NroPedido == null ? "PRODUCTO :" + C.v_CodInterno.Trim() + " " + C.v_Descripcion.Trim() + "  PEDIDO :" : "PRODUCTO :" + C.v_CodInterno.Trim() + " " + C.v_Descripcion.Trim() + "  PEDIDO :" + A.v_NroPedido.Trim(),
                                             v_NombreProductoInventarioSunat = FormatoCant == (int)FormatoCantidad.UnidadMedidaProducto ? K == null || K.v_Value2 == null || K.v_Value1 == null ? "CODIGO EXISTENCIA : " + C.v_CodInterno.Trim() + "\n" + "DESCRIPCIÓN : " + C.v_Descripcion + "\n" + "UNIDAD MEDIDA : " + I.v_Value4 + " " + I.v_Value1 : "CODIGO EXISTENCIA : " + C.v_CodInterno.Trim() + " \n" + "TIPO : " + K.v_Value2 + " " + K.v_Value1 + "\n" + "DESCRIPCIÓN : " + C.v_Descripcion + "\n" + "UNIDAD MEDIDA : " + I.v_Value4 + " " + I.v_Value1
                                                                                                                                        : K == null || K.v_Value2 == null || K.v_Value1 == null ? "CODIGO EXISTENCIA : " + C.v_CodInterno.Trim() + "\n" + "DESCRIPCIÓN : " + C.v_Descripcion + "\n" + "UNIDAD MEDIDA : 07 UNIDAD" : "CODIGO EXISTENCIA : " + C.v_CodInterno.Trim() + " \n" + "TIPO : " + K.v_Value2 + " " + K.v_Value1 + "\n" + "DESCRIPCIÓN : " + C.v_Descripcion + "\n" + "UNIDAD MEDIDA : 07 UNIDAD",
                                             t_Fecha = D.t_Fecha,
                                             i_IdTipoMovimiento = D.i_IdTipoMovimiento,
                                             d_Cantidad = A.d_CantidadEmpaque == null ? A.d_Cantidad == null ? 0 : A.d_Cantidad : A.d_CantidadEmpaque,
                                             d_Precio = D.v_OrigenTipo == "I" ? pintIdMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : A.d_Total,
                                             i_EsDevolucion = D.i_EsDevolucion,
                                             i_IdTipoMotivo = D.i_IdTipoMotivo,
                                             NroPedido = IncluirPedido == 1 ? A.v_NroPedido == null ? "" : A.v_NroPedido.Trim() : "",
                                             v_IdLinea = C.v_IdLinea,
                                             IdMoneda = D.i_IdMoneda.Value,
                                             TipoCambio = D.d_TipoCambio.Value,
                                             Almacen = G.v_Nombre,
                                             ClienteProveedor = D.v_OrigenTipo == "T" ? D.v_Glosa == null || D.v_Glosa == string.Empty ? "TRANSFERENCIA " : D.v_Glosa : E == null ? "**CLIENTE/PROVEEDOR  NO EXISTE**" : (E.v_ApePaterno + " " + E.v_ApeMaterno + " " + E.v_PrimerNombre + " " + E.v_RazonSocial).Trim(),
                                             Guia = string.IsNullOrEmpty(A.v_NroGuiaRemision) ? D.i_IdTipoDocumento == null ? "" : D.i_IdTipoDocumento == (int)TiposDocumentos.GuiaRemision ? D.v_SerieDocumento + "-" + D.v_CorrelativoDocumento : "" : A.v_NroGuiaRemision,
                                             Documento = H == null ? A.v_NumeroDocumento : H.v_Siglas + " " + A.v_NumeroDocumento,
                                             DocumentoInventarioSunat = string.IsNullOrEmpty(A.v_NumeroDocumento) ? "" : A.v_NumeroDocumento,
                                             IdAlmacen = D.i_IdAlmacenOrigen.Value,
                                             TipoMotivo = D.i_IdTipoMotivo == 5 ? 1 : D.v_OrigenTipo == "T" ? 3 : 2,
                                             IdAlmacenOrigen = D.i_IdAlmacenOrigen.Value,
                                             NroRegistro = D.v_Mes + "-" + D.v_Correlativo,
                                             i_IdTipoDocumentoDetalle = A.i_IdTipoDocumento == null ? -1 : A.i_IdTipoDocumento.Value,
                                             EsDocumentoInterno = H == null ? 0 : H.i_UsadoDocumentoInterno == null ? 0 : H.i_UsadoDocumentoInterno.Value,
                                             EsDocInverso = H == null ? 0 : H.i_UsadoDocumentoInverso == null ? 0 : H.i_UsadoDocumentoInverso.Value,
                                             ValorUM = I.v_Value2,
                                             v_IdMarca = C.v_IdMarca,
                                             Origen = D == null ? "" : D.v_OrigenTipo,
                                             d_Total = D == null ? 0 : D.v_OrigenTipo == "I" ? pintIdMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : A.d_Total,
                                             i_IdTipoProducto = C == null ? -1 : C.i_IdTipoProducto ?? 0,
                                             CodProducto = C.v_CodInterno.Trim(),
                                             TipoExistencia = K == null ? "" : K.v_Value2,
                                             DescripcionProducto = C == null ? "" : string.IsNullOrEmpty(C.v_Descripcion) ? "" : C.v_Descripcion.Trim(),
                                             UnidadMedidaProducto = I.v_Value1,
                                             Modelo = C == null ? "" : string.IsNullOrEmpty(C.v_Modelo) ? "" : C.v_Modelo,
                                             Marca = J4 == null || string.IsNullOrEmpty(J4.v_Nombre) ? "" : J4.v_Nombre,
                                             Ubicacion = C == null || string.IsNullOrEmpty(C.v_Ubicacion) ? "" : C.v_Ubicacion,
                                             UnidadMedida = I == null || I.v_Value4 == null || I.v_Value4.Trim() == "" ? "07" : I.v_Value4,
                                             SoloNumeroDocumentoDetalle = A.v_NumeroDocumento == null || A.v_NumeroDocumento.Trim() == "" ? "" : A.v_NumeroDocumento.Trim(),
                                             TipoOperacionInventarioSunat = L == null ? "" : L.v_Value2,
                                             TipoeExistenciaCompleto = K == null ? "TIPO : " + "NO EXISTE TIPO PRODUCTO " : string.IsNullOrEmpty(K.v_Value4) ? "TIPO : " + K.v_Value1 + " - " : "TIPO : " + K.v_Value4 + " - " + K.v_Value1,
                                             UnidadMedidaCompleto = I == null ? "UNIDAD MEDIDA : NO EXISTE UNIDAD ASIGNADA" : I.v_Value4 == null ? "UNIDAD MEDIDA : 07 - UNIDAD" : "UNIDAD MEDIDA : " + I.v_Value4 + " - " + I.v_Value1,
                                             NroParte = C == null || string.IsNullOrEmpty(C.v_NroParte) ? "" : C.v_NroParte,
                                             v_NroLote =A.v_NroLote ,
                                             v_NroSerie =A.v_NroSerie ,
                                             t_FechaCaducidad =A.t_FechaCaducidad ==null ? Fechanull: A.t_FechaCaducidad.Value,
                                         }).ToList().AsQueryable();


                            if (!string.IsNullOrEmpty(pstrFilterExpression))
                            {
                                query = query.Where(pstrFilterExpression);
                            }

                            //queryFinal = query.ToList().OrderBy(x => x.IdAlmacenOrigen).ThenBy(x => x.v_IdProducto).ThenBy(x => x.t_Fecha).ThenBy(x => x.i_IdTipoMovimiento).ThenBy(x => x.TipoMotivo).ThenBy(x => x.NroPedido).ThenBy(o => o.v_IdMovimientoDetalle).ToList();
                            queryFinal = query.ToList().OrderBy(x => x.IdAlmacenOrigen).ThenBy(x => x.v_NombreProducto).ThenBy(x => x.t_Fecha).ThenBy(x => x.i_IdTipoMovimiento).ThenBy(x => x.TipoMotivo).ThenBy(x => x.NroPedido).ThenBy(o => o.v_IdMovimientoDetalle).ToList();


                        }


                        #endregion

                    }
                    else
                    {
                        #region ConsideraGuiaCompra



                        var query = (from A in movimientodetalle
                                     join B in dbContext.productodetalle on new { IdProductoDetalle = A.v_IdProductoDetalle, eliminado = 0 } equals new { IdProductoDetalle = B.v_IdProductoDetalle, eliminado = B.i_Eliminado.Value } into B_join
                                     from B in B_join.DefaultIfEmpty()
                                     join C in dbContext.producto on new { IdProducto = B.v_IdProducto, eliminado = 0 } equals new { IdProducto = C == null ? null : C.v_IdProducto, eliminado = C.i_Eliminado.Value } into C_join
                                     from C in C_join.DefaultIfEmpty()
                                     join D in dbContext.movimiento  on new { IdMovimiento = A.v_IdMovimiento, eliminado = 0 } equals new { IdMovimiento = D.v_IdMovimiento, eliminado = D.i_Eliminado.Value } into D_join
                                     from D in D_join.DefaultIfEmpty()
                                     join E in dbContext.cliente  on new { IdCliente = D.v_IdCliente, eliminado = 0 } equals new { IdCliente = E.v_IdCliente, eliminado = E.i_Eliminado.Value } into E_join
                                     from E in E_join.DefaultIfEmpty()
                                     join G in dbContext.almacen  on new { IdAlmacen = D.i_IdEstablecimiento.Value, eliminado = 0 } equals new { IdAlmacen = G.i_IdAlmacen, eliminado = G.i_Eliminado.Value } into G_join
                                     from G in G_join.DefaultIfEmpty()

                                     join H in dbContext.documento  on new { IdTipoDoc = A.i_IdTipoDocumento.Value, eliminado = 0 } equals new { IdTipoDoc = H == null ? -1 : H.i_CodigoDocumento, eliminado = H.i_Eliminado.Value } into H_join
                                     from H in H_join.DefaultIfEmpty()

                                     join I in dbContext.datahierarchy  on new { Grupo = 17, eliminado = 0, UnidadMedida = C == null ? -1 : C.i_IdUnidadMedida ?? -1 } equals new { Grupo = I == null ? -1 : I.i_GroupId, eliminado = I == null ? 0 : I.i_IsDeleted.Value, UnidadMedida = I == null ? -1 : I.i_ItemId } into I_join
                                     from I in I_join.DefaultIfEmpty()

                                     join K in dbContext.datahierarchy  on new { Grupo = 6, TipoProd = C == null ? -1 : C.i_IdTipoProducto ?? -1, eliminado = 0 } equals new { Grupo = K == null ? -1 : K.i_GroupId, TipoProd = K == null ? -1 : K.i_ItemId, eliminado = K == null ? 0 : K.i_IsDeleted.Value } into K_join
                                     from K in K_join.DefaultIfEmpty()
                                     join J4 in dbContext.marca  on new { marc = C == null ? null : C.v_IdMarca, eliminado = 0 } equals new { marc = J4.v_IdMarca, eliminado = J4.i_Eliminado.Value } into J4_join
                                     from J4 in J4_join.DefaultIfEmpty()

                                     where A.i_Eliminado == 0 && D.i_IdTipoMovimiento != (int)TipoDeMovimiento.Transferencia
                                     && C != null && D != null
                                     && (D.t_Fecha.Value >= pdtFechaInicio && D.t_Fecha.Value <= pdtFechaFin.Value)
                                     && D.i_IdEstablecimiento == idEstablecimiento
                                     && (C.v_CodInterno == pstrCodigoProducto || pstrCodigoProducto == "") && (A.v_NroPedido == pstrNumeroPedido || pstrNumeroPedido == "")
                                     && C.i_EsServicio == 0
                                     && C.i_EsActivoFijo == 0
                                     && (C.v_IdLinea == IdLinea || IdLinea == "-1")
                                     && (C.v_IdMarca == IdMarca || IdMarca == "-1")
                                     && (C.v_Modelo == Modelo || Modelo == "")
                                     select new KardexList
                                     {
                                         v_IdMovimientoDetalle = A.v_IdMovimientoDetalle,
                                         v_IdProducto = C == null ? "" : C.v_IdProducto,
                                         v_NombreProducto = IncluirPedido == 1 ? A.v_NroPedido == null ? "PRODUCTO :" + C.v_CodInterno.Trim() + " " + C.v_Descripcion.Trim() + "  PEDIDO :" : "PRODUCTO :" + C.v_CodInterno.Trim() + " " + C.v_Descripcion.Trim() + "  PEDIDO :" + A.v_NroPedido.Trim() : "PRODUCTO :" + C.v_CodInterno.Trim() + " " + C.v_Descripcion.Trim(),
                                         //v_NombreProductoInventarioSunat = FormatoCant == (int)FormatoCantidad.UnidadMedidaProducto ? "CODIGO EXISTENCIA : " + C.v_CodInterno.Trim() + " \n" + "TIPO : " + K.v_Value2 + " " + K.v_Value1 + "\n" + "DESCRIPCIÓN : " + C.v_Descripcion + "\n" + "UNIDAD MEDIDA : " + I.v_Value4 + " " + I.v_Value1 : "CODIGO EXISTENCIA : " + C.v_CodInterno.Trim() + " \n" + "TIPO : " + K.v_Value2 + " " + K.v_Value1 + "\n" + "DESCRIPCIÓN : " + C.v_Descripcion + "\n" + "UNIDAD MEDIDA : 07 UNIDAD",
                                         v_NombreProductoInventarioSunat = FormatoCant == (int)FormatoCantidad.UnidadMedidaProducto ? K == null || K.v_Value2 == null || K.v_Value1 == null ? "CODIGO EXISTENCIA : " + C.v_CodInterno.Trim() + "\n" + "DESCRIPCIÓN : " + C.v_Descripcion + "\n" + "UNIDAD MEDIDA : " + I.v_Value4 + " " + I.v_Value1 : "CODIGO EXISTENCIA : " + C.v_CodInterno.Trim() + " \n" + "TIPO : " + K.v_Value2 + " " + K.v_Value1 + "\n" + "DESCRIPCIÓN : " + C.v_Descripcion + "\n" + "UNIDAD MEDIDA : " + I.v_Value4 + " " + I.v_Value1
                                                                                         : K == null || K.v_Value2 == null || K.v_Value1 == null ? "CODIGO EXISTENCIA : " + C.v_CodInterno.Trim() + "\n" + "DESCRIPCIÓN : " + C.v_Descripcion + "\n" + "UNIDAD MEDIDA : 07 UNIDAD" : "CODIGO EXISTENCIA : " + C.v_CodInterno.Trim() + " \n" + "TIPO : " + K.v_Value2 + " " + K.v_Value1 + "\n" + "DESCRIPCIÓN : " + C.v_Descripcion + "\n" + "UNIDAD MEDIDA : 07 UNIDAD",
                                         t_Fecha = D.t_Fecha,
                                         i_IdTipoMovimiento = D.i_IdTipoMovimiento,
                                         d_Cantidad = A.d_CantidadEmpaque == null ? A.d_Cantidad == null ? 0 : A.d_Cantidad : A.d_CantidadEmpaque,
                                         d_Precio = D.v_OrigenTipo == "I" ? pintIdMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : A.d_Total,
                                         i_EsDevolucion = D.i_EsDevolucion,
                                         i_IdTipoMotivo = D.i_IdTipoMotivo,
                                         NroPedido = A.v_NroPedido == null ? "" : A.v_NroPedido,
                                         v_IdLinea = C.v_IdLinea,
                                         IdMoneda = D.i_IdMoneda.Value,
                                         TipoCambio = D.d_TipoCambio.Value,
                                         Almacen = G.v_Nombre,
                                         ClienteProveedor = D.v_OrigenTipo == "T" ? D.v_Glosa == null || D.v_Glosa == string.Empty ? "TRANSFERENCIA " : D.v_Glosa : E == null ? "**CLIENTE/PROVEEDOR  NO EXISTE**" : (E.v_ApePaterno + " " + E.v_ApeMaterno + " " + E.v_PrimerNombre + " " + E.v_RazonSocial).Trim(),
                                         //Guia = A.v_NroGuiaRemision,
                                         Guia = string.IsNullOrEmpty(A.v_NroGuiaRemision) ? D.i_IdTipoDocumento == (int)TiposDocumentos.GuiaRemision ? D.v_SerieDocumento + "-" + D.v_CorrelativoDocumento : "" : A.v_NroGuiaRemision,
                                         Documento = H == null ? A.v_NumeroDocumento : H.v_Siglas.Trim() + " " + A.v_NumeroDocumento.Trim(),
                                         DocumentoInventarioSunat = string.IsNullOrEmpty(A.v_NumeroDocumento) ? "" : A.v_NumeroDocumento,
                                         IdAlmacen = D.i_IdAlmacenOrigen.Value,
                                         TipoMotivo = D.i_IdTipoMotivo == 5 ? 1 : D.v_OrigenTipo == "T" ? 3 : 2,
                                         IdAlmacenOrigen = D.i_IdAlmacenOrigen.Value,
                                         NroRegistro = D.v_Mes + "-" + D.v_Correlativo,
                                         i_IdTipoDocumentoDetalle = A.i_IdTipoDocumento == null ? -1 : A.i_IdTipoDocumento.Value,
                                         EsDocumentoInterno = H == null || H.i_UsadoDocumentoInterno == null ? 0 : H.i_UsadoDocumentoInterno.Value,
                                         EsDocInverso = H == null || H.i_UsadoDocumentoInverso == null ? 0 : H.i_UsadoDocumentoInverso.Value,
                                         ValorUM = I.v_Value2,
                                         v_IdMarca = C.v_IdMarca,
                                         Origen = D.v_OrigenTipo,
                                         d_Total = D.v_OrigenTipo == "I" ? pintIdMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : A.d_Total,
                                         i_IdTipoProducto = C == null ? -1 : C.i_IdTipoProducto ?? 0,
                                         CodProducto = C == null ? "" : C.v_CodInterno.Trim(),
                                         TipoExistencia = K == null ? "" : K.v_Value2,
                                         UnidadMedidaProducto = I.v_Value1,
                                         Modelo = C == null ? "" : string.IsNullOrEmpty(C.v_Modelo) ? "" : C.v_Modelo,
                                         Marca = J4 == null || string.IsNullOrEmpty(J4.v_Nombre) ? "" : J4.v_Nombre,
                                         Ubicacion = C == null || string.IsNullOrEmpty(C.v_Ubicacion) ? "" : C.v_Ubicacion,
                                         NroParte = C == null || string.IsNullOrEmpty(C.v_NroParte) ? "" : C.v_NroParte,
                                         v_NroLote = A.v_NroLote,
                                         v_NroSerie = A.v_NroSerie,
                                         t_FechaCaducidad = A.t_FechaCaducidad == null ? Fechanull : A.t_FechaCaducidad.Value,
                                        
                                     }).ToList().AsQueryable();

                        if (!string.IsNullOrEmpty(pstrFilterExpression))
                        {
                            query = query.Where(pstrFilterExpression);
                        }
                        queryFinal = query.ToList().OrderBy(x => x.IdAlmacenOrigen).ThenBy(x => x.v_NombreProducto).ThenBy(x => x.t_Fecha).ThenBy(x => x.i_IdTipoMovimiento).ThenBy(x => x.TipoMotivo).ThenBy(x => x.NroPedido).ThenBy(o => o.v_IdMovimientoDetalle).ToList();

                        #endregion

                    }


                    if (string.IsNullOrEmpty(pstrFilterExpression)) // Cuando el formato es de la Sunat
                    {
                        var Iniciales = queryFinal.Where(l => l.i_IdTipoMotivo == (int)TipoDeMovimiento.Inicial && l.i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso).ToList().GroupBy(p => new { p.v_NombreProducto }).Select(d =>
                        {
                            var k = d.FirstOrDefault();
                            k.DescripcionProducto = Globals.ClientSession.v_RucEmpresa == Constants.RucWortec ? k.DescripcionProducto + " ," + "PEDIDO :" + k.NroPedido : k.DescripcionProducto;
                            k.d_Cantidad = d.Sum(h => h.d_Cantidad);
                            k.d_Precio = d.Sum(l => l.d_Precio);
                            k.d_Total = d.Sum(l => l.d_Total);
                            return k;
                        }).AsQueryable().ToList();
                        var OtrosIngresos = queryFinal.Where(l => l.i_IdTipoMotivo != (int)TipoDeMovimiento.Inicial && l.i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso).ToList();
                        OtrosIngresos.AsParallel().ToList().ForEach(ni =>
                        {
                            ni.DescripcionProducto = Globals.ClientSession.v_RucEmpresa == Constants.RucWortec ? ni.DescripcionProducto + " ," + "PEDIDO :" + ni.NroPedido : ni.DescripcionProducto;
                        });

                        var OtrosSalidas = queryFinal.Where(o => o.i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeSalida).ToList();
                        OtrosSalidas.AsParallel().ToList().ForEach(ns =>
                        {
                            ns.DescripcionProducto = Globals.ClientSession.v_RucEmpresa == Constants.RucWortec ? ns.DescripcionProducto + " ," + "PEDIDO :" + ns.NroPedido : ns.DescripcionProducto;
                        });
                        queryFinal = Iniciales.Concat(OtrosIngresos).Concat (OtrosSalidas).ToList();
                        queryFinal = queryFinal.ToList().OrderBy(x => x.v_NombreProducto).ThenBy(x => x.t_Fecha).ThenBy(x => x.i_IdTipoMovimiento).ThenBy(x => x.TipoMotivo).ThenBy(x => x.NroPedido).ThenBy(o => o.v_IdMovimientoDetalle).ToList();
                    }

                    queryFinal = queryFinal.ToList().Select(n => new KardexList
                    {


                        v_IdMovimientoDetalle = n.v_IdMovimientoDetalle,
                        v_IdProducto = n.v_IdProducto,
                       // v_NombreProducto = n.v_NombreProducto,
                        v_NombreProducto =  Agrupar ==""?n.v_NombreProducto : Agrupar == "Lote" ?  n.v_NombreProducto +" "+ " - LOTE : "+n.v_NroLote  + " - Fecha Venc. : "+ n.t_FechaCaducidad.Date.ToShortDateString ():n.v_NombreProducto +" "+" - SERIE : "+ n.v_NroSerie,
                        v_NombreProductoInventarioSunat =
                         "CODIGO EXISTENCIA : " + n.CodProducto + " \n" + n.TipoeExistenciaCompleto + "\n" + "DESCRIPCIÓN : " + n.DescripcionProducto + "\n" + n.UnidadMedidaCompleto,
                        t_Fecha = n.t_Fecha,
                        i_IdTipoMovimiento = n.i_IdTipoMovimiento,
                        d_Cantidad = n.d_Cantidad == 0 || decimal.Parse(n.ValorUM) == 0 ? 0 : FormatoCant == (int)FormatoCantidad.UnidadMedidaProducto ? n.d_Cantidad / decimal.Parse(n.ValorUM) : n.d_Cantidad,
                        d_Precio = n.d_Cantidad == 0 || decimal.Parse(n.ValorUM) == 0 || n.d_Precio == 0 ? 0 : FormatoCant == (int)FormatoCantidad.UnidadMedidaProducto ? n.d_Cantidad == 0 ? 0 : n.d_Precio / (n.d_Cantidad / (decimal.Parse(n.ValorUM))) : n.d_Cantidad == 0 ? 0 : n.d_Precio / n.d_Cantidad,
                        i_EsDevolucion = n.i_EsDevolucion,
                        i_IdTipoMotivo = n.i_IdTipoMotivo,
                        NroPedido = n.NroPedido,
                        v_IdLinea = n.v_IdLinea,
                        IdMoneda = n.IdMoneda,
                        TipoCambio = n.TipoCambio,
                        ClienteProveedor = n.ClienteProveedor,
                        Guia = n.Guia,
                        Documento = n.Documento,
                        TipoMotivo = n.TipoMotivo,
                        NroRegistro = n.NroRegistro,
                        i_IdTipoDocumentoDetalle = n.i_IdTipoDocumentoDetalle,
                        EsDocumentoInterno = n.EsDocumentoInterno,
                        EsDocInverso = n.EsDocInverso,
                        Origen = n.Origen,
                        d_Total = n.d_Total,
                        Almacen = string.IsNullOrEmpty(pstrFilterExpression) ? "" : n.Almacen.ToUpper(),
                        IdAlmacen = string.IsNullOrEmpty(pstrFilterExpression) ? -1 : n.IdAlmacen,
                        IdAlmacenOrigen = string.IsNullOrEmpty(pstrFilterExpression) ? -1 : n.IdAlmacenOrigen,
                        DocumentoInventarioSunat = n.i_IdTipoDocumentoDetalle.ToString("00") + "-" + n.DocumentoInventarioSunat,
                        TipoExistencia = n.TipoExistencia,
                        SoloNumeroDocumentoDetalle = n.SoloNumeroDocumentoDetalle,
                        TipoOperacionInventarioSunat = n.TipoOperacionInventarioSunat,
                        CodProducto = n.CodProducto,
                        DescripcionProducto = n.DescripcionProducto ,
                        UnidadMedida = n.UnidadMedida,
                        UnidadMedidaCompleto = n.UnidadMedidaCompleto,
                        UnidadMedidaProducto = n.UnidadMedidaProducto,
                        GrupoLlave = n.GrupoLlave,
                        UnidadMedidaCodigoSunat = n.UnidadMedida,
                        Marca = n.Marca,
                        Modelo = n.Modelo,
                        Ubicacion = n.Ubicacion,
                        NroParte = n.NroParte,


                    }).ToList();
                    if (string.IsNullOrEmpty(pstrFilterExpression))
                    {
                        queryFinal = queryFinal.ToList().OrderBy(x => x.v_NombreProducto).ThenBy(x => x.t_Fecha).ThenBy(x => x.i_IdTipoMovimiento).ThenBy(x => x.TipoMotivo).ThenBy(x => x.NroPedido).ThenBy(o => o.v_IdMovimientoDetalle).ToList();
                    }
                    else
                    {
                        queryFinal = queryFinal.ToList().OrderBy(x => x.IdAlmacenOrigen).ThenBy(x => x.v_NombreProducto).ThenBy(x => x.t_Fecha).ThenBy(x => x.i_IdTipoMovimiento).ThenBy(x => x.TipoMotivo).ThenBy(x => x.NroPedido).ThenBy(o => o.v_IdMovimientoDetalle).ToList();
                    }
                    int Contador = queryFinal.Count();
                    int NumeroElmento = 0;
                    int PosicionProducto = 0;
                    decimal UltimoPrecioSaldo = 0;
                    for (int i = 0; i < Contador; i++)
                    {
                        if (ConsideraDocInternos == 1 || (ConsideraDocInternos == 0 && queryFinal[i].EsDocumentoInterno == 0))
                        {
                            oKardexList = new KardexList();
                            oKardexList = queryFinal[i];
                            oKardexList.v_NombreProductoAuxiliar = queryFinal[i].v_NombreProducto + " - MODELO : " + queryFinal[i].Modelo + " - UBICACIÓN : " + queryFinal[i].Ubicacion + " - NRO. PARTE : " + queryFinal[i].NroParte;
                            oKardexList.TipoOperacionInventarioSunat = string.IsNullOrEmpty(queryFinal[i].TipoOperacionInventarioSunat) ? "" : int.Parse(queryFinal[i].TipoOperacionInventarioSunat).ToString("00"); //!string.IsNullOrEmpty ( TipoOperacion) ?  int.Parse ( TipoOperacion).ToString ("00")  : "00 NO EXISTE CÓDIGO ";
                            oKardexList.UnidadMedidaCodigoSunat = queryFinal[i].UnidadMedida;
                            oKardexList.UnidadMedida = FormatoCant == (int)FormatoCantidad.Unidades ? "UNIDAD" : queryFinal.Where(x => x.CodProducto == oKardexList.CodProducto && x.UnidadMedidaProducto == "UNIDAD").Count() != 0 ? "UNIDAD" : queryFinal[i].UnidadMedidaProducto;
                            oKardexList.v_Fecha = queryFinal[i].t_Fecha.Value.ToString("dd-MMM");
                            oKardexList.v_NombreTipoMovimiento = queryFinal[i].i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso && queryFinal[i].i_IdTipoMotivo == 5 ? "INICIAL" + " " + queryFinal[i].NroRegistro : queryFinal[i].i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso && queryFinal[i].i_IdTipoMotivo != 5 ? "N/I" + " " + queryFinal[i].NroRegistro : "N/S" + " " + queryFinal[i].NroRegistro;
                            oKardexList.Empresa = "";
                            oKardexList.Ruc = "";
                            oKardexList.Moneda = "MONEDA REPORTE : " + pstrMoneda.ToUpper();
                            oKardexList.Almacen = queryFinal[i].Almacen.ToUpper();
                            oKardexList.Mes = "";
                            oKardexList.Al = "";
                            //oKardexList.v_NombreProductoInventarioSunat = queryFinal[i].v_NombreProductoInventarioSunat;
                            //oKardexList.Marca = queryFinal[i].Marca;
                            //oKardexList.Modelo = queryFinal[i].Modelo;
                            //oKardexList.Ubicacion = queryFinal[i].Ubicacion;
                            //oKardexList.TipoMotivo = queryFinal[i].TipoMotivo;
                            //oKardexList.v_IdLinea = queryFinal[i].v_IdLinea;
                            //oKardexList.v_IdProducto = queryFinal[i].v_IdProducto;
                            //oKardexList.NroPedido = queryFinal[i].NroPedido;
                            //oKardexList.CorrelativoPle = queryFinal[i].CorrelativoPle;
                            //oKardexList.CodProducto = queryFinal[i].CodProducto;
                            //oKardexList.DescripcionProducto = queryFinal[i].DescripcionProducto;
                            //oKardexList.SoloNumeroDocumentoDetalle = queryFinal[i].SoloNumeroDocumentoDetalle;
                            //oKardexList.i_IdTipoDocumentoDetalle = queryFinal[i].i_IdTipoDocumentoDetalle;
                            //oKardexList.TipoExistencia = queryFinal[i].TipoExistencia;
                            //oKardexList.v_NombreProducto = queryFinal[i].v_NombreProducto;
                            //oKardexList.t_Fecha = queryFinal[i].t_Fecha;
                            //oKardexList.i_IdTipoMotivo = queryFinal[i].i_IdTipoMotivo;
                            //oKardexList.Documento = queryFinal[i].Documento;
                            //oKardexList.IdMoneda = queryFinal[i].IdMoneda;
                            //oKardexList.TipoCambio = queryFinal[i].TipoCambio;
                            //oKardexList.i_EsDevolucion = queryFinal[i].i_EsDevolucion;
                            //oKardexList.ClienteProveedor = queryFinal[i].ClienteProveedor;

                            //oKardexList.IdAlmacen = queryFinal[i].IdAlmacen;
                            //oKardexList.Guia = queryFinal[i].Guia;
                            //oKardexList.Documento = queryFinal[i].Documento;
                            //oKardexList.DocumentoInventarioSunat = queryFinal[i].DocumentoInventarioSunat;
                            //oKardexList.i_IdTipoMovimiento = queryFinal[i].i_IdTipoMovimiento;

                            if (LibroInventario && pstrGrupoLlave == "LINEA")
                            {
                                linea l = LineaDictionary.TryGetValue(oKardexList.v_IdLinea, out l) ? l : new linea();
                                oKardexList.GrupoLlave = l != null ? l.v_Nombre : "";
                            }


                            if (i == 0)
                            {


                                if ((oKardexList.i_IdTipoMotivo != 5 && oKardexList.i_IdTipoMovimiento == 1) || oKardexList.i_IdTipoMovimiento == 2)
                                {
                                    KardexList oKardexList1 = new KardexList();
                                    oKardexList1 = (KardexList)oKardexList.Clone();
                                    oKardexList1.v_NombreTipoMovimiento = "INICIAL";
                                    DateTime date = DateTime.Parse("01/" + pdtFechaInicio.Value.Month.ToString("00") + "/" + pdtFechaInicio.Value.Year.ToString());
                                    oKardexList1.t_Fecha = date;

                                    oKardexList1.DocumentoInventarioSunat = "";
                                    oKardexList1.v_Fecha = date.ToString("dd-MMM");
                                    oKardexList1.DocumentoInventarioSunat = "";
                                    oKardexList1.Guia = "";
                                    oKardexList1.Documento = "";
                                    oKardexList1.ClienteProveedor = "";
                                    oKardexList1.Ingreso_Cantidad = 0;
                                    oKardexList1.Ingreso_Precio = 0;
                                    oKardexList1.Ingreso_Total = 0;
                                    oKardexList1.Salida_Cantidad = null;
                                    oKardexList1.i_IdTipoMotivo = 5;
                                    oKardexList1.Salida_Precio = null;
                                    oKardexList1.Salida_Total = null;
                                    oKardexList1.Saldo_Cantidad = 0;
                                    oKardexList1.Saldo_Precio = 0;
                                    oKardexList1.Saldo_Total = 0;
                                    oKardexList1.NumeroElemento = 0;
                                    oKardexList1.i_IdTipoMovimiento = 1;
                                    oKardexList1.IngresoCantidadCalculo = oKardexList1.Ingreso_Cantidad == null ? 0 : oKardexList1.Ingreso_Cantidad;
                                    oKardexList1.Salida_CantidadCalculos = oKardexList1.Salida_Cantidad == null ? 0 : oKardexList1.Salida_Cantidad;
                                    oKardexList1.IngresoTotalCalculo = oKardexList1.Ingreso_Total == null ? 0 : oKardexList1.Ingreso_Total;
                                    oKardexList1.SalidaTotalCalculo = oKardexList1.Salida_Total == null ? 0 : oKardexList1.Salida_Total;
                                    oKardexList1.TipoOperacionInventarioSunat = int.Parse(TipoMovimientos.Where(l => l.i_ItemId == 5).FirstOrDefault().v_Value2).ToString("00");
                                    Lista.Add(oKardexList1);
                                    PosicionProducto = PosicionProducto + 1;
                                    IdProductoOld = oKardexList1.v_NombreProducto + " " + oKardexList1.IdAlmacen;// Se cambio 7 marzo 2017 , porque ahora el nombre producto tiene marca modelo


                                }

                            }

                            CantidadValorizadas objProceso = new CantidadValorizadas();
                            objProceso = Globals.ClientSession.i_Periodo == 2016 ? ProcesoValorizacionProductoAlmacenPedido2016(ref  pobjOperationResult, queryFinal[i].i_IdTipoMovimiento.Value, IdProductoOld, queryFinal[i].v_NombreProducto, queryFinal[i].IdAlmacen.ToString(), pintIdMoneda, queryFinal[i].Origen, queryFinal[i].i_EsDevolucion ?? 0, queryFinal[i].d_Cantidad.Value, queryFinal[i].IdMoneda, queryFinal[i].d_Precio ?? 0, PosicionProducto, queryFinal[i].TipoCambio, ref UltimoPrecioSaldo, NumeroElmento, Lista, queryFinal[i].d_Total ?? 0) : ProcesoValorizacionProductoAlmacenPedido2017(ref  pobjOperationResult, queryFinal[i].i_IdTipoMovimiento.Value, IdProductoOld, queryFinal[i].v_NombreProducto, queryFinal[i].IdAlmacen.ToString(), pintIdMoneda, queryFinal[i].Origen, queryFinal[i].i_EsDevolucion ?? 0, queryFinal[i].d_Cantidad.Value, queryFinal[i].IdMoneda, queryFinal[i].d_Precio ?? 0, PosicionProducto, queryFinal[i].TipoCambio, ref UltimoPrecioSaldo, NumeroElmento, Lista, queryFinal[i].d_Total ?? 0);

                            oKardexList.Ingreso_Cantidad = objProceso.Ingreso_Cantidad;
                            oKardexList.Ingreso_Precio = objProceso.Ingreso_Precio;
                            oKardexList.Ingreso_Total = objProceso.Ingreso_Total;
                            oKardexList.Saldo_Cantidad = objProceso.Saldo_Cantidad;
                            oKardexList.Saldo_Precio = objProceso.Saldo_Precio;
                            oKardexList.Saldo_Total = objProceso.Saldo_Total;
                            oKardexList.Salida_Cantidad = objProceso.Salida_Cantidad;
                            oKardexList.Salida_Precio = objProceso.Salida_Precio;
                            oKardexList.Salida_Total = objProceso.Salida_Total;
                            oKardexList.Ingreso_CantidadInicial = queryFinal[i].i_IdTipoMotivo == 5 ? null : oKardexList.Ingreso_Cantidad;
                            oKardexList.Ingreso_PrecioInicial = queryFinal[i].i_IdTipoMotivo == 5 ? null : oKardexList.Ingreso_Precio;
                            oKardexList.Ingreso_TotalInicial = queryFinal[i].i_IdTipoMotivo == 5 ? null : oKardexList.Ingreso_Total;


                            oKardexList.IngresoCantidadCalculo = oKardexList.Ingreso_Cantidad == null ? 0 : oKardexList.Ingreso_Cantidad;
                            oKardexList.Salida_CantidadCalculos = oKardexList.Salida_Cantidad == null ? 0 : oKardexList.Salida_Cantidad;
                            oKardexList.IngresoTotalCalculo = oKardexList.Ingreso_Total == null ? 0 : oKardexList.Ingreso_Total;
                            oKardexList.SalidaTotalCalculo = oKardexList.Salida_Total == null ? 0 : oKardexList.Salida_Total;


                            oKardexList.NumeroElemento = objProceso.NumeroElemento;
                            IdProductoOld = queryFinal[i].v_NombreProducto + " " + queryFinal[i].IdAlmacen;

                            if (i + 1 < Contador && IdProductoOld == queryFinal[i + 1].v_NombreProducto + " " + queryFinal[i + 1].IdAlmacen)
                            {
                                ///Se agrego para valorizacion sunat
                                int Reg = Lista.Count();
                                //Se agrego para los saldos iniciales 
                                if (PosicionProducto == 0 && Reg > 0 && Lista[Reg - 1].v_NombreProducto + " " + Lista[Reg - 1].IdAlmacen != oKardexList.v_NombreProducto + " " + oKardexList.IdAlmacen)
                                {
                                    if (int.Parse(pdtFechaInicio.Value.Month.ToString()) > 1)
                                    {
                                        KardexList oKardexList1 = new KardexList();
                                        oKardexList1 = (KardexList)queryFinal[i + 1].Clone();

                                        if (oKardexList1.CodProducto == "130005875")
                                        {
                                            string h = "";
                                        }
                                        oKardexList1.v_NombreTipoMovimiento = "INICIAL";
                                        DateTime date = DateTime.Parse("01/" + pdtFechaInicio.Value.Month.ToString("00") + "/" + pdtFechaInicio.Value.Year.ToString());
                                        oKardexList1.t_Fecha = date;
                                        oKardexList1.DocumentoInventarioSunat = "";
                                        oKardexList1.v_Fecha = date.ToString("dd-MMM");
                                        oKardexList1.DocumentoInventarioSunat = "";
                                        oKardexList1.Guia = "";
                                        oKardexList1.Documento = "";
                                        oKardexList1.ClienteProveedor = "";
                                        oKardexList1.Ingreso_Cantidad = 0;
                                        oKardexList1.Ingreso_Precio = 0;
                                        oKardexList1.Ingreso_Total = 0;
                                        oKardexList1.Salida_Cantidad = null;
                                        oKardexList1.i_IdTipoMotivo = 5;
                                        oKardexList1.Salida_Precio = null;
                                        oKardexList1.Salida_Total = null;
                                        oKardexList1.Saldo_Cantidad = 0;
                                        oKardexList1.Saldo_Precio = 0;
                                        oKardexList1.Saldo_Total = 0;
                                        oKardexList1.NumeroElemento = 0;
                                        oKardexList1.IngresoCantidadCalculo = oKardexList1.Ingreso_Cantidad == null ? 0 : oKardexList1.Ingreso_Cantidad;
                                        oKardexList1.Salida_CantidadCalculos = oKardexList1.Salida_Cantidad == null ? 0 : oKardexList1.Salida_Cantidad;
                                        oKardexList1.IngresoTotalCalculo = oKardexList1.Ingreso_Total == null ? 0 : oKardexList1.Ingreso_Total;
                                        oKardexList1.SalidaTotalCalculo = oKardexList1.Salida_Total == null ? 0 : oKardexList1.Salida_Total;
                                        oKardexList1.TipoOperacionInventarioSunat = int.Parse(TipoMovimientos.Where(l => l.i_ItemId == 5).FirstOrDefault().v_Value2).ToString("00");
                                        oKardexList1.i_IdTipoMovimiento = 1;
                                        oKardexList1.v_NombreProductoAuxiliar = queryFinal[i + 1].v_NombreProducto + " - MODELO : " + queryFinal[i + 1].Modelo + " - UBICACIÓN : " + queryFinal[i + 1].Ubicacion + " - NRO. PARTE : " + queryFinal[i + 1].NroParte;
                                        Lista.Add(oKardexList1);
                                        PosicionProducto = PosicionProducto + 1;
                                        IdProductoOld = oKardexList1.v_NombreProducto + " " + oKardexList1.IdAlmacen;
                                    }

                                }

                                // Hasta Acá;

                                PosicionProducto = PosicionProducto + 1;
                                Lista.Add(oKardexList);

                            }
                            else
                            {
                                PosicionProducto = 0;
                                UltimoPrecioSaldo = 0;
                                Lista.Add(oKardexList);
                                if (i + 1 < Contador)// !SeInsertoLista
                                {
                                    KardexList oKardexList1 = new KardexList();
                                    oKardexList1 = (KardexList)queryFinal[i + 1].Clone();
                                    //var ExisteListaReporte = Lista.Where(l => l.v_IdProducto + " " + l.IdAlmacen == oKardexList1.v_IdProducto + " " + oKardexList.IdAlmacen && l.NroPedido == oKardexList1.NroPedido).ToList();
                                    if (((queryFinal[i + 1].i_IdTipoMotivo != 5 && queryFinal[i + 1].i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso)) || queryFinal[i + 1].i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeSalida)
                                    {
                                        oKardexList1.v_NombreTipoMovimiento = "INICIAL";
                                        DateTime date = DateTime.Parse("01/" + pdtFechaInicio.Value.Month.ToString("00") + "/" + pdtFechaInicio.Value.Year.ToString());
                                        //DateTime date = DateTime.Parse("01/" + FechaInicioReport.Month.ToString("00") + "/" + FechaInicioReport.Year.ToString());
                                        oKardexList1.v_Fecha = date.ToString("dd-MMM");
                                        oKardexList1.t_Fecha = date;
                                        oKardexList1.DocumentoInventarioSunat = "";
                                        oKardexList1.Guia = "";
                                        oKardexList1.Documento = "";
                                        oKardexList1.ClienteProveedor = "";
                                        oKardexList1.Ingreso_Cantidad = 0;
                                        oKardexList1.Ingreso_Precio = 0;
                                        oKardexList1.Ingreso_Total = 0;
                                        oKardexList1.Salida_Cantidad = null;
                                        oKardexList1.i_IdTipoMotivo = 5;
                                        oKardexList1.Salida_Precio = null;
                                        oKardexList1.Salida_Total = null;
                                        oKardexList1.Saldo_Cantidad = 0;
                                        oKardexList1.Saldo_Precio = 0;
                                        oKardexList1.Saldo_Total = 0;
                                        oKardexList1.NumeroElemento = 0;
                                        oKardexList1.IngresoCantidadCalculo = oKardexList1.Ingreso_Cantidad == null ? 0 : oKardexList1.Ingreso_Cantidad;
                                        oKardexList1.Salida_CantidadCalculos = oKardexList1.Salida_Cantidad == null ? 0 : oKardexList1.Salida_Cantidad;
                                        oKardexList1.IngresoTotalCalculo = oKardexList1.Ingreso_Total == null ? 0 : oKardexList1.Ingreso_Total;
                                        oKardexList1.SalidaTotalCalculo = oKardexList1.Salida_Total == null ? 0 : oKardexList1.Salida_Total;
                                        oKardexList1.TipoOperacionInventarioSunat = int.Parse(TipoMovimientos.Where(l => l.i_ItemId == 5).FirstOrDefault().v_Value2).ToString("00");
                                        oKardexList1.i_IdTipoMovimiento = 1;
                                        oKardexList1.v_NombreProductoAuxiliar = queryFinal[i + 1].v_NombreProducto + " - MODELO : " + queryFinal[i + 1].Modelo + " - UBICACIÓN : " + queryFinal[i + 1].Ubicacion + " - NRO. PARTE : " + queryFinal[i + 1].NroParte;
                                        Lista.Add(oKardexList1);
                                        PosicionProducto = PosicionProducto + 1;
                                        IdProductoOld = oKardexList1.v_NombreProducto + " " + oKardexList1.IdAlmacen;
                                    }
                                }
                            }

                            ContadorError = ContadorError + 1;
                        }//Cierre If Documentos Internos 
                    }
                    if (FechaInicioReport.Month.ToString("00") != "01")
                    {

                        //var MesAnterior = int.Parse(pdtFechaFin.Value.Month.ToString()) - 1;
                        var MesAnterior = int.Parse(FechaInicioReport.Month.ToString()) - 1;
                        var FechaAnt = DateTime.Parse(DateTime.DaysInMonth(Globals.ClientSession.i_Periodo.Value, MesAnterior).ToString() + "/" + MesAnterior + "/" + Globals.ClientSession.i_Periodo.ToString() + " 23:59");

                        List<KardexList> Sald = new List<KardexList>();
                        Sald = Lista.Select(item => (KardexList)item.Clone()).ToList();

                        var ListaSaldosAnt = Sald.ToList().Where(l => l.t_Fecha <= FechaAnt).GroupBy(l => new { l.v_NombreProducto, l.IdAlmacen }).ToList().Select(d =>
                        {
                            var k = d.LastOrDefault();
                            k.v_NombreTipoMovimiento = "INICIAL";
                            DateTime date = DateTime.Parse("01/" + FechaInicioReport.Month.ToString("00") + "/" + FechaInicioReport.Year.ToString()); // DateTime.Parse("01/" + pdtFechaInicio.Value.Month.ToString("00") + "/" + pdtFechaInicio.Value.Year.ToString());
                            k.v_Fecha = date.ToString("dd-MMM");
                            k.t_Fecha = date;
                            k.Guia = "";
                            k.Documento = "";
                            k.ClienteProveedor = "";
                            k.Salida_Cantidad = null;
                            k.Ingreso_Cantidad = null;
                            k.Ingreso_Precio = null;
                            k.Ingreso_Total = null;
                            k.Salida_Cantidad = null;
                            k.Salida_Precio = null;
                            k.Salida_Total = null;
                            k.i_IdTipoMotivo = 5;
                            k.TipoMotivo = 1; // se agrego porque salia distorsionado cuando mostraba solo articulos con mov
                            k.i_IdTipoMovimiento = 1;  // se agrego porque salia distorsionado cuando mostraba solo articulos con mov
                            k.i_IdTipoDocumentoDetalle = -1;// se agrego porque es saldo inicial
                            k.SoloNumeroDocumentoDetalle = "";// se agrego porque es saldo inicial
                            //k.IngresoCantidadCalculo = k.Ingreso_Cantidad == null ? 0 : k.Ingreso_Cantidad; //se cambió el 12 junio
                            k.IngresoCantidadCalculo = k.Saldo_Cantidad;
                            k.Salida_CantidadCalculos = k.Salida_Cantidad == null ? 0 : k.Salida_Cantidad;
                            // k.IngresoTotalCalculo = k.Ingreso_Total == null ? 0 : k.Ingreso_Total;
                            k.IngresoTotalCalculo = k.Saldo_Total ?? 0;
                            k.SalidaTotalCalculo = k.Salida_Total == null ? 0 : k.Salida_Total;
                            // k.Ingreso_CantidadInicial = k.IngresoCantidadCalculo;//se cambió el 12 junio
                            k.Ingreso_CantidadInicial = k.Ingreso_Cantidad;
                            k.DocumentoInventarioSunat = "";
                            k.TipoOperacionInventarioSunat = "16";
                            k.UnidadMedidaCodigoSunat = k.UnidadMedidaCodigoSunat;
                            k.v_NombreProductoAuxiliar = k.v_NombreProductoAuxiliar;
                            return k;
                        }).ToList();

                        var jj = Lista.Where(l => l.v_IdProducto == "N001-PD000000527").ToList();
                        var Lista2 = Lista.Where(l => l.t_Fecha > FechaAnt).ToList();
                        Lista = new List<KardexList>();
                        Lista = ListaSaldosAnt.Concat(Lista2).ToList();
                    }
                    List<KardexList> ListaReporte = new List<KardexList>();
                    if (SoloArticulosMovimiento == 1)
                    {
                        var UltimoCadaGrupo = Lista.GroupBy(x => new { x.Almacen, x.v_NombreProducto }).ToList().Select(y => y.LastOrDefault());
                        foreach (var item in UltimoCadaGrupo)
                        {
                            if (!item.v_NombreTipoMovimiento.Contains("INICIAL"))
                            {

                                ListaUltimosRegistros.Add(item);
                            }
                        }

                        foreach (var item in ListaUltimosRegistros)
                        {
                            var Registros = Lista.Where(x => x.v_NombreProducto == item.v_NombreProducto && x.Almacen == item.Almacen).ToList();
                            {
                                foreach (var item1 in Registros)
                                {
                                    ListaReporte.Add(item1);
                                }
                            }

                        }
                        ListaReporte = ListaReporte.ToList().OrderBy(x => x.IdAlmacen).ThenBy(x => x.v_NombreProducto).ThenBy(x => x.t_Fecha).ThenBy(x => x.i_IdTipoMovimiento).ThenBy(x => x.TipoMotivo).ThenBy(x => x.NroPedido).ToList();

                        return ListaReporte;
                    }
                    else
                    {
                        if (ArticulosStockMayor0 == 1)
                        {
                            var UltimoCadaGrupo = Lista.GroupBy(x => new { x.Almacen, x.v_NombreProducto }).ToList().Select(y => y.LastOrDefault()).Where(l => l.Saldo_Cantidad > 0).ToList();
                            foreach (var itemUltimo in UltimoCadaGrupo)
                            {
                                var detalles = Lista.Where(l => l.Almacen == itemUltimo.Almacen && l.v_NombreProducto == itemUltimo.v_NombreProducto).ToList();
                                foreach (var itemDetalles in detalles)
                                {
                                    ListaReporte.Add(itemDetalles);
                                }
                            }

                            return ListaReporte;

                        }
                        else if (ArticuloStockCero == 1)
                        {

                            var UltimoCadaGrupo = Lista.GroupBy(x => new { x.Almacen, x.v_NombreProducto }).ToList().Select(y => y.LastOrDefault()).Where(l => l.Saldo_Cantidad == 0).ToList();
                            foreach (var itemUltimo in UltimoCadaGrupo)
                            {
                                var detalles = Lista.Where(l => l.Almacen == itemUltimo.Almacen && l.v_NombreProducto == itemUltimo.v_NombreProducto).ToList();
                                foreach (var itemDetalles in detalles)
                                {
                                    ListaReporte.Add(itemDetalles);
                                }
                            }
                            return ListaReporte;
                        }

                        else if (ArticuloStockNegativo == 1)
                        {
                            var UltimoCadaGrupo = Lista.GroupBy(x => new { x.Almacen, x.v_NombreProducto }).ToList().Select(y => y.LastOrDefault()).Where(l => l.Saldo_Cantidad < 0).ToList();
                            foreach (var itemUltimo in UltimoCadaGrupo)
                            {
                                var detalles = Lista.Where(l => l.Almacen == itemUltimo.Almacen && l.v_NombreProducto == itemUltimo.v_NombreProducto).ToList();
                                foreach (var itemDetalles in detalles)
                                {
                                    ListaReporte.Add(itemDetalles);
                                }
                            }
                            return ListaReporte;
                        }
                        else
                        {
                            if (!LibroInventario)
                            {
                                return Lista;
                            }
                            else
                            {

                                if (OrdenLibroInventario == "CÓDIGO PRODUCTO")
                                {
                                    Lista = Lista.GroupBy(x => new { x.v_IdProducto, x.NroPedido, x.Almacen })
                                                .Select(group => group.Last())
                                                .OrderBy(o => o.CodProducto).ToList();
                                }
                                else
                                {
                                    Lista = Lista.GroupBy(x => new { x.v_IdProducto, x.NroPedido, x.Almacen })
                                                  .Select(group => group.Last())
                                                  .OrderBy(o => o.v_NombreProducto).ToList();
                                }
                                return Lista;

                            }
                        }


                    }
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ProductoBL.ReporteKardex(), ,Producto :" + IdProductoOld + "Contador Error " + ContadorError;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }


        }


        public List<KardexList> ReporteKardexValorizadoSunatAntiguo(ref OperationResult pobjOperationResult, DateTime? pdtFechaInicio, DateTime? pdtFechaFin, string pstrFilterExpression, int pintIdMoneda, string pstrEmpresa, string pstrRUC, string pstrMoneda, int idEstablecimiento, int IncluirPedido, string pstrCodigoProducto, string pstrNumeroPedido, int ArticulosStockMayor0, int SoloArticulosMovimiento, int ConsideraDocInternos, int FormatoCant, string IdMarca, bool ConsiderarGuiasCompras, int ArticuloStockNegativo, int ArticuloStockCero, string IdLinea, DateTime FechaInicioReport, bool LibroInventario, string OrdenLibroInventario, string Modelo, string pstrGrupoLlave = "", int IdZona = -1)// List<KardexList> ListaSaldosAnteriores = null)
        {
            // Kardex Ultima Version  12 abril 2017
            //Inicialmente en cantidad se toma todas las cantidadempaque , si son nulos (que no deberia pasar) se toma las cantidades
            //En precio estoy capturando los totales de las entradas y/o salidas , luego el precio propiamente dicho se calculara dividiendo el total /cantidad empaque 

            pobjOperationResult.Success = 1;
            string IdProductoOld = "";
            int ContadorError = 1;


            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    dbContext.CommandTimeout = 0;
                    List<KardexList> Lista = new List<KardexList>();
                    List<KardexList> ListaUltimosRegistros = new List<KardexList>();
                    List<KardexList> queryFinal = new List<KardexList>();
                    List<KardexList> Consultas = new List<KardexList>();
                    KardexList oKardexList;
                    string Periodo = Globals.ClientSession.i_Periodo.ToString();
                    var LineaDictionary = dbContext.linea.ToList().ToDictionary(o => o.v_IdLinea, o => o);
                    var TipoMovimientos = dbContext.datahierarchy.Where(o => o.i_IsDeleted == 0 && o.i_GroupId == 19).ToList();
                    if (!ConsiderarGuiasCompras)
                    {
                        #region NoConsideraGuiaCompras


                        if (IncluirPedido == 0 || IncluirPedido == 1) // De aca se realizan todos los Reportes
                        {

                            var query = (from A in dbContext.movimientodetalle
                                         join B in dbContext.productodetalle on new { IdProductoDetalle = A.v_IdProductoDetalle, eliminado = 0 } equals new { IdProductoDetalle = B.v_IdProductoDetalle, eliminado = B.i_Eliminado.Value } into B_join
                                         from B in B_join.DefaultIfEmpty()
                                         join C in dbContext.producto on new { IdProducto = B.v_IdProducto, eliminado = 0 } equals new { IdProducto = C.v_IdProducto, eliminado = C.i_Eliminado.Value } into C_join
                                         from C in C_join.DefaultIfEmpty()
                                         join D in dbContext.movimiento on new { IdMovimiento = A.v_IdMovimiento, eliminado = 0 } equals new { IdMovimiento = D.v_IdMovimiento, eliminado = D.i_Eliminado.Value } into D_join
                                         from D in D_join.DefaultIfEmpty()
                                         join E in dbContext.cliente on new { IdCliente = D.v_IdCliente, eliminado = 0 } equals new { IdCliente = E.v_IdCliente, eliminado = E.i_Eliminado.Value } into E_join
                                         from E in E_join.DefaultIfEmpty()
                                         join G in dbContext.almacen on new { pIntAlmacen = D.i_IdAlmacenOrigen.Value, eliminado = 0 } equals new { pIntAlmacen = G.i_IdAlmacen, eliminado = G.i_Eliminado.Value } into G_join
                                         from G in G_join.DefaultIfEmpty()
                                         join H in dbContext.documento on new { IdTipoDoc = A.i_IdTipoDocumento.Value, eliminado = 0 } equals new { IdTipoDoc = H.i_CodigoDocumento, eliminado = H.i_Eliminado.Value } into H_join
                                         from H in H_join.DefaultIfEmpty()
                                         join I in dbContext.datahierarchy on new { Grupo = 17, eliminado = 0, Um = C.i_IdUnidadMedida.Value } equals new { Grupo = I.i_GroupId, eliminado = I.i_IsDeleted.Value, Um = I.i_ItemId } into I_join
                                         from I in I_join.DefaultIfEmpty()
                                         join K in dbContext.datahierarchy on new { Grupo = 6, TipoProd = C.i_IdTipoProducto ?? -1, eliminado = 0 } equals new { Grupo = K.i_GroupId, TipoProd = K.i_ItemId, eliminado = K.i_IsDeleted.Value } into K_join
                                         from K in K_join.DefaultIfEmpty()
                                         join L in dbContext.datahierarchy on new { MotivosIngreso = D.i_IdTipoMovimiento == 1 ? 19 : 20, mot = D.i_IdTipoMotivo.Value, eliminado = 0 } equals new { MotivosIngreso = L.i_GroupId, mot = L.i_ItemId, eliminado = L.i_IsDeleted.Value } into L_join
                                         from L in L_join.DefaultIfEmpty()
                                         join J4 in dbContext.marca on new { marc = C.v_IdMarca, eliminado = 0 } equals new { marc = J4.v_IdMarca, eliminado = J4.i_Eliminado.Value } into J4_join
                                         from J4 in J4_join.DefaultIfEmpty()
                                         where A.i_Eliminado == 0 && D.i_IdTipoMovimiento != (int)TipoDeMovimiento.Transferencia
                                         && (D.t_Fecha.Value >= pdtFechaInicio && D.t_Fecha.Value <= pdtFechaFin)
                                         && D.i_IdEstablecimiento == idEstablecimiento
                                         && (C.v_CodInterno == pstrCodigoProducto || pstrCodigoProducto == "")
                                         && (A.v_NroPedido == pstrNumeroPedido || pstrNumeroPedido == "")
                                         && C.i_EsServicio == 0
                                         && (C.v_IdMarca == IdMarca || IdMarca == "-1")
                                         && C.i_EsActivoFijo == 0
                                         && (C.v_IdLinea == IdLinea || IdLinea == "-1")
                                         && (D.v_OrigenTipo == null || D.v_OrigenTipo == "" || D.v_OrigenTipo == Constants.OrigenCompra || D.v_OrigenTipo == Constants.OrigenImportacion || D.v_OrigenTipo == Constants.OrigenTransferencia || D.v_OrigenTipo == Constants.OrigenVenta || D.v_OrigenTipo == Constants.OrigenGuiaInterna || (!string.IsNullOrEmpty(D.v_OrigenTipo) && D.v_OrigenTipo != Constants.OrigenGuiaCompra))
                                         && D.i_IdEstablecimiento == idEstablecimiento
                                         && (C.v_Modelo == Modelo || Modelo == "")
                                         select new KardexList
                                         {
                                             v_IdMovimientoDetalle = A.v_IdMovimientoDetalle,
                                             v_IdProducto = C.v_IdProducto,
                                             v_NombreProducto = IncluirPedido == 0 ? "PRODUCTO :" + C.v_CodInterno.Trim() + " " + C.v_Descripcion.Trim() : A.v_NroPedido == null ? "PRODUCTO :" + C.v_CodInterno.Trim() + " " + C.v_Descripcion.Trim() + "  PEDIDO :" : "PRODUCTO :" + C.v_CodInterno.Trim() + " " + C.v_Descripcion.Trim() + "  PEDIDO :" + A.v_NroPedido.Trim(),
                                             v_NombreProductoInventarioSunat = FormatoCant == (int)FormatoCantidad.UnidadMedidaProducto ? "CODIGO EXISTENCIA : " + C.v_CodInterno.Trim() + " \n" + "TIPO : " + K.v_Value2 + " " + K.v_Value1 + "\n" + "DESCRIPCIÓN : " + C.v_Descripcion + "\n" + "UNIDAD MEDIDA : " + I.v_Value4 + " " + I.v_Value1 : "CODIGO EXISTENCIA : " + C.v_CodInterno.Trim() + " \n" + "TIPO : " + K.v_Value2 + " " + K.v_Value1 + "\n" + "DESCRIPCIÓN : " + C.v_Descripcion + "\n" + "UNIDAD MEDIDA : 07 UNIDAD",
                                             t_Fecha = D.t_Fecha,
                                             i_IdTipoMovimiento = D.i_IdTipoMovimiento,
                                             d_Cantidad = A.d_CantidadEmpaque == null ? A.d_Cantidad == null ? 0 : A.d_Cantidad : A.d_CantidadEmpaque,
                                             d_Precio = D.v_OrigenTipo == "I" ? pintIdMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : A.d_Total,
                                             i_EsDevolucion = D.i_EsDevolucion,
                                             i_IdTipoMotivo = D.i_IdTipoMotivo,
                                             NroPedido = IncluirPedido == 1 ? A.v_NroPedido == null ? "" : A.v_NroPedido.Trim() : "",
                                             v_IdLinea = C.v_IdLinea,
                                             IdMoneda = D.i_IdMoneda.Value,
                                             TipoCambio = D.d_TipoCambio.Value,
                                             Almacen = G.v_Nombre,
                                             ClienteProveedor = D.v_OrigenTipo == "T" ? D.v_Glosa == null || D.v_Glosa == string.Empty ? "TRANSFERENCIA " : D.v_Glosa : E == null ? "**CLIENTE/PROVEEDOR  NO EXISTE**" : (E.v_ApePaterno + " " + E.v_ApeMaterno + " " + E.v_PrimerNombre + " " + E.v_RazonSocial).Trim(),
                                             Guia = string.IsNullOrEmpty(A.v_NroGuiaRemision) ? D.i_IdTipoDocumento == (int)TiposDocumentos.GuiaRemision ? D.v_SerieDocumento + "-" + D.v_CorrelativoDocumento : "" : A.v_NroGuiaRemision,
                                             Documento = H == null ? A.v_NumeroDocumento : H.v_Siglas + " " + A.v_NumeroDocumento,
                                             DocumentoInventarioSunat = string.IsNullOrEmpty(A.v_NumeroDocumento) ? "" : A.v_NumeroDocumento,
                                             IdAlmacen = D.i_IdAlmacenOrigen.Value,
                                             TipoMotivo = D.i_IdTipoMotivo == 5 ? 1 : D.v_OrigenTipo == "T" ? 3 : 2,
                                             IdAlmacenOrigen = D.i_IdAlmacenOrigen.Value,
                                             NroRegistro = D.v_Mes + "-" + D.v_Correlativo,
                                             i_IdTipoDocumentoDetalle = A.i_IdTipoDocumento == null ? -1 : A.i_IdTipoDocumento.Value,
                                             EsDocumentoInterno = H.i_UsadoDocumentoInterno == null ? 0 : H.i_UsadoDocumentoInterno.Value,
                                             EsDocInverso = H.i_UsadoDocumentoInverso == null ? 0 : H.i_UsadoDocumentoInverso.Value,
                                             ValorUM = I.v_Value2,
                                             v_IdMarca = C.v_IdMarca,
                                             Origen = D.v_OrigenTipo,
                                             d_Total = D.v_OrigenTipo == "I" ? pintIdMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : A.d_Total,
                                             i_IdTipoProducto = C == null ? -1 : C.i_IdTipoProducto ?? 0,
                                             CodProducto = C.v_CodInterno.Trim(),
                                             TipoExistencia = K == null ? "" : K.v_Value2,
                                             DescripcionProducto = C == null ? "" : string.IsNullOrEmpty(C.v_Descripcion) ? "" : C.v_Descripcion.Trim(),
                                             UnidadMedidaProducto = I.v_Value1,
                                             Modelo = C == null ? "" : string.IsNullOrEmpty(C.v_Modelo) ? "" : C.v_Modelo,
                                             Marca = J4 == null || string.IsNullOrEmpty(J4.v_Nombre) ? "" : J4.v_Nombre,
                                             Ubicacion = C == null || string.IsNullOrEmpty(C.v_Ubicacion) ? "" : C.v_Ubicacion,
                                             UnidadMedida = I == null || I.v_Value4.Trim() == null || I.v_Value4.Trim() == "" ? "07" : I.v_Value4,
                                             // CorrelativoPle = D.v_Mes.Trim() + D.v_Correlativo.Trim().Substring(0, 2) + D.v_Correlativo.Trim().Substring(3, 8),
                                             SoloNumeroDocumentoDetalle = string.IsNullOrEmpty(A.v_NumeroDocumento.Trim()) ? "" : A.v_NumeroDocumento.Trim(),
                                             TipoOperacionInventarioSunat = L == null ? "" : L.v_Value2,
                                             TipoeExistenciaCompleto = K == null ? "TIPO : " + "NO EXISTE TIPO PRODUCTO " : string.IsNullOrEmpty(K.v_Value4) ? "TIPO : " + K.v_Value1 + " - " : "TIPO : " + K.v_Value4 + " - " + K.v_Value1,
                                             UnidadMedidaCompleto = I == null ? "UNIDAD MEDIDA : NO EXISTE UNIDAD ASIGNADA" : I.v_Value4 == null ? "UNIDAD MEDIDA : 07 - UNIDAD" : "UNIDAD MEDIDA : " + I.v_Value4 + " - " + I.v_Value1,
                                             NroParte = C == null || string.IsNullOrEmpty(C.v_NroParte) ? "" : C.v_NroParte,

                                         }).ToList().AsQueryable();


                            if (!string.IsNullOrEmpty(pstrFilterExpression))
                            {
                                query = query.Where(pstrFilterExpression);
                            }

                            //queryFinal = query.ToList().OrderBy(x => x.IdAlmacenOrigen).ThenBy(x => x.v_IdProducto).ThenBy(x => x.t_Fecha).ThenBy(x => x.i_IdTipoMovimiento).ThenBy(x => x.TipoMotivo).ThenBy(x => x.NroPedido).ThenBy(o => o.v_IdMovimientoDetalle).ToList();
                            queryFinal = query.ToList().OrderBy(x => x.IdAlmacenOrigen).ThenBy(x => x.v_NombreProducto).ThenBy(x => x.t_Fecha).ThenBy(x => x.i_IdTipoMovimiento).ThenBy(x => x.TipoMotivo).ThenBy(x => x.NroPedido).ThenBy(o => o.v_IdMovimientoDetalle).ToList();


                        }


                        #endregion

                    }
                    else
                    {
                        #region ConsideraGuiaCompra



                        var query = (from A in dbContext.movimientodetalle
                                     join B in dbContext.productodetalle on new { IdProductoDetalle = A.v_IdProductoDetalle, eliminado = 0 } equals new { IdProductoDetalle = B.v_IdProductoDetalle, eliminado = B.i_Eliminado.Value } into B_join
                                     from B in B_join.DefaultIfEmpty()
                                     join C in dbContext.producto on new { IdProducto = B.v_IdProducto, eliminado = 0 } equals new { IdProducto = C.v_IdProducto, eliminado = C.i_Eliminado.Value } into C_join
                                     from C in C_join.DefaultIfEmpty()
                                     join D in dbContext.movimiento on new { IdMovimiento = A.v_IdMovimiento, eliminado = 0 } equals new { IdMovimiento = D.v_IdMovimiento, eliminado = D.i_Eliminado.Value } into D_join
                                     from D in D_join.DefaultIfEmpty()
                                     join E in dbContext.cliente on new { IdCliente = D.v_IdCliente, eliminado = 0 } equals new { IdCliente = E.v_IdCliente, eliminado = E.i_Eliminado.Value } into E_join
                                     from E in E_join.DefaultIfEmpty()
                                     join G in dbContext.almacen on new { IdAlmacen = D.i_IdEstablecimiento.Value, eliminado = 0 } equals new { IdAlmacen = G.i_IdAlmacen, eliminado = G.i_Eliminado.Value } into G_join
                                     from G in G_join.DefaultIfEmpty()

                                     join H in dbContext.documento on new { IdTipoDoc = A.i_IdTipoDocumento.Value, eliminado = 0 } equals new { IdTipoDoc = H.i_CodigoDocumento, eliminado = H.i_Eliminado.Value } into H_join
                                     from H in H_join.DefaultIfEmpty()

                                     join I in dbContext.datahierarchy on new { Grupo = 17, eliminado = 0, UnidadMedida = C.i_IdUnidadMedida.Value } equals new { Grupo = I.i_GroupId, eliminado = I.i_IsDeleted.Value, UnidadMedida = I.i_ItemId } into I_join
                                     from I in I_join.DefaultIfEmpty()

                                     join K in dbContext.datahierarchy on new { Grupo = 6, TipoProd = C.i_IdTipoProducto ?? -1, eliminado = 0 } equals new { Grupo = K.i_GroupId, TipoProd = K.i_ItemId, eliminado = K.i_IsDeleted.Value } into K_join
                                     from K in K_join.DefaultIfEmpty()
                                     join J4 in dbContext.marca on new { marc = C.v_IdMarca, eliminado = 0 } equals new { marc = J4.v_IdMarca, eliminado = J4.i_Eliminado.Value } into J4_join
                                     from J4 in J4_join.DefaultIfEmpty()

                                     where A.i_Eliminado == 0 && D.i_IdTipoMovimiento != (int)TipoDeMovimiento.Transferencia
                                     && (D.t_Fecha.Value >= pdtFechaInicio && D.t_Fecha.Value <= pdtFechaFin.Value)
                                     && D.i_IdEstablecimiento == idEstablecimiento
                                     && (C.v_CodInterno == pstrCodigoProducto || pstrCodigoProducto == "") && (A.v_NroPedido == pstrNumeroPedido || pstrNumeroPedido == "")
                                     && C.i_EsServicio == 0
                                      && C.i_EsActivoFijo == 0
                                       && (C.v_IdLinea == IdLinea || IdLinea == "-1")
                                      && (C.v_IdMarca == IdMarca || IdMarca == "-1")
                                        && (C.v_Modelo == Modelo || Modelo == "")
                                     select new KardexList
                                     {
                                         v_IdMovimientoDetalle = A.v_IdMovimientoDetalle,
                                         v_IdProducto = C == null ? "" : C.v_IdProducto,
                                         v_NombreProducto = IncluirPedido == 1 ? A.v_NroPedido == null ? "PRODUCTO :" + C.v_CodInterno.Trim() + " " + C.v_Descripcion.Trim() + "  PEDIDO :" : "PRODUCTO :" + C.v_CodInterno.Trim() + " " + C.v_Descripcion.Trim() + "  PEDIDO :" + A.v_NroPedido.Trim() : "PRODUCTO :" + C.v_CodInterno.Trim() + " " + C.v_Descripcion.Trim(),
                                         v_NombreProductoInventarioSunat = FormatoCant == (int)FormatoCantidad.UnidadMedidaProducto ? "CODIGO EXISTENCIA : " + C.v_CodInterno.Trim() + " \n" + "TIPO : " + K.v_Value2 + " " + K.v_Value1 + "\n" + "DESCRIPCIÓN : " + C.v_Descripcion + "\n" + "UNIDAD MEDIDA : " + I.v_Value4 + " " + I.v_Value1 : "CODIGO EXISTENCIA : " + C.v_CodInterno.Trim() + " \n" + "TIPO : " + K.v_Value2 + " " + K.v_Value1 + "\n" + "DESCRIPCIÓN : " + C.v_Descripcion + "\n" + "UNIDAD MEDIDA : 07 UNIDAD",
                                         t_Fecha = D.t_Fecha,
                                         i_IdTipoMovimiento = D.i_IdTipoMovimiento,
                                         d_Cantidad = A.d_CantidadEmpaque == null ? A.d_Cantidad == null ? 0 : A.d_Cantidad : A.d_CantidadEmpaque,
                                         d_Precio = D.v_OrigenTipo == "I" ? pintIdMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : A.d_Total,
                                         i_EsDevolucion = D.i_EsDevolucion,
                                         i_IdTipoMotivo = D.i_IdTipoMotivo,
                                         NroPedido = A.v_NroPedido == null ? "" : A.v_NroPedido,
                                         v_IdLinea = C.v_IdLinea,
                                         IdMoneda = D.i_IdMoneda.Value,
                                         TipoCambio = D.d_TipoCambio.Value,
                                         Almacen = G.v_Nombre,
                                         ClienteProveedor = D.v_OrigenTipo == "T" ? D.v_Glosa == null || D.v_Glosa == string.Empty ? "TRANSFERENCIA " : D.v_Glosa : E == null ? "**CLIENTE/PROVEEDOR  NO EXISTE**" : (E.v_ApePaterno + " " + E.v_ApeMaterno + " " + E.v_PrimerNombre + " " + E.v_RazonSocial).Trim(),
                                         //Guia = A.v_NroGuiaRemision,
                                         Guia = string.IsNullOrEmpty(A.v_NroGuiaRemision) ? D.i_IdTipoDocumento == (int)TiposDocumentos.GuiaRemision ? D.v_SerieDocumento + "-" + D.v_CorrelativoDocumento : "" : A.v_NroGuiaRemision,
                                         Documento = H == null ? A.v_NumeroDocumento : H.v_Siglas.Trim() + " " + A.v_NumeroDocumento.Trim(),
                                         DocumentoInventarioSunat = string.IsNullOrEmpty(A.v_NumeroDocumento) ? "" : A.v_NumeroDocumento,
                                         IdAlmacen = D.i_IdAlmacenOrigen.Value,
                                         TipoMotivo = D.i_IdTipoMotivo == 5 ? 1 : D.v_OrigenTipo == "T" ? 3 : 2,
                                         IdAlmacenOrigen = D.i_IdAlmacenOrigen.Value,
                                         NroRegistro = D.v_Mes + "-" + D.v_Correlativo,
                                         i_IdTipoDocumentoDetalle = A.i_IdTipoDocumento == null ? -1 : A.i_IdTipoDocumento.Value,
                                         EsDocumentoInterno = H.i_UsadoDocumentoInterno == null ? 0 : H.i_UsadoDocumentoInterno.Value,
                                         EsDocInverso = H.i_UsadoDocumentoInverso == null ? 0 : H.i_UsadoDocumentoInverso.Value,
                                         ValorUM = I.v_Value2,
                                         v_IdMarca = C.v_IdMarca,
                                         Origen = D.v_OrigenTipo,
                                         d_Total = D.v_OrigenTipo == "I" ? pintIdMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : A.d_Total,
                                         i_IdTipoProducto = C == null ? -1 : C.i_IdTipoProducto ?? 0,
                                         CodProducto = C == null ? "" : C.v_CodInterno.Trim(),
                                         TipoExistencia = K == null ? "" : K.v_Value2,
                                         UnidadMedidaProducto = I.v_Value1,
                                         Modelo = C == null ? "" : string.IsNullOrEmpty(C.v_Modelo) ? "" : C.v_Modelo,
                                         Marca = J4 == null || string.IsNullOrEmpty(J4.v_Nombre) ? "" : J4.v_Nombre,
                                         Ubicacion = C == null || string.IsNullOrEmpty(C.v_Ubicacion) ? "" : C.v_Ubicacion,
                                         NroParte = C == null || string.IsNullOrEmpty(C.v_NroParte) ? "" : C.v_NroParte,
                                     }).ToList().AsQueryable();

                        if (!string.IsNullOrEmpty(pstrFilterExpression))
                        {
                            query = query.Where(pstrFilterExpression);
                        }
                        queryFinal = query.ToList().OrderBy(x => x.IdAlmacenOrigen).ThenBy(x => x.v_NombreProducto).ThenBy(x => x.t_Fecha).ThenBy(x => x.i_IdTipoMovimiento).ThenBy(x => x.TipoMotivo).ThenBy(x => x.NroPedido).ThenBy(o => o.v_IdMovimientoDetalle).ToList();

                        #endregion

                    }


                    if (string.IsNullOrEmpty(pstrFilterExpression)) // Cuando el formato es de la Sunat
                    {
                        var Iniciales = queryFinal.Where(l => l.i_IdTipoMotivo == (int)TipoDeMovimiento.Inicial && l.i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso).ToList().GroupBy(p => new { p.v_NombreProducto }).Select(d =>
                        {
                            var k = d.FirstOrDefault();
                            k.DescripcionProducto = Globals.ClientSession.v_RucEmpresa == Constants.RucWortec ? k.DescripcionProducto + " ," + "PEDIDO :" + k.NroPedido : k.DescripcionProducto;
                            k.d_Cantidad = d.Sum(h => h.d_Cantidad);
                            k.d_Precio = d.Sum(l => l.d_Precio);
                            k.d_Total = d.Sum(l => l.d_Total);
                            return k;
                        }).AsQueryable().ToList(); ;
                        var Otros = queryFinal.Where(l => l.i_IdTipoMotivo != (int)TipoDeMovimiento.Inicial).ToList();
                        queryFinal = Iniciales.Concat(Otros).ToList();
                        //queryFinal = queryFinal.ToList().OrderBy(x => x.v_NombreProducto).ThenBy(x => x.t_Fecha).ThenBy(x => x.i_IdTipoMovimiento).ThenBy(x => x.TipoMotivo).ThenBy(x => x.NroPedido).ToList();
                        queryFinal = queryFinal.ToList().OrderBy(x => x.v_NombreProducto).ThenBy(x => x.t_Fecha).ThenBy(x => x.i_IdTipoMovimiento).ThenBy(x => x.TipoMotivo).ThenBy(x => x.NroPedido).ThenBy(o => o.v_IdMovimientoDetalle).ToList();
                    }

                    queryFinal = queryFinal.ToList().Select(n => new KardexList
                    {


                        v_IdMovimientoDetalle = n.v_IdMovimientoDetalle,
                        v_IdProducto = n.v_IdProducto,
                        v_NombreProducto = n.v_NombreProducto,
                        v_NombreProductoInventarioSunat =
                      "CODIGO EXISTENCIA : " + n.CodProducto + " \n" + n.TipoeExistenciaCompleto + "\n" + "DESCRIPCIÓN : " + n.DescripcionProducto + "\n" + n.UnidadMedidaCompleto,
                        t_Fecha = n.t_Fecha,
                        i_IdTipoMovimiento = n.i_IdTipoMovimiento,
                        d_Cantidad = n.d_Cantidad == 0 || decimal.Parse(n.ValorUM) == 0 ? 0 : FormatoCant == (int)FormatoCantidad.UnidadMedidaProducto ? n.d_Cantidad / decimal.Parse(n.ValorUM) : n.d_Cantidad,
                        d_Precio = n.d_Cantidad == 0 || decimal.Parse(n.ValorUM) == 0 || n.d_Precio == 0 ? 0 : FormatoCant == (int)FormatoCantidad.UnidadMedidaProducto ? n.d_Cantidad == 0 ? 0 : n.d_Precio / (n.d_Cantidad / (decimal.Parse(n.ValorUM))) : n.d_Cantidad == 0 ? 0 : n.d_Precio / n.d_Cantidad,
                        i_EsDevolucion = n.i_EsDevolucion,
                        i_IdTipoMotivo = n.i_IdTipoMotivo,
                        NroPedido = n.NroPedido,
                        v_IdLinea = n.v_IdLinea,
                        IdMoneda = n.IdMoneda,
                        TipoCambio = n.TipoCambio,
                        ClienteProveedor = n.ClienteProveedor,
                        Guia = n.Guia,
                        Documento = n.Documento,
                        TipoMotivo = n.TipoMotivo,
                        NroRegistro = n.NroRegistro,
                        i_IdTipoDocumentoDetalle = n.i_IdTipoDocumentoDetalle,
                        EsDocumentoInterno = n.EsDocumentoInterno,
                        EsDocInverso = n.EsDocInverso,
                        Origen = n.Origen,
                        d_Total = n.d_Total,
                        Almacen = string.IsNullOrEmpty(pstrFilterExpression) ? "" : n.Almacen.ToUpper(),
                        IdAlmacen = string.IsNullOrEmpty(pstrFilterExpression) ? -1 : n.IdAlmacen,
                        IdAlmacenOrigen = string.IsNullOrEmpty(pstrFilterExpression) ? -1 : n.IdAlmacenOrigen,
                        DocumentoInventarioSunat = n.i_IdTipoDocumentoDetalle.ToString("00") + "-" + n.DocumentoInventarioSunat,
                        //CorrelativoPle = n.CorrelativoPle,
                        TipoExistencia = n.TipoExistencia,
                        SoloNumeroDocumentoDetalle = n.SoloNumeroDocumentoDetalle,
                        TipoOperacionInventarioSunat = n.TipoOperacionInventarioSunat,
                        CodProducto = n.CodProducto,
                        DescripcionProducto = n.DescripcionProducto,
                        UnidadMedida = n.UnidadMedida,
                        UnidadMedidaCompleto = n.UnidadMedidaCompleto,
                        UnidadMedidaProducto = n.UnidadMedidaProducto,
                        GrupoLlave = n.GrupoLlave,
                        UnidadMedidaCodigoSunat = n.UnidadMedida,
                        Marca = n.Marca,
                        Modelo = n.Modelo,
                        Ubicacion = n.Ubicacion,
                        NroParte = n.NroParte,


                    }).ToList();


                    int Contador = queryFinal.Count();
                    int NumeroElmento = 0;
                    int PosicionProducto = 0;
                    decimal UltimoPrecioSaldo = 0;
                    for (int i = 0; i < Contador; i++)
                    {
                        if (ConsideraDocInternos == 1 || (ConsideraDocInternos == 0 && queryFinal[i].EsDocumentoInterno == 0))
                        {
                            oKardexList = new KardexList();
                            oKardexList = queryFinal[i];
                            oKardexList.v_NombreProductoAuxiliar = queryFinal[i].v_NombreProducto + " - MODELO : " + queryFinal[i].Modelo + " - UBICACIÓN : " + queryFinal[i].Ubicacion + " - NRO. PARTE : " + queryFinal[i].NroParte;
                            oKardexList.TipoOperacionInventarioSunat = string.IsNullOrEmpty(queryFinal[i].TipoOperacionInventarioSunat) ? "" : int.Parse(queryFinal[i].TipoOperacionInventarioSunat).ToString("00"); //!string.IsNullOrEmpty ( TipoOperacion) ?  int.Parse ( TipoOperacion).ToString ("00")  : "00 NO EXISTE CÓDIGO ";
                            oKardexList.UnidadMedidaCodigoSunat = queryFinal[i].UnidadMedida;
                            oKardexList.UnidadMedida = FormatoCant == (int)FormatoCantidad.Unidades ? "UNIDAD" : queryFinal.Where(x => x.CodProducto == oKardexList.CodProducto && x.UnidadMedidaProducto == "UNIDAD").Count() != 0 ? "UNIDAD" : queryFinal[i].UnidadMedidaProducto;
                            oKardexList.v_Fecha = queryFinal[i].t_Fecha.Value.ToString("dd-MMM");
                            oKardexList.v_NombreTipoMovimiento = queryFinal[i].i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso && queryFinal[i].i_IdTipoMotivo == 5 ? "INICIAL" + " " + queryFinal[i].NroRegistro : queryFinal[i].i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso && queryFinal[i].i_IdTipoMotivo != 5 ? "N/I" + " " + queryFinal[i].NroRegistro : "N/S" + " " + queryFinal[i].NroRegistro;
                            oKardexList.Empresa = "";
                            oKardexList.Ruc = "";
                            oKardexList.Moneda = "MONEDA REPORTE : " + pstrMoneda.ToUpper();
                            oKardexList.Almacen = queryFinal[i].Almacen.ToUpper();
                            oKardexList.Mes = "";
                            oKardexList.Al = "";
                            //oKardexList.v_NombreProductoInventarioSunat = queryFinal[i].v_NombreProductoInventarioSunat;
                            //oKardexList.Marca = queryFinal[i].Marca;
                            //oKardexList.Modelo = queryFinal[i].Modelo;
                            //oKardexList.Ubicacion = queryFinal[i].Ubicacion;
                            //oKardexList.TipoMotivo = queryFinal[i].TipoMotivo;
                            //oKardexList.v_IdLinea = queryFinal[i].v_IdLinea;
                            //oKardexList.v_IdProducto = queryFinal[i].v_IdProducto;
                            //oKardexList.NroPedido = queryFinal[i].NroPedido;
                            //oKardexList.CorrelativoPle = queryFinal[i].CorrelativoPle;
                            //oKardexList.CodProducto = queryFinal[i].CodProducto;
                            //oKardexList.DescripcionProducto = queryFinal[i].DescripcionProducto;
                            //oKardexList.SoloNumeroDocumentoDetalle = queryFinal[i].SoloNumeroDocumentoDetalle;
                            //oKardexList.i_IdTipoDocumentoDetalle = queryFinal[i].i_IdTipoDocumentoDetalle;
                            //oKardexList.TipoExistencia = queryFinal[i].TipoExistencia;
                            //oKardexList.v_NombreProducto = queryFinal[i].v_NombreProducto;
                            //oKardexList.t_Fecha = queryFinal[i].t_Fecha;
                            //oKardexList.i_IdTipoMotivo = queryFinal[i].i_IdTipoMotivo;
                            //oKardexList.Documento = queryFinal[i].Documento;
                            //oKardexList.IdMoneda = queryFinal[i].IdMoneda;
                            //oKardexList.TipoCambio = queryFinal[i].TipoCambio;
                            //oKardexList.i_EsDevolucion = queryFinal[i].i_EsDevolucion;
                            //oKardexList.ClienteProveedor = queryFinal[i].ClienteProveedor;

                            //oKardexList.IdAlmacen = queryFinal[i].IdAlmacen;
                            //oKardexList.Guia = queryFinal[i].Guia;
                            //oKardexList.Documento = queryFinal[i].Documento;
                            //oKardexList.DocumentoInventarioSunat = queryFinal[i].DocumentoInventarioSunat;
                            //oKardexList.i_IdTipoMovimiento = queryFinal[i].i_IdTipoMovimiento;

                            if (LibroInventario && pstrGrupoLlave == "LINEA")
                            {
                                linea l = LineaDictionary.TryGetValue(oKardexList.v_IdLinea, out l) ? l : new linea();
                                oKardexList.GrupoLlave = l != null ? l.v_Nombre : "";
                            }


                            if (i == 0)
                            {


                                if ((oKardexList.i_IdTipoMotivo != 5 && oKardexList.i_IdTipoMovimiento == 1) || oKardexList.i_IdTipoMovimiento == 2)
                                {
                                    KardexList oKardexList1 = new KardexList();
                                    oKardexList1 = (KardexList)oKardexList.Clone();
                                    oKardexList1.v_NombreTipoMovimiento = "INICIAL";
                                    DateTime date = DateTime.Parse("01/" + pdtFechaInicio.Value.Month.ToString("00") + "/" + pdtFechaInicio.Value.Year.ToString());
                                    oKardexList1.t_Fecha = date;

                                    oKardexList1.DocumentoInventarioSunat = "";
                                    oKardexList1.v_Fecha = date.ToString("dd-MMM");
                                    oKardexList1.DocumentoInventarioSunat = "";
                                    oKardexList1.Guia = "";
                                    oKardexList1.Documento = "";
                                    oKardexList1.ClienteProveedor = "";
                                    oKardexList1.Ingreso_Cantidad = 0;
                                    oKardexList1.Ingreso_Precio = 0;
                                    oKardexList1.Ingreso_Total = 0;
                                    oKardexList1.Salida_Cantidad = null;
                                    oKardexList1.i_IdTipoMotivo = 5;
                                    oKardexList1.Salida_Precio = null;
                                    oKardexList1.Salida_Total = null;
                                    oKardexList1.Saldo_Cantidad = 0;
                                    oKardexList1.Saldo_Precio = 0;
                                    oKardexList1.Saldo_Total = 0;
                                    oKardexList1.NumeroElemento = 0;
                                    oKardexList1.i_IdTipoMovimiento = 1;
                                    oKardexList1.IngresoCantidadCalculo = oKardexList1.Ingreso_Cantidad == null ? 0 : oKardexList1.Ingreso_Cantidad;
                                    oKardexList1.Salida_CantidadCalculos = oKardexList1.Salida_Cantidad == null ? 0 : oKardexList1.Salida_Cantidad;
                                    oKardexList1.IngresoTotalCalculo = oKardexList1.Ingreso_Total == null ? 0 : oKardexList1.Ingreso_Total;
                                    oKardexList1.SalidaTotalCalculo = oKardexList1.Salida_Total == null ? 0 : oKardexList1.Salida_Total;
                                    oKardexList1.TipoOperacionInventarioSunat = int.Parse(TipoMovimientos.Where(l => l.i_ItemId == 5).FirstOrDefault().v_Value2).ToString("00");
                                    Lista.Add(oKardexList1);
                                    PosicionProducto = PosicionProducto + 1;
                                    IdProductoOld = oKardexList1.v_NombreProducto + " " + oKardexList1.IdAlmacen;// Se cambio 7 marzo 2017 , porque ahora el nombre producto tiene marca modelo


                                }

                            }

                            CantidadValorizadas objProceso = new CantidadValorizadas();
                            objProceso = Globals.ClientSession.i_Periodo == 2016 ? ProcesoValorizacionProductoAlmacenPedido2016(ref  pobjOperationResult, queryFinal[i].i_IdTipoMovimiento.Value, IdProductoOld, queryFinal[i].v_NombreProducto, queryFinal[i].IdAlmacen.ToString(), pintIdMoneda, queryFinal[i].Origen, queryFinal[i].i_EsDevolucion ?? 0, queryFinal[i].d_Cantidad.Value, queryFinal[i].IdMoneda, queryFinal[i].d_Precio ?? 0, PosicionProducto, queryFinal[i].TipoCambio, ref UltimoPrecioSaldo, NumeroElmento, Lista, queryFinal[i].d_Total ?? 0) : ProcesoValorizacionProductoAlmacenPedido2017(ref  pobjOperationResult, queryFinal[i].i_IdTipoMovimiento.Value, IdProductoOld, queryFinal[i].v_NombreProducto, queryFinal[i].IdAlmacen.ToString(), pintIdMoneda, queryFinal[i].Origen, queryFinal[i].i_EsDevolucion ?? 0, queryFinal[i].d_Cantidad.Value, queryFinal[i].IdMoneda, queryFinal[i].d_Precio ?? 0, PosicionProducto, queryFinal[i].TipoCambio, ref UltimoPrecioSaldo, NumeroElmento, Lista, queryFinal[i].d_Total ?? 0);

                            oKardexList.Ingreso_Cantidad = objProceso.Ingreso_Cantidad;
                            oKardexList.Ingreso_Precio = objProceso.Ingreso_Precio;
                            oKardexList.Ingreso_Total = objProceso.Ingreso_Total;
                            oKardexList.Saldo_Cantidad = objProceso.Saldo_Cantidad;
                            oKardexList.Saldo_Precio = objProceso.Saldo_Precio;
                            oKardexList.Saldo_Total = objProceso.Saldo_Total;
                            oKardexList.Salida_Cantidad = objProceso.Salida_Cantidad;
                            oKardexList.Salida_Precio = objProceso.Salida_Precio;
                            oKardexList.Salida_Total = objProceso.Salida_Total;
                            oKardexList.Ingreso_CantidadInicial = queryFinal[i].i_IdTipoMotivo == 5 ? null : oKardexList.Ingreso_Cantidad;
                            oKardexList.Ingreso_PrecioInicial = queryFinal[i].i_IdTipoMotivo == 5 ? null : oKardexList.Ingreso_Precio;
                            oKardexList.Ingreso_TotalInicial = queryFinal[i].i_IdTipoMotivo == 5 ? null : oKardexList.Ingreso_Total;


                            oKardexList.IngresoCantidadCalculo = oKardexList.Ingreso_Cantidad == null ? 0 : oKardexList.Ingreso_Cantidad;
                            oKardexList.Salida_CantidadCalculos = oKardexList.Salida_Cantidad == null ? 0 : oKardexList.Salida_Cantidad;
                            oKardexList.IngresoTotalCalculo = oKardexList.Ingreso_Total == null ? 0 : oKardexList.Ingreso_Total;
                            oKardexList.SalidaTotalCalculo = oKardexList.Salida_Total == null ? 0 : oKardexList.Salida_Total;


                            oKardexList.NumeroElemento = objProceso.NumeroElemento;
                            IdProductoOld = queryFinal[i].v_NombreProducto + " " + queryFinal[i].IdAlmacen;

                            if (i + 1 < Contador && IdProductoOld == queryFinal[i + 1].v_NombreProducto + " " + queryFinal[i + 1].IdAlmacen)
                            {
                                ///Se agrego para valorizacion sunat
                                int Reg = Lista.Count();
                                //Se agrego para los saldos iniciales 
                                if (PosicionProducto == 0 && Reg > 0 && Lista[Reg - 1].v_NombreProducto + " " + Lista[Reg - 1].IdAlmacen != oKardexList.v_NombreProducto + " " + oKardexList.IdAlmacen)
                                {
                                    if (int.Parse(pdtFechaInicio.Value.Month.ToString()) > 1)
                                    {
                                        KardexList oKardexList1 = new KardexList();
                                        oKardexList1 = (KardexList)queryFinal[i + 1].Clone();

                                        if (oKardexList1.CodProducto == "130005875")
                                        {
                                            string h = "";
                                        }
                                        oKardexList1.v_NombreTipoMovimiento = "INICIAL";
                                        DateTime date = DateTime.Parse("01/" + pdtFechaInicio.Value.Month.ToString("00") + "/" + pdtFechaInicio.Value.Year.ToString());
                                        oKardexList1.t_Fecha = date;
                                        oKardexList1.DocumentoInventarioSunat = "";
                                        oKardexList1.v_Fecha = date.ToString("dd-MMM");
                                        oKardexList1.DocumentoInventarioSunat = "";
                                        oKardexList1.Guia = "";
                                        oKardexList1.Documento = "";
                                        oKardexList1.ClienteProveedor = "";
                                        oKardexList1.Ingreso_Cantidad = 0;
                                        oKardexList1.Ingreso_Precio = 0;
                                        oKardexList1.Ingreso_Total = 0;
                                        oKardexList1.Salida_Cantidad = null;
                                        oKardexList1.i_IdTipoMotivo = 5;
                                        oKardexList1.Salida_Precio = null;
                                        oKardexList1.Salida_Total = null;
                                        oKardexList1.Saldo_Cantidad = 0;
                                        oKardexList1.Saldo_Precio = 0;
                                        oKardexList1.Saldo_Total = 0;
                                        oKardexList1.NumeroElemento = 0;
                                        oKardexList1.IngresoCantidadCalculo = oKardexList1.Ingreso_Cantidad == null ? 0 : oKardexList1.Ingreso_Cantidad;
                                        oKardexList1.Salida_CantidadCalculos = oKardexList1.Salida_Cantidad == null ? 0 : oKardexList1.Salida_Cantidad;
                                        oKardexList1.IngresoTotalCalculo = oKardexList1.Ingreso_Total == null ? 0 : oKardexList1.Ingreso_Total;
                                        oKardexList1.SalidaTotalCalculo = oKardexList1.Salida_Total == null ? 0 : oKardexList1.Salida_Total;
                                        oKardexList1.TipoOperacionInventarioSunat = int.Parse(TipoMovimientos.Where(l => l.i_ItemId == 5).FirstOrDefault().v_Value2).ToString("00");
                                        oKardexList1.i_IdTipoMovimiento = 1;
                                        oKardexList1.v_NombreProductoAuxiliar = queryFinal[i + 1].v_NombreProducto + " - MODELO : " + queryFinal[i + 1].Modelo + " - UBICACIÓN : " + queryFinal[i + 1].Ubicacion + " - NRO. PARTE : " + queryFinal[i + 1].NroParte;
                                        Lista.Add(oKardexList1);
                                        PosicionProducto = PosicionProducto + 1;
                                        IdProductoOld = oKardexList1.v_NombreProducto + " " + oKardexList1.IdAlmacen;
                                    }

                                }

                                // Hasta Acá;

                                PosicionProducto = PosicionProducto + 1;
                                Lista.Add(oKardexList);

                            }
                            else
                            {
                                PosicionProducto = 0;
                                UltimoPrecioSaldo = 0;
                                Lista.Add(oKardexList);
                                if (i + 1 < Contador)// !SeInsertoLista
                                {
                                    KardexList oKardexList1 = new KardexList();
                                    oKardexList1 = (KardexList)queryFinal[i + 1].Clone();
                                    //var ExisteListaReporte = Lista.Where(l => l.v_IdProducto + " " + l.IdAlmacen == oKardexList1.v_IdProducto + " " + oKardexList.IdAlmacen && l.NroPedido == oKardexList1.NroPedido).ToList();
                                    if (((queryFinal[i + 1].i_IdTipoMotivo != 5 && queryFinal[i + 1].i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso)) || queryFinal[i + 1].i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeSalida)
                                    {
                                        oKardexList1.v_NombreTipoMovimiento = "INICIAL";
                                        DateTime date = DateTime.Parse("01/" + pdtFechaInicio.Value.Month.ToString("00") + "/" + pdtFechaInicio.Value.Year.ToString());
                                        //DateTime date = DateTime.Parse("01/" + FechaInicioReport.Month.ToString("00") + "/" + FechaInicioReport.Year.ToString());
                                        oKardexList1.v_Fecha = date.ToString("dd-MMM");
                                        oKardexList1.t_Fecha = date;
                                        oKardexList1.DocumentoInventarioSunat = "";
                                        oKardexList1.Guia = "";
                                        oKardexList1.Documento = "";
                                        oKardexList1.ClienteProveedor = "";
                                        oKardexList1.Ingreso_Cantidad = 0;
                                        oKardexList1.Ingreso_Precio = 0;
                                        oKardexList1.Ingreso_Total = 0;
                                        oKardexList1.Salida_Cantidad = null;
                                        oKardexList1.i_IdTipoMotivo = 5;
                                        oKardexList1.Salida_Precio = null;
                                        oKardexList1.Salida_Total = null;
                                        oKardexList1.Saldo_Cantidad = 0;
                                        oKardexList1.Saldo_Precio = 0;
                                        oKardexList1.Saldo_Total = 0;
                                        oKardexList1.NumeroElemento = 0;
                                        oKardexList1.IngresoCantidadCalculo = oKardexList1.Ingreso_Cantidad == null ? 0 : oKardexList1.Ingreso_Cantidad;
                                        oKardexList1.Salida_CantidadCalculos = oKardexList1.Salida_Cantidad == null ? 0 : oKardexList1.Salida_Cantidad;
                                        oKardexList1.IngresoTotalCalculo = oKardexList1.Ingreso_Total == null ? 0 : oKardexList1.Ingreso_Total;
                                        oKardexList1.SalidaTotalCalculo = oKardexList1.Salida_Total == null ? 0 : oKardexList1.Salida_Total;
                                        oKardexList1.TipoOperacionInventarioSunat = int.Parse(TipoMovimientos.Where(l => l.i_ItemId == 5).FirstOrDefault().v_Value2).ToString("00");
                                        oKardexList1.i_IdTipoMovimiento = 1;
                                        oKardexList1.v_NombreProductoAuxiliar = queryFinal[i + 1].v_NombreProducto + " - MODELO : " + queryFinal[i + 1].Modelo + " - UBICACIÓN : " + queryFinal[i + 1].Ubicacion + " - NRO. PARTE : " + queryFinal[i + 1].NroParte;
                                        Lista.Add(oKardexList1);
                                        PosicionProducto = PosicionProducto + 1;
                                        IdProductoOld = oKardexList1.v_NombreProducto + " " + oKardexList1.IdAlmacen;
                                    }
                                }
                            }

                            ContadorError = ContadorError + 1;
                        }//Cierre If Documentos Internos 
                    }
                    if (FechaInicioReport.Month.ToString("00") != "01")
                    {

                        //var MesAnterior = int.Parse(pdtFechaFin.Value.Month.ToString()) - 1;
                        var MesAnterior = int.Parse(FechaInicioReport.Month.ToString()) - 1;
                        var FechaAnt = DateTime.Parse(DateTime.DaysInMonth(Globals.ClientSession.i_Periodo.Value, MesAnterior).ToString() + "/" + MesAnterior + "/" + Globals.ClientSession.i_Periodo.ToString() + " 23:59");

                        List<KardexList> Sald = new List<KardexList>();
                        Sald = Lista.Select(item => (KardexList)item.Clone()).ToList();

                        var ListaSaldosAnt = Sald.ToList().Where(l => l.t_Fecha <= FechaAnt).GroupBy(l => new { l.v_NombreProducto, l.IdAlmacen }).ToList().Select(d =>
                        {
                            var k = d.LastOrDefault();
                            k.v_NombreTipoMovimiento = "INICIAL";
                            DateTime date = DateTime.Parse("01/" + FechaInicioReport.Month.ToString("00") + "/" + FechaInicioReport.Year.ToString()); // DateTime.Parse("01/" + pdtFechaInicio.Value.Month.ToString("00") + "/" + pdtFechaInicio.Value.Year.ToString());
                            k.v_Fecha = date.ToString("dd-MMM");
                            k.t_Fecha = date;
                            k.Guia = "";
                            k.Documento = "";
                            k.ClienteProveedor = "";
                            k.Salida_Cantidad = null;
                            k.Ingreso_Cantidad = null;
                            k.Ingreso_Precio = null;
                            k.Ingreso_Total = null;
                            k.Salida_Cantidad = null;
                            k.Salida_Precio = null;
                            k.Salida_Total = null;
                            k.i_IdTipoMotivo = 5;
                            k.TipoMotivo = 1; // se agrego porque salia distorsionado cuando mostraba solo articulos con mov
                            k.i_IdTipoMovimiento = 1;  // se agrego porque salia distorsionado cuando mostraba solo articulos con mov
                            k.i_IdTipoDocumentoDetalle = -1;// se agrego porque es saldo inicial
                            k.SoloNumeroDocumentoDetalle = "";// se agrego porque es saldo inicial
                            //k.IngresoCantidadCalculo = k.Ingreso_Cantidad == null ? 0 : k.Ingreso_Cantidad; //se cambió el 12 junio
                            k.IngresoCantidadCalculo = k.Saldo_Cantidad;
                            k.Salida_CantidadCalculos = k.Salida_Cantidad == null ? 0 : k.Salida_Cantidad;
                            // k.IngresoTotalCalculo = k.Ingreso_Total == null ? 0 : k.Ingreso_Total;
                            k.IngresoTotalCalculo = k.Saldo_Total ?? 0;
                            k.SalidaTotalCalculo = k.Salida_Total == null ? 0 : k.Salida_Total;
                            // k.Ingreso_CantidadInicial = k.IngresoCantidadCalculo;//se cambió el 12 junio
                            k.Ingreso_CantidadInicial = k.Ingreso_Cantidad;
                            k.DocumentoInventarioSunat = "";
                            k.TipoOperacionInventarioSunat = "16";
                            k.UnidadMedidaCodigoSunat = k.UnidadMedidaCodigoSunat;
                            k.v_NombreProductoAuxiliar = k.v_NombreProductoAuxiliar;
                            return k;
                        }).ToList();

                        var jj = Lista.Where(l => l.v_IdProducto == "N001-PD000000527").ToList();
                        var Lista2 = Lista.Where(l => l.t_Fecha > FechaAnt).ToList();
                        Lista = new List<KardexList>();
                        Lista = ListaSaldosAnt.Concat(Lista2).ToList();
                    }
                    List<KardexList> ListaReporte = new List<KardexList>();
                    if (SoloArticulosMovimiento == 1)
                    {
                        var UltimoCadaGrupo = Lista.GroupBy(x => new { x.Almacen, x.v_NombreProducto }).ToList().Select(y => y.LastOrDefault());
                        foreach (var item in UltimoCadaGrupo)
                        {
                            if (!item.v_NombreTipoMovimiento.Contains("INICIAL"))
                            {

                                ListaUltimosRegistros.Add(item);
                            }
                        }

                        foreach (var item in ListaUltimosRegistros)
                        {
                            var Registros = Lista.Where(x => x.v_NombreProducto == item.v_NombreProducto && x.Almacen == item.Almacen).ToList();
                            {
                                foreach (var item1 in Registros)
                                {
                                    ListaReporte.Add(item1);
                                }
                            }

                        }
                        ListaReporte = ListaReporte.ToList().OrderBy(x => x.IdAlmacen).ThenBy(x => x.v_NombreProducto).ThenBy(x => x.t_Fecha).ThenBy(x => x.i_IdTipoMovimiento).ThenBy(x => x.TipoMotivo).ThenBy(x => x.NroPedido).ToList();

                        return ListaReporte;
                    }
                    else
                    {
                        if (ArticulosStockMayor0 == 1)
                        {
                            var UltimoCadaGrupo = Lista.GroupBy(x => new { x.Almacen, x.v_NombreProducto }).ToList().Select(y => y.LastOrDefault()).Where(l => l.Saldo_Cantidad > 0).ToList();
                            foreach (var itemUltimo in UltimoCadaGrupo)
                            {
                                var detalles = Lista.Where(l => l.Almacen == itemUltimo.Almacen && l.v_NombreProducto == itemUltimo.v_NombreProducto).ToList();
                                foreach (var itemDetalles in detalles)
                                {
                                    ListaReporte.Add(itemDetalles);
                                }
                            }

                            return ListaReporte;

                        }
                        else if (ArticuloStockCero == 1)
                        {

                            var UltimoCadaGrupo = Lista.GroupBy(x => new { x.Almacen, x.v_NombreProducto }).ToList().Select(y => y.LastOrDefault()).Where(l => l.Saldo_Cantidad == 0).ToList();
                            foreach (var itemUltimo in UltimoCadaGrupo)
                            {
                                var detalles = Lista.Where(l => l.Almacen == itemUltimo.Almacen && l.v_NombreProducto == itemUltimo.v_NombreProducto).ToList();
                                foreach (var itemDetalles in detalles)
                                {
                                    ListaReporte.Add(itemDetalles);
                                }
                            }
                            return ListaReporte;
                        }

                        else if (ArticuloStockNegativo == 1)
                        {
                            var UltimoCadaGrupo = Lista.GroupBy(x => new { x.Almacen, x.v_NombreProducto }).ToList().Select(y => y.LastOrDefault()).Where(l => l.Saldo_Cantidad < 0).ToList();
                            foreach (var itemUltimo in UltimoCadaGrupo)
                            {
                                var detalles = Lista.Where(l => l.Almacen == itemUltimo.Almacen && l.v_NombreProducto == itemUltimo.v_NombreProducto).ToList();
                                foreach (var itemDetalles in detalles)
                                {
                                    ListaReporte.Add(itemDetalles);
                                }
                            }
                            return ListaReporte;
                        }
                        else
                        {
                            if (!LibroInventario)
                            {
                                return Lista;
                            }
                            else
                            {

                                if (OrdenLibroInventario == "CÓDIGO PRODUCTO")
                                {
                                    Lista = Lista.GroupBy(x => new { x.v_IdProducto, x.NroPedido, x.Almacen })
                                                .Select(group => group.Last())
                                                .OrderBy(o => o.CodProducto).ToList();
                                }
                                else
                                {
                                    Lista = Lista.GroupBy(x => new { x.v_IdProducto, x.NroPedido, x.Almacen })
                                                  .Select(group => group.Last())
                                                  .OrderBy(o => o.v_NombreProducto).ToList();
                                }
                                return Lista;

                            }
                        }


                    }
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ProductoBL.ReporteKardex(), ,Producto :" + IdProductoOld + "Contador Error " + ContadorError;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }


        }
        public CantidadValorizadas ProcesoValorizacionProductoAlmacenPedido2017(ref  OperationResult objOperationResult, int TipoMovimiento, string IdProductoOld, string v_NombreProducto, string IdAlmacen, int pintIdMoneda, string Origen, int i_EsDevolucion, decimal d_Cantidad, int IdMoneda, decimal d_Precio, int i, decimal TipoCambio, ref decimal UltimoPrecioSaldo, int NumeroElmento, List<KardexList> Lista, decimal Total)
        {
            // Kardex Fisico y Valorizado Sunat 
            try
            {
                objOperationResult.Success = 1;
                CantidadValorizadas objCantidadValorizada = new CantidadValorizadas();
                if (pintIdMoneda == (int)Currency.Soles)
                {
                    if (TipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso)
                    {
                        #region NotaIngreso-Soles

                        objCantidadValorizada.Ingreso_Cantidad = i_EsDevolucion == 1 ? (d_Cantidad * -1) : d_Cantidad;
                        objCantidadValorizada.Ingreso_Precio = pintIdMoneda == IdMoneda ? d_Precio : Origen == "I" ? d_Precio : d_Precio * TipoCambio;
                        objCantidadValorizada.Ingreso_Total = pintIdMoneda == IdMoneda ? objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 : Total : Origen == "I" ? objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 : Total : objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 * TipoCambio : Total * TipoCambio;
                        objCantidadValorizada.Ingreso_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Ingreso_Total ?? 0, 2);

                        if (i == 0)
                        {
                            objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * -1 : d_Cantidad;
                            objCantidadValorizada.Saldo_Total = objCantidadValorizada.Ingreso_Total;
                            objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Ingreso_Precio;

                            if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                            {
                                UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio ?? 0;
                            }

                            NumeroElmento = 0;

                        }
                        else
                        {
                            if (IdProductoOld != v_NombreProducto + " " + IdAlmacen)
                            {
                                objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * -1 : d_Cantidad;

                                objCantidadValorizada.Saldo_Precio = pintIdMoneda == IdMoneda ? d_Precio : Origen == "I" ? d_Precio : d_Precio * TipoCambio;
                                objCantidadValorizada.Saldo_Total = (objCantidadValorizada.Saldo_Cantidad * objCantidadValorizada.Saldo_Precio);
                                objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);

                                UltimoPrecioSaldo = (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0) ? objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value : 0;
                                NumeroElmento = 0;
                            }
                            else
                            {

                                int Posicion = Lista.Count(); //Se agrego cuando se edito para Considerar DocumentosInternos
                                objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value + objCantidadValorizada.Ingreso_Cantidad; //Se agrego cuando se edito para Considerar DocumentosInternos
                                objCantidadValorizada.Saldo_Total = (Lista[Posicion - 1].Saldo_Total.Value + objCantidadValorizada.Ingreso_Total);//Se agrego cuando se edito para Considerar DocumentosInternos

                                objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);

                                objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Saldo_Cantidad == 0 ? 0 : (objCantidadValorizada.Saldo_Total / objCantidadValorizada.Saldo_Cantidad);
                                if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                                {
                                    UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio ?? 0;
                                }

                                NumeroElmento = NumeroElmento + 1;

                            }
                        }
                        objCantidadValorizada.NumeroElemento = NumeroElmento;
                    }

                        #endregion
                    else// Nota Salida
                    {
                        #region NotaSalidaSoles
                        objCantidadValorizada.Salida_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * -1 : d_Cantidad;

                        if (i != 0)
                        {
                            int Posicion = Lista.Count();
                            if (Lista[Posicion - 1].Saldo_Precio == 0)
                            {
                                objCantidadValorizada.Salida_Precio = UltimoPrecioSaldo;

                            }
                            else
                            {
                                objCantidadValorizada.Salida_Precio = Lista[Posicion - 1].Saldo_Precio.Value;
                            }
                        }

                        objCantidadValorizada.Salida_Total = (objCantidadValorizada.Salida_Cantidad * objCantidadValorizada.Salida_Precio);
                        objCantidadValorizada.Salida_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Salida_Total ?? 0, 2);
                        if (i == 0)
                        {

                            objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? objCantidadValorizada.Salida_Cantidad : objCantidadValorizada.Salida_Cantidad * -1;
                            objCantidadValorizada.Saldo_Total = objCantidadValorizada.Salida_Total == null ? 0 : i_EsDevolucion == 1 ? objCantidadValorizada.Salida_Total : objCantidadValorizada.Salida_Total * -1;
                            objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                            objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Salida_Precio == null ? 0 : objCantidadValorizada.Salida_Precio;
                            if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                            {

                                UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio ?? 0;
                            }



                        }
                        else
                        {
                            if (IdProductoOld != v_NombreProducto + " " + IdAlmacen)
                            {

                                objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad : objCantidadValorizada.Saldo_Cantidad = d_Cantidad * -1; // cambié 18 Abril
                                objCantidadValorizada.Saldo_Precio = pintIdMoneda == IdMoneda ? d_Precio : d_Precio * TipoCambio;
                                objCantidadValorizada.Saldo_Total = objCantidadValorizada.Saldo_Cantidad * objCantidadValorizada.Saldo_Precio;
                                objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                UltimoPrecioSaldo = (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0) ? objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value : 0;


                            }
                            else
                            {
                                int Posicion = Lista.Count();
                                if (i_EsDevolucion == 1)
                                {

                                    objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value - (d_Cantidad * -1);
                                }
                                else
                                {

                                    objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value - d_Cantidad;
                                }
                                objCantidadValorizada.Saldo_Total = (Lista[Posicion - 1].Saldo_Total.Value - objCantidadValorizada.Salida_Total);
                                objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Saldo_Cantidad == 0 ? 0 : (objCantidadValorizada.Saldo_Total / objCantidadValorizada.Saldo_Cantidad);
                                if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                                {

                                    UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio ?? 0;
                                }
                            }
                        }

                        objCantidadValorizada.NumeroElemento = NumeroElmento;
                        #endregion
                    }
                }
                else
                {
                    if (TipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso)
                    {
                        #region NotaIngresoDolares
                        objCantidadValorizada.Ingreso_Cantidad = i_EsDevolucion == 1 ? (d_Cantidad * -1) : d_Cantidad;
                        objCantidadValorizada.Ingreso_Precio = pintIdMoneda == IdMoneda ? d_Precio : Origen == "I" ? d_Precio : d_Precio / TipoCambio;
                        objCantidadValorizada.Ingreso_Total = pintIdMoneda == IdMoneda ? objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 : Total : Origen == "I" ? objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 : Total : objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? (Total * -1) / TipoCambio : Total / TipoCambio;
                        objCantidadValorizada.Ingreso_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Ingreso_Total ?? 0, 2);
                        if (i == 0)
                        {
                            objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * 1 : d_Cantidad;
                            objCantidadValorizada.Saldo_Total = objCantidadValorizada.Ingreso_Total;
                            objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Ingreso_Precio;
                            if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                            {

                                UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio ?? 0;
                            }

                            NumeroElmento = 0;
                        }
                        else
                        {
                            if (IdProductoOld != v_NombreProducto + " " + IdAlmacen)
                            {
                                objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * -1 : d_Cantidad;

                                objCantidadValorizada.Saldo_Precio = pintIdMoneda == IdMoneda ? d_Precio : Origen == "I" ? d_Precio : d_Precio / TipoCambio;
                                objCantidadValorizada.Saldo_Total = (objCantidadValorizada.Saldo_Cantidad * objCantidadValorizada.Saldo_Precio);
                                objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                UltimoPrecioSaldo = (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0) ? objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value : 0;
                                NumeroElmento = 0;
                            }
                            else
                            {

                                int Posicion = Lista.Count();
                                objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value + objCantidadValorizada.Ingreso_Cantidad;
                                objCantidadValorizada.Saldo_Total = (Lista[Posicion - 1].Saldo_Total.Value + objCantidadValorizada.Ingreso_Total);
                                objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Saldo_Cantidad == 0 ? 0 : (objCantidadValorizada.Saldo_Total / objCantidadValorizada.Saldo_Cantidad);
                                if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                                {

                                    UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio ?? 0;
                                }

                                NumeroElmento = NumeroElmento + 1;
                            }

                        }
                        objCantidadValorizada.NumeroElemento = NumeroElmento;
                        #endregion
                    }
                    else
                    {
                        #region NotaSalidaDolares

                        if (i_EsDevolucion == 1)
                        {
                            objCantidadValorizada.Salida_Cantidad = d_Cantidad * -1;
                        }
                        else
                        {
                            objCantidadValorizada.Salida_Cantidad = d_Cantidad;
                        }
                        //int Posicion = Lista.Count(); //5 de abril
                        if (i != 0)
                        {

                            int Posicion = Lista.Count(); //antes del 5 de abril

                            if (Lista[Posicion - 1].Saldo_Precio == 0)
                            {
                                objCantidadValorizada.Salida_Precio = UltimoPrecioSaldo;

                            }
                            else
                            {
                                objCantidadValorizada.Salida_Precio = Lista[Posicion - 1].Saldo_Precio.Value;
                            }
                        }

                        objCantidadValorizada.Salida_Total = (objCantidadValorizada.Salida_Precio * objCantidadValorizada.Salida_Cantidad);
                        objCantidadValorizada.Salida_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Salida_Total ?? 0, 2);

                        if (i == 0)
                        {
                            if (i_EsDevolucion == 1)
                            {
                                objCantidadValorizada.Saldo_Cantidad = d_Cantidad;
                            }
                            else
                            {
                                objCantidadValorizada.Saldo_Cantidad = d_Cantidad * -1;
                            }

                            objCantidadValorizada.Saldo_Total = objCantidadValorizada.Salida_Total == null ? 0 : objCantidadValorizada.Salida_Total;
                            objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                            objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Salida_Precio == null ? 0 : objCantidadValorizada.Salida_Precio;
                            if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                            {
                                UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value;
                            }


                        }
                        else
                        {
                            if (IdProductoOld != v_NombreProducto + " " + IdAlmacen)
                            {
                                if (i_EsDevolucion == 1)
                                {
                                    objCantidadValorizada.Saldo_Cantidad = d_Cantidad;
                                }
                                else
                                {
                                    objCantidadValorizada.Saldo_Cantidad = d_Cantidad * -1;
                                }

                                objCantidadValorizada.Saldo_Precio = pintIdMoneda == IdMoneda ? d_Precio : d_Precio / TipoCambio;
                                objCantidadValorizada.Saldo_Total = objCantidadValorizada.Saldo_Cantidad * objCantidadValorizada.Saldo_Precio;
                                objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                UltimoPrecioSaldo = (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0) ? objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value : 0;

                            }
                            else
                            {
                                int Posicion = Lista.Count();
                                if (i_EsDevolucion == 1)
                                {

                                    objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value - (d_Cantidad * -1);
                                }
                                else
                                {

                                    objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value - d_Cantidad;
                                }
                                objCantidadValorizada.Saldo_Total = (Lista[Posicion - 1].Saldo_Total.Value - objCantidadValorizada.Salida_Total);
                                objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Saldo_Cantidad == 0 ? 0 : (objCantidadValorizada.Saldo_Total / objCantidadValorizada.Saldo_Cantidad);
                                if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                                {
                                    UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value;
                                }


                            }
                        }
                        objCantidadValorizada.NumeroElemento = NumeroElmento;

                        #endregion
                    }
                }

                return objCantidadValorizada;
            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                return null;

            }

        }



        public CantidadValorizadas ProcesoValorizacionProductoAlmacenPedido2016(ref  OperationResult objOperationResult, int TipoMovimiento, string IdProductoOld, string v_NombreProducto, string IdAlmacen, int pintIdMoneda, string Origen, int i_EsDevolucion, decimal d_Cantidad, int IdMoneda, decimal d_Precio, int i, decimal TipoCambio, ref decimal UltimoPrecioSaldo, int NumeroElmento, List<KardexList> Lista, decimal Total)
        {
            // Kardex Fisico y Valorizado Sunat 
            try
            {
                objOperationResult.Success = 1;
                CantidadValorizadas objCantidadValorizada = new CantidadValorizadas();
                if (pintIdMoneda == (int)Currency.Soles)
                {
                    if (TipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso)
                    {
                        #region NotaIngreso-Soles

                        objCantidadValorizada.Ingreso_Cantidad = i_EsDevolucion == 1 ? (d_Cantidad * -1) : d_Cantidad;
                        objCantidadValorizada.Ingreso_Precio = pintIdMoneda == IdMoneda ? d_Precio : Origen == "I" ? d_Precio : d_Precio * TipoCambio;
                        objCantidadValorizada.Ingreso_Total = pintIdMoneda == IdMoneda ? objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 : Total : Origen == "I" ? objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 : Total : objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 * TipoCambio : Total * TipoCambio;
                        // objCantidadValorizada.Ingreso_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Ingreso_Total ?? 0, 2);

                        if (i == 0)
                        {
                            objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * -1 : d_Cantidad;
                            objCantidadValorizada.Saldo_Total = objCantidadValorizada.Ingreso_Total;
                            objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Ingreso_Precio;

                            if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                            {
                                UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio ?? 0;
                            }

                            NumeroElmento = 0;

                        }
                        else
                        {
                            if (IdProductoOld != v_NombreProducto + " " + IdAlmacen)
                            {
                                objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * -1 : d_Cantidad;

                                objCantidadValorizada.Saldo_Precio = pintIdMoneda == IdMoneda ? d_Precio : Origen == "I" ? d_Precio : d_Precio * TipoCambio;
                                objCantidadValorizada.Saldo_Total = (objCantidadValorizada.Saldo_Cantidad * objCantidadValorizada.Saldo_Precio);
                                //objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);

                                UltimoPrecioSaldo = (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0) ? objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value : 0;
                                NumeroElmento = 0;
                            }
                            else
                            {

                                int Posicion = Lista.Count(); //Se agrego cuando se edito para Considerar DocumentosInternos
                                objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value + objCantidadValorizada.Ingreso_Cantidad; //Se agrego cuando se edito para Considerar DocumentosInternos
                                objCantidadValorizada.Saldo_Total = (Lista[Posicion - 1].Saldo_Total.Value + objCantidadValorizada.Ingreso_Total);//Se agrego cuando se edito para Considerar DocumentosInternos

                                //objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);

                                objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Saldo_Cantidad == 0 ? 0 : (objCantidadValorizada.Saldo_Total / objCantidadValorizada.Saldo_Cantidad);
                                if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                                {
                                    UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio ?? 0;
                                }

                                NumeroElmento = NumeroElmento + 1;

                            }
                        }
                        objCantidadValorizada.NumeroElemento = NumeroElmento;
                    }

                        #endregion
                    else// Nota Salida
                    {
                        #region NotaSalidaSoles
                        objCantidadValorizada.Salida_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * -1 : d_Cantidad;

                        if (i != 0)
                        {
                            int Posicion = Lista.Count();
                            if (Lista[Posicion - 1].Saldo_Precio == 0)
                            {
                                objCantidadValorizada.Salida_Precio = UltimoPrecioSaldo;

                            }
                            else
                            {
                                objCantidadValorizada.Salida_Precio = Lista[Posicion - 1].Saldo_Precio.Value;
                            }
                        }

                        objCantidadValorizada.Salida_Total = (objCantidadValorizada.Salida_Cantidad * objCantidadValorizada.Salida_Precio);
                        // objCantidadValorizada.Salida_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Salida_Total ?? 0, 2);
                        if (i == 0)
                        {

                            objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? objCantidadValorizada.Salida_Cantidad : objCantidadValorizada.Salida_Cantidad * -1;
                            objCantidadValorizada.Saldo_Total = objCantidadValorizada.Salida_Total == null ? 0 : i_EsDevolucion == 1 ? objCantidadValorizada.Salida_Total : objCantidadValorizada.Salida_Total * -1;
                            // objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                            objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Salida_Precio == null ? 0 : objCantidadValorizada.Salida_Precio;
                            if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                            {

                                UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio ?? 0;
                            }



                        }
                        else
                        {
                            if (IdProductoOld != v_NombreProducto + " " + IdAlmacen)
                            {

                                objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad : objCantidadValorizada.Saldo_Cantidad = d_Cantidad * -1; // cambié 18 Abril
                                objCantidadValorizada.Saldo_Precio = pintIdMoneda == IdMoneda ? d_Precio : d_Precio * TipoCambio;
                                objCantidadValorizada.Saldo_Total = objCantidadValorizada.Saldo_Cantidad * objCantidadValorizada.Saldo_Precio;
                                // objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                UltimoPrecioSaldo = (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0) ? objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value : 0;


                            }
                            else
                            {
                                int Posicion = Lista.Count();
                                if (i_EsDevolucion == 1)
                                {

                                    objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value - (d_Cantidad * -1);
                                }
                                else
                                {

                                    objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value - d_Cantidad;
                                }
                                objCantidadValorizada.Saldo_Total = (Lista[Posicion - 1].Saldo_Total.Value - objCantidadValorizada.Salida_Total);
                                //objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Saldo_Cantidad == 0 ? 0 : (objCantidadValorizada.Saldo_Total / objCantidadValorizada.Saldo_Cantidad);
                                if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                                {

                                    UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio ?? 0;
                                }
                            }
                        }

                        objCantidadValorizada.NumeroElemento = NumeroElmento;
                        #endregion
                    }
                }
                else
                {
                    if (TipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso)
                    {
                        #region NotaIngresoDolares
                        objCantidadValorizada.Ingreso_Cantidad = i_EsDevolucion == 1 ? (d_Cantidad * -1) : d_Cantidad;
                        objCantidadValorizada.Ingreso_Precio = pintIdMoneda == IdMoneda ? d_Precio : Origen == "I" ? d_Precio : d_Precio / TipoCambio;
                        objCantidadValorizada.Ingreso_Total = pintIdMoneda == IdMoneda ? objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 : Total : Origen == "I" ? objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 : Total : objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? (Total * -1) / TipoCambio : Total / TipoCambio;
                        // objCantidadValorizada.Ingreso_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Ingreso_Total ?? 0, 2);
                        if (i == 0)
                        {
                            objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * 1 : d_Cantidad;
                            objCantidadValorizada.Saldo_Total = objCantidadValorizada.Ingreso_Total;
                            objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Ingreso_Precio;
                            if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                            {

                                UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio ?? 0;
                            }

                            NumeroElmento = 0;
                        }
                        else
                        {
                            if (IdProductoOld != v_NombreProducto + " " + IdAlmacen)
                            {
                                objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * -1 : d_Cantidad;

                                objCantidadValorizada.Saldo_Precio = pintIdMoneda == IdMoneda ? d_Precio : Origen == "I" ? d_Precio : d_Precio / TipoCambio;
                                objCantidadValorizada.Saldo_Total = (objCantidadValorizada.Saldo_Cantidad * objCantidadValorizada.Saldo_Precio);
                                // objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                UltimoPrecioSaldo = (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0) ? objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value : 0;
                                NumeroElmento = 0;
                            }
                            else
                            {

                                int Posicion = Lista.Count();
                                objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value + objCantidadValorizada.Ingreso_Cantidad;
                                objCantidadValorizada.Saldo_Total = (Lista[Posicion - 1].Saldo_Total.Value + objCantidadValorizada.Ingreso_Total);
                                //objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Saldo_Cantidad == 0 ? 0 : (objCantidadValorizada.Saldo_Total / objCantidadValorizada.Saldo_Cantidad);
                                if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                                {

                                    UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio ?? 0;
                                }

                                NumeroElmento = NumeroElmento + 1;
                            }

                        }
                        objCantidadValorizada.NumeroElemento = NumeroElmento;
                        #endregion
                    }
                    else
                    {
                        #region NotaSalidaDolares

                        if (i_EsDevolucion == 1)
                        {
                            objCantidadValorizada.Salida_Cantidad = d_Cantidad * -1;
                        }
                        else
                        {
                            objCantidadValorizada.Salida_Cantidad = d_Cantidad;
                        }
                        //int Posicion = Lista.Count(); //5 de abril
                        if (i != 0)
                        {

                            int Posicion = Lista.Count(); //antes del 5 de abril

                            if (Lista[Posicion - 1].Saldo_Precio == 0)
                            {
                                objCantidadValorizada.Salida_Precio = UltimoPrecioSaldo;

                            }
                            else
                            {
                                objCantidadValorizada.Salida_Precio = Lista[Posicion - 1].Saldo_Precio.Value;
                            }
                        }

                        objCantidadValorizada.Salida_Total = (objCantidadValorizada.Salida_Precio * objCantidadValorizada.Salida_Cantidad);
                        //objCantidadValorizada.Salida_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Salida_Total ?? 0, 2);

                        if (i == 0)
                        {
                            if (i_EsDevolucion == 1)
                            {
                                objCantidadValorizada.Saldo_Cantidad = d_Cantidad;
                            }
                            else
                            {
                                objCantidadValorizada.Saldo_Cantidad = d_Cantidad * -1;
                            }

                            objCantidadValorizada.Saldo_Total = objCantidadValorizada.Salida_Total == null ? 0 : objCantidadValorizada.Salida_Total;
                            // objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                            objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Salida_Precio == null ? 0 : objCantidadValorizada.Salida_Precio;
                            if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                            {
                                UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value;
                            }


                        }
                        else
                        {
                            if (IdProductoOld != v_NombreProducto + " " + IdAlmacen)
                            {
                                if (i_EsDevolucion == 1)
                                {
                                    objCantidadValorizada.Saldo_Cantidad = d_Cantidad;
                                }
                                else
                                {
                                    objCantidadValorizada.Saldo_Cantidad = d_Cantidad * -1;
                                }

                                objCantidadValorizada.Saldo_Precio = pintIdMoneda == IdMoneda ? d_Precio : d_Precio / TipoCambio;
                                objCantidadValorizada.Saldo_Total = objCantidadValorizada.Saldo_Cantidad * objCantidadValorizada.Saldo_Precio;
                                // objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                UltimoPrecioSaldo = (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0) ? objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value : 0;

                            }
                            else
                            {
                                int Posicion = Lista.Count();
                                if (i_EsDevolucion == 1)
                                {

                                    objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value - (d_Cantidad * -1);
                                }
                                else
                                {

                                    objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value - d_Cantidad;
                                }
                                objCantidadValorizada.Saldo_Total = (Lista[Posicion - 1].Saldo_Total.Value - objCantidadValorizada.Salida_Total);
                                //objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Saldo_Cantidad == 0 ? 0 : (objCantidadValorizada.Saldo_Total / objCantidadValorizada.Saldo_Cantidad);
                                if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                                {
                                    UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value;
                                }


                            }
                        }
                        objCantidadValorizada.NumeroElemento = NumeroElmento;

                        #endregion
                    }
                }

                return objCantidadValorizada;
            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                return null;

            }

        }


        public CantidadValorizadas ProcesoValorizacionProductoPedido2017(int TipoMovimiento, string IdProductoOld, string v_NombreProducto, string IdAlmacen, int pintIdMoneda, string Origen, int i_EsDevolucion, decimal d_Cantidad, int IdMoneda, decimal d_Precio, int i, decimal TipoCambio, ref  decimal UltimoPrecioSaldo, int NumeroElmento, List<KardexList> Lista, decimal Total)
        {
            // ReporteStock
            CantidadValorizadas objCantidadValorizada = new CantidadValorizadas();

            if (pintIdMoneda == (int)Currency.Soles)
            {
                if (TipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso)
                {
                    #region NotaIngreso-Soles

                    objCantidadValorizada.Ingreso_Cantidad = i_EsDevolucion == 1 ? (d_Cantidad * -1) : d_Cantidad;
                    objCantidadValorizada.Ingreso_Precio = pintIdMoneda == IdMoneda ? d_Precio : Origen == "I" ? d_Precio : d_Precio * TipoCambio;
                    objCantidadValorizada.Ingreso_Total = pintIdMoneda == IdMoneda ? objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 : Total : Origen == "I" ? objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 : Total : objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 * TipoCambio : Total * TipoCambio;
                    objCantidadValorizada.Ingreso_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Ingreso_Total ?? 0, 2);

                    if (i == 0)
                    {
                        objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * -1 : d_Cantidad;
                        objCantidadValorizada.Saldo_Total = objCantidadValorizada.Ingreso_Total;

                        objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                        objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Ingreso_Precio;

                        if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                        {
                            UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value;
                        }


                    }
                    else
                    {
                        if (IdProductoOld != v_NombreProducto)
                        {
                            objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * -1 : d_Cantidad;
                            objCantidadValorizada.Saldo_Precio = pintIdMoneda == IdMoneda ? d_Precio : Origen == "I" ? d_Precio : d_Precio * TipoCambio;
                            objCantidadValorizada.Saldo_Total = (objCantidadValorizada.Saldo_Cantidad * objCantidadValorizada.Saldo_Precio);

                            objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                            UltimoPrecioSaldo = (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0) ? objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value : 0;
                            NumeroElmento = 0;
                        }
                        else
                        {

                            int Posicion = Lista.Count(); //Se agrego cuando se edito para Considerar DocumentosInternos
                            objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value + objCantidadValorizada.Ingreso_Cantidad; //Se agrego cuando se edito para Considerar DocumentosInternos
                            objCantidadValorizada.Saldo_Total = (Lista[Posicion - 1].Saldo_Total.Value + objCantidadValorizada.Ingreso_Total);//Se agrego cuando se edito para Considerar DocumentosInternos
                            objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);

                            objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Saldo_Cantidad == 0 ? 0 : (objCantidadValorizada.Saldo_Total / objCantidadValorizada.Saldo_Cantidad);
                            if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                            {
                                UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value;
                            }


                        }
                    }
                    objCantidadValorizada.NumeroElemento = NumeroElmento;
                }

                    #endregion
                else// Nota Salida
                {
                    #region NotaSalidaDolares
                    objCantidadValorizada.Salida_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * -1 : d_Cantidad;

                    if (i != 0)
                    {
                        int Posicion = Lista.Count();
                        if (Lista[Posicion - 1].Saldo_Precio == 0)
                        {
                            objCantidadValorizada.Salida_Precio = UltimoPrecioSaldo;
                        }
                        else
                        {
                            objCantidadValorizada.Salida_Precio = Lista[Posicion - 1].Saldo_Precio.Value;
                        }
                    }

                    objCantidadValorizada.Salida_Total = (objCantidadValorizada.Salida_Cantidad * objCantidadValorizada.Salida_Precio);

                    objCantidadValorizada.Salida_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Salida_Total ?? 0, 2);
                    if (i == 0)
                    {

                        objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? objCantidadValorizada.Salida_Cantidad : objCantidadValorizada.Salida_Cantidad * -1;
                        objCantidadValorizada.Saldo_Total = objCantidadValorizada.Salida_Total == null ? 0 : i_EsDevolucion == 1 ? objCantidadValorizada.Salida_Total : objCantidadValorizada.Salida_Total * -1;
                        objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                        objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Salida_Precio == null ? 0 : objCantidadValorizada.Salida_Precio;
                        if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                        {

                            UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value;
                        }


                    }
                    else
                    {
                        if (IdProductoOld != v_NombreProducto)
                        {

                            objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad : objCantidadValorizada.Saldo_Cantidad = d_Cantidad * -1; // cambié 18 Abril
                            objCantidadValorizada.Saldo_Precio = pintIdMoneda == IdMoneda ? d_Precio : d_Precio * TipoCambio;
                            objCantidadValorizada.Saldo_Total = objCantidadValorizada.Saldo_Cantidad * objCantidadValorizada.Saldo_Precio;
                            objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                            UltimoPrecioSaldo = (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0) ? objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value : 0;


                        }
                        else
                        {
                            int Posicion = Lista.Count();
                            if (i_EsDevolucion == 1)
                            {

                                objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value - (d_Cantidad * -1);
                            }
                            else
                            {

                                objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value - d_Cantidad;
                            }
                            objCantidadValorizada.Saldo_Total = (Lista[Posicion - 1].Saldo_Total.Value - objCantidadValorizada.Salida_Total);

                            objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                            objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Saldo_Cantidad == 0 ? 0 : (objCantidadValorizada.Saldo_Total / objCantidadValorizada.Saldo_Cantidad);
                            if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                            {

                                UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value;
                            }


                        }
                    }
                    objCantidadValorizada.NumeroElemento = NumeroElmento;

                    #endregion
                }
            }
            else
            {
                if (TipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso)
                {
                    #region NotaIngresoDolares
                    objCantidadValorizada.Ingreso_Cantidad = i_EsDevolucion == 1 ? (d_Cantidad * -1) : d_Cantidad;
                    objCantidadValorizada.Ingreso_Precio = pintIdMoneda == IdMoneda ? d_Precio : Origen == "I" ? d_Precio : d_Precio / TipoCambio;
                    objCantidadValorizada.Ingreso_Total = pintIdMoneda == IdMoneda ? objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 : Total : Origen == "I" ? objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 : Total : objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? (Total * -1) / TipoCambio : Total / TipoCambio;
                    objCantidadValorizada.Ingreso_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Ingreso_Total ?? 0, 2);
                    if (i == 0)
                    {
                        objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * 1 : d_Cantidad;
                        objCantidadValorizada.Saldo_Total = objCantidadValorizada.Ingreso_Total;
                        objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Ingreso_Precio;
                        if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                        {

                            UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value;
                        }

                    }
                    else
                    {
                        if (IdProductoOld != v_NombreProducto)
                        {
                            objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * -1 : d_Cantidad;

                            objCantidadValorizada.Saldo_Precio = pintIdMoneda == IdMoneda ? d_Precio : Origen == "I" ? d_Precio : d_Precio / TipoCambio;
                            objCantidadValorizada.Saldo_Total = (objCantidadValorizada.Saldo_Cantidad * objCantidadValorizada.Saldo_Precio);
                            objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                            UltimoPrecioSaldo = (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0) ? objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value : 0;
                            NumeroElmento = 0;
                        }
                        else
                        {

                            int Posicion = Lista.Count();
                            objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value + objCantidadValorizada.Ingreso_Cantidad;
                            objCantidadValorizada.Saldo_Total = (Lista[Posicion - 1].Saldo_Total.Value + objCantidadValorizada.Ingreso_Total);
                            objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                            objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Saldo_Cantidad == 0 ? 0 : (objCantidadValorizada.Saldo_Total / objCantidadValorizada.Saldo_Cantidad);
                            if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                            {

                                UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value;
                            }
                            // NumeroElmento = NumeroElmento + 1;
                        }

                    }
                    objCantidadValorizada.NumeroElemento = NumeroElmento;
                    #endregion
                }
                else
                {
                    #region NotaSalidaDolares

                    if (i_EsDevolucion == 1)
                    {
                        objCantidadValorizada.Salida_Cantidad = d_Cantidad * -1;
                    }
                    else
                    {
                        objCantidadValorizada.Salida_Cantidad = d_Cantidad;
                    }
                    //int Posicion = Lista.Count(); //5 de abril
                    if (i != 0)
                    {

                        int Posicion = Lista.Count(); //antes del 5 de abril

                        if (Lista[Posicion - 1].Saldo_Precio == 0)
                        {
                            objCantidadValorizada.Salida_Precio = UltimoPrecioSaldo;
                        }
                        else
                        {

                            objCantidadValorizada.Salida_Precio = Lista[Posicion - 1].Saldo_Precio.Value;
                        }
                    }

                    objCantidadValorizada.Salida_Total = (objCantidadValorizada.Salida_Precio * objCantidadValorizada.Salida_Cantidad);

                    objCantidadValorizada.Salida_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Salida_Total ?? 0, 2);

                    if (i == 0)
                    {

                        if (i_EsDevolucion == 1)
                        {
                            objCantidadValorizada.Saldo_Cantidad = d_Cantidad;
                        }
                        else
                        {
                            objCantidadValorizada.Saldo_Cantidad = d_Cantidad * -1;
                        }

                        objCantidadValorizada.Saldo_Total = objCantidadValorizada.Salida_Total == null ? 0 : objCantidadValorizada.Salida_Total;
                        objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                        objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Salida_Precio == null ? 0 : objCantidadValorizada.Salida_Precio;
                        if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                        {
                            UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value;
                        }

                    }
                    else
                    {
                        if (IdProductoOld != v_NombreProducto)
                        {
                            if (i_EsDevolucion == 1)
                            {
                                objCantidadValorizada.Saldo_Cantidad = d_Cantidad;
                            }
                            else
                            {
                                objCantidadValorizada.Saldo_Cantidad = d_Cantidad * -1;
                            }

                            objCantidadValorizada.Saldo_Precio = pintIdMoneda == IdMoneda ? d_Precio : d_Precio / TipoCambio;
                            objCantidadValorizada.Saldo_Total = objCantidadValorizada.Saldo_Cantidad * objCantidadValorizada.Saldo_Precio;
                            objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                            UltimoPrecioSaldo = (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0) ? objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value : 0;

                        }
                        else
                        {
                            int Posicion = Lista.Count();
                            if (i_EsDevolucion == 1)
                            {

                                objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value - (d_Cantidad * -1);
                            }
                            else
                            {

                                objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value - d_Cantidad;
                            }


                            objCantidadValorizada.Saldo_Total = (Lista[Posicion - 1].Saldo_Total.Value - objCantidadValorizada.Salida_Total);
                            objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                            objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Saldo_Cantidad == 0 ? 0 : (objCantidadValorizada.Saldo_Total / objCantidadValorizada.Saldo_Cantidad);
                            if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                            {
                                UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value;
                            }

                        }
                    }
                    objCantidadValorizada.NumeroElemento = NumeroElmento;

                    #endregion
                }
            }

            return objCantidadValorizada;

        }




        public CantidadValorizadas ProcesoValorizacionProductoPedido2016(int TipoMovimiento, string IdProductoOld, string v_NombreProducto, string IdAlmacen, int pintIdMoneda, string Origen, int i_EsDevolucion, decimal d_Cantidad, int IdMoneda, decimal d_Precio, int i, decimal TipoCambio, ref  decimal UltimoPrecioSaldo, int NumeroElmento, List<KardexList> Lista, decimal Total)
        {
            // ReporteStock
            CantidadValorizadas objCantidadValorizada = new CantidadValorizadas();

            if (pintIdMoneda == (int)Currency.Soles)
            {
                if (TipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso)
                {
                    #region NotaIngreso-Soles

                    objCantidadValorizada.Ingreso_Cantidad = i_EsDevolucion == 1 ? (d_Cantidad * -1) : d_Cantidad;
                    objCantidadValorizada.Ingreso_Precio = pintIdMoneda == IdMoneda ? d_Precio : Origen == "I" ? d_Precio : d_Precio * TipoCambio;
                    objCantidadValorizada.Ingreso_Total = pintIdMoneda == IdMoneda ? objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 : Total : Origen == "I" ? objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 : Total : objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 * TipoCambio : Total * TipoCambio;
                    //objCantidadValorizada.Ingreso_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Ingreso_Total ?? 0, 2);

                    if (i == 0)
                    {
                        objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * -1 : d_Cantidad;
                        objCantidadValorizada.Saldo_Total = objCantidadValorizada.Ingreso_Total;

                        //objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                        objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Ingreso_Precio;

                        if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                        {
                            UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value;
                        }


                    }
                    else
                    {
                        if (IdProductoOld != v_NombreProducto)
                        {
                            objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * -1 : d_Cantidad;
                            objCantidadValorizada.Saldo_Precio = pintIdMoneda == IdMoneda ? d_Precio : Origen == "I" ? d_Precio : d_Precio * TipoCambio;
                            objCantidadValorizada.Saldo_Total = (objCantidadValorizada.Saldo_Cantidad * objCantidadValorizada.Saldo_Precio);

                            //objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                            UltimoPrecioSaldo = (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0) ? objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value : 0;
                            NumeroElmento = 0;
                        }
                        else
                        {

                            int Posicion = Lista.Count(); //Se agrego cuando se edito para Considerar DocumentosInternos
                            objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value + objCantidadValorizada.Ingreso_Cantidad; //Se agrego cuando se edito para Considerar DocumentosInternos
                            objCantidadValorizada.Saldo_Total = (Lista[Posicion - 1].Saldo_Total.Value + objCantidadValorizada.Ingreso_Total);//Se agrego cuando se edito para Considerar DocumentosInternos
                            // objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);

                            objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Saldo_Cantidad == 0 ? 0 : (objCantidadValorizada.Saldo_Total / objCantidadValorizada.Saldo_Cantidad);
                            if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                            {
                                UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value;
                            }


                        }
                    }
                    objCantidadValorizada.NumeroElemento = NumeroElmento;
                }

                    #endregion
                else// Nota Salida
                {
                    #region NotaSalidaDolares
                    objCantidadValorizada.Salida_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * -1 : d_Cantidad;

                    if (i != 0)
                    {
                        int Posicion = Lista.Count();
                        if (Lista[Posicion - 1].Saldo_Precio == 0)
                        {
                            objCantidadValorizada.Salida_Precio = UltimoPrecioSaldo;
                        }
                        else
                        {
                            objCantidadValorizada.Salida_Precio = Lista[Posicion - 1].Saldo_Precio.Value;
                        }
                    }

                    objCantidadValorizada.Salida_Total = (objCantidadValorizada.Salida_Cantidad * objCantidadValorizada.Salida_Precio);

                    // objCantidadValorizada.Salida_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Salida_Total ?? 0, 2);
                    if (i == 0)
                    {

                        objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? objCantidadValorizada.Salida_Cantidad : objCantidadValorizada.Salida_Cantidad * -1;
                        objCantidadValorizada.Saldo_Total = objCantidadValorizada.Salida_Total == null ? 0 : i_EsDevolucion == 1 ? objCantidadValorizada.Salida_Total : objCantidadValorizada.Salida_Total * -1;
                        // objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                        objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Salida_Precio == null ? 0 : objCantidadValorizada.Salida_Precio;
                        if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                        {

                            UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value;
                        }


                    }
                    else
                    {
                        if (IdProductoOld != v_NombreProducto)
                        {

                            objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad : objCantidadValorizada.Saldo_Cantidad = d_Cantidad * -1; // cambié 18 Abril
                            objCantidadValorizada.Saldo_Precio = pintIdMoneda == IdMoneda ? d_Precio : d_Precio * TipoCambio;
                            objCantidadValorizada.Saldo_Total = objCantidadValorizada.Saldo_Cantidad * objCantidadValorizada.Saldo_Precio;
                            // objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                            UltimoPrecioSaldo = (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0) ? objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value : 0;


                        }
                        else
                        {
                            int Posicion = Lista.Count();
                            if (i_EsDevolucion == 1)
                            {

                                objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value - (d_Cantidad * -1);
                            }
                            else
                            {

                                objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value - d_Cantidad;
                            }
                            objCantidadValorizada.Saldo_Total = (Lista[Posicion - 1].Saldo_Total.Value - objCantidadValorizada.Salida_Total);

                            // objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                            objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Saldo_Cantidad == 0 ? 0 : (objCantidadValorizada.Saldo_Total / objCantidadValorizada.Saldo_Cantidad);
                            if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                            {

                                UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value;
                            }


                        }
                    }
                    objCantidadValorizada.NumeroElemento = NumeroElmento;

                    #endregion
                }
            }
            else
            {
                if (TipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso)
                {
                    #region NotaIngresoDolares
                    objCantidadValorizada.Ingreso_Cantidad = i_EsDevolucion == 1 ? (d_Cantidad * -1) : d_Cantidad;
                    objCantidadValorizada.Ingreso_Precio = pintIdMoneda == IdMoneda ? d_Precio : Origen == "I" ? d_Precio : d_Precio / TipoCambio;
                    objCantidadValorizada.Ingreso_Total = pintIdMoneda == IdMoneda ? objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 : Total : Origen == "I" ? objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 : Total : objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? (Total * -1) / TipoCambio : Total / TipoCambio;
                    //objCantidadValorizada.Ingreso_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Ingreso_Total ?? 0, 2);
                    if (i == 0)
                    {
                        objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * 1 : d_Cantidad;
                        objCantidadValorizada.Saldo_Total = objCantidadValorizada.Ingreso_Total;
                        objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Ingreso_Precio;
                        if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                        {

                            UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value;
                        }

                    }
                    else
                    {
                        if (IdProductoOld != v_NombreProducto)
                        {
                            objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * -1 : d_Cantidad;

                            objCantidadValorizada.Saldo_Precio = pintIdMoneda == IdMoneda ? d_Precio : Origen == "I" ? d_Precio : d_Precio / TipoCambio;
                            objCantidadValorizada.Saldo_Total = (objCantidadValorizada.Saldo_Cantidad * objCantidadValorizada.Saldo_Precio);
                            // objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                            UltimoPrecioSaldo = (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0) ? objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value : 0;
                            NumeroElmento = 0;
                        }
                        else
                        {

                            int Posicion = Lista.Count();
                            objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value + objCantidadValorizada.Ingreso_Cantidad;
                            objCantidadValorizada.Saldo_Total = (Lista[Posicion - 1].Saldo_Total.Value + objCantidadValorizada.Ingreso_Total);
                            // objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                            objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Saldo_Cantidad == 0 ? 0 : (objCantidadValorizada.Saldo_Total / objCantidadValorizada.Saldo_Cantidad);
                            if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                            {

                                UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value;
                            }
                            // NumeroElmento = NumeroElmento + 1;
                        }

                    }
                    objCantidadValorizada.NumeroElemento = NumeroElmento;
                    #endregion
                }
                else
                {
                    #region NotaSalidaDolares

                    if (i_EsDevolucion == 1)
                    {
                        objCantidadValorizada.Salida_Cantidad = d_Cantidad * -1;
                    }
                    else
                    {
                        objCantidadValorizada.Salida_Cantidad = d_Cantidad;
                    }
                    //int Posicion = Lista.Count(); //5 de abril
                    if (i != 0)
                    {

                        int Posicion = Lista.Count(); //antes del 5 de abril

                        if (Lista[Posicion - 1].Saldo_Precio == 0)
                        {
                            objCantidadValorizada.Salida_Precio = UltimoPrecioSaldo;
                        }
                        else
                        {

                            objCantidadValorizada.Salida_Precio = Lista[Posicion - 1].Saldo_Precio.Value;
                        }
                    }

                    objCantidadValorizada.Salida_Total = (objCantidadValorizada.Salida_Precio * objCantidadValorizada.Salida_Cantidad);

                    // objCantidadValorizada.Salida_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Salida_Total ?? 0, 2);

                    if (i == 0)
                    {

                        if (i_EsDevolucion == 1)
                        {
                            objCantidadValorizada.Saldo_Cantidad = d_Cantidad;
                        }
                        else
                        {
                            objCantidadValorizada.Saldo_Cantidad = d_Cantidad * -1;
                        }

                        objCantidadValorizada.Saldo_Total = objCantidadValorizada.Salida_Total == null ? 0 : objCantidadValorizada.Salida_Total;
                        // objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                        objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Salida_Precio == null ? 0 : objCantidadValorizada.Salida_Precio;
                        if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                        {
                            UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value;
                        }

                    }
                    else
                    {
                        if (IdProductoOld != v_NombreProducto)
                        {
                            if (i_EsDevolucion == 1)
                            {
                                objCantidadValorizada.Saldo_Cantidad = d_Cantidad;
                            }
                            else
                            {
                                objCantidadValorizada.Saldo_Cantidad = d_Cantidad * -1;
                            }

                            objCantidadValorizada.Saldo_Precio = pintIdMoneda == IdMoneda ? d_Precio : d_Precio / TipoCambio;
                            objCantidadValorizada.Saldo_Total = objCantidadValorizada.Saldo_Cantidad * objCantidadValorizada.Saldo_Precio;
                            //objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                            UltimoPrecioSaldo = (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0) ? objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value : 0;

                        }
                        else
                        {
                            int Posicion = Lista.Count();
                            if (i_EsDevolucion == 1)
                            {

                                objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value - (d_Cantidad * -1);
                            }
                            else
                            {

                                objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value - d_Cantidad;
                            }


                            objCantidadValorizada.Saldo_Total = (Lista[Posicion - 1].Saldo_Total.Value - objCantidadValorizada.Salida_Total);
                            // objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                            objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Saldo_Cantidad == 0 ? 0 : (objCantidadValorizada.Saldo_Total / objCantidadValorizada.Saldo_Cantidad);
                            if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                            {
                                UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value;
                            }

                        }
                    }
                    objCantidadValorizada.NumeroElemento = NumeroElmento;

                    #endregion
                }
            }

            return objCantidadValorizada;

        }



        public CantidadValorizadas ProcesoValorizacionProductoAlmacenPedidoNotasSalida2017(ref  OperationResult objOperationResult, int TipoMovimiento, string IdProductoOld, string v_NombreProducto, string IdAlmacen, int pintIdMoneda, string Origen, int i_EsDevolucion, decimal d_Cantidad, int IdMoneda, decimal d_Precio, int i, decimal TipoCambio, ref  decimal UltimoPrecioSaldo, int NumeroElmento, List<ReporteListadoSalidaAlmacenAnalitico> Lista, decimal Total, decimal TotalDolares, decimal d_PrecioDolares, ref decimal UltimoPrecioSaldoDolares)
        {


            try
            {

                int NumElemento = NumeroElmento;
                objOperationResult.Success = 1;
                CantidadValorizadas objCantidadValorizada = new CantidadValorizadas();
                if (pintIdMoneda == (int)Currency.Soles)
                {
                    if (TipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso)
                    {
                        #region NotaIngreso-Soles

                        objCantidadValorizada.Ingreso_Cantidad = i_EsDevolucion == 1 ? (d_Cantidad * -1) : d_Cantidad;
                        objCantidadValorizada.Ingreso_Precio = pintIdMoneda == IdMoneda ? d_Precio : Origen == "I" ? d_Precio : d_Precio * TipoCambio;
                        objCantidadValorizada.Ingreso_Total = pintIdMoneda == IdMoneda ? objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 : Total : Origen == "I" ? objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 : Total : objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 * TipoCambio : Total * TipoCambio;
                        objCantidadValorizada.Ingreso_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Ingreso_Total ?? 0, 2);
                        if (i == 0)
                        {
                            objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * -1 : d_Cantidad;
                            objCantidadValorizada.Saldo_Total = objCantidadValorizada.Ingreso_Total;

                            objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                            objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Ingreso_Precio;

                            if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                            {
                                UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio ?? 0;
                            }
                            NumeroElmento = 0;

                        }
                        else
                        {
                            if (IdProductoOld != v_NombreProducto + " " + IdAlmacen)
                            {
                                objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * -1 : d_Cantidad;

                                objCantidadValorizada.Saldo_Precio = pintIdMoneda == IdMoneda ? d_Precio : Origen == "I" ? d_Precio : d_Precio * TipoCambio;
                                objCantidadValorizada.Saldo_Total = (objCantidadValorizada.Saldo_Cantidad * objCantidadValorizada.Saldo_Precio);

                                objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                UltimoPrecioSaldo = (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0) ? objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value : 0;
                                NumeroElmento = 0;
                            }
                            else
                            {

                                int Posicion = Lista.Count(); //Se agrego cuando se edito para Considerar DocumentosInternos
                                objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value + objCantidadValorizada.Ingreso_Cantidad; //Se agrego cuando se edito para Considerar DocumentosInternos
                                objCantidadValorizada.Saldo_Total = (Lista[Posicion - 1].Saldo_Total.Value + objCantidadValorizada.Ingreso_Total);//Se agrego cuando se edito para Considerar DocumentosInternos
                                objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Saldo_Cantidad == 0 ? 0 : (objCantidadValorizada.Saldo_Total / objCantidadValorizada.Saldo_Cantidad);
                                if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                                {
                                    UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio ?? 0;
                                }
                                NumeroElmento = NumeroElmento + 1;

                            }
                        }
                        objCantidadValorizada.NumeroElemento = NumeroElmento;
                    }

                        #endregion
                    else// Nota Salida
                    {
                        #region NotaSalidaSoles
                        objCantidadValorizada.Salida_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * -1 : d_Cantidad;

                        if (i != 0)
                        {
                            int Posicion = Lista.Count();
                            if (Lista[Posicion - 1].Saldo_Precio == 0)
                            {
                                objCantidadValorizada.Salida_Precio = UltimoPrecioSaldo;
                            }
                            else
                            {
                                objCantidadValorizada.Salida_Precio = Lista[Posicion - 1].Saldo_Precio.Value;
                            }
                        }

                        objCantidadValorizada.Salida_Total = (objCantidadValorizada.Salida_Cantidad * objCantidadValorizada.Salida_Precio);
                        objCantidadValorizada.Salida_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Salida_Total ?? 0, 2);
                        if (i == 0)
                        {

                            objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? objCantidadValorizada.Salida_Cantidad : objCantidadValorizada.Salida_Cantidad * -1;
                            objCantidadValorizada.Saldo_Total = objCantidadValorizada.Salida_Total == null ? 0 : i_EsDevolucion == 1 ? objCantidadValorizada.Salida_Total : objCantidadValorizada.Salida_Total * -1;
                            objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);

                            objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Salida_Precio == null ? 0 : objCantidadValorizada.Salida_Precio;
                            if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                            {

                                UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio ?? 0;
                            }


                        }
                        else
                        {
                            if (IdProductoOld != v_NombreProducto + " " + IdAlmacen)
                            {

                                objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad : objCantidadValorizada.Saldo_Cantidad = d_Cantidad * -1; // cambié 18 Abril
                                objCantidadValorizada.Saldo_Precio = pintIdMoneda == IdMoneda ? d_Precio : d_Precio * TipoCambio;
                                objCantidadValorizada.Saldo_Total = objCantidadValorizada.Saldo_Cantidad * objCantidadValorizada.Saldo_Precio;
                                objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                UltimoPrecioSaldo = (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0) ? objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value : 0;


                            }
                            else
                            {
                                int Posicion = Lista.Count();
                                if (i_EsDevolucion == 1)
                                {

                                    objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value - (d_Cantidad * -1);
                                }
                                else
                                {

                                    objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value - d_Cantidad;
                                }
                                objCantidadValorizada.Saldo_Total = (Lista[Posicion - 1].Saldo_Total.Value - objCantidadValorizada.Salida_Total);
                                objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Saldo_Cantidad == 0 ? 0 : (objCantidadValorizada.Saldo_Total / objCantidadValorizada.Saldo_Cantidad);
                                if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                                {

                                    UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio ?? 0;
                                }
                            }
                        }

                        objCantidadValorizada.NumeroElemento = NumeroElmento;
                        #endregion
                    }
                }
                else if (pintIdMoneda == (int)Currency.Dolares)
                {
                    if (TipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso)
                    {
                        #region NotaIngresoDolares
                        objCantidadValorizada.Ingreso_Cantidad = i_EsDevolucion == 1 ? (d_Cantidad * -1) : d_Cantidad;
                        objCantidadValorizada.Ingreso_Precio = pintIdMoneda == IdMoneda ? d_Precio : Origen == "I" ? d_Precio : d_Precio / TipoCambio;
                        objCantidadValorizada.Ingreso_Total = pintIdMoneda == IdMoneda ? objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 : Total : Origen == "I" ? objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 : Total : objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? (Total * -1) / TipoCambio : Total / TipoCambio;
                        objCantidadValorizada.Ingreso_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Ingreso_Total ?? 0, 2);
                        if (i == 0)
                        {
                            objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * 1 : d_Cantidad;
                            objCantidadValorizada.Saldo_Total = objCantidadValorizada.Ingreso_Total;
                            objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                            objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Ingreso_Precio;
                            if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                            {

                                UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio ?? 0;
                            }
                            NumeroElmento = 0;
                        }
                        else
                        {
                            if (IdProductoOld != v_NombreProducto + " " + IdAlmacen)
                            {
                                objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * -1 : d_Cantidad;

                                objCantidadValorizada.Saldo_Precio = pintIdMoneda == IdMoneda ? d_Precio : Origen == "I" ? d_Precio : d_Precio / TipoCambio;
                                objCantidadValorizada.Saldo_Total = (objCantidadValorizada.Saldo_Cantidad * objCantidadValorizada.Saldo_Precio);
                                objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                UltimoPrecioSaldo = (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0) ? objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value : 0;
                                NumeroElmento = 0;
                            }
                            else
                            {

                                int Posicion = Lista.Count();
                                objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value + objCantidadValorizada.Ingreso_Cantidad;
                                objCantidadValorizada.Saldo_Total = (Lista[Posicion - 1].Saldo_Total.Value + objCantidadValorizada.Ingreso_Total);
                                objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Saldo_Cantidad == 0 ? 0 : (objCantidadValorizada.Saldo_Total / objCantidadValorizada.Saldo_Cantidad);
                                if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                                {

                                    UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio ?? 0;
                                }
                                NumeroElmento = NumeroElmento + 1;
                            }

                        }
                        objCantidadValorizada.NumeroElemento = NumeroElmento;
                        #endregion
                    }
                    else
                    {
                        #region NotaSalidaDolares

                        if (i_EsDevolucion == 1)
                        {
                            objCantidadValorizada.Salida_Cantidad = d_Cantidad * -1;
                        }
                        else
                        {
                            objCantidadValorizada.Salida_Cantidad = d_Cantidad;
                        }
                        if (i != 0)
                        {

                            int Posicion = Lista.Count(); //antes del 5 de abril

                            if (Lista[Posicion - 1].Saldo_Precio == 0)
                            {
                                objCantidadValorizada.Salida_Precio = UltimoPrecioSaldo;
                            }
                            else
                            {

                                objCantidadValorizada.Salida_Precio = Lista[Posicion - 1].Saldo_Precio.Value;
                            }
                        }

                        objCantidadValorizada.Salida_Total = (objCantidadValorizada.Salida_Precio * objCantidadValorizada.Salida_Cantidad);
                        objCantidadValorizada.Salida_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Salida_Total ?? 0, 2);

                        if (i == 0)
                        {

                            if (i_EsDevolucion == 1)
                            {
                                objCantidadValorizada.Saldo_Cantidad = d_Cantidad;
                            }
                            else
                            {
                                objCantidadValorizada.Saldo_Cantidad = d_Cantidad * -1;
                            }

                            objCantidadValorizada.Saldo_Total = objCantidadValorizada.Salida_Total == null ? 0 : objCantidadValorizada.Salida_Total;
                            objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                            objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Salida_Precio == null ? 0 : objCantidadValorizada.Salida_Precio;
                            if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                            {
                                UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value;
                            }

                        }
                        else
                        {
                            if (IdProductoOld != v_NombreProducto + " " + IdAlmacen)
                            {
                                if (i_EsDevolucion == 1)
                                {
                                    objCantidadValorizada.Saldo_Cantidad = d_Cantidad;
                                }
                                else
                                {
                                    objCantidadValorizada.Saldo_Cantidad = d_Cantidad * -1;
                                }

                                objCantidadValorizada.Saldo_Precio = pintIdMoneda == IdMoneda ? d_Precio : d_Precio / TipoCambio;
                                objCantidadValorizada.Saldo_Total = objCantidadValorizada.Saldo_Cantidad * objCantidadValorizada.Saldo_Precio;
                                objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                UltimoPrecioSaldo = (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0) ? objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value : 0;

                            }
                            else
                            {
                                int Posicion = Lista.Count();
                                if (i_EsDevolucion == 1)
                                {

                                    objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value - (d_Cantidad * -1);
                                }
                                else
                                {

                                    objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value - d_Cantidad;
                                }


                                objCantidadValorizada.Saldo_Total = (Lista[Posicion - 1].Saldo_Total.Value - objCantidadValorizada.Salida_Total);
                                objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Saldo_Cantidad == 0 ? 0 : (objCantidadValorizada.Saldo_Total / objCantidadValorizada.Saldo_Cantidad);
                                if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                                {
                                    UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value;
                                }

                            }
                        }
                        objCantidadValorizada.NumeroElemento = NumeroElmento;

                        #endregion
                    }
                }

                else if (pintIdMoneda == (int)Currency.Todas)
                {
                    pintIdMoneda = (int)Currency.Soles;

                    if (pintIdMoneda == (int)Currency.Soles)
                    {
                        if (TipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso)
                        {
                            #region NotaIngreso-Soles

                            objCantidadValorizada.Ingreso_Cantidad = i_EsDevolucion == 1 ? (d_Cantidad * -1) : d_Cantidad;
                            objCantidadValorizada.Ingreso_Precio = pintIdMoneda == IdMoneda ? d_Precio : Origen == "I" ? d_Precio : d_Precio * TipoCambio;
                            objCantidadValorizada.Ingreso_Total = pintIdMoneda == IdMoneda ? objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 : Total : Origen == "I" ? objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 : Total : objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 * TipoCambio : Total * TipoCambio;
                            objCantidadValorizada.Ingreso_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Ingreso_Total ?? 0, 2);

                            if (i == 0)
                            {
                                objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * -1 : d_Cantidad;
                                objCantidadValorizada.Saldo_Total = objCantidadValorizada.Ingreso_Total;
                                objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Ingreso_Precio;

                                if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                                {
                                    UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio ?? 0;
                                }
                                NumeroElmento = 0;

                            }
                            else
                            {
                                if (IdProductoOld != v_NombreProducto + " " + IdAlmacen)
                                {
                                    objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * -1 : d_Cantidad;

                                    objCantidadValorizada.Saldo_Precio = pintIdMoneda == IdMoneda ? d_Precio : Origen == "I" ? d_Precio : d_Precio * TipoCambio;
                                    objCantidadValorizada.Saldo_Total = (objCantidadValorizada.Saldo_Cantidad * objCantidadValorizada.Saldo_Precio);
                                    objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                    UltimoPrecioSaldo = (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0) ? objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value : 0;
                                    NumeroElmento = 0;
                                }
                                else
                                {

                                    int Posicion = Lista.Count(); //Se agrego cuando se edito para Considerar DocumentosInternos
                                    objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value + objCantidadValorizada.Ingreso_Cantidad; //Se agrego cuando se edito para Considerar DocumentosInternos
                                    objCantidadValorizada.Saldo_Total = (Lista[Posicion - 1].Saldo_Total.Value + objCantidadValorizada.Ingreso_Total);//Se agrego cuando se edito para Considerar DocumentosInternos
                                    objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                    objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Saldo_Cantidad == 0 ? 0 : (objCantidadValorizada.Saldo_Total / objCantidadValorizada.Saldo_Cantidad);
                                    if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                                    {
                                        UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio ?? 0;
                                    }
                                    NumeroElmento = NumeroElmento + 1;

                                }
                            }
                            objCantidadValorizada.NumeroElemento = NumeroElmento;
                        }

                            #endregion
                        else// Nota Salida
                        {
                            #region NotaSalidaSoles
                            objCantidadValorizada.Salida_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * -1 : d_Cantidad;

                            if (i != 0)
                            {
                                int Posicion = Lista.Count();
                                if (Lista[Posicion - 1].Saldo_Precio == 0)
                                {
                                    objCantidadValorizada.Salida_Precio = UltimoPrecioSaldo;
                                }
                                else
                                {
                                    objCantidadValorizada.Salida_Precio = Lista[Posicion - 1].Saldo_Precio.Value;
                                }
                            }

                            objCantidadValorizada.Salida_Total = (objCantidadValorizada.Salida_Cantidad * objCantidadValorizada.Salida_Precio);
                            objCantidadValorizada.Salida_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Salida_Total ?? 0, 2);
                            if (i == 0)
                            {

                                objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? objCantidadValorizada.Salida_Cantidad : objCantidadValorizada.Salida_Cantidad * -1;
                                objCantidadValorizada.Saldo_Total = objCantidadValorizada.Salida_Total == null ? 0 : i_EsDevolucion == 1 ? objCantidadValorizada.Salida_Total : objCantidadValorizada.Salida_Total * -1;
                                objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Salida_Precio == null ? 0 : objCantidadValorizada.Salida_Precio;
                                if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                                {

                                    UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio ?? 0;
                                }


                            }
                            else
                            {
                                if (IdProductoOld != v_NombreProducto + " " + IdAlmacen)
                                {

                                    objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad : objCantidadValorizada.Saldo_Cantidad = d_Cantidad * -1; // cambié 18 Abril
                                    objCantidadValorizada.Saldo_Precio = pintIdMoneda == IdMoneda ? d_Precio : d_Precio * TipoCambio;
                                    objCantidadValorizada.Saldo_Total = objCantidadValorizada.Saldo_Cantidad * objCantidadValorizada.Saldo_Precio;
                                    objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                    UltimoPrecioSaldo = (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0) ? objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value : 0;


                                }
                                else
                                {
                                    int Posicion = Lista.Count();
                                    if (i_EsDevolucion == 1)
                                    {

                                        objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value - (d_Cantidad * -1);
                                    }
                                    else
                                    {

                                        objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value - d_Cantidad;
                                    }
                                    objCantidadValorizada.Saldo_Total = (Lista[Posicion - 1].Saldo_Total.Value - objCantidadValorizada.Salida_Total);
                                    objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                    objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Saldo_Cantidad == 0 ? 0 : (objCantidadValorizada.Saldo_Total / objCantidadValorizada.Saldo_Cantidad);
                                    if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                                    {

                                        UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio ?? 0;
                                    }
                                }
                            }

                            objCantidadValorizada.NumeroElemento = NumeroElmento;
                            #endregion
                        }
                    }
                    pintIdMoneda = (int)Currency.Dolares;
                    if (pintIdMoneda == (int)Currency.Dolares)
                    {
                        if (TipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso)
                        {
                            #region NotaIngresoDolares
                            objCantidadValorizada.Ingreso_CantidadDolares = i_EsDevolucion == 1 ? (d_Cantidad * -1) : d_Cantidad;
                            objCantidadValorizada.Ingreso_PrecioDolares = pintIdMoneda == IdMoneda ? d_PrecioDolares : Origen == "I" ? d_Precio : d_Precio / TipoCambio;
                            objCantidadValorizada.Ingreso_TotalDolares = pintIdMoneda == IdMoneda ? objCantidadValorizada.Ingreso_TotalDolares = i_EsDevolucion == 1 ? TotalDolares * -1 : TotalDolares : Origen == "I" ? objCantidadValorizada.Ingreso_TotalDolares = i_EsDevolucion == 1 ? TotalDolares * -1 : TotalDolares : objCantidadValorizada.Ingreso_TotalDolares = i_EsDevolucion == 1 ? (TotalDolares * -1) : TotalDolares;
                            objCantidadValorizada.Ingreso_TotalDolares = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Ingreso_TotalDolares ?? 0, 2);
                            if (i == 0)
                            {
                                objCantidadValorizada.Saldo_CantidadDolares = i_EsDevolucion == 1 ? d_Cantidad * 1 : d_Cantidad;
                                objCantidadValorizada.Saldo_TotalDolares = objCantidadValorizada.Ingreso_TotalDolares;
                                objCantidadValorizada.Saldo_TotalDolares = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_TotalDolares ?? 0, 2);
                                objCantidadValorizada.Saldo_PrecioDolares = objCantidadValorizada.Ingreso_PrecioDolares;

                                if (objCantidadValorizada.Saldo_CantidadDolares != 0 && objCantidadValorizada.Saldo_PrecioDolares != 0)
                                {

                                    UltimoPrecioSaldoDolares = objCantidadValorizada.Saldo_PrecioDolares ?? 0;
                                }
                                NumeroElmento = 0;
                            }
                            else
                            {
                                if (IdProductoOld != v_NombreProducto + " " + IdAlmacen)
                                {
                                    objCantidadValorizada.Saldo_CantidadDolares = i_EsDevolucion == 1 ? d_Cantidad * -1 : d_Cantidad;
                                    objCantidadValorizada.Saldo_PrecioDolares = pintIdMoneda == IdMoneda ? d_PrecioDolares : Origen == "I" ? d_PrecioDolares : d_PrecioDolares / TipoCambio;
                                    objCantidadValorizada.Saldo_TotalDolares = (objCantidadValorizada.Saldo_CantidadDolares * objCantidadValorizada.Saldo_PrecioDolares);
                                    objCantidadValorizada.Saldo_TotalDolares = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_TotalDolares ?? 0, 2);
                                    UltimoPrecioSaldoDolares = (objCantidadValorizada.Saldo_CantidadDolares != 0 && objCantidadValorizada.Saldo_PrecioDolares != 0) ? objCantidadValorizada.Saldo_PrecioDolares == null ? 0 : objCantidadValorizada.Saldo_PrecioDolares.Value : 0;
                                    NumeroElmento = 0;
                                }
                                else
                                {

                                    int Posicion = Lista.Count();
                                    objCantidadValorizada.Saldo_CantidadDolares = Lista[Posicion - 1].Saldo_CantidadDolares.Value + objCantidadValorizada.Ingreso_CantidadDolares;
                                    objCantidadValorizada.Saldo_TotalDolares = (Lista[Posicion - 1].Saldo_TotalDolares.Value + objCantidadValorizada.Ingreso_TotalDolares);
                                    objCantidadValorizada.Saldo_TotalDolares = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_TotalDolares ?? 0, 2);
                                    objCantidadValorizada.Saldo_PrecioDolares = objCantidadValorizada.Saldo_CantidadDolares == 0 ? 0 : (objCantidadValorizada.Saldo_TotalDolares / objCantidadValorizada.Saldo_CantidadDolares);
                                    if (objCantidadValorizada.Saldo_CantidadDolares != 0 && objCantidadValorizada.Saldo_PrecioDolares != 0)
                                    {

                                        UltimoPrecioSaldoDolares = objCantidadValorizada.Saldo_PrecioDolares ?? 0;
                                    }
                                    NumeroElmento = NumeroElmento + 1;
                                }

                            }
                            objCantidadValorizada.NumeroElemento = NumeroElmento;
                            #endregion
                        }
                        else
                        {
                            #region NotaSalidaDolares

                            if (i_EsDevolucion == 1)
                            {
                                objCantidadValorizada.Salida_CantidadDolares = d_Cantidad * -1;
                            }
                            else
                            {
                                objCantidadValorizada.Salida_CantidadDolares = d_Cantidad;
                            }

                            if (i != 0)
                            {

                                int Posicion = Lista.Count(); //antes del 5 de abril

                                if (Lista[Posicion - 1].Saldo_PrecioDolares == 0)
                                {
                                    objCantidadValorizada.Salida_PrecioDolares = UltimoPrecioSaldoDolares;
                                }
                                else
                                {

                                    objCantidadValorizada.Salida_PrecioDolares = Lista[Posicion - 1].Saldo_PrecioDolares.Value;
                                }
                            }
                            objCantidadValorizada.Salida_TotalDolares = (objCantidadValorizada.Salida_PrecioDolares * objCantidadValorizada.Salida_CantidadDolares);
                            objCantidadValorizada.Salida_TotalDolares = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Salida_TotalDolares ?? 0, 2);

                            if (i == 0)
                            {

                                if (i_EsDevolucion == 1)
                                {
                                    objCantidadValorizada.Saldo_CantidadDolares = d_Cantidad;
                                }
                                else
                                {
                                    objCantidadValorizada.Saldo_CantidadDolares = d_Cantidad * -1;
                                }

                                objCantidadValorizada.Saldo_TotalDolares = objCantidadValorizada.Salida_TotalDolares == null ? 0 : objCantidadValorizada.Salida_TotalDolares;
                                objCantidadValorizada.Saldo_TotalDolares = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_TotalDolares ?? 0, 2);
                                objCantidadValorizada.Saldo_PrecioDolares = objCantidadValorizada.Salida_PrecioDolares == null ? 0 : objCantidadValorizada.Salida_PrecioDolares;
                                if (objCantidadValorizada.Saldo_CantidadDolares != 0 && objCantidadValorizada.Saldo_PrecioDolares != 0)
                                {
                                    UltimoPrecioSaldoDolares = objCantidadValorizada.Saldo_PrecioDolares == null ? 0 : objCantidadValorizada.Saldo_PrecioDolares.Value;
                                }

                            }
                            else
                            {
                                if (IdProductoOld != v_NombreProducto + " " + IdAlmacen)
                                {
                                    if (i_EsDevolucion == 1)
                                    {
                                        objCantidadValorizada.Saldo_CantidadDolares = d_Cantidad;
                                    }
                                    else
                                    {
                                        objCantidadValorizada.Saldo_CantidadDolares = d_Cantidad * -1;
                                    }

                                    objCantidadValorizada.Saldo_PrecioDolares = pintIdMoneda == IdMoneda ? d_PrecioDolares : d_PrecioDolares / TipoCambio;
                                    objCantidadValorizada.Saldo_TotalDolares = objCantidadValorizada.Saldo_CantidadDolares * objCantidadValorizada.Saldo_PrecioDolares;
                                    objCantidadValorizada.Saldo_TotalDolares = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_TotalDolares ?? 0, 2);
                                    UltimoPrecioSaldoDolares = (objCantidadValorizada.Saldo_CantidadDolares != 0 && objCantidadValorizada.Saldo_PrecioDolares != 0) ? objCantidadValorizada.Saldo_PrecioDolares == null ? 0 : objCantidadValorizada.Saldo_PrecioDolares.Value : 0;

                                }
                                else
                                {
                                    int Posicion = Lista.Count();
                                    if (i_EsDevolucion == 1)
                                    {

                                        objCantidadValorizada.Saldo_CantidadDolares = Lista[Posicion - 1].Saldo_CantidadDolares.Value - (d_Cantidad * -1);
                                    }
                                    else
                                    {

                                        objCantidadValorizada.Saldo_CantidadDolares = Lista[Posicion - 1].Saldo_CantidadDolares.Value - d_Cantidad;
                                    }


                                    objCantidadValorizada.Saldo_TotalDolares = (Lista[Posicion - 1].Saldo_TotalDolares.Value - objCantidadValorizada.Salida_TotalDolares);
                                    objCantidadValorizada.Saldo_TotalDolares = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_TotalDolares ?? 0, 2);
                                    objCantidadValorizada.Saldo_PrecioDolares = objCantidadValorizada.Saldo_CantidadDolares == 0 ? 0 : (objCantidadValorizada.Saldo_TotalDolares / objCantidadValorizada.Saldo_CantidadDolares);
                                    if (objCantidadValorizada.Saldo_CantidadDolares != 0 && objCantidadValorizada.Saldo_PrecioDolares != 0)
                                    {
                                        UltimoPrecioSaldoDolares = objCantidadValorizada.Saldo_PrecioDolares == null ? 0 : objCantidadValorizada.Saldo_PrecioDolares.Value;
                                    }

                                }
                            }
                            objCantidadValorizada.NumeroElemento = NumeroElmento;

                            #endregion
                        }
                    }
                }

                return objCantidadValorizada;
            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                return null;

            }

        }


        public CantidadValorizadas ProcesoValorizacionProductoAlmacenPedidoNotasSalida2016(ref  OperationResult objOperationResult, int TipoMovimiento, string IdProductoOld, string v_NombreProducto, string IdAlmacen, int pintIdMoneda, string Origen, int i_EsDevolucion, decimal d_Cantidad, int IdMoneda, decimal d_Precio, int i, decimal TipoCambio, ref  decimal UltimoPrecioSaldo, int NumeroElmento, List<ReporteListadoSalidaAlmacenAnalitico> Lista, decimal Total, decimal TotalDolares, decimal d_PrecioDolares, ref decimal UltimoPrecioSaldoDolares)
        {


            try
            {

                int NumElemento = NumeroElmento;
                objOperationResult.Success = 1;
                CantidadValorizadas objCantidadValorizada = new CantidadValorizadas();
                if (pintIdMoneda == (int)Currency.Soles)
                {
                    if (TipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso)
                    {
                        #region NotaIngreso-Soles

                        objCantidadValorizada.Ingreso_Cantidad = i_EsDevolucion == 1 ? (d_Cantidad * -1) : d_Cantidad;
                        objCantidadValorizada.Ingreso_Precio = pintIdMoneda == IdMoneda ? d_Precio : Origen == "I" ? d_Precio : d_Precio * TipoCambio;
                        objCantidadValorizada.Ingreso_Total = pintIdMoneda == IdMoneda ? objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 : Total : Origen == "I" ? objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 : Total : objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 * TipoCambio : Total * TipoCambio;
                        // objCantidadValorizada.Ingreso_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Ingreso_Total ?? 0, 2);
                        if (i == 0)
                        {
                            objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * -1 : d_Cantidad;
                            objCantidadValorizada.Saldo_Total = objCantidadValorizada.Ingreso_Total;

                            // objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                            objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Ingreso_Precio;

                            if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                            {
                                UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio ?? 0;
                            }
                            NumeroElmento = 0;

                        }
                        else
                        {
                            if (IdProductoOld != v_NombreProducto + " " + IdAlmacen)
                            {
                                objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * -1 : d_Cantidad;

                                objCantidadValorizada.Saldo_Precio = pintIdMoneda == IdMoneda ? d_Precio : Origen == "I" ? d_Precio : d_Precio * TipoCambio;
                                objCantidadValorizada.Saldo_Total = (objCantidadValorizada.Saldo_Cantidad * objCantidadValorizada.Saldo_Precio);

                                //objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                UltimoPrecioSaldo = (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0) ? objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value : 0;
                                NumeroElmento = 0;
                            }
                            else
                            {

                                int Posicion = Lista.Count(); //Se agrego cuando se edito para Considerar DocumentosInternos
                                objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value + objCantidadValorizada.Ingreso_Cantidad; //Se agrego cuando se edito para Considerar DocumentosInternos
                                objCantidadValorizada.Saldo_Total = (Lista[Posicion - 1].Saldo_Total.Value + objCantidadValorizada.Ingreso_Total);//Se agrego cuando se edito para Considerar DocumentosInternos
                                //objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Saldo_Cantidad == 0 ? 0 : (objCantidadValorizada.Saldo_Total / objCantidadValorizada.Saldo_Cantidad);
                                if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                                {
                                    UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio ?? 0;
                                }
                                NumeroElmento = NumeroElmento + 1;

                            }
                        }
                        objCantidadValorizada.NumeroElemento = NumeroElmento;
                    }

                        #endregion
                    else// Nota Salida
                    {
                        #region NotaSalidaSoles
                        objCantidadValorizada.Salida_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * -1 : d_Cantidad;

                        if (i != 0)
                        {
                            int Posicion = Lista.Count();
                            if (Lista[Posicion - 1].Saldo_Precio == 0)
                            {
                                objCantidadValorizada.Salida_Precio = UltimoPrecioSaldo;
                            }
                            else
                            {
                                objCantidadValorizada.Salida_Precio = Lista[Posicion - 1].Saldo_Precio.Value;
                            }
                        }

                        objCantidadValorizada.Salida_Total = (objCantidadValorizada.Salida_Cantidad * objCantidadValorizada.Salida_Precio);
                        //objCantidadValorizada.Salida_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Salida_Total ?? 0, 2);
                        if (i == 0)
                        {

                            objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? objCantidadValorizada.Salida_Cantidad : objCantidadValorizada.Salida_Cantidad * -1;
                            objCantidadValorizada.Saldo_Total = objCantidadValorizada.Salida_Total == null ? 0 : i_EsDevolucion == 1 ? objCantidadValorizada.Salida_Total : objCantidadValorizada.Salida_Total * -1;
                            // objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);

                            objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Salida_Precio == null ? 0 : objCantidadValorizada.Salida_Precio;
                            if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                            {

                                UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio ?? 0;
                            }


                        }
                        else
                        {
                            if (IdProductoOld != v_NombreProducto + " " + IdAlmacen)
                            {

                                objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad : objCantidadValorizada.Saldo_Cantidad = d_Cantidad * -1; // cambié 18 Abril
                                objCantidadValorizada.Saldo_Precio = pintIdMoneda == IdMoneda ? d_Precio : d_Precio * TipoCambio;
                                objCantidadValorizada.Saldo_Total = objCantidadValorizada.Saldo_Cantidad * objCantidadValorizada.Saldo_Precio;
                                // objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                UltimoPrecioSaldo = (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0) ? objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value : 0;


                            }
                            else
                            {
                                int Posicion = Lista.Count();
                                if (i_EsDevolucion == 1)
                                {

                                    objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value - (d_Cantidad * -1);
                                }
                                else
                                {

                                    objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value - d_Cantidad;
                                }
                                objCantidadValorizada.Saldo_Total = (Lista[Posicion - 1].Saldo_Total.Value - objCantidadValorizada.Salida_Total);
                                // objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Saldo_Cantidad == 0 ? 0 : (objCantidadValorizada.Saldo_Total / objCantidadValorizada.Saldo_Cantidad);
                                if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                                {

                                    UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio ?? 0;
                                }
                            }
                        }

                        objCantidadValorizada.NumeroElemento = NumeroElmento;
                        #endregion
                    }
                }
                else if (pintIdMoneda == (int)Currency.Dolares)
                {
                    if (TipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso)
                    {
                        #region NotaIngresoDolares
                        objCantidadValorizada.Ingreso_Cantidad = i_EsDevolucion == 1 ? (d_Cantidad * -1) : d_Cantidad;
                        objCantidadValorizada.Ingreso_Precio = pintIdMoneda == IdMoneda ? d_Precio : Origen == "I" ? d_Precio : d_Precio / TipoCambio;
                        objCantidadValorizada.Ingreso_Total = pintIdMoneda == IdMoneda ? objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 : Total : Origen == "I" ? objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 : Total : objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? (Total * -1) / TipoCambio : Total / TipoCambio;
                        // objCantidadValorizada.Ingreso_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Ingreso_Total ?? 0, 2);
                        if (i == 0)
                        {
                            objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * 1 : d_Cantidad;
                            objCantidadValorizada.Saldo_Total = objCantidadValorizada.Ingreso_Total;
                            // objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                            objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Ingreso_Precio;
                            if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                            {

                                UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio ?? 0;
                            }
                            NumeroElmento = 0;
                        }
                        else
                        {
                            if (IdProductoOld != v_NombreProducto + " " + IdAlmacen)
                            {
                                objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * -1 : d_Cantidad;

                                objCantidadValorizada.Saldo_Precio = pintIdMoneda == IdMoneda ? d_Precio : Origen == "I" ? d_Precio : d_Precio / TipoCambio;
                                objCantidadValorizada.Saldo_Total = (objCantidadValorizada.Saldo_Cantidad * objCantidadValorizada.Saldo_Precio);
                                // objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                UltimoPrecioSaldo = (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0) ? objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value : 0;
                                NumeroElmento = 0;
                            }
                            else
                            {

                                int Posicion = Lista.Count();
                                objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value + objCantidadValorizada.Ingreso_Cantidad;
                                objCantidadValorizada.Saldo_Total = (Lista[Posicion - 1].Saldo_Total.Value + objCantidadValorizada.Ingreso_Total);
                                // objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Saldo_Cantidad == 0 ? 0 : (objCantidadValorizada.Saldo_Total / objCantidadValorizada.Saldo_Cantidad);
                                if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                                {

                                    UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio ?? 0;
                                }
                                NumeroElmento = NumeroElmento + 1;
                            }

                        }
                        objCantidadValorizada.NumeroElemento = NumeroElmento;
                        #endregion
                    }
                    else
                    {
                        #region NotaSalidaDolares

                        if (i_EsDevolucion == 1)
                        {
                            objCantidadValorizada.Salida_Cantidad = d_Cantidad * -1;
                        }
                        else
                        {
                            objCantidadValorizada.Salida_Cantidad = d_Cantidad;
                        }
                        if (i != 0)
                        {

                            int Posicion = Lista.Count(); //antes del 5 de abril

                            if (Lista[Posicion - 1].Saldo_Precio == 0)
                            {
                                objCantidadValorizada.Salida_Precio = UltimoPrecioSaldo;
                            }
                            else
                            {

                                objCantidadValorizada.Salida_Precio = Lista[Posicion - 1].Saldo_Precio.Value;
                            }
                        }

                        objCantidadValorizada.Salida_Total = (objCantidadValorizada.Salida_Precio * objCantidadValorizada.Salida_Cantidad);
                        // objCantidadValorizada.Salida_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Salida_Total ?? 0, 2);

                        if (i == 0)
                        {

                            if (i_EsDevolucion == 1)
                            {
                                objCantidadValorizada.Saldo_Cantidad = d_Cantidad;
                            }
                            else
                            {
                                objCantidadValorizada.Saldo_Cantidad = d_Cantidad * -1;
                            }

                            objCantidadValorizada.Saldo_Total = objCantidadValorizada.Salida_Total == null ? 0 : objCantidadValorizada.Salida_Total;
                            //  objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                            objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Salida_Precio == null ? 0 : objCantidadValorizada.Salida_Precio;
                            if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                            {
                                UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value;
                            }

                        }
                        else
                        {
                            if (IdProductoOld != v_NombreProducto + " " + IdAlmacen)
                            {
                                if (i_EsDevolucion == 1)
                                {
                                    objCantidadValorizada.Saldo_Cantidad = d_Cantidad;
                                }
                                else
                                {
                                    objCantidadValorizada.Saldo_Cantidad = d_Cantidad * -1;
                                }

                                objCantidadValorizada.Saldo_Precio = pintIdMoneda == IdMoneda ? d_Precio : d_Precio / TipoCambio;
                                objCantidadValorizada.Saldo_Total = objCantidadValorizada.Saldo_Cantidad * objCantidadValorizada.Saldo_Precio;
                                // objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                UltimoPrecioSaldo = (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0) ? objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value : 0;

                            }
                            else
                            {
                                int Posicion = Lista.Count();
                                if (i_EsDevolucion == 1)
                                {

                                    objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value - (d_Cantidad * -1);
                                }
                                else
                                {

                                    objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value - d_Cantidad;
                                }


                                objCantidadValorizada.Saldo_Total = (Lista[Posicion - 1].Saldo_Total.Value - objCantidadValorizada.Salida_Total);
                                // objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Saldo_Cantidad == 0 ? 0 : (objCantidadValorizada.Saldo_Total / objCantidadValorizada.Saldo_Cantidad);
                                if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                                {
                                    UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value;
                                }

                            }
                        }
                        objCantidadValorizada.NumeroElemento = NumeroElmento;

                        #endregion
                    }
                }

                else if (pintIdMoneda == (int)Currency.Todas)
                {
                    pintIdMoneda = (int)Currency.Soles;

                    if (pintIdMoneda == (int)Currency.Soles)
                    {
                        if (TipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso)
                        {
                            #region NotaIngreso-Soles

                            objCantidadValorizada.Ingreso_Cantidad = i_EsDevolucion == 1 ? (d_Cantidad * -1) : d_Cantidad;
                            objCantidadValorizada.Ingreso_Precio = pintIdMoneda == IdMoneda ? d_Precio : Origen == "I" ? d_Precio : d_Precio * TipoCambio;
                            objCantidadValorizada.Ingreso_Total = pintIdMoneda == IdMoneda ? objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 : Total : Origen == "I" ? objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 : Total : objCantidadValorizada.Ingreso_Total = i_EsDevolucion == 1 ? Total * -1 * TipoCambio : Total * TipoCambio;
                            // objCantidadValorizada.Ingreso_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Ingreso_Total ?? 0, 2);

                            if (i == 0)
                            {
                                objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * -1 : d_Cantidad;
                                objCantidadValorizada.Saldo_Total = objCantidadValorizada.Ingreso_Total;
                                // objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Ingreso_Precio;

                                if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                                {
                                    UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio ?? 0;
                                }
                                NumeroElmento = 0;

                            }
                            else
                            {
                                if (IdProductoOld != v_NombreProducto + " " + IdAlmacen)
                                {
                                    objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * -1 : d_Cantidad;

                                    objCantidadValorizada.Saldo_Precio = pintIdMoneda == IdMoneda ? d_Precio : Origen == "I" ? d_Precio : d_Precio * TipoCambio;
                                    objCantidadValorizada.Saldo_Total = (objCantidadValorizada.Saldo_Cantidad * objCantidadValorizada.Saldo_Precio);
                                    // objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                    UltimoPrecioSaldo = (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0) ? objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value : 0;
                                    NumeroElmento = 0;
                                }
                                else
                                {

                                    int Posicion = Lista.Count(); //Se agrego cuando se edito para Considerar DocumentosInternos
                                    objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value + objCantidadValorizada.Ingreso_Cantidad; //Se agrego cuando se edito para Considerar DocumentosInternos
                                    objCantidadValorizada.Saldo_Total = (Lista[Posicion - 1].Saldo_Total.Value + objCantidadValorizada.Ingreso_Total);//Se agrego cuando se edito para Considerar DocumentosInternos
                                    // objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                    objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Saldo_Cantidad == 0 ? 0 : (objCantidadValorizada.Saldo_Total / objCantidadValorizada.Saldo_Cantidad);
                                    if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                                    {
                                        UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio ?? 0;
                                    }
                                    NumeroElmento = NumeroElmento + 1;

                                }
                            }
                            objCantidadValorizada.NumeroElemento = NumeroElmento;
                        }

                            #endregion
                        else// Nota Salida
                        {
                            #region NotaSalidaSoles
                            objCantidadValorizada.Salida_Cantidad = i_EsDevolucion == 1 ? d_Cantidad * -1 : d_Cantidad;

                            if (i != 0)
                            {
                                int Posicion = Lista.Count();
                                if (Lista[Posicion - 1].Saldo_Precio == 0)
                                {
                                    objCantidadValorizada.Salida_Precio = UltimoPrecioSaldo;
                                }
                                else
                                {
                                    objCantidadValorizada.Salida_Precio = Lista[Posicion - 1].Saldo_Precio.Value;
                                }
                            }

                            objCantidadValorizada.Salida_Total = (objCantidadValorizada.Salida_Cantidad * objCantidadValorizada.Salida_Precio);
                            // objCantidadValorizada.Salida_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Salida_Total ?? 0, 2);
                            if (i == 0)
                            {

                                objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? objCantidadValorizada.Salida_Cantidad : objCantidadValorizada.Salida_Cantidad * -1;
                                objCantidadValorizada.Saldo_Total = objCantidadValorizada.Salida_Total == null ? 0 : i_EsDevolucion == 1 ? objCantidadValorizada.Salida_Total : objCantidadValorizada.Salida_Total * -1;
                                // objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Salida_Precio == null ? 0 : objCantidadValorizada.Salida_Precio;
                                if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                                {

                                    UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio ?? 0;
                                }


                            }
                            else
                            {
                                if (IdProductoOld != v_NombreProducto + " " + IdAlmacen)
                                {

                                    objCantidadValorizada.Saldo_Cantidad = i_EsDevolucion == 1 ? d_Cantidad : objCantidadValorizada.Saldo_Cantidad = d_Cantidad * -1; // cambié 18 Abril
                                    objCantidadValorizada.Saldo_Precio = pintIdMoneda == IdMoneda ? d_Precio : d_Precio * TipoCambio;
                                    objCantidadValorizada.Saldo_Total = objCantidadValorizada.Saldo_Cantidad * objCantidadValorizada.Saldo_Precio;
                                    // objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                    UltimoPrecioSaldo = (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0) ? objCantidadValorizada.Saldo_Precio == null ? 0 : objCantidadValorizada.Saldo_Precio.Value : 0;


                                }
                                else
                                {
                                    int Posicion = Lista.Count();
                                    if (i_EsDevolucion == 1)
                                    {

                                        objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value - (d_Cantidad * -1);
                                    }
                                    else
                                    {

                                        objCantidadValorizada.Saldo_Cantidad = Lista[Posicion - 1].Saldo_Cantidad.Value - d_Cantidad;
                                    }
                                    objCantidadValorizada.Saldo_Total = (Lista[Posicion - 1].Saldo_Total.Value - objCantidadValorizada.Salida_Total);
                                    //objCantidadValorizada.Saldo_Total = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_Total ?? 0, 2);
                                    objCantidadValorizada.Saldo_Precio = objCantidadValorizada.Saldo_Cantidad == 0 ? 0 : (objCantidadValorizada.Saldo_Total / objCantidadValorizada.Saldo_Cantidad);
                                    if (objCantidadValorizada.Saldo_Cantidad != 0 && objCantidadValorizada.Saldo_Precio != 0)
                                    {

                                        UltimoPrecioSaldo = objCantidadValorizada.Saldo_Precio ?? 0;
                                    }
                                }
                            }

                            objCantidadValorizada.NumeroElemento = NumeroElmento;
                            #endregion
                        }
                    }
                    pintIdMoneda = (int)Currency.Dolares;
                    if (pintIdMoneda == (int)Currency.Dolares)
                    {
                        if (TipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso)
                        {
                            #region NotaIngresoDolares
                            objCantidadValorizada.Ingreso_CantidadDolares = i_EsDevolucion == 1 ? (d_Cantidad * -1) : d_Cantidad;
                            objCantidadValorizada.Ingreso_PrecioDolares = pintIdMoneda == IdMoneda ? d_PrecioDolares : Origen == "I" ? d_Precio : d_Precio / TipoCambio;
                            objCantidadValorizada.Ingreso_TotalDolares = pintIdMoneda == IdMoneda ? objCantidadValorizada.Ingreso_TotalDolares = i_EsDevolucion == 1 ? TotalDolares * -1 : TotalDolares : Origen == "I" ? objCantidadValorizada.Ingreso_TotalDolares = i_EsDevolucion == 1 ? TotalDolares * -1 : TotalDolares : objCantidadValorizada.Ingreso_TotalDolares = i_EsDevolucion == 1 ? (TotalDolares * -1) : TotalDolares;
                            // objCantidadValorizada.Ingreso_TotalDolares = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Ingreso_TotalDolares ?? 0, 2);
                            if (i == 0)
                            {
                                objCantidadValorizada.Saldo_CantidadDolares = i_EsDevolucion == 1 ? d_Cantidad * 1 : d_Cantidad;
                                objCantidadValorizada.Saldo_TotalDolares = objCantidadValorizada.Ingreso_TotalDolares;
                                // objCantidadValorizada.Saldo_TotalDolares = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_TotalDolares ?? 0, 2);
                                objCantidadValorizada.Saldo_PrecioDolares = objCantidadValorizada.Ingreso_PrecioDolares;

                                if (objCantidadValorizada.Saldo_CantidadDolares != 0 && objCantidadValorizada.Saldo_PrecioDolares != 0)
                                {

                                    UltimoPrecioSaldoDolares = objCantidadValorizada.Saldo_PrecioDolares ?? 0;
                                }
                                NumeroElmento = 0;
                            }
                            else
                            {
                                if (IdProductoOld != v_NombreProducto + " " + IdAlmacen)
                                {
                                    objCantidadValorizada.Saldo_CantidadDolares = i_EsDevolucion == 1 ? d_Cantidad * -1 : d_Cantidad;
                                    objCantidadValorizada.Saldo_PrecioDolares = pintIdMoneda == IdMoneda ? d_PrecioDolares : Origen == "I" ? d_PrecioDolares : d_PrecioDolares / TipoCambio;
                                    objCantidadValorizada.Saldo_TotalDolares = (objCantidadValorizada.Saldo_CantidadDolares * objCantidadValorizada.Saldo_PrecioDolares);
                                    // objCantidadValorizada.Saldo_TotalDolares = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_TotalDolares ?? 0, 2);
                                    UltimoPrecioSaldoDolares = (objCantidadValorizada.Saldo_CantidadDolares != 0 && objCantidadValorizada.Saldo_PrecioDolares != 0) ? objCantidadValorizada.Saldo_PrecioDolares == null ? 0 : objCantidadValorizada.Saldo_PrecioDolares.Value : 0;
                                    NumeroElmento = 0;
                                }
                                else
                                {

                                    int Posicion = Lista.Count();
                                    objCantidadValorizada.Saldo_CantidadDolares = Lista[Posicion - 1].Saldo_CantidadDolares.Value + objCantidadValorizada.Ingreso_CantidadDolares;
                                    objCantidadValorizada.Saldo_TotalDolares = (Lista[Posicion - 1].Saldo_TotalDolares.Value + objCantidadValorizada.Ingreso_TotalDolares);
                                    // objCantidadValorizada.Saldo_TotalDolares = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_TotalDolares ?? 0, 2);
                                    objCantidadValorizada.Saldo_PrecioDolares = objCantidadValorizada.Saldo_CantidadDolares == 0 ? 0 : (objCantidadValorizada.Saldo_TotalDolares / objCantidadValorizada.Saldo_CantidadDolares);
                                    if (objCantidadValorizada.Saldo_CantidadDolares != 0 && objCantidadValorizada.Saldo_PrecioDolares != 0)
                                    {

                                        UltimoPrecioSaldoDolares = objCantidadValorizada.Saldo_PrecioDolares ?? 0;
                                    }
                                    NumeroElmento = NumeroElmento + 1;
                                }

                            }
                            objCantidadValorizada.NumeroElemento = NumeroElmento;
                            #endregion
                        }
                        else
                        {
                            #region NotaSalidaDolares

                            if (i_EsDevolucion == 1)
                            {
                                objCantidadValorizada.Salida_CantidadDolares = d_Cantidad * -1;
                            }
                            else
                            {
                                objCantidadValorizada.Salida_CantidadDolares = d_Cantidad;
                            }

                            if (i != 0)
                            {

                                int Posicion = Lista.Count(); //antes del 5 de abril

                                if (Lista[Posicion - 1].Saldo_PrecioDolares == 0)
                                {
                                    objCantidadValorizada.Salida_PrecioDolares = UltimoPrecioSaldoDolares;
                                }
                                else
                                {

                                    objCantidadValorizada.Salida_PrecioDolares = Lista[Posicion - 1].Saldo_PrecioDolares.Value;
                                }
                            }
                            objCantidadValorizada.Salida_TotalDolares = (objCantidadValorizada.Salida_PrecioDolares * objCantidadValorizada.Salida_CantidadDolares);
                            // objCantidadValorizada.Salida_TotalDolares = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Salida_TotalDolares ?? 0, 2);

                            if (i == 0)
                            {

                                if (i_EsDevolucion == 1)
                                {
                                    objCantidadValorizada.Saldo_CantidadDolares = d_Cantidad;
                                }
                                else
                                {
                                    objCantidadValorizada.Saldo_CantidadDolares = d_Cantidad * -1;
                                }

                                objCantidadValorizada.Saldo_TotalDolares = objCantidadValorizada.Salida_TotalDolares == null ? 0 : objCantidadValorizada.Salida_TotalDolares;
                                // objCantidadValorizada.Saldo_TotalDolares = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_TotalDolares ?? 0, 2);
                                objCantidadValorizada.Saldo_PrecioDolares = objCantidadValorizada.Salida_PrecioDolares == null ? 0 : objCantidadValorizada.Salida_PrecioDolares;
                                if (objCantidadValorizada.Saldo_CantidadDolares != 0 && objCantidadValorizada.Saldo_PrecioDolares != 0)
                                {
                                    UltimoPrecioSaldoDolares = objCantidadValorizada.Saldo_PrecioDolares == null ? 0 : objCantidadValorizada.Saldo_PrecioDolares.Value;
                                }

                            }
                            else
                            {
                                if (IdProductoOld != v_NombreProducto + " " + IdAlmacen)
                                {
                                    if (i_EsDevolucion == 1)
                                    {
                                        objCantidadValorizada.Saldo_CantidadDolares = d_Cantidad;
                                    }
                                    else
                                    {
                                        objCantidadValorizada.Saldo_CantidadDolares = d_Cantidad * -1;
                                    }

                                    objCantidadValorizada.Saldo_PrecioDolares = pintIdMoneda == IdMoneda ? d_PrecioDolares : d_PrecioDolares / TipoCambio;
                                    objCantidadValorizada.Saldo_TotalDolares = objCantidadValorizada.Saldo_CantidadDolares * objCantidadValorizada.Saldo_PrecioDolares;
                                    // objCantidadValorizada.Saldo_TotalDolares = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_TotalDolares ?? 0, 2);
                                    UltimoPrecioSaldoDolares = (objCantidadValorizada.Saldo_CantidadDolares != 0 && objCantidadValorizada.Saldo_PrecioDolares != 0) ? objCantidadValorizada.Saldo_PrecioDolares == null ? 0 : objCantidadValorizada.Saldo_PrecioDolares.Value : 0;

                                }
                                else
                                {
                                    int Posicion = Lista.Count();
                                    if (i_EsDevolucion == 1)
                                    {

                                        objCantidadValorizada.Saldo_CantidadDolares = Lista[Posicion - 1].Saldo_CantidadDolares.Value - (d_Cantidad * -1);
                                    }
                                    else
                                    {

                                        objCantidadValorizada.Saldo_CantidadDolares = Lista[Posicion - 1].Saldo_CantidadDolares.Value - d_Cantidad;
                                    }


                                    objCantidadValorizada.Saldo_TotalDolares = (Lista[Posicion - 1].Saldo_TotalDolares.Value - objCantidadValorizada.Salida_TotalDolares);
                                    // objCantidadValorizada.Saldo_TotalDolares = Utils.Windows.DevuelveValorRedondeado(objCantidadValorizada.Saldo_TotalDolares ?? 0, 2);
                                    objCantidadValorizada.Saldo_PrecioDolares = objCantidadValorizada.Saldo_CantidadDolares == 0 ? 0 : (objCantidadValorizada.Saldo_TotalDolares / objCantidadValorizada.Saldo_CantidadDolares);
                                    if (objCantidadValorizada.Saldo_CantidadDolares != 0 && objCantidadValorizada.Saldo_PrecioDolares != 0)
                                    {
                                        UltimoPrecioSaldoDolares = objCantidadValorizada.Saldo_PrecioDolares == null ? 0 : objCantidadValorizada.Saldo_PrecioDolares.Value;
                                    }

                                }
                            }
                            objCantidadValorizada.NumeroElemento = NumeroElmento;

                            #endregion
                        }
                    }
                }

                return objCantidadValorizada;
            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                return null;

            }

        }


        /// <summary>
        /// Este proceso permite identificar las Guias de Compra que no se tomaron en cuenta en el Reporte de Kardex Valorizado 
        /// </summary>
        /// <returns></returns>

        public List<string> IdentificarGuias(ref OperationResult pobjOperationResult, DateTime? pdtFechaInicio, DateTime? pdtFechaFin, string pstrFilterExpression, int pintIdMoneda, string pstrEmpresa, string pstrRUC, string pstrMoneda, int idEstablecimiento, int IncluirPedido, string pstrCodigoProducto, string pstrNumeroPedido, int ArticulosStockMayor0, int SoloArticulosMovimiento, int ConsideraDocInternos, int FormatoCant, string IdMarca, bool ConsiderarGuiasCompras)
        {
            try
            {
                pobjOperationResult.Success = 1;
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region NoConsideraGuiaCompras
                    List<string> GuiasNoConsideradas = new List<string>();
                    if (IncluirPedido == 1 && ArticulosStockMayor0 == 1)
                    {

                        var query = (from A in dbContext.movimientodetalle
                                     join B in dbContext.productodetalle on new { IdProductoDetalle = A.v_IdProductoDetalle, eliminado = 0 } equals new { IdProductoDetalle = B.v_IdProductoDetalle, eliminado = B.i_Eliminado.Value } into B_join
                                     from B in B_join.DefaultIfEmpty()
                                     join C in dbContext.producto on new { IdProducto = B.v_IdProducto, eliminado = 0 } equals new { IdProducto = C.v_IdProducto, eliminado = C.i_Eliminado.Value } into C_join
                                     from C in C_join.DefaultIfEmpty()
                                     join D in dbContext.movimiento on new { A.v_IdMovimiento, eliminado = 0 } equals new { D.v_IdMovimiento, eliminado = D.i_Eliminado.Value } into D_join
                                     from D in D_join.DefaultIfEmpty()
                                     join E in dbContext.cliente on new { IdCliente = D.v_IdCliente, eliminado = 0 } equals new { IdCliente = E.v_IdCliente, eliminado = E.i_Eliminado.Value } into E_join
                                     from E in E_join.DefaultIfEmpty()
                                     join F in dbContext.establecimientoalmacen on new { IdAlmacen = D.i_IdAlmacenOrigen, eliminado = 0 } equals new { IdAlmacen = F.i_IdAlmacen, eliminado = F.i_Eliminado.Value } into F_join
                                     from F in F_join.DefaultIfEmpty()
                                     join G in dbContext.almacen on new { IdAlmacen = F.i_IdAlmacen.Value, eliminado = 0 } equals new { IdAlmacen = G.i_IdAlmacen, eliminado = G.i_Eliminado.Value } into G_join
                                     from G in G_join.DefaultIfEmpty()
                                     join H in dbContext.documento on new { IdTipoDoc = A.i_IdTipoDocumento.Value, eliminado = 0 } equals new { IdTipoDoc = H.i_CodigoDocumento, eliminado = H.i_Eliminado.Value } into H_join
                                     from H in H_join.DefaultIfEmpty()
                                     join I in dbContext.productoalmacen on new { IdProd = A.v_IdProductoDetalle, IdAlmcen = D.i_IdAlmacenOrigen.Value, eliminado = 0, per = Periodo } equals new { IdProd = I.v_ProductoDetalleId, IdAlmcen = I.i_IdAlmacen, eliminado = I.i_Eliminado.Value, per = I.v_Periodo } into I_join
                                     from I in I_join.DefaultIfEmpty()
                                     join K in dbContext.datahierarchy on new { Grupo = 17, UnidadMedida = C.i_IdUnidadMedida.Value, eliminado = 0 } equals new { Grupo = K.i_GroupId, UnidadMedida = K.i_ItemId, eliminado = K.i_IsDeleted.Value } into K_join
                                     from K in K_join.DefaultIfEmpty()
                                     where A.i_Eliminado == 0 && D.i_IdTipoMovimiento != (int)TipoDeMovimiento.Transferencia
                                     && (D.t_Fecha.Value >= pdtFechaInicio && D.t_Fecha.Value <= pdtFechaFin) && F.i_IdEstablecimiento == idEstablecimiento && (C.v_CodInterno.Trim() == pstrCodigoProducto.Trim() || pstrCodigoProducto == "")
                                     && (A.v_NroPedido == pstrNumeroPedido || pstrNumeroPedido == "")
                                     && I.d_StockActual > 0
                                     && C.i_EsServicio == 0
                                     && (C.v_IdMarca == IdMarca || IdMarca == "-1")
                                     && (D.v_OrigenTipo == Constants.OrigenGuiaCompra)
                                     && C.i_EsActivoFijo == 0
                                     select new GuiasComprasNoConsideradas
                                     {

                                         GuiasNoseTomaronCuenta = D.v_OrigenTipo + " " + D.v_OrigenRegMes + " " + D.v_OrigenRegCorrelativo + "     Periodo : " + D.v_OrigenRegPeriodo + "   Proveedor : " + E.v_ApePaterno + " " + E.v_ApeMaterno + " " + E.v_PrimerNombre + " " + E.v_RazonSocial,
                                         IdAlmacen = D.i_IdAlmacenOrigen.Value,
                                         v_IdLinea = C.v_IdLinea,
                                     }).ToList().AsQueryable();
                        if (!string.IsNullOrEmpty(pstrFilterExpression))
                        {
                            query = query.Where(pstrFilterExpression);
                        }


                        return GuiasNoConsideradas = query.ToList().Select(x => x.GuiasNoseTomaronCuenta + " \n").ToList();

                    }
                    else if (IncluirPedido == 1 && ArticulosStockMayor0 == 0)
                    {

                        var query = (from A in dbContext.movimientodetalle
                                     join B in dbContext.productodetalle on new { IdProductoDetalle = A.v_IdProductoDetalle, eliminado = 0 } equals new { IdProductoDetalle = B.v_IdProductoDetalle, eliminado = B.i_Eliminado.Value } into B_join
                                     from B in B_join.DefaultIfEmpty()
                                     join C in dbContext.producto on new { IdProducto = B.v_IdProducto, eliminado = 0 } equals new { IdProducto = C.v_IdProducto, eliminado = C.i_Eliminado.Value } into C_join
                                     from C in C_join.DefaultIfEmpty()
                                     join D in dbContext.movimiento on new { IdMovimiento = A.v_IdMovimiento, eliminado = 0 } equals new { IdMovimiento = D.v_IdMovimiento, eliminado = D.i_Eliminado.Value } into D_join
                                     from D in D_join.DefaultIfEmpty()
                                     join E in dbContext.cliente on new { IdCliente = D.v_IdCliente, eliminado = 0 } equals new { IdCliente = E.v_IdCliente, eliminado = E.i_Eliminado.Value } into E_join
                                     from E in E_join.DefaultIfEmpty()
                                     join F in dbContext.establecimientoalmacen on new { IdAlmacenOrigen = D.i_IdAlmacenOrigen, eliminado = 0 } equals new { IdAlmacenOrigen = F.i_IdAlmacen, eliminado = F.i_Eliminado.Value } into F_join
                                     from F in F_join.DefaultIfEmpty()
                                     join G in dbContext.almacen on new { IdAlmacen = F.i_IdAlmacen.Value, eliminado = 0 } equals new { IdAlmacen = G.i_IdAlmacen, eliminado = G.i_Eliminado.Value } into G_join
                                     from G in G_join.DefaultIfEmpty()
                                     join H in dbContext.documento on new { IdTipoDoc = A.i_IdTipoDocumento.Value, eliminado = 0 } equals new { IdTipoDoc = H.i_CodigoDocumento, eliminado = H.i_Eliminado.Value } into H_join
                                     from H in H_join.DefaultIfEmpty()
                                     join I in dbContext.datahierarchy on new { Grupo = 17, eliminado = 0, UnidadMedida = C.i_IdUnidadMedida.Value } equals new { Grupo = I.i_GroupId, eliminado = I.i_IsDeleted.Value, UnidadMedida = I.i_ItemId } into I_join
                                     from I in I_join.DefaultIfEmpty()
                                     where A.i_Eliminado == 0 && D.i_IdTipoMovimiento != (int)TipoDeMovimiento.Transferencia
                                     && (D.t_Fecha.Value >= pdtFechaInicio && D.t_Fecha.Value <= pdtFechaFin.Value) && F.i_IdEstablecimiento == idEstablecimiento && (C.v_CodInterno == pstrCodigoProducto || pstrCodigoProducto == "") && (A.v_NroPedido == pstrNumeroPedido || pstrNumeroPedido == "")
                                     && C.i_EsServicio == 0
                                      && (C.v_IdMarca == IdMarca || IdMarca == "-1")
                                      && (D.v_OrigenTipo == Constants.OrigenGuiaCompra)
                                        && C.i_EsActivoFijo == 0
                                     select new GuiasComprasNoConsideradas
                                     {
                                         GuiasNoseTomaronCuenta = D.v_OrigenTipo + " " + D.v_OrigenRegMes + " " + D.v_OrigenRegCorrelativo + "     Periodo : " + D.v_OrigenRegPeriodo + "   Proveedor : " + E.v_ApePaterno + " " + E.v_ApeMaterno + " " + E.v_PrimerNombre + " " + E.v_RazonSocial,
                                         IdAlmacen = D.i_IdAlmacenOrigen.Value,
                                         v_IdLinea = C.v_IdLinea,

                                     }).ToList().AsQueryable();

                        if (!string.IsNullOrEmpty(pstrFilterExpression))
                        {
                            query = query.Where(pstrFilterExpression);
                        }
                        return GuiasNoConsideradas = query.ToList().Select(x => x.GuiasNoseTomaronCuenta + " \n").ToList();
                    }
                    else if (IncluirPedido == 0 && ArticulosStockMayor0 == 0)
                    {

                        var query = (from A in dbContext.movimientodetalle
                                     join B in dbContext.productodetalle on new { IdProductoDetalle = A.v_IdProductoDetalle, eliminado = 0 } equals new { IdProductoDetalle = B.v_IdProductoDetalle, eliminado = B.i_Eliminado.Value } into B_join
                                     from B in B_join.DefaultIfEmpty()
                                     join C in dbContext.producto on new { IdProducto = B.v_IdProducto, eliminado = 0 } equals new { IdProducto = C.v_IdProducto, eliminado = C.i_Eliminado.Value } into C_join
                                     from C in C_join.DefaultIfEmpty()
                                     join D in dbContext.movimiento on new { IdMovimiento = A.v_IdMovimiento, eliminado = 0 } equals new { IdMovimiento = D.v_IdMovimiento, eliminado = D.i_Eliminado.Value } into D_join
                                     from D in D_join.DefaultIfEmpty()
                                     join E in dbContext.cliente on new { IdCliente = D.v_IdCliente, eliminado = 0 } equals new { IdCliente = E.v_IdCliente, eliminado = E.i_Eliminado.Value } into E_join
                                     from E in E_join.DefaultIfEmpty()
                                     join F in dbContext.establecimientoalmacen on new { IdAlmacen = D.i_IdAlmacenOrigen, eliminado = 0 } equals new { IdAlmacen = F.i_IdAlmacen, eliminado = F.i_Eliminado.Value } into F_join
                                     from F in F_join.DefaultIfEmpty()
                                     join G in dbContext.almacen on new { pIntAlmacen = F.i_IdAlmacen.Value, eliminado = 0 } equals new { pIntAlmacen = G.i_IdAlmacen, eliminado = G.i_Eliminado.Value } into G_join
                                     from G in G_join.DefaultIfEmpty()
                                     join H in dbContext.documento on new { IdTipoDoc = A.i_IdTipoDocumento.Value, eliminado = 0 } equals new { IdTipoDoc = H.i_CodigoDocumento, eliminado = H.i_Eliminado.Value } into H_join
                                     from H in H_join.DefaultIfEmpty()
                                     join I in dbContext.datahierarchy on new { Grupo = 17, eliminado = 0, Um = C.i_IdUnidadMedida.Value } equals new { Grupo = I.i_GroupId, eliminado = I.i_IsDeleted.Value, Um = I.i_ItemId } into I_join
                                     from I in I_join.DefaultIfEmpty()
                                     where A.i_Eliminado == 0 && D.i_IdTipoMovimiento != (int)TipoDeMovimiento.Transferencia
                                     && (D.t_Fecha.Value >= pdtFechaInicio && D.t_Fecha.Value <= pdtFechaFin) && F.i_IdEstablecimiento == idEstablecimiento && (C.v_CodInterno == pstrCodigoProducto || pstrCodigoProducto == "") && (A.v_NroPedido == pstrNumeroPedido || pstrNumeroPedido == "")
                                     && C.i_EsServicio == 0
                                      && (C.v_IdMarca == IdMarca || IdMarca == "-1")
                                      && (D.v_OrigenTipo == Constants.OrigenGuiaCompra)
                                        && C.i_EsActivoFijo == 0
                                     select new GuiasComprasNoConsideradas
                                     {
                                         GuiasNoseTomaronCuenta = D.v_OrigenTipo + " " + D.v_OrigenRegMes + " " + D.v_OrigenRegCorrelativo + "     Periodo : " + D.v_OrigenRegPeriodo + "   Proveedor : " + E.v_ApePaterno + " " + E.v_ApeMaterno + " " + E.v_PrimerNombre + " " + E.v_RazonSocial,
                                         IdAlmacen = D.i_IdAlmacenOrigen.Value,
                                         v_IdLinea = C.v_IdLinea,
                                     }).ToList().AsQueryable();


                        if (!string.IsNullOrEmpty(pstrFilterExpression))
                        {
                            query = query.Where(pstrFilterExpression);
                        }

                        return GuiasNoConsideradas = query.ToList().Select(x => x.GuiasNoseTomaronCuenta + " \n").ToList();

                    }
                    else if (IncluirPedido == 0 && ArticulosStockMayor0 == 1)
                    {
                        var query = (from A in dbContext.movimientodetalle
                                     join B in dbContext.productodetalle on new { IdProductoDetalle = A.v_IdProductoDetalle, eliminado = 0 } equals new { IdProductoDetalle = B.v_IdProductoDetalle, eliminado = B.i_Eliminado.Value } into B_join
                                     from B in B_join.DefaultIfEmpty()
                                     join C in dbContext.producto on new { IdProducto = B.v_IdProducto, eliminado = 0 } equals new { IdProducto = C.v_IdProducto, eliminado = C.i_Eliminado.Value } into C_join
                                     from C in C_join.DefaultIfEmpty()
                                     join D in dbContext.movimiento on new { IdMovimiento = A.v_IdMovimiento, eliminado = 0 } equals new { IdMovimiento = D.v_IdMovimiento, eliminado = D.i_Eliminado.Value } into D_join
                                     from D in D_join.DefaultIfEmpty()
                                     join E in dbContext.cliente on new { IdCliente = D.v_IdCliente, eliminado = 0 } equals new { IdCliente = E.v_IdCliente, eliminado = E.i_Eliminado.Value } into E_join
                                     from E in E_join.DefaultIfEmpty()
                                     join F in dbContext.establecimientoalmacen on new { AlmacenOrigen = D.i_IdAlmacenOrigen, eliminado = 0 } equals new { AlmacenOrigen = F.i_IdAlmacen, eliminado = F.i_Eliminado.Value } into F_join
                                     from F in F_join.DefaultIfEmpty()
                                     join G in dbContext.almacen on new { IdAlmacen = F.i_IdAlmacen.Value, eliminado = 0 } equals new { IdAlmacen = G.i_IdAlmacen, eliminado = G.i_Eliminado.Value } into G_join

                                     from G in G_join.DefaultIfEmpty()
                                     join H in dbContext.documento on new { IdTipoDoc = A.i_IdTipoDocumento.Value, eliminado = 0 } equals new { IdTipoDoc = H.i_CodigoDocumento, eliminado = H.i_Eliminado.Value } into H_join
                                     from H in H_join.DefaultIfEmpty()

                                     join I in dbContext.productoalmacen on new { IdProd = A.v_IdProductoDetalle, IdAlmcen = D.i_IdAlmacenOrigen.Value, eliminado = 0, per = Periodo } equals new { IdProd = I.v_ProductoDetalleId, IdAlmcen = I.i_IdAlmacen, eliminado = I.i_Eliminado.Value, per = I.v_Periodo } into I_join
                                     from I in I_join.DefaultIfEmpty()
                                     join K in dbContext.datahierarchy on new { Grupo = 17, eliminado = 0, Um = C.i_IdUnidadMedida.Value } equals new { Grupo = K.i_GroupId, eliminado = K.i_IsDeleted.Value, Um = K.i_ItemId } into K_join

                                     from K in K_join.DefaultIfEmpty()
                                     where A.i_Eliminado == 0 && D.i_IdTipoMovimiento != (int)TipoDeMovimiento.Transferencia
                                     && (D.t_Fecha.Value >= pdtFechaInicio && D.t_Fecha.Value <= pdtFechaFin) && F.i_IdEstablecimiento == idEstablecimiento && (C.v_CodInterno == pstrCodigoProducto || pstrCodigoProducto == "") && (A.v_NroPedido == pstrNumeroPedido || pstrNumeroPedido == "")
                                     && I.d_StockActual > 0
                                     && C.i_EsServicio == 0
                                      && (C.v_IdMarca == IdMarca || IdMarca == "-1")
                                     && (D.v_OrigenTipo == Constants.OrigenGuiaCompra)
                                       && C.i_EsActivoFijo == 0

                                     select new GuiasComprasNoConsideradas
                                     {
                                         GuiasNoseTomaronCuenta = D.v_OrigenTipo + " " + D.v_OrigenRegMes + " " + D.v_OrigenRegCorrelativo + "     Periodo : " + D.v_OrigenRegPeriodo + "   Proveedor : " + E.v_ApePaterno + " " + E.v_ApeMaterno + " " + E.v_PrimerNombre + " " + E.v_RazonSocial,
                                         IdAlmacen = D.i_IdAlmacenOrigen.Value,
                                         v_IdLinea = C.v_IdLinea,
                                     }).ToList().AsQueryable();


                        if (!string.IsNullOrEmpty(pstrFilterExpression))
                        {
                            query = query.Where(pstrFilterExpression);
                        }

                        return GuiasNoConsideradas = query.ToList().Select(x => x.GuiasNoseTomaronCuenta + " \n").ToList();

                    }

                    return null;
                    #endregion

                }
            }
            catch (Exception ex)
            {

                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ProductoBL.IdentificarGuias";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }
        public List<KardexList> ReporteKardexPorAlmacen(ref OperationResult pobjOperationResult, DateTime? pdtFechaInicio, DateTime? pdtFechaFin, int pintIdMoneda, int pintIdAlmacen)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    List<KardexList> Lista = new List<KardexList>();
                    KardexList oKardexList;
                    string IdProductoOld = "";

                    var query = (from A in dbContext.movimientodetalle
                                 join B in dbContext.productodetalle on A.v_IdProductoDetalle equals B.v_IdProductoDetalle
                                 join C in dbContext.producto on B.v_IdProducto equals C.v_IdProducto
                                 join D in dbContext.movimiento on A.v_IdMovimiento equals D.v_IdMovimiento
                                 join E in dbContext.cliente on D.v_IdCliente equals E.v_IdCliente
                                 where A.i_Eliminado == 0 && D.i_IdAlmacenOrigen == pintIdAlmacen
                                 && (D.t_Fecha.Value >= pdtFechaInicio && D.t_Fecha.Value <= pdtFechaFin)
                                 orderby C.v_Descripcion, D.t_Fecha
                                 select new KardexList
                                 {
                                     v_IdMovimientoDetalle = A.v_IdMovimientoDetalle,
                                     v_IdProducto = C.v_IdProducto,
                                     v_NombreProducto = C.v_Descripcion,
                                     t_Fecha = D.t_Fecha,
                                     i_IdTipoMovimiento = D.i_IdTipoMovimiento,
                                     d_Cantidad = A.d_Cantidad,
                                     d_Precio = A.d_Precio,
                                     i_EsDevolucion = D.i_EsDevolucion,
                                     i_IdTipoMotivo = D.i_IdTipoMotivo,
                                     NroPedido = A.v_NroPedido,
                                     v_IdLinea = C.v_IdLinea,
                                     IdMoneda = D.i_IdMoneda.Value,
                                     TipoCambio = D.d_TipoCambio.Value,
                                     ClienteProveedor = E.v_RazonSocial,
                                     Guia = A.v_NroGuiaRemision,
                                     Documento = A.v_NumeroDocumento,
                                     IdAlmacen = D.i_IdAlmacenOrigen.Value
                                 });

                    var queryList = query.ToList();
                    int Contador = query.Count();

                    for (int i = 0; i < Contador; i++)
                    {
                        oKardexList = new KardexList();
                        oKardexList.v_IdProducto = queryList[i].v_IdProducto;
                        oKardexList.v_NombreProducto = queryList[i].v_NombreProducto;
                        oKardexList.t_Fecha = queryList[i].t_Fecha;
                        oKardexList.v_Fecha = queryList[i].t_Fecha.Value.ToString("dd-MMM");
                        oKardexList.v_NombreTipoMovimiento = queryList[i].i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso ? "INGRESO" : "SALIDA";
                        oKardexList.i_IdTipoMotivo = queryList[i].i_IdTipoMotivo;
                        oKardexList.Documento = queryList[i].Documento;
                        oKardexList.IdMoneda = queryList[i].IdMoneda;
                        oKardexList.TipoCambio = queryList[i].TipoCambio;
                        oKardexList.i_EsDevolucion = queryList[i].i_EsDevolucion;
                        oKardexList.ClienteProveedor = queryList[i].ClienteProveedor;
                        oKardexList.Empresa = queryList[i].Empresa;
                        oKardexList.Ruc = queryList[i].Ruc;
                        oKardexList.Moneda = queryList[i].Moneda;
                        oKardexList.Almacen = queryList[i].Almacen;
                        oKardexList.Mes = queryList[i].Mes;
                        oKardexList.Al = queryList[i].Al;
                        oKardexList.Guia = queryList[i].Guia;
                        oKardexList.Documento = queryList[i].Documento;

                        if (pintIdMoneda == (int)Currency.Soles)
                        {
                            if (queryList[i].i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso)
                            {
                                oKardexList.Ingreso_Cantidad = oKardexList.i_EsDevolucion == 1 ? (queryList[i].d_Cantidad * -1) : queryList[i].d_Cantidad;
                                oKardexList.Ingreso_Precio = pintIdMoneda == oKardexList.IdMoneda ? queryList[i].d_Precio : queryList[i].d_Precio * oKardexList.TipoCambio;
                                oKardexList.Ingreso_Total = (oKardexList.Ingreso_Cantidad * oKardexList.Ingreso_Precio);

                                if (i == 0)
                                {
                                    oKardexList.Saldo_Cantidad = queryList[i].d_Cantidad;
                                    oKardexList.Saldo_Total = oKardexList.Ingreso_Total;
                                    oKardexList.Saldo_Precio = oKardexList.Ingreso_Precio;
                                }
                                else
                                {
                                    if (IdProductoOld != queryList[i].v_IdProducto)
                                    {
                                        oKardexList.Saldo_Cantidad = queryList[i].d_Cantidad;
                                        oKardexList.Saldo_Precio = pintIdMoneda == oKardexList.IdMoneda ? queryList[i].d_Precio : queryList[i].d_Precio * oKardexList.TipoCambio;
                                        oKardexList.Saldo_Total = (oKardexList.Saldo_Cantidad * oKardexList.Saldo_Precio);
                                    }
                                    else
                                    {
                                        oKardexList.Saldo_Cantidad = Lista[i - 1].Saldo_Cantidad + oKardexList.Ingreso_Cantidad;
                                        oKardexList.Saldo_Total = (Lista[i - 1].Saldo_Total + oKardexList.Ingreso_Total);
                                        oKardexList.Saldo_Precio = (oKardexList.Saldo_Total / oKardexList.Saldo_Cantidad);
                                    }
                                }
                            }
                            else
                            {
                                oKardexList.Salida_Cantidad = queryList[i].d_Cantidad;
                                if (i != 0) oKardexList.Salida_Precio = Lista[i - 1].Saldo_Precio;
                                oKardexList.Salida_Total = (oKardexList.Salida_Cantidad * oKardexList.Salida_Precio);

                                if (i == 0)
                                {
                                    oKardexList.Saldo_Cantidad = oKardexList.Salida_Cantidad;
                                    oKardexList.Saldo_Total = oKardexList.Salida_Total;
                                    oKardexList.Saldo_Precio = oKardexList.Salida_Precio;
                                }
                                else
                                {
                                    if (IdProductoOld != queryList[i].v_IdProducto)
                                    {
                                        oKardexList.Saldo_Cantidad = queryList[i].d_Cantidad;
                                        oKardexList.Saldo_Precio = pintIdMoneda == oKardexList.IdMoneda ? queryList[i].d_Precio : queryList[i].d_Precio * oKardexList.TipoCambio;
                                        oKardexList.Saldo_Total = oKardexList.Saldo_Cantidad * oKardexList.Saldo_Precio;
                                    }
                                    else
                                    {
                                        oKardexList.Saldo_Cantidad = Lista[i - 1].Saldo_Cantidad - queryList[i].d_Cantidad;
                                        oKardexList.Saldo_Total = (Lista[i - 1].Saldo_Total - oKardexList.Salida_Total);
                                        oKardexList.Saldo_Precio = oKardexList.Saldo_Cantidad == 0 ? 0 : (oKardexList.Saldo_Total / oKardexList.Saldo_Cantidad);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (queryList[i].i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso)
                            {
                                oKardexList.Ingreso_Cantidad = oKardexList.i_EsDevolucion == 1 ? (queryList[i].d_Cantidad * -1) : queryList[i].d_Cantidad;
                                oKardexList.Ingreso_Precio = pintIdMoneda == oKardexList.IdMoneda ? queryList[i].d_Precio : queryList[i].d_Precio / oKardexList.TipoCambio;
                                oKardexList.Ingreso_Total = (oKardexList.Ingreso_Cantidad * oKardexList.Ingreso_Precio);

                                if (i == 0)
                                {
                                    oKardexList.Saldo_Cantidad = queryList[i].d_Cantidad;
                                    oKardexList.Saldo_Total = oKardexList.Ingreso_Total;
                                    oKardexList.Saldo_Precio = oKardexList.Ingreso_Precio;
                                }
                                else
                                {
                                    if (IdProductoOld != queryList[i].v_IdProducto)
                                    {
                                        oKardexList.Saldo_Cantidad = queryList[i].d_Cantidad;
                                        oKardexList.Saldo_Precio = pintIdMoneda == oKardexList.IdMoneda ? queryList[i].d_Precio : queryList[i].d_Precio / oKardexList.TipoCambio;
                                        oKardexList.Saldo_Total = (oKardexList.Saldo_Cantidad * oKardexList.Saldo_Precio);
                                    }
                                    else
                                    {
                                        oKardexList.Saldo_Cantidad = Lista[i - 1].Saldo_Cantidad + oKardexList.Ingreso_Cantidad;
                                        oKardexList.Saldo_Total = (Lista[i - 1].Saldo_Total + oKardexList.Ingreso_Total);
                                        oKardexList.Saldo_Precio = (oKardexList.Saldo_Total / oKardexList.Saldo_Cantidad);
                                    }

                                }
                            }
                            else
                            {
                                oKardexList.Salida_Cantidad = queryList[i].d_Cantidad;
                                if (i != 0) oKardexList.Salida_Precio = Lista[i - 1].Saldo_Precio;
                                oKardexList.Salida_Total = (oKardexList.Salida_Precio * oKardexList.Salida_Cantidad);

                                if (i == 0)
                                {
                                    oKardexList.Saldo_Cantidad = queryList[i].d_Cantidad;
                                    oKardexList.Saldo_Total = oKardexList.Salida_Total;
                                    oKardexList.Saldo_Precio = oKardexList.Salida_Precio;
                                }
                                else
                                {
                                    if (IdProductoOld != queryList[i].v_IdProducto)
                                    {
                                        oKardexList.Saldo_Cantidad = queryList[i].d_Cantidad;
                                        oKardexList.Saldo_Precio = pintIdMoneda == oKardexList.IdMoneda ? queryList[i].d_Precio : queryList[i].d_Precio / oKardexList.TipoCambio;
                                        oKardexList.Saldo_Total = oKardexList.Saldo_Cantidad * oKardexList.Saldo_Precio;
                                    }
                                    else
                                    {
                                        oKardexList.Saldo_Cantidad = Lista[i - 1].Saldo_Cantidad - queryList[i].d_Cantidad;
                                        oKardexList.Saldo_Total = (Lista[i - 1].Saldo_Total - oKardexList.Salida_Total);
                                        oKardexList.Saldo_Precio = oKardexList.Saldo_Cantidad == 0 ? 0 : (oKardexList.Saldo_Total / oKardexList.Saldo_Cantidad);
                                    }
                                }
                            }
                        }
                        IdProductoOld = queryList[i].v_IdProducto;
                        Lista.Add(oKardexList);
                    }
                    return Lista;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ProductoBL.ReporteKardexPorAlmacen()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }


        }

        public List<KardexList> SaldosIniciales(string Periodo, string CodigoProducto)
        {

            using (var dbContext = new SAMBHSEntitiesModelWin())
            {

                var SaldosIniciales = (from a in dbContext.movimientodetalle

                                       join b in dbContext.movimiento on new { mov = a.v_IdMovimiento, eliminado = 0 } equals new { mov = b.v_IdMovimiento, eliminado = b.i_Eliminado.Value } into b_join
                                       from b in b_join.DefaultIfEmpty()
                                       join c in dbContext.productodetalle on new { pd = a.v_IdProductoDetalle, eliminado = 0 } equals new { pd = c.v_IdProductoDetalle, eliminado = c.i_Eliminado.Value } into c_join
                                       from c in c_join.DefaultIfEmpty()

                                       where b.i_IdTipoMotivo == (int)TipoDeMovimiento.Inicial
                                       && b.v_Periodo == Periodo
                                       && (c.producto.v_CodInterno == CodigoProducto || CodigoProducto == "")

                                       && b.i_IdAlmacenOrigen == Globals.ClientSession.i_IdAlmacenPredeterminado.Value
                                       select new KardexList
                                       {
                                           Saldo_Cantidad = a.d_CantidadEmpaque,

                                       }).ToList();

                return SaldosIniciales;
            }
        }

        public List<CargaInicial> CargaInicial(ref OperationResult pobjOperationResult, DateTime? pdtFechaInicio, DateTime? pdtFechaFin, int pintIdMoneda, int pintIdAlmacen)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    List<CargaInicial> Lista = new List<CargaInicial>();
                    CargaInicial oKardexList;
                    string IdProductoOld = "";

                    List<CargaInicial> query = (from A in dbContext.movimientodetalle

                                                join B in dbContext.productodetalle on new { pd = A.v_IdProductoDetalle, eliminado = 0 } equals new { pd = B.v_IdProductoDetalle, eliminado = B.i_Eliminado.Value } into B_join
                                                from B in B_join.DefaultIfEmpty()

                                                join C in dbContext.producto on new { p = B.v_IdProducto, eliminado = 0 } equals new { p = C.v_IdProducto, eliminado = C.i_Eliminado.Value } into C_join

                                                from C in C_join.DefaultIfEmpty()
                                                join D in dbContext.movimiento on new { m = A.v_IdMovimiento, eliminado = 0 } equals new { m = D.v_IdMovimiento, eliminado = D.i_Eliminado.Value } into D_join

                                                from D in D_join.DefaultIfEmpty()
                                                join E in dbContext.cliente on new { c = D.v_IdCliente, eliminado = 0 } equals new { c = E.v_IdCliente, eliminado = E.i_Eliminado.Value } into E_join

                                                from E in E_join.DefaultIfEmpty()
                                                join F in dbContext.datahierarchy on new { a = A.i_IdUnidad.Value, b = 17, eliminado = 0 }
                                                                                   equals new { a = F.i_ItemId, b = F.i_GroupId, eliminado = F.i_IsDeleted.Value } into F_join
                                                from F in F_join.DefaultIfEmpty()

                                                join G in dbContext.datahierarchy on new { um = A.i_IdUnidad.Value, grupo = 17, eliminado = 0 } equals new { um = G.i_ItemId, grupo = G.i_GroupId, eliminado = G.i_IsDeleted.Value } into G_join

                                                from G in G_join.DefaultIfEmpty()

                                                where A.i_Eliminado == 0 && D.i_IdAlmacenOrigen == pintIdAlmacen && D.i_Eliminado == 0 && C.i_EsActivoFijo == 0
                                                && (D.t_Fecha.Value >= pdtFechaInicio && D.t_Fecha.Value <= pdtFechaFin) && D.i_IdTipoMovimiento != (int)TipoDeMovimiento.Transferencia

                                                && C.v_IdProducto != "N002-PD000000000"

                                                && C.i_EsServicio == 0
                                                select new CargaInicial
                                                {
                                                    v_IdMovimientoDetalle = A.v_IdMovimientoDetalle,
                                                    v_IdProducto = B.v_IdProductoDetalle,
                                                    v_NombreProducto = C.v_Descripcion,
                                                    t_Fecha = D.t_Fecha,
                                                    i_IdTipoMovimiento = D.i_IdTipoMovimiento,
                                                    d_Cantidad = A.d_Cantidad,
                                                    d_CantidadEmpaque = A.d_CantidadEmpaque,
                                                    d_Precio = D.v_OrigenTipo == "I" ? pintIdMoneda == D.i_IdMoneda.Value ? A.d_Total / A.d_CantidadEmpaque : A.d_TotalCambio / A.d_CantidadEmpaque : A.d_Total / A.d_CantidadEmpaque,
                                                    // d_Precio = C.d_Empaque.Value == 1 && F.v_Value2 == "1" ? A.d_Precio : A.d_CantidadEmpaque == 0 ? 0 : A.d_Total / A.d_CantidadEmpaque, //Se calcula el precio unitario de acuerdo a su cantidad  Empaque
                                                    i_EsDevolucion = D.i_EsDevolucion,
                                                    i_IdTipoMotivo = D.i_IdTipoMotivo,
                                                    NroPedido = A.v_NroPedido == null || A.v_NroPedido == string.Empty ? null : A.v_NroPedido,
                                                    v_IdLinea = C.v_IdLinea,
                                                    IdMoneda = D.i_IdMoneda.Value,
                                                    TipoCambio = D.d_TipoCambio.Value,
                                                    ClienteProveedor = E.v_RazonSocial,
                                                    Guia = A.v_NroGuiaRemision,
                                                    Documento = A.v_NumeroDocumento,
                                                    IdAlmacen = D.i_IdAlmacenOrigen.Value,
                                                    i_IdUnidad = C.i_IdUnidadMedida.Value,      // C.d_Empaque.Value == 1 && F.v_Value2 == "1" ? C.i_IdUnidadMedida.Value : 15,
                                                    ValorUM = G.v_Value2 == "" ? "1" : G.v_Value2,
                                                    CodigoProducto = C.v_CodInterno.Trim(),
                                                    Origen = D.v_OrigenTipo,

                                                }).ToList().AsQueryable().ToList();

                    query = query.ToList().OrderBy(x => x.v_IdProducto).ThenBy(x => x.t_Fecha).ThenBy(x => x.i_IdTipoMovimiento).ThenBy(x => x.NroPedido).ToList();
                    var queryList = query.ToList();
                    int Contador = query.Count();
                    decimal UltimoPrecioSaldo = 0;
                    decimal UltimoPrecioSaldoEmpaque = 0;
                    for (int i = 0; i < Contador; i++)
                    {
                        oKardexList = new CargaInicial();
                        oKardexList.v_IdProducto = queryList[i].v_IdProducto.Trim();
                        oKardexList.CodigoProducto = queryList[i].CodigoProducto.Trim();
                        oKardexList.v_NombreProducto = queryList[i].v_NombreProducto.Trim();
                        oKardexList.t_Fecha = queryList[i].t_Fecha;
                        oKardexList.v_Fecha = queryList[i].t_Fecha.Value.ToString("dd-MMM");
                        oKardexList.v_NombreTipoMovimiento = queryList[i].i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso ? "INGRESO" : "SALIDA";
                        oKardexList.i_IdTipoMotivo = queryList[i].i_IdTipoMotivo;
                        oKardexList.Documento = queryList[i].Documento;
                        oKardexList.IdMoneda = queryList[i].IdMoneda;
                        oKardexList.TipoCambio = queryList[i].TipoCambio;
                        oKardexList.i_EsDevolucion = queryList[i].i_EsDevolucion;
                        oKardexList.Moneda = queryList[i].Moneda;
                        oKardexList.Almacen = queryList[i].Almacen;
                        oKardexList.Guia = queryList[i].Guia;
                        oKardexList.Documento = queryList[i].Documento;
                        oKardexList.i_IdUnidad = queryList[i].i_IdUnidad;
                        oKardexList.NroPedido = queryList[i].NroPedido;

                        oKardexList.DValorUM = decimal.Parse(queryList[i].ValorUM);

                        if (pintIdMoneda == (int)Currency.Soles)
                        {
                            if (queryList[i].i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso)
                            {
                                #region Nota Ingreso
                                oKardexList.Ingreso_CantidadEmpaque = oKardexList.i_EsDevolucion == 1 ? (queryList[i].d_CantidadEmpaque * -1) : queryList[i].d_CantidadEmpaque;
                                oKardexList.Ingreso_PrecioEmpaque = pintIdMoneda == oKardexList.IdMoneda ? queryList[i].d_Precio : queryList[i].Origen == "I" ? queryList[i].d_Precio : queryList[i].d_Precio * oKardexList.TipoCambio;
                                oKardexList.Ingreso_TotalEmpaque = (oKardexList.Ingreso_CantidadEmpaque * oKardexList.Ingreso_PrecioEmpaque);
                                oKardexList.Ingreso_Cantidad = oKardexList.i_EsDevolucion == 1 ? (queryList[i].d_Cantidad * -1) : queryList[i].d_Cantidad;
                                oKardexList.Ingreso_Precio = pintIdMoneda == oKardexList.IdMoneda ? queryList[i].d_Precio : queryList[i].Origen == "I" ? queryList[i].d_Precio : queryList[i].d_Precio * oKardexList.TipoCambio;
                                oKardexList.Ingreso_Total = (oKardexList.Ingreso_Cantidad * oKardexList.Ingreso_Precio);
                                if (i == 0)
                                {
                                    oKardexList.Saldo_Cantidad = queryList[i].d_Cantidad;
                                    oKardexList.Saldo_Total = oKardexList.Ingreso_Total;
                                    oKardexList.Saldo_Precio = oKardexList.Ingreso_Precio;

                                    oKardexList.Saldo_CantidadEmpaque = queryList[i].d_CantidadEmpaque;
                                    oKardexList.Saldo_TotalEmpaque = oKardexList.Ingreso_TotalEmpaque;
                                    oKardexList.Saldo_PrecioEmpaque = oKardexList.Ingreso_PrecioEmpaque;

                                    if (oKardexList.Saldo_Cantidad != 0 && oKardexList.Saldo_Precio != 0)
                                    {
                                        UltimoPrecioSaldo = oKardexList.Saldo_Precio ?? 0;
                                        UltimoPrecioSaldoEmpaque = oKardexList.Saldo_PrecioEmpaque ?? 0;
                                    }


                                }
                                else
                                {
                                    if (IdProductoOld != queryList[i].v_IdProducto + " " + queryList[i].NroPedido)
                                    {
                                        oKardexList.Saldo_Cantidad = queryList[i].d_Cantidad;
                                        oKardexList.Saldo_Precio = pintIdMoneda == oKardexList.IdMoneda ? queryList[i].d_Precio : queryList[i].Origen == "I" ? queryList[i].d_Precio : queryList[i].d_Precio * oKardexList.TipoCambio;
                                        oKardexList.Saldo_Total = (oKardexList.Saldo_Cantidad * oKardexList.Saldo_Precio);
                                        oKardexList.Saldo_CantidadEmpaque = queryList[i].d_CantidadEmpaque;

                                        oKardexList.Saldo_PrecioEmpaque = pintIdMoneda == oKardexList.IdMoneda ? queryList[i].d_Precio : queryList[i].Origen == "I" ? queryList[i].d_Precio : queryList[i].d_Precio * oKardexList.TipoCambio;
                                        oKardexList.Saldo_TotalEmpaque = (oKardexList.Saldo_CantidadEmpaque * oKardexList.Saldo_PrecioEmpaque);
                                        UltimoPrecioSaldo = (oKardexList.Saldo_Cantidad != 0 && oKardexList.Saldo_Precio != 0) ? oKardexList.Saldo_Precio.Value : 0;
                                        UltimoPrecioSaldoEmpaque = (oKardexList.Saldo_CantidadEmpaque != 0 && oKardexList.Saldo_PrecioEmpaque != 0) ? oKardexList.Saldo_PrecioEmpaque.Value : 0;
                                    }
                                    else
                                    {
                                        oKardexList.Saldo_Cantidad = Lista[i - 1].Saldo_Cantidad + oKardexList.Ingreso_Cantidad;
                                        oKardexList.Saldo_Total = (Lista[i - 1].Saldo_Total + oKardexList.Ingreso_Total);
                                        oKardexList.Saldo_Precio = oKardexList.Saldo_Cantidad == 0 ? 0 : (oKardexList.Saldo_Total / oKardexList.Saldo_Cantidad);
                                        oKardexList.Saldo_CantidadEmpaque = Lista[i - 1].Saldo_CantidadEmpaque + oKardexList.Ingreso_CantidadEmpaque;
                                        oKardexList.Saldo_TotalEmpaque = (Lista[i - 1].Saldo_TotalEmpaque + oKardexList.Ingreso_TotalEmpaque);
                                        oKardexList.Saldo_PrecioEmpaque = oKardexList.Saldo_CantidadEmpaque == 0 ? 0 : (oKardexList.Saldo_TotalEmpaque / oKardexList.Saldo_CantidadEmpaque);

                                        if (oKardexList.Saldo_Cantidad != 0 && oKardexList.Saldo_Precio != 0)
                                        {
                                            UltimoPrecioSaldo = oKardexList.Saldo_Precio ?? 0;
                                            UltimoPrecioSaldoEmpaque = oKardexList.Saldo_PrecioEmpaque ?? 0;
                                        }
                                    }
                                }

                                #endregion
                            }
                            else
                            {

                                #region NotaSalida
                                oKardexList.Salida_Cantidad = queryList[i].d_Cantidad;
                                oKardexList.Salida_Total = (oKardexList.Salida_Cantidad * oKardexList.Salida_Precio);
                                if (i != 0)
                                {
                                    oKardexList.Salida_Precio = Lista[i - 1].Saldo_Precio == 0 ? oKardexList.Salida_Precio = UltimoPrecioSaldo : Lista[i - 1].Saldo_Precio;
                                    oKardexList.Salida_PrecioEmpaque = Lista[i - 1].Saldo_PrecioEmpaque == 0 ? oKardexList.Salida_PrecioEmpaque = UltimoPrecioSaldoEmpaque : Lista[i - 1].Saldo_PrecioEmpaque;
                                }

                                oKardexList.Salida_CantidadEmpaque = queryList[i].d_CantidadEmpaque;
                                oKardexList.Salida_TotalEmpaque = (oKardexList.Salida_CantidadEmpaque * oKardexList.Salida_PrecioEmpaque);


                                if (i == 0)
                                {
                                    oKardexList.Saldo_Cantidad = oKardexList.Salida_Cantidad;
                                    oKardexList.Saldo_Total = oKardexList.Salida_Total;
                                    oKardexList.Saldo_Precio = oKardexList.Salida_Precio;


                                    oKardexList.Saldo_CantidadEmpaque = oKardexList.Salida_CantidadEmpaque;
                                    oKardexList.Saldo_TotalEmpaque = oKardexList.Salida_TotalEmpaque;
                                    oKardexList.Saldo_PrecioEmpaque = oKardexList.Salida_PrecioEmpaque;

                                    if (oKardexList.Saldo_Cantidad != 0 && oKardexList.Saldo_Precio != 0)
                                    {
                                        UltimoPrecioSaldo = oKardexList.Saldo_Precio ?? 0;
                                        UltimoPrecioSaldoEmpaque = oKardexList.Saldo_PrecioEmpaque ?? 0;
                                    }
                                }
                                else
                                {
                                    if (IdProductoOld != queryList[i].v_IdProducto + " " + queryList[i].NroPedido)
                                    {
                                        oKardexList.Saldo_Cantidad = queryList[i].d_Cantidad;
                                        oKardexList.Saldo_Precio = pintIdMoneda == oKardexList.IdMoneda ? queryList[i].d_Precio : queryList[i].d_Precio * oKardexList.TipoCambio;
                                        oKardexList.Saldo_Total = oKardexList.Saldo_Cantidad * oKardexList.Saldo_Precio;

                                        oKardexList.Saldo_CantidadEmpaque = queryList[i].d_CantidadEmpaque;
                                        oKardexList.Saldo_PrecioEmpaque = pintIdMoneda == oKardexList.IdMoneda ? queryList[i].d_Precio : queryList[i].d_Precio * oKardexList.TipoCambio;
                                        oKardexList.Saldo_TotalEmpaque = oKardexList.Saldo_CantidadEmpaque * oKardexList.Saldo_PrecioEmpaque;
                                        UltimoPrecioSaldo = (oKardexList.Saldo_Cantidad != 0 && oKardexList.Saldo_Precio != 0) ? oKardexList.Saldo_Precio.Value : 0;
                                        UltimoPrecioSaldoEmpaque = (oKardexList.Saldo_CantidadEmpaque != 0 && oKardexList.Saldo_PrecioEmpaque != 0) ? oKardexList.Saldo_PrecioEmpaque.Value : 0;

                                    }
                                    else
                                    {
                                        oKardexList.Saldo_Cantidad = Lista[i - 1].Saldo_Cantidad - queryList[i].d_Cantidad;
                                        oKardexList.Saldo_Total = (Lista[i - 1].Saldo_Total - oKardexList.Salida_Total);
                                        oKardexList.Saldo_Precio = oKardexList.Saldo_Cantidad == 0 ? 0 : (oKardexList.Saldo_Total / oKardexList.Saldo_Cantidad);



                                        oKardexList.Saldo_CantidadEmpaque = Lista[i - 1].Saldo_CantidadEmpaque - queryList[i].d_CantidadEmpaque;
                                        oKardexList.Saldo_TotalEmpaque = (Lista[i - 1].Saldo_TotalEmpaque - oKardexList.Salida_TotalEmpaque);
                                        oKardexList.Saldo_PrecioEmpaque = oKardexList.Saldo_CantidadEmpaque == 0 ? 0 : (oKardexList.Saldo_TotalEmpaque / oKardexList.Saldo_CantidadEmpaque);
                                        if (oKardexList.Saldo_Cantidad != 0 && oKardexList.Saldo_Precio != 0)
                                        {
                                            UltimoPrecioSaldo = oKardexList.Saldo_Precio ?? 0;
                                            UltimoPrecioSaldoEmpaque = oKardexList.Saldo_PrecioEmpaque ?? 0;
                                        }
                                    }
                                }

                                #endregion
                            }
                        }
                        else
                        {
                            if (queryList[i].i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso)
                            {

                                #region Nota Ingreso
                                oKardexList.Ingreso_Cantidad = oKardexList.i_EsDevolucion == 1 ? (queryList[i].d_Cantidad * -1) : queryList[i].d_Cantidad;
                                oKardexList.Ingreso_Precio = pintIdMoneda == oKardexList.IdMoneda ? queryList[i].d_Precio : queryList[i].Origen == "I" ? queryList[i].d_Precio : queryList[i].d_Precio / oKardexList.TipoCambio;
                                oKardexList.Ingreso_Total = (oKardexList.Ingreso_Cantidad * oKardexList.Ingreso_Precio);
                                oKardexList.Ingreso_CantidadEmpaque = oKardexList.i_EsDevolucion == 1 ? (queryList[i].d_CantidadEmpaque * -1) : queryList[i].d_CantidadEmpaque;
                                oKardexList.Ingreso_PrecioEmpaque = pintIdMoneda == oKardexList.IdMoneda ? queryList[i].d_Precio : queryList[i].Origen == "I" ? queryList[i].d_Precio : queryList[i].d_Precio / oKardexList.TipoCambio;
                                oKardexList.Ingreso_TotalEmpaque = (oKardexList.Ingreso_CantidadEmpaque * oKardexList.Ingreso_PrecioEmpaque);

                                if (i == 0)
                                {
                                    oKardexList.Saldo_CantidadEmpaque = queryList[i].d_CantidadEmpaque;
                                    oKardexList.Saldo_TotalEmpaque = oKardexList.Ingreso_TotalEmpaque;
                                    oKardexList.Saldo_PrecioEmpaque = oKardexList.Ingreso_PrecioEmpaque;

                                    oKardexList.Saldo_Cantidad = queryList[i].d_Cantidad;
                                    oKardexList.Saldo_Total = oKardexList.Ingreso_Total;
                                    oKardexList.Saldo_Precio = oKardexList.Ingreso_Precio;
                                    if (oKardexList.Saldo_Cantidad != 0 && oKardexList.Saldo_Precio != 0)
                                    {
                                        UltimoPrecioSaldo = oKardexList.Saldo_Precio ?? 0;
                                        UltimoPrecioSaldoEmpaque = oKardexList.Saldo_PrecioEmpaque ?? 0;
                                    }
                                }
                                else
                                {
                                    if (IdProductoOld != queryList[i].v_IdProducto + " " + queryList[i].NroPedido)
                                    {
                                        oKardexList.Saldo_Cantidad = queryList[i].d_Cantidad;
                                        //oKardexList.Saldo_Precio = pintIdMoneda == oKardexList.IdMoneda ? queryList[i].d_Precio : queryList[i].d_Precio / oKardexList.TipoCambio;
                                        oKardexList.Saldo_Precio = pintIdMoneda == oKardexList.IdMoneda ? queryList[i].d_Precio : queryList[i].Origen == "I" ? queryList[i].d_Precio : queryList[i].d_Precio / oKardexList.TipoCambio;
                                        oKardexList.Saldo_Total = (oKardexList.Saldo_Cantidad * oKardexList.Saldo_Precio);

                                        oKardexList.Saldo_CantidadEmpaque = queryList[i].d_CantidadEmpaque;
                                        //oKardexList.Saldo_PrecioEmpaque = pintIdMoneda == oKardexList.IdMoneda ? queryList[i].d_Precio : queryList[i].d_Precio / oKardexList.TipoCambio;
                                        oKardexList.Saldo_PrecioEmpaque = pintIdMoneda == oKardexList.IdMoneda ? queryList[i].d_Precio : queryList[i].Origen == "I" ? queryList[i].d_Precio : queryList[i].d_Precio / oKardexList.TipoCambio;
                                        oKardexList.Saldo_TotalEmpaque = (oKardexList.Saldo_CantidadEmpaque * oKardexList.Saldo_PrecioEmpaque);
                                        UltimoPrecioSaldo = (oKardexList.Saldo_Cantidad != 0 && oKardexList.Saldo_Precio != 0) ? oKardexList.Saldo_Precio.Value : 0;
                                        UltimoPrecioSaldoEmpaque = oKardexList.Saldo_CantidadEmpaque != 0 && oKardexList.Saldo_PrecioEmpaque != 0 ? oKardexList.Saldo_PrecioEmpaque.Value : 0;
                                    }
                                    else
                                    {
                                        oKardexList.Saldo_Cantidad = Lista[i - 1].Saldo_Cantidad + oKardexList.Ingreso_Cantidad;
                                        oKardexList.Saldo_Total = (Lista[i - 1].Saldo_Total + oKardexList.Ingreso_Total);
                                        oKardexList.Saldo_Precio = oKardexList.Saldo_Cantidad == 0 ? 0 : (oKardexList.Saldo_Total / oKardexList.Saldo_Cantidad);


                                        oKardexList.Saldo_CantidadEmpaque = Lista[i - 1].Saldo_CantidadEmpaque + oKardexList.Ingreso_CantidadEmpaque;
                                        oKardexList.Saldo_TotalEmpaque = (Lista[i - 1].Saldo_TotalEmpaque + oKardexList.Ingreso_TotalEmpaque);
                                        oKardexList.Saldo_PrecioEmpaque = oKardexList.Saldo_CantidadEmpaque == 0 ? 0 : (oKardexList.Saldo_TotalEmpaque / oKardexList.Saldo_CantidadEmpaque);
                                        if (oKardexList.Saldo_Cantidad != 0 && oKardexList.Saldo_Precio != 0)
                                        {
                                            UltimoPrecioSaldo = oKardexList.Saldo_Precio ?? 0;
                                            UltimoPrecioSaldoEmpaque = oKardexList.Saldo_PrecioEmpaque ?? 0;
                                        }
                                    }

                                }

                                #endregion
                            }
                            else
                            {

                                #region Nota Salida


                                oKardexList.Salida_Cantidad = queryList[i].d_Cantidad;
                                oKardexList.Salida_Total = (oKardexList.Salida_Cantidad * oKardexList.Salida_Precio);
                                if (i != 0)
                                {
                                    oKardexList.Salida_Precio = Lista[i - 1].Saldo_Precio == 0 ? oKardexList.Salida_Precio = UltimoPrecioSaldo : Lista[i - 1].Saldo_Precio;
                                    oKardexList.Salida_PrecioEmpaque = Lista[i - 1].Saldo_PrecioEmpaque == 0 ? oKardexList.Salida_PrecioEmpaque = UltimoPrecioSaldoEmpaque : Lista[i - 1].Saldo_PrecioEmpaque;
                                }

                                oKardexList.Salida_CantidadEmpaque = queryList[i].d_CantidadEmpaque;
                                oKardexList.Salida_TotalEmpaque = (oKardexList.Salida_CantidadEmpaque * oKardexList.Salida_PrecioEmpaque);

                                if (i == 0)
                                {
                                    oKardexList.Saldo_Cantidad = queryList[i].d_Cantidad;
                                    oKardexList.Saldo_Total = oKardexList.Salida_Total;
                                    oKardexList.Saldo_Precio = oKardexList.Salida_Precio;

                                    oKardexList.Saldo_CantidadEmpaque = queryList[i].d_CantidadEmpaque;
                                    oKardexList.Saldo_TotalEmpaque = oKardexList.Salida_TotalEmpaque;
                                    oKardexList.Saldo_PrecioEmpaque = oKardexList.Salida_PrecioEmpaque;

                                    if (oKardexList.Saldo_Cantidad != 0 && oKardexList.Saldo_Precio != 0)
                                    {
                                        UltimoPrecioSaldo = oKardexList.Saldo_Precio ?? 0;
                                        UltimoPrecioSaldoEmpaque = oKardexList.Saldo_PrecioEmpaque ?? 0;
                                    }
                                }
                                else
                                {
                                    if (IdProductoOld != queryList[i].v_IdProducto + " " + queryList[i].NroPedido)
                                    {
                                        oKardexList.Saldo_Cantidad = queryList[i].d_Cantidad;
                                        oKardexList.Saldo_Precio = pintIdMoneda == oKardexList.IdMoneda ? queryList[i].d_Precio : queryList[i].d_Precio / oKardexList.TipoCambio;
                                        oKardexList.Saldo_Total = oKardexList.Saldo_Cantidad * oKardexList.Saldo_Precio;

                                        oKardexList.Saldo_CantidadEmpaque = queryList[i].d_CantidadEmpaque;
                                        oKardexList.Saldo_PrecioEmpaque = pintIdMoneda == oKardexList.IdMoneda ? queryList[i].d_Precio : queryList[i].d_Precio / oKardexList.TipoCambio;
                                        oKardexList.Saldo_TotalEmpaque = oKardexList.Saldo_CantidadEmpaque * oKardexList.Saldo_PrecioEmpaque;
                                        UltimoPrecioSaldo = (oKardexList.Saldo_Cantidad != 0 && oKardexList.Saldo_Precio != 0) ? oKardexList.Saldo_Precio.Value : 0;
                                        UltimoPrecioSaldoEmpaque = (oKardexList.Saldo_CantidadEmpaque != 0 && oKardexList.Saldo_PrecioEmpaque != 0) ? oKardexList.Saldo_PrecioEmpaque.Value : 0;
                                    }
                                    else
                                    {
                                        oKardexList.Saldo_Cantidad = Lista[i - 1].Saldo_Cantidad - queryList[i].d_Cantidad;
                                        oKardexList.Saldo_Total = (Lista[i - 1].Saldo_Total - oKardexList.Salida_Total);
                                        oKardexList.Saldo_Precio = oKardexList.Saldo_Cantidad == 0 ? 0 : (oKardexList.Saldo_Total / oKardexList.Saldo_Cantidad);

                                        oKardexList.Saldo_CantidadEmpaque = Lista[i - 1].Saldo_CantidadEmpaque - queryList[i].d_CantidadEmpaque;
                                        oKardexList.Saldo_TotalEmpaque = (Lista[i - 1].Saldo_Total - oKardexList.Salida_Total);
                                        oKardexList.Saldo_PrecioEmpaque = oKardexList.Saldo_CantidadEmpaque == 0 ? 0 : (oKardexList.Saldo_Total / oKardexList.Saldo_CantidadEmpaque);

                                        if (oKardexList.Saldo_Cantidad != 0 && oKardexList.Saldo_Precio != 0)
                                        {
                                            UltimoPrecioSaldo = oKardexList.Saldo_Precio ?? 0;
                                            UltimoPrecioSaldoEmpaque = oKardexList.Saldo_PrecioEmpaque ?? 0;
                                        }

                                    }
                                }

                                #endregion
                            }
                        }



                        IdProductoOld = queryList[i].v_IdProducto + " " + queryList[i].NroPedido;
                        Lista.Add(oKardexList);
                    }

                    return Lista;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ProductoBL.CargaInicial()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }


        }

        public List<KardexList> ReporteStockAntiguo(ref OperationResult pobjOperationResult, int Establecimiento, DateTime? pdtFechaInicio, DateTime? pdtFechaFin, string pstrFilterExpression, int pintIdMoneda, string pstrCodigoProducto, string Pedido, string Linea, int SoloStock0, int SoloStock, int SoloStockNegativo, int SoloStockMinimo, int ConsideraDocInternos, int FormatoCant, string Modelo, DateTime FechaRepResumenAlmaMov, int IncluirNroPedido, string IdMovimientoDetalleConsumo = null, bool AsientoConsumo = false, int TipoMovimiento = -1, bool RepResumenAlma = false, bool RepResumenMov = false, bool SeUsoCargaINICIAL = false)
        {
            // //Inicialmente en cantidad se toma todas las cantidadempaque , si son nulos (que no deberia pasar) se toma las cantidades
            // En  precio estoy capturando los totales de las entradas y/o salidas , luego el precio propiamente dicho se calculara dividiendo el total /cantidad empaque 
            DateTime FechaInicioReporte = DateTime.Parse("01/01/" + Globals.ClientSession.i_Periodo.ToString()); // Siempre debe ser 1 enero de cada año , solo cambio para Resumen Movimientos
            string IdProductoOld = "";
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    Stopwatch _timer = new Stopwatch();
                    _timer.Start();
                    List<KardexList> Lista = new List<KardexList>();
                    KardexList oKardexList;


                    List<KardexList> Movimientos = new List<KardexList>();
                    if (TipoMovimiento == -1)
                    {

                        Movimientos = (from A in dbContext.movimientodetalle
                                       join B in dbContext.productodetalle on new { IdProductoDetalle = A.v_IdProductoDetalle, eliminado = 0 } equals new { IdProductoDetalle = B.v_IdProductoDetalle, eliminado = B.i_Eliminado.Value } into B_join
                                       from B in B_join.DefaultIfEmpty()
                                       join C in dbContext.producto on new { IdProducto = B.v_IdProducto, eliminado = 0 } equals new { IdProducto = C.v_IdProducto, eliminado = C.i_Eliminado.Value } into C_join
                                       from C in C_join.DefaultIfEmpty()
                                       join D in dbContext.movimiento on new { IdMovimiento = A.v_IdMovimiento, eliminado = 0 } equals new { IdMovimiento = D.v_IdMovimiento, eliminado = D.i_Eliminado.Value } into D_join
                                       from D in D_join.DefaultIfEmpty()
                                       join E in dbContext.cliente on new { IdCliente = D.v_IdCliente, eliminado = 0 } equals new { IdCliente = E.v_IdCliente, eliminado = E.i_Eliminado.Value } into E_join
                                       from E in E_join.DefaultIfEmpty()
                                       join G in dbContext.almacen on new { IdAlmacen = D.i_IdAlmacenOrigen.Value, eliminado = 0 } equals new { IdAlmacen = G.i_IdAlmacen, eliminado = G.i_Eliminado.Value } into G_join
                                       from G in G_join.DefaultIfEmpty()

                                       join H in dbContext.documento on new { IdTipoDoc = A.i_IdTipoDocumento.Value, eliminado = 0 } equals new { IdTipoDoc = H.i_CodigoDocumento, eliminado = H.i_Eliminado.Value } into H_join
                                       from H in H_join.DefaultIfEmpty()

                                       join I in dbContext.datahierarchy on new { Grupo = 17, eliminado = 0, UnidadMedida = C.i_IdUnidadMedida.Value } equals new { Grupo = I.i_GroupId, eliminado = I.i_IsDeleted.Value, UnidadMedida = I.i_ItemId } into I_join
                                       from I in I_join.DefaultIfEmpty()

                                       join J1 in dbContext.datahierarchy on new { a = A.i_IdUnidad.Value, b = 17, eliminado = 0 }
                                                                    equals new { a = J1.i_ItemId, b = J1.i_GroupId, eliminado = J1.i_IsDeleted.Value } into J1_join
                                       from J1 in J1_join.DefaultIfEmpty()

                                       join J2 in dbContext.establecimiento on new { est = D.i_IdEstablecimiento.Value, eliminado = 0 } equals new { est = J2.i_IdEstablecimiento, eliminado = J2.i_IdEstablecimiento } into J2_join
                                       from J2 in J2_join.DefaultIfEmpty()

                                       join J3 in dbContext.datahierarchy on new { umProducto = C.i_IdUnidadMedida.Value, b = 17, eliminado = 0 }
                                                                    equals new { umProducto = J3.i_ItemId, b = J3.i_GroupId, eliminado = J3.i_IsDeleted.Value } into J3_join
                                       from J3 in J3_join.DefaultIfEmpty()

                                       join J4 in dbContext.marca on new { marc = C.v_IdMarca, eliminado = 0 } equals new { marc = J4.v_IdMarca, eliminado = J4.i_Eliminado.Value } into J4_join
                                       from J4 in J4_join.DefaultIfEmpty()

                                       where A.i_Eliminado == 0 && D.i_IdTipoMovimiento != (int)TipoDeMovimiento.Transferencia
                                       && (D.t_Fecha.Value >= pdtFechaInicio && D.t_Fecha.Value <= pdtFechaFin)
                                       && (C.v_CodInterno == pstrCodigoProducto || pstrCodigoProducto == "")
                                       && (A.v_NroPedido.Trim() == Pedido || Pedido == "")
                                       && C.i_EsServicio == 0
                                       && C.i_EsActivoFijo == 0
                                       && (D.i_IdEstablecimiento == Establecimiento)
                                       && B_join.Any(o => o.v_IdProductoDetalle == A.v_IdProductoDetalle)
                                       && B_join.Any(o => o.v_IdProducto == C.v_IdProducto)
                                       && (C.v_Modelo == Modelo || Modelo == "")
                                       && (C.v_IdLinea == Linea || Linea == "-1")
                                       && (D.v_OrigenTipo == null || D.v_OrigenTipo == "" || D.v_OrigenTipo == Constants.OrigenCompra || D.v_OrigenTipo == Constants.OrigenImportacion || D.v_OrigenTipo == Constants.OrigenTransferencia || D.v_OrigenTipo == Constants.OrigenVenta || D.v_OrigenTipo == Constants.OrigenGuiaInterna || (!string.IsNullOrEmpty(D.v_OrigenTipo) && D.v_OrigenTipo != Constants.OrigenGuiaCompra))


                                       select new KardexList
                                       {
                                           v_IdMovimientoDetalle = A == null ? "" : A.v_IdMovimientoDetalle,
                                           v_IdProducto = IncluirNroPedido == 1 ? A.v_NroPedido == null || A.v_NroPedido.Trim() == string.Empty ? C.v_IdProducto : C.v_IdProducto + " " + A.v_NroPedido.Trim() : C.v_IdProducto,
                                           v_NombreProducto = C == null ? "** PRODUCTO NO EXISTE **" : C.v_Descripcion,
                                           t_Fecha = D.t_Fecha,
                                           i_IdTipoMovimiento = D.i_IdTipoMovimiento,
                                           d_Cantidad = A == null ? 0 : A.d_CantidadEmpaque == null ? A.d_Cantidad == null ? 0 : A.d_Cantidad : A.d_CantidadEmpaque,
                                           d_Precio = D.v_OrigenTipo == "I" ? pintIdMoneda == D.i_IdMoneda.Value ? A.d_Total.Value : A.d_TotalCambio.Value : A.d_Total.Value,
                                           i_EsDevolucion = D.i_EsDevolucion,
                                           i_IdTipoMotivo = D.i_IdTipoMotivo,
                                           NroPedido = IncluirNroPedido == 1 ? A.v_NroPedido == null || A.v_NroPedido == string.Empty ? "" : A.v_NroPedido.Trim() : "",
                                           v_IdLinea = C == null ? "" : C.v_IdLinea,
                                           IdMoneda = D.i_IdMoneda.Value,
                                           TipoCambio = D.d_TipoCambio.Value,
                                           ClienteProveedor = E == null ? "" : (E.v_ApePaterno + " " + E.v_ApeMaterno + " " + E.v_PrimerNombre + " " + E.v_RazonSocial).Trim(),
                                           Guia = A.v_NroGuiaRemision,
                                           Documento = H.v_Siglas + " " + A.v_NumeroDocumento,
                                           CodProducto = C == null ? "" : C.v_CodInterno,
                                           IdAlmacen = D.i_IdAlmacenOrigen.Value,
                                           d_Empaque = C.d_Empaque,
                                           EsDocumentoInterno = H.i_UsadoDocumentoInterno == null ? 0 : H.i_UsadoDocumentoInterno.Value,
                                           ValorUM = I.v_Value2,
                                           v_IdMarca = C.v_IdMarca,
                                           Almacen = G.v_Nombre,
                                           TipoMotivo = D.i_IdTipoMotivo == 5 ? 1 : D.v_OrigenTipo == "T" ? 3 : 2,
                                           Origen = D.v_OrigenTipo,
                                           d_Total = D.v_OrigenTipo == "I" ? pintIdMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : A.d_Total,
                                           StockMinimo = C == null ? 0 : C.d_StockMinimo ?? 0,
                                           UnidadMedida = J1.v_Value1,
                                           UnidadMedidaProducto = J3.v_Value1,
                                           i_IdUnidadMedidaProducto = C.i_IdUnidadMedida ?? 0,
                                           i_Activo = C.i_EsActivo ?? 0,
                                           v_IdProductoDetalle = A.v_IdProductoDetalle,
                                           Modelo = C.v_Modelo,
                                           NroParte = C.v_NroParte,
                                           Ubicacion = C.v_Ubicacion,
                                           Marca = J4 == null ? "" : J4.v_Nombre,


                                       }).ToList();
                        if (RepResumenAlma)
                        {
                            //Cuando es Reporte Resumen Almacen y se visualiza los Movimientos ya no se toma en cuenta la Carga Inicial
                            //Porque se tomó en cuenta en el saldo Anterior
                            // Movimientos = Movimientos.Where(o => (o.i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso && o.i_IdTipoMotivo != (int)MotiveType.CARGAINICIAL)|| o.i_IdTipoMovimiento ==(int)TipoDeMovimiento.NotadeSalida).ToList();
                        }

                        var h = Movimientos.Where(o => o.CodProducto == "0001").ToList();

                    }
                    else
                    {
                        var PeriodoCargaInicial = pdtFechaInicio.Value.Year.ToString();
                        Movimientos = (from A in dbContext.movimientodetalle
                                       join B in dbContext.productodetalle on new { IdProductoDetalle = A.v_IdProductoDetalle, eliminado = 0 } equals new { IdProductoDetalle = B.v_IdProductoDetalle, eliminado = B.i_Eliminado.Value } into B_join
                                       from B in B_join.DefaultIfEmpty()
                                       join C in dbContext.producto on new { IdProducto = B.v_IdProducto, eliminado = 0 } equals new { IdProducto = C.v_IdProducto, eliminado = C.i_Eliminado.Value } into C_join
                                       from C in C_join.DefaultIfEmpty()
                                       join D in dbContext.movimiento on new { IdMovimiento = A.v_IdMovimiento, eliminado = 0 } equals new { IdMovimiento = D.v_IdMovimiento, eliminado = D.i_Eliminado.Value } into D_join
                                       from D in D_join.DefaultIfEmpty()
                                       join E in dbContext.cliente on new { IdCliente = D.v_IdCliente, eliminado = 0 } equals new { IdCliente = E.v_IdCliente, eliminado = E.i_Eliminado.Value } into E_join
                                       from E in E_join.DefaultIfEmpty()
                                       join G in dbContext.almacen on new { IdAlmacen = D.i_IdAlmacenOrigen.Value, eliminado = 0 } equals new { IdAlmacen = G.i_IdAlmacen, eliminado = G.i_Eliminado.Value } into G_join
                                       from G in G_join.DefaultIfEmpty()

                                       join H in dbContext.documento on new { IdTipoDoc = A.i_IdTipoDocumento.Value, eliminado = 0 } equals new { IdTipoDoc = H.i_CodigoDocumento, eliminado = H.i_Eliminado.Value } into H_join
                                       from H in H_join.DefaultIfEmpty()

                                       join I in dbContext.datahierarchy on new { Grupo = 17, eliminado = 0, UnidadMedida = C.i_IdUnidadMedida.Value } equals new { Grupo = I.i_GroupId, eliminado = I.i_IsDeleted.Value, UnidadMedida = I.i_ItemId } into I_join
                                       from I in I_join.DefaultIfEmpty()

                                       join J1 in dbContext.datahierarchy on new { a = A.i_IdUnidad.Value, b = 17, eliminado = 0 }
                                                                    equals new { a = J1.i_ItemId, b = J1.i_GroupId, eliminado = J1.i_IsDeleted.Value } into J1_join
                                       from J1 in J1_join.DefaultIfEmpty()

                                       join J2 in dbContext.establecimiento on new { est = D.i_IdEstablecimiento.Value, eliminado = 0 } equals new { est = J2.i_IdEstablecimiento, eliminado = J2.i_IdEstablecimiento } into J2_join
                                       from J2 in J2_join.DefaultIfEmpty()

                                       join J3 in dbContext.datahierarchy on new { umProducto = C.i_IdUnidadMedida.Value, b = 17, eliminado = 0 }
                                                                    equals new { umProducto = J3.i_ItemId, b = J3.i_GroupId, eliminado = J3.i_IsDeleted.Value } into J3_join
                                       from J3 in J3_join.DefaultIfEmpty()

                                       join J4 in dbContext.marca on new { marc = C.v_IdMarca, eliminado = 0 } equals new { marc = J4.v_IdMarca, eliminado = J4.i_Eliminado.Value } into J4_join
                                       from J4 in J4_join.DefaultIfEmpty()

                                       where A.i_Eliminado == 0 && D.i_IdTipoMovimiento != (int)TipoDeMovimiento.Transferencia
                                       && (C.v_CodInterno == pstrCodigoProducto || pstrCodigoProducto == "")
                                       && (A.v_NroPedido.Trim() == Pedido || Pedido == "")
                                       && C.i_EsServicio == 0
                                       && C.i_EsActivoFijo == 0
                                       && (D.i_IdEstablecimiento == Establecimiento)
                                       && B_join.Any(o => o.v_IdProductoDetalle == A.v_IdProductoDetalle)
                                       && B_join.Any(o => o.v_IdProducto == C.v_IdProducto)
                                       && (C.v_Modelo == Modelo || Modelo == "")
                                       && (C.v_IdLinea == Linea || Linea == "-1")
                                       && D.i_IdTipoMovimiento == TipoMovimiento
                                       && D.i_IdTipoMotivo == (int)MotiveType.CARGAINICIAL
                                       && (D.v_Periodo == PeriodoCargaInicial)
                                       && (D.v_OrigenTipo == null || D.v_OrigenTipo == "" || D.v_OrigenTipo == Constants.OrigenCompra || D.v_OrigenTipo == Constants.OrigenImportacion || D.v_OrigenTipo == Constants.OrigenTransferencia || D.v_OrigenTipo == Constants.OrigenVenta || D.v_OrigenTipo == Constants.OrigenGuiaInterna || (!string.IsNullOrEmpty(D.v_OrigenTipo) && D.v_OrigenTipo != Constants.OrigenGuiaCompra))

                                       select new KardexList
                                       {
                                           v_IdMovimientoDetalle = A == null ? "" : A.v_IdMovimientoDetalle,
                                           //v_IdProducto = A.v_NroPedido == null || A.v_NroPedido.Trim() == string.Empty ? C.v_IdProducto : C.v_IdProducto + " " + A.v_NroPedido.Trim(),
                                           v_IdProducto = IncluirNroPedido == 1 ? A.v_NroPedido == null || A.v_NroPedido.Trim() == string.Empty ? C.v_IdProducto : C.v_IdProducto + " " + A.v_NroPedido.Trim() : C.v_IdProducto,
                                           v_NombreProducto = C == null ? "** PRODUCTO NO EXISTE **" : C.v_Descripcion,
                                           t_Fecha = D.t_Fecha,
                                           i_IdTipoMovimiento = D.i_IdTipoMovimiento,
                                           d_Cantidad = A == null ? 0 : A.d_CantidadEmpaque == null ? A.d_Cantidad == null ? 0 : A.d_Cantidad : A.d_CantidadEmpaque,
                                           d_Precio = D.v_OrigenTipo == "I" ? pintIdMoneda == D.i_IdMoneda.Value ? A.d_Total.Value : A.d_TotalCambio.Value : A.d_Total.Value,
                                           i_EsDevolucion = D.i_EsDevolucion,
                                           i_IdTipoMotivo = D.i_IdTipoMotivo,
                                           NroPedido = A.v_NroPedido == null || A.v_NroPedido == string.Empty ? "" : A.v_NroPedido.Trim(),
                                           v_IdLinea = C == null ? "" : C.v_IdLinea,
                                           IdMoneda = D.i_IdMoneda.Value,
                                           TipoCambio = D.d_TipoCambio.Value,
                                           ClienteProveedor = E == null ? "" : (E.v_ApePaterno + " " + E.v_ApeMaterno + " " + E.v_PrimerNombre + " " + E.v_RazonSocial).Trim(),
                                           Guia = A.v_NroGuiaRemision,
                                           Documento = H.v_Siglas + " " + A.v_NumeroDocumento,
                                           CodProducto = C == null ? "" : C.v_CodInterno,
                                           IdAlmacen = D.i_IdAlmacenOrigen.Value,
                                           d_Empaque = C.d_Empaque,
                                           EsDocumentoInterno = H.i_UsadoDocumentoInterno == null ? 0 : H.i_UsadoDocumentoInterno.Value,
                                           ValorUM = I.v_Value2,
                                           v_IdMarca = C.v_IdMarca,
                                           Almacen = G.v_Nombre,
                                           TipoMotivo = D.i_IdTipoMotivo == 5 ? 1 : D.v_OrigenTipo == "T" ? 3 : 2,
                                           Origen = D.v_OrigenTipo,
                                           d_Total = D.v_OrigenTipo == "I" ? pintIdMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : A.d_Total,
                                           StockMinimo = C == null ? 0 : C.d_StockMinimo ?? 0,
                                           UnidadMedida = J1.v_Value1,
                                           UnidadMedidaProducto = J3.v_Value1,
                                           i_IdUnidadMedidaProducto = C.i_IdUnidadMedida ?? 0,
                                           i_Activo = C.i_EsActivo ?? 0,
                                           v_IdProductoDetalle = A.v_IdProductoDetalle,
                                           Modelo = C.v_Modelo,
                                           NroParte = C.v_NroParte,
                                           Ubicacion = C.v_Ubicacion,
                                           Marca = J4 == null ? "" : J4.v_Nombre,


                                       }).ToList();
                    }


                    _timer.Stop();
                    var gg = _timer.Elapsed;

                    if (!string.IsNullOrEmpty(pstrFilterExpression))
                    {
                        Movimientos = Movimientos.AsQueryable().Where(pstrFilterExpression).ToList();
                    }

                    if (AsientoConsumo)
                        Movimientos = Movimientos.AsQueryable().Where(h => h.v_IdMovimientoDetalle != IdMovimientoDetalleConsumo).ToList();
                    var queryList = Movimientos.ToList();
                    queryList = Movimientos.OrderBy(x => x.IdAlmacenOrigen).ThenBy(x => x.v_IdProducto).ThenBy(x => x.t_Fecha).ThenBy(x => x.i_IdTipoMovimiento).ThenBy(x => x.TipoMotivo).ThenBy(x => x.NroPedido).ThenBy(o => o.v_IdMovimientoDetalle).ToList();
                    queryList = queryList.Select(x => new KardexList
                    {
                        v_IdMovimientoDetalle = x.v_IdMovimientoDetalle,
                        v_IdProducto = x.v_IdProducto,
                        v_NombreProducto = x.v_NombreProducto.Trim(),
                        t_Fecha = x.t_Fecha,
                        i_IdTipoMovimiento = x.i_IdTipoMovimiento,
                        d_Cantidad = x.d_Cantidad == 0 || decimal.Parse(x.ValorUM) == 0 ? 0 : FormatoCant == (int)FormatoCantidad.UnidadMedidaProducto ? x.d_Cantidad / decimal.Parse(x.ValorUM) : x.d_Cantidad,
                        d_Precio = x.d_Cantidad == 0 || decimal.Parse(x.ValorUM) == 0 || x.d_Precio == 0 ? 0 : FormatoCant == (int)FormatoCantidad.UnidadMedidaProducto ? x.d_Cantidad == 0 ? 0 : x.d_Precio / (x.d_Cantidad / (decimal.Parse(x.ValorUM))) : x.d_Cantidad == 0 ? 0 : x.d_Precio / x.d_Cantidad,
                        i_EsDevolucion = x.i_EsDevolucion,
                        i_IdTipoMotivo = x.i_IdTipoMotivo,
                        NroPedido = x.NroPedido,
                        v_IdLinea = x.v_IdLinea,
                        IdMoneda = x.IdMoneda,
                        TipoCambio = x.TipoCambio,
                        ClienteProveedor = x.ClienteProveedor,
                        Guia = x.Guia,
                        Documento = x.Documento,
                        CodProducto = x.CodProducto,
                        UnidadMedida = x.UnidadMedida,
                        IdAlmacen = x.IdAlmacen,
                        d_Empaque = x.d_Empaque,
                        EsDocumentoInterno = x.EsDocumentoInterno,
                        Almacen = x.Almacen,
                        Origen = x.Origen,
                        d_Total = x.d_Total,
                        StockMinimo = x.StockMinimo,
                        UnidadMedidaProducto = x.UnidadMedidaProducto,
                        i_IdUnidadMedidaProducto = x.i_IdUnidadMedidaProducto,
                        i_Activo = x.i_Activo,
                        ValorUM = x.ValorUM,
                        v_IdProductoDetalle = x.v_IdProductoDetalle,
                        Modelo = x.Modelo,
                        NroParte = x.NroParte,
                        Ubicacion = x.Ubicacion,
                        Marca = x.Marca,
                        // GrupoLlave =x.GrupoLlave ,
                    }).ToList();

                    int Contador = queryList.Count;
                    decimal UltimoPrecio = 0;
                    int PosicionProducto = 0;
                    for (int i = 0; i < Contador; i++)
                    {
                        if (ConsideraDocInternos == 1 || (ConsideraDocInternos == 0 && queryList[i].EsDocumentoInterno == 0))
                        {
                            oKardexList = new KardexList();
                            oKardexList = (KardexList)queryList[i].Clone();
                            oKardexList.i_IdUnidad = FormatoCant == (int)FormatoCantidad.Unidades ? oKardexList.ValorUM == "1" ? oKardexList.i_IdUnidadMedidaProducto : 15 : oKardexList.i_IdUnidadMedidaProducto;
                            oKardexList.UnidadMedida = FormatoCant == (int)FormatoCantidad.Unidades ? "UNIDAD" : queryList.Where(x => x.CodProducto == oKardexList.CodProducto && x.UnidadMedidaProducto == "UNIDAD").Count() != 0 ? "UNIDAD" : queryList[i].UnidadMedidaProducto;//   queryList.Where(x => x.CodProducto == oKardexList.CodProducto && x.UnidadMedida == "UNIDAD").Count() != 0 ? "UNIDAD" : queryList[i].UnidadMedida;
                            oKardexList.v_Fecha = queryList[i].t_Fecha.Value.ToString("dd-MMM");
                            oKardexList.v_NombreTipoMovimiento = queryList[i].i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso ? "INGRESO" : "SALIDA";
                            oKardexList.Almacen = "ALMACÉN : " + queryList[i].Almacen.ToUpper();
                            if (i == 0)
                            {

                                if ((oKardexList.i_IdTipoMotivo != 5 && oKardexList.i_IdTipoMovimiento == 1) || oKardexList.i_IdTipoMovimiento == 2)
                                {
                                    KardexList oKardexList1 = new KardexList();
                                    oKardexList1 = (KardexList)oKardexList.Clone();
                                    oKardexList1.v_NombreTipoMovimiento = "INICIAL";
                                    DateTime date = DateTime.Parse("01/" + pdtFechaInicio.Value.Month.ToString("00") + "/" + pdtFechaInicio.Value.Year.ToString());
                                    oKardexList1.t_Fecha = date;
                                    oKardexList1.DocumentoInventarioSunat = "";
                                    oKardexList1.v_Fecha = date.ToString("dd-MMM");
                                    oKardexList1.DocumentoInventarioSunat = "";
                                    oKardexList1.Guia = "";
                                    oKardexList1.Documento = "";
                                    oKardexList1.ClienteProveedor = "";
                                    oKardexList1.Ingreso_Cantidad = 0;
                                    oKardexList1.Ingreso_Precio = 0;
                                    oKardexList1.Ingreso_Total = 0;
                                    oKardexList1.Salida_Cantidad = null;
                                    oKardexList1.i_IdTipoMotivo = 5;
                                    oKardexList1.Salida_Precio = 0;
                                    oKardexList1.Salida_Total = 0;
                                    oKardexList1.Saldo_Cantidad = 0;
                                    oKardexList1.Saldo_Precio = 0;
                                    oKardexList1.Saldo_Total = 0;
                                    oKardexList1.NumeroElemento = 0;
                                    oKardexList1.i_IdTipoMovimiento = 1;
                                    Lista.Add(oKardexList1);
                                    PosicionProducto = PosicionProducto + 1;


                                    IdProductoOld = oKardexList1.v_IdProducto;

                                }

                            }
                            CantidadValorizadas objProceso = new CantidadValorizadas();
                            objProceso = Globals.ClientSession.i_Periodo == 2016 ? ProcesoValorizacionProductoPedido2016(queryList[i].i_IdTipoMovimiento.Value, IdProductoOld, queryList[i].v_IdProducto, queryList[i].IdAlmacen.ToString(), pintIdMoneda, queryList[i].Origen, queryList[i].i_EsDevolucion ?? 0, queryList[i].d_Cantidad.Value, queryList[i].IdMoneda, queryList[i].d_Precio ?? 0, PosicionProducto, queryList[i].TipoCambio, ref  UltimoPrecio, -1, Lista, queryList[i].d_Total ?? 0) : ProcesoValorizacionProductoPedido2017(queryList[i].i_IdTipoMovimiento.Value, IdProductoOld, queryList[i].v_IdProducto, queryList[i].IdAlmacen.ToString(), pintIdMoneda, queryList[i].Origen, queryList[i].i_EsDevolucion ?? 0, queryList[i].d_Cantidad.Value, queryList[i].IdMoneda, queryList[i].d_Precio ?? 0, PosicionProducto, queryList[i].TipoCambio, ref  UltimoPrecio, -1, Lista, queryList[i].d_Total ?? 0);

                            oKardexList.Ingreso_Cantidad = objProceso.Ingreso_Cantidad;
                            oKardexList.Ingreso_Precio = objProceso.Ingreso_Precio;
                            oKardexList.Ingreso_Total = objProceso.Ingreso_Total;
                            oKardexList.Saldo_Cantidad = objProceso.Saldo_Cantidad;
                            oKardexList.Saldo_Precio = objProceso.Saldo_Precio;
                            oKardexList.Saldo_Total = objProceso.Saldo_Total;
                            oKardexList.Salida_Cantidad = objProceso.Salida_Cantidad;
                            oKardexList.Salida_Precio = objProceso.Salida_Precio;
                            oKardexList.Salida_Total = objProceso.Salida_Total;

                            oKardexList.Saldo_CantidadExcel = Utils.Windows.DevuelveValorRedondeado(objProceso.Saldo_Cantidad.Value, Globals.ClientSession.i_CantidadDecimales.Value) == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado(objProceso.Saldo_Cantidad.Value, Globals.ClientSession.i_CantidadDecimales.Value);
                            oKardexList.Saldo_PrecioExcel = Utils.Windows.DevuelveValorRedondeado(objProceso.Saldo_Precio.Value, Globals.ClientSession.i_PrecioDecimales.Value) == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado(objProceso.Saldo_Precio.Value, Globals.ClientSession.i_PrecioDecimales.Value);
                            oKardexList.Saldo_TotalExcel = Utils.Windows.DevuelveValorRedondeado(objProceso.Saldo_Total.Value, Globals.ClientSession.i_PrecioDecimales.Value) == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado(objProceso.Saldo_Total.Value, Globals.ClientSession.i_PrecioDecimales.Value);
                            oKardexList.StockFisico = queryList[i].StockMinimo;
                            oKardexList.NumeroElemento = objProceso.NumeroElemento;
                            IdProductoOld = queryList[i].v_IdProducto;
                            if (i + 1 < Contador && IdProductoOld == queryList[i + 1].v_IdProducto)
                            {
                                ///Se agrego para valorizacion sunat
                                int Reg = Lista.Count();
                                //Se agrego para los saldos iniciales 
                                if (PosicionProducto == 0 && Reg > 0 && Lista[Reg - 1].v_IdProducto != oKardexList.v_IdProducto)
                                {
                                    if (int.Parse(pdtFechaInicio.Value.Month.ToString()) > 1)
                                    {
                                        KardexList oKardexList1 = new KardexList();
                                        oKardexList1 = (KardexList)queryList[i + 1].Clone();
                                        if (oKardexList1.v_IdProducto == "N001-PD000000527")
                                        {
                                            string h = "";
                                        }
                                        oKardexList1.v_NombreTipoMovimiento = "INICIAL";
                                        DateTime date = DateTime.Parse("01/" + pdtFechaInicio.Value.Month.ToString("00") + "/" + pdtFechaInicio.Value.Year.ToString());
                                        oKardexList1.t_Fecha = date;
                                        oKardexList1.DocumentoInventarioSunat = "";
                                        oKardexList1.v_Fecha = date.ToString("dd-MMM");
                                        oKardexList1.DocumentoInventarioSunat = "";
                                        oKardexList1.Guia = "";
                                        oKardexList1.Documento = "";
                                        oKardexList1.ClienteProveedor = "";
                                        oKardexList1.Ingreso_Cantidad = 0;
                                        oKardexList1.Ingreso_Precio = 0;
                                        oKardexList1.Ingreso_Total = 0;
                                        oKardexList1.Salida_Cantidad = null;
                                        oKardexList1.i_IdTipoMotivo = 5;
                                        oKardexList1.Salida_Precio = 0;
                                        oKardexList1.Salida_Total = 0;
                                        oKardexList1.Saldo_Cantidad = 0;
                                        oKardexList1.Saldo_Precio = 0;
                                        oKardexList1.Saldo_Total = 0;
                                        oKardexList1.NumeroElemento = 0;
                                        oKardexList1.i_IdTipoMovimiento = 1;
                                        Lista.Add(oKardexList1);
                                        PosicionProducto = PosicionProducto + 1;
                                        IdProductoOld = oKardexList1.v_IdProducto;
                                    }

                                }

                                // Hasta Acá;

                                PosicionProducto = PosicionProducto + 1;
                                Lista.Add(oKardexList);

                            }
                            else
                            {
                                PosicionProducto = 0;
                                UltimoPrecio = 0;
                                Lista.Add(oKardexList);
                                if (i + 1 < Contador)// !SeInsertoLista
                                {
                                    KardexList oKardexList1 = new KardexList();
                                    oKardexList1 = (KardexList)queryList[i + 1].Clone();
                                    //var ExisteListaReporte = Lista.Where(l => l.v_IdProducto == oKardexList1.v_IdProducto && l.NroPedido == oKardexList1.NroPedido).ToList();
                                    if (((queryList[i + 1].i_IdTipoMotivo != 5 && queryList[i + 1].i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso)) || queryList[i + 1].i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeSalida)
                                    {
                                        oKardexList1.v_NombreTipoMovimiento = "INICIAL";
                                        DateTime date = DateTime.Parse("01/" + pdtFechaInicio.Value.Month.ToString("00") + "/" + pdtFechaInicio.Value.Year.ToString());
                                        oKardexList1.v_Fecha = date.ToString("dd-MMM");
                                        oKardexList1.t_Fecha = date;
                                        oKardexList1.DocumentoInventarioSunat = "";
                                        oKardexList1.Guia = "";
                                        oKardexList1.Documento = "";
                                        oKardexList1.ClienteProveedor = "";
                                        oKardexList1.Ingreso_Cantidad = 0;
                                        oKardexList1.Ingreso_Precio = 0;
                                        oKardexList1.Ingreso_Total = 0;
                                        oKardexList1.Salida_Cantidad = null;
                                        oKardexList1.i_IdTipoMotivo = 5;
                                        oKardexList1.Salida_Precio = 0;
                                        oKardexList1.Salida_Total = 0;
                                        oKardexList1.Saldo_Cantidad = 0;
                                        oKardexList1.Saldo_Precio = 0;
                                        oKardexList1.Saldo_Total = 0;
                                        oKardexList1.NumeroElemento = 0;
                                        // oKardexList1.TipoOperacionInventarioSunat = int.Parse(TipoMovimientos.Where(l => l.i_ItemId == 5).FirstOrDefault().v_Value2).ToString("00");
                                        oKardexList1.i_IdTipoMovimiento = 1;
                                        Lista.Add(oKardexList1);
                                        PosicionProducto = PosicionProducto + 1;
                                        IdProductoOld = oKardexList1.v_IdProducto;
                                    }
                                }
                            }
                        }
                    }

                    if (RepResumenAlma || RepResumenMov)
                    {

                        if (FechaRepResumenAlmaMov.Month.ToString("00") != "01")
                        {
                            var MesAnterior = int.Parse(FechaRepResumenAlmaMov.Month.ToString()) - 1;
                            var FechaAnt = DateTime.Parse(DateTime.DaysInMonth(Globals.ClientSession.i_Periodo.Value, MesAnterior).ToString() + "/" + MesAnterior + "/" + Globals.ClientSession.i_Periodo.ToString() + " 23:59");
                            List<KardexList> Sald = new List<KardexList>();
                            Sald = Lista.Select(item => (KardexList)item.Clone()).ToList();
                            var ListaSaldosAnt = Sald.ToList().Where(l => l.t_Fecha <= FechaAnt).GroupBy(l => l.v_NombreProducto).ToList().Select(d =>
                            {
                                var k = d.LastOrDefault();
                                k.v_NombreTipoMovimiento = "INICIAL";
                                DateTime date = DateTime.Parse("01/" + FechaRepResumenAlmaMov.Month.ToString("00") + "/" + FechaRepResumenAlmaMov.Year.ToString()); // DateTime.Parse("01/" + pdtFechaInicio.Value.Month.ToString("00") + "/" + pdtFechaInicio.Value.Year.ToString());
                                k.v_Fecha = date.ToString("dd-MMM");
                                k.t_Fecha = date;
                                k.Guia = "";
                                k.Documento = "";
                                k.ClienteProveedor = "";
                                k.Salida_Cantidad = null;
                                k.Ingreso_Cantidad = null;
                                k.Ingreso_Precio = null;
                                k.Ingreso_Total = null;
                                k.Salida_Cantidad = null;
                                k.Salida_Precio = null;
                                k.Salida_Total = null;
                                k.i_IdTipoMotivo = 5;
                                k.DocumentoInventarioSunat = "";
                                k.TipoOperacionInventarioSunat = "16";
                                return k;
                            }).ToList();


                            var Lista2 = Lista.Where(l => l.t_Fecha > FechaAnt).ToList();
                            Lista = new List<KardexList>();
                            Lista = ListaSaldosAnt.Concat(Lista2).ToList();
                        }

                    }
                    else
                    {
                        if (FechaInicioReporte.Month.ToString("00") != "01")
                        {


                            var MesAnterior = int.Parse(FechaInicioReporte.Month.ToString()) - 1;
                            var FechaAnt = DateTime.Parse(DateTime.DaysInMonth(Globals.ClientSession.i_Periodo.Value, MesAnterior).ToString() + "/" + MesAnterior + "/" + Globals.ClientSession.i_Periodo.ToString() + " 23:59");

                            List<KardexList> Sald = new List<KardexList>();
                            Sald = Lista.Select(item => (KardexList)item.Clone()).ToList();

                            var ListaSaldosAnt = Sald.ToList().Where(l => l.t_Fecha <= FechaAnt).GroupBy(l => l.v_NombreProducto).ToList().Select(d =>
                            {
                                if (d.LastOrDefault().v_IdProducto == "N001-PD000000527")
                                {
                                    string h = "";
                                }
                                var k = d.LastOrDefault();
                                k.v_NombreTipoMovimiento = "INICIAL";
                                DateTime date = DateTime.Parse("01/" + FechaInicioReporte.Month.ToString("00") + "/" + FechaInicioReporte.Year.ToString()); // DateTime.Parse("01/" + pdtFechaInicio.Value.Month.ToString("00") + "/" + pdtFechaInicio.Value.Year.ToString());
                                k.v_Fecha = date.ToString("dd-MMM");
                                k.t_Fecha = date;
                                k.Guia = "";
                                k.Documento = "";
                                k.ClienteProveedor = "";
                                k.Salida_Cantidad = null;
                                k.Ingreso_Cantidad = null;
                                k.Ingreso_Precio = null;
                                k.Ingreso_Total = null;
                                k.Salida_Cantidad = null;
                                k.Salida_Precio = null;
                                k.Salida_Total = null;
                                k.i_IdTipoMotivo = 5;
                                k.DocumentoInventarioSunat = "";
                                k.TipoOperacionInventarioSunat = "16";
                                return k;
                            }).ToList();
                            var Lista2 = Lista.Where(l => l.t_Fecha > FechaAnt).ToList();
                            Lista = new List<KardexList>();
                            Lista = ListaSaldosAnt.Concat(Lista2).ToList();
                        }
                    }

                    List<KardexList> ListaAgrupada = new List<KardexList>();
                    if (RepResumenAlma)
                    {

                        //Para Reporte Resumen Almacen ya no se toma en cuenta carga Inicial
                        //Debido a que se esta tomando en una columnaInicial
                        if (IncluirNroPedido == 1)
                        {
                            ListaAgrupada = Lista.Where(o => o.t_Fecha >= FechaRepResumenAlmaMov).GroupBy(x => new { x.v_IdProducto, x.NroPedido }).ToList()
                               .Select(p =>
                           {
                               var k = p.LastOrDefault();
                               var CargaInicial = p.Where(o => (o.i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso && o.i_IdTipoMotivo == (int)MotiveType.CARGAINICIAL)).ToList();
                               var SumaInicialIngresos = CargaInicial.Any() ? CargaInicial.Sum(j => j.Saldo_Cantidad).Value : 0;
                               var SumaInicialPrecios = CargaInicial.Any() ? CargaInicial.Sum(j => j.Saldo_Precio).Value : 0;
                               var SumaInicialTotal = CargaInicial.Any() ? CargaInicial.Sum(d => d.Saldo_Total).Value : 0;
                               k.SumatoriaIngresos = SeUsoCargaINICIAL ? p.Sum(f => f.Ingreso_Cantidad ?? 0) - SumaInicialIngresos : p.Sum(f => f.Ingreso_Cantidad ?? 0);
                               k.SumatoriaSalidas = p.Sum(f => f.Salida_Cantidad ?? 0);
                               k.SumatoriaTotalIngresos = SeUsoCargaINICIAL ? p.Sum(f => f.Ingreso_Total ?? 0) - SumaInicialTotal : p.Sum(f => f.Ingreso_Total ?? 0);
                               k.SumatoriaTotalSalidas = p.Sum(f => f.Salida_Total ?? 0);
                               k.SumatoriaPreciosIngresos = SeUsoCargaINICIAL ? p.Sum(f => f.Ingreso_Precio ?? 0) - SumaInicialPrecios : p.Sum(f => f.Ingreso_Precio ?? 0);
                               k.SumatoriaPreciosSalidas = p.Sum(f => f.Salida_Precio ?? 0);
                               return k;


                           }).ToList();
                        }
                        else
                        {
                            ListaAgrupada = Lista.Where(o => o.t_Fecha >= FechaRepResumenAlmaMov).GroupBy(x => new { x.v_IdProducto }).ToList()
                                   .Select(p =>
                                   {
                                       var k = p.LastOrDefault();
                                       var CargaInicial = p.Where(o => (o.i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso && o.i_IdTipoMotivo == (int)MotiveType.CARGAINICIAL)).ToList();
                                       var SumaInicialIngresos = CargaInicial.Any() ? CargaInicial.Sum(j => j.Saldo_Cantidad).Value : 0;
                                       var SumaInicialPrecios = CargaInicial.Any() ? CargaInicial.Sum(j => j.Saldo_Precio).Value : 0;
                                       var SumaInicialTotal = CargaInicial.Any() ? CargaInicial.Sum(d => d.Saldo_Total).Value : 0;
                                       k.SumatoriaIngresos = SeUsoCargaINICIAL ? p.Sum(f => f.Ingreso_Cantidad ?? 0) - SumaInicialIngresos : p.Sum(f => f.Ingreso_Cantidad ?? 0);
                                       k.SumatoriaSalidas = p.Sum(f => f.Salida_Cantidad ?? 0);
                                       k.SumatoriaTotalIngresos = SeUsoCargaINICIAL ? p.Sum(f => f.Ingreso_Total ?? 0) - SumaInicialTotal : p.Sum(f => f.Ingreso_Total ?? 0);
                                       k.SumatoriaTotalSalidas = p.Sum(f => f.Salida_Total ?? 0);
                                       k.SumatoriaPreciosIngresos = SeUsoCargaINICIAL ? p.Sum(f => f.Ingreso_Precio ?? 0) - SumaInicialPrecios : p.Sum(f => f.Ingreso_Precio ?? 0);
                                       k.SumatoriaPreciosSalidas = p.Sum(f => f.Salida_Precio ?? 0);
                                       return k;


                                   }).ToList();

                        }



                    }
                    else if (RepResumenMov)
                    {


                        ListaAgrupada = Lista.GroupBy(x => new { x.v_IdProducto, x.i_IdTipoMovimiento }).ToList()
                               .Select(p =>
                               {
                                   var k = p.LastOrDefault();
                                   k.SumatoriaIngresos = p.Sum(f => f.Ingreso_Cantidad ?? 0);
                                   k.SumatoriaSalidas = p.Sum(f => f.Salida_Cantidad ?? 0);
                                   k.SumatoriaTotalIngresos = p.Sum(f => f.Ingreso_Total ?? 0);
                                   k.SumatoriaTotalSalidas = p.Sum(f => f.Salida_Total ?? 0);
                                   k.SumatoriaPreciosIngresos = p.Sum(f => f.Ingreso_Precio ?? 0);
                                   k.SumatoriaPreciosSalidas = p.Sum(f => f.Salida_Precio ?? 0);
                                   return k;


                               }).ToList();
                    }
                    else
                    {
                        if (IncluirNroPedido == 1)
                        {
                            ListaAgrupada = Lista.GroupBy(x => new { x.v_IdProducto, x.NroPedido })
                                        .Select(group => group.Last())
                                        .OrderBy(o => o.CodProducto).ToList();
                        }
                        else
                        {
                            ListaAgrupada = Lista.GroupBy(x => new { x.v_IdProducto })
                                           .Select(group => group.Last())
                                           .OrderBy(o => o.CodProducto).ToList();
                        }

                    }
                    pobjOperationResult.Success = 1;
                    if (SoloStock0 == 1)
                    {

                        return ListaAgrupada.Where(x => x.Saldo_Cantidad == 0).ToList().OrderBy(l => l.CodProducto).ToList();
                    }
                    else if (SoloStock == 1)
                    {

                        return ListaAgrupada.Where(x => x.Saldo_Cantidad > 0).ToList().OrderBy(l => l.CodProducto).ToList();
                    }
                    else if (SoloStockNegativo == 1)
                    {

                        return ListaAgrupada.Where(x => x.Saldo_Cantidad < 0).ToList().OrderBy(l => l.CodProducto).ToList();
                    }

                    else
                    {

                        return ListaAgrupada.OrderBy(l => l.CodProducto).ToList();
                    }
                }
            }

            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ProductoBL.ReporteStock() ,Producto :" + IdProductoOld;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public List<KardexList> ReporteStock(ref OperationResult pobjOperationResult, int Establecimiento, DateTime? pdtFechaInicio, DateTime? pdtFechaFin, string pstrFilterExpression, int pintIdMoneda, string pstrCodigoProducto, string Pedido, string Linea, int SoloStock0, int SoloStock, int SoloStockNegativo, int SoloStockMinimo, int ConsideraDocInternos, int FormatoCant, string Modelo, DateTime FechaRepResumenAlmaMov, int IncluirNroPedido, string IdMovimientoDetalleConsumo = null, bool AsientoConsumo = false, int TipoMovimiento = -1, bool RepResumenAlma = false, bool RepResumenMov = false, bool SeUsoCargaINICIAL = false,string Agrupar="")
        {
            // //Inicialmente en cantidad se toma todas las cantidadempaque , si son nulos (que no deberia pasar) se toma las cantidades
            // En  precio estoy capturando los totales de las entradas y/o salidas , luego el precio propiamente dicho se calculara dividiendo el total /cantidad empaque 
            DateTime FechaInicioReporte = DateTime.Parse("01/01/" + Globals.ClientSession.i_Periodo.ToString()); // Siempre debe ser 1 enero de cada año , solo cambio para Resumen Movimientos
            string IdProductoOld = "";
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    List<KardexList> Lista = new List<KardexList>();
                    KardexList oKardexList;
                    List<KardexList> Movimientos = new List<KardexList>();
                    List<movimientodetalle> movimientodetalle;

                    string queryToExecute = "";
                    if (TipoMovimiento == -1)
                    {
                        queryToExecute = ObtenerConsultaAnaliticoMovimientos(pdtFechaInicio.Value, pdtFechaFin.Value, pstrCodigoProducto);
                    }
                    else
                    {
                        queryToExecute = ObtenerConsultaAnaliticoMovimientosCargaInicial(pstrCodigoProducto);
                    }


                    switch (Globals.TipoMotor)
                    {
                        case TipoMotorBD.PostgreSQL:
                            using (var cnx = new PgSqlConnection(Globals.CadenaConexion))
                            {
                                movimientodetalle = cnx.Query<movimientodetalle>(queryToExecute).ToList();
                            }
                            break;

                        default:
                            using (var cnx = new SqlConnection(Globals.CadenaConexion))
                            {
                                movimientodetalle = cnx.Query<movimientodetalle>(queryToExecute).ToList();
                            }
                            break;
                    }



                    if (TipoMovimiento == -1)
                    {

                        Movimientos = (from A in movimientodetalle
                                       //, eliminado = 0 , eliminado = B.i_Eliminado.Value 
                                       join B in dbContext.productodetalle on new { IdProductoDetalle = A.v_IdProductoDetalle} equals new { IdProductoDetalle = B.v_IdProductoDetalle} into B_join
                                       from B in B_join.DefaultIfEmpty()
                                       join C in dbContext.producto on new { IdProducto = B.v_IdProducto, eliminado = 0 } equals new { IdProducto = C.v_IdProducto, eliminado = C.i_Eliminado.Value } into C_join
                                       from C in C_join.DefaultIfEmpty()
                                       join D in dbContext.movimiento on new { IdMovimiento = A.v_IdMovimiento, eliminado = 0 } equals new { IdMovimiento = D.v_IdMovimiento, eliminado = D.i_Eliminado.Value } into D_join
                                       from D in D_join.DefaultIfEmpty()
                                       join E in dbContext.cliente on new { IdCliente = D.v_IdCliente, eliminado = 0 } equals new { IdCliente = E.v_IdCliente, eliminado = E.i_Eliminado.Value } into E_join
                                       from E in E_join.DefaultIfEmpty()
                                       join G in dbContext.almacen on new { IdAlmacen = D.i_IdAlmacenOrigen.Value, eliminado = 0 } equals new { IdAlmacen = G.i_IdAlmacen, eliminado = G.i_Eliminado.Value } into G_join
                                       from G in G_join.DefaultIfEmpty()
                                       join H in dbContext.documento on new { IdTipoDoc = A.i_IdTipoDocumento.Value, eliminado = 0 } equals new { IdTipoDoc = H.i_CodigoDocumento, eliminado = H.i_Eliminado.Value } into H_join
                                       from H in H_join.DefaultIfEmpty()
                                       join I in dbContext.datahierarchy on new { Grupo = 17, eliminado = 0, UnidadMedida = C == null ? -1 : C.i_IdUnidadMedida ?? -1 } equals new { Grupo = I.i_GroupId, eliminado = I.i_IsDeleted.Value, UnidadMedida = I.i_ItemId } into I_join
                                       from I in I_join.DefaultIfEmpty()

                                       join J1 in dbContext.datahierarchy on new { a = A.i_IdUnidad ?? -1, b = 17, eliminado = 0 }
                                                                    equals new { a = J1 == null ? -1 : J1.i_ItemId, b = J1 == null ? -1 : J1.i_GroupId, eliminado = J1 == null ? 0 : J1.i_IsDeleted.Value } into J1_join
                                       from J1 in J1_join.DefaultIfEmpty()
                                       join J3 in dbContext.datahierarchy on new { umProducto = C == null ? -1 : C.i_IdUnidadMedida ?? -1, b = 17, eliminado = 0 }
                                                                    equals new { umProducto = J3.i_ItemId, b = J3.i_GroupId, eliminado = J3.i_IsDeleted.Value } into J3_join
                                       from J3 in J3_join.DefaultIfEmpty()
                                       join J4 in dbContext.marca on new { marc = C == null ? null : C.v_IdMarca, eliminado = 0 } equals new { marc = J4.v_IdMarca, eliminado = J4.i_Eliminado.Value } into J4_join
                                       from J4 in J4_join.DefaultIfEmpty()
                                       where C != null && D != null
                                       && A.i_Eliminado == 0 && D.i_IdTipoMovimiento != (int)TipoDeMovimiento.Transferencia
                                       && (D.t_Fecha.Value >= pdtFechaInicio && D.t_Fecha.Value <= pdtFechaFin)
                                       && (C.v_CodInterno == pstrCodigoProducto || pstrCodigoProducto == "")
                                       && (A.v_NroPedido == Pedido || Pedido == "")
                                       && C.i_EsServicio == 0
                                       && C.i_EsActivoFijo == 0
                                       && (D.i_IdEstablecimiento == Establecimiento)
                                       && (C.v_Modelo == Modelo || Modelo == "")
                                       && (C.v_IdLinea == Linea || Linea == "-1")
                                       && (D.v_OrigenTipo == null || D.v_OrigenTipo == "" || D.v_OrigenTipo == Constants.OrigenCompra || D.v_OrigenTipo == Constants.OrigenImportacion || D.v_OrigenTipo == Constants.OrigenTransferencia || D.v_OrigenTipo == Constants.OrigenVenta || D.v_OrigenTipo == Constants.OrigenGuiaInterna || (!string.IsNullOrEmpty(D.v_OrigenTipo) && D.v_OrigenTipo != Constants.OrigenGuiaCompra))
                                      
                                       select new KardexList
                                       {
                                           v_IdMovimientoDetalle = A == null ? "" : A.v_IdMovimientoDetalle,
                                           v_IdProducto = IncluirNroPedido == 1 ? A.v_NroPedido == null || A.v_NroPedido.Trim() == string.Empty ? C.v_IdProducto : C.v_IdProducto + " " + A.v_NroPedido.Trim() : C.v_IdProducto,
                                           v_NombreProducto = C == null ? "** PRODUCTO NO EXISTE **" : C.v_Descripcion,
                                           t_Fecha = D.t_Fecha,
                                           i_IdTipoMovimiento = D.i_IdTipoMovimiento,
                                           d_Cantidad = A == null ? 0 : A.d_CantidadEmpaque == null ? A.d_Cantidad == null ? 0 : A.d_Cantidad : A.d_CantidadEmpaque,
                                           d_Precio = D.v_OrigenTipo == "I" ? pintIdMoneda == D.i_IdMoneda.Value ? A.d_Total.Value : A.d_TotalCambio.Value : A.d_Total.Value,
                                           i_EsDevolucion = D.i_EsDevolucion,
                                           i_IdTipoMotivo = D.i_IdTipoMotivo,
                                           NroPedido = IncluirNroPedido == 1 ? A.v_NroPedido == null || A.v_NroPedido == string.Empty ? "" : A.v_NroPedido.Trim() : "",
                                           v_IdLinea = C == null ? "" : C.v_IdLinea,
                                           IdMoneda = D.i_IdMoneda.Value,
                                           TipoCambio = D.d_TipoCambio.Value,
                                           ClienteProveedor = E == null ? "" : (E.v_ApePaterno + " " + E.v_ApeMaterno + " " + E.v_PrimerNombre + " " + E.v_RazonSocial).Trim(),
                                           Guia = A == null ? "" : A.v_NroGuiaRemision,
                                           Documento = H == null ? A.v_NumeroDocumento : H.v_Siglas + " " + A.v_NumeroDocumento,
                                           CodProducto = C == null ? "" : C.v_CodInterno,
                                           IdAlmacen = D == null ? -1 : D.i_IdAlmacenOrigen.Value,
                                           d_Empaque = C.d_Empaque,
                                           EsDocumentoInterno = H == null || H.i_UsadoDocumentoInterno == null ? 0 : H.i_UsadoDocumentoInterno.Value,
                                           ValorUM = I.v_Value2,
                                           v_IdMarca = C.v_IdMarca,
                                           Almacen = G.v_Nombre,
                                           TipoMotivo = D == null ? -1 : D.i_IdTipoMotivo == 5 ? 1 : D.v_OrigenTipo == "T" ? 3 : 2,
                                           Origen = D == null ? null : D.v_OrigenTipo,
                                           d_Total = D == null ? null : D.v_OrigenTipo == "I" ? pintIdMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : A.d_Total,
                                           StockMinimo = C == null ? 0 : C.d_StockMinimo ?? 0,
                                           UnidadMedida = J1 == null ? "1" : J1.v_Value1,
                                           UnidadMedidaProducto = J3.v_Value1,
                                           i_IdUnidadMedidaProducto = C.i_IdUnidadMedida ?? 0,
                                           i_Activo = C.i_EsActivo ?? 0,
                                           v_IdProductoDetalle = A.v_IdProductoDetalle,
                                           Modelo = C.v_Modelo,
                                           NroParte = C.v_NroParte,
                                           Ubicacion = C.v_Ubicacion,
                                           Marca = J4 == null ? "" : J4.v_Nombre,
                                           v_NroLote =A.v_NroLote ,
                                           v_NroSerie =A.v_NroSerie ,
                                           t_FechaCaducidad =A.t_FechaCaducidad ==null ? DateTime.MinValue : A.t_FechaCaducidad.Value ,


                                       }).ToList();
                        if (RepResumenAlma)
                        {
                            //Cuando es Reporte Resumen Almacen y se visualiza los Movimientos ya no se toma en cuenta la Carga Inicial
                            //Porque se tomó en cuenta en el saldo Anterior
                            // Movimientos = Movimientos.Where(o => (o.i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso && o.i_IdTipoMotivo != (int)MotiveType.CARGAINICIAL)|| o.i_IdTipoMovimiento ==(int)TipoDeMovimiento.NotadeSalida).ToList();
                        }

                    }
                    else
                    {
                        var PeriodoCargaInicial = pdtFechaInicio.Value.Year.ToString();
                        Movimientos = (from A in movimientodetalle
                                       join B in dbContext.productodetalle on new { IdProductoDetalle = A.v_IdProductoDetalle, eliminado = 0 } equals new { IdProductoDetalle = B.v_IdProductoDetalle, eliminado = B.i_Eliminado.Value } into B_join
                                       from B in B_join.DefaultIfEmpty()
                                       join C in dbContext.producto on new { IdProducto = B.v_IdProducto, eliminado = 0 } equals new { IdProducto = C.v_IdProducto, eliminado = C.i_Eliminado.Value } into C_join
                                       from C in C_join.DefaultIfEmpty()
                                       join D in dbContext.movimiento  on new { IdMovimiento = A.v_IdMovimiento, eliminado = 0 } equals new { IdMovimiento = D.v_IdMovimiento, eliminado = D.i_Eliminado.Value } into D_join
                                       from D in D_join.DefaultIfEmpty()
                                       join E in dbContext.cliente  on new { IdCliente = D.v_IdCliente, eliminado = 0 } equals new { IdCliente = E == null ? null : E.v_IdCliente, eliminado = E == null ? 0 : E.i_Eliminado.Value } into E_join
                                       from E in E_join.DefaultIfEmpty()
                                       join G in dbContext.almacen on new { IdAlmacen = D.i_IdAlmacenOrigen.Value, eliminado = 0 } equals new { IdAlmacen = G == null ? -1 : G.i_IdAlmacen, eliminado = G == null ? 0 : G.i_Eliminado.Value } into G_join
                                       from G in G_join.DefaultIfEmpty()

                                       join H in dbContext.documento on new { IdTipoDoc = A.i_IdTipoDocumento ?? -1, eliminado = 0 } equals new { IdTipoDoc = H == null ? -1 : H.i_CodigoDocumento, eliminado = H == null ? 0 : H.i_Eliminado.Value } into H_join
                                       from H in H_join.DefaultIfEmpty()

                                       join I in dbContext.datahierarchy  on new { Grupo = 17, eliminado = 0, UnidadMedida = C.i_IdUnidadMedida ?? -1 } equals new { Grupo = I == null ? -1 : I.i_GroupId, eliminado = I == null ? 0 : I.i_IsDeleted.Value, UnidadMedida = I == null ? -1 : I.i_ItemId } into I_join
                                       from I in I_join.DefaultIfEmpty()

                                       join J1 in dbContext.datahierarchy on new { a = A.i_IdUnidad ?? -1, b = 17, eliminado = 0 }
                                                                    equals new { a = J1 == null ? -1 : J1.i_ItemId, b = J1 == null ? -1 : J1.i_GroupId, eliminado = J1 == null ? 0 : J1.i_IsDeleted.Value } into J1_join
                                       from J1 in J1_join.DefaultIfEmpty()

                                       join J3 in dbContext.datahierarchy  on new { umProducto = C == null ? -1 : C.i_IdUnidadMedida ?? -1, b = 17, eliminado = 0 }
                                                                    equals new { umProducto = J3 == null ? -1 : J3.i_ItemId, b = J3 == null ? -1 : J3.i_GroupId, eliminado = J3 == null ? 0 : J3.i_IsDeleted.Value } into J3_join
                                       from J3 in J3_join.DefaultIfEmpty()

                                       join J4 in dbContext.marca  on new { marc = C == null ? null : C.v_IdMarca, eliminado = 0 } equals new { marc = J4 == null ? null : J4.v_IdMarca, eliminado = J4 == null ? 0 : J4.i_Eliminado.Value } into J4_join
                                       from J4 in J4_join.DefaultIfEmpty()

                                       where C != null && D != null
                                       && A.i_Eliminado == 0 && D.i_IdTipoMovimiento != (int)TipoDeMovimiento.Transferencia
                                       && (C.v_CodInterno == pstrCodigoProducto || pstrCodigoProducto == "") 
                                       && (A.v_NroPedido == Pedido || Pedido == "")
                                       && C.i_EsServicio == 0
                                       && C.i_EsActivoFijo == 0
                                       && (D.i_IdEstablecimiento == Establecimiento)  
                                       && (C.v_Modelo == Modelo || Modelo == "")
                                       && (C.v_IdLinea == Linea || Linea == "-1")
                                       && D.i_IdTipoMovimiento == TipoMovimiento
                                       && D.i_IdTipoMotivo == (int)MotiveType.CARGAINICIAL
                                       && (D.v_Periodo == PeriodoCargaInicial)
                                       && (D.v_OrigenTipo == null || D.v_OrigenTipo == "" || D.v_OrigenTipo == Constants.OrigenCompra || D.v_OrigenTipo == Constants.OrigenImportacion || D.v_OrigenTipo == Constants.OrigenTransferencia || D.v_OrigenTipo == Constants.OrigenVenta || D.v_OrigenTipo == Constants.OrigenGuiaInterna || (!string.IsNullOrEmpty(D.v_OrigenTipo) && D.v_OrigenTipo != Constants.OrigenGuiaCompra))

                                       select new KardexList
                                       {
                                           v_IdMovimientoDetalle = A == null ? "" : A.v_IdMovimientoDetalle,
                                           //v_IdProducto = A.v_NroPedido == null || A.v_NroPedido.Trim() == string.Empty ? C.v_IdProducto : C.v_IdProducto + " " + A.v_NroPedido.Trim(),
                                           v_IdProducto = IncluirNroPedido == 1 ? A.v_NroPedido == null || A.v_NroPedido.Trim() == string.Empty ? C.v_IdProducto : C.v_IdProducto + " " + A.v_NroPedido.Trim() : C.v_IdProducto,
                                           v_NombreProducto = C == null ? "** PRODUCTO NO EXISTE **" : C.v_Descripcion,
                                           t_Fecha = D.t_Fecha,
                                           i_IdTipoMovimiento = D.i_IdTipoMovimiento,
                                           d_Cantidad = A == null ? 0 : A.d_CantidadEmpaque == null ? A.d_Cantidad == null ? 0 : A.d_Cantidad : A.d_CantidadEmpaque,
                                           d_Precio = D.v_OrigenTipo == "I" ? pintIdMoneda == D.i_IdMoneda.Value ? A.d_Total.Value : A.d_TotalCambio.Value : A.d_Total.Value,
                                           i_EsDevolucion = D.i_EsDevolucion,
                                           i_IdTipoMotivo = D.i_IdTipoMotivo,
                                           NroPedido = A.v_NroPedido == null || A.v_NroPedido == string.Empty ? "" : A.v_NroPedido.Trim(),
                                           v_IdLinea = C == null ? "" : C.v_IdLinea,
                                           IdMoneda = D.i_IdMoneda.Value,
                                           TipoCambio = D.d_TipoCambio.Value,
                                           ClienteProveedor = E == null ? "" : (E.v_ApePaterno + " " + E.v_ApeMaterno + " " + E.v_PrimerNombre + " " + E.v_RazonSocial).Trim(),
                                           Guia =A==null ?"": A.v_NroGuiaRemision,
                                           Documento =H==null ?A.v_NumeroDocumento : H.v_Siglas + " " + A.v_NumeroDocumento,
                                           CodProducto = C == null ? "" : C.v_CodInterno,
                                           IdAlmacen = D.i_IdAlmacenOrigen.Value,
                                           d_Empaque = C.d_Empaque,
                                           EsDocumentoInterno =H==null || H.i_UsadoDocumentoInterno == null ? 0 : H.i_UsadoDocumentoInterno.Value,
                                           ValorUM = I.v_Value2,
                                           v_IdMarca = C.v_IdMarca,
                                           Almacen = G.v_Nombre,
                                           TipoMotivo =D==null ?-1 : D.i_IdTipoMotivo == 5 ? 1 : D.v_OrigenTipo == "T" ? 3 : 2,
                                           Origen =D==null ?null : D.v_OrigenTipo,
                                           d_Total =D==null ?null : D.v_OrigenTipo == "I" ? pintIdMoneda == D.i_IdMoneda.Value ? A.d_Total : A.d_TotalCambio : A.d_Total,
                                           StockMinimo = C == null ? 0 : C.d_StockMinimo ?? 0,
                                           UnidadMedida =J1 ==null ?"1": J1.v_Value1,
                                           UnidadMedidaProducto =J3 ==null ?"1": J3.v_Value1,
                                           i_IdUnidadMedidaProducto = C.i_IdUnidadMedida ?? 0,
                                           i_Activo = C.i_EsActivo ?? 0,
                                           v_IdProductoDetalle = A.v_IdProductoDetalle,
                                           Modelo = C.v_Modelo,
                                           NroParte = C.v_NroParte,
                                           Ubicacion = C.v_Ubicacion,
                                           Marca = J4 == null ? "" : J4.v_Nombre,
                                           v_NroLote = A.v_NroLote,
                                           v_NroSerie = A.v_NroSerie,
                                           t_FechaCaducidad = A.t_FechaCaducidad==null ?DateTime.MinValue :A.t_FechaCaducidad.Value,


                                       }).ToList();
                    }
                    if (!string.IsNullOrEmpty(pstrFilterExpression))
                    {
                        Movimientos = Movimientos.AsQueryable().Where(pstrFilterExpression).ToList();
                    }

                    if (AsientoConsumo)
                        Movimientos = Movimientos.AsQueryable().Where(h => h.v_IdMovimientoDetalle != IdMovimientoDetalleConsumo).ToList();
                    var queryList = Movimientos.ToList();
                    //queryList = Movimientos.OrderBy(x => x.IdAlmacenOrigen).ThenBy(x => x.v_IdProducto).ThenBy(x => x.t_Fecha).ThenBy(x => x.i_IdTipoMovimiento).ThenBy(x => x.TipoMotivo).ThenBy(x => x.NroPedido).ThenBy(o => o.v_IdMovimientoDetalle).ToList();
                    queryList = queryList.Select(x => new KardexList
                    {
                        v_IdMovimientoDetalle = x.v_IdMovimientoDetalle,
                        //v_IdProducto = x.v_IdProducto,
                        //v_NombreProducto = x.v_NombreProducto.Trim(),
                        v_IdProducto = Agrupar == "" ? x.v_IdProducto : Agrupar == "Lote" ? x.v_IdProducto + " " + " - LOTE : " + x.v_NroLote + " - Fecha Venc. : " + x.t_FechaCaducidad.Date.ToShortDateString() : x.v_NombreProducto + " " + " - SERIE : " + x.v_NroSerie,
                        v_NombreProducto = Agrupar == "" ? x.v_NombreProducto : Agrupar == "Lote" ? x.v_NombreProducto + " " + " - LOTE : " + x.v_NroLote + " - Fecha Venc. : " + x.t_FechaCaducidad.Date.ToShortDateString() : x.v_NombreProducto + " " + " - SERIE : " + x.v_NroSerie,
                        t_Fecha = x.t_Fecha,
                        i_IdTipoMovimiento = x.i_IdTipoMovimiento,
                        d_Cantidad = x.d_Cantidad == 0 || decimal.Parse(x.ValorUM) == 0 ? 0 : FormatoCant == (int)FormatoCantidad.UnidadMedidaProducto ? x.d_Cantidad / decimal.Parse(x.ValorUM) : x.d_Cantidad,
                        d_Precio = x.d_Cantidad == 0 || decimal.Parse(x.ValorUM) == 0 || x.d_Precio == 0 ? 0 : FormatoCant == (int)FormatoCantidad.UnidadMedidaProducto ? x.d_Cantidad == 0 ? 0 : x.d_Precio / (x.d_Cantidad / (decimal.Parse(x.ValorUM))) : x.d_Cantidad == 0 ? 0 : x.d_Precio / x.d_Cantidad,
                        i_EsDevolucion = x.i_EsDevolucion,
                        i_IdTipoMotivo = x.i_IdTipoMotivo,
                        NroPedido = x.NroPedido,
                        v_IdLinea = x.v_IdLinea,
                        IdMoneda = x.IdMoneda,
                        TipoCambio = x.TipoCambio,
                        ClienteProveedor = x.ClienteProveedor,
                        Guia = x.Guia,
                        Documento = x.Documento,
                        CodProducto = x.CodProducto,
                        UnidadMedida = x.UnidadMedida,
                        IdAlmacen = x.IdAlmacen,
                        d_Empaque = x.d_Empaque,
                        EsDocumentoInterno = x.EsDocumentoInterno,
                        Almacen = x.Almacen,
                        Origen = x.Origen,
                        d_Total = x.d_Total,
                        StockMinimo = x.StockMinimo,
                        UnidadMedidaProducto = x.UnidadMedidaProducto,
                        i_IdUnidadMedidaProducto = x.i_IdUnidadMedidaProducto,
                        i_Activo = x.i_Activo,
                        ValorUM = x.ValorUM,
                        v_IdProductoDetalle = x.v_IdProductoDetalle,
                        Modelo = x.Modelo,
                        NroParte = x.NroParte,
                        Ubicacion = x.Ubicacion,
                        Marca = x.Marca,
                        // GrupoLlave =x.GrupoLlave ,
                    }).ToList();
                    queryList = queryList.OrderBy(x => x.IdAlmacenOrigen).ThenBy(x => x.v_IdProducto).ThenBy(x => x.t_Fecha).ThenBy(x => x.i_IdTipoMovimiento).ThenBy(x => x.TipoMotivo).ThenBy(x => x.NroPedido).ThenBy(o => o.v_IdMovimientoDetalle).ToList();
                    int Contador = queryList.Count;
                    decimal UltimoPrecio = 0;
                    int PosicionProducto = 0;
                    for (int i = 0; i < Contador; i++)
                    {
                        if (ConsideraDocInternos == 1 || (ConsideraDocInternos == 0 && queryList[i].EsDocumentoInterno == 0))
                        {
                            oKardexList = new KardexList();
                            oKardexList = (KardexList)queryList[i].Clone();
                            oKardexList.i_IdUnidad = FormatoCant == (int)FormatoCantidad.Unidades ? oKardexList.ValorUM == "1" ? oKardexList.i_IdUnidadMedidaProducto : 15 : oKardexList.i_IdUnidadMedidaProducto;
                            oKardexList.UnidadMedida = FormatoCant == (int)FormatoCantidad.Unidades ? "UNIDAD" : queryList.Where(x => x.CodProducto == oKardexList.CodProducto && x.UnidadMedidaProducto == "UNIDAD").Count() != 0 ? "UNIDAD" : queryList[i].UnidadMedidaProducto;//   queryList.Where(x => x.CodProducto == oKardexList.CodProducto && x.UnidadMedida == "UNIDAD").Count() != 0 ? "UNIDAD" : queryList[i].UnidadMedida;
                            oKardexList.v_Fecha = queryList[i].t_Fecha.Value.ToString("dd-MMM");
                            oKardexList.v_NombreTipoMovimiento = queryList[i].i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso ? "INGRESO" : "SALIDA";
                            oKardexList.Almacen = "ALMACÉN : " + queryList[i].Almacen.ToUpper();
                            if (i == 0)
                            {

                                if ((oKardexList.i_IdTipoMotivo != 5 && oKardexList.i_IdTipoMovimiento == 1) || oKardexList.i_IdTipoMovimiento == 2)
                                {
                                    KardexList oKardexList1 = new KardexList();
                                    oKardexList1 = (KardexList)oKardexList.Clone();
                                    oKardexList1.v_NombreTipoMovimiento = "INICIAL";
                                    DateTime date = DateTime.Parse("01/" + pdtFechaInicio.Value.Month.ToString("00") + "/" + pdtFechaInicio.Value.Year.ToString());
                                    oKardexList1.t_Fecha = date;
                                    oKardexList1.DocumentoInventarioSunat = "";
                                    oKardexList1.v_Fecha = date.ToString("dd-MMM");
                                    oKardexList1.DocumentoInventarioSunat = "";
                                    oKardexList1.Guia = "";
                                    oKardexList1.Documento = "";
                                    oKardexList1.ClienteProveedor = "";
                                    oKardexList1.Ingreso_Cantidad = 0;
                                    oKardexList1.Ingreso_Precio = 0;
                                    oKardexList1.Ingreso_Total = 0;
                                    oKardexList1.Salida_Cantidad = null;
                                    oKardexList1.i_IdTipoMotivo = 5;
                                    oKardexList1.Salida_Precio = 0;
                                    oKardexList1.Salida_Total = 0;
                                    oKardexList1.Saldo_Cantidad = 0;
                                    oKardexList1.Saldo_Precio = 0;
                                    oKardexList1.Saldo_Total = 0;
                                    oKardexList1.NumeroElemento = 0;
                                    oKardexList1.i_IdTipoMovimiento = 1;
                                    Lista.Add(oKardexList1);
                                    PosicionProducto = PosicionProducto + 1;
                                    IdProductoOld = oKardexList1.v_IdProducto;

                                }

                            }
                            CantidadValorizadas objProceso = new CantidadValorizadas();
                            objProceso = Globals.ClientSession.i_Periodo == 2016 ? ProcesoValorizacionProductoPedido2016(queryList[i].i_IdTipoMovimiento.Value, IdProductoOld, queryList[i].v_IdProducto, queryList[i].IdAlmacen.ToString(), pintIdMoneda, queryList[i].Origen, queryList[i].i_EsDevolucion ?? 0, queryList[i].d_Cantidad.Value, queryList[i].IdMoneda, queryList[i].d_Precio ?? 0, PosicionProducto, queryList[i].TipoCambio, ref  UltimoPrecio, -1, Lista, queryList[i].d_Total ?? 0) : ProcesoValorizacionProductoPedido2017(queryList[i].i_IdTipoMovimiento.Value, IdProductoOld, queryList[i].v_IdProducto, queryList[i].IdAlmacen.ToString(), pintIdMoneda, queryList[i].Origen, queryList[i].i_EsDevolucion ?? 0, queryList[i].d_Cantidad.Value, queryList[i].IdMoneda, queryList[i].d_Precio ?? 0, PosicionProducto, queryList[i].TipoCambio, ref  UltimoPrecio, -1, Lista, queryList[i].d_Total ?? 0);

                            oKardexList.Ingreso_Cantidad = objProceso.Ingreso_Cantidad;
                            oKardexList.Ingreso_Precio = objProceso.Ingreso_Precio;
                            oKardexList.Ingreso_Total = objProceso.Ingreso_Total;
                            oKardexList.Saldo_Cantidad = objProceso.Saldo_Cantidad;
                            oKardexList.Saldo_Precio = objProceso.Saldo_Precio;
                            oKardexList.Saldo_Total = objProceso.Saldo_Total;
                            oKardexList.Salida_Cantidad = objProceso.Salida_Cantidad;
                            oKardexList.Salida_Precio = objProceso.Salida_Precio;
                            oKardexList.Salida_Total = objProceso.Salida_Total;
                            oKardexList.Saldo_CantidadExcel = Utils.Windows.DevuelveValorRedondeado(objProceso.Saldo_Cantidad.Value, Globals.ClientSession.i_CantidadDecimales.Value) == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado(objProceso.Saldo_Cantidad.Value, Globals.ClientSession.i_CantidadDecimales.Value);
                            oKardexList.Saldo_PrecioExcel = Utils.Windows.DevuelveValorRedondeado(objProceso.Saldo_Precio.Value, Globals.ClientSession.i_PrecioDecimales.Value) == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado(objProceso.Saldo_Precio.Value, Globals.ClientSession.i_PrecioDecimales.Value);
                            oKardexList.Saldo_TotalExcel = Utils.Windows.DevuelveValorRedondeado(objProceso.Saldo_Total.Value, Globals.ClientSession.i_PrecioDecimales.Value) == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado(objProceso.Saldo_Total.Value, Globals.ClientSession.i_PrecioDecimales.Value);
                            oKardexList.StockFisico = queryList[i].StockMinimo;
                            oKardexList.NumeroElemento = objProceso.NumeroElemento;
                            IdProductoOld = queryList[i].v_IdProducto;
                            if (i + 1 < Contador && IdProductoOld == queryList[i + 1].v_IdProducto)
                            {
                                ///Se agrego para valorizacion sunat
                                int Reg = Lista.Count();
                                //Se agrego para los saldos iniciales 
                                if (PosicionProducto == 0 && Reg > 0 && Lista[Reg - 1].v_IdProducto != oKardexList.v_IdProducto)
                                {
                                    if (int.Parse(pdtFechaInicio.Value.Month.ToString()) > 1)
                                    {
                                        KardexList oKardexList1 = new KardexList();
                                        oKardexList1 = (KardexList)queryList[i + 1].Clone();
                                        if (oKardexList1.v_IdProducto == "N001-PD000000527")
                                        {
                                            string h = "";
                                        }
                                        oKardexList1.v_NombreTipoMovimiento = "INICIAL";
                                        DateTime date = DateTime.Parse("01/" + pdtFechaInicio.Value.Month.ToString("00") + "/" + pdtFechaInicio.Value.Year.ToString());
                                        oKardexList1.t_Fecha = date;
                                        oKardexList1.DocumentoInventarioSunat = "";
                                        oKardexList1.v_Fecha = date.ToString("dd-MMM");
                                        oKardexList1.DocumentoInventarioSunat = "";
                                        oKardexList1.Guia = "";
                                        oKardexList1.Documento = "";
                                        oKardexList1.ClienteProveedor = "";
                                        oKardexList1.Ingreso_Cantidad = 0;
                                        oKardexList1.Ingreso_Precio = 0;
                                        oKardexList1.Ingreso_Total = 0;
                                        oKardexList1.Salida_Cantidad = null;
                                        oKardexList1.i_IdTipoMotivo = 5;
                                        oKardexList1.Salida_Precio = 0;
                                        oKardexList1.Salida_Total = 0;
                                        oKardexList1.Saldo_Cantidad = 0;
                                        oKardexList1.Saldo_Precio = 0;
                                        oKardexList1.Saldo_Total = 0;
                                        oKardexList1.NumeroElemento = 0;
                                        oKardexList1.i_IdTipoMovimiento = 1;
                                        Lista.Add(oKardexList1);
                                        PosicionProducto = PosicionProducto + 1;
                                        IdProductoOld = oKardexList1.v_IdProducto;
                                    }

                                }

                                // Hasta Acá;

                                PosicionProducto = PosicionProducto + 1;
                                Lista.Add(oKardexList);

                            }
                            else
                            {
                                PosicionProducto = 0;
                                UltimoPrecio = 0;
                                Lista.Add(oKardexList);
                                if (i + 1 < Contador)// !SeInsertoLista
                                {
                                    KardexList oKardexList1 = new KardexList();
                                    oKardexList1 = (KardexList)queryList[i + 1].Clone();
                                    //var ExisteListaReporte = Lista.Where(l => l.v_IdProducto == oKardexList1.v_IdProducto && l.NroPedido == oKardexList1.NroPedido).ToList();
                                    if (((queryList[i + 1].i_IdTipoMotivo != 5 && queryList[i + 1].i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso)) || queryList[i + 1].i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeSalida)
                                    {
                                        oKardexList1.v_NombreTipoMovimiento = "INICIAL";
                                        DateTime date = DateTime.Parse("01/" + pdtFechaInicio.Value.Month.ToString("00") + "/" + pdtFechaInicio.Value.Year.ToString());
                                        oKardexList1.v_Fecha = date.ToString("dd-MMM");
                                        oKardexList1.t_Fecha = date;
                                        oKardexList1.DocumentoInventarioSunat = "";
                                        oKardexList1.Guia = "";
                                        oKardexList1.Documento = "";
                                        oKardexList1.ClienteProveedor = "";
                                        oKardexList1.Ingreso_Cantidad = 0;
                                        oKardexList1.Ingreso_Precio = 0;
                                        oKardexList1.Ingreso_Total = 0;
                                        oKardexList1.Salida_Cantidad = null;
                                        oKardexList1.i_IdTipoMotivo = 5;
                                        oKardexList1.Salida_Precio = 0;
                                        oKardexList1.Salida_Total = 0;
                                        oKardexList1.Saldo_Cantidad = 0;
                                        oKardexList1.Saldo_Precio = 0;
                                        oKardexList1.Saldo_Total = 0;
                                        oKardexList1.NumeroElemento = 0;
                                        // oKardexList1.TipoOperacionInventarioSunat = int.Parse(TipoMovimientos.Where(l => l.i_ItemId == 5).FirstOrDefault().v_Value2).ToString("00");
                                        oKardexList1.i_IdTipoMovimiento = 1;
                                        Lista.Add(oKardexList1);
                                        PosicionProducto = PosicionProducto + 1;
                                        IdProductoOld = oKardexList1.v_IdProducto;
                                    }
                                }
                            }
                        }
                    }

                    if (RepResumenAlma || RepResumenMov)
                    {

                        if (FechaRepResumenAlmaMov.Month.ToString("00") != "01")
                        {
                            var MesAnterior = int.Parse(FechaRepResumenAlmaMov.Month.ToString()) - 1;
                            var FechaAnt = DateTime.Parse(DateTime.DaysInMonth(Globals.ClientSession.i_Periodo.Value, MesAnterior).ToString() + "/" + MesAnterior + "/" + Globals.ClientSession.i_Periodo.ToString() + " 23:59");
                            List<KardexList> Sald = new List<KardexList>();
                            Sald = Lista.Select(item => (KardexList)item.Clone()).ToList();
                            var ListaSaldosAnt = Sald.ToList().Where(l => l.t_Fecha <= FechaAnt).GroupBy(l => l.v_NombreProducto).ToList().Select(d =>
                            {
                                var k = d.LastOrDefault();
                                k.v_NombreTipoMovimiento = "INICIAL";
                                DateTime date = DateTime.Parse("01/" + FechaRepResumenAlmaMov.Month.ToString("00") + "/" + FechaRepResumenAlmaMov.Year.ToString()); // DateTime.Parse("01/" + pdtFechaInicio.Value.Month.ToString("00") + "/" + pdtFechaInicio.Value.Year.ToString());
                                k.v_Fecha = date.ToString("dd-MMM");
                                k.t_Fecha = date;
                                k.Guia = "";
                                k.Documento = "";
                                k.ClienteProveedor = "";
                                k.Salida_Cantidad = null;
                                k.Ingreso_Cantidad = null;
                                k.Ingreso_Precio = null;
                                k.Ingreso_Total = null;
                                k.Salida_Cantidad = null;
                                k.Salida_Precio = null;
                                k.Salida_Total = null;
                                k.i_IdTipoMotivo = 5;
                                k.DocumentoInventarioSunat = "";
                                k.TipoOperacionInventarioSunat = "16";
                                return k;
                            }).ToList();


                            var Lista2 = Lista.Where(l => l.t_Fecha > FechaAnt).ToList();
                            Lista = new List<KardexList>();
                            Lista = ListaSaldosAnt.Concat(Lista2).ToList();
                        }

                    }
                    else
                    {
                        if (FechaInicioReporte.Month.ToString("00") != "01")
                        {


                            var MesAnterior = int.Parse(FechaInicioReporte.Month.ToString()) - 1;
                            var FechaAnt = DateTime.Parse(DateTime.DaysInMonth(Globals.ClientSession.i_Periodo.Value, MesAnterior).ToString() + "/" + MesAnterior + "/" + Globals.ClientSession.i_Periodo.ToString() + " 23:59");

                            List<KardexList> Sald = new List<KardexList>();
                            Sald = Lista.Select(item => (KardexList)item.Clone()).ToList();

                            var ListaSaldosAnt = Sald.ToList().Where(l => l.t_Fecha <= FechaAnt).GroupBy(l => l.v_NombreProducto).ToList().Select(d =>
                            {
                                if (d.LastOrDefault().v_IdProducto == "N001-PD000000527")
                                {
                                    string h = "";
                                }
                                var k = d.LastOrDefault();
                                k.v_NombreTipoMovimiento = "INICIAL";
                                DateTime date = DateTime.Parse("01/" + FechaInicioReporte.Month.ToString("00") + "/" + FechaInicioReporte.Year.ToString()); // DateTime.Parse("01/" + pdtFechaInicio.Value.Month.ToString("00") + "/" + pdtFechaInicio.Value.Year.ToString());
                                k.v_Fecha = date.ToString("dd-MMM");
                                k.t_Fecha = date;
                                k.Guia = "";
                                k.Documento = "";
                                k.ClienteProveedor = "";
                                k.Salida_Cantidad = null;
                                k.Ingreso_Cantidad = null;
                                k.Ingreso_Precio = null;
                                k.Ingreso_Total = null;
                                k.Salida_Cantidad = null;
                                k.Salida_Precio = null;
                                k.Salida_Total = null;
                                k.i_IdTipoMotivo = 5;
                                k.DocumentoInventarioSunat = "";
                                k.TipoOperacionInventarioSunat = "16";
                                return k;
                            }).ToList();
                            var Lista2 = Lista.Where(l => l.t_Fecha > FechaAnt).ToList();
                            Lista = new List<KardexList>();
                            Lista = ListaSaldosAnt.Concat(Lista2).ToList();
                        }
                    }

                    List<KardexList> ListaAgrupada = new List<KardexList>();
                    if (RepResumenAlma)
                    {

                        //Para Reporte Resumen Almacen ya no se toma en cuenta carga Inicial
                        //Debido a que se esta tomando en una columnaInicial
                        if (IncluirNroPedido == 1)
                        {
                            ListaAgrupada = Lista.Where(o => o.t_Fecha >= FechaRepResumenAlmaMov).GroupBy(x => new { x.v_IdProducto, x.NroPedido }).ToList()
                               .Select(p =>
                               {
                                   var k = p.LastOrDefault();
                                   var CargaInicial = p.Where(o => (o.i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso && o.i_IdTipoMotivo == (int)MotiveType.CARGAINICIAL)).ToList();
                                   var SumaInicialIngresos = CargaInicial.Any() ? CargaInicial.Sum(j => j.Saldo_Cantidad).Value : 0;
                                   var SumaInicialPrecios = CargaInicial.Any() ? CargaInicial.Sum(j => j.Saldo_Precio).Value : 0;
                                   var SumaInicialTotal = CargaInicial.Any() ? CargaInicial.Sum(d => d.Saldo_Total).Value : 0;
                                   k.SumatoriaIngresos = SeUsoCargaINICIAL ? p.Sum(f => f.Ingreso_Cantidad ?? 0) - SumaInicialIngresos : p.Sum(f => f.Ingreso_Cantidad ?? 0);
                                   k.SumatoriaSalidas = p.Sum(f => f.Salida_Cantidad ?? 0);
                                   k.SumatoriaTotalIngresos = SeUsoCargaINICIAL ? p.Sum(f => f.Ingreso_Total ?? 0) - SumaInicialTotal : p.Sum(f => f.Ingreso_Total ?? 0);
                                   k.SumatoriaTotalSalidas = p.Sum(f => f.Salida_Total ?? 0);
                                   k.SumatoriaPreciosIngresos = SeUsoCargaINICIAL ? p.Sum(f => f.Ingreso_Precio ?? 0) - SumaInicialPrecios : p.Sum(f => f.Ingreso_Precio ?? 0);
                                   k.SumatoriaPreciosSalidas = p.Sum(f => f.Salida_Precio ?? 0);
                                   return k;


                               }).ToList();
                        }
                        else
                        {
                            ListaAgrupada = Lista.Where(o => o.t_Fecha >= FechaRepResumenAlmaMov).GroupBy(x => new { x.v_IdProducto }).ToList()
                                   .Select(p =>
                                   {
                                       var k = p.LastOrDefault();
                                       var CargaInicial = p.Where(o => (o.i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso && o.i_IdTipoMotivo == (int)MotiveType.CARGAINICIAL)).ToList();
                                       var SumaInicialIngresos = CargaInicial.Any() ? CargaInicial.Sum(j => j.Saldo_Cantidad).Value : 0;
                                       var SumaInicialPrecios = CargaInicial.Any() ? CargaInicial.Sum(j => j.Saldo_Precio).Value : 0;
                                       var SumaInicialTotal = CargaInicial.Any() ? CargaInicial.Sum(d => d.Saldo_Total).Value : 0;
                                       k.SumatoriaIngresos = SeUsoCargaINICIAL ? p.Sum(f => f.Ingreso_Cantidad ?? 0) - SumaInicialIngresos : p.Sum(f => f.Ingreso_Cantidad ?? 0);
                                       k.SumatoriaSalidas = p.Sum(f => f.Salida_Cantidad ?? 0);
                                       k.SumatoriaTotalIngresos = SeUsoCargaINICIAL ? p.Sum(f => f.Ingreso_Total ?? 0) - SumaInicialTotal : p.Sum(f => f.Ingreso_Total ?? 0);
                                       k.SumatoriaTotalSalidas = p.Sum(f => f.Salida_Total ?? 0);
                                       k.SumatoriaPreciosIngresos = SeUsoCargaINICIAL ? p.Sum(f => f.Ingreso_Precio ?? 0) - SumaInicialPrecios : p.Sum(f => f.Ingreso_Precio ?? 0);
                                       k.SumatoriaPreciosSalidas = p.Sum(f => f.Salida_Precio ?? 0);
                                       return k;


                                   }).ToList();

                        }



                    }
                    else if (RepResumenMov)
                    {


                        ListaAgrupada = Lista.GroupBy(x => new { x.v_IdProducto, x.i_IdTipoMovimiento }).ToList()
                               .Select(p =>
                               {
                                   var k = p.LastOrDefault();
                                   k.SumatoriaIngresos = p.Sum(f => f.Ingreso_Cantidad ?? 0);
                                   k.SumatoriaSalidas = p.Sum(f => f.Salida_Cantidad ?? 0);
                                   k.SumatoriaTotalIngresos = p.Sum(f => f.Ingreso_Total ?? 0);
                                   k.SumatoriaTotalSalidas = p.Sum(f => f.Salida_Total ?? 0);
                                   k.SumatoriaPreciosIngresos = p.Sum(f => f.Ingreso_Precio ?? 0);
                                   k.SumatoriaPreciosSalidas = p.Sum(f => f.Salida_Precio ?? 0);
                                   return k;


                               }).ToList();
                    }
                    else
                    {
                        if (IncluirNroPedido == 1)
                        {
                            ListaAgrupada = Lista.GroupBy(x => new { x.v_IdProducto, x.NroPedido })
                                        .Select(group => group.Last())
                                        .OrderBy(o => o.CodProducto).ToList();
                        }
                        else
                        {
                            ListaAgrupada = Lista.GroupBy(x => new { x.v_IdProducto })
                                           .Select(group => group.Last())
                                           .OrderBy(o => o.CodProducto).ToList();
                        }

                    }
                    pobjOperationResult.Success = 1;
                    if (SoloStock0 == 1)
                    {

                        return ListaAgrupada.Where(x => x.Saldo_Cantidad == 0).ToList().OrderBy(l => l.CodProducto).ToList();
                    }
                    else if (SoloStock == 1)
                    {

                        return ListaAgrupada.Where(x => x.Saldo_Cantidad > 0).ToList().OrderBy(l => l.CodProducto).ToList();
                    }
                    else if (SoloStockNegativo == 1)
                    {

                        return ListaAgrupada.Where(x => x.Saldo_Cantidad < 0).ToList().OrderBy(l => l.CodProducto).ToList();
                    }

                    else
                    {

                        return ListaAgrupada.OrderBy(l => l.CodProducto).ToList();
                    }
                }
            }

            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ProductoBL.ReporteStock() ,Producto :" + IdProductoOld;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }



        public List<KardexList> ReporteCruceInventario(ref OperationResult pobjOperationResult, DateTime pdtFechaInicio, DateTime pdtFechaFin, string idLinea, int idEstab, int idAlmacen)
        {
            try
            {
                var fechaInit = new DateTime(pdtFechaInicio.Year, 1, 1);
                var inv = new ProductoInventarioBL().GetData(ref pobjOperationResult, pdtFechaInicio, pdtFechaFin, idEstab, idAlmacen, idLinea);

                if (inv == null)
                {
                    return null;
                }
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var res = (from m in dbContext.movimientodetalle
                               join d in dbContext.productodetalle on new { m.v_IdProductoDetalle, e = 0 } equals new { d.v_IdProductoDetalle, e = d.i_Eliminado.Value } into dJoin
                               from d in dJoin.DefaultIfEmpty()
                               join p in dbContext.producto on new { d.v_IdProducto, e = 0 } equals new { p.v_IdProducto, e = p.i_Eliminado.Value } into pJoin
                               from p in pJoin.DefaultIfEmpty()
                               join a in dbContext.movimiento on new { id = m.v_IdMovimiento, e = 0 } equals new { id = a.v_IdMovimiento, e = a.i_Eliminado.Value } into aJoin
                               from a in aJoin.DefaultIfEmpty()
                               where a.i_IdEstablecimiento == idEstab &&
                               a.t_Fecha >= fechaInit && a.t_Fecha <= pdtFechaFin && a.i_IdTipoMovimiento != (int)TipoDeMovimiento.Transferencia
                               && p.i_EsServicio == 0 && p.i_EsActivoFijo == 0
                               && (p.v_IdLinea == idLinea || idLinea == null)
                               orderby p.v_CodInterno
                               group new KardexList
                               {
                                   CodProducto = p == null ? "" : p.v_CodInterno,
                                   v_NombreProducto = p == null ? "" : p.v_Descripcion,
                                   d_Cantidad = m == null ? 0 : m.d_CantidadEmpaque ?? (m.d_Cantidad ?? 0),
                                   i_IdTipoMovimiento = a == null ? 3 : a.i_IdTipoMovimiento,
                                   i_EsDevolucion = a == null ? 0 : a.i_EsDevolucion,
                                   v_IdProducto = p == null ? "" : p.v_IdProducto,


                               } by p.v_IdProducto).ToDictionary(r => r.Key, f =>
                               {
                                   var fs = f.FirstOrDefault();
                                   if (fs != null)
                                       fs.d_Cantidad = f.Sum(r =>
                                       {
                                           var val = r.d_Cantidad;
                                           if ((r.i_IdTipoMovimiento == 1 && r.i_EsDevolucion == 1) ||
                                               (r.i_IdTipoMovimiento == 2 && r.i_EsDevolucion == 0))
                                               val *= -1;
                                           return val;
                                       });
                                   return fs;
                               });
                    var ProductosconMovimientos = res.Select(k =>
                    {
                        var f = inv.FirstOrDefault(r => r.v_IdProducto == k.Key);
                        return new KardexList
                        {
                            CodProducto = k.Value.CodProducto,
                            v_NombreProducto = k.Value.v_NombreProducto,
                            StockFisico = f != null ? (f.d_Cantidad ?? 0) : 0,
                            Saldo_Cantidad = k.Value.d_Cantidad,
                            v_IdProducto = k.Value.v_IdProducto,

                        };
                    }).OrderBy(r => r.CodProducto).ToList();

                    var TodosProductos = new ProductoBL().ListaTodosProductosDetallesAct(idLinea);
                    var prodSinMovDiccionario = TodosProductos.ToDictionary(p => p.v_IdProducto, o => o);
                    var InventarioDiccionario = inv.ToDictionary(p => p.v_IdProducto, o => o);
                    var CodigosTodosProductos = TodosProductos.Select(x => x.v_IdProducto).ToList();
                    var NuevosProd = CodigosTodosProductos.Except(ProductosconMovimientos.Select(x => x.v_IdProducto)).ToList();
                    List<KardexList> ListaFaltantes = NuevosProd.Select(idProducto =>
                        {

                            productodetalleDto pd;
                            productoinventarioDto Inv;
                            pd = prodSinMovDiccionario.TryGetValue(idProducto, out pd) ? pd : new productodetalleDto();
                            Inv = InventarioDiccionario.TryGetValue(idProducto, out Inv) ? Inv : new productoinventarioDto();
                            return new KardexList
                            {
                                CodProducto = pd == null ? "" : pd.v_IdColor,
                                v_NombreProducto = pd == null ? "" : pd.v_IdTalla,
                                Saldo_Cantidad = 0,
                                StockFisico = Inv != null ? (Inv.d_Cantidad ?? 0) : 0
                            };
                        }
                        ).ToList();

                    pobjOperationResult.Success = 1;
                    return ProductosconMovimientos.Concat(ListaFaltantes).OrderBy(o => o.CodProducto).ToList();
                    //return result;
                }
            }

            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ProductoBL.CruceInventario()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public List<ReporteStockConsolidado> ReporteStockConsolidadoAlmacen(ref OperationResult objOperationResult, DateTime FechaInicial, DateTime FechaFinal, string Orden, string pstrLinea, int idEstablecimiento, string CodInternoProducto, string IdMarca, int Almacen)
        {
            try
            {
                IQueryable<ReporteStockConsolidado> query;
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {


                    query = (from a in dbContext.movimiento

                             join b in dbContext.movimientodetalle on new { IdMovimiento = a.v_IdMovimiento, eliminado = 0 } equals new { IdMovimiento = b.v_IdMovimiento, eliminado = b.i_Eliminado.Value } into b_join

                             from b in b_join.DefaultIfEmpty()

                             join c in dbContext.productodetalle on new { IdProductoDetalle = b.v_IdProductoDetalle, eliminado = 0 } equals new { IdProductoDetalle = c.v_IdProductoDetalle, eliminado = c.i_Eliminado.Value } into c_join

                             from c in c_join.DefaultIfEmpty()

                             join d in dbContext.producto on new { IdProducto = c.v_IdProducto, eliminado = 0 } equals new { IdProducto = d.v_IdProducto, eliminado = d.i_Eliminado.Value } into d_join

                             from d in d_join.DefaultIfEmpty()

                             join g in dbContext.datahierarchy on new { g = 17, i = b.i_IdUnidad.Value, eliminado = 0 }
                                                             equals new { g = g.i_GroupId, i = g.i_ItemId, eliminado = g.i_IsDeleted.Value } into g_join
                             from g in g_join.DefaultIfEmpty()

                             where a.t_Fecha >= FechaInicial
                             && a.t_Fecha <= FechaFinal && a.i_Eliminado == 0 && (d.v_IdLinea == pstrLinea || pstrLinea == "-1")
                             && (a.i_IdAlmacenOrigen == Almacen)
                             && (d.v_CodInterno == CodInternoProducto || CodInternoProducto == "") && a.i_IdTipoMovimiento != (int)TipoDeMovimiento.Transferencia
                             && d.i_EsServicio == 0 && d.i_EsActivoFijo == 0 && (d.v_IdMarca == IdMarca || IdMarca == "-1")
                             select new ReporteStockConsolidado
                             {
                                 codigoProducto = d.v_CodInterno,
                                 IdProducto = d.v_IdProducto,
                                 descripcionProducto = d.v_Descripcion.Trim(),
                                 tipoMovimiento = a.i_IdTipoMovimiento.Value,
                                 devolucion = a.i_EsDevolucion.Value,
                                 idAlmacen = a.i_IdAlmacenOrigen.Value,
                                 cantidad = d.d_Empaque.Value == 1 && g.v_Value2 == "1" ? b.d_Cantidad : b.d_CantidadEmpaque ?? 0,
                                 unidad = d.d_Empaque.Value == 1 && g.v_Value2 == "1" ? g.v_Value1 : "UNIDAD"
                             }).ToList().AsQueryable();
                    if (!string.IsNullOrEmpty(Orden))
                    {
                        query = query.OrderBy(Orden);
                    }
                }
                var listReport = GetConsolidadoAlmacen(query, idEstablecimiento);
                objOperationResult.Success = 1;
                return listReport;
            }
            catch (Exception ex)
            {
                objOperationResult.ExceptionMessage = ex.Message;
                return null;
            }

        }

        private List<ReporteStockConsolidado> GetConsolidadoAlmacen(IQueryable<ReporteStockConsolidado> q, int idestabl)
        {
            OperationResult objOperationReult = new OperationResult();
            var queryList = q.GroupBy(x => x.codigoProducto);

            //var almacenes = ObtenerAlmacenesxEstablecimiento(idestabl);
            var almacenes = ObtenerAlmacen(ref objOperationReult, 1);
            var listReport = new List<ReporteStockConsolidado>();
            var ty = typeof(ReporteStockConsolidado);
            foreach (var prod in queryList)
            {
                var first = prod.FirstOrDefault();
                if (first == null) continue;
                decimal? total = 0;
                for (var i = 0; i < 1; i++)
                {
                    var al = almacenes;
                    var prop = ty.GetProperty("almacen" + (i + 1));
                    prop.SetValue(first, al.v_Nombre, null);
                    var sum = prod.Where(r => r.idAlmacen == al.i_IdAlmacen)
                        .Sum(r => (r.tipoMovimiento == 1
                             ? r.cantidad : r.cantidad * -1)
                            * (r.devolucion == 1 ? -1 : 1));
                    total += sum;
                    prop = ty.GetProperty("cantidadAlmacen" + (i + 1));
                    prop.SetValue(first, sum, null);
                }
                first.cantidad = total;
                listReport.Add(first);
            }
            return listReport;
        }


        public List<ProgramacionFabricacion> ProgramacionFabricacion(ref OperationResult objOperationResult, DateTime Fecha)
        {
            var StockConsolidado = ReporteStockConsolidadoAlmacen(ref objOperationResult, DateTime.Parse("01/01/" + Globals.ClientSession.i_Periodo.ToString()), Fecha, "", "-1", Globals.ClientSession.i_IdEstablecimiento.Value, "", "-1", 1);
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {


                var Pedidos = (from a in dbContext.pedidodetalle

                               join b in dbContext.productodetalle on new { pd = a.v_IdProductoDetalle, eliminado = 0 } equals new { pd = b.v_IdProductoDetalle, eliminado = b.i_Eliminado.Value } into b_join

                               from b in b_join.DefaultIfEmpty()

                               join c in dbContext.producto on new { p = b.v_IdProducto, eliminado = 0, activo = 1, actfijo = 0 } equals new { p = c.v_IdProducto, eliminado = c.i_Eliminado.Value, activo = c.i_EsActivo.Value, actfijo = c.i_EsActivoFijo.Value } into c_join

                               from c in c_join.DefaultIfEmpty()

                               join d in dbContext.pedido on new { ped = a.v_IdPedido, eliminado = 0 } equals new { ped = d.v_IdPedido, eliminado = d.i_Eliminado.Value } into d_join
                               from d in d_join.DefaultIfEmpty()

                               where a.i_Eliminado == 0 && d.i_IdEstado != 3 && d.i_IdEstado == 0

                               select new ProgramacionFabricacion
                               {


                                   CodigoProducto = c.v_CodInterno,
                                   DescripcionProducto = c.v_Descripcion,
                                   IdProducto = c.v_IdProducto,
                                   CantidadFabricacion = c.i_CantidadFabricacionMensual ?? 0,
                                   CantidadPedido = a.d_CantidadEmpaque.Value,



                               }).ToList().Select(p =>
                               {

                                   var ultimostock = StockConsolidado.Any() ? StockConsolidado.Where(l => l.IdProducto == p.IdProducto).ToList() : null;
                                   var stock = ultimostock.Count > 0 && ultimostock != null ? ultimostock.LastOrDefault().cantidad.Value : 0;
                                   return new ProgramacionFabricacion
                                      {
                                          CodigoProducto = p.CodigoProducto,
                                          DescripcionProducto = p.DescripcionProducto,
                                          IdProducto = p.IdProducto,
                                          CantidadFabricacion = p.CantidadFabricacion,
                                          CantidadPedido = p.CantidadPedido,
                                          Stock = stock == null ? 0 : stock,
                                          sFechaFabricacion = Fecha.ToShortDateString(),
                                      };


                               }).ToList().GroupBy(l => new { prod = l.IdProducto }).ToList().Select(p =>
                               {

                                   var k = p.FirstOrDefault();
                                   k.CantidadPedido = p.Sum(l => l.CantidadPedido);
                                   k.SaldoUnidades = p.FirstOrDefault().Stock == 0 ? 0 : p.FirstOrDefault().Stock - p.Sum(l => l.CantidadPedido);
                                   k.SaldoDias = p.FirstOrDefault().CantidadFabricacion == 0 ? 0 : (p.FirstOrDefault().Stock * 30) / p.FirstOrDefault().CantidadFabricacion;
                                   return k;
                               }).ToList();

                return Pedidos;


            }
        }


        public List<ReporteTrazabilidad> ReporteTrazabilidad(ref OperationResult objOperationResult,DateTime FechaInicio, DateTime FechaFin,string CodigoProducto, int Almacen)
        {

            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                var NotasIngresoPT = (from a in dbContext.movimientodetalle

                                    join b in dbContext.movimiento on new { mov = a.v_IdMovimiento, eliminado = 0 } equals new { mov = b.v_IdMovimiento, eliminado = b.i_Eliminado.Value } into b_join
                                    from b in b_join.DefaultIfEmpty()
                                    join c in dbContext.productodetalle on new { pd = a.v_IdProductoDetalle, eliminado = 0 } equals new { pd = c.v_IdProductoDetalle, eliminado = c.i_Eliminado.Value } into c_join
                                    from c in c_join.DefaultIfEmpty()
                                    join d in dbContext.producto on new { p = c.v_IdProducto, eliminado = 0 } equals new { p = d.v_IdProducto, eliminado = d.i_Eliminado.Value } into d_join
                                    from d in d_join.DefaultIfEmpty()
                                    join e in dbContext.datahierarchy on new { Grupo = 17, um = a.i_IdUnidad.Value, eliminado = 0 } equals new { Grupo = 17, um = e.i_ItemId, eliminado = e.i_IsDeleted.Value } into e_join
                                    from e in e_join.DefaultIfEmpty()
                                    join f in dbContext.almacen on new { almac = b.i_IdAlmacenOrigen.Value , eliminado = 0 } equals new {almac = f.i_IdAlmacen , eliminado= f.i_Eliminado.Value  } into f_join
                                     from f in f_join.DefaultIfEmpty ()
                                    where a.i_Eliminado == 0
                                    && b.t_Fecha >= FechaInicio && b.t_Fecha <= FechaFin
                                    && b.i_IdAlmacenOrigen == Almacen && b.i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeIngreso
                                    && a.v_NroOrdenProduccion !=null && a.v_NroOrdenProduccion !=""

                                    select new
                                    {
                                        Key = d != null ? d.v_CodInterno + " " + d.v_Descripcion : "",
                                        Cantidad = a.d_Cantidad,
                                        UnidadMedida = e.v_Field,
                                        NroOrdenProduccion = a.v_NroOrdenProduccion,
                                        Almacen = f.v_Nombre ,
                                        NroRegistro =b.v_Mes +" "+ b.v_Correlativo ,


                                    }).ToList().Select(o => new ReporteTrazabilidad
                                    {
                                        Key="PRODUCTO : "+ o.Key +"CANTIDAD : "+ o.Cantidad.ToString ()+ o.UnidadMedida +" NRO. ORDEN PRODUCCIÓN : "+ o.NroOrdenProduccion ,
                                       KeyOrdenProduccion =o.NroOrdenProduccion ,
                                       Almacen =o.Almacen ,
                                       NroRegistro =o.NroRegistro ,
                                    }).ToList();

                var NotasSalidaMP = (from a in dbContext.movimientodetalle

                                     join b in dbContext.movimiento on new { mov = a.v_IdMovimiento, eliminado = 0 } equals new { mov = b.v_IdMovimiento, eliminado = b.i_Eliminado.Value } into b_join
                                     from b in b_join.DefaultIfEmpty()
                                     join c in dbContext.productodetalle on new { pd = a.v_IdProductoDetalle, eliminado = 0 } equals new { pd = c.v_IdProductoDetalle, eliminado = c.i_Eliminado.Value } into c_join
                                     from c in c_join.DefaultIfEmpty()
                                     join d in dbContext.producto on new { p = c.v_IdProducto, eliminado = 0 } equals new { p = d.v_IdProducto, eliminado = d.i_Eliminado.Value } into d_join
                                     from d in d_join.DefaultIfEmpty()
                                     join e in dbContext.datahierarchy on new { Grupo = 17, um = a.i_IdUnidad.Value, eliminado = 0 } equals new { Grupo = 17, um = e.i_ItemId, eliminado = e.i_IsDeleted.Value } into e_join
                                     from e in e_join.DefaultIfEmpty()
                                     where a.i_Eliminado == 0
                                     join f in dbContext.almacen on new { almac = b.i_IdAlmacenOrigen.Value , eliminado = 0 } equals new {almac = f.i_IdAlmacen , eliminado= f.i_Eliminado.Value  } into f_join
                                     from f in f_join.DefaultIfEmpty ()
                                     
                                     where a.i_Eliminado ==0 && b.i_IdTipoMovimiento == (int)TipoDeMovimiento.NotadeSalida

                                     select new
                                     {
                                         Key = d != null ? d.v_CodInterno + " " + d.v_Descripcion : "",
                                         Cantidad = a.d_Cantidad,
                                         UnidadMedida = e.v_Field,
                                         NroOrdenProduccion = a.v_NroOrdenProduccion,
                                         NroLote = a.v_NroLote,
                                         FechaCaducidad = a.t_FechaCaducidad,
                                         Almacen = f.v_Nombre ,
                                         NroRegistro =b.v_Mes +" "+ b.v_Correlativo ,
                                         Fecha =b.t_Fecha ,


                                     }).ToList().Select(o => new ReporteTrazabilidad
                                    {
                                        Key = "PRODUCTO : " + o.Key,
                                        KeyOrdenProduccion = o.NroOrdenProduccion,
                                        Cantidad = o.Cantidad ?? 0,
                                        NroLote = o.NroLote,
                                        FechaCaducidad = o.FechaCaducidad == null ? "" : o.FechaCaducidad.Value.ToShortDateString(),
                                        Almacen =o.Almacen ,
                                        NroRegistro =o.NroRegistro ,
                                        Fecha =o.Fecha.Value.ToShortDateString (),
                                       
                                        
                                    }).ToList();




                var NotasSalidasContenidasNotaIngreso = NotasIngresoPT.Where(o => NotasSalidaMP.Select(p => p.KeyOrdenProduccion).Contains(o.KeyOrdenProduccion)).ToList();

                return NotasSalidasContenidasNotaIngreso;
            
            
            
            
            }
        
        
        
        }
        #endregion

    }
}
