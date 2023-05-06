using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using SAMBHS.Common.BE.Custom;
using SAMBHS.Windows.SigesoftIntegration.UI.Dtos;
using SAMBHS.Common.Resource;

namespace SAMBHS.Windows.SigesoftIntegration.UI.BLL
{
    public class CalendarBL
    {
        public static bool CircuitStart(string calendarId, DateTime FechaInicio, int userId, int modality, int serviceStatusId)
        {
            try
            {
                CalendarList objCalendar = new CalendarList();
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var updateQuery = "update calendar set " +
                                " i_LineStatusId = " + (int)LineStatus.EnCircuito +
                                ", i_CalendarStatusId = " + (int)CalendarStatus.Atendido +
                                ", d_CircuitStartDate = '" + FechaInicio.ToString() + "'" +
                                ", d_EntryTimeCM = '" + FechaInicio.ToString() + "'" +
                                ", i_UpdateUserId = " + userId +
                                ", d_UpdateDate = '" + DateTime.Now + "'" +
                                "where v_CalendarId = '" + calendarId + "'";
                    cnx.Execute(updateQuery);
                    
                    var queryService = "select * from calendar where v_CalendarId  = '" + calendarId + "'";
                    var objCal = cnx.Query<CalendarList>(queryService).FirstOrDefault();

                    UpdateServiceForCalendar(objCal.v_ServiceId, FechaInicio, userId, modality, serviceStatusId);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool CancelAtx(string calendarId, int userId)
        {
            try
            {
                CalendarList objCalendar = new CalendarList();
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var updateQuery = "update calendar set " +
                                      " i_LineStatusId = " + (int)LineStatus.FueraCircuito +
                                      ", i_UpdateUserId = " + userId +
                                      ", d_UpdateDate = '" + DateTime.Now + "'" +
                                      ", i_IsDeleted = 1" +
                                      "where v_CalendarId = '" + calendarId + "'";
                    cnx.Execute(updateQuery);

                    var queryService = "select * from calendar where v_CalendarId  = '" + calendarId + "'";
                    var objCal = cnx.Query<CalendarList>(queryService).FirstOrDefault();

                    CancelService(objCal.v_ServiceId, userId);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool CancelSchedule(string calendarId, int userId)
        {
            try
            {
                CalendarList objCalendar = new CalendarList();
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var updateQuery = "update calendar set " +
                                      " i_LineStatusId = " + (int)LineStatus.FueraCircuito +
                                      ", i_CalendarStatusId = " + (int)CalendarStatus.Cancelado +
                                      ", i_UpdateUserId = " + userId +
                                      ", d_UpdateDate = '" + DateTime.Now + "'" +
                                      "where v_CalendarId = '" + calendarId + "'";
                    cnx.Execute(updateQuery);

                    var queryService = "select * from calendar where v_CalendarId  = '" + calendarId + "'";
                    var objCal = cnx.Query<CalendarList>(queryService).FirstOrDefault();

                    CancelService(objCal.v_ServiceId, userId);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static void CancelService(string serviceId, int userId)
        {
            try
            {

                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {

                    var updateQuery = "update service set " +
                                      " i_IsDeleted = 1" +
                                      ", i_UpdateUserId = " + userId +
                                      ", d_UpdateDate = '" + DateTime.Now + "'" +
                                      "where v_ServiceId = '" + serviceId + "'";
                    cnx.Execute(updateQuery);
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static void UpdateServiceForCalendar(string serviceId, DateTime FechaInicio, int userId, int modalidad, int serviceStatusId)
        {
            try
            {

                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var queryService = "select * from service where v_ServiceId = '" + serviceId + "'";
                    var objService = cnx.Query<ServiceBE>(queryService).FirstOrDefault();

                    if (modalidad == (int)modality.NuevoServicio)
                    {
                        objService.i_ServiceStatusId = (int)ServiceStatus.Iniciado;
                    }
                    else if (modalidad == (int)modality.ContinuacionServicio)
                    {
                        objService.i_ServiceStatusId = serviceStatusId;
                    }




                    var updateQuery = "update service set " +
                                      " d_ServiceDate = '" + FechaInicio + "'" +
                                      ", i_UpdateUserId = " + userId +
                                      ", i_ServiceStatusId = " + objService.i_ServiceStatusId +
                                      ", d_UpdateDate = '" + DateTime.Now + "'" +
                                      "where v_ServiceId = '" + serviceId + "'";
                    cnx.Execute(updateQuery);

                    var queryDigital = "update DigitalContactCenter set i_EstadoAtencion = '3' where v_ServiceId = '" + serviceId + "'";
                    cnx.Execute(queryDigital);

                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public int GetTypeSchedule(string calendarId)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var queryCalendar = "select * from calendar where v_CalendarId = '" + calendarId + "'";
                    var objCalendar = cnx.Query<CalendarList>(queryCalendar).FirstOrDefault();
                    return objCalendar.i_ServiceTypeId;
                }


            }
            catch (Exception e)
            {
                return -1;
            }
        }

        public ServiceBE GetDataservice(string serviceId)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var queryService = "select * from service where v_ServiceId = '" + serviceId + "'";
                    var objService = cnx.Query<ServiceBE>(queryService).FirstOrDefault();
                    return objService;
                }


            }
            catch (Exception e)
            {
                return null;
            }
        }

        public CalendarList GetDataCalendar(string calendarId)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var queryCalendar = "select * from calendar where v_CalendarId = '" + calendarId + "'";
                    var objCalendar = cnx.Query<CalendarList>(queryCalendar).FirstOrDefault();
                    return objCalendar;
                }


            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
