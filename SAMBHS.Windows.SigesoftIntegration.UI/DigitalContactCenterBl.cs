using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using Dapper;
using Infragistics.Win.UltraWinEditors;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.Windows.SigesoftIntegration.UI.Dtos;
using System.Data;
using System.Transactions;
using SAMBHS.Windows.SigesoftIntegration.UI.Reports;
using SAMBHS.Common.BE.Custom;
using SAMBHS.Common.DataModel;

namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    public class DigitalContactCenterBl
    {
        public static List<DigitalContactCenterDto> ObtenerDigitalContactCenterLista(DateTime? FechaInicio, DateTime? FechaFin)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                var query = "exec getdigitalcontactcenterNew_sp '" + FechaInicio + "', '" + FechaFin + "'";

                var data = cnx.Query<DigitalContactCenterDto>(query).ToList();
                return data;
            }

        }

        public static string UpdateCalendar(int _estado, string _comentarios, int _userId, string v_DigitalContact)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                var query = "update DigitalContactCenter set i_EstadoAtencion = '" + _estado + "', v_Comentarios = '" + _comentarios + "', i_UpdateUserId = '" + _userId + "', d_UpdateDate = '" + DateTime.Now + "' where v_DigitalContactCenterId = '" + v_DigitalContact + "'";

                cnx.Execute(query);

                return v_DigitalContact;


            }
        }

        public static bool CancelarCita(int userId, string v_DigitalContact)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var searchDCC = "select  v_ServiceId as v_ServiceId from DigitalContactCenter where v_DigitalContactCenterId = '" + v_DigitalContact + "'";
                    var firstOrDefaultDCC = cnx.Query<DigitalContactCenterDtoNew>(searchDCC).FirstOrDefault();

                    if (firstOrDefaultDCC != null)
                    {
                        var query = "update calendar set " +
                                    " i_LineStatusId = " + (int)LineStatus.FueraCircuito +
                                    ", i_CalendarStatusId = " + (int)CalendarStatus.Cancelado +
                                    ", i_UpdateUserId = " + userId +
                                    ", d_UpdateDate = '" + DateTime.Now + "'" +
                                    "where v_ServiceId = '" + firstOrDefaultDCC.v_ServiceId + "'";

                        cnx.Execute(query);

                        var query2 = "update service set " +
                                     " i_IsDeleted = 1" +
                                     ", i_UpdateUserId = " + userId +
                                     ", d_UpdateDate = '" + DateTime.Now + "'" +
                                     "where v_ServiceId = '" + firstOrDefaultDCC.v_ServiceId + "'";
                        cnx.Execute(query2);
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                   
                    //var queryDigital = "update DigitalContactCenter set i_EstadoAtencion = '5' where v_ServiceId = '" + serviceId + "'";
                    //cnx.Execute(queryDigital);

                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
