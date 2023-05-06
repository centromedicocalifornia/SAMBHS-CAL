using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;

namespace SAMBHS.Requerimientos.NBS
{
    public class RegenerarDbf
    {
        private readonly BackgroundWorker _backgroundWorker = new BackgroundWorker();
        public delegate void Estado(int index);
        public delegate void Error(string error);
        public delegate void ProcesoTerminado();
        public event Estado EstadoEvent;
        public event Error ErrorEvent;
        public event ProcesoTerminado ProcesoTerminadoEvent;
        public string RutaDbfCabecera { get; set; }
        public string RutaDbfDetalle { get; set; }
        private Dictionary<string, ventaDto> _dataVentas;
        private Dictionary<string, IGrouping<string, cobranzadetalle>> _dataDetalleCobranzas;
        private Dictionary<string, cobranzaDto> _dataCobranzas;
        private Dictionary<string, IGrouping<string, nbs_ventakardex>> _dataVentaKardex;

        public void ComenzarAsync()
        {

            _backgroundWorker.DoWork += _backgroundWorker_DoWork;
            _backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;
            _backgroundWorker.RunWorkerAsync();
        }

        private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (ProcesoTerminadoEvent != null) ProcesoTerminadoEvent();
        }

        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (EstadoEvent != null) EstadoEvent(0);
                _dataVentas = ObtenerVentasSistema;
                _dataDetalleCobranzas = ObtenerCobranzasDetalleSistema;
                _dataCobranzas = ObtenerCobranzasSistema;
                _dataVentaKardex = ObtenerVentaKardex;
                EliminarVentas();
                var ids = _dataVentas.Select(p => p.Key).ToList();
                var objOperationResult = new OperationResult();
                var counter = 1;
                foreach (var id in ids.AsParallel())
                {
                    ventaDto venta;
                    _dataVentas.TryGetValue(id, out venta);

                    ActualizarDatosVenta(ref objOperationResult, id, DbfSincronizador.TipoAccion.Venta);
                    if (objOperationResult.Success == 0)
                        throw new Exception(objOperationResult.ErrorMessage);

                    if (venta != null && venta.i_IdCondicionPago != 1)
                    {
                        IGrouping<string, cobranzadetalle> cobranzas;
                        _dataDetalleCobranzas.TryGetValue(id, out cobranzas);
                        if (cobranzas != null)
                        {
                            foreach (var cobranzadetalle in cobranzas)
                            {
                                cobranzaDto cobranza;
                                if (_dataCobranzas.TryGetValue(cobranzadetalle.v_IdCobranza, out cobranza))
                                {
                                    ActualizarDatosVenta(ref objOperationResult, id, DbfSincronizador.TipoAccion.Cobranza,
                                      new DbfSincronizador.DatosCobranza
                                      {
                                          FechaCobranza = cobranza.t_FechaRegistro ?? DateTime.Now,
                                          IdFormaPago = cobranzadetalle.i_IdFormaPago ?? -1,
                                          MontoCobrado = cobranzadetalle.d_ImporteSoles ?? 0M ,
                                          IdCobranzaDetalle = cobranzadetalle.v_IdCobranzaDetalle
                                      });
                                    if (objOperationResult.Success == 0)
                                        throw new Exception(objOperationResult.ErrorMessage);
                                }
                            }
                        }
                    }

                    if (EstadoEvent != null) EstadoEvent((counter * 100) / ids.Count);
                    counter++;
                }
            }
            catch (Exception ex)
            {
                if (ErrorEvent != null) ErrorEvent(ex.Message + '\n' + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
            }
        }

        protected virtual void OnEstadoEvent(int estado)
        {
            var handler = EstadoEvent;
            if (handler != null) handler(estado);
        }

        protected virtual void OnErrorEvent(string error)
        {
            var handler = ErrorEvent;
            if (handler != null) handler(error);
        }

        private static Dictionary<string, ventaDto> ObtenerVentasSistema
        {
            get
            {
                try
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var ventas = (from n in dbContext.venta
                                      where n.v_SerieDocumento != null && n.v_CorrelativoDocumento != null &&
                                      dbContext.nbs_ventakardex.Any(p => p.v_IdVenta.Equals(n.v_IdVenta))
                                      orderby n.t_FechaRegistro
                                      select n).ToDTOs();

                        return ventas.ToDictionary(k => k.v_IdVenta, o => o);
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        private static Dictionary<string, cobranzaDto> ObtenerCobranzasSistema
        {
            get
            {
                try
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var cobranza =
                            dbContext.cobranza.Where(p => p.i_Eliminado == 0)
                                .ToDTOs()
                                .ToDictionary(k => k.v_IdCobranza, o => o);
                        return cobranza;
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        private static Dictionary<string, IGrouping<string, cobranzadetalle>> ObtenerCobranzasDetalleSistema
        {
            get
            {
                try
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var cobranzas =
                            dbContext.cobranzadetalle.Where(p => p.i_Eliminado == 0).ToList()
                                .GroupBy(g => g.v_IdVenta)
                                .ToDictionary(k => k.Key, g => g);
                        return cobranzas;
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        private static Dictionary<string, IGrouping<string, nbs_ventakardex>> ObtenerVentaKardex
        {
            get
            {
                try
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var kardex = dbContext.nbs_ventakardex.Where(p => p.i_Eliminado == 0).ToList()
                                .GroupBy(g => g.v_IdVenta)
                                .ToDictionary(k => k.Key, g => g);
                        return kardex;
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        private void EliminarVentas()
        {
            try
            {
                var datosConnection =
                    new OleDbConnection(string.Format("Provider=VFPOLEDB.1;Data Source={0};Exclusive=No", RutaDbfCabecera));

                datosConnection.Open();

                if (datosConnection.State == ConnectionState.Open)
                {
                    var t = datosConnection.BeginTransaction();
                    const string mySql = "delete from C_PAGOS";

                    const string mySqlD = "delete from D_PAGOS";

                    var myQuery = new OleDbCommand(mySql, datosConnection, t);
                    myQuery.ExecuteNonQuery();

                    var myQueryD = new OleDbCommand(mySqlD, datosConnection, t);
                    myQueryD.ExecuteNonQuery();
                    t.Commit();
                    datosConnection.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Llamado desde venta ya sea cuando es nueva o cuando se edita, revisa si se debe insertar, modificar o dar de baja.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="idVenta"></param>
        /// <param name="tipoAccion"></param>
        /// <param name="datosCobranza"></param>
        private void ActualizarDatosVenta(ref OperationResult pobjOperationResult, string idVenta, 
            DbfSincronizador.TipoAccion tipoAccion, DbfSincronizador.DatosCobranza datosCobranza = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(RutaDbfCabecera) || string.IsNullOrWhiteSpace(RutaDbfDetalle))
                    throw new Exception("Por favor especifique las rutas de los dbfs correctamente!");

                ventaDto venta;
                _dataVentas.TryGetValue(idVenta, out venta);
                if (venta == null) throw new Exception("La venta no fue encontrada!");

                var condicionPago = tipoAccion == DbfSincronizador.TipoAccion.Cobranza
                    ? DbfSincronizador.TipoCondicion.CobranzaCredito
                    : venta.i_IdEstado == 1
                        ? venta.i_IdCondicionPago == 1
                            ? DbfSincronizador.TipoCondicion.EmitidoCobrado
                            : DbfSincronizador.TipoCondicion.EmitidoCredito
                        : DbfSincronizador.TipoCondicion.Anulado;

                if (tipoAccion == DbfSincronizador.TipoAccion.Cobranza && datosCobranza != null)
                {
                    InsertarVenta(ref pobjOperationResult, venta, (int)condicionPago, datosCobranza.IdFormaPago,
                        datosCobranza.FechaCobranza, datosCobranza.MontoCobrado, datosCobranza.IdCobranzaDetalle);
                }
                else
                {
                    InsertarVenta(ref pobjOperationResult, venta, (int)condicionPago, venta.i_IdCondicionPago == 1 ? 1 : -1);
                }
                if (pobjOperationResult.Success == 0) throw new Exception(pobjOperationResult.ErrorMessage);
                pobjOperationResult.Success = 1;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.AdditionalInformation = idVenta;
            }
        }

        /// <summary>
        /// Obtiene los tipos de pagos equivalentes al sistema notarial
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static DbfSincronizador.TipoPago ObtenerFormaPago(int id)
        {
            switch (id)
            {
                case 1:
                    return DbfSincronizador.TipoPago.Efectivo;

                case 2:
                    return DbfSincronizador.TipoPago.TarjetaCredito;

                case 3:
                    return DbfSincronizador.TipoPago.TarjetaCredito;

                case 4:
                    return DbfSincronizador.TipoPago.NotaCredito;

                case 10:
                    return DbfSincronizador.TipoPago.Efectivo;
                case 9:
                    return DbfSincronizador.TipoPago.DepositoCuenta;
                case 12:
                    return DbfSincronizador.TipoPago.DepositoCuenta;
                case 13:
                    return DbfSincronizador.TipoPago.DepositoCuenta;
                case 14:
                    return DbfSincronizador.TipoPago.DepositoCuenta;
                case 15:
                    return DbfSincronizador.TipoPago.DepositoCuenta;

                default:
                    return DbfSincronizador.TipoPago.Credito;

            }
        }

        /// <summary>
        /// Inserta la venta al dbf
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="objVentaDto"></param>
        /// <param name="condicionPago"></param>
        /// <param name="idFormaPago"></param>
        /// <param name="fechaCancelacion"></param>
        /// <param name="montoPagado"></param>
        private void InsertarVenta(ref OperationResult pobjOperationResult, ventaDto objVentaDto,
            int condicionPago, int idFormaPago, DateTime? fechaCancelacion = null, decimal montoPagado = 0, string idCobranzaDetalle = "")
        {
            try
            {
                var datosConnection =
                    new OleDbConnection(string.Format("Provider=VFPOLEDB.1;Data Source={0};Exclusive=No", RutaDbfCabecera));

                datosConnection.Open();

                if (datosConnection.State == ConnectionState.Open)
                {
                    IGrouping<string, nbs_ventakardex> ventaKardex;
                    _dataVentaKardex.TryGetValue(objVentaDto.v_IdVenta, out ventaKardex);
                    var esNcr = (objVentaDto.i_IdTipoDocumento ?? -1) == 7;

                    if (!esNcr)
                    {
                        #region Si NO es Nota de crédito
                        if (ventaKardex != null && ventaKardex.Any())
                        {
                            var nbsVentakardex = ventaKardex.FirstOrDefault();
                            if (nbsVentakardex != null)
                            {
                                #region Inserta la data en el dbf
                                {
                                    var nroKardex = !objVentaDto.v_IdTipoKardex.Trim().Equals("V")
                                        ? nbsVentakardex.v_NroKardex.Trim()
                                        : "";

                                    var montoKardex =  objVentaDto.d_Total ?? 0;

                                    var formaPago = ObtenerFormaPago(idFormaPago);

                                    string mySql;
                                    if (fechaCancelacion == null)
                                    {
                                        mySql = @"insert into C_PAGOS " +
                                                "(tipopago, tipodoc, serie, documento, formapago, tipodet, numdet, monto, procesado, " +
                                                "tip_docref, t_fchventa, key, t_fchreg) values " +
                                                "(" + condicionPago + ", '" +
                                                objVentaDto.i_IdTipoDocumento +
                                                "', '" +
                                                int.Parse(objVentaDto.v_SerieDocumento) + "','" +
                                                int.Parse(objVentaDto.v_CorrelativoDocumento) +
                                                "'," + (int)formaPago + ", '" + objVentaDto.v_IdTipoKardex +
                                                "', '" + nroKardex + "', " +
                                                montoKardex +
                                                ", '0','" + (objVentaDto.i_IdTipoDocumentoRef ?? -1) +
                                                "', ?, '" + objVentaDto.v_IdVenta + "', ?)";

                                        var myQuery = new OleDbCommand(mySql, datosConnection);
                                        myQuery.Parameters.Add("@p1", OleDbType.Date).Value =
                                            objVentaDto.t_FechaRegistro ??
                                            DateTime.Now;
                                        myQuery.Parameters.Add("@p2", OleDbType.Date).Value = DateTime.Now;
                                        myQuery.ExecuteNonQuery();
                                    }
                                    else
                                    {
                                        mySql = @"insert into C_PAGOS " +
                                                "(tipopago, tipodoc, serie, documento, formapago, tipodet, numdet, monto, procesado, " +
                                                "tip_docref, t_fchventa, t_fchpago, key, t_fchreg) values " +
                                                "(" + condicionPago + ", '" +
                                                objVentaDto.i_IdTipoDocumento +
                                                "', '" +
                                                int.Parse(objVentaDto.v_SerieDocumento) + "','" +
                                                int.Parse(objVentaDto.v_CorrelativoDocumento) +
                                                "'," + (int)formaPago + ", '" + objVentaDto.v_IdTipoKardex +
                                                "', '" + nroKardex + "', " +
                                                montoPagado +
                                                ", '0','" + (objVentaDto.i_IdTipoDocumentoRef ?? -1) +
                                                "', ?, ?, '" + idCobranzaDetalle + "', ?)";

                                        var myQuery = new OleDbCommand(mySql, datosConnection);
                                        myQuery.Parameters.Add("@p1", OleDbType.Date).Value =
                                            objVentaDto.t_FechaRegistro ??
                                            DateTime.Now;
                                        myQuery.Parameters.Add("@p2", OleDbType.Date).Value =
                                            fechaCancelacion.Value;
                                        myQuery.Parameters.Add("@p3", OleDbType.Date).Value = DateTime.Now;
                                        myQuery.ExecuteNonQuery();
                                    }
                                }

                                #endregion

                                if (ventaKardex.Count() > 1 && fechaCancelacion == null && condicionPago != (int)DbfSincronizador.TipoCondicion.Anulado)
                                {
                                    foreach (var ventaK in ventaKardex)
                                    {
                                        #region Inserta la data en el dbf

                                        var tipoKardex = ventaK.v_TipoKardex.Trim();
                                        var nroKardex = ventaK.v_NroKardex.Trim();
                                        var montoKardex = ventaK.d_Monto;

                                        var mySql = @"insert into D_PAGOS " +
                                                    "(tipodoc, serie, documento, tipodet, numdet, monto, procesado, keydet, keycab, estado, fechareg) values " +
                                                    "('" + (objVentaDto.i_IdTipoDocumento ?? -1) + "', '" +
                                                    int.Parse(objVentaDto.v_SerieDocumento) + "','" +
                                                    int.Parse(objVentaDto.v_CorrelativoDocumento) + "','" +
                                                    tipoKardex + "', '" + nroKardex + "', " +
                                                    montoKardex + ", '0', '" + ventaK.v_IdVentaKardex + "', '" + ventaK.v_IdVenta + "', " + 1 + ", ?)";

                                        var myQuery = new OleDbCommand(mySql, datosConnection);
                                        myQuery.Parameters.Add("@p1", OleDbType.Date).Value = DateTime.Now;
                                        myQuery.ExecuteNonQuery();

                                        #endregion
                                    }
                                }
                            }
                            datosConnection.Close();
                        }
                        #endregion
                    }
                    else
                    {
                        #region Si ES nota de crédito
                        {
                            var montoNcr = objVentaDto.d_Total ?? 0;

                            const DbfSincronizador.TipoPago formaPago = DbfSincronizador.TipoPago.NotaCredito;

                            var mySql = @"insert into C_PAGOS " +
                                                "(tipopago, tipodoc, serie, documento, formapago, tipodet, numdet, monto, procesado, " +
                                                "tip_docref, t_fchventa, key, t_fchreg) values " +
                                                "(" + condicionPago + ", '" +
                                                objVentaDto.i_IdTipoDocumentoRef +
                                                "', '" +
                                                int.Parse(objVentaDto.v_SerieDocumentoRef) + "','" +
                                                int.Parse(objVentaDto.v_CorrelativoDocumentoRef) +
                                                "'," + (int)formaPago + ", '" + objVentaDto.v_IdTipoKardex +
                                                "', '', " + montoNcr + ", '0','-1', ?, '" + objVentaDto.v_IdVenta + "', ?)";

                            var myQuery = new OleDbCommand(mySql, datosConnection);
                            myQuery.Parameters.Add("@p1", OleDbType.Date).Value =
                                objVentaDto.t_FechaRegistro ??
                                DateTime.Now;
                            myQuery.Parameters.Add("@p2", OleDbType.Date).Value = DateTime.Now;

                            myQuery.ExecuteNonQuery();
                        }
                        #endregion
                    }

                }

                pobjOperationResult.Success = 1;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
            }
        }

        protected virtual void OnProcesoTerminadoEvent()
        {
            var handler = ProcesoTerminadoEvent;
            if (handler != null) handler();
        }
    }
}
