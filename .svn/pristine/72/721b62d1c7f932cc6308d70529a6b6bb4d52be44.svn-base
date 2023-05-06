using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Forms;


namespace SAMBHS.Common.Resource
{
    public class ResultStatus
    {
        public int Id { get; set; }
        public string NameSuscriptor { get; set; }
        public DateTime Time { get; set; }
        public string Comentario { get; set; }
        public bool Status { get; set; }
    }

    public class ReplicationStatus
    {
        private static string GetConexionString()
        {
            var strConexion = System.Configuration.ConfigurationManager.ConnectionStrings["SAMBHSConnectionStringWin"].ConnectionString;
            strConexion = strConexion.Substring(strConexion.IndexOf("connection string=", StringComparison.Ordinal) + "connection string=".Length).Replace("\"", "");
            return strConexion;
        }
        public static List<ResultStatus> GetStatus()
        {
            var listResult = new List<ResultStatus>();
            try
            {

                using (var con = new SqlConnection(GetConexionString()))
                {
                    var result = con.Query<AgentsMerge>(@"USE distribution; select a.id as [Id], a.subscriber_name as [NameSusc], MAX(h.time) as [LastTime] from MSmerge_agents as a 
                                                            join MSmerge_history as h on a.id = h.agent_id
                                                            where a.publisher_db != 'TIS_INTEGRADO'
                                                            group by a.id, a.subscriber_name").ToList();
                    result.ForEach(r =>
                    {
                        var item = new ResultStatus
                        {
                            Id = r.Id,
                            NameSuscriptor = r.NameSusc,
                            Time = r.LastTime
                        };
                        System.Diagnostics.Debug.WriteLine(r.LastTime.ToString(CultureInfo.InvariantCulture));
                        var res = con.ExecuteReader(@"USE distribution; SELECT h.comments ,e.error_code FROM MSmerge_history h LEFT OUTER JOIN MSrepl_errors e ON h.error_id = e.id where h.time = '" + r.LastTime.ToString("yyyy-MM-dd HH:mm:ss.FFF") + "' order by h.time desc;");
                        res.Read();
                        item.Comentario = res.GetString(0);
                        var code = res.GetValue(1) as string;
                        item.Status = string.IsNullOrEmpty(code);
                        res.Close();
                        listResult.Add(item);
                        if (code == "-2147199402")
                        {
                            if (Application.OpenForms["ConexionPerdida"] != null) return;
                            var f = new ConexionPerdida(r.NameSusc);
                            f.Show();
                        }
                    });
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return listResult;
        }

        public static List<HistorialSubscritor> GetHistorial(int pintId, int pintCantidad)
        {
            var res = new List<HistorialSubscritor>();
            try
            {
                using (var con = new SqlConnection(GetConexionString()))
                {
                    res = con.Query<HistorialSubscritor>("" +
                                                         "USE distribution; " +
                                                         "SELECT Top(" + pintCantidad + ") comments as [Contenido],error_id as [ErrorCode],time as [Fecha] FROM MSmerge_history " +
                                                         "WHERE agent_id = " + pintId + " order by time desc").ToList();
                }
            }
            catch (Exception)
            {

            }
            return res;
        }
        /// <summary>
        /// Clase para Captar el historial de un agente subscriptor
        /// </summary>
        public class HistorialSubscritor
        {
            public string Contenido { get; set; }
            public int ErrorCode { get; set; }
            public DateTime Fecha { get; set; }
        }

        class AgentsMerge
        {
            public int Id { get; set; }
            public string NameSusc { get; set; }
            public DateTime LastTime { get; set; }
        }
    }


}
