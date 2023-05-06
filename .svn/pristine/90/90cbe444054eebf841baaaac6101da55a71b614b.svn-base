using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.EntityClient;
using System.Linq;

namespace SAMBHS.Common.Resource
{
    public class ConnectionStringManager
    {
        #region Mantenimientos
        public static string GetConnectionString(string connectionStringName)
        {
            var appconfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var connStringSettings = appconfig.ConnectionStrings.ConnectionStrings[connectionStringName];
            return connStringSettings.ConnectionString;
        }

        public static void SaveConnectionString(ref OperationResult pobjOperationResult, string connectionStringName, string connectionString)
        {
            try
            {
                var appconfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                appconfig.ConnectionStrings.ConnectionStrings[connectionStringName].ConnectionString = connectionString;
                appconfig.AppSettings.SectionInformation.ForceSave = true;
                appconfig.Save(ConfigurationSaveMode.Modified, true);
                ConfigurationManager.RefreshSection("connectionStrings");
                pobjOperationResult.Success = 1;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ConnectionStringManager.SaveConnectionString()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
            }
        }

        public static List<string> GetConnectionStringNames()
        {
            var appconfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            return (from ConnectionStringSettings cn in appconfig.ConnectionStrings.ConnectionStrings select cn.Name).ToList();
        }
        #endregion

        #region Modificaciones
        public static string SetConnectionStringDatabaseName(string connectionString, string databaseName)
        {
            var builder = new EntityConnectionStringBuilder(connectionString)
            {
                ProviderConnectionString = databaseName
            };

            return builder.ConnectionString;
        }
        #endregion
    }
}
