using System;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;

namespace SAMBHS.Requerimientos.NBS
{
    /// <summary>
    /// Clase para sincronizar la dbf que el Ing. Velázquez dió para que lo consulte su sistema.
    /// </summary>
    public class DbfSincronizador
    {
        private string _rutaDbfCabecera;
        private string _rutaDbfDetalle;

        public enum TipoAccion
        {
            Venta,
            Cobranza
        }

        public class DatosCobranza
        {
            public int IdFormaPago { get; set; }
            public DateTime FechaCobranza { get; set; }
            public decimal MontoCobrado { get; set; }
            public string IdCobranzaDetalle { get; set; }
            public bool Anulada { get; set; }
        }

        /// <summary>
        /// Ruta donde se encuentra el dbf cabecera.
        /// </summary>
        public string RutaDbfCabecera
        {
            set { _rutaDbfCabecera = value; }
        }

        /// <summary>
        /// Ruta donde se encuentra el dbf detalle.
        /// </summary>
        public string RutaDbfDetalle
        {
            set { _rutaDbfDetalle = value; }
        }

        private bool RegistroYaExiste(string key)
        {
            try
            {
                var datosConnection = new OleDbConnection(string.Format("Provider=VFPOLEDB.1;Data Source={0};Exclusive=No", _rutaDbfCabecera));
                datosConnection.Open();
                var ventasEncontradas = new DataTable();

                if (datosConnection.State == ConnectionState.Open)
                {
                    var da = new OleDbDataAdapter();
                    var mySql = "select * from C_PAGOS where key = '" + key + "'";

                    var myQuery = new OleDbCommand(mySql, datosConnection);
                    da.SelectCommand = myQuery;
                    da.Fill(ventasEncontradas);
                    datosConnection.Close();
                }

                return ventasEncontradas.AsEnumerable().Any();
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Llamado desde venta ya sea cuando es nueva o cuando se edita, revisa si se debe insertar, modificar o dar de baja.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="idVenta"></param>
        /// <param name="tipoAccion"></param>
        /// <param name="datosCobranza"></param>
        public void ActualizarDatosVenta(ref OperationResult pobjOperationResult, string idVenta, TipoAccion tipoAccion, DatosCobranza datosCobranza = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_rutaDbfCabecera) || string.IsNullOrWhiteSpace(_rutaDbfDetalle))
                    throw new Exception("Por favor especifique las rutas de los dbfs correctamente!");

                ventaDto venta;
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    venta = dbContext.venta.FirstOrDefault(p => p.v_IdVenta.Equals(idVenta)).ToDTO();
                    if (venta == null) throw new Exception("La venta no fue encontrada!");
                }

                var condicionPago = tipoAccion == TipoAccion.Cobranza
                    ? TipoCondicion.CobranzaCredito
                    : venta.i_IdEstado == 1
                        ? venta.i_IdCondicionPago == 1
                            ? TipoCondicion.EmitidoCobrado
                            : TipoCondicion.EmitidoCredito
                        : TipoCondicion.Anulado;

                if (tipoAccion == TipoAccion.Cobranza && datosCobranza != null)
                {
                    if (!datosCobranza.Anulada)
                    {
                        var esModificacion = RegistroYaExiste(datosCobranza.IdCobranzaDetalle);
                        if (esModificacion) condicionPago = TipoCondicion.Modificado;
                    }
                    else
                        condicionPago = TipoCondicion.Anulado;

                    InsertarVenta(ref pobjOperationResult, venta, (int)condicionPago, datosCobranza.IdFormaPago,
                        datosCobranza.FechaCobranza, datosCobranza.MontoCobrado, datosCobranza.IdCobranzaDetalle);
                }
                else
                {
                    var esModificacion = RegistroYaExiste(venta.v_IdVenta);
                    if (esModificacion) condicionPago = TipoCondicion.Modificado;
                    InsertarVenta(ref pobjOperationResult, venta, (int)condicionPago, venta.i_IdCondicionPago == 1 ? 1 : -1);
                }
                if (pobjOperationResult.Success == 0) throw new Exception(pobjOperationResult.ErrorMessage);
                pobjOperationResult.Success = 1;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
            }
        }

        /// <summary>
        /// Elimina del dbf una venta eliminada.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstrIdCobranza"></param>
        public void EliminarCobranza(ref OperationResult pobjOperationResult, string pstrIdCobranza)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var cobranza = dbContext.cobranza.FirstOrDefault(p => p.v_IdCobranza.Equals(pstrIdCobranza));
                    if (cobranza == null)
                        throw new Exception("La cobranza realizada no se encuentra");

                    var cobranzaDetalles = dbContext.cobranzadetalle.Where(p => p.v_IdCobranza.Equals(pstrIdCobranza)).ToList();

                    foreach (var cobranzaDetalle in cobranzaDetalles)
                    {
                        EliminarCobranzaDetalle(ref pobjOperationResult, cobranzaDetalle.v_IdCobranzaDetalle,
                            cobranza.t_FechaRegistro.Value);
                        if (pobjOperationResult.Success == 0) return;
                    }
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
            }
        }


        public void EliminarCobranzaDetalle(ref OperationResult pobjOperationResult, string pstrIdCobranzaDetalle, DateTime fechaCobranza)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var cobranzadetalle = dbContext.cobranzadetalle.FirstOrDefault(p => p.v_IdCobranzaDetalle.Equals(pstrIdCobranzaDetalle));
                    if (cobranzadetalle == null)
                        throw new Exception("La cobranza detalle realizada no se encuentra");

                    var ventaDto = dbContext.venta.FirstOrDefault(p => p.v_IdVenta.Equals(cobranzadetalle.v_IdVenta)).ToDTO();
                    //Sólo se manda un registro al dbf si se elimina la cobranza de una venta al contado.
                    if (ventaDto.i_IdCondicionPago != 1)
                    {
                        InsertarVenta(ref pobjOperationResult, ventaDto, (int)TipoCondicion.Anulado, cobranzadetalle.i_IdFormaPago ?? 1,
                                        fechaCobranza, cobranzadetalle.d_ImporteSoles ?? 0, cobranzadetalle.v_IdCobranzaDetalle);
                        if (pobjOperationResult.Success == 0) return;
                    }

                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
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
        private void InsertarVenta(ref OperationResult pobjOperationResult, ventaDto objVentaDto, int condicionPago, int idFormaPago, DateTime? fechaCancelacion = null, decimal montoPagado = 0, string idCobranzaDetalle = "")
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var datosConnection =
                        new OleDbConnection(string.Format("Provider=VFPOLEDB.1;Data Source={0};Exclusive=No", _rutaDbfCabecera));

                    datosConnection.Open();

                    if (datosConnection.State == ConnectionState.Open)
                    {
                        var ventaKardex = dbContext.nbs_ventakardex.Where(p => p.v_IdVenta.Equals(objVentaDto.v_IdVenta)).ToList();
                        var esNcr = (objVentaDto.i_IdTipoDocumento ?? -1) == 7;

                        if (!esNcr)
                        {
                            #region Si NO es Nota de crédito
                            if (ventaKardex.Any())
                            {
                                var nbsVentakardex = ventaKardex.FirstOrDefault();
                                if (nbsVentakardex != null)
                                {
                                    #region Inserta la data en el dbf
                                    {
                                        var nroKardex = !objVentaDto.v_IdTipoKardex.Trim().Equals("V")
                                            ? nbsVentakardex.v_NroKardex.Trim()
                                            : "";
                                        var montoKardex = nbsVentakardex.d_Monto ?? 0;

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
                                        else if (objVentaDto.i_IdCondicionPago != 1)
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

                                    if (ventaKardex.Count > 1 && fechaCancelacion == null && condicionPago != (int)TipoCondicion.Anulado)
                                    {
                                        foreach (var ventaK in ventaKardex)
                                        {
                                            #region Inserta la data en el dbf

                                            var tipoKardex = ventaK.v_TipoKardex.Trim();
                                            var nroKardex = ventaK.v_NroKardex.Trim();
                                            var montoKardex = ventaK.d_Monto;
                                            var estado = ventaK.i_Eliminado != 1 ? ObtenerEstadoVentaKardex(ventaK.v_IdVentaKardex) : EstadoDetalle.Eliminado;

                                            if (estado != EstadoDetalle.Eliminado)
                                            {
                                                var mySql = @"insert into D_PAGOS " +
                                                       "(tipodoc, serie, documento, tipodet, numdet, monto, procesado, keydet, keycab, estado, fechareg) values " +
                                                       "('" + (objVentaDto.i_IdTipoDocumento ?? -1) + "', '" +
                                                       int.Parse(objVentaDto.v_SerieDocumento) + "','" +
                                                       int.Parse(objVentaDto.v_CorrelativoDocumento) + "','" +
                                                       tipoKardex + "', '" + nroKardex + "', " +
                                                       montoKardex + ", '0', '" + ventaK.v_IdVentaKardex + "', '" + ventaK.v_IdVenta + "', " + (int)estado + ", ?)";

                                                var myQuery = new OleDbCommand(mySql, datosConnection);
                                                myQuery.Parameters.Add("@p1", OleDbType.Date).Value = DateTime.Now;
                                                myQuery.ExecuteNonQuery();
                                            }
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

                                const TipoPago formaPago = TipoPago.NotaCredito;

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
                }
                pobjOperationResult.Success = 1;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
            }
        }

        private EstadoDetalle ObtenerEstadoVentaKardex(string keydet)
        {
            try
            {
                var datosConnection = new OleDbConnection(string.Format("Provider=VFPOLEDB.1;Data Source={0};Exclusive=No", _rutaDbfCabecera));
                datosConnection.Open();
                var ventasEncontradas = new DataTable();

                if (datosConnection.State == ConnectionState.Open)
                {
                    var da = new OleDbDataAdapter();
                    var mySql = "select * from D_PAGOS where keydet = '" + keydet + "' and estado <> 3";

                    var myQuery = new OleDbCommand(mySql, datosConnection);
                    da.SelectCommand = myQuery;
                    da.Fill(ventasEncontradas);
                    datosConnection.Close();
                }

                var result = ventasEncontradas.AsEnumerable().Any() ? EstadoDetalle.Editado : EstadoDetalle.Nuevo;
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Indica el estado con el que se insertará el detalle del comprobante.
        /// </summary>
        private enum EstadoDetalle
        {
            Nuevo = 1,
            Editado = 2,
            Eliminado = 3
        }

        /// <summary>
        /// Obtiene los tipos de pagos equivalentes al sistema notarial
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static TipoPago ObtenerFormaPago(int id)
        {
            switch (id)
            {
                case 1:
                    return TipoPago.Efectivo;

                case 2:
                    return TipoPago.TarjetaCredito;

                case 3:
                    return TipoPago.TarjetaCredito;

                case 4:
                    return TipoPago.NotaCredito;

                case 10:
                    return TipoPago.Efectivo;
                case 9:
                    return TipoPago.DepositoCuenta;
                case 12:
                    return TipoPago.DepositoCuenta;
                case 13:
                    return TipoPago.DepositoCuenta;
                case 14:
                    return TipoPago.DepositoCuenta;
                case 15:
                    return TipoPago.DepositoCuenta;

                default:
                    return TipoPago.Credito;

            }
        }

        /// <summary>
        /// Enums para especificar el tipo de pago en el dbf.
        /// </summary>
        public enum TipoCondicion
        {
            EmitidoCobrado = 1,
            EmitidoCredito = 2,
            CobranzaCredito = 3,
            Anulado = 4,
            Modificado = 5
        }

        /// <summary>
        /// Enumera los tipos de pagos dados por el ingeniero de la notaria.
        /// </summary>
        public enum TipoPago
        {
            Efectivo = 1,
            Credito = 2,
            Gratuito = 3,
            Cheque = 4,
            NotaCredito = 5,
            Canje = 6,
            TarjetaCredito = 7,
            DepositoCuenta = 8,
            Detraccion = 9,
            DevolucionIrpe = 0
        }
    }
}