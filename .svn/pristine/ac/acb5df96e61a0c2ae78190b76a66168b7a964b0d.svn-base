using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Dapper;
using Devart.Data.PostgreSql;
using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;

namespace SAMBHS.Venta.BL
{
    public class VentasUtils
    {
        private readonly BackgroundWorker _backgroundWorker;
        public delegate void Error(string mensaje);
        public delegate void Terminado(string mensaje);
        public delegate void Estado(string estado);
        public event Error ErrorEvent;
        public event Terminado TerminadoEvent;
        public event Estado EstadoEvent;

        private string Periodo { get; set; }
        private string Mes { get; set; }
        private int IdTipoDocumento { get; set; }
        private string Serie { get; set; }

        public VentasUtils()
        {
            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.DoWork += BackgroundWorkerOnDoWork;
            _backgroundWorker.RunWorkerCompleted += BackgroundWorkerOnRunWorkerCompleted;
        }

        private void BackgroundWorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
        {
            if (TerminadoEvent != null) TerminadoEvent("El proceso termino correctamente!");
        }

        private void BackgroundWorkerOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    if (EstadoEvent != null) EstadoEvent("Buscando...");
                    var ventas = (from n in dbContext.venta
                                  where n.i_Eliminado == 0 && n.v_Periodo.Equals(Periodo) && n.v_Mes.Equals(Mes)
                                          && (Serie == "" || n.v_SerieDocumento.Equals(Serie))
                                          && (IdTipoDocumento == 0 || n.i_IdTipoDocumento.Value == IdTipoDocumento)
                                  select n).AsEnumerable();

                    var listadoVentas = ventas as IList<venta> ?? ventas.ToList();

                    if (listadoVentas.Any())
                    {
                        var idsVentas = listadoVentas.Select(p => p.v_IdVenta).ToArray();
                        var ventaDetalles = dbContext.ventadetalle.ToList().Where(p => idsVentas.Contains(p.v_IdVenta) && p.i_Eliminado == 0).ToList();

                        var tieneCobranzas = dbContext.cobranzadetalle.Count(p => p.i_Eliminado == 0 && idsVentas.Contains(p.v_IdVenta)) > 0;

                        if (tieneCobranzas) throw new Exception("Una o más ventas por eliminar tiene cobranzas administrativas!. El proceso falló.");

                        var cobranzasPendientes = dbContext.cobranzapendiente.ToList().Where(p => p.i_Eliminado == 0 && idsVentas.Contains(p.v_IdVenta)).ToList();
                        var diarios = dbContext.diario.ToList().Where(p => idsVentas.Contains(p.v_IdDocumentoReferencia) && p.i_Eliminado == 0).ToList();
                        var idDiarios = diarios.Select(p => p.v_IdDiario).ToArray();
                        var diarioDetalles = dbContext.diariodetalle.ToList().Where(p => idDiarios.Contains(p.v_IdDiario) && p.i_Eliminado == 0).ToList();
                        if (EstadoEvent != null) EstadoEvent("Procesando...");
                        foreach (var venta in listadoVentas)
                        {
                            venta.v_MotivoEliminacion = "Eliminación por Utilitario de Ventas";
                            venta.i_Eliminado = 1;
                            venta.t_ActualizaFecha = DateTime.Now;
                            venta.i_ActualizaIdUsuario = Globals.ClientSession.i_SystemUserId;
                            dbContext.venta.ApplyCurrentValues(venta);
                        }

                        foreach (var ventaDetalle in ventaDetalles)
                        {
                            ventaDetalle.i_Eliminado = 1;
                            ventaDetalle.t_ActualizaFecha = DateTime.Now;
                            ventaDetalle.i_ActualizaIdUsuario = Globals.ClientSession.i_SystemUserId;
                        }

                        foreach (var cobranzaPendiente in cobranzasPendientes)
                        {
                            cobranzaPendiente.i_Eliminado = 1;
                            cobranzaPendiente.t_ActualizaFecha = DateTime.Now;
                            cobranzaPendiente.i_ActualizaIdUsuario = Globals.ClientSession.i_SystemUserId;
                        }

                        foreach (var diario in diarios)
                        {
                            diario.v_MotivoEliminacion = "Eliminación por Utilitario de Ventas";
                            diario.i_Eliminado = 1;
                            diario.t_ActualizaFecha = DateTime.Now;
                            diario.i_ActualizaIdUsuario = Globals.ClientSession.i_SystemUserId;
                        }

                        foreach (var diarioDetalle in diarioDetalles)
                        {
                            diarioDetalle.i_Eliminado = 1;
                            diarioDetalle.t_ActualizaFecha = DateTime.Now;
                            diarioDetalle.i_ActualizaIdUsuario = Globals.ClientSession.i_SystemUserId;
                        }
                        if (EstadoEvent != null) EstadoEvent("Guardando...");
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ErrorEvent != null) ErrorEvent(ex.ToString());
            }
        }

        public void EliminarVentas(string periodo, string mes, int idTipoDocumento, string serie)
        {
            Periodo = periodo;
            Mes = mes;
            IdTipoDocumento = idTipoDocumento;
            Serie = serie;
            _backgroundWorker.RunWorkerAsync();
        }

        protected virtual void OnErrorEvent(string mensaje)
        {
            var handler = ErrorEvent;
            if (handler != null) handler(mensaje);
        }

        protected virtual void OnTerminadoEvent(string mensaje)
        {
            var handler = TerminadoEvent;
            if (handler != null) handler(mensaje);
        }
    }

    public class BulkInsertVentasBl
    {
        private readonly BackgroundWorker _backgroundWorker;
        public delegate void Error(string mensaje);
        public delegate void Terminado(string mensaje);
        public delegate void Estado(string estado);
        public event Error ErrorEvent;
        public event Terminado TerminadoEvent;
        public event Estado EstadoEvent;
        private List<ImportacionRegistroVentaDto> _ventasImportada;
        private List<ImportacionRegistroVentaCobranzaDto> _ventasCobranzas;
        public Dictionary<string, KeyValueDTO> ListaClientes { get; set; }
        private static string SufijoString
        {
            get { return Globals.TipoMotor == TipoMotorBD.MSSQLServer ? "N" : "E"; }
        }

        public BulkInsertVentasBl()
        {
            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.DoWork += _backgroundWorker_DoWork;
        }

        public void Comenzar(List<ImportacionRegistroVentaDto> data, List<ImportacionRegistroVentaCobranzaDto> cobranzas)
        {
            _ventasImportada = data;
            _ventasCobranzas = cobranzas;
            _backgroundWorker.RunWorkerAsync();
        }

        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var sw = new Stopwatch();
                sw.Start();
                var setFormatoFecha = Globals.TipoMotor == TipoMotorBD.MSSQLServer ? "set dateformat 'dmy';" : "SET datestyle = dmy;";
                var queryClientes = ObtenerClienteQuery(_ventasImportada);
                var queryVentas = ObtenerVentaQuery(_ventasImportada);
                var queryVentasDetalle = ObtenerVentaDetalleQuery(_ventasImportada);
                var queryCobranzaPendiente = ObtenerCobranzaPendienteQuery(_ventasImportada);
                var queryDiarios = ObtenerLibroDiarioQuery(_ventasImportada);
                var queryCobranzas = ObtenerTesoreriaQuery(_ventasImportada, _ventasCobranzas);

                var querys = queryClientes.Concat(queryVentas.Concat(queryVentasDetalle).Concat(queryCobranzaPendiente))
                    .Concat(queryDiarios).Concat(queryCobranzas).ToList();

                querys.Insert(0, setFormatoFecha);

                BulkInsertData(querys);
                sw.Stop();

                if (TerminadoEvent != null)
                    TerminadoEvent(string.Format("Proceso terminado correctamente!\nTiempo transcurrido: {0}:{1} min.",
                        Math.Floor(sw.Elapsed.TotalMinutes), sw.Elapsed.ToString("ss")));
            }
            catch (Exception exception)
            {
                if (ErrorEvent != null) ErrorEvent(exception.ToString());
            }
        }

        public static IEnumerable<string> VentasKeysExistentes
        {
            get
            {
                try
                {
                    switch (Globals.TipoMotor)
                    {
                        case TipoMotorBD.MSSQLServer:
                            using (var cnx = new SqlConnection(Globals.CadenaConexion))
                            {
                                if (cnx.State == ConnectionState.Closed) cnx.Open();
                                try
                                {
                                    var query = "select ltrim(rtrim(STR(\"i_IdTipoDocumento\") + \"v_SerieDocumento\" + \"v_CorrelativoDocumento\")) as \"KeyVenta\" from venta where \"i_Eliminado\" = 0";
                                    return cnx.Query<string>(query).ToList();
                                }
                                catch
                                {
                                    return null;
                                }
                            }

                        case TipoMotorBD.PostgreSQL:
                            using (var cnx = new PgSqlConnection(Globals.CadenaConexion))
                            {
                                if (cnx.State == ConnectionState.Closed) cnx.Open();
                                try
                                {
                                    var query = "select \"i_IdTipoDocumento\" || \"v_SerieDocumento\" || \"v_CorrelativoDocumento\" as \"KeyVenta\" from venta where \"i_Eliminado\" = 0";
                                    return cnx.Query<string>(query).ToList();
                                }
                                catch
                                {
                                    return null;
                                }
                            }

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        public static IEnumerable<string> CuentasExistentes
        {
            get
            {
                try
                {
                    switch (Globals.TipoMotor)
                    {
                        case TipoMotorBD.MSSQLServer:
                            using (var cnx = new SqlConnection(Globals.CadenaConexion))
                            {
                                if (cnx.State == ConnectionState.Closed) cnx.Open();
                                try
                                {
                                    var query = "select ltrim(rtrim(STR(\"i_IdTipoDocumento\") + \"v_SerieDocumento\" + \"v_CorrelativoDocumento\")) as \"KeyVenta\" from venta where \"i_Eliminado\" = 0";
                                    return cnx.Query<string>(query).ToList();
                                }
                                catch
                                {
                                    return null;
                                }
                            }

                        case TipoMotorBD.PostgreSQL:
                            using (var cnx = new PgSqlConnection(Globals.CadenaConexion))
                            {
                                if (cnx.State == ConnectionState.Closed) cnx.Open();
                                try
                                {
                                    var query = "select \"i_IdTipoDocumento\" || \"v_SerieDocumento\" || \"v_CorrelativoDocumento\" as \"KeyVenta\" from venta where \"i_Eliminado\" = 0";
                                    return cnx.Query<string>(query).ToList();
                                }
                                catch
                                {
                                    return null;
                                }
                            }

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        public ventaimportaciondataconfig Configuraciones
        {
            get
            {
                try
                {
                    switch (Globals.TipoMotor)
                    {
                        case TipoMotorBD.MSSQLServer:
                            using (var cnx = new SqlConnection(Globals.CadenaConexion))
                            {
                                if (cnx.State == ConnectionState.Closed) cnx.Open();
                                try
                                {
                                    var query = "select* from ventaimportaciondataconfig";
                                    return cnx.Query<ventaimportaciondataconfig>(query).FirstOrDefault();
                                }
                                catch
                                {
                                    return null;
                                }
                            }

                        case TipoMotorBD.PostgreSQL:
                            using (var cnx = new PgSqlConnection(Globals.CadenaConexion))
                            {
                                if (cnx.State == ConnectionState.Closed) cnx.Open();
                                try
                                {
                                    var query = "select* from ventaimportaciondataconfig";
                                    return cnx.Query<ventaimportaciondataconfig>(query).FirstOrDefault();
                                }
                                catch
                                {
                                    return null;
                                }
                            }

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public static IEnumerable<administracionconceptos> Conceptos
        {
            get
            {
                try
                {
                    var periodo = Globals.ClientSession.i_Periodo.ToString();
                    switch (Globals.TipoMotor)
                    {
                        case TipoMotorBD.MSSQLServer:
                            using (var cnx = new SqlConnection(Globals.CadenaConexion))
                            {
                                if (cnx.State == ConnectionState.Closed) cnx.Open();
                                try
                                {
                                    var query = "select* from administracionconceptos where \"v_Periodo\" = '" + periodo + "' and \"i_Eliminado\" = 0";
                                    return cnx.Query<administracionconceptos>(query).ToList();
                                }
                                catch
                                {
                                    return null;
                                }
                            }

                        case TipoMotorBD.PostgreSQL:
                            using (var cnx = new PgSqlConnection(Globals.CadenaConexion))
                            {
                                if (cnx.State == ConnectionState.Closed) cnx.Open();
                                try
                                {
                                    var query = "select* from administracionconceptos where \"v_Periodo\" = '" + periodo + "' and \"i_Eliminado\" = 0";
                                    return cnx.Query<administracionconceptos>(query).ToList();
                                }
                                catch
                                {
                                    return null;
                                }
                            }

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        private void BulkInsertData(IList<string> querys)
        {
            try
            {
                var count = querys.Count;
                File.WriteAllLines("D:\\querys.txt", querys);

                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entityConnection = dbContext.Connection as System.Data.EntityClient.EntityConnection;
                    if (entityConnection == null) return;
                    var cnx = entityConnection.StoreConnection;
                    if (cnx.State == ConnectionState.Closed) cnx.Open();
                    var t = cnx.BeginTransaction();
                    try
                    {
                        var cmd = cnx.CreateCommand();
                        cmd.Transaction = t;
                        for (var i = 0; i < count; i++)
                        {
                            var estado = string.Format("Completado.. {0}%", i * 100 / count);
                            if (EstadoEvent != null) EstadoEvent(estado);
                            cmd.CommandText = querys[i];
                            cmd.ExecuteNonQuery();
                        }
                        t.Commit();
                        cnx.Close();
                    }
                    catch (Exception e)
                    {
                        t.Rollback();
                        if (ErrorEvent != null) ErrorEvent(e.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                if (ErrorEvent != null) ErrorEvent(e.ToString());
            }
        }

        private IEnumerable<string> ObtenerVentaQuery(IEnumerable<ImportacionRegistroVentaDto> ventasImportada)
        {
            try
            {
                var ventasQuery = new List<string>();
                var keysExistentes = VentasKeysExistentes.ToList();
                var idVenta = GetNewIdValue(40, "ZQ");
                foreach (var vtaAgrupada in ventasImportada.GroupBy(g => new { periodo = DateTime.Parse(g.FechaEmision).Year, mes = DateTime.Parse(g.FechaEmision).Month }))
                {
                    var nroVenta = new VentaBL().ObtenerCorrelativoVenta(vtaAgrupada.Key.periodo.ToString(), vtaAgrupada.Key.mes.ToString("00"));
                    foreach (var vta in vtaAgrupada)
                    {
                        if (keysExistentes.Contains(vta.KeyVenta)) throw new Exception("Una o más ventas ya existen en la base de datos. Proceso falló.");
                        int i;
                        vta.RazonSocialNombre = vta.RazonSocialNombre.Replace("'", string.Empty);
                        vta.Glosa = vta.Glosa.Replace("'", string.Empty);
                        var fechaEmision = DateTime.Parse(vta.FechaEmision);
                        var serie = int.TryParse(vta.Serie.Trim(), out i) ? i.ToString("0000") : vta.Serie.Trim();
                        var correlativo = int.Parse(vta.Correlativo.Trim()).ToString("00000000");
                        vta.NroRegistro = string.Format("01{0}", nroVenta.ToString("000000"));
                        var fecha = string.Format("{0}-{1}-{2}", fechaEmision.Year, fechaEmision.Month, fechaEmision.Day);
                        var igv = vta.ImporteVenta - Utils.Windows.DevuelveValorRedondeado(vta.ImporteVenta / 1.18m, 2);
                        var valor = Utils.Windows.DevuelveValorRedondeado(vta.ImporteVenta / 1.18m, 2);
                        var valorVenta = Utils.Windows.DevuelveValorRedondeado(vta.ImporteVenta / 1.18m, 2);
                        var total = vta.ImporteVenta;
                        var serieref = !string.IsNullOrWhiteSpace(vta.SerieRef) ? int.TryParse(vta.SerieRef.Trim(), out i) ? i.ToString("0000") : vta.SerieRef.Trim() : "";
                        var correlativoref = !string.IsNullOrWhiteSpace(vta.CorrelativoRef) ? int.Parse(vta.CorrelativoRef.Trim()).ToString("00000000") : "";
                        var tipoVenta = vta.Moneda.Equals("S") ? 3 : 4;
                        var moneda = vta.Moneda.Equals("S") ? 1 : 2;
                        var anulado = vta.Anulado.Equals("S") ? 0 : 1;
                        vta.IdVenta = idVenta;
                        ventasQuery.Add(string.Format("INSERT INTO venta (\"v_IdVenta\", \"v_Periodo\", \"v_Mes\", \"v_Correlativo\", \"i_IdIgv\", " +
                        "\"i_IdTipoDocumento\", \"v_SerieDocumento\", \"v_CorrelativoDocumento\", \"v_CorrelativoDocumentoFin\", \"i_IdTipoDocumentoRef\", " +
                        "\"v_SerieDocumentoRef\", \"v_CorrelativoDocumentoRef\", \"t_FechaRef\", \"v_IdCliente\", \"v_NombreClienteTemporal\", " +
                        "\"v_DireccionClienteTemporal\", \"t_FechaRegistro\", \"d_TipoCambio\", \"i_NroDias\", \"t_FechaVencimiento\", \"i_IdCondicionPago\", " +
                        "\"v_Concepto\", \"i_IdMoneda\", \"i_IdEstado\", \"i_EsAfectoIgv\", \"i_PreciosIncluyenIgv\", \"v_IdVendedor\", \"v_IdVendedorRef\", " +
                        "\"d_PorcDescuento\", \"d_PocComision\", \"d_ValorFOB\", \"i_DeduccionAnticipio\", \"v_NroPedido\", \"v_NroGuiaRemisionSerie\", " +
                        "\"v_NroGuiaRemisionCorrelativo\", \"v_OrdenCompra\", \"t_FechaOrdenCompra\", \"i_IdTipoVenta\", \"i_IdTipoOperacion\", " +
                        "\"i_IdEstablecimiento\", \"i_IdPuntoEmbarque\", \"i_IdPuntoDestino\", \"i_IdTipoEmbarque\", \"i_IdMedioPagoVenta\", \"v_Marca\", " +
                        "\"i_DrawBack\", \"v_NroBulto\", \"v_BultoDimensiones\", \"d_PesoBrutoKG\", \"d_PesoNetoKG\", \"d_Valor\", \"d_ValorVenta\", \"d_Descuento\", " +
                        "\"d_Percepcion\", \"d_Anticipio\", \"d_IGV\", \"d_Total\", \"d_total_isc\", \"d_total_otrostributos\", \"v_PlacaVehiculo\", \"i_Impresion\"," +
                        " \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"v_IdFormatoUnicoFacturacion\", \"v_IdTipoKardex\", " +
                        "\"i_EstadoSunat\", \"i_IdTipoNota\", \"i_EsGratuito\", \"i_IdTipoBulto\", \"i_IdDireccionCliente\", \"d_SeguroTotal\", \"d_FleteTotal\", \"d_CantidaTotal\"," +
                        " \"v_NroBL\", \"t_FechaPagoBL\", \"v_Contenedor\", \"v_Banco\", \"v_Naviera\", \"i_AplicaPercepcion\", \"i_ClienteEsAgente\", " +
                        "\"d_PorcentajePercepcion\", \"i_ItemsAfectosPercepcion\", \"v_MotivoEliminacion\", \"v_NroBultoIngles\") " +
                        "VALUES ({0}'{1}', {0}'{2}', {0}'{3:00}', {0}'{4}', 1, {5}, {0}'{6}', {0}'{7}', {0}'{7}', {22}, {0}'{23}', {0}'{24}'," +
                        " {0}'{8}', {0}'{10}', {0}'{11}', {0}'', {0}'{8}', '{12}', 0, {0}'{8}', " +
                        "1, {0}'{9}', {18}, {21}, 1, 1, {0}'-1', {0}'-1', '0', '1', '0', 0, {0}'', {0}'', {0}'', {0}'', {0}'{8}', {19}, 1, 1, " +
                        "NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, '{13}', '{14}', '0', NULL, '0', '{15}', '{16}', '0', '0', NULL, NULL, " +
                        "0, {20}, {17}, NULL, NULL, NULL, NULL, NULL, NULL, NULL, -1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL," +
                        "NULL, NULL, NULL, NULL, NULL);", SufijoString, vta.IdVenta, fechaEmision.Year, fechaEmision.Month, vta.NroRegistro, vta.IdTipoDoc,
                        serie, correlativo, fecha, vta.Glosa, vta.IdCliente, vta.RazonSocialNombre, vta.TipoCambio, valor, valorVenta, igv,
                        total, FechaInsercion(), moneda, tipoVenta, Globals.ClientSession.i_SystemUserId, anulado, vta.IdTipoDocRef, serieref, correlativoref));
                        idVenta = GetNextIdValue(idVenta);
                        nroVenta++;
                    }
                }

                ventasQuery.Add(UpdateLastsecuential(40, idVenta));
                return ventasQuery;
            }
            catch (Exception e)
            {
                if (ErrorEvent != null) ErrorEvent(e.ToString());
                return null;
            }
        }

        private IEnumerable<string> ObtenerCobranzaPendienteQuery(IEnumerable<ImportacionRegistroVentaDto> ventasImportada)
        {
            try
            {
                var cobranzaPendienteQuery = new List<string>();
                var idCobranzaPendiente = GetNewIdValue(46, "ZC");

                foreach (var vta in ventasImportada)
                {
                    if (vta.IdTipoDoc == 7) continue;

                    cobranzaPendienteQuery.Add(string.Format("INSERT INTO cobranzapendiente (\"v_IdCobranzaPendiente\", \"v_IdVenta\", \"d_Acuenta\", \"d_Saldo\", \"i_Eliminado\",  " +
                    "\"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\") " +
                    "VALUES ({0}'{1}', {0}'{2}', '0', '{3}', 0, {4}, {5}, NULL, NULL);", SufijoString, idCobranzaPendiente, vta.IdVenta, vta.ImporteVenta,
                    Globals.ClientSession.i_SystemUserId, FechaInsercion()));
                    idCobranzaPendiente = GetNextIdValue(idCobranzaPendiente);
                }

                cobranzaPendienteQuery.Add(UpdateLastsecuential(46, idCobranzaPendiente));
                return cobranzaPendienteQuery;
            }
            catch (Exception e)
            {
                if (ErrorEvent != null) ErrorEvent(e.ToString());
                return null;
            }
        }

        private IEnumerable<string> ObtenerVentaDetalleQuery(IEnumerable<ImportacionRegistroVentaDto> ventasImportada)
        {
            try
            {
                var ventasQuery = new List<string>();
                var idVentaDetalle = GetNewIdValue(41, "ZN");

                foreach (var vta in ventasImportada)
                {
                    var igv = vta.ImporteVenta - Utils.Windows.DevuelveValorRedondeado(vta.ImporteVenta / 1.18m, 2);
                    var valor = Utils.Windows.DevuelveValorRedondeado(vta.ImporteVenta / 1.18m, 2);
                    var valorVenta = Utils.Windows.DevuelveValorRedondeado(vta.ImporteVenta / 1.18m, 2);
                    var total = vta.ImporteVenta;

                    ventasQuery.Add(string.Format("INSERT INTO ventadetalle (\"v_IdVentaDetalle\", \"v_IdVenta\", \"v_IdMovimientoDetalle\", \"v_IdDiarioDetalle\", " +
                                "\"i_IdAlmacen\", \"v_NroCuenta\", \"i_Anticipio\", \"v_IdProductoDetalle\", \"v_DescripcionProducto\", \"d_Cantidad\", " +
                                "\"d_CantidadEmpaque\", \"i_IdUnidadMedida\", \"d_Precio\", \"d_Valor\", \"d_ValorVenta\", \"d_Descuento\", \"d_Igv\", " +
                                "\"d_PrecioVenta\", \"d_PrecioImpresion\", \"d_Percepcion\", \"d_isc\", \"d_otrostributos\", \"i_IdMonedaLP\", \"i_IdTipoOperacion\"," +
                                " \"i_IdTipoOperacionAnexo\", \"i_NroUnidades\", \"d_PorcentajeComision\", \"v_Observaciones\", \"v_PedidoExportacion\"," +
                                " \"v_FacturaRef\", \"i_IdCentroCosto\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\"," +
                                " \"t_ActualizaFecha\", \"i_IdVentaDetalleAnexo\", \"d_PrecioPactado\", \"d_SeguroXProducto\", \"d_FleteXProducto\") " +
                                "VALUES ({0}'{1}', {0}'{2}', NULL, NULL, {3}, {0}'{4}', 0, NULL, NULL, '1', '0'," +
                                " -1, '{5}', '{6}', '{7}', '0', '{8}', '{9}', NULL, NULL, '0', '0', NULL, 1, NULL, 0, '0', NULL, NULL, {0}'00', " +
                                "{0}'0', 0, {10}, {11}, NULL, NULL, -1, NULL, NULL, NULL);",
                               SufijoString, idVentaDetalle, vta.IdVenta, Globals.ClientSession.i_IdAlmacenPredeterminado ?? 1, vta.CuentaMercaderia, vta.ImporteVenta,
                               valorVenta, valor, igv, total, Globals.ClientSession.i_SystemUserId, FechaInsercion()));
                    idVentaDetalle = GetNextIdValue(idVentaDetalle);
                }
                ventasQuery.Add(UpdateLastsecuential(41, idVentaDetalle));
                return ventasQuery;
            }
            catch (Exception e)
            {
                if (ErrorEvent != null) ErrorEvent(e.ToString());
                return null;
            }
        }

        private IEnumerable<string> ObtenerClienteQuery(IEnumerable<ImportacionRegistroVentaDto> ventasImportada)
        {
            try
            {
                var ventasQuery = new List<string>();
                var idCliente = GetNewIdValue(14, "CL");
                foreach (var vta in ventasImportada)
                {
                    if (!string.IsNullOrWhiteSpace(vta.NroDocumentoIdentidad) && !vta.NroDocumentoIdentidad.Equals("0"))
                    {
                        KeyValueDTO cliente = ListaClientes.TryGetValue(vta.NroDocumentoIdentidad, out cliente) ? cliente : null;
                        if (cliente != null)
                        {
                            vta.IdCliente = cliente.Id;
                        }
                        else
                        {
                            vta.Direccion = vta.Direccion.Replace("'", string.Empty);
                            vta.IdCliente = idCliente;
                            var esPersonaNatural = vta.NroDocumentoIdentidad.Trim().Length == 8 || vta.NroDocumentoIdentidad.Trim().StartsWith("1");
                            ventasQuery.Add(string.Format(
                                "INSERT INTO cliente (\"v_IdCliente\", \"v_IdVendedor\", \"v_CodCliente\", \"v_FlagPantalla\", \"i_IdTipoIdentificacion\", " +
                                "\"v_NroDocIdentificacion\", \"i_IdTipoPersona\", \"v_PrimerNombre\", \"v_SegundoNombre\", \"v_ApePaterno\", \"v_ApeMaterno\", " +
                                "\"v_RazonSocial\", \"i_IdSexo\", \"v_DirecPrincipal\", \"i_IdPais\", \"i_IdDepartamento\", \"i_IdProvincia\", \"i_IdDistrito\"," +
                                " \"v_DirecSecundaria\", \"v_TelefonoFijo\", \"v_TelefonoFax\", \"v_TelefonoMovil\", \"v_Correo\", \"v_PaginaWeb\", \"i_Activo\"," +
                                " \"t_FechaNacimiento\", \"i_Nacionalidad\", \"v_NombreContacto\", \"i_IdGrupoCliente\", \"i_IdListaPrecios\", \"i_IdZona\", " +
                                "\"i_EsPrestadorServicios\", \"v_Servicio\", \"i_IdConvenioDobleTributacion\", \"i_AfectoDetraccion\", \"i_UsaLineaCredito\", " +
                                "\"i_Eliminado\", \"i_InsertaIdUsuario\", \"v_NroCuentaDetraccion\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", " +
                                "\"v_Password\", \"v_Alias\", \"v_MotivoEliminacion\", \"i_IdTipoAccionesSocio\", \"i_NumeroAccionesSuscritas\", \"i_NumeroAccionesPagadas\") " +
                                "VALUES ({0}'{1}', NULL, {0}'{2}', {0}'C', 6, {0}'{2}', {6}, {0}'', {0}'', {0}'', {0}'', " +
                                "{0}'{3}', -1, {0}'{4}', 1, 1391, 1392, 1407, NULL," +
                                " {0}'', NULL, {0}'', {0}'', NULL, 1, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, 0, 1, NULL, {5}, " +
                                "NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);", SufijoString, vta.IdCliente,
                                vta.NroDocumentoIdentidad, vta.RazonSocialNombre,
                                vta.Direccion, FechaInsercion(), esPersonaNatural ? 1 : 2));
                            idCliente = GetNextIdValue(idCliente);
                        }
                    }
                    else
                        vta.IdCliente = "N002-CL000000000";
                }

                ventasQuery.Add(UpdateLastsecuential(14, idCliente));
                return ventasQuery;
            }
            catch (Exception e)
            {
                if (ErrorEvent != null) ErrorEvent(e.ToString());
                return null;
            }
        }

        private IEnumerable<string> ObtenerLibroDiarioQuery(IEnumerable<ImportacionRegistroVentaDto> ventasImportada)
        {
            try
            {
                var diarioQuery = new List<string>();
                var diarioDetalleQuery = new List<string>();
                var idDiario = GetNewIdValue(59, "XI");
                var idDiarioDetalle = GetNewIdValue(60, "XJ");
                var conceptos = Conceptos.ToDictionary(k => k.v_Codigo, o => o);

                foreach (var vta in ventasImportada)
                {
                    int i;
                    if (vta.Anulado.Equals("S")) continue;
                    var igv = vta.ImporteVenta - Utils.Windows.DevuelveValorRedondeado(vta.ImporteVenta / 1.18m, 2);
                    var valorVenta = Utils.Windows.DevuelveValorRedondeado(vta.ImporteVenta / 1.18m, 2);
                    var total = vta.ImporteVenta;
                    var moneda = vta.Moneda.Equals("S") ? 1 : 2;
                    var tipoVenta = vta.Moneda.Equals("S") ? "03" : "04";
                    var serie = int.TryParse(vta.Serie.Trim(), out i) ? i.ToString("0000") : vta.Serie.Trim();
                    var correlativo = int.Parse(vta.Correlativo.Trim()).ToString("00000000");
                    var serieref = !string.IsNullOrWhiteSpace(vta.SerieRef) ? int.TryParse(vta.SerieRef.Trim(), out i) ? i.ToString("0000") : vta.SerieRef.Trim() : String.Empty;
                    var correlativoref = !string.IsNullOrWhiteSpace(vta.CorrelativoRef) ? int.Parse(vta.CorrelativoRef.Trim()).ToString("00000000") : String.Empty;
                    administracionconceptos concepto = conceptos.TryGetValue(tipoVenta, out concepto) ? concepto : null;
                    if (concepto == null) throw new Exception("Concepto relacionado con la venta no existe.");
                    var fecha = DateTime.Parse(vta.FechaEmision);
                    vta.Cuenta12 = concepto.v_CuentaPVenta;
                    var fechaStr = string.Format("{0}-{1}-{2}", fecha.Year, fecha.Month, fecha.Day);
                    var cuentasDetalle = new[] { concepto.v_CuentaIGV, concepto.v_CuentaPVenta, vta.CuentaMercaderia };
                    decimal importe12 = 0m, cambio12 = 0m;

                    foreach (var detalle in cuentasDetalle)
                    {
                        var cuenta = string.Empty;
                        var nrodocRef = !string.IsNullOrWhiteSpace(serieref) ? serieref + "-" + correlativoref : "";
                        var naturaleza = string.Empty;
                        var importe40 = igv;
                        var cambio40 = Utils.Windows.DevuelveValorRedondeado(moneda == 1 ? igv / vta.TipoCambio : igv * vta.TipoCambio, 2);
                        var importe70 = valorVenta;
                        var cambio70 = Utils.Windows.DevuelveValorRedondeado(moneda == 1 ? valorVenta / vta.TipoCambio : valorVenta * vta.TipoCambio, 2);
                        importe12 = total;
                        cambio12 = cambio40 + cambio70;
                        decimal importe = 0m, cambio = 0m;

                        switch (detalle.Substring(0, 2))
                        {
                            case "12":
                                cuenta = detalle;
                                naturaleza = "D";
                                importe = importe12;
                                cambio = cambio12;
                                break;

                            case "40":
                                cuenta = detalle;
                                naturaleza = "H";
                                importe = importe40;
                                cambio = cambio40;
                                break;

                            case "70":
                                cuenta = detalle;
                                naturaleza = "H";
                                importe = importe70;
                                cambio = cambio70;
                                break;
                        }

                        diarioDetalleQuery.Add(string.Format("INSERT INTO diariodetalle (\"v_IdDiarioDetalle\", \"i_EsDestino\", \"v_IdDiario\", \"v_IdCliente\", " +
                        "\"v_NroCuenta\", \"v_Naturaleza\", \"d_Importe\", \"d_Cambio\", \"i_IdCentroCostos\", \"i_IdTipoDocumento\", " +
                        "\"v_NroDocumento\", \"t_Fecha\", \"i_IdTipoDocumentoRef\", \"v_NroDocumentoRef\", \"v_Analisis\", \"v_OrigenDestino\"," +
                        " \"v_Pedido\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", \"i_ActualizaIdUsuario\", " +
                        "\"t_ActualizaFecha\", \"i_IdPatrimonioNeto\") " +
                        "VALUES ({0}'{1}', NULL, {0}'{2}', {0}'{3}', {0}'{4}', " +
                        "{0}'{10}', '{11}', '{12}', {0}'0', {8}, {0}'{9}', {0}'{7}', {13}, '{14}', NULL, NULL," +
                        " NULL, 0, {6}, {5}, NULL, NULL, NULL);", SufijoString, idDiarioDetalle, idDiario,
                        vta.IdCliente, cuenta, FechaInsercion(), Globals.ClientSession.i_SystemUserId, fechaStr,
                        vta.IdTipoDoc, serie + "-" + correlativo, naturaleza, importe, cambio, vta.IdTipoDocRef, nrodocRef));

                        idDiarioDetalle = GetNextIdValue(idDiarioDetalle);
                    }

                    diarioQuery.Add(string.Format("INSERT INTO diario (\"v_IdDiario\", \"v_IdDocumentoReferencia\", \"i_IdPlanillaNumeracion\", " +
                    "\"v_Periodo\", \"v_Mes\", \"i_IdTipoDocumento\", \"v_Correlativo\", \"d_TipoCambio\", \"i_IdTipoComprobante\", " +
                    "\"i_IdMoneda\", \"i_IdEstado\", \"v_Nombre\", \"v_Glosa\", \"t_Fecha\", \"d_TotalDebe\", \"d_TotalHaber\", \"d_TotalDebeCambio\", " +
                    "\"d_TotalHaberCambio\", \"d_DiferenciaDebe\", \"d_DiferenciaHaber\", \"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\", " +
                    "\"i_ActualizaIdUsuario\", \"t_ActualizaFecha\", \"v_MotivoEliminacion\") " +
                    "VALUES ({0}'{1}', {0}'{2}', NULL, {0}'{3}', {0}'{4:00}', 337, {0}'{5}', '{6}', 2, {7}, " +
                    "NULL, {0}'{8}', {0}'{9}', {0}'{10}', '{13}', '{13}', '{14}', '{14}', '0', '0', 0, {11}, " +
                    "{12}, NULL, NULL, NULL);",
                    SufijoString, idDiario, vta.IdVenta, fecha.Year, fecha.Month, vta.NroRegistro, vta.TipoCambio, moneda,
                    vta.RazonSocialNombre, vta.Glosa, fechaStr, Globals.ClientSession.i_SystemUserId, FechaInsercion(), importe12, cambio12));
                    idDiario = GetNextIdValue(idDiario);
                }

                diarioQuery.Add(UpdateLastsecuential(59, idDiario));
                diarioDetalleQuery.Add(UpdateLastsecuential(60, idDiarioDetalle));

                return diarioQuery.Concat(diarioDetalleQuery);

            }
            catch (Exception e)
            {
                if (ErrorEvent != null) ErrorEvent(e.ToString());
                return null;
            }
        }

        private IEnumerable<string> ObtenerTesoreriaQuery(List<ImportacionRegistroVentaDto> ventasImportada,
            List<ImportacionRegistroVentaCobranzaDto> cobranzasImportadas)
        {
            try
            {
                if (!cobranzasImportadas.Any()) return new List<string>();

                var diccionarioVentas = ventasImportada.Where(v => v.Anulado == "N").ToDictionary(k => k.KeyVenta, o => o);
                var tesoreriaQuery = new List<string>();
                var tesoreriaDetalleQuery = new List<string>();
                var idTesoreria = GetNewIdValue(55, "XE");
                var idTesoreriaDetalle = GetNewIdValue(56, "XF");
                var objOperationResult = new OperationResult();
                var agurpadoFecha = cobranzasImportadas.GroupBy(g =>
                    new { periodo = DateTime.Parse(g.FechaCobranza).Year, mes = DateTime.Parse(g.FechaCobranza).Month, tipoTarjeta = g.TarjetaEnum });

                foreach (var cobranzasAgrupadoFecha in agurpadoFecha)
                {
                    int i;
                    var idTipoDocumentoBanco = GetBankDocument(cobranzasAgrupadoFecha.Key.tipoTarjeta);
                    if (idTipoDocumentoBanco == -1) throw new Exception("Tipo Documento Cobranza no definido!");

                    var cta10 = GetCta10(cobranzasAgrupadoFecha.Key.tipoTarjeta);
                    if (string.IsNullOrWhiteSpace(cta10)) throw new Exception("Cuenta de caja/banco no definida.!");

                    var fechaReferencial = new DateTime(cobranzasAgrupadoFecha.Key.periodo, cobranzasAgrupadoFecha.Key.mes, 1);
                    var corr = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult, ListaProcesos.Tesoreria, null, fechaReferencial, null, idTipoDocumentoBanco);
                    var correlativo = int.TryParse(corr.Substring(2, 6), out i) ? i : 1;

                    foreach (var cobranzasVenta in cobranzasAgrupadoFecha.GroupBy(g => g.KeyVenta))
                    {
                        ImportacionRegistroVentaDto vtaRelacionada;
                        if (diccionarioVentas.TryGetValue(cobranzasVenta.Key, out vtaRelacionada))
                        {
                            foreach (var objCobranza in cobranzasVenta)
                            {
                                var serie = int.TryParse(vtaRelacionada.Serie.Trim(), out i) ? i.ToString("0000") : vtaRelacionada.Serie.Trim();
                                var correlativovta = int.Parse(vtaRelacionada.Correlativo.Trim()).ToString("00000000");
                                var fecha = DateTime.Parse(objCobranza.FechaCobranza);
                                var nroRegistro = string.Format("01{0}", correlativo.ToString("000000"));
                                var fechaRegistro = string.Format("{0}-{1}-{2}", fecha.Day, fecha.Month, fecha.Year);
                                var concepto = string.Format("DOCUMENTO: {0} DÍA: {1}", vtaRelacionada.NroDocumento, fechaRegistro);
                                var moneda = objCobranza.Moneda.Equals("S") ? 1 : 2;
                                var importe = objCobranza.MontoCobrado;
                                var cambio = Utils.Windows.DevuelveValorRedondeado(moneda == 1 ? importe / objCobranza.TipoCambio : importe * objCobranza.TipoCambio, 2);

                                var ctasDetalle = new[] { cta10, vtaRelacionada.Cuenta12 };

                                foreach (var ctaDetalleCobranza in ctasDetalle)
                                {
                                    if (string.IsNullOrWhiteSpace(ctaDetalleCobranza))
                                        throw new Exception("Cuenta de banco no relacionada Venta:" + vtaRelacionada.NroDocumento);

                                    var esdocumentoVenta = ctaDetalleCobranza.StartsWith("12");
                                    var naturaleza = esdocumentoVenta ? "H" : "D";
                                    var tipoDocDetalle = esdocumentoVenta ? vtaRelacionada.IdTipoDoc : idTipoDocumentoBanco;
                                    var nroDocumento = esdocumentoVenta ? (serie + "-" + correlativovta) : nroRegistro;
                                    var idAnexo = esdocumentoVenta ? "'" + vtaRelacionada.IdCliente + "'" : "NULL";

                                    tesoreriaDetalleQuery.Add(string.Format("INSERT INTO tesoreriadetalle (\"v_IdTesoreriaDetalle\", \"i_EsDestino\", \"v_IdTesoreria\", \"v_IdCliente\", " +
                                    "	\"v_NroCuenta\", \"v_Naturaleza\", \"d_Importe\", \"d_Cambio\", \"i_IdCentroCostos\", \"i_IdCaja\", \"i_IdTipoDocumento\", " +
                                    "	\"v_NroDocumento\", \"t_Fecha\", \"i_IdTipoDocumentoRef\", \"v_NroDocumentoRef\", \"v_Analisis\", \"v_OrigenDestino\", " +
                                    "	\"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\")" +
                                    "VALUES ({0}'{1}', {0}'0', {0}'{2}', {12}, {0}'{3}', {0}'{4}', '{10}', '{11}', {0}'', 0, {5}, " +
                                    "	{0}'{6}', {0}'{7}', -1, {0}'', NULL, NULL, 0, {8}, {9});",
                                    SufijoString, idTesoreriaDetalle, idTesoreria, ctaDetalleCobranza, naturaleza,
                                    tipoDocDetalle, nroDocumento, fechaRegistro, Globals.ClientSession.i_SystemUserId,
                                    FechaInsercion(), importe, cambio, idAnexo));

                                    idTesoreriaDetalle = GetNextIdValue(idTesoreriaDetalle);
                                }

                                tesoreriaQuery.Add(string.Format("INSERT INTO tesoreria (\"v_IdTesoreria\", \"v_Periodo\", \"i_IdTipoDocumento\", \"v_Mes\", \"v_IdCobranza\", " +
                                   "	\"v_NroCuentaCajaBanco\", \"v_Correlativo\", \"i_TipoMovimiento\", \"t_FechaRegistro\", \"d_TipoCambio\", " +
                                   "	\"i_IdMedioPago\", \"v_Nombre\", \"v_Glosa\", \"i_IdMoneda\", \"i_IdEstado\", \"d_TotalDebe_Importe\", \"d_TotalDebe_Cambio\", " +
                                   "	\"d_TotalHaber_Importe\", \"d_TotalHaber_Cambio\", \"d_Diferencia_Importe\", \"d_Diferencia_Cambio\", \"i_AplicaRetencion\", " +
                                   "	\"i_Eliminado\", \"i_InsertaIdUsuario\", \"t_InsertaFecha\")" +
                                   "VALUES ({0}'{1}', {0}'{2}', {3}, {0}'{4:00}', NULL, {0}'{5}', {0}'{6}', 1, {0}'{7}', '{8}', " +
                                   "	9, {0}'{12}', {0}'{9}', {10}, 1, '{14}', '{15}', '{14}', '{15}', '0', '0', NULL, 0, {13}, " +
                                   "	{11});", SufijoString, idTesoreria, cobranzasAgrupadoFecha.Key.periodo,
                                   idTipoDocumentoBanco, cobranzasAgrupadoFecha.Key.mes, cta10, nroRegistro, fechaRegistro, objCobranza.TipoCambio,
                                   concepto, moneda, FechaInsercion(), concepto, Globals.ClientSession.i_SystemUserId, importe, cambio));

                                idTesoreria = GetNextIdValue(idTesoreria);
                                correlativo++;
                            }
                        }
                    }
                }

                tesoreriaQuery.Add(UpdateLastsecuential(55, idTesoreria));
                tesoreriaDetalleQuery.Add(UpdateLastsecuential(56, idTesoreriaDetalle));

                return tesoreriaQuery.Concat(tesoreriaDetalleQuery);
            }
            catch (Exception e)
            {
                if (ErrorEvent != null) ErrorEvent(e.ToString());
                return null;
            }
        }

        #region Utils

        private int GetBankDocument(ImportacionRegistroVentaCobranzaDto.Tarjeta tipoTarjeta)
        {
            switch (tipoTarjeta)
            {
                case ImportacionRegistroVentaCobranzaDto.Tarjeta.Ninguna:
                    return Configuraciones.i_IdDocumentoEfectivo ?? -1;

                case ImportacionRegistroVentaCobranzaDto.Tarjeta.Visa:
                    return Configuraciones.i_IdDocumentoVisa ?? -1;

                case ImportacionRegistroVentaCobranzaDto.Tarjeta.Mastercard:
                    return Configuraciones.i_IdDocumentoMastercard ?? -1;

                case ImportacionRegistroVentaCobranzaDto.Tarjeta.AmericanExpress:
                    return Configuraciones.i_IdDocumentoAmericanExpress ?? -1;

                default:
                    return Configuraciones.i_IdDocumentoEfectivo ?? -1;
            }
        }

        private string GetCta10(ImportacionRegistroVentaCobranzaDto.Tarjeta tipoTarjeta)
        {
            switch (tipoTarjeta)
            {
                case ImportacionRegistroVentaCobranzaDto.Tarjeta.Ninguna:
                    return Configuraciones.v_CtaEfectivo;

                case ImportacionRegistroVentaCobranzaDto.Tarjeta.Visa:
                    return Configuraciones.v_CtaVisa;

                case ImportacionRegistroVentaCobranzaDto.Tarjeta.Mastercard:
                    return Configuraciones.v_CtaMastercard;

                case ImportacionRegistroVentaCobranzaDto.Tarjeta.AmericanExpress:
                    return Configuraciones.v_CtaAmericanExpress;

                default:
                    return Configuraciones.v_CtaEfectivo;
            }
        }

        private static string GetNewIdValue(int idTable, string sufix)
        {
            var intNodeId = Globals.ClientSession.i_IdAlmacenPredeterminado ?? 1;
            var secuentialId = new SecuentialBL().GetNextSecuentialId(intNodeId, idTable);
            var newIdVenta = Utils.GetNewId(intNodeId, secuentialId, sufix);
            return newIdVenta;
        }

        private static string GetNextIdValue(string previousId)
        {
            //N001-ZN000043805
            var body = previousId.Substring(0, 7);
            var secuential = int.Parse(previousId.Substring(8, 8)) + 1;
            return string.Format("{0}{1:000000000}", body, secuential);
        }

        private static string FechaInsercion()
        {
            return Globals.TipoMotor == TipoMotorBD.MSSQLServer ? "getdate()" : "clock_timestamp()";
        }

        private static string UpdateLastsecuential(int tableId, string ultimoId)
        {
            var secuential = int.Parse(ultimoId.Substring(8, 8)) + 1;
            return string.Format("update secuential set \"i_SecuentialId\" = {0} where \"i_TableId\" = {1} and \"v_ReplicationID\" = '{2}' and \"i_NodeId\" = {3};", secuential, tableId, Globals.ClientSession.ReplicationNodeID, Globals.ClientSession.i_IdAlmacenPredeterminado ?? 1);
        }

        #endregion
    }

    public class BulkInsertVentasConfigBl
    {
        public ventaimportaciondataconfigDto GetConfig()
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var config = dbContext.ventaimportaciondataconfig.FirstOrDefault();
                return config != null ? config.ToDTO() : new ventaimportaciondataconfigDto();
            }
        }

        public void UpdateConfig(ventaimportaciondataconfigDto entityDto)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var config = dbContext.ventaimportaciondataconfig.FirstOrDefault();
                if (config != null)
                {
                    config = entityDto.ToEntity();
                    dbContext.ventaimportaciondataconfig.ApplyCurrentValues(config);
                }
                else
                {
                    dbContext.ventaimportaciondataconfig.AddObject(entityDto.ToEntity());
                }

                dbContext.SaveChanges();
            }
        }
    }
}