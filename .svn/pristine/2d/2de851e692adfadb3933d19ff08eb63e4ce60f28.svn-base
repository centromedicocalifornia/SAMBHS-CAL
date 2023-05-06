using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Dynamic;
using SAMBHS.Common.BE.Custom;
using SAMBHS.CommonWIN.BL;

namespace SAMBHS.Tesoreria.BL
{
    public class DocumentoRetencionBL
    {
        private static string Periodo
        {
            get { return Globals.ClientSession.i_Periodo.ToString(); }
        }

        #region Methods

        public List<DocumentoRetencionPendiente> ObtenerRetencionesPendientesTesoreria(
            ref OperationResult pobjOperationResult, List<DocumentoRetencionPendiente> listaParametros)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var listaAnexos = listaParametros.Select(p => p.IdAnexo);
                    var listaTipoDocs = listaParametros.Select(p => p.i_IdTipoDocumento);
                    var listaSeries = listaParametros.Select(p => p.v_SerieDocumento);
                    var listaCorrelativos = listaParametros.Select(p => p.v_CorrelativoDocumento);

                    var consulta = (from n in dbContext.documentoretenciondetalle
                                    join J1 in dbContext.documentoretencion on n.v_IdDocumentoRetencion equals
                                        J1.v_IdDocumentoRetencion into J1_join
                                    from J1 in J1_join.DefaultIfEmpty()
                                    join J2 in dbContext.cliente on J1.v_IdCliente equals J2.v_IdCliente into J2_join
                                    from J2 in J2_join.DefaultIfEmpty()
                                    join J3 in dbContext.documento on n.i_IdTipoDocumento equals J3.i_CodigoDocumento into J3_join
                                    from J3 in J3_join.DefaultIfEmpty()

                                    where
                                        listaAnexos.Contains(J1.v_IdCliente) && listaTipoDocs.Contains(n.i_IdTipoDocumento.Value) &&
                                        listaSeries.Contains(n.v_SerieDocumento) &&
                                        listaCorrelativos.Contains(n.v_CorrelativoDocumento)
                                        && n.i_Eliminado == 0 && n.i_Procesado == 0
                                    select new DocumentoRetencionPendiente
                                    {
                                        d_MontoRetenido = n.d_MontoRetenido ?? 0,
                                        i_IdTipoDocumento = n.i_IdTipoDocumento ?? -1,
                                        IdAnexo = J1.v_IdCliente,
                                        RazonSocial =
                                            (J2.v_PrimerNombre.Trim() + " " + J2.v_ApePaterno.Trim() + " " + J2.v_ApeMaterno.Trim() +
                                             " " + J2.v_RazonSocial.Trim()).Trim(),
                                        RucAnexo = J2.v_NroDocIdentificacion,
                                        TipoDocumento = J3.v_Siglas,
                                        v_CorrelativoDocumento = n.v_CorrelativoDocumento,
                                        v_IdDocumentoRetencionDetalle = n.v_IdDocumentoRetencionDetalle,
                                        v_SerieDocumento = n.v_SerieDocumento,
                                        Importe = n.d_MontoPago ?? 0,
                                        Cambio = n.d_Cambio ?? 0,
                                        CodAnexo = J2.v_CodCliente,
                                        IdMoneda = J1.i_IdMoneda ?? 1,
                                        v_IdDocumentoRetencion = J1.v_IdDocumentoRetencion
                                    }).ToList();

                    return consulta;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.ExceptionMessage = ex.Message;
                pobjOperationResult.AdditionalInformation = "DocumentoRetencionBL.ObtenerRetencionesPendientesTesoreria";
                return null;
            }
        }

        public List<KeyValueDTO> ObtenerListadoDocumentos(ref OperationResult pobjOperationResult, string pstringPeriodo, string pstringMes)
        {
            try
            {
                using (var dbcontext = new SAMBHSEntitiesModelWin())
                {
                    var query = from n in dbcontext.documentoretencion
                                where n.v_Periodo == pstringPeriodo && n.v_Mes == pstringMes

                                orderby n.v_Correlativo ascending
                                select new
                                {
                                    n.v_Correlativo,
                                    n.v_IdDocumentoRetencion
                                };
                    var query2 = query.AsEnumerable()
                                .Select(x => new KeyValueDTO
                                {
                                    Value1 = x.v_Correlativo,
                                    Value2 = x.v_IdDocumentoRetencion
                                }).ToList();
                    pobjOperationResult.Success = 1;
                    return query2;
                }
            }
            catch (Exception er)
            {
                pobjOperationResult.ExceptionMessage = er.Message;
                pobjOperationResult.AdditionalInformation = "DocumentoRetencionBL.ObtenerListadoDocumentos";
                return null;
            }
        }

        public void GeneraDocumentoRetencionDesdeTesoreria(ref OperationResult pobjOperationResult, string pstrIdTesoreria, List<string> ClientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var objTesoreria = (dbContext.tesoreria.Where(t => t.v_IdTesoreria == pstrIdTesoreria)).FirstOrDefault();

                    var tesoreriaDetalles = (dbContext.tesoreriadetalle.Where(
                        r => r.v_IdTesoreria == pstrIdTesoreria && r.i_Eliminado == 0)).ToList();

                    if (tesoreriaDetalles.Any())
                    {
                        #region Inserta Cabecera

                        string newIdRetencion = string.Empty;
                        string newIdRetencionDetalle = string.Empty;
                        var objSecuentialBl = new SecuentialBL();

                        documentoretencion _documentoretencion = new documentoretencion();
                        List<documentoretenciondetalle> ListaXInsertar = new List<documentoretenciondetalle>();

                        _documentoretencion.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        var listadoDocumentos = ObtenerListadoDocumentos(ref pobjOperationResult, objTesoreria.t_FechaRegistro.Value.Year.ToString(), objTesoreria.t_FechaRegistro.Value.Month.ToString("00"));

                        var maxMovimiento = listadoDocumentos.Count > 0 ? int.Parse(listadoDocumentos[listadoDocumentos.Count() - 1].Value1) : 0;
                        maxMovimiento++;

                        var intNodeId = int.Parse(ClientSession[0]);
                        var secuentialId = objSecuentialBl.GetNextSecuentialId(intNodeId, 63);
                        newIdRetencion = Utils.GetNewId(int.Parse(ClientSession[0]), secuentialId, "XX");
                        _documentoretencion.v_IdDocumentoRetencion = newIdRetencion;
                        _documentoretencion.i_Estado = 1;
                        _documentoretencion.t_FechaRegistro = DateTime.Today;
                        _documentoretencion.v_IdTesoreria = pstrIdTesoreria;
                        _documentoretencion.v_IdCliente = tesoreriaDetalles.First().v_IdCliente;
                        _documentoretencion.v_Periodo = objTesoreria.t_FechaRegistro.Value.Year.ToString();
                        _documentoretencion.v_Mes = objTesoreria.t_FechaRegistro.Value.Month.ToString("00");
                        _documentoretencion.v_Correlativo = maxMovimiento.ToString("00000000");
                        _documentoretencion.t_InsertaFecha = DateTime.Now.Date;

                        #endregion

                        #region Inserta Detalles
                        foreach (tesoreriadetalle Retencion in tesoreriaDetalles.Where(p => p.v_NroCuenta.Substring(0, 2) == "42"))
                        {
                            var filaRetencion = tesoreriaDetalles.FirstOrDefault(p => p.v_NroCuenta == Globals.ClientSession.v_NroCuentaRetencion && p.v_NroDocumento == Retencion.v_NroDocumento && p.i_IdTipoDocumento == Retencion.i_IdTipoDocumento && p.v_IdCliente == Retencion.v_IdCliente);
                            if (filaRetencion != null)
                            {
                                documentoretenciondetalle objdocumentoretenciondetalle = new documentoretenciondetalle
                                {
                                    i_InsertaIdUsuario = Int32.Parse(ClientSession[2])
                                };
                                intNodeId = int.Parse(ClientSession[0]);
                                secuentialId = objSecuentialBl.GetNextSecuentialId(intNodeId, 64);
                                newIdRetencionDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), secuentialId, "XY");
                                objdocumentoretenciondetalle.v_IdDocumentoRetencion = newIdRetencion;
                                objdocumentoretenciondetalle.v_IdDocumentoRetencionDetalle = newIdRetencionDetalle;
                                var nroDocumento = Retencion.v_NroDocumento.Split('-');
                                objdocumentoretenciondetalle.v_SerieDocumento = nroDocumento[0];
                                objdocumentoretenciondetalle.v_CorrelativoDocumento = nroDocumento[1];
                                objdocumentoretenciondetalle.d_MontoPago = objTesoreria.i_IdMoneda != 1 ? (Retencion.d_Cambio ?? 0) : (Retencion.d_Importe ?? 0);
                                objdocumentoretenciondetalle.d_MontoRetenido = objTesoreria.i_IdMoneda != 1 ? (filaRetencion.d_Cambio ?? 0) : (filaRetencion.d_Importe ?? 0);
                                objdocumentoretenciondetalle.i_IdTipoDocumento = Retencion.i_IdTipoDocumento;
                                ListaXInsertar.Add(objdocumentoretenciondetalle);
                                Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "documentoretenciondetalle", newIdRetencionDetalle);
                            }
                        }

                        #endregion

                        ListaXInsertar.ForEach(p => dbContext.AddTodocumentoretenciondetalle(p));
                        _documentoretencion.d_TotalRetenido = ListaXInsertar.Sum(p => p.d_MontoRetenido ?? 0);
                        dbContext.AddTodocumentoretencion(_documentoretencion);

                        dbContext.SaveChanges();
                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "documentoretencion", newIdRetencion);
                    }
                    else
                    {
                        pobjOperationResult.Success = 0;
                        pobjOperationResult.ErrorMessage = "No se encontró la tesorería";
                        pobjOperationResult.AdditionalInformation = "DocumentoRetencionBL.GeneraDocumentoRetencion()";
                        Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                    }
                }
                pobjOperationResult.Success = 1;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "DocumentoRetencionBL.GeneraDocumentoRetencion()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException.Message;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public string InsertarDocumentoRetencion(ref OperationResult pobjOperationResult, documentoretencionDto pHeader, List<documentoretenciondetalleDto> pListInsert, List<string> ClientSession)
        {
            try
            {
                var intNodeId = int.Parse(ClientSession[0]);
                var objSecuentialBl = new SecuentialBL();
                var secuentialId = objSecuentialBl.GetNextSecuentialId(intNodeId, 63);
                var newIdRetencion = Utils.GetNewId(int.Parse(ClientSession[0]), secuentialId, "XX");

                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region Inserta Cabecera

                    var documentoretencion = pHeader.ToEntity();
                    documentoretencion.t_InsertaFecha = DateTime.Now.Date;
                    documentoretencion.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    documentoretencion.v_IdDocumentoRetencion = newIdRetencion;
                    documentoretencion.i_Eliminado = 0;
                    dbContext.documentoretencion.AddObject(documentoretencion);
                    #endregion

                    #region Inserta Detalles
                    foreach (var retencion in pListInsert)
                    {
                        var objdocumentoretenciondetalle = retencion.ToEntity();
                        intNodeId = int.Parse(ClientSession[0]);
                        secuentialId = objSecuentialBl.GetNextSecuentialId(intNodeId, 64);
                        var newIdRetencionDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), secuentialId, "XY");
                        objdocumentoretenciondetalle.v_IdDocumentoRetencion = newIdRetencion;
                        objdocumentoretenciondetalle.v_IdDocumentoRetencionDetalle = newIdRetencionDetalle;
                        objdocumentoretenciondetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objdocumentoretenciondetalle.i_Eliminado = 0;
                        objdocumentoretenciondetalle.i_Procesado = 0;
                        dbContext.documentoretenciondetalle.AddObject(objdocumentoretenciondetalle);
                    }
                    #endregion

                    dbContext.SaveChanges();

                    Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "documentoretencion", newIdRetencion);

                }
                pobjOperationResult.Success = 1;
                return newIdRetencion;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "DocumentoRetencionBL.GeneraDocumentoRetencion()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException.Message;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public void EditarDocumentoRetencion(ref OperationResult pobjOperationResult, documentoretencionDto pHeader, List<documentoretenciondetalleDto> pListInsert, List<documentoretenciondetalleDto> pListEdit, List<documentoretenciondetalleDto> pListDelete, List<string> ClientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region Inserta Cabecera
                    var intNodeId = int.Parse(ClientSession[0]);
                    var objSecuentialBl = new SecuentialBL();
                    var originalEntity = dbContext.documentoretencion.FirstOrDefault(p => p.v_IdDocumentoRetencion.Equals(pHeader.v_IdDocumentoRetencion));

                    originalEntity = pHeader.ToEntity();
                    dbContext.documentoretencion.ApplyCurrentValues(originalEntity);
                    #endregion

                    #region Inserta Detalles
                    foreach (var retencion in pListInsert)
                    {
                        var objdocumentoretenciondetalle = retencion.ToEntity();
                        intNodeId = int.Parse(ClientSession[0]);
                        var secuentialId = objSecuentialBl.GetNextSecuentialId(intNodeId, 64);
                        var newIdRetencionDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), secuentialId, "XY");
                        objdocumentoretenciondetalle.v_IdDocumentoRetencion = originalEntity.v_IdDocumentoRetencion;
                        objdocumentoretenciondetalle.v_IdDocumentoRetencionDetalle = newIdRetencionDetalle;
                        objdocumentoretenciondetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objdocumentoretenciondetalle.i_Eliminado = 0;
                        dbContext.documentoretenciondetalle.AddObject(objdocumentoretenciondetalle);
                    }
                    #endregion

                    #region Editar Detalles
                    foreach (var retencion in pListEdit)
                    {
                        var original =
                            dbContext.documentoretenciondetalle.FirstOrDefault(
                                p => p.v_IdDocumentoRetencionDetalle.Equals(retencion.v_IdDocumentoRetencionDetalle));

                        if (original != null)
                        {
                            original = retencion.ToEntity();
                            original.i_ActualizaUsuario = Int32.Parse(ClientSession[2]);
                            original.t_ActualizaFecha = DateTime.Now;
                            dbContext.documentoretenciondetalle.ApplyCurrentValues(original);
                        }
                    }
                    #endregion

                    #region Eliminar Detalles
                    foreach (var retencion in pListDelete)
                    {
                        var original =
                             dbContext.documentoretenciondetalle.FirstOrDefault(
                                 p => p.v_IdDocumentoRetencionDetalle.Equals(retencion.v_IdDocumentoRetencionDetalle));

                        if (original != null)
                        {
                            original = retencion.ToEntity();
                            original.i_ActualizaUsuario = Int32.Parse(ClientSession[2]);
                            original.t_ActualizaFecha = DateTime.Now;
                            original.i_Eliminado = 1;
                            dbContext.documentoretenciondetalle.ApplyCurrentValues(original);
                        }
                    }
                    #endregion

                    dbContext.SaveChanges();
                    Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "documentoretencion", originalEntity.v_IdDocumentoRetencion);
                }
                pobjOperationResult.Success = 1;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "DocumentoRetencionBL.EditarDocumentoRetencion()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                if (ex.InnerException != null) pobjOperationResult.ExceptionMessage = ex.InnerException.Message;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public void EliminarDocumentoRetencion(ref OperationResult pobjOperationResult, string pstrIdRetencion, List<string> clientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var header = dbContext.documentoretencion.FirstOrDefault(p => p.v_IdDocumentoRetencion.Equals(pstrIdRetencion));
                    if (header == null) throw new Exception("No existe el documento en la base de datos.");
                    header.i_ActualizaUsuario = int.Parse(clientSession[2]);
                    header.i_Eliminado = 1;
                    dbContext.documentoretencion.ApplyCurrentValues(header);

                    var details = dbContext.documentoretenciondetalle.
                        Where(p => p.i_Eliminado == 0 && p.v_IdDocumentoRetencion.
                        Equals(header.v_IdDocumentoRetencion))
                        .ToList();

                    if (details.Any(p => p.i_Procesado == 1))
                        throw new Exception(
                            "No se puede eliminar el registro porque uno de sus elementos ya fue usado en tesorería.");

                    details.ForEach(d =>
                    {
                        d.i_ActualizaUsuario = int.Parse(clientSession[2]);
                        d.i_Eliminado = 1;
                        dbContext.documentoretenciondetalle.ApplyCurrentValues(d);
                    });

                    EliminarTesoreria(ref pobjOperationResult, pstrIdRetencion);

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "DocumentoRetencionBL.EliminarDocumentoRetencion()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                if (ex.InnerException != null) pobjOperationResult.ExceptionMessage = ex.InnerException.Message;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public documentoretencionDto ObtenerCabecera(ref OperationResult pobjOperationResult, string pstrIdDocumento)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var result = (from d in dbContext.documentoretencion
                                  join J1 in dbContext.cliente on d.v_IdCliente equals J1.v_IdCliente into J1_join
                                  from J1 in J1_join.DefaultIfEmpty()
                                  where d.v_IdDocumentoRetencion == pstrIdDocumento
                                  select new documentoretencionDto
                                  {
                                      v_Periodo = d.v_Periodo,
                                      v_Correlativo = d.v_Correlativo,
                                      v_CorrelativoDocumento = d.v_CorrelativoDocumento,
                                      v_IdCliente = d.v_IdCliente,
                                      v_IdDocumentoRetencion = d.v_IdDocumentoRetencion,
                                      v_Mes = d.v_Mes,
                                      v_IdTesoreria = d.v_IdTesoreria,
                                      v_SerieDocumento = d.v_SerieDocumento,
                                      i_Estado = d.i_Estado,
                                      i_InsertaIdUsuario = d.i_InsertaIdUsuario,
                                      t_InsertaFecha = d.t_InsertaFecha,
                                      t_FechaRegistro = d.t_FechaRegistro,
                                      NombreProveedor = (J1.v_PrimerNombre.Trim() + " " + J1.v_ApePaterno.Trim() + " " + J1.v_ApeMaterno.Trim() + " " + J1.v_RazonSocial.Trim()).Trim(),
                                      RucProveedor = J1.v_NroDocIdentificacion,
                                      i_ActualizaUsuario = d.i_ActualizaUsuario,
                                      t_ActualizaFecha = d.t_ActualizaFecha,
                                      i_Eliminado = d.i_Eliminado,
                                      i_IdMoneda = d.i_IdMoneda,
                                      d_TipoCambio = d.d_TipoCambio ?? 0
                                  }
                                ).FirstOrDefault();

                    pobjOperationResult.Success = 1;
                    return result;
                }
            }
            catch (Exception er)
            {
                pobjOperationResult.ExceptionMessage = er.Message;
                pobjOperationResult.AdditionalInformation = "DocumentoRetencionBL.ObtenerCabecera";
                return null;
            }
        }

        public static List<ConsultaComprasRetencion> ObtenerDocumentosParaRetencionPorRuc(ref OperationResult pobjOperationResult,
            string idProveedor)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var result = (from n in dbContext.compra
                                  join J1 in dbContext.documento on n.i_IdTipoDocumento equals J1.i_CodigoDocumento into J1_join
                                  from J1 in J1_join.DefaultIfEmpty()
                                  where n.v_IdProveedor.Equals(idProveedor) && n.i_Eliminado == 0 &&
                                  !dbContext.documentoretenciondetalle.Any(p => p.v_IdCompra.Equals(n.v_IdCompra) && p.i_Eliminado == 0)
                                  select new ConsultaComprasRetencion
                                  {
                                      d_MontoPago = n.i_IdMoneda == 1 ? (n.d_Total ?? 0) : (n.d_Total ?? 0) * n.d_TipoCambio.Value,
                                      i_IdTipoDocumento = n.i_IdTipoDocumento ?? -1,
                                      TipoDocumento = J1.v_Siglas,
                                      v_CorrelativoDocumento = n.v_CorrelativoDocumento,
                                      v_SerieDocumento = n.v_SerieDocumento,
                                      v_IdCompra = n.v_IdCompra
                                  }).ToList();
                    pobjOperationResult.Success = 1;
                    return result.Where(p => p.d_MontoPago > 700).ToList();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.ExceptionMessage = ex.Message;
                pobjOperationResult.AdditionalInformation = "DocumentoRetencionBL.ObtenerCabecera";
                return null;
            }
        }

        public List<ReporteRetencion> ReporteRetencion(ref OperationResult pobjOperationResult, string pstrIdDocumento)
        {
            try
            {



                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    var Compras = (from a in dbContext.compra
                                   where a.i_Eliminado == 0
                                   select a).ToList();

                    var result = (from a in dbContext.documentoretencion
                                  join b in dbContext.cliente on new { a.v_IdCliente } equals new { b.v_IdCliente } into b_join
                                  from b in b_join.DefaultIfEmpty()

                                  join c in dbContext.documentoretenciondetalle on new { ret = a.v_IdDocumentoRetencion } equals new { ret = c.v_IdDocumentoRetencion } into c_join
                                  from c in c_join.DefaultIfEmpty()

                                  join d in dbContext.documento on new { i_IdTipoDocumento = c.i_IdTipoDocumento.Value } equals new { i_IdTipoDocumento = d.i_CodigoDocumento } into d_join
                                  from d in d_join.DefaultIfEmpty()


                                  where a.v_IdDocumentoRetencion == pstrIdDocumento && a.i_Eliminado == 0
                                  select new
                                  {

                                      t_FechaRegistro = a.t_FechaRegistro,
                                      NombreProveedor = b == null ? "" : (b.v_ApePaterno.Trim() + " " + b.v_ApeMaterno.Trim() + " " + b.v_PrimerNombre.Trim() + " " + b.v_RazonSocial.Trim()),
                                      RucProveedor = b == null ? "" : b.v_NroDocIdentificacion,
                                      TipoDocumentoDetalle = d == null ? "" : d.v_Siglas,
                                      v_SerieDocumentoDetalle = c.v_SerieDocumento,
                                      v_CorrelativoDocumentoDetalle = c.v_CorrelativoDocumento,
                                      MontoPagado = a.i_IdMoneda == (int)Currency.Soles ? c.d_MontoPago : c.d_Cambio.Value,  // c.d_MontoPago,
                                      ImporteRetenido = c.d_MontoRetenido,
                                      NumeroDocumentoRetencion = string.IsNullOrEmpty(a.v_SerieDocumento) || string.IsNullOrEmpty(a.v_CorrelativoDocumento) ? "" : a.v_SerieDocumento + " " + a.v_CorrelativoDocumento,
                                      v_IdProveedor = b.v_IdCliente,
                                      i_IdTipoDocumentoDetalle = c.i_IdTipoDocumento,
                                      MonedaRetencion = "S/",

                                  }).ToList().Select(o => new ReporteRetencion
                                {
                                    NombreProveedor = o.NombreProveedor,
                                    RucProveedor = o.RucProveedor,
                                    sFechaRegistro = o.t_FechaRegistro.Value.ToShortDateString(),
                                    TipoDocumentoDetalle = o.TipoDocumentoDetalle,
                                    v_SerieDocumentoDetalle = o.v_SerieDocumentoDetalle,
                                    v_CorrelativoDocumentoDetalle = o.v_CorrelativoDocumentoDetalle,
                                    MontoPagado = o.MontoPagado ?? 0,
                                    MontoRetenido = o.ImporteRetenido ?? 0,
                                    NumeroDocumentoRetencion = o.NumeroDocumentoRetencion,
                                    v_IdProveedor = o.v_IdProveedor,
                                    i_IdTipoDocumentoDetalle = o.i_IdTipoDocumentoDetalle.Value,

                                    MonedaRetencion = o.MonedaRetencion,
                                }).ToList();


                    var totalEnNumero = result.Any() ? result.Sum(o => o.MontoRetenido).ToString("0.00") : "0.00";




                    result.ForEach(docReten =>
                    {
                        var FechaCompra = Compras.Where(o => o.v_IdProveedor == docReten.v_IdProveedor && o.i_IdTipoDocumento == docReten.i_IdTipoDocumentoDetalle && o.v_SerieDocumento == docReten.v_SerieDocumentoDetalle && o.v_CorrelativoDocumento == docReten.v_CorrelativoDocumentoDetalle).Select(o => o.t_FechaEmision).FirstOrDefault();
                        docReten.TotalLetras = Utils.ConvertirenLetras(totalEnNumero);
                        docReten.sFechaEmisionCompra = FechaCompra != null ? FechaCompra.Value.ToShortDateString() : "";

                    });


                    pobjOperationResult.Success = 1;
                    return result;
                }
            }
            catch (Exception er)
            {
                pobjOperationResult.ExceptionMessage = er.Message;
                pobjOperationResult.AdditionalInformation = "DocumentoRetencionBL.ObtenerCabecera";
                return null;
            }
        }

        #region Tesorerias
        public void GenerarTesoreria(ref OperationResult pobjOperationResult, string strIdRetencion, int tipoComprobante, string glosa, string nroCuenta, int idMedioPago, int idDocPago, string nroOperacion)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var pobjDtoEntity = dbContext.documentoretencion.FirstOrDefault(p => p.v_IdDocumentoRetencion.Equals(strIdRetencion));
                    var pTemp_Insertar = pobjDtoEntity.documentoretenciondetalle.Where(p => p.i_Eliminado == 0).ToDTOs();
                    var _objTesoreriaBL = new TesoreriaBL();
                    var C_tesoreriaDto = new tesoreriaDto();
                    var _ListadoTesorerias = new List<KeyValueDTO>();
                    var _TesoreriaDetalleXInsertar = new List<tesoreriadetalleDto>();
                    var _TesoreriaDetalleGastos = new List<tesoreriadetalleDto>();
                    var _TesoreriaDetalleIntereses = new List<tesoreriadetalleDto>();
                    var _TesoreriaDetalleRetenciones = new List<tesoreriadetalleDto>();
                    var cliente = dbContext.cliente.FirstOrDefault(p => p.v_IdCliente.Equals(pobjDtoEntity.v_IdCliente));

                    _ListadoTesorerias = _objTesoreriaBL.ObtenerListadoTesorerias(ref pobjOperationResult, pobjDtoEntity.t_FechaRegistro.Value.Year.ToString(), pobjDtoEntity.t_FechaRegistro.Value.Month.ToString("00"), tipoComprobante);

                    var _MaxMovimiento = _ListadoTesorerias.Any() ? int.Parse(_ListadoTesorerias[_ListadoTesorerias.Count() - 1].Value1) : 0;
                    _MaxMovimiento++;

                    C_tesoreriaDto.v_Mes = pobjDtoEntity.t_FechaRegistro.Value.Month.ToString("00");
                    C_tesoreriaDto.v_Periodo = pobjDtoEntity.t_FechaRegistro.Value.Year.ToString();
                    C_tesoreriaDto.v_Correlativo = _MaxMovimiento.ToString("00000000");
                    C_tesoreriaDto.d_TipoCambio = pobjDtoEntity.d_TipoCambio ?? 0;
                    C_tesoreriaDto.i_IdMoneda = pobjDtoEntity.i_IdMoneda ?? 1;
                    C_tesoreriaDto.i_IdTipoDocumento = tipoComprobante;
                    C_tesoreriaDto.v_Glosa = glosa;
                    C_tesoreriaDto.v_NroCuentaCajaBanco = nroCuenta;
                    C_tesoreriaDto.i_IdEstado = 1;
                    C_tesoreriaDto.i_IdMedioPago = idMedioPago;
                    C_tesoreriaDto.t_FechaRegistro = pobjDtoEntity.t_FechaRegistro.Value;
                    C_tesoreriaDto.v_IdDocumentoRetencion = pobjDtoEntity.v_IdDocumentoRetencion;
                    C_tesoreriaDto.i_TipoMovimiento = (int?)TipoMovimientoTesoreria.Egreso;
                    if (cliente != null)
                        C_tesoreriaDto.v_Nombre = (cliente.v_RazonSocial + " " + cliente.v_ApePaterno + " " + cliente.v_ApeMaterno + " " +
                         cliente.v_PrimerNombre + " " + cliente.v_SegundoNombre).Trim();

                    #region Agrega cuentas detalle
                    foreach (var Fila in pTemp_Insertar)
                    {
                        var dhTesoreriadetalleDto = new tesoreriadetalleDto();
                        const string idConcepto = "01";
                        var importe = (Fila.d_MontoPago ?? 0);
                        dhTesoreriadetalleDto.v_IdTesoreria = C_tesoreriaDto.v_IdTesoreria;
                        dhTesoreriadetalleDto.d_Cambio = C_tesoreriaDto.i_IdMoneda == 1 ? importe / C_tesoreriaDto.d_TipoCambio : importe * C_tesoreriaDto.d_TipoCambio;
                        dhTesoreriadetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(dhTesoreriadetalleDto.d_Cambio ?? 0, 2);

                        dhTesoreriadetalleDto.d_Importe = importe;
                        dhTesoreriadetalleDto.i_IdTipoDocumentoRef = -1;
                        dhTesoreriadetalleDto.v_Naturaleza = "D";

                        dhTesoreriadetalleDto.v_Analisis = C_tesoreriaDto.v_Glosa;
                        dhTesoreriadetalleDto.v_IdCliente = cliente.v_IdCliente;

                        dhTesoreriadetalleDto.v_NroCuenta = (dbContext.administracionconceptos.Where(
                            v => v.v_Codigo == idConcepto && v.v_Periodo.Equals(Periodo))
                            .Select(v => new { v.v_CuentaPVenta })).FirstOrDefault().v_CuentaPVenta;

                        dhTesoreriadetalleDto.v_NroDocumento = Fila.v_SerieDocumento + "-" + Fila.v_CorrelativoDocumento;
                        dhTesoreriadetalleDto.v_OrigenDestino = "";
                        dhTesoreriadetalleDto.v_Pedido = "";
                        dhTesoreriadetalleDto.t_Fecha = pobjDtoEntity.t_FechaRegistro;
                        dhTesoreriadetalleDto.i_IdTipoDocumento = Fila.i_IdTipoDocumento ?? 1;
                        dhTesoreriadetalleDto.i_IdCentroCostos = "0";
                        dhTesoreriadetalleDto.i_IdCaja = 0;
                        _TesoreriaDetalleXInsertar.Add(dhTesoreriadetalleDto);

                        //----------------------------------------------------

                        dhTesoreriadetalleDto = new tesoreriadetalleDto();
                        var importeRetencion = (Fila.d_MontoRetenido ?? 0);
                        dhTesoreriadetalleDto.v_IdTesoreria = C_tesoreriaDto.v_IdTesoreria;
                        dhTesoreriadetalleDto.d_Cambio = C_tesoreriaDto.i_IdMoneda == 1 ? importeRetencion / C_tesoreriaDto.d_TipoCambio : importeRetencion * C_tesoreriaDto.d_TipoCambio;
                        dhTesoreriadetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(dhTesoreriadetalleDto.d_Cambio ?? 0, 2);

                        dhTesoreriadetalleDto.d_Importe = importeRetencion;
                        if (C_tesoreriaDto.i_IdMoneda == 2)
                        {
                            dhTesoreriadetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(dhTesoreriadetalleDto.d_Importe /
                                                              C_tesoreriaDto.d_TipoCambio ?? 1, 2);
                            dhTesoreriadetalleDto.d_Cambio = importeRetencion;
                        }

                        dhTesoreriadetalleDto.i_IdTipoDocumentoRef = -1;
                        dhTesoreriadetalleDto.v_Naturaleza = "H";

                        dhTesoreriadetalleDto.v_Analisis = C_tesoreriaDto.v_Glosa;
                        dhTesoreriadetalleDto.v_IdCliente = cliente.v_IdCliente;
                        dhTesoreriadetalleDto.v_NroCuenta = Globals.ClientSession.v_NroCuentaRetencion;
                        dhTesoreriadetalleDto.v_NroDocumento = Fila.v_SerieDocumento + "-" + Fila.v_CorrelativoDocumento;
                        dhTesoreriadetalleDto.v_OrigenDestino = "";
                        dhTesoreriadetalleDto.v_Pedido = "";
                        dhTesoreriadetalleDto.t_Fecha = pobjDtoEntity.t_FechaRegistro;
                        dhTesoreriadetalleDto.i_IdTipoDocumento = Fila.i_IdTipoDocumento ?? 1;
                        dhTesoreriadetalleDto.i_IdCentroCostos = "0";
                        dhTesoreriadetalleDto.v_IdDocumentoRetencionDetalle = Fila.v_IdDocumentoRetencionDetalle;
                        dhTesoreriadetalleDto.i_IdCaja = 0;
                        _TesoreriaDetalleXInsertar.Add(dhTesoreriadetalleDto);

                    }
                    #endregion

                    #region Se elabora la contracuenta cuando se cobra una letra que no esta en descuento
                    {
                        var ddTesoreriadetalleDto = new tesoreriadetalleDto();
                        var sumaD = _TesoreriaDetalleXInsertar.Where(p => p.v_Naturaleza.Equals("D")).Sum(o => o.d_Importe ?? 0);
                        var sumaH = _TesoreriaDetalleXInsertar.Where(p => p.v_Naturaleza.Equals("H")).Sum(o => o.d_Importe ?? 0);
                        var cambioD = _TesoreriaDetalleXInsertar.Where(p => p.v_Naturaleza.Equals("D")).Sum(o => o.d_Cambio ?? 0);
                        var cambioH = _TesoreriaDetalleXInsertar.Where(p => p.v_Naturaleza.Equals("H")).Sum(o => o.d_Cambio ?? 0);
                        var importe = sumaD - sumaH;
                        var cambio = cambioD - cambioH;
                        var idTipoDocRef = _TesoreriaDetalleXInsertar.LastOrDefault() != null
                            ? _TesoreriaDetalleXInsertar.LastOrDefault().i_IdTipoDocumentoRef ?? -1
                            : -1;
                        ddTesoreriadetalleDto.v_IdTesoreria = C_tesoreriaDto.v_IdTesoreria;
                        ddTesoreriadetalleDto.d_Cambio = cambio;
                        Utils.Windows.DevuelveValorRedondeado(ddTesoreriadetalleDto.d_Cambio ?? 0, 2);
                        ddTesoreriadetalleDto.d_Importe = importe;
                        ddTesoreriadetalleDto.i_IdTipoDocumentoRef = _TesoreriaDetalleXInsertar.Count == 1
                            ? _TesoreriaDetalleXInsertar[0].i_IdTipoDocumento ?? -1
                            : -1;
                        ddTesoreriadetalleDto.v_Naturaleza = "H";
                        ddTesoreriadetalleDto.v_Analisis = C_tesoreriaDto.v_Glosa;
                        ddTesoreriadetalleDto.v_NroCuenta = C_tesoreriaDto.v_NroCuentaCajaBanco;
                        ddTesoreriadetalleDto.v_NroDocumento = nroOperacion;
                        ddTesoreriadetalleDto.v_NroDocumentoRef = _TesoreriaDetalleXInsertar.Count == 1
                            ? _TesoreriaDetalleXInsertar[0].v_NroDocumento
                            : string.Empty;
                        ddTesoreriadetalleDto.v_OrigenDestino = "";
                        ddTesoreriadetalleDto.v_Pedido = "";
                        ddTesoreriadetalleDto.t_Fecha = C_tesoreriaDto.t_FechaRegistro;
                        ddTesoreriadetalleDto.i_IdTipoDocumento = idDocPago;
                        ddTesoreriadetalleDto.i_IdCentroCostos = "0";
                        ddTesoreriadetalleDto.i_IdCaja = 0;
                        if (Utils.Windows.CuentaRequiereDetalle(ddTesoreriadetalleDto.v_NroCuenta))
                        {
                            var filaCliente = _TesoreriaDetalleXInsertar.FirstOrDefault(p => p.v_IdCliente != null);
                            if (filaCliente != null)
                                ddTesoreriadetalleDto.v_IdCliente = filaCliente.v_IdCliente;
                        }
                        _TesoreriaDetalleXInsertar.Add(ddTesoreriadetalleDto);
                    }

                    #endregion

                    _TesoreriaDetalleXInsertar = _TesoreriaDetalleXInsertar.Concat(_TesoreriaDetalleGastos)
                            .Concat(_TesoreriaDetalleIntereses).Concat(_TesoreriaDetalleRetenciones).ToList();
                    decimal TotDebe, TotHaber, TotDebeC, TotHaberC;
                    TotDebe = _TesoreriaDetalleXInsertar.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Importe.Value);
                    TotHaber = _TesoreriaDetalleXInsertar.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Importe.Value);
                    TotDebeC = _TesoreriaDetalleXInsertar.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Cambio.Value);
                    TotHaberC = _TesoreriaDetalleXInsertar.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Cambio.Value);
                    C_tesoreriaDto.d_TotalDebe_Importe = TotDebe;
                    C_tesoreriaDto.d_TotalHaber_Importe = TotHaber;
                    C_tesoreriaDto.d_TotalDebe_Cambio = TotDebeC;
                    C_tesoreriaDto.d_TotalHaber_Cambio = TotHaberC;
                    C_tesoreriaDto.d_Diferencia_Importe = (TotDebe - TotHaber);
                    C_tesoreriaDto.d_Diferencia_Cambio = (TotDebeC - TotHaberC);

                    if (_TesoreriaDetalleXInsertar.Any())
                    {
                        _objTesoreriaBL.Insertartesoreria(ref pobjOperationResult, C_tesoreriaDto, Globals.ClientSession.GetAsList(), _TesoreriaDetalleXInsertar);
                        if (pobjOperationResult.Success == 0) return;
                    }
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "CobranzaBL.GenerarTesoreria()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public string DevolverAsientoContableTesoreria(string pstrIdRetencion)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var asientoRef = dbContext.tesoreria.FirstOrDefault(p => p.v_IdDocumentoRetencion.Equals(pstrIdRetencion) && p.i_Eliminado == 0);
                    return asientoRef != null ? asientoRef.v_IdTesoreria : null;
                }
            }
            catch
            {
                return null;
            }
        }

        public void EliminarTesoreria(ref OperationResult pobjOperationResult, string pstrIdRetencion)
        {
            try
            {
                var idt = DevolverAsientoContableTesoreria(pstrIdRetencion);
                if (idt == null) return;
                new TesoreriaBL().Eliminartesoreria(ref pobjOperationResult, idt, Globals.ClientSession.GetAsList());
                if (pobjOperationResult.Success == 0) return;
                pobjOperationResult.Success = 1;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "CobranzaBL.EliminarTesoreria()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = pobjOperationResult.ExceptionMessage != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }
        #endregion

        public BindingList<documentoretenciondetalleDto> ObtenerDetalles(ref OperationResult pobjOperationResult, string pstrIdDocumento)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var consulta = (from a in dbContext.documentoretenciondetalle
                                    join J4 in dbContext.documento on new { i_IdTipoDocumento = a.i_IdTipoDocumento.Value }
                                                                    equals new { i_IdTipoDocumento = J4.i_CodigoDocumento } into J4_join
                                    from J4 in J4_join.DefaultIfEmpty()

                                    where a.v_IdDocumentoRetencion == pstrIdDocumento && a.i_Eliminado == 0
                                    select new documentoretenciondetalleDto
                                    {
                                        v_CorrelativoDocumento = a.v_CorrelativoDocumento,
                                        v_IdDocumentoRetencion = a.v_IdDocumentoRetencion,
                                        v_IdDocumentoRetencionDetalle = a.v_IdDocumentoRetencionDetalle,
                                        v_SerieDocumento = a.v_SerieDocumento,
                                        d_MontoPago = a.d_MontoPago,
                                        d_MontoRetenido = a.d_MontoRetenido,
                                        i_IdTipoDocumento = a.i_IdTipoDocumento,
                                        i_InsertaIdUsuario = a.i_InsertaIdUsuario,
                                        t_InsertaFecha = a.t_InsertaFecha,
                                        TipoDocumento = J4.v_Siglas,
                                        i_ActualizaUsuario = a.i_ActualizaUsuario,
                                        i_Eliminado = a.i_Eliminado,
                                        t_ActualizaFecha = a.t_ActualizaFecha,
                                        v_IdCompra = a.v_IdCompra,
                                        d_Cambio = a.d_Cambio,
                                        i_Procesado = a.i_Procesado,
                                        i_Independiente = a.i_Independiente
                                    }).ToList();

                    pobjOperationResult.Success = 1;
                    return new BindingList<documentoretenciondetalleDto>(consulta);
                }
            }
            catch (Exception er)
            {
                pobjOperationResult.ExceptionMessage = er.Message;
                pobjOperationResult.AdditionalInformation = "DocumentoRetencionBL.ObtenerDetalles";
                return null;
            }
        }

        public List<documentoretencionDto> ListarDocumentos(ref OperationResult pobjOperationResult, string filterExpression, DateTime fIni, DateTime fFin)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var result = (from d in dbContext.documentoretencion

                                  join J3 in dbContext.cliente on d.v_IdCliente equals J3.v_IdCliente into J3_join
                                  from J3 in J3_join.DefaultIfEmpty()

                                  join J1 in dbContext.systemuser on new { i_InsertUserId = d.i_InsertaIdUsuario.Value }
                                      equals new { i_InsertUserId = J1.i_SystemUserId } into J1_join
                                  from J1 in J1_join.DefaultIfEmpty()

                                  where d.t_FechaRegistro >= fIni && d.t_FechaRegistro <= fFin && d.i_Eliminado == 0

                                  select new documentoretencionDto
                                  {
                                      v_Periodo = d.v_Periodo,
                                      v_Correlativo = d.v_Correlativo,
                                      v_CorrelativoDocumento = d.v_CorrelativoDocumento,
                                      v_IdCliente = d.v_IdCliente,
                                      v_IdDocumentoRetencion = d.v_IdDocumentoRetencion,
                                      v_Mes = d.v_Mes,
                                      v_IdTesoreria = d.v_IdTesoreria,
                                      v_SerieDocumento = d.v_SerieDocumento,
                                      i_Estado = d.i_Estado,
                                      i_InsertaIdUsuario = d.i_InsertaIdUsuario,
                                      t_InsertaFecha = d.t_InsertaFecha,
                                      t_FechaRegistro = d.t_FechaRegistro,
                                      NombreProveedor =
                                          J3.v_PrimerNombre + " " + J3.v_ApePaterno + " " + J3.v_ApeMaterno + " " +
                                          J3.v_RazonSocial,
                                      RucProveedor = J3.v_NroDocIdentificacion,
                                      NroRegistro = d.v_Mes + "-" + d.v_Correlativo,
                                      UsuarioCreacion = J1.v_UserName,
                                      d_TotalRetenido = d.d_TotalRetenido,
                                      i_EstadoSunat = d.i_EstadoSunat,
                                      Contabilizado = dbContext.tesoreria.Any(p => p.v_IdDocumentoRetencion.Equals(d.v_IdDocumentoRetencion) && p.i_Eliminado == 0),
                                      NroRetencion = d.v_SerieDocumento + "-" + d.v_CorrelativoDocumento,
                                      //Tesoreria = (from t in dbContext.tesoreria
                                      //            where t.v_IdDocumentoRetencion.Equals(d.v_IdDocumentoRetencion) && t.i_Eliminado == 0
                                      //            select new { nro = t.v_Periodo + " " + t.v_Mes + "-"+ t.v_Correlativo }).FirstOrDefault().nro
                                  }
                        );

                    pobjOperationResult.Success = 1;

                    if (!string.IsNullOrEmpty(filterExpression))
                        result = result.Where(filterExpression);

                    return result.ToList();
                }
            }
            catch (Exception er)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = er.Message;
                pobjOperationResult.AdditionalInformation = "DocumentoRetencionBL.ListarDocumentos";
                return null;
            }
        }

        public void GenerarRegistro(ref OperationResult pobjOperationResult, string pstrIdDocumento, string serie = null, string correlativo = null)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var objDocumento = (from a in dbContext.documentoretencion
                                        where a.v_IdDocumentoRetencion == pstrIdDocumento
                                        select a).FirstOrDefault();

                    if (objDocumento != null)
                    {
                        //20 es el documento de retenciones fijado por la sunat
                        var serieSistema =
                            new DocumentoBL().DevolverSeriePorDocumento(Globals.ClientSession.i_IdEstablecimiento, 20);
                        var correlativoSistema =
                            new DocumentoBL().DevolverCorrelativoPorDocumento(
                                Globals.ClientSession.i_IdEstablecimiento, 20).ToString("00000000");
                        objDocumento.v_SerieDocumento = serie ?? serieSistema;
                        objDocumento.v_CorrelativoDocumento = correlativo ?? correlativoSistema;

                        if (!string.IsNullOrEmpty(objDocumento.v_SerieDocumento) && int.Parse(objDocumento.v_CorrelativoDocumento) != 0)
                        {
                            #region Actualiza Correlativo EmpresaDetalle
                            if (int.Parse(objDocumento.v_CorrelativoDocumento) > int.Parse(correlativoSistema))
                                new DocumentoBL().ActualizarCorrelativoPorSerie(ref pobjOperationResult, Globals.ClientSession.i_IdEstablecimiento, 20, objDocumento.v_SerieDocumento, int.Parse(objDocumento.v_CorrelativoDocumento) + 1);
                            #endregion

                            dbContext.documentoretencion.ApplyCurrentValues(objDocumento);
                            dbContext.SaveChanges();
                            pobjOperationResult.Success = 1;
                        }
                        else
                        {
                            pobjOperationResult.AdditionalInformation = "El documento no se encuentra registrado en la Configuración de la empresa.";
                            pobjOperationResult.Success = 0;
                        }
                    }
                    else
                    {
                        pobjOperationResult.AdditionalInformation = "El documento no se encontró.";
                        pobjOperationResult.Success = 0;
                    }

                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException.Message;
                pobjOperationResult.AdditionalInformation = "DocumentoRetencionBL.GenerarRegistro()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.Success = 0;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public void AnularDocumento(ref OperationResult pobjOperationResult, string pstrIdDocumento)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    documentoretencion objDocumento = (from a in dbContext.documentoretencion
                                                       where a.v_IdDocumentoRetencion == pstrIdDocumento && a.i_Estado == 1
                                                       select a).FirstOrDefault();

                    if (objDocumento != null)
                    {
                        objDocumento.i_Estado = 0;
                        dbContext.documentoretencion.ApplyCurrentValues(objDocumento);
                        dbContext.SaveChanges();
                        pobjOperationResult.Success = 1;
                    }
                    else
                    {
                        pobjOperationResult.AdditionalInformation = "El documento no se encontró o ya está anulado.";
                        pobjOperationResult.Success = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException.Message;
                pobjOperationResult.AdditionalInformation = "DocumentoRetencionBL.AnularDocumento()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.Success = 0;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public bool RetencionContabilizada(ref OperationResult pobjOperationResult, string idRetencion)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var contabilizado = dbContext.tesoreria.Any(p => p.v_IdDocumentoRetencion.Equals(idRetencion) && p.i_Eliminado == 0);
                    pobjOperationResult.Success = 1;
                    return contabilizado;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.ErrorMessage = ex.Message;
                if (ex.InnerException != null) pobjOperationResult.ExceptionMessage = ex.InnerException.Message;
                pobjOperationResult.AdditionalInformation = "DocumentoRetencionBL.RetencionContabilizada()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.Success = 0;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return false;
            }
        }

        public bool RetencionExiste(ref OperationResult pobjOperationResult, string serie, string correlativo, string id)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    id = string.IsNullOrWhiteSpace(id) ? null : id;
                    var existe =
                        dbContext.documentoretencion.Any(
                            p =>
                                p.v_SerieDocumento.Equals(serie) && p.v_CorrelativoDocumento.Equals(correlativo) &&
                                p.i_Eliminado == 0 && (id == null || p.v_IdDocumentoRetencion != id));

                    pobjOperationResult.Success = 1;
                    return existe;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.ErrorMessage = ex.Message;
                if (ex.InnerException != null) pobjOperationResult.ExceptionMessage = ex.InnerException.Message;
                pobjOperationResult.AdditionalInformation = "DocumentoRetencionBL.RetencionExiste()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.Success = 0;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return false;
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
                                  TipoCambio = r.d_TipoCambio ?? 1,
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
        /// <param name="pstrIdRetencion">Id del Documento de Retencion</param>
        /// <param name="tipo">1 = CDR Envio, 2 = CDR de Reversion</param>
        public byte[] GetZipContent(ref OperationResult pobjOperationResult, string pstrIdRetencion, char tipo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var item = (from n in dbContext.documentoretencion
                                where n.v_IdDocumentoRetencion == pstrIdRetencion
                                select n).First();
                    var bFile = tipo == '1' ?
                            (from i in item.documentoretencionhomologacion
                             select i.b_FileXml).First() :
                            (from i in item.documentoretencionhomologacion
                             select i.b_ResponseTicket).First();
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
