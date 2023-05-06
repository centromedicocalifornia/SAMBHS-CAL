using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.BE.Custom;

namespace SAMBHS.Requerimientos.NBS
{
    /// <summary>
    /// Lógica del negocio para los Formatos únicos de Facturación por requerimiento de la Notaría Becerra Sosaya
    /// Dic-2015 EQC
    /// </summary>
    public static class FormatoUnicoFacturacionBl
    {
        /// <summary>
        /// Obtiene los datos para la bandeja de los formatos unicos de facturación.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pdateFechaInicio"></param>
        /// <param name="pdateFechaFin"></param>
        /// <returns></returns>
        public static List<nbs_formatounicofacturacionDto> ObtenerFormatosUnicosBandeja(ref OperationResult pobjOperationResult, DateTime pdateFechaInicio,
            DateTime pdateFechaFin, string IdCliente, string TipoDoc)
        {
            try
            {

                //i_FacturadoContabilidad ,1 se utilizarà como irpe , 0 se utilizarà en fac o boleta
                List<nbs_formatounicofacturacionDto> consulta = new List<nbs_formatounicofacturacionDto>();
                List<nbs_formatounicofacturacionDto> consultaFinal = new List<nbs_formatounicofacturacionDto>();
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {


                    List<venta> venta = dbContext.venta.ToList();
                    List<documento> documento = dbContext.documento.ToList();

                    if (TipoDoc == "N")
                    {
                        consulta = (from n in dbContext.nbs_formatounicofacturacion
                                    join J1 in dbContext.systemuser on n.i_InsertaIdUsuario equals J1.i_SystemUserId into J1_join
                                    from J1 in J1_join.DefaultIfEmpty()
                                    join J2 in dbContext.systemuser on n.i_ActualizaIdUsuario equals J2.i_SystemUserId into J2_join
                                    from J2 in J2_join.DefaultIfEmpty()


                                    where n.t_FechaRegistro.Value >= pdateFechaInicio && n.t_FechaRegistro <= pdateFechaFin && (n.v_IdCliente == IdCliente || IdCliente == string.Empty) && n.i_Eliminado == 0


                                    select new
                                    {
                                        t_FechaRegistro = n.t_FechaRegistro,
                                        v_NroFormato = n.v_NroFormato,
                                        i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                        UsuarioCreacion = J1.v_UserName,
                                        UsuarioModificacion = J2.v_UserName,
                                        cliente = n.cliente,
                                        FechaCrea = n.t_InsertaFecha,
                                        FechaAct = n.t_ActualizaFecha,
                                        Facturado = n.i_Facturado,
                                        v_IdFormatoUnicoFacturacion = n.v_IdFormatoUnicoFacturacion.Trim(),
                                        d_Total = n.d_Total ?? 0,
                                        i_FacturadoF = n.i_Facturado == null ? 0 : n.i_Facturado == 1 ? 1 : 0,
                                        NroDocumentoVenta = "",

                                    })
                                            .ToList()
                                            .Select(o =>
                                            {
                                                string nrodoc = string.Empty;
                                                int Cont = 1;
                                                var Venta = (from a in venta
                                                             join b in documento on new { doc = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { doc = b.i_CodigoDocumento, eliminado = b.i_Eliminado.Value }
                                                             where a.v_IdFormatoUnicoFacturacion == o.v_IdFormatoUnicoFacturacion && a.i_Eliminado == 0 && a.i_IdEstado == 1

                                                             select new
                                                             {
                                                                 NroDocumentoVenta = b.v_Siglas + " " + a.v_SerieDocumento + " " + a.v_CorrelativoDocumento,

                                                             }).ToList();
                                                int Nroreg = Venta.Count();
                                                foreach (var item in Venta)
                                                {
                                                    if (Nroreg == 1)
                                                    {
                                                        nrodoc = item.NroDocumentoVenta;
                                                    }
                                                    else if (Nroreg == Cont)
                                                    {
                                                        nrodoc = nrodoc + " " + item.NroDocumentoVenta;
                                                    }
                                                    else
                                                    {
                                                        nrodoc = item.NroDocumentoVenta + " , " + nrodoc;
                                                    }
                                                    Cont = Cont + 1;
                                                }

                                                return new nbs_formatounicofacturacionDto
                                                {
                                                    t_FechaRegistro = o.t_FechaRegistro,
                                                    UsuarioResponsable = DevolverUsuarioResponsable(o.i_InsertaIdUsuario.Value),
                                                    v_NroFormato = o.v_NroFormato,
                                                    NombreCliente =
                                                        (o.cliente.v_ApePaterno + " " + o.cliente.v_ApeMaterno + " " + o.cliente.v_PrimerNombre +
                                                        " " + o.cliente.v_SegundoNombre + " " + o.cliente.v_RazonSocial).Trim(),
                                                    UsuarioCreacion = o.UsuarioCreacion,
                                                    UsuarioModificacion = o.UsuarioModificacion,
                                                    t_InsertaFecha = o.FechaCrea,
                                                    t_ActualizaFecha = o.FechaAct,
                                                    i_Facturado = o.Facturado ?? 0,

                                                    v_IdFormatoUnicoFacturacion = o.v_IdFormatoUnicoFacturacion.Trim(),
                                                    d_Total = o.d_Total,
                                                    i_FacturadoF = o.i_FacturadoF,
                                                    NroDocumentoVenta = nrodoc,
                                                };

                                            }
                                            ).ToList().OrderBy(x => x.v_NroFormato).ToList();
                        pobjOperationResult.Success = 1;
                    }
                    else
                    {


                        if (TipoDoc == ((int)TiposDocumentos.Irpe).ToString())
                        {
                            consulta = (from a in dbContext.nbs_formatounicofacturaciondetalle

                                        join b in dbContext.nbs_formatounicofacturacion on new { fu = a.v_IdFormatoUnicoFacturacion, eliminado = 0 } equals new { fu = b.v_IdFormatoUnicoFacturacion, eliminado = b.i_Eliminado.Value }

                                        join J1 in dbContext.systemuser on b.i_InsertaIdUsuario equals J1.i_SystemUserId into J1_join
                                        from J1 in J1_join.DefaultIfEmpty()
                                        join J2 in dbContext.systemuser on b.i_ActualizaIdUsuario equals J2.i_SystemUserId into J2_join
                                        from J2 in J2_join.DefaultIfEmpty()

                                        where b.t_FechaRegistro.Value >= pdateFechaInicio && b.t_FechaRegistro <= pdateFechaFin && (b.v_IdCliente == IdCliente || IdCliente == string.Empty) && a.i_Eliminado == 0

                                        && (a.i_UsadoIrpe == null || a.i_UsadoIrpe == 0) && a.i_FacturadoContabilidad == 1

                                        select new
                                        {
                                            t_FechaRegistro = b.t_FechaRegistro,
                                            v_NroFormato = b.v_NroFormato,
                                            i_InsertaIdUsuario = b.i_InsertaIdUsuario,
                                            UsuarioCreacion = J1.v_UserName,
                                            UsuarioModificacion = J2.v_UserName,
                                            cliente = b.cliente,
                                            FechaCrea = b.t_InsertaFecha,
                                            FechaAct = b.t_ActualizaFecha,
                                            Facturado = b.i_Facturado,
                                            v_IdFormatoUnicoFacturacion = b.v_IdFormatoUnicoFacturacion.Trim(),
                                            d_Total = b.d_Total ?? 0,
                                            i_FacturadoF = b.i_Facturado == null ? 0 : b.i_Facturado == 1 ? 1 : 0,
                                            NroDocumentoVenta = "",
                                            v_IdFormatoUnicoFacturacionDetalle = a.v_IdFormatoUnicoFacturacionDetalle,

                                        })
                                         .ToList()
                                         .Select(o =>
                                         {
                                             string nrodoc = string.Empty;
                                             int Cont = 1;
                                             var Venta = (from a in venta
                                                          join b in documento on new { doc = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { doc = b.i_CodigoDocumento, eliminado = b.i_Eliminado.Value }
                                                          where a.v_IdFormatoUnicoFacturacion == o.v_IdFormatoUnicoFacturacion && a.i_Eliminado == 0 && a.i_IdEstado == 1

                                                          select new
                                                          {
                                                              NroDocumentoVenta = b.v_Siglas + " " + a.v_SerieDocumento + " " + a.v_CorrelativoDocumento,

                                                          }).ToList();
                                             int Nroreg = Venta.Count();
                                             foreach (var item in Venta)
                                             {
                                                 if (Nroreg == 1)
                                                 {
                                                     nrodoc = item.NroDocumentoVenta;
                                                 }
                                                 else if (Nroreg == Cont)
                                                 {
                                                     nrodoc = nrodoc + " " + item.NroDocumentoVenta;
                                                 }
                                                 else
                                                 {
                                                     nrodoc = item.NroDocumentoVenta + " , " + nrodoc;
                                                 }
                                                 Cont = Cont + 1;
                                             }

                                             return new nbs_formatounicofacturacionDto
                                             {
                                                 t_FechaRegistro = o.t_FechaRegistro,
                                                 UsuarioResponsable = DevolverUsuarioResponsable(o.i_InsertaIdUsuario.Value),
                                                 v_NroFormato = o.v_NroFormato,
                                                 NombreCliente =
                                                     (o.cliente.v_ApePaterno + " " + o.cliente.v_ApeMaterno + " " + o.cliente.v_PrimerNombre +
                                                     " " + o.cliente.v_SegundoNombre + " " + o.cliente.v_RazonSocial).Trim(),
                                                 UsuarioCreacion = o.UsuarioCreacion,
                                                 UsuarioModificacion = o.UsuarioModificacion,
                                                 t_InsertaFecha = o.FechaCrea,
                                                 t_ActualizaFecha = o.FechaAct,
                                                 i_Facturado = o.Facturado ?? 0,

                                                 v_IdFormatoUnicoFacturacion = o.v_IdFormatoUnicoFacturacion.Trim(),
                                                 d_Total = o.d_Total,
                                                 i_FacturadoF = o.i_FacturadoF,
                                                 NroDocumentoVenta = nrodoc,
                                                 v_IdFormatoUnicoFacturacionDetalle = o.v_IdFormatoUnicoFacturacionDetalle,

                                             };

                                         }
                                         ).ToList().GroupBy(x => new { x.v_IdFormatoUnicoFacturacion })
                                           .Select(group => group.Last())
                                              .OrderBy(o => o.v_NroFormato).ToList();
                        }
                        else
                        {
                            consulta = (from a in dbContext.nbs_formatounicofacturaciondetalle

                                        join b in dbContext.nbs_formatounicofacturacion on new { fu = a.v_IdFormatoUnicoFacturacion, eliminado = 0 } equals new { fu = b.v_IdFormatoUnicoFacturacion, eliminado = b.i_Eliminado.Value }

                                        join J1 in dbContext.systemuser on b.i_InsertaIdUsuario equals J1.i_SystemUserId into J1_join
                                        from J1 in J1_join.DefaultIfEmpty()
                                        join J2 in dbContext.systemuser on b.i_ActualizaIdUsuario equals J2.i_SystemUserId into J2_join
                                        from J2 in J2_join.DefaultIfEmpty()

                                        where b.t_FechaRegistro.Value >= pdateFechaInicio && b.t_FechaRegistro <= pdateFechaFin && (b.v_IdCliente == IdCliente || IdCliente == string.Empty) && a.i_Eliminado == 0

                                        && (a.i_UsadoVenta == null || a.i_UsadoVenta == 0)

                                        select new
                                        {
                                            t_FechaRegistro = b.t_FechaRegistro,
                                            v_NroFormato = b.v_NroFormato,
                                            i_InsertaIdUsuario = b.i_InsertaIdUsuario,
                                            UsuarioCreacion = J1.v_UserName,
                                            UsuarioModificacion = J2.v_UserName,
                                            cliente = b.cliente,
                                            FechaCrea = b.t_InsertaFecha,
                                            FechaAct = b.t_ActualizaFecha,
                                            Facturado = b.i_Facturado,
                                            v_IdFormatoUnicoFacturacion = b.v_IdFormatoUnicoFacturacion.Trim(),
                                            d_Total = b.d_Total ?? 0,
                                            i_FacturadoF = b.i_Facturado == null ? 0 : b.i_Facturado == 1 ? 1 : 0,
                                            NroDocumentoVenta = "",
                                            v_IdFormatoUnicoFacturacionDetalle = a.v_IdFormatoUnicoFacturacionDetalle,

                                        })
                                               .ToList()
                                               .Select(o =>
                                               {
                                                   string nrodoc = string.Empty;
                                                   int Cont = 1;
                                                   var Venta = (from a in venta
                                                                join b in  documento on new { doc = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { doc = b.i_CodigoDocumento, eliminado = b.i_Eliminado.Value }
                                                                where a.v_IdFormatoUnicoFacturacion == o.v_IdFormatoUnicoFacturacion && a.i_Eliminado == 0 && a.i_IdEstado == 1

                                                                select new
                                                                {
                                                                    NroDocumentoVenta = b.v_Siglas + " " + a.v_SerieDocumento + " " + a.v_CorrelativoDocumento,

                                                                }).ToList();
                                                   int Nroreg = Venta.Count();
                                                   foreach (var item in Venta)
                                                   {
                                                       if (Nroreg == 1)
                                                       {
                                                           nrodoc = item.NroDocumentoVenta;
                                                       }
                                                       else if (Nroreg == Cont)
                                                       {
                                                           nrodoc = nrodoc + " " + item.NroDocumentoVenta;
                                                       }
                                                       else
                                                       {
                                                           nrodoc = item.NroDocumentoVenta + " , " + nrodoc;
                                                       }
                                                       Cont = Cont + 1;
                                                   }

                                                   return new nbs_formatounicofacturacionDto
                                                   {
                                                       t_FechaRegistro = o.t_FechaRegistro,
                                                       UsuarioResponsable = DevolverUsuarioResponsable(o.i_InsertaIdUsuario.Value),
                                                       v_NroFormato = o.v_NroFormato,
                                                       NombreCliente =
                                                           (o.cliente.v_ApePaterno + " " + o.cliente.v_ApeMaterno + " " + o.cliente.v_PrimerNombre +
                                                           " " + o.cliente.v_SegundoNombre + " " + o.cliente.v_RazonSocial).Trim(),
                                                       UsuarioCreacion = o.UsuarioCreacion,
                                                       UsuarioModificacion = o.UsuarioModificacion,
                                                       t_InsertaFecha = o.FechaCrea,
                                                       t_ActualizaFecha = o.FechaAct,
                                                       i_Facturado = o.Facturado ?? 0,

                                                       v_IdFormatoUnicoFacturacion = o.v_IdFormatoUnicoFacturacion.Trim(),
                                                       d_Total = o.d_Total,
                                                       i_FacturadoF = o.i_FacturadoF,
                                                       NroDocumentoVenta = nrodoc,
                                                       v_IdFormatoUnicoFacturacionDetalle = o.v_IdFormatoUnicoFacturacionDetalle,

                                                   };

                                               }
                                               ).ToList().GroupBy(x => new { x.v_IdFormatoUnicoFacturacion })
                                                 .Select(group => group.Last())
                                                    .OrderBy(o => o.v_NroFormato).ToList();
                        }
                        pobjOperationResult.Success = 1;
                    }

                    return consulta;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "FormatoUnicoFacturacionBL.ObtenerFormatosUnicosBandeja()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        /// <summary>
        /// Obtiene los usuarios responsables cuyos datos se encuentran en la base de seguridad.
        /// </summary>
        /// <param name="pintIdUsuarioCreacion">Id del usuario de creacion.</param>
        /// <returns></returns>
        private static string DevolverUsuarioResponsable(int pintIdUsuarioCreacion)
        {
            using (var dbContextSeg = new SAMBHSEntitiesModel())
            {
                var usuario = dbContextSeg.systemuser.FirstOrDefault(p => p.i_SystemUserId == pintIdUsuarioCreacion);

                if (usuario != null)
                {
                    if (usuario.person != null)
                        return usuario.person.v_FirstLastName + " " + usuario.person.v_SecondLastName + " " + usuario.person.v_FirstName;
                }

                return "Usuario no existe";
            }
        }

        /// <summary>
        /// Inserta en la tabla de formato unico de facturacion y sus detalles.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pobjFormatounicofacturacionDto"></param>
        /// <param name="pobjFormatounicofacturaciondetalleDtos"></param>
        /// <param name="clientSession"></param>
        public static string InsertarFormatoUnicoFacturacion(ref OperationResult pobjOperationResult,
            nbs_formatounicofacturacionDto pobjFormatounicofacturacionDto, List<nbs_formatounicofacturaciondetalleDto> pobjFormatounicofacturaciondetalleDtos,
            List<string> clientSession)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var objEntityFuf = pobjFormatounicofacturacionDto.ToEntity();
                        var objSecuentialBl = new SecuentialBL();

                        #region Inserta Cabecera
                        objEntityFuf.t_InsertaFecha = DateTime.Now;
                        objEntityFuf.i_InsertaIdUsuario = Int32.Parse(clientSession[2]);
                        objEntityFuf.i_Eliminado = 0;
                        var intNodeId = int.Parse(clientSession[0]);
                        var SecuentialId = objSecuentialBl.GetNextSecuentialId(intNodeId, 101);
                        var newIdFormatoUnico = Utils.GetNewId(int.Parse(clientSession[0]), SecuentialId, "FU");
                        objEntityFuf.v_IdFormatoUnicoFacturacion = newIdFormatoUnico;
                        dbContext.AddTonbs_formatounicofacturacion(objEntityFuf);
                        #endregion

                        #region Inserta Detalles
                        foreach (var objEntityDetalle in pobjFormatounicofacturaciondetalleDtos.Select(objDetalle => objDetalle.ToEntity()))
                        {
                            SecuentialId = objSecuentialBl.GetNextSecuentialId(intNodeId, 104);
                            var newIdFormatoUnicoDetalle = Utils.GetNewId(int.Parse(clientSession[0]), SecuentialId, "FE");
                            objEntityDetalle.v_IdFormatoUnicoFacturacionDetalle = newIdFormatoUnicoDetalle;
                            objEntityDetalle.v_IdFormatoUnicoFacturacion = newIdFormatoUnico;
                            objEntityDetalle.t_InsertaFecha = DateTime.Now;
                            objEntityDetalle.i_InsertaIdUsuario = Int32.Parse(clientSession[2]);
                            objEntityDetalle.i_Eliminado = 0;
                            ActualizarDetalleOT(ref pobjOperationResult, objEntityDetalle.v_IdOrdenTrabajo, objEntityDetalle.v_IdProductoDetalle, true, TipoAccion.Nuevo);
                            if (pobjOperationResult.Success == 0) return null;

                            dbContext.AddTonbs_formatounicofacturaciondetalle(objEntityDetalle);
                            Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "nbs_formatounicofacturaciondetalle", newIdFormatoUnicoDetalle);
                        }
                        #endregion

                        #region Actualiza Correlativo EmpresaDetalle
                        new DocumentoBL().ActualizarCorrelativoPorSerie(ref pobjOperationResult, Globals.ClientSession.i_IdEstablecimiento, 437, Globals.ClientSession.i_Periodo.ToString(), int.Parse(objEntityFuf.v_NroFormato) + 1);
                        if (pobjOperationResult.Success == 0) return string.Empty;
                        #endregion

                        dbContext.SaveChanges();
                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "nbs_formatounicofacturacion", newIdFormatoUnico);
                        pobjOperationResult.Success = 1;
                        ts.Complete();
                        return newIdFormatoUnico;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "FormatoUnicoFacturacionBL.InsertarFormatoUnicoFacturacion()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        /// <summary>
        /// Método para actualizar los formatos unicos de facturacion
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pobjFormatounicofacturacionDto"></param>
        /// <param name="ptemInsertarFormatounicofacturaciondetalleDtos"></param>
        /// <param name="ptemEditarFormatounicofacturaciondetalleDtos"></param>
        /// <param name="ptemEliminarFormatounicofacturaciondetalleDtos"></param>
        /// <param name="clientSession"></param>
        public static void ActualizarFormatoUnicoFacturacion(ref OperationResult pobjOperationResult,
            nbs_formatounicofacturacionDto pobjFormatounicofacturacionDto,
            List<nbs_formatounicofacturaciondetalleDto> ptemInsertarFormatounicofacturaciondetalleDtos,
            List<nbs_formatounicofacturaciondetalleDto> ptemEditarFormatounicofacturaciondetalleDtos,
            List<nbs_formatounicofacturaciondetalleDto> ptemEliminarFormatounicofacturaciondetalleDtos,
            List<string> clientSession)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var intNodeId = int.Parse(clientSession[0]);

                        #region Actualiza Cabecera
                        var entitySource = dbContext.nbs_formatounicofacturacion.FirstOrDefault(o =>
                                                    o.v_IdFormatoUnicoFacturacion == pobjFormatounicofacturacionDto.v_IdFormatoUnicoFacturacion);

                        var entityHeader = pobjFormatounicofacturacionDto.ToEntity();
                        entityHeader.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                        entityHeader.t_ActualizaFecha = DateTime.Now;
                        dbContext.nbs_formatounicofacturacion.ApplyCurrentValues(entityHeader);
                        #endregion

                        #region Actualiza Detalle

                        #region Inserción
                        foreach (var objEntityDetalle in ptemInsertarFormatounicofacturaciondetalleDtos.Select(objDetalle => objDetalle.ToEntity()))
                        {
                            var SecuentialId = new SecuentialBL().GetNextSecuentialId(intNodeId, 104);
                            var newIdFormatoUnicoDetalle = Utils.GetNewId(int.Parse(clientSession[0]), SecuentialId, "FE");
                            objEntityDetalle.v_IdFormatoUnicoFacturacionDetalle = newIdFormatoUnicoDetalle;
                            objEntityDetalle.v_IdFormatoUnicoFacturacion = entityHeader.v_IdFormatoUnicoFacturacion;
                            objEntityDetalle.t_InsertaFecha = DateTime.Now;
                            objEntityDetalle.i_InsertaIdUsuario = Int32.Parse(clientSession[2]);
                            objEntityDetalle.i_Eliminado = 0;

                            ActualizarDetalleOT(ref pobjOperationResult, objEntityDetalle.v_IdOrdenTrabajo, objEntityDetalle.v_IdProductoDetalle, true, TipoAccion.Nuevo);
                            if (pobjOperationResult.Success == 0) return;

                            dbContext.AddTonbs_formatounicofacturaciondetalle(objEntityDetalle);
                            Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "nbs_formatounicofacturaciondetalle", newIdFormatoUnicoDetalle);
                        }
                        #endregion

                        #region Edición
                        foreach (var objEntityDetalle in ptemEditarFormatounicofacturaciondetalleDtos.Select(objDetalle => objDetalle.ToEntity()))
                        {
                            var entitySourceDetalle = dbContext.nbs_formatounicofacturaciondetalle.FirstOrDefault(o =>
                                                    o.v_IdFormatoUnicoFacturacionDetalle == objEntityDetalle.v_IdFormatoUnicoFacturacionDetalle);

                            objEntityDetalle.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                            objEntityDetalle.t_ActualizaFecha = DateTime.Now;
                            dbContext.nbs_formatounicofacturaciondetalle.ApplyCurrentValues(objEntityDetalle);

                            ActualizarDetalleOT(ref pobjOperationResult, objEntityDetalle.v_IdOrdenTrabajo, objEntityDetalle.v_IdProductoDetalle, true, TipoAccion.Edicion);
                            if (pobjOperationResult.Success == 0) return;

                            Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "nbs_formatounicofacturaciondetalle", objEntityDetalle.v_IdFormatoUnicoFacturacionDetalle);
                        }
                        #endregion

                        #region Eliminación
                        foreach (var objEntityDetalle in ptemEliminarFormatounicofacturaciondetalleDtos.Select(objDetalle => objDetalle.ToEntity()))
                        {
                            var entitySourceDetalle = dbContext.nbs_formatounicofacturaciondetalle.FirstOrDefault(o =>
                                                    o.v_IdFormatoUnicoFacturacionDetalle == objEntityDetalle.v_IdFormatoUnicoFacturacionDetalle);

                            objEntityDetalle.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                            objEntityDetalle.t_ActualizaFecha = DateTime.Now;
                            objEntityDetalle.i_Eliminado = 1;

                            ActualizarDetalleOT(ref pobjOperationResult, objEntityDetalle.v_IdOrdenTrabajo, objEntityDetalle.v_IdProductoDetalle, false, TipoAccion.Eliminacion);
                            if (pobjOperationResult.Success == 0) return;

                            dbContext.nbs_formatounicofacturaciondetalle.ApplyCurrentValues(objEntityDetalle);
                            Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "nbs_formatounicofacturaciondetalle", objEntityDetalle.v_IdFormatoUnicoFacturacionDetalle);
                        }
                        #endregion

                        #endregion

                        dbContext.SaveChanges();
                        Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "nbs_formatounicofacturacion", entityHeader.v_IdFormatoUnicoFacturacion);
                        pobjOperationResult.Success = 1;
                        ts.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "FormatoUnicoFacturacionBL.ActualizarFormatoUnicoFacturacion()" + "\nLinea: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        /// <summary>
        /// Eliminar Formato Unico de facturacion y sus tablas relacionadas.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstrIdFormatoUnicoFacturacion"></param>
        /// <param name="clientSession"></param>
        public static void EliminarFormatoUnicoFacturacion(ref OperationResult pobjOperationResult, string pstrIdFormatoUnicoFacturacion, List<string> clientSession)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var objFormatoUnico = dbContext.nbs_formatounicofacturacion.FirstOrDefault(p => p.v_IdFormatoUnicoFacturacion == pstrIdFormatoUnicoFacturacion);
                        if (objFormatoUnico != null)
                        {
                            objFormatoUnico.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                            objFormatoUnico.t_ActualizaFecha = DateTime.Now;
                            objFormatoUnico.i_Eliminado = 1;
                            dbContext.nbs_formatounicofacturacion.ApplyCurrentValues(objFormatoUnico);

                            if (objFormatoUnico.nbs_formatounicofacturaciondetalle != null)
                            {
                                foreach (var nbsFormatounicofacturaciondetalle in objFormatoUnico.nbs_formatounicofacturaciondetalle.Where(p => p.i_Eliminado == 0))
                                {
                                    nbsFormatounicofacturaciondetalle.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                                    nbsFormatounicofacturaciondetalle.t_ActualizaFecha = DateTime.Now;
                                    nbsFormatounicofacturaciondetalle.i_Eliminado = 1;

                                    ActualizarDetalleOT(ref pobjOperationResult, nbsFormatounicofacturaciondetalle.v_IdOrdenTrabajo,
                                        nbsFormatounicofacturaciondetalle.v_IdProductoDetalle, false, TipoAccion.Eliminacion);
                                    if (pobjOperationResult.Success == 0) return;

                                    dbContext.nbs_formatounicofacturaciondetalle.ApplyCurrentValues(nbsFormatounicofacturaciondetalle);
                                }
                            }

                            dbContext.SaveChanges();
                            pobjOperationResult.Success = 1;
                            Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "nbs_formatounicofacturacion", objFormatoUnico.v_IdFormatoUnicoFacturacion);
                            ts.Complete();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "FormatoUnicoFacturacionBL.EliminarFormatoUnicoFacturacion()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        /// <summary>
        /// Obtiene la cabecera del documeto de formato unico de facturacion.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstrIdFormatoUnico"></param>
        /// <returns></returns>
        public static nbs_formatounicofacturacionDto ObtenerCabecera(ref OperationResult pobjOperationResult,
            string pstrIdFormatoUnico)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var result = (from n in dbContext.nbs_formatounicofacturacion
                                  join J1 in dbContext.cliente on n.v_IdCliente equals J1.v_IdCliente into J1_join
                                  from J1 in J1_join.DefaultIfEmpty()
                                  join J2 in dbContext.cliente on n.v_IdClienteFacturar equals J2.v_IdCliente into J2_join
                                  from J2 in J2_join.DefaultIfEmpty()
                                  where n.v_IdFormatoUnicoFacturacion == pstrIdFormatoUnico
                                  select new nbs_formatounicofacturacionDto
                                  {
                                      NombreCliente =
                                          (J1.v_ApePaterno + " " + J1.v_ApeMaterno + " " + J1.v_PrimerNombre +
                                          " " + J1.v_SegundoNombre + " " + J1.v_RazonSocial).Trim(),
                                      NombreClienteFacturacion =
                                     (J2.v_ApePaterno + " " + J2.v_ApeMaterno + " " + J2.v_PrimerNombre +
                                      " " + J2.v_SegundoNombre + " " + J2.v_RazonSocial).Trim(),
                                      NroDocumentoCliente = J1.v_NroDocIdentificacion,
                                      NroDocumentoClienteFacturacion = J2.v_NroDocIdentificacion,
                                      v_IdFormatoUnicoFacturacion = n.v_IdFormatoUnicoFacturacion,
                                      t_FechaRegistro = n.t_FechaRegistro,
                                      i_Eliminado = n.i_Eliminado,
                                      d_Total = n.d_Total,
                                      i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                      i_Facturado = n.i_Facturado,
                                      i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                      t_ActualizaFecha = n.t_ActualizaFecha,
                                      t_InsertaFecha = n.t_InsertaFecha,
                                      v_Correlativo = n.v_Correlativo,
                                      v_IdCliente = n.v_IdCliente,
                                      v_IdClienteFacturar = n.v_IdClienteFacturar,
                                      v_Mes = n.v_Mes,
                                      v_NroFormato = n.v_NroFormato,
                                      v_Periodo = n.v_Periodo,
                                      DireccionClienteFacturacion = J2.v_DirecPrincipal,
                                      i_IdTipoDocumentoCliente =J1.i_IdTipoIdentificacion ??-1,
                                      



                                  }).FirstOrDefault();

                    if (result != null)
                    {
                        if (result.i_InsertaIdUsuario != null)
                            result.UsuarioResponsable = DevolverUsuarioResponsable(result.i_InsertaIdUsuario.Value);

                        pobjOperationResult.Success = 1;
                        return result;
                    }

                    throw new ArgumentNullException("No existe data.");
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "FormatoUnicoFacturacionBL.ObtenerCabecera()" + "\nLinea: " +
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
        /// Obtiene el detalle del formato unico de facturacion
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstrIdFormatoUnico"></param>
        /// <returns></returns>
        public static BindingList<nbs_formatounicofacturaciondetalleDto> ObtenerDetalle(
            ref OperationResult pobjOperationResult, string pstrIdFormatoUnico)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var result = (from a in dbContext.nbs_formatounicofacturaciondetalle
                                  join b in dbContext.productodetalle on new { pd = a.v_IdProductoDetalle, eliminado = 0 } equals new { pd = b.v_IdProductoDetalle, eliminado = 0 } into b_join
                                  from b in b_join.DefaultIfEmpty()
                                  join c in dbContext.producto on new { p = b.v_IdProducto, eliminado = 0 } equals new { p = c.v_IdProducto, eliminado = c.i_Eliminado.Value } into c_join
                                  from c in c_join.DefaultIfEmpty()
                                  join d in dbContext.linea on new { l = c.v_IdLinea, eliminado = 0 } equals new { l = d.v_IdLinea, eliminado = d.i_Eliminado.Value } into d_join
                                  from d in d_join.DefaultIfEmpty()
                                  where a.v_IdFormatoUnicoFacturacion == pstrIdFormatoUnico && a.i_Eliminado == 0
                                  select new nbs_formatounicofacturaciondetalleDto
                                          {
                                              v_IdFormatoUnicoFacturacion = a.v_IdFormatoUnicoFacturacion,
                                              v_IdFormatoUnicoFacturacionDetalle = a.v_IdFormatoUnicoFacturacionDetalle,
                                              i_Eliminado = a.i_Eliminado,
                                              i_InsertaIdUsuario = a.i_InsertaIdUsuario,
                                              t_ActualizaFecha = a.t_ActualizaFecha,
                                              t_InsertaFecha = a.t_InsertaFecha,
                                              i_ActualizaIdUsuario = a.i_ActualizaIdUsuario,
                                              v_IdProductoDetalle = a.v_IdProductoDetalle,
                                              d_Total = a.d_Total,
                                              d_Importe = a.d_Importe,
                                              i_Cantidad = a.i_Cantidad,
                                              v_IdOrdenTrabajo = a.v_IdOrdenTrabajo,
                                              CodigoServicio = a.productodetalle.producto.v_CodInterno,
                                              DescripcionServicio = a.v_DescripcionTemporal, //a.productodetalle.producto.v_Descripcion,
                                              v_DescripcionTemporal = a.v_DescripcionTemporal,
                                              NroOrdenTrabajo = a.nbs_ordentrabajo.v_NroOrdenTrabajo,
                                              NroCuenta = d.v_NroCuentaVenta,
                                              i_EsServicio = c.i_EsServicio == null ? 0 : c.i_EsServicio.Value,
                                              i_IdUnidadMedida = c.i_IdUnidadMedida == null ? -1 : c.i_IdUnidadMedida.Value,
                                              i_FacturadoContabilidad = a.i_FacturadoContabilidad.Value,
                                              i_UsadoVenta = a.i_UsadoVenta,
                                              d_ImporteRegistral = a.d_ImporteRegistral ?? 0,
                                          }
                                ).ToList();

                    pobjOperationResult.Success = 1;
                    return new BindingList<nbs_formatounicofacturaciondetalleDto>(result);
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "FormatoUnicoFacturacionBL.ObtenerDetalle()" + "\nLinea: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        /// <summary>
        /// Obtiene las órdenes de trabajo pendientes de terminar de facturar.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstrIdCliente">Id del cliente para que se realice la búsqueda.</param>
        /// <returns></returns>
        /// 





        public static BindingList<nbs_formatounicofacturaciondetalleDto> ObtenerDetalleFUFparaVenta(
          ref OperationResult pobjOperationResult, string pstrIdFormatoUnico)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var result = (from a in dbContext.nbs_formatounicofacturaciondetalle
                                  join b in dbContext.productodetalle on new { pd = a.v_IdProductoDetalle, eliminado = 0 } equals new { pd = b.v_IdProductoDetalle, eliminado = 0 } into b_join
                                  from b in b_join.DefaultIfEmpty()
                                  join c in dbContext.producto on new { p = b.v_IdProducto, eliminado = 0 } equals new { p = c.v_IdProducto, eliminado = c.i_Eliminado.Value } into c_join
                                  from c in c_join.DefaultIfEmpty()
                                  join d in dbContext.linea on new { l = c.v_IdLinea, eliminado = 0 } equals new { l = d.v_IdLinea, eliminado = d.i_Eliminado.Value } into d_join
                                  from d in d_join.DefaultIfEmpty()
                                  where a.v_IdFormatoUnicoFacturacion == pstrIdFormatoUnico && a.i_Eliminado == 0
                                  select new nbs_formatounicofacturaciondetalleDto
                                  {
                                      v_IdFormatoUnicoFacturacion = a.v_IdFormatoUnicoFacturacion,
                                      v_IdFormatoUnicoFacturacionDetalle = a.v_IdFormatoUnicoFacturacionDetalle,
                                      i_Eliminado = a.i_Eliminado,
                                      i_InsertaIdUsuario = a.i_InsertaIdUsuario,
                                      t_ActualizaFecha = a.t_ActualizaFecha,
                                      t_InsertaFecha = a.t_InsertaFecha,
                                      i_ActualizaIdUsuario = a.i_ActualizaIdUsuario,
                                      v_IdProductoDetalle = a.v_IdProductoDetalle,
                                      d_Total = a.d_Total,
                                      d_Importe = a.d_Importe,
                                      i_Cantidad = a.i_Cantidad,
                                      v_IdOrdenTrabajo = a.v_IdOrdenTrabajo,
                                      CodigoServicio = a.productodetalle.producto.v_CodInterno,
                                      DescripcionServicio = a.v_DescripcionTemporal, //a.productodetalle.producto.v_Descripcion,
                                      v_DescripcionTemporal = a.v_DescripcionTemporal,
                                      NroOrdenTrabajo = a.nbs_ordentrabajo.v_NroOrdenTrabajo,
                                      NroCuenta = d.v_NroCuentaVenta,
                                      i_EsServicio = c.i_EsServicio == null ? 0 : c.i_EsServicio.Value,
                                      i_IdUnidadMedida = c.i_IdUnidadMedida == null ? -1 : c.i_IdUnidadMedida.Value,
                                      i_FacturadoContabilidad = a.i_FacturadoContabilidad.Value,
                                      i_UsadoVenta = a.i_UsadoVenta,
                                      d_ImporteRegistral = a.d_ImporteRegistral ?? 0,
                                  }
                                ).ToList();
                    var result1 = result.Where(l => l.d_Importe > 0).GroupBy(l => new { cod = l.CodigoServicio }).Select(p =>
                    {


                        var k = p.FirstOrDefault();
                        k.i_Cantidad = 1;
                        //k.d_Importe = p.Sum (l=>l.i_Cantidad) * p.Sum(l => l.d_Importe);
                        //k.d_ImporteRegistral = p.Sum(l=>l.i_Cantidad) * p.Sum(l => l.d_ImporteRegistral);
                        //k.d_Importe = p.Sum(l => l.d_Importe);
                        //k.d_ImporteRegistral = p.Sum(l => l.d_ImporteRegistral);
                        k.d_Importe = p.Sum(l => l.d_Total);
                        //k.d_ImporteRegistral = p.Sum(l => l.d_Total);
                        k.d_Total = p.Sum(l => l.d_Total);
                        return k;

                    }).ToList();


                    var result2 = result.Where(l => l.d_ImporteRegistral > 0).GroupBy(l => new { cod = l.CodigoServicio }).Select(p =>
                      {


                          var k = p.FirstOrDefault();
                          k.i_Cantidad = 1;
                          //k.d_Importe = p.Sum (l=>l.i_Cantidad )* p.Sum(l => l.d_Importe);
                          //k.d_ImporteRegistral =  p.Sum (l=>l.i_Cantidad) * p.Sum(l => l.d_ImporteRegistral);
                          // k.d_Importe = p.Sum(l => l.d_Total);
                          k.d_ImporteRegistral = p.Sum(l => l.d_Total);
                          k.d_Total = p.Sum(l => l.d_Total);
                          return k;

                      }).ToList();

                    pobjOperationResult.Success = 1;
                    return new BindingList<nbs_formatounicofacturaciondetalleDto>(result1.Concat(result2).ToList());
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "FormatoUnicoFacturacionBL.ObtenerDetalle()" + "\nLinea: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }


        public static List<nbs_ordentrabajoDto> BuscarOrdenesTrabajoPendientes(ref OperationResult pobjOperationResult, string pstrIdCliente)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var consulta = dbContext.nbs_ordentrabajo.Where(p => p.v_IdCliente == pstrIdCliente &&
                                                                         p.nbs_ordentrabajodetalle.Any(d => (d.i_UsadoEnFUF == null || d.i_UsadoEnFUF.Value == 0) &&
                                                                         d.i_Eliminado == 0) && p.i_Eliminado == 0 && p.i_IdEstado == 1).ToList()
                        .Select(p => new nbs_ordentrabajoDto
                        {
                            CodigoCliente = p.cliente.v_CodCliente,
                            d_Total = p.d_Total,
                            i_ActualizaIdUsuario = p.i_ActualizaIdUsuario,
                            RazonSocialCliente = (p.cliente.v_ApePaterno + " " + p.cliente.v_ApeMaterno + " " + p.cliente.v_PrimerNombre
                                                    + " " + p.cliente.v_RazonSocial).Trim(),
                            Responsable = DevolverUsuarioResponsable(p.i_InsertaIdUsuario.Value),
                            v_Mes = p.v_Mes,
                            v_Periodo = p.v_Periodo,
                            v_Correlativo = p.v_Correlativo,
                            v_Glosa = p.v_Glosa,
                            v_NroOrdenCompra = p.v_NroOrdenCompra,
                            v_NroOrdenTrabajo = p.v_NroOrdenTrabajo,
                            v_IdOrdenTrabajo = p.v_IdOrdenTrabajo
                        }
                        ).ToList();

                    pobjOperationResult.Success = 1;

                    return consulta;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation =
                    "FormatoUnicoFacturacionBL.BuscarOrdenesTrabajoPendientes()" + "\nLinea: " +
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
        /// Devuelve los detalles pendientes de facturacion de las OTs seleccionadas.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="plstrIdOT"></param>
        /// <returns></returns>
        public static List<nbs_formatounicofacturaciondetalleDto> DevolverDetallePendientesOT(
            ref OperationResult pobjOperationResult, List<string> plstrIdOT)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var result = new List<nbs_formatounicofacturaciondetalleDto>();

                    plstrIdOT.Distinct().ToList().ForEach(idOt =>
                    {
                        var nbsOrdentrabajo = dbContext.nbs_ordentrabajo.FirstOrDefault(p => p.v_IdOrdenTrabajo == idOt);
                        if (nbsOrdentrabajo != null)
                        {
                            var ordenTrabajoDetalle = nbsOrdentrabajo.nbs_ordentrabajodetalle.Where(
                                p => (p.i_UsadoEnFUF == null || p.i_UsadoEnFUF == 0) && p.i_Eliminado == 0).ToList();

                            if (ordenTrabajoDetalle.Any())
                            {
                                var detalleResult = ordenTrabajoDetalle.Select(p => new nbs_formatounicofacturaciondetalleDto
                                    {
                                        CodigoServicio = p.productodetalle.producto.v_CodInterno,
                                        d_Importe = p.d_Importe,
                                        d_Total = p.d_Total,
                                        DescripcionServicio = string.IsNullOrEmpty(p.v_DescripcionTemporal) ? p.productodetalle.producto.v_Descripcion : p.v_DescripcionTemporal,
                                        i_ActualizaIdUsuario = p.i_ActualizaIdUsuario,
                                        i_Cantidad = p.i_Cantidad,
                                        i_Eliminado = p.i_Eliminado,
                                        NroOrdenTrabajo = p.nbs_ordentrabajo.v_NroOrdenTrabajo,
                                        v_IdOrdenTrabajo = p.nbs_ordentrabajo.v_IdOrdenTrabajo,
                                        v_IdProductoDetalle = p.v_IdProductoDetalle,
                                        d_ImporteRegistral = p.d_ImporteRegistral,
                                        i_FacturadoContabilidad = p.d_ImporteRegistral != 0 && p.d_ImporteRegistral != null ? 1 : 0,

                                        v_DescripcionTemporal = string.IsNullOrEmpty(p.v_DescripcionTemporal) ? p.productodetalle.producto.v_Descripcion : p.v_DescripcionTemporal,
                                    }).ToList();

                                result = result.Concat(detalleResult).ToList();
                            }
                        }
                    });

                    pobjOperationResult.Success = 1;
                    return result;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "FormatoUnicoFacturacionBL.DevolverDetallePendientesOT()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        /// <summary>
        /// Busca y encuentra al detalle de la OT usada en el FUF para colocarle el flag de i_UsadoEnFUF en 1.
        /// En caso de que antes de empezar la Transacción otra operacion la halla usado anteriormente arroja una excepcion para evitar inconsistencias de data.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstrIdOT"></param>
        /// <param name="pstrIdProducto"></param>
        /// <param name="UsadoEnFUF"></param>
        /// <param name="_tipoAccion"></param>
        private static void ActualizarDetalleOT(ref OperationResult pobjOperationResult, string pstrIdOT, string pstrIdProducto, bool UsadoEnFUF, TipoAccion _tipoAccion)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var detalleOt = dbContext.nbs_ordentrabajodetalle.FirstOrDefault(p => p.v_IdOrdenTrabajo == pstrIdOT
                                                                              && p.v_IdProductoDetalle == pstrIdProducto &&
                                                                              p.i_Eliminado == 0);

                    if (detalleOt == null)
                        throw new ArgumentNullException("Detalle de OT Ya no existe!" + pstrIdOT + " - " +
                                                        pstrIdProducto);

                    if (detalleOt.i_UsadoEnFUF == 1 && _tipoAccion == TipoAccion.Nuevo)
                        throw new ArgumentNullException(
                            "Detalle de OT acaba de ser usada en otro Formato único de Facturación" + pstrIdOT + " - " +
                            pstrIdProducto);

                    detalleOt.i_UsadoEnFUF = UsadoEnFUF ? 1 : 0;
                    dbContext.nbs_ordentrabajodetalle.ApplyCurrentValues(detalleOt);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "FormatoUnicoFacturacionBL.ActualizarDetalleOT()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        //private static BindingList<VentaDetalles> BuscaFormatoUnico(ref  OperationResult pobjOperationResult, string IdFormato)
        //{
        //    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
        //    {

        //        var fuf = (from a in dbContext.nbs_formatounicofacturacion

        //                   join b in dbContext.nbs_formatounicofacturaciondetalle on new { Id = a.v_IdFormatoUnicoFacturacion, eliminado = 0 } equals new { Id = b.v_IdFormatoUnicoFacturacion, eliminado = b.i_Eliminado.Value } into b_join

        //                   from b in b_join.DefaultIfEmpty()

        //                   join c in dbContext.cliente on new { cliente = a.v_IdClienteFacturar , eliminado = 0 } equals new { cliente = c.v_IdCliente, eliminado = c.i_Eliminado.Value } into c_join

        //                   from c in c_join.DefaultIfEmpty()

        //                   select new VentaDetalles
        //                   {

        //                       IdCliente = c.v_IdCliente.Trim(),
        //                       NombreCliente = (c.v_ApePaterno + " " + c.v_ApeMaterno + " " + c.v_PrimerNombre + " " + c.v_SegundoNombre + " " + c.v_RazonSocial).Trim(),
        //                       CodigoCliente = c.v_CodCliente.Trim(),
        //                       Direccion = c.v_DirecPrincipal.Trim(),



        //                   }).ToList();


        //    }

        //}

        private enum TipoAccion
        {
            Nuevo,
            Edicion,
            Eliminacion
        }

        public static nbs_formatounicofacturacionDto ObtenerFUFxNroFormato(ref OperationResult pobOperationResult, string NroFormato)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    string Periodo = Globals.ClientSession.i_Periodo.Value.ToString();
                    var fu = (from a in dbContext.nbs_formatounicofacturacion

                              where a.i_Eliminado == 0 && a.v_NroFormato == NroFormato && a.v_Periodo == Periodo

                              select a).FirstOrDefault();

                    nbs_formatounicofacturacionDto objFUF = nbs_formatounicofacturacionAssembler.ToDTO(fu);
                    pobOperationResult.Success = 1;
                    return objFUF;

                }
            }
            catch (Exception ex)
            {

                pobOperationResult.Success = 0;
                return null;
            }



        }
    }

    public static class ObtenerConsultaReporteFlujoBl
    {
        public static List<ConsultaFlujoProcesosNBS> ObtenerConsultaFlujoProcesos(ref OperationResult pobjOperationResult, string pstrIdCliente, DateTime FechaInicio, DateTime FechaFin)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    List<cobranzapendiente> ListaCobranzaPendiente = dbContext.cobranzapendiente.ToList();
                    List<venta> Ventitas = dbContext.venta.ToList().Where(l => l.i_Eliminado == 0 && l.v_Periodo == Globals.ClientSession.i_Periodo.ToString()).ToList();
                    List<documento> Documentitos = dbContext.documento.Where(l => l.i_Eliminado == 0).ToList();
                    List<cliente> Clientes = dbContext.cliente.Where(l => l.i_Eliminado == 0).ToList();
                    List<nbs_formatounicofacturaciondetalle> FFD = dbContext.nbs_formatounicofacturaciondetalle.Where(o => o.i_Eliminado == 0).ToList();
                    List<nbs_ventakardex> ventaKardex = dbContext.nbs_ventakardex.Where(o => o.i_Eliminado == 0).ToList();
                    var result = (from n in dbContext.nbs_ordentrabajo
                                  join a in dbContext.cliente on new { c = n.v_IdCliente, eliminado = 0 } equals new { c = a.v_IdCliente, eliminado = a.i_Eliminado.Value } into a_join
                                  from a in a_join.DefaultIfEmpty()
                                  join b in dbContext.cliente on new { responsable = n.v_IdResponsable, eliminado = 0 } equals new { responsable = b.v_IdCliente, eliminado = b.i_Eliminado.Value } into b_join
                                  from b in b_join.DefaultIfEmpty()
                                  where (n.v_IdCliente == pstrIdCliente || pstrIdCliente == null) && n.i_Eliminado == 0
                                  && (n.t_FechaRegistro >= FechaInicio && n.t_FechaRegistro <= FechaFin)
                                  select new
                                  {
                                      NroOrdenTrabajo = n.v_NroOrdenTrabajo,
                                      FechaOrdenTrabajo = n.t_FechaRegistro ?? null,
                                      IdOrdenTrabajo = n.v_IdOrdenTrabajo,
                                      NombreCliente = a == null ? "** Cliente no existe **" : n.i_IdEstado == 0 ? "ANULADO" : (a.v_ApePaterno + " " + a.v_ApeMaterno + " " + a.v_PrimerNombre + " " + a.v_RazonSocial).Trim(),
                                      Responsable = b.v_PrimerNombre,
                                  }
                                  ).ToList().Select(p =>
                                  {
                                      var fuf = ObtenerFormatoUnicoFacturacion(p.IdOrdenTrabajo, FFD);
                                      var ventasPorFuf = ObtenerVentasPorFormatoUnicoFacturacion(fuf != null ? fuf.v_IdFormatoUnicoFacturacion : null, Ventitas, Documentitos);
                                      var nroKardex = ObtenerKardexPorVenta(ventasPorFuf != null ? ventasPorFuf.Select(x => x.v_IdVenta).ToList() : null, ventaKardex);
                                      var montoCobrado = ObtenerMontoCobrado(ventasPorFuf != null ? ventasPorFuf.Select(x => x.v_IdVenta).ToList() : null, ListaCobranzaPendiente);
                                      return new ConsultaFlujoProcesosNBS
                                      {
                                          NroOrdenTrabajo = p.NroOrdenTrabajo,
                                          FechaOrdenTrabajo = p.FechaOrdenTrabajo,
                                          NroFormatoUnicoFacturacion = fuf != null ? fuf.v_NroFormato : string.Empty,
                                          FechaFormatoUnicoFacturacion = fuf != null ? fuf.t_FechaRegistro ?? null : null,
                                          FormatoFacturado = fuf != null ? fuf.i_Facturado != null && fuf.i_Facturado.Value != 0 ? "SI" : "NO" : string.Empty,
                                          SerieCorrelativoVenta = ventasPorFuf != null ? string.Join(" / ", ventasPorFuf.Select(x => x.TipoDocumento + " " + x.v_SerieDocumento.Trim() + "-" + x.v_CorrelativoDocumento)) : string.Empty,
                                          FechaVenta = ventasPorFuf != null ? ventasPorFuf.FirstOrDefault().t_FechaRegistro ?? null : null,
                                          NroKardexs = nroKardex,
                                          MontoCobrado = montoCobrado,
                                          MontoFormatoUnicoFacturacion = fuf != null ? fuf.d_Total : null,
                                          NombreCliente = p.NombreCliente,
                                          Responsable = p.Responsable,

                                      };
                                  }
                                  ).ToList();

                    pobjOperationResult.Success = 1;
                    return result;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ObtenerConsultaReporteFlujo.ObtenerConsultaFlujoProcesos()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }


        public static List<KardexDesdeFacturacion> ObtenerKardexDesdeFacturacion(ref OperationResult pobjOperationResult, string pstrIdCliente, DateTime FechaInicio, DateTime FechaFin, string TipoKardex, string NroKardex)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    List<nbs_ventakardex> ListaVentaKardex = new List<nbs_ventakardex>();
                    ListaVentaKardex = dbContext.nbs_ventakardex.ToList();
                    List<cobranzapendiente> ListaCobranzaPendiente = new List<cobranzapendiente>();
                    ListaCobranzaPendiente = dbContext.cobranzapendiente.ToList();
                    var result = (from a in dbContext.venta
                                  join b in dbContext.documento on new { doc = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { doc = b.i_CodigoDocumento, eliminado = b.i_Eliminado.Value } into b_join
                                  from b in b_join.DefaultIfEmpty()
                                  join c in dbContext.cliente on new { c = a.v_IdCliente } equals new { c = c.v_IdCliente } into c_join
                                  from c in c_join.DefaultIfEmpty()
                                  where (a.v_IdCliente == pstrIdCliente || pstrIdCliente == null) &&
                                  a.i_Eliminado == 0 && a.i_IdEstado == 1
                                  && a.t_FechaRegistro >= FechaInicio && a.t_FechaRegistro <= FechaFin
                                  select new
                                  {
                                      v_IdTipoKardex = a.v_IdTipoKardex,
                                      v_IdVenta = a.v_IdVenta,
                                      SerieCorrelativoVenta = b.v_Siglas + " " + a.v_SerieDocumento + " " + a.v_CorrelativoDocumento,
                                      FechaVenta = a.t_FechaRegistro.Value,
                                      Cliente = c == null ? "** CLIENTE NO EXISTE **" : string.IsNullOrEmpty(a.v_NombreClienteTemporal) ? (c.v_ApePaterno + " " + c.v_ApeMaterno + " " + c.v_PrimerNombre + " " + c.v_RazonSocial).Trim() : a.v_NombreClienteTemporal,

                                  }
                                  ).ToList().Select(p =>
                                  {


                                      List<string> ListaIdVenta = new List<string>();
                                      ListaIdVenta.Add(p.v_IdVenta);
                                      var nroKardex = ObtenerKardexPorVentaDesdeFacturacion(ListaIdVenta, TipoKardex.Trim(), NroKardex.Trim(), ListaVentaKardex);
                                      var montoCobrado = ObtenerMontoCobrado(ListaIdVenta, ListaCobranzaPendiente);
                                      return new KardexDesdeFacturacion
                                      {
                                          v_IdTipoKardex = p.v_IdTipoKardex,
                                          FechaVenta = p.FechaVenta,
                                          NroKardexs = nroKardex != null ? p.v_IdTipoKardex == "V" ? nroKardex : nroKardex.Substring(2) : nroKardex,
                                          MontoCobrado = montoCobrado ?? 0,
                                          SerieCorrelativoVenta = p.SerieCorrelativoVenta,
                                          Cliente = p.Cliente,


                                      };
                                  }
                                  ).ToList();

                    result = NroKardex != string.Empty || TipoKardex != String.Empty ? result.Where(x => x.NroKardexs != null).ToList() : result;

                    pobjOperationResult.Success = 1;
                    return result;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ObtenerConsultaReporteFlujo.ObtenerKardexDesdeFacturacion()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }






        private static nbs_formatounicofacturacion ObtenerFormatoUnicoFacturacion(string pstrIdOT, List<nbs_formatounicofacturaciondetalle> nbs_formatounicofacturaciondetalle)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var result = nbs_formatounicofacturaciondetalle.FirstOrDefault(p => p.v_IdOrdenTrabajo == pstrIdOT && p.i_Eliminado == 0);
                    if (result != null)
                        return result.nbs_formatounicofacturacion;

                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        private static List<ventaDto> ObtenerVentasPorFormatoUnicoFacturacion(string pstrIdFormatoUnicoFacturacion, List<venta> Ventitas, List<documento> Doc)
        {
            try
            {
                using (var dbContex = new SAMBHSEntitiesModelWin())
                {
                    if (pstrIdFormatoUnicoFacturacion != null)
                    {
                        var result = Ventitas.Where(p => p.v_IdFormatoUnicoFacturacion == pstrIdFormatoUnicoFacturacion).ToList();
                        if (result.Any())
                        {
                            var ventDtos = result.ToDTOs();

                            ventDtos.ForEach(vent =>
                            {
                                vent.TipoDocumento = Doc.FirstOrDefault(p => p.i_CodigoDocumento == vent.i_IdTipoDocumento).v_Siglas;
                            });

                            return ventDtos;
                        }
                    }

                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        private static string ObtenerKardexPorVenta(List<string> pstrIdVenta, List<nbs_ventakardex> nbs_ventakardex)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var consulta = nbs_ventakardex.Where(p => pstrIdVenta.Contains(p.v_IdVenta) && p.i_Eliminado == 0).ToList();

                    if (consulta.Any())
                        return String.Join(" / ", consulta.Select(p => p.v_TipoKardex.Trim() + "-" + p.v_NroKardex.Trim()));

                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        private static string ObtenerKardexPorVentaDesdeFacturacion(List<string> pstrIdVenta, string TipoKardex, string NroKardex, List<nbs_ventakardex> ListaVentaKardex)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    //var consulta = dbContext.nbs_ventakardex.Where(p => pstrIdVenta.Contains(p.v_IdVenta) && p.i_Eliminado == 0 && (p.v_TipoKardex == TipoKardex || TipoKardex == String.Empty) && (p.v_NroKardex == NroKardex || NroKardex == String.Empty)).ToList();
                    var consulta = ListaVentaKardex.Where(p => pstrIdVenta.Contains(p.v_IdVenta) && p.i_Eliminado == 0 && (p.v_TipoKardex == TipoKardex || TipoKardex == String.Empty) && (p.v_NroKardex == NroKardex || NroKardex == String.Empty)).ToList();
                    if (consulta.Any())
                        return String.Join(" / ", consulta.Select(p => p.v_TipoKardex.Trim() + "-" + p.v_NroKardex.Trim()));

                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        private static decimal? ObtenerMontoCobrado(List<string> pstrIdVenta, List<cobranzapendiente> ListaCobranzaPendiente)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    //var consulta = dbContext.cobranzapendiente.Where(p => pstrIdVenta.Contains(p.v_IdVenta) && p.i_Eliminado == 0).ToList();
                    var consulta = ListaCobranzaPendiente.Where(p => pstrIdVenta.Contains(p.v_IdVenta) && p.i_Eliminado == 0).ToList();
                    if (consulta.Any())
                        return consulta.Sum(x => x.d_Acuenta.Value);

                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
