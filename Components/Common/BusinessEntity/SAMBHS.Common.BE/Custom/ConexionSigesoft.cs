﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAMBHS.Common.BE.Custom
{
    public class ConexionSigesoft
    {
        string cadena = GetApplicationConfigValue("ConexionSigesoft").ToString();

        public static object GetApplicationConfigValue(string appkey)
        {
            return Convert.ToString(ConfigurationManager.AppSettings[appkey]);
        }
        public SqlConnection conectarsigesoft = new SqlConnection();
        public ConexionSigesoft()
        {
            conectarsigesoft.ConnectionString = cadena;
        }
        public void opensigesoft()
        {
            try
            {
                conectarsigesoft.Open();
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error al abrir la BD Sigesoft" + ex.Message, "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        public void closesigesoft()
        {
            conectarsigesoft.Close();
        }
    }
}
