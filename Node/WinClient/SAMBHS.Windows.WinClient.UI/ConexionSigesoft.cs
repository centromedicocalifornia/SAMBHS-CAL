using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI
{
    public class ConexionSigesoft
    {
        string cadena;
        public SqlConnection conectarsigesoft = new SqlConnection();
        public ConexionSigesoft()
        {
            cadena = GetApplicationConfigValue("ConexionSigesoft");

            conectarsigesoft.ConnectionString = cadena;
        }

        private string GetApplicationConfigValue(string nombre)
        {
            return Convert.ToString(ConfigurationManager.AppSettings[nombre]);
        }
        public void opensigesoft()
        {
            try
            {
                conectarsigesoft.Open();
            }
            catch (Exception ex)
            {

                MessageBox.Show(@"Error al abrir la BD Sigesoft" + ex.Message, @"Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        public void closesigesoft()
        {
            conectarsigesoft.Close();
        }
    }
}
