using SAMBHS.Common.BE.Custom;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public class RecipeManagerDAL
    {
        ConexionSigesoft conecion = new ConexionSigesoft();
        private string sql;
        private SqlCommand comando;
        private SqlDataReader lector;


        internal List<Receta> GetRecetas()
        {
            sql = "select v_ReceipId, v_ServiceId, i_MedicoId, v_MedicoName, d_Total from receipHeader where i_IsDeleted = 0";
            conecion.opensigesoft();
            comando = new SqlCommand(sql, conecion.conectarsigesoft);
            lector = comando.ExecuteReader();
            List<Receta> recetas = new List<Receta>();

            while (lector.Read())
            {
                Receta r = new Receta();
                r.v_ReceiptId = (string) lector.GetValue(0);
                r.v_ServiceId = (string)lector.GetValue(1);
                r.i_MedicoId = (int)lector.GetValue(2);
                r.v_MedicoName = (string)lector.GetValue(3);
                r.d_total = decimal.Parse(lector.GetValue(4).ToString());
                recetas.Add(r);
            }

            return recetas;
        }
        internal List<Receta> GetRecetas_1(string v_ServiceId)
        {
            sql = "select v_ReceipId, v_ServiceId, i_MedicoId, v_MedicoName, d_Total from receipHeader where i_IsDeleted = 0 and v_ServiceId = '" + v_ServiceId + "'";
            conecion.opensigesoft();
            comando = new SqlCommand(sql, conecion.conectarsigesoft);
            lector = comando.ExecuteReader();
            List<Receta> recetas = new List<Receta>();

            while (lector.Read())
            {
                Receta r = new Receta();
                r.v_ReceiptId = (string)lector.GetValue(0);
                r.v_ServiceId = (string)lector.GetValue(1);
                r.i_MedicoId = (int)lector.GetValue(2);
                r.v_MedicoName = (string)lector.GetValue(3);
                r.d_total = decimal.Parse(lector.GetValue(4).ToString());
                recetas.Add(r);
            }
            recetas = recetas.OrderBy(p => p.v_ReceiptId).ToList();

            conecion.closesigesoft();
            comando.Clone();
            lector.Close();

            return recetas;
        }

        public int GetUserId(string UserName)
        {
            int userId = 0;
            ConexionSigesoft conecion = new ConexionSigesoft();
            sql = "";
            conecion.opensigesoft();
            comando = new SqlCommand(sql, conecion.conectarsigesoft);
            lector = comando.ExecuteReader();
            while (lector.Read())
            {
                userId = (int)lector.GetValue(0);
            }

            return userId;
        }
    }
}
