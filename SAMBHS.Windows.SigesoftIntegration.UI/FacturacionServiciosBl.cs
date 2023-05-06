using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using Infragistics.Win.UltraWinEditors;
using SAMBHS.Common.BE;
using System.ComponentModel;
using System.Security.RightsManagement;
using SAMBHS.Windows.SigesoftIntegration.UI.Dtos;

namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    public static class FacturacionServiciosBl
    {
        private static TipoFacturacion _tipoFacturacion;
        //private static Tipose _tipoFacturacion;

        public static string GetQuotedString(string str)
        {
            return str != null ? "'" + str.Trim() + "'" : "''";
        }

        private static string ConsultaServiciosPendientes(TipoFacturacion tipo, string f1, string f2, int pintEsoTypeId,
            string pstrGroupOccupationId, string pstrIdClienteAseguradora, string pstrCustomerLocationId = null, int servicio = -1, int cargo = -1, int tipoServicio = -1, string pDni = null, int tipoFact = -1, string pNroLiq = null)
        {
            try
            {
                switch (tipo)
                {
                    case TipoFacturacion.Ocupacional:

                        if (pNroLiq != "")
                        {
                            #region retorna query
                            return @"SELECT 
                    							Filter1.v_ServiceId1 AS ServiceId, 
                    							Filter1.v_CalendarId AS CalendarId, 
                    							Filter1.v_ProtocolId1 AS ProtocolId, 
                    							Filter1.v_PersonId1 AS PersonId, 
                    							Filter1.i_MasterServiceId AS MasterServiceId, 
                    							Filter1.v_Value11 AS TipoServicio, 
                    							Filter1.i_ServiceStatusId AS ServiceStatusId, 
                    							Filter1.v_Value12 AS Estado, 
                    							Filter1.i_AptitudeStatusId AS AptitudeStatusId, 
                    							 CAST(CAST( Filter1.d_ServiceDate AS datetime2) AS datetime2) AS C3, 
                    							 CAST(CAST( Filter1.d_GlobalExpirationDate AS datetime2) AS datetime2) AS C4, 
                    							 CAST(CAST( Filter1.d_ObsExpirationDate AS datetime2) AS datetime2) AS C5, 
                    							Filter1.i_FlagAgentId AS i_FlagAgentId, 
                    							 CAST( Filter1.d_DateTimeCalendar AS datetime2) AS C6, 
                    							o.v_Name AS Organizacion, 
                    							l.v_Name AS Establecimiento, 
                    							sp3.v_Value1 AS MasterServiceType, 
                    							p.i_MasterServiceTypeId AS MasterServiceTypeId, 
                    							p.i_EsoTypeId AS EsoId, 
                    							p.v_EmployerLocationId AS v_EmployerLocationId, 
                    							s0.v_FirstLastName + N' ' + s0.v_SecondLastName + N' ' + s0.v_FirstName AS C7, 
                    							s0.v_FirstName + N' ' + s0.v_FirstLastName + N' ' + s0.v_SecondLastName + N' ' + s0.v_DocNumber AS C8, 
                    							p.v_Name AS Protocolo, 
                    							s7.v_UserName AS UsuarioCrea, 
                    							s8.v_UserName AS UsuarioActualiza, 
                    							Filter1.d_InsertDate1 AS FechaCrea, 
                    							Filter1.d_UpdateDate1 AS FechaActualiza, 
                    							s1.v_Value1 AS Aptitud, 
                    							s2.v_Value1 AS TipoDocumento, 
                    							s0.v_DocNumber AS NroDocumento, 
                    							p.v_CustomerOrganizationId AS v_CustomerOrganizationId, 
                    							p.v_CustomerLocationId AS v_CustomerLocationId, 
                    							s3.v_Value1 AS Eso, 
                    							Filter1.i_InsertUserOccupationalMedicalId AS i_InsertUserOccupationalMedicalId, 
                    							Filter1.i_IsFac AS i_IsFac, 
                    							Filter1.i_ServiceId AS i_ServiceId, 
                    							s4.v_Name AS GrupoOcupacion, 
                    							p.v_GroupOccupationId AS v_GroupOccupationId
                    							FROM (SELECT s.v_NroLiquidacion,s.v_ServiceId AS v_ServiceId1, s.v_ProtocolId AS v_ProtocolId1, s.v_PersonId AS v_PersonId1, s.i_MasterServiceId AS i_MasterServiceId, 
                    							s.i_ServiceStatusId AS i_ServiceStatusId, s.i_AptitudeStatusId AS i_AptitudeStatusId, 
                    							s.d_ServiceDate AS d_ServiceDate, s.d_GlobalExpirationDate AS d_GlobalExpirationDate, 
                    							s.d_ObsExpirationDate AS d_ObsExpirationDate, s.i_FlagAgentId AS i_FlagAgentId, s.d_UpdateDate AS d_UpdateDate1, 
                    							s.i_InsertUserId AS i_InsertUserId1, s.d_InsertDate AS d_InsertDate1, s.i_UpdateUserId AS i_UpdateUserId1, 
                    							s.i_InsertUserOccupationalMedicalId AS i_InsertUserOccupationalMedicalId, s.i_IsFac AS i_IsFac, 
                    							sp1.v_Value1 AS v_Value12, sp2.v_Value1 AS v_Value11, Extent5.v_CalendarId AS v_CalendarId, Extent5.d_DateTimeCalendar AS d_DateTimeCalendar, Extent5.i_ServiceId AS i_ServiceId
                    								FROM service AS s
                    								LEFT OUTER JOIN systemparameter AS sp1 ON (s.i_ServiceStatusId = sp1.i_ParameterId) AND (125 = sp1.i_GroupId)
                    								LEFT OUTER JOIN systemparameter AS sp2 ON (s.i_MasterServiceId = sp2.i_ParameterId) AND (119 = sp2.i_GroupId)
                    								INNER JOIN calendar AS Extent5 ON s.v_ServiceId = Extent5.v_ServiceId
                    								WHERE (0 = s.i_IsDeleted) 
                    								AND (3 = s.i_ServiceStatusId) 
                    								AND ((s.v_IdVentaCliente is null)) 
                    							 AND (1 = Extent5.i_LineStatusId) ) AS Filter1 " +
                                                @"INNER JOIN protocol AS p ON Filter1.v_ProtocolId1 = p.v_ProtocolId
                    							INNER JOIN organization AS o ON p.v_EmployerOrganizationId = o.v_OrganizationId
                    							INNER JOIN location AS l ON p.v_EmployerLocationId = l.v_LocationId
                    							LEFT OUTER JOIN systemparameter AS sp3 ON (p.i_MasterServiceTypeId = sp3.i_ParameterId) AND (119 = sp3.i_GroupId)
                    							INNER JOIN person AS s0 ON Filter1.v_PersonId1 = s0.v_PersonId
                    							LEFT OUTER JOIN systemparameter AS s1 ON (Filter1.i_AptitudeStatusId = s1.i_ParameterId) AND (124 = s1.i_GroupId)
                    							LEFT OUTER JOIN datahierarchy AS s2 ON (s0.i_DocTypeId = s2.i_ItemId) AND (106 = s2.i_GroupId)
                    							LEFT OUTER JOIN systemparameter AS s3 ON (p.i_EsoTypeId = s3.i_ParameterId) AND (118 = s3.i_GroupId)
                    							LEFT OUTER JOIN groupoccupation AS s4 ON p.v_GroupOccupationId = s4.v_GroupOccupationId
                    							LEFT OUTER JOIN organization AS s5 ON p.v_CustomerOrganizationId = s5.v_OrganizationId
                    							LEFT OUTER JOIN location AS s6 ON ((p.v_CustomerOrganizationId = s6.v_OrganizationId) OR ((p.v_CustomerOrganizationId IS NULL) AND (s6.v_OrganizationId IS NULL))) AND (p.v_CustomerLocationId = s6.v_LocationId)
                    							LEFT OUTER JOIN systemuser AS s7 ON Filter1.i_InsertUserId1 = s7.i_SystemUserId
                    							LEFT OUTER JOIN systemuser AS s8 ON Filter1.i_UpdateUserId1 = s8.i_SystemUserId " +
                                                         "WHERE '" + pNroLiq + "' = Filter1.v_NroLiquidacion and Filter1.i_IsFac <> 2";
                            #endregion
                        }
                        else
                        {
                            #region retorna query
                            return @"SELECT 
                    							Filter1.v_ServiceId1 AS ServiceId, 
                    							Filter1.v_CalendarId AS CalendarId, 
                    							Filter1.v_ProtocolId1 AS ProtocolId, 
                    							Filter1.v_PersonId1 AS PersonId, 
                    							Filter1.i_MasterServiceId AS MasterServiceId, 
                    							Filter1.v_Value11 AS TipoServicio, 
                    							Filter1.i_ServiceStatusId AS ServiceStatusId, 
                    							Filter1.v_Value12 AS Estado, 
                    							Filter1.i_AptitudeStatusId AS AptitudeStatusId, 
                    							 CAST(CAST( Filter1.d_ServiceDate AS datetime2) AS datetime2) AS C3, 
                    							 CAST(CAST( Filter1.d_GlobalExpirationDate AS datetime2) AS datetime2) AS C4, 
                    							 CAST(CAST( Filter1.d_ObsExpirationDate AS datetime2) AS datetime2) AS C5, 
                    							Filter1.i_FlagAgentId AS i_FlagAgentId, 
                    							 CAST( Filter1.d_DateTimeCalendar AS datetime2) AS C6, 
                    							o.v_Name AS Organizacion, 
                    							l.v_Name AS Establecimiento, 
                    							sp3.v_Value1 AS MasterServiceType, 
                    							p.i_MasterServiceTypeId AS MasterServiceTypeId, 
                    							p.i_EsoTypeId AS EsoId, 
                    							p.v_EmployerLocationId AS v_EmployerLocationId, 
                    							s0.v_FirstLastName + N' ' + s0.v_SecondLastName + N' ' + s0.v_FirstName AS C7, 
                    							s0.v_FirstName + N' ' + s0.v_FirstLastName + N' ' + s0.v_SecondLastName + N' ' + s0.v_DocNumber AS C8, 
                    							p.v_Name AS Protocolo, 
                    							s7.v_UserName AS UsuarioCrea, 
                    							s8.v_UserName AS UsuarioActualiza, 
                    							Filter1.d_InsertDate1 AS FechaCrea, 
                    							Filter1.d_UpdateDate1 AS FechaActualiza, 
                    							s1.v_Value1 AS Aptitud, 
                    							s2.v_Value1 AS TipoDocumento, 
                    							s0.v_DocNumber AS NroDocumento, 
                    							p.v_CustomerOrganizationId AS v_CustomerOrganizationId, 
                    							p.v_CustomerLocationId AS v_CustomerLocationId, 
                    							s3.v_Value1 AS Eso, 
                    							Filter1.i_InsertUserOccupationalMedicalId AS i_InsertUserOccupationalMedicalId, 
                    							Filter1.i_IsFac AS i_IsFac, 
                    							Filter1.i_ServiceId AS i_ServiceId, 
                    							s4.v_Name AS GrupoOcupacion, 
                    							p.v_GroupOccupationId AS v_GroupOccupationId
                    							FROM (SELECT s.v_ServiceId AS v_ServiceId1, s.v_ProtocolId AS v_ProtocolId1, s.v_PersonId AS v_PersonId1, s.i_MasterServiceId AS i_MasterServiceId, 
                    							s.i_ServiceStatusId AS i_ServiceStatusId, s.i_AptitudeStatusId AS i_AptitudeStatusId, 
                    							s.d_ServiceDate AS d_ServiceDate, s.d_GlobalExpirationDate AS d_GlobalExpirationDate, 
                    							s.d_ObsExpirationDate AS d_ObsExpirationDate, s.i_FlagAgentId AS i_FlagAgentId, s.d_UpdateDate AS d_UpdateDate1, 
                    							s.i_InsertUserId AS i_InsertUserId1, s.d_InsertDate AS d_InsertDate1, s.i_UpdateUserId AS i_UpdateUserId1, 
                    							s.i_InsertUserOccupationalMedicalId AS i_InsertUserOccupationalMedicalId, s.i_IsFac AS i_IsFac, 
                    							sp1.v_Value1 AS v_Value12, sp2.v_Value1 AS v_Value11, Extent5.v_CalendarId AS v_CalendarId, Extent5.d_DateTimeCalendar AS d_DateTimeCalendar, Extent5.i_ServiceId AS i_ServiceId
                    								FROM service AS s
                    								LEFT OUTER JOIN systemparameter AS sp1 ON (s.i_ServiceStatusId = sp1.i_ParameterId) AND (125 = sp1.i_GroupId)
                    								LEFT OUTER JOIN systemparameter AS sp2 ON (s.i_MasterServiceId = sp2.i_ParameterId) AND (119 = sp2.i_GroupId)
                    								INNER JOIN calendar AS Extent5 ON s.v_ServiceId = Extent5.v_ServiceId
                    								WHERE (0 = s.i_IsDeleted) 
                    								AND (3 = s.i_ServiceStatusId) 
                    								AND ((s.v_IdVentaCliente is null)) 
                    								AND ( CAST(CAST( s.d_ServiceDate AS datetime2) AS datetime2) >= convert(datetime2, '" + f1 + "', 121)) AND ( CAST(CAST( s.d_ServiceDate AS datetime2) AS datetime2) <= convert(datetime2, '" + f2 + " 23:59', 121)) AND (1 = Extent5.i_LineStatusId) ) AS Filter1 " +
                                                @"INNER JOIN protocol AS p ON Filter1.v_ProtocolId1 = p.v_ProtocolId
                    							INNER JOIN organization AS o ON p.v_EmployerOrganizationId = o.v_OrganizationId
                    							INNER JOIN location AS l ON p.v_EmployerLocationId = l.v_LocationId
                    							LEFT OUTER JOIN systemparameter AS sp3 ON (p.i_MasterServiceTypeId = sp3.i_ParameterId) AND (119 = sp3.i_GroupId)
                    							INNER JOIN person AS s0 ON Filter1.v_PersonId1 = s0.v_PersonId
                    							LEFT OUTER JOIN systemparameter AS s1 ON (Filter1.i_AptitudeStatusId = s1.i_ParameterId) AND (124 = s1.i_GroupId)
                    							LEFT OUTER JOIN datahierarchy AS s2 ON (s0.i_DocTypeId = s2.i_ItemId) AND (106 = s2.i_GroupId)
                    							LEFT OUTER JOIN systemparameter AS s3 ON (p.i_EsoTypeId = s3.i_ParameterId) AND (118 = s3.i_GroupId)
                    							LEFT OUTER JOIN groupoccupation AS s4 ON p.v_GroupOccupationId = s4.v_GroupOccupationId
                    							LEFT OUTER JOIN organization AS s5 ON p.v_CustomerOrganizationId = s5.v_OrganizationId
                    							LEFT OUTER JOIN location AS s6 ON ((p.v_CustomerOrganizationId = s6.v_OrganizationId) OR ((p.v_CustomerOrganizationId IS NULL) AND (s6.v_OrganizationId IS NULL))) AND (p.v_CustomerLocationId = s6.v_LocationId)
                    							LEFT OUTER JOIN systemuser AS s7 ON Filter1.i_InsertUserId1 = s7.i_SystemUserId
                    							LEFT OUTER JOIN systemuser AS s8 ON Filter1.i_UpdateUserId1 = s8.i_SystemUserId " +
                                                        "WHERE (" + pintEsoTypeId + " = -1 or " + pintEsoTypeId + " = p.i_EsoTypeId) " +
                                                        "AND (" + pstrGroupOccupationId + " is null or " + pstrGroupOccupationId + " = p.v_GroupOccupationId) " +
                                                        "AND (" + pstrIdClienteAseguradora + " = p.v_CustomerOrganizationId)  " +
                                                        "AND (" + pstrCustomerLocationId + " = p.v_CustomerLocationId)" +
                                                        "AND (" + servicio + " = p.i_MasterServiceId)";
                            #endregion
                        }
                   

                    case TipoFacturacion.Aseguradora:

                        #region retorna query
                        return @"SELECT 
                    							Filter1.v_ServiceId1 AS ServiceId, 
                    							Filter1.v_CalendarId AS CalendarId, 
                    							Filter1.v_ProtocolId1 AS ProtocolId, 
                    							Filter1.v_PersonId1 AS PersonId, 
                    							Filter1.i_MasterServiceId AS MasterServiceId, 
                    							Filter1.v_Value11 AS TipoServicio, 
                    							Filter1.i_ServiceStatusId AS ServiceStatusId, 
                    							Filter1.v_Value12 AS Estado, 
                    							Filter1.i_AptitudeStatusId AS AptitudeStatusId, 
                    							 CAST(CAST( Filter1.d_ServiceDate AS datetime2) AS datetime2) AS C3, 
                    							 CAST(CAST( Filter1.d_GlobalExpirationDate AS datetime2) AS datetime2) AS C4, 
                    							 CAST(CAST( Filter1.d_ObsExpirationDate AS datetime2) AS datetime2) AS C5, 
                    							Filter1.i_FlagAgentId AS i_FlagAgentId, 
                    							 CAST( Filter1.d_DateTimeCalendar AS datetime2) AS C6, 
                    							o.v_Name AS Organizacion, 
                    							l.v_Name AS Establecimiento, 
                    							sp3.v_Value1 AS MasterServiceType, 
                    							p.i_MasterServiceTypeId AS MasterServiceTypeId, 
                    							p.i_EsoTypeId AS EsoId, 
                    							p.v_EmployerLocationId AS v_EmployerLocationId, 
                    							s0.v_FirstLastName + N' ' + s0.v_SecondLastName + N' ' + s0.v_FirstName AS C7, 
                    							s0.v_FirstName + N' ' + s0.v_FirstLastName + N' ' + s0.v_SecondLastName + N' ' + s0.v_DocNumber AS C8, 
                    							p.v_Name AS Protocolo, 
                    							s7.v_UserName AS UsuarioCrea, 
                    							s8.v_UserName AS UsuarioActualiza, 
                    							Filter1.d_InsertDate1 AS FechaCrea, 
                    							Filter1.d_UpdateDate1 AS FechaActualiza, 
                    							s1.v_Value1 AS Aptitud, 
                    							s2.v_Value1 AS TipoDocumento, 
                    							s0.v_DocNumber AS NroDocumento, 
                    							p.v_CustomerOrganizationId AS v_CustomerOrganizationId, 
                    							p.v_CustomerLocationId AS v_CustomerLocationId, 
                    							s3.v_Value1 AS Eso, 
                    							Filter1.i_InsertUserOccupationalMedicalId AS i_InsertUserOccupationalMedicalId, 
                    							Filter1.i_IsFac AS i_IsFac, 
                    							Filter1.i_ServiceId AS i_ServiceId, 
                    							s4.v_Name AS GrupoOcupacion, 
                    							p.v_GroupOccupationId AS v_GroupOccupationId
                    							FROM (SELECT s.v_ServiceId AS v_ServiceId1, s.v_ProtocolId AS v_ProtocolId1, s.v_PersonId AS v_PersonId1, s.i_MasterServiceId AS i_MasterServiceId, 
                    							s.i_ServiceStatusId AS i_ServiceStatusId, s.i_AptitudeStatusId AS i_AptitudeStatusId, 
                    							s.d_ServiceDate AS d_ServiceDate, s.d_GlobalExpirationDate AS d_GlobalExpirationDate, 
                    							s.d_ObsExpirationDate AS d_ObsExpirationDate, s.i_FlagAgentId AS i_FlagAgentId, s.d_UpdateDate AS d_UpdateDate1, 
                    							s.i_InsertUserId AS i_InsertUserId1, s.d_InsertDate AS d_InsertDate1, s.i_UpdateUserId AS i_UpdateUserId1, 
                    							s.i_InsertUserOccupationalMedicalId AS i_InsertUserOccupationalMedicalId, s.i_IsFac AS i_IsFac, 
                    							sp1.v_Value1 AS v_Value12, sp2.v_Value1 AS v_Value11, Extent5.v_CalendarId AS v_CalendarId, Extent5.d_DateTimeCalendar AS d_DateTimeCalendar, Extent5.i_ServiceId AS i_ServiceId
                    								FROM service AS s
                    								LEFT OUTER JOIN systemparameter AS sp1 ON (s.i_ServiceStatusId = sp1.i_ParameterId) AND (125 = sp1.i_GroupId)
                    								LEFT OUTER JOIN systemparameter AS sp2 ON (s.i_MasterServiceId = sp2.i_ParameterId) AND (119 = sp2.i_GroupId)
                    								INNER JOIN calendar AS Extent5 ON s.v_ServiceId = Extent5.v_ServiceId
                    								WHERE (0 = s.i_IsDeleted) 
                    								AND (3 = s.i_ServiceStatusId) 
                    								AND ((s.v_IdVentaAseguradora is null)) 
                    								AND ( CAST(CAST( s.d_ServiceDate AS datetime2) AS datetime2) >= convert(datetime2, '" + f1 + "', 121)) AND ( CAST(CAST( s.d_ServiceDate AS datetime2) AS datetime2) <= convert(datetime2, '" + f2 + " 23:59', 121)) AND (1 = Extent5.i_LineStatusId) ) AS Filter1 " +
                                            @"INNER JOIN protocol AS p ON Filter1.v_ProtocolId1 = p.v_ProtocolId
                    							INNER JOIN organization AS o ON p.v_EmployerOrganizationId = o.v_OrganizationId
                    							INNER JOIN location AS l ON p.v_EmployerLocationId = l.v_LocationId
                    							LEFT OUTER JOIN systemparameter AS sp3 ON (p.i_MasterServiceTypeId = sp3.i_ParameterId) AND (119 = sp3.i_GroupId)
                    							INNER JOIN person AS s0 ON Filter1.v_PersonId1 = s0.v_PersonId
                    							LEFT OUTER JOIN systemparameter AS s1 ON (Filter1.i_AptitudeStatusId = s1.i_ParameterId) AND (124 = s1.i_GroupId)
                    							LEFT OUTER JOIN datahierarchy AS s2 ON (s0.i_DocTypeId = s2.i_ItemId) AND (106 = s2.i_GroupId)
                    							LEFT OUTER JOIN systemparameter AS s3 ON (p.i_EsoTypeId = s3.i_ParameterId) AND (118 = s3.i_GroupId)
                    							LEFT OUTER JOIN groupoccupation AS s4 ON p.v_GroupOccupationId = s4.v_GroupOccupationId
                    							LEFT OUTER JOIN organization AS s5 ON p.v_CustomerOrganizationId = s5.v_OrganizationId
                    							LEFT OUTER JOIN location AS s6 ON ((p.v_CustomerOrganizationId = s6.v_OrganizationId) OR ((p.v_CustomerOrganizationId IS NULL) AND (s6.v_OrganizationId IS NULL))) AND (p.v_CustomerLocationId = s6.v_LocationId)
                    							LEFT OUTER JOIN systemuser AS s7 ON Filter1.i_InsertUserId1 = s7.i_SystemUserId
                    							LEFT OUTER JOIN systemuser AS s8 ON Filter1.i_UpdateUserId1 = s8.i_SystemUserId " +
                                                    "WHERE (" + pintEsoTypeId + " = -1 or " + pintEsoTypeId + " = p.i_EsoTypeId) " +
                                                    "AND (" + pstrGroupOccupationId + " is null or " + pstrGroupOccupationId + " = p.v_GroupOccupationId) " +
                                                    "AND (" + pstrIdClienteAseguradora + " = p.v_AseguradoraOrganizationId)  ";
                        #endregion

                    case TipoFacturacion.Asistencial:

                        #region retorna query

                        return @"select s.d_PagoMedico AS d_PagoMedico, s.d_PagoPaciente AS d_PagoPaciente from hospitalizacion s INNER JOIN person p on  s.v_PersonId = p.v_PersonId " +
                            " where ( CAST(CAST( s.d_FechaIngreso AS datetime2) AS datetime2) >= convert(datetime2,'" + f1 + "' , 121)) AND ( CAST(CAST( s.d_FechaAlta AS datetime2) AS datetime2) <= convert(datetime2, '" + f2 + "', 121)) " +
                            " AND p.v_DocNumber = '" + pDni + "'";
//                        return @"SELECT 
//                    							Filter1.v_ServiceId1 AS ServiceId, 
//                    							Filter1.v_CalendarId AS CalendarId, 
//                    							Filter1.v_ProtocolId1 AS ProtocolId, 
//                    							Filter1.v_PersonId1 AS PersonId, 
//                    							Filter1.i_MasterServiceId AS MasterServiceId, 
//                    							Filter1.v_Value11 AS TipoServicio, 
//                    							Filter1.i_ServiceStatusId AS ServiceStatusId, 
//                    							Filter1.v_Value12 AS Estado, 
//                    							Filter1.i_AptitudeStatusId AS AptitudeStatusId, 
//                    							 CAST(CAST( Filter1.d_ServiceDate AS datetime2) AS datetime2) AS C3, 
//                    							 CAST(CAST( Filter1.d_GlobalExpirationDate AS datetime2) AS datetime2) AS C4, 
//                    							 CAST(CAST( Filter1.d_ObsExpirationDate AS datetime2) AS datetime2) AS C5, 
//                    							Filter1.i_FlagAgentId AS i_FlagAgentId, 
//                    							 CAST( Filter1.d_DateTimeCalendar AS datetime2) AS C6, 
//                    							o.v_Name AS Organizacion, 
//                    							l.v_Name AS Establecimiento, 
//                    							sp3.v_Value1 AS MasterServiceType, 
//                    							p.i_MasterServiceTypeId AS MasterServiceTypeId, 
//                    							p.i_EsoTypeId AS EsoId, 
//                    							p.v_EmployerLocationId AS v_EmployerLocationId, 
//                    							s0.v_FirstLastName + N' ' + s0.v_SecondLastName + N' ' + s0.v_FirstName AS C7, 
//                    							s0.v_FirstName + N' ' + s0.v_FirstLastName + N' ' + s0.v_SecondLastName + N' ' + s0.v_DocNumber AS C8, 
//                    							p.v_Name AS Protocolo, 
//                    							s7.v_UserName AS UsuarioCrea, 
//                    							s8.v_UserName AS UsuarioActualiza, 
//                    							Filter1.d_InsertDate1 AS FechaCrea, 
//                    							Filter1.d_UpdateDate1 AS FechaActualiza, 
//                    							s1.v_Value1 AS Aptitud, 
//                    							s2.v_Value1 AS TipoDocumento, 
//                    							s0.v_DocNumber AS NroDocumento, 
//                    							p.v_CustomerOrganizationId AS v_CustomerOrganizationId, 
//                    							p.v_CustomerLocationId AS v_CustomerLocationId, 
//                    							s3.v_Value1 AS Eso, 
//                    							Filter1.i_InsertUserOccupationalMedicalId AS i_InsertUserOccupationalMedicalId, 
//                    							Filter1.i_IsFac AS i_IsFac, 
//                    							Filter1.i_ServiceId AS i_ServiceId, 
//                    							s4.v_Name AS GrupoOcupacion, 
//                    							p.v_GroupOccupationId AS v_GroupOccupationId
//                    							FROM (SELECT s.v_ServiceId AS v_ServiceId1, s.v_ProtocolId AS v_ProtocolId1, s.v_PersonId AS v_PersonId1, s.i_MasterServiceId AS i_MasterServiceId, 
//                    							s.i_ServiceStatusId AS i_ServiceStatusId, s.i_AptitudeStatusId AS i_AptitudeStatusId, 
//                    							s.d_ServiceDate AS d_ServiceDate, s.d_GlobalExpirationDate AS d_GlobalExpirationDate, 
//                    							s.d_ObsExpirationDate AS d_ObsExpirationDate, s.i_FlagAgentId AS i_FlagAgentId, s.d_UpdateDate AS d_UpdateDate1, 
//                    							s.i_InsertUserId AS i_InsertUserId1, s.d_InsertDate AS d_InsertDate1, s.i_UpdateUserId AS i_UpdateUserId1, 
//                    							s.i_InsertUserOccupationalMedicalId AS i_InsertUserOccupationalMedicalId, s.i_IsFac AS i_IsFac, 
//                    							sp1.v_Value1 AS v_Value12, sp2.v_Value1 AS v_Value11, Extent5.v_CalendarId AS v_CalendarId, Extent5.d_DateTimeCalendar AS d_DateTimeCalendar, Extent5.i_ServiceId AS i_ServiceId
//                    								FROM service AS s
//                    								LEFT OUTER JOIN systemparameter AS sp1 ON (s.i_ServiceStatusId = sp1.i_ParameterId) AND (125 = sp1.i_GroupId)
//                    								LEFT OUTER JOIN systemparameter AS sp2 ON (s.i_MasterServiceId = sp2.i_ParameterId) AND (119 = sp2.i_GroupId)
//                    								INNER JOIN calendar AS Extent5 ON s.v_ServiceId = Extent5.v_ServiceId
//                    								WHERE (0 = s.i_IsDeleted) 
//                    								AND (3 = s.i_ServiceStatusId) 
//                    								AND (s.v_IdVentaCliente is null) " +
//                                                "AND ( CAST(CAST( s.d_ServiceDate AS datetime2) AS datetime2) >= convert(datetime2, '" + f1 + "', 121)) AND ( CAST(CAST( s.d_ServiceDate AS datetime2) AS datetime2) <= convert(datetime2, '" + f2 + " 23:59', 121)) AND (1 = Extent5.i_LineStatusId) ) AS Filter1 INNER JOIN protocol AS p ON Filter1.v_ProtocolId1 = p.v_ProtocolId " +
//                                            @"INNER JOIN organization AS o ON p.v_EmployerOrganizationId = o.v_OrganizationId
//                    							INNER JOIN location AS l ON p.v_EmployerLocationId = l.v_LocationId
//                    							LEFT OUTER JOIN systemparameter AS sp3 ON (p.i_MasterServiceTypeId = sp3.i_ParameterId) AND (119 = sp3.i_GroupId)
//                    							INNER JOIN person AS s0 ON Filter1.v_PersonId1 = s0.v_PersonId
//                    							LEFT OUTER JOIN systemparameter AS s1 ON (Filter1.i_AptitudeStatusId = s1.i_ParameterId) AND (124 = s1.i_GroupId)
//                    							LEFT OUTER JOIN datahierarchy AS s2 ON (s0.i_DocTypeId = s2.i_ItemId) AND (106 = s2.i_GroupId)
//                    							LEFT OUTER JOIN systemparameter AS s3 ON (p.i_EsoTypeId = s3.i_ParameterId) AND (118 = s3.i_GroupId)
//                    							LEFT OUTER JOIN groupoccupation AS s4 ON p.v_GroupOccupationId = s4.v_GroupOccupationId
//                    							LEFT OUTER JOIN organization AS s5 ON p.v_CustomerOrganizationId = s5.v_OrganizationId
//                    							LEFT OUTER JOIN location AS s6 ON ((p.v_CustomerOrganizationId = s6.v_OrganizationId) OR ((p.v_CustomerOrganizationId IS NULL) AND (s6.v_OrganizationId IS NULL))) AND (p.v_CustomerLocationId = s6.v_LocationId)
//                    							LEFT OUTER JOIN systemuser AS s7 ON Filter1.i_InsertUserId1 = s7.i_SystemUserId
//                    							LEFT OUTER JOIN systemuser AS s8 ON Filter1.i_UpdateUserId1 = s8.i_SystemUserId " +
//                                            "WHERE s0.v_DocNumber = " + pstrIdClienteAseguradora;
                        #endregion

                    case TipoFacturacion.Hospitalizacion:

                        #region MyRegion

                        if (tipoFact == 1)
                        {
                            return @"select ser.v_ServiceId as ServiceId, ser.v_ProtocolId as ProtocolId, ser.v_PersonId as PersonId, ser.i_MasterServiceId as MasterServiceId, 
                                sp2.v_Value1 as TipoServicio, ser.i_ServiceStatusId as i_ServiceStatusId, sp1.v_Value1 as Estado, ser.i_AptitudeStatusId as i_AptitudeStatusId,
                                orgCli.v_Name as Organizacion, locCli.v_Name as Establecimiento, sp3.v_Value1 as MasterServiceType, prot.i_MasterServiceTypeId as MasterServiceTypeId,
                                prot.i_EsoTypeId as i_EsoTypeId, prot.v_Name as Protocolo, s1.v_Value1 as Aptitud, s2.v_Value1 as TipoDocumento, s0.v_DocNumber as NroDocumento,
                                s3.v_Value1 as Eso, s4.v_Name as GrupoOcupacion,
                                s0.v_FirstLastName + N' ' + s0.v_SecondLastName + N' ' + s0.v_FirstName AS C7, CAST(CAST( ser.d_ServiceDate AS datetime2) AS datetime2) AS C3
                                from service ser
                                inner join protocol prot on ser.v_ProtocolId = prot.v_ProtocolId
                                inner join systemparameter AS sp2 ON (ser.i_MasterServiceId = sp2.i_ParameterId) AND (119 = sp2.i_GroupId)
                                inner join systemparameter AS sp1 ON (ser.i_ServiceStatusId = sp1.i_ParameterId) AND (125 = sp1.i_GroupId)
                                inner join organization orgCli on prot.v_CustomerOrganizationId = orgCli.v_OrganizationId
                                inner join location locCli on prot.v_CustomerLocationId = locCli.v_LocationId
                                inner join systemparameter AS sp3 ON (prot.i_MasterServiceTypeId = sp3.i_ParameterId) AND (119 = sp3.i_GroupId)
                                inner join systemparameter AS s1 ON (ser.i_AptitudeStatusId = s1.i_ParameterId) AND (124 = s1.i_GroupId)
                                INNER JOIN person AS s0 ON ser.v_PersonId = s0.v_PersonId
                                inner join datahierarchy AS s2 ON (s0.i_DocTypeId = s2.i_ItemId) AND (106 = s2.i_GroupId)
                                inner join systemparameter AS s3 ON (prot.i_EsoTypeId = s3.i_ParameterId) AND (118 = s3.i_GroupId)
                                inner join groupoccupation AS s4 ON prot.v_GroupOccupationId = s4.v_GroupOccupationId " +
                             "where prot.v_CustomerOrganizationId = " + pstrIdClienteAseguradora + " and prot.v_CustomerLocationId = " + pstrCustomerLocationId + " and ser.i_IsFac = 0 and ser.i_MasterServiceId = " + tipoServicio +
                             "AND(ser.d_ServiceDate > CONVERT(datetime,'" + f1 + "',121) and  ser.d_ServiceDate < CONVERT(datetime,'" + f2 + "',121)) " +
                              "AND s0.v_DocNumber = '" + pDni + "' and ser.i_IsFac = 0";
                        }
                        if (tipoFact == 2)
                        {

                            if (pNroLiq != "")
                            {
                                return @" select sum(hh.d_Precio) as TotalHabitacion , " +
                                   " sum(td.d_PrecioVenta) as TotalTicket,   " +
                                   " sum(sc.r_Price) as TotalService, " +
                                   " sum(hh.d_Precio) + sum(td.d_PrecioVenta) + sum(sc.r_Price) as Total " +

                                   " from hospitalizacion h " +
                                   " inner join hospitalizacionhabitacion hh on h.v_HopitalizacionId = hh.v_HopitalizacionId " +
                                   " inner join hospitalizacionservice hs on h.v_HopitalizacionId = hs.v_HopitalizacionId " +
                                   " inner join servicecomponent sc on hs.v_ServiceId = sc.v_ServiceId " +
                                   " inner join ticket t on hs.v_ServiceId  = t.v_ServiceId " +
                                   " inner join ticketdetalle td on t.v_TicketId = td.v_TicketId " +
                                   " where v_NroLiquidacion = '" + pNroLiq + "'" +
                                       " and sc.i_IsRequiredId = 1 and sc.i_IsDeleted = 0  " +
                                       "  and td.i_IsDeleted = 0 " +
                                       "  and t.i_ConCargoA = " + cargo +
                                       "  and hh.i_ConCargoA  = " + cargo +
                                       "  and sc.i_ConCargoA = " + cargo +
                                  "  group by hs.v_ServiceId,hh.d_Precio, hh.v_HopitalizacionId, hs.v_HopitalizacionId, t.v_ServiceId ";
                            }
                            else
                            {
                                return @"select ser.v_ServiceId as ServiceId, ser.v_ProtocolId as ProtocolId, ser.v_PersonId as PersonId, ser.i_MasterServiceId as MasterServiceId, 
                                sp2.v_Value1 as TipoServicio, ser.i_ServiceStatusId as i_ServiceStatusId, sp1.v_Value1 as Estado, ser.i_AptitudeStatusId as i_AptitudeStatusId,
                                orgCli.v_Name as Organizacion, locCli.v_Name as Establecimiento, sp3.v_Value1 as MasterServiceType, prot.i_MasterServiceTypeId as MasterServiceTypeId,
                                prot.i_EsoTypeId as i_EsoTypeId, prot.v_Name as Protocolo, s1.v_Value1 as Aptitud, s2.v_Value1 as TipoDocumento, s0.v_DocNumber as NroDocumento,
                                s3.v_Value1 as Eso, s4.v_Name as GrupoOcupacion,
                                s0.v_FirstLastName + N' ' + s0.v_SecondLastName + N' ' + s0.v_FirstName AS C7, CAST(CAST( ser.d_ServiceDate AS datetime2) AS datetime2) AS C3
                                from service ser
                                inner join protocol prot on ser.v_ProtocolId = prot.v_ProtocolId
                                inner join systemparameter AS sp2 ON (ser.i_MasterServiceId = sp2.i_ParameterId) AND (119 = sp2.i_GroupId)
                                inner join systemparameter AS sp1 ON (ser.i_ServiceStatusId = sp1.i_ParameterId) AND (125 = sp1.i_GroupId)
                                inner join organization orgCli on prot.v_CustomerOrganizationId = orgCli.v_OrganizationId
                                inner join location locCli on prot.v_CustomerLocationId = locCli.v_LocationId
                                inner join systemparameter AS sp3 ON (prot.i_MasterServiceTypeId = sp3.i_ParameterId) AND (119 = sp3.i_GroupId)
                                inner join systemparameter AS s1 ON (ser.i_AptitudeStatusId = s1.i_ParameterId) AND (124 = s1.i_GroupId)
                                INNER JOIN person AS s0 ON ser.v_PersonId = s0.v_PersonId
                                inner join datahierarchy AS s2 ON (s0.i_DocTypeId = s2.i_ItemId) AND (106 = s2.i_GroupId)
                                inner join systemparameter AS s3 ON (prot.i_EsoTypeId = s3.i_ParameterId) AND (118 = s3.i_GroupId)
                                inner join groupoccupation AS s4 ON prot.v_GroupOccupationId = s4.v_GroupOccupationId " +
                                   "where prot.v_CustomerOrganizationId = " + pstrIdClienteAseguradora + " and prot.v_CustomerLocationId = " + pstrCustomerLocationId + " and ser.i_IsFac = 0 and ser.i_MasterServiceId = " + tipoServicio +
                                   "AND(ser.d_ServiceDate > CONVERT(datetime,'" + f1 + "',121) and  ser.d_ServiceDate < CONVERT(datetime,'" + f2 + "',121)) " +
                                   "AND s0.v_DocNumber = '" + pDni + "' and ser.i_IsFacMedico = 0";
                            }


//                            
                        }

                        #endregion
                        break;
                }

            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }

        public static List<ConsultaServicioDto> ObtenerServiciosPendientes( TipoFacturacion tipofacturacion, DateTime fechaIni, DateTime fechaFin,
            string pstrCustomerOrganizationId, string pstrCustomerLocationId, int pintEsoTypeId = -1, string pstrGroupOccupationId = null, int servicio = -1, int cargo = -1, int tipoServicio = -1, string pDni = null, int Tipofact = -1, string pNroLiq = null)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();
                    pstrCustomerLocationId = GetQuotedString(pstrCustomerLocationId);
                    pstrCustomerOrganizationId = GetQuotedString(pstrCustomerOrganizationId);
                    var f1 = string.Format("{0}-{1}-{2}", fechaIni.Year, fechaIni.Month, fechaIni.Day);
                    var f2 = string.Format("{0}-{1}-{2}", fechaFin.Year, fechaFin.Month, fechaFin.Day);
                    pstrGroupOccupationId = string.IsNullOrEmpty(pstrGroupOccupationId) || pstrGroupOccupationId.Equals("-1") ? "null" : GetQuotedString(pstrGroupOccupationId);

                    //if (tipoServicio == 2)
                    //{
                    //    _tipoFacturacion = TipoFacturacion.Ocupacional;
                    //}
                    //else if (tipoServicio == 3)
                    //{
                    //    _tipoFacturacion = TipoFacturacion.Asistencial;
                    //}
                    //else if (tipoServicio == 19)
                    //{
                    //    _tipoFacturacion = TipoFacturacion.Hospitalizacion;
                    //}

                    #region Consulta
                    var consulta = ConsultaServiciosPendientes(tipofacturacion, f1, f2, pintEsoTypeId, pstrGroupOccupationId, pstrCustomerOrganizationId, pstrCustomerLocationId, servicio, cargo, tipoServicio, pDni, Tipofact, pNroLiq);
                    #endregion

                    var resultado = cnx.Query<ConsultaServicioDto>(consulta).ToList();

                    List<ConsultaServicioDto> list = new List<ConsultaServicioDto>();
                    if (cargo == 1)
                    {
                        var primerServiceId = "";
                        if (resultado.Count() > 0)
                        {
                            primerServiceId = resultado.ToList()[0].ServiceId;
                        }
                      

                        foreach (var item in resultado)
                        {
                            var oConsultaServicioDto = new ConsultaServicioDto();
                            oConsultaServicioDto.ServiceId = item.ServiceId;

                            oConsultaServicioDto.ProtocolId = item.ProtocolId;
                            oConsultaServicioDto.PersonId = item.PersonId;
                            oConsultaServicioDto.MasterServiceId = item.MasterServiceId;
                            oConsultaServicioDto.TipoServicio = item.TipoServicio;
                            oConsultaServicioDto.ServiceStatusId = item.ServiceStatusId;
                            oConsultaServicioDto.Estado = item.Estado;
                            oConsultaServicioDto.AptitudeStatusId = item.AptitudeStatusId;
                            oConsultaServicioDto.Organizacion = item.Organizacion;
                            oConsultaServicioDto.Establecimiento = item.Establecimiento;
                            oConsultaServicioDto.MasterServiceType = item.MasterServiceType;


                            oConsultaServicioDto.MasterServiceTypeId = item.MasterServiceTypeId;
                            oConsultaServicioDto.EsoId = item.EsoId;
                            oConsultaServicioDto.Protocolo = item.Protocolo;
                            oConsultaServicioDto.Aptitud = item.Aptitud;
                            oConsultaServicioDto.TipoDocumento = item.TipoDocumento;
                            oConsultaServicioDto.NroDocumento = item.NroDocumento;
                            oConsultaServicioDto.Eso = item.Eso;
                            oConsultaServicioDto.GrupoOcupacion = item.GrupoOcupacion;
                            oConsultaServicioDto.C7 = item.C7;
                            oConsultaServicioDto.C3 = item.C3;

                            oConsultaServicioDto.Total = item.d_PagoMedico;// CalcularTotalMedico(item.ServiceId, primerServiceId);

                            list.Add(oConsultaServicioDto);
                        }
                    }
                    else
                    {

                        foreach (var item in resultado)
                        {
                            var oConsultaServicioDto = new ConsultaServicioDto();
                            oConsultaServicioDto.ServiceId = item.ServiceId;

                            oConsultaServicioDto.ProtocolId = item.ProtocolId;
                            oConsultaServicioDto.PersonId = item.PersonId;
                            oConsultaServicioDto.MasterServiceId = item.MasterServiceId;
                            oConsultaServicioDto.TipoServicio = item.TipoServicio;
                            oConsultaServicioDto.ServiceStatusId = item.ServiceStatusId;
                            oConsultaServicioDto.Estado = item.Estado;
                            oConsultaServicioDto.AptitudeStatusId = item.AptitudeStatusId;
                            oConsultaServicioDto.Organizacion = item.Organizacion;
                            oConsultaServicioDto.Establecimiento = item.Establecimiento;
                            oConsultaServicioDto.MasterServiceType = item.MasterServiceType;

                            oConsultaServicioDto.MasterServiceTypeId = item.MasterServiceTypeId;
                            oConsultaServicioDto.EsoId = item.EsoId;
                            oConsultaServicioDto.Protocolo = item.Protocolo;
                            oConsultaServicioDto.Aptitud = item.Aptitud;
                            oConsultaServicioDto.TipoDocumento = item.TipoDocumento;
                            oConsultaServicioDto.NroDocumento = item.NroDocumento;
                            oConsultaServicioDto.Eso = item.Eso;
                            oConsultaServicioDto.GrupoOcupacion = item.GrupoOcupacion;
                            oConsultaServicioDto.C7 = item.C7;
                            oConsultaServicioDto.C3 = item.C3;

                            oConsultaServicioDto.Total = item.d_PagoPaciente;

                            list.Add(oConsultaServicioDto);
                        }
                    }


                    return list;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static decimal CalcularTotalMedico(string pServiceId, string primerServiceId)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                    //Query tickets
                    var query = "select t.v_TicketId, t.v_ServiceId, td.v_TicketDetalleId, td.v_Descripcion, td.v_IdProductoDetalle,  td.v_CodInterno,  td.d_Cantidad,(select  d_PrecioVenta from [20505310072].[dbo].[producto] where v_CodInterno = td.v_CodInterno) as d_PrecioVenta, (select  d_PrecioVenta from [20505310072].[dbo].[producto] where v_CodInterno = td.v_CodInterno) * td.d_Cantidad as d_Total from ticket t inner join ticketdetalle td on t.v_TicketId = td.v_TicketId where t.v_ServiceId = '" + pServiceId + "' and t.i_ConCargoA = 1";
                    var tickets =  cnx.Query<TicketDto>(query).ToList();

                    decimal total = 0;
                    foreach (var ti in tickets)
                    {
                        total += ti.d_Total;
                    }


                    //query servicecomponent
                    var queryServiceComponent = " select case when Sum(r_Price) is null then 0 else  Sum(r_Price)  end   from servicecomponent sc " +
                                " where sc.v_ServiceId = '" + pServiceId + "' and sc.i_ConCargoA = 1 and sc.i_IsDeleted = 0 and i_IsRequiredId =1";
                    var totalService = cnx.Query<decimal>(queryServiceComponent).FirstOrDefault();
                    total += totalService;

                    //Se le suma el costo de habitación al primer servicio
                    if (pServiceId == primerServiceId)
                    {
                        #region no borrar
                        //var querylHabitacion = "select hh.d_EndDate,hh.d_StartDate, sum(DATEDIFF(day,hh.d_StartDate , hh.d_EndDate)) as dias, sum(DATEDIFF(day,hh.d_StartDate , hh.d_EndDate) * hh.d_Precio) as total " + 
                        //                     " from hospitalizacionservice hs " +
                        //                     " inner join hospitalizacionhabitacion hh on hh.v_HopitalizacionId = hs.v_HopitalizacionId " +
                        //                     " where hs.v_ServiceId = '" + pServiceId + "' and hh.i_ConCargoA =1 and hs.i_IsDeleted = 0  and hh.i_IsDeleted = 0  group by hh.d_EndDate,hh.d_StartDate,hh.d_Precio";

                        #endregion                      
                        var querylHabitacion = "select sum(DATEDIFF(day,hh.d_StartDate , hh.d_EndDate) * hh.d_Precio)" +
                                             " from hospitalizacionservice hs " +
                                             " inner join hospitalizacionhabitacion hh on hh.v_HopitalizacionId = hs.v_HopitalizacionId " +
                                             " where hs.v_ServiceId = '" + pServiceId + "' and hh.i_ConCargoA =1 and hs.i_IsDeleted = 0  and hh.i_IsDeleted = 0  group by hh.d_EndDate,hh.d_StartDate,hh.d_Precio";
                     
                        var totalHabitacion = cnx.Query<decimal>(querylHabitacion).FirstOrDefault();
                        total += totalHabitacion;
                    }

                    return total;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static decimal CalcularTotalPaciente(string pServiceId)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                    var query = "select t.v_TicketId, t.v_ServiceId, td.v_TicketDetalleId, td.v_Descripcion, td.v_IdProductoDetalle,  td.v_CodInterno,  td.d_Cantidad,(select  d_PrecioVenta from [20505310072].[dbo].[producto] where v_CodInterno = td.v_CodInterno) as d_PrecioVenta, (select  d_PrecioVenta from [20505310072].[dbo].[producto] where v_CodInterno = td.v_CodInterno) * td.d_Cantidad as d_Total from ticket t inner join ticketdetalle td on t.v_TicketId = td.v_TicketId where t.v_ServiceId = '" + pServiceId + "' and t.i_ConCargoA = 2";
                    var tickets = cnx.Query<TicketDto>(query).ToList();

                    decimal total = 0;
                    foreach (var ti in tickets)
                    {
                        total += ti.d_Total;
                    }

                    var queryComponente = "select r_Price from servicecomponent where v_ServiceId = '" + pServiceId + "'";
                    var componentes = cnx.Query<ServiceComponentList>(queryComponente).ToList();
                    foreach (var pre in componentes)
                    {
                        total += decimal.Parse(pre.r_Price.ToString());
                    }

                    return total;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<LocationDto> GetLocationsByOrganizationId(string pstrOrganizationId)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                    var query = "select v_LocationId as 'LocationId', v_Name as 'Nombre' from location where v_OrganizationId = '" + pstrOrganizationId + "'";
                    return cnx.Query<LocationDto>(query).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<KeyValueDTO> GetOrganizationId(string pstrOrganizationId)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                    var query = "select v_OrganizationId as 'Id', v_Name as 'Value1' from organization where v_OrganizationId = '" + pstrOrganizationId + "'";
                    return cnx.Query<KeyValueDTO>(query).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string ObtenerOrganizationIdByRuc(string pstrRUC)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                    var q = "select v_OrganizationId from organization where i_IsDeleted = 0 and v_IdentificationNumber = '" + pstrRUC + "'";
                    var id = cnx.Query<string>(q).FirstOrDefault();
                    return id;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void LlenarComboEso(UltraComboEditor cbo)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                    var query = @"select i_ParameterId as 'EsoId', v_Value1 as 'Nombre' from systemparameter
								  where i_GroupId = 118 and i_IsDeleted = 0";

                    var data = cnx.Query<EsoDto>(query).ToList();
                    data.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Todos--" });

                    cbo.DataSource = data;
                    cbo.DisplayMember = "Nombre";
                    cbo.ValueMember = "EsoId";
                    cbo.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void LlenarComboGeso(UltraComboEditor cbo, string pstrEmpresaId, string pstrLocationId)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                    var query = "select v_GroupOccupationId as GroupOccupationId, g.v_Name as Nombre from groupoccupation g " +
                                "join location l on g.v_LocationId =l.v_LocationId " +
                                "join organization o on l.v_OrganizationId = o.v_OrganizationId " +
                                "where o.v_OrganizationId = '" + pstrEmpresaId + "' " +
                                "and l.v_LocationId = '" + pstrLocationId + "' " +
                                "and g.i_IsDeleted = 0";

                    var data = cnx.Query<GrupoOcupacionalDto>(query).ToList();
                    data.Insert(0, new GrupoOcupacionalDto { GroupOccupationId = "-1", Nombre = "--Todos--" });

                    cbo.DataSource = data;
                    cbo.DisplayMember = "Nombre";
                    cbo.ValueMember = "GroupOccupationId";
                    cbo.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string ObtenerArrayConcatenado(string[] strings)
        {
            try
            {
                return string.Join(", ", strings.Select(p => GetQuotedString(p)));
            }
            catch (Exception ex)
            {
                return "''";
            }
        }

        private static productoDto ObtenerServicioSigesoft()
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewContasolConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();
                    var query = @"select p.v_CodInterno, i_IdUnidadMedida, pd.v_IdProductoDetalle, l.v_NroCuentaVenta as NroCuenta from producto p
								join productodetalle pd on p.v_IdProducto = pd.v_IdProducto
								join linea l on p.v_IdLinea = l.v_IdLinea
								where pd.v_IdProductoDetalle = (select v_IdProductoDetalleFlete from configuracionempresa)";

                    var r = cnx.Query<productoDto>(query).FirstOrDefault();
                    return r;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static void ProcesarMedicinaPaciente(ComponentServiceDto componente)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewContasolConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();
                    if (!string.IsNullOrEmpty(componente.IdMedicina))
                    {
                        var producto = cnx.Query<productoDto>(@"select p.v_CodInterno, p.v_Descripcion, p.d_PrecioVenta from producto p
													join productodetalle pd on p.v_idproducto = pd.v_IdProducto
													where pd.v_IdProductoDetalle = @id", new { id = componente.IdMedicina }).FirstOrDefault();
                        if (producto == null) throw new Exception("Medicina Contasol no encontrada");

                        componente.Precio = decimal.Parse(componente.d_SaldoPaciente.ToString());
                        componente.Descripcion = producto.v_Descripcion;
                        componente.Codigo = producto.v_CodInterno;
                        return;
                    }

                    throw new Exception("Medicina Sigesoft no encontrada");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static void ProcesarMedicinaAseguradora(ComponentServiceDto componente)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewContasolConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();
                    if (!string.IsNullOrEmpty(componente.IdMedicina))
                    {
                        var producto = cnx.Query<productoDto>(@"select p.v_CodInterno, p.v_Descripcion, p.d_PrecioVenta from producto p
													join productodetalle pd on p.v_idproducto = pd.v_IdProducto
													where pd.v_IdProductoDetalle = @id", new { id = componente.IdMedicina }).FirstOrDefault();
                        if (producto == null) throw new Exception("Medicina Contasol no encontrada");

                        componente.Precio = decimal.Parse(componente.d_SaldoAseguradora.ToString());
                        componente.Descripcion = producto.v_Descripcion;
                        componente.Codigo = producto.v_CodInterno;
                        return;
                    }

                    throw new Exception("Medicina Sigesoft no encontrada");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static BindingList<ventadetalleDto> ObtenerDetalleVenta(List<FacturarHospitalizacion> data)
        {

            var lista = new List<ventadetalleDto>();
            foreach (var item in data)
            {                 
                var oventadetalleDto = new ventadetalleDto();

                var cant = 1;
                var pu = item.d_Total;
                var valorV = Math.Round((decimal) (cant * pu), 2, MidpointRounding.AwayFromZero);
                var igv = Math.Round(valorV * 0.18m, 2, MidpointRounding.AwayFromZero);
                var pv = valorV + igv;

                oventadetalleDto.i_Anticipio = 0;
                oventadetalleDto.i_IdAlmacen = 1;
                oventadetalleDto.i_IdCentroCosto = "0";
                oventadetalleDto.i_IdUnidadMedida = 15;
                oventadetalleDto.ProductoNombre = "Productos y Servicios de Hospitalización";
                oventadetalleDto.v_DescripcionProducto = "Productos y Servicios de Hospitalización";
                oventadetalleDto.v_IdProductoDetalle = "N001-PE000015780";
                oventadetalleDto.v_NroCuenta = "";
                oventadetalleDto.d_PrecioVenta = pv;
                oventadetalleDto.d_Igv = igv;
                oventadetalleDto.d_Cantidad = 1;
                oventadetalleDto.d_CantidadEmpaque = 1;
                oventadetalleDto.d_Precio = pu;
                oventadetalleDto.d_Valor = valorV;
                oventadetalleDto.d_ValorVenta = valorV;
                oventadetalleDto.d_PrecioImpresion = pu;
                oventadetalleDto.v_CodigoInterno = "ATMD02";
                oventadetalleDto.Empaque = 1;
                oventadetalleDto.UMEmpaque = "UND";
                oventadetalleDto.i_EsServicio = 1;
                oventadetalleDto.i_IdUnidadMedidaProducto = 15;
                oventadetalleDto.v_HopitalizacionId = item.v_HopitalizacionId;
                oventadetalleDto.ACargo = item.ACargo;

                lista.Add(oventadetalleDto);
            }
            return new BindingList<ventadetalleDto>(lista);
        }
        public static BindingList<ventadetalleDto> ObtenerDetalleVenta(string[] idServicios, TipoFacturacion tipoCalculo, decimal totalHospi, string pNroLiq, string modoDetalle)
        {
            try
            {
                if (tipoCalculo == TipoFacturacion.Aseguradora)
                {
                    #region Ocupacional
                    using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                    {
                        if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();
                        var prodServicio = ObtenerServicioSigesoft();

                        if (prodServicio == null) throw new Exception("No se especificó el servicio en la conf. de empresa");

                        var queryservicios = "select c.v_Name as 'Descripcion', s.r_Price - (s.r_Price * 20 /100) as 'Precio', pl.i_EsCoaseguro as 'EsCoaseguro',  " +
                                    "pl.i_EsDeducible as 'EsDeducible', " +
                                    "pl.d_Importe as 'ImporteDescontado', pl.v_IdUnidadProductiva as 'IdUnidadProductiva', " +
                                    "s.d_SaldoPaciente, " +
                                    "s.d_SaldoAseguradora " +
                                    "from servicecomponent s " +
                                    "left join component c on s.v_ComponentId = c.v_ComponentId  " +
                                    "left join service ss on s.v_ServiceId = ss.v_ServiceId " +
                                    "left join protocol p on ss.v_ProtocolId = p.v_ProtocolId " +
                                    "left join \"plan\" pl on p.v_ProtocolId = pl.v_ProtocoloId and s.v_IdUnidadProductiva = pl.v_IdUnidadProductiva " +
                                    "where s.v_ServiceId in (" + ObtenerArrayConcatenado(idServicios) + ")  " +
                                    "and s.i_IsDeleted = 0 " +
                                    "and s.r_Price > 0 " +
                                    "order by c.v_Name";


                        var tikects = "select " +
                                "td.v_CodInterno, pd.v_Descripcion,td.d_PrecioVenta, td.d_Cantidad, pl.i_EsCoaseguro, pl.i_EsDeducible, pd.v_IdLinea, pl.d_Importe, " +
                                "CASE WHEN  pl.i_EsDeducible = 1 THEN  (td.d_PrecioVenta * td.d_Cantidad * pl.d_Importe /100) ELSE (pl.d_Importe) END  as TotalPagarPaciente, " +
                                "CASE WHEN  pl.i_EsCoaseguro = 1 THEN  (td.d_PrecioVenta-(td.d_PrecioVenta * td.d_Cantidad * pl.d_Importe /100)) ELSE (td.d_PrecioVenta-pl.d_Importe) END  as TotalPagarAseguradora, " +
                                "td.d_SaldoPaciente, " +
                                "td.d_SaldoAseguradora, " +
                                "td.v_IdProductoDetalle, " +
                                "pd.i_IdUnidadMedida " +
                                "from ticket t " +
                                "inner join ticketdetalle td on  t.v_TicketId = td.v_TicketId  " +
                                "inner join service s on s.v_ServiceId = t.v_ServiceId " +
                                "inner join [20505310072].dbo.producto pd on pd.v_CodInterno = td.v_CodInterno " +
                                "left join protocol p on s.v_ProtocolId = p.v_ProtocolId " +
                                "left join [plan] pl on s.v_ProtocolId = pl.v_ProtocoloId and pd.v_IdLinea = pl.v_IdUnidadProductiva " +
                                "where t.v_ServiceId in (" + ObtenerArrayConcatenado(idServicios) + ") ";


                        
                        var querymedicinas = @"select pl.i_EsCoaseguro as 'EsCoaseguro',  r.v_IdProductoDetalle as 'IdMedicina',
								pl.i_EsDeducible as 'EsDeducible', pl.d_Importe as 'ImporteDescontado', 
								pl.v_IdUnidadProductiva as 'IdUnidadProductiva', r.d_Cantidad as 'Cantidad', r.d_SaldoPaciente, r.d_SaldoAseguradora
								from receta r
								left join diagnosticrepository d on r.v_DiagnosticRepositoryId = d.v_DiagnosticRepositoryId
								left join [service] s on d.v_ServiceId = s.v_ServiceId
								left join protocol p on s.v_ProtocolId = p.v_ProtocolId
								left join [plan] pl on p.v_ProtocolId = pl.v_ProtocoloId and r.v_IdUnidadProductiva = pl.v_IdUnidadProductiva
								where s.v_ServiceId in (" + ObtenerArrayConcatenado(idServicios) + ")  " +
                                    "and r.i_Lleva = 1";

                        var dataServicios = cnx.Query<ComponentServiceDto>(queryservicios).ToList();
                        var dataMedicinas = cnx.Query<ComponentServiceDto>(querymedicinas).ToList();
                        var datTickets = cnx.Query<TicketCobranza>(tikects).ToList();

                        var totalticket = datTickets.Sum(p => p.d_SaldoAseguradora);

                        dataServicios = dataServicios.Where(p => p.PrecioRedondeadoAseguradora > 0).ToList();
                        dataMedicinas.ForEach(p => ProcesarMedicinaAseguradora(p));
                        var data = dataServicios.Concat(dataMedicinas).ToList();
                        data.ForEach(p => p.TipoCalculo = tipoCalculo);
                        var dataAgrupada = data.GroupBy(g => new { id = g.IdMedicina, cod = g.Codigo, nombre = g.Descripcion, precio = g.PrecioRedondeadoAseguradora });
                        var result = dataAgrupada.Select(n =>
                        {
                            var cant = decimal.Parse("1.0");// n.Sum(p => p.Cantidad);
                            var pu = n.Key.precio;
                            var valorV = Math.Round(cant * pu, 2, MidpointRounding.AwayFromZero);
                            var igv = Math.Round(valorV * 0.18m, 2, MidpointRounding.AwayFromZero);
                            var pv = valorV + igv;
                            //var precioContra = dataMedicinas[0].ImporteDescontado ;
                            return new ventadetalleDto
                            {
                                i_Anticipio = 0,
                                i_IdAlmacen = 1,
                                i_IdCentroCosto = "0",
                                i_IdUnidadMedida = prodServicio.i_IdUnidadMedida ?? 5,
                                ProductoNombre = n.Key.nombre,
                                v_DescripcionProducto = n.Key.nombre,
                                v_IdProductoDetalle = string.IsNullOrEmpty(n.Key.id) ? prodServicio.v_IdProductoDetalle : n.Key.id,
                                v_NroCuenta = prodServicio.NroCuenta,
                                d_PrecioVenta = pv,
                                d_Igv = igv,
                                d_Cantidad = cant,
                                d_CantidadEmpaque = cant,
                                d_Precio = pu,
                                d_Valor = valorV,
                                d_ValorVenta = valorV,
                                d_PrecioImpresion = pu,
                                v_CodigoInterno = string.IsNullOrEmpty(n.Key.cod) ? prodServicio.v_CodInterno : n.Key.cod,
                                Empaque = 1,
                                UMEmpaque = "UND",
                                i_EsServicio = 1,
                                i_IdUnidadMedidaProducto = prodServicio.i_IdUnidadMedida ?? 5,
                                //d_PrecioContraparte = prodServicio.v_CodInterno.Contains("MED") == true ? valorV - precioContra : 0  // if(prodServicio.v_CodInterno.Contains("MED")){} else{valorV - precioContra } 
                            };
                        }).ToList();

                        var TicketsConPrecio = datTickets.FindAll(p => p.d_SaldoAseguradora != 0);
                        foreach (var item in TicketsConPrecio)
                        {
                            var cant1 = decimal.Parse("1.0");
                            var pu1 = decimal.Parse(totalticket.ToString());
                            var valorV1 = Math.Round(cant1 * pu1, 2, MidpointRounding.AwayFromZero);
                            var igv1 = Math.Round(valorV1 * 0.18m, 2, MidpointRounding.AwayFromZero);
                            var pv1 = valorV1 + igv1;

                            ventadetalleDto oventadetalleDto = new ventadetalleDto();
                            oventadetalleDto.i_Anticipio = 0;
                            oventadetalleDto.i_IdAlmacen = 1;
                            oventadetalleDto.i_IdCentroCosto = "0";
                            oventadetalleDto.i_IdUnidadMedida = item.i_IdUnidadMedida;
                            oventadetalleDto.ProductoNombre = item.v_Descripcion;// prodServicio.v_Descripcion;
                            oventadetalleDto.v_DescripcionProducto = item.v_Descripcion;
                            oventadetalleDto.v_IdProductoDetalle = item.v_IdProductoDetalle; //ya
                            oventadetalleDto.v_NroCuenta = prodServicio.NroCuenta;
                            oventadetalleDto.d_PrecioVenta = pv1;
                            oventadetalleDto.d_Igv = igv1;
                            oventadetalleDto.d_Cantidad = cant1;
                            oventadetalleDto.d_CantidadEmpaque = cant1;
                            oventadetalleDto.d_Precio = pu1;
                            oventadetalleDto.d_Valor = valorV1;
                            oventadetalleDto.d_ValorVenta = valorV1;
                            oventadetalleDto.d_PrecioImpresion = pu1;
                            oventadetalleDto.v_CodigoInterno = item.v_CodInterno;
                            oventadetalleDto.Empaque = 1;
                            oventadetalleDto.UMEmpaque = "UND";
                            oventadetalleDto.i_EsServicio = 1;
                            oventadetalleDto.i_IdUnidadMedidaProducto = item.i_IdUnidadMedida;
                            result.Add(oventadetalleDto);
                        }


                        return new BindingList<ventadetalleDto>(result);
                    }
                    #endregion

                }
                else if (tipoCalculo == TipoFacturacion.Ocupacional)
                {
                    #region Ocupacional
                    using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                    {
                        if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                        //var obj = "select sc.v_ComponentId, SUM(sc.r_Price) AS d_Precio , COUNT(sc.v_ComponentId) AS d_Cantidad,c.v_Name AS ProductoNombre, c.v_IdUnidadProductiva " +
                        //          " from service s " +
                        //          " inner join servicecomponent sc on sc.v_ServiceId = s.v_ServiceId " +
                        //          " inner join component c on c.v_ComponentId = sc.v_ComponentId " +
                        //          " where  s.v_NroLiquidacion = '" + pNroLiq + "' " +
                        //          " group by sc.v_ComponentId,c.v_Name,c.v_IdUnidadProductiva ";

                        //var dataServicios = cnx.Query<ventadetalleDto>(obj).ToList().FindAll(p => p.d_Precio.Value != 0);
                        //var result = new List<ventadetalleDto>();
                        //foreach (var item in dataServicios)
                        //{
                        //    ventadetalleDto oventadetalleDto = new ventadetalleDto();
                        //    oventadetalleDto.i_Anticipio = 0;
                        //    oventadetalleDto.i_IdAlmacen = 1;
                        //    oventadetalleDto.i_IdCentroCosto = "0";
                        //    oventadetalleDto.i_IdUnidadMedida = 15;
                        //    oventadetalleDto.ProductoNombre = item.ProductoNombre;// prodServicio.v_Descripcion;
                        //    oventadetalleDto.v_DescripcionProducto = item.ProductoNombre;
                        //    oventadetalleDto.v_IdProductoDetalle = "N001-PE000015780"; //ya
                        //    oventadetalleDto.v_NroCuenta = "";// prodServicio.NroCuenta;
                        //    oventadetalleDto.d_PrecioVenta = item.d_Precio;
                        //    oventadetalleDto.d_Igv = Math.Round(item.d_Precio.Value * 0.18m, 2, MidpointRounding.AwayFromZero);
                        //    oventadetalleDto.d_Cantidad = 1;// item.d_Cantidad;
                        //    oventadetalleDto.d_CantidadEmpaque = 1;// item.d_Cantidad;
                        //    oventadetalleDto.d_Precio = item.d_Precio;
                        //    oventadetalleDto.d_Valor = Math.Round(item.d_Cantidad.Value * item.d_Precio.Value, 2, MidpointRounding.AwayFromZero);
                        //    oventadetalleDto.d_ValorVenta = Math.Round(item.d_Cantidad.Value * item.d_Precio.Value, 2, MidpointRounding.AwayFromZero);
                        //    oventadetalleDto.d_PrecioImpresion = Math.Round(item.d_Cantidad.Value * item.d_Precio.Value, 2, MidpointRounding.AwayFromZero);
                        //    oventadetalleDto.v_CodigoInterno = "ATMD01";// item.v_IdUnidadProductiva;
                        //    oventadetalleDto.Empaque = 1;
                        //    oventadetalleDto.UMEmpaque = "UND";
                        //    oventadetalleDto.i_EsServicio = 1;
                        //    oventadetalleDto.i_IdUnidadMedidaProducto = item.i_IdUnidadMedida;
                        //    result.Add(oventadetalleDto);
                        //}

                        //return new BindingList<ventadetalleDto>(result);

                        var obj = "";
                        if (modoDetalle == "Consolidado")
                        {
                            obj = "select  s.v_NroLiquidacion, SUM(sc.r_Price) AS d_Precio , 'SERVICIOS DE EXAMENES MEDICOS OCUPACIONALES' AS ProductoNombre  " +
                                    " from service s " +
                                    " inner join servicecomponent sc on sc.v_ServiceId = s.v_ServiceId " +
                                    " inner join component c on c.v_ComponentId = sc.v_ComponentId " +
                                    " where  s.v_NroLiquidacion = '" + pNroLiq + "' " +
                                    " and sc.i_IsDeleted = 0 and sc.i_IsRequiredId = 1" +
                                    " group by s.v_NroLiquidacion ";
                        }
                        else{
                            obj = "select  s.v_NroLiquidacion, SUM(sc.r_Price) AS d_Precio, sp.v_Value1 AS ProductoNombre  " +
                                "from service s  " +
                                "inner join servicecomponent sc on sc.v_ServiceId = s.v_ServiceId  inner join component c on c.v_ComponentId = sc.v_ComponentId  " +
                                "inner join protocol p on s.v_ProtocolId = p.v_ProtocolId " +
                                "inner join systemparameter sp on p.i_EsoTypeId = sp.i_ParameterId and sp.i_GroupId = 118  "+
                                "where  s.v_NroLiquidacion = '" + pNroLiq + "' " +
                                " and sc.i_IsDeleted = 0 and sc.i_IsRequiredId = 1 " +
                                "group by p.i_EsoTypeId ,s.v_NroLiquidacion,sp.v_Value1 ";
                        }          




                        var dataServicios = cnx.Query<ventadetalleDto>(obj).ToList().FindAll(p => p.d_Precio.Value != 0);
                        var result = new List<ventadetalleDto>();
                        foreach (var item in dataServicios)
                        {
                            ventadetalleDto oventadetalleDto = new ventadetalleDto();
                            oventadetalleDto.i_Anticipio = 0;
                            oventadetalleDto.i_IdAlmacen = 1;
                            oventadetalleDto.i_IdCentroCosto = "0";
                            oventadetalleDto.i_IdUnidadMedida = 15;
                            oventadetalleDto.ProductoNombre = item.ProductoNombre;
                            oventadetalleDto.v_DescripcionProducto = item.ProductoNombre;
                            oventadetalleDto.v_IdProductoDetalle = "N001-PE000015780"; //ya
                            oventadetalleDto.v_NroCuenta = "";// prodServicio.NroCuenta;
                            oventadetalleDto.d_PrecioVenta = item.d_Precio;
                            oventadetalleDto.d_Igv = Math.Round(item.d_Precio.Value * 0.18m, 2, MidpointRounding.AwayFromZero);
                            oventadetalleDto.d_Cantidad = 1;// item.d_Cantidad;
                            oventadetalleDto.d_CantidadEmpaque = 1;// item.d_Cantidad;
                            oventadetalleDto.d_Precio = item.d_Precio;
                            oventadetalleDto.d_Valor = Math.Round(1 * item.d_Precio.Value, 2, MidpointRounding.AwayFromZero);
                            oventadetalleDto.d_ValorVenta = Math.Round(1 * item.d_Precio.Value, 2, MidpointRounding.AwayFromZero);
                            oventadetalleDto.d_PrecioImpresion = Math.Round(1 * item.d_Precio.Value, 2, MidpointRounding.AwayFromZero);
                            oventadetalleDto.v_CodigoInterno = "ATMD04";// item.v_IdUnidadProductiva;
                            oventadetalleDto.Empaque = 1;
                            oventadetalleDto.UMEmpaque = "UND";
                            oventadetalleDto.i_EsServicio = 1;
                            oventadetalleDto.i_IdUnidadMedidaProducto = item.i_IdUnidadMedida;
                            result.Add(oventadetalleDto);
                        }

                        return new BindingList<ventadetalleDto>(result);
                    }
                    #endregion
                }
                else if (tipoCalculo == TipoFacturacion.Asistencial)
                {
                    #region Asistencial
                    using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                    {
                        if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();
                        var prodServicio = ObtenerServicioSigesoft();

                        if (prodServicio == null) throw new Exception("No se especificó el servicio en la conf. de empresa");

                        var queryservicios = "select c.v_Name as 'Descripcion', CASE WHEN pl.i_EsDeducible is NULL THEN s.r_Price ELSE	s.r_Price * pl.i_EsDeducible /100 END as 'Precio', pl.i_EsCoaseguro as 'EsCoaseguro',  " +
                                    "pl.i_EsDeducible as 'EsDeducible', " +
                                    "pl.d_Importe as 'ImporteDescontado', pl.v_IdUnidadProductiva as 'IdUnidadProductiva', " +
                                    "s.d_SaldoPaciente, " +
                                    "s.d_SaldoAseguradora " +
                                    "from servicecomponent s " +
                                    "left join component c on s.v_ComponentId = c.v_ComponentId  " +
                                    "left join service ss on s.v_ServiceId = ss.v_ServiceId " +
                                    "left join protocol p on ss.v_ProtocolId = p.v_ProtocolId " +
                                    "left join \"plan\" pl on p.v_ProtocolId = pl.v_ProtocoloId and s.v_IdUnidadProductiva = pl.v_IdUnidadProductiva " +
                                    "where s.v_ServiceId in (" + ObtenerArrayConcatenado(idServicios) + ")  " +
                                    "and s.i_IsDeleted = 0 " +
                                    "and s.r_Price > 0 " +
                                    "order by c.v_Name";


                        var tikects = "select " +
                                "td.v_CodInterno, pd.v_Descripcion,td.d_PrecioVenta, td.d_Cantidad, pl.i_EsCoaseguro, pl.i_EsDeducible, pd.v_IdLinea, pl.d_Importe, " +
                                "CASE WHEN  pl.i_EsDeducible = 1 THEN  (td.d_PrecioVenta * td.d_Cantidad * pl.d_Importe /100) ELSE (pl.d_Importe) END  as TotalPagarPaciente, " +
                                "CASE WHEN  pl.i_EsCoaseguro = 1 THEN  (td.d_PrecioVenta-(td.d_PrecioVenta * td.d_Cantidad * pl.d_Importe /100)) ELSE (td.d_PrecioVenta-pl.d_Importe) END  as TotalPagarAseguradora, " +
                                "td.d_SaldoPaciente, " +
                                "td.d_SaldoAseguradora, " +
                                "td.v_IdProductoDetalle, " +
                                "pd.i_IdUnidadMedida " +
                                "from ticket t " +
                                "inner join ticketdetalle td on  t.v_TicketId = td.v_TicketId  " +
                                "inner join service s on s.v_ServiceId = t.v_ServiceId " +
                                "inner join [20505310072].dbo.producto pd on pd.v_CodInterno = td.v_CodInterno " +
                                "left join protocol p on s.v_ProtocolId = p.v_ProtocolId " +
                                "left join [plan] pl on s.v_ProtocolId = pl.v_ProtocoloId and pd.v_IdLinea = pl.v_IdUnidadProductiva " +
                                "where t.v_ServiceId in (" + ObtenerArrayConcatenado(idServicios) + ") ";

                        var querymedicinas = @"select pl.i_EsCoaseguro as 'EsCoaseguro',  r.v_IdProductoDetalle as 'IdMedicina',
								pl.i_EsDeducible as 'EsDeducible', pl.d_Importe as 'ImporteDescontado', 
								pl.v_IdUnidadProductiva as 'IdUnidadProductiva', r.d_Cantidad as 'Cantidad', r.d_SaldoPaciente, r.d_SaldoAseguradora
								from receta r
								left join diagnosticrepository d on r.v_DiagnosticRepositoryId = d.v_DiagnosticRepositoryId
								left join [service] s on d.v_ServiceId = s.v_ServiceId
								left join protocol p on s.v_ProtocolId = p.v_ProtocolId
								left join [plan] pl on p.v_ProtocolId = pl.v_ProtocoloId and r.v_IdUnidadProductiva = pl.v_IdUnidadProductiva
								where s.v_ServiceId in (" + ObtenerArrayConcatenado(idServicios) + ")  " +
                                    "and r.i_Lleva = 1";

                        var dataServicios = cnx.Query<ComponentServiceDto>(queryservicios).ToList();
                        var dataMedicinas = cnx.Query<ComponentServiceDto>(querymedicinas).ToList();
                        var datTickets = cnx.Query<TicketCobranza>(tikects).ToList();

                        var totalticket = datTickets.Sum(p => p.d_SaldoPaciente);
                        dataServicios = dataServicios.Where(p => p.PrecioRedondeadoPaciente > 0).ToList();
                        dataMedicinas.ForEach(p => ProcesarMedicinaPaciente(p));
                        var data = dataServicios.Concat(dataMedicinas).ToList();
                        data.ForEach(p => p.TipoCalculo = tipoCalculo);
                        var dataAgrupada = data.GroupBy(g => new { id = g.IdMedicina, cod = g.Codigo, nombre = g.Descripcion, precio = g.PrecioRedondeadoPaciente });
                        var result = dataAgrupada.Select(n =>
                        {
                            var cant = decimal.Parse("1.0"); // n.Sum(p => p.Cantidad);
                            var pu = n.Key.precio;
                            var valorV = n.Key.precio;// Math.Round(cant * pu, 2, MidpointRounding.AwayFromZero);
                            var igv = Math.Round(valorV * 0.18m, 2, MidpointRounding.AwayFromZero);
                            var pv = valorV + igv;
                            //var preContra = dataMedicinas.Count == 0 ? 0 : dataMedicinas[0].ImporteDescontado ;
                            return new ventadetalleDto
                            {
                                i_Anticipio = 0,
                                i_IdAlmacen = 1,
                                i_IdCentroCosto = "0",
                                i_IdUnidadMedida = prodServicio.i_IdUnidadMedida ?? 5,
                                ProductoNombre = n.Key.nombre,
                                v_DescripcionProducto = n.Key.nombre,
                                v_IdProductoDetalle = string.IsNullOrEmpty(n.Key.id) ? prodServicio.v_IdProductoDetalle : n.Key.id,
                                v_NroCuenta = prodServicio.NroCuenta,
                                d_PrecioVenta = pv,
                                d_Igv = igv,
                                d_Cantidad = cant,
                                d_CantidadEmpaque = cant,
                                d_Precio = pu,
                                d_Valor = valorV,
                                d_ValorVenta = valorV,
                                d_PrecioImpresion = pu,
                                v_CodigoInterno = string.IsNullOrEmpty(n.Key.cod) ? prodServicio.v_CodInterno : n.Key.cod,
                                Empaque = 1,
                                UMEmpaque = "UND",
                                i_EsServicio = 1,
                                i_IdUnidadMedidaProducto = prodServicio.i_IdUnidadMedida ?? 5,
                                //d_PrecioContraparte = prodServicio.v_CodInterno.Substring(0,2).Contains("MED") == true ? preContra : 0 //preContra
                            };
                        }).ToList();

                        var TicketsConPrecio = datTickets.FindAll(p => p.d_SaldoPaciente != 0);
                        foreach (var item in TicketsConPrecio)
                        {
                            var cant1 = decimal.Parse("1.0");
                            var pu1 = decimal.Parse(totalticket.ToString());
                            var valorV1 = Math.Round(cant1 * pu1, 2, MidpointRounding.AwayFromZero);
                            var igv1 = Math.Round(valorV1 * 0.18m, 2, MidpointRounding.AwayFromZero);
                            var pv1 = valorV1 + igv1;

                            ventadetalleDto oventadetalleDto = new ventadetalleDto();
                            oventadetalleDto.i_Anticipio = 0;
                            oventadetalleDto.i_IdAlmacen = 1;
                            oventadetalleDto.i_IdCentroCosto = "0";
                            oventadetalleDto.i_IdUnidadMedida = item.i_IdUnidadMedida;
                            oventadetalleDto.ProductoNombre = item.v_Descripcion;// prodServicio.v_Descripcion;
                            oventadetalleDto.v_DescripcionProducto = item.v_Descripcion;
                            oventadetalleDto.v_IdProductoDetalle = item.v_IdProductoDetalle; //ya
                            oventadetalleDto.v_NroCuenta = prodServicio.NroCuenta;
                            oventadetalleDto.d_PrecioVenta = pv1;
                            oventadetalleDto.d_Igv = igv1;
                            oventadetalleDto.d_Cantidad = cant1;
                            oventadetalleDto.d_CantidadEmpaque = cant1;
                            oventadetalleDto.d_Precio = pu1;
                            oventadetalleDto.d_Valor = valorV1;
                            oventadetalleDto.d_ValorVenta = valorV1;
                            oventadetalleDto.d_PrecioImpresion = pu1;
                            oventadetalleDto.v_CodigoInterno = item.v_CodInterno;
                            oventadetalleDto.Empaque = 1;
                            oventadetalleDto.UMEmpaque = "UND";
                            oventadetalleDto.i_EsServicio = 1;
                            oventadetalleDto.i_IdUnidadMedidaProducto = item.i_IdUnidadMedida;
                            result.Add(oventadetalleDto);
                        }

                        //ventadetalleDto oventadetalleDto = new ventadetalleDto();
                        //oventadetalleDto.i_Anticipio = 0;
                        //oventadetalleDto.i_IdAlmacen = 1;
                        //oventadetalleDto.i_IdCentroCosto = "0";
                        //oventadetalleDto.i_IdUnidadMedida = "NOSE";
                        //oventadetalleDto.ProductoNombre = "NOSE";
                        //oventadetalleDto.v_DescripcionProducto = "NOSE";
                        //oventadetalleDto.v_IdProductoDetalle = "NOSE";
                        //oventadetalleDto.v_NroCuenta = "NOSE";
                        //oventadetalleDto.d_PrecioVenta = pv1;
                        //oventadetalleDto.d_Igv = igv1;
                        //oventadetalleDto.d_Cantidad = cant1;
                        //oventadetalleDto.d_CantidadEmpaque = cant1;
                        //oventadetalleDto.d_Precio = pu1;
                        //oventadetalleDto.d_Valor = valorV1;
                        //oventadetalleDto.d_ValorVenta = valorV1;
                        //oventadetalleDto.d_PrecioImpresion = pu1;
                        //oventadetalleDto.v_CodigoInterno = item.v_CodInterno;
                        //oventadetalleDto.Empaque = 1;
                        //oventadetalleDto.UMEmpaque = "UND";
                        //oventadetalleDto.i_EsServicio = 1;
                        //oventadetalleDto.i_IdUnidadMedidaProducto = item.i_IdUnidadMedida;
                        //result.Add(oventadetalleDto);
                        //return new BindingList<ventadetalleDto>(result);
                    }
                    #endregion
                }
                else if (tipoCalculo == TipoFacturacion.Hospitalizacion)
                {
                    var lista = new List<ventadetalleDto>();
                   var oventadetalleDto = new ventadetalleDto();

                   var cant = 1;
                   var pu = totalHospi;
                   var valorV = Math.Round((decimal) (cant * pu), 2, MidpointRounding.AwayFromZero);
                   var igv = Math.Round(valorV * 0.18m, 2, MidpointRounding.AwayFromZero);
                   var pv = valorV + igv;

                    oventadetalleDto.i_Anticipio = 0;
                     oventadetalleDto.i_IdAlmacen = 1;
                                oventadetalleDto.i_IdCentroCosto = "0";
                                oventadetalleDto.i_IdUnidadMedida = 15;
                                oventadetalleDto.ProductoNombre = "Productos y Servicios de Hospitalización";
                                oventadetalleDto.v_DescripcionProducto = "Productos y Servicios de Hospitalización";
                                oventadetalleDto.v_IdProductoDetalle = "N001-PE000015780";
                                oventadetalleDto.v_NroCuenta = "";
                                oventadetalleDto.d_PrecioVenta = pv;
                                oventadetalleDto.d_Igv = igv;
                                oventadetalleDto.d_Cantidad = 1;
                                oventadetalleDto.d_CantidadEmpaque = 1;
                                oventadetalleDto.d_Precio = pu;
                                oventadetalleDto.d_Valor = valorV;
                                oventadetalleDto.d_ValorVenta = valorV;
                                oventadetalleDto.d_PrecioImpresion = pu;
                                oventadetalleDto.v_CodigoInterno = "ATMD01";
                                oventadetalleDto.Empaque = 1;
                                oventadetalleDto.UMEmpaque = "UND";
                                oventadetalleDto.i_EsServicio = 1;
                    oventadetalleDto.i_IdUnidadMedidaProducto = 15;

                    lista.Add(oventadetalleDto);

                   return new BindingList<ventadetalleDto>(lista);
                }

                return new BindingList<ventadetalleDto>(new List<ventadetalleDto>());
            }
            catch (Exception)
            {

                throw;
            }
        }
        
        public static bool ProcesarServicio(TipoFacturacion tipoFacturacion, TipoAccionFacturacion tipoAccion, string pstrServiceId, string pstrVentaId = null)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();
                    if (tipoAccion == TipoAccionFacturacion.Facturar && string.IsNullOrEmpty(pstrVentaId)) throw new Exception("Id de la venta requerida para esta accion");
                    var columna = tipoFacturacion == TipoFacturacion.Aseguradora ? "v_IdVentaAseguradora" : "v_IdVentaCliente";
                    var valor = tipoAccion == TipoAccionFacturacion.Anular ? "null" : GetQuotedString(pstrVentaId);
                    var qry = "update service set " + columna + " = " + valor + " where v_ServiceId = " + GetQuotedString(pstrServiceId) + ";";
                    cnx.Execute(qry);
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void ActualizarFacturacionServicio(int tipoFact, List<string> servicios)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();
                foreach (var id in servicios)
                {
                    if (tipoFact == 1)
                    {
                        var qry = "update service set i_IsFac = 1  where v_ServiceId = '" + id + "'";
                        cnx.Execute(qry);
                    }
                    else if (tipoFact == 2)
                    {
                        var qry = "update service set i_IsFacMedico = 1  where v_ServiceId = '" + id + "'";
                        cnx.Execute(qry);
                    }
                }
               
               
            }
        }

        public static void ActualizarFacturacionReceta(string comprobante, string servicios)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();
                if (comprobante != null && servicios != null)
                {
                    var qry = "update receta set v_Comprobante = '" + comprobante + " '  where v_ServiceId = '" + servicios + "'";
                    cnx.Execute(qry);
                }   
            }
        }

        public static void ActualizarTicketHospi(string comprobante, string ticket)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();
                if (comprobante != null && ticket != null)
                {
                    var qry = "update ticket set v_Comprobante = '" + comprobante + " '  where v_TicketId = '" + ticket + "'";
                    cnx.Execute(qry);
                }


            }
        }
        public static List<FacturarHospitalizacion> FacturarHospitalizacion(string dni, DateTime? fechaInicio, DateTime? fechaFin, string cobarA)
        {

            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query = @"select DISTINCT s.v_HopitalizacionId , p.v_FirstName + ' ' + p.v_FirstLastName + ' ' + p.v_SecondLastName AS v_Pacient, s.d_FechaIngreso AS d_FechaIngreso, s.d_PagoMedico AS d_PagoMedico, s.d_PagoPaciente AS d_PagoPaciente, i_MedicoPago, i_PacientePago  from hospitalizacion s INNER JOIN person p on  s.v_PersonId = p.v_PersonId join hospitalizacionservice hs on s.v_HopitalizacionId = hs.v_HopitalizacionId
                                join service ss on hs.v_ServiceId = ss.v_ServiceId
                                join calendar c on ss.v_ServiceId = c.v_ServiceId" +
                                " where ( CAST(CAST( s.d_FechaIngreso AS datetime2) AS datetime2) >= convert(datetime2,'" + fechaInicio.Value.Date.ToString("yyyy-MM-dd") + "' , 121)) AND ( CAST(CAST( s.d_FechaIngreso AS datetime2) AS datetime2) <= convert(datetime2, '" + fechaFin.Value.Date.ToString("yyyy-MM-dd") + "', 121)) " +
                                " AND p.v_DocNumber = '" + dni + "' and c.i_CalendarStatusId <> 4";

                    var result  =  cnx.Query<FacturarHospitalizacion>(query).ToList();      

                    var list = new List<FacturarHospitalizacion>();
                    foreach (var item in result)
	                {
		                var oFacturarHospitalizacion = new FacturarHospitalizacion();

                        oFacturarHospitalizacion.v_HopitalizacionId = item.v_HopitalizacionId;
                        oFacturarHospitalizacion.v_Pacient = item.v_Pacient;
                        oFacturarHospitalizacion.d_FechaIngreso = item.d_FechaIngreso;
                        oFacturarHospitalizacion.i_MedicoPago = item.i_MedicoPago;
                        oFacturarHospitalizacion.i_PacientePago = item.i_PacientePago;
                        if (cobarA == "Paciente") { oFacturarHospitalizacion.d_Total = item.d_PagoPaciente; oFacturarHospitalizacion.ACargo = "Paciente"; }
                        else { oFacturarHospitalizacion.d_Total = item.d_PagoMedico; oFacturarHospitalizacion.ACargo = "Medico"; }
                        list.Add(oFacturarHospitalizacion);
	                }
                    return list;
                }
            }
            catch (Exception)
            {
                
                throw;
            }

             
        }

        public static List<SaldoPaciente> SaldoPacienteAseguradora(string serviceId)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query = @"select SC.d_SaldoPaciente, CP.v_Name from service SR " +
                                "inner join servicecomponent SC " +
                                "ON SR.v_ServiceId = SC.v_ServiceId  " +
                                "inner join component CP "+
                                "ON CP.v_ComponentId=SC.v_ComponentId "+
                                "where SR.v_ServiceId='" + serviceId + "' and SC.d_SaldoPaciente<>0.00";
                    var result = cnx.Query<SaldoPaciente>(query).ToList();


                    //var list = new List<SaldoPaciente>();
                    //foreach (var item in result)
                    //{
                    //    var oSaldoPaciente = new SaldoPaciente();
                    //    oSaldoPaciente.d_SaldoPaciente = item.d_SaldoPaciente;
                    //    oSaldoPaciente.v_Name = item.v_Name;
                    //    list.Add(oSaldoPaciente);
                    //}
                    //return list;

                    return result;
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public static List<SaldoPaciente> DetalleVenta(string serviceId)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query = @"select SC.r_Price as d_SaldoPaciente, CP.v_Name from service SR " +
                                "inner join servicecomponent SC " +
                                "ON SR.v_ServiceId = SC.v_ServiceId  " +
                                "inner join component CP " +
                                "ON CP.v_ComponentId=SC.v_ComponentId " +
                                "where SR.v_ServiceId='" + serviceId + "' and SC.r_Price<>0.00";
                    var result = cnx.Query<SaldoPaciente>(query).ToList();


                    //var list = new List<SaldoPaciente>();
                    //foreach (var item in result)
                    //{
                    //    var oSaldoPaciente = new SaldoPaciente();
                    //    oSaldoPaciente.d_SaldoPaciente = item.d_SaldoPaciente;
                    //    oSaldoPaciente.v_Name = item.v_Name;
                    //    list.Add(oSaldoPaciente);
                    //}
                    //return list;

                    return result;
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public static float ExamenesSinFacturar(string serviceId)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                var query = "select ISNULL(SUM(SC.r_Price),0) from servicecomponent  SC  where SC.v_ServiceId='" + serviceId +
                            "' and (SC.i_IsRequiredId = 1 )";

                var result = cnx.Query<float>(query).FirstOrDefault();

                return result;
            }


        }
    }

    public class SaldoPaciente
    {
        public float? d_SaldoPaciente { get; set; }
        public string v_Name { get; set; }
    }

    public class FacturarHospitalizacion
    {
        public string v_HopitalizacionId { get; set; }
        public string v_Pacient { get; set; }
        public DateTime? d_FechaIngreso { get; set; }
        public decimal? d_PagoMedico { get; set; }
        public decimal? d_PagoPaciente { get; set; }    
        public decimal? d_Total { get; set; }

        public int? i_MedicoPago { get; set; }
        public int? i_PacientePago { get; set; }
        public string ACargo { get; set; }
    }


    public enum TipoAccionFacturacion
    {
        Facturar,
        Anular
    }

    public enum TipoFacturacion
    {
        Ocupacional = 2,
        Asistencial = 3,
        Aseguradora = 10,
        Hospitalizacion = 19
    }

   


}
