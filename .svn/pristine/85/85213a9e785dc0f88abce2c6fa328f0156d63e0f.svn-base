using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.Linq;
using Devart.Data.PostgreSql;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.Common.DataModel;

namespace SAMBHS.Requerimientos.NBS
{
    /// <summary>
    /// EQC 6-10-2016
    /// </summary>
    public class ExportarIrpesBl
    {
        public string Host { get; set; }
        public string BaseDatosDestino { get; set; }
        public string Usuario { get; set; }
        public string Password { get; set; }

        private PgSqlConnectionStringBuilder ObjStringBuilderBdDestino
        {
            get
            {
                return new PgSqlConnectionStringBuilder
                {
                    Host = Host,
                    Database = BaseDatosDestino,
                    UserId = Usuario,
                    Password = Password,
                    Schema = "public"
                };
            }
        }

        /// <summary>
        /// Arma la cadena de conexión completa
        /// </summary>
        /// <param name="rawCnString"></param>
        /// <returns></returns>
        private static string GetConnString(string rawCnString)
        {
            var newconnstring = new EntityConnectionStringBuilder
            {
                Metadata = @"res://*/DMMSQLWIN.csdl|res://*/DMMSQLWIN.ssdl|res://*/DMMSQLWIN.msl",
                Provider = "Devart.Data.PostgreSql",
                ProviderConnectionString = rawCnString
            };
            return newconnstring.ToString();
        }

        /// <summary>
        /// Obtiene un listado de las irpes que se cobraron en la fecha solicitada
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="fechaBusqueda"></param>
        /// <returns></returns>
        private static IEnumerable<AsientoIrpeDto> ObtenerAsientoIrpes(ref OperationResult pobjOperationResult, DateTime fechaBusqueda)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var documentoIrpe = dbContext.documento.FirstOrDefault(p => p.i_CodigoDocumento.Equals(438));
                    if (documentoIrpe == null)
                        throw new ArgumentNullException(@"El no se encontró el documento 438 Irpe");
                    var anio = fechaBusqueda.Year;
                    var mes = fechaBusqueda.Month;
                    var dia = fechaBusqueda.Day;
                    var ctaIrpe = documentoIrpe.v_NroCuenta;

                    var query = (from n in dbContext.cobranzadetalle
                                 join j1 in dbContext.venta on new { id = n.v_IdVenta, eliminado = 0 }
                                                             equals new { id = j1.v_IdVenta, eliminado = j1.i_Eliminado.Value } into j1Join
                                 from j1 in j1Join.DefaultIfEmpty()
                                 join j2 in dbContext.cliente on j1.v_IdCliente equals j2.v_IdCliente into j2Join
                                 from j2 in j2Join.DefaultIfEmpty()
                                 join j3 in dbContext.cobranza on n.v_IdCobranza equals j3.v_IdCobranza into j3Join
                                 from j3 in j3Join.DefaultIfEmpty()
                                 join j4 in dbContext.documento on j3.i_IdTipoDocumento.Value equals j4.i_CodigoDocumento into j4Join
                                 from j4 in j4Join.DefaultIfEmpty()

                                 where n.i_Eliminado == 0 && (j1.i_IdTipoDocumento ?? -1) == 438
                                 && j3.t_FechaRegistro.Value.Year == anio && j3.t_FechaRegistro.Value.Month == mes
                                 && j3.t_FechaRegistro.Value.Day == dia && j3.i_Eliminado == 0
                                 select new AsientoIrpeDto
                                 {
                                     CentroCosto = "",
                                     IdCliente = j2.v_IdCliente,
                                     IdDocumento = j1.i_IdTipoDocumento ?? -1,
                                     Naturaleza = "D",
                                     NroCuentaCaja = j4.v_NroCuenta,
                                     NroDocumento = j1.v_SerieDocumento + "-" + j1.v_CorrelativoDocumento,
                                     Importe = n.d_ImporteSoles ?? 0,
                                     TipoCambio = j3.d_TipoCambio ?? 0,
                                     Moneda = j3.i_IdMoneda == 1 ? "S" : "D",
                                     TipoDocumentoCaja = j3.i_IdTipoDocumento ?? -1,
                                     NroDocIdentidadCliente = j2.v_NroDocIdentificacion
                                 }).ToList();

                    query.ForEach(p => p.NroCuentaIrpe = ctaIrpe);

                    pobjOperationResult.Success = 1;
                    return query;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ExportarIrpesBL.ObtenerAsientoIrpes()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        /// <summary>
        /// Se efectua en la base de datos destino, y se compara los clientes que se recibe, si no existe se crea.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="listaIdClientes"></param>
        private List<clienteDto> RegularClientes(ref OperationResult pobjOperationResult, IEnumerable<string> listaIdClientes)
        {
            var listaClienteRetorno = new List<clienteDto>();
            try
            {
                List<clienteDto> listaClientePorRegular;
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    listaClientePorRegular = dbContext.cliente.Where(p => listaIdClientes.Contains(p.v_IdCliente)).ToDTOs();
                }

                using (var dbContext = new SAMBHSEntitiesModelWin(GetConnString(ObjStringBuilderBdDestino.ConnectionString)))
                {
                    foreach (var clienteDto in listaClientePorRegular)
                    {
                        var clienteEquivalente =
                            dbContext.cliente.FirstOrDefault(p =>
                                    p.v_NroDocIdentificacion.Equals(clienteDto.v_NroDocIdentificacion) &&
                                    p.v_FlagPantalla.Equals("C"));

                        if (clienteEquivalente == null)
                        {
                            var objEntity = clienteDto.ToEntity();
                            objEntity.t_InsertaFecha = DateTime.Now;
                            objEntity.i_InsertaIdUsuario = int.Parse(Globals.ClientSession.GetAsList()[2]);
                            objEntity.i_Eliminado = 0;
                            var intNodeId = int.Parse(Globals.ClientSession.GetAsList()[2]);
                            var secuentialId = GetNextSecuentialId(intNodeId, 14);
                            var newIdCliente = Utils.GetNewId(int.Parse(Globals.ClientSession.GetAsList()[2]), secuentialId, "CL");
                            objEntity.v_IdCliente = newIdCliente;
                            dbContext.AddTocliente(objEntity);
                            listaClienteRetorno.Add(objEntity.ToDTO());
                        }
                        else
                        {
                            listaClienteRetorno.Add(clienteEquivalente.ToDTO());
                        }
                    }

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }

                return listaClienteRetorno;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ExportarIrpesBL.RegularClientes()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        /// <summary>
        /// Inserta la tesorería en la bd destino.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pobjDtoEntity"></param>
        /// <param name="pTempInsertar"></param>
        private void InsertarTesoreriaExterna(ref OperationResult pobjOperationResult, tesoreriaDto pobjDtoEntity, IEnumerable<tesoreriadetalleDto> pTempInsertar)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin(GetConnString(ObjStringBuilderBdDestino.ConnectionString)))
                {
                    var idUsuario = Globals.ClientSession.GetAsList()[2];
                    var idNodeId = Globals.ClientSession.GetAsList()[0];
                    var objEntitytesoreria = pobjDtoEntity.ToEntity();

                    #region Inserta Cabecera

                    objEntitytesoreria.t_InsertaFecha = DateTime.Now;
                    objEntitytesoreria.i_InsertaIdUsuario = int.Parse(idUsuario);
                    objEntitytesoreria.i_Eliminado = 0;

                    var intNodeId = int.Parse(idNodeId);
                    var secuentialId = GetNextSecuentialId(intNodeId, 55);
                    var newIdtesoreria = Utils.GetNewId(int.Parse(idNodeId), secuentialId, "XE");
                    pobjDtoEntity.v_IdTesoreria = newIdtesoreria;
                    objEntitytesoreria.v_IdTesoreria = newIdtesoreria;
                    dbContext.AddTotesoreria(objEntitytesoreria);
                    dbContext.SaveChanges();

                    #endregion

                    #region Inserta Detalle Y Actualiza Pendientes Por Cobrar

                    foreach (var tesoreriadetalleDto in pTempInsertar)
                    {
                        var objEntitytesoreriaDetalle = tesoreriadetalleDto.ToEntity();
                        secuentialId = GetNextSecuentialId(intNodeId, 56);
                        var newIdtesoreriaDetalle = Utils.GetNewId(int.Parse(idNodeId), secuentialId, "XF");
                        objEntitytesoreriaDetalle.v_IdTesoreriaDetalle = newIdtesoreriaDetalle;
                        objEntitytesoreriaDetalle.v_IdCliente = string.IsNullOrWhiteSpace(objEntitytesoreriaDetalle.v_IdCliente) ? null : objEntitytesoreriaDetalle.v_IdCliente.Trim();
                        objEntitytesoreriaDetalle.v_IdTesoreria = newIdtesoreria;
                        objEntitytesoreriaDetalle.t_InsertaFecha = DateTime.Now;
                        objEntitytesoreriaDetalle.i_InsertaIdUsuario = Int32.Parse(idUsuario);
                        objEntitytesoreriaDetalle.i_Eliminado = 0;

                        dbContext.AddTotesoreriadetalle(objEntitytesoreriaDetalle);
                    }
                    dbContext.SaveChanges();

                    #endregion

                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ExportarIrpesBL.InsertarTesoreriaExterna()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        /// <summary>
        /// Obtiene el siguiente secuencial de la tabla de la bd destino
        /// </summary>
        /// <param name="pintNodeId"></param>
        /// <param name="pintTableId"></param>
        /// <returns></returns>
        private int GetNextSecuentialId(int pintNodeId, int pintTableId)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin(GetConnString(ObjStringBuilderBdDestino.ConnectionString)))
            {
                var replicationId = Globals.ClientSession.ReplicationNodeID;
                var objSecuential = (dbContext.secuential.Where(
                    a => a.i_TableId == pintTableId && a.i_NodeId == pintNodeId && a.v_ReplicationID == replicationId)).SingleOrDefault();

                if (objSecuential != null)
                {
                    objSecuential.i_SecuentialId = objSecuential.i_SecuentialId + 1;
                }
                else
                {
                    objSecuential = new secuential
                    {
                        i_NodeId = pintNodeId,
                        i_TableId = pintTableId,
                        i_SecuentialId = 1,
                        v_ReplicationID = replicationId
                    };
                    dbContext.AddTosecuential(objSecuential);
                }

                dbContext.SaveChanges();

                return objSecuential.i_SecuentialId ?? 0;
            }
        }

        /// <summary>
        /// Obtiene el listado de tesorerías para obtener la siguiente
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstringPeriodo"></param>
        /// <param name="pstringMes"></param>
        /// <param name="idTipoDocumento"></param>
        /// <returns></returns>
        private List<KeyValueDTO> ObtenerListadoTesorerias(ref OperationResult pobjOperationResult, string pstringPeriodo,
            string pstringMes, int idTipoDocumento)
        {
            try
            {
                if (Globals.ClientSession.i_IdAlmacenPredeterminado != null)
                {
                    var almacenpredeterminado = Globals.ClientSession.i_IdAlmacenPredeterminado.Value.ToString("00");
                    var replicationId = Globals.ClientSession.ReplicationNodeID;
                    SAMBHSEntitiesModelWin dbcontext = new SAMBHSEntitiesModelWin(GetConnString(ObjStringBuilderBdDestino.ConnectionString));
                    var query = (from n in dbcontext.tesoreria
                                 where
                                     n.i_Eliminado == 0 && n.v_Periodo == pstringPeriodo && n.v_Mes == pstringMes &&
                                     n.i_IdTipoDocumento == idTipoDocumento &&
                                     n.v_IdTesoreria.Substring(2, 2) == almacenpredeterminado
                                     && n.v_IdTesoreria.Substring(0, 1) == replicationId
                                 orderby n.v_Correlativo ascending
                                 select new
                                 {
                                     v_Correlativo = n.v_Correlativo,
                                     v_IdTesoreria = n.v_IdTesoreria
                                 }
                        );

                    pobjOperationResult.Success = 1;
                    if (query.Any())
                    {
                        var query2 = query.AsEnumerable()
                            .Select(x => new KeyValueDTO
                            {
                                Value1 = x.v_Correlativo,
                                Value2 = x.v_IdTesoreria
                            }).ToList();
                        return query2;
                    }
                    else
                    {
                        return new List<KeyValueDTO> { new KeyValueDTO { Value1 = almacenpredeterminado + "000000" } };
                    }
                }

                throw new Exception("No se encontró un almacén predeterminado");
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                return null;
            }
        }

        public void EliminarExportacionAnterior(ref OperationResult pobjOperationResult, DateTime fechaBusqueda)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin(GetConnString(ObjStringBuilderBdDestino.ConnectionString)))
                {
                    var anio = fechaBusqueda.Year;
                    var mes = fechaBusqueda.Month;
                    var dia = fechaBusqueda.Day;
                    var tesoreriasAnteriores = dbContext.tesoreria.Where(p =>
                                p.i_Eliminado == 0 && p.v_IdCobranza.Equals("M_Exportacion") &&
                                p.t_FechaRegistro.Value.Year == anio && p.t_FechaRegistro.Value.Month == mes &&
                                p.t_FechaRegistro.Value.Day == dia);

                    foreach (var tesoreriasAnterior in tesoreriasAnteriores)
                    {
                        tesoreriasAnterior.tesoreriadetalle.ToList()
                            .ForEach(t => dbContext.tesoreriadetalle.DeleteObject(t));
                        dbContext.tesoreria.DeleteObject(tesoreriasAnterior);
                    }

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ExportarIrpesBL.EliminarExportacionAnterior()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        /// <summary>
        /// Inserta la tesorería o tesorerías en la base de datos determinada por la cadena de conexión.
        /// Por cada cuenta 10 diferente en la lista recibida, se inserta una tesoreria diferente.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="fechaBusqueda"></param>
        public void IniciarProceso(ref OperationResult pobjOperationResult, DateTime fechaBusqueda)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    EliminarExportacionAnterior(ref pobjOperationResult, fechaBusqueda);
                    if (pobjOperationResult.Success == 0) return;

                    var listaCobranzasIrpe = ObtenerAsientoIrpes(ref pobjOperationResult, fechaBusqueda).ToList();

                    if (listaCobranzasIrpe.Any() && pobjOperationResult.Success == 1)
                    {
                        var idClientesPorRegular = listaCobranzasIrpe.Select(p => p.IdCliente).ToList();
                        var clientesBdDestino = RegularClientes(ref pobjOperationResult, idClientesPorRegular);
                        if (pobjOperationResult.Success == 0) return;

                        foreach (var subListaIrpe in listaCobranzasIrpe.GroupBy(p => new
                        {
                            nroCtaCaja = p.NroCuentaCaja,
                            moneda = p.Moneda,
                            documentoCaja = p.TipoDocumentoCaja
                        }))
                        {
                            #region arma cabecera
                            var listadoTesorerias = ObtenerListadoTesorerias(ref pobjOperationResult,
                                                fechaBusqueda.Year.ToString(), fechaBusqueda.Month.ToString("00"),
                                                subListaIrpe.Key.documentoCaja);
                            if (pobjOperationResult.Success == 0) return;
                            var maxMovimiento = listadoTesorerias.Any()
                                ? int.Parse(listadoTesorerias[listadoTesorerias.Count - 1].Value1) : 0;

                            maxMovimiento++;
                            var cTesoreriaDto = new tesoreriaDto
                            {
                                d_TipoCambio = subListaIrpe.Average(p => p.TipoCambio),
                                i_IdMoneda = subListaIrpe.Key.moneda == "S" ? 1 : 2,
                                v_Mes = fechaBusqueda.Month.ToString("00"),
                                v_Periodo = fechaBusqueda.Year.ToString(),
                                v_Correlativo = maxMovimiento.ToString("00000000"),
                                i_IdTipoDocumento = subListaIrpe.Key.documentoCaja,
                                v_Glosa = "Asientos Irpe Importados de Notaría Becerra Sosaya",
                                v_Nombre = "Asientos Irpe Importados de Notaría Becerra Sosaya",
                                v_NroCuentaCajaBanco = subListaIrpe.Key.nroCtaCaja.Trim(),
                                i_IdEstado = 1,
                                i_IdMedioPago = 1,
                                v_IdCobranza = "M_Exportacion",
                                t_FechaRegistro = fechaBusqueda,
                                i_TipoMovimiento = (int?)TipoMovimientoTesoreria.Ingreso,
                            };
                            #endregion

                            #region arma detalles
                            var tesoreriadetalleDtos = subListaIrpe.Select(p => new tesoreriadetalleDto
                            {
                                d_Importe = p.Importe,
                                d_Cambio = p.Moneda.Equals("S") ? p.Importe / p.TipoCambio : p.Importe * p.TipoCambio,
                                i_IdTipoDocumentoRef = -1,
                                v_Naturaleza = p.Naturaleza,
                                v_NroCuenta = p.NroCuentaIrpe,
                                v_NroDocumento = p.NroDocumento,
                                v_NroDocumentoRef = string.Empty,
                                v_OrigenDestino = "",
                                v_Pedido = "",
                                t_Fecha = fechaBusqueda,
                                i_IdTipoDocumento = 438, //<-seleccionar el tipo documento por parametro
                                i_IdCentroCostos = string.Empty,
                                i_IdCaja = 0,
                                v_IdCliente = clientesBdDestino.FirstOrDefault(d => d.v_NroDocIdentificacion.Equals(p.NroDocIdentidadCliente)).v_IdCliente
                            }).ToList();
                            var importeIrpes = tesoreriadetalleDtos.Sum(p => p.d_Importe);

                            tesoreriadetalleDtos.Add(new tesoreriadetalleDto
                            {
                                d_Importe = importeIrpes,
                                d_Cambio =
                                    cTesoreriaDto.i_IdMoneda.Equals(1)
                                        ? importeIrpes / cTesoreriaDto.d_TipoCambio ?? 1
                                        : importeIrpes * cTesoreriaDto.d_TipoCambio ?? 1,
                                i_IdTipoDocumentoRef = -1,
                                v_Naturaleza = "H",
                                v_NroCuenta = subListaIrpe.Key.nroCtaCaja,
                                v_NroDocumento = cTesoreriaDto.v_Correlativo,
                                v_NroDocumentoRef = string.Empty,
                                v_OrigenDestino = "",
                                v_Pedido = "",
                                t_Fecha = fechaBusqueda,
                                i_IdTipoDocumento = subListaIrpe.Key.documentoCaja,
                                i_IdCentroCostos = string.Empty,
                                i_IdCaja = 0,
                            });
                            #endregion

                            var totDebe = tesoreriadetalleDtos.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Importe ?? 0);
                            var totHaber = tesoreriadetalleDtos.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Importe ?? 0);
                            var totDebeC = tesoreriadetalleDtos.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Cambio ?? 0);
                            var totHaberC = tesoreriadetalleDtos.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Cambio ?? 0);
                            cTesoreriaDto.d_TotalDebe_Importe = totDebe;
                            cTesoreriaDto.d_TotalHaber_Importe = totHaber;
                            cTesoreriaDto.d_TotalDebe_Cambio = totDebeC;
                            cTesoreriaDto.d_TotalHaber_Cambio = totHaberC;
                            cTesoreriaDto.d_Diferencia_Importe = (totDebe - totHaber);
                            cTesoreriaDto.d_Diferencia_Cambio = (totDebeC - totHaberC);

                            InsertarTesoreriaExterna(ref pobjOperationResult, cTesoreriaDto, tesoreriadetalleDtos);
                            if (pobjOperationResult.Success == 0) return;
                        }

                        pobjOperationResult.Success = 1;
                        ts.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ExportarIrpesBL.InsertarTesoreriaExterna()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }
    }

    internal class AsientoIrpeDto
    {
        public string NroCuentaIrpe { get; set; }
        public string NroCuentaCaja { get; set; }
        public string Naturaleza { get; set; }
        public string CentroCosto { get; set; }
        public string IdCliente { get; set; }
        public int IdDocumento { get; set; }
        public string NroDocumento { get; set; }
        public decimal Importe { get; set; }
        public decimal TipoCambio { get; set; }
        public string Moneda { get; set; }
        public int TipoDocumentoCaja { get; set; }
        public string NroDocIdentidadCliente { get; set; }
    }
}
