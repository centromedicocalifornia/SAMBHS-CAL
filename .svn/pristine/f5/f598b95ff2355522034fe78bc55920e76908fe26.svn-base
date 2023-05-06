using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAMBHS.Common.Resource;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.BE;
using System.ComponentModel;
using System.Transactions;
using SAMBHS.CommonWIN.BL;
using System.Linq.Dynamic;


namespace SAMBHS.Requerimientos.NBS
{
    public class OrdenTrabajoBL
    {

        //public List<KeyValueDTO> ObtenerListadoOrdenTrabajo(ref OperationResult pobjOperationResult, string pstringPeriodo, string pstringMes)
        //{
        //    try
        //    {
        //        SAMBHSEntitiesModelWin dbcontext = new SAMBHSEntitiesModelWin();
        //        string idAlmacen = Globals.ClientSession.i_IdAlmacenPredeterminado.Value.ToString("00");
        //        var query = (from n in dbcontext.nbs_ordentrabajo
        //                     where n.i_Eliminado == 0 && n.v_Periodo == pstringPeriodo && n.v_Mes == pstringMes && n.v_IdOrdenTrabajo.Substring(2, 2) == idAlmacen
        //                     orderby n.v_Correlativo ascending
        //                     select new
        //                     {
        //                         v_Correlativo = n.v_Correlativo,
        //                         v_IdOrdenTrabajo=n.v_IdOrdenTrabajo ,

        //                     }
        //                     );

        //        var query2 = query.AsEnumerable()
        //                    .Select(x => new KeyValueDTO
        //                    {
        //                        Value1 = x.v_Correlativo,
        //                        Value2 = x.v_IdOrdenTrabajo ,
        //                    }).ToList();

        //        return query2;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        public nbs_ordentrabajoDto ObtenerOrdenTrabajoCabecera(ref OperationResult pobjOperationResult, string pstrIdOrdenTrabajo)
        {
            try
            {
                //SAMBHSEntitiesModelWin dbContextWeb = new SAMBHSEntitiesModel();

                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                var objEntity = (from a in dbContext.nbs_ordentrabajo
                                 join A in dbContext.cliente on new { Cliente = a.v_IdCliente, eliminado = 0 } equals
                                     new { Cliente = A.v_IdCliente, eliminado = A.i_Eliminado.Value } into A_join
                                 from A in A_join.DefaultIfEmpty()
                                 where a.v_IdOrdenTrabajo == pstrIdOrdenTrabajo
                                 select new nbs_ordentrabajoDto

                                 {
                                     v_IdOrdenTrabajo = a.v_IdOrdenTrabajo,
                                     v_Mes = a.v_Mes,
                                     v_Correlativo = a.v_Correlativo,
                                     v_Periodo = a.v_Periodo,
                                     v_NroOrdenTrabajo = a.v_NroOrdenTrabajo,
                                     // CodigoCliente = A.v_CodCliente,
                                     RucCliente = A.v_NroDocIdentificacion,
                                     RazonSocialCliente = (A.v_ApePaterno + " " + A.v_ApeMaterno + " " + " " + A.v_PrimerNombre + " " + A.v_SegundoNombre + " " + A.v_RazonSocial).Trim(),
                                     DireccionCliente = A.v_DirecPrincipal,

                                     v_Referencia = a.v_Referencia,
                                     v_Glosa = a.v_Glosa,
                                     t_FechaRegistro = a.t_FechaRegistro,
                                     v_IdCliente = a.v_IdCliente,
                                     v_NroOrdenCompra = a.v_NroOrdenCompra,
                                     d_Total = a.d_Total,
                                     i_Eliminado = a.i_Eliminado.Value,
                                     i_ActualizaIdUsuario = a.i_ActualizaIdUsuario.Value,
                                     t_InsertaFecha = a.t_InsertaFecha.Value,
                                     t_ActualizaFecha = a.t_ActualizaFecha,
                                     i_InsertaIdUsuario = a.i_InsertaIdUsuario,
                                     d_TotalRegistral = a.d_TotalRegistral ?? 0,
                                     i_IdEstado = a.i_IdEstado ?? 0,
                                     v_IdResponsable = a.v_IdResponsable,


                                 }).FirstOrDefault();
                pobjOperationResult.Success = 1;
                return objEntity;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                return null;
            }
        }
        public BindingList<nbs_ordentrabajodetalleDto> ObtenerDetalle(ref OperationResult objOperationResult, string v_IdOrdenTrabajo)
        {
            try
            {
                string Orden = "v_IdOrdenTrabajoDetalle";
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    var ordentrabajodetalle = (from b in dbContext.nbs_ordentrabajodetalle


                                               join c in dbContext.productodetalle on new { ProdDet = b.v_IdProductoDetalle, eliminado = 0 } equals new { ProdDet = c.v_IdProductoDetalle, eliminado = c.i_Eliminado.Value } into c_join

                                               from c in c_join.DefaultIfEmpty()

                                               join d in dbContext.producto on new { Prod = c.v_IdProducto, eliminado = 0 } equals new { Prod = d.v_IdProducto, eliminado = d.i_Eliminado.Value } into d_join

                                               from d in d_join.DefaultIfEmpty()


                                               where b.i_Eliminado == 0 && b.v_IdOrdenTrabajo == v_IdOrdenTrabajo

                                               select new nbs_ordentrabajodetalleDto
                                              {
                                                  v_IdOrdenTrabajoDetalle = b.v_IdOrdenTrabajoDetalle,
                                                  v_IdProductoDetalle = b.v_IdProductoDetalle,
                                                  i_Cantidad = b.i_Cantidad.Value,
                                                  CodigoProducto = d.v_CodInterno,
                                                  DescripcionProducto = b.v_DescripcionTemporal == null || b.v_DescripcionTemporal == string.Empty ? d.v_Descripcion.Trim() : b.v_DescripcionTemporal.Trim(),
                                                  d_Importe = b.d_Importe.Value,
                                                  d_Total = b.d_Total.Value,
                                                  v_IdOrdenTrabajo = b.v_IdOrdenTrabajo,
                                                  i_Eliminado = b.i_Eliminado.Value,
                                                  i_ActualizaIdUsuario = b.i_ActualizaIdUsuario,
                                                  i_InsertaIdUsuario = b.i_InsertaIdUsuario,
                                                  t_ActualizaFecha = b.t_ActualizaFecha,
                                                  t_InsertaFecha = b.t_InsertaFecha,
                                                  i_UsadoEnFUF = b.i_UsadoEnFUF == null ? 0 : b.i_UsadoEnFUF.Value,
                                                  EsNombreEditable = d.i_NombreEditable ?? 0,
                                                  d_ImporteRegistral = b.d_ImporteRegistral ?? 0,


                                              });

                    ordentrabajodetalle = ordentrabajodetalle.OrderBy(Orden);

                    var ordentrabajodetalleFinal = new BindingList<nbs_ordentrabajodetalleDto>(ordentrabajodetalle.ToList());

                    objOperationResult.Success = 1;
                    return ordentrabajodetalleFinal;
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;

            }

        }
        public string BuscarNumeroFormatoUnico(string ProductoDetalle, string OrdenTrabajo)
        {

            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {

                var fu = (from a in dbContext.nbs_formatounicofacturacion

                          join b in dbContext.nbs_formatounicofacturaciondetalle on new { fut = a.v_IdFormatoUnicoFacturacion, eliminado = 0 } equals new { fut = b.v_IdFormatoUnicoFacturacion, eliminado = b.i_Eliminado.Value } into b_join

                          from b in b_join.DefaultIfEmpty()


                          where b.v_IdProductoDetalle == ProductoDetalle && b.v_IdOrdenTrabajo == OrdenTrabajo && a.i_Eliminado == 0

                          select new
                          {
                              NumeroFU = a.v_NroFormato,

                          }).FirstOrDefault();
                if (fu != null)
                {

                    return fu.NumeroFU;
                }
                else
                {
                    return string.Empty;
                }
            }

        }


        public string BuscarNumeroFormatoUnico(string OrdenTrabajo)
        {

            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {

                var fu = (from a in dbContext.nbs_formatounicofacturacion

                          join b in dbContext.nbs_formatounicofacturaciondetalle on new { fut = a.v_IdFormatoUnicoFacturacion, eliminado = 0 } equals new { fut = b.v_IdFormatoUnicoFacturacion, eliminado = b.i_Eliminado.Value } into b_join

                          from b in b_join.DefaultIfEmpty()


                          where b.v_IdOrdenTrabajo == OrdenTrabajo && a.i_Eliminado == 0

                          select new
                          {
                              NumeroFU = a.v_NroFormato,

                          }).FirstOrDefault();
                if (fu != null)
                {

                    return fu.NumeroFU;
                }
                else
                {
                    return string.Empty;
                }
            }

        }
        public List<nbs_ordentrabajoDto> ListarBusquedaOrdenTrabajo(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression, DateTime F_Inicio, DateTime F_Fin)
        {
            try
            {
                List<systemuserDto> ListaUsuarios = new List<systemuserDto>();

                //int IdUsuario=  Int32.Parse(Globals.ClientSession.GetAsList()[2]);

                //using (SAMBHSEntitiesModel dbContextWeb = new SAMBHSEntitiesModel())
                //{

                //    ListaUsuarios = (from a in dbContextWeb.systemuser
                //                         join b in dbContextWeb.person on new { Persona = a.i_PersonId.Value , eliminado = 0 } equals new { Persona = b.i_PersonId, eliminado = b.i_IsDeleted.Value } into b_join
                //                         from b in b_join.DefaultIfEmpty()
                //                         select new systemuserDto 
                //                         {
                //                             v_Password =b==null ?"": b.v_FirstName +" "+ b.v_FirstLastName ,
                //                             i_SystemUserId = a.i_SystemUserId ,
                //                         }).ToList();



                //}

                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from a in dbContext.nbs_ordentrabajo

                                 join b in dbContext.cliente on new { cliente = a.v_IdCliente, eliminado = 0 } equals new { cliente = b.v_IdCliente, eliminado = b.i_Eliminado.Value } into b_join
                                 from b in b_join.DefaultIfEmpty()

                                 join c in dbContext.systemuser on new { usuarioCreacion = a.i_InsertaIdUsuario.Value } equals new { usuarioCreacion = c.i_SystemUserId } into c_join

                                 from c in c_join.DefaultIfEmpty()

                                 join d in dbContext.systemuser on new { usuarioMofidicacion = a.i_ActualizaIdUsuario.Value } equals new { usuarioMofidicacion = d.i_SystemUserId } into d_join

                                 from d in d_join.DefaultIfEmpty()

                                 join e in dbContext.cliente on new { Responsable = a.v_IdResponsable, eliminado = 0 } equals new { Responsable = e.v_IdCliente, eliminado = e.i_Eliminado.Value } into e_join
                                 from e in e_join.DefaultIfEmpty()

                                 where a.i_Eliminado == 0 && a.t_FechaRegistro >= F_Inicio && a.t_FechaRegistro <= F_Fin //&& a.i_InsertaIdUsuario == IdUsuario


                                 select new nbs_ordentrabajoDto
                                 {
                                     RazonSocialCliente = (b.v_ApePaterno + " " + b.v_ApeMaterno + " " + b.v_PrimerNombre + " " + b.v_SegundoNombre + " " + b.v_RazonSocial).Trim(),
                                     t_FechaRegistro = a.t_FechaRegistro,
                                     v_NroOrdenTrabajo = a.v_NroOrdenTrabajo,
                                     Responsable = e.v_PrimerNombre,
                                     d_Total = a.d_Total,
                                     v_IdOrdenTrabajo = a.v_IdOrdenTrabajo,
                                     i_InsertaIdUsuario = a.i_InsertaIdUsuario,
                                     v_IdCliente = a.v_IdCliente,
                                     UsuarioCreacion = c == null ? "" : c.v_UserName,
                                     UsuarioModificacion = d == null ? "" : d.v_UserName,
                                     t_ActualizaFecha = a.t_ActualizaFecha.Value,
                                     t_InsertaFecha = a.t_InsertaFecha.Value,
                                     i_IdEstado = a.i_IdEstado ?? 1,



                                 }).ToList().AsQueryable();


                    if (!string.IsNullOrEmpty(pstrFilterExpression))
                    {

                        query = query.Where(pstrFilterExpression);
                    }


                    var ff = query.ToList().Select(n =>
                    {
                        var FU = (from a in dbContext.nbs_formatounicofacturaciondetalle
                                  join b in dbContext.nbs_formatounicofacturacion on new { fu = a.v_IdFormatoUnicoFacturacion, eliminado = 0 } equals new { fu = b.v_IdFormatoUnicoFacturacion, eliminado = b.i_Eliminado.Value } into b_join

                                  from b in b_join.DefaultIfEmpty()
                                  where a.v_IdOrdenTrabajo == n.v_IdOrdenTrabajo && a.i_Eliminado == 0
                                  select new
                                  {
                                      NroFuf = b.v_NroFormato.Trim(),
                                  }).FirstOrDefault();

                        return new nbs_ordentrabajoDto
                        {


                            RazonSocialCliente = n.RazonSocialCliente,
                            t_FechaRegistro = n.t_FechaRegistro,
                            v_NroOrdenTrabajo = n.v_NroOrdenTrabajo,
                            Responsable = n.Responsable, // ListaUsuarios.Where(x => x.i_SystemUserId == n.i_InsertaIdUsuario.Value).Select(x => x.v_Password).FirstOrDefault(),
                            d_Total = n.d_Total,
                            v_IdOrdenTrabajo = n.v_IdOrdenTrabajo,
                            i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                            UsuarioModificacion = n.UsuarioCreacion,
                            UsuarioCreacion = n.UsuarioCreacion,
                            t_InsertaFecha = n.t_InsertaFecha,
                            t_ActualizaFecha = n.t_ActualizaFecha,
                            UsadoFUF = FU != null ? 1 : 0,
                            NroFUF = FU != null ? FU.NroFuf : "",
                            i_IdEstado = n.i_IdEstado.Value,

                        };

                    }).ToList().OrderBy(x => x.v_NroOrdenTrabajo).ToList();
                    pobjOperationResult.Success = 1;

                    return ff;


                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                return null;
            }
        }

        public bool ExisteNroRegistro(string Periodo, string Mes, string Correlativo)
        {
            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

            var Registro = (from n in dbContext.nbs_ordentrabajo
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

        public bool ExisteNroDocumento(string pstrCorrelativo)
        {
            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
            var Registro = (from n in dbContext.nbs_ordentrabajo
                            where n.i_Eliminado == 0 && n.v_NroOrdenTrabajo == pstrCorrelativo
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

        public string InsertarOrdenTrabajo(ref OperationResult pobjOperationResult, nbs_ordentrabajoDto pobjOrdenTrabajoDto,
           List<nbs_ordentrabajodetalleDto> ListaOrdenTrabajoDetalle,
           List<string> ClientSession)
        {
            try
            {

                DocumentoBL _objDocumentoBL = new DocumentoBL();


                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        SecuentialBL objSecuentialBL = new SecuentialBL();


                        nbs_ordentrabajo objEntityOrdenTrabajo = nbs_ordentrabajoAssembler.ToEntity(pobjOrdenTrabajoDto);
                        int SecuentialId = 0;
                        string newIdOrdenT = string.Empty;
                        string newIdOTDetalle = string.Empty;
                        int intNodeId;

                        #region Inserta Cabecera

                        objEntityOrdenTrabajo.t_InsertaFecha = DateTime.Now;
                        objEntityOrdenTrabajo.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntityOrdenTrabajo.i_Eliminado = 0;

                        intNodeId = int.Parse(ClientSession[0]);
                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 102);
                        newIdOrdenT = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "OT");
                        objEntityOrdenTrabajo.v_IdOrdenTrabajo = newIdOrdenT;
                        dbContext.AddTonbs_ordentrabajo(objEntityOrdenTrabajo);

                        #endregion
                        #region Inserta Letras Detalle

                        foreach (var OTDetalle in ListaOrdenTrabajoDetalle)
                        {
                            nbs_ordentrabajodetalle _otdetalle = new nbs_ordentrabajodetalle();

                            _otdetalle = nbs_ordentrabajodetalleAssembler.ToEntity(OTDetalle);
                            SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 103);
                            newIdOTDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "OD");
                            _otdetalle.v_IdOrdenTrabajoDetalle = newIdOTDetalle;
                            _otdetalle.v_IdOrdenTrabajo = newIdOrdenT;
                            _otdetalle.t_InsertaFecha = DateTime.Now;
                            _otdetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                            _otdetalle.i_Eliminado = 0;
                            OTDetalle.v_IdOrdenTrabajoDetalle = newIdOTDetalle;
                            dbContext.AddTonbs_ordentrabajodetalle(_otdetalle);
                            Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "nbs_ordentrabajodetalle", newIdOTDetalle);

                        }
                        #endregion
                        #region ActualizaCorrelativoDocumento

                        _objDocumentoBL.ActualizarCorrelativoPorSerie(ref pobjOperationResult, Globals.ClientSession.i_IdEstablecimiento, (int)TiposDocumentos.OrdenTrabajo, Globals.ClientSession.i_Periodo.Value.ToString(), int.Parse(pobjOrdenTrabajoDto.v_NroOrdenTrabajo) + 1);

                        #endregion
                        dbContext.SaveChanges();
                        ts.Complete();
                        pobjOperationResult.Success = 1;
                        return newIdOrdenT;


                    }
                }
            }

            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "OrdenTrabajoBL.InsertarOrdenTrabajo()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }



        public string ActualizaOrdenTrabajo(ref OperationResult poObjOperationResult, nbs_ordentrabajoDto pobOrdenTrabajo, List<nbs_ordentrabajodetalleDto> ListaOrdenTrabajoDetalle_Agregar, List<nbs_ordentrabajodetalleDto> ListaOrdenTrabajoDetalle_Modificar,
            List<nbs_ordentrabajodetalleDto> ListaOrdenTrabajoDetalle_Eliminar,
           List<string> ClientSession)
        {

            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        OperationResult objOperationResult = new OperationResult();
                        SecuentialBL objSecuentialBL = new SecuentialBL();

                        int SecuentialId = 0;
                        string newIdOrdenT = string.Empty;
                        string newIdOTDetalle = string.Empty;
                        int intNodeId = -1;


                        #region Actualiza Cabecera
                        intNodeId = int.Parse(ClientSession[0]);
                        nbs_ordentrabajo objEntity = (from a in dbContext.nbs_ordentrabajo
                                                      where a.v_IdOrdenTrabajo == pobOrdenTrabajo.v_IdOrdenTrabajo
                                                      select a).FirstOrDefault();

                        pobOrdenTrabajo.t_ActualizaFecha = DateTime.Now;
                        pobOrdenTrabajo.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                        objEntity = nbs_ordentrabajoAssembler.ToEntity(pobOrdenTrabajo);
                        dbContext.nbs_ordentrabajo.ApplyCurrentValues(objEntity);
                        #endregion
                        #region Actualiza Detalle
                        foreach (nbs_ordentrabajodetalleDto otdetalleDto in ListaOrdenTrabajoDetalle_Agregar)
                        {
                            nbs_ordentrabajodetalle objEntityOTDetalle = nbs_ordentrabajodetalleAssembler.ToEntity(otdetalleDto);

                            SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 103);
                            newIdOTDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "OD");
                            objEntityOTDetalle.v_IdOrdenTrabajoDetalle = newIdOTDetalle;

                            objEntityOTDetalle.v_IdOrdenTrabajo = pobOrdenTrabajo.v_IdOrdenTrabajo;
                            objEntityOTDetalle.t_InsertaFecha = DateTime.Now;
                            objEntityOTDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                            objEntityOTDetalle.i_Eliminado = 0;
                            dbContext.AddTonbs_ordentrabajodetalle(objEntityOTDetalle);
                            Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "nbs_ordentrabajodetalle", newIdOTDetalle);

                        }

                        foreach (nbs_ordentrabajodetalleDto otdetalleDto in ListaOrdenTrabajoDetalle_Modificar)
                        {
                            nbs_ordentrabajodetalle _objEntity = nbs_ordentrabajodetalleAssembler.ToEntity(otdetalleDto);
                            var query = (from n in dbContext.nbs_ordentrabajodetalle
                                         where n.v_IdOrdenTrabajoDetalle == otdetalleDto.v_IdOrdenTrabajoDetalle
                                         select n).FirstOrDefault();

                            _objEntity.t_ActualizaFecha = DateTime.Now;
                            _objEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            dbContext.nbs_ordentrabajodetalle.ApplyCurrentValues(_objEntity);
                            dbContext.SaveChanges();
                            Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "nbs_ordentrabajodetalle", query.v_IdOrdenTrabajoDetalle);

                        }
                        foreach (nbs_ordentrabajodetalleDto otdetalleDto in ListaOrdenTrabajoDetalle_Eliminar)
                        {
                            nbs_ordentrabajodetalle _objEntity = nbs_ordentrabajodetalleAssembler.ToEntity(otdetalleDto);

                            var query = (from n in dbContext.nbs_ordentrabajodetalle
                                         where n.v_IdOrdenTrabajoDetalle == otdetalleDto.v_IdOrdenTrabajoDetalle
                                         select n).FirstOrDefault();

                            if (query != null)
                            {
                                query.t_ActualizaFecha = DateTime.Now;
                                query.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                                query.i_Eliminado = 1;


                            }

                            dbContext.nbs_ordentrabajodetalle.ApplyCurrentValues(query);
                            Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "nbs_ordentrabajodetalle", query.v_IdOrdenTrabajoDetalle);
                            dbContext.SaveChanges();
                        }
                        #endregion
                        dbContext.SaveChanges();
                        poObjOperationResult.Success = 1;
                        ts.Complete();
                        return pobOrdenTrabajo.v_IdOrdenTrabajo;
                    }
                }
            }

            catch (Exception ex)
            {
                poObjOperationResult.Success = 0;
                poObjOperationResult.AdditionalInformation = "OrdenTrabajoBL.ActualizarOrdenTrabajo()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                poObjOperationResult.ErrorMessage = ex.Message;
                poObjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, poObjOperationResult);
                return null;

            }

        }




        public void EliminarOrdenTrabajo(ref OperationResult pobjOperationResult, string pstrIdOrdenTrabajo, List<string> ClientSession)
        {

            OperationResult objOperationResult = new OperationResult();
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                    #region Elimina Cabecera
                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.nbs_ordentrabajo
                                           where a.v_IdOrdenTrabajo == pstrIdOrdenTrabajo
                                           select a).FirstOrDefault();

                    // Crear la entidad con los datos actualizados
                    objEntitySource.t_ActualizaFecha = DateTime.Now;
                    objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntitySource.i_Eliminado = 1;
                    Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "OrdenTrabajo", objEntitySource.v_IdOrdenTrabajo);
                    #endregion

                    #region Elimina Detalles
                    //Eliminar detalles del movimiento eliminado.
                    var objEntitySourceDetallesPedido = (from a in dbContext.nbs_ordentrabajodetalle
                                                         where a.v_IdOrdenTrabajo == pstrIdOrdenTrabajo
                                                         select a).ToList();

                    foreach (var OrdenTrabajoDetalle in objEntitySourceDetallesPedido)
                    {
                        OrdenTrabajoDetalle.t_ActualizaFecha = DateTime.Now;
                        OrdenTrabajoDetalle.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        OrdenTrabajoDetalle.i_Eliminado = 1;
                        Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "OrdenTrabajoDetalle", OrdenTrabajoDetalle.v_IdOrdenTrabajoDetalle);

                    }
                    #endregion
                    dbContext.SaveChanges();
                    ts.Complete();
                }
            }
            catch (Exception ex)
            {

                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return;
            }
        }
    }
}
