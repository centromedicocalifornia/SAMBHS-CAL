using SAMBHS.Common.Resource;
using System;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            var objOperationResult = new OperationResult();
            try
            {
                #region Cadena Conexión.
                var providerString = string.Empty;

                switch (UserConfig.Default.csTipoMotorBD)
                {
                    case TipoMotorBD.MSSQLServer:
                        providerString = "Data Source=" + UserConfig.Default.csServidor + ";Initial Catalog=" + "TIS_INTEGRADO" + ";Integrated Security=False;Persist Security Info=True;User ID=" + UserConfig.Default.csUsuario + ";Password=" + Crypto.DecryptStringAES(UserConfig.Default.csPassword, "TiSolUciOnEs") + "";
                        break;

                    case TipoMotorBD.PostgreSQL:
                        providerString = "User Id=" + UserConfig.Default.csUsuario + "; password=" + Crypto.DecryptStringAES(UserConfig.Default.csPassword, "TiSolUciOnEs") + ";Host=" + UserConfig.Default.csServidor + ";Database=" + "TIS_INTEGRADO" + ";Initial Schema=public";
                        break;
                }

                var cs = ConnectionStringManager.GetConnectionString("SAMBHSConnectionString");
                var newConStr = ConnectionStringManager.SetConnectionStringDatabaseName(cs, providerString);
                ConnectionStringManager.SaveConnectionString(ref objOperationResult, "SAMBHSConnectionString", newConStr);

                #endregion
            }
            catch
            {
                var cnx = new frmPreferenciasConexion();
                cnx.ShowDialog();
                if (cnx.Guardado) Application.Restart();
                return;
            }

            var dataSource = UserConfig.Default.csServidor;

            if (dataSource.Contains(@"\"))
            {
                var indexSlash = dataSource.IndexOf(@"\", StringComparison.Ordinal);
                dataSource = dataSource.Substring(0, indexSlash);
            }
            
            TransactionUtils.OverrideTransactionScopeMaximumTimeout(TimeSpan.FromHours(2));
            Infragistics.Win.AppStyling.StyleManager.Load(@"Style.isl");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmLogin());
            //test
        }
    }
}
