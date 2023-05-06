using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;

namespace SAMBHS.Requerimientos.NBS
{
    /// <summary>
    /// Clase que sirve para establecer una conexion a los dbfs del sistema notarial para consultar los datos del kardex
    /// </summary>
    public class DbfConnector
    {
        public delegate void OnError(string error);
        /// <summary>
        /// Evento disparado cuando sucede una excepción.
        /// </summary>
        public event OnError ErrorEvent;

        private string _dbfDataPath;
        private string _dbfDataClientPath;
        private string _dbfDataFilename;
        private string _dbfDataClientFilename;

        /// <summary>
        /// Ruta del dbf de los kardex llamada 'datos'
        /// </summary>
        public string DataPath
        {
            set { _dbfDataPath = value; }
        }

        /// <summary>
        /// Ruta del dbf donde esta la relacion cliente_kardex llamada ***
        /// </summary>
        public string DataClientPath
        {
            set { _dbfDataClientPath = value; }
        }

        /// <summary>
        /// Nombre del dbf
        /// </summary>
        public string DataFileName
        {
            set { _dbfDataFilename = value; }
        }

        /// <summary>
        /// Nombre del dbf
        /// </summary>
        public string DataClientFileName
        {
            set { _dbfDataClientFilename = value; }
        }

        /// <summary>
        /// Llama a los métodos que establecen la conexion y retornan la lista cliente_kardex
        /// </summary>
        /// <param name="nroKardex"></param>
        /// <param name="tipoKardex"></param>
        /// <returns></returns>
        public IEnumerable<KardexClienteDto> ObtenerInfo(string nroKardex, string tipoKardex)
        {
            try
            {
                var kardexInfo = ObtieneDatoKardex(nroKardex, tipoKardex);
                if (kardexInfo != null)
                {
                    var result = ObtieneDatosContrato(nroKardex, tipoKardex);
                    if (result != null)
                    {
                        return result;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                if (ErrorEvent != null)
                    ErrorEvent(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Revisa si el kardex ingresado existe, sino retorna nulo.
        /// </summary>
        /// <param name="nroKardex"></param>
        /// <param name="tipoKardex"></param>
        /// <returns></returns>
        private KardexDto ObtieneDatoKardex(string nroKardex, string tipoKardex)
        {
            try
            {
                var yourResultSet = new DataTable();

                var datosConnection = new OleDbConnection(
                    string.Format("Provider=VFPOLEDB.1;Data Source={0};", _dbfDataPath));

                datosConnection.Open();

                if (datosConnection.State == ConnectionState.Open)
                {
                    var da = new OleDbDataAdapter();
                    var mySql = "select * from " + _dbfDataFilename +
                                @" where kardex = '" + nroKardex + "' and sufkar = '" + tipoKardex + "'";

                    var myQuery = new OleDbCommand(mySql, datosConnection);
                    da.SelectCommand = myQuery;
                    da.Fill(yourResultSet);
                    datosConnection.Close();
                }

                var result = yourResultSet.AsEnumerable()
                    .Select(p => new KardexDto
                    {
                        Kardex = p.Field<string>("kardex").Trim(),
                        TipoKardex = p.Field<string>("sufkar").Trim(),
                        Responsable = p.Field<string>("resp_not").Trim()
                    }).SingleOrDefault();

                return result;
            }
            catch (Exception ex)
            {
                if (ErrorEvent != null)
                    ErrorEvent(ex.Message);
                return null;
            }

        }

        /// <summary>
        /// Si existe el kardex busca en la siguiente tabla para traer la relacion de clientes de ese kardex.
        /// </summary>
        /// <param name="nroKardex"></param>
        /// <param name="tipoKardex"></param>
        /// <returns></returns>
        private IEnumerable<KardexClienteDto> ObtieneDatosContrato(string nroKardex, string tipoKardex)
        {
            try
            {
                var yourResultSet = new DataTable();

                var datosConnection = new OleDbConnection(
                    string.Format("Provider=VFPOLEDB.1;Data Source={0};", _dbfDataClientPath));

                datosConnection.Open();

                if (datosConnection.State == ConnectionState.Open)
                {
                    var da = new OleDbDataAdapter();
                    var mySql = "select * from " + _dbfDataClientFilename +
                         @" where kardex = '" + nroKardex + "' and sufkar = '" + tipoKardex + "'";

                    var myQuery = new OleDbCommand(mySql, datosConnection);
                    da.SelectCommand = myQuery;
                    da.Fill(yourResultSet);

                    da.InsertCommand = new OleDbCommand("insert into kardex, sufkar, tipper, codped, nomper values ('001', 'X', 12,'edu','eduardo quiroz')", datosConnection);

                    datosConnection.Close();
                }

                var result = yourResultSet.AsEnumerable()
                    .Select(p => new KardexClienteDto
                    {
                        Kardex = p.Field<string>("kardex").Trim(),
                        TipoKardex = p.Field<string>("sufkar").Trim(),
                        NroDocumento = p.Field<string>("tipper").Trim().Equals("N") ? p.Field<string>("dni").Trim() : p.Field<string>("ruc").Trim(),
                        Codigo = p.Field<string>("codper").Trim(),
                        NombreApellidosRazonSocial = p.Field<string>("nomper").Trim(),
                        TipoPersona = p.Field<string>("tipper").Trim(),
                        Direccion = p.Field<string>("domicilio").Trim()
                    });

                return result;
            }
            catch (Exception ex)
            {
                if (ErrorEvent != null)
                    ErrorEvent(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Entidad que almacena los datos del kardex buscado.
        /// </summary>
        public class KardexDto
        {
            public string Kardex { get; set; }
            public string TipoKardex { get; set; }
            public string Responsable { get; set; }
        }

        /// <summary>
        /// Entidad que almacena el listado de clientes del kardex buscado
        /// </summary>
        public class KardexClienteDto
        {
            public string Kardex { get; set; }
            public string TipoKardex { get; set; }
            public string NroDocumento { get; set; }
            public string TipoPersona { get; set; }
            public string Codigo { get; set; }
            public string NombreApellidosRazonSocial { get; set; }
            public string Direccion { get; set; }
        }

        /// <summary>
        /// Inicializador del evento error.
        /// </summary>
        /// <param name="error"></param>
        protected virtual void OnErrorEvent(string error)
        {
            var handler = ErrorEvent;
            if (handler != null) handler(error);
        }
    }
}
