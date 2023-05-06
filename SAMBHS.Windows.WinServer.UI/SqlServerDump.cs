using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using SAMBHS.Common.BL;
using Dapper;
using SAMBHS.Common.Resource;
namespace SAMBHS.Windows.WinServer.UI
{
    public class SqlServerDump
    {
        #region Construct
        private SqlServerDump() { }
        #endregion

        #region Public Method
        public static void Backup(string pstrNameDatabase, string pstrPath)
        {
            try
            {
                using (var SqlCon = new SqlConnection(LocalGlobals.ConnectionString))
                {
                    SqlCon.Execute("USE [" + pstrNameDatabase  + "];");
                    var query = string.Format("BACKUP DATABASE [{0}] TO DISK = '{1}' WITH INIT, NAME = N'{0}-Full Database Backup', SKIP;", pstrNameDatabase, pstrPath);
                    SqlCon.Execute(query);
                }
            }
            catch
            {
                
            }
        }
        
        public static bool DropDB(string databaseName, int nodeID)
        {
            try
            {
                using (var SqlCon = new SqlConnection(LocalGlobals.ConnectionString))
                {
                    var CierraConexiones = "USE MASTER; ALTER DATABASE [" + databaseName + "] SET OFFLINE WITH ROLLBACK IMMEDIATE;";
                    var OpenDB = "ALTER DATABASE [" + databaseName + "] SET ONLINE WITH ROLLBACK IMMEDIATE;";
                    var EliminarBaseDatos = string.Format("DROP DATABASE [{0}];", databaseName);

                    SqlCon.Execute(CierraConexiones);
                    SqlCon.Execute(OpenDB);
                    SqlCon.Execute(EliminarBaseDatos);
                    if (nodeID != -1)
                    {
                        NodeBL objNodeBL = new NodeBL();
                        OperationResult objOperationResult = new OperationResult();
                        objNodeBL.DeleteNode(ref objOperationResult, nodeID, Globals.ClientSession.GetAsList());

                        if (objOperationResult.Success == 0) return false;
                    }
                    return true;
                }
            }
            catch
            {
                return false;
            }

        }

        public static bool RestoreDB(string BackupFilePath, string strDatabaseName)
        {
            if (BackupFilePath == string.Empty)
            {
                throw new Exception("Ingrese la ruta del archivo .backup");
            }
            try
            {
                using (var SqlCon = new SqlConnection(LocalGlobals.ConnectionString))
                {

                    var result = SqlCon.ExecuteReader("RESTORE FILELISTONLY FROM DISK=N'" + BackupFilePath + "'");
                    result.Read();
                    var nameDbBackup = result.GetString(0);
                    result.Read();
                    var nameLogBackup = result.GetString(0);
                    result.Close();
                    result = SqlCon.ExecuteReader(@"USE [" + strDatabaseName + "]; select physical_name from sys.database_files;");
                    result.Read();
                    var PathDB = result.GetString(0);
                    result.Read();
                    var PathDBLog = result.GetString(0);
                    result.Close();

                    string FullQuery = string.Format(@"USE MASTER; GO ALTER DATABASE [{0}] SET OFFLINE WITH ROLLBACK IMMEDIATE; GO ALTER DATABASE [{0}] SET ONLINE WITH ROLLBACK IMMEDIATE; GO ", strDatabaseName);
                    FullQuery += string.Format(@"USE master; GO RESTORE DATABASE [{0}] FROM DISK=N'{1}' WITH MOVE '{2}' TO '{3}', MOVE '{4}' TO '{5}', REPLACE,RECOVERY", strDatabaseName, BackupFilePath, nameDbBackup, PathDB, nameLogBackup, PathDBLog);
                    var queries = GetQuerys(FullQuery);
                    foreach (var query in queries)
                    {
                        SqlCon.Execute(query);
                    }
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool CreateDB(string Empresa)
        {
            try
            {
                using (var SqlCon = new SqlConnection(LocalGlobals.ConnectionString))
                {
                    var queryCrearDb = @"CREATE DATABASE [" + Empresa + "] COLLATE Modern_Spanish_CI_AS;";
                    SqlCon.Execute("USE master;");
                    SqlCon.Execute(queryCrearDb);
                    return true;
                }
            }
            catch
            {
                return false;
            }

        }

        private static string[] GetQuerys(string pstrQueryExtend)
        {
            return pstrQueryExtend.Split(new[] { " GO " }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static bool IsServerLocal()
        {
            SqlConnectionStringBuilder con = new SqlConnectionStringBuilder(LocalGlobals.ConnectionString);
            if (con.DataSource.StartsWith(".")) return true;
            if (con.DataSource.ToUpper().StartsWith(System.Environment.MachineName.ToUpper()))
                return true;
            return true;
        }
        #endregion

       
    }
}
