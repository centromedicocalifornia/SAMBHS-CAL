using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    public class ConnectionHelper
    {
        /// <summary>
        /// Obtiene la cadena de conexion del App.Config
        /// </summary>
        private static string ConnectionString
        {
            get
            {
                var csConf = ConfigurationManager.ConnectionStrings["SigesoftConnectionString_Linq"];
                return csConf != null ? csConf.ConnectionString : string.Empty;
            }
        }

        /// <summary>
        /// Obtiene una conexión nativa para consultas rápidas a la bd de Contasol.
        /// </summary>
        public static IDbConnection GetNewSigesoftConnection
        {
            get
            {
                return new SqlConnection(ConnectionString);
            }
        }

        public static IDbConnection GetNewContasolConnection
        {
            get
            {                
                return new SqlConnection(Globals.CadenaConexion);
            }
        }
    }
}
