using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Devart.Data.PostgreSql;
using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;

namespace SAMBHS.Contabilidad.BL
{
    public class ContabilidadDao
    {
        /// <summary>
        /// Obtiene un empaquetado de todas las tesorerias por rango de fechas.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="fIni"></param>
        /// <param name="fFin"></param>
        /// <param name="ctaMayor"></param>
        /// <param name="rangoCuentas"></param>
        /// <param name="moneda"></param>
        /// <param name="nroDocumentoIdentidad"></param>
        /// <returns></returns>
        public static IEnumerable<ReporteLibroMayor> ObtenerTesorerias(ref OperationResult pobjOperationResult, DateTime fIni, 
            DateTime fFin, string ctaMayor, ICollection<string> rangoCuentas, int moneda, string nroDocumentoIdentidad)
        {
            try
            {
                List<ReporteLibroMayor> listaRetorno;
                var motorBd = Globals.TipoMotor;
                var concat = motorBd == TipoMotorBD.MSSQLServer ? "+" : "||";
                var ini = string.Format("{0}-{1}-{2}", fIni.Day, fIni.Month, fIni.Year);
                var fin = string.Format("{0}-{1}-{2}", fFin.Day, fFin.Month, fFin.Year);
                var rangoCtas = rangoCuentas != null && rangoCuentas.Any() ? string.Join(", ", rangoCuentas.Select(s => "'" + s + "'")) : "null";
                var periodo = Globals.ClientSession.i_Periodo.ToString();
                ctaMayor = string.IsNullOrWhiteSpace(ctaMayor) ? "null" : "'" + ctaMayor.Trim() + "%'";
                var r = rangoCuentas != null ? rangoCuentas.Count : 0;
                var formatDate = motorBd == TipoMotorBD.MSSQLServer ? "set dateformat 'dmy';" : string.Empty;
                nroDocumentoIdentidad = string.IsNullOrWhiteSpace(nroDocumentoIdentidad) ? "null" : "'" + nroDocumentoIdentidad.Trim() + "%'";

                #region Consulta
                var query = formatDate + "select  " +
                                    "t.\"t_FechaRegistro\" as \"fecha\"," +
                                    "d1.\"v_Siglas\" as \"SiglasComprobante\", " +
                                    "d1.\"v_Nombre\" as \"NombreComprobante\", " +
                                    "d1.\"v_Siglas\" " + concat + " ' ' " + concat + "  ltrim(rtrim(t.\"v_Mes\")) " + concat + " '-' " + concat + " ltrim(rtrim(t.\"v_Correlativo\")) as \"nroComprobante\",  " +
                                    "case when(td.\"v_NroDocumento\" is null) then '' else d2.\"v_Siglas\" " + concat + " ' ' " + concat + "  ltrim(rtrim(td.\"v_NroDocumento\")) end as \"docReferencia\",     " +
                                    "ltrim(rtrim(td.\"v_NroDocumento\")) as SerieCorrelativoDetalle, " + 
                                    "t.\"v_Nombre\" as \"nombre\",     " +
                                    "case when (td.\"v_Analisis\" is null or td.\"v_Analisis\" = '') then t.\"v_Glosa\" else td.\"v_Analisis\" end as \"descripcionOperacion\", " +
                                    "CASE WHEN ((td.\"v_Naturaleza\" = 'D') AND (t.\"i_IdMoneda\" = 1)) AND (1 = " + moneda + ") THEN td.\"d_Importe\" WHEN ((td.\"v_Naturaleza\" = 'D') AND (2 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 1) THEN td.\"d_Importe\" WHEN ((td.\"v_Naturaleza\" = 'D') AND (1 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 2) THEN td.\"d_Cambio\" WHEN ((td.\"v_Naturaleza\" = 'D') AND (2 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 2) THEN td.\"d_Cambio\" WHEN ((td.\"v_Naturaleza\" = 'D') AND (-1 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 1) THEN td.\"d_Importe\" WHEN ((td.\"v_Naturaleza\" = 'D') AND (-1 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 2) THEN td.\"d_Cambio\" ELSE 0 END AS \"debeSoles\"," +
                                    "CASE WHEN ((td.\"v_Naturaleza\" = 'D') AND (t.\"i_IdMoneda\" = 2)) AND (2 = " + moneda + ") THEN td.\"d_Importe\" WHEN ((td.\"v_Naturaleza\" = 'D') AND (2 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 1) THEN td.\"d_Cambio\" WHEN ((td.\"v_Naturaleza\" = 'D') AND (1 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 2) THEN td.\"d_Importe\" WHEN ((td.\"v_Naturaleza\" = 'D') AND (1 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 1) THEN td.\"d_Cambio\" WHEN ((td.\"v_Naturaleza\" = 'D') AND (-1 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 1) THEN td.\"d_Cambio\" WHEN ((td.\"v_Naturaleza\" = 'D') AND (-1 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 2) THEN td.\"d_Importe\" ELSE 0 END AS \"debeDolares\", " +
                                    "CASE WHEN ((td.\"v_Naturaleza\" = 'H') AND (t.\"i_IdMoneda\" = 1)) AND (1 = " + moneda + ") THEN td.\"d_Importe\" WHEN ((td.\"v_Naturaleza\" = 'H') AND (2 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 1) THEN td.\"d_Importe\" WHEN ((td.\"v_Naturaleza\" = 'H') AND (1 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 2) THEN td.\"d_Cambio\" WHEN ((td.\"v_Naturaleza\" = 'H') AND (2 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 2) THEN td.\"d_Cambio\" WHEN ((td.\"v_Naturaleza\" = 'H') AND (-1 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 1) THEN td.\"d_Importe\" WHEN ((td.\"v_Naturaleza\" = 'H') AND (-1 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 2) THEN td.\"d_Cambio\" ELSE 0 END AS \"haberSoles\",     " +
                                    "CASE WHEN ((td.\"v_Naturaleza\" = 'H') AND (t.\"i_IdMoneda\" = 2)) AND (2 = " + moneda + ") THEN td.\"d_Importe\" WHEN ((td.\"v_Naturaleza\" = 'H') AND (2 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 1) THEN td.\"d_Cambio\" WHEN ((td.\"v_Naturaleza\" = 'H') AND (1 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 2) THEN td.\"d_Importe\" WHEN ((td.\"v_Naturaleza\" = 'H') AND (1 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 1) THEN td.\"d_Cambio\" WHEN ((td.\"v_Naturaleza\" = 'H') AND (-1 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 1) THEN td.\"d_Cambio\" WHEN ((td.\"v_Naturaleza\" = 'H') AND (-1 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 2) THEN td.\"d_Importe\" ELSE 0 END AS \"haberDolares\",      " +
                                    "' ' as \"cuentaMayor\", " +
                                    "case when (a.\"v_NroCuenta\" is null or td.\"v_NroCuenta\" = '') then '*No Existe*' else td.\"v_NroCuenta\" " + concat + " ' - ' " + concat + " a.\"v_NombreCuenta\" end as \"cuenta\",   " +
                                    "case when (a.\"v_NroCuenta\" is null or td.\"v_NroCuenta\" = '') then '*No Existe*' else td.\"v_NroCuenta\" end as \"numeroCuenta\",   " +
                                    "case when (t.\"i_IdMoneda\" = 1) then 'S/.' else 'US$.' end as \"monedaTransaccion\",   " +
                                    "t.\"t_InsertaFecha\" as \"FechaInsercion\",    " +
                                    "t.\"t_FechaRegistro\" as \"FechaMostrar\",  " +
                                    "t.\"v_Mes\" " + concat + " t.\"v_Correlativo\" as \"NroRegistro\",  " +
                                    "t.\"i_IdTipoDocumento\" as \"i_CodigoDocumento\", " +
                                    "d.\"v_Field\" as \"MonedaSiglas\",   " +
                                    "CASE WHEN t.\"i_IdEstado\" = 1 THEN CASE WHEN td.\"v_IdCliente\" IS NULL THEN 0 WHEN td.\"v_IdCliente\" = 'N002-CL000000000' THEN 1 ELSE c.\"i_IdTipoIdentificacion\" END ELSE 0 END AS \"TipoDocumentoEmisor\",  " +
                                    "CASE WHEN t.\"i_IdEstado\" = 1 THEN CASE WHEN td.\"v_IdCliente\" IS NULL THEN '0' WHEN td.\"v_IdCliente\" = 'N002-CL000000000' THEN '00000000' ELSE c.\"v_NroDocIdentificacion\" END ELSE '0' END AS \"NroDocumentoEmisor\",   " +
                                    "c.\"v_FlagPantalla\" as \"TipoCliente\", " +
                                    "CASE WHEN td.\"i_IdTipoDocumento\" IS NULL THEN -1 ELSE td.\"i_IdTipoDocumento\" END AS \"TipoComprobanteDetalle\", " +
                                    "td.\"t_Fecha\" as \"FechaDetalle\",  " +
                                    "t.\"v_IdTesoreria\" as \"IdCabecera\", " +
                                    "td.\"v_Naturaleza\" as \"Naturaleza\",  " +
                                    "a.\"i_Naturaleza\" as \"NaturalezaCuenta\",  " +
                                    "td.\"d_Importe\" as \"Importe\",  " +
                                    "td.\"d_Cambio\" as \"Cambio\",  " +
                                    "c.\"v_IdCliente\" as \"IdAnexo\", " +
                                    "2 as \"TipoComprobante\"  " +
                                "from tesoreriadetalle td       " +
                                "join tesoreria t on td.\"v_IdTesoreria\" = t.\"v_IdTesoreria\"    " +
                                "left join documento d1 on t.\"i_IdTipoDocumento\" = d1.\"i_CodigoDocumento\"    " +
                                "left join documento d2 on td.\"i_IdTipoDocumento\" = d2.\"i_CodigoDocumento\"  " +
                                "left join asientocontable a on td.\"v_NroCuenta\" = a.\"v_NroCuenta\" and a.\"v_Periodo\" = '" + periodo + "' and a.\"i_Eliminado\" = 0 " +
                                "left join datahierarchy d on t.\"i_IdMoneda\" = d.\"i_ItemId\" and d.\"i_GroupId\" = 18 and d.\"i_IsDeleted\" = 0 " +
                                "left join cliente c on td.\"v_IdCliente\" = c.\"v_IdCliente\" and c.\"i_Eliminado\" = 0 " +
                                "where t.\"i_Eliminado\" = 0 and td.\"i_Eliminado\" = 0 " +
                                "and t.\"t_FechaRegistro\" >= '" + ini + "' and t.\"t_FechaRegistro\" <= '" + fin + " 23:59' " +
                                "and (" + ctaMayor + " is null or td.\"v_NroCuenta\" like " + ctaMayor + ")" +
                                "and (" + nroDocumentoIdentidad + " is null or c.\"v_NroDocIdentificacion\" like " + nroDocumentoIdentidad + ")" +
                                "and (" + r + " = 0 or td.\"v_NroCuenta\" in (" + rangoCtas + ")) and t.\"i_IdEstado\" = 1";
                #endregion

                switch (motorBd)
                {
                    case TipoMotorBD.MSSQLServer:
                        using (var cnx = new SqlConnection(Globals.CadenaConexion)) 
                        {
                            if (cnx.State == ConnectionState.Closed) cnx.Open();
                            listaRetorno = cnx.Query<ReporteLibroMayor>(query).ToList();
                            cnx.Close();
                        }
                        break;

                    case TipoMotorBD.PostgreSQL:
                        using (var cnx = new PgSqlConnection(Globals.CadenaConexion))
                        {
                            if (cnx.State == ConnectionState.Closed) cnx.Open();
                            //!+Se puso en una transacción porque el 
                            //!+datestyle funciona sólo dentro de una transacción o session.
                            var ts = cnx.BeginTransaction();
                            cnx.Execute("SET datestyle = dmy;", transaction: ts);
                            listaRetorno = cnx.Query<ReporteLibroMayor>(query, transaction: ts).ToList();
                            ts.Commit();
                            cnx.Close();
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
                pobjOperationResult.Success = 1;
                return listaRetorno;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ContabilidadDAO.ObtenerTesorerias()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        /// <summary>
        /// Obtiene un empaquetado de todos los diarios por un rango de fechas.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="fIni"></param>
        /// <param name="fFin"></param>
        /// <param name="ctaMayor"></param>
        /// <param name="rangoCuentas"></param>
        /// <param name="moneda"></param>
        /// <param name="nroDocumentoIdentidad"></param>
        /// <returns></returns>
        public static IEnumerable<ReporteLibroMayor> ObtenerDiarios(ref OperationResult pobjOperationResult, DateTime fIni,
            DateTime fFin, string ctaMayor, ICollection<string> rangoCuentas, int moneda, string nroDocumentoIdentidad)
        {
            try
            {
                List<ReporteLibroMayor> listaRetorno;
                var motorBd = Globals.TipoMotor;
                var concat = motorBd == TipoMotorBD.MSSQLServer ? "+" : "||";
                var ini = string.Format("{0}-{1}-{2}", fIni.Day, fIni.Month, fIni.Year);
                var fin = string.Format("{0}-{1}-{2}", fFin.Day, fFin.Month, fFin.Year);
                var rangoCtas = rangoCuentas != null && rangoCuentas.Any() ? string.Join(", ", rangoCuentas.Select(s => "'" + s + "'")) : "null";
                var periodo = Globals.ClientSession.i_Periodo.ToString();
                ctaMayor = string.IsNullOrWhiteSpace(ctaMayor) ? "null" : "'" + ctaMayor.Trim() + "%'";
                var r = rangoCuentas != null ? rangoCuentas.Count : 0;
                var formatDate = motorBd == TipoMotorBD.MSSQLServer ? "set dateformat 'dmy';" : string.Empty;
                nroDocumentoIdentidad = string.IsNullOrWhiteSpace(nroDocumentoIdentidad) ? "null" : "'" + nroDocumentoIdentidad.Trim() + "%'";

                #region Consulta
                
                var query = formatDate + "select " +
                    "t.\"t_Fecha\" as \"fecha\",  " +
                    "d1.\"v_Siglas\" as \"SiglasComprobante\", "+
                    "d1.\"v_Nombre\" as \"NombreComprobante\", "+
                    "d1.\"v_Siglas\" " + concat + " ' ' " + concat + "  ltrim(rtrim(t.\"v_Mes\")) " + concat + " '-' " + concat + " ltrim(rtrim(t.\"v_Correlativo\")) as \"nroComprobante\",  " +
                    "case when(td.\"v_NroDocumento\" is null) then '' else d2.\"v_Siglas\" " + concat + " ' ' " + concat + "  ltrim(rtrim(td.\"v_NroDocumento\")) end as \"docReferencia\",     " +
                    "t.\"v_Nombre\" as \"nombre\", " +
                    "ltrim(rtrim(td.\"v_NroDocumento\")) as SerieCorrelativoDetalle, " + 
                    "case when (td.\"v_Analisis\" is null or td.\"v_Analisis\" = '') then t.\"v_Glosa\" else td.\"v_Analisis\" end as \"descripcionOperacion\", " +
                    "CASE WHEN ((td.\"v_Naturaleza\" = 'D') AND (t.\"i_IdMoneda\" = 1)) AND (1 = " + moneda + ") THEN td.\"d_Importe\" WHEN ((td.\"v_Naturaleza\" = 'D') AND (2 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 1) THEN td.\"d_Importe\" WHEN ((td.\"v_Naturaleza\" = 'D') AND (1 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 2) THEN td.\"d_Cambio\" WHEN ((td.\"v_Naturaleza\" = 'D') AND (2 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 2) THEN td.\"d_Cambio\" WHEN ((td.\"v_Naturaleza\" = 'D') AND (-1 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 1) THEN td.\"d_Importe\" WHEN ((td.\"v_Naturaleza\" = 'D') AND (-1 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 2) THEN td.\"d_Cambio\" ELSE 0 END AS \"debeSoles\"," +
                    "CASE WHEN ((td.\"v_Naturaleza\" = 'D') AND (t.\"i_IdMoneda\" = 2)) AND (2 = " + moneda + ") THEN td.\"d_Importe\" WHEN ((td.\"v_Naturaleza\" = 'D') AND (2 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 1) THEN td.\"d_Cambio\" WHEN ((td.\"v_Naturaleza\" = 'D') AND (1 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 2) THEN td.\"d_Importe\" WHEN ((td.\"v_Naturaleza\" = 'D') AND (1 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 1) THEN td.\"d_Cambio\" WHEN ((td.\"v_Naturaleza\" = 'D') AND (-1 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 1) THEN td.\"d_Cambio\" WHEN ((td.\"v_Naturaleza\" = 'D') AND (-1 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 2) THEN td.\"d_Importe\" ELSE 0 END AS \"debeDolares\", " +
                    "CASE WHEN ((td.\"v_Naturaleza\" = 'H') AND (t.\"i_IdMoneda\" = 1)) AND (1 = " + moneda + ") THEN td.\"d_Importe\" WHEN ((td.\"v_Naturaleza\" = 'H') AND (2 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 1) THEN td.\"d_Importe\" WHEN ((td.\"v_Naturaleza\" = 'H') AND (1 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 2) THEN td.\"d_Cambio\" WHEN ((td.\"v_Naturaleza\" = 'H') AND (2 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 2) THEN td.\"d_Cambio\" WHEN ((td.\"v_Naturaleza\" = 'H') AND (-1 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 1) THEN td.\"d_Importe\" WHEN ((td.\"v_Naturaleza\" = 'H') AND (-1 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 2) THEN td.\"d_Cambio\" ELSE 0 END AS \"haberSoles\",     " +
                    "CASE WHEN ((td.\"v_Naturaleza\" = 'H') AND (t.\"i_IdMoneda\" = 2)) AND (2 = " + moneda + ") THEN td.\"d_Importe\" WHEN ((td.\"v_Naturaleza\" = 'H') AND (2 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 1) THEN td.\"d_Cambio\" WHEN ((td.\"v_Naturaleza\" = 'H') AND (1 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 2) THEN td.\"d_Importe\" WHEN ((td.\"v_Naturaleza\" = 'H') AND (1 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 1) THEN td.\"d_Cambio\" WHEN ((td.\"v_Naturaleza\" = 'H') AND (-1 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 1) THEN td.\"d_Cambio\" WHEN ((td.\"v_Naturaleza\" = 'H') AND (-1 = " + moneda + ")) AND (t.\"i_IdMoneda\" = 2) THEN td.\"d_Importe\" ELSE 0 END AS \"haberDolares\",      " +
                    "' ' as \"cuentaMayor\", " +
                    "case when (a.\"v_NroCuenta\" is null or td.\"v_NroCuenta\" = '') then '*No Existe*' else td.\"v_NroCuenta\" " + concat + " ' - ' " + concat + " a.\"v_NombreCuenta\" end as \"cuenta\",   " +
                    "case when (a.\"v_NroCuenta\" is null or td.\"v_NroCuenta\" = '') then '*No Existe*' else td.\"v_NroCuenta\" end as \"numeroCuenta\",   " +
                    "case when (t.\"i_IdMoneda\" = 1) then 'S/.' else 'US$.' end as \"monedaTransaccion\",  " +
                    "t.\"t_InsertaFecha\" as \"FechaInsercion\", " +
                    "t.\"t_Fecha\" as \"FechaMostrar\",        " +
                    "t.\"v_Mes\" " + concat + " t.\"v_Correlativo\" as \"NroRegistro\",  " +
                    "t.\"i_IdTipoDocumento\" as \"i_CodigoDocumento\", " +
                    "d.\"v_Field\" as \"MonedaSiglas\",    " +
                    "CASE WHEN td.\"v_IdCliente\" IS NULL THEN 0 WHEN td.\"v_IdCliente\" = 'N002-CL000000000' THEN 1 ELSE c.\"i_IdTipoIdentificacion\" END AS \"TipoDocumentoEmisor\", " +
                    "CASE WHEN td.\"v_IdCliente\" IS NULL THEN '0' WHEN td.\"v_IdCliente\" = 'N002-CL000000000' THEN '00000000' ELSE c.\"v_NroDocIdentificacion\" END AS \"NroDocumentoEmisor\",  " +
                    "c.\"v_FlagPantalla\" as \"TipoCliente\", " +
                    "CASE WHEN td.\"i_IdTipoDocumento\" IS NULL THEN -1 ELSE td.\"i_IdTipoDocumento\" END AS \"TipoComprobanteDetalle\",  " +
                    "td.\"t_Fecha\" as \"FechaDetalle\", " +
                    "t.\"v_IdDiario\" as \"IdCabecera\"," +
                    "td.\"v_Naturaleza\" as \"Naturaleza\",  " +
                    "a.\"i_Naturaleza\" as \"NaturalezaCuenta\",  " +
                    "td.\"d_Importe\" as \"Importe\",  " +
                    "c.\"v_IdCliente\" as \"IdAnexo\", " +
                    "td.\"d_Cambio\" as \"Cambio\",  " +
                    "t.\"i_IdTipoComprobante\" as \"TipoComprobante\"  " +
                "from diariodetalle td   " +
                "join diario t on td.\"v_IdDiario\" = t.\"v_IdDiario\"  " +
                "left join documento d1 on t.\"i_IdTipoDocumento\" = d1.\"i_CodigoDocumento\"  " +
                "left join documento d2 on td.\"i_IdTipoDocumento\" = d2.\"i_CodigoDocumento\"  " +
                "left join asientocontable a on td.\"v_NroCuenta\" = a.\"v_NroCuenta\" and a.\"v_Periodo\" = '" + periodo + "' and a.\"i_Eliminado\" = 0 " +
                "left join datahierarchy d on t.\"i_IdMoneda\" = d.\"i_ItemId\" and d.\"i_GroupId\" = 18 and d.\"i_IsDeleted\" = 0 " +
                "left join cliente c on td.\"v_IdCliente\" = c.\"v_IdCliente\" and c.\"i_Eliminado\" = 0 " +
                "where t.\"i_Eliminado\" = 0 and td.\"i_Eliminado\" = 0 " +
                "and t.\"t_Fecha\" >= '" + ini + "' and t.\"t_Fecha\" <= '" + fin + " 23:59' " +
                "and (" + ctaMayor + " is null or td.\"v_NroCuenta\" like " + ctaMayor + ")" +
                "and (" + nroDocumentoIdentidad + " is null or c.\"v_NroDocIdentificacion\" like " + nroDocumentoIdentidad + ")" +
                "and (" + r + " = 0 or td.\"v_NroCuenta\" in (" + rangoCtas + "))";
                #endregion

                switch (motorBd)
                {
                    case TipoMotorBD.MSSQLServer:
                        using (var cnx = new SqlConnection(Globals.CadenaConexion)) 
                        {
                            if (cnx.State == ConnectionState.Closed) cnx.Open();
                            listaRetorno = cnx.Query<ReporteLibroMayor>(query).ToList();
                            cnx.Close();
                        }
                        break;

                    case TipoMotorBD.PostgreSQL:
                        using (var cnx = new PgSqlConnection(Globals.CadenaConexion)) 
                        {
                            if(cnx.State == ConnectionState.Closed) cnx.Open();
                            //!+Se puso en una transacción porque el 
                            //!+datestyle funciona sólo dentro de una transacción o session.
                            var ts = cnx.BeginTransaction();
                            cnx.Execute("SET datestyle = dmy;", transaction: ts);
                            listaRetorno = cnx.Query<ReporteLibroMayor>(query, transaction: ts).ToList();
                            ts.Commit();
                            cnx.Close();
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
                pobjOperationResult.Success = 1;
                return listaRetorno;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ContabilidadDAO.ObtenerDiarios()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        /// <summary>
        /// Obtiene un diccionario temporal de las cuentas mayores.
        /// </summary>
        /// <param name="periodo"></param>
        /// <returns></returns>
        public Dictionary<string, Tuple<string, int>> CuentasMayores(string periodo)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var cuentas =
                    dbContext.asientocontable.Where(p => p.v_Periodo.Equals(periodo) && p.i_Eliminado == 0 && p.v_NroCuenta.Length == 2).ToList()
                        .GroupBy(g => g.v_NroCuenta).Select(s => s.FirstOrDefault())
                        .ToDictionary(k => k.v_NroCuenta, o => new Tuple<string, int>(o.v_NombreCuenta, o.i_Naturaleza?? 1));

                return cuentas;
            }
        }

        /// <summary>
        /// Obtiene la data necesaria producto del cruce de diarios con tesorerias para que alimente al diario mayor.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="fIni"></param>
        /// <param name="fFin"></param>
        /// <param name="ctaMayor"></param>
        /// <param name="rangoCuentas"></param>
        /// <param name="moneda"></param>
        /// <param name="acumuladoAnterior"></param>
        /// <param name="pstrMesSaldoAnterior"></param>
        /// <returns></returns>
        public List<ReporteLibroMayor> 
            ObtenerDataLibroMayor(ref OperationResult pobjOperationResult,
            DateTime fIni, DateTime fFin, string ctaMayor, List<string> rangoCuentas, int moneda, 
            List<saldoscontablesDto> acumuladoAnterior, string pstrMesSaldoAnterior,int FormatoEstructura, List<int> ListaDocumentoResumir)
        {
            try
            {
                List<ReporteLibroMayor> result = new List<ReporteLibroMayor>();
                var dataDiarios = ObtenerDiarios(ref pobjOperationResult, fIni, fFin, ctaMayor, rangoCuentas, moneda, null);
                if (pobjOperationResult.Success == 0) return null;

                dataDiarios.AsParallel().ToList().ForEach(asientos =>
                {
                    asientos.nroComprobante = FormatoEstructura == (int)FormatoEstructuraReporteLibroDiario.Completa ? asientos.nroComprobante : ListaDocumentoResumir.Contains(asientos.i_CodigoDocumento) ? asientos.SiglasComprobante + " " + "00000000" : asientos.nroComprobante;
                    asientos.nombre = FormatoEstructura == (int)FormatoEstructuraLibroMayor.Completa ? asientos.nombre : ListaDocumentoResumir.Contains(asientos.i_CodigoDocumento) ? asientos.NombreComprobante + " " + " DEL MES" : asientos.nombre;
                    asientos.descripcionOperacion = FormatoEstructura == (int)FormatoEstructuraLibroMayor.Completa ? asientos.descripcionOperacion : ListaDocumentoResumir.Contains(asientos.i_CodigoDocumento) ? asientos.NombreComprobante + " " + " DEL MES" : asientos.descripcionOperacion;

                });
               
                var dataTesorerias = ObtenerTesorerias(ref pobjOperationResult, fIni, fFin, ctaMayor, rangoCuentas, moneda, null);
                if (pobjOperationResult.Success == 0) return null;
                dataTesorerias.AsParallel().ToList().ForEach(asientos =>
                {
                    asientos.nroComprobante = FormatoEstructura == (int)FormatoEstructuraLibroMayor.Completa ? asientos.nroComprobante : ListaDocumentoResumir.Contains(-1) ? asientos.SiglasComprobante + " " + "00000000" : asientos.nroComprobante;
                    asientos.nombre = FormatoEstructura == (int)FormatoEstructuraLibroMayor.Completa ? asientos.nombre : ListaDocumentoResumir.Contains(-1) ? asientos.NombreComprobante + " " + " DEL MES" : asientos.nombre;
                    asientos.descripcionOperacion = FormatoEstructura == (int)FormatoEstructuraLibroMayor.Completa ? asientos.descripcionOperacion : ListaDocumentoResumir.Contains(-1) ? asientos.descripcionOperacion + " " + " DEL MES" : asientos.descripcionOperacion;

                });
                

                if (FormatoEstructura == (int)FormatoEstructuraLibroMayor.Resumido)
                {
                    if (ListaDocumentoResumir.Contains(-1))
                    {

                        ListaDocumentoResumir = ListaDocumentoResumir.Select(o => o).Where(o => o != -1).ToList();
                        var Tesorerias = dataTesorerias.GroupBy(o => new { o.nroComprobante, o.cuenta }).Select(d =>
                        {
                            var k = d.FirstOrDefault();
                            k.debeSoles = d.Sum(h => h.debeSoles);
                            k.haberSoles = d.Sum(h => h.haberSoles);
                            k.debeDolares = d.Sum(h => h.debeDolares);
                            k.haberDolares = d.Sum(h => h.haberDolares);
                            return k;
                        }).ToList();

                        var DiariosContenidasLista = dataDiarios.Where(o => ListaDocumentoResumir.Contains(o.i_CodigoDocumento)).ToList();
                        var DiariosNoContenidasLista = dataDiarios.Except(DiariosContenidasLista).ToList();

                        var Diarios = DiariosContenidasLista.GroupBy(o => new { o.nroComprobante, o.cuenta }).Select(d =>
                        {

                            var k = d.FirstOrDefault();
                            k.debeSoles = d.Sum(h => h.debeSoles);
                            k.haberSoles = d.Sum(h => h.haberSoles);
                            k.debeDolares = d.Sum(h => h.debeDolares);
                            k.haberDolares= d.Sum(h => h.haberDolares);
                            return k;
                        }).ToList();




                        //ReporteLibroDiario = Tesorerias.Concat(Diarios).ToList().Concat(DiariosNoContenidasLista).ToList();
                        dataDiarios = Diarios.Concat(DiariosNoContenidasLista);
                        dataTesorerias = Tesorerias;
                        result = dataDiarios.Concat(dataTesorerias).ToList();

                    }
                    else
                    {
                        var ComprasVentasResumidas = dataDiarios.Concat(dataTesorerias).ToList().Where(o => ListaDocumentoResumir.Contains(o.i_CodigoDocumento)).GroupBy(o => new { o.nroComprobante, o.cuenta }).Select(d =>
                        {

                            var k = d.FirstOrDefault();
                            k.debeSoles = d.Sum(h => h.debeSoles);
                            k.haberSoles = d.Sum(h => h.haberSoles);
                            k.debeDolares = d.Sum(h => h.debeDolares);
                            k.haberDolares = d.Sum(h => h.haberDolares);
                            return k;
                        }).ToList();
                        var OtrosDocumentos = dataDiarios.Concat(dataTesorerias).ToList().Where(o => !ListaDocumentoResumir.Contains(o.i_CodigoDocumento)).ToList();
                        result = ComprasVentasResumidas.Concat(OtrosDocumentos).ToList();
                    }
                }
                else
                {

                     result = dataDiarios.Concat(dataTesorerias).ToList();
                }
                var acumuladoDiccionario = acumuladoAnterior.ToDictionary(k => k.v_NroCuenta.Trim(), o => o);

               //var result = dataDiarios.Concat(dataTesorerias).ToList();
                var mayores = CuentasMayores(fIni.Year.ToString());
                foreach (var d in result.AsParallel())
                {
                    Tuple<string, int> tuple;
                    d.cuentaMayor = d.cuenta.Substring(0, 2);
                    var nombreMayor = mayores.TryGetValue(d.cuentaMayor, out tuple) ? tuple.Item1 : "-";
                    d.cuentaMayor += " - " + nombreMayor;
                    d.FechaMostrar = d.FechaMostrar.Trim().Substring(0, 10);
                    saldoscontablesDto a;
                    if (!acumuladoDiccionario.TryGetValue(d.numeroCuenta, out a)) continue;
                    d.acumuladoAnteriorDebeSoles = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? 0 : a == null ? 0 : a.d_ImporteSolesD ?? 0;
                    d.acumuladoAnteriorDebeDolares = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? 0 : a == null ? 0 : a.d_ImporteDolaresD ?? 0;
                    d.acumuladoAnteriorHaberSoles = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? 0 : a == null ? 0 : a.d_ImporteSolesH ?? 0;
                    d.acumuladoAnteriorHaberDolares = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? 0 : a == null ? 0 : a.d_ImporteDolaresH ?? 0;
                    d.saldoAnteriorSoles = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? 0 : a == null ? 0 : (a.d_ImporteSolesD ?? 0) - (a.d_ImporteSolesH ?? 0);
                    d.saldoAnteriorDolares = pstrMesSaldoAnterior == ((int)Mes.Enero).ToString("00") ? 0 : a == null ? 0 : (a.d_ImporteDolaresD ?? 0) - (a.d_ImporteDolaresH ?? 0);
                }

                pobjOperationResult.Success = 1;
                return result;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ContabilidadDAO.ObtenerDataLibroMayor()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public List<ReporteLibroMayor> ObtenerDataLibroMayor(ref OperationResult pobjOperationResult, DateTime fIni,
            DateTime fFin, string ctaMayor, List<string> rangoCuentas, int moneda)
        {
            try
            {
                var dataDiarios = ObtenerDiarios(ref pobjOperationResult, fIni, fFin, ctaMayor, rangoCuentas, moneda, null);
                if (pobjOperationResult.Success == 0) return null;

                var dataTesorerias = ObtenerTesorerias(ref pobjOperationResult, fIni, fFin, ctaMayor, rangoCuentas, moneda, null);
                if (pobjOperationResult.Success == 0) return null;

                var result = dataDiarios.Concat(dataTesorerias).ToList();
                var mayores = CuentasMayores(fIni.Year.ToString());
                foreach (var d in result)
                {
                    Tuple<string, int> tuple;
                    d.cuentaMayor = d.cuenta.Substring(0, 2);
                    var nombreMayor = mayores.TryGetValue(d.cuentaMayor, out tuple) ? tuple.Item1 : "-";
                    d.cuentaMayor += " - " + nombreMayor;
                    d.FechaMostrar = d.FechaMostrar.Trim().Substring(0, 10);
                    d.NaturalezaMayor = mayores.TryGetValue(d.cuenta.Substring(0, 2), out tuple) ? tuple.Item2 : 1; ;
                }

                pobjOperationResult.Success = 1;
                return result;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ContabilidadDAO.ObtenerDataLibroMayor()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }
    }

    public class DiarioSimplificadoBl
    {
        public List<DiarioSimplificadoDto> ObtenerReporteDiarioSimplificado(ref OperationResult pobjOperationResult, DateTime fIni, DateTime fFin, List<string> rangoCuentas)
        {
            try
            {
                var objConta = new ContabilidadDao();
                var data = objConta.ObtenerDataLibroMayor(ref pobjOperationResult, fIni, fFin, null, rangoCuentas, 1);
                
                if (data == null) return new List<DiarioSimplificadoDto>();

                var result = data.Select(d => new DiarioSimplificadoDto
                {
                    Fecha = d.fecha,
                    CodOperacion = d.i_CodigoDocumento  + "-" + d.NroRegistro,
                    Importe = d.Naturaleza.Equals("D") ? d.debeSoles : d.haberSoles,
                    Glosa = d.descripcionOperacion,
                    NroCuenta = d.numeroCuenta,
                    NaturalezaImporte = d.Naturaleza
                }).ToList();

                pobjOperationResult.Success = 1;
                return result;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "DiarioSimplificadoBl.ObtenerReporteDiarioSimplificado()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public class DiarioSimplificadoDto
        {
            public string CodOperacion { get; set; }
            public DateTime Fecha { get; set; }
            public string Glosa { get; set; }
            public string NroCuenta { get; set; }

            public string NaturalezaImporte { get; set; }

            public decimal Importe { get; set; }

            #region Getters
            public string NroCuentaMayor
            {
                get
                {
                    try
                    {
                        return NroCuenta.Substring(0, 2);
                    }
                    catch (Exception)
                    {
                        return "00";
                    }
                }
            }

            public string TipoCta
            {
                get
                {
                    int mayor;
                    mayor = int.TryParse(NroCuentaMayor, out mayor) ? mayor : 0;

                    if (mayor >= 10 && mayor <= 39)
                        return "1. ACTIVOS";

                    if (mayor >= 40 && mayor <= 46)
                        return "2. PASIVOS";

                    if (mayor >= 60 && mayor <= 69)
                        return "3. GASTOS";

                    if (mayor >= 70 && mayor <= 79)
                        return "4. INGRESOS";

                    if (mayor >= 94 && mayor <= 97)
                        return "5. CUENTAS DE FUNCIÓN DEL GASTO";

                    return "INDEFINIDO";
                }
            }
            #endregion
        }

    }
}
