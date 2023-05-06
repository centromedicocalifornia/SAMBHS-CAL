using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.Windows.WinClient.UI;

namespace SAMBHS.Windows.WinServer.UI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            OperationResult objOperationResult = new OperationResult();
            try
            {
                #region Cadena Conexión.
                string providerString = string.Empty;

                switch (UserConfig.Default.csTipoMotorBD)
                {
                    case TipoMotorBD.MSSQLServer:
                        providerString = "Data Source=" + UserConfig.Default.csServidor + ";Initial Catalog=" + "TIS_INTEGRADO" + ";Integrated Security=False;Persist Security Info=True;User ID=" + UserConfig.Default.csUsuario + ";Password=" + Crypto.DecryptStringAES(UserConfig.Default.csPassword, "TiSolUciOnEs") + "";
                        break;

                    case TipoMotorBD.PostgreSQL:
                        providerString = "User Id=" + UserConfig.Default.csUsuario + "; password=" + Crypto.DecryptStringAES(UserConfig.Default.csPassword, "TiSolUciOnEs") + ";Host=" + UserConfig.Default.csServidor + ";Database=" + "TIS_INTEGRADO" + ";Initial Schema=public";
                        break;
                }

                string cs = ConnectionStringManager.GetConnectionString("SAMBHSConnectionString");
                string newConStr = ConnectionStringManager.SetConnectionStringDatabaseName(cs, providerString);
                ConnectionStringManager.SaveConnectionString(ref objOperationResult, "SAMBHSConnectionString", newConStr);
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            string DataSource = UserConfig.Default.csServidor;

            if (DataSource.Contains(@"\"))
            {
                int IndexSlash = DataSource.IndexOf(@"\");
                DataSource = DataSource.Substring(0, IndexSlash);
            }

            if (Utils.PingNetwork(DataSource, 500, 10))
            {
            Infragistics.Win.AppStyling.StyleManager.Load(@"Style.isl");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmLogin());
            }
            else
            {
                var Mensaje = MessageBox.Show("No se pudo establecer una conexión al servidor. ¿Desea Editar la Configuración de Conexión?", "Error de conexión", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (Mensaje == DialogResult.Yes)
                {
                    frmPreferenciasConexion cnx = new frmPreferenciasConexion();
                    cnx.ShowDialog();
                }
            }

        }
    }
}
