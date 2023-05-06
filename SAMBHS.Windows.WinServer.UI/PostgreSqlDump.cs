using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Dapper;
using Devart.Data.PostgreSql;
using SAMBHS.Common.Resource;
using SAMBHS.Common.BL;

namespace SAMBHS.Windows.WinServer.UI
{
    public class PostgreSqlDump
    {
        /// <summary>
        /// Metodo para Crear backups en postgres.
        /// </summary>
        /// <param name="pgDumpPath">Ruta del Archivo pg_dump.exe</param>
        /// <param name="outFile">Ruta del archivo de salida.</param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="database"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        public static void BackupBD(
        string pgDumpPath,
        string outFile,
        string host,
        string port,
        string database,
        string user,
        string password)
        {
            SetPassword();
            String dumpCommand = "\"" + pgDumpPath + "\"" + " -h " + host + " -p " + port + " -U " + user + " -F c -b -v -f " + outFile + " " + database;
            //String dumpCommand = "\"" + pgDumpPath + "\"" + " -Fc" + " -h " + host + " -p " + port + " -d " + database + " -U " + user + "";
            String passFileContent = "" + host + ":" + port + ":" + database + ":" + user + ":" + password + "";

            String batFilePath = Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid().ToString() + ".bat");

            String passFilePath = Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid().ToString() + ".conf");

            try
            {
                String batchContent = "";
                batchContent += "@" + "set PGPASSFILE=" + passFilePath + "\n";
                batchContent += "@" + dumpCommand;

                File.WriteAllText(
                    batFilePath,
                    batchContent,
                    Encoding.ASCII);

                File.WriteAllText(
                    passFilePath,
                    passFileContent,
                    Encoding.ASCII);

                ProcessStartInfo oInfo = new ProcessStartInfo(batFilePath);
                oInfo.UseShellExecute = false;
                oInfo.CreateNoWindow = true;

                using (Process proc = System.Diagnostics.Process.Start(oInfo))
                {
                    proc.WaitForExit();
                    proc.Close();
                }
            }
            finally
            {
                if (File.Exists(batFilePath))
                    File.Delete(batFilePath);
            }
        }

        /// <summary>
        /// Restaura la base de datos seleccionada por medio de un backup.
        /// </summary>
        /// <param name="BackupFilePath">La ruta del archivo .backup</param>
        /// <param name="strDatabaseName">El nombre de la base de datos</param>
        /// <param name="strPG_dumpPath">La ruta de la carpeta bin del postgres.</param>
        /// <param name="strServer">El servidor de la base de datos</param>
        /// <param name="strPort">El puerto de instalación.</param>
        /// <returns></returns>
        public static bool RestoreDB(string BackupFilePath, string strDatabaseName, string strPG_dumpPath, string strServer, string strPort)
        {
            try
            {
                if (BackupFilePath == string.Empty)
                {
                    throw new Exception("Ingrese la ruta del archivo .backup");
                }
                //check for the pre-requisites before restoring the database.*********
                if (strDatabaseName != "")
                {
                    if (BackupFilePath != "")
                    {
                        SetPassword();
                        StreamWriter sw = new StreamWriter("DBRestore.bat");
                        // Do not change lines / spaces b/w words.
                        StringBuilder strSB = new StringBuilder("\"" + strPG_dumpPath);
                        if (strSB.Length != 0)
                        {
                            strSB.Append("pg_restore.exe" + "\"" + " -i -h " + strServer +
                               " -p " + strPort + " -U postgres -d ");
                            strSB.Append(strDatabaseName);
                            strSB.Append(" -v ");
                            strSB.Append("\"" + BackupFilePath + "\"");
                            sw.WriteLine(strSB);
                            sw.Dispose();
                            sw.Close();
                            Process processDB = Process.Start("DBRestore.bat");
                            do
                            {//dont perform anything
                            }
                            while (!processDB.HasExited);
                            {
                                return true;
                            }
                        }
                        else
                        {
                            throw new Exception("Por favor ingrese la ruta de la carpeta del archivo bin de postgres.");
                        }
                    }
                }
                else
                {
                    throw new Exception("Por favor ingrese el nombre de la base de datos a restaurar");
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Elimina la base de datos de manera forzada, cerrando las conexiones a esta primero y luego eliminando.
        /// </summary>
        /// <param name="databaseName"></param>
        public static bool DropDB(string databaseName, int nodeID)
        {
            try
            {
                using (PgSqlConnection Cnx = new PgSqlConnection(LocalGlobals.ConnectionString))
                {
                    var cierraConexiones = string.Format("update pg_database set datallowconn = 'true' where datname = '{0}';", databaseName);
                    var terminaLasConexionesActuales = string.Format("SELECT pg_terminate_backend(procpid) FROM pg_stat_activity WHERE datname = '{0}';", databaseName);
                    var eliminarBaseDatos = @"Drop Database " + "\"" + databaseName + "\"" + ";";

                    Cnx.Execute(cierraConexiones);
                    Cnx.Execute(terminaLasConexionesActuales);
                    Cnx.Execute(eliminarBaseDatos);

                    if (nodeID != -1)
                    {
                        var objNodeBl = new NodeBL();
                        var objOperationResult = new OperationResult();
                        objNodeBl.DeleteNode(ref objOperationResult, nodeID, Globals.ClientSession.GetAsList());

                        if (objOperationResult.Success == 0) return false;
                    }
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Crea una base de datos en postgres, la coloca en Latino y reordena la secuencial de node para la creacion de la empresa.
        /// </summary>
        /// <param name="Empresa"></param>
        /// <returns></returns>
        public static bool CreateDB(string Empresa)
        {
            using (PgSqlConnection Cnx = new PgSqlConnection(LocalGlobals.ConnectionString))
            {
                try
                {
                    var queryCrearDB = @"CREATE DATABASE " + "\"" + Empresa + "\"" + " WITH OWNER = postgres " +
                    "ENCODING = 'UTF8' TABLESPACE = pg_default CONNECTION LIMIT = -1;";
                    Cnx.Execute(queryCrearDB);

                    var querySpanish = @"update pg_database set encoding=8 where datname='" + Empresa + "';";
                    Cnx.Execute(querySpanish);

                    var queryReorderSecuential = "ALTER SEQUENCE " + "\"" + "node_i_NodeId_seq" + "\"" + " RESTART WITH " + new NodeBL().GetNextNodeId() + ";";
                    Cnx.Execute(queryReorderSecuential);

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static void SetPassword()
        {
            string subPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\postgresql"; // your code goes here
            if (!Directory.Exists(subPath)) Directory.CreateDirectory(subPath);
            var passFileContent = "localhost:5432:*:postgres:" + Crypto.DecryptStringAES(UserConfig.Default.csPassword, "TiSolUciOnEs");
            var passFilePath = subPath + @"\pgpass.conf";
            File.WriteAllText(passFilePath, passFileContent, Encoding.ASCII);
        }
    }
}
